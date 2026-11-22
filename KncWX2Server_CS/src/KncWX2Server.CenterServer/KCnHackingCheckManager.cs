using System.Collections.Concurrent;
using KncWX2Server.Core.Scripting;
using MoonSharp.Interpreter;

namespace KncWX2Server.CenterServer;

/// <summary>
/// Faithful C# port of CenterServer/CnHackingCheckManager.{h,cpp}:
/// validates dungeon result ranks, minimum dungeon clear time, and Henir stage clear times.
/// </summary>
public sealed class KCnHackingCheckManager
{
    public enum DungeonRankType
    {
        RT_NONE = 0,
        RT_F = 1,
        RT_E = 2,
        RT_D = 3,
        RT_C = 4,
        RT_B = 5,
        RT_A = 6,
        RT_S = 7,
        RT_SS = 8,
        RT_SSS = 9,
    }

    public sealed class DungeonResultCheckConfig
    {
        public bool CheckClear { get; set; } = true;
        public int StartedMember { get; set; } = 1;
        public DungeonRankType ComboRank { get; set; } = DungeonRankType.RT_SSS;
        public DungeonRankType TechnicalRank { get; set; } = DungeonRankType.RT_SSS;
        public DungeonRankType TimeRank { get; set; } = DungeonRankType.RT_SSS;
        public DungeonRankType DamagedRank { get; set; } = DungeonRankType.RT_SSS;
        public int TechnicalScore { get; set; } = 100000;
        public int ContinueCount { get; set; }
        public bool HavePet { get; set; }
        public HashSet<byte> CheckUnitClasses { get; } = [];
        public HashSet<int> NoAdventDungeonIds { get; } = [];
        public int MinLevelCheck { get; set; } = 1;
    }

    public sealed class DungeonClearTimeCheckConfig
    {
        public bool CheckClear { get; set; } = true;
        public int StartedMember { get; set; } = 1;
        public HashSet<byte> CheckUnitClasses { get; } = [];
        public HashSet<int> NoAdventDungeonIds { get; } = [];
        public int MinLevelCheck { get; set; } = 1;
        public int MinClearPlayTimeSec { get; set; } = 15;
    }

    private readonly ConcurrentDictionary<int, int> _henirStageMinTimes = new();
    private readonly ConcurrentQueue<(long UserUid, int DungeonId, string Reason, DateTime AtUtc)> _flaggedLogs = new();
    public List<string> NotifyPhoneNums { get; } = new();

    public bool IsDungeonResultCheckEnable { get; set; } = true;
    public bool IsDungeonClearTimeCheckEnable { get; set; } = true;
    public bool IsHenirClearTimeCheckEnable { get; set; } = true;
    public DungeonResultCheckConfig ResultCheckData { get; } = new();
    public DungeonClearTimeCheckConfig ClearTimeCheckData { get; } = new();
    public IReadOnlyCollection<(long UserUid, int DungeonId, string Reason, DateTime AtUtc)> FlaggedLogs => _flaggedLogs;

    public void SetHenirStageMinimumClearTime(int stageId, int minSeconds)
        => _henirStageMinTimes[stageId] = Math.Max(0, minSeconds);

    public bool CheckDungeonResultAutoHackUser(
        int dungeonId, sbyte difficultyLevel, byte unitClass, int unitLevel,
        bool isClear, int startedMember,
        DungeonRankType comboRank, DungeonRankType technicalRank,
        DungeonRankType timeRank, DungeonRankType damagedRank,
        int technicalScore, int continueCount, bool havePet)
    {
        if (!IsDungeonResultCheckEnable) return false;
        if (ResultCheckData.NoAdventDungeonIds.Contains(dungeonId)) return false;
        if (ResultCheckData.CheckUnitClasses.Count > 0 && !ResultCheckData.CheckUnitClasses.Contains(unitClass))
            return false;
        if (unitLevel < ResultCheckData.MinLevelCheck) return false;
        if (ResultCheckData.CheckClear && !isClear) return false;
        if (startedMember < ResultCheckData.StartedMember) return false;

        if (comboRank > ResultCheckData.ComboRank ||
            technicalRank > ResultCheckData.TechnicalRank ||
            timeRank > ResultCheckData.TimeRank ||
            damagedRank > ResultCheckData.DamagedRank ||
            technicalScore > ResultCheckData.TechnicalScore)
        {
            _flaggedLogs.Enqueue((0, dungeonId, $"ResultHack:Diff={difficultyLevel},Score={technicalScore}", DateTime.UtcNow));
            return true;
        }
        return false;
    }

