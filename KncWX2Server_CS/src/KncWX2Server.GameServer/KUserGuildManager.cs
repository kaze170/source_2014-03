using KncWX2Server.Core.Packets;
using KncWX2Server.Core.Region;

namespace KncWX2Server.GameServer;

/// <summary>
/// Faithful C# port of GameServer/UserGuildManager.{h,cpp}:
/// manages character-side guild membership, HONOR_POINT_MAX (1600) -> Guild EXP rollover,
/// Guild Foundation Item purchase checks (ERR_GUILD_37/38/44),
/// and guild passive skill stats/EXP/ED bonus filtering (excluding GUG_NEW_USER).
/// </summary>
public sealed class KUserGuildManager
{
    public const int HonorPointMax = 1600;
    public const int GuildQuestId = 7630;
    public const int GuildFoundationItemId = 130040;

    public const byte GradeInvalid = 0;
    public const byte GradeMaster = 1;
    public const byte GradeSysop = 2;
    public const byte GradeOldUser = 3;
    public const byte GradeVipUser = 4;
    public const byte GradeNormalUser = 5;
    public const byte GradeNewUser = 6;

    private readonly Dictionary<short, KGuildSkillData> _skillTree = new();

    public int GuildUid { get; private set; }
    public string GuildName { get; set; } = string.Empty;
    public byte MemberShipGrade { get; private set; } = GradeInvalid;
    public int HonorPoint { get; private set; }
    public int MaxGuildCsPoint { get; set; }
    public bool IsGuildNameChanging { get; set; }

    public bool IsGuildMaster => MemberShipGrade == GradeMaster;
    public bool IsCashSkillPointExpired => MaxGuildCsPoint == 0;

    public void Clear()
    {
        GuildUid = 0;
        GuildName = string.Empty;
        MemberShipGrade = GradeInvalid;
        HonorPoint = 0;
        _skillTree.Clear();
        MaxGuildCsPoint = 0;
        IsGuildNameChanging = false;
    }

    public void SetGuildInfo(KUserGuildInfo info)
    {
        if (info.GuildUid <= 0) return;
        GuildUid = info.GuildUid;
        GuildName = info.GuildName;
        MemberShipGrade = info.MemberShipGrade;
        HonorPoint = info.HonorPoint;
    }

    public void SetMemberShipGrade(byte grade) => MemberShipGrade = grade;

    public int AddHonorPointAndCheckRollover(int deltaHonor)
    {
        if (GuildUid <= 0 || deltaHonor <= 0) return 0;
        HonorPoint += deltaHonor;
        int gainedGuildExp = 0;
        while (HonorPoint >= HonorPointMax)
        {
            HonorPoint -= HonorPointMax;
            gainedGuildExp++;
        }
        return gainedGuildExp;
    }

    public (bool ok, int netError) CheckBuyGuildFoundation(
        byte authLevel,
        bool isCompleteGuildQuest,
        bool hasFoundationItemInInventoryOrBank)
    {
        if (authLevel >= 5) return (true, NetError.NET_OK);
        if (GuildUid > 0) return (false, NetError.ERR_GUILD_37);
        if (!isCompleteGuildQuest) return (false, NetError.ERR_GUILD_44);
        if (hasFoundationItemInInventoryOrBank) return (false, NetError.ERR_GUILD_38);
        return (true, NetError.NET_OK);
    }

    public void SetGuildSkill(KGuildSkillInfo info)
    {
        _skillTree.Clear();
        foreach (var s in info.GuildSkill)
            _skillTree[s.SkillId] = s;
        MaxGuildCsPoint = info.MaxGuildCsPoint;
    }

    public void UpdateGuildSkill(short skillId, byte skillLevel, byte skillCsPoint)
    {
        if (_skillTree.TryGetValue(skillId, out var existing))
        {
            existing.SkillLevel = skillLevel;
            existing.SkillCsPoint = skillCsPoint;
        }
        else
        {
            _skillTree[skillId] = new KGuildSkillData
            {
                SkillId = skillId,
                SkillLevel = skillLevel,
                SkillCsPoint = skillCsPoint,
            };
        }
    }

    public bool ResetGuildSkill(short skillId)
    {
        if (!_skillTree.TryGetValue(skillId, out var existing))
            return false;
        if (existing.SkillLevel <= 1)
            _skillTree.Remove(skillId);
        else
            existing.SkillLevel--;
        return true;
    }

    public void ExpireGuildSkill(IEnumerable<KGuildSkillData> updatedSkills)
    {
        foreach (var s in updatedSkills)
        {
            if (_skillTree.TryGetValue(s.SkillId, out var existing))
            {
                existing.SkillLevel = s.SkillLevel;
                existing.SkillCsPoint = s.SkillCsPoint;
            }
        }
        MaxGuildCsPoint = 0;
    }

    public bool IsExistGuildSkill(short skillId) => _skillTree.ContainsKey(skillId);

    public List<KGuildSkillData> GetGuildPassiveSkillData()
    {
        if (MemberShipGrade == GradeNewUser || GuildUid <= 0)
            return [];
        return _skillTree.Values.Where(s => s.SkillLevel > 0).ToList();
    }
}
