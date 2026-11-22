namespace KncWX2Server.GameServer;

/// <summary>
/// Faithful C# port of GameServer/UserPartyManager.{h,cpp}:
/// tracks active/candidate/quick-join party UIDs, 20-second kicked-party cooldowns,
/// and 10-second pending party invite timeouts.
/// </summary>
public sealed class KUserPartyManager
{
    public const double KickedCooldownSeconds = 20.0;
    public const double WaitUserTimeoutSeconds = 10.0;

    private readonly Dictionary<long, DateTime> _kickedParties = new();
    private readonly List<(long unitUid, DateTime registeredUtc)> _waitPartyUsers = [];

    public long PartyUid { get; set; }
    public long CandidatePartyUid { get; set; }
    public long QuickJoinPartyUid { get; set; }

    public void Clear()
    {
        PartyUid = 0;
        CandidatePartyUid = 0;
        QuickJoinPartyUid = 0;
        _kickedParties.Clear();
        _waitPartyUsers.Clear();
    }

    public void AddKickedPartyUid(long partyUid, DateTime? nowUtc = null)
    {
        if (_kickedParties.ContainsKey(partyUid)) return;
        _kickedParties[partyUid] = nowUtc ?? DateTime.UtcNow;
    }

    public bool CheckKickedPartyUid(long partyUid) => _kickedParties.ContainsKey(partyUid);

    public void CheckExpiredKickedPartyUid(DateTime? nowUtc = null)
    {
        if (_kickedParties.Count == 0) return;
        var now = nowUtc ?? DateTime.UtcNow;
        foreach (long pid in _kickedParties.Where(kv => (now - kv.Value).TotalSeconds > KickedCooldownSeconds).Select(kv => kv.Key).ToList())
            _kickedParties.Remove(pid);
    }

    public bool RegWaitPartyUser(long unitUid, DateTime? nowUtc = null)
    {
        if (IsWaitPartyUser(unitUid)) return false;
        _waitPartyUsers.Add((unitUid, nowUtc ?? DateTime.UtcNow));
        return true;
    }

    public bool UnregWaitPartyUser(long unitUid)
    {
        if (!IsWaitPartyUser(unitUid)) return false;
        _waitPartyUsers.Clear();
        return true;
    }

    public bool IsWaitPartyUser(long unitUid)
        => _waitPartyUsers.Count > 0 && _waitPartyUsers[0].unitUid == unitUid;

    public bool IsEmptyWaitPartyUser => _waitPartyUsers.Count == 0;

    public void CheckWaitPartyUser(DateTime? nowUtc = null)
    {
        if (_waitPartyUsers.Count == 0) return;
        var now = nowUtc ?? DateTime.UtcNow;
        if (_waitPartyUsers.Any(w => (now - w.registeredUtc).TotalSeconds > WaitUserTimeoutSeconds))
            _waitPartyUsers.Clear();
    }
}
