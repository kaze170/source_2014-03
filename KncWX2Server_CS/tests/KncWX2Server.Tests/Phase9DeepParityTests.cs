using KncWX2Server.CenterServer;
using KncWX2Server.Core.Packets;
using KncWX2Server.Core.Region;
using KncWX2Server.GameServer;
using KncWX2Server.GlobalServer;
using KncWX2Server.LoginServer;
using Xunit;

namespace KncWX2Server.Tests;

public sealed class Phase9DeepParityTests
{
    [Fact]
    public void GlobalServer_ServerSN_And_GlobalCcu_EnforceOverlapAndAggregation()
    {
        var sn = new KServerSNManager();
        var (r1, o1) = sn.RegServerSN(999999L, 1001, 1);
        Assert.Equal(NetError.NET_OK, r1);
        Assert.Empty(o1);

        var (r2, o2) = sn.RegServerSN(999999L, 1002, 2);
        Assert.Equal(NetError.ERR_SN_OVERLAP_01, r2);
        Assert.Equal(2, o2.Count);

        Assert.Equal(NetError.NET_OK, sn.UnRegServerSN(999999L, 1001));
        Assert.Equal(NetError.ERR_SN_OVERLAP_02, sn.UnRegServerSN(999999L, 1001));
        Assert.Equal(NetError.NET_OK, sn.UnRegServerSN(999999L, 1002));
        Assert.Equal(0, sn.ActiveSerialCount);

        var ccu = new KGlobalServerCCUManager();
        ccu.UpdateServerCcu(10, 0, 120, 15);
        ccu.UpdateServerCcu(20, 1, 80, 10);
        Assert.Equal(200, ccu.GetTotalConcurrentUser());
        Assert.Equal(120, ccu.GetGroupConcurrentUser(0));
    }

    [Fact]
    public void LoginServer_BuffManager_And_ServerCcu_Purge6HoursAndDeduplicateMinute()
    {
        var bm = new KLoginBuffManager();
        long t0 = 1_700_000_000L;
        Assert.True(bm.AddUnitBuffInfo(5001, [new KLoginBuffManager.CachedBuffInfo { BuffTempletId = 126, FactorId = 1 }], t0));
        Assert.Equal(1, bm.CachedUnitCount);

        // Renew within 6h keeps alive
        Assert.True(bm.GetUnitAllBuffInfo(5001, out var entry, t0 + 3600));
        Assert.NotNull(entry);
        Assert.Equal(0, bm.CheckEndBuff(t0 + 3600 + (5 * 3600)));

        // Exceed 6 hours purges expired cached unit buffs
        Assert.Equal(1, bm.CheckEndBuff(t0 + 3600 + (6 * 3600) + 1));
        Assert.Equal(0, bm.CachedUnitCount);

        var ccu = new KServerCCUManager();
        ccu.UpdateCcuInfo(new KServerCCUManager.ServerCcuInfo { ServerUid = 1, ConcurrentUser = 55, CurPartyCount = 4 });
        var now = new DateTime(2026, 11, 20, 12, 30, 0, DateTimeKind.Utc);
        var (emitted1, total1, snap1) = ccu.TryBuildDbUpdate(now);
        Assert.True(emitted1);
        Assert.Equal(55, total1.ConcurrentUser);
        Assert.Equal(2, snap1.Count);
        Assert.False(ccu.TryBuildDbUpdate(now.AddSeconds(20)).emitted);
        Assert.True(ccu.TryBuildDbUpdate(now.AddMinutes(1)).emitted);
    }

