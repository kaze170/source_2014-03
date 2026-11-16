// tests/TestLoginService.cpp - packet level tests of LoginService without any socket.
//
// LoginService takes a LoginResponder, so the whole login/verify/new-user state machine can be
// driven directly: no ports, no threads, no timing. This is where the *policy* is pinned down (which
// NetError each bad input maps to, what a block looks like on the wire, what the verify reply
// carries), while TestServer.cpp owns the transport.
#include <cstdio>
#include <memory>
#include <string>
#include <vector>

#include "TestFramework.h"
#include "kcore/Log.h"
#include "kcore/Serialize.h"
#include "kdb/Schema.h"
#include "kgame/Rules.h"
#include "kproto/Event.h"
#include "kproto/LoginPackets.h"
#include "kserver/Config.h"
#include "kserver/LoginService.h"

using namespace knc;
using namespace knc::server;

namespace {

// Records everything the service sends and answers the questions it asks the transport.
class RecordingResponder : public LoginResponder {
public:
    explicit RecordingResponder(std::string ip = "10.0.0.7", int connected = 1)
        : connected_(connected), ip_(std::move(ip)) {}

    bool send(EventId id, const SerBuffer& payload) override {
        Sent sent;
        sent.id = id;
        sent.payload = payload;
        sent_.push_back(std::move(sent));
        return true;
    }
    void disconnect(const std::string& reason) override { disconnectReason = reason; }
    const std::string& peerIp() const override { return ip_; }
    int connectedUsers() const override { return connected_; }

    struct Sent {
        EventId id = 0;
        SerBuffer payload;
    };

    // Decodes the last reply as T; false when nothing was sent or the bytes do not parse.
    template <class T>
    bool last(T& out) const {
        if (sent_.empty()) return false;
        SerBuffer copy = sent_.back().payload;
        return decodePayload(copy, out);
    }

    EventId lastId() const { return sent_.empty() ? EventId(0) : sent_.back().id; }
    std::size_t count() const { return sent_.size(); }
    void clear() { sent_.clear(); }

    std::string disconnectReason;
    int connected_ = 0;

private:
    std::string ip_;
    std::vector<Sent> sent_;
};

struct Fixture {
    Fixture() {
        Logger::instance().setLevel(LogLevel::kError);
        db::Database::Config config;
        config.memory = true;
        std::string error;
        if (!database.open(config, &error)) std::fprintf(stderr, "open: %s\n", error.c_str());
        if (!db::applySchema(database).ok) std::fprintf(stderr, "schema failed\n");
        if (!db::seedDefaultAccounts(database, &error)) std::fprintf(stderr, "seed: %s\n", error.c_str());
        ownedRepos = db::makeRepositories(database);
        serverConfig = Config::defaults();
        serverConfig.seedTestAccounts = true;
        service = std::make_unique<LoginService>(*ownedRepos, serverConfig);
    }

    // The event a decoded frame turns into: client proxy performer, no target uids.
    Event makeEvent(EventId id) const {
        Event event;
        event.setData(kPiGsProxy, nullptr, id);
        return event;
    }

    template <class T>
    Event eventWith(EventId id, const T& payload) const {
        Event event = makeEvent(id);
        encodePayload(payload, event.payload);
        return event;
    }

    ConnectReq connectReq(const char* version = nullptr) const {
        ConnectReq req;
        req.version = toWide(version ? version : "2014031300");
        return req;
    }

    UserAuthenticateReq authReq(const std::string& user, const std::string& password) const {
        UserAuthenticateReq req;
        req.userId = toWide(user);
        req.password = toWide(password);
        return req;
    }

    // The two steps every real client performs before anything else.
    void connectAndVerify(RecordingResponder& responder, const std::string& user = "test",
                          const std::string& password = "test") {
        Event connect = eventWith(EGS_CONNECT_REQ, connectReq());
        service->handle(session, responder, connect);
        Event verify = eventWith(EGS_VERIFY_ACCOUNT_REQ, authReq(user, password));
        service->handle(session, responder, verify);
    }

