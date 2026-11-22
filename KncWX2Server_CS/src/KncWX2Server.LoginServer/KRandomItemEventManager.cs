namespace KncWX2Server.LoginServer;

/// <summary>
/// Faithful C# port of LoginServer/RandomItemEventManager.{h,cpp} (SERV_TIME_OPEN_RANDOM_ITEM_EVENT):
/// time-windowed cube/random-box bonus reward tracking with per-account one-time claim enforcement.
/// </summary>
public sealed class KRandomItemEventManager
{
    public sealed class RandomItemEventEntry
    {
        public int EventId { get; init; }
        public int RewardItemId { get; init; }
        public int OpenItemId { get; init; } // 0 = any cube
        public DateTime DropStartUtc { get; init; }
        public DateTime DropEndUtc { get; init; }
    }

    private readonly List<RandomItemEventEntry> _events = new();
    private readonly HashSet<long> _rewardedUserUids = [];
    private readonly object _sync = new();

    public void UpdateEventInfo(IEnumerable<RandomItemEventEntry> events, IEnumerable<long> rewardedUserUids)
    {
        lock (_sync)
        {
            _events.Clear();
            _events.AddRange(events);
            _rewardedUserUids.Clear();
            foreach (long u in rewardedUserUids)
                _rewardedUserUids.Add(u);
        }
    }

    public int CheckTimeOpenRandomItemEvent(long userUid, long unitUid, int openItemId, DateTime? nowUtc = null)
    {
        var now = nowUtc ?? DateTime.UtcNow;
        lock (_sync)
        {
            if (_rewardedUserUids.Contains(userUid))
                return 0;

            foreach (var ev in _events)
            {
                if (now < ev.DropStartUtc || now > ev.DropEndUtc)
                    continue;
                if (ev.OpenItemId != 0 && ev.OpenItemId != openItemId)
                    continue;

                _rewardedUserUids.Add(userUid);
                return ev.RewardItemId;
            }
            return 0;
        }
    }
}
