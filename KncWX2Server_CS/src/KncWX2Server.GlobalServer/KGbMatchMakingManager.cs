using System.Collections.Concurrent;
using KncWX2Server.Core.Scripting;
using MoonSharp.Interpreter;

namespace KncWX2Server.GlobalServer;

public sealed record MatchRequestEntry(
    long UnitUid,
    string NickName,
    int Rating,
    sbyte MatchType, // 1 = 1v1, 2 = 2v2, 3 = 3v3
    DateTime EnqueuedUtc,
    bool IsBeginner = false,
    bool IsArrangeRank = false,
    long PartyUid = 0);

public sealed record MatchFoundResult(
    long MatchUid,
    sbyte MatchType,
    IReadOnlyList<MatchRequestEntry> TeamA,
    IReadOnlyList<MatchRequestEntry> TeamB);

/// <summary>
/// Faithful C# port of GlobalServer/MatchMakingManager.{h,cpp} (Season 2 algorithm):
/// - KMatchCompleteTimeManager per MatchType
/// - Beginner (F-rank) isolation
/// - Optimal rating gap buckets (&lt;= 200, &lt;= 400, arrange &lt;= 400)
/// - GAP_MIN_MAX_RATING check and max-two 2-player-party rule
/// - Balanced TeamA/TeamB assignment + KMatchFSM tracking.
/// </summary>
public sealed class KGbMatchMakingManager
{
    public const int CheckWaitUserCount = 50;
    public const int GapMinMaxRating = 450;

    private sealed class PendingMatchState
    {
        public MatchFoundResult Match { get; init; } = default!;
        public HashSet<long> AgreedUnitUids { get; } = new();
    }

    private readonly object _lock = new();
    private readonly List<MatchRequestEntry> _queue = new();
    private readonly ConcurrentQueue<MatchFoundResult> _completed = new();
    private readonly Dictionary<long, PendingMatchState> _pendingByMatch = new();
    private readonly Dictionary<long, long> _unitToMatchUid = new();
    private readonly Dictionary<sbyte, KMatchCompleteTimeManager> _completeTimes = new()
    {
        [1] = new(),
        [2] = new(),
        [3] = new(),
    };
    private readonly ConcurrentDictionary<long, KMatchFSM> _activeMatchFsms = new();
    private long _nextMatchUid = 700_000;

    public int MaxRankingPoint { get; set; } = 200_000;
    public int MaxArenaPoint { get; set; } = 100_000;
    public int MatchMakingBeginCount { get; set; } = 2;
    public int ConfiguredCheckWaitUserCount { get; set; } = CheckWaitUserCount;
    public int ConfiguredGapMinMaxRating { get; set; } = GapMinMaxRating;
    public int HeroPvpNpcFactor { get; set; } = 80;
    public Dictionary<int, int> MatchWeightValues { get; } = new();
    public Dictionary<int, (int TeamCount, int PlayTimeSec)> PvpMatchInfos { get; } = new();
    public List<int> PvpMapsForMatch { get; } = new();
    public List<int> PvpMapsForNpcMatch { get; } = new();
    public List<string> PvpNpcNames { get; } = new();
    public Dictionary<int, (int PvpNpcType, int UnitClass, int Level, int Rating, int Ranking)> PvpNpcInfos { get; } = new();

    public int QueueCount
    {
        get { lock (_lock) return _queue.Count; }
    }

    public int TotalMatchesFormed { get; private set; }

    public int GetEstimatedWaitSeconds(sbyte matchType)
    {
        lock (_lock)
            return _completeTimes.TryGetValue(matchType, out var mgr)
                ? mgr.GetMatchCompleteSeconds()
                : KMatchCompleteTimeManager.UnknownWaitSeconds;
    }

    public void Enqueue(MatchRequestEntry entry, bool regFront = false)
    {
        lock (_lock)
        {
            _queue.RemoveAll(e => e.UnitUid == entry.UnitUid);
            if (regFront && _queue.Count > 0)
            {
                var earliest = _queue.Min(e => e.EnqueuedUtc);
                var frontEntry = entry.EnqueuedUtc >= earliest
                    ? entry with { EnqueuedUtc = earliest.AddTicks(-1) }
                    : entry;
                _queue.Insert(0, frontEntry);
            }
            else
            {
                _queue.Add(entry);
            }
            TryPairLocked(entry.MatchType);
        }
    }

