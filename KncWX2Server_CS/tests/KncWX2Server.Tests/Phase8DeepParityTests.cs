using KncWX2Server.CenterServer;
using KncWX2Server.Core.Packets;
using KncWX2Server.Core.Region;
using KncWX2Server.GameServer;
using Xunit;

namespace KncWX2Server.Tests;

public sealed class Phase8DeepParityTests
{
    [Fact]
    public void TutorialManager_And_UserTutorialInfo_EnforceLevelCapsPaginationAndGraduation()
    {
        var tm = new KTutorialManager();
        Assert.False(tm.AddUnit(new KTutorialUnitInfo { UnitUid = 99, Level = 15, NickName = "TooHigh" }));

        for (int i = 1; i <= 7; i++)
            Assert.True(tm.AddUnit(new KTutorialUnitInfo { UnitUid = i, Level = (byte)(i + 1), NickName = $"Stu{i}" }));

        Assert.False(tm.AddUnit(new KTutorialUnitInfo { UnitUid = 1, Level = 2, NickName = "Dup" }));
        Assert.Equal(7, tm.StudentPoolCount);

        var page1 = tm.GetTutorPageInfo(1);
        var page2 = tm.GetTutorPageInfo(2);
        Assert.Equal(2, page1.TotalPage);
        Assert.Equal(5, page1.TutorialStudentList.Count);
        Assert.Equal(2, page2.TutorialStudentList.Count);

        // Reaching Level 20 graduates and removes student from pool
        Assert.True(tm.SetLevelData(1, 20));
        Assert.False(tm.IsExistByList(1));

        Assert.Equal(NetError.ERR_TUTORIAL_07, KTutorialManager.GetDelReason(KTutorialManager.TutorialDelReason.TdrDisconnectDay));
        Assert.Equal(NetError.ERR_TUTORIAL_08, KTutorialManager.GetDelReason(KTutorialManager.TutorialDelReason.TdrLevelUpEnd));
        Assert.Equal(NetError.ERR_TUTORIAL_09, KTutorialManager.GetDelReason(KTutorialManager.TutorialDelReason.TdrDeleteUnit));
        Assert.Equal(NetError.ERR_TUTORIAL_10, KTutorialManager.GetDelReason(KTutorialManager.TutorialDelReason.TdrRefusalUser));

        var tut = new KUserTutorialInfo();
        Assert.True(tut.AddRelation(KUserTutorialInfo.TutorialUnitType.TutTeacher, new KTutorialUnitInfo { UnitUid = 500, Level = 50, NickName = "Master" }));
        Assert.False(tut.AddRelation(KUserTutorialInfo.TutorialUnitType.TutTeacher, new KTutorialUnitInfo { UnitUid = 501, Level = 55, NickName = "ExtraMaster" }));
        Assert.True(tut.CheckGraduationOnLevelUp(19, 20));
        Assert.False(tut.CheckGraduationOnLevelUp(20, 21));
    }

    [Fact]
    public void UserWishList_Tracks21Slots_CompactsOnDelete_AndFlushesDirtySlots()
    {
        var wish = new KUserWishList();
        wish.InitWishList(new Dictionary<int, int> { [0] = 130001, [1] = 130002, [2] = 130003 });
        Assert.Empty(wish.FlushWishListChange());

        // Duplicate returns ok without duplicating
        Assert.True(wish.AddWish(130002).ok);
        Assert.Equal(3, wish.GetWishList().Count);

        // Delete middle item compacts later items left (0=130001, 1=130003)
        Assert.True(wish.DelWish(130002));
        var map = wish.GetWishList();
        Assert.Equal(2, map.Count);
        Assert.Equal(130001, map[0]);
        Assert.Equal(130003, map[1]);

        var dirty = wish.FlushWishListChange();
        Assert.Equal(130003, dirty[1]);
        Assert.Equal(0, dirty[2]);
        Assert.Empty(wish.FlushWishListChange());

        // Fill up to 21 slots then verify ERR_WISH_LIST_02
        for (int i = 0; i < 19; i++)
            Assert.True(wish.AddWish(200000 + i).ok);
        var (fullOk, fullErr) = wish.AddWish(299999);
        Assert.False(fullOk);
        Assert.Equal(NetError.ERR_WISH_LIST_02, fullErr);
    }

    [Fact]
    public void CompanyIp_ItemVariation_ManufactureManager_And_BadAttitudeTable()
    {
        var ipTable = new KCompanyIPTable { RestrictGmCheatByIp = true };
        Assert.True(ipTable.CheckIp("127.0.0.1"));
        Assert.False(ipTable.CheckIp("203.0.113.5"));
        Assert.True(ipTable.AddCompanyIp("203.0.113.5"));
        Assert.True(ipTable.CheckIp("203.0.113.5"));

        var varTracker = new KItemQuantityVariation();
        varTracker.IncreaseQuantity(130010, 5);
        varTracker.IncreaseQuantity(130010, 3);
        varTracker.DecreaseQuantity(130010, 2);
        Assert.Equal(8, varTracker.GetIncreaseSnapshot()[130010]);
        Assert.Equal(2, varTracker.GetDecreaseSnapshot()[130010]);

        var mfg = new KManufactureItemManager();
        mfg.AddManufactureResultGroupWithRate(50, 70001, 100.0, isSealed: true, periodDays: 30);
        mfg.RegisterManufacture(new KManufactureItemManager.ManufactureData
        {
            ManufactureId = 10,
            ResultGroupId = 50,
            Cost = 5000,
            Materials = [new KManufactureItemManager.MaterialData(130010, 3)],
        });
        Assert.True(mfg.IsPossibleManufactureId(10));
        int crafted = mfg.RollResultItemId(10, fixedRoll: 0.25);
        Assert.Equal(70001, crafted);
        Assert.True(mfg.IsItemSealed(70001));
        Assert.Equal(30, mfg.GetResultItemPeriod(50, 70001));

        var badTable = new KBadAttitudeTable();
        Assert.Equal(KBadAttitudeTable.DefaultThreshold, badTable.GetBadAttitudeCutLinePoint(0));
        badTable.SetBadAttitudeCutLinePoint(2, 500);
        badTable.SetForceExitPoint(2, 800);
        Assert.Equal(500, badTable.GetBadAttitudeCutLinePoint(2));
        Assert.Equal(800, badTable.GetForceExitPoint(2));
    }
}