    db::Database database;
    std::unique_ptr<db::Repositories> ownedRepos;
    Config serverConfig;
    std::unique_ptr<LoginService> service;

    db::Repositories& repos() { return *ownedRepos; }
    AccountSession session;
};

}  // namespace

// --------------------------------------------------------------------------- connect
KTEST(Login, connectRepliesWithTheConfiguredPorts) {
    Fixture f;
    RecordingResponder responder;
    Event connect = f.eventWith(EGS_CONNECT_REQ, f.connectReq());
    KCHECK(f.service->handle(f.session, responder, connect));

    KCHECK_EQ(responder.lastId(), EGS_CONNECT_ACK);
    ConnectAck ack;
    KCHECK(responder.last(ack));
    KCHECK_EQ(ack.ok, static_cast<int>(kNetOk));
    KCHECK_EQ(ack.channelId, f.serverConfig.channels.front().channelId);
    KCHECK_EQ(ack.udpPort, static_cast<int>(f.serverConfig.channels.front().ncUdpPort));
    KCHECK(!ack.udpKickOff);
    KCHECK(f.session.state == LoginState::kConnectedClient);
}

KTEST(Login, connectRejectsAnUnknownVersion) {
    Fixture f;
    RecordingResponder responder;
    Event connect = f.eventWith(EGS_CONNECT_REQ, f.connectReq("1999010100"));
    KCHECK(!f.service->handle(f.session, responder, connect));  // false: drop after the ack

    ConnectAck ack;
    KCHECK(responder.last(ack));
    KCHECK_EQ(ack.ok, static_cast<int>(kErrConnectVersion));  // ERR_CONNECT_00
    KCHECK(f.session.state == LoginState::kConnected);        // never advanced
    KCHECK_EQ(f.service->rejectedConnects(), 1LL);
}

KTEST(Login, fullServerIsRefusedWithErrConnect04) {
    Fixture f;
    // connectedUsers() counts the session asking, so maxUser connections means "one more" is full.
    RecordingResponder responder("10.0.0.7", f.serverConfig.login.maxUser + 1);
    Event connect = f.eventWith(EGS_CONNECT_REQ, f.connectReq());
    KCHECK(!f.service->handle(f.session, responder, connect));

    ConnectAck ack;
    KCHECK(responder.last(ack));
    KCHECK_EQ(ack.ok, static_cast<int>(kErrConnectFull));  // ERR_CONNECT_04
}

// --------------------------------------------------------------------------- verify
KTEST(Login, verifyReturnsTheAccountAndClearsTheBlockInfo) {
    Fixture f;
    RecordingResponder responder;
    f.connectAndVerify(responder);
    KCHECK_EQ(responder.lastId(), EGS_VERIFY_ACCOUNT_ACK);

    VerifyAccountAck ack;
    KCHECK(responder.last(ack));
    KCHECK_EQ(ack.ok, static_cast<int>(kNetOk));
    KCHECK_EQ(ack.accountInfo.id, WideString(u"test"));
    KCHECK(ack.accountInfo.userUid > 0);
    // The block info travels on every verify: an empty end time means "not blocked".
    KCHECK(ack.accountInfo.blockInfo.endTime.empty());
    KCHECK_EQ(ack.clientIp, WideString(u"10.0.0.7"));
    KCHECK(f.session.authenticated);
    KCHECK(f.session.state == LoginState::kAuthenticated);
    KCHECK_EQ(f.service->successfulLogins(), 1LL);
}

