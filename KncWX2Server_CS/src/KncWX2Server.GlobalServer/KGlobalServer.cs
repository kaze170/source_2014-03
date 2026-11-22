using System.Diagnostics;
using KncWX2Server.Core.Events;
using KncWX2Server.Core.Framework;
using KncWX2Server.Core.Logging;
using KncWX2Server.Core.Packets;
using KncWX2Server.Core.Region;
using KncWX2Server.Core.Scripting;
using MoonSharp.Interpreter;

namespace KncWX2Server.GlobalServer;

public sealed class KGbSimLayer : KSimLayer { }

public sealed class KGbNetLayer : KNetLayer
{
    private readonly KGlobalServer _server;

    public KGbNetLayer(KGlobalServer server)
    {
        _server = server;
        Port = server.Region.DefaultGlobalPort;
    }

    public override bool Init()
        => TcpServer.Start(Port, session => { session.ReserveDestroy(); return Task.CompletedTask; }, () => _server.Region);
}

/// <summary>
/// Managed port of KGlobalServer (79 C++ files).
/// Coordinates PvP official matchmaking and dungeon AutoParty matchmaking across regions.
/// </summary>
public sealed class KGlobalServer : KBaseServer
{
    private readonly KGbSimLayer _simLayer = new();
    private readonly KGbNetLayer _netLayer;
    private readonly bool _dbInitialized;
    private CancellationTokenSource? _tickCts;
    private Task? _tickTask;

    public KGbMatchMakingManager MatchMakingManager { get; } = new();
    public KGbAutoPartyManager AutoPartyManager { get; } = new();
    public KGlobalMissionManager GlobalMissionManager { get; } = new();
    public KFieldBossManager FieldBossManager { get; } = new();
    public KGlobalBlockListManager BlockListManager { get; } = new();
    public KWorldMissionManager WorldMissionManager { get; } = new();
    public KServerSNManager ServerSnManager { get; } = new();
    public KGlobalServerCCUManager ServerCcuManager { get; } = new();
    public KExchangeLimitManager ExchangeLimitManager { get; } = new();
    public KTimeEnchantEventManager TimeEnchantEventManager { get; } = new();
    public KJackPotEventManager JackPotEventManager { get; } = new();
    public KGameSysValTable GameSysValTable { get; } = new();
    private readonly Dictionary<string, KHeroPvpUserInfo> _heroPvpUsers = new(StringComparer.OrdinalIgnoreCase);
    private readonly HashSet<long> _heroPvpObservers = new();
    private readonly List<(bool Connected, KHeroPvpUserInfo Info)> _heroPvpUserChangeInfo = new();
    private readonly Dictionary<long, KRoomUserInfo> _pendingMatchUsers = new();
    private readonly Dictionary<long, List<KRoomUserInfo>> _pendingRegroupParties = new();
    private readonly Dictionary<long, (long UserUid, long GameServerUid, long ServerGroupId, string NickName)> _onlineUnits = new();
    private readonly Dictionary<sbyte, int> _disconnectReports = new();
    private readonly Dictionary<int, int> _todayRandomQuests = new();
    private readonly object _heroPvpSync = new();

    public override uint PerformerTypeId => PerformerId.PI_GB_SERVER;
    public override KSimLayer SimLayer => _simLayer;
    public override KNetLayer NetLayer => _netLayer;
    public ushort ListenPort => _netLayer.Port;
    public bool IsRunning { get; private set; }
    public KScriptRefreshManager ScriptRefreshManager { get; }

    public event Action<MatchFoundResult>? MatchFormed;

    public KGlobalServer(
        RegionCode initialRegion = RegionCode.TW,
        InProcessRouter? sharedRouter = null,
        string? dbPath = null)
        : base(sharedRouter)
    {
        Region = RegionProfile.FromCode(initialRegion);
        _simLayer.NationFlag = Region.DefaultNationFlag;
        _netLayer = new KGbNetLayer(this);
        if (!string.IsNullOrWhiteSpace(dbPath))
        {
            DbLayer.Initialize(dbPath);
            _dbInitialized = true;
            InitJackPotFromDb();
        }
        ScriptRefreshManager = new KScriptRefreshManager(Router, PerformerId.PI_GB_SERVER, Uid);
        ScriptRefreshManager.SetDefaultReloadHandler(_ => LoadServerResources());
    }

    public bool InitJackPotFromDb()
    {
        var rows = DbLayer.Sqlite.GetJackpotEventCoupons();
        if (rows.Count == 0)
            return false;
        return JackPotEventManager.Init(rows.Select(r => new KJackPotEventManager.KJackPotEventInitInfo
        {
            CouponNumber = r.CouponNumber,
            BeginDate = r.BeginDate,
            EndDate = r.EndDate,
            UserUid = r.UserUid,
            UnitUid = r.UnitUid,
            State = r.State,
        }));
    }

    public bool Start(ushort? overridePort = null, int serverGroupId = 0)
    {
        if (IsRunning) return true;
        ServerGroupId = serverGroupId;
        Uid = KncUid.MakeUid(Region, serverGroupId, 400);
        Name = "KGlobalServer";

        _netLayer.Port = overridePort ?? Region.DefaultGlobalPort;
        Router.RegisterPerformer(PerformerId.PI_GB_SERVER, this);
        Router.RegisterPerformer(PerformerId.PI_GLOBAL_MATCH_MAKING, this);
        Router.RegisterPerformer(PerformerId.PI_GLOBAL_AUTO_PARTY, this);
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
        ScriptManager.RegisterGlobal("GameSysVal", GameSysValTable);
        ScriptManager.RegisterGlobal("MatchManager", MatchMakingManager);
        ScriptManager.RegisterGlobal("AutoPartyManager", AutoPartyManager);
        ScriptManager.RegisterGlobal("BlockListManager", BlockListManager);
        ScriptManager.RegisterGlobal("GlobalMissionManager", GlobalMissionManager);
        ScriptManager.RegisterGlobal("WorldMissionManager", WorldMissionManager);
        ScriptRefreshManager.BindToLua(ScriptManager, "ScriptRefresh");
    }

    public void SwitchRegion(RegionCode region)
    {
        Region = RegionProfile.FromCode(region);
        _simLayer.NationFlag = Region.DefaultNationFlag;
    }

