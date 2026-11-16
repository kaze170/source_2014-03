// tests/TestServer.cpp - end to end tests of the single process server over real sockets.
//
// Boots knc::server::Server on ephemeral ports against a temporary database and drives the 2014
// login/channel packet flow with a raw client (same framing + KSecureBuffer handshake as the
// original client): EGS_CONNECT_REQ, EGS_VERIFY_ACCOUNT_REQ, EGS_GET_SERVER_SET_DATA_REQ,
// EGS_MY_UNIT_AND_INVENTORY_INFO_LIST_REQ, EGS_CREATE_UNIT_REQ, EGS_SELECT_UNIT_REQ.
#if defined(_WIN32)
#include <winsock2.h>
#include <ws2tcpip.h>
#else
#include <arpa/inet.h>
#include <netinet/in.h>
#include <sys/socket.h>
#include <unistd.h>
#endif

#include <algorithm>
#include <atomic>
#include <chrono>
#include <cstdio>
#include <cstring>
#include <memory>
#include <thread>
#include <vector>

#include "TestFramework.h"
#include "kcore/IniFile.h"
#include "kcore/Log.h"
#include "kcore/Serialize.h"
#include "kproto/Event.h"
#include "kproto/LoginPackets.h"
#include "kproto/Packet.h"
#include "kproto/UnitPackets.h"
#include "ksec/SecureBuffer.h"
#include "kdb/Schema.h"
#include "kserver/Config.h"
#include "kserver/LoginService.h"
#include "kserver/Server.h"

using namespace knc;
using namespace knc::server;

namespace {

unsigned long currentProcessId() {
#if defined(_WIN32)
    return static_cast<unsigned long>(::GetCurrentProcessId());
#else
    return static_cast<unsigned long>(::getpid());
#endif
}

unsigned short pickTcpPort() {
    const net::SocketHandle sock = ::socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
    sockaddr_in addr{};
    addr.sin_family = AF_INET;
    addr.sin_port = 0;
    addr.sin_addr.s_addr = htonl(INADDR_LOOPBACK);
    ::bind(sock, reinterpret_cast<sockaddr*>(&addr), sizeof(addr));
#if defined(_WIN32)
    int len = static_cast<int>(sizeof(addr));
#else
    socklen_t len = sizeof(addr);
#endif
    ::getsockname(sock, reinterpret_cast<sockaddr*>(&addr), &len);
    const unsigned short port = ntohs(addr.sin_port);
    net::closeSocket(sock);
    return port;
}

// Builds a framed packet: [u16_le length][sealed payload]. Every test client needs this, and doing
// it by hand trips -Warray-bounds because the compiler cannot see through the vector growth.
std::vector<byte> makeFrame(const sec::SecureBuffer& secure) {
    const std::size_t total = 2 + secure.size();
    std::vector<byte> frame(total);
    frame[0] = static_cast<byte>(total & 0xff);
    frame[1] = static_cast<byte>((total >> 8) & 0xff);
    std::copy_n(secure.dataPtr(), secure.size(), frame.begin() + 2);
    return frame;
}

// A raw 2014-protocol client: framing, handshake, sealed events, KEvent decode.
class RawClient {
public:
    ~RawClient() { close(); }

    bool connectTo(unsigned short port) {
        sock_ = ::socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
        if (sock_ == net::kInvalidSocket) return false;
        sockaddr_in addr{};
        addr.sin_family = AF_INET;
        addr.sin_port = htons(port);
        addr.sin_addr.s_addr = htonl(INADDR_LOOPBACK);
        if (::connect(sock_, reinterpret_cast<sockaddr*>(&addr), sizeof(addr)) != 0) return false;
        // Short receive timeout so the "no reply expected" cases can time out instead of blocking.
        if (!net::setRecvTimeout(sock_, 20)) return false;
        return true;
    }

    bool handshake(int timeoutMs = 3000) {
        const auto deadline = std::chrono::steady_clock::now() + std::chrono::milliseconds(timeoutMs);
        while (std::chrono::steady_clock::now() < deadline) {
            if (readFrame(25)) {
                Event accept;
                if (!decodeLast(0, accept)) return false;
                if (accept.eventId != E_ACCEPT_CONNECTION_NOT) return false;
                SerBuffer copy = accept.payload;
                Serializer s;
                s.beginReading(&copy);
                sec::SecurityAssociation sa;
                s.get(spi, sa);
                s.end();
                return s.ok() && spi != 0;
            }
        }
        return false;
    }

