using KncWX2Server.Core.Region;

namespace KncWX2Server.GameServer;

/// <summary>
/// Faithful C# port of GameServer/AntiAddictionManager.{h,cpp} (CN region fatigue rules):
/// - 0..3h play: 100% reward (AART_NONE)
/// - 3..5h play: 50% reward (AART_HALF)
/// - >= 5h play: 0% reward (AART_FULL)
/// - >= 5h offline rest resets play time to 0.
/// </summary>
public sealed class KAntiAddictionManager
{
    public enum RewardState
    {
        None = 0, // 100% reward
        Half = 1, // 50% reward (>= 3 hours)
        Full = 2, // 0% reward (>= 5 hours)
    }

    public static readonly TimeSpan HalfThreshold = TimeSpan.FromHours(3);
    public static readonly TimeSpan FullThreshold = TimeSpan.FromHours(5);
    public static readonly TimeSpan RestResetThreshold = TimeSpan.FromHours(5);

    private TimeSpan _accumulatedPlayTime;
    private DateTime _sessionStartUtc = DateTime.UtcNow;

    public bool IsEnabled { get; private set; }
    public bool IsAdult { get; private set; } = true;

    public void Initialize(RegionProfile region, bool isAdult, TimeSpan priorPlayTime, TimeSpan offlineDuration)
    {
        IsEnabled = region.Region == RegionCode.CN && !isAdult;
        IsAdult = isAdult;
        _sessionStartUtc = DateTime.UtcNow;
        _accumulatedPlayTime = offlineDuration >= RestResetThreshold
            ? TimeSpan.Zero
            : priorPlayTime;
    }

    public TimeSpan TotalPlayTime => _accumulatedPlayTime + (DateTime.UtcNow - _sessionStartUtc);

    public RewardState GetCurrentState()
    {
        if (!IsEnabled) return RewardState.None;
        var total = TotalPlayTime;
        if (total >= FullThreshold) return RewardState.Full;
        if (total >= HalfThreshold) return RewardState.Half;
        return RewardState.None;
    }

    public float GetRewardMultiplier() => GetCurrentState() switch
    {
        RewardState.Half => 0.5f,
        RewardState.Full => 0.0f,
        _ => 1.0f,
    };
}