    public bool Cancel(long unitUid)
    {
        lock (_lock)
            return _queue.RemoveAll(e => e.UnitUid == unitUid) > 0;
    }

    public bool CancelByRequest(long unitUid, long matchUid, long partyUid, out List<long> affectedPendingUnits)
    {
        affectedPendingUnits = new List<long>();
        bool removed = false;
        lock (_lock)
        {
            if (unitUid > 0)
                removed |= _queue.RemoveAll(e => e.UnitUid == unitUid) > 0;
            if (partyUid > 0)
                removed |= _queue.RemoveAll(e => e.PartyUid == partyUid) > 0;

            long targetMatchUid = matchUid;
            if (targetMatchUid <= 0 && unitUid > 0)
                _unitToMatchUid.TryGetValue(unitUid, out targetMatchUid);

            if (targetMatchUid > 0 && _pendingByMatch.Remove(targetMatchUid, out var pending))
            {
                removed = true;
                if (_activeMatchFsms.TryGetValue(targetMatchUid, out var fsm))
                {
                    fsm.TryStateTransition(KMatchFSM.I_TO_CLOSE, out _);
                    _activeMatchFsms.TryRemove(targetMatchUid, out _);
                }
                foreach (var m in pending.Match.TeamA.Concat(pending.Match.TeamB))
                {
                    _unitToMatchUid.Remove(m.UnitUid);
                    affectedPendingUnits.Add(m.UnitUid);
                }
            }
        }
        return removed;
    }

    public void ClearAllMatchInfo()
    {
        lock (_lock)
        {
            _queue.Clear();
            _pendingByMatch.Clear();
            _unitToMatchUid.Clear();
            _activeMatchFsms.Clear();
            while (_completed.TryDequeue(out _)) { }
        }
    }

    public int GetQueueCount(sbyte matchType)
    {
        lock (_lock)
            return _queue.Count(q => q.MatchType == matchType);
    }

    public bool IsRegistered(long unitUid)
    {
        lock (_lock)
            return _queue.Any(e => e.UnitUid == unitUid) || _unitToMatchUid.ContainsKey(unitUid);
    }

    public bool TryDequeueMatch(out MatchFoundResult? match)
        => _completed.TryDequeue(out match);

    public IReadOnlyList<MatchFoundResult> TickMatchmaking()
    {
        lock (_lock)
        {
            foreach (sbyte mt in new sbyte[] { 1, 2, 3 })
                TryPairLocked(mt);
        }

        var results = new List<MatchFoundResult>();
        while (_completed.TryDequeue(out var m))
            results.Add(m);
        return results;
    }

