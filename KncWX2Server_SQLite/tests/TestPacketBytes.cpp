// tests/TestPacketBytes.cpp - the bytes the services actually put on the wire.
//
// TestPacketLayout pins the structs; this pins what LoginService and ChannelService hand to the
// transport, decoded with the *2014* field order rather than by round-tripping through our own
// serializer. A round trip only proves we agree with ourselves: these tests walk the payload with a
// bare Serializer and read the fields in the order CommonPacket.cpp / ClientPacket.cpp write them,
// so a struct that agrees with itself but not with the 2014 client still fails.
#include <cstdio>
#include <memory>
#include <string>
#include <vector>

#include "TestFramework.h"
#include "kcore/Log.h"
#include "kcore/Serialize.h"
#include "kdb/Schema.h"
#include "kproto/Event.h"
#include "kproto/LoginPackets.h"
#include "kproto/UnitPackets.h"
#include "kserver/ChannelService.h"
#include "kserver/Config.h"
#include "kserver/LoginService.h"

using namespace knc;
using namespace knc::server;

namespace {

struct SentPacket {
    EventId id = 0;
    SerBuffer payload;
};

class RecordingResponder : public LoginResponder {
public:
    bool send(EventId id, const SerBuffer& payload) override {
        SentPacket packet;
        packet.id = id;
        packet.payload = payload;
        sent.push_back(std::move(packet));
        return true;
    }
    void disconnect(const std::string&) override {}
    const std::string& peerIp() const override { return ip_; }
    int connectedUsers() const override { return 1; }

    const SentPacket* lastOf(EventId id) const {
        for (auto it = sent.rbegin(); it != sent.rend(); ++it) {
            if (it->id == id) return &*it;
        }
        return nullptr;
    }

    std::vector<SentPacket> sent;

private:
    std::string ip_ = "10.1.2.3";
};

// A strict reader: every get() is checked, so a short payload fails instead of silently zeroing.
struct Reader {
    explicit Reader(const SerBuffer& buffer) : copy(buffer) {
        ser.beginReading(&copy);
    }

    template <class T>
    T get() {
        T value{};
        ser.get(value);
        return value;
    }

    bool ok() const { return ser.ok(); }
    std::size_t remaining() const { return copy.readRemaining(); }

    SerBuffer copy;
    Serializer ser;
};

// Builds the two login events a 2014 client sends before anything else.
template <class T>
Event makeEvent(EventId id, const T& payload) {
    Event event;
    event.setData(kPiGsProxy, nullptr, id);
    encodePayload(payload, event.payload);
    return event;
}

struct Fixture {
    Fixture() {
        Logger::instance().setLevel(LogLevel::kError);
        db::Database::Config config;
        config.memory = true;
        std::string error;
        if (!database.open(config, &error)) std::fprintf(stderr, "open: %s\n", error.c_str());
        if (!db::applySchema(database).ok) std::fprintf(stderr, "schema failed\n");
        if (!db::seedDefaultAccounts(database, &error))
            std::fprintf(stderr, "seed: %s\n", error.c_str());
        ownedRepos = db::makeRepositories(database);
        repos = ownedRepos.get();
        serverConfig = Config::defaults();
        serverConfig.seedTestAccounts = true;
        config_ = serverConfig;
        login = std::make_unique<LoginService>(*repos, serverConfig);
        channel = std::make_unique<ChannelService>(*repos, serverConfig);
        loginSession = AccountSession{};
        channelSession = AccountSession{};
    }

    void connectAndVerify(RecordingResponder& responder, const char* user = "test",
                          const char* password = "test") {
        ConnectReq connect;
        connect.version = config_.version();
        Event connectEvent = makeEvent(EGS_CONNECT_REQ, connect);
        login->handle(loginSession, responder, connectEvent);

        UserAuthenticateReq auth;
        auth.userId = toWide(user);
        auth.password = toWide(password);
        Event verifyEvent = makeEvent(EGS_VERIFY_ACCOUNT_REQ, auth);
        login->handle(loginSession, responder, verifyEvent);
    }

    // The session has to be authenticated before the channel port accepts anything; the other
    // channel tests do the same through the account repository so the record is a real one.
    void authenticateChannelSession() {
        channelSession.authenticated = true;
        channelSession.state = LoginState::kAuthenticated;
        if (repos->accounts.authenticate("test", "test", "10.1.2.3", &channelSession.account) !=
            db::AuthOutcome::kOk) {
            std::fprintf(stderr, "fixture: could not authenticate the seeded account\n");
        }
    }

