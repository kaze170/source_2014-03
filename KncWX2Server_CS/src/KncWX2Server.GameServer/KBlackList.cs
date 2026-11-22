namespace KncWX2Server.GameServer;

/// <summary>
/// Faithful C# port of GameServer/BlackList.{h,cpp}:
/// social deny preferences (BL_ON, BL_OFF, BL_ONLY_FRIEND), 20-user blacklist cap,
/// 2s add/del/opt cooldowns, and UnitUID/NickName verification.
/// </summary>
public sealed class KBlackList
{
    public const int MaxBlacklistUser = 20;
    public const double LimitSeconds = 2.0;

    public enum BlackListDenyState : sbyte
    {
        BlOn = 0,
        BlOff = 1,
        BlOnlyFriend = 2,
    }

    private readonly Dictionary<long, string> _blockedUsers = new();
    private readonly object _sync = new();
    private DateTime _lastAddUtc = DateTime.MinValue;
    private DateTime _lastDelUtc = DateTime.MinValue;
    private DateTime _lastOptUtc = DateTime.MinValue;

    public BlackListDenyState InsertFriendDeny { get; private set; } = BlackListDenyState.BlOff;
    public BlackListDenyState ManToManChatDeny { get; set; } = BlackListDenyState.BlOff;
    public BlackListDenyState PartyDeny { get; set; } = BlackListDenyState.BlOff;
    public BlackListDenyState PrivateDealDeny { get; set; } = BlackListDenyState.BlOff;

    public bool SetInsertFriendDeny(BlackListDenyState stateVal)
    {
        InsertFriendDeny = stateVal;
        return true;
    }

    public bool IsAddPossible(DateTime? nowUtc = null)
        => ((nowUtc ?? DateTime.UtcNow) - _lastAddUtc).TotalSeconds >= LimitSeconds && _blockedUsers.Count < MaxBlacklistUser;

    public bool IsDelPossible(DateTime? nowUtc = null)
        => ((nowUtc ?? DateTime.UtcNow) - _lastDelUtc).TotalSeconds >= LimitSeconds;

    public bool IsUpdateOptPossible(DateTime? nowUtc = null)
    {
        var now = nowUtc ?? DateTime.UtcNow;
        if ((now - _lastOptUtc).TotalSeconds < LimitSeconds) return false;
        _lastOptUtc = now;
        return true;
    }

    public bool AddNewBlackListUser(long unitUid, string nickName, DateTime? nowUtc = null)
    {
        lock (_sync)
        {
            if (_blockedUsers.Count >= MaxBlacklistUser) return false;
            _blockedUsers[unitUid] = nickName;
            _lastAddUtc = nowUtc ?? DateTime.UtcNow;
            return true;
        }
    }

    public bool DeleteBlackListUser(string nickName, out long removedUnitUid, DateTime? nowUtc = null)
    {
        lock (_sync)
        {
            foreach (var kv in _blockedUsers)
            {
                if (string.Equals(kv.Value, nickName, StringComparison.OrdinalIgnoreCase))
                {
                    removedUnitUid = kv.Key;
                    _blockedUsers.Remove(kv.Key);
                    _lastDelUtc = nowUtc ?? DateTime.UtcNow;
                    return true;
                }
            }
            removedUnitUid = 0;
            return false;
        }
    }

    public bool VerifyBlackListUser(long unitUid)
    {
        lock (_sync) return _blockedUsers.ContainsKey(unitUid);
    }

    public bool VerifyBlackListUser(string nickName)
    {
        lock (_sync)
            return _blockedUsers.Values.Any(v => string.Equals(v, nickName, StringComparison.OrdinalIgnoreCase));
    }

    public IReadOnlyDictionary<long, string> GetBlackListSnapshot()
    {
        lock (_sync) return new Dictionary<long, string>(_blockedUsers);
    }
}
