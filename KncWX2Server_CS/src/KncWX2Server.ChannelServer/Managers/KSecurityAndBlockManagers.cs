using KncWX2Server.Core.Database;
using KncWX2Server.Core.Events;
using KncWX2Server.Core.Framework;
using KncWX2Server.Core.Packets;

namespace KncWX2Server.ChannelServer.Managers;

/// <summary>
/// Port of ChannelServer/MachineBlockManager.{h,cpp}.
/// </summary>
public sealed class KMachineBlockManager
{
    private readonly object _lock = new();
    private HashSet<long> _blockedAccounts = [];
    private HashSet<string> _blockedMachines = new(StringComparer.OrdinalIgnoreCase);
    private DateTime _lastRefresh = DateTime.UtcNow;
    private bool _refreshRequested;

    public void Tick(InProcessRouter router)
    {
        if (_refreshRequested && (DateTime.UtcNow - _lastRefresh).TotalSeconds >= 60)
        {
            _refreshRequested = false;
            _lastRefresh = DateTime.UtcNow;
            router.Send(PerformerId.PI_CHANNEL_SERVER, 0, PerformerId.PI_CHANNEL_ACCOUNT_DB, 0,
                EventId.DBE_CHECK_ACCOUNT_BLOCK_LIST_REQ, new KDBE_CHECK_ACCOUNT_BLOCK_LIST_REQ());
        }
    }

    public void UpdateBlockList(Dictionary<int, HashSet<long>> accounts, IEnumerable<string> machines)
    {
        lock (_lock)
        {
            var merged = new HashSet<long>();
            foreach (var set in accounts.Values)
            {
                foreach (long uid in set)
                    merged.Add(uid);
            }
            _blockedAccounts = merged;
            _blockedMachines = new HashSet<string>(machines, StringComparer.OrdinalIgnoreCase);
        }
    }

    public bool IsBlockedAccount(long userUid)
    {
        lock (_lock)
            return _blockedAccounts.Contains(userUid);
    }

    public bool IsBlockedMachine(string machineId, SqliteDatabaseManager? db = null)
    {
        lock (_lock)
        {
            if (_blockedMachines.Contains(machineId))
                return true;
        }
        return db != null && db.IsMachineBlocked(machineId);
    }
}

/// <summary>
/// Port of Common/HackingProtectionManager.{h,cpp}.
/// </summary>
public sealed class KHackingProtectionManager
{
    private readonly object _lock = new();
    private List<KHackingToolInfo> _hackingTools = [];

    public void UpdateHackingToolList(IEnumerable<KHackingToolInfo> tools)
    {
        lock (_lock)
        {
            _hackingTools = tools.ToList();
        }
    }

    public void GetHackingToolList(out int randomKey, out List<KHackingToolInfo> tools)
    {
        randomKey = Random.Shared.Next(1, 10000);
        lock (_lock)
        {
            tools = _hackingTools.ToList();
        }
    }
}
