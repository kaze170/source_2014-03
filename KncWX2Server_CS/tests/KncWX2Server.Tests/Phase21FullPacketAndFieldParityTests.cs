using System.Buffers.Binary;
using System.Net;
using System.Net.Sockets;
using KncWX2Server.CenterServer;
using KncWX2Server.ChannelServer.Managers;
using KncWX2Server.Core.Database;
using KncWX2Server.Core.Events;
using KncWX2Server.Core.Framework;
using KncWX2Server.Core.Network;
using KncWX2Server.Core.Packets;
using KncWX2Server.Core.Region;
using KncWX2Server.Core.Scripting;
using KncWX2Server.Core.Security;
using KncWX2Server.Core.Serialization;
using KncWX2Server.GameServer;
using KncWX2Server.GlobalServer;
using KncWX2Server.LoginServer;
using Xunit;

namespace KncWX2Server.Tests;

public sealed class Phase21FullPacketAndFieldParityTests : IDisposable
{
    private readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"elsword_p21_{Guid.NewGuid():N}.db");

    public void Dispose()
    {
        if (File.Exists(_dbPath))
        {
            try { File.Delete(_dbPath); } catch { }
        }
    }

    private static T RoundTrip<T>(T value, RegionProfile region) where T : IKSerializable, new()
    {
        var buf = new KSerBuffer();
        var writer = new KSerializer(region);
        writer.BeginWriting(buf);
        Assert.True(value.Put(writer, region));
        writer.EndWriting();

        var reader = new KSerializer(region);
        reader.BeginReading(buf);
        var decoded = new T();
        Assert.True(decoded.Get(reader, region));
        return decoded;
    }

    [Fact]
    public void Extended_Packet_DTOs_Serialize_And_Deserialize_Across_Regions()
    {
        var tw = RegionProfile.Taiwan;
        var cn = RegionProfile.FromCode(RegionCode.CN);

        var joinAck = RoundTrip(new KEGS_JOIN_ROOM_ACK
        {
            Ok = NetError.NET_OK,
            RoomInfo = new KRoomInfo { RoomUid = 1001, RoomName = "FieldParty" },
            CNIP = "127.0.0.1",
        }, tw);
        Assert.Equal(1001, joinAck.RoomInfo.RoomUid);
        Assert.Equal("127.0.0.1", joinAck.CNIP);

        var leaveAck = RoundTrip(new KEGS_LEAVE_ROOM_ACK
        {
            Ok = NetError.NET_OK,
            Reason = 2,
            ItemId = 130046,
            NotLeaveParty = true,
        }, tw);
        Assert.Equal(2, leaveAck.Reason);
        Assert.True(leaveAck.NotLeaveParty);

        var guildMsg = RoundTrip(new KEGS_GUILD_MESSAGE_NOT
        {
            MessageType = KEGS_GUILD_MESSAGE_NOT.MT_GUILD_LEVEL_UP,
            UnitUid = 5001,
            NickName = "ElswordTW",
            Message = "Guild leveled up!",
            MessageData = 10,
        }, tw);
        Assert.Equal(KEGS_GUILD_MESSAGE_NOT.MT_GUILD_LEVEL_UP, guildMsg.MessageType);
        Assert.Equal("ElswordTW", guildMsg.NickName);

        var comeBack = RoundTrip(new KEPM_CHANGE_COME_BACK_BUFF_NOT
        {
            IsBuffFinish = true,
            ComeBackBuffEnd = "2026-12-01 00:00:00",
        }, tw);
        Assert.True(comeBack.IsBuffFinish);

        var trInv = RoundTrip(new KETR_TOGGLE_INVISIBLE_NOT
        {
            Uid = [5001, 5002],
            UnitUid = 5001,
            FieldUserInfo = new KFieldUserInfo { UnitUid = 5001, Invisible = true },
        }, tw);
        Assert.True(trInv.FieldUserInfo.Invisible);
        Assert.Equal(2, trInv.Uid.Count);

        var trRide = RoundTrip(new KETR_SUMMON_RIDING_PET_NOT
        {
            Uid = [5002],
            UnitUid = 5001,
            RidingPetUid = 99001,
            RidingPetId = 12,
        }, tw);
        Assert.Equal((ushort)12, trRide.RidingPetId);

        // Region-gated CN field layout test on KEGIANT_INFO_USERONLINE_ACK
        var giantCn = RoundTrip(new KEGIANT_INFO_USERONLINE_ACK
        {
            UserName = "cn_player",
            GMName = "gm_cn",
            UserOnline = 1,
        }, cn);
        Assert.Equal("cn_player", giantCn.UserName);
        Assert.Equal((byte)1, giantCn.UserOnline);

        var giantTw = RoundTrip(new KEGIANT_INFO_USERONLINE_ACK
        {
            UserName = "ignored_in_tw",
            GMName = "ignored_in_tw",
            UserOnline = 1,
        }, tw);
        Assert.Equal(string.Empty, giantTw.UserName);
        Assert.Equal((byte)1, giantTw.UserOnline);
    }

    [Fact]
    public void FieldZone_Routes_Through_PI_GS_FIELD_And_Handles_Field_Handlers()
    {
        var router = new InProcessRouter();
        using var center = new KCenterServer(RegionCode.TW, router);
        using var global = new KGlobalServer(RegionCode.TW, router);
        using var game = new KGameServer(RegionCode.TW, _dbPath, router, center, global);
        center.Uid = 10;
        global.Uid = 20;
        game.Uid = 30;
        center.Init();
        global.Init();
        game.Init();
        game.Start(overridePort: 0, overrideTrPort: 0, overrideUdpEchoPort: 0, overrideUdpRelayPort: 0);

        var gsUserEvents = new List<KEvent>();
        router.RegisterRoute(PerformerId.PI_GS_USER, ev =>
        {
            gsUserEvents.Add(ev);
            return true;
        });

        const long mapId = 20001;
        router.Send(PerformerId.PI_GS_SERVER, game.Uid, PerformerId.PI_GS_FIELD, mapId,
            EventId.ERM_OPEN_FIELD_NOT, new KPacketOK());

        router.Send(PerformerId.PI_GS_USER, 3001, PerformerId.PI_GS_FIELD, mapId,
            EventId.ERM_JOIN_FIELD_REQ, new KERM_JOIN_FIELD_REQ
            {
                FieldUid = mapId,
                FieldUserInfo = new KFieldUserInfo
                {
                    UnitUid = 3001,
                    NickName = "AishaField",
                    Level = 65,
                    UnitClass = 2,
                    Pos = new KVector3 { X = 100, Y = 0, Z = 100 },
                },
            });
        router.Send(PerformerId.PI_GS_USER, 3002, PerformerId.PI_GS_FIELD, mapId,
            EventId.ERM_JOIN_FIELD_REQ, new KERM_JOIN_FIELD_REQ
            {
                FieldUid = mapId,
                FieldUserInfo = new KFieldUserInfo
                {
                    UnitUid = 3002,
                    NickName = "RenaField",
                    Level = 66,
                    UnitClass = 3,
                    Pos = new KVector3 { X = 150, Y = 0, Z = 100 },
                },
            });

        var zone = game.FieldManager.GetZone((int)mapId);
        Assert.NotNull(zone);
        Assert.Equal(KFieldFSM.S_WAIT, zone!.Fsm.StateId);
        Assert.Equal(2, zone.UserCount);
        Assert.Contains(gsUserEvents, e => e.EventId == EventId.ERM_JOIN_FIELD_ACK);

        // Chat + Field user list + Riding Pet + Invisible + Couple + Leave
        router.Send(PerformerId.PI_GS_USER, 3001, PerformerId.PI_GS_FIELD, mapId,
            EventId.ERM_CHAT_REQ, new KEGS_CHAT_REQ
            {
                ChatPacketType = 1,
                RoomChatType = 0,
                Message = "Hello Ruben!",
            });
        Assert.Contains(gsUserEvents, e => e.EventId == EventId.ERM_CHAT_ACK);

        router.Send(PerformerId.PI_GS_USER, 3001, PerformerId.PI_GS_FIELD, mapId,
            EventId.ERM_FIELD_USER_LIST_REQ, new KEGS_COMMUNITY_USER_LIST_REQ { ViewPage = 1 });
        var listAck = gsUserEvents.Last(e => e.EventId == EventId.ERM_FIELD_USER_LIST_ACK)
            .GetPayload<KEGS_COMMUNITY_USER_LIST_ACK>(RegionProfile.Taiwan);
        Assert.Equal(NetError.NET_OK, listAck.Ok);
        Assert.Equal(2, listAck.UserList.Count);

        router.Send(PerformerId.PI_GS_USER, 3001, PerformerId.PI_GS_FIELD, mapId,
            EventId.ERM_SUMMON_RIDING_PET_NOT, new KEGS_SUMMON_RIDING_PET_NOT
            {
                UnitUid = 3001,
                RidingPetUid = 777,
                RidingPetId = 5,
            });
        router.Send(PerformerId.PI_GS_USER, 3001, PerformerId.PI_GS_FIELD, mapId,
            EventId.ERM_TOGGLE_INVISIBLE_NOT, new KERM_TOGGLE_INVISIBLE_NOT
            {
                UnitUid = 3001,
                FieldUserInfo = new KFieldUserInfo { UnitUid = 3001, NickName = "AishaField", Invisible = true },
            });

        // Invisible GM hidden from non-monitoring community list
        router.Send(PerformerId.PI_GS_USER, 3002, PerformerId.PI_GS_FIELD, mapId,
            EventId.ERM_FIELD_USER_LIST_REQ, new KEGS_COMMUNITY_USER_LIST_REQ { ViewPage = 1 });
        var listAfterInv = gsUserEvents.Last(e => e.EventId == EventId.ERM_FIELD_USER_LIST_ACK)
            .GetPayload<KEGS_COMMUNITY_USER_LIST_ACK>(RegionProfile.Taiwan);
        Assert.Single(listAfterInv.UserList);

        router.Send(PerformerId.PI_GS_USER, 3001, PerformerId.PI_GS_FIELD, mapId,
            EventId.ERM_LEAVE_FIELD_REQ, new KERM_LEAVE_FIELD_REQ { Reason = 0, MapId = (int)mapId });
        Assert.Equal(1, zone.UserCount);
        Assert.Contains(gsUserEvents, e => e.EventId == EventId.ERM_LEAVE_FIELD_ACK);
    }

    [Fact]
    public void LoginServer_CenterServer_And_Managers_Cover_Expanded_Parity_Logic()
    {
        var router = new InProcessRouter();
        using var center = new KCenterServer(RegionCode.TW, router);
        using var login = new KLoginServer(RegionCode.TW, _dbPath, router);
        center.Uid = 100;
        login.Uid = 200;
        center.Init();
        login.Init();

        var gsUserEvents = new List<KEvent>();
        router.RegisterRoute(PerformerId.PI_GS_USER, ev =>
        {
            gsUserEvents.Add(ev);
            return true;
        });

        // CenterServer statistics DBLOG/LOCLOG counters
        router.Send(PerformerId.PI_GS_SERVER, 300, PerformerId.PI_CN_SERVER, center.Uid,
            EventId.ECN_SEND_STATISTICS_DBLOG_REQ, new KPacketOK());
        router.Send(PerformerId.PI_GS_SERVER, 300, PerformerId.PI_CN_SERVER, center.Uid,
            EventId.ECN_SEND_STATISTICS_LOCLOG_REQ, new KPacketOK());
        center.Tick();
        Assert.Equal(1, center.ServerLogCounters[EventId.ECN_SEND_STATISTICS_DBLOG_REQ]);
        Assert.Equal(1, center.ServerLogCounters[EventId.ECN_SEND_STATISTICS_LOCLOG_REQ]);

        // LoginServer EPUBLISHER_REG_USER_ACK -> KELG_REGISTER_USER_GLOBAL_PUBLISHER_ACK
        Assert.True(login.UserList.RegAuthWaitUser("garena_user_1", 9001));
        router.Send(PerformerId.PI_GS_SERVER, 300, PerformerId.PI_LOGIN_SERVER, login.Uid,
            EventId.EPUBLISHER_REG_USER_ACK, new KEPUBLISHER_REG_USER_ACK
            {
                Ok = NetError.NET_OK,
                ServiceAccountId = "garena_user_1",
                IsPcBang = true,
                PcBangType = 2,
                CheckState = 1,
            });
        login.Tick();
        var pubAck = gsUserEvents.Last(e => e.EventId == EventId.ELG_REGISTER_USER_GLOBAL_PUBLISHER_ACK)
            .GetPayload<KELG_REGISTER_USER_GLOBAL_PUBLISHER_ACK>(RegionProfile.Taiwan);
        Assert.Equal(NetError.NET_OK, pubAck.Ok);
        Assert.Equal("garena_user_1", pubAck.ServiceAccountId);
        Assert.True(pubAck.IsPcBang);

        // KDropTable expanded parity
        var dropTable = new KDropTable();
        dropTable.RegisterStaticDrop(30000, 130010, 100.0, 3);
        var (staticDropped, staticItemId, staticQty) = dropTable.RollStaticDrop(30000);
        Assert.True(staticDropped);
        Assert.Equal(130010, staticItemId);
        Assert.Equal(3, staticQty);

        dropTable.RegisterBattleFieldMonsterDrop(20001, 4001, 130001, 100.0, 2);
        var (bfDropped, bfItemId, _) = dropTable.RollBattleFieldDrop(20001, 4001);
        Assert.True(bfDropped);
        Assert.Equal(130001, bfItemId);

        dropTable.RegisterTimeDrop(new KDropTable.TimeDropItemInfo(1, 130099, "TimeDrop", DateTime.UtcNow.AddMinutes(-1), true));
        Assert.NotNull(dropTable.TryClaimTimeDrop());
        Assert.Null(dropTable.TryClaimTimeDrop());

        // KUserLetterBox expanded parity
        var box = new KUserLetterBox();
        box.Init("Elsword", [
            new KPostItemInfo
            {
                PostNo = 10,
                ScriptType = (sbyte)KUserLetterBox.LetterType.LtPostOffice,
                ScriptIndex = 130010,
                Quantity = 5,
                EnchantLevel = 7,
                Title = "Gift",
            },
            new KPostItemInfo
            {
                PostNo = 11,
                ScriptType = (sbyte)KUserLetterBox.LetterType.LtEvent,
                ScriptIndex = 999,
                Title = "EventReward",
            },
        ]);
        Assert.False(box.IsSystemLetter(10));
        Assert.True(box.IsSystemLetter(11));
        Assert.True(box.IsItemIdLetter(10));
        Assert.True(box.IsExistRewardInLetter(999));
        Assert.True(box.GetItemInfo(10, out var itemInfo));
        Assert.Equal(130010, itemInfo.ItemId);
        Assert.Equal(5, itemInfo.Quantity);

        // KUserAbuserManager + KGameEventManager expanded parity
        var db = new SqliteDatabaseManager(_dbPath);
        db.InitializeAndSeed();
        var abuser = new KUserAbuserManager();
        abuser.InitAccountAbuser(monitoringAbuser: true, tradeBlockUser: false);
        abuser.InitCharAbuser(itemAbuser: false, enchantAbuser: false, edAbuser: false, trafficAbuser: false, tradeBlockUnit: false);
        Assert.True(abuser.IsMonitoringAbuser);
        Assert.True(abuser.CheckItemAbuser(db, 1, 2, authLevel: 0, itemId: 130010, quantity: 250));
        Assert.True(abuser.IsItemAbuser);

        var evtMgr = new KGameEventManager();
        var now = DateTime.UtcNow;
        evtMgr.AddSpiritEvent(new KGameEventManager.EventPeriodRule(1, 0, now.AddHours(-1), now.AddHours(1)));
        evtMgr.AddMaxLevelEvent(new KGameEventManager.EventPeriodRule(2, 0, now.AddHours(-1), now.AddHours(1), Count: 75));
        evtMgr.AddHenirRewardCountEvent(new KGameEventManager.EventPeriodRule(3, 0, now.AddHours(-1), now.AddHours(1), Count: 10, Flag: true));
        Assert.True(evtMgr.CheckSpiritEvent(now));
        Assert.Equal(75, evtMgr.GetMaxLevel(70, now));
        Assert.True(evtMgr.GetHenirRewardCountEvent(out bool unlimited, out int maxCount, now));
        Assert.True(unlimited);
        Assert.Equal(10, maxCount);
    }

    [Fact]
    public void LoginServer_Billing_And_CrossServer_Handlers_Work()
    {
        var router = new InProcessRouter();
        using var login = new KLoginServer(RegionCode.TW, _dbPath, router);
        login.Uid = 210;
        login.Init();

        var gsUserEvents = new List<KEvent>();
        router.RegisterRoute(PerformerId.PI_GS_USER, ev =>
        {
            gsUserEvents.Add(ev);
            return true;
        });

        router.Send(PerformerId.PI_GS_USER, 4001, PerformerId.PI_LOGIN_SERVER, login.Uid,
            EventId.ELG_VERIFY_GIANT_MATRIX_CARD_REQ, new KPacketOK());
        router.Send(PerformerId.PI_GS_USER, 4001, PerformerId.PI_LOGIN_SERVER, login.Uid,
            EventId.EBILL_GET_TRANS_FOR_EXCHANGE_REQ, new KPacketOK());
        router.Send(PerformerId.PI_GS_USER, 4001, PerformerId.PI_LOGIN_SERVER, login.Uid,
            EventId.EBILL_GN_CHANGE_GAME_CURRENCY_REQ, new KPacketOK());
        router.Send(PerformerId.PI_GS_USER, 4001, PerformerId.PI_LOGIN_SERVER, login.Uid,
            EventId.ELG_CASH_DIRECT_CHARGE_CN_REQ, new KPacketOK());
        login.Tick();

        Assert.Contains(gsUserEvents, e => e.EventId == EventId.ELG_VERIFY_GIANT_MATRIX_CARD_ACK);
        Assert.Contains(gsUserEvents, e => e.EventId == EventId.EBILL_GET_TRANS_FOR_EXCHANGE_ACK);
        Assert.Contains(gsUserEvents, e => e.EventId == EventId.EBILL_GN_CHANGE_GAME_CURRENCY_ACK);
        var chargeAck = gsUserEvents.Last(e => e.EventId == EventId.ELG_CASH_DIRECT_CHARGE_CN_ACK)
            .GetPayload<KEGS_CASH_DIRECT_CHARGE_CN_ACK>(RegionProfile.Taiwan);
        Assert.Equal(NetError.NET_OK, chargeAck.Ok);
    }

    [Fact]
    public void Billing_Mission_And_Monster_Managers_Work()
    {
        // KBillManager reverse pagination + coupon/package filtering + purchase numbers
        var bill = new KBillManager();
        bill.PageSize = 1;
        bill.SetBillProductInfo(new Dictionary<int, KBillProductInfo>
        {
            [101] = new() { ProductNo = 101, ProductId = 130001, CategoryNo = 11, Sale = true, Show = true, SalePrice = 100 },
            [102] = new() { ProductNo = 102, ProductId = 130002, CategoryNo = KBillManager.CategoryCoupon, Sale = true, Show = true, SalePrice = 200 },
            [103] = new() { ProductNo = 103, ProductId = 130003, CategoryNo = 11, Sale = true, Show = true, SalePrice = 300 },
        });
        bill.SetBillProductPage();
        Assert.True(bill.GetBillProductPage(1, out var page1, out bool isFinal1));
        Assert.False(isFinal1);
        Assert.Single(page1);
        Assert.Equal(103, page1[0].ProductNo);
        Assert.True(bill.GetBillProductPage(2, out var page2, out bool isFinal2));
        Assert.True(isFinal2);
        Assert.Equal(101, page2[0].ProductNo);
        Assert.Equal(1u, bill.GetNextPurchaseNo());
        Assert.True(bill.GetTotalPrice([new KNXBTPurchaseReqInfo { ProductNo = 101, OrderQuantity = 2 }], out int total) && total == 200);

        // KGSWorldMissionManager & KGSGlobalMissionManager
        var worldMission = new KGSWorldMissionManager();
        worldMission.UpdateMissionInfo(new KEGB_GET_MISSION_INFO_ACK
        {
            IsActive = true,
            StartedCrystalCount = 5,
            ProtectedCrystalCount = 18,
            RemainingSeconds = 300,
        }, isBegin: true);
        Assert.Equal(KGSWorldMissionManager.WbscStepThree, worldMission.GetWorldCrystalCount());
        var worldNot = worldMission.GetMissionInfo();
        Assert.True(worldNot.Active);
        Assert.Equal(18, worldNot.WorldCrystalCount);

        var globalMission = new KGSGlobalMissionManager();
        globalMission.UpdateGlobalMissionInfo(new KEGB_GET_GLOBAL_MISSION_INFO_ACK
        {
            Ok = NetError.NET_OK,
            MissionId = 7,
            CurrentCount = 50,
            TargetCount = 100,
            IsActive = true,
        }, isBegin: true);
        var globalNot = globalMission.GetGlobalMissionInfo();
        Assert.True(globalNot.GlobalMissionInfo.ContainsKey("7"));

        // KDungeonMonsterManager & KPvpMonsterManager
        var roomMonsters = new KRoomMonsterManager();
        var dungeonMonsters = new KDungeonMonsterManager(roomMonsters);
        int bossUid = roomMonsters.CreateMonster(9001, 60, KRoomMonsterManager.MonsterGrade.MG_BOSS_NPC, explicitNpcUid: 10);
        dungeonMonsters.InsertSubStageNpcList(1, 2,
        [
            new KNPCUnitReq { Uid = bossUid, NpcId = 9001, MonsterGrade = (sbyte)KRoomMonsterManager.MonsterGrade.MG_BOSS_NPC, Active = true },
        ]);
        Assert.False(dungeonMonsters.IsAllSubStageNpcDie(1, 2));
        Assert.True(roomMonsters.TryKillMonster(bossUid, out _));
        Assert.True(dungeonMonsters.IsAllSubStageNpcDie(1, 2));
        dungeonMonsters.IncreaseMonsterDieCount(KDungeonMonsterManager.MonsterTypeFactor.BossNpc);
        Assert.Equal(1, dungeonMonsters.BossDieCount);

        var pvpMonsters = new KPvpMonsterManager(roomMonsters);
        pvpMonsters.SetPvpNpcInfo(new Dictionary<long, KPvpNpcInfo> { [55] = new KPvpNpcInfo { NpcId = 8001 } });
        Assert.True(pvpMonsters.CreatePvpMonster(new KNPCUnitReq { NpcId = 8001, Level = 50 }, out int pvpUid));
        Assert.Equal(55, pvpUid);

        // LocalRankingRankUp + RecruitRecruiterInfo DTO round-trip + Recommend state + Endurance DB
        var rankUp = RoundTrip(new KELG_LOCAL_RANKING_RANK_UP_NOT
        {
            MyRank = 3,
            MainTabIndex = 1,
            SubTabIndex = 2,
            MyUnitInfo = new KLocalRankingUnitInfo { UnitUid = 7001, NickName = "Raven" },
        });
        Assert.Equal(3, rankUp.MyRank);

        var recMgr = new KUserRecommendManager();
        recMgr.SetRecruitUnitList([new KRecommendUserInfo { UnitUid = 8002, Level = 10, IsOnline = false }]);
        recMgr.ModifyRecruitRecruiterState(new KELG_RECRUIT_RECRUITER_INFO_NOT
        {
            SenderUnitUid = 7001,
            ReceiverUnitUid = 8002,
            Type = KRecommendUserInfo.RuitLevelUp,
            Data = 35,
        });
        Assert.Equal((byte)35, recMgr.GetRecruitUnitList()[0].Level);

        // KGSAutoPartyManager & KGSRankingManager & KGameServer billing products
        var gsAutoParty = new KGSAutoPartyManager();
        gsAutoParty.UpdateAutoPartyBonusInfo(0, 30010, true);
        Assert.True(gsAutoParty.IsAutoPartyBonus(0, 30010));
        Assert.Equal(1, gsAutoParty.UpdateTick);
        Assert.Single(gsAutoParty.SnapshotAll());

        var gsRanking = new KGSRankingManager();
        gsRanking.UpdateHenirRanking(
            new Dictionary<int, List<KHenirRankingInfo>>
            {
                [KGSRankingManager.RtDayRanking] =
                [
                    new KHenirRankingInfo { Rank = 1, UnitUid = 9001, NickName = "Els", StageCount = 40, PlayTime = 240 },
                ],
            },
            new Dictionary<int, uint> { [KGSRankingManager.RtDayRanking] = 100 });
        gsRanking.UpdateDungeonAndPvpRanking(new KELG_WEB_RANKING_REFRESH_NOT
        {
            Init = true,
            RankingRefreshType = KELG_WEB_RANKING_REFRESH_NOT.PVP_VECTOR,
            TotalSize = 1,
            PvpRanking = [new KPvpRankingInfo { Rank = 1, UnitUid = 9001, NickName = "Els", RPoint = 1800, Win = 12 }],
        });
        var rankAck = new KEGS_GET_RANKING_INFO_ACK();
        Assert.True(gsRanking.GetRankingInfo(new KEGS_GET_RANKING_INFO_REQ { RankingType = KGSRankingManager.RtDayRanking, ViewPage = 1 }, 9001, rankAck));
        Assert.Single(rankAck.HenirRankingPage);
        var pvpRankAck = new KEGS_GET_RANKING_INFO_ACK();
        Assert.True(gsRanking.GetRankingInfo(new KEGS_GET_RANKING_INFO_REQ { RankingType = KGSRankingManager.RtPvpRanking, ViewPage = 1 }, 9001, pvpRankAck));
        Assert.Single(pvpRankAck.PvpRankingPage);

        Assert.All(KGameServer.GetDefaultBillingProducts(), p => Assert.True(p.Sale));

        // SqliteDatabaseManager endurance decrease & repair
        var db = new SqliteDatabaseManager(_dbPath);
        db.InitializeAndSeed();
        var item = db.GrantUnitItem(9001, 1001, 1, slotCategory: 2);
        var dec = db.DecreaseEquippedItemEndurance(9001, enduranceDamage: 10);
        Assert.Single(dec);
        Assert.Equal(item.ItemUid, dec[0].ItemUid);
        Assert.True(dec[0].Endurance < 100);
        Assert.Equal(1, db.RepairItems(9001, [item.ItemUid]));

        // KGSFieldBossManager & KCnTradeRoom DB update parity
        var gsFieldBoss = new KGSFieldBossManager();
        Assert.True(gsFieldBoss.CheckTerm(100));
        Assert.False(gsFieldBoss.CheckTerm(120));
        gsFieldBoss.SetConnected(true);
        Assert.True(gsFieldBoss.IsConnected);
        gsFieldBoss.SetInfo(100, 600, 3600, 40001, 40999, true, true);
        Assert.True(gsFieldBoss.IsPortalOpen);
        Assert.True(gsFieldBoss.IsCoolTime);
        gsFieldBoss.ClosePortal();
        Assert.False(gsFieldBoss.IsPortalOpen);
        gsFieldBoss.SetCoolTimeState(false);
        Assert.False(gsFieldBoss.IsCoolTime);

        var tradeRoom = new KCnTradeRoom { TradeUid = 500 };
        tradeRoom.OpenTrade(new KCnTradeRoom.TradeParticipant { UnitUid = 101, GsUid = 1, CurrentEd = 1000 });
        Assert.Equal(NetError.NET_OK, tradeRoom.JoinTrade(new KCnTradeRoom.TradeParticipant { UnitUid = 102, GsUid = 1, CurrentEd = 1000 }));
        Assert.Equal(NetError.NET_OK, tradeRoom.OfferTrade(101).ok);
        Assert.Equal(NetError.NET_OK, tradeRoom.OfferTrade(102).ok);
        Assert.Equal(NetError.NET_OK, tradeRoom.ApproveTrade(101).ok);
        Assert.Equal(NetError.NET_OK, tradeRoom.ApproveTrade(102).ok);
        Assert.True(tradeRoom.ConfirmTrade(101).ok == NetError.NET_OK);
        Assert.True(tradeRoom.ConfirmTrade(102).bothConfirmed);
        Assert.True(tradeRoom.MarkDbUpdateReady(101, out bool allReady1));
        Assert.False(allReady1);
        Assert.True(tradeRoom.MarkDbUpdateReady(102, out bool allReady2));
        Assert.True(allReady2);
        Assert.Equal(KCnTradeRoom.TradeRoomState.Close, tradeRoom.State);

        // Wire round-trip verification for newly fixed packet DTOs
        Assert.Equal((sbyte)4, KPostItemInfo.LtEvent);
        var chargePointAck = RoundTrip(new KEPUBLISHER_CHARGE_POINT_ACK { Ok = NetError.NET_OK, ChargedCash = 500 });
        Assert.Equal(500, chargePointAck.ChargedCash);
        var chargePointNot = RoundTrip(new KEGS_CHARGE_POINT_NOT { Ok = NetError.NET_OK, ChargedCash = 500 });
        Assert.Equal(500, chargePointNot.ChargedCash);
        var feverNot = RoundTrip(new KEGS_CHANGE_PARTY_FEVER_STATE_NOT { IsStart = true, PartyFever = 100 });
        Assert.True(feverNot.IsStart);
        Assert.Equal(100, feverNot.PartyFever);
        var regroupAck = RoundTrip(new KEGB_GET_REGROUP_PARTY_INFO_ACK { AutoPartyUid = 777 });
        Assert.Equal(777, regroupAck.AutoPartyUid);
        var naverReq = RoundTrip(new KNAVER_SOAP_GET_TOKEN_REQ { NaverSn = 123456789L, GameCode = 101 });
        Assert.Equal(123456789L, naverReq.NaverSn);
        Assert.Equal(101, naverReq.GameCode);
        var naverAck = RoundTrip(new KNAVER_SOAP_GET_TOKEN_ACK { Ok = NetError.NET_OK, AccessToken = "tok-abc" });
        Assert.Equal("tok-abc", naverAck.AccessToken);
    }

    [Fact]
    public void LoginGuild_AgencyShop_And_CrossDtoTranscoding_Work()
    {
        var router = new InProcessRouter();
        using var login = new KLoginServer(RegionCode.TW, _dbPath, router);
        login.Start(overridePort: 0);

        var gsUserEvents = new List<KEvent>();
        router.RegisterRoute(PerformerId.PI_GS_USER, ev =>
        {
            gsUserEvents.Add(ev);
            return true;
        });

        // Create account & two units in SQLite
        var (_, acctUid, _, _, _) = login.Db.VerifyOrRegisterAccount("TW", "guildtest_acct", "pw", autoCreateOnMissing: true);
        var (_, master, _) = login.Db.CreateUnit(RegionProfile.Taiwan, acctUid, "MasterUnit", 1);
        var (_, member, _) = login.Db.CreateUnit(RegionProfile.Taiwan, acctUid, "MemberUnit", 2);
        Assert.NotNull(master);
        Assert.NotNull(member);

        var (created, guild, _) = login.GuildManager.CreateGuild(login.Db, RegionProfile.Taiwan, master!.UnitUid, "TestGuild", "Hello");
        Assert.True(created);
        Assert.NotNull(guild);
        var (joined, _, _) = login.GuildManager.JoinGuild(login.Db, RegionProfile.Taiwan, guild!.GuildUid, member!.UnitUid);
        Assert.True(joined);

        // Register online units in UserList so LoginServer can broadcast to them
        login.UserList.RegisterOnlineUnit(master.UnitUid, acctUid, 1, master.UnitUid, 1, master.NickName);
        login.UserList.RegisterOnlineUnit(member.UnitUid, acctUid + 1000, 1, member.UnitUid, 1, member.NickName);

        // 1. DBE_RESET_GUILD_SKILL_ACK & ELG_INSERT_GUILD_CASH_SKILL_POINT_NOT
        router.Send(PerformerId.PI_GS_USER, master.UnitUid, PerformerId.PI_LOGIN_SERVER, login.Uid,
            EventId.DBE_RESET_GUILD_SKILL_ACK, new KDBE_RESET_GUILD_SKILL_ACK
            {
                Ok = NetError.NET_OK,
                GuildUid = guild.GuildUid,
                GuildSkillId = 5001,
                GuildSkillLevel = 0,
                GuildSkillCSPoint = 0,
                GuildSPoint = 5,
                GuildCSPoint = 2,
            });
        router.Send(PerformerId.PI_GS_USER, master.UnitUid, PerformerId.PI_LOGIN_SERVER, login.Uid,
            EventId.ELG_INSERT_GUILD_CASH_SKILL_POINT_NOT, new KELG_INSERT_GUILD_CASH_SKILL_POINT_NOT
            {
                GuildUid = guild.GuildUid,
                UnitUid = master.UnitUid,
                GuildCSPoint = 3,
                Period = 30,
            });
        router.Send(PerformerId.PI_GS_USER, master.UnitUid, PerformerId.PI_LOGIN_SERVER, login.Uid,
            EventId.ELG_UPDATE_CHANGE_GUILD_MEMBER_MESSAGE_NOT, new KDBE_CHANGE_GUILD_MEMBER_MESSAGE_ACK
            {
                Ok = NetError.NET_OK,
                GuildUid = guild.GuildUid,
                TargetUnitUid = member.UnitUid,
                Message = "Ready!",
            });
        router.Send(PerformerId.PI_GS_USER, master.UnitUid, PerformerId.PI_LOGIN_SERVER, login.Uid,
            EventId.ELG_UPDATE_GUILD_MEMBER_INFO_NOT, new KELG_UPDATE_GUILD_MEMBER_INFO_NOT
            {
                GuildUid = guild.GuildUid,
                MemberInfo = new KGuildMemberInfo
                {
                    UnitUid = member.UnitUid,
                    NickName = member.NickName,
                    Level = 70,
                },
            });
        router.Send(PerformerId.PI_GS_USER, master.UnitUid, PerformerId.PI_LOGIN_SERVER, login.Uid,
            EventId.ELG_INVITE_GUILD_FIND_USER_ACK, new KELG_INVITE_GUILD_FIND_USER_ACK
            {
                Ok = NetError.NET_OK,
                EventId = (ushort)EventId.ELG_INVITE_GUILD_REQ,
                GuildUid = guild.GuildUid,
                SenderUnitUid = master.UnitUid,
                ReceiverUnitUid = member.UnitUid,
                ReceiverNickName = member.NickName,
            });
        login.Tick();

        var gEntry = login.GuildManager.GetGuildEntry(guild.GuildUid);
        Assert.NotNull(gEntry);
        Assert.Equal(5, gEntry!.SkillTree.GuildSPoint);
        Assert.Equal(5, gEntry.SkillTree.GuildCsPoint);
        Assert.Equal("Ready!", gEntry.MemberMessages[member.UnitUid]);
        Assert.Equal((byte)70, gEntry.MemberInfos[member.UnitUid].Level);
        Assert.Contains(gsUserEvents, e => e.EventId == EventId.ELG_RESET_GUILD_SKILL_NOT);
        Assert.Contains(gsUserEvents, e => e.EventId == EventId.ELG_INSERT_GUILD_CASH_SKILL_POINT_NOT);
        Assert.Contains(gsUserEvents, e => e.EventId == EventId.ELG_INVITE_GUILD_ACK);
        Assert.Contains(gsUserEvents, e => e.EventId == EventId.ELG_INVITE_GUILD_FIND_USER_REQ);

        // 2. Agency shop join / buy / sell + sold-out notifications / period extend / leave / zombie check
        gsUserEvents.Clear();
        var shop = login.PersonalShopAgencyManager.CreateAgencyShop(master.UnitUid, master.NickName, "AgentShop",
            [new KPersonalShopAgencyManager.AgencyShopItem { ItemUid = 7001, ItemId = 130001, Quantity = 1, PricePerUnit = 500 }],
            TimeSpan.FromHours(1));
        Assert.NotNull(shop);
        Assert.Same(shop, login.PersonalShopAgencyManager.GetShopByUid(shop!.ShopUid));

        router.Send(PerformerId.PI_GS_USER, member.UnitUid, PerformerId.PI_LOGIN_SERVER, login.Uid,
            EventId.ERM_JOIN_PERSONAL_SHOP_REQ, new KERM_JOIN_PERSONAL_SHOP_REQ { ShopUid = shop.ShopUid, UnitUid = member.UnitUid });
        router.Send(PerformerId.PI_GS_USER, member.UnitUid, PerformerId.PI_LOGIN_SERVER, login.Uid,
            EventId.ERM_BUY_PERSONAL_SHOP_ITEM_REQ, new KERM_BUY_PERSONAL_SHOP_ITEM_REQ
            {
                ShopUid = shop.ShopUid,
                BuyerUnitUid = member.UnitUid,
                ItemUid = 7001,
                Quantity = 1,
            });
        router.Send(PerformerId.PI_GS_USER, master.UnitUid, PerformerId.PI_LOGIN_SERVER, login.Uid,
            EventId.ERM_UPDATE_PERIOD_PSHOP_AGENCY_NOT, new KERM_UPDATE_PERIOD_PSHOP_AGENCY_NOT
            {
                HostUnitUid = master.UnitUid,
                AddMinutes = 60,
            });
        router.Send(PerformerId.PI_GS_USER, member.UnitUid, PerformerId.PI_LOGIN_SERVER, login.Uid,
            EventId.ERM_LEAVE_PERSONAL_SHOP_REQ, new KPacketOK());
        router.Send(PerformerId.PI_GS_USER, master.UnitUid, PerformerId.PI_LOGIN_SERVER, login.Uid,
            EventId.ERM_CHECK_ZOMBIE_USER_ACK, new KERM_CHECK_ZOMBIE_USER_ACK { ZombieCheckType = KERM_CHECK_ZOMBIE_USER_ACK.ZCT_CONNECT_ZOMBIE });
        login.Tick();

        Assert.Contains(gsUserEvents, e => e.EventId == EventId.ERM_JOIN_PERSONAL_SHOP_ACK);
        Assert.Contains(gsUserEvents, e => e.EventId == EventId.ERM_BUY_PERSONAL_SHOP_ITEM_ACK);
        var agencyMsgs = gsUserEvents.Where(e => e.EventId == EventId.ELG_PSHOP_AGENCY_MESSAGE_NOT)
            .Select(e => e.GetPayload<KEGS_PSHOP_AGENCY_MESSAGE_NOT>(RegionProfile.Taiwan))
            .ToList();
        Assert.Contains(agencyMsgs, m => m.MessageType == KEGS_PSHOP_AGENCY_MESSAGE_NOT.PAMT_SELL_ITEM);
        Assert.Contains(agencyMsgs, m => m.MessageType == KEGS_PSHOP_AGENCY_MESSAGE_NOT.PAMT_SOLD_OUT);
        Assert.False(shop.ActiveBuyerUnitUids.Contains(member.UnitUid));

        // 3. KEvent cross-DTO transcoding & KUserDungeonManager.IsHenirDungeon
        var ev = new KEvent();
        ev.SetInProcess(PerformerId.PI_GS_USER, null, EventId.ELG_MODIFY_REG_GUILD_AD_ACK,
            new KELG_MODIFY_REG_GUILD_AD_ACK { Ok = NetError.ERR_GUILD_05 });
        Assert.Equal(NetError.ERR_GUILD_05, ev.GetPayload<KPacketOK>(RegionProfile.Taiwan).Ok);
        Assert.True(KUserDungeonManager.IsHenirDungeon(KUserDungeonManager.HenirDungeonId));
        Assert.False(KUserDungeonManager.IsHenirDungeon(30000));

        // 4. KInventoryLocker.TryLock, KBadAttitudeManager.UpdateUserCheckInfo, and KGlobalServer block check
        var locker = new KInventoryLocker();
        Assert.True(locker.TryLock(out int lockOk));
        Assert.Equal(NetError.NET_OK, lockOk);
        Assert.False(locker.TryLock(out int lockBusy));
        Assert.Equal(NetError.ERR_INVENTORY_LOCK_00, lockBusy);
        locker.Unlock();

        var badMgr = new KBadAttitudeManager();
        badMgr.Init([1001, 1002]);
        badMgr.UpdateUserCheckInfo(1001, 0, 0, 0, 0);

        using var global = new KGlobalServer(RegionCode.TW, router);
        global.Uid = 20;
        global.Init();
        global.BlockListManager.SetTradeBlock(acctUid, true, "TestBlock");
        gsUserEvents.Clear();
        router.Send(PerformerId.PI_GS_USER, master.UnitUid, PerformerId.PI_GLOBAL_SERVER, global.Uid,
            EventId.EGB_CHECK_BLOCK_USER_NOT, new KEGB_CHECK_BLOCK_USER_NOT
            {
                BlockType = [0, 1],
                UserUid = acctUid,
                UnitUid = master.UnitUid,
                Ip = "127.0.0.1",
            });
        global.Tick();
        var blkEv = Assert.Single(gsUserEvents, e => e.EventId == EventId.EGB_UPDATE_BLOCK_RESULT_NOT);
        var blkRes = blkEv.GetPayload<KEGB_UPDATE_BLOCK_RESULT_NOT>(RegionProfile.Taiwan);
        Assert.True(blkRes.BlockResult[0]);

        // 5. KEventDelaySystemManager & KESR_SCRIPT_REFRESH_ORDER_NOT sentinel routing
        var delayMgr = new KEventDelaySystemManager();
        var cubeReq = new KEGS_OPEN_RANDOM_ITEM_REQ { ItemUid = 777, DelayedOk = false, NotifyGetItem = true };
        delayMgr.RegOpenRandomCubeEvent(master.UnitUid, cubeReq);
        Assert.True(delayMgr.IsRegRandomCubeEvent(master.UnitUid));
        var dispatched = new List<(long UnitUid, KEGS_OPEN_RANDOM_ITEM_REQ Req)>();
        int sent = delayMgr.Tick(true, 0.0, 10, (u, r) => dispatched.Add((u, r)));
        Assert.Equal(1, sent);
        Assert.Single(dispatched);
        Assert.True(dispatched[0].Req.DelayedOk);
        Assert.False(delayMgr.IsRegRandomCubeEvent(master.UnitUid));

        var gsServerEvents = new List<KEvent>();
        router.RegisterRoute(PerformerId.PI_GS_SERVER, e => { gsServerEvents.Add(e.Clone()); return true; });
        using var center = new KCenterServer(RegionCode.TW, router);
        center.Uid = 30;
        center.Init();
        router.Send(PerformerId.PI_CN_SERVER, center.Uid, PerformerId.PI_CN_SERVER, center.Uid,
            EventId.ESR_ORDER_TO_CENTER_SERVER_NOT, new KESR_SCRIPT_REFRESH_ORDER_NOT
            {
                OrderType = KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_ITEM_MANAGER,
            });
        center.Tick();
        var fwdRefresh = Assert.Single(gsServerEvents, e => e.EventId == EventId.ESR_ORDER_TO_GAME_SERVER_NOT);
        Assert.Equal(KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_ITEM_MANAGER,
            fwdRefresh.GetPayload<KESR_SCRIPT_REFRESH_ORDER_NOT>(RegionProfile.Taiwan).OrderType);

        // 6. Archuangel coin event letter DB check & packet round-trips
        Assert.False(login.Db.HasTodayEventLetter(master.UnitUid));
        login.Db.SendLetter(RegionProfile.Taiwan, master.UnitUid, master.NickName, master.UnitUid,
            "Angel Coin", "Event Reward", 834, 1, 0, KPostItemInfo.LtEvent);
        Assert.True(login.Db.HasTodayEventLetter(master.UnitUid));

        var coinAck = RoundTrip(new KEGS_CHECK_THE_ARCHUANGEL_S_COIN_EVENT_LETTER_ACK
        {
            Ok = NetError.NET_OK,
            ReceiveTheLetter = true,
        });
        Assert.Equal(NetError.NET_OK, coinAck.Ok);
        Assert.True(coinAck.ReceiveTheLetter);

        var whAck = RoundTrip(new KEGS_JOIN_WEDDING_HALL_ACK
        {
            Ok = NetError.NET_OK,
            WeddingInfo = new KWeddingHallInfo { WeddingUid = 12, RoomUid = 999 },
            RoomInfo = new KRoomInfo { RoomUid = 999, RoomType = 6 },
            UdpRelayIp = "127.0.0.1",
            UdpRelayPort = 9101,
        });
        Assert.Equal(12, whAck.WeddingInfo.WeddingUid);
        Assert.Equal(999, whAck.RoomInfo.RoomUid);
    }

    [Fact]
    public void LuaManagers_Pet_Resolve_Enchant_Drop_And_GameSysVal_BindAndLoadData()
    {
        var script = new KScriptManager();
        var petMgr = new KPetManager();
        var resolveMgr = new KResolveItemManager();
        var enchantMgr = new KEnchantItemManager();
        var dropTable = new KDropTable();
        var sysVal = new KGameSysValTable();

        petMgr.BindToLua(script, "g_pPetManager");
        resolveMgr.BindToLua(script, "g_pResolveItemManager");
        enchantMgr.BindToLua(script, "EnchantItemManager");
        dropTable.BindToLua(script, "DropTable");
        sysVal.BindToLua(script, "GameSysVal");

        script.Lua.DoString("""
            g_pPetManager:SetNewPetInfo(0, 3840, 10, 1, 2)
            g_pPetManager:AddPetCreateItemInfo(500000, PET_UNIT_ID["PUI_PETTE_PPORU"], -1)
            g_pPetManager:AddRandomPetCreateItemInfo(90002290, PET_UNIT_ID["PUI_HALLOWEEN_PUMPKIN_TANYA"], 100)
            g_pPetManager:AddTransformPetItemInfo(90002300, 90002290, PET_UNIT_ID["PUI_HALLOWEEN_PUMPKIN_ELDENA"], 10)
            g_pPetManager:AddPetCashFeedItemInfo(500030, 468)
            g_pPetManager:AddSpecialFeedItemID(500030)
            g_pPetManager:AddPetSatietyDecreaseFactor(3, 0.5)
            g_pPetManager:IncreaseSpecialFeedIntimacyRate(0.0201)
            g_pPetManager:DecreaseSpecialFeedIntimacyRate(0.0501)

            g_pResolveItemManager:AddResolveData(ITEM_GRADE["IG_UNIQUE"], 109950, { fSuccRate = 100, iRandomCnt = 1, iResultCnt = 2, iBeginLv = 0, iEndLv = 100 })

            EnchantItemManager:AddEventSupportMaterialInfo(160786, 0, 20)
            EnchantItemManager:AddEventRestoreItemInfo(160790, ITEM_TYPE["IT_NONE"], 0, 20)
            EnchantItemManager:SetEnchantRate(5, 1.25)
            EnchantItemManager:SetEnchantProbability_Event(5, { Up1 = 25, NoChange = 50, Down1 = 25, DownTo0 = 0, Break = 0 })
            EnchantItemManager:AddDropRandomEnchantInfo(3, 100)
            EnchantItemManager:AddCubeRandomEnchantInfo(6, 100)

            DropTable:AddAttribNpcSpecialDrop(DIFFICULTY_LEVEL["DL_NORMAL"], 77280, 6)
            DropTable:SetManyEDDropNpcEvent(true)
            DropTable:SetPVPFailRate(PVP_FAILRATE["PF_WINTEAM"], 15)
            DropTable:SetPVPItemChangeTime(5)

            GameSysVal:SetBlockPersonalShop(true)
            GameSysVal:SetMachineIDBlock(true)
            GameSysVal:SetComeBackReward(1, 30)
            GameSysVal:AddPcBangPremiumItem(UNIT_TYPE["UT_ELSWORD"], 131254)
            GameSysVal:SetLanBugOutCheckOption { bLanBugOutCheck = true, byteLanBugOutCheckMaxCount = 2, fLanBugOutCheckTime = 1.5, byteLanBugOutCheckCount = 8, fLanBugOutCheckTermMin = 1.0, fLanBugOutCheckTerm = 0.5, fLanBugOutCheckRepeatTerm = 0.5 }
            GameSysVal:SetLanBugOutVerifyOption { bLanBugOutVerify = true, byteLanBugOutVerifyMaxCount = 10, fLanBugOutVerifyTime = 1.0, byteLanBugOutVerifyCount = 10, fLanBugOutVerifyTermMin = 1.0, fLanBugOutVerifyTerm = 0.0, fLanBugOutVerifyRepeatTerm = 0.5 }
            """);

        Assert.Equal((short)3840, petMgr.NewPetSatiety);
        Assert.True(petMgr.TryGetPetByItemId(500000, out int petId, out short period));
        Assert.Equal(2, petId);
        Assert.Equal((short)-1, period);
        Assert.True(petMgr.TryGetPetByItemId(90002290, out int randPetId, out _));
        Assert.Equal(30012, randPetId);
        Assert.True(petMgr.TryGetCashFeedSatiety(500030, out short feedSat));
        Assert.Equal((short)468, feedSat);
        Assert.True(petMgr.IsSpecialFeedItemId(500030));
        Assert.Equal(0.5, petMgr.GetSatietyDecreaseFactor(3));

        Assert.Single(resolveMgr.ResolveDataRules);
        Assert.Equal(109950, resolveMgr.ResolveDataRules[0].ResultItemId);
        var (resolvedMap, _) = resolveMgr.ResolveItem(10, isWeapon: true, deterministicRoll: 0.05, itemGrade: 1);
        Assert.True(resolvedMap.TryGetValue(109950, out int resolvedStoneQty) && resolvedStoneQty >= 2);

        Assert.Single(enchantMgr.EventSupportMaterials);
        Assert.Single(enchantMgr.EventRestoreItems);
        Assert.Equal(1.25f, enchantMgr.GetInstanceEnchantStatMultiplier(5));
        Assert.Equal(3, enchantMgr.RollDropRandomEnchantLevel(0.1));
        Assert.Equal(6, enchantMgr.RollCubeRandomEnchantLevel(0.1));

        Assert.True(dropTable.ManyEdDropNpcEventEnabled);
        Assert.Equal(5, dropTable.PvpItemChangeTimeSec);
        Assert.Equal(6, dropTable.AttribNpcSpecialDrops[(0, 77280)]);
        Assert.Equal(15.0, dropTable.PvpFailRates[0]);

        Assert.True(sysVal.BlockPersonalShop);
        Assert.True(sysVal.MachineIdBlock);
        Assert.Equal(30, sysVal.ComeBackRewardDays[1]);
        Assert.Contains(131254, sysVal.GetPcBangPremiumItems(1));
        Assert.True(sysVal.LanBugOutCheckEnabled);
        Assert.True(sysVal.LanBugOutVerifyEnabled);
    }

    [Fact]
    public void PvpMatchResult_ResolveFashion_DungeonSysVal_And_ServerSN_Parity()
    {
        var script = new KScriptManager();
        KPvpMatchResultTable.BindToLua(script, "KPvpMatchResultTable");
        script.Lua.DoString("KPvpMatchResultTable:AddPvpExpRewardInfo(50, 10076)");

        // Exact GET_ARENA_POINT_PVP_RESULT: rating 1500 => floor(15) + 10 = 25; 1v1 team win (2 + 0) * 25 = 50
        Assert.Equal(50, KPvpMatchResultTable.GetAPointPvpResult(1500, isWin: true, killNum: 0, mdKillNum: 0, (int)KPvpMatchResultTable.MatchType.MT_TEAM_1_ON_1));
        Assert.Equal(500, KPvpMatchResultTable.GetAPointPvpResult(1500, isWin: true, killNum: 0, mdKillNum: 0, (int)KPvpMatchResultTable.MatchType.MT_TEAM_1_ON_1, KPvpMatchResultTable.PvpNpcTypeHero));
        Assert.Equal(-60, KPvpMatchResultTable.GetRPointPvpPenalty());
        Assert.Equal(-250, KPvpMatchResultTable.GetAPointPvpPenalty(1500, (int)KPvpMatchResultTable.MatchType.MT_TEAM_1_ON_1));

        // Exact GET_EXP_PVP_RESULT: Lv50 (10076) * (1.0 + 0.4) * 1.25 = 17633
        Assert.Equal(17633, KPvpMatchResultTable.GetPvpExpReward(50, isWin: true, killNum: 2, mdKillNum: 0, (int)KPvpMatchResultTable.MatchType.MT_TEAM_3_ON_3));

        // Fashion item resolve: 1000 / 200 = 5 ATI_UNKNOWN fragments
        Assert.True(KResolveItemManager.ResolveFashionItem(1000, out var fashionOut, out int fashionQty));
        Assert.Equal(5, fashionQty);
        Assert.Equal(5, fashionOut[KSocketItemManager.AtiUnknown]);

        // Dungeon manager honors AdditionalHenirRewardCount & IsSpiritFree/IsSecretDungeonFree from GameSysValTable
        var sysVal = new KGameSysValTable();
        var dgnMgr = new KUserDungeonManager();
        dgnMgr.Init(false, 0, 0, 0, 0, 0, isPcBang: true, pcBangType: 1, additionalHenirRewardCount: 4);
        Assert.Equal(4, dgnMgr.PossiblePremiumCount);
        dgnMgr.RegisterDungeonTemplate(new KUserDungeonManager.DungeonTemplate
        {
            DungeonId = 39000,
            MinLevel = 1,
            MaxLevel = 99,
            RequiredSpirit = 10,
            RequiredItemId = 99001,
            IsSecretDungeon = true,
        });
        var (canEnter, err) = dgnMgr.CanEnterDungeon(39000, 50, currentSpirit: 0, _ => false, sysVal);
        Assert.True(canEnter);
        Assert.Equal(NetError.NET_OK, err);

        // ServerSN empty/valid check
        Assert.True(KServerSNMaker.IsEmptyServerSN(new byte[12]));
        Assert.True(KServerSNMaker.MakeServerSN(123456789L, out byte[] snBytes));
        Assert.False(KServerSNMaker.IsEmptyServerSN(snBytes));
        Assert.True(KServerSNMaker.CheckServerSN(snBytes, out long decodedSn));
        Assert.Equal(123456789L, decodedSn);
    }

    [Fact]
    public void DungeonResultDataNew_AttractionEvent_ComeBackDb_And_MonitorLog_Parity()
    {
        // 1. ResultData_new.lua exact weights & rank thresholds
        Assert.Equal(9.0, KResultProcess.WeightByDungeonId(38100));
        Assert.Equal(1.5, KResultProcess.WeightByDungeonId(33030));
        Assert.Equal(3.0, KResultProcess.WeightByDungeonId(33051));
        Assert.Equal(1.0, KResultProcess.WeightByDungeonId(30000));

        var mm = new KRoomMonsterManager();
        for (int i = 0; i < 10; i++)
        {
            int uid = mm.CreateMonster(1, 10, KRoomMonsterManager.MonsterGrade.MG_NORMAL_NPC);
            Assert.True(mm.TryKillMonster(uid, out _));
        }
        int bossUid = mm.CreateMonster(2, 15, KRoomMonsterManager.MonsterGrade.MG_BOSS_NPC);
        Assert.True(mm.TryKillMonster(bossUid, out _)); // weightTime = 10*1 + 12 = 22, weightCombo = 10*30 + 360 = 660
        var (tw, cw, techW) = KResultProcess.ComputeMonsterWeights(mm);
        Assert.Equal(22.0, tw);
        Assert.Equal(660.0, cw);
        Assert.Equal(22.0, techW);

        var (tRank, tBonus) = KResultProcess.ComputeDungeonTimeResult(30000, mm, 66); // 66 / 22 = 3.0 <= 3.5 -> SS (+0.03)
        Assert.Equal(KResultProcess.RankType.RT_SS, tRank);
        Assert.Equal(0.03f, tBonus, 3);

        var (dRank, dBonus) = KResultProcess.ComputeDungeonDamageResult(1000, 200, 10000); // 8.0% <= 20 -> SS (+0.03)
        Assert.Equal(KResultProcess.RankType.RT_SS, dRank);
        Assert.Equal(0.03f, dBonus, 3);

        var (cRank, cBonus) = KResultProcess.ComputeDungeonComboResult(50000, 30000, mm, 1); // 50000/660 = 75.7 > 65 -> SS (+0.05)
        Assert.Equal(KResultProcess.RankType.RT_SS, cRank);
        Assert.Equal(0.05f, cBonus, 3);

        var (tcRank, tcBonus) = KResultProcess.ComputeDungeonTechnicalResult(20000, 8, 30000, mm, 1); // (20000*1.21)/22 = 1100 > 750 -> SS (+0.05)
        Assert.Equal(KResultProcess.RankType.RT_SS, tcRank);
        Assert.Equal(0.05f, tcBonus, 3);

        Assert.Equal(KResultProcess.RankType.RT_SS, KResultProcess.TotalRankSumToRank((int)cRank, (int)tcRank, (int)dRank, (int)tRank));

        // BuildDungeonResult caps rankBonusRate at 0.15f (0.03+0.03+0.05+0.05 = 0.16 -> 0.15)
        var room = new KCnRoom(1, 1, new KRoomInfo { RoomType = 1, DungeonId = 30000, MaxSlot = 4 }, "127.0.0.1", 9101);
        Assert.Equal(NetError.NET_OK, room.TryAddUser(new KRoomUserInfo { UnitUid = 1001, NickName = "Tester", Level = 30 }, asObserver: false, password: "").ok);
        for (int i = 0; i < 10; i++)
        {
            int uid = room.MonsterManager.CreateMonster(1, 10, KRoomMonsterManager.MonsterGrade.MG_NORMAL_NPC);
            Assert.True(room.MonsterManager.TryKillMonster(uid, out _));
        }
        int roomBossUid = room.MonsterManager.CreateMonster(2, 15, KRoomMonsterManager.MonsterGrade.MG_BOSS_NPC);
        Assert.True(room.MonsterManager.TryKillMonster(roomBossUid, out _));
        room.EndGame();
        room.SubmitDungeonPlayResult(1001, new KDungeonPlayResultInfo
        {
            Hp = 10000f,
            ComboScore = 50000,
            TechScore = 20000,
            UsingKindOfSkill = 8,
            TotalAttackedDamage = 1000,
            TotalRecoveryHp = 200,
            TotalScore = 99999,
        });
        var dgnResult = KResultProcess.BuildDungeonResult(room, 66, isWin: true);
        var uRes = Assert.Single(dgnResult.DungeonUnitInfo);
        Assert.Equal((sbyte)KResultProcess.RankType.RT_SS, uRes.TotalRank);
        int expPlusParty = uRes.Exp + uRes.PartyBonusExp;
        Assert.Equal((int)(expPlusParty * 0.3f), uRes.ClearBonusExp);
        Assert.Equal((int)(expPlusParty * 0.15f), uRes.RankBonusExp);

        // 2. AttractionItemTimeEvent mapping (GameSysVal.cpp:2437)
        var sysVal = new KGameSysValTable();
        var now = new DateTime(2026, 11, 20, 12, 0, 0, DateTimeKind.Utc);
        sysVal.AddAttractionTimeEvent(new KGameSysValTable.AttractionItemTimeEventInfo
        {
            SourceItemId = 160873,
            EventAttractionItemId = 160893,
            BeginUtc = now.AddHours(-1),
            EndUtc = now.AddHours(1),
        });
        Assert.Equal(160893, sysVal.ResolveAttractionItemTimeEvent(160873, now));
        Assert.Equal(160873, sysVal.ResolveAttractionItemTimeEvent(160873, now.AddHours(2)));

        // 3. ComeBack & Letter/Trade monitor DB persistence
        string dbPath = Path.Combine(Path.GetTempPath(), $"knc_cb_mon_{Guid.NewGuid():N}.db");
        try
        {
            var db = new SqliteDatabaseManager(dbPath);
            db.InitializeAndSeed();
            var (_, uid, _) = db.CreateAccount(RegionProfile.Taiwan, "cb_user_test", "pw123");
            db.SaveAccountComeBackReward(uid, 2, now.AddDays(15).ToString("O"));
            db.UpdateAccountLoginTimestamp(uid, now);
            var (lastLogin, step, endStr) = db.GetAccountComeBackInfo(uid);
            Assert.NotNull(lastLogin);
            Assert.Equal(2, step);
            Assert.False(string.IsNullOrEmpty(endStr));
            db.ClearAccountComeBackState(uid);
            var (_, clearedStep, clearedEnd) = db.GetAccountComeBackInfo(uid);
            Assert.Equal(0, clearedStep);
            Assert.Equal(string.Empty, clearedEnd);
            db.LogLetterAndTradeMonitoring(1001, 0, "2026-11-20 12:00:00");
        }
        finally
        {
            if (File.Exists(dbPath)) File.Delete(dbPath);
        }

        // 4. PC Bang & Event bonus rates in KResultProcess + monitoring time window + deserialize check
        sysVal.RegisterPcBangProfile(new KGameSysValTable.PcBangRewardProfile
        {
            PcBangType = 1,
            ExpRate = 0.50f,
            EdRate = 0.25f,
            VpRate = 0.30f,
            PetExpRate = 0.10f,
            IsUsingPcBangTitle = true,
        });
        Assert.True(sysVal.IsUsingPcBangTitle(1));

        var titleMgr = new KUserTitleManager();
        titleMgr.Init(100, usePcBangTitle: sysVal.IsUsingPcBangTitle(1));
        Assert.True(titleMgr.UsePcBangTitle);

        var gbRoom = new KCnRoom(2, 2, new KRoomInfo { RoomType = 1, DungeonId = 30000, MaxSlot = 4 }, "127.0.0.1", 9101)
        {
            GameSysValTable = sysVal,
        };
        var gbUser = new KRoomUserInfo
        {
            UnitUid = 2001,
            NickName = "PcBangHero",
            Level = 30,
            IsGameBang = true,
            PcBangType = 1,
        };
        gbUser.Pet.Add(new KPetInfo { PetUid = 1, PetId = 10 });
        gbUser.BonusRate[5] = 0.20f; // BT_GEV_EXP
        gbUser.BonusRate[7] = 0.50f; // BT_GEV_ED
        gbRoom.TryAddUser(gbUser, false, "");
        gbRoom.EndGame();
        var gbResult = gbRoom.FinishDungeonGame(isWin: true);
        var gbRes = Assert.Single(gbResult.DungeonUnitInfo);
        int gbExpPlusParty = gbRes.Exp + gbRes.PartyBonusExp;
        Assert.Equal((int)(gbExpPlusParty * 0.50f) + (int)(gbExpPlusParty * 0.10f), gbRes.GameBangBonusExp);
        Assert.Equal((int)(gbExpPlusParty * 0.20f), gbRes.EventExp);

        sysVal.MonitoringBlockStartHour = 23;
        sysVal.MonitoringBlockStartMinute = 0;
        sysVal.MonitoringBlockStartSecond = 0;
        sysVal.MonitoringBlockDurationSec = 7200; // 23:00 .. 01:00 (wraps midnight)
        Assert.True(sysVal.IsMonitoringLetterAndTradeBlockTime(new TimeSpan(23, 30, 0)));
        Assert.True(sysVal.IsMonitoringLetterAndTradeBlockTime(new TimeSpan(0, 30, 0)));
        Assert.False(sysVal.IsMonitoringLetterAndTradeBlockTime(new TimeSpan(12, 0, 0)));

        var hm = new KUserHackingManager();
        sysVal.DeserializeFailCheckCount = 2;
        hm.IncreaseDeserializeFailCount((ushort)EventId.EGS_DUNGEON_PLAY_INFO_TO_SERVER_NOT);
        hm.IncreaseDeserializeFailCount((ushort)EventId.EGS_DUNGEON_PLAY_INFO_TO_SERVER_NOT);
        hm.IncreaseDeserializeFailCount((ushort)EventId.EGS_DUNGEON_PLAY_INFO_TO_SERVER_NOT);
        Assert.True(hm.HackingCheckDeserializeFailCount(sysVal));
        Assert.False(hm.HackingCheckDeserializeFailCount(sysVal));

        // 5. CheckPingScore host migration + MaxPingScore guard + dungeon skip
        sysVal.CheckChangeHostTimeSec = 1.0f;
        sysVal.MaxPingScore = 300;
        var pvpRoom = new KCnRoom(3, 3, new KRoomInfo { RoomType = 0, MaxSlot = 4 }, "127.0.0.1", 9101)
        {
            GameSysValTable = sysVal,
        };
        pvpRoom.TryAddUser(new KRoomUserInfo { UnitUid = 3001, NickName = "HostSlow", Level = 20 }, false, "");
        pvpRoom.TryAddUser(new KRoomUserInfo { UnitUid = 3002, NickName = "FastClient", Level = 20 }, false, "");
        pvpRoom.StartGame();
        pvpRoom.UpdatePingScore(3001, forceHost: false, pingScore: 250);
        pvpRoom.UpdatePingScore(3002, forceHost: false, pingScore: 40);
        var (hostChanged, oldHostUid, newHostUid) = pvpRoom.CheckPingScore(DateTime.UtcNow.AddSeconds(5));
        Assert.True(hostChanged);
        Assert.Equal(3001L, oldHostUid);
        Assert.Equal(3002L, newHostUid);

        // 6. Quest known IDs + dungeon payment drop + Henir stage rewards
        var qm = new KUserQuestManager();
        qm.Init([], [(9001, DateTime.UtcNow)]);
        Assert.Contains(9001, qm.GetKnownQuestIds());
        var henirTable = new KHenirResultTable();
        var henirItems = henirTable.RollStageRewards(5, 0);
        Assert.NotEmpty(henirItems);
        Assert.True(KUserDungeonManager.IsHenirDungeon(KUserDungeonManager.HenirDungeonId));

        // 7. Defence Dungeon quick-slot pools, manufacture random sockets, special cards, guardian phases, & room template propagation
        var ddmTemplate = new KDefenceDungeonManager();
        ddmTemplate.DefenceGuardianMonsters.Add(991001);
        ddmTemplate.EnterLevelConditions[5] = 25.0;
        ddmTemplate.SocketOptionRandomGroups[1] = [(3720, 50.0), (3760, 50.0)];
        ddmTemplate.ManufactureSocketOptionGroups[78801] = [1];
        ddmTemplate.SpecialCardGroups[78801] = new Dictionary<int, (int, int, int)>
        {
            [3720] = (78876, 78877, 78878),
            [3760] = (78879, 78880, 78881),
        };
        ddmTemplate.QuickSlotItemGroups[1] = [(78840, 78841, 78842), (78843, 78844, 78845)];
        ddmTemplate.QuickSlotItemGroups[2] = [(78846, 78847, 78848)];
        ddmTemplate.QuickSlotItemGroups[3] = [(78852, 78853, 78854)];
        ddmTemplate.OldDefenceItemIds.Add(113500);

        Assert.True(ddmTemplate.IsManufactureDefenceDungeonItem(78801));
        var rolledSockets = new List<int>();
        Assert.True(ddmTemplate.InsertSocketOptionForDefenceDungeonItem(78801, rolledSockets));
        Assert.Single(rolledSockets);
        Assert.True(ddmTemplate.GetResultSpecialCardByDefenceDungeonItem(KDefenceDungeonManager.DdpPhase2, 78801, rolledSockets, out int cardId));
        Assert.True(cardId is 78877 or 78880);

        var qsItems = new List<int>();
        ddmTemplate.GetResultQuickSlotItem(KDefenceDungeonManager.DdpPhase1, qsItems);
        Assert.Equal(3, qsItems.Count);
        Assert.True(ddmTemplate.IsOldDefenceItemId(113500));

        var rmMgr = new KCnRoomManager
        {
            GameSysValTable = sysVal,
            DefenceDungeonManagerTemplate = ddmTemplate,
        };
        var ddRoom = rmMgr.CreateRoom(new KRoomInfo { RoomType = 2, DungeonId = 30080, MaxSlot = 4 });
        ddRoom.DefenceDungeonManager.IsOpenDefenceDungeon = true;
        Assert.True(ddRoom.DefenceDungeonManager.DecisionEnterDefenceDungeon(30080, 50, new Dictionary<long, int> { [1L] = 50 }, 1.0f, is100PercentEventOn: true));
        Assert.Equal(KDefenceDungeonManager.DdpPhase1, ddRoom.DefenceDungeonManager.DefenceDungeonPhase);
        Assert.False(ddRoom.DefenceDungeonManager.IncreaseDefenceDungeonPhase(12345)); // non-guardian ignored
        Assert.True(ddRoom.DefenceDungeonManager.IncreaseDefenceDungeonPhase(991001));
        Assert.Equal(KDefenceDungeonManager.DdpPhase2, ddRoom.DefenceDungeonManager.DefenceDungeonPhase);

        // 8. SubStageNpcDieCheck (HctCheckNpcDieTime = 14) + dead unit tracking
        Assert.Equal(14, KEGS_SERVER_CHECK_HACK_USER_REQ.HctCheckNpcDieTime);
        Assert.Equal(21, KEGS_SERVER_CHECK_HACK_USER_REQ.HctChangeServerSnSendMail);
        sysVal.SubStageNpcDieCheck = true;
        ddRoom.TryAddUser(new KRoomUserInfo { UnitUid = 7001, NickName = "Solo", Level = 50 }, false, "");
        ddRoom.StartGame();
        ddRoom.BeginStageLoad(1);
        ddRoom.BeginSubStageOpen(1);
        ddRoom.RecordSubStageActiveNpc(5);
        var t0 = DateTime.UtcNow;
        for (int i = 0; i < 4; i++)
            Assert.False(ddRoom.CheckSubStageNpcDieHack(true, t0.AddMilliseconds(i * 50)));
        Assert.True(ddRoom.CheckSubStageNpcDieHack(true, t0.AddMilliseconds(250)));

        ddRoom.SetUserDieState(7001, true);
        Assert.Contains(7001L, ddRoom.GetDeadUnitUids());
        ddRoom.SetUserDieState(7001, false);
        Assert.DoesNotContain(7001L, ddRoom.GetDeadUnitUids());

        // 9. Inventory RandomSocketIds, Period, & DeleteUnitCategoryItems DB round-trip
        string invDbPath = Path.Combine(Path.GetTempPath(), $"knc_inv_rt_{Guid.NewGuid():N}.db");
        try
        {
            var invDb = new SqliteDatabaseManager(invDbPath);
            invDb.InitializeAndSeed();
            var (_, invAcctUid, _) = invDb.CreateAccount(RegionProfile.Taiwan, "p21_inv_user", "pw123");
            var (_, invUnit, _) = invDb.CreateUnit(RegionProfile.Taiwan, invAcctUid, "P21InvUnit", 1);
            long invUid = invUnit!.UnitUid;
            var testInvItem = invDb.GrantUnitItem(invUid, 78801, 1);
            var updatedRandom = invDb.UpdateItemRandomSockets(invUid, testInvItem.ItemUid, [3720]);
            Assert.Contains(3720, updatedRandom.ItemInfo.RandomSockets);
            var updatedPeriod = invDb.UpdateItemPeriod(invUid, testInvItem.ItemUid, 30, "2026-12-20 00:00:00");
            Assert.Equal((short)30, updatedPeriod.ItemInfo.Period);
            Assert.Equal("2026-12-20 00:00:00", updatedPeriod.ItemInfo.ExpirationDate);
            invDb.GrantUnitItem(invUid, 70001, 1, slotCategory: 12);
            Assert.True(invDb.DeleteUnitCategoryItems(invUid, 12) >= 1);
        }
        finally
        {
            if (File.Exists(invDbPath)) File.Delete(invDbPath);
        }

        // 10. TrainingCenterTable, CnHackingCheckManager, DungeonEventManager, CheckSumManager, HCT/NetError parity
        var tcTable = new KTrainingCenterTable();
        var clearedTc = new Dictionary<int, KTCClearInfo>();
        Assert.True(tcTable.CheckIfEnter(clearedTc, 1, unitLevel: 1, unitClass: 0));
        Assert.False(tcTable.CheckIfEnter(clearedTc, 2, unitLevel: 5, unitClass: 0)); // requires BeforeId = 1
        clearedTc[1] = new KTCClearInfo { TcId = 1 };
        Assert.True(tcTable.CheckIfEnter(clearedTc, 2, unitLevel: 5, unitClass: 0));
        Assert.Equal(40002, tcTable.GetDungeonId(2));
        Assert.True(tcTable.TryGetReward(2, out int tcEd, out int tcExp));
        Assert.Equal(2000, tcEd);
        Assert.Equal(1200, tcExp);

        var hackMgr = new KCnHackingCheckManager();
        hackMgr.SetHenirStageMinimumClearTime(54, 480);
        Assert.True(hackMgr.CheckHenirClearTimeHackUser(0, 1L, 38100, 54, 300));
        Assert.False(hackMgr.CheckHenirClearTimeHackUser(3, 1L, 38100, 54, 300)); // GM immune
        Assert.True(hackMgr.CheckDungeonClearTimeAutoHackUser(0, 1L, 30020, 0, 1, 20, true, 1, 5));

        var dgnEventMgr = new KDungeonEventManager();
        dgnEventMgr.UpdateTimeDropEventMonsterInfo([new KDungeonEventManager.TimeDropEventMonsterInfo(1, 90010, DateTime.UtcNow.AddSeconds(-5), 35)]);
        var stagePkt = new KEGS_DUNGEON_STAGE_LOAD_NOT();
        stagePkt.NpcData[0] = new KNPCList();
        Assert.True(dgnEventMgr.CheckTimeDropEventMonster(stagePkt, out int evUid, out int evNpcId));
        Assert.Equal(1, evUid);
        Assert.Equal(90010, evNpcId);
        Assert.Single(stagePkt.NpcData[0].NpcList);

        var csMgr = new KCheckSumManager();
        csMgr.AddCheckSum("data036.kom", "ABC123");
        Assert.True(csMgr.GetIsValidCheckSum(new Dictionary<string, string> { ["data036.kom"] = "ABC123" }));
        Assert.False(csMgr.GetIsValidCheckSum(new Dictionary<string, string> { ["data036.kom"] = "BAD" }));

        Assert.Equal(6, KEGS_SERVER_CHECK_HACK_USER_REQ.HctDungeonClearResult);
        Assert.Equal(39, KEGS_SERVER_CHECK_HACK_USER_REQ.HctImpossibleSellItem);
        Assert.Equal(280, NetError.ERR_TC_01);
        Assert.Equal(281, NetError.ERR_TC_03);

        var chMgr = new KChannelingUserManager();
        chMgr.UpdateChannelingUserList(login: true, channelingCode: 1);
        Assert.Equal(1, chMgr.GetChannelingUserList()[1]);
        chMgr.UpdateChannelingUserList(login: false, channelingCode: 1, gameServerLoggedIn: true);
        Assert.Equal(0, chMgr.GetChannelingUserList()[1]);

        Assert.False(henirTable.IsClearTempInventoryStage(9999));
        Assert.False(henirTable.IsResurrectionStage(9999));
        Assert.False(henirTable.IsClearNotifyStage(9999));
    }

    [Fact]
    public void Phase21_BuiltInEnums_DailyGiftBox_And_SubTitleOpenMission_Parity()
    {
        var script = new KScriptManager();
        Assert.Equal(38100, (int)script.Lua.Globals.Get("DUNGEON_ID").Table.Get("DI_ELDER_HENIR_SPACE").Number);
        Assert.Equal(30300, (int)script.Lua.Globals.Get("DUNGEON_ID").Table.Get("DI_ALTERA_CORE_NORMAL").Number);
        Assert.Equal(8, (int)script.Lua.Globals.Get("TITLE_MISSION_CLEAR_TYPE").Table.Get("TMCT_DUNGEON_CLEAR_COUNT").Number);
        Assert.Equal(2, (int)script.Lua.Globals.Get("RANK_TYPE").Table.Get("RT_E").Number);
        Assert.Equal(7, (int)script.Lua.Globals.Get("RANK_TYPE").Table.Get("RT_S").Number);
        Assert.Equal(-1, (int)script.Lua.Globals.Get("DIFFICULTY_LEVEL").Table.Get("DL_NULL").Number);

        var dgb = new KDailyGiftBoxManager();
        dgb.AddPackageInfo(251760, 532);
        dgb.AddPackageInfo(251760, 533);
        Assert.True(dgb.IsDailyGiftBoxItemId(251760));
        Assert.True(dgb.IsDailyGiftBoxScriptId(532, out int itemId532));
        Assert.Equal(251760, itemId532);
        Assert.True(dgb.IsDailyGiftBoxScriptId(533, out int itemId533));
        Assert.Equal(251760, itemId533);
    }

    [Fact]
    public void Phase21_DropTable_EventBoss_EnchantEventStone_And_CashResolveAdd_Parity()
    {
        var script = new KScriptManager();
        var drops = new KDropTable();
        drops.BindToLua(script);
        script.DoString("""
            DropTable:AddToGroup(9001, 777101, 100)
            DropTable:AddMonsterDropInfo {
                DungeonID = 30000,
                MonsterID = 501,
                Exp = 10,
                ED = 50,
                EDProperty = 100,
                DropItemList = { },
                DropGroupList = { { 9001, 100 } },
            }
            DropTable:AddHenirMonsterDropInfo {
                HenirDungeonMode = 1,
                MonsterID = 502,
                Exp = 20,
                ED = 0,
                EDProperty = 0,
                DropItemList = { { 777102, 100 } },
                DropGroupList = { },
            }
            DropTable:AddExtraStageMonsterDropInfo {
                MonsterID = 503,
                Exp = 0,
                ED = 0,
                EDProperty = 0,
                DropItemList = { { 777103, 100 } },
                DropGroupList = { },
            }
            DropTable:AddBattleFieldNpcDropInfo {
                BattleFieldID = 40001,
                MonsterID = 504,
                Exp = 5,
                ED = 20,
                EDProperty = 100,
                DropItemList = { { 777104, 100 } },
                DropGroupList = { },
            }
            DropTable:AddStaticDropInfo {
                DungeonID = 30000,
                Enable = True,
                DropItemList = { { 777105, 100 } },
            }
            DropTable:AddBattleFieldStaticDropInfo {
                BattleFieldID = 40001,
                Enable = True,
                DropItemList = { { 777106, 100 } },
            }
        """);

        Assert.Equal((true, 777101, 1), drops.RollMonsterDrop(501, dungeonId: 30000));
        Assert.Equal((true, 777102, 1), drops.RollMonsterDrop(502, isHenir: true, henirMode: 1));
        Assert.Equal((true, 777103, 1), drops.RollExtraStageDrop(503));
        Assert.Equal((true, 777104, 1), drops.RollBattleFieldDrop(40001, 504));
        Assert.Equal((true, 777105, 1), drops.RollStaticDrop(30000));
        Assert.Equal((true, 777106, 1), drops.RollBattleFieldDrop(40001, 999999));

        KBattleFieldGameManager.BindToLua(script);
        script.DoString("""
            g_pBattleFieldManager:BattleFieldEventBossDropInfo {
                BattleFieldID = 40001,
                MonsterID = 4038,
                SetEventBossCommonBonusItemID = 132000,
                SetEventBossCommonBonusItemCount = 2,
                SetEventBossBonusItemID = 132001,
                SetEventBossBonusItemIDCount_High = 2,
                SetEventBossBonusItemIDCount_Low = 1,
            }
        """);
        Assert.True(KBattleFieldGameManager.GetBattleFieldEventBossDropInfo(40001, 4038, out var evBoss));
        Assert.NotNull(evBoss);
        Assert.Equal(132000, evBoss!.CommonBonusItemId);
        Assert.Equal(2, evBoss.BonusItemCountHigh);

        var enchant = new KEnchantItemManager();
        enchant.BindToLua(script);
        script.DoString("EnchantItemManager:AddEventEnchantStoneInfo(80000450, 1, 41, 50)");
        Assert.Contains(80000450, enchant.GetMatchingStoneIds(1, 45, isRare: false));

        using var game = new KGameServer(RegionCode.TW, _dbPath);
        KServerResourceLoader.BindGlobalProxies(game, script);
        script.DoString("""
            g_pCashItemManager:AddCashItemGroup(10, 210000222)
            g_pCashItemManager:AddCashItemResolveAdd(10, 210000223, 1, 100)
            g_pCashItemManager:AddCashItemResolveAdd(10, 210000224, 2, 100)
        """);
        Assert.True(game.TryResolveCashItemAdd(210000222, out var cashOut));
        Assert.Equal(1, cashOut[210000223]);
        Assert.Equal(2, cashOut[210000224]);
    }

    [Fact]
    public void Phase21_WireParity_BattleFieldDrops_And_GuildExpansion_Tests()
    {
        var db = new SqliteDatabaseManager(_dbPath);
        db.InitializeAndSeed();

        var (_, userUid, _) = db.CreateAccount(RegionProfile.Taiwan, "p21guilduser", "pw");
        var (_, unit, _) = db.CreateUnit(RegionProfile.Taiwan, userUid, "P21GuildHero", 1);
        Assert.NotNull(unit);
        var (gOk, guild, _) = db.CreateGuild(RegionProfile.Taiwan, unit!.UnitUid, "P21Guild", "hello");
        Assert.True(gOk);
        Assert.NotNull(guild);
        Assert.Equal((ushort)30, guild!.MaxNumMember);

        var (expOk, newMax) = db.ExpandGuildMaxMembers(RegionProfile.Taiwan, guild.GuildUid, 10, 200);
        Assert.True(expOk);
        Assert.Equal((ushort)40, newMax);
        Assert.Equal((ushort)40, db.GetGuildInfo(RegionProfile.Taiwan, guild.GuildUid)!.MaxNumMember);

        var sellAck = RoundTrip(new KERM_CHECK_SELL_ED_ITEM_ACK { Ok = NetError.NET_OK, ItemUid = 9001, Quantity = 3 });
        Assert.Equal(NetError.NET_OK, sellAck.Ok);
        Assert.Equal(9001L, sellAck.ItemUid);
        Assert.Equal(3, sellAck.Quantity);

        var connAck = RoundTrip(new KECN_VERIFY_SERVER_CONNECT_ACK { Ok = NetError.NET_OK, CenterUid = 42L });
        Assert.Equal(NetError.NET_OK, connAck.Ok);
        Assert.Equal(42L, connAck.CenterUid);

        var stopAck = RoundTrip(new KEGS_STOP_DUNGEON_CONTINUE_TIME_ACK { Ok = NetError.NET_OK, IsStop = true });
        Assert.True(stopAck.IsStop);

        var regShopAck = RoundTrip(new KEGS_REG_PSHOP_AGENCY_ITEM_ACK { Ok = NetError.ERR_PERSONAL_SHOP_01, Ed = 12345 });
        Assert.Equal(NetError.ERR_PERSONAL_SHOP_01, regShopAck.Ok);
        Assert.Equal(12345, regShopAck.Ed);

        var expandAck = RoundTrip(new KDBE_EXPAND_GUILD_MAX_MEMBER_ACK { Ok = NetError.NET_OK, GuildUid = guild.GuildUid, MaxNumMember = newMax });
        Assert.Equal(guild.GuildUid, expandAck.GuildUid);
        Assert.Equal((ushort)40, expandAck.MaxNumMember);
    }

    [Fact]
    public void Phase21_GlobalMatchAndAutoParty_SkillPage_LogMonitoring_Tests()
    {
        var skillReq = RoundTrip(new KDBE_EXPAND_SKILL_PAGE_REQ
        {
            UnitUid = 1001,
            SkillPageNumberToBeAdded = [2],
            SPoint = 50,
            CSPoint = 10,
            DefaultSkill = [101, 102, 103, 104, 105, 106],
            UnitClass = 1,
            EventId = 99,
        });
        Assert.Equal(new[] { 101, 102, 103, 104, 105, 106 }, skillReq.DefaultSkill);

        var skillAck = RoundTrip(new KDBE_EXPAND_SKILL_PAGE_ACK
        {
            Ok = NetError.NET_OK,
            UnitUid = 1001,
            SkillPageNumberToBeAdded = 2,
            SPoint = 50,
            CSPoint = 10,
            DefaultSkill = [201, 202, 203, 204, 205, 206],
            UnitClass = 1,
            EventId = 99,
        });
        Assert.Equal(new[] { 201, 202, 203, 204, 205, 206 }, skillAck.DefaultSkill);

        var regroup = RoundTrip(new KEPM_REGROUP_PARTY_FOR_AUTO_PARTY_DUNGEON_GAME_REQ
        {
            AutoPartyUid = 555,
            AutoPartyHostUnitUid = 101,
            CreatePartyInfo = new KCreatePartyInfo { PartyType = 0, DungeonId = 30100, DifficultyLevel = 2, DungeonMode = 1 },
            Members = [new KPartyUserInfo { UnitUid = 101, NickName = "Host" }],
        });
        Assert.Equal(30100, regroup.CreatePartyInfo.DungeonId);
        Assert.Equal((sbyte)2, regroup.CreatePartyInfo.DifficultyLevel);

        var router = new InProcessRouter();
        using var gb = new KGlobalServer(RegionCode.TW, router);
        gb.Start(serverGroupId: 0);

        var sentEvents = new List<KEvent>();
        router.RegisterRoute(PerformerId.PI_GS_USER, ev => sentEvents.Add(ev));

        router.Send(PerformerId.PI_GS_USER, 1, PerformerId.PI_GB_SERVER, gb.Uid,
            EventId.EGB_REQUEST_AUTO_PARTY_MAKING_REQ, new KEGB_REQUEST_AUTO_PARTY_MAKING_REQ
            {
                AutoPartyCondition = new KAutoPartyConditionInfo { DungeonId = 30100, DifficultyLevel = 1, DungeonMode = 0 },
                AutoPartyUser = { new KAutoPartyUserInfo { UnitUid = 1001, NickName = "P1", Level = 50 } },
            });
        router.Send(PerformerId.PI_GS_USER, 2, PerformerId.PI_GB_SERVER, gb.Uid,
            EventId.EGB_REQUEST_AUTO_PARTY_MAKING_REQ, new KEGB_REQUEST_AUTO_PARTY_MAKING_REQ
            {
                AutoPartyCondition = new KAutoPartyConditionInfo { DungeonId = 30100, DifficultyLevel = 1, DungeonMode = 0 },
                AutoPartyUser = { new KAutoPartyUserInfo { UnitUid = 1002, NickName = "P2", Level = 50 } },
            });
        gb.Tick();

        Assert.Equal(2, sentEvents.Count(e => e.EventId == EventId.EGB_AUTO_PARTY_MAKING_SUCCESS_NOT));
        sentEvents.Clear();

        router.Send(PerformerId.PI_GS_USER, 1, PerformerId.PI_GB_SERVER, gb.Uid,
            EventId.EGB_AUTO_PARTY_MAKING_SUCCESS_REPLY_NOT, new KEGB_AUTO_PARTY_MAKING_SUCCESS_REPLY_NOT { UnitUid = 1001, Accept = true });
        router.Send(PerformerId.PI_GS_USER, 2, PerformerId.PI_GB_SERVER, gb.Uid,
            EventId.EGB_AUTO_PARTY_MAKING_SUCCESS_REPLY_NOT, new KEGB_AUTO_PARTY_MAKING_SUCCESS_REPLY_NOT { UnitUid = 1002, Accept = true });
        gb.Tick();

        var regroupEv = Assert.Single(sentEvents, e => e.EventId == EventId.EGB_REGROUP_PARTY_FOR_AUTO_PARTY_DUNGEON_GAME_REQ);
        var regroupPayload = regroupEv.GetPayload<KEPM_REGROUP_PARTY_FOR_AUTO_PARTY_DUNGEON_GAME_REQ>(RegionProfile.Taiwan);
        Assert.Equal(30100, regroupPayload.CreatePartyInfo.DungeonId);
        Assert.Equal(2, regroupPayload.Members.Count);

        var dt = new KDropTable();
        var script = new KScriptManager();
        dt.BindToLua(script, "DropTable");
        script.DoString("DropTable:AddManyEDDropNpc(5501, 2.5, 4)");
        Assert.True(dt.TryGetManyEdDropMultiplier(5501, out float edMult, out float cntMult));
        Assert.Equal(2.5f, edMult);
        Assert.Equal(4.0f, cntMult);

        gb.ShutDown();
    }

    [Fact]
    public void Phase21_AuditParity_MatchUserAck_FeverTick_GuildSkillExpire_Tests()
    {
        var pshop = RoundTrip(new KSquarePersonalShopInfo
        {
            UpdateType = KSquarePersonalShopInfo.UtAll,
            PersonalShopState = 2,
            PersonalShopType = 1,
            PersonalShopUid = 90001,
            PersonalShopName = "RubenMarket",
        });
        Assert.Equal(KSquarePersonalShopInfo.UtAll, pshop.UpdateType);
        Assert.Equal((sbyte)2, pshop.PersonalShopState);
        Assert.Equal((sbyte)1, pshop.PersonalShopType);
        Assert.Equal(90001L, pshop.PersonalShopUid);
        Assert.Equal("RubenMarket", pshop.PersonalShopName);

        var partyMgr = new KCnPartyManager();
        var party = partyMgr.CreateParty(
            new KCreatePartyInfo { DungeonId = 30000 },
            new KPartyUserInfo { UnitUid = 7001, NickName = "Host", Host = true, Ready = true });
        party.AddFever(KCnParty.MaxFeverPoint);
        Assert.True(party.IsFeverTime);
        Assert.Empty(partyMgr.TickFeverExpirations(DateTime.UtcNow));
        var expiredFever = partyMgr.TickFeverExpirations(DateTime.UtcNow.AddMinutes(5));
        var feverItem = Assert.Single(expiredFever);
        Assert.Equal(party.Info.PartyUid, feverItem.PartyUid);
        Assert.False(party.IsFeverTime);

        var router = new InProcessRouter();
        var sentEvents = new List<KEvent>();
        router.RegisterRoute(PerformerId.PI_GS_USER, ev => sentEvents.Add(ev));
        using var gb = new KGlobalServer(RegionCode.TW, router) { Uid = 1 };
        gb.Init();

        router.Send(PerformerId.PI_GS_USER, 7001, PerformerId.PI_GB_SERVER, gb.Uid,
            EventId.EGB_GET_ROOM_USER_INFO_FOR_MATCH_ACK,
            new KEGB_GET_ROOM_USER_INFO_FOR_MATCH_ACK
            {
                MatchUid = 1,
                RoomUserInfo = new KRoomUserInfo { UnitUid = 7001, NickName = "P1", Level = 55, UnitClass = 10 },
            });
        router.Send(PerformerId.PI_GS_USER, 7001, PerformerId.PI_GB_SERVER, gb.Uid,
            EventId.EGB_REQUEST_MATCH_MAKING_REQ,
            new KEGB_REQUEST_MATCH_MAKING_REQ
            {
                PvpPlayerCount = 1,
                MatchUserInfo = { new KMatchUserInfo { UnitUid = 7001, NickName = "P1", Rating = 1200, CurrentRank = 1, OfficialMatchCnt = 10 } },
            });
        router.Send(PerformerId.PI_GS_USER, 7002, PerformerId.PI_GB_SERVER, gb.Uid,
            EventId.EGB_REQUEST_MATCH_MAKING_REQ,
            new KEGB_REQUEST_MATCH_MAKING_REQ
            {
                PvpPlayerCount = 1,
                MatchUserInfo = { new KMatchUserInfo { UnitUid = 7002, NickName = "P2", Rating = 1210, CurrentRank = 1, OfficialMatchCnt = 10 } },
            });
        gb.Tick();

        router.Send(PerformerId.PI_GS_USER, 7001, PerformerId.PI_GB_SERVER, gb.Uid,
            EventId.EGB_MATCH_MAKING_SUCCESS_REPLY_NOT,
            new KEGB_MATCH_MAKING_SUCCESS_REPLY_NOT { UnitUid = 7001, Accept = true });
        router.Send(PerformerId.PI_GS_USER, 7002, PerformerId.PI_GB_SERVER, gb.Uid,
            EventId.EGB_MATCH_MAKING_SUCCESS_REPLY_NOT,
            new KEGB_MATCH_MAKING_SUCCESS_REPLY_NOT { UnitUid = 7002, Accept = true });
        gb.Tick();

        var openMatchEv = Assert.Single(sentEvents, e => e.EventId == EventId.EGB_OPEN_PVP_ROOM_FOR_MATCH_REQ);
        var openMatch = openMatchEv.GetPayload<KEGB_OPEN_PVP_ROOM_FOR_MATCH_REQ>(RegionProfile.Taiwan);
        Assert.Contains(openMatch.RedTeam.Concat(openMatch.BlueTeam), u => u.UnitUid == 7001 && u.Level == 55);

        gb.ShutDown();
    }

    [Fact]
    public void Exp_Spirit_And_Reward_Tables_Bind_To_Lua_Globals_And_Positional_Calls()
    {
        var exp = new KExpTable(seedDefaultCurve: false);
        var spirit = new KSpiritTable();
        var reward = new KRewardTable();
        var script = new KScriptManager();

        exp.BindToLua(script);
        spirit.BindToLua(script);
        reward.BindToLua(script);

        Assert.True(script.DoString("""
            LEVEL = 1; NEED_EXP = 0; TOTAL_EXP = 0; g_pkExpTable:AddExpTable_LUA()
            LEVEL = 2; NEED_EXP = 500; TOTAL_EXP = 500; g_pkExpTable:AddExpTable_LUA()
            g_pkExpTable:AddExpTable_LUA(3, 1500, 2000)

            MAX_SPIRIT = 150; g_pkSpiritTable:SetMaxSpirit()
            LEVEL = 12; SPIRIT = 125; g_pkSpiritTable:AddSpiritTable_LUA()
            g_pkSpiritTable:AddSpiritTable_LUA(15, 140)

            g_pKRewardTable:AddRewardData(9001, 130048, 3, 7)
            g_pKRewardTable:AddLevelUpRewardData(25, 9001)
            g_pKRewardTable:SetOutUserRetainingRewardItemInfo(9, 130040, 2)
            """, "lua_tables_parity"));

        Assert.Equal(500, exp.GetRequireNeedExpByLevel(2));
        Assert.Equal(2000, exp.GetRequireTotalExpByLevel(3));
        Assert.Equal((byte)3, exp.CheckLevelUp(1, 2500));

        Assert.Equal(150, spirit.MaxSpirit);
        Assert.Equal(125, spirit.GetSpiritOrDefault(12));
        Assert.Equal(140, spirit.GetSpiritOrDefault(15));

        Assert.True(reward.GetRewardInfo(9001, out var items));
        Assert.Equal(3, items[130048]);
        Assert.Contains(9001, reward.GetLevelUpRewardIds(25));
        Assert.True(reward.TryGetOutUserRetainingReward(9, out int retItem, out int retQty));
        Assert.Equal(130040, retItem);
        Assert.Equal(2, retQty);
    }

    [Fact]
    public async Task ScriptRefreshManager_Dispatches_Lua_Orders_And_Executes_Selective_Reloads()
    {
        var router = new InProcessRouter();
        using var gs = new KGameServer(RegionCode.TW, _dbPath, router);
        Assert.True(gs.Start(overridePort: 0, overrideTrPort: 0, overrideUdpEchoPort: 0, overrideUdpRelayPort: 0));

        var receivedOrders = new List<int>();
        gs.ScriptRefreshManager.RegisterOrderHandler(KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_EXP_TABLE,
            () => receivedOrders.Add(KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_EXP_TABLE));

        var centerOrders = new List<int>();
        router.RegisterRoute(PerformerId.PI_CN_SERVER, ev =>
        {
            if (ev.EventId == EventId.ESR_ORDER_TO_CENTER_SERVER_NOT)
                centerOrders.Add(ev.GetPayload<KESR_SCRIPT_REFRESH_ORDER_NOT>(RegionProfile.Taiwan).OrderType);
            return true;
        });

        var script = new KScriptManager();
        KServerResourceLoader.BindGlobalProxies(gs, script);
        Assert.True(script.DoString("""
            ScriptRefresh:ExpTable()
            ScriptRefresh:DropTable()
            """, "script_refresh_lua"));

        await Task.Delay(50);
        // Execute queued GS event directly
        var evOrder = new KEvent();
        evOrder.SetInProcess(PerformerId.PI_GS_SERVER, null, EventId.ESR_ORDER_TO_GAME_SERVER_NOT,
            new KESR_SCRIPT_REFRESH_ORDER_NOT { OrderType = KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_EXP_TABLE });
        gs.QueueingEvent(evOrder);
        gs.Tick();

        Assert.Contains(KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_EXP_TABLE, receivedOrders);
        Assert.Contains(KESR_SCRIPT_REFRESH_ORDER_NOT.OT_CN_DROP_TABLE, centerOrders);
        gs.ShutDown();
    }

    [Fact]
    public void SelectUnit_1_To_5_Notifications_RoundTrip_All_Fields()
    {
        var n1 = RoundTrip(new KEGS_SELECT_UNIT_1_NOT
        {
            Ok = NetError.NET_OK,
            UnitInfo = new KUnitInfo { UnitUid = 101, NickName = "Hero", Level = 35 },
            RealDataED = 50_000,
            NumResurrectionStone = 5,
            SkillNoteMaxPageNum = 4,
            RelationshipInfo = new KRelationshipInfo { RelationshipType = 2, OtherUnitUid = 202, OtherNickName = "Partner" },
        });
        Assert.Equal(101, n1.UnitInfo.UnitUid);
        Assert.Equal(50_000, n1.RealDataED);
        Assert.Equal(2, n1.RelationshipInfo.RelationshipType);

        var n2 = RoundTrip(new KEGS_SELECT_UNIT_2_NOT { Ok = NetError.NET_OK });
        Assert.Equal(NetError.NET_OK, n2.Ok);

        var n3 = RoundTrip(new KEGS_SELECT_UNIT_3_NOT { Ok = NetError.NET_OK, SummonedPetUid = 999 });
        Assert.Equal(999, n3.SummonedPetUid);

        var n4 = RoundTrip(new KEGS_SELECT_UNIT_4_NOT { Ok = NetError.NET_OK, IsRecommend = true, RecommendUnitUid = 303 });
        Assert.True(n4.IsRecommend);
        Assert.Equal(303, n4.RecommendUnitUid);

        var n5 = RoundTrip(new KEGS_SELECT_UNIT_5_NOT { Ok = NetError.NET_OK, CashShopOpen = true });
        Assert.True(n5.CashShopOpen);
    }

    [Fact]
    public async Task TcpEventBridge_Relays_Serialized_KEvent_Across_Separate_Routers()
    {
        var routerA = new InProcessRouter();
        var routerB = new InProcessRouter();
        int portB = routerB.EnableRemoteBridge(RegionProfile.Taiwan, listenPort: 0, bindAddress: IPAddress.Loopback);
        Assert.True(portB > 0);

        var tcs = new TaskCompletionSource<KEvent>(TaskCreationOptions.RunContinuationsAsynchronously);
        routerB.RegisterRoute(PerformerId.PI_CN_SERVER, ev =>
        {
            tcs.TrySetResult(ev);
            return true;
        });

        routerA.RegisterRemoteRoute(PerformerId.PI_CN_SERVER, "127.0.0.1", portB, RegionProfile.Taiwan);
        bool sent = routerA.Send(PerformerId.PI_GS_SERVER, 100, PerformerId.PI_CN_SERVER, 300,
            EventId.ESR_ORDER_TO_CENTER_SERVER_NOT,
            new KESR_SCRIPT_REFRESH_ORDER_NOT { OrderType = KESR_SCRIPT_REFRESH_ORDER_NOT.OT_CN_DROP_TABLE });
        Assert.True(sent);

        var received = await tcs.Task.WaitAsync(TimeSpan.FromSeconds(5));
        Assert.Equal(EventId.ESR_ORDER_TO_CENTER_SERVER_NOT, received.EventId);
        var order = received.GetPayload<KESR_SCRIPT_REFRESH_ORDER_NOT>(RegionProfile.Taiwan);
        Assert.Equal(KESR_SCRIPT_REFRESH_ORDER_NOT.OT_CN_DROP_TABLE, order.OrderType);

        routerA.UnregisterAll();
        routerB.UnregisterAll();
    }

    [Fact]
    public void ChatLog_Record_And_JpEventRewardComplete_RoundTrip_And_Persist()
    {
        var pkt = RoundTrip(new KDCHATLOG_RECORD_CHAT_NOT
        {
            AccountUid = 1001,
            UnitUid = 2002,
            ServerGroupId = 1,
            ChatText = "Hello Elsword",
            ChatType = 2,
        });
        Assert.Equal(1001, pkt.AccountUid);
        Assert.Equal(2002, pkt.UnitUid);
        Assert.Equal(1, pkt.ServerGroupId);
        Assert.Equal("Hello Elsword", pkt.ChatText);
        Assert.Equal(2, pkt.ChatType);

        string dbPath = Path.Combine(Path.GetTempPath(), $"knc_chatlog_{Guid.NewGuid():N}.db");
        try
        {
            var db = new SqliteDatabaseManager(dbPath);
            db.InitializeAndSeed();
            db.RecordChatLog(pkt.AccountUid, pkt.UnitUid, pkt.ServerGroupId, pkt.ChatText, pkt.ChatType);
            var logs = db.GetChatLogs(pkt.UnitUid);
            Assert.Single(logs);
            Assert.Equal("Hello Elsword", logs[0].ChatText);
            Assert.Equal(2, logs[0].ChatType);

            var jpAck = RoundTrip(new KEBILL_REWARD_COUPON_JP_EVENT_ACK
            {
                RewardInfo = new KJPEventRewardInfo { RId = 777, UserId = "jpuser", UserUid = 1001, UnitUid = 2002, RewardId = 55 },
                UseCouponPacket = new KEBILL_USE_COUPON_ACK { Ok = NetError.NET_OK },
            });
            Assert.Equal(777, jpAck.RewardInfo.RId);
            Assert.Equal(NetError.NET_OK, jpAck.UseCouponPacket.Ok);
        }
        finally
        {
            try { File.Delete(dbPath); } catch { }
        }
    }

    [Fact]
    public void LuaConfigConstants_And_ServerBindings_Execute_RealConfigPatterns()
    {
        var router = new InProcessRouter();
        string dbPath = Path.Combine(Path.GetTempPath(), $"knc_cfg_{Guid.NewGuid():N}.db");
        try
        {
            using var game = new KGameServer(RegionCode.TW, dbPath, router);
            game.RegToLua();
            game.Center.RegToLua();

            Assert.True(game.ScriptManager.DoString("""
                local ip = GetLocalIP()
                assert(type(ip) == "string" and #ip > 0)
                Server:SetUseVersion(VE_SERVICE)
                Server:SetPassportLevel(2)
                Server:SetHackShieldHSBFilePath("HackShield/X2.hsb")
                Server:AddMSGBeforShutDown(23, 30, 0)
                SimLayer:SetBillingFlag(BF_GLOBAL_SERVICE)
                SimLayer:SetVersionFlag(VF_SERVICE)
                SimLayer:SetNationFlag(NF_BR)
                SimLayer:AddCommonFlag(CF_HSHIELD)
                SimLayer:SetEnableAuthWait(true)
                SimLayer:SetPublisherCheckGameServerLogin(true)
                DBLayer:AddDB(DC_KOG_BILLING, "Billing.dsn", 4, false)
                DBLayer:SetManagerDB(ESC_CENTER, "Manager.dsn", false)
                StatisticsManager:SetUserLog(true)
                StatisticsManager:SetBillingPacketLog(true)
                StatisticsManager:SendToFTPStatistics(true)
                StatisticsManager:AddStatistics(SI_CHARACTER, 10, true)
                StatisticsManager:AddStatistics(SI_ITEM_DB, 3600, true)
                StatisticsManager:SetStatisticsFlushTime(SI_ITEM_DB, 4)
                TRServer:TRWorkerThreadCount(6)
                """, "test_gs_cfg"));

            Assert.Equal(2, game.UseVersion);
            Assert.Equal(2, game.PassportLevel);
            Assert.Single(game.ShutDownMessageTimes);
            Assert.Equal(new TimeSpan(23, 30, 0), game.ShutDownMessageTimes[0]);
            Assert.Equal((int)BillingFlag.GlobalService, game.SimLayer.GetBillingFlag());
            Assert.Equal(NationFlag.BR, game.SimLayer.NationFlag);
            Assert.True(game.SimLayer.CheckCommonFlag((uint)ClientFlag.HShield));
            Assert.Equal(6, game.TrServer.WorkerThreadCount);
            Assert.True(game.Statistics.UserLog);
            Assert.True(game.Statistics.BillingPacketLog);
            Assert.True(game.Statistics.SendToFtpStatistics);
            Assert.Equal(4, game.Statistics.Schedules[KStatisticsManager.SI_ITEM_DB].FlushHour);

            Assert.True(game.Center.ScriptManager.DoString("""
                RoomManager:SetNumPvPRoom(1500)
                RoomManager:SetNumDungeonRoom(1500)
                RoomManager:SetNumSquare(10)
                RoomManager:SetNumTrade(300)
                RoomManager:SetNumPersonalShop(150)
                RoomManager:SetNumTrainingCenterRoom(1500)
                RoomManager:SetNumBattleField(1800)
                RoomManager:SetZUTickTime(300)
                RoomManager:SetBattleFieldZUTickTime(20)
                """, "test_cn_cfg"));

            Assert.Equal(1500, game.Center.RoomManager.NumPvPRoom);
            Assert.Equal(1800, game.Center.RoomManager.NumBattleField);
            Assert.Equal(300.0, game.Center.RoomManager.ZUTickTime);
            Assert.Equal(20.0, game.Center.RoomManager.BattleFieldZUTickTime);
        }
        finally
        {
            try { File.Delete(dbPath); } catch { }
        }
    }

    [Fact]
    public void GlobalServer_ShutdownNotify_EarthQuake_And_WorldBuff_Broadcast_To_GameServer()
    {
        var router = new InProcessRouter();
        var gsEvents = new List<KEvent>();
        router.RegisterRoute(PerformerId.PI_GS_SERVER, ev =>
        {
            gsEvents.Add(ev);
            return true;
        });

        using var global = new KGlobalServer(RegionCode.TW, router);
        var nowLocal = new DateTime(2026, 11, 20, 23, 30, 0, DateTimeKind.Local);
        global.AddMSGBeforShutDown(23, 30, 0);
        global.CheckSendMSGBeforeShutDown(DateTime.UtcNow, nowLocal);
        Assert.Contains(gsEvents, e => e.EventId == EventId.EGB_SHUT_DOWN_NOTIFY_NOT);

        global.CheckEarthQuakeEffect(DateTime.UtcNow.AddMinutes(10));
        Assert.Contains(gsEvents, e => e.EventId == EventId.EGB_EARTH_QUAKE_NOT);

        global.WorldMissionManager.StartMission(100, 30);
        global.Tick();
        Assert.Contains(gsEvents, e => e.EventId == EventId.EGB_UPDATE_WORLD_BUFF_NOT);
    }

    [Fact]
    public void SecureBuffer_SequenceMismatch_Flags_SequenceMismatch_OutParam()
    {
        var saDb = new KncWX2Server.Core.Security.KSADatabase();
        ushort spi = saDb.CreateNewSA(out _);

        var sb1 = new KncWX2Server.Core.Security.KSecureBuffer(saDb, spi: spi);
        Assert.True(sb1.Create(new byte[] { 1, 2, 3, 4 }));
        byte[] wire = sb1.WireBytes.ToArray();

        // First read advances expected sequence; replaying the exact same wire bytes has valid ICV but stale seqNum
        var rx1 = new KncWX2Server.Core.Security.KSecureBuffer(saDb, wire);
        Assert.True(rx1.TryGetPayload(out byte[] p1, out bool seqErr1));
        Assert.False(seqErr1);
        Assert.Equal(new byte[] { 1, 2, 3, 4 }, p1);

        var rxReplay = new KncWX2Server.Core.Security.KSecureBuffer(saDb, wire);
        Assert.False(rxReplay.TryGetPayload(out _, out bool seqErrReplay));
        Assert.True(seqErrReplay);
    }

    [Fact]
    public void PurchaserInfo_And_ConfirmTradeAck_Match_Cpp_Wire_Fields()
    {
        var purchaser = RoundTrip(new KPurchaserInfo
        {
            ServerGroupId = 2,
            UserUid = 1001,
            UserName = "TesterTW",
            UnitUid = 2002,
            UnitNickName = "HeroTW",
            Level = 70,
            Ip = "127.0.0.1",
            PublisherUid = 99999,
            ChannelingCode = 1,
        });
        Assert.Equal(2, purchaser.ServerGroupId);
        Assert.Equal(1001, purchaser.UserUid);
        Assert.Equal("TesterTW", purchaser.UserName);
        Assert.Equal(2002, purchaser.UnitUid);
        Assert.Equal("HeroTW", purchaser.UnitNickName);
        Assert.Equal((byte)70, purchaser.Level);
        Assert.Equal("127.0.0.1", purchaser.Ip);
        Assert.Equal(99999u, purchaser.PublisherUid);
        Assert.Equal(1, purchaser.ChannelingCode);

        var confirm = RoundTrip(new KERM_CONFIRM_TRADE_ACK
        {
            Ok = NetError.NET_OK,
            ItemQuantityUpdateTemp = new KItemQuantityUpdate(),
            ItemInfoTemp = [new KItemInfo { ItemId = 70001, Quantity = 3 }],
            EdTemp = 50000,
        });
        Assert.Equal(NetError.NET_OK, confirm.Ok);
        Assert.Single(confirm.ItemInfoTemp);
        Assert.Equal(70001, confirm.ItemInfoTemp[0].ItemId);
        Assert.Equal(50000, confirm.EdTemp);
    }

    [Fact]
    public void GlobalServer_JackPotEvent_And_HeroPvpObserver_Check_Emit_Expected_Notifications()
    {
        var router = new InProcessRouter();
        var gsEvents = new List<KEvent>();
        var gsUserEvents = new List<KEvent>();
        router.RegisterRoute(PerformerId.PI_GS_SERVER, ev => { gsEvents.Add(ev); return true; });
        router.RegisterRoute(PerformerId.PI_GS_USER, ev => { gsUserEvents.Add(ev); return true; });

        using var global = new KncWX2Server.GlobalServer.KGlobalServer(RegionCode.TW, router);

        var now = DateTime.Now;
        Assert.True(global.JackPotEventManager.Init(
        [
            new KncWX2Server.GlobalServer.KJackPotEventManager.KJackPotEventInitInfo
            {
                CouponNumber = "JP-COUPON-001",
                BeginDate = now.AddMinutes(-5).ToString("yyyy-MM-dd HH:mm:ss"),
                EndDate = now.AddMinutes(30).ToString("yyyy-MM-dd HH:mm:ss"),
                State = 0,
            },
        ]));

        // Register observer & online hero + jackpot participant
        var obsEv = new KEvent();
        obsEv.SetInProcess(PerformerId.PI_GB_SERVER, [9901L], EventId.EGB_ADMIN_HERO_PVP_USER_LIST_REQ, new KPacketOK());
        global.QueueingEvent(obsEv);

        var addEv = new KEvent();
        addEv.SetInProcess(PerformerId.PI_GB_SERVER, [9901L], EventId.EGB_ADMIN_HERO_PVP_USER_ADD_REQ,
            new KEGS_ADMIN_HERO_PVP_USER_ADD_REQ { UserList = [(1L, "JackPotHero")] });
        global.QueueingEvent(addEv);

        var regEv = new KEvent();
        regEv.SetInProcess(PerformerId.PI_GB_SERVER, [1L], EventId.EGB_REGISTER_UNIT_NOT, new KEGB_REGISTER_UNIT_NOT
        {
            UserUid = 5001,
            UnitUid = 6001,
            GameServerUid = 1,
            ServerGroupId = 1,
            NickName = "JackPotHero",
        });
        global.QueueingEvent(regEv);
        global.Tick();

        global.CheckJackPotEvent(now, bypassThrottle: true);
        Assert.Contains(gsEvents, e => e.EventId == EventId.EGB_JACKPOT_EVENT_NOTIFY_NOT);
        Assert.Contains(gsEvents, e => e.EventId == EventId.EGB_JACKPOT_EVENT_POST_NOT);

        global.CheckHeroPvpUserList();
        Assert.Contains(gsUserEvents, e => e.EventId == EventId.EGB_ADMIN_HERO_PVP_USER_CONNECTION_NOT);
    }

    [Fact]
    public void StopSalePShopAgency_Validates_OpenAgency_And_Forwards_To_LoginServer()
    {
        var router = new InProcessRouter();
        using var login = new KncWX2Server.LoginServer.KLoginServer(RegionCode.TW, _dbPath, router);
        router.RegisterPerformer(PerformerId.PI_LOGIN_SERVER, login);
        using var game = new KncWX2Server.GameServer.KGameServer(RegionCode.TW, _dbPath, router);

        var gsUserEvents = new List<KEvent>();
        router.RegisterRoute(PerformerId.PI_GS_USER, ev => { gsUserEvents.Add(ev); return true; });

        login.PersonalShopAgencyManager.CreateAgencyShop(8001, "Seller8001", "Agency8001",
            [new KPersonalShopAgencyManager.AgencyShopItem { ItemUid = 1, ItemId = 70001, Quantity = 1, PricePerUnit = 100 }],
            TimeSpan.FromMinutes(30));

        router.Send(PerformerId.PI_GS_USER, 8001, PerformerId.PI_LOGIN_SERVER, login.Uid,
            EventId.ERM_STOP_SALE_PSHOP_AGENCY_REQ, new KPacketLong { Value = 8001 });
        login.Tick();

        var stopAck = gsUserEvents.Last(e => e.EventId == EventId.ERM_STOP_SALE_PSHOP_AGENCY_ACK)
            .GetPayload<KERM_PICK_UP_FROM_PSHOP_AGENCY_ACK>(RegionProfile.Taiwan);
        Assert.Equal(NetError.NET_OK, stopAck.Ok);
        Assert.NotNull(game);
    }

    [Fact]
    public void JackpotDbPersistence_And_DbeEvents_WorkEndToEnd()
    {
        var initInfo = RoundTrip(new KJackPotEventInitInfo
        {
            CouponNumber = "JP-001",
            BeginDate = "2026-01-01 00:00:00",
            EndDate = "2026-12-31 23:59:59",
            UserUid = 10,
            UnitUid = 20,
            State = 1,
        });
        Assert.Equal("JP-001", initInfo.CouponNumber);
        Assert.Equal(1, initInfo.State);

        var db = new SqliteDatabaseManager(_dbPath);
        db.InitializeAndSeed();
        db.UpsertJackpotEventCoupon("JP-DB-777", "2026-01-01 00:00:00", "2026-12-31 23:59:59");

        var router = new InProcessRouter();
        var sentEvents = new List<KEvent>();
        router.RegisterRoute(PerformerId.PI_GS_SERVER, ev => sentEvents.Add(ev));

        using var gb = new KGlobalServer(RegionCode.TW, router, _dbPath) { Uid = 1 };
        gb.Init();

        router.Send(PerformerId.PI_GS_USER, 9001, PerformerId.PI_GB_SERVER, gb.Uid,
            EventId.EGB_REGISTER_UNIT_NOT, new KEGB_REGISTER_UNIT_NOT
            {
                UserUid = 5001,
                UnitUid = 9001,
                NickName = "JackpotHero",
                GameServerUid = 10,
            });
        gb.Tick();

        gb.CheckJackPotEvent(new DateTime(2026, 06, 01, 12, 0, 0), bypassThrottle: true);

        Assert.Contains(sentEvents, e => e.EventId == EventId.EGB_JACKPOT_EVENT_NOTIFY_NOT);
        Assert.Contains(sentEvents, e => e.EventId == EventId.EGB_JACKPOT_EVENT_POST_NOT);

        var updated = db.GetJackpotEventCoupons().Single(c => c.CouponNumber == "JP-DB-777");
        Assert.Equal(5001L, updated.UserUid);
        Assert.Equal(9001L, updated.UnitUid);
        Assert.Equal(1, updated.State);

        router.Send(PerformerId.PI_GS_SERVER, 10, PerformerId.PI_GB_SERVER, gb.Uid,
            EventId.DBE_JACKPOT_EVENT_INIT_REQ, new KPacketOK());
        gb.Tick();
        var initAck = sentEvents.Last(e => e.EventId == EventId.DBE_JACKPOT_EVENT_INIT_ACK)
            .GetPayload<KDBE_JACKPOT_EVENT_INIT_ACK>(RegionProfile.Taiwan);
        Assert.Contains(initAck.JackpotEventInitInfo, x => x.CouponNumber == "JP-DB-777" && x.State == 1);

        gb.ShutDown();
    }

    [Fact]
    public void MatchMaking_And_AutoParty_CppWireLayout_And_CancelLogic_MatchParity()
    {
        var mmReq = RoundTrip(new KEGB_REQUEST_MATCH_MAKING_REQ
        {
            RegFrontOfList = true,
            PvpPlayerCount = 2,
            PvpGameType = 1,
            MatchUserInfo = { new KMatchUserInfo { UnitUid = 3001, NickName = "M1", Rating = 1500, CurrentRank = 2, OfficialMatchCnt = 10 } },
        });
        Assert.True(mmReq.RegFrontOfList);
        Assert.Equal((sbyte)2, mmReq.PvpPlayerCount);
        Assert.Single(mmReq.MatchUserInfo);

        var apReq = RoundTrip(new KEGB_REQUEST_AUTO_PARTY_MAKING_REQ
        {
            RemakingAutoParty = true,
            AutoPartyCondition = new KAutoPartyConditionInfo { DungeonId = 30200, DifficultyLevel = 2, DungeonMode = 1, LimitPlayerCount = 4 },
            AutoPartyUser = { new KAutoPartyUserInfo { UnitUid = 3002, NickName = "A1", Level = 60 } },
        });
        Assert.True(apReq.RemakingAutoParty);
        Assert.Equal(30200, apReq.AutoPartyCondition.DungeonId);
        Assert.Single(apReq.AutoPartyUser);

        var mmCancel = RoundTrip(new KEGB_CANCEL_MATCH_MAKING_REQ { MatchUid = 4001, PartyUid = 5001, Reason = 9, SendBySystem = true });
        Assert.Equal(4001L, mmCancel.MatchUid);
        Assert.Equal(5001L, mmCancel.PartyUid);
        Assert.Equal(9, mmCancel.Reason);
        Assert.True(mmCancel.SendBySystem);

        var apCancel = RoundTrip(new KEGB_CANCEL_AUTO_PARTY_MAKING_REQ { AutoPartyUid = 6001, AutoPartyWaitNumber = 7001, Reason = 4, SendBySystem = true });
        Assert.Equal(6001L, apCancel.AutoPartyUid);
        Assert.Equal(7001L, apCancel.AutoPartyWaitNumber);
        Assert.Equal(4, apCancel.Reason);
        Assert.True(apCancel.SendBySystem);

        var router = new InProcessRouter();
        var gsUserEvents = new List<KEvent>();
        router.RegisterRoute(PerformerId.PI_GS_USER, ev => gsUserEvents.Add(ev));
        using var gb = new KGlobalServer(RegionCode.TW, router) { Uid = 1 };
        gb.Init();

        router.Send(PerformerId.PI_GS_USER, 8101, PerformerId.PI_GB_SERVER, gb.Uid,
            EventId.EGB_REQUEST_MATCH_MAKING_REQ, new KEGB_REQUEST_MATCH_MAKING_REQ
            {
                PvpPlayerCount = 1,
                MatchUserInfo = { new KMatchUserInfo { UnitUid = 8101, NickName = "A", Rating = 1200, CurrentRank = 1, OfficialMatchCnt = 10 } },
            });
        router.Send(PerformerId.PI_GS_USER, 8102, PerformerId.PI_GB_SERVER, gb.Uid,
            EventId.EGB_REQUEST_MATCH_MAKING_REQ, new KEGB_REQUEST_MATCH_MAKING_REQ
            {
                PvpPlayerCount = 1,
                MatchUserInfo = { new KMatchUserInfo { UnitUid = 8102, NickName = "B", Rating = 1210, CurrentRank = 1, OfficialMatchCnt = 10 } },
            });
        gb.Tick();

        var matchSuccess = Assert.IsType<KEGS_MATCH_MAKING_SUCCESS_NOT>(
            gsUserEvents.First(e => e.EventId == EventId.EGB_MATCH_MAKING_SUCCESS_NOT)
                .GetPayload<KEGS_MATCH_MAKING_SUCCESS_NOT>(RegionProfile.Taiwan));

        router.Send(PerformerId.PI_GS_USER, 8101, PerformerId.PI_GB_SERVER, gb.Uid,
            EventId.EGB_CANCEL_MATCH_MAKING_REQ, new KEGB_CANCEL_MATCH_MAKING_REQ
            {
                MatchUid = matchSuccess.MatchUid,
                Reason = NetError.NOT_LEAVE_MATCH_REASON_01,
                SendBySystem = false,
            });
        gb.Tick();

        Assert.Equal(2, gsUserEvents.Count(e => e.EventId == EventId.EGB_MATCH_CLOSE_NOT));
        gb.ShutDown();
    }

    [Fact]
    public void RegionalAuthBilling_LuaConfigs_And_ActionDecider_ExecuteAcrossAllRegions()
    {
        var router = new InProcessRouter();
        using var center = new KncWX2Server.CenterServer.KCenterServer(RegionCode.TW, router);
        center.RegToLua();

        const string lua = """
            SimLayer:SetCheckIPMode( CIM_SERVICE_MODE )
            SimLayer:SetCheckIPType( CIT_ALL_IP_ALLOW )
            SimLayer:SetCheckModeCheckIPType( CIT_CHECK_IP_BLOCK )
            SimLayer:SetHanInitFlag( HIF_REAL )
            SimLayer:SetServerGroupNum( 1 )
            SimLayer:AddIP( '127.0.0.1' )
            SimLayer:AddCheckModeIP( '192.168.1' )
            SimLayer:SetAuthFlag( AF_GAME_FORGE_EU )
            SimLayer:SetBillingFlag( BF_GAME_FORGE_EU )

            RoomManager:SetNumArcadeRoom( 10 )
            RoomManager:SetNumParty( 500 )
            RoomManager:SetNumWeddingHall( 50 )

            ActionDecider:AddAction( 1, 101, 0.5 )
            ActionDecider:AddAction( 1, 102, 0.5 )

            -- BR
            LevelUpAuthManager:AddWebServiceAddress( 'http://auth.br/soap' )
            LevelUpAuthManager:AddWebMethod( LU_SALT, 'GetSalt' )
            LevelUpAuthManager:AddWebMethod( LU_AUTH, 'Authenticate' )
            LevelUpAuthManager:Init( 4 )
            PayletterBillingManager:InitPayletterBillingInfo( '10.0.0.1', 5000, 1 )
            PayletterBillingManager:SetNumThread( 4 )

            -- CN
            GiantAuthManager:SetAddress( '10.0.1.1', 9001 )
            GiantAuthManager:SetNumThread( 2 )
            GiantBillingManager:SetAddress( '10.0.1.2', 9002 )
            GiantBillingManager:SetHeartBitInterval( 10000 )
            GiantBillingManager:SetConnectionCheckInterval( 5000 )
            GiantBillingManager:SetNumThread( 2 )
            GiantCouponManager:SetAddress( '10.0.1.3', 9003 )
            GiantCouponManager:SetNumThread( 2 )
            GiantInfoManager:SetAddress( '10.0.1.4', 9004 )
            GiantInfoManager:SetNumThread( 2 )
            GiantRoleRegManager:SetAddress( '10.0.1.5', 9005 )
            GiantRoleRegManager:SetNumThread( 2 )

            -- EU
            GFAuthManager:AddWebServiceAddress( 'http://gf.eu/auth' )
            GFAuthManager:AddWebMethod( GFO_AUTH, 'urn:Auth' )
            GFAuthManager:Init( 2 )
            GFBillingManager:AddWebServiceAddress( 'http://gf.eu/bill' )
            GFBillingManager:AddWebMethod( GFBilling_GetCash, 'urn:GetCash' )
            GFBillingManager:AddWebMethod( GFBilling_ReduceCash, 'urn:ReduceCash' )
            GFBillingManager:Init( 2 )

            -- ID / IN / JP / PH / TH / TW / US / KR
            CJBillingManager:InitCJBillingInfo( '10.0.2.1', 7000 )
            CJBillingManager:SetNumThread( 2 )
            FunizenAuthManager:SetNumThread( 2 )
            NaunBillingManager:SetNumThread( 2 )
            LoginNetCafeManager:SetNumThread( 2 )
            PurpleAuthManager:SetNumThread( 2 )
            PurpleBillingManager:SetNumThread( 2 )
            KGarenaBillingServer:WorkerThreadCount( 4 )
            AsiaSoftAuthManager:InitAsiaSoftAuthValue( 'EL', 'TH' )
            AsiaSoftAuthManager:InitAsiaSoftAuthAddress( '10.0.3.1', 8001 )
            AsiaSoftAuthManager:InitAsiaSoftAKeyAddress( '10.0.3.2', 8002 )
            AsiaSoftAuthManager:InitAsiaSoftIPBonusAddress( '10.0.3.3', 8003 )
            AsiaSoftAuthManager:SetNumThread( 2 )
            GASHAuthManager:InitGASHAuthValue( '300060', 'TW' )
            GASHAuthManager:InitGASHAuthAddress( '10.0.4.1', 5410, 5411, 5412 )
            GASHAuthManager:SetNumThread( 2 )
            GASHBillingTCPManager:InitGASHBillingTCPValue( '300060', 'TW' )
            GASHBillingTCPManager:InitGASHBillingTCPAddress( '10.0.4.2', 5421, 5422 )
            GASHBillingTCPManager:SetNumThread( 2 )
            K3RComboAuthManager:AddWebServiceAddress( 'http://k3r.us/sec' )
            K3RComboAuthManager:AddWebMethod( K3R_AUTH, 'Login' )
            K3RComboAuthManager:AddWebMethod( K3R_AUTH_SECOND_SECURITY, 'SecQ' )
            K3RComboAuthManager:Init( 2 )
            NXAuthManager:InitNexonAuthInfo( '10.0.5.1', 10001, 1, 'elsword', 1, 1 )
            NXAuthManager:SetNumThread( 2 )
            NXBillingAuthManager:InitNexonBillingAuthInfo( '10.0.5.2', 10002, 1, 'elsword' )
            NXBillingAuthManager:SetNumThread( 2 )
            NXBillingManager:AddWebServiceAddress( NBO_PURCHASE_COUPON, 'http://nx/coupon', 'BuyCoupon' )
            NXBillingManager:Init( 2 )
            NXBillingTCPManager:InitNexonBillingTCPInfo( '10.0.5.3', 6503, 99 )
            NXBillingTCPManager:SetNumThread( 2 )
            NexonSOAP:AddWebServiceAddress( 'http://nx/soap' )
            NexonSOAP:AddWebMethod( NXSOAP_AUTH, 'Auth' )
            NexonSOAP:Init( 2, 'http://nx/user', 'http://nx/cpn' )
            """;

        Assert.True(center.ScriptManager.DoString(lua));
        Assert.Equal(AuthFlag.GlobalService, center.SimLayer.AuthFlag);
        Assert.Equal(BillingFlag.GlobalService, center.SimLayer.BillingFlag);
        Assert.Equal(50, center.RoomManager.NumWeddingHall);
        Assert.Equal(101, center.ActionDecider.MakeDecision(1, 0.25f));
        Assert.Equal(102, center.ActionDecider.MakeDecision(1, 0.75f));
        Assert.Equal("http://auth.br/soap", center.RegionalAuthBilling.LevelUpAuthManager.WsdlAddress);
        Assert.Equal("10.0.1.2", center.RegionalAuthBilling.GiantBillingManager.ServerIp);
        Assert.Equal(10000, center.RegionalAuthBilling.GiantBillingManager.HeartBitInterval);
        Assert.Equal((ushort)5412, center.RegionalAuthBilling.GASHAuthManager.AccountingPort);
        Assert.Equal("TW", center.RegionalAuthBilling.GASHBillingTCPManager.ServiceRegion);
        Assert.Equal((ushort)6503, center.RegionalAuthBilling.NXBillingTCPManager.Port);
        Assert.Equal(4, center.RegionalAuthBilling.GarenaBillingServer.WorkerThreads);
    }

    [Fact]
    public void ClientPacketWireLayoutParity_NewlyAddedAndFixedPackets()
    {
        var udpKick = RoundTrip(new KEGS_UDP_KICK_STATE_CHANGE_FIELD_NOT { AccusationUnitUid = 9000012345L });
        Assert.Equal(9000012345L, udpKick.AccusationUnitUid);

        var fieldLoad = RoundTrip(new KEGS_FIELD_LOADING_COMPLETE_ACK { Ok = NetError.NET_OK, PopupTheGuideLine = true });
        Assert.Equal(NetError.NET_OK, fieldLoad.Ok);
        Assert.True(fieldLoad.PopupTheGuideLine);

        var beforeShutdown = RoundTrip(new KEGS_BEFORE_SHUT_DOWN_MESSAGE_NOT { NetErrorId = 1, Time = "23:55" });
        Assert.Equal(1, beforeShutdown.NetErrorId);
        Assert.Equal("23:55", beforeShutdown.Time);

        var shutdown = RoundTrip(new KEGS_SHUT_DOWN_MESSAGE_NOT { NetErrorId = 2 });
        Assert.Equal(2, shutdown.NetErrorId);

        var pvpEv = RoundTrip(new KEGS_PVP_EVENT_INFO_NOT { EventInfo = true });
        Assert.True(pvpEv.EventInfo);

        var tooni = RoundTrip(new KEGS_TOONILAND_USER_ID_NOT { TooniLandId = "tooni_test" });
        Assert.Equal("tooni_test", tooni.TooniLandId);

        var evMoney = RoundTrip(new KEGS_UPDATE_EVENT_MONEY_NOT { EventMoney = 12345 });
        Assert.Equal(12345, evMoney.EventMoney);

        var hackNot = RoundTrip(new KEGS_SERVER_CHECK_HACK_USER_NOT { HackingCheckType = KEGS_SERVER_CHECK_HACK_USER_REQ.HctDungeonClearTime });
        Assert.Equal(KEGS_SERVER_CHECK_HACK_USER_REQ.HctDungeonClearTime, hackNot.HackingCheckType);
    }

    [Fact]
    public async Task GSUserAndCenterServer_ErmTranslationAndLifecycleHandlers()
    {
        var router = new InProcessRouter();
        using var center = new KCenterServer(RegionCode.TW, router);
        using var global = new KGlobalServer(RegionCode.TW, router, _dbPath);
        using var game = new KGameServer(RegionCode.TW, _dbPath, router, center, global);
        Assert.True(center.Start(overridePort: 0));
        Assert.True(global.Start(overridePort: 0));
        Assert.True(game.Start(overridePort: 0, overrideTrPort: 0, overrideUdpEchoPort: 0, overrideUdpRelayPort: 0));

        try
        {
            // Verify CenterServer handles both KEGS_SERVER_CHECK_HACK_USER_NOT and KERM_DUNGEON_SECURITY_REPORT_NOT
            var hackCenterEv = new KEvent();
            hackCenterEv.SetInProcess(PerformerId.PI_GS_USER, [1001L], EventId.ERM_SERVER_CHECK_HACK_USER_NOT,
                new KEGS_SERVER_CHECK_HACK_USER_NOT { HackingCheckType = KEGS_SERVER_CHECK_HACK_USER_REQ.HctDungeonClearTime });
            center.QueueingEvent(hackCenterEv);

            var secCenterEv = new KEvent();
            secCenterEv.SetInProcess(PerformerId.PI_GS_USER, [1001L], EventId.ERM_SERVER_CHECK_HACK_USER_NOT,
                new KERM_DUNGEON_SECURITY_REPORT_NOT { RoomUid = 1, DungeonId = 30000, UnitUid = 1001, MaxSingleDamage = 500 });
            center.QueueingEvent(secCenterEv);
            center.Tick();

            // Connect real TCP client and select unit
            using var client = new TcpClient();
            await client.ConnectAsync("127.0.0.1", game.ListenPort);
            using var stream = client.GetStream();
            var sa = new KSADatabase();
            ushort spi = await TestHandshakeAsync(stream, sa, RegionProfile.Taiwan);

            await TestSendAsync(stream, sa, spi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_CONNECT_REQ, new KEGS_CONNECT_REQ { Version = "TW_2014_03" });
            Assert.Equal(EventId.EGS_CONNECT_ACK, (await TestReadEventAsync(stream, sa, RegionProfile.Taiwan)).EventId);

            game.Db.VerifyOrRegisterAccount("TW", "p21_erm_user", "pw", autoCreateOnMissing: true);
            await TestSendAsync(stream, sa, spi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_VERIFY_ACCOUNT_REQ, new KUserAuthenticateReq { UserId = "p21_erm_user", Password = "pw" });
            Assert.Equal(EventId.EGS_VERIFY_ACCOUNT_ACK, (await TestReadEventAsync(stream, sa, RegionProfile.Taiwan)).EventId);

            await TestSendAsync(stream, sa, spi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_CREATE_UNIT_REQ, new KEGS_CREATE_UNIT_REQ { NickName = "P21ErmHero", UnitClass = 1 });
            long unitUid = (await TestReadEventAsync(stream, sa, RegionProfile.Taiwan))
                .GetPayload<KEGS_CREATE_UNIT_ACK>(RegionProfile.Taiwan).UnitInfo.UnitUid;

            await TestSendAsync(stream, sa, spi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_SELECT_UNIT_REQ, new KEGS_SELECT_UNIT_REQ { UnitUid = unitUid });
            while ((await TestReadEventAsync(stream, sa, RegionProfile.Taiwan)).EventId != EventId.EGS_GET_MY_INVENTORY_ACK) { }

            var gsUser = game.GetUserByUnitUid(unitUid);
            Assert.NotNull(gsUser);
            gsUser!.TempInventory.AddTempInventoryItem(100100, 0);
            Assert.False(gsUser.TempInventory.IsEmpty);

            var clearEv = new KEvent();
            clearEv.SetInProcess(PerformerId.PI_GS_USER, [center.Uid], EventId.ERM_CLEAR_TEMP_INVENTORY_NOT, new KPacketOK());
            gsUser.QueueingEvent(clearEv);
            gsUser.Tick();
            Assert.True(gsUser.TempInventory.IsEmpty);

            var zeEv = new KEvent();
            zeEv.SetInProcess(PerformerId.PI_GS_USER, [center.Uid], EventId.ERM_ZERO_ENDURANCE_ITEM_IN_ROOM_NOT,
                new KERM_ZERO_ENDURANCE_ITEM_IN_ROOM_NOT
                {
                    UnitUid = unitUid,
                    RoomUserInfo = new KRoomUserInfo { UnitUid = unitUid, GameStat = new KStat { AtkPhysic = 777 } },
                });
            gsUser.QueueingEvent(zeEv);
            gsUser.Tick();
            var zeAck = await TestReadEventAsync(stream, sa, RegionProfile.Taiwan);
            Assert.Equal(EventId.EGS_ZERO_ENDURANCE_ITEM_IN_ROOM_NOT, zeAck.EventId);
            Assert.Equal(777, zeAck.GetPayload<KEGS_ZERO_ENDURANCE_ITEM_IN_ROOM_NOT>(RegionProfile.Taiwan).GameStat.AtkPhysic);

            var joinShopEv = new KEvent();
            joinShopEv.SetInProcess(PerformerId.PI_GS_USER, [center.Uid], EventId.ERM_JOIN_PERSONAL_SHOP_ACK,
                new KERM_JOIN_PERSONAL_SHOP_ACK
                {
                    Ok = NetError.NET_OK,
                    PersonalShopUid = 555001,
                    PersonalShopName = "TestShop",
                    NickName = "Host",
                });
            gsUser.QueueingEvent(joinShopEv);
            gsUser.Tick();
            var joinShopAck = await TestReadEventAsync(stream, sa, RegionProfile.Taiwan);
            Assert.Equal(EventId.EGS_JOIN_PERSONAL_SHOP_ACK, joinShopAck.EventId);
            Assert.Equal("TestShop", joinShopAck.GetPayload<KEGS_JOIN_PERSONAL_SHOP_ACK>(RegionProfile.Taiwan).PersonalShopName);
            Assert.Equal(555001L, gsUser.PersonalShopManager.PersonalShopUid);

            var breakShopEv = new KEvent();
            breakShopEv.SetInProcess(PerformerId.PI_GS_USER, [center.Uid], EventId.ERM_BREAK_PERSONAL_SHOP_NOT,
                new KERM_BREAK_PERSONAL_SHOP_NOT { Reason = NetError.NOT_LEAVE_ROOM_REASON_01, HostUid = 555001 });
            gsUser.QueueingEvent(breakShopEv);
            gsUser.Tick();
            var breakShopNot = await TestReadEventAsync(stream, sa, RegionProfile.Taiwan);
            Assert.Equal(EventId.EGS_BREAK_PERSONAL_SHOP_NOT, breakShopNot.EventId);
            Assert.Equal(NetError.NOT_LEAVE_ROOM_REASON_01, breakShopNot.GetPayload<KEGS_BREAK_PERSONAL_SHOP_NOT>(RegionProfile.Taiwan).Reason);
            Assert.Equal(0L, gsUser.PersonalShopManager.PersonalShopUid);

            var searchEv = new KEvent();
            searchEv.SetInProcess(PerformerId.PI_GS_USER, [1L], EventId.ERM_SEARCH_UNIT_ACK,
                new KELG_SEARCH_UNIT_ACK
                {
                    Ok = NetError.NET_OK,
                    EventIdValue = (ushort)EventId.EGS_GET_CONNECTION_UNIT_INFO_REQ,
                    CUnitInfo = new KConnectionUnitInfo { UnitUid = 42, CharName = "Target" },
                });
            gsUser.QueueingEvent(searchEv);
            gsUser.Tick();
            var connAck = await TestReadEventAsync(stream, sa, RegionProfile.Taiwan);
            Assert.Equal(EventId.EGS_GET_CONNECTION_UNIT_INFO_ACK, connAck.EventId);
            Assert.Equal("Target", connAck.GetPayload<KEGS_GET_CONNECTION_UNIT_INFO_ACK>(RegionProfile.Taiwan).CUnitInfo.CharName);

            var fieldChEv = new KEvent();
            fieldChEv.SetInProcess(PerformerId.PI_GS_USER, [center.Uid], EventId.ERM_JOIN_FIELD_CHANNEL_CHANGE_ACK,
                new KERM_JOIN_FIELD_ACK { Ok = NetError.NET_OK, FieldUid = 777001 });
            gsUser.QueueingEvent(fieldChEv);
            gsUser.Tick();
            var fieldChAck = await TestReadEventAsync(stream, sa, RegionProfile.Taiwan);
            Assert.Equal(EventId.EGS_JOIN_FIELD_CHANNEL_CHANGE_ACK, fieldChAck.EventId);
            Assert.Equal(NetError.NET_OK, fieldChAck.GetPayload<KEGS_JOIN_FIELD_CHANNEL_CHANGE_ACK>(RegionProfile.Taiwan).Ok);
            Assert.Equal(777001L, gsUser.CurrentFieldUid);

            var selfTradeEv = new KEvent();
            selfTradeEv.SetInProcess(PerformerId.PI_GS_USER, [center.Uid], EventId.ERM_JOIN_TRADE_NOT,
                new KEGS_JOIN_TRADE_NOT { TradeUserInfo = new KTradeUserInfo { UnitUid = unitUid, NickName = "Self" } });
            gsUser.QueueingEvent(selfTradeEv);

            var otherTradeEv = new KEvent();
            otherTradeEv.SetInProcess(PerformerId.PI_GS_USER, [center.Uid], EventId.ERM_JOIN_TRADE_NOT,
                new KEGS_JOIN_TRADE_NOT { TradeUserInfo = new KTradeUserInfo { UnitUid = unitUid + 99, NickName = "Partner" } });
            gsUser.QueueingEvent(otherTradeEv);
            gsUser.Tick();
            var tradeNot = await TestReadEventAsync(stream, sa, RegionProfile.Taiwan);
            Assert.Equal(EventId.EGS_JOIN_TRADE_NOT, tradeNot.EventId);
            Assert.Equal("Partner", tradeNot.GetPayload<KEGS_JOIN_TRADE_NOT>(RegionProfile.Taiwan).TradeUserInfo.NickName);

            var buffEv = new KEvent();
            buffEv.SetNoParam(PerformerId.PI_GS_USER, [center.Uid], EventId.ERM_UPDATE_BUFF_INFO_NOT, RegionProfile.Taiwan);
            gsUser.QueueingEvent(buffEv);
            gsUser.Tick();
            Assert.Equal(EventId.EGS_UPDATE_BUFF_INFO_NOT, (await TestReadEventAsync(stream, sa, RegionProfile.Taiwan)).EventId);
        }
        finally
        {
            game.ShutDown();
            global.ShutDown();
            center.ShutDown();
        }
    }

    private static async Task<ushort> TestHandshakeAsync(NetworkStream stream, KSADatabase saDb, RegionProfile region)
    {
        var acceptEv = await TestReadEventAsync(stream, saDb, region);
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

    private static async Task TestSendAsync(NetworkStream stream, KSADatabase saDb, ushort spi, RegionProfile region, uint destPi, EventId id, IKSerializable packet)
    {
        var ev = new KEvent();
        ev.DestPerformer.PerformerId = destPi;
        ev.EventId = id;
        var ks = new KSerializer(region);
        ks.BeginWriting(ev.Buffer);
        packet.Put(ks, region);
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

    private static async Task<KEvent> TestReadEventAsync(NetworkStream stream, KSADatabase saDb, RegionProfile region)
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
        var sec = new KSecureBuffer(saDb, payload);
        Assert.True(sec.TryGetPayload(out byte[] plain));
        var ev = new KEvent();
        var ks = new KSerializer(region);
        ks.BeginReading(new KSerBuffer(plain));
        Assert.True(ev.Get(ks, region));
        ks.EndReading();
        ev.Buffer.UnCompress();
        return ev;
    }

    private static T RoundTrip<T>(T pkt) where T : IKSerializable, new()
    {
        var buf = new KSerBuffer();
        var ks = new KSerializer(RegionProfile.Taiwan);
        ks.BeginWriting(buf);
        Assert.True(pkt.Put(ks, RegionProfile.Taiwan));
        ks.EndWriting();

        var outPkt = new T();
        ks.BeginReading(buf);
        Assert.True(outPkt.Get(ks, RegionProfile.Taiwan));
        ks.EndReading();
        return outPkt;
    }
}
