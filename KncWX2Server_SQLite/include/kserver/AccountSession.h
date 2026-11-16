// kserver/AccountSession.h - per connection login state.
//
// The 2014 LoginServer kept this in KLoginUser and the ChannelServer mirrored it in KChannelUser:
// which account is authenticated, which unit is being selected, and which channel the client was
// sent to. In the single process build the same state hangs off the Session.
#pragma once

#include <cstdint>
#include <set>
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
    std::string machineId;
    std::string clientIp;
    // KInventory::m_setGetMyInventory: which inventory categories were already handed to the
    // 2014 client ("인벤정보는 한번만 준다!"). Cleared by KInventory::Clear, i.e. when the unit
    // is (re)loaded - see handleSelectUnit.
    std::set<int> reportedInventoryCategories;
    std::int64_t connectedAt = 0;

    void reset() {
        state = LoginState::kConnected;
        account = db::AccountRecord{};
        authenticated = false;
        unitUid = 0;
        channelId = 0;
        serverSetId = 0;
        machineId.clear();
        reportedInventoryCategories.clear();
    }
};

}  // namespace knc::server
