using System.Collections.Concurrent;
using System.Globalization;
using KncWX2Server.Core.Scripting;
using MoonSharp.Interpreter;

namespace KncWX2Server.Core.Framework;

/// <summary>
/// Faithful C# port of Common/GameSysVal/GameSysVal.{h,cpp}:
/// centralized game system constants, PCBang multipliers, VIP multipliers,
/// ComeBack user duration tiers, level caps, and security monitoring toggles.
/// </summary>
public sealed class KGameSysValTable
{
    public enum GameSysValueType
    {
        GsvtGbExp = 0,
        GsvtGbEd = 1,
        GsvtGbVp = 2,
        GsvtGbSpirit = 3,
        GsvtPremiumExp = 4,
        GsvtGbPetExp = 5,
        GsvtVipExp = 6,
        GsvtTotalNum = 7,
    }

    public enum ComeBackStep
    {
        CbsNone = 0,
        CbsOne = 1,
        CbsTwo = 2,
        CbsThree = 3,
    }

    public enum ComeBackStepDuration
    {
        CbsdNone = 0,
        CbsdOne = 15,
        CbsdTwo = 30,
        CbsdThree = 60,
    }

    public sealed class PcBangRewardProfile
    {
        public int PcBangType { get; init; }
        public float ExpRate { get; set; } = 0.30f;
        public float EdRate { get; set; } = 0.20f;
        public float VpRate { get; set; } = 0.20f;
        public float PetExpRate { get; set; } = 0.30f;
        public bool IsSpiritFree { get; set; }
        public bool IsSecretDungeonFree { get; set; } = true;
        public int AdditionalHenirRewardCount { get; set; } = 2;
        public bool IsUsingPcBangAvatar { get; set; } = true;
        public bool IsUsingPcBangTitle { get; set; } = true;
    }

    public sealed class AttractionItemTimeEventInfo
    {
        public int SourceItemId { get; init; }
        public int EventAttractionItemId { get; init; }
        public DateTime BeginUtc { get; init; }
        public DateTime EndUtc { get; init; }
    }

    private readonly float[] _values = new float[(int)GameSysValueType.GsvtTotalNum];
    private readonly ConcurrentDictionary<int, PcBangRewardProfile> _pcBangProfiles = new();
    private readonly List<AttractionItemTimeEventInfo> _attractionTimeEvents = [];
    private readonly object _sync = new();

    public float CheckPowerMultiplayer { get; set; } = 1.0f;
    public int LimitsMaxLevel { get; set; } = 70;
    public int GuestUserLimitLevel { get; set; } = 20;
    public string AdvertisementUrl { get; set; } = string.Empty;

    public bool IsHackUserRegRejected { get; set; }
    public bool IsHackUserRegMonitors { get; set; } = true;
    public bool IsHackUserKick { get; set; }
    public bool IsHackUserBlockTrade { get; set; } = true;
    public bool IsHackUserPacketMonitor { get; set; } = true;

