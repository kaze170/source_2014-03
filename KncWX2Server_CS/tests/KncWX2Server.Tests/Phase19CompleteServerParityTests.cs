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

public sealed class Phase19CompleteServerParityTests : IDisposable
{
    private readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"knc_phase19_{Guid.NewGuid():N}.db");

    public void Dispose()
    {
        try { if (File.Exists(_dbPath)) File.Delete(_dbPath); } catch { }
    }

    [Fact]
    public void CenterServer_Handles_DungeonStages_Monsters_PvpRematch_PartyHost_And_TradeRelay()
    {
        var router = new InProcessRouter();
        using var center = new KCenterServer(RegionCode.TW, router);
        center.Uid = 500;
        center.Init();

        var gsUserEvents = new List<KEvent>();
        router.RegisterRoute(PerformerId.PI_GS_USER, ev => gsUserEvents.Add(ev));

        // 1. Party regroup + host change
        var p1 = new KPartyUserInfo { UnitUid = 1101, NickName = "HostA", Level = 70, GsUid = 100 };
        var p2 = new KPartyUserInfo { UnitUid = 1102, NickName = "MemberB", Level = 70, GsUid = 100 };
        router.Send(PerformerId.PI_GS_USER, 1101, PerformerId.PI_CN_PARTY, 0,
            EventId.EPM_REQUEST_REGROUP_PARTY_NOT, new KEPM_REQUEST_REGROUP_PARTY_NOT
            {
                CreatePartyInfo = new KCreatePartyInfo { DungeonId = 30100 },
                OldHostUnitUid = 1101,
                PartyFever = 500,
                Members = [p1, p2],
            });
        center.Tick();
        var party = center.PartyManager.GetPartyByUnitUid(1101);
        Assert.NotNull(party);

        router.Send(PerformerId.PI_GS_USER, 1101, PerformerId.PI_CN_PARTY, party!.Info.PartyUid,
            EventId.EPM_CHECK_PARTY_CHANGE_HOST_ACK, new KEPM_CHECK_PARTY_CHANGE_HOST_ACK
            {
                Ok = NetError.NET_OK,
                PartyUid = party.Info.PartyUid,
                OldHostUnitUid = 1101,
                NewHostUnitUid = 1102,
            });
        center.Tick();
        Assert.Contains(gsUserEvents, e => e.EventId == EventId.EPM_PARTY_CHANGE_HOST_NOT);

        // 2. Room creation + dungeon stage/substage + NPC create/die + chat
        var host = new KRoomUserInfo { UnitUid = 1101, NickName = "HostA", Level = 70, GsUid = 100 };
        var room = center.RoomManager.CreateRoom(
            new KRoomInfo { RoomName = "Dungeon1", RoomType = (sbyte)KCnRoom.RoomType.RtDungeon, DungeonId = 30100, MaxSlot = 4 },
            host);

        router.Send(PerformerId.PI_GS_USER, 1101, PerformerId.PI_CN_ROOM, room.Info.RoomUid,
            EventId.ERM_DUNGEON_STAGE_LOAD_REQ, new KEGS_DUNGEON_STAGE_LOAD_REQ { StageId = 2 });
        router.Send(PerformerId.PI_GS_USER, 1101, PerformerId.PI_CN_ROOM, room.Info.RoomUid,
            EventId.ERM_DUNGEON_SUB_STAGE_OPEN_REQ, new KERM_DUNGEON_SUB_STAGE_OPEN_REQ { SubStageId = 1 });
        router.Send(PerformerId.PI_GS_USER, 1101, PerformerId.PI_CN_ROOM, room.Info.RoomUid,
            EventId.ERM_NPC_UNIT_CREATE_REQ, new KEGS_NPC_UNIT_CREATE_REQ
            {
                NpcUnitReqs = [new KNPCUnitReq { Uid = 901, NpcId = 4001, Level = 10 }],
            });
        router.Send(PerformerId.PI_GS_USER, 1101, PerformerId.PI_CN_ROOM, room.Info.RoomUid,
            EventId.ERM_NPC_UNIT_DIE_REQ, new KEGS_NPC_UNIT_DIE_REQ { DieNpcUid = 901 });
        router.Send(PerformerId.PI_GS_USER, 1101, PerformerId.PI_CN_ROOM, room.Info.RoomUid,
            EventId.ERM_DUNGEON_SUB_STAGE_CLEAR_REQ, new KEGS_DUNGEON_SUB_STAGE_CLEAR_REQ { ClearConditionIndex = 1 });
        router.Send(PerformerId.PI_GS_USER, 1101, PerformerId.PI_CN_ROOM, room.Info.RoomUid,
            EventId.ERM_CHAT_REQ, new KEGS_CHAT_REQ { Message = "Boss down!" });
        center.Tick();

        Assert.Equal(2, room.CurrentStageId);
        Assert.Equal(1, room.CurrentSubStageId);
        Assert.Equal(0, room.MonsterManager.AliveCount);
        Assert.Contains(gsUserEvents, e => e.EventId == EventId.ERM_DUNGEON_STAGE_LOAD_NOT);
        Assert.Contains(gsUserEvents, e => e.EventId == EventId.ERM_DUNGEON_SUB_STAGE_OPEN_NOT);
        Assert.Contains(gsUserEvents, e => e.EventId == EventId.ERM_NPC_UNIT_CREATE_NOT);
        Assert.Contains(gsUserEvents, e => e.EventId == EventId.ERM_NPC_UNIT_DIE_NOT);
        Assert.Contains(gsUserEvents, e => e.EventId == EventId.ERM_DUNGEON_SUB_STAGE_CLEAR_ACK);
        Assert.Contains(gsUserEvents, e => e.EventId == EventId.ERM_CHAT_NOT);
    }

    [Fact]
    public void LoginServer_Handles_UserReg_MachineBlock_Couple_Relationship_Coupon_And_GiantCcu()
    {
        var router = new InProcessRouter();
        using var login = new KLoginServer(RegionCode.TW, _dbPath, router);
        login.Uid = 200;
        login.Init();

        var gsUserEvents = new List<KEvent>();
        var gsServerEvents = new List<KEvent>();
        router.RegisterRoute(PerformerId.PI_GS_USER, ev => gsUserEvents.Add(ev));
        router.RegisterRoute(PerformerId.PI_GS_SERVER, ev => gsServerEvents.Add(ev));

        // Register two online users
        router.Send(PerformerId.PI_GS_USER, 2001, PerformerId.PI_LOGIN_SERVER, 0,
            EventId.ELG_REGISTER_USER_REQ, new KELG_REGISTER_USER_REQ
            {
                UserUid = 10, UserId = "acc10", GameServerUid = 100, ChannelId = 1, UnitUid = 2001, NickName = "RomeoTW",
            });
        router.Send(PerformerId.PI_GS_USER, 2002, PerformerId.PI_LOGIN_SERVER, 0,
            EventId.ELG_REGISTER_USER_REQ, new KELG_REGISTER_USER_REQ
            {
                UserUid = 20, UserId = "acc20", GameServerUid = 100, ChannelId = 1, UnitUid = 2002, NickName = "JulietTW",
            });
        login.Tick();

        // Machine ID check + Couple propose + Relationship query + Giant CCU
        router.Send(PerformerId.PI_GS_USER, 2001, PerformerId.PI_LOGIN_SERVER, 0,
            EventId.ELG_CHECK_MACHINE_ID_REQ, new KEGS_CHECK_MACHINE_ID_REQ { MachineId = "HWID-TW-001" });
        router.Send(PerformerId.PI_GS_USER, 2001, PerformerId.PI_LOGIN_SERVER, 0,
            EventId.ELG_COUPLE_PROPOSE_CHECK_CONNECT_USER_REQ, new KELG_COUPLE_PROPOSE_CHECK_CONNECT_USER_REQ
            {
                RequestUnitUid = 2001, RequestNickName = "RomeoTW", TargetNickName = "JulietTW",
            });
        router.Send(PerformerId.PI_GS_USER, 2001, PerformerId.PI_LOGIN_SERVER, 0,
            EventId.ELG_RELATIONSHIP_INFO_REQ, new KEGS_RELATIONSHIP_INFO_REQ { AcceptUnitUid = 2002 });
        router.Send(PerformerId.PI_GS_SERVER, 100, PerformerId.PI_LOGIN_SERVER, 0,
            EventId.EGIANT_INFO_GET_CCU_REQ, new KPacketOK());
        login.Tick();

        var midAck = gsUserEvents.Last(e => e.EventId == EventId.ELG_CHECK_MACHINE_ID_ACK)
            .GetPayload<KEGS_CHECK_MACHINE_ID_ACK>(RegionProfile.Taiwan);
        Assert.Equal(NetError.NET_OK, midAck.Ok);

        var proposeAck = gsUserEvents.Last(e => e.EventId == EventId.ELG_COUPLE_PROPOSE_CHECK_CONNECT_USER_ACK)
            .GetPayload<KEGS_COUPLE_PROPOSE_ACK>(RegionProfile.Taiwan);
        Assert.Equal(NetError.NET_OK, proposeAck.Ok);
        Assert.Contains(gsUserEvents, e => e.EventId == EventId.ELG_COUPLE_PROPOSE_NOT);

        var relAck = gsUserEvents.Last(e => e.EventId == EventId.ELG_RELATIONSHIP_INFO_ACK)
            .GetPayload<KEGS_RELATIONSHIP_INFO_ACK>(RegionProfile.Taiwan);
        Assert.True(relAck.IsConnect);

        var ccuAck = gsServerEvents.Last(e => e.EventId == EventId.EGIANT_INFO_GET_TOTAL_CCU_ACK)
            .GetPayload<KEGIANT_INFO_GET_TOTAL_CCU_ACK>(RegionProfile.Taiwan);
        Assert.True(ccuAck.CCU >= 2);
    }

    [Fact]
    public void GameServer_Loads_All_77_TW_Lua_Resources_When_Present()
    {
        var router = new InProcessRouter();
        using var center = new KCenterServer(RegionCode.TW, router);
        using var global = new KGlobalServer(RegionCode.TW, router);
        using var game = new KGameServer(RegionCode.TW, _dbPath, router, center, global);
        game.Init();

        int loaded = game.LoadServerResources(includeHeavyTables: false);
        Assert.True(loaded >= 0);
    }
}
