using KncWX2Server.Core.Packets;

namespace KncWX2Server.LoginServer;

/// <summary>
/// Faithful C# port of LoginServer/LoginLocalRankingManager.{h,cpp} (SERV_LOCAL_RANKING_SYSTEM):
/// maintains multi-category local ranking tabs (Spirit &amp; AP), gender/category filtering,
/// top-N truncation, delta classification (Category/Spirit/AP/Data), rank-up notifications,
/// and Friday 06:00 weekly settlement/reset.
/// </summary>
public sealed class KLoginLocalRankingManager
{
    public const int MaxRankersPerTab = 100;

    public enum MainTabType
    {
        LrmtcSpirit = 0,
        LrmtcAp = 1,
    }

    public enum ChangeResult
    {
        None = 0,
        DataChanged = 1,
        CategoryChanged = 2,
        SpiritChanged = 3,
        ApChanged = 4,
        InsertData = 5,
    }

    private sealed class RankEntry
    {
        public long UnitUid { get; init; }
        public long UserUid { get; init; }
        public KLocalRankingUserInfo UserInfo { get; set; } = new();
        public KLocalRankingUnitInfo UnitInfo { get; set; } = new();
    }

    private readonly Dictionary<long, RankEntry> _entriesByUnit = new();
    private readonly Dictionary<long, KLocalRankingUserInfo> _usersByAccount = new();
    private readonly Dictionary<long, int> _lastKnownRanks = new();
    private readonly object _sync = new();

    public int SystemUid { get; private set; } = 1;
    public bool IsInitialized { get; private set; }
    public DateTime NextResetUtc { get; private set; }

    public IReadOnlyDictionary<long, KLocalRankingUnitInfo> LastWeekBackupUnits { get; private set; }
        = new Dictionary<long, KLocalRankingUnitInfo>();

    public KLoginLocalRankingManager()
    {
        Init(1, DateTime.UtcNow);
    }

    public void Init(int systemUid, DateTime anchorUtc)
    {
        lock (_sync)
        {
            SystemUid = systemUid;
            IsInitialized = true;
            // Next Friday 06:00 UTC (mirrors C++ DayOfWeek=6 / 5 - DayOfWeek term)
            int dow = (int)anchorUtc.DayOfWeek;
            int daysUntilFri = ((int)DayOfWeek.Friday - dow + 7) % 7;
            if (daysUntilFri == 0) daysUntilFri = 7;
            var baseDay = new DateTime(anchorUtc.Year, anchorUtc.Month, anchorUtc.Day, 6, 0, 0, DateTimeKind.Utc);
            NextResetUtc = baseDay.AddDays(daysUntilFri);
        }
    }

    public ChangeResult CheckChangedInfo(KLocalRankingUserInfo userInfo, KLocalRankingUnitInfo unitInfo)
    {
        lock (_sync)
        {
            if (_usersByAccount.TryGetValue(userInfo.UserUid, out var prevUser))
            {
                if (prevUser.Category != userInfo.Category || prevUser.Filter != userInfo.Filter)
                    return ChangeResult.CategoryChanged;
            }

            if (_entriesByUnit.TryGetValue(unitInfo.UnitUid, out var prevEntry))
            {
                if (prevEntry.UnitInfo.Point[0] != unitInfo.Point[0])
                    return ChangeResult.SpiritChanged;
                if (prevEntry.UnitInfo.Point[1] != unitInfo.Point[1])
                    return ChangeResult.ApChanged;
                if (prevEntry.UnitInfo.Level != unitInfo.Level ||
                    prevEntry.UnitInfo.NickName != unitInfo.NickName ||
                    prevEntry.UserInfo.Profile != userInfo.Profile)
                    return ChangeResult.DataChanged;
                return ChangeResult.None;
            }

            return (unitInfo.Point[0] > 0 || unitInfo.Point[1] > 0)
                ? ChangeResult.InsertData
                : ChangeResult.None;
        }
    }

    public (ChangeResult change, int? newRank, int? prevRank) SomethingChanged(
        int systemUid, KLocalRankingUserInfo userInfo, KLocalRankingUnitInfo unitInfo)
    {
        if (!IsInitialized || systemUid != SystemUid)
            return (ChangeResult.None, null, null);

        var change = CheckChangedInfo(userInfo, unitInfo);
        if (change == ChangeResult.None)
            return (ChangeResult.None, null, null);

        lock (_sync)
        {
            _usersByAccount[userInfo.UserUid] = userInfo;
            _entriesByUnit[unitInfo.UnitUid] = new RankEntry
            {
                UnitUid = unitInfo.UnitUid,
                UserUid = userInfo.UserUid,
                UserInfo = userInfo,
                UnitInfo = unitInfo,
            };

            int? prevRank = _lastKnownRanks.TryGetValue(unitInfo.UnitUid, out int oldR) ? oldR : null;
            var ranked = GetOrderedLocked(0, userInfo.Category, filterMask: 0, requesterUser: null);
            int idx = ranked.FindIndex(e => e.UnitUid == unitInfo.UnitUid);
            int? curRank = idx >= 0 && idx < MaxRankersPerTab ? idx + 1 : null;
            if (curRank.HasValue)
                _lastKnownRanks[unitInfo.UnitUid] = curRank.Value;
            else
                _lastKnownRanks.Remove(unitInfo.UnitUid);

            return (change, curRank, prevRank);
        }
    }

