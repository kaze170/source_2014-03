using System.Collections.Concurrent;
using System.Diagnostics;
using KncWX2Server.CenterServer;
using KncWX2Server.Core.Database;
using KncWX2Server.Core.Events;
using KncWX2Server.Core.Framework;
using KncWX2Server.Core.Logging;
using KncWX2Server.Core.Network;
using KncWX2Server.Core.Region;
using KncWX2Server.Core.Scripting;
using KncWX2Server.Core.Serialization;
using KncWX2Server.GlobalServer;

namespace KncWX2Server.GameServer;

public sealed class KGSSimLayer : KSimLayer { }

public sealed class KGSNetLayer : KNetLayer
{
    private readonly KGameServer _server;

    public KGSNetLayer(KGameServer server)
    {
        _server = server;
        Port = server.Region.DefaultGamePort;
    }

    public override bool Init()
        => TcpServer.Start(Port, _server.AcceptSessionAsync, () => _server.Region);
}

/// <summary>
/// Managed port of KGameServer (316 C++ files).
/// Integrates TCP client sessions (KGSUser), TCP Relay (KTRServer), UDP Relay (KUdpRelayServer),
/// UDP Echo (KUdpEchoServer), Village/Field zones, and Center/Global coordination.
/// </summary>
public sealed class KGameServer : KBaseServer
{
    private readonly KGSSimLayer _simLayer = new();
    private readonly KGSNetLayer _netLayer;
    private readonly ConcurrentDictionary<long, KGSUser> _byAccountUid = new();
    private readonly ConcurrentDictionary<long, KGSUser> _byUnitUid = new();
    private CancellationTokenSource? _tickCts;
    private Task? _tickTask;

    public SqliteDatabaseManager Db => DbLayer.Sqlite;
    public KCenterServer Center { get; }
    public KGlobalServer Global { get; }
    public KGSFieldManager FieldManager { get; } = new();
    public KStatTable StatTable { get; } = new();
    public KSocketItemManager SocketItemManager { get; } = new();
    public KEnchantItemManager EnchantItemManager { get; } = new();
    public KResolveItemManager ResolveItemManager { get; } = new();
    public KTradeManager TradeManager { get; } = new();
    public KGameEventManager GameEventManager { get; } = new();
    public KStringFilterManager StringFilterManager { get; } = new();
    public KClassChangeTable ClassChangeTable { get; } = new();
    public KStatisticsManager StatisticsManager { get; } = new();
    public KGameSysValTable GameSysValTable { get; } = new();
    public KRewardTable RewardTable { get; } = new();
    public KSpiritTable SpiritTable { get; } = new();
    public KExpTable ExpTable => Db.ExpTable;
    public ConcurrentDictionary<EventId, long> ServerCounters { get; } = new();
    public KScriptRefreshManager ScriptRefreshManager { get; private set; } = null!;
    public KRecommendUserTable RecommendUserTable { get; } = new();
    public KTrainingCenterTable TrainingCenterTable { get; } = new();
    public KGSContentManager ContentManager { get; } = new();
    public KBillManager BillManager { get; } = new();
    public KGSWorldMissionManager WorldMissionManager { get; } = new();
    public KGSGlobalMissionManager GlobalMissionManager { get; } = new();
    public KGSAutoPartyManager AutoPartyManager { get; } = new();
    public KGSRankingManager RankingManager { get; } = new();
    public KTutorialManager TutorialManager { get; } = new();
    public KCompanyIPTable CompanyIpTable { get; } = new();
    public KManufactureItemManager ManufactureItemManager { get; } = new();
    public KRidingPetManager RidingPetManager { get; } = new();
    public KPetManager PetManager { get; } = new();
    public KCorruptionMonitor CorruptionMonitor { get; } = new();
    public KSynthesisManager SynthesisManager { get; } = new();
    public KEdInventoryExpand EdInventoryExpand { get; } = new();
    public KEventDelaySystemManager EventDelaySystemManager { get; } = new();
    public KChannelingUserManager ChannelingUserManager { get; } = new();
    public KCheckSumManager CheckSumManager { get; } = new();
    public KRandomItemManager RandomItemManager { get; } = new();
    public KBuffManager BuffManager { get; } = new();
    public KItemExchangeManager ItemExchangeManager { get; } = new();
    public Dictionary<int, int> ExpandPeriodItems { get; } = new();
    public HashSet<int> DeleteItemIds { get; } = new();
    public HashSet<int> ImpossibleSellItemIds { get; } = new();
    public KDailyGiftBoxManager DailyGiftBoxManager { get; } = new();
    public Dictionary<int, int> DailyGiftBoxPackages => DailyGiftBoxManager.PackageMap;
    public Dictionary<int, (int ClearType, int DungeonId, int Difficulty, int ClearCount)> SubTitleOpenMissions { get; } = new();
    public Dictionary<int, List<int>> ItemConvertGroups { get; } = new();
    public Dictionary<int, (int ConvertType, int CommissionEd, List<int> ResultItemIds)> ItemConvertTable { get; } = new();

    // BingoEventInfo.lua
    public HashSet<int> BingoBanSpots { get; } = new();
    public Dictionary<int, (uint ProductNo, bool IsBanPresent)> BingoPresentInfos { get; } = new();
    public Dictionary<uint, uint> BingoPackageItemPrices { get; } = new();
    public Dictionary<uint, uint> BingoPackageGiftPrices { get; } = new();

    // CashItemData.lua & CashItemPrice.lua & BillingConfig.lua
    public Dictionary<int, HashSet<int>> CashItemServerGroupBlocked { get; } = new();
    public Dictionary<int, HashSet<uint>> CashItemProductNoServerGroupBlocked { get; } = new();
    public Dictionary<int, uint> CashItemPrices { get; } = new();
    public Dictionary<int, int> CashItemGroupMap { get; } = new();
    public Dictionary<int, List<(int ItemId, int Quantity, double Probability)>> CashItemResolveGroups { get; } = new();
    public Dictionary<int, int> BillingProductCategoryOverrides { get; } = new();
    public Dictionary<int, string> ClientScripts { get; } = new();

    public bool TryResolveCashItemAdd(int itemId, out Dictionary<int, int> outputs)
    {
        outputs = new Dictionary<int, int>();
        if (!CashItemGroupMap.TryGetValue(itemId, out int groupId))
            return false;
        if (!CashItemResolveGroups.TryGetValue(groupId, out var entries) || entries.Count == 0)
            return false;

        foreach (var (resItemId, qty, prob) in entries)
        {
            if (resItemId <= 0 || qty <= 0 || prob <= 0.0) continue;
            var lot = new KncWX2Server.Core.Framework.KLottery();
            lot.AddCase(1, prob);
            if (lot.Decision() == 1)
                outputs[resItemId] = outputs.GetValueOrDefault(resItemId) + qty;
        }
        return true;
    }

    // KeepItem.lua
    public Dictionary<int, List<int>> KeepShowItems { get; } = new();
    public Dictionary<int, KncWX2Server.Core.Packets.kDisCountItemInfo> KeepDiscountItems { get; } = new();

    // MapData.lua
    public sealed record MapDataEntry(
        int MapId,
        int BeforeMapId,
        int AfterMapId,
        bool IsDefault,
        int MinLevel,
        int MaxLevel,
        IReadOnlyList<int>? RequireDungeonIds = null);
    public Dictionary<int, MapDataEntry> MapDatas { get; } = new();
    public List<(int LocalMapId, int DungeonMode, int DungeonId)> LocalMapInfos { get; } = new();

    // RoomListData.lua
    public sealed record RoomListTemplate(int RoomListId, int RoomListType, int PvpClass, int MinLevel, int MaxLevel, int MinVp, int MaxVp);
    public Dictionary<int, RoomListTemplate> RoomListTemplates { get; } = new();
    public int RoomListStartPvpId { get; set; } = 3;
    public int RoomListTournamentPvpId { get; set; } = 4;
    public int RoomListOfficialPvpId { get; set; }
    public bool IsLagCheckActive { get; private set; } = true;
    public KncWX2Server.Core.Packets.KEGB_EXCHANGE_LIMIT_INFO_NOT? LatestExchangeLimitInfo { get; private set; }
    public KncWX2Server.Core.Packets.KEGB_TIME_ENCHANT_EVENT_INFO_NOT? LatestTimeEnchantEventInfo { get; private set; }
    public Dictionary<int, int> LatestTodayRandomQuests { get; private set; } = new();
    public Dictionary<string, int> LatestModuleInfos { get; private set; } = new();
    public KncWX2Server.Core.Packets.KEGS_ADMIN_NOTIFY_HERO_PVP_USER_REQ? LatestHeroPvpNotify { get; private set; }

    // ServerMornitoringConfig.lua
    public sealed class MornitoringConfigState
    {
        public bool SendSms { get; set; }
        public bool SendMail { get; set; }
        public bool CheckX2Module { get; set; }
        public List<int> DumpServerUids { get; } = [];
        public List<string> PhoneNumbers { get; } = [];
        public List<string> ServerNetworkMailAddrs { get; } = [];
        public List<string> ServerDumpMailAddrs { get; } = [];
        public List<string> CheckModuleList { get; } = [];
    }
    public MornitoringConfigState MornitoringConfig { get; } = new();

