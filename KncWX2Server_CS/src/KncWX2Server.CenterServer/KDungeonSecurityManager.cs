using System.Collections.Concurrent;

namespace KncWX2Server.CenterServer;

/// <summary>
/// Faithful C# port of CenterServer/DungeonSecurityManager.{h,cpp} (SERV_BATTLE_FIELD_SYSTEM):
/// monitors dungeon room start/end timing, abnormal fast-clear detection, and excessive single-hit damage.
/// </summary>
public sealed class KDungeonSecurityManager
{
    public sealed class SecurityAnomalyReport
    {
        public long RoomUid { get; init; }
        public int DungeonId { get; init; }
        public long UnitUid { get; init; }
        public string Reason { get; init; } = string.Empty;
        public DateTime RecordedUtc { get; init; } = DateTime.UtcNow;
    }

    private readonly ConcurrentDictionary<long, DateTime> _roomStartTimes = new();
    private readonly ConcurrentDictionary<int, int> _minClearSecondsByDungeon = new();
    private readonly ConcurrentQueue<SecurityAnomalyReport> _anomalies = new();

    public int MaxAllowedSingleDamage { get; set; } = 50_000_000;
    public int DefaultMinClearSeconds { get; set; } = 10;
    public int AnomalyCount => _anomalies.Count;

    public void SetMinClearSeconds(int dungeonId, int minSeconds)
        => _minClearSecondsByDungeon[dungeonId] = Math.Max(1, minSeconds);

    public void StartGame(long roomUid, DateTime? nowUtc = null)
        => _roomStartTimes[roomUid] = nowUtc ?? DateTime.UtcNow;

    public void EndGame(long roomUid)
        => _roomStartTimes.TryRemove(roomUid, out _);

    public bool ValidateClearTiming(long roomUid, int dungeonId, long unitUid, out int elapsedSeconds, DateTime? nowUtc = null)
    {
        var now = nowUtc ?? DateTime.UtcNow;
        if (_roomStartTimes.TryGetValue(roomUid, out var started))
            elapsedSeconds = Math.Max(0, (int)(now - started).TotalSeconds);
        else
            elapsedSeconds = DefaultMinClearSeconds;

        int minReq = _minClearSecondsByDungeon.GetValueOrDefault(dungeonId, DefaultMinClearSeconds);
        if (elapsedSeconds < minReq)
        {
            _anomalies.Enqueue(new SecurityAnomalyReport
            {
                RoomUid = roomUid,
                DungeonId = dungeonId,
                UnitUid = unitUid,
                Reason = $"AbnormalFastClear:{elapsedSeconds}s<{minReq}s",
                RecordedUtc = now,
            });
            return false;
        }
        return true;
    }

    public bool ValidateDamage(long roomUid, int dungeonId, long unitUid, int singleDamage)
    {
        if (singleDamage > MaxAllowedSingleDamage)
        {
            _anomalies.Enqueue(new SecurityAnomalyReport
            {
                RoomUid = roomUid,
                DungeonId = dungeonId,
                UnitUid = unitUid,
                Reason = $"ExcessiveDamage:{singleDamage}>{MaxAllowedSingleDamage}",
            });
            return false;
        }
        return true;
    }

    public IReadOnlyList<SecurityAnomalyReport> SnapshotAnomalies() => _anomalies.ToArray();
}