    public bool CheckDungeonClearTimeAutoHackUser(
        sbyte authLevel, long userUid, int dungeonId, sbyte difficultyLevel,
        byte unitClass, int unitLevel, bool isClear, int startedMember, int playTimeSeconds)
    {
        if (!IsDungeonClearTimeCheckEnable || authLevel >= 2) return false;
        if (ClearTimeCheckData.NoAdventDungeonIds.Contains(dungeonId)) return false;
        if (ClearTimeCheckData.CheckUnitClasses.Count > 0 && !ClearTimeCheckData.CheckUnitClasses.Contains(unitClass))
            return false;
        if (unitLevel < ClearTimeCheckData.MinLevelCheck) return false;
        if (ClearTimeCheckData.CheckClear && !isClear) return false;
        if (startedMember < ClearTimeCheckData.StartedMember) return false;

        if (playTimeSeconds < ClearTimeCheckData.MinClearPlayTimeSec)
        {
            _flaggedLogs.Enqueue((userUid, dungeonId, $"ClearTimeHack:PlayTime={playTimeSeconds}s", DateTime.UtcNow));
            return true;
        }
        return false;
    }

    public bool CheckHenirClearTimeHackUser(sbyte authLevel, long userUid, int dungeonId, int stageId, int playTimeSeconds)
    {
        if (!IsHenirClearTimeCheckEnable || authLevel >= 2) return false;
        if (_henirStageMinTimes.TryGetValue(stageId, out int minSec) && playTimeSeconds < minSec)
        {
            _flaggedLogs.Enqueue((userUid, dungeonId, $"HenirStageHack:Stage={stageId},Time={playTimeSeconds}s<{minSec}s", DateTime.UtcNow));
            return true;
        }
        return false;
    }

