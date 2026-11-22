using KncWX2Server.Core.Packets;

namespace KncWX2Server.GameServer;

/// <summary>
/// Faithful C# port of GameServer/GSHenirRanking.{h,cpp}:
/// maintains paginated Henir Time-Space rankings for a single ranking type.
/// </summary>
public sealed class KGSHenirRanking
{
    public const int PagePerCount = 5;

    private readonly List<KHenirRankingInfo> _rankingInfo = [];
    private uint _lastRank;
    private readonly object _sync = new();

    public int Count
    {
        get { lock (_sync) return _rankingInfo.Count; }
    }

    public bool IsInRankingUser(long unitUid)
    {
        lock (_sync) return _rankingInfo.Any(r => r.UnitUid == unitUid);
    }

    public void SetLastRank(uint lastRank)
    {
        lock (_sync) _lastRank = lastRank;
    }

    public void ClearRankingInfo()
    {
        lock (_sync) _rankingInfo.Clear();
    }

    public bool CheckNewRecord(KHenirRankingInfo newRank)
    {
        lock (_sync)
        {
            if (_rankingInfo.Count >= _lastRank + 3 && _rankingInfo.Count > 0)
            {
                var oldRank = _rankingInfo[^1];
                if (!IsBetterRecord(newRank, oldRank))
                    return false;
            }
            return true;
        }
    }

    public void UpdateRankingInfo(IEnumerable<KHenirRankingInfo> rankingInfo)
    {
        lock (_sync)
        {
            _rankingInfo.Clear();
            _rankingInfo.AddRange(rankingInfo);
        }
    }

    public void GetRankingInfo(ref uint viewPage, out uint totalPage, List<KHenirRankingInfo> outList)
    {
        outList.Clear();
        if (viewPage == 0) viewPage = 1;

        lock (_sync)
        {
            if (_rankingInfo.Count == 0)
            {
                viewPage = 1;
                totalPage = 1;
                return;
            }

            totalPage = (uint)((_rankingInfo.Count + PagePerCount - 1) / PagePerCount);
            int beginIndex = (int)((viewPage - 1) * PagePerCount);
            if (_rankingInfo.Count <= beginIndex)
            {
                viewPage = totalPage;
                beginIndex = (int)((viewPage - 1) * PagePerCount);
                if (_rankingInfo.Count <= beginIndex)
                    return;
            }

            for (int i = beginIndex; i < _rankingInfo.Count && outList.Count < PagePerCount; i++)
                outList.Add(_rankingInfo[i]);
        }
    }

    public KHenirRankingInfo? FindByUnitUid(long unitUid)
    {
        lock (_sync) return _rankingInfo.FirstOrDefault(r => r.UnitUid == unitUid);
    }

    private static bool IsBetterRecord(KHenirRankingInfo candidate, KHenirRankingInfo current)
    {
        if (candidate.StageCount != current.StageCount)
            return candidate.StageCount > current.StageCount;
        if (candidate.PlayTime != current.PlayTime)
            return candidate.PlayTime < current.PlayTime;
        return candidate.RegDate <= current.RegDate;
    }
}

/// <summary>
/// Faithful C# port of GameServer/GSRankingManager.{h,cpp}:
/// caches server-wide Henir, Dungeon, and PvP rankings synced from LoginServer.
/// </summary>
public sealed class KGSRankingManager
{
    public const int RtDayRanking = 0;
    public const int RtWeekRanking = 1;
    public const int RtMonthRanking = 2;
    public const int RtDungeonRanking = 3;
    public const int RtPvpRanking = 4;
    public const int RtHeroRanking = 9;

    private readonly Dictionary<int, KGSHenirRanking> _henirRankings = new();
    private readonly List<KDungeonRankingInfo> _dungeonRanking = [];
    private readonly Dictionary<long, KDungeonRankingInfo> _dungeonRankingMap = new();
    private readonly List<KPvpRankingInfo> _pvpRanking = [];
    private readonly Dictionary<long, KPvpRankingInfo> _pvpRankingMap = new();
    private readonly object _sync = new();

