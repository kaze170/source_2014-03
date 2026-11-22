using KncWX2Server.Core.Framework;
using KncWX2Server.Core.Scripting;
using MoonSharp.Interpreter;

namespace KncWX2Server.GlobalServer;

/// <summary>
/// Faithful C# port of GlobalServer/WorldMissionManager.{h,cpp} (SERV_NEW_DEFENCE_DUNGEON):
/// coordinates weekly/daily world defense schedules, CCU-scaled crystal counts,
/// periodic crystal reduction, BuffProcess (BTI_BUFF_2013_DEFENSE_* 201/202/203),
/// individual crystal buff tiers (1..4), and random earth-quake timers via KLottery.
/// </summary>
public sealed class KWorldMissionManager
{
    public const int DefenseRewardBuffLevel1Needs = 0;
    public const int DefenseRewardBuffLevel2Needs = 3;
    public const int DefenseRewardBuffLevel3Needs = 9;
    public const int DefenseRewardBuffLevel4Needs = 15;
    public const int DefenseRewardBuffMaxNeeds = 16;

    public const int BtiNone = 0;
    public const int BtiBuff2013DefenseFakeBuff = 201;
    public const int BtiBuff2013DefenseBuff = 202;
    public const int BtiBuff2013DefenseDebuff = 203;

    public sealed record DefenseOpenTime(DayOfWeek Day, int StartHour, int StartMinute, int DurationMinutes);

    private readonly Dictionary<DayOfWeek, List<DefenseOpenTime>> _openTimes = new();
    public IReadOnlyDictionary<DayOfWeek, List<DefenseOpenTime>> OpenTimes => _openTimes;
    private readonly KLottery _earthQuakeLottery = new();
    private readonly object _sync = new();

    public bool WorldMissionOn { get; set; } = true;
    public bool IsActive { get; set; }
    public bool MissionSuccess { get; set; }
    public bool WorldBuffTime { get; private set; }
    public int StartedCrystalCount { get; private set; }
    public int ProtectedCrystalCount { get; private set; }
    public int WorldBuffId { get; private set; } = BtiNone;
    public int IndividualBuffLevel { get; private set; } = 1;
    public ushort WorldBuffDurationMinutes { get; set; } = 30;
    public float ReduceTermSeconds { get; set; } = 60.0f;
    public int ReduceCount { get; set; } = 1;
    public DateTime? MissionStartUtc { get; private set; }
    public DateTime? MissionEndUtc { get; private set; }
    public DateTime? WorldBuffEndUtc { get; private set; }

    private DateTime _lastReduceUtc = DateTime.UtcNow;

    public KWorldMissionManager()
    {
        // Default TW schedule: weekend evening defense window
        AddDefenseOpenTime(DayOfWeek.Saturday, 20, 0, 30);
        AddDefenseOpenTime(DayOfWeek.Sunday, 20, 0, 30);
        AddRandomEarthQuakeTime(180, 40.0f);
        AddRandomEarthQuakeTime(300, 60.0f);
    }

    public void AddDefenseOpenTime(DayOfWeek day, int startHour, int startMinute, int durationMinutes)
    {
        lock (_sync)
        {
            if (!_openTimes.TryGetValue(day, out var list))
            {
                list = [];
                _openTimes[day] = list;
            }
            list.Add(new DefenseOpenTime(day, startHour, startMinute, Math.Max(1, durationMinutes)));
        }
    }

    public void AddRandomEarthQuakeTime(int seconds, float ratePercent)
        => _earthQuakeLottery.AddCase(seconds, ratePercent);

    public int GetRandomEarthQuakeTime()
    {
        int res = _earthQuakeLottery.Decision();
        return res == KLottery.CaseBlank ? 240 : res;
    }

    public void StartMission(int totalCcu, int durationMinutes = 30, DateTime? nowUtc = null)
    {
        var now = nowUtc ?? DateTime.UtcNow;
        lock (_sync)
        {
            IsActive = true;
            WorldBuffTime = false;
            MissionSuccess = false;
            StartedCrystalCount = Math.Max(10, totalCcu / 10 + 10);
            ProtectedCrystalCount = 0;
            MissionStartUtc = now;
            MissionEndUtc = now.AddMinutes(Math.Max(1, durationMinutes));
            _lastReduceUtc = now;
            BuffProcessLocked(now);
        }
    }

    public void SetCrystalCount(int protectedCount)
    {
        lock (_sync)
            ProtectedCrystalCount = Math.Clamp(protectedCount, 0, DefenseRewardBuffMaxNeeds);
    }

    public void ResetProtectedCrystalCount()
    {
        lock (_sync)
            ProtectedCrystalCount = 0;
    }

    public int IncreaseProtectedCrystalCount(int delta)
    {
        lock (_sync)
        {
            ProtectedCrystalCount = Math.Clamp(ProtectedCrystalCount + delta, 0, DefenseRewardBuffMaxNeeds);
            return ProtectedCrystalCount;
        }
    }

    public int GetDefenseRewardBuffLevel()
    {
        lock (_sync)
        {
            if (ProtectedCrystalCount >= DefenseRewardBuffLevel4Needs) return 4;
            if (ProtectedCrystalCount >= DefenseRewardBuffLevel3Needs) return 3;
            if (ProtectedCrystalCount >= DefenseRewardBuffLevel2Needs) return 2;
            return 1;
        }
    }