    // Server-level synchronized registries (matching C++ KGameServer / KRoomListManager / KPartyListManager)
    public ConcurrentDictionary<long, KncWX2Server.Core.Packets.KServerInfo> ConnectedServers { get; } = new();
    public ConcurrentDictionary<long, KncWX2Server.Core.Packets.KPartyListInfo> PartyListRegistry { get; } = new();
    public ConcurrentDictionary<long, List<KncWX2Server.Core.Packets.KPartyMemberInfo>> PartyMemberRegistry { get; } = new();
    public ConcurrentDictionary<long, KncWX2Server.Core.Packets.KRoomInfo> RoomListRegistry { get; } = new();
    public ConcurrentDictionary<long, KncWX2Server.Core.Packets.KRoomInfo> BattleFieldRoomRegistry { get; } = new();
    public ConcurrentDictionary<long, KncWX2Server.Core.Packets.KSquareInfo> SquareListRegistry { get; } = new();
    public ConcurrentDictionary<int, KncWX2Server.Core.Packets.KWeddingHallInfo> WeddingHallRegistry { get; } = new();
    public ConcurrentDictionary<long, KncWX2Server.Core.Packets.KERM_PERSONAL_SHOP_INFO_NOT> PersonalShopRegistry { get; } = new();
    public ConcurrentDictionary<long, KncWX2Server.Core.Packets.KUserListView> PvpLobbyUsers { get; } = new();
    public ConcurrentDictionary<int, KncWX2Server.Core.Packets.KERM_UPDATE_FIELD_INFO_NOT> FieldStatistics { get; } = new();
    public int PvpRoomCount { get; private set; }
    public int DungeonRoomCount { get; private set; }
    public int BattleFieldRoomCount { get; private set; }
    public int TotalFieldDangerousValue { get; private set; }
    public bool IsBillingEnabled { get; private set; } = true;
    public int BillingReleaseTick { get; private set; }
    public bool IsWebRankingRefreshNeeded { get; private set; }
    public bool IsLocalRankingEnabled { get; private set; } = true;

    public bool TryGetItemConvertResult(int targetItemId, out int resultItemId, out int commissionEd)
    {
        resultItemId = 0;
        commissionEd = 0;
        if (!ItemConvertTable.TryGetValue(targetItemId, out var entry) || entry.ResultItemIds.Count == 0)
            return false;
        resultItemId = entry.ResultItemIds[Random.Shared.Next(entry.ResultItemIds.Count)];
        commissionEd = entry.CommissionEd;
        return resultItemId > 0;
    }

    public static bool IsVillageMapId(int mapId) => mapId is >= 20000 and < 20008;

    public int CheckMapRequireData(
        int mapId,
        byte level,
        IReadOnlyDictionary<int, KncWX2Server.Core.Packets.KDungeonClearInfo> dungeonClear)
    {
        if (!MapDatas.TryGetValue(mapId, out var entry))
            return MapDatas.Count == 0 ? KncWX2Server.Core.Region.NetError.NET_OK : KncWX2Server.Core.Region.NetError.ERR_FIELD_00;

        if (entry.MinLevel > 0 && level < entry.MinLevel)
            return KncWX2Server.Core.Region.NetError.ERR_FIELD_05;

        if (entry.RequireDungeonIds is { Count: > 0 } &&
            entry.RequireDungeonIds.Any(reqDungeonId => !dungeonClear.ContainsKey(reqDungeonId)))
            return KncWX2Server.Core.Region.NetError.ERR_FIELD_06;

        return KncWX2Server.Core.Region.NetError.NET_OK;
    }

    public int GetPossibleEnterVillageMapId(
        byte level,
        IReadOnlyDictionary<int, KncWX2Server.Core.Packets.KDungeonClearInfo> dungeonClear,
        int fallbackMapId = 20000)
    {
        int defaultMapId = MapDatas.Values.FirstOrDefault(m => m.IsDefault)?.MapId ?? fallbackMapId;
        int possibleMapId = defaultMapId;
        foreach (var kv in MapDatas.OrderBy(k => k.Key))
        {
            if (!IsVillageMapId(kv.Key)) continue;
            if (CheckMapRequireData(kv.Key, level, dungeonClear) == KncWX2Server.Core.Region.NetError.NET_OK)
                possibleMapId = kv.Key;
        }
        return possibleMapId;
    }
    public KncWX2Server.Core.Packets.KEGB_GET_MISSION_INFO_ACK? LatestWorldMissionSnapshot { get; private set; }
    public KncWX2Server.Core.Packets.KEGB_GET_GLOBAL_MISSION_INFO_ACK? LatestGlobalMissionSnapshot { get; private set; }
    public string WorldMissionStartTime { get; private set; } = string.Empty;
    public string GlobalMissionStartTime { get; private set; } = string.Empty;
    public ConcurrentDictionary<long, KncWX2Server.Core.Packets.KELG_LOCAL_RANKING_CHANGED_RANKER_INFO_NOT> LocalRankingRankerCache { get; } = new();
    public KGSFieldBossManager FieldBossManager { get; } = new();
    public KncWX2Server.Core.Packets.KEGB_BATTLE_FIELD_BOSS_INFO_NOT? LatestFieldBossInfo { get; private set; }
    public KncWX2Server.Core.Packets.KELG_GET_RANKING_FOR_GS_RANKING_MANAGER_ACK? LatestHenirRankingSnapshot { get; private set; }
    public List<KncWX2Server.Core.Packets.KAutoPartyBonusInfo> LatestAutoPartyBonusInfos { get; private set; } = [];
    private readonly ConcurrentDictionary<sbyte, int> _disconnectReports = new();
    public int GetDisconnectReportCount(sbyte reportType) => _disconnectReports.TryGetValue(reportType, out int c) ? c : 0;
    public Func<sbyte, int, (List<KncWX2Server.Core.Packets.KGuildAdInfo> ads, int totalPages)>? GuildAdListProvider { get; set; }
    private readonly ConcurrentDictionary<int, KncWX2Server.Core.Packets.KWeddingHallInfo> _weddingHalls = new();
    public Func<int, KncWX2Server.Core.Packets.KWeddingHallInfo?>? WeddingHallGetProvider { get; set; }
    public Func<KncWX2Server.Core.Packets.KWeddingHallInfo, bool>? WeddingHallAddProvider { get; set; }
    public Func<int, long, bool>? WeddingHallRoomUpdateProvider { get; set; }
    private readonly ConcurrentDictionary<long, uint> _cashBalances = new();
    private readonly ConcurrentDictionary<long, KncWX2Server.Core.Packets.KChannelChangeInfo> _channelChangeUsers = new();
    private readonly ConcurrentDictionary<string, KncWX2Server.Core.Packets.KHeroPvpUserInfo> _heroPvpUsers = new(StringComparer.OrdinalIgnoreCase);
    public bool ActiveLagCheck { get; set; }
    public float DamageMultiplier { get; set; } = 1.0f;
    public bool UdpSetKick { get; set; }
    public bool BonusCashEventActive { get; set; }
    public bool IsGlobalConnected { get; private set; }
    public bool IsCenterConnected { get; private set; }
    public bool IsLoginConnected { get; private set; }
    public long LoginServerUid { get; set; }
    public string MainVersion { get; set; } = string.Empty;
    public KTRServer TrServer { get; }
    public KUdpRelayServer UdpRelay { get; } = new();
    public KUdpEchoServer UdpEcho { get; }

    public override uint PerformerTypeId => PerformerId.PI_GS_SERVER;
    public override KSimLayer SimLayer => _simLayer;
    public override KNetLayer NetLayer => _netLayer;

    public ushort ListenPort => _netLayer.Port;
    public ushort TrPort { get; private set; }
    public ushort UdpEchoPort { get; private set; }
    public ushort UdpRelayPort { get; private set; }
    public bool IsRunning { get; private set; }
    public int ConnectedUserCount => _simLayer.ActorManager.Count;
    public int ActiveUnitCount => _byUnitUid.Count;

    public KGameServer(
        RegionCode initialRegion = RegionCode.TW,
        string dbPath = "KncWX2Server.db",
        InProcessRouter? sharedRouter = null,
        KCenterServer? centerServer = null,
        KGlobalServer? globalServer = null)
        : base(sharedRouter)
    {
        Region = RegionProfile.FromCode(initialRegion);
        _simLayer.NationFlag = Region.DefaultNationFlag;
        _netLayer = new KGSNetLayer(this);
        DbLayer.Initialize(dbPath);

        Center = centerServer ?? new KCenterServer(initialRegion, Router);
        Global = globalServer ?? new KGlobalServer(initialRegion, Router);
        TrServer = new KTRServer(Region);
        UdpEcho = new KUdpEchoServer(Router, PerformerId.PI_GS_SERVER);
        ScriptRefreshManager = new KScriptRefreshManager(Router, PerformerId.PI_GS_SERVER, Uid);
        ScriptRefreshManager.SetDefaultReloadHandler(ot => KServerResourceLoader.LoadOrder(this, ot));
    }

