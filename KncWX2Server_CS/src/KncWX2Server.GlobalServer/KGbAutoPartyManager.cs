using System.Collections.Concurrent;
using KncWX2Server.Core.Scripting;
using MoonSharp.Interpreter;

namespace KncWX2Server.GlobalServer;

public sealed record AutoPartyEntry(
    long UnitUid,
    string NickName,
    int DungeonId,
    sbyte DifficultyLevel,
    DateTime EnqueuedUtc,
    sbyte DungeonMode = 0,
    long WaitNumber = 0);

public sealed record AutoPartyGroup(
    long AutoPartyUid,
    int DungeonId,
    sbyte DifficultyLevel,
    IReadOnlyList<AutoPartyEntry> Members,
    sbyte DungeonMode = 0);

/// <summary>
/// Managed port of KGlobalServer AutoPartyManager (cross-server dungeon queue + KAutoPartyFSM accept/decline tracking).
/// </summary>
public sealed class KGbAutoPartyManager
{
    private sealed class PendingAutoPartyState
    {
        public AutoPartyGroup Group { get; init; } = default!;
        public KAutoPartyFSM Fsm { get; } = new();
        public HashSet<long> AgreedUnitUids { get; } = new();
    }

    private readonly object _lock = new();
    private readonly List<AutoPartyEntry> _queue = new();
    private readonly ConcurrentQueue<AutoPartyGroup> _formed = new();
    private readonly Dictionary<long, PendingAutoPartyState> _pendingByParty = new();
    private readonly Dictionary<long, long> _unitToPartyUid = new();
    private readonly Dictionary<(sbyte mode, int dungeonId), bool> _autoPartyBonus = new();
    private readonly List<(sbyte Mode, int DungeonId, int MinLevel, int MaxLevel)> _autoPartyDungeonRanges = new();
    private long _nextAutoPartyUid = 800_000;

    public int SameDifficultyLevelWaitTime { get; set; } = 20;
    public int SameUnitTypeWaitTime { get; set; } = 10;
    public int DirectGameStartWaitTime { get; set; } = 120;
    public int AutoPartyBonusTime { get; set; } = 120;
    public int AutoPartyMakingBeginCount { get; set; } = 1;
    public int CheckWaitUserCount { get; set; } = 10;
    public int AutoPartyMatchingSuccessNum { get; set; } = 4;
    public bool IsAllMemberCheckSameUnitTypeTime { get; set; } = true;
    public bool IsAllMemberCheckSameDifficultyLevelTime { get; set; } = true;
    public bool IsAllMemberCheckDirectGameStartTime { get; set; }
    public IReadOnlyList<(sbyte Mode, int DungeonId, int MinLevel, int MaxLevel)> AutoPartyDungeonRanges => _autoPartyDungeonRanges;

    public void SetAutoPartyBonus(sbyte dungeonMode, int dungeonId, bool activatedBonus)
    {
        lock (_lock)
            _autoPartyBonus[(dungeonMode, dungeonId)] = activatedBonus;
    }

    public bool IsAutoPartyBonus(sbyte dungeonMode, int dungeonId)
    {
        lock (_lock)
            return _autoPartyBonus.TryGetValue((dungeonMode, dungeonId), out bool active) && active;
    }

    public List<KncWX2Server.Core.Packets.KAutoPartyBonusInfo> GetAllAutoPartyBonusInfos()
    {
        lock (_lock)
        {
            return _autoPartyBonus
                .Select(kv => new KncWX2Server.Core.Packets.KAutoPartyBonusInfo
                {
                    DungeonMode = kv.Key.mode,
                    DungeonIdWithDif = kv.Key.dungeonId,
                    ActivatedBonus = kv.Value,
                })
                .ToList();
        }
    }

    public int QueueCount
    {
        get { lock (_lock) return _queue.Count; }
    }

    public int PendingCount
    {
        get { lock (_lock) return _pendingByParty.Count; }
    }

