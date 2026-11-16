// kserver/LoginService.h - the login/channel state machine.
//
// In 2014 this logic lived in three executables and crossed process boundaries three times:
//
//   client -> LoginServer   EGS_CONNECT_REQ  / EGS_VERIFY_ACCOUNT_REQ  (mup_auth_user)
//   client -> ChannelServer EGS_GET_SERVER_SET_DATA_REQ, EGS_SELECT_SERVER_SET_REQ,
//                           EGS_MY_UNIT_AND_INVENTORY_INFO_LIST_REQ, EGS_CREATE_UNIT_REQ,
//                           EGS_DELETE_UNIT_REQ, EGS_SELECT_UNIT_REQ
//
// LoginService keeps the same packets and the same reply codes (Common/NetError_def.h) but answers
// them in one process against kdb; the only thing that still crosses a socket is the client moving
// from the login port to the channel port, exactly as it did before.
//
// The service is deliberately transport agnostic: `handle()` takes a decoded event and a small
// responder interface, so the packet handling can be unit tested without sockets, while
// kserver::ServerSession binds it to a real knc::net::Session.
#pragma once

#include <cstdint>
#include <string>

#include "kcore/Types.h"
#include "kdb/Repositories.h"
#include "kproto/Event.h"
#include "kproto/LoginPackets.h"
#include "kserver/AccountSession.h"
#include "kserver/Config.h"

namespace knc::server {

// NetErrorEnum values used by the login flow, numbered exactly as in Common/NetError_def.h
// (the enum starts at NET_OK = 0 and every entry advances by one).
enum NetError : int {
    kNetOk = 0,                        // NET_OK
    kErrUnknown = 1,                   // ERR_UNKNOWN
    kErrClientState = 2,               // ERR_CLIENT_STATE
    kErrConnectVersion = 3,            // ERR_CONNECT_00  version mismatch
    kErrConnectFull = 4,               // ERR_CONNECT_04  server full
    kErrConnectMaintenance = 5,        // ERR_CONNECT_05  under maintenance
    kErrVerifySameServer = 6,          // ERR_VERIFY_00
    kErrVerifyOtherServer = 7,         // ERR_VERIFY_01
    kErrVerifyIpMismatch = 8,          // ERR_VERIFY_02
    kErrVerifyUidConflict = 9,         // ERR_VERIFY_03
    kErrVerifyNoUser = 10,             // ERR_VERIFY_04
    kErrVerifyDeletedUser = 11,        // ERR_VERIFY_05
    kErrVerifyBadPassword = 12,        // ERR_VERIFY_06
    kErrVerifyBlocked = 15,            // ERR_VERIFY_11
    kErrVerifyNoChannelId = 19,        // ERR_VERIFY_15
    kErrVerifyIdLength = 20,           // ERR_VERIFY_16
    kErrNewAccountTransaction = 21,    // ERR_NEW_ACCOUNT_02
    kErrNewAccountIdLength = 22,       // ERR_NEW_ACCOUNT_03
    kErrNewAccountPasswordLength = 23, // ERR_NEW_ACCOUNT_04
    kErrNewAccountNameLength = 24,     // ERR_NEW_ACCOUNT_06
    kErrCreateUnitDeletedUser = 25,    // ERR_CREATE_UNIT_00
    kErrCreateUnitNameTaken = 26,      // ERR_CREATE_UNIT_01
    kErrCreateUnitTransaction = 27,    // ERR_CREATE_UNIT_02
    kErrCreateUnitNickLength = 29,     // ERR_CREATE_UNIT_04
    kErrCreateUnitTooMany = 30,        // ERR_CREATE_UNIT_05
    kErrCreateUnitNameReserved = 31,   // ERR_CREATE_UNIT_06
    kErrCreateUnitNameForbidden = 33,  // ERR_CREATE_UNIT_08
    kErrCreateUnitDailyLimit = 34,     // ERR_CREATE_UNIT_09
    kErrSelectUnitNotFound = 35,       // ERR_SELECT_UNIT_00
    kErrSelectUnitBadClass = 36,       // ERR_SELECT_UNIT_01
    kErrSelectUnitDbError = 39,        // ERR_SELECT_UNIT_05
};

// Everything the service needs from the transport.
class LoginResponder {
public:
    virtual ~LoginResponder() = default;
    // Sends one event; the implementation fills in the KEvent envelope (performer, uid, trace).
    virtual bool send(EventId id, const SerBuffer& payload) = 0;
    virtual void disconnect(const std::string& reason) = 0;
    virtual const std::string& peerIp() const = 0;
    // Server side counters used by the capacity check (login port connections).
    virtual int connectedUsers() const = 0;
};

class LoginService {
public:
    struct Options {
        WideString expectedVersion;   // KEGS_CONNECT_REQ.m_wstrVersion must equal this
        int maxUser = 500;            // login port capacity
        bool allowGuestAccounts = true;
        std::int64_t unitDeleteReuseDelayMs = 7LL * 24 * 60 * 60 * 1000;  // SERV_UNIT_WAIT_DELETE
        unsigned int authFailLimit = 100;
    };

    LoginService(db::Repositories& repos, const Config& config);

    void setOptions(const Options& options) { options_ = options; }
    const Options& options() const { return options_; }

    // Decodes and handles one login-port event. Returns false when the connection should be
    // dropped after the reply has been queued.
    bool handle(AccountSession& state, LoginResponder& responder, Event& event);

    // Individual handlers, also used by the tests.
    bool handleConnect(AccountSession& state, LoginResponder& responder, const SerBuffer& payload);
    bool handleVerifyAccount(AccountSession& state, LoginResponder& responder, const SerBuffer& payload);
    bool handleNewUser(AccountSession& state, LoginResponder& responder, const SerBuffer& payload);

    // Converts a stored account row into the wire KAccountInfo the client receives.
    static AccountInfo toAccountInfo(const db::AccountRecord& account);

    // Maps a kdb auth outcome onto the wire error code.
    static int errorFor(db::AuthOutcome outcome);

    long long rejectedConnects() const { return rejectedConnect_; }
    long long acceptedConnects() const { return acceptedConnect_; }
    long long successfulLogins() const { return successfulLogin_; }

private:
    db::Repositories& repos_;
    Config config_;
    Options options_;
    long long rejectedConnect_ = 0;
    long long acceptedConnect_ = 0;
    long long successfulLogin_ = 0;
};

}  // namespace knc::server
