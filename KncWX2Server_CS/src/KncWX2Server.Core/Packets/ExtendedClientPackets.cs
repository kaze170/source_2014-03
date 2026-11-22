using KncWX2Server.Core.Region;
using KncWX2Server.Core.Serialization;

namespace KncWX2Server.Core.Packets;

public class KEGS_ACCEPT_FRIEND_ACK : IKSerializable
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

public sealed class KEGS_AUTO_FEED_NOT : IKSerializable
{
    public int FeedItemId { get; set; }
    public List<KInventoryItemInfo> PetInventorySlotInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(FeedItemId)) return false;
        if (!ks.PutList(PetInventorySlotInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; FeedItemId = v0;
        if (!ks.GetList(PetInventorySlotInfo)) return false;
        return true;
    }
}

public sealed class KEGS_BAD_ATTITUDE_USER_NOT : IKSerializable
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

public sealed class KEGS_BATTLE_FIELD_BOSS_GATE_CLOSE_NOT : IKSerializable
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

public sealed class KEGS_BATTLE_FIELD_BOSS_GATE_OPEN_NOT : IKSerializable
{
    public int BattleFieldId { get; set; }
    public int BossFieldId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(BattleFieldId)) return false;
        if (!ks.Put(BossFieldId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; BattleFieldId = v0;
        if (!ks.Get(out int v1)) return false; BossFieldId = v1;
        return true;
    }
}

public sealed class KEGS_BATTLE_FIELD_JOIN_USER_LOAD_COMPLETE_NOT : IKSerializable
{
    public long LoadedUnitUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(LoadedUnitUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; LoadedUnitUid = v0;
        return true;
    }
}

public sealed class KEGS_BATTLE_FIELD_NPC_LOAD_NOT : IKSerializable
{
    public List<KNPCUnitReq> NPCData { get; set; } = new();
    public Dictionary<int, KAttribEnchantNpcInfo> AttribNpcInfo { get; set; } = new();
    public int BattleFieldId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutList(NPCData)) return false;
        if (!ks.PutMap(AttribNpcInfo, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.Put(BattleFieldId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetList(NPCData)) return false;
        if (!ks.GetMap(AttribNpcInfo, static s => (s.Get(out int v), v), static s => { var v = new KAttribEnchantNpcInfo(); return (s.Get(v), v); })) return false;
        if (!ks.Get(out int v2)) return false; BattleFieldId = v2;
        return true;
    }
}

public class KEGS_BLOCK_FRIEND_ACK : IKSerializable
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

public sealed class KEGS_BREAK_PERSONAL_SHOP_NOT : IKSerializable
{
    public int Reason { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Reason)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Reason = v0;
        return true;
    }
}

public sealed class KEGS_BREAK_PERSONAL_SHOP_REQ : IKSerializable
{
    public int Reason { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Reason)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Reason = v0;
        return true;
    }
}

public sealed class KEGS_CASH_DIRECT_CHARGE_CN_NOT : IKSerializable
{
    public int Ok { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.CN) && !ks.Put(Ok)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out int v0)) return false; Ok = v0; }
        return true;
    }
}

public sealed class KEGS_CASH_SHOP_OPEN_NOT : IKSerializable
{
    public bool CashShopOpen { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(CashShopOpen)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool v0)) return false; CashShopOpen = v0;
        return true;
    }
}

public sealed class KEGS_CHANGED_PET_SATIETY_INTIMACY_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public long PetUid { get; set; }
    public short CurrentSatiety { get; set; }
    public int CurrentIntimacy { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(PetUid)) return false;
        if (!ks.Put(CurrentSatiety)) return false;
        if (!ks.Put(CurrentIntimacy)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out long v1)) return false; PetUid = v1;
        if (!ks.Get(out short v2)) return false; CurrentSatiety = v2;
        if (!ks.Get(out int v3)) return false; CurrentIntimacy = v3;
        return true;
    }
}

public class KEGS_CHANGE_EQUIPPED_ITEM_IN_ROOM_NOT : IKSerializable
{
    // enum CHANGE_EQUIPPED_PACKET_TYPE
    public const int CEPT_NONE = 0;
    public const int CEPT_CHANGE_EQUIP = 1;
    public const int CEPT_CHANGE_TITLE = 2;
    public const int CEPT_CHANGE_PET = 3;

    public sbyte ChangeEquippedPacketType { get; set; }
    public long UnitUid { get; set; }
    public List<KInventoryItemInfo> InventorySlotInfo { get; set; } = new();
    public KStat GameStat { get; set; } = new();
    public int EquippedTitleId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ChangeEquippedPacketType)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutList(InventorySlotInfo)) return false;
        if (!ks.Put(GameStat)) return false;
        if (!ks.Put(EquippedTitleId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte v0)) return false; ChangeEquippedPacketType = v0;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        if (!ks.GetList(InventorySlotInfo)) return false;
        if (!ks.Get(GameStat)) return false;
        if (!ks.Get(out int v4)) return false; EquippedTitleId = v4;
        return true;
    }
}

public sealed class KEGS_CHANGE_EQUIPPED_ITEM_IN_SQUARE_NOT : IKSerializable
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

public sealed class KEGS_CHANGE_GAME_STAT_INFO_IN_ROOM_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public KStat GameStat { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(GameStat)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(GameStat)) return false;
        return true;
    }
}

public sealed class KEGS_CHANGE_JOB_CASH_ITEM_NOT : IKSerializable
{
    public KUnitInfo UnitInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(UnitInfo)) return false;
        return true;
    }
}

public sealed class KEGS_CHANGE_MAP_ACK : IKSerializable
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

public sealed class KEGS_CHANGE_MOTION_ACK : IKSerializable
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

public sealed class KEGS_CHANGE_MY_UNIT_INFO_NOT : IKSerializable
{
    public sbyte UnitClass { get; set; }
    public int NewDefaultSkill_1 { get; set; }
    public int NewDefaultSkill_2 { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitClass)) return false;
        if (!ks.Put(NewDefaultSkill_1)) return false;
        if (!ks.Put(NewDefaultSkill_2)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte v0)) return false; UnitClass = v0;
        if (!ks.Get(out int v1)) return false; NewDefaultSkill_1 = v1;
        if (!ks.Get(out int v2)) return false; NewDefaultSkill_2 = v2;
        return true;
    }
}

public sealed class KEGS_CHANGE_PARTY_FEVER_STATE_NOT : IKSerializable
{
    public bool IsStart { get; set; }
    public int PartyFever { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(IsStart)) return false;
        if (!ks.Put(PartyFever)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool v0)) return false; IsStart = v0;
        if (!ks.Get(out int v1)) return false; PartyFever = v1;
        return true;
    }
}

public sealed class KEGS_CHANGE_PET_ID_NOT : IKSerializable
{
    public int Ok { get; set; }
    public long PetUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(PetUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out long v1)) return false; PetUid = v1;
        return true;
    }
}

public sealed class KEGS_CHANGE_PSHOP_AGENCY_EXPIRATION_NOT : IKSerializable
{
    public sbyte ShopType { get; set; }
    public string AgencyExpirationDate { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ShopType)) return false;
        if (!ks.PutWideString(AgencyExpirationDate)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte v0)) return false; ShopType = v0;
        if (!ks.GetWideString(out string s1)) return false; AgencyExpirationDate = s1;
        return true;
    }
}

public sealed class KEGS_CHANGE_PVP_ITEM_NOT : IKSerializable
{
    public int DropItemUid { get; set; }
    public int ItemId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(DropItemUid)) return false;
        if (!ks.Put(ItemId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; DropItemUid = v0;
        if (!ks.Get(out int v1)) return false; ItemId = v1;
        return true;
    }
}

public sealed class KEGS_CHANGE_RANDOM_QUEST_NOT : IKSerializable
{
    public Dictionary<int, int> RandomQuestList { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutMap(RandomQuestList, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetMap(RandomQuestList, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false;
        return true;
    }
}

public sealed class KEGS_CHANGE_READY_ACK : IKSerializable
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

public sealed class KEGS_CHANGE_SKILL_INFO_IN_ROOM_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public byte MemberShipGrade { get; set; }
    public KStat GameStat { get; set; } = new();
    public KUnitSkillData UnitSkillData { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(MemberShipGrade)) return false;
        if (!ks.Put(GameStat)) return false;
        if (!ks.Put(UnitSkillData)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out byte v1)) return false; MemberShipGrade = v1;
        if (!ks.Get(GameStat)) return false;
        if (!ks.Get(UnitSkillData)) return false;
        return true;
    }
}

public sealed class KEGS_CHANGE_SKILL_SLOT_NOT : IKSerializable
{
    public int SlotId { get; set; }
    public int SkillId { get; set; }
    public int SlotID2 { get; set; }
    public int SkillID2 { get; set; }
    public long UnitUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(SlotId)) return false;
        if (!ks.Put(SkillId)) return false;
        if (!ks.Put(SlotID2)) return false;
        if (!ks.Put(SkillID2)) return false;
        if (!ks.Put(UnitUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; SlotId = v0;
        if (!ks.Get(out int v1)) return false; SkillId = v1;
        if (!ks.Get(out int v2)) return false; SlotID2 = v2;
        if (!ks.Get(out int v3)) return false; SkillID2 = v3;
        if (!ks.Get(out long v4)) return false; UnitUid = v4;
        return true;
    }
}

public sealed class KEGS_CHANGE_TEAM_ACK : IKSerializable
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

public sealed class KEGS_CHAR_LEVEL_UP_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public byte Level { get; set; }
    public KStat BaseStat { get; set; } = new();
    public KStat GameStat { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(Level)) return false;
        if (!ks.Put(BaseStat)) return false;
        if (!ks.Put(GameStat)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out byte v1)) return false; Level = v1;
        if (!ks.Get(BaseStat)) return false;
        if (!ks.Get(GameStat)) return false;
        return true;
    }
}

public sealed class KEGS_CHAT_OPTION_INFO_NOT : IKSerializable
{
    public int Ok { get; set; }
    public Dictionary<byte, bool> ChatOptionSetInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.PutMap(ChatOptionSetInfo, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.GetMap(ChatOptionSetInfo, static s => (s.Get(out byte v), v), static s => (s.Get(out bool v), v))) return false;
        return true;
    }
}

public sealed class KEGS_CHECK_INVALID_ROOM_NOT : IKSerializable
{
    public int Ok { get; set; }
    public int MapId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(MapId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out int v1)) return false; MapId = v1;
        return true;
    }
}

public sealed class KEGS_CHECK_PING_SCORE_CHANGE_HOST_NOT : IKSerializable
{
    public long OldHostUid { get; set; }
    public long NewHostUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(OldHostUid)) return false;
        if (!ks.Put(NewHostUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; OldHostUid = v0;
        if (!ks.Get(out long v1)) return false; NewHostUid = v1;
        return true;
    }
}

public sealed class KEGS_CHECK_SUM_ACK : IKSerializable
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

public sealed class KEGS_CHECK_SUM_REQ : IKSerializable
{
    public Dictionary<string, string> CheckSum { get; set; } = new();
    public uint ProtectTableCSVal { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutMap(CheckSum, static (s, k) => s.PutWideString(k), static (s, v) => s.PutWideString(v))) return false;
        if (!ks.Put(ProtectTableCSVal)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetMap(CheckSum, static s => (s.GetWideString(out string v), v), static s => (s.GetWideString(out string v), v))) return false;
        if (!ks.Get(out uint v1)) return false; ProtectTableCSVal = v1;
        return true;
    }
}

public sealed class KEGS_CLOSE_PARTY_NOT : IKSerializable
{
    public long PartyUid { get; set; }
    public int Reason { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PartyUid)) return false;
        if (!ks.Put(Reason)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; PartyUid = v0;
        if (!ks.Get(out int v1)) return false; Reason = v1;
        return true;
    }
}

public sealed class KEGS_COME_BACK_BUFF_NOT : IKSerializable
{
    public bool IsFinish { get; set; }
    public long ComeBackBuffEnd { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(IsFinish)) return false;
        if (!ks.Put(ComeBackBuffEnd)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool v0)) return false; IsFinish = v0;
        if (!ks.Get(out long v1)) return false; ComeBackBuffEnd = v1;
        return true;
    }
}

public sealed class KEGS_CREATE_ATTRIB_NPC_NOT : IKSerializable
{
    public List<KAttribNpcUnitInfo> AttribNPCUnit { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutList(AttribNPCUnit)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetList(AttribNPCUnit)) return false;
        return true;
    }
}

public sealed class KEGS_CURRENT_KILL_SCORE_NOT : IKSerializable
{
    public Dictionary<long, (int, int)> KillScore { get; set; } = new();
    public Dictionary<long, int> MDKillScore { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutMap(KillScore, static (s, k) => s.Put(k), static (s, v) => s.Put(v.Item1) && s.Put(v.Item2))) return false;
        if (!ks.PutMap(MDKillScore, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetMap(KillScore, static s => (s.Get(out long v), v), static s => { bool ok1 = s.Get(out int a); bool ok2 = s.Get(out int b); return (ok1 && ok2, (a, b)); })) return false;
        if (!ks.GetMap(MDKillScore, static s => (s.Get(out long v), v), static s => (s.Get(out int v), v))) return false;
        return true;
    }
}

public sealed class KEGS_CURRENT_TEAM_KILL_SCORE_NOT : IKSerializable
{
    public sbyte RedTeamKill { get; set; }
    public sbyte BlueTeamKill { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(RedTeamKill)) return false;
        if (!ks.Put(BlueTeamKill)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte v0)) return false; RedTeamKill = v0;
        if (!ks.Get(out sbyte v1)) return false; BlueTeamKill = v1;
        return true;
    }
}

public sealed class KEGS_DECISION_ENTER_DEFENCE_DUNGEON_NOT : IKSerializable
{
    public KVector3 DieBossPos { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(DieBossPos)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(DieBossPos)) return false;
        return true;
    }
}

