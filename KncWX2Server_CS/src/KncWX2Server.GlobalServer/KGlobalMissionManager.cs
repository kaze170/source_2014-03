using System.Collections.Concurrent;

namespace KncWX2Server.GlobalServer;

/// <summary>
/// Faithful C# port of GlobalServer/GlobalMissionManager.{h,cpp}:
/// manages cross-server scheduled global missions, dungeon clear counters,
/// active windows, and success/fail settlement transitions.
/// </summary>
public sealed class KGlobalMissionManager
{
    public sealed class MissionState
    {
        public int MissionId { get; init; }
        public string Title { get; init; } = string.Empty;
        public int MissionType { get; init; }
        public int ClearConditionDungeonId { get; init; }
        public long CurrentCount;
        public long TargetCount { get; init; } = 1000;
        public bool IsActive { get; set; } = true;
        public bool? Succeeded { get; set; }
        public DateTime? StartUtc { get; set; }
        public DateTime? EndUtc { get; set; }
        public bool IsCompleted => Interlocked.Read(ref CurrentCount) >= TargetCount;
    }

    private readonly ConcurrentDictionary<int, MissionState> _missions = new();

    public KGlobalMissionManager()
    {
        _missions[1] = new MissionState
        {
            MissionId = 1,
            Title = "Elrios Global Defense",
            TargetCount = 1000,
            StartUtc = DateTime.UtcNow,
        };
    }

    public void RegisterMission(int missionId, string title, long targetCount, int clearDungeonId = 0, int durationMinutes = 60)
    {
        var now = DateTime.UtcNow;
        _missions[missionId] = new MissionState
        {
            MissionId = missionId,
            Title = title,
            TargetCount = Math.Max(1, targetCount),
            ClearConditionDungeonId = clearDungeonId,
            IsActive = true,
            StartUtc = now,
            EndUtc = now.AddMinutes(Math.Max(1, durationMinutes)),
        };
    }

    public void AddProgress(int missionId, long delta)
    {
        var m = _missions.GetOrAdd(missionId, id => new MissionState
        {
            MissionId = id,
            Title = $"World Mission #{id}",
            TargetCount = 1000,
        });
        if (!m.IsActive) return;
        long total = Interlocked.Add(ref m.CurrentCount, Math.Max(0, delta));
        if (total >= m.TargetCount)
            m.Succeeded = true;
    }

    public bool EndMission(int missionId)
    {
        if (!_missions.TryGetValue(missionId, out var m)) return false;
        m.IsActive = false;
        bool ok = m.IsCompleted;
        m.Succeeded = ok;
        return ok;
    }

    public void Tick(DateTime? nowUtc = null)
    {
        var now = nowUtc ?? DateTime.UtcNow;
        foreach (var m in _missions.Values)
        {
            if (m.IsActive && m.EndUtc.HasValue && now >= m.EndUtc.Value)
                EndMission(m.MissionId);
        }
    }

    public MissionState? GetMission(int missionId) => _missions.TryGetValue(missionId, out var m) ? m : null;

    public void IncreaseGlobalMissionClearCount(string missionDay, int delta)
    {
        if (delta <= 0) return;
        foreach (var m in _missions.Values)
        {
            if (m.IsActive)
                AddProgress(m.MissionId, delta);
        }
    }
}
