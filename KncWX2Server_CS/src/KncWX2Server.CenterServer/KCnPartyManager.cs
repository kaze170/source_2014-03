using System.Collections.Concurrent;
using KncWX2Server.Core.Packets;

namespace KncWX2Server.CenterServer;

/// <summary>
/// Faithful C# port of CenterServer/CnParty.{h,cpp} &amp; CnPartyFeverManager.{h,cpp}:
/// integrates KCnPartyFSM, Party Fever gauge (0..10000), ready state, dungeon change, and game start checks.
/// </summary>
public sealed class KCnParty
{
    public const int MaxFeverPoint = 10_000;
    private readonly object _lock = new();

    public KCnPartyFSM Fsm { get; } = new();
    public KCnPartyFeverManager PartyFeverManager { get; } = new();
    public KPartyInfo Info { get; }
    public List<KPartyUserInfo> Members { get; } = new();
    public int FeverPoint { get; private set; }
    public bool IsFeverTime { get; private set; }
    public DateTime FeverExpireUtc { get; private set; } = DateTime.MinValue;

    public KCnParty(long partyUid, KCreatePartyInfo template, KPartyUserInfo host)
    {
        Info = new KPartyInfo
        {
            PartyUid = partyUid,
            PartyType = template.PartyType,
            PartyName = $"{host.NickName}'s Party",
            JoinSlot = 1,
            MaxSlot = 4,
            Public = true,
            DungeonId = template.DungeonId,
            DifficultyLevel = template.DifficultyLevel,
            GetItemType = template.GetItemType,
            DungeonMode = template.DungeonMode,
            CheckLowLevel = template.CheckLowLevel,
            PvpGameType = template.PvpGameType,
        };
        host.Host = true;
        host.Ready = true;
        Members.Add(host);
        Fsm.TryStateTransition(KCnPartyFSM.I_TO_WAIT, out _);
    }

    public bool TryJoin(KPartyUserInfo user)
    {
        lock (_lock)
        {
            if (Members.Count >= Info.MaxSlot || Members.Any(m => m.UnitUid == user.UnitUid))
                return false;
            user.Host = false;
            user.Ready = false;
            Members.Add(user);
            Info.JoinSlot = (sbyte)Members.Count;
            return true;
        }
    }

    public (bool removed, bool empty, long newHostUid) Leave(long unitUid)
    {
        lock (_lock)
        {
            int idx = Members.FindIndex(m => m.UnitUid == unitUid);
            if (idx < 0) return (false, false, 0);
            bool wasHost = Members[idx].Host;
            Members.RemoveAt(idx);
            Info.JoinSlot = (sbyte)Members.Count;
            if (Members.Count == 0)
            {
                Fsm.TryStateTransition(KCnPartyFSM.I_TO_CLOSE, out _);
                return (true, true, 0);
            }

            long newHost = Members.First(m => m.Host || !wasHost).UnitUid;
            if (wasHost)
            {
                Members[0].Host = true;
                Members[0].Ready = true;
                newHost = Members[0].UnitUid;
            }
            return (true, false, newHost);
        }
    }

    public bool SetReady(long unitUid, bool ready)
    {
        lock (_lock)
        {
            var m = Members.FirstOrDefault(x => x.UnitUid == unitUid);
            if (m == null) return false;
            m.Ready = m.Host || ready;
            return true;
        }
    }

    public bool ChangeDungeon(long requesterUnitUid, int dungeonId, sbyte difficulty, sbyte mode)
    {
        lock (_lock)
        {
            var host = Members.FirstOrDefault(m => m.Host);
            if (host == null || host.UnitUid != requesterUnitUid) return false;
            Info.DungeonId = dungeonId;
            Info.DifficultyLevel = difficulty;
            Info.DungeonMode = mode;
            foreach (var m in Members)
                m.Ready = m.Host;
            return true;
        }
    }