    db::Database database;
    std::unique_ptr<db::Repositories> ownedRepos;
    db::Repositories* repos = nullptr;
    Config serverConfig;
    Config config_;
    std::unique_ptr<LoginService> login;
    std::unique_ptr<ChannelService> channel;
    AccountSession loginSession;
    AccountSession channelSession;
    RecordingResponder responder;
};

}  // namespace

// EGS_CONNECT_ACK = { int ok, int udpPort, int channelId } - three ints, nothing else. Any extra
// bool here (udpKickOff) would push channelId out of place.
KTEST(PacketBytes, connectAckIsThreeInts) {
    Fixture f;
    ConnectReq connect;
    connect.version = f.config_.version();
    Event connectEvent = makeEvent(EGS_CONNECT_REQ, connect);
    f.login->handle(f.loginSession, f.responder, connectEvent);

    const SentPacket* ack = f.responder.lastOf(EGS_CONNECT_ACK);
    KCHECK(ack != nullptr);
    if (!ack) return;
    KCHECK_EQ(ack->payload.size(), std::size_t{12});

    Reader reader(ack->payload);
    KCHECK_EQ(reader.get<int>(), 0);                                    // ok == NET_OK
    const unsigned short expectedUdpPort =
        f.serverConfig.channels.empty() ? 0 : f.serverConfig.channels.front().ncUdpPort;
    const int expectedChannelId =
        f.serverConfig.channels.empty() ? 0 : f.serverConfig.channels.front().channelId;
    KCHECK_EQ(reader.get<int>(), static_cast<int>(expectedUdpPort));
    KCHECK_EQ(reader.get<int>(), expectedChannelId);
    KCHECK(reader.ok());
    KCHECK_EQ(reader.remaining(), std::size_t{0});
}

// EGS_VERIFY_ACCOUNT_ACK, read in the 2014 order:
//   int ok, KAccountInfo, char hackingUserType, wstring adURL, bool sex, u32 age, u32 nexonSn,
//   wstring clientIP, u8 channelCode, wstring channelUserID
KTEST(PacketBytes, verifyAccountAckFieldOrder) {
    Fixture f;
    f.connectAndVerify(f.responder);

    const SentPacket* ack = f.responder.lastOf(EGS_VERIFY_ACCOUNT_ACK);
    KCHECK(ack != nullptr);
    if (!ack) return;

    Reader reader(ack->payload);
    KCHECK_EQ(reader.get<int>(), 0);                                    // ok

    KCHECK(reader.get<UidType>() > 0);                                  // accountInfo.userUid
    KCHECK_EQ(reader.get<WideString>(), WideString(u"test"));           // accountInfo.id
    (void)reader.get<WideString>();                                     // accountInfo.name
    (void)reader.get<int>();                                            // accountInfo.authLevel
    (void)reader.get<bool>();                                           // accountInfo.internalUser
    (void)reader.get<bool>();                                           // accountOption.playGuide
    (void)reader.get<WideString>();                                     // blockInfo.endTime
    (void)reader.get<WideString>();                                     // blockInfo.blockReason
    (void)reader.get<bool>();                                           // accountInfo.isRecommend
    (void)reader.get<bool>();                                           // accountInfo.isGuestUser
    (void)reader.get<WideString>();                                     // accountInfo.otp
    (void)reader.get<WideString>();                                     // accountInfo.regDate
    (void)reader.get<WideString>();                                     // accountInfo.lastLogin

    KCHECK_EQ(reader.get<char>(), 0);                                   // hackingUserType
    (void)reader.get<WideString>();                                     // adURL
    (void)reader.get<bool>();                                           // sex
    (void)reader.get<UINT32>();                                         // age
    (void)reader.get<UINT32>();                                         // nexonSn
    KCHECK_EQ(reader.get<WideString>(), toWide(f.responder.peerIp()));  // clientIP
    KCHECK_EQ(reader.get<byte>(), 0);                                   // channelCode
    (void)reader.get<WideString>();                                     // channelUserID

    // SERV_EVENT_MONEY is off, so there is nothing left - a stray int here would be exactly the
    // kind of field the client would read as the next packet.
    KCHECK(reader.ok());
    KCHECK_EQ(reader.remaining(), std::size_t{0});
}