    template <class T>
    bool sendPacket(SpiType useSpi, EventId id, const T& payload) {
        Event event;
        event.setData(kPiGsProxy, nullptr, id);
        encodePayload(payload, event.payload);
        return sendEvent(useSpi, event);
    }

    bool sendEvent(SpiType useSpi, Event& event) {
        SerBuffer wire;
        Serializer s;
        s.beginWriting(&wire);
        s.put(event);
        s.end();
        sec::SecureBuffer secure(useSpi);
        if (!secure.create_notRWM(wire.data(), wire.size())) return false;
        const std::vector<byte> frame = makeFrame(secure);
        std::size_t off = 0;
        while (off < frame.size()) {
            const auto n = ::send(sock_, reinterpret_cast<const char*>(frame.data() + off),
                                  static_cast<int>(frame.size() - off), 0);
            if (n <= 0) return false;
            off += static_cast<std::size_t>(n);
        }
        return true;
    }

    // Waits for the next frame and decodes it. Returns false on timeout/close.
    bool receive(Event& out, int timeoutMs = 3000) {
        const auto deadline = std::chrono::steady_clock::now() + std::chrono::milliseconds(timeoutMs);
        while (std::chrono::steady_clock::now() < deadline) {
            if (readFrame(20)) return decodeLast(spi, out, false);
        }
        return false;
    }

    // True when nothing arrives within timeoutMs (used to assert that a request is refused).
    bool expectSilence(int timeoutMs = 400) {
        Event ignored;
        return !receive(ignored, timeoutMs);
    }

    void close() {
        if (sock_ != net::kInvalidSocket) {
            net::closeSocket(sock_);
            sock_ = net::kInvalidSocket;
        }
    }

    SpiType spi = 0;

private:
    bool readFrame(int timeoutMs) {
        const auto deadline = std::chrono::steady_clock::now() + std::chrono::milliseconds(timeoutMs);
        while (std::chrono::steady_clock::now() < deadline) {
            if (inBuf_.size() >= 2) {
                std::uint16_t len = 0;
                std::memcpy(&len, inBuf_.data(), 2);
                if (len >= 2 && inBuf_.size() >= len) {
                    frame_.assign(inBuf_.begin() + 2, inBuf_.begin() + len);
                    inBuf_.erase(inBuf_.begin(), inBuf_.begin() + len);
                    return true;
                }
            }
            byte chunk[4096];
            const int n = ::recv(sock_, reinterpret_cast<char*>(chunk), sizeof(chunk), 0);
            if (n == 0) return false;              // peer closed
            if (n < 0) continue;                   // SO_RCVTIMEO expired: keep waiting for the deadline
            inBuf_.insert(inBuf_.end(), chunk, chunk + n);
        }
        return false;
    }

    bool decodeLast(SpiType useSpi, Event& out, bool checkSeq = false) {
        sec::SecureBuffer buffer(useSpi, frame_.data(), frame_.size());
        if (!buffer.isAuthentic_notRWM(checkSeq)) return false;
        Bytes payload;
        if (!buffer.getPayload_notRWM(payload, false, checkSeq)) return false;
        SerBuffer buf;
        buf.assign(payload.data(), payload.size());
        Serializer s;
        s.beginReading(&buf);
        s.get(out);
        s.end();
        out.payload.uncompress();
        return s.ok();
    }

    net::SocketHandle sock_ = net::kInvalidSocket;
    std::vector<byte> inBuf_;
    std::vector<byte> frame_;
};

std::string tempDbPath() {
    static std::atomic<int> counter{0};
    char buffer[256];
    std::snprintf(buffer, sizeof(buffer), "build/test_knc_%lu_%d.db", currentProcessId(),
                  counter.fetch_add(1));
    return buffer;
}

// Boots a Server on ephemeral ports with a temp database and runs its reactor on a thread.
class ServerFixture {
public:
    ServerFixture() : path_(tempDbPath()) {
        config_ = Config::defaults();
        config_.logLevel = LogLevel::kWarn;
        config_.seedTestAccounts = true;
        config_.migrateOnStart = true;
        config_.database.path = path_;
        config_.login.port = pickTcpPort();
        config_.channel.port = pickTcpPort();
        config_.game.enabled = false;
        const unsigned short channelPort = config_.channel.port;
        config_.channels.front().masterPort = channelPort;
        config_.channels.front().ncUdpPort = 9151;
        config_.serverSets.front().port = channelPort;
        server_ = std::make_unique<Server>(config_);
    }
    ~ServerFixture() {
        stop();
        // Leave nothing behind: the database and its WAL side files are per-run scratch data.
        std::remove(path_.c_str());
        std::remove((path_ + "-wal").c_str());
        std::remove((path_ + "-shm").c_str());
    }