    public bool ChangePartyType(long requesterUnitUid, KPartyInfo template)
    {
        lock (_lock)
        {
            var host = Members.FirstOrDefault(m => m.Host);
            if (host == null || host.UnitUid != requesterUnitUid) return false;
            Info.PartyType = template.PartyType;
            Info.PartyName = template.PartyName;
            Info.Public = template.Public;
            Info.DungeonId = template.DungeonId;
            Info.DifficultyLevel = template.DifficultyLevel;
            Info.GetItemType = template.GetItemType;
            Info.DungeonMode = template.DungeonMode;
            Info.CheckLowLevel = template.CheckLowLevel;
            Info.PvpGameType = template.PvpGameType;
            foreach (var m in Members)
                m.Ready = m.Host;
            return true;
        }
    }

    public bool ChangePvpMatchInfo(long requesterUnitUid, sbyte pvpGameType)
    {
        lock (_lock)
        {
            var host = Members.FirstOrDefault(m => m.Host);
            if (host == null || host.UnitUid != requesterUnitUid) return false;
            Info.PvpGameType = pvpGameType;
            foreach (var m in Members)
                m.Ready = m.Host;
            return true;
        }
    }

    public bool CanStartGame(long requesterUnitUid)
    {
        lock (_lock)
        {
            var host = Members.FirstOrDefault(m => m.Host);
            if (host == null || host.UnitUid != requesterUnitUid) return false;
            if (!Members.All(m => m.Host || m.Ready)) return false;
            Fsm.TryStateTransition(KCnPartyFSM.I_TO_PREPARE_FOR_PLAY, out _);
            return true;
        }
    }

    public void ReturnToWait()
    {
        lock (_lock)
            Fsm.TryStateTransition(KCnPartyFSM.I_TO_WAIT, out _);
    }

    public bool CapFeverAt100Percent()
    {
        lock (_lock)
        {
            if (FeverPoint <= MaxFeverPoint) return false;
            FeverPoint = MaxFeverPoint;
            return true;
        }
    }

    public bool ChangeHost(long requesterUnitUid, long targetUnitUid, out long beforeUid, out long afterUid)
    {
        lock (_lock)
        {
            beforeUid = 0;
            afterUid = 0;
            var host = Members.FirstOrDefault(m => m.Host);
            var target = Members.FirstOrDefault(m => m.UnitUid == targetUnitUid);
            if (host == null || host.UnitUid != requesterUnitUid || target == null || target.UnitUid == requesterUnitUid)
                return false;
            beforeUid = host.UnitUid;
            afterUid = target.UnitUid;
            host.Host = false;
            target.Host = true;
            target.Ready = true;
            return true;
        }
    }

    public bool BanUser(long requesterUnitUid, long targetUnitUid)
    {
        lock (_lock)
        {
            var host = Members.FirstOrDefault(m => m.Host);
            if (host == null || host.UnitUid != requesterUnitUid || targetUnitUid == requesterUnitUid)
                return false;
            int idx = Members.FindIndex(m => m.UnitUid == targetUnitUid);
            if (idx < 0) return false;
            Members.RemoveAt(idx);
            Info.JoinSlot = (sbyte)Members.Count;
            return true;
        }
    }

    public bool ChangeMaxSlot(long requesterUnitUid, sbyte maxSlot)
    {
        lock (_lock)
        {
            var host = Members.FirstOrDefault(m => m.Host);
            if (host == null || host.UnitUid != requesterUnitUid) return false;
            if (maxSlot < Members.Count || maxSlot > 4) return false;
            Info.MaxSlot = maxSlot;
            return true;
        }
    }

    public bool ChangeGetItemType(long requesterUnitUid, sbyte getItemType)
    {
        lock (_lock)
        {
            var host = Members.FirstOrDefault(m => m.Host);
            if (host == null || host.UnitUid != requesterUnitUid) return false;
            Info.GetItemType = getItemType;
            return true;
        }
    }

