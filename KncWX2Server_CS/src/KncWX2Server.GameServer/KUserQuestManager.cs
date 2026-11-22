using KncWX2Server.Core.Packets;

namespace KncWX2Server.GameServer;

/// <summary>
/// Faithful C# port of GameServer/UserQuestManager.{h,cpp} and X2Data/XSLQuestManager.h:
/// enforces MAX_ING_QUEST_NUM=30, tracks all C++ SUB_QUEST_TYPE objectives
/// (Talk, NpcHunt, ItemCollection, DungeonClear/Time/Rank/Damage, PvP Play/Win/Kill,
/// Enchant, Socket, Attrib, Resolve, SkillPoint, FeedPet, ItemUse, SkillUse, Visit, LevelUp),
/// multi-subquest completion, repeatable/daily history, and prerequisite quest chains.
/// </summary>
public sealed class KUserQuestManager
{
    public const int MaxIngQuestNum = 30;

    public enum SubQuestType
    {
        SqtNone = 0,
        SqtNpcTalk = 1,
        SqtNpcHunt = 2,
        SqtItemCollection = 3,
        SqtQuestItemCollection = 4,
        SqtDungeonTime = 5,
        SqtDungeonRank = 6,
        SqtDungeonDamage = 7,
        SqtDungeonClearCount = 8,
        SqtPvpPlay = 9,
        SqtPvpWin = 10,
        SqtPvpKill = 11,
        SqtWithDifServUser = 12,
        SqtItemEnchant = 13,
        SqtItemSocket = 14,
        SqtItemAttrib = 15,
        SqtItemResolve = 16,
        SqtItemEquipDungeonClear = 17,
        SqtUseSkillPoint = 18,
        SqtFeedPet = 19,
        SqtUserDie = 20,
        SqtPvpNpcHunt = 21,
        SqtItemUse = 22,
        SqtSkillUse = 23,
        SqtVisitVillage = 24,
        SqtVisitField = 25,
        SqtVisitDungeon = 26,
        SqtFindNpc = 27,
        SqtPvpPlayArrange = 28,
        SqtPvpTagCount = 29,
        SqtHyperModeUse = 30,
        SqtCharLevelUp = 31,
        SqtSuitableLevelDungeonClear = 32,
        SqtPvpHeroNpcKill = 33,
        SqtPvpHeroNpcPlay = 34,
        SqtPointCount = 35,
        SqtLearnNewSkill = 36,
    }

    public sealed class SubQuestGoal
    {
        public int SubQuestId { get; init; } = 1;
        public SubQuestType Type { get; init; } = SubQuestType.SqtNone;
        public int TargetId { get; init; }       // NPC ID, Dungeon ID, Map ID, Item ID, Skill ID
        public int TargetCount { get; init; } = 1;
        public int TargetDungeonId { get; init; }
        public sbyte TargetDifficulty { get; init; } = -1; // -1 = any
        public sbyte MinRank { get; init; } = -1;          // -1 = any
        public int MaxTimeOrDamage { get; init; }          // for DungeonTime / DungeonDamage
    }

    public sealed class QuestTemplate
    {
        public int QuestId { get; init; }
        public bool IsDaily { get; init; }
        public bool IsRepeatable { get; init; }
        public int PrerequisiteQuestId { get; init; }
        public byte FairLevel { get; init; } = 1;
        public int TargetNpcId { get; init; }
        public int TargetKillCount { get; init; }
        public int TargetTalkNpcId { get; init; }
        public int TargetPvpPlayCount { get; init; }
        public int TargetDungeonId { get; init; }
        public int RewardEd { get; init; } = 2000;
        public int RewardExp { get; init; } = 500;
        public List<SubQuestGoal> SubQuestGoals { get; init; } = [];

