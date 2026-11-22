using KncWX2Server.CenterServer;
using KncWX2Server.Core.Database;
using KncWX2Server.Core.Events;
using KncWX2Server.Core.Framework;
using KncWX2Server.Core.Packets;
using KncWX2Server.Core.Region;
using KncWX2Server.GameServer;
using KncWX2Server.GlobalServer;
using KncWX2Server.LoginServer;
using Xunit;

namespace KncWX2Server.Tests;

public sealed class Phase6DeepSubsystemAndIntegrationTests
{
    [Fact]
    public void GameSysVal_RewardTable_SpiritTable_And_ComeBackUser_MatchCppRules()
    {
        var sysVal = new KGameSysValTable();
        Assert.Equal(KGameSysValTable.ComeBackStep.CbsNone, KGameSysValTable.EvaluateComeBackStep(10));
        Assert.Equal(KGameSysValTable.ComeBackStep.CbsOne, KGameSysValTable.EvaluateComeBackStep(15));
        Assert.Equal(KGameSysValTable.ComeBackStep.CbsTwo, KGameSysValTable.EvaluateComeBackStep(35));
        Assert.Equal(KGameSysValTable.ComeBackStep.CbsThree, KGameSysValTable.EvaluateComeBackStep(90));
        Assert.True(sysVal.IsHackUserBlockTrade);

        var spirit = new KSpiritTable();
        Assert.Equal(50, spirit.GetSpiritOrDefault(1));
        Assert.Equal(100, spirit.GetSpiritOrDefault(99));

        var comeBack = new KComeBackUserManager();
        var now = new DateTime(2026, 11, 20, 12, 0, 0, DateTimeKind.Utc);
        comeBack.EvaluateFromLastLogin(now.AddDays(-32), now);
        Assert.True(comeBack.IsComeBackUser);
        Assert.Equal(KGameSysValTable.ComeBackStep.CbsTwo, comeBack.ComeBackRewardStep);
        Assert.True(comeBack.TryGiveComeBackRewardCube(out int cubeId, out var buffEnd, out int periodDays, now));
        Assert.True(cubeId > 0);
        Assert.Equal(30, periodDays);
        Assert.False(comeBack.CheckComeBackBuffEnd(now.AddDays(5)));
        Assert.True(comeBack.CheckComeBackBuffEnd(buffEnd.AddMinutes(1)));
    }

    [Fact]
    public void UserAbuserManager_Flags_Ed_Item_Traffic_And_GmExemption()
    {
        string dbPath = Path.Combine(Path.GetTempPath(), $"abuser_{Guid.NewGuid():N}.db");
        try
        {
            var db = new SqliteDatabaseManager(dbPath);
            db.InitializeAndSeed();
            var sysVal = new KGameSysValTable();
            var abuser = new KUserAbuserManager();

            // GM (authLevel > 2) is exempt
            Assert.False(abuser.CheckEdDeltaAndFlag(db, sysVal, 1, 10, authLevel: 5, 0, 100_000_000));
            Assert.False(abuser.IsEdAbuser);

            // Normal user exceeding threshold gets flagged + trade blocked
            Assert.True(abuser.CheckEdDeltaAndFlag(db, sysVal, 2, 20, authLevel: 0, 0, 60_000_000));
            Assert.True(abuser.IsEdAbuser);
            Assert.True(abuser.IsTradeBlockUnit);

            Assert.True(abuser.IncreaseGetItemCountAndCheck(db, 2, 20, authLevel: 0, 250));
            Assert.True(abuser.IsItemAbuser);
            Assert.True(abuser.IsMonitoringAbuser);

            Assert.Equal(500, Enumerable.Range(0, 500).Select(_ => abuser.RecordTrafficEvent((ushort)EventId.EGS_STATE_CHANGE_FIELD_REQ)).Last());
            Assert.True(abuser.IsTrafficAbuser);

            abuser.IncreaseHackingCheckCount(1, out bool b1);
            abuser.IncreaseHackingCheckCount(1, out bool b2);
            abuser.IncreaseHackingCheckCount(1, out bool b3);
            Assert.False(b1);
            Assert.False(b2);
            Assert.True(b3);
        }
        finally
        {
            if (File.Exists(dbPath)) File.Delete(dbPath);
        }
    }