    public bool Start(
        ushort? overridePort = null,
        ushort? overrideTrPort = null,
        ushort? overrideUdpEchoPort = null,
        ushort? overrideUdpRelayPort = null,
        int serverGroupId = 0)
    {
        if (IsRunning) return true;
        ServerGroupId = serverGroupId;
        Uid = KncUid.MakeUid(Region, serverGroupId, 200);
        Name = "KGameServer";

        Router.RegisterPerformer(PerformerId.PI_GS_SERVER, this);
        Router.RegisterPerformer(PerformerId.PI_GS_KOG_BILLING_MANAGER, BillManager);
        Router.RegisterRoute(PerformerId.PI_GS_USER, ev =>
        {
            foreach (long uid in ev.DestPerformer.Uids)
            {
                if (_simLayer.ActorManager.QueueingTo(uid, ev.Clone())) continue;
                if (_byUnitUid.TryGetValue(uid, out var byUnit)) byUnit.QueueingEvent(ev.Clone());
                else if (_byAccountUid.TryGetValue(uid, out var byAcct)) byAcct.QueueingEvent(ev.Clone());
            }
            return true;
        });
        Router.RegisterRoute(PerformerId.PI_GS_CHARACTER, ev =>
        {
            foreach (long uid in ev.DestPerformer.Uids)
            {
                if (_byUnitUid.TryGetValue(uid, out var byUnit)) byUnit.QueueingEvent(ev.Clone());
            }
            return true;
        });
        Router.RegisterRoute(PerformerId.PI_GS_PROXY, ev =>
        {
            foreach (long uid in ev.DestPerformer.Uids)
            {
                if (_byAccountUid.TryGetValue(uid, out var user) ||
                    _byUnitUid.TryGetValue(uid, out user))
                {
                    _ = user.Session.SendEventAsync(ev);
                }
            }
            return true;
        });
        Router.RegisterRoute(PerformerId.PI_GS_FIELD, ev =>
        {
            foreach (long fuid in ev.DestPerformer.Uids)
            {
                var zone = FieldManager.GetZoneByFieldUid(fuid) ?? FieldManager.GetOrCreateZone((int)fuid);
                zone.ProcessEvent(ev.Clone(), this);
            }
            return true;
        });

        UdpRelayPort = UdpRelay.Start(overrideUdpRelayPort ?? Region.DefaultUdpRelayPort);
        Center.RoomManager.DefaultUdpRelayPort = UdpRelayPort;
        Center.RoomManager.DefaultUdpRelayIp = PublicIp;
        UdpEchoPort = UdpEcho.Start(overrideUdpEchoPort ?? Region.DefaultUdpEchoPort);
        TrPort = TrServer.Start(overrideTrPort ?? Region.DefaultTrPort);

        _netLayer.Port = overridePort ?? Region.DefaultGamePort;
        RegToLua();
        if (!_netLayer.Init()) return false;
        _netLayer.Port = _netLayer.TcpServer.Port;

        IsRunning = true;
        ServerReady = true;
        ScriptRefreshManager.Start();
        _tickCts = new CancellationTokenSource();
        _tickTask = Task.Run(() => TickLoopAsync(_tickCts.Token));
        KLog.Cout($"[{Name}] Started TCP:{ListenPort}, TR:{TrPort}, UdpEcho:{UdpEchoPort}, UdpRelay:{UdpRelayPort} (Region={Region.Code})");
        return true;
    }

    public override void RegToLua()
    {
        base.RegToLua();
        ScriptManager.RegisterGlobal("TRServer", TrServer);
        ScriptManager.RegisterGlobal("BillManager", BillManager);
        ScriptManager.RegisterGlobal("StringFilterManager", StringFilterManager);
        ScriptManager.RegisterGlobal("GameSysVal", GameSysValTable);
        ScriptRefreshManager.BindToLua(ScriptManager, "ScriptRefresh");
    }

    public void SwitchRegion(RegionCode region)
    {
        Region = RegionProfile.FromCode(region);
        _simLayer.NationFlag = Region.DefaultNationFlag;
        TrServer.Region = Region;
        Center.SwitchRegion(region);
        Global.SwitchRegion(region);
    }

    public int LoadServerResources(string? resourceBaseDir = null, bool includeHeavyTables = false)
        => KServerResourceLoader.LoadAll(this, resourceBaseDir, includeHeavyTables);

    public int ReloadScriptOrder(int orderType, string? resourceBaseDir = null)
        => KServerResourceLoader.LoadOrder(this, orderType, resourceBaseDir);

    public Task AcceptSessionAsync(KTcpSession session)
    {
        var user = new KGSUser(session, this);
        session.OnEventReceived += ev => user.QueueingEvent(ev);
        session.OnDisconnected += _ => user.OnDestroy();
        if (!_simLayer.ActorManager.Add(user))
        {
            session.ReserveDestroy();
            return Task.CompletedTask;
        }
        return session.StartAsync();
    }

    internal void RegisterAccountSession(KGSUser user)
    {
        if (user.AccountUid > 0)
            _byAccountUid[user.AccountUid] = user;
    }

    internal void RegisterActiveUnit(KGSUser user)
    {
        if (user.ActiveUnit != null)
        {
            _byUnitUid[user.ActiveUnit.UnitUid] = user;
            if (Router.HasRoute(PerformerId.PI_LOGIN_SERVER))
            {
                Router.Send(PerformerId.PI_GS_SERVER, Uid, PerformerId.PI_LOGIN_SERVER, 0,
                    EventId.ELG_REGISTER_UNIT_NOT,
                    new KncWX2Server.Core.Packets.KELG_REGISTER_UNIT_NOT
                    {
                        UserUid = user.AccountUid,
                        UnitUid = user.ActiveUnit.UnitUid,
                        NickName = user.ActiveUnit.NickName,
                        GuildUid = user.ActiveUnit.UserGuildInfo.GuildUid,
                        GameServerUid = Uid,
                    },
                    trace: [user.Uid]);
            }
        }
    }

    internal void OnUserDisconnected(KGSUser user)
    {
        if (user.AccountUid > 0)
            _byAccountUid.TryRemove(user.AccountUid, out _);
        if (user.ActiveUnit != null)
        {
            _byUnitUid.TryRemove(user.ActiveUnit.UnitUid, out _);
            if (Router.HasRoute(PerformerId.PI_LOGIN_SERVER))
            {
                Router.Send(PerformerId.PI_GS_SERVER, Uid, PerformerId.PI_LOGIN_SERVER, 0,
                    EventId.ELG_UNIT_DISCONNECT_NOT,
                    new KncWX2Server.Core.Packets.KELG_UNIT_DISCONNECT_NOT
                    {
                        UnitUid = user.ActiveUnit.UnitUid,
                        GuildUid = user.ActiveUnit.UserGuildInfo.GuildUid,
                    });
            }
        }
    }

    public void SendToUnit(long unitUid, EventId eventId, IKSerializable packet)
    {
        if (_byUnitUid.TryGetValue(unitUid, out var user))
            user.SendPacket(eventId, packet);
    }

    public KGSUser? GetUserByUnitUid(long unitUid)
        => _byUnitUid.TryGetValue(unitUid, out var user) ? user : null;

    public KGSUser? GetUserByNickName(string nickName)
        => _byUnitUid.Values.FirstOrDefault(u => u.ActiveUnit != null &&
            string.Equals(u.ActiveUnit.NickName, nickName, StringComparison.OrdinalIgnoreCase));

    public IReadOnlyList<KGSUser> GetActiveUsersSnapshot()
        => _byUnitUid.Values.Where(u => u.ActiveUnit != null).ToArray();

    public uint GetAccountCash(long accountUid)
        => _cashBalances.GetOrAdd(accountUid, uid => Db.GetAccountCashBalance(uid));

    public bool TrySpendAccountCash(long accountUid, uint cost, out uint remaining)
    {
        var (ok, rem) = Db.TrySpendAccountCash(accountUid, cost);
        remaining = rem;
        _cashBalances[accountUid] = rem;
        return ok;
    }

    public void AddAccountCash(long accountUid, uint amount)
    {
        uint updated = Db.AddAccountCash(accountUid, amount);
        _cashBalances[accountUid] = updated;
    }

    public KncWX2Server.Core.Packets.KWeddingHallInfo? GetWeddingHall(int weddingUid)
        => WeddingHallGetProvider != null
            ? WeddingHallGetProvider(weddingUid)
            : (_weddingHalls.TryGetValue(weddingUid, out var h) ? h : null);

    public bool RegisterWeddingHall(KncWX2Server.Core.Packets.KWeddingHallInfo info)
    {
        _weddingHalls[info.WeddingUid] = info;
        return WeddingHallAddProvider == null || WeddingHallAddProvider(info);
    }

    public bool BindWeddingHallRoom(int weddingUid, long roomUid)
    {
        if (_weddingHalls.TryGetValue(weddingUid, out var h))
            h.RoomUid = roomUid;
        return WeddingHallRoomUpdateProvider == null || WeddingHallRoomUpdateProvider(weddingUid, roomUid);
    }

    public KncWX2Server.Core.Packets.KWeddingHallInfo? GetWeddingHallByRoom(long roomUid)
        => roomUid > 0
            ? _weddingHalls.Values.FirstOrDefault(h => h.RoomUid == roomUid)
            : null;

    public void RegisterChannelChangeReservation(long unitUid, KncWX2Server.Core.Packets.KChannelChangeInfo info)
    {
        if (unitUid > 0)
            _channelChangeUsers[unitUid] = info;
    }

    public bool TryConsumeChannelChangeReservation(long unitUid, out KncWX2Server.Core.Packets.KChannelChangeInfo info)
        => _channelChangeUsers.TryRemove(unitUid, out info!);

    public KGSUser? GetUserByUserId(string userId)
        => _byAccountUid.Values.FirstOrDefault(u =>
            string.Equals(u.UserId, userId, StringComparison.OrdinalIgnoreCase));

    public void BroadcastNotice(sbyte notifyType, sbyte count, string message, int randomItem = 0)
    {
        var pkt = new KncWX2Server.Core.Packets.KEGS_NOTIFY_MSG_REQ
        {
            NotifyType = notifyType,
            Count = count,
            Msg = message,
            RandomItem = randomItem,
        };
        foreach (var u in _byAccountUid.Values)
            u.SendPacket(EventId.EGS_NOTIFY_MSG_NOT, pkt);
    }

    public bool KickUserByAdmin(string nameOrId, bool isUserId)
    {
        var target = isUserId ? GetUserByUserId(nameOrId) : GetUserByNickName(nameOrId);
        if (target == null) return false;
        target.OnDestroy();
        return true;
    }

