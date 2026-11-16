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

// Reserves `count` distinct free TCP ports and releases them again.
//
// Picking them one at a time is not good enough: the kernel happily hands the same ephemeral port
// back for the next bind, and by the time the server starts binding, anything else on the machine
// may have taken it. Holding all the sockets at once guarantees the numbers differ and that no one
// else grabs them while we are still talking to the previous bind.
std::vector<unsigned short> pickTcpPorts(std::size_t count) {
    std::vector<net::SocketHandle> socks;
    std::vector<unsigned short> ports;
    socks.reserve(count);
    ports.reserve(count);
    for (std::size_t i = 0; i < count; ++i) {
        const net::SocketHandle sock = ::socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
        sockaddr_in addr{};
        addr.sin_family = AF_INET;
        addr.sin_port = 0;
        addr.sin_addr.s_addr = htonl(INADDR_LOOPBACK);
        if (::bind(sock, reinterpret_cast<sockaddr*>(&addr), sizeof(addr)) != 0) {
            net::closeSocket(sock);
            continue;
        }
#if defined(_WIN32)
        int len = static_cast<int>(sizeof(addr));
#else
        socklen_t len = sizeof(addr);
#endif
        ::getsockname(sock, reinterpret_cast<sockaddr*>(&addr), &len);
        ports.push_back(ntohs(addr.sin_port));
        socks.push_back(sock);
    }
    for (const auto sock : socks) net::closeSocket(sock);
    return ports;
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
                if (!s.ok() || spi == 0) return false;
                // Keep our own copy of the association. The server removes it from the shared
                // SecurityDatabase when it closes the connection, and an unknown SPI silently
                // falls back to the SPI 0 keys - so decoding through the database would fail for
                // exactly the replies that arrive just before a hang-up. A real client keeps its
                // keys locally.
                sa_ = sa;
                haveSa_ = true;
                return true;
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

    // True when the server has hung up: the socket reports readability and a zero byte read.
    bool peerClosed(int timeoutMs = 2000) {
        const auto deadline = std::chrono::steady_clock::now() + std::chrono::milliseconds(timeoutMs);
        while (std::chrono::steady_clock::now() < deadline) {
            if (inBuf_.empty() && readFrame(50)) continue;  // drain anything still in flight
            char probe = 0;
#if defined(_WIN32)
            const int n = ::recv(static_cast<SOCKET>(sock_), &probe, 1, 0);
            const bool gone = n == 0 || (n < 0 && ::WSAGetLastError() == WSAECONNRESET);
#else
            const int n = static_cast<int>(::recv(sock_, &probe, 1, MSG_PEEK));
            const bool gone = n == 0 || (n < 0 && (errno == ECONNRESET || errno == EPIPE));
#endif
            if (gone) return true;
            if (n < 0 && errno != EAGAIN && errno != EWOULDBLOCK) return false;
        }
        return false;
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
        // Put our own copy of the association back under its SPI. The server deletes it when it
        // closes, and an unknown SPI quietly falls back to the SPI 0 keys - so without this the
        // replies that arrive just before a hang-up would fail to authenticate. A real client
        // keeps its keys locally and never looks them up in the server's database.
        if (haveSa_ && useSpi != 0) {
            // Put our own association back under its SPI. The server deletes it when the connection
            // closes, and an unknown SPI silently falls back to the SPI 0 keys, so without this the
            // replies that arrive just before a hang-up would fail to authenticate. A real client
            // keeps its keys locally and never consults the server's database at all.
            sec::SecurityDatabase::instance().remove(useSpi);
            sec::SecurityDatabase::instance().insert(useSpi, sa_);
        }
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
    sec::SecurityAssociation sa_;
    bool haveSa_ = false;
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
        const std::vector<unsigned short> ports = pickTcpPorts(3);
        if (ports.size() != 3) std::fprintf(stderr, "fixture: could not reserve three ports\n");
        config_.login.port = ports.size() > 0 ? ports[0] : 0;
        config_.channel.port = ports.size() > 1 ? ports[1] : 0;
        // The game port is enabled so the routing tests can prove that a channel packet sent there
        // is not silently answered by the login or channel service.
        config_.game.port = ports.size() > 2 ? ports[2] : 0;
        config_.game.enabled = true;
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
    unsigned short gamePort() const { return config_.game.port; }

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

// --------------------------------------------------------------------------- counters
KTEST(Server, startBindsEveryEnabledPortAndReportsCounters) {
    KCHECK(net::startupSockets());
    ServerFixture fixture;
    KCHECK(fixture.start());

    KCHECK(fixture.server().started());
    KCHECK_EQ(fixture.server().activeOn(PortRole::kLogin), std::size_t(0));
    KCHECK_EQ(fixture.server().acceptedOn(PortRole::kLogin), std::size_t(0));

    // A connection that only completes the handshake still counts as accepted and active.
    RawClient client;
    KCHECK(client.connectTo(fixture.loginPort()));
    KCHECK(client.handshake());
    KCHECK_EQ(fixture.server().acceptedOn(PortRole::kLogin), std::size_t(1));
    KCHECK_EQ(fixture.server().activeOn(PortRole::kLogin), std::size_t(1));
    KCHECK_EQ(fixture.server().acceptedOn(PortRole::kChannel), std::size_t(0));

    client.close();
    fixture.stop();
    // Closing the client is asynchronous, so the counters are only asserted while it was open.
    KCHECK_EQ(fixture.server().acceptedOn(PortRole::kLogin), std::size_t(1));
}

// A handler that ends the session (EGS_SELECT_SERVER_SET_REQ for a blocked account) closes the
// socket from inside onEvent(). Closing erases the entry from Server::sessions_, which used to
// destroy the ServerSession while its own onEvent() was still running - a use after free that
// crashed intermittently. This drives that exact path against a real socket.
KTEST(Server, aHandlerThatHangsUpLeavesNoDanglingSession) {
    KCHECK(net::startupSockets());
    ServerFixture fixture;
    // The hang-up path logs at debug; raise the level before the server boots so a failure here
    // comes with the server's own account of what happened.
    Logger::instance().setLevel(LogLevel::kDebug);
    KCHECK(fixture.start());
    Logger::instance().setLevel(LogLevel::kWarn);

    // Punish the account the client is about to use, by looking it up the way the server does
    // rather than assuming a uid.
    db::AccountRecord account;
    KCHECK(fixture.server().repositories().accounts.authenticate("test", "test", "127.0.0.1",
                                                                &account) == db::AuthOutcome::kOk);
    KCHECK(fixture.server().repositories().accounts.applyBlock(account.uid, "2014-04-01 00:00",
                                                              "cheating", -3, "", ""));

    RawClient channel;
    KCHECK(channel.connectTo(fixture.channelPort()));
    KCHECK(channel.handshake());

    ConnectReq connect;
    connect.version = WideString(u"2014031300");
    Event event;
    KCHECK(channel.sendPacket(channel.spi, EGS_CONNECT_REQ, connect));
    KCHECK(channel.receive(event));
    KCHECK_EQ(event.eventId, EGS_CONNECT_ACK);

    UserAuthenticateReq auth;
    auth.userId = u"test";
    auth.password = u"test";
    KCHECK(channel.sendPacket(channel.spi, EGS_VERIFY_ACCOUNT_REQ, auth));
    KCHECK(channel.receive(event));
    KCHECK_EQ(event.eventId, EGS_VERIFY_ACCOUNT_ACK);
    VerifyAccountAck verifyAck;
    KCHECK(decodePayload(event.payload, verifyAck));
    // The verify itself is the "blocked account" answer: code 15, the reason, and then the server
    // hangs up - returning false from handle() closes the session from inside onEvent().
    KCHECK_EQ(verifyAck.ok, static_cast<int>(kErrVerifyBlocked));
    KCHECK(verifyAck.accountInfo.blockInfo.blockReason == WideString(u"cheating"));

    // The hang-up has to be real, and the server must survive it: a session erased while its own
    // onEvent() was running would have freed it underneath the caller. The session count is not
    // asserted here - the server drops the entry on its own thread, so a strict zero is a race,
    // and "still serving" below is the stronger property anyway.
    KCHECK(channel.peerClosed());

    channel.close();
    // Still serving: a fresh connection is accepted and answered.
    RawClient again;
    KCHECK(again.connectTo(fixture.channelPort()));
    KCHECK(again.handshake());
    KCHECK(again.sendPacket(again.spi, EGS_CONNECT_REQ, connect));
    KCHECK(again.receive(event));
    KCHECK_EQ(event.eventId, EGS_CONNECT_ACK);
    again.close();
    fixture.stop();
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
    // A fresh server has 0 CCU, which the 2014 thresholds call FREE.
    KCHECK_EQ(static_cast<int>(setAck.serverSetList[0].userCountLevel),
              static_cast<int>(ServerSetData::kUclFree));

    SelectServerSetReq select;
    select.serverSetId = 1;
    KCHECK(channel.sendPacket(channel.spi, EGS_SELECT_SERVER_SET_REQ, select));
    KCHECK(channel.receive(event));
    KCHECK_EQ(event.eventId, EGS_SELECT_SERVER_SET_ACK);
    SelectServerSetAck selectAck;
    KCHECK(decodePayload(event.payload, selectAck));
    KCHECK_EQ(selectAck.ok, static_cast<int>(kNetOk));
    KCHECK_EQ(selectAck.port, fixture.channelPort());
    // The ack carries the address the client should dial, taken from the configured set.
    KCHECK_EQ(selectAck.ip, WideString(u"127.0.0.1"));
    // GSUserSession.cpp:2166 ignores m_ServerSetID, so a bogus id is answered with the same data.
    KCHECK(channel.sendPacket(channel.spi, EGS_SELECT_SERVER_SET_REQ, select));
    KCHECK(channel.receive(event));
    KCHECK_EQ(event.eventId, EGS_SELECT_SERVER_SET_ACK);
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

    // A brand new character has no items, so the whole answer is a single frame: the ack, which
    // the 2014 handler sends *after* the per-bag notices (GSUserGameCommon.cpp:1158).
    KCHECK(channel.sendPacket(channel.spi, EGS_GET_MY_INVENTORY_REQ, PacketOk{}));
    KCHECK(channel.receive(event));
    KCHECK_EQ(event.eventId, EGS_GET_MY_INVENTORY_ACK);
    GetMyInventoryAck inventoryAck;
    KCHECK(decodePayload(event.payload, inventoryAck));
    KCHECK_EQ(inventoryAck.ok, static_cast<int>(kNetOk));
    KCHECK(!inventoryAck.isRecommend);
    KCHECK(channel.expectSilence(300));  // no empty notices follow it

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

// ServerSession::onEvent picks a service from the port role, *except* that the three login ids are
// accepted from any port (the 2014 client repeated the handshake after being redirected). Those two
// halves are what this pins: a login id must work on the game port, and a channel id must not.
KTEST(Server, portRoleDecidesWhichServiceAnswers) {
    KCHECK(net::startupSockets());
    ServerFixture fixture;
    KCHECK(fixture.start());

    RawClient game;
    KCHECK(game.connectTo(fixture.gamePort()));
    KCHECK(game.handshake());
    KCHECK_EQ(fixture.server().acceptedOn(PortRole::kGame), std::size_t(1));

    Event event;

    // 1. Before authenticating, a channel request on the game port reaches ChannelService, which
    //    refuses it: the client must get no data rather than a half filled reply.
    KCHECK(game.sendPacket(game.spi, EGS_GET_SERVER_SET_DATA_REQ, PacketOk{}));
    KCHECK(game.expectSilence(400));

    // 2. A login packet on the game port is routed to LoginService, exactly like the channel port.
    ConnectReq connect;
    connect.version = WideString(u"2014031300");
    KCHECK(game.sendPacket(game.spi, EGS_CONNECT_REQ, connect));
    KCHECK(game.receive(event));
    KCHECK_EQ(event.eventId, EGS_CONNECT_ACK);
    ConnectAck connectAck;
    KCHECK(decodePayload(event.payload, connectAck));
    KCHECK_EQ(connectAck.ok, static_cast<int>(kNetOk));

    UserAuthenticateReq auth;
    auth.userId = u"test";
    auth.password = u"test";
    KCHECK(game.sendPacket(game.spi, EGS_VERIFY_ACCOUNT_REQ, auth));
    KCHECK(game.receive(event));
    KCHECK_EQ(event.eventId, EGS_VERIFY_ACCOUNT_ACK);
    VerifyAccountAck verifyAck;
    KCHECK(decodePayload(event.payload, verifyAck));
    KCHECK_EQ(verifyAck.ok, static_cast<int>(kNetOk));

    // 3. Now that the same session is authenticated, the very same request is answered - one
    //    process, one session, so the login handshake that just succeeded is what unlocks the
    //    channel service on this port.
    KCHECK(game.sendPacket(game.spi, EGS_GET_SERVER_SET_DATA_REQ, PacketOk{}));
    KCHECK(game.receive(event));
    KCHECK_EQ(event.eventId, EGS_GET_SERVER_SET_DATA_ACK);
    GetServerSetDataAck setAck;
    KCHECK(decodePayload(event.payload, setAck));
    KCHECK_EQ(setAck.ok, static_cast<int>(kNetOk));
    game.close();

    game.close();

    // 4. The same request on the login port is refused: LoginService has no handler for it and only
    //    logs, so the client sees nothing rather than a channel answer.
    RawClient login;
    KCHECK(login.connectTo(fixture.loginPort()));
    KCHECK(login.handshake());
    KCHECK(login.sendPacket(login.spi, EGS_GET_SERVER_SET_DATA_REQ, PacketOk{}));
    KCHECK(login.expectSilence(400));
    login.close();

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