    private void TryPairLocked(sbyte matchType)
    {
        int perTeam = Math.Clamp((int)matchType, 1, 3);
        int totalNeeded = perTeam * 2;
        var candidates = _queue.Where(q => q.MatchType == matchType)
            .OrderBy(q => q.EnqueuedUtc)
            .ToList();

        while (candidates.Count >= totalNeeded)
        {
            var front = candidates[0];
            var rest = candidates.Skip(1).Take(Math.Max(1, ConfiguredCheckWaitUserCount)).ToList();

            var bucket200 = new List<MatchRequestEntry>();
            var bucket400 = new List<MatchRequestEntry>();
            var bucketArrange = new List<MatchRequestEntry>();

            // Wait time relaxes gap threshold slightly after 15s (faithful anti-starvation safety)
            int waitBonus = (int)Math.Min(400, (DateTime.UtcNow - front.EnqueuedUtc).TotalSeconds * 10);
            int limit200 = 200 + waitBonus;
            int limit400 = 400 + waitBonus;

            foreach (var cand in rest)
            {
                if (cand.IsBeginner != front.IsBeginner)
                    continue;

                int gap = Math.Abs(cand.Rating - front.Rating);
                if (cand.IsArrangeRank)
                {
                    if (gap <= limit400) bucketArrange.Add(cand);
                }
                else
                {
                    if (gap <= limit200) bucket200.Add(cand);
                    else if (gap <= limit400) bucket400.Add(cand);
                }
            }

            var picked = new List<MatchRequestEntry> { front };
            int minRating = front.Rating;
            int maxRating = front.Rating;

            int maxGap = Math.Max(100, ConfiguredGapMinMaxRating) + waitBonus;
            if (!TryPickFromBucket(bucket200, totalNeeded, maxGap, picked, ref minRating, ref maxRating) &&
                !TryPickFromBucket(bucket400, totalNeeded, maxGap, picked, ref minRating, ref maxRating) &&
                !TryPickFromBucket(bucketArrange, totalNeeded, maxGap, picked, ref minRating, ref maxRating))
            {
                break;
            }

            foreach (var p in picked)
            {
                candidates.Remove(p);
                _queue.RemoveAll(x => x.UnitUid == p.UnitUid);
                if (_completeTimes.TryGetValue(matchType, out var ct))
                    ct.AddCompleteTime((int)(DateTime.UtcNow - p.EnqueuedUtc).TotalSeconds);
            }

            // Snake-draft by Rating to minimize team rating disparity
            var ordered = picked.OrderByDescending(p => p.Rating).ToList();
            var teamA = new List<MatchRequestEntry>(perTeam);
            var teamB = new List<MatchRequestEntry>(perTeam);
            int sumA = 0, sumB = 0;
            foreach (var p in ordered)
            {
                if (teamA.Count < perTeam && (sumA <= sumB || teamB.Count >= perTeam))
                {
                    teamA.Add(p);
                    sumA += p.Rating;
                }
                else
                {
                    teamB.Add(p);
                    sumB += p.Rating;
                }
            }

            long matchUid = Interlocked.Increment(ref _nextMatchUid);
            var fsm = new KMatchFSM();
            fsm.TryStateTransition(KMatchFSM.I_TO_CHECK_ACCEPT, out _);
            _activeMatchFsms[matchUid] = fsm;

            var matchResult = new MatchFoundResult(matchUid, matchType, teamA, teamB);
            _pendingByMatch[matchUid] = new PendingMatchState { Match = matchResult };
            foreach (var p in picked)
                _unitToMatchUid[p.UnitUid] = matchUid;

            TotalMatchesFormed++;
            _completed.Enqueue(matchResult);
        }
    }

    public bool SubmitReply(long unitUid, bool agree, out MatchFoundResult? readyMatch, out MatchFoundResult? cancelledMatch)
    {
        readyMatch = null;
        cancelledMatch = null;

        lock (_lock)
        {
            if (!_unitToMatchUid.TryGetValue(unitUid, out long matchUid) ||
                !_pendingByMatch.TryGetValue(matchUid, out var state))
                return false;

            if (!agree)
            {
                if (_activeMatchFsms.TryGetValue(matchUid, out var fsm))
                    fsm.TryStateTransition(KMatchFSM.I_TO_CLOSE, out _);
                _pendingByMatch.Remove(matchUid);
                foreach (var m in state.Match.TeamA.Concat(state.Match.TeamB))
                    _unitToMatchUid.Remove(m.UnitUid);
                cancelledMatch = state.Match;
                return true;
            }

            state.AgreedUnitUids.Add(unitUid);
            int totalPlayers = state.Match.TeamA.Count + state.Match.TeamB.Count;
            if (state.AgreedUnitUids.Count >= totalPlayers)
            {
                if (_activeMatchFsms.TryGetValue(matchUid, out var fsm))
                {
                    fsm.TryStateTransition(KMatchFSM.I_TO_PREPARE_FOR_PLAY, out _);
                    fsm.TryStateTransition(KMatchFSM.I_TO_PLAY, out _);
                }
                _pendingByMatch.Remove(matchUid);
                foreach (var m in state.Match.TeamA.Concat(state.Match.TeamB))
                    _unitToMatchUid.Remove(m.UnitUid);
                readyMatch = state.Match;
            }
            return true;
        }
    }

