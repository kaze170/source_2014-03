using KncWX2Server.CenterServer;
using KncWX2Server.Core.Region;
using KncWX2Server.GameServer;
using Xunit;

namespace KncWX2Server.Tests;

public sealed class Phase11DeepParityTests
{
    [Fact]
    public void UserPvpManager_EnforcesSeason2HysteresisRanksCompensationAndRollback()
    {
        var pvp = new KUserPvpManager();
        pvp.Init(10, 240, 240, 50, 100, false, 6, 4, KUserPvpManager.PvpRank.E, 15f, false, 0);

        // Rank up E -> D at >=250 resets RPoint to 0
        Assert.True(pvp.AddRatingAndRankUpDownProcess(20)); // 260
        Assert.Equal(KUserPvpManager.PvpRank.D, pvp.Rank);
        Assert.Equal(260, pvp.MaxRating);
        Assert.Equal(0, pvp.RPoint);

        // Hysteresis keeps D at 200 (since D lower bound is 150)
        Assert.False(pvp.AddRatingAndRankUpDownProcess(-60));
        Assert.Equal(KUserPvpManager.PvpRank.D, pvp.Rank);

        // Dropping below 150 demotes back to E with 67 compensation RPoint
        Assert.True(pvp.AddRatingAndRankUpDownProcess(-60)); // 140
        Assert.Equal(KUserPvpManager.PvpRank.E, pvp.Rank);
        Assert.Equal(67, pvp.RPoint);

        pvp.AddKFactor(1000f);
        Assert.Equal(KUserPvpManager.KFactorMax, pvp.KFactor);

        pvp.UpdateWinLose(1, 0, true);
        Assert.True(pvp.IsWinBeforeMatch);
        var deltas = pvp.FlushDbDeltas();
        Assert.Equal(1, deltas.dWin);
        Assert.Equal(-100, deltas.dRating);
        pvp.RollbackDbDeltas(deltas.dMatchCnt, deltas.dRating, deltas.dMaxRating, deltas.dRPoint, deltas.dAPoint, deltas.dWin, deltas.dLose, deltas.dKFactor);
        Assert.Equal(-100, pvp.FlushDbDeltas().dRating);
    }

    [Fact]
    public void UserPartyManager_Synthesis_EdExpand_Cheat_And_CnPersonalShopRoom()
    {
        var pm = new KUserPartyManager();
        var t0 = DateTime.UtcNow;
        pm.AddKickedPartyUid(50, t0);
        Assert.True(pm.CheckKickedPartyUid(50));
        pm.CheckExpiredKickedPartyUid(t0.AddSeconds(21));
        Assert.False(pm.CheckKickedPartyUid(50));
        Assert.True(pm.RegWaitPartyUser(77, t0));
        pm.CheckWaitPartyUser(t0.AddSeconds(11));
        Assert.True(pm.IsEmptyWaitPartyUser);

        var syn = new KSynthesisManager();
        Assert.True(syn.AddAvatarSocketOptionForEquipPosition(KSynthesisManager.EquipWeaponHand, 501));
        Assert.False(syn.AddAvatarSocketOptionForEquipPosition(99, 501));
        int gid = KSynthesisManager.ComputeGroupId(1, KSynthesisManager.EquipWeaponHand); // 1*10 + (2-2) = 10
        Assert.Equal(10, gid);
        Assert.True(syn.AddSynthesisGroup(10, 80001, 100f));
        var (ok, itemId, item) = syn.GetSynthesisResultItem(1, KSynthesisManager.EquipWeaponHand, 999, fixedRoll: 0.1);
        Assert.Equal(NetError.NET_OK, ok);
        Assert.Equal(80001, itemId);
        Assert.Equal(999, item!.ItemSockets[0]);

        var expand = new KEdInventoryExpand();
        expand.SetBankGradeEd(2, 500_000);
        expand.SetInventoryGradeEd(3, 750_000);
        Assert.Equal(500_000, expand.GetNextBankUpgradeEd(2));
        Assert.Equal(750_000, expand.GetNextInventoryUpgradeEd(3));

        var cheat = new KUserCheatManager { ObserverMode = true };
        Assert.True(cheat.ObserverMode);

        var shop = new KCnPersonalShopRoom { ShopUid = 300 };
        Assert.Equal(NetError.NET_OK, shop.OpenShop(100, "GemShop", isHostPremium: false));
        Assert.Equal(NetError.NET_OK, shop.RegisterItems(100, [
            new KCnPersonalShopRoom.ShopListing { ItemUid = 1, ItemId = 130010, Quantity = 2, PricePerOne = 1000 },
        ]));
        Assert.Equal(NetError.NET_OK, shop.JoinShop(200));
        Assert.Equal(NetError.ERR_PERSONAL_SHOP_15, shop.BuyItem(100, 1, 1).ok);
        var buy = shop.BuyItem(200, 1, 2, viaTradeBoard: false);
        Assert.Equal(NetError.NET_OK, buy.ok);
        Assert.Equal(2000, buy.totalCost);
        Assert.Equal(200, buy.commissionFee); // 10% non-premium commission
        Assert.Equal(1800, buy.netSellerProceeds);
        Assert.True(buy.shopSoldOut);
        Assert.Equal(KCnPersonalShopRoom.ShopState.Close, shop.State);
    }
}