    public void Enqueue(AutoPartyEntry entry, int minPartySize = 2, int maxPartySize = 4, bool remakingAutoParty = false)
    {
        lock (_lock)
        {
            _queue.RemoveAll(e => e.UnitUid == entry.UnitUid);
            if (remakingAutoParty && _queue.Count > 0)
                _queue.Insert(0, entry);
            else
                _queue.Add(entry);

            var sameDungeon = _queue
                .Where(q => q.DungeonId == entry.DungeonId && q.DifficultyLevel == entry.DifficultyLevel)
                .ToList();
            if (sameDungeon.Count >= minPartySize)
            {
                var members = sameDungeon.Take(maxPartySize).ToList();
                foreach (var m in members)
                    _queue.RemoveAll(x => x.UnitUid == m.UnitUid);

                long uid = Interlocked.Increment(ref _nextAutoPartyUid);
                var group = new AutoPartyGroup(uid, entry.DungeonId, entry.DifficultyLevel, members, entry.DungeonMode);

                var pending = new PendingAutoPartyState { Group = group };
                pending.Fsm.TryStateTransition(KAutoPartyFSM.I_TO_CHECK_ACCEPT, out _);
                _pendingByParty[uid] = pending;
                foreach (var m in members)
                    _unitToPartyUid[m.UnitUid] = uid;

                _formed.Enqueue(group);
            }
        }
    }

    public bool Cancel(long unitUid)
    {
        lock (_lock)
            return _queue.RemoveAll(e => e.UnitUid == unitUid) > 0;
    }

    public bool CancelByRequest(long unitUid, long autoPartyUid, long waitNumber, out List<long> affectedPendingUnits)
    {
        affectedPendingUnits = new List<long>();
        bool removed = false;
        lock (_lock)
        {
            if (unitUid > 0)
                removed |= _queue.RemoveAll(e => e.UnitUid == unitUid) > 0;
            if (waitNumber > 0)
                removed |= _queue.RemoveAll(e => e.WaitNumber == waitNumber) > 0;

            long targetAutoPartyUid = autoPartyUid;
            if (targetAutoPartyUid <= 0 && unitUid > 0)
                _unitToPartyUid.TryGetValue(unitUid, out targetAutoPartyUid);

            if (targetAutoPartyUid > 0 && _pendingByParty.Remove(targetAutoPartyUid, out var pending))
            {
                removed = true;
                pending.Fsm.TryStateTransition(KAutoPartyFSM.I_TO_CLOSE, out _);
                foreach (var m in pending.Group.Members)
                {
                    _unitToPartyUid.Remove(m.UnitUid);
                    affectedPendingUnits.Add(m.UnitUid);
                }
            }
        }
        return removed;
    }

    public void ClearAllAutoPartyInfo()
    {
        lock (_lock)
        {
            _queue.Clear();
            _pendingByParty.Clear();
            _unitToPartyUid.Clear();
            while (_formed.TryDequeue(out _)) { }
        }
    }

    public int GetWaitUserCount(int dungeonIdOrWaitNumber = 0)
    {
        lock (_lock)
        {
            return dungeonIdOrWaitNumber <= 0
                ? _queue.Count
                : _queue.Count(q => q.DungeonId == dungeonIdOrWaitNumber);
        }
    }

    public bool TryDequeueGroup(out AutoPartyGroup? group)
        => _formed.TryDequeue(out group);

    public bool SubmitReply(long unitUid, bool agree, out AutoPartyGroup? readyGroup, out AutoPartyGroup? cancelledGroup)
    {
        readyGroup = null;
        cancelledGroup = null;

        lock (_lock)
        {
            if (!_unitToPartyUid.TryGetValue(unitUid, out long partyUid) ||
                !_pendingByParty.TryGetValue(partyUid, out var state))
                return false;

            if (!agree)
            {
                state.Fsm.TryStateTransition(KAutoPartyFSM.I_TO_CLOSE, out _);
                _pendingByParty.Remove(partyUid);
                foreach (var m in state.Group.Members)
                    _unitToPartyUid.Remove(m.UnitUid);
                cancelledGroup = state.Group;
                return true;
            }

            state.AgreedUnitUids.Add(unitUid);
            if (state.AgreedUnitUids.Count >= state.Group.Members.Count)
            {
                state.Fsm.TryStateTransition(KAutoPartyFSM.I_TO_PREPARE_FOR_PLAY, out _);
                state.Fsm.TryStateTransition(KAutoPartyFSM.I_TO_CLOSE, out _);
                _pendingByParty.Remove(partyUid);
                foreach (var m in state.Group.Members)
                    _unitToPartyUid.Remove(m.UnitUid);
                readyGroup = state.Group;
            }
            return true;
        }
    }

