using System.Collections.Concurrent;
using System.Threading.Channels;
using KncWX2Server.Core.Events;
using KncWX2Server.Core.Framework;
using KncWX2Server.Core.Logging;
using KncWX2Server.Core.Packets;
using MoonSharp.Interpreter;

namespace KncWX2Server.Core.Scripting;

/// <summary>
/// Unified C# port of C++ *ScriptRefreshManager and *ScriptRefreshThread
/// (GSScriptRefreshManager/Thread, CnScriptRefreshManager/Thread, LgScriptRefreshManager/Thread,
/// GBScriptRefreshManager/Thread, ChScriptRefreshManager/Thread).
/// </summary>
public sealed class KScriptRefreshManager : IDisposable
{
    private readonly InProcessRouter _router;
    private readonly uint _sourcePerformerId;
    private readonly long _sourceUid;
    private readonly ConcurrentDictionary<int, Action> _orderHandlers = new();
    private readonly Channel<KESR_SCRIPT_REFRESH_ORDER_NOT> _queue = Channel.CreateUnbounded<KESR_SCRIPT_REFRESH_ORDER_NOT>();
    private readonly ConcurrentDictionary<string, DateTime> _lastWatchedEventUtc = new(StringComparer.OrdinalIgnoreCase);

    private Action<int>? _defaultReloadHandler;
    private CancellationTokenSource? _cts;
    private Task? _workerTask;
    private FileSystemWatcher? _watcher;

    public long ProcessedOrderCount { get; private set; }
    public int LastProcessedOrderType { get; private set; }

    public KScriptRefreshManager(InProcessRouter router, uint sourcePerformerId, long sourceUid = 0)
    {
        _router = router;
        _sourcePerformerId = sourcePerformerId;
        _sourceUid = sourceUid;
    }

    public void RegisterOrderHandler(int orderType, Action handler)
        => _orderHandlers[orderType] = handler;

    public void SetDefaultReloadHandler(Action<int> handler)
        => _defaultReloadHandler = handler;

    public void Start()
    {
        if (_workerTask != null) return;
        _cts = new CancellationTokenSource();
        _workerTask = Task.Run(() => WorkerLoopAsync(_cts.Token));
    }

    public async Task StopAsync()
    {
        if (_cts == null) return;
        _cts.Cancel();
        _queue.Writer.TryComplete();
        if (_workerTask != null)
        {
            try { await _workerTask.ConfigureAwait(false); }
            catch (OperationCanceledException) { }
        }
        _cts.Dispose();
        _cts = null;
        _workerTask = null;
    }

    public void EnqueueOrder(KESR_SCRIPT_REFRESH_ORDER_NOT order)
    {
        _queue.Writer.TryWrite(order);
    }

    public bool ExecuteRefreshOrder(int orderType)
    {
        LastProcessedOrderType = orderType;
        ProcessedOrderCount++;
        if (_orderHandlers.TryGetValue(orderType, out var specific))
        {
            specific();
            return true;
        }
        if (_defaultReloadHandler != null)
        {
            _defaultReloadHandler(orderType);
            return true;
        }
        return false;
    }

