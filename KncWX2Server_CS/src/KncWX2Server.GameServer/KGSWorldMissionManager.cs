using System.Diagnostics;
using System.Globalization;
using KncWX2Server.Core.Logging;
using KncWX2Server.Core.Packets;

namespace KncWX2Server.GameServer;

/// <summary>
/// Faithful C# port of GameServer/GSWorldMissionManager.{h,cpp}:
/// tracks world mission / Gate of Darkness state, crystal counts, schedule windows,
/// world buff duration calculation, and individual buff levels.
/// </summary>
public sealed class KGSWorldMissionManager
{
    public const int BuffHeroOfEliosLevel1 = 1001;
    public const int BuffHeroOfEliosLevel2 = 1002;
    public const int BuffHeroOfEliosLevel3 = 1003;
    public const int BuffHeroOfEliosLevel4 = 1004;

    public const int WbscStepNone = 0;
    public const int WbscStepOne = 3;
    public const int WbscStepTwo = 9;
    public const int WbscStepThree = 15;

    public const int DefenseRewardBuffLevel1Needs = 0;
    public const int DefenseRewardBuffLevel2Needs = 3;
    public const int DefenseRewardBuffLevel3Needs = 9;
    public const int DefenseRewardBuffLevel4Needs = 15;
    public const int DefenseRewardBuffMaxNeeds = 16;

    public const int GateOfTheDarknessHard = 7;
    public const int GateOfTheDarknessExpert = 13;

    private readonly Dictionary<int, List<KDefenseOpenTime>> _defenseOpenTime = new();
    private readonly Stopwatch _missionTimer = new();
    private DateTime _worldMissionEndTime = DateTime.MinValue;
    private DateTime _worldMissionNextStartTime = DateTime.MinValue;

    public bool IsActive { get; private set; }
    public int WorldCrystalCountRaw { get; private set; }
    public int WorldMaxCrystalCount { get; private set; }
    public int MissionTimeSeconds { get; private set; }
    public int TickCount { get; private set; }
    public ushort WorldBuff { get; private set; }
    public ushort WorldBuffDurationTime { get; private set; }
    public int IndividualBuffId { get; private set; }

    public void SetDefenseOpenSchedule(IDictionary<int, List<KDefenseOpenTime>> schedule)
    {
        _defenseOpenTime.Clear();
        foreach (var kv in schedule)
            _defenseOpenTime[kv.Key] = [.. kv.Value];
    }

    public void UpdateMissionInfo(KEGB_GET_MISSION_INFO_ACK ack, bool isBegin = false)
    {
        IsActive = ack.IsActive;
        WorldCrystalCountRaw = ack.ProtectedCrystalCount;
        WorldMaxCrystalCount = Math.Max(ack.StartedCrystalCount, ack.ProtectedCrystalCount);
        MissionTimeSeconds = ack.RemainingSeconds;
        if (ack.WorldBuffId > 0)
            WorldBuff = (ushort)ack.WorldBuffId;
        if (ack.IndividualBuffLevel > 0)
            IndividualBuffId = ack.IndividualBuffLevel;

        if (isBegin)
        {
            WorldBuff = 0;
            WorldBuffDurationTime = 0;
            _missionTimer.Restart();
        }
        else if (IsActive && !_missionTimer.IsRunning)
        {
            _missionTimer.Restart();
        }
        else if (!IsActive)
        {
            _missionTimer.Stop();
            _worldMissionEndTime = DateTime.UtcNow;
        }

        TickCount++;
    }

    public KEGS_WORLD_MISSION_UPDATE_NOT GetMissionInfo() => new()
    {
        Active = IsActive,
        RemainTime = GetRemainTime(),
        WorldCrystalCount = WorldCrystalCountRaw,
        WorldMaxCrystalCount = WorldMaxCrystalCount,
        MissionStartRemainTime = GetMissionStartRemainTime(),
    };

