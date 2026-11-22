using KncWX2Server.CenterServer;
using KncWX2Server.Core.Database;
using KncWX2Server.Core.Framework;
using KncWX2Server.Core.Packets;
using KncWX2Server.Core.Region;
using KncWX2Server.GameServer;
using KncWX2Server.GlobalServer;
using KncWX2Server.LoginServer;
using Xunit;

namespace KncWX2Server.Tests;

public sealed class Phase3DeepSubsystemsTests
{
    [Fact]
    public void SectorId_AdjacentAndRemainder_MatchesCppSemantics()
    {
        int s1 = KSectorId.SetIndex(4, 5, 6);
        Assert.Equal(4, KSectorId.ExtractX(s1));
        Assert.Equal(5, KSectorId.ExtractY(s1));
        Assert.Equal(6, KSectorId.ExtractZ(s1));

        var adj1 = KSectorId.ExtractAdjacentSector(s1, 16, 16, 16);
        Assert.Equal(27, adj1.Count);
        for (int i = 1; i < adj1.Count; i++)
            Assert.True(adj1[i - 1] < adj1[i]);

        int s2 = KSectorId.SetIndex(5, 5, 6);
        var adj2 = KSectorId.ExtractAdjacentSector(s2, 16, 16, 16);
        KSectorId.FindRemainder(adj1, adj2, out var left, out var entered);

        Assert.Equal(9, left.Count);
        Assert.Equal(9, entered.Count);
        Assert.All(left, x => Assert.Equal(3, KSectorId.ExtractX(x)));
        Assert.All(entered, x => Assert.Equal(6, KSectorId.ExtractX(x)));
    }

    [Fact]
    public void PvpMatchResultTable_Season2EloAndRPoint_MatchLuaFormulas()
    {
        double expectEqual = KPvpMatchResultTable.GetExpectOfVictory(1500, 1500);
        Assert.Equal(0.5, expectEqual, 6);

        // Placement (<10 games) streak break reduces KFactor by 0.8 -> delta is -0.2 * K
        float kDelta = KPvpMatchResultTable.GetKFactorIncrement(
            pvpPlayCount: 3, isWinBeforeMatch: true, isWin: false, userRating: 1200, currentKFactor: 300f);
        Assert.Equal(-60f, kDelta, 2);

        // Post-placement rating brackets
        Assert.Equal(12f - 32f, KPvpMatchResultTable.GetKFactorIncrement(35, true, true, 2400, 32f), 2);
        Assert.Equal(32f - 24f, KPvpMatchResultTable.GetKFactorIncrement(35, true, true, 1600, 24f), 2);

        // ArrangeTeamsMatch halves rating loss
        int normalLoss = KPvpMatchResultTable.GetRatingPvpResult(1500, 1500, 1500, isWin: false, kFactor: 32.0, arrangeTeamsMatch: false);
        int arrangeLoss = KPvpMatchResultTable.GetRatingPvpResult(1500, 1500, 1500, isWin: false, kFactor: 32.0, arrangeTeamsMatch: true);
        Assert.Equal(-16, normalLoss);
        Assert.Equal(-8, arrangeLoss);

        // Ranking Point bounds: WinLose min -6, Kill/Death clamped [-4, 12]
        int rpMax = KPvpMatchResultTable.GetRPointPvpResult(1200, 1600, isWin: true, killNum: 20, mdKillNum: 5, deathNum: 0, matchType: 5);
        Assert.True(rpMax >= 25);
        int rpMin = KPvpMatchResultTable.GetRPointPvpResult(2000, 1000, isWin: false, killNum: 0, mdKillNum: 0, deathNum: 10, matchType: 0);
        Assert.Equal(-10, rpMin); // -6 + -4
    }

    [Fact]
    public void ResultProcess_PvpAndDungeon_ProducesExpectedRanksAndRewards()
    {
        var mgr = new KCnRoomManager();
        var room = mgr.CreateRoom(new KRoomInfo
        {
            RoomType = 1,
            DungeonId = 30000,
            DifficultyLevel = 2,
            DungeonMode = 0,
            MaxSlot = 4,
        }, new KRoomUserInfo { UnitUid = 101, NickName = "Els", Level = 30 });

        room.TryAddUser(new KRoomUserInfo { UnitUid = 102, NickName = "Aisha", Level = 30 }, false, "");
        room.StartLoading(0, 30000);
        room.UpdateLoading(101, 100, out _);
        room.UpdateLoading(102, 100, out bool allLoaded);
        Assert.True(allLoaded);

        room.RecordDungeonStats(101, addCombo: 4900, addTech: 4800, addDamaged: 500, addNpcKills: 24);
        room.RecordDungeonStats(102, addCombo: 1500, addTech: 1500, addDamaged: 12000, addNpcKills: 8);

        var dgnRes = room.FinishDungeonGame(isWin: true);
        Assert.True(dgnRes.IsWin);
        Assert.Equal(2, dgnRes.DungeonUnitInfo.Count);
        var els = dgnRes.DungeonUnitInfo.First(u => u.UnitUid == 101);
        Assert.True(els.IsMvp);
        Assert.True(els.TotalRank >= (sbyte)KResultProcess.RankType.RT_A);
        Assert.True(els.SumExp() > 0 && els.Ed > 0);
    }