    public void DispatchOrder(int orderType)
    {
        var order = new KESR_SCRIPT_REFRESH_ORDER_NOT { OrderType = orderType };
        if (orderType > KESR_SCRIPT_REFRESH_ORDER_NOT.OT_NONE &&
            orderType < KESR_SCRIPT_REFRESH_ORDER_NOT.OT_CN_SENTINEL)
        {
            _router.Send(_sourcePerformerId, _sourceUid, PerformerId.PI_CN_SERVER, 0,
                EventId.ESR_ORDER_TO_CENTER_SERVER_NOT, order);
        }
        else if (orderType > KESR_SCRIPT_REFRESH_ORDER_NOT.OT_CN_SENTINEL &&
                 orderType < KESR_SCRIPT_REFRESH_ORDER_NOT.OT_LG_SENTINEL)
        {
            _router.Send(_sourcePerformerId, _sourceUid, PerformerId.PI_LOGIN_SERVER, 0,
                EventId.ESR_ORDER_TO_REFRESH_MANAGER_REQ, order);
        }
        else if (orderType > KESR_SCRIPT_REFRESH_ORDER_NOT.OT_LG_SENTINEL &&
                 orderType < KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_SENTINEL)
        {
            _router.Send(_sourcePerformerId, _sourceUid, PerformerId.PI_GS_SERVER, 0,
                EventId.ESR_ORDER_TO_GAME_SERVER_NOT, order);
        }
        else if (orderType > KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_SENTINEL &&
                 orderType < KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GB_SENTINEL)
        {
            _router.Send(_sourcePerformerId, _sourceUid, PerformerId.PI_GLOBAL_SERVER, 0,
                EventId.ESR_ORDER_TO_GLOBAL_SERVER_NOT, order);
        }
        else if (orderType > KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GB_SENTINEL &&
                 orderType < KESR_SCRIPT_REFRESH_ORDER_NOT.OT_CH_SENTINEL)
        {
            _router.Send(_sourcePerformerId, _sourceUid, PerformerId.PI_CHANNEL_SERVER, 0,
                EventId.ESR_ORDER_TO_REFRESH_MANAGER_REQ, order);
        }
    }

