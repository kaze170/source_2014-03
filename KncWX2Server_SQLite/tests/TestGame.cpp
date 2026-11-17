// tests/TestGame.cpp - kgame rules: level curve, stat table, characters, inventory, enchant, drops.
#include <random>

#include "TestFramework.h"
#include "kdb/Repositories.h"
#include "kgame/Character.h"
#include "kgame/ExpTable.h"
#include "kgame/Rules.h"
#include "kgame/Rules.h"
#include "kgame/StatTable.h"

using namespace knc;
using namespace knc::game;

namespace {

db::UnitRecord makeRecord(int unitClass, int level) {
    db::UnitRecord record;
    record.unitUid = 100;
    record.accountUid = 1;
    record.slot = 0;
    record.nickName = "Rena";
    record.unitClass = unitClass;
    record.level = level;
    return record;
}

}  // namespace

KTEST(Game, expTableMatchesTheSpreadsheet) {
    const ExpTable& table = defaultExpTable();
    KCHECK(table.validate());
    KCHECK_EQ(table.maxLevel(), 80);

    // Values read out of ScriptData/ExpTable.xls column 1 (cumulative exp per level).
    KCHECK_EQ(table.expToReachLevel(1), std::int64_t(0));  // starting level costs nothing
    KCHECK_EQ(table.expToReachLevel(2), std::int64_t(600));
    KCHECK_EQ(table.expToReachLevel(3), std::int64_t(1300));
    KCHECK_EQ(table.expToReachLevel(4), std::int64_t(2000));
    KCHECK_EQ(table.expToReachLevel(80), std::int64_t(91056800));
    // Derived gaps: level 2 -> 3 costs 700 (1300 - 600), level 3 -> 4 costs 700.
    KCHECK_EQ(table.expToNextLevel(2), std::int64_t(700));
    KCHECK_EQ(table.expToNextLevel(3), std::int64_t(700));
    KCHECK_EQ(table.expToNextLevel(4), std::int64_t(800));
    // The last level has no "next level", so no requirement is reported there.
    KCHECK_EQ(table.expToNextLevel(80), std::int64_t(0));
    KCHECK_EQ(table.expToNextLevel(81), std::int64_t(0));
}

KTEST(Game, levelFromExpAndLevelUps) {
    const ExpTable& table = defaultExpTable();
    // Level 2 is held from 600 total exp, level 3 from 1300, level 4 from 2000.
    KCHECK_EQ(table.levelForExp(0), 1);
    KCHECK_EQ(table.levelForExp(1), 1);
    KCHECK_EQ(table.levelForExp(599), 1);
    KCHECK_EQ(table.levelForExp(600), 2);
    KCHECK_EQ(table.levelForExp(1299), 2);
    KCHECK_EQ(table.levelForExp(1300), 3);
    KCHECK_EQ(table.levelForExp(1999), 3);
    KCHECK_EQ(table.levelForExp(2000), 4);
    // The table has 80 rows, but KExpTable::CheckLevelUp clamps to GetLimitsLevel() and
    // GameSysValTable.lua sets AddLimitsMaxLevel( 65 ) - so past the cap the answer is 65, not 80.
    KCHECK_EQ(table.levelForExp(91056800), ExpRules::kMaxLevel);
    KCHECK_EQ(table.levelForExp(999999999), ExpRules::kMaxLevel);
    // The clamp is a ceiling, not a rewrite: below it the table decides.
    KCHECK_EQ(table.levelForExp(table.expToReachLevel(65) - 1), 64);
    KCHECK_EQ(table.levelForExp(table.expToReachLevel(65)), 65);
    // Callers that model a different deployment can pass its own limit.
    KCHECK_EQ(table.levelForExp(91056800, 70), 70);
    KCHECK_EQ(table.levelForExp(91056800, 80), 80);
    KCHECK_EQ(table.levelForExp(91056800, 0), 1);  // a nonsensical limit degrades to level 1

    // A single large award must cross several levels and keep the remainder.
    const auto result = table.applyExp(1, 0, 2000);
    KCHECK_EQ(result.level, 4);
    KCHECK_EQ(result.levelsGained, 3);
    KCHECK_EQ(result.exp, std::int64_t(2000));
    KCHECK(!result.reachedMax);
    KCHECK_EQ(table.expWithinLevel(3, 1300), std::int64_t(0));
    KCHECK_EQ(table.expWithinLevel(3, 2000), std::int64_t(700));  // 700 of the 700 needed
    KCHECK_EQ(table.expWithinLevel(4, 2000), std::int64_t(0));

    // At the cap the exp keeps accumulating but the level stops.
    const auto capped = table.applyExp(ExpRules::kMaxLevel, 91056800, 1000);
    KCHECK_EQ(capped.level, ExpRules::kMaxLevel);
    KCHECK_EQ(capped.levelsGained, 0);
    KCHECK(capped.reachedMax);

    // A negative delta is not allowed to remove exp; the level follows the (unchanged) exp total.
    const auto none = table.applyExp(5, 4000, -100);
    KCHECK_EQ(none.exp, std::int64_t(4000));
    KCHECK_EQ(none.level, table.levelForExp(4000));  // 4000 >= 3800, so level 6
    KCHECK_EQ(none.level, 6);
    KCHECK_EQ(none.levelsGained, 1);

    // And an in-place delta (no level change) reports no levels gained.
    const auto stay = table.applyExp(6, 4000, 100);
    KCHECK_EQ(stay.level, 6);
    KCHECK_EQ(stay.levelsGained, 0);
    KCHECK_EQ(stay.exp, std::int64_t(4100));
}