    public (int ccu, Dictionary<int, int> pvpRooms, int curPartyCount, int playGamePartyCount) GetServerStatusSnapshot()
    {
        var pvpRooms = new Dictionary<int, int>();
        foreach (var r in Center.RoomManager.GetRoomsSnapshot())
        {
            if (r.Info.RoomType == 0)
                pvpRooms[r.Info.PvpGameType] = pvpRooms.GetValueOrDefault(r.Info.PvpGameType, 0) + 1;
        }
        var parties = Center.PartyManager.GetPartiesSnapshot();
        int playCount = parties.Count(p => p.Fsm.CurrentStateId == KCnPartyFSM.S_PREPARE_FOR_PLAY || p.Fsm.CurrentStateId == KCnPartyFSM.S_ACCEPT_FOR_PLAY);
        return (_byAccountUid.Count, pvpRooms, parties.Count, playCount);
    }

    public List<KncWX2Server.Core.Packets.KHeroPvpUserInfo> GetHeroPvpUsers()
        => _heroPvpUsers.Values.ToList();

    public void AddHeroPvpUsers(IEnumerable<(long ServerGroupId, string NickName)> list)
    {
        foreach (var (sg, nick) in list)
        {
            if (string.IsNullOrWhiteSpace(nick)) continue;
            bool conn = GetUserByNickName(nick) != null;
            _heroPvpUsers[nick] = new KncWX2Server.Core.Packets.KHeroPvpUserInfo
            {
                ServerGroupId = sg,
                NickName = nick,
                On = true,
                Connect = conn,
            };
        }
    }

    public void RemoveHeroPvpUsers(IEnumerable<(long ServerGroupId, string NickName)> list)
    {
        foreach (var (_, nick) in list)
            _heroPvpUsers.TryRemove(nick, out _);
    }

    public KGSUser? GetOnlineUserByUnitUid(long unitUid) =>
        _byUnitUid.TryGetValue(unitUid, out var user) ? user : null;

    public int BroadcastMegaphone(bool allServers, KncWX2Server.Core.Packets.KEGS_MEGAPHONE_MESSAGE_NOT notPacket)
    {
        int sent = 0;
        foreach (var user in _byUnitUid.Values)
        {
            user.SendPacket(EventId.EGS_MEGAPHONE_MESSAGE_NOT, notPacket);
            sent++;
        }
        return sent;
    }

    public IReadOnlyDictionary<int, int> GetManufacturePeriodGroups() => new Dictionary<int, int>
    {
        [1] = 30,
        [2] = 15,
        [3] = 7,
    };

    public void PopulateRankingInfo(
        KncWX2Server.Core.Packets.KEGS_GET_RANKING_INFO_REQ req,
        long requesterUnitUid,
        KncWX2Server.Core.Packets.KEGS_GET_RANKING_INFO_ACK ack)
    {
        const int pageSize = 10;
        ack.RankingType = req.RankingType;
        ack.ViewPage = Math.Max(1u, req.ViewPage);

        if (req.RankingType is KGSRankingManager.RtDayRanking
            or KGSRankingManager.RtWeekRanking
            or KGSRankingManager.RtMonthRanking
            or KGSRankingManager.RtHeroRanking)
        {
            if (RankingManager.HasHenirRanking(req.RankingType))
            {
                RankingManager.GetRankingInfo(req, requesterUnitUid, ack);
                return;
            }

            var dbHenir = Db.GetHenirRankings(req.RankingType);
            if (dbHenir.Count > 0)
            {
                ack.TotalPage = Math.Max(1u, (uint)((dbHenir.Count + pageSize - 1) / pageSize));
                int skip = ((int)ack.ViewPage - 1) * pageSize;
                int rank = skip + 1;
                foreach (var r in dbHenir.Skip(skip).Take(pageSize))
                {
                    var row = new KncWX2Server.Core.Packets.KHenirRankingInfo
                    {
                        Rank = rank++,
                        StageCount = r.stageCount,
                        PlayTime = (uint)Math.Max(0, r.clearTimeSec),
                        RegDate = new DateTimeOffset(r.recordedUtc).ToUnixTimeSeconds(),
                        UnitUid = r.unitUid,
                        NickName = r.nickName,
                        UnitClass = (sbyte)r.unitClass,
                        Level = r.level,
                    };
                    ack.HenirRankingPage.Add(row);
                    if (r.unitUid == requesterUnitUid) ack.MyHenirRanking.Add(row);
                }
                return;
            }

            var lvls = Db.GetTopLevelRankings(Region, 100);
            ack.TotalPage = Math.Max(1u, (uint)((lvls.Count + pageSize - 1) / pageSize));
            int lvlSkip = ((int)ack.ViewPage - 1) * pageSize;
            int lvlRank = lvlSkip + 1;
            foreach (var r in lvls.Skip(lvlSkip).Take(pageSize))
            {
                var u = Db.GetUnitByUid(Region, r.UnitUid);
                var row = new KncWX2Server.Core.Packets.KHenirRankingInfo
                {
                    Rank = lvlRank++,
                    StageCount = Math.Max(1, r.Level / 2),
                    PlayTime = 180,
                    RegDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    UnitUid = r.UnitUid,
                    NickName = r.NickName,
                    UnitClass = u?.UnitClass ?? 1,
                    Level = (byte)Math.Clamp(r.Level, 1, 99),
                };
                ack.HenirRankingPage.Add(row);
                if (r.UnitUid == requesterUnitUid) ack.MyHenirRanking.Add(row);
            }
        }
        else if (req.RankingType == KGSRankingManager.RtDungeonRanking)
        {
            if (RankingManager.HasDungeonRanking)
            {
                RankingManager.GetRankingInfo(req, requesterUnitUid, ack);
                return;
            }

            var lvls = Db.GetTopLevelRankings(Region, 100);
            ack.TotalPage = Math.Max(1u, (uint)((lvls.Count + pageSize - 1) / pageSize));
            int skip = ((int)ack.ViewPage - 1) * pageSize;
            int rank = skip + 1;
            foreach (var r in lvls.Skip(skip).Take(pageSize))
            {
                var u = Db.GetUnitByUid(Region, r.UnitUid);
                var row = new KncWX2Server.Core.Packets.KDungeonRankingInfo
                {
                    Rank = rank++,
                    UnitUid = r.UnitUid,
                    NickName = r.NickName,
                    UnitClass = u?.UnitClass ?? 1,
                    Level = (byte)Math.Clamp(r.Level, 1, 99),
                    Exp = r.Exp,
                };
                ack.DungeonRankingPage.Add(row);
                if (r.UnitUid == requesterUnitUid) ack.MyDungeonRanking.Add(row);
            }
        }
        else if (req.RankingType == KGSRankingManager.RtPvpRanking)
        {
            if (RankingManager.HasPvpRanking)
            {
                RankingManager.GetRankingInfo(req, requesterUnitUid, ack);
                return;
            }

            var pvps = Db.GetTopPvpRankings(Region, 100);
            ack.TotalPage = Math.Max(1u, (uint)((pvps.Count + pageSize - 1) / pageSize));
            int skip = ((int)ack.ViewPage - 1) * pageSize;
            int rank = skip + 1;
            foreach (var r in pvps.Skip(skip).Take(pageSize))
            {
                var u = Db.GetUnitByUid(Region, r.UnitUid);
                var row = new KncWX2Server.Core.Packets.KPvpRankingInfo
                {
                    Rank = rank++,
                    UnitUid = r.UnitUid,
                    NickName = r.NickName,
                    UnitClass = u?.UnitClass ?? 1,
                    Level = u?.Level ?? 1,
                    PvpRank = u?.Rank ?? 0,
                    RPoint = r.RPoint,
                    Win = r.Win,
                };
                ack.PvpRankingPage.Add(row);
                if (r.UnitUid == requesterUnitUid) ack.MyPvpRanking.Add(row);
            }
        }
        else
        {
            RankingManager.GetRankingInfo(req, requesterUnitUid, ack);
        }
    }

    public static IReadOnlyList<KncWX2Server.Core.Packets.KBillProductInfo> GetDefaultBillingProducts() =>
    [
        new() { ProductNo = 1001, ProductId = 130001, ProductName = "Resurrection Stone (10)", CategoryNo = 1, SalePrice = 100, RealPrice = 100, Quantity = 10, Sale = true, Show = true, EnableGift = true },
        new() { ProductNo = 1002, ProductId = 130048, ProductName = "Ice Burner", CategoryNo = 2, SalePrice = 150, RealPrice = 150, Quantity = 1, Sale = true, Show = true, EnableGift = true },
        new() { ProductNo = 1003, ProductId = 130020, ProductName = "Skill Note (1)", CategoryNo = 3, SalePrice = 200, RealPrice = 200, Quantity = 1, Sale = true, Show = true, EnableGift = true },
        new() { ProductNo = 1004, ProductId = 130050, ProductName = "Gnosis Blessing (30 Days)", CategoryNo = 3, SalePrice = 300, RealPrice = 300, Period = 30, Quantity = 1, Sale = true, Show = true, EnableGift = true },
        new() { ProductNo = 1005, ProductId = 130060, ProductName = "Warp VIP (30 Days)", CategoryNo = 4, SalePrice = 250, RealPrice = 250, Period = 30, Quantity = 1, Sale = true, Show = true, EnableGift = true },
    ];

