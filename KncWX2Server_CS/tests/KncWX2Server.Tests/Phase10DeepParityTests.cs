using KncWX2Server.CenterServer;
using KncWX2Server.Core.Packets;
using KncWX2Server.Core.Region;
using KncWX2Server.GameServer;
using Xunit;

namespace KncWX2Server.Tests;

public sealed class Phase10DeepParityTests
{
    [Fact]
    public void CenterServer_WeddingHall_And_Square_EnforceLifecycleCapacityAndHandshakes()
    {
        var hall = new KCnWeddingHall { RoomUid = 100 };
        var t0 = DateTime.UtcNow;
        var groomSlot = new KRoomSlotInfo { Index = 0, RoomUserInfo = new KRoomUserInfo { UnitUid = 1001, NickName = "Groom" } };
        var brideSlot = new KRoomSlotInfo { Index = 1, RoomUserInfo = new KRoomUserInfo { UnitUid = 1002, NickName = "Bride" } };
        var guestSlot = new KRoomSlotInfo { Index = 2, RoomUserInfo = new KRoomUserInfo { UnitUid = 1003, NickName = "Guest" } };

        Assert.Equal(NetError.NET_OK, hall.OpenHall(new KWeddingHallInfo { WeddingUid = 55, Groom = 1001, Bride = 1002 }, groomSlot, t0).ok);
        Assert.Equal(KCnWeddingHall.HallState.Play, hall.State);

        // Partner not in hall yet -> ERR_RELATIONSHIP_42
        Assert.Equal(NetError.ERR_RELATIONSHIP_42, hall.CheckStartWedding(1001).ok);

        Assert.Equal(NetError.NET_OK, hall.JoinHall(guestSlot));
        // Non-groom/bride cannot start wedding -> ERR_RELATIONSHIP_41
        Assert.Equal(NetError.ERR_RELATIONSHIP_41, hall.CheckStartWedding(1003).ok);

        Assert.Equal(NetError.NET_OK, hall.JoinHall(brideSlot));
        Assert.Equal((NetError.NET_OK, 1002L), hall.CheckStartWedding(1001));
        Assert.True(hall.RespondStartWedding(1002, true));
        Assert.True(hall.HallInfo.Success);

        // Empty hall transitions to Wait and closes after >10s
        Assert.Equal(NetError.NET_OK, hall.LeaveHall(1001, t0));
        Assert.Equal(NetError.NET_OK, hall.LeaveHall(1002, t0));
        Assert.Equal(NetError.NET_OK, hall.LeaveHall(1003, t0));
        Assert.Equal(KCnWeddingHall.HallState.Wait, hall.State);
        Assert.False(hall.Tick(t0.AddSeconds(9.0)));
        Assert.True(hall.Tick(t0.AddSeconds(10.5)));
        Assert.Equal(KCnWeddingHall.HallState.Close, hall.State);

        var square = new KCnSquare { SquareUid = 200, SquareType = 1, RoomName = "Ruben" };
        for (int i = 1; i <= KCnSquare.SquareMaxSlot; i++)
            Assert.Equal(NetError.NET_OK, square.JoinSquare(new KCnSquare.SquareMember { UnitUid = i, GsUid = 10, NickName = $"U{i}" }));
        Assert.Equal(NetError.ERR_ROOM_09, square.JoinSquare(new KCnSquare.SquareMember { UnitUid = 999, GsUid = 10 }));
        Assert.True(square.SyncPosition(1, 100f, 50f, -20f));
        Assert.Equal(60, square.DeleteByGsUid(10));
        Assert.True(square.IsEmpty);
    }

    [Fact]
    public void GameServer_UserGuild_Statistics_EventManager_And_CorruptionMonitor()
    {
        var guild = new KUserGuildManager();
        guild.SetGuildInfo(new KUserGuildInfo { GuildUid = 1, GuildName = "ElSearch", MemberShipGrade = KUserGuildManager.GradeMaster, HonorPoint = 1500 });
        Assert.True(guild.IsGuildMaster);
        Assert.Equal(1, guild.AddHonorPointAndCheckRollover(200)); // 1700 -> +1 EXP, 100 remaining
        Assert.Equal(100, guild.HonorPoint);

        // New user grade gets no passive guild skills; regular grade does
        guild.UpdateGuildSkill(101, 3, 0);
        Assert.Single(guild.GetGuildPassiveSkillData());
        guild.SetMemberShipGrade(KUserGuildManager.GradeNewUser);
        Assert.Empty(guild.GetGuildPassiveSkillData());

        // Guild foundation purchase checks
        Assert.Equal(NetError.ERR_GUILD_37, guild.CheckBuyGuildFoundation(0, true, false).netError);
        guild.Clear();
        Assert.Equal(NetError.ERR_GUILD_44, guild.CheckBuyGuildFoundation(0, false, false).netError);
        Assert.Equal(NetError.ERR_GUILD_38, guild.CheckBuyGuildFoundation(0, true, true).netError);
        Assert.True(guild.CheckBuyGuildFoundation(0, true, false).ok);

        var stats = new KUserStatistics();
        stats.SetIsNormalUser(5); // GM does not pollute statistics
        stats.IncreaseCount(KUserStatistics.UsTable.Dungeon, 30000, 0, 1);
        Assert.Equal(0, stats.GetCount(KUserStatistics.UsTable.Dungeon, 30000, 0));
        stats.SetIsNormalUser(0);
        stats.IncreaseCount(KUserStatistics.UsTable.Dungeon, 30000, 0, 2);
        stats.IncreaseCount(KUserStatistics.UsTable.Dungeon, 30000, 0, 3);
        Assert.Equal(5, stats.GetCount(KUserStatistics.UsTable.Dungeon, 30000, 0));
        stats.IncreaseAccountCount(KUserStatistics.AccountGameCountType.BuyCashItem);
        Assert.True(stats.IsFirstBuy);

        var ev = new KUserEventManager();
        var baseUtc = DateTime.UtcNow;
        ev.RegisterCumulativeEvent(701, initialMinutes: 28, targetMinutes: 30);
        ev.RegisterConnectTimeEvent(702, baseUtc, intervalSeconds: 60.0, giveOnce: true);
        var fired = ev.AdvanceTime(130.0, baseUtc.AddSeconds(65));
        Assert.Contains(701, fired);
        Assert.Contains(702, fired);
        Assert.True(ev.IsCompleted(701));
        Assert.True(ev.IsCompleted(702));

        var mon = new KCorruptionMonitor { MonitoringOn = true };
        mon.SetEdThreshold(1, 50_000_000L);
        mon.SetItemThreshold(130010, 500);
        Assert.True(mon.IsEdAbnormal(1, 60_000_000L));
        Assert.False(mon.IsItemAbnormal(130010, 100));
    }
}