KTEST(Game, statTableLookup) {
    const StatTable& table = defaultStatTable();
    KCHECK(table.size() > 4000);
    KCHECK(table.hasClass(1));
    KCHECK(table.hasClass(121));
    KCHECK(!table.hasClass(9));  // no rows for the unused class code
    KCHECK_EQ(table.maxLevelForClass(1), 80);

    // StatTable.lua line 1: SetUnitStat( 1, 1, { AtkPhysic = 360, AtkMagic = 296, DefPhysic = 80,
    //                                            DefMagic = 66, HP = 11250 } )
    BaseStat stat{};
    KCHECK(table.getUnitStat(1, 1, &stat));
    KCHECK_EQ(stat.baseHp, 11250);
    KCHECK_EQ(stat.atkPhysic, 360);
    KCHECK_EQ(stat.atkMagic, 296);
    KCHECK_EQ(stat.defPhysic, 80);
    KCHECK_EQ(stat.defMagic, 66);

    // Level 2 HP is stored as a half (12937.5) and truncated on the way into the integer KStat.
    KCHECK(table.getUnitStat(1, 2, &stat));
    KCHECK_EQ(stat.baseHp, 12937);

    // Level 40 of the same class.
    KCHECK(table.getUnitStat(1, 40, &stat));
    KCHECK_EQ(stat.atkPhysic, 754);

    BaseStat unknown{};
    KCHECK(!table.getUnitStat(9, 1, &unknown));
    KCHECK_EQ(unknown.atkPhysic, 0);
    KCHECK(!table.getUnitStat(1, 0, &unknown));
}

KTEST(Game, characterFromRecordUsesTableAndDerivesLevel) {
    db::UnitRecord record = makeRecord(1, 1);
    record.exp = 2000;  // enough for level 4, even though the row says level 1
    const Character character = Character::fromRecord(record);
    KCHECK_EQ(character.level(), 4);
    KCHECK_EQ(character.baseStat().atkPhysic, 390);  // class 1, level 4
    KCHECK_EQ(character.totalStat().atkPhysic, 390);
    KCHECK_EQ(character.nickName(), std::string("Rena"));
    KCHECK_EQ(character.unitUid(), UidType(100));
}

KTEST(Game, characterStatsAddOverridesAndEquipment) {
    db::UnitRecord record = makeRecord(1, 4);
    record.exp = 1900;
    Character character = Character::fromRecord(record);
    character.setStatOverride(0, 500);  // +500 HP from UnitStats
    character.recomputeStats();
    KCHECK_EQ(character.totalStat().baseHp, character.baseStat().baseHp + 500);

    db::ItemRecord weapon;
    weapon.itemUid = 7;
    weapon.itemCode = 1001;
    weapon.category = static_cast<int>(ItemCategory::kEquipment);
    weapon.equipped = true;
    weapon.attributes.push_back(db::ItemAttributeRecord{0, 1, 120});  // +120 atk phys
    weapon.sockets.push_back(db::ItemSocketRecord{0, 4, 30});         // +30 def magic
    character.inventory().push_back(weapon);
    character.recomputeStats();

    KCHECK_EQ(character.totalStat().atkPhysic, character.baseStat().atkPhysic + 120);
    KCHECK_EQ(character.totalStat().defMagic, character.baseStat().defMagic + 30);
}