public sealed class KEGS_DECREASE_ENDURANCE_NOT : IKSerializable
{
    public List<KDecreaseEnduranceInfo> EnduranceUpdate { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutList(EnduranceUpdate)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetList(EnduranceUpdate)) return false;
        return true;
    }
}

public sealed class KEGS_DECREASE_SPIRIT_NOT : IKSerializable
{
    public int Spirit { get; set; }
    public int HonorPoint { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Spirit)) return false;
        if (!ks.Put(HonorPoint)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Spirit = v0;
        if (!ks.Get(out int v1)) return false; HonorPoint = v1;
        return true;
    }
}

public sealed class KEGS_DEFENCE_DUNGEON_GAME_START_NOT : IKSerializable
{
    public short WorldId { get; set; }
    public KRoomInfo RoomInfo { get; set; } = new();
    public List<KRoomSlotInfo> Slot { get; set; } = new();
    public List<KRoomSlotInfo> ObserverSlot { get; set; } = new();
    public string CNIP { get; set; } = string.Empty;
    public List<KInventoryItemInfo> InventorySlotInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(WorldId)) return false;
        if (!ks.Put(RoomInfo)) return false;
        if (!ks.PutList(Slot)) return false;
        if (!ks.PutList(ObserverSlot)) return false;
        if (!ks.PutWideString(CNIP)) return false;
        if (!ks.PutList(InventorySlotInfo)) return false;
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
        return true;
    }
}

public class KEGS_DELETE_FRIEND_ACK : IKSerializable
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

public class KEGS_DENY_FRIEND_ACK : IKSerializable
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

public sealed class KEGS_DROP_ITEM_NOT : IKSerializable
{
    public List<KDropItemData> DropItemDataList { get; set; } = new();
    public KVector3 CreatePos { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutList(DropItemDataList)) return false;
        if (!ks.Put(CreatePos)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetList(DropItemDataList)) return false;
        if (!ks.Get(CreatePos)) return false;
        return true;
    }
}

public sealed class KEGS_DUNGEON_EFFECT_REMAINING_TIME_NOT : IKSerializable
{
    public float RemainingTime { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(RemainingTime)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out float v0)) return false; RemainingTime = v0;
        return true;
    }
}

public sealed class KEGS_DUNGEON_EFFECT_TIME_OUT_NOT : IKSerializable
{
    // enum DUNGEON_DFFECT_TYPE
    public const int DDT_NONE = 0;
    public const int DDT_POISON = 1;

    public sbyte TypeVal { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(TypeVal)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte v0)) return false; TypeVal = v0;
        return true;
    }
}

public sealed class KEGS_DUNGEON_SECRET_STAGE_ENTER_RESULT_NOT : IKSerializable
{
    public bool ProceedToEnterSecretStage { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ProceedToEnterSecretStage)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool v0)) return false; ProceedToEnterSecretStage = v0;
        return true;
    }
}

public sealed class KEGS_DUNGEON_SUB_STAGE_GO_NEXT_ALL_NOT : IKSerializable
{
    public sbyte BeforeStageId { get; set; }
    public sbyte NextStageId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(BeforeStageId)) return false;
        if (!ks.Put(NextStageId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte v0)) return false; BeforeStageId = v0;
        if (!ks.Get(out sbyte v1)) return false; NextStageId = v1;
        return true;
    }
}

public sealed class KEGS_DUNGEON_SUB_STAGE_GO_NEXT_REQ : IKSerializable
{
    public sbyte BeforeStageId { get; set; }
    public sbyte NextStageId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(BeforeStageId)) return false;
        if (!ks.Put(NextStageId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte v0)) return false; BeforeStageId = v0;
        if (!ks.Get(out sbyte v1)) return false; NextStageId = v1;
        return true;
    }
}

public sealed class KEGS_DUNGEON_SUB_STAGE_LOAD_COMPLETE_REQ : IKSerializable
{
    public sbyte SubStageId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(SubStageId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte v0)) return false; SubStageId = v0;
        return true;
    }
}

public sealed class KEGS_DUNGEON_SUB_STAGE_OPEN_NOT : IKSerializable
{
    public sbyte SubStageId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(SubStageId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte v0)) return false; SubStageId = v0;
        return true;
    }
}

public sealed class KEGS_DUNGEON_SUB_STAGE_OPEN_REQ : IKSerializable
{
    public sbyte SubStageId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(SubStageId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte v0)) return false; SubStageId = v0;
        return true;
    }
}

public sealed class KEGS_DUNGEON_SUB_STAGE_START_NOT : IKSerializable
{
    public sbyte SubStageId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(SubStageId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte v0)) return false; SubStageId = v0;
        return true;
    }
}

public sealed class KEGS_EAT_SPECIAL_FEED_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public long PetUid { get; set; }
    public short Percent { get; set; }
    public int CurrentIntimacy { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(PetUid)) return false;
        if (!ks.Put(Percent)) return false;
        if (!ks.Put(CurrentIntimacy)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out long v1)) return false; PetUid = v1;
        if (!ks.Get(out short v2)) return false; Percent = v2;
        if (!ks.Get(out int v3)) return false; CurrentIntimacy = v3;
        return true;
    }
}

public sealed class KEGS_EPAY_INFO_NOT : IKSerializable
{
    public string LoginId { get; set; } = string.Empty;
    public ushort Zone { get; set; }
    public ushort Game { get; set; }
    public string CharacterName { get; set; } = string.Empty;
    public long UnitUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.CN) && !ks.PutWideString(LoginId)) return false;
        if ((region.Region == RegionCode.CN) && !ks.Put(Zone)) return false;
        if ((region.Region == RegionCode.CN) && !ks.Put(Game)) return false;
        if ((region.Region == RegionCode.CN) && !ks.PutWideString(CharacterName)) return false;
        if ((region.Region == RegionCode.CN) && !ks.Put(UnitUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.CN)) { if (!ks.GetWideString(out string s0)) return false; LoginId = s0; }
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out ushort v1)) return false; Zone = v1; }
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out ushort v2)) return false; Game = v2; }
        if ((region.Region == RegionCode.CN)) { if (!ks.GetWideString(out string s3)) return false; CharacterName = s3; }
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out long v4)) return false; UnitUid = v4; }
        return true;
    }
}

public sealed class KEGS_EVENT_PROPOSE_RESULT_ACCEPTOR_NOT : IKSerializable
{
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.CN) && !ks.PutList(UpdatedInventorySlot)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.CN)) { if (!ks.GetList(UpdatedInventorySlot)) return false; }
        return true;
    }
}

public sealed class KEGS_EVENT_QUEST_CHECK_FOR_ADMIN_NOT : IKSerializable
{
    public List<int> WarningQuestId { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutList(WarningQuestId, static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetList(WarningQuestId, static s => (s.Get(out int v), v))) return false;
        return true;
    }
}

public sealed class KEGS_EVENT_QUEST_INFO_NOT : IKSerializable
{
    // enum EVENT_QUEST_INFO_TYPE
    public const int EQIT_BEGIN = 0;
    public const int EQIT_END = 1;

    public sbyte TypeVal { get; set; }
    public List<int> QuestId { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(TypeVal)) return false;
        if (!ks.PutList(QuestId, static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte v0)) return false; TypeVal = v0;
        if (!ks.GetList(QuestId, static s => (s.Get(out int v), v))) return false;
        return true;
    }
}

public sealed class KEGS_EXPAND_BANK_SLOT_NOT : IKSerializable
{
    public Dictionary<int, int> ExpandedCategorySlot { get; set; } = new();
    public int Ok { get; set; }
    public int Ed { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutMap(ExpandedCategorySlot, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(Ed)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetMap(ExpandedCategorySlot, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false;
        if (!ks.Get(out int v1)) return false; Ok = v1;
        if (!ks.Get(out int v2)) return false; Ed = v2;
        return true;
    }
}

public sealed class KEGS_EXPAND_CHAR_SLOT_NOT : IKSerializable
{
    public int CharSlotSize { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(CharSlotSize)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; CharSlotSize = v0;
        return true;
    }
}

public sealed class KEGS_EXPAND_SKILL_NOTE_PAGE_NUM_NOT : IKSerializable
{
    public sbyte ExpandedPageNum { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ExpandedPageNum)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte v0)) return false; ExpandedPageNum = v0;
        return true;
    }
}

public sealed class KEGS_EXPAND_SKILL_SLOT_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public sbyte SkillSlotBExpirationState { get; set; }
    public string SkillSlotBEndDate { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(SkillSlotBExpirationState)) return false;
        if (!ks.PutWideString(SkillSlotBEndDate)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out sbyte v1)) return false; SkillSlotBExpirationState = v1;
        if (!ks.GetWideString(out string s2)) return false; SkillSlotBEndDate = s2;
        return true;
    }
}

public sealed class KEGS_EXPIRE_GUILD_CASH_SKILL_POINT_NOT : IKSerializable
{
    public int GuildSPoint { get; set; }
    public int GuildCSPoint { get; set; }
    public List<KGuildSkillData> GuildSkillData { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(GuildSPoint)) return false;
        if (!ks.Put(GuildCSPoint)) return false;
        if (!ks.PutList(GuildSkillData)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; GuildSPoint = v0;
        if (!ks.Get(out int v1)) return false; GuildCSPoint = v1;
        if (!ks.GetList(GuildSkillData)) return false;
        return true;
    }
}

public sealed class KEGS_FIRST_FEED_PETS_NOT : IKSerializable
{
    public bool AutoFeed { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(AutoFeed)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool v0)) return false; AutoFeed = v0;
        return true;
    }
}

public sealed class KEGS_GAME_P2P_CONNECT_RESULT_ACK : IKSerializable
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

public sealed class KEGS_GAME_P2P_CONNECT_RESULT_REQ : IKSerializable
{
    public bool ConnectSuccess { get; set; }
    public long UnitUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ConnectSuccess)) return false;
        if (!ks.Put(UnitUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool v0)) return false; ConnectSuccess = v0;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        return true;
    }
}

public sealed class KEGS_GAME_START_TIME_COUNT_NOT : IKSerializable
{
    public long RoomUid { get; set; }
    public sbyte SecondCount { get; set; }
    public bool IsDungeonRoom { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(RoomUid)) return false;
        if (!ks.Put(SecondCount)) return false;
        if (!ks.Put(IsDungeonRoom)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; RoomUid = v0;
        if (!ks.Get(out sbyte v1)) return false; SecondCount = v1;
        if (!ks.Get(out bool v2)) return false; IsDungeonRoom = v2;
        return true;
    }
}

public sealed class KEGS_GET_GUILD_SKILL_NOT : IKSerializable
{
    public int GuildSkillId { get; set; }
    public int GuildSkillLv { get; set; }
    public int GuildSkillCSPoint { get; set; }
    public int GuildSPoint { get; set; }
    public int GuildCSPoint { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(GuildSkillId)) return false;
        if (!ks.Put(GuildSkillLv)) return false;
        if (!ks.Put(GuildSkillCSPoint)) return false;
        if (!ks.Put(GuildSPoint)) return false;
        if (!ks.Put(GuildCSPoint)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; GuildSkillId = v0;
        if (!ks.Get(out int v1)) return false; GuildSkillLv = v1;
        if (!ks.Get(out int v2)) return false; GuildSkillCSPoint = v2;
        if (!ks.Get(out int v3)) return false; GuildSPoint = v3;
        if (!ks.Get(out int v4)) return false; GuildCSPoint = v4;
        return true;
    }
}

public sealed class KEGS_GET_ITEM_TEMP_INVENTORY_NOT : IKSerializable
{
    public KTempInventoryItemInfo TempInvenItem { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(TempInvenItem)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(TempInvenItem)) return false;
        return true;
    }
}

public sealed class KEGS_GLOBAL_MISSION_UPDATE_NOT : IKSerializable
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

public sealed class KEGS_GUILD_MEMBER_INFO_NOT : IKSerializable
{
    public KGuildMemberInfo MemberInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(MemberInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(MemberInfo)) return false;
        return true;
    }
}

public sealed class KEGS_GUILD_MESSAGE_NOT : IKSerializable
{
    // enum MESSAGE_TYPE
    public const int MT_JOIN_MEMBER = 0;
    public const int MT_CHANGE_MEMBER_GRADE = 1;
    public const int MT_KICK_MEMBER = 2;
    public const int MT_CONNECT_MEMBER = 3;
    public const int MT_DISCONNECT_MEMBER = 4;
    public const int MT_CHANGE_MEMBER_MESSAGE = 5;
    public const int MT_UPDATE_MEMBER_INFO = 6;
    public const int MT_KICK_BY_MASTER = 7;
    public const int MT_GUILD_MESSAGE = 8;
    public const int MT_GUILD_EXP = 9;
    public const int MT_GUILD_LEVEL_UP = 10;
    public const int MT_EXPAND_MAX_MEMBER = 11;
    public const int MT_INSERT_GUILD_CSPOINT = 12;
    public const int MT_GUILD_AD_PERIOD_INFO = 13;
    public const int MT_REG_GUILD_AD = 14;
    public const int MT_CHANGE_GUILD_NAME = 15;

    public sbyte MessageType { get; set; }
    public long UnitUid { get; set; }
    public string NickName { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public int MessageData { get; set; }
    public int MessageData2 { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(MessageType)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutWideString(NickName)) return false;
        if (!ks.PutWideString(Message)) return false;
        if (!ks.Put(MessageData)) return false;
        if (!ks.Put(MessageData2)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte v0)) return false; MessageType = v0;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        if (!ks.GetWideString(out string s2)) return false; NickName = s2;
        if (!ks.GetWideString(out string s3)) return false; Message = s3;
        if (!ks.Get(out int v4)) return false; MessageData = v4;
        if (!ks.Get(out int v5)) return false; MessageData2 = v5;
        return true;
    }
}