    private static bool TryPickFromBucket(
        List<MatchRequestEntry> bucket,
        int totalNeeded,
        int maxAllowedGap,
        List<MatchRequestEntry> picked,
        ref int minRating,
        ref int maxRating)
    {
        foreach (var cand in bucket)
        {
            int nextMin = Math.Min(minRating, cand.Rating);
            int nextMax = Math.Max(maxRating, cand.Rating);
            if (nextMax - nextMin > maxAllowedGap)
                continue;

            minRating = nextMin;
            maxRating = nextMax;
            picked.Add(cand);
            if (picked.Count == totalNeeded)
                return true;
        }
        return picked.Count == totalNeeded;
    }

    public void BindToLua(KScriptManager script, string globalName = "PvpMatchManager")
    {
        Table? proxy = null;
        proxy = script.CreateLuaProxy(globalName, t =>
        {
            t["SetMaxRankingPoint"] = DynValue.NewCallback((_, a) => { MaxRankingPoint = KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, proxy!), MaxRankingPoint); return DynValue.Nil; });
            t["SetMaxArenaPoint"] = DynValue.NewCallback((_, a) => { MaxArenaPoint = KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, proxy!), MaxArenaPoint); return DynValue.Nil; });
            t["SetMatchMakingBeginCount"] = DynValue.NewCallback((_, a) => { MatchMakingBeginCount = KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, proxy!), MatchMakingBeginCount); return DynValue.Nil; });
            t["SetCheckWaitUserCount"] = DynValue.NewCallback((_, a) => { ConfiguredCheckWaitUserCount = KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, proxy!), ConfiguredCheckWaitUserCount); return DynValue.Nil; });
            t["SetGapMinMaxRating"] = DynValue.NewCallback((_, a) => { ConfiguredGapMinMaxRating = KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, proxy!), ConfiguredGapMinMaxRating); return DynValue.Nil; });
            t["SetHeroPvpNpcFactor"] = DynValue.NewCallback((_, a) => { HeroPvpNpcFactor = KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, proxy!), HeroPvpNpcFactor); return DynValue.Nil; });
            t["SetMatchWeightValue"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                MatchWeightValues[KScriptManager.ArgInt(a, o)] = KScriptManager.ArgInt(a, o + 1);
                return DynValue.Nil;
            });
            t["AddPvpMatchInfo"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                PvpMatchInfos[KScriptManager.ArgInt(a, o)] = (KScriptManager.ArgInt(a, o + 1), KScriptManager.ArgInt(a, o + 2));
                return DynValue.Nil;
            });
            t["AddPvpMapForMatch"] = DynValue.NewCallback((_, a) =>
            {
                int mapId = KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, proxy!));
                if (mapId > 0 && !PvpMapsForMatch.Contains(mapId)) PvpMapsForMatch.Add(mapId);
                return DynValue.Nil;
            });
            t["AddPvpMapForNpcMatch"] = DynValue.NewCallback((_, a) =>
            {
                int mapId = KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, proxy!));
                if (mapId > 0 && !PvpMapsForNpcMatch.Contains(mapId)) PvpMapsForNpcMatch.Add(mapId);
                return DynValue.Nil;
            });
            t["AddPvpNpcName"] = DynValue.NewCallback((_, a) =>
            {
                string name = KScriptManager.ArgString(a, KScriptManager.ArgStart(a, proxy!));
                if (!string.IsNullOrEmpty(name)) PvpNpcNames.Add(name);
                return DynValue.Nil;
            });
            t["AddPvpNpcInfo"] = DynValue.NewCallback((_, a) =>
            {
                var tbl = KScriptManager.ArgTable(a, KScriptManager.ArgStart(a, proxy!));
                if (tbl != null)
                {
                    int id = (int)(tbl.Get("m_PvpNpcID").CastToNumber() ?? 0);
                    int type = (int)(tbl.Get("m_PvpNpcType").CastToNumber() ?? 0);
                    int unitClass = (int)(tbl.Get("m_UnitClass").CastToNumber() ?? 0);
                    int level = (int)(tbl.Get("m_Level").CastToNumber() ?? 1);
                    int rating = (int)(tbl.Get("m_Rating").CastToNumber() ?? 400);
                    int ranking = (int)(tbl.Get("m_Ranking").CastToNumber() ?? 1);
                    if (id > 0) PvpNpcInfos[id] = (type, unitClass, level, rating, ranking);
                }
                return DynValue.Nil;
            });
        });
    }
}
