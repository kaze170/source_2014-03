#include "kgame/Character.h"

#include <algorithm>

namespace knc::game {
namespace {

// UnitStats keys, mirroring the KStat field order the 2014 GUnitStat table used.
enum StatKey : int {
    kStatHp = 0,
    kStatAtkPhysic = 1,
    kStatAtkMagic = 2,
    kStatDefPhysic = 3,
    kStatDefMagic = 4,
};

int firstFreeSlot(const std::vector<db::ItemRecord>& items, int category) {
    int slot = 0;
    bool used = true;
    while (used) {
        used = false;
        for (const auto& item : items) {
            if (item.category == category && item.slotIndex == slot) {
                used = true;
                ++slot;
                break;
            }
        }
    }
    return slot;
}

}  // namespace

Character Character::fromRecord(const db::UnitRecord& record, const StatTable& stats,
                                const ExpTable& exp) {
    Character character;
    character.unitUid_ = record.unitUid;
    character.accountUid_ = record.accountUid;
    character.slot_ = record.slot;
    character.nickName_ = record.nickName;
    character.unitClass_ = record.unitClass;
    character.level_ = record.level > 0 ? record.level : 1;
    character.exp_ = record.exp;
    character.ed_ = record.ed;
    character.sp_ = record.sp;
    character.csp_ = record.csp;
    character.maxCsp_ = record.maxCsp;
    character.cspEndDate_ = record.cspEndDate;
    character.mapId_ = record.mapId;
    character.lastFieldId_ = record.lastFieldId;
    character.lastPosX_ = record.lastPosX;
    character.lastPosY_ = record.lastPosY;
    character.lastPosZ_ = record.lastPosZ;
    character.lastTown_ = record.lastTown;

    // A stored level can lag the exp total after an exp handout crashed between the two writes;
    // the exp total wins, exactly like the original login path which re-derived the level.
    const int derived = exp.levelForExp(character.exp_);
    if (derived > character.level_) character.level_ = derived;

    character.recomputeStats(stats);
    return character;
}

bool Character::addEd(std::int64_t delta) {
    if (delta < 0 && ed_ + delta < 0) return false;
    ed_ += delta;
    return true;
}

void Character::addExp(std::int64_t delta, const ExpTable& table) {
    const ExpTable::ApplyResult result = table.applyExp(level_, exp_, delta);
    level_ = result.level;
    exp_ = result.exp;
}

void Character::setStatOverride(int key, int value) {
    switch (key) {
        case kStatHp: overrideStat_.baseHp = value; break;
        case kStatAtkPhysic: overrideStat_.atkPhysic = value; break;
        case kStatAtkMagic: overrideStat_.atkMagic = value; break;
        case kStatDefPhysic: overrideStat_.defPhysic = value; break;
        case kStatDefMagic: overrideStat_.defMagic = value; break;
        default: break;
    }
}

void Character::recomputeStats(const StatTable& stats) {
    baseStat_ = BaseStat{};
    BaseStat table{};
    if (stats.getUnitStat(unitClass_, level_, &table)) baseStat_ = table;

    totalStat_ = baseStat_;
    totalStat_.add(overrideStat_);
    totalStat_.add(equipmentStat());
}

int Character::inventoryCount(ItemCategory category) const {
    const int value = static_cast<int>(category);
    return static_cast<int>(std::count_if(items_.begin(), items_.end(), [value](const db::ItemRecord& item) {
        return item.category == value;
    }));
}

int Character::inventorySize(ItemCategory category) const {
    const int index = static_cast<int>(category);
    if (index < 0 || index > 5) return 0;
    return inventorySize_[index];
}

void Character::setInventorySize(ItemCategory category, int size) {
    const int index = static_cast<int>(category);
    if (index < 0 || index > 5) return;
    inventorySize_[index] = std::max(0, size);
}

bool Character::hasFreeSlot(ItemCategory category) const {
    return inventoryCount(category) < inventorySize(category);
}

bool Character::addItem(int itemCode, ItemCategory category, int quantity) {
    if (itemCode <= 0 || quantity <= 0) return false;
    const int value = static_cast<int>(category);

    // Stackable categories merge into an existing stack, like KItemManager did for consumables.
    if (category == ItemCategory::kConsumable || category == ItemCategory::kQuest) {
        for (auto& item : items_) {
            if (item.category == value && item.itemCode == itemCode) {
                item.quantity += quantity;
                return true;
            }
        }
    }
    if (!hasFreeSlot(category)) return false;

    db::ItemRecord item;
    item.unitUid = unitUid_;
    item.itemCode = itemCode;
    item.category = value;
    item.quantity = quantity;
    item.slotIndex = firstFreeSlot(items_, value);
    items_.push_back(std::move(item));
    return true;
}

bool Character::removeItem(std::int64_t itemUid, int quantity) {
    if (quantity <= 0) return false;
    for (auto it = items_.begin(); it != items_.end(); ++it) {
        if (it->itemUid != itemUid) continue;
        if (it->quantity < quantity) return false;
        it->quantity -= quantity;
        if (it->quantity == 0) items_.erase(it);
        return true;
    }
    return false;
}

const db::ItemRecord* Character::findItem(std::int64_t itemUid) const {
    for (const auto& item : items_) {
        if (item.itemUid == itemUid) return &item;
    }
    return nullptr;
}

int Character::equippedTitle() const {
    for (const auto& title : titles_) {
        if (title.equipped) return title.titleId;
    }
    return 0;
}

BaseStat Character::equipmentStat() const {
    BaseStat total{};
    for (const auto& item : items_) {
        if (!item.equipped) continue;
        // Item attribute slots carry flat stat bonuses: AttrType is the StatKey, AttrValue the value.
        for (const auto& attribute : item.attributes) {
            BaseStat bonus{};
            switch (attribute.type) {
                case kStatHp: bonus.baseHp = attribute.value; break;
                case kStatAtkPhysic: bonus.atkPhysic = attribute.value; break;
                case kStatAtkMagic: bonus.atkMagic = attribute.value; break;
                case kStatDefPhysic: bonus.defPhysic = attribute.value; break;
                case kStatDefMagic: bonus.defMagic = attribute.value; break;
                default: break;
            }
            total.add(bonus);
        }
        // Socket values are percentage bonuses on the same stat keys; apply them as flat adds for
        // now (the exact socket formula lived in the item template, which is data we do not ship).
        for (const auto& socket : item.sockets) {
            BaseStat bonus{};
            switch (socket.type) {
                case kStatHp: bonus.baseHp = socket.value; break;
                case kStatAtkPhysic: bonus.atkPhysic = socket.value; break;
                case kStatAtkMagic: bonus.atkMagic = socket.value; break;
                case kStatDefPhysic: bonus.defPhysic = socket.value; break;
                case kStatDefMagic: bonus.defMagic = socket.value; break;
                default: break;
            }
            total.add(bonus);
        }
    }
    return total;
}

db::UnitRecord Character::toRecord() const {
    db::UnitRecord record;
    record.unitUid = unitUid_;
    record.accountUid = accountUid_;
    record.slot = slot_;
    record.nickName = nickName_;
    record.unitClass = unitClass_;
    record.level = level_;
    record.exp = exp_;
    record.ed = ed_;
    record.sp = sp_;
    record.csp = csp_;
    record.maxCsp = maxCsp_;
    record.cspEndDate = cspEndDate_;
    record.mapId = mapId_;
    record.lastFieldId = lastFieldId_;
    record.lastPosX = lastPosX_;
    record.lastPosY = lastPosY_;
    record.lastPosZ = lastPosZ_;
    record.lastTown = lastTown_;
    return record;
}

}  // namespace knc::game