    public void BindToLua(KScriptManager script, string globalName = "g_pCnHackingCheckManager")
    {
        Table? proxy = null;
        proxy = script.CreateLuaProxy(globalName, t =>
        {
            t["AddPhoneNum"] = DynValue.NewCallback((_, a) =>
            {
                string num = KScriptManager.ArgString(a, KScriptManager.ArgStart(a, proxy!));
                if (!string.IsNullOrWhiteSpace(num) && !NotifyPhoneNums.Contains(num)) NotifyPhoneNums.Add(num);
                return DynValue.Nil;
            });
            t["SetDungeonResultCheckEnable"] = DynValue.NewCallback((_, a) =>
            {
                IsDungeonResultCheckEnable = KScriptManager.ArgBool(a, KScriptManager.ArgStart(a, proxy!), IsDungeonResultCheckEnable);
                return DynValue.Nil;
            });
            t["SetDungeonClearTimeCheckEnable"] = DynValue.NewCallback((_, a) =>
            {
                IsDungeonClearTimeCheckEnable = KScriptManager.ArgBool(a, KScriptManager.ArgStart(a, proxy!), IsDungeonClearTimeCheckEnable);
                return DynValue.Nil;
            });
            t["SetHenirClearTimeCheckEnable"] = DynValue.NewCallback((_, a) =>
            {
                IsHenirClearTimeCheckEnable = KScriptManager.ArgBool(a, KScriptManager.ArgStart(a, proxy!), IsHenirClearTimeCheckEnable);
                return DynValue.Nil;
            });
            t["SetDungeonResultHackUserCheckData"] = DynValue.NewCallback((_, a) =>
            {
                var tbl = KScriptManager.ArgTable(a, KScriptManager.ArgStart(a, proxy!));
                if (tbl != null)
                {
                    if (tbl.Get("m_bClear").Type == DataType.Boolean) ResultCheckData.CheckClear = tbl.Get("m_bClear").Boolean;
                    if (tbl.Get("m_iStartedMember").Type == DataType.Number) ResultCheckData.StartedMember = (int)tbl.Get("m_iStartedMember").Number;
                    if (tbl.Get("m_eComboRank").Type == DataType.Number) ResultCheckData.ComboRank = (DungeonRankType)(int)tbl.Get("m_eComboRank").Number;
                    if (tbl.Get("m_eTechnicalRank").Type == DataType.Number) ResultCheckData.TechnicalRank = (DungeonRankType)(int)tbl.Get("m_eTechnicalRank").Number;
                    if (tbl.Get("m_eTimeRank").Type == DataType.Number) ResultCheckData.TimeRank = (DungeonRankType)(int)tbl.Get("m_eTimeRank").Number;
                    if (tbl.Get("m_eDamagedRank").Type == DataType.Number) ResultCheckData.DamagedRank = (DungeonRankType)(int)tbl.Get("m_eDamagedRank").Number;
                    if (tbl.Get("m_iTechnicalScore").Type == DataType.Number) ResultCheckData.TechnicalScore = (int)tbl.Get("m_iTechnicalScore").Number;
                    if (tbl.Get("m_iContinueCount").Type == DataType.Number) ResultCheckData.ContinueCount = (int)tbl.Get("m_iContinueCount").Number;
                    if (tbl.Get("m_bHavePet").Type == DataType.Boolean) ResultCheckData.HavePet = tbl.Get("m_bHavePet").Boolean;
                    if (tbl.Get("m_iMinLevelCheck").Type == DataType.Number) ResultCheckData.MinLevelCheck = (int)tbl.Get("m_iMinLevelCheck").Number;

                    var ucTbl = tbl.Get("CHECK_UNIT_CLASS").Table;
                    if (ucTbl != null)
                    {
                        for (int i = 1; i <= ucTbl.Length; i++)
                        {
                            int uc = (int)(ucTbl.Get(i).CastToNumber() ?? -1);
                            if (uc >= 0) ResultCheckData.CheckUnitClasses.Add((byte)uc);
                        }
                    }

                    var noTbl = tbl.Get("NO_ADVENT_DUNGEON_TABLE").Table;
                    if (noTbl != null)
                    {
                        for (int i = 1; i <= noTbl.Length; i++)
                        {
                            int did = (int)(noTbl.Get(i).CastToNumber() ?? 0);
                            if (did > 0) ResultCheckData.NoAdventDungeonIds.Add(did);
                        }
                    }
                }
                return DynValue.Nil;
            });
            t["SetDungeonClearTimeHackUserCheckData"] = DynValue.NewCallback((_, a) =>
            {
                var tbl = KScriptManager.ArgTable(a, KScriptManager.ArgStart(a, proxy!));
                if (tbl != null)
                {
                    if (tbl.Get("m_bClear").Type == DataType.Boolean) ClearTimeCheckData.CheckClear = tbl.Get("m_bClear").Boolean;
                    if (tbl.Get("m_iStartedMember").Type == DataType.Number) ClearTimeCheckData.StartedMember = (int)tbl.Get("m_iStartedMember").Number;
                    if (tbl.Get("m_iMinLevelCheck").Type == DataType.Number) ClearTimeCheckData.MinLevelCheck = (int)tbl.Get("m_iMinLevelCheck").Number;
                    if (tbl.Get("m_iCheckPlayTime").Type == DataType.Number) ClearTimeCheckData.MinClearPlayTimeSec = (int)tbl.Get("m_iCheckPlayTime").Number;

                    var ucTbl = tbl.Get("CHECK_UNIT_CLASS").Table;
                    if (ucTbl != null)
                    {
                        for (int i = 1; i <= ucTbl.Length; i++)
                        {
                            int uc = (int)(ucTbl.Get(i).CastToNumber() ?? -1);
                            if (uc >= 0) ClearTimeCheckData.CheckUnitClasses.Add((byte)uc);
                        }
                    }

                    var noTbl = tbl.Get("NO_ADVENT_DUNGEON_TABLE").Table;
                    if (noTbl != null)
                    {
                        for (int i = 1; i <= noTbl.Length; i++)
                        {
                            int did = (int)(noTbl.Get(i).CastToNumber() ?? 0);
                            if (did > 0) ClearTimeCheckData.NoAdventDungeonIds.Add(did);
                        }
                    }
                }
                return DynValue.Nil;
            });
            t["SetHenirClearTimeHackUserCheckData"] = DynValue.NewCallback((_, a) =>
            {
                var tbl = KScriptManager.ArgTable(a, KScriptManager.ArgStart(a, proxy!));
                if (tbl != null)
                {
                    if (tbl.Length > 0)
                    {
                        for (int i = 1; i <= tbl.Length; i++)
                        {
                            var entry = tbl.Get(i).Table;
                            if (entry == null) continue;
                            int stage = (int)(entry.Get("m_iStageID").CastToNumber() ?? entry.Get("m_iStage").CastToNumber() ?? 0);
                            int minSec = (int)(entry.Get("m_iClearTime").CastToNumber() ?? entry.Get("m_iMinClearTime").CastToNumber() ?? 0);
                            if (stage > 0) SetHenirStageMinimumClearTime(stage, minSec);
                        }
                    }
                    else
                    {
                        int stage = (int)(tbl.Get("m_iStageID").CastToNumber() ?? tbl.Get("m_iStage").CastToNumber() ?? 0);
                        int minSec = (int)(tbl.Get("m_iClearTime").CastToNumber() ?? tbl.Get("m_iMinClearTime").CastToNumber() ?? 0);
                        if (stage > 0) SetHenirStageMinimumClearTime(stage, minSec);
                    }
                }
                return DynValue.Nil;
            });
        });
    }
}