public sealed class KEGS_HENIR_REWARD_COUNT_NOT : IKSerializable
{
    public bool UnLimited { get; set; }
    public int Normal { get; set; }
    public int Premium { get; set; }
    public int EventVal { get; set; }
    public int PremiumMAX { get; set; }
    public int EventMAX { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnLimited)) return false;
        if (!ks.Put(Normal)) return false;
        if (!ks.Put(Premium)) return false;
        if (!ks.Put(EventVal)) return false;
        if (!ks.Put(PremiumMAX)) return false;
        if (!ks.Put(EventMAX)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool v0)) return false; UnLimited = v0;
        if (!ks.Get(out int v1)) return false; Normal = v1;
        if (!ks.Get(out int v2)) return false; Premium = v2;
        if (!ks.Get(out int v3)) return false; EventVal = v3;
        if (!ks.Get(out int v4)) return false; PremiumMAX = v4;
        if (!ks.Get(out int v5)) return false; EventMAX = v5;
        return true;
    }
}

public sealed class KEGS_IDENTITY_CONFIRM_POPUP_MESSAGE_NOT : IKSerializable
{
    public bool CheckAdult { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.CN) && !ks.Put(CheckAdult)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out bool v0)) return false; CheckAdult = v0; }
        return true;
    }
}

public sealed class KEGS_INIT_GUILD_SKILL_NOT : IKSerializable
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

public sealed class KEGS_INVITE_PARTY_DENY_NOT : IKSerializable
{
    public int DenyReasonNetError { get; set; }
    public long UnitUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(DenyReasonNetError)) return false;
        if (!ks.Put(UnitUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; DenyReasonNetError = v0;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        return true;
    }
}

public sealed class KEGS_ITEM_EXPIRATION_NOT : IKSerializable
{
    public List<long> ItemUid { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutList(ItemUid, static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetList(ItemUid, static s => (s.Get(out long v), v))) return false;
        return true;
    }
}

public sealed class KEGS_JOIN_PARTY_INVITE_DENY_NOT : IKSerializable
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

public sealed class KEGS_JOIN_ROOM_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KRoomInfo RoomInfo { get; set; } = new();
    public List<KRoomSlotInfo> Slot { get; set; } = new();
    public List<KRoomSlotInfo> ObserverSlot { get; set; } = new();
    public string CNIP { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(RoomInfo)) return false;
        if (!ks.PutList(Slot)) return false;
        if (!ks.PutList(ObserverSlot)) return false;
        if (!ks.PutWideString(CNIP)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(RoomInfo)) return false;
        if (!ks.GetList(Slot)) return false;
        if (!ks.GetList(ObserverSlot)) return false;
        if (!ks.GetWideString(out string s4)) return false; CNIP = s4;
        return true;
    }
}

public sealed class KEGS_JOIN_TRADE_NOT : IKSerializable
{
    public KTradeUserInfo TradeUserInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(TradeUserInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(TradeUserInfo)) return false;
        return true;
    }
}

public sealed class KEGS_KEYBOARD_MAPPING_INFO_NOT : IKSerializable
{
    public int Ok { get; set; }
    public KKeyboardMappingInfo KeyboardMappingInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(KeyboardMappingInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(KeyboardMappingInfo)) return false;
        return true;
    }
}

public sealed class KEGS_LEAVE_BATTLE_FIELD_NOT : IKSerializable
{
    public sbyte RoomState { get; set; }
    public long UnitUid { get; set; }
    public long PartyUid { get; set; }
    public int Reason { get; set; }
    public List<KRoomSlotInfo> Slot { get; set; } = new();
    public List<KRoomSlotInfo> ObserverSlot { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(RoomState)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(PartyUid)) return false;
        if (!ks.Put(Reason)) return false;
        if (!ks.PutList(Slot)) return false;
        if (!ks.PutList(ObserverSlot)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte v0)) return false; RoomState = v0;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        if (!ks.Get(out long v2)) return false; PartyUid = v2;
        if (!ks.Get(out int v3)) return false; Reason = v3;
        if (!ks.GetList(Slot)) return false;
        if (!ks.GetList(ObserverSlot)) return false;
        return true;
    }
}

public class KEGS_LEAVE_ROOM_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int Reason { get; set; }
    public int ItemId { get; set; }
    public bool NotLeaveParty { get; set; }
    public KBattleFieldJoinInfo BattleFieldJoinInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(Reason)) return false;
        if (!ks.Put(ItemId)) return false;
        if (!ks.Put(NotLeaveParty)) return false;
        if (!ks.Put(BattleFieldJoinInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out int v1)) return false; Reason = v1;
        if (!ks.Get(out int v2)) return false; ItemId = v2;
        if (!ks.Get(out bool v3)) return false; NotLeaveParty = v3;
        if (!ks.Get(BattleFieldJoinInfo)) return false;
        return true;
    }
}

public sealed class KEGS_LEAVE_ROOM_NOT : IKSerializable
{
    public sbyte RoomState { get; set; }
    public long UnitUid { get; set; }
    public int Reason { get; set; }
    public List<KRoomSlotInfo> Slot { get; set; } = new();
    public List<KRoomSlotInfo> ObserverSlot { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(RoomState)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(Reason)) return false;
        if (!ks.PutList(Slot)) return false;
        if (!ks.PutList(ObserverSlot)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte v0)) return false; RoomState = v0;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        if (!ks.Get(out int v2)) return false; Reason = v2;
        if (!ks.GetList(Slot)) return false;
        if (!ks.GetList(ObserverSlot)) return false;
        return true;
    }
}

public sealed class KEGS_LEAVE_WEDDING_HALL_NOT : IKSerializable
{
    public int Reason { get; set; }
    public sbyte RoomState { get; set; }
    public long UnitUid { get; set; }
    public List<KRoomSlotInfo> Slot { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Reason)) return false;
        if (!ks.Put(RoomState)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutList(Slot)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Reason = v0;
        if (!ks.Get(out sbyte v1)) return false; RoomState = v1;
        if (!ks.Get(out long v2)) return false; UnitUid = v2;
        if (!ks.GetList(Slot)) return false;
        return true;
    }
}

public sealed class KEGS_NEW_CHARACTER_TRADE_BLOCK_MSG_NOT : IKSerializable
{
    public string NewCharTBMsg { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutWideString(NewCharTBMsg)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string s0)) return false; NewCharTBMsg = s0;
        return true;
    }
}

public sealed class KEGS_NEW_MISSION_NOT : IKSerializable
{
    public bool First { get; set; }
    public List<KMissionInstance> NewMission { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(First)) return false;
        if (!ks.PutList(NewMission)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool v0)) return false; First = v0;
        if (!ks.GetList(NewMission)) return false;
        return true;
    }
}

public sealed class KEGS_NEW_PRESENT_CASH_INVENTORY_NOT : IKSerializable
{
    public int PresentCount { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PresentCount)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; PresentCount = v0;
        return true;
    }
}

public sealed class KEGS_NPC_UNIT_CREATE_MIDDLE_BOSS_NOT : IKSerializable
{
    public KEGS_NPC_UNIT_CREATE_NOT CreatePacket { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(CreatePacket)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(CreatePacket)) return false;
        return true;
    }
}

public sealed class KEGS_NPC_UNIT_DIE_ACK : IKSerializable
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

public sealed class KEGS_PARTY_GAME_START_OPINION_CHECK_NOT : IKSerializable
{
    public int DungeonId { get; set; }
    public sbyte DifficultyLevel { get; set; }
    public sbyte DungeonMode { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(DungeonId)) return false;
        if (!ks.Put(DifficultyLevel)) return false;
        if (!ks.Put(DungeonMode)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; DungeonId = v0;
        if (!ks.Get(out sbyte v1)) return false; DifficultyLevel = v1;
        if (!ks.Get(out sbyte v2)) return false; DungeonMode = v2;
        return true;
    }
}

public sealed class KEGS_PC_BANG_AUTH_RESULT_NOT : IKSerializable
{
    // enum PC_BANG_RESULT_TYPE
    public const int PBRT_FORBIDDEN = 0;
    public const int PBRT_ALLOWED_1 = 1;
    public const int PBRT_ALLOWED_2 = 2;
    public const int PBRT_TERMINATE_1 = 3;
    public const int PBRT_TERMINATE_2 = 4;
    public const int PBRT_TERMINATE_3 = 5;
    public const int PBRT_MESSAGE = 6;
    public const int PBRT_RECONNECT = 7;
    public const int PBRT_PEACE_OF_MIND_LOGIN = 8;
    public const int PBRT_SELECTIVE_SHUTDOWN_SYSTEM = 9;

    public sbyte TypeVal { get; set; }
    public uint Data { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(TypeVal)) return false;
        if (!ks.Put(Data)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte v0)) return false; TypeVal = v0;
        if (!ks.Get(out uint v1)) return false; Data = v1;
        return true;
    }
}

public sealed class KEGS_PSHOP_AGENCY_MESSAGE_NOT : IKSerializable
{
    // enum PSHOP_AGENCY_MESSAGE_TYPE
    public const int PAMT_SELL_ITEM = 0;
    public const int PAMT_SOLD_OUT = 1;
    public const int PAMT_EXPIRATION = 2;
    public const int PAMT_OVER_24_HOURS = 3;

    public sbyte MessageType { get; set; }
    public long HostUnitUid { get; set; }
    public int SellItemId { get; set; }
    public int SellQuantity { get; set; }
    public int SellED { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(MessageType)) return false;
        if (!ks.Put(HostUnitUid)) return false;
        if (!ks.Put(SellItemId)) return false;
        if (!ks.Put(SellQuantity)) return false;
        if (!ks.Put(SellED)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte v0)) return false; MessageType = v0;
        if (!ks.Get(out long v1)) return false; HostUnitUid = v1;
        if (!ks.Get(out int v2)) return false; SellItemId = v2;
        if (!ks.Get(out int v3)) return false; SellQuantity = v3;
        if (!ks.Get(out int v4)) return false; SellED = v4;
        return true;
    }
}

public sealed class KEGS_PVP_KILL_DIE_INFO_NOT : IKSerializable
{
    public bool ShowKillCount { get; set; }
    public int KillCount { get; set; }
    public int DieCount { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ShowKillCount)) return false;
        if (!ks.Put(KillCount)) return false;
        if (!ks.Put(DieCount)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool v0)) return false; ShowKillCount = v0;
        if (!ks.Get(out int v1)) return false; KillCount = v1;
        if (!ks.Get(out int v2)) return false; DieCount = v2;
        return true;
    }
}

public sealed class KEGS_RECOMMEND_USER_INFO_NOT : IKSerializable
{
    public long RecommendUnitUid { get; set; }
    public sbyte TypeVal { get; set; }
    public int Data { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(RecommendUnitUid)) return false;
        if (!ks.Put(TypeVal)) return false;
        if (!ks.Put(Data)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; RecommendUnitUid = v0;
        if (!ks.Get(out sbyte v1)) return false; TypeVal = v1;
        if (!ks.Get(out int v2)) return false; Data = v2;
        return true;
    }
}

public sealed class KEGS_RECOMMEND_USER_RESULT_NOT : IKSerializable
{
    public long RecommendedUnitUid { get; set; }
    public long RecommendUnitUid { get; set; }
    public string FromNickName { get; set; } = string.Empty;
    public byte Level { get; set; }
    public string RecommendDate { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(RecommendedUnitUid)) return false;
        if (!ks.Put(RecommendUnitUid)) return false;
        if (!ks.PutWideString(FromNickName)) return false;
        if (!ks.Put(Level)) return false;
        if (!ks.PutWideString(RecommendDate)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; RecommendedUnitUid = v0;
        if (!ks.Get(out long v1)) return false; RecommendUnitUid = v1;
        if (!ks.GetWideString(out string s2)) return false; FromNickName = s2;
        if (!ks.Get(out byte v3)) return false; Level = v3;
        if (!ks.GetWideString(out string s4)) return false; RecommendDate = s4;
        return true;
    }
}

public sealed class KEGS_REG_AUTO_PARTY_WAIT_LIST_SUCCESS_NOT : IKSerializable
{
    public long WaitNumber { get; set; }
    public int EstimatedTime { get; set; }
    public bool RemakingAutoParty { get; set; }
    public bool AutoPartyBonus { get; set; }
    public int AutoPartyWaitUserCount { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(WaitNumber)) return false;
        if (!ks.Put(EstimatedTime)) return false;
        if (!ks.Put(RemakingAutoParty)) return false;
        if (!ks.Put(AutoPartyBonus)) return false;
        if (!ks.Put(AutoPartyWaitUserCount)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; WaitNumber = v0;
        if (!ks.Get(out int v1)) return false; EstimatedTime = v1;
        if (!ks.Get(out bool v2)) return false; RemakingAutoParty = v2;
        if (!ks.Get(out bool v3)) return false; AutoPartyBonus = v3;
        if (!ks.Get(out int v4)) return false; AutoPartyWaitUserCount = v4;
        return true;
    }
}

public sealed class KEGS_REG_MATCH_WAIT_LIST_SUCCESS_NOT : IKSerializable
{
    public int EstimatedTime { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(EstimatedTime)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; EstimatedTime = v0;
        return true;
    }
}

public sealed class KEGS_RELEASE_PET_ACK : IKSerializable
{
    public long Ok { get; set; }
    public long PetUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(PetUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; Ok = v0;
        if (!ks.Get(out long v1)) return false; PetUid = v1;
        return true;
    }
}

public sealed class KEGS_RELEASE_PET_NOT : IKSerializable
{
    public long PetUid { get; set; }
    public long UnitUid { get; set; }
    public int PetId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PetUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(PetId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; PetUid = v0;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        if (!ks.Get(out int v2)) return false; PetId = v2;
        return true;
    }
}

public sealed class KEGS_RELEASE_PET_REQ : IKSerializable
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