    [Fact]
    public void UserQuestManager_MultiGoal_SubQuestTypes_And_Dungeon_Skill_Cash_DeepValidation()
    {
        // 1. Multi-goal Quest (NpcKill + DungeonClear + EnchantItem)
        var qm = new KUserQuestManager();
        qm.RegisterTemplate(new KUserQuestManager.QuestTemplate
        {
            QuestId = 9001,
            FairLevel = 5,
            RewardEd = 5000,
            RewardExp = 2000,
            SubQuestGoals =
            [
                new KUserQuestManager.SubQuestGoal { SubQuestId = 1, Type = KUserQuestManager.SubQuestType.SqtNpcHunt, TargetId = 101, TargetCount = 2 },
                new KUserQuestManager.SubQuestGoal { SubQuestId = 2, Type = KUserQuestManager.SubQuestType.SqtDungeonClearCount, TargetId = 30000, TargetCount = 1, TargetDifficulty = 1 },
                new KUserQuestManager.SubQuestGoal { SubQuestId = 3, Type = KUserQuestManager.SubQuestType.SqtItemEnchant, TargetId = 0, TargetCount = 1 },
            ],
        });

        Assert.False(qm.AcceptQuest(100, 9001, unitLevel: 3).ok);
        Assert.True(qm.AcceptQuest(100, 9001, unitLevel: 10).ok);

        qm.OnNpcUnitDie(30000, 101);
        Assert.False(qm.CompleteQuest(9001).ok);
        qm.OnNpcUnitDie(30000, 101);
        qm.OnDungeonClear(30000, difficulty: 1, totalRank: 4, playTimeSec: 120, takenDamage: 10);
        qm.OnEnchantItem(12345);
        var (completed, ed, exp) = qm.CompleteQuest(9001);
        Assert.True(completed);
        Assert.Equal(5000, ed);
        Assert.Equal(2000, exp);

        // 2. SkillTree prereq + seal + reset
        string dbPath = Path.Combine(Path.GetTempPath(), $"skill_{Guid.NewGuid():N}.db");
        try
        {
            var db = new SqliteDatabaseManager(dbPath);
            db.InitializeAndSeed();
            var st = new KUserSkillTree();
            st.Init(30);
            st.SetAvailableSkillPoints(1, 20);
            st.RegisterSkillTemplet(new KUserSkillTree.SkillTempletInfo { SkillId = 1001, MaxLevel = 5, RequiredLevel = 1 });
            st.RegisterSkillTemplet(new KUserSkillTree.SkillTempletInfo { SkillId = 1002, MaxLevel = 5, RequiredLevel = 15, BeforeSkillId = 1001, IsSealed = true });

            Assert.False(st.TryLearnSkill(1002, unitLevel: 30)); // locked & prereq unmet
            Assert.True(st.TryLearnSkill(1001, unitLevel: 30));
            Assert.True(st.TryLearnSkill(1001, unitLevel: 30));
            st.UnlockSealedSkill(1002);
            Assert.True(st.TryLearnSkill(1002, unitLevel: 30));
            int refunded = st.ResetSingleSkill(1002);
            Assert.Equal(1, refunded);
            Assert.Equal(0, st.GetSkillLevel(1002));
        }
        finally
        {
            if (File.Exists(dbPath)) File.Delete(dbPath);
        }

        // 3. Dungeon entry validation + Cash purchase limit
        var dm = new KUserDungeonManager();
        dm.RegisterDungeonTemplate(new KUserDungeonManager.DungeonTemplate
        {
            DungeonId = 30100,
            MinLevel = 15,
            MaxLevel = 30,
            RequiredSpirit = 10,
            PrerequisiteDungeonId = 30000,
        });
        Assert.Equal(NetError.ERR_ROOM_26, dm.CanEnterDungeon(30100, 10, 100).netError);
        Assert.Equal(NetError.ERR_ROOM_26, dm.CanEnterDungeon(30100, 20, 100).netError); // pre-dungeon not cleared
        dm.RecordDungeonClear(30000);
        Assert.Equal(NetError.NET_OK, dm.CanEnterDungeon(30100, 20, 100).netError);

        var cash = new KUserCashInventory();
        cash.RegisterProductRule(new KUserCashInventory.CashProductRule
        {
            ProductNo = 1001,
            MinLevel = 10,
            MaxPurchasePerAccount = 2,
            IsGiftable = false,
        });
        Assert.False(cash.ValidatePurchase(1001, 1, unitLevel: 5, unitType: 1, isGift: false).ok);
        Assert.False(cash.ValidatePurchase(1001, 1, unitLevel: 20, unitType: 1, isGift: true).ok);
        Assert.True(cash.ValidatePurchase(1001, 2, unitLevel: 20, unitType: 1, isGift: false).ok);
        cash.RecordPurchase(1001, 2);
        Assert.False(cash.ValidatePurchase(1001, 1, unitLevel: 20, unitType: 1, isGift: false).ok);
    }

