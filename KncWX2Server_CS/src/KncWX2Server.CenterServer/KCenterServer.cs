using System.Collections.Concurrent;
using System.Diagnostics;
using KncWX2Server.Core.Events;
using KncWX2Server.Core.Framework;
using KncWX2Server.Core.Logging;
using KncWX2Server.Core.Packets;
using KncWX2Server.Core.Region;
using KncWX2Server.Core.Scripting;
using MoonSharp.Interpreter;

namespace KncWX2Server.CenterServer;

public sealed class KCnSimLayer : KSimLayer { }

public sealed class KCnNetLayer : KNetLayer
{
    private readonly KCenterServer _server;

    public KCnNetLayer(KCenterServer server)
    {
        _server = server;
        Port = server.Region.DefaultCenterPort;
    }

    public override bool Init()
        => TcpServer.Start(Port, session => { session.ReserveDestroy(); return Task.CompletedTask; }, () => _server.Region);
}

/// <summary>
/// Managed port of KCenterServer (128 C++ files).
/// Authoritative coordinator for Rooms (PvP, Dungeon, BattleField, Training) and Parties.
/// </summary>
public sealed class KCenterServer : KBaseServer
{
    private readonly KCnSimLayer _simLayer = new();
    private readonly KCnNetLayer _netLayer;
    private CancellationTokenSource? _tickCts;
    private Task? _tickTask;

    public KCnRoomManager RoomManager { get; } = new();
    public KGameSysValTable GameSysValTable { get; } = new();
    public KCnPartyManager PartyManager { get; } = new();
    public KDropTable DropTable { get; } = new();
    public KHenirResultTable HenirResultTable { get; } = new();
    public KCnHackingCheckManager HackingCheckManager { get; } = new();
    public KAttribNpcTable AttribNpcTable { get; } = new();
    public KDungeonSecurityManager DungeonSecurityManager { get; } = new();
    public KDungeonEventManager DungeonEventManager { get; } = new();
    public KDungeonSubStageManager DungeonSubStageManager { get; } = new();
    public KBadAttitudeTable BadAttitudeTable { get; } = new();
    public KDefenceDungeonManager DefenceDungeonManager { get; } = new();
    private readonly ConcurrentDictionary<long, KRoomUserInfo> _pvpLobbyUsers = new();
    private readonly ConcurrentDictionary<long, KServerInfo> _connectedGameServers = new();
    private readonly ConcurrentDictionary<long, int> _udpKickReports = new();
    public ConcurrentDictionary<EventId, long> ServerLogCounters { get; } = new();
    private readonly ConcurrentDictionary<long, DateTime> _zombieEchoTimes = new();
    private readonly ConcurrentDictionary<long, byte> _henirRewardedUnits = new();
    public int SecretDungeonExtraStageNpcLevel { get; private set; } = 2;
    public float SecretDungeonExtraStageFactor { get; private set; } = 1.0f;
    public List<(int MinLv, int MaxLv, float Rate)> SecretDungeonExtraStageConditions { get; } = new();
    public ConcurrentDictionary<long, KCnTradeRoom> TradeRooms { get; } = new();
    public ConcurrentDictionary<long, KCnWeddingHall> WeddingHalls { get; } = new();
    public ConcurrentDictionary<long, KCnSquare> Squares { get; } = new();
    public ConcurrentDictionary<long, KCnPersonalShopRoom> PersonalShopRooms { get; } = new();
    public ConcurrentDictionary<long, KTrainingCenterRoom> TrainingCenterRooms { get; } = new();
    private long _nextTradeRoomUid = 1;
    private long _nextWeddingHallUid = 1;
    private long _nextSquareUid = 1;
    private long _nextShopUid = 1;
    private long _nextTcRoomUid = 1;

    public KTrainingCenterRoom CreateTrainingCenterRoom()
    {
        long uid = Interlocked.Increment(ref _nextTcRoomUid);
        var tc = new KTrainingCenterRoom { RoomUid = uid };
        TrainingCenterRooms[uid] = tc;
        return tc;
    }

    public KCnPersonalShopRoom CreatePersonalShopRoom()
    {
        long uid = Interlocked.Increment(ref _nextShopUid);
        var shop = new KCnPersonalShopRoom { ShopUid = uid };
        PersonalShopRooms[uid] = shop;
        return shop;
    }

    public KCnTradeRoom CreateTradeRoom()
    {
        long uid = Interlocked.Increment(ref _nextTradeRoomUid);
        var room = new KCnTradeRoom { TradeUid = uid };
        TradeRooms[uid] = room;
        return room;
    }

    public KCnWeddingHall CreateWeddingHall()
    {
        long uid = Interlocked.Increment(ref _nextWeddingHallUid);
        var hall = new KCnWeddingHall { RoomUid = uid };
        WeddingHalls[uid] = hall;
        return hall;
    }

    public KCnSquare CreateSquare(sbyte squareType, string roomName, bool isDefault = false)
    {
        long uid = Interlocked.Increment(ref _nextSquareUid);
        var sq = new KCnSquare
        {
            SquareUid = uid,
            SquareType = squareType,
            RoomName = roomName,
            IsDefault = isDefault,
            RelayPort = Region.DefaultUdpRelayPort,
        };
        Squares[uid] = sq;
        return sq;
    }

    public override uint PerformerTypeId => PerformerId.PI_CN_SERVER;
    public override KSimLayer SimLayer => _simLayer;
    public override KNetLayer NetLayer => _netLayer;
    public ushort ListenPort => _netLayer.Port;
    public bool IsRunning { get; private set; }
    public KScriptRefreshManager ScriptRefreshManager { get; }

    public KCenterServer(
        RegionCode initialRegion = RegionCode.TW,
        InProcessRouter? sharedRouter = null)
        : base(sharedRouter)
    {
        Region = RegionProfile.FromCode(initialRegion);
        _simLayer.NationFlag = Region.DefaultNationFlag;
        _netLayer = new KCnNetLayer(this);
        RoomManager.GameSysValTable = GameSysValTable;
        RoomManager.DefenceDungeonManagerTemplate = DefenceDungeonManager;
        ScriptRefreshManager = new KScriptRefreshManager(Router, PerformerId.PI_CN_SERVER, Uid);
        ScriptRefreshManager.SetDefaultReloadHandler(_ => LoadServerResources());
    }

    public bool Start(ushort? overridePort = null, int serverGroupId = 0)
    {
        if (IsRunning) return true;
        ServerGroupId = serverGroupId;
        Uid = KncUid.MakeUid(Region, serverGroupId, 300);
        Name = "KCenterServer";
        RoomManager.DefaultUdpRelayPort = Region.DefaultUdpRelayPort;

        _netLayer.Port = overridePort ?? Region.DefaultCenterPort;
        Router.RegisterPerformer(PerformerId.PI_CN_SERVER, this);
        Router.RegisterPerformer(PerformerId.PI_CN_ROOM, this);
        Router.RegisterPerformer(PerformerId.PI_CN_PARTY, this);
        RegToLua();

        if (!_netLayer.Init()) return false;
        _netLayer.Port = _netLayer.TcpServer.Port;

        IsRunning = true;
        ServerReady = true;
        ScriptRefreshManager.Start();
        _tickCts = new CancellationTokenSource();
        _tickTask = Task.Run(() => TickLoopAsync(_tickCts.Token));
        KLog.Cout($"[{Name}] Started on :{ListenPort} (Region={Region.Code}, UID={Uid})");
        return true;
    }

    public override void RegToLua()
    {
        base.RegToLua();
        ScriptManager.RegisterGlobal("RoomManager", RoomManager);
        ScriptManager.RegisterGlobal("GameSysVal", GameSysValTable);
        ScriptManager.RegisterGlobal("DropTable", DropTable);
        ScriptRefreshManager.BindToLua(ScriptManager, "ScriptRefresh");
    }

    public void SwitchRegion(RegionCode region)
    {
        Region = RegionProfile.FromCode(region);
        _simLayer.NationFlag = Region.DefaultNationFlag;
        RoomManager.DefaultUdpRelayPort = Region.DefaultUdpRelayPort;
    }