KTEST(Login, verifyRejectsABadPasswordWithoutLeakingTheAccount) {
    Fixture f;
    RecordingResponder responder;
    f.connectAndVerify(responder, "test", "not-the-password");
    KCHECK_EQ(responder.lastId(), EGS_VERIFY_ACCOUNT_ACK);

    VerifyAccountAck ack;
    KCHECK(responder.last(ack));
    KCHECK_EQ(ack.ok, static_cast<int>(kErrVerifyBadPassword));  // ERR_VERIFY_06
    KCHECK_EQ(ack.accountInfo.userUid, UidType(0));  // nothing about the account is disclosed
    KCHECK(!f.session.authenticated);
}

KTEST(Login, verifyRejectsAnUnknownAccount) {
    Fixture f;
    RecordingResponder responder;
    f.connectAndVerify(responder, "nobody", "whatever");

    VerifyAccountAck ack;
    KCHECK(responder.last(ack));
    KCHECK_EQ(ack.ok, static_cast<int>(kErrVerifyNoUser));  // ERR_VERIFY_04
}

KTEST(Login, verifyReportsTheBlockDetails) {
    Fixture f;
    KCHECK(f.repos().accounts.applyBlock(1, "9999-12-31 23:59:59", "policy", 1, "", ""));
    RecordingResponder responder;
    f.connectAndVerify(responder);

    VerifyAccountAck ack;
    KCHECK(responder.last(ack));
    KCHECK_EQ(ack.ok, static_cast<int>(kErrVerifyBlocked));  // ERR_VERIFY_11
    // A blocked login still receives the block details, otherwise the client cannot show why.
    KCHECK_EQ(ack.accountInfo.blockInfo.endTime, WideString(u"9999-12-31 23:59:59"));
    KCHECK_EQ(ack.accountInfo.blockInfo.blockReason, WideString(u"policy"));
    KCHECK_EQ(static_cast<int>(ack.accountInfo.blockInfo.blockType), 1);
    KCHECK(!f.session.authenticated);
}

KTEST(Login, verifyArrivingBeforeConnectIsRefusedAndEndsTheSession) {
    Fixture f;
    RecordingResponder responder;
    Event verify = f.eventWith(EGS_VERIFY_ACCOUNT_REQ, f.authReq("test", "test"));
    KCHECK(!f.service->handle(f.session, responder, verify));

    VerifyAccountAck ack;
    KCHECK(responder.last(ack));
    KCHECK_EQ(ack.ok, static_cast<int>(kErrClientState));  // ERR_CLIENT_STATE
    KCHECK(!f.session.authenticated);
}

KTEST(Login, secondVerifyAfterSuccessIsRefused) {
    Fixture f;
    RecordingResponder responder;
    f.connectAndVerify(responder);
    const std::size_t afterFirst = responder.count();

    Event again = f.eventWith(EGS_VERIFY_ACCOUNT_REQ, f.authReq("test", "test"));
    f.service->handle(f.session, responder, again);
    KCHECK_EQ(responder.count(), afterFirst + 1);
    VerifyAccountAck ack;
    KCHECK(responder.last(ack));
    KCHECK_EQ(ack.ok, static_cast<int>(kErrClientState));
}

KTEST(Login, idsLongerThanTheProtocolAllowsAreRejected) {
    Fixture f;
    RecordingResponder responder;
    Event connect = f.eventWith(EGS_CONNECT_REQ, f.connectReq());
    f.service->handle(f.session, responder, connect);

    Event verify = f.eventWith(EGS_VERIFY_ACCOUNT_REQ, f.authReq(std::string(200, 'x'), "pw"));
    KCHECK(!f.service->handle(f.session, responder, verify));
    VerifyAccountAck ack;
    KCHECK(responder.last(ack));
    KCHECK_EQ(ack.ok, static_cast<int>(kErrVerifyIdLength));  // ERR_VERIFY_16
}

