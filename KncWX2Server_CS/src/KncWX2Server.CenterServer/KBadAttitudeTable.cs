namespace KncWX2Server.CenterServer;

/// <summary>
/// Faithful C# port of CenterServer/BadAttitudeTable.{h,cpp} (SERV_DUNGEON_FORCED_EXIT_SYSTEM):
/// maps dungeon types to bad-attitude detection thresholds and forced-exit kick thresholds.
/// </summary>
public sealed class KBadAttitudeTable
{
    public const int DefaultThreshold = 1000;

    private readonly Dictionary<int, int> _cutLinePoints = new();
    private readonly Dictionary<int, int> _forceExitPoints = new();

    public void SetBadAttitudeCutLinePoint(int dungeonType, int point)
        => _cutLinePoints[dungeonType] = point;

    public void SetForceExitPoint(int dungeonType, int point)
        => _forceExitPoints[dungeonType] = point;

    public int GetBadAttitudeCutLinePoint(int dungeonType)
        => _cutLinePoints.GetValueOrDefault(dungeonType, DefaultThreshold);

    public int GetForceExitPoint(int dungeonType)
        => _forceExitPoints.GetValueOrDefault(dungeonType, DefaultThreshold);
}
