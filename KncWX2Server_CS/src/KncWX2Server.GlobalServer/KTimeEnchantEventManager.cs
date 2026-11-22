namespace KncWX2Server.GlobalServer;

/// <summary>
/// Faithful C# port of KGlobalServer TimeEnchantEvent system:
/// manages timed global enchant-level bonus slots and automatic window transitions.
/// </summary>
public sealed class KTimeEnchantEventManager
{
    public sealed class TimeEnchantEventEntry
    {
        public int EventUid { get; init; }
        public sbyte EnchantLevel { get; init; }
        public bool NotifyGetItem { get; init; } = true;
        public DateTime BeginUtc { get; init; }
        public DateTime EndUtc { get; init; }
    }

    private readonly object _sync = new();
    private readonly Dictionary<int, TimeEnchantEventEntry> _active = new();
    private readonly List<TimeEnchantEventEntry> _pending = new();

    public int TimeEnchantEventUid { get; private set; } = 1;

    public void Initialize(int timeEnchantEventUid, IEnumerable<TimeEnchantEventEntry> entries, DateTime? nowUtc = null)
    {
        var now = nowUtc ?? DateTime.UtcNow;
        lock (_sync)
        {
            TimeEnchantEventUid = timeEnchantEventUid;
            _active.Clear();
            _pending.Clear();
            foreach (var e in entries)
            {
                if (e.EndUtc <= now) continue;
                if (e.BeginUtc <= now) _active[e.EventUid] = e;
                else _pending.Add(e);
            }
        }
    }

    public void Tick(DateTime? nowUtc = null)
    {
        var now = nowUtc ?? DateTime.UtcNow;
        lock (_sync)
        {
            foreach (int expiredId in _active.Where(kv => kv.Value.EndUtc <= now).Select(kv => kv.Key).ToList())
                _active.Remove(expiredId);

            for (int i = _pending.Count - 1; i >= 0; i--)
            {
                var p = _pending[i];
                if (p.BeginUtc <= now)
                {
                    _pending.RemoveAt(i);
                    if (p.EndUtc > now)
                        _active[p.EventUid] = p;
                }
            }
        }
    }

    public bool TryRetrieve(out TimeEnchantEventEntry? entry, out int eventGroupUid, DateTime? nowUtc = null)
    {
        Tick(nowUtc);
        lock (_sync)
        {
            if (_active.Count == 0)
            {
                entry = null;
                eventGroupUid = TimeEnchantEventUid;
                return false;
            }
            var first = _active.OrderBy(kv => kv.Key).First();
            entry = first.Value;
            eventGroupUid = TimeEnchantEventUid;
            _active.Remove(first.Key);
            return true;
        }
    }

    public int ActiveCount
    {
        get { lock (_sync) return _active.Count; }
    }

    public int GetNumOfBeginTimeEnchantEventInfo(DateTime? nowUtc = null)
    {
        Tick(nowUtc);
        lock (_sync) return _active.Count;
    }
}
