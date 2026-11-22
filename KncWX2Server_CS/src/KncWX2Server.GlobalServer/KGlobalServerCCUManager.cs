namespace KncWX2Server.GlobalServer;

/// <summary>
/// Faithful C# port of GlobalServer/GlobalServerCCUManager.{h,cpp}:
/// aggregates concurrent user counts across game server groups and tracks total global CCU.
/// </summary>
public sealed class KGlobalServerCCUManager
{
    public sealed record GlobalCcuEntry(long ServerUid, int ServerGroupId, int ConcurrentUser, int CurPartyCount, DateTime UpdatedUtc);

    private readonly Dictionary<long, GlobalCcuEntry> _ccuMap = new();
    private readonly object _sync = new();

    public void UpdateServerCcu(long serverUid, int serverGroupId, int concurrentUser, int curPartyCount = 0, DateTime? nowUtc = null)
    {
        lock (_sync)
        {
            _ccuMap[serverUid] = new GlobalCcuEntry(serverUid, serverGroupId, concurrentUser, curPartyCount, nowUtc ?? DateTime.UtcNow);
        }
    }

    public int GetTotalConcurrentUser()
    {
        lock (_sync) return _ccuMap.Values.Sum(v => v.ConcurrentUser);
    }

    public int GetGroupConcurrentUser(int serverGroupId)
    {
        lock (_sync) return _ccuMap.Values.Where(v => v.ServerGroupId == serverGroupId).Sum(v => v.ConcurrentUser);
    }

    public IReadOnlyList<GlobalCcuEntry> Snapshot()
    {
        lock (_sync) return _ccuMap.Values.ToList();
    }
}