    public List<KncWX2Server.Core.Packets.KBillProductInfo> GetFilteredBillingProducts(int serverGroupId)
    {
        CashItemServerGroupBlocked.TryGetValue(serverGroupId, out var blockedItems);
        CashItemProductNoServerGroupBlocked.TryGetValue(serverGroupId, out var blockedProducts);

        var list = new List<KncWX2Server.Core.Packets.KBillProductInfo>();
        foreach (var src in GetDefaultBillingProducts())
        {
            if (blockedItems != null && blockedItems.Contains(src.ProductId))
                continue;
            if (blockedProducts != null && blockedProducts.Contains((uint)src.ProductNo))
                continue;

            int price = CashItemPrices.TryGetValue(src.ProductId, out uint overridePrice)
                ? (int)overridePrice
                : src.SalePrice;
            int categoryNo = BillingProductCategoryOverrides.TryGetValue(src.ProductId, out int overrideCat)
                ? overrideCat
                : src.CategoryNo;

            list.Add(new KncWX2Server.Core.Packets.KBillProductInfo
            {
                ProductNo = src.ProductNo,
                ProductId = src.ProductId,
                ProductName = src.ProductName,
                CategoryNo = (sbyte)Math.Clamp(categoryNo, sbyte.MinValue, sbyte.MaxValue),
                SalePrice = price,
                RealPrice = price,
                Period = src.Period,
                Quantity = src.Quantity,
                Sale = true,
                Show = src.Show,
                EnableGift = src.EnableGift,
            });
        }
        return list;
    }

    public List<KncWX2Server.Core.Packets.KBillProductInfo> GetBillProductPage(int serverGroupId, int page, out bool isFinal, int pageSize = 800)
    {
        if (BillManager.GetBillProductInfo().Count == 0)
        {
            var defaultMap = GetFilteredBillingProducts(serverGroupId).ToDictionary(p => p.ProductNo, p => p);
            BillManager.PageSize = pageSize;
            BillManager.SetBillProductInfo(defaultMap);
            BillManager.SetBillProductPage();
        }
        if (page == 0)
        {
            BillManager.GetBillProductPage(1, out var first, out isFinal);
            return first;
        }
        if (BillManager.GetBillProductPage(page, out var paged, out isFinal))
            return paged;

        isFinal = true;
        return [];
    }

    private async Task TickLoopAsync(CancellationToken ct)
    {
        var sw = new Stopwatch();
        while (!ct.IsCancellationRequested)
        {
            sw.Restart();
            try { Tick(); } catch (Exception ex) { KLog.Cerr($"KGameServer Tick error: {ex.Message}"); }
            sw.Stop();
            try { await Task.Delay(Math.Max(5, 50 - (int)sw.ElapsedMilliseconds), ct); }
            catch (OperationCanceledException) { break; }
        }
    }

