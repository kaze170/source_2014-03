namespace KncWX2Server.LoginServer;

/// <summary>
/// Faithful C# port of LoginServer/GuildSkillTree.{h,cpp} (SERV_UPGRADE_SKILL_SYSTEM_2013):
/// guild skill map, master-level clamping, tier SP calculation, and cash skill point expiry.
/// </summary>
public sealed class KGuildSkillTree
{
    public sealed class GuildSkill
    {
        public int SkillLevel { get; set; }
        public int SkillCsPoint { get; set; }
    }

    public sealed class GuildSkillTemplate
    {
        public int SkillId { get; init; }
        public int Tier { get; init; } // 0..N
        public int MasterLevel { get; init; } = 5;
        public List<int> PrecedingSkills { get; init; } = [];
        public List<int> FollowingSkills { get; init; } = [];
    }

    private readonly Dictionary<int, GuildSkill> _skills = new();
    private readonly Dictionary<int, GuildSkillTemplate> _templates;
    private readonly object _sync = new();

    public int GuildSPoint { get; set; }
    public int GuildCsPoint { get; set; }
    public int MaxGuildCsPoint { get; set; }
    public DateTime? GuildCsPointEndDateUtc { get; private set; }

    public KGuildSkillTree(IReadOnlyDictionary<int, GuildSkillTemplate>? templates = null)
    {
        _templates = templates != null
            ? new Dictionary<int, GuildSkillTemplate>(templates)
            : BuildDefaultTemplates();
    }

    public void RegisterTemplate(GuildSkillTemplate tpl)
    {
        lock (_sync) _templates[tpl.SkillId] = tpl;
    }

    public void ResetGuildSkillTree()
    {
        lock (_sync)
        {
            _skills.Clear();
            GuildSPoint = 0;
            GuildCsPoint = 0;
            MaxGuildCsPoint = 0;
            GuildCsPointEndDateUtc = null;
        }
    }

    public bool SetGuildCsPointEndDate(string endDate)
    {
        if (DateTime.TryParse(endDate, out var dt))
        {
            GuildCsPointEndDateUtc = dt.ToUniversalTime();
            return true;
        }
        return false;
    }

    public bool SetSkillLevelAndCsp(int skillId, int skillLevel, int skillCsPoint)
    {
        lock (_sync)
        {
            int maxLvl = _templates.TryGetValue(skillId, out var tpl) ? tpl.MasterLevel : 5;
            int clamped = Math.Clamp(skillLevel, 0, maxLvl);
            _skills[skillId] = new GuildSkill
            {
                SkillLevel = clamped,
                SkillCsPoint = Math.Max(0, skillCsPoint),
            };
            return true;
        }
    }

    public int GetSkillLevel(int skillId)
    {
        lock (_sync)
            return _skills.TryGetValue(skillId, out var s) ? s.SkillLevel : 0;
    }

    public bool GetSkillLevelAndCsp(int skillId, out int skillLevel, out int skillCsPoint)
    {
        lock (_sync)
        {
            if (_skills.TryGetValue(skillId, out var s))
            {
                skillLevel = s.SkillLevel;
                skillCsPoint = s.SkillCsPoint;
                return true;
            }
            skillLevel = -1;
            skillCsPoint = 0;
            return false;
        }
    }

    public bool IsMasterSkillLevel(int skillId)
    {
        lock (_sync)
        {
            int maxLvl = _templates.TryGetValue(skillId, out var tpl) ? tpl.MasterLevel : 5;
            return _skills.TryGetValue(skillId, out var s) && s.SkillLevel >= maxLvl;
        }
    }

    public bool IsAllPrecedingSkillMasterLevel(int skillId)
    {
        lock (_sync)
        {
            if (!_templates.TryGetValue(skillId, out var tpl)) return true;
            foreach (int pre in tpl.PrecedingSkills)
            {
                if (!IsMasterSkillLevel(pre)) return false;
            }
            return true;
        }
    }

    public bool IsAllFollowingSkillLevelZero(int skillId)
    {
        lock (_sync)
        {
            if (!_templates.TryGetValue(skillId, out var tpl)) return true;
            foreach (int fol in tpl.FollowingSkills)
            {
                if (GetSkillLevel(fol) > 0) return false;
            }
            return true;
        }
    }

    public bool IsTierOpened(int skillId)
    {
        lock (_sync)
        {
            if (!_templates.TryGetValue(skillId, out var tpl)) return true;
            if (tpl.Tier <= 0) return true;
            var tierPoints = CalcCumulativeUsedSPointOnEachTier();
            int prevIdx = tpl.Tier - 1;
            return prevIdx < tierPoints.Count && tierPoints[prevIdx] >= tpl.Tier * 5;
        }
    }

    public List<int> CalcCumulativeUsedSPointOnEachTier()
    {
        lock (_sync)
        {
            int maxTier = _templates.Values.Count > 0 ? _templates.Values.Max(t => t.Tier) : 0;
            var perTier = new int[maxTier + 1];
            foreach (var kv in _skills)
            {
                int tier = _templates.TryGetValue(kv.Key, out var tpl) ? tpl.Tier : 0;
                perTier[tier] += kv.Value.SkillLevel;
            }
            var cumulative = new List<int>(maxTier + 1);
            int acc = 0;
            for (int i = 0; i <= maxTier; i++)
            {
                acc += perTier[i];
                cumulative.Add(acc);
            }
            return cumulative;
        }
    }

    public bool IsCashSkillPointExpired(DateTime? nowUtc = null)
        => GuildCsPointEndDateUtc.HasValue && (nowUtc ?? DateTime.UtcNow) >= GuildCsPointEndDateUtc.Value;

    public (int newGuildSPoint, int retrievedSPoint, List<(int skillId, int newLevel, int newCsp)> modified)
        CalcAndExpireCashSkillPoint()
    {
        lock (_sync)
        {
            int retrieved = 0;
            var modified = new List<(int, int, int)>();
            foreach (var kv in _skills.ToList())
            {
                if (kv.Value.SkillCsPoint <= 0) continue;
                int drop = Math.Min(kv.Value.SkillLevel, kv.Value.SkillCsPoint);
                kv.Value.SkillLevel -= drop;
                kv.Value.SkillCsPoint = 0;
                retrieved += drop;
                modified.Add((kv.Key, kv.Value.SkillLevel, 0));
            }
            GuildCsPoint = 0;
            MaxGuildCsPoint = 0;
            return (GuildSPoint, retrieved, modified);
        }
    }

    public (int spUsed, int cspUsed) CalcUsedSPointAndCsPoint()
    {
        lock (_sync)
        {
            int total = _skills.Values.Sum(s => s.SkillLevel);
            int csp = _skills.Values.Sum(s => s.SkillCsPoint);
            return (Math.Max(0, total - csp), csp);
        }
    }

    private static Dictionary<int, GuildSkillTemplate> BuildDefaultTemplates()
        => new()
        {
            [9001] = new() { SkillId = 9001, Tier = 0, MasterLevel = 5, FollowingSkills = [9002] },
            [9002] = new() { SkillId = 9002, Tier = 1, MasterLevel = 5, PrecedingSkills = [9001] },
            [9003] = new() { SkillId = 9003, Tier = 0, MasterLevel = 5 },
        };
}
