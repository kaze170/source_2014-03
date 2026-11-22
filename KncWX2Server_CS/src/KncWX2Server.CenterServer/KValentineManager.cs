namespace KncWX2Server.CenterServer;

/// <summary>
/// Faithful C# port of CenterServer/ValentineManager.{h,cpp} (SERV_EVENT_VALENTINE_DUNGEON):
/// manages 99-second Valentine dungeon countdown and 5-second sync interval notifications.
/// </summary>
public sealed class KValentineManager
{
    public const int LimitDungeonTimeSeconds = 99;

    public enum TimerSlot
    {
        AfterDungeonStart = 0,
        AfterDungeonStart5Sec = 1,
    }

    public enum TimeCheckResult
    {
        None = 0,
        SyncTime = 1,
        EndGame = 2,
    }

    private readonly DateTime?[] _startedUtc = new DateTime?[2];

    public void Init()
    {
        _startedUtc[0] = null;
        _startedUtc[1] = null;
    }

    public bool SetStartTimer(TimerSlot slot, bool start, DateTime? nowUtc = null)
    {
        int idx = (int)slot;
        _startedUtc[idx] = start ? (nowUtc ?? DateTime.UtcNow) : null;
        return true;
    }

    public bool IsOnTimer(TimerSlot slot) => _startedUtc[(int)slot].HasValue;

    public TimeCheckResult CheckTimer(TimerSlot slot, DateTime? nowUtc = null)
    {
        int idx = (int)slot;
        if (!_startedUtc[idx].HasValue) return TimeCheckResult.None;
        double elapsed = ((nowUtc ?? DateTime.UtcNow) - _startedUtc[idx]!.Value).TotalSeconds;

        if (slot == TimerSlot.AfterDungeonStart && elapsed >= LimitDungeonTimeSeconds)
        {
            _startedUtc[idx] = null;
            return TimeCheckResult.EndGame;
        }
        if (slot == TimerSlot.AfterDungeonStart5Sec && elapsed >= 5.0)
        {
            _startedUtc[idx] = nowUtc ?? DateTime.UtcNow;
            return TimeCheckResult.SyncTime;
        }
        return TimeCheckResult.None;
    }

    public int GetRemainTime(TimerSlot slot, DateTime? nowUtc = null)
    {
        int idx = (int)slot;
        if (!_startedUtc[idx].HasValue) return LimitDungeonTimeSeconds;
        double elapsed = ((nowUtc ?? DateTime.UtcNow) - _startedUtc[idx]!.Value).TotalSeconds;
        return Math.Max(0, (int)Math.Ceiling(LimitDungeonTimeSeconds - elapsed));
    }
}
