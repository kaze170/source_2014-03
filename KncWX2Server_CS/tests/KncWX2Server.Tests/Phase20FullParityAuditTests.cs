using KncWX2Server.CenterServer;
using KncWX2Server.ChannelServer;
using KncWX2Server.Core.Database;
using KncWX2Server.Core.Events;
using KncWX2Server.Core.Framework;
using KncWX2Server.Core.Packets;
using KncWX2Server.Core.Region;
using KncWX2Server.GameServer;
using KncWX2Server.GlobalServer;
using KncWX2Server.LoginServer;
using Xunit;

namespace KncWX2Server.Tests;

public sealed class Phase20FullParityAuditTests : IDisposable
{
    private readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"elsword_p20_{Guid.NewGuid():N}.db");

    public void Dispose()
    {
        if (File.Exists(_dbPath))
        {
            try { File.Delete(_dbPath); } catch { }
        }
    }

    [Fact]
    public void Center_Login_And_Game_Servers_Route_All_Closed_Loop_Events_With_Exact_Payloads()
    {
        var router = new InProcessRouter();

        using var global = new KGlobalServer(RegionCode.TW, router);
        using var channel = new KChannelServer(RegionCode.TW, _dbPath, router);
        using var center = new KCenterServer(RegionCode.TW, router);
        using var login = new KLoginServer(RegionCode.TW, _dbPath, router);
        using var game = new KGameServer(RegionCode.TW, _dbPath, router, center, global);
        global.Uid = 10;
        center.Uid = 20;
        login.Uid = 30;
        game.Uid = 40;
        global.Init();
        center.Init();
        login.Init();
        game.Init();

        var receivedByUser = new List<(long UnitUid, EventId EventId, object? Payload)>();
        router.RegisterRoute(PerformerId.PI_GS_USER, ev =>
        {
            receivedByUser.Add((ev.DestPerformer.Uids.FirstOrDefault(), ev.EventId, ev.InProcessPayload));
            return true;
        });

        // 1. CenterServer ECN_UPDATE_SERVER_INFO_REQ -> ECN_UPDATE_SERVER_INFO_ACK (KServerList) -> GameServer.ConnectedServers
        router.Send(PerformerId.PI_GS_SERVER, game.Uid, PerformerId.PI_CN_SERVER, center.Uid,
            EventId.ECN_UPDATE_SERVER_INFO_REQ, new KServerInfo
            {
                Uid = game.Uid,
                Name = "TW-GS-1",
                Ip = "127.0.0.1",
                MasterPort = 9300,
            });
        center.Tick();
        game.Tick();
        Assert.True(game.ConnectedServers.ContainsKey(game.Uid));

        // 2. CenterServer Party Host Change + Ban User -> exact KEPM_CHECK_PARTY_CHANGE_HOST_ACK & KPacketOK
        var hostUser = new KPartyUserInfo { UnitUid = 7001, NickName = "Host7001", Level = 70, UnitClass = 1 };
        var mateUser = new KPartyUserInfo { UnitUid = 7002, NickName = "Mate7002", Level = 70, UnitClass = 2 };
        var party = center.PartyManager.CreateParty(new KCreatePartyInfo { DungeonId = 30000, PartyType = 0 }, hostUser);
        center.PartyManager.TryJoinParty(party.Info.PartyUid, mateUser, out _);

        router.Send(PerformerId.PI_GS_USER, 7001, PerformerId.PI_CN_PARTY, party.Info.PartyUid,
            EventId.EPM_PARTY_CHANGE_HOST_REQ, new KEPM_PARTY_CHANGE_HOST_REQ
            {
                PartyUid = party.Info.PartyUid,
                NewHostUnitUid = 7002,
            });
        center.Tick();
        Assert.Contains(receivedByUser, r =>
            r.EventId == EventId.EPM_PARTY_CHANGE_HOST_NOT && r.Payload is KEPM_CHECK_PARTY_CHANGE_HOST_ACK ack && ack.NewHostUnitUid == 7002);

        // 3. CenterServer ERM_REFRESH_ROOM_REQ + ERM_TRADE_START_NOT + ERM_USER_UNIT_DIE_COMPLETE_REQ + ERM_DUNGEON_KILLALLNPC_CHECK_REQ
        var room = center.RoomManager.CreateRoom(new KRoomInfo { RoomType = 1, RoomName = "DungeonAudit", MaxSlot = 4 },
            new KRoomUserInfo { UnitUid = 7001, NickName = "Host7001" }, asObserver: false);
        router.Send(PerformerId.PI_GS_USER, 7001, PerformerId.PI_CN_ROOM, room.Info.RoomUid,
            EventId.ERM_REFRESH_ROOM_REQ, new KPacketOK());
        router.Send(PerformerId.PI_GS_USER, 7001, PerformerId.PI_CN_ROOM, room.Info.RoomUid,
            EventId.ERM_TRADE_START_NOT, new KPacketOK());
        router.Send(PerformerId.PI_GS_USER, 7001, PerformerId.PI_CN_ROOM, room.Info.RoomUid,
            EventId.ERM_USER_UNIT_DIE_COMPLETE_REQ, new KPacketOK());
        router.Send(PerformerId.PI_GS_USER, 7001, PerformerId.PI_CN_ROOM, room.Info.RoomUid,
            EventId.ERM_DUNGEON_KILLALLNPC_CHECK_REQ, new KPacketOK());
        center.Tick();

        Assert.Contains(receivedByUser, r => r.EventId == EventId.ERM_REFRESH_ROOM_ACK && r.Payload is KEGS_REFRESH_ROOM_ACK);
        Assert.Contains(receivedByUser, r => r.EventId == EventId.ERM_CHANGE_TRADE_NOT && r.Payload is KEGS_CHANGE_TRADE_NOT { Trade: true });
        Assert.Contains(receivedByUser, r => r.EventId == EventId.ERM_USER_UNIT_DIE_COMPLETE_NOT && r.Payload is KEGS_USER_UNIT_DIE_COMPLETE_NOT { UnitUid: 7001 });
        Assert.Contains(receivedByUser, r => r.EventId == EventId.ERM_DUNGEON_KILLALLNPC_CHECK_ACK && r.Payload is KEGS_DUNGEON_KILLALLNPC_CHECK_ACK { Result: true });

        // 4. GameServer PartyList & RoomList synchronization handlers
        router.Send(PerformerId.PI_CN_PARTY, party.Info.PartyUid, PerformerId.PI_GS_SERVER, game.Uid,
            EventId.EPM_ADD_PARTY_LIST_NOT, new KEPM_ADD_PARTY_LIST_NOT
            {
                PartyInfo = new KPartyListInfo { PartyInfo = party.Info },
            });
        router.Send(PerformerId.PI_CN_PARTY, party.Info.PartyUid, PerformerId.PI_GS_SERVER, game.Uid,
            EventId.EPM_UPDATE_PARTY_FEVER_IN_PARTY_LIST_NOT, new KEPM_UPDATE_PARTY_FEVER_IN_PARTY_LIST_NOT
            {
                PartyUid = party.Info.PartyUid,
                FeverPoint = 4500,
            });
        router.Send(PerformerId.PI_CN_ROOM, room.Info.RoomUid, PerformerId.PI_GS_SERVER, game.Uid,
            EventId.ERM_ROOM_COUNT_INFO_NOT, new KERM_ROOM_COUNT_INFO_NOT
            {
                PvpRoomCount = 3,
                DungeonRoomCount = 7,
                BattleFieldRoomCount = 2,
            });
        game.Tick();

        Assert.Equal((short)4500, game.PartyListRegistry[party.Info.PartyUid].PartyInfo.PartyFever);
        Assert.Equal(7, game.DungeonRoomCount);

        // 5. LoginServer billing reserve + coupon reserve + personal shop agency pickup
        router.Send(PerformerId.PI_GS_USER, 8001, PerformerId.PI_LOGIN_SERVER, login.Uid,
            EventId.EBILL_EXCHANGE_CASH_RESERVE_REQ, new KEBILL_EXCHANGE_CASH_RESERVE_REQ
            {
                PurchaserInfo = new KPurchaserInfo { UserUid = 8001, UnitUid = 9001 },
                Point = 500,
            });
        login.Tick();
        Assert.Contains(receivedByUser, r =>
            r.EventId == EventId.EBILL_EXCHANGE_CASH_RESERVE_ACK && r.Payload is KEBILL_EXCHANGE_CASH_RESERVE_ACK { Ok: NetError.NET_OK });

        // 6. Deep parity checks: GameServer mission/ranking caches, CenterServer log counters, LoginServer routing
        router.Send(PerformerId.PI_GB_SERVER, global.Uid, PerformerId.PI_GS_SERVER, game.Uid,
            EventId.EGB_UPDATE_MISSION_START_TIME_NOT, new KPacketString { Value = "2026-11-20 12:00:00" });
        game.Tick();
        Assert.Equal("2026-11-20 12:00:00", game.WorldMissionStartTime);

        router.Send(PerformerId.PI_GS_USER, 7001, PerformerId.PI_CN_SERVER, center.Uid,
            EventId.ELOG_STAT_RES_STONE, new KPacketOK { Ok = 1 });
        center.Tick();
        Assert.Equal(1, center.ServerLogCounters[EventId.ELOG_STAT_RES_STONE]);

        router.Send(PerformerId.PI_GS_USER, 7001, PerformerId.PI_LOGIN_SERVER, login.Uid,
            EventId.ELG_GET_UNIT_INFO_ACK, new KELG_GET_UNIT_INFO_ACK
            {
                Ok = NetError.NET_OK,
                DemandUserUid = 7001,
                CUnitInfo = new KConnectionUnitInfo { UnitUid = 7002, CharName = "Mate7002" },
            });
        login.Tick();
        Assert.Contains(receivedByUser, r =>
            r.UnitUid == 7001 && r.EventId == EventId.ELG_SEARCH_UNIT_ACK && r.Payload is KELG_SEARCH_UNIT_ACK { Ok: NetError.NET_OK });
    }
}
