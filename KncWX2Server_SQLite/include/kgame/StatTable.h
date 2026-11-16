// kgame/StatTable.h - per class / per level base stats (KStatTable in the 2014 GameServer).
//
// The original loaded ServerResource/StatTable.lua and exposed
//     KStatTable::GetUnitStat( class, level ) -> KStat
// which the character code used as the base for KUnitInfo::m_kStat before items, buffs and titles
// were folded in. tools/gen_stat_table.py compiles those rows into src/kgame/StatTableData.gen.cpp.
#pragma once

#include <cstddef>
#include <vector>

#include "kcore/Types.h"

namespace knc::game {

// One SetUnitStat row. Values are doubles because the source table stores halves (12937.5 HP).
struct StatEntry {
    int unitClass = 0;
    int level = 0;
    double hp = 0.0;
    double atkPhysic = 0.0;
    double atkMagic = 0.0;
    double defPhysic = 0.0;
    double defMagic = 0.0;
};

// Mirrors KStat in Common/CommonPacket.h: the five integers that travel in KUnitInfo.
struct BaseStat {
    int baseHp = 0;
    int atkPhysic = 0;
    int atkMagic = 0;
    int defPhysic = 0;
    int defMagic = 0;

    void add(const BaseStat& other);
    void multiply(double factor);
};

const StatEntry* statTableData();
std::size_t statTableSize();
const int* statTableClasses();
std::size_t statTableClassCount();
int statTableMaxLevel();

class StatTable {
public:
    // Default table over the generated data.
    StatTable();
    // Explicit table, used by the tests.
    explicit StatTable(std::vector<StatEntry> entries);

    // Base stats for a class/level. Unknown class or level returns false and leaves `out` zeroed;
    // a level above the table clamps to the highest level of that class (the original did the same
    // so a level 99 character still got stats from a level 80 table).
    bool getUnitStat(int unitClass, int level, BaseStat* out) const;
    bool hasClass(int unitClass) const;
    int maxLevelForClass(int unitClass) const;
    std::size_t size() const { return entries_.size(); }

private:
    std::vector<StatEntry> entries_;  // sorted by (class, level)
};

const StatTable& defaultStatTable();

}  // namespace knc::game
