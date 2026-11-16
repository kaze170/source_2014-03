#include "kgame/ExpTable.h"

#include <algorithm>

namespace knc::game {

ExpTable::ExpTable() {
    entries_.assign(expTableData(), expTableData() + expTableSize());
}

int ExpTable::levelForExp(std::int64_t exp) const {
    if (entries_.empty()) return 1;
    if (exp <= 0) return entries_.front().level;

    // Entries are ordered by level and expToReach is monotonic; the level is the last entry whose
    // cumulative requirement is <= exp. This is exactly KExpTable::CheckLevelUp's comparison
    // (nExp < nTotalExp), just resolved with a binary search instead of a map walk.
    const auto it = std::upper_bound(entries_.begin(), entries_.end(), exp,
                                     [](std::int64_t value, const ExpEntry& entry) {
                                         return value < entry.expToReach;
                                     });
    if (it == entries_.begin()) return entries_.front().level;
    return std::prev(it)->level;
}

std::int64_t ExpTable::expToReachLevel(int level) const {
    if (entries_.empty() || level < 1) return 0;
    if (static_cast<std::size_t>(level) > entries_.size()) return entries_.back().expToReach;
    return entries_[static_cast<std::size_t>(level - 1)].expToReach;
}

std::int64_t ExpTable::expToNextLevel(int level) const {
    if (entries_.empty() || level < 1) return 0;
    const std::size_t index = static_cast<std::size_t>(level - 1);
    if (index >= entries_.size()) return 0;
    // At the top level the unit keeps collecting exp but never advances.
    if (index + 1 >= entries_.size()) return 0;
    return entries_[index].expToNext;
}

std::int64_t ExpTable::expWithinLevel(int level, std::int64_t exp) const {
    const std::int64_t base = expToReachLevel(level);
    const std::int64_t within = exp - base;
    if (within <= 0) return 0;
    const std::int64_t span = expToNextLevel(level);
    if (span > 0 && within > span) return span;
    return within;
}

ExpTable::ApplyResult ExpTable::applyExp(int level, std::int64_t exp, std::int64_t delta) const {
    ApplyResult result;
    result.level = std::max(1, level);
    result.exp = std::max<std::int64_t>(0, exp + std::max<std::int64_t>(0, delta));
    const int target = levelForExp(result.exp);
    result.levelsGained = std::max(0, target - result.level);
    result.level = target;
    result.reachedMax = !entries_.empty() && result.level >= entries_.back().level;
    return result;
}

bool ExpTable::validate() const {
    if (entries_.empty()) return false;
    std::int64_t previous = -1;
    for (std::size_t i = 0; i < entries_.size(); ++i) {
        const ExpEntry& entry = entries_[i];
        if (entry.level != static_cast<int>(i + 1)) return false;
        if (entry.expToReach < previous) return false;  // must be monotonic
        if (entry.expToNext < 0) return false;
        previous = entry.expToReach;
    }
    return entries_.front().expToReach == 0;  // level 1 costs nothing
}

const ExpTable& defaultExpTable() {
    static const ExpTable table;
    return table;
}

}  // namespace knc::game
