using System.Diagnostics;
using KncWX2Server.ChannelServer.Auth;
using KncWX2Server.ChannelServer.DB;
using KncWX2Server.ChannelServer.Managers;
using KncWX2Server.Core.Events;
using KncWX2Server.Core.Framework;
using KncWX2Server.Core.Logging;
using KncWX2Server.Core.Network;
using KncWX2Server.Core.Packets;
using KncWX2Server.Core.Region;
using KncWX2Server.Core.Scripting;

namespace KncWX2Server.ChannelServer;

/// <summary>
/// Port of ChannelServer/ChannelServer.{h,cpp} running multi-region TW/HK/CN/JP ChannelServer,
/// SQLite DB worker threads, MoonSharp Lua configs, regional publisher auth, and live runtime region switching.
/// </summary>
public sealed class KChannelServer : KBaseServer
{
    private readonly KChannelSimLayer _simLayer;
    private readonly KChannelNetLayer _netLayer;
    private readonly Dictionary<RegionCode, IRegionalAuthProvider> _authProviders = new();
    private readonly object _channelLock = new();

    private Dictionary<int, KChannelInfo> _channelList = new();
    private Dictionary<int, KChannelBonusInfo> _channelBonusList = new();
    private Dictionary<int, KServerGroupInfo> _serverGroupList = new();

    private CancellationTokenSource? _tickCts;
    private Task? _tickTask;

    private DateTime _lastChannelRefresh = DateTime.MinValue;
    private DateTime _lastServerGroupRefresh = DateTime.MinValue;
    private DateTime _lastLoginStatFlush = DateTime.UtcNow;
    private DateTime _lastPerfFlush = DateTime.UtcNow;
    public bool IsLoginConnected { get; private set; }

    public override KSimLayer SimLayer => _simLayer;
    public override KNetLayer NetLayer => _netLayer;
    public KChannelSimLayer ChannelSimLayer => _simLayer;
    public KncWX2Server.Core.Database.SqliteDatabaseManager Db => DbLayer.Sqlite;

    public KCCUToolManager CcuToolManager { get; } = new();
    public KMachineBlockManager MachineBlockManager { get; } = new();
    public KHackingProtectionManager HackingProtectionManager { get; } = new();
    public KWaitQueueManager WaitQueueManager { get; } = new();
    public KDDOSGuardSimulator DdosGuardSimulator { get; } = new();
    public KGameSysVal GameSysVal { get; } = new();
    public KSMSPhoneNumberManager SmsPhoneNumberManager { get; } = new();

    public sealed record ChannelTemplate(int ChannelId, int ChannelType, int PvpClass, int MinLevel, int MaxLevel, int MinVp, int MaxVp);
    public Dictionary<int, ChannelTemplate> ChannelTemplates { get; } = new();
    public int StartPvpChannelId { get; private set; } = 3;
    public int StartArcadeChannelId { get; private set; }
    public int StartDungeonChannelId { get; private set; } = 100;

    public bool UdpRelayCheckerOn { get; private set; }
    public List<int> UdpRelayCheckerServerClasses { get; } = new();
    public bool UdpRelayCheckerPrintLog { get; private set; }
    public int UdpRelayCheckerPort { get; private set; } = 26424;
    public float UdpRelayCheckerTimeGap { get; private set; } = 5.0f;
    public float UdpRelayCheckerDbUpdateTimeGap { get; private set; } = 300.0f;

    public GashAuthManager GashAuthManager { get; }
    public PurpleAuthManager PurpleAuthManager { get; }
    public GiantLoginAuthManager GiantLoginAuthManager { get; }
    public KScriptRefreshManager ScriptRefreshManager { get; }

    public int TickCount { get; private set; }
    public bool IsRunning { get; private set; }

    public event Action? OnStateChanged;

    public KChannelServer(RegionCode initialRegion = RegionCode.TW, string dbPath = "KncWX2Server.db", InProcessRouter? sharedRouter = null)
        : base(sharedRouter)
    {
        Region = RegionProfile.FromCode(initialRegion);
        _simLayer = new KChannelSimLayer { NationFlag = Region.DefaultNationFlag };
        _netLayer = new KChannelNetLayer(this);
        DbLayer.Initialize(dbPath);

        GashAuthManager = new GashAuthManager(DbLayer.Sqlite, Router);
        PurpleAuthManager = new PurpleAuthManager(DbLayer.Sqlite, Router);
        GiantLoginAuthManager = new GiantLoginAuthManager(DbLayer.Sqlite, Router);

        _authProviders[RegionCode.TW] = GashAuthManager;
        _authProviders[RegionCode.HK] = GashAuthManager;
        _authProviders[RegionCode.JP] = PurpleAuthManager;
        _authProviders[RegionCode.CN] = GiantLoginAuthManager;

        ScriptRefreshManager = new KScriptRefreshManager(Router, PerformerId.PI_CHANNEL_SERVER, Uid);
        ScriptRefreshManager.SetDefaultReloadHandler(_ => LoadServerResources());
    }