    public int LoadServerResources(string? resourceBaseDir = null, bool includeHeavyDropTables = false)
    {
        var script = new KScriptManager();
        GameSysValTable.BindToLua(script, "GameSysVal");
        DropTable.BindToLua(script, "DropTable");
        ScriptRefreshManager.BindToLua(script, "ScriptRefresh");

        Table? badProxy = null;
        badProxy = script.CreateLuaProxy("BadAttitudeTable", t =>
        {
            t["AddBadAttitudeCutLinePoint"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, badProxy!);
                BadAttitudeTable.SetBadAttitudeCutLinePoint(KScriptManager.ArgInt(a, o), KScriptManager.ArgInt(a, o + 1));
                return DynValue.Nil;
            });
            t["AddForceExitPoint"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, badProxy!);
                BadAttitudeTable.SetForceExitPoint(KScriptManager.ArgInt(a, o), KScriptManager.ArgInt(a, o + 1));
                return DynValue.Nil;
            });
        });

        AttribNpcTable.BindToLua(script, "g_pAttribNpcManager");
        HenirResultTable.BindToLua(script, "HenirResultTable");
        HackingCheckManager.BindToLua(script, "g_pCnHackingCheckManager");
        KCnPartyFeverManager.BindToLua(script, "g_pPartyManager");
        KBattleFieldGameManager.BindToLua(script, "g_pBattleFieldManager");
        DefenceDungeonManager.BindToLua(script, "DefenceDungeonManager");
        KResultProcess.BindToLua(script, "ResultProcess");
        KPvpMatchResultTable.BindToLua(script, "KPvpMatchResultTable");
        Table? dgnProxy = null;
        dgnProxy = script.CreateLuaProxy("g_pDungeonManager", t =>
        {
            t["SetSecretDungeonExtraStageNpcLevel"] = DynValue.NewCallback((_, a) => { SecretDungeonExtraStageNpcLevel = KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, dgnProxy!), SecretDungeonExtraStageNpcLevel); return DynValue.Nil; });
            t["SetExtraStageFactor"] = DynValue.NewCallback((_, a) => { SecretDungeonExtraStageFactor = KScriptManager.ArgFloat(a, KScriptManager.ArgStart(a, dgnProxy!), SecretDungeonExtraStageFactor); return DynValue.Nil; });
            t["AddExtraStageCondition"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, dgnProxy!);
                SecretDungeonExtraStageConditions.Add((KScriptManager.ArgInt(a, o), KScriptManager.ArgInt(a, o + 1), KScriptManager.ArgFloat(a, o + 2)));
                return DynValue.Nil;
            });
        });

        int loaded = 0;
        string[] files = includeHeavyDropTables
            ? [
                "GameSysValTable.lua", "BadAttitudeTable.lua", "AttribNpcTable.lua", "PvPDropTable.lua", "DropData.lua",
                "HenirResultTable.lua", "HenirData.lua", "HenirChallangeReward.lua", "HenirSpecialNpcDrop.lua", "NpcExpTable.lua",
                "CnHackingCheckData.lua", "PartyServerData.lua", "BattleFieldServerData.lua", "FieldBonusDrop.lua",
                "SecretDungeonData.lua", "DefenceDungeonData.lua", "PaymentItemTable.lua", "ResultData_new.lua", "PvpMatchResultTable.lua",
                "StaticDropTable.lua", "DropTable.lua"
              ]
            : [
                "GameSysValTable.lua", "BadAttitudeTable.lua", "AttribNpcTable.lua", "PvPDropTable.lua", "DropData.lua",
                "HenirResultTable.lua", "HenirData.lua", "HenirChallangeReward.lua", "HenirSpecialNpcDrop.lua", "NpcExpTable.lua",
                "CnHackingCheckData.lua", "PartyServerData.lua", "BattleFieldServerData.lua", "FieldBonusDrop.lua",
                "SecretDungeonData.lua", "DefenceDungeonData.lua", "PaymentItemTable.lua", "ResultData_new.lua", "PvpMatchResultTable.lua"
              ];

        foreach (string f in files)
        {
            if (script.DoServerResourceFile(f, Region, resourceBaseDir))
                loaded++;
        }
        return loaded;
    }

    private async Task TickLoopAsync(CancellationToken ct)
    {
        var sw = new Stopwatch();
        while (!ct.IsCancellationRequested)
        {
            sw.Restart();
            try { Tick(); } catch (Exception ex) { KLog.Cerr($"KCenterServer Tick error: {ex.Message}"); }
            sw.Stop();
            try { await Task.Delay(Math.Max(5, 50 - (int)sw.ElapsedMilliseconds), ct); }
            catch (OperationCanceledException) { break; }
        }
    }

    public override void Tick()
    {
        base.Tick();
        _simLayer.Tick();
        foreach (var (roomUid, memberUids, oldHostUid, newHostUid) in RoomManager.TickRooms())
        {
            var not = new KEGS_CHECK_PING_SCORE_CHANGE_HOST_NOT
            {
                OldHostUid = oldHostUid,
                NewHostUid = newHostUid,
            };
            foreach (long uid in memberUids)
                Router.Send(PerformerId.PI_CN_ROOM, roomUid, PerformerId.PI_GS_USER, uid, EventId.ERM_CHECK_PING_SCORE_CHANGE_HOST_NOT, not);
        }
        foreach (var (partyUid, memberUids, feverPoint) in PartyManager.TickFeverExpirations())
        {
            var feverEnd = new KEGS_CHANGE_PARTY_FEVER_STATE_NOT
            {
                IsStart = false,
                PartyFever = feverPoint,
            };
            foreach (long uid in memberUids)
                Router.Send(PerformerId.PI_CN_PARTY, partyUid, PerformerId.PI_GS_USER, uid, EventId.EPM_CHANGE_PARTY_FEVER_STATE_NOT, feverEnd);
        }
        foreach (var kv in TradeRooms)
        {
            kv.Value.TickWaitTimeout();
            if (kv.Value.State == KCnTradeRoom.TradeRoomState.Close)
                TradeRooms.TryRemove(kv.Key, out _);
        }
        foreach (var kv in WeddingHalls)
        {
            kv.Value.Tick();
            if (kv.Value.State == KCnWeddingHall.HallState.Close)
                WeddingHalls.TryRemove(kv.Key, out _);
        }
        foreach (var kv in PersonalShopRooms)
        {
            if (kv.Value.State == KCnPersonalShopRoom.ShopState.Close)
                PersonalShopRooms.TryRemove(kv.Key, out _);
        }
        foreach (var kv in TrainingCenterRooms)
        {
            if (kv.Value.State == KTrainingCenterRoom.TcRoomState.Close)
                TrainingCenterRooms.TryRemove(kv.Key, out _);
        }
    }

    protected override void ProcessEvent(KEvent ev)
    {
        switch (ev.EventId)
        {
            case EventId.E_HEART_BEAT:
                break;

            case EventId.EGS_REPORT_HACK_USER_NOT:
            case EventId.EGS_SERVER_CHECK_HACK_USER_NOT:
                DungeonSecurityManager.ValidateDamage(0, 0, ev.FirstSenderUid > 0 ? ev.FirstSenderUid : 0, 0);
                break;

            case EventId.ERM_SERVER_CHECK_HACK_USER_NOT:
            {
                if (ev.InProcessPayload is KEGS_SERVER_CHECK_HACK_USER_NOT hackNot)
                {
                    DungeonSecurityManager.ValidateDamage(0, 0, ev.FirstSenderUid > 0 ? ev.FirstSenderUid : 0, hackNot.HackingCheckType);
                }
                else
                {
                    var rep = ev.GetPayload<KERM_DUNGEON_SECURITY_REPORT_NOT>(Region);
                    DungeonSecurityManager.ValidateDamage(rep.RoomUid, rep.DungeonId, rep.UnitUid, rep.MaxSingleDamage);
                }
                break;
            }

            case EventId.EPM_CREATE_PARTY_BY_INVITE_REQ:
            {
                var req = ev.GetPayload<KEPM_CREATE_PARTY_BY_INVITE_REQ>(Region);
                PartyManager.CreateParty(req.CreatePartyInfo, req.HostInfo);
                break;
            }

            case EventId.EPM_LEAVE_PARTY_REQ:
            {
                var req = ev.GetPayload<KEPM_LEAVE_PARTY_REQ>(Region);
                PartyManager.LeaveParty(req.PartyUid, req.UnitUid);
                break;
            }

            case EventId.ERM_OPEN_TC_ROOM_REQ:
            {
                var req = ev.GetPayload<KERM_OPEN_TC_ROOM_REQ>(Region);
                var tc = CreateTrainingCenterRoom();
                tc.OpenRoom(req.UnitUid, req.TcId, req.DungeonId, req.PlayTimeLimit);
                break;
            }

            case EventId.ECN_ADD_COMMON_FLAG_NOT:
            {
                var req = ev.GetPayload<KECN_ADD_COMMON_FLAG_NOT>(Region);
                _simLayer.AddCommonFlag(req.Flag);
                break;
            }

            case EventId.ECN_DEL_COMMON_FLAG_NOT:
            {
                var req = ev.GetPayload<KECN_DEL_COMMON_FLAG_NOT>(Region);
                _simLayer.DeleteCommonFlag(req.Flag);
                break;
            }

            case EventId.ECN_MEGAPHONE_MESSAGE_NOT:
            {
                var req = ev.GetPayload<KECN_MEGAPHONE_MESSAGE_NOT>(Region);
                KLog.Clog($"[{Name}] Megaphone from {req.SenderNickName}: {req.Message}");
                break;
            }

            case EventId.ECN_NOTIFY_MSG_NOT:
            {
                var req = ev.GetPayload<KECN_NOTIFY_MSG_NOT>(Region);
                KLog.Clog($"[{Name}] Notify({req.NotifyType}) x{req.Count}: {req.Message}");
                break;
            }

            case EventId.ECN_VERIFY_SERVER_CONNECT_REQ:
            {
                var req = ev.GetPayload<KECN_VERIFY_SERVER_CONNECT_REQ>(Region);
                if (req.ServerUid > 0)
                {
                    Router.Send(PerformerId.PI_CN_SERVER, Uid, PerformerId.PI_GS_SERVER, req.ServerUid,
                        EventId.ECN_VERIFY_SERVER_CONNECT_ACK,
                        new KECN_VERIFY_SERVER_CONNECT_ACK { Ok = NetError.NET_OK, CenterUid = Uid });
                }
                break;
            }

            case EventId.ECN_CHECK_CHANNEL_CHANGE_REQ:
            {
                var req = ev.GetPayload<KECN_CHECK_CHANNEL_CHANGE_REQ>(Region);
                if (ev.FirstSenderUid > 0)
                {
                    Router.Send(PerformerId.PI_CN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ECN_CHECK_CHANNEL_CHANGE_ACK, new KECN_CHECK_CHANNEL_CHANGE_ACK
                        {
                            Ok = NetError.NET_OK,
                            UserUid = req.UserUid,
                            TargetChannelId = req.TargetChannelId,
                        });
                }
                break;
            }

            case EventId.ECN_NEW_PVP_USER_INFO_NOT:
            case EventId.ECN_UPDATE_PVP_USER_INFO_NOT:
            {
                var info = ev.GetPayload<KRoomUserInfo>(Region);
                if (info.UnitUid > 0) _pvpLobbyUsers[info.UnitUid] = info;
                break;
            }

            case EventId.ERM_OPEN_PVP_ROOM_REQ:
            case EventId.ERM_OPEN_DUNGEON_ROOM_REQ:
            case EventId.ERM_SINGLE_OPEN_DUNGEON_ROOM_REQ:
            case EventId.ERM_PARTY_OPEN_DUNGEON_ROOM_REQ:
            {
                var req = ev.GetPayload<KERM_OPEN_ROOM_REQ>(Region);
                var room = RoomManager.CreateRoom(req.RoomInfo, req.HostInfo);
                if (ev.FirstSenderUid > 0)
                {
                    Router.Send(PerformerId.PI_CN_ROOM, room.Info.RoomUid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ERM_OPEN_ROOM_ACK, new KERM_OPEN_ROOM_ACK
                        {
                            Ok = NetError.NET_OK,
                            RoomInfo = room.Info,
                            Slots = room.SnapshotSlots(),
                        });
                }
                break;
            }

            case EventId.ERM_OPEN_PVP_ROOM_FOR_MATCH_REQ:
            {
                var req = ev.GetPayload<KEGB_OPEN_PVP_ROOM_FOR_MATCH_REQ>(Region);
                var room = RoomManager.CreateRoom(req.RoomInfo);
                foreach (var u in req.RedTeam.Concat(req.BlueTeam))
                    room.TryAddUser(u, asObserver: false, password: string.Empty);
                room.StartGame();
                if (ev.FirstSenderUid > 0)
                {
                    Router.Send(PerformerId.PI_CN_ROOM, room.Info.RoomUid, PerformerId.PI_GB_SERVER, ev.FirstSenderUid,
                        EventId.EGB_OPEN_PVP_ROOM_FOR_MATCH_ACK, new KPacketOK { Ok = NetError.NET_OK });
                }
                break;
            }

            case EventId.ERM_OPEN_BATTLE_FIELD_REQ:
            case EventId.ERM_JOIN_BATTLE_FIELD_REQ:
            {
                var req = ev.GetPayload<KERM_JOIN_ROOM_REQ>(Region);
                var room = RoomManager.GetOrCreateBattleFieldRoom(req.MapId);
                var (ok, _) = room.TryAddUser(req.UserInfo, asObserver: false, password: string.Empty);
                if (ev.FirstSenderUid > 0)
                {
                    Router.Send(PerformerId.PI_CN_ROOM, room.Info.RoomUid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ERM_JOIN_BATTLE_FIELD_ACK, new KERM_JOIN_ROOM_ACK
                        {
                            Ok = ok,
                            RoomInfo = room.Info,
                            Slots = room.SnapshotSlots(),
                        });
                }
                break;
            }

            case EventId.ERM_JOIN_ROOM_REQ:
            {
                var req = ev.GetPayload<KERM_JOIN_ROOM_REQ>(Region);
                var (ok, room) = RoomManager.JoinRoom(req.RoomUid, req.UserInfo, req.Password, req.IsObserver);
                if (ev.FirstSenderUid > 0)
                {
                    Router.Send(PerformerId.PI_CN_ROOM, req.RoomUid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ERM_JOIN_ROOM_ACK, new KERM_JOIN_ROOM_ACK
                        {
                            Ok = ok,
                            RoomInfo = room?.Info ?? new KRoomInfo(),
                            Slots = room?.SnapshotSlots() ?? [],
                        });
                }
                break;
            }

            case EventId.ERM_LEAVE_ROOM_REQ:
            {
                var req = ev.GetPayload<KERM_LEAVE_ROOM_REQ>(Region);
                RoomManager.LeaveRoom(req.RoomUid, req.UnitUid);
                break;
            }

            case EventId.ERM_GAME_START_REQ:
            {
                var req = ev.GetPayload<KERM_GAME_START_REQ>(Region);
                var room = RoomManager.GetRoom(req.RoomUid);
                if (room != null)
                    room.StartGame();
                break;
            }

            case EventId.ERM_DUNGEON_STAGE_LOAD_COMPLETE_REQ:
            {
                if (ev.InProcessPayload is KERM_LEAVE_ROOM_REQ legacy)
                {
                    RoomManager.GetRoom(legacy.RoomUid)?.MarkStageLoaded(legacy.UnitUid, out _, out _);
                }
                else
                {
                    _ = ev.GetPayload<KEGS_DUNGEON_STAGE_LOAD_COMPLETE_REQ>(Region);
                    long unitUid = ev.FirstSenderUid;
                    var room = RoomManager.GetRoomByUnitUid(unitUid);
                    room?.MarkStageLoaded(unitUid, out _, out _);
                }
                break;
            }

            case EventId.ERM_DUNGEON_SUB_STAGE_LOAD_COMPLETE_REQ:
            {
                if (ev.InProcessPayload is KERM_LEAVE_ROOM_REQ legacy)
                {
                    RoomManager.GetRoom(legacy.RoomUid)?.MarkSubStageLoaded(legacy.UnitUid, out _);
                }
                else
                {
                    _ = ev.GetPayload<KEGS_DUNGEON_SUB_STAGE_LOAD_COMPLETE_REQ>(Region);
                    long unitUid = ev.FirstSenderUid;
                    var room = RoomManager.GetRoomByUnitUid(unitUid);
                    room?.MarkSubStageLoaded(unitUid, out _);
                }
                break;
            }

            case EventId.ERM_DUNGEON_SUB_STAGE_GO_NEXT_REQ:
            {
                if (ev.InProcessPayload is KERM_LEAVE_ROOM_REQ legacy)
                {
                    var room = RoomManager.GetRoom(legacy.RoomUid);
                    if (room != null)
                    {
                        var next = DungeonSubStageManager.ResolveNextStage(
                            room.Info.DungeonId, room.CurrentStageId, room.CurrentSubStageId);
                        if (next.ClearType == KDungeonSubStageManager.ClearType.CtSubStage)
                            room.BeginSubStageOpen((sbyte)next.SubStageIndex);
                        else if (next.ClearType is KDungeonSubStageManager.ClearType.CtStage
                            or KDungeonSubStageManager.ClearType.CtStageForce
                            or KDungeonSubStageManager.ClearType.CtStageSecret)
                            room.BeginStageLoad(next.StageIndex);
                    }
                }
                else
                {
                    var req = ev.GetPayload<KEGS_DUNGEON_SUB_STAGE_GO_NEXT_REQ>(Region);
                    var room = RoomManager.GetRoomByUnitUid(ev.FirstSenderUid);
                    if (room != null)
                    {
                        var next = DungeonSubStageManager.ResolveNextStage(
                            room.Info.DungeonId, req.BeforeStageId, room.CurrentSubStageId);
                        if (next.ClearType == KDungeonSubStageManager.ClearType.CtSubStage)
                            room.BeginSubStageOpen((sbyte)next.SubStageIndex);
                        else if (next.ClearType is KDungeonSubStageManager.ClearType.CtStage
                            or KDungeonSubStageManager.ClearType.CtStageForce
                            or KDungeonSubStageManager.ClearType.CtStageSecret)
                            room.BeginStageLoad(next.StageIndex);
                        else
                            room.BeginSubStageOpen(req.NextStageId);
                    }
                }
                break;
            }

            case EventId.ERM_OPEN_TRADE_REQ:
            {
                var req = ev.GetPayload<KERM_OPEN_TRADE_REQ>(Region);
                var tr = CreateTradeRoom();
                tr.JoinTrade(new KCnTradeRoom.TradeParticipant
                {
                    UnitUid = req.OfferUnitUid,
                    NickName = req.OfferNickName,
                });
                break;
            }

            case EventId.ERM_JOIN_TRADE_REQ:
            {
                var req = ev.GetPayload<KERM_JOIN_TRADE_REQ>(Region);
                if (TradeRooms.TryGetValue(req.TradeUid, out var tr))
                    tr.JoinTrade(new KCnTradeRoom.TradeParticipant { UnitUid = req.UnitUid });
                break;
            }

            case EventId.ERM_UPDATE_TRADE_ED_REQ:
            {
                var req = ev.GetPayload<KERM_UPDATE_TRADE_ED_REQ>(Region);
                if (TradeRooms.TryGetValue(req.TradeUid, out var tr))
                    tr.UpdateTradeEd(req.UnitUid, (int)Math.Clamp(req.EdAmount, 0, KCnTradeRoom.MaxHoldEd));
                break;
            }

            case EventId.ERM_UPDATE_TRADE_ITEM_REQ:
            {
                var req = ev.GetPayload<KERM_UPDATE_TRADE_ITEM_REQ>(Region);
                if (TradeRooms.TryGetValue(req.TradeUid, out var tr))
                    tr.UpdateTradeItem(req.UnitUid, req.Item, !req.Add);
                break;
            }

            case EventId.ERM_OPEN_PERSONAL_SHOP_REQ:
            {
                var req = ev.GetPayload<KERM_OPEN_PERSONAL_SHOP_REQ>(Region);
                var shop = CreatePersonalShopRoom();
                shop.OpenShop(req.OwnerUnitUid, req.ShopName, req.ShopType > 0);
                break;
            }

            case EventId.ERM_JOIN_PERSONAL_SHOP_REQ:
            {
                var req = ev.GetPayload<KERM_JOIN_PERSONAL_SHOP_REQ>(Region);
                if (PersonalShopRooms.TryGetValue(req.ShopUid, out var shop))
                    shop.JoinShop(req.UnitUid);
                break;
            }

            case EventId.ERM_LEAVE_PERSONAL_SHOP_REQ:
            {
                var req = ev.GetPayload<KERM_LEAVE_PERSONAL_SHOP_REQ>(Region);
                if (PersonalShopRooms.TryGetValue(req.ShopUid, out var shop))
                    shop.LeaveShop(req.UnitUid);
                break;
            }

            case EventId.ERM_OPEN_WEDDING_HALL_REQ:
            {
                var req = ev.GetPayload<KERM_OPEN_WEDDING_HALL_REQ>(Region);
                CreateWeddingHall();
                break;
            }

            case EventId.ERM_JOIN_WEDDING_HALL_REQ:
            {
                var req = ev.GetPayload<KERM_JOIN_WEDDING_HALL_REQ>(Region);
                if (WeddingHalls.TryGetValue(req.RoomUid, out var hall))
                {
                    hall.JoinHall(new KRoomSlotInfo
                    {
                        SlotState = 2,
                        RoomUserInfo = new KRoomUserInfo { UnitUid = req.UnitUid, NickName = req.NickName },
                    });
                }
                break;
            }

            case EventId.ERM_OPEN_SQUARE_NOT:
            {
                var req = ev.GetPayload<KERM_OPEN_SQUARE_NOT>(Region);
                CreateSquare(req.SquareType, req.RoomName, req.IsDefault);
                break;
            }

            case EventId.ERM_JOIN_SQUARE_REQ:
            {
                var req = ev.GetPayload<KERM_JOIN_SQUARE_REQ>(Region);
                if (Squares.TryGetValue(req.SquareUid, out var sq))
                    sq.JoinSquare(new KCnSquare.SquareMember(req.UnitUid, req.NickName, ev.LastSenderUid));
                break;
            }

            // ---- Additional ECN_* / ESR_* / E_* ----
            case EventId.ECN_CHECK_INVALID_PVP_USER_NOT:
            {
                var pkt = ev.GetPayload<KPacketLong>(Region);
                if (pkt.Value > 0) _pvpLobbyUsers.TryRemove(pkt.Value, out _);
                break;
            }

            case EventId.ECN_UDP_KICK_GAMEEDIT_NOT:
            {
                var req = ev.GetPayload<KEGS_UDP_KICK_GAMEEDIT_NOT>(Region);
                if (ev.FirstSenderUid > 0)
                    _udpKickReports.AddOrUpdate(ev.FirstSenderUid, 1, static (_, c) => c + 1);
                break;
            }

            case EventId.ECN_UPDATE_SERVER_INFO_REQ:
            {
                var info = ev.GetPayload<KServerInfo>(Region);
                if (info.Uid > 0) _connectedGameServers[info.Uid] = info;
                if (ev.LastSenderUid > 0)
                {
                    Router.Send(PerformerId.PI_CN_SERVER, Uid, PerformerId.PI_GS_SERVER, ev.LastSenderUid,
                        EventId.ECN_UPDATE_SERVER_INFO_ACK, new KServerList { ServerList = _connectedGameServers.Values.ToList() });
                }
                break;
            }

            case EventId.ECN_WORLD_TRIGGER_RELOCATION_REQ:
            {
                var req = ev.GetPayload<KEGS_WORLD_TRIGGER_RELOCATION_REQ>(Region);
                if (ev.FirstSenderUid > 0)
                {
                    Router.Send(PerformerId.PI_CN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.EGS_WORLD_TRIGGER_RELOCATION_NOT, req);
                }
                break;
            }

            case EventId.ECN_SEND_STATISTICS_DBLOG_REQ:
            case EventId.ECN_SEND_STATISTICS_LOCLOG_REQ:
                ServerLogCounters.AddOrUpdate(ev.EventId, 1, static (_, c) => c + 1);
                break;

            case EventId.EGS_GET_SERVER_SET_DATA_REQ:
            {
                if (ev.FirstSenderUid > 0)
                {
                    Router.Send(PerformerId.PI_CN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.EGS_GET_SERVER_SET_DATA_ACK, new KEGS_GET_SERVER_SET_DATA_ACK { Ok = NetError.NET_OK });
                }
                break;
            }

            case EventId.ESR_ORDER_TO_CENTER_SERVER_NOT:
            {
                var order = ev.GetPayload<KESR_SCRIPT_REFRESH_ORDER_NOT>(Region);
                if (order.OrderType > KESR_SCRIPT_REFRESH_ORDER_NOT.OT_NONE &&
                    order.OrderType < KESR_SCRIPT_REFRESH_ORDER_NOT.OT_CN_SENTINEL)
                {
                    ScriptRefreshManager.ExecuteRefreshOrder(order.OrderType);
                }
                else if (order.OrderType > KESR_SCRIPT_REFRESH_ORDER_NOT.OT_CN_SENTINEL &&
                         order.OrderType < KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_SENTINEL)
                {
                    Router.Send(PerformerId.PI_CN_SERVER, Uid, PerformerId.PI_GS_SERVER, 0,
                        EventId.ESR_ORDER_TO_GAME_SERVER_NOT, order);
                }
                else
                {
                    KLog.Cwarn($"[{Name}] Undefined script refresh order type: {order.OrderType}");
                }
                break;
            }

            case EventId.ESR_ORDER_TO_REFRESH_MANAGER_REQ:
            case EventId.ESR_ORDER_TO_REFRESH_MANAGER_ACK:
            {
                var order = ev.PayloadBytes.Length > 0
                    ? ev.GetPayload<KESR_SCRIPT_REFRESH_ORDER_NOT>(Region)
                    : new KESR_SCRIPT_REFRESH_ORDER_NOT();
                if (order.OrderType > 0)
                    ScriptRefreshManager.ExecuteRefreshOrder(order.OrderType);
                else
                    LoadServerResources();
                break;
            }

            case EventId.E_RESERVE_DESTROY:
                ShutDown();
                break;

            // ---- EPM_* Party Engine ----
            case EventId.EPM_CHANGE_PARTY_TYPE_REQ:
            {
                var req = ev.GetPayload<KPartyInfo>(Region);
                var party = PartyManager.ResolveParty(req.PartyUid, ev.FirstSenderUid);
                bool ok = party != null && party.ChangePartyType(ev.FirstSenderUid, req);
                if (ev.FirstSenderUid > 0)
                {
                    Router.Send(PerformerId.PI_CN_PARTY, party?.Info.PartyUid ?? 0, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.EPM_CHANGE_PARTY_TYPE_ACK, new KPacketOK { Ok = ok ? NetError.NET_OK : NetError.ERR_PARTY_01 });
                }
                if (ok && party != null)
                {
                    foreach (long uid in party.GetUnitUids())
                        Router.Send(PerformerId.PI_CN_PARTY, party.Info.PartyUid, PerformerId.PI_GS_USER, uid,
                            EventId.EPM_CHANGE_PARTY_TYPE_NOT, party.Info);
                }
                break;
            }

            case EventId.EPM_CHAT_REQ:
            {
                var req = ev.GetPayload<KEPM_CHAT_REQ>(Region);
                var party = PartyManager.ResolveParty(req.PartyUid, ev.FirstSenderUid);
                if (party != null)
                {
                    foreach (long uid in party.GetUnitUids())
                        Router.Send(PerformerId.PI_CN_PARTY, party.Info.PartyUid, PerformerId.PI_GS_USER, uid,
                            EventId.EPM_CHAT_NOT, req);
                }
                if (ev.FirstSenderUid > 0)
                {
                    Router.Send(PerformerId.PI_CN_PARTY, req.PartyUid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.EPM_CHAT_ACK, new KERM_CHAT_ACK
                        {
                            Ok = party != null ? NetError.NET_OK : NetError.ERR_PARTY_00,
                            ChatPacketType = req.ChatPacketType,
                        });
                }
                break;
            }

            case EventId.EPM_INVITE_PARTY_REQ:
            {
                var req = ev.GetPayload<KEPM_INVITE_PARTY_REQ>(Region);
                var party = PartyManager.ResolveParty(req.PartyUid, ev.FirstSenderUid);
                bool ok = party != null;
                if (ev.FirstSenderUid > 0)
                {
                    Router.Send(PerformerId.PI_CN_PARTY, req.PartyUid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.EPM_INVITE_PARTY_ACK, new KEGS_INVITE_PARTY_ACK
                        {
                            Ok = ok ? NetError.NET_OK : NetError.ERR_PARTY_00,
                            ReceiverUid = req.PartyUserInfo.UnitUid,
                        });
                }
                if (ok && req.PartyUserInfo.UnitUid > 0)
                {
                    Router.Send(PerformerId.PI_CN_PARTY, req.PartyUid, PerformerId.PI_GS_USER, req.PartyUserInfo.UnitUid,
                        EventId.EPM_INVITE_PARTY_NOT, req);
                }
                break;
            }

            case EventId.EPM_INVITE_PARTY_REPLY_NOT:
            {
                var req = ev.GetPayload<KEPM_INVITE_PARTY_REPLY_NOT>(Region);
                if (req.ReplyReason == 0 && PartyManager.TryJoinParty(req.PartyUid, req.PartyUserInfo, out var party) && party != null)
                {
                    foreach (long uid in party.GetUnitUids())
                        Router.Send(PerformerId.PI_CN_PARTY, party.Info.PartyUid, PerformerId.PI_GS_USER, uid,
                            EventId.EPM_UPDATE_PARTY_USER_INFO_NOT, new KEPM_UPDATE_PARTY_USER_INFO_NOT
                            {
                                PartyUid = party.Info.PartyUid,
                                UserInfo = req.PartyUserInfo,
                            });
                }
                break;
            }

            case EventId.EPM_PARTY_BAN_USER_REQ:
            {
                var req = ev.GetPayload<KEPM_PARTY_BAN_USER_REQ>(Region);
                var party = PartyManager.ResolveParty(req.PartyUid, ev.FirstSenderUid);
                bool ok = party != null && party.BanUser(ev.FirstSenderUid, req.UnitUid);
                if (ev.FirstSenderUid > 0)
                {
                    Router.Send(PerformerId.PI_CN_PARTY, req.PartyUid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.EPM_PARTY_BAN_USER_ACK, new KPacketOK { Ok = ok ? NetError.NET_OK : NetError.ERR_PARTY_01 });
                }
                if (ok && party != null)
                {
                    var banNot = new KPacketOK { Ok = NetError.NET_OK };
                    Router.Send(PerformerId.PI_CN_PARTY, party.Info.PartyUid, PerformerId.PI_GS_USER, req.UnitUid,
                        EventId.EPM_PARTY_BAN_USER_NOT, banNot);
                    foreach (long uid in party.GetUnitUids())
                        Router.Send(PerformerId.PI_CN_PARTY, party.Info.PartyUid, PerformerId.PI_GS_USER, uid,
                            EventId.EPM_PARTY_BAN_USER_NOT, banNot);
                }
                break;
            }

            case EventId.EPM_PARTY_CHANGE_CHECK_MIN_LEVEL_REQ:
            {
                var req = ev.GetPayload<KEPM_PARTY_CHANGE_CHECK_MIN_LEVEL_REQ>(Region);
                var party = PartyManager.ResolveParty(req.PartyUid, ev.FirstSenderUid);
                bool ok = party != null && party.SetLimitLevel(ev.FirstSenderUid, req.Check);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_CN_PARTY, req.PartyUid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.EPM_PARTY_CHANGE_CHECK_MIN_LEVEL_ACK, new KPacketOK { Ok = ok ? NetError.NET_OK : NetError.ERR_PARTY_01 });
                if (ok && party != null)
                {
                    foreach (long uid in party.GetUnitUids())
                        Router.Send(PerformerId.PI_CN_PARTY, party.Info.PartyUid, PerformerId.PI_GS_USER, uid,
                            EventId.EPM_PARTY_CHANGE_CHECK_MIN_LEVEL_NOT, req);
                }
                break;
            }

            case EventId.EPM_PARTY_CHANGE_DUNGEON_REQ:
            {
                var req = ev.GetPayload<KEPM_PARTY_CHANGE_DUNGEON_REQ>(Region);
                var party = PartyManager.ResolveParty(req.PartyUid, ev.FirstSenderUid);
                bool ok = party != null && party.ChangeDungeon(ev.FirstSenderUid, req.DungeonId, req.DifficultyLevel, req.DungeonMode);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_CN_PARTY, req.PartyUid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.EPM_PARTY_CHANGE_DUNGEON_ACK, new KPacketOK { Ok = ok ? NetError.NET_OK : NetError.ERR_PARTY_01 });
                if (ok && party != null)
                {
                    foreach (long uid in party.GetUnitUids())
                        Router.Send(PerformerId.PI_CN_PARTY, party.Info.PartyUid, PerformerId.PI_GS_USER, uid,
                            EventId.EPM_PARTY_CHANGE_DUNGEON_NOT, req);
                }
                break;
            }

            case EventId.EPM_PARTY_CHANGE_GET_ITEM_TYPE_REQ:
            {
                var req = ev.GetPayload<KEPM_PARTY_CHANGE_GET_ITEM_TYPE_REQ>(Region);
                var party = PartyManager.ResolveParty(req.PartyUid, ev.FirstSenderUid);
                bool ok = party != null && party.ChangeGetItemType(ev.FirstSenderUid, req.GetItemType);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_CN_PARTY, req.PartyUid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.EPM_PARTY_CHANGE_GET_ITEM_TYPE_ACK, new KPacketOK { Ok = ok ? NetError.NET_OK : NetError.ERR_PARTY_01 });
                if (ok && party != null)
                {
                    foreach (long uid in party.GetUnitUids())
                        Router.Send(PerformerId.PI_CN_PARTY, party.Info.PartyUid, PerformerId.PI_GS_USER, uid,
                            EventId.EPM_PARTY_CHANGE_GET_ITEM_TYPE_NOT, req);
                }
                break;
            }

            case EventId.EPM_PARTY_CHANGE_HOST_REQ:
            {
                var req = ev.GetPayload<KEPM_PARTY_CHANGE_HOST_REQ>(Region);
                var party = PartyManager.ResolveParty(req.PartyUid, ev.FirstSenderUid);
                long beforeUid = 0;
                long afterUid = 0;
                bool ok = party != null && party.ChangeHost(ev.FirstSenderUid, req.NewHostUnitUid, out beforeUid, out afterUid);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_CN_PARTY, req.PartyUid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.EPM_PARTY_CHANGE_HOST_ACK, new KPacketOK { Ok = ok ? NetError.NET_OK : NetError.ERR_PARTY_01 });
                if (ok && party != null)
                {
                    var hostNot = new KEPM_CHECK_PARTY_CHANGE_HOST_ACK
                    {
                        Ok = NetError.NET_OK,
                        PartyUid = party.Info.PartyUid,
                        OldHostUnitUid = beforeUid,
                        NewHostUnitUid = afterUid,
                    };
                    foreach (long uid in party.GetUnitUids())
                        Router.Send(PerformerId.PI_CN_PARTY, party.Info.PartyUid, PerformerId.PI_GS_USER, uid,
                            EventId.EPM_PARTY_CHANGE_HOST_NOT, hostNot);
                }
                break;
            }

            case EventId.EPM_PARTY_CHANGE_NUM_OF_PER_REQ:
            {
                var req = ev.GetPayload<KEPM_PARTY_CHANGE_NUM_OF_PER_REQ>(Region);
                var party = PartyManager.ResolveParty(req.PartyUid, ev.FirstSenderUid);
                bool ok = party != null && party.ChangeMaxSlot(ev.FirstSenderUid, req.NumOfPer);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_CN_PARTY, req.PartyUid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.EPM_PARTY_CHANGE_NUM_OF_PER_ACK, new KPacketOK { Ok = ok ? NetError.NET_OK : NetError.ERR_PARTY_01 });
                if (ok && party != null)
                {
                    foreach (long uid in party.GetUnitUids())
                        Router.Send(PerformerId.PI_CN_PARTY, party.Info.PartyUid, PerformerId.PI_GS_USER, uid,
                            EventId.EPM_PARTY_CHANGE_NUM_OF_PER_NOT, req);
                }
                break;
            }

            case EventId.EPM_PARTY_CHANGE_READY_REQ:
            {
                var req = ev.GetPayload<KEPM_PARTY_CHANGE_READY_REQ>(Region);
                var party = PartyManager.ResolveParty(req.PartyUid, ev.FirstSenderUid);
                bool ok = party != null && party.SetReady(ev.FirstSenderUid, req.Ready);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_CN_PARTY, req.PartyUid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.EPM_PARTY_CHANGE_READY_ACK, new KPacketOK { Ok = ok ? NetError.NET_OK : NetError.ERR_PARTY_01 });
                if (ok && party != null)
                {
                    var not = new KEGS_PARTY_CHANGE_READY_NOT { UnitUid = ev.FirstSenderUid, Ready = req.Ready };
                    foreach (long uid in party.GetUnitUids())
                        Router.Send(PerformerId.PI_CN_PARTY, party.Info.PartyUid, PerformerId.PI_GS_USER, uid,
                            EventId.EPM_PARTY_CHANGE_READY_NOT, not);
                }
                break;
            }

            case EventId.EPM_PARTY_GAME_START_REQ:
            {
                var req = ev.GetPayload<KEPM_PARTY_GAME_START_REQ>(Region);
                var party = PartyManager.ResolveParty(req.PartyUid, ev.FirstSenderUid);
                bool ok = party != null && party.CanStartGame(ev.FirstSenderUid);
                if (ev.FirstSenderUid > 0)
                {
                    Router.Send(PerformerId.PI_CN_PARTY, req.PartyUid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.EPM_PARTY_GAME_START_ACK, new KEPM_PARTY_GAME_START_ACK
                        {
                            Ok = ok ? NetError.NET_OK : NetError.ERR_PARTY_17,
                            DungeonId = party?.Info.DungeonId ?? 0,
                            DungeonMode = party?.Info.DungeonMode ?? 0,
                        });
                }
                if (ok && party != null)
                {
                    foreach (long uid in party.GetUnitUids())
                        Router.Send(PerformerId.PI_CN_PARTY, party.Info.PartyUid, PerformerId.PI_GS_USER, uid,
                            EventId.EGS_PARTY_GAME_START_NOT, new KEGS_PARTY_GAME_START_NOT
                            {
                                RoomInfo = new KRoomInfo { RoomUid = party.Info.PartyUid },
                            });
                }
                break;
            }

            case EventId.EPM_PVP_PARTY_CHANGE_MATCH_INFO_REQ:
            {
                var req = ev.GetPayload<KEGS_PVP_PARTY_CHANGE_MATCH_INFO_REQ>(Region);
                var party = PartyManager.ResolveParty(ev.DestPerformer.Uids.FirstOrDefault(), ev.FirstSenderUid);
                bool ok = party != null && party.ChangePvpMatchInfo(ev.FirstSenderUid, req.PvpGameType);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_CN_PARTY, party?.Info.PartyUid ?? 0, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.EPM_PVP_PARTY_CHANGE_MATCH_INFO_ACK, new KPacketOK { Ok = ok ? NetError.NET_OK : NetError.ERR_PARTY_01 });
                if (ok && party != null)
                {
                    foreach (long uid in party.GetUnitUids())
                        Router.Send(PerformerId.PI_CN_PARTY, party.Info.PartyUid, PerformerId.PI_GS_USER, uid,
                            EventId.EPM_PVP_PARTY_CHANGE_MATCH_INFO_NOT, req);
                }
                break;
            }

            case EventId.EPM_INCREASE_PARTY_FEVER_NOT:
            {
                var req = ev.GetPayload<KERM_INCREASE_PARTY_FEVER_NOT>(Region);
                var party = PartyManager.ResolveParty(req.PartyUid, ev.FirstSenderUid);
                if (party != null)
                {
                    var (pt, triggered) = party.AddFever(Math.Max(100, req.SubStageNpcCount * 100));
                    var not = new KEGS_PARTY_FEVER_POINT_NOT { PartyFever = (short)pt };
                    foreach (long uid in party.GetUnitUids())
                        Router.Send(PerformerId.PI_CN_PARTY, party.Info.PartyUid, PerformerId.PI_GS_USER, uid,
                            EventId.EPM_CHANGE_PARTY_FEVER_STATE_NOT, not);
                    if (triggered)
                        KLog.Clog($"[{Name}] Party[{party.Info.PartyUid}] triggered Party Fever!");
                }
                break;
            }

            case EventId.EPM_MAX_PARTY_FEVER_NOT:
            {
                var req = ev.GetPayload<KERM_MAX_PARTY_FEVER_NOT>(Region);
                var party = PartyManager.ResolveParty(req.PartyUid, ev.FirstSenderUid);
                if (party != null)
                {
                    int pt = party.IncreaseFeverByAdmin(req.DeltaFever);
                    var not = new KEGS_PARTY_FEVER_POINT_NOT { PartyFever = (short)pt };
                    foreach (long uid in party.GetUnitUids())
                        Router.Send(PerformerId.PI_CN_PARTY, party.Info.PartyUid, PerformerId.PI_GS_USER, uid,
                            EventId.EPM_CHANGE_PARTY_FEVER_STATE_NOT, not);
                }
                break;
            }

            case EventId.EPM_END_GAME_NOT:
            {
                var req = ev.GetPayload<KEPM_END_GAME_NOT>(Region);
                PartyManager.ResolveParty(req.PartyUid, ev.FirstSenderUid)?.ReturnToWait();
                break;
            }

            case EventId.EPM_DUNGEON_SECRET_STAGE_ENTER_CHECK_REQ:
            {
                var party = PartyManager.ResolveParty(ev.DestPerformer.Uids.FirstOrDefault(), ev.FirstSenderUid);
                bool allow = party != null && KDungeonSubStageManager.EvaluateSecretStageEntry(
                    party.Members.Select(m => (int)m.Level).DefaultIfEmpty(1).Max(),
                    SecretDungeonExtraStageConditions);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_CN_PARTY, party?.Info.PartyUid ?? 0, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.EPM_DUNGEON_SECRET_STAGE_ENTER_CHECK_ACK,
                        new KEGS_DUNGEON_SECRET_STAGE_ENTER_CHECK_ACK
                        {
                            Ok = allow ? NetError.NET_OK : NetError.ERR_ROOM_09,
                            CanEnterSecretStage = allow,
                        });
                break;
            }

            case EventId.EPM_GET_PARTY_INFO_FOR_AUTO_PARTY_MAKING_REQ:
            {
                var req = ev.GetPayload<KEPM_GET_PARTY_INFO_FOR_AUTO_PARTY_MAKING_REQ>(Region);
                var party = PartyManager.ResolveParty(req.PartyUid, ev.FirstSenderUid);
                if (ev.FirstSenderUid > 0)
                {
                    Router.Send(PerformerId.PI_CN_PARTY, req.PartyUid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.EPM_GET_PARTY_INFO_FOR_AUTO_PARTY_MAKING_ACK, new KEPM_GET_PARTY_INFO_FOR_AUTO_PARTY_MAKING_ACK
                        {
                            Ok = party != null ? NetError.NET_OK : NetError.ERR_PARTY_00,
                            PartyInfo = party?.Info ?? new KPartyInfo(),
                            Members = party?.SnapshotMembers() ?? [],
                        });
                }
                break;
            }

            case EventId.EPM_GET_PARTY_INFO_FOR_JOIN_BATTLE_FIELD_REQ:
            {
                var req = ev.GetPayload<KEPM_GET_PARTY_INFO_FOR_JOIN_BATTLE_FIELD_REQ>(Region);
                var party = PartyManager.GetPartyByUnitUid(ev.FirstSenderUid);
                if (ev.FirstSenderUid > 0)
                {
                    Router.Send(PerformerId.PI_CN_PARTY, party?.Info.PartyUid ?? 0, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.EPM_GET_PARTY_INFO_FOR_JOIN_BATTLE_FIELD_ACK, new KEPM_GET_PARTY_INFO_FOR_JOIN_BATTLE_FIELD_ACK
                        {
                            Ok = party != null ? NetError.NET_OK : NetError.ERR_PARTY_00,
                            BattleFieldJoinInfo = req.BattleFieldJoinInfo,
                            PartyMemberUids = party?.GetUnitUids() ?? [],
                        });
                }
                break;
            }

            case EventId.EPM_GET_PARTY_INFO_FOR_MATCH_MAKING_REQ:
            {
                var req = ev.GetPayload<KEPM_GET_PARTY_INFO_FOR_MATCH_MAKING_REQ>(Region);
                var party = PartyManager.ResolveParty(ev.DestPerformer.Uids.FirstOrDefault(), ev.FirstSenderUid);
                if (ev.FirstSenderUid > 0)
                {
                    Router.Send(PerformerId.PI_CN_PARTY, party?.Info.PartyUid ?? 0, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.EPM_GET_PARTY_INFO_FOR_MATCH_MAKING_ACK, new KEPM_GET_PARTY_INFO_FOR_MATCH_MAKING_ACK
                        {
                            Ok = party != null ? NetError.NET_OK : NetError.ERR_PARTY_00,
                            RegFrontOfList = req.RegFrontOfList,
                            PvpPlayerCount = req.PvpPlayerCount,
                            PvpGameType = req.PvpGameType,
                            Members = party?.SnapshotMembers() ?? [],
                        });
                }
                break;
            }

            case EventId.EPM_REGROUP_PARTY_FOR_AUTO_PARTY_DUNGEON_GAME_REQ:
            {
                var req = ev.GetPayload<KEPM_REGROUP_PARTY_FOR_AUTO_PARTY_DUNGEON_GAME_REQ>(Region);
                var host = req.Members.FirstOrDefault(m => m.UnitUid == req.AutoPartyHostUnitUid)
                           ?? req.Members.FirstOrDefault()
                           ?? new KPartyUserInfo { UnitUid = req.AutoPartyHostUnitUid };
                var createInfo = req.CreatePartyInfo ?? new KCreatePartyInfo { PartyType = 1 };
                if (createInfo.DungeonId <= 0)
                    createInfo.DungeonId = 30000;
                var party = PartyManager.CreateParty(createInfo, host);
                foreach (var m in req.Members.Where(m => m.UnitUid != host.UnitUid))
                    party.TryJoin(m);
                Router.Send(PerformerId.PI_CN_PARTY, party.Info.PartyUid, PerformerId.PI_GB_SERVER, 0,
                    EventId.EGB_REGROUP_PARTY_FOR_AUTO_PARTY_DUNGEON_GAME_ACK,
                    new KEPM_REGROUP_PARTY_FOR_AUTO_PARTY_DUNGEON_GAME_ACK
                    {
                        Ok = NetError.NET_OK,
                        PartyUid = party.Info.PartyUid,
                        AutoPartyUid = req.AutoPartyUid,
                    });
                break;
            }

            case EventId.EPM_UPDATE_PARTY_USER_INFO_NOT:
            {
                var req = ev.GetPayload<KEPM_UPDATE_PARTY_USER_INFO_NOT>(Region);
                var party = PartyManager.ResolveParty(req.PartyUid, ev.FirstSenderUid);
                if (party != null)
                {
                    foreach (long uid in party.GetUnitUids())
                        Router.Send(PerformerId.PI_CN_PARTY, party.Info.PartyUid, PerformerId.PI_GS_USER, uid,
                            EventId.EPM_UPDATE_PARTY_USER_INFO_NOT, req);
                }
                break;
            }

            case EventId.EPM_UPDATE_GAME_PLAY_STATUS_NOT:
            {
                var req = ev.GetPayload<KEPM_UPDATE_GAME_PLAY_STATUS_NOT>(Region);
                var party = PartyManager.ResolveParty(req.PartyUid, ev.FirstSenderUid);
                if (party != null && !req.Playing)
                    party.ReturnToWait();
                break;
            }

            case EventId.EPM_CHECK_INVALID_USER_NOT:
            {
                if (ev.FirstSenderUid > 0)
                {
                    var p = PartyManager.GetPartyByUnitUid(ev.FirstSenderUid);
                    if (p != null) PartyManager.LeaveParty(p.Info.PartyUid, ev.FirstSenderUid);
                }
                break;
            }

            // ---- Additional ERM_* Room Combat / Management / Trade / Shop / Field Handlers ----
            case EventId.ERM_BAN_USER_REQ:
            {
                var req = ev.GetPayload<KEGS_BAN_USER_REQ>(Region);
                var room = RoomManager.ResolveRoom(ev.DestPerformer.Uids.FirstOrDefault(), ev.FirstSenderUid);
                bool ok = room != null && room.BanUser(ev.FirstSenderUid, req.UnitUid);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_CN_ROOM, room?.Info.RoomUid ?? 0, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ERM_BAN_USER_ACK, new KPacketOK { Ok = ok ? NetError.NET_OK : NetError.ERR_ROOM_28 });
                if (ok && room != null)
                    Router.Send(PerformerId.PI_CN_ROOM, room.Info.RoomUid, PerformerId.PI_GS_USER, req.UnitUid,
                        EventId.ERM_BAN_USER_NOT, new KPacketOK { Ok = NetError.NET_OK });
                break;
            }

            case EventId.ERM_CHANGE_MAP_REQ:
            {
                var req = ev.GetPayload<KEGS_CHANGE_MAP_REQ>(Region);
                var room = RoomManager.ResolveRoom(ev.DestPerformer.Uids.FirstOrDefault(), ev.FirstSenderUid);
                bool ok = room != null && room.ChangeMap(ev.FirstSenderUid, req.WorldId);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_CN_ROOM, room?.Info.RoomUid ?? 0, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ERM_CHANGE_MAP_ACK, new KPacketOK { Ok = ok ? NetError.NET_OK : NetError.ERR_ROOM_28 });
                if (ok && room != null)
                {
                    foreach (long uid in room.GetMemberUnitUids())
                        Router.Send(PerformerId.PI_CN_ROOM, room.Info.RoomUid, PerformerId.PI_GS_USER, uid,
                            EventId.ERM_CHANGE_MAP_NOT, req);
                }
                break;
            }

            case EventId.ERM_CHANGE_READY_REQ:
            {
                var req = ev.GetPayload<KEGS_CHANGE_READY_REQ>(Region);
                var room = RoomManager.ResolveRoom(ev.DestPerformer.Uids.FirstOrDefault(), ev.FirstSenderUid);
                bool ok = room != null && room.SetReady(ev.FirstSenderUid, req.Ready);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_CN_ROOM, room?.Info.RoomUid ?? 0, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ERM_CHANGE_READY_ACK, new KERM_CHANGE_READY_ACK
                        {
                            Ok = ok ? NetError.NET_OK : NetError.ERR_ROOM_09,
                            DungeonId = room?.Info.DungeonId ?? 0,
                            DungeonMode = room?.Info.DungeonMode ?? 0,
                        });
                if (ok && room != null)
                {
                    foreach (long uid in room.GetMemberUnitUids())
                        Router.Send(PerformerId.PI_CN_ROOM, room.Info.RoomUid, PerformerId.PI_GS_USER, uid,
                            EventId.ERM_CHANGE_READY_NOT, req);
                }
                break;
            }

            case EventId.ERM_CHANGE_TEAM_REQ:
            {
                var req = ev.GetPayload<KEGS_CHANGE_TEAM_REQ>(Region);
                var room = RoomManager.ResolveRoom(ev.DestPerformer.Uids.FirstOrDefault(), ev.FirstSenderUid);
                var res = room?.SetTeam(ev.FirstSenderUid, req.TeamNum) ?? (false, (sbyte)-1);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_CN_ROOM, room?.Info.RoomUid ?? 0, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ERM_CHANGE_TEAM_ACK, new KPacketOK { Ok = res.ok ? NetError.NET_OK : NetError.ERR_ROOM_09 });
                if (res.ok && room != null)
                {
                    foreach (long uid in room.GetMemberUnitUids())
                        Router.Send(PerformerId.PI_CN_ROOM, room.Info.RoomUid, PerformerId.PI_GS_USER, uid,
                            EventId.ERM_CHANGE_TEAM_NOT, req);
                }
                break;
            }

            case EventId.ERM_CHANGE_SLOT_OPEN_REQ:
            {
                var req = ev.GetPayload<KEGS_CHANGE_SLOT_OPEN_REQ>(Region);
                var room = RoomManager.ResolveRoom(ev.DestPerformer.Uids.FirstOrDefault(), ev.FirstSenderUid);
                var (ok, _) = room?.ChangeSlotOpen(ev.FirstSenderUid, req.SlotIndex, req.SlotState) ?? (NetError.ERR_ROOM_09, (sbyte)-1);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_CN_ROOM, room?.Info.RoomUid ?? 0, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ERM_CHANGE_SLOT_OPEN_ACK, new KPacketOK { Ok = ok });
                if (ok == NetError.NET_OK && room != null)
                {
                    foreach (long uid in room.GetMemberUnitUids())
                        Router.Send(PerformerId.PI_CN_ROOM, room.Info.RoomUid, PerformerId.PI_GS_USER, uid,
                            EventId.ERM_CHANGE_SLOT_OPEN_NOT, req);
                }
                break;
            }

            case EventId.ERM_CHANGE_WINNING_NUM_KILL_REQ:
            {
                var req = ev.GetPayload<KEGS_CHANGE_WINNING_NUM_KILL_REQ>(Region);
                var room = RoomManager.ResolveRoom(ev.DestPerformer.Uids.FirstOrDefault(), ev.FirstSenderUid);
                int ok = room?.ChangeWinningNumKill(ev.FirstSenderUid, req.WinKillNum) ?? NetError.ERR_ROOM_09;
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_CN_ROOM, room?.Info.RoomUid ?? 0, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ERM_CHANGE_WINNING_NUM_KILL_ACK, new KPacketOK { Ok = ok });
                break;
            }

            case EventId.ERM_CHANGE_PVP_GAME_OPTION_REQ:
            {
                var req = ev.GetPayload<KEGS_CHANGE_PVP_GAME_OPTION_REQ>(Region);
                var room = RoomManager.ResolveRoom(ev.DestPerformer.Uids.FirstOrDefault(), ev.FirstSenderUid);
                int ok = room?.ChangePvpGameOption(ev.FirstSenderUid, req.BaseDefence, req.BossCombat) ?? NetError.ERR_ROOM_09;
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_CN_ROOM, room?.Info.RoomUid ?? 0, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ERM_CHANGE_PVP_GAME_OPTION_ACK, new KPacketOK { Ok = ok });
                break;
            }

            case EventId.ERM_CHANGE_PVP_ITEMMODE_REQ:
            {
                var room = RoomManager.ResolveRoom(ev.DestPerformer.Uids.FirstOrDefault(), ev.FirstSenderUid);
                int ok = room != null ? room.TogglePvpItemMode(out _) : NetError.ERR_ROOM_09;
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_CN_ROOM, room?.Info.RoomUid ?? 0, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ERM_CHANGE_PVP_ITEMMODE_ACK, new KPacketOK { Ok = ok });
                break;
            }

            case EventId.ERM_CREATE_PVP_ITEM_REQ:
            {
                var req = ev.GetPayload<KEGS_CREATE_PVP_ITEM_REQ>(Region);
                var room = RoomManager.ResolveRoom(ev.DestPerformer.Uids.FirstOrDefault(), ev.FirstSenderUid);
                int ok = room != null ? room.CreatePvpItem(req.PositionIndex, 0, out _) : NetError.ERR_ROOM_09;
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_CN_ROOM, room?.Info.RoomUid ?? 0, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ERM_CREATE_PVP_ITEM_ACK, new KPacketOK { Ok = ok });
                break;
            }

            case EventId.ERM_CHANGE_PVP_ITEM_POS_REQ:
            {
                var req = ev.GetPayload<KEGS_CHANGE_PVP_ITEM_POS_REQ>(Region);
                var room = RoomManager.ResolveRoom(ev.DestPerformer.Uids.FirstOrDefault(), ev.FirstSenderUid);
                int ok = room?.ChangePvpItemPos(req.ItemUid, req.Pos) ?? NetError.ERR_ROOM_09;
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_CN_ROOM, room?.Info.RoomUid ?? 0, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ERM_CHANGE_PVP_ITEM_POS_ACK, new KPacketOK { Ok = ok });
                break;
            }

            case EventId.ERM_GAME_LOADING_REQ:
            {
                var req = ev.GetPayload<KEGS_GAME_LOADING_REQ>(Region);
                var room = RoomManager.ResolveRoom(ev.DestPerformer.Uids.FirstOrDefault(), ev.FirstSenderUid);
                bool ok = room != null && room.UpdateLoading(ev.FirstSenderUid, req.LoadingProgress, out _);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_CN_ROOM, room?.Info.RoomUid ?? 0, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ERM_GAME_LOADING_ACK, new KPacketOK { Ok = ok ? NetError.NET_OK : NetError.ERR_ROOM_09 });
                break;
            }

            case EventId.ERM_END_GAME_REQ:
            {
                var room = RoomManager.ResolveRoom(ev.DestPerformer.Uids.FirstOrDefault(), ev.FirstSenderUid);
                if (room != null)
                {
                    room.EndGame();
                    foreach (long uid in room.GetMemberUnitUids())
                        Router.Send(PerformerId.PI_CN_ROOM, room.Info.RoomUid, PerformerId.PI_GS_USER, uid,
                            EventId.ERM_END_GAME_NOT, new KERM_END_GAME_NOT());
                }
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_CN_ROOM, room?.Info.RoomUid ?? 0, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ERM_END_GAME_ACK, new KPacketOK { Ok = room != null ? NetError.NET_OK : NetError.ERR_ROOM_09 });
                break;
            }

            case EventId.ERM_LEAVE_GAME_REQ:
            {
                var room = RoomManager.ResolveRoom(ev.DestPerformer.Uids.FirstOrDefault(), ev.FirstSenderUid);
                var (ok, _) = room?.LeaveGame(ev.FirstSenderUid) ?? (NetError.ERR_ROOM_09, false);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_CN_ROOM, room?.Info.RoomUid ?? 0, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ERM_LEAVE_GAME_ACK, new KPacketOK { Ok = ok });
                break;
            }

            case EventId.ERM_INTRUDE_START_REQ:
            {
                var room = RoomManager.ResolveRoom(ev.DestPerformer.Uids.FirstOrDefault(), ev.FirstSenderUid);
                var (ok, _, _) = room?.IntrudeStart(ev.FirstSenderUid) ?? (NetError.ERR_ROOM_09, null, false);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_CN_ROOM, room?.Info.RoomUid ?? 0, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ERM_INTRUDE_START_ACK, new KPacketOK { Ok = ok });
                break;
            }

            case EventId.ERM_RESULT_SUCCESS_REQ:
            {
                var room = RoomManager.ResolveRoom(ev.DestPerformer.Uids.FirstOrDefault(), ev.FirstSenderUid);
                var (ok, _) = room?.ConfirmResultSuccess(ev.FirstSenderUid) ?? (NetError.ERR_ROOM_09, false);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_CN_ROOM, room?.Info.RoomUid ?? 0, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ERM_RESULT_SUCCESS_ACK, new KEGS_RESULT_SUCCESS_ACK
                        {
                            Ok = ok,
                            DungeonId = room?.Info.DungeonId ?? 0,
                        });
                break;
            }

            case EventId.ERM_BAD_ATTITUDE_USER_CHECK_INFO_NOT:
            {
                var req = ev.GetPayload<KEGS_BAD_ATTITUDE_USER_CHECK_INFO_NOT>(Region);
                var room = RoomManager.ResolveRoom(ev.DestPerformer.Uids.FirstOrDefault(), ev.FirstSenderUid);
                room?.BadAttitudeManager.UpdateUserCheckInfo(ev.FirstSenderUid, req.StageId, req.SubStageId, req.ComboScore, req.TechScore);
                break;
            }

            case EventId.ERM_BATTLE_FIELD_DANGEROUS_VALUE_REQ:
            {
                var room = RoomManager.ResolveRoom(ev.DestPerformer.Uids.FirstOrDefault(), ev.FirstSenderUid);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_CN_ROOM, room?.Info.RoomUid ?? 0, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ERM_BATTLE_FIELD_DANGEROUS_VALUE_ACK,
                        new KPacketOK { Ok = room?.BattleFieldGameManager.DangerousValue ?? 0 });
                break;
            }

            case EventId.ERM_ADMIN_BATTLE_FIELD_INCREASE_DANGER_REQ:
            {
                var req = ev.GetPayload<KEGS_ADMIN_BATTLE_FIELD_INCREASE_DANGER_REQ>(Region);
                var room = RoomManager.ResolveRoom(ev.DestPerformer.Uids.FirstOrDefault(), ev.FirstSenderUid);
                room?.BattleFieldGameManager.IncreaseDangerousValue(req.Danger);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_CN_ROOM, room?.Info.RoomUid ?? 0, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ERM_ADMIN_BATTLE_FIELD_INCREASE_DANGER_ACK,
                        new KEGS_ADMIN_BATTLE_FIELD_INCREASE_DANGER_ACK
                        {
                            Ok = room != null ? NetError.NET_OK : NetError.ERR_ROOM_09,
                            Danger = room?.BattleFieldGameManager.DangerousValue ?? 0,
                        });
                break;
            }

            case EventId.ERM_ADMIN_NPC_UNIT_CREATE_REQ:
            {
                var req = ev.GetPayload<KEGS_ADMIN_NPC_UNIT_CREATE_REQ>(Region);
                var room = RoomManager.ResolveRoom(ev.DestPerformer.Uids.FirstOrDefault(), ev.FirstSenderUid);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_CN_ROOM, room?.Info.RoomUid ?? 0, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ERM_ADMIN_NPC_UNIT_CREATE_ACK, new KPacketOK { Ok = room != null ? NetError.NET_OK : NetError.ERR_ROOM_09 });
                if (room != null)
                {
                    foreach (long uid in room.GetMemberUnitUids())
                        Router.Send(PerformerId.PI_CN_ROOM, room.Info.RoomUid, PerformerId.PI_GS_USER, uid,
                            EventId.ERM_ADMIN_NPC_UNIT_CREATE_NOT, req);
                }
                break;
            }

            case EventId.ERM_APPROVE_TRADE_REQ:
            {
                long tradeUid = ev.DestPerformer.Uids.FirstOrDefault();
                int ok = NetError.ERR_ROOM_09;
                if (TradeRooms.TryGetValue(tradeUid, out var tr))
                    (ok, _) = tr.ApproveTrade(ev.FirstSenderUid);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_CN_ROOM, tradeUid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ERM_APPROVE_TRADE_ACK, new KPacketOK { Ok = ok });
                break;
            }

            case EventId.ERM_BREAK_TRADE_REQ:
            {
                var req = ev.GetPayload<KERM_JOIN_TRADE_REQ>(Region);
                long tradeUid = req.TradeUid > 0 ? req.TradeUid : ev.DestPerformer.Uids.FirstOrDefault();
                if (TradeRooms.TryGetValue(tradeUid, out var tr))
                    tr.BreakTrade(NetError.NOT_LEAVE_ROOM_REASON_08);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_CN_ROOM, tradeUid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ERM_BREAK_TRADE_ACK, new KPacketOK { Ok = NetError.NET_OK });
                break;
            }

            case EventId.ERM_BUY_PERSONAL_SHOP_ITEM_REQ:
            {
                var req = ev.GetPayload<KERM_BUY_PERSONAL_SHOP_ITEM_REQ>(Region);
                if (PersonalShopRooms.TryGetValue(req.ShopUid, out var shop))
                {
                    var (ok, totalCost, netProceeds, fee, soldOut) =
                        shop.BuyItem(req.BuyerUnitUid, req.ItemUid, req.Quantity, req.ViaTradeBoard);
                    if (ev.FirstSenderUid > 0)
                        Router.Send(PerformerId.PI_CN_ROOM, req.ShopUid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                            EventId.ERM_BUY_PERSONAL_SHOP_ITEM_ACK, new KERM_BUY_PERSONAL_SHOP_ITEM_ACK
                            {
                                Ok = ok,
                                TotalCost = totalCost,
                                NetSellerProceeds = netProceeds,
                                CommissionFee = fee,
                                ShopSoldOut = soldOut,
                            });
                }
                break;
            }

            case EventId.ERM_SET_TC_REMAINING_TIME_REQ:
            {
                var req = ev.GetPayload<KEGS_SET_TC_REMAINING_TIME_REQ>(Region);
                var room = RoomManager.ResolveRoom(ev.DestPerformer.Uids.FirstOrDefault(), ev.FirstSenderUid);
                int ok = room?.SetTcRemainingTime(req.Enable) ?? NetError.ERR_ROOM_09;
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_CN_ROOM, room?.Info.RoomUid ?? 0, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ERM_SET_TC_REMAINING_TIME_ACK, new KPacketOK { Ok = ok });
                break;
            }

            case EventId.ERM_DUNGEON_SECRET_STAGE_ENTER_CHECK_REQ:
            {
                var room = RoomManager.ResolveRoom(ev.DestPerformer.Uids.FirstOrDefault(), ev.FirstSenderUid);
                bool enter = room != null && KDungeonSubStageManager.EvaluateSecretStageEntry(
                    room.Slots.Where(s => s.RoomUserInfo.UnitUid > 0).Select(s => (int)s.RoomUserInfo.Level).DefaultIfEmpty(1).Max(),
                    SecretDungeonExtraStageConditions);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_CN_ROOM, room?.Info.RoomUid ?? 0, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ERM_DUNGEON_SECRET_STAGE_ENTER_CHECK_ACK,
                        new KEGS_DUNGEON_SECRET_STAGE_ENTER_CHECK_ACK
                        {
                            Ok = enter ? NetError.NET_OK : NetError.ERR_ROOM_09,
                            CanEnterSecretStage = enter,
                        });
                break;
            }

            case EventId.ERM_ADD_UDP_CHECK_KICK_USER_NOT:
            {
                var pkt = ev.GetPayload<KPacketLong>(Region);
                var room = RoomManager.ResolveRoom(ev.DestPerformer.Uids.FirstOrDefault(), pkt.Value);
                if (room != null && pkt.Value > 0)
                    RoomManager.LeaveRoom(room.Info.RoomUid, pkt.Value);
                break;
            }

            case EventId.ERM_BUFF_SKILL_INFO_CONVEY_REQ:
            {
                var req = ev.GetPayload<KEGS_BUFF_SKILL_INFO_CONVEY_REQ>(Region);
                var room = RoomManager.ResolveRoom(ev.DestPerformer.Uids.FirstOrDefault(), ev.FirstSenderUid);
                if (room != null)
                {
                    var (ok, not) = room.ProcessBuffSkill(req);
                    if (ok)
                    {
                        foreach (long uid in room.GetMemberUnitUids())
                            Router.Send(PerformerId.PI_CN_ROOM, room.Info.RoomUid, PerformerId.PI_GS_USER, uid,
                                EventId.ERM_BUFF_SKILL_INFO_CONVEY_NOT, not);
                    }
                }
                break;
            }

            case EventId.ERM_GET_ITEM_REQ:
            {
                var req = ev.GetPayload<KERM_GET_ITEM_REQ>(Region);
                var room = RoomManager.ResolveRoom(req.RoomUid, req.UnitUid);
                room?.TryClaimDropItem(req.DropItemUid, out _);
                break;
            }

            case EventId.ERM_DUNGEON_PLAY_INFO_TO_SERVER_NOT:
            {
                var req = ev.GetPayload<KERM_DUNGEON_PLAY_INFO_TO_SERVER_NOT>(Region);
                var room = RoomManager.ResolveRoom(req.RoomUid, req.UnitUid);
                room?.RecordDungeonStats(req.UnitUid, req.ComboScore, req.TechScore, req.Damaged, req.NpcKills);
                break;
            }

            case EventId.ERM_ADD_ON_STAT_REQ:
            {
                var req = ev.GetPayload<KERM_ADD_ON_STAT_REQ>(Region);
                var room = RoomManager.ResolveRoom(req.RoomUid, req.KillerUnitUid);
                room?.RecordPvpKill(req.KillerUnitUid, req.VictimUnitUid, req.IsMidKill);
                break;
            }

            case EventId.ERM_CHECK_INVALID_USER_NOT:
            {
                if (ev.FirstSenderUid > 0)
                {
                    var room = RoomManager.GetRoomByUnitUid(ev.FirstSenderUid);
                    if (room != null) RoomManager.LeaveRoom(room.Info.RoomUid, ev.FirstSenderUid);
                }
                break;
            }

            // ---- Remaining ECN_* / EPM_* / ELOG_* Handlers ----
            case EventId.ECN_CHECK_CHANNEL_CHANGE_ACK:
                if (ev.FirstSenderUid > 0 && ev.InProcessPayload != null)
                    Router.Send(PerformerId.PI_CN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ECN_CHECK_CHANNEL_CHANGE_ACK, ev.InProcessPayload);
                break;

            case EventId.ECN_UPDATE_SERVER_INFO_ACK:
            {
                var ack = ev.GetPayload<KServerList>(Region);
                foreach (var s in ack.ServerList)
                    if (s.Uid > 0) _connectedGameServers[s.Uid] = s;
                break;
            }

            case EventId.EPM_CHECK_DUNGEON_GAME_PLAY_CONDITION_ACK:
            {
                var ack = ev.GetPayload<KEPM_CHECK_DUNGEON_GAME_PLAY_CONDITION_ACK>(Region);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_CN_PARTY, 0, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.EPM_CHECK_DUNGEON_GAME_PLAY_CONDITION_ACK, ack);
                break;
            }

            case EventId.EPM_CHECK_FOR_AUTO_PARTY_MAKING_ACK:
            {
                var ack = ev.GetPayload<KEPM_CHECK_FOR_AUTO_PARTY_MAKING_ACK>(Region);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_CN_PARTY, 0, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.EPM_CHECK_FOR_AUTO_PARTY_MAKING_ACK, ack);
                break;
            }

            case EventId.EPM_CHECK_FOR_PARTY_GAME_START_ACK:
            {
                var ack = ev.GetPayload<KEPM_CHECK_FOR_PARTY_GAME_START_ACK>(Region);
                var party = PartyManager.ResolveParty(ack.PartyUid, ev.FirstSenderUid);
                if (party != null && ack.Ok == NetError.NET_OK)
                    party.Fsm.TryStateTransition(KCnPartyFSM.I_TO_PREPARE_FOR_PLAY, out _);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_CN_PARTY, party?.Info.PartyUid ?? 0, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.EPM_PARTY_GAME_START_ACK, new KEPM_PARTY_GAME_START_ACK
                        {
                            Ok = ack.Ok,
                            DungeonId = party?.Info.DungeonId ?? 0,
                            DungeonMode = party?.Info.DungeonMode ?? 0,
                        });
                break;
            }

            case EventId.EPM_CHECK_PARTY_CHANGE_HOST_ACK:
            {
                var ack = ev.GetPayload<KEPM_CHECK_PARTY_CHANGE_HOST_ACK>(Region);
                var party = PartyManager.ResolveParty(ack.PartyUid, ack.OldHostUnitUid);
                bool ok = ack.Ok == NetError.NET_OK && party != null &&
                          PartyManager.ChangeHost(party.Info.PartyUid, ack.OldHostUnitUid, ack.NewHostUnitUid);
                if (ok && party != null)
                {
                    foreach (long uid in party.GetUnitUids())
                        Router.Send(PerformerId.PI_CN_PARTY, party.Info.PartyUid, PerformerId.PI_GS_USER, uid,
                            EventId.EPM_PARTY_CHANGE_HOST_NOT, ack);
                }
                break;
            }

            case EventId.EPM_PARTY_GAME_START_OPINION_CHECK_REPLY_NOT:
            {
                var req = ev.GetPayload<KEGS_PARTY_GAME_START_OPINION_CHECK_REPLY_NOT>(Region);
                var party = PartyManager.GetPartyByUnitUid(ev.FirstSenderUid);
                if (party != null)
                {
                    foreach (long uid in party.GetUnitUids())
                        Router.Send(PerformerId.PI_CN_PARTY, party.Info.PartyUid, PerformerId.PI_GS_USER, uid,
                            EventId.EPM_PARTY_GAME_START_OPINION_CHECK_REPLY_NOT, req);
                }
                break;
            }

            case EventId.EPM_REQUEST_REGROUP_PARTY_NOT:
            {
                var req = ev.GetPayload<KEPM_REQUEST_REGROUP_PARTY_NOT>(Region);
                if (req.Members.Count > 0)
                {
                    var host = req.Members.FirstOrDefault(m => m.UnitUid == req.OldHostUnitUid) ?? req.Members[0];
                    var np = PartyManager.CreateParty(req.CreatePartyInfo, host);
                    foreach (var m in req.Members)
                    {
                        if (m.UnitUid != host.UnitUid)
                            PartyManager.TryJoinParty(np.Info.PartyUid, m, out _);
                    }
                    if (req.PartyFever > 0) np.IncreaseFeverByAdmin(req.PartyFever);
                }
                break;
            }

            case EventId.EPM_REQUEST_UPDATE_PARTY_MEMBER_LIST_NOT:
            {
                var req = ev.GetPayload<KEPM_REQUEST_UPDATE_PARTY_MEMBER_LIST_NOT>(Region);
                var party = PartyManager.ResolveParty(req.PartyUid, ev.FirstSenderUid);
                if (party != null)
                {
                    foreach (long uid in party.GetUnitUids())
                        Router.Send(PerformerId.PI_CN_PARTY, party.Info.PartyUid, PerformerId.PI_GS_USER, uid,
                            EventId.EPM_OPEN_PARTY_LIST_NOT, party.Info);
                }
                break;
            }

            case EventId.EPM_UPDATE_PARTY_USER_JOIN_ROOM_INFO_NOT:
            {
                var req = ev.GetPayload<KEPM_UPDATE_PARTY_USER_JOIN_ROOM_INFO_NOT>(Region);
                PartyManager.UpdateMemberJoinRoom(req.UnitUid, req.State, req.StateCode);
                break;
            }

            case EventId.EPM_UPDATE_PVP_INFO_NOT:
            {
                var req = ev.GetPayload<KEPM_UPDATE_PVP_INFO_NOT>(Region);
                PartyManager.UpdateMemberPvpInfo(req.UnitUid, req.Rank, req.OfficialMatchCnt, req.Rating, req.RPoint);
                break;
            }

            case EventId.ELOG_APPROPRIATE_LEVEL_STATISTICS_NOT:
            case EventId.ELOG_BATTLE_FIELD_LEAVE_LOG_NOT:
            case EventId.ELOG_CHAT_LOG_NOT:
            case EventId.ELOG_COLLECTION_OF_RELAY_AND_P2P_INFO_NOT:
            case EventId.ELOG_DUNGEON_STAGE_LOG_NOT:
            case EventId.ELOG_GEN_FIELD_MIDDLE_BOSS_STAT_NOT:
            case EventId.ELOG_HENIR_DUNGEON_CLEAR_NOT:
            case EventId.ELOG_HENIR_HACKING_LOG_NOT:
            case EventId.ELOG_INSERT_ABUSER_LOG_NOT:
            case EventId.ELOG_INSERT_PERSONAL_SHOP_LOG_NOT:
            case EventId.ELOG_INSERT_TRADE_LOG_NOT:
            case EventId.ELOG_KILL_FIELD_MIDDLE_BOSS_STAT_NOT:
            case EventId.ELOG_STAT_RES_STONE:
                ServerLogCounters.AddOrUpdate(ev.EventId, 1, static (_, cur) => cur + 1);
                break;

            // ---- Remaining ERM_* Room / Dungeon / PvP / BattleField / Square / Wedding / Trade / PersonalShop ----
            case EventId.ERM_CHAT_REQ:
            {
                var req = ev.GetPayload<KEGS_CHAT_REQ>(Region);
                var room = RoomManager.ResolveRoom(ev.DestPerformer.Uids.FirstOrDefault(), ev.FirstSenderUid);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_CN_ROOM, room?.Info.RoomUid ?? 0, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ERM_CHAT_ACK, new KERM_CHAT_ACK
                        {
                            Ok = room != null ? NetError.NET_OK : NetError.ERR_ROOM_16,
                            ChatPacketType = req.ChatPacketType,
                        });
                if (room != null)
                {
                    var senderSlot = room.Slots.FirstOrDefault(s => s.RoomUserInfo.UnitUid == ev.FirstSenderUid);
                    var not = new KEGS_CHAT_NOT
                    {
                        ChatPacketType = req.ChatPacketType,
                        RoomChatType = req.RoomChatType,
                        SenderUnitUid = ev.FirstSenderUid,
                        SenderNickName = senderSlot?.RoomUserInfo.NickName ?? string.Empty,
                        ReceiverUnitUid = req.ToUnitUid,
                        ReceiverNickName = req.ReceiverNickName,
                        Message = req.Message,
                    };
                    foreach (long uid in room.GetMemberUnitUids())
                        Router.Send(PerformerId.PI_CN_ROOM, room.Info.RoomUid, PerformerId.PI_GS_USER, uid,
                            EventId.ERM_CHAT_NOT, not);
                }
                break;
            }

            case EventId.ERM_CHANGE_MOTION_REQ:
            case EventId.ERM_CHANGE_PITIN_REQ:
            case EventId.ERM_CHANGE_PLAY_TIME_LIMIT_REQ:
            case EventId.ERM_CHANGE_PVP_ROOM_NAME_REQ:
            case EventId.ERM_CHANGE_PVP_ROOM_PUBLIC_REQ:
            {
                var room = RoomManager.ResolveRoom(ev.DestPerformer.Uids.FirstOrDefault(), ev.FirstSenderUid);
                int ok = NetError.NET_OK;
                EventId ackId = EventId.ERM_CHANGE_MOTION_ACK;
                EventId notId = EventId.ERM_CHANGE_MOTION_NOT;
                object? notPayload = null;
                if (room == null)
                {
                    ok = NetError.ERR_ROOM_16;
                }
                else if (ev.EventId == EventId.ERM_CHANGE_MOTION_REQ)
                {
                    notPayload = ev.GetPayload<KEGS_CHANGE_MOTION_REQ>(Region);
                    ackId = EventId.ERM_CHANGE_MOTION_ACK;
                    notId = EventId.ERM_CHANGE_MOTION_NOT;
                }
                else if (ev.EventId == EventId.ERM_CHANGE_PITIN_REQ)
                {
                    var r = ev.GetPayload<KEGS_CHANGE_PITIN_REQ>(Region);
                    if (!room.SetPitIn(ev.FirstSenderUid, r.PitIn)) ok = NetError.ERR_ROOM_09;
                    notPayload = r;
                    ackId = EventId.ERM_CHANGE_PITIN_ACK;
                    notId = EventId.ERM_CHANGE_PITIN_NOT;
                }
                else if (ev.EventId == EventId.ERM_CHANGE_PLAY_TIME_LIMIT_REQ)
                {
                    var r = ev.GetPayload<KEGS_CHANGE_PLAY_TIME_LIMIT_REQ>(Region);
                    if (!room.ChangePlayTime(ev.FirstSenderUid, r.PlayTime)) ok = NetError.ERR_ROOM_09;
                    notPayload = r;
                    ackId = EventId.ERM_CHANGE_PLAY_TIME_LIMIT_ACK;
                    notId = EventId.ERM_CHANGE_PLAY_TIME_LIMIT_NOT;
                }
                else if (ev.EventId == EventId.ERM_CHANGE_PVP_ROOM_NAME_REQ)
                {
                    var r = ev.GetPayload<KEGS_CHANGE_PVP_ROOM_NAME_REQ>(Region);
                    if (!room.ChangeRoomName(ev.FirstSenderUid, r.RoomName)) ok = NetError.ERR_ROOM_09;
                    notPayload = r;
                    ackId = EventId.ERM_CHANGE_PVP_ROOM_NAME_ACK;
                    notId = EventId.ERM_CHANGE_PVP_ROOM_NAME_NOT;
                }
                else
                {
                    var r = ev.GetPayload<KEGS_CHANGE_PVP_ROOM_PUBLIC_REQ>(Region);
                    if (!room.ChangePublic(ev.FirstSenderUid, r.Public, r.Password)) ok = NetError.ERR_ROOM_09;
                    notPayload = r;
                    ackId = EventId.ERM_CHANGE_PVP_ROOM_PUBLIC_ACK;
                    notId = EventId.ERM_CHANGE_PVP_ROOM_PUBLIC_NOT;
                }
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_CN_ROOM, room?.Info.RoomUid ?? 0, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        ackId, new KPacketOK { Ok = ok });
                if (ok == NetError.NET_OK && room != null && notPayload != null)
                {
                    foreach (long uid in room.GetMemberUnitUids())
                        Router.Send(PerformerId.PI_CN_ROOM, room.Info.RoomUid, PerformerId.PI_GS_USER, uid,
                            notId, notPayload);
                }
                break;
            }

            case EventId.ERM_CHANGE_GAME_STAT_INFO_IN_ROOM_NOT:
            case EventId.ERM_CHANGE_ROOM_USER_INFO_NOT:
            case EventId.ERM_CHANGE_SKILL_INFO_IN_ROOM_NOT:
            case EventId.ERM_SET_UNIT_INFO_NOT:
            {
                var info = ev.GetPayload<KRoomUserInfo>(Region);
                var room = RoomManager.ResolveRoom(ev.DestPerformer.Uids.FirstOrDefault(), info.UnitUid);
                if (room != null)
                {
                    RoomManager.UpdateRoomUserInfo(room, info);
                    foreach (long uid in room.GetMemberUnitUids())
                        Router.Send(PerformerId.PI_CN_ROOM, room.Info.RoomUid, PerformerId.PI_GS_USER, uid,
                            ev.EventId, info);
                }
                break;
            }

            case EventId.ERM_CHAR_LEVEL_UP_NOT:
            {
                var req = ev.GetPayload<KERM_CHAR_LEVEL_UP_NOT>(Region);
                var room = RoomManager.ResolveRoom(ev.DestPerformer.Uids.FirstOrDefault(), req.RoomUserInfo.UnitUid);
                if (room != null)
                {
                    RoomManager.UpdateRoomUserInfo(room, req.RoomUserInfo);
                    foreach (long uid in room.GetMemberUnitUids())
                        Router.Send(PerformerId.PI_CN_ROOM, room.Info.RoomUid, PerformerId.PI_GS_USER, uid,
                            EventId.ERM_CHAR_LEVEL_UP_NOT, req);
                }
                break;
            }

            case EventId.ERM_CHANGE_EQUIPPED_ITEM_IN_ROOM_NOT:
            case EventId.ERM_REPAIR_ITEM_NOT:
            case EventId.ERM_ZERO_ENDURANCE_ITEM_IN_ROOM_NOT:
            case EventId.ERM_CHANGED_PET_INTIMACY_NOT:
            case EventId.ERM_CHANGED_PET_SATIETY_NOT:
            case EventId.ERM_CHANGED_PET_SATIETY_INTIMACY_NOT:
            case EventId.ERM_PET_ACTION_NOT:
            case EventId.ERM_PET_EVOLUTION_NOT:
            case EventId.ERM_SUMMON_PET_NOT:
            case EventId.ERM_SUMMON_RIDING_PET_NOT:
            case EventId.ERM_UNSUMMON_RIDING_PET_NOT:
            case EventId.ERM_RELATIONSHIP_EFFECT_NOT:
            case EventId.ERM_COUPLE_MAKING_SUCCESS_NOT:
            case EventId.ERM_RESTORE_SPIRIT_NOT:
            case EventId.ERM_USE_QUICK_SLOT_NOT:
            case EventId.ERM_USE_SKILL_BUFF_IN_PLAY_NOT:
            case EventId.ERM_UPDATE_BUFF_INFO_IN_ROOM_NOT:
            case EventId.ERM_UPDATE_USER_UNIT_BUFF_INFO_NOT:
            case EventId.ERM_UPDATE_NPC_UNIT_BUFF_INFO_NOT:
            case EventId.ERM_SET_QUEST_ITEM_INFO_NOT:
            case EventId.ERM_TUTORIAL_DUNGEON_ROOM_UPDATE_NOT:
            case EventId.ERM_UPDATE_TUTORIAL_INFO_NOT:
            case EventId.ERM_FORCE_RELAY_NOT:
            case EventId.ERM_FIELD_UNIT_SYNC_DATA_NOT:
            case EventId.ERM_FIELD_USER_FRAME_INFO_NOT:
            case EventId.ERM_SYNC_BATTLE_FIELD_DAMAGED_MAP_NOT:
            case EventId.ERM_GET_ITEM_COMPLETE_NOT:
            case EventId.ERM_DEFENCE_DUNGEON_WAVE_ID_NOT:
            case EventId.ERM_REMOVE_LANBUG_CHECK_USER_NOT:
            case EventId.ERM_REMOVE_LANBUG_VERIFY_USER_NOT:
            case EventId.ERM_UDP_CHECK_KICK_USER_NOT:
            {
                var room = RoomManager.ResolveRoom(ev.DestPerformer.Uids.FirstOrDefault(), ev.FirstSenderUid);
                if (room != null)
                {
                    if (ev.EventId == EventId.ERM_CHANGE_EQUIPPED_ITEM_IN_ROOM_NOT)
                    {
                        var eq = ev.GetPayload<KERM_CHANGE_EQUIPPED_ITEM_IN_ROOM_NOT>(Region);
                        if (eq.EquippedTitleId > 0) RoomManager.SetUserTitle(room, eq.UnitUid, eq.EquippedTitleId);
                    }
                    else if (ev.EventId == EventId.ERM_UPDATE_BUFF_INFO_IN_ROOM_NOT)
                    {
                        var bf = ev.GetPayload<KERM_UPDATE_BUFF_INFO_IN_ROOM_NOT>(Region);
                        RoomManager.SetUserBuffs(room, bf.UnitUid, bf.ActivateBuffList);
                    }
                    foreach (long uid in room.GetMemberUnitUids())
                    {
                        if (ev.InProcessPayload != null)
                            Router.Send(PerformerId.PI_CN_ROOM, room.Info.RoomUid, PerformerId.PI_GS_USER, uid,
                                ev.EventId, ev.InProcessPayload);
                    }
                }
                break;
            }

            case EventId.ERM_VISIT_CASH_SHOP_NOT:
            {
                var req = ev.GetPayload<KEGS_VISIT_CASH_SHOP_NOT>(Region);
                var room = RoomManager.ResolveRoom(ev.DestPerformer.Uids.FirstOrDefault(), req.UnitUid);
                if (room != null)
                {
                    RoomManager.SetUserCashShop(room, req.UnitUid, req.EnterCashShop);
                    foreach (long uid in room.GetMemberUnitUids())
                        Router.Send(PerformerId.PI_CN_ROOM, room.Info.RoomUid, PerformerId.PI_GS_USER, uid,
                            EventId.ERM_VISIT_CASH_SHOP_NOT, req);
                }
                break;
            }

            case EventId.ERM_UDP_PING_SCORE_NOT:
            {
                var req = ev.GetPayload<KEGS_UDP_PING_SCORE_NOT>(Region);
                var room = RoomManager.ResolveRoom(ev.DestPerformer.Uids.FirstOrDefault(), ev.FirstSenderUid);
                room?.UpdatePingScore(ev.FirstSenderUid, false, req.PingScore);
                break;
            }

            // ---- Dungeon & BattleField Stage / Monster / Combat ----
            case EventId.ERM_DUNGEON_STAGE_LOAD_REQ:
            {
                var req = ev.GetPayload<KEGS_DUNGEON_STAGE_LOAD_REQ>(Region);
                var room = RoomManager.ResolveRoom(ev.DestPerformer.Uids.FirstOrDefault(), ev.FirstSenderUid);
                if (room != null)
                {
                    room.BeginStageLoad(req.StageId);
                    if (ev.FirstSenderUid > 0)
                        Router.Send(PerformerId.PI_CN_ROOM, room.Info.RoomUid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                            EventId.ERM_DUNGEON_STAGE_LOAD_ACK, new KPacketOK { Ok = NetError.NET_OK });
                    var not = new KEGS_DUNGEON_STAGE_LOAD_NOT { StageId = req.StageId, NumMember = room.Slots.Count };
                    foreach (long uid in room.GetMemberUnitUids())
                        Router.Send(PerformerId.PI_CN_ROOM, room.Info.RoomUid, PerformerId.PI_GS_USER, uid,
                            EventId.ERM_DUNGEON_STAGE_LOAD_NOT, not);
                }
                break;
            }

            case EventId.ERM_DUNGEON_SUB_STAGE_OPEN_REQ:
            {
                var req = ev.GetPayload<KERM_DUNGEON_SUB_STAGE_OPEN_REQ>(Region);
                var room = RoomManager.ResolveRoom(ev.DestPerformer.Uids.FirstOrDefault(), ev.FirstSenderUid);
                if (room != null)
                {
                    room.BeginSubStageOpen(req.SubStageId);
                    if (ev.FirstSenderUid > 0)
                        Router.Send(PerformerId.PI_CN_ROOM, room.Info.RoomUid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                            EventId.ERM_DUNGEON_SUB_STAGE_OPEN_ACK, new KPacketOK { Ok = NetError.NET_OK });
                    foreach (long uid in room.GetMemberUnitUids())
                        Router.Send(PerformerId.PI_CN_ROOM, room.Info.RoomUid, PerformerId.PI_GS_USER, uid,
                            EventId.ERM_DUNGEON_SUB_STAGE_OPEN_NOT, req);
                }
                break;
            }

            case EventId.ERM_DUNGEON_SUB_STAGE_CLEAR_REQ:
            {
                var req = ev.GetPayload<KEGS_DUNGEON_SUB_STAGE_CLEAR_REQ>(Region);
                var room = RoomManager.ResolveRoom(ev.DestPerformer.Uids.FirstOrDefault(), ev.FirstSenderUid);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_CN_ROOM, room?.Info.RoomUid ?? 0, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ERM_DUNGEON_SUB_STAGE_CLEAR_ACK, new KEGS_DUNGEON_SUB_STAGE_CLEAR_ACK
                        {
                            Ok = room != null ? NetError.NET_OK : NetError.ERR_ROOM_16,
                            ClearType = req.ClearConditionIndex,
                            StageIndex = room?.CurrentStageId ?? 0,
                            SubStageIndex = room?.CurrentSubStageId ?? 0,
                        });
                break;
            }

            case EventId.ERM_DUNGEON_SECRET_STAGE_ENTER_SELECT_NOT:
            {
                var req = ev.GetPayload<KEGS_DUNGEON_SECRET_STAGE_ENTER_SELECT_NOT>(Region);
                var room = RoomManager.ResolveRoom(ev.DestPerformer.Uids.FirstOrDefault(), ev.FirstSenderUid);
                if (room != null)
                {
                    var (allVoted, enter) = room.VoteSecretStage(ev.FirstSenderUid, req.WantToEnterSecretStage);
                    if (allVoted && enter)
                        KLog.Clog($"[{Name}] Room[{room.Info.RoomUid}] entering Secret Stage!");
                }
                break;
            }

            case EventId.ERM_SECRET_STAGE_LOAD_REQ:
            {
                var req = ev.GetPayload<KEGS_SECRET_STAGE_LOAD_REQ>(Region);
                var room = RoomManager.ResolveRoom(ev.DestPerformer.Uids.FirstOrDefault(), ev.FirstSenderUid);
                if (room != null)
                {
                    room.MarkStageLoaded(ev.FirstSenderUid, out _, out _, isSecretStage: true);
                    if (ev.FirstSenderUid > 0)
                        Router.Send(PerformerId.PI_CN_ROOM, room.Info.RoomUid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                            EventId.ERM_SECRET_STAGE_LOAD_ACK, new KPacketOK { Ok = NetError.NET_OK });
                }
                break;
            }

            case EventId.ERM_ENTER_DEFENCE_DUNGEON_POPUP_REPLY_NOT:
            {
                var req = ev.GetPayload<KEGS_ENTER_DEFENCE_DUNGEON_POPUP_REPLY_NOT>(Region);
                var room = RoomManager.ResolveRoom(ev.DestPerformer.Uids.FirstOrDefault(), ev.FirstSenderUid);
                room?.VoteEnterDefenceDungeon(ev.FirstSenderUid, req.Agree);
                break;
            }

            case EventId.ERM_PREPARE_FOR_DEFENCE_DUNGEON_ACK:
            {
                var req = ev.GetPayload<KERM_PREPARE_FOR_DEFENCE_DUNGEON_ACK>(Region);
                var room = RoomManager.ResolveRoom(ev.DestPerformer.Uids.FirstOrDefault(), req.RoomUserInfo.UnitUid);
                if (room != null) RoomManager.UpdateRoomUserInfo(room, req.RoomUserInfo);
                break;
            }

            case EventId.ERM_MY_USER_UNIT_INFO_TO_SERVER_REQ:
            {
                var req = ev.GetPayload<KERM_MY_USER_UNIT_INFO_TO_SERVER_REQ>(Region);
                var room = RoomManager.ResolveRoom(ev.DestPerformer.Uids.FirstOrDefault(), req.RoomUserInfo.UnitUid);
                if (room != null)
                {
                    RoomManager.UpdateRoomUserInfo(room, req.RoomUserInfo);
                    room.SubmitDungeonPlayResult(req.RoomUserInfo.UnitUid, req.DungeonUnitInfo);
                }
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_CN_ROOM, room?.Info.RoomUid ?? 0, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ERM_MY_USER_UNIT_INFO_TO_SERVER_ACK, new KPacketOK { Ok = room != null ? NetError.NET_OK : NetError.ERR_ROOM_16 });
                break;
            }

            case EventId.ERM_NPC_UNIT_CREATE_REQ:
            {
                var req = ev.GetPayload<KEGS_NPC_UNIT_CREATE_REQ>(Region);
                var room = RoomManager.ResolveRoom(ev.DestPerformer.Uids.FirstOrDefault(), ev.FirstSenderUid);
                if (room != null)
                {
                    foreach (var n in req.NpcUnitReqs)
                        room.MonsterManager.CreateMonster(n.NpcId, n.Level, (KRoomMonsterManager.MonsterGrade)n.MonsterGrade, n.NoDrop, n.GroupId, n.Uid);
                    if (room.CurrentStageId > 0)
                        room.DungeonMonsterManager.InsertSubStageNpcList(room.CurrentStageId, room.CurrentSubStageId, req.NpcUnitReqs);
                    if (ev.FirstSenderUid > 0)
                        Router.Send(PerformerId.PI_CN_ROOM, room.Info.RoomUid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                            EventId.ERM_NPC_UNIT_CREATE_ACK, new KPacketOK { Ok = NetError.NET_OK });
                    var not = new KEGS_NPC_UNIT_CREATE_NOT { BattleFieldId = room.Info.BattleFieldId };
                    foreach (long uid in room.GetMemberUnitUids())
                        Router.Send(PerformerId.PI_CN_ROOM, room.Info.RoomUid, PerformerId.PI_GS_USER, uid,
                            EventId.ERM_NPC_UNIT_CREATE_NOT, not);
                }
                break;
            }

            case EventId.ERM_NPC_UNIT_DIE_REQ:
            {
                var req = ev.GetPayload<KEGS_NPC_UNIT_DIE_REQ>(Region);
                var room = RoomManager.ResolveRoom(ev.DestPerformer.Uids.FirstOrDefault(), ev.FirstSenderUid);
                if (room != null)
                {
                    if (room.MonsterManager.TryKillMonster(req.DieNpcUid, out var killed) && killed != null)
                    {
                        room.DungeonMonsterManager.IncreaseMonsterDieCount((KDungeonMonsterManager.MonsterTypeFactor)(sbyte)killed.Grade);
                        if (room.Info.RoomType == 3)
                        {
                            room.BattleFieldMonsterManager.RecordNpcDie(req.DieNpcUid, (sbyte)killed.Grade, killed.NpcId);
                            room.BattleFieldGameManager.OnNpcUnitDie(
                                room.GetMemberUnitUids().Count(),
                                killed.IsAttribNpc,
                                room.Info.DifficultyLevel,
                                (sbyte)killed.Grade);
                        }

                        if (!killed.NoDrop)
                        {
                            if (room.Info.RoomType == 3)
                            {
                                bool anyDropped = false;
                                var bfDrop = DropTable.RollBattleFieldDrop(room.Info.BattleFieldId, killed.NpcId);
                                if (bfDrop.dropped)
                                {
                                    room.RegisterDropItem(bfDrop.itemId, bfDrop.quantity);
                                    anyDropped = true;
                                }
                                if (KBattleFieldGameManager.MiddleBossDropInfos.TryGetValue((room.Info.BattleFieldId, killed.NpcId), out var mbDrop))
                                {
                                    if (mbDrop.CommonBonusItemId > 0) { room.RegisterDropItem(mbDrop.CommonBonusItemId, Math.Max(1, mbDrop.CommonBonusItemCount)); anyDropped = true; }
                                    if (mbDrop.BonusItemId > 0) { room.RegisterDropItem(mbDrop.BonusItemId, Math.Max(1, mbDrop.BonusItemCountHigh)); anyDropped = true; }
                                }
                                if (KBattleFieldGameManager.GetBattleFieldEventBossDropInfo(room.Info.BattleFieldId, killed.NpcId, out var ebDrop) && ebDrop != null)
                                {
                                    if (ebDrop.CommonBonusItemId > 0) { room.RegisterDropItem(ebDrop.CommonBonusItemId, Math.Max(1, ebDrop.CommonBonusItemCount)); anyDropped = true; }
                                    if (ebDrop.BonusItemId > 0) { room.RegisterDropItem(ebDrop.BonusItemId, Math.Max(1, ebDrop.BonusItemCountHigh)); anyDropped = true; }
                                }
                                if (room.BattleFieldGameManager.CheckAndDeleteReservedDangerousEvent(KBattleFieldGameManager.DangerousEvent.DeEliteMonsterDrop))
                                {
                                    var eliteDrop = DropTable.RollBattleFieldDrop(room.Info.BattleFieldId, killed.NpcId, rateMultiplier: 2.0f);
                                    if (eliteDrop.dropped) { room.RegisterDropItem(eliteDrop.itemId, eliteDrop.quantity); anyDropped = true; }
                                }
                                if (!anyDropped)
                                    room.RegisterDropItem(127000, 1, ed: 150);
                            }
                            else
                            {
                                bool isHenirDrop = room.Info.DungeonMode is 1 or 2;
                                var mDrop = DropTable.RollMonsterDrop(killed.NpcId, isHenir: isHenirDrop, dungeonId: room.Info.DungeonId, henirMode: room.Info.DungeonMode);
                                if (mDrop.dropped) room.RegisterDropItem(mDrop.itemId, mDrop.quantity);
                                var sDrop = DropTable.RollStaticDrop(room.Info.DungeonId);
                                if (sDrop.dropped) room.RegisterDropItem(sDrop.itemId, sDrop.quantity);
                                bool extraDropped = false;
                                if (room.IsSecretStage)
                                {
                                    var exDrop = DropTable.RollExtraStageDrop(killed.NpcId);
                                    if (exDrop.dropped) { room.RegisterDropItem(exDrop.itemId, exDrop.quantity); extraDropped = true; }
                                }
                                if (!mDrop.dropped && !sDrop.dropped && !extraDropped)
                                    room.RegisterDropItem(127000, 1, ed: 150);
                            }
                        }
                    }
                    if (ev.FirstSenderUid > 0)
                        Router.Send(PerformerId.PI_CN_ROOM, room.Info.RoomUid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                            EventId.ERM_NPC_UNIT_DIE_ACK, new KERM_NPC_UNIT_DIE_ACK
                            {
                                Ok = NetError.NET_OK,
                                NpcId = killed?.NpcId ?? 0,
                            });
                    var not = new KEGS_NPC_UNIT_DIE_NOT
                    {
                        DieNpcUid = req.DieNpcUid,
                        NpcId = killed?.NpcId ?? 0,
                        AttUnitType = req.AttUnitType,
                        AttUnit = req.AttUnit,
                    };
                    foreach (long uid in room.GetMemberUnitUids())
                        Router.Send(PerformerId.PI_CN_ROOM, room.Info.RoomUid, PerformerId.PI_GS_USER, uid,
                            EventId.ERM_NPC_UNIT_DIE_NOT, not);
                }
                break;
            }

            case EventId.ERM_USER_UNIT_DIE_REQ:
            {
                var req = ev.GetPayload<KEGS_USER_UNIT_DIE_REQ>(Region);
                var room = RoomManager.ResolveRoom(ev.DestPerformer.Uids.FirstOrDefault(), ev.FirstSenderUid);
                if (room != null)
                {
                    if (req.KillerUserUnitUid > 0) room.RecordPvpKill(req.KillerUserUnitUid, ev.FirstSenderUid);
                    if (ev.FirstSenderUid > 0)
                        Router.Send(PerformerId.PI_CN_ROOM, room.Info.RoomUid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                            EventId.ERM_USER_UNIT_DIE_ACK, new KEGS_USER_UNIT_DIE_ACK { Ok = NetError.NET_OK, ReBirthTime = 3.0f });
                    var not = new KEGS_USER_UNIT_DIE_NOT
                    {
                        KillerNpcUid = req.KillerNpcUid,
                        KillerUserUnitUid = req.KillerUserUnitUid,
                        MaxDamageKillerUserUnitUid = req.MaxDamageKillerUserUnitUid,
                        KilledUserUnitUid = ev.FirstSenderUid,
                    };
                    foreach (long uid in room.GetMemberUnitUids())
                        Router.Send(PerformerId.PI_CN_ROOM, room.Info.RoomUid, PerformerId.PI_GS_USER, uid,
                            EventId.ERM_USER_UNIT_DIE_NOT, not);
                }
                break;
            }

            case EventId.ERM_CHECK_EXIST_MONSTER_UID_REQ:
            {
                var req = ev.GetPayload<KEGS_CHECK_EXIST_MONSTER_UID_REQ>(Region);
                var room = RoomManager.ResolveRoom(ev.DestPerformer.Uids.FirstOrDefault(), ev.FirstSenderUid);
                var missing = room?.GetNotExistNpcUids(req.NpcUid) ?? new List<int>();
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_CN_ROOM, room?.Info.RoomUid ?? 0, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ERM_CHECK_EXIST_MONSTER_UID_ACK, new KEGS_CHECK_EXIST_MONSTER_UID_ACK
                        {
                            Ok = NetError.NET_OK,
                            NotExistNpcUid = missing,
                        });
                break;
            }

            case EventId.ERM_FORCED_EXIT_VOTE_REQ:
            {
                var req = ev.GetPayload<KEGS_FORCED_EXIT_VOTE_REQ>(Region);
                var room = RoomManager.ResolveRoom(ev.DestPerformer.Uids.FirstOrDefault(), ev.FirstSenderUid);
                bool ok = room != null && room.RecordForcedExitVote(ev.FirstSenderUid, req.UserUid, req.WantForceExit);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_CN_ROOM, room?.Info.RoomUid ?? 0, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ERM_FORCED_EXIT_VOTE_ACK, new KEGS_FORCED_EXIT_VOTE_ACK
                        {
                            Ok = ok ? NetError.NET_OK : NetError.ERR_ROOM_16,
                            UserUid = req.UserUid,
                        });
                break;
            }

            case EventId.ERM_RESURRECT_TO_CONTINUE_DUNGEON_REQ:
            {
                var req = ev.GetPayload<KEGS_RESURRECT_TO_CONTINUE_DUNGEON_REQ>(Region);
                var room = RoomManager.ResolveRoom(ev.DestPerformer.Uids.FirstOrDefault(), ev.FirstSenderUid);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_CN_ROOM, room?.Info.RoomUid ?? 0, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ERM_RESURRECT_TO_CONTINUE_DUNGEON_ACK, new KEGS_RESURRECT_TO_CONTINUE_DUNGEON_ACK
                        {
                            Ok = room != null ? NetError.NET_OK : NetError.ERR_ROOM_16,
                            DungeonId = room?.Info.DungeonId ?? 0,
                            UseCash = req.UseCash,
                        });
                if (room != null)
                {
                    var not = new KEGS_RESURRECT_TO_CONTINUE_DUNGEON_NOT
                    {
                        UsedByUnitUid = req.UsedByUnitUid,
                        UnitUid = req.UnitUid,
                    };
                    foreach (long uid in room.GetMemberUnitUids())
                        Router.Send(PerformerId.PI_CN_ROOM, room.Info.RoomUid, PerformerId.PI_GS_USER, uid,
                            EventId.ERM_RESURRECT_TO_CONTINUE_DUNGEON_NOT, not);
                }
                break;
            }

            case EventId.ERM_STOP_DUNGEON_CONTINUE_TIME_REQ:
            {
                var req = ev.GetPayload<KEGS_STOP_DUNGEON_CONTINUE_TIME_REQ>(Region);
                var room = RoomManager.ResolveRoom(ev.DestPerformer.Uids.FirstOrDefault(), ev.FirstSenderUid);
                int ok = room?.SetStopContinueTime(ev.FirstSenderUid, req.IsStop) ?? NetError.ERR_ROOM_16;
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_CN_ROOM, room?.Info.RoomUid ?? 0, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ERM_STOP_DUNGEON_CONTINUE_TIME_ACK, new KEGS_STOP_DUNGEON_CONTINUE_TIME_ACK
                        {
                            Ok = ok,
                            IsStop = req.IsStop,
                        });
                break;
            }

            case EventId.ERM_STATE_CHANGE_RESULT_REQ:
            {
                var room = RoomManager.ResolveRoom(ev.DestPerformer.Uids.FirstOrDefault(), ev.FirstSenderUid);
                int ok = room?.StateChangeResult(ev.FirstSenderUid) ?? NetError.ERR_ROOM_16;
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_CN_ROOM, room?.Info.RoomUid ?? 0, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ERM_STATE_CHANGE_RESULT_ACK, new KPacketOK { Ok = ok });
                if (ok == NetError.NET_OK && room != null)
                {
                    var resultNot = new KEGS_STATE_CHANGE_RESULT_NOT
                    {
                        RoomState = room.Info.RoomState,
                        Slot = [.. room.Slots],
                    };
                    foreach (long uid in room.GetMemberUnitUids())
                        Router.Send(PerformerId.PI_CN_ROOM, room.Info.RoomUid, PerformerId.PI_GS_USER, uid,
                            EventId.ERM_STATE_CHANGE_RESULT_NOT, resultNot);
                }
                break;
            }

            case EventId.ERM_CHECK_SELL_ED_ITEM_REQ:
            {
                var room = RoomManager.ResolveRoom(ev.DestPerformer.Uids.FirstOrDefault(), ev.FirstSenderUid);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_CN_ROOM, room?.Info.RoomUid ?? 0, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ERM_CHECK_SELL_ED_ITEM_ACK, new KERM_CHECK_SELL_ED_ITEM_ACK { Ok = room != null ? NetError.NET_OK : NetError.ERR_ROOM_16 });
                break;
            }

            // ---- PvP & BattleField & TC ----
            case EventId.ERM_PVP_NPC_UNIT_INFO_TO_SERVER_REQ:
            {
                var room = RoomManager.ResolveRoom(ev.DestPerformer.Uids.FirstOrDefault(), ev.FirstSenderUid);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_CN_ROOM, room?.Info.RoomUid ?? 0, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ERM_PVP_NPC_UNIT_INFO_TO_SERVER_ACK, new KPacketOK { Ok = room != null ? NetError.NET_OK : NetError.ERR_ROOM_16 });
                break;
            }

            case EventId.ERM_PVP_NPC_UNIT_RE_BIRTH_POS_REQ:
            {
                var req = ev.GetPayload<KEGS_PVP_NPC_UNIT_RE_BIRTH_POS_REQ>(Region);
                var room = RoomManager.ResolveRoom(ev.DestPerformer.Uids.FirstOrDefault(), ev.FirstSenderUid);
                bool ok = room != null && room.UpdatePvpNpcRebirthPos(req.PvpNpcUnitUid, req.StartPosIndex);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_CN_ROOM, room?.Info.RoomUid ?? 0, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ERM_PVP_NPC_UNIT_RE_BIRTH_POS_ACK, new KPacketOK { Ok = ok ? NetError.NET_OK : NetError.ERR_ROOM_16 });
                break;
            }

            case EventId.ERM_USER_UNIT_RE_BIRTH_POS_REQ:
            {
                var req = ev.GetPayload<KEGS_USER_UNIT_RE_BIRTH_POS_REQ>(Region);
                var room = RoomManager.ResolveRoom(ev.DestPerformer.Uids.FirstOrDefault(), ev.FirstSenderUid);
                int ok = room?.SetUnitRebirthPos(ev.FirstSenderUid, req.StartPosIndex) ?? NetError.ERR_ROOM_16;
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_CN_ROOM, room?.Info.RoomUid ?? 0, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ERM_USER_UNIT_RE_BIRTH_POS_ACK, new KPacketOK { Ok = ok });
                break;
            }

            case EventId.ERM_PVP_REMATCH_REQ:
            {
                var req = ev.GetPayload<KEGS_PVP_REMATCH_REQ>(Region);
                var room = RoomManager.ResolveRoom(ev.DestPerformer.Uids.FirstOrDefault(), ev.FirstSenderUid);
                var (ok, allAccepted) = room?.VoteRematch(ev.FirstSenderUid, req.AcceptRematch) ?? (NetError.ERR_ROOM_16, false);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_CN_ROOM, room?.Info.RoomUid ?? 0, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ERM_PVP_REMATCH_ACK, new KEGS_PVP_REMATCH_ACK { Ok = ok });
                if (ok == NetError.NET_OK && room != null)
                {
                    var not = new KEGS_PVP_REMATCH_NOT { UnitUid = ev.FirstSenderUid, AcceptRematch = req.AcceptRematch };
                    foreach (long uid in room.GetMemberUnitUids())
                        Router.Send(PerformerId.PI_CN_ROOM, room.Info.RoomUid, PerformerId.PI_GS_USER, uid,
                            EventId.ERM_PVP_REMATCH_NOT, not);
                    if (allAccepted) room.StartGame();
                }
                break;
            }

            case EventId.ERM_UPDATE_PVP_PLAY_UNIT_INFO_NOT:
            {
                var req = ev.GetPayload<KERM_UPDATE_PVP_PLAY_UNIT_INFO_NOT>(Region);
                var room = RoomManager.ResolveRoom(ev.DestPerformer.Uids.FirstOrDefault(), req.UnitUid);
                if (room != null && req.KillNum > 0) room.RecordPvpKill(req.UnitUid, 0);
                break;
            }

            case EventId.ERM_BATTLE_FIELD_GAME_LOADING_REQ:
            {
                var req = ev.GetPayload<KEGS_GAME_LOADING_REQ>(Region);
                var room = RoomManager.ResolveRoom(ev.DestPerformer.Uids.FirstOrDefault(), ev.FirstSenderUid);
                bool ok = room != null && room.UpdateLoading(ev.FirstSenderUid, req.LoadingProgress, out _);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_CN_ROOM, room?.Info.RoomUid ?? 0, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ERM_BATTLE_FIELD_GAME_LOADING_ACK, new KPacketOK { Ok = ok ? NetError.NET_OK : NetError.ERR_ROOM_16 });
                break;
            }

            case EventId.ERM_BATTLE_FIELD_JOIN_USER_LOAD_COMPLETE_NOT:
            {
                var req = ev.GetPayload<KERM_BATTLE_FIELD_JOIN_USER_LOAD_COMPLETE_NOT>(Region);
                var room = RoomManager.ResolveRoom(ev.DestPerformer.Uids.FirstOrDefault(), req.LoadedUnitUid);
                if (room != null)
                {
                    room.MarkStageLoaded(req.LoadedUnitUid, out _, out _);
                    foreach (long uid in room.GetMemberUnitUids())
                        Router.Send(PerformerId.PI_CN_ROOM, room.Info.RoomUid, PerformerId.PI_GS_USER, uid,
                            EventId.ERM_BATTLE_FIELD_JOIN_USER_LOAD_COMPLETE_NOT, req);
                }
                break;
            }

            case EventId.ERM_BATTLE_FIELD_NPC_DAMAGED_MAP_REQ:
            {
                var req = ev.GetPayload<KEGS_BATTLE_FIELD_NPC_DAMAGED_MAP_REQ>(Region);
                var room = RoomManager.ResolveRoom(ev.DestPerformer.Uids.FirstOrDefault(), req.UnitUid);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_CN_ROOM, room?.Info.RoomUid ?? 0, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ERM_BATTLE_FIELD_NPC_DAMAGED_MAP_ACK, new KEGS_BATTLE_FIELD_NPC_DAMAGED_MAP_ACK
                        {
                            UnitUid = req.UnitUid,
                            NpcUid = req.NpcUid,
                        });
                break;
            }

            case EventId.ERM_BOSS_FIELD_LOG_NOT:
            {
                var req = ev.GetPayload<KERM_BOSS_FIELD_LOG_NOT>(Region);
                var room = RoomManager.ResolveRoom(ev.DestPerformer.Uids.FirstOrDefault(), ev.FirstSenderUid);
                room?.RecordBossFieldLog(req.GivenDamage, req.AttackDamage);
                break;
            }

            case EventId.ERM_COMMUNITY_IN_BATTLE_FIELD_REQ:
            {
                var req = ev.GetPayload<KEGS_COMMUNITY_USER_LIST_REQ>(Region);
                var room = RoomManager.ResolveRoom(ev.DestPerformer.Uids.FirstOrDefault(), ev.FirstSenderUid);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_CN_ROOM, room?.Info.RoomUid ?? 0, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ERM_COMMUNITY_IN_BATTLE_FIELD_ACK, new KEGS_COMMUNITY_USER_LIST_ACK
                        {
                            Ok = room != null ? NetError.NET_OK : NetError.ERR_ROOM_16,
                            TotalPage = 1,
                            ViewPage = (uint)Math.Max(1, req.ViewPage),
                        });
                break;
            }

            case EventId.ERM_CALL_MY_LOVER_CHECK_ROOM_REQ:
            {
                var req = ev.GetPayload<KERM_CALL_MY_LOVER_CHECK_ROOM_REQ>(Region);
                var room = RoomManager.GetRoomByUnitUid(req.LoverUnitUid);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_CN_ROOM, room?.Info.RoomUid ?? 0, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ERM_CALL_MY_LOVER_CHECK_ROOM_ACK, new KERM_CALL_MY_LOVER_CHECK_ROOM_ACK
                        {
                            Ok = room != null ? NetError.NET_OK : NetError.ERR_ROOM_16,
                            LoverUnitUid = req.LoverUnitUid,
                            CallType = req.CallType,
                            VPos = req.Pos,
                            LastTouchLineIndex = req.LastTouchLineIndex,
                        });
                break;
            }

            case EventId.ERM_CALL_MY_LOVER_JOIN_BATTLE_FIELD_REQ:
            case EventId.ERM_LEAVE_BATTLE_FIELD_FOR_CALL_MY_LOVER_REQ:
            {
                var req = ev.GetPayload<KERM_CALL_MY_LOVER_JOIN_BATTLE_FIELD_REQ>(Region);
                var room = RoomManager.GetRoom(req.RoomUid);
                int ok = room != null ? room.TryAddUser(req.RoomUserInfo, false, string.Empty).ok : NetError.ERR_ROOM_16;
                EventId ackId = ev.EventId == EventId.ERM_CALL_MY_LOVER_JOIN_BATTLE_FIELD_REQ
                    ? EventId.ERM_CALL_MY_LOVER_JOIN_BATTLE_FIELD_ACK
                    : EventId.ERM_LEAVE_BATTLE_FIELD_FOR_CALL_MY_LOVER_ACK;
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_CN_ROOM, req.RoomUid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        ackId, new KPacketOK { Ok = ok });
                break;
            }

            case EventId.ERM_LEAVE_BATTLE_FIELD_FOR_DUNGEON_GAME_START_REQ:
            case EventId.ERM_LEAVE_BATTLE_FIELD_FOR_JOIN_BATTLE_FIELD_REQ:
            case EventId.ERM_LEAVE_BATTLE_FIELD_FOR_PVP_GAME_START_REQ:
            case EventId.ERM_LEAVE_ROOM_FOR_ENTER_THE_VILLAGE_REQ:
            case EventId.ERM_LEAVE_ROOM_FOR_JOIN_BATTLE_FIELD_REQ:
            case EventId.ERM_LEAVE_ROOM_FOR_WORKINGS_BLOCK_REQ:
            case EventId.ERM_LEAVE_TC_ROOM_REQ:
            {
                var req = ev.GetPayload<KERM_LEAVE_ROOM_REQ>(Region);
                var room = RoomManager.ResolveRoom(req.RoomUid, req.UnitUid);
                if (room != null) RoomManager.LeaveRoom(room.Info.RoomUid, req.UnitUid);
                EventId ackId = ev.EventId switch
                {
                    EventId.ERM_LEAVE_BATTLE_FIELD_FOR_DUNGEON_GAME_START_REQ => EventId.ERM_LEAVE_BATTLE_FIELD_FOR_DUNGEON_GAME_START_ACK,
                    EventId.ERM_LEAVE_BATTLE_FIELD_FOR_JOIN_BATTLE_FIELD_REQ => EventId.ERM_LEAVE_BATTLE_FIELD_FOR_JOIN_BATTLE_FIELD_ACK,
                    EventId.ERM_LEAVE_BATTLE_FIELD_FOR_PVP_GAME_START_REQ => EventId.ERM_LEAVE_BATTLE_FIELD_FOR_PVP_GAME_START_ACK,
                    EventId.ERM_LEAVE_ROOM_FOR_ENTER_THE_VILLAGE_REQ => EventId.ERM_LEAVE_ROOM_FOR_ENTER_THE_VILLAGE_ACK,
                    EventId.ERM_LEAVE_ROOM_FOR_JOIN_BATTLE_FIELD_REQ => EventId.ERM_LEAVE_ROOM_FOR_JOIN_BATTLE_FIELD_ACK,
                    EventId.ERM_LEAVE_ROOM_FOR_WORKINGS_BLOCK_REQ => EventId.ERM_LEAVE_ROOM_FOR_WORKINGS_BLOCK_ACK,
                    _ => EventId.ERM_LEAVE_TC_ROOM_ACK,
                };
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_CN_ROOM, room?.Info.RoomUid ?? 0, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        ackId, new KPacketOK { Ok = NetError.NET_OK });
                break;
            }

            case EventId.ERM_LEAVE_BATTLE_FIELD_FOR_JOIN_WEDDING_HALL_REQ:
            {
                var req = ev.GetPayload<KERM_LEAVE_BATTLE_FIELD_FOR_JOIN_WEDDING_HALL_REQ>(Region);
                var room = RoomManager.GetRoomByUnitUid(ev.FirstSenderUid);
                if (room != null) RoomManager.LeaveRoom(room.Info.RoomUid, ev.FirstSenderUid);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_CN_ROOM, req.WeddingInfo.RoomUid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ERM_LEAVE_BATTLE_FIELD_FOR_JOIN_WEDDING_HALL_ACK, new KPacketOK { Ok = NetError.NET_OK });
                break;
            }

            case EventId.ERM_END_TC_GAME_REQ:
            {
                var req = ev.GetPayload<KEGS_END_TC_GAME_REQ>(Region);
                var tc = TrainingCenterRooms.Values.FirstOrDefault(t => t.OwnerUnitUid == ev.FirstSenderUid);
                tc?.EndGame(req.IsSuccess);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_CN_ROOM, tc?.RoomUid ?? 0, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ERM_END_TC_GAME_ACK, new KEGS_END_TC_GAME_ACK { Ok = NetError.NET_OK, IsSuccess = req.IsSuccess });
                break;
            }

            case EventId.ERM_UPDATE_PARTY_INFO_TO_BATTLE_FIELD_NOT:
            {
                var req = ev.GetPayload<KERM_UPDATE_PARTY_INFO_TO_BATTLE_FIELD_NOT>(Region);
                var room = RoomManager.GetRoomByUnitUid(ev.FirstSenderUid);
                if (room != null)
                {
                    var slot = room.Slots.FirstOrDefault(s => s.RoomUserInfo.UnitUid == ev.FirstSenderUid);
                    if (slot != null)
                        slot.RoomUserInfo.PartyUid = req.PartyUid;
                }
                break;
            }

            case EventId.ERM_KILL_ZOMBIE_USER_IN_BATTLE_FIELD_NOT:
            {
                var req = ev.GetPayload<KERM_KILL_ZOMBIE_USER_IN_BATTLE_FIELD_NOT>(Region);
                if (req.UnitUid > 0)
                {
                    var room = RoomManager.GetRoomByUnitUid(req.UnitUid);
                    if (room != null) RoomManager.LeaveRoom(room.Info.RoomUid, req.UnitUid);
                }
                break;
            }

            case EventId.ERM_DUMP_BATTLE_FIELD_NOT:
            case EventId.ERM_DUMP_ROOM_MONSTER_NOT:
            {
                var room = RoomManager.ResolveRoom(ev.DestPerformer.Uids.FirstOrDefault(), ev.FirstSenderUid);
                if (room != null)
                    KLog.Clog($"[{Name}] {ev.EventId}: room={room.Info.RoomUid}, members={room.Slots.Count(s => s.IsOccupied)}");
                break;
            }

            case EventId.ERM_NEXT_RETURN_TO_FIELD_NOT:
            {
                var room = RoomManager.ResolveRoom(ev.DestPerformer.Uids.FirstOrDefault(), ev.FirstSenderUid);
                if (room != null)
                {
                    var memberUids = room.GetMemberUnitUids();
                    foreach (long uid in memberUids)
                        Router.Send(PerformerId.PI_CN_ROOM, room.Info.RoomUid, PerformerId.PI_GS_USER, uid,
                            EventId.ERM_RESULT_ALL_UNIT_SUCCESS_NOT, new KPacketOK { Ok = NetError.NET_OK });
                }
                break;
            }

            // ---- Square / Wedding / Trade / PersonalShop / Lookup ----
            case EventId.ERM_LEAVE_SQUARE_REQ:
            {
                var sq = Squares.Values.FirstOrDefault(s => s.SnapshotMembers().Any(m => m.UnitUid == ev.FirstSenderUid));
                sq?.LeaveSquare(ev.FirstSenderUid);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_CN_ROOM, sq?.SquareUid ?? 0, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ERM_LEAVE_SQUARE_ACK, new KPacketOK { Ok = NetError.NET_OK });
                break;
            }

            case EventId.ERM_SQUARE_UNIT_SYNC_DATA_REQ:
            {
                var req = ev.GetPayload<KEGS_SQUARE_UNIT_SYNC_DATA_REQ>(Region);
                var sq = Squares.Values.FirstOrDefault(s => s.SnapshotMembers().Any(m => m.UnitUid == ev.FirstSenderUid));
                if (sq != null)
                {
                    sq.SyncPosition(ev.FirstSenderUid, req.Pos.X, req.Pos.Y, req.Pos.Z);
                    foreach (var m in sq.SnapshotMembers())
                        Router.Send(PerformerId.PI_CN_ROOM, sq.SquareUid, PerformerId.PI_GS_USER, m.UnitUid,
                            EventId.ERM_SQUARE_UNIT_SYNC_DATA_NOT, req);
                }
                break;
            }

            case EventId.ERM_CHANGE_SQUARE_USER_INFO_NOT:
            case EventId.ERM_CHANGE_EQUIPPED_ITEM_IN_SQUARE_NOT:
            case EventId.ERM_SQUARE_UNIT_REFRESH_ED_NOT:
            case EventId.ERM_SQUARE_LIST_INFO_NOT:
            case EventId.ERM_UPDATE_SQUARE_PERSONAL_SHOP_INFO_NOT:
            {
                var sq = Squares.Values.FirstOrDefault(s => s.SnapshotMembers().Any(m => m.UnitUid == ev.FirstSenderUid));
                if (sq != null && ev.InProcessPayload != null)
                {
                    foreach (var m in sq.SnapshotMembers())
                        Router.Send(PerformerId.PI_CN_ROOM, sq.SquareUid, PerformerId.PI_GS_USER, m.UnitUid,
                            ev.EventId, ev.InProcessPayload);
                }
                break;
            }

            case EventId.ERM_LEAVE_WEDDING_HALL_REQ:
            {
                var hall = WeddingHalls.Values.FirstOrDefault(h => h.SnapshotSlots().Any(s => s.RoomUserInfo.UnitUid == ev.FirstSenderUid));
                hall?.LeaveHall(ev.FirstSenderUid);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_CN_ROOM, hall?.RoomUid ?? 0, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ERM_LEAVE_WEDDING_HALL_ACK, new KPacketOK { Ok = NetError.NET_OK });
                break;
            }

            case EventId.ERM_START_WEDDING_ASK_NOT:
            {
                var req = ev.GetPayload<KEGS_START_WEDDING_ASK_ACK>(Region);
                var hall = WeddingHalls.Values.FirstOrDefault(h => h.IsGroomOrBride(ev.FirstSenderUid));
                hall?.RespondStartWedding(ev.FirstSenderUid, req.Agree);
                break;
            }

            case EventId.ERM_REQUEST_TRADE_NOT:
            {
                var req = ev.GetPayload<KERM_REQUEST_TRADE_NOT>(Region);
                Router.Send(PerformerId.PI_CN_ROOM, 0, PerformerId.PI_GS_USER, req.ReceiverUid,
                    EventId.ERM_REQUEST_TRADE_NOT, req);
                break;
            }

            case EventId.ERM_REQUEST_TRADE_REPLY_NOT:
            {
                var req = ev.GetPayload<KERM_REQUEST_TRADE_REPLY_NOT>(Region);
                Router.Send(PerformerId.PI_CN_ROOM, 0, PerformerId.PI_GS_USER, req.MemberUid,
                    EventId.ERM_REQUEST_TRADE_REPLY_NOT, req);
                break;
            }

            case EventId.ERM_CONFIRM_TRADE_ACK:
            {
                var ack = ev.GetPayload<KERM_CONFIRM_TRADE_ACK>(Region);
                long senderUnitUid = ev.FirstSenderUid;
                long tradeUid = ev.DestPerformer.Uids.FirstOrDefault();
                var tr = tradeUid > 0 && TradeRooms.TryGetValue(tradeUid, out var byUid)
                    ? byUid
                    : TradeRooms.Values.FirstOrDefault(r => r.HasParticipant(senderUnitUid));
                if (tr != null)
                {
                    if (ack.Ok != NetError.NET_OK)
                    {
                        tr.BreakTrade(ack.Ok);
                        foreach (long uid in tr.GetParticipantUnitUids())
                            Router.Send(PerformerId.PI_CN_ROOM, tr.TradeUid, PerformerId.PI_GS_USER, uid,
                                EventId.ERM_BREAK_TRADE_NOT, new KPacketOK { Ok = ack.Ok });
                    }
                    else
                    {
                        var (ok, bothConfirmed) = tr.ConfirmTrade(senderUnitUid);
                        if (ok == NetError.NET_OK && bothConfirmed)
                        {
                            foreach (long uid in tr.GetParticipantUnitUids())
                                Router.Send(PerformerId.PI_CN_ROOM, tr.TradeUid, PerformerId.PI_GS_USER, uid,
                                    EventId.ERM_DB_UPDATE_TRADE_COMPLETE_NOT, new KPacketOK { Ok = NetError.NET_OK });
                        }
                    }
                }
                break;
            }

            case EventId.ERM_DB_UPDATE_TRADE_RESULT_NOT:
            {
                var not = ev.GetPayload<KERM_DB_UPDATE_TRADE_RESULT_NOT>(Region);
                long unitUid = not.UnitUid > 0 ? not.UnitUid : ev.FirstSenderUid;
                long tradeUid = ev.DestPerformer.Uids.FirstOrDefault();
                var tr = tradeUid > 0 && TradeRooms.TryGetValue(tradeUid, out var byUid)
                    ? byUid
                    : TradeRooms.Values.FirstOrDefault(r => r.HasParticipant(unitUid));
                if (tr != null)
                {
                    tr.MarkDbUpdateReady(unitUid, out bool allReady);
                    if (not.UnitUid > 0)
                        Router.Send(PerformerId.PI_CN_ROOM, tr.TradeUid, PerformerId.PI_GS_USER, not.UnitUid,
                            EventId.ERM_DB_UPDATE_TRADE_RESULT_NOT, not);
                    if (allReady)
                    {
                        foreach (long uid in tr.GetParticipantUnitUids())
                            Router.Send(PerformerId.PI_CN_ROOM, tr.TradeUid, PerformerId.PI_GS_USER, uid,
                                EventId.ERM_DB_UPDATE_TRADE_COMPLETE_NOT, new KPacketOK { Ok = NetError.NET_OK });
                    }
                }
                break;
            }

            case EventId.ERM_REG_PERSONAL_SHOP_ITEM_REQ:
            {
                var req = ev.GetPayload<KERM_REG_PERSONAL_SHOP_ITEM_REQ>(Region);
                var shop = PersonalShopRooms.Values.FirstOrDefault(s => s.ShopUid == req.ShopUid || s.HostUnitUid == ev.FirstSenderUid);
                int ok = shop?.RegisterItems(ev.FirstSenderUid, req.SellItems.Select(i => new KCnPersonalShopRoom.ShopListing
                {
                    ItemUid = i.ItemUid,
                    ItemId = i.ItemId,
                    Quantity = i.Quantity,
                    PricePerOne = i.PricePerUnit,
                })) ?? NetError.ERR_PERSONAL_SHOP_09;
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_CN_ROOM, shop?.ShopUid ?? 0, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ERM_REG_PERSONAL_SHOP_ITEM_ACK, new KERM_REG_PERSONAL_SHOP_ITEM_ACK
                        {
                            Ok = ok,
                            UnitUid = ev.FirstSenderUid,
                            PersonalShopName = req.PersonalShopName,
                        });
                break;
            }

            case EventId.ERM_CHECK_JOIN_PERSONAL_SHOP_REQ:
            {
                var req = ev.GetPayload<KEGS_JOIN_PERSONAL_SHOP_REQ>(Region);
                var shop = PersonalShopRooms.Values.FirstOrDefault(s => s.HostUnitUid == req.UnitUid);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_CN_ROOM, shop?.ShopUid ?? 0, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ERM_CHECK_JOIN_PERSONAL_SHOP_ACK, new KERM_CHECK_JOIN_PERSONAL_SHOP_ACK
                        {
                            Ok = shop != null ? NetError.NET_OK : NetError.ERR_PERSONAL_SHOP_09,
                            PersonalShopUid = shop?.ShopUid ?? 0,
                        });
                break;
            }

            case EventId.ERM_BREAK_PERSONAL_SHOP_REQ:
            {
                var req = ev.GetPayload<KERM_BREAK_PERSONAL_SHOP_REQ>(Region);
                var shop = PersonalShopRooms.Values.FirstOrDefault(s => s.HostUnitUid == ev.FirstSenderUid);
                shop?.BreakShop(req.Reason);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_CN_ROOM, shop?.ShopUid ?? 0, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ERM_BREAK_PERSONAL_SHOP_ACK, new KPacketOK { Ok = shop != null ? NetError.NET_OK : NetError.ERR_PERSONAL_SHOP_09 });
                break;
            }

            case EventId.ERM_CONFIRM_BUY_PERSONAL_SHOP_ITEM_ACK:
            {
                var ack = ev.GetPayload<KERM_CONFIRM_BUY_PERSONAL_SHOP_ITEM_ACK>(Region);
                long shopUid = ev.DestPerformer.GetFirstUID();
                PersonalShopRooms.TryGetValue(shopUid, out var shop);
                shop ??= PersonalShopRooms.Values.FirstOrDefault(s => s.HostUnitUid == ev.FirstSenderUid);
                if (shop != null && ack.Ok == NetError.NET_OK)
                {
                    var (ok, totalCost, netProceeds, fee, soldOut) =
                        shop.BuyItem(ack.BuyUnitUid, ack.BuyItemUid, ack.BuyQuantity, viaTradeBoard: false);
                    if (ack.BuyUnitUid > 0)
                        Router.Send(PerformerId.PI_CN_ROOM, shop.ShopUid, PerformerId.PI_GS_USER, ack.BuyUnitUid,
                            EventId.ERM_BUY_PERSONAL_SHOP_ITEM_ACK, new KERM_BUY_PERSONAL_SHOP_ITEM_ACK
                            {
                                Ok = ok,
                                TotalCost = totalCost,
                                NetSellerProceeds = netProceeds,
                                CommissionFee = fee,
                                ShopSoldOut = soldOut,
                            });
                }
                else if (ack.BuyUnitUid > 0)
                {
                    Router.Send(PerformerId.PI_CN_ROOM, shopUid, PerformerId.PI_GS_USER, ack.BuyUnitUid,
                        EventId.ERM_BUY_PERSONAL_SHOP_ITEM_ACK, new KERM_BUY_PERSONAL_SHOP_ITEM_ACK { Ok = ack.Ok });
                }
                break;
            }

            case EventId.ERM_SEARCH_UNIT_REQ:
            {
                var req = ev.GetPayload<KELG_SEARCH_UNIT_REQ>(Region);
                var room = RoomManager.GetRoomByUnitUid(ev.FirstSenderUid);
                var slot = room?.Slots.FirstOrDefault(s =>
                    (req.UnitUid > 0 && s.RoomUserInfo.UnitUid == req.UnitUid) ||
                    (!string.IsNullOrWhiteSpace(req.NickName) && string.Equals(s.RoomUserInfo.NickName, req.NickName, StringComparison.OrdinalIgnoreCase)));
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_CN_ROOM, room?.Info.RoomUid ?? 0, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ERM_SEARCH_UNIT_ACK, new KEGS_SEARCH_UNIT_ACK
                        {
                            Ok = slot != null ? NetError.NET_OK : NetError.ERR_SEARCH_UNIT_03,
                            CUnitInfo = slot != null
                                ? new KConnectionUnitInfo { UnitUid = slot.RoomUserInfo.UnitUid, CharName = slot.RoomUserInfo.NickName }
                                : new KConnectionUnitInfo(),
                        });
                break;
            }

            case EventId.ERM_LOCAL_RANKING_WATCH_UNIT_REQ:
            {
                var req = ev.GetPayload<KELG_LOCAL_RANKING_WATCH_UNIT_REQ>(Region);
                var room = RoomManager.GetRoomByUnitUid(req.TargetUnitUid);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_CN_ROOM, room?.Info.RoomUid ?? 0, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ERM_LOCAL_RANKING_WATCH_UNIT_ACK, new KELG_LOCAL_RANKING_WATCH_UNIT_ACK
                        {
                            Ok = room != null ? NetError.NET_OK : NetError.ERR_SEARCH_UNIT_03,
                        });
                break;
            }

            case EventId.ERM_LOCAL_RANKING_WATCH_UNIT_ACK:
                if (ev.FirstSenderUid > 0 && ev.InProcessPayload != null)
                    Router.Send(PerformerId.PI_CN_ROOM, ev.DestPerformer.GetFirstUID(), PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ERM_LOCAL_RANKING_WATCH_UNIT_ACK, ev.InProcessPayload);
                break;

            case EventId.ERM_GET_UNIT_INFO_ACK:
                if (ev.FirstSenderUid > 0 && ev.InProcessPayload != null)
                    Router.Send(PerformerId.PI_CN_ROOM, ev.DestPerformer.GetFirstUID(), PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ERM_GET_UNIT_INFO_ACK, ev.InProcessPayload);
                break;

            // ---- Additional C++ CenterServer EPM_* / ERM_* Parity Handlers ----
            case EventId.EPM_END_GAME_REGROUP_PARTY_NOT:
            {
                var party = PartyManager.GetPartyByUnitUid(ev.FirstSenderUid);
                if (party != null)
                {
                    party.ReturnToWait();
                    var regroup = new KEPM_REQUEST_REGROUP_PARTY_NOT
                    {
                        CreatePartyInfo = new KCreatePartyInfo
                        {
                            PartyType = party.Info.PartyType,
                            DungeonId = party.Info.DungeonId,
                            DifficultyLevel = party.Info.DifficultyLevel,
                            DungeonMode = party.Info.DungeonMode,
                            GetItemType = party.Info.GetItemType,
                            CheckLowLevel = party.Info.CheckLowLevel,
                            PvpGameType = party.Info.PvpGameType,
                        },
                        PartyFever = (short)party.FeverPoint,
                        Members = party.SnapshotMembers(),
                    };
                    foreach (long uid in party.GetUnitUids())
                        Router.Send(PerformerId.PI_CN_PARTY, party.Info.PartyUid, PerformerId.PI_GS_USER, uid,
                            EventId.EPM_REQUEST_REGROUP_PARTY_NOT, regroup);
                }
                break;
            }

            case EventId.ERM_END_DUNGEON_GAME_PARTY_FEVER_MODIFY_NOT:
            {
                var party = PartyManager.GetPartyByUnitUid(ev.FirstSenderUid);
                if (party != null && party.CapFeverAt100Percent())
                {
                    foreach (long uid in party.GetUnitUids())
                        Router.Send(PerformerId.PI_CN_PARTY, party.Info.PartyUid, PerformerId.PI_GS_USER, uid,
                            EventId.EPM_PARTY_FEVER_POINT_NOT, new KEGS_PARTY_FEVER_POINT_NOT { PartyFever = (short)party.FeverPoint });
                }
                break;
            }

            case EventId.ERM_USER_UNIT_DIE_COMPLETE_REQ:
            {
                var room = RoomManager.GetRoomByUnitUid(ev.FirstSenderUid);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_CN_ROOM, room?.Info.RoomUid ?? 0, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ERM_USER_UNIT_DIE_COMPLETE_ACK, new KPacketOK { Ok = room != null ? NetError.NET_OK : NetError.ERR_ROOM_01 });
                if (room != null)
                {
                    var not = new KEGS_USER_UNIT_DIE_COMPLETE_NOT { UnitUid = ev.FirstSenderUid };
                    foreach (long uid in room.GetMemberUnitUids())
                        Router.Send(PerformerId.PI_CN_ROOM, room.Info.RoomUid, PerformerId.PI_GS_USER, uid,
                            EventId.ERM_USER_UNIT_DIE_COMPLETE_NOT, not);
                }
                break;
            }

            case EventId.ERM_BATTLE_FIELD_NPC_LOAD_COMPLETE_REQ:
            {
                var room = RoomManager.GetRoomByUnitUid(ev.FirstSenderUid);
                if (room != null)
                    room.MarkStageLoaded(ev.FirstSenderUid, out _, out _);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_CN_ROOM, room?.Info.RoomUid ?? 0, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ERM_BATTLE_FIELD_NPC_LOAD_COMPLETE_ACK, new KPacketOK { Ok = room != null ? NetError.NET_OK : NetError.ERR_ROOM_01 });
                break;
            }

            case EventId.ERM_BATTLE_FIELD_ZU_ECHO_ACK:
                if (ev.FirstSenderUid > 0)
                    _zombieEchoTimes[ev.FirstSenderUid] = DateTime.UtcNow;
                break;

            case EventId.ERM_BATTLE_FIELD_ZOMBIE_ALERT_NOT:
                if (ev.FirstSenderUid > 0)
                {
                    var room = RoomManager.GetRoomByUnitUid(ev.FirstSenderUid);
                    if (room != null) RoomManager.LeaveRoom(room.Info.RoomUid, ev.FirstSenderUid);
                    _zombieEchoTimes.TryRemove(ev.FirstSenderUid, out _);
                }
                break;

            case EventId.ERM_BOSS_FIELD_INTRUDE_RESTRICTION_REQ:
            {
                var room = RoomManager.GetRoomByUnitUid(ev.FirstSenderUid);
                int ok = NetError.NET_OK;
                if (room == null || (room.BossFieldStateFlags & 1) == 0)
                    ok = NetError.ERR_FIELD_BOSS_03;
                else if (!room.IsHost(ev.FirstSenderUid))
                    ok = NetError.ERR_FIELD_BOSS_04;
                else
                    room.BossFieldStateFlags |= 2;

                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_CN_ROOM, room?.Info.RoomUid ?? 0, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ERM_BOSS_FIELD_INTRUDE_RESTRICTION_ACK, new KPacketOK { Ok = ok });
                break;
            }

            case EventId.ERM_SKIP_CONTINUE_DUNGEON_NOT:
            {
                var room = RoomManager.GetRoomByUnitUid(ev.FirstSenderUid);
                if (room != null)
                {
                    var result = room.FinishDungeonGame(isWin: false);
                    foreach (long uid in room.GetMemberUnitUids())
                        Router.Send(PerformerId.PI_CN_ROOM, room.Info.RoomUid, PerformerId.PI_GS_USER, uid,
                            EventId.ERM_END_GAME_DUNGEON_RESULT_DATA_NOT, result);
                }
                break;
            }

            case EventId.ERM_PARTY_BAN_USER_REQ:
            {
                var room = RoomManager.GetRoomByUnitUid(ev.FirstSenderUid);
                if (room != null)
                    room.RemoveUser(ev.FirstSenderUid);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_CN_ROOM, room?.Info.RoomUid ?? 0, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ERM_PARTY_BAN_USER_ACK, new KPacketOK { Ok = room != null ? NetError.NET_OK : NetError.ERR_SLOT_05 });
                break;
            }

            case EventId.ERM_DUNGEON_KILLALLNPC_CHECK_REQ:
            {
                var room = RoomManager.GetRoomByUnitUid(ev.FirstSenderUid);
                if (room != null && room.IsHost(ev.FirstSenderUid))
                {
                    Router.Send(PerformerId.PI_CN_ROOM, room.Info.RoomUid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ERM_DUNGEON_KILLALLNPC_CHECK_ACK, new KEGS_DUNGEON_KILLALLNPC_CHECK_ACK { Result = true });
                }
                break;
            }

            case EventId.ERM_UPDATE_HENIR_REWARD_USER_NOT:
                if (ev.FirstSenderUid > 0)
                    _henirRewardedUnits[ev.FirstSenderUid] = 1;
                break;

            case EventId.ERM_SYNC_DUNGEON_TIMER_NOT:
            {
                var room = RoomManager.GetRoomByUnitUid(ev.FirstSenderUid);
                if (room != null && room.IsHost(ev.FirstSenderUid))
                {
                    var not = new KEGS_SYNC_DUNGEON_TIMER_NOT { RemainTime = 60 };
                    foreach (long uid in room.GetMemberUnitUids())
                        Router.Send(PerformerId.PI_CN_ROOM, room.Info.RoomUid, PerformerId.PI_GS_USER, uid,
                            EventId.ERM_SYNC_DUNGEON_TIMER_NOT, not);
                }
                break;
            }

            case EventId.ERM_STATE_CHANGE_GAME_INTRUDE_REQ:
            {
                var room = RoomManager.GetRoomByUnitUid(ev.FirstSenderUid);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_CN_ROOM, room?.Info.RoomUid ?? 0, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ERM_STATE_CHANGE_GAME_INTRUDE_ACK, new KPacketOK { Ok = room != null ? NetError.NET_OK : NetError.ERR_ROOM_18 });
                break;
            }

            case EventId.ERM_REFRESH_ROOM_REQ:
            {
                var room = RoomManager.GetRoomByUnitUid(ev.FirstSenderUid);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_CN_ROOM, room?.Info.RoomUid ?? 0, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ERM_REFRESH_ROOM_ACK, new KEGS_REFRESH_ROOM_ACK
                        {
                            Ok = room != null ? NetError.NET_OK : NetError.ERR_ROOM_01,
                            RoomInfo = room?.Info ?? new KRoomInfo(),
                            Slot = room?.SnapshotSlots() ?? [],
                        });
                break;
            }

            case EventId.ERM_TRADE_START_NOT:
            case EventId.ERM_TRADE_FINISH_NOT:
            {
                bool isTrading = ev.EventId == EventId.ERM_TRADE_START_NOT;
                var room = RoomManager.GetRoomByUnitUid(ev.FirstSenderUid);
                if (room != null)
                {
                    var tradeNot = new KEGS_CHANGE_TRADE_NOT { UnitUid = ev.FirstSenderUid, Trade = isTrading };
                    foreach (long uid in room.GetMemberUnitUids())
                        Router.Send(PerformerId.PI_CN_ROOM, room.Info.RoomUid, PerformerId.PI_GS_USER, uid,
                            EventId.ERM_CHANGE_TRADE_NOT, tradeNot);
                }
                break;
            }

            case EventId.ERM_BREAK_UP_SUCCESS_NOT:
            {
                var room = RoomManager.GetRoomByUnitUid(ev.FirstSenderUid);
                var slot = room?.Slots.FirstOrDefault(s => s.RoomUserInfo.UnitUid == ev.FirstSenderUid);
                if (slot != null)
                    slot.RoomUserInfo.LoverUnitUid = 0;
                break;
            }

            case EventId.ERM_CHECK_OPEN_PERSONAL_SHOP_REQ:
            {
                var sq = Squares.Values.FirstOrDefault(s => s.SnapshotMembers().Any(m => m.UnitUid == ev.FirstSenderUid));
                var member = sq?.SnapshotMembers().FirstOrDefault(m => m.UnitUid == ev.FirstSenderUid);
                int ok = member == null ? NetError.ERR_PERSONAL_SHOP_03
                    : member.IsOpeningPersonalShop ? NetError.ERR_PERSONAL_SHOP_22
                    : NetError.NET_OK;
                if (member != null && ok == NetError.NET_OK)
                    member.IsOpeningPersonalShop = true;
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_CN_ROOM, sq?.SquareUid ?? 0, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ERM_CHECK_OPEN_PERSONAL_SHOP_ACK, new KPacketOK { Ok = ok });
                break;
            }

            case EventId.ERM_OFFER_TRADE_REQ:
            {
                var tr = TradeRooms.Values.FirstOrDefault(t => t.HasParticipant(ev.FirstSenderUid));
                if (tr != null)
                {
                    var (ok, bothOffered) = tr.OfferTrade(ev.FirstSenderUid);
                    if (ev.FirstSenderUid > 0)
                        Router.Send(PerformerId.PI_CN_ROOM, tr.TradeUid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                            EventId.ERM_OFFER_TRADE_ACK, new KPacketOK { Ok = ok });
                    if (ok == NetError.NET_OK)
                    {
                        foreach (long uid in tr.GetParticipantUnitUids())
                            Router.Send(PerformerId.PI_CN_ROOM, tr.TradeUid, PerformerId.PI_GS_USER, uid,
                                EventId.ERM_OFFER_TRADE_NOT, new KPacketOK { Ok = bothOffered ? 1 : 0 });
                    }
                }
                else if (ev.FirstSenderUid > 0)
                {
                    Router.Send(PerformerId.PI_CN_ROOM, 0, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ERM_OFFER_TRADE_ACK, new KPacketOK { Ok = NetError.ERR_TRADE_02 });
                }
                break;
            }

            case EventId.ERM_CHECK_START_WEDDING_REQ:
            {
                var hall = WeddingHalls.Values.FirstOrDefault(h => h.IsGroomOrBride(ev.FirstSenderUid));
                int ok = NetError.ERR_RELATIONSHIP_41;
                if (hall != null)
                {
                    var (checkOk, partnerUid) = hall.CheckStartWedding(ev.FirstSenderUid);
                    ok = checkOk;
                    if (checkOk == NetError.NET_OK && partnerUid > 0)
                    {
                        Router.Send(PerformerId.PI_CN_ROOM, hall.RoomUid, PerformerId.PI_GS_USER, partnerUid,
                            EventId.ERM_CHECK_START_WEDDING_NOT, new KPacketOK { Ok = NetError.NET_OK });
                    }
                }
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_CN_ROOM, hall?.RoomUid ?? 0, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ERM_CHECK_START_WEDDING_ACK, new KPacketOK { Ok = ok });
                break;
            }

            default:
                KLog.Clog($"[{Name}] Processed in-process event {ev.EventId}");
                break;
        }
    }

    public override bool DestroyAllSession()
    {
        _simLayer.ActorManager.DestroyAll();
        return true;
    }

    public override void ShutDown()
    {
        if (!IsRunning) return;
        IsRunning = false;
        try { _tickCts?.Cancel(); } catch { }
        try { _tickTask?.Wait(1500); } catch { }
        try { ScriptRefreshManager.StopAsync().GetAwaiter().GetResult(); } catch { }
        base.ShutDown();
        ServerReady = false;
    }
}