    [Fact]
    public void CenterServer_DungeonSecurity_And_MvpRule_RequireThreePlayers()
    {
        var sec = new KDungeonSecurityManager();
        sec.SetMinClearSeconds(30000, 15);
        var start = new DateTime(2026, 11, 20, 10, 0, 0, DateTimeKind.Utc);
        sec.StartGame(501, start);
        Assert.False(sec.ValidateClearTiming(501, 30000, 777, out int elapsed, start.AddSeconds(5)));
        Assert.Equal(5, elapsed);
        Assert.False(sec.ValidateDamage(501, 30000, 777, 99_999_999));
        Assert.Equal(2, sec.AnomalyCount);

        var roomInfo = new KRoomInfo { RoomUid = 501, RoomType = 1, DungeonId = 30000, DifficultyLevel = 1, MaxSlot = 4 };
        var cnRoom = new KCnRoom(501, 1, roomInfo, "127.0.0.1", 9301);
        cnRoom.TryAddUser(new KRoomUserInfo { UnitUid = 10, NickName = "P1", Level = 30 }, false, string.Empty);
        cnRoom.TryAddUser(new KRoomUserInfo { UnitUid = 20, NickName = "P2", Level = 30 }, false, string.Empty);
        cnRoom.StartGame();
        cnRoom.RecordDungeonStats(10, addCombo: 1000, addTech: 200, addDamaged: 50, addNpcKills: 5);
        cnRoom.RecordDungeonStats(20, addCombo: 500, addTech: 100, addDamaged: 50, addNpcKills: 3);

        var res2 = KResultProcess.BuildDungeonResult(cnRoom, 60, isWin: true);
        Assert.All(res2.DungeonUnitInfo, u => Assert.Equal(0, u.MvpBonusExp)); // C++ ResultProcess.cpp L1424 requires >= 3 players

        cnRoom.TryAddUser(new KRoomUserInfo { UnitUid = 30, NickName = "P3", Level = 30 }, false, string.Empty);
        cnRoom.RecordDungeonStats(30, addCombo: 5000, addTech: 1000, addDamaged: 10, addNpcKills: 20);
        var res3 = KResultProcess.BuildDungeonResult(cnRoom, 60, isWin: true);
        Assert.True(res3.DungeonUnitInfo.First(u => u.UnitUid == 30).MvpBonusExp > 0);
        Assert.Equal(0, res3.DungeonUnitInfo.First(u => u.UnitUid == 10).MvpBonusExp);
    }

    [Fact]
    public void GlobalServer_WorldMission_Schedule_Buffs_And_CrossServerRouter()
    {
        var router = new InProcessRouter();
        string dbPath = Path.Combine(Path.GetTempPath(), $"global_int_{Guid.NewGuid():N}.db");
        try
        {
            using var global = new KGlobalServer(RegionCode.TW, router);
            using var game = new KGameServer(RegionCode.TW, dbPath, router, globalServer: global);
            Assert.True(global.Start(overridePort: 0));
            Assert.True(game.Start(overridePort: 0, overrideTrPort: 0, overrideUdpEchoPort: 0, overrideUdpRelayPort: 0));

            var t0 = new DateTime(2026, 11, 21, 20, 5, 0, DateTimeKind.Utc); // Saturday 20:05
            global.WorldMissionManager.Tick(t0, totalCcu: 500);
            Assert.True(global.WorldMissionManager.IsActive);
            Assert.Equal(KWorldMissionManager.BtiBuff2013DefenseFakeBuff, global.WorldMissionManager.WorldBuffId);

            // Route crystal increment from GameServer -> GlobalServer
            router.Send(PerformerId.PI_GS_SERVER, game.Uid, PerformerId.PI_GB_SERVER, global.Uid,
                EventId.EGB_INCREASE_THE_PROTECTED_CRYSTAL_COUNT_NOT,
                new KEGB_INCREASE_THE_PROTECTED_CRYSTAL_COUNT_NOT { DeltaCrystalCount = 12 });
            global.Tick();
            Assert.Equal(12, global.WorldMissionManager.ProtectedCrystalCount);
            Assert.Equal(3, global.WorldMissionManager.GetDefenseRewardBuffLevel());

            // Query world mission snapshot from GameServer -> GlobalServer -> GameServer
            router.Send(PerformerId.PI_GS_SERVER, game.Uid, PerformerId.PI_GB_SERVER, global.Uid,
                EventId.EGB_GET_MISSION_INFO_REQ,
                new KEGB_GET_MISSION_INFO_REQ { RequesterServerUid = game.Uid });
            global.Tick();
            game.Tick();

            Assert.NotNull(game.LatestWorldMissionSnapshot);
            Assert.Equal(12, game.LatestWorldMissionSnapshot!.ProtectedCrystalCount);

            global.WorldMissionManager.CompleteMission(true, 15, t0.AddMinutes(25));
            Assert.Equal(KWorldMissionManager.BtiBuff2013DefenseBuff, global.WorldMissionManager.WorldBuffId);
            Assert.Equal(4, global.WorldMissionManager.IndividualBuffLevel);
        }
        finally
        {
            if (File.Exists(dbPath)) File.Delete(dbPath);
        }
    }