public sealed class KEGS_RELEASE_RIDING_PET_NOT : IKSerializable
{
    public long RidingPetUid { get; set; }
    public ushort RindingPetId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(RidingPetUid)) return false;
        if (!ks.Put(RindingPetId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; RidingPetUid = v0;
        if (!ks.Get(out ushort v1)) return false; RindingPetId = v1;
        return true;
    }
}

public sealed class KEGS_REQUEST_MATCH_MAKING_REQ : IKSerializable
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

public sealed class KEGS_REQUEST_TRADE_DENY_NOT : IKSerializable
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

public sealed class KEGS_RESET_GUILD_SKILL_NOT : IKSerializable
{
    public int DelGuildSkillId { get; set; }
    public int RestoreGuildSPoint { get; set; }
    public int RestoreGuildCSPoint { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(DelGuildSkillId)) return false;
        if (!ks.Put(RestoreGuildSPoint)) return false;
        if (!ks.Put(RestoreGuildCSPoint)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; DelGuildSkillId = v0;
        if (!ks.Get(out int v1)) return false; RestoreGuildSPoint = v1;
        if (!ks.Get(out int v2)) return false; RestoreGuildCSPoint = v2;
        return true;
    }
}

public sealed class KEGS_RESET_PERIOD_ITEM_NOT : IKSerializable
{
    public List<KInventoryItemInfo> InventorySlotInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutList(InventorySlotInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetList(InventorySlotInfo)) return false;
        return true;
    }
}

public sealed class KEGS_RESOLVE_ITEM_AVATAR_ACK : IKSerializable
{
    public int Ok { get; set; }
    public bool Jackpot { get; set; }
    public Dictionary<int, int> InsertedItem { get; set; } = new();
    public List<KInventoryItemInfo> KInventorySlotInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.JP) && !ks.Put(Ok)) return false;
        if ((region.Region == RegionCode.JP) && !ks.Put(Jackpot)) return false;
        if ((region.Region == RegionCode.JP) && !ks.PutMap(InsertedItem, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if ((region.Region == RegionCode.JP) && !ks.PutList(KInventorySlotInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.JP)) { if (!ks.Get(out int v0)) return false; Ok = v0; }
        if ((region.Region == RegionCode.JP)) { if (!ks.Get(out bool v1)) return false; Jackpot = v1; }
        if ((region.Region == RegionCode.JP)) { if (!ks.GetMap(InsertedItem, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false; }
        if ((region.Region == RegionCode.JP)) { if (!ks.GetList(KInventorySlotInfo)) return false; }
        return true;
    }
}

public sealed class KEGS_RESTORE_SPIRIT_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public int Spirit { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(Spirit)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out int v1)) return false; Spirit = v1;
        return true;
    }
}

public sealed class KEGS_RESULT_ALL_UNIT_SUCCESS_NOT : IKSerializable
{
    public KRoomInfo RoomInfo { get; set; } = new();
    public List<KRoomSlotInfo> Slot { get; set; } = new();
    public KReturnToFieldInfo ReturnToFieldInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(RoomInfo)) return false;
        if (!ks.PutList(Slot)) return false;
        if (!ks.Put(ReturnToFieldInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(RoomInfo)) return false;
        if (!ks.GetList(Slot)) return false;
        if (!ks.Get(ReturnToFieldInfo)) return false;
        return true;
    }
}

public sealed class KEGS_REWARD_TITLE_NOT : IKSerializable
{
    public KTitleInfo Title { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Title)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(Title)) return false;
        return true;
    }
}

public sealed class KEGS_REWARD_TO_RECOMMEND_USER_NOT : IKSerializable
{
    public string NickName { get; set; } = string.Empty;
    public sbyte Level { get; set; }
    public Dictionary<int, int> RewardItem { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutWideString(NickName)) return false;
        if (!ks.Put(Level)) return false;
        if (!ks.PutMap(RewardItem, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string s0)) return false; NickName = s0;
        if (!ks.Get(out sbyte v1)) return false; Level = v1;
        if (!ks.GetMap(RewardItem, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false;
        return true;
    }
}

public sealed class KEGS_REWARD_TO_TUTORIAL_USER_NOT : IKSerializable
{
    public string NickName { get; set; } = string.Empty;
    public sbyte Level { get; set; }
    public Dictionary<int, int> RewardItem { get; set; } = new();
    public bool Graduation { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutWideString(NickName)) return false;
        if (!ks.Put(Level)) return false;
        if (!ks.PutMap(RewardItem, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.Put(Graduation)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string s0)) return false; NickName = s0;
        if (!ks.Get(out sbyte v1)) return false; Level = v1;
        if (!ks.GetMap(RewardItem, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false;
        if (!ks.Get(out bool v3)) return false; Graduation = v3;
        return true;
    }
}

public sealed class KEGS_SELECT_REWARD_BOX_ACK : IKSerializable
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

public sealed class KEGS_SELECT_REWARD_BOX_NOT : IKSerializable
{
    public sbyte BoxNum { get; set; }
    public long UnitUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(BoxNum)) return false;
        if (!ks.Put(UnitUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte v0)) return false; BoxNum = v0;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        return true;
    }
}

public sealed class KEGS_SELECT_REWARD_BOX_REQ : IKSerializable
{
    public sbyte BoxNum { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(BoxNum)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte v0)) return false; BoxNum = v0;
        return true;
    }
}

public sealed class KEGS_SELECT_UNIT_1_NOT : IKSerializable
{
    public int Ok { get; set; }
    public KUnitInfo UnitInfo { get; set; } = new();
    public string UnitCreateDate { get; set; } = string.Empty;
    public int RealDataED { get; set; }
    public string UnitLastLoginDate { get; set; } = string.Empty;
    public Dictionary<int, int> InventorySlotSize { get; set; } = new();
    public Dictionary<long, KInventoryItemInfo> Item { get; set; } = new();
    public int NumResurrectionStone { get; set; }
    public int NumAutoPaymentResStone { get; set; }
    public string AutoPayResStoneLastDate { get; set; } = string.Empty;
    public List<KUserSkillPageData> UserSkillPageData { get; set; } = new();
    public List<short> SkillUnsealed { get; set; } = new();
    public sbyte SkillNoteMaxPageNum { get; set; }
    public Dictionary<sbyte, int> SkillNote { get; set; } = new();
    public bool IsNewUnitTradeBlock { get; set; }
    public int NewUnitTradeBlockDay { get; set; }
    public int NewUnitTradeBlockUnitClass { get; set; }
    public KRelationshipInfo RelationshipInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(UnitInfo)) return false;
        if (!ks.PutWideString(UnitCreateDate)) return false;
        if (!ks.Put(RealDataED)) return false;
        if (!ks.PutWideString(UnitLastLoginDate)) return false;
        if (!ks.PutMap(InventorySlotSize, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.PutMap(Item, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.Put(NumResurrectionStone)) return false;
        if (!ks.Put(NumAutoPaymentResStone)) return false;
        if (!ks.PutWideString(AutoPayResStoneLastDate)) return false;
        if (!ks.PutList(UserSkillPageData)) return false;
        if (!ks.PutList(SkillUnsealed, static (s, v) => s.Put(v))) return false;
        if (!ks.Put(SkillNoteMaxPageNum)) return false;
        if (!ks.PutMap(SkillNote, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.Put(IsNewUnitTradeBlock)) return false;
        if (!ks.Put(NewUnitTradeBlockDay)) return false;
        if (!ks.Put(NewUnitTradeBlockUnitClass)) return false;
        if (!ks.Put(RelationshipInfo)) return false;
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
        if (!ks.Get(out int v7)) return false; NumResurrectionStone = v7;
        if (!ks.Get(out int v8)) return false; NumAutoPaymentResStone = v8;
        if (!ks.GetWideString(out string s9)) return false; AutoPayResStoneLastDate = s9;
        if (!ks.GetList(UserSkillPageData)) return false;
        if (!ks.GetList(SkillUnsealed, static s => (s.Get(out short v), v))) return false;
        if (!ks.Get(out sbyte v12)) return false; SkillNoteMaxPageNum = v12;
        if (!ks.GetMap(SkillNote, static s => (s.Get(out sbyte v), v), static s => (s.Get(out int v), v))) return false;
        if (!ks.Get(out bool v14)) return false; IsNewUnitTradeBlock = v14;
        if (!ks.Get(out int v15)) return false; NewUnitTradeBlockDay = v15;
        if (!ks.Get(out int v16)) return false; NewUnitTradeBlockUnitClass = v16;
        if (!ks.Get(RelationshipInfo)) return false;
        return true;
    }
}

public sealed class KEGS_SELECT_UNIT_2_NOT : IKSerializable
{
    public int Ok { get; set; }
    public List<KQuestInstance> Quest { get; set; } = new();
    public List<KCompleteQuestInfo> CompletQuest { get; set; } = new();
    public Dictionary<int, int> RandomQuestList { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.PutList(Quest)) return false;
        if (!ks.PutList(CompletQuest)) return false;
        if (!ks.PutMap(RandomQuestList, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.GetList(Quest)) return false;
        if (!ks.GetList(CompletQuest)) return false;
        if (!ks.GetMap(RandomQuestList, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false;
        return true;
    }
}

public sealed class KEGS_SELECT_UNIT_3_NOT : IKSerializable
{
    public int Ok { get; set; }
    public List<KMissionInstance> Mission { get; set; } = new();
    public List<KTitleInfo> Title { get; set; } = new();
    public long SummonedPetUid { get; set; }
    public List<KPetInfo> PetList { get; set; } = new();
    public List<long> CanEvolutionPetList { get; set; } = new();
    public List<long> NeverSummonPetList { get; set; } = new();
    public Dictionary<long, Dictionary<long, KInventoryItemInfo>> PetItem { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.PutList(Mission)) return false;
        if (!ks.PutList(Title)) return false;
        if (!ks.Put(SummonedPetUid)) return false;
        if (!ks.PutList(PetList)) return false;
        if (!ks.PutList(CanEvolutionPetList, static (s, v) => s.Put(v))) return false;
        if (!ks.PutList(NeverSummonPetList, static (s, v) => s.Put(v))) return false;
        if (!ks.PutMap(PetItem, static (s, k) => s.Put(k), static (s, v) => s.PutMap(v, static (s2, k2) => s2.Put(k2), static (s2, v2) => s2.Put(v2)))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.GetList(Mission)) return false;
        if (!ks.GetList(Title)) return false;
        if (!ks.Get(out long v3)) return false; SummonedPetUid = v3;
        if (!ks.GetList(PetList)) return false;
        if (!ks.GetList(CanEvolutionPetList, static s => (s.Get(out long v), v))) return false;
        if (!ks.GetList(NeverSummonPetList, static s => (s.Get(out long v), v))) return false;
        if (!ks.GetMap(PetItem, static s => (s.Get(out long v), v), static s => { var d = new Dictionary<long, KInventoryItemInfo>(); return (s.GetMap(d, static s2 => (s2.Get(out long v), v), static s2 => { var v = new KInventoryItemInfo(); return (s2.Get(v), v); }), d); })) return false;
        return true;
    }
}

public sealed class KEGS_SELECT_UNIT_4_NOT : IKSerializable
{
    public int Ok { get; set; }
    public List<KChatBlackListUnit> ChatBlackList { get; set; } = new();
    public KDenyOptions DenyOptions { get; set; } = new();
    public bool IsRecommend { get; set; }
    public long RecommendUnitUid { get; set; }
    public List<KTutorialDBUnitInfo> TutorialDBUnitInfo { get; set; } = new();
    public List<KConnectTimeEventInfo> ConnectTimeEvent { get; set; } = new();
    public List<KCumulativeTimeEventInfo> CumulativeTimeEvent { get; set; } = new();
    public List<KHenirRankingInfo> HenirRanking { get; set; } = new();
    public KMessengerInfo MessengerInfo { get; set; } = new();
    public KGamePlayStatusContainer GamePlayStatus { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.PutList(ChatBlackList)) return false;
        if (!ks.Put(DenyOptions)) return false;
        if (!ks.Put(IsRecommend)) return false;
        if (!ks.Put(RecommendUnitUid)) return false;
        if (!ks.PutList(TutorialDBUnitInfo)) return false;
        if (!ks.PutList(ConnectTimeEvent)) return false;
        if (!ks.PutList(CumulativeTimeEvent)) return false;
        if (!ks.PutList(HenirRanking)) return false;
        if (!ks.Put(MessengerInfo)) return false;
        if (!ks.Put(GamePlayStatus)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.GetList(ChatBlackList)) return false;
        if (!ks.Get(DenyOptions)) return false;
        if (!ks.Get(out bool v3)) return false; IsRecommend = v3;
        if (!ks.Get(out long v4)) return false; RecommendUnitUid = v4;
        if (!ks.GetList(TutorialDBUnitInfo)) return false;
        if (!ks.GetList(ConnectTimeEvent)) return false;
        if (!ks.GetList(CumulativeTimeEvent)) return false;
        if (!ks.GetList(HenirRanking)) return false;
        if (!ks.Get(MessengerInfo)) return false;
        if (!ks.Get(GamePlayStatus)) return false;
        return true;
    }
}

public sealed class KEGS_SELECT_UNIT_5_NOT : IKSerializable
{
    public int Ok { get; set; }
    public bool IsSpiritUpdated { get; set; }
    public Dictionary<int, int> CharGameCount { get; set; } = new();
    public KUserPShopAgencyInfo PShopAgencyInfo { get; set; } = new();
    public KEGS_HENIR_REWARD_COUNT_NOT PacketHenirRewardCount { get; set; } = new();
    public bool CashShopOpen { get; set; }
    public bool FirstSelect { get; set; }
    public bool Couple { get; set; }
    public long RelationTargetUserUid { get; set; }
    public string RelationTargetUserNickname { get; set; } = string.Empty;
    public List<bool> Vec4ThAnnivEventRewardInfo { get; set; } = new();
    public long LastRewardTime { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(IsSpiritUpdated)) return false;
        if (!ks.PutMap(CharGameCount, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.Put(PShopAgencyInfo)) return false;
        if (!ks.Put(PacketHenirRewardCount)) return false;
        if (!ks.Put(CashShopOpen)) return false;
        if (!ks.Put(FirstSelect)) return false;
        if ((region.Region == RegionCode.CN) && !ks.Put(Couple)) return false;
        if ((region.Region == RegionCode.CN) && !ks.Put(RelationTargetUserUid)) return false;
        if ((region.Region == RegionCode.CN) && !ks.PutWideString(RelationTargetUserNickname)) return false;
        if ((region.Region == RegionCode.JP) && !ks.PutList(Vec4ThAnnivEventRewardInfo, static (s, v) => s.Put(v))) return false;
        if ((region.Region == RegionCode.JP) && !ks.Put(LastRewardTime)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out bool v1)) return false; IsSpiritUpdated = v1;
        if (!ks.GetMap(CharGameCount, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false;
        if (!ks.Get(PShopAgencyInfo)) return false;
        if (!ks.Get(PacketHenirRewardCount)) return false;
        if (!ks.Get(out bool v5)) return false; CashShopOpen = v5;
        if (!ks.Get(out bool v6)) return false; FirstSelect = v6;
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out bool v7)) return false; Couple = v7; }
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out long v8)) return false; RelationTargetUserUid = v8; }
        if ((region.Region == RegionCode.CN)) { if (!ks.GetWideString(out string s9)) return false; RelationTargetUserNickname = s9; }
        if ((region.Region == RegionCode.JP)) { if (!ks.GetList(Vec4ThAnnivEventRewardInfo, static s => (s.Get(out bool v), v))) return false; }
        if ((region.Region == RegionCode.JP)) { if (!ks.Get(out long v11)) return false; LastRewardTime = v11; }
        return true;
    }
}