    public void CompleteMission(bool success, int protectedCrystals, DateTime? nowUtc = null)
    {
        var now = nowUtc ?? DateTime.UtcNow;
        lock (_sync)
        {
            IsActive = false;
            MissionSuccess = success;
            ProtectedCrystalCount = Math.Clamp(protectedCrystals, 0, DefenseRewardBuffMaxNeeds);
            WorldBuffTime = success;
            if (success)
                WorldBuffEndUtc = now.AddMinutes(WorldBuffDurationMinutes);
            BuffProcessLocked(now);
        }
    }

    public bool IsWorldBuffActive(DateTime? nowUtc = null)
    {
        var now = nowUtc ?? DateTime.UtcNow;
        lock (_sync)
            return WorldBuffEndUtc.HasValue && now < WorldBuffEndUtc.Value;
    }

    public void Tick(DateTime? nowUtc = null, int totalCcu = 0)
    {
        if (!WorldMissionOn) return;
        var now = nowUtc ?? DateTime.UtcNow;
        lock (_sync)
        {
            // Check schedule activation if not currently active
            if (!IsActive && _openTimes.TryGetValue(now.DayOfWeek, out var windows))
            {
                foreach (var w in windows)
                {
                    var st = new DateTime(now.Year, now.Month, now.Day, w.StartHour, w.StartMinute, 0, DateTimeKind.Utc);
                    var ed = st.AddMinutes(w.DurationMinutes);
                    if (now >= st && now < ed && (!MissionStartUtc.HasValue || MissionStartUtc.Value < st))
                    {
                        IsActive = true;
                        WorldBuffTime = false;
                        StartedCrystalCount = Math.Max(10, totalCcu / 10 + 10);
                        ProtectedCrystalCount = 0;
                        MissionStartUtc = st;
                        MissionEndUtc = ed;
                        _lastReduceUtc = now;
                        break;
                    }
                }
            }

            if (IsActive)
            {
                if ((now - _lastReduceUtc).TotalSeconds >= ReduceTermSeconds && ReduceCount > 0)
                {
                    ProtectedCrystalCount = Math.Max(0, ProtectedCrystalCount - ReduceCount);
                    _lastReduceUtc = now;
                }

                if (MissionEndUtc.HasValue && now >= MissionEndUtc.Value)
                {
                    bool success = ProtectedCrystalCount >= DefenseRewardBuffLevel2Needs;
                    IsActive = false;
                    MissionSuccess = success;
                    WorldBuffTime = success;
                    if (success)
                        WorldBuffEndUtc = now.AddMinutes(WorldBuffDurationMinutes);
                }
            }
            else if (WorldBuffTime && WorldBuffEndUtc.HasValue && now >= WorldBuffEndUtc.Value)
            {
                WorldBuffTime = false;
            }

            BuffProcessLocked(now);
        }
    }

    private void BuffProcessLocked(DateTime now)
    {
        if (IsActive)
        {
            WorldBuffId = BtiBuff2013DefenseFakeBuff;
            IndividualBuffLevel = GetDefenseRewardBuffLevel();
        }
        else if (WorldBuffTime && WorldBuffEndUtc.HasValue && now < WorldBuffEndUtc.Value)
        {
            WorldBuffId = MissionSuccess ? BtiBuff2013DefenseBuff : BtiBuff2013DefenseDebuff;
            IndividualBuffLevel = GetDefenseRewardBuffLevel();
        }
        else
        {
            WorldBuffId = BtiNone;
        }
    }

    public void BindToLua(KScriptManager script, string globalName = "WorldMissionManager")
    {
        Table? proxy = null;
        bool clearedDefaults = false;
        proxy = script.CreateLuaProxy(globalName, t =>
        {
            t["SetReduceTerm"] = DynValue.NewCallback((_, a) =>
            {
                ReduceTermSeconds = KScriptManager.ArgFloat(a, KScriptManager.ArgStart(a, proxy!), ReduceTermSeconds);
                return DynValue.Nil;
            });
            t["SetWorldBuffDurationTime"] = DynValue.NewCallback((_, a) =>
            {
                WorldBuffDurationMinutes = (ushort)Math.Max(0, KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, proxy!), WorldBuffDurationMinutes));
                return DynValue.Nil;
            });
            t["SetDefenseOpenTime"] = DynValue.NewCallback((_, a) =>
            {
                if (!clearedDefaults)
                {
                    lock (_sync) _openTimes.Clear();
                    clearedDefaults = true;
                }
                int o = KScriptManager.ArgStart(a, proxy!);
                int wom = KScriptManager.ArgInt(a, o);
                if (wom is >= 1 and <= 7)
                {
                    AddDefenseOpenTime(
                        (DayOfWeek)(wom - 1),
                        KScriptManager.ArgInt(a, o + 1),
                        KScriptManager.ArgInt(a, o + 2),
                        KScriptManager.ArgInt(a, o + 3, 25));
                }
                return DynValue.Nil;
            });
        });
    }
}
