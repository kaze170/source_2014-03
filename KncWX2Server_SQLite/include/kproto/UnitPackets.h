// kproto/UnitPackets.h - KUnitInfo and the character selection / inventory packets.
//
// KUnitInfo is the biggest wire struct in the protocol: the 2014 header carried it across ~340
// lines of DECL_DATA with 30 feature-flag blocks inside. The layout below is that struct with the
// flags of this build's fixed set (include/kproto/FeatureFlags.gen.h) applied, so the byte stream
// matches what a 2014 KR service client expects. The kFeature* constants are kept in the code (as
// `if constexpr`) instead of being deleted, so the mapping back to the original #ifdefs stays
// readable.
#pragma once

#include <cstdint>
#include <map>
#include <string>
#include <vector>

#include "kcore/Serialize.h"
#include "kcore/Types.h"
#include "kproto/FeatureFlags.gen.h"
#include "kproto/LoginPackets.h"
#include "kproto/Packet.h"

namespace knc {

// The feature set is not decoration: the layout below is only correct for the flags this build
// resolves to. These assertions make a future change to tools/gen_featureflags.py fail loudly here
// instead of silently shifting every field of KUnitInfo on the wire.
static_assert(features::kSERV_PVP_NEW_SYSTEM, "KUnitInfo carries the 2011 PvP block");
static_assert(features::kSERV_2012_PVP_SEASON2, "KUnitInfo carries rank/kFactor/pastSeasonWin");
static_assert(features::kSERV_BATTLE_FIELD_SYSTEM, "KUnitInfo carries KLastPositionInfo");
static_assert(features::kSERV_BATTLE_FIELD_BOSS, "KLastPositionInfo and KItemInfo carry their boss fields");
static_assert(features::kSERV_TITLE_DATA_SIZE, "m_iTitleID is an int, not a short");
static_assert(features::kGUILD_TEST, "KUnitInfo carries KUserGuildInfo");
static_assert(features::kSERV_PC_BANG_TYPE, "KUnitInfo carries m_iPcBangType");
static_assert(features::kSERV_REFORM_THE_GATE_OF_DARKNESS, "KUnitInfo carries m_vecBuffInfo");
static_assert(features::kSERV_LIMITED_DUNGEON_PLAY_TIMES, "KUnitInfo carries m_mapDungeonPlay");
static_assert(features::kSERV_ADD_WARP_BUTTON, "KUnitInfo carries m_trWarpVipEndData");
static_assert(features::kSERV_GROW_UP_SOCKET, "KUnitInfo carries the event quest counters");
static_assert(!features::kSERV_UNIT_WAIT_DELETE, "the deleted-unit block is off in this build");
static_assert(features::kSERV_NEW_ITEM_SYSTEM_2013_05, "KItemInfo carries the 2013 item state");
static_assert(features::kSERV_ITEM_OPTION_DATA_SIZE, "KItemInfo sockets are int, not short");
static_assert(features::kSERV_EXPAND_SLOT_ID_DATA_SIZE, "KInventoryItemInfo slot is a short");
static_assert(features::kSERV_SKILL_PAGE_SYSTEM, "KUnitSkillData carries the page counters");
static_assert(features::kGUILD_SKILL_TEST, "KUnitSkillData carries the guild passive skills");
static_assert(features::kSERV_SKILL_NOTE, "KUnitSkillData carries the skill note list");
static_assert(features::kSERV_CODE_EVENT, "EGS_SELECT_UNIT_REQ carries m_setCodeEventScriptID");
static_assert(!features::kSERV_GLOBAL_EVENT_TABLE, "EGS_SELECT_UNIT_REQ carries no global event map");
static_assert(!features::kSERV_GOLD_TICKET, "KItemInfo carries no m_iGoldTicketKeyUID");

// --------------------------------------------------------------------------- KStat
// DECL_DATA( KStat ) - the five integers KUnitInfo carries twice (base and game stat).
struct Stat {
    int baseHp = 0;
    int atkPhysic = 0;
    int atkMagic = 0;
    int defPhysic = 0;
    int defMagic = 0;

    void serialize(Serializer& s) {
        s.io(baseHp);
        s.io(atkPhysic);
        s.io(atkMagic);
        s.io(defPhysic);
        s.io(defMagic);
    }
};

// --------------------------------------------------------------------------- KLastPositionInfo
struct LastPositionInfo {
    int mapId = 0;
    unsigned char lastTouchLineIndex = 0;
    unsigned short lastPosValue = 0;
    bool ignoreLastTouch = false;  // SERV_BATTLE_FIELD_BOSS