KTEST(Game, inventorySlotsStackingAndRemoval) {
    Character character = Character::fromRecord(makeRecord(1, 1));
    character.setInventorySize(ItemCategory::kEquipment, 2);
    character.setInventorySize(ItemCategory::kConsumable, 2);

    KCHECK(character.addItem(1001, ItemCategory::kEquipment));
    KCHECK(character.addItem(1002, ItemCategory::kEquipment));
    KCHECK(!character.addItem(1003, ItemCategory::kEquipment));  // bag has 2 slots, both used
    KCHECK_EQ(character.inventoryCount(ItemCategory::kEquipment), 2);

    // Consumables stack instead of taking a second slot.
    KCHECK(character.addItem(2001, ItemCategory::kConsumable, 3));
    KCHECK(character.addItem(2001, ItemCategory::kConsumable, 4));
    KCHECK_EQ(character.inventoryCount(ItemCategory::kConsumable), 1);
    KCHECK_EQ(character.inventory().back().quantity, 7);

    // Items added in memory have no database uid (0), so address the stack by giving it one.
    character.inventory().back().itemUid = 9001;
    KCHECK(character.removeItem(9001, 3));
    KCHECK_EQ(character.inventory().back().quantity, 4);
    KCHECK(!character.removeItem(9001, 5));  // more than the stack holds
    KCHECK_EQ(character.inventory().back().quantity, 4);
    KCHECK(character.removeItem(9001, 4));    // empties the stack, which then disappears
    KCHECK_EQ(character.inventoryCount(ItemCategory::kConsumable), 0);
    KCHECK(!character.removeItem(9001, 1));
    KCHECK(!character.addItem(0, ItemCategory::kEquipment));
    KCHECK(!character.addItem(-5, ItemCategory::kEquipment));
}

KTEST(Game, edAndNicknameRules) {
    Character character = Character::fromRecord(makeRecord(1, 1));
    KCHECK_EQ(character.ed(), std::int64_t(0));
    KCHECK(character.addEd(500));
    KCHECK_EQ(character.ed(), std::int64_t(500));
    KCHECK(!character.addEd(-600));
    KCHECK_EQ(character.ed(), std::int64_t(500));
    KCHECK(character.addEd(-500));
    KCHECK_EQ(character.ed(), std::int64_t(0));

    KCHECK(validateNickname(u"Rena") == NicknameVerdict::kOk);
    KCHECK(validateNickname(u"레나") == NicknameVerdict::kOk);      // 2 Hangul syllables
    KCHECK(validateNickname(u"R") == NicknameVerdict::kTooShort);
    KCHECK(validateNickname(u"1234567890123") == NicknameVerdict::kTooLong);
    KCHECK(validateNickname(u"1234") == NicknameVerdict::kAllDigits);
    KCHECK(validateNickname(u"Re na") == NicknameVerdict::kBadCharacter);
    KCHECK(validateNickname(u"Re\tna") == NicknameVerdict::kBadCharacter);

    KCHECK(validateLoginId("test") == NicknameVerdict::kOk);
    KCHECK(validateLoginId("abc") == NicknameVerdict::kTooShort);
    KCHECK(validateLoginId("bad id") == NicknameVerdict::kBadCharacter);
    KCHECK(validatePassword("test") == NicknameVerdict::kOk);
    KCHECK(validatePassword("pass word") == NicknameVerdict::kBadCharacter);
    KCHECK(makeGuestLoginId(42) == std::string("Guest00000042"));
}

KTEST(Game, enchantLadder) {
    KCHECK_EQ(EnchantRules::successChance(0), 100);
    KCHECK_EQ(EnchantRules::successChance(4), 100);
    KCHECK_EQ(EnchantRules::successChance(5), 90);
    KCHECK_EQ(EnchantRules::successChance(20), 0);
    KCHECK(!EnchantRules::destroysOnFailure(6));
    KCHECK(EnchantRules::destroysOnFailure(7));
    KCHECK(!EnchantRules::destroysOnFailure(20));

    std::mt19937 rng(1234);
    // +0..+4 can never fail, so no roll can produce a non-success.
    for (int i = 0; i < 50; ++i) KCHECK(rollEnchant(4, rng) == EnchantOutcome::kSuccess);
    // At +20 further enchanting is rejected outright.
    KCHECK(rollEnchant(20, rng) == EnchantOutcome::kInvalid);
    // Somewhere in the ladder a failure must be reachable.
    // From +7 up a failed attempt destroys the item, so the same roll must show both outcomes
    // across the ladder below +7 (failure) and at/above +7 (destruction).
    bool sawFailure = false;
    bool sawDestroy = false;
    for (int i = 0; i < 4000 && !(sawFailure && sawDestroy); ++i) {
        if (rollEnchant(5, rng) == EnchantOutcome::kFailed) sawFailure = true;   // 90% success
        if (rollEnchant(8, rng) == EnchantOutcome::kDestroyed) sawDestroy = true;  // 70% success
    }
    KCHECK(sawFailure);
    KCHECK(sawDestroy);
    // Between +7 and +19 no failure merely downgrades: the outcome is success or destruction.
    for (int i = 0; i < 500; ++i) {
        KCHECK(rollEnchant(12, rng) != EnchantOutcome::kFailed);
    }
}