public sealed class KEGS_SHARING_BACK_OPEN_NOT : IKSerializable
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

public sealed class KEGS_SKILL_SLOT_CHANGE_ITEM_NOT : IKSerializable
{
    public sbyte SkillSlotBExpirationState { get; set; }
    public string SkillSlotBEndDate { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(SkillSlotBExpirationState)) return false;
        if (!ks.PutWideString(SkillSlotBEndDate)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte v0)) return false; SkillSlotBExpirationState = v0;
        if (!ks.GetWideString(out string s1)) return false; SkillSlotBEndDate = s1;
        return true;
    }
}

public class KEGS_TIME_EVENT_INFO_NOT : IKSerializable
{
    // enum TIME_EVENT_NOTIFY_TYPE
    public const int TENT_INIT = 0;
    public const int TENT_BEGIN = 1;
    public const int TENT_END = 2;

    public sbyte NotifyType { get; set; }
    public List<KTimeEventSimpleInfo> TimeEvent { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(NotifyType)) return false;
        if (!ks.PutList(TimeEvent)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte v0)) return false; NotifyType = v0;
        if (!ks.GetList(TimeEvent)) return false;
        return true;
    }
}

public sealed class KEGS_TITLE_EXPIRATION_NOT : IKSerializable
{
    public int TitleId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(TitleId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; TitleId = v0;
        return true;
    }
}

public sealed class KEGS_TRADE_USER_INFO_NOT : IKSerializable
{
    public List<KTradeUserInfo> TradeUserInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutList(TradeUserInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetList(TradeUserInfo)) return false;
        return true;
    }
}

public sealed class KEGS_TUTORIAL_UPDATE_UNIT_INFO_NOT : IKSerializable
{
    public bool IsReply { get; set; }
    public long ReceiverUid { get; set; }
    public KTutorialUnitInfo TutorialUnitInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(IsReply)) return false;
        if (!ks.Put(ReceiverUid)) return false;
        if (!ks.Put(TutorialUnitInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool v0)) return false; IsReply = v0;
        if (!ks.Get(out long v1)) return false; ReceiverUid = v1;
        if (!ks.Get(TutorialUnitInfo)) return false;
        return true;
    }
}

public class KEGS_UNBLOCK_FRIEND_ACK : IKSerializable
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

public sealed class KEGS_UNREG_AUTO_PARTY_WAIT_LIST_NOT : IKSerializable
{
    public int Reason { get; set; }
    public string CancelUnitNickName { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Reason)) return false;
        if (!ks.PutWideString(CancelUnitNickName)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Reason = v0;
        if (!ks.GetWideString(out string s1)) return false; CancelUnitNickName = s1;
        return true;
    }
}

public sealed class KEGS_UNREG_MATCH_WAIT_LIST_NOT : IKSerializable
{
    public int Reason { get; set; }
    public string CancelUnitNickName { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Reason)) return false;
        if (!ks.PutWideString(CancelUnitNickName)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Reason = v0;
        if (!ks.GetWideString(out string s1)) return false; CancelUnitNickName = s1;
        return true;
    }
}

public sealed class KEGS_UNSEAL_SKILL_NOT : IKSerializable
{
    public int SkillId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(SkillId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; SkillId = v0;
        return true;
    }
}

public sealed class KEGS_UPDATE_BUFF_INFO_IN_ROOM_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public List<KBuffInfo> ActivateBuffList { get; set; } = new();
    public KStat GameStat { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutList(ActivateBuffList)) return false;
        if (!ks.Put(GameStat)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.GetList(ActivateBuffList)) return false;
        if (!ks.Get(GameStat)) return false;
        return true;
    }
}

public sealed class KEGS_UPDATE_BUFF_INFO_NOT : IKSerializable
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

public sealed class KEGS_UPDATE_CHECK_POWER_NOT : IKSerializable
{
    public byte CheckPowerCount { get; set; }
    public long CheckPowerTime { get; set; }
    public byte CheckPowerScore { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.JP || region.Region == RegionCode.TW) && !ks.Put(CheckPowerCount)) return false;
        if ((region.Region == RegionCode.JP || region.Region == RegionCode.TW) && !ks.Put(CheckPowerTime)) return false;
        if ((region.Region == RegionCode.JP || region.Region == RegionCode.TW) && !ks.Put(CheckPowerScore)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.JP || region.Region == RegionCode.TW)) { if (!ks.Get(out byte v0)) return false; CheckPowerCount = v0; }
        if ((region.Region == RegionCode.JP || region.Region == RegionCode.TW)) { if (!ks.Get(out long v1)) return false; CheckPowerTime = v1; }
        if ((region.Region == RegionCode.JP || region.Region == RegionCode.TW)) { if (!ks.Get(out byte v2)) return false; CheckPowerScore = v2; }
        return true;
    }
}

public sealed class KEGS_UPDATE_CODE_EVENT_NOT : IKSerializable
{
    public Dictionary<int, bool> EnableCodeEnum { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutMap(EnableCodeEnum, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetMap(EnableCodeEnum, static s => (s.Get(out int v), v), static s => (s.Get(out bool v), v))) return false;
        return true;
    }
}

public sealed class KEGS_UPDATE_DUNGEON_STATUS_NOT : IKSerializable
{
    public Dictionary<int, bool> EnableDungeon { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutMap(EnableDungeon, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetMap(EnableDungeon, static s => (s.Get(out int v), v), static s => (s.Get(out bool v), v))) return false;
        return true;
    }
}

public sealed class KEGS_UPDATE_INVENTORY_SLOT_INFO_NOT : IKSerializable
{
    public List<KInventoryItemInfo> InventorySlotInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutList(InventorySlotInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetList(InventorySlotInfo)) return false;
        return true;
    }
}

public sealed class KEGS_UPDATE_ITEM_POSITION_NOT : IKSerializable
{
    public List<KTradeShareItemResult> ShareItemResult { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutList(ShareItemResult)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetList(ShareItemResult)) return false;
        return true;
    }
}

public sealed class KEGS_UPDATE_MAX_LEVEL_NOT : IKSerializable
{
    public int MaxLevel { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(MaxLevel)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; MaxLevel = v0;
        return true;
    }
}

public sealed class KEGS_UPDATE_MISSION_NOT : IKSerializable
{
    public List<KMissionInstance> MissionInst { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutList(MissionInst)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetList(MissionInst)) return false;
        return true;
    }
}

public sealed class KEGS_UPDATE_MOVE_TO_PARTY_MEMBER_INFO_NOT : IKSerializable
{
    public bool IsOn { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(IsOn)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool v0)) return false; IsOn = v0;
        return true;
    }
}

public sealed class KEGS_UPDATE_NPC_UNIT_BUFF_INFO_BROAD_NOT : IKSerializable
{
    public List<KNpcUnitBuffInfo> NpcUnitBuff { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutList(NpcUnitBuff)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetList(NpcUnitBuff)) return false;
        return true;
    }
}

public sealed class KEGS_UPDATE_PARTY_MEMBER_STATUS_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public KPartyMemberStatus Status { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(Status)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(Status)) return false;
        return true;
    }
}

public sealed class KEGS_UPDATE_PARTY_USER_INFO_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public byte State { get; set; }
    public int StateCode { get; set; }
    public sbyte UnitClass { get; set; }
    public byte Level { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(State)) return false;
        if (!ks.Put(StateCode)) return false;
        if (!ks.Put(UnitClass)) return false;
        if (!ks.Put(Level)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out byte v1)) return false; State = v1;
        if (!ks.Get(out int v2)) return false; StateCode = v2;
        if (!ks.Get(out sbyte v3)) return false; UnitClass = v3;
        if (!ks.Get(out byte v4)) return false; Level = v4;
        return true;
    }
}

public sealed class KEGS_UPDATE_QUEST_ACK : IKSerializable
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

public sealed class KEGS_UPDATE_QUEST_NOT : IKSerializable
{
    public List<KQuestInstance> QuestInst { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutList(QuestInst)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetList(QuestInst)) return false;
        return true;
    }
}

public sealed class KEGS_UPDATE_QUEST_REQ : IKSerializable
{
    public KQuestInstance QuestInst { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(QuestInst)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(QuestInst)) return false;
        return true;
    }
}

public sealed class KEGS_UPDATE_SQUARE_PERSONAL_SHOP_INFO_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public sbyte PersonalShopState { get; set; }
    public sbyte PersonalShopType { get; set; }
    public string PersonalShopName { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(PersonalShopState)) return false;
        if (!ks.Put(PersonalShopType)) return false;
        if (!ks.PutWideString(PersonalShopName)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out sbyte v1)) return false; PersonalShopState = v1;
        if (!ks.Get(out sbyte v2)) return false; PersonalShopType = v2;
        if (!ks.GetWideString(out string s3)) return false; PersonalShopName = s3;
        return true;
    }
}

public sealed class KEGS_UPDATE_TRADE_ED_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public int Ed { get; set; }
    public Dictionary<long, bool> OfferState { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(Ed)) return false;
        if (!ks.PutMap(OfferState, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out int v1)) return false; Ed = v1;
        if (!ks.GetMap(OfferState, static s => (s.Get(out long v), v), static s => (s.Get(out bool v), v))) return false;
        return true;
    }
}

public sealed class KEGS_UPDATE_TRADE_ITEM_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public long ItemUid { get; set; }
    public bool Delete { get; set; }
    public KItemInfo ItemInfo { get; set; } = new();
    public Dictionary<long, bool> OfferState { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(ItemUid)) return false;
        if (!ks.Put(Delete)) return false;
        if (!ks.Put(ItemInfo)) return false;
        if (!ks.PutMap(OfferState, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out long v1)) return false; ItemUid = v1;
        if (!ks.Get(out bool v2)) return false; Delete = v2;
        if (!ks.Get(ItemInfo)) return false;
        if (!ks.GetMap(OfferState, static s => (s.Get(out long v), v), static s => (s.Get(out bool v), v))) return false;
        return true;
    }
}

public sealed class KEGS_UPDATE_USER_UNIT_BUFF_INFO_BROAD_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public List<KBuffFactor> BuffFactor { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutList(BuffFactor)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.GetList(BuffFactor)) return false;
        return true;
    }
}

public sealed class KEGS_UPDATE_WORLD_BUFF_NOT : IKSerializable
{
    public ushort SuccessWorldBuff { get; set; }
    public ushort WorldBuffDurationTime { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(SuccessWorldBuff)) return false;
        if (!ks.Put(WorldBuffDurationTime)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out ushort v0)) return false; SuccessWorldBuff = v0;
        if (!ks.Get(out ushort v1)) return false; WorldBuffDurationTime = v1;
        return true;
    }
}

public sealed class KEGS_USER_UNIT_RE_BIRTH_NOT : IKSerializable
{
    public long UserUnitUid { get; set; }
    public int StartPosIndex { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UserUnitUid)) return false;
        if (!ks.Put(StartPosIndex)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UserUnitUid = v0;
        if (!ks.Get(out int v1)) return false; StartPosIndex = v1;
        return true;
    }
}

public sealed class KEGS_USE_SKILL_BUFF_IN_PLAY_BROAD_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public int SkillId { get; set; }
    public bool On { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(SkillId)) return false;
        if (!ks.Put(On)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out int v1)) return false; SkillId = v1;
        if (!ks.Get(out bool v2)) return false; On = v2;
        return true;
    }
}

public sealed class KEGS_WORLD_MISSION_UPDATE_NOT : IKSerializable
{
    public bool Active { get; set; }
    public int RemainTime { get; set; }
    public int WorldCrystalCount { get; set; }
    public int WorldMaxCrystalCount { get; set; }
    public int MissionStartRemainTime { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Active)) return false;
        if (!ks.Put(RemainTime)) return false;
        if (!ks.Put(WorldCrystalCount)) return false;
        if (!ks.Put(WorldMaxCrystalCount)) return false;
        if (!ks.Put(MissionStartRemainTime)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool v0)) return false; Active = v0;
        if (!ks.Get(out int v1)) return false; RemainTime = v1;
        if (!ks.Get(out int v2)) return false; WorldCrystalCount = v2;
        if (!ks.Get(out int v3)) return false; WorldMaxCrystalCount = v3;
        if (!ks.Get(out int v4)) return false; MissionStartRemainTime = v4;
        return true;
    }
}

