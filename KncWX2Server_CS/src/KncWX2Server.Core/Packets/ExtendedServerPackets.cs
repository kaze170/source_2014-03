using KncWX2Server.Core.Region;
using KncWX2Server.Core.Serialization;

namespace KncWX2Server.Core.Packets;

public sealed class KDBE_2013_EVENT_MISSION_COMPLETE_ACK : IKSerializable
{
    public int Ok { get; set; }
    public byte Level { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(Level)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out byte v1)) return false; Level = v1;
        return true;
    }
}

public sealed class KDBE_2013_EVENT_MISSION_COMPLETE_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public byte Level { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(Level)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out byte v1)) return false; Level = v1;
        return true;
    }
}

public sealed class KDBE_2014_EVENT_MISSION_COMPLETE_REQ : IKSerializable
{
    public long UserUid { get; set; }
    public long UnitUid { get; set; }
    public int NewYearMissionStepId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UserUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(NewYearMissionStepId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UserUid = v0;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        if (!ks.Get(out int v2)) return false; NewYearMissionStepId = v2;
        return true;
    }
}

public sealed class KDBE_ABUSER_LIST_ACK : IKSerializable
{
    public Dictionary<int, KAbuserList> AbuserList { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutMap(AbuserList, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetMap(AbuserList, static s => (s.Get(out int v), v), static s => { var v = new KAbuserList(); return (s.Get(v), v); })) return false;
        return true;
    }
}

public sealed class KDBE_ABUSER_LIST_REQ : IKSerializable
{
    public Dictionary<int, int> ReleaseTick { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutMap(ReleaseTick, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetMap(ReleaseTick, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false;
        return true;
    }
}

public sealed class KDBE_ACCEPT_FRIEND_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public long FriendUnitUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(FriendUnitUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out long v1)) return false; FriendUnitUid = v1;
        return true;
    }
}

public sealed class KDBE_ACCEPT_JOIN_GUILD_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int GuildUid { get; set; }
    public KGuildMemberInfo JoinGuildMemberInfo { get; set; } = new();
    public string GuildName { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(GuildUid)) return false;
        if (!ks.Put(JoinGuildMemberInfo)) return false;
        if (!ks.PutWideString(GuildName)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out int v1)) return false; GuildUid = v1;
        if (!ks.Get(JoinGuildMemberInfo)) return false;
        if (!ks.GetWideString(out string s3)) return false; GuildName = s3;
        return true;
    }
}

public sealed class KDBE_ACCEPT_JOIN_GUILD_REQ : IKSerializable
{
    public int GuildUid { get; set; }
    public KGuildMemberInfo JoinGuildMemberInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(GuildUid)) return false;
        if (!ks.Put(JoinGuildMemberInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; GuildUid = v0;
        if (!ks.Get(JoinGuildMemberInfo)) return false;
        return true;
    }
}

public sealed class KDBE_ACCOUNT_QUEST_COMPLETE_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KDBE_QUEST_COMPLETE_REQ QuestReq { get; set; } = new();
    public long CompleteTime { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(QuestReq)) return false;
        if (!ks.Put(CompleteTime)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(QuestReq)) return false;
        if (!ks.Get(out long v2)) return false; CompleteTime = v2;
        return true;
    }
}

public sealed class KDBE_ACCOUNT_QUEST_UPDATE_NOT : IKSerializable
{
    public long UserUid { get; set; }
    public List<KQuestUpdate> AccountQuesting { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UserUid)) return false;
        if (!ks.PutList(AccountQuesting)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UserUid = v0;
        if (!ks.GetList(AccountQuesting)) return false;
        return true;
    }
}

public sealed class KDBE_ADMIN_AUTO_GET_ALL_SKILL_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long UnitUid { get; set; }
    public int ActiveSkillPageNumber { get; set; }
    public sbyte UnitClass { get; set; }
    public int SPoint { get; set; }
    public int CSPoint { get; set; }
    public Dictionary<int, int> GetSkillList { get; set; } = new();
    public List<short> UnsealedSkillId { get; set; } = new();
    public int NewDefaultSkill1 { get; set; }
    public int NewDefaultSkill2 { get; set; }
    public int NewDefaultSkill3 { get; set; }
    public int NewDefaultSkill4 { get; set; }
    public int NewDefaultSkill5 { get; set; }
    public int NewDefaultSkill6 { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(ActiveSkillPageNumber)) return false;
        if (!ks.Put(UnitClass)) return false;
        if (!ks.Put(SPoint)) return false;
        if (!ks.Put(CSPoint)) return false;
        if (!ks.PutMap(GetSkillList, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.PutList(UnsealedSkillId, static (s, v) => s.Put(v))) return false;
        if (!ks.Put(NewDefaultSkill1)) return false;
        if (!ks.Put(NewDefaultSkill2)) return false;
        if (!ks.Put(NewDefaultSkill3)) return false;
        if (!ks.Put(NewDefaultSkill4)) return false;
        if (!ks.Put(NewDefaultSkill5)) return false;
        if (!ks.Put(NewDefaultSkill6)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        if (!ks.Get(out int v2)) return false; ActiveSkillPageNumber = v2;
        if (!ks.Get(out sbyte v3)) return false; UnitClass = v3;
        if (!ks.Get(out int v4)) return false; SPoint = v4;
        if (!ks.Get(out int v5)) return false; CSPoint = v5;
        if (!ks.GetMap(GetSkillList, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false;
        if (!ks.GetList(UnsealedSkillId, static s => (s.Get(out short v), v))) return false;
        if (!ks.Get(out int v8)) return false; NewDefaultSkill1 = v8;
        if (!ks.Get(out int v9)) return false; NewDefaultSkill2 = v9;
        if (!ks.Get(out int v10)) return false; NewDefaultSkill3 = v10;
        if (!ks.Get(out int v11)) return false; NewDefaultSkill4 = v11;
        if (!ks.Get(out int v12)) return false; NewDefaultSkill5 = v12;
        if (!ks.Get(out int v13)) return false; NewDefaultSkill6 = v13;
        return true;
    }
}

public sealed class KDBE_ADMIN_AUTO_GET_ALL_SKILL_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public int ActiveSkillPageNumber { get; set; }
    public sbyte UnitClass { get; set; }
    public int SPoint { get; set; }
    public int CSPoint { get; set; }
    public Dictionary<int, int> GetSkillList { get; set; } = new();
    public List<short> UnsealedSkillId { get; set; } = new();
    public int NewDefaultSkill1 { get; set; }
    public int NewDefaultSkill2 { get; set; }
    public int NewDefaultSkill3 { get; set; }
    public int NewDefaultSkill4 { get; set; }
    public int NewDefaultSkill5 { get; set; }
    public int NewDefaultSkill6 { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(ActiveSkillPageNumber)) return false;
        if (!ks.Put(UnitClass)) return false;
        if (!ks.Put(SPoint)) return false;
        if (!ks.Put(CSPoint)) return false;
        if (!ks.PutMap(GetSkillList, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.PutList(UnsealedSkillId, static (s, v) => s.Put(v))) return false;
        if (!ks.Put(NewDefaultSkill1)) return false;
        if (!ks.Put(NewDefaultSkill2)) return false;
        if (!ks.Put(NewDefaultSkill3)) return false;
        if (!ks.Put(NewDefaultSkill4)) return false;
        if (!ks.Put(NewDefaultSkill5)) return false;
        if (!ks.Put(NewDefaultSkill6)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out int v1)) return false; ActiveSkillPageNumber = v1;
        if (!ks.Get(out sbyte v2)) return false; UnitClass = v2;
        if (!ks.Get(out int v3)) return false; SPoint = v3;
        if (!ks.Get(out int v4)) return false; CSPoint = v4;
        if (!ks.GetMap(GetSkillList, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false;
        if (!ks.GetList(UnsealedSkillId, static s => (s.Get(out short v), v))) return false;
        if (!ks.Get(out int v7)) return false; NewDefaultSkill1 = v7;
        if (!ks.Get(out int v8)) return false; NewDefaultSkill2 = v8;
        if (!ks.Get(out int v9)) return false; NewDefaultSkill3 = v9;
        if (!ks.Get(out int v10)) return false; NewDefaultSkill4 = v10;
        if (!ks.Get(out int v11)) return false; NewDefaultSkill5 = v11;
        if (!ks.Get(out int v12)) return false; NewDefaultSkill6 = v12;
        return true;
    }
}

public sealed class KDBE_ADMIN_CASH_SKILL_POINT_DATE_CHANGE_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public string ModifyDate { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutWideString(ModifyDate)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.GetWideString(out string s1)) return false; ModifyDate = s1;
        return true;
    }
}

public sealed class KDBE_ADMIN_CHANGE_COUPLE_DATE_REQ : IKSerializable
{
    public float AddRate { get; set; }
    public long UnitUid { get; set; }
    public long LoverUnitUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(AddRate)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(LoverUnitUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out float v0)) return false; AddRate = v0;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        if (!ks.Get(out long v2)) return false; LoverUnitUid = v2;
        return true;
    }
}

public sealed class KDBE_ADMIN_CHANGE_UNIT_CLASS_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long UnitUid { get; set; }
    public sbyte UnitClass { get; set; }
    public int NewDefaultSkill1 { get; set; }
    public int NewDefaultSkill2 { get; set; }
    public int NewDefaultSkill3 { get; set; }
    public int NewDefaultSkill4 { get; set; }
    public int NewDefaultSkill5 { get; set; }
    public int NewDefaultSkill6 { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(UnitClass)) return false;
        if (!ks.Put(NewDefaultSkill1)) return false;
        if (!ks.Put(NewDefaultSkill2)) return false;
        if (!ks.Put(NewDefaultSkill3)) return false;
        if (!ks.Put(NewDefaultSkill4)) return false;
        if (!ks.Put(NewDefaultSkill5)) return false;
        if (!ks.Put(NewDefaultSkill6)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        if (!ks.Get(out sbyte v2)) return false; UnitClass = v2;
        if (!ks.Get(out int v3)) return false; NewDefaultSkill1 = v3;
        if (!ks.Get(out int v4)) return false; NewDefaultSkill2 = v4;
        if (!ks.Get(out int v5)) return false; NewDefaultSkill3 = v5;
        if (!ks.Get(out int v6)) return false; NewDefaultSkill4 = v6;
        if (!ks.Get(out int v7)) return false; NewDefaultSkill5 = v7;
        if (!ks.Get(out int v8)) return false; NewDefaultSkill6 = v8;
        return true;
    }
}

public sealed class KDBE_ADMIN_CHANGE_UNIT_CLASS_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public sbyte UnitClass { get; set; }
    public int NewDefaultSkill1 { get; set; }
    public int NewDefaultSkill2 { get; set; }
    public int NewDefaultSkill3 { get; set; }
    public int NewDefaultSkill4 { get; set; }
    public int NewDefaultSkill5 { get; set; }
    public int NewDefaultSkill6 { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(UnitClass)) return false;
        if (!ks.Put(NewDefaultSkill1)) return false;
        if (!ks.Put(NewDefaultSkill2)) return false;
        if (!ks.Put(NewDefaultSkill3)) return false;
        if (!ks.Put(NewDefaultSkill4)) return false;
        if (!ks.Put(NewDefaultSkill5)) return false;
        if (!ks.Put(NewDefaultSkill6)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out sbyte v1)) return false; UnitClass = v1;
        if (!ks.Get(out int v2)) return false; NewDefaultSkill1 = v2;
        if (!ks.Get(out int v3)) return false; NewDefaultSkill2 = v3;
        if (!ks.Get(out int v4)) return false; NewDefaultSkill3 = v4;
        if (!ks.Get(out int v5)) return false; NewDefaultSkill4 = v5;
        if (!ks.Get(out int v6)) return false; NewDefaultSkill5 = v6;
        if (!ks.Get(out int v7)) return false; NewDefaultSkill6 = v7;
        return true;
    }
}

public sealed class KDBE_ADMIN_CHEAT_GET_ALL_SKILL_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public int ActiveSkillPageNumber { get; set; }
    public List<short> UnSealedSkill { get; set; } = new();
    public Dictionary<int, KAdminCheatSkill> SkillInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(ActiveSkillPageNumber)) return false;
        if (!ks.PutList(UnSealedSkill, static (s, v) => s.Put(v))) return false;
        if (!ks.PutMap(SkillInfo, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out int v1)) return false; ActiveSkillPageNumber = v1;
        if (!ks.GetList(UnSealedSkill, static s => (s.Get(out short v), v))) return false;
        if (!ks.GetMap(SkillInfo, static s => (s.Get(out int v), v), static s => { var v = new KAdminCheatSkill(); return (s.Get(v), v); })) return false;
        return true;
    }
}

public sealed class KDBE_ADMIN_GET_ITEM_SET_NOT : IKSerializable
{
    public string NickName { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutWideString(NickName)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string s0)) return false; NickName = s0;
        return true;
    }
}

public sealed class KDBE_ADMIN_GET_SKILL_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int ActiveSkillPageNumber { get; set; }
    public int SkillId { get; set; }
    public int SkillLevel { get; set; }
    public int CSPoint { get; set; }
    public bool Unsealed { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(ActiveSkillPageNumber)) return false;
        if (!ks.Put(SkillId)) return false;
        if (!ks.Put(SkillLevel)) return false;
        if (!ks.Put(CSPoint)) return false;
        if (!ks.Put(Unsealed)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out int v1)) return false; ActiveSkillPageNumber = v1;
        if (!ks.Get(out int v2)) return false; SkillId = v2;
        if (!ks.Get(out int v3)) return false; SkillLevel = v3;
        if (!ks.Get(out int v4)) return false; CSPoint = v4;
        if (!ks.Get(out bool v5)) return false; Unsealed = v5;
        return true;
    }
}

public sealed class KDBE_ADMIN_GET_SKILL_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public int ActiveSkillPageNumber { get; set; }
    public int SkillId { get; set; }
    public int SkillLevel { get; set; }
    public int CSPoint { get; set; }
    public bool Unsealed { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(ActiveSkillPageNumber)) return false;
        if (!ks.Put(SkillId)) return false;
        if (!ks.Put(SkillLevel)) return false;
        if (!ks.Put(CSPoint)) return false;
        if (!ks.Put(Unsealed)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out int v1)) return false; ActiveSkillPageNumber = v1;
        if (!ks.Get(out int v2)) return false; SkillId = v2;
        if (!ks.Get(out int v3)) return false; SkillLevel = v3;
        if (!ks.Get(out int v4)) return false; CSPoint = v4;
        if (!ks.Get(out bool v5)) return false; Unsealed = v5;
        return true;
    }
}

public sealed class KDBE_APPLY_JOIN_GUILD_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KApplyJoinGuildInfo ApplyJoinGuildInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(ApplyJoinGuildInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(ApplyJoinGuildInfo)) return false;
        return true;
    }
}

public sealed class KDBE_APPLY_JOIN_GUILD_REQ : IKSerializable
{
    public KApplyJoinGuildInfo ApplyJoinGuildInfo { get; set; } = new();
    public bool OldApplyDelete { get; set; }
    public List<KApplyDeleteInfo> DeleteApply { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ApplyJoinGuildInfo)) return false;
        if (!ks.Put(OldApplyDelete)) return false;
        if (!ks.PutList(DeleteApply)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(ApplyJoinGuildInfo)) return false;
        if (!ks.Get(out bool v1)) return false; OldApplyDelete = v1;
        if (!ks.GetList(DeleteApply)) return false;
        return true;
    }
}

public sealed class KDBE_ATTRIB_ATTACH_ITEM_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long ItemUid { get; set; }
    public KItemAttributeEnchantInfo AttribEnchantInfo { get; set; } = new();
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(ItemUid)) return false;
        if (!ks.Put(AttribEnchantInfo)) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out long v1)) return false; ItemUid = v1;
        if (!ks.Get(AttribEnchantInfo)) return false;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        return true;
    }
}

public sealed class KDBE_ATTRIB_ATTACH_ITEM_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public long ItemUid { get; set; }
    public KItemAttributeEnchantInfo AttribEnchantInfo { get; set; } = new();
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(ItemUid)) return false;
        if (!ks.Put(AttribEnchantInfo)) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out long v1)) return false; ItemUid = v1;
        if (!ks.Get(AttribEnchantInfo)) return false;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        return true;
    }
}

public sealed class KDBE_ATTRIB_ENCHANT_ITEM_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long ItemUid { get; set; }
    public int ItemId { get; set; }
    public sbyte AttribEnchantSlotNo { get; set; }
    public sbyte AttribEnchantId { get; set; }
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(ItemUid)) return false;
        if (!ks.Put(ItemId)) return false;
        if (!ks.Put(AttribEnchantSlotNo)) return false;
        if (!ks.Put(AttribEnchantId)) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out long v1)) return false; ItemUid = v1;
        if (!ks.Get(out int v2)) return false; ItemId = v2;
        if (!ks.Get(out sbyte v3)) return false; AttribEnchantSlotNo = v3;
        if (!ks.Get(out sbyte v4)) return false; AttribEnchantId = v4;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        return true;
    }
}

public sealed class KDBE_ATTRIB_ENCHANT_ITEM_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public long ItemUid { get; set; }
    public int ItemId { get; set; }
    public sbyte AttribEnchantSlotNo { get; set; }
    public sbyte AttribEnchantId { get; set; }
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(ItemUid)) return false;
        if (!ks.Put(ItemId)) return false;
        if (!ks.Put(AttribEnchantSlotNo)) return false;
        if (!ks.Put(AttribEnchantId)) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out long v1)) return false; ItemUid = v1;
        if (!ks.Get(out int v2)) return false; ItemId = v2;
        if (!ks.Get(out sbyte v3)) return false; AttribEnchantSlotNo = v3;
        if (!ks.Get(out sbyte v4)) return false; AttribEnchantId = v4;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        return true;
    }
}

public sealed class KDBE_BLOCK_FRIEND_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public long FriendUnitUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(FriendUnitUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out long v1)) return false; FriendUnitUid = v1;
        return true;
    }
}

public sealed class KDBE_BOSS_FIELD_LOG_NOT : IKSerializable
{
    public KBossFieldLog FieldLog { get; set; } = new();
    public Dictionary<long, List<KBossFieldUserLog>> UserLog { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(FieldLog)) return false;
        if (!ks.PutMap(UserLog, static (s, k) => s.Put(k), static (s, v) => s.PutList(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(FieldLog)) return false;
        if (!ks.GetMap(UserLog, static s => (s.Get(out long v), v), static s => { var l = new List<KBossFieldUserLog>(); return (s.GetList(l), l); })) return false;
        return true;
    }
}

public sealed class KDBE_BREAK_UP_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long MyUnitUid { get; set; }
    public long LoverUnitUid { get; set; }
    public int Cost { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(MyUnitUid)) return false;
        if (!ks.Put(LoverUnitUid)) return false;
        if (!ks.Put(Cost)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out long v1)) return false; MyUnitUid = v1;
        if (!ks.Get(out long v2)) return false; LoverUnitUid = v2;
        if (!ks.Get(out int v3)) return false; Cost = v3;
        return true;
    }
}

public sealed class KDBE_BREAK_UP_REQ : IKSerializable
{
    public long MyUnitUid { get; set; }
    public long LoverUnitUid { get; set; }
    public int Cost { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(MyUnitUid)) return false;
        if (!ks.Put(LoverUnitUid)) return false;
        if (!ks.Put(Cost)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; MyUnitUid = v0;
        if (!ks.Get(out long v1)) return false; LoverUnitUid = v1;
        if (!ks.Get(out int v2)) return false; Cost = v2;
        return true;
    }
}

public sealed class KDBE_BUY_AP_ITEM_LOG_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public int ItemId { get; set; }
    public int Quantity { get; set; }
    public int BeforAP { get; set; }
    public int AfterAP { get; set; }
    public string BuyDate { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(ItemId)) return false;
        if (!ks.Put(Quantity)) return false;
        if (!ks.Put(BeforAP)) return false;
        if (!ks.Put(AfterAP)) return false;
        if (!ks.PutWideString(BuyDate)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out int v1)) return false; ItemId = v1;
        if (!ks.Get(out int v2)) return false; Quantity = v2;
        if (!ks.Get(out int v3)) return false; BeforAP = v3;
        if (!ks.Get(out int v4)) return false; AfterAP = v4;
        if (!ks.GetWideString(out string s5)) return false; BuyDate = s5;
        return true;
    }
}

public sealed class KDBE_BUY_ED_ITEM_ACK : IKSerializable
{
    public int Ed { get; set; }
    public int APoint { get; set; }
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public Dictionary<long, int> ItemEnduranceUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();
    public Dictionary<long, KItemInfo> ItemInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ed)) return false;
        if (!ks.Put(APoint)) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutMap(ItemEnduranceUpdate, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        if (!ks.PutMap(ItemInfo, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ed = v0;
        if (!ks.Get(out int v1)) return false; APoint = v1;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetMap(ItemEnduranceUpdate, static s => (s.Get(out long v), v), static s => (s.Get(out int v), v))) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        if (!ks.GetMap(ItemInfo, static s => (s.Get(out long v), v), static s => { var v = new KItemInfo(); return (s.Get(v), v); })) return false;
        return true;
    }
}

public sealed class KDBE_BUY_ED_ITEM_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public Dictionary<long, int> ItemEnduranceUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();
    public List<KBuyGPItemInfo> BuyGPItemInfo { get; set; } = new();
    public bool Cheat { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutMap(ItemEnduranceUpdate, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        if (!ks.PutList(BuyGPItemInfo)) return false;
        if (!ks.Put(Cheat)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetMap(ItemEnduranceUpdate, static s => (s.Get(out long v), v), static s => (s.Get(out int v), v))) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        if (!ks.GetList(BuyGPItemInfo)) return false;
        if (!ks.Get(out bool v5)) return false; Cheat = v5;
        return true;
    }
}

public sealed class KDBE_BUY_PERSONAL_SHOP_ITEM_COMPLETE_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KDBE_BUY_PERSONAL_SHOP_ITEM_COMPLETE_REQ Result { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(Result)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(Result)) return false;
        return true;
    }
}

public sealed class KDBE_BUY_PERSONAL_SHOP_ITEM_COMPLETE_REQ : IKSerializable
{
    public long SellUnitUid { get; set; }
    public long SellUnitGSUid { get; set; }
    public int SellUnitEDIN { get; set; }
    public int SellCommissionED { get; set; }
    public long BuyUnitUid { get; set; }
    public long BuyUnitGSUid { get; set; }
    public int BuyUnitEDOUT { get; set; }
    public int BuyCommissionED { get; set; }
    public bool BuyUnitIsSquare { get; set; }
    public KInventoryItemInfo SellItemInfo { get; set; } = new();
    public KInventoryItemInfo BuyItemInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(SellUnitUid)) return false;
        if (!ks.Put(SellUnitGSUid)) return false;
        if (!ks.Put(SellUnitEDIN)) return false;
        if (!ks.Put(SellCommissionED)) return false;
        if (!ks.Put(BuyUnitUid)) return false;
        if (!ks.Put(BuyUnitGSUid)) return false;
        if (!ks.Put(BuyUnitEDOUT)) return false;
        if (!ks.Put(BuyCommissionED)) return false;
        if (!ks.Put(BuyUnitIsSquare)) return false;
        if (!ks.Put(SellItemInfo)) return false;
        if (!ks.Put(BuyItemInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; SellUnitUid = v0;
        if (!ks.Get(out long v1)) return false; SellUnitGSUid = v1;
        if (!ks.Get(out int v2)) return false; SellUnitEDIN = v2;
        if (!ks.Get(out int v3)) return false; SellCommissionED = v3;
        if (!ks.Get(out long v4)) return false; BuyUnitUid = v4;
        if (!ks.Get(out long v5)) return false; BuyUnitGSUid = v5;
        if (!ks.Get(out int v6)) return false; BuyUnitEDOUT = v6;
        if (!ks.Get(out int v7)) return false; BuyCommissionED = v7;
        if (!ks.Get(out bool v8)) return false; BuyUnitIsSquare = v8;
        if (!ks.Get(SellItemInfo)) return false;
        if (!ks.Get(BuyItemInfo)) return false;
        return true;
    }
}

public sealed class KDBE_BUY_PSHOP_AGENCY_ITEM_REQ : IKSerializable
{
    public int Ok { get; set; }
    public long HostUnitUid { get; set; }
    public int SellUnitEDIn { get; set; }
    public KInventoryItemInfo SellItemDBUpdate { get; set; } = new();
    public long PriceGSUid { get; set; }
    public long PriceUnitUid { get; set; }
    public int PriceUnitEDOUT { get; set; }
    public int PriceCommissionED { get; set; }
    public KInventoryItemInfo PriceIntemInfoIN { get; set; } = new();
    public KItemQuantityUpdate ItemQuantityUpdateTemp { get; set; } = new();
    public List<KItemInfo> ItemInfoTemp { get; set; } = new();
    public int EDTemp { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(HostUnitUid)) return false;
        if (!ks.Put(SellUnitEDIn)) return false;
        if (!ks.Put(SellItemDBUpdate)) return false;
        if (!ks.Put(PriceGSUid)) return false;
        if (!ks.Put(PriceUnitUid)) return false;
        if (!ks.Put(PriceUnitEDOUT)) return false;
        if (!ks.Put(PriceCommissionED)) return false;
        if (!ks.Put(PriceIntemInfoIN)) return false;
        if (!ks.Put(ItemQuantityUpdateTemp)) return false;
        if (!ks.PutList(ItemInfoTemp)) return false;
        if (!ks.Put(EDTemp)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out long v1)) return false; HostUnitUid = v1;
        if (!ks.Get(out int v2)) return false; SellUnitEDIn = v2;
        if (!ks.Get(SellItemDBUpdate)) return false;
        if (!ks.Get(out long v4)) return false; PriceGSUid = v4;
        if (!ks.Get(out long v5)) return false; PriceUnitUid = v5;
        if (!ks.Get(out int v6)) return false; PriceUnitEDOUT = v6;
        if (!ks.Get(out int v7)) return false; PriceCommissionED = v7;
        if (!ks.Get(PriceIntemInfoIN)) return false;
        if (!ks.Get(ItemQuantityUpdateTemp)) return false;
        if (!ks.GetList(ItemInfoTemp)) return false;
        if (!ks.Get(out int v11)) return false; EDTemp = v11;
        return true;
    }
}

public sealed class KDBE_BUY_UNIT_CLASS_CHANGE_REQ : IKSerializable
{
    public int Ok { get; set; }
    public long UnitUid { get; set; }
    public int NewUnitClass { get; set; }
    public int SPoint { get; set; }
    public int CSPoint { get; set; }
    public int TheNumberOfSkillPagesAvailable { get; set; }
    public int DefaultSkillID1 { get; set; }
    public int DefaultSkillID2 { get; set; }
    public int DefaultSkillID3 { get; set; }
    public int DefaultSkillID4 { get; set; }
    public int DefaultSkillID5 { get; set; }
    public int DefaultSkillID6 { get; set; }
    public Dictionary<int, int> ChangeSkill { get; set; } = new();
    public Dictionary<int, int> ChangeMemo { get; set; } = new();
    public Dictionary<long, int> ChangeItem { get; set; } = new();
    public Dictionary<int, int> ChangeCompleteQuest { get; set; } = new();
    public Dictionary<int, int> ChangeInProgressQuest { get; set; } = new();
    public List<int> DeleteCompleteQuest { get; set; } = new();
    public List<int> DeleteInProgressQuest { get; set; } = new();
    public bool UnlimitedSecondJobItem { get; set; }
    public int OldUnitClass { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(NewUnitClass)) return false;
        if (!ks.Put(SPoint)) return false;
        if (!ks.Put(CSPoint)) return false;
        if (!ks.Put(TheNumberOfSkillPagesAvailable)) return false;
        if (!ks.Put(DefaultSkillID1)) return false;
        if (!ks.Put(DefaultSkillID2)) return false;
        if (!ks.Put(DefaultSkillID3)) return false;
        if (!ks.Put(DefaultSkillID4)) return false;
        if (!ks.Put(DefaultSkillID5)) return false;
        if (!ks.Put(DefaultSkillID6)) return false;
        if (!ks.PutMap(ChangeSkill, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.PutMap(ChangeMemo, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.PutMap(ChangeItem, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.PutMap(ChangeCompleteQuest, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.PutMap(ChangeInProgressQuest, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.PutList(DeleteCompleteQuest, static (s, v) => s.Put(v))) return false;
        if (!ks.PutList(DeleteInProgressQuest, static (s, v) => s.Put(v))) return false;
        if (!ks.Put(UnlimitedSecondJobItem)) return false;
        if (!ks.Put(OldUnitClass)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        if (!ks.Get(out int v2)) return false; NewUnitClass = v2;
        if (!ks.Get(out int v3)) return false; SPoint = v3;
        if (!ks.Get(out int v4)) return false; CSPoint = v4;
        if (!ks.Get(out int v5)) return false; TheNumberOfSkillPagesAvailable = v5;
        if (!ks.Get(out int v6)) return false; DefaultSkillID1 = v6;
        if (!ks.Get(out int v7)) return false; DefaultSkillID2 = v7;
        if (!ks.Get(out int v8)) return false; DefaultSkillID3 = v8;
        if (!ks.Get(out int v9)) return false; DefaultSkillID4 = v9;
        if (!ks.Get(out int v10)) return false; DefaultSkillID5 = v10;
        if (!ks.Get(out int v11)) return false; DefaultSkillID6 = v11;
        if (!ks.GetMap(ChangeSkill, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false;
        if (!ks.GetMap(ChangeMemo, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false;
        if (!ks.GetMap(ChangeItem, static s => (s.Get(out long v), v), static s => (s.Get(out int v), v))) return false;
        if (!ks.GetMap(ChangeCompleteQuest, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false;
        if (!ks.GetMap(ChangeInProgressQuest, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false;
        if (!ks.GetList(DeleteCompleteQuest, static s => (s.Get(out int v), v))) return false;
        if (!ks.GetList(DeleteInProgressQuest, static s => (s.Get(out int v), v))) return false;
        if (!ks.Get(out bool v19)) return false; UnlimitedSecondJobItem = v19;
        if (!ks.Get(out int v20)) return false; OldUnitClass = v20;
        return true;
    }
}

public sealed class KDBE_CCU_AGENT_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int DailyUniqueUser { get; set; }
    public int DailyResisterredUser { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(DailyUniqueUser)) return false;
        if (!ks.Put(DailyResisterredUser)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out int v1)) return false; DailyUniqueUser = v1;
        if (!ks.Get(out int v2)) return false; DailyResisterredUser = v2;
        return true;
    }
}

public class KDBE_CHANGE_GUILD_MEMBER_GRADE_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int GuildUid { get; set; }
    public long TargetUnitUid { get; set; }
    public string TargetNickName { get; set; } = string.Empty;
    public byte MemberShipGrade { get; set; }
    public bool ChangeGuildMaster { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(GuildUid)) return false;
        if (!ks.Put(TargetUnitUid)) return false;
        if (!ks.PutWideString(TargetNickName)) return false;
        if (!ks.Put(MemberShipGrade)) return false;
        if (!ks.Put(ChangeGuildMaster)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out int v1)) return false; GuildUid = v1;
        if (!ks.Get(out long v2)) return false; TargetUnitUid = v2;
        if (!ks.GetWideString(out string s3)) return false; TargetNickName = s3;
        if (!ks.Get(out byte v4)) return false; MemberShipGrade = v4;
        if (!ks.Get(out bool v5)) return false; ChangeGuildMaster = v5;
        return true;
    }
}

public class KDBE_CHANGE_GUILD_MEMBER_GRADE_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public int GuildUid { get; set; }
    public long TargetUnitUid { get; set; }
    public string TargetNickName { get; set; } = string.Empty;
    public byte MemberShipGrade { get; set; }
    public bool ChangeGuildMaster { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(GuildUid)) return false;
        if (!ks.Put(TargetUnitUid)) return false;
        if (!ks.PutWideString(TargetNickName)) return false;
        if (!ks.Put(MemberShipGrade)) return false;
        if (!ks.Put(ChangeGuildMaster)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out int v1)) return false; GuildUid = v1;
        if (!ks.Get(out long v2)) return false; TargetUnitUid = v2;
        if (!ks.GetWideString(out string s3)) return false; TargetNickName = s3;
        if (!ks.Get(out byte v4)) return false; MemberShipGrade = v4;
        if (!ks.Get(out bool v5)) return false; ChangeGuildMaster = v5;
        return true;
    }
}

public sealed class KDBE_CHANGE_GUILD_MEMBER_MESSAGE_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long UnitUid { get; set; }
    public long TargetUnitUid { get; set; }
    public int GuildUid { get; set; }
    public string Message { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(TargetUnitUid)) return false;
        if (!ks.Put(GuildUid)) return false;
        if (!ks.PutWideString(Message)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        if (!ks.Get(out long v2)) return false; TargetUnitUid = v2;
        if (!ks.Get(out int v3)) return false; GuildUid = v3;
        if (!ks.GetWideString(out string s4)) return false; Message = s4;
        return true;
    }
}

public sealed class KDBE_CHANGE_GUILD_MEMBER_MESSAGE_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public long TargetUnitUid { get; set; }
    public int GuildUid { get; set; }
    public string Message { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(TargetUnitUid)) return false;
        if (!ks.Put(GuildUid)) return false;
        if (!ks.PutWideString(Message)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out long v1)) return false; TargetUnitUid = v1;
        if (!ks.Get(out int v2)) return false; GuildUid = v2;
        if (!ks.GetWideString(out string s3)) return false; Message = s3;
        return true;
    }
}

public sealed class KDBE_CHANGE_GUILD_MESSAGE_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int GuildUid { get; set; }
    public string Message { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(GuildUid)) return false;
        if (!ks.PutWideString(Message)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out int v1)) return false; GuildUid = v1;
        if (!ks.GetWideString(out string s2)) return false; Message = s2;
        return true;
    }
}

public sealed class KDBE_CHANGE_GUILD_MESSAGE_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public int GuildUid { get; set; }
    public string Message { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(GuildUid)) return false;
        if (!ks.PutWideString(Message)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out int v1)) return false; GuildUid = v1;
        if (!ks.GetWideString(out string s2)) return false; Message = s2;
        return true;
    }
}

public sealed class KDBE_CHANGE_GUILD_NAME_CHECK_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long UnitUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(UnitUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        return true;
    }
}

public sealed class KDBE_CHANGE_INVENTORY_SLOT_ITEM_ACK : IKSerializable
{
    public sbyte FromSlotType { get; set; }
    public int FromSlotId { get; set; }
    public sbyte ToSlotType { get; set; }
    public int ToSlotId { get; set; }
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public Dictionary<long, int> ItemEnduranceUpdate { get; set; } = new();
    public Dictionary<long, KItemPosition> ItemPositionUpdate { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(FromSlotType)) return false;
        if (!ks.Put(FromSlotId)) return false;
        if (!ks.Put(ToSlotType)) return false;
        if (!ks.Put(ToSlotId)) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutMap(ItemEnduranceUpdate, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.PutMap(ItemPositionUpdate, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte v0)) return false; FromSlotType = v0;
        if (!ks.Get(out int v1)) return false; FromSlotId = v1;
        if (!ks.Get(out sbyte v2)) return false; ToSlotType = v2;
        if (!ks.Get(out int v3)) return false; ToSlotId = v3;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetMap(ItemEnduranceUpdate, static s => (s.Get(out long v), v), static s => (s.Get(out int v), v))) return false;
        if (!ks.GetMap(ItemPositionUpdate, static s => (s.Get(out long v), v), static s => { var v = new KItemPosition(); return (s.Get(v), v); })) return false;
        return true;
    }
}

public sealed class KDBE_CHANGE_INVENTORY_SLOT_ITEM_REQ : IKSerializable
{
    public sbyte FromSlotType { get; set; }
    public int FromSlotId { get; set; }
    public sbyte ToSlotType { get; set; }
    public int ToSlotId { get; set; }
    public long UnitUid { get; set; }
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public Dictionary<long, int> ItemEnduranceUpdate { get; set; } = new();
    public Dictionary<long, KItemPosition> ItemPositionUpdate { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(FromSlotType)) return false;
        if (!ks.Put(FromSlotId)) return false;
        if (!ks.Put(ToSlotType)) return false;
        if (!ks.Put(ToSlotId)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutMap(ItemEnduranceUpdate, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.PutMap(ItemPositionUpdate, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte v0)) return false; FromSlotType = v0;
        if (!ks.Get(out int v1)) return false; FromSlotId = v1;
        if (!ks.Get(out sbyte v2)) return false; ToSlotType = v2;
        if (!ks.Get(out int v3)) return false; ToSlotId = v3;
        if (!ks.Get(out long v4)) return false; UnitUid = v4;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetMap(ItemEnduranceUpdate, static s => (s.Get(out long v), v), static s => (s.Get(out int v), v))) return false;
        if (!ks.GetMap(ItemPositionUpdate, static s => (s.Get(out long v), v), static s => { var v = new KItemPosition(); return (s.Get(v), v); })) return false;
        return true;
    }
}

public sealed class KDBE_CHANGE_LOVE_WORD_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long UseUnitUid { get; set; }
    public long LoverUnitUid { get; set; }
    public string NewLoveWord { get; set; } = string.Empty;
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(UseUnitUid)) return false;
        if (!ks.Put(LoverUnitUid)) return false;
        if (!ks.PutWideString(NewLoveWord)) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out long v1)) return false; UseUnitUid = v1;
        if (!ks.Get(out long v2)) return false; LoverUnitUid = v2;
        if (!ks.GetWideString(out string s3)) return false; NewLoveWord = s3;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        return true;
    }
}

public sealed class KDBE_CHANGE_LOVE_WORD_REQ : IKSerializable
{
    public long UseUnitUid { get; set; }
    public long LoverUnitUid { get; set; }
    public string NewLoveWord { get; set; } = string.Empty;
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UseUnitUid)) return false;
        if (!ks.Put(LoverUnitUid)) return false;
        if (!ks.PutWideString(NewLoveWord)) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UseUnitUid = v0;
        if (!ks.Get(out long v1)) return false; LoverUnitUid = v1;
        if (!ks.GetWideString(out string s2)) return false; NewLoveWord = s2;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        return true;
    }
}

public sealed class KDBE_CHANGE_PET_ID_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long PetUid { get; set; }
    public int BeforePetId { get; set; }
    public int AfterPetId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(PetUid)) return false;
        if (!ks.Put(BeforePetId)) return false;
        if (!ks.Put(AfterPetId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out long v1)) return false; PetUid = v1;
        if (!ks.Get(out int v2)) return false; BeforePetId = v2;
        if (!ks.Get(out int v3)) return false; AfterPetId = v3;
        return true;
    }
}

public sealed class KDBE_CHANGE_PET_ID_LOG_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public long PetUid { get; set; }
    public int BeforePetId { get; set; }
    public int AfterPetId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(PetUid)) return false;
        if (!ks.Put(BeforePetId)) return false;
        if (!ks.Put(AfterPetId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out long v1)) return false; PetUid = v1;
        if (!ks.Get(out int v2)) return false; BeforePetId = v2;
        if (!ks.Get(out int v3)) return false; AfterPetId = v3;
        return true;
    }
}

public sealed class KDBE_CHANGE_PET_ID_REQ : IKSerializable
{
    public long PetUid { get; set; }
    public int BeforePetId { get; set; }
    public int AfterPetId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PetUid)) return false;
        if (!ks.Put(BeforePetId)) return false;
        if (!ks.Put(AfterPetId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; PetUid = v0;
        if (!ks.Get(out int v1)) return false; BeforePetId = v1;
        if (!ks.Get(out int v2)) return false; AfterPetId = v2;
        return true;
    }
}

public sealed class KDBE_CHANGE_PET_NAME_ACK : IKSerializable
{
    public long ItemUid { get; set; }
    public int Ok { get; set; }
    public KPetInfo PetInfo { get; set; } = new();
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ItemUid)) return false;
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(PetInfo)) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; ItemUid = v0;
        if (!ks.Get(out int v1)) return false; Ok = v1;
        if (!ks.Get(PetInfo)) return false;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        return true;
    }
}

public sealed class KDBE_CHANGE_PET_NAME_REQ : IKSerializable
{
    public long ItemUid { get; set; }
    public long UnitUid { get; set; }
    public KPetInfo PetInfo { get; set; } = new();
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ItemUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(PetInfo)) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; ItemUid = v0;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        if (!ks.Get(PetInfo)) return false;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        return true;
    }
}

public sealed class KDBE_CHANGE_SECOND_SECURITY_PW_REQ : IKSerializable
{
    public long UserId { get; set; }
    public string OldSecondPW { get; set; } = string.Empty;
    public string NewSecondPW { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UserId)) return false;
        if (!ks.PutWideString(OldSecondPW)) return false;
        if (!ks.PutWideString(NewSecondPW)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UserId = v0;
        if (!ks.GetWideString(out string s1)) return false; OldSecondPW = s1;
        if (!ks.GetWideString(out string s2)) return false; NewSecondPW = s2;
        return true;
    }
}

public sealed class KDBE_CHANGE_UNIT_CLASS_ACK : IKSerializable
{
    public int Ok { get; set; }
    public ushort EventId { get; set; }
    public sbyte UnitClass { get; set; }
    public int NewDefaultSkill1 { get; set; }
    public int NewDefaultSkill2 { get; set; }
    public int TheNumberOfSkillPagesAvailable { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(EventId)) return false;
        if (!ks.Put(UnitClass)) return false;
        if (!ks.Put(NewDefaultSkill1)) return false;
        if (!ks.Put(NewDefaultSkill2)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out ushort v1)) return false; EventId = v1;
        if (!ks.Get(out sbyte v2)) return false; UnitClass = v2;
        if (!ks.Get(out int v3)) return false; NewDefaultSkill1 = v3;
        if (!ks.Get(out int v4)) return false; NewDefaultSkill2 = v4;
        return true;
    }
}

public sealed class KDBE_CHANGE_UNIT_CLASS_REQ : IKSerializable
{
    public ushort EventId { get; set; }
    public long UnitUid { get; set; }
    public sbyte UnitClass { get; set; }
    public int NewDefaultSkill1 { get; set; }
    public int NewDefaultSkill2 { get; set; }
    public int TheNumberOfSkillPagesAvailable { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(EventId)) return false;
        if (!ks.Put(UnitClass)) return false;
        if (!ks.Put(NewDefaultSkill1)) return false;
        if (!ks.Put(NewDefaultSkill2)) return false;
        if (!ks.Put(TheNumberOfSkillPagesAvailable)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out ushort v1)) return false; EventId = v1;
        if (!ks.Get(out sbyte v2)) return false; UnitClass = v2;
        if (!ks.Get(out int v3)) return false; NewDefaultSkill1 = v3;
        if (!ks.Get(out int v4)) return false; NewDefaultSkill2 = v4;
        if (!ks.Get(out int v5)) return false; TheNumberOfSkillPagesAvailable = v5;
        return true;
    }
}

public sealed class KDBE_CHANNEL_CHANGE_GAME_SELECT_UNIT_ACK : IKSerializable
{
    public KEGS_CONNECT_ACK ConnectAck { get; set; } = new();
    public KEGS_VERIFY_ACCOUNT_ACK VerifyAccountAck { get; set; } = new();
    public KDBE_SELECT_UNIT_ACK SelectUnitAck { get; set; } = new();
    public KChannelChangeInfo ChangeUserInfo { get; set; } = new();
    public int NewUnitCh { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ConnectAck)) return false;
        if (!ks.Put(VerifyAccountAck)) return false;
        if (!ks.Put(SelectUnitAck)) return false;
        if (!ks.Put(ChangeUserInfo)) return false;
        if ((region.Region == RegionCode.JP) && !ks.Put(NewUnitCh)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(ConnectAck)) return false;
        if (!ks.Get(VerifyAccountAck)) return false;
        if (!ks.Get(SelectUnitAck)) return false;
        if (!ks.Get(ChangeUserInfo)) return false;
        if ((region.Region == RegionCode.JP)) { if (!ks.Get(out int v4)) return false; NewUnitCh = v4; }
        return true;
    }
}

public sealed class KDBE_CHANNEL_CHANGE_GAME_SELECT_UNIT_EVENT_DATA_NOT : IKSerializable
{
    public long UserUid { get; set; }
    public KDBE_CHANNEL_CHANGE_GAME_SELECT_UNIT_ACK SelectUnitAck { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.JP) && !ks.Put(UserUid)) return false;
        if ((region.Region == RegionCode.JP) && !ks.Put(SelectUnitAck)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.JP)) { if (!ks.Get(out long v0)) return false; UserUid = v0; }
        if ((region.Region == RegionCode.JP)) { if (!ks.Get(SelectUnitAck)) return false; }
        return true;
    }
}

public sealed class KDBE_CHANNEL_CHANGE_GAME_SELECT_UNIT_REQ : IKSerializable
{
    public KEGS_CONNECT_ACK ConnectAck { get; set; } = new();
    public KEGS_VERIFY_ACCOUNT_ACK VerifyAccountAck { get; set; } = new();
    public KEGS_SELECT_UNIT_REQ SelectUnitReq { get; set; } = new();
    public KChannelChangeInfo ChangeUserInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ConnectAck)) return false;
        if (!ks.Put(VerifyAccountAck)) return false;
        if (!ks.Put(SelectUnitReq)) return false;
        if (!ks.Put(ChangeUserInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(ConnectAck)) return false;
        if (!ks.Get(VerifyAccountAck)) return false;
        if (!ks.Get(SelectUnitReq)) return false;
        if (!ks.Get(ChangeUserInfo)) return false;
        return true;
    }
}

public sealed class KDBE_CHARACTER_LEVELUP_NOT : IKSerializable
{
    public long CharUid { get; set; }
    public byte LevelBefore { get; set; }
    public byte LevelAfter { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(CharUid)) return false;
        if (!ks.Put(LevelBefore)) return false;
        if (!ks.Put(LevelAfter)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; CharUid = v0;
        if (!ks.Get(out byte v1)) return false; LevelBefore = v1;
        if (!ks.Get(out byte v2)) return false; LevelAfter = v2;
        return true;
    }
}

public sealed class KDBE_CHAT_OPTION_INFO_WRITE_REQ : IKSerializable
{
    public long UserUid { get; set; }
    public byte Index { get; set; }
    public bool Value { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UserUid)) return false;
        if (!ks.Put(Index)) return false;
        if (!ks.Put(Value)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UserUid = v0;
        if (!ks.Get(out byte v1)) return false; Index = v1;
        if (!ks.Get(out bool v2)) return false; Value = v2;
        return true;
    }
}

public sealed class KDBE_CHECK_BLOCK_LIST_ACK : IKSerializable
{
    public int ReleaseTick { get; set; }
    public Dictionary<int, List<KBlockInfo>> BlockInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ReleaseTick)) return false;
        if (!ks.PutMap(BlockInfo, static (s, k) => s.Put(k), static (s, v) => s.PutList(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; ReleaseTick = v0;
        if (!ks.GetMap(BlockInfo, static s => (s.Get(out int v), v), static s => { var l = new List<KBlockInfo>(); return (s.GetList(l), l); })) return false;
        return true;
    }
}

public sealed class KDBE_CHECK_BLOCK_LIST_REQ : IKSerializable
{
    public int ReleaseTick { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ReleaseTick)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; ReleaseTick = v0;
        return true;
    }
}

public sealed class KDBE_CHECK_EVENT_UPDATE_ACK : IKSerializable
{
    public Dictionary<int, int> ReleaseTick { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutMap(ReleaseTick, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetMap(ReleaseTick, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false;
        return true;
    }
}

public sealed class KDBE_CHECK_REWARD_FROM_RELAY_DB_JP_EVENT_ACK : IKSerializable
{
    public List<KJPEventRewardInfo> RewardInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutList(RewardInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetList(RewardInfo)) return false;
        return true;
    }
}

public sealed class KDBE_CHECK_REWARD_FROM_RELAY_DB_JP_EVENT_REQ : IKSerializable
{
    public long UserUid { get; set; }
    public long UnitUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UserUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UserUid = v0;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        return true;
    }
}

public sealed class KDBE_CHECK_TIME_DROP_RESULT_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long GsUid { get; set; }
    public long UserUid { get; set; }
    public long UnitUid { get; set; }
    public string NickName { get; set; } = string.Empty;
    public int TimeDropId { get; set; }
    public int RewardItemId { get; set; }
    public string RewardItemName { get; set; } = string.Empty;
    public int DropItemUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(GsUid)) return false;
        if (!ks.Put(UserUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutWideString(NickName)) return false;
        if (!ks.Put(TimeDropId)) return false;
        if (!ks.Put(RewardItemId)) return false;
        if (!ks.PutWideString(RewardItemName)) return false;
        if (!ks.Put(DropItemUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out long v1)) return false; GsUid = v1;
        if (!ks.Get(out long v2)) return false; UserUid = v2;
        if (!ks.Get(out long v3)) return false; UnitUid = v3;
        if (!ks.GetWideString(out string s4)) return false; NickName = s4;
        if (!ks.Get(out int v5)) return false; TimeDropId = v5;
        if (!ks.Get(out int v6)) return false; RewardItemId = v6;
        if (!ks.GetWideString(out string s7)) return false; RewardItemName = s7;
        if (!ks.Get(out int v8)) return false; DropItemUid = v8;
        return true;
    }
}

public sealed class KDBE_CHECK_TIME_DROP_RESULT_REQ : IKSerializable
{
    public long GsUid { get; set; }
    public long UserUid { get; set; }
    public long UnitUid { get; set; }
    public string NickName { get; set; } = string.Empty;
    public int TimeDropId { get; set; }
    public int RewardItemId { get; set; }
    public string RewardItemName { get; set; } = string.Empty;
    public int DropItemUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(GsUid)) return false;
        if (!ks.Put(UserUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutWideString(NickName)) return false;
        if (!ks.Put(TimeDropId)) return false;
        if (!ks.Put(RewardItemId)) return false;
        if (!ks.PutWideString(RewardItemName)) return false;
        if (!ks.Put(DropItemUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; GsUid = v0;
        if (!ks.Get(out long v1)) return false; UserUid = v1;
        if (!ks.Get(out long v2)) return false; UnitUid = v2;
        if (!ks.GetWideString(out string s3)) return false; NickName = s3;
        if (!ks.Get(out int v4)) return false; TimeDropId = v4;
        if (!ks.Get(out int v5)) return false; RewardItemId = v5;
        if (!ks.GetWideString(out string s6)) return false; RewardItemName = s6;
        if (!ks.Get(out int v7)) return false; DropItemUid = v7;
        return true;
    }
}

public sealed class KDBE_CHECK_TIME_EVENT_COMPLETE_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int RewardCount { get; set; }
    public List<int> RestartEvent { get; set; } = new();
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();
    public Dictionary<long, KItemInfo> ItemInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(RewardCount)) return false;
        if (!ks.PutList(RestartEvent, static (s, v) => s.Put(v))) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        if (!ks.PutMap(ItemInfo, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out int v1)) return false; RewardCount = v1;
        if (!ks.GetList(RestartEvent, static s => (s.Get(out int v), v))) return false;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        if (!ks.GetMap(ItemInfo, static s => (s.Get(out long v), v), static s => { var v = new KItemInfo(); return (s.Get(v), v); })) return false;
        return true;
    }
}

public sealed class KDBE_CHECK_TIME_EVENT_COMPLETE_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public List<KCumulativeTimeEventInfo> CompletedEvent { get; set; } = new();
    public Dictionary<int, int> InsertedItem { get; set; } = new();
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();
    public List<KItemInfo> ItemInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutList(CompletedEvent)) return false;
        if (!ks.PutMap(InsertedItem, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        if (!ks.PutList(ItemInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.GetList(CompletedEvent)) return false;
        if (!ks.GetMap(InsertedItem, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        if (!ks.GetList(ItemInfo)) return false;
        return true;
    }
}

public class KDBE_COUPLE_MAKING_SUCCESS_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long RequestUnitUid { get; set; }
    public string RequestUnitName { get; set; } = string.Empty;
    public long AcceptUserUid { get; set; }
    public byte AcceptUnitLevel { get; set; }
    public sbyte AcceptUnitClass { get; set; }
    public long AcceptUnitUid { get; set; }
    public string AcceptUnitName { get; set; } = string.Empty;
    public long Date { get; set; }
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(RequestUnitUid)) return false;
        if (!ks.PutWideString(RequestUnitName)) return false;
        if (!ks.Put(AcceptUserUid)) return false;
        if (!ks.Put(AcceptUnitLevel)) return false;
        if (!ks.Put(AcceptUnitClass)) return false;
        if (!ks.Put(AcceptUnitUid)) return false;
        if (!ks.PutWideString(AcceptUnitName)) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        if (!ks.Put(Date)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out long v1)) return false; RequestUnitUid = v1;
        if (!ks.GetWideString(out string s2)) return false; RequestUnitName = s2;
        if (!ks.Get(out long v3)) return false; AcceptUserUid = v3;
        if (!ks.Get(out byte v4)) return false; AcceptUnitLevel = v4;
        if (!ks.Get(out sbyte v5)) return false; AcceptUnitClass = v5;
        if (!ks.Get(out long v6)) return false; AcceptUnitUid = v6;
        if (!ks.GetWideString(out string s7)) return false; AcceptUnitName = s7;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        if (!ks.Get(out long v10)) return false; Date = v10;
        return true;
    }
}

public class KDBE_COUPLE_MAKING_SUCCESS_REQ : IKSerializable
{
    public long RequestUnitUid { get; set; }
    public string RequestUnitName { get; set; } = string.Empty;
    public long AcceptUserUid { get; set; }
    public byte AcceptUnitLevel { get; set; }
    public sbyte AcceptUnitClass { get; set; }
    public long AcceptUnitUid { get; set; }
    public string AcceptUnitName { get; set; } = string.Empty;
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();
    public List<KDeletedItemInfo> Deleted { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(RequestUnitUid)) return false;
        if (!ks.PutWideString(RequestUnitName)) return false;
        if (!ks.Put(AcceptUserUid)) return false;
        if (!ks.Put(AcceptUnitLevel)) return false;
        if (!ks.Put(AcceptUnitClass)) return false;
        if (!ks.Put(AcceptUnitUid)) return false;
        if (!ks.PutWideString(AcceptUnitName)) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        if (!ks.PutList(Deleted)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; RequestUnitUid = v0;
        if (!ks.GetWideString(out string s1)) return false; RequestUnitName = s1;
        if (!ks.Get(out long v2)) return false; AcceptUserUid = v2;
        if (!ks.Get(out byte v3)) return false; AcceptUnitLevel = v3;
        if (!ks.Get(out sbyte v4)) return false; AcceptUnitClass = v4;
        if (!ks.Get(out long v5)) return false; AcceptUnitUid = v5;
        if (!ks.GetWideString(out string s6)) return false; AcceptUnitName = s6;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        if (!ks.GetList(Deleted)) return false;
        return true;
    }
}

public sealed class KDBE_COUPLE_PROPOSE_USER_FIND_ACK : IKSerializable
{
    public int Ok { get; set; }
    public string OtherNickName { get; set; } = string.Empty;
    public long OtherUserUid { get; set; }
    public long OtherUnitUid { get; set; }
    public sbyte UnitClass { get; set; }
    public sbyte RelationshipType { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.PutWideString(OtherNickName)) return false;
        if (!ks.Put(OtherUserUid)) return false;
        if (!ks.Put(OtherUnitUid)) return false;
        if (!ks.Put(UnitClass)) return false;
        if (!ks.Put(RelationshipType)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.GetWideString(out string s1)) return false; OtherNickName = s1;
        if (!ks.Get(out long v2)) return false; OtherUserUid = v2;
        if (!ks.Get(out long v3)) return false; OtherUnitUid = v3;
        if (!ks.Get(out sbyte v4)) return false; UnitClass = v4;
        if (!ks.Get(out sbyte v5)) return false; RelationshipType = v5;
        return true;
    }
}

public sealed class KDBE_CREATE_PET_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KPetInfo PetInfo { get; set; } = new();
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(PetInfo)) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(PetInfo)) return false;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        return true;
    }
}

public sealed class KDBE_CREATE_PET_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public KPetInfo PetInfo { get; set; } = new();
    public short Period { get; set; }
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(PetInfo)) return false;
        if (!ks.Put(Period)) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(PetInfo)) return false;
        if (!ks.Get(out short v2)) return false; Period = v2;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        return true;
    }
}

public sealed class KDBE_CREATE_RIDING_PET_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KRidingPetInfo CreatedPetInfo { get; set; } = new();
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(CreatedPetInfo)) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(CreatedPetInfo)) return false;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        return true;
    }
}

public sealed class KDBE_CREATE_RIDING_PET_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public ushort CreatePetId { get; set; }
    public short Period { get; set; }
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(CreatePetId)) return false;
        if (!ks.Put(Period)) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out ushort v1)) return false; CreatePetId = v1;
        if (!ks.Get(out short v2)) return false; Period = v2;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        return true;
    }
}

public class KDBE_CREATE_SECOND_SECURITY_ACK : IKSerializable
{
    public int Ok { get; set; }
    public string SecondPW { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.PutWideString(SecondPW)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.GetWideString(out string s1)) return false; SecondPW = s1;
        return true;
    }
}

public class KDBE_CREATE_SECOND_SECURITY_REQ : IKSerializable
{
    public long UserId { get; set; }
    public string SecondPW { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UserId)) return false;
        if (!ks.PutWideString(SecondPW)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UserId = v0;
        if (!ks.GetWideString(out string s1)) return false; SecondPW = s1;
        return true;
    }
}

public sealed class KDBE_DB_UPDATE_TRADE_RESULT_ACK : IKSerializable
{
    public int Ok { get; set; }
    public List<KERM_DB_UPDATE_TRADE_RESULT_NOT> DBUpdate { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.PutList(DBUpdate)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.GetList(DBUpdate)) return false;
        return true;
    }
}

public sealed class KDBE_DB_UPDATE_TRADE_RESULT_REQ : IKSerializable
{
    public List<KERM_DB_UPDATE_TRADE_RESULT_NOT> DBUpdate { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutList(DBUpdate)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetList(DBUpdate)) return false;
        return true;
    }
}

public sealed class KDBE_DECIDE_TO_USE_THIS_SKILL_PAGE_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int SkillPagesNumberToBeActive { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(SkillPagesNumberToBeActive)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out int v1)) return false; SkillPagesNumberToBeActive = v1;
        return true;
    }
}

public sealed class KDBE_DECIDE_TO_USE_THIS_SKILL_PAGE_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public int SkillPagesNumberToBeActive { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(SkillPagesNumberToBeActive)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out int v1)) return false; SkillPagesNumberToBeActive = v1;
        return true;
    }
}

public sealed class KDBE_DELETE_APLLY_FOR_ACCEPT_JOIN_GUILD_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int GuildUid { get; set; }
    public long UnitUid { get; set; }
    public long ApplyJoinUnitUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(GuildUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(ApplyJoinUnitUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out int v1)) return false; GuildUid = v1;
        if (!ks.Get(out long v2)) return false; UnitUid = v2;
        if (!ks.Get(out long v3)) return false; ApplyJoinUnitUid = v3;
        return true;
    }
}

public sealed class KDBE_DELETE_APPLY_JOIN_GUILD_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int GuildUid { get; set; }
    public long UnitUid { get; set; }
    public long DeletedUnitUid { get; set; }
    public bool DeleteChar { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(GuildUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(DeletedUnitUid)) return false;
        if (!ks.Put(DeleteChar)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out int v1)) return false; GuildUid = v1;
        if (!ks.Get(out long v2)) return false; UnitUid = v2;
        if (!ks.Get(out long v3)) return false; DeletedUnitUid = v3;
        if (!ks.Get(out bool v4)) return false; DeleteChar = v4;
        return true;
    }
}

public sealed class KDBE_DELETE_FRIEND_GROUP_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public sbyte GroupId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(GroupId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out sbyte v1)) return false; GroupId = v1;
        return true;
    }
}

public sealed class KDBE_DELETE_FRIEND_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public long FriendUnitUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(FriendUnitUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out long v1)) return false; FriendUnitUid = v1;
        return true;
    }
}

public sealed class KDBE_DELETE_GUILD_AD_LIST_NOT : IKSerializable
{
    public List<int> DeleteGuildAd { get; set; } = new();
    public List<KApplyDeleteInfo> DeleteApply { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutList(DeleteGuildAd, static (s, v) => s.Put(v))) return false;
        if (!ks.PutList(DeleteApply)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetList(DeleteGuildAd, static s => (s.Get(out int v), v))) return false;
        if (!ks.GetList(DeleteApply)) return false;
        return true;
    }
}

public sealed class KDBE_DELETE_LETTER_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public List<long> PostNo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutList(PostNo, static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.GetList(PostNo, static s => (s.Get(out long v), v))) return false;
        return true;
    }
}

public class KDBE_DELETE_NICK_NAME_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public string NickName { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutWideString(NickName)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.GetWideString(out string s1)) return false; NickName = s1;
        return true;
    }
}

public sealed class KDBE_DELETE_RANKING_INFO_NOT : IKSerializable
{
    public int RankingType { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(RankingType)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; RankingType = v0;
        return true;
    }
}

public sealed class KDBE_DEL_BLACKLIST_USER_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long BlackListUnitUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(BlackListUnitUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out long v1)) return false; BlackListUnitUid = v1;
        return true;
    }
}

public sealed class KDBE_DEL_BLACKLIST_USER_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public long BlackListUnitUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(BlackListUnitUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out long v1)) return false; BlackListUnitUid = v1;
        return true;
    }
}

public sealed class KDBE_DEL_TUTORIAL_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long TeacherUid { get; set; }
    public long StudentUid { get; set; }
    public sbyte Reason { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(TeacherUid)) return false;
        if (!ks.Put(StudentUid)) return false;
        if (!ks.Put(Reason)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out long v1)) return false; TeacherUid = v1;
        if (!ks.Get(out long v2)) return false; StudentUid = v2;
        if (!ks.Get(out sbyte v3)) return false; Reason = v3;
        return true;
    }
}

public sealed class KDBE_DEL_TUTORIAL_REQ : IKSerializable
{
    public long TeacherUid { get; set; }
    public long StudentUid { get; set; }
    public sbyte Reason { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(TeacherUid)) return false;
        if (!ks.Put(StudentUid)) return false;
        if (!ks.Put(Reason)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; TeacherUid = v0;
        if (!ks.Get(out long v1)) return false; StudentUid = v1;
        if (!ks.Get(out sbyte v2)) return false; Reason = v2;
        return true;
    }
}

public sealed class KDBE_DENY_FRIEND_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public long FriendUnitUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(FriendUnitUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out long v1)) return false; FriendUnitUid = v1;
        return true;
    }
}

public sealed class KDBE_DISBAND_GUILD_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int GuildUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(GuildUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out int v1)) return false; GuildUid = v1;
        return true;
    }
}

public sealed class KDBE_DISBAND_GUILD_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public int GuildUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(GuildUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out int v1)) return false; GuildUid = v1;
        return true;
    }
}

public sealed class KDBE_DUNGEON_RESULT_LEAVE_LOG_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public string LeaveReason { get; set; } = string.Empty;
    public string RoomState { get; set; } = string.Empty;
    public string RegDate { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutWideString(LeaveReason)) return false;
        if (!ks.PutWideString(RoomState)) return false;
        if (!ks.PutWideString(RegDate)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.GetWideString(out string s1)) return false; LeaveReason = s1;
        if (!ks.GetWideString(out string s2)) return false; RoomState = s2;
        if (!ks.GetWideString(out string s3)) return false; RegDate = s3;
        return true;
    }
}

public sealed class KDBE_ENCHANT_ATTACH_ITEM_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long ItemUid { get; set; }
    public int LevelAfterEnchant { get; set; }
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(ItemUid)) return false;
        if (!ks.Put(LevelAfterEnchant)) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out long v1)) return false; ItemUid = v1;
        if (!ks.Get(out int v2)) return false; LevelAfterEnchant = v2;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        return true;
    }
}

public sealed class KDBE_ENCHANT_ATTACH_ITEM_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public long ItemUid { get; set; }
    public int LevelAfterEnchant { get; set; }
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(ItemUid)) return false;
        if (!ks.Put(LevelAfterEnchant)) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out long v1)) return false; ItemUid = v1;
        if (!ks.Get(out int v2)) return false; LevelAfterEnchant = v2;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        return true;
    }
}

public sealed class KDBE_ENCHANT_ITEM_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int EnchantResult { get; set; }
    public long ItemUid { get; set; }
    public int ItemId { get; set; }
    public int LevelAfterEnchant { get; set; }
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(EnchantResult)) return false;
        if (!ks.Put(ItemUid)) return false;
        if (!ks.Put(ItemId)) return false;
        if (!ks.Put(LevelAfterEnchant)) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out int v1)) return false; EnchantResult = v1;
        if (!ks.Get(out long v2)) return false; ItemUid = v2;
        if (!ks.Get(out int v3)) return false; ItemId = v3;
        if (!ks.Get(out int v4)) return false; LevelAfterEnchant = v4;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        return true;
    }
}

public sealed class KDBE_ENCHANT_ITEM_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public int EnchantResult { get; set; }
    public long ItemUid { get; set; }
    public int ItemId { get; set; }
    public int LevelAfterEnchant { get; set; }
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(EnchantResult)) return false;
        if (!ks.Put(ItemUid)) return false;
        if (!ks.Put(ItemId)) return false;
        if (!ks.Put(LevelAfterEnchant)) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out int v1)) return false; EnchantResult = v1;
        if (!ks.Get(out long v2)) return false; ItemUid = v2;
        if (!ks.Get(out int v3)) return false; ItemId = v3;
        if (!ks.Get(out int v4)) return false; LevelAfterEnchant = v4;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        return true;
    }
}

public sealed class KDBE_EVENT_DIVORCE_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long RelationTargetUserUid { get; set; }
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.CN) && !ks.Put(Ok)) return false;
        if ((region.Region == RegionCode.CN) && !ks.Put(RelationTargetUserUid)) return false;
        if ((region.Region == RegionCode.CN) && !ks.Put(ItemQuantityUpdate)) return false;
        if ((region.Region == RegionCode.CN) && !ks.PutList(UpdatedInventorySlot)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out int v0)) return false; Ok = v0; }
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out long v1)) return false; RelationTargetUserUid = v1; }
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(ItemQuantityUpdate)) return false; }
        if ((region.Region == RegionCode.CN)) { if (!ks.GetList(UpdatedInventorySlot)) return false; }
        return true;
    }
}

public sealed class KDBE_EVENT_DIVORCE_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public long RelationTargetUserUid { get; set; }
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();
    public List<KDeletedItemInfo> Deleted { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.CN) && !ks.Put(UnitUid)) return false;
        if ((region.Region == RegionCode.CN) && !ks.Put(RelationTargetUserUid)) return false;
        if ((region.Region == RegionCode.CN) && !ks.PutList(UpdatedInventorySlot)) return false;
        if ((region.Region == RegionCode.CN) && !ks.PutList(Deleted)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out long v0)) return false; UnitUid = v0; }
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out long v1)) return false; RelationTargetUserUid = v1; }
        if ((region.Region == RegionCode.CN)) { if (!ks.GetList(UpdatedInventorySlot)) return false; }
        if ((region.Region == RegionCode.CN)) { if (!ks.GetList(Deleted)) return false; }
        return true;
    }
}

public sealed class KDBE_EVENT_MAKING_SUCCESS_ACCEPTOR_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.CN) && !ks.Put(Ok)) return false;
        if ((region.Region == RegionCode.CN) && !ks.Put(ItemQuantityUpdate)) return false;
        if ((region.Region == RegionCode.CN) && !ks.PutList(UpdatedInventorySlot)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out int v0)) return false; Ok = v0; }
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(ItemQuantityUpdate)) return false; }
        if ((region.Region == RegionCode.CN)) { if (!ks.GetList(UpdatedInventorySlot)) return false; }
        return true;
    }
}

public sealed class KDBE_EVENT_MAKING_SUCCESS_ACCEPTOR_REQ : IKSerializable
{
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();
    public List<KDeletedItemInfo> Deleted { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.CN) && !ks.PutList(UpdatedInventorySlot)) return false;
        if ((region.Region == RegionCode.CN) && !ks.PutList(Deleted)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.CN)) { if (!ks.GetList(UpdatedInventorySlot)) return false; }
        if ((region.Region == RegionCode.CN)) { if (!ks.GetList(Deleted)) return false; }
        return true;
    }
}

public sealed class KDBE_EVENT_PROPOSE_USER_FIND_ACK : IKSerializable
{
    public int Ok { get; set; }
    public string OtherNickName { get; set; } = string.Empty;
    public long OtherUserUid { get; set; }
    public long OtherUnitUid { get; set; }
    public sbyte UnitClass { get; set; }
    public bool Couple { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.CN) && !ks.Put(Ok)) return false;
        if ((region.Region == RegionCode.CN) && !ks.PutWideString(OtherNickName)) return false;
        if ((region.Region == RegionCode.CN) && !ks.Put(OtherUserUid)) return false;
        if ((region.Region == RegionCode.CN) && !ks.Put(OtherUnitUid)) return false;
        if ((region.Region == RegionCode.CN) && !ks.Put(UnitClass)) return false;
        if ((region.Region == RegionCode.CN) && !ks.Put(Couple)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out int v0)) return false; Ok = v0; }
        if ((region.Region == RegionCode.CN)) { if (!ks.GetWideString(out string s1)) return false; OtherNickName = s1; }
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out long v2)) return false; OtherUserUid = v2; }
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out long v3)) return false; OtherUnitUid = v3; }
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out sbyte v4)) return false; UnitClass = v4; }
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out bool v5)) return false; Couple = v5; }
        return true;
    }
}

public sealed class KDBE_EVENT_UPDATE_ACK : IKSerializable
{
    public List<KEventInfo> EventList { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutList(EventList)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetList(EventList)) return false;
        return true;
    }
}

public sealed class KDBE_EXCHANGE_LIMIT_INFO_ACK : IKSerializable
{
    public List<KExchangeLimitInfo> ExchangeLimitInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutList(ExchangeLimitInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetList(ExchangeLimitInfo)) return false;
        return true;
    }
}

public sealed class KDBE_EXCHANGE_LIMIT_INFO_NOT : IKSerializable
{
    public int ExchangeLimitUid { get; set; }
    public int LimitUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ExchangeLimitUid)) return false;
        if (!ks.Put(LimitUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; ExchangeLimitUid = v0;
        if (!ks.Get(out int v1)) return false; LimitUid = v1;
        return true;
    }
}

public sealed class KDBE_EXCHANGE_LIMIT_INFO_ROLLBACK_NOT : IKSerializable
{
    public int RollBackUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(RollBackUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; RollBackUid = v0;
        return true;
    }
}

public sealed class KDBE_EXCHANGE_LIMIT_INFO_UPDATE_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int ExchangeLimitUid { get; set; }
    public int LimitUid { get; set; }
    public int RollBackUid { get; set; }
    public long GsUid { get; set; }
    public long UnitUid { get; set; }
    public int AfterWorkStorageKey { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(ExchangeLimitUid)) return false;
        if (!ks.Put(LimitUid)) return false;
        if (!ks.Put(RollBackUid)) return false;
        if (!ks.Put(GsUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(AfterWorkStorageKey)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out int v1)) return false; ExchangeLimitUid = v1;
        if (!ks.Get(out int v2)) return false; LimitUid = v2;
        if (!ks.Get(out int v3)) return false; RollBackUid = v3;
        if (!ks.Get(out long v4)) return false; GsUid = v4;
        if (!ks.Get(out long v5)) return false; UnitUid = v5;
        if (!ks.Get(out int v6)) return false; AfterWorkStorageKey = v6;
        return true;
    }
}

public sealed class KDBE_EXCHANGE_LIMIT_INFO_UPDATE_REQ : IKSerializable
{
    public int ExchangeLimitUid { get; set; }
    public int LimitUid { get; set; }
    public int DestItemId { get; set; }
    public string RegDate { get; set; } = string.Empty;
    public long GsUid { get; set; }
    public long UnitUid { get; set; }
    public int AfterWorkStorageKey { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ExchangeLimitUid)) return false;
        if (!ks.Put(LimitUid)) return false;
        if (!ks.Put(DestItemId)) return false;
        if (!ks.PutWideString(RegDate)) return false;
        if (!ks.Put(GsUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(AfterWorkStorageKey)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; ExchangeLimitUid = v0;
        if (!ks.Get(out int v1)) return false; LimitUid = v1;
        if (!ks.Get(out int v2)) return false; DestItemId = v2;
        if (!ks.GetWideString(out string s3)) return false; RegDate = s3;
        if (!ks.Get(out long v4)) return false; GsUid = v4;
        if (!ks.Get(out long v5)) return false; UnitUid = v5;
        if (!ks.Get(out int v6)) return false; AfterWorkStorageKey = v6;
        return true;
    }
}

public sealed class KDBE_EXPAND_CHAR_SLOT_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int CharSlotSize { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(CharSlotSize)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out int v1)) return false; CharSlotSize = v1;
        return true;
    }
}

public sealed class KDBE_EXPAND_CHAR_SLOT_REQ : IKSerializable
{
    public int ExpandSlotSize { get; set; }
    public int CharSlotMax { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ExpandSlotSize)) return false;
        if (!ks.Put(CharSlotMax)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; ExpandSlotSize = v0;
        if (!ks.Get(out int v1)) return false; CharSlotMax = v1;
        return true;
    }
}

public sealed class KDBE_EXPAND_GUILD_MAX_MEMBER_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int GuildUid { get; set; }
    public ushort MaxNumMember { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(GuildUid)) return false;
        if (!ks.Put(MaxNumMember)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out int v1)) return false; GuildUid = v1;
        if (!ks.Get(out ushort v2)) return false; MaxNumMember = v2;
        return true;
    }
}

public sealed class KDBE_EXPAND_GUILD_MAX_MEMBER_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public int GuildUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(GuildUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out int v1)) return false; GuildUid = v1;
        return true;
    }
}

public class KDBE_EXPAND_INVENTORY_SLOT_ACK : IKSerializable
{
    public ushort EventId { get; set; }
    public Dictionary<int, int> ExpandedSlot { get; set; } = new();
    public int Ed { get; set; }
    public int Ok { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutMap(ExpandedSlot, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.Put(EventId)) return false;
        if (!ks.Put(Ed)) return false;
        if (!ks.Put(Ok)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetMap(ExpandedSlot, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false;
        if (!ks.Get(out ushort v1)) return false; EventId = v1;
        if (!ks.Get(out int v2)) return false; Ed = v2;
        if (!ks.Get(out int v3)) return false; Ok = v3;
        return true;
    }
}

public class KDBE_EXPAND_INVENTORY_SLOT_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public ushort EventId { get; set; }
    public Dictionary<int, int> ExpandedSlot { get; set; } = new();
    public int Ed { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(EventId)) return false;
        if (!ks.PutMap(ExpandedSlot, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.Put(Ed)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out ushort v1)) return false; EventId = v1;
        if (!ks.GetMap(ExpandedSlot, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false;
        if (!ks.Get(out int v3)) return false; Ed = v3;
        return true;
    }
}

public sealed class KDBE_EXPAND_SKILL_NOTE_PAGE_ACK : IKSerializable
{
    public int Ok { get; set; }
    public sbyte ExpandedMaxPageNum { get; set; }
    public sbyte RollBackMaxPageNum { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(ExpandedMaxPageNum)) return false;
        if (!ks.Put(RollBackMaxPageNum)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out sbyte v1)) return false; ExpandedMaxPageNum = v1;
        if (!ks.Get(out sbyte v2)) return false; RollBackMaxPageNum = v2;
        return true;
    }
}

public sealed class KDBE_EXPAND_SKILL_NOTE_PAGE_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public sbyte ExpandedMaxPageNum { get; set; }
    public sbyte RollBackMaxPageNum { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(ExpandedMaxPageNum)) return false;
        if (!ks.Put(RollBackMaxPageNum)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out sbyte v1)) return false; ExpandedMaxPageNum = v1;
        if (!ks.Get(out sbyte v2)) return false; RollBackMaxPageNum = v2;
        return true;
    }
}

public sealed class KDBE_EXPAND_SKILL_PAGE_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long UnitUid { get; set; }
    public int SkillPageNumberToBeAdded { get; set; }
    public int SPoint { get; set; }
    public int CSPoint { get; set; }
    public int[] DefaultSkill { get; set; } = new int[6];
    public sbyte UnitClass { get; set; }
    public ushort EventId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(SkillPageNumberToBeAdded)) return false;
        if (!ks.Put(SPoint)) return false;
        if (!ks.Put(CSPoint)) return false;
        for (int i = 0; i < 6; i++)
            if (!ks.Put(DefaultSkill is { Length: > 0 } arr && i < arr.Length ? arr[i] : 0)) return false;
        if (!ks.Put(UnitClass)) return false;
        if (!ks.Put(EventId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        if (!ks.Get(out int v2)) return false; SkillPageNumberToBeAdded = v2;
        if (!ks.Get(out int v3)) return false; SPoint = v3;
        if (!ks.Get(out int v4)) return false; CSPoint = v4;
        DefaultSkill = new int[6];
        for (int i = 0; i < 6; i++)
            if (!ks.Get(out DefaultSkill[i])) return false;
        if (!ks.Get(out sbyte v11)) return false; UnitClass = v11;
        if (!ks.Get(out ushort v12)) return false; EventId = v12;
        return true;
    }
}

public sealed class KDBE_EXPAND_SKILL_PAGE_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public List<int> SkillPageNumberToBeAdded { get; set; } = new();
    public int SPoint { get; set; }
    public int CSPoint { get; set; }
    public int[] DefaultSkill { get; set; } = new int[6];
    public sbyte UnitClass { get; set; }
    public ushort EventId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutList(SkillPageNumberToBeAdded, static (s, v) => s.Put(v))) return false;
        if (!ks.Put(SPoint)) return false;
        if (!ks.Put(CSPoint)) return false;
        for (int i = 0; i < 6; i++)
            if (!ks.Put(DefaultSkill is { Length: > 0 } arr && i < arr.Length ? arr[i] : 0)) return false;
        if (!ks.Put(UnitClass)) return false;
        if (!ks.Put(EventId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.GetList(SkillPageNumberToBeAdded, static s => (s.Get(out int v), v))) return false;
        if (!ks.Get(out int v2)) return false; SPoint = v2;
        if (!ks.Get(out int v3)) return false; CSPoint = v3;
        DefaultSkill = new int[6];
        for (int i = 0; i < 6; i++)
            if (!ks.Get(out DefaultSkill[i])) return false;
        if (!ks.Get(out sbyte v10)) return false; UnitClass = v10;
        if (!ks.Get(out ushort v11)) return false; EventId = v11;
        return true;
    }
}

public sealed class KDBE_EXPAND_SKILL_SLOT_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int PeriodExpire { get; set; }
    public string SkillSlotBEndDate { get; set; } = string.Empty;
    public ushort EventId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(PeriodExpire)) return false;
        if (!ks.PutWideString(SkillSlotBEndDate)) return false;
        if (!ks.Put(EventId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out int v1)) return false; PeriodExpire = v1;
        if (!ks.GetWideString(out string s2)) return false; SkillSlotBEndDate = s2;
        if (!ks.Get(out ushort v3)) return false; EventId = v3;
        return true;
    }
}

public sealed class KDBE_EXPAND_SKILL_SLOT_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public int PeriodExpire { get; set; }
    public ushort TheNumberOfSkillPagesAvailable { get; set; }
    public ushort EventId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(PeriodExpire)) return false;
        if (!ks.Put(TheNumberOfSkillPagesAvailable)) return false;
        if (!ks.Put(EventId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out int v1)) return false; PeriodExpire = v1;
        if (!ks.Get(out ushort v2)) return false; TheNumberOfSkillPagesAvailable = v2;
        if (!ks.Get(out ushort v3)) return false; EventId = v3;
        return true;
    }
}

public sealed class KDBE_EXPIRE_CASH_SKILL_POINT_ACK : IKSerializable
{
    public int Ok { get; set; }
    public List<int> RetrievedSPoint { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.PutList(RetrievedSPoint, static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.GetList(RetrievedSPoint, static s => (s.Get(out int v), v))) return false;
        return true;
    }
}

public sealed class KDBE_EXPIRE_CASH_SKILL_POINT_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public List<KRetrievedSkillPageData> RetrievedSkillPageData { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutList(RetrievedSkillPageData)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.GetList(RetrievedSkillPageData)) return false;
        return true;
    }
}

public sealed class KDBE_EXPIRE_GUILD_CASH_SKILL_POINT_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int GuildUid { get; set; }
    public int RetrievedSPoint { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(GuildUid)) return false;
        if (!ks.Put(RetrievedSPoint)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out int v1)) return false; GuildUid = v1;
        if (!ks.Get(out int v2)) return false; RetrievedSPoint = v2;
        return true;
    }
}

public sealed class KDBE_EXPIRE_GUILD_CASH_SKILL_POINT_REQ : IKSerializable
{
    public int GuildUid { get; set; }
    public int RetrievedSPoint { get; set; }
    public int GuildSPoint { get; set; }
    public List<KGuildSkillData> GuildSkillData { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(GuildUid)) return false;
        if (!ks.Put(RetrievedSPoint)) return false;
        if (!ks.Put(GuildSPoint)) return false;
        if (!ks.PutList(GuildSkillData)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; GuildUid = v0;
        if (!ks.Get(out int v1)) return false; RetrievedSPoint = v1;
        if (!ks.Get(out int v2)) return false; GuildSPoint = v2;
        if (!ks.GetList(GuildSkillData)) return false;
        return true;
    }
}

public sealed class KDBE_FIRST_WRITE_COME_BACK_INFO_NOT : IKSerializable
{
    public long UserUid { get; set; }
    public int Age { get; set; }
    public long UnitUid { get; set; }
    public int UnitClass { get; set; }
    public int UnitLevel { get; set; }
    public bool TodayFirst { get; set; }
    public int RewardLevel { get; set; }
    public string WsrtRegDate { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UserUid)) return false;
        if (!ks.Put(Age)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(UnitClass)) return false;
        if (!ks.Put(UnitLevel)) return false;
        if (!ks.Put(TodayFirst)) return false;
        if (!ks.Put(RewardLevel)) return false;
        if (!ks.PutWideString(WsrtRegDate)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UserUid = v0;
        if (!ks.Get(out int v1)) return false; Age = v1;
        if (!ks.Get(out long v2)) return false; UnitUid = v2;
        if (!ks.Get(out int v3)) return false; UnitClass = v3;
        if (!ks.Get(out int v4)) return false; UnitLevel = v4;
        if (!ks.Get(out bool v5)) return false; TodayFirst = v5;
        if (!ks.Get(out int v6)) return false; RewardLevel = v6;
        if (!ks.GetWideString(out string s7)) return false; WsrtRegDate = s7;
        return true;
    }
}

public sealed class KDBE_FIRST_WRITE_SECURITY_TYPE_NOT : IKSerializable
{
    public long UserUid { get; set; }
    public int Age { get; set; }
    public int SecurityType { get; set; }
    public string CurrTime { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UserUid)) return false;
        if (!ks.Put(Age)) return false;
        if (!ks.Put(SecurityType)) return false;
        if (!ks.PutWideString(CurrTime)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UserUid = v0;
        if (!ks.Get(out int v1)) return false; Age = v1;
        if (!ks.Get(out int v2)) return false; SecurityType = v2;
        if (!ks.GetWideString(out string s3)) return false; CurrTime = s3;
        return true;
    }
}

public sealed class KDBE_FRIEND_MESSAGE_NOT : IKSerializable
{
    public long SenderUnitUid { get; set; }
    public string ReceiverNickName { get; set; } = string.Empty;
    public sbyte MessageType { get; set; }
    public string Message { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(SenderUnitUid)) return false;
        if (!ks.PutWideString(ReceiverNickName)) return false;
        if (!ks.Put(MessageType)) return false;
        if (!ks.PutWideString(Message)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; SenderUnitUid = v0;
        if (!ks.GetWideString(out string s1)) return false; ReceiverNickName = s1;
        if (!ks.Get(out sbyte v2)) return false; MessageType = v2;
        if (!ks.GetWideString(out string s3)) return false; Message = s3;
        return true;
    }
}

public sealed class KDBE_GAME_CREATE_UNIT_REQ : IKSerializable
{
    public string NickName { get; set; } = string.Empty;
    public int Class { get; set; }
    public string RegDate { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutWideString(NickName)) return false;
        if (!ks.Put(Class)) return false;
        if (!ks.PutWideString(RegDate)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string s0)) return false; NickName = s0;
        if (!ks.Get(out int v1)) return false; Class = v1;
        if (!ks.GetWideString(out string s2)) return false; RegDate = s2;
        return true;
    }
}

public sealed class KDBE_GAME_SELECT_UNIT_EVENT_DATA_NOT : IKSerializable
{
    public long UserUid { get; set; }
    public KDBE_SELECT_UNIT_ACK SelectUnitAck { get; set; } = new();
    public Dictionary<int, KGlobalEventTableData> GlobalEventData { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.JP) && !ks.Put(UserUid)) return false;
        if ((region.Region == RegionCode.JP) && !ks.Put(SelectUnitAck)) return false;
        if ((region.Region == RegionCode.JP) && !ks.PutMap(GlobalEventData, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.JP)) { if (!ks.Get(out long v0)) return false; UserUid = v0; }
        if ((region.Region == RegionCode.JP)) { if (!ks.Get(SelectUnitAck)) return false; }
        if ((region.Region == RegionCode.JP)) { if (!ks.GetMap(GlobalEventData, static s => (s.Get(out int v), v), static s => { var v = new KGlobalEventTableData(); return (s.Get(v), v); })) return false; }
        return true;
    }
}

public sealed class KDBE_GET_ANTI_ADDICTION_INFO_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int PlayTime { get; set; }
    public int RestTime { get; set; }
    public string LastLogoutTime { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.CN) && !ks.Put(Ok)) return false;
        if ((region.Region == RegionCode.CN) && !ks.Put(PlayTime)) return false;
        if ((region.Region == RegionCode.CN) && !ks.Put(RestTime)) return false;
        if ((region.Region == RegionCode.CN) && !ks.PutWideString(LastLogoutTime)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out int v0)) return false; Ok = v0; }
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out int v1)) return false; PlayTime = v1; }
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out int v2)) return false; RestTime = v2; }
        if ((region.Region == RegionCode.CN)) { if (!ks.GetWideString(out string s3)) return false; LastLogoutTime = s3; }
        return true;
    }
}

public sealed class KDBE_GET_CREATE_UNIT_TODAY_COUNT_REQ : IKSerializable
{
    public long UserUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UserUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UserUid = v0;
        return true;
    }
}

public sealed class KDBE_GET_CUBE_IN_ITEM_MAPPING_ONOFF_ACK : IKSerializable
{
    public Dictionary<int, int> TimeControlCubeInItemMapping_StaticDBReleaseTick { get; set; } = new();
    public Dictionary<int, List<KRandomItemMappingToInfoServer>> GetCubeInItemMappingOnOff { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.JP) && !ks.PutMap(TimeControlCubeInItemMapping_StaticDBReleaseTick, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if ((region.Region == RegionCode.JP) && !ks.PutMap(GetCubeInItemMappingOnOff, static (s, k) => s.Put(k), static (s, v) => s.PutList(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.JP)) { if (!ks.GetMap(TimeControlCubeInItemMapping_StaticDBReleaseTick, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false; }
        if ((region.Region == RegionCode.JP)) { if (!ks.GetMap(GetCubeInItemMappingOnOff, static s => (s.Get(out int v), v), static s => { var l = new List<KRandomItemMappingToInfoServer>(); return (s.GetList(l), l); })) return false; }
        return true;
    }
}

public sealed class KDBE_GET_CUBE_IN_ITEM_MAPPING_ONOFF_REQ : IKSerializable
{
    public Dictionary<int, int> TimeControlCubeInItemMapping_StaticDBReleaseTick { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.JP) && !ks.PutMap(TimeControlCubeInItemMapping_StaticDBReleaseTick, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.JP)) { if (!ks.GetMap(TimeControlCubeInItemMapping_StaticDBReleaseTick, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false; }
        return true;
    }
}

public sealed class KDBE_GET_DUNGEON_EVENT_INFO_ACK : IKSerializable
{
    public List<KDungeonDropEventNpcInfo> EventInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutList(EventInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetList(EventInfo)) return false;
        return true;
    }
}

public class KDBE_GET_GLOBAL_MISSION_INFO_REQ : IKSerializable
{
    public Dictionary<string, KGlobalMissionInfo> GlobalMissionInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutMap(GlobalMissionInfo, static (s, k) => s.PutWideString(k), static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetMap(GlobalMissionInfo, static s => (s.GetWideString(out string v), v), static s => { var v = new KGlobalMissionInfo(); return (s.Get(v), v); })) return false;
        return true;
    }
}

public sealed class KDBE_GET_GUILD_BOARD_INFO_ACK : IKSerializable
{
    public List<KGuildAdInfo> GuildAdList { get; set; } = new();
    public List<KApplyJoinGuildInfo> ApplyJoinGuildList { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutList(GuildAdList)) return false;
        if (!ks.PutList(ApplyJoinGuildList)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetList(GuildAdList)) return false;
        if (!ks.GetList(ApplyJoinGuildList)) return false;
        return true;
    }
}

public sealed class KDBE_GET_GUILD_INFO_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KGuildInfo GuildInfo { get; set; } = new();
    public List<KGuildMemberInfo> MemberList { get; set; } = new();
    public KGuildMemberInfo LoginGuildMember { get; set; } = new();
    public KGuildSkillInfo GuildSkillInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(GuildInfo)) return false;
        if (!ks.PutList(MemberList)) return false;
        if (!ks.Put(LoginGuildMember)) return false;
        if (!ks.Put(GuildSkillInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(GuildInfo)) return false;
        if (!ks.GetList(MemberList)) return false;
        if (!ks.Get(LoginGuildMember)) return false;
        if (!ks.Get(GuildSkillInfo)) return false;
        return true;
    }
}

public sealed class KDBE_GET_GUILD_SKILL_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int GuildUid { get; set; }
    public long UnitUid { get; set; }
    public int GuildSkillId { get; set; }
    public int GuildSkillLevel { get; set; }
    public int GuildSkillCSPoint { get; set; }
    public int GuildSPoint { get; set; }
    public int GuildCSPoint { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(GuildUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(GuildSkillId)) return false;
        if (!ks.Put(GuildSkillLevel)) return false;
        if (!ks.Put(GuildSkillCSPoint)) return false;
        if (!ks.Put(GuildSPoint)) return false;
        if (!ks.Put(GuildCSPoint)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out int v1)) return false; GuildUid = v1;
        if (!ks.Get(out long v2)) return false; UnitUid = v2;
        if (!ks.Get(out int v3)) return false; GuildSkillId = v3;
        if (!ks.Get(out int v4)) return false; GuildSkillLevel = v4;
        if (!ks.Get(out int v5)) return false; GuildSkillCSPoint = v5;
        if (!ks.Get(out int v6)) return false; GuildSPoint = v6;
        if (!ks.Get(out int v7)) return false; GuildCSPoint = v7;
        return true;
    }
}

public sealed class KDBE_GET_GUILD_SKILL_IN_BOARD_ACK : IKSerializable
{
    public int GuildUid { get; set; }
    public long UnitUid { get; set; }
    public KGuildSkillInfo GuildSkillInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(GuildUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(GuildSkillInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; GuildUid = v0;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        if (!ks.Get(GuildSkillInfo)) return false;
        return true;
    }
}

public sealed class KDBE_GET_GUILD_SKILL_REQ : IKSerializable
{
    public int GuildUid { get; set; }
    public long UnitUid { get; set; }
    public int GuildSkillId { get; set; }
    public int GuildSkillLevel { get; set; }
    public int GuildSkillCSPoint { get; set; }
    public int GuildSPoint { get; set; }
    public int GuildCSPoint { get; set; }
    public int MaxGuildCSPoint { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(GuildUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(GuildSkillId)) return false;
        if (!ks.Put(GuildSkillLevel)) return false;
        if (!ks.Put(GuildSkillCSPoint)) return false;
        if (!ks.Put(GuildSPoint)) return false;
        if (!ks.Put(GuildCSPoint)) return false;
        if (!ks.Put(MaxGuildCSPoint)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; GuildUid = v0;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        if (!ks.Get(out int v2)) return false; GuildSkillId = v2;
        if (!ks.Get(out int v3)) return false; GuildSkillLevel = v3;
        if (!ks.Get(out int v4)) return false; GuildSkillCSPoint = v4;
        if (!ks.Get(out int v5)) return false; GuildSPoint = v5;
        if (!ks.Get(out int v6)) return false; GuildCSPoint = v6;
        if (!ks.Get(out int v7)) return false; MaxGuildCSPoint = v7;
        return true;
    }
}

public sealed class KDBE_GET_ITEM_FROM_LETTER_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long PostNo { get; set; }
    public sbyte LetterType { get; set; }
    public int WeddingUid { get; set; }
    public long WeddingItemUid { get; set; }
    public Dictionary<int, int> InsertedItem { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();
    public Dictionary<long, KItemInfo> ItemInfo { get; set; } = new();
    public int Ed { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(PostNo)) return false;
        if (!ks.Put(LetterType)) return false;
        if (!ks.Put(WeddingUid)) return false;
        if (!ks.Put(WeddingItemUid)) return false;
        if (!ks.PutMap(InsertedItem, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        if (!ks.PutMap(ItemInfo, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.Put(Ed)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out long v1)) return false; PostNo = v1;
        if (!ks.Get(out sbyte v2)) return false; LetterType = v2;
        if (!ks.Get(out int v3)) return false; WeddingUid = v3;
        if (!ks.Get(out long v4)) return false; WeddingItemUid = v4;
        if (!ks.GetMap(InsertedItem, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        if (!ks.GetMap(ItemInfo, static s => (s.Get(out long v), v), static s => { var v = new KItemInfo(); return (s.Get(v), v); })) return false;
        if (!ks.Get(out int v8)) return false; Ed = v8;
        return true;
    }
}

public sealed class KDBE_GET_ITEM_FROM_LETTER_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public long PostNo { get; set; }
    public sbyte LetterType { get; set; }
    public Dictionary<int, int> InsertedItem { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();
    public List<KItemInfo> ItemInfo { get; set; } = new();
    public int Ed { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(PostNo)) return false;
        if (!ks.Put(LetterType)) return false;
        if (!ks.PutMap(InsertedItem, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        if (!ks.PutList(ItemInfo)) return false;
        if (!ks.Put(Ed)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out long v1)) return false; PostNo = v1;
        if (!ks.Get(out sbyte v2)) return false; LetterType = v2;
        if (!ks.GetMap(InsertedItem, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        if (!ks.GetList(ItemInfo)) return false;
        if (!ks.Get(out int v6)) return false; Ed = v6;
        return true;
    }
}

public sealed class KDBE_GET_ITEM_INSERT_TO_INVENTORY_ACK : IKSerializable
{
    public int Ok { get; set; }
    public Dictionary<int, int> InsertItem { get; set; } = new();
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();
    public Dictionary<long, KItemInfo> ItemInfo { get; set; } = new();
    public KEGS_GET_ITEM_NOT BroadInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.PutMap(InsertItem, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        if (!ks.PutMap(ItemInfo, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.Put(BroadInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.GetMap(InsertItem, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        if (!ks.GetMap(ItemInfo, static s => (s.Get(out long v), v), static s => { var v = new KItemInfo(); return (s.Get(v), v); })) return false;
        if (!ks.Get(BroadInfo)) return false;
        return true;
    }
}

public sealed class KDBE_GET_ITEM_INSERT_TO_INVENTORY_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public Dictionary<int, int> InsertItem { get; set; } = new();
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();
    public List<KItemInfo> ItemInfo { get; set; } = new();
    public KEGS_GET_ITEM_NOT BroadInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutMap(InsertItem, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        if (!ks.PutList(ItemInfo)) return false;
        if (!ks.Put(BroadInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.GetMap(InsertItem, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        if (!ks.GetList(ItemInfo)) return false;
        if (!ks.Get(BroadInfo)) return false;
        return true;
    }
}

public sealed class KDBE_GET_MODULE_INFO_LIST_ACK : IKSerializable
{
    public List<KModuleInfo> ModuleList { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutList(ModuleList)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetList(ModuleList)) return false;
        return true;
    }
}

public sealed class KDBE_GET_NICKNAME_BY_UNITUID_ACK : IKSerializable
{
    public Dictionary<long, string> NickName { get; set; } = new();
    public List<KBillOrderInfo> BillOrderInfo { get; set; } = new();
    public List<KBillPackageInfo> PackageInfo { get; set; } = new();
    public List<int> SubPackageTrans { get; set; } = new();
    public List<int> SubPackageInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutMap(NickName, static (s, k) => s.Put(k), static (s, v) => s.PutWideString(v))) return false;
        if (!ks.PutList(BillOrderInfo)) return false;
        if (!ks.PutList(PackageInfo)) return false;
        if (!ks.PutList(SubPackageTrans, static (s, v) => s.Put(v))) return false;
        if (!ks.PutList(SubPackageInfo, static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetMap(NickName, static s => (s.Get(out long v), v), static s => (s.GetWideString(out string v), v))) return false;
        if (!ks.GetList(BillOrderInfo)) return false;
        if (!ks.GetList(PackageInfo)) return false;
        if (!ks.GetList(SubPackageTrans, static s => (s.Get(out int v), v))) return false;
        if (!ks.GetList(SubPackageInfo, static s => (s.Get(out int v), v))) return false;
        return true;
    }
}

public sealed class KDBE_GET_NICKNAME_BY_UNITUID_REQ : IKSerializable
{
    public List<long> Uid { get; set; } = new();
    public List<KBillOrderInfo> BillOrderInfo { get; set; } = new();
    public List<KBillPackageInfo> PackageInfo { get; set; } = new();
    public List<int> SubPackageTrans { get; set; } = new();
    public List<int> SubPackageInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutList(Uid, static (s, v) => s.Put(v))) return false;
        if (!ks.PutList(BillOrderInfo)) return false;
        if (!ks.PutList(PackageInfo)) return false;
        if (!ks.PutList(SubPackageTrans, static (s, v) => s.Put(v))) return false;
        if (!ks.PutList(SubPackageInfo, static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetList(Uid, static s => (s.Get(out long v), v))) return false;
        if (!ks.GetList(BillOrderInfo)) return false;
        if (!ks.GetList(PackageInfo)) return false;
        if (!ks.GetList(SubPackageTrans, static s => (s.Get(out int v), v))) return false;
        if (!ks.GetList(SubPackageInfo, static s => (s.Get(out int v), v))) return false;
        return true;
    }
}

public sealed class KDBE_GET_POST_LETTER_LIST_ACK : IKSerializable
{
    public int Ok { get; set; }
    public List<KPostItemInfo> PostItem { get; set; } = new();
    public List<string> BlackList { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.PutList(PostItem)) return false;
        if (!ks.PutList(BlackList, static (s, v) => s.PutWideString(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.GetList(PostItem)) return false;
        if (!ks.GetList(BlackList, static s => (s.GetWideString(out string v), v))) return false;
        return true;
    }
}

public sealed class KDBE_GET_POST_LETTER_LIST_REQ : IKSerializable
{
    public long UnitUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        return true;
    }
}

public sealed class KDBE_GET_PUNISHMENT_ACK : IKSerializable
{
    public int Ok { get; set; }
    public List<KPunishmentInfo> Punishment { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.CN) && !ks.Put(Ok)) return false;
        if ((region.Region == RegionCode.CN) && !ks.PutList(Punishment)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out int v0)) return false; Ok = v0; }
        if ((region.Region == RegionCode.CN)) { if (!ks.GetList(Punishment)) return false; }
        return true;
    }
}

public sealed class KDBE_GET_PURCHASED_CASH_ITEM_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int ItemId { get; set; }
    public int Quantity { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(ItemId)) return false;
        if (!ks.Put(Quantity)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out int v1)) return false; ItemId = v1;
        if (!ks.Get(out int v2)) return false; Quantity = v2;
        return true;
    }
}

public sealed class KDBE_GET_PURCHASED_CASH_ITEM_REQ : IKSerializable
{
    public int OrderNo { get; set; }
    public int ProductNo { get; set; }
    public int PackageProductNo { get; set; }
    public int Quantity { get; set; }
    public int ItemId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(OrderNo)) return false;
        if (!ks.Put(ProductNo)) return false;
        if (!ks.Put(PackageProductNo)) return false;
        if (!ks.Put(Quantity)) return false;
        if (!ks.Put(ItemId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; OrderNo = v0;
        if (!ks.Get(out int v1)) return false; ProductNo = v1;
        if (!ks.Get(out int v2)) return false; PackageProductNo = v2;
        if (!ks.Get(out int v3)) return false; Quantity = v3;
        if (!ks.Get(out int v4)) return false; ItemId = v4;
        return true;
    }
}

public sealed class KDBE_GET_RECOMMEND_USER_LIST_ACK : IKSerializable
{
    public int Ok { get; set; }
    public List<KRecommendUserInfo> RecommendUserList { get; set; } = new();
    public uint ViewPage { get; set; }
    public uint TotalPage { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.PutList(RecommendUserList)) return false;
        if (!ks.Put(ViewPage)) return false;
        if (!ks.Put(TotalPage)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.GetList(RecommendUserList)) return false;
        if (!ks.Get(out uint v2)) return false; ViewPage = v2;
        if (!ks.Get(out uint v3)) return false; TotalPage = v3;
        return true;
    }
}

public sealed class KDBE_GET_RECOMMEND_USER_LIST_REQ : IKSerializable
{
    public long UnitUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        return true;
    }
}

public sealed class KDBE_GET_RIDING_PET_LIST_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int AfterWorkStorageKey { get; set; }
    public List<KRidingPetInfo> RidingPetList { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(AfterWorkStorageKey)) return false;
        if (!ks.PutList(RidingPetList)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out int v1)) return false; AfterWorkStorageKey = v1;
        if (!ks.GetList(RidingPetList)) return false;
        return true;
    }
}

public sealed class KDBE_GET_RIDING_PET_LIST_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public int AfterWorkStorageKey { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(AfterWorkStorageKey)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out int v1)) return false; AfterWorkStorageKey = v1;
        return true;
    }
}

public class KDBE_GET_SECOND_SECURITY_INFO_ACK : IKSerializable
{
    public int Ok { get; set; }
    public bool UseSecondPW { get; set; }
    public string SecondPW { get; set; } = string.Empty;
    public int FailedCount { get; set; }
    public string LastAuthDate { get; set; } = string.Empty;
    public bool IsFirst { get; set; }
    public bool IsComeBackUser { get; set; }
    public int RewardStep { get; set; }
    public string LastConnectDate { get; set; } = string.Empty;
    public string ComeBackBuffEndDate { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(UseSecondPW)) return false;
        if (!ks.Put(FailedCount)) return false;
        if (!ks.PutWideString(SecondPW)) return false;
        if (!ks.PutWideString(LastAuthDate)) return false;
        if (!ks.Put(IsFirst)) return false;
        if (!ks.Put(IsComeBackUser)) return false;
        if (!ks.Put(RewardStep)) return false;
        if (!ks.PutWideString(LastConnectDate)) return false;
        if (!ks.PutWideString(ComeBackBuffEndDate)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out bool v1)) return false; UseSecondPW = v1;
        if (!ks.Get(out int v2)) return false; FailedCount = v2;
        if (!ks.GetWideString(out string s3)) return false; SecondPW = s3;
        if (!ks.GetWideString(out string s4)) return false; LastAuthDate = s4;
        if (!ks.Get(out bool v5)) return false; IsFirst = v5;
        if (!ks.Get(out bool v6)) return false; IsComeBackUser = v6;
        if (!ks.Get(out int v7)) return false; RewardStep = v7;
        if (!ks.GetWideString(out string s8)) return false; LastConnectDate = s8;
        if (!ks.GetWideString(out string s9)) return false; ComeBackBuffEndDate = s9;
        return true;
    }
}

public class KDBE_GET_SECOND_SECURITY_INFO_REQ : IKSerializable
{
    public long UserUid { get; set; }
    public Dictionary<int, int> ComeBackRewardCondition { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UserUid)) return false;
        if (!ks.PutMap(ComeBackRewardCondition, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UserUid = v0;
        if (!ks.GetMap(ComeBackRewardCondition, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false;
        return true;
    }
}

public sealed class KDBE_GET_TEMP_ITEM_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int TempItemUid { get; set; }
    public Dictionary<int, int> InsertItem { get; set; } = new();
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();
    public Dictionary<long, KItemInfo> ItemInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(TempItemUid)) return false;
        if (!ks.PutMap(InsertItem, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        if (!ks.PutMap(ItemInfo, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out int v1)) return false; TempItemUid = v1;
        if (!ks.GetMap(InsertItem, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        if (!ks.GetMap(ItemInfo, static s => (s.Get(out long v), v), static s => { var v = new KItemInfo(); return (s.Get(v), v); })) return false;
        return true;
    }
}

public sealed class KDBE_GET_TEMP_ITEM_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public int TempItemUid { get; set; }
    public Dictionary<int, int> InsertItem { get; set; } = new();
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();
    public List<KItemInfo> ItemInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(TempItemUid)) return false;
        if (!ks.PutMap(InsertItem, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        if (!ks.PutList(ItemInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out int v1)) return false; TempItemUid = v1;
        if (!ks.GetMap(InsertItem, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        if (!ks.GetList(ItemInfo)) return false;
        return true;
    }
}

public sealed class KDBE_GET_TIME_DROP_INFO_ACK : IKSerializable
{
    public List<KTimeDropInfo> TimeDropInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutList(TimeDropInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetList(TimeDropInfo)) return false;
        return true;
    }
}

public sealed class KDBE_GET_TIME_RANDOM_ITEM_EVENT_INFO_ACK : IKSerializable
{
    public List<KTimeOpenRandomItemEventInfo> TimeOpenRandomItemInfo { get; set; } = new();
    public HashSet<long> GetRewardUserUIDList { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutList(TimeOpenRandomItemInfo)) return false;
        if (!ks.PutList(GetRewardUserUIDList, static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetList(TimeOpenRandomItemInfo)) return false;
        if (!ks.GetHashSet(GetRewardUserUIDList, static s => (s.Get(out long v), v))) return false;
        return true;
    }
}

public sealed class KDBE_GET_WEB_RANKING_INFO_ACK : IKSerializable
{
    public List<KDungeonRankingInfo> DungeonRanking { get; set; } = new();
    public List<KPvpRankingInfo> PvpRanking { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutList(DungeonRanking)) return false;
        if (!ks.PutList(PvpRanking)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetList(DungeonRanking)) return false;
        if (!ks.GetList(PvpRanking)) return false;
        return true;
    }
}

public sealed class KDBE_GET_WISH_LIST_ACK : IKSerializable
{
    public Dictionary<int, int> WishList { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutMap(WishList, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetMap(WishList, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false;
        return true;
    }
}

public sealed class KDBE_GET_WISH_LIST_REQ : IKSerializable
{
    public long UserUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UserUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UserUid = v0;
        return true;
    }
}

public sealed class KDBE_GIANT_USER_LOGIN_ACK : IKSerializable
{
    public int Ok { get; set; }
    public string UserId { get; set; } = string.Empty;
    public long UserUid { get; set; }
    public string OTP { get; set; } = string.Empty;
    public uint GiantUid { get; set; }
    public uint AccountType { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.PutWideString(UserId)) return false;
        if (!ks.Put(UserUid)) return false;
        if (!ks.PutWideString(OTP)) return false;
        if (!ks.Put(GiantUid)) return false;
        if (!ks.Put(AccountType)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.GetWideString(out string s1)) return false; UserId = s1;
        if (!ks.Get(out long v2)) return false; UserUid = v2;
        if (!ks.GetWideString(out string s3)) return false; OTP = s3;
        if (!ks.Get(out uint v4)) return false; GiantUid = v4;
        if (!ks.Get(out uint v5)) return false; AccountType = v5;
        return true;
    }
}

public sealed class KDBE_GIANT_USER_LOGIN_REQ : IKSerializable
{
    public bool Login { get; set; }
    public string ServiceAccountId { get; set; } = string.Empty;
    public uint GiantUid { get; set; }
    public string OTP { get; set; } = string.Empty;
    public uint AccountType { get; set; }
    public uint AccountState { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Login)) return false;
        if (!ks.PutWideString(ServiceAccountId)) return false;
        if (!ks.Put(GiantUid)) return false;
        if (!ks.PutWideString(OTP)) return false;
        if (!ks.Put(AccountType)) return false;
        if (!ks.Put(AccountState)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool v0)) return false; Login = v0;
        if (!ks.GetWideString(out string s1)) return false; ServiceAccountId = s1;
        if (!ks.Get(out uint v2)) return false; GiantUid = v2;
        if (!ks.GetWideString(out string s3)) return false; OTP = s3;
        if (!ks.Get(out uint v4)) return false; AccountType = v4;
        if (!ks.Get(out uint v5)) return false; AccountState = v5;
        return true;
    }
}

public sealed class KDBE_GLOBAL_EVENT_TABLE_INSERT_NOT : IKSerializable
{
    public Dictionary<int, KGlobalEventTableData> GlobalEventData { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.JP) && !ks.PutMap(GlobalEventData, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.JP)) { if (!ks.GetMap(GlobalEventData, static s => (s.Get(out int v), v), static s => { var v = new KGlobalEventTableData(); return (s.Get(v), v); })) return false; }
        return true;
    }
}

public sealed class KDBE_GUILD_LEVEL_UP_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int GuildUid { get; set; }
    public string GuildName { get; set; } = string.Empty;
    public byte GuildLevel { get; set; }
    public int GuildSPoint { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(GuildUid)) return false;
        if (!ks.PutWideString(GuildName)) return false;
        if (!ks.Put(GuildLevel)) return false;
        if (!ks.Put(GuildSPoint)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out int v1)) return false; GuildUid = v1;
        if (!ks.GetWideString(out string s2)) return false; GuildName = s2;
        if (!ks.Get(out byte v3)) return false; GuildLevel = v3;
        if (!ks.Get(out int v4)) return false; GuildSPoint = v4;
        return true;
    }
}

public sealed class KDBE_GUILD_LEVEL_UP_REQ : IKSerializable
{
    public int GuildUid { get; set; }
    public string GuildName { get; set; } = string.Empty;
    public byte GuildLevel { get; set; }
    public int GuildSPoint { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(GuildUid)) return false;
        if (!ks.PutWideString(GuildName)) return false;
        if (!ks.Put(GuildLevel)) return false;
        if (!ks.Put(GuildSPoint)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; GuildUid = v0;
        if (!ks.GetWideString(out string s1)) return false; GuildName = s1;
        if (!ks.Get(out byte v2)) return false; GuildLevel = v2;
        if (!ks.Get(out int v3)) return false; GuildSPoint = v3;
        return true;
    }
}

public class KDBE_INCREASE_DEFENCE_CRYSTAL_REQ : IKSerializable
{
    public string WsrtStartTime { get; set; } = string.Empty;
    public int ProtectedCrystalCount { get; set; }
    public int TargetCrystal { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutWideString(WsrtStartTime)) return false;
        if (!ks.Put(ProtectedCrystalCount)) return false;
        if (!ks.Put(TargetCrystal)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string s0)) return false; WsrtStartTime = s0;
        if (!ks.Get(out int v1)) return false; ProtectedCrystalCount = v1;
        if (!ks.Get(out int v2)) return false; TargetCrystal = v2;
        return true;
    }
}

public sealed class KDBE_INIT_GUILD_SKILL_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int GuildUid { get; set; }
    public int GuildSPoint { get; set; }
    public int GuildCSPoint { get; set; }
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(GuildUid)) return false;
        if (!ks.Put(GuildSPoint)) return false;
        if (!ks.Put(GuildCSPoint)) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out int v1)) return false; GuildUid = v1;
        if (!ks.Get(out int v2)) return false; GuildSPoint = v2;
        if (!ks.Get(out int v3)) return false; GuildCSPoint = v3;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        return true;
    }
}

public sealed class KDBE_INIT_GUILD_SKILL_REQ : IKSerializable
{
    public int GuildUid { get; set; }
    public long UnitUid { get; set; }
    public long ItemUid { get; set; }
    public int GuildSPoint { get; set; }
    public int GuildCSPoint { get; set; }
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(GuildUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(ItemUid)) return false;
        if (!ks.Put(GuildSPoint)) return false;
        if (!ks.Put(GuildCSPoint)) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; GuildUid = v0;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        if (!ks.Get(out long v2)) return false; ItemUid = v2;
        if (!ks.Get(out int v3)) return false; GuildSPoint = v3;
        if (!ks.Get(out int v4)) return false; GuildCSPoint = v4;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        return true;
    }
}

public sealed class KDBE_INIT_SKILL_TREE_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int ActiveSkillPageNumber { get; set; }
    public long ItemUid { get; set; }
    public int SPoint { get; set; }
    public int CSPoint { get; set; }
    public int DefaultSkillID1 { get; set; }
    public int DefaultSkillID2 { get; set; }
    public int DefaultSkillID3 { get; set; }
    public int DefaultSkillID4 { get; set; }
    public int DefaultSkillID5 { get; set; }
    public int DefaultSkillID6 { get; set; }
    public int BeforeSPoint { get; set; }
    public int BeforeCSPoint { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(ActiveSkillPageNumber)) return false;
        if (!ks.Put(ItemUid)) return false;
        if (!ks.Put(SPoint)) return false;
        if (!ks.Put(CSPoint)) return false;
        if (!ks.Put(DefaultSkillID1)) return false;
        if (!ks.Put(DefaultSkillID2)) return false;
        if (!ks.Put(DefaultSkillID3)) return false;
        if (!ks.Put(DefaultSkillID4)) return false;
        if (!ks.Put(DefaultSkillID5)) return false;
        if (!ks.Put(DefaultSkillID6)) return false;
        if (!ks.Put(BeforeSPoint)) return false;
        if (!ks.Put(BeforeCSPoint)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out int v1)) return false; ActiveSkillPageNumber = v1;
        if (!ks.Get(out long v2)) return false; ItemUid = v2;
        if (!ks.Get(out int v3)) return false; SPoint = v3;
        if (!ks.Get(out int v4)) return false; CSPoint = v4;
        if (!ks.Get(out int v5)) return false; DefaultSkillID1 = v5;
        if (!ks.Get(out int v6)) return false; DefaultSkillID2 = v6;
        if (!ks.Get(out int v7)) return false; DefaultSkillID3 = v7;
        if (!ks.Get(out int v8)) return false; DefaultSkillID4 = v8;
        if (!ks.Get(out int v9)) return false; DefaultSkillID5 = v9;
        if (!ks.Get(out int v10)) return false; DefaultSkillID6 = v10;
        if (!ks.Get(out int v11)) return false; BeforeSPoint = v11;
        if (!ks.Get(out int v12)) return false; BeforeCSPoint = v12;
        return true;
    }
}

public sealed class KDBE_INIT_SKILL_TREE_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public int ActiveSkillPageNumber { get; set; }
    public long ItemUid { get; set; }
    public int SPoint { get; set; }
    public int CSPoint { get; set; }
    public int DefaultSkillID1 { get; set; }
    public int DefaultSkillID2 { get; set; }
    public int DefaultSkillID3 { get; set; }
    public int DefaultSkillID4 { get; set; }
    public int DefaultSkillID5 { get; set; }
    public int DefaultSkillID6 { get; set; }
    public int BeforeSPoint { get; set; }
    public int BeforeCSPoint { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(ActiveSkillPageNumber)) return false;
        if (!ks.Put(ItemUid)) return false;
        if (!ks.Put(SPoint)) return false;
        if (!ks.Put(CSPoint)) return false;
        if (!ks.Put(DefaultSkillID1)) return false;
        if (!ks.Put(DefaultSkillID2)) return false;
        if (!ks.Put(DefaultSkillID3)) return false;
        if (!ks.Put(DefaultSkillID4)) return false;
        if (!ks.Put(DefaultSkillID5)) return false;
        if (!ks.Put(DefaultSkillID6)) return false;
        if (!ks.Put(BeforeSPoint)) return false;
        if (!ks.Put(BeforeCSPoint)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out int v1)) return false; ActiveSkillPageNumber = v1;
        if (!ks.Get(out long v2)) return false; ItemUid = v2;
        if (!ks.Get(out int v3)) return false; SPoint = v3;
        if (!ks.Get(out int v4)) return false; CSPoint = v4;
        if (!ks.Get(out int v5)) return false; DefaultSkillID1 = v5;
        if (!ks.Get(out int v6)) return false; DefaultSkillID2 = v6;
        if (!ks.Get(out int v7)) return false; DefaultSkillID3 = v7;
        if (!ks.Get(out int v8)) return false; DefaultSkillID4 = v8;
        if (!ks.Get(out int v9)) return false; DefaultSkillID5 = v9;
        if (!ks.Get(out int v10)) return false; DefaultSkillID6 = v10;
        if (!ks.Get(out int v11)) return false; BeforeSPoint = v11;
        if (!ks.Get(out int v12)) return false; BeforeCSPoint = v12;
        return true;
    }
}

public sealed class KDBE_INSERT_ADVERTISEMENT_EVENT_INFO_NOT : IKSerializable
{
    public long UserUid { get; set; }
    public List<int> CompletedEvent { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UserUid)) return false;
        if (!ks.PutList(CompletedEvent, static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UserUid = v0;
        if (!ks.GetList(CompletedEvent, static s => (s.Get(out int v), v))) return false;
        return true;
    }
}

public sealed class KDBE_INSERT_CASH_DECREASE_ERROR_LOG_NOT : IKSerializable
{
    public long UserUid { get; set; }
    public int ResultCode { get; set; }
    public string OutString { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UserUid)) return false;
        if (!ks.Put(ResultCode)) return false;
        if (!ks.PutNarrowString(OutString)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UserUid = v0;
        if (!ks.Get(out int v1)) return false; ResultCode = v1;
        if (!ks.GetNarrowString(out string s2)) return false; OutString = s2;
        return true;
    }
}

public sealed class KDBE_INSERT_CASH_SKILL_POINT_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int CSPoint { get; set; }
    public int Period { get; set; }
    public string EndDate { get; set; } = string.Empty;
    public bool UpdateEndDateOnly { get; set; }
    public int SkillPointItemId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(CSPoint)) return false;
        if (!ks.Put(Period)) return false;
        if (!ks.PutWideString(EndDate)) return false;
        if (!ks.Put(UpdateEndDateOnly)) return false;
        if (!ks.Put(SkillPointItemId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out int v1)) return false; CSPoint = v1;
        if (!ks.Get(out int v2)) return false; Period = v2;
        if (!ks.GetWideString(out string s3)) return false; EndDate = s3;
        if (!ks.Get(out bool v4)) return false; UpdateEndDateOnly = v4;
        if (!ks.Get(out int v5)) return false; SkillPointItemId = v5;
        return true;
    }
}

public sealed class KDBE_INSERT_CASH_SKILL_POINT_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public int CSPoint { get; set; }
    public int Period { get; set; }
    public bool UpdateEndDateOnly { get; set; }
    public int SkillPointItemId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(CSPoint)) return false;
        if (!ks.Put(Period)) return false;
        if (!ks.Put(UpdateEndDateOnly)) return false;
        if (!ks.Put(SkillPointItemId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out int v1)) return false; CSPoint = v1;
        if (!ks.Get(out int v2)) return false; Period = v2;
        if (!ks.Get(out bool v3)) return false; UpdateEndDateOnly = v3;
        if (!ks.Get(out int v4)) return false; SkillPointItemId = v4;
        return true;
    }
}

public sealed class KDBE_INSERT_CONNECT_EVENT_REQ : IKSerializable
{
    public string EventName { get; set; } = string.Empty;
    public int EventType { get; set; }
    public string NextDayStartTime { get; set; } = string.Empty;
    public string NextDayEndTime { get; set; } = string.Empty;
    public int ScriptId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutWideString(EventName)) return false;
        if (!ks.Put(EventType)) return false;
        if (!ks.PutWideString(NextDayStartTime)) return false;
        if (!ks.PutWideString(NextDayEndTime)) return false;
        if (!ks.Put(ScriptId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string s0)) return false; EventName = s0;
        if (!ks.Get(out int v1)) return false; EventType = v1;
        if (!ks.GetWideString(out string s2)) return false; NextDayStartTime = s2;
        if (!ks.GetWideString(out string s3)) return false; NextDayEndTime = s3;
        if (!ks.Get(out int v4)) return false; ScriptId = v4;
        return true;
    }
}

public sealed class KDBE_INSERT_GUILD_CASH_SKILL_POINT_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int GuildUid { get; set; }
    public int GuildCSPoint { get; set; }
    public int Period { get; set; }
    public string EndDate { get; set; } = string.Empty;
    public bool UpdateEndDateOnly { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(GuildUid)) return false;
        if (!ks.Put(GuildCSPoint)) return false;
        if (!ks.Put(Period)) return false;
        if (!ks.PutWideString(EndDate)) return false;
        if (!ks.Put(UpdateEndDateOnly)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out int v1)) return false; GuildUid = v1;
        if (!ks.Get(out int v2)) return false; GuildCSPoint = v2;
        if (!ks.Get(out int v3)) return false; Period = v3;
        if (!ks.GetWideString(out string s4)) return false; EndDate = s4;
        if (!ks.Get(out bool v5)) return false; UpdateEndDateOnly = v5;
        return true;
    }
}

public sealed class KDBE_INSERT_GUILD_CASH_SKILL_POINT_REQ : IKSerializable
{
    public int GuildUid { get; set; }
    public int GuildCSPoint { get; set; }
    public int Period { get; set; }
    public bool UpdateEndDateOnly { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(GuildUid)) return false;
        if (!ks.Put(GuildCSPoint)) return false;
        if (!ks.Put(Period)) return false;
        if (!ks.Put(UpdateEndDateOnly)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; GuildUid = v0;
        if (!ks.Get(out int v1)) return false; GuildCSPoint = v1;
        if (!ks.Get(out int v2)) return false; Period = v2;
        if (!ks.Get(out bool v3)) return false; UpdateEndDateOnly = v3;
        return true;
    }
}

public sealed class KDBE_INSERT_ITEM_ACK : IKSerializable
{
    public Dictionary<int, int> InsertedItem { get; set; } = new();
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public Dictionary<long, int> ItemEnduranceUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();
    public Dictionary<long, KItemInfo> ItemInfo { get; set; } = new();
    public bool OutRoom { get; set; }
    public Dictionary<int, int> GetItem { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutMap(InsertedItem, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutMap(ItemEnduranceUpdate, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        if (!ks.PutMap(ItemInfo, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.Put(OutRoom)) return false;
        if (!ks.PutMap(GetItem, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetMap(InsertedItem, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetMap(ItemEnduranceUpdate, static s => (s.Get(out long v), v), static s => (s.Get(out int v), v))) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        if (!ks.GetMap(ItemInfo, static s => (s.Get(out long v), v), static s => { var v = new KItemInfo(); return (s.Get(v), v); })) return false;
        if (!ks.Get(out bool v5)) return false; OutRoom = v5;
        if (!ks.GetMap(GetItem, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false;
        return true;
    }
}

public sealed class KDBE_INSERT_ITEM_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public sbyte GetItemReason { get; set; }
    public Dictionary<int, int> InsertedItem { get; set; } = new();
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public Dictionary<long, int> ItemEnduranceUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();
    public List<KItemInfo> ItemInfo { get; set; } = new();
    public bool OutRoom { get; set; }
    public HashSet<int> SealCashItem { get; set; } = new();
    public Dictionary<int, int> GetItem { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(GetItemReason)) return false;
        if (!ks.PutMap(InsertedItem, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutMap(ItemEnduranceUpdate, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        if (!ks.PutList(ItemInfo)) return false;
        if (!ks.Put(OutRoom)) return false;
        if (!ks.PutList(SealCashItem, static (s, v) => s.Put(v))) return false;
        if (!ks.PutMap(GetItem, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out sbyte v1)) return false; GetItemReason = v1;
        if (!ks.GetMap(InsertedItem, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetMap(ItemEnduranceUpdate, static s => (s.Get(out long v), v), static s => (s.Get(out int v), v))) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        if (!ks.GetList(ItemInfo)) return false;
        if (!ks.Get(out bool v7)) return false; OutRoom = v7;
        if (!ks.GetHashSet(SealCashItem, static s => (s.Get(out int v), v))) return false;
        if (!ks.GetMap(GetItem, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false;
        return true;
    }
}

public sealed class KDBE_INSERT_LETTER_TO_POST_ACK : IKSerializable
{
    public int Ok { get; set; }
    public string ToNickName { get; set; } = string.Empty;
    public KPostItemInfo SendLetter { get; set; } = new();
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();
    public int SendLetterCost { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.PutWideString(ToNickName)) return false;
        if (!ks.Put(SendLetter)) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        if (!ks.Put(SendLetterCost)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.GetWideString(out string s1)) return false; ToNickName = s1;
        if (!ks.Get(SendLetter)) return false;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        if (!ks.Get(out int v5)) return false; SendLetterCost = v5;
        return true;
    }
}

public sealed class KDBE_INSERT_LETTER_TO_POST_REQ : IKSerializable
{
    public long FromUnitUid { get; set; }
    public string ToNickName { get; set; } = string.Empty;
    public KPostItemInfo SendLetter { get; set; } = new();
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();
    public int SendLetterCost { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(FromUnitUid)) return false;
        if (!ks.PutWideString(ToNickName)) return false;
        if (!ks.Put(SendLetter)) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        if (!ks.Put(SendLetterCost)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; FromUnitUid = v0;
        if (!ks.GetWideString(out string s1)) return false; ToNickName = s1;
        if (!ks.Get(SendLetter)) return false;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        if (!ks.Get(out int v5)) return false; SendLetterCost = v5;
        return true;
    }
}

public sealed class KDBE_INSERT_PERIOD_PSHOP_AGENCY_ACK : IKSerializable
{
    public int Ok { get; set; }
    public string AgencyExpirationDate { get; set; } = string.Empty;
    public ushort EventId { get; set; }
    public sbyte ShopType { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.PutWideString(AgencyExpirationDate)) return false;
        if (!ks.Put(EventId)) return false;
        if (!ks.Put(ShopType)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.GetWideString(out string s1)) return false; AgencyExpirationDate = s1;
        if (!ks.Get(out ushort v2)) return false; EventId = v2;
        if (!ks.Get(out sbyte v3)) return false; ShopType = v3;
        return true;
    }
}

public sealed class KDBE_INSERT_PERIOD_PSHOP_AGENCY_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public short AgencyPeriod { get; set; }
    public ushort EventId { get; set; }
    public sbyte ShopType { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(AgencyPeriod)) return false;
        if (!ks.Put(EventId)) return false;
        if (!ks.Put(ShopType)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out short v1)) return false; AgencyPeriod = v1;
        if (!ks.Get(out ushort v2)) return false; EventId = v2;
        if (!ks.Get(out sbyte v3)) return false; ShopType = v3;
        return true;
    }
}

public sealed class KDBE_INSERT_POST_ITEM_TO_INVENTORY_ACK : IKSerializable
{
    public int Ok { get; set; }
    public Dictionary<int, int> InsertedItem { get; set; } = new();
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();
    public Dictionary<long, KItemInfo> ItemInfo { get; set; } = new();
    public List<KPostItemInfo> PostItemInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.PutMap(InsertedItem, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        if (!ks.PutMap(ItemInfo, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.PutList(PostItemInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.GetMap(InsertedItem, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        if (!ks.GetMap(ItemInfo, static s => (s.Get(out long v), v), static s => { var v = new KItemInfo(); return (s.Get(v), v); })) return false;
        if (!ks.GetList(PostItemInfo)) return false;
        return true;
    }
}

public sealed class KDBE_INSERT_POST_ITEM_TO_INVENTORY_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public Dictionary<int, int> InsertedItem { get; set; } = new();
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();
    public List<KItemInfo> ItemInfo { get; set; } = new();
    public List<KPostItemInfo> PostItemInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutMap(InsertedItem, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        if (!ks.PutList(ItemInfo)) return false;
        if (!ks.PutList(PostItemInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.GetMap(InsertedItem, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        if (!ks.GetList(ItemInfo)) return false;
        if (!ks.GetList(PostItemInfo)) return false;
        return true;
    }
}

public sealed class KDBE_INSERT_PURCHASED_CASH_PACKAGE_ACK : IKSerializable
{
    public Dictionary<int, int> InsertedItem { get; set; } = new();
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public Dictionary<long, int> ItemEnduranceUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();
    public Dictionary<long, KItemInfo> ItemInfo { get; set; } = new();
    public uint ProductNo { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutMap(InsertedItem, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutMap(ItemEnduranceUpdate, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        if (!ks.PutMap(ItemInfo, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.Put(ProductNo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetMap(InsertedItem, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetMap(ItemEnduranceUpdate, static s => (s.Get(out long v), v), static s => (s.Get(out int v), v))) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        if (!ks.GetMap(ItemInfo, static s => (s.Get(out long v), v), static s => { var v = new KItemInfo(); return (s.Get(v), v); })) return false;
        if (!ks.Get(out uint v5)) return false; ProductNo = v5;
        return true;
    }
}

public sealed class KDBE_INSERT_PURCHASED_CASH_PACKAGE_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public Dictionary<int, int> InsertedItem { get; set; } = new();
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public Dictionary<long, int> ItemEnduranceUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();
    public List<KItemInfo> ItemInfo { get; set; } = new();
    public uint ProductNo { get; set; }
    public HashSet<int> SealCashItem { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutMap(InsertedItem, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutMap(ItemEnduranceUpdate, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        if (!ks.PutList(ItemInfo)) return false;
        if (!ks.Put(ProductNo)) return false;
        if (!ks.PutList(SealCashItem, static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.GetMap(InsertedItem, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetMap(ItemEnduranceUpdate, static s => (s.Get(out long v), v), static s => (s.Get(out int v), v))) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        if (!ks.GetList(ItemInfo)) return false;
        if (!ks.Get(out uint v6)) return false; ProductNo = v6;
        if (!ks.GetHashSet(SealCashItem, static s => (s.Get(out int v), v))) return false;
        return true;
    }
}

public sealed class KDBE_INSERT_REWARD_TO_POST_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KPostItemInfo RewardLetter { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(RewardLetter)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(RewardLetter)) return false;
        return true;
    }
}

public sealed class KDBE_INSERT_REWARD_TO_POST_REQ : IKSerializable
{
    public long FromUnitUid { get; set; }
    public long ToUnitUid { get; set; }
    public int RewardType { get; set; }
    public int RewardId { get; set; }
    public short Quantity { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool GameServerEvent { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(FromUnitUid)) return false;
        if (!ks.Put(ToUnitUid)) return false;
        if (!ks.Put(RewardType)) return false;
        if (!ks.Put(RewardId)) return false;
        if (!ks.Put(Quantity)) return false;
        if (!ks.PutWideString(Message)) return false;
        if (!ks.Put(GameServerEvent)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; FromUnitUid = v0;
        if (!ks.Get(out long v1)) return false; ToUnitUid = v1;
        if (!ks.Get(out int v2)) return false; RewardType = v2;
        if (!ks.Get(out int v3)) return false; RewardId = v3;
        if (!ks.Get(out short v4)) return false; Quantity = v4;
        if (!ks.GetWideString(out string s5)) return false; Message = s5;
        if (!ks.Get(out bool v6)) return false; GameServerEvent = v6;
        return true;
    }
}

public sealed class KDBE_INSERT_SKILL_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int ActiveSkillPageNumber { get; set; }
    public Dictionary<int, KGetSkillInfo> SkillList { get; set; } = new();
    public int CSPoint { get; set; }
    public int TotalSpendSkillPoint { get; set; }
    public int BeforeCSPoint { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(ActiveSkillPageNumber)) return false;
        if (!ks.PutMap(SkillList, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.Put(CSPoint)) return false;
        if (!ks.Put(TotalSpendSkillPoint)) return false;
        if (!ks.Put(BeforeCSPoint)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out int v1)) return false; ActiveSkillPageNumber = v1;
        if (!ks.GetMap(SkillList, static s => (s.Get(out int v), v), static s => { var v = new KGetSkillInfo(); return (s.Get(v), v); })) return false;
        if (!ks.Get(out int v3)) return false; CSPoint = v3;
        if (!ks.Get(out int v4)) return false; TotalSpendSkillPoint = v4;
        if (!ks.Get(out int v5)) return false; BeforeCSPoint = v5;
        return true;
    }
}

public sealed class KDBE_INSERT_SKILL_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public int ActiveSkillPageNumber { get; set; }
    public Dictionary<int, KGetSkillInfo> SkillList { get; set; } = new();
    public int CSPoint { get; set; }
    public int TotalSpendSkillPoint { get; set; }
    public int BeforeCSPoint { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(ActiveSkillPageNumber)) return false;
        if (!ks.PutMap(SkillList, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.Put(CSPoint)) return false;
        if (!ks.Put(TotalSpendSkillPoint)) return false;
        if (!ks.Put(BeforeCSPoint)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out int v1)) return false; ActiveSkillPageNumber = v1;
        if (!ks.GetMap(SkillList, static s => (s.Get(out int v), v), static s => { var v = new KGetSkillInfo(); return (s.Get(v), v); })) return false;
        if (!ks.Get(out int v3)) return false; CSPoint = v3;
        if (!ks.Get(out int v4)) return false; TotalSpendSkillPoint = v4;
        if (!ks.Get(out int v5)) return false; BeforeCSPoint = v5;
        return true;
    }
}

public sealed class KDBE_INSERT_TITLE_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long UnitUid { get; set; }
    public int TitleId { get; set; }
    public string EndDate { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(TitleId)) return false;
        if (!ks.PutWideString(EndDate)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        if (!ks.Get(out int v2)) return false; TitleId = v2;
        if (!ks.GetWideString(out string s3)) return false; EndDate = s3;
        return true;
    }
}

public sealed class KDBE_INSERT_TITLE_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public int TitleId { get; set; }
    public short Period { get; set; }
    public bool GameServerEvent { get; set; }
    public bool ExpandPeriod { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(TitleId)) return false;
        if (!ks.Put(Period)) return false;
        if (!ks.Put(GameServerEvent)) return false;
        if (!ks.Put(ExpandPeriod)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out int v1)) return false; TitleId = v1;
        if (!ks.Get(out short v2)) return false; Period = v2;
        if (!ks.Get(out bool v3)) return false; GameServerEvent = v3;
        if (!ks.Get(out bool v4)) return false; ExpandPeriod = v4;
        return true;
    }
}

public sealed class KDBE_INSERT_TO_INVENTORY_PICK_UP_FROM_PSHOP_AGENCY_ACK : IKSerializable
{
    public int Ok { get; set; }
    public Dictionary<int, int> InsertItem { get; set; } = new();
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();
    public Dictionary<long, KItemInfo> ItemInfo { get; set; } = new();
    public List<KSellPersonalShopItemInfo> SellItemInfo { get; set; } = new();
    public bool RemainSellItem { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.PutMap(InsertItem, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        if (!ks.PutMap(ItemInfo, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.PutList(SellItemInfo)) return false;
        if (!ks.Put(RemainSellItem)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.GetMap(InsertItem, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        if (!ks.GetMap(ItemInfo, static s => (s.Get(out long v), v), static s => { var v = new KItemInfo(); return (s.Get(v), v); })) return false;
        if (!ks.GetList(SellItemInfo)) return false;
        if (!ks.Get(out bool v6)) return false; RemainSellItem = v6;
        return true;
    }
}

public sealed class KDBE_INSERT_TO_INVENTORY_PICK_UP_FROM_PSHOP_AGENCY_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public Dictionary<int, int> InsertItem { get; set; } = new();
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();
    public List<KItemInfo> ItemInfo { get; set; } = new();
    public List<KSellPersonalShopItemInfo> SellItemInfo { get; set; } = new();
    public bool RemainSellItem { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutMap(InsertItem, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        if (!ks.PutList(ItemInfo)) return false;
        if (!ks.PutList(SellItemInfo)) return false;
        if (!ks.Put(RemainSellItem)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.GetMap(InsertItem, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        if (!ks.GetList(ItemInfo)) return false;
        if (!ks.GetList(SellItemInfo)) return false;
        if (!ks.Get(out bool v6)) return false; RemainSellItem = v6;
        return true;
    }
}

public class KDBE_INSERT_TRADE_ITEM_ACK : IKSerializable
{
    public Dictionary<int, int> InsertedItem { get; set; } = new();
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();
    public Dictionary<long, KItemInfo> ItemInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutMap(InsertedItem, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        if (!ks.PutMap(ItemInfo, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetMap(InsertedItem, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        if (!ks.GetMap(ItemInfo, static s => (s.Get(out long v), v), static s => { var v = new KItemInfo(); return (s.Get(v), v); })) return false;
        return true;
    }
}

public sealed class KDBE_INSERT_TRADE_ITEM_BY_SERVER_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public int Ed { get; set; }
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KItemInfo> ItemInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(Ed)) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutList(ItemInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out int v1)) return false; Ed = v1;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetList(ItemInfo)) return false;
        return true;
    }
}

public class KDBE_INSERT_TRADE_ITEM_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public Dictionary<int, int> InsertedItem { get; set; } = new();
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();
    public List<KItemInfo> ItemInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutMap(InsertedItem, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        if (!ks.PutList(ItemInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.GetMap(InsertedItem, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        if (!ks.GetList(ItemInfo)) return false;
        return true;
    }
}

public sealed class KDBE_INSERT_TUTORIAL_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long TeacherUid { get; set; }
    public KTutorialUnitInfo StudentUnitInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(TeacherUid)) return false;
        if (!ks.Put(StudentUnitInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out long v1)) return false; TeacherUid = v1;
        if (!ks.Get(StudentUnitInfo)) return false;
        return true;
    }
}

public sealed class KDBE_INSERT_TUTORIAL_REQ : IKSerializable
{
    public long TeacherUid { get; set; }
    public KTutorialUnitInfo StudentUnitInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(TeacherUid)) return false;
        if (!ks.Put(StudentUnitInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; TeacherUid = v0;
        if (!ks.Get(StudentUnitInfo)) return false;
        return true;
    }
}

public sealed class KDBE_INSERT_WARP_VIP_ACK : IKSerializable
{
    public int Ok { get; set; }
    public string EndDate { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.PutWideString(EndDate)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.GetWideString(out string s1)) return false; EndDate = s1;
        return true;
    }
}

public sealed class KDBE_INSERT_WARP_VIP_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public int Period { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(Period)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out int v1)) return false; Period = v1;
        return true;
    }
}

public sealed class KDBE_INSERT_WEDDING_REWARD_ACK : IKSerializable
{
    public int Ok { get; set; }
    public bool TitleReward { get; set; }
    public int TitleRewardStep { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(TitleReward)) return false;
        if (!ks.Put(TitleRewardStep)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out bool v1)) return false; TitleReward = v1;
        if (!ks.Get(out int v2)) return false; TitleRewardStep = v2;
        return true;
    }
}

public sealed class KDBE_INSERT_WEDDING_REWARD_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public long LoverUnitUid { get; set; }
    public bool TitleReward { get; set; }
    public int TitleRewardStep { get; set; }
    public KDBE_INSERT_REWARD_TO_POST_REQ PostReq { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(LoverUnitUid)) return false;
        if (!ks.Put(TitleReward)) return false;
        if (!ks.Put(TitleRewardStep)) return false;
        if (!ks.Put(PostReq)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out long v1)) return false; LoverUnitUid = v1;
        if (!ks.Get(out bool v2)) return false; TitleReward = v2;
        if (!ks.Get(out int v3)) return false; TitleRewardStep = v3;
        if (!ks.Get(PostReq)) return false;
        return true;
    }
}

public sealed class KDBE_ITEM_CONVERT_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int CommissionED { get; set; }
    public Dictionary<long, KItemInfo> ItemInfo { get; set; } = new();
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(CommissionED)) return false;
        if (!ks.PutMap(ItemInfo, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out int v1)) return false; CommissionED = v1;
        if (!ks.GetMap(ItemInfo, static s => (s.Get(out long v), v), static s => { var v = new KItemInfo(); return (s.Get(v), v); })) return false;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        return true;
    }
}

public sealed class KDBE_ITEM_CONVERT_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public int CommissionED { get; set; }
    public List<KItemInfo> ItemInfo { get; set; } = new();
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(CommissionED)) return false;
        if (!ks.PutList(ItemInfo)) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out int v1)) return false; CommissionED = v1;
        if (!ks.GetList(ItemInfo)) return false;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        return true;
    }
}

public sealed class KDBE_ITEM_EVALUATE_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long ItemUid { get; set; }
    public int Cost { get; set; }
    public List<int> RandomSocket { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(ItemUid)) return false;
        if (!ks.Put(Cost)) return false;
        if (!ks.PutList(RandomSocket, static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out long v1)) return false; ItemUid = v1;
        if (!ks.Get(out int v2)) return false; Cost = v2;
        if (!ks.GetList(RandomSocket, static s => (s.Get(out int v), v))) return false;
        return true;
    }
}

public sealed class KDBE_ITEM_EVALUATE_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public long ItemUid { get; set; }
    public int Cost { get; set; }
    public List<int> RandomSocket { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(ItemUid)) return false;
        if (!ks.Put(Cost)) return false;
        if (!ks.PutList(RandomSocket, static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out long v1)) return false; ItemUid = v1;
        if (!ks.Get(out int v2)) return false; Cost = v2;
        if (!ks.GetList(RandomSocket, static s => (s.Get(out int v), v))) return false;
        return true;
    }
}

public sealed class KDBE_ITEM_EXCHANGE_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();
    public Dictionary<long, KItemInfo> ItemInfo { get; set; } = new();
    public Dictionary<int, int> ResultItem { get; set; } = new();
    public int SourceItemId { get; set; }
    public int SourceItemQuantity { get; set; }
    public sbyte ExchangeType { get; set; }
    public int ExchangeCount { get; set; }
    public bool B12TimesRewarded { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        if (!ks.PutMap(ItemInfo, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.PutMap(ResultItem, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.Put(SourceItemId)) return false;
        if (!ks.Put(SourceItemQuantity)) return false;
        if (!ks.Put(ExchangeType)) return false;
        if (!ks.Put(ExchangeCount)) return false;
        if (!ks.Put(B12TimesRewarded)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        if (!ks.GetMap(ItemInfo, static s => (s.Get(out long v), v), static s => { var v = new KItemInfo(); return (s.Get(v), v); })) return false;
        if (!ks.GetMap(ResultItem, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false;
        if (!ks.Get(out int v5)) return false; SourceItemId = v5;
        if (!ks.Get(out int v6)) return false; SourceItemQuantity = v6;
        if (!ks.Get(out sbyte v7)) return false; ExchangeType = v7;
        if (!ks.Get(out int v8)) return false; ExchangeCount = v8;
        if (!ks.Get(out bool v9)) return false; B12TimesRewarded = v9;
        return true;
    }
}

public sealed class KDBE_ITEM_EXCHANGE_LIMIT_CHECK_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KDBE_ITEM_EXCHANGE_REQ DB { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(DB)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(DB)) return false;
        return true;
    }
}

public sealed class KDBE_ITEM_EXCHANGE_LIMIT_CHECK_REQ : IKSerializable
{
    public KEGS_ITEM_EXCHANGE_REQ Req { get; set; } = new();
    public KDBE_ITEM_EXCHANGE_REQ ToDB { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Req)) return false;
        if (!ks.Put(ToDB)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(Req)) return false;
        if (!ks.Get(ToDB)) return false;
        return true;
    }
}

public sealed class KDBE_ITEM_EXCHANGE_LOG_NOT : IKSerializable
{
    public long UserUid { get; set; }
    public long UnitUid { get; set; }
    public sbyte ExchangeType { get; set; }
    public int SourceItemId { get; set; }
    public int SourceItemQuantity { get; set; }
    public Dictionary<int, int> ResultItem { get; set; } = new();
    public string RegDate { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UserUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(ExchangeType)) return false;
        if (!ks.Put(SourceItemId)) return false;
        if (!ks.Put(SourceItemQuantity)) return false;
        if (!ks.PutMap(ResultItem, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.PutWideString(RegDate)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UserUid = v0;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        if (!ks.Get(out sbyte v2)) return false; ExchangeType = v2;
        if (!ks.Get(out int v3)) return false; SourceItemId = v3;
        if (!ks.Get(out int v4)) return false; SourceItemQuantity = v4;
        if (!ks.GetMap(ResultItem, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false;
        if (!ks.GetWideString(out string s6)) return false; RegDate = s6;
        return true;
    }
}

public sealed class KDBE_ITEM_EXCHANGE_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();
    public List<KItemInfo> ItemInfo { get; set; } = new();
    public Dictionary<int, int> ResultItem { get; set; } = new();
    public int SourceItemId { get; set; }
    public int SourceItemQuantity { get; set; }
    public sbyte ExchangeType { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        if (!ks.PutList(ItemInfo)) return false;
        if (!ks.PutMap(ResultItem, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.Put(SourceItemId)) return false;
        if (!ks.Put(SourceItemQuantity)) return false;
        if (!ks.Put(ExchangeType)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        if (!ks.GetList(ItemInfo)) return false;
        if (!ks.GetMap(ResultItem, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false;
        if (!ks.Get(out int v5)) return false; SourceItemId = v5;
        if (!ks.Get(out int v6)) return false; SourceItemQuantity = v6;
        if (!ks.Get(out sbyte v7)) return false; ExchangeType = v7;
        return true;
    }
}

public sealed class KDBE_ITEM_EXTRACT_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();
    public Dictionary<long, KItemInfo> ItemInfo { get; set; } = new();
    public Dictionary<int, int> ResultItem { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        if (!ks.PutMap(ItemInfo, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.PutMap(ResultItem, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        if (!ks.GetMap(ItemInfo, static s => (s.Get(out long v), v), static s => { var v = new KItemInfo(); return (s.Get(v), v); })) return false;
        if (!ks.GetMap(ResultItem, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false;
        return true;
    }
}

public sealed class KDBE_ITEM_EXTRACT_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();
    public List<KItemInfo> ItemInfo { get; set; } = new();
    public Dictionary<int, int> ResultItem { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        if (!ks.PutList(ItemInfo)) return false;
        if (!ks.PutMap(ResultItem, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        if (!ks.GetList(ItemInfo)) return false;
        if (!ks.GetMap(ResultItem, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false;
        return true;
    }
}

public sealed class KDBE_ITEM_MANUFACTURE_ACK : IKSerializable
{
    public int Ok { get; set; }
    public Dictionary<int, int> InsertedItem { get; set; } = new();
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();
    public Dictionary<long, KItemInfo> ItemInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.PutMap(InsertedItem, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        if (!ks.PutMap(ItemInfo, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.GetMap(InsertedItem, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        if (!ks.GetMap(ItemInfo, static s => (s.Get(out long v), v), static s => { var v = new KItemInfo(); return (s.Get(v), v); })) return false;
        return true;
    }
}

public sealed class KDBE_ITEM_MANUFACTURE_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public Dictionary<int, int> InsertedItem { get; set; } = new();
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();
    public List<KItemInfo> ItemInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutMap(InsertedItem, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        if (!ks.PutList(ItemInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.GetMap(InsertedItem, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        if (!ks.GetList(ItemInfo)) return false;
        return true;
    }
}

public sealed class KDBE_JOIN_ACCOUNT_CHECK_IP_LOG_NOT : IKSerializable
{
    public long UserUid { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string LoginIP { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UserUid)) return false;
        if (!ks.PutWideString(UserId)) return false;
        if (!ks.PutWideString(LoginIP)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UserUid = v0;
        if (!ks.GetWideString(out string s1)) return false; UserId = s1;
        if (!ks.GetWideString(out string s2)) return false; LoginIP = s2;
        return true;
    }
}

public sealed class KDBE_JOIN_GUILD_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int GuildUid { get; set; }
    public KGuildMemberInfo JoinGuildMember { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(GuildUid)) return false;
        if (!ks.Put(JoinGuildMember)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out int v1)) return false; GuildUid = v1;
        if (!ks.Get(JoinGuildMember)) return false;
        return true;
    }
}

public sealed class KDBE_JOIN_GUILD_REQ : IKSerializable
{
    public int GuildUid { get; set; }
    public KGuildMemberInfo JoinGuildMember { get; set; } = new();
    public long GuildMasterUnitUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(GuildUid)) return false;
        if (!ks.Put(JoinGuildMember)) return false;
        if (!ks.Put(GuildMasterUnitUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; GuildUid = v0;
        if (!ks.Get(JoinGuildMember)) return false;
        if (!ks.Get(out long v2)) return false; GuildMasterUnitUid = v2;
        return true;
    }
}

public sealed class KDBE_KEYBOARD_MAPPING_INFO_WRITE_REQ : IKSerializable
{
    public long UserUid { get; set; }
    public KKeyboardMappingInfo KeyboardMappingInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UserUid)) return false;
        if (!ks.Put(KeyboardMappingInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UserUid = v0;
        if (!ks.Get(KeyboardMappingInfo)) return false;
        return true;
    }
}

public sealed class KDBE_KICK_GUILD_MEMBER_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long UnitUid { get; set; }
    public int GuildUid { get; set; }
    public long TargetUnitUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(GuildUid)) return false;
        if (!ks.Put(TargetUnitUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        if (!ks.Get(out int v2)) return false; GuildUid = v2;
        if (!ks.Get(out long v3)) return false; TargetUnitUid = v3;
        return true;
    }
}

public sealed class KDBE_KICK_GUILD_MEMBER_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public int GuildUid { get; set; }
    public long TargetUnitUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(GuildUid)) return false;
        if (!ks.Put(TargetUnitUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out int v1)) return false; GuildUid = v1;
        if (!ks.Get(out long v2)) return false; TargetUnitUid = v2;
        return true;
    }
}

public sealed class KDBE_LOAD_PSHOP_AGENCY_ACK : IKSerializable
{
    public List<KERM_OPEN_PSHOP_AGENCY_BY_SERVER_NOT> OpenPShopAgency { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutList(OpenPShopAgency)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetList(OpenPShopAgency)) return false;
        return true;
    }
}

public sealed class KDBE_LOAD_WEDDING_HALL_INFO_ACK : IKSerializable
{
    public int Ok { get; set; }
    public Dictionary<int, KWeddingHallInfo> WeddingInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.PutMap(WeddingInfo, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.GetMap(WeddingInfo, static s => (s.Get(out int v), v), static s => { var v = new KWeddingHallInfo(); return (s.Get(v), v); })) return false;
        return true;
    }
}

public sealed class KDBE_MAKE_FRIEND_GROUP_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public sbyte GroupId { get; set; }
    public string GroupName { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(GroupId)) return false;
        if (!ks.PutWideString(GroupName)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out sbyte v1)) return false; GroupId = v1;
        if (!ks.GetWideString(out string s2)) return false; GroupName = s2;
        return true;
    }
}

public sealed class KDBE_MOMOTI_QUIZ_EVENT_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int CheckReward { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.JP) && !ks.Put(Ok)) return false;
        if ((region.Region == RegionCode.JP) && !ks.Put(CheckReward)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.JP)) { if (!ks.Get(out int v0)) return false; Ok = v0; }
        if ((region.Region == RegionCode.JP)) { if (!ks.Get(out int v1)) return false; CheckReward = v1; }
        return true;
    }
}

public sealed class KDBE_MOMOTI_QUIZ_EVENT_REQ : IKSerializable
{
    public int Ok { get; set; }
    public long UserUid { get; set; }
    public long UnitUid { get; set; }
    public int IstrReply { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.JP) && !ks.Put(Ok)) return false;
        if ((region.Region == RegionCode.JP) && !ks.Put(UserUid)) return false;
        if ((region.Region == RegionCode.JP) && !ks.Put(UnitUid)) return false;
        if ((region.Region == RegionCode.JP) && !ks.Put(IstrReply)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.JP)) { if (!ks.Get(out int v0)) return false; Ok = v0; }
        if ((region.Region == RegionCode.JP)) { if (!ks.Get(out long v1)) return false; UserUid = v1; }
        if ((region.Region == RegionCode.JP)) { if (!ks.Get(out long v2)) return false; UnitUid = v2; }
        if ((region.Region == RegionCode.JP)) { if (!ks.Get(out int v3)) return false; IstrReply = v3; }
        return true;
    }
}

public sealed class KDBE_MOVE_FRIEND_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public long FriendUnitUid { get; set; }
    public sbyte TargetGroupId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(FriendUnitUid)) return false;
        if (!ks.Put(TargetGroupId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out long v1)) return false; FriendUnitUid = v1;
        if (!ks.Get(out sbyte v2)) return false; TargetGroupId = v2;
        return true;
    }
}

public sealed class KDBE_NEW_ACCOUNT_QUEST_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KDBE_NEW_QUEST_REQ QuestReq { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(QuestReq)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(QuestReq)) return false;
        return true;
    }
}

public sealed class KDBE_NEW_BLACKLIST_USER_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KChatBlackListUnit ChatBlackListUnit { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(ChatBlackListUnit)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(ChatBlackListUnit)) return false;
        return true;
    }
}

public sealed class KDBE_NEW_BLACKLIST_USER_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public long BlackListUnitUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(BlackListUnitUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out long v1)) return false; BlackListUnitUid = v1;
        return true;
    }
}

public class KDBE_NEW_POST_BLACK_LIST_ACK : IKSerializable
{
    public int Ok { get; set; }
    public string NickName { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.PutWideString(NickName)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.GetWideString(out string s1)) return false; NickName = s1;
        return true;
    }
}

public class KDBE_NEW_POST_BLACK_LIST_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public string NickName { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutWideString(NickName)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.GetWideString(out string s1)) return false; NickName = s1;
        return true;
    }
}

public sealed class KDBE_NEW_QUEST_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public int QuestId { get; set; }
    public int TalkNPCId { get; set; }
    public Dictionary<int, int> InsertedItem { get; set; } = new();
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();
    public List<KItemInfo> ItemInfo { get; set; } = new();
    public long UserUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(QuestId)) return false;
        if (!ks.Put(TalkNPCId)) return false;
        if (!ks.PutMap(InsertedItem, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        if (!ks.PutList(ItemInfo)) return false;
        if (!ks.Put(UserUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out int v1)) return false; QuestId = v1;
        if (!ks.Get(out int v2)) return false; TalkNPCId = v2;
        if (!ks.GetMap(InsertedItem, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        if (!ks.GetList(ItemInfo)) return false;
        if (!ks.Get(out long v7)) return false; UserUid = v7;
        return true;
    }
}

public class KDBE_OPEN_RANDOM_ITEM_ACK : IKSerializable
{
    public int Ok { get; set; }
    public Dictionary<int, int> InsertedItem { get; set; } = new();
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();
    public Dictionary<long, KItemInfo> ItemInfo { get; set; } = new();
    public int NumResurrectionStone { get; set; }
    public int SpendED { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.PutMap(InsertedItem, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        if (!ks.PutMap(ItemInfo, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.Put(NumResurrectionStone)) return false;
        if (!ks.Put(SpendED)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.GetMap(InsertedItem, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        if (!ks.GetMap(ItemInfo, static s => (s.Get(out long v), v), static s => { var v = new KItemInfo(); return (s.Get(v), v); })) return false;
        if (!ks.Get(out int v5)) return false; NumResurrectionStone = v5;
        if (!ks.Get(out int v6)) return false; SpendED = v6;
        return true;
    }
}

public class KDBE_OPEN_RANDOM_ITEM_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public Dictionary<int, int> InsertedItem { get; set; } = new();
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();
    public List<KItemInfo> ItemInfo { get; set; } = new();
    public HashSet<int> SealRandomItem { get; set; } = new();
    public int NumResurrectionStone { get; set; }
    public int SpendED { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutMap(InsertedItem, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        if (!ks.PutList(ItemInfo)) return false;
        if (!ks.PutList(SealRandomItem, static (s, v) => s.Put(v))) return false;
        if (!ks.Put(NumResurrectionStone)) return false;
        if (!ks.Put(SpendED)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.GetMap(InsertedItem, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        if (!ks.GetList(ItemInfo)) return false;
        if (!ks.GetHashSet(SealRandomItem, static s => (s.Get(out int v), v))) return false;
        if (!ks.Get(out int v6)) return false; NumResurrectionStone = v6;
        if (!ks.Get(out int v7)) return false; SpendED = v7;
        return true;
    }
}

public sealed class KDBE_OPEN_SYNTHESIS_ITEM_ACK : IKSerializable
{
    public int Ok { get; set; }
    public Dictionary<int, int> InsertedItem { get; set; } = new();
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();
    public Dictionary<long, KItemInfo> ItemInfo { get; set; } = new();
    public Dictionary<int, long> SynthesisData { get; set; } = new();
    public long KeyData { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.JP) && !ks.Put(Ok)) return false;
        if ((region.Region == RegionCode.JP) && !ks.PutMap(InsertedItem, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if ((region.Region == RegionCode.JP) && !ks.Put(ItemQuantityUpdate)) return false;
        if ((region.Region == RegionCode.JP) && !ks.PutList(UpdatedInventorySlot)) return false;
        if ((region.Region == RegionCode.JP) && !ks.PutMap(ItemInfo, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if ((region.Region == RegionCode.JP) && !ks.PutMap(SynthesisData, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if ((region.Region == RegionCode.JP) && !ks.Put(KeyData)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.JP)) { if (!ks.Get(out int v0)) return false; Ok = v0; }
        if ((region.Region == RegionCode.JP)) { if (!ks.GetMap(InsertedItem, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false; }
        if ((region.Region == RegionCode.JP)) { if (!ks.Get(ItemQuantityUpdate)) return false; }
        if ((region.Region == RegionCode.JP)) { if (!ks.GetList(UpdatedInventorySlot)) return false; }
        if ((region.Region == RegionCode.JP)) { if (!ks.GetMap(ItemInfo, static s => (s.Get(out long v), v), static s => { var v = new KItemInfo(); return (s.Get(v), v); })) return false; }
        if ((region.Region == RegionCode.JP)) { if (!ks.GetMap(SynthesisData, static s => (s.Get(out int v), v), static s => (s.Get(out long v), v))) return false; }
        if ((region.Region == RegionCode.JP)) { if (!ks.Get(out long v6)) return false; KeyData = v6; }
        return true;
    }
}

public sealed class KDBE_OPEN_SYNTHESIS_ITEM_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public Dictionary<int, int> InsertedItem { get; set; } = new();
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();
    public List<KItemInfo> ItemInfo { get; set; } = new();
    public HashSet<int> SealCashItem { get; set; } = new();
    public Dictionary<int, long> SynthesisData { get; set; } = new();
    public long KeyData { get; set; }
    public int Level { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.JP) && !ks.Put(UnitUid)) return false;
        if ((region.Region == RegionCode.JP) && !ks.PutMap(InsertedItem, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if ((region.Region == RegionCode.JP) && !ks.Put(ItemQuantityUpdate)) return false;
        if ((region.Region == RegionCode.JP) && !ks.PutList(UpdatedInventorySlot)) return false;
        if ((region.Region == RegionCode.JP) && !ks.PutList(ItemInfo)) return false;
        if ((region.Region == RegionCode.JP) && !ks.PutList(SealCashItem, static (s, v) => s.Put(v))) return false;
        if ((region.Region == RegionCode.JP) && !ks.PutMap(SynthesisData, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if ((region.Region == RegionCode.JP) && !ks.Put(KeyData)) return false;
        if ((region.Region == RegionCode.JP) && !ks.Put(Level)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.JP)) { if (!ks.Get(out long v0)) return false; UnitUid = v0; }
        if ((region.Region == RegionCode.JP)) { if (!ks.GetMap(InsertedItem, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false; }
        if ((region.Region == RegionCode.JP)) { if (!ks.Get(ItemQuantityUpdate)) return false; }
        if ((region.Region == RegionCode.JP)) { if (!ks.GetList(UpdatedInventorySlot)) return false; }
        if ((region.Region == RegionCode.JP)) { if (!ks.GetList(ItemInfo)) return false; }
        if ((region.Region == RegionCode.JP)) { if (!ks.GetHashSet(SealCashItem, static s => (s.Get(out int v), v))) return false; }
        if ((region.Region == RegionCode.JP)) { if (!ks.GetMap(SynthesisData, static s => (s.Get(out int v), v), static s => (s.Get(out long v), v))) return false; }
        if ((region.Region == RegionCode.JP)) { if (!ks.Get(out long v7)) return false; KeyData = v7; }
        if ((region.Region == RegionCode.JP)) { if (!ks.Get(out int v8)) return false; Level = v8; }
        return true;
    }
}

public sealed class KDBE_PET_AUTO_LOOTING_NOT : IKSerializable
{
    public long PetUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PetUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; PetUid = v0;
        return true;
    }
}

public sealed class KDBE_PICK_UP_FROM_PSHOP_AGENCY_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long GsUid { get; set; }
    public long UnitUid { get; set; }
    public List<KSellPersonalShopItemInfo> PickUpItemList { get; set; } = new();
    public int SellItemTotalED { get; set; }
    public List<long> PickUpFailList { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(GsUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutList(PickUpItemList)) return false;
        if (!ks.Put(SellItemTotalED)) return false;
        if (!ks.PutList(PickUpFailList, static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out long v1)) return false; GsUid = v1;
        if (!ks.Get(out long v2)) return false; UnitUid = v2;
        if (!ks.GetList(PickUpItemList)) return false;
        if (!ks.Get(out int v4)) return false; SellItemTotalED = v4;
        if (!ks.GetList(PickUpFailList, static s => (s.Get(out long v), v))) return false;
        return true;
    }
}

public sealed class KDBE_PICK_UP_FROM_PSHOP_AGENCY_REQ : IKSerializable
{
    public long GsUid { get; set; }
    public long UnitUid { get; set; }
    public List<KSellPersonalShopItemInfo> PickUpItemList { get; set; } = new();
    public int SellItemTotalED { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(GsUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutList(PickUpItemList)) return false;
        if (!ks.Put(SellItemTotalED)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; GsUid = v0;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        if (!ks.GetList(PickUpItemList)) return false;
        if (!ks.Get(out int v3)) return false; SellItemTotalED = v3;
        return true;
    }
}

public sealed class KDBE_PREPARE_INSERT_LETTER_TO_POST_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long ItemUid { get; set; }
    public string ToNickName { get; set; } = string.Empty;
    public KPostItemInfo SendLetter { get; set; } = new();
    public int SendLetterCost { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(ItemUid)) return false;
        if (!ks.PutWideString(ToNickName)) return false;
        if (!ks.Put(SendLetter)) return false;
        if (!ks.Put(SendLetterCost)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out long v1)) return false; ItemUid = v1;
        if (!ks.GetWideString(out string s2)) return false; ToNickName = s2;
        if (!ks.Get(SendLetter)) return false;
        if (!ks.Get(out int v4)) return false; SendLetterCost = v4;
        return true;
    }
}

public sealed class KDBE_PREPARE_INSERT_LETTER_TO_POST_REQ : IKSerializable
{
    public long FromUnitUid { get; set; }
    public long ItemUid { get; set; }
    public string ToNickName { get; set; } = string.Empty;
    public KPostItemInfo SendLetter { get; set; } = new();
    public int SendLetterCost { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(FromUnitUid)) return false;
        if (!ks.Put(ItemUid)) return false;
        if (!ks.PutWideString(ToNickName)) return false;
        if (!ks.Put(SendLetter)) return false;
        if (!ks.Put(SendLetterCost)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; FromUnitUid = v0;
        if (!ks.Get(out long v1)) return false; ItemUid = v1;
        if (!ks.GetWideString(out string s2)) return false; ToNickName = s2;
        if (!ks.Get(SendLetter)) return false;
        if (!ks.Get(out int v4)) return false; SendLetterCost = v4;
        return true;
    }
}

public sealed class KDBE_PREPARE_REG_PSHOP_AGENCY_ITEM_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();
    public string PersonalShopName { get; set; } = string.Empty;
    public List<KSellPersonalShopItemInfo> SellItemInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        if (!ks.PutWideString(PersonalShopName)) return false;
        if (!ks.PutList(SellItemInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        if (!ks.GetWideString(out string s3)) return false; PersonalShopName = s3;
        if (!ks.GetList(SellItemInfo)) return false;
        return true;
    }
}

public sealed class KDBE_PREPARE_REG_PSHOP_AGENCY_ITEM_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();
    public string PersonalShopName { get; set; } = string.Empty;
    public List<KSellPersonalShopItemInfo> SellItemInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        if (!ks.PutWideString(PersonalShopName)) return false;
        if (!ks.PutList(SellItemInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        if (!ks.GetWideString(out string s3)) return false; PersonalShopName = s3;
        if (!ks.GetList(SellItemInfo)) return false;
        return true;
    }
}

public sealed class KDBE_PRESENT_CASH_ITEM_CHECK_NICKNAME_ACK : IKSerializable
{
    public int Ok { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public List<KNXBTPurchaseReqInfo> PurchaseReqInfo { get; set; } = new();
    public long ReceiverUserUid { get; set; }
    public long ReceiverUnitUid { get; set; }
    public string ReceiverNickName { get; set; } = string.Empty;
    public sbyte ReceiverLevel { get; set; }
    public int ReceiverUnitClass { get; set; }
    public int UseCashType { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.PutWideString(UserId)) return false;
        if (!ks.PutWideString(Message)) return false;
        if (!ks.PutList(PurchaseReqInfo)) return false;
        if (!ks.Put(ReceiverUserUid)) return false;
        if (!ks.Put(ReceiverUnitUid)) return false;
        if (!ks.PutWideString(ReceiverNickName)) return false;
        if (!ks.Put(ReceiverLevel)) return false;
        if (!ks.Put(ReceiverUnitClass)) return false;
        if (!ks.Put(UseCashType)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.GetWideString(out string s1)) return false; UserId = s1;
        if (!ks.GetWideString(out string s2)) return false; Message = s2;
        if (!ks.GetList(PurchaseReqInfo)) return false;
        if (!ks.Get(out long v4)) return false; ReceiverUserUid = v4;
        if (!ks.Get(out long v5)) return false; ReceiverUnitUid = v5;
        if (!ks.GetWideString(out string s6)) return false; ReceiverNickName = s6;
        if (!ks.Get(out sbyte v7)) return false; ReceiverLevel = v7;
        if (!ks.Get(out int v8)) return false; ReceiverUnitClass = v8;
        if (!ks.Get(out int v9)) return false; UseCashType = v9;
        return true;
    }
}

public sealed class KDBE_PRESENT_CASH_ITEM_CHECK_NICKNAME_REQ : IKSerializable
{
    public string ReceiverNickName { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public List<KNXBTPurchaseReqInfo> PurchaseReqInfo { get; set; } = new();
    public int UseCashType { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutWideString(ReceiverNickName)) return false;
        if (!ks.PutWideString(Message)) return false;
        if (!ks.PutList(PurchaseReqInfo)) return false;
        if (!ks.Put(UseCashType)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string s0)) return false; ReceiverNickName = s0;
        if (!ks.GetWideString(out string s1)) return false; Message = s1;
        if (!ks.GetList(PurchaseReqInfo)) return false;
        if (!ks.Get(out int v3)) return false; UseCashType = v3;
        return true;
    }
}

public sealed class KDBE_PVP_MATCH_RESULT_LOG_NOT : IKSerializable
{
    public sbyte MatchType { get; set; }
    public sbyte NpcPvpType { get; set; }
    public string RegDate { get; set; } = string.Empty;
    public int PlayTime { get; set; }
    public List<KPvpMatchUserResultLog> PvpMatchUserLog { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(MatchType)) return false;
        if (!ks.Put(NpcPvpType)) return false;
        if (!ks.PutWideString(RegDate)) return false;
        if (!ks.Put(PlayTime)) return false;
        if (!ks.PutList(PvpMatchUserLog)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte v0)) return false; MatchType = v0;
        if (!ks.Get(out sbyte v1)) return false; NpcPvpType = v1;
        if (!ks.GetWideString(out string s2)) return false; RegDate = s2;
        if (!ks.Get(out int v3)) return false; PlayTime = v3;
        if (!ks.GetList(PvpMatchUserLog)) return false;
        return true;
    }
}

public sealed class KDBE_QUEST_COMPLETE_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KCompleteQuestInfo CompleteQuestInfo { get; set; } = new();
    public bool IsChangeJob { get; set; }
    public sbyte ChangeUnitClass { get; set; }
    public bool IsNew { get; set; }
    public Dictionary<int, KItemInfo> InsertedItem { get; set; } = new();
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();
    public Dictionary<long, KItemInfo> ItemInfo { get; set; } = new();
    public bool AutoComplete { get; set; }
    public int NewDefaultSkill1 { get; set; }
    public int NewDefaultSkill2 { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(CompleteQuestInfo)) return false;
        if (!ks.Put(IsChangeJob)) return false;
        if (!ks.Put(ChangeUnitClass)) return false;
        if (!ks.Put(IsNew)) return false;
        if (!ks.PutMap(InsertedItem, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        if (!ks.PutMap(ItemInfo, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.Put(AutoComplete)) return false;
        if (!ks.Put(NewDefaultSkill1)) return false;
        if (!ks.Put(NewDefaultSkill2)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(CompleteQuestInfo)) return false;
        if (!ks.Get(out bool v2)) return false; IsChangeJob = v2;
        if (!ks.Get(out sbyte v3)) return false; ChangeUnitClass = v3;
        if (!ks.Get(out bool v4)) return false; IsNew = v4;
        if (!ks.GetMap(InsertedItem, static s => (s.Get(out int v), v), static s => { var v = new KItemInfo(); return (s.Get(v), v); })) return false;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        if (!ks.GetMap(ItemInfo, static s => (s.Get(out long v), v), static s => { var v = new KItemInfo(); return (s.Get(v), v); })) return false;
        if (!ks.Get(out bool v9)) return false; AutoComplete = v9;
        if (!ks.Get(out int v10)) return false; NewDefaultSkill1 = v10;
        if (!ks.Get(out int v11)) return false; NewDefaultSkill2 = v11;
        return true;
    }
}

public sealed class KDBE_QUEST_COMPLETE_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public int QuestId { get; set; }
    public bool IsRepeat { get; set; }
    public bool IsChangeJob { get; set; }
    public sbyte ChangeUnitClass { get; set; }
    public bool IsNew { get; set; }
    public Dictionary<int, KItemInfo> InsertedItem { get; set; } = new();
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();
    public List<KItemInfo> ItemInfo { get; set; } = new();
    public bool AutoComplete { get; set; }
    public int NewDefaultSkill1 { get; set; }
    public int NewDefaultSkill2 { get; set; }
    public int TheNumberOfSkillPagesAvailable { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(QuestId)) return false;
        if (!ks.Put(IsRepeat)) return false;
        if (!ks.Put(IsChangeJob)) return false;
        if (!ks.Put(ChangeUnitClass)) return false;
        if (!ks.Put(IsNew)) return false;
        if (!ks.PutMap(InsertedItem, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        if (!ks.PutList(ItemInfo)) return false;
        if (!ks.Put(AutoComplete)) return false;
        if (!ks.Put(NewDefaultSkill1)) return false;
        if (!ks.Put(NewDefaultSkill2)) return false;
        if (!ks.Put(TheNumberOfSkillPagesAvailable)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out int v1)) return false; QuestId = v1;
        if (!ks.Get(out bool v2)) return false; IsRepeat = v2;
        if (!ks.Get(out bool v3)) return false; IsChangeJob = v3;
        if (!ks.Get(out sbyte v4)) return false; ChangeUnitClass = v4;
        if (!ks.Get(out bool v5)) return false; IsNew = v5;
        if (!ks.GetMap(InsertedItem, static s => (s.Get(out int v), v), static s => { var v = new KItemInfo(); return (s.Get(v), v); })) return false;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        if (!ks.GetList(ItemInfo)) return false;
        if (!ks.Get(out bool v10)) return false; AutoComplete = v10;
        if (!ks.Get(out int v11)) return false; NewDefaultSkill1 = v11;
        if (!ks.Get(out int v12)) return false; NewDefaultSkill2 = v12;
        if (!ks.Get(out int v13)) return false; TheNumberOfSkillPagesAvailable = v13;
        return true;
    }
}

public sealed class KDBE_QUIT_USER_PVP_RESULT_UPDATE_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public int OfficialMatchCnt { get; set; }
    public int Rating { get; set; }
    public int MaxRating { get; set; }
    public int RPoint { get; set; }
    public int APoint { get; set; }
    public bool IsWinBeforeMatch { get; set; }
    public int EmblemEnum { get; set; }
    public double KFactor { get; set; }
    public int Win { get; set; }
    public int Lose { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(OfficialMatchCnt)) return false;
        if (!ks.Put(Rating)) return false;
        if (!ks.Put(MaxRating)) return false;
        if (!ks.Put(RPoint)) return false;
        if (!ks.Put(APoint)) return false;
        if (!ks.Put(IsWinBeforeMatch)) return false;
        if (!ks.Put(EmblemEnum)) return false;
        if (!ks.Put(KFactor)) return false;
        if (!ks.Put(Win)) return false;
        if (!ks.Put(Lose)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out int v1)) return false; OfficialMatchCnt = v1;
        if (!ks.Get(out int v2)) return false; Rating = v2;
        if (!ks.Get(out int v3)) return false; MaxRating = v3;
        if (!ks.Get(out int v4)) return false; RPoint = v4;
        if (!ks.Get(out int v5)) return false; APoint = v5;
        if (!ks.Get(out bool v6)) return false; IsWinBeforeMatch = v6;
        if (!ks.Get(out int v7)) return false; EmblemEnum = v7;
        if (!ks.Get(out double v8)) return false; KFactor = v8;
        if (!ks.Get(out int v9)) return false; Win = v9;
        if (!ks.Get(out int v10)) return false; Lose = v10;
        return true;
    }
}

public sealed class KDBE_READY_TO_SOSUN_EVENT_ACK : IKSerializable
{
    public int Ok { get; set; }
    public Dictionary<int, int> InsertedItem { get; set; } = new();
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();
    public Dictionary<long, KItemInfo> ItemInfo { get; set; } = new();
    public int FirstUnitClass { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.CN) && !ks.Put(Ok)) return false;
        if ((region.Region == RegionCode.CN) && !ks.PutMap(InsertedItem, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if ((region.Region == RegionCode.CN) && !ks.Put(ItemQuantityUpdate)) return false;
        if ((region.Region == RegionCode.CN) && !ks.PutList(UpdatedInventorySlot)) return false;
        if ((region.Region == RegionCode.CN) && !ks.PutMap(ItemInfo, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if ((region.Region == RegionCode.CN) && !ks.Put(FirstUnitClass)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out int v0)) return false; Ok = v0; }
        if ((region.Region == RegionCode.CN)) { if (!ks.GetMap(InsertedItem, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false; }
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(ItemQuantityUpdate)) return false; }
        if ((region.Region == RegionCode.CN)) { if (!ks.GetList(UpdatedInventorySlot)) return false; }
        if ((region.Region == RegionCode.CN)) { if (!ks.GetMap(ItemInfo, static s => (s.Get(out long v), v), static s => { var v = new KItemInfo(); return (s.Get(v), v); })) return false; }
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out int v5)) return false; FirstUnitClass = v5; }
        return true;
    }
}

public sealed class KDBE_READY_TO_SOSUN_EVENT_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public Dictionary<int, int> InsertedItem { get; set; } = new();
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();
    public List<KItemInfo> ItemInfo { get; set; } = new();
    public int FirstUnitClass { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.CN) && !ks.Put(UnitUid)) return false;
        if ((region.Region == RegionCode.CN) && !ks.PutMap(InsertedItem, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if ((region.Region == RegionCode.CN) && !ks.Put(ItemQuantityUpdate)) return false;
        if ((region.Region == RegionCode.CN) && !ks.PutList(UpdatedInventorySlot)) return false;
        if ((region.Region == RegionCode.CN) && !ks.PutList(ItemInfo)) return false;
        if ((region.Region == RegionCode.CN) && !ks.Put(FirstUnitClass)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out long v0)) return false; UnitUid = v0; }
        if ((region.Region == RegionCode.CN)) { if (!ks.GetMap(InsertedItem, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false; }
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(ItemQuantityUpdate)) return false; }
        if ((region.Region == RegionCode.CN)) { if (!ks.GetList(UpdatedInventorySlot)) return false; }
        if ((region.Region == RegionCode.CN)) { if (!ks.GetList(ItemInfo)) return false; }
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out int v5)) return false; FirstUnitClass = v5; }
        return true;
    }
}

public sealed class KDBE_READ_LETTER_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public long PostNo { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(PostNo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out long v1)) return false; PostNo = v1;
        return true;
    }
}

public sealed class KDBE_REALTIME_RESURRECTION_CASHSTONE_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public int NumResurrection_CashStone { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.CN) && !ks.Put(UnitUid)) return false;
        if ((region.Region == RegionCode.CN) && !ks.Put(NumResurrection_CashStone)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out long v0)) return false; UnitUid = v0; }
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out int v1)) return false; NumResurrection_CashStone = v1; }
        return true;
    }
}

public sealed class KDBE_RECOMMEND_USER_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long RecommendedUnitUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(RecommendedUnitUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out long v1)) return false; RecommendedUnitUid = v1;
        return true;
    }
}

public sealed class KDBE_RECOMMEND_USER_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public string NickName { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutWideString(NickName)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.GetWideString(out string s1)) return false; NickName = s1;
        return true;
    }
}

public class KDBE_REGISTRATION_GUILD_AD_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long UnitUid { get; set; }
    public int GuildUid { get; set; }
    public short Period { get; set; }
    public string AdRegDate { get; set; } = string.Empty;
    public string AdEndDate { get; set; } = string.Empty;
    public string AdMessage { get; set; } = string.Empty;
    public int Cost { get; set; }
    public bool ExistExpiredAd { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(GuildUid)) return false;
        if (!ks.Put(Period)) return false;
        if (!ks.PutWideString(AdRegDate)) return false;
        if (!ks.PutWideString(AdEndDate)) return false;
        if (!ks.PutWideString(AdMessage)) return false;
        if (!ks.Put(Cost)) return false;
        if (!ks.Put(ExistExpiredAd)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        if (!ks.Get(out int v2)) return false; GuildUid = v2;
        if (!ks.Get(out short v3)) return false; Period = v3;
        if (!ks.GetWideString(out string s4)) return false; AdRegDate = s4;
        if (!ks.GetWideString(out string s5)) return false; AdEndDate = s5;
        if (!ks.GetWideString(out string s6)) return false; AdMessage = s6;
        if (!ks.Get(out int v7)) return false; Cost = v7;
        if (!ks.Get(out bool v8)) return false; ExistExpiredAd = v8;
        return true;
    }
}

public sealed class KDBE_REGISTRATION_GUILD_AD_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public int GuildUid { get; set; }
    public short Period { get; set; }
    public string AdMessage { get; set; } = string.Empty;
    public int Cost { get; set; }
    public bool ExistExpiredAd { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(GuildUid)) return false;
        if (!ks.Put(Period)) return false;
        if (!ks.PutWideString(AdMessage)) return false;
        if (!ks.Put(Cost)) return false;
        if (!ks.Put(ExistExpiredAd)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out int v1)) return false; GuildUid = v1;
        if (!ks.Get(out short v2)) return false; Period = v2;
        if (!ks.GetWideString(out string s3)) return false; AdMessage = s3;
        if (!ks.Get(out int v4)) return false; Cost = v4;
        if (!ks.Get(out bool v5)) return false; ExistExpiredAd = v5;
        return true;
    }
}

public sealed class KDBE_REGIST_GLOBAL_MISSION_CLEAR_NOT : IKSerializable
{
    public string GlobalMissionDay { get; set; } = string.Empty;
    public long UnitUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutWideString(GlobalMissionDay)) return false;
        if (!ks.Put(UnitUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string s0)) return false; GlobalMissionDay = s0;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        return true;
    }
}

public sealed class KDBE_REG_ABUSER_AUTOMATIC_NOT : IKSerializable
{
    public Dictionary<int, long> RegAbuserInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutMap(RegAbuserInfo, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetMap(RegAbuserInfo, static s => (s.Get(out int v), v), static s => (s.Get(out long v), v))) return false;
        return true;
    }
}

public sealed class KDBE_REG_DUNGEON_CLEAR_TIME_HACK_USER_NOT : IKSerializable
{
    public long UserUid { get; set; }
    public string RegDate { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UserUid)) return false;
        if (!ks.PutWideString(RegDate)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UserUid = v0;
        if (!ks.GetWideString(out string s1)) return false; RegDate = s1;
        return true;
    }
}

public sealed class KDBE_REG_SKILL_NOTE_MEMO_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public sbyte PageNum { get; set; }
    public int MemoId { get; set; }
    public List<KInventoryItemInfo> InventorySlotInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(PageNum)) return false;
        if (!ks.Put(MemoId)) return false;
        if (!ks.PutList(InventorySlotInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out sbyte v1)) return false; PageNum = v1;
        if (!ks.Get(out int v2)) return false; MemoId = v2;
        if (!ks.GetList(InventorySlotInfo)) return false;
        return true;
    }
}

public sealed class KDBE_RELEASE_PET_REQ : IKSerializable
{
    public long PetUid { get; set; }
    public long UnitUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PetUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; PetUid = v0;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        return true;
    }
}

public sealed class KDBE_RELEASE_RIDING_PET_REQ : IKSerializable
{
    public long RidingPetUid { get; set; }
    public long UnitUid { get; set; }
    public bool EventQuest { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(RidingPetUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        if ((region.Region == RegionCode.CN) && !ks.Put(EventQuest)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; RidingPetUid = v0;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out bool v2)) return false; EventQuest = v2; }
        return true;
    }
}

public sealed class KDBE_RENAME_FRIEND_GROUP_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public sbyte GroupId { get; set; }
    public string GroupName { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(GroupId)) return false;
        if (!ks.PutWideString(GroupName)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out sbyte v1)) return false; GroupId = v1;
        if (!ks.GetWideString(out string s2)) return false; GroupName = s2;
        return true;
    }
}

public sealed class KDBE_REQUEST_FRIEND_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long UnitUid { get; set; }
    public string NickName { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutWideString(NickName)) return false;
        if (!ks.PutWideString(Message)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        if (!ks.GetWideString(out string s2)) return false; NickName = s2;
        if (!ks.GetWideString(out string s3)) return false; Message = s3;
        return true;
    }
}

public sealed class KDBE_REQUEST_FRIEND_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public string NickName { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutWideString(NickName)) return false;
        if (!ks.PutWideString(Message)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.GetWideString(out string s1)) return false; NickName = s1;
        if (!ks.GetWideString(out string s2)) return false; Message = s2;
        return true;
    }
}

public sealed class KDBE_RESET_GUILD_SKILL_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int GuildUid { get; set; }
    public int GuildSkillId { get; set; }
    public int GuildSkillLevel { get; set; }
    public int GuildSkillCSPoint { get; set; }
    public int GuildSPoint { get; set; }
    public int GuildCSPoint { get; set; }
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(GuildUid)) return false;
        if (!ks.Put(GuildSkillId)) return false;
        if (!ks.Put(GuildSkillLevel)) return false;
        if (!ks.Put(GuildSkillCSPoint)) return false;
        if (!ks.Put(GuildSPoint)) return false;
        if (!ks.Put(GuildCSPoint)) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out int v1)) return false; GuildUid = v1;
        if (!ks.Get(out int v2)) return false; GuildSkillId = v2;
        if (!ks.Get(out int v3)) return false; GuildSkillLevel = v3;
        if (!ks.Get(out int v4)) return false; GuildSkillCSPoint = v4;
        if (!ks.Get(out int v5)) return false; GuildSPoint = v5;
        if (!ks.Get(out int v6)) return false; GuildCSPoint = v6;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        return true;
    }
}

public sealed class KDBE_RESET_GUILD_SKILL_REQ : IKSerializable
{
    public int GuildUid { get; set; }
    public long UnitUid { get; set; }
    public int GuildSkillId { get; set; }
    public int GuildSkillLevel { get; set; }
    public int GuildSkillCSPoint { get; set; }
    public int GuildSPoint { get; set; }
    public int GuildCSPoint { get; set; }
    public int MaxGuildCSPoint { get; set; }
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(GuildUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(GuildSkillId)) return false;
        if (!ks.Put(GuildSkillLevel)) return false;
        if (!ks.Put(GuildSkillCSPoint)) return false;
        if (!ks.Put(GuildSPoint)) return false;
        if (!ks.Put(GuildCSPoint)) return false;
        if (!ks.Put(MaxGuildCSPoint)) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; GuildUid = v0;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        if (!ks.Get(out int v2)) return false; GuildSkillId = v2;
        if (!ks.Get(out int v3)) return false; GuildSkillLevel = v3;
        if (!ks.Get(out int v4)) return false; GuildSkillCSPoint = v4;
        if (!ks.Get(out int v5)) return false; GuildSPoint = v5;
        if (!ks.Get(out int v6)) return false; GuildCSPoint = v6;
        if (!ks.Get(out int v7)) return false; MaxGuildCSPoint = v7;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        return true;
    }
}

public sealed class KDBE_RESET_PERIOD_ITEM_ACK : IKSerializable
{
    public int Ok { get; set; }
    public List<KItemPeriodInfo> ItemPeriodInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.PutList(ItemPeriodInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.GetList(ItemPeriodInfo)) return false;
        return true;
    }
}

public sealed class KDBE_RESET_PERIOD_ITEM_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public Dictionary<long, short> ExpandPeriodItemList { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutMap(ExpandPeriodItemList, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.GetMap(ExpandPeriodItemList, static s => (s.Get(out long v), v), static s => (s.Get(out short v), v))) return false;
        return true;
    }
}

public sealed class KDBE_RESET_SKILL_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int ActiveSkillPageNumber { get; set; }
    public int DelSkillId { get; set; }
    public int DelSkillLevel { get; set; }
    public int CSPoint { get; set; }
    public int BeforeSPoint { get; set; }
    public int BeforeCSPoint { get; set; }
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(ActiveSkillPageNumber)) return false;
        if (!ks.Put(DelSkillId)) return false;
        if (!ks.Put(DelSkillLevel)) return false;
        if (!ks.Put(CSPoint)) return false;
        if (!ks.Put(BeforeSPoint)) return false;
        if (!ks.Put(BeforeCSPoint)) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out int v1)) return false; ActiveSkillPageNumber = v1;
        if (!ks.Get(out int v2)) return false; DelSkillId = v2;
        if (!ks.Get(out int v3)) return false; DelSkillLevel = v3;
        if (!ks.Get(out int v4)) return false; CSPoint = v4;
        if (!ks.Get(out int v5)) return false; BeforeSPoint = v5;
        if (!ks.Get(out int v6)) return false; BeforeCSPoint = v6;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        return true;
    }
}

public sealed class KDBE_RESET_SKILL_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public int ActiveSkillPageNumber { get; set; }
    public int DelSkillId { get; set; }
    public int DelSkillLevel { get; set; }
    public int CSPoint { get; set; }
    public int BeforeSPoint { get; set; }
    public int BeforeCSPoint { get; set; }
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(ActiveSkillPageNumber)) return false;
        if (!ks.Put(DelSkillId)) return false;
        if (!ks.Put(DelSkillLevel)) return false;
        if (!ks.Put(CSPoint)) return false;
        if (!ks.Put(BeforeSPoint)) return false;
        if (!ks.Put(BeforeCSPoint)) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out int v1)) return false; ActiveSkillPageNumber = v1;
        if (!ks.Get(out int v2)) return false; DelSkillId = v2;
        if (!ks.Get(out int v3)) return false; DelSkillLevel = v3;
        if (!ks.Get(out int v4)) return false; CSPoint = v4;
        if (!ks.Get(out int v5)) return false; BeforeSPoint = v5;
        if (!ks.Get(out int v6)) return false; BeforeCSPoint = v6;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        return true;
    }
}

public sealed class KDBE_RESOLVE_ITEM_ACK : IKSerializable
{
    public int Ok { get; set; }
    public bool Jackpot { get; set; }
    public Dictionary<int, int> InsertedItem { get; set; } = new();
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();
    public Dictionary<long, KItemInfo> ItemInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(Jackpot)) return false;
        if (!ks.PutMap(InsertedItem, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        if (!ks.PutMap(ItemInfo, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out bool v1)) return false; Jackpot = v1;
        if (!ks.GetMap(InsertedItem, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        if (!ks.GetMap(ItemInfo, static s => (s.Get(out long v), v), static s => { var v = new KItemInfo(); return (s.Get(v), v); })) return false;
        return true;
    }
}

public sealed class KDBE_RESOLVE_ITEM_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public bool Jackpot { get; set; }
    public Dictionary<int, int> InsertedItem { get; set; } = new();
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();
    public List<KItemInfo> ItemInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(Jackpot)) return false;
        if (!ks.PutMap(InsertedItem, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        if (!ks.PutList(ItemInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out bool v1)) return false; Jackpot = v1;
        if (!ks.GetMap(InsertedItem, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        if (!ks.GetList(ItemInfo)) return false;
        return true;
    }
}

public sealed class KDBE_RESTORE_ITEM_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long ItemUid { get; set; }
    public int LevelAfterEnchant { get; set; }
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(ItemUid)) return false;
        if (!ks.Put(LevelAfterEnchant)) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out long v1)) return false; ItemUid = v1;
        if (!ks.Get(out int v2)) return false; LevelAfterEnchant = v2;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        return true;
    }
}

public sealed class KDBE_RESTORE_ITEM_EVALUATE_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long TargetItemUid { get; set; }
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(TargetItemUid)) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out long v1)) return false; TargetItemUid = v1;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        return true;
    }
}

public sealed class KDBE_RESTORE_ITEM_EVALUATE_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public long TargetItemUid { get; set; }
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(TargetItemUid)) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out long v1)) return false; TargetItemUid = v1;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        return true;
    }
}

public sealed class KDBE_RESTORE_ITEM_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public long ItemUid { get; set; }
    public int LevelAfterEnchant { get; set; }
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(ItemUid)) return false;
        if (!ks.Put(LevelAfterEnchant)) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out long v1)) return false; ItemUid = v1;
        if (!ks.Get(out int v2)) return false; LevelAfterEnchant = v2;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        return true;
    }
}

public sealed class KDBE_RESURRECTION_CASHSTONE_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public int NumResurrection_CashStone { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.CN) && !ks.Put(UnitUid)) return false;
        if ((region.Region == RegionCode.CN) && !ks.Put(NumResurrection_CashStone)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out long v0)) return false; UnitUid = v0; }
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out int v1)) return false; NumResurrection_CashStone = v1; }
        return true;
    }
}

public sealed class KDBE_ROLLBACK_PURCHASED_CASH_ITEM_NOT : IKSerializable
{
    public int OrderNo { get; set; }
    public int ProductNo { get; set; }
    public string Msg { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(OrderNo)) return false;
        if (!ks.Put(ProductNo)) return false;
        if (!ks.PutWideString(Msg)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; OrderNo = v0;
        if (!ks.Get(out int v1)) return false; ProductNo = v1;
        if (!ks.GetWideString(out string s2)) return false; Msg = s2;
        return true;
    }
}

public class KDBE_SEAL_ITEM_ACK : IKSerializable
{
    public int Ok { get; set; }
    public byte SealResult { get; set; }
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(SealResult)) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out byte v1)) return false; SealResult = v1;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        return true;
    }
}

public class KDBE_SEAL_ITEM_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public long ItemUid { get; set; }
    public byte SealResult { get; set; }
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(ItemUid)) return false;
        if (!ks.Put(SealResult)) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out long v1)) return false; ItemUid = v1;
        if (!ks.Get(out byte v2)) return false; SealResult = v2;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        return true;
    }
}

public sealed class KDBE_SELECT_UNIT_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KUnitInfo UnitInfo { get; set; } = new();
    public string UnitCreateDate { get; set; } = string.Empty;
    public int RealDataED { get; set; }
    public string UnitLastLoginDate { get; set; } = string.Empty;
    public Dictionary<int, int> InventorySlotSize { get; set; } = new();
    public Dictionary<long, KInventoryItemInfo> Item { get; set; } = new();
    public Dictionary<long, Dictionary<long, KInventoryItemInfo>> PetItem { get; set; } = new();
    public int NumResurrectionStone { get; set; }
    public int NumAutoPaymentResStone { get; set; }
    public string AutoPayResStoneLastDate { get; set; } = string.Empty;
    public List<KQuestInstance> Quest { get; set; } = new();
    public List<KCompleteQuestInfo> CompletQuest { get; set; } = new();
    public List<KMissionInstance> Mission { get; set; } = new();
    public List<KTitleInfo> Title { get; set; } = new();
    public List<KUserSkillPageData> UserSkillPageData { get; set; } = new();
    public List<short> SkillUnsealed { get; set; } = new();
    public sbyte SkillNoteMaxPageNum { get; set; }
    public Dictionary<sbyte, int> SkillNote { get; set; } = new();
    public List<KChatBlackListUnit> ChatBlackList { get; set; } = new();
    public KDenyOptions DenyOptions { get; set; } = new();
    public bool IsRecommend { get; set; }
    public long RecommendUnitUid { get; set; }
    public List<KTutorialDBUnitInfo> TutorialDBUnitInfo { get; set; } = new();
    public bool IsSpiritUpdated { get; set; }
    public List<KConnectTimeEventInfo> ConnectTimeEvent { get; set; } = new();
    public List<KCumulativeTimeEventInfo> CumulativeTimeEvent { get; set; } = new();
    public List<KHenirRankingInfo> HenirRanking { get; set; } = new();
    public KMessengerInfo MessengerInfo { get; set; } = new();
    public long SummonedPetUid { get; set; }
    public List<KPetInfo> PetList { get; set; } = new();
    public List<long> CanEvolutionPetList { get; set; } = new();
    public List<long> NeverSummonPetList { get; set; } = new();
    public Dictionary<int, int> CharGameCount { get; set; } = new();
    public KUserPShopAgencyInfo PShopAgencyInfo { get; set; } = new();
    public KEGS_HENIR_REWARD_COUNT_NOT PacketHenirRewardCount { get; set; } = new();
    public Dictionary<int, int> RandomQuestList { get; set; } = new();
    public bool CashShopOpen { get; set; }
    public KGamePlayStatusContainer GamePlayStatus { get; set; } = new();
    public List<KQuestInstance> AccountQuesting { get; set; } = new();
    public List<KCompleteQuestInfo> AccountCompleteQuest { get; set; } = new();
    public bool FirstSelect { get; set; }
    public List<int> AdvertisementEvent { get; set; } = new();
    public KRelationshipInfo RelationshipInfo { get; set; } = new();
    public long WrongUnitUid { get; set; }
    public bool Couple { get; set; }
    public long RelationTargetUserUid { get; set; }
    public string RelationTargetUserNickname { get; set; } = string.Empty;
    public int NewUnitE { get; set; }
    public Dictionary<int, KGlobalEventTableData> GlobalEventData { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(UnitInfo)) return false;
        if (!ks.PutWideString(UnitCreateDate)) return false;
        if (!ks.Put(RealDataED)) return false;
        if (!ks.PutWideString(UnitLastLoginDate)) return false;
        if (!ks.PutMap(InventorySlotSize, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.PutMap(Item, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.PutMap(PetItem, static (s, k) => s.Put(k), static (s, v) => s.PutMap(v, static (s2, k2) => s2.Put(k2), static (s2, v2) => s2.Put(v2)))) return false;
        if (!ks.Put(NumResurrectionStone)) return false;
        if (!ks.Put(NumAutoPaymentResStone)) return false;
        if (!ks.PutWideString(AutoPayResStoneLastDate)) return false;
        if (!ks.PutList(Quest)) return false;
        if (!ks.PutList(CompletQuest)) return false;
        if (!ks.PutList(Mission)) return false;
        if (!ks.PutList(Title)) return false;
        if (!ks.PutList(UserSkillPageData)) return false;
        if (!ks.PutList(SkillUnsealed, static (s, v) => s.Put(v))) return false;
        if (!ks.Put(SkillNoteMaxPageNum)) return false;
        if (!ks.PutMap(SkillNote, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.PutList(ChatBlackList)) return false;
        if (!ks.Put(DenyOptions)) return false;
        if (!ks.Put(IsRecommend)) return false;
        if (!ks.Put(RecommendUnitUid)) return false;
        if (!ks.PutList(TutorialDBUnitInfo)) return false;
        if (!ks.Put(IsSpiritUpdated)) return false;
        if (!ks.PutList(ConnectTimeEvent)) return false;
        if (!ks.PutList(CumulativeTimeEvent)) return false;
        if (!ks.PutList(HenirRanking)) return false;
        if (!ks.Put(MessengerInfo)) return false;
        if (!ks.Put(SummonedPetUid)) return false;
        if (!ks.PutList(PetList)) return false;
        if (!ks.PutList(CanEvolutionPetList, static (s, v) => s.Put(v))) return false;
        if (!ks.PutList(NeverSummonPetList, static (s, v) => s.Put(v))) return false;
        if (!ks.PutMap(CharGameCount, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.Put(PShopAgencyInfo)) return false;
        if (!ks.Put(PacketHenirRewardCount)) return false;
        if (!ks.PutMap(RandomQuestList, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.Put(CashShopOpen)) return false;
        if (!ks.Put(GamePlayStatus)) return false;
        if (!ks.PutList(AccountQuesting)) return false;
        if (!ks.PutList(AccountCompleteQuest)) return false;
        if (!ks.Put(FirstSelect)) return false;
        if (!ks.PutList(AdvertisementEvent, static (s, v) => s.Put(v))) return false;
        if (!ks.Put(RelationshipInfo)) return false;
        if (!ks.Put(WrongUnitUid)) return false;
        if ((region.Region == RegionCode.CN) && !ks.Put(Couple)) return false;
        if ((region.Region == RegionCode.CN) && !ks.Put(RelationTargetUserUid)) return false;
        if ((region.Region == RegionCode.CN) && !ks.PutWideString(RelationTargetUserNickname)) return false;
        if ((region.Region == RegionCode.JP) && !ks.Put(NewUnitE)) return false;
        if ((region.Region == RegionCode.JP) && !ks.PutMap(GlobalEventData, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(UnitInfo)) return false;
        if (!ks.GetWideString(out string s2)) return false; UnitCreateDate = s2;
        if (!ks.Get(out int v3)) return false; RealDataED = v3;
        if (!ks.GetWideString(out string s4)) return false; UnitLastLoginDate = s4;
        if (!ks.GetMap(InventorySlotSize, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false;
        if (!ks.GetMap(Item, static s => (s.Get(out long v), v), static s => { var v = new KInventoryItemInfo(); return (s.Get(v), v); })) return false;
        if (!ks.GetMap(PetItem, static s => (s.Get(out long v), v), static s => { var d = new Dictionary<long, KInventoryItemInfo>(); return (s.GetMap(d, static s2 => (s2.Get(out long v), v), static s2 => { var v = new KInventoryItemInfo(); return (s2.Get(v), v); }), d); })) return false;
        if (!ks.Get(out int v8)) return false; NumResurrectionStone = v8;
        if (!ks.Get(out int v9)) return false; NumAutoPaymentResStone = v9;
        if (!ks.GetWideString(out string s10)) return false; AutoPayResStoneLastDate = s10;
        if (!ks.GetList(Quest)) return false;
        if (!ks.GetList(CompletQuest)) return false;
        if (!ks.GetList(Mission)) return false;
        if (!ks.GetList(Title)) return false;
        if (!ks.GetList(UserSkillPageData)) return false;
        if (!ks.GetList(SkillUnsealed, static s => (s.Get(out short v), v))) return false;
        if (!ks.Get(out sbyte v17)) return false; SkillNoteMaxPageNum = v17;
        if (!ks.GetMap(SkillNote, static s => (s.Get(out sbyte v), v), static s => (s.Get(out int v), v))) return false;
        if (!ks.GetList(ChatBlackList)) return false;
        if (!ks.Get(DenyOptions)) return false;
        if (!ks.Get(out bool v21)) return false; IsRecommend = v21;
        if (!ks.Get(out long v22)) return false; RecommendUnitUid = v22;
        if (!ks.GetList(TutorialDBUnitInfo)) return false;
        if (!ks.Get(out bool v24)) return false; IsSpiritUpdated = v24;
        if (!ks.GetList(ConnectTimeEvent)) return false;
        if (!ks.GetList(CumulativeTimeEvent)) return false;
        if (!ks.GetList(HenirRanking)) return false;
        if (!ks.Get(MessengerInfo)) return false;
        if (!ks.Get(out long v29)) return false; SummonedPetUid = v29;
        if (!ks.GetList(PetList)) return false;
        if (!ks.GetList(CanEvolutionPetList, static s => (s.Get(out long v), v))) return false;
        if (!ks.GetList(NeverSummonPetList, static s => (s.Get(out long v), v))) return false;
        if (!ks.GetMap(CharGameCount, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false;
        if (!ks.Get(PShopAgencyInfo)) return false;
        if (!ks.Get(PacketHenirRewardCount)) return false;
        if (!ks.GetMap(RandomQuestList, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false;
        if (!ks.Get(out bool v37)) return false; CashShopOpen = v37;
        if (!ks.Get(GamePlayStatus)) return false;
        if (!ks.GetList(AccountQuesting)) return false;
        if (!ks.GetList(AccountCompleteQuest)) return false;
        if (!ks.Get(out bool v41)) return false; FirstSelect = v41;
        if (!ks.GetList(AdvertisementEvent, static s => (s.Get(out int v), v))) return false;
        if (!ks.Get(RelationshipInfo)) return false;
        if (!ks.Get(out long v44)) return false; WrongUnitUid = v44;
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out bool v45)) return false; Couple = v45; }
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out long v46)) return false; RelationTargetUserUid = v46; }
        if ((region.Region == RegionCode.CN)) { if (!ks.GetWideString(out string s47)) return false; RelationTargetUserNickname = s47; }
        if ((region.Region == RegionCode.JP)) { if (!ks.Get(out int v48)) return false; NewUnitE = v48; }
        if ((region.Region == RegionCode.JP)) { if (!ks.GetMap(GlobalEventData, static s => (s.Get(out int v), v), static s => { var v = new KGlobalEventTableData(); return (s.Get(v), v); })) return false; }
        return true;
    }
}

public sealed class KDBE_SEUCCESS_SECOND_SECURITY_ACK : IKSerializable
{
    public int Ok { get; set; }
    public string CurrTime { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.PutWideString(CurrTime)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.GetWideString(out string s1)) return false; CurrTime = s1;
        return true;
    }
}

public sealed class KDBE_SEUCCESS_SECOND_SECURITY_REQ : IKSerializable
{
    public long UserId { get; set; }
    public string CurrTime { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UserId)) return false;
        if (!ks.PutWideString(CurrTime)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UserId = v0;
        if (!ks.GetWideString(out string s1)) return false; CurrTime = s1;
        return true;
    }
}

public sealed class KDBE_SHARING_BACK_OPEN_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int OpenType { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(OpenType)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out int v1)) return false; OpenType = v1;
        return true;
    }
}

public sealed class KDBE_SHARING_BACK_OPEN_REQ : IKSerializable
{
    // enum SHARING_BANK_OPEN_TYPE
    public const int SBOT_QUEST = 1;
    public const int SBOT_CASH = 2;

    public long UserUid { get; set; }
    public long UnitUid { get; set; }
    public int OpenType { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UserUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(OpenType)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UserUid = v0;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        if (!ks.Get(out int v2)) return false; OpenType = v2;
        return true;
    }
}

public sealed class KDBE_SHARING_BANK_EVENT_REQ : IKSerializable
{
    public long UserUid { get; set; }
    public long UnitUid { get; set; }
    public int ItemId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.CN) && !ks.Put(UserUid)) return false;
        if ((region.Region == RegionCode.CN) && !ks.Put(UnitUid)) return false;
        if ((region.Region == RegionCode.CN) && !ks.Put(ItemId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out long v0)) return false; UserUid = v0; }
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out long v1)) return false; UnitUid = v1; }
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out int v2)) return false; ItemId = v2; }
        return true;
    }
}

public sealed class KDBE_SOCKET_EXPAND_ITEM_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long ItemUid { get; set; }
    public byte ExpandedSocketNum { get; set; }
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(ItemUid)) return false;
        if (!ks.Put(ExpandedSocketNum)) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out long v1)) return false; ItemUid = v1;
        if (!ks.Get(out byte v2)) return false; ExpandedSocketNum = v2;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        return true;
    }
}

public sealed class KDBE_SOCKET_EXPAND_ITEM_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public long ItemUid { get; set; }
    public List<int> ItemSocket { get; set; } = new();
    public byte ExpandedSocketNum { get; set; }
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(ItemUid)) return false;
        if (!ks.PutList(ItemSocket, static (s, v) => s.Put(v))) return false;
        if (!ks.Put(ExpandedSocketNum)) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out long v1)) return false; ItemUid = v1;
        if (!ks.GetList(ItemSocket, static s => (s.Get(out int v), v))) return false;
        if (!ks.Get(out byte v3)) return false; ExpandedSocketNum = v3;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        return true;
    }
}

public sealed class KDBE_SOCKET_ITEM_ACK : IKSerializable
{
    public int Ok { get; set; }
    public List<int> SocketInfo { get; set; } = new();
    public long ItemUid { get; set; }
    public int ItemId { get; set; }
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();
    public int SocketUseCount { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.PutList(SocketInfo, static (s, v) => s.Put(v))) return false;
        if (!ks.Put(ItemUid)) return false;
        if (!ks.Put(ItemId)) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        if (!ks.Put(SocketUseCount)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.GetList(SocketInfo, static s => (s.Get(out int v), v))) return false;
        if (!ks.Get(out long v2)) return false; ItemUid = v2;
        if (!ks.Get(out int v3)) return false; ItemId = v3;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        if (!ks.Get(out int v6)) return false; SocketUseCount = v6;
        return true;
    }
}

public sealed class KDBE_SOCKET_ITEM_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public List<int> SocketInfo { get; set; } = new();
    public long ItemUid { get; set; }
    public int ItemId { get; set; }
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();
    public int SocketUseCount { get; set; }
    public byte ExpandedSocketNum { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutList(SocketInfo, static (s, v) => s.Put(v))) return false;
        if (!ks.Put(ItemUid)) return false;
        if (!ks.Put(ItemId)) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        if (!ks.Put(SocketUseCount)) return false;
        if (!ks.Put(ExpandedSocketNum)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.GetList(SocketInfo, static s => (s.Get(out int v), v))) return false;
        if (!ks.Get(out long v2)) return false; ItemUid = v2;
        if (!ks.Get(out int v3)) return false; ItemId = v3;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        if (!ks.Get(out int v6)) return false; SocketUseCount = v6;
        if (!ks.Get(out byte v7)) return false; ExpandedSocketNum = v7;
        return true;
    }
}

public sealed class KDBE_SPIRIT_STATISTICS_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public int SpiritPlayTime { get; set; }
    public int DurationTime { get; set; }
    public int UsedSpirit { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(SpiritPlayTime)) return false;
        if (!ks.Put(DurationTime)) return false;
        if (!ks.Put(UsedSpirit)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out int v1)) return false; SpiritPlayTime = v1;
        if (!ks.Get(out int v2)) return false; DurationTime = v2;
        if (!ks.Get(out int v3)) return false; UsedSpirit = v3;
        return true;
    }
}

public sealed class KDBE_START_CHECK_POWER_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public byte CheckPowerCount { get; set; }
    public long CheckPowerTime { get; set; }
    public byte CheckPowerScore { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.JP || region.Region == RegionCode.TW) && !ks.Put(UnitUid)) return false;
        if ((region.Region == RegionCode.JP || region.Region == RegionCode.TW) && !ks.Put(CheckPowerCount)) return false;
        if ((region.Region == RegionCode.JP || region.Region == RegionCode.TW) && !ks.Put(CheckPowerTime)) return false;
        if ((region.Region == RegionCode.JP || region.Region == RegionCode.TW) && !ks.Put(CheckPowerScore)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.JP || region.Region == RegionCode.TW)) { if (!ks.Get(out long v0)) return false; UnitUid = v0; }
        if ((region.Region == RegionCode.JP || region.Region == RegionCode.TW)) { if (!ks.Get(out byte v1)) return false; CheckPowerCount = v1; }
        if ((region.Region == RegionCode.JP || region.Region == RegionCode.TW)) { if (!ks.Get(out long v2)) return false; CheckPowerTime = v2; }
        if ((region.Region == RegionCode.JP || region.Region == RegionCode.TW)) { if (!ks.Get(out byte v3)) return false; CheckPowerScore = v3; }
        return true;
    }
}

public sealed class KDBE_STOP_SALE_PSHOP_AGENCY_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long GsUid { get; set; }
    public long UnitUid { get; set; }
    public int Reason { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(GsUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(Reason)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out long v1)) return false; GsUid = v1;
        if (!ks.Get(out long v2)) return false; UnitUid = v2;
        if (!ks.Get(out int v3)) return false; Reason = v3;
        return true;
    }
}

public sealed class KDBE_STOP_SALE_PSHOP_AGENCY_REQ : IKSerializable
{
    public long GsUid { get; set; }
    public long UnitUid { get; set; }
    public int Reason { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(GsUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(Reason)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; GsUid = v0;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        if (!ks.Get(out int v2)) return false; Reason = v2;
        return true;
    }
}

public sealed class KDBE_SUMMON_PET_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long BeforeSummonPetUid { get; set; }
    public long SummonPetUid { get; set; }
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public Dictionary<long, int> ItemEnduranceUpdate { get; set; } = new();
    public Dictionary<long, KItemPosition> ItemPositionUpdate { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(BeforeSummonPetUid)) return false;
        if (!ks.Put(SummonPetUid)) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutMap(ItemEnduranceUpdate, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.PutMap(ItemPositionUpdate, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out long v1)) return false; BeforeSummonPetUid = v1;
        if (!ks.Get(out long v2)) return false; SummonPetUid = v2;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetMap(ItemEnduranceUpdate, static s => (s.Get(out long v), v), static s => (s.Get(out int v), v))) return false;
        if (!ks.GetMap(ItemPositionUpdate, static s => (s.Get(out long v), v), static s => { var v = new KItemPosition(); return (s.Get(v), v); })) return false;
        return true;
    }
}

public sealed class KDBE_SUMMON_PET_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public long BeforeSummonPetUid { get; set; }
    public long SummonPetUid { get; set; }
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public Dictionary<long, int> ItemEnduranceUpdate { get; set; } = new();
    public Dictionary<long, KItemPosition> ItemPositionUpdate { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(BeforeSummonPetUid)) return false;
        if (!ks.Put(SummonPetUid)) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutMap(ItemEnduranceUpdate, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.PutMap(ItemPositionUpdate, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out long v1)) return false; BeforeSummonPetUid = v1;
        if (!ks.Get(out long v2)) return false; SummonPetUid = v2;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetMap(ItemEnduranceUpdate, static s => (s.Get(out long v), v), static s => (s.Get(out int v), v))) return false;
        if (!ks.GetMap(ItemPositionUpdate, static s => (s.Get(out long v), v), static s => { var v = new KItemPosition(); return (s.Get(v), v); })) return false;
        return true;
    }
}

public sealed class KDBE_SYNC_ED_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KDBE_SYNC_ED_REQ Info { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(Info)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(Info)) return false;
        return true;
    }
}

public sealed class KDBE_SYNC_ED_REQ : IKSerializable
{
    public long FromUnitUid { get; set; }
    public bool IsGmMessage { get; set; }
    public string ToNickName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public int Ed { get; set; }
    public long ItemUid { get; set; }
    public int Quantity { get; set; }
    public int ChangeED { get; set; }
    public int SendLetterCost { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(FromUnitUid)) return false;
        if (!ks.Put(IsGmMessage)) return false;
        if (!ks.PutWideString(ToNickName)) return false;
        if (!ks.PutWideString(Title)) return false;
        if (!ks.PutWideString(Message)) return false;
        if (!ks.Put(Ed)) return false;
        if (!ks.Put(ItemUid)) return false;
        if (!ks.Put(Quantity)) return false;
        if (!ks.Put(ChangeED)) return false;
        if (!ks.Put(SendLetterCost)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; FromUnitUid = v0;
        if (!ks.Get(out bool v1)) return false; IsGmMessage = v1;
        if (!ks.GetWideString(out string s2)) return false; ToNickName = s2;
        if (!ks.GetWideString(out string s3)) return false; Title = s3;
        if (!ks.GetWideString(out string s4)) return false; Message = s4;
        if (!ks.Get(out int v5)) return false; Ed = v5;
        if (!ks.Get(out long v6)) return false; ItemUid = v6;
        if (!ks.Get(out int v7)) return false; Quantity = v7;
        if (!ks.Get(out int v8)) return false; ChangeED = v8;
        if (!ks.Get(out int v9)) return false; SendLetterCost = v9;
        return true;
    }
}

public sealed class KDBE_SYNTHESIS_LOG_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public int Level { get; set; }
    public long KeyUid { get; set; }
    public Dictionary<int, long> SynthesisData { get; set; } = new();
    public List<KItemInfo> ItemInfo { get; set; } = new();
    public Dictionary<long, KItemInfo> InsertedItemInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.JP) && !ks.Put(UnitUid)) return false;
        if ((region.Region == RegionCode.JP) && !ks.Put(Level)) return false;
        if ((region.Region == RegionCode.JP) && !ks.Put(KeyUid)) return false;
        if ((region.Region == RegionCode.JP) && !ks.PutMap(SynthesisData, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if ((region.Region == RegionCode.JP) && !ks.PutList(ItemInfo)) return false;
        if ((region.Region == RegionCode.JP) && !ks.PutMap(InsertedItemInfo, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.JP)) { if (!ks.Get(out long v0)) return false; UnitUid = v0; }
        if ((region.Region == RegionCode.JP)) { if (!ks.Get(out int v1)) return false; Level = v1; }
        if ((region.Region == RegionCode.JP)) { if (!ks.Get(out long v2)) return false; KeyUid = v2; }
        if ((region.Region == RegionCode.JP)) { if (!ks.GetMap(SynthesisData, static s => (s.Get(out int v), v), static s => (s.Get(out long v), v))) return false; }
        if ((region.Region == RegionCode.JP)) { if (!ks.GetList(ItemInfo)) return false; }
        if ((region.Region == RegionCode.JP)) { if (!ks.GetMap(InsertedItemInfo, static s => (s.Get(out long v), v), static s => { var v = new KItemInfo(); return (s.Get(v), v); })) return false; }
        return true;
    }
}

public sealed class KDBE_TIME_ENCHANT_EVENT_INFO_ACK : IKSerializable
{
    public List<KTimeEnchantEventInfo> TimeEnchantEventInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutList(TimeEnchantEventInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetList(TimeEnchantEventInfo)) return false;
        return true;
    }
}

public sealed class KDBE_TIME_ENCHANT_EVENT_INFO_NOT : IKSerializable
{
    public int TimeEnchantEventUid { get; set; }
    public KTimeEnchantEventInfo TimeEnchantEventInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(TimeEnchantEventUid)) return false;
        if (!ks.Put(TimeEnchantEventInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; TimeEnchantEventUid = v0;
        if (!ks.Get(TimeEnchantEventInfo)) return false;
        return true;
    }
}

public sealed class KDBE_TIME_ENCHANT_EVENT_INFO_UPDATE_ACK : IKSerializable
{
    public long GsUid { get; set; }
    public long UnitUid { get; set; }
    public int AfterWorkStorageKey { get; set; }
    public ushort EventId { get; set; }
    public sbyte EnchantLevel { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(GsUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(AfterWorkStorageKey)) return false;
        if (!ks.Put(EventId)) return false;
        if (!ks.Put(EnchantLevel)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; GsUid = v0;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        if (!ks.Get(out int v2)) return false; AfterWorkStorageKey = v2;
        if (!ks.Get(out ushort v3)) return false; EventId = v3;
        if (!ks.Get(out sbyte v4)) return false; EnchantLevel = v4;
        return true;
    }
}

public sealed class KDBE_TIME_ENCHANT_EVENT_INFO_UPDATE_REQ : IKSerializable
{
    public long GsUid { get; set; }
    public long UnitUid { get; set; }
    public int TimeEnchantEventUid { get; set; }
    public KTimeEnchantEventInfo TimeEnchantEventInfo { get; set; } = new();
    public int AfterWorkStorageKey { get; set; }
    public ushort EventId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(GsUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(TimeEnchantEventUid)) return false;
        if (!ks.Put(TimeEnchantEventInfo)) return false;
        if (!ks.Put(AfterWorkStorageKey)) return false;
        if (!ks.Put(EventId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; GsUid = v0;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        if (!ks.Get(out int v2)) return false; TimeEnchantEventUid = v2;
        if (!ks.Get(TimeEnchantEventInfo)) return false;
        if (!ks.Get(out int v4)) return false; AfterWorkStorageKey = v4;
        if (!ks.Get(out ushort v5)) return false; EventId = v5;
        return true;
    }
}

public sealed class KDBE_TRADE_COMPLETE_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long UnitUid { get; set; }
    public int Ed { get; set; }
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> InventorySlotInfo { get; set; } = new();
    public Dictionary<long, int> UpdateFailed { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(Ed)) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutList(InventorySlotInfo)) return false;
        if (!ks.PutMap(UpdateFailed, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        if (!ks.Get(out int v2)) return false; Ed = v2;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetList(InventorySlotInfo)) return false;
        if (!ks.GetMap(UpdateFailed, static s => (s.Get(out long v), v), static s => (s.Get(out int v), v))) return false;
        return true;
    }
}

public sealed class KDBE_TRADE_COMPLETE_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public int Ed { get; set; }
    public KItemQuantityUpdate QuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> InventorySlotInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(Ed)) return false;
        if (!ks.Put(QuantityUpdate)) return false;
        if (!ks.PutList(InventorySlotInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out int v1)) return false; Ed = v1;
        if (!ks.Get(QuantityUpdate)) return false;
        if (!ks.GetList(InventorySlotInfo)) return false;
        return true;
    }
}

public sealed class KDBE_UDP_CHECK_KICK_USER_NOT : IKSerializable
{
    public long RoomUid { get; set; }
    public int PVPChannelClass { get; set; }
    public int MatchType { get; set; }
    public long UnitUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(RoomUid)) return false;
        if (!ks.Put(PVPChannelClass)) return false;
        if (!ks.Put(MatchType)) return false;
        if (!ks.Put(UnitUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; RoomUid = v0;
        if (!ks.Get(out int v1)) return false; PVPChannelClass = v1;
        if (!ks.Get(out int v2)) return false; MatchType = v2;
        if (!ks.Get(out long v3)) return false; UnitUid = v3;
        return true;
    }
}

public sealed class KDBE_UNBLOCK_FRIEND_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public long FriendUnitUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(FriendUnitUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out long v1)) return false; FriendUnitUid = v1;
        return true;
    }
}

public sealed class KDBE_UNLIMITED_SECOND_CHANGE_JOB_NOT : IKSerializable
{
    public int UnitUid { get; set; }
    public int OldUnitClass { get; set; }
    public int NewUnitClass { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(OldUnitClass)) return false;
        if (!ks.Put(NewUnitClass)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; UnitUid = v0;
        if (!ks.Get(out int v1)) return false; OldUnitClass = v1;
        if (!ks.Get(out int v2)) return false; NewUnitClass = v2;
        return true;
    }
}

public sealed class KDBE_UNSEAL_SKILL_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int SkillId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(SkillId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out int v1)) return false; SkillId = v1;
        return true;
    }
}

public sealed class KDBE_UNSEAL_SKILL_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public int SkillId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(SkillId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out int v1)) return false; SkillId = v1;
        return true;
    }
}

public sealed class KDBE_UPDATE_ANTI_ADDICTION_INFO : IKSerializable
{
    public long UserUid { get; set; }
    public int PlayTime { get; set; }
    public int RestTime { get; set; }
    public string LastLogoutTime { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.CN) && !ks.Put(UserUid)) return false;
        if ((region.Region == RegionCode.CN) && !ks.Put(PlayTime)) return false;
        if ((region.Region == RegionCode.CN) && !ks.Put(RestTime)) return false;
        if ((region.Region == RegionCode.CN) && !ks.PutWideString(LastLogoutTime)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out long v0)) return false; UserUid = v0; }
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out int v1)) return false; PlayTime = v1; }
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out int v2)) return false; RestTime = v2; }
        if ((region.Region == RegionCode.CN)) { if (!ks.GetWideString(out string s3)) return false; LastLogoutTime = s3; }
        return true;
    }
}

public sealed class KDBE_UPDATE_CCU_INFO_NOT : IKSerializable
{
    public string RegDate { get; set; } = string.Empty;
    public List<KUpdateCCUInfo> CCUInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutWideString(RegDate)) return false;
        if (!ks.PutList(CCUInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string s0)) return false; RegDate = s0;
        if (!ks.GetList(CCUInfo)) return false;
        return true;
    }
}

public sealed class KDBE_UPDATE_EVENT_MONSTER_KILL_INFO_NOT : IKSerializable
{
    public int EventUid { get; set; }
    public long KillUnitUid { get; set; }
    public long NpcDieDate { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(EventUid)) return false;
        if (!ks.Put(KillUnitUid)) return false;
        if (!ks.Put(NpcDieDate)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; EventUid = v0;
        if (!ks.Get(out long v1)) return false; KillUnitUid = v1;
        if (!ks.Get(out long v2)) return false; NpcDieDate = v2;
        return true;
    }
}

public sealed class KDBE_UPDATE_EVENT_TIME_ACK : IKSerializable
{
    public int Ok { get; set; }
    public HashSet<int> ConnectTimeEvent { get; set; } = new();
    public HashSet<int> CumulativeTimeEvent { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.PutList(ConnectTimeEvent, static (s, v) => s.Put(v))) return false;
        if (!ks.PutList(CumulativeTimeEvent, static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.GetHashSet(ConnectTimeEvent, static s => (s.Get(out int v), v))) return false;
        if (!ks.GetHashSet(CumulativeTimeEvent, static s => (s.Get(out int v), v))) return false;
        return true;
    }
}

public sealed class KDBE_UPDATE_EVENT_TIME_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public List<KConnectTimeEventInfo> ConnectTimeEvent { get; set; } = new();
    public List<KCumulativeTimeEventInfo> CumulativeTimeEvent { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutList(ConnectTimeEvent)) return false;
        if (!ks.PutList(CumulativeTimeEvent)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.GetList(ConnectTimeEvent)) return false;
        if (!ks.GetList(CumulativeTimeEvent)) return false;
        return true;
    }
}

public sealed class KDBE_UPDATE_GLOBAL_MISSION_INFO_REQ : IKSerializable
{
    public string GlobalMissionDay { get; set; } = string.Empty;
    public int NowCount { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutWideString(GlobalMissionDay)) return false;
        if (!ks.Put(NowCount)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string s0)) return false; GlobalMissionDay = s0;
        if (!ks.Get(out int v1)) return false; NowCount = v1;
        return true;
    }
}

public sealed class KDBE_UPDATE_GUILD_EXP_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long UnitUid { get; set; }
    public int GuildUid { get; set; }
    public int GuildEXP { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(GuildUid)) return false;
        if (!ks.Put(GuildEXP)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        if (!ks.Get(out int v2)) return false; GuildUid = v2;
        if (!ks.Get(out int v3)) return false; GuildEXP = v3;
        return true;
    }
}

public sealed class KDBE_UPDATE_GUILD_EXP_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public int GuildUid { get; set; }
    public int HonorPoint { get; set; }
    public int GuildEXP { get; set; }
    public int GuildLimitEXP { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(GuildUid)) return false;
        if (!ks.Put(HonorPoint)) return false;
        if (!ks.Put(GuildEXP)) return false;
        if (!ks.Put(GuildLimitEXP)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out int v1)) return false; GuildUid = v1;
        if (!ks.Get(out int v2)) return false; HonorPoint = v2;
        if (!ks.Get(out int v3)) return false; GuildEXP = v3;
        if (!ks.Get(out int v4)) return false; GuildLimitEXP = v4;
        return true;
    }
}

public sealed class KDBE_UPDATE_INVENTORY_ITEM_POS_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public Dictionary<long, KItemPosition> ItemPositionUpdate { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutMap(ItemPositionUpdate, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.GetMap(ItemPositionUpdate, static s => (s.Get(out long v), v), static s => { var v = new KItemPosition(); return (s.Get(v), v); })) return false;
        return true;
    }
}

public sealed class KDBE_UPDATE_IS_LOGIN_NOT : IKSerializable
{
    public long UserUid { get; set; }
    public bool IsLogin { get; set; }
    public Dictionary<int, int> AccCountInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UserUid)) return false;
        if (!ks.Put(IsLogin)) return false;
        if (!ks.PutMap(AccCountInfo, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UserUid = v0;
        if (!ks.Get(out bool v1)) return false; IsLogin = v1;
        if (!ks.GetMap(AccCountInfo, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false;
        return true;
    }
}

public sealed class KDBE_UPDATE_PAKCET_MORNITORING_LOG_NOT : IKSerializable
{
    public List<KPacketProcessingLatencyInfo> GSUserPacketLatencyInfo { get; set; } = new();
    public List<KPacketProcessingCountInfo> LoginProxyPacketCountInfo { get; set; } = new();
    public List<KPacketProcessingCountInfo> CnProxyPacketCountInfo { get; set; } = new();
    public List<KPacketProcessingCountInfo> GlobalProxyPacketCountInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutList(GSUserPacketLatencyInfo)) return false;
        if (!ks.PutList(LoginProxyPacketCountInfo)) return false;
        if (!ks.PutList(CnProxyPacketCountInfo)) return false;
        if (!ks.PutList(GlobalProxyPacketCountInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetList(GSUserPacketLatencyInfo)) return false;
        if (!ks.GetList(LoginProxyPacketCountInfo)) return false;
        if (!ks.GetList(CnProxyPacketCountInfo)) return false;
        if (!ks.GetList(GlobalProxyPacketCountInfo)) return false;
        return true;
    }
}

public sealed class KDBE_UPDATE_REWARD_COMPLETE_JP_EVENT_NOT : IKSerializable
{
    public long RId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(RId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; RId = v0;
        return true;
    }
}

public sealed class KDBE_UPDATE_SECURITY_TYPE_NOT : IKSerializable
{
    public long UserUid { get; set; }
    public int SecurityType { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UserUid)) return false;
        if (!ks.Put(SecurityType)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UserUid = v0;
        if (!ks.Get(out int v1)) return false; SecurityType = v1;
        return true;
    }
}

public sealed class KDBE_UPDATE_SERVER_INFO_REQ : IKSerializable
{
    public string RegDate { get; set; } = string.Empty;
    public long ServerUid { get; set; }
    public int ConcurrentUser { get; set; }
    public int CurPartyCount { get; set; }
    public int PlayGamePartyCount { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutWideString(RegDate)) return false;
        if (!ks.Put(ServerUid)) return false;
        if (!ks.Put(ConcurrentUser)) return false;
        if (!ks.Put(CurPartyCount)) return false;
        if (!ks.Put(PlayGamePartyCount)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string s0)) return false; RegDate = s0;
        if (!ks.Get(out long v1)) return false; ServerUid = v1;
        if (!ks.Get(out int v2)) return false; ConcurrentUser = v2;
        if (!ks.Get(out int v3)) return false; CurPartyCount = v3;
        if (!ks.Get(out int v4)) return false; PlayGamePartyCount = v4;
        return true;
    }
}

public sealed class KDBE_UPDATE_SHARE_ITEM_ACK : IKSerializable
{
    public int Ok { get; set; }
    public bool Reload { get; set; }
    public string ReloadNickname { get; set; } = string.Empty;
    public List<KTradeShareItemInfo> TradeShareItemResult { get; set; } = new();
    public List<KInventoryItemInfo> InventorySlotItem { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(Reload)) return false;
        if (!ks.PutWideString(ReloadNickname)) return false;
        if (!ks.PutList(TradeShareItemResult)) return false;
        if (!ks.PutList(InventorySlotItem)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out bool v1)) return false; Reload = v1;
        if (!ks.GetWideString(out string s2)) return false; ReloadNickname = s2;
        if (!ks.GetList(TradeShareItemResult)) return false;
        if (!ks.GetList(InventorySlotItem)) return false;
        return true;
    }
}

public sealed class KDBE_UPDATE_SHARE_ITEM_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public Dictionary<long, KItemPosition> UpdatePosition { get; set; } = new();
    public Dictionary<long, KTradeShareItemInfo> InItem { get; set; } = new();
    public Dictionary<long, KTradeShareItemInfo> OutItem { get; set; } = new();
    public Dictionary<long, KUpdateShareItemInfo> UpdateEndurance { get; set; } = new();
    public Dictionary<long, KUpdateShareItemInfo> UpdateQuantity { get; set; } = new();
    public List<long> DeletedItem { get; set; } = new();
    public bool Reload { get; set; }
    public string ReloadNickname { get; set; } = string.Empty;
    public List<KInventoryItemInfo> InventorySlotItem { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutMap(UpdatePosition, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.PutMap(InItem, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.PutMap(OutItem, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.PutMap(UpdateEndurance, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.PutMap(UpdateQuantity, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.PutList(DeletedItem, static (s, v) => s.Put(v))) return false;
        if (!ks.Put(Reload)) return false;
        if (!ks.PutWideString(ReloadNickname)) return false;
        if (!ks.PutList(InventorySlotItem)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.GetMap(UpdatePosition, static s => (s.Get(out long v), v), static s => { var v = new KItemPosition(); return (s.Get(v), v); })) return false;
        if (!ks.GetMap(InItem, static s => (s.Get(out long v), v), static s => { var v = new KTradeShareItemInfo(); return (s.Get(v), v); })) return false;
        if (!ks.GetMap(OutItem, static s => (s.Get(out long v), v), static s => { var v = new KTradeShareItemInfo(); return (s.Get(v), v); })) return false;
        if (!ks.GetMap(UpdateEndurance, static s => (s.Get(out long v), v), static s => { var v = new KUpdateShareItemInfo(); return (s.Get(v), v); })) return false;
        if (!ks.GetMap(UpdateQuantity, static s => (s.Get(out long v), v), static s => { var v = new KUpdateShareItemInfo(); return (s.Get(v), v); })) return false;
        if (!ks.GetList(DeletedItem, static s => (s.Get(out long v), v))) return false;
        if (!ks.Get(out bool v7)) return false; Reload = v7;
        if (!ks.GetWideString(out string s8)) return false; ReloadNickname = s8;
        if (!ks.GetList(InventorySlotItem)) return false;
        return true;
    }
}

public sealed class KDBE_UPDATE_TIME_RANDOM_ITEM_EVENT_RESULT_NOT : IKSerializable
{
    public KTimeOpenRandomItemEventDBUPdate DBUpdateInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(DBUpdateInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(DBUpdateInfo)) return false;
        return true;
    }
}

public sealed class KDBE_UPDATE_UNIT_CLASS_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public sbyte UnitClass { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(UnitClass)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out sbyte v1)) return false; UnitClass = v1;
        return true;
    }
}

public sealed class KDBE_UPDATE_UNIT_INFO_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int EventId { get; set; }
    public int Exp { get; set; }
    public int Ed { get; set; }
    public int OfficialMatchCnt { get; set; }
    public int Rating { get; set; }
    public int MaxRating { get; set; }
    public int RPoint { get; set; }
    public int APoint { get; set; }
    public int Win { get; set; }
    public int Lose { get; set; }
    public float KFactor { get; set; }
    public List<int> DungeonClear { get; set; } = new();
    public List<int> TCClear { get; set; } = new();
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public Dictionary<long, int> ItemEnduranceUpdate { get; set; } = new();
    public Dictionary<long, KItemPosition> ItemPositionUpdate { get; set; } = new();
    public int NumResurrectionStone { get; set; }
    public int NumAutoPaymentResStone { get; set; }
    public List<KPetInfo> Pet { get; set; } = new();
    public List<long> RidingPetUid { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(EventId)) return false;
        if (!ks.Put(Exp)) return false;
        if (!ks.Put(Ed)) return false;
        if (!ks.Put(OfficialMatchCnt)) return false;
        if (!ks.Put(Rating)) return false;
        if (!ks.Put(MaxRating)) return false;
        if (!ks.Put(RPoint)) return false;
        if (!ks.Put(APoint)) return false;
        if (!ks.Put(Win)) return false;
        if (!ks.Put(Lose)) return false;
        if (!ks.Put(KFactor)) return false;
        if (!ks.PutList(DungeonClear, static (s, v) => s.Put(v))) return false;
        if (!ks.PutList(TCClear, static (s, v) => s.Put(v))) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutMap(ItemEnduranceUpdate, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.PutMap(ItemPositionUpdate, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.Put(NumResurrectionStone)) return false;
        if (!ks.Put(NumAutoPaymentResStone)) return false;
        if (!ks.PutList(Pet)) return false;
        if (!ks.PutList(RidingPetUid, static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out int v1)) return false; EventId = v1;
        if (!ks.Get(out int v2)) return false; Exp = v2;
        if (!ks.Get(out int v3)) return false; Ed = v3;
        if (!ks.Get(out int v4)) return false; OfficialMatchCnt = v4;
        if (!ks.Get(out int v5)) return false; Rating = v5;
        if (!ks.Get(out int v6)) return false; MaxRating = v6;
        if (!ks.Get(out int v7)) return false; RPoint = v7;
        if (!ks.Get(out int v8)) return false; APoint = v8;
        if (!ks.Get(out int v9)) return false; Win = v9;
        if (!ks.Get(out int v10)) return false; Lose = v10;
        if (!ks.Get(out float v11)) return false; KFactor = v11;
        if (!ks.GetList(DungeonClear, static s => (s.Get(out int v), v))) return false;
        if (!ks.GetList(TCClear, static s => (s.Get(out int v), v))) return false;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetMap(ItemEnduranceUpdate, static s => (s.Get(out long v), v), static s => (s.Get(out int v), v))) return false;
        if (!ks.GetMap(ItemPositionUpdate, static s => (s.Get(out long v), v), static s => { var v = new KItemPosition(); return (s.Get(v), v); })) return false;
        if (!ks.Get(out int v17)) return false; NumResurrectionStone = v17;
        if (!ks.Get(out int v18)) return false; NumAutoPaymentResStone = v18;
        if (!ks.GetList(Pet)) return false;
        if (!ks.GetList(RidingPetUid, static s => (s.Get(out long v), v))) return false;
        return true;
    }
}

public sealed class KDBE_UPDATE_UNIT_INFO_REQ : IKSerializable
{
    public bool Final { get; set; }
    public int EventId { get; set; }
    public long UnitUid { get; set; }
    public int Exp { get; set; }
    public int Level { get; set; }
    public int Ed { get; set; }
    public int RealDataED { get; set; }
    public int OfficialMatchCnt { get; set; }
    public int Rating { get; set; }
    public int MaxRating { get; set; }
    public int RPoint { get; set; }
    public int APoint { get; set; }
    public bool IsWinBeforeMatch { get; set; }
    public sbyte EmblemEnum { get; set; }
    public float KFactor { get; set; }
    public int Win { get; set; }
    public int Lose { get; set; }
    public KLastPositionInfo LastPos { get; set; } = new();
    public Dictionary<int, KDungeonClearInfo> DungeonClear { get; set; } = new();
    public Dictionary<int, KTCClearInfo> TCClear { get; set; } = new();
    public Dictionary<int, KDungeonPlayInfo> DungeonPlay { get; set; } = new();
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public Dictionary<long, int> ItemEnduranceUpdate { get; set; } = new();
    public Dictionary<long, KItemPosition> ItemPositionUpdate { get; set; } = new();
    public List<KQuestUpdate> QuestData { get; set; } = new();
    public List<KMissionUpdate> MissionData { get; set; } = new();
    public int InitTitleId { get; set; }
    public int EquippedTitleId { get; set; }
    public Dictionary<int, List<int>> SkillSlotVector { get; set; } = new();
    public string SkillSlotBEndDate { get; set; } = string.Empty;
    public KDenyOptions DenyOptions { get; set; } = new();
    public int Spirit { get; set; }
    public bool IsSpiritUpdated { get; set; }
    public int NumResurrectionStone { get; set; }
    public int NumAutoPaymentResStone { get; set; }
    public Dictionary<int, int> WishList { get; set; } = new();
    public List<KHenirRankingInfo> HenirRanking { get; set; } = new();
    public int GuildUid { get; set; }
    public int HonorPoint { get; set; }
    public List<KCumulativeTimeEventInfo> UpdateEventTime { get; set; } = new();
    public List<int> GiveUpQuestList { get; set; } = new();
    public List<KPetInfo> Pet { get; set; } = new();
    public Dictionary<int, int> CharGameCount { get; set; } = new();
    public KEGS_HENIR_REWARD_COUNT_NOT HenirRewardCnt { get; set; } = new();
    public KGamePlayStatusContainer GamePlayStatus { get; set; } = new();
    public List<KRecordBuffInfo> RecordBuffInfo { get; set; } = new();
    public List<KRidingPetInfo> RidingPetList { get; set; } = new();
    public sbyte UnitRelationshipType { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Final)) return false;
        if (!ks.Put(EventId)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(Exp)) return false;
        if (!ks.Put(Level)) return false;
        if (!ks.Put(Ed)) return false;
        if (!ks.Put(RealDataED)) return false;
        if (!ks.Put(OfficialMatchCnt)) return false;
        if (!ks.Put(Rating)) return false;
        if (!ks.Put(MaxRating)) return false;
        if (!ks.Put(RPoint)) return false;
        if (!ks.Put(APoint)) return false;
        if (!ks.Put(IsWinBeforeMatch)) return false;
        if (!ks.Put(EmblemEnum)) return false;
        if (!ks.Put(KFactor)) return false;
        if (!ks.Put(Win)) return false;
        if (!ks.Put(Lose)) return false;
        if (!ks.Put(LastPos)) return false;
        if (!ks.PutMap(DungeonClear, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.PutMap(TCClear, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.PutMap(DungeonPlay, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutMap(ItemEnduranceUpdate, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.PutMap(ItemPositionUpdate, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.PutList(QuestData)) return false;
        if (!ks.PutList(MissionData)) return false;
        if (!ks.Put(InitTitleId)) return false;
        if (!ks.Put(EquippedTitleId)) return false;
        if (!ks.PutMap(SkillSlotVector, static (s, k) => s.Put(k), static (s, v) => s.PutList(v, static (s2, x) => s2.Put(x)))) return false;
        if (!ks.PutWideString(SkillSlotBEndDate)) return false;
        if (!ks.Put(DenyOptions)) return false;
        if (!ks.Put(Spirit)) return false;
        if (!ks.Put(IsSpiritUpdated)) return false;
        if (!ks.Put(NumResurrectionStone)) return false;
        if (!ks.Put(NumAutoPaymentResStone)) return false;
        if (!ks.PutMap(WishList, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.PutList(HenirRanking)) return false;
        if (!ks.Put(GuildUid)) return false;
        if (!ks.Put(HonorPoint)) return false;
        if (!ks.PutList(UpdateEventTime)) return false;
        if (!ks.PutList(GiveUpQuestList, static (s, v) => s.Put(v))) return false;
        if (!ks.PutList(Pet)) return false;
        if (!ks.PutMap(CharGameCount, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.Put(HenirRewardCnt)) return false;
        if (!ks.Put(GamePlayStatus)) return false;
        if (!ks.PutList(RecordBuffInfo)) return false;
        if (!ks.PutList(RidingPetList)) return false;
        if (!ks.Put(UnitRelationshipType)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool v0)) return false; Final = v0;
        if (!ks.Get(out int v1)) return false; EventId = v1;
        if (!ks.Get(out long v2)) return false; UnitUid = v2;
        if (!ks.Get(out int v3)) return false; Exp = v3;
        if (!ks.Get(out int v4)) return false; Level = v4;
        if (!ks.Get(out int v5)) return false; Ed = v5;
        if (!ks.Get(out int v6)) return false; RealDataED = v6;
        if (!ks.Get(out int v7)) return false; OfficialMatchCnt = v7;
        if (!ks.Get(out int v8)) return false; Rating = v8;
        if (!ks.Get(out int v9)) return false; MaxRating = v9;
        if (!ks.Get(out int v10)) return false; RPoint = v10;
        if (!ks.Get(out int v11)) return false; APoint = v11;
        if (!ks.Get(out bool v12)) return false; IsWinBeforeMatch = v12;
        if (!ks.Get(out sbyte v13)) return false; EmblemEnum = v13;
        if (!ks.Get(out float v14)) return false; KFactor = v14;
        if (!ks.Get(out int v15)) return false; Win = v15;
        if (!ks.Get(out int v16)) return false; Lose = v16;
        if (!ks.Get(LastPos)) return false;
        if (!ks.GetMap(DungeonClear, static s => (s.Get(out int v), v), static s => { var v = new KDungeonClearInfo(); return (s.Get(v), v); })) return false;
        if (!ks.GetMap(TCClear, static s => (s.Get(out int v), v), static s => { var v = new KTCClearInfo(); return (s.Get(v), v); })) return false;
        if (!ks.GetMap(DungeonPlay, static s => (s.Get(out int v), v), static s => { var v = new KDungeonPlayInfo(); return (s.Get(v), v); })) return false;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetMap(ItemEnduranceUpdate, static s => (s.Get(out long v), v), static s => (s.Get(out int v), v))) return false;
        if (!ks.GetMap(ItemPositionUpdate, static s => (s.Get(out long v), v), static s => { var v = new KItemPosition(); return (s.Get(v), v); })) return false;
        if (!ks.GetList(QuestData)) return false;
        if (!ks.GetList(MissionData)) return false;
        if (!ks.Get(out int v26)) return false; InitTitleId = v26;
        if (!ks.Get(out int v27)) return false; EquippedTitleId = v27;
        if (!ks.GetMap(SkillSlotVector, static s => (s.Get(out int v), v), static s => { var l = new List<int>(); return (s.GetList(l, static s2 => (s2.Get(out int x), x)), l); })) return false;
        if (!ks.GetWideString(out string s29)) return false; SkillSlotBEndDate = s29;
        if (!ks.Get(DenyOptions)) return false;
        if (!ks.Get(out int v31)) return false; Spirit = v31;
        if (!ks.Get(out bool v32)) return false; IsSpiritUpdated = v32;
        if (!ks.Get(out int v33)) return false; NumResurrectionStone = v33;
        if (!ks.Get(out int v34)) return false; NumAutoPaymentResStone = v34;
        if (!ks.GetMap(WishList, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false;
        if (!ks.GetList(HenirRanking)) return false;
        if (!ks.Get(out int v37)) return false; GuildUid = v37;
        if (!ks.Get(out int v38)) return false; HonorPoint = v38;
        if (!ks.GetList(UpdateEventTime)) return false;
        if (!ks.GetList(GiveUpQuestList, static s => (s.Get(out int v), v))) return false;
        if (!ks.GetList(Pet)) return false;
        if (!ks.GetMap(CharGameCount, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false;
        if (!ks.Get(HenirRewardCnt)) return false;
        if (!ks.Get(GamePlayStatus)) return false;
        if (!ks.GetList(RecordBuffInfo)) return false;
        if (!ks.GetList(RidingPetList)) return false;
        if (!ks.Get(out sbyte v47)) return false; UnitRelationshipType = v47;
        return true;
    }
}

public sealed class KDBE_UPDATE_USER_INFO_TO_RELAY_DB_JP_EVENT_NOT : IKSerializable
{
    // enum UPDATE_TYPE
    public const int UT_NONE = 0;
    public const int UT_LEVEL_UP = 1;
    public const int UT_UNITCLASS_CHANGE = 2;

    public string UserId { get; set; } = string.Empty;
    public long UserUid { get; set; }
    public long UnitUid { get; set; }
    public string NickName { get; set; } = string.Empty;
    public sbyte Level { get; set; }
    public sbyte UnitClass { get; set; }
    public bool DeletedUnit { get; set; }
    public string UserRegDate { get; set; } = string.Empty;
    public string UnitRegDate { get; set; } = string.Empty;
    public int UpdateType { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutWideString(UserId)) return false;
        if (!ks.Put(UserUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutWideString(NickName)) return false;
        if (!ks.Put(Level)) return false;
        if (!ks.Put(UnitClass)) return false;
        if (!ks.Put(DeletedUnit)) return false;
        if (!ks.PutWideString(UserRegDate)) return false;
        if (!ks.PutWideString(UnitRegDate)) return false;
        if (!ks.Put(UpdateType)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string s0)) return false; UserId = s0;
        if (!ks.Get(out long v1)) return false; UserUid = v1;
        if (!ks.Get(out long v2)) return false; UnitUid = v2;
        if (!ks.GetWideString(out string s3)) return false; NickName = s3;
        if (!ks.Get(out sbyte v4)) return false; Level = v4;
        if (!ks.Get(out sbyte v5)) return false; UnitClass = v5;
        if (!ks.Get(out bool v6)) return false; DeletedUnit = v6;
        if (!ks.GetWideString(out string s7)) return false; UserRegDate = s7;
        if (!ks.GetWideString(out string s8)) return false; UnitRegDate = s8;
        if (!ks.Get(out int v9)) return false; UpdateType = v9;
        return true;
    }
}

public sealed class KDBE_USE_FINALITY_SKILL_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        return true;
    }
}

public sealed class KDBE_USE_FINALITY_SKILL_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();
    public List<KItemInfo> ItemInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        if (!ks.PutList(ItemInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        if (!ks.GetList(ItemInfo)) return false;
        return true;
    }
}

public sealed class KDBE_USE_ITEM_IN_INVENTORY_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int UsedItemId { get; set; }
    public int WarpPointMapId { get; set; }
    public int Ed { get; set; }
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();
    public long TempCode { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(UsedItemId)) return false;
        if (!ks.Put(WarpPointMapId)) return false;
        if (!ks.Put(Ed)) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        if (!ks.Put(TempCode)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out int v1)) return false; UsedItemId = v1;
        if (!ks.Get(out int v2)) return false; WarpPointMapId = v2;
        if (!ks.Get(out int v3)) return false; Ed = v3;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        if (!ks.Get(out long v6)) return false; TempCode = v6;
        return true;
    }
}

public sealed class KDBE_USE_ITEM_IN_INVENTORY_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public int UsedItemId { get; set; }
    public int WarpPointMapId { get; set; }
    public int Ed { get; set; }
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();
    public long TempCode { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(UsedItemId)) return false;
        if (!ks.Put(WarpPointMapId)) return false;
        if (!ks.Put(Ed)) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        if (!ks.Put(TempCode)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out int v1)) return false; UsedItemId = v1;
        if (!ks.Get(out int v2)) return false; WarpPointMapId = v2;
        if (!ks.Get(out int v3)) return false; Ed = v3;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        if (!ks.Get(out long v6)) return false; TempCode = v6;
        return true;
    }
}

public sealed class KDBE_WEDDING_COMPLETE_ACK : IKSerializable
{
    public int Ok { get; set; }
    public string WeddingDate { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.PutWideString(WeddingDate)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.GetWideString(out string s1)) return false; WeddingDate = s1;
        return true;
    }
}

public sealed class KDBE_WEDDING_COMPLETE_REQ : IKSerializable
{
    public int WeddingUid { get; set; }
    public long Groom { get; set; }
    public long Bride { get; set; }
    public string WeddingDate { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(WeddingUid)) return false;
        if (!ks.Put(Groom)) return false;
        if (!ks.Put(Bride)) return false;
        if (!ks.PutWideString(WeddingDate)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; WeddingUid = v0;
        if (!ks.Get(out long v1)) return false; Groom = v1;
        if (!ks.Get(out long v2)) return false; Bride = v2;
        if (!ks.GetWideString(out string s3)) return false; WeddingDate = s3;
        return true;
    }
}

public sealed class KDBE_WEDDING_HALL_INFO_UPDATE_NOT : IKSerializable
{
    public Dictionary<int, KWeddingHallInfo> WeddingInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutMap(WeddingInfo, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetMap(WeddingInfo, static s => (s.Get(out int v), v), static s => { var v = new KWeddingHallInfo(); return (s.Get(v), v); })) return false;
        return true;
    }
}

public sealed class KDBE_WEDDING_ITEM_DELETE_ACK : IKSerializable
{
    public int Ok { get; set; }
    public Dictionary<long, KWeddingItemInfo> WeddingItemInfo { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.PutMap(WeddingItemInfo, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.GetMap(WeddingItemInfo, static s => (s.Get(out long v), v), static s => { var v = new KWeddingItemInfo(); return (s.Get(v), v); })) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        return true;
    }
}

public sealed class KDBE_WEDDING_ITEM_DELETE_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public Dictionary<long, KWeddingItemInfo> WeddingItemInfo { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();
    public List<KDeletedItemInfo> Deleted { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutMap(WeddingItemInfo, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        if (!ks.PutList(Deleted)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.GetMap(WeddingItemInfo, static s => (s.Get(out long v), v), static s => { var v = new KWeddingItemInfo(); return (s.Get(v), v); })) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        if (!ks.GetList(Deleted)) return false;
        return true;
    }
}

public sealed class KDBE_WEDDING_PROPOSE_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int WeddingUid { get; set; }
    public long ManUid { get; set; }
    public long GirlUid { get; set; }
    public sbyte WeddingHallType { get; set; }
    public sbyte OfficiantNPC { get; set; }
    public string WeddingDate { get; set; } = string.Empty;
    public string WeddingMsg { get; set; } = string.Empty;
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(WeddingUid)) return false;
        if (!ks.Put(ManUid)) return false;
        if (!ks.Put(GirlUid)) return false;
        if (!ks.Put(WeddingHallType)) return false;
        if (!ks.Put(OfficiantNPC)) return false;
        if (!ks.PutWideString(WeddingDate)) return false;
        if (!ks.PutWideString(WeddingMsg)) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out int v1)) return false; WeddingUid = v1;
        if (!ks.Get(out long v2)) return false; ManUid = v2;
        if (!ks.Get(out long v3)) return false; GirlUid = v3;
        if (!ks.Get(out sbyte v4)) return false; WeddingHallType = v4;
        if (!ks.Get(out sbyte v5)) return false; OfficiantNPC = v5;
        if (!ks.GetWideString(out string s6)) return false; WeddingDate = s6;
        if (!ks.GetWideString(out string s7)) return false; WeddingMsg = s7;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        return true;
    }
}

public sealed class KDBE_WEDDING_PROPOSE_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public long ManUid { get; set; }
    public long GirlUid { get; set; }
    public sbyte WeddingHallType { get; set; }
    public sbyte OfficiantNPC { get; set; }
    public string WeddingDate { get; set; } = string.Empty;
    public string WeddingMsg { get; set; } = string.Empty;
    public string WeddingNickName { get; set; } = string.Empty;
    public List<long> InviteUnitList { get; set; } = new();
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(ManUid)) return false;
        if (!ks.Put(GirlUid)) return false;
        if (!ks.Put(WeddingHallType)) return false;
        if (!ks.Put(OfficiantNPC)) return false;
        if (!ks.PutWideString(WeddingDate)) return false;
        if (!ks.PutWideString(WeddingMsg)) return false;
        if (!ks.PutWideString(WeddingNickName)) return false;
        if (!ks.PutList(InviteUnitList, static (s, v) => s.Put(v))) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out long v1)) return false; ManUid = v1;
        if (!ks.Get(out long v2)) return false; GirlUid = v2;
        if (!ks.Get(out sbyte v3)) return false; WeddingHallType = v3;
        if (!ks.Get(out sbyte v4)) return false; OfficiantNPC = v4;
        if (!ks.GetWideString(out string s5)) return false; WeddingDate = s5;
        if (!ks.GetWideString(out string s6)) return false; WeddingMsg = s6;
        if (!ks.GetWideString(out string s7)) return false; WeddingNickName = s7;
        if (!ks.GetList(InviteUnitList, static s => (s.Get(out long v), v))) return false;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        return true;
    }
}

public sealed class KDBE_WRITE_COME_BACK_LOGOUT_NOT : IKSerializable
{
    public long UserUid { get; set; }
    public string WsrtLogOutDate { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UserUid)) return false;
        if (!ks.PutWideString(WsrtLogOutDate)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UserUid = v0;
        if (!ks.GetWideString(out string s1)) return false; WsrtLogOutDate = s1;
        return true;
    }
}

public sealed class KDBE_WRITE_COME_BACK_REWARD_NOT : IKSerializable
{
    public long UserUid { get; set; }
    public int RewardStep { get; set; }
    public string ComeBackBuffEndDate { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UserUid)) return false;
        if (!ks.Put(RewardStep)) return false;
        if (!ks.PutWideString(ComeBackBuffEndDate)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UserUid = v0;
        if (!ks.Get(out int v1)) return false; RewardStep = v1;
        if (!ks.GetWideString(out string s2)) return false; ComeBackBuffEndDate = s2;
        return true;
    }
}

public sealed class KDBE_WRITE_LETTER_AND_TRADE_MOINTORING_REQ : IKSerializable
{
    public sbyte TypeVal { get; set; }
    public long RecvUnitUid { get; set; }
    public string RegDate { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(TypeVal)) return false;
        if (!ks.Put(RecvUnitUid)) return false;
        if (!ks.PutWideString(RegDate)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte v0)) return false; TypeVal = v0;
        if (!ks.Get(out long v1)) return false; RecvUnitUid = v1;
        if (!ks.GetWideString(out string s2)) return false; RegDate = s2;
        return true;
    }
}

public sealed class KDBE_WRITE_RELATIONSHIP_INFO_LOG_ACK : IKSerializable
{
    public int Ok { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        return true;
    }
}

public sealed class KDBE_WRITE_RELATIONSHIP_INFO_LOG_REQ : IKSerializable
{
    public long UserUid { get; set; }
    public long UnitUid { get; set; }
    public byte Level { get; set; }
    public sbyte UnitClass { get; set; }
    public sbyte CoupleType { get; set; }
    public string RegDate { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UserUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(Level)) return false;
        if (!ks.Put(UnitClass)) return false;
        if (!ks.Put(CoupleType)) return false;
        if (!ks.PutWideString(RegDate)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UserUid = v0;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        if (!ks.Get(out byte v2)) return false; Level = v2;
        if (!ks.Get(out sbyte v3)) return false; UnitClass = v3;
        if (!ks.Get(out sbyte v4)) return false; CoupleType = v4;
        if (!ks.GetWideString(out string s5)) return false; RegDate = s5;
        return true;
    }
}

public sealed class KEBILL_BUY_PRODUCT_ACK : IKSerializable
{
    public int Ok { get; set; }
    public List<KBillOrderInfo> BillOrderInfo { get; set; } = new();
    public List<int> PackageId { get; set; } = new();
    public int TotalCash { get; set; }
    public int CashType { get; set; }
    public KGlobalCashInfo RemainCashInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.PutList(BillOrderInfo)) return false;
        if (!ks.PutList(PackageId, static (s, v) => s.Put(v))) return false;
        if (!ks.Put(TotalCash)) return false;
        if (!ks.Put(CashType)) return false;
        if (!ks.Put(RemainCashInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.GetList(BillOrderInfo)) return false;
        if (!ks.GetList(PackageId, static s => (s.Get(out int v), v))) return false;
        if (!ks.Get(out int v3)) return false; TotalCash = v3;
        if (!ks.Get(out int v4)) return false; CashType = v4;
        if (!ks.Get(RemainCashInfo)) return false;
        return true;
    }
}

public sealed class KEBILL_BUY_PRODUCT_REQ : IKSerializable
{
    public int ServerGroupId { get; set; }
    public int ChannelingCode { get; set; }
    public string UserId { get; set; } = string.Empty;
    public uint PublisherUid { get; set; }
    public long UserUid { get; set; }
    public long UnitUid { get; set; }
    public string NickName { get; set; } = string.Empty;
    public string UserIP { get; set; } = string.Empty;
    public List<KBillBuyInfo> BillBuyInfo { get; set; } = new();
    public KBuyCharInfo BuyCharInfo { get; set; } = new();
    public int UseCashType { get; set; }
    public int IUserIP { get; set; }
    public string TotalOrderId { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ServerGroupId)) return false;
        if (!ks.Put(ChannelingCode)) return false;
        if (!ks.PutWideString(UserId)) return false;
        if (!ks.Put(PublisherUid)) return false;
        if (!ks.Put(UserUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutWideString(NickName)) return false;
        if (!ks.PutNarrowString(UserIP)) return false;
        if (!ks.PutList(BillBuyInfo)) return false;
        if (!ks.Put(BuyCharInfo)) return false;
        if (!ks.Put(UseCashType)) return false;
        if (!ks.Put(IUserIP)) return false;
        if (!ks.PutWideString(TotalOrderId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; ServerGroupId = v0;
        if (!ks.Get(out int v1)) return false; ChannelingCode = v1;
        if (!ks.GetWideString(out string s2)) return false; UserId = s2;
        if (!ks.Get(out uint v3)) return false; PublisherUid = v3;
        if (!ks.Get(out long v4)) return false; UserUid = v4;
        if (!ks.Get(out long v5)) return false; UnitUid = v5;
        if (!ks.GetWideString(out string s6)) return false; NickName = s6;
        if (!ks.GetNarrowString(out string s7)) return false; UserIP = s7;
        if (!ks.GetList(BillBuyInfo)) return false;
        if (!ks.Get(BuyCharInfo)) return false;
        if (!ks.Get(out int v10)) return false; UseCashType = v10;
        if (!ks.Get(out int v11)) return false; IUserIP = v11;
        if (!ks.GetWideString(out string s12)) return false; TotalOrderId = s12;
        return true;
    }
}

public sealed class KEBILL_CHECK_BALANCE_REQ : IKSerializable
{
    public int PublisherCashBalance { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PublisherCashBalance)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; PublisherCashBalance = v0;
        return true;
    }
}

public sealed class KEBILL_CHECK_PRODUCT_INFO_REQ : IKSerializable
{
    public Dictionary<int, int> ReleaseTick { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutMap(ReleaseTick, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetMap(ReleaseTick, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false;
        return true;
    }
}

public sealed class KEBILL_GET_TRANS_FOR_CHECK_BALANCE_ACK : IKSerializable
{
    public int Ok { get; set; }
    public string Transaction { get; set; } = string.Empty;
    public KEPUBLISHER_BILLING_BALANCE_REQ CheckGiantBalanceReq { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.PutWideString(Transaction)) return false;
        if (!ks.Put(CheckGiantBalanceReq)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.GetWideString(out string s1)) return false; Transaction = s1;
        if (!ks.Get(CheckGiantBalanceReq)) return false;
        return true;
    }
}

public sealed class KEBILL_GET_TRANS_FOR_CHECK_BALANCE_REQ : IKSerializable
{
    public KEPUBLISHER_BILLING_BALANCE_REQ CheckGiantBalanceReq { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(CheckGiantBalanceReq)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(CheckGiantBalanceReq)) return false;
        return true;
    }
}

public sealed class KEBILL_GIFT_ITEM_ACK : IKSerializable
{
    public int Ok { get; set; }
    public List<KBillOrderInfo> BillOrderInfo { get; set; } = new();
    public int TotalCash { get; set; }
    public int CashType { get; set; }
    public KGlobalCashInfo RemainCashInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.PutList(BillOrderInfo)) return false;
        if (!ks.Put(TotalCash)) return false;
        if (!ks.Put(CashType)) return false;
        if (!ks.Put(RemainCashInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.GetList(BillOrderInfo)) return false;
        if (!ks.Get(out int v2)) return false; TotalCash = v2;
        if (!ks.Get(out int v3)) return false; CashType = v3;
        if (!ks.Get(RemainCashInfo)) return false;
        return true;
    }
}

public sealed class KEBILL_GIFT_ITEM_REQ : IKSerializable
{
    public int ServerGroupId { get; set; }
    public int ChannelingCode { get; set; }
    public string SenderUserId { get; set; } = string.Empty;
    public uint SenderPublisherUid { get; set; }
    public string SenderNickName { get; set; } = string.Empty;
    public string ReceiverUserId { get; set; } = string.Empty;
    public string ReceiverNickName { get; set; } = string.Empty;
    public List<KBillBuyInfo> BillBuyInfo { get; set; } = new();
    public long SenderUserUid { get; set; }
    public long SenderUnitUid { get; set; }
    public long ReceiverUserUid { get; set; }
    public long ReceiverUnitUid { get; set; }
    public string PresentMessage { get; set; } = string.Empty;
    public KBuyCharInfo BuyCharInfo { get; set; } = new();
    public KBuyCharInfo RecvCharInfo { get; set; } = new();
    public int UseCashType { get; set; }
    public string UserIP { get; set; } = string.Empty;
    public string TotalOrderId { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ServerGroupId)) return false;
        if (!ks.Put(ChannelingCode)) return false;
        if (!ks.PutWideString(SenderUserId)) return false;
        if (!ks.Put(SenderPublisherUid)) return false;
        if (!ks.PutWideString(SenderNickName)) return false;
        if (!ks.PutWideString(ReceiverUserId)) return false;
        if (!ks.PutWideString(ReceiverNickName)) return false;
        if (!ks.PutList(BillBuyInfo)) return false;
        if (!ks.Put(SenderUserUid)) return false;
        if (!ks.Put(SenderUnitUid)) return false;
        if (!ks.Put(ReceiverUserUid)) return false;
        if (!ks.Put(ReceiverUnitUid)) return false;
        if (!ks.PutWideString(PresentMessage)) return false;
        if (!ks.Put(BuyCharInfo)) return false;
        if (!ks.Put(RecvCharInfo)) return false;
        if (!ks.Put(UseCashType)) return false;
        if (!ks.PutWideString(UserIP)) return false;
        if (!ks.PutWideString(TotalOrderId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; ServerGroupId = v0;
        if (!ks.Get(out int v1)) return false; ChannelingCode = v1;
        if (!ks.GetWideString(out string s2)) return false; SenderUserId = s2;
        if (!ks.Get(out uint v3)) return false; SenderPublisherUid = v3;
        if (!ks.GetWideString(out string s4)) return false; SenderNickName = s4;
        if (!ks.GetWideString(out string s5)) return false; ReceiverUserId = s5;
        if (!ks.GetWideString(out string s6)) return false; ReceiverNickName = s6;
        if (!ks.GetList(BillBuyInfo)) return false;
        if (!ks.Get(out long v8)) return false; SenderUserUid = v8;
        if (!ks.Get(out long v9)) return false; SenderUnitUid = v9;
        if (!ks.Get(out long v10)) return false; ReceiverUserUid = v10;
        if (!ks.Get(out long v11)) return false; ReceiverUnitUid = v11;
        if (!ks.GetWideString(out string s12)) return false; PresentMessage = s12;
        if (!ks.Get(BuyCharInfo)) return false;
        if (!ks.Get(RecvCharInfo)) return false;
        if (!ks.Get(out int v15)) return false; UseCashType = v15;
        if (!ks.GetWideString(out string s16)) return false; UserIP = s16;
        if (!ks.GetWideString(out string s17)) return false; TotalOrderId = s17;
        return true;
    }
}

public sealed class KEBILL_PICK_UP_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int ProductNo { get; set; }
    public Dictionary<int, int> SocketForCash { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(ProductNo)) return false;
        if (!ks.PutMap(SocketForCash, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out int v1)) return false; ProductNo = v1;
        if (!ks.GetMap(SocketForCash, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false;
        return true;
    }
}

public sealed class KEBILL_PICK_UP_REQ : IKSerializable
{
    public long TransNo { get; set; }
    public int ProductNo { get; set; }
    public long UnitUid { get; set; }
    public Dictionary<int, int> SocketForCash { get; set; } = new();
    public byte ProductKind { get; set; }
    public List<KNXBTPickUpPackageInfo> PickUpPackageInfo { get; set; } = new();
    public int SubProductNo { get; set; }
    public int SubProductId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(TransNo)) return false;
        if (!ks.Put(ProductNo)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutMap(SocketForCash, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.Put(ProductKind)) return false;
        if (!ks.PutList(PickUpPackageInfo)) return false;
        if (!ks.Put(SubProductNo)) return false;
        if (!ks.Put(SubProductId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; TransNo = v0;
        if (!ks.Get(out int v1)) return false; ProductNo = v1;
        if (!ks.Get(out long v2)) return false; UnitUid = v2;
        if (!ks.GetMap(SocketForCash, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false;
        if (!ks.Get(out byte v4)) return false; ProductKind = v4;
        if (!ks.GetList(PickUpPackageInfo)) return false;
        if (!ks.Get(out int v6)) return false; SubProductNo = v6;
        if (!ks.Get(out int v7)) return false; SubProductId = v7;
        return true;
    }
}

public sealed class KEBILL_PREPARE_BUY_PRODUCT_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KEBILL_BUY_PRODUCT_REQ EBILL_BUY_PRODUCT_REQ { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(EBILL_BUY_PRODUCT_REQ)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(EBILL_BUY_PRODUCT_REQ)) return false;
        return true;
    }
}

public sealed class KEBILL_PREPARE_GIFT_ITEM_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KEBILL_GIFT_ITEM_REQ EBILL_GIFT_ITEM_REQ { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(EBILL_GIFT_ITEM_REQ)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(EBILL_GIFT_ITEM_REQ)) return false;
        return true;
    }
}

public sealed class KEBILL_REWARD_COUPON_JP_EVENT_ACK : IKSerializable
{
    public KJPEventRewardInfo RewardInfo { get; set; } = new();
    public KEBILL_USE_COUPON_ACK UseCouponPacket { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(RewardInfo)) return false;
        if (!ks.Put(UseCouponPacket)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(RewardInfo)) return false;
        if (!ks.Get(UseCouponPacket)) return false;
        return true;
    }
}

public sealed class KEBILL_REWARD_COUPON_JP_EVENT_REQ : IKSerializable
{
    public KJPEventRewardInfo RewardInfo { get; set; } = new();
    public KEBILL_USE_COUPON_REQ UseCouponPacket { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(RewardInfo)) return false;
        if (!ks.Put(UseCouponPacket)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(RewardInfo)) return false;
        if (!ks.Get(UseCouponPacket)) return false;
        return true;
    }
}

public sealed class KEBILL_USE_COUPON_ACK : IKSerializable
{
    public int Ok { get; set; }
    public List<KBillCouponItemInfo> BillCouponItemInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.PutList(BillCouponItemInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.GetList(BillCouponItemInfo)) return false;
        return true;
    }
}

public sealed class KEGASH_ACCOUNTING_REQ : IKSerializable
{
    // enum GASH_RECEIVE_PACKET_ACCOUNTING
    public const int GRPAT_SERVICE_CODE = 0;
    public const int GRPAT_SERVICE_REGION = 1;
    public const int GRPAT_SERVICE_ACCOUNTID = 2;
    public const int GRPAT_TYPE = 3;
    public const int GRPAT_LOGINTIME = 4;
    public const int GRPAT_LOGOUTTIME = 5;
    public const int GRPAT_IPADDRESS = 6;
    public const int GRPAT_PACKETID = 7;
    public const int GRPAT_NUM = 8;

    public long ActorUid { get; set; }
    public string ServiceCode { get; set; } = string.Empty;
    public string ServiceRegion { get; set; } = string.Empty;
    public string ServiceAccountId { get; set; } = string.Empty;
    public string TypeVal { get; set; } = string.Empty;
    public string LoginTime { get; set; } = string.Empty;
    public string LogoutTime { get; set; } = string.Empty;
    public string IPAddress { get; set; } = string.Empty;
    public string PacketId { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.TW) && !ks.Put(ActorUid)) return false;
        if ((region.Region == RegionCode.TW) && !ks.PutNarrowString(ServiceCode)) return false;
        if ((region.Region == RegionCode.TW) && !ks.PutNarrowString(ServiceRegion)) return false;
        if ((region.Region == RegionCode.TW) && !ks.PutNarrowString(ServiceAccountId)) return false;
        if ((region.Region == RegionCode.TW) && !ks.PutNarrowString(TypeVal)) return false;
        if ((region.Region == RegionCode.TW) && !ks.PutNarrowString(LoginTime)) return false;
        if ((region.Region == RegionCode.TW) && !ks.PutNarrowString(LogoutTime)) return false;
        if ((region.Region == RegionCode.TW) && !ks.PutNarrowString(IPAddress)) return false;
        if ((region.Region == RegionCode.TW) && !ks.PutNarrowString(PacketId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.TW)) { if (!ks.Get(out long v0)) return false; ActorUid = v0; }
        if ((region.Region == RegionCode.TW)) { if (!ks.GetNarrowString(out string s1)) return false; ServiceCode = s1; }
        if ((region.Region == RegionCode.TW)) { if (!ks.GetNarrowString(out string s2)) return false; ServiceRegion = s2; }
        if ((region.Region == RegionCode.TW)) { if (!ks.GetNarrowString(out string s3)) return false; ServiceAccountId = s3; }
        if ((region.Region == RegionCode.TW)) { if (!ks.GetNarrowString(out string s4)) return false; TypeVal = s4; }
        if ((region.Region == RegionCode.TW)) { if (!ks.GetNarrowString(out string s5)) return false; LoginTime = s5; }
        if ((region.Region == RegionCode.TW)) { if (!ks.GetNarrowString(out string s6)) return false; LogoutTime = s6; }
        if ((region.Region == RegionCode.TW)) { if (!ks.GetNarrowString(out string s7)) return false; IPAddress = s7; }
        if ((region.Region == RegionCode.TW)) { if (!ks.GetNarrowString(out string s8)) return false; PacketId = s8; }
        return true;
    }
}

public sealed class KEGASH_AUTHENTICATION_REQ : IKSerializable
{
    // enum GASH_RECEIVE_PACKET_AUTHENTICATION
    public const int GRPAC_SERVICE_CODE = 0;
    public const int GRPAC_SERVICE_REGION = 1;
    public const int GRPAC_SERVICE_ACCOUNTID = 2;
    public const int GRPAC_SERVICE_ACCOUNTPASSWORD = 3;
    public const int GRPAC_LOCKFLAG = 4;
    public const int GRPAC_IPADDRESS = 5;
    public const int GRPAC_PACKETID = 6;
    public const int GRPAC_NUM = 7;

    public long ActorUid { get; set; }
    public string ServiceCode { get; set; } = string.Empty;
    public string ServiceRegion { get; set; } = string.Empty;
    public string ServiceAccountId { get; set; } = string.Empty;
    public string ServicePassword { get; set; } = string.Empty;
    public string LockFlag { get; set; } = string.Empty;
    public string IPAddress { get; set; } = string.Empty;
    public string PacketId { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.TW) && !ks.Put(ActorUid)) return false;
        if ((region.Region == RegionCode.TW) && !ks.PutNarrowString(ServiceCode)) return false;
        if ((region.Region == RegionCode.TW) && !ks.PutNarrowString(ServiceRegion)) return false;
        if ((region.Region == RegionCode.TW) && !ks.PutNarrowString(ServiceAccountId)) return false;
        if ((region.Region == RegionCode.TW) && !ks.PutNarrowString(ServicePassword)) return false;
        if ((region.Region == RegionCode.TW) && !ks.PutNarrowString(LockFlag)) return false;
        if ((region.Region == RegionCode.TW) && !ks.PutNarrowString(IPAddress)) return false;
        if ((region.Region == RegionCode.TW) && !ks.PutNarrowString(PacketId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.TW)) { if (!ks.Get(out long v0)) return false; ActorUid = v0; }
        if ((region.Region == RegionCode.TW)) { if (!ks.GetNarrowString(out string s1)) return false; ServiceCode = s1; }
        if ((region.Region == RegionCode.TW)) { if (!ks.GetNarrowString(out string s2)) return false; ServiceRegion = s2; }
        if ((region.Region == RegionCode.TW)) { if (!ks.GetNarrowString(out string s3)) return false; ServiceAccountId = s3; }
        if ((region.Region == RegionCode.TW)) { if (!ks.GetNarrowString(out string s4)) return false; ServicePassword = s4; }
        if ((region.Region == RegionCode.TW)) { if (!ks.GetNarrowString(out string s5)) return false; LockFlag = s5; }
        if ((region.Region == RegionCode.TW)) { if (!ks.GetNarrowString(out string s6)) return false; IPAddress = s6; }
        if ((region.Region == RegionCode.TW)) { if (!ks.GetNarrowString(out string s7)) return false; PacketId = s7; }
        return true;
    }
}

public sealed class KEGASH_AUTHORIZATION_REQ : IKSerializable
{
    // enum GASH_RECEIVE_PACKET_AUTHORIZATION
    public const int GRPAZ_SERVICE_CODE = 0;
    public const int GRPAZ_SERVICE_REGION = 1;
    public const int GRPAZ_SERVICE_ACCOUNTID = 2;
    public const int GRPAZ_SERVICE_ACCOUNTPASSWORD = 3;
    public const int GRPAZ_AUTHENTICATIONFLAG = 4;
    public const int GRPAZ_TYPE = 5;
    public const int GRPAZ_DUPLICATECHECK = 6;
    public const int GRPAZ_IPADDRESS = 7;
    public const int GRPAZ_PACKETID = 8;
    public const int GRPAZ_NUM = 9;

    public long ActorUid { get; set; }
    public string ServiceCode { get; set; } = string.Empty;
    public string ServiceRegion { get; set; } = string.Empty;
    public string ServiceAccountId { get; set; } = string.Empty;
    public string ServicePassword { get; set; } = string.Empty;
    public string AuthenticationFlag { get; set; } = string.Empty;
    public string TypeVal { get; set; } = string.Empty;
    public string DuplicateCheck { get; set; } = string.Empty;
    public string IPAddress { get; set; } = string.Empty;
    public string PacketId { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.TW) && !ks.Put(ActorUid)) return false;
        if ((region.Region == RegionCode.TW) && !ks.PutNarrowString(ServiceCode)) return false;
        if ((region.Region == RegionCode.TW) && !ks.PutNarrowString(ServiceRegion)) return false;
        if ((region.Region == RegionCode.TW) && !ks.PutNarrowString(ServiceAccountId)) return false;
        if ((region.Region == RegionCode.TW) && !ks.PutNarrowString(ServicePassword)) return false;
        if ((region.Region == RegionCode.TW) && !ks.PutNarrowString(AuthenticationFlag)) return false;
        if ((region.Region == RegionCode.TW) && !ks.PutNarrowString(TypeVal)) return false;
        if ((region.Region == RegionCode.TW) && !ks.PutNarrowString(DuplicateCheck)) return false;
        if ((region.Region == RegionCode.TW) && !ks.PutNarrowString(IPAddress)) return false;
        if ((region.Region == RegionCode.TW) && !ks.PutNarrowString(PacketId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.TW)) { if (!ks.Get(out long v0)) return false; ActorUid = v0; }
        if ((region.Region == RegionCode.TW)) { if (!ks.GetNarrowString(out string s1)) return false; ServiceCode = s1; }
        if ((region.Region == RegionCode.TW)) { if (!ks.GetNarrowString(out string s2)) return false; ServiceRegion = s2; }
        if ((region.Region == RegionCode.TW)) { if (!ks.GetNarrowString(out string s3)) return false; ServiceAccountId = s3; }
        if ((region.Region == RegionCode.TW)) { if (!ks.GetNarrowString(out string s4)) return false; ServicePassword = s4; }
        if ((region.Region == RegionCode.TW)) { if (!ks.GetNarrowString(out string s5)) return false; AuthenticationFlag = s5; }
        if ((region.Region == RegionCode.TW)) { if (!ks.GetNarrowString(out string s6)) return false; TypeVal = s6; }
        if ((region.Region == RegionCode.TW)) { if (!ks.GetNarrowString(out string s7)) return false; DuplicateCheck = s7; }
        if ((region.Region == RegionCode.TW)) { if (!ks.GetNarrowString(out string s8)) return false; IPAddress = s8; }
        if ((region.Region == RegionCode.TW)) { if (!ks.GetNarrowString(out string s9)) return false; PacketId = s9; }
        return true;
    }
}

public sealed class KEGB_CHECK_BLOCK_USER_NOT : IKSerializable
{
    public List<int> BlockType { get; set; } = new();
    public long UserUid { get; set; }
    public long UnitUid { get; set; }
    public string Ip { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutList(BlockType, static (s, v) => s.Put(v))) return false;
        if (!ks.Put(UserUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutWideString(Ip)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetList(BlockType, static s => (s.Get(out int v), v))) return false;
        if (!ks.Get(out long v1)) return false; UserUid = v1;
        if (!ks.Get(out long v2)) return false; UnitUid = v2;
        if (!ks.GetWideString(out string s3)) return false; Ip = s3;
        return true;
    }
}

public sealed class KEGB_GET_REGROUP_PARTY_INFO_REQ : IKSerializable
{
    public long AutoPartyUid { get; set; }
    public KDungeonGameInfo DungeonGameInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(AutoPartyUid)) return false;
        if (!ks.Put(DungeonGameInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; AutoPartyUid = v0;
        if (!ks.Get(DungeonGameInfo)) return false;
        return true;
    }
}

public sealed class KEGB_REMAKING_AUTO_PARTY_NOT : IKSerializable
{
    public KAutoPartyConditionInfo AutoPartyInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(AutoPartyInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(AutoPartyInfo)) return false;
        return true;
    }
}

public sealed class KEGB_REMATCH_MAKING_USER_NOT : IKSerializable
{
    public sbyte PvpPlayerCount { get; set; }
    public sbyte PvpGameType { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PvpPlayerCount)) return false;
        if (!ks.Put(PvpGameType)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte v0)) return false; PvpPlayerCount = v0;
        if (!ks.Get(out sbyte v1)) return false; PvpGameType = v1;
        return true;
    }
}

public sealed class KEGB_UPDATE_AUTO_PARTY_BONUS_INFO_NOT : IKSerializable
{
    public sbyte DungeonMode { get; set; }
    public int DungeonIDWithDif { get; set; }
    public bool ActivatedBonus { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(DungeonMode)) return false;
        if (!ks.Put(DungeonIDWithDif)) return false;
        if (!ks.Put(ActivatedBonus)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte v0)) return false; DungeonMode = v0;
        if (!ks.Get(out int v1)) return false; DungeonIDWithDif = v1;
        if (!ks.Get(out bool v2)) return false; ActivatedBonus = v2;
        return true;
    }
}

public sealed class KEGB_UPDATE_BLOCK_RESULT_NOT : IKSerializable
{
    public Dictionary<int, bool> BlockResult { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutMap(BlockResult, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetMap(BlockResult, static s => (s.Get(out int v), v), static s => (s.Get(out bool v), v))) return false;
        return true;
    }
}

public sealed class KEGB_UPDATE_GLOBAL_MISSION_START_TIME_NOT : IKSerializable
{
    public Dictionary<int, string> GlobalMissionStartTime { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutMap(GlobalMissionStartTime, static (s, k) => s.Put(k), static (s, v) => s.PutWideString(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetMap(GlobalMissionStartTime, static s => (s.Get(out int v), v), static s => (s.GetWideString(out string v), v))) return false;
        return true;
    }
}

public sealed class KEGB_UPDATE_MISSION_START_TIME_NOT : IKSerializable
{
    public string WsrtStartTime { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutWideString(WsrtStartTime)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string s0)) return false; WsrtStartTime = s0;
        return true;
    }
}

public sealed class KEGB_UPDATE_WORLD_BUFF_NOT : IKSerializable
{
    public bool Active { get; set; }
    public ushort SuccessWorldBuff { get; set; }
    public ushort WorldBuffDurationTime { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Active)) return false;
        if (!ks.Put(SuccessWorldBuff)) return false;
        if (!ks.Put(WorldBuffDurationTime)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool v0)) return false; Active = v0;
        if (!ks.Get(out ushort v1)) return false; SuccessWorldBuff = v1;
        if (!ks.Get(out ushort v2)) return false; WorldBuffDurationTime = v2;
        return true;
    }
}

public sealed class KEGIANT_AUTH_DIRECT_CHARGE_ACK : IKSerializable
{
    public int Ok { get; set; }
    public string ServiceAccountId { get; set; } = string.Empty;
    public uint UserUid { get; set; }
    public uint GameZone { get; set; }
    public uint UnitUid { get; set; }
    public string UnitNickName { get; set; } = string.Empty;
    public string Ip { get; set; } = string.Empty;
    public ushort TokenLen { get; set; }
    public string Token { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.CN) && !ks.Put(Ok)) return false;
        if ((region.Region == RegionCode.CN) && !ks.PutWideString(ServiceAccountId)) return false;
        if ((region.Region == RegionCode.CN) && !ks.Put(UserUid)) return false;
        if ((region.Region == RegionCode.CN) && !ks.Put(GameZone)) return false;
        if ((region.Region == RegionCode.CN) && !ks.Put(UnitUid)) return false;
        if ((region.Region == RegionCode.CN) && !ks.PutWideString(UnitNickName)) return false;
        if ((region.Region == RegionCode.CN) && !ks.PutWideString(Ip)) return false;
        if ((region.Region == RegionCode.CN) && !ks.Put(TokenLen)) return false;
        if ((region.Region == RegionCode.CN) && !ks.PutWideString(Token)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out int v0)) return false; Ok = v0; }
        if ((region.Region == RegionCode.CN)) { if (!ks.GetWideString(out string s1)) return false; ServiceAccountId = s1; }
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out uint v2)) return false; UserUid = v2; }
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out uint v3)) return false; GameZone = v3; }
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out uint v4)) return false; UnitUid = v4; }
        if ((region.Region == RegionCode.CN)) { if (!ks.GetWideString(out string s5)) return false; UnitNickName = s5; }
        if ((region.Region == RegionCode.CN)) { if (!ks.GetWideString(out string s6)) return false; Ip = s6; }
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out ushort v7)) return false; TokenLen = v7; }
        if ((region.Region == RegionCode.CN)) { if (!ks.GetWideString(out string s8)) return false; Token = s8; }
        return true;
    }
}

public sealed class KEGIANT_AUTH_DIRECT_CHARGE_REQ : IKSerializable
{
    public string ServiceAccountId { get; set; } = string.Empty;
    public uint UserUid { get; set; }
    public uint GameZone { get; set; }
    public uint UnitUid { get; set; }
    public string UnitNickName { get; set; } = string.Empty;
    public string Ip { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.CN) && !ks.PutWideString(ServiceAccountId)) return false;
        if ((region.Region == RegionCode.CN) && !ks.Put(UserUid)) return false;
        if ((region.Region == RegionCode.CN) && !ks.Put(GameZone)) return false;
        if ((region.Region == RegionCode.CN) && !ks.Put(UnitUid)) return false;
        if ((region.Region == RegionCode.CN) && !ks.PutWideString(UnitNickName)) return false;
        if ((region.Region == RegionCode.CN) && !ks.PutWideString(Ip)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.CN)) { if (!ks.GetWideString(out string s0)) return false; ServiceAccountId = s0; }
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out uint v1)) return false; UserUid = v1; }
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out uint v2)) return false; GameZone = v2; }
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out uint v3)) return false; UnitUid = v3; }
        if ((region.Region == RegionCode.CN)) { if (!ks.GetWideString(out string s4)) return false; UnitNickName = s4; }
        if ((region.Region == RegionCode.CN)) { if (!ks.GetWideString(out string s5)) return false; Ip = s5; }
        return true;
    }
}

public sealed class KEGIANT_AUTH_LOGIN_FAIL : IKSerializable
{
    public uint RequestId { get; set; }
    public int ReturnCode { get; set; }
    public string Name { get; set; } = string.Empty;
    public uint Key { get; set; }
    public string Coordinate { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(RequestId)) return false;
        if (!ks.Put(ReturnCode)) return false;
        if (!ks.PutWideString(Name)) return false;
        if (!ks.Put(Key)) return false;
        if (!ks.PutWideString(Coordinate)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint v0)) return false; RequestId = v0;
        if (!ks.Get(out int v1)) return false; ReturnCode = v1;
        if (!ks.GetWideString(out string s2)) return false; Name = s2;
        if (!ks.Get(out uint v3)) return false; Key = v3;
        if (!ks.GetWideString(out string s4)) return false; Coordinate = s4;
        return true;
    }
}

public sealed class KEGIANT_AUTH_LOGIN_SUCCESS : IKSerializable
{
    public uint RequestId { get; set; }
    public uint Uid { get; set; }
    public string Name { get; set; } = string.Empty;
    public uint TypeVal { get; set; }
    public string EMail { get; set; } = string.Empty;
    public uint State { get; set; }
    public uint BindType { get; set; }
    public string Superpasswd { get; set; } = string.Empty;
    public string MobileAccount { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(RequestId)) return false;
        if (!ks.Put(Uid)) return false;
        if (!ks.PutWideString(Name)) return false;
        if (!ks.Put(TypeVal)) return false;
        if (!ks.PutWideString(EMail)) return false;
        if (!ks.Put(State)) return false;
        if (!ks.Put(BindType)) return false;
        if (!ks.PutWideString(Superpasswd)) return false;
        if (!ks.PutWideString(MobileAccount)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint v0)) return false; RequestId = v0;
        if (!ks.Get(out uint v1)) return false; Uid = v1;
        if (!ks.GetWideString(out string s2)) return false; Name = s2;
        if (!ks.Get(out uint v3)) return false; TypeVal = v3;
        if (!ks.GetWideString(out string s4)) return false; EMail = s4;
        if (!ks.Get(out uint v5)) return false; State = v5;
        if (!ks.Get(out uint v6)) return false; BindType = v6;
        if (!ks.GetWideString(out string s7)) return false; Superpasswd = s7;
        if (!ks.GetWideString(out string s8)) return false; MobileAccount = s8;
        return true;
    }
}

public sealed class KEGIANT_BILLING_CONSUME_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int Balance { get; set; }
    public int Bonus { get; set; }
    public bool HasFilled { get; set; }
    public int Point { get; set; }
    public uint GiantUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(Balance)) return false;
        if (!ks.Put(Bonus)) return false;
        if (!ks.Put(HasFilled)) return false;
        if (!ks.Put(Point)) return false;
        if (!ks.Put(GiantUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out int v1)) return false; Balance = v1;
        if (!ks.Get(out int v2)) return false; Bonus = v2;
        if (!ks.Get(out bool v3)) return false; HasFilled = v3;
        if (!ks.Get(out int v4)) return false; Point = v4;
        if (!ks.Get(out uint v5)) return false; GiantUid = v5;
        return true;
    }
}

public sealed class KEGIANT_BILLING_CONSUME_REQ : IKSerializable
{
    public uint GiantUid { get; set; }
    public int Point { get; set; }
    public int Source { get; set; }
    public string TId { get; set; } = string.Empty;
    public string Remark { get; set; } = string.Empty;
    public string Ip { get; set; } = string.Empty;
    public int AT { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(GiantUid)) return false;
        if (!ks.Put(Point)) return false;
        if (!ks.Put(Source)) return false;
        if (!ks.PutWideString(TId)) return false;
        if (!ks.PutWideString(Remark)) return false;
        if (!ks.PutWideString(Ip)) return false;
        if (!ks.Put(AT)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint v0)) return false; GiantUid = v0;
        if (!ks.Get(out int v1)) return false; Point = v1;
        if (!ks.Get(out int v2)) return false; Source = v2;
        if (!ks.GetWideString(out string s3)) return false; TId = s3;
        if (!ks.GetWideString(out string s4)) return false; Remark = s4;
        if (!ks.GetWideString(out string s5)) return false; Ip = s5;
        if (!ks.Get(out int v6)) return false; AT = v6;
        return true;
    }
}

public sealed class KEGIANT_BILLING_PCARD_CMD : IKSerializable
{
    public uint GiantUid { get; set; }
    public int Source { get; set; }
    public string TId { get; set; } = string.Empty;
    public string PCardId { get; set; } = string.Empty;
    public string Ip { get; set; } = string.Empty;
    public int AT { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(GiantUid)) return false;
        if (!ks.Put(Source)) return false;
        if (!ks.PutWideString(TId)) return false;
        if (!ks.PutWideString(PCardId)) return false;
        if (!ks.PutWideString(Ip)) return false;
        if (!ks.Put(AT)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint v0)) return false; GiantUid = v0;
        if (!ks.Get(out int v1)) return false; Source = v1;
        if (!ks.GetWideString(out string s2)) return false; TId = s2;
        if (!ks.GetWideString(out string s3)) return false; PCardId = s3;
        if (!ks.GetWideString(out string s4)) return false; Ip = s4;
        if (!ks.Get(out int v5)) return false; AT = v5;
        return true;
    }
}

public sealed class KEGIANT_BILLING_RETURN : IKSerializable
{
    public string TId { get; set; } = string.Empty;
    public int Balance { get; set; }
    public int Bonus { get; set; }
    public int HadFilled { get; set; }
    public int Ret { get; set; }
    public int Subat { get; set; }
    public int AT { get; set; }
    public int Point { get; set; }
    public uint GiantUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutWideString(TId)) return false;
        if (!ks.Put(Balance)) return false;
        if (!ks.Put(Bonus)) return false;
        if (!ks.Put(HadFilled)) return false;
        if (!ks.Put(Ret)) return false;
        if (!ks.Put(Subat)) return false;
        if (!ks.Put(AT)) return false;
        if (!ks.Put(Point)) return false;
        if (!ks.Put(GiantUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string s0)) return false; TId = s0;
        if (!ks.Get(out int v1)) return false; Balance = v1;
        if (!ks.Get(out int v2)) return false; Bonus = v2;
        if (!ks.Get(out int v3)) return false; HadFilled = v3;
        if (!ks.Get(out int v4)) return false; Ret = v4;
        if (!ks.Get(out int v5)) return false; Subat = v5;
        if (!ks.Get(out int v6)) return false; AT = v6;
        if (!ks.Get(out int v7)) return false; Point = v7;
        if (!ks.Get(out uint v8)) return false; GiantUid = v8;
        return true;
    }
}

public sealed class KEGIANT_BILLING_SCARD_CMD : IKSerializable
{
    public uint GiantUid { get; set; }
    public int Source { get; set; }
    public string TId { get; set; } = string.Empty;
    public string SCardId { get; set; } = string.Empty;
    public string Ip { get; set; } = string.Empty;
    public int AT { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(GiantUid)) return false;
        if (!ks.Put(Source)) return false;
        if (!ks.PutWideString(TId)) return false;
        if (!ks.PutWideString(SCardId)) return false;
        if (!ks.PutWideString(Ip)) return false;
        if (!ks.Put(AT)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint v0)) return false; GiantUid = v0;
        if (!ks.Get(out int v1)) return false; Source = v1;
        if (!ks.GetWideString(out string s2)) return false; TId = s2;
        if (!ks.GetWideString(out string s3)) return false; SCardId = s3;
        if (!ks.GetWideString(out string s4)) return false; Ip = s4;
        if (!ks.Get(out int v5)) return false; AT = v5;
        return true;
    }
}

public sealed class KEGIANT_BILLING_USE_CARD_CMD : IKSerializable
{
    public short Source { get; set; }
    public string Account { get; set; } = string.Empty;
    public string TId { get; set; } = string.Empty;
    public string CardId { get; set; } = string.Empty;
    public string Passwd { get; set; } = string.Empty;
    public string Ip { get; set; } = string.Empty;
    public uint Uid { get; set; }
    public int AT { get; set; }
    public uint ConTempId { get; set; }
    public uint OperateUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Source)) return false;
        if (!ks.PutWideString(Account)) return false;
        if (!ks.PutWideString(TId)) return false;
        if (!ks.PutWideString(CardId)) return false;
        if (!ks.PutWideString(Passwd)) return false;
        if (!ks.PutWideString(Ip)) return false;
        if (!ks.Put(Uid)) return false;
        if (!ks.Put(AT)) return false;
        if (!ks.Put(ConTempId)) return false;
        if (!ks.Put(OperateUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out short v0)) return false; Source = v0;
        if (!ks.GetWideString(out string s1)) return false; Account = s1;
        if (!ks.GetWideString(out string s2)) return false; TId = s2;
        if (!ks.GetWideString(out string s3)) return false; CardId = s3;
        if (!ks.GetWideString(out string s4)) return false; Passwd = s4;
        if (!ks.GetWideString(out string s5)) return false; Ip = s5;
        if (!ks.Get(out uint v6)) return false; Uid = v6;
        if (!ks.Get(out int v7)) return false; AT = v7;
        if (!ks.Get(out uint v8)) return false; ConTempId = v8;
        if (!ks.Get(out uint v9)) return false; OperateUid = v9;
        return true;
    }
}

public sealed class KEGIANT_BILLING_USE_CARD_RET_CMD : IKSerializable
{
    public short Source { get; set; }
    public string Account { get; set; } = string.Empty;
    public string TId { get; set; } = string.Empty;
    public string CardId { get; set; } = string.Empty;
    public string Passwd { get; set; } = string.Empty;
    public string Ip { get; set; } = string.Empty;
    public int Point { get; set; }
    public int Ret { get; set; }
    public uint Uid { get; set; }
    public int AT { get; set; }
    public uint ConTempId { get; set; }
    public uint OperateUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Source)) return false;
        if (!ks.PutWideString(Account)) return false;
        if (!ks.PutWideString(TId)) return false;
        if (!ks.PutWideString(CardId)) return false;
        if (!ks.PutWideString(Passwd)) return false;
        if (!ks.PutWideString(Ip)) return false;
        if (!ks.Put(Point)) return false;
        if (!ks.Put(Ret)) return false;
        if (!ks.Put(Uid)) return false;
        if (!ks.Put(AT)) return false;
        if (!ks.Put(ConTempId)) return false;
        if (!ks.Put(OperateUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out short v0)) return false; Source = v0;
        if (!ks.GetWideString(out string s1)) return false; Account = s1;
        if (!ks.GetWideString(out string s2)) return false; TId = s2;
        if (!ks.GetWideString(out string s3)) return false; CardId = s3;
        if (!ks.GetWideString(out string s4)) return false; Passwd = s4;
        if (!ks.GetWideString(out string s5)) return false; Ip = s5;
        if (!ks.Get(out int v6)) return false; Point = v6;
        if (!ks.Get(out int v7)) return false; Ret = v7;
        if (!ks.Get(out uint v8)) return false; Uid = v8;
        if (!ks.Get(out int v9)) return false; AT = v9;
        if (!ks.Get(out uint v10)) return false; ConTempId = v10;
        if (!ks.Get(out uint v11)) return false; OperateUid = v11;
        return true;
    }
}

public sealed class KEGIANT_COMMON_INITIALIZE_ACK : IKSerializable
{
    public ushort Zone { get; set; }
    public ushort Game { get; set; }
    public string ZoneName { get; set; } = string.Empty;
    public byte NetType { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Zone)) return false;
        if (!ks.Put(Game)) return false;
        if (!ks.PutWideString(ZoneName)) return false;
        if (!ks.Put(NetType)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out ushort v0)) return false; Zone = v0;
        if (!ks.Get(out ushort v1)) return false; Game = v1;
        if (!ks.GetWideString(out string s2)) return false; ZoneName = s2;
        if (!ks.Get(out byte v3)) return false; NetType = v3;
        return true;
    }
}

public sealed class KEGIANT_COMMON_INITIALIZE_REQ : IKSerializable
{
    public string Ip { get; set; } = string.Empty;
    public ushort Port { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutWideString(Ip)) return false;
        if (!ks.Put(Port)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string s0)) return false; Ip = s0;
        if (!ks.Get(out ushort v1)) return false; Port = v1;
        return true;
    }
}

public sealed class KEGIANT_COMMON_NULL_CLIENT : IKSerializable
{

    public bool Put(KSerializer ks, RegionProfile region)
    {
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        return true;
    }
}

public sealed class KEGIANT_COMMON_NULL_SERVER : IKSerializable
{

    public bool Put(KSerializer ks, RegionProfile region)
    {
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        return true;
    }
}

public sealed class KEGIANT_INFO_CHAT_NOT : IKSerializable
{
    public ushort Zone { get; set; }
    public ushort Game { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string GMName { get; set; } = string.Empty;
    public uint CountryId { get; set; }
    public uint SceneId { get; set; }
    public uint TypeVal { get; set; }
    public string Content { get; set; } = string.Empty;
    public string Server { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Zone)) return false;
        if (!ks.Put(Game)) return false;
        if (!ks.PutWideString(UserName)) return false;
        if (!ks.PutWideString(GMName)) return false;
        if (!ks.Put(CountryId)) return false;
        if (!ks.Put(SceneId)) return false;
        if (!ks.Put(TypeVal)) return false;
        if (!ks.PutWideString(Content)) return false;
        if (!ks.PutWideString(Server)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out ushort v0)) return false; Zone = v0;
        if (!ks.Get(out ushort v1)) return false; Game = v1;
        if (!ks.GetWideString(out string s2)) return false; UserName = s2;
        if (!ks.GetWideString(out string s3)) return false; GMName = s3;
        if (!ks.Get(out uint v4)) return false; CountryId = v4;
        if (!ks.Get(out uint v5)) return false; SceneId = v5;
        if (!ks.Get(out uint v6)) return false; TypeVal = v6;
        if (!ks.GetWideString(out string s7)) return false; Content = s7;
        if (!ks.GetWideString(out string s8)) return false; Server = s8;
        return true;
    }
}

public sealed class KEGIANT_INFO_GET_CCU_REQ : IKSerializable
{
    public long TimeStamp { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(TimeStamp)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; TimeStamp = v0;
        return true;
    }
}

public sealed class KEGIANT_INFO_GET_CHANNEL_CCU_ACK : IKSerializable
{
    public long TimeStamp { get; set; }
    public ushort Zone { get; set; }
    public ushort Game { get; set; }
    public Dictionary<long, uint> ChannelCCU { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(TimeStamp)) return false;
        if (!ks.Put(Zone)) return false;
        if (!ks.Put(Game)) return false;
        if (!ks.PutMap(ChannelCCU, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; TimeStamp = v0;
        if (!ks.Get(out ushort v1)) return false; Zone = v1;
        if (!ks.Get(out ushort v2)) return false; Game = v2;
        if (!ks.GetMap(ChannelCCU, static s => (s.Get(out long v), v), static s => (s.Get(out uint v), v))) return false;
        return true;
    }
}

public sealed class KEGIANT_INFO_GET_TOTAL_CCU_ACK : IKSerializable
{
    public long TimeStamp { get; set; }
    public ushort ServerId { get; set; }
    public ushort ServerType { get; set; }
    public ushort Zone { get; set; }
    public ushort Game { get; set; }
    public string ZoneName { get; set; } = string.Empty;
    public uint CCU { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(TimeStamp)) return false;
        if (!ks.Put(ServerId)) return false;
        if (!ks.Put(ServerType)) return false;
        if (!ks.Put(Zone)) return false;
        if (!ks.Put(Game)) return false;
        if (!ks.PutWideString(ZoneName)) return false;
        if (!ks.Put(CCU)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; TimeStamp = v0;
        if (!ks.Get(out ushort v1)) return false; ServerId = v1;
        if (!ks.Get(out ushort v2)) return false; ServerType = v2;
        if (!ks.Get(out ushort v3)) return false; Zone = v3;
        if (!ks.Get(out ushort v4)) return false; Game = v4;
        if (!ks.GetWideString(out string s5)) return false; ZoneName = s5;
        if (!ks.Get(out uint v6)) return false; CCU = v6;
        return true;
    }
}

public sealed class KEGIANT_INFO_PUNISH_REQ : IKSerializable
{
    public ushort Zone { get; set; }
    public ushort Game { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string CountryName { get; set; } = string.Empty;
    public string ServerName { get; set; } = string.Empty;
    public string GMName { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public ushort Operation { get; set; }
    public int Delay { get; set; }
    public short Level { get; set; }
    public uint GiantUid { get; set; }
    public uint StartTime { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Zone)) return false;
        if (!ks.Put(Game)) return false;
        if (!ks.PutWideString(UserName)) return false;
        if (!ks.PutWideString(CountryName)) return false;
        if (!ks.PutWideString(ServerName)) return false;
        if (!ks.PutWideString(GMName)) return false;
        if (!ks.PutWideString(Reason)) return false;
        if (!ks.Put(Operation)) return false;
        if (!ks.Put(Delay)) return false;
        if (!ks.Put(Level)) return false;
        if (!ks.Put(GiantUid)) return false;
        if (!ks.Put(StartTime)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out ushort v0)) return false; Zone = v0;
        if (!ks.Get(out ushort v1)) return false; Game = v1;
        if (!ks.GetWideString(out string s2)) return false; UserName = s2;
        if (!ks.GetWideString(out string s3)) return false; CountryName = s3;
        if (!ks.GetWideString(out string s4)) return false; ServerName = s4;
        if (!ks.GetWideString(out string s5)) return false; GMName = s5;
        if (!ks.GetWideString(out string s6)) return false; Reason = s6;
        if (!ks.Get(out ushort v7)) return false; Operation = v7;
        if (!ks.Get(out int v8)) return false; Delay = v8;
        if (!ks.Get(out short v9)) return false; Level = v9;
        if (!ks.Get(out uint v10)) return false; GiantUid = v10;
        if (!ks.Get(out uint v11)) return false; StartTime = v11;
        return true;
    }
}

public sealed class KEGIANT_INFO_REQUESTID_ACK : IKSerializable
{
    public string UserName { get; set; } = string.Empty;
    public string GMName { get; set; } = string.Empty;
    public uint GiantUid { get; set; }
    public string Account { get; set; } = string.Empty;
    public byte Write { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.CN) && !ks.PutWideString(UserName)) return false;
        if ((region.Region == RegionCode.CN) && !ks.PutWideString(GMName)) return false;
        if ((region.Region == RegionCode.CN) && !ks.Put(GiantUid)) return false;
        if ((region.Region == RegionCode.CN) && !ks.PutWideString(Account)) return false;
        if ((region.Region == RegionCode.CN) && !ks.Put(Write)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.CN)) { if (!ks.GetWideString(out string s0)) return false; UserName = s0; }
        if ((region.Region == RegionCode.CN)) { if (!ks.GetWideString(out string s1)) return false; GMName = s1; }
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out uint v2)) return false; GiantUid = v2; }
        if ((region.Region == RegionCode.CN)) { if (!ks.GetWideString(out string s3)) return false; Account = s3; }
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out byte v4)) return false; Write = v4; }
        return true;
    }
}

public sealed class KEGIANT_INFO_REQUESTID_REQ : IKSerializable
{
    public ushort Zone { get; set; }
    public ushort Game { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string GMName { get; set; } = string.Empty;
    public uint GiantUid { get; set; }
    public string Account { get; set; } = string.Empty;
    public byte Write { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.CN) && !ks.Put(Zone)) return false;
        if ((region.Region == RegionCode.CN) && !ks.Put(Game)) return false;
        if ((region.Region == RegionCode.CN) && !ks.PutWideString(UserName)) return false;
        if ((region.Region == RegionCode.CN) && !ks.PutWideString(GMName)) return false;
        if ((region.Region == RegionCode.CN) && !ks.Put(GiantUid)) return false;
        if ((region.Region == RegionCode.CN) && !ks.PutWideString(Account)) return false;
        if ((region.Region == RegionCode.CN) && !ks.Put(Write)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out ushort v0)) return false; Zone = v0; }
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out ushort v1)) return false; Game = v1; }
        if ((region.Region == RegionCode.CN)) { if (!ks.GetWideString(out string s2)) return false; UserName = s2; }
        if ((region.Region == RegionCode.CN)) { if (!ks.GetWideString(out string s3)) return false; GMName = s3; }
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out uint v4)) return false; GiantUid = v4; }
        if ((region.Region == RegionCode.CN)) { if (!ks.GetWideString(out string s5)) return false; Account = s5; }
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out byte v6)) return false; Write = v6; }
        return true;
    }
}

public sealed class KEGIANT_INFO_USERONLINE_ACK : IKSerializable
{
    public string UserName { get; set; } = string.Empty;
    public string GMName { get; set; } = string.Empty;
    public byte UserOnline { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.CN) && !ks.PutWideString(UserName)) return false;
        if ((region.Region == RegionCode.CN) && !ks.PutWideString(GMName)) return false;
        if ((region.Region == RegionCode.CN) && !ks.Put(UserOnline)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.CN)) { if (!ks.GetWideString(out string s0)) return false; UserName = s0; }
        if ((region.Region == RegionCode.CN)) { if (!ks.GetWideString(out string s1)) return false; GMName = s1; }
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out byte v2)) return false; UserOnline = v2; }
        return true;
    }
}

public sealed class KEGIANT_ROLEREG : IKSerializable
{
    public ushort RegType { get; set; }
    public ushort ServerId { get; set; }
    public uint GiantUid { get; set; }
    public ushort Zone { get; set; }
    public ushort Game { get; set; }
    public string Name { get; set; } = string.Empty;
    public uint RoleUid { get; set; }
    public ushort State { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(RegType)) return false;
        if (!ks.Put(ServerId)) return false;
        if (!ks.Put(GiantUid)) return false;
        if (!ks.Put(Zone)) return false;
        if (!ks.Put(Game)) return false;
        if (!ks.PutWideString(Name)) return false;
        if (!ks.Put(RoleUid)) return false;
        if (!ks.Put(State)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out ushort v0)) return false; RegType = v0;
        if (!ks.Get(out ushort v1)) return false; ServerId = v1;
        if (!ks.Get(out uint v2)) return false; GiantUid = v2;
        if (!ks.Get(out ushort v3)) return false; Zone = v3;
        if (!ks.Get(out ushort v4)) return false; Game = v4;
        if (!ks.GetWideString(out string s5)) return false; Name = s5;
        if (!ks.Get(out uint v6)) return false; RoleUid = v6;
        if (!ks.Get(out ushort v7)) return false; State = v7;
        return true;
    }
}

public sealed class KEGIANT_ROLEREG_CREATE_GUILD_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KEGS_CREATE_GUILD_REQ CreateGuildReqPacket { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(CreateGuildReqPacket)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(CreateGuildReqPacket)) return false;
        return true;
    }
}

public sealed class KEGIANT_ROLEREG_CREATE_GUILD_REQ : IKSerializable
{
    public long ActorUid { get; set; }
    public uint GiantUid { get; set; }
    public KEGS_CREATE_GUILD_REQ CreateGuildReqPacket { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ActorUid)) return false;
        if (!ks.Put(GiantUid)) return false;
        if (!ks.Put(CreateGuildReqPacket)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; ActorUid = v0;
        if (!ks.Get(out uint v1)) return false; GiantUid = v1;
        if (!ks.Get(CreateGuildReqPacket)) return false;
        return true;
    }
}

public sealed class KEGIANT_ROLEREG_CREATE_UNIT_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KEGS_CREATE_UNIT_REQ CreateUnitReqPacket { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(CreateUnitReqPacket)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(CreateUnitReqPacket)) return false;
        return true;
    }
}

public sealed class KEGIANT_ROLEREG_CREATE_UNIT_REQ : IKSerializable
{
    public long ActorUid { get; set; }
    public uint GiantUid { get; set; }
    public KEGS_CREATE_UNIT_REQ CreateUnitReqPacket { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ActorUid)) return false;
        if (!ks.Put(GiantUid)) return false;
        if (!ks.Put(CreateUnitReqPacket)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; ActorUid = v0;
        if (!ks.Get(out uint v1)) return false; GiantUid = v1;
        if (!ks.Get(CreateUnitReqPacket)) return false;
        return true;
    }
}

public sealed class KEGIANT_ROLEREG_DELETE_GUILD_ACK : IKSerializable
{
    public int Ok { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        return true;
    }
}

public sealed class KEGIANT_ROLEREG_DELETE_GUILD_REQ : IKSerializable
{
    public string GuildName { get; set; } = string.Empty;
    public uint GiantUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutWideString(GuildName)) return false;
        if (!ks.Put(GiantUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string s0)) return false; GuildName = s0;
        if (!ks.Get(out uint v1)) return false; GiantUid = v1;
        return true;
    }
}

public sealed class KEGIANT_ROLEREG_DELETE_UNIT_ACK : IKSerializable
{
    public int Ok { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        return true;
    }
}

public sealed class KEGIANT_ROLEREG_DELETE_UNIT_REQ : IKSerializable
{
    public string NickName { get; set; } = string.Empty;
    public uint GiantUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutWideString(NickName)) return false;
        if (!ks.Put(GiantUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string s0)) return false; NickName = s0;
        if (!ks.Get(out uint v1)) return false; GiantUid = v1;
        return true;
    }
}

public sealed class KELG_ADMIN_DELETE_GUILD_ADD_NOT : IKSerializable
{
    public int GuildUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(GuildUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; GuildUid = v0;
        return true;
    }
}

public sealed class KELG_ADMIN_DELETE_HENIR_RANKING_NICKNAME_NOT : IKSerializable
{
    public int RankingType { get; set; }
    public string NickName { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(RankingType)) return false;
        if (!ks.PutWideString(NickName)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; RankingType = v0;
        if (!ks.GetWideString(out string s1)) return false; NickName = s1;
        return true;
    }
}

public sealed class KELG_ADMIN_DELETE_HENIR_RANKING_RANK_NOT : IKSerializable
{
    public int RankingType { get; set; }
    public int Rank { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(RankingType)) return false;
        if (!ks.Put(Rank)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; RankingType = v0;
        if (!ks.Get(out int v1)) return false; Rank = v1;
        return true;
    }
}

public sealed class KELG_ADMIN_DELETE_HENIR_RANKING_UNITUID_NOT : IKSerializable
{
    public int RankingType { get; set; }
    public long UnitUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(RankingType)) return false;
        if (!ks.Put(UnitUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; RankingType = v0;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        return true;
    }
}

public sealed class KELG_ADMIN_DELETE_RECORD_NOT : IKSerializable
{
    public int RankingType { get; set; }
    public string NickName { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(RankingType)) return false;
        if (!ks.PutWideString(NickName)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; RankingType = v0;
        if (!ks.GetWideString(out string s1)) return false; NickName = s1;
        return true;
    }
}

public sealed class KELG_ADMIN_GET_GUILD_SKILL_POINT_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int GuildUid { get; set; }
    public long UnitUid { get; set; }
    public int GuildSPoint { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(GuildUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(GuildSPoint)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out int v1)) return false; GuildUid = v1;
        if (!ks.Get(out long v2)) return false; UnitUid = v2;
        if (!ks.Get(out int v3)) return false; GuildSPoint = v3;
        return true;
    }
}

public sealed class KELG_ADMIN_NEW_RECORD_NOT : IKSerializable
{
    public int RankingType { get; set; }
    public KHenirRankingInfo NewRecordRanking { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(RankingType)) return false;
        if (!ks.Put(NewRecordRanking)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; RankingType = v0;
        if (!ks.Get(NewRecordRanking)) return false;
        return true;
    }
}

public sealed class KELG_ADMIN_NEW_RECORD_TEST_NOT : IKSerializable
{
    public List<KHenirRankingInfo> NewRecordTest { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutList(NewRecordTest)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetList(NewRecordTest)) return false;
        return true;
    }
}

public sealed class KELG_ADMIN_SHOW_ED_CHEAT_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long SenderUserUid { get; set; }
    public string NickName { get; set; } = string.Empty;
    public int Ed { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(SenderUserUid)) return false;
        if (!ks.PutWideString(NickName)) return false;
        if (!ks.Put(Ed)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out long v1)) return false; SenderUserUid = v1;
        if (!ks.GetWideString(out string s2)) return false; NickName = s2;
        if (!ks.Get(out int v3)) return false; Ed = v3;
        return true;
    }
}

public sealed class KELG_ADMIN_SHOW_ED_CHEAT_REQ : IKSerializable
{
    public long SenderUserUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(SenderUserUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; SenderUserUid = v0;
        return true;
    }
}

public class KELG_BREAK_UP_NOT : IKSerializable
{
    public long LoverUnitUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(LoverUnitUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; LoverUnitUid = v0;
        return true;
    }
}

public sealed class KELG_CASH_DIRECT_CHARGE_CN_REQ : IKSerializable
{
    public long UserUid { get; set; }
    public string ServiceAccountId { get; set; } = string.Empty;
    public long UnitUid { get; set; }
    public string UnitNickName { get; set; } = string.Empty;
    public string Ip { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.CN) && !ks.Put(UserUid)) return false;
        if ((region.Region == RegionCode.CN) && !ks.PutWideString(ServiceAccountId)) return false;
        if ((region.Region == RegionCode.CN) && !ks.Put(UnitUid)) return false;
        if ((region.Region == RegionCode.CN) && !ks.PutWideString(UnitNickName)) return false;
        if ((region.Region == RegionCode.CN) && !ks.PutWideString(Ip)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out long v0)) return false; UserUid = v0; }
        if ((region.Region == RegionCode.CN)) { if (!ks.GetWideString(out string s1)) return false; ServiceAccountId = s1; }
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out long v2)) return false; UnitUid = v2; }
        if ((region.Region == RegionCode.CN)) { if (!ks.GetWideString(out string s3)) return false; UnitNickName = s3; }
        if ((region.Region == RegionCode.CN)) { if (!ks.GetWideString(out string s4)) return false; Ip = s4; }
        return true;
    }
}

public sealed class KELG_CHANGE_GUILD_NAME_RESULT_REQ : IKSerializable
{
    public int GuildUid { get; set; }
    public string GuildName { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(GuildUid)) return false;
        if (!ks.PutWideString(GuildName)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; GuildUid = v0;
        if (!ks.GetWideString(out string s1)) return false; GuildName = s1;
        return true;
    }
}

public sealed class KELG_CHANGE_GUILD_NAME_TIME_CHECK_ACK : IKSerializable
{
    public int Ok { get; set; }
    public uint OrderNo { get; set; }
    public uint ProductNo { get; set; }
    public int Quantity { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(OrderNo)) return false;
        if (!ks.Put(ProductNo)) return false;
        if (!ks.Put(Quantity)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out uint v1)) return false; OrderNo = v1;
        if (!ks.Get(out uint v2)) return false; ProductNo = v2;
        if (!ks.Get(out int v3)) return false; Quantity = v3;
        return true;
    }
}

public sealed class KELG_CHANGE_GUILD_NAME_TIME_CHECK_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public uint OrderNo { get; set; }
    public uint ProductNo { get; set; }
    public int Quantity { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(OrderNo)) return false;
        if (!ks.Put(ProductNo)) return false;
        if (!ks.Put(Quantity)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out uint v1)) return false; OrderNo = v1;
        if (!ks.Get(out uint v2)) return false; ProductNo = v2;
        if (!ks.Get(out int v3)) return false; Quantity = v3;
        return true;
    }
}

public sealed class KELG_CHANGE_LOVE_WORD_NOT : IKSerializable
{
    public long UseUnitUid { get; set; }
    public long LoverUnitUid { get; set; }
    public string NewLoveWord { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UseUnitUid)) return false;
        if (!ks.Put(LoverUnitUid)) return false;
        if (!ks.PutWideString(NewLoveWord)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UseUnitUid = v0;
        if (!ks.Get(out long v1)) return false; LoverUnitUid = v1;
        if (!ks.GetWideString(out string s2)) return false; NewLoveWord = s2;
        return true;
    }
}

public sealed class KELG_CHANNEL_CHANGE_NEXON_USER_AUTHENTICATE_ACK : IKSerializable
{
    public KEGS_CONNECT_ACK ConnectAck { get; set; } = new();
    public KELG_NEXON_USER_AUTHENTICATE_ACK UserAuthAck { get; set; } = new();
    public KEGS_SELECT_UNIT_REQ SelectUnitReq { get; set; } = new();
    public KChannelChangeInfo ChangeUserInfo { get; set; } = new();
    public bool UseSecondPW { get; set; }
    public string SecondPW { get; set; } = string.Empty;
    public int FailedCount { get; set; }
    public string LastAuthDate { get; set; } = string.Empty;
    public ushort Zone { get; set; }
    public ushort Game { get; set; }
    public uint GiantAccountType { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ConnectAck)) return false;
        if (!ks.Put(UserAuthAck)) return false;
        if (!ks.Put(SelectUnitReq)) return false;
        if (!ks.Put(ChangeUserInfo)) return false;
        if (!ks.Put(UseSecondPW)) return false;
        if (!ks.PutWideString(SecondPW)) return false;
        if (!ks.Put(FailedCount)) return false;
        if (!ks.PutWideString(LastAuthDate)) return false;
        if ((region.Region == RegionCode.CN) && !ks.Put(Zone)) return false;
        if ((region.Region == RegionCode.CN) && !ks.Put(Game)) return false;
        if ((region.Region == RegionCode.CN) && !ks.Put(GiantAccountType)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(ConnectAck)) return false;
        if (!ks.Get(UserAuthAck)) return false;
        if (!ks.Get(SelectUnitReq)) return false;
        if (!ks.Get(ChangeUserInfo)) return false;
        if (!ks.Get(out bool v4)) return false; UseSecondPW = v4;
        if (!ks.GetWideString(out string s5)) return false; SecondPW = s5;
        if (!ks.Get(out int v6)) return false; FailedCount = v6;
        if (!ks.GetWideString(out string s7)) return false; LastAuthDate = s7;
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out ushort v8)) return false; Zone = v8; }
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out ushort v9)) return false; Game = v9; }
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out uint v10)) return false; GiantAccountType = v10; }
        return true;
    }
}

public sealed class KELG_CHANNEL_CHANGE_NEXON_USER_AUTHENTICATE_REQ : IKSerializable
{
    public KEGS_CONNECT_ACK ConnectAck { get; set; } = new();
    public KELG_NEXON_USER_AUTHENTICATE_REQ UserAuthReq { get; set; } = new();
    public KEGS_SELECT_UNIT_REQ SelectUnitReq { get; set; } = new();
    public KChannelChangeInfo ChangeUserInfo { get; set; } = new();
    public bool ServerUseKogOTP { get; set; }
    public ushort Zone { get; set; }
    public ushort Game { get; set; }
    public uint GiantAccountType { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ConnectAck)) return false;
        if (!ks.Put(UserAuthReq)) return false;
        if (!ks.Put(SelectUnitReq)) return false;
        if (!ks.Put(ChangeUserInfo)) return false;
        if (!ks.Put(ServerUseKogOTP)) return false;
        if ((region.Region == RegionCode.CN) && !ks.Put(Zone)) return false;
        if ((region.Region == RegionCode.CN) && !ks.Put(Game)) return false;
        if ((region.Region == RegionCode.CN) && !ks.Put(GiantAccountType)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(ConnectAck)) return false;
        if (!ks.Get(UserAuthReq)) return false;
        if (!ks.Get(SelectUnitReq)) return false;
        if (!ks.Get(ChangeUserInfo)) return false;
        if (!ks.Get(out bool v4)) return false; ServerUseKogOTP = v4;
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out ushort v5)) return false; Zone = v5; }
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out ushort v6)) return false; Game = v6; }
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out uint v7)) return false; GiantAccountType = v7; }
        return true;
    }
}

public sealed class KELG_CHANNEL_CHANGE_REGISTER_USER_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KEGS_CONNECT_ACK ConnectAck { get; set; } = new();
    public KEGS_VERIFY_ACCOUNT_ACK VerifyAccountAck { get; set; } = new();
    public KEGS_SELECT_UNIT_REQ SelectUnitReq { get; set; } = new();
    public KChannelChangeInfo ChangeUserInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(ConnectAck)) return false;
        if (!ks.Put(VerifyAccountAck)) return false;
        if (!ks.Put(SelectUnitReq)) return false;
        if (!ks.Put(ChangeUserInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(ConnectAck)) return false;
        if (!ks.Get(VerifyAccountAck)) return false;
        if (!ks.Get(SelectUnitReq)) return false;
        if (!ks.Get(ChangeUserInfo)) return false;
        return true;
    }
}

public sealed class KELG_CHANNEL_CHANGE_REGISTER_USER_REQ : IKSerializable
{
    public KEGS_CONNECT_ACK ConnectAck { get; set; } = new();
    public KEGS_VERIFY_ACCOUNT_ACK VerifyAccountAck { get; set; } = new();
    public KEGS_SELECT_UNIT_REQ SelectUnitReq { get; set; } = new();
    public KChannelChangeInfo ChangeUserInfo { get; set; } = new();
    public KELG_REGISTER_USER_REQ RegUserReq { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ConnectAck)) return false;
        if (!ks.Put(VerifyAccountAck)) return false;
        if (!ks.Put(SelectUnitReq)) return false;
        if (!ks.Put(ChangeUserInfo)) return false;
        if (!ks.Put(RegUserReq)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(ConnectAck)) return false;
        if (!ks.Get(VerifyAccountAck)) return false;
        if (!ks.Get(SelectUnitReq)) return false;
        if (!ks.Get(ChangeUserInfo)) return false;
        if (!ks.Get(RegUserReq)) return false;
        return true;
    }
}

public sealed class KELG_CHAT_NOT : IKSerializable
{
    public sbyte ChatPacketType { get; set; }
    public long SenderUnitUid { get; set; }
    public string SenderUnitNickName { get; set; } = string.Empty;
    public long ReceiverUnitUid { get; set; }
    public string RecieverUnitNickName { get; set; } = string.Empty;
    public string Msg { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ChatPacketType)) return false;
        if (!ks.Put(SenderUnitUid)) return false;
        if (!ks.PutWideString(SenderUnitNickName)) return false;
        if (!ks.Put(ReceiverUnitUid)) return false;
        if (!ks.PutWideString(RecieverUnitNickName)) return false;
        if (!ks.PutWideString(Msg)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte v0)) return false; ChatPacketType = v0;
        if (!ks.Get(out long v1)) return false; SenderUnitUid = v1;
        if (!ks.GetWideString(out string s2)) return false; SenderUnitNickName = s2;
        if (!ks.Get(out long v3)) return false; ReceiverUnitUid = v3;
        if (!ks.GetWideString(out string s4)) return false; RecieverUnitNickName = s4;
        if (!ks.GetWideString(out string s5)) return false; Msg = s5;
        return true;
    }
}

public sealed class KELG_CHECK_EXPAND_GUILD_MAX_MEMBER_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long ItemUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(ItemUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out long v1)) return false; ItemUid = v1;
        return true;
    }
}

public sealed class KELG_CHECK_TIME_RANDOM_ITEM_EVENT_ACK : IKSerializable
{
    public long ItemUid { get; set; }
    public int EventItemId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ItemUid)) return false;
        if (!ks.Put(EventItemId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; ItemUid = v0;
        if (!ks.Get(out int v1)) return false; EventItemId = v1;
        return true;
    }
}

public class KELG_COUPLE_PROPOSE_CHECK_CONNECT_USER_ACK : IKSerializable
{
    public int Ok { get; set; }
    public string NickName { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.PutWideString(NickName)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.GetWideString(out string s1)) return false; NickName = s1;
        return true;
    }
}

public class KELG_COUPLE_PROPOSE_NOT : IKSerializable
{
    public string RequestUnitName { get; set; } = string.Empty;
    public long RequestUnitUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutWideString(RequestUnitName)) return false;
        if (!ks.Put(RequestUnitUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string s0)) return false; RequestUnitName = s0;
        if (!ks.Get(out long v1)) return false; RequestUnitUid = v1;
        return true;
    }
}

public sealed class KELG_DELETE_APPLY_JOIN_GUILD_REQ : IKSerializable
{
    public int GuildUid { get; set; }
    public long UnitUid { get; set; }
    public long DeletedUnitUid { get; set; }
    public bool DeleteChar { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(GuildUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(DeletedUnitUid)) return false;
        if (!ks.Put(DeleteChar)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; GuildUid = v0;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        if (!ks.Get(out long v2)) return false; DeletedUnitUid = v2;
        if (!ks.Get(out bool v3)) return false; DeleteChar = v3;
        return true;
    }
}

public sealed class KELG_DELETE_FRIEND_NOT : IKSerializable
{
    public long DestUnitUid { get; set; }
    public long UnitUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(DestUnitUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; DestUnitUid = v0;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        return true;
    }
}

public sealed class KELG_DEL_TUTORIAL_NOT : IKSerializable
{
    public long ReceiverUnitUid { get; set; }
    public long DeleteUnitUid { get; set; }
    public string Reason { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ReceiverUnitUid)) return false;
        if (!ks.Put(DeleteUnitUid)) return false;
        if (!ks.PutWideString(Reason)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; ReceiverUnitUid = v0;
        if (!ks.Get(out long v1)) return false; DeleteUnitUid = v1;
        if (!ks.GetWideString(out string s2)) return false; Reason = s2;
        return true;
    }
}

public sealed class KELG_DENY_FRIEND_NOT : IKSerializable
{
    public long DestUnitUid { get; set; }
    public long UnitUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(DestUnitUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; DestUnitUid = v0;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        return true;
    }
}

public sealed class KELG_EXCHANGE_CASH_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int Point { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(Point)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out int v1)) return false; Point = v1;
        return true;
    }
}

public sealed class KELG_FIND_DELETE_CHAR_IN_RANKING_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public List<int> RankingType { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutList(RankingType, static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.GetList(RankingType, static s => (s.Get(out int v), v))) return false;
        return true;
    }
}

public sealed class KELG_GAME_USER_AUTHENTICATE_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KAccountInfo AccountInfo { get; set; } = new();
    public KNexonAccountInfo NexonAccountInfo { get; set; } = new();
    public sbyte HackingUserType { get; set; }
    public uint GiantAccountType { get; set; }
    public bool JoinAccount { get; set; }
    public Dictionary<int, int> AccCountInfo { get; set; } = new();
    public ushort Zone { get; set; }
    public ushort Game { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(AccountInfo)) return false;
        if (!ks.Put(NexonAccountInfo)) return false;
        if (!ks.Put(HackingUserType)) return false;
        if (!ks.Put(GiantAccountType)) return false;
        if (!ks.PutMap(AccCountInfo, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if ((region.Region == RegionCode.CN) && !ks.Put(Zone)) return false;
        if ((region.Region == RegionCode.CN) && !ks.Put(Game)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(AccountInfo)) return false;
        if (!ks.Get(NexonAccountInfo)) return false;
        if (!ks.Get(out sbyte v3)) return false; HackingUserType = v3;
        if (!ks.Get(out uint v4)) return false; GiantAccountType = v4;
        if (!ks.GetMap(AccCountInfo, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false;
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out ushort v6)) return false; Zone = v6; }
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out ushort v7)) return false; Game = v7; }
        return true;
    }
}

public sealed class KELG_GAME_USER_AUTHENTICATE_REQ : IKSerializable
{
    public KUserAuthenticateReq AuthenticateReq { get; set; } = new();
    public KNexonAccountInfo NexonAccountInfo { get; set; } = new();
    public ushort Zone { get; set; }
    public ushort Game { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(AuthenticateReq)) return false;
        if (!ks.Put(NexonAccountInfo)) return false;
        if ((region.Region == RegionCode.CN) && !ks.Put(Zone)) return false;
        if ((region.Region == RegionCode.CN) && !ks.Put(Game)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(AuthenticateReq)) return false;
        if (!ks.Get(NexonAccountInfo)) return false;
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out ushort v2)) return false; Zone = v2; }
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out ushort v3)) return false; Game = v3; }
        return true;
    }
}

public sealed class KELG_GET_GUILD_SKILL_IN_BOARD_REQ : IKSerializable
{
    public int GuildUid { get; set; }
    public long UnitUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(GuildUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; GuildUid = v0;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        return true;
    }
}

public sealed class KELG_GET_RANKING_FOR_GS_RANKING_MANAGER_REQ : IKSerializable
{
    public long ServerUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ServerUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; ServerUid = v0;
        return true;
    }
}

public sealed class KELG_GET_UNIT_INFO_REQ : IKSerializable
{
    public ushort EventId { get; set; }
    public long DemandUserUid { get; set; }
    public long DemandGSUid { get; set; }
    public int KNMSN { get; set; }
    public string NickName { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(EventId)) return false;
        if (!ks.Put(DemandUserUid)) return false;
        if (!ks.Put(DemandGSUid)) return false;
        if (!ks.Put(KNMSN)) return false;
        if (!ks.PutWideString(NickName)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out ushort v0)) return false; EventId = v0;
        if (!ks.Get(out long v1)) return false; DemandUserUid = v1;
        if (!ks.Get(out long v2)) return false; DemandGSUid = v2;
        if (!ks.Get(out int v3)) return false; KNMSN = v3;
        if (!ks.GetWideString(out string s4)) return false; NickName = s4;
        return true;
    }
}

public sealed class KELG_GET_WEB_RANKING_REFRESH_NOT : IKSerializable
{
    public long ServerUid { get; set; }
    public sbyte RankingRefreshType { get; set; }
    public uint LastIndex { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ServerUid)) return false;
        if (!ks.Put(RankingRefreshType)) return false;
        if (!ks.Put(LastIndex)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; ServerUid = v0;
        if (!ks.Get(out sbyte v1)) return false; RankingRefreshType = v1;
        if (!ks.Get(out uint v2)) return false; LastIndex = v2;
        return true;
    }
}

public class KELG_HENIR_RANKING_REFRESH_NOT : IKSerializable
{
    public Dictionary<int, List<KHenirRankingInfo>> HenirRanking { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutMap(HenirRanking, static (s, k) => s.Put(k), static (s, v) => s.PutList(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetMap(HenirRanking, static s => (s.Get(out int v), v), static s => { var l = new List<KHenirRankingInfo>(); return (s.GetList(l), l); })) return false;
        return true;
    }
}

public sealed class KELG_INIT_GUILD_SKILL_NOT : IKSerializable
{
    public int GuildSPoint { get; set; }
    public int GuildCSPoint { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(GuildSPoint)) return false;
        if (!ks.Put(GuildCSPoint)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; GuildSPoint = v0;
        if (!ks.Get(out int v1)) return false; GuildCSPoint = v1;
        return true;
    }
}

public sealed class KELG_INIT_GUILD_SKILL_REQ : IKSerializable
{
    public int GuildUid { get; set; }
    public long UnitUid { get; set; }
    public long ItemUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(GuildUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(ItemUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; GuildUid = v0;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        if (!ks.Get(out long v2)) return false; ItemUid = v2;
        return true;
    }
}

public sealed class KELG_INSERT_GUILD_CASH_SKILL_POINT_NOT : IKSerializable
{
    public int GuildUid { get; set; }
    public long UnitUid { get; set; }
    public int GuildCSPoint { get; set; }
    public int Period { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(GuildUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(GuildCSPoint)) return false;
        if (!ks.Put(Period)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; GuildUid = v0;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        if (!ks.Get(out int v2)) return false; GuildCSPoint = v2;
        if (!ks.Get(out int v3)) return false; Period = v3;
        return true;
    }
}

public sealed class KELG_INVITE_GUILD_ACK : IKSerializable
{
    public int Ok { get; set; }
    public string ReceiverNickName { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.PutWideString(ReceiverNickName)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.GetWideString(out string s1)) return false; ReceiverNickName = s1;
        return true;
    }
}

public sealed class KELG_INVITE_GUILD_FIND_USER_ACK : IKSerializable
{
    public int Ok { get; set; }
    public ushort EventId { get; set; }
    public int GuildUid { get; set; }
    public long SenderUnitUid { get; set; }
    public long ReceiverUnitUid { get; set; }
    public string ReceiverNickName { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(EventId)) return false;
        if (!ks.Put(GuildUid)) return false;
        if (!ks.Put(SenderUnitUid)) return false;
        if (!ks.Put(ReceiverUnitUid)) return false;
        if (!ks.PutWideString(ReceiverNickName)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out ushort v1)) return false; EventId = v1;
        if (!ks.Get(out int v2)) return false; GuildUid = v2;
        if (!ks.Get(out long v3)) return false; SenderUnitUid = v3;
        if (!ks.Get(out long v4)) return false; ReceiverUnitUid = v4;
        if (!ks.GetWideString(out string s5)) return false; ReceiverNickName = s5;
        return true;
    }
}

public sealed class KELG_INVITE_GUILD_FIND_USER_REQ : IKSerializable
{
    public ushort EventId { get; set; }
    public KGuildInfo GuildInfo { get; set; } = new();
    public long SenderUnitUid { get; set; }
    public string SenderNickName { get; set; } = string.Empty;
    public long ReceiverUnitUid { get; set; }
    public bool AutoJoinGuild { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(EventId)) return false;
        if (!ks.Put(GuildInfo)) return false;
        if (!ks.Put(SenderUnitUid)) return false;
        if (!ks.PutWideString(SenderNickName)) return false;
        if (!ks.Put(ReceiverUnitUid)) return false;
        if (!ks.Put(AutoJoinGuild)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out ushort v0)) return false; EventId = v0;
        if (!ks.Get(GuildInfo)) return false;
        if (!ks.Get(out long v2)) return false; SenderUnitUid = v2;
        if (!ks.GetWideString(out string s3)) return false; SenderNickName = s3;
        if (!ks.Get(out long v4)) return false; ReceiverUnitUid = v4;
        if (!ks.Get(out bool v5)) return false; AutoJoinGuild = v5;
        return true;
    }
}

public sealed class KELG_INVITE_GUILD_REPLY_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int GuildUid { get; set; }
    public long GuildMasterUnitUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(GuildUid)) return false;
        if (!ks.Put(GuildMasterUnitUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out int v1)) return false; GuildUid = v1;
        if (!ks.Get(out long v2)) return false; GuildMasterUnitUid = v2;
        return true;
    }
}

public sealed class KELG_INVITE_PARTY_REPLY_TO_HOST_NOT : IKSerializable
{
    public long HostUnitUid { get; set; }
    public sbyte ReplyReason { get; set; }
    public KPartyUserInfo PartyUserInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(HostUnitUid)) return false;
        if (!ks.Put(ReplyReason)) return false;
        if (!ks.Put(PartyUserInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; HostUnitUid = v0;
        if (!ks.Get(out sbyte v1)) return false; ReplyReason = v1;
        if (!ks.Get(PartyUserInfo)) return false;
        return true;
    }
}

public sealed class KELG_JOIN_GUILD_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KEGS_GUILD_INFO_NOT GuildInfoNot { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(GuildInfoNot)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(GuildInfoNot)) return false;
        return true;
    }
}

public sealed class KELG_JOIN_GUILD_REQ : IKSerializable
{
    public int Ok { get; set; }
    public long UnitUid { get; set; }
    public int GuildUid { get; set; }
    public KGuildMemberInfo LoginGuildMember { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(GuildUid)) return false;
        if (!ks.Put(LoginGuildMember)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        if (!ks.Get(out int v2)) return false; GuildUid = v2;
        if (!ks.Get(LoginGuildMember)) return false;
        return true;
    }
}

public sealed class KELG_KNM_INVITE_REQ : IKSerializable
{
    public string InviteUnitNickName { get; set; } = string.Empty;
    public string MyNickName { get; set; } = string.Empty;
    public KNMUnitInfo MyUnitInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutWideString(InviteUnitNickName)) return false;
        if (!ks.PutWideString(MyNickName)) return false;
        if (!ks.Put(MyUnitInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string s0)) return false; InviteUnitNickName = s0;
        if (!ks.GetWideString(out string s1)) return false; MyNickName = s1;
        if (!ks.Get(MyUnitInfo)) return false;
        return true;
    }
}

public sealed class KELG_KNM_TOGETHER_BY_UNIT_INFO_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long DemandGSUid { get; set; }
    public long DemandUserUid { get; set; }
    public KNMUnitInfo UnitInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(DemandGSUid)) return false;
        if (!ks.Put(DemandUserUid)) return false;
        if (!ks.Put(UnitInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out long v1)) return false; DemandGSUid = v1;
        if (!ks.Get(out long v2)) return false; DemandUserUid = v2;
        if (!ks.Get(UnitInfo)) return false;
        return true;
    }
}

public sealed class KELG_KNM_TOGETHER_BY_UNIT_INFO_REQ : IKSerializable
{
    public long DemandGSUid { get; set; }
    public long DemandUserUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(DemandGSUid)) return false;
        if (!ks.Put(DemandUserUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; DemandGSUid = v0;
        if (!ks.Get(out long v1)) return false; DemandUserUid = v1;
        return true;
    }
}

public sealed class KELG_NEXON_USER_AUTHENTICATE_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KAccountInfo AccountInfo { get; set; } = new();
    public KNexonAccountInfo NexonAccountInfo { get; set; } = new();
    public sbyte HackingUserType { get; set; }
    public bool JoinAccount { get; set; }
    public Dictionary<int, int> AccCountInfo { get; set; } = new();
    public uint GiantAccountType { get; set; }
    public ushort Zone { get; set; }
    public ushort Game { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(AccountInfo)) return false;
        if (!ks.Put(NexonAccountInfo)) return false;
        if (!ks.Put(HackingUserType)) return false;
        if (!ks.Put(JoinAccount)) return false;
        if (!ks.PutMap(AccCountInfo, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if ((region.Region == RegionCode.CN) && !ks.Put(GiantAccountType)) return false;
        if ((region.Region == RegionCode.CN) && !ks.Put(Zone)) return false;
        if ((region.Region == RegionCode.CN) && !ks.Put(Game)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(AccountInfo)) return false;
        if (!ks.Get(NexonAccountInfo)) return false;
        if (!ks.Get(out sbyte v3)) return false; HackingUserType = v3;
        if (!ks.Get(out bool v4)) return false; JoinAccount = v4;
        if (!ks.GetMap(AccCountInfo, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false;
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out uint v6)) return false; GiantAccountType = v6; }
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out ushort v7)) return false; Zone = v7; }
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out ushort v8)) return false; Game = v8; }
        return true;
    }
}

public sealed class KELG_NEXON_USER_AUTHENTICATE_REQ : IKSerializable
{
    public KUserAuthenticateReq AuthenticateReq { get; set; } = new();
    public KNexonAccountInfo NexonAccountInfo { get; set; } = new();
    public bool ServerUseKogOTP { get; set; }
    public ushort Zone { get; set; }
    public ushort Game { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(AuthenticateReq)) return false;
        if (!ks.Put(NexonAccountInfo)) return false;
        if (!ks.Put(ServerUseKogOTP)) return false;
        if ((region.Region == RegionCode.CN) && !ks.Put(Zone)) return false;
        if ((region.Region == RegionCode.CN) && !ks.Put(Game)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(AuthenticateReq)) return false;
        if (!ks.Get(NexonAccountInfo)) return false;
        if (!ks.Get(out bool v2)) return false; ServerUseKogOTP = v2;
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out ushort v3)) return false; Zone = v3; }
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out ushort v4)) return false; Game = v4; }
        return true;
    }
}

public sealed class KELG_NEXON_USER_LOGIN_TIME_OUT_NOT : IKSerializable
{
    public string UserId { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutWideString(UserId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string s0)) return false; UserId = s0;
        return true;
    }
}

public sealed class KELG_PRESENT_CASH_ITEM_ACK : IKSerializable
{
    public int Ok { get; set; }
    public string ReceiverNickName { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public List<KNXBTPurchaseReqInfo> PurchaseReqInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.PutWideString(ReceiverNickName)) return false;
        if (!ks.PutWideString(Message)) return false;
        if (!ks.PutList(PurchaseReqInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.GetWideString(out string s1)) return false; ReceiverNickName = s1;
        if (!ks.GetWideString(out string s2)) return false; Message = s2;
        if (!ks.GetList(PurchaseReqInfo)) return false;
        return true;
    }
}

public sealed class KELG_PRESENT_CASH_ITEM_REQ : IKSerializable
{
    public uint PwdHash { get; set; }
    public string Password { get; set; } = string.Empty;
    public string ReceiverNickName { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public List<KNXBTPurchaseReqInfo> PurchaseReqInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PwdHash)) return false;
        if (!ks.PutWideString(Password)) return false;
        if (!ks.PutWideString(ReceiverNickName)) return false;
        if (!ks.PutWideString(Message)) return false;
        if (!ks.PutList(PurchaseReqInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint v0)) return false; PwdHash = v0;
        if (!ks.GetWideString(out string s1)) return false; Password = s1;
        if (!ks.GetWideString(out string s2)) return false; ReceiverNickName = s2;
        if (!ks.GetWideString(out string s3)) return false; Message = s3;
        if (!ks.GetList(PurchaseReqInfo)) return false;
        return true;
    }
}

public sealed class KELG_REGISTER_USER_ACK : IKSerializable
{
    public int Ok { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        return true;
    }
}

public sealed class KELG_REGISTER_USER_GLOBAL_PUBLISHER_ACK : IKSerializable
{
    public int Ok { get; set; }
    public string ServiceAccountId { get; set; } = string.Empty;
    public bool IsPcBang { get; set; }
    public int PCBangType { get; set; }
    public int CheckState { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.PutWideString(ServiceAccountId)) return false;
        if ((region.Region == RegionCode.JP) && !ks.Put(IsPcBang)) return false;
        if ((region.Region == RegionCode.JP) && !ks.Put(PCBangType)) return false;
        if ((region.Region == RegionCode.JP) && !ks.Put(CheckState)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.GetWideString(out string s1)) return false; ServiceAccountId = s1;
        if ((region.Region == RegionCode.JP)) { if (!ks.Get(out bool v2)) return false; IsPcBang = v2; }
        if ((region.Region == RegionCode.JP)) { if (!ks.Get(out int v3)) return false; PCBangType = v3; }
        if ((region.Region == RegionCode.JP)) { if (!ks.Get(out int v4)) return false; CheckState = v4; }
        return true;
    }
}

public sealed class KELG_REG_REJECTED_USER_NOT : IKSerializable
{
    public long UserUid { get; set; }
    public sbyte RejectedReason { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UserUid)) return false;
        if (!ks.Put(RejectedReason)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UserUid = v0;
        if (!ks.Get(out sbyte v1)) return false; RejectedReason = v1;
        return true;
    }
}

public sealed class KELG_RELATIONSHIP_INFO_REQ : IKSerializable
{
    public long RequestGSUid { get; set; }
    public long RequestUnitUid { get; set; }
    public long AcceptUnitUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(RequestGSUid)) return false;
        if (!ks.Put(RequestUnitUid)) return false;
        if (!ks.Put(AcceptUnitUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; RequestGSUid = v0;
        if (!ks.Get(out long v1)) return false; RequestUnitUid = v1;
        if (!ks.Get(out long v2)) return false; AcceptUnitUid = v2;
        return true;
    }
}

public sealed class KELG_REQUEST_FRIEND_NOT : IKSerializable
{
    public long DestUnitUid { get; set; }
    public string DestNickName { get; set; } = string.Empty;
    public long UnitUid { get; set; }
    public string NickName { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(DestUnitUid)) return false;
        if (!ks.PutWideString(DestNickName)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutWideString(NickName)) return false;
        if (!ks.PutWideString(Message)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; DestUnitUid = v0;
        if (!ks.GetWideString(out string s1)) return false; DestNickName = s1;
        if (!ks.Get(out long v2)) return false; UnitUid = v2;
        if (!ks.GetWideString(out string s3)) return false; NickName = s3;
        if (!ks.GetWideString(out string s4)) return false; Message = s4;
        return true;
    }
}

public sealed class KELG_REQUEST_TUTORIAL_NOT : IKSerializable
{
    public long ReceiverUnitUid { get; set; }
    public KTutorialUnitInfo TutorialUnitInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ReceiverUnitUid)) return false;
        if (!ks.Put(TutorialUnitInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; ReceiverUnitUid = v0;
        if (!ks.Get(TutorialUnitInfo)) return false;
        return true;
    }
}

public sealed class KELG_SEARCH_BLACK_LIST_UNIT_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long UnitUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(UnitUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        return true;
    }
}

public sealed class KELG_TRADE_BLOCK_NOT : IKSerializable
{
    public bool IsTradeBlock { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(IsTradeBlock)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool v0)) return false; IsTradeBlock = v0;
        return true;
    }
}

public sealed class KELG_UNBLOCK_FRIEND_NOT : IKSerializable
{
    public long DestUnitUid { get; set; }
    public long UnitUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(DestUnitUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; DestUnitUid = v0;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        return true;
    }
}

public sealed class KELG_UPDATE_GUILD_MEMBER_INFO_NOT : IKSerializable
{
    public int GuildUid { get; set; }
    public KGuildMemberInfo MemberInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(GuildUid)) return false;
        if (!ks.Put(MemberInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; GuildUid = v0;
        if (!ks.Get(MemberInfo)) return false;
        return true;
    }
}

public sealed class KELG_VERIFY_GIANT_MATRIX_CARD_REQ : IKSerializable
{
    public string Id { get; set; } = string.Empty;
    public string Matrix { get; set; } = string.Empty;
    public uint GiantMatrixKey { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutWideString(Id)) return false;
        if (!ks.PutWideString(Matrix)) return false;
        if (!ks.Put(GiantMatrixKey)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string s0)) return false; Id = s0;
        if (!ks.GetWideString(out string s1)) return false; Matrix = s1;
        if (!ks.Get(out uint v2)) return false; GiantMatrixKey = v2;
        return true;
    }
}

public sealed class KELG_WEB_RANKING_REFRESH_NOT : IKSerializable
{
    // enum RANKING_REFRESH_TYPE
    public const int DUNGEON_VECTOR = 0;
    public const int DUNGEON_MAP = 1;
    public const int PVP_VECTOR = 2;
    public const int PVP_MAP = 3;

    public bool Init { get; set; }
    public sbyte RankingRefreshType { get; set; }
    public uint TotalSize { get; set; }
    public List<KDungeonRankingInfo> DungeonRanking { get; set; } = new();
    public Dictionary<long, KDungeonRankingInfo> ApDungeonRanking { get; set; } = new();
    public List<KPvpRankingInfo> PvpRanking { get; set; } = new();
    public Dictionary<long, KPvpRankingInfo> ApPvpRanking { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Init)) return false;
        if (!ks.Put(RankingRefreshType)) return false;
        if (!ks.Put(TotalSize)) return false;
        if (!ks.PutList(DungeonRanking)) return false;
        if (!ks.PutMap(ApDungeonRanking, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.PutList(PvpRanking)) return false;
        if (!ks.PutMap(ApPvpRanking, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool v0)) return false; Init = v0;
        if (!ks.Get(out sbyte v1)) return false; RankingRefreshType = v1;
        if (!ks.Get(out uint v2)) return false; TotalSize = v2;
        if (!ks.GetList(DungeonRanking)) return false;
        if (!ks.GetMap(ApDungeonRanking, static s => (s.Get(out long v), v), static s => { var v = new KDungeonRankingInfo(); return (s.Get(v), v); })) return false;
        if (!ks.GetList(PvpRanking)) return false;
        if (!ks.GetMap(ApPvpRanking, static s => (s.Get(out long v), v), static s => { var v = new KPvpRankingInfo(); return (s.Get(v), v); })) return false;
        return true;
    }
}

public sealed class KELOG_ABNORMAL_DISCONNECT : IKSerializable
{
    public long ServerUid { get; set; }
    public Dictionary<uint, int> AbnormalDisconnect { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ServerUid)) return false;
        if (!ks.PutMap(AbnormalDisconnect, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; ServerUid = v0;
        if (!ks.GetMap(AbnormalDisconnect, static s => (s.Get(out uint v), v), static s => (s.Get(out int v), v))) return false;
        return true;
    }
}

public sealed class KELOG_ATTACH_ITEM_ENCHANT_LOG_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public int AttachItemId { get; set; }
    public long DestItemUid { get; set; }
    public int BeforeEnchantLv { get; set; }
    public int AfterEnchantLv { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(AttachItemId)) return false;
        if (!ks.Put(DestItemUid)) return false;
        if (!ks.Put(BeforeEnchantLv)) return false;
        if (!ks.Put(AfterEnchantLv)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out int v1)) return false; AttachItemId = v1;
        if (!ks.Get(out long v2)) return false; DestItemUid = v2;
        if (!ks.Get(out int v3)) return false; BeforeEnchantLv = v3;
        if (!ks.Get(out int v4)) return false; AfterEnchantLv = v4;
        return true;
    }
}

public sealed class KELOG_ATTRIB_ATTACH_ITEM_LOG_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public int AttachItemId { get; set; }
    public long DestItemUid { get; set; }
    public KItemAttributeEnchantInfo BeforeAttribEnchantInfo { get; set; } = new();
    public KItemAttributeEnchantInfo AfterAttribEnchantInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(AttachItemId)) return false;
        if (!ks.Put(DestItemUid)) return false;
        if (!ks.Put(BeforeAttribEnchantInfo)) return false;
        if (!ks.Put(AfterAttribEnchantInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out int v1)) return false; AttachItemId = v1;
        if (!ks.Get(out long v2)) return false; DestItemUid = v2;
        if (!ks.Get(BeforeAttribEnchantInfo)) return false;
        if (!ks.Get(AfterAttribEnchantInfo)) return false;
        return true;
    }
}

public sealed class KELOG_BATTLE_FIELD_LEAVE_LOG_NOT : IKSerializable
{
    // enum LEAVE_TYPE
    public const int LT_NONE = 0;
    public const int LT_DISCONNECT = 1;
    public const int LT_CHANNEL_CHANGE = 2;
    public const int LT_WALK_TO_DIF_FIELD = 3;
    public const int LT_USE_WARP_ITEM = 4;
    public const int LT_MOVE_PARTY_FIELD = 5;
    public const int LT_ENTER_THE_DUNGEON = 6;
    public const int LT_ENTER_THE_PVP = 7;
    public const int LT_ABNORMAL_DISCONNECT = 8;
    public const int LT_CLIENT_CRASH = 9;

    public string ServerIP { get; set; } = string.Empty;
    public int BattleFieldId { get; set; }
    public long RoomUid { get; set; }
    public sbyte LeaveType { get; set; }
    public long UnitUid { get; set; }
    public string NickName { get; set; } = string.Empty;
    public sbyte UnitClass { get; set; }
    public byte Level { get; set; }
    public int PlayTime { get; set; }
    public int GetEXP { get; set; }
    public int GetED { get; set; }
    public string RegDate { get; set; } = string.Empty;
    public int FieldFrame { get; set; }
    public short NpcDieCount { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutWideString(ServerIP)) return false;
        if (!ks.Put(BattleFieldId)) return false;
        if (!ks.Put(RoomUid)) return false;
        if (!ks.Put(LeaveType)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutWideString(NickName)) return false;
        if (!ks.Put(UnitClass)) return false;
        if (!ks.Put(Level)) return false;
        if (!ks.Put(PlayTime)) return false;
        if (!ks.Put(GetEXP)) return false;
        if (!ks.Put(GetED)) return false;
        if (!ks.PutWideString(RegDate)) return false;
        if (!ks.Put(FieldFrame)) return false;
        if (!ks.Put(NpcDieCount)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string s0)) return false; ServerIP = s0;
        if (!ks.Get(out int v1)) return false; BattleFieldId = v1;
        if (!ks.Get(out long v2)) return false; RoomUid = v2;
        if (!ks.Get(out sbyte v3)) return false; LeaveType = v3;
        if (!ks.Get(out long v4)) return false; UnitUid = v4;
        if (!ks.GetWideString(out string s5)) return false; NickName = s5;
        if (!ks.Get(out sbyte v6)) return false; UnitClass = v6;
        if (!ks.Get(out byte v7)) return false; Level = v7;
        if (!ks.Get(out int v8)) return false; PlayTime = v8;
        if (!ks.Get(out int v9)) return false; GetEXP = v9;
        if (!ks.Get(out int v10)) return false; GetED = v10;
        if (!ks.GetWideString(out string s11)) return false; RegDate = s11;
        if (!ks.Get(out int v12)) return false; FieldFrame = v12;
        if (!ks.Get(out short v13)) return false; NpcDieCount = v13;
        return true;
    }
}

public sealed class KELOG_BUY_CASH_ITEM_LOG_NOT : IKSerializable
{
    // enum CASH_ITEM_BUY_TYPE
    public const int CIBT_BUY = 0;
    public const int CIBT_PRESENT = 1;

    public long UserUid { get; set; }
    public int ServerGroupId { get; set; }
    public long UnitUid { get; set; }
    public sbyte UnitClass { get; set; }
    public byte Level { get; set; }
    public bool IsFirstBuy { get; set; }
    public sbyte BuyType { get; set; }
    public int ProductNo { get; set; }
    public int BuyQuantity { get; set; }
    public int TotalSalePrice { get; set; }
    public string RegDate { get; set; } = string.Empty;
    public string ReceiverUserId { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UserUid)) return false;
        if (!ks.Put(ServerGroupId)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(UnitClass)) return false;
        if (!ks.Put(Level)) return false;
        if (!ks.Put(IsFirstBuy)) return false;
        if (!ks.Put(BuyType)) return false;
        if (!ks.Put(ProductNo)) return false;
        if (!ks.Put(BuyQuantity)) return false;
        if (!ks.Put(TotalSalePrice)) return false;
        if (!ks.PutWideString(RegDate)) return false;
        if (!ks.PutWideString(ReceiverUserId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UserUid = v0;
        if (!ks.Get(out int v1)) return false; ServerGroupId = v1;
        if (!ks.Get(out long v2)) return false; UnitUid = v2;
        if (!ks.Get(out sbyte v3)) return false; UnitClass = v3;
        if (!ks.Get(out byte v4)) return false; Level = v4;
        if (!ks.Get(out bool v5)) return false; IsFirstBuy = v5;
        if (!ks.Get(out sbyte v6)) return false; BuyType = v6;
        if (!ks.Get(out int v7)) return false; ProductNo = v7;
        if (!ks.Get(out int v8)) return false; BuyQuantity = v8;
        if (!ks.Get(out int v9)) return false; TotalSalePrice = v9;
        if (!ks.GetWideString(out string s10)) return false; RegDate = s10;
        if (!ks.GetWideString(out string s11)) return false; ReceiverUserId = s11;
        return true;
    }
}

public sealed class KELOG_CASH_ITEM_RESOLVE_LOG_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public long ItemUid { get; set; }
    public int ResultQuantity { get; set; }
    public int AfterItemId { get; set; }
    public int AfterItemQuantity { get; set; }
    public int Level { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(ItemUid)) return false;
        if (!ks.Put(ResultQuantity)) return false;
        if ((region.Region == RegionCode.JP) && !ks.Put(AfterItemId)) return false;
        if ((region.Region == RegionCode.JP) && !ks.Put(AfterItemQuantity)) return false;
        if ((region.Region == RegionCode.JP) && !ks.Put(Level)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out long v1)) return false; ItemUid = v1;
        if (!ks.Get(out int v2)) return false; ResultQuantity = v2;
        if ((region.Region == RegionCode.JP)) { if (!ks.Get(out int v3)) return false; AfterItemId = v3; }
        if ((region.Region == RegionCode.JP)) { if (!ks.Get(out int v4)) return false; AfterItemQuantity = v4; }
        if ((region.Region == RegionCode.JP)) { if (!ks.Get(out int v5)) return false; Level = v5; }
        return true;
    }
}

public sealed class KELOG_CASH_PRODUCT_LIST_UPDATE_NOT : IKSerializable
{
    public List<KCashPruductInfo> CashProductList { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutList(CashProductList)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetList(CashProductList)) return false;
        return true;
    }
}

public sealed class KELOG_CHARM_ITEM_LOG_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public int CharmItemId { get; set; }
    public int ResultItemId { get; set; }
    public int Quantity { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(CharmItemId)) return false;
        if (!ks.Put(ResultItemId)) return false;
        if (!ks.Put(Quantity)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out int v1)) return false; CharmItemId = v1;
        if (!ks.Get(out int v2)) return false; ResultItemId = v2;
        if (!ks.Get(out int v3)) return false; Quantity = v3;
        return true;
    }
}

public sealed class KELOG_CHAT_LOG_NOT : IKSerializable
{
    public long RoomUid { get; set; }
    public long CharUid { get; set; }
    public long UserUid { get; set; }
    public sbyte ChatType { get; set; }
    public string Msg { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(RoomUid)) return false;
        if (!ks.Put(CharUid)) return false;
        if (!ks.Put(UserUid)) return false;
        if (!ks.Put(ChatType)) return false;
        if (!ks.PutWideString(Msg)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; RoomUid = v0;
        if (!ks.Get(out long v1)) return false; CharUid = v1;
        if (!ks.Get(out long v2)) return false; UserUid = v2;
        if (!ks.Get(out sbyte v3)) return false; ChatType = v3;
        if (!ks.GetWideString(out string s4)) return false; Msg = s4;
        return true;
    }
}

public sealed class KELOG_CHEAT_LOG_NOT : IKSerializable
{
    public long UserUid { get; set; }
    public long UnitUid { get; set; }
    public sbyte AuthLevel { get; set; }
    public string WsCheatCode { get; set; } = string.Empty;
    public string WsCheatContents { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UserUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(AuthLevel)) return false;
        if (!ks.PutWideString(WsCheatCode)) return false;
        if (!ks.PutWideString(WsCheatContents)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UserUid = v0;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        if (!ks.Get(out sbyte v2)) return false; AuthLevel = v2;
        if (!ks.GetWideString(out string s3)) return false; WsCheatCode = s3;
        if (!ks.GetWideString(out string s4)) return false; WsCheatContents = s4;
        return true;
    }
}

public sealed class KELOG_COLLECTION_OF_RELAY_AND_P2P_INFO_NOT : IKSerializable
{
    public List<KDBUpdateGamePlayNetworkInfo> DBUpdateInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutList(DBUpdateInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetList(DBUpdateInfo)) return false;
        return true;
    }
}

public sealed class KELOG_ED_ABUSER_LOG_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public int State { get; set; }
    public int BeforeED { get; set; }
    public int AfterED { get; set; }
    public int Ed { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(State)) return false;
        if (!ks.Put(BeforeED)) return false;
        if (!ks.Put(AfterED)) return false;
        if (!ks.Put(Ed)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out int v1)) return false; State = v1;
        if (!ks.Get(out int v2)) return false; BeforeED = v2;
        if (!ks.Get(out int v3)) return false; AfterED = v3;
        if (!ks.Get(out int v4)) return false; Ed = v4;
        return true;
    }
}

public sealed class KELOG_ENCHANT_ABUSER_LOG_NOT : IKSerializable
{
    public long UserUid { get; set; }
    public long UnitUid { get; set; }
    public long ItemUid { get; set; }
    public int ItemId { get; set; }
    public sbyte EnchantLevel { get; set; }
    public int EnchantResult { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UserUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(ItemUid)) return false;
        if (!ks.Put(ItemId)) return false;
        if (!ks.Put(EnchantLevel)) return false;
        if (!ks.Put(EnchantResult)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UserUid = v0;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        if (!ks.Get(out long v2)) return false; ItemUid = v2;
        if (!ks.Get(out int v3)) return false; ItemId = v3;
        if (!ks.Get(out sbyte v4)) return false; EnchantLevel = v4;
        if (!ks.Get(out int v5)) return false; EnchantResult = v5;
        return true;
    }
}

public sealed class KELOG_ENCHANT_ITEM_BREAK_LOG_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public long ItemUid { get; set; }
    public int ItemId { get; set; }
    public sbyte EnchantLevel { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(ItemUid)) return false;
        if (!ks.Put(ItemId)) return false;
        if (!ks.Put(EnchantLevel)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out long v1)) return false; ItemUid = v1;
        if (!ks.Get(out int v2)) return false; ItemId = v2;
        if (!ks.Get(out sbyte v3)) return false; EnchantLevel = v3;
        return true;
    }
}

public sealed class KELOG_ENCHANT_SUPPORT_MATERIAL_LOG_NOT : IKSerializable
{
    public long ItemUid { get; set; }
    public sbyte BeforeLevel { get; set; }
    public sbyte AfterLevel { get; set; }
    public bool UseSupport { get; set; }
    public bool UsePlus { get; set; }
    public bool UseDestroyGuard { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ItemUid)) return false;
        if (!ks.Put(BeforeLevel)) return false;
        if (!ks.Put(AfterLevel)) return false;
        if ((region.Region == RegionCode.TW) && !ks.Put(UseSupport)) return false;
        if ((region.Region == RegionCode.TW) && !ks.Put(UsePlus)) return false;
        if ((region.Region == RegionCode.TW) && !ks.Put(UseDestroyGuard)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; ItemUid = v0;
        if (!ks.Get(out sbyte v1)) return false; BeforeLevel = v1;
        if (!ks.Get(out sbyte v2)) return false; AfterLevel = v2;
        if ((region.Region == RegionCode.TW)) { if (!ks.Get(out bool v3)) return false; UseSupport = v3; }
        if ((region.Region == RegionCode.TW)) { if (!ks.Get(out bool v4)) return false; UsePlus = v4; }
        if ((region.Region == RegionCode.TW)) { if (!ks.Get(out bool v5)) return false; UseDestroyGuard = v5; }
        return true;
    }
}

public sealed class KELOG_EXPAND_BANK_INVENTORY_NOT : IKSerializable
{
    public long UserUid { get; set; }
    public long UnitUid { get; set; }
    public int UnitLevel { get; set; }
    public int BeforeED { get; set; }
    public int UseED { get; set; }
    public int Category { get; set; }
    public int NumSlot { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UserUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(UnitLevel)) return false;
        if (!ks.Put(BeforeED)) return false;
        if (!ks.Put(UseED)) return false;
        if (!ks.Put(Category)) return false;
        if (!ks.Put(NumSlot)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UserUid = v0;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        if (!ks.Get(out int v2)) return false; UnitLevel = v2;
        if (!ks.Get(out int v3)) return false; BeforeED = v3;
        if (!ks.Get(out int v4)) return false; UseED = v4;
        if (!ks.Get(out int v5)) return false; Category = v5;
        if (!ks.Get(out int v6)) return false; NumSlot = v6;
        return true;
    }
}

public sealed class KELOG_EXPAND_SKILL_PAGE_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public byte Level { get; set; }
    public byte UnitClass { get; set; }
    public byte BeforeTheNumberOfSkillPagesAvailable { get; set; }
    public byte PaymentType { get; set; }
    public string SenderNickName { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(Level)) return false;
        if (!ks.Put(UnitClass)) return false;
        if (!ks.Put(BeforeTheNumberOfSkillPagesAvailable)) return false;
        if (!ks.Put(PaymentType)) return false;
        if (!ks.PutWideString(SenderNickName)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out byte v1)) return false; Level = v1;
        if (!ks.Get(out byte v2)) return false; UnitClass = v2;
        if (!ks.Get(out byte v3)) return false; BeforeTheNumberOfSkillPagesAvailable = v3;
        if (!ks.Get(out byte v4)) return false; PaymentType = v4;
        if (!ks.GetWideString(out string s5)) return false; SenderNickName = s5;
        return true;
    }
}

public sealed class KELOG_GET_PURCHASED_CASH_ITEM_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public int ItemId { get; set; }
    public short Period { get; set; }
    public int Quantity { get; set; }
    public byte Level { get; set; }
    public sbyte UnitClass { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(ItemId)) return false;
        if (!ks.Put(Period)) return false;
        if (!ks.Put(Quantity)) return false;
        if (!ks.Put(Level)) return false;
        if (!ks.Put(UnitClass)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out int v1)) return false; ItemId = v1;
        if (!ks.Get(out short v2)) return false; Period = v2;
        if (!ks.Get(out int v3)) return false; Quantity = v3;
        if (!ks.Get(out byte v4)) return false; Level = v4;
        if (!ks.Get(out sbyte v5)) return false; UnitClass = v5;
        return true;
    }
}

public sealed class KELOG_HENIR_DUNGEON_CLEAR_NOT : IKSerializable
{
    public long UserUid { get; set; }
    public long UnitUid { get; set; }
    public int PlayTime { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UserUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(PlayTime)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UserUid = v0;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        if (!ks.Get(out int v2)) return false; PlayTime = v2;
        return true;
    }
}

public sealed class KELOG_HENIR_HACKING_LOG_NOT : IKSerializable
{
    public List<KHenirHackUserData> HackUser { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutList(HackUser)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetList(HackUser)) return false;
        return true;
    }
}

public sealed class KELOG_HIGH_ENCHANT_ITEM_LOG_NOT : IKSerializable
{
    // enum ENCHANT_ITEM_RESULT
    public const int EIS_START_LOG_LEVEL = 7;
    public const int EIS_NONE = 0;
    public const int EIS_SUCCESS_ENCHANT = 1;
    public const int EIS_BREAK_ITEM = 2;
    public const int EIS_NO_CHANGE = 3;
    public const int EIS_DOWN_ENCHANT = 4;
    public const int EIS_INIT_ITEM = 5;

    public long UnitUid { get; set; }
    public long ItemUid { get; set; }
    public int ItemId { get; set; }
    public int BeforeLevel { get; set; }
    public int AfterLevel { get; set; }
    public bool SupportMaterial { get; set; }
    public int EnchantResult { get; set; }
    public string Time { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(ItemUid)) return false;
        if (!ks.Put(ItemId)) return false;
        if (!ks.Put(BeforeLevel)) return false;
        if (!ks.Put(AfterLevel)) return false;
        if (!ks.Put(SupportMaterial)) return false;
        if (!ks.Put(EnchantResult)) return false;
        if (!ks.PutWideString(Time)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out long v1)) return false; ItemUid = v1;
        if (!ks.Get(out int v2)) return false; ItemId = v2;
        if (!ks.Get(out int v3)) return false; BeforeLevel = v3;
        if (!ks.Get(out int v4)) return false; AfterLevel = v4;
        if (!ks.Get(out bool v5)) return false; SupportMaterial = v5;
        if (!ks.Get(out int v6)) return false; EnchantResult = v6;
        if (!ks.GetWideString(out string s7)) return false; Time = s7;
        return true;
    }
}

public sealed class KELOG_INIT_SKILL_TREE_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public int SPoint { get; set; }
    public int CSPoint { get; set; }
    public long ItemUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(SPoint)) return false;
        if (!ks.Put(CSPoint)) return false;
        if (!ks.Put(ItemUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out int v1)) return false; SPoint = v1;
        if (!ks.Get(out int v2)) return false; CSPoint = v2;
        if (!ks.Get(out long v3)) return false; ItemUid = v3;
        return true;
    }
}

public sealed class KELOG_INSERT_ABUSER_LOG_NOT : IKSerializable
{
    public long UserUid { get; set; }
    public long UnitUid { get; set; }
    public string Ip { get; set; } = string.Empty;
    public int GameType { get; set; }
    public float PlayTimeLimit { get; set; }
    public bool Public { get; set; }
    public int PlayWithMember { get; set; }
    public string EndGameTime { get; set; } = string.Empty;
    public int PlayTime { get; set; }
    public int RewardVP { get; set; }
    public int RewardEXP { get; set; }
    public int KillCount { get; set; }
    public int DeathCount { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UserUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutWideString(Ip)) return false;
        if (!ks.Put(GameType)) return false;
        if (!ks.Put(PlayTimeLimit)) return false;
        if (!ks.Put(Public)) return false;
        if (!ks.Put(PlayWithMember)) return false;
        if (!ks.PutWideString(EndGameTime)) return false;
        if (!ks.Put(PlayTime)) return false;
        if (!ks.Put(RewardVP)) return false;
        if (!ks.Put(RewardEXP)) return false;
        if (!ks.Put(KillCount)) return false;
        if (!ks.Put(DeathCount)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UserUid = v0;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        if (!ks.GetWideString(out string s2)) return false; Ip = s2;
        if (!ks.Get(out int v3)) return false; GameType = v3;
        if (!ks.Get(out float v4)) return false; PlayTimeLimit = v4;
        if (!ks.Get(out bool v5)) return false; Public = v5;
        if (!ks.Get(out int v6)) return false; PlayWithMember = v6;
        if (!ks.GetWideString(out string s7)) return false; EndGameTime = s7;
        if (!ks.Get(out int v8)) return false; PlayTime = v8;
        if (!ks.Get(out int v9)) return false; RewardVP = v9;
        if (!ks.Get(out int v10)) return false; RewardEXP = v10;
        if (!ks.Get(out int v11)) return false; KillCount = v11;
        if (!ks.Get(out int v12)) return false; DeathCount = v12;
        return true;
    }
}

public sealed class KELOG_INSERT_CASH_SKILL_POINT_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public int UnitLevel { get; set; }
    public int MaxCSPoint { get; set; }
    public int Period { get; set; }
    public bool UpdateEndDateOnly { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(UnitLevel)) return false;
        if (!ks.Put(MaxCSPoint)) return false;
        if (!ks.Put(Period)) return false;
        if (!ks.Put(UpdateEndDateOnly)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out int v1)) return false; UnitLevel = v1;
        if (!ks.Get(out int v2)) return false; MaxCSPoint = v2;
        if (!ks.Get(out int v3)) return false; Period = v3;
        if (!ks.Get(out bool v4)) return false; UpdateEndDateOnly = v4;
        return true;
    }
}

public sealed class KELOG_INSERT_TRADE_LOG_NOT : IKSerializable
{
    public long FromUserUid { get; set; }
    public long FromUnitUid { get; set; }
    public long ToUserUid { get; set; }
    public long ToUnitUid { get; set; }
    public long ItemUid { get; set; }
    public int ItemId { get; set; }
    public int Quantity { get; set; }
    public string RegDate { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(FromUserUid)) return false;
        if (!ks.Put(FromUnitUid)) return false;
        if (!ks.Put(ToUserUid)) return false;
        if (!ks.Put(ToUnitUid)) return false;
        if (!ks.Put(ItemUid)) return false;
        if (!ks.Put(ItemId)) return false;
        if (!ks.Put(Quantity)) return false;
        if (!ks.PutWideString(RegDate)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; FromUserUid = v0;
        if (!ks.Get(out long v1)) return false; FromUnitUid = v1;
        if (!ks.Get(out long v2)) return false; ToUserUid = v2;
        if (!ks.Get(out long v3)) return false; ToUnitUid = v3;
        if (!ks.Get(out long v4)) return false; ItemUid = v4;
        if (!ks.Get(out int v5)) return false; ItemId = v5;
        if (!ks.Get(out int v6)) return false; Quantity = v6;
        if (!ks.GetWideString(out string s7)) return false; RegDate = s7;
        return true;
    }
}

public sealed class KELOG_ITEM_ABUSER_LOG_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public List<KItemAbuserLogInfo> ItemAbuserLog { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutList(ItemAbuserLog)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.GetList(ItemAbuserLog)) return false;
        return true;
    }
}

public sealed class KELOG_ITEM_EXCHANGE_LOG_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public long DeletedItemUid { get; set; }
    public int DeletedItemId { get; set; }
    public long ChangedItemUid { get; set; }
    public int ChangedItemId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(DeletedItemUid)) return false;
        if (!ks.Put(DeletedItemId)) return false;
        if (!ks.Put(ChangedItemUid)) return false;
        if (!ks.Put(ChangedItemId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out long v1)) return false; DeletedItemUid = v1;
        if (!ks.Get(out int v2)) return false; DeletedItemId = v2;
        if (!ks.Get(out long v3)) return false; ChangedItemUid = v3;
        if (!ks.Get(out int v4)) return false; ChangedItemId = v4;
        return true;
    }
}

public sealed class KELOG_ITEM_TRADE_ERROR_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public int TypeVal { get; set; }
    public string WsrtRegDate { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(TypeVal)) return false;
        if (!ks.PutWideString(WsrtRegDate)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out int v1)) return false; TypeVal = v1;
        if (!ks.GetWideString(out string s2)) return false; WsrtRegDate = s2;
        return true;
    }
}

public sealed class KELOG_KOM_FILE_CHECK_LOG_NOT : IKSerializable
{
    public string InvaildKomName { get; set; } = string.Empty;
    public long UserUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutWideString(InvaildKomName)) return false;
        if (!ks.Put(UserUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string s0)) return false; InvaildKomName = s0;
        if (!ks.Get(out long v1)) return false; UserUid = v1;
        return true;
    }
}

public sealed class KELOG_MEGAPHONE_LOG_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public int ItemId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(ItemId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out int v1)) return false; ItemId = v1;
        return true;
    }
}

public sealed class KELOG_RESOLVE_JACKPOT_LOG_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public int ResolveItemId { get; set; }
    public int ElapsedTime { get; set; }
    public string RegDate { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(ResolveItemId)) return false;
        if (!ks.Put(ElapsedTime)) return false;
        if (!ks.PutWideString(RegDate)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out int v1)) return false; ResolveItemId = v1;
        if (!ks.Get(out int v2)) return false; ElapsedTime = v2;
        if (!ks.GetWideString(out string s3)) return false; RegDate = s3;
        return true;
    }
}

public sealed class KELOG_RESTORE_ITEM_LOG_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public long ItemUid { get; set; }
    public int BeforeEnchantLv { get; set; }
    public int AfterEnchantLv { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(ItemUid)) return false;
        if (!ks.Put(BeforeEnchantLv)) return false;
        if (!ks.Put(AfterEnchantLv)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out long v1)) return false; ItemUid = v1;
        if (!ks.Get(out int v2)) return false; BeforeEnchantLv = v2;
        if (!ks.Get(out int v3)) return false; AfterEnchantLv = v3;
        return true;
    }
}

public sealed class KELOG_RESURRECTION_STONE_LOG_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public int DungeonId { get; set; }
    public string RegDate { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(DungeonId)) return false;
        if (!ks.PutWideString(RegDate)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out int v1)) return false; DungeonId = v1;
        if (!ks.GetWideString(out string s2)) return false; RegDate = s2;
        return true;
    }
}

public sealed class KELOG_STAT_PLAY_TIME : IKSerializable
{
    public long UserUid { get; set; }
    public string Ip { get; set; } = string.Empty;
    public string LoginTime { get; set; } = string.Empty;
    public string LogoutTime { get; set; } = string.Empty;
    public bool LoginFail { get; set; }
    public bool LogoutFail { get; set; }
    public int TotalPlayTime { get; set; }
    public int PvpPlayTime { get; set; }
    public int DungeonPlayTime { get; set; }
    public int FieldPlayTime { get; set; }
    public int DisconnectReason { get; set; }
    public int DisconnectUserFSM { get; set; }
    public int ServerGroupId { get; set; }
    public string MachineId { get; set; } = string.Empty;
    public bool IsPcbang { get; set; }
    public byte ChannelCode { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UserUid)) return false;
        if (!ks.PutWideString(Ip)) return false;
        if (!ks.PutWideString(LoginTime)) return false;
        if (!ks.PutWideString(LogoutTime)) return false;
        if (!ks.Put(LoginFail)) return false;
        if (!ks.Put(LogoutFail)) return false;
        if (!ks.Put(TotalPlayTime)) return false;
        if (!ks.Put(PvpPlayTime)) return false;
        if (!ks.Put(DungeonPlayTime)) return false;
        if (!ks.Put(FieldPlayTime)) return false;
        if (!ks.Put(DisconnectReason)) return false;
        if (!ks.Put(DisconnectUserFSM)) return false;
        if (!ks.Put(ServerGroupId)) return false;
        if (!ks.PutWideString(MachineId)) return false;
        if (!ks.Put(IsPcbang)) return false;
        if ((region.Region == RegionCode.JP) && !ks.Put(ChannelCode)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UserUid = v0;
        if (!ks.GetWideString(out string s1)) return false; Ip = s1;
        if (!ks.GetWideString(out string s2)) return false; LoginTime = s2;
        if (!ks.GetWideString(out string s3)) return false; LogoutTime = s3;
        if (!ks.Get(out bool v4)) return false; LoginFail = v4;
        if (!ks.Get(out bool v5)) return false; LogoutFail = v5;
        if (!ks.Get(out int v6)) return false; TotalPlayTime = v6;
        if (!ks.Get(out int v7)) return false; PvpPlayTime = v7;
        if (!ks.Get(out int v8)) return false; DungeonPlayTime = v8;
        if (!ks.Get(out int v9)) return false; FieldPlayTime = v9;
        if (!ks.Get(out int v10)) return false; DisconnectReason = v10;
        if (!ks.Get(out int v11)) return false; DisconnectUserFSM = v11;
        if (!ks.Get(out int v12)) return false; ServerGroupId = v12;
        if (!ks.GetWideString(out string s13)) return false; MachineId = s13;
        if (!ks.Get(out bool v14)) return false; IsPcbang = v14;
        if ((region.Region == RegionCode.JP)) { if (!ks.Get(out byte v15)) return false; ChannelCode = v15; }
        return true;
    }
}

public sealed class KELOG_TRAFFIC_ATTACKER_LOG_NOT : IKSerializable
{
    public string UserId { get; set; } = string.Empty;
    public int ChannelId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutWideString(UserId)) return false;
        if (!ks.Put(ChannelId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string s0)) return false; UserId = s0;
        if (!ks.Get(out int v1)) return false; ChannelId = v1;
        return true;
    }
}

public sealed class KELOG_UPDATE_STONE_NOT : IKSerializable
{
    public int SupplyCnt { get; set; }
    public int Quantity { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.JP || region.Region == RegionCode.TW) && !ks.Put(SupplyCnt)) return false;
        if ((region.Region == RegionCode.JP || region.Region == RegionCode.TW) && !ks.Put(Quantity)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.JP || region.Region == RegionCode.TW)) { if (!ks.Get(out int v0)) return false; SupplyCnt = v0; }
        if ((region.Region == RegionCode.JP || region.Region == RegionCode.TW)) { if (!ks.Get(out int v1)) return false; Quantity = v1; }
        return true;
    }
}

public sealed class KELOG_USER_CHARACTER_LOG_NOT : IKSerializable
{
    // enum LOG_TYPE
    public const int LT_NONE = 0;
    public const int LT_CREATE_CHARACTER = 1;
    public const int LT_FIRST_QUEST_COMPLETE = 2;
    public const int LT_FIRST_DUNGEON_PLAY = 3;
    public const int LT_DUNGEON_PLAY_COMPLETE = 4;
    public const int LT_FIRST_PVP = 5;
    public const int LT_FIRST_USE_SKILL_POINT = 6;
    public const int LT_FIRST_DESELECT = 7;
    public const int LT_SECOND_SELECT = 8;
    public const int LT_SECOND_DESELECT = 9;
    public const int LT_THIRD_SELECT = 10;
    public const int LT_FIRST_JOB_CHANGE = 11;
    public const int LT_SECOND_JOB_CHANGE = 12;
    public const int LT_FIRST_USE_CASH = 13;
    public const int LT_LEVEL_UP = 14;
    public const int LT_FULL_LEVEL = 15;
    public const int LT_DELETE_CHARACTER = 16;
    public const int LT_JOIN_GUILD_SELECT_CHAR = 17;
    public const int LT_FIRST_BUY_CASH_ITEM = 18;
    public const int LT_DAILY_CHAR_FIRST_SELECT = 19;
    public const int LT_MAX = 20;

    public long UserUid { get; set; }
    public long UnitUid { get; set; }
    public string NickName { get; set; } = string.Empty;
    public bool OnlyOneUnit { get; set; }
    public long ServerUid { get; set; }
    public short LogType { get; set; }
    public byte Level { get; set; }
    public sbyte UnitClass { get; set; }
    public int DungeonId { get; set; }
    public int ProductNo { get; set; }
    public int TotalDungeonPlayTime { get; set; }
    public int TotalPvpPlayTime { get; set; }
    public int DurationTime { get; set; }
    public string AccountCreateDate { get; set; } = string.Empty;
    public string UnitCreateDate { get; set; } = string.Empty;
    public string RegDate { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UserUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutWideString(NickName)) return false;
        if (!ks.Put(OnlyOneUnit)) return false;
        if (!ks.Put(ServerUid)) return false;
        if (!ks.Put(LogType)) return false;
        if (!ks.Put(Level)) return false;
        if (!ks.Put(UnitClass)) return false;
        if (!ks.Put(DungeonId)) return false;
        if (!ks.Put(ProductNo)) return false;
        if (!ks.Put(TotalDungeonPlayTime)) return false;
        if (!ks.Put(TotalPvpPlayTime)) return false;
        if (!ks.Put(DurationTime)) return false;
        if (!ks.PutWideString(AccountCreateDate)) return false;
        if (!ks.PutWideString(UnitCreateDate)) return false;
        if (!ks.PutWideString(RegDate)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UserUid = v0;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        if (!ks.GetWideString(out string s2)) return false; NickName = s2;
        if (!ks.Get(out bool v3)) return false; OnlyOneUnit = v3;
        if (!ks.Get(out long v4)) return false; ServerUid = v4;
        if (!ks.Get(out short v5)) return false; LogType = v5;
        if (!ks.Get(out byte v6)) return false; Level = v6;
        if (!ks.Get(out sbyte v7)) return false; UnitClass = v7;
        if (!ks.Get(out int v8)) return false; DungeonId = v8;
        if (!ks.Get(out int v9)) return false; ProductNo = v9;
        if (!ks.Get(out int v10)) return false; TotalDungeonPlayTime = v10;
        if (!ks.Get(out int v11)) return false; TotalPvpPlayTime = v11;
        if (!ks.Get(out int v12)) return false; DurationTime = v12;
        if (!ks.GetWideString(out string s13)) return false; AccountCreateDate = s13;
        if (!ks.GetWideString(out string s14)) return false; UnitCreateDate = s14;
        if (!ks.GetWideString(out string s15)) return false; RegDate = s15;
        return true;
    }
}

public sealed class KELOG_USER_STATISTICS : IKSerializable
{
    public long UserUid { get; set; }
    public long UnitUid { get; set; }
    public Dictionary<(int, int), List<int>> PvpStatistics { get; set; } = new();
    public Dictionary<(int, int), List<int>> DungeonStatistics { get; set; } = new();
    public Dictionary<(int, int), List<int>> TCenterStatistics { get; set; } = new();
    public Dictionary<(int, int), List<int>> EtcStatistics { get; set; } = new();
    public Dictionary<(int, int), List<int>> EDStatistics { get; set; } = new();
    public int AuthLevel { get; set; }
    public sbyte UnitClass { get; set; }
    public byte Level { get; set; }
    public int Ed { get; set; }
    public int VSPoint { get; set; }
    public int VSPointMax { get; set; }
    public string LoginTime { get; set; } = string.Empty;
    public string LogoutTime { get; set; } = string.Empty;
    public string NickName { get; set; } = string.Empty;
    public uint GiantUid { get; set; }
    public string AccountCreateDate { get; set; } = string.Empty;
    public int Exp { get; set; }
    public string UnitCreateDate { get; set; } = string.Empty;
    public int IiDurationTime { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UserUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutMap(PvpStatistics, static (s, k) => s.Put(k.Item1) && s.Put(k.Item2), static (s, v) => s.PutList(v, static (s2, x) => s2.Put(x)))) return false;
        if (!ks.PutMap(DungeonStatistics, static (s, k) => s.Put(k.Item1) && s.Put(k.Item2), static (s, v) => s.PutList(v, static (s2, x) => s2.Put(x)))) return false;
        if (!ks.PutMap(TCenterStatistics, static (s, k) => s.Put(k.Item1) && s.Put(k.Item2), static (s, v) => s.PutList(v, static (s2, x) => s2.Put(x)))) return false;
        if (!ks.PutMap(EtcStatistics, static (s, k) => s.Put(k.Item1) && s.Put(k.Item2), static (s, v) => s.PutList(v, static (s2, x) => s2.Put(x)))) return false;
        if (!ks.PutMap(EDStatistics, static (s, k) => s.Put(k.Item1) && s.Put(k.Item2), static (s, v) => s.PutList(v, static (s2, x) => s2.Put(x)))) return false;
        if (!ks.Put(AuthLevel)) return false;
        if (!ks.Put(UnitClass)) return false;
        if (!ks.Put(Level)) return false;
        if (!ks.Put(Ed)) return false;
        if (!ks.Put(VSPoint)) return false;
        if (!ks.Put(VSPointMax)) return false;
        if (!ks.PutWideString(LoginTime)) return false;
        if (!ks.PutWideString(LogoutTime)) return false;
        if (!ks.PutWideString(NickName)) return false;
        if (!ks.Put(GiantUid)) return false;
        if (!ks.PutWideString(AccountCreateDate)) return false;
        if (!ks.Put(Exp)) return false;
        if (!ks.PutWideString(UnitCreateDate)) return false;
        if (!ks.Put(IiDurationTime)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UserUid = v0;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        if (!ks.GetMap(PvpStatistics, static s => { bool ok1 = s.Get(out int a); bool ok2 = s.Get(out int b); return (ok1 && ok2, (a, b)); }, static s => { var l = new List<int>(); return (s.GetList(l, static s2 => (s2.Get(out int x), x)), l); })) return false;
        if (!ks.GetMap(DungeonStatistics, static s => { bool ok1 = s.Get(out int a); bool ok2 = s.Get(out int b); return (ok1 && ok2, (a, b)); }, static s => { var l = new List<int>(); return (s.GetList(l, static s2 => (s2.Get(out int x), x)), l); })) return false;
        if (!ks.GetMap(TCenterStatistics, static s => { bool ok1 = s.Get(out int a); bool ok2 = s.Get(out int b); return (ok1 && ok2, (a, b)); }, static s => { var l = new List<int>(); return (s.GetList(l, static s2 => (s2.Get(out int x), x)), l); })) return false;
        if (!ks.GetMap(EtcStatistics, static s => { bool ok1 = s.Get(out int a); bool ok2 = s.Get(out int b); return (ok1 && ok2, (a, b)); }, static s => { var l = new List<int>(); return (s.GetList(l, static s2 => (s2.Get(out int x), x)), l); })) return false;
        if (!ks.GetMap(EDStatistics, static s => { bool ok1 = s.Get(out int a); bool ok2 = s.Get(out int b); return (ok1 && ok2, (a, b)); }, static s => { var l = new List<int>(); return (s.GetList(l, static s2 => (s2.Get(out int x), x)), l); })) return false;
        if (!ks.Get(out int v7)) return false; AuthLevel = v7;
        if (!ks.Get(out sbyte v8)) return false; UnitClass = v8;
        if (!ks.Get(out byte v9)) return false; Level = v9;
        if (!ks.Get(out int v10)) return false; Ed = v10;
        if (!ks.Get(out int v11)) return false; VSPoint = v11;
        if (!ks.Get(out int v12)) return false; VSPointMax = v12;
        if (!ks.GetWideString(out string s13)) return false; LoginTime = s13;
        if (!ks.GetWideString(out string s14)) return false; LogoutTime = s14;
        if (!ks.GetWideString(out string s15)) return false; NickName = s15;
        if (!ks.Get(out uint v16)) return false; GiantUid = v16;
        if (!ks.GetWideString(out string s17)) return false; AccountCreateDate = s17;
        if (!ks.Get(out int v18)) return false; Exp = v18;
        if (!ks.GetWideString(out string s19)) return false; UnitCreateDate = s19;
        if (!ks.Get(out int v20)) return false; IiDurationTime = v20;
        return true;
    }
}

public sealed class KELOG_USE_SEAL_ITEM_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public long DestItemUid { get; set; }
    public long SealItemUid { get; set; }
    public byte SealCount { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(DestItemUid)) return false;
        if (!ks.Put(SealItemUid)) return false;
        if (!ks.Put(SealCount)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out long v1)) return false; DestItemUid = v1;
        if (!ks.Get(out long v2)) return false; SealItemUid = v2;
        if (!ks.Get(out byte v3)) return false; SealCount = v3;
        return true;
    }
}

public sealed class KEMAIL_SEND_EMAIL_NOT : IKSerializable
{
    public List<string> RecvMailAddrList { get; set; } = new();
    public string Title { get; set; } = string.Empty;
    public string Desc { get; set; } = string.Empty;
    public List<string> AttachFileNameList { get; set; } = new();
    public List<string> AttachFileFullPathList { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutList(RecvMailAddrList, static (s, v) => s.PutWideString(v))) return false;
        if (!ks.PutWideString(Title)) return false;
        if (!ks.PutWideString(Desc)) return false;
        if (!ks.PutList(AttachFileNameList, static (s, v) => s.PutWideString(v))) return false;
        if (!ks.PutList(AttachFileFullPathList, static (s, v) => s.PutWideString(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetList(RecvMailAddrList, static s => (s.GetWideString(out string v), v))) return false;
        if (!ks.GetWideString(out string s1)) return false; Title = s1;
        if (!ks.GetWideString(out string s2)) return false; Desc = s2;
        if (!ks.GetList(AttachFileNameList, static s => (s.GetWideString(out string v), v))) return false;
        if (!ks.GetList(AttachFileFullPathList, static s => (s.GetWideString(out string v), v))) return false;
        return true;
    }
}

public sealed class KENX_ADM_SEARCH_ACK : IKSerializable
{
    public uint TransactionId { get; set; }
    public byte Result { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(TransactionId)) return false;
        if (!ks.Put(Result)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint v0)) return false; TransactionId = v0;
        if (!ks.Get(out byte v1)) return false; Result = v1;
        return true;
    }
}

public sealed class KENX_ADM_SEARCH_REQ : IKSerializable
{
    public uint TransactionId { get; set; }
    public string UserId { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(TransactionId)) return false;
        if (!ks.PutWideString(UserId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint v0)) return false; TransactionId = v0;
        if (!ks.GetWideString(out string s1)) return false; UserId = s1;
        return true;
    }
}

public sealed class KENX_ALIVE_NOT : IKSerializable
{
    public byte PacketType { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PacketType)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out byte v0)) return false; PacketType = v0;
        return true;
    }
}

public sealed class KENX_AUTH_ALIVE_NOT : IKSerializable
{
    public byte PacketType { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PacketType)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out byte v0)) return false; PacketType = v0;
        return true;
    }
}

public sealed class KENX_AUTH_INITIALIZE_ACK : IKSerializable
{
    public byte PacketType { get; set; }
    public byte InitializeType { get; set; }
    public byte Result { get; set; }
    public byte DomainSN { get; set; }
    public string Message { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PacketType)) return false;
        if (!ks.Put(InitializeType)) return false;
        if (!ks.Put(Result)) return false;
        if (!ks.Put(DomainSN)) return false;
        if (!ks.PutWideString(Message)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out byte v0)) return false; PacketType = v0;
        if (!ks.Get(out byte v1)) return false; InitializeType = v1;
        if (!ks.Get(out byte v2)) return false; Result = v2;
        if (!ks.Get(out byte v3)) return false; DomainSN = v3;
        if (!ks.GetWideString(out string s4)) return false; Message = s4;
        return true;
    }
}

public sealed class KENX_AUTH_INITIALIZE_REQ : IKSerializable
{
    public byte PacketType { get; set; }
    public byte InitializeType { get; set; }
    public byte GameSN { get; set; }
    public byte DomainSN { get; set; }
    public string DomainName { get; set; } = string.Empty;
    public byte SynchronizeType { get; set; }
    public uint SynchronizeCount { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PacketType)) return false;
        if (!ks.Put(InitializeType)) return false;
        if (!ks.Put(GameSN)) return false;
        if (!ks.Put(DomainSN)) return false;
        if (!ks.PutWideString(DomainName)) return false;
        if (!ks.Put(SynchronizeType)) return false;
        if (!ks.Put(SynchronizeCount)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out byte v0)) return false; PacketType = v0;
        if (!ks.Get(out byte v1)) return false; InitializeType = v1;
        if (!ks.Get(out byte v2)) return false; GameSN = v2;
        if (!ks.Get(out byte v3)) return false; DomainSN = v3;
        if (!ks.GetWideString(out string s4)) return false; DomainName = s4;
        if (!ks.Get(out byte v5)) return false; SynchronizeType = v5;
        if (!ks.Get(out uint v6)) return false; SynchronizeCount = v6;
        return true;
    }
}

public sealed class KENX_AUTH_LOGIN_ACK : IKSerializable
{
    public byte PacketType { get; set; }
    public byte LoginType { get; set; }
    public long SessionNo { get; set; }
    public string UserId { get; set; } = string.Empty;
    public byte AuthorizeResult { get; set; }
    public byte AuthorizeType { get; set; }
    public byte ChargeType { get; set; }
    public byte Option { get; set; }
    public uint Argument { get; set; }
    public byte IsCharged { get; set; }
    public Dictionary<ushort, KPropertyValue> Property { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PacketType)) return false;
        if (!ks.Put(LoginType)) return false;
        if (!ks.Put(SessionNo)) return false;
        if (!ks.PutWideString(UserId)) return false;
        if (!ks.Put(AuthorizeResult)) return false;
        if (!ks.Put(AuthorizeType)) return false;
        if (!ks.Put(ChargeType)) return false;
        if (!ks.Put(Option)) return false;
        if (!ks.Put(Argument)) return false;
        if (!ks.Put(IsCharged)) return false;
        if (!ks.PutMap(Property, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out byte v0)) return false; PacketType = v0;
        if (!ks.Get(out byte v1)) return false; LoginType = v1;
        if (!ks.Get(out long v2)) return false; SessionNo = v2;
        if (!ks.GetWideString(out string s3)) return false; UserId = s3;
        if (!ks.Get(out byte v4)) return false; AuthorizeResult = v4;
        if (!ks.Get(out byte v5)) return false; AuthorizeType = v5;
        if (!ks.Get(out byte v6)) return false; ChargeType = v6;
        if (!ks.Get(out byte v7)) return false; Option = v7;
        if (!ks.Get(out uint v8)) return false; Argument = v8;
        if (!ks.Get(out byte v9)) return false; IsCharged = v9;
        if (!ks.GetMap(Property, static s => (s.Get(out ushort v), v), static s => { var v = new KPropertyValue(); return (s.Get(v), v); })) return false;
        return true;
    }
}

public sealed class KENX_AUTH_LOGIN_REQ : IKSerializable
{
    public byte PacketType { get; set; }
    public byte LoginType { get; set; }
    public string UserId { get; set; } = string.Empty;
    public uint IP { get; set; }
    public Dictionary<ushort, KPropertyValue> Property { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PacketType)) return false;
        if (!ks.Put(LoginType)) return false;
        if (!ks.PutWideString(UserId)) return false;
        if (!ks.Put(IP)) return false;
        if (!ks.PutMap(Property, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out byte v0)) return false; PacketType = v0;
        if (!ks.Get(out byte v1)) return false; LoginType = v1;
        if (!ks.GetWideString(out string s2)) return false; UserId = s2;
        if (!ks.Get(out uint v3)) return false; IP = v3;
        if (!ks.GetMap(Property, static s => (s.Get(out ushort v), v), static s => { var v = new KPropertyValue(); return (s.Get(v), v); })) return false;
        return true;
    }
}

public sealed class KENX_AUTH_LOGOUT_NOT : IKSerializable
{
    public byte PacketType { get; set; }
    public byte LogoutType { get; set; }
    public string UserId { get; set; } = string.Empty;
    public long SessionNo { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PacketType)) return false;
        if (!ks.Put(LogoutType)) return false;
        if (!ks.PutWideString(UserId)) return false;
        if (!ks.Put(SessionNo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out byte v0)) return false; PacketType = v0;
        if (!ks.Get(out byte v1)) return false; LogoutType = v1;
        if (!ks.GetWideString(out string s2)) return false; UserId = s2;
        if (!ks.Get(out long v3)) return false; SessionNo = v3;
        return true;
    }
}

public sealed class KENX_AUTH_MESSAGE_NOT : IKSerializable
{
    public byte PacketType { get; set; }
    public byte MessageType { get; set; }
    public long SessionNo { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string CharacterName { get; set; } = string.Empty;
    public byte Option { get; set; }
    public uint Argument { get; set; }
    public uint SessionCount { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PacketType)) return false;
        if (!ks.Put(MessageType)) return false;
        if (!ks.Put(SessionNo)) return false;
        if (!ks.PutWideString(UserId)) return false;
        if (!ks.PutWideString(CharacterName)) return false;
        if (!ks.Put(Option)) return false;
        if (!ks.Put(Argument)) return false;
        if (!ks.Put(SessionCount)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out byte v0)) return false; PacketType = v0;
        if (!ks.Get(out byte v1)) return false; MessageType = v1;
        if (!ks.Get(out long v2)) return false; SessionNo = v2;
        if (!ks.GetWideString(out string s3)) return false; UserId = s3;
        if (!ks.GetWideString(out string s4)) return false; CharacterName = s4;
        if (!ks.Get(out byte v5)) return false; Option = v5;
        if (!ks.Get(out uint v6)) return false; Argument = v6;
        if (!ks.Get(out uint v7)) return false; SessionCount = v7;
        return true;
    }
}

public sealed class KENX_AUTH_SYNCHRONIZE_ACK : IKSerializable
{
    public byte PacketType { get; set; }
    public byte IsMonitoring { get; set; }
    public Dictionary<long, byte> SynchronizeAck { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PacketType)) return false;
        if (!ks.Put(IsMonitoring)) return false;
        if (!ks.PutMap(SynchronizeAck, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out byte v0)) return false; PacketType = v0;
        if (!ks.Get(out byte v1)) return false; IsMonitoring = v1;
        if (!ks.GetMap(SynchronizeAck, static s => (s.Get(out long v), v), static s => (s.Get(out byte v), v))) return false;
        return true;
    }
}

public sealed class KENX_AUTH_SYNCHRONIZE_REQ : IKSerializable
{
    public byte PacketType { get; set; }
    public byte IsMonitoring { get; set; }
    public Dictionary<long, string> SynchronizeReq { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PacketType)) return false;
        if (!ks.Put(IsMonitoring)) return false;
        if (!ks.PutMap(SynchronizeReq, static (s, k) => s.Put(k), static (s, v) => s.PutWideString(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out byte v0)) return false; PacketType = v0;
        if (!ks.Get(out byte v1)) return false; IsMonitoring = v1;
        if (!ks.GetMap(SynchronizeReq, static s => (s.Get(out long v), v), static s => (s.GetWideString(out string v), v))) return false;
        return true;
    }
}

public sealed class KENX_AUTH_TERMINATE_NOT : IKSerializable
{
    public byte PacketType { get; set; }
    public byte TerminateType { get; set; }
    public long SessionNo { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string CharacterName { get; set; } = string.Empty;
    public byte Option { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PacketType)) return false;
        if (!ks.Put(TerminateType)) return false;
        if (!ks.Put(SessionNo)) return false;
        if (!ks.PutWideString(UserId)) return false;
        if (!ks.PutWideString(CharacterName)) return false;
        if (!ks.Put(Option)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out byte v0)) return false; PacketType = v0;
        if (!ks.Get(out byte v1)) return false; TerminateType = v1;
        if (!ks.Get(out long v2)) return false; SessionNo = v2;
        if (!ks.GetWideString(out string s3)) return false; UserId = s3;
        if (!ks.GetWideString(out string s4)) return false; CharacterName = s4;
        if (!ks.Get(out byte v5)) return false; Option = v5;
        return true;
    }
}

public sealed class KENX_BT_CATEGORY_INQUIRY_ACK : IKSerializable
{
    public uint PacketNo { get; set; }
    public byte PacketType { get; set; }
    public uint Result { get; set; }
    public List<KNXBTCategoryInfo> NXBTCategoryInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PacketNo)) return false;
        if (!ks.Put(PacketType)) return false;
        if (!ks.Put(Result)) return false;
        if (!ks.PutList(NXBTCategoryInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint v0)) return false; PacketNo = v0;
        if (!ks.Get(out byte v1)) return false; PacketType = v1;
        if (!ks.Get(out uint v2)) return false; Result = v2;
        if (!ks.GetList(NXBTCategoryInfo)) return false;
        return true;
    }
}

public sealed class KENX_BT_CATEGORY_INQUIRY_REQ : IKSerializable
{
    public uint PacketNo { get; set; }
    public byte PacketType { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PacketNo)) return false;
        if (!ks.Put(PacketType)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint v0)) return false; PacketNo = v0;
        if (!ks.Get(out byte v1)) return false; PacketType = v1;
        return true;
    }
}

public sealed class KENX_BT_CHECK_AUTO_PAYMENT_USER_ACK : IKSerializable
{
    public uint PacketNo { get; set; }
    public byte PacketType { get; set; }
    public uint Result { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PacketNo)) return false;
        if (!ks.Put(PacketType)) return false;
        if (!ks.Put(Result)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint v0)) return false; PacketNo = v0;
        if (!ks.Get(out byte v1)) return false; PacketType = v1;
        if (!ks.Get(out uint v2)) return false; Result = v2;
        return true;
    }
}

public sealed class KENX_BT_CHECK_AUTO_PAYMENT_USER_REQ : IKSerializable
{
    public uint PacketNo { get; set; }
    public byte PacketType { get; set; }
    public string GameId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public uint ProductNo { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PacketNo)) return false;
        if (!ks.Put(PacketType)) return false;
        if (!ks.PutWideString(GameId)) return false;
        if (!ks.PutWideString(UserId)) return false;
        if (!ks.Put(ProductNo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint v0)) return false; PacketNo = v0;
        if (!ks.Get(out byte v1)) return false; PacketType = v1;
        if (!ks.GetWideString(out string s2)) return false; GameId = s2;
        if (!ks.GetWideString(out string s3)) return false; UserId = s3;
        if (!ks.Get(out uint v4)) return false; ProductNo = v4;
        return true;
    }
}

public sealed class KENX_BT_CHECK_BALANCE_ACK : IKSerializable
{
    public uint PacketNo { get; set; }
    public byte PacketType { get; set; }
    public uint Result { get; set; }
    public uint Balance { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PacketNo)) return false;
        if (!ks.Put(PacketType)) return false;
        if (!ks.Put(Result)) return false;
        if (!ks.Put(Balance)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint v0)) return false; PacketNo = v0;
        if (!ks.Get(out byte v1)) return false; PacketType = v1;
        if (!ks.Get(out uint v2)) return false; Result = v2;
        if (!ks.Get(out uint v3)) return false; Balance = v3;
        return true;
    }
}

public sealed class KENX_BT_CHECK_BALANCE_REQ : IKSerializable
{
    public uint PacketNo { get; set; }
    public byte PacketType { get; set; }
    public uint PaymentType { get; set; }
    public string UserId { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PacketNo)) return false;
        if (!ks.Put(PacketType)) return false;
        if (!ks.Put(PaymentType)) return false;
        if (!ks.PutWideString(UserId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint v0)) return false; PacketNo = v0;
        if (!ks.Get(out byte v1)) return false; PacketType = v1;
        if (!ks.Get(out uint v2)) return false; PaymentType = v2;
        if (!ks.GetWideString(out string s3)) return false; UserId = s3;
        return true;
    }
}

public sealed class KENX_BT_COUPON_INQUIRY_ACK : IKSerializable
{
    public uint PacketNo { get; set; }
    public byte PacketType { get; set; }
    public uint Result { get; set; }
    public List<KNXBTCouponProductInfo> ProductInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PacketNo)) return false;
        if (!ks.Put(PacketType)) return false;
        if (!ks.Put(Result)) return false;
        if (!ks.PutList(ProductInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint v0)) return false; PacketNo = v0;
        if (!ks.Get(out byte v1)) return false; PacketType = v1;
        if (!ks.Get(out uint v2)) return false; Result = v2;
        if (!ks.GetList(ProductInfo)) return false;
        return true;
    }
}

public sealed class KENX_BT_COUPON_INQUIRY_NEW_ACK : IKSerializable
{
    public uint PacketNo { get; set; }
    public byte PacketType { get; set; }
    public uint Result { get; set; }
    public List<KNXBTCouponProductInfo> ProductInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PacketNo)) return false;
        if (!ks.Put(PacketType)) return false;
        if (!ks.Put(Result)) return false;
        if (!ks.PutList(ProductInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint v0)) return false; PacketNo = v0;
        if (!ks.Get(out byte v1)) return false; PacketType = v1;
        if (!ks.Get(out uint v2)) return false; Result = v2;
        if (!ks.GetList(ProductInfo)) return false;
        return true;
    }
}

public sealed class KENX_BT_COUPON_INQUIRY_NEW_REQ : IKSerializable
{
    public uint PacketNo { get; set; }
    public byte PacketType { get; set; }
    public uint UserIP { get; set; }
    public string GameId { get; set; } = string.Empty;
    public byte IsCafe { get; set; }
    public string UserId { get; set; } = string.Empty;
    public uint UserOId { get; set; }
    public string CouponString { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PacketNo)) return false;
        if (!ks.Put(PacketType)) return false;
        if (!ks.Put(UserIP)) return false;
        if (!ks.PutWideString(GameId)) return false;
        if (!ks.Put(IsCafe)) return false;
        if (!ks.PutWideString(UserId)) return false;
        if (!ks.Put(UserOId)) return false;
        if (!ks.PutWideString(CouponString)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint v0)) return false; PacketNo = v0;
        if (!ks.Get(out byte v1)) return false; PacketType = v1;
        if (!ks.Get(out uint v2)) return false; UserIP = v2;
        if (!ks.GetWideString(out string s3)) return false; GameId = s3;
        if (!ks.Get(out byte v4)) return false; IsCafe = v4;
        if (!ks.GetWideString(out string s5)) return false; UserId = s5;
        if (!ks.Get(out uint v6)) return false; UserOId = v6;
        if (!ks.GetWideString(out string s7)) return false; CouponString = s7;
        return true;
    }
}

public sealed class KENX_BT_COUPON_INQUIRY_REQ : IKSerializable
{
    public uint PacketNo { get; set; }
    public byte PacketType { get; set; }
    public byte IsCafe { get; set; }
    public string CouponString { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PacketNo)) return false;
        if (!ks.Put(PacketType)) return false;
        if (!ks.Put(IsCafe)) return false;
        if (!ks.PutWideString(CouponString)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint v0)) return false; PacketNo = v0;
        if (!ks.Get(out byte v1)) return false; PacketType = v1;
        if (!ks.Get(out byte v2)) return false; IsCafe = v2;
        if (!ks.GetWideString(out string s3)) return false; CouponString = s3;
        return true;
    }
}

public sealed class KENX_BT_COUPON_USING_ACK : IKSerializable
{
    public uint PacketNo { get; set; }
    public byte PacketType { get; set; }
    public string CouponTXId { get; set; } = string.Empty;
    public uint Result { get; set; }
    public uint OrderNo { get; set; }
    public List<KNXBTCouponProductInfo> CouponProductInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PacketNo)) return false;
        if (!ks.Put(PacketType)) return false;
        if (!ks.PutWideString(CouponTXId)) return false;
        if (!ks.Put(Result)) return false;
        if (!ks.Put(OrderNo)) return false;
        if (!ks.PutList(CouponProductInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint v0)) return false; PacketNo = v0;
        if (!ks.Get(out byte v1)) return false; PacketType = v1;
        if (!ks.GetWideString(out string s2)) return false; CouponTXId = s2;
        if (!ks.Get(out uint v3)) return false; Result = v3;
        if (!ks.Get(out uint v4)) return false; OrderNo = v4;
        if (!ks.GetList(CouponProductInfo)) return false;
        return true;
    }
}

public sealed class KENX_BT_COUPON_USING_REQ : IKSerializable
{
    public uint PacketNo { get; set; }
    public byte PacketType { get; set; }
    public uint RemoteIP { get; set; }
    public byte Reason { get; set; }
    public string GameId { get; set; } = string.Empty;
    public byte IsCafe { get; set; }
    public string UserId { get; set; } = string.Empty;
    public uint UserOId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string CouponString { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PacketNo)) return false;
        if (!ks.Put(PacketType)) return false;
        if (!ks.Put(RemoteIP)) return false;
        if (!ks.Put(Reason)) return false;
        if (!ks.PutWideString(GameId)) return false;
        if (!ks.Put(IsCafe)) return false;
        if (!ks.PutWideString(UserId)) return false;
        if (!ks.Put(UserOId)) return false;
        if (!ks.PutWideString(UserName)) return false;
        if (!ks.PutWideString(CouponString)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint v0)) return false; PacketNo = v0;
        if (!ks.Get(out byte v1)) return false; PacketType = v1;
        if (!ks.Get(out uint v2)) return false; RemoteIP = v2;
        if (!ks.Get(out byte v3)) return false; Reason = v3;
        if (!ks.GetWideString(out string s4)) return false; GameId = s4;
        if (!ks.Get(out byte v5)) return false; IsCafe = v5;
        if (!ks.GetWideString(out string s6)) return false; UserId = s6;
        if (!ks.Get(out uint v7)) return false; UserOId = v7;
        if (!ks.GetWideString(out string s8)) return false; UserName = s8;
        if (!ks.GetWideString(out string s9)) return false; CouponString = s9;
        return true;
    }
}

public sealed class KENX_BT_HEART_BEAT_ACK : IKSerializable
{
    public uint PacketNo { get; set; }
    public byte PacketType { get; set; }
    public uint Result { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PacketNo)) return false;
        if (!ks.Put(PacketType)) return false;
        if (!ks.Put(Result)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint v0)) return false; PacketNo = v0;
        if (!ks.Get(out byte v1)) return false; PacketType = v1;
        if (!ks.Get(out uint v2)) return false; Result = v2;
        return true;
    }
}

public sealed class KENX_BT_HEART_BEAT_REQ : IKSerializable
{
    public uint PacketNo { get; set; }
    public byte PacketType { get; set; }
    public long ReleaseTick { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PacketNo)) return false;
        if (!ks.Put(PacketType)) return false;
        if (!ks.Put(ReleaseTick)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint v0)) return false; PacketNo = v0;
        if (!ks.Get(out byte v1)) return false; PacketType = v1;
        if (!ks.Get(out long v2)) return false; ReleaseTick = v2;
        return true;
    }
}

public sealed class KENX_BT_INITIALIZE_ACK : IKSerializable
{
    public uint PacketNo { get; set; }
    public byte PacketType { get; set; }
    public string ServiceCode { get; set; } = string.Empty;
    public byte Domain { get; set; }
    public uint Result { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PacketNo)) return false;
        if (!ks.Put(PacketType)) return false;
        if (!ks.PutWideString(ServiceCode)) return false;
        if (!ks.Put(Domain)) return false;
        if (!ks.Put(Result)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint v0)) return false; PacketNo = v0;
        if (!ks.Get(out byte v1)) return false; PacketType = v1;
        if (!ks.GetWideString(out string s2)) return false; ServiceCode = s2;
        if (!ks.Get(out byte v3)) return false; Domain = v3;
        if (!ks.Get(out uint v4)) return false; Result = v4;
        return true;
    }
}

public sealed class KENX_BT_INITIALIZE_REQ : IKSerializable
{
    public uint PacketNo { get; set; }
    public byte PacketType { get; set; }
    public string ServiceCode { get; set; } = string.Empty;
    public byte Domain { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PacketNo)) return false;
        if (!ks.Put(PacketType)) return false;
        if (!ks.PutWideString(ServiceCode)) return false;
        if (!ks.Put(Domain)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint v0)) return false; PacketNo = v0;
        if (!ks.Get(out byte v1)) return false; PacketType = v1;
        if (!ks.GetWideString(out string s2)) return false; ServiceCode = s2;
        if (!ks.Get(out byte v3)) return false; Domain = v3;
        return true;
    }
}

public sealed class KENX_BT_NISMS_INVENTORY_CHECK_ACK : IKSerializable
{
    public uint PacketNo { get; set; }
    public byte PacketType { get; set; }
    public uint Result { get; set; }
    public byte OrderType { get; set; }
    public uint TotalCount { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PacketNo)) return false;
        if (!ks.Put(PacketType)) return false;
        if (!ks.Put(Result)) return false;
        if (!ks.Put(OrderType)) return false;
        if (!ks.Put(TotalCount)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint v0)) return false; PacketNo = v0;
        if (!ks.Get(out byte v1)) return false; PacketType = v1;
        if (!ks.Get(out uint v2)) return false; Result = v2;
        if (!ks.Get(out byte v3)) return false; OrderType = v3;
        if (!ks.Get(out uint v4)) return false; TotalCount = v4;
        return true;
    }
}

public sealed class KENX_BT_NISMS_INVENTORY_CHECK_REQ : IKSerializable
{
    public uint PacketNo { get; set; }
    public byte PacketType { get; set; }
    public string GameId { get; set; } = string.Empty;
    public byte ShowInventory { get; set; }
    public byte IsPresent { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PacketNo)) return false;
        if (!ks.Put(PacketType)) return false;
        if (!ks.PutWideString(GameId)) return false;
        if (!ks.Put(ShowInventory)) return false;
        if (!ks.Put(IsPresent)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint v0)) return false; PacketNo = v0;
        if (!ks.Get(out byte v1)) return false; PacketType = v1;
        if (!ks.GetWideString(out string s2)) return false; GameId = s2;
        if (!ks.Get(out byte v3)) return false; ShowInventory = v3;
        if (!ks.Get(out byte v4)) return false; IsPresent = v4;
        return true;
    }
}

public sealed class KENX_BT_NISMS_INVENTORY_INQUIRY_ACK : IKSerializable
{
    public uint PacketNo { get; set; }
    public byte PacketType { get; set; }
    public uint Result { get; set; }
    public uint TotalCount { get; set; }
    public List<KNXBTOrderInfo> NXBTOrderInfo { get; set; } = new();
    public List<KNXBTPackageInfo> NXBTPackageInfo { get; set; } = new();
    public List<KNXBTLotteryInfo> NXBTLotteryInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PacketNo)) return false;
        if (!ks.Put(PacketType)) return false;
        if (!ks.Put(Result)) return false;
        if (!ks.Put(TotalCount)) return false;
        if (!ks.PutList(NXBTOrderInfo)) return false;
        if (!ks.PutList(NXBTPackageInfo)) return false;
        if (!ks.PutList(NXBTLotteryInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint v0)) return false; PacketNo = v0;
        if (!ks.Get(out byte v1)) return false; PacketType = v1;
        if (!ks.Get(out uint v2)) return false; Result = v2;
        if (!ks.Get(out uint v3)) return false; TotalCount = v3;
        if (!ks.GetList(NXBTOrderInfo)) return false;
        if (!ks.GetList(NXBTPackageInfo)) return false;
        if (!ks.GetList(NXBTLotteryInfo)) return false;
        return true;
    }
}

public sealed class KENX_BT_NISMS_INVENTORY_INQUIRY_REQ : IKSerializable
{
    public uint PacketNo { get; set; }
    public byte PacketType { get; set; }
    public string GameId { get; set; } = string.Empty;
    public byte ShowInventory { get; set; }
    public uint PageIndex { get; set; }
    public uint RowPerPage { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PacketNo)) return false;
        if (!ks.Put(PacketType)) return false;
        if (!ks.PutWideString(GameId)) return false;
        if (!ks.Put(ShowInventory)) return false;
        if (!ks.Put(PageIndex)) return false;
        if (!ks.Put(RowPerPage)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint v0)) return false; PacketNo = v0;
        if (!ks.Get(out byte v1)) return false; PacketType = v1;
        if (!ks.GetWideString(out string s2)) return false; GameId = s2;
        if (!ks.Get(out byte v3)) return false; ShowInventory = v3;
        if (!ks.Get(out uint v4)) return false; PageIndex = v4;
        if (!ks.Get(out uint v5)) return false; RowPerPage = v5;
        return true;
    }
}

public sealed class KENX_BT_NISMS_INVENTORY_PICK_UP_ACK : IKSerializable
{
    public uint PacketNo { get; set; }
    public byte PacketType { get; set; }
    public uint Result { get; set; }
    public uint OrderNo { get; set; }
    public uint ProductNo { get; set; }
    public ushort OrderQuantity { get; set; }
    public string ExtendValue { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PacketNo)) return false;
        if (!ks.Put(PacketType)) return false;
        if (!ks.Put(Result)) return false;
        if (!ks.Put(OrderNo)) return false;
        if (!ks.Put(ProductNo)) return false;
        if (!ks.Put(OrderQuantity)) return false;
        if (!ks.PutWideString(ExtendValue)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint v0)) return false; PacketNo = v0;
        if (!ks.Get(out byte v1)) return false; PacketType = v1;
        if (!ks.Get(out uint v2)) return false; Result = v2;
        if (!ks.Get(out uint v3)) return false; OrderNo = v3;
        if (!ks.Get(out uint v4)) return false; ProductNo = v4;
        if (!ks.Get(out ushort v5)) return false; OrderQuantity = v5;
        if (!ks.GetWideString(out string s6)) return false; ExtendValue = s6;
        return true;
    }
}

public sealed class KENX_BT_NISMS_INVENTORY_PICK_UP_FOR_PACKAGE_ACK : IKSerializable
{
    public uint PacketNo { get; set; }
    public byte PacketType { get; set; }
    public uint Result { get; set; }
    public uint OrderNo { get; set; }
    public uint PackageProductNo { get; set; }
    public uint ProductNo { get; set; }
    public uint SubProductNo { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PacketNo)) return false;
        if (!ks.Put(PacketType)) return false;
        if (!ks.Put(Result)) return false;
        if (!ks.Put(OrderNo)) return false;
        if (!ks.Put(PackageProductNo)) return false;
        if (!ks.Put(ProductNo)) return false;
        if (!ks.Put(SubProductNo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint v0)) return false; PacketNo = v0;
        if (!ks.Get(out byte v1)) return false; PacketType = v1;
        if (!ks.Get(out uint v2)) return false; Result = v2;
        if (!ks.Get(out uint v3)) return false; OrderNo = v3;
        if (!ks.Get(out uint v4)) return false; PackageProductNo = v4;
        if (!ks.Get(out uint v5)) return false; ProductNo = v5;
        if (!ks.Get(out uint v6)) return false; SubProductNo = v6;
        return true;
    }
}

public sealed class KENX_BT_NISMS_INVENTORY_PICK_UP_FOR_PACKAGE_REQ : IKSerializable
{
    public uint PacketNo { get; set; }
    public byte PacketType { get; set; }
    public uint OrderNo { get; set; }
    public uint PackageProductNo { get; set; }
    public uint ProductNo { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PacketNo)) return false;
        if (!ks.Put(PacketType)) return false;
        if (!ks.Put(OrderNo)) return false;
        if (!ks.Put(PackageProductNo)) return false;
        if (!ks.Put(ProductNo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint v0)) return false; PacketNo = v0;
        if (!ks.Get(out byte v1)) return false; PacketType = v1;
        if (!ks.Get(out uint v2)) return false; OrderNo = v2;
        if (!ks.Get(out uint v3)) return false; PackageProductNo = v3;
        if (!ks.Get(out uint v4)) return false; ProductNo = v4;
        return true;
    }
}

public sealed class KENX_BT_NISMS_INVENTORY_PICK_UP_ONCE_ACK : IKSerializable
{
    public uint PacketNo { get; set; }
    public byte PacketType { get; set; }
    public uint Result { get; set; }
    public uint OrderNo { get; set; }
    public uint ProductNo { get; set; }
    public byte ProductKind { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public ushort ProductExpire { get; set; }
    public ushort ProductPieces { get; set; }
    public ushort OrderQuantity { get; set; }
    public string ProductAttribute0 { get; set; } = string.Empty;
    public string ProductAttribute1 { get; set; } = string.Empty;
    public string ProductAttribute2 { get; set; } = string.Empty;
    public string ProductAttribute3 { get; set; } = string.Empty;
    public string ProductAttribute4 { get; set; } = string.Empty;
    public string ExtendValue { get; set; } = string.Empty;
    public List<KNXBTSubProductInfo> NXBTSubProductInfo { get; set; } = new();
    public List<KNXBTBonusProductInfoPickUp> KNXBTBonusProductInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PacketNo)) return false;
        if (!ks.Put(PacketType)) return false;
        if (!ks.Put(Result)) return false;
        if (!ks.Put(OrderNo)) return false;
        if (!ks.Put(ProductNo)) return false;
        if (!ks.Put(ProductKind)) return false;
        if (!ks.PutWideString(ProductName)) return false;
        if (!ks.PutWideString(ProductId)) return false;
        if (!ks.Put(ProductExpire)) return false;
        if (!ks.Put(ProductPieces)) return false;
        if (!ks.Put(OrderQuantity)) return false;
        if (!ks.PutWideString(ProductAttribute0)) return false;
        if (!ks.PutWideString(ProductAttribute1)) return false;
        if (!ks.PutWideString(ProductAttribute2)) return false;
        if (!ks.PutWideString(ProductAttribute3)) return false;
        if (!ks.PutWideString(ProductAttribute4)) return false;
        if (!ks.PutWideString(ExtendValue)) return false;
        if (!ks.PutList(NXBTSubProductInfo)) return false;
        if (!ks.PutList(KNXBTBonusProductInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint v0)) return false; PacketNo = v0;
        if (!ks.Get(out byte v1)) return false; PacketType = v1;
        if (!ks.Get(out uint v2)) return false; Result = v2;
        if (!ks.Get(out uint v3)) return false; OrderNo = v3;
        if (!ks.Get(out uint v4)) return false; ProductNo = v4;
        if (!ks.Get(out byte v5)) return false; ProductKind = v5;
        if (!ks.GetWideString(out string s6)) return false; ProductName = s6;
        if (!ks.GetWideString(out string s7)) return false; ProductId = s7;
        if (!ks.Get(out ushort v8)) return false; ProductExpire = v8;
        if (!ks.Get(out ushort v9)) return false; ProductPieces = v9;
        if (!ks.Get(out ushort v10)) return false; OrderQuantity = v10;
        if (!ks.GetWideString(out string s11)) return false; ProductAttribute0 = s11;
        if (!ks.GetWideString(out string s12)) return false; ProductAttribute1 = s12;
        if (!ks.GetWideString(out string s13)) return false; ProductAttribute2 = s13;
        if (!ks.GetWideString(out string s14)) return false; ProductAttribute3 = s14;
        if (!ks.GetWideString(out string s15)) return false; ProductAttribute4 = s15;
        if (!ks.GetWideString(out string s16)) return false; ExtendValue = s16;
        if (!ks.GetList(NXBTSubProductInfo)) return false;
        if (!ks.GetList(KNXBTBonusProductInfo)) return false;
        return true;
    }
}

public sealed class KENX_BT_NISMS_INVENTORY_PICK_UP_ONCE_REQ : IKSerializable
{
    public uint PacketNo { get; set; }
    public byte PacketType { get; set; }
    public uint OrderNo { get; set; }
    public uint ProductNo { get; set; }
    public ushort OrderQuantity { get; set; }
    public string ExtendValue { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PacketNo)) return false;
        if (!ks.Put(PacketType)) return false;
        if (!ks.Put(OrderNo)) return false;
        if (!ks.Put(ProductNo)) return false;
        if (!ks.Put(OrderQuantity)) return false;
        if (!ks.PutWideString(ExtendValue)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint v0)) return false; PacketNo = v0;
        if (!ks.Get(out byte v1)) return false; PacketType = v1;
        if (!ks.Get(out uint v2)) return false; OrderNo = v2;
        if (!ks.Get(out uint v3)) return false; ProductNo = v3;
        if (!ks.Get(out ushort v4)) return false; OrderQuantity = v4;
        if (!ks.GetWideString(out string s5)) return false; ExtendValue = s5;
        return true;
    }
}

public sealed class KENX_BT_NISMS_INVENTORY_PICK_UP_REQ : IKSerializable
{
    public uint PacketNo { get; set; }
    public byte PacketType { get; set; }
    public uint OrderNo { get; set; }
    public uint ProductNo { get; set; }
    public ushort OrderQuantity { get; set; }
    public string ExtendValue { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PacketNo)) return false;
        if (!ks.Put(PacketType)) return false;
        if (!ks.Put(OrderNo)) return false;
        if (!ks.Put(ProductNo)) return false;
        if (!ks.Put(OrderQuantity)) return false;
        if (!ks.PutWideString(ExtendValue)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint v0)) return false; PacketNo = v0;
        if (!ks.Get(out byte v1)) return false; PacketType = v1;
        if (!ks.Get(out uint v2)) return false; OrderNo = v2;
        if (!ks.Get(out uint v3)) return false; ProductNo = v3;
        if (!ks.Get(out ushort v4)) return false; OrderQuantity = v4;
        if (!ks.GetWideString(out string s5)) return false; ExtendValue = s5;
        return true;
    }
}

public sealed class KENX_BT_NISMS_INVENTORY_PICK_UP_ROLLBACK_ACK : IKSerializable
{
    public uint PacketNo { get; set; }
    public byte PacketType { get; set; }
    public uint Result { get; set; }
    public uint OrderNo { get; set; }
    public uint ProductNo { get; set; }
    public string ExtendValue { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PacketNo)) return false;
        if (!ks.Put(PacketType)) return false;
        if (!ks.Put(Result)) return false;
        if (!ks.Put(OrderNo)) return false;
        if (!ks.Put(ProductNo)) return false;
        if (!ks.PutWideString(ExtendValue)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint v0)) return false; PacketNo = v0;
        if (!ks.Get(out byte v1)) return false; PacketType = v1;
        if (!ks.Get(out uint v2)) return false; Result = v2;
        if (!ks.Get(out uint v3)) return false; OrderNo = v3;
        if (!ks.Get(out uint v4)) return false; ProductNo = v4;
        if (!ks.GetWideString(out string s5)) return false; ExtendValue = s5;
        return true;
    }
}

public sealed class KENX_BT_NISMS_INVENTORY_PICK_UP_ROLLBACK_REQ : IKSerializable
{
    public uint PacketNo { get; set; }
    public byte PacketType { get; set; }
    public uint OrderNo { get; set; }
    public uint ProductNo { get; set; }
    public string ExtendValue { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PacketNo)) return false;
        if (!ks.Put(PacketType)) return false;
        if (!ks.Put(OrderNo)) return false;
        if (!ks.Put(ProductNo)) return false;
        if (!ks.PutWideString(ExtendValue)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint v0)) return false; PacketNo = v0;
        if (!ks.Get(out byte v1)) return false; PacketType = v1;
        if (!ks.Get(out uint v2)) return false; OrderNo = v2;
        if (!ks.Get(out uint v3)) return false; ProductNo = v3;
        if (!ks.GetWideString(out string s4)) return false; ExtendValue = s4;
        return true;
    }
}

public sealed class KENX_BT_NISMS_INVENTORY_READABLE_ACK : IKSerializable
{
    public uint PacketNo { get; set; }
    public byte PacketType { get; set; }
    public uint Result { get; set; }
    public byte OrderType { get; set; }
    public uint TotalCount { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PacketNo)) return false;
        if (!ks.Put(PacketType)) return false;
        if (!ks.Put(Result)) return false;
        if (!ks.Put(OrderType)) return false;
        if (!ks.Put(TotalCount)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint v0)) return false; PacketNo = v0;
        if (!ks.Get(out byte v1)) return false; PacketType = v1;
        if (!ks.Get(out uint v2)) return false; Result = v2;
        if (!ks.Get(out byte v3)) return false; OrderType = v3;
        if (!ks.Get(out uint v4)) return false; TotalCount = v4;
        return true;
    }
}

public sealed class KENX_BT_NISMS_INVENTORY_READABLE_REQ : IKSerializable
{
    public uint PacketNo { get; set; }
    public byte PacketType { get; set; }
    public string GameId { get; set; } = string.Empty;
    public byte ShowInventory { get; set; }
    public byte IsPresent { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PacketNo)) return false;
        if (!ks.Put(PacketType)) return false;
        if (!ks.PutWideString(GameId)) return false;
        if (!ks.Put(ShowInventory)) return false;
        if (!ks.Put(IsPresent)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint v0)) return false; PacketNo = v0;
        if (!ks.Get(out byte v1)) return false; PacketType = v1;
        if (!ks.GetWideString(out string s2)) return false; GameId = s2;
        if (!ks.Get(out byte v3)) return false; ShowInventory = v3;
        if (!ks.Get(out byte v4)) return false; IsPresent = v4;
        return true;
    }
}

public sealed class KENX_BT_NISMS_INVENTORY_READ_ACK : IKSerializable
{
    public uint PacketNo { get; set; }
    public byte PacketType { get; set; }
    public uint Result { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PacketNo)) return false;
        if (!ks.Put(PacketType)) return false;
        if (!ks.Put(Result)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint v0)) return false; PacketNo = v0;
        if (!ks.Get(out byte v1)) return false; PacketType = v1;
        if (!ks.Get(out uint v2)) return false; Result = v2;
        return true;
    }
}

public sealed class KENX_BT_NISMS_INVENTORY_READ_REQ : IKSerializable
{
    public uint PacketNo { get; set; }
    public byte PacketType { get; set; }
    public uint OrderNo { get; set; }
    public uint ProductNo { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PacketNo)) return false;
        if (!ks.Put(PacketType)) return false;
        if (!ks.Put(OrderNo)) return false;
        if (!ks.Put(ProductNo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint v0)) return false; PacketNo = v0;
        if (!ks.Get(out byte v1)) return false; PacketType = v1;
        if (!ks.Get(out uint v2)) return false; OrderNo = v2;
        if (!ks.Get(out uint v3)) return false; ProductNo = v3;
        return true;
    }
}

public sealed class KENX_BT_PRODUCT_INQUIRY_ACK : IKSerializable
{
    public uint PacketNo { get; set; }
    public byte PacketType { get; set; }
    public uint Result { get; set; }
    public long ReleaseTick { get; set; }
    public uint TotalProductCount { get; set; }
    public List<KNXBTProductInfo> NXBTProductInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PacketNo)) return false;
        if (!ks.Put(PacketType)) return false;
        if (!ks.Put(Result)) return false;
        if (!ks.Put(ReleaseTick)) return false;
        if (!ks.Put(TotalProductCount)) return false;
        if (!ks.PutList(NXBTProductInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint v0)) return false; PacketNo = v0;
        if (!ks.Get(out byte v1)) return false; PacketType = v1;
        if (!ks.Get(out uint v2)) return false; Result = v2;
        if (!ks.Get(out long v3)) return false; ReleaseTick = v3;
        if (!ks.Get(out uint v4)) return false; TotalProductCount = v4;
        if (!ks.GetList(NXBTProductInfo)) return false;
        return true;
    }
}

public sealed class KENX_BT_PRODUCT_INQUIRY_REQ : IKSerializable
{
    public uint PacketNo { get; set; }
    public byte PacketType { get; set; }
    public uint PageIndex { get; set; }
    public uint RowPerPage { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PacketNo)) return false;
        if (!ks.Put(PacketType)) return false;
        if (!ks.Put(PageIndex)) return false;
        if (!ks.Put(RowPerPage)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint v0)) return false; PacketNo = v0;
        if (!ks.Get(out byte v1)) return false; PacketType = v1;
        if (!ks.Get(out uint v2)) return false; PageIndex = v2;
        if (!ks.Get(out uint v3)) return false; RowPerPage = v3;
        return true;
    }
}

public sealed class KENX_BT_PRODUCT_INQUIRY_XML_ACK : IKSerializable
{
    public uint PacketNo { get; set; }
    public byte PacketType { get; set; }
    public uint Result { get; set; }
    public long ReleaseTick { get; set; }
    public uint TotalProductCount { get; set; }
    public string ResultXML { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PacketNo)) return false;
        if (!ks.Put(PacketType)) return false;
        if (!ks.Put(Result)) return false;
        if (!ks.Put(ReleaseTick)) return false;
        if (!ks.Put(TotalProductCount)) return false;
        if (!ks.PutWideString(ResultXML)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint v0)) return false; PacketNo = v0;
        if (!ks.Get(out byte v1)) return false; PacketType = v1;
        if (!ks.Get(out uint v2)) return false; Result = v2;
        if (!ks.Get(out long v3)) return false; ReleaseTick = v3;
        if (!ks.Get(out uint v4)) return false; TotalProductCount = v4;
        if (!ks.GetWideString(out string s5)) return false; ResultXML = s5;
        return true;
    }
}

public sealed class KENX_BT_PRODUCT_INQUIRY_XML_REQ : IKSerializable
{
    public uint PacketNo { get; set; }
    public byte PacketType { get; set; }
    public uint PageIndex { get; set; }
    public uint RowPerPage { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PacketNo)) return false;
        if (!ks.Put(PacketType)) return false;
        if (!ks.Put(PageIndex)) return false;
        if (!ks.Put(RowPerPage)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint v0)) return false; PacketNo = v0;
        if (!ks.Get(out byte v1)) return false; PacketType = v1;
        if (!ks.Get(out uint v2)) return false; PageIndex = v2;
        if (!ks.Get(out uint v3)) return false; RowPerPage = v3;
        return true;
    }
}

public sealed class KENX_BT_PURCHASE_GIFT_ACK : IKSerializable
{
    public uint PacketNo { get; set; }
    public byte PacketType { get; set; }
    public string OrderId { get; set; } = string.Empty;
    public uint Result { get; set; }
    public uint OrderNo { get; set; }
    public List<KNXBTPurchaseAckInfo> NXBTPurchaseInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PacketNo)) return false;
        if (!ks.Put(PacketType)) return false;
        if (!ks.PutWideString(OrderId)) return false;
        if (!ks.Put(Result)) return false;
        if (!ks.Put(OrderNo)) return false;
        if (!ks.PutList(NXBTPurchaseInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint v0)) return false; PacketNo = v0;
        if (!ks.Get(out byte v1)) return false; PacketType = v1;
        if (!ks.GetWideString(out string s2)) return false; OrderId = s2;
        if (!ks.Get(out uint v3)) return false; Result = v3;
        if (!ks.Get(out uint v4)) return false; OrderNo = v4;
        if (!ks.GetList(NXBTPurchaseInfo)) return false;
        return true;
    }
}

public sealed class KENX_BT_PURCHASE_GIFT_REQ : IKSerializable
{
    public uint PacketNo { get; set; }
    public byte PacketType { get; set; }
    public uint RemoteIP { get; set; }
    public byte Reason { get; set; }
    public string SenderGameId { get; set; } = string.Empty;
    public string SenderUserId { get; set; } = string.Empty;
    public uint SenderUserOId { get; set; }
    public string SenderUserName { get; set; } = string.Empty;
    public byte SenderUserAge { get; set; }
    public byte ReceiverServerNo { get; set; }
    public string ReceiverGameId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string OrderId { get; set; } = string.Empty;
    public uint PaymentType { get; set; }
    public uint TotalAmount { get; set; }
    public List<KNXBTPurchaseReqInfo> NXBTPurchaseInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PacketNo)) return false;
        if (!ks.Put(PacketType)) return false;
        if (!ks.Put(RemoteIP)) return false;
        if (!ks.Put(Reason)) return false;
        if (!ks.PutWideString(SenderGameId)) return false;
        if (!ks.PutWideString(SenderUserId)) return false;
        if (!ks.Put(SenderUserOId)) return false;
        if (!ks.PutWideString(SenderUserName)) return false;
        if (!ks.Put(SenderUserAge)) return false;
        if (!ks.Put(ReceiverServerNo)) return false;
        if (!ks.PutWideString(ReceiverGameId)) return false;
        if (!ks.PutWideString(Message)) return false;
        if (!ks.PutWideString(OrderId)) return false;
        if (!ks.Put(PaymentType)) return false;
        if (!ks.Put(TotalAmount)) return false;
        if (!ks.PutList(NXBTPurchaseInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint v0)) return false; PacketNo = v0;
        if (!ks.Get(out byte v1)) return false; PacketType = v1;
        if (!ks.Get(out uint v2)) return false; RemoteIP = v2;
        if (!ks.Get(out byte v3)) return false; Reason = v3;
        if (!ks.GetWideString(out string s4)) return false; SenderGameId = s4;
        if (!ks.GetWideString(out string s5)) return false; SenderUserId = s5;
        if (!ks.Get(out uint v6)) return false; SenderUserOId = v6;
        if (!ks.GetWideString(out string s7)) return false; SenderUserName = s7;
        if (!ks.Get(out byte v8)) return false; SenderUserAge = v8;
        if (!ks.Get(out byte v9)) return false; ReceiverServerNo = v9;
        if (!ks.GetWideString(out string s10)) return false; ReceiverGameId = s10;
        if (!ks.GetWideString(out string s11)) return false; Message = s11;
        if (!ks.GetWideString(out string s12)) return false; OrderId = s12;
        if (!ks.Get(out uint v13)) return false; PaymentType = v13;
        if (!ks.Get(out uint v14)) return false; TotalAmount = v14;
        if (!ks.GetList(NXBTPurchaseInfo)) return false;
        return true;
    }
}

public sealed class KENX_BT_PURCHASE_ITEM_ACK : IKSerializable
{
    public uint PacketNo { get; set; }
    public byte PacketType { get; set; }
    public string OrderId { get; set; } = string.Empty;
    public uint Result { get; set; }
    public uint OrderNo { get; set; }
    public List<KNXBTPurchaseAckInfo> NXBTPurchaseInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PacketNo)) return false;
        if (!ks.Put(PacketType)) return false;
        if (!ks.PutWideString(OrderId)) return false;
        if (!ks.Put(Result)) return false;
        if (!ks.Put(OrderNo)) return false;
        if (!ks.PutList(NXBTPurchaseInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint v0)) return false; PacketNo = v0;
        if (!ks.Get(out byte v1)) return false; PacketType = v1;
        if (!ks.GetWideString(out string s2)) return false; OrderId = s2;
        if (!ks.Get(out uint v3)) return false; Result = v3;
        if (!ks.Get(out uint v4)) return false; OrderNo = v4;
        if (!ks.GetList(NXBTPurchaseInfo)) return false;
        return true;
    }
}

public sealed class KENX_BT_PURCHASE_ITEM_REQ : IKSerializable
{
    public uint PacketNo { get; set; }
    public byte PacketType { get; set; }
    public uint RemoteIP { get; set; }
    public byte Reason { get; set; }
    public string GameId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public uint UserOId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public byte UserAge { get; set; }
    public string OrderId { get; set; } = string.Empty;
    public uint PaymentType { get; set; }
    public uint TotalAmount { get; set; }
    public List<KNXBTPurchaseReqInfo> NXBTPurchaseInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PacketNo)) return false;
        if (!ks.Put(PacketType)) return false;
        if (!ks.Put(RemoteIP)) return false;
        if (!ks.Put(Reason)) return false;
        if (!ks.PutWideString(GameId)) return false;
        if (!ks.PutWideString(UserId)) return false;
        if (!ks.Put(UserOId)) return false;
        if (!ks.PutWideString(UserName)) return false;
        if (!ks.Put(UserAge)) return false;
        if (!ks.PutWideString(OrderId)) return false;
        if (!ks.Put(PaymentType)) return false;
        if (!ks.Put(TotalAmount)) return false;
        if (!ks.PutList(NXBTPurchaseInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint v0)) return false; PacketNo = v0;
        if (!ks.Get(out byte v1)) return false; PacketType = v1;
        if (!ks.Get(out uint v2)) return false; RemoteIP = v2;
        if (!ks.Get(out byte v3)) return false; Reason = v3;
        if (!ks.GetWideString(out string s4)) return false; GameId = s4;
        if (!ks.GetWideString(out string s5)) return false; UserId = s5;
        if (!ks.Get(out uint v6)) return false; UserOId = v6;
        if (!ks.GetWideString(out string s7)) return false; UserName = s7;
        if (!ks.Get(out byte v8)) return false; UserAge = v8;
        if (!ks.GetWideString(out string s9)) return false; OrderId = s9;
        if (!ks.Get(out uint v10)) return false; PaymentType = v10;
        if (!ks.Get(out uint v11)) return false; TotalAmount = v11;
        if (!ks.GetList(NXBTPurchaseInfo)) return false;
        return true;
    }
}

public sealed class KENX_BT_REFUND_ACK : IKSerializable
{
    public uint PacketNo { get; set; }
    public byte PacketType { get; set; }
    public uint Result { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PacketNo)) return false;
        if (!ks.Put(PacketType)) return false;
        if (!ks.Put(Result)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint v0)) return false; PacketNo = v0;
        if (!ks.Get(out byte v1)) return false; PacketType = v1;
        if (!ks.Get(out uint v2)) return false; Result = v2;
        return true;
    }
}

public sealed class KENX_BT_REFUND_REQ : IKSerializable
{
    public uint PacketNo { get; set; }
    public byte PacketType { get; set; }
    public uint RemoteIP { get; set; }
    public string RequestId { get; set; } = string.Empty;
    public string GameId { get; set; } = string.Empty;
    public uint OrderNo { get; set; }
    public uint ProductNo { get; set; }
    public ushort Quantity { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PacketNo)) return false;
        if (!ks.Put(PacketType)) return false;
        if (!ks.Put(RemoteIP)) return false;
        if (!ks.PutWideString(RequestId)) return false;
        if (!ks.PutWideString(GameId)) return false;
        if (!ks.Put(OrderNo)) return false;
        if (!ks.Put(ProductNo)) return false;
        if (!ks.Put(Quantity)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint v0)) return false; PacketNo = v0;
        if (!ks.Get(out byte v1)) return false; PacketType = v1;
        if (!ks.Get(out uint v2)) return false; RemoteIP = v2;
        if (!ks.GetWideString(out string s3)) return false; RequestId = s3;
        if (!ks.GetWideString(out string s4)) return false; GameId = s4;
        if (!ks.Get(out uint v5)) return false; OrderNo = v5;
        if (!ks.Get(out uint v6)) return false; ProductNo = v6;
        if (!ks.Get(out ushort v7)) return false; Quantity = v7;
        return true;
    }
}

public sealed class KENX_CHAR_SEARCH_ACK : IKSerializable
{
    public uint TransactionId { get; set; }
    public byte Result { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(TransactionId)) return false;
        if (!ks.Put(Result)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint v0)) return false; TransactionId = v0;
        if (!ks.Get(out byte v1)) return false; Result = v1;
        return true;
    }
}

public sealed class KENX_CHAR_SEARCH_REQ : IKSerializable
{
    public uint TransactionId { get; set; }
    public string UnitNickName { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(TransactionId)) return false;
        if (!ks.PutWideString(UnitNickName)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint v0)) return false; TransactionId = v0;
        if (!ks.GetWideString(out string s1)) return false; UnitNickName = s1;
        return true;
    }
}

public sealed class KENX_INITIALIZE_BILLING_AUTH_NOT : IKSerializable
{
    public byte PacketType { get; set; }
    public byte GameSN { get; set; }
    public byte DomainSN { get; set; }
    public string DomainName { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PacketType)) return false;
        if (!ks.Put(GameSN)) return false;
        if (!ks.Put(DomainSN)) return false;
        if (!ks.PutWideString(DomainName)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out byte v0)) return false; PacketType = v0;
        if (!ks.Get(out byte v1)) return false; GameSN = v1;
        if (!ks.Get(out byte v2)) return false; DomainSN = v2;
        if (!ks.GetWideString(out string s3)) return false; DomainName = s3;
        return true;
    }
}

public sealed class KENX_NEW_SHOP_GIFT_NOT : IKSerializable
{
    public uint TransactionId { get; set; }
    public string UnitNickName { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(TransactionId)) return false;
        if (!ks.PutWideString(UnitNickName)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint v0)) return false; TransactionId = v0;
        if (!ks.GetWideString(out string s1)) return false; UnitNickName = s1;
        return true;
    }
}

public sealed class KENX_NEW_SHOP_ITEM_NOT : IKSerializable
{
    public uint TransactionId { get; set; }
    public string UnitNickName { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(TransactionId)) return false;
        if (!ks.PutWideString(UnitNickName)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint v0)) return false; TransactionId = v0;
        if (!ks.GetWideString(out string s1)) return false; UnitNickName = s1;
        return true;
    }
}

public sealed class KENX_UPDATE_PRODUCT_LIST_NOT : IKSerializable
{
    public List<int> ProductItemId { get; set; } = new();
    public List<KCashItemResolveInfo> CashItemRessolveInfo { get; set; } = new();
    public List<KCashPruductInfo> CashProductList { get; set; } = new();
    public uint ProductNoResStone { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutList(ProductItemId, static (s, v) => s.Put(v))) return false;
        if (!ks.PutList(CashItemRessolveInfo)) return false;
        if (!ks.PutList(CashProductList)) return false;
        if (!ks.Put(ProductNoResStone)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetList(ProductItemId, static s => (s.Get(out int v), v))) return false;
        if (!ks.GetList(CashItemRessolveInfo)) return false;
        if (!ks.GetList(CashProductList)) return false;
        if (!ks.Get(out uint v3)) return false; ProductNoResStone = v3;
        return true;
    }
}

public sealed class KEPM_CHANGE_COME_BACK_BUFF_NOT : IKSerializable
{
    public bool IsBuffFinish { get; set; }
    public string ComeBackBuffEnd { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(IsBuffFinish)) return false;
        if (!ks.PutWideString(ComeBackBuffEnd)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool v0)) return false; IsBuffFinish = v0;
        if (!ks.GetWideString(out string s1)) return false; ComeBackBuffEnd = s1;
        return true;
    }
}

public sealed class KEPM_CHECK_DUNGEON_GAME_PLAY_CONDITION_REQ : IKSerializable
{
    public KDungeonGameInfo DungeonGameInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(DungeonGameInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(DungeonGameInfo)) return false;
        return true;
    }
}

public sealed class KEPM_CHECK_FOR_PARTY_GAME_START_REQ : IKSerializable
{
    public long PartyUid { get; set; }
    public ushort EventId { get; set; }
    public short WorldId { get; set; }
    public KDungeonGameInfo DungeonGameInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PartyUid)) return false;
        if (!ks.Put(EventId)) return false;
        if (!ks.Put(WorldId)) return false;
        if (!ks.Put(DungeonGameInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; PartyUid = v0;
        if (!ks.Get(out ushort v1)) return false; EventId = v1;
        if (!ks.Get(out short v2)) return false; WorldId = v2;
        if (!ks.Get(DungeonGameInfo)) return false;
        return true;
    }
}

public sealed class KEPM_CHECK_PARTY_CHANGE_HOST_REQ : IKSerializable
{
    public long PartyUid { get; set; }
    public sbyte PartyType { get; set; }
    public long OldHostUnitUid { get; set; }
    public long NewHostUnitUid { get; set; }
    public int DungeonId { get; set; }
    public sbyte Difficulty { get; set; }
    public sbyte DungeonMode { get; set; }
    public bool ForRegroupParty { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PartyUid)) return false;
        if (!ks.Put(PartyType)) return false;
        if (!ks.Put(OldHostUnitUid)) return false;
        if (!ks.Put(NewHostUnitUid)) return false;
        if (!ks.Put(DungeonId)) return false;
        if (!ks.Put(Difficulty)) return false;
        if (!ks.Put(DungeonMode)) return false;
        if (!ks.Put(ForRegroupParty)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; PartyUid = v0;
        if (!ks.Get(out sbyte v1)) return false; PartyType = v1;
        if (!ks.Get(out long v2)) return false; OldHostUnitUid = v2;
        if (!ks.Get(out long v3)) return false; NewHostUnitUid = v3;
        if (!ks.Get(out int v4)) return false; DungeonId = v4;
        if (!ks.Get(out sbyte v5)) return false; Difficulty = v5;
        if (!ks.Get(out sbyte v6)) return false; DungeonMode = v6;
        if (!ks.Get(out bool v7)) return false; ForRegroupParty = v7;
        return true;
    }
}

public sealed class KEPM_CLOSE_PARTY_LIST_NOT : IKSerializable
{
    public long PartyUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PartyUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; PartyUid = v0;
        return true;
    }
}

public sealed class KEPM_CREATE_PARTY_BY_INVITE_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KPartyInfo PartyInfo { get; set; } = new();
    public KPartyUserInfo HostPartyUserInfo { get; set; } = new();
    public KPartyUserInfo ReceiverPartyUserInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(PartyInfo)) return false;
        if (!ks.Put(HostPartyUserInfo)) return false;
        if (!ks.Put(ReceiverPartyUserInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(PartyInfo)) return false;
        if (!ks.Get(HostPartyUserInfo)) return false;
        if (!ks.Get(ReceiverPartyUserInfo)) return false;
        return true;
    }
}

public sealed class KEPM_DUNGEON_SECRET_STAGE_ENTER_CHECK_REQ : IKSerializable
{
    public long PartyUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PartyUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; PartyUid = v0;
        return true;
    }
}

public sealed class KEPM_LEAVE_PARTY_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int Reason { get; set; }
    public long PartyUid { get; set; }
    public long NewJoinPartyUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(Reason)) return false;
        if (!ks.Put(PartyUid)) return false;
        if (!ks.Put(NewJoinPartyUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out int v1)) return false; Reason = v1;
        if (!ks.Get(out long v2)) return false; PartyUid = v2;
        if (!ks.Get(out long v3)) return false; NewJoinPartyUid = v3;
        return true;
    }
}

public sealed class KEPM_OPEN_PARTY_LIST_NOT : IKSerializable
{
    public KPartyInfo PartyInfo { get; set; } = new();
    public List<KPartyUserInfo> UserInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PartyInfo)) return false;
        if (!ks.PutList(UserInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(PartyInfo)) return false;
        if (!ks.GetList(UserInfo)) return false;
        return true;
    }
}

public sealed class KEPM_PARTY_BAN_USER_NOT : IKSerializable
{
    public long PartyUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PartyUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; PartyUid = v0;
        return true;
    }
}

public sealed class KEPM_PARTY_GAME_START_ACK : IKSerializable
{
    public int Ok { get; set; }
    public ushort EventId { get; set; }
    public int DungeonMode { get; set; }
    public int DungeonId { get; set; }
    public long FailUnitUid { get; set; }
    public string FailUserNickName { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(EventId)) return false;
        if (!ks.Put(DungeonMode)) return false;
        if (!ks.Put(DungeonId)) return false;
        if (!ks.Put(FailUnitUid)) return false;
        if (!ks.PutWideString(FailUserNickName)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out ushort v1)) return false; EventId = v1;
        if (!ks.Get(out int v2)) return false; DungeonMode = v2;
        if (!ks.Get(out int v3)) return false; DungeonId = v3;
        if (!ks.Get(out long v4)) return false; FailUnitUid = v4;
        if (!ks.GetWideString(out string s5)) return false; FailUserNickName = s5;
        return true;
    }
}

public sealed class KEPM_SEND_UPDATE_PARTY_MEMBER_LIST_NOT : IKSerializable
{
    public long PartyUid { get; set; }
    public HashSet<long> PartyUIDList { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PartyUid)) return false;
        if (!ks.PutList(PartyUIDList, static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; PartyUid = v0;
        if (!ks.GetHashSet(PartyUIDList, static s => (s.Get(out long v), v))) return false;
        return true;
    }
}

public sealed class KEPUBLISHER_REG_USER_REQ : IKSerializable
{
    public long ActorUid { get; set; }
    public string ServiceAccountId { get; set; } = string.Empty;
    public string ServicePassword { get; set; } = string.Empty;
    public string IP { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ActorUid)) return false;
        if (!ks.PutWideString(ServiceAccountId)) return false;
        if (!ks.PutWideString(ServicePassword)) return false;
        if (!ks.PutNarrowString(IP)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; ActorUid = v0;
        if (!ks.GetWideString(out string s1)) return false; ServiceAccountId = s1;
        if (!ks.GetWideString(out string s2)) return false; ServicePassword = s2;
        if (!ks.GetNarrowString(out string s3)) return false; IP = s3;
        return true;
    }
}

public sealed class KEPUBLISHER_REPORT_HACK_USER_NOT : IKSerializable
{
    public long UserUid { get; set; }
    public string UserId { get; set; } = string.Empty;
    public long UnitUid { get; set; }
    public string NickName { get; set; } = string.Empty;
    public long ServerUid { get; set; }
    public string IP { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UserUid)) return false;
        if (!ks.PutWideString(UserId)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutWideString(NickName)) return false;
        if (!ks.Put(ServerUid)) return false;
        if (!ks.PutNarrowString(IP)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UserUid = v0;
        if (!ks.GetWideString(out string s1)) return false; UserId = s1;
        if (!ks.Get(out long v2)) return false; UnitUid = v2;
        if (!ks.GetWideString(out string s3)) return false; NickName = s3;
        if (!ks.Get(out long v4)) return false; ServerUid = v4;
        if (!ks.GetNarrowString(out string s5)) return false; IP = s5;
        return true;
    }
}

public sealed class KEPUBLISHER_UNREG_USER_REQ : IKSerializable
{
    public long ActorUid { get; set; }
    public string ServiceAccountId { get; set; } = string.Empty;
    public string ServicePassword { get; set; } = string.Empty;
    public string IP { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ActorUid)) return false;
        if (!ks.PutWideString(ServiceAccountId)) return false;
        if (!ks.PutWideString(ServicePassword)) return false;
        if (!ks.PutNarrowString(IP)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; ActorUid = v0;
        if (!ks.GetWideString(out string s1)) return false; ServiceAccountId = s1;
        if (!ks.GetWideString(out string s2)) return false; ServicePassword = s2;
        if (!ks.GetNarrowString(out string s3)) return false; IP = s3;
        return true;
    }
}

public sealed class KERM_ACTIVATE_BUFF_NOT : IKSerializable
{
    public List<KBuffInfo> ActivateBuffList { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutList(ActivateBuffList)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetList(ActivateBuffList)) return false;
        return true;
    }
}

public sealed class KERM_BAD_ATTITUDE_USER_LOG_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public int DungeonIDAndDif { get; set; }
    public short StartMember { get; set; }
    public short NowMember { get; set; }
    public short VoteCount { get; set; }
    public short ExitStage { get; set; }
    public short ExitSubStage { get; set; }
    public short BadAttitudePoint { get; set; }
    public string RegDate { get; set; } = string.Empty;
    public bool ExitResult { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(DungeonIDAndDif)) return false;
        if (!ks.Put(StartMember)) return false;
        if (!ks.Put(NowMember)) return false;
        if (!ks.Put(VoteCount)) return false;
        if (!ks.Put(ExitStage)) return false;
        if (!ks.Put(ExitSubStage)) return false;
        if (!ks.Put(BadAttitudePoint)) return false;
        if (!ks.PutWideString(RegDate)) return false;
        if (!ks.Put(ExitResult)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out int v1)) return false; DungeonIDAndDif = v1;
        if (!ks.Get(out short v2)) return false; StartMember = v2;
        if (!ks.Get(out short v3)) return false; NowMember = v3;
        if (!ks.Get(out short v4)) return false; VoteCount = v4;
        if (!ks.Get(out short v5)) return false; ExitStage = v5;
        if (!ks.Get(out short v6)) return false; ExitSubStage = v6;
        if (!ks.Get(out short v7)) return false; BadAttitudePoint = v7;
        if (!ks.GetWideString(out string s8)) return false; RegDate = s8;
        if (!ks.Get(out bool v9)) return false; ExitResult = v9;
        return true;
    }
}

public sealed class KERM_BATTLE_FIELD_ZU_ECHO_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public long RoomUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(RoomUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out long v1)) return false; RoomUid = v1;
        return true;
    }
}

public sealed class KERM_BREAK_PERSONAL_SHOP_NOT : IKSerializable
{
    public int Reason { get; set; }
    public long HostUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Reason)) return false;
        if (!ks.Put(HostUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Reason = v0;
        if (!ks.Get(out long v1)) return false; HostUid = v1;
        return true;
    }
}

public sealed class KERM_BREAK_PSHOP_AGENCY_NOT : IKSerializable
{
    public int Reason { get; set; }
    public long HostGSUid { get; set; }
    public long HostUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Reason)) return false;
        if (!ks.Put(HostGSUid)) return false;
        if (!ks.Put(HostUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Reason = v0;
        if (!ks.Get(out long v1)) return false; HostGSUid = v1;
        if (!ks.Get(out long v2)) return false; HostUid = v2;
        return true;
    }
}

public sealed class KERM_BUY_PERSONAL_SHOP_ITEM_COMPLETE_NOT : IKSerializable
{
    public long SellUnitUid { get; set; }
    public int SellUnitEDIN { get; set; }
    public int SellCommissionED { get; set; }
    public KInventoryItemInfo SellIntemInfoOUT { get; set; } = new();
    public long PriceUnitUid { get; set; }
    public int PriceUnitEDOUT { get; set; }
    public int PriceCommissionED { get; set; }
    public KInventoryItemInfo PriceIntemInfoIN { get; set; } = new();
    public bool IsSquare { get; set; }
    public KItemQuantityUpdate ItemQuantityUpdateTemp { get; set; } = new();
    public List<KItemInfo> ItemInfoTemp { get; set; } = new();
    public int EDTemp { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(SellUnitUid)) return false;
        if (!ks.Put(SellUnitEDIN)) return false;
        if (!ks.Put(SellCommissionED)) return false;
        if (!ks.Put(SellIntemInfoOUT)) return false;
        if (!ks.Put(PriceUnitUid)) return false;
        if (!ks.Put(PriceUnitEDOUT)) return false;
        if (!ks.Put(PriceCommissionED)) return false;
        if (!ks.Put(PriceIntemInfoIN)) return false;
        if (!ks.Put(IsSquare)) return false;
        if (!ks.Put(ItemQuantityUpdateTemp)) return false;
        if (!ks.PutList(ItemInfoTemp)) return false;
        if (!ks.Put(EDTemp)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; SellUnitUid = v0;
        if (!ks.Get(out int v1)) return false; SellUnitEDIN = v1;
        if (!ks.Get(out int v2)) return false; SellCommissionED = v2;
        if (!ks.Get(SellIntemInfoOUT)) return false;
        if (!ks.Get(out long v4)) return false; PriceUnitUid = v4;
        if (!ks.Get(out int v5)) return false; PriceUnitEDOUT = v5;
        if (!ks.Get(out int v6)) return false; PriceCommissionED = v6;
        if (!ks.Get(PriceIntemInfoIN)) return false;
        if (!ks.Get(out bool v8)) return false; IsSquare = v8;
        if (!ks.Get(ItemQuantityUpdateTemp)) return false;
        if (!ks.GetList(ItemInfoTemp)) return false;
        if (!ks.Get(out int v11)) return false; EDTemp = v11;
        return true;
    }
}

public sealed class KERM_CALL_MY_LOVER_CHECK_ROOM_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KBattleFieldRoomInfo BTInfo { get; set; } = new();
    public KVector3 VPos { get; set; } = new();
    public byte LastTouchLineIndex { get; set; }
    public long LoverUnitUid { get; set; }
    public sbyte CallType { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(BTInfo)) return false;
        if (!ks.Put(VPos)) return false;
        if (!ks.Put(LastTouchLineIndex)) return false;
        if (!ks.Put(LoverUnitUid)) return false;
        if (!ks.Put(CallType)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(BTInfo)) return false;
        if (!ks.Get(VPos)) return false;
        if (!ks.Get(out byte v3)) return false; LastTouchLineIndex = v3;
        if (!ks.Get(out long v4)) return false; LoverUnitUid = v4;
        if (!ks.Get(out sbyte v5)) return false; CallType = v5;
        return true;
    }
}

public sealed class KERM_CALL_MY_LOVER_LEAVE_FIELD_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int Reason { get; set; }
    public int MapId { get; set; }
    public KVector3 VPos { get; set; } = new();
    public byte LastTouchLineIndex { get; set; }
    public int LimitLevel { get; set; }
    public int RequireDungeonId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(Reason)) return false;
        if (!ks.Put(MapId)) return false;
        if (!ks.Put(VPos)) return false;
        if (!ks.Put(LastTouchLineIndex)) return false;
        if (!ks.Put(LimitLevel)) return false;
        if (!ks.Put(RequireDungeonId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out int v1)) return false; Reason = v1;
        if (!ks.Get(out int v2)) return false; MapId = v2;
        if (!ks.Get(VPos)) return false;
        if (!ks.Get(out byte v4)) return false; LastTouchLineIndex = v4;
        if (!ks.Get(out int v5)) return false; LimitLevel = v5;
        if (!ks.Get(out int v6)) return false; RequireDungeonId = v6;
        return true;
    }
}

public sealed class KERM_CALL_MY_LOVER_LEAVE_FIELD_REQ : IKSerializable
{
    public int Reason { get; set; }
    public int MapId { get; set; }
    public KVector3 VPos { get; set; } = new();
    public byte LastTouchLineIndex { get; set; }
    public int LimitLevel { get; set; }
    public int RequireDungeonId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Reason)) return false;
        if (!ks.Put(MapId)) return false;
        if (!ks.Put(VPos)) return false;
        if (!ks.Put(LastTouchLineIndex)) return false;
        if (!ks.Put(LimitLevel)) return false;
        if (!ks.Put(RequireDungeonId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Reason = v0;
        if (!ks.Get(out int v1)) return false; MapId = v1;
        if (!ks.Get(VPos)) return false;
        if (!ks.Get(out byte v3)) return false; LastTouchLineIndex = v3;
        if (!ks.Get(out int v4)) return false; LimitLevel = v4;
        if (!ks.Get(out int v5)) return false; RequireDungeonId = v5;
        return true;
    }
}

public sealed class KERM_CHANGE_DEFENCE_DUNGEON_PHASE_NOT : IKSerializable
{
    public int DefenceDungeonPhase { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(DefenceDungeonPhase)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; DefenceDungeonPhase = v0;
        return true;
    }
}

public sealed class KERM_CHANGE_EQUIPPED_ITEM_IN_FIELD_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public List<KInventoryItemInfo> InventorySlotInfo { get; set; } = new();
    public int EquippedTitleId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutList(InventorySlotInfo)) return false;
        if (!ks.Put(EquippedTitleId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.GetList(InventorySlotInfo)) return false;
        if (!ks.Get(out int v2)) return false; EquippedTitleId = v2;
        return true;
    }
}

public sealed class KERM_CHANGE_FIELD_USER_INFO_NOT : IKSerializable
{
    public KFieldUserInfo Info { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Info)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(Info)) return false;
        return true;
    }
}

public sealed class KERM_CHANGE_READY_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int DungeonId { get; set; }
    public sbyte DungeonMode { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(DungeonId)) return false;
        if (!ks.Put(DungeonMode)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out int v1)) return false; DungeonId = v1;
        if (!ks.Get(out sbyte v2)) return false; DungeonMode = v2;
        return true;
    }
}

public sealed class KERM_CHAT_ACK : IKSerializable
{
    public int Ok { get; set; }
    public sbyte ChatPacketType { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(ChatPacketType)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out sbyte v1)) return false; ChatPacketType = v1;
        return true;
    }
}

public sealed class KERM_CHECK_INVALID_ZOMBIE_USER_NOT : IKSerializable
{
    public long RoomUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(RoomUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; RoomUid = v0;
        return true;
    }
}

public sealed class KERM_CHECK_JOIN_PERSONAL_SHOP_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long PersonalShopUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(PersonalShopUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out long v1)) return false; PersonalShopUid = v1;
        return true;
    }
}

public sealed class KERM_CHECK_MY_PSHOP_AGENCY_INFO_ACK : IKSerializable
{
    public long PShopAgencyUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PShopAgencyUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; PShopAgencyUid = v0;
        return true;
    }
}

public sealed class KERM_CHECK_MY_PSHOP_AGENCY_INFO_REQ : IKSerializable
{
    public long UserUid { get; set; }
    public long UnitUid { get; set; }
    public string NickName { get; set; } = string.Empty;
    public sbyte PersonalShopType { get; set; }
    public string AgencyExpirationDate { get; set; } = string.Empty;
    public string AgencyOpenDate { get; set; } = string.Empty;
    public string PersonalShopName { get; set; } = string.Empty;
    public List<KSellPShopItemBackupData> SellItemInfo { get; set; } = new();
    public bool OnSale { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UserUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutWideString(NickName)) return false;
        if (!ks.Put(PersonalShopType)) return false;
        if (!ks.PutWideString(AgencyExpirationDate)) return false;
        if (!ks.PutWideString(AgencyOpenDate)) return false;
        if (!ks.PutWideString(PersonalShopName)) return false;
        if (!ks.PutList(SellItemInfo)) return false;
        if (!ks.Put(OnSale)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UserUid = v0;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        if (!ks.GetWideString(out string s2)) return false; NickName = s2;
        if (!ks.Get(out sbyte v3)) return false; PersonalShopType = v3;
        if (!ks.GetWideString(out string s4)) return false; AgencyExpirationDate = s4;
        if (!ks.GetWideString(out string s5)) return false; AgencyOpenDate = s5;
        if (!ks.GetWideString(out string s6)) return false; PersonalShopName = s6;
        if (!ks.GetList(SellItemInfo)) return false;
        if (!ks.Get(out bool v8)) return false; OnSale = v8;
        return true;
    }
}

public sealed class KERM_CHECK_SELL_ED_ITEM_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long ItemUid { get; set; }
    public int Quantity { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(ItemUid)) return false;
        if (!ks.Put(Quantity)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out long v1)) return false; ItemUid = v1;
        if (!ks.Get(out int v2)) return false; Quantity = v2;
        return true;
    }
}

public sealed class KERM_CHECK_ZOMBIE_USER_ACK : IKSerializable
{
    // enum ZOMBIE_CHECK_TYPE
    public const int ZCT_CONNECT_NORMAL = 0;
    public const int ZCT_CONNECT_ZOMBIE = 1;
    public const int ZCT_JOIN_DIF_ROOM = 2;
    public const int ZCT_DISCONNECT_ZOMBIE = 3;

    public sbyte ZombieCheckType { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ZombieCheckType)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte v0)) return false; ZombieCheckType = v0;
        return true;
    }
}

public sealed class KERM_CHECK_ZOMBIE_USER_REQ : IKSerializable
{
    public long RoomUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(RoomUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; RoomUid = v0;
        return true;
    }
}

public sealed class KERM_CONFIRM_BUY_PERSONAL_SHOP_ITEM_REQ : IKSerializable
{
    public KInventoryItemInfo SellItemInfo { get; set; } = new();
    public int BuyQuantity { get; set; }
    public long BuyUnitUid { get; set; }
    public int TotalPrice { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(SellItemInfo)) return false;
        if (!ks.Put(BuyQuantity)) return false;
        if (!ks.Put(BuyUnitUid)) return false;
        if (!ks.Put(TotalPrice)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(SellItemInfo)) return false;
        if (!ks.Get(out int v1)) return false; BuyQuantity = v1;
        if (!ks.Get(out long v2)) return false; BuyUnitUid = v2;
        if (!ks.Get(out int v3)) return false; TotalPrice = v3;
        return true;
    }
}

public sealed class KERM_CONFIRM_TRADE_NOT : IKSerializable
{
    public long TradeUid { get; set; }
    public List<KInventoryItemInfo> ItemIn { get; set; } = new();
    public List<KInventoryItemInfo> ItemOut { get; set; } = new();
    public int EDIn { get; set; }
    public int EDOut { get; set; }
    public KItemQuantityUpdate ItemQuantityUpdateTemp { get; set; } = new();
    public List<KItemInfo> ItemInfoTemp { get; set; } = new();
    public int EDTemp { get; set; }
    public long OtherCId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(TradeUid)) return false;
        if (!ks.PutList(ItemIn)) return false;
        if (!ks.PutList(ItemOut)) return false;
        if (!ks.Put(EDIn)) return false;
        if (!ks.Put(EDOut)) return false;
        if (!ks.Put(ItemQuantityUpdateTemp)) return false;
        if (!ks.PutList(ItemInfoTemp)) return false;
        if (!ks.Put(EDTemp)) return false;
        if (!ks.Put(OtherCId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; TradeUid = v0;
        if (!ks.GetList(ItemIn)) return false;
        if (!ks.GetList(ItemOut)) return false;
        if (!ks.Get(out int v3)) return false; EDIn = v3;
        if (!ks.Get(out int v4)) return false; EDOut = v4;
        if (!ks.Get(ItemQuantityUpdateTemp)) return false;
        if (!ks.GetList(ItemInfoTemp)) return false;
        if (!ks.Get(out int v7)) return false; EDTemp = v7;
        if (!ks.Get(out long v8)) return false; OtherCId = v8;
        return true;
    }
}

public sealed class KERM_CONFIRM_TRADE_REQ : IKSerializable
{
    public List<KInventoryItemInfo> ItemIn { get; set; } = new();
    public List<KInventoryItemInfo> ItemOut { get; set; } = new();
    public int EDIn { get; set; }
    public int EDOut { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutList(ItemIn)) return false;
        if (!ks.PutList(ItemOut)) return false;
        if (!ks.Put(EDIn)) return false;
        if (!ks.Put(EDOut)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetList(ItemIn)) return false;
        if (!ks.GetList(ItemOut)) return false;
        if (!ks.Get(out int v2)) return false; EDIn = v2;
        if (!ks.Get(out int v3)) return false; EDOut = v3;
        return true;
    }
}

public class KERM_COUPLE_PROPOSE_RESULT_FAIL_NOT : IKSerializable
{
    public int Ok { get; set; }
    public long RequestUnitUid { get; set; }
    public string RequestUnitName { get; set; } = string.Empty;
    public long AcceptUnitUid { get; set; }
    public string AcceptUnitName { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(RequestUnitUid)) return false;
        if (!ks.PutWideString(RequestUnitName)) return false;
        if (!ks.Put(AcceptUnitUid)) return false;
        if (!ks.PutWideString(AcceptUnitName)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out long v1)) return false; RequestUnitUid = v1;
        if (!ks.GetWideString(out string s2)) return false; RequestUnitName = s2;
        if (!ks.Get(out long v3)) return false; AcceptUnitUid = v3;
        if (!ks.GetWideString(out string s4)) return false; AcceptUnitName = s4;
        return true;
    }
}

public class KERM_COUPLE_PROPOSE_RESULT_SUCCESS_NOT : IKSerializable
{
    public int Ok { get; set; }
    public long RequestUnitUid { get; set; }
    public string RequestUnitName { get; set; } = string.Empty;
    public long AcceptUnitUid { get; set; }
    public string AcceptUnitName { get; set; } = string.Empty;
    public long Date { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(RequestUnitUid)) return false;
        if (!ks.PutWideString(RequestUnitName)) return false;
        if (!ks.Put(AcceptUnitUid)) return false;
        if (!ks.PutWideString(AcceptUnitName)) return false;
        if (!ks.Put(Date)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out long v1)) return false; RequestUnitUid = v1;
        if (!ks.GetWideString(out string s2)) return false; RequestUnitName = s2;
        if (!ks.Get(out long v3)) return false; AcceptUnitUid = v3;
        if (!ks.GetWideString(out string s4)) return false; AcceptUnitName = s4;
        if (!ks.Get(out long v5)) return false; Date = v5;
        return true;
    }
}

public sealed class KERM_DECREASE_ENDURANCE_DUNGEON_NOT : IKSerializable
{
    public sbyte PartyNumMember { get; set; }
    public int EnduranceDamage { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PartyNumMember)) return false;
        if (!ks.Put(EnduranceDamage)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte v0)) return false; PartyNumMember = v0;
        if (!ks.Get(out int v1)) return false; EnduranceDamage = v1;
        return true;
    }
}

public sealed class KERM_DECREASE_PET_SATIETY_NOT : IKSerializable
{
    public sbyte TeamNumMember { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(TeamNumMember)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte v0)) return false; TeamNumMember = v0;
        return true;
    }
}

public sealed class KERM_DUNGEON_USER_STATISTICS_NOT : IKSerializable
{
    // enum US_GAME_RESULT
    public const int UGR_WIN = 0;
    public const int UGR_LOSE = 1;
    public const int UGR_DROP = 2;

    public int DungeonId { get; set; }
    public sbyte GameResult { get; set; }
    public bool IsPartyPlay { get; set; }
    public int GamePlayTime { get; set; }
    public int UsedResurrectionStone { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(DungeonId)) return false;
        if (!ks.Put(GameResult)) return false;
        if (!ks.Put(IsPartyPlay)) return false;
        if (!ks.Put(GamePlayTime)) return false;
        if (!ks.Put(UsedResurrectionStone)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; DungeonId = v0;
        if (!ks.Get(out sbyte v1)) return false; GameResult = v1;
        if (!ks.Get(out bool v2)) return false; IsPartyPlay = v2;
        if (!ks.Get(out int v3)) return false; GamePlayTime = v3;
        if (!ks.Get(out int v4)) return false; UsedResurrectionStone = v4;
        return true;
    }
}

public sealed class KERM_END_GAME_NOT : IKSerializable
{
    public int WinTeam { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(WinTeam)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; WinTeam = v0;
        return true;
    }
}

public sealed class KERM_END_GAME_REGROUP_PARTY_NOT : IKSerializable
{
    public long GsUid { get; set; }
    public long PartyUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(GsUid)) return false;
        if (!ks.Put(PartyUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; GsUid = v0;
        if (!ks.Get(out long v1)) return false; PartyUid = v1;
        return true;
    }
}

public sealed class KERM_END_TC_GAME_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int TCId { get; set; }
    public bool IsSuccess { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(TCId)) return false;
        if (!ks.Put(IsSuccess)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out int v1)) return false; TCId = v1;
        if (!ks.Get(out bool v2)) return false; IsSuccess = v2;
        return true;
    }
}

public sealed class KERM_FIELD_USER_LIST_MONITORING_MODE_REQ : IKSerializable
{
    public uint ViewPage { get; set; }
    public Dictionary<long, int> Unit_ED { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ViewPage)) return false;
        if (!ks.PutMap(Unit_ED, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint v0)) return false; ViewPage = v0;
        if (!ks.GetMap(Unit_ED, static s => (s.Get(out long v), v), static s => (s.Get(out int v), v))) return false;
        return true;
    }
}

public sealed class KERM_FIELD_USER_STATISTICS_NOT : IKSerializable
{
    public int GamePlayTime { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(GamePlayTime)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; GamePlayTime = v0;
        return true;
    }
}

public sealed class KERM_GAME_START_NOT : IKSerializable
{
    public sbyte RoomState { get; set; }
    public int GameType { get; set; }
    public List<KRoomSlotInfo> Slot { get; set; } = new();
    public int DungeonId { get; set; }
    public short WorldId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(RoomState)) return false;
        if (!ks.Put(GameType)) return false;
        if (!ks.PutList(Slot)) return false;
        if (!ks.Put(DungeonId)) return false;
        if (!ks.Put(WorldId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte v0)) return false; RoomState = v0;
        if (!ks.Get(out int v1)) return false; GameType = v1;
        if (!ks.GetList(Slot)) return false;
        if (!ks.Get(out int v3)) return false; DungeonId = v3;
        if (!ks.Get(out short v4)) return false; WorldId = v4;
        return true;
    }
}

public sealed class KERM_GET_ITEM_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KItemInfo ItemInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(ItemInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(ItemInfo)) return false;
        return true;
    }
}

public sealed class KERM_JOIN_BATTLE_FIELD_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KBattleFieldJoinInfo BattleFieldJoinInfo { get; set; } = new();
    public KRoomInfo RoomInfo { get; set; } = new();
    public List<KRoomSlotInfo> Slot { get; set; } = new();
    public string CNIP { get; set; } = string.Empty;
    public int RequireLevel { get; set; }
    public int RequireDungeonId { get; set; }
    public string UDPRelayIP { get; set; } = string.Empty;
    public ushort UDPRelayPort { get; set; }
    public List<long> EnterCashShopUser { get; set; } = new();
    public long RemainFieldHoldingTime { get; set; }
    public KBossFieldJoinInfo BossFieldJoinInfo { get; set; } = new();
    public bool BossField { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(BattleFieldJoinInfo)) return false;
        if (!ks.Put(RoomInfo)) return false;
        if (!ks.PutList(Slot)) return false;
        if (!ks.PutWideString(CNIP)) return false;
        if (!ks.Put(RequireLevel)) return false;
        if (!ks.Put(RequireDungeonId)) return false;
        if (!ks.PutWideString(UDPRelayIP)) return false;
        if (!ks.Put(UDPRelayPort)) return false;
        if (!ks.PutList(EnterCashShopUser, static (s, v) => s.Put(v))) return false;
        if (!ks.Put(RemainFieldHoldingTime)) return false;
        if (!ks.Put(BossFieldJoinInfo)) return false;
        if (!ks.Put(BossField)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(BattleFieldJoinInfo)) return false;
        if (!ks.Get(RoomInfo)) return false;
        if (!ks.GetList(Slot)) return false;
        if (!ks.GetWideString(out string s4)) return false; CNIP = s4;
        if (!ks.Get(out int v5)) return false; RequireLevel = v5;
        if (!ks.Get(out int v6)) return false; RequireDungeonId = v6;
        if (!ks.GetWideString(out string s7)) return false; UDPRelayIP = s7;
        if (!ks.Get(out ushort v8)) return false; UDPRelayPort = v8;
        if (!ks.GetList(EnterCashShopUser, static s => (s.Get(out long v), v))) return false;
        if (!ks.Get(out long v10)) return false; RemainFieldHoldingTime = v10;
        if (!ks.Get(BossFieldJoinInfo)) return false;
        if (!ks.Get(out bool v12)) return false; BossField = v12;
        return true;
    }
}

public sealed class KERM_JOIN_BATTLE_FIELD_REQ : IKSerializable
{
    public KRoomUserInfo RoomUserInfo { get; set; } = new();
    public List<long> StudentUnitUid { get; set; } = new();
    public KBattleFieldJoinInfo BattleFieldJoinInfo { get; set; } = new();
    public KReturnToFieldInfo ReturnToFieldInfo { get; set; } = new();
    public KBossFieldJoinInfo BossFieldJoinInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(RoomUserInfo)) return false;
        if (!ks.PutList(StudentUnitUid, static (s, v) => s.Put(v))) return false;
        if (!ks.Put(BattleFieldJoinInfo)) return false;
        if (!ks.Put(ReturnToFieldInfo)) return false;
        if (!ks.Put(BossFieldJoinInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(RoomUserInfo)) return false;
        if (!ks.GetList(StudentUnitUid, static s => (s.Get(out long v), v))) return false;
        if (!ks.Get(BattleFieldJoinInfo)) return false;
        if (!ks.Get(ReturnToFieldInfo)) return false;
        if (!ks.Get(BossFieldJoinInfo)) return false;
        return true;
    }
}

public sealed class KERM_JOIN_FIELD_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long FieldUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(FieldUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out long v1)) return false; FieldUid = v1;
        return true;
    }
}

public sealed class KERM_JOIN_FIELD_REQ : IKSerializable
{
    public long FieldUid { get; set; }
    public KFieldUserInfo FieldUserInfo { get; set; } = new();
    public HashSet<long> PartyUIDList { get; set; } = new();
    public HashSet<long> FriendUIDList { get; set; } = new();
    public KFieldOption FieldOption { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(FieldUid)) return false;
        if (!ks.Put(FieldUserInfo)) return false;
        if (!ks.PutList(PartyUIDList, static (s, v) => s.Put(v))) return false;
        if (!ks.PutList(FriendUIDList, static (s, v) => s.Put(v))) return false;
        if (!ks.Put(FieldOption)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; FieldUid = v0;
        if (!ks.Get(FieldUserInfo)) return false;
        if (!ks.GetHashSet(PartyUIDList, static s => (s.Get(out long v), v))) return false;
        if (!ks.GetHashSet(FriendUIDList, static s => (s.Get(out long v), v))) return false;
        if (!ks.Get(FieldOption)) return false;
        return true;
    }
}

public sealed class KERM_JOIN_PERSONAL_SHOP_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long PersonalShopUid { get; set; }
    public string PersonalShopName { get; set; } = string.Empty;
    public string NickName { get; set; } = string.Empty;
    public List<KSellPersonalShopItemInfo> SellItemInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(PersonalShopUid)) return false;
        if (!ks.PutWideString(PersonalShopName)) return false;
        if (!ks.PutWideString(NickName)) return false;
        if (!ks.PutList(SellItemInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out long v1)) return false; PersonalShopUid = v1;
        if (!ks.GetWideString(out string s2)) return false; PersonalShopName = s2;
        if (!ks.GetWideString(out string s3)) return false; NickName = s3;
        if (!ks.GetList(SellItemInfo)) return false;
        return true;
    }
}

public sealed class KERM_JOIN_TRADE_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long TradeUid { get; set; }
    public List<KTradeUserInfo> TradeUserInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(TradeUid)) return false;
        if (!ks.PutList(TradeUserInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out long v1)) return false; TradeUid = v1;
        if (!ks.GetList(TradeUserInfo)) return false;
        return true;
    }
}

public sealed class KERM_LEAVE_FIELD_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int Reason { get; set; }
    public int MapId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(Reason)) return false;
        if (!ks.Put(MapId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out int v1)) return false; Reason = v1;
        if (!ks.Get(out int v2)) return false; MapId = v2;
        return true;
    }
}

public sealed class KERM_LEAVE_FIELD_REQ : IKSerializable
{
    public int Reason { get; set; }
    public int MapId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Reason)) return false;
        if (!ks.Put(MapId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Reason = v0;
        if (!ks.Get(out int v1)) return false; MapId = v1;
        return true;
    }
}

public sealed class KERM_LOAD_PSHOP_BOARD_INFO_NOT : IKSerializable
{
    public long GsUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(GsUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; GsUid = v0;
        return true;
    }
}

public sealed class KERM_NPC_UNIT_DIE_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int NpcId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(NpcId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out int v1)) return false; NpcId = v1;
        return true;
    }
}

public sealed class KERM_NPC_UNIT_DIE_NOT : IKSerializable
{
    public int DungeonId { get; set; }
    public sbyte Difficulty { get; set; }
    public int DieNPCUid { get; set; }
    public int NpcId { get; set; }
    public sbyte AttUnitType { get; set; }
    public long AttUnit { get; set; }
    public Dictionary<long, KEXPData> EXPList { get; set; } = new();
    public short SubStageExpNpcNum { get; set; }
    public sbyte UserCount { get; set; }
    public bool NoDrop { get; set; }
    public bool DecreaseEndurance { get; set; }
    public bool IsPvpNpc { get; set; }
    public int GameType { get; set; }
    public sbyte DungeonMode { get; set; }
    public bool QuestComplete { get; set; }
    public bool HeroNPC { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(DungeonId)) return false;
        if (!ks.Put(Difficulty)) return false;
        if (!ks.Put(DieNPCUid)) return false;
        if (!ks.Put(NpcId)) return false;
        if (!ks.Put(AttUnitType)) return false;
        if (!ks.Put(AttUnit)) return false;
        if (!ks.PutMap(EXPList, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.Put(SubStageExpNpcNum)) return false;
        if (!ks.Put(UserCount)) return false;
        if (!ks.Put(NoDrop)) return false;
        if (!ks.Put(DecreaseEndurance)) return false;
        if (!ks.Put(IsPvpNpc)) return false;
        if (!ks.Put(GameType)) return false;
        if (!ks.Put(DungeonMode)) return false;
        if (!ks.Put(QuestComplete)) return false;
        if (!ks.Put(HeroNPC)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; DungeonId = v0;
        if (!ks.Get(out sbyte v1)) return false; Difficulty = v1;
        if (!ks.Get(out int v2)) return false; DieNPCUid = v2;
        if (!ks.Get(out int v3)) return false; NpcId = v3;
        if (!ks.Get(out sbyte v4)) return false; AttUnitType = v4;
        if (!ks.Get(out long v5)) return false; AttUnit = v5;
        if (!ks.GetMap(EXPList, static s => (s.Get(out long v), v), static s => { var v = new KEXPData(); return (s.Get(v), v); })) return false;
        if (!ks.Get(out short v7)) return false; SubStageExpNpcNum = v7;
        if (!ks.Get(out sbyte v8)) return false; UserCount = v8;
        if (!ks.Get(out bool v9)) return false; NoDrop = v9;
        if (!ks.Get(out bool v10)) return false; DecreaseEndurance = v10;
        if (!ks.Get(out bool v11)) return false; IsPvpNpc = v11;
        if (!ks.Get(out int v12)) return false; GameType = v12;
        if (!ks.Get(out sbyte v13)) return false; DungeonMode = v13;
        if (!ks.Get(out bool v14)) return false; QuestComplete = v14;
        if (!ks.Get(out bool v15)) return false; HeroNPC = v15;
        return true;
    }
}

public sealed class KERM_OPEN_BATTLE_FIELD_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KRoomInfo RoomInfo { get; set; } = new();
    public List<KRoomSlotInfo> Slot { get; set; } = new();
    public string CNIP { get; set; } = string.Empty;
    public KBattleFieldJoinInfo BattleFieldJoinInfo { get; set; } = new();
    public string UDPRelayIP { get; set; } = string.Empty;
    public ushort UDPRelayPort { get; set; }
    public long RemainFieldHoldingTime { get; set; }
    public KBossFieldJoinInfo BossFieldJoinInfo { get; set; } = new();
    public bool BossField { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(RoomInfo)) return false;
        if (!ks.PutList(Slot)) return false;
        if (!ks.PutWideString(CNIP)) return false;
        if (!ks.Put(BattleFieldJoinInfo)) return false;
        if (!ks.PutWideString(UDPRelayIP)) return false;
        if (!ks.Put(UDPRelayPort)) return false;
        if (!ks.Put(RemainFieldHoldingTime)) return false;
        if (!ks.Put(BossFieldJoinInfo)) return false;
        if (!ks.Put(BossField)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(RoomInfo)) return false;
        if (!ks.GetList(Slot)) return false;
        if (!ks.GetWideString(out string s3)) return false; CNIP = s3;
        if (!ks.Get(BattleFieldJoinInfo)) return false;
        if (!ks.GetWideString(out string s5)) return false; UDPRelayIP = s5;
        if (!ks.Get(out ushort v6)) return false; UDPRelayPort = v6;
        if (!ks.Get(out long v7)) return false; RemainFieldHoldingTime = v7;
        if (!ks.Get(BossFieldJoinInfo)) return false;
        if (!ks.Get(out bool v9)) return false; BossField = v9;
        return true;
    }
}

public sealed class KERM_OPEN_BATTLE_FIELD_REQ : IKSerializable
{
    public KRoomInfo RoomInfo { get; set; } = new();
    public KRoomUserInfo RoomUserInfo { get; set; } = new();
    public List<long> StudentUnitUid { get; set; } = new();
    public KBattleFieldJoinInfo BattleFieldJoinInfo { get; set; } = new();
    public KReturnToFieldInfo ReturnToFieldInfo { get; set; } = new();
    public KBossFieldCreateInfo KBossFieldCreateInfo { get; set; } = new();
    public KBossFieldJoinInfo BossFieldJoinInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(RoomInfo)) return false;
        if (!ks.Put(RoomUserInfo)) return false;
        if (!ks.PutList(StudentUnitUid, static (s, v) => s.Put(v))) return false;
        if (!ks.Put(BattleFieldJoinInfo)) return false;
        if (!ks.Put(ReturnToFieldInfo)) return false;
        if (!ks.Put(KBossFieldCreateInfo)) return false;
        if (!ks.Put(BossFieldJoinInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(RoomInfo)) return false;
        if (!ks.Get(RoomUserInfo)) return false;
        if (!ks.GetList(StudentUnitUid, static s => (s.Get(out long v), v))) return false;
        if (!ks.Get(BattleFieldJoinInfo)) return false;
        if (!ks.Get(ReturnToFieldInfo)) return false;
        if (!ks.Get(KBossFieldCreateInfo)) return false;
        if (!ks.Get(BossFieldJoinInfo)) return false;
        return true;
    }
}

public sealed class KERM_OPEN_PERSONAL_SHOP_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long PersonalShopUid { get; set; }
    public sbyte PersonalShopType { get; set; }
    public long GsUid { get; set; }
    public long UserUid { get; set; }
    public long UnitUid { get; set; }
    public string AgencyOpenDate { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(PersonalShopUid)) return false;
        if (!ks.Put(PersonalShopType)) return false;
        if (!ks.Put(GsUid)) return false;
        if (!ks.Put(UserUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutWideString(AgencyOpenDate)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out long v1)) return false; PersonalShopUid = v1;
        if (!ks.Get(out sbyte v2)) return false; PersonalShopType = v2;
        if (!ks.Get(out long v3)) return false; GsUid = v3;
        if (!ks.Get(out long v4)) return false; UserUid = v4;
        if (!ks.Get(out long v5)) return false; UnitUid = v5;
        if (!ks.GetWideString(out string s6)) return false; AgencyOpenDate = s6;
        return true;
    }
}

public sealed class KERM_OPEN_PSHOP_AGENCY_BY_SERVER_NOT : IKSerializable
{
    public long UserUid { get; set; }
    public long UnitUid { get; set; }
    public string NickName { get; set; } = string.Empty;
    public sbyte PersonalShopType { get; set; }
    public string AgencyExpirationDate { get; set; } = string.Empty;
    public string AgencyOpenDate { get; set; } = string.Empty;
    public string PersonalShopName { get; set; } = string.Empty;
    public List<KSellPShopItemBackupData> SellItemInfo { get; set; } = new();
    public bool OnSale { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UserUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutWideString(NickName)) return false;
        if (!ks.Put(PersonalShopType)) return false;
        if (!ks.PutWideString(AgencyExpirationDate)) return false;
        if (!ks.PutWideString(AgencyOpenDate)) return false;
        if (!ks.PutWideString(PersonalShopName)) return false;
        if (!ks.PutList(SellItemInfo)) return false;
        if (!ks.Put(OnSale)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UserUid = v0;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        if (!ks.GetWideString(out string s2)) return false; NickName = s2;
        if (!ks.Get(out sbyte v3)) return false; PersonalShopType = v3;
        if (!ks.GetWideString(out string s4)) return false; AgencyExpirationDate = s4;
        if (!ks.GetWideString(out string s5)) return false; AgencyOpenDate = s5;
        if (!ks.GetWideString(out string s6)) return false; PersonalShopName = s6;
        if (!ks.GetList(SellItemInfo)) return false;
        if (!ks.Get(out bool v8)) return false; OnSale = v8;
        return true;
    }
}

public sealed class KERM_OPEN_TRADE_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long TradeUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(TradeUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out long v1)) return false; TradeUid = v1;
        return true;
    }
}

public class KERM_OPEN_WEDDING_HALL_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KWeddingHallInfo WeddingInfo { get; set; } = new();
    public KRoomInfo RoomInfo { get; set; } = new();
    public List<KRoomSlotInfo> Slot { get; set; } = new();
    public string CNIP { get; set; } = string.Empty;
    public string UDPRelayIP { get; set; } = string.Empty;
    public ushort UDPRelayPort { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(WeddingInfo)) return false;
        if (!ks.Put(RoomInfo)) return false;
        if (!ks.PutList(Slot)) return false;
        if (!ks.PutWideString(CNIP)) return false;
        if (!ks.PutWideString(UDPRelayIP)) return false;
        if (!ks.Put(UDPRelayPort)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(WeddingInfo)) return false;
        if (!ks.Get(RoomInfo)) return false;
        if (!ks.GetList(Slot)) return false;
        if (!ks.GetWideString(out string s4)) return false; CNIP = s4;
        if (!ks.GetWideString(out string s5)) return false; UDPRelayIP = s5;
        if (!ks.Get(out ushort v6)) return false; UDPRelayPort = v6;
        return true;
    }
}

public sealed class KERM_OPTION_UPDATE_NOT : IKSerializable
{
    public KFieldOption FieldOption { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(FieldOption)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(FieldOption)) return false;
        return true;
    }
}

public sealed class KERM_PARTY_OPEN_DUNGEON_ROOM_NOT : IKSerializable
{
    public short WorldId { get; set; }
    public KRoomInfo RoomInfo { get; set; } = new();
    public List<KRoomSlotInfo> Slot { get; set; } = new();
    public List<KRoomSlotInfo> ObserverSlot { get; set; } = new();
    public string CNIP { get; set; } = string.Empty;
    public List<KInventoryItemInfo> InventorySlotInfo { get; set; } = new();
    public Dictionary<long, List<KBuffInfo>> ActivateBuff { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(WorldId)) return false;
        if (!ks.Put(RoomInfo)) return false;
        if (!ks.PutList(Slot)) return false;
        if (!ks.PutList(ObserverSlot)) return false;
        if (!ks.PutWideString(CNIP)) return false;
        if (!ks.PutList(InventorySlotInfo)) return false;
        if (!ks.PutMap(ActivateBuff, static (s, k) => s.Put(k), static (s, v) => s.PutList(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out short v0)) return false; WorldId = v0;
        if (!ks.Get(RoomInfo)) return false;
        if (!ks.GetList(Slot)) return false;
        if (!ks.GetList(ObserverSlot)) return false;
        if (!ks.GetWideString(out string s4)) return false; CNIP = s4;
        if (!ks.GetList(InventorySlotInfo)) return false;
        if (!ks.GetMap(ActivateBuff, static s => (s.Get(out long v), v), static s => { var l = new List<KBuffInfo>(); return (s.GetList(l), l); })) return false;
        return true;
    }
}

public sealed class KERM_PARTY_OPEN_DUNGEON_ROOM_REQ : IKSerializable
{
    public KRoomInfo RoomInfo { get; set; } = new();
    public short WorldId { get; set; }
    public List<KPartyRoomUserInfo> UserInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(RoomInfo)) return false;
        if (!ks.Put(WorldId)) return false;
        if (!ks.PutList(UserInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(RoomInfo)) return false;
        if (!ks.Get(out short v1)) return false; WorldId = v1;
        if (!ks.GetList(UserInfo)) return false;
        return true;
    }
}

public sealed class KERM_PREPARE_FOR_DEFENCE_DUNGEON_REQ : IKSerializable
{
    public KDungeonGameInfo DefenceDungeonInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(DefenceDungeonInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(DefenceDungeonInfo)) return false;
        return true;
    }
}

public sealed class KERM_PREPARE_PICK_UP_FROM_PSHOP_AGENCY_REQ : IKSerializable
{
    public List<long> PickUpItemList { get; set; } = new();
    public Dictionary<int, int> PickUpItemInfo { get; set; } = new();
    public int TotalAddED { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutList(PickUpItemList, static (s, v) => s.Put(v))) return false;
        if (!ks.PutMap(PickUpItemInfo, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.Put(TotalAddED)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetList(PickUpItemList, static s => (s.Get(out long v), v))) return false;
        if (!ks.GetMap(PickUpItemInfo, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false;
        if (!ks.Get(out int v2)) return false; TotalAddED = v2;
        return true;
    }
}

public sealed class KERM_PVP_USER_STATISTICS_NOT : IKSerializable
{
    // enum US_GAME_RESULT
    public const int UGR_WIN = 0;
    public const int UGR_LOSE = 1;
    public const int UGR_DRAW = 2;
    public const int UGR_DROP = 3;

    public sbyte GameType { get; set; }
    public sbyte GameResult { get; set; }
    public short WorldId { get; set; }
    public bool Intrude { get; set; }
    public int GamePlayTime { get; set; }
    public int KillNum { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(GameType)) return false;
        if (!ks.Put(GameResult)) return false;
        if (!ks.Put(WorldId)) return false;
        if (!ks.Put(Intrude)) return false;
        if (!ks.Put(GamePlayTime)) return false;
        if (!ks.Put(KillNum)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte v0)) return false; GameType = v0;
        if (!ks.Get(out sbyte v1)) return false; GameResult = v1;
        if (!ks.Get(out short v2)) return false; WorldId = v2;
        if (!ks.Get(out bool v3)) return false; Intrude = v3;
        if (!ks.Get(out int v4)) return false; GamePlayTime = v4;
        if (!ks.Get(out int v5)) return false; KillNum = v5;
        return true;
    }
}

public sealed class KERM_REG_PERSONAL_SHOP_ITEM_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long GsUid { get; set; }
    public long UnitUid { get; set; }
    public List<KInventoryItemInfo> InventorySlotInfo { get; set; } = new();
    public int OpeningED { get; set; }
    public string PersonalShopName { get; set; } = string.Empty;
    public List<KSellPersonalShopItemInfo> SellItemInfo { get; set; } = new();
    public List<KSellPersonalShopItemInfo> AddCompleteItemInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(GsUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutList(InventorySlotInfo)) return false;
        if (!ks.Put(OpeningED)) return false;
        if (!ks.PutWideString(PersonalShopName)) return false;
        if (!ks.PutList(SellItemInfo)) return false;
        if (!ks.PutList(AddCompleteItemInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out long v1)) return false; GsUid = v1;
        if (!ks.Get(out long v2)) return false; UnitUid = v2;
        if (!ks.GetList(InventorySlotInfo)) return false;
        if (!ks.Get(out int v4)) return false; OpeningED = v4;
        if (!ks.GetWideString(out string s5)) return false; PersonalShopName = s5;
        if (!ks.GetList(SellItemInfo)) return false;
        if (!ks.GetList(AddCompleteItemInfo)) return false;
        return true;
    }
}

public sealed class KERM_START_WEDDING_NOT : IKSerializable
{
    public int Ok { get; set; }
    public long Groom { get; set; }
    public long Bride { get; set; }
    public Dictionary<int, int> RewardItem { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(Groom)) return false;
        if (!ks.Put(Bride)) return false;
        if (!ks.PutMap(RewardItem, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out long v1)) return false; Groom = v1;
        if (!ks.Get(out long v2)) return false; Bride = v2;
        if (!ks.GetMap(RewardItem, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false;
        return true;
    }
}

public sealed class KERM_TC_USER_STATISTICS_NOT : IKSerializable
{
    // enum US_GAME_RESULT
    public const int UGR_SUCCESS = 0;
    public const int UGR_FAIL = 1;
    public const int UGR_DROP = 2;

    public int TCId { get; set; }
    public sbyte GameResult { get; set; }
    public int GamePlayTime { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(TCId)) return false;
        if (!ks.Put(GameResult)) return false;
        if (!ks.Put(GamePlayTime)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; TCId = v0;
        if (!ks.Get(out sbyte v1)) return false; GameResult = v1;
        if (!ks.Get(out int v2)) return false; GamePlayTime = v2;
        return true;
    }
}

public sealed class KERM_TOGGLE_INVISIBLE_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public KFieldUserInfo FieldUserInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(FieldUserInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(FieldUserInfo)) return false;
        return true;
    }
}

public sealed class KERM_UPDATE_DUNGEON_UNIT_INFO_NOT : IKSerializable
{
    public int Exp { get; set; }
    public int Ed { get; set; }
    public int PvPGameType { get; set; }
    public int DungeonId { get; set; }
    public int UserCount { get; set; }
    public bool DungeonClear { get; set; }
    public Dictionary<int, int> GetItem { get; set; } = new();
    public Dictionary<int, KItemInfo> ResultItem { get; set; } = new();
    public int DropItemUid { get; set; }
    public bool OutRoom { get; set; }
    public int PlayTime { get; set; }
    public int MaxScore { get; set; }
    public sbyte MaxTotalRank { get; set; }
    public bool IsTutorialSuccess { get; set; }
    public bool GetWorldMissionReward { get; set; }
    public List<KHenirRankingInfo> HenirRankingInfo { get; set; } = new();
    public bool StartedByAutoParty { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Exp)) return false;
        if (!ks.Put(Ed)) return false;
        if (!ks.Put(PvPGameType)) return false;
        if (!ks.Put(DungeonId)) return false;
        if (!ks.Put(UserCount)) return false;
        if (!ks.Put(DungeonClear)) return false;
        if (!ks.PutMap(GetItem, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.PutMap(ResultItem, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.Put(DropItemUid)) return false;
        if (!ks.Put(OutRoom)) return false;
        if (!ks.Put(PlayTime)) return false;
        if (!ks.Put(MaxScore)) return false;
        if (!ks.Put(MaxTotalRank)) return false;
        if (!ks.Put(IsTutorialSuccess)) return false;
        if (!ks.Put(GetWorldMissionReward)) return false;
        if (!ks.PutList(HenirRankingInfo)) return false;
        if (!ks.Put(StartedByAutoParty)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Exp = v0;
        if (!ks.Get(out int v1)) return false; Ed = v1;
        if (!ks.Get(out int v2)) return false; PvPGameType = v2;
        if (!ks.Get(out int v3)) return false; DungeonId = v3;
        if (!ks.Get(out int v4)) return false; UserCount = v4;
        if (!ks.Get(out bool v5)) return false; DungeonClear = v5;
        if (!ks.GetMap(GetItem, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false;
        if (!ks.GetMap(ResultItem, static s => (s.Get(out int v), v), static s => { var v = new KItemInfo(); return (s.Get(v), v); })) return false;
        if (!ks.Get(out int v8)) return false; DropItemUid = v8;
        if (!ks.Get(out bool v9)) return false; OutRoom = v9;
        if (!ks.Get(out int v10)) return false; PlayTime = v10;
        if (!ks.Get(out int v11)) return false; MaxScore = v11;
        if (!ks.Get(out sbyte v12)) return false; MaxTotalRank = v12;
        if (!ks.Get(out bool v13)) return false; IsTutorialSuccess = v13;
        if (!ks.Get(out bool v14)) return false; GetWorldMissionReward = v14;
        if (!ks.GetList(HenirRankingInfo)) return false;
        if (!ks.Get(out bool v16)) return false; StartedByAutoParty = v16;
        return true;
    }
}

public sealed class KERM_UPDATE_FRIEND_NOT : IKSerializable
{
    public HashSet<long> FriendUIDList { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutList(FriendUIDList, static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetHashSet(FriendUIDList, static s => (s.Get(out long v), v))) return false;
        return true;
    }
}

public sealed class KERM_UPDATE_PARTY_NOT : IKSerializable
{
    public long PartyUid { get; set; }
    public HashSet<long> PartyUIDList { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PartyUid)) return false;
        if (!ks.PutList(PartyUIDList, static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; PartyUid = v0;
        if (!ks.GetHashSet(PartyUIDList, static s => (s.Get(out long v), v))) return false;
        return true;
    }
}

public sealed class KERM_UPDATE_PVP_UNIT_INFO_NOT : IKSerializable
{
    public int Exp { get; set; }
    public int CurrentRating { get; set; }
    public int CurrentMaxRating { get; set; }
    public int CurrentRPoint { get; set; }
    public int CurrentAPoint { get; set; }
    public bool IsOfficialMatch { get; set; }
    public int Rating { get; set; }
    public int RPoint { get; set; }
    public int APoint { get; set; }
    public sbyte Rank { get; set; }
    public sbyte RankForServer { get; set; }
    public float KFactorIncrement { get; set; }
    public int Win { get; set; }
    public int Lose { get; set; }
    public bool OutRoom { get; set; }
    public int MemberCount { get; set; }
    public int PlayTime { get; set; }
    public Dictionary<int, KItemInfo> Item { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Exp)) return false;
        if (!ks.Put(CurrentRating)) return false;
        if (!ks.Put(CurrentMaxRating)) return false;
        if (!ks.Put(CurrentRPoint)) return false;
        if (!ks.Put(CurrentAPoint)) return false;
        if (!ks.Put(IsOfficialMatch)) return false;
        if (!ks.Put(Rating)) return false;
        if (!ks.Put(RPoint)) return false;
        if (!ks.Put(APoint)) return false;
        if (!ks.Put(Rank)) return false;
        if (!ks.Put(RankForServer)) return false;
        if (!ks.Put(KFactorIncrement)) return false;
        if (!ks.Put(Win)) return false;
        if (!ks.Put(Lose)) return false;
        if (!ks.Put(OutRoom)) return false;
        if (!ks.Put(MemberCount)) return false;
        if (!ks.Put(PlayTime)) return false;
        if (!ks.PutMap(Item, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Exp = v0;
        if (!ks.Get(out int v1)) return false; CurrentRating = v1;
        if (!ks.Get(out int v2)) return false; CurrentMaxRating = v2;
        if (!ks.Get(out int v3)) return false; CurrentRPoint = v3;
        if (!ks.Get(out int v4)) return false; CurrentAPoint = v4;
        if (!ks.Get(out bool v5)) return false; IsOfficialMatch = v5;
        if (!ks.Get(out int v6)) return false; Rating = v6;
        if (!ks.Get(out int v7)) return false; RPoint = v7;
        if (!ks.Get(out int v8)) return false; APoint = v8;
        if (!ks.Get(out sbyte v9)) return false; Rank = v9;
        if (!ks.Get(out sbyte v10)) return false; RankForServer = v10;
        if (!ks.Get(out float v11)) return false; KFactorIncrement = v11;
        if (!ks.Get(out int v12)) return false; Win = v12;
        if (!ks.Get(out int v13)) return false; Lose = v13;
        if (!ks.Get(out bool v14)) return false; OutRoom = v14;
        if (!ks.Get(out int v15)) return false; MemberCount = v15;
        if (!ks.Get(out int v16)) return false; PlayTime = v16;
        if (!ks.GetMap(Item, static s => (s.Get(out int v), v), static s => { var v = new KItemInfo(); return (s.Get(v), v); })) return false;
        return true;
    }
}

public sealed class KERM_UPDATE_QUEST_INFO_NOT : IKSerializable
{
    public List<int> NPC { get; set; } = new();
    public bool IsDungeon { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutList(NPC, static (s, v) => s.Put(v))) return false;
        if (!ks.Put(IsDungeon)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetList(NPC, static s => (s.Get(out int v), v))) return false;
        if (!ks.Get(out bool v1)) return false; IsDungeon = v1;
        return true;
    }
}

public sealed class KERM_UPDATE_VISIT_CASH_SHOP_NOT : IKSerializable
{
    public bool EnterCashShop { get; set; }
    public long UnitUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(EnterCashShop)) return false;
        if (!ks.Put(UnitUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool v0)) return false; EnterCashShop = v0;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        return true;
    }
}

public sealed class KERM_USER_UNIT_DIE_ACK : IKSerializable
{
    public int Ok { get; set; }
    public float ReBirthTime { get; set; }
    public short VP { get; set; }
    public short EXP { get; set; }
    public byte DieReason { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(ReBirthTime)) return false;
        if (!ks.Put(VP)) return false;
        if (!ks.Put(EXP)) return false;
        if (!ks.Put(DieReason)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out float v1)) return false; ReBirthTime = v1;
        if (!ks.Get(out short v2)) return false; VP = v2;
        if (!ks.Get(out short v3)) return false; EXP = v3;
        if (!ks.Get(out byte v4)) return false; DieReason = v4;
        return true;
    }
}

public sealed class KERM_USER_UNIT_DIE_NOT : IKSerializable
{
    public int GameType { get; set; }
    public int KillerNPCUid { get; set; }
    public long KillerUserUnitUid { get; set; }
    public long MaxDamageKillerUserUnitUid { get; set; }
    public long KilledUserUnitUid { get; set; }
    public bool OfficialMatch { get; set; }
    public bool HeroNPC { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(GameType)) return false;
        if (!ks.Put(KillerNPCUid)) return false;
        if (!ks.Put(KillerUserUnitUid)) return false;
        if (!ks.Put(MaxDamageKillerUserUnitUid)) return false;
        if (!ks.Put(KilledUserUnitUid)) return false;
        if (!ks.Put(OfficialMatch)) return false;
        if (!ks.Put(HeroNPC)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; GameType = v0;
        if (!ks.Get(out int v1)) return false; KillerNPCUid = v1;
        if (!ks.Get(out long v2)) return false; KillerUserUnitUid = v2;
        if (!ks.Get(out long v3)) return false; MaxDamageKillerUserUnitUid = v3;
        if (!ks.Get(out long v4)) return false; KilledUserUnitUid = v4;
        if (!ks.Get(out bool v5)) return false; OfficialMatch = v5;
        if (!ks.Get(out bool v6)) return false; HeroNPC = v6;
        return true;
    }
}

public sealed class KE_HENIR_RANKING_LOG_NOT : IKSerializable
{
    public int RankingType { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(RankingType)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; RankingType = v0;
        return true;
    }
}

public sealed class KE_LOCAL_LOG_ABUSER_MORNITORING_NOT : IKSerializable
{
    public List<KAbuserEventLog> AbuserEventLog { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutList(AbuserEventLog)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetList(AbuserEventLog)) return false;
        return true;
    }
}

public sealed class KE_LOCAL_LOG_BILLING_PACKET_NOT : IKSerializable
{
    // enum BILLING_PACKET_LOG_TYPE
    public const int BPLT_SEND = 0;
    public const int BPLT_RECV = 1;

    public sbyte LogType { get; set; }
    public long UserUid { get; set; }
    public uint OrderNo { get; set; }
    public string RegDate { get; set; } = string.Empty;
    public ushort EventId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(LogType)) return false;
        if (!ks.Put(UserUid)) return false;
        if (!ks.Put(OrderNo)) return false;
        if (!ks.PutWideString(RegDate)) return false;
        if (!ks.Put(EventId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte v0)) return false; LogType = v0;
        if (!ks.Get(out long v1)) return false; UserUid = v1;
        if (!ks.Get(out uint v2)) return false; OrderNo = v2;
        if (!ks.GetWideString(out string s3)) return false; RegDate = s3;
        if (!ks.Get(out ushort v4)) return false; EventId = v4;
        return true;
    }
}

public sealed class KE_LOCAL_LOG_DUNGEON_NOT : IKSerializable
{
    // enum DUNGEON_CLEAR_TYPE
    public const int DCT_FAILED = 0;
    public const int DCT_SUCCESS = 1;
    public const int DCT_UNIT_DIE_LEAVE_ROOM = 2;
    public const int DCT_UNIT_ALIVE_LEAVE_ROOM = 3;
    public const int DCT_CLIENT_CRASH_LEAVE_ROOM = 4;
    public const int DCT_CLIENT_HACKING_LEAVE_ROOM = 5;

    public int DungeonId { get; set; }
    public long UnitUid { get; set; }
    public string NickName { get; set; } = string.Empty;
    public int DifficultyLevel { get; set; }
    public int IsChallenge { get; set; }
    public int UnitClass { get; set; }
    public int Clear { get; set; }
    public int StartNumMember { get; set; }
    public int EndNumMember { get; set; }
    public byte Level { get; set; }
    public int PlayTime { get; set; }
    public int Exp { get; set; }
    public int Ed { get; set; }
    public sbyte ComboRank { get; set; }
    public int ComboScore { get; set; }
    public sbyte TechnicalRank { get; set; }
    public int TechnicalScore { get; set; }
    public sbyte TimeRank { get; set; }
    public sbyte DamagedRank { get; set; }
    public int DamageNum { get; set; }
    public sbyte TotalRank { get; set; }
    public int RessurectionStoneCount { get; set; }
    public int PassedStageCount { get; set; }
    public int PassedSubStageCount { get; set; }
    public bool IsWithPet { get; set; }
    public string RegDate { get; set; } = string.Empty;
    public short NpcDieCount { get; set; }
    public long GivenDamage { get; set; }
    public long AttackDamage { get; set; }
    public short UsingSkillKind { get; set; }
    public int TotalEXP { get; set; }
    public int PetId { get; set; }
    public sbyte PetEvoStep { get; set; }
    public int DashCount { get; set; }
    public int BaseHP { get; set; }
    public int EndHP { get; set; }
    public int RecovHP { get; set; }
    public int Continue { get; set; }
    public int StartContinue { get; set; }
    public int ContinueUseStage { get; set; }
    public int ContinueUseSub { get; set; }
    public int Frame { get; set; }
    public string ChannelIP { get; set; } = string.Empty;
    public long UserUid { get; set; }
    public sbyte AutoPartyPlay { get; set; }
    public int AutoPartyWaitTime { get; set; }
    public int LeaveReason { get; set; }
    public sbyte RoomState { get; set; }
    public bool SkillSlotType { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(DungeonId)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutWideString(NickName)) return false;
        if (!ks.Put(DifficultyLevel)) return false;
        if (!ks.Put(IsChallenge)) return false;
        if (!ks.Put(UnitClass)) return false;
        if (!ks.Put(Clear)) return false;
        if (!ks.Put(StartNumMember)) return false;
        if (!ks.Put(EndNumMember)) return false;
        if (!ks.Put(Level)) return false;
        if (!ks.Put(PlayTime)) return false;
        if (!ks.Put(Exp)) return false;
        if (!ks.Put(Ed)) return false;
        if (!ks.Put(ComboRank)) return false;
        if (!ks.Put(ComboScore)) return false;
        if (!ks.Put(TechnicalRank)) return false;
        if (!ks.Put(TechnicalScore)) return false;
        if (!ks.Put(TimeRank)) return false;
        if (!ks.Put(DamagedRank)) return false;
        if (!ks.Put(DamageNum)) return false;
        if (!ks.Put(TotalRank)) return false;
        if (!ks.Put(RessurectionStoneCount)) return false;
        if (!ks.Put(PassedStageCount)) return false;
        if (!ks.Put(PassedSubStageCount)) return false;
        if (!ks.Put(IsWithPet)) return false;
        if (!ks.PutWideString(RegDate)) return false;
        if (!ks.Put(NpcDieCount)) return false;
        if (!ks.Put(GivenDamage)) return false;
        if (!ks.Put(AttackDamage)) return false;
        if (!ks.Put(UsingSkillKind)) return false;
        if (!ks.Put(TotalEXP)) return false;
        if (!ks.Put(PetId)) return false;
        if (!ks.Put(PetEvoStep)) return false;
        if (!ks.Put(DashCount)) return false;
        if (!ks.Put(BaseHP)) return false;
        if (!ks.Put(EndHP)) return false;
        if (!ks.Put(RecovHP)) return false;
        if (!ks.Put(Continue)) return false;
        if (!ks.Put(StartContinue)) return false;
        if (!ks.Put(ContinueUseStage)) return false;
        if (!ks.Put(ContinueUseSub)) return false;
        if (!ks.Put(Frame)) return false;
        if (!ks.PutWideString(ChannelIP)) return false;
        if (!ks.Put(UserUid)) return false;
        if (!ks.Put(AutoPartyPlay)) return false;
        if (!ks.Put(AutoPartyWaitTime)) return false;
        if (!ks.Put(LeaveReason)) return false;
        if (!ks.Put(RoomState)) return false;
        if (!ks.Put(SkillSlotType)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; DungeonId = v0;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        if (!ks.GetWideString(out string s2)) return false; NickName = s2;
        if (!ks.Get(out int v3)) return false; DifficultyLevel = v3;
        if (!ks.Get(out int v4)) return false; IsChallenge = v4;
        if (!ks.Get(out int v5)) return false; UnitClass = v5;
        if (!ks.Get(out int v6)) return false; Clear = v6;
        if (!ks.Get(out int v7)) return false; StartNumMember = v7;
        if (!ks.Get(out int v8)) return false; EndNumMember = v8;
        if (!ks.Get(out byte v9)) return false; Level = v9;
        if (!ks.Get(out int v10)) return false; PlayTime = v10;
        if (!ks.Get(out int v11)) return false; Exp = v11;
        if (!ks.Get(out int v12)) return false; Ed = v12;
        if (!ks.Get(out sbyte v13)) return false; ComboRank = v13;
        if (!ks.Get(out int v14)) return false; ComboScore = v14;
        if (!ks.Get(out sbyte v15)) return false; TechnicalRank = v15;
        if (!ks.Get(out int v16)) return false; TechnicalScore = v16;
        if (!ks.Get(out sbyte v17)) return false; TimeRank = v17;
        if (!ks.Get(out sbyte v18)) return false; DamagedRank = v18;
        if (!ks.Get(out int v19)) return false; DamageNum = v19;
        if (!ks.Get(out sbyte v20)) return false; TotalRank = v20;
        if (!ks.Get(out int v21)) return false; RessurectionStoneCount = v21;
        if (!ks.Get(out int v22)) return false; PassedStageCount = v22;
        if (!ks.Get(out int v23)) return false; PassedSubStageCount = v23;
        if (!ks.Get(out bool v24)) return false; IsWithPet = v24;
        if (!ks.GetWideString(out string s25)) return false; RegDate = s25;
        if (!ks.Get(out short v26)) return false; NpcDieCount = v26;
        if (!ks.Get(out long v27)) return false; GivenDamage = v27;
        if (!ks.Get(out long v28)) return false; AttackDamage = v28;
        if (!ks.Get(out short v29)) return false; UsingSkillKind = v29;
        if (!ks.Get(out int v30)) return false; TotalEXP = v30;
        if (!ks.Get(out int v31)) return false; PetId = v31;
        if (!ks.Get(out sbyte v32)) return false; PetEvoStep = v32;
        if (!ks.Get(out int v33)) return false; DashCount = v33;
        if (!ks.Get(out int v34)) return false; BaseHP = v34;
        if (!ks.Get(out int v35)) return false; EndHP = v35;
        if (!ks.Get(out int v36)) return false; RecovHP = v36;
        if (!ks.Get(out int v37)) return false; Continue = v37;
        if (!ks.Get(out int v38)) return false; StartContinue = v38;
        if (!ks.Get(out int v39)) return false; ContinueUseStage = v39;
        if (!ks.Get(out int v40)) return false; ContinueUseSub = v40;
        if (!ks.Get(out int v41)) return false; Frame = v41;
        if (!ks.GetWideString(out string s42)) return false; ChannelIP = s42;
        if (!ks.Get(out long v43)) return false; UserUid = v43;
        if (!ks.Get(out sbyte v44)) return false; AutoPartyPlay = v44;
        if (!ks.Get(out int v45)) return false; AutoPartyWaitTime = v45;
        if (!ks.Get(out int v46)) return false; LeaveReason = v46;
        if (!ks.Get(out sbyte v47)) return false; RoomState = v47;
        if (!ks.Get(out bool v48)) return false; SkillSlotType = v48;
        return true;
    }
}

public sealed class KE_LOCAL_LOG_DUNGEON_ROOM_NOT : IKSerializable
{
    public int DungeonId { get; set; }
    public int DifficultyLevel { get; set; }
    public int IsChallenge { get; set; }
    public int Win { get; set; }
    public int Lose { get; set; }
    public int StartNumMember { get; set; }
    public int EndNumMember { get; set; }
    public int TotalPlayTime { get; set; }
    public string EndGameTime { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(DungeonId)) return false;
        if (!ks.Put(DifficultyLevel)) return false;
        if (!ks.Put(IsChallenge)) return false;
        if (!ks.Put(Win)) return false;
        if (!ks.Put(Lose)) return false;
        if (!ks.Put(StartNumMember)) return false;
        if (!ks.Put(EndNumMember)) return false;
        if (!ks.Put(TotalPlayTime)) return false;
        if (!ks.PutWideString(EndGameTime)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; DungeonId = v0;
        if (!ks.Get(out int v1)) return false; DifficultyLevel = v1;
        if (!ks.Get(out int v2)) return false; IsChallenge = v2;
        if (!ks.Get(out int v3)) return false; Win = v3;
        if (!ks.Get(out int v4)) return false; Lose = v4;
        if (!ks.Get(out int v5)) return false; StartNumMember = v5;
        if (!ks.Get(out int v6)) return false; EndNumMember = v6;
        if (!ks.Get(out int v7)) return false; TotalPlayTime = v7;
        if (!ks.GetWideString(out string s8)) return false; EndGameTime = s8;
        return true;
    }
}

public sealed class KE_LOCAL_LOG_HENIR_RANKING_NOT : IKSerializable
{
    public int RankingType { get; set; }
    public List<KHenirRankingInfo> RankingInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(RankingType)) return false;
        if (!ks.PutList(RankingInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; RankingType = v0;
        if (!ks.GetList(RankingInfo)) return false;
        return true;
    }
}

public sealed class KE_LOCAL_LOG_PET_SUMMON_NOT : IKSerializable
{
    public string NickName { get; set; } = string.Empty;
    public byte Level { get; set; }
    public long PetUid { get; set; }
    public int PetId { get; set; }
    public sbyte EvolutionStep { get; set; }
    public int Intimacy { get; set; }
    public short Extroversion { get; set; }
    public short Emotion { get; set; }
    public string RegDate { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutWideString(NickName)) return false;
        if (!ks.Put(Level)) return false;
        if (!ks.Put(PetUid)) return false;
        if (!ks.Put(PetId)) return false;
        if (!ks.Put(EvolutionStep)) return false;
        if (!ks.Put(Intimacy)) return false;
        if (!ks.Put(Extroversion)) return false;
        if (!ks.Put(Emotion)) return false;
        if (!ks.PutWideString(RegDate)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string s0)) return false; NickName = s0;
        if (!ks.Get(out byte v1)) return false; Level = v1;
        if (!ks.Get(out long v2)) return false; PetUid = v2;
        if (!ks.Get(out int v3)) return false; PetId = v3;
        if (!ks.Get(out sbyte v4)) return false; EvolutionStep = v4;
        if (!ks.Get(out int v5)) return false; Intimacy = v5;
        if (!ks.Get(out short v6)) return false; Extroversion = v6;
        if (!ks.Get(out short v7)) return false; Emotion = v7;
        if (!ks.GetWideString(out string s8)) return false; RegDate = s8;
        return true;
    }
}

public sealed class KE_LOCAL_LOG_PVP_NOT : IKSerializable
{
    public int PVPChannelClass { get; set; }
    public long OwnerUserUid { get; set; }
    public string Ip { get; set; } = string.Empty;
    public long UnitUid { get; set; }
    public string NickName { get; set; } = string.Empty;
    public int UnitClass { get; set; }
    public long RoomUid { get; set; }
    public int PlayMode { get; set; }
    public bool IsItemMode { get; set; }
    public bool Public { get; set; }
    public int NumMember { get; set; }
    public float PlayTimeLimit { get; set; }
    public string EndGameTime { get; set; } = string.Empty;
    public int PlayTime { get; set; }
    public int VSPoint { get; set; }
    public int Exp { get; set; }
    public int NumMDKill { get; set; }
    public int NumDie { get; set; }
    public int Result { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PVPChannelClass)) return false;
        if (!ks.Put(OwnerUserUid)) return false;
        if (!ks.PutWideString(Ip)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutWideString(NickName)) return false;
        if (!ks.Put(UnitClass)) return false;
        if (!ks.Put(RoomUid)) return false;
        if (!ks.Put(PlayMode)) return false;
        if (!ks.Put(IsItemMode)) return false;
        if (!ks.Put(Public)) return false;
        if (!ks.Put(NumMember)) return false;
        if (!ks.Put(PlayTimeLimit)) return false;
        if (!ks.PutWideString(EndGameTime)) return false;
        if (!ks.Put(PlayTime)) return false;
        if (!ks.Put(VSPoint)) return false;
        if (!ks.Put(Exp)) return false;
        if (!ks.Put(NumMDKill)) return false;
        if (!ks.Put(NumDie)) return false;
        if (!ks.Put(Result)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; PVPChannelClass = v0;
        if (!ks.Get(out long v1)) return false; OwnerUserUid = v1;
        if (!ks.GetWideString(out string s2)) return false; Ip = s2;
        if (!ks.Get(out long v3)) return false; UnitUid = v3;
        if (!ks.GetWideString(out string s4)) return false; NickName = s4;
        if (!ks.Get(out int v5)) return false; UnitClass = v5;
        if (!ks.Get(out long v6)) return false; RoomUid = v6;
        if (!ks.Get(out int v7)) return false; PlayMode = v7;
        if (!ks.Get(out bool v8)) return false; IsItemMode = v8;
        if (!ks.Get(out bool v9)) return false; Public = v9;
        if (!ks.Get(out int v10)) return false; NumMember = v10;
        if (!ks.Get(out float v11)) return false; PlayTimeLimit = v11;
        if (!ks.GetWideString(out string s12)) return false; EndGameTime = s12;
        if (!ks.Get(out int v13)) return false; PlayTime = v13;
        if (!ks.Get(out int v14)) return false; VSPoint = v14;
        if (!ks.Get(out int v15)) return false; Exp = v15;
        if (!ks.Get(out int v16)) return false; NumMDKill = v16;
        if (!ks.Get(out int v17)) return false; NumDie = v17;
        if (!ks.Get(out int v18)) return false; Result = v18;
        return true;
    }
}

public sealed class KE_LOCAL_LOG_PVP_ROOM_NOT : IKSerializable
{
    public int PlayMode { get; set; }
    public bool IsItemMode { get; set; }
    public short WorldId { get; set; }
    public int NumMember { get; set; }
    public int PlayTime { get; set; }
    public string EndGameTime { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PlayMode)) return false;
        if (!ks.Put(IsItemMode)) return false;
        if (!ks.Put(WorldId)) return false;
        if (!ks.Put(NumMember)) return false;
        if (!ks.Put(PlayTime)) return false;
        if (!ks.PutWideString(EndGameTime)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; PlayMode = v0;
        if (!ks.Get(out bool v1)) return false; IsItemMode = v1;
        if (!ks.Get(out short v2)) return false; WorldId = v2;
        if (!ks.Get(out int v3)) return false; NumMember = v3;
        if (!ks.Get(out int v4)) return false; PlayTime = v4;
        if (!ks.GetWideString(out string s5)) return false; EndGameTime = s5;
        return true;
    }
}

public sealed class KE_LOCAL_LOG_SERVER_DISCONNECT_NOT : IKSerializable
{
    // enum SERVER_DISCONNECT_LOG_TYPE
    public const int SDLT_CONNECT = 0;
    public const int SDLT_DISCONNECT = 1;

    public sbyte LogType { get; set; }
    public string SourceName { get; set; } = string.Empty;
    public string DestinationName { get; set; } = string.Empty;
    public string DestinationIP { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string RegDate { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(LogType)) return false;
        if (!ks.PutWideString(SourceName)) return false;
        if (!ks.PutWideString(DestinationName)) return false;
        if (!ks.PutWideString(DestinationIP)) return false;
        if (!ks.PutWideString(Reason)) return false;
        if (!ks.PutWideString(RegDate)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte v0)) return false; LogType = v0;
        if (!ks.GetWideString(out string s1)) return false; SourceName = s1;
        if (!ks.GetWideString(out string s2)) return false; DestinationName = s2;
        if (!ks.GetWideString(out string s3)) return false; DestinationIP = s3;
        if (!ks.GetWideString(out string s4)) return false; Reason = s4;
        if (!ks.GetWideString(out string s5)) return false; RegDate = s5;
        return true;
    }
}

public sealed class KE_LOCAL_LOG_TITLE_NOT : IKSerializable
{
    public int TitleId { get; set; }
    public string CharName { get; set; } = string.Empty;
    public byte Level { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(TitleId)) return false;
        if (!ks.PutWideString(CharName)) return false;
        if (!ks.Put(Level)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; TitleId = v0;
        if (!ks.GetWideString(out string s1)) return false; CharName = s1;
        if (!ks.Get(out byte v2)) return false; Level = v2;
        return true;
    }
}

public sealed class KE_STATISTICS_INFO_NOT : IKSerializable
{
    public List<KStatisticsInfo> StatisticsInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutList(StatisticsInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetList(StatisticsInfo)) return false;
        return true;
    }
}

public sealed class KDBE_ACCEPT_FRIEND_ACK : KEGS_ACCEPT_FRIEND_ACK { }

public sealed class KDBE_ADMIN_CHEAT_GET_ALL_SKILL_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int ActiveSkillPageNumber { get; set; }
    public List<short> UnSealedSkill { get; set; } = new();
    public Dictionary<int, KAdminCheatSkill> SkillInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(ActiveSkillPageNumber)) return false;
        if (!ks.PutList(UnSealedSkill, static (s, v) => s.Put(v))) return false;
        if (!ks.PutMap(SkillInfo, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out int v1)) return false; ActiveSkillPageNumber = v1;
        if (!ks.GetList(UnSealedSkill, static s => (s.Get(out short v), v))) return false;
        if (!ks.GetMap(SkillInfo, static s => (s.Get(out int v), v), static s => { var v = new KAdminCheatSkill(); return (s.Get(v), v); })) return false;
        return true;
    }
}

public sealed class KDBE_BACKUP_RANKING_INFO_NOT : KELG_HENIR_RANKING_REFRESH_NOT { }

public sealed class KDBE_BLOCK_FRIEND_ACK : KEGS_BLOCK_FRIEND_ACK { }

public sealed class KDBE_CHANGE_GUILD_NAME_ACK : KChangeGuildNameACK { }

public sealed class KDBE_CHANGE_GUILD_NAME_CHECK_REQ : KChangeGuildNameCheckREQ { }

public sealed class KDBE_CHANGE_GUILD_NAME_REQ : KChangeGuildNameREQ { }

public sealed class KDBE_CHANGE_SECOND_SECURITY_PW_ACK : KDBE_CREATE_SECOND_SECURITY_ACK { }

public sealed class KDBE_CREATE_GUILD_ACK : KCreateGuildACK { }

public sealed class KDBE_CREATE_GUILD_REQ : KCreateGuildREQ { }

public sealed class KDBE_DELETE_FRIEND_ACK : KEGS_DELETE_FRIEND_ACK { }

public sealed class KDBE_DELETE_FRIEND_GROUP_ACK : IKSerializable
{
    public int Ok { get; set; }
    public sbyte GroupId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(GroupId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out sbyte v1)) return false; GroupId = v1;
        return true;
    }
}

public sealed class KDBE_DELETE_NICK_NAME_ACK : KPacketOK { }

public sealed class KDBE_DELETE_SECOND_SECURITY_ACK : KPacketOK { }

public sealed class KDBE_DELETE_SECOND_SECURITY_REQ : KDBE_CREATE_SECOND_SECURITY_REQ { }

public sealed class KDBE_DEL_POST_BLACK_LIST_ACK : KDBE_NEW_POST_BLACK_LIST_ACK { }

public sealed class KDBE_DEL_POST_BLACK_LIST_REQ : KDBE_NEW_POST_BLACK_LIST_REQ { }

public sealed class KDBE_DENY_FRIEND_ACK : KEGS_DENY_FRIEND_ACK { }

public sealed class KDBE_EVENT_MAKING_SUCCESS_ACK : KDBE_COUPLE_MAKING_SUCCESS_ACK { }

public sealed class KDBE_EVENT_MAKING_SUCCESS_REQ : KDBE_COUPLE_MAKING_SUCCESS_REQ { }

public sealed class KDBE_EXPAND_BANK_INVENTORY_ACK : KDBE_EXPAND_INVENTORY_SLOT_ACK { }

public sealed class KDBE_EXPAND_BANK_INVENTORY_REQ : KDBE_EXPAND_INVENTORY_SLOT_REQ { }

public sealed class KDBE_GET_DEFENCE_CRYSTAL_ACK : KDBE_INCREASE_DEFENCE_CRYSTAL_REQ { }

public sealed class KDBE_GET_DEFENCE_CRYSTAL_REQ : KDBE_INCREASE_DEFENCE_CRYSTAL_REQ { }

public sealed class KDBE_GET_GLOBAL_MISSION_INFO_ACK : KDBE_GET_GLOBAL_MISSION_INFO_REQ { }

public sealed class KDBE_GET_GUILD_INFO_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public int GuildUid { get; set; }
    public KGuildMemberInfo LoginGuildMember { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(GuildUid)) return false;
        if (!ks.Put(LoginGuildMember)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out int v1)) return false; GuildUid = v1;
        if (!ks.Get(LoginGuildMember)) return false;
        return true;
    }
}

public sealed class KDBE_GET_HENIR_RANKING_INFO_ACK : KELG_HENIR_RANKING_REFRESH_NOT { }

public sealed class KDBE_GET_PURE_SECOND_SECURITY_INFO_ACK : KDBE_GET_SECOND_SECURITY_INFO_ACK { }

public sealed class KDBE_GET_PURE_SECOND_SECURITY_INFO_REQ : KDBE_GET_SECOND_SECURITY_INFO_REQ { }

public sealed class KDBE_IDENTIFY_ITEM_ACK : KDBE_OPEN_RANDOM_ITEM_ACK { }

public sealed class KDBE_IDENTIFY_ITEM_REQ : KDBE_OPEN_RANDOM_ITEM_REQ { }

public sealed class KDBE_INSERT_BUY_PERSONAL_SHOP_ITEM_ACK : KDBE_INSERT_TRADE_ITEM_ACK { }

public sealed class KDBE_INSERT_BUY_PERSONAL_SHOP_ITEM_REQ : KDBE_INSERT_TRADE_ITEM_REQ { }

public sealed class KDBE_MAKE_FRIEND_GROUP_ACK : IKSerializable
{
    public int Ok { get; set; }
    public sbyte GroupId { get; set; }
    public string GroupName { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(GroupId)) return false;
        if (!ks.PutWideString(GroupName)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out sbyte v1)) return false; GroupId = v1;
        if (!ks.GetWideString(out string s2)) return false; GroupName = s2;
        return true;
    }
}

public sealed class KDBE_MODIFY_REG_GUILD_AD_ACK : KDBE_REGISTRATION_GUILD_AD_ACK { }

public sealed class KDBE_MOVE_FRIEND_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long UnitUid { get; set; }
    public sbyte TargetGroupId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(TargetGroupId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        if (!ks.Get(out sbyte v2)) return false; TargetGroupId = v2;
        return true;
    }
}

public sealed class KDBE_RENAME_FRIEND_GROUP_ACK : IKSerializable
{
    public int Ok { get; set; }
    public sbyte GroupId { get; set; }
    public string GroupName { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(GroupId)) return false;
        if (!ks.PutWideString(GroupName)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out sbyte v1)) return false; GroupId = v1;
        if (!ks.GetWideString(out string s2)) return false; GroupName = s2;
        return true;
    }
}

public sealed class KDBE_START_CHECK_POWER_ACK : KPacketOK { }

public sealed class KDBE_UNBLOCK_FRIEND_ACK : KEGS_UNBLOCK_FRIEND_ACK { }

public sealed class KDBE_UNSEAL_ITEM_ACK : KDBE_SEAL_ITEM_ACK { }

public sealed class KDBE_UNSEAL_ITEM_REQ : KDBE_SEAL_ITEM_REQ { }

public sealed class KDBE_UPDATE_CHECK_POWER_ACK : KPacketOK { }

public sealed class KEGASH_ACCOUNTING_ACK : KGashResponseData { }

public sealed class KEGASH_AUTHENTICATION_ACK : KGashResponseData { }

public sealed class KEGASH_AUTHORIZATION_ACK : KGashResponseData { }

public sealed class KEGASH_DECREASE_GASHPOINT_ACK : KGashResponseData { }

public sealed class KEGASH_GET_GASHPOINT_ACK : KGashResponseData { }

public sealed class KEGB_ADMIN_BOSS_FIELD_GATE_OPEN_ACK : KPacketOK { }

public sealed class KEGB_ADMIN_BOSS_FIELD_GATE_OPEN_REQ : IKSerializable
{
    public int BattleFieldId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(BattleFieldId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; BattleFieldId = v0;
        return true;
    }
}

public sealed class KEGB_ADMIN_GET_TOTAL_DANGEROUS_VALUE_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int BattleFieldId { get; set; }
    public byte TotalDangerousValue { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(BattleFieldId)) return false;
        if (!ks.Put(TotalDangerousValue)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out int v1)) return false; BattleFieldId = v1;
        if (!ks.Get(out byte v2)) return false; TotalDangerousValue = v2;
        return true;
    }
}

public sealed class KEGB_ADMIN_GET_TOTAL_DANGEROUS_VALUE_REQ : IKSerializable
{
    public int BattleFieldId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(BattleFieldId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; BattleFieldId = v0;
        return true;
    }
}

public sealed class KEGB_ADMIN_SET_TOTAL_DANGEROUS_VALUE_ACK : KPacketOK { }

public sealed class KEGB_ADMIN_SET_TOTAL_DANGEROUS_VALUE_REQ : IKSerializable
{
    public int BattleFieldId { get; set; }
    public int TotalDangerousValue { get; set; }
    public bool Set { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(BattleFieldId)) return false;
        if (!ks.Put(TotalDangerousValue)) return false;
        if (!ks.Put(Set)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; BattleFieldId = v0;
        if (!ks.Get(out int v1)) return false; TotalDangerousValue = v1;
        if (!ks.Get(out bool v2)) return false; Set = v2;
        return true;
    }
}

public sealed class KEGB_EXCHANGE_LIMIT_INFO_ACK : IKSerializable
{
    public int ExchangeLimitUid { get; set; }
    public Dictionary<int, int> ExchangeLimitInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ExchangeLimitUid)) return false;
        if (!ks.PutMap(ExchangeLimitInfo, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; ExchangeLimitUid = v0;
        if (!ks.GetMap(ExchangeLimitInfo, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false;
        return true;
    }
}

public sealed class KEGB_OPEN_PVP_ROOM_FOR_MATCH_ACK : KPacketOK { }

public sealed class KEGB_REQUEST_AUTO_PARTY_MAKING_ACK : KPacketOK { }

public sealed class KEGB_TIME_ENCHANT_EVENT_CUBE_NOT : IKSerializable
{
    public int AfterWorkStorageKey { get; set; }
    public sbyte TimeEnchantEventLevel { get; set; }
    public bool NotifyGetItem { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(AfterWorkStorageKey)) return false;
        if (!ks.Put(TimeEnchantEventLevel)) return false;
        if (!ks.Put(NotifyGetItem)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; AfterWorkStorageKey = v0;
        if (!ks.Get(out sbyte v1)) return false; TimeEnchantEventLevel = v1;
        if (!ks.Get(out bool v2)) return false; NotifyGetItem = v2;
        return true;
    }
}

public sealed class KEGB_TIME_ENCHANT_EVENT_DROP_NOT : IKSerializable
{
    public int AfterWorkStorageKey { get; set; }
    public sbyte TimeEnchantEventLevel { get; set; }
    public bool NotifyGetItem { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(AfterWorkStorageKey)) return false;
        if (!ks.Put(TimeEnchantEventLevel)) return false;
        if (!ks.Put(NotifyGetItem)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; AfterWorkStorageKey = v0;
        if (!ks.Get(out sbyte v1)) return false; TimeEnchantEventLevel = v1;
        if (!ks.Get(out bool v2)) return false; NotifyGetItem = v2;
        return true;
    }
}

public sealed class KEGB_TIME_ENCHANT_EVENT_INFO_ACK : IKSerializable
{
    public int TimeEnchantEventUid { get; set; }
    public int NumOfTimeEnchantEvent { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(TimeEnchantEventUid)) return false;
        if (!ks.Put(NumOfTimeEnchantEvent)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; TimeEnchantEventUid = v0;
        if (!ks.Get(out int v1)) return false; NumOfTimeEnchantEvent = v1;
        return true;
    }
}

public sealed class KEGB_UPDATE_MISSION_INFO_NOT : IKSerializable
{
    // enum FLAG_TYPE
    public const int FT_NONE = 0;
    public const int FT_INIT = 1;
    public const int FT_UPDATE = 2;
    public const int FT_BEGIN = 3;
    public const int FT_SUCCESS = 4;
    public const int FT_FAIL = 5;

    public bool Active { get; set; }
    public int Flag { get; set; }
    public int WorldCrystalCount { get; set; }
    public int WorldMaxCrystalCount { get; set; }
    public int MissionTime { get; set; }
    public int DungeonPlayTime { get; set; }
    public Dictionary<int, List<KDefenseOpenTime>> DefenseOpenTime { get; set; } = new();
    public ushort WorldBuffDurationTime { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Active)) return false;
        if (!ks.Put(Flag)) return false;
        if (!ks.Put(WorldCrystalCount)) return false;
        if (!ks.Put(WorldMaxCrystalCount)) return false;
        if (!ks.Put(MissionTime)) return false;
        if (!ks.Put(DungeonPlayTime)) return false;
        if (!ks.PutMap(DefenseOpenTime, static (s, k) => s.Put(k), static (s, v) => s.PutList(v))) return false;
        if (!ks.Put(WorldBuffDurationTime)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool v0)) return false; Active = v0;
        if (!ks.Get(out int v1)) return false; Flag = v1;
        if (!ks.Get(out int v2)) return false; WorldCrystalCount = v2;
        if (!ks.Get(out int v3)) return false; WorldMaxCrystalCount = v3;
        if (!ks.Get(out int v4)) return false; MissionTime = v4;
        if (!ks.Get(out int v5)) return false; DungeonPlayTime = v5;
        if (!ks.GetMap(DefenseOpenTime, static s => (s.Get(out int v), v), static s => { var l = new List<KDefenseOpenTime>(); return (s.GetList(l), l); })) return false;
        if (!ks.Get(out ushort v7)) return false; WorldBuffDurationTime = v7;
        return true;
    }
}

public sealed class KELG_CHANGE_GUILD_NAME_ACK : KChangeGuildNameACK { }

public sealed class KELG_CHANGE_GUILD_NAME_CHECK_ACK : KPacketOK { }

public sealed class KELG_CHANGE_GUILD_NAME_CHECK_REQ : KChangeGuildNameCheckREQ { }

public sealed class KELG_CHECK_CHANGE_GUILD_MEMBER_GRADE_ACK : KDBE_CHANGE_GUILD_MEMBER_GRADE_ACK { }

public sealed class KELG_CHECK_CHANGE_GUILD_MEMBER_GRADE_REQ : KDBE_CHANGE_GUILD_MEMBER_GRADE_REQ { }

public sealed class KELG_DIVORCE_NOT : KELG_BREAK_UP_NOT { }

public sealed class KELG_EVENT_PROPOSE_CHECK_CONNECT_USER_ACK : KELG_COUPLE_PROPOSE_CHECK_CONNECT_USER_ACK { }

public sealed class KELG_EVENT_PROPOSE_CHECK_CONNECT_USER_REQ : IKSerializable
{
    public long OtherUserUid { get; set; }
    public long OtherUnitUid { get; set; }
    public string RequestUnitName { get; set; } = string.Empty;
    public long RequestUserUid { get; set; }
    public long RequestUnitUid { get; set; }
    public string AcceptUnitName { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(OtherUserUid)) return false;
        if (!ks.Put(OtherUnitUid)) return false;
        if (!ks.PutWideString(RequestUnitName)) return false;
        if (!ks.Put(RequestUserUid)) return false;
        if (!ks.Put(RequestUnitUid)) return false;
        if (!ks.PutWideString(AcceptUnitName)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; OtherUserUid = v0;
        if (!ks.Get(out long v1)) return false; OtherUnitUid = v1;
        if (!ks.GetWideString(out string s2)) return false; RequestUnitName = s2;
        if (!ks.Get(out long v3)) return false; RequestUserUid = v3;
        if (!ks.Get(out long v4)) return false; RequestUnitUid = v4;
        if (!ks.GetWideString(out string s5)) return false; AcceptUnitName = s5;
        return true;
    }
}

public sealed class KELG_EVENT_PROPOSE_NOT : KELG_COUPLE_PROPOSE_NOT { }

public sealed class KELG_MODIFY_REG_GUILD_AD_ACK : IKSerializable
{
    public int Ok { get; set; }
    public short Period { get; set; }
    public string AdMessage { get; set; } = string.Empty;
    public int Cost { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(Period)) return false;
        if (!ks.PutWideString(AdMessage)) return false;
        if (!ks.Put(Cost)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out short v1)) return false; Period = v1;
        if (!ks.GetWideString(out string s2)) return false; AdMessage = s2;
        if (!ks.Get(out int v3)) return false; Cost = v3;
        return true;
    }
}

public sealed class KELG_RESET_GUILD_SKILL_REQ : IKSerializable
{
    public int GuildUid { get; set; }
    public long UnitUid { get; set; }
    public int GuildSkillId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(GuildUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(GuildSkillId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; GuildUid = v0;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        if (!ks.Get(out int v2)) return false; GuildSkillId = v2;
        return true;
    }
}

public sealed class KELG_VERIFY_GIANT_MATRIX_CARD_ACK : KPacketOK { }

public sealed class KELG_VERIFY_GIANT_MATRIX_CARD_NOT : KPacketOK { }

public sealed class KELOG_CHANGE_NICK_NAME_NOT : KDBE_DELETE_NICK_NAME_REQ { }

public sealed class KELOG_DELETE_NICK_NAME_NOT : KDBE_DELETE_NICK_NAME_REQ { }

public sealed class KENX_USER_LOGIN_ACK : KENX_COMMON_ACK_NOT { }

public sealed class KENX_USER_LOGIN_REQ : KENX_COMMON_REQ { }

public sealed class KENX_USER_LOGOUT_REQ : KENX_COMMON_REQ { }

public sealed class KEPM_CHECK_FOR_AUTO_PARTY_MAKING_REQ : KDungeonGameInfo { }

public sealed class KEPM_PARTY_CHANGE_CHECK_MIN_LEVEL_ACK : KPacketOK { }

public sealed class KERM_BOSS_FIELD_INTRUDE_RESTRICTION_ACK : KPacketOK { }

public sealed class KERM_CHANGE_SKILL_INFO_IN_ROOM_NOT : IKSerializable
{
    // enum BONUS_TYPE
    public const int BT_PREMIUM_EXP_RATE = 0;
    public const int BT_SOCKET_OPT_EXP_RATE = 1;
    public const int BT_CHANNEL_BONUS_EXP_RATE = 2;
    public const int BT_SOCKET_OPT_ED_RATE = 3;
    public const int BT_GUILD_BONUS_EXP_RATE = 4;
    public const int BT_GEV_EXP = 5;
    public const int BT_GEV_VP = 6;
    public const int BT_GEV_ED = 7;
    public const int BT_GEV_PARTY_EXP = 8;
    public const int BT_GEV_PARTY_VP = 9;
    public const int BT_GEV_PARTY_ED = 10;
    public const int BT_GEV_WITH_CHAR_EXP = 11;
    public const int BT_GEV_RANKING_POINT = 12;
    public const int BT_GEV_PARTY_RANKING_POINT = 13;

    public long GsUid { get; set; }
    public long OwnerUserUid { get; set; }
    public short ServerGroupId { get; set; }
    public sbyte AuthLevel { get; set; }
    public bool Male { get; set; }
    public byte Age { get; set; }
    public long UnitUid { get; set; }
    public uint KNMSerialNum { get; set; }
    public sbyte UnitClass { get; set; }
    public string NickName { get; set; } = string.Empty;
    public int NumResurrectionStone { get; set; }
    public string Ip { get; set; } = string.Empty;
    public ushort Port { get; set; }
    public string InternalIP { get; set; } = string.Empty;
    public ushort InternalPort { get; set; }
    public long PartyUid { get; set; }
    public List<KGamePlayStatus> GamePlayStatus { get; set; } = new();
    public List<KBuffInfo> BuffInfo { get; set; } = new();
    public byte Level { get; set; }
    public KStat GameStat { get; set; } = new();
    public Dictionary<int, KInventoryItemInfo> EquippedItem { get; set; } = new();
    public Dictionary<int, KInventoryItemInfo> SpecialItem { get; set; } = new();
    public KUnitSkillData UnitSkillData { get; set; } = new();
    public bool IsPvpNpc { get; set; }
    public int OfficialMatchCnt { get; set; }
    public int Rating { get; set; }
    public int MaxRating { get; set; }
    public bool IsWinBeforeMatch { get; set; }
    public sbyte Rank { get; set; }
    public sbyte RankForServer { get; set; }
    public float KFactor { get; set; }
    public bool IsRedistributionUser { get; set; }
    public bool IsGameBang { get; set; }
    public int PcBangType { get; set; }
    public bool IsObserver { get; set; }
    public Dictionary<sbyte, float> BonusRate { get; set; } = new();
    public bool IsRingofpvprebirth { get; set; }
    public bool IsGuestUser { get; set; }
    public Dictionary<int, KSubQuestInfo> OngoingQuest { get; set; } = new();
    public int TitleId { get; set; }
    public int GuildUid { get; set; }
    public string GuildName { get; set; } = string.Empty;
    public byte MemberShipGrade { get; set; }
    public List<KPetInfo> Pet { get; set; } = new();
    public HashSet<int> QuestInfo { get; set; } = new();
    public bool UseItem { get; set; }
    public HashSet<int> GoingQuestInfo { get; set; } = new();
    public bool ComeBackUser { get; set; }
    public bool HenirReward { get; set; }
    public HashSet<int> UseSkillBuffInPlay { get; set; } = new();
    public bool EnterCashShop { get; set; }
    public long RidingPetUid { get; set; }
    public ushort RidingPetId { get; set; }
    public sbyte WeddingStatus { get; set; }
    public long LoverUnitUid { get; set; }
    public int EventQuestClearCount { get; set; }
    public int ExchangeCount { get; set; }
    public bool Couple { get; set; }
    public long RelationTargetUserUid { get; set; }
    public string RelationTargetUserNickname { get; set; } = string.Empty;
    public uint AccountPVPLoseCount { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(GsUid)) return false;
        if (!ks.Put(OwnerUserUid)) return false;
        if (!ks.Put(ServerGroupId)) return false;
        if (!ks.Put(AuthLevel)) return false;
        if (!ks.Put(Male)) return false;
        if (!ks.Put(Age)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(KNMSerialNum)) return false;
        if (!ks.Put(UnitClass)) return false;
        if (!ks.PutWideString(NickName)) return false;
        if (!ks.Put(NumResurrectionStone)) return false;
        if (!ks.PutWideString(Ip)) return false;
        if (!ks.Put(Port)) return false;
        if (!ks.PutWideString(InternalIP)) return false;
        if (!ks.Put(InternalPort)) return false;
        if (!ks.Put(PartyUid)) return false;
        if (!ks.PutList(GamePlayStatus)) return false;
        if (!ks.PutList(BuffInfo)) return false;
        if (!ks.Put(Level)) return false;
        if (!ks.Put(GameStat)) return false;
        if (!ks.PutMap(EquippedItem, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.PutMap(SpecialItem, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.Put(UnitSkillData)) return false;
        if (!ks.Put(IsPvpNpc)) return false;
        if (!ks.Put(OfficialMatchCnt)) return false;
        if (!ks.Put(Rating)) return false;
        if (!ks.Put(MaxRating)) return false;
        if (!ks.Put(IsWinBeforeMatch)) return false;
        if (!ks.Put(Rank)) return false;
        if (!ks.Put(RankForServer)) return false;
        if (!ks.Put(KFactor)) return false;
        if (!ks.Put(IsRedistributionUser)) return false;
        if (!ks.Put(IsGameBang)) return false;
        if (!ks.Put(PcBangType)) return false;
        if (!ks.Put(IsObserver)) return false;
        if (!ks.PutMap(BonusRate, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.Put(IsRingofpvprebirth)) return false;
        if (!ks.Put(IsGuestUser)) return false;
        if (!ks.PutMap(OngoingQuest, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.Put(TitleId)) return false;
        if (!ks.PutWideString(GuildName)) return false;
        if (!ks.Put(GuildUid)) return false;
        if (!ks.Put(MemberShipGrade)) return false;
        if (!ks.PutList(Pet)) return false;
        if (!ks.PutList(QuestInfo, static (s, v) => s.Put(v))) return false;
        if (!ks.Put(UseItem)) return false;
        if (!ks.PutList(GoingQuestInfo, static (s, v) => s.Put(v))) return false;
        if (!ks.Put(ComeBackUser)) return false;
        if (!ks.Put(HenirReward)) return false;
        if (!ks.PutList(UseSkillBuffInPlay, static (s, v) => s.Put(v))) return false;
        if (!ks.Put(EnterCashShop)) return false;
        if (!ks.Put(RidingPetUid)) return false;
        if (!ks.Put(RidingPetId)) return false;
        if (!ks.Put(WeddingStatus)) return false;
        if (!ks.Put(LoverUnitUid)) return false;
        if (!ks.Put(EventQuestClearCount)) return false;
        if (!ks.Put(ExchangeCount)) return false;
        if ((region.Region == RegionCode.CN) && !ks.Put(Couple)) return false;
        if ((region.Region == RegionCode.CN) && !ks.Put(RelationTargetUserUid)) return false;
        if ((region.Region == RegionCode.CN) && !ks.PutWideString(RelationTargetUserNickname)) return false;
        if ((region.Region == RegionCode.JP) && !ks.Put(AccountPVPLoseCount)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; GsUid = v0;
        if (!ks.Get(out long v1)) return false; OwnerUserUid = v1;
        if (!ks.Get(out short v2)) return false; ServerGroupId = v2;
        if (!ks.Get(out sbyte v3)) return false; AuthLevel = v3;
        if (!ks.Get(out bool v4)) return false; Male = v4;
        if (!ks.Get(out byte v5)) return false; Age = v5;
        if (!ks.Get(out long v6)) return false; UnitUid = v6;
        if (!ks.Get(out uint v7)) return false; KNMSerialNum = v7;
        if (!ks.Get(out sbyte v8)) return false; UnitClass = v8;
        if (!ks.GetWideString(out string s9)) return false; NickName = s9;
        if (!ks.Get(out int v10)) return false; NumResurrectionStone = v10;
        if (!ks.GetWideString(out string s11)) return false; Ip = s11;
        if (!ks.Get(out ushort v12)) return false; Port = v12;
        if (!ks.GetWideString(out string s13)) return false; InternalIP = s13;
        if (!ks.Get(out ushort v14)) return false; InternalPort = v14;
        if (!ks.Get(out long v15)) return false; PartyUid = v15;
        if (!ks.GetList(GamePlayStatus)) return false;
        if (!ks.GetList(BuffInfo)) return false;
        if (!ks.Get(out byte v18)) return false; Level = v18;
        if (!ks.Get(GameStat)) return false;
        if (!ks.GetMap(EquippedItem, static s => (s.Get(out int v), v), static s => { var v = new KInventoryItemInfo(); return (s.Get(v), v); })) return false;
        if (!ks.GetMap(SpecialItem, static s => (s.Get(out int v), v), static s => { var v = new KInventoryItemInfo(); return (s.Get(v), v); })) return false;
        if (!ks.Get(UnitSkillData)) return false;
        if (!ks.Get(out bool v23)) return false; IsPvpNpc = v23;
        if (!ks.Get(out int v24)) return false; OfficialMatchCnt = v24;
        if (!ks.Get(out int v25)) return false; Rating = v25;
        if (!ks.Get(out int v26)) return false; MaxRating = v26;
        if (!ks.Get(out bool v27)) return false; IsWinBeforeMatch = v27;
        if (!ks.Get(out sbyte v28)) return false; Rank = v28;
        if (!ks.Get(out sbyte v29)) return false; RankForServer = v29;
        if (!ks.Get(out float v30)) return false; KFactor = v30;
        if (!ks.Get(out bool v31)) return false; IsRedistributionUser = v31;
        if (!ks.Get(out bool v32)) return false; IsGameBang = v32;
        if (!ks.Get(out int v33)) return false; PcBangType = v33;
        if (!ks.Get(out bool v34)) return false; IsObserver = v34;
        if (!ks.GetMap(BonusRate, static s => (s.Get(out sbyte v), v), static s => (s.Get(out float v), v))) return false;
        if (!ks.Get(out bool v36)) return false; IsRingofpvprebirth = v36;
        if (!ks.Get(out bool v37)) return false; IsGuestUser = v37;
        if (!ks.GetMap(OngoingQuest, static s => (s.Get(out int v), v), static s => { var v = new KSubQuestInfo(); return (s.Get(v), v); })) return false;
        if (!ks.Get(out int v39)) return false; TitleId = v39;
        if (!ks.GetWideString(out string s40)) return false; GuildName = s40;
        if (!ks.Get(out int v41)) return false; GuildUid = v41;
        if (!ks.Get(out byte v42)) return false; MemberShipGrade = v42;
        if (!ks.GetList(Pet)) return false;
        if (!ks.GetHashSet(QuestInfo, static s => (s.Get(out int v), v))) return false;
        if (!ks.Get(out bool v45)) return false; UseItem = v45;
        if (!ks.GetHashSet(GoingQuestInfo, static s => (s.Get(out int v), v))) return false;
        if (!ks.Get(out bool v47)) return false; ComeBackUser = v47;
        if (!ks.Get(out bool v48)) return false; HenirReward = v48;
        if (!ks.GetHashSet(UseSkillBuffInPlay, static s => (s.Get(out int v), v))) return false;
        if (!ks.Get(out bool v50)) return false; EnterCashShop = v50;
        if (!ks.Get(out long v51)) return false; RidingPetUid = v51;
        if (!ks.Get(out ushort v52)) return false; RidingPetId = v52;
        if (!ks.Get(out sbyte v53)) return false; WeddingStatus = v53;
        if (!ks.Get(out long v54)) return false; LoverUnitUid = v54;
        if (!ks.Get(out int v55)) return false; EventQuestClearCount = v55;
        if (!ks.Get(out int v56)) return false; ExchangeCount = v56;
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out bool v57)) return false; Couple = v57; }
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out long v58)) return false; RelationTargetUserUid = v58; }
        if ((region.Region == RegionCode.CN)) { if (!ks.GetWideString(out string s59)) return false; RelationTargetUserNickname = s59; }
        if ((region.Region == RegionCode.JP)) { if (!ks.Get(out uint v60)) return false; AccountPVPLoseCount = v60; }
        return true;
    }
}

public sealed class KERM_CHECK_OPEN_PERSONAL_SHOP_ACK : KPacketOK { }

public sealed class KERM_CHECK_OPEN_PSHOP_AGENCY_ACK : KPacketOK { }

public sealed class KERM_CHECK_START_WEDDING_ACK : KPacketOK { }

public sealed class KERM_EVENT_PROPOSE_RESULT_FAIL_NOT : KERM_COUPLE_PROPOSE_RESULT_FAIL_NOT { }

public sealed class KERM_EVENT_PROPOSE_RESULT_SUCCESS_NOT : KERM_COUPLE_PROPOSE_RESULT_SUCCESS_NOT { }

public sealed class KERM_GAME_START_ACK : KPacketOK { }

public sealed class KERM_JOIN_WEDDING_HALL_ACK : KERM_OPEN_WEDDING_HALL_ACK { }

public sealed class KERM_LEAVE_TC_ROOM_REQ : IKSerializable
{
    public int Reason { get; set; }
    public int ItemId { get; set; }
    public bool SendAck { get; set; }
    public KBattleFieldJoinInfo BattleFieldJoinInfo { get; set; } = new();
    public KDungeonPlayResultInfo ResultInfo { get; set; } = new();
    public KGamePlayNetworkInfoPackage GamePlayNetworkInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Reason)) return false;
        if (!ks.Put(ItemId)) return false;
        if (!ks.Put(SendAck)) return false;
        if (!ks.Put(BattleFieldJoinInfo)) return false;
        if (!ks.Put(ResultInfo)) return false;
        if (!ks.Put(GamePlayNetworkInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Reason = v0;
        if (!ks.Get(out int v1)) return false; ItemId = v1;
        if (!ks.Get(out bool v2)) return false; SendAck = v2;
        if (!ks.Get(BattleFieldJoinInfo)) return false;
        if (!ks.Get(ResultInfo)) return false;
        if (!ks.Get(GamePlayNetworkInfo)) return false;
        return true;
    }
}

public sealed class KERM_PARTY_BAN_USER_ACK : KPacketOK { }

public sealed class KERM_PARTY_OPEN_DUNGEON_ROOM_ACK : KPacketOK { }

public sealed class KERM_SINGLE_OPEN_DUNGEON_ROOM_ACK : KPacketOK { }

public sealed class KERM_STATE_CHANGE_GAME_INTRUDE_ACK : KPacketOK { }

public sealed class KERM_SUCCESS_TRADE_ACK : KSuccessTradeData { }

public sealed class KERM_SUCCESS_TRADE_REQ : KSuccessTradeData { }

public sealed class KERM_UPDATE_TOTAL_DANGEROUS_VALUE_NOT : IKSerializable
{
    public int BattleFieldId { get; set; }
    public byte IncreasedValue { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(BattleFieldId)) return false;
        if (!ks.Put(IncreasedValue)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; BattleFieldId = v0;
        if (!ks.Get(out byte v1)) return false; IncreasedValue = v1;
        return true;
    }
}

public sealed class KFieldCreateInfo : KSquareCreateInfo { }

public sealed class KEPUBLISHER_CHARGE_POINT_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int ChargedCash { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.Put(ChargedCash);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out int v1)) return false; ChargedCash = v1;
        return true;
    }
}

public sealed class KNAVER_SOAP_GET_TOKEN_REQ : IKSerializable
{
    public long NaverSn { get; set; }
    public int GameCode { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(NaverSn) && ks.Put(GameCode);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; NaverSn = v0;
        if (!ks.Get(out int v1)) return false; GameCode = v1;
        return true;
    }
}

public sealed class KNAVER_SOAP_GET_TOKEN_ACK : IKSerializable
{
    public int Ok { get; set; }
    public string AccessToken { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.PutNarrowString(AccessToken);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.GetNarrowString(out string s1)) return false; AccessToken = s1;
        return true;
    }
}

public sealed class KDCHATLOG_RECORD_CHAT_NOT : IKSerializable
{
    public long AccountUid { get; set; }
    public long UnitUid { get; set; }
    public int ServerGroupId { get; set; }
    public string ChatText { get; set; } = string.Empty;
    public int ChatType { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
        => ks.Put(AccountUid)
        && ks.Put(UnitUid)
        && ks.Put(ServerGroupId)
        && ks.PutWideString(ChatText)
        && ks.Put(ChatType);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; AccountUid = v0;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        if (!ks.Get(out int v2)) return false; ServerGroupId = v2;
        if (!ks.GetWideString(out string s3)) return false; ChatText = s3;
        if (!ks.Get(out int v4)) return false; ChatType = v4;
        return true;
    }
}

public sealed class KJackPotEventInitInfo : IKSerializable
{
    public string CouponNumber { get; set; } = string.Empty;
    public string BeginDate { get; set; } = string.Empty;
    public string EndDate { get; set; } = string.Empty;
    public long UserUid { get; set; }
    public long UnitUid { get; set; }
    public int State { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
        => ks.PutWideString(CouponNumber)
        && ks.PutWideString(BeginDate)
        && ks.PutWideString(EndDate)
        && ks.Put(UserUid)
        && ks.Put(UnitUid)
        && ks.Put(State);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string s0)) return false; CouponNumber = s0;
        if (!ks.GetWideString(out string s1)) return false; BeginDate = s1;
        if (!ks.GetWideString(out string s2)) return false; EndDate = s2;
        if (!ks.Get(out long v3)) return false; UserUid = v3;
        if (!ks.Get(out long v4)) return false; UnitUid = v4;
        if (!ks.Get(out int v5)) return false; State = v5;
        return true;
    }
}

public sealed class KJackPotEventWinInfo : IKSerializable
{
    public string CouponNumber { get; set; } = string.Empty;
    public long UserUid { get; set; }
    public long UnitUid { get; set; }
    public int State { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
        => ks.PutWideString(CouponNumber)
        && ks.Put(UserUid)
        && ks.Put(UnitUid)
        && ks.Put(State);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string s0)) return false; CouponNumber = s0;
        if (!ks.Get(out long v1)) return false; UserUid = v1;
        if (!ks.Get(out long v2)) return false; UnitUid = v2;
        if (!ks.Get(out int v3)) return false; State = v3;
        return true;
    }
}

public sealed class KDBE_JACKPOT_EVENT_INIT_ACK : IKSerializable
{
    public List<KJackPotEventInitInfo> JackpotEventInitInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
        => ks.PutList(JackpotEventInitInfo);

    public bool Get(KSerializer ks, RegionProfile region)
        => ks.GetList(JackpotEventInitInfo);
}

public sealed class KDBE_JACKPOT_EVENT_UPDATE_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long UnitUid { get; set; }
    public string CouponNumber { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
        => ks.Put(Ok) && ks.Put(UnitUid) && ks.PutWideString(CouponNumber);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        if (!ks.GetWideString(out string s2)) return false; CouponNumber = s2;
        return true;
    }
}
