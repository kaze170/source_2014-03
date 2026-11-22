using System.Collections.Concurrent;
using KncWX2Server.Core.Database;
using KncWX2Server.Core.Packets;
using KncWX2Server.Core.Region;

namespace KncWX2Server.LoginServer;

/// <summary>
/// Faithful C# port of LoginServer/GuildManager.{h,cpp}, Guild.{h,cpp}, and X2Data/XSLGuildManager.cpp:
/// central guild directory, level 1..20 EXP progression table, grade permission checks
/// (Master=1, Admin=2, Senior=3, Normal=4, NewUser=5), kick/disband, EXP/SPoint grants, and guild skill tree.
/// </summary>
public sealed class KLoginGuildManager
{
    public const byte GradeMaster = 1;
    public const byte GradeAdmin = 2;
    public const byte GradeSenior = 3;
    public const byte GradeNormal = 4;
    public const byte GradeNewUser = 5;

    public const int MaxGuildLevel = 20;

    // Cumulative Guild EXP required to reach level 1..20 (XSLGuildManager EXP curve)
    private static readonly int[] GuildLevelExpThresholds =
    [
        0,       // Lv 1
        500,     // Lv 2
        1_500,   // Lv 3
        3_500,   // Lv 4
        7_000,   // Lv 5
        12_000,  // Lv 6
        19_000,  // Lv 7
        28_000,  // Lv 8
        40_000,  // Lv 9
        55_000,  // Lv 10
        75_000,  // Lv 11
        100_000, // Lv 12
        130_000, // Lv 13
        165_000, // Lv 14
        205_000, // Lv 15
        250_000, // Lv 16
        305_000, // Lv 17
        370_000, // Lv 18
        445_000, // Lv 19
        530_000, // Lv 20
    ];

    public sealed class GuildEntry
    {
        public KGuildInfo Info { get; set; } = new();
        public ConcurrentDictionary<long, byte> MemberGrades { get; } = new(); // UnitUid -> Grade (1..5)
        public ConcurrentDictionary<long, string> MemberMessages { get; } = new();
        public ConcurrentDictionary<long, KGuildMemberInfo> MemberInfos { get; } = new();
        public int GuildSkillPoints { get; set; }
        public KGuildSkillTree SkillTree { get; } = new();
    }

    private readonly ConcurrentDictionary<int, GuildEntry> _guilds = new();

    public int GuildCount => _guilds.Count;

    public static byte CalculateGuildLevel(int totalGuildExp)
    {
        int exp = Math.Max(0, totalGuildExp);
        byte lvl = 1;
        for (int i = 1; i < GuildLevelExpThresholds.Length; i++)
        {
            if (exp >= GuildLevelExpThresholds[i])
                lvl = (byte)(i + 1);
            else
                break;
        }
        return lvl;
    }

    public static bool CanInvite(byte actorGrade) => actorGrade <= GradeSenior;
    public static bool CanChangeMessage(byte actorGrade) => actorGrade <= GradeAdmin;
    public static bool CanLearnSkill(byte actorGrade) => actorGrade == GradeMaster;
    public static bool CanKickMember(byte actorGrade, byte targetGrade) => actorGrade <= GradeAdmin && actorGrade < targetGrade;
    public static bool CanChangeMemberGrade(byte actorGrade, byte targetGrade, byte newGrade)
        => actorGrade == GradeMaster && targetGrade != GradeMaster && newGrade >= GradeAdmin && newGrade <= GradeNewUser;

    public (bool ok, KGuildInfo? guild, int netError) CreateGuild(
        SqliteDatabaseManager db, RegionProfile region, long masterUnitUid, string guildName, string guildMessage)
    {
        var res = db.CreateGuild(region, masterUnitUid, guildName, guildMessage);
        if (!res.ok || res.guild == null) return res;

        var entry = new GuildEntry { Info = res.guild };
        entry.MemberGrades[masterUnitUid] = GradeMaster;
        _guilds[res.guild.GuildUid] = entry;
        return res;
    }

    public (bool ok, KGuildInfo? guild, int netError) JoinGuild(
        SqliteDatabaseManager db, RegionProfile region, int guildUid, long unitUid)
    {
        if (_guilds.TryGetValue(guildUid, out var existing) &&
            existing.MemberGrades.Count >= Math.Max(10, (int)existing.Info.MaxNumMember))
        {
            return (false, null, NetError.ERR_GUILD_05);
        }

        var res = db.JoinGuild(region, guildUid, unitUid);
        if (!res.ok || res.guild == null) return res;

        var entry = _guilds.GetOrAdd(guildUid, _ => new GuildEntry { Info = res.guild });
        entry.MemberGrades[unitUid] = GradeNewUser;
        return res;
    }

    public bool QuitGuild(SqliteDatabaseManager db, int guildUid, long unitUid)
    {
        bool ok = db.QuitGuild(unitUid);
        if (ok && _guilds.TryGetValue(guildUid, out var entry))
        {
            entry.MemberGrades.TryRemove(unitUid, out _);
            entry.MemberInfos.TryRemove(unitUid, out _);
            entry.MemberMessages.TryRemove(unitUid, out _);
        }
        return ok;
    }

