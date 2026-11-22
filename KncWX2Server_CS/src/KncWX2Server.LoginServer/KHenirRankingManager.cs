using KncWX2Server.Core.Packets;

namespace KncWX2Server.LoginServer;

/// <summary>
/// Faithful C# port of LoginServer/HenirRanking.{h,cpp} &amp; LoginLocalRankingManager.{h,cpp}:
/// Top-100 Henir's Time &amp; Space leaderboard sorted by highest stage then shortest clear time,
/// with Daily/Weekly/Monthly reset scheduling.
/// </summary>
public sealed class KHenirRankingManager
{
    public enum RankingRefreshType
    {
        RtDayRanking = 0,
        RtWeekRanking = 1,
        RtMonthRanking = 2,
    }

    public sealed class HenirRecord
    {
        public long UnitUid { get; init; }
        public string NickName { get; set; } = string.Empty;
        public byte UnitClass { get; set; }
        public byte Level { get; set; }
        public int StageCount { get; set; }
        public int ClearTimeSeconds { get; set; }
        public DateTime RecordedUtc { get; set; } = DateTime.UtcNow;
    }

    public const int MaxRankCount = 100;

    private readonly List<HenirRecord> _records = [];
    private readonly object _sync = new();

    public bool IsRankingChanged { get; private set; }
    public RankingRefreshType RefreshType { get; private set; } = RankingRefreshType.RtWeekRanking;
    public DateTime NextRefreshUtc { get; private set; } = DateTime.UtcNow.AddDays(7);

    public void SetRefreshSchedule(RankingRefreshType type, DayOfWeek dayOfWeek, int hourUtc, DateTime? nowUtc = null)
    {
        RefreshType = type;
        var now = nowUtc ?? DateTime.UtcNow;
        var candidate = new DateTime(now.Year, now.Month, now.Day, Math.Clamp(hourUtc, 0, 23), 0, 0, DateTimeKind.Utc);
        if (type == RankingRefreshType.RtDayRanking)
        {
            if (candidate <= now) candidate = candidate.AddDays(1);
        }
        else if (type == RankingRefreshType.RtWeekRanking)
        {
            int delta = ((int)dayOfWeek - (int)candidate.DayOfWeek + 7) % 7;
            candidate = candidate.AddDays(delta);
            if (candidate <= now) candidate = candidate.AddDays(7);
        }
        else
        {
            candidate = candidate.AddMonths(1);
        }
        NextRefreshUtc = candidate;
    }

    public bool CheckNewRecord(KHenirRankingInfo info)
        => CheckNewRecord(new HenirRecord
        {
            UnitUid = info.UnitUid,
            NickName = info.NickName,
            UnitClass = (byte)Math.Max((sbyte)0, info.UnitClass),
            Level = info.Level,
            StageCount = info.StageCount,
            ClearTimeSeconds = (int)info.PlayTime,
        });

    public bool CheckNewRecord(HenirRecord candidate)
    {
        lock (_sync)
        {
            var existing = _records.FirstOrDefault(r => r.UnitUid == candidate.UnitUid);
            if (existing != null)
            {
                bool better = candidate.StageCount > existing.StageCount ||
                              (candidate.StageCount == existing.StageCount && candidate.ClearTimeSeconds < existing.ClearTimeSeconds);
                if (!better) return false;
                _records.Remove(existing);
            }

            _records.Add(candidate);
            _records.Sort((a, b) =>
            {
                int cmp = b.StageCount.CompareTo(a.StageCount);
                return cmp != 0 ? cmp : a.ClearTimeSeconds.CompareTo(b.ClearTimeSeconds);
            });
            if (_records.Count > MaxRankCount)
                _records.RemoveRange(MaxRankCount, _records.Count - MaxRankCount);

            IsRankingChanged = _records.Any(r => r.UnitUid == candidate.UnitUid);
            return IsRankingChanged;
        }
    }

    public bool DeleteRecord(long unitUid)
    {
        lock (_sync)
            return _records.RemoveAll(r => r.UnitUid == unitUid) > 0;
    }

    public bool DeleteRecordByNickName(string nickName)
    {
        if (string.IsNullOrWhiteSpace(nickName)) return false;
        lock (_sync)
            return _records.RemoveAll(r => string.Equals(r.NickName, nickName, StringComparison.OrdinalIgnoreCase)) > 0;
    }

    public bool DeleteRecordByRank(int rank)
    {
        if (rank <= 0) return false;
        lock (_sync)
        {
            if (rank > _records.Count) return false;
            _records.RemoveAt(rank - 1);
            return true;
        }
    }

    public void InitFromDb(IEnumerable<HenirRecord> records)
    {
        lock (_sync)
        {
            _records.Clear();
            _records.AddRange(records);
            _records.Sort((a, b) =>
            {
                int cmp = b.StageCount.CompareTo(a.StageCount);
                return cmp != 0 ? cmp : a.ClearTimeSeconds.CompareTo(b.ClearTimeSeconds);
            });
            if (_records.Count > MaxRankCount)
                _records.RemoveRange(MaxRankCount, _records.Count - MaxRankCount);
        }
    }

    public List<HenirRecord> GetRankings()
    {
        lock (_sync) return _records.ToList();
    }

    public void ClearIfDue(DateTime? nowUtc = null)
    {
        var now = nowUtc ?? DateTime.UtcNow;
        if (now < NextRefreshUtc) return;
        lock (_sync)
        {
            _records.Clear();
            IsRankingChanged = true;
            NextRefreshUtc = RefreshType switch
            {
                RankingRefreshType.RtDayRanking => NextRefreshUtc.AddDays(1),
                RankingRefreshType.RtWeekRanking => NextRefreshUtc.AddDays(7),
                _ => NextRefreshUtc.AddMonths(1),
            };
        }
    }
}
