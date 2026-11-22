namespace KncWX2Server.LoginServer;

/// <summary>
/// Faithful C# port of LoginServer/ServerCCUManager.{h,cpp}:
/// collects per-server concurrent user/party counts, aggregates total CCU,
/// and throttles/deduplicates DB updates per minute.
/// </summary>
public sealed class KServerCCUManager
{
    public sealed class ServerCcuInfo
    {
        public long ServerUid { get; set; }
        public int ConcurrentUser { get; set; }
        public int CurPartyCount { get; set; }
        public int PlayGamePartyCount { get; set; }
        public int GuestConcurrentUser { get; set; }
    }

    private readonly Dictionary<long, ServerCcuInfo> _ccuMap = new();
    private readonly object _sync = new();
    private DateTime _lastDbUpdateUtc = DateTime.MinValue;

    public void UpdateCcuInfo(ServerCcuInfo info)
    {
        lock (_sync)
        {
            _ccuMap[info.ServerUid] = info;
        }
    }

    public (bool emitted, ServerCcuInfo total, List<ServerCcuInfo> snapshot) TryBuildDbUpdate(DateTime? nowUtc = null)
    {
        var now = nowUtc ?? DateTime.UtcNow;
        lock (_sync)
        {
            if (_lastDbUpdateUtc != DateTime.MinValue &&
                now.Year == _lastDbUpdateUtc.Year &&
                now.Month == _lastDbUpdateUtc.Month &&
                now.Day == _lastDbUpdateUtc.Day &&
                now.Hour == _lastDbUpdateUtc.Hour &&
                now.Minute == _lastDbUpdateUtc.Minute)
            {
                return (false, new ServerCcuInfo(), []);
            }

            _lastDbUpdateUtc = now;
            var total = new ServerCcuInfo { ServerUid = 0 };
            var snapshot = new List<ServerCcuInfo>(_ccuMap.Count + 1);
            foreach (var entry in _ccuMap.Values)
            {
                snapshot.Add(entry);
                total.ConcurrentUser += entry.ConcurrentUser;
                total.CurPartyCount += entry.CurPartyCount;
                total.PlayGamePartyCount += entry.PlayGamePartyCount;
                total.GuestConcurrentUser += entry.GuestConcurrentUser;
            }
            snapshot.Add(total);
            return (true, total, snapshot);
        }
    }

    public int GetTotalConcurrentUser()
    {
        lock (_sync) return _ccuMap.Values.Sum(x => x.ConcurrentUser);
    }
}