    [Fact]
    public void LoginServer_LocalRanking_GuildProgression_And_CrossServerGuildCreate()
    {
        var router = new InProcessRouter();
        string dbPath = Path.Combine(Path.GetTempPath(), $"login_int_{Guid.NewGuid():N}.db");
        try
        {
            using var login = new KLoginServer(RegionCode.TW, dbPath, router);
            Assert.True(login.Start(overridePort: 0));

            // Local ranking multi-tab sorting & weekly reset
            var u1 = new KLocalRankingUserInfo { UserUid = 1, Category = 1, Gender = 1, Birth = 20000101 };
            var n1 = new KLocalRankingUnitInfo { UnitUid = 101, NickName = "Els", Level = 70 };
            n1.Point[0] = 300;
            n1.Point[1] = 100;

            var u2 = new KLocalRankingUserInfo { UserUid = 2, Category = 1, Gender = 2, Birth = 19980505 };
            var n2 = new KLocalRankingUnitInfo { UnitUid = 102, NickName = "Aisha", Level = 70 };
            n2.Point[0] = 500;
            n2.Point[1] = 50;

            login.LocalRankingManager.SomethingChanged(login.LocalRankingManager.SystemUid, u1, n1);
            login.LocalRankingManager.SomethingChanged(login.LocalRankingManager.SystemUid, u2, n2);

            var (spiritList, _, _, _) = login.LocalRankingManager.GetRankerList(0, 1, 0, 1, 101);
            Assert.Equal(102, spiritList[0].UnitInfo.UnitUid); // Spirit: 500 > 300

            var (apList, _, _, _) = login.LocalRankingManager.GetRankerList(1, 1, 0, 1, 101);
            Assert.Equal(101, apList[0].UnitInfo.UnitUid); // AP: 100 > 50

            Assert.True(login.LocalRankingManager.TickWeeklyReset(login.LocalRankingManager.NextResetUtc.AddMinutes(1)));
            Assert.Equal(2, login.LocalRankingManager.LastWeekBackupUnits.Count);

            // Guild level curve & permissions
            var reg = RegionProfile.FromCode(RegionCode.TW);
            var (_, userUid, _, _, _) = login.Db.VerifyOrRegisterAccount("TW", "gmaster", "pass");
            var (cuOk, masterUnit, _) = login.Db.CreateUnit(reg, userUid, "GuildMaster", 1);
            Assert.True(cuOk && masterUnit != null);

            var (gOk, guild, _) = login.GuildManager.CreateGuild(login.Db, reg, masterUnit!.UnitUid, "ElriosKnights", "Welcome");
            Assert.True(gOk && guild != null);
            var (_, newLvl, spGained) = login.GuildManager.AddGuildExp(login.Db, reg, guild!.GuildUid, 3500);
            Assert.Equal(4, newLvl);
            Assert.Equal(3, spGained);
            Assert.True(KLoginGuildManager.CanKickMember(KLoginGuildManager.GradeMaster, KLoginGuildManager.GradeNormal));
            Assert.False(KLoginGuildManager.CanKickMember(KLoginGuildManager.GradeNormal, KLoginGuildManager.GradeAdmin));
        }
        finally
        {
            if (File.Exists(dbPath)) File.Delete(dbPath);
        }
    }
}