    public bool BlockPersonalTrade { get; set; }
    public bool BlockPersonalShop { get; set; }
    public bool BlockSendLetter { get; set; }
    public bool MachineIdBlock { get; set; }
    public bool MachineIdDuplicateCheck { get; set; }
    public bool IsNewUnitTradeBlock { get; set; }
    public int NewUnitTradeBlockDay { get; set; } = 5;
    public int NewUnitTradeBlockUnitClass { get; set; }
    public bool DisagreeHackUserFunc { get; set; }
    public bool LanBugOutJustLog { get; set; } = true;
    public bool SubStageNpcDieCheck { get; set; }
    public bool ProcessCommunicationEnabled { get; set; } = true;
    public float CheckChangeHostTimeSec { get; set; } = 180f;
    public float HostCheckTermSec { get; set; } = 3.0f;
    public int MaxPingScore { get; set; } = 5000;
    public bool LetterAndTradeMonitoringIsOn { get; set; } = true;
    public int LimitsCountOfLetter { get; set; } = 30;
    public int LetterMonitoringCheckMinutes { get; set; } = 20;
    public int LimitsCountOfTrade { get; set; } = 20;
    public int TradeMonitoringCheckMinutes { get; set; } = 20;
    public int MonitoringMinEd { get; set; } = 400_000;
    public int MonitoringMaxEd { get; set; } = 7_000_000;
    public int MonitoringBlockStartHour { get; set; } = 5;
    public int MonitoringBlockStartMinute { get; set; }
    public int MonitoringBlockStartSecond { get; set; }
    public int MonitoringBlockDurationSec { get; set; } = 10800;
    public int StorageMessageDeleteTerm1Sec { get; set; } = 100;
    public int StorageMessageDeleteTerm2Sec { get; set; } = 300;
    public int StorageMessageDeleteTerm3Sec { get; set; } = 5;
    public string CharStatisticsStartDate { get; set; } = string.Empty;
    public string AccountStatisticsStartDate { get; set; } = string.Empty;
    public bool LanBugOutCheckEnabled { get; set; }
    public byte LanBugOutCheckMaxCount { get; set; }
    public float LanBugOutCheckTime { get; set; } = 1.0f;
    public byte LanBugOutCheckCount { get; set; } = 10;
    public float LanBugOutCheckTermMin { get; set; } = 1.0f;
    public float LanBugOutCheckTerm { get; set; }
    public float LanBugOutCheckRepeatTerm { get; set; } = 0.5f;
    public bool LanBugOutVerifyEnabled { get; set; }
    public byte LanBugOutVerifyMaxCount { get; set; } = 10;
    public float LanBugOutVerifyTime { get; set; } = 1.0f;
    public byte LanBugOutVerifyCount { get; set; } = 10;
    public float LanBugOutVerifyTermMin { get; set; } = 1.0f;
    public float LanBugOutVerifyTerm { get; set; }
    public float LanBugOutVerifyRepeatTerm { get; set; } = 0.5f;
    public bool IsDeserializeFailCheck { get; set; } = true;
    public int DeserializeFailCheckCount { get; set; } = 10;
    public bool IsEnableOpenRandomCubeDelaySystem { get; set; }
    public double OpenRandomCubeDelayTime { get; set; } = 1.0;
    public int OpenRandomCubeDelayCount { get; set; } = 100;

    private readonly ConcurrentDictionary<int, int> _comeBackRewardDays = new();
    private readonly ConcurrentDictionary<int, List<int>> _pcBangPremiumItems = new();
    private long _nextTempItemUid = 1_000_000_000L;
    public IReadOnlyDictionary<int, int> ComeBackRewardDays => _comeBackRewardDays;
    public long NextTempItemUid() => Interlocked.Increment(ref _nextTempItemUid);

    public IReadOnlyList<int> GetPcBangPremiumItems(int unitType, int pcBangType = -1)
    {
        if (pcBangType >= 0 && !(GetPcBangReward(pcBangType)?.IsUsingPcBangAvatar ?? false))
            return [];
        return _pcBangPremiumItems.TryGetValue(unitType, out var list) ? list : [];
    }

    public KGameSysValTable()
    {
        _values[(int)GameSysValueType.GsvtGbExp] = 0.30f;
        _values[(int)GameSysValueType.GsvtGbEd] = 0.20f;
        _values[(int)GameSysValueType.GsvtGbVp] = 0.20f;
        _values[(int)GameSysValueType.GsvtGbSpirit] = 0.50f;
        _values[(int)GameSysValueType.GsvtPremiumExp] = 0.50f;
        _values[(int)GameSysValueType.GsvtGbPetExp] = 0.30f;
        _values[(int)GameSysValueType.GsvtVipExp] = 0.20f;

        _pcBangProfiles[0] = new PcBangRewardProfile { PcBangType = 0 };
        _pcBangProfiles[1] = new PcBangRewardProfile { PcBangType = 1, ExpRate = 0.50f, IsSpiritFree = true };
    }

    public void SetValue(GameSysValueType type, float value)
    {
        int idx = (int)type;
        if (idx >= 0 && idx < _values.Length)
            _values[idx] = value;
    }

    public void RegisterPcBangProfile(PcBangRewardProfile profile)
        => _pcBangProfiles[profile.PcBangType] = profile;

    public PcBangRewardProfile? GetPcBangReward(int pcBangType = 0)
        => _pcBangProfiles.TryGetValue(pcBangType, out var prof) ? prof : _pcBangProfiles.GetValueOrDefault(0);

