using System.Buffers.Binary;
using System.Net.Sockets;
using KncWX2Server.CenterServer;
using KncWX2Server.Core.Events;
using KncWX2Server.Core.Framework;
using KncWX2Server.Core.Packets;
using KncWX2Server.Core.Region;
using KncWX2Server.Core.Security;
using KncWX2Server.Core.Serialization;
using KncWX2Server.GameServer;
using KncWX2Server.GlobalServer;
using KncWX2Server.LoginServer;
using Xunit;

namespace KncWX2Server.Tests;

public sealed class Phase16WireAndGameplayCompletenessTests : IDisposable
{
    private readonly string _dbPath;

    public Phase16WireAndGameplayCompletenessTests()
    {
        _dbPath = Path.Combine(Path.GetTempPath(), $"knc_p16_{Guid.NewGuid():N}.db");
    }

    public void Dispose()
    {
        try { if (File.Exists(_dbPath)) File.Delete(_dbPath); } catch { }
    }

    [Fact]
    public void SubCollections_And_NewClientPackets_RoundTrip_Strictly()
    {
        var region = RegionProfile.Taiwan;

        var roomUser = new KRoomUserInfo
        {
            GsUid = 10,
            OwnerUserUid = 20,
            UnitUid = 30,
            NickName = "WireHero",
            GamePlayStatus = [new KGamePlayStatus { MaxHp = 50000, CurHp = 42000, MaxMp = 300, CurMp = 250 }],
            BuffInfo = [new KBuffInfo { FactorInfo = new KBuffFactor { FactorId = 77 }, BuffStartTime = 100, BuffEndTime = 200 }],
            OngoingQuest = new Dictionary<int, KSubQuestInfo>
            {
                [1001] = new KSubQuestInfo
                {
                    SubQuestInfo = new Dictionary<int, KSubQuestData>
                    {
                        [1] = new KSubQuestData { InInventoryItemCount = 3, Success = true },
                    },
                },
            },
            Pet = [new KPetInfo { PetUid = 999, PetId = 5, PetName = "Uniring", Satiety = 3600 }],
            QuestInfo = [1000, 1001],
            GoingQuestInfo = [1001, 1002],
            UseSkillBuffInPlay = [55, 56],
        };

        var decRoomUser = RoundTrip(roomUser, region);
        Assert.Single(decRoomUser.GamePlayStatus);
        Assert.Equal(42000, decRoomUser.GamePlayStatus[0].CurHp);
        Assert.Single(decRoomUser.BuffInfo);
        Assert.Equal(77, decRoomUser.BuffInfo[0].FactorInfo.FactorId);
        Assert.True(decRoomUser.OngoingQuest[1001].SubQuestInfo[1].Success);
        Assert.Equal(3, decRoomUser.OngoingQuest[1001].SubQuestInfo[1].InInventoryItemCount);
        Assert.Single(decRoomUser.Pet);
        Assert.Equal("Uniring", decRoomUser.Pet[0].PetName);
        Assert.Contains(1001, decRoomUser.QuestInfo);
        Assert.Contains(1002, decRoomUser.GoingQuestInfo);
        Assert.Contains(56, decRoomUser.UseSkillBuffInPlay);

        var unitInfo = new KUnitInfo
        {
            OwnerUserUid = 1,
            UnitUid = 2,
            NickName = "BuffUnit",
            BuffInfo = [new KBuffInfo { FactorInfo = new KBuffFactor { FactorId = 88 }, BuffStartTime = 10, BuffEndTime = 90 }],
        };
        var decUnit = RoundTrip(unitInfo, region);
        Assert.Single(decUnit.BuffInfo);
        Assert.Equal(88, decUnit.BuffInfo[0].FactorInfo.FactorId);

        var partyUser = new KPartyUserInfo
        {
            UnitUid = 50,
            NickName = "PartyLead",
            GamePlayStatus = new KGamePlayStatus { MaxHp = 33000, CurHp = 21000, MaxMp = 300, CurMp = 120 },
        };
        var decPartyUser = RoundTrip(partyUser, region);
        Assert.Equal(21000, decPartyUser.GamePlayStatus.CurHp);

        var applyGuild = RoundTrip(new KEGS_APPLY_JOIN_GUILD_REQ { GuildUid = 42, Message = "Hello", OldApplyDelete = true }, region);
        Assert.Equal(42, applyGuild.GuildUid);
        Assert.Equal("Hello", applyGuild.Message);
        Assert.True(applyGuild.OldApplyDelete);

        var admLvl = RoundTrip(new KEGS_ADMIN_MODIFY_UNIT_LEVEL_REQ { UnitNickName = "Target", Level = 70, Exp = 999 }, region);
        Assert.Equal("Target", admLvl.UnitNickName);
        Assert.Equal(70, admLvl.Level);

        var admEd = RoundTrip(new KEGS_ADMIN_CHANGE_ED_REQ { UnitNickName = "Target", Ed = 123456 }, region);
        Assert.Equal(123456, admEd.Ed);

        var admSp = RoundTrip(new KEGS_ADMIN_CHANGE_SPIRIT_ACK { Ok = NetError.NET_OK, ChangedSpirit = 85 }, region);
        Assert.Equal(85, admSp.ChangedSpirit);

        var dieNot = RoundTrip(new KEGS_USER_UNIT_DIE_COMPLETE_NOT { UnitUid = 987654 }, region);
        Assert.Equal(987654, dieNot.UnitUid);

        var tradeNot = RoundTrip(new KEGS_CREATE_TRADE_NOT { TradeUid = 55667788L }, region);
        Assert.Equal(55667788L, tradeNot.TradeUid);

        var breakUpNot = RoundTrip(new KEGS_BREAK_UP_NOT { Cost = 100000 }, region);
        Assert.Equal(100000, breakUpNot.Cost);

        var coboNot = RoundTrip(new KEGS_EVENT_COBO_DUNGEON_FIELD_NOT
        {
            StartUi = true,
            DungeonCountUi = true,
            DungeonCount = 3,
            Ok = NetError.NET_OK,
            RemaindTime = 120,
            PushTimeUnix = 1700000000L,
        }, region);
        Assert.True(coboNot.StartUi);
        Assert.Equal(1700000000L, coboNot.PushTimeUnix);

        var guildInfoNot = RoundTrip(new KEGS_GUILD_INFO_NOT
        {
            GuildInfo = new KGuildInfo { GuildUid = 7, GuildName = "TWGuild" },
            GuildMasterName = "Master",
            MemberUnitUid = [11L, 22L],
        }, region);
        Assert.Equal("TWGuild", guildInfoNot.GuildInfo.GuildName);
        Assert.Equal(2, guildInfoNot.MemberUnitUid.Count);

        var dgResult = RoundTrip(new KEGS_END_GAME_DUNGEON_RESULT_DATA_NOT
        {
            Sec = 180,
            IsWin = true,
            DungeonId = 30000,
            OngoingQuest = new Dictionary<long, Dictionary<int, KSubQuestInfo>>
            {
                [30L] = roomUser.OngoingQuest,
            },
            HaveExpInDungeon = new Dictionary<long, bool> { [30L] = true },
            SuitableLevelInfo = new Dictionary<long, bool> { [30L] = true },
        }, region);
        Assert.True(dgResult.HaveExpInDungeon[30L]);
        Assert.True(dgResult.SuitableLevelInfo[30L]);
        Assert.True(dgResult.OngoingQuest[30L][1001].SubQuestInfo[1].Success);
    }