    void serialize(Serializer& s) {
        s.io(mapId);
        s.io(lastTouchLineIndex);
        s.io(lastPosValue);
        s.io(ignoreLastTouch);
    }
};

// --------------------------------------------------------------------------- KDungeonClearInfo / KTCClearInfo
struct DungeonClearInfo {
    int dungeonId = 0;
    int maxScore = 0;
    char maxTotalRank = 0;
    WideString clearTime;
    bool isNew = false;

    void serialize(Serializer& s) {
        s.io(dungeonId);
        s.io(maxScore);
        s.io(maxTotalRank);
        s.io(clearTime);
        s.io(isNew);
    }
};

struct TCClearInfo {
    int tcId = 0;
    WideString clearTime;
    bool isNew = false;

    void serialize(Serializer& s) {
        s.io(tcId);
        s.io(clearTime);
        s.io(isNew);
    }
};

// --------------------------------------------------------------------------- KDungeonPlayInfo
// DECL_DATA( KDungeonPlayInfo ), SERV_LIMITED_DUNGEON_PLAY_TIMES.
struct DungeonPlayInfo {
    int dungeonId = 0;
    int playTimes = 0;
    int clearTimes = 0;
    bool isNew = false;

    void serialize(Serializer& s) {
        s.io(dungeonId);
        s.io(playTimes);
        s.io(clearTimes);
        s.io(isNew);
    }
};

// --------------------------------------------------------------------------- buffs
// DECL_DATA( KBuffBehaviorFactor / KBuffFinalizerFactor ) share a shape:
// { UINT type; std::vector<float> values; }
struct BuffBehaviorFactor {
    unsigned int type = 0;
    std::vector<float> values;

    void serialize(Serializer& s) {
        s.io(type);
        s.io(values);
    }
};

struct BuffFinalizerFactor {
    unsigned int type = 0;
    std::vector<float> values;

    void serialize(Serializer& s) {
        s.io(type);
        s.io(values);
    }
};

// DECL_DATA( KBuffIdentity ) { int templateId; UINT uniqueNum; }
struct BuffIdentity {
    int templateId = 0;
    unsigned int uniqueNum = 0;

    void serialize(Serializer& s) {
        s.io(templateId);
        s.io(uniqueNum);
    }
};

// DECL_DATA( KBuffFactor ). Note the PUT order: the identity, the accumulation pair and the factor
// id go out, but m_uidMesGameUnit and m_bIsMesGameUnitNpc are deliberately absent from the 2014
// serializer, so they must not appear here either.
struct BuffFactor {
    std::vector<BuffBehaviorFactor> behaviorFactors;
    std::vector<BuffFinalizerFactor> finalizerFactors;
    BuffIdentity identity;
    float accumulationMultiplier = 0.0f;
    unsigned char accumulationCountNow = 0;
    int factorId = 0;

    void serialize(Serializer& s) {
        s.io(behaviorFactors);
        s.io(finalizerFactors);
        s.io(identity);
        s.io(accumulationMultiplier);
        s.io(accumulationCountNow);
        s.io(factorId);
    }
};

// DECL_DATA( KBuffInfo ) { KBuffFactor factorInfo; __int64 startTime; __int64 endTime; }
struct BuffInfo {
    BuffFactor factorInfo;
    std::int64_t buffStartTime = 0;
    std::int64_t buffEndTime = 0;

    void serialize(Serializer& s) {
        s.io(factorInfo);
        s.io(buffStartTime);
        s.io(buffEndTime);
    }
};

// --------------------------------------------------------------------------- KItemInfo
// The 2013-05 item rework is enabled in this build, so the state and random socket fields exist.
struct ItemAttributeEnchantInfo {
    char attribEnchant0 = 0;
    char attribEnchant1 = 0;
    char attribEnchant2 = 0;
    void serialize(Serializer& s) {
        s.io(attribEnchant0);
        s.io(attribEnchant1);
        s.io(attribEnchant2);
    }
};

struct ItemInfo {
    enum ItemState : char {
        kIsInit = 0,
        kIsNormal = 1,
        kIsNotEvaluated = 2,
    };

    int itemId = 0;
    char usageType = 0;
    int quantity = 1;
    short endurance = 0;
    unsigned char sealData = 0;
    char enchantLevel = 0;
    ItemAttributeEnchantInfo attribEnchantInfo;
    std::vector<int> itemSocket;        // SERV_ITEM_OPTION_DATA_SIZE
    std::vector<int> randomSocket;      // SERV_NEW_ITEM_SYSTEM_2013_05
    char itemState = kIsInit;           // SERV_NEW_ITEM_SYSTEM_2013_05
    short period = 0;
    WideString expirationDate;
    byte expandedSocketNum = 0;         // SERV_BATTLE_FIELD_BOSS (on in this build)