public sealed class KEGS_ZERO_ENDURANCE_ITEM_IN_ROOM_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public List<KInventoryItemInfo> InventorySlotInfo { get; set; } = new();
    public KStat GameStat { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutList(InventorySlotInfo)) return false;
        if (!ks.Put(GameStat)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.GetList(InventorySlotInfo)) return false;
        if (!ks.Get(GameStat)) return false;
        return true;
    }
}

public sealed class KENX_USER_LOGIN_NOT : IKSerializable
{
    public bool IsGameBang { get; set; }
    public KPcBangReward PcBangReward { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(IsGameBang)) return false;
        if (region.PcBangType && !ks.Put(PcBangReward)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool v0)) return false; IsGameBang = v0;
        if (region.PcBangType && !ks.Get(PcBangReward)) return false;
        return true;
    }
}

public sealed class KETR_CHANGE_EQUIPPED_ITEM_IN_FIELD_NOT : IKSerializable
{
    public List<long> Uid { get; set; } = new();
    public long UnitUid { get; set; }
    public List<KInventoryItemInfo> InventorySlotInfo { get; set; } = new();
    public int EquippedTitleId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutList(Uid, static (s, v) => s.Put(v))) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutList(InventorySlotInfo)) return false;
        if (!ks.Put(EquippedTitleId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetList(Uid, static s => (s.Get(out long v), v))) return false;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        if (!ks.GetList(InventorySlotInfo)) return false;
        if (!ks.Get(out int v3)) return false; EquippedTitleId = v3;
        return true;
    }
}

public sealed class KETR_CHANGE_FIELD_USER_INFO_NOT : IKSerializable
{
    public List<long> Uid { get; set; } = new();
    public long UnitUid { get; set; }
    public sbyte UnitClass { get; set; }
    public string GuildName { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutList(Uid, static (s, v) => s.Put(v))) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(UnitClass)) return false;
        if (!ks.PutWideString(GuildName)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetList(Uid, static s => (s.Get(out long v), v))) return false;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        if (!ks.Get(out sbyte v2)) return false; UnitClass = v2;
        if (!ks.GetWideString(out string s3)) return false; GuildName = s3;
        return true;
    }
}

public sealed class KETR_CHANGE_USER_LIST_IN_SECTOR_NOT : IKSerializable
{
    public List<long> Uid { get; set; } = new();
    public List<KFieldUserInfo> NewUserList { get; set; } = new();
    public List<long> DelUserList { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutList(Uid, static (s, v) => s.Put(v))) return false;
        if (!ks.PutList(NewUserList)) return false;
        if (!ks.PutList(DelUserList, static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetList(Uid, static s => (s.Get(out long v), v))) return false;
        if (!ks.GetList(NewUserList)) return false;
        if (!ks.GetList(DelUserList, static s => (s.Get(out long v), v))) return false;
        return true;
    }
}

public sealed class KETR_CHAT_NOT : IKSerializable
{
    public List<long> Uid { get; set; } = new();
    public sbyte ChatPacketType { get; set; }
    public long SenderUnitUid { get; set; }
    public string SenderUnitNickName { get; set; } = string.Empty;
    public string RecieverUnitNickName { get; set; } = string.Empty;
    public long RecieverUnitUid { get; set; }
    public sbyte RoomChatType { get; set; }
    public string Msg { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutList(Uid, static (s, v) => s.Put(v))) return false;
        if (!ks.Put(ChatPacketType)) return false;
        if (!ks.Put(SenderUnitUid)) return false;
        if (!ks.PutWideString(SenderUnitNickName)) return false;
        if (!ks.PutWideString(RecieverUnitNickName)) return false;
        if (!ks.Put(RecieverUnitUid)) return false;
        if (!ks.Put(RoomChatType)) return false;
        if (!ks.PutWideString(Msg)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetList(Uid, static s => (s.Get(out long v), v))) return false;
        if (!ks.Get(out sbyte v1)) return false; ChatPacketType = v1;
        if (!ks.Get(out long v2)) return false; SenderUnitUid = v2;
        if (!ks.GetWideString(out string s3)) return false; SenderUnitNickName = s3;
        if (!ks.GetWideString(out string s4)) return false; RecieverUnitNickName = s4;
        if (!ks.Get(out long v5)) return false; RecieverUnitUid = v5;
        if (!ks.Get(out sbyte v6)) return false; RoomChatType = v6;
        if (!ks.GetWideString(out string s7)) return false; Msg = s7;
        return true;
    }
}

public sealed class KETR_COUPLE_MAKING_SUCCESS_NOT : IKSerializable
{
    public List<long> Uid { get; set; } = new();
    public string RequestUnitName { get; set; } = string.Empty;
    public string AcceptUnitName { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutList(Uid, static (s, v) => s.Put(v))) return false;
        if (!ks.PutWideString(RequestUnitName)) return false;
        if (!ks.PutWideString(AcceptUnitName)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetList(Uid, static s => (s.Get(out long v), v))) return false;
        if (!ks.GetWideString(out string s1)) return false; RequestUnitName = s1;
        if (!ks.GetWideString(out string s2)) return false; AcceptUnitName = s2;
        return true;
    }
}

public sealed class KETR_FIELD_UNIT_SYNC_DATA_BROAD_NOT : IKSerializable
{
    public List<long> Uid { get; set; } = new();
    public long UnitUid { get; set; }
    public KVector3 VPos { get; set; } = new();
    public List<KFieldFrameInfo> FieldFrame { get; set; } = new();
    public bool IsRight { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutList(Uid, static (s, v) => s.Put(v))) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(VPos)) return false;
        if (!ks.PutList(FieldFrame)) return false;
        if (!ks.Put(IsRight)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetList(Uid, static s => (s.Get(out long v), v))) return false;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        if (!ks.Get(VPos)) return false;
        if (!ks.GetList(FieldFrame)) return false;
        if (!ks.Get(out bool v4)) return false; IsRight = v4;
        return true;
    }
}

public sealed class KETR_PET_ACTION_NOT : IKSerializable
{
    public List<long> Uid { get; set; } = new();
    public long UnitUid { get; set; }
    public sbyte ActionType { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutList(Uid, static (s, v) => s.Put(v))) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(ActionType)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetList(Uid, static s => (s.Get(out long v), v))) return false;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        if (!ks.Get(out sbyte v2)) return false; ActionType = v2;
        return true;
    }
}

public sealed class KETR_PET_EVOLUTION_NOT : IKSerializable
{
    public List<long> Uid { get; set; } = new();
    public long UnitUid { get; set; }
    public KPetInfo EvolutionPetInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutList(Uid, static (s, v) => s.Put(v))) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(EvolutionPetInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetList(Uid, static s => (s.Get(out long v), v))) return false;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        if (!ks.Get(EvolutionPetInfo)) return false;
        return true;
    }
}

public sealed class KETR_RELATIONSHIP_EFFECT_NOT : IKSerializable
{
    public List<long> Uid { get; set; } = new();
    public long ManUid { get; set; }
    public long GirlUid { get; set; }
    public string LoveWord { get; set; } = string.Empty;
    public int EffectIndex { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutList(Uid, static (s, v) => s.Put(v))) return false;
        if (!ks.Put(ManUid)) return false;
        if (!ks.Put(GirlUid)) return false;
        if (!ks.PutWideString(LoveWord)) return false;
        if (!ks.Put(EffectIndex)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetList(Uid, static s => (s.Get(out long v), v))) return false;
        if (!ks.Get(out long v1)) return false; ManUid = v1;
        if (!ks.Get(out long v2)) return false; GirlUid = v2;
        if (!ks.GetWideString(out string s3)) return false; LoveWord = s3;
        if (!ks.Get(out int v4)) return false; EffectIndex = v4;
        return true;
    }
}

public sealed class KETR_SUMMON_PET_NOT : IKSerializable
{
    public List<long> Uid { get; set; } = new();
    public long UnitUid { get; set; }
    public List<KPetInfo> PetInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutList(Uid, static (s, v) => s.Put(v))) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutList(PetInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetList(Uid, static s => (s.Get(out long v), v))) return false;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        if (!ks.GetList(PetInfo)) return false;
        return true;
    }
}

public sealed class KETR_SUMMON_RIDING_PET_NOT : IKSerializable
{
    public List<long> Uid { get; set; } = new();
    public long UnitUid { get; set; }
    public long RidingPetUid { get; set; }
    public ushort RidingPetId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutList(Uid, static (s, v) => s.Put(v))) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(RidingPetUid)) return false;
        if (!ks.Put(RidingPetId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetList(Uid, static s => (s.Get(out long v), v))) return false;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        if (!ks.Get(out long v2)) return false; RidingPetUid = v2;
        if (!ks.Get(out ushort v3)) return false; RidingPetId = v3;
        return true;
    }
}

public sealed class KETR_TOGGLE_INVISIBLE_NOT : IKSerializable
{
    public List<long> Uid { get; set; } = new();
    public long UnitUid { get; set; }
    public KFieldUserInfo FieldUserInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutList(Uid, static (s, v) => s.Put(v))) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(FieldUserInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetList(Uid, static s => (s.Get(out long v), v))) return false;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        if (!ks.Get(FieldUserInfo)) return false;
        return true;
    }
}

public sealed class KETR_UNSUMMON_RIDING_PET_NOT : IKSerializable
{
    public List<long> Uid { get; set; } = new();
    public long UnitUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutList(Uid, static (s, v) => s.Put(v))) return false;
        if (!ks.Put(UnitUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetList(Uid, static s => (s.Get(out long v), v))) return false;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        return true;
    }
}

public sealed class KETR_UPDATE_PARTY_NOT : IKSerializable
{
    public List<long> Uid { get; set; } = new();
    public long UnitUid { get; set; }
    public long PartyUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutList(Uid, static (s, v) => s.Put(v))) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(PartyUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetList(Uid, static s => (s.Get(out long v), v))) return false;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        if (!ks.Get(out long v2)) return false; PartyUid = v2;
        return true;
    }
}

public sealed class KECH_GET_CHANNEL_LIST_ACK : KPacketOK { }

public sealed class KECH_GET_SERVERGROUP_LIST_ACK : KPacketOK { }

public sealed class KEGS_ADMIN_BOSS_FIELD_GATE_OPEN_ACK : KPacketOK { }

public sealed class KEGS_ADMIN_CHANGE_AUTH_LEVEL_ACK : KPacketOK { }

public sealed class KEGS_ADMIN_CHANGE_COUPLE_DATE_ACK : KPacketOK { }

public sealed class KEGS_ADMIN_CHANGE_ED_ACK : KPacketOK { }

public sealed class KEGS_ADMIN_GET_TITLE_CHEAT_ACK : KPacketOK { }

public sealed class KEGS_ADMIN_HERO_PVP_USER_ADD_ACK : IKSerializable
{
    public List<(long, string)> UserList { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutList(UserList, static (s, v) => s.Put(v.Item1) && s.PutWideString(v.Item2))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetList(UserList, static s => { bool ok1 = s.Get(out long a); bool ok2 = s.GetWideString(out string b); return (ok1 && ok2, (a, b)); })) return false;
        return true;
    }
}

public sealed class KEGS_ADMIN_HERO_PVP_USER_REMOVE_ACK : IKSerializable
{
    public List<(long, string)> UserList { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutList(UserList, static (s, v) => s.Put(v.Item1) && s.PutWideString(v.Item2))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetList(UserList, static s => { bool ok1 = s.Get(out long a); bool ok2 = s.GetWideString(out string b); return (ok1 && ok2, (a, b)); })) return false;
        return true;
    }
}

public sealed class KEGS_ADMIN_HERO_PVP_USER_REMOVE_REQ : IKSerializable
{
    public List<(long, string)> UserList { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutList(UserList, static (s, v) => s.Put(v.Item1) && s.PutWideString(v.Item2))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetList(UserList, static s => { bool ok1 = s.Get(out long a); bool ok2 = s.GetWideString(out string b); return (ok1 && ok2, (a, b)); })) return false;
        return true;
    }
}

public sealed class KEGS_ADMIN_KICK_USER_ACK : KPacketOK { }

public sealed class KEGS_ADMIN_OBSERVER_MODE_ACK : KPacketOK { }

public sealed class KEGS_ADMIN_SET_TOTAL_DANGEROUS_VALUE_ACK : KPacketOK { }

public sealed class KEGS_ADVERTISEMENT_EVENT_INFO_NOT : KEGS_TIME_EVENT_INFO_NOT { }

public sealed class KEGS_APPLY_COUPON_ACK : KPacketOK { }

public sealed class KEGS_APPLY_JOIN_GUILD_ACK : KPacketOK { }

public sealed class KEGS_APPROVE_TRADE_ACK : KPacketOK { }

public sealed class KEGS_BATTLE_FIELD_NPC_LOAD_COMPLETE_ACK : KPacketOK { }