    public bool SetLimitLevel(long requesterUnitUid, bool checkLowLevel)
    {
        lock (_lock)
        {
            var host = Members.FirstOrDefault(m => m.Host);
            if (host == null || host.UnitUid != requesterUnitUid) return false;
            Info.CheckLowLevel = checkLowLevel;
            return true;
        }
    }

    public (int feverPoint, bool feverTriggered) AddFever(int delta)
    {
        lock (_lock)
        {
            if (IsFeverTime && DateTime.UtcNow < FeverExpireUtc)
                return (FeverPoint, false);

            IsFeverTime = false;
            FeverPoint = Math.Clamp(FeverPoint + Math.Max(0, delta), 0, MaxFeverPoint);
            if (FeverPoint >= MaxFeverPoint)
            {
                IsFeverTime = true;
                FeverExpireUtc = DateTime.UtcNow.AddSeconds(60);
                FeverPoint = 0;
                return (FeverPoint, true);
            }
            return (FeverPoint, false);
        }
    }

    public List<long> GetUnitUids()
    {
        lock (_lock)
            return Members.Select(m => m.UnitUid).ToList();
    }

    public List<KPartyUserInfo> SnapshotMembers()
    {
        lock (_lock)
            return Members.ToList();
    }

    public bool TryExpireFever(out int feverPoint, out List<long> memberUids, DateTime? nowUtc = null)
    {
        var now = nowUtc ?? DateTime.UtcNow;
        lock (_lock)
        {
            if (IsFeverTime && now >= FeverExpireUtc)
            {
                IsFeverTime = false;
                feverPoint = FeverPoint;
                memberUids = Members.Select(m => m.UnitUid).ToList();
                return true;
            }
            feverPoint = FeverPoint;
            memberUids = [];
            return false;
        }
    }

    public int IncreaseFeverByAdmin(int delta = 1000)
    {
        lock (_lock)
        {
            var (_, display) = PartyFeverManager.UpdateFeverPoint(FeverPoint + Math.Max(1, delta), Math.Max(2, Members.Count));
            FeverPoint = Math.Min(MaxFeverPoint, display);
            if (FeverPoint >= KCnPartyFeverManager.Fever100PercentPoint)
            {
                IsFeverTime = true;
                FeverExpireUtc = DateTime.UtcNow.AddMinutes(3);
            }
            return FeverPoint;
        }
    }
}

public sealed class KCnPartyManager
{
    private readonly ConcurrentDictionary<long, KCnParty> _parties = new();
    private long _nextPartyUid = 500_000;

    public int PartyCount => _parties.Count;

    public KCnParty CreateParty(KCreatePartyInfo info, KPartyUserInfo host)
    {
        long puid = Interlocked.Increment(ref _nextPartyUid);
        var party = new KCnParty(puid, info, host);
        _parties[puid] = party;
        return party;
    }

    public KCnParty? GetParty(long partyUid) => _parties.TryGetValue(partyUid, out var p) ? p : null;

    public bool TryJoinParty(long partyUid, KPartyUserInfo user, out KCnParty? party)
    {
        if (_parties.TryGetValue(partyUid, out party) && party.TryJoin(user))
            return true;
        party = null;
        return false;
    }

    public (bool removed, long newHostUid) LeaveParty(long partyUid, long unitUid)
    {
        if (!_parties.TryGetValue(partyUid, out var party))
            return (false, 0);
        var (removed, empty, newHost) = party.Leave(unitUid);
        if (empty)
            _parties.TryRemove(partyUid, out _);
        return (removed, newHost);
    }

    public IReadOnlyCollection<KCnParty> GetPartiesSnapshot() => _parties.Values.ToArray();