    [Fact]
    public void MatchMakingManager_Season2_IsolatesBeginnersAndTracksWaitTimes()
    {
        var mm = new KGbMatchMakingManager();
        Assert.Equal(KMatchCompleteTimeManager.UnknownWaitSeconds, mm.GetEstimatedWaitSeconds(1));

        // Beginner vs non-beginner of identical rating must NOT pair
        mm.Enqueue(new MatchRequestEntry(1, "Newbie", 1000, 1, DateTime.UtcNow, IsBeginner: true));
        mm.Enqueue(new MatchRequestEntry(2, "Vet", 1000, 1, DateTime.UtcNow, IsBeginner: false));
        Assert.Empty(mm.TickMatchmaking());

        // Matching beginner pairs cleanly
        mm.Enqueue(new MatchRequestEntry(3, "Newbie2", 1050, 1, DateTime.UtcNow, IsBeginner: true));
        var formed = mm.TickMatchmaking();
        Assert.Single(formed);
        Assert.NotEqual(KMatchCompleteTimeManager.UnknownWaitSeconds, mm.GetEstimatedWaitSeconds(1));
    }

    [Fact]
    public void Spirit_AntiAddiction_StatTable_And_FSMs_MatchCppRules()
    {
        var spirit = new KUserSpiritManager();
        spirit.Init(0, 100);
        Assert.False(spirit.CheckingSpirit(30000, isPcBang: false));
        Assert.True(spirit.CheckingSpirit(10001, isPcBang: false)); // Tutorial exemption
        Assert.True(spirit.CheckingSpirit(30000, isPcBang: true));  // PC Bang exemption
        Assert.Equal(50, spirit.RestoreSpirit(50));

        var fatigue = new KAntiAddictionManager();
        fatigue.Initialize(RegionProfile.FromCode(RegionCode.CN), isAdult: false, TimeSpan.FromHours(3.5), TimeSpan.FromHours(1));
        Assert.Equal(KAntiAddictionManager.RewardState.Half, fatigue.GetCurrentState());
        Assert.Equal(0.5f, fatigue.GetRewardMultiplier());

        fatigue.Initialize(RegionProfile.FromCode(RegionCode.CN), isAdult: false, TimeSpan.FromHours(4), TimeSpan.FromHours(6));
        Assert.Equal(KAntiAddictionManager.RewardState.None, fatigue.GetCurrentState()); // 5h rest resets!

        var stats = new KStatTable();
        var s = stats.GetUnitStat(1, 50);
        Assert.True(s.AtkPhysic > s.AtkMagic);
        Assert.True(s.Hp > 15000f);

        var gsFsm = new KGSFSM();
        Assert.True(gsFsm.TryStateTransition(KGSFSM.I_TO_LOGINED, out _));
        Assert.True(gsFsm.TryStateTransition(KGSFSM.I_TO_CHECK_SECOND_PW, out _));
        Assert.True(gsFsm.TryStateTransition(KGSFSM.I_TO_SERVER_SELECT, out _));
        Assert.Equal(KGSFSM.S_SERVER_SELECT, gsFsm.CurrentState);

        var partyFsm = new KCnPartyFSM();
        Assert.True(partyFsm.TryStateTransition(KCnPartyFSM.I_TO_WAIT, out _));
        Assert.True(partyFsm.TryStateTransition(KCnPartyFSM.I_TO_PREPARE_FOR_PLAY, out _));
        Assert.Equal(KCnPartyFSM.S_PREPARE_FOR_PLAY, partyFsm.CurrentState);
    }

    [Fact]
    public void Database_SecondSecurity_AccountBlock_Guild_And_Rankings_WorkEndToEnd()
    {
        string dbPath = Path.Combine(Path.GetTempPath(), $"knc_phase3_{Guid.NewGuid():N}.db");
        try
        {
            var db = new SqliteDatabaseManager(dbPath);
            db.InitializeAndSeed();
            var region = RegionProfile.FromCode(RegionCode.TW);

            var (_, userUid, _, _, _) = db.VerifyOrRegisterAccount(region.Code, "sec_tester", "pass123");
            db.CreateSecondSecurityPin(userUid, "123456");
            Assert.True(db.GetSecondSecurityStatus(userUid).useSecondPw);
            var bad = db.AuthSecondSecurityPin(userUid, "000000");
            Assert.False(bad.ok);
            Assert.Equal(1, bad.failedCount);
            var good = db.AuthSecondSecurityPin(userUid, "123456");
            Assert.True(good.ok);
            Assert.Equal(0, good.failedCount);

            var blockMgr = new KAccountBlockManager();
            blockMgr.SetBlock(db, region, "bad_user", KAccountBlockManager.BlockType.Connect, true, "Cheat");
            blockMgr.RefreshNow(db, region);
            Assert.True(blockMgr.IsBlocked("bad_user", KAccountBlockManager.BlockType.Connect, out string reason));
            Assert.Equal("Cheat", reason);

            var (_, u1, _) = db.CreateUnit(region, userUid, "GuildLeader", 1);
            var (_, u2, _) = db.CreateUnit(region, userUid, "GuildMember", 2);
            var (gok, guild, _) = db.CreateGuild(region, u1!.UnitUid, "ElriosKnights", "Welcome");
            Assert.True(gok);
            Assert.True(db.JoinGuild(region, guild!.GuildUid, u2!.UnitUid).ok);
            var (chgOk, nick) = db.ChangeGuildMemberGrade(guild.GuildUid, u2.UnitUid, 2);
            Assert.True(chgOk);
            Assert.Equal("GuildMember", nick);

            db.UpdateUnitPvpResult(u1.UnitUid, deltaRating: 120, deltaRPoint: 25, deltaAPoint: 40, deltaKFactor: -10f, addExp: 300, win: true);
            var ranks = db.GetTopPvpRankings(region, 10);
            Assert.Contains(ranks, r => r.UnitUid == u1.UnitUid && r.Rating == 1120);
        }
        finally
        {
            try { if (File.Exists(dbPath)) File.Delete(dbPath); } catch { }
        }
    }
}