    public IRegionalAuthProvider GetAuthProvider(RegionCode regionCode)
        => _authProviders.TryGetValue(regionCode, out var p) ? p : GashAuthManager;

    public Dictionary<int, KChannelInfo> GetChannelListSnapshot()
    {
        lock (_channelLock)
            return new Dictionary<int, KChannelInfo>(_channelList);
    }

    public Dictionary<int, KChannelBonusInfo> GetChannelBonusListSnapshot()
    {
        lock (_channelLock)
            return new Dictionary<int, KChannelBonusInfo>(_channelBonusList);
    }

    public Dictionary<int, KServerGroupInfo> GetServerGroupListSnapshot()
    {
        lock (_channelLock)
            return new Dictionary<int, KServerGroupInfo>(_serverGroupList);
    }

    public int LoadServerResources(string? resourceBaseDir = null)
    {
        var script = new KScriptManager();
        WaitQueueManager.BindToLua(script, "WaitQueueManager");
        GameSysVal.BindToLua(script, "GameSysVal");
        SmsPhoneNumberManager.BindToLua(script, "g_SMSPhoneNumberManager");

        MoonSharp.Interpreter.Table? chProxy = null;
        chProxy = script.CreateLuaProxy("g_pChannelManager", t =>
        {
            t["SetStartPVPID"] = MoonSharp.Interpreter.DynValue.NewCallback((_, a) =>
            {
                StartPvpChannelId = KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, chProxy!), StartPvpChannelId);
                return MoonSharp.Interpreter.DynValue.Nil;
            });
            t["SetStartArcadeID"] = MoonSharp.Interpreter.DynValue.NewCallback((_, a) =>
            {
                StartArcadeChannelId = KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, chProxy!), StartArcadeChannelId);
                return MoonSharp.Interpreter.DynValue.Nil;
            });
            t["SetStartDungeonID"] = MoonSharp.Interpreter.DynValue.NewCallback((_, a) =>
            {
                StartDungeonChannelId = KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, chProxy!), StartDungeonChannelId);
                return MoonSharp.Interpreter.DynValue.Nil;
            });
            t["AddCMTemplet"] = MoonSharp.Interpreter.DynValue.NewCallback((_, a) =>
            {
                var tbl = KScriptManager.ArgTable(a, KScriptManager.ArgStart(a, chProxy!));
                if (tbl != null)
                {
                    int id = (int)(tbl.Get("CHANNEL_ID").CastToNumber() ?? 0);
                    int type = (int)(tbl.Get("CHANNEL_TYPE").CastToNumber() ?? 0);
                    int pvpClass = (int)(tbl.Get("PVP_CLASS").CastToNumber() ?? 0);
                    int minLv = (int)(tbl.Get("MIN_LEVEL").CastToNumber() ?? 0);
                    int maxLv = (int)(tbl.Get("MAX_LEVEL").CastToNumber() ?? 0);
                    int minVp = (int)(tbl.Get("MIN_VP").CastToNumber() ?? 0);
                    int maxVp = (int)(tbl.Get("MAX_VP").CastToNumber() ?? 0);
                    if (id > 0) ChannelTemplates[id] = new ChannelTemplate(id, type, pvpClass, minLv, maxLv, minVp, maxVp);
                }
                return MoonSharp.Interpreter.DynValue.Nil;
            });
        });

        MoonSharp.Interpreter.Table? checkerProxy = null;
        checkerProxy = script.CreateLuaProxy("UdpRelayChecker", t =>
        {
            t["SetUdpRelayCheckerOn"] = MoonSharp.Interpreter.DynValue.NewCallback((_, a) =>
            {
                UdpRelayCheckerOn = KScriptManager.ArgBool(a, KScriptManager.ArgStart(a, checkerProxy!));
                return MoonSharp.Interpreter.DynValue.Nil;
            });
            t["AddUdpRelayCheckerOnServerClass"] = MoonSharp.Interpreter.DynValue.NewCallback((_, a) =>
            {
                int sc = KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, checkerProxy!));
                if (sc > 0 && !UdpRelayCheckerServerClasses.Contains(sc)) UdpRelayCheckerServerClasses.Add(sc);
                return MoonSharp.Interpreter.DynValue.Nil;
            });
            t["SetUdpRelayCheckerPrintLog"] = MoonSharp.Interpreter.DynValue.NewCallback((_, a) =>
            {
                UdpRelayCheckerPrintLog = KScriptManager.ArgBool(a, KScriptManager.ArgStart(a, checkerProxy!));
                return MoonSharp.Interpreter.DynValue.Nil;
            });
            t["SetUdpRelayCheckerPort"] = MoonSharp.Interpreter.DynValue.NewCallback((_, a) =>
            {
                UdpRelayCheckerPort = KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, checkerProxy!), UdpRelayCheckerPort);
                return MoonSharp.Interpreter.DynValue.Nil;
            });
            t["SetUdpRelayCheckerTimeGap"] = MoonSharp.Interpreter.DynValue.NewCallback((_, a) =>
            {
                UdpRelayCheckerTimeGap = KScriptManager.ArgFloat(a, KScriptManager.ArgStart(a, checkerProxy!), UdpRelayCheckerTimeGap);
                return MoonSharp.Interpreter.DynValue.Nil;
            });
            t["SetDBUpdateTimeGap"] = MoonSharp.Interpreter.DynValue.NewCallback((_, a) =>
            {
                UdpRelayCheckerDbUpdateTimeGap = KScriptManager.ArgFloat(a, KScriptManager.ArgStart(a, checkerProxy!), UdpRelayCheckerDbUpdateTimeGap);
                return MoonSharp.Interpreter.DynValue.Nil;
            });
        });

        int loaded = 0;
        foreach (string f in new[] { "WaitQueueData.lua", "GameSysValTable.lua", "SMSPhoneNumberList.lua", "channels.lua", "UdpRelayCheckerConfig.lua" })
        {
            if (script.DoServerResourceFile(f, Region, resourceBaseDir))
                loaded++;
        }
        lock (_channelLock)
            MergeChannelTemplatesLocked();
        return loaded;
    }

    public override void RegToLua()
    {
        base.RegToLua();
        ScriptManager.RegisterGlobal("WaitQueueManager", WaitQueueManager);
        ScriptManager.RegisterGlobal("GASHAuthManager", GashAuthManager);
        ScriptManager.RegisterGlobal("PurpleAuthManager", PurpleAuthManager);
        ScriptManager.RegisterGlobal("GameSysVal", GameSysVal);
        ScriptManager.RegisterGlobal("g_SMSPhoneNumberManager", SmsPhoneNumberManager);
        ScriptRefreshManager.BindToLua(ScriptManager, "ScriptRefresh");
    }

    public bool Start(string? configLuaPath = null, ushort? overridePort = null, int? serverGroupId = null)
    {
        if (IsRunning) return true;

        if (serverGroupId.HasValue)
            ServerGroupId = serverGroupId.Value;
        else
            ServerGroupId = Region.Region == RegionCode.CN ? 1 : 0;

        if (overridePort.HasValue)
            _netLayer.Port = overridePort.Value;

        base.Init();
        RegToLua();
        LoadServerResources();

        if (!string.IsNullOrWhiteSpace(configLuaPath) && File.Exists(configLuaPath))
        {
            KLog.Cout($"Loading Lua config: {configLuaPath}");
            ScriptManager.DoFile(configLuaPath);
        }

        if (overridePort.HasValue)
            _netLayer.Port = overridePort.Value;

        // Start SQLite DB threads
        DbLayer.RegisterThread(PerformerId.PI_CHANNEL_ACCOUNT_DB, new KChannelAccountDBThread(DbLayer.Sqlite, Router, () => Region), Router);
        DbLayer.RegisterThread(PerformerId.PI_CHANNEL_LOG_DB, new KChannelLogDBThread(DbLayer.Sqlite, Router, () => this), Router);
        DbLayer.RegisterThread(PerformerId.PI_CHANNEL_SMS_DB, new KChannelSMSDBThread(DbLayer.Sqlite, Router, () => Region), Router);

        // Start TCP listener
        if (!_netLayer.Init())
        {
            KLog.Cerr("Failed to initialize KChannelNetLayer.");
            return false;
        }

        // Request server info & initial lists from LogDB
        SendToLogDB(EventId.DBE_SERVER_ON_REQ, new KNetAddress { Ip = PublicIp, Port = _netLayer.Port });
        SendToLogDB(EventId.DBE_CHECK_HACKING_TOOL_LIST_REQ, new KDBE_CHECK_HACKING_TOOL_LIST_REQ());

        IsRunning = true;
        ScriptRefreshManager.Start();
        _tickCts = new CancellationTokenSource();
        _tickTask = Task.Run(() => TickLoopAsync(_tickCts.Token));
        OnStateChanged?.Invoke();
        return true;
    }

    public void SwitchRegion(RegionCode newRegion, string? configLuaPath = null)
    {
        Region = RegionProfile.FromCode(newRegion);
        RegionProfile.SetCurrent(Region);
        _simLayer.NationFlag = Region.DefaultNationFlag;
        ServerGroupId = newRegion == RegionCode.CN ? 1 : 0;

        if (!string.IsNullOrWhiteSpace(configLuaPath) && File.Exists(configLuaPath))
            ScriptManager.DoFile(configLuaPath);

        if (IsRunning)
        {
            SendToLogDB(EventId.DBE_SERVER_ON_REQ, new KNetAddress { Ip = PublicIp, Port = _netLayer.Port });
            RefreshChannelList();
            RefreshServerGroupList();
        }

        KLog.Cout($"Switched active runtime region to {Region.Code} ({Region.DisplayName})");
        OnStateChanged?.Invoke();
    }

    public Task AcceptSessionAsync(KTcpSession session)
    {
        session.CheckSequenceNum = _netLayer.CheckSequenceNum;
        session.PacketAuthFailLimit = _netLayer.PacketAuthFailLimit;

        var user = new KChannelUser(session, this);
        session.OnEventReceived += ev => user.QueueingEvent(ev);
        session.OnDisconnected += _ => user.OnDestroy();

        if (!_simLayer.ActorManager.Add(user))
        {
            session.ReserveDestroy();
            return Task.CompletedTask;
        }

        OnStateChanged?.Invoke();
        return session.StartAsync();
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
        try { _tickTask?.Wait(2000); } catch { }
        try { ScriptRefreshManager.StopAsync().GetAwaiter().GetResult(); } catch { }

        SendToLogDB(EventId.DBE_SERVER_OFF_NOT);
        base.ShutDown();
        ServerReady = false;
        OnStateChanged?.Invoke();
    }

    private async Task TickLoopAsync(CancellationToken ct)
    {
        var sw = new Stopwatch();
        while (!ct.IsCancellationRequested)
        {
            sw.Restart();
            try
            {
                Tick();
            }
            catch (Exception ex)
            {
                KLog.Cerr($"KChannelServer Tick error: {ex}");
            }
            sw.Stop();
            int sleepMs = Math.Max(5, 50 - (int)sw.ElapsedMilliseconds);
            try { await Task.Delay(sleepMs, ct).ConfigureAwait(false); }
            catch (OperationCanceledException) { break; }
        }
    }

    public override void Tick()
    {
        TickCount++;
        base.Tick();
        _simLayer.Tick();
        WaitQueueManager.Tick(ServerGroupId, Router, _simLayer.ActorManager);
        MachineBlockManager.Tick(Router);

        var now = DateTime.UtcNow;
        if ((now - _lastChannelRefresh).TotalSeconds >= 60)
        {
            _lastChannelRefresh = now;
            RefreshChannelList();
        }
        if ((now - _lastServerGroupRefresh).TotalMinutes >= 60)
        {
            _lastServerGroupRefresh = now;
            RefreshServerGroupList();
        }
        if ((now - _lastLoginStatFlush).TotalSeconds >= 60)
        {
            _lastLoginStatFlush = now;
            var counts = Statistics.DrainLoginResults();
            if (counts.Any(c => c > 0))
                SendToLogDB(EventId.DBE_INSERT_LOGIN_RESULT_INFO_REQ, new KDBE_INSERT_LOGIN_RESULT_INFO_REQ
                {
                    ServerGroupId = ServerGroupId,
                    ServerUid = Uid,
                    ResultCount = counts,
                });
        }
        if ((now - _lastPerfFlush).TotalMinutes >= 5)
        {
            _lastPerfFlush = now;
            var errStats = KLog.DrainErrorStatistics();
            if (errStats.Count > 0)
            {
                SendToLogDB(EventId.DBE_LOG_STATISTICS_INFO_NOT, new KDBE_LOG_STATISTICS_INFO_NOT
                {
                    ServerUid = Uid,
                    LogStats = errStats,
                });
            }
        }
    }

    public void RefreshChannelList()
        => SendToLogDB(EventId.DBE_CHANNEL_LIST_REQ, new KDBE_CHANNEL_LIST_REQ { ServerGroupId = ServerGroupId });

    private void MergeChannelTemplatesLocked()
    {
        foreach (var tpl in ChannelTemplates.Values)
        {
            if (_channelList.ContainsKey(tpl.ChannelId)) continue;
            _channelList[tpl.ChannelId] = new KChannelInfo
            {
                ServerUid = Uid,
                ChannelId = tpl.ChannelId,
                ChannelName = $"Channel_{tpl.ChannelId}",
                ServerGroupId = ServerGroupId,
                Ip = PublicIp,
                MasterPort = Region.DefaultGamePort,
                NcUdpPort = Region.DefaultUdpRelayPort,
                MaxUser = 1000,
                BonusInfo = new KChannelBonusInfo
                {
                    Enable = tpl.MinLevel > 0 || tpl.MaxLevel > 0,
                    BeginLv = tpl.MinLevel,
                    EndLv = tpl.MaxLevel,
                },
            };
        }
    }

    public void RefreshServerGroupList()
        => SendToLogDB(EventId.DBE_SERVERGROUP_LIST_REQ);

    protected override void ProcessEvent(KEvent ev)
    {
        switch (ev.EventId)
        {
            case EventId.DBE_SERVER_ON_ACK:
            {
                var ack = ev.GetPayload<KDBE_CHANNEL_SERVER_ON_ACK>(Region);
                if (ack.Ok == NetError.NET_OK)
                {
                    SetServerInfo(ack.ServerInfo);
                    lock (_channelLock)
                    {
                        _channelList = ack.ChannelList;
                        MergeChannelTemplatesLocked();
                        _serverGroupList = ack.ServerGroupList;
                    }
                    OnServerReadyComplete();
                    OnStateChanged?.Invoke();
                }
                break;
            }

            case EventId.DBE_CHANNEL_LIST_ACK:
            {
                var ack = ev.GetPayload<KDBE_CHANNEL_LIST_ACK>(Region);
                lock (_channelLock)
                {
                    _channelList = ack.ChannelList;
                    MergeChannelTemplatesLocked();
                }
                OnStateChanged?.Invoke();
                break;
            }

            case EventId.DBE_SERVERGROUP_LIST_ACK:
            {
                var ack = ev.GetPayload<KDBE_SERVERGROUP_LIST_ACK>(Region);
                lock (_channelLock)
                    _serverGroupList = ack.ServerGroupList;
                OnStateChanged?.Invoke();
                break;
            }

            case EventId.DBE_CHECK_HACKING_TOOL_LIST_ACK:
            {
                var ack = ev.GetPayload<KDBE_CHECK_HACKING_TOOL_LIST_ACK>(Region);
                HackingProtectionManager.UpdateHackingToolList(ack.HackingToolList);
                break;
            }

            case EventId.DBE_CHECK_ACCOUNT_BLOCK_LIST_ACK:
            {
                var ack = ev.GetPayload<KDBE_CHECK_ACCOUNT_BLOCK_LIST_ACK>(Region);
                MachineBlockManager.UpdateBlockList(ack.AccountBlockList, ack.MachineIdBlockList);
                break;
            }

            case EventId.DBE_GET_TOTAL_CCU_FOR_WAIT_QUEUE_ACK:
            {
                var ack = ev.GetPayload<KDBE_GET_TOTAL_CCU_FOR_WAIT_QUEUE_ACK>(Region);
                WaitQueueManager.UpdateTotalCCU(ack.TotalCcu);
                break;
            }

            case EventId.ELG_VERIFY_SERVER_CONNECT_ACK:
            {
                var ack = ev.GetPayload<KECN_VERIFY_SERVER_CONNECT_ACK>(Region);
                IsLoginConnected = ack.Ok == NetError.NET_OK;
                break;
            }

            case EventId.ESR_ORDER_TO_REFRESH_MANAGER_REQ:
            case EventId.ESR_ORDER_TO_REFRESH_MANAGER_ACK:
            {
                var order = ev.PayloadBytes.Length > 0
                    ? ev.GetPayload<KESR_SCRIPT_REFRESH_ORDER_NOT>(Region)
                    : new KESR_SCRIPT_REFRESH_ORDER_NOT();
                ScriptRefreshManager.ExecuteRefreshOrder(order.OrderType);
                break;
            }

            default:
                KLog.Cwarn($"[KChannelServer] Unhandled server event: {ev.EventId} ({ev.EventIdValue})");
                break;
        }
    }
}
