#include "kserver/ChannelService.h"

#include <algorithm>

#include "kcore/Log.h"
#include "kcore/Serialize.h"
#include "kgame/Character.h"
#include "kgame/ExpTable.h"
#include "kgame/Rules.h"
#include "kproto/LoginPackets.h"
#include "kproto/Packet.h"

namespace knc::server {
namespace {

template <class T>
SerBuffer encode(const T& value) {
    SerBuffer buffer;
    encodePayload(value, buffer);
    return buffer;
}

// KServerSetData::USER_COUNT_LEVEL from the current/maximum pair.
char userCountLevel(int current, int max) {
    if (max <= 0) return ServerSetData::kUclFree;
    if (current >= max) return ServerSetData::kUclFull;
    const double ratio = static_cast<double>(current) / static_cast<double>(max);
    if (ratio < 0.3) return ServerSetData::kUclFree;
    if (ratio < 0.7) return ServerSetData::kUclNormal;
    return ServerSetData::kUclBusy;
}

int errorForCreate(db::CreateUnitReason reason) {
    switch (reason) {
        case db::CreateUnitReason::kOk: return kNetOk;
        case db::CreateUnitReason::kNameTaken: return kErrCreateUnitNameTaken;
        case db::CreateUnitReason::kNoSlot: return kErrCreateUnitTooMany;
        case db::CreateUnitReason::kInvalidClass: return kErrCreateUnitNickLength;
        case db::CreateUnitReason::kDatabaseError: return kErrCreateUnitTransaction;
    }
    return kErrCreateUnitTransaction;
}

int errorForNickname(knc::game::NicknameVerdict verdict) {
    switch (verdict) {
        case knc::game::NicknameVerdict::kOk: return kNetOk;
        case knc::game::NicknameVerdict::kTooShort:
        case knc::game::NicknameVerdict::kTooLong: return kErrCreateUnitNickLength;
        case knc::game::NicknameVerdict::kAllDigits: return kErrCreateUnitNameForbidden;
        case knc::game::NicknameVerdict::kBadCharacter: return kErrCreateUnitNameForbidden;
    }
    return kErrCreateUnitNameForbidden;
}

}  // namespace

bool ChannelService::requireAuthenticated(AccountSession& state, LoginResponder& responder,
                                         const char* what) {
    if (state.authenticated) return true;
    KLOG(LogLevel::kWarn, "channel") << responder.peerIp() << " " << what
                                     << " from an unauthenticated session";
    return false;
}

UnitInfo ChannelService::toUnitInfo(const db::UnitRecord& unit) const {
    UnitInfo info;
    info.ownerUserUid = unit.accountUid;
    info.authLevel = static_cast<char>(unit.authLevel);
    info.unitUid = unit.unitUid;
    info.knmSerialNum = static_cast<unsigned int>(unit.knmSerial);
    info.unitClass = static_cast<char>(unit.unitClass);
    info.nickName = toWide(unit.nickName);
    info.ed = static_cast<int>(unit.ed > 0x7FFFFFFF ? 0x7FFFFFFF : unit.ed);
    info.level = static_cast<UCHAR>(std::clamp(unit.level, 0, 255));
    info.exp = static_cast<int>(unit.exp > 0x7FFFFFFF ? 0x7FFFFFFF : unit.exp);

    info.officialMatchCnt = unit.officialMatch;
    info.rating = unit.rating;
    info.maxRating = unit.maxRating;
    info.rPoint = unit.rPoint;
    info.aPoint = unit.aPoint;
    info.rank = static_cast<char>(unit.rank);
    info.kFactor = static_cast<float>(unit.kFactor);

    info.sPoint = unit.sp;
    info.csPoint = unit.csp;
    info.maxCsPoint = unit.maxCsp;
    info.csPointEndDate = toWide(unit.cspEndDate);
    info.straightVictories = unit.straightWins;

    // Base and game stats come from the same level curve the game rules use.
    const knc::game::Character character = knc::game::Character::fromRecord(unit);
    const knc::game::BaseStat& base = character.totalStat();
    info.stat.baseHp = base.baseHp;
    info.stat.atkPhysic = base.atkPhysic;
    info.stat.atkMagic = base.atkMagic;
    info.stat.defPhysic = base.defPhysic;
    info.stat.defMagic = base.defMagic;
    info.gameStat = info.stat;

    const knc::game::ExpTable& exp = knc::game::defaultExpTable();
    info.nowBaseLevelExp = static_cast<int>(exp.expWithinLevel(unit.level, unit.exp));
    const std::int64_t next = exp.expToNextLevel(unit.level);
    info.nextBaseLevelExp = next > 0x7FFFFFFF ? 0x7FFFFFFF : static_cast<int>(next);

    info.lastPos.mapId = unit.mapId;
    info.lastPos.lastTouchLineIndex = 0;
    info.lastPos.lastPosValue = 0;
    info.lastLoginTime = toWide(unit.lastTown);

    // GUILD_TEST is on in this build, so the guild block is always on the wire. A character
    // without a guild sends the zeroed block the 2014 server sent (uid 0, empty name).
    db::GuildRecord guild;
    if (repos_.social.loadGuildOf(unit.unitUid, &guild)) {
        info.guildInfo.guildUid = static_cast<int>(guild.guildUid);
        info.guildInfo.guildName = toWide(guild.name);
    }
    return info;
}

ChannelBonusInfo ChannelService::bonusInfoFor(const ChannelConfig& channel) {
    ChannelBonusInfo info;
    info.enable = channel.bonusExpPercent > 0 || channel.bonusDropPercent > 0;
    // The 2014 table applied a bonus to a level range; a whole-channel event covers every level.
    info.beginLevel = 1;
    info.endLevel = 99;
    info.perExp = channel.bonusExpPercent;
    info.perEd = channel.bonusDropPercent;
    return info;
}

ChannelInfo ChannelService::toChannelInfo(const ChannelConfig& channel) const {
    ChannelInfo info;
    info.serverUid = static_cast<int>(channel.serverUid);
    info.channelId = channel.channelId;
    info.channelName = channel.displayName;
    info.serverGroupId = channel.serverSetId;
    info.ip = toWide(channel.ip);
    info.masterPort = channel.masterPort;
    info.ncUdpPort = channel.ncUdpPort;
    info.maxUser = channel.maxUser;
    info.currentUser = 0;
    info.curPartyCount = 0;
    info.playGamePartyCount = 0;

    return info;
}

bool ChannelService::handleServerSetList(AccountSession& state, LoginResponder& responder) {
    if (!requireAuthenticated(state, responder, "EGS_GET_SERVER_SET_DATA_REQ")) return true;

    GetServerSetDataAck ack;
    ack.ok = kNetOk;
    for (const auto& set : config_.serverSets) {
        ServerSetData data;
        data.serverSetId = set.id;
        data.serverSetName = set.name;
        data.userCountLevel = userCountLevel(0, set.maxUser);
        ack.serverSetList.push_back(data);
    }
    responder.send(EGS_GET_SERVER_SET_DATA_ACK, encode(ack));
    return true;
}

bool ChannelService::handleSelectServerSet(AccountSession& state, LoginResponder& responder,
                                          const SerBuffer& payload) {
    SelectServerSetReq request;
    if (!requireAuthenticated(state, responder, "EGS_SELECT_SERVER_SET_REQ")) return true;
    if (!decodePayload(payload, request, true)) {
        SelectServerSetAck ack;
        ack.ok = kErrClientState;
        responder.send(EGS_SELECT_SERVER_SET_ACK, encode(ack));
        return true;
    }

    const ServerSetConfig* chosen = nullptr;
    for (const auto& set : config_.serverSets) {
        if (set.id == request.serverSetId) {
            chosen = &set;
            break;
        }
    }

    SelectServerSetAck ack;
    if (!chosen) {
        ack.ok = kErrVerifyNoChannelId;
    } else {
        ack.ok = kNetOk;
        ack.ip = toWide(chosen->ip);
        ack.port = chosen->port;
        state.serverSetId = chosen->id;
        // The 2014 client showed the block reason on this screen when the account was punished.
        ack.accountBlockInfo.endTime = state.account.blockInfo.endTime;
        ack.accountBlockInfo.blockReason = state.account.blockInfo.blockReason;
    }
    responder.send(EGS_SELECT_SERVER_SET_ACK, encode(ack));
    return true;
}

bool ChannelService::handleChannelList(AccountSession& state, LoginResponder& responder) {
    if (!requireAuthenticated(state, responder, "EGS_GET_CHANNEL_LIST_REQ")) return true;

    GetChannelListAck ack;
    ack.serverGroupId = state.serverSetId == 0
                            ? config_.serverSets.front().id
                            : state.serverSetId;
    for (const auto& channel : config_.channels) {
        ack.channelInfo.push_back(toChannelInfo(channel));
        // KChannelInfo itself does not carry the bonus (SERV_INTEGRATE_THE_CHANNEL_INFO is off);
        // the per-channel table travels in the map instead.
        ack.channelBonusInfo[channel.channelId] = bonusInfoFor(channel);
    }
    responder.send(EGS_GET_CHANNEL_LIST_ACK, encode(ack));
    return true;
}

bool ChannelService::handleUnitList(AccountSession& state, LoginResponder& responder) {
    if (!requireAuthenticated(state, responder, "EGS_MY_UNIT_AND_INVENTORY_INFO_LIST_REQ")) return true;

    MyUnitAndInventoryInfoListAck ack;
    ack.ok = kNetOk;
    std::vector<db::UnitSummary> summaries;
    std::string error;
    if (!repos_.units.listSummaries(state.account.uid, &summaries, &error)) {
        KLOG(LogLevel::kError, "channel") << "unit list failed: " << error;
        ack.ok = kErrUnknown;
        responder.send(EGS_MY_UNIT_AND_INVENTORY_INFO_LIST_ACK, encode(ack));
        return true;
    }

    for (const auto& summary : summaries) {
        db::UnitRecord unit;
        if (!repos_.units.load(summary.unitUid, &unit)) continue;
        ack.unitInfo.push_back(toUnitInfo(unit));
        ack.unitSlot = std::max(ack.unitSlot, summary.slot + 1);
    }
    // The client wants empty slots up to the page size, and the 2014 server answered with the
    // number of slots the account owns rather than the number of characters.
    ack.unitSlot = std::max(ack.unitSlot, ChannelLimits::kMaxUnitsPerAccount);
    state.state = LoginState::kUnitList;

    responder.send(EGS_MY_UNIT_AND_INVENTORY_INFO_LIST_ACK, encode(ack));
    KLOG(LogLevel::kDebug, "channel") << state.account.loginId << " unit list (" << ack.unitInfo.size()
                                     << " units)";
    return true;
}

bool ChannelService::handleCreateUnit(AccountSession& state, LoginResponder& responder,
                                     const SerBuffer& payload) {
    CreateUnitReq request;
    if (!requireAuthenticated(state, responder, "EGS_CREATE_UNIT_REQ")) return true;
    if (!decodePayload(payload, request, true)) {
        CreateUnitAck ack;
        ack.ok = kErrClientState;
        responder.send(EGS_CREATE_UNIT_ACK, encode(ack));
        return true;
    }

    CreateUnitAck ack;
    if (state.account.blocked) {
        ack.ok = kErrCreateUnitDeletedUser;
        responder.send(EGS_CREATE_UNIT_ACK, encode(ack));
        return true;
    }

    const knc::game::NicknameVerdict verdict = knc::game::validateNickname(request.nickName);
    if (verdict != knc::game::NicknameVerdict::kOk) {
        ack.ok = errorForNickname(verdict);
        responder.send(EGS_CREATE_UNIT_ACK, encode(ack));
        return true;
    }

    if (repos_.units.countForAccount(state.account.uid) >= ChannelLimits::kMaxUnitsPerAccount) {
        ack.ok = kErrCreateUnitTooMany;
        responder.send(EGS_CREATE_UNIT_ACK, encode(ack));
        return true;
    }
    if (repos_.units.countCreatedToday(state.account.uid) >= ChannelLimits::kMaxUnitsCreatedPerDay) {
        ack.ok = kErrCreateUnitDailyLimit;
        responder.send(EGS_CREATE_UNIT_ACK, encode(ack));
        return true;
    }

    // Pick the first free slot: the client sends only nickname + class.
    int slot = 0;
    std::vector<db::UnitSummary> existing;
    if (repos_.units.listSummaries(state.account.uid, &existing)) {
        std::vector<bool> used(ChannelLimits::kMaxUnitsPerAccount, false);
        for (const auto& unit : existing) {
            if (unit.slot >= 0 && unit.slot < ChannelLimits::kMaxUnitsPerAccount) used[unit.slot] = true;
        }
        while (slot < ChannelLimits::kMaxUnitsPerAccount && used[slot]) ++slot;
        if (slot >= ChannelLimits::kMaxUnitsPerAccount) {
            ack.ok = kErrCreateUnitTooMany;
            responder.send(EGS_CREATE_UNIT_ACK, encode(ack));
            return true;
        }
    }

    const std::string nickName = toUtf8(request.nickName);
    const db::CreateUnitResult created =
        repos_.units.create(state.account.uid, slot, nickName, request.unitClass, 1);
    if (!created.ok) {
        ack.ok = errorForCreate(created.reason);
        KLOG(LogLevel::kInfo, "channel") << state.account.loginId << " create unit '" << nickName
                                         << "' failed: " << created.message;
        responder.send(EGS_CREATE_UNIT_ACK, encode(ack));
        return true;
    }

    db::UnitRecord unit;
    if (!repos_.units.load(created.unitUid, &unit)) {
        ack.ok = kErrCreateUnitTransaction;
        responder.send(EGS_CREATE_UNIT_ACK, encode(ack));
        return true;
    }

    // The 2014 CreateUnitCount table limited creations to four per account per day.
    repos_.progress.log(/*category=*/1, /*level=*/2, config_.name, state.account.uid, unit.unitUid,
                        "create unit " + nickName);

    ack.ok = kNetOk;
    ack.unitInfo = toUnitInfo(unit);
    ack.skillSlot = {0, 0, 0, 0};
    ack.enableDate = u"";
    ++unitsCreated_;
    responder.send(EGS_CREATE_UNIT_ACK, encode(ack));
    KLOG(LogLevel::kInfo, "channel") << state.account.loginId << " created unit '" << nickName << "' ("
                                     << unit.unitUid << ", class " << unit.unitClass << ")";
    return true;
}

bool ChannelService::handleDeleteUnit(AccountSession& state, LoginResponder& responder,
                                      const SerBuffer& payload) {
    DeleteUnitReq request;
    if (!requireAuthenticated(state, responder, "EGS_DELETE_UNIT_REQ")) return true;
    if (!decodePayload(payload, request, true)) {
        DeleteUnitAck ack;
        ack.ok = kErrClientState;
        responder.send(EGS_DELETE_UNIT_ACK, encode(ack));
        return true;
    }

    DeleteUnitAck ack;
    db::UnitRecord unit;
    if (!repos_.units.load(request.unitUid, &unit) || unit.accountUid != state.account.uid) {
        // Deleting somebody else's character must look exactly like a missing character.
        ack.ok = kErrSelectUnitNotFound;
        responder.send(EGS_DELETE_UNIT_ACK, encode(ack));
        return true;
    }

    ack.unitUid = unit.unitUid;
    ack.knmSerialNum = static_cast<UINT32>(unit.knmSerial);
    db::GuildRecord guild;
    if (repos_.social.loadGuildOf(unit.unitUid, &guild)) {
        ack.guildUid = static_cast<int>(guild.guildUid);
    }
    ack.nickName = toWide(unit.nickName);
    ack.level = static_cast<UCHAR>(std::clamp(unit.level, 0, 255));
    ack.unitClass = static_cast<char>(unit.unitClass);

    if (!repos_.units.remove(unit.unitUid, deleteReuseDelayMs_)) {
        ack.ok = kErrCreateUnitTransaction;
        responder.send(EGS_DELETE_UNIT_ACK, encode(ack));
        return true;
    }

    ++unitsDeleted_;
    ack.ok = kNetOk;
    responder.send(EGS_DELETE_UNIT_ACK, encode(ack));
    KLOG(LogLevel::kInfo, "channel") << state.account.loginId << " deleted unit '" << unit.nickName
                                     << "' (" << unit.unitUid << ")";
    return true;
}

bool ChannelService::handleSelectUnit(AccountSession& state, LoginResponder& responder,
                                      const SerBuffer& payload) {
    SelectUnitReq request;
    if (!requireAuthenticated(state, responder, "EGS_SELECT_UNIT_REQ")) return true;
    if (!decodePayload(payload, request, true)) {
        SelectUnitAck ack;
        ack.ok = kErrClientState;
        responder.send(EGS_SELECT_UNIT_ACK, encode(ack));
        return true;
    }

    SelectUnitAck ack;
    db::UnitRecord unit;
    if (!repos_.units.load(request.unitUid, &unit) || unit.accountUid != state.account.uid) {
        ack.ok = kErrSelectUnitNotFound;
        responder.send(EGS_SELECT_UNIT_ACK, encode(ack));
        return true;
    }
    if (unit.unitClass < 0 || unit.unitClass > 128) {
        ack.ok = kErrSelectUnitBadClass;
        responder.send(EGS_SELECT_UNIT_ACK, encode(ack));
        return true;
    }

    state.unitUid = unit.unitUid;
    state.state = LoginState::kInChannel;
    ack.ok = kNetOk;
    responder.send(EGS_SELECT_UNIT_ACK, encode(ack));
    KLOG(LogLevel::kInfo, "channel") << state.account.loginId << " selected unit '" << unit.nickName
                                     << "' (" << unit.unitUid << ")";
    return true;
}

bool ChannelService::handleInventory(AccountSession& state, LoginResponder& responder) {
    if (!requireAuthenticated(state, responder, "EGS_GET_MY_INVENTORY_REQ")) return true;
    if (state.unitUid == 0) {
        GetMyInventoryAck ack;
        ack.ok = kErrClientState;
        responder.send(EGS_GET_MY_INVENTORY_ACK, encode(ack));
        return true;
    }

    GetMyInventoryAck ack;
    ack.ok = kNetOk;
    ack.isRecommend = false;
    responder.send(EGS_GET_MY_INVENTORY_ACK, encode(ack));

    for (int category = 1; category <= 5; ++category) {
        std::vector<db::ItemRecord> items;
        if (!repos_.inventory.listForUnit(state.unitUid, category, &items)) continue;
        SelectUnitInventoryInfoNot notice;
        notice.slotCategory = category;
        for (const auto& item : items) {
            InventoryItemInfo wire;
            wire.itemUid = item.itemUid;
            wire.slotCategory = static_cast<char>(category);
            wire.slotId = static_cast<short>(item.slotIndex);
            wire.itemInfo.itemId = item.itemCode;
            wire.itemInfo.quantity = item.quantity;
            wire.itemInfo.endurance = static_cast<short>(item.endurance);
            wire.itemInfo.enchantLevel = static_cast<char>(item.enchantLevel);
            for (const auto& attribute : item.attributes) {
                char* slots[3] = {&wire.itemInfo.attribEnchantInfo.attribEnchant0,
                                  &wire.itemInfo.attribEnchantInfo.attribEnchant1,
                                  &wire.itemInfo.attribEnchantInfo.attribEnchant2};
                if (attribute.index >= 0 && attribute.index < 3) {
                    *slots[attribute.index] = static_cast<char>(attribute.type);
                }
            }
            for (const auto& socket : item.sockets) {
                wire.itemInfo.itemSocket.push_back(socket.value);
                wire.itemInfo.randomSocket.push_back(socket.type);
            }
            wire.itemInfo.expirationDate = toWide(item.periodEnd);
            wire.itemInfo.itemState = ItemInfo::kIsNormal;
            notice.items[item.itemUid] = wire;
        }
        responder.send(EGS_SELECT_UNIT_INVENTORY_INFO_NOT, encode(notice));
    }
    return true;
}

bool ChannelService::handle(AccountSession& state, LoginResponder& responder, Event& event) {
    switch (event.eventId) {
        case E_HEART_BEAT:
            return true;
        case EGS_GET_SERVER_SET_DATA_REQ:
            return handleServerSetList(state, responder);
        case EGS_SELECT_SERVER_SET_REQ:
            return handleSelectServerSet(state, responder, event.payload);
        case EGS_GET_CHANNEL_LIST_REQ:
        case ECH_GET_CHANNEL_LIST_REQ:
            return handleChannelList(state, responder);
        case EGS_MY_UNIT_AND_INVENTORY_INFO_LIST_REQ:
            return handleUnitList(state, responder);
        case EGS_CREATE_UNIT_REQ:
            return handleCreateUnit(state, responder, event.payload);
        case EGS_DELETE_UNIT_REQ:
            return handleDeleteUnit(state, responder, event.payload);
        case EGS_SELECT_UNIT_REQ:
            return handleSelectUnit(state, responder, event.payload);
        case EGS_GET_MY_INVENTORY_REQ:
            return handleInventory(state, responder);
        default:
            KLOG(LogLevel::kWarn, "channel") << responder.peerIp() << " unhandled channel event "
                                             << event.describe();
            return true;
    }
}

}  // namespace knc::server
