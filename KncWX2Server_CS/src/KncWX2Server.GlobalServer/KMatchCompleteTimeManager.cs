namespace KncWX2Server.GlobalServer;

/// <summary>
/// Exact C# port of GlobalServer/MatchMakingManager.cpp KMatchCompleteTimeManager:
/// keeps the last 5 completed match wait durations within a 2-hour window and rounds up
/// the average to 10-second intervals (returning 2000 when no valid samples exist).
/// </summary>
public sealed class KMatchCompleteTimeManager
{
    public const int MaxRecordCount = 5;
    public static readonly TimeSpan MaxTimeDiff = TimeSpan.FromHours(2);
    public const int UnknownWaitSeconds = 2000;

    private readonly LinkedList<(DateTime RecordedUtc, int Seconds)> _records = new();

    public void AddCompleteTime(int completeSeconds)
    {
        var now = DateTime.UtcNow;
        if (_records.Count >= MaxRecordCount)
            _records.RemoveFirst();
        _records.AddLast((now, Math.Max(0, completeSeconds)));
    }

    public int GetMatchCompleteSeconds()
    {
        var now = DateTime.UtcNow;
        while (_records.First != null && (now - _records.First.Value.RecordedUtc) > MaxTimeDiff)
            _records.RemoveFirst();

        if (_records.Count == 0)
            return UnknownWaitSeconds;

        int sum = 0;
        foreach (var r in _records)
            sum += r.Seconds;

        int avg = sum / _records.Count;
        // C++: ((avg / 10) + 1) * 10
        return ((avg / 10) + 1) * 10;
    }
}