    public bool HasHenirRanking(int rankingType)
    {
        lock (_sync)
            return _henirRankings.TryGetValue(rankingType, out var hr) && hr.Count > 0;
    }

    public bool HasDungeonRanking
    {
        get { lock (_sync) return _dungeonRanking.Count > 0; }
    }

    public bool HasPvpRanking
    {
        get { lock (_sync) return _pvpRanking.Count > 0; }
    }

    public bool GetRankingInfo(KEGS_GET_RANKING_INFO_REQ req, long requesterUnitUid, KEGS_GET_RANKING_INFO_ACK ack)
    {
        ack.RankingType = req.RankingType;
        uint viewPage = Math.Max(1u, req.ViewPage);
        uint totalPage = 1;

        lock (_sync)
        {
            switch (req.RankingType)
            {
                case RtDayRanking:
                case RtWeekRanking:
                case RtMonthRanking:
                case RtHeroRanking:
                    if (_henirRankings.TryGetValue(req.RankingType, out var hr))
                    {
                        hr.GetRankingInfo(ref viewPage, out totalPage, ack.HenirRankingPage);
                        var mine = hr.FindByUnitUid(requesterUnitUid);
                        if (mine != null) ack.MyHenirRanking.Add(mine);
                    }
                    ack.ViewPage = viewPage;
                    ack.TotalPage = totalPage;
                    return true;

                case RtDungeonRanking:
                    GetDungeonRankingPage(ref viewPage, out totalPage, ack.DungeonRankingPage);
                    if (_dungeonRankingMap.TryGetValue(requesterUnitUid, out var myDung))
                        ack.MyDungeonRanking.Add(myDung);
                    ack.ViewPage = viewPage;
                    ack.TotalPage = totalPage;
                    return true;

                case RtPvpRanking:
                    GetPvpRankingPage(ref viewPage, out totalPage, ack.PvpRankingPage);
                    if (_pvpRankingMap.TryGetValue(requesterUnitUid, out var myPvp))
                        ack.MyPvpRanking.Add(myPvp);
                    ack.ViewPage = viewPage;
                    ack.TotalPage = totalPage;
                    return true;

                default:
                    ack.ViewPage = viewPage;
                    ack.TotalPage = 1;
                    return false;
            }
        }
    }

    public bool CheckNewRecord(KHenirRankingInfo rankingInfo)
    {
        lock (_sync)
        {
            bool anyNew = false;
            foreach (var (rt, hr) in _henirRankings)
            {
                if (rt == RtHeroRanking && rankingInfo.StageCount <= 0)
                    continue;
                if (hr.CheckNewRecord(rankingInfo))
                    anyNew = true;
            }
            return anyNew;
        }
    }

    public bool GetDungeonRankingByUnitUid(long unitUid, out KDungeonRankingInfo? info)
    {
        lock (_sync) return _dungeonRankingMap.TryGetValue(unitUid, out info);
    }

    public bool GetPvpRankingByUnitUid(long unitUid, out KPvpRankingInfo? info)
    {
        lock (_sync) return _pvpRankingMap.TryGetValue(unitUid, out info);
    }

    public List<int> IsInRankingUser(long unitUid)
    {
        lock (_sync)
        {
            return _henirRankings
                .Where(kv => kv.Value.IsInRankingUser(unitUid))
                .Select(kv => kv.Key)
                .ToList();
        }
    }

    public void UpdateHenirRanking(IDictionary<int, List<KHenirRankingInfo>> mapHenirRanking)
    {
        lock (_sync)
        {
            foreach (var (rt, list) in mapHenirRanking)
            {
                if (!_henirRankings.TryGetValue(rt, out var hr))
                {
                    hr = new KGSHenirRanking();
                    _henirRankings[rt] = hr;
                }
                if (list.Count == 0) hr.ClearRankingInfo();
                else hr.UpdateRankingInfo(list);
            }
        }
    }