    /// <summary>
    /// Exposes the exact C++ KGSScriptRefreshManager Lua "ScriptRefresh" global API.
    /// </summary>
    public void BindToLua(KScriptManager script, string globalName = "ScriptRefresh")
    {
        script.CreateLuaProxy(globalName, t =>
        {
            void Reg(string method, params int[] orderTypes)
            {
                t[method] = DynValue.NewCallback((_, _) =>
                {
                    foreach (int ot in orderTypes)
                        DispatchOrder(ot);
                    return DynValue.Nil;
                });
            }

            Reg("ItemManager", KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_ITEM_MANAGER, KESR_SCRIPT_REFRESH_ORDER_NOT.OT_CN_ITEM_MANAGER);
            Reg("RandomItemManager", KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_RANDOM_ITEM_MANAGER);
            Reg("ManufactureItemManager", KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_MANUFACTURE_ITEM_MANAGER);
            Reg("RewardTable", KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_REWARD_TABLE);
            Reg("QuestManager", KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_QUEST_MANAGER, KESR_SCRIPT_REFRESH_ORDER_NOT.OT_CN_QUEST_MANAGER, KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GB_QUEST_MANAGER);
            Reg("TitleManager", KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_TITLE_MANAGER, KESR_SCRIPT_REFRESH_ORDER_NOT.OT_LG_TITLE_MANAGER);
            Reg("DungeonManager", KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_DUNGEON_MANAGER, KESR_SCRIPT_REFRESH_ORDER_NOT.OT_CN_DUNGEON_MANAGER);
            Reg("DropTable", KESR_SCRIPT_REFRESH_ORDER_NOT.OT_CN_DROP_TABLE);
            Reg("GameEventScriptManager", KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_GAME_EVENT_SCRIPT_MANAGER, KESR_SCRIPT_REFRESH_ORDER_NOT.OT_CN_GAME_EVENT_SCRIPT_MANAGER);
            Reg("MapData", KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_MAP_DATA);
            Reg("GuildManager", KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_GUILD_MANAGER, KESR_SCRIPT_REFRESH_ORDER_NOT.OT_LG_GUILD_MANAGER);
            Reg("GameSysVal", KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_GAME_SYS_VAL, KESR_SCRIPT_REFRESH_ORDER_NOT.OT_CN_GAME_SYS_VAL, KESR_SCRIPT_REFRESH_ORDER_NOT.OT_LG_GAME_SYS_VAL);
            Reg("ResolveItemManager", KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_RESOLVE_ITEM_MANAGER);
            Reg("EnchantItemManager", KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_ENCHANT_ITEM_MANAGER);
            Reg("SocketItem", KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_SOCKET_ITEM);
            Reg("AttribEnchantItem", KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_ATTRIB_ENCHANT_ITEM);
            Reg("RecommendUserTable", KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_RECOMMEND_USER_TABLE);
            Reg("ExpTable", KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_EXP_TABLE);
            Reg("PetManager", KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_PET_MANAGER);
            Reg("AttribNpcTable", KESR_SCRIPT_REFRESH_ORDER_NOT.OT_CN_ATTRIB_NPC_TABLE);
            Reg("CnHackingCheckManager", KESR_SCRIPT_REFRESH_ORDER_NOT.OT_CN_HACKING_CHECK_MANAGER);
            Reg("MornitoringManager", KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_MORNITORING_MANAGER, KESR_SCRIPT_REFRESH_ORDER_NOT.OT_LG_MORNITORING_MANAGER, KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GB_MORNITORING_MANAGER);
            Reg("AbuserLogManager", KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_ABUSER_LOG_MANAGER, KESR_SCRIPT_REFRESH_ORDER_NOT.OT_CN_ABUSER_LOG_MANAGER);
            Reg("ResultProcess", KESR_SCRIPT_REFRESH_ORDER_NOT.OT_CN_RESULT_PROCESS);
            Reg("WorldMissionManager", KESR_SCRIPT_REFRESH_ORDER_NOT.OT_LG_WORLD_MISSION_MANAGER, KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GB_WORLD_MISSION_MANAGER);
            Reg("PvpMatchManager", KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_PVP_MATCH_MANAGER, KESR_SCRIPT_REFRESH_ORDER_NOT.OT_CN_PVP_MATCH_MANAGER, KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GB_PVP_MATCH_MANAGER);
            Reg("PvpMatchResultTable", KESR_SCRIPT_REFRESH_ORDER_NOT.OT_CN_PVP_MATCH_RESULT_TABLE);
            Reg("HenirResultTable", KESR_SCRIPT_REFRESH_ORDER_NOT.OT_CN_HENIR_RESULT_TABLE);
            Reg("ContentManager", KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_CONTENT_MANAGER);
            Reg("CompanyIPTable", KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_COMPANY_IP_TABLE);
            Reg("ClassChangeTable", KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_CLASS_CHANGE_TABLE);
            Reg("StringFilter", KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_STRING_FILTER);
            Reg("DailyGiftBoxManager", KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_DAILY_GIFT_BOX_MANAGER);
            Reg("BattleFieldManager", KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_BATTLE_FIELD_MANAGER, KESR_SCRIPT_REFRESH_ORDER_NOT.OT_CN_BATTLE_FIELD_MANAGER, KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GB_BATTLE_FIELD_MANAGER);
            Reg("AutoPartyScriptManager", KESR_SCRIPT_REFRESH_ORDER_NOT.OT_CN_AUTO_PARTY_SCRIPT_MANAGER, KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GB_AUTO_PARTY_SCRIPT_MANAGER);
            Reg("PartyManager", KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_PARTY_MANAGER, KESR_SCRIPT_REFRESH_ORDER_NOT.OT_CN_PARTY_MANAGER);
            Reg("SmsManager", KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_SMS_MANAGER, KESR_SCRIPT_REFRESH_ORDER_NOT.OT_CN_SMS_MANAGER, KESR_SCRIPT_REFRESH_ORDER_NOT.OT_LG_SMS_MANAGER, KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GB_SMS_MANAGER, KESR_SCRIPT_REFRESH_ORDER_NOT.OT_CH_SMS_MANAGER);
            Reg("BingoEventManager", KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_BINGO_EVENT_MANAGER);
            Reg("BadAttitudeTable", KESR_SCRIPT_REFRESH_ORDER_NOT.OT_CN_BAD_ATTITUDE_TABLE);
            Reg("SynthesisTable", KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_SYNTHESIS_TABLE);
            Reg("LogManager", KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_LOG_MANAGER, KESR_SCRIPT_REFRESH_ORDER_NOT.OT_CN_LOG_MANAGER, KESR_SCRIPT_REFRESH_ORDER_NOT.OT_LG_LOG_MANAGER, KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GB_LOG_MANAGER, KESR_SCRIPT_REFRESH_ORDER_NOT.OT_CH_LOG_MANAGER);
            Reg("DefenceDungeonManager", KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_DEFENCE_DUNGEON_MANAGER, KESR_SCRIPT_REFRESH_ORDER_NOT.OT_CN_DEFENCE_DUNGEON_MANAGER);
            Reg("BuffManager", KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_BUFF_MANAGER, KESR_SCRIPT_REFRESH_ORDER_NOT.OT_CN_BUFF_MANAGER);
            Reg("RidingPetManager", KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_RIDING_PET_MANAGER);
            Reg("SkillTree", KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_SKILL_TREE, KESR_SCRIPT_REFRESH_ORDER_NOT.OT_LG_SKILL_TREE);
            Reg("FieldBossData", KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_FIELD_BOSS_DATA, KESR_SCRIPT_REFRESH_ORDER_NOT.OT_CN_FIELD_BOSS_DATA, KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GB_FIELD_BOSS_DATA);
            Reg("EDInventory", KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_ED_INVENTORY_EXPAND);
        });
    }

