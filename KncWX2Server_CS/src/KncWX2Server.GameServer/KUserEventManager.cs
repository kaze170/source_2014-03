namespace KncWX2Server.GameServer;

/// <summary>
/// Faithful C# port of GameServer/UserEventManager.{h,cpp}:
/// tracks cumulative login-duration events (minutes -> seconds), scheduled connect-time events,
/// and completed one-time event UIDs per character/account.
/// </summary>
public sealed class KUserEventManager
{
    public const double EndEventMarker = -1.0;

    public sealed class CumulativeTimeEventEntry
    {
        public int EventUid { get; init; }
        public double TargetDurationSeconds { get; init; }
        public double AccumulatedSeconds { get; set; }
        public bool Completed { get; set; }
    }

    public sealed class ConnectTimeEventEntry
    {
        public int EventUid { get; init; }
        public DateTime LastRewardUtc { get; set; }
        public double EventTimeSeconds { get; set; }
        public bool GiveOnce { get; init; } = true;
    }

    private readonly Dictionary<int, CumulativeTimeEventEntry> _cumulativeEvents = new();
    private readonly Dictionary<int, ConnectTimeEventEntry> _connectEvents = new();
    private readonly HashSet<int> _completedEvents = [];

    public void Clear()
    {
        _cumulativeEvents.Clear();
        _connectEvents.Clear();
        _completedEvents.Clear();
    }

    public void MarkCompleted(int eventUid) => _completedEvents.Add(eventUid);
    public bool IsCompleted(int eventUid) => _completedEvents.Contains(eventUid);

    public void RegisterCumulativeEvent(int eventUid, int initialMinutes, int targetMinutes)
    {
        if (_completedEvents.Contains(eventUid)) return;
        _cumulativeEvents[eventUid] = new CumulativeTimeEventEntry
        {
            EventUid = eventUid,
            AccumulatedSeconds = initialMinutes * 60.0,
            TargetDurationSeconds = targetMinutes * 60.0,
        };
    }

    public void RegisterConnectTimeEvent(int eventUid, DateTime lastRewardUtc, double intervalSeconds, bool giveOnce = true)
    {
        if (giveOnce && _completedEvents.Contains(eventUid)) return;
        _connectEvents[eventUid] = new ConnectTimeEventEntry
        {
            EventUid = eventUid,
            LastRewardUtc = lastRewardUtc,
            EventTimeSeconds = intervalSeconds,
            GiveOnce = giveOnce,
        };
    }

    public List<int> AdvanceTime(double elapsedSeconds, DateTime? nowUtcOverride = null)
    {
        var now = nowUtcOverride ?? DateTime.UtcNow;
        var triggered = new List<int>();

        foreach (var ev in _cumulativeEvents.Values)
        {
            if (ev.Completed || _completedEvents.Contains(ev.EventUid)) continue;
            ev.AccumulatedSeconds += elapsedSeconds;
            if (ev.AccumulatedSeconds >= ev.TargetDurationSeconds)
            {
                ev.Completed = true;
                _completedEvents.Add(ev.EventUid);
                triggered.Add(ev.EventUid);
            }
        }

        foreach (var ev in _connectEvents.Values)
        {
            if (ev.EventTimeSeconds <= 0 || _completedEvents.Contains(ev.EventUid)) continue;
            if ((now - ev.LastRewardUtc).TotalSeconds >= ev.EventTimeSeconds)
            {
                ev.LastRewardUtc = now;
                if (ev.GiveOnce)
                    _completedEvents.Add(ev.EventUid);
                triggered.Add(ev.EventUid);
            }
        }

        return triggered;
    }

    public Dictionary<int, int> SnapshotCumulativeMinutes()
        => _cumulativeEvents.ToDictionary(k => k.Key, v => (int)(v.Value.AccumulatedSeconds / 60.0));
}
