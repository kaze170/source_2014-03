namespace KncWX2Server.GlobalServer;

/// <summary>
/// Faithful C# port of KGlobalServer ExchangeLimit system (SERV_ITEM_EXCHANGE_LIMIT):
/// tracks global limited-stock item exchanges, atomic decrement/rollback, and expiry windows.
/// </summary>
public sealed class KExchangeLimitManager
{
    public enum ExchangeLimitResult
    {
        NotInited = 0,
        Succeed = 1,
        Exhausted = 2,
        Pass = 3,
    }

    public sealed class ExchangeLimitEntry
    {
        public int LimitUid { get; init; }
        public HashSet<int> DestItemIds { get; init; } = new();
        public int LimitQuantity { get; set; }
        public DateTime BeginUtc { get; init; }
        public DateTime EndUtc { get; init; }
    }

    private readonly object _sync = new();
    private readonly List<ExchangeLimitEntry> _activeEntries = new();
    private readonly List<ExchangeLimitEntry> _pendingEntries = new();
    private int _nextRollBackUid = 1;

    public int ExchangeLimitUid { get; private set; } = 1;
    public bool Changed { get; private set; }

    public void Initialize(int exchangeLimitUid, IEnumerable<ExchangeLimitEntry> entries, DateTime? nowUtc = null)
    {
        var now = nowUtc ?? DateTime.UtcNow;
        lock (_sync)
        {
            ExchangeLimitUid = exchangeLimitUid;
            _activeEntries.Clear();
            _pendingEntries.Clear();
            foreach (var e in entries)
            {
                if (e.EndUtc <= now) continue;
                if (e.BeginUtc <= now) _activeEntries.Add(e);
                else _pendingEntries.Add(e);
            }
            Changed = false;
        }
    }

    public void Tick(DateTime? nowUtc = null)
    {
        var now = nowUtc ?? DateTime.UtcNow;
        lock (_sync)
        {
            int removed = _activeEntries.RemoveAll(e => e.EndUtc <= now);
            if (removed > 0) Changed = true;

            for (int i = _pendingEntries.Count - 1; i >= 0; i--)
            {
                var p = _pendingEntries[i];
                if (p.BeginUtc <= now)
                {
                    _pendingEntries.RemoveAt(i);
                    if (p.EndUtc > now)
                    {
                        _activeEntries.Add(p);
                        Changed = true;
                    }
                }
            }
        }
    }

    public (ExchangeLimitResult result, int limitUid, int rollBackUid) RetrieveExchangeLimitData(int destItemId)
    {
        lock (_sync)
        {
            if (ExchangeLimitUid == 0)
                return (ExchangeLimitResult.NotInited, 0, 0);

            var result = ExchangeLimitResult.Pass;
            foreach (var entry in _activeEntries)
            {
                if (!entry.DestItemIds.Contains(destItemId))
                    continue;
                if (entry.LimitQuantity <= 0)
                {
                    result = ExchangeLimitResult.Exhausted;
                    continue;
                }

                entry.LimitQuantity--;
                Changed = true;
                int rbUid = _nextRollBackUid++;
                return (ExchangeLimitResult.Succeed, entry.LimitUid, rbUid);
            }
            return (result, 0, 0);
        }
    }

    public bool CheckExchangeLimitUid(int exchangeLimitUid)
    {
        lock (_sync) return ExchangeLimitUid != 0 && ExchangeLimitUid == exchangeLimitUid;
    }

    public bool ReInsertExchangeLimitData(int limitUid)
    {
        lock (_sync)
        {
            var entry = _activeEntries.FirstOrDefault(e => e.LimitUid == limitUid);
            if (entry == null) return false;
            entry.LimitQuantity++;
            Changed = true;
            return true;
        }
    }

    public int GetRemainingQuantity(int destItemId)
    {
        lock (_sync)
        {
            return _activeEntries
                .Where(e => e.DestItemIds.Contains(destItemId))
                .Sum(e => Math.Max(0, e.LimitQuantity));
        }
    }

    public Dictionary<int, int> GetBeginExchangeLimitInfo()
    {
        lock (_sync)
        {
            var map = new Dictionary<int, int>();
            foreach (var entry in _activeEntries)
            {
                foreach (int itemId in entry.DestItemIds)
                    map[itemId] = entry.LimitQuantity;
            }
            return map;
        }
    }
}