KTEST(Game, dropsAndSockets) {
    std::vector<DropEntry> table = {
        {1001, 50, 1, 1},
        {1002, 30, 1, 2},
        {1003, 20, 1, 1},
    };
    std::mt19937 rng(99);
    int counts[4] = {0, 0, 0, 0};
    for (int i = 0; i < 20000; ++i) {
        const DropEntry* entry = rollDrop(table, rng);
        KCHECK(entry != nullptr);
        if (!entry) break;
        if (entry->itemCode >= 1001 && entry->itemCode <= 1003) ++counts[entry->itemCode - 1000];
    }
    // 50/30/20 weights out of 100 over 20k rolls: allow generous slack for the RNG.
    KCHECK(counts[1] > 9000 && counts[1] < 11000);
    KCHECK(counts[2] > 5000 && counts[2] < 7000);
    KCHECK(counts[3] > 3200 && counts[3] < 4800);
    // The weight check must add up to the total we expect.
    KCHECK_EQ(counts[1] + counts[2] + counts[3], 20000);

    KCHECK(rollDrop({}, rng) == nullptr);
    std::vector<DropEntry> zero = {{1, 0, 1, 1}};
    KCHECK(rollDrop(zero, rng) == nullptr);

    // A 100% bonus doubles the effective table, so the first entry now wins ~2/3 of the rolls.
    int first = 0;
    for (int i = 0; i < 20000; ++i) {
        const DropEntry* entry = rollDrop(table, rng, 100);
        if (entry && entry->itemCode == 1001) ++first;
    }
    // total = 100, bonus 100% -> the roll is taken over 1..200 and entry 1001 covers 1..50.
    KCHECK(first > 4000 && first < 6000);

    std::vector<SocketGroupEntry> sockets = {{1003, 5601, 8}, {1003, 40002, 5}, {1004, 1, 1}};
    int hits = 0;
    for (int i = 0; i < 2000; ++i) {
        const int code = pickSocketItem(1003, sockets, rng);
        KCHECK(code == 5601 || code == 40002);
        if (code == 5601) ++hits;
    }
    // 8 vs 5 weights -> ~61.5% for 5601.
    KCHECK(hits > 1000 && hits < 1500);
    KCHECK_EQ(pickSocketItem(4242, sockets, rng), 0);
}

KTEST(Game, expBonusAndLevelWindows) {
    KCHECK_EQ(ExpRules::applyBonus(1000, false, false), std::int64_t(1000));
    KCHECK_EQ(ExpRules::applyBonus(1000, true, false), std::int64_t(1300));
    KCHECK_EQ(ExpRules::applyBonus(1000, false, true), std::int64_t(1050));
    KCHECK_EQ(ExpRules::applyBonus(1000, true, true), std::int64_t(1350));
    // 1000 * 1.35 = 1350, but 1000 * 1.35 with an extra 0.5 = 1850.
    KCHECK_EQ(ExpRules::applyBonus(1000, true, true, 0.5), std::int64_t(1850));
    KCHECK_EQ(ExpRules::applyBonus(0, true, true), std::int64_t(0));
    KCHECK_EQ(ExpRules::applyBonus(-5, true, true), std::int64_t(0));

    KCHECK(!ChannelRules::canEnterPvp(1));
    KCHECK(!ChannelRules::canEnterPvp(2));
    KCHECK(ChannelRules::canEnterPvp(3));
    KCHECK(ChannelRules::canEnterPvp(ExpRules::kMaxLevel));
    KCHECK(!ChannelRules::canEnterPvp(ExpRules::kMaxLevel + 1));
}