    [Fact]
    public async Task GameAndLoginServers_Handle_WireAlignedPackets_MapPrereqs_Coupons_And_LiveEvents()
    {
        var router = new InProcessRouter();
        using var center = new KCenterServer(RegionCode.TW, router);
        using var global = new KGlobalServer(RegionCode.TW, router);
        using var login = new KLoginServer(RegionCode.TW, _dbPath, router);
        using var game = new KGameServer(RegionCode.TW, _dbPath, router, center, global);

        game.MapDatas[20004] = new KGameServer.MapDataEntry(20004, 20003, 20005, false, 15, 30, [30010]);
        game.Db.GrantAccountCoupon(1, "TW-ITEM-01", couponType: 0, productNo: 5001, itemId: 130048, quantity: 2);
        game.Db.GrantAccountCoupon(1, "TW-DISC-20", couponType: 1, productNo: 20, itemId: 70001, quantity: 1);

        center.Start(0);
        global.Start(0);
        login.Start(0);
        game.Start(0);

        try
        {
            // 1. LoginServer EGS_GET_SERVER_SET_DATA_REQ returns KEGS_GET_SERVER_SET_DATA_ACK
            using (var lgClient = new TcpClient())
            {
                await lgClient.ConnectAsync("127.0.0.1", login.ListenPort);
                using var lgStream = lgClient.GetStream();
                var lgSa = new KSADatabase();
                ushort lgSpi = await HandshakeAsync(lgStream, lgSa, RegionProfile.Taiwan);

                await SendNoParamAsync(lgStream, lgSa, lgSpi, RegionProfile.Taiwan, PerformerId.PI_LOGIN_USER,
                    EventId.EGS_GET_SERVER_SET_DATA_REQ);
                var setEv = await ReadEventAsync(lgStream, lgSa, RegionProfile.Taiwan);
                Assert.Equal(EventId.EGS_GET_SERVER_SET_DATA_ACK, setEv.EventId);
                var setAck = setEv.GetPayload<KEGS_GET_SERVER_SET_DATA_ACK>(RegionProfile.Taiwan);
                Assert.Equal(NetError.NET_OK, setAck.Ok);
            }

            // 2. GameServer gameplay & wire-aligned handlers
            using var gsClient = new TcpClient();
            await gsClient.ConnectAsync("127.0.0.1", game.ListenPort);
            using var gsStream = gsClient.GetStream();
            var gsSa = new KSADatabase();
            ushort gsSpi = await HandshakeAsync(gsStream, gsSa, RegionProfile.Taiwan);

            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_CONNECT_REQ, new KEGS_CONNECT_REQ { Version = "TW_2014_03" });
            Assert.Equal(EventId.EGS_CONNECT_ACK, (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).EventId);

            game.Db.VerifyOrRegisterAccount(RegionProfile.Taiwan, "p16gm", "p16gm", false);
            game.Db.SetAccountAuthLevel(RegionProfile.Taiwan, "p16gm", 3);

            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_VERIFY_ACCOUNT_REQ, new KUserAuthenticateReq { UserId = "p16gm", Password = "p16gm" });
            var verifyAck = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KEGS_VERIFY_ACCOUNT_ACK>(RegionProfile.Taiwan);
            Assert.Equal(NetError.NET_OK, verifyAck.Ok);
            long accountUid = verifyAck.AccountInfo.UserUid;