    public void UpdateHenirRanking(
        IDictionary<int, List<KHenirRankingInfo>> mapHenirRanking,
        IDictionary<int, uint> mapLastRank)
    {
        lock (_sync)
        {
            foreach (var (rt, lastRank) in mapLastRank)
            {
                if (!_henirRankings.TryGetValue(rt, out var hr))
                {
                    hr = new KGSHenirRanking();
                    _henirRankings[rt] = hr;
                }
                hr.SetLastRank(lastRank);
            }
        }
        UpdateHenirRanking(mapHenirRanking);
    }

    public void UpdateDungeonAndPvpRanking(KELG_WEB_RANKING_REFRESH_NOT info)
    {
        lock (_sync)
        {
            switch (info.RankingRefreshType)
            {
                case KELG_WEB_RANKING_REFRESH_NOT.DUNGEON_VECTOR:
                    if (info.Init) _dungeonRanking.Clear();
                    _dungeonRanking.AddRange(info.DungeonRanking);
                    break;

                case KELG_WEB_RANKING_REFRESH_NOT.DUNGEON_MAP:
                    if (info.Init) _dungeonRankingMap.Clear();
                    foreach (var (uid, entry) in info.ApDungeonRanking)
                        _dungeonRankingMap[uid] = entry;
                    break;

                case KELG_WEB_RANKING_REFRESH_NOT.PVP_VECTOR:
                    if (info.Init) _pvpRanking.Clear();
                    _pvpRanking.AddRange(info.PvpRanking);
                    break;

                case KELG_WEB_RANKING_REFRESH_NOT.PVP_MAP:
                    if (info.Init) _pvpRankingMap.Clear();
                    foreach (var (uid, entry) in info.ApPvpRanking)
                        _pvpRankingMap[uid] = entry;
                    break;
            }
        }
    }

    private void GetDungeonRankingPage(ref uint viewPage, out uint totalPage, List<KDungeonRankingInfo> outList)
    {
        outList.Clear();
        if (viewPage == 0) viewPage = 1;
        if (_dungeonRanking.Count == 0)
        {
            viewPage = 1;
            totalPage = 1;
            return;
        }

        totalPage = (uint)((_dungeonRanking.Count + KGSHenirRanking.PagePerCount - 1) / KGSHenirRanking.PagePerCount);
        int begin = (int)((viewPage - 1) * KGSHenirRanking.PagePerCount);
        if (_dungeonRanking.Count <= begin)
        {
            viewPage = totalPage;
            begin = (int)((viewPage - 1) * KGSHenirRanking.PagePerCount);
            if (_dungeonRanking.Count <= begin) return;
        }
        for (int i = begin; i < _dungeonRanking.Count && outList.Count < KGSHenirRanking.PagePerCount; i++)
            outList.Add(_dungeonRanking[i]);
    }

    private void GetPvpRankingPage(ref uint viewPage, out uint totalPage, List<KPvpRankingInfo> outList)
    {
        outList.Clear();
        if (viewPage == 0) viewPage = 1;
        if (_pvpRanking.Count == 0)
        {
            viewPage = 1;
            totalPage = 1;
            return;
        }

        totalPage = (uint)((_pvpRanking.Count + KGSHenirRanking.PagePerCount - 1) / KGSHenirRanking.PagePerCount);
        int begin = (int)((viewPage - 1) * KGSHenirRanking.PagePerCount);
        if (_pvpRanking.Count <= begin)
        {
            viewPage = totalPage;
            begin = (int)((viewPage - 1) * KGSHenirRanking.PagePerCount);
            if (_pvpRanking.Count <= begin) return;
        }
        for (int i = begin; i < _pvpRanking.Count && outList.Count < KGSHenirRanking.PagePerCount; i++)
            outList.Add(_pvpRanking[i]);
    }
}
