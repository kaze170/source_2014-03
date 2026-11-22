using System.Collections.Concurrent;
using KncWX2Server.Core.Logging;

namespace KncWX2Server.LoginServer;

/// <summary>
/// Faithful C# port of LoginServer/UserList.{h,cpp} &amp; ServerCCUManager.{h,cpp}:
/// tracks active logged-in accounts across all servers, handles duplicate login kicks,
/// and records per-channel CCU snapshots.
/// </summary>
public sealed class KLoginUserList
{
    public sealed class OnlineAccountEntry
    {
        public long UserUid { get; init; }
        public string UserId { get; init; } = string.Empty;
        public long LoginActorUid { get; set; }
        public long GameServerUid { get; set; }
        public int ChannelId { get; set; }
        public DateTime LoginUtc { get; init; } = DateTime.UtcNow;
    }

    public sealed class OnlineUnitEntry
    {
        public long UnitUid { get; init; }
        public long UserUid { get; init; }
        public long GsUid { get; init; }
        public long GsUserUid { get; init; }
        public int ChannelId { get; init; } = 1;
        public string NickName { get; init; } = string.Empty;
    }

    private readonly ConcurrentDictionary<long, OnlineAccountEntry> _byUserUid = new();
    private readonly ConcurrentDictionary<long, OnlineUnitEntry> _byUnitUid = new();
    private readonly ConcurrentDictionary<string, long> _userIdToUid = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, long> _authWaitUsers = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<int, int> _channelCcu = new();

    public int OnlineCount => _byUserUid.Count;

    public bool RegAuthWaitUser(string userId, long gsUserUid)
    {
        if (string.IsNullOrEmpty(userId)) return false;
        return _authWaitUsers.TryAdd(userId, gsUserUid);
    }

    public bool UnRegAuthWaitUser(string userId, out long gsUserUid)
        => !string.IsNullOrEmpty(userId)
            ? _authWaitUsers.TryRemove(userId, out gsUserUid)
            : (gsUserUid = 0) != 0;

    public bool TryGetAccountByUserId(string userId, out OnlineAccountEntry entry)
    {
        if (!string.IsNullOrEmpty(userId) && _userIdToUid.TryGetValue(userId, out long userUid) &&
            _byUserUid.TryGetValue(userUid, out var found))
        {
            entry = found;
            return true;
        }
        entry = null!;
        return false;
    }

    public void RegisterOnlineUnit(long unitUid, long userUid, long gsUid, long gsUserUid, int channelId = 1, string nickName = "")
    {
        if (unitUid <= 0) return;
        _byUnitUid[unitUid] = new OnlineUnitEntry
        {
            UnitUid = unitUid,
            UserUid = userUid,
            GsUid = gsUid,
            GsUserUid = gsUserUid,
            ChannelId = channelId,
            NickName = nickName,
        };
    }

    public void UnregisterOnlineUnit(long unitUid)
    {
        if (unitUid > 0)
            _byUnitUid.TryRemove(unitUid, out _);
    }

    public bool TryGetGSUserInfoByUnitUid(long unitUid, out OnlineUnitEntry info)
        => _byUnitUid.TryGetValue(unitUid, out info!);

    public bool TryGetGSUserInfoByUserUid(long userUid, out OnlineUnitEntry info)
    {
        info = _byUnitUid.Values.FirstOrDefault(u => u.UserUid == userUid)!;
        return info != null;
    }

    public bool TryGetGSUserInfoByNickName(string nickName, out OnlineUnitEntry info)
    {
        info = _byUnitUid.Values.FirstOrDefault(u =>
            string.Equals(u.NickName, nickName, StringComparison.OrdinalIgnoreCase))!;
        return info != null;
    }

    public (bool wasDuplicate, OnlineAccountEntry? previous) RegisterOnline(
        long userUid, string userId, long loginActorUid, int channelId = 1, long gameServerUid = 0)
    {
        OnlineAccountEntry? previous = null;
        bool dup = false;

        if (_byUserUid.TryGetValue(userUid, out var prev) && prev.LoginActorUid != loginActorUid)
        {
            dup = true;
            previous = prev;
            KLog.Cwarn($"[KLoginUserList] Duplicate login detected for UserUID={userUid} ({userId}); kicking previous session {prev.LoginActorUid}");
        }

        var entry = new OnlineAccountEntry
        {
            UserUid = userUid,
            UserId = userId,
            LoginActorUid = loginActorUid,
            ChannelId = channelId,
            GameServerUid = gameServerUid,
        };
        _byUserUid[userUid] = entry;
        if (!string.IsNullOrEmpty(userId))
            _userIdToUid[userId] = userUid;

        return (dup, previous);
    }

    public void UnregisterOnline(long userUid, long loginActorUid)
    {
        if (_byUserUid.TryGetValue(userUid, out var cur) && cur.LoginActorUid == loginActorUid)
        {
            if (_byUserUid.TryRemove(userUid, out var removed) && !string.IsNullOrEmpty(removed.UserId))
                _userIdToUid.TryRemove(removed.UserId, out _);
        }
    }

    public void UpdateChannelCcu(int channelId, int ccu) => _channelCcu[channelId] = ccu;

    public IReadOnlyDictionary<int, int> GetChannelCcuSnapshot() => _channelCcu;
}