    bool start() {
        std::string error;
        if (!server_->start(&error)) {
            std::fprintf(stderr, "server start failed: %s\n", error.c_str());
            return false;
        }
        thread_ = std::thread([this] { server_->run(); });
        return true;
    }
    void stop() {
        if (!server_ || stopped_) return;
        stopped_ = true;
        server_->reactor().stop();
        if (thread_.joinable()) thread_.join();
        server_->stop();
    }

    Server& server() { return *server_; }
    unsigned short loginPort() const { return config_.login.port; }
    unsigned short channelPort() const { return config_.channel.port; }

private:
    std::string path_;
    Config config_;
    std::unique_ptr<Server> server_;
    std::thread thread_;
    bool stopped_ = false;
};

}  // namespace

// --------------------------------------------------------------------------- config
KTEST(Server, configParsesThe2014IniShape) {
    const char* text = R"(; comment
[Version]
Internal Version = 2014031300
OpenTest Version = 2014031300
Service Version  = 2014031300
UseVersion = internal

[Server]
Name = TestBox
LogLevel = debug
SeedTestAccounts = false

[Database]
Path = data/test.db
WAL = true

[Login]
Port = 7001
MaxUser = 25

[Channel]
Port = 7002
MaxUser = 40

[Channels]
Channel0 = 1 | Ch.1 | 127.0.0.1 | 7002 | 7051 | 40 | 10 | 5
Channel1 = 2 | Ch.2 | 127.0.0.1 | 7003 | 7052 | 40 | 0 | 0

[ServerSet]
Set0 = 1 | Test Server | 127.0.0.1 | 7002 | 40
)";
    Config config = Config::defaults();
    std::string error;
    KCHECK(config.loadFromText(text, "inline", &error));
    KCHECK_EQ(config.name, std::string("TestBox"));
    KCHECK(config.logLevel == LogLevel::kDebug);
    KCHECK(!config.seedTestAccounts);
    KCHECK_EQ(config.database.path, std::string("data/test.db"));
    KCHECK_EQ(config.login.port, static_cast<unsigned short>(7001));
    KCHECK_EQ(config.login.maxUser, 25);
    KCHECK_EQ(config.channel.port, static_cast<unsigned short>(7002));
    KCHECK_EQ(config.channels.size(), std::size_t(2));
    KCHECK_EQ(config.channels[0].channelId, 1);
    KCHECK_EQ(config.channels[0].displayName, u"Ch.1");
    KCHECK_EQ(config.channels[0].masterPort, static_cast<unsigned short>(7002));
    KCHECK_EQ(config.channels[0].ncUdpPort, static_cast<unsigned short>(7051));
    KCHECK_EQ(config.channels[0].bonusExpPercent, 10);
    KCHECK_EQ(config.channels[1].channelId, 2);
    KCHECK_EQ(config.serverSets.size(), std::size_t(1));
    KCHECK_EQ(config.serverSets[0].port, static_cast<unsigned short>(7002));
    KCHECK_EQ(config.version(), WideString(u"2014031300"));
    KCHECK(config.validate(&error));

    // A [Version] block without the announced string is rejected.
    Config broken = Config::defaults();
    broken.internalVersion.clear();
    broken.openTestVersion.clear();
    broken.serviceVersion.clear();
    KCHECK(!broken.validate(nullptr));
}