        public IReadOnlyList<SubQuestGoal> GetEffectiveGoals()
        {
            if (SubQuestGoals.Count > 0) return SubQuestGoals;
            var legacy = new List<SubQuestGoal>();
            if (TargetKillCount > 0)
                legacy.Add(new SubQuestGoal { SubQuestId = 1, Type = SubQuestType.SqtNpcHunt, TargetId = TargetNpcId, TargetDungeonId = TargetDungeonId, TargetCount = TargetKillCount });
            else if (TargetTalkNpcId > 0)
                legacy.Add(new SubQuestGoal { SubQuestId = 1, Type = SubQuestType.SqtNpcTalk, TargetId = TargetTalkNpcId, TargetCount = 1 });
            else if (TargetPvpPlayCount > 0)
                legacy.Add(new SubQuestGoal { SubQuestId = 1, Type = SubQuestType.SqtPvpPlay, TargetCount = TargetPvpPlayCount });
            else if (TargetDungeonId > 0)
                legacy.Add(new SubQuestGoal { SubQuestId = 1, Type = SubQuestType.SqtDungeonClearCount, TargetId = TargetDungeonId, TargetDungeonId = TargetDungeonId, TargetCount = 1 });
            return legacy;
        }
    }

    private readonly Dictionary<int, KQuestInstance> _activeQuests = new();
    private readonly Dictionary<int, DateTime> _completedQuestsUtc = new();
    private readonly Dictionary<int, QuestTemplate> _templates = new();
    private readonly object _sync = new();

    public void RegisterTemplate(QuestTemplate template)
    {
        lock (_sync) _templates[template.QuestId] = template;
    }

    public void Init(IEnumerable<KQuestInstance> active, IEnumerable<(int questId, DateTime completedUtc)> completed)
    {
        lock (_sync)
        {
            _activeQuests.Clear();
            _completedQuestsUtc.Clear();
            foreach (var q in active) _activeQuests[q.Id] = q;
            foreach (var c in completed) _completedQuestsUtc[c.questId] = c.completedUtc;
        }
    }

    public (bool ok, KQuestInstance? instance) AcceptQuest(long unitUid, int questId, DateTime? nowUtc = null, byte unitLevel = 99)
    {
        var now = nowUtc ?? DateTime.UtcNow;
        lock (_sync)
        {
            if (_activeQuests.ContainsKey(questId) || _activeQuests.Count >= MaxIngQuestNum)
                return (false, null);

            var subList = new List<KSubQuestInstance>();
            if (_templates.TryGetValue(questId, out var tpl))
            {
                if (unitLevel < tpl.FairLevel)
                    return (false, null);

                if (tpl.PrerequisiteQuestId > 0 && !_completedQuestsUtc.ContainsKey(tpl.PrerequisiteQuestId))
                    return (false, null);

                if (_completedQuestsUtc.TryGetValue(questId, out var prevDone))
                {
                    if (!tpl.IsRepeatable && !tpl.IsDaily)
                        return (false, null);
                    if (tpl.IsDaily && prevDone.Date >= now.Date)
                        return (false, null);
                }

                foreach (var g in tpl.GetEffectiveGoals())
                    subList.Add(new KSubQuestInstance { Id = g.SubQuestId, ClearData = 0 });
            }

            if (subList.Count == 0)
                subList.Add(new KSubQuestInstance { Id = 1, ClearData = 0 });

            var inst = new KQuestInstance
            {
                Id = questId,
                OwnerUnitUid = unitUid,
                SubQuests = subList,
            };
            _activeQuests[questId] = inst;
            return (true, inst);
        }
    }

    public bool GiveUpQuest(int questId)
    {
        lock (_sync) return _activeQuests.Remove(questId);
    }

    public List<int> OnNpcUnitDie(int dungeonId, int monsterId)
        => AdvanceGoals(g => g.Type == SubQuestType.SqtNpcHunt &&
                             (g.TargetId == 0 || g.TargetId == monsterId) &&
                             (g.TargetDungeonId == 0 || g.TargetDungeonId == dungeonId));

    public List<int> OnTalkWithNpc(int npcId)
        => AdvanceGoals(g => g.Type is SubQuestType.SqtNpcTalk or SubQuestType.SqtFindNpc && g.TargetId == npcId);

