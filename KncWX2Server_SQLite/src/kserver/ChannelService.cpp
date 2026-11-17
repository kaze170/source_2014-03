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

// GSUserSession.cpp:2086 hard codes the server set name. Configured names win (the rewrite serves
// more than the one set the 2014 build ever shipped), but an unnamed set falls back to the literal
// so the reply still looks like the original.
const WideString kServerSetNameLiteral = WideString(u"\uC194\uB808\uC2A4");  // 솔레스

template <class T>
SerBuffer encode(const T& value) {
    SerBuffer buffer;
    encodePayload(value, buffer);
    return buffer;
}



// SEnum::UAL_BLOCKED_2..UAL_BLOCKED_9 (-2..-9), plus the SERV_ADD_BLOCK_DAY trio, which the
// shipped enum lists unconditionally. UAL_BLOCKED_1 (-1) is a warning and still plays.
bool isBlockedAuthLevel(int authLevel) {
    return authLevel <= -2 && authLevel >= -9;
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

char userCountLevel(int current, int max) {
    if (max <= 0) max = 1;  // the original logs a warning and substitutes 1
    if (current < 1500) return ServerSetData::kUclFree;
    if (current < 3000) return ServerSetData::kUclNormal;
    if (current < max) return ServerSetData::kUclBusy;
    return ServerSetData::kUclFull;
}

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
    // The 2014 server never trusts the stored level when it builds KUnitInfo: it re-derives it from
    // the exp total (GSUserFunction.cpp:4737, CheckLevelUp( 1, m_iEXP )), which also applies the
    // configured level cap. A crash between the level and exp writes would otherwise show the
    // client a stale level forever.
    const int storedExp = static_cast<int>(unit.exp > 0x7FFFFFFF ? 0x7FFFFFFF : unit.exp);
    info.level = static_cast<UCHAR>(
        std::clamp(knc::game::defaultExpTable().levelForExp(storedExp), 0, 255));
    info.exp = storedExp;

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
    // Straight victories are not reported here: the 2014 server zeroes the field on both paths
    // that build KUnitInfo for the client - the unit list (GSUserGameCommon.cpp:226) and the select
    // ack (GSUserFunction.cpp:4715) - right after the packet comes back from the DB. Whatever the
    // column holds, the client is told 0, so echoing it would change what the client displays.
    info.straightVictories = 0;

    // KUnitInfo carries the class/level row of StatTable. GSUserFunction.cpp:4742-4747 adds
    // GetUnitStat( class, level ) to a packet loaded straight from the database, so equipment is
    // not part of this payload. fromRecord() does not load the unit's items, which is why
    // totalStat() and baseStat() happen to agree here - asking for baseStat() states the intent
    // instead of relying on that coincidence.
    const knc::game::Character character = knc::game::Character::fromRecord(unit);
    const knc::game::BaseStat& base = character.baseStat();
    info.stat.baseHp = base.baseHp;
    info.stat.atkPhysic = base.atkPhysic;
    info.stat.atkMagic = base.atkMagic;
    info.stat.defPhysic = base.defPhysic;
    info.stat.defMagic = base.defMagic;
    // KUnitInfo::m_kGameStat stays zeroed here. It is the *in-battle* stat block - the room code
    // fills it (GSUserFunction.cpp:11828, 12054) - and the character select path never touches it,
    // so the packet that reaches the client carries an Init()'d block. Copying m_kStat into it
    // would tell the client it is already in a fight.

    // m_nNowBaseLevelEXP / m_nNextBaseLevelEXP are cumulative totals, not the window inside the
    // level: GSUserFunction.cpp:4738-4739 fills them with GetRequireTotalExpbyLevel( level ) and
    // GetRequireTotalExpbyLevel( level + 1 ), using the level it just recomputed.
    const knc::game::ExpTable& exp = knc::game::defaultExpTable();
    const int level = static_cast<int>(info.level);
    const auto clampToInt = [](std::int64_t value) {
        if (value > 0x7FFFFFFF) return 0x7FFFFFFF;
        if (value < 0) return 0;
        return static_cast<int>(value);
    };
    info.nowBaseLevelExp = clampToInt(exp.expToReachLevel(level));
    info.nextBaseLevelExp = clampToInt(exp.expToReachLevel(level + 1));

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
    // The live user count of this channel. The 2014 server read it back from the DB column the game
    // server refreshed from KActorManager::GetCount() (GameServer.cpp:1148, ChannelLogDBThread.cpp:339);
    // The process-wide session count is an approximation, not per-channel CCU: it includes other
    // listener roles and is repeated for configured channels. Party counts stay 0: the
    // rewrite has no party system, and the original also published 0 for a channel with no parties
    // (ChannelServer.cpp:546-548 clears both counters on every channel-list update).
    info.currentUser = userCount_ ? userCount_() : 0;
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
        // The original ignores the configured name and sends a literal (GSUserSession.cpp:2086
        // sets serverSetName = L"\uC194\uB808\uC2A4" for every set).
        data.serverSetName = set.name.empty() ? kServerSetNameLiteral : set.name;
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

    SelectServerSetAck ack;

    // The block branch runs before anything else in the original: a punished account is told why
    // and then disconnected, whatever set id it asked for.
    if (isBlockedAuthLevel(state.account.authLevel)) {
        // SERV_ACCOUNT_BLOCK_MESSAGE_RENEWAL is defined nowhere in this deployment, so the 2014
        // build takes the #else path and reports ERR_VERIFY_11 (15) rather than 1293.
        ack.ok = kErrVerifyBlocked;
        ack.accountBlockInfo.endTime = state.account.blockInfo.endTime;
        ack.accountBlockInfo.blockReason = state.account.blockInfo.blockReason;
        responder.send(EGS_SELECT_SERVER_SET_ACK, encode(ack));
        responder.disconnect("account blocked");
        return false;
    }

    // "현재는 서버군이 하나다" - the original never inspects m_ServerSetID. It answers with its own
    // public address and the master port, which is exactly how a client is steered to the channel
    // server. A configured set only contributes those two values.
    const ServerSetConfig& chosen = config_.serverSets.front();
    ack.ok = kNetOk;
    ack.ip = toWide(chosen.ip);
    ack.port = chosen.port;
    state.serverSetId = chosen.id;
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
    // KInventory::Clear() runs when the unit is loaded, and it resets the "already sent" set.
    state.reportedInventoryCategories.clear();
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

    // KInventory::GetSelectUnitInventoryInfo walks ST_EQUIP (1) up to ST_END (16) but skips the two
    // categories that are not reachable from this packet - ST_BANK (8) and ST_E_EQUIP (9) - and
    // emits nothing at all for an empty category. The notices therefore arrive in category order
    // and only for bags that actually hold something.
    for (int category = kFirstInventoryCategory; category < kSentinelInventoryCategory; ++category) {
        if (category == kStBank || category == kStEquipped) continue;

        // "인벤정보는 한번만 준다!" - GetInventoryInfo() marks the category as sent *before* it
        // looks at the items, so a second request only answers with the ack. The mark is per
        // category and lives until the unit is reloaded.
        if (!state.reportedInventoryCategories.insert(category).second) continue;

        std::vector<db::ItemRecord> items;
        if (!repos_.inventory.listForUnit(state.unitUid, category, &items)) continue;
        if (items.empty()) continue;

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
            // KNOWN DEVIATION (see docs/porting.md): KItemInfo's two socket vectors are lists of
            // *option ids* with independent lengths - GSGameDBThread.cpp:9258-9265 and :9189-9197
            // load them from two separate 5-slot arrays and truncate trailing zeroes on their own -
            // while our ItemSockets row is (SocketType, SocketValue), a stat kind plus its amount.
            // Mapping the pair onto the two vectors is not the 2014 encoding; fixing it needs the
            // option id stored in the schema, not a different mapping here.
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

    // The ack comes last: GSUserGameCommon.cpp:1158 sends every notice first and only then
    // EGS_GET_MY_INVENTORY_ACK, so a client that waits for the ack to know the list is complete
    // keeps working.
    GetMyInventoryAck ack;
    ack.ok = kNetOk;
    // "추천인 등록 여부" (m_bIsRecommend of KAccountInfo) tells the client whether this account may
    // still recommend somebody; it is spent when a character is created. The bank flag comes from
    // P_GBankShare_SEL - whether the account has the shared bank unlock.
    ack.isRecommend = repos_.accounts.getOption(state.account.uid, db::kOptIsRecommend) != 0;
    ack.sharingBank = repos_.accounts.getOption(state.account.uid, db::kOptShareBank) != 0;
    responder.send(EGS_GET_MY_INVENTORY_ACK, encode(ack));
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