    public (bool ok, int netError) KickMember(
        SqliteDatabaseManager db, int guildUid, long requesterUnitUid, long targetUnitUid)
    {
        if (!_guilds.TryGetValue(guildUid, out var entry))
            return (false, NetError.ERR_GUILD_01);

        byte actorGrade = entry.MemberGrades.GetValueOrDefault(requesterUnitUid, GradeNewUser);
        byte targetGrade = entry.MemberGrades.GetValueOrDefault(targetUnitUid, GradeNewUser);
        if (!CanKickMember(actorGrade, targetGrade))
            return (false, NetError.ERR_GUILD_04);

        bool ok = db.QuitGuild(targetUnitUid);
        if (ok)
        {
            entry.MemberGrades.TryRemove(targetUnitUid, out _);
            entry.MemberInfos.TryRemove(targetUnitUid, out _);
            entry.MemberMessages.TryRemove(targetUnitUid, out _);
        }
        return (ok, ok ? NetError.NET_OK : NetError.ERR_GUILD_01);
    }

    public (bool ok, int netError) DisbandGuild(SqliteDatabaseManager db, int guildUid, long requesterUnitUid)
    {
        if (!_guilds.TryGetValue(guildUid, out var entry))
            return (false, NetError.ERR_GUILD_01);

        if (entry.MemberGrades.GetValueOrDefault(requesterUnitUid, GradeNewUser) != GradeMaster)
            return (false, NetError.ERR_GUILD_04);

        foreach (long memberUid in entry.MemberGrades.Keys.ToList())
            db.QuitGuild(memberUid);

        _guilds.TryRemove(guildUid, out _);
        return (true, NetError.NET_OK);
    }

    public (bool ok, byte newLevel, int grantedSPoint) AddGuildExp(
        SqliteDatabaseManager db, RegionProfile region, int guildUid, int deltaExp)
    {
        if (deltaExp <= 0) return (false, 1, 0);
        var info = GetGuildInfo(db, region, guildUid);
        if (info == null) return (false, 1, 0);

        byte oldLevel = info.GuildLevel;
        info.GuildExp += deltaExp;
        byte newLevel = CalculateGuildLevel(info.GuildExp);
        int gainedSp = Math.Max(0, newLevel - oldLevel);
        if (gainedSp > 0)
        {
            info.GuildLevel = newLevel;
            info.MaxNumMember = (ushort)Math.Max(info.MaxNumMember, 30 + (newLevel - 1) * 5);
            if (_guilds.TryGetValue(guildUid, out var entry))
            {
                entry.GuildSkillPoints += gainedSp;
                entry.SkillTree.GuildSPoint += gainedSp;
            }
        }
        return (true, info.GuildLevel, gainedSp);
    }

    public (bool ok, string targetNick) ChangeMemberGrade(
        SqliteDatabaseManager db, int guildUid, long targetUnitUid, byte newGrade)
    {
        var res = db.ChangeGuildMemberGrade(guildUid, targetUnitUid, newGrade);
        if (res.ok && _guilds.TryGetValue(guildUid, out var entry))
            entry.MemberGrades[targetUnitUid] = newGrade;
        return res;
    }

    public KGuildInfo? GetGuildInfo(SqliteDatabaseManager db, RegionProfile region, int guildUid)
    {
        if (_guilds.TryGetValue(guildUid, out var entry))
            return entry.Info;
        var loaded = db.GetGuildInfo(region, guildUid);
        if (loaded != null)
            _guilds[guildUid] = new GuildEntry { Info = loaded };
        return loaded;
    }

    public GuildEntry? GetGuildEntry(int guildUid)
        => _guilds.TryGetValue(guildUid, out var entry) ? entry : null;

    public bool ResetGuildSkill(int guildUid, int skillId, int skillLevel, int skillCsPoint, int guildSPoint, int guildCsPoint)
    {
        if (!_guilds.TryGetValue(guildUid, out var entry)) return false;
        entry.GuildSkillPoints = guildSPoint;
        entry.SkillTree.GuildSPoint = guildSPoint;
        entry.SkillTree.GuildCsPoint = guildCsPoint;
        if (skillId > 0)
            entry.SkillTree.SetSkillLevelAndCsp(skillId, skillLevel, skillCsPoint);
        return true;
    }

    public bool InsertGuildCashSkillPoint(int guildUid, int csPoint, int periodDays)
    {
        if (!_guilds.TryGetValue(guildUid, out var entry)) return false;
        entry.SkillTree.GuildCsPoint += Math.Max(0, csPoint);
        entry.SkillTree.MaxGuildCsPoint = Math.Max(entry.SkillTree.MaxGuildCsPoint, entry.SkillTree.GuildCsPoint);
        if (periodDays > 0)
            entry.SkillTree.SetGuildCsPointEndDate(DateTime.UtcNow.AddDays(periodDays).ToString("yyyy-MM-dd HH:mm:ss"));
        return true;
    }

    public bool UpdateMemberMessage(int guildUid, long targetUnitUid, string message)
    {
        if (!_guilds.TryGetValue(guildUid, out var entry)) return false;
        entry.MemberMessages[targetUnitUid] = message;
        if (entry.MemberInfos.TryGetValue(targetUnitUid, out var m))
            m.Message = message;
        return true;
    }

    public bool UpdateMemberInfo(int guildUid, KGuildMemberInfo member)
    {
        if (!_guilds.TryGetValue(guildUid, out var entry)) return false;
        entry.MemberGrades[member.UnitUid] = member.MemberShipGrade;
        entry.MemberInfos[member.UnitUid] = member;
        return true;
    }

    public int CheckExpiredCashSkillPoints(DateTime? nowUtc = null)
    {
        int expiredGuilds = 0;
        foreach (var entry in _guilds.Values)
        {
            if (entry.SkillTree.GuildCsPoint > 0 && entry.SkillTree.IsCashSkillPointExpired(nowUtc))
            {
                entry.SkillTree.CalcAndExpireCashSkillPoint();
                expiredGuilds++;
            }
        }
        return expiredGuilds;
    }
}
