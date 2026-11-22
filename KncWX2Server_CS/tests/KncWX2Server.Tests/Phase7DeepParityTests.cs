using KncWX2Server.CenterServer;
using KncWX2Server.ChannelServer.Managers;
using KncWX2Server.Core.Events;
using KncWX2Server.Core.Framework;
using KncWX2Server.Core.Packets;
using KncWX2Server.GameServer;
using Xunit;

namespace KncWX2Server.Tests;

public sealed class Phase7DeepParityTests
{
    [Fact]
    public void GameServer_SecurityManager_ValidatesMachineIdChecksum_SnMonitor_And_DelayedKick()
    {
        var sec = new KUserSecurityManager();
        byte[] raw = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14];
        string validB64 = KUserSecurityManager.EncodeMachineIdWithChecksum(raw);
        Assert.True(sec.CheckValidMachineId(validB64));

        byte[] corrupted = Convert.FromBase64String(validB64);
        corrupted[0] ^= 0xFF;
        Assert.False(sec.CheckValidMachineId(Convert.ToBase64String(corrupted)));
        Assert.False(sec.CheckValidMachineId("short"));

        var t0 = new DateTime(2026, 11, 20, 10, 0, 0, DateTimeKind.Utc);
        sec.MonitoringFlag = KUserSecurityManager.ServerSnMonitoring.SsmMonitoring;
        Assert.False(sec.CheckServerSnMonitoring(t0.AddSeconds(10)));
        Assert.True(sec.CheckServerSnMonitoring(t0.AddSeconds(31)));
        Assert.False(sec.CheckServerSnMonitoring(t0.AddSeconds(40)));