    public float GetGbExpRate(int pcBangType = -1)
        => pcBangType >= 0 ? (GetPcBangReward(pcBangType)?.ExpRate ?? 0f) : _values[(int)GameSysValueType.GsvtGbExp];

    public float GetGbEdRate(int pcBangType = -1)
        => pcBangType >= 0 ? (GetPcBangReward(pcBangType)?.EdRate ?? 0f) : _values[(int)GameSysValueType.GsvtGbEd];

    public float GetGbVpRate(int pcBangType = -1)
        => pcBangType >= 0 ? (GetPcBangReward(pcBangType)?.VpRate ?? 0f) : _values[(int)GameSysValueType.GsvtGbVp];

    public float GetGbPetExpRate(int pcBangType = -1)
        => pcBangType >= 0 ? (GetPcBangReward(pcBangType)?.PetExpRate ?? 0f) : _values[(int)GameSysValueType.GsvtGbPetExp];

    public bool IsSpiritFree(int pcBangType = -1)
        => pcBangType >= 0 && (GetPcBangReward(pcBangType)?.IsSpiritFree ?? false);

    public bool IsSecretDungeonFree(int pcBangType = -1)
        => pcBangType >= 0 && (GetPcBangReward(pcBangType)?.IsSecretDungeonFree ?? false);

    public int GetAdditionalHenirRewardCount(int pcBangType = -1)
        => pcBangType >= 0 ? (GetPcBangReward(pcBangType)?.AdditionalHenirRewardCount ?? 0) : 0;

    public bool IsUsingPcBangTitle(int pcBangType = -1)
        => pcBangType >= 0 && (GetPcBangReward(pcBangType)?.IsUsingPcBangTitle ?? false);

    public bool IsMonitoringLetterAndTradeBlockTime(TimeSpan timeOfDay)
    {
        if (MonitoringBlockDurationSec <= 0) return false;
        var start = new TimeSpan(MonitoringBlockStartHour, MonitoringBlockStartMinute, MonitoringBlockStartSecond);
        var end = start.Add(TimeSpan.FromSeconds(MonitoringBlockDurationSec));
        if (end.TotalDays >= 1.0)
        {
            var wrappedEnd = TimeSpan.FromTicks(end.Ticks % TimeSpan.TicksPerDay);
            return timeOfDay >= start || timeOfDay <= wrappedEnd;
        }
        return timeOfDay >= start && timeOfDay <= end;
    }

    public float GetPremiumExpRate() => _values[(int)GameSysValueType.GsvtPremiumExp];
    public float GetVipExpRate() => _values[(int)GameSysValueType.GsvtVipExp];

    public void AddAttractionTimeEvent(AttractionItemTimeEventInfo info)
    {
        lock (_sync) _attractionTimeEvents.Add(info);
    }

    public bool IsAttractionTimeEventActive(int itemId, DateTime? nowUtc = null)
    {
        var now = nowUtc ?? DateTime.UtcNow;
        lock (_sync)
            return _attractionTimeEvents.Any(e =>
                (e.EventAttractionItemId == itemId || e.SourceItemId == itemId) &&
                (e.BeginUtc == DateTime.MinValue || now >= e.BeginUtc) &&
                (e.EndUtc == DateTime.MinValue || now < e.EndUtc));
    }

    /// <summary>
    /// Faithful port of KGameSysVal::CheckAttractionItemTimeEvent (GameSysVal.cpp:2437):
    /// resolves a source attraction/cube item ID to its active time-event replacement item ID.
    /// </summary>
    public int ResolveAttractionItemTimeEvent(int itemId, DateTime? nowUtc = null)
    {
        var now = nowUtc ?? DateTime.UtcNow;
        lock (_sync)
        {
            foreach (var ev in _attractionTimeEvents)
            {
                if (ev.SourceItemId != itemId) continue;
                if (ev.BeginUtc != DateTime.MinValue && now < ev.BeginUtc) continue;
                if (ev.EndUtc != DateTime.MinValue && ev.EndUtc <= now) continue;
                return ev.EventAttractionItemId;
            }
        }
        return itemId;
    }