    public int LoadServerResources(string? resourceBaseDir = null)
    {
        var script = new KScriptManager();
        GameSysValTable.BindToLua(script, "GameSysVal");
        MatchMakingManager.BindToLua(script, "PvpMatchManager");
        AutoPartyManager.BindToLua(script, "AutoPartyScriptManager");
        WorldMissionManager.BindToLua(script, "WorldMissionManager");
        FieldBossManager.BindToLua(script, "FieldBossData");
        ScriptRefreshManager.BindToLua(script, "ScriptRefresh");

        int loaded = 0;
        foreach (string f in new[]
        {
            "GameSysValTable.lua",
            "PvpMatchData.lua",
            "PvpNpcData.lua",
            "PvpNpcData_EN.lua",
            "AutoPartyData.lua",
            "WorldMissionConfig.lua",
            "FieldBossData.lua"
        })
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
            try { Tick(); } catch (Exception ex) { KLog.Cerr($"KGlobalServer Tick error: {ex.Message}"); }
            sw.Stop();
            try { await Task.Delay(Math.Max(5, 50 - (int)sw.ElapsedMilliseconds), ct); }
            catch (OperationCanceledException) { break; }
        }
    }

    private DateTime _lastShutDownCheckUtc = DateTime.MinValue;
    private DateTime _lastEarthQuakeUtc = DateTime.UtcNow;
    private int _nextEarthQuakeSeconds = 240;
    private int _lastWorldBuffId;

    public override void Tick()
    {
        base.Tick();
        _simLayer.Tick();
        int prevWorldBuffId = WorldMissionManager.WorldBuffId;
        WorldMissionManager.Tick();
        if (WorldMissionManager.WorldBuffId != prevWorldBuffId)
        {
            _lastWorldBuffId = WorldMissionManager.WorldBuffId;
            Router.Send(PerformerId.PI_GB_SERVER, Uid, PerformerId.PI_GS_SERVER, 0,
                EventId.EGB_UPDATE_WORLD_BUFF_NOT, new KEGB_UPDATE_WORLD_BUFF_NOT
                {
                    Active = WorldMissionManager.IsWorldBuffActive() || WorldMissionManager.IsActive,
                    SuccessWorldBuff = (ushort)Math.Max(0, WorldMissionManager.WorldBuffId),
                    WorldBuffDurationTime = WorldMissionManager.WorldBuffDurationMinutes,
                });
        }
        GlobalMissionManager.Tick();
        ExchangeLimitManager.Tick();
        TimeEnchantEventManager.Tick();
        CheckSendMSGBeforeShutDown();
        CheckEarthQuakeEffect();
        CheckJackPotEvent();
        CheckHeroPvpUserList();
        long nowUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        if (FieldBossManager.Tick(nowUnix))
        {
            var info = BuildFieldBossInfoPacket(nowUnix);
            Router.Send(PerformerId.PI_GB_SERVER, Uid, PerformerId.PI_GS_SERVER, 0,
                EventId.EGB_BATTLE_FIELD_BOSS_INFO_NOT, info);
        }
        var matches = MatchMakingManager.TickMatchmaking();
        foreach (var m in matches)
        {
            MatchFormed?.Invoke(m);
            var successNot = new KEGS_MATCH_MAKING_SUCCESS_NOT { MatchUid = m.MatchUid };
            foreach (var member in m.TeamA.Concat(m.TeamB))
            {
                Router.Send(PerformerId.PI_GB_SERVER, Uid, PerformerId.PI_GS_USER, member.UnitUid,
                    EventId.EGB_MATCH_MAKING_SUCCESS_NOT, successNot);
            }
        }
    }

    public bool CheckMSGTimeBeforeShutDown(DateTime? nowLocal = null)
    {
        var curr = nowLocal ?? DateTime.Now;
        foreach (var ts in ShutDownMessageTimes)
        {
            if (ts.Hours == curr.Hour && ts.Minutes == curr.Minute)
                return true;
        }
        return false;
    }

    public void CheckSendMSGBeforeShutDown(DateTime? nowUtc = null, DateTime? nowLocal = null)
    {
        var utc = nowUtc ?? DateTime.UtcNow;
        if ((utc - _lastShutDownCheckUtc).TotalSeconds < 60.0)
            return;
        _lastShutDownCheckUtc = utc;

        if (CheckMSGTimeBeforeShutDown(nowLocal))
        {
            Router.Send(PerformerId.PI_GB_SERVER, Uid, PerformerId.PI_GS_SERVER, 0,
                EventId.EGB_SHUT_DOWN_NOTIFY_NOT,
                new KPacketString { Value = "Shutdown notice: Play access is restricted from 00:00 to 06:00 under the shutdown policy." });
        }
    }

    public void CheckEarthQuakeEffect(DateTime? nowUtc = null)
    {
        var utc = nowUtc ?? DateTime.UtcNow;
        if ((utc - _lastEarthQuakeUtc).TotalSeconds < _nextEarthQuakeSeconds)
            return;

        _lastEarthQuakeUtc = utc;
        _nextEarthQuakeSeconds = WorldMissionManager.GetRandomEarthQuakeTime();
        Router.Send(PerformerId.PI_GB_SERVER, Uid, PerformerId.PI_GS_SERVER, 0,
            EventId.EGB_EARTH_QUAKE_NOT, new KPacketString { Value = string.Empty });
    }

    public void CheckJackPotEvent(DateTime? nowLocal = null, bool bypassThrottle = false)
    {
        JackPotEventManager.Tick(out var winInfos, out var endInfos, nowLocal, bypassThrottle);

        foreach (var win in winInfos)
        {
            string winnerNick = string.Empty;
            lock (_heroPvpSync)
            {
                if (_onlineUnits.TryGetValue(win.UnitUid, out var u))
                    winnerNick = u.NickName;
            }
            if (string.IsNullOrEmpty(winnerNick))
            {
                JackPotEventManager.ReInsert(win);
                continue;
            }

            if (_dbInitialized)
            {
                int dbOk = DbLayer.Sqlite.UpdateJackpotEventCouponWinner(win.CouponNumber, win.UserUid, win.UnitUid, win.State);
                if (dbOk != NetError.NET_OK)
                {
                    JackPotEventManager.FaildProcess(win.UserUid, win.UnitUid, win.CouponNumber);
                    KLog.Cerr($"[JackPotEvent] DB update failed for unit={win.UnitUid}, coupon={win.CouponNumber}");
                    continue;
                }
            }

            Router.Send(PerformerId.PI_GB_SERVER, Uid, PerformerId.PI_GS_SERVER, 0,
                EventId.EGB_JACKPOT_EVENT_NOTIFY_NOT, new KEGB_JACKPOT_EVENT_NOTIFY_NOT
                {
                    WinnerUnitUid = win.UnitUid,
                    WinnerNickName = winnerNick,
                    RewardItemId = 0,
                });

            Router.Send(PerformerId.PI_GB_SERVER, Uid, PerformerId.PI_GS_SERVER, 0,
                EventId.EGB_JACKPOT_EVENT_POST_NOT, new KEGB_JACKPOT_EVENT_POST_NOT
                {
                    UnitUid = win.UnitUid,
                    NickName = winnerNick,
                    RewardItemId = 0,
                    Quantity = 1,
                });
        }

        foreach (var end in endInfos)
        {
            KLog.Cout($"[JackPotEvent] Ended {end.BeginDate}~{end.EndDate}, remainingCoupons={end.ValidCouponCount}");
        }
    }

    public void CheckHeroPvpUserList()
    {
        List<(bool Connected, KHeroPvpUserInfo Info)> changes;
        List<long> observers;
        lock (_heroPvpSync)
        {
            if (_heroPvpUserChangeInfo.Count == 0 || _heroPvpObservers.Count == 0)
            {
                _heroPvpUserChangeInfo.Clear();
                return;
            }
            changes = new List<(bool Connected, KHeroPvpUserInfo Info)>(_heroPvpUserChangeInfo);
            _heroPvpUserChangeInfo.Clear();
            observers = _heroPvpObservers.ToList();
        }

        var not = new KEGS_ADMIN_HERO_PVP_USER_CONNECTION_NOT { UserList = changes };
        foreach (long obsUid in observers)
        {
            Router.Send(PerformerId.PI_GB_SERVER, Uid, PerformerId.PI_GS_USER, obsUid,
                EventId.EGB_ADMIN_HERO_PVP_USER_CONNECTION_NOT, not);
        }
    }

    public KEGB_BATTLE_FIELD_BOSS_INFO_NOT BuildFieldBossInfoPacket(long? currentUnixSeconds = null)
    {
        long nowUnix = currentUnixSeconds ?? DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var (remainPortal, remainCool) = FieldBossManager.GetRemainingTimes(nowUnix);
        return new KEGB_BATTLE_FIELD_BOSS_INFO_NOT
        {
            RemainPortalTime = remainPortal,
            RemainCoolTime = remainCool,
            PortalAppearanceMap = FieldBossManager.PortalAppearanceMap,
            PortalDestination = FieldBossManager.PortalDestination,
            PortalOpen = FieldBossManager.IsPortalOpen,
            CoolTime = FieldBossManager.IsCoolTime,
        };
    }

    public List<KHeroPvpUserInfo> GetHeroPvpUsers()
    {
        lock (_heroPvpSync)
            return _heroPvpUsers.Values.ToList();
    }

    protected override void ProcessEvent(KEvent ev)
    {
        switch (ev.EventId)
        {
            case EventId.E_HEART_BEAT:
                break;

            case EventId.EGB_REQUEST_MATCH_MAKING_REQ:
            {
                var req = ev.GetPayload<KEGB_REQUEST_MATCH_MAKING_REQ>(Region);
                foreach (var u in req.MatchUserInfo)
                {
                    MatchMakingManager.Enqueue(new MatchRequestEntry(
                        u.UnitUid,
                        u.NickName,
                        u.Rating,
                        Math.Max((sbyte)1, req.PvpPlayerCount),
                        DateTime.UtcNow,
                        IsBeginner: u.CurrentRank <= 0,
                        IsArrangeRank: u.OfficialMatchCnt < 5,
                        PartyUid: u.PartyUid), regFront: req.RegFrontOfList);
                }
                break;
            }

            case EventId.EGB_CANCEL_MATCH_MAKING_REQ:
            {
                var req = ev.GetPayload<KEGB_CANCEL_MATCH_MAKING_REQ>(Region);
                MatchMakingManager.CancelByRequest(ev.FirstSenderUid, req.MatchUid, req.PartyUid, out var affectedUnits);
                lock (_heroPvpSync)
                {
                    if (ev.FirstSenderUid > 0)
                        _pendingMatchUsers.Remove(ev.FirstSenderUid);
                    foreach (var uid in affectedUnits)
                        _pendingMatchUsers.Remove(uid);
                }
                if (affectedUnits.Count > 0)
                {
                    var closeNot = new KEGS_MATCH_CLOSE_NOT
                    {
                        Reason = req.Reason != 0 ? req.Reason : NetError.NOT_LEAVE_MATCH_REASON_01,
                        CancelUnitUid = ev.FirstSenderUid,
                    };
                    foreach (var uid in affectedUnits)
                    {
                        Router.Send(PerformerId.PI_GB_SERVER, Uid, PerformerId.PI_GS_USER, uid,
                            EventId.EGB_MATCH_CLOSE_NOT, closeNot);
                    }
                }
                break;
            }

            case EventId.EGB_MATCH_MAKING_SUCCESS_REPLY_NOT:
            {
                var req = ev.GetPayload<KEGB_MATCH_MAKING_SUCCESS_REPLY_NOT>(Region);
                if (!MatchMakingManager.SubmitReply(req.UnitUid, req.Accept, out var readyMatch, out var cancelledMatch))
                    break;

                if (cancelledMatch != null)
                {
                    var closeNot = new KEGS_MATCH_CLOSE_NOT
                    {
                        Reason = NetError.NOT_LEAVE_MATCH_REASON_01,
                        CancelUnitUid = req.UnitUid,
                    };
                    lock (_heroPvpSync)
                    {
                        foreach (var m in cancelledMatch.TeamA.Concat(cancelledMatch.TeamB))
                            _pendingMatchUsers.Remove(m.UnitUid);
                    }
                    foreach (var m in cancelledMatch.TeamA.Concat(cancelledMatch.TeamB))
                    {
                        Router.Send(PerformerId.PI_GB_SERVER, Uid, PerformerId.PI_GS_USER, m.UnitUid,
                            EventId.EGB_MATCH_CLOSE_NOT, closeNot);
                    }
                    break;
                }

                if (readyMatch != null && readyMatch.TeamA.Count > 0)
                {
                    int totalSlots = readyMatch.TeamA.Count + readyMatch.TeamB.Count;
                    var pvpInfo = MatchMakingManager.PvpMatchInfos.TryGetValue(readyMatch.MatchType, out var mi)
                        ? mi
                        : (TeamCount: Math.Max(1, readyMatch.TeamA.Count), PlayTimeSec: 300);
                    short worldId = MatchMakingManager.PvpMapsForMatch.Count > 0
                        ? (short)MatchMakingManager.PvpMapsForMatch[0]
                        : (short)10001;

                    var openReq = new KEGB_OPEN_PVP_ROOM_FOR_MATCH_REQ
                    {
                        MatchUid = readyMatch.MatchUid,
                        MatchType = readyMatch.MatchType,
                        PvpNpcType = 0,
                        RoomInfo = new KRoomInfo
                        {
                            RoomType = 0, // RT_PVP
                            RoomName = $"Match_{readyMatch.MatchUid}",
                            MaxSlot = (sbyte)Math.Clamp(totalSlots, 2, 8),
                            PvpGameType = (sbyte)Math.Max(1, pvpInfo.TeamCount),
                            WorldId = worldId,
                        },
                    };
                    lock (_heroPvpSync)
                    {
                        foreach (var m in readyMatch.TeamA)
                        {
                            openReq.RedTeam.Add(_pendingMatchUsers.Remove(m.UnitUid, out var ru)
                                ? ru
                                : new KRoomUserInfo { UnitUid = m.UnitUid, NickName = m.NickName });
                        }
                        foreach (var m in readyMatch.TeamB)
                        {
                            openReq.BlueTeam.Add(_pendingMatchUsers.Remove(m.UnitUid, out var ru)
                                ? ru
                                : new KRoomUserInfo { UnitUid = m.UnitUid, NickName = m.NickName });
                        }
                    }
                    Router.Send(PerformerId.PI_GB_SERVER, Uid, PerformerId.PI_GS_USER, readyMatch.TeamA[0].UnitUid,
                        EventId.EGB_OPEN_PVP_ROOM_FOR_MATCH_REQ, openReq);
                }
                break;
            }

            case EventId.EGB_REQUEST_AUTO_PARTY_MAKING_REQ:
            {
                var req = ev.GetPayload<KEGB_REQUEST_AUTO_PARTY_MAKING_REQ>(Region);
                int maxParty = req.AutoPartyCondition.LimitPlayerCount > 0 ? req.AutoPartyCondition.LimitPlayerCount : 4;
                foreach (var u in req.AutoPartyUser)
                {
                    AutoPartyManager.Enqueue(new AutoPartyEntry(
                        u.UnitUid,
                        u.NickName,
                        req.AutoPartyCondition.DungeonId,
                        req.AutoPartyCondition.DifficultyLevel,
                        DateTime.UtcNow,
                        req.AutoPartyCondition.DungeonMode,
                        u.OldPartyInfo.OldPartyUid),
                        minPartySize: 2,
                        maxPartySize: maxParty,
                        remakingAutoParty: req.RemakingAutoParty);
                }
                while (AutoPartyManager.TryDequeueGroup(out var group) && group != null)
                {
                    var foundNot = new KEGS_AUTO_PARTY_MAKING_SUCCESS_NOT
                    {
                        AutoPartyUid = group.AutoPartyUid,
                        MemberCount = group.Members.Count,
                        DungeonId = group.DungeonId,
                        DifficultyLevel = group.DifficultyLevel,
                        DungeonMode = group.DungeonMode,
                    };
                    foreach (var m in group.Members)
                    {
                        Router.Send(PerformerId.PI_GB_SERVER, Uid, PerformerId.PI_GS_USER, m.UnitUid,
                            EventId.EGB_AUTO_PARTY_MAKING_SUCCESS_NOT, foundNot);
                    }
                }
                break;
            }

            case EventId.EGB_CANCEL_AUTO_PARTY_MAKING_REQ:
            {
                var req = ev.GetPayload<KEGB_CANCEL_AUTO_PARTY_MAKING_REQ>(Region);
                AutoPartyManager.CancelByRequest(ev.FirstSenderUid, req.AutoPartyUid, req.AutoPartyWaitNumber, out var affectedUnits);
                lock (_heroPvpSync)
                {
                    if (req.AutoPartyUid > 0)
                        _pendingRegroupParties.Remove(req.AutoPartyUid);
                    foreach (var list in _pendingRegroupParties.Values)
                    {
                        if (ev.FirstSenderUid > 0)
                            list.RemoveAll(u => u.UnitUid == ev.FirstSenderUid);
                        foreach (var uid in affectedUnits)
                            list.RemoveAll(u => u.UnitUid == uid);
                    }
                }
                if (affectedUnits.Count > 0)
                {
                    var closeNot = new KEGS_AUTO_PARTY_CLOSE_NOT
                    {
                        Reason = req.Reason != 0 ? req.Reason : NetError.ERR_MATCH_MAKING_08,
                        CancelUnitUid = ev.FirstSenderUid,
                    };
                    foreach (var uid in affectedUnits)
                    {
                        Router.Send(PerformerId.PI_GB_SERVER, Uid, PerformerId.PI_GS_USER, uid,
                            EventId.EGB_AUTO_PARTY_CLOSE_NOT, closeNot);
                    }
                }
                break;
            }

            case EventId.EGB_AUTO_PARTY_MAKING_SUCCESS_REPLY_NOT:
            {
                var req = ev.GetPayload<KEGB_AUTO_PARTY_MAKING_SUCCESS_REPLY_NOT>(Region);
                if (!AutoPartyManager.SubmitReply(req.UnitUid, req.Accept, out var readyGroup, out var cancelledGroup))
                    break;

                if (cancelledGroup != null)
                {
                    lock (_heroPvpSync)
                        _pendingRegroupParties.Remove(cancelledGroup.AutoPartyUid);
                    var closeNot = new KEGS_AUTO_PARTY_CLOSE_NOT
                    {
                        Reason = NetError.NOT_LEAVE_AUTO_PARTY_REASON_01,
                        CancelUnitUid = req.UnitUid,
                    };
                    foreach (var m in cancelledGroup.Members)
                    {
                        Router.Send(PerformerId.PI_GB_SERVER, Uid, PerformerId.PI_GS_USER, m.UnitUid,
                            EventId.EGB_AUTO_PARTY_CLOSE_NOT, closeNot);
                    }
                    break;
                }

                if (readyGroup != null && readyGroup.Members.Count > 0)
                {
                    var regroupReq = new KEPM_REGROUP_PARTY_FOR_AUTO_PARTY_DUNGEON_GAME_REQ
                    {
                        AutoPartyUid = readyGroup.AutoPartyUid,
                        AutoPartyHostUnitUid = readyGroup.Members[0].UnitUid,
                        CreatePartyInfo = new KCreatePartyInfo
                        {
                            PartyType = 0, // PT_DUNGEON_PARTY
                            DungeonId = readyGroup.DungeonId,
                            DifficultyLevel = readyGroup.DifficultyLevel,
                            DungeonMode = readyGroup.DungeonMode,
                        },
                    };
                    List<KRoomUserInfo>? cachedUsers;
                    lock (_heroPvpSync)
                        _pendingRegroupParties.Remove(readyGroup.AutoPartyUid, out cachedUsers);
                    foreach (var m in readyGroup.Members)
                    {
                        var cached = cachedUsers?.FirstOrDefault(u => u.UnitUid == m.UnitUid);
                        regroupReq.Members.Add(new KPartyUserInfo
                        {
                            UnitUid = m.UnitUid,
                            NickName = string.IsNullOrEmpty(cached?.NickName) ? m.NickName : cached.NickName,
                            UnitClass = cached?.UnitClass ?? 0,
                            Level = cached?.Level ?? 0,
                        });
                    }
                    Router.Send(PerformerId.PI_GB_SERVER, Uid, PerformerId.PI_GS_USER, readyGroup.Members[0].UnitUid,
                        EventId.EGB_REGROUP_PARTY_FOR_AUTO_PARTY_DUNGEON_GAME_REQ, regroupReq);
                }
                break;
            }

            case EventId.EGB_GET_AUTO_PARTY_BONUS_INFO_REQ:
            {
                long senderGs = ev.FirstSenderUid;
                if (senderGs > 0)
                {
                    Router.Send(PerformerId.PI_GB_SERVER, Uid, PerformerId.PI_GS_SERVER, senderGs,
                        EventId.EGB_GET_AUTO_PARTY_BONUS_INFO_ACK,
                        new KEGB_GET_AUTO_PARTY_BONUS_INFO_ACK
                        {
                            AutoPartyBonusInfo = AutoPartyManager.GetAllAutoPartyBonusInfos(),
                        });
                }
                break;
            }

            case EventId.EGB_INCREASE_THE_PROTECTED_CRYSTAL_COUNT_NOT:
            {
                var req = ev.GetPayload<KEGB_INCREASE_THE_PROTECTED_CRYSTAL_COUNT_NOT>(Region);
                WorldMissionManager.IncreaseProtectedCrystalCount(req.DeltaCrystalCount);
                break;
            }

            case EventId.EGB_GET_MISSION_INFO_REQ:
            {
                var req = ev.GetPayload<KEGB_GET_MISSION_INFO_REQ>(Region);
                if (req.RequesterServerUid > 0)
                {
                    int rem = WorldMissionManager.MissionEndUtc.HasValue
                        ? Math.Max(0, (int)(WorldMissionManager.MissionEndUtc.Value - DateTime.UtcNow).TotalSeconds)
                        : 0;
                    var ack = new KEGB_GET_MISSION_INFO_ACK
                    {
                        IsActive = WorldMissionManager.IsActive,
                        StartedCrystalCount = WorldMissionManager.StartedCrystalCount,
                        ProtectedCrystalCount = WorldMissionManager.ProtectedCrystalCount,
                        WorldBuffId = WorldMissionManager.WorldBuffId,
                        IndividualBuffLevel = WorldMissionManager.IndividualBuffLevel,
                        RemainingSeconds = rem,
                    };
                    Router.Send(PerformerId.PI_GB_SERVER, Uid, PerformerId.PI_GS_SERVER, req.RequesterServerUid,
                        EventId.EGB_GET_MISSION_INFO_ACK, ack);
                }
                break;
            }

            case EventId.EGB_VERIFY_SERVER_CONNECT_REQ:
            {
                var req = ev.GetPayload<KEGB_VERIFY_SERVER_CONNECT_REQ>(Region);
                if (req.ServerUid > 0)
                {
                    Router.Send(PerformerId.PI_GB_SERVER, Uid, PerformerId.PI_GS_SERVER, req.ServerUid,
                        EventId.EGB_VERIFY_SERVER_CONNECT_ACK, new KPacketOK { Ok = NetError.NET_OK });
                }
                break;
            }

            case EventId.EGB_UPDATE_CCU_INFO_NOT:
            {
                var req = ev.GetPayload<KEGB_UPDATE_CCU_INFO_NOT>(Region);
                ServerCcuManager.UpdateServerCcu(req.ServerUid, req.ServerGroupId, req.ConcurrentUser, req.CurPartyCount);
                break;
            }

            case EventId.EGB_REGISTER_UNIT_NOT:
            {
                var req = ev.GetPayload<KEGB_REGISTER_UNIT_NOT>(Region);
                if (req.ServerSn > 0)
                    ServerSnManager.RegServerSN(req.ServerSn, req.UserUid, req.GameServerUid);
                if (req.UserUid > 0 && req.UnitUid > 0)
                    JackPotEventManager.AddUser(req.UserUid, req.UnitUid);
                if (req.UnitUid > 0)
                {
                    lock (_heroPvpSync)
                    {
                        _onlineUnits[req.UnitUid] = (req.UserUid, req.GameServerUid, req.ServerGroupId, req.NickName);
                        if (!string.IsNullOrWhiteSpace(req.NickName) && _heroPvpUsers.TryGetValue(req.NickName, out var hero))
                        {
                            hero.On = true;
                            _heroPvpUserChangeInfo.Add((true, new KHeroPvpUserInfo
                            {
                                ServerGroupId = hero.ServerGroupId,
                                NickName = hero.NickName,
                                On = true,
                                Connect = hero.Connect,
                            }));
                        }
                    }
                }
                break;
            }

            case EventId.EGB_UNIT_DISCONNECT_NOT:
            {
                var req = ev.GetPayload<KEGB_UNIT_DISCONNECT_NOT>(Region);
                if (req.ServerSn > 0)
                    ServerSnManager.UnRegServerSN(req.ServerSn, req.UserUid);
                if (req.UserUid > 0)
                    JackPotEventManager.RemoveUser(req.UserUid, req.UnitUid);
                lock (_heroPvpSync)
                {
                    if (req.UnitUid > 0)
                    {
                        _heroPvpObservers.Remove(req.UnitUid);
                        if (_onlineUnits.TryGetValue(req.UnitUid, out var existing) &&
                            !string.IsNullOrWhiteSpace(existing.NickName) &&
                            _heroPvpUsers.TryGetValue(existing.NickName, out var hero))
                        {
                            hero.On = false;
                            _heroPvpUserChangeInfo.Add((false, new KHeroPvpUserInfo
                            {
                                ServerGroupId = hero.ServerGroupId,
                                NickName = hero.NickName,
                                On = false,
                                Connect = hero.Connect,
                            }));
                        }
                        _onlineUnits.Remove(req.UnitUid);
                    }
                    else
                    {
                        foreach (long uid in _onlineUnits.Where(kv => kv.Value.UserUid == req.UserUid).Select(kv => kv.Key).ToList())
                        {
                            _heroPvpObservers.Remove(uid);
                            if (_onlineUnits.TryGetValue(uid, out var existing) &&
                                !string.IsNullOrWhiteSpace(existing.NickName) &&
                                _heroPvpUsers.TryGetValue(existing.NickName, out var hero))
                            {
                                hero.On = false;
                                _heroPvpUserChangeInfo.Add((false, new KHeroPvpUserInfo
                                {
                                    ServerGroupId = hero.ServerGroupId,
                                    NickName = hero.NickName,
                                    On = false,
                                    Connect = hero.Connect,
                                }));
                            }
                            _onlineUnits.Remove(uid);
                        }
                    }
                }
                break;
            }

            case EventId.EGB_UPDATE_GLOBAL_MISSION_INFO_NOT:
            {
                var req = ev.GetPayload<KEGB_UPDATE_GLOBAL_MISSION_INFO_NOT>(Region);
                GlobalMissionManager.AddProgress(req.MissionId, req.DeltaCount);
                break;
            }

            case EventId.EGB_GET_GLOBAL_MISSION_INFO_REQ:
            {
                var req = ev.GetPayload<KEGB_GET_GLOBAL_MISSION_INFO_REQ>(Region);
                var mission = GlobalMissionManager.GetMission(req.MissionId);
                if (req.RequesterServerUid > 0)
                {
                    Router.Send(PerformerId.PI_GB_SERVER, Uid, PerformerId.PI_GS_SERVER, req.RequesterServerUid,
                        EventId.EGB_GET_GLOBAL_MISSION_INFO_ACK, new KEGB_GET_GLOBAL_MISSION_INFO_ACK
                        {
                            Ok = mission != null ? NetError.NET_OK : NetError.ERR_CLIENT_STATE,
                            MissionId = req.MissionId,
                            Title = mission?.Title ?? string.Empty,
                            CurrentCount = mission != null ? Interlocked.Read(ref mission.CurrentCount) : 0,
                            TargetCount = mission?.TargetCount ?? 0,
                            IsActive = mission?.IsActive ?? false,
                            Succeeded = mission?.Succeeded,
                        });
                }
                break;
            }

            case EventId.EGB_BATTLE_FIELD_BOSS_INFO_NOT:
            {
                var info = BuildFieldBossInfoPacket();
                if (ev.FirstSenderUid > 0)
                {
                    Router.Send(PerformerId.PI_GB_SERVER, Uid, PerformerId.PI_GS_SERVER, ev.FirstSenderUid,
                        EventId.EGB_BATTLE_FIELD_BOSS_INFO_NOT, info);
                }
                break;
            }

            case EventId.EGB_ADMIN_BOSS_FIELD_GATE_OPEN_REQ:
            {
                var req = ev.GetPayload<KEGS_ADMIN_BOSS_FIELD_GATE_OPEN_REQ>(Region);
                int ok = FieldBossManager.BossFieldOpenProcess(req.BattleFieldId, DateTimeOffset.UtcNow.ToUnixTimeSeconds());
                if (ev.FirstSenderUid > 0)
                {
                    Router.Send(PerformerId.PI_GB_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.EGB_ADMIN_BOSS_FIELD_GATE_OPEN_ACK, new KPacketOK { Ok = ok });
                }
                if (ok == NetError.NET_OK && ev.LastSenderUid > 0)
                {
                    Router.Send(PerformerId.PI_GB_SERVER, Uid, PerformerId.PI_GS_SERVER, ev.LastSenderUid,
                        EventId.EGB_BATTLE_FIELD_BOSS_INFO_NOT, BuildFieldBossInfoPacket());
                }
                break;
            }

            case EventId.EGB_ADMIN_BOSS_FIELD_GATE_CLOSE_NOT:
            {
                bool wasPortal = FieldBossManager.IsPortalOpen;
                bool wasCool = FieldBossManager.IsCoolTime;
                if (wasPortal) FieldBossManager.ClosePortal();
                if (wasCool)
                {
                    FieldBossManager.SetCoolTimeState(false);
                    FieldBossManager.ClearTotalDangerousValue();
                }
                if ((wasPortal != FieldBossManager.IsPortalOpen || wasCool != FieldBossManager.IsCoolTime) && ev.LastSenderUid > 0)
                {
                    Router.Send(PerformerId.PI_GB_SERVER, Uid, PerformerId.PI_GS_SERVER, ev.LastSenderUid,
                        EventId.EGB_BATTLE_FIELD_BOSS_INFO_NOT, BuildFieldBossInfoPacket());
                }
                break;
            }

            case EventId.EGB_ADMIN_GET_TOTAL_DANGEROUS_VALUE_REQ:
            {
                var req = ev.GetPayload<KEGS_ADMIN_GET_TOTAL_DANGEROUS_VALUE_REQ>(Region);
                if (ev.FirstSenderUid > 0)
                {
                    Router.Send(PerformerId.PI_GB_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.EGB_ADMIN_GET_TOTAL_DANGEROUS_VALUE_ACK, new KEGS_ADMIN_GET_TOTAL_DANGEROUS_VALUE_ACK
                        {
                            Ok = NetError.NET_OK,
                            BattleFieldId = req.BattleFieldId,
                            TotalDangerousValue = FieldBossManager.GetTotalDangerousValue(req.BattleFieldId),
                        });
                }
                break;
            }

            case EventId.EGB_ADMIN_SET_TOTAL_DANGEROUS_VALUE_REQ:
            {
                var req = ev.GetPayload<KEGS_ADMIN_SET_TOTAL_DANGEROUS_VALUE_REQ>(Region);
                byte val = (byte)Math.Clamp(
                    req.Set ? req.TotalDangerousValue : req.TotalDangerousValue + FieldBossManager.GetTotalDangerousValue(req.BattleFieldId),
                    0, 255);
                int ok = FieldBossManager.TotalDangerousValueUpdateProcess(req.BattleFieldId, val, DateTimeOffset.UtcNow.ToUnixTimeSeconds());
                if (ev.FirstSenderUid > 0)
                {
                    Router.Send(PerformerId.PI_GB_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.EGB_ADMIN_SET_TOTAL_DANGEROUS_VALUE_ACK, new KPacketOK { Ok = ok });
                }
                break;
            }

            case EventId.EGB_ADMIN_HERO_PVP_USER_LIST_REQ:
            {
                if (ev.FirstSenderUid > 0)
                {
                    lock (_heroPvpSync)
                        _heroPvpObservers.Add(ev.FirstSenderUid);
                    Router.Send(PerformerId.PI_GB_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.EGB_ADMIN_HERO_PVP_USER_LIST_ACK, new KEGS_ADMIN_HERO_PVP_USER_LIST_ACK
                        {
                            UserList = GetHeroPvpUsers(),
                        });
                }
                break;
            }

            case EventId.EGB_ADMIN_HERO_PVP_USER_ADD_REQ:
            {
                var req = ev.GetPayload<KEGS_ADMIN_HERO_PVP_USER_ADD_REQ>(Region);
                lock (_heroPvpSync)
                {
                    foreach (var (sgId, nick) in req.UserList)
                    {
                        if (string.IsNullOrWhiteSpace(nick)) continue;
                        var info = new KHeroPvpUserInfo
                        {
                            ServerGroupId = sgId,
                            NickName = nick,
                            On = true,
                        };
                        _heroPvpUsers[nick] = info;
                        _heroPvpUserChangeInfo.Add((true, info));
                    }
                }
                if (ev.FirstSenderUid > 0)
                {
                    Router.Send(PerformerId.PI_GB_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.EGB_ADMIN_HERO_PVP_USER_ADD_ACK, req);
                }
                break;
            }

            case EventId.EGB_ADMIN_HERO_PVP_USER_REMOVE_REQ:
            {
                var req = ev.GetPayload<KEGS_ADMIN_HERO_PVP_USER_ADD_REQ>(Region);
                lock (_heroPvpSync)
                {
                    foreach (var (_, nick) in req.UserList)
                    {
                        if (_heroPvpUsers.Remove(nick, out var removed))
                            _heroPvpUserChangeInfo.Add((false, removed));
                    }
                }
                if (ev.FirstSenderUid > 0)
                {
                    Router.Send(PerformerId.PI_GB_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.EGB_ADMIN_HERO_PVP_USER_REMOVE_ACK, req);
                }
                break;
            }

            case EventId.EGB_CHECK_BLOCK_USER_NOT:
            {
                var req = ev.GetPayload<KEGB_CHECK_BLOCK_USER_NOT>(Region);
                var result = new KEGB_UPDATE_BLOCK_RESULT_NOT();
                foreach (int blockType in req.BlockType)
                {
                    switch (blockType)
                    {
                        case 0: // BT_TRADE_BLOCK
                            if (BlockListManager.IsTradeBlockUser(req.UserUid))
                                result.BlockResult[blockType] = true;
                            break;
                        case 1: // BT_ACCOUNT_BLOCK
                            if (BlockListManager.IsAccountBlockUser(req.UserUid))
                                result.BlockResult[blockType] = true;
                            break;
                        case 2: // BT_IP_BLOCK
                            if (!string.IsNullOrEmpty(req.Ip) && BlockListManager.IsIpBlockUser(req.Ip))
                                result.BlockResult[blockType] = true;
                            break;
                    }
                }
                long targetUnit = req.UnitUid > 0 ? req.UnitUid : ev.FirstSenderUid;
                if (result.BlockResult.Count > 0 && targetUnit > 0)
                {
                    Router.Send(PerformerId.PI_GB_SERVER, Uid, PerformerId.PI_GS_USER, targetUnit,
                        EventId.EGB_UPDATE_BLOCK_RESULT_NOT, result);
                }
                break;
            }

            case EventId.EGB_REGISTER_SERVERSN_REQ:
            {
                var req = ev.GetPayload<KEGB_REGISTER_SERVERSN_REQ>(Region);
                var (res, overlapUsers) = ServerSnManager.RegServerSN(req.ServerSn, req.UserUid, ev.LastSenderUid);
                if (ev.FirstSenderUid > 0)
                {
                    Router.Send(PerformerId.PI_GB_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.EGB_REGISTER_SERVERSN_ACK,
                        new KEGB_REGISTER_SERVERSN_ACK { Ok = res, UserUid = req.UserUid });
                }
                if (res != NetError.NET_OK)
                {
                    foreach (var kv in overlapUsers)
                    {
                        if (kv.Key == req.UserUid) continue;
                        Router.Send(PerformerId.PI_GB_SERVER, Uid, PerformerId.PI_GS_USER, kv.Key,
                            EventId.EGB_REGISTER_SERVERSN_NOT,
                            new KEGB_REGISTER_SERVERSN_ACK { Ok = res, UserUid = kv.Key });
                    }
                }
                break;
            }

            case EventId.EGB_UNREGISTER_SERVERSN_NOT:
            {
                var req = ev.GetPayload<KEGB_UNREGISTER_SERVERSN_NOT>(Region);
                ServerSnManager.UnRegServerSN(req.ServerSn, req.UserUid);
                break;
            }

            case EventId.EGB_UPDATE_TOTAL_DANGEROUS_VALUE_NOT:
            {
                var req = ev.GetPayload<KEGB_UPDATE_TOTAL_DANGEROUS_VALUE_NOT>(Region);
                long nowUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                byte nextVal = (byte)Math.Min(255, FieldBossManager.GetTotalDangerousValue(req.BattleFieldId) + req.IncreasedValue);
                if (FieldBossManager.TotalDangerousValueUpdateProcess(req.BattleFieldId, nextVal, nowUnix) == NetError.NET_OK &&
                    FieldBossManager.IsPortalOpen)
                {
                    Router.Send(PerformerId.PI_GB_SERVER, Uid, PerformerId.PI_GS_SERVER, 0,
                        EventId.EGB_BATTLE_FIELD_BOSS_INFO_NOT, BuildFieldBossInfoPacket(nowUnix));
                }
                break;
            }

            case EventId.EGB_REQUEST_AUTO_PARTY_WAIT_USER_COUNT_NOT:
            {
                var req = ev.GetPayload<KEGB_REQUEST_AUTO_PARTY_WAIT_USER_COUNT_NOT>(Region);
                int cnt = AutoPartyManager.GetWaitUserCount(req.DungeonId);
                if (ev.FirstSenderUid > 0)
                {
                    Router.Send(PerformerId.PI_GB_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.EGS_AUTO_PARTY_WAIT_USER_COUNT_NOT,
                        new KEGS_AUTO_PARTY_WAIT_USER_COUNT_NOT { AutoPartyWaitUserCount = cnt });
                }
                break;
            }

            case EventId.EGB_MATCH_MAKING_ADMIN_COMMAND_NOT:
            {
                var req = ev.GetPayload<KEGB_MATCH_MAKING_ADMIN_COMMAND_NOT>(Region);
                if (req.CommandType == KEGB_MATCH_MAKING_ADMIN_COMMAND_NOT.CtClear)
                    MatchMakingManager.ClearAllMatchInfo();
                break;
            }

            case EventId.EGB_AUTO_PARTY_MAKING_ADMIN_COMMAND_NOT:
            {
                var req = ev.GetPayload<KEGB_AUTO_PARTY_MAKING_ADMIN_COMMAND_NOT>(Region);
                if (req.CommandType == KEGB_AUTO_PARTY_MAKING_ADMIN_COMMAND_NOT.CtClear)
                    AutoPartyManager.ClearAllAutoPartyInfo();
                break;
            }

            case EventId.EGB_ADMIN_HERO_PVP_USER_TOURNAMENT_CONNECTION_NOT:
            {
                var req = ev.GetPayload<KEGB_ADMIN_HERO_PVP_USER_TOURNAMENT_CONNECTION_NOT>(Region);
                lock (_heroPvpSync)
                {
                    if (!string.IsNullOrWhiteSpace(req.NickName))
                    {
                        if (_heroPvpUsers.TryGetValue(req.NickName, out var existing))
                        {
                            existing.Connect = req.Connect;
                            _heroPvpUserChangeInfo.Add((true, new KHeroPvpUserInfo
                            {
                                ServerGroupId = existing.ServerGroupId,
                                NickName = existing.NickName,
                                On = existing.On,
                                Connect = req.Connect,
                            }));
                        }
                        else
                        {
                            var info = new KHeroPvpUserInfo
                            {
                                ServerGroupId = req.ServerGroupId,
                                NickName = req.NickName,
                                On = req.Connect,
                                Connect = req.Connect,
                            };
                            _heroPvpUsers[req.NickName] = info;
                            _heroPvpUserChangeInfo.Add((true, info));
                        }
                    }
                }
                break;
            }

            case EventId.EGB_EXCHANGE_LIMIT_REQ:
            {
                var req = ev.GetPayload<KEGB_EXCHANGE_LIMIT_REQ>(Region);
                var (res, limitUid, rbUid) = ExchangeLimitManager.RetrieveExchangeLimitData(req.DestItemId);
                int ok = res switch
                {
                    KExchangeLimitManager.ExchangeLimitResult.NotInited => NetError.ERR_EXCHANGE_LIMIT_01,
                    KExchangeLimitManager.ExchangeLimitResult.Exhausted => NetError.ERR_EXCHANGE_LIMIT_02,
                    _ => NetError.NET_OK,
                };
                if (ev.FirstSenderUid > 0)
                {
                    Router.Send(PerformerId.PI_GB_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.EGB_EXCHANGE_LIMIT_ACK, new KEGB_EXCHANGE_LIMIT_ACK
                        {
                            Ok = ok,
                            ExchangeLimitUid = ExchangeLimitManager.ExchangeLimitUid,
                            LimitUid = limitUid,
                            RollBackUid = rbUid,
                            AfterWorkStorageKey = req.AfterWorkStorageKey,
                        });
                }
                break;
            }

            case EventId.EGB_EXCHANGE_LIMIT_INFO_ROLLBACK_NOT:
            {
                var req = ev.GetPayload<KEGB_EXCHANGE_LIMIT_INFO_ROLLBACK_NOT>(Region);
                if (ExchangeLimitManager.CheckExchangeLimitUid(req.ExchangeLimitUid))
                    ExchangeLimitManager.ReInsertExchangeLimitData(req.LimitUid);
                break;
            }

            case EventId.EGB_TIME_ENCHANT_EVENT_NOT:
            {
                var req = ev.GetPayload<KEGB_TIME_ENCHANT_EVENT_NOT>(Region);
                var result = new KTimeEnchantEventResult
                {
                    AfterWorkStorageKey = req.AfterWorkStorageKey,
                    TimeEnchantEventLevel = 0,
                    NotifyGetItem = true,
                };
                if (TimeEnchantEventManager.TryRetrieve(out var entry, out _) && entry != null)
                {
                    result.TimeEnchantEventLevel = entry.EnchantLevel;
                    result.NotifyGetItem = entry.NotifyGetItem;
                }
                if (ev.FirstSenderUid > 0)
                {
                    Router.Send(PerformerId.PI_GB_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        (EventId)req.EventId, result);
                }
                break;
            }

            case EventId.EGB_OPEN_PVP_ROOM_FOR_MATCH_ACK:
            {
                var ack = ev.GetPayload<KPacketOK>(Region);
                if (ack.Ok != NetError.NET_OK)
                    KLog.Cwarn($"[{Name}] EGB_OPEN_PVP_ROOM_FOR_MATCH_ACK failed: {ack.Ok}");
                break;
            }

            case EventId.EGB_GET_ROOM_USER_INFO_FOR_MATCH_ACK:
            {
                var ack = ev.GetPayload<KEGB_GET_ROOM_USER_INFO_FOR_MATCH_ACK>(Region);
                lock (_heroPvpSync)
                    _pendingMatchUsers[ack.RoomUserInfo.UnitUid] = ack.RoomUserInfo;
                break;
            }

            case EventId.EGB_GET_REGROUP_PARTY_INFO_ACK:
            {
                var ack = ev.GetPayload<KEGB_GET_REGROUP_PARTY_INFO_ACK>(Region);
                lock (_heroPvpSync)
                {
                    if (!_pendingRegroupParties.TryGetValue(ack.AutoPartyUid, out var list))
                        _pendingRegroupParties[ack.AutoPartyUid] = list = new List<KRoomUserInfo>();
                    list.RemoveAll(u => u.UnitUid == ack.RoomUserInfo.UnitUid);
                    list.Add(ack.RoomUserInfo);
                }
                break;
            }

            case EventId.EGB_REGROUP_PARTY_FOR_AUTO_PARTY_DUNGEON_GAME_ACK:
            {
                var ack = ev.GetPayload<KEPM_REGROUP_PARTY_FOR_AUTO_PARTY_DUNGEON_GAME_ACK>(Region);
                if (ack.Ok != NetError.NET_OK)
                    KLog.Cwarn($"[{Name}] EGB_REGROUP_PARTY_FOR_AUTO_PARTY_DUNGEON_GAME_ACK failed: {ack.Ok}");
                break;
            }

            case EventId.EGB_ADMIN_NOTIFY_HERO_PVP_USER_REQ:
            {
                var req = ev.GetPayload<KEGS_ADMIN_NOTIFY_HERO_PVP_USER_REQ>(Region);
                int ok = NetError.NET_OK;
                var targets = new List<long>();
                lock (_heroPvpSync)
                {
                    if (string.IsNullOrWhiteSpace(req.NickName))
                    {
                        foreach (var hero in _heroPvpUsers.Values.Where(h => h.On))
                        {
                            var found = _onlineUnits.Values.FirstOrDefault(u =>
                                (hero.ServerGroupId < 0 || u.ServerGroupId == hero.ServerGroupId) &&
                                string.Equals(u.NickName, hero.NickName, StringComparison.OrdinalIgnoreCase));
                            if (found.UserUid > 0)
                                targets.Add(found.UserUid);
                        }
                    }
                    else
                    {
                        if (!_heroPvpUsers.ContainsKey(req.NickName))
                            ok = NetError.ERR_SEARCH_UNIT_03;
                        else
                        {
                            var found = _onlineUnits.Values.FirstOrDefault(u =>
                                (req.ServerGroupId < 0 || u.ServerGroupId == req.ServerGroupId) &&
                                string.Equals(u.NickName, req.NickName, StringComparison.OrdinalIgnoreCase));
                            if (found.UserUid > 0)
                                targets.Add(found.UserUid);
                            else
                                ok = NetError.ERR_SEARCH_UNIT_03;
                        }
                    }
                }
                if (ev.FirstSenderUid > 0)
                {
                    Router.Send(PerformerId.PI_GB_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.EGB_ADMIN_NOTIFY_HERO_PVP_USER_ACK, new KPacketOK { Ok = ok });
                }
                foreach (long userUid in targets)
                {
                    Router.Send(PerformerId.PI_GB_SERVER, Uid, PerformerId.PI_GS_USER, userUid,
                        EventId.EGB_ADMIN_NOTIFY_HERO_PVP_USER_NOT, req);
                }
                break;
            }

            case EventId.EGB_CHECK_INVALID_AUTO_PARTY_NOT:
                AutoPartyManager.ClearAllAutoPartyInfo();
                break;

            case EventId.EGB_CHECK_INVALID_MATCH_NOT:
                MatchMakingManager.ClearAllMatchInfo();
                break;

            case EventId.EGB_CLEAR_MISSION_DUNGEON_NOT:
            {
                var pkt = ev.GetPayload<KPacketOK>(Region);
                WorldMissionManager.IncreaseProtectedCrystalCount(pkt.Ok);
                break;
            }

            case EventId.EGB_RESET_INCREASE_THE_PROTECTED_CRYSTAL_COUNT_NOT:
                WorldMissionManager.ResetProtectedCrystalCount();
                break;

            case EventId.EGB_GLOBAL_MISSION_UPDATE_NOT:
            {
                var req = ev.GetPayload<KEGB_GLOBAL_MISSION_UPDATE_NOT>(Region);
                GlobalMissionManager.IncreaseGlobalMissionClearCount(req.GlobalMissionDay, req.NowCount);
                break;
            }

            case EventId.EGB_EXCHANGE_LIMIT_INFO_REQ:
            {
                if (ev.LastSenderUid > 0)
                {
                    int limitUid = ExchangeLimitManager.ExchangeLimitUid;
                    var ack = new KEGB_EXCHANGE_LIMIT_INFO_NOT
                    {
                        ExchangeLimitUid = limitUid,
                        ExchangeLimitInfo = limitUid != 0 ? ExchangeLimitManager.GetBeginExchangeLimitInfo() : new Dictionary<int, int>(),
                    };
                    Router.Send(PerformerId.PI_GB_SERVER, Uid, PerformerId.PI_GS_SERVER, ev.LastSenderUid,
                        EventId.EGB_EXCHANGE_LIMIT_INFO_ACK, ack);
                }
                break;
            }

            case EventId.EGB_TIME_ENCHANT_EVENT_INFO_REQ:
            {
                if (ev.LastSenderUid > 0)
                {
                    var ack = new KEGB_TIME_ENCHANT_EVENT_INFO_NOT
                    {
                        TimeEnchantEventUid = TimeEnchantEventManager.TimeEnchantEventUid,
                        NumOfTimeEnchantEvent = TimeEnchantEventManager.GetNumOfBeginTimeEnchantEventInfo(),
                    };
                    Router.Send(PerformerId.PI_GB_SERVER, Uid, PerformerId.PI_GS_SERVER, ev.LastSenderUid,
                        EventId.EGB_TIME_ENCHANT_EVENT_INFO_ACK, ack);
                }
                break;
            }

            case EventId.EGB_TODAY_RANDOM_QUEST_REQ:
            {
                var req = ev.GetPayload<KPacketLong>(Region);
                long targetGs = req.Value > 0 ? req.Value : ev.LastSenderUid;
                if (targetGs > 0)
                {
                    Dictionary<int, int> quests;
                    lock (_heroPvpSync)
                        quests = new Dictionary<int, int>(_todayRandomQuests);
                    Router.Send(PerformerId.PI_GB_SERVER, Uid, PerformerId.PI_GS_SERVER, targetGs,
                        EventId.EGB_TODAY_RANDOM_QUEST_ACK, new KEGB_TODAY_RANDOM_QUEST_ACK { TodayRandomQuest = quests });
                }
                break;
            }

            case EventId.EGB_INVITE_PARTY_FIND_RECEIVER_REQ:
            {
                var req = ev.GetPayload<KELG_INVITE_PARTY_FIND_RECEIVER_REQ>(Region);
                (long UserUid, long GameServerUid, long ServerGroupId, string NickName) foundInfo = default;
                long foundUnitUid = 0;
                lock (_heroPvpSync)
                {
                    if (req.ReceiverUnitUid > 0 && _onlineUnits.TryGetValue(req.ReceiverUnitUid, out foundInfo))
                    {
                        foundUnitUid = req.ReceiverUnitUid;
                    }
                    else if (!string.IsNullOrWhiteSpace(req.ReceiverNickName))
                    {
                        var kv = _onlineUnits.FirstOrDefault(u =>
                            string.Equals(u.Value.NickName, req.ReceiverNickName, StringComparison.OrdinalIgnoreCase));
                        if (kv.Key > 0)
                        {
                            foundUnitUid = kv.Key;
                            foundInfo = kv.Value;
                        }
                    }
                }
                if (ev.FirstSenderUid > 0)
                {
                    Router.Send(PerformerId.PI_GB_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.EGB_INVITE_PARTY_FIND_RECEIVER_ACK, new KELG_INVITE_PARTY_FIND_RECEIVER_ACK
                        {
                            Ok = foundUnitUid > 0 ? NetError.NET_OK : NetError.ERR_PARTY_07,
                            HostUnitUid = req.HostUnitUid,
                            ReceiverUnitUid = foundUnitUid > 0 ? foundUnitUid : req.ReceiverUnitUid,
                            ReceiverNickName = foundUnitUid > 0 ? foundInfo.NickName : req.ReceiverNickName,
                        });
                }
                break;
            }

            case EventId.EGB_INVITE_PARTY_GET_RECEIVER_USER_INFO_ACK:
            {
                var ack = ev.GetPayload<KELG_INVITE_PARTY_FIND_RECEIVER_ACK>(Region);
                (long UserUid, long GameServerUid, long ServerGroupId, string NickName) hostInfo = default;
                lock (_heroPvpSync)
                    _onlineUnits.TryGetValue(ack.HostUnitUid, out hostInfo);
                if (hostInfo.UserUid > 0)
                {
                    Router.Send(PerformerId.PI_GB_SERVER, Uid, PerformerId.PI_GS_USER, hostInfo.UserUid,
                        EventId.EGB_INVITE_PARTY_FIND_RECEIVER_ACK, ack);
                }
                break;
            }

            case EventId.EGB_MODULE_INFO_UPDATE_NOT:
            {
                var req = ev.GetPayload<KEGS_MODULE_INFO_UPDATE_NOT>(Region);
                BlockListManager.UpdateModuleList(req.ModuleName);
                var matched = BlockListManager.CheckHackingModuleList(req.ModuleName);
                if (matched.Count > 0 && ev.FirstSenderUid > 0)
                {
                    var not = new KELG_CHECK_MODULE_INFO_UPDATE_NOT { Ok = NetError.ERR_CHECK_DLL_01 };
                    foreach (string mod in matched)
                        not.ModuleName[mod] = 1;
                    Router.Send(PerformerId.PI_GB_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ELG_CHECK_MODULE_INFO_UPDATE_NOT, not);
                }
                break;
            }

            case EventId.ELG_DISCONNECT_SERVER_REPORT_NOT:
            {
                var req = ev.GetPayload<KE_DISCONNECT_SERVER_REPORT_NOT>(Region);
                lock (_heroPvpSync)
                {
                    _disconnectReports.TryGetValue(req.Type, out int cur);
                    _disconnectReports[req.Type] = cur + 1;
                }
                break;
            }

            case EventId.ESR_ORDER_TO_GLOBAL_SERVER_NOT:
            case EventId.ESR_ORDER_TO_REFRESH_MANAGER_REQ:
            case EventId.ESR_ORDER_TO_REFRESH_MANAGER_ACK:
            {
                var order = ev.GetPayload<KESR_SCRIPT_REFRESH_ORDER_NOT>(Region);
                if (order.OrderType > KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_SENTINEL &&
                    order.OrderType < KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GB_SENTINEL)
                {
                    ScriptRefreshManager.ExecuteRefreshOrder(order.OrderType);
                }
                else if (ev.EventId == EventId.ESR_ORDER_TO_GLOBAL_SERVER_NOT)
                {
                    KLog.Cwarn($"[{Name}] Undefined GlobalServer script refresh order type: {order.OrderType}");
                }
                break;
            }

            case EventId.DBE_JACKPOT_EVENT_INIT_REQ:
            {
                var rows = _dbInitialized
                    ? DbLayer.Sqlite.GetJackpotEventCoupons()
                    : new List<(string CouponNumber, string BeginDate, string EndDate, long UserUid, long UnitUid, int State)>();
                var ack = new KDBE_JACKPOT_EVENT_INIT_ACK
                {
                    JackpotEventInitInfo = rows.Select(r => new KJackPotEventInitInfo
                    {
                        CouponNumber = r.CouponNumber,
                        BeginDate = r.BeginDate,
                        EndDate = r.EndDate,
                        UserUid = r.UserUid,
                        UnitUid = r.UnitUid,
                        State = r.State,
                    }).ToList(),
                };
                if (ack.JackpotEventInitInfo.Count > 0)
                {
                    JackPotEventManager.Init(ack.JackpotEventInitInfo.Select(i => new KJackPotEventManager.KJackPotEventInitInfo
                    {
                        CouponNumber = i.CouponNumber,
                        BeginDate = i.BeginDate,
                        EndDate = i.EndDate,
                        UserUid = i.UserUid,
                        UnitUid = i.UnitUid,
                        State = i.State,
                    }));
                }
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_GB_SERVER, Uid, PerformerId.PI_GS_SERVER, ev.FirstSenderUid,
                        EventId.DBE_JACKPOT_EVENT_INIT_ACK, ack);
                break;
            }

            case EventId.DBE_JACKPOT_EVENT_INIT_ACK:
            {
                var ack = ev.GetPayload<KDBE_JACKPOT_EVENT_INIT_ACK>(Region);
                JackPotEventManager.Init(ack.JackpotEventInitInfo.Select(i => new KJackPotEventManager.KJackPotEventInitInfo
                {
                    CouponNumber = i.CouponNumber,
                    BeginDate = i.BeginDate,
                    EndDate = i.EndDate,
                    UserUid = i.UserUid,
                    UnitUid = i.UnitUid,
                    State = i.State,
                }));
                break;
            }

            case EventId.DBE_JACKPOT_EVENT_UPDATE_REQ:
            {
                var req = ev.GetPayload<KJackPotEventWinInfo>(Region);
                int ok = _dbInitialized
                    ? DbLayer.Sqlite.UpdateJackpotEventCouponWinner(req.CouponNumber, req.UserUid, req.UnitUid, req.State)
                    : NetError.NET_OK;
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_GB_SERVER, Uid, PerformerId.PI_GS_SERVER, ev.FirstSenderUid,
                        EventId.DBE_JACKPOT_EVENT_UPDATE_ACK, new KDBE_JACKPOT_EVENT_UPDATE_ACK
                        {
                            Ok = ok,
                            UnitUid = req.UnitUid,
                            CouponNumber = req.CouponNumber,
                        });
                break;
            }

            case EventId.DBE_JACKPOT_EVENT_UPDATE_ACK:
            {
                var ack = ev.GetPayload<KDBE_JACKPOT_EVENT_UPDATE_ACK>(Region);
                if (ack.Ok != NetError.NET_OK)
                {
                    KLog.Cerr($"[{Name}] DBE_JACKPOT_EVENT_UPDATE_ACK failed: Unit={ack.UnitUid}, Coupon={ack.CouponNumber}, Ok={ack.Ok}");
                    break;
                }
                string nick = string.Empty;
                lock (_heroPvpSync)
                {
                    if (_onlineUnits.TryGetValue(ack.UnitUid, out var u))
                        nick = u.NickName;
                }
                Router.Send(PerformerId.PI_GB_SERVER, Uid, PerformerId.PI_GS_SERVER, 0,
                    EventId.EGB_JACKPOT_EVENT_POST_NOT, new KEGB_JACKPOT_EVENT_POST_NOT
                    {
                        UnitUid = ack.UnitUid,
                        NickName = nick,
                        RewardItemId = 0,
                        Quantity = 1,
                    });
                break;
            }

            case EventId.E_RESERVE_DESTROY:
                ShutDown();
                break;

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