    public (List<KLocalRankingRankerInfo> pageList, uint totalPage, uint viewPage, KLocalRankingRankerInfo myRankInfo)
        GetRankerList(
            int mainTabIndex,
            int category,
            byte filterMask,
            uint viewPage,
            long myUnitUid,
            KLocalRankingUserInfo? requesterUser = null,
            int pageSize = 10)
    {
        lock (_sync)
        {
            var ordered = GetOrderedLocked(mainTabIndex, category, filterMask, requesterUser)
                .Take(MaxRankersPerTab)
                .ToList();

            uint totalPage = (uint)Math.Max(1, (ordered.Count + pageSize - 1) / pageSize);
            uint clampedPage = Math.Clamp(viewPage == 0 ? 1u : viewPage, 1u, totalPage);

            var slice = ordered
                .Skip((int)((clampedPage - 1) * pageSize))
                .Take(pageSize)
                .Select((e, i) => new KLocalRankingRankerInfo
                {
                    Rank = (int)((clampedPage - 1) * pageSize) + i + 1,
                    UserInfo = e.UserInfo,
                    UnitInfo = e.UnitInfo,
                })
                .ToList();

            int myIdx = ordered.FindIndex(e => e.UnitUid == myUnitUid);
            KLocalRankingRankerInfo myRank;
            if (myIdx >= 0)
            {
                myRank = new KLocalRankingRankerInfo
                {
                    Rank = myIdx + 1,
                    UserInfo = ordered[myIdx].UserInfo,
                    UnitInfo = ordered[myIdx].UnitInfo,
                };
            }
            else if (_entriesByUnit.TryGetValue(myUnitUid, out var self))
            {
                myRank = new KLocalRankingRankerInfo { Rank = 0, UserInfo = self.UserInfo, UnitInfo = self.UnitInfo };
            }
            else
            {
                myRank = new KLocalRankingRankerInfo();
            }

            return (slice, totalPage, clampedPage, myRank);
        }
    }

    public (int rank, long nextUnitUid) GetRankAndNextUnitUid(int mainTabIndex, int category, long myUnitUid)
    {
        lock (_sync)
        {
            var ordered = GetOrderedLocked(mainTabIndex, category, 0, null).Take(MaxRankersPerTab).ToList();
            int idx = ordered.FindIndex(e => e.UnitUid == myUnitUid);
            if (idx < 0) return (0, 0);
            long nextUid = idx > 0 ? ordered[idx - 1].UnitUid : 0;
            return (idx + 1, nextUid);
        }
    }

    public bool TickWeeklyReset(DateTime? nowUtc = null)
    {
        var now = nowUtc ?? DateTime.UtcNow;
        lock (_sync)
        {
            if (!IsInitialized || now < NextResetUtc)
                return false;

            LastWeekBackupUnits = _entriesByUnit.ToDictionary(kv => kv.Key, kv => kv.Value.UnitInfo);
            _entriesByUnit.Clear();
            _lastKnownRanks.Clear();
            SystemUid++;
            NextResetUtc = NextResetUtc.AddDays(7);
            return true;
        }
    }

    private List<RankEntry> GetOrderedLocked(
        int mainTabIndex, int category, byte filterMask, KLocalRankingUserInfo? requesterUser)
    {
        int ptIdx = mainTabIndex == (int)MainTabType.LrmtcAp ? 1 : 0;
        IEnumerable<RankEntry> q = _entriesByUnit.Values;
        if (category >= 0)
            q = q.Where(e => e.UserInfo.Category == category);
        if ((filterMask & 0x01) != 0 && requesterUser != null) // Same gender
            q = q.Where(e => e.UserInfo.Gender == requesterUser.Gender);
        if ((filterMask & 0x02) != 0 && requesterUser != null && requesterUser.Birth > 0) // Same birth year
            q = q.Where(e => e.UserInfo.Birth / 10000 == requesterUser.Birth / 10000);

        return q
            .OrderByDescending(e => e.UnitInfo.Point[ptIdx])
            .ThenByDescending(e => e.UnitInfo.Level)
            .ThenBy(e => e.UnitUid)
            .ToList();
    }
}