// EGS_MY_UNIT_AND_INVENTORY_INFO_LIST_ACK = { int ok, int unitSlot, vector<KUnitInfo>, bool
// sharingBank }. sharingBank is last; reading it where a vector is expected would be a silent
// misinterpretation, so both positions are checked.
KTEST(PacketBytes, unitListAckCarriesSharingBankLast) {
    Fixture f;
    f.authenticateChannelSession();
    Event listEvent = makeEvent<UINT>(EGS_MY_UNIT_AND_INVENTORY_INFO_LIST_REQ, 0);
    f.channel->handle(f.channelSession, f.responder, listEvent);

    const SentPacket* ack = f.responder.lastOf(EGS_MY_UNIT_AND_INVENTORY_INFO_LIST_ACK);
    KCHECK(ack != nullptr);
    if (!ack) return;

    Reader reader(ack->payload);
    KCHECK_EQ(reader.get<int>(), 0);                                    // ok
    const int unitSlot = reader.get<int>();                             // unitSlot
    KCHECK(unitSlot >= 0);
    const UINT unitCount = reader.get<UINT>();                          // vector count
    KCHECK_EQ(unitCount, static_cast<UINT>(0));                         // the seeded account has none
    // The 2014 server only set the flag when P_GBankShare_* said the bank is shared; a fresh
    // account has no share row, so the honest value here is false rather than "the flag is on".
    KCHECK_EQ(reader.get<bool>(), false);                               // sharingBank
    KCHECK(reader.ok());
    KCHECK_EQ(reader.remaining(), std::size_t{0});
}

// A created character is reported back inside EGS_CREATE_UNIT_ACK as { int ok, KUnitInfo,
// vector<int> skillSlot, wstring enableDate }. Walk the KUnitInfo fields in the 2014 order and
// check the ones the database decides, then the tail.
KTEST(PacketBytes, createUnitAckUnitInfoOrder) {
    Fixture f;
    f.authenticateChannelSession();

    CreateUnitReq request;
    request.nickName = WideString(u"ByteCheck");
    request.unitClass = 0;
    Event createEvent = makeEvent(EGS_CREATE_UNIT_REQ, request);
    f.channel->handle(f.channelSession, f.responder, createEvent);

    const SentPacket* ack = f.responder.lastOf(EGS_CREATE_UNIT_ACK);
    KCHECK(ack != nullptr);
    if (!ack) return;

    Reader reader(ack->payload);
    KCHECK_EQ(reader.get<int>(), 0);                                    // ok

    const UidType owner = reader.get<UidType>();
    KCHECK(owner > 0);                                                  // ownerUserUid
    const char authLevel = reader.get<char>();
    KCHECK(authLevel >= 0);                                             // authLevel
    const UidType unitUid = reader.get<UidType>();
    KCHECK(unitUid > 0);                                                // unitUid
    // m_uiKNMSerialNum comes from the Nexon messenger service (P_MessengerSN_SEL); a database that
    // was never linked to it stores 0, and that is what goes on the wire.
    const unsigned int knmSerial = reader.get<unsigned int>();
    KCHECK_EQ(knmSerial, static_cast<unsigned int>(0));                 // knmSerialNum
    KCHECK_EQ(reader.get<char>(), 0);                                   // unitClass
    KCHECK_EQ(reader.get<WideString>(), WideString(u"ByteCheck"));      // nickName

    (void)reader.get<WideString>();                                     // ip
    (void)reader.get<USHORT>();                                         // port
    (void)reader.get<int>();                                            // ed
    KCHECK_EQ(reader.get<byte>(), byte{1});                             // level == 1
    (void)reader.get<int>();                                            // exp

    for (int i = 0; i < 4; ++i) (void)reader.get<int>();                // officialMatchCnt..rPoint
    (void)reader.get<int>();                                            // aPoint
    (void)reader.get<bool>();                                           // isWinBeforeMatch
    (void)reader.get<char>();                                           // rank
    (void)reader.get<float>();                                          // kFactor
    (void)reader.get<bool>();                                           // isRedistributionUser
    (void)reader.get<int>();                                            // pastSeasonWin

    (void)reader.get<int>();                                            // sPoint
    (void)reader.get<int>();                                            // csPoint
    (void)reader.get<int>();                                            // maxCsPoint
    (void)reader.get<WideString>();                                     // csPointEndDate

    (void)reader.get<int>();                                            // nowBaseLevelExp
    (void)reader.get<int>();                                            // nextBaseLevelExp
    (void)reader.get<int>();                                            // straightVictories

    for (int stat = 0; stat < 2; ++stat) {                              // stat, gameStat
        for (int field = 0; field < 5; ++field) (void)reader.get<int>();
    }

    (void)reader.get<int>();                                            // lastPos.mapId
    (void)reader.get<unsigned char>();                                  // lastPos.lastTouchLineIndex
    (void)reader.get<unsigned short>();                                 // lastPos.lastPosValue
    (void)reader.get<bool>();                                           // lastPos.ignoreLastTouch

    const UINT buffCount = reader.get<UINT>();                          // vector<KBuffInfo>
    KCHECK_EQ(buffCount, static_cast<UINT>(0));

    (void)reader.get<int>();                                            // win
    (void)reader.get<int>();                                            // lose

    KCHECK_EQ(reader.get<UINT>(), static_cast<UINT>(0));                // map dungeonClear
    KCHECK_EQ(reader.get<UINT>(), static_cast<UINT>(0));                // map tcClear
    KCHECK_EQ(reader.get<UINT>(), static_cast<UINT>(0));                // map dungeonPlay
    KCHECK_EQ(reader.get<UINT>(), static_cast<UINT>(0));                // map equippedItem

    // KUnitSkillData: eight 3 byte slots, then the strings/containers, then the two page counters.
    for (int i = 0; i < 2 * UnitSkillData::kEquippedSkillSlotCount; ++i) {
        (void)reader.get<std::int16_t>();                               // skillId
        (void)reader.get<byte>();                                       // skillLevel
    }
    KCHECK_EQ(reader.get<WideString>(), WideString());                  // skillSlotBEndDate
    KCHECK_EQ(reader.get<char>(), 0);                                   // skillSlotBExpirationState
    KCHECK_EQ(reader.get<UINT>(), static_cast<UINT>(0));                // passiveSkill
    KCHECK_EQ(reader.get<UINT>(), static_cast<UINT>(0));                // guildPassiveSkill
    KCHECK_EQ(reader.get<UINT>(), static_cast<UINT>(0));                // skillNote
    KCHECK_EQ(reader.get<byte>(), byte{1});                             // activeSkillPagesNumber
    KCHECK_EQ(reader.get<byte>(), byte{1});                             // skillPagesAvailable

    KCHECK_EQ(reader.get<bool>(), false);                               // isParty
    (void)reader.get<int>();                                            // spiritMax
    (void)reader.get<int>();                                            // spirit
    (void)reader.get<bool>();                                           // isGameBang
    (void)reader.get<int>();                                            // pcBangType (int, not char)
    (void)reader.get<int>();                                            // titleId

    KCHECK_EQ(reader.get<int>(), 0);                                    // guildInfo.guildUid
    KCHECK_EQ(reader.get<WideString>(), WideString());                  // guildInfo.guildName
    KCHECK_EQ(reader.get<byte>(), byte{0});                             // guildInfo.membershipGrade
    KCHECK_EQ(reader.get<int>(), 0);                                    // guildInfo.honorPoint

    (void)reader.get<WideString>();                                     // lastLoginTime
    (void)reader.get<std::int64_t>();                                   // warpVipEndDate
    (void)reader.get<int>();                                            // eventQuestClearCount
    (void)reader.get<int>();                                            // exchangeCount

    const UINT skillSlots = reader.get<UINT>();                          // vector<int> skillSlot
    KCHECK_EQ(skillSlots, static_cast<UINT>(UnitSkillData::kEquippedSkillSlotCount));
    for (UINT i = 0; i < skillSlots; ++i) (void)reader.get<int>();
    (void)reader.get<WideString>();                                     // enableDate

    KCHECK(reader.ok());
    KCHECK_EQ(reader.remaining(), std::size_t{0});
}

