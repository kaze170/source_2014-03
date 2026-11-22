using KncWX2Server.Core.Packets;
using KncWX2Server.Core.Scripting;
using MoonSharp.Interpreter;

namespace KncWX2Server.GameServer;

/// <summary>
/// Faithful C# port of GameServer/TrainingCenterTable.{h,cpp}:
/// validates training-center prerequisites (`BeforeID`, level, unit class)
/// and provides dungeon IDs, time limits, and ED/EXP completion rewards.
/// </summary>
public sealed class KTrainingCenterTable
{
    public sealed class TrainingCenterEntry
    {
        public int Id { get; init; }
        public int DungeonId { get; init; }
        public int BeforeId { get; init; }
        public int RewardEd { get; init; }
        public int RewardExp { get; init; }
        public int MinLevel { get; init; } = 1;
        public float PlayTimeSeconds { get; init; } = 180.0f;
        public int Difficulty { get; init; }
        public List<sbyte> AllowedUnitClasses { get; init; } = [];
    }

    private readonly Dictionary<int, TrainingCenterEntry> _entries = new();

    public KTrainingCenterTable()
    {
        // Standard basic training stages
        AddEntry(new TrainingCenterEntry { Id = 1, DungeonId = 40001, BeforeId = 0, RewardEd = 1000, RewardExp = 500, MinLevel = 1, PlayTimeSeconds = 180f });
        AddEntry(new TrainingCenterEntry { Id = 2, DungeonId = 40002, BeforeId = 1, RewardEd = 2000, RewardExp = 1200, MinLevel = 3, PlayTimeSeconds = 240f });
        AddEntry(new TrainingCenterEntry { Id = 3, DungeonId = 40003, BeforeId = 2, RewardEd = 3500, RewardExp = 2500, MinLevel = 5, PlayTimeSeconds = 300f });
    }

    public void AddEntry(TrainingCenterEntry entry) => _entries[entry.Id] = entry;

    public bool CheckIfEnter(IReadOnlyDictionary<int, KTCClearInfo> cleared, int tcId, byte unitLevel = 99, sbyte unitClass = -1)
    {
        if (!_entries.TryGetValue(tcId, out var entry))
            return false;
        if (unitLevel < entry.MinLevel)
            return false;
        if (entry.BeforeId > 0 && !cleared.ContainsKey(entry.BeforeId))
            return false;
        if (unitClass >= 0 && entry.AllowedUnitClasses.Count > 0 && !entry.AllowedUnitClasses.Contains(unitClass))
            return false;
        return true;
    }

    public int GetDungeonId(int tcId)
        => _entries.TryGetValue(tcId, out var e) ? e.DungeonId : -1;

    public float GetPlayTime(int tcId)
        => _entries.TryGetValue(tcId, out var e) ? e.PlayTimeSeconds : 0f;

    public bool TryGetReward(int tcId, out int rewardEd, out int rewardExp)
    {
        if (_entries.TryGetValue(tcId, out var e))
        {
            rewardEd = e.RewardEd;
            rewardExp = e.RewardExp;
            return true;
        }
        rewardEd = 0;
        rewardExp = 0;
        return false;
    }

    public bool IsTrainingCenterDungeon(int dungeonId)
        => _entries.Values.Any(e => e.DungeonId == dungeonId);

    public Dictionary<int, List<int>> TrainingListByUnitClass { get; } = new();

    public void BindToLua(KScriptManager script, string globalName = "KTrainingCenterTable")
    {
        Table? proxy = null;
        proxy = script.CreateLuaProxy(globalName, t =>
        {
            t["AddTCTemplet"] = DynValue.NewCallback((_, a) =>
            {
                var tbl = KScriptManager.ArgTable(a, KScriptManager.ArgStart(a, proxy!));
                if (tbl != null)
                {
                    int id = (int)(tbl.Get("m_iID").CastToNumber() ?? 0);
                    int dungeonId = (int)(tbl.Get("m_iDungeonID").CastToNumber() ?? 0);
                    int minLv = (int)(tbl.Get("m_iMinLevel").CastToNumber() ?? 1);
                    int beforeId = (int)(tbl.Get("m_iBeforeID").CastToNumber() ?? 0);
                    float playTime = (float)(tbl.Get("m_fPlayTime").CastToNumber() ?? 180.0);
                    int difficulty = (int)(tbl.Get("m_iDifficulty").CastToNumber() ?? 0);

                    var allowedClasses = new List<sbyte>();
                    var ucTbl = tbl.Get("UNIT_CLASS").Table;
                    if (ucTbl != null)
                    {
                        for (int i = 1; i <= ucTbl.Length; i++)
                        {
                            int uc = (int)(ucTbl.Get(i).CastToNumber() ?? -1);
                            if (uc >= 0) allowedClasses.Add((sbyte)uc);
                        }
                    }

                    int rewardEd = 0, rewardExp = 0;
                    var rewTbl = tbl.Get("REWARD").Table;
                    if (rewTbl != null)
                    {
                        rewardEd = (int)(rewTbl.Get("m_iRewardED").CastToNumber() ?? 0);
                        rewardExp = (int)(rewTbl.Get("m_iRewardEXP").CastToNumber() ?? 0);
                    }

                    if (id > 0)
                    {
                        AddEntry(new TrainingCenterEntry
                        {
                            Id = id,
                            DungeonId = dungeonId,
                            BeforeId = beforeId,
                            RewardEd = rewardEd,
                            RewardExp = rewardExp,
                            MinLevel = minLv,
                            PlayTimeSeconds = playTime,
                            Difficulty = difficulty,
                            AllowedUnitClasses = allowedClasses,
                        });
                    }
                }
                return DynValue.Nil;
            });
            t["SetTrainingListByUnitClass_LUA"] = DynValue.NewCallback((_, a) =>
            {
                var tbl = KScriptManager.ArgTable(a, KScriptManager.ArgStart(a, proxy!));
                if (tbl != null)
                {
                    int unitClass = (int)(tbl.Get("UNIT_CLASS").CastToNumber() ?? tbl.Get(1).CastToNumber() ?? -1);
                    var list = new List<int>();
                    var tcTbl = tbl.Get("TC_LIST").Table ?? tbl;
                    for (int i = 1; i <= tcTbl.Length; i++)
                    {
                        int tcId = (int)(tcTbl.Get(i).CastToNumber() ?? 0);
                        if (tcId > 0) list.Add(tcId);
                    }
                    if (unitClass >= 0)
                        TrainingListByUnitClass[unitClass] = list;
                }
                return DynValue.Nil;
            });
        });
    }
}
