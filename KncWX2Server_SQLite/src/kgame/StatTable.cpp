#include "kgame/StatTable.h"

#include <algorithm>

namespace knc::game {

void BaseStat::add(const BaseStat& other) {
    baseHp += other.baseHp;
    atkPhysic += other.atkPhysic;
    atkMagic += other.atkMagic;
    defPhysic += other.defPhysic;
    defMagic += other.defMagic;
}

void BaseStat::multiply(double factor) {
    baseHp = static_cast<int>(static_cast<double>(baseHp) * factor);
    atkPhysic = static_cast<int>(static_cast<double>(atkPhysic) * factor);
    atkMagic = static_cast<int>(static_cast<double>(atkMagic) * factor);
    defPhysic = static_cast<int>(static_cast<double>(defPhysic) * factor);
    defMagic = static_cast<int>(static_cast<double>(defMagic) * factor);
}

StatTable::StatTable() {
    entries_.assign(statTableData(), statTableData() + statTableSize());
}

StatTable::StatTable(std::vector<StatEntry> entries) : entries_(std::move(entries)) {
    std::sort(entries_.begin(), entries_.end(), [](const StatEntry& a, const StatEntry& b) {
        return a.unitClass != b.unitClass ? a.unitClass < b.unitClass : a.level < b.level;
    });
}

bool StatTable::hasClass(int unitClass) const {
    return std::any_of(entries_.begin(), entries_.end(),
                       [unitClass](const StatEntry& entry) { return entry.unitClass == unitClass; });
}

int StatTable::maxLevelForClass(int unitClass) const {
    int level = 0;
    for (const auto& entry : entries_) {
        if (entry.unitClass == unitClass) level = std::max(level, entry.level);
    }
    return level;
}

bool StatTable::getUnitStat(int unitClass, int level, BaseStat* out) const {
    if (out) *out = BaseStat{};
    if (!out || level < 1) return false;

    const auto first = std::lower_bound(entries_.begin(), entries_.end(), unitClass,
                                        [](const StatEntry& entry, int value) {
                                            return entry.unitClass < value;
                                        });
    if (first == entries_.end() || first->unitClass != unitClass) return false;

    // Walk forward while the class matches to clamp the level, then pick the requested row.
    const StatEntry* best = nullptr;
    for (auto it = first; it != entries_.end() && it->unitClass == unitClass; ++it) {
        best = &*it;  // last touched is the highest level for this class
        if (it->level == level) {
            best = &*it;
            break;
        }
        if (it->level > level) {
            // No exact row for this level; fall back to the closest lower level (never the higher
            // one, so a character cannot borrow stats it has not earned).
            best = (it == first) ? nullptr : &*(it - 1);
            break;
        }
    }
    if (!best) return false;

    out->baseHp = static_cast<int>(best->hp);
    out->atkPhysic = static_cast<int>(best->atkPhysic);
    out->atkMagic = static_cast<int>(best->atkMagic);
    out->defPhysic = static_cast<int>(best->defPhysic);
    out->defMagic = static_cast<int>(best->defMagic);
    return true;
}

const StatTable& defaultStatTable() {
    static const StatTable table;
    return table;
}

}  // namespace knc::game