// --------------------------------------------------------------------------- login service
KTEST(Server, loginServiceAnswersTheHandshake) {
    // A database on its own, no sockets: the service is transport agnostic.
    db::Database database;
    db::Database::Config dbConfig;
    dbConfig.memory = true;
    std::string error;
    KCHECK(database.open(dbConfig, &error));
    KCHECK(db::applySchema(database).ok);
    KCHECK(db::seedDefaultAccounts(database, &error));
    auto repos = db::makeRepositories(database);

    Config config = Config::defaults();
    LoginService service(*repos, config);

    struct Recorder : LoginResponder {
        std::vector<std::pair<EventId, SerBuffer>> sent;
        std::string ip = "127.0.0.1";
        int users = 1;
        bool send(EventId id, const SerBuffer& payload) override {
            sent.emplace_back(id, payload);
            return true;
        }
        void disconnect(const std::string&) override {}
        const std::string& peerIp() const override { return ip; }
        int connectedUsers() const override { return users; }
    };

    AccountSession state;
    Recorder responder;

    // Wrong version -> ERR_CONNECT_00 (3).
    ConnectReq wrong;
    wrong.version = u"1999010100";
    SerBuffer payload;
    encodePayload(wrong, payload);
    KCHECK(!service.handleConnect(state, responder, payload));
    KCHECK_EQ(responder.sent.size(), std::size_t(1));
    ConnectAck ack;
    KCHECK(decodePayload(responder.sent.back().second, ack));
    KCHECK_EQ(ack.ok, static_cast<int>(kErrConnectVersion));

    // Correct version -> NET_OK, channel id and UDP port from the config.
    responder.sent.clear();
    ConnectReq good;
    good.version = config.version();
    encodePayload(good, payload);
    KCHECK(service.handleConnect(state, responder, payload));
    KCHECK_EQ(responder.sent.size(), std::size_t(1));
    KCHECK(decodePayload(responder.sent.back().second, ack));
    KCHECK_EQ(ack.ok, static_cast<int>(kNetOk));
    KCHECK_EQ(ack.channelId, 1);
    KCHECK_EQ(ack.udpPort, 9151);

    // Full server -> ERR_CONNECT_04 (4).
    responder.users = 100000;
    responder.sent.clear();
    KCHECK(!service.handleConnect(state, responder, payload));
    KCHECK(decodePayload(responder.sent.back().second, ack));
    KCHECK_EQ(ack.ok, static_cast<int>(kErrConnectFull));
    responder.users = 1;

    // Authentication: seeded account test/test.
    AccountSession loginState;
    loginState.state = LoginState::kConnectedClient;
    responder.sent.clear();
    UserAuthenticateReq auth;
    auth.userId = u"test";
    auth.password = u"test";
    auth.machineId = "machine-1";
    encodePayload(auth, payload);
    KCHECK(service.handleVerifyAccount(loginState, responder, payload));
    KCHECK(loginState.authenticated);
    VerifyAccountAck verifyAck;
    KCHECK(decodePayload(responder.sent.back().second, verifyAck));
    KCHECK_EQ(verifyAck.ok, static_cast<int>(kNetOk));
    KCHECK_EQ(verifyAck.accountInfo.id, WideString(u"test"));
    KCHECK(verifyAck.accountInfo.userUid > 0);
    KCHECK_EQ(service.successfulLogins(), 1LL);

    // Bad password -> ERR_VERIFY_06 (12), and the session stays unauthenticated.
    AccountSession badState;
    badState.state = LoginState::kConnectedClient;
    auth.password = u"wrong";
    encodePayload(auth, payload);
    responder.sent.clear();
    KCHECK(!service.handleVerifyAccount(badState, responder, payload));
    KCHECK(decodePayload(responder.sent.back().second, verifyAck));
    KCHECK_EQ(verifyAck.ok, static_cast<int>(kErrVerifyBadPassword));
    KCHECK(!badState.authenticated);

    // Unknown account -> ERR_VERIFY_04 (10).
    auth.userId = u"nobody";
    auth.password = u"test";
    encodePayload(auth, payload);
    responder.sent.clear();
    KCHECK(!service.handleVerifyAccount(badState, responder, payload));
    KCHECK(decodePayload(responder.sent.back().second, verifyAck));
    KCHECK_EQ(verifyAck.ok, static_cast<int>(kErrVerifyNoUser));

    // A blocked account answers ERR_VERIFY_11 (15) and carries the block reason.
    db::AccountRecord account;
    std::int64_t gmUid = 0;
    KCHECK(repos->accounts.authenticate("gm", "gm", "127.0.0.1", &account) == db::AuthOutcome::kOk);
    // A block is "active" when BlockEndTime is non-empty (kdb models it that way), so a permanent
    // block uses the far future rather than an empty string.
    KCHECK(repos->accounts.applyBlock(account.uid, "9999-12-31 23:59:59", "hacking", 0, "", ""));
    (void)gmUid;
    auth.userId = u"gm";
    auth.password = u"gm";
    encodePayload(auth, payload);
    responder.sent.clear();
    KCHECK(!service.handleVerifyAccount(badState, responder, payload));
    KCHECK(decodePayload(responder.sent.back().second, verifyAck));
    KCHECK_EQ(verifyAck.ok, static_cast<int>(kErrVerifyBlocked));
    KCHECK(verifyAck.accountInfo.blockInfo.blockReason == u"hacking");
}