        sec.SetKickRandomTime(5, 1, t0);
        sec.SetKickState(KUserSecurityManager.KickType.KtAfterRandomTime);
        Assert.False(sec.CheckIsKickUser(t0.AddSeconds(3)));
        Assert.True(sec.CheckIsKickUser(t0.AddSeconds(6)));
    }

    [Fact]
    public void GameServer_HackingManager_MonitorsLetterTrade_DeserializeFail_And_ChallengeTimeout()
    {
        var sysVal = new KGameSysValTable { LimitsCountOfLetter = 3, LimitsCountOfTrade = 2, DeserializeFailCheckCount = 2 };
        var hm = new KUserHackingManager();

        Assert.False(hm.AddLetterMonitoringList(101, sysVal));
        Assert.False(hm.AddLetterMonitoringList(102, sysVal));
        Assert.True(hm.AddLetterMonitoringList(103, sysVal));

        Assert.False(hm.AddTradeMonitoringList(201, sysVal));
        Assert.True(hm.AddTradeMonitoringList(202, sysVal));

        hm.IncreaseDeserializeFailCount((ushort)EventId.EGS_CHAT_REQ); // ignored by C++ switch
        hm.IncreaseDeserializeFailCount((ushort)EventId.EGS_DUNGEON_PLAY_INFO_TO_SERVER_NOT);
        hm.IncreaseDeserializeFailCount((ushort)EventId.EGS_DUNGEON_PLAY_INFO_TO_SERVER_NOT);
        hm.IncreaseDeserializeFailCount((ushort)EventId.EGS_DUNGEON_PLAY_INFO_TO_SERVER_NOT);
        Assert.True(hm.HackingCheckDeserializeFailCount(sysVal));
        Assert.False(hm.HackingCheckDeserializeFailCount(sysVal)); // only triggers once per C++ m_bCheckedDeserializeFail

        var t0 = new DateTime(2026, 11, 20, 12, 0, 0, DateTimeKind.Utc);
        hm.SetHackingUserSendType(2, t0);
        Assert.True(hm.CheckHackingUserReceivedPacket(out _, t0.AddSeconds(5)));
        Assert.False(hm.CheckHackingUserReceivedPacket(out sbyte missed, t0.AddSeconds(12)));
        Assert.Equal(2, missed);

        hm.SetHackingUserSendType(3, t0);
        Assert.True(hm.SetHackingUserReceiveType(3, out _, t0.AddSeconds(2)));
    }

    [Fact]
    public void GameServer_AccountQuest_Recommend_TrainingCenter_Content_And_TripleAttrib()
    {
        var aq = new KUserAccountQuestManager();
        Assert.True(aq.AddAccountQuest(new KQuestInstance { Id = 80001 }));
        Assert.False(aq.AddAccountQuest(new KQuestInstance { Id = 80001 }));
        Assert.True(aq.CompleteAccountQuest(80001));
        Assert.True(aq.IsCompletedAccountQuest(80001));
        Assert.True(KUserAccountQuestManager.IsSuitableLevelUser(11, new Dictionary<long, bool> { [11] = true }));

        var recTable = new KRecommendUserTable();
        Assert.Equal(10020, recTable.GetRecommendUserReward(20));

        var recMgr = new KUserRecommendManager();
        var list = Enumerable.Range(1, 10).Select(i => new KRecommendUserInfo { UnitUid = i, NickName = $"U{i}", Level = 10 }).ToList();
        recMgr.SetRecommendUserList(list);
        var (vp, tp, page2) = recMgr.GetRecommendUserList(2);
        Assert.Equal(2u, vp);
        Assert.Equal(2u, tp);
        Assert.Equal(2, page2.Count);
        Assert.True(recMgr.UpdateRecommendStatus(10, isOnline: true, newLevel: 25));

        var tc = new KTrainingCenterTable();
        var cleared = new Dictionary<int, KTCClearInfo>();
        Assert.True(tc.CheckIfEnter(cleared, 1, unitLevel: 1));
        Assert.False(tc.CheckIfEnter(cleared, 2, unitLevel: 5)); // BeforeId 1 not cleared
        cleared[1] = new KTCClearInfo { TcId = 1 };
        Assert.True(tc.CheckIfEnter(cleared, 2, unitLevel: 5));
        Assert.True(tc.IsTrainingCenterDungeon(40002));

        var content = new KGSContentManager();
        content.Init(enableCashShop: false, releaseTick: 42);
        Assert.False(content.IsEnableCashShop);
        Assert.Equal(42, content.ReleaseTick);

        var sock = new KSocketItemManager();
        sbyte triple = sock.RollOrResolveAttribute(
            KSocketItemManager.AtiUnknown,
            (sbyte)KSocketItemManager.EnchantElement.Wind,
            fixedUnitRoll: 0.1,
            currentSlot1Attrib: (sbyte)KSocketItemManager.EnchantElement.Blaze);
        Assert.Equal((sbyte)KSocketItemManager.EnchantElement.Wind, triple);
        Assert.Equal(128, KSocketItemManager.GetRequiredShardCount(
            KSocketItemManager.AttribEnchantType.AetTripleWeapon, 40, KSocketItemManager.ItemGrade.IgUnique));
    }

    [Fact]
    public void CenterServer_BattleFieldDanger_DungeonEventNpc_And_ChannelHackingInterval()
    {
        var bf = new KBattleFieldGameManager
        {
            EliteMonsterDropValue = 100,
            DangerousValueWarning = 200,
            DangerousValueMax = 1000,
            BossCheckUserCount = 1,
        };
        // Raise danger to >= 200 with deterministic low rolls to trigger warning, elite, middle-boss, boss
        for (int i = 0; i < 20; i++)
            bf.OnNpcUnitDie(playerCount: 2, isAttribNpc: false, difficultyLevel: 0, monsterGrade: 0, fixedEliteRoll: 0.01, fixedBossRoll: 0.01);

        Assert.True(bf.CheckAndDeleteReservedDangerousEvent(KBattleFieldGameManager.DangerousEvent.DeWarningMessage));
        Assert.True(bf.CheckAndDeleteReservedDangerousEvent(KBattleFieldGameManager.DangerousEvent.DeEliteMonsterDrop));
        Assert.True(bf.CheckAndDeleteReservedDangerousEvent(KBattleFieldGameManager.DangerousEvent.DeBossMonsterDrop));

        var de = new KDungeonEventManager();
        var now = new DateTime(2026, 11, 20, 15, 0, 0, DateTimeKind.Utc);
        de.UpdateTimeDropEventMonsterInfo([new KDungeonEventManager.TimeDropEventMonsterInfo(77, 99001, now.AddMinutes(-1), Level: 55)]);

        var stageNot = new KEGS_DUNGEON_STAGE_LOAD_NOT { StageId = 1 };
        stageNot.NpcData[0] = new KNPCList();
        Assert.True(de.CheckTimeDropEventMonster(stageNot, out int evUid, out int npcId, now));
        Assert.Equal(77, evUid);
        Assert.Equal(99001, npcId);
        Assert.Single(stageNot.NpcData[0].NpcList);

        var chHack = new KChannelUserHackingManager();
        Assert.InRange(chHack.HackingToolCheckTimeSeconds, KChannelUserHackingManager.HeMinTimeSeconds, KChannelUserHackingManager.HeMaxTimeSeconds);
    }
}
