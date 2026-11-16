// kserver/ChannelService.h - what the ChannelServer did after the login handshake.
//
// Packet flow (Common/ClientPacket.h), unchanged:
//   EGS_GET_SERVER_SET_DATA_REQ      -> EGS_GET_SERVER_SET_DATA_ACK      (server set list)
//   EGS_SELECT_SERVER_SET_REQ        -> EGS_SELECT_SERVER_SET_ACK        (ip/port of the channel)
//   EGS_GET_CHANNEL_LIST_REQ         -> EGS_GET_CHANNEL_LIST_ACK         (KChannelInfo per channel)
//   EGS_MY_UNIT_AND_INVENTORY_INFO_LIST_REQ
//                                    -> EGS_MY_UNIT_AND_INVENTORY_INFO_LIST_ACK (KUnitInfo list)
//   EGS_CREATE_UNIT_REQ              -> EGS_CREATE_UNIT_ACK
//   EGS_DELETE_UNIT_REQ              -> EGS_DELETE_UNIT_ACK
//   EGS_SELECT_UNIT_REQ              -> EGS_SELECT_UNIT_ACK
//   EGS_GET_MY_INVENTORY_REQ         -> EGS_GET_MY_INVENTORY_ACK + EGS_SELECT_UNIT_INVENTORY_INFO_NOT
//
// The 2014 ChannelServer kept the character list in memory and round-tripped to the game DB for
// every mutation; ChannelService reads and writes kdb directly, so a restart cannot lose a unit.
#pragma once

#include <cstdint>
#include <string>
#include <vector>

#include "kcore/Types.h"
#include "kdb/Repositories.h"
#include "kproto/Event.h"
#include "kproto/UnitPackets.h"
#include "kserver/AccountSession.h"
#include "kserver/Config.h"
#include "kserver/LoginService.h"

namespace knc::server {

// Game rule limits that used to be spread over KHackingProtectionManager / KActorManager.
struct ChannelLimits {
    static constexpr int kMaxUnitsPerAccount = 12;   // 3 pages of 4 in the 2014 client
    static constexpr int kMaxUnitsCreatedPerDay = 4; // ERR_CREATE_UNIT_09
    static constexpr int kDefaultEquipmentSlots = 80;
};

// KServerSetData::USER_COUNT_LEVEL for a (current, max) pair. GSUserSession.cpp:2086 compares
// against the absolute constants 1500 and 3000 and only falls back to the capacity for BUSY/FULL,
// so this is not a percentage. Exposed so the rules can be tested directly.
char userCountLevel(int current, int max);

class ChannelService {
public:
    ChannelService(db::Repositories& repos, const Config& config)
        : repos_(repos), config_(config) {}

    // Handles one channel-port event. Returns false when the connection should be dropped.
    bool handle(AccountSession& state, LoginResponder& responder, Event& event);

    // Individually testable handlers.
    bool handleServerSetList(AccountSession& state, LoginResponder& responder);
    bool handleSelectServerSet(AccountSession& state, LoginResponder& responder, const SerBuffer& payload);
    bool handleChannelList(AccountSession& state, LoginResponder& responder);
    bool handleUnitList(AccountSession& state, LoginResponder& responder);
    bool handleCreateUnit(AccountSession& state, LoginResponder& responder, const SerBuffer& payload);
    bool handleDeleteUnit(AccountSession& state, LoginResponder& responder, const SerBuffer& payload);
    bool handleSelectUnit(AccountSession& state, LoginResponder& responder, const SerBuffer& payload);
    bool handleInventory(AccountSession& state, LoginResponder& responder);

    // Builds the wire KUnitInfo for a stored unit, including the inventory sized stat fields the
    // client shows on the character select screen.
    UnitInfo toUnitInfo(const db::UnitRecord& unit) const;

    // Builds a KChannelInfo row for the channel list reply.
    ChannelInfo toChannelInfo(const ChannelConfig& channel) const;
    // Builds the KChannelBonusInfo the reply's map carries for that channel.
    static ChannelBonusInfo bonusInfoFor(const ChannelConfig& channel);

    void setDeleteReuseDelayMs(std::int64_t value) { deleteReuseDelayMs_ = value; }

    int unitsCreated() const { return unitsCreated_; }
    int unitsDeleted() const { return unitsDeleted_; }

private:
    bool requireAuthenticated(AccountSession& state, LoginResponder& responder, const char* what);

    db::Repositories& repos_;
    Config config_;
    std::int64_t deleteReuseDelayMs_ = 7LL * 24 * 60 * 60 * 1000;  // SERV_UNIT_WAIT_DELETE window
    int unitsCreated_ = 0;
    int unitsDeleted_ = 0;
};

}  // namespace knc::server