// --------------------------------------------------------------------------- new user
KTEST(Login, newUserCreatesTheAccountAndCanVerifyStraightAfter) {
    Fixture f;
    RecordingResponder responder;
    Event connect = f.eventWith(EGS_CONNECT_REQ, f.connectReq());
    f.service->handle(f.session, responder, connect);

    NewUserJoinReq request;
    request.id = WideString(u"newcomer");
    request.password = WideString(u"secret12");
    request.name = WideString(u"Newcomer");
    request.guestUser = 0;
    Event join = f.eventWith(EGS_NEW_USER_JOIN_REQ, request);
    KCHECK(f.service->handle(f.session, responder, join));

    NewUserJoinAck ack;
    KCHECK(responder.last(ack));
    KCHECK_EQ(ack.ok, static_cast<int>(kNetOk));
    KCHECK(ack.userUid > 0);

    // The new account is usable immediately.
    Event verify = f.eventWith(EGS_VERIFY_ACCOUNT_REQ, f.authReq("newcomer", "secret12"));
    KCHECK(f.service->handle(f.session, responder, verify));
    VerifyAccountAck verified;
    KCHECK(responder.last(verified));
    KCHECK_EQ(verified.ok, static_cast<int>(kNetOk));
    KCHECK_EQ(verified.accountInfo.userUid, ack.userUid);
}

KTEST(Login, newUserEnforcesTheNicknameRules) {
    Fixture f;
    RecordingResponder responder;
    Event connect = f.eventWith(EGS_CONNECT_REQ, f.connectReq());
    f.service->handle(f.session, responder, connect);

    // Too short for the login id (the 2014 minimum is 4, the seeded gm/gm only got in because it
    // pre-dates the rule).
    NewUserJoinReq shortId;
    shortId.id = WideString(u"abc");
    shortId.password = WideString(u"secret12");
    shortId.name = WideString(u"Somebody");
    Event join = f.eventWith(EGS_NEW_USER_JOIN_REQ, shortId);
    KCHECK(!f.service->handle(f.session, responder, join));
    NewUserJoinAck ack;
    KCHECK(responder.last(ack));
    KCHECK_EQ(ack.ok, static_cast<int>(kErrNewAccountIdLength));

    // Too short for the password.
    NewUserJoinReq shortPassword = shortId;
    shortPassword.id = WideString(u"validid");
    shortPassword.password = WideString(u"abc");
    Event shortPasswordEvent = f.eventWith(EGS_NEW_USER_JOIN_REQ, shortPassword);
    f.service->handle(f.session, responder, shortPasswordEvent);
    KCHECK(responder.last(ack));
    KCHECK_EQ(ack.ok, static_cast<int>(kErrNewAccountPasswordLength));

    // Duplicate login id: the UNIQUE(LoginID) violation is reported as a transaction failure.
    NewUserJoinReq duplicate = shortId;
    duplicate.id = WideString(u"test");
    duplicate.password = WideString(u"secret12");
    Event duplicateEvent = f.eventWith(EGS_NEW_USER_JOIN_REQ, duplicate);
    f.service->handle(f.session, responder, duplicateEvent);
    KCHECK(responder.last(ack));
    KCHECK_EQ(ack.ok, static_cast<int>(kErrNewAccountTransaction));
    KCHECK_EQ(f.repos().accounts.count(), std::int64_t(2));  // test + gm, nothing was added
}

// --------------------------------------------------------------------------- dispatch
KTEST(Login, heartBeatIsAnsweredWithoutAReply) {
    Fixture f;
    RecordingResponder responder;
    Event beat = f.makeEvent(E_HEART_BEAT);
    KCHECK(f.service->handle(f.session, responder, beat));
    KCHECK_EQ(responder.count(), std::size_t(0));
}

KTEST(Login, unknownPacketIsIgnoredWithoutDroppingTheSession) {
    Fixture f;
    RecordingResponder responder;
    f.connectAndVerify(responder);
    const std::size_t before = responder.count();

    Event foreign = f.makeEvent(EGS_GET_MY_INVENTORY_REQ);  // a channel packet on a login session
    KCHECK(f.service->handle(f.session, responder, foreign));
    KCHECK_EQ(responder.count(), before);
    KCHECK(responder.disconnectReason.empty());
}