    public static int GetComeBackStepDurationDays(ComeBackStep step) => step switch
    {
        ComeBackStep.CbsOne => (int)ComeBackStepDuration.CbsdOne,
        ComeBackStep.CbsTwo => (int)ComeBackStepDuration.CbsdTwo,
        ComeBackStep.CbsThree => (int)ComeBackStepDuration.CbsdThree,
        _ => 0,
    };

    public static ComeBackStep EvaluateComeBackStep(int offlineDays) => offlineDays switch
    {
        >= 60 => ComeBackStep.CbsThree,
        >= 30 => ComeBackStep.CbsTwo,
        >= 15 => ComeBackStep.CbsOne,
        _ => ComeBackStep.CbsNone,
    };

    public void BindToLua(KScriptManager script, string globalName = "GameSysVal")
    {
        Table? proxy = null;
        proxy = script.CreateLuaProxy(globalName, t =>
        {
            t["AddLimitsMaxLevel"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                LimitsMaxLevel = Math.Max(1, KScriptManager.ArgInt(a, o, LimitsMaxLevel));
                return DynValue.Nil;
            });
            t["AddPremiumEXP"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                SetValue(GameSysValueType.GsvtPremiumExp, KScriptManager.ArgFloat(a, o));
                return DynValue.Nil;
            });
            t["AddVIPEXP"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                SetValue(GameSysValueType.GsvtVipExp, KScriptManager.ArgFloat(a, o));
                return DynValue.Nil;
            });
            t["AddKNXGameBang"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                SetValue(GameSysValueType.GsvtGbExp, KScriptManager.ArgFloat(a, o));
                SetValue(GameSysValueType.GsvtGbEd, KScriptManager.ArgFloat(a, o + 1));
                SetValue(GameSysValueType.GsvtGbVp, KScriptManager.ArgFloat(a, o + 2));
                SetValue(GameSysValueType.GsvtGbSpirit, KScriptManager.ArgFloat(a, o + 3));
                return DynValue.Nil;
            });
            t["AddKNXGameBangPetEXP"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                SetValue(GameSysValueType.GsvtGbPetExp, KScriptManager.ArgFloat(a, o));
                return DynValue.Nil;
            });
            t["AddPCBang"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                RegisterPcBangProfile(new PcBangRewardProfile
                {
                    PcBangType = KScriptManager.ArgInt(a, o),
                    ExpRate = KScriptManager.ArgFloat(a, o + 1),
                    EdRate = KScriptManager.ArgFloat(a, o + 2),
                    VpRate = KScriptManager.ArgFloat(a, o + 3),
                    IsSpiritFree = KScriptManager.ArgBool(a, o + 4),
                    PetExpRate = KScriptManager.ArgFloat(a, o + 5),
                    AdditionalHenirRewardCount = KScriptManager.ArgInt(a, o + 6),
                    IsSecretDungeonFree = KScriptManager.ArgBool(a, o + 7),
                    IsUsingPcBangAvatar = KScriptManager.ArgBool(a, o + 8),
                    IsUsingPcBangTitle = KScriptManager.ArgBool(a, o + 9),
                });
                return DynValue.Nil;
            });
            t["SetGuestUserLimitLevel"] = DynValue.NewCallback((_, a) => { GuestUserLimitLevel = KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, proxy!), GuestUserLimitLevel); return DynValue.Nil; });
            t["SetIsHackUserRegRejected"] = DynValue.NewCallback((_, a) => { IsHackUserRegRejected = KScriptManager.ArgBool(a, KScriptManager.ArgStart(a, proxy!)); return DynValue.Nil; });
            t["SetIsHackUserRegMonitors"] = DynValue.NewCallback((_, a) => { IsHackUserRegMonitors = KScriptManager.ArgBool(a, KScriptManager.ArgStart(a, proxy!)); return DynValue.Nil; });
            t["SetIsHackUserKick"] = DynValue.NewCallback((_, a) => { IsHackUserKick = KScriptManager.ArgBool(a, KScriptManager.ArgStart(a, proxy!)); return DynValue.Nil; });
            t["SetHackingUserKick"] = DynValue.NewCallback((_, a) => { IsHackUserKick = KScriptManager.ArgBool(a, KScriptManager.ArgStart(a, proxy!)); return DynValue.Nil; });
            t["SetIsHackUserBlockTrade"] = DynValue.NewCallback((_, a) => { IsHackUserBlockTrade = KScriptManager.ArgBool(a, KScriptManager.ArgStart(a, proxy!)); return DynValue.Nil; });
            t["SetHackUserPacketMornitor"] = DynValue.NewCallback((_, a) => { IsHackUserPacketMonitor = KScriptManager.ArgBool(a, KScriptManager.ArgStart(a, proxy!)); return DynValue.Nil; });
            t["SetBlockPersonalTrade"] = DynValue.NewCallback((_, a) => { BlockPersonalTrade = KScriptManager.ArgBool(a, KScriptManager.ArgStart(a, proxy!)); return DynValue.Nil; });
            t["SetBlockPersonalShop"] = DynValue.NewCallback((_, a) => { BlockPersonalShop = KScriptManager.ArgBool(a, KScriptManager.ArgStart(a, proxy!)); return DynValue.Nil; });
            t["SetBlockSendLetter"] = DynValue.NewCallback((_, a) => { BlockSendLetter = KScriptManager.ArgBool(a, KScriptManager.ArgStart(a, proxy!)); return DynValue.Nil; });
            t["SetMachineIDBlock"] = DynValue.NewCallback((_, a) => { MachineIdBlock = KScriptManager.ArgBool(a, KScriptManager.ArgStart(a, proxy!)); return DynValue.Nil; });
            t["SetMachineIDDuplicateCheck"] = DynValue.NewCallback((_, a) => { MachineIdDuplicateCheck = KScriptManager.ArgBool(a, KScriptManager.ArgStart(a, proxy!)); return DynValue.Nil; });
            t["SetIsNewUnitTradeBlock"] = DynValue.NewCallback((_, a) => { IsNewUnitTradeBlock = KScriptManager.ArgBool(a, KScriptManager.ArgStart(a, proxy!)); return DynValue.Nil; });
            t["SetNewUnitTradeBlockDay"] = DynValue.NewCallback((_, a) => { NewUnitTradeBlockDay = KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, proxy!), NewUnitTradeBlockDay); return DynValue.Nil; });
            t["SetNewUnitTradeBlockUnitClass"] = DynValue.NewCallback((_, a) => { NewUnitTradeBlockUnitClass = KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, proxy!), NewUnitTradeBlockUnitClass); return DynValue.Nil; });
            t["SetDisagreeHackUserFunc"] = DynValue.NewCallback((_, a) => { DisagreeHackUserFunc = KScriptManager.ArgBool(a, KScriptManager.ArgStart(a, proxy!)); return DynValue.Nil; });
            t["SetLanBugOutJustLog"] = DynValue.NewCallback((_, a) => { LanBugOutJustLog = KScriptManager.ArgBool(a, KScriptManager.ArgStart(a, proxy!)); return DynValue.Nil; });
            t["SetSubStageNpcDieCheck"] = DynValue.NewCallback((_, a) => { SubStageNpcDieCheck = KScriptManager.ArgBool(a, KScriptManager.ArgStart(a, proxy!)); return DynValue.Nil; });
            t["SetProcessCommunication"] = DynValue.NewCallback((_, a) => { ProcessCommunicationEnabled = KScriptManager.ArgBool(a, KScriptManager.ArgStart(a, proxy!)); return DynValue.Nil; });
            t["SetCheckChangeHostTime"] = DynValue.NewCallback((_, a) => { CheckChangeHostTimeSec = KScriptManager.ArgFloat(a, KScriptManager.ArgStart(a, proxy!), CheckChangeHostTimeSec); return DynValue.Nil; });
            t["SetHostCheckTerm"] = DynValue.NewCallback((_, a) => { HostCheckTermSec = KScriptManager.ArgFloat(a, KScriptManager.ArgStart(a, proxy!), HostCheckTermSec); return DynValue.Nil; });
            t["SetMaxPingScore"] = DynValue.NewCallback((_, a) => { MaxPingScore = KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, proxy!), MaxPingScore); return DynValue.Nil; });
            t["SetLetterAndTradeMonitoringIsOn"] = DynValue.NewCallback((_, a) => { LetterAndTradeMonitoringIsOn = KScriptManager.ArgBool(a, KScriptManager.ArgStart(a, proxy!)); return DynValue.Nil; });
            t["SetLimitsCountOfLetter"] = DynValue.NewCallback((_, a) => { LimitsCountOfLetter = KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, proxy!), LimitsCountOfLetter); return DynValue.Nil; });
            t["SetLetterMonitoringCheckTime"] = DynValue.NewCallback((_, a) => { LetterMonitoringCheckMinutes = KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, proxy!), LetterMonitoringCheckMinutes); return DynValue.Nil; });
            t["SetLimitsCountOfTrade"] = DynValue.NewCallback((_, a) => { LimitsCountOfTrade = KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, proxy!), LimitsCountOfTrade); return DynValue.Nil; });
            t["SetTradeMonitoringCheckTime"] = DynValue.NewCallback((_, a) => { TradeMonitoringCheckMinutes = KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, proxy!), TradeMonitoringCheckMinutes); return DynValue.Nil; });
            t["SetMonitoringMinED"] = DynValue.NewCallback((_, a) => { MonitoringMinEd = KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, proxy!), MonitoringMinEd); return DynValue.Nil; });
            t["SetMonitoringMaxED"] = DynValue.NewCallback((_, a) => { MonitoringMaxEd = KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, proxy!), MonitoringMaxEd); return DynValue.Nil; });
            t["SetMonitoringLetterAndTradeBlockTime"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                MonitoringBlockStartHour = KScriptManager.ArgInt(a, o, MonitoringBlockStartHour);
                MonitoringBlockStartMinute = KScriptManager.ArgInt(a, o + 1, MonitoringBlockStartMinute);
                MonitoringBlockStartSecond = KScriptManager.ArgInt(a, o + 2, MonitoringBlockStartSecond);
                MonitoringBlockDurationSec = KScriptManager.ArgInt(a, o + 3, MonitoringBlockDurationSec);
                return DynValue.Nil;
            });
            t["AfterWorkStorageMessageDeleteTerm_Second"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                StorageMessageDeleteTerm1Sec = KScriptManager.ArgInt(a, o, StorageMessageDeleteTerm1Sec);
                StorageMessageDeleteTerm2Sec = KScriptManager.ArgInt(a, o + 1, StorageMessageDeleteTerm2Sec);
                StorageMessageDeleteTerm3Sec = KScriptManager.ArgInt(a, o + 2, StorageMessageDeleteTerm3Sec);
                return DynValue.Nil;
            });
            t["SetComeBackReward"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                int step = KScriptManager.ArgInt(a, o);
                int days = KScriptManager.ArgInt(a, o + 1);
                if (step > 0 && days > 0)
                    _comeBackRewardDays[step] = days;
                return DynValue.Nil;
            });
            t["AddPcBangPremiumItem"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                int unitType = KScriptManager.ArgInt(a, o);
                int itemId = KScriptManager.ArgInt(a, o + 1);
                if (unitType > 0 && itemId > 0)
                {
                    var list = _pcBangPremiumItems.GetOrAdd(unitType, _ => []);
                    lock (list) list.Add(itemId);
                }
                return DynValue.Nil;
            });
            t["SetCharStatisticsStartDate"] = DynValue.NewCallback((_, a) => { CharStatisticsStartDate = KScriptManager.ArgString(a, KScriptManager.ArgStart(a, proxy!)); return DynValue.Nil; });
            t["SetAccountStatisticsStartDate"] = DynValue.NewCallback((_, a) => { AccountStatisticsStartDate = KScriptManager.ArgString(a, KScriptManager.ArgStart(a, proxy!)); return DynValue.Nil; });
            t["SetLanBugOutCheckOption"] = DynValue.NewCallback((_, a) =>
            {
                var tbl = KScriptManager.ArgTable(a, KScriptManager.ArgStart(a, proxy!));
                if (tbl != null)
                {
                    LanBugOutCheckEnabled = tbl.Get("bLanBugOutCheck").CastToBool();
                    LanBugOutCheckMaxCount = (byte)(tbl.Get("byteLanBugOutCheckMaxCount").CastToNumber() ?? 0);
                    LanBugOutCheckTime = (float)(tbl.Get("fLanBugOutCheckTime").CastToNumber() ?? 1.0);
                    LanBugOutCheckCount = (byte)(tbl.Get("byteLanBugOutCheckCount").CastToNumber() ?? 10);
                    LanBugOutCheckTermMin = (float)(tbl.Get("fLanBugOutCheckTermMin").CastToNumber() ?? 1.0);
                    LanBugOutCheckTerm = (float)(tbl.Get("fLanBugOutCheckTerm").CastToNumber() ?? 0.0);
                    LanBugOutCheckRepeatTerm = (float)(tbl.Get("fLanBugOutCheckRepeatTerm").CastToNumber() ?? 0.5);
                }
                return DynValue.Nil;
            });
            t["SetLanBugOutVerifyOption"] = DynValue.NewCallback((_, a) =>
            {
                var tbl = KScriptManager.ArgTable(a, KScriptManager.ArgStart(a, proxy!));
                if (tbl != null)
                {
                    LanBugOutVerifyEnabled = tbl.Get("bLanBugOutVerify").CastToBool();
                    LanBugOutVerifyMaxCount = (byte)(tbl.Get("byteLanBugOutVerifyMaxCount").CastToNumber() ?? 10);
                    LanBugOutVerifyTime = (float)(tbl.Get("fLanBugOutVerifyTime").CastToNumber() ?? 1.0);
                    LanBugOutVerifyCount = (byte)(tbl.Get("byteLanBugOutVerifyCount").CastToNumber() ?? 10);
                    LanBugOutVerifyTermMin = (float)(tbl.Get("fLanBugOutVerifyTermMin").CastToNumber() ?? 1.0);
                    LanBugOutVerifyTerm = (float)(tbl.Get("fLanBugOutVerifyTerm").CastToNumber() ?? 0.0);
                    LanBugOutVerifyRepeatTerm = (float)(tbl.Get("fLanBugOutVerifyRepeatTerm").CastToNumber() ?? 0.5);
                }
                return DynValue.Nil;
            });
            t["SetDeserializeFailCheck"] = DynValue.NewCallback((_, a) => { IsDeserializeFailCheck = KScriptManager.ArgBool(a, KScriptManager.ArgStart(a, proxy!)); return DynValue.Nil; });
            t["SetDeserializeFailCheckCount"] = DynValue.NewCallback((_, a) => { DeserializeFailCheckCount = KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, proxy!), DeserializeFailCheckCount); return DynValue.Nil; });
            t["SetEnableOpenRandomCubeDelaySystem"] = DynValue.NewCallback((_, a) => { IsEnableOpenRandomCubeDelaySystem = KScriptManager.ArgBool(a, KScriptManager.ArgStart(a, proxy!)); return DynValue.Nil; });
            t["SetOpenRandomCubeDelayTime"] = DynValue.NewCallback((_, a) => { OpenRandomCubeDelayTime = KScriptManager.ArgDouble(a, KScriptManager.ArgStart(a, proxy!), OpenRandomCubeDelayTime); return DynValue.Nil; });
            t["SetOpenRandomCubeDelayCount"] = DynValue.NewCallback((_, a) => { OpenRandomCubeDelayCount = KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, proxy!), OpenRandomCubeDelayCount); return DynValue.Nil; });
            t["SetAdvertisementURL"] = DynValue.NewCallback((_, a) => { AdvertisementUrl = KScriptManager.ArgString(a, KScriptManager.ArgStart(a, proxy!)); return DynValue.Nil; });
            t["SetAttractionItemTimeEvent"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                int srcItemId = KScriptManager.ArgInt(a, o);
                int dstItemId = KScriptManager.ArgInt(a, o + 1);
                var tbl = KScriptManager.ArgTable(a, o + 2);
                if (srcItemId > 0 && dstItemId > 0 && tbl != null)
                {
                    DateTime.TryParse(tbl.Get("wstrBeginDate").String, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var beginUtc);
                    DateTime.TryParse(tbl.Get("wstrEndDate").String, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var endUtc);
                    AddAttractionTimeEvent(new AttractionItemTimeEventInfo
                    {
                        SourceItemId = srcItemId,
                        EventAttractionItemId = dstItemId,
                        BeginUtc = beginUtc == default ? DateTime.MinValue : beginUtc.ToUniversalTime(),
                        EndUtc = endUtc == default ? DateTime.MinValue : endUtc.ToUniversalTime(),
                    });
                }
                return DynValue.Nil;
            });
        });
    }
}