public sealed class KEGS_BILL_GET_PURCHASED_CASH_ITEM_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int NumResurrectionStone { get; set; }
    public Dictionary<int, int> ExpandedCategorySlot { get; set; } = new();
    public List<KInventoryItemInfo> InventorySlotInfo { get; set; } = new();
    public int ExceptionProcessItemId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(NumResurrectionStone)) return false;
        if (!ks.PutMap(ExpandedCategorySlot, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.PutList(InventorySlotInfo)) return false;
        if (!ks.Put(ExceptionProcessItemId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out int v1)) return false; NumResurrectionStone = v1;
        if (!ks.GetMap(ExpandedCategorySlot, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false;
        if (!ks.GetList(InventorySlotInfo)) return false;
        if (!ks.Get(out int v4)) return false; ExceptionProcessItemId = v4;
        return true;
    }
}

public sealed class KEGS_BOSS_FIELD_RETURN_PROCESS_NOT : KBossFieldJoinInfo { }

public sealed class KEGS_BREAK_PERSONAL_SHOP_ACK : KPacketOK { }

public sealed class KEGS_BREAK_TRADE_ACK : KPacketOK { }

public sealed class KEGS_BREAK_TRADE_NOT : KPacketReason { }

public sealed class KEGS_BREAK_TRADE_REQ : KPacketReason { }

public sealed class KEGS_BREAK_UP_ACK : KPacketOK { }

public sealed class KEGS_BUY_PERSONAL_SHOP_ITEM_ACK : KPacketOK { }

public sealed class KEGS_CALL_MY_LOVER_ACK : KPacketOK { }

public sealed class KEGS_CALL_MY_LOVER_JOIN_BATTLE_FIELD_FAILD_NOT : KPacketOK { }

public sealed class KEGS_CANCEL_AUTO_PARTY_MAKING_ACK : KPacketOK { }

public sealed class KEGS_CANCEL_MATCH_MAKING_ACK : KPacketOK { }

public sealed class KEGS_CASH_ITEM_REFUND_ACK : KPacketOK { }

public sealed class KEGS_CHANGE_DUNGEON_DIFFICULTY_ACK : KPacketOK { }

public sealed class KEGS_CHANGE_GUILD_MESSAGE_ACK : KPacketOK { }

public sealed class KEGS_CHANGE_GUILD_NAME_CHECK_ACK : KPacketOK { }

public sealed class KEGS_CHANGE_LOVE_WORD_ACK : KPacketOK { }

public sealed class KEGS_CHANGE_PARTY_TYPE_ACK : KPacketOK { }

public sealed class KEGS_CHANGE_PARTY_TYPE_REQ : IKSerializable
{
    // enum PARTY_TYPE
    public const int PT_NONE = 0;
    public const int PT_DUNGEON_PARTY = 1;
    public const int PT_PVP_PARTY = 2;

    public long PartyUid { get; set; }
    public sbyte PartyType { get; set; }
    public string PartyName { get; set; } = string.Empty;
    public sbyte JoinSlot { get; set; }
    public sbyte MaxSlot { get; set; }
    public bool Public { get; set; }
    public int DungeonId { get; set; }
    public sbyte DifficultyLevel { get; set; }
    public sbyte GetItemType { get; set; }
    public sbyte DungeonMode { get; set; }
    public short PartyFever { get; set; }
    public bool CheckLowLevel { get; set; }
    public sbyte PVPGameType { get; set; }
    public bool ComeBackBuff { get; set; }
    public string WsrtComeBackBuffEnd { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PartyUid)) return false;
        if (!ks.Put(PartyType)) return false;
        if (!ks.PutWideString(PartyName)) return false;
        if (!ks.Put(JoinSlot)) return false;
        if (!ks.Put(MaxSlot)) return false;
        if (!ks.Put(Public)) return false;
        if (!ks.Put(DungeonId)) return false;
        if (!ks.Put(DifficultyLevel)) return false;
        if (!ks.Put(GetItemType)) return false;
        if (!ks.Put(DungeonMode)) return false;
        if (!ks.Put(PartyFever)) return false;
        if (!ks.Put(CheckLowLevel)) return false;
        if (!ks.Put(PVPGameType)) return false;
        if (!ks.Put(ComeBackBuff)) return false;
        if (!ks.PutWideString(WsrtComeBackBuffEnd)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; PartyUid = v0;
        if (!ks.Get(out sbyte v1)) return false; PartyType = v1;
        if (!ks.GetWideString(out string s2)) return false; PartyName = s2;
        if (!ks.Get(out sbyte v3)) return false; JoinSlot = v3;
        if (!ks.Get(out sbyte v4)) return false; MaxSlot = v4;
        if (!ks.Get(out bool v5)) return false; Public = v5;
        if (!ks.Get(out int v6)) return false; DungeonId = v6;
        if (!ks.Get(out sbyte v7)) return false; DifficultyLevel = v7;
        if (!ks.Get(out sbyte v8)) return false; GetItemType = v8;
        if (!ks.Get(out sbyte v9)) return false; DungeonMode = v9;
        if (!ks.Get(out short v10)) return false; PartyFever = v10;
        if (!ks.Get(out bool v11)) return false; CheckLowLevel = v11;
        if (!ks.Get(out sbyte v12)) return false; PVPGameType = v12;
        if (!ks.Get(out bool v13)) return false; ComeBackBuff = v13;
        if (!ks.GetWideString(out string s14)) return false; WsrtComeBackBuffEnd = s14;
        return true;
    }
}

public sealed class KEGS_CHANGE_PITIN_ACK : KPacketOK { }

public sealed class KEGS_CHANGE_PLAY_TIME_LIMIT_ACK : KPacketOK { }

public sealed class KEGS_CHANGE_SECOND_SECURITY_PW_ACK : KPacketOK { }

public sealed class KEGS_CHAT_ACK : KPacketOK { }

public sealed class KEGS_CHECK_CHANNEL_CHANGE_ACK : KPacketOK { }

public sealed class KEGS_CHECK_SECOND_SECURITY_PW_CHRACTER_LIST_ACK : KPacketOK { }

public sealed class KEGS_COMMANDS_FOR_PETS_ACK : KPacketOK { }

public sealed class KEGS_COMPARE_SECOND_SECURITY_PW_ACK : KPacketOK { }

public sealed class KEGS_CREATE_PSHOP_AGENCY_ACK : IKSerializable
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

public sealed class KEGS_CREATE_SECOND_SECURITY_ACK : KPacketOK { }

public sealed class KEGS_DELETE_APPLY_JOIN_GUILD_ACK : KPacketOK { }

public sealed class KEGS_DELETE_SECOND_SECURITY_ACK : KPacketOK { }

public sealed class KEGS_DELETE_SECOND_SECURITY_REQ : IKSerializable
{
    public string SecondPW { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutWideString(SecondPW)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string s0)) return false; SecondPW = s0;
        return true;
    }
}

public sealed class KEGS_DELETE_SECOND_SECURITY_VERIFY_ACK : KPacketOK { }

public sealed class KEGS_DELETE_TEMP_ITEM_REQ : IKSerializable
{
    public int TempItemUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(TempItemUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; TempItemUid = v0;
        return true;
    }
}

public sealed class KEGS_DEL_BLACKLIST_USER_ACK : IKSerializable
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

public sealed class KEGS_DEL_POST_BLACK_LIST_ACK : IKSerializable
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

public sealed class KEGS_DEL_POST_BLACK_LIST_REQ : IKSerializable
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

public sealed class KEGS_DISBAND_GUILD_ACK : KPacketOK { }

public sealed class KEGS_DIVORCE_NOT : IKSerializable
{
    public int Cost { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Cost)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Cost = v0;
        return true;
    }
}

public sealed class KEGS_DUNGEON_REQUIRE_ITEM_MESSAGE_NOT : KPacketOK { }

public sealed class KEGS_DUNGEON_STAGE_LOAD_ACK : KPacketOK { }

public sealed class KEGS_DUNGEON_SUB_STAGE_GO_NEXT_ACK : KPacketOK { }

public sealed class KEGS_DUNGEON_SUB_STAGE_LOAD_COMPLETE_ACK : KPacketOK { }

public sealed class KEGS_DUNGEON_SUB_STAGE_OPEN_ACK : KPacketOK { }

public sealed class KEGS_EVENT_PROPOSE_ACK : IKSerializable
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

public sealed class KEGS_EVENT_PROPOSE_AGREE_NOT : IKSerializable
{
    // enum COUPLE_RESULT_TYPE
    public const int CRT_YES = 0;
    public const int CRT_NO = 1;
    public const int CRT_NOT_ANSWER = 2;

    public sbyte Answer { get; set; }
    public long RequestUnitUid { get; set; }
    public string RequestUnitName { get; set; } = string.Empty;
    public long AcceptUserUid { get; set; }
    public byte AcceptUnitLevel { get; set; }
    public sbyte AcceptUnitClass { get; set; }
    public long AcceptUnitUid { get; set; }
    public string AcceptUnitName { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Answer)) return false;
        if (!ks.Put(RequestUnitUid)) return false;
        if (!ks.PutWideString(RequestUnitName)) return false;
        if (!ks.Put(AcceptUserUid)) return false;
        if (!ks.Put(AcceptUnitLevel)) return false;
        if (!ks.Put(AcceptUnitClass)) return false;
        if (!ks.Put(AcceptUnitUid)) return false;
        if (!ks.PutWideString(AcceptUnitName)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte v0)) return false; Answer = v0;
        if (!ks.Get(out long v1)) return false; RequestUnitUid = v1;
        if (!ks.GetWideString(out string s2)) return false; RequestUnitName = s2;
        if (!ks.Get(out long v3)) return false; AcceptUserUid = v3;
        if (!ks.Get(out byte v4)) return false; AcceptUnitLevel = v4;
        if (!ks.Get(out sbyte v5)) return false; AcceptUnitClass = v5;
        if (!ks.Get(out long v6)) return false; AcceptUnitUid = v6;
        if (!ks.GetWideString(out string s7)) return false; AcceptUnitName = s7;
        return true;
    }
}

public sealed class KEGS_EVENT_PROPOSE_NOT : IKSerializable
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

public sealed class KEGS_EVENT_PROPOSE_RESULT_NOT : IKSerializable
{
    public int Ok { get; set; }
    public sbyte Result { get; set; }
    public long UnitUid { get; set; }
    public string UnitName { get; set; } = string.Empty;
    public long Date { get; set; }
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(Result)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutWideString(UnitName)) return false;
        if (!ks.Put(Date)) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out sbyte v1)) return false; Result = v1;
        if (!ks.Get(out long v2)) return false; UnitUid = v2;
        if (!ks.GetWideString(out string s3)) return false; UnitName = s3;
        if (!ks.Get(out long v4)) return false; Date = v4;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        return true;
    }
}

public sealed class KEGS_FIELD_LOADING_COMPLETE_ACK : IKSerializable
{
    public int Ok { get; set; }
    public bool PopupTheGuideLine { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
        => ks.Put(Ok) && ks.Put(PopupTheGuideLine);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out bool v1)) return false; PopupTheGuideLine = v1;
        return true;
    }
}

public sealed class KEGS_GAME_LOADING_ACK : KPacketOK { }

public sealed class KEGS_GET_GUILD_SKILL_ACK : KPacketOK { }

public sealed class KEGS_GET_ITEM_ACK : KPacketOK { }

public sealed class KEGS_GET_ITEM_FROM_LETTER_REQ : IKSerializable
{
    public long PostNo { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PostNo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; PostNo = v0;
        return true;
    }
}

public sealed class KEGS_GET_NEXT_INVENTORY_ED_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int Ed { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(Ed)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out int v1)) return false; Ed = v1;
        return true;
    }
}

public sealed class KEGS_GET_PURCHASED_PACKAGE_CASH_ITEM_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int NumResurrectionStone { get; set; }
    public Dictionary<int, int> ExpandedCategorySlot { get; set; } = new();
    public List<KInventoryItemInfo> InventorySlotInfo { get; set; } = new();
    public int ExceptionProcessItemId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(NumResurrectionStone)) return false;
        if (!ks.PutMap(ExpandedCategorySlot, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.PutList(InventorySlotInfo)) return false;
        if (!ks.Put(ExceptionProcessItemId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out int v1)) return false; NumResurrectionStone = v1;
        if (!ks.GetMap(ExpandedCategorySlot, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false;
        if (!ks.GetList(InventorySlotInfo)) return false;
        if (!ks.Get(out int v4)) return false; ExceptionProcessItemId = v4;
        return true;
    }
}

public sealed class KEGS_GET_SECOND_SECURITY_INFO_ACK : IKSerializable
{
    public int Ok { get; set; }
    public bool UseSecondPW { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(UseSecondPW)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out bool v1)) return false; UseSecondPW = v1;
        return true;
    }
}

public sealed class KEGS_INVITE_GUILD_REPLY_ACK : KPacketOK { }

public sealed class KEGS_JOIN_BATTLE_FIELD_NOT : IKSerializable
{
    public KRoomSlotInfo JoinSlot { get; set; } = new();
    public int StartPosIndex { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(JoinSlot)) return false;
        if (!ks.Put(StartPosIndex)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(JoinSlot)) return false;
        if (!ks.Get(out int v1)) return false; StartPosIndex = v1;
        return true;
    }
}

public sealed class KEGS_JOIN_FIELD_CHANNEL_CHANGE_ACK : KPacketOK { }

public sealed class KEGS_JOIN_GUILD_SUCCESS_NOT : IKSerializable
{
    public KGuildInfo GuildInfo { get; set; } = new();
    public string GuildMasterName { get; set; } = string.Empty;
    public List<long> MemberUnitUid { get; set; } = new();
    public KGuildSkillInfo GuildSkillInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(GuildInfo)) return false;
        if (!ks.PutWideString(GuildMasterName)) return false;
        if (!ks.PutList(MemberUnitUid, static (s, v) => s.Put(v))) return false;
        if (!ks.Put(GuildSkillInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(GuildInfo)) return false;
        if (!ks.GetWideString(out string s1)) return false; GuildMasterName = s1;
        if (!ks.GetList(MemberUnitUid, static s => (s.Get(out long v), v))) return false;
        if (!ks.Get(GuildSkillInfo)) return false;
        return true;
    }
}

public sealed class KEGS_JOIN_WEDDING_HALL_NOT : IKSerializable
{
    public KRoomSlotInfo JoinSlot { get; set; } = new();
    public int StartPosIndex { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(JoinSlot)) return false;
        if (!ks.Put(StartPosIndex)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(JoinSlot)) return false;
        if (!ks.Get(out int v1)) return false; StartPosIndex = v1;
        return true;
    }
}