    public static IReadOnlyList<int> ResolveOrderTypesForFile(string fileName)
    {
        string name = Path.GetFileName(fileName);
        return name.ToUpperInvariant() switch
        {
            "EXPTABLE.LUA" => [KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_EXP_TABLE],
            "REWARDTABLE.LUA" => [KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_REWARD_TABLE],
            "GAMESYSVALTABLE.LUA" =>
            [
                KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_GAME_SYS_VAL,
                KESR_SCRIPT_REFRESH_ORDER_NOT.OT_CN_GAME_SYS_VAL,
                KESR_SCRIPT_REFRESH_ORDER_NOT.OT_LG_GAME_SYS_VAL,
            ],
            "EDINVENTORYEXPAND.LUA" => [KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_ED_INVENTORY_EXPAND],
            "SYNTHESISTABLE.LUA" => [KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_SYNTHESIS_TABLE],
            "RESOLVETABLE.LUA" => [KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_RESOLVE_ITEM_MANAGER],
            "ENCHANTTABLE.LUA" => [KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_ENCHANT_ITEM_MANAGER],
            "ATTRIBENCHANTTABLE.LUA" or "ATTRIBATTACHTABLE.LUA" => [KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_ATTRIB_ENCHANT_ITEM],
            "SOCKETITEMTABLE.LUA" or "ITEMRANDOMSOCKET.LUA" => [KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_SOCKET_ITEM],
            "RANDOMITEMDATA.LUA" or "RANDOMITEMMAPPING.LUA" or "RANDOMITEMTABLE.LUA" => [KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_RANDOM_ITEM_MANAGER],
            "MANUFACTURERESULTTABLE.LUA" => [KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_MANUFACTURE_ITEM_MANAGER],
            "CLASSCHANGETABLE.LUA" => [KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_CLASS_CHANGE_TABLE],
            "COMPANYIPTABLE.LUA" => [KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_COMPANY_IP_TABLE],
            "PETDATA.LUA" => [KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_PET_MANAGER],
            "RIDINGPETDATA.LUA" => [KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_RIDING_PET_MANAGER],
            "CONTENTDATA.LUA" => [KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_CONTENT_MANAGER],
            "RECOMMENDUSERTABLE.LUA" => [KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_RECOMMEND_USER_TABLE],
            "ABUSERLIST.LUA" => [KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_ABUSER_LOG_MANAGER, KESR_SCRIPT_REFRESH_ORDER_NOT.OT_CN_ABUSER_LOG_MANAGER],
            "DAILYGIFTBOXPACKAGE.LUA" => [KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_DAILY_GIFT_BOX_MANAGER],
            "BINGOEVENTINFO.LUA" => [KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_BINGO_EVENT_MANAGER],
            "EVENTDATA.LUA" => [KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_GAME_EVENT_SCRIPT_MANAGER, KESR_SCRIPT_REFRESH_ORDER_NOT.OT_CN_GAME_EVENT_SCRIPT_MANAGER],
            "FIELDDATA.LUA" or "MAPDATA.LUA" => [KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_MAP_DATA],
            "DROPTABLE.LUA" or "SPECIALDROPDATA.LUA" => [KESR_SCRIPT_REFRESH_ORDER_NOT.OT_CN_DROP_TABLE],
            "PVPMATCHMANAGER.LUA" => [KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_PVP_MATCH_MANAGER, KESR_SCRIPT_REFRESH_ORDER_NOT.OT_CN_PVP_MATCH_MANAGER, KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GB_PVP_MATCH_MANAGER],
            "AUTOPARTYDATA.LUA" => [KESR_SCRIPT_REFRESH_ORDER_NOT.OT_CN_AUTO_PARTY_SCRIPT_MANAGER, KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GB_AUTO_PARTY_SCRIPT_MANAGER],
            "BATTLEFIELDDATA.LUA" => [KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_BATTLE_FIELD_MANAGER, KESR_SCRIPT_REFRESH_ORDER_NOT.OT_CN_BATTLE_FIELD_MANAGER, KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GB_BATTLE_FIELD_MANAGER],
            "LOGCONFIG.LUA" => [KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_LOG_MANAGER, KESR_SCRIPT_REFRESH_ORDER_NOT.OT_CN_LOG_MANAGER],
            _ => [KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_GAME_SYS_VAL],
        };
    }