// EGS_DELETE_UNIT_ACK = { int ok, i64 unitUid, u32 knmSerialNum, int guildUid, wstring nickName,
// u8 level, char unitClass }. The guild uid is the field that was missing.
KTEST(PacketBytes, deleteUnitAckFieldOrder) {
    Fixture f;
    f.authenticateChannelSession();

    CreateUnitReq request;
    request.nickName = WideString(u"ToDelete");
    request.unitClass = 0;
    Event createEvent = makeEvent(EGS_CREATE_UNIT_REQ, request);
    f.channel->handle(f.channelSession, f.responder, createEvent);

    // Find the character through the unit list, which is what the client does too.
    db::UnitRecord unit;
    {
        std::vector<db::UnitSummary> units;
        std::string error;
        if (!f.repos->units.listSummaries(f.channelSession.account.uid, &units, &error) || units.empty()) {
            std::fprintf(stderr, "list units: %s\n", error.c_str());
            KCHECK(false);
            return;
        }
        KCHECK(f.repos->units.load(units.front().unitUid, &unit));
    }

    DeleteUnitReq deleteRequest;
    deleteRequest.unitUid = unit.unitUid;
    deleteRequest.userUid = f.channelSession.account.uid;
    Event deleteEvent = makeEvent(EGS_DELETE_UNIT_REQ, deleteRequest);
    f.channel->handle(f.channelSession, f.responder, deleteEvent);

    const SentPacket* ack = f.responder.lastOf(EGS_DELETE_UNIT_ACK);
    KCHECK(ack != nullptr);
    if (!ack) return;

    Reader reader(ack->payload);
    KCHECK_EQ(reader.get<int>(), 0);                                    // ok
    KCHECK_EQ(reader.get<UidType>(), unit.unitUid);
    KCHECK_EQ(reader.get<UINT32>(), static_cast<UINT32>(unit.knmSerial));
    KCHECK_EQ(reader.get<int>(), 0);                                    // guildUid (no guild here)
    KCHECK_EQ(reader.get<WideString>(), WideString(u"ToDelete"));        // nickName
    KCHECK_EQ(reader.get<byte>(), byte{1});                             // level
    KCHECK_EQ(reader.get<char>(), 0);                                   // unitClass
    KCHECK(reader.ok());
    KCHECK_EQ(reader.remaining(), std::size_t{0});
}

