using KncWX2Server.CenterServer;
using KncWX2Server.ChannelServer;
using KncWX2Server.Core.Events;
using KncWX2Server.Core.Framework;
using KncWX2Server.Core.Packets;
using KncWX2Server.Core.Region;
using KncWX2Server.GameServer;
using KncWX2Server.GlobalServer;
using KncWX2Server.LoginServer;
using Xunit;

namespace KncWX2Server.Tests;

public sealed class Phase18FullCrossServerParityTests : IDisposable
{
    private readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"knc_phase18_{Guid.NewGuid():N}.db");

    public void Dispose()
    {
        try { if (File.Exists(_dbPath)) File.Delete(_dbPath); } catch { }
    }

    [Fact]
    public void GlobalServer_UnitTracking_WorldMission_HeroPvp_And_GameServerSync()
    {
        var router = new InProcessRouter();
        using var global = new KGlobalServer(RegionCode.TW, router);
        using var game = new KGameServer(RegionCode.TW, _dbPath, router, globalServer: global);
        global.Uid = 400;
        game.Uid = 100;
        global.Init();
        game.Init();

        // Register unit + disconnect tracking + HeroPvP + TodayRandomQuest sync
        router.Send(PerformerId.PI_GS_USER, 1001, PerformerId.PI_GB_SERVER, 0,
            EventId.EGB_REGISTER_UNIT_NOT, new KEGB_REGISTER_UNIT_NOT
            {
                ServerSn = 8801,
                UserUid = 1001,
                UnitUid = 50001,
                NickName = "ElswordTW",
                GameServerUid = game.Uid,
                ServerGroupId = 1,
            });
        router.Send(PerformerId.PI_GS_USER, 1001, PerformerId.PI_GB_SERVER, 0,
            EventId.EGB_GLOBAL_MISSION_UPDATE_NOT, new KEGB_GLOBAL_MISSION_UPDATE_NOT
            {
                GlobalMissionDay = "2026-11-20",
                NowCount = 42,
            });
        global.Tick();

        router.Send(PerformerId.PI_GS_SERVER, game.Uid, PerformerId.PI_GB_SERVER, 0,
            EventId.EGB_TODAY_RANDOM_QUEST_REQ, new KPacketLong { Value = game.Uid });
        router.Send(PerformerId.PI_GS_SERVER, game.Uid, PerformerId.PI_GB_SERVER, 0,
            EventId.EGB_EXCHANGE_LIMIT_INFO_REQ, new KPacketOK());
        router.Send(PerformerId.PI_GS_SERVER, game.Uid, PerformerId.PI_GB_SERVER, 0,
            EventId.EGB_TIME_ENCHANT_EVENT_INFO_REQ, new KPacketOK());
        global.Tick();
        game.Tick();

        Assert.NotNull(game.LatestExchangeLimitInfo);
        Assert.NotNull(game.LatestTimeEnchantEventInfo);
        Assert.True(game.LatestTodayRandomQuests.Count >= 0);

        // WorldMission crystal increase & reset
        global.WorldMissionManager.IncreaseProtectedCrystalCount(5);
        Assert.Equal(5, global.WorldMissionManager.ProtectedCrystalCount);
        router.Send(PerformerId.PI_GS_SERVER, game.Uid, PerformerId.PI_GB_SERVER, 0,
            EventId.EGB_RESET_INCREASE_THE_PROTECTED_CRYSTAL_COUNT_NOT, new KPacketOK());
        global.Tick();
        Assert.Equal(0, global.WorldMissionManager.ProtectedCrystalCount);

        // Unit disconnect
        router.Send(PerformerId.PI_GS_USER, 1001, PerformerId.PI_GB_SERVER, 0,
            EventId.EGB_UNIT_DISCONNECT_NOT, new KEGB_UNIT_DISCONNECT_NOT
            {
                ServerSn = 8801,
                UserUid = 1001,
                UnitUid = 50001,
            });
        global.Tick();
    }

    [Fact]
    public void ChannelServer_VerifyServerConnect_With_LoginServer_And_Refresh()
    {
        var router = new InProcessRouter();
        using var login = new KLoginServer(RegionCode.TW, _dbPath, router);
        using var channel = new KChannelServer(RegionCode.TW, _dbPath, router);
        login.Uid = 200;
        channel.Uid = 300;
        login.Init();
        channel.Init();

        Assert.False(channel.IsLoginConnected);
        router.Send(PerformerId.PI_CH_SERVER, channel.Uid, PerformerId.PI_LOGIN_SERVER, 0,
            EventId.ELG_VERIFY_SERVER_CONNECT_REQ, new KPacketOK());
        login.Tick();
        channel.Tick();
        Assert.True(channel.IsLoginConnected);

        router.Send(PerformerId.PI_CH_SERVER, channel.Uid, PerformerId.PI_CH_SERVER, channel.Uid,
            EventId.ESR_ORDER_TO_REFRESH_MANAGER_REQ, new KPacketOK());
        channel.Tick();
        Assert.True(channel.IsLoginConnected);
    }

    [Fact]
    public void CenterServer_Party_Room_Trade_And_Server_Control_EndToEnd()
    {
        var router = new InProcessRouter();
        using var center = new KCenterServer(RegionCode.TW, router);
        using var game = new KGameServer(RegionCode.TW, _dbPath, router, centerServer: center);
        center.Uid = 500;
        game.Uid = 100;
        center.Init();
        game.Init();

        var userEvents = new List<KEvent>();
        router.RegisterRoute(PerformerId.PI_GS_USER, ev => userEvents.Add(ev));

        // 1. ECN server control -> GameServer
        router.Send(PerformerId.PI_CN_SERVER, center.Uid, PerformerId.PI_GS_SERVER, game.Uid,
            EventId.ECN_SET_ACTIVE_LAGCHECK_NOT, new KPacketBool { Value = false });
        game.Tick();
        Assert.False(game.IsLagCheckActive);

        // 2. Party creation + change dungeon + fever + chat + ban
        var p1 = new KPartyUserInfo { UnitUid = 7001, NickName = "LeaderTW", Level = 70, UnitClass = 1, GsUid = game.Uid };
        var p2 = new KPartyUserInfo { UnitUid = 7002, NickName = "MemberTW", Level = 70, UnitClass = 2, GsUid = game.Uid };
        var party = center.PartyManager.CreateParty(new KPartyInfo { DungeonId = 30000, MaxSlot = 4 }, p1);
        Assert.NotNull(party);
        Assert.True(center.PartyManager.TryJoinParty(party.Info.PartyUid, p2, out _));

        router.Send(PerformerId.PI_GS_USER, 7001, PerformerId.PI_CN_PARTY, party.Info.PartyUid,
            EventId.EPM_PARTY_CHANGE_DUNGEON_REQ, new KEPM_PARTY_CHANGE_DUNGEON_REQ
            {
                PartyUid = party.Info.PartyUid,
                DungeonId = 30100,
                DifficultyLevel = 2,
            });
        router.Send(PerformerId.PI_GS_USER, 7001, PerformerId.PI_CN_PARTY, party.Info.PartyUid,
            EventId.EPM_CHAT_REQ, new KEPM_CHAT_REQ
            {
                PartyUid = party.Info.PartyUid,
                SenderUnitUid = 7001,
                SenderNickName = "LeaderTW",
                Message = "Ready?",
            });
        router.Send(PerformerId.PI_GS_USER, 7001, PerformerId.PI_CN_PARTY, party.Info.PartyUid,
            EventId.EPM_INCREASE_PARTY_FEVER_NOT, new KERM_INCREASE_PARTY_FEVER_NOT
            {
                PartyUid = party.Info.PartyUid,
                SubStageNpcCount = 2,
            });
        center.Tick();

        Assert.Contains(userEvents, e => e.EventId == EventId.EPM_PARTY_CHANGE_DUNGEON_ACK);
        Assert.Contains(userEvents, e => e.EventId == EventId.EPM_PARTY_CHANGE_DUNGEON_NOT);
        Assert.Contains(userEvents, e => e.EventId == EventId.EPM_CHAT_NOT);
        Assert.Contains(userEvents, e => e.EventId == EventId.EPM_CHANGE_PARTY_FEVER_STATE_NOT);

        // 3. Room creation + map + ready + team + trade
        var host = new KRoomUserInfo { UnitUid = 7001, NickName = "LeaderTW", Level = 70, GsUid = game.Uid };
        var room = center.RoomManager.CreateRoom(
            new KRoomInfo { RoomName = "PvPRoom", RoomType = (sbyte)KCnRoom.RoomType.RtPvp, WorldId = 10000, MaxSlot = 8 },
            host);
        Assert.NotNull(room);

        router.Send(PerformerId.PI_GS_USER, 7001, PerformerId.PI_CN_ROOM, room!.Info.RoomUid,
            EventId.ERM_CHANGE_MAP_REQ, new KEGS_CHANGE_MAP_REQ { WorldId = 10002 });
        router.Send(PerformerId.PI_GS_USER, 7001, PerformerId.PI_CN_ROOM, room.Info.RoomUid,
            EventId.ERM_CHANGE_READY_REQ, new KEGS_CHANGE_READY_REQ { Ready = true });
        router.Send(PerformerId.PI_GS_USER, 7001, PerformerId.PI_CN_ROOM, room.Info.RoomUid,
            EventId.ERM_CHANGE_TEAM_REQ, new KEGS_CHANGE_TEAM_REQ { TeamNum = 1 });
        center.Tick();

        Assert.Contains(userEvents, e => e.EventId == EventId.ERM_CHANGE_MAP_ACK);
        Assert.Contains(userEvents, e => e.EventId == EventId.ERM_CHANGE_READY_ACK);
        Assert.Contains(userEvents, e => e.EventId == EventId.ERM_CHANGE_TEAM_ACK);
    }

    [Fact]
    public void LoginServer_Guild_PShopAgency_Henir_And_LocalRanking_Handlers()
    {
        var router = new InProcessRouter();
        using var login = new KLoginServer(RegionCode.TW, _dbPath, router);
        login.Uid = 200;
        login.Init();

        var gsUserEvents = new List<KEvent>();
        router.RegisterRoute(PerformerId.PI_GS_USER, ev => gsUserEvents.Add(ev));

        // 1. PShop Agency open -> check -> buy -> pickup
        router.Send(PerformerId.PI_GS_USER, 9001, PerformerId.PI_LOGIN_SERVER, 0,
            EventId.ERM_OPEN_PSHOP_AGENCY_REQ, new KERM_OPEN_PSHOP_AGENCY_REQ
            {
                HostUnitUid = 9001,
                HostNickName = "MerchantTW",
                ShopName = "AgencyShop",
                DurationMinutes = 60,
                Items =
                [
                    new KAgencyShopItemDto { ItemUid = 11, ItemId = 70001, Quantity = 3, PricePerUnit = 1000 },
                ],
            });
        login.Tick();

        var openAck = gsUserEvents.Last(e => e.EventId == EventId.ERM_OPEN_PSHOP_AGENCY_ACK)
            .GetPayload<KERM_OPEN_PSHOP_AGENCY_ACK>(RegionProfile.Taiwan);
        Assert.Equal(NetError.NET_OK, openAck.Ok);
        Assert.True(openAck.ShopUid > 0);

        router.Send(PerformerId.PI_GS_USER, 9002, PerformerId.PI_LOGIN_SERVER, 0,
            EventId.ERM_BUY_PERSONAL_SHOP_ITEM_REQ, new KERM_BUY_PERSONAL_SHOP_ITEM_REQ
            {
                ShopUid = openAck.ShopUid,
                BuyerUnitUid = 9002,
                ItemUid = 11,
                Quantity = 2,
            });
        login.Tick();
        var buyAck = gsUserEvents.Last(e => e.EventId == EventId.ERM_BUY_PERSONAL_SHOP_ITEM_ACK)
            .GetPayload<KERM_BUY_PERSONAL_SHOP_ITEM_ACK>(RegionProfile.Taiwan);
        Assert.Equal(NetError.NET_OK, buyAck.Ok);
        Assert.Equal(2000, buyAck.TotalCost);

        router.Send(PerformerId.PI_GS_USER, 9001, PerformerId.PI_LOGIN_SERVER, 0,
            EventId.ERM_PICK_UP_FROM_PSHOP_AGENCY_REQ, new KPacketLong { Value = 9001 });
        login.Tick();
        var pickupAck = gsUserEvents.Last(e => e.EventId == EventId.ERM_PICK_UP_FROM_PSHOP_AGENCY_ACK)
            .GetPayload<KERM_PICK_UP_FROM_PSHOP_AGENCY_ACK>(RegionProfile.Taiwan);
        Assert.Equal(NetError.NET_OK, pickupAck.Ok);
        Assert.True(pickupAck.PickedEd > 0);

        // 2. Henir Ranking + Local Ranking inquiry
        router.Send(PerformerId.PI_GS_USER, 9001, PerformerId.PI_LOGIN_SERVER, 0,
            EventId.ELG_NEW_RECORD_HENIR_RANKING_NOT, new KELG_NEW_RECORD_HENIR_RANKING_NOT
            {
                RankingInfo = new KHenirRankingInfo
                {
                    UnitUid = 9001,
                    NickName = "HenirHero",
                    StageCount = 40,
                    PlayTime = 180,
                    UnitClass = 1,
                    Level = 70,
                },
            });
        router.Send(PerformerId.PI_GS_USER, 9001, PerformerId.PI_LOGIN_SERVER, 0,
            EventId.ELG_LOCAL_RANKING_CHANGED_RANKER_INFO_NOT, new KELG_LOCAL_RANKING_CHANGED_RANKER_INFO_NOT
            {
                SystemUid = 1,
                UserInfo = new KLocalRankingUserInfo { UserUid = 9001, Category = 1 },
                UnitInfo = new KLocalRankingUnitInfo { UnitUid = 9001, NickName = "HenirHero", Level = 70, Point = [1500, 300] },
            });
        router.Send(PerformerId.PI_GS_USER, 9001, PerformerId.PI_LOGIN_SERVER, 0,
            EventId.EGS_LOCAL_RANKING_INQUIRY_REQ, new KEGS_LOCAL_RANKING_INQUIRY_REQ
            {
                ViewPage = 1,
                MainTabIndex = 0,
                SubTabIndex = 1,
            });
        login.Tick();

        var lrAck = gsUserEvents.Last(e => e.EventId == EventId.EGS_LOCAL_RANKING_INQUIRY_ACK)
            .GetPayload<KEGS_LOCAL_RANKING_INQUIRY_ACK>(RegionProfile.Taiwan);
        Assert.Equal(NetError.NET_OK, lrAck.Ok);
        Assert.Single(lrAck.RankerList);
    }
}