    void serialize(Serializer& s) {
        s.io(itemId);
        s.io(usageType);
        s.io(quantity);
        s.io(endurance);
        s.io(sealData);
        s.io(enchantLevel);
        s.io(attribEnchantInfo);
        s.io(itemSocket);
        s.io(randomSocket);
        s.io(itemState);
        s.io(period);
        s.io(expirationDate);
        s.io(expandedSocketNum);
    }
};

struct InventoryItemInfo {
    UidType itemUid = 0;
    char slotCategory = 0;
    short slotId = 0;  // SERV_EXPAND_SLOT_ID_DATA_SIZE
    ItemInfo itemInfo;

    void serialize(Serializer& s) {
        s.io(itemUid);
        s.io(slotCategory);
        s.io(slotId);
        s.io(itemInfo);
    }
};

// --------------------------------------------------------------------------- skills
struct SkillData {
    short skillId = 0;
    UCHAR skillLevel = 0;
    void serialize(Serializer& s) {
        s.io(skillId);
        s.io(skillLevel);
    }
};

struct UnitSkillData {
    static constexpr int kEquippedSkillSlotCount = 4;

    SkillData equippedSkill[kEquippedSkillSlotCount];
    SkillData equippedSkillSlotB[kEquippedSkillSlotCount];
    WideString skillSlotBEndDate;
    char skillSlotBExpirationState = 0;
    std::vector<SkillData> passiveSkill;
    std::vector<SkillData> guildPassiveSkill;   // GUILD_SKILL_TEST
    std::vector<int> skillNote;                 // SERV_SKILL_NOTE
    unsigned char activeSkillPagesNumber = 1;   // SERV_SKILL_PAGE_SYSTEM
    unsigned char skillPagesAvailable = 1;

    void serialize(Serializer& s) {
        // The 2014 serializer wrote the eight slots one by one (PUT(m_aEquippedSkill[0]) ... [3]).
        // A fixed size array goes out unframed - no count - exactly like those eight PUTs.
        s.io(equippedSkill);
        s.io(equippedSkillSlotB);
        s.io(skillSlotBEndDate);
        s.io(skillSlotBExpirationState);
        s.io(passiveSkill);
        s.io(guildPassiveSkill);
        s.io(skillNote);
        s.io(activeSkillPagesNumber);
        s.io(skillPagesAvailable);
    }
};

// --------------------------------------------------------------------------- KUserGuildInfo
struct UserGuildInfo {
    int guildUid = 0;
    WideString guildName;
    unsigned char membershipGrade = 0;
    int honorPoint = 0;

    void serialize(Serializer& s) {
        s.io(guildUid);
        s.io(guildName);
        s.io(membershipGrade);
        s.io(honorPoint);
    }
};

// --------------------------------------------------------------------------- KUnitInfo
struct UnitInfo {
    UidType ownerUserUid = 0;
    char authLevel = 0;
    UidType unitUid = 0;
    unsigned int knmSerialNum = 0;
    char unitClass = 0;
    WideString nickName;

    WideString ip;
    USHORT port = 0;

    int ed = 0;
    UCHAR level = 0;
    int exp = 0;

    // SERV_PVP_NEW_SYSTEM
    int officialMatchCnt = 0;
    int rating = 0;
    int maxRating = 0;
    int rPoint = 0;
    int aPoint = 0;
    bool isWinBeforeMatch = false;
    // SERV_2012_PVP_SEASON2
    char rank = 0;
    float kFactor = 0.0f;
    bool isRedistributionUser = false;
    int pastSeasonWin = 0;

    int sPoint = 0;
    int csPoint = 0;
    int maxCsPoint = 0;
    WideString csPointEndDate;

    int nowBaseLevelExp = 0;
    int nextBaseLevelExp = 0;
    int straightVictories = 0;

    Stat stat;
    Stat gameStat;

    LastPositionInfo lastPos;  // SERV_BATTLE_FIELD_SYSTEM

    std::vector<BuffInfo> buffInfo;  // SERV_REFORM_THE_GATE_OF_DARKNESS

    int win = 0;
    int lose = 0;