    public override void Tick()
    {
        base.Tick();
        _simLayer.Tick();
        BillManager.Tick();
        EventDelaySystemManager.Tick(
            GameSysValTable.IsEnableOpenRandomCubeDelaySystem,
            GameSysValTable.OpenRandomCubeDelayTime,
            GameSysValTable.OpenRandomCubeDelayCount,
            (unitUid, req) => Router.Send(
                PerformerId.PI_GS_SERVER, Uid,
                PerformerId.PI_GS_CHARACTER, unitUid,
                EventId.EGS_OPEN_RANDOM_ITEM_REQ, req));
        CheckFieldBossSystem(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
    }

    public void CheckFieldBossSystem(long currentUnixSeconds)
    {
        if (!FieldBossManager.CheckTerm(currentUnixSeconds))
            return;
        if (FieldBossManager.IsConnected)
            return;

        long appearTime = FieldBossManager.PortalAppearanceTime;
        if (FieldBossManager.IsPortalOpen && appearTime + FieldBossManager.RemainPortalTime <= currentUnixSeconds)
        {
            FieldBossManager.ClosePortal();
            var closeNot = new KncWX2Server.Core.Packets.KEGS_BATTLE_FIELD_BOSS_GATE_CLOSE_NOT
            {
                BattleFieldId = FieldBossManager.PortalAppearanceMap,
            };
            foreach (var u in _simLayer.ActorManager.GetAllActors().OfType<KGSUser>())
                if (u.ActiveUnit != null)
                    u.SendPacket(EventId.EGS_BATTLE_FIELD_BOSS_GATE_CLOSE_NOT, closeNot);
        }

        if (FieldBossManager.IsCoolTime && appearTime + FieldBossManager.RemainCoolTime <= currentUnixSeconds)
            FieldBossManager.SetCoolTimeState(false);
    }

    protected override void ProcessEvent(KEvent ev)
    {
        switch (ev.EventId)
        {
            case EventId.EGB_GET_MISSION_INFO_ACK:
            {
                var ack = ev.GetPayload<KncWX2Server.Core.Packets.KEGB_GET_MISSION_INFO_ACK>(Region);
                LatestWorldMissionSnapshot = ack;
                WorldMissionManager.UpdateMissionInfo(ack);
                break;
            }

            case EventId.EGB_GET_GLOBAL_MISSION_INFO_ACK:
            {
                var ack = ev.GetPayload<KncWX2Server.Core.Packets.KEGB_GET_GLOBAL_MISSION_INFO_ACK>(Region);
                LatestGlobalMissionSnapshot = ack;
                GlobalMissionManager.UpdateGlobalMissionInfo(ack);
                break;
            }

            case EventId.ELG_CREATE_GUILD_ACK:
            {
                var ack = ev.GetPayload<KncWX2Server.Core.Packets.KELG_CREATE_GUILD_ACK>(Region);
                if (ack.MasterUnitUid > 0)
                {
                    SendToUnit(ack.MasterUnitUid, EventId.EGS_CREATE_GUILD_ACK,
                        new KncWX2Server.Core.Packets.KEGS_CREATE_GUILD_ACK
                        {
                            Ok = ack.Ok,
                            GuildInfo = ack.GuildInfo,
                        });
                }
                break;
            }

            case EventId.EGB_VERIFY_SERVER_CONNECT_ACK:
            {
                var ack = ev.GetPayload<KncWX2Server.Core.Packets.KPacketOK>(Region);
                IsGlobalConnected = ack.Ok == KncWX2Server.Core.Region.NetError.NET_OK;
                break;
            }

            case EventId.ECN_VERIFY_SERVER_CONNECT_ACK:
            {
                var ack = ev.GetPayload<KncWX2Server.Core.Packets.KECN_VERIFY_SERVER_CONNECT_ACK>(Region);
                IsCenterConnected = ack.Ok == KncWX2Server.Core.Region.NetError.NET_OK;
                break;
            }

            case EventId.ELG_GET_RANKING_FOR_GS_RANKING_MANAGER_ACK:
            {
                var ack = ev.GetPayload<KncWX2Server.Core.Packets.KELG_GET_RANKING_FOR_GS_RANKING_MANAGER_ACK>(Region);
                LatestHenirRankingSnapshot = ack;
                RankingManager.UpdateHenirRanking(ack.HenirRanking, ack.LastRank);
                break;
            }

            case EventId.EGB_BATTLE_FIELD_BOSS_INFO_NOT:
            {
                var not = ev.GetPayload<KncWX2Server.Core.Packets.KEGB_BATTLE_FIELD_BOSS_INFO_NOT>(Region);
                LatestFieldBossInfo = not;
                FieldBossManager.ReservedTimeForInitRequest = 0;
                bool wasPortalOpen = FieldBossManager.IsPortalOpen;
                FieldBossManager.SetInfo(
                    DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    not.RemainPortalTime,
                    not.RemainCoolTime,
                    not.PortalAppearanceMap,
                    not.PortalDestination,
                    not.PortalOpen,
                    not.CoolTime);

                if (wasPortalOpen != not.PortalOpen)
                {
                    if (not.PortalOpen)
                    {
                        var openNot = new KncWX2Server.Core.Packets.KEGS_BATTLE_FIELD_BOSS_GATE_OPEN_NOT
                        {
                            BattleFieldId = not.PortalAppearanceMap,
                            BossFieldId = not.PortalDestination,
                        };
                        foreach (var u in _simLayer.ActorManager.GetAllActors().OfType<KGSUser>())
                            if (u.ActiveUnit != null)
                                u.SendPacket(EventId.EGS_BATTLE_FIELD_BOSS_GATE_OPEN_NOT, openNot);
                    }
                    else
                    {
                        var closeNot = new KncWX2Server.Core.Packets.KEGS_BATTLE_FIELD_BOSS_GATE_CLOSE_NOT
                        {
                            BattleFieldId = not.PortalAppearanceMap,
                        };
                        foreach (var u in _simLayer.ActorManager.GetAllActors().OfType<KGSUser>())
                            if (u.ActiveUnit != null)
                                u.SendPacket(EventId.EGS_BATTLE_FIELD_BOSS_GATE_CLOSE_NOT, closeNot);
                    }
                }
                break;
            }

            case EventId.EGB_GET_AUTO_PARTY_BONUS_INFO_ACK:
            {
                var ack = ev.GetPayload<KncWX2Server.Core.Packets.KEGB_GET_AUTO_PARTY_BONUS_INFO_ACK>(Region);
                LatestAutoPartyBonusInfos = ack.AutoPartyBonusInfo;
                foreach (var info in ack.AutoPartyBonusInfo)
                    AutoPartyManager.UpdateAutoPartyBonusInfo(info.DungeonMode, info.DungeonIdWithDif, info.ActivatedBonus);
                break;
            }

            case EventId.EGS_ADD_COMMON_FLAG_NOT:
            {
                var req = ev.GetPayload<KncWX2Server.Core.Packets.KEGS_ADD_COMMON_FLAG_NOT>(Region);
                _simLayer.AddCommonFlag(req.Flag);
                break;
            }

            case EventId.EGS_DEL_COMMON_FLAG_NOT:
            {
                var req = ev.GetPayload<KncWX2Server.Core.Packets.KEGS_DEL_COMMON_FLAG_NOT>(Region);
                _simLayer.DeleteCommonFlag(req.Flag);
                break;
            }

            case EventId.E_DISCONNECT_SERVER_REPORT_NOT:
            {
                var req = ev.GetPayload<KncWX2Server.Core.Packets.KE_DISCONNECT_SERVER_REPORT_NOT>(Region);
                _disconnectReports.AddOrUpdate(req.Type, 1, static (_, cur) => cur + 1);
                break;
            }

            case EventId.ECN_MEGAPHONE_MESSAGE_NOT:
            {
                var not = ev.GetPayload<KncWX2Server.Core.Packets.KEGS_MEGAPHONE_MESSAGE_NOT>(Region);
                BroadcastMegaphone(false, not);
                break;
            }

            case EventId.ECN_NOTIFY_MSG_NOT:
            {
                var not = ev.GetPayload<KncWX2Server.Core.Packets.KECN_NOTIFY_MSG_NOT>(Region);
                BroadcastNotice(not.NotifyType, (sbyte)Math.Clamp(not.Count, 1, sbyte.MaxValue), not.Message);
                break;
            }

            case EventId.ECN_PARTY_BUFF_UPDATE_NOT:
            {
                var not = ev.GetPayload<KncWX2Server.Core.Packets.KECN_PARTY_BUFF_UPDATE_NOT>(Region);
                if (not.UnitUid > 0)
                    SendToUnit(not.UnitUid, EventId.ECN_PARTY_BUFF_UPDATE_NOT, not);
                break;
            }

            case EventId.ECN_SET_ACTIVE_LAGCHECK_NOT:
            {
                var pkt = ev.GetPayload<KncWX2Server.Core.Packets.KPacketBool>(Region);
                IsLagCheckActive = pkt.Value;
                break;
            }

            case EventId.ECN_UDP_KICK_GAMEEDIT_NOT:
            {
                var pkt = ev.GetPayload<KncWX2Server.Core.Packets.KEGS_UDP_KICK_GAMEEDIT_NOT>(Region);
                if (ev.FirstSenderUid > 0)
                    SendToUnit(ev.FirstSenderUid, EventId.EGS_UDP_KICK_GAMEEDIT_NOT, pkt);
                break;
            }

            case EventId.ECN_CHECK_INVALID_PVP_USER_NOT:
            {
                var pkt = ev.GetPayload<KncWX2Server.Core.Packets.KPacketLong>(Region);
                if (pkt.Value > 0 && GetUserByUnitUid(pkt.Value) is { } u)
                    u.SendPacket(EventId.E_KICK_USER_NOT, new KncWX2Server.Core.Packets.KPacketOK { Ok = NetError.ERR_MATCH_MAKING_15 });
                break;
            }

            case EventId.EGB_EXCHANGE_LIMIT_INFO_ACK:
            case EventId.EGB_EXCHANGE_LIMIT_INFO_NOT:
            {
                LatestExchangeLimitInfo = ev.GetPayload<KncWX2Server.Core.Packets.KEGB_EXCHANGE_LIMIT_INFO_NOT>(Region);
                break;
            }

            case EventId.EGB_TIME_ENCHANT_EVENT_INFO_ACK:
            case EventId.EGB_TIME_ENCHANT_EVENT_INFO_NOT:
            {
                LatestTimeEnchantEventInfo = ev.GetPayload<KncWX2Server.Core.Packets.KEGB_TIME_ENCHANT_EVENT_INFO_NOT>(Region);
                break;
            }

            case EventId.EGB_TODAY_RANDOM_QUEST_ACK:
            {
                var ack = ev.GetPayload<KncWX2Server.Core.Packets.KEGB_TODAY_RANDOM_QUEST_ACK>(Region);
                LatestTodayRandomQuests = new Dictionary<int, int>(ack.TodayRandomQuest);
                break;
            }

            case EventId.EGB_ADMIN_NOTIFY_HERO_PVP_USER_NOT:
            {
                var not = ev.GetPayload<KncWX2Server.Core.Packets.KEGS_ADMIN_NOTIFY_HERO_PVP_USER_REQ>(Region);
                LatestHeroPvpNotify = not;
                BroadcastNotice(1, 1, $"[HeroPvP] {not.NickName}: {not.Message}");
                break;
            }

            case EventId.ELG_CHECK_MODULE_INFO_UPDATE_NOT:
            {
                var not = ev.GetPayload<KncWX2Server.Core.Packets.KELG_CHECK_MODULE_INFO_UPDATE_NOT>(Region);
                LatestModuleInfos = new Dictionary<string, int>(not.ModuleName);
                break;
            }

            case EventId.ESR_ORDER_TO_REFRESH_MANAGER_REQ:
            case EventId.ESR_ORDER_TO_REFRESH_MANAGER_ACK:
            {
                var order = (ev.InProcessPayload != null || ev.Buffer.Length > 0)
                    ? ev.GetPayload<KncWX2Server.Core.Packets.KESR_SCRIPT_REFRESH_ORDER_NOT>(Region)
                    : new KncWX2Server.Core.Packets.KESR_SCRIPT_REFRESH_ORDER_NOT();
                if (order.OrderType > 0)
                    ScriptRefreshManager.ExecuteRefreshOrder(order.OrderType);
                else
                    LoadServerResources();
                break;
            }

            case EventId.EGB_UPDATE_WORLD_BUFF_NOT:
            {
                var not = ev.GetPayload<KncWX2Server.Core.Packets.KEGB_UPDATE_WORLD_BUFF_NOT>(Region);
                if (WorldMissionManager.UpdateWorldBuffInfo(not))
                {
                    var buffNot = WorldMissionManager.GetWorldBuff();
                    foreach (var user in GetActiveUsersSnapshot())
                        user.QueueEvent(EventId.EGB_UPDATE_WORLD_BUFF_NOT, buffNot);
                }
                break;
            }

            case EventId.EGB_UPDATE_GLOBAL_MISSION_INFO_NOT:
            {
                var ack = ev.GetPayload<KncWX2Server.Core.Packets.KEGB_GET_GLOBAL_MISSION_INFO_ACK>(Region);
                LatestGlobalMissionSnapshot = ack;
                GlobalMissionManager.UpdateGlobalMissionInfo(ack);
                break;
            }

            case EventId.EGB_UPDATE_MISSION_INFO_NOT:
            {
                var ack = ev.GetPayload<KncWX2Server.Core.Packets.KEGB_GET_MISSION_INFO_ACK>(Region);
                LatestWorldMissionSnapshot = ack;
                WorldMissionManager.UpdateMissionInfo(ack);
                break;
            }

            case EventId.ELG_HENIR_RANKING_REFRESH_NOT:
            {
                var not = ev.GetPayload<KncWX2Server.Core.Packets.KELG_HENIR_RANKING_REFRESH_NOT>(Region);
                RankingManager.UpdateHenirRanking(not.HenirRanking);
                break;
            }

            case EventId.ESR_ORDER_TO_GAME_SERVER_NOT:
            {
                var order = ev.GetPayload<KncWX2Server.Core.Packets.KESR_SCRIPT_REFRESH_ORDER_NOT>(Region);
                KLog.Clog($"[{Name}] ESR_ORDER_TO_GAME_SERVER_NOT: OrderType={order.OrderType}");
                if (order.OrderType > KncWX2Server.Core.Packets.KESR_SCRIPT_REFRESH_ORDER_NOT.OT_CN_SENTINEL &&
                    order.OrderType < KncWX2Server.Core.Packets.KESR_SCRIPT_REFRESH_ORDER_NOT.OT_LG_SENTINEL)
                {
                    Router.Send(PerformerId.PI_GS_SERVER, Uid, PerformerId.PI_LOGIN_SERVER, 0,
                        EventId.ESR_ORDER_TO_REFRESH_MANAGER_REQ, order);
                }
                else if (order.OrderType > KncWX2Server.Core.Packets.KESR_SCRIPT_REFRESH_ORDER_NOT.OT_LG_SENTINEL &&
                         order.OrderType < KncWX2Server.Core.Packets.KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_SENTINEL)
                {
                    ScriptRefreshManager.ExecuteRefreshOrder(order.OrderType);
                }
                else
                {
                    KLog.Cwarn($"[{Name}] Undefined GameServer script refresh order type: {order.OrderType}");
                }
                ServerCounters.AddOrUpdate(ev.EventId, 1, static (_, c) => c + 1);
                break;
            }

            case EventId.EGB_SHUT_DOWN_NOTIFY_NOT:
                BroadcastNotice(1, 1, ev.GetPayload<KncWX2Server.Core.Packets.KPacketString>(Region).Value);
                break;

            case EventId.ECN_WORLD_TRIGGER_RELOCATION_NOT:
            {
                var reloc = ev.GetPayload<KncWX2Server.Core.Packets.KEGS_WORLD_TRIGGER_RELOCATION_NOT>(Region);
                foreach (var u in _simLayer.ActorManager.GetAllActors().OfType<KGSUser>())
                    if (u.ActiveUnit != null)
                        u.SendPacket(EventId.EGS_WORLD_TRIGGER_RELOCATION_NOT, reloc);
                break;
            }

            case EventId.EGB_UPDATE_AUTO_PARTY_BONUS_INFO_NOT:
            {
                var not = ev.GetPayload<KncWX2Server.Core.Packets.KEGB_UPDATE_AUTO_PARTY_BONUS_INFO_NOT>(Region);
                AutoPartyManager.UpdateAutoPartyBonusInfo(not.DungeonMode, not.DungeonIDWithDif, not.ActivatedBonus);
                LatestAutoPartyBonusInfos = AutoPartyManager.SnapshotAll();
                break;
            }

            case EventId.ELG_USER_DISCONNECT_ACK:
            {
                if (ev.TryReadPayload<KncWX2Server.Core.Packets.KELG_USER_DISCONNECT_ACK>(Region, out var dack) && dack.UserUid > 0)
                {
                    var u = _simLayer.ActorManager.GetAllActors().OfType<KGSUser>().FirstOrDefault(a => a.AccountUid == dack.UserUid);
                    u?.ReserveDestroy();
                }
                break;
            }

            case EventId.ELG_ADMIN_KICK_USER_ACK:
                if (ev.FirstSenderUid > 0 && ev.InProcessPayload is IKSerializable kickAck)
                    SendToUnit(ev.FirstSenderUid, EventId.ELG_ADMIN_KICK_USER_ACK, kickAck);
                break;

            case EventId.ELG_GET_GUILD_INFO_ACK:
            {
                var gack = ev.GetPayload<KncWX2Server.Core.Packets.KELG_GET_GUILD_INFO_ACK>(Region);
                if (gack.RequesterUnitUid > 0)
                    SendToUnit(gack.RequesterUnitUid, EventId.ELG_GET_GUILD_INFO_ACK, gack);
                break;
            }

            // ---- Additional C++ GameServer Server-Level Parity Handlers ----
            case EventId.ELG_VERIFY_SERVER_CONNECT_ACK:
            {
                var ack = ev.GetPayload<KncWX2Server.Core.Packets.KECN_VERIFY_SERVER_CONNECT_ACK>(Region);
                IsLoginConnected = ack.Ok == NetError.NET_OK;
                break;
            }

            case EventId.ECN_UPDATE_SERVER_INFO_ACK:
            {
                var ack = ev.GetPayload<KncWX2Server.Core.Packets.KServerList>(Region);
                foreach (var srv in ack.ServerList)
                    if (srv.Uid > 0) ConnectedServers[srv.Uid] = srv;
                break;
            }

            case EventId.ECN_CHECK_CHANNEL_CHANGE_REQ:
            {
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_GS_SERVER, Uid, PerformerId.PI_CN_SERVER, Center.Uid,
                        EventId.ECN_CHECK_CHANNEL_CHANGE_ACK, new KncWX2Server.Core.Packets.KECN_CHECK_CHANNEL_CHANGE_ACK
                        {
                            Ok = _byUnitUid.Count < 5000 ? NetError.NET_OK : NetError.ERR_CHANNEL_CHANGE_01,
                        });
                break;
            }

            case EventId.ECN_NEW_PVP_USER_INFO_NOT:
            {
                var not = ev.GetPayload<KncWX2Server.Core.Packets.KECN_NEW_PVP_USER_INFO_NOT>(Region);
                if (not.UnitUid > 0) PvpLobbyUsers[not.UnitUid] = not.Info;
                break;
            }

            case EventId.ECN_UPDATE_PVP_USER_INFO_NOT:
            {
                var not = ev.GetPayload<KncWX2Server.Core.Packets.KECN_UPDATE_PVP_USER_INFO_NOT>(Region);
                if (PvpLobbyUsers.TryGetValue(not.UnitUid, out var u))
                    u.State = not.State;
                break;
            }

            case EventId.EBILL_CHECK_PRODUCT_INFO_ACK:
            {
                var ack = ev.GetPayload<KncWX2Server.Core.Packets.KEBILL_CHECK_PRODUCT_INFO_ACK>(Region);
                IsBillingEnabled = ack.Enable;
                BillingReleaseTick = ack.ReleaseTick;
                break;
            }

            case EventId.EGB_VERIFY_SERVER_CONNECT_NOT:
                IsGlobalConnected = true;
                FieldBossManager.ReservedTimeForInitRequest = 1;
                FieldBossManager.SetConnected(true);
                break;

            case EventId.EGB_VERIFY_SERVER_DISCONNECT_NOT:
                IsGlobalConnected = false;
                FieldBossManager.ReservedTimeForInitRequest = 0;
                FieldBossManager.SetConnected(false);
                break;

            case EventId.EGB_EARTH_QUAKE_NOT:
            {
                foreach (var u in _byAccountUid.Values)
                    u.SendID(EventId.EGS_EARTH_QUAKE_NOT);
                if (ev.TryReadPayload<KncWX2Server.Core.Packets.KPacketString>(Region, out var eqMsg) && !string.IsNullOrEmpty(eqMsg.Value))
                    BroadcastNotice(1, 1, eqMsg.Value);
                break;
            }

            case EventId.EGB_JACKPOT_EVENT_NOTIFY_NOT:
            {
                var not = ev.GetPayload<KncWX2Server.Core.Packets.KEGB_JACKPOT_EVENT_NOTIFY_NOT>(Region);
                BroadcastNotice(1, 1, $"[Jackpot] {not.WinnerNickName} won item #{not.RewardItemId}");
                break;
            }

            case EventId.EGB_JACKPOT_EVENT_POST_NOT:
            {
                var not = ev.GetPayload<KncWX2Server.Core.Packets.KEGB_JACKPOT_EVENT_POST_NOT>(Region);
                if (not.UnitUid > 0)
                {
                    Db.SendLetter(Region, 0, "GM", not.UnitUid,
                        "Jackpot Event Reward", $"Reward Item #{not.RewardItemId}", not.RewardItemId, Math.Max(1, not.Quantity), 0);
                }
                break;
            }

            case EventId.EGB_THE_GATE_OF_DARKNESS_INDIVIDUAL_BUFF_NOT:
            {
                var not = ev.GetPayload<KncWX2Server.Core.Packets.KEGB_THE_GATE_OF_DARKNESS_INDIVIDUAL_BUFF_NOT>(Region);
                if (not.UnitUid > 0)
                    SendToUnit(not.UnitUid, EventId.EGS_UPDATE_BUFF_INFO_NOT, not);
                break;
            }

            case EventId.EGB_UPDATE_GLOBAL_MISSION_START_TIME_NOT:
                if (ev.TryReadPayload<KncWX2Server.Core.Packets.KPacketString>(Region, out var gsTime))
                    GlobalMissionStartTime = gsTime.Value;
                break;

            case EventId.EGB_UPDATE_MISSION_START_TIME_NOT:
                if (ev.TryReadPayload<KncWX2Server.Core.Packets.KPacketString>(Region, out var msTime))
                {
                    WorldMissionStartTime = msTime.Value;
                    WorldMissionManager.UpdateMissionStartTime(msTime.Value);
                }
                break;

            case EventId.ELG_GUILD_LEVEL_UP_NOTIFY_NOT:
            {
                var not = ev.GetPayload<KncWX2Server.Core.Packets.KELG_GUILD_LEVEL_UP_NOTIFY_NOT>(Region);
                BroadcastNotice(1, 1, $"Guild [{not.GuildName}] reached Level {not.GuildLevel}!");
                break;
            }

            case EventId.ELG_KICK_RESULT_LETTER_NOT:
            {
                var not = ev.GetPayload<KncWX2Server.Core.Packets.KELG_KICK_RESULT_LETTER_NOT>(Region);
                if (not.TargetUnitUid > 0)
                    Db.SendLetter(Region, 0, "Guild", not.TargetUnitUid,
                        "Guild Notice", $"Removed from guild [{not.GuildName}].", 0, 0, 0);
                break;
            }

            case EventId.ELG_SEND_LETTER_JOIN_GUILD_NOT:
            {
                var not = ev.GetPayload<KncWX2Server.Core.Packets.KELG_SEND_LETTER_JOIN_GUILD_NOT>(Region);
                if (not.TargetUnitUid > 0)
                    Db.SendLetter(Region, 0, "Guild", not.TargetUnitUid,
                        "Guild Welcome", $"Joined guild [{not.GuildName}].", 0, 0, 0);
                break;
            }

            case EventId.ELG_RANKING_TITLE_REWARD_NOT:
            {
                var not = ev.GetPayload<KncWX2Server.Core.Packets.KELG_RANKING_TITLE_REWARD_NOT>(Region);
                if (not.UnitUid > 0 && not.TitleId > 0)
                    Db.GrantUnitTitle(not.UnitUid, not.TitleId);
                break;
            }

            case EventId.ELG_WEB_POINT_REWARD_INSERT_TO_POST_NOT:
            {
                var not = ev.GetPayload<KncWX2Server.Core.Packets.KELG_WEB_POINT_REWARD_INSERT_TO_POST_NOT>(Region);
                if (not.UnitUid > 0 && not.ItemId > 0)
                    Db.SendLetter(Region, 0, "System", not.UnitUid,
                        "Web Point Reward", $"Reward Item #{not.ItemId}", not.ItemId, Math.Max(1, not.Quantity), 0);
                break;
            }

            case EventId.ELG_WEB_RANKING_REFRESH_NOT:
            {
                IsWebRankingRefreshNeeded = true;
                var not = ev.GetPayload<KncWX2Server.Core.Packets.KELG_WEB_RANKING_REFRESH_NOT>(Region);
                RankingManager.UpdateDungeonAndPvpRanking(not);
                break;
            }

            case EventId.ELG_WEDDING_HALL_INFO_UPDATE_NOT:
            {
                var not = ev.GetPayload<KncWX2Server.Core.Packets.KELG_WEDDING_HALL_INFO_UPDATE_NOT>(Region);
                if (not.WeddingUid > 0) WeddingHallRegistry[not.WeddingUid] = not.HallInfo;
                break;
            }

            case EventId.ELG_LOCAL_RANKING_CHANGED_RANKER_INFO_CHECK_NOT:
                LocalRankingRankerCache.Clear();
                break;

            case EventId.ELG_LOCAL_RANKING_CHANGED_RANKER_INFO_NOT:
            {
                var not = ev.GetPayload<KncWX2Server.Core.Packets.KELG_LOCAL_RANKING_CHANGED_RANKER_INFO_NOT>(Region);
                if (not.UnitInfo.UnitUid > 0)
                    LocalRankingRankerCache[not.UnitInfo.UnitUid] = not;
                break;
            }

            case EventId.ELG_LOCAL_RANKING_RESET_NOT:
                LocalRankingRankerCache.Clear();
                break;

            case EventId.ELG_LOCAL_RANKING_SYSTEM_CHECK_NOT:
                IsLocalRankingEnabled = ev.GetPayload<KncWX2Server.Core.Packets.KPacketBool>(Region).Value;
                break;

            case EventId.EPM_OPEN_PARTY_LIST_NOT:
                PartyListRegistry.Clear();
                PartyMemberRegistry.Clear();
                break;

            case EventId.EPM_CLOSE_PARTY_LIST_NOT:
                PartyListRegistry.Clear();
                PartyMemberRegistry.Clear();
                break;

            case EventId.EPM_ADD_PARTY_LIST_NOT:
            {
                var not = ev.GetPayload<KncWX2Server.Core.Packets.KEPM_ADD_PARTY_LIST_NOT>(Region);
                PartyListRegistry[not.PartyInfo.PartyInfo.PartyUid] = not.PartyInfo;
                break;
            }

            case EventId.EPM_UPDATE_PARTY_IN_PARTY_LIST_NOT:
            {
                var not = ev.GetPayload<KncWX2Server.Core.Packets.KEPM_UPDATE_PARTY_IN_PARTY_LIST_NOT>(Region);
                PartyListRegistry[not.PartyInfo.PartyInfo.PartyUid] = not.PartyInfo;
                break;
            }

            case EventId.EPM_DEL_PARTY_LIST_NOT:
            {
                var not = ev.GetPayload<KncWX2Server.Core.Packets.KEPM_DEL_PARTY_LIST_NOT>(Region);
                PartyListRegistry.TryRemove(not.PartyUid, out _);
                PartyMemberRegistry.TryRemove(not.PartyUid, out _);
                break;
            }

            case EventId.EPM_ADD_PARTY_MEMBER_IN_PARTY_LIST_NOT:
            {
                var not = ev.GetPayload<KncWX2Server.Core.Packets.KEPM_ADD_PARTY_MEMBER_IN_PARTY_LIST_NOT>(Region);
                var list = PartyMemberRegistry.GetOrAdd(not.PartyUid, _ => new());
                lock (list)
                {
                    list.RemoveAll(m => m.UnitUid == not.MemberInfo.UnitUid);
                    list.Add(not.MemberInfo);
                }
                break;
            }

            case EventId.EPM_DEL_PARTY_MEMBER_IN_PARTY_LIST_NOT:
            {
                var not = ev.GetPayload<KncWX2Server.Core.Packets.KEPM_DEL_PARTY_MEMBER_IN_PARTY_LIST_NOT>(Region);
                if (PartyMemberRegistry.TryGetValue(not.PartyUid, out var list))
                    lock (list) list.RemoveAll(m => m.UnitUid == not.UnitUid);
                break;
            }

            case EventId.EPM_UPDATE_PARTY_MEMBER_IN_PARTY_LIST_NOT:
            {
                var not = ev.GetPayload<KncWX2Server.Core.Packets.KEPM_UPDATE_PARTY_MEMBER_IN_PARTY_LIST_NOT>(Region);
                if (PartyMemberRegistry.TryGetValue(not.PartyUid, out var list))
                    lock (list)
                    {
                        int idx = list.FindIndex(m => m.UnitUid == not.MemberInfo.UnitUid);
                        if (idx >= 0) list[idx] = not.MemberInfo;
                        else list.Add(not.MemberInfo);
                    }
                break;
            }

            case EventId.EPM_UPDATE_PARTY_FEVER_IN_PARTY_LIST_NOT:
            {
                var not = ev.GetPayload<KncWX2Server.Core.Packets.KEPM_UPDATE_PARTY_FEVER_IN_PARTY_LIST_NOT>(Region);
                if (PartyListRegistry.TryGetValue(not.PartyUid, out var p))
                    p.PartyInfo.PartyFever = not.FeverPoint;
                break;
            }

            case EventId.ERM_ROOM_LIST_INFO_NOT:
            {
                var not = ev.GetPayload<KncWX2Server.Core.Packets.KERM_ROOM_LIST_INFO_NOT>(Region);
                if (not.InfoType == 2) RoomListRegistry.TryRemove(not.RoomInfo.RoomUid, out _);
                else if (not.RoomInfo.RoomUid > 0) RoomListRegistry[not.RoomInfo.RoomUid] = not.RoomInfo;
                break;
            }

            case EventId.ERM_BATTLE_FIELD_ROOM_INFO_NOT:
            {
                var not = ev.GetPayload<KncWX2Server.Core.Packets.KERM_BATTLE_FIELD_ROOM_INFO_NOT>(Region);
                if (not.InfoType == 2) BattleFieldRoomRegistry.TryRemove(not.RoomInfo.RoomUid, out _);
                else if (not.RoomInfo.RoomUid > 0) BattleFieldRoomRegistry[not.RoomInfo.RoomUid] = not.RoomInfo;
                break;
            }

            case EventId.ERM_ROOM_COUNT_INFO_NOT:
            {
                var not = ev.GetPayload<KncWX2Server.Core.Packets.KERM_ROOM_COUNT_INFO_NOT>(Region);
                PvpRoomCount = not.PvpRoomCount;
                DungeonRoomCount = not.DungeonRoomCount;
                BattleFieldRoomCount = not.BattleFieldRoomCount;
                break;
            }

            case EventId.ERM_SQUARE_LIST_INFO_NOT:
            {
                var not = ev.GetPayload<KncWX2Server.Core.Packets.KERM_SQUARE_LIST_INFO_NOT>(Region);
                if (not.InfoType == 2) SquareListRegistry.TryRemove(not.SquareInfo.SquareUid, out _);
                else if (not.SquareInfo.SquareUid > 0) SquareListRegistry[not.SquareInfo.SquareUid] = not.SquareInfo;
                break;
            }

            case EventId.ERM_PERSONAL_SHOP_INFO_NOT:
            {
                var not = ev.GetPayload<KncWX2Server.Core.Packets.KERM_PERSONAL_SHOP_INFO_NOT>(Region);
                if (not.InfoType == 2) PersonalShopRegistry.TryRemove(not.ShopUid, out _);
                else if (not.ShopUid > 0) PersonalShopRegistry[not.ShopUid] = not;
                break;
            }

            case EventId.ERM_WEDDING_HALL_INFO_NOT:
            {
                var not = ev.GetPayload<KncWX2Server.Core.Packets.KERM_WEDDING_HALL_INFO_NOT>(Region);
                if (not.InfoType == 2) WeddingHallRegistry.TryRemove(not.HallInfo.WeddingUid, out _);
                else if (not.HallInfo.WeddingUid > 0) WeddingHallRegistry[not.HallInfo.WeddingUid] = not.HallInfo;
                break;
            }

            case EventId.ERM_UPDATE_FIELD_INFO_NOT:
            {
                var not = ev.GetPayload<KncWX2Server.Core.Packets.KERM_UPDATE_FIELD_INFO_NOT>(Region);
                if (not.MapId > 0) FieldStatistics[not.MapId] = not;
                break;
            }

            case EventId.ERM_UPDATE_TOTAL_DANGEROUS_VALUE_NOT:
            {
                if (ev.InProcessPayload is KncWX2Server.Core.Packets.KPacketOK legacyOk)
                {
                    TotalFieldDangerousValue = legacyOk.Ok;
                }
                else
                {
                    var req = ev.GetPayload<KncWX2Server.Core.Packets.KERM_UPDATE_TOTAL_DANGEROUS_VALUE_NOT>(Region);
                    TotalFieldDangerousValue += req.IncreasedValue;
                    Router.Send(PerformerId.PI_GS_SERVER, Uid, PerformerId.PI_GLOBAL_SERVER, Global.Uid,
                        EventId.EGB_UPDATE_TOTAL_DANGEROUS_VALUE_NOT,
                        new KncWX2Server.Core.Packets.KEGB_UPDATE_TOTAL_DANGEROUS_VALUE_NOT
                        {
                            BattleFieldId = req.BattleFieldId,
                            IncreasedValue = req.IncreasedValue,
                        });
                }
                break;
            }

            case EventId.DCHATLOG_RECORD_CHAT_NOT:
            {
                var chatLog = ev.GetPayload<KncWX2Server.Core.Packets.KDCHATLOG_RECORD_CHAT_NOT>(Region);
                Db.RecordChatLog(chatLog.AccountUid, chatLog.UnitUid, chatLog.ServerGroupId, chatLog.ChatText, chatLog.ChatType);
                break;
            }

            case EventId.DBE_UPDATE_REWARD_COMPLETE_JP_EVENT_NOT:
            {
                var not = ev.GetPayload<KncWX2Server.Core.Packets.KDBE_UPDATE_REWARD_COMPLETE_JP_EVENT_NOT>(Region);
                Db.InsertGameLog(0, not.RId, "JP_EVENT_REWARD_COMPLETE", (int)not.RId, 0, "Completed");
                break;
            }

            case EventId.ERM_COMPLETE_RETURN_TO_FIELD_NOT:
            case EventId.ERM_END_GAME_REGROUP_PARTY_NOT:
            case EventId.ERM_INCREASE_PARTY_FEVER_NOT:
            case EventId.ERM_MAX_PARTY_FEVER_NOT:
            {
                if (ev.FirstSenderUid > 0 && ev.InProcessPayload is IKSerializable serPayload)
                    SendToUnit(ev.FirstSenderUid, ev.EventId, serPayload);
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

        _byAccountUid.Clear();
        _byUnitUid.Clear();
        TrServer.DisposeAsync().AsTask().GetAwaiter().GetResult();
        UdpEcho.DisposeAsync().AsTask().GetAwaiter().GetResult();
        UdpRelay.DisposeAsync().AsTask().GetAwaiter().GetResult();
        base.ShutDown();
        ServerReady = false;
    }
}