            int itemCardNo = game.Db.GrantAccountCoupon(accountUid, "TW-ITEM-99", couponType: 0, productNo: 5002, itemId: 130048, quantity: 3);
            game.Db.GrantAccountCoupon(accountUid, "TW-DISC-30", couponType: 1, productNo: 30, itemId: 77777, quantity: 1);

            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_CREATE_UNIT_REQ, new KEGS_CREATE_UNIT_REQ { NickName = "P16Hero", UnitClass = 1 });
            long unitUid = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan))
                .GetPayload<KEGS_CREATE_UNIT_ACK>(RegionProfile.Taiwan).UnitInfo.UnitUid;

            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_SELECT_UNIT_REQ, new KEGS_SELECT_UNIT_REQ { UnitUid = unitUid });
            Assert.Equal(EventId.EGS_SELECT_UNIT_1_NOT, (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).EventId);
            Assert.Equal(EventId.EGS_SELECT_UNIT_2_NOT, (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).EventId);
            Assert.Equal(EventId.EGS_SELECT_UNIT_3_NOT, (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).EventId);
            Assert.Equal(EventId.EGS_SELECT_UNIT_4_NOT, (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).EventId);
            Assert.Equal(EventId.EGS_SELECT_UNIT_5_NOT, (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).EventId);
            Assert.Equal(EventId.EGS_SELECT_UNIT_ACK, (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).EventId);
            Assert.Equal(EventId.EGS_GET_MY_INVENTORY_ACK, (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).EventId);

            // GM packets using exact C++ wire structs
            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_ADMIN_MODIFY_UNIT_LEVEL_REQ, new KEGS_ADMIN_MODIFY_UNIT_LEVEL_REQ { UnitNickName = "P16Hero", Level = 20 });
            var modLvlAck = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KEGS_ADMIN_MODIFY_UNIT_LEVEL_ACK>(RegionProfile.Taiwan);
            Assert.Equal(NetError.NET_OK, modLvlAck.Ok);
            Assert.Equal(20, modLvlAck.UnitInfo.Level);

            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_ADMIN_CHANGE_ED_REQ, new KEGS_ADMIN_CHANGE_ED_REQ { UnitNickName = "P16Hero", Ed = 250_000 });
            Assert.Equal(NetError.NET_OK, (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KPacketOK>(RegionProfile.Taiwan).Ok);

            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_ADMIN_CHANGE_SPIRIT_REQ, new KEGS_ADMIN_CHANGE_SPIRIT_REQ { Spirit = 75 });
            var spAck = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KEGS_ADMIN_CHANGE_SPIRIT_ACK>(RegionProfile.Taiwan);
            Assert.Equal(NetError.NET_OK, spAck.Ok);
            Assert.Equal(75, spAck.ChangedSpirit);

            // MapData REQUIRE_DUNGEON check -> ERR_FIELD_06 for normal auth level
            game.Db.SetAccountAuthLevel(RegionProfile.Taiwan, "p16gm", 0);
            // Re-verify or test via unit AuthLevel (which is 0 on unit row)
            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_STATE_CHANGE_FIELD_REQ, new KEGS_STATE_CHANGE_FIELD_REQ { MapId = 20004 });
            var mapAck = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KEGS_STATE_CHANGE_FIELD_ACK>(RegionProfile.Taiwan);
            Assert.Equal(NetError.ERR_FIELD_06, mapAck.Ok);

            // Leave Personal Shop before joining returns ERR_PERSONAL_SHOP_12
            await SendNoParamAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_LEAVE_PERSONAL_SHOP_REQ);
            Assert.Equal(NetError.ERR_PERSONAL_SHOP_12, (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KPacketOK>(RegionProfile.Taiwan).Ok);

            // Personal Shop buy sends KPacketOK ACK + KEGS_BUY_PERSONAL_SHOP_ITEM_COMPLETE_NOT
            long listingUid = game.Db.CreatePersonalShopListing(RegionProfile.Taiwan, unitUid + 999, "OtherSeller", 130048, 2, 1000);
            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_BUY_PERSONAL_SHOP_ITEM_REQ, new KEGS_BUY_PERSONAL_SHOP_ITEM_REQ { ItemUid = listingUid, Quantity = 1 });
            var buyAck = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KPacketOK>(RegionProfile.Taiwan);
            Assert.Equal(NetError.NET_OK, buyAck.Ok);
            var buyNotEv = await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan);
            Assert.Equal(EventId.EGS_BUY_PERSONAL_SHOP_ITEM_COMPLETE_NOT, buyNotEv.EventId);
            var buyNot = buyNotEv.GetPayload<KEGS_BUY_PERSONAL_SHOP_ITEM_COMPLETE_NOT>(RegionProfile.Taiwan);
            Assert.Single(buyNot.InventorySlotInfo);

            // Apply Join Guild -> KPacketOK + persisted in GuildJoinApplies
            var (_, createdGuild, _) = game.Db.CreateGuild(RegionProfile.Taiwan, unitUid + 500, "TestGuild", "Welcome");
            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_APPLY_JOIN_GUILD_REQ, new KEGS_APPLY_JOIN_GUILD_REQ
                {
                    GuildUid = createdGuild!.GuildUid,
                    Message = "Let me join!",
                    OldApplyDelete = true,
                });
            Assert.Equal(NetError.NET_OK, (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KPacketOK>(RegionProfile.Taiwan).Ok);
            Assert.Single(game.Db.GetGuildApplies(createdGuild.GuildUid));

            // Coupons: List, Use Item Coupon, Discount Inquiry
            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_COUPON_LIST_REQ, new KEGS_COUPON_LIST_REQ { Used = false, CouponBoxType = 0, PageNum = 1 });
            var couponListAck = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KEGS_COUPON_LIST_ACK>(RegionProfile.Taiwan);
            Assert.Equal(2, couponListAck.CouponCount);

            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_USE_ITEM_COUPON_REQ, new KEGS_USE_ITEM_COUPON_REQ { CouponCardNo = itemCardNo });
            Assert.Equal(NetError.NET_OK, (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KPacketOK>(RegionProfile.Taiwan).Ok);

            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_DISCOUNT_COUPON_INQUIRY_REQ, new KDiscountCouponInquriyInfo { ProductNo = 77777, OrderQuantity = 1 });
            var discAck = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KEGS_DISCOUNT_COUPON_INQUIRY_ACK>(RegionProfile.Taiwan);
            Assert.Single(discAck.DiscountCoupon);

            // BattleField NPC load outside BattleField returns ERR_BATTLEFIELD_16; inside BattleField returns NET_OK
            await SendNoParamAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_BATTLE_FIELD_NPC_LOAD_COMPLETE_REQ);
            Assert.Equal(NetError.ERR_BATTLEFIELD_16, (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KPacketOK>(RegionProfile.Taiwan).Ok);

            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_JOIN_BATTLE_FIELD_REQ, new KEGS_JOIN_BATTLE_FIELD_REQ
                {
                    BattleFieldJoinInfo = new KBattleFieldJoinInfo { BattleFieldId = 40001 },
                });
            Assert.Equal(EventId.EGS_JOIN_BATTLE_FIELD_ACK, (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).EventId);

            await SendNoParamAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_BATTLE_FIELD_NPC_LOAD_COMPLETE_REQ);
            Assert.Equal(NetError.NET_OK, (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KPacketOK>(RegionProfile.Taiwan).Ok);

            // Unit die complete broadcasts KEGS_USER_UNIT_DIE_COMPLETE_NOT
            await SendNoParamAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_USER_UNIT_DIE_COMPLETE_REQ);
            Assert.Equal(NetError.NET_OK, (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KPacketOK>(RegionProfile.Taiwan).Ok);
            var dieEv = await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan);
            Assert.Equal(EventId.EGS_USER_UNIT_DIE_COMPLETE_NOT, dieEv.EventId);
            Assert.Equal(unitUid, dieEv.GetPayload<KEGS_USER_UNIT_DIE_COMPLETE_NOT>(RegionProfile.Taiwan).UnitUid);

            // Live GameEventManager timed event completion
            game.GameEventManager.RegisterEvent(new KGameEventManager.TimedGameEvent
            {
                EventUid = 9901,
                EventName = "LiveConnect1Min",
                StartUtc = DateTime.UtcNow.AddMinutes(-10),
                EndUtc = DateTime.UtcNow.AddMinutes(10),
                ConnectMinutesRequired = 1,
                RewardItemId = 130048,
            });
            var completed = game.GameEventManager.TickUserEvents(unitUid, 65.0);
            Assert.Single(completed);

            // Map requirement fallback helper matches CXSLMapData::GetPossibleEnterVillageMapID
            game.MapDatas[20000] = new KGameServer.MapDataEntry(20000, 0, 20001, true, 1, 99, []);
            int fallback = game.GetPossibleEnterVillageMapId(5, new Dictionary<int, KDungeonClearInfo>());
            Assert.Equal(20000, fallback);

            // Dungeon clear persistence unlocks map requirement & reloads via GetUnitByUid
            game.Db.RecordDungeonClear(unitUid, 30010, 95000, 1);
            var reloaded = game.Db.GetUnitByUid(RegionProfile.Taiwan, unitUid);
            Assert.NotNull(reloaded);
            Assert.True(reloaded!.DungeonClear.ContainsKey(30010));
            Assert.Equal(NetError.NET_OK, game.CheckMapRequireData(20004, 20, reloaded.DungeonClear));

            // Billing product page & ContentManager toggle
            game.ContentManager.Init(enableCashShop: false);
            var closedPage = game.GetBillProductPage(game.ServerGroupId, 1, out bool closedFinal);
            Assert.True(closedFinal);
            game.ContentManager.Init(enableCashShop: true);
            var page1 = game.GetBillProductPage(game.ServerGroupId, 1, out bool pageFinal);
            Assert.True(pageFinal || page1.Count > 0);

            // Quest persistence roundtrip (Accept -> GetUnitQuests -> GiveUp -> Complete)
            game.Db.AcceptQuest(unitUid, 7701);
            game.Db.AcceptQuest(unitUid, 7702);
            game.Db.GiveUpQuest(unitUid, 7701);
            game.Db.CompleteQuest(unitUid, 7702);
            var (activeQs, completedQs) = game.Db.GetUnitQuests(unitUid);
            Assert.DoesNotContain(activeQs, q => q.Id == 7701);
            Assert.Contains(completedQs, c => c.questId == 7702);

            // TW macro parity check
            Assert.False(RegionProfile.Taiwan.AccountBlockMessageRenewal);

            // CommonFlag & FieldBossManager lifecycle (BossFieldOpenProcess, cooldown, auto-threshold, Tick expiration)
            game.SimLayer.AddCommonFlag(0x01u);
            Assert.True(game.SimLayer.CheckCommonFlag(0x01u));
            game.SimLayer.DeleteCommonFlag(0x01u);
            Assert.False(game.SimLayer.CheckCommonFlag(0x01u));

            long t0 = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            Assert.Equal(NetError.NET_OK, global.FieldBossManager.BossFieldOpenProcess(40001, t0));
            Assert.True(global.FieldBossManager.IsPortalOpen);
            Assert.True(global.FieldBossManager.IsCoolTime);
            Assert.Equal(NetError.ERR_FIELD_BOSS_02, global.FieldBossManager.BossFieldOpenProcess(40001, t0 + 5));
            Assert.True(global.FieldBossManager.Tick(t0 + (global.FieldBossManager.PortalHoldingTimeMinutes * 60L) + 1));
            Assert.False(global.FieldBossManager.IsPortalOpen);
            Assert.True(global.FieldBossManager.Tick(t0 + (global.FieldBossManager.FieldBossCoolTimeMinutes * 60L) + 120));
            Assert.False(global.FieldBossManager.IsCoolTime);
            Assert.Equal(NetError.NET_OK, global.FieldBossManager.TotalDangerousValueUpdateProcess(40001, 255, t0 + 4000));
            Assert.True(global.FieldBossManager.IsPortalOpen);

            // SQLite persistence: Account CashBalance, Inventory sockets/attrib/seal, SkillPage state, UserEventProgress, TC/DungeonPlay, HenirRankings
            uint initCash = game.GetAccountCash(accountUid);
            Assert.Equal(10_000u, initCash);
            Assert.True(game.TrySpendAccountCash(accountUid, 1500u, out uint remCash));
            Assert.Equal(8500u, remCash);
            Assert.Equal(8500u, game.Db.GetAccountCashBalance(accountUid));
            game.AddAccountCash(accountUid, 500u);
            Assert.Equal(9000u, game.Db.GetAccountCashBalance(accountUid));

            game.Db.SaveUnitSkillPageState(unitUid, 2, 2, new Dictionary<int, int> { [1] = 301 });
            var (spCount, spActive, spNotes) = game.Db.GetUnitSkillPageState(unitUid);
            Assert.Equal(2, spCount);
            Assert.Equal(2, spActive);
            Assert.Equal(301, spNotes[1]);

            game.Db.SaveUserEventState(unitUid, 45, 3, t0, 2, 10, 4, [10, 20, 30, 0, 0, 0], "2014-03-01");
            var evState = game.Db.GetUserEventState(unitUid);
            Assert.Equal(45, evState.oldYearRewardedLevel);
            Assert.Equal(3, evState.newYearStepId);
            Assert.Equal(20, evState.chinaSpirit[1]);

            game.Db.RecordTcClear(unitUid, 1);
            game.Db.RecordDungeonPlay(unitUid, 30010, cleared: false);
            game.Db.RecordDungeonPlay(unitUid, 30010, cleared: true);
            var reloadedAll = game.Db.GetUnitByUid(RegionProfile.Taiwan, unitUid);
            Assert.NotNull(reloadedAll);
            Assert.True(reloadedAll!.TcClear.ContainsKey(1));
            Assert.Equal(1, reloadedAll.DungeonPlay[30010].PlayTimes);
            Assert.Equal(1, reloadedAll.DungeonPlay[30010].ClearTimes);

            game.Db.UpsertHenirRanking(0, unitUid, "Phase16Hero", 1, 65, 50, 420, DateTime.UtcNow);
            Assert.Contains(game.Db.GetHenirRankings(), h => h.unitUid == unitUid && h.stageCount == 50);

            // ResultData_new.lua DUNGEON_TOTAL_RANK_RESULT parity
            Assert.Equal(KResultProcess.RankType.RT_SS, KResultProcess.TotalRankSumToRank(8, 8, 7, 6));
            Assert.Equal(KResultProcess.RankType.RT_S, KResultProcess.TotalRankSumToRank(7, 7, 7, 6));
            Assert.Equal(KResultProcess.RankType.RT_A, KResultProcess.TotalRankSumToRank(5, 5, 5, 5));

            // EGB_GET_AUTO_PARTY_BONUS_INFO_REQ -> EGB_GET_AUTO_PARTY_BONUS_INFO_ACK roundtrip
            global.AutoPartyManager.SetAutoPartyBonus(0, 30010, true);
            router.SendNoParam(PerformerId.PI_GS_SERVER, game.Uid, PerformerId.PI_GB_SERVER, global.Uid,
                EventId.EGB_GET_AUTO_PARTY_BONUS_INFO_REQ, trace: [game.Uid]);
            await Task.Delay(120);
            Assert.Contains(game.LatestAutoPartyBonusInfos, b => b.DungeonIdWithDif == 30010 && b.ActivatedBonus);
        }
        finally
        {
            game.ShutDown();
            login.ShutDown();
            global.ShutDown();
            center.ShutDown();
        }
    }

    private static T RoundTrip<T>(T value, RegionProfile region) where T : IKSerializable, new()
    {
        var buf = new KSerBuffer();
        var ks = new KSerializer(region);
        ks.BeginWriting(buf);
        Assert.True(value.Put(ks, region));
        ks.EndWriting();

        var decoded = new T();
        ks.BeginReading(buf);
        Assert.True(decoded.Get(ks, region));
        ks.EndReading();
        return decoded;
    }

    private static async Task<ushort> HandshakeAsync(NetworkStream stream, KSADatabase saDb, RegionProfile region)
    {
        var acceptEv = await ReadEventAsync(stream, saDb, region);
        Assert.Equal(EventId.E_ACCEPT_CONNECTION_NOT, acceptEv.EventId);
        var ks = new KSerializer(region);
        ks.BeginReading(acceptEv.Buffer);
        Assert.True(ks.Get(out ushort newSpi));
        var newSa = new KSecurityAssociation();
        Assert.True(newSa.Get(ks, region));
        ks.EndReading();
        saDb.Insert(newSpi, newSa);
        return newSpi;
    }

    private static async Task SendPacketAsync(NetworkStream stream, KSADatabase saDb, ushort spi, RegionProfile region, uint destPi, EventId id, IKSerializable payload)
    {
        var ev = new KEvent();
        ev.DestPerformer.PerformerId = destPi;
        ev.EventId = id;
        var ks = new KSerializer(region);
        ks.BeginWriting(ev.Buffer);
        Assert.True(payload.Put(ks, region));
        ks.EndWriting();
        var wire = new KSerBuffer();
        ks.BeginWriting(wire);
        ev.Put(ks, region);
        ks.EndWriting();
        var sec = new KSecureBuffer(saDb, spi);
        Assert.True(sec.Create(wire.AsSpan()));
        ushort totalLen = (ushort)(sec.WireBytes.Length + 2);
        byte[] frame = new byte[totalLen];
        BinaryPrimitives.WriteUInt16LittleEndian(frame.AsSpan(0, 2), totalLen);
        sec.WireBytes.CopyTo(frame.AsSpan(2));
        await stream.WriteAsync(frame);
    }

    private static async Task SendNoParamAsync(NetworkStream stream, KSADatabase saDb, ushort spi, RegionProfile region, uint destPi, EventId id)
    {
        var ev = new KEvent();
        ev.DestPerformer.PerformerId = destPi;
        ev.EventId = id;
        var ks = new KSerializer(region);
        ks.BeginWriting(ev.Buffer);
        ks.Put((sbyte)0);
        ks.EndWriting();
        var wire = new KSerBuffer();
        ks.BeginWriting(wire);
        ev.Put(ks, region);
        ks.EndWriting();
        var sec = new KSecureBuffer(saDb, spi);
        Assert.True(sec.Create(wire.AsSpan()));
        ushort totalLen = (ushort)(sec.WireBytes.Length + 2);
        byte[] frame = new byte[totalLen];
        BinaryPrimitives.WriteUInt16LittleEndian(frame.AsSpan(0, 2), totalLen);
        sec.WireBytes.CopyTo(frame.AsSpan(2));
        await stream.WriteAsync(frame);
    }

    private static async Task<KEvent> ReadEventAsync(NetworkStream stream, KSADatabase saDb, RegionProfile region)
    {
        byte[] lenBuf = new byte[2];
        int o = 0;
        while (o < 2)
        {
            int n = await stream.ReadAsync(lenBuf.AsMemory(o, 2 - o));
            if (n == 0) throw new EndOfStreamException();
            o += n;
        }
        ushort totalLen = BinaryPrimitives.ReadUInt16LittleEndian(lenBuf);
        byte[] payload = new byte[totalLen - 2];
        o = 0;
        while (o < payload.Length)
        {
            int n = await stream.ReadAsync(payload.AsMemory(o, payload.Length - o));
            if (n == 0) throw new EndOfStreamException();
            o += n;
        }
        var sec = new KSecureBuffer(saDb);
        Assert.True(sec.Parse(payload));
        var ev = new KEvent();
        var ks = new KSerializer(region);
        ks.BeginReading(new KSerBuffer(sec.PayloadBytes));
        Assert.True(ev.Get(ks, region));
        ks.EndReading();
        ev.Buffer.UnCompress();
        return ev;
    }

    [Fact]
    public void RegionProfile_FSMs_BillingConfig_And_BuffSync_Match_TW_Cpp_Parity()
    {
        // 1. TW SharingBankQuestCash matches ServerDefine_Global.h
        Assert.True(RegionProfile.Taiwan.SharingBankQuestCash);
        var invAck = new KEGS_MY_UNIT_AND_INVENTORY_INFO_LIST_ACK
        {
            Ok = NetError.NET_OK,
            UnitSlot = 8,
            SharingBank = true,
        };
        var buf = new KSerBuffer();
        var ks = new KSerializer(RegionProfile.Taiwan);
        ks.BeginWriting(buf);
        Assert.True(invAck.Put(ks, RegionProfile.Taiwan));
        ks.EndWriting();
        var bytes = buf.ToArray();
        var ksRead = new KSerializer(RegionProfile.Taiwan);
        ksRead.BeginReading(new KSerBuffer(bytes));
        var roundTrip = new KEGS_MY_UNIT_AND_INVENTORY_INFO_LIST_ACK();
        Assert.True(roundTrip.Get(ksRead, RegionProfile.Taiwan));
        ksRead.EndReading();
        Assert.True(roundTrip.SharingBank);

        // 2. KGSFSM, KMatchFSM, KAutoPartyFSM include full C++ state graphs
        var gsFsm = new KGSFSM();
        Assert.True(gsFsm.TryStateTransition(KGSFSM.I_TO_LOGINED, out _));
        Assert.True(gsFsm.TryStateTransition(KGSFSM.I_TO_SERVER_SELECT, out _));
        Assert.True(gsFsm.TryStateTransition(KGSFSM.I_TO_CHECK_SECOND_PW, out _));

        var matchFsm = new KMatchFSM();
        Assert.True(matchFsm.TryStateTransition(KMatchFSM.I_TO_MATCH_MAKING, out _));
        Assert.True(matchFsm.TryStateTransition(KMatchFSM.I_TO_CHECK_ACCEPT, out _));
        Assert.True(matchFsm.TryStateTransition(KMatchFSM.I_TO_PREPARE_FOR_PLAY, out _));
        Assert.True(matchFsm.TryStateTransition(KMatchFSM.I_TO_PLAY, out _));
        Assert.True(matchFsm.TryStateTransition(KMatchFSM.I_TO_CLOSE, out _));
        Assert.True(matchFsm.TryStateTransition(KMatchFSM.I_TO_INIT, out _));

        var apFsm = new KAutoPartyFSM();
        Assert.True(apFsm.TryStateTransition(KAutoPartyFSM.I_TO_AUTO_PARTY_MAKING, out _));
        Assert.True(apFsm.TryStateTransition(KAutoPartyFSM.I_TO_CHECK_ACCEPT, out _));
        Assert.True(apFsm.TryStateTransition(KAutoPartyFSM.I_TO_REGROUP_PARTY, out _));
        Assert.True(apFsm.TryStateTransition(KAutoPartyFSM.I_TO_PLAY, out _));
        Assert.True(apFsm.TryStateTransition(KAutoPartyFSM.I_TO_CLOSE, out _));
        Assert.True(apFsm.TryStateTransition(KAutoPartyFSM.I_TO_INIT, out _));

        // 3. BillingConfig.lua proxy & KUserServerBuffManager.ToPacketBuffInfos
        using var game = new KGameServer(RegionCode.TW, _dbPath);
        string? resDir = Directory.Exists(Path.Combine(AppContext.BaseDirectory, "../../../../../KncWX2Server/ServerResource"))
            ? Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../../KncWX2Server/ServerResource"))
            : null;
        if (resDir != null)
        {
            game.LoadServerResources(resDir, includeHeavyTables: false);
            Assert.Equal(790, game.BillingProductCategoryOverrides[206730]);
        }

        var buffMgr = new KUserServerBuffManager();
        buffMgr.ActivateFromTemplet(new KBuffManager.BuffTemplet { BuffId = 55001, BuffPeriodSecond = 60 }, expRate: 0.3f);
        var infos = buffMgr.ToPacketBuffInfos();
        Assert.Single(infos);
        Assert.Equal(55001, infos[0].FactorInfo.BuffIdentity.BuffTempletId);
    }
}