    public bool EnableFileWatcher(string resourceDirectory, TimeSpan? debounce = null)
    {
        if (!Directory.Exists(resourceDirectory))
            return false;

        DisableFileWatcher();
        TimeSpan minInterval = debounce ?? TimeSpan.FromMilliseconds(250);
        _watcher = new FileSystemWatcher(resourceDirectory, "*.lua")
        {
            IncludeSubdirectories = true,
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.Size,
            EnableRaisingEvents = true,
        };
        _watcher.Changed += (_, e) => OnWatchedFileChanged(e.FullPath, minInterval);
        _watcher.Created += (_, e) => OnWatchedFileChanged(e.FullPath, minInterval);
        return true;
    }

    public void DisableFileWatcher()
    {
        if (_watcher == null) return;
        _watcher.EnableRaisingEvents = false;
        _watcher.Dispose();
        _watcher = null;
    }

    private void OnWatchedFileChanged(string fullPath, TimeSpan minInterval)
    {
        var now = DateTime.UtcNow;
        if (_lastWatchedEventUtc.TryGetValue(fullPath, out var prev) && now - prev < minInterval)
            return;
        _lastWatchedEventUtc[fullPath] = now;

        foreach (int ot in ResolveOrderTypesForFile(fullPath))
            DispatchOrder(ot);
    }

    private async Task WorkerLoopAsync(CancellationToken ct)
    {
        await foreach (var order in _queue.Reader.ReadAllAsync(ct).ConfigureAwait(false))
        {
            try
            {
                ExecuteRefreshOrder(order.OrderType);
            }
            catch (Exception ex)
            {
                KLog.Cerr($"[KScriptRefreshManager] Failed executing refresh order {order.OrderType}: {ex.Message}");
            }
        }
    }

    public void Dispose()
    {
        DisableFileWatcher();
        _cts?.Cancel();
        _queue.Writer.TryComplete();
        _cts?.Dispose();
    }
}