    public KCnParty CreateParty(KPartyInfo template, KPartyUserInfo host)
    {
        long puid = Interlocked.Increment(ref _nextPartyUid);
        var createInfo = new KCreatePartyInfo
        {
            PartyType = template.PartyType,
            DungeonId = template.DungeonId,
            DifficultyLevel = template.DifficultyLevel,
            GetItemType = template.GetItemType,
            DungeonMode = template.DungeonMode,
            CheckLowLevel = template.CheckLowLevel,
            PvpGameType = template.PvpGameType,
        };
        var party = new KCnParty(puid, createInfo, host);
        if (!string.IsNullOrWhiteSpace(template.PartyName))
            party.Info.PartyName = template.PartyName;
        if (template.MaxSlot > 0)
            party.Info.MaxSlot = template.MaxSlot;
        party.Info.Public = template.Public;
        _parties[puid] = party;
        return party;
    }

    public bool ChangePartyName(long partyUid, long requesterUnitUid, string newName)
    {
        if (!_parties.TryGetValue(partyUid, out var party)) return false;
        var host = party.Members.FirstOrDefault(m => m.Host);
        if (host == null || host.UnitUid != requesterUnitUid) return false;
        party.Info.PartyName = newName;
        return true;
    }

    public bool ChangePartyPublic(long partyUid, long requesterUnitUid, bool isPublic)
    {
        if (!_parties.TryGetValue(partyUid, out var party)) return false;
        var host = party.Members.FirstOrDefault(m => m.Host);
        if (host == null || host.UnitUid != requesterUnitUid) return false;
        party.Info.Public = isPublic;
        return true;
    }

    public KCnParty? QuickJoinParty(int dungeonId, sbyte difficulty, sbyte mode, KPartyUserInfo joiner)
    {
        foreach (var p in _parties.Values)
        {
            if (!p.Info.Public) continue;
            if (p.Info.DungeonId != dungeonId || p.Info.DifficultyLevel != difficulty || p.Info.DungeonMode != mode) continue;
            if (p.TryJoin(joiner))
                return p;
        }
        return null;
    }

    public KCnParty? GetPartyByUnitUid(long unitUid)
        => unitUid <= 0 ? null : _parties.Values.FirstOrDefault(p => p.GetUnitUids().Contains(unitUid));

    public KCnParty? ResolveParty(long explicitPartyUid, long unitUid)
        => GetParty(explicitPartyUid) ?? GetPartyByUnitUid(unitUid);

    public bool ChangeHost(long partyUid, long oldHostUid, long newHostUid)
    {
        var party = ResolveParty(partyUid, oldHostUid);
        if (party == null) return false;
        var list = party.Members;
        lock (list)
        {
            var oldH = list.FirstOrDefault(m => m.UnitUid == oldHostUid);
            var newH = list.FirstOrDefault(m => m.UnitUid == newHostUid);
            if (newH == null) return false;
            if (oldH != null) oldH.Host = false;
            newH.Host = true;
            newH.Ready = true;
            return true;
        }
    }

    public bool UpdateMemberPvpInfo(long unitUid, sbyte rank, int officialMatchCnt, int rating, int rPoint)
    {
        var party = GetPartyByUnitUid(unitUid);
        if (party == null) return false;
        var m = party.Members.FirstOrDefault(x => x.UnitUid == unitUid);
        if (m == null) return false;
        m.Rank = rank;
        m.OfficialMatchCnt = officialMatchCnt;
        m.Rating = rating;
        m.RPoint = rPoint;
        return true;
    }

    public bool UpdateMemberJoinRoom(long unitUid, sbyte state, int stateCode)
    {
        var party = GetPartyByUnitUid(unitUid);
        if (party == null) return false;
        var m = party.Members.FirstOrDefault(x => x.UnitUid == unitUid);
        if (m == null) return false;
        m.State = state;
        m.StateCode = stateCode;
        return true;
    }

    public List<(long PartyUid, List<long> MemberUids, int FeverPoint)> TickFeverExpirations(DateTime? nowUtc = null)
    {
        var expired = new List<(long, List<long>, int)>();
        var now = nowUtc ?? DateTime.UtcNow;
        foreach (var party in _parties.Values)
        {
            if (party.TryExpireFever(out int feverPoint, out var members, now))
                expired.Add((party.Info.PartyUid, members, feverPoint));
        }
        return expired;
    }
}