    [Fact]
    public void CenterServer_CnTradeRoom_ExecutesFullHandshakeMaxEdAndTimeout()
    {
        var room = new KCnTradeRoom { TradeUid = 77 };
        var t0 = DateTime.UtcNow;
        Assert.Equal(NetError.NET_OK, room.OpenTrade(new KCnTradeRoom.TradeParticipant
        {
            UserUid = 1, UnitUid = 10, GsUid = 100, NickName = "Alice", CurrentEd = 1_999_999_000,
        }, t0).ok);

        // 5-second timeout check before join
        var timeoutRoom = new KCnTradeRoom { TradeUid = 78 };
        timeoutRoom.OpenTrade(new KCnTradeRoom.TradeParticipant { UserUid = 1, UnitUid = 10, GsUid = 100 }, t0);
        Assert.True(timeoutRoom.TickWaitTimeout(t0.AddSeconds(5.1)));
        Assert.Equal(KCnTradeRoom.TradeRoomState.Close, timeoutRoom.State);
        Assert.Equal(NetError.NOT_LEAVE_ROOM_REASON_08, timeoutRoom.BreakReason);

        Assert.Equal(NetError.NET_OK, room.JoinTrade(new KCnTradeRoom.TradeParticipant
        {
            UserUid = 2, UnitUid = 20, GsUid = 100, NickName = "Bob", CurrentEd = 1_999_999_500,
        }));

        // Alice offers 1000 ED -> Bob would exceed MaxHoldEd -> ERR_TRADE_18
        Assert.Equal(NetError.NET_OK, room.UpdateTradeEd(10, 1000));
        Assert.Equal(NetError.ERR_TRADE_18, room.OfferTrade(10).ok);

        // Valid ED + item
        Assert.Equal(NetError.NET_OK, room.UpdateTradeEd(10, 100));
        Assert.Equal(NetError.NET_OK, room.UpdateTradeItem(20, new KInventoryItemInfo { ItemUid = 9001 }, false));
        Assert.Equal(NetError.ERR_TRADE_15, room.UpdateTradeItem(20, new KInventoryItemInfo { ItemUid = 9001 }, false));

        Assert.False(room.OfferTrade(10).bothOffered);
        Assert.True(room.OfferTrade(20).bothOffered);
        Assert.False(room.ApproveTrade(10).bothApproved);
        Assert.True(room.ApproveTrade(20).bothApproved);
        Assert.Equal(KCnTradeRoom.TradeRoomState.Confirm, room.State);

        Assert.False(room.ConfirmTrade(10).bothConfirmed);
        Assert.True(room.ConfirmTrade(20).bothConfirmed);
        Assert.Equal(KCnTradeRoom.TradeRoomState.DbUpdate, room.State);
    }

    [Fact]
    public void GameServer_Jumping_GameOption_LocalRanking_And_RidingPetRecovery()
    {
        var jump = new KUserJumpingCharacterManager();
        Assert.Equal(KUserJumpingCharacterManager.JumpingCharacterResult.JcrNotInit, jump.IsJumpingCharacter(10));
        Assert.Equal(KUserJumpingCharacterManager.JumpingCharacterResult.JcrSucceed,
            jump.Init(new Dictionary<long, string> { [10] = "2026-11-15 10:00:00" }));
        Assert.Equal(KUserJumpingCharacterManager.JumpingCharacterResult.JcrFailed,
            jump.Check(20, new DateTime(2026, 11, 1), new DateTime(2026, 11, 30), accountEvent: true));
        Assert.Equal(KUserJumpingCharacterManager.JumpingCharacterResult.JcrSucceed,
            jump.Check(20, new DateTime(2026, 11, 1), new DateTime(2026, 11, 30), accountEvent: false));

        var opt = new KUserGameOption();
        Assert.False(opt.SetDenyOptions(new KDenyOptions { DenyFriendShip = 2 })); // > DOS_OFF
        Assert.True(opt.SetDenyOptions(new KDenyOptions { DenyFriendShip = 0, DenyParty = 2 }));
        Assert.True(opt.IsDenyFriendShip);
        for (int i = 1; i <= 30; i++) Assert.True(opt.AddChatBlackListUnit(i, $"B{i}"));
        Assert.False(opt.AddChatBlackListUnit(31, "Overflow"));

        var rank = new KUserLocalRankingInfo();
        rank.InitInfo(1, 100, 200, 2, 10, 20, "2026-11-20");
        rank.IncreasedUnitInfo(5, 15);
        Assert.Equal((5, 15), rank.GetChangedValue());
        Assert.Equal((0, 0), rank.GetChangedValue());
        rank.AdjustInitValue(5, 15);
        Assert.Equal((5, 15), rank.GetChangedValue());

        var rp = new KRidingPetManager { EnableStamina = 10f, RecoveryTimePerOne = 3f };
        rp.AddRidingPetTemplet(1, 100, 1.0f);
        var pet = new KRidingPetInfo { RidingPetId = 1, Stamina = 20f, LastUnSummonDate = 1000 };
        rp.ApplyOfflineRecovery(pet, created: false, nowUnixOverride: 1090); // +90 / 3 = +30 => 50
        Assert.Equal(50f, pet.Stamina);
        Assert.True(rp.EnableCheck(pet));
    }
}
