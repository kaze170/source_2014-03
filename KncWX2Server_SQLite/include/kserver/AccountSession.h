// kserver/AccountSession.h - per connection login state.
//
// The 2014 LoginServer kept this in KLoginUser and the ChannelServer mirrored it in KChannelUser:
// which account is authenticated, which unit is being selected, and which channel the client was
// sent to. In the single process build the same state hangs off the Session.
#pragma once

#include <cstdint>
#include <string>

#include "kcore/Types.h"
#include "kdb/Repositories.h"

namespace knc::server {

enum class LoginState {
    kConnected,     // handshake done, nothing else yet
    kConnectedClient,  // EGS_CONNECT_REQ accepted
    kAuthenticated, // EGS_VERIFY_ACCOUNT_REQ accepted
    kUnitList,      // unit list sent, waiting for create/delete/select
    kInChannel,     // unit selected, client moved to the channel port
};

struct AccountSession {
    LoginState state = LoginState::kConnected;
    db::AccountRecord account;
    bool authenticated = false;
    UidType unitUid = 0;
    int channelId = 0;
    int serverSetId = 0;
    int channelRandomKey = 0;
    std::string machineId;
    std::string clientIp;
    std::int64_t connectedAt = 0;

    void reset() {
        state = LoginState::kConnected;
        account = db::AccountRecord{};
        authenticated = false;
        unitUid = 0;
        channelId = 0;
        serverSetId = 0;
        channelRandomKey = 0;
        machineId.clear();
    }
};

}  // namespace knc::server