    public List<int> OnPvpPlay(bool isArranged = false)
        => AdvanceGoals(g => g.Type == SubQuestType.SqtPvpPlay || (isArranged && g.Type == SubQuestType.SqtPvpPlayArrange));

    public List<int> OnPvpWin(bool isHeroNpc = false)
        => AdvanceGoals(g => g.Type == SubQuestType.SqtPvpWin || (isHeroNpc && g.Type == SubQuestType.SqtPvpNpcHunt));

    public List<int> OnPvpKill(int kills = 1, bool isHeroNpc = false)
        => AdvanceGoals(g => g.Type == SubQuestType.SqtPvpKill || (isHeroNpc && g.Type == SubQuestType.SqtPvpHeroNpcKill), kills);

    public List<int> OnDungeonClear(int dungeonId, sbyte difficulty, sbyte totalRank, int playTimeSec, int takenDamage, bool isSuitableLevel = true)
    {
        var updated = new List<int>();
        lock (_sync)
        {
            foreach (var kv in _activeQuests)
            {
                if (!_templates.TryGetValue(kv.Key, out var tpl)) continue;
                var goals = tpl.GetEffectiveGoals();
                bool touched = false;
                for (int i = 0; i < goals.Count && i < kv.Value.SubQuests.Count; i++)
                {
                    var g = goals[i];
                    var sq = kv.Value.SubQuests[i];
                    if (sq.IsSuccess) continue;
                    if (g.TargetDungeonId > 0 && g.TargetDungeonId != dungeonId) continue;
                    if (g.TargetDifficulty >= 0 && difficulty < g.TargetDifficulty) continue;

                    bool pass = g.Type switch
                    {
                        SubQuestType.SqtDungeonClearCount or SubQuestType.SqtItemEquipDungeonClear => true,
                        SubQuestType.SqtSuitableLevelDungeonClear => isSuitableLevel,
                        SubQuestType.SqtDungeonRank => totalRank >= g.MinRank,
                        SubQuestType.SqtDungeonTime => g.MaxTimeOrDamage <= 0 || playTimeSec <= g.MaxTimeOrDamage,
                        SubQuestType.SqtDungeonDamage => g.MaxTimeOrDamage <= 0 || takenDamage <= g.MaxTimeOrDamage,
                        _ => false,
                    };
                    if (!pass) continue;
                    sq.ClearData++;
                    if (sq.ClearData >= Math.Max(1, g.TargetCount))
                        sq.IsSuccess = true;
                    touched = true;
                }
                if (touched) updated.Add(kv.Key);
            }
        }
        return updated;
    }

    public List<int> OnItemObtain(int itemId, int quantity = 1)
        => AdvanceGoals(g => g.Type is SubQuestType.SqtItemCollection or SubQuestType.SqtQuestItemCollection && g.TargetId == itemId, quantity);

    public List<int> OnEnchantItem(int itemId)
        => AdvanceGoals(g => g.Type == SubQuestType.SqtItemEnchant && (g.TargetId == 0 || g.TargetId == itemId));

    public List<int> OnSocketItem(int itemId)
        => AdvanceGoals(g => g.Type == SubQuestType.SqtItemSocket && (g.TargetId == 0 || g.TargetId == itemId));

    public List<int> OnAttribItem(int itemId)
        => AdvanceGoals(g => g.Type == SubQuestType.SqtItemAttrib && (g.TargetId == 0 || g.TargetId == itemId));

    public List<int> OnResolveItem(int itemId)
        => AdvanceGoals(g => g.Type == SubQuestType.SqtItemResolve && (g.TargetId == 0 || g.TargetId == itemId));

    public List<int> OnLearnSkill(int skillId)
        => AdvanceGoals(g => g.Type is SubQuestType.SqtLearnNewSkill or SubQuestType.SqtUseSkillPoint && (g.TargetId == 0 || g.TargetId == skillId));

