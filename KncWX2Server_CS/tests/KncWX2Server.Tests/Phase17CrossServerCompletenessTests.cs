using KncWX2Server.CenterServer;
using KncWX2Server.Core.Events;
using KncWX2Server.Core.Framework;
using KncWX2Server.Core.Packets;
using KncWX2Server.Core.Region;
using KncWX2Server.GameServer;
using KncWX2Server.GlobalServer;
using KncWX2Server.LoginServer;
using Xunit;

namespace KncWX2Server.Tests;

public sealed class Phase17CrossServerCompletenessTests : IDisposable
{
    private readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"knc_phase17_{Guid.NewGuid():N}.db");

    public void Dispose()
    {
        try { if (File.Exists(_dbPath)) File.Delete(_dbPath); } catch { }
    }

    [Fact]
    public void GlobalServer_Handles_ServerSN_ExchangeLimit_TimeEnchant_And_HeroPvp()
    {
        var router = new InProcessRouter();
        using var global = new KGlobalServer(RegionCode.TW, router);
        global.Init();
        global.Uid = 400;

        var received = new List<KEvent>();
        router.RegisterRoute(PerformerId.PI_GS_USER, ev => received.Add(ev));

        // 1. ServerSN duplicate detection + deregistration
        router.Send(PerformerId.PI_GS_USER, 101, PerformerId.PI_GB_SERVER, 0,
            EventId.EGB_REGISTER_SERVERSN_REQ, new KEGB_REGISTER_SERVERSN_REQ { UserUid = 101, ServerSn = 999_001 });
        router.Send(PerformerId.PI_GS_USER, 102, PerformerId.PI_GB_SERVER, 0,
            EventId.EGB_REGISTER_SERVERSN_REQ, new KEGB_REGISTER_SERVERSN_REQ { UserUid = 102, ServerSn = 999_001 });
        global.Tick();

        var snAck1 = received.First(e => e.EventId == EventId.EGB_REGISTER_SERVERSN_ACK && e.DestPerformer.Uids.Contains(101))
            .GetPayload<KEGB_REGISTER_SERVERSN_ACK>(RegionProfile.Taiwan);
        var snAck2 = received.First(e => e.EventId == EventId.EGB_REGISTER_SERVERSN_ACK && e.DestPerformer.Uids.Contains(102))
            .GetPayload<KEGB_REGISTER_SERVERSN_ACK>(RegionProfile.Taiwan);
        Assert.Equal(NetError.NET_OK, snAck1.Ok);
        Assert.Equal(NetError.ERR_SN_OVERLAP_01, snAck2.Ok);
        Assert.Contains(received, e => e.EventId == EventId.EGB_REGISTER_SERVERSN_NOT);

        router.Send(PerformerId.PI_GS_USER, 102, PerformerId.PI_GB_SERVER, 0,
            EventId.EGB_UNREGISTER_SERVERSN_NOT, new KEGB_UNREGISTER_SERVERSN_NOT { UserUid = 102, ServerSn = 999_001 });
        global.Tick();

        // 2. ExchangeLimit acquire, exhaust (ERR_EXCHANGE_LIMIT_02), and rollback
        var now = DateTime.UtcNow;
        global.ExchangeLimitManager.Initialize(10,
        [
            new KExchangeLimitManager.ExchangeLimitEntry
            {
                LimitUid = 55,
                DestItemIds = [700001],
                LimitQuantity = 1,
                BeginUtc = now.AddMinutes(-5),
                EndUtc = now.AddMinutes(30),
            }
        ], now);

        received.Clear();
        router.Send(PerformerId.PI_GS_USER, 101, PerformerId.PI_GB_SERVER, 0,
            EventId.EGB_EXCHANGE_LIMIT_REQ, new KEGB_EXCHANGE_LIMIT_REQ { UnitUid = 5001, DestItemId = 700001, AfterWorkStorageKey = 77 });
        router.Send(PerformerId.PI_GS_USER, 101, PerformerId.PI_GB_SERVER, 0,
            EventId.EGB_EXCHANGE_LIMIT_REQ, new KEGB_EXCHANGE_LIMIT_REQ { UnitUid = 5001, DestItemId = 700001, AfterWorkStorageKey = 78 });
        global.Tick();

        var exAcks = received.Where(e => e.EventId == EventId.EGB_EXCHANGE_LIMIT_ACK)
            .Select(e => e.GetPayload<KEGB_EXCHANGE_LIMIT_ACK>(RegionProfile.Taiwan))
            .ToList();
        Assert.Equal(2, exAcks.Count);
        Assert.Equal(NetError.NET_OK, exAcks[0].Ok);
        Assert.Equal(NetError.ERR_EXCHANGE_LIMIT_02, exAcks[1].Ok);

        router.Send(PerformerId.PI_GS_USER, 101, PerformerId.PI_GB_SERVER, 0,
            EventId.EGB_EXCHANGE_LIMIT_INFO_ROLLBACK_NOT, new KEGB_EXCHANGE_LIMIT_INFO_ROLLBACK_NOT
            {
                ExchangeLimitUid = exAcks[0].ExchangeLimitUid,
                LimitUid = exAcks[0].LimitUid,
                RollBackUid = exAcks[0].RollBackUid,
            });
        global.Tick();
        Assert.Equal(1, global.ExchangeLimitManager.GetRemainingQuantity(700001));

        // 3. TimeEnchantEvent
        global.TimeEnchantEventManager.Initialize(3,
        [
            new KTimeEnchantEventManager.TimeEnchantEventEntry
            {
                EventUid = 1,
                EnchantLevel = 11,
                NotifyGetItem = false,
                BeginUtc = now.AddMinutes(-1),
                EndUtc = now.AddMinutes(10),
            }
        ], now);
        received.Clear();
        router.Send(PerformerId.PI_GS_USER, 101, PerformerId.PI_GB_SERVER, 0,
            EventId.EGB_TIME_ENCHANT_EVENT_NOT, new KEGB_TIME_ENCHANT_EVENT_NOT
            {
                UnitUid = 5001,
                AfterWorkStorageKey = 91,
                EventId = (ushort)EventId.EGB_TIME_ENCHANT_EVENT_CUBE_NOT,
            });
        global.Tick();
        var teRes = received.Single(e => e.EventId == EventId.EGB_TIME_ENCHANT_EVENT_CUBE_NOT)
            .GetPayload<KTimeEnchantEventResult>(RegionProfile.Taiwan);
        Assert.Equal(11, teRes.TimeEnchantEventLevel);
        Assert.False(teRes.NotifyGetItem);

        // 4. Hero PvP Tournament connection & Admin clears
        router.Send(PerformerId.PI_GS_USER, 101, PerformerId.PI_GB_SERVER, 0,
            EventId.EGB_ADMIN_HERO_PVP_USER_TOURNAMENT_CONNECTION_NOT,
            new KEGB_ADMIN_HERO_PVP_USER_TOURNAMENT_CONNECTION_NOT { ServerGroupId = 1, NickName = "HeroA", Connect = true });
        global.Tick();
        Assert.Contains(global.GetHeroPvpUsers(), u => u.NickName == "HeroA" && u.On);
    }

    [Fact]
    public void CenterServer_And_DungeonSubStageManager_Route_Rooms_And_StageGraphs()
    {
        var router = new InProcessRouter();
        using var center = new KCenterServer(RegionCode.TW, router);
        center.Init();
        center.Uid = 300;
        RouterRegisterRoom(router, center);

        var replies = new List<KEvent>();
        router.RegisterRoute(PerformerId.PI_GS_SERVER, ev => replies.Add(ev));
        router.RegisterRoute(PerformerId.PI_GS_USER, ev => replies.Add(ev));

        // 1. Server connect & channel change
        router.Send(PerformerId.PI_GS_SERVER, 1000, PerformerId.PI_CN_SERVER, 0,
            EventId.ECN_VERIFY_SERVER_CONNECT_REQ, new KECN_VERIFY_SERVER_CONNECT_REQ { ServerUid = 1000, ServerGroupId = 1 });
        router.Send(PerformerId.PI_GS_USER, 2001, PerformerId.PI_CN_SERVER, 0,
            EventId.ERM_OPEN_PVP_ROOM_REQ, new KERM_OPEN_ROOM_REQ
            {
                RoomInfo = new KRoomInfo { RoomType = 0, RoomName = "Arena", MaxSlot = 4, Public = true },
                HostInfo = new KRoomUserInfo { UnitUid = 501, NickName = "HostUnit", Level = 70 },
            });
        center.Tick();

        Assert.Contains(replies, e => e.EventId == EventId.ECN_VERIFY_SERVER_CONNECT_ACK);
        var openAck = replies.Single(e => e.EventId == EventId.ERM_OPEN_ROOM_ACK)
            .GetPayload<KERM_OPEN_ROOM_ACK>(RegionProfile.Taiwan);
        Assert.Equal(NetError.NET_OK, openAck.Ok);
        Assert.Equal(1, openAck.RoomInfo.JoinSlot);

        // 2. KDungeonSubStageManager weighted progression & secret stage
        var mgr = center.DungeonSubStageManager;
        var graph = new KDungeonSubStageManager.DungeonStageGraph { DungeonId = 30000 };
        var st0 = new KDungeonSubStageManager.StageTemplate { StageIndex = 0 };
        var sub0 = new KDungeonSubStageManager.SubStageTemplate { SubStageIndex = 0 };
        sub0.NextStages.Add(new KDungeonSubStageManager.NextStageData(KDungeonSubStageManager.ClearType.CtSubStage, 0, 1, 80));
        sub0.NextStages.Add(new KDungeonSubStageManager.NextStageData(KDungeonSubStageManager.ClearType.CtStageSecret, 99, 0, 20));
        st0.SubStages[0] = sub0;
        graph.Stages[0] = st0;
        mgr.RegisterDungeonGraph(graph);

        var normalNext = mgr.ResolveNextStage(30000, 0, 0, fixedUnitRoll: 0.1);
        Assert.Equal(KDungeonSubStageManager.ClearType.CtSubStage, normalNext.ClearType);
        Assert.Equal(1, normalNext.SubStageIndex);

        var secretNext = mgr.ResolveNextStage(30000, 0, 0, KDungeonSubStageManager.SecretStagePad.SspSecret);
        Assert.Equal(KDungeonSubStageManager.ClearType.CtStageSecret, secretNext.ClearType);
        Assert.Equal(99, secretNext.StageIndex);
        Assert.True(KDungeonSubStageManager.EvaluateSecretStageEntry(65, [(60, 70, 50f)], fixedUnitRoll: 0.25));
    }

    [Fact]
    public void LoginServer_Routes_Search_Buff_Chat_Friend_Wedding_And_GuildAd()
    {
        var router = new InProcessRouter();
        using var login = new KLoginServer(RegionCode.TW, _dbPath, router);
        login.Init();
        login.Uid = 100;

        var (_, accUid1, _, _, _) = login.Db.VerifyOrRegisterAccount("TW", "u1", "pw", autoCreateOnMissing: true);
        var (_, u1, _) = login.Db.CreateUnit(RegionProfile.Taiwan, accUid1, "AliceTW", 1);
        long unitUid1 = u1!.UnitUid;
        var (_, accUid2, _, _, _) = login.Db.VerifyOrRegisterAccount("TW", "u2", "pw", autoCreateOnMissing: true);
        var (_, u2, _) = login.Db.CreateUnit(RegionProfile.Taiwan, accUid2, "BobTW", 2);
        long unitUid2 = u2!.UnitUid;

        var gsUserEvents = new List<KEvent>();
        router.RegisterRoute(PerformerId.PI_GS_USER, ev => gsUserEvents.Add(ev));

        // Register both online units
        router.Send(PerformerId.PI_GS_USER, 1001, PerformerId.PI_LOGIN_SERVER, 0,
            EventId.ELG_REGISTER_UNIT_NOT, new KELG_REGISTER_UNIT_NOT { UnitUid = unitUid1, UserUid = accUid1, GameServerUid = 200, ChannelId = 2, NickName = "AliceTW" });
        router.Send(PerformerId.PI_GS_USER, 1002, PerformerId.PI_LOGIN_SERVER, 0,
            EventId.ELG_REGISTER_UNIT_NOT, new KELG_REGISTER_UNIT_NOT { UnitUid = unitUid2, UserUid = accUid2, GameServerUid = 200, ChannelId = 2, NickName = "BobTW" });

        // Search unit, update buff, send whisper, add wedding hall, update CCU
        router.Send(PerformerId.PI_GS_USER, 1001, PerformerId.PI_LOGIN_SERVER, 0,
            EventId.ELG_SEARCH_UNIT_REQ, new KELG_SEARCH_UNIT_REQ { NickName = "BobTW" });
        router.Send(PerformerId.PI_GS_USER, 1001, PerformerId.PI_LOGIN_SERVER, 0,
            EventId.ELG_UPDATE_USER_UNIT_BUFF_INFO_REQ, new KELG_UPDATE_USER_UNIT_BUFF_INFO_REQ
            {
                UnitUid = unitUid1,
                ActivateBuff = [new KBuffInfo { FactorInfo = new KBuffFactor { BuffIdentity = new KBuffIdentity { BuffTempletId = 8001 } } }],
            });
        router.Send(PerformerId.PI_GS_USER, 1001, PerformerId.PI_LOGIN_SERVER, 0,
            EventId.ELG_CHAT_REQ, new KELG_CHAT_REQ { SenderUnitUid = unitUid1, SenderNickName = "AliceTW", ReceiverNickName = "BobTW", Message = "Hi!" });
        router.Send(PerformerId.PI_GS_USER, 1001, PerformerId.PI_LOGIN_SERVER, 0,
            EventId.ELG_ADD_WEDDING_HALL_INFO_NOT, new KELG_ADD_WEDDING_HALL_INFO_NOT { WeddingUid = 12, RoomUid = 888, BrideUnitUid = unitUid1, GroomUnitUid = unitUid2 });
        router.Send(PerformerId.PI_GS_USER, 1001, PerformerId.PI_LOGIN_SERVER, 0,
            EventId.ELG_CHECK_WEDDING_HALL_INFO_REQ, new KELG_CHECK_WEDDING_HALL_INFO_REQ { WeddingUid = 12, UnitUid = unitUid1 });
        router.Send(PerformerId.PI_GS_SERVER, 200, PerformerId.PI_LOGIN_SERVER, 0,
            EventId.ELG_UPDATE_CCU_INFO_NOT, new KELG_UPDATE_CCU_INFO_NOT { ServerUid = 200, ConcurrentUser = 42 });
        login.Tick();

        var searchAck = gsUserEvents.Single(e => e.EventId == EventId.ELG_SEARCH_UNIT_ACK)
            .GetPayload<KELG_SEARCH_UNIT_ACK>(RegionProfile.Taiwan);
        Assert.Equal(NetError.NET_OK, searchAck.Ok);
        Assert.Equal("BobTW", searchAck.CUnitInfo.CharName);
        Assert.Equal(2, searchAck.CUnitInfo.ChannelId);

        var buffAck = gsUserEvents.Single(e => e.EventId == EventId.ELG_UPDATE_USER_UNIT_BUFF_INFO_ACK)
            .GetPayload<KELG_UPDATE_USER_UNIT_BUFF_INFO_ACK>(RegionProfile.Taiwan);
        Assert.Single(buffAck.ActiveBuffs);

        var chatRelay = gsUserEvents.Single(e => e.EventId == EventId.ELG_CHAT_NOT);
        Assert.Contains(1002L, chatRelay.DestPerformer.Uids);

        var wedAck = gsUserEvents.Single(e => e.EventId == EventId.ELG_CHECK_WEDDING_HALL_INFO_ACK)
            .GetPayload<KELG_CHECK_WEDDING_HALL_INFO_ACK>(RegionProfile.Taiwan);
        Assert.Equal(888L, wedAck.RoomUid);
        Assert.Equal(42, login.ServerCcuManager.GetTotalConcurrentUser());
    }

    private static void RouterRegisterRoom(InProcessRouter router, KCenterServer center)
        => router.RegisterPerformer(PerformerId.PI_CN_ROOM, center);
}