    std::map<int, DungeonClearInfo> dungeonClear;
    std::map<int, TCClearInfo> tcClear;
    std::map<int, DungeonPlayInfo> dungeonPlay;  // SERV_LIMITED_DUNGEON_PLAY_TIMES
    std::map<int, InventoryItemInfo> equippedItem;

    UnitSkillData unitSkillData;

    bool isParty = false;
    int spiritMax = 0;
    int spirit = 0;
    bool isGameBang = false;
    int pcBangType = 0;  // SERV_PC_BANG_TYPE

    int titleId = 0;  // SERV_TITLE_DATA_SIZE (int, not short)
    UserGuildInfo guildInfo;  // GUILD_TEST

    WideString lastLoginTime;

    std::int64_t warpVipEndDate = 0;  // SERV_ADD_WARP_BUTTON
    int eventQuestClearCount = 0;     // SERV_GROW_UP_SOCKET
    int exchangeCount = 0;

    void serialize(Serializer& s) {
        s.io(ownerUserUid);
        s.io(authLevel);
        s.io(unitUid);
        s.io(knmSerialNum);
        s.io(unitClass);
        s.io(nickName);
        s.io(ip);
        s.io(port);
        s.io(ed);
        s.io(level);
        s.io(exp);
        s.io(officialMatchCnt);
        s.io(rating);
        s.io(maxRating);
        s.io(rPoint);
        s.io(aPoint);
        s.io(isWinBeforeMatch);
        s.io(rank);
        s.io(kFactor);
        s.io(isRedistributionUser);
        s.io(pastSeasonWin);
        s.io(sPoint);
        s.io(csPoint);
        s.io(maxCsPoint);
        s.io(csPointEndDate);
        s.io(nowBaseLevelExp);
        s.io(nextBaseLevelExp);
        s.io(straightVictories);
        s.io(stat);
        s.io(gameStat);
        s.io(lastPos);
        s.io(buffInfo);
        s.io(win);
        s.io(lose);
        s.io(dungeonClear);
        s.io(tcClear);
        s.io(dungeonPlay);
        s.io(equippedItem);
        s.io(unitSkillData);
        s.io(isParty);
        s.io(spiritMax);
        s.io(spirit);
        s.io(isGameBang);
        s.io(pcBangType);
        s.io(titleId);
        s.io(guildInfo);
        s.io(lastLoginTime);
        s.io(warpVipEndDate);
        s.io(eventQuestClearCount);
        s.io(exchangeCount);
    }
};

// --------------------------------------------------------------------------- unit list / create
// EGS_MY_UNIT_AND_INVENTORY_INFO_LIST_ACK
struct MyUnitAndInventoryInfoListAck {
    int ok = 0;
    int unitSlot = 0;
    std::vector<UnitInfo> unitInfo;
    bool sharingBank = false;  // SERV_SHARING_BANK_QUEST_CASH (on in this build)

    void serialize(Serializer& s) {
        s.io(ok);
        s.io(unitSlot);
        s.io(unitInfo);
        s.io(sharingBank);
    }
};

// EGS_CREATE_UNIT_ACK
struct CreateUnitAck {
    int ok = 0;
    UnitInfo unitInfo;
    std::vector<int> skillSlot;
    WideString enableDate;

    void serialize(Serializer& s) {
        s.io(ok);
        s.io(unitInfo);
        s.io(skillSlot);
        s.io(enableDate);
    }
};

// EGS_GET_MY_INVENTORY_ACK
struct GetMyInventoryAck {
    int ok = 0;
    bool isRecommend = false;
    bool sharingBank = false;  // SERV_SHARING_BANK_QUEST_CASH (on in this build)

    void serialize(Serializer& s) {
        s.io(ok);
        s.io(isRecommend);
        s.io(sharingBank);
    }
};

// EGS_SELECT_UNIT_INVENTORY_INFO_NOT
struct SelectUnitInventoryInfoNot {
    int slotCategory = 0;
    std::map<UidType, InventoryItemInfo> items;

    void serialize(Serializer& s) {
        s.io(slotCategory);
        s.io(items);
    }
};

// EGS_SELECT_UNIT_REQ / ACK
struct SelectUnitReq {
    UidType unitUid = 0;
    std::set<int> codeEventScriptId;  // SERV_CODE_EVENT
    // SERV_GLOBAL_EVENT_TABLE is disabled, so m_mapGlobalEventData is absent.

    void serialize(Serializer& s) {
        s.io(unitUid);
        s.io(codeEventScriptId);
    }
};

struct SelectUnitAck {
    int ok = 0;
    void serialize(Serializer& s) { s.io(ok); }
};

}  // namespace knc
