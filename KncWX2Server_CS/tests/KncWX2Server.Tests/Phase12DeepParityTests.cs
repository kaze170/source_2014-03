using KncWX2Server.CenterServer;
using KncWX2Server.Core.Packets;
using KncWX2Server.Core.Region;
using KncWX2Server.GameServer;
using Xunit;

namespace KncWX2Server.Tests;

public sealed class Phase12DeepParityTests
{
    [Fact]
    public void CenterServer_TrainingCenterRoom_TracksSuccessFailDropAndAutoClose()
    {
        var tc = new KTrainingCenterRoom { RoomUid = 500 };
        var t0 = DateTime.UtcNow;
        Assert.Equal(NetError.NET_OK, tc.OpenRoom(1001, tcId: 3, dungeonId: 30010, playTimeLimit: 180f, t0));
        Assert.False(tc.RemainingPlayTimeEnabled);
        Assert.Equal(NetError.NET_OK, tc.SetRemainingTimeEnabled(true));
        Assert.True(tc.RemainingPlayTimeEnabled);

        var (ok, result, sec) = tc.EndGame(1001, isSuccess: true, t0.AddSeconds(42));
        Assert.Equal(NetError.NET_OK, ok);
        Assert.Equal(KTrainingCenterRoom.TcGameResult.Success, result);
        Assert.Equal(42, sec);
        Assert.Equal(KTrainingCenterRoom.TcRoomState.Close, tc.State);

        var tcDrop = new KTrainingCenterRoom { RoomUid = 501 };
        tcDrop.OpenRoom(1002, 4, 30020, 180f, t0);
        var dropRes = tcDrop.LeaveRoom(1002, t0.AddSeconds(15));
        Assert.Equal((NetError.NET_OK, KTrainingCenterRoom.TcGameResult.Drop, 15), dropRes);
        Assert.Equal(KTrainingCenterRoom.TcRoomState.Close, tcDrop.State);
    }

    [Fact]
    public void GameServer_Log_Ranking_Channeling_CheckSum_And_RandomItemManager()
    {
        var log = new KUserLogManager();
        for (ushort i = 1; i <= 7; i++)
        {
            log.SaveSendPacket(i, 1000 + i);
            log.SaveProcessEvent((ushort)(i + 100), 1000 + i);
        }
        string snap = log.FormatDiagnosticSnapshot(KUserLogManager.UserLogType.HeartBeat, 10, 20);
        Assert.DoesNotContain("0x0001", snap);
        Assert.Contains("0x0007", snap);
        Assert.Contains("0x006B", snap); // 107

        var rank = new KUserRankingManager();
        rank.SetRankingInfo(new KHenirRankingInfo { UnitUid = 20, StageCount = 35, PlayTime = 420 });
        Assert.True(rank.HasPendingHenirUpdate);
        Assert.True(rank.HasPendingHeroUpdate);
        var (std, hero) = rank.GetUpdateInfo();
        Assert.Single(std);
        Assert.Single(hero);
        rank.MarkPersisted();
        Assert.False(rank.HasPendingHenirUpdate);

        var ch = new KChannelingUserManager();
        ch.UpdateChannelingUserList(true, 1);
        ch.UpdateChannelingUserList(true, 1);
        ch.UpdateChannelingUserList(false, 1, gameServerLoggedIn: true);
        Assert.Equal(1, ch.GetChannelingUserList()[1]);

        var cs = new KCheckSumManager();
        Assert.True(cs.LoadFromXmlContent("<PatchInfo><File><Item Name=\"data\\script.kom\" CheckSum=\"ABCD1234\" /></File></PatchInfo>"));
        Assert.True(cs.GetIsValidCheckSum(new Dictionary<string, string> { ["SCRIPT.KOM"] = "abcd1234" }));
        Assert.False(cs.GetIsValidCheckSum(new Dictionary<string, string> { ["SCRIPT.KOM"] = "TAMPERED" }));

        var rm = new KRandomItemManager { NotifyMsgEnabled = true };
        rm.AddAttractionItemInfo(99001, notify: true);
        rm.AddRandomItemGroup(100, 99001, 100f, period: 0, quantity: 1);
        rm.AddRandomItemGroup(200, 99002, 100f, period: 7, quantity: 3);
        rm.RegisterRandomItem(new KRandomItemManager.RandomItemData
        {
            ItemId = 160001,
            Condition = KRandomItemManager.UseCondition.OneUnit,
            UnitData = [new KRandomItemManager.RandomUnitData(1, 100)],
        });
        var t0 = DateTime.UtcNow;
        rm.AddMappingRule(new KRandomItemManager.MappingRule(160001, 0, 100, 200, t0.AddHours(1), t0.AddHours(2)));

        // Outside mapping window -> group 100
        var open1 = rm.OpenRandomItem(unitClass: 4, randomItemId: 160001, nowUtcOverride: t0, fixedRoll: 0.1);
        Assert.Equal(NetError.NET_OK, open1.ok);
        Assert.Equal(99001, open1.rewardItems[0].ItemId);
        Assert.Equal("Alice;IceWeapon", rm.MakeNotifyMsgAttractionItem(99001, "Alice", "IceWeapon"));

        // Inside mapping window -> remapped to group 200
        var open2 = rm.OpenRandomItem(unitClass: 4, randomItemId: 160001, nowUtcOverride: t0.AddMinutes(90), fixedRoll: 0.1);
        Assert.Equal(NetError.NET_OK, open2.ok);
        Assert.Equal(99002, open2.rewardItems[0].ItemId);
        Assert.Equal(3, open2.rewardItems[0].Quantity);
        Assert.Equal((short)7, open2.rewardItems[0].Period);
    }
}