    public bool UpdateWorldBuffInfo(KEGB_UPDATE_WORLD_BUFF_NOT packet)
    {
        IsActive = packet.Active;
        if (packet.SuccessWorldBuff == WorldBuff)
            return false;

        WorldBuff = packet.SuccessWorldBuff;
        WorldBuffDurationTime = packet.WorldBuffDurationTime;
        return true;
    }

    public KEGS_UPDATE_WORLD_BUFF_NOT GetWorldBuff() => new()
    {
        SuccessWorldBuff = WorldBuff,
        WorldBuffDurationTime = (ushort)Math.Clamp(GetWorldBuffEndTime(), 0, ushort.MaxValue),
    };

    public int GetWorldBuffState() =>
        WorldBuffDurationTime > 0 && WorldBuff > 0 ? WorldBuff : 0;

    public int GetWorldBuffEndTime(DateTime? nowOverride = null)
    {
        DateTime now = nowOverride ?? DateTime.UtcNow;
        int dayKey = (int)now.DayOfWeek + 1; // MFC CTime::GetDayOfWeek: Sun=1..Sat=7
        if (!_defenseOpenTime.TryGetValue(dayKey, out var slots) &&
            !_defenseOpenTime.TryGetValue((int)now.DayOfWeek, out slots))
        {
            return IsActive ? WorldBuffDurationTime : 0;
        }

        foreach (var open in slots)
        {
            var start = new DateTime(now.Year, now.Month, now.Day, open.StartHour, open.StartMinute, 0, now.Kind);
            var end = start.AddMinutes(open.DurationTime + WorldBuffDurationTime);
            if (start <= now && now < end)
            {
                _worldMissionEndTime = start.AddMinutes(open.DurationTime);
                int remaining = (int)((_worldMissionEndTime - now).TotalSeconds + WorldBuffDurationTime);
                return Math.Max(0, remaining);
            }
        }
        return 0;
    }

    public bool UpdateIndividualBuffInfo(KEGB_THE_GATE_OF_DARKNESS_INDIVIDUAL_BUFF_NOT packet)
    {
        if (IndividualBuffId != packet.BuffId)
        {
            IndividualBuffId = packet.BuffId;
            return true;
        }
        return false;
    }

    public int GetWorldCrystalCount() => Math.Min(WorldCrystalCountRaw, WbscStepThree);

    public bool GetDefenseDungeonEndTime(out DateTime startTime, out DateTime endTime, DateTime? nowOverride = null)
    {
        DateTime now = nowOverride ?? DateTime.UtcNow;
        startTime = DateTime.MinValue;
        endTime = DateTime.MinValue;
        int dayKey = (int)now.DayOfWeek + 1;
        if (!_defenseOpenTime.TryGetValue(dayKey, out var slots) &&
            !_defenseOpenTime.TryGetValue((int)now.DayOfWeek, out slots))
        {
            return false;
        }

        for (int i = slots.Count - 1; i >= 0; i--)
        {
            var open = slots[i];
            startTime = new DateTime(now.Year, now.Month, now.Day, open.StartHour, open.StartMinute, 0, now.Kind);
            endTime = startTime.AddMinutes(open.DurationTime);
            if (IsActive)
            {
                if (startTime <= now && now < endTime)
                    return true;
            }
            else if (endTime <= now)
            {
                return true;
            }
        }
        return false;
    }

    public int GetRemainTime()
    {
        if (!IsActive) return 0;
        int remaining = MissionTimeSeconds - (int)_missionTimer.Elapsed.TotalSeconds;
        return Math.Max(0, remaining);
    }

    public void UpdateMissionStartTime(string dateText)
    {
        if (DateTime.TryParse(dateText, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var parsed))
            _worldMissionNextStartTime = parsed;
        else
            KLog.Cwarn($"[KGSWorldMissionManager] Failed to parse start time: {dateText}");
    }

    public int GetMissionStartRemainTime(DateTime? nowOverride = null)
    {
        DateTime now = nowOverride ?? DateTime.UtcNow;
        if (_worldMissionNextStartTime > now)
            return (int)(_worldMissionNextStartTime - now).TotalMinutes;
        if (MissionTimeSeconds == 0)
            return -1;
        return 0;
    }
}
