using KncWX2Server.CenterServer;
using KncWX2Server.Core.Database;
using KncWX2Server.Core.Framework;
using KncWX2Server.Core.Region;
using KncWX2Server.GameServer;
using Xunit;

namespace KncWX2Server.Tests;

public sealed class Phase4GameplaySystemsTests
{
    [Fact]
    public void Lottery_And_DropTable_And_Henir_WorkDeterministically()
    {
        var lot = new KLottery(seed: 123);
        Assert.True(lot.AddCase(100, 60.0, param1: 3));
        Assert.True(lot.AddCase(200, 40.0, param1: 1));
        Assert.False(lot.AddCaseIntegerCast(300, 5.0)); // >100%

        int rolled = lot.Decision(out double roulette);
        Assert.InRange(roulette, 0.0, 100.0);
        Assert.Contains(rolled, new[] { 100, 200 });

        var drops = new KDropTable();
        drops.RegisterMonsterDrop(9999, 777001, 100.0, quantity: 2);
        var (dropped, itemId, qty) = drops.RollMonsterDrop(9999);
        Assert.True(dropped);
        Assert.Equal(777001, itemId);
        Assert.Equal(2, qty);

        var henir = new KHenirResultTable();
        Assert.True(henir.IsResurrectionStage(10));
        Assert.True(henir.IsClearTempInventoryStage(20));
        Assert.NotEmpty(henir.RollStageRewards(10, 0));
    }

    [Fact]
    public void RoomMonsterManager_And_BadAttitudeManager_MatchCppRules()
    {
        var monsters = new KRoomMonsterManager();
        monsters.StartGame();
        int u1 = monsters.CreateMonster(1001, 10, KRoomMonsterManager.MonsterGrade.MG_NORMAL_NPC);
        int u2 = monsters.CreateMonster(1002, 12, KRoomMonsterManager.MonsterGrade.MG_LOW_ELITE_NPC);
        int u3 = monsters.CreateMonster(1003, 15, KRoomMonsterManager.MonsterGrade.MG_BOSS_NPC);
        Assert.Equal(3, monsters.AliveCount);

        Assert.True(monsters.TryKillMonster(u1, out _));
        Assert.True(monsters.TryKillMonster(u2, out _));
        Assert.True(monsters.TryKillMonster(u3, out _));
        // 1*1 + 1*2 + 1*12 = 15
        Assert.Equal(15, monsters.GetMonsterWeightScore());

        var bad = new KBadAttitudeManager();
        bad.Init([1, 2, 3, 4]);
        Assert.Equal((true, false), bad.VoteForcedExit(1, 4));
        Assert.Equal((true, false), bad.VoteForcedExit(2, 4));
        Assert.Equal((true, true), bad.VoteForcedExit(3, 4)); // 3 of 4 voted -> kick!

        Assert.True(bad.RemoveUnit(1, out _));
    }

    [Fact]
    public void EnchantItemManager_RespectsFluoriteAndDestroyGuard()
    {
        Assert.Equal(1.00f, KEnchantItemManager.GetEnchantStatMultiplier(0));
        Assert.Equal(2.15f, KEnchantItemManager.GetEnchantStatMultiplier(10));

        // +0 -> +1 and +1 -> +2 are 100% guaranteed
        var r1 = KEnchantItemManager.ExecuteEnchant(0, false, false, false, seed: 7);
        Assert.Equal(KEnchantItemManager.EnchantResult.Up1, r1.result);
        Assert.Equal(1, r1.nextLevel);
        Assert.False(r1.isBroken);

        // With Fluorite Ore at <= +10, item never downgrades or breaks across 50 trials
        for (int i = 0; i < 50; i++)
        {
            var r = KEnchantItemManager.ExecuteEnchant(9, isSupportMaterial: true, isEnchantPlus: false, isDestroyGuard: false, seed: i + 10);
            Assert.False(r.isBroken);
            Assert.True(r.nextLevel >= 9);
        }

        // With Destroy Guard at +11, item never breaks across 50 trials
        for (int i = 0; i < 50; i++)
        {
            var r = KEnchantItemManager.ExecuteEnchant(11, isSupportMaterial: false, isEnchantPlus: false, isDestroyGuard: true, seed: i + 100);
            Assert.False(r.isBroken);
        }
    }

    [Fact]
    public void SkillTree_TitleManager_And_PersonalShop_PersistInSqlite()
    {
        string dbPath = Path.Combine(Path.GetTempPath(), $"knc_phase4_{Guid.NewGuid():N}.db");
        try
        {
            var db = new SqliteDatabaseManager(dbPath);
            db.InitializeAndSeed();
            var region = RegionProfile.FromCode(RegionCode.TW);

            var (_, sellerAcct, _, _, _) = db.VerifyOrRegisterAccount(region.Code, "seller01", "pw");
            var (_, buyerAcct, _, _, _) = db.VerifyOrRegisterAccount(region.Code, "buyer01", "pw");
            var (_, seller, _) = db.CreateUnit(region, sellerAcct, "SellerUnit", 1);
            var (_, buyer, _) = db.CreateUnit(region, buyerAcct, "BuyerUnit", 2);

            // Skill tree learn & slot swap
            var skills = new KUserSkillTree();
            skills.Init(10);
            Assert.True(skills.TryLearnSkill(5001));
            var eq1 = skills.EquipSkillSlot(db, buyer!.UnitUid, 0, 5001);
            Assert.True(eq1.ok);
            var eq2 = skills.EquipSkillSlot(db, buyer.UnitUid, 4, 5001); // Moves from slot 0 to slot 4
            Assert.True(eq2.ok);
            Assert.Equal(0, eq2.slot2);
            Assert.Equal(5001, db.GetUnitSkillSlots(buyer.UnitUid)[4]);

            // Title mission unlock & equip
            var titles = new KUserTitleManager();
            titles.Init(0);
            Assert.True(titles.RecordMissionProgress(db, buyer.UnitUid, 2001, 5, requiredCount: 5));
            Assert.True(titles.EquipTitle(db, buyer.UnitUid, 2001));
            Assert.Equal(2001, db.GetUnitByUid(region, buyer.UnitUid)!.TitleId);

            // Personal shop open + buy (5% market fee)
            var shop = new KUserPersonalShopManager();
            long listingUid = shop.OpenShopAndRegisterItem(db, region, seller!.UnitUid, seller.NickName, 130077, 5, 2000);
            var (buyOk, buyerNewEd, granted) = shop.BuyItem(db, region, buyer.UnitUid, listingUid, 2);
            Assert.True(buyOk);
            Assert.NotNull(granted);
            Assert.Equal(100000 - 4000, buyerNewEd);
            Assert.Equal(100000 + 3800, db.GetUnitByUid(region, seller.UnitUid)!.Ed);
        }
        finally
        {
            try { if (File.Exists(dbPath)) File.Delete(dbPath); } catch { }
        }
    }
}