// EGS_GET_CHANNEL_LIST_ACK = { int serverGroupId, vector<KChannelInfo>, map<int,
// KChannelBonusInfo> }. KChannelInfo must *not* carry a bonus info of its own
// (SERV_INTEGRATE_THE_CHANNEL_INFO is off), so after the ten documented fields the next bytes are
// the map count.
KTEST(PacketBytes, channelListAckShape) {
    Fixture f;
    f.authenticateChannelSession();
    Event channelListEvent = makeEvent<UINT>(EGS_GET_CHANNEL_LIST_REQ, 0);
    f.channel->handle(f.channelSession, f.responder, channelListEvent);

    const SentPacket* ack = f.responder.lastOf(EGS_GET_CHANNEL_LIST_ACK);
    KCHECK(ack != nullptr);
    if (!ack) return;

    Reader reader(ack->payload);
    (void)reader.get<int>();                                            // serverGroupId
    const UINT channelCount = reader.get<UINT>();
    KCHECK_EQ(channelCount, static_cast<UINT>(f.serverConfig.channels.size()));
    for (UINT i = 0; i < channelCount; ++i) {
        (void)reader.get<int>();                                        // serverUid
        (void)reader.get<int>();                                        // channelId
        (void)reader.get<WideString>();                                 // channelName
        (void)reader.get<int>();                                        // serverGroupId
        (void)reader.get<WideString>();                                 // ip
        (void)reader.get<USHORT>();                                     // masterPort
        (void)reader.get<USHORT>();                                     // ncUdpPort
        (void)reader.get<int>();                                        // maxUser
        (void)reader.get<int>();                                        // currentUser
        (void)reader.get<int>();                                        // curPartyCount
        (void)reader.get<int>();                                        // playGamePartyCount
    }
    const UINT bonusCount = reader.get<UINT>();                         // map<int, KChannelBonusInfo>
    KCHECK_EQ(bonusCount, static_cast<UINT>(f.serverConfig.channels.size()));
    for (UINT i = 0; i < bonusCount; ++i) {
        (void)reader.get<int>();                                        // key: channelId
        (void)reader.get<bool>();                                       // enable
        (void)reader.get<int>();                                        // beginLevel
        (void)reader.get<int>();                                        // endLevel
        (void)reader.get<int>();                                        // perExp
        (void)reader.get<int>();                                        // perEd
    }
    KCHECK(reader.ok());
    KCHECK_EQ(reader.remaining(), std::size_t{0});
}

// EGS_SELECT_SERVER_SET_ACK = { int ok, wstring ip, u16 port, KAccountBlockInfo }, and the block
// info is only two strings in this build.
KTEST(PacketBytes, selectServerSetAckShape) {
    Fixture f;
    f.authenticateChannelSession();
    SelectServerSetReq request;
    request.serverSetId = f.serverConfig.serverSets.empty() ? 0 : f.serverConfig.serverSets.front().id;
    Event selectSetEvent = makeEvent(EGS_SELECT_SERVER_SET_REQ, request);
    f.channel->handle(f.channelSession, f.responder, selectSetEvent);

    const SentPacket* ack = f.responder.lastOf(EGS_SELECT_SERVER_SET_ACK);
    KCHECK(ack != nullptr);
    if (!ack) return;

    Reader reader(ack->payload);
    KCHECK_EQ(reader.get<int>(), 0);                                    // ok
    (void)reader.get<WideString>();                                     // ip
    (void)reader.get<USHORT>();                                         // port
    (void)reader.get<WideString>();                                     // blockInfo.endTime
    (void)reader.get<WideString>();                                     // blockInfo.blockReason
    KCHECK(reader.ok());
    KCHECK_EQ(reader.remaining(), std::size_t{0});
}
