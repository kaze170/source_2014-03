using System.Collections.Concurrent;
using KncWX2Server.Core.Packets;

namespace KncWX2Server.Core.Framework;

/// <summary>
/// Faithful C# port of Common/Statistics.{h,cpp} &amp; StatisticsID_def.h:
/// tracks login, inventory, economy, dungeon, and PvP telemetry counters across server layers.
/// </summary>
public sealed class KStatisticsManager
{
    public const int SI_LOGIN_LOG = 1;

    // StatisticsID_def.h constants
    public const int SI_PVP_MAP = 0;
    public const int SI_FPS = 1;
    public const int SI_RES_STONE = 2;
    public const int SI_ED = 3;
    public const int SI_QUEST = 4;
    public const int SI_DISCONNECT_REASON = 5;
    public const int SI_DISCONNECT_USERFSM = 6;
    public const int SI_LOC_ITEM = 7;
    public const int SI_LOC_PVP = 8;
    public const int SI_LOC_DUNGEON = 9;
    public const int SI_LOC_SPIRIT = 10;
    public const int SI_LOC_ENCHANT = 11;
    public const int SI_LOC_PVP_VICTORY_PER = 12;
    public const int SI_LOC_PVP_ROOM = 13;
    public const int SI_LOC_DUNGEON_ROOM = 14;
    public const int SI_LOC_HENIR_RANKING = 15;
    public const int SI_LOC_TITLE = 16;
    public const int SI_LOC_PET_SUMMON = 17;
    public const int SI_LOC_SERV_DISCONNECT = 18;
    public const int SI_LOC_BILLING_PACKET = 19;
    public const int SI_LOC_ABUSER_MORNITORING = 20;
    public const int SI_CHARACTER = 21;
    public const int SI_ITEM_DB = 22;
    public const int SI_SENTINEL = 23;
    public const int SI_LOC_UNDEFINED_QUEST_TEMPLET = 24;

    public sealed class StatisticsSchedule
    {
        public int StatisticsId { get; set; }
        public double IntervalSeconds { get; set; }
        public bool UseDb { get; set; }
        public int FlushHour { get; set; } = -1;
    }

    public enum StatInventory
    {
        SiItemTrade = 0,
        SiItemPersonalShop = 1,
        SiItemTradeBoard = 2,
        SiItemBuy = 3,
        SiItemSell = 4,
        SiItemUse = 5,
        SiItemMake = 6,
        SiItemFailed = 7,
        SiItemDrop = 8,
        SiItemQuest = 9,
        SiItemEquip = 10,
        SiItemRandom = 11,
        SiItemResolve = 12,
        SiItemResolveResult = 13,
        SiItemMall = 14,
        SiItemPost = 15,
        SiItemIdentify = 16,
        SiItemExchangeSend = 17,
        SiItemExchangeRecv = 18,
        SiItemDiscard = 19,
        SiItemPetFeed = 20,
        SiItemRegPShop = 21,
        SiItemBuyPShopAgency = 22,
        SiItemPickUpPShop = 23,
    }

    private readonly int[] _loginResults = new int[(int)LoginResultType.Sentinel];
    private readonly object _loginLock = new();
    private readonly ConcurrentDictionary<(StatInventory category, int itemId), long> _itemStats = new();
    private readonly ConcurrentDictionary<string, long> _namedCounters = new();
    private readonly ConcurrentDictionary<int, StatisticsSchedule> _schedules = new();

    public bool SendToFtpStatistics { get; private set; }
    public bool BillingPacketLog { get; private set; }
    public bool UserLog { get; private set; }
    public IReadOnlyDictionary<int, StatisticsSchedule> Schedules => _schedules;

    public void AddStatistics(int statisticsId, double intervalSeconds, bool useDb)
    {
        _schedules[statisticsId] = new StatisticsSchedule
        {
            StatisticsId = statisticsId,
            IntervalSeconds = intervalSeconds,
            UseDb = useDb,
        };
    }

    public void SetStatisticsFlushTime(int statisticsId, int flushHour)
    {
        var sched = _schedules.GetOrAdd(statisticsId, id => new StatisticsSchedule { StatisticsId = id });
        sched.FlushHour = flushHour;
    }

    public void SendToFTPStatistics(bool enable) => SendToFtpStatistics = enable;
    public void SetBillingPacketLog(bool enable) => BillingPacketLog = enable;
    public void SetUserLog(bool enable) => UserLog = enable;

    public void AddLoginResult(LoginResultType type, int count = 1)
    {
        int idx = (int)type;
        if (idx < 0 || idx >= _loginResults.Length) return;
        lock (_loginLock)
            _loginResults[idx] += count;
    }

    public List<int> DrainLoginResults()
    {
        lock (_loginLock)
        {
            var copy = _loginResults.ToList();
            Array.Clear(_loginResults);
            return copy;
        }
    }

    public void AddItemStat(StatInventory category, int itemId, long quantity)
        => _itemStats.AddOrUpdate((category, itemId), quantity, (_, cur) => cur + quantity);

    public void IncrementCounter(string key, long delta = 1)
        => _namedCounters.AddOrUpdate(key, delta, (_, cur) => cur + delta);

    public long GetItemStat(StatInventory category, int itemId)
        => _itemStats.TryGetValue((category, itemId), out long v) ? v : 0;

    public long GetCounter(string key)
        => _namedCounters.TryGetValue(key, out long v) ? v : 0;

    public Dictionary<string, long> FlushAndResetCounters()
    {
        var snap = new Dictionary<string, long>(_namedCounters);
        _namedCounters.Clear();
        return snap;
    }
}
