using KncWX2Server.Core.Framework;

namespace KncWX2Server.GameServer;

/// <summary>
/// Faithful C# port of GameServer/ComeBackUserManager.{h,cpp} (SERV_COME_BACK_USER_REWARD):
/// evaluates returning-user inactivity tier (15/30/60 days), issues step reward cubes,
/// and tracks ComeBack buff expiration.
/// </summary>
public sealed class KComeBackUserManager
{
    public bool IsComeBackUser { get; set; }
    public bool IsComeBackFirst { get; set; }
    public bool IsUnitSelect { get; set; }
    public KGameSysValTable.ComeBackStep ComeBackRewardStep { get; set; } = KGameSysValTable.ComeBackStep.CbsNone;
    public DateTime ComeBackBuffEndUtc { get; private set; } = DateTime.MinValue;

    public void Clear()
    {
        IsComeBackUser = false;
        IsComeBackFirst = false;
        IsUnitSelect = false;
        ComeBackRewardStep = KGameSysValTable.ComeBackStep.CbsNone;
        ComeBackBuffEndUtc = DateTime.MinValue;
    }

    public void EvaluateFromLastLogin(DateTime? lastLoginUtc, DateTime? nowUtc = null)
    {
        if (!lastLoginUtc.HasValue) return;
        int offlineDays = (int)Math.Max(0, ((nowUtc ?? DateTime.UtcNow) - lastLoginUtc.Value).TotalDays);
        var step = KGameSysValTable.EvaluateComeBackStep(offlineDays);
        if (step != KGameSysValTable.ComeBackStep.CbsNone)
        {
            IsComeBackUser = true;
            IsComeBackFirst = true;
            ComeBackRewardStep = step;
        }
    }

    public void SetComeBackBuffEndTime(string dateStr)
    {
        if (DateTime.TryParse(dateStr, out var dt))
            ComeBackBuffEndUtc = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
        else
            ComeBackBuffEndUtc = DateTime.MinValue;
    }

    public bool TryGiveComeBackRewardCube(out int rewardItemId, out DateTime buffEndUtc, out int rewardPeriodDays, DateTime? nowUtc = null)
    {
        var now = nowUtc ?? DateTime.UtcNow;
        rewardItemId = KRewardTable.GetComeBackCubeItemId(ComeBackRewardStep);
        rewardPeriodDays = KGameSysValTable.GetComeBackStepDurationDays(ComeBackRewardStep);
        if (rewardItemId <= 0 || rewardPeriodDays <= 0)
        {
            buffEndUtc = DateTime.MinValue;
            return false;
        }

        buffEndUtc = now.AddDays(rewardPeriodDays);
        ComeBackBuffEndUtc = buffEndUtc;
        IsComeBackFirst = false;
        return true;
    }

    public bool CheckComeBackBuffEnd(DateTime? nowUtc = null)
    {
        var now = nowUtc ?? DateTime.UtcNow;
        if (IsComeBackUser && ComeBackBuffEndUtc != DateTime.MinValue && ComeBackBuffEndUtc < now)
        {
            IsComeBackUser = false;
            return true;
        }
        return false;
    }
}
