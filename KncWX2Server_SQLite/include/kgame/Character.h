// kgame/Character.h - runtime character state built from a kdb unit row plus the game rules.
//
// The 2014 code kept this in KGSUser / KUnitActor: a KUnitInfo (wire struct) plus inventories,
// skills and quest state. kgame::Character is the pure game-side object; it never talks to a
// socket and never runs SQL. kserver wires it to a Session and to kdb.
#pragma once

#include <cstdint>
#include <string>
#include <vector>

#include "kcore/Types.h"
#include "kdb/Repositories.h"
#include "kgame/ExpTable.h"
#include "kgame/StatTable.h"

namespace knc::game {

// Item categories. The 2014 server used the same numbering in KItem::eCategory and in the
// Items.Category column; category 1 is the equipment bag that EGS_GET_MY_INVENTORY_REQ returns.
enum class ItemCategory : int {
    kEquipment = 1,
    kConsumable = 2,
    kCash = 3,
    kQuest = 4,
    kSpecial = 5,
};

// ---------------------------------------------------------------- skill / quest / title records
struct SkillRecord {
    int skillId = 0;
    int level = 0;
    int slot = 0;
};

struct QuestRecord {
    int questId = 0;
    int state = 0;   // 0 = in progress, 1 = complete, 2 = rewarded
    int progress = 0;
};

struct TitleRecord {
    int titleId = 0;
    bool equipped = false;
};

// ---------------------------------------------------------------- character
struct CharacterCreateOptions {
    int slot = 0;
    std::string nickName;
    int unitClass = 1;
};

class Character {
public:
    Character() = default;

    // Builds the character from a database row. Base stats come from the StatTable unless the row
    // already carries per-unit overrides in UnitStats (they are additive, like the original).
    static Character fromRecord(const db::UnitRecord& record, const StatTable& stats = defaultStatTable(),
                                const ExpTable& exp = defaultExpTable());

    // Identity
    UidType unitUid() const { return unitUid_; }
    std::int64_t accountUid() const { return accountUid_; }
    int slot() const { return slot_; }
    const std::string& nickName() const { return nickName_; }
    void setNickName(const std::string& value) { nickName_ = value; }
    int unitClass() const { return unitClass_; }

    // Progression
    int level() const { return level_; }
    std::int64_t exp() const { return exp_; }
    std::int64_t ed() const { return ed_; }
    void setEd(std::int64_t value) { ed_ = value < 0 ? 0 : value; }
    bool addEd(std::int64_t delta);          // false when the delta would go negative
    void addExp(std::int64_t delta, const ExpTable& table = defaultExpTable());
    int sp() const { return sp_; }
    int csp() const { return csp_; }

    // Stats
    const BaseStat& baseStat() const { return baseStat_; }
    const BaseStat& totalStat() const { return totalStat_; }
    void setStatOverride(int key, int value);  // additive per-unit bonus from UnitStats
    void recomputeStats(const StatTable& stats = defaultStatTable());

    // Inventory / skills / quests / titles
    std::vector<db::ItemRecord>& inventory() { return items_; }
    const std::vector<db::ItemRecord>& inventory() const { return items_; }
    std::vector<SkillRecord>& skills() { return skills_; }
    const std::vector<SkillRecord>& skills() const { return skills_; }
    std::vector<QuestRecord>& quests() { return quests_; }
    const std::vector<QuestRecord>& quests() const { return quests_; }
    std::vector<TitleRecord>& titles() { return titles_; }
    const std::vector<TitleRecord>& titles() const { return titles_; }

    // Inventory helpers that mirror the 2014 KItemManager rules.
    int inventoryCount(ItemCategory category) const;
    int inventorySize(ItemCategory category) const;
    void setInventorySize(ItemCategory category, int size);
    bool hasFreeSlot(ItemCategory category) const;
    // Adds an item to the first free slot of the category; returns false when the bag is full.
    bool addItem(int itemCode, ItemCategory category, int quantity = 1);
    bool removeItem(std::int64_t itemUid, int quantity = 1);
    const db::ItemRecord* findItem(std::int64_t itemUid) const;
    int equippedTitle() const;

    // Aggregate of the equipped equipment stats (KStat + KGameStat in the original).
    BaseStat equipmentStat() const;

    // Persistence projection: what kdb::UnitRepository::save wants.
    db::UnitRecord toRecord() const;

private:
    UidType unitUid_ = 0;
    std::int64_t accountUid_ = 0;
    int slot_ = 0;
    std::string nickName_;
    int unitClass_ = 1;
    int level_ = 1;
    std::int64_t exp_ = 0;
    std::int64_t ed_ = 0;
    int sp_ = 0;
    int csp_ = 0;
    int maxCsp_ = 0;
    std::string cspEndDate_;
    int mapId_ = 0;
    int lastFieldId_ = 0;
    double lastPosX_ = 0, lastPosY_ = 0, lastPosZ_ = 0;
    std::string lastTown_;

    BaseStat baseStat_{};
    BaseStat overrideStat_{};
    BaseStat totalStat_{};
    std::vector<db::ItemRecord> items_;
    std::vector<SkillRecord> skills_;
    std::vector<QuestRecord> quests_;
    std::vector<TitleRecord> titles_;
    int inventorySize_[6] = {0, 80, 60, 40, 20, 20};  // index = ItemCategory, [0] unused
};

}  // namespace knc::game