    public void BindToLua(KScriptManager script, string globalName = "AutoPartyScriptManager")
    {
        Table? proxy = null;
        proxy = script.CreateLuaProxy(globalName, t =>
        {
            t["SetSameDifficultyLevelWaitTime"] = DynValue.NewCallback((_, a) => { SameDifficultyLevelWaitTime = KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, proxy!), SameDifficultyLevelWaitTime); return DynValue.Nil; });
            t["SetSameUnitTypeWaitTime"] = DynValue.NewCallback((_, a) => { SameUnitTypeWaitTime = KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, proxy!), SameUnitTypeWaitTime); return DynValue.Nil; });
            t["SetDirectGameStartWaitTime"] = DynValue.NewCallback((_, a) => { DirectGameStartWaitTime = KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, proxy!), DirectGameStartWaitTime); return DynValue.Nil; });
            t["SetAutoPartyBonusTime"] = DynValue.NewCallback((_, a) => { AutoPartyBonusTime = KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, proxy!), AutoPartyBonusTime); return DynValue.Nil; });
            t["SetAutoPartyMakingBeginCount"] = DynValue.NewCallback((_, a) => { AutoPartyMakingBeginCount = KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, proxy!), AutoPartyMakingBeginCount); return DynValue.Nil; });
            t["SetCheckWaitUserCount"] = DynValue.NewCallback((_, a) => { CheckWaitUserCount = KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, proxy!), CheckWaitUserCount); return DynValue.Nil; });
            t["SetAutoPartyMatchingSuccessNum"] = DynValue.NewCallback((_, a) => { AutoPartyMatchingSuccessNum = KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, proxy!), AutoPartyMatchingSuccessNum); return DynValue.Nil; });
            t["SetIsAllMemberCheckSameUnitTypeTime"] = DynValue.NewCallback((_, a) => { IsAllMemberCheckSameUnitTypeTime = KScriptManager.ArgBool(a, KScriptManager.ArgStart(a, proxy!), IsAllMemberCheckSameUnitTypeTime); return DynValue.Nil; });
            t["SetIsAllMemberCheckSameDifficultyLevelTime"] = DynValue.NewCallback((_, a) => { IsAllMemberCheckSameDifficultyLevelTime = KScriptManager.ArgBool(a, KScriptManager.ArgStart(a, proxy!), IsAllMemberCheckSameDifficultyLevelTime); return DynValue.Nil; });
            t["SetIsAllMemberCheckDirectGameStartTime"] = DynValue.NewCallback((_, a) => { IsAllMemberCheckDirectGameStartTime = KScriptManager.ArgBool(a, KScriptManager.ArgStart(a, proxy!), IsAllMemberCheckDirectGameStartTime); return DynValue.Nil; });
            t["AddAutoPartyDungeonInfo"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                var tbl = KScriptManager.ArgTable(a, o);
                if (tbl != null)
                {
                    sbyte mode = (sbyte)(tbl.Get("m_eDungeonMode").CastToNumber() ?? 0);
                    int did = (int)(tbl.Get("m_iDungeonID").CastToNumber() ?? 0);
                    bool bonus = tbl.Get("m_bActivatedBonus").Type == DataType.Boolean && tbl.Get("m_bActivatedBonus").Boolean;
                    if (did > 0) SetAutoPartyBonus(mode, did, bonus);
                }
                else
                {
                    sbyte mode = (sbyte)KScriptManager.ArgInt(a, o);
                    int did = KScriptManager.ArgInt(a, o + 1);
                    int minLv = KScriptManager.ArgInt(a, o + 2, 1);
                    int maxLv = KScriptManager.ArgInt(a, o + 3, 70);
                    if (did > 0)
                    {
                        lock (_lock)
                            _autoPartyDungeonRanges.Add((mode, did, minLv, maxLv));
                    }
                }
                return DynValue.Nil;
            });
        });
    }
}