// --------------------------------------------------------------------------- end to end
KTEST(Server, endToEndLoginAndUnitFlow) {
    KCHECK(net::startupSockets());
    ServerFixture fixture;
    KCHECK(fixture.start());

    // ---- login port: handshake, connect, verify ---------------------------------
    RawClient login;
    KCHECK(login.connectTo(fixture.loginPort()));
    KCHECK(login.handshake());

    ConnectReq connect;
    connect.version = WideString(u"2014031300");
    KCHECK(login.sendPacket(login.spi, EGS_CONNECT_REQ, connect));
    Event event;
    KCHECK(login.receive(event));
    KCHECK_EQ(event.eventId, EGS_CONNECT_ACK);
    ConnectAck connectAck;
    KCHECK(decodePayload(event.payload, connectAck));
    KCHECK_EQ(connectAck.ok, static_cast<int>(kNetOk));

    UserAuthenticateReq auth;
    auth.userId = u"test";
    auth.password = u"test";
    auth.machineId = "machine-e2e";
    KCHECK(login.sendPacket(login.spi, EGS_VERIFY_ACCOUNT_REQ, auth));
    KCHECK(login.receive(event));
    KCHECK_EQ(event.eventId, EGS_VERIFY_ACCOUNT_ACK);
    VerifyAccountAck verifyAck;
    KCHECK(decodePayload(event.payload, verifyAck));
    KCHECK_EQ(verifyAck.ok, static_cast<int>(kNetOk));
    KCHECK_EQ(verifyAck.accountInfo.id, WideString(u"test"));
    login.close();

    // ---- channel port: same handshake, then the character flow ------------------
    RawClient channel;
    KCHECK(channel.connectTo(fixture.channelPort()));
    KCHECK(channel.handshake());

    // The 2014 client repeated EGS_CONNECT_REQ on the channel port before verifying.
    KCHECK(channel.sendPacket(channel.spi, EGS_CONNECT_REQ, connect));
    KCHECK(channel.receive(event));
    KCHECK_EQ(event.eventId, EGS_CONNECT_ACK);
    KCHECK(decodePayload(event.payload, connectAck));
    KCHECK_EQ(connectAck.ok, static_cast<int>(kNetOk));

    KCHECK(channel.sendPacket(channel.spi, EGS_VERIFY_ACCOUNT_REQ, auth));
    KCHECK(channel.receive(event));
    KCHECK(decodePayload(event.payload, verifyAck));
    KCHECK_EQ(verifyAck.ok, static_cast<int>(kNetOk));

    KCHECK(channel.sendPacket(channel.spi, EGS_GET_SERVER_SET_DATA_REQ, PacketOk{}));
    KCHECK(channel.receive(event));
    KCHECK_EQ(event.eventId, EGS_GET_SERVER_SET_DATA_ACK);
    GetServerSetDataAck setAck;
    KCHECK(decodePayload(event.payload, setAck));
    KCHECK_EQ(setAck.ok, static_cast<int>(kNetOk));
    KCHECK_EQ(setAck.serverSetList.size(), std::size_t(1));
    KCHECK_EQ(setAck.serverSetList[0].serverSetId, 1);
    KCHECK(setAck.serverSetList[0].serverSetName == u"Test Server");

    SelectServerSetReq select;
    select.serverSetId = 1;
    KCHECK(channel.sendPacket(channel.spi, EGS_SELECT_SERVER_SET_REQ, select));
    KCHECK(channel.receive(event));
    KCHECK_EQ(event.eventId, EGS_SELECT_SERVER_SET_ACK);
    SelectServerSetAck selectAck;
    KCHECK(decodePayload(event.payload, selectAck));
    KCHECK_EQ(selectAck.ok, static_cast<int>(kNetOk));
    KCHECK_EQ(selectAck.port, fixture.channelPort());

    KCHECK(channel.sendPacket(channel.spi, EGS_GET_CHANNEL_LIST_REQ, PacketOk{}));
    KCHECK(channel.receive(event));
    KCHECK_EQ(event.eventId, EGS_GET_CHANNEL_LIST_ACK);
    GetChannelListAck listAck;
    KCHECK(decodePayload(event.payload, listAck));
    KCHECK_EQ(listAck.channelInfo.size(), std::size_t(1));
    KCHECK_EQ(listAck.channelInfo[0].channelId, 1);
    KCHECK_EQ(listAck.channelInfo[0].masterPort, fixture.channelPort());

    // Empty unit list.
    KCHECK(channel.sendPacket(channel.spi, EGS_MY_UNIT_AND_INVENTORY_INFO_LIST_REQ, PacketOk{}));
    KCHECK(channel.receive(event));
    KCHECK_EQ(event.eventId, EGS_MY_UNIT_AND_INVENTORY_INFO_LIST_ACK);
    MyUnitAndInventoryInfoListAck unitsAck;
    KCHECK(decodePayload(event.payload, unitsAck));
    KCHECK_EQ(unitsAck.ok, static_cast<int>(kNetOk));
    KCHECK_EQ(unitsAck.unitInfo.size(), std::size_t(0));

    // Create a character.
    CreateUnitReq createUnit;
    createUnit.nickName = u"Rena";
    createUnit.unitClass = 2;
    KCHECK(channel.sendPacket(channel.spi, EGS_CREATE_UNIT_REQ, createUnit));
    KCHECK(channel.receive(event));
    KCHECK_EQ(event.eventId, EGS_CREATE_UNIT_ACK);
    CreateUnitAck createAck;
    KCHECK(decodePayload(event.payload, createAck));
    KCHECK_EQ(createAck.ok, static_cast<int>(kNetOk));
    KCHECK(createAck.unitInfo.unitUid > 0);
    KCHECK_EQ(createAck.unitInfo.nickName, WideString(u"Rena"));
    KCHECK_EQ(static_cast<int>(createAck.unitInfo.unitClass), 2);
    KCHECK_EQ(static_cast<int>(createAck.unitInfo.level), 1);
    // Class 2 level 1 base stats come from StatTable.lua.
    KCHECK(createAck.unitInfo.stat.atkPhysic > 0);
    const UidType unitUid = createAck.unitInfo.unitUid;

    // A duplicate nickname is rejected with ERR_CREATE_UNIT_01 (26).
    KCHECK(channel.sendPacket(channel.spi, EGS_CREATE_UNIT_REQ, createUnit));
    KCHECK(channel.receive(event));
    KCHECK(decodePayload(event.payload, createAck));
    KCHECK_EQ(createAck.ok, static_cast<int>(kErrCreateUnitNameTaken));

    // The list now contains the character.
    KCHECK(channel.sendPacket(channel.spi, EGS_MY_UNIT_AND_INVENTORY_INFO_LIST_REQ, PacketOk{}));
    KCHECK(channel.receive(event));
    KCHECK(decodePayload(event.payload, unitsAck));
    KCHECK_EQ(unitsAck.unitInfo.size(), std::size_t(1));
    KCHECK_EQ(unitsAck.unitInfo[0].unitUid, unitUid);

    // Select it, then ask for the inventory.
    SelectUnitReq selectUnit;
    selectUnit.unitUid = unitUid;
    KCHECK(channel.sendPacket(channel.spi, EGS_SELECT_UNIT_REQ, selectUnit));
    KCHECK(channel.receive(event));
    KCHECK_EQ(event.eventId, EGS_SELECT_UNIT_ACK);
    SelectUnitAck selectUnitAck;
    KCHECK(decodePayload(event.payload, selectUnitAck));
    KCHECK_EQ(selectUnitAck.ok, static_cast<int>(kNetOk));

    KCHECK(channel.sendPacket(channel.spi, EGS_GET_MY_INVENTORY_REQ, PacketOk{}));
    KCHECK(channel.receive(event));
    KCHECK_EQ(event.eventId, EGS_GET_MY_INVENTORY_ACK);
    GetMyInventoryAck inventoryAck;
    KCHECK(decodePayload(event.payload, inventoryAck));
    KCHECK_EQ(inventoryAck.ok, static_cast<int>(kNetOk));

    // The server answers with one EGS_SELECT_UNIT_INVENTORY_INFO_NOT per bag (1..5); an empty
    // character still gets the five empty notices.
    for (int category = 1; category <= 5; ++category) {
        KCHECK(channel.receive(event));
        KCHECK_EQ(event.eventId, EGS_SELECT_UNIT_INVENTORY_INFO_NOT);
        SelectUnitInventoryInfoNot notice;
        KCHECK(decodePayload(event.payload, notice));
        KCHECK_EQ(notice.slotCategory, category);
        KCHECK_EQ(notice.items.size(), std::size_t(0));
    }

    // Selecting a character that does not exist answers ERR_SELECT_UNIT_00 (35).
    selectUnit.unitUid = 999999;
    KCHECK(channel.sendPacket(channel.spi, EGS_SELECT_UNIT_REQ, selectUnit));
    KCHECK(channel.receive(event));
    KCHECK(decodePayload(event.payload, selectUnitAck));
    KCHECK_EQ(selectUnitAck.ok, static_cast<int>(kErrSelectUnitNotFound));

    // Deleting the character frees the nickname and reports success.
    DeleteUnitReq deleteUnit;
    deleteUnit.unitUid = unitUid;
    KCHECK(channel.sendPacket(channel.spi, EGS_DELETE_UNIT_REQ, deleteUnit));
    KCHECK(channel.receive(event));
    KCHECK_EQ(event.eventId, EGS_DELETE_UNIT_ACK);
    DeleteUnitAck deleteAck;
    KCHECK(decodePayload(event.payload, deleteAck));
    KCHECK_EQ(deleteAck.ok, static_cast<int>(kNetOk));
    KCHECK_EQ(deleteAck.unitUid, unitUid);

    KCHECK(channel.sendPacket(channel.spi, EGS_MY_UNIT_AND_INVENTORY_INFO_LIST_REQ, PacketOk{}));
    KCHECK(channel.receive(event));
    KCHECK(decodePayload(event.payload, unitsAck));
    KCHECK_EQ(unitsAck.unitInfo.size(), std::size_t(0));

    // The character really is gone from the database, and the deletion is recorded in the nickname
    // reservation table so the name cannot be reused immediately. (Read before stop(): closing the
    // server closes the database handle.)
    {
        db::Rows rows = fixture.server().database().query(
            "SELECT COUNT(*) AS c FROM Units WHERE DeletedAt = 0;", {});
        KCHECK(!rows.empty());
        KCHECK_EQ(rows.front().at("c").asInt(), std::int64_t(0));
        rows = fixture.server().database().query("SELECT COUNT(*) AS c FROM DeletedNickNames;", {});
        KCHECK(!rows.empty());
        KCHECK_EQ(rows.front().at("c").asInt(), std::int64_t(1));
        // The daily creation counter counts the one successful create; the duplicate-nickname
        // attempt never reached the insert, and deleting the unit does not refund the day.
        rows = fixture.server().database().query("SELECT Day, Count FROM CreateUnitCount;", {});
        KCHECK_EQ(rows.size(), std::size_t(1));
        KCHECK_EQ(rows.front().at("Count").asInt(), std::int64_t(1));
    }

    channel.close();
    fixture.stop();
}

KTEST(Server, unauthenticatedChannelRequestIsRefused) {
    KCHECK(net::startupSockets());
    ServerFixture fixture;
    KCHECK(fixture.start());

    RawClient channel;
    KCHECK(channel.connectTo(fixture.channelPort()));
    KCHECK(channel.handshake());

    // Asking for the character list before authenticating must not answer with data.
    KCHECK(channel.sendPacket(channel.spi, EGS_MY_UNIT_AND_INVENTORY_INFO_LIST_REQ, PacketOk{}));
    Event event;
    // No reply is expected at all; the frame read must time out.
    KCHECK(channel.expectSilence(400));
    channel.close();
    fixture.stop();
}