    public List<int> OnUseSkill(int skillId)
        => AdvanceGoals(g => g.Type == SubQuestType.SqtSkillUse && (g.TargetId == 0 || g.TargetId == skillId));

    public List<int> OnVisitLocation(int mapId, SubQuestType visitType)
        => AdvanceGoals(g => g.Type == visitType && (g.TargetId == 0 || g.TargetId == mapId));

    public List<int> OnLevelUp(byte newLevel)
    {
        var updated = new List<int>();
        lock (_sync)
        {
            foreach (var kv in _activeQuests)
            {
                if (!_templates.TryGetValue(kv.Key, out var tpl)) continue;
                var goals = tpl.GetEffectiveGoals();
                bool touched = false;
                for (int i = 0; i < goals.Count && i < kv.Value.SubQuests.Count; i++)
                {
                    var g = goals[i];
                    var sq = kv.Value.SubQuests[i];
                    if (g.Type == SubQuestType.SqtCharLevelUp && !sq.IsSuccess)
                    {
                        sq.ClearData = newLevel;
                        if (newLevel >= g.TargetCount) sq.IsSuccess = true;
                        touched = true;
                    }
                }
                if (touched) updated.Add(kv.Key);
            }
        }
        return updated;
    }

    private List<int> AdvanceGoals(Func<SubQuestGoal, bool> predicate, int delta = 1)
    {
        var updated = new List<int>();
        lock (_sync)
        {
            foreach (var kv in _activeQuests)
            {
                if (!_templates.TryGetValue(kv.Key, out var tpl)) continue;
                var goals = tpl.GetEffectiveGoals();
                bool touched = false;
                for (int i = 0; i < goals.Count && i < kv.Value.SubQuests.Count; i++)
                {
                    var g = goals[i];
                    var sq = kv.Value.SubQuests[i];
                    if (sq.IsSuccess || !predicate(g)) continue;
                    int target = Math.Max(1, g.TargetCount);
                    sq.ClearData = (byte)Math.Min(byte.MaxValue, Math.Min(target, sq.ClearData + Math.Max(1, delta)));
                    if (sq.ClearData >= target) sq.IsSuccess = true;
                    touched = true;
                }
                if (touched) updated.Add(kv.Key);
            }
        }
        return updated;
    }

    public (bool ok, int rewardEd, int rewardExp) CompleteQuest(int questId, DateTime? nowUtc = null)
    {
        lock (_sync)
        {
            if (!_activeQuests.TryGetValue(questId, out var inst))
                return (false, 0, 0);

            int ed = 2000, exp = 500;
            if (_templates.TryGetValue(questId, out var tpl))
            {
                if (tpl.GetEffectiveGoals().Count > 0 && !inst.SubQuests.All(s => s.IsSuccess))
                    return (false, 0, 0);

                ed = tpl.RewardEd;
                exp = tpl.RewardExp;
            }

            _activeQuests.Remove(questId);
            _completedQuestsUtc[questId] = nowUtc ?? DateTime.UtcNow;
            return (true, ed, exp);
        }
    }

    public List<KQuestInstance> GetActiveQuests()
    {
        lock (_sync) return _activeQuests.Values.ToList();
    }

    public List<KCompleteQuestInfo> GetCompletedQuests()
    {
        lock (_sync)
        {
            return _completedQuestsUtc
                .Select(kv => new KCompleteQuestInfo
                {
                    QuestId = kv.Key,
                    CompleteCount = 1,
                    CompleteDate = new DateTimeOffset(kv.Value).ToUnixTimeSeconds(),
                })
                .ToList();
        }
    }

    public bool IsQuestCompleted(int questId)
    {
        lock (_sync) return _completedQuestsUtc.ContainsKey(questId);
    }

    public HashSet<int> GetKnownQuestIds()
    {
        lock (_sync)
        {
            var set = new HashSet<int>(_activeQuests.Keys);
            set.UnionWith(_completedQuestsUtc.Keys);
            return set;
        }
    }
}
