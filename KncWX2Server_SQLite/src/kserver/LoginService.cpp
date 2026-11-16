#include "kserver/LoginService.h"

#include <chrono>

#include "kcore/Log.h"
#include "kcore/Serialize.h"
#include "kgame/Rules.h"
#include "kproto/Packet.h"

namespace knc::server {
namespace {

std::int64_t nowMs() {
    return std::chrono::duration_cast<std::chrono::milliseconds>(
               std::chrono::system_clock::now().time_since_epoch())
        .count();
}

template <class T>
SerBuffer encode(const T& value) {
    SerBuffer buffer;
    encodePayload(value, buffer);
    return buffer;
}

// Converts the 2014 "YYYY-MM-DD HH:MM:SS" timestamp stored in Accounts rows into the same text the
// client displays; empty stays empty.
WideString timestampToWide(std::int64_t epochMs) {
    if (epochMs <= 0) return WideString();
    const std::time_t seconds = static_cast<std::time_t>(epochMs / 1000);
    std::tm parts{};
#if defined(_WIN32)
    gmtime_s(&parts, &seconds);
#else
    gmtime_r(&seconds, &parts);
#endif
    char text[64];
    std::snprintf(text, sizeof(text), "%04d-%02d-%02d %02d:%02d:%02d", parts.tm_year + 1900,
                  parts.tm_mon + 1, parts.tm_mday, parts.tm_hour, parts.tm_min, parts.tm_sec);
    return toWide(text);
}

}  // namespace

LoginService::LoginService(db::Repositories& repos, const Config& config)
    : repos_(repos), config_(config) {
    options_.expectedVersion = config.version();
    options_.maxUser = config.login.maxUser;
    options_.authFailLimit = config.login.authFailLimit;
}

int LoginService::errorFor(db::AuthOutcome outcome) {
    switch (outcome) {
        case db::AuthOutcome::kOk: return kNetOk;
        case db::AuthOutcome::kNoSuchAccount: return kErrVerifyNoUser;
        case db::AuthOutcome::kBadPassword: return kErrVerifyBadPassword;
        case db::AuthOutcome::kBlocked: return kErrVerifyBlocked;
        case db::AuthOutcome::kDeleted: return kErrVerifyDeletedUser;
        case db::AuthOutcome::kDatabaseError: return kErrUnknown;
    }
    return kErrUnknown;
}

AccountInfo LoginService::toAccountInfo(const db::AccountRecord& account, bool isRecommend) {
    AccountInfo info;
    info.userUid = account.uid;
    info.id = toWide(account.loginId);
    info.name = toWide(account.displayName);
    info.authLevel = account.authLevel;
    info.internalUser = account.internalUser;
    info.option.playGuide = false;
    info.blockInfo.endTime = account.blockInfo.endTime;
    info.blockInfo.blockReason = account.blockInfo.blockReason;
    // The 2013 extra block fields (SERV_HACKING_USER_CHECK_COUNT) are not on the wire.
    info.isRecommend = isRecommend;
    info.isGuestUser = account.guestUser;
    info.regDate = timestampToWide(account.createdAt);
    info.lastLogin = timestampToWide(account.lastLoginAt);
    return info;
}

bool LoginService::handleConnect(AccountSession& state, LoginResponder& responder,
                                const SerBuffer& payload) {
    ConnectReq request;
    if (!decodePayload(payload, request, /*lenient=*/true)) {
        KLOG(LogLevel::kWarn, "login") << responder.peerIp() << " malformed EGS_CONNECT_REQ";
        ConnectAck ack;
        ack.ok = kErrClientState;
        responder.send(EGS_CONNECT_ACK, encode(ack));
        return false;
    }

    // KGSUser::OnConnectReq: compare m_wstrVersion with GetMainVersion().
    if (request.version != options_.expectedVersion) {
        ++rejectedConnect_;
        KLOG(LogLevel::kInfo, "login") << responder.peerIp() << " version mismatch, client '"
                                      << toUtf8(request.version) << "' server '"
                                      << toUtf8(options_.expectedVersion) << "'";
        ConnectAck ack;
        ack.ok = kErrConnectVersion;
        responder.send(EGS_CONNECT_ACK, encode(ack));
        return false;
    }

    // Capacity check, including the connection that is asking (iNowUser - 1 >= iMaxUser).
    if (options_.maxUser > 0 && responder.connectedUsers() - 1 >= options_.maxUser) {
        ++rejectedConnect_;
        KLOG(LogLevel::kWarn, "login") << responder.peerIp() << " rejected: server full ("
                                      << responder.connectedUsers() << "/" << options_.maxUser << ")";
        ConnectAck ack;
        ack.ok = kErrConnectFull;
        responder.send(EGS_CONNECT_ACK, encode(ack));
        return false;
    }

    ++acceptedConnect_;
    state.state = LoginState::kConnectedClient;

    ConnectAck ack;
    ack.ok = kNetOk;
    ack.udpPort = static_cast<int>(config_.channels.empty() ? 0 : config_.channels.front().ncUdpPort);
    ack.channelId = config_.channels.empty() ? 0 : config_.channels.front().channelId;
    responder.send(EGS_CONNECT_ACK, encode(ack));
    return true;
}

bool LoginService::handleVerifyAccount(AccountSession& state, LoginResponder& responder,
                                       const SerBuffer& payload) {
    UserAuthenticateReq request;
    if (!decodePayload(payload, request, /*lenient=*/true)) {
        KLOG(LogLevel::kWarn, "login") << responder.peerIp() << " malformed EGS_VERIFY_ACCOUNT_REQ";
        VerifyAccountAck ack;
        ack.ok = kErrClientState;
        responder.send(EGS_VERIFY_ACCOUNT_ACK, encode(ack));
        return false;
    }

    const std::string loginId = toUtf8(request.userId);
    const std::string password = toUtf8(request.password);
    // ERR_VERIFY_16 guards the *shape* of the id a client sends, not the minimum length of an
    // existing account: the 2014 deployment shipped 2 character ids (the seeded gm/gm pair among
    // them), so the minimum length rule only applies to newly created accounts.
    if (loginId.empty() || loginId.size() > 32) {
        VerifyAccountAck ack;
        ack.ok = kErrVerifyIdLength;
        responder.send(EGS_VERIFY_ACCOUNT_ACK, encode(ack));
        return false;
    }

    std::string error;
    db::AccountRecord account;
    const db::AuthOutcome outcome =
        repos_.accounts.authenticate(loginId, password, responder.peerIp(), &account, &error);
    if (outcome != db::AuthOutcome::kOk) {
        if (outcome == db::AuthOutcome::kDatabaseError) {
            KLOG(LogLevel::kError, "login") << loginId << " authentication failed: " << error;
        } else {
            KLOG(LogLevel::kInfo, "login") << loginId << " authentication rejected ("
                                          << errorFor(outcome) << ")";
        }
        VerifyAccountAck ack;
        ack.ok = errorFor(outcome);
        if (outcome == db::AuthOutcome::kBlocked) {
            ack.accountInfo = toAccountInfo(
                account, repos_.accounts.getOption(account.uid, db::kOptIsRecommend) != 0);
        }
        responder.send(EGS_VERIFY_ACCOUNT_ACK, encode(ack));
        return false;
    }

    state.account = account;
    state.authenticated = true;
    state.state = LoginState::kAuthenticated;
    state.machineId = request.machineId;
    state.clientIp = responder.peerIp();
    state.connectedAt = nowMs();
    ++successfulLogin_;

    repos_.accounts.recordLogin(account.uid, loginId, responder.peerIp(), request.machineId,
                                config_.name, kNetOk);

    AccountInfo info =
        toAccountInfo(account, repos_.accounts.getOption(account.uid, db::kOptIsRecommend) != 0);

    VerifyAccountAck ack;
    ack.ok = kNetOk;
    ack.accountInfo = info;
    ack.sex = account.sex != 0;
    ack.age = static_cast<UINT32>(account.age < 0 ? 0 : account.age);
    ack.nexonSn = account.nexonSn;
    ack.clientIp = toWide(responder.peerIp());
    responder.send(EGS_VERIFY_ACCOUNT_ACK, encode(ack));

    KLOG(LogLevel::kInfo, "login") << loginId << " (uid " << account.uid << ") logged in from "
                                  << responder.peerIp();
    return true;
}

bool LoginService::handleNewUser(AccountSession& state, LoginResponder& responder,
                                 const SerBuffer& payload) {
    NewUserJoinReq request;
    if (!decodePayload(payload, request, true)) {
        NewUserJoinAck ack;
        ack.ok = kErrClientState;
        responder.send(EGS_NEW_USER_JOIN_ACK, encode(ack));
        return false;
    }

    const std::string loginId = toUtf8(request.id);
    const std::string password = toUtf8(request.password);
    const std::string name = toUtf8(request.name);

    const knc::game::NicknameVerdict idVerdict = knc::game::validateLoginId(loginId);
    const knc::game::NicknameVerdict passwordVerdict = knc::game::validatePassword(password);

    NewUserJoinAck ack;
    if (idVerdict != knc::game::NicknameVerdict::kOk) {
        ack.ok = kErrNewAccountIdLength;
    } else if (passwordVerdict != knc::game::NicknameVerdict::kOk) {
        ack.ok = kErrNewAccountPasswordLength;
    } else if (request.guestUser != 0 && !options_.allowGuestAccounts) {
        ack.ok = kErrNewAccountTransaction;
    } else if (name.empty() || name.size() > 32) {
        ack.ok = kErrNewAccountNameLength;
    } else {
        std::int64_t uid = 0;
        std::string error;
        if (repos_.accounts.create(loginId, password, name, request.guestUser != 0, &uid, &error)) {
            ack.ok = kNetOk;
            ack.userUid = uid;
            KLOG(LogLevel::kInfo, "login") << "created account " << loginId << " (uid " << uid << ")";
        } else {
            // A duplicate login id surfaces as the UNIQUE(LoginID) violation.
            ack.ok = kErrNewAccountTransaction;
            KLOG(LogLevel::kInfo, "login") << "account creation failed for " << loginId << ": " << error;
        }
    }
    (void)state;
    responder.send(EGS_NEW_USER_JOIN_ACK, encode(ack));
    return ack.ok == kNetOk;
}

bool LoginService::handle(AccountSession& state, LoginResponder& responder, Event& event) {
    switch (event.eventId) {
        case E_HEART_BEAT:
            return true;

        case EGS_CONNECT_REQ:
            return handleConnect(state, responder, event.payload);

        case EGS_VERIFY_ACCOUNT_REQ:
            if (state.state != LoginState::kConnectedClient) {
                KLOG(LogLevel::kWarn, "login") << responder.peerIp()
                                               << " EGS_VERIFY_ACCOUNT_REQ before connect";
                VerifyAccountAck ack;
                ack.ok = kErrClientState;
                responder.send(EGS_VERIFY_ACCOUNT_ACK, encode(ack));
                return false;
            }
            return handleVerifyAccount(state, responder, event.payload);

        case EGS_NEW_USER_JOIN_REQ:
            return handleNewUser(state, responder, event.payload);

        default:
            KLOG(LogLevel::kWarn, "login") << responder.peerIp() << " unhandled login event "
                                           << event.describe();
            return true;
    }
}

}  // namespace knc::server
