using System.Collections.Concurrent;
using KncWX2Server.Core.Database;
using KncWX2Server.Core.Logging;
using KncWX2Server.Core.Region;

namespace KncWX2Server.LoginServer;

/// <summary>
/// Faithful C# port of LoginServer/AccountBlockManager.{h,cpp}:
/// periodically refreshes blocked accounts/types from DB and checks connect/trade/chat/post blocks.
/// </summary>
public sealed class KAccountBlockManager
{
    public enum BlockType
    {
        Connect = 0,
        Trade = 1,
        Chat = 2,
        Post = 3,
        PersonalShop = 4,
    }

    private readonly ConcurrentDictionary<(string UserId, int BlockType), string> _blocks = new();
    private DateTime _lastRefreshUtc = DateTime.MinValue;
    public TimeSpan RefreshInterval { get; set; } = TimeSpan.FromSeconds(60);

    public int Count => _blocks.Count;

    public void Tick(SqliteDatabaseManager db, RegionProfile region)
    {
        if (DateTime.UtcNow - _lastRefreshUtc < RefreshInterval) return;
        RefreshNow(db, region);
    }

    public void RefreshNow(SqliteDatabaseManager db, RegionProfile region)
    {
        _lastRefreshUtc = DateTime.UtcNow;
        var entries = db.GetAccountBlockList(region);
        var nextKeys = new HashSet<(string, int)>();
        foreach (var (userId, blockType, reason) in entries)
        {
            var key = (userId, blockType);
            nextKeys.Add(key);
            _blocks[key] = reason;
        }

        foreach (var existing in _blocks.Keys)
        {
            if (!nextKeys.Contains(existing))
                _blocks.TryRemove(existing, out _);
        }
    }

    public bool IsBlocked(string userId, BlockType type, out string reason)
    {
        if (_blocks.TryGetValue((userId, (int)type), out var r))
        {
            reason = r;
            return true;
        }
        reason = string.Empty;
        return false;
    }

    public void SetBlock(SqliteDatabaseManager db, RegionProfile region, string userId, BlockType type, bool blocked, string reason = "")
    {
        db.SetAccountBlock(region, userId, (int)type, blocked, reason);
        var key = (userId, (int)type);
        if (blocked)
        {
            _blocks[key] = reason;
            KLog.Cwarn($"[AccountBlockManager] Blocked {userId} ({type}): {reason}");
        }
        else
        {
            _blocks.TryRemove(key, out _);
            KLog.Cout($"[AccountBlockManager] Unblocked {userId} ({type})");
        }
    }
}