public sealed class KEGS_KNM_INVITE_ACK : KPacketOK { }

public sealed class KEGS_KNM_INVITE_RESULT_NOT : KPacketOK { }

public sealed class KEGS_LEAVE_PARTY_ACK : KPacketOK { }

public sealed class KEGS_LEAVE_PERSONAL_SHOP_ACK : KPacketOK { }

public sealed class KEGS_LEAVE_PSHOP_AGENCY_ACK : KPacketOK { }

public sealed class KEGS_LEAVE_TC_ROOM_ACK : KEGS_LEAVE_ROOM_ACK { }

public sealed class KEGS_LEAVE_WEDDING_HALL_ACK : KPacketOK { }

public sealed class KEGS_LEAVE_WEDDING_HALL_REQ : KPacketReason { }

public sealed class KEGS_MODIFY_REG_GUILD_AD_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int Ed { get; set; }
    public short Period { get; set; }
    public string AdMessage { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(Ed)) return false;
        if (!ks.Put(Period)) return false;
        if (!ks.PutWideString(AdMessage)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out int v1)) return false; Ed = v1;
        if (!ks.Get(out short v2)) return false; Period = v2;
        if (!ks.GetWideString(out string s3)) return false; AdMessage = s3;
        return true;
    }
}

public sealed class KEGS_MODIFY_REG_GUILD_AD_REQ : IKSerializable
{
    public short Period { get; set; }
    public string AdMessage { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Period)) return false;
        if (!ks.PutWideString(AdMessage)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out short v0)) return false; Period = v0;
        if (!ks.GetWideString(out string s1)) return false; AdMessage = s1;
        return true;
    }
}

public sealed class KEGS_NEW_QUEST_ACK : KPacketOK { }

public sealed class KEGS_NOTIFY_MSG_NOT : IKSerializable
{
    // enum NOTIFY_TYPE
    public const int NT_GM_NOTIFY_ALL_SERVERS = 0;
    public const int NT_GM_NOTIFY_SERVER_GROUP = 1;
    public const int NT_RANDOM_GET_ITEM = 2;
    public const int NT_DROP_GET_ITEM = 3;
    public const int NT_MAX_LEVEL_UP = 4;
    public const int NT_ENCHANT_SUCCESS = 5;
    public const int NT_HENIR_CLEAR = 6;
    public const int NT_GUILD_CREATION = 7;
    public const int NT_GUILD_LEVEL_UP = 8;
    public const int NT_RANDOM_GET_ITEM_ALL_NOTIFY = 9;
    public const int NT_SHUT_DOWN_ALL_NOTIFY = 10;
    public const int NT_GUILD_NAME_CHANGED = 11;
    public const int NT_THE_GATE_OF_DARKNESS = 12;
    public const int NT_WEDDING_SUCCESS = 13;

    public sbyte NotifyType { get; set; }
    public sbyte Count { get; set; }
    public string MSG { get; set; } = string.Empty;
    public int RandomItem { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(NotifyType)) return false;
        if (!ks.Put(Count)) return false;
        if (!ks.PutWideString(MSG)) return false;
        if (!ks.Put(RandomItem)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte v0)) return false; NotifyType = v0;
        if (!ks.Get(out sbyte v1)) return false; Count = v1;
        if (!ks.GetWideString(out string s2)) return false; MSG = s2;
        if (!ks.Get(out int v3)) return false; RandomItem = v3;
        return true;
    }
}

public sealed class KEGS_NPROTECT_CSAUTH3_GAMEHACK_REPORT_NOT : IKSerializable
{
    public List<byte> DeqCSAuth3 { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.CN || region.Region == RegionCode.JP) && !ks.PutList(DeqCSAuth3, static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.CN || region.Region == RegionCode.JP)) { if (!ks.GetList(DeqCSAuth3, static s => (s.Get(out byte v), v))) return false; }
        return true;
    }
}

public sealed class KEGS_NPROTECT_CSAUTH3_RESPONE_NOT : IKSerializable
{
    public List<byte> DeqCSAuth3 { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.CN || region.Region == RegionCode.JP) && !ks.PutList(DeqCSAuth3, static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.CN || region.Region == RegionCode.JP)) { if (!ks.GetList(DeqCSAuth3, static s => (s.Get(out byte v), v))) return false; }
        return true;
    }
}

public sealed class KEGS_OFFER_TRADE_ACK : KPacketOK { }

public sealed class KEGS_OPTION_UPDATE_ACK : KPacketOK { }

public sealed class KEGS_PARTY_BAN_USER_ACK : KPacketOK { }

public sealed class KEGS_PARTY_CHANGE_DUNGEON_ACK : KPacketOK { }

public sealed class KEGS_PARTY_CHANGE_DUNGEON_NOT : IKSerializable
{
    public int DungeonId { get; set; }
    public sbyte DifficultyLevel { get; set; }
    public sbyte DungeonMode { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(DungeonId)) return false;
        if (!ks.Put(DifficultyLevel)) return false;
        if (!ks.Put(DungeonMode)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; DungeonId = v0;
        if (!ks.Get(out sbyte v1)) return false; DifficultyLevel = v1;
        if (!ks.Get(out sbyte v2)) return false; DungeonMode = v2;
        return true;
    }
}

public sealed class KEGS_PARTY_CHANGE_GET_ITEM_TYPE_ACK : KPacketOK { }

public sealed class KEGS_PARTY_CHANGE_HOST_ACK : KPacketOK { }

public sealed class KEGS_PARTY_CHANGE_NUM_OF_PER_ACK : KPacketOK { }

public sealed class KEGS_PARTY_CHANGE_READY_ACK : KPacketOK { }

public sealed class KEGS_PARTY_GAME_START_ACK : KPacketOK { }

public sealed class KEGS_PARTY_LIMIT_LEVEL_ACK : KPacketOK { }

public sealed class KEGS_PRESENT_CASH_ITEM_ACK : IKSerializable
{
    public int Ok { get; set; }
    public List<KNXBTPurchaseReqInfo> PurchaseReqInfo { get; set; } = new();
    public bool IsWishList { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.PutList(PurchaseReqInfo)) return false;
        if (!ks.Put(IsWishList)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.GetList(PurchaseReqInfo)) return false;
        if (!ks.Get(out bool v2)) return false; IsWishList = v2;
        return true;
    }
}

public sealed class KEGS_PVP_NPC_UNIT_INFO_TO_SERVER_ACK : KPacketOK { }

public sealed class KEGS_PVP_NPC_UNIT_RE_BIRTH_POS_ACK : KPacketOK { }

public sealed class KEGS_PVP_PARTY_CHANGE_MATCH_INFO_ACK : KPacketOK { }

public sealed class KEGS_PVP_PARTY_CHANGE_MATCH_INFO_NOT : IKSerializable
{
    public sbyte PvpGameType { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PvpGameType)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte v0)) return false; PvpGameType = v0;
        return true;
    }
}

public sealed class KEGS_REG_PSHOP_AGENCY_ITEM_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int Ed { get; set; }
    public string PersonalShopName { get; set; } = string.Empty;
    public List<KSellPersonalShopItemInfo> SellItemInfo { get; set; } = new();
    public List<KInventoryItemInfo> InventorySlotInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(Ed)) return false;
        if (!ks.PutWideString(PersonalShopName)) return false;
        if (!ks.PutList(SellItemInfo)) return false;
        if (!ks.PutList(InventorySlotInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out int v1)) return false; Ed = v1;
        if (!ks.GetWideString(out string s2)) return false; PersonalShopName = s2;
        if (!ks.GetList(SellItemInfo)) return false;
        if (!ks.GetList(InventorySlotInfo)) return false;
        return true;
    }
}

public sealed class KEGS_REG_PSHOP_AGENCY_ITEM_REQ : IKSerializable
{
    public string PersonalShopName { get; set; } = string.Empty;
    public List<KRegPersonalShopItemInfo> RegItem { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutWideString(PersonalShopName)) return false;
        if (!ks.PutList(RegItem)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string s0)) return false; PersonalShopName = s0;
        if (!ks.GetList(RegItem)) return false;
        return true;
    }
}

public sealed class KEGS_RENEWAL_LETTER_ACK : IKSerializable
{
    public int NewUserLetterCount { get; set; }
    public int NewSystemLetterCount { get; set; }
    public int NewRelationshipLetterCount { get; set; }
    public int TotalLetterCount { get; set; }
    public List<KPostSmallTitleInfo> NewLetterTitle { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(NewUserLetterCount)) return false;
        if (!ks.Put(NewSystemLetterCount)) return false;
        if (!ks.Put(NewRelationshipLetterCount)) return false;
        if (!ks.Put(TotalLetterCount)) return false;
        if (!ks.PutList(NewLetterTitle)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; NewUserLetterCount = v0;
        if (!ks.Get(out int v1)) return false; NewSystemLetterCount = v1;
        if (!ks.Get(out int v2)) return false; NewRelationshipLetterCount = v2;
        if (!ks.Get(out int v3)) return false; TotalLetterCount = v3;
        if (!ks.GetList(NewLetterTitle)) return false;
        return true;
    }
}

public sealed class KEGS_REPAIR_ITEM_NOT : KEGS_CHANGE_EQUIPPED_ITEM_IN_ROOM_NOT { }

public sealed class KEGS_REQUEST_MATCH_MAKING_ACK : KPacketOK { }

public sealed class KEGS_SHOW_DISCONNECT_REASON_NOT : KPacketOK { }

public sealed class KEGS_START_WEDDING_ACK : KPacketOK { }

public sealed class KEGS_START_WEDDING_AGREE_NOT : IKSerializable
{
    public bool Agree { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Agree)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool v0)) return false; Agree = v0;
        return true;
    }
}

public sealed class KEGS_STATE_CHANGE_GAME_START_ACK : KPacketOK { }

public sealed class KEGS_STOP_SALE_PSHOP_AGENCY_ACK : KPacketOK { }

public sealed class KEGS_UDP_KICK_STATE_CHANGE_FIELD_NOT : IKSerializable
{
    public long AccusationUnitUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(AccusationUnitUid);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; AccusationUnitUid = v0;
        return true;
    }
}

public sealed class KEGS_BEFORE_SHUT_DOWN_MESSAGE_NOT : IKSerializable
{
    public int NetErrorId { get; set; }
    public string Time { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
        => ks.Put(NetErrorId) && ks.PutWideString(Time);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; NetErrorId = v0;
        if (!ks.GetWideString(out string s1)) return false; Time = s1;
        return true;
    }
}

public sealed class KEGS_SHUT_DOWN_MESSAGE_NOT : IKSerializable
{
    public int NetErrorId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(NetErrorId);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; NetErrorId = v0;
        return true;
    }
}

public sealed class KEGS_PVP_EVENT_INFO_NOT : IKSerializable
{
    public bool EventInfo { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(EventInfo);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool v0)) return false; EventInfo = v0;
        return true;
    }
}

public sealed class KEGS_TOONILAND_USER_ID_NOT : IKSerializable
{
    public string TooniLandId { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region) => ks.PutWideString(TooniLandId);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string s0)) return false; TooniLandId = s0;
        return true;
    }
}

public sealed class KEGS_UPDATE_EVENT_MONEY_NOT : IKSerializable
{
    public int EventMoney { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(EventMoney);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; EventMoney = v0;
        return true;
    }
}

public sealed class KEGS_SERVER_CHECK_HACK_USER_NOT : IKSerializable
{
    public sbyte HackingCheckType { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(HackingCheckType);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte v0)) return false; HackingCheckType = v0;
        return true;
    }
}

public sealed class KEGS_UNSEAL_ITEM_ACK : IKSerializable
{
    public int Ok { get; set; }
    public List<KInventoryItemInfo> InventorySlotInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.PutList(InventorySlotInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.GetList(InventorySlotInfo)) return false;
        return true;
    }
}

public sealed class KEGS_UPDATE_PERSONAL_SHOP_ITEM_INFO_NOT : KPersonalShopItemInfo { }

public sealed class KEGS_UPDATE_TRADE_ED_ACK : KPacketOK { }

public sealed class KEGS_UPDATE_TRADE_ITEM_ACK : KPacketOK { }

public sealed class KEGS_USER_UNIT_DIE_COMPLETE_ACK : KPacketOK { }

public sealed class KEGS_VERIFY_ACCOUNT_REQ : IKSerializable
{
    public bool DebugAuth { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Passwd { get; set; } = string.Empty;
    public string HWId { get; set; } = string.Empty;
    public string MachineId { get; set; } = string.Empty;
    public int ChannelingCode { get; set; }
    public bool ManualLogin { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(DebugAuth)) return false;
        if (!ks.PutWideString(UserId)) return false;
        if (!ks.PutWideString(Passwd)) return false;
        if (!ks.PutWideString(HWId)) return false;
        if (!ks.PutNarrowString(MachineId)) return false;
        if (!ks.Put(ChannelingCode)) return false;
        if (!ks.Put(ManualLogin)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool v0)) return false; DebugAuth = v0;
        if (!ks.GetWideString(out string s1)) return false; UserId = s1;
        if (!ks.GetWideString(out string s2)) return false; Passwd = s2;
        if (!ks.GetWideString(out string s3)) return false; HWId = s3;
        if (!ks.GetNarrowString(out string s4)) return false; MachineId = s4;
        if (!ks.Get(out int v5)) return false; ChannelingCode = v5;
        if (!ks.Get(out bool v6)) return false; ManualLogin = v6;
        return true;
    }
}

public sealed class KEGS_CHARGE_POINT_NOT : IKSerializable
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

public sealed class KEGS_CHECK_THE_ARCHUANGEL_S_COIN_EVENT_LETTER_ACK : IKSerializable
{
    public int Ok { get; set; }
    public bool ReceiveTheLetter { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.Put(ReceiveTheLetter);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out bool v1)) return false; ReceiveTheLetter = v1;
        return true;
    }
}
