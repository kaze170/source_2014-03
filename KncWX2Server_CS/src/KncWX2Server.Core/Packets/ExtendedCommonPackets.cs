using KncWX2Server.Core.Region;
using KncWX2Server.Core.Serialization;

namespace KncWX2Server.Core.Packets;

public sealed class K4ThAnnivEventInfo : IKSerializable
{
    public long TimeFirstPlay { get; set; }
    public long TimeFirstPet { get; set; }
    public long TimeFirstHenir { get; set; }
    public long TimeFirstDeleteChar { get; set; }
    public int ItemIDFirstBuy { get; set; }
    public int LongestConnectTime { get; set; }
    public int CountQuestComplete { get; set; }
    public int CountReceivedPost { get; set; }
    public int DayTotalConnect { get; set; }
    public int CountPvpLose { get; set; }
    public int CountResurrect { get; set; }
    public int CountDungeonClear { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.JP) && !ks.Put(TimeFirstPlay)) return false;
        if ((region.Region == RegionCode.JP) && !ks.Put(TimeFirstPet)) return false;
        if ((region.Region == RegionCode.JP) && !ks.Put(TimeFirstHenir)) return false;
        if ((region.Region == RegionCode.JP) && !ks.Put(TimeFirstDeleteChar)) return false;
        if ((region.Region == RegionCode.JP) && !ks.Put(ItemIDFirstBuy)) return false;
        if ((region.Region == RegionCode.JP) && !ks.Put(LongestConnectTime)) return false;
        if ((region.Region == RegionCode.JP) && !ks.Put(CountQuestComplete)) return false;
        if ((region.Region == RegionCode.JP) && !ks.Put(CountReceivedPost)) return false;
        if ((region.Region == RegionCode.JP) && !ks.Put(DayTotalConnect)) return false;
        if ((region.Region == RegionCode.JP) && !ks.Put(CountPvpLose)) return false;
        if ((region.Region == RegionCode.JP) && !ks.Put(CountResurrect)) return false;
        if ((region.Region == RegionCode.JP) && !ks.Put(CountDungeonClear)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.JP)) { if (!ks.Get(out long v0)) return false; TimeFirstPlay = v0; }
        if ((region.Region == RegionCode.JP)) { if (!ks.Get(out long v1)) return false; TimeFirstPet = v1; }
        if ((region.Region == RegionCode.JP)) { if (!ks.Get(out long v2)) return false; TimeFirstHenir = v2; }
        if ((region.Region == RegionCode.JP)) { if (!ks.Get(out long v3)) return false; TimeFirstDeleteChar = v3; }
        if ((region.Region == RegionCode.JP)) { if (!ks.Get(out int v4)) return false; ItemIDFirstBuy = v4; }
        if ((region.Region == RegionCode.JP)) { if (!ks.Get(out int v5)) return false; LongestConnectTime = v5; }
        if ((region.Region == RegionCode.JP)) { if (!ks.Get(out int v6)) return false; CountQuestComplete = v6; }
        if ((region.Region == RegionCode.JP)) { if (!ks.Get(out int v7)) return false; CountReceivedPost = v7; }
        if ((region.Region == RegionCode.JP)) { if (!ks.Get(out int v8)) return false; DayTotalConnect = v8; }
        if ((region.Region == RegionCode.JP)) { if (!ks.Get(out int v9)) return false; CountPvpLose = v9; }
        if ((region.Region == RegionCode.JP)) { if (!ks.Get(out int v10)) return false; CountResurrect = v10; }
        if ((region.Region == RegionCode.JP)) { if (!ks.Get(out int v11)) return false; CountDungeonClear = v11; }
        return true;
    }
}

public sealed class KAbuserEventLog : IKSerializable
{
    public long UserUid { get; set; }
    public long UnitUid { get; set; }
    public int Ed { get; set; }
    public int GetItemCount { get; set; }
    public int Spirit { get; set; }
    public long RegDate { get; set; }
    public ushort EventId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UserUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(Ed)) return false;
        if (!ks.Put(GetItemCount)) return false;
        if (!ks.Put(Spirit)) return false;
        if (!ks.Put(RegDate)) return false;
        if (!ks.Put(EventId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UserUid = v0;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        if (!ks.Get(out int v2)) return false; Ed = v2;
        if (!ks.Get(out int v3)) return false; GetItemCount = v3;
        if (!ks.Get(out int v4)) return false; Spirit = v4;
        if (!ks.Get(out long v5)) return false; RegDate = v5;
        if (!ks.Get(out ushort v6)) return false; EventId = v6;
        return true;
    }
}

public sealed class KAbuserList : IKSerializable
{
    public int ReleaseTick { get; set; }
    public List<long> AbuserList { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ReleaseTick)) return false;
        if (!ks.PutList(AbuserList, static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; ReleaseTick = v0;
        if (!ks.GetList(AbuserList, static s => (s.Get(out long v), v))) return false;
        return true;
    }
}

public sealed class KApplyDeleteInfo : IKSerializable
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

public sealed class KAttribNpcUnitInfo : IKSerializable
{
    public KNPCUnitReq NPCUnitReq { get; set; } = new();
    public KAttribEnchantNpcInfo AttribEnchantNpcInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(NPCUnitReq)) return false;
        if (!ks.Put(AttribEnchantNpcInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(NPCUnitReq)) return false;
        if (!ks.Get(AttribEnchantNpcInfo)) return false;
        return true;
    }
}

public sealed class KAutoPartyConditionInfo : IKSerializable
{
    public int DungeonId { get; set; }
    public sbyte DifficultyLevel { get; set; }
    public sbyte GetItemType { get; set; }
    public sbyte DungeonMode { get; set; }
    public sbyte LimitPlayerCount { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(DungeonId)) return false;
        if (!ks.Put(DifficultyLevel)) return false;
        if (!ks.Put(GetItemType)) return false;
        if (!ks.Put(DungeonMode)) return false;
        if (!ks.Put(LimitPlayerCount)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; DungeonId = v0;
        if (!ks.Get(out sbyte v1)) return false; DifficultyLevel = v1;
        if (!ks.Get(out sbyte v2)) return false; GetItemType = v2;
        if (!ks.Get(out sbyte v3)) return false; DungeonMode = v3;
        if (!ks.Get(out sbyte v4)) return false; LimitPlayerCount = v4;
        return true;
    }
}

public sealed class KAutoPartyMakingProcessCondition : IKSerializable
{
    public int DungeonMode { get; set; }
    public int DungeonIDWithDif { get; set; }
    public int BeginLevel { get; set; }
    public int EndLevel { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(DungeonMode)) return false;
        if (!ks.Put(DungeonIDWithDif)) return false;
        if (!ks.Put(BeginLevel)) return false;
        if (!ks.Put(EndLevel)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; DungeonMode = v0;
        if (!ks.Get(out int v1)) return false; DungeonIDWithDif = v1;
        if (!ks.Get(out int v2)) return false; BeginLevel = v2;
        if (!ks.Get(out int v3)) return false; EndLevel = v3;
        return true;
    }
}

public sealed class KAutoPartyUserInfo : IKSerializable
{
    public long GsUid { get; set; }
    public long UnitUid { get; set; }
    public KOldPartyInfo OldPartyInfo { get; set; } = new();
    public string NickName { get; set; } = string.Empty;
    public sbyte UnitClass { get; set; }
    public byte Level { get; set; }
    public int AutoPartyWaitTime { get; set; }
    public bool AutoPartyBonus { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(GsUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(OldPartyInfo)) return false;
        if (!ks.PutWideString(NickName)) return false;
        if (!ks.Put(UnitClass)) return false;
        if (!ks.Put(Level)) return false;
        if (!ks.Put(AutoPartyWaitTime)) return false;
        if (!ks.Put(AutoPartyBonus)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; GsUid = v0;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        if (!ks.Get(OldPartyInfo)) return false;
        if (!ks.GetWideString(out string s3)) return false; NickName = s3;
        if (!ks.Get(out sbyte v4)) return false; UnitClass = v4;
        if (!ks.Get(out byte v5)) return false; Level = v5;
        if (!ks.Get(out int v6)) return false; AutoPartyWaitTime = v6;
        if (!ks.Get(out bool v7)) return false; AutoPartyBonus = v7;
        return true;
    }
}

public sealed class KBattleFieldRoomInfo : IKSerializable
{
    public int BattleFieldId { get; set; }
    public long RoomUid { get; set; }
    public sbyte MaxSlot { get; set; }
    public Dictionary<long, long> UnitUIDPartyUid { get; set; } = new();
    public Dictionary<long, HashSet<long>> PartyList { get; set; } = new();
    public byte BossFieldState { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(BattleFieldId)) return false;
        if (!ks.Put(RoomUid)) return false;
        if (!ks.Put(MaxSlot)) return false;
        if (!ks.PutMap(UnitUIDPartyUid, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.PutMap(PartyList, static (s, k) => s.Put(k), static (s, v) => s.PutList(v, static (s2, x) => s2.Put(x)))) return false;
        if (!ks.Put(BossFieldState)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; BattleFieldId = v0;
        if (!ks.Get(out long v1)) return false; RoomUid = v1;
        if (!ks.Get(out sbyte v2)) return false; MaxSlot = v2;
        if (!ks.GetMap(UnitUIDPartyUid, static s => (s.Get(out long v), v), static s => (s.Get(out long v), v))) return false;
        if (!ks.GetMap(PartyList, static s => (s.Get(out long v), v), static s => { var hs = new HashSet<long>(); return (s.GetHashSet(hs, static s2 => (s2.Get(out long x), x)), hs); })) return false;
        if (!ks.Get(out byte v5)) return false; BossFieldState = v5;
        return true;
    }
}

public sealed class KBillBuyInfo : IKSerializable
{
    public int ProductNo { get; set; }
    public int Point { get; set; }
    public int ProductId { get; set; }
    public ushort OrderQuantity { get; set; }
    public int Category { get; set; }
    public string Memo { get; set; } = string.Empty;
    public string OrderId { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ProductNo)) return false;
        if (!ks.Put(Point)) return false;
        if (!ks.Put(ProductId)) return false;
        if (!ks.Put(OrderQuantity)) return false;
        if (!ks.Put(Category)) return false;
        if (!ks.PutWideString(Memo)) return false;
        if (!ks.PutWideString(OrderId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; ProductNo = v0;
        if (!ks.Get(out int v1)) return false; Point = v1;
        if (!ks.Get(out int v2)) return false; ProductId = v2;
        if (!ks.Get(out ushort v3)) return false; OrderQuantity = v3;
        if (!ks.Get(out int v4)) return false; Category = v4;
        if (!ks.GetWideString(out string s5)) return false; Memo = s5;
        if (!ks.GetWideString(out string s6)) return false; OrderId = s6;
        return true;
    }
}

public sealed class KBillCouponItemInfo : IKSerializable
{
    public int ProductNO { get; set; }
    public int Count { get; set; }
    public long TranNo { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ProductNO)) return false;
        if (!ks.Put(Count)) return false;
        if (!ks.Put(TranNo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; ProductNO = v0;
        if (!ks.Get(out int v1)) return false; Count = v1;
        if (!ks.Get(out long v2)) return false; TranNo = v2;
        return true;
    }
}

public sealed class KBillProductAttribute : IKSerializable
{
    public int ProductNo { get; set; }
    public List<int> Attribute { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ProductNo)) return false;
        if (!ks.PutList(Attribute, static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; ProductNo = v0;
        if (!ks.GetList(Attribute, static s => (s.Get(out int v), v))) return false;
        return true;
    }
}

public sealed class KBillProductCategory : IKSerializable
{
    public sbyte CategoryNo { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public sbyte ParentCategoryNo { get; set; }
    public sbyte DisplayNo { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(CategoryNo)) return false;
        if (!ks.PutWideString(CategoryName)) return false;
        if (!ks.Put(ParentCategoryNo)) return false;
        if (!ks.Put(DisplayNo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte v0)) return false; CategoryNo = v0;
        if (!ks.GetWideString(out string s1)) return false; CategoryName = s1;
        if (!ks.Get(out sbyte v2)) return false; ParentCategoryNo = v2;
        if (!ks.Get(out sbyte v3)) return false; DisplayNo = v3;
        return true;
    }
}

public sealed class KBlockInfo : IKSerializable
{
    // enum BLOCK_TYPE
    public const int BT_TRADE_BLOCK = 0;
    public const int BT_ACCOUNT_BLOCK = 1;
    public const int BT_IP_BLOCK = 2;
    public const int BT_DLL_BLOCK = 3;
    public const int BT_MAX = 4;

    public long BlockUid { get; set; }
    public sbyte BlockType { get; set; }
    public long BlockTarget { get; set; }
    public string WstrBlockTarget { get; set; } = string.Empty;
    public string StartDate { get; set; } = string.Empty;
    public string EndDate { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(BlockUid)) return false;
        if (!ks.Put(BlockType)) return false;
        if (!ks.Put(BlockTarget)) return false;
        if (!ks.PutWideString(WstrBlockTarget)) return false;
        if (!ks.PutWideString(StartDate)) return false;
        if (!ks.PutWideString(EndDate)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; BlockUid = v0;
        if (!ks.Get(out sbyte v1)) return false; BlockType = v1;
        if (!ks.Get(out long v2)) return false; BlockTarget = v2;
        if (!ks.GetWideString(out string s3)) return false; WstrBlockTarget = s3;
        if (!ks.GetWideString(out string s4)) return false; StartDate = s4;
        if (!ks.GetWideString(out string s5)) return false; EndDate = s5;
        return true;
    }
}

public sealed class KBossFieldCreateInfo : IKSerializable
{
    public long FieldHoldingTime { get; set; }
    public long PortalOpenTime { get; set; }
    public int BossFieldId { get; set; }
    public int PortalMapId { get; set; }
    public bool BossField { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(FieldHoldingTime)) return false;
        if (!ks.Put(PortalOpenTime)) return false;
        if (!ks.Put(BossFieldId)) return false;
        if (!ks.Put(PortalMapId)) return false;
        if (!ks.Put(BossField)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; FieldHoldingTime = v0;
        if (!ks.Get(out long v1)) return false; PortalOpenTime = v1;
        if (!ks.Get(out int v2)) return false; BossFieldId = v2;
        if (!ks.Get(out int v3)) return false; PortalMapId = v3;
        if (!ks.Get(out bool v4)) return false; BossField = v4;
        return true;
    }
}

public class KBossFieldJoinInfo : IKSerializable
{
    public int ReturnMapId { get; set; }
    public byte LastTouchLineIndex { get; set; }
    public ushort LastPosValue { get; set; }
    public bool IgnoreLastTouch { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ReturnMapId)) return false;
        if (!ks.Put(LastTouchLineIndex)) return false;
        if (!ks.Put(LastPosValue)) return false;
        if (!ks.Put(IgnoreLastTouch)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; ReturnMapId = v0;
        if (!ks.Get(out byte v1)) return false; LastTouchLineIndex = v1;
        if (!ks.Get(out ushort v2)) return false; LastPosValue = v2;
        if (!ks.Get(out bool v3)) return false; IgnoreLastTouch = v3;
        return true;
    }
}

public sealed class KBossFieldLog : IKSerializable
{
    public int BossFieldId { get; set; }
    public int PortalMapId { get; set; }
    public int PlayTime { get; set; }
    public byte RemainUserCount { get; set; }
    public bool Succeed { get; set; }
    public string PortalOpenTime { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(BossFieldId)) return false;
        if (!ks.Put(PortalMapId)) return false;
        if (!ks.Put(PlayTime)) return false;
        if (!ks.Put(RemainUserCount)) return false;
        if (!ks.Put(Succeed)) return false;
        if (!ks.PutWideString(PortalOpenTime)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; BossFieldId = v0;
        if (!ks.Get(out int v1)) return false; PortalMapId = v1;
        if (!ks.Get(out int v2)) return false; PlayTime = v2;
        if (!ks.Get(out byte v3)) return false; RemainUserCount = v3;
        if (!ks.Get(out bool v4)) return false; Succeed = v4;
        if (!ks.GetWideString(out string s5)) return false; PortalOpenTime = s5;
        return true;
    }
}

public sealed class KBossFieldUserLog : IKSerializable
{
    public long UnitUid { get; set; }
    public long GivenDamage { get; set; }
    public long AttackDamage { get; set; }
    public int Exp { get; set; }
    public int Ed { get; set; }
    public short NumResurrectionStone { get; set; }
    public byte Level { get; set; }
    public byte Class { get; set; }
    public byte CompletionType { get; set; }
    public byte ContributionRank { get; set; }
    public string NickName { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(GivenDamage)) return false;
        if (!ks.Put(AttackDamage)) return false;
        if (!ks.Put(Exp)) return false;
        if (!ks.Put(Ed)) return false;
        if (!ks.Put(NumResurrectionStone)) return false;
        if (!ks.Put(Level)) return false;
        if (!ks.Put(Class)) return false;
        if (!ks.Put(CompletionType)) return false;
        if (!ks.Put(ContributionRank)) return false;
        if (!ks.PutWideString(NickName)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out long v1)) return false; GivenDamage = v1;
        if (!ks.Get(out long v2)) return false; AttackDamage = v2;
        if (!ks.Get(out int v3)) return false; Exp = v3;
        if (!ks.Get(out int v4)) return false; Ed = v4;
        if (!ks.Get(out short v5)) return false; NumResurrectionStone = v5;
        if (!ks.Get(out byte v6)) return false; Level = v6;
        if (!ks.Get(out byte v7)) return false; Class = v7;
        if (!ks.Get(out byte v8)) return false; CompletionType = v8;
        if (!ks.Get(out byte v9)) return false; ContributionRank = v9;
        if (!ks.GetWideString(out string s10)) return false; NickName = s10;
        return true;
    }
}

public sealed class KBuyCharInfo : IKSerializable
{
    public sbyte Level { get; set; }
    public int UnitClass { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Level)) return false;
        if (!ks.Put(UnitClass)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte v0)) return false; Level = v0;
        if (!ks.Get(out int v1)) return false; UnitClass = v1;
        return true;
    }
}

public sealed class KBuyGPItemInfo : IKSerializable
{
    public int ItemId { get; set; }
    public int Price { get; set; }
    public int PvPPoint { get; set; }
    public sbyte PeriodType { get; set; }
    public int Quantity { get; set; }
    public short Endurance { get; set; }
    public short Period { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ItemId)) return false;
        if (!ks.Put(Price)) return false;
        if (!ks.Put(PvPPoint)) return false;
        if (!ks.Put(PeriodType)) return false;
        if (!ks.Put(Quantity)) return false;
        if (!ks.Put(Endurance)) return false;
        if (!ks.Put(Period)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; ItemId = v0;
        if (!ks.Get(out int v1)) return false; Price = v1;
        if (!ks.Get(out int v2)) return false; PvPPoint = v2;
        if (!ks.Get(out sbyte v3)) return false; PeriodType = v3;
        if (!ks.Get(out int v4)) return false; Quantity = v4;
        if (!ks.Get(out short v5)) return false; Endurance = v5;
        if (!ks.Get(out short v6)) return false; Period = v6;
        return true;
    }
}

public sealed class KCashItemResolveInfo : IKSerializable
{
    public int ItemId { get; set; }
    public int SalePrice { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ItemId)) return false;
        if (!ks.Put(SalePrice)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; ItemId = v0;
        if (!ks.Get(out int v1)) return false; SalePrice = v1;
        return true;
    }
}

public sealed class KCashPruductInfo : IKSerializable
{
    public uint ProductNo { get; set; }
    public short ProductExpire { get; set; }
    public ushort ProductPieces { get; set; }
    public int ItemId { get; set; }
    public uint SalePrice { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ProductNo)) return false;
        if (!ks.Put(ProductExpire)) return false;
        if (!ks.Put(ProductPieces)) return false;
        if (!ks.Put(ItemId)) return false;
        if (!ks.Put(SalePrice)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint v0)) return false; ProductNo = v0;
        if (!ks.Get(out short v1)) return false; ProductExpire = v1;
        if (!ks.Get(out ushort v2)) return false; ProductPieces = v2;
        if (!ks.Get(out int v3)) return false; ItemId = v3;
        if (!ks.Get(out uint v4)) return false; SalePrice = v4;
        return true;
    }
}

public class KChangeGuildNameACK : IKSerializable
{
    public int Ok { get; set; }
    public long UnitUid { get; set; }
    public KGuildInfo GuildInfo { get; set; } = new();
    public string OldGuildName { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(GuildInfo)) return false;
        if (!ks.PutWideString(OldGuildName)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        if (!ks.Get(GuildInfo)) return false;
        if (!ks.GetWideString(out string s3)) return false; OldGuildName = s3;
        return true;
    }
}

public class KChangeGuildNameCheckREQ : IKSerializable
{
    public long UnitUid { get; set; }
    public string NewGuildName { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutWideString(NewGuildName)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.GetWideString(out string s1)) return false; NewGuildName = s1;
        return true;
    }
}

public class KChangeGuildNameREQ : IKSerializable
{
    public long UnitUid { get; set; }
    public int GuildUid { get; set; }
    public string OldGuildName { get; set; } = string.Empty;
    public string NewGuildName { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(GuildUid)) return false;
        if (!ks.PutWideString(OldGuildName)) return false;
        if (!ks.PutWideString(NewGuildName)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out int v1)) return false; GuildUid = v1;
        if (!ks.GetWideString(out string s2)) return false; OldGuildName = s2;
        if (!ks.GetWideString(out string s3)) return false; NewGuildName = s3;
        return true;
    }
}

public sealed class KChannelSimpleInfo : IKSerializable
{
    public int ChannelId { get; set; }
    public int MaxUser { get; set; }
    public int CurrentUser { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ChannelId)) return false;
        if (!ks.Put(MaxUser)) return false;
        if (!ks.Put(CurrentUser)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; ChannelId = v0;
        if (!ks.Get(out int v1)) return false; MaxUser = v1;
        if (!ks.Get(out int v2)) return false; CurrentUser = v2;
        return true;
    }
}

public sealed class KConnectTimeEventInfo : IKSerializable
{
    public int EventUid { get; set; }
    public string EventTime { get; set; } = string.Empty;
    public bool AccountEvent { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(EventUid)) return false;
        if (!ks.PutWideString(EventTime)) return false;
        if (!ks.Put(AccountEvent)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; EventUid = v0;
        if (!ks.GetWideString(out string s1)) return false; EventTime = s1;
        if (!ks.Get(out bool v2)) return false; AccountEvent = v2;
        return true;
    }
}

public class KCreateGuildACK : IKSerializable
{
    public int Ok { get; set; }
    public long UnitUid { get; set; }
    public long ItemUid { get; set; }
    public KGuildInfo CreatedGuildInfo { get; set; } = new();
    public KGuildSkillInfo GuildSkillInfo { get; set; } = new();
    public bool ReqGiant_DeleteName { get; set; }
    public string GuildName { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(ItemUid)) return false;
        if (!ks.Put(CreatedGuildInfo)) return false;
        if (!ks.Put(GuildSkillInfo)) return false;
        if ((region.Region == RegionCode.CN) && !ks.Put(ReqGiant_DeleteName)) return false;
        if ((region.Region == RegionCode.CN) && !ks.PutWideString(GuildName)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        if (!ks.Get(out long v2)) return false; ItemUid = v2;
        if (!ks.Get(CreatedGuildInfo)) return false;
        if (!ks.Get(GuildSkillInfo)) return false;
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out bool v5)) return false; ReqGiant_DeleteName = v5; }
        if ((region.Region == RegionCode.CN)) { if (!ks.GetWideString(out string s6)) return false; GuildName = s6; }
        return true;
    }
}

public class KCreateGuildREQ : IKSerializable
{
    public long UnitUid { get; set; }
    public long ItemUid { get; set; }
    public string GuildName { get; set; } = string.Empty;
    public string GuildMessage { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(ItemUid)) return false;
        if (!ks.PutWideString(GuildName)) return false;
        if (!ks.PutWideString(GuildMessage)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out long v1)) return false; ItemUid = v1;
        if (!ks.GetWideString(out string s2)) return false; GuildName = s2;
        if (!ks.GetWideString(out string s3)) return false; GuildMessage = s3;
        return true;
    }
}

public sealed class KCumulativeTimeEventInfo : IKSerializable
{
    public int EventUid { get; set; }
    public int CumulativeTime { get; set; }
    public bool AccountEvent { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(EventUid)) return false;
        if (!ks.Put(CumulativeTime)) return false;
        if (!ks.Put(AccountEvent)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; EventUid = v0;
        if (!ks.Get(out int v1)) return false; CumulativeTime = v1;
        if (!ks.Get(out bool v2)) return false; AccountEvent = v2;
        return true;
    }
}

public sealed class KDBConnectionInfo : IKSerializable
{
    public int DBType { get; set; }
    public string ConnectionString { get; set; } = string.Empty;
    public int ThreadCount { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(DBType)) return false;
        if (!ks.PutWideString(ConnectionString)) return false;
        if (!ks.Put(ThreadCount)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; DBType = v0;
        if (!ks.GetWideString(out string s1)) return false; ConnectionString = s1;
        if (!ks.Get(out int v2)) return false; ThreadCount = v2;
        return true;
    }
}

public sealed class KDBE_CENTER_SERVER_ON_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KServerInfo ServerInfo { get; set; } = new();
    public int ProxyId { get; set; }
    public List<KSquareCreateInfo> SquareList { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(ServerInfo)) return false;
        if (!ks.Put(ProxyId)) return false;
        if (!ks.PutList(SquareList)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(ServerInfo)) return false;
        if (!ks.Get(out int v2)) return false; ProxyId = v2;
        if (!ks.GetList(SquareList)) return false;
        return true;
    }
}

public sealed class KDBE_GAME_SERVER_ON_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KServerInfo ServerInfo { get; set; } = new();
    public Dictionary<int, KNetAddress> CenterList { get; set; } = new();
    public Dictionary<int, KNetAddress> LoginList { get; set; } = new();
    public int ChannelId { get; set; }
    public KChannelBonusInfo BonusInfo { get; set; } = new();
    public Dictionary<int, KNetAddress> GlobalList { get; set; } = new();
    public Dictionary<string, int> PCBangIPList { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(ServerInfo)) return false;
        if (!ks.PutMap(CenterList, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.PutMap(LoginList, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.Put(ChannelId)) return false;
        if (!ks.Put(BonusInfo)) return false;
        if (!ks.PutMap(GlobalList, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if ((region.Region == RegionCode.CN) && !ks.PutMap(PCBangIPList, static (s, k) => s.PutWideString(k), static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(ServerInfo)) return false;
        if (!ks.GetMap(CenterList, static s => (s.Get(out int v), v), static s => { var v = new KNetAddress(); return (s.Get(v), v); })) return false;
        if (!ks.GetMap(LoginList, static s => (s.Get(out int v), v), static s => { var v = new KNetAddress(); return (s.Get(v), v); })) return false;
        if (!ks.Get(out int v4)) return false; ChannelId = v4;
        if (!ks.Get(BonusInfo)) return false;
        if (!ks.GetMap(GlobalList, static s => (s.Get(out int v), v), static s => { var v = new KNetAddress(); return (s.Get(v), v); })) return false;
        if ((region.Region == RegionCode.CN)) { if (!ks.GetMap(PCBangIPList, static s => (s.GetWideString(out string v), v), static s => (s.Get(out int v), v))) return false; }
        return true;
    }
}

public sealed class KDBE_LOGIN_SERVER_ON_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int ProxyId { get; set; }
    public KServerInfo ServerInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(ProxyId)) return false;
        if (!ks.Put(ServerInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out int v1)) return false; ProxyId = v1;
        if (!ks.Get(ServerInfo)) return false;
        return true;
    }
}

public sealed class KDBE_PURCHASED_CASH_ITEM_LIST_REQ : IKSerializable
{
    public int ItemPerPage { get; set; }
    public int CurrentPage { get; set; }
    public string UnitNickName { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ItemPerPage)) return false;
        if (!ks.Put(CurrentPage)) return false;
        if (!ks.PutWideString(UnitNickName)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; ItemPerPage = v0;
        if (!ks.Get(out int v1)) return false; CurrentPage = v1;
        if (!ks.GetWideString(out string s2)) return false; UnitNickName = s2;
        return true;
    }
}

public sealed class KDBUpdateGamePlayNetworkInfo : IKSerializable
{
    public int GameType { get; set; }
    public int HolePunchingTryCount { get; set; }
    public int P2PPlayTime { get; set; }
    public int RelayPlayTime { get; set; }
    public string RelayServerIP { get; set; } = string.Empty;
    public string RegDate { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(GameType)) return false;
        if (!ks.Put(HolePunchingTryCount)) return false;
        if (!ks.Put(P2PPlayTime)) return false;
        if (!ks.Put(RelayPlayTime)) return false;
        if (!ks.PutWideString(RelayServerIP)) return false;
        if (!ks.PutWideString(RegDate)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; GameType = v0;
        if (!ks.Get(out int v1)) return false; HolePunchingTryCount = v1;
        if (!ks.Get(out int v2)) return false; P2PPlayTime = v2;
        if (!ks.Get(out int v3)) return false; RelayPlayTime = v3;
        if (!ks.GetWideString(out string s4)) return false; RelayServerIP = s4;
        if (!ks.GetWideString(out string s5)) return false; RegDate = s5;
        return true;
    }
}

public sealed class KDangerousEventInfo : IKSerializable
{
    // enum DANGEROUS_EVENT
    public const int DE_WARNING_MESSAGE = 0;
    public const int DE_ELITE_MONSTER_DROP = 1;
    public const int DE_BOSS_MONSTER_DROP = 2;
    public const int DE_MIDDLE_BOSS_MONSTER_DROP = 3;
    public const int DE_EVENT_BOSS_MONSTER_DROP = 4;

    public HashSet<int> EventInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutList(EventInfo, static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetHashSet(EventInfo, static s => (s.Get(out int v), v))) return false;
        return true;
    }
}

public sealed class KDecreaseEnduranceInfo : IKSerializable
{
    public long ItemUid { get; set; }
    public int Endurance { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ItemUid)) return false;
        if (!ks.Put(Endurance)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; ItemUid = v0;
        if (!ks.Get(out int v1)) return false; Endurance = v1;
        return true;
    }
}

public sealed class KDefenseOpenTime : IKSerializable
{
    public int StartHour { get; set; }
    public int StartMinute { get; set; }
    public int DurationTime { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(StartHour)) return false;
        if (!ks.Put(StartMinute)) return false;
        if (!ks.Put(DurationTime)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; StartHour = v0;
        if (!ks.Get(out int v1)) return false; StartMinute = v1;
        if (!ks.Get(out int v2)) return false; DurationTime = v2;
        return true;
    }
}

public sealed class KDungeonDropEventNpcInfo : IKSerializable
{
    public int EventUid { get; set; }
    public int NpcId { get; set; }
    public long DropDate { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(EventUid)) return false;
        if (!ks.Put(NpcId)) return false;
        if (!ks.Put(DropDate)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; EventUid = v0;
        if (!ks.Get(out int v1)) return false; NpcId = v1;
        if (!ks.Get(out long v2)) return false; DropDate = v2;
        return true;
    }
}

public class KDungeonGameInfo : IKSerializable
{
    public int DungeonId { get; set; }
    public sbyte DungeonMode { get; set; }
    public sbyte Difficulty { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(DungeonId)) return false;
        if (!ks.Put(DungeonMode)) return false;
        if (!ks.Put(Difficulty)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; DungeonId = v0;
        if (!ks.Get(out sbyte v1)) return false; DungeonMode = v1;
        if (!ks.Get(out sbyte v2)) return false; Difficulty = v2;
        return true;
    }
}

public sealed class KDungeonNextStageData : IKSerializable
{
    public string StageId { get; set; } = string.Empty;
    public int SubStageIndex { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutWideString(StageId)) return false;
        if (!ks.Put(SubStageIndex)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string s0)) return false; StageId = s0;
        if (!ks.Get(out int v1)) return false; SubStageIndex = v1;
        return true;
    }
}

public class KENX_COMMON_ACK_NOT : IKSerializable
{
    public byte PacketType { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UnitNickName { get; set; } = string.Empty;
    public byte Result { get; set; }
    public byte AddressDesc { get; set; }
    public byte AccountDesc { get; set; }
    public byte Option { get; set; }
    public uint Argument { get; set; }
    public byte IsGameBangBillType { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PacketType)) return false;
        if (!ks.PutWideString(UserId)) return false;
        if (!ks.PutWideString(UnitNickName)) return false;
        if (!ks.Put(Result)) return false;
        if (!ks.Put(AddressDesc)) return false;
        if (!ks.Put(AccountDesc)) return false;
        if (!ks.Put(Option)) return false;
        if (!ks.Put(Argument)) return false;
        if (!ks.Put(IsGameBangBillType)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out byte v0)) return false; PacketType = v0;
        if (!ks.GetWideString(out string s1)) return false; UserId = s1;
        if (!ks.GetWideString(out string s2)) return false; UnitNickName = s2;
        if (!ks.Get(out byte v3)) return false; Result = v3;
        if (!ks.Get(out byte v4)) return false; AddressDesc = v4;
        if (!ks.Get(out byte v5)) return false; AccountDesc = v5;
        if (!ks.Get(out byte v6)) return false; Option = v6;
        if (!ks.Get(out uint v7)) return false; Argument = v7;
        if (!ks.Get(out byte v8)) return false; IsGameBangBillType = v8;
        return true;
    }
}

public class KENX_COMMON_REQ : IKSerializable
{
    public byte PacketType { get; set; }
    public byte Login { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UnitNickName { get; set; } = string.Empty;
    public uint IP { get; set; }
    public byte CanTry { get; set; }
    public byte GuestUser { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PacketType)) return false;
        if (!ks.Put(Login)) return false;
        if (!ks.PutWideString(UserId)) return false;
        if (!ks.PutWideString(UnitNickName)) return false;
        if (!ks.Put(IP)) return false;
        if (!ks.Put(CanTry)) return false;
        if (!ks.Put(GuestUser)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out byte v0)) return false; PacketType = v0;
        if (!ks.Get(out byte v1)) return false; Login = v1;
        if (!ks.GetWideString(out string s2)) return false; UserId = s2;
        if (!ks.GetWideString(out string s3)) return false; UnitNickName = s3;
        if (!ks.Get(out uint v4)) return false; IP = v4;
        if (!ks.Get(out byte v5)) return false; CanTry = v5;
        if (!ks.Get(out byte v6)) return false; GuestUser = v6;
        return true;
    }
}

public sealed class KEventInfo : IKSerializable
{
    public int EventUid { get; set; }
    public int EventType { get; set; }
    public string EventName { get; set; } = string.Empty;
    public string BeginDate { get; set; } = string.Empty;
    public string EndDate { get; set; } = string.Empty;
    public int ScriptId { get; set; }
    public bool Enable { get; set; }
    public bool On { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(EventUid)) return false;
        if (!ks.Put(EventType)) return false;
        if (!ks.PutWideString(EventName)) return false;
        if (!ks.PutWideString(BeginDate)) return false;
        if (!ks.PutWideString(EndDate)) return false;
        if (!ks.Put(ScriptId)) return false;
        if (!ks.Put(Enable)) return false;
        if (!ks.Put(On)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; EventUid = v0;
        if (!ks.Get(out int v1)) return false; EventType = v1;
        if (!ks.GetWideString(out string s2)) return false; EventName = s2;
        if (!ks.GetWideString(out string s3)) return false; BeginDate = s3;
        if (!ks.GetWideString(out string s4)) return false; EndDate = s4;
        if (!ks.Get(out int v5)) return false; ScriptId = v5;
        if (!ks.Get(out bool v6)) return false; Enable = v6;
        if (!ks.Get(out bool v7)) return false; On = v7;
        return true;
    }
}

public sealed class KEventQuestInfo : IKSerializable
{
    public bool Init { get; set; }
    public int EventUid { get; set; }
    public int QuestId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Init)) return false;
        if (!ks.Put(EventUid)) return false;
        if (!ks.Put(QuestId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool v0)) return false; Init = v0;
        if (!ks.Get(out int v1)) return false; EventUid = v1;
        if (!ks.Get(out int v2)) return false; QuestId = v2;
        return true;
    }
}

public sealed class KExchangeLimitInfo : IKSerializable
{
    public int LimitUid { get; set; }
    public long BeginDate { get; set; }
    public long EndDate { get; set; }
    public HashSet<int> DestItemId { get; set; } = new();
    public int LimitQuantity { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(LimitUid)) return false;
        if (!ks.Put(BeginDate)) return false;
        if (!ks.Put(EndDate)) return false;
        if (!ks.PutList(DestItemId, static (s, v) => s.Put(v))) return false;
        if (!ks.Put(LimitQuantity)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; LimitUid = v0;
        if (!ks.Get(out long v1)) return false; BeginDate = v1;
        if (!ks.Get(out long v2)) return false; EndDate = v2;
        if (!ks.GetHashSet(DestItemId, static s => (s.Get(out int v), v))) return false;
        if (!ks.Get(out int v4)) return false; LimitQuantity = v4;
        return true;
    }
}

public sealed class KFieldBossGenKillStat : IKSerializable
{
    public int FieldId { get; set; }
    public int NumUser { get; set; }
    public int SpawnId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(FieldId)) return false;
        if (!ks.Put(NumUser)) return false;
        if (!ks.Put(SpawnId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; FieldId = v0;
        if (!ks.Get(out int v1)) return false; NumUser = v1;
        if (!ks.Get(out int v2)) return false; SpawnId = v2;
        return true;
    }
}

public sealed class KFieldCoordinates : IKSerializable
{
    public short PosX { get; set; }
    public short PosY { get; set; }
    public short PosZ { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PosX)) return false;
        if (!ks.Put(PosY)) return false;
        if (!ks.Put(PosZ)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out short v0)) return false; PosX = v0;
        if (!ks.Get(out short v1)) return false; PosY = v1;
        if (!ks.Get(out short v2)) return false; PosZ = v2;
        return true;
    }
}

public sealed class KFriendMessageInfo : IKSerializable
{
    // enum MESSAGE_TYPE
    public const int MT_REQUEST_FRIEND = 0;

    public long UnitUid { get; set; }
    public string NickName { get; set; } = string.Empty;
    public sbyte MessageType { get; set; }
    public string Message { get; set; } = string.Empty;
    public string RegDate { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutWideString(NickName)) return false;
        if (!ks.Put(MessageType)) return false;
        if (!ks.PutWideString(Message)) return false;
        if (!ks.PutWideString(RegDate)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.GetWideString(out string s1)) return false; NickName = s1;
        if (!ks.Get(out sbyte v2)) return false; MessageType = v2;
        if (!ks.GetWideString(out string s3)) return false; Message = s3;
        if (!ks.GetWideString(out string s4)) return false; RegDate = s4;
        return true;
    }
}

public sealed class KGamePlayStatusContainer : IKSerializable
{
    public List<KGamePlayStatus> GamePlayStatus { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutList(GamePlayStatus)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetList(GamePlayStatus)) return false;
        return true;
    }
}

public class KGashResponseData : IKSerializable
{
    // enum GASH_RESPONSE
    public const int GR_RESULT_CODE = 0;
    public const int GR_OUTSTRING = 1;
    public const int GR_PACKETID = 2;
    public const int GR_NUM = 3;

    public string ResultCode { get; set; } = string.Empty;
    public string OutString { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string PacketId { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.TW) && !ks.PutNarrowString(ResultCode)) return false;
        if ((region.Region == RegionCode.TW) && !ks.PutNarrowString(OutString)) return false;
        if ((region.Region == RegionCode.TW) && !ks.PutNarrowString(AccountId)) return false;
        if ((region.Region == RegionCode.TW) && !ks.PutNarrowString(PacketId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.TW)) { if (!ks.GetNarrowString(out string s0)) return false; ResultCode = s0; }
        if ((region.Region == RegionCode.TW)) { if (!ks.GetNarrowString(out string s1)) return false; OutString = s1; }
        if ((region.Region == RegionCode.TW)) { if (!ks.GetNarrowString(out string s2)) return false; AccountId = s2; }
        if ((region.Region == RegionCode.TW)) { if (!ks.GetNarrowString(out string s3)) return false; PacketId = s3; }
        return true;
    }
}

public sealed class KGiantAccountType : IKSerializable
{
    // enum TYPE_MASK
    public const int NORMAL_USER = 0;
    public const int SPECIAL_USER = 2;
    public const int VIP_USER = 4;
    public const int EVENT_PLAYER = 8;
    public const int ADULT_PLAYER = 16;
    public const int BLACKLIST_USER = 32;
    public const int DEALER = 512;
    public const int WEB_DEALER = 1024;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        return true;
    }
}

public sealed class KGiantDeletedUnitInfo : IKSerializable
{
    public string NickName { get; set; } = string.Empty;
    public long UnitUid { get; set; }
    public uint GiantUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutWideString(NickName)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(GiantUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string s0)) return false; NickName = s0;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        if (!ks.Get(out uint v2)) return false; GiantUid = v2;
        return true;
    }
}

public sealed class KGlobalEventTableData : IKSerializable
{
    // enum EVENT_ACCOUNT_ENUM
    public const int EAE_NONE = 0;
    public const int EAE_ACCOUNT_NORMAL = 1;
    public const int EAE_ACCOUNT_SUM = 2;

    public long UserUid { get; set; }
    public long UnitUid { get; set; }
    public int EventId { get; set; }
    public ushort Category { get; set; }
    public List<int> VeciParamData { get; set; } = new();
    public int EventType { get; set; }
    public int EventScriptId { get; set; }
    public sbyte EventAccountType { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.JP) && !ks.Put(UserUid)) return false;
        if ((region.Region == RegionCode.JP) && !ks.Put(UnitUid)) return false;
        if ((region.Region == RegionCode.JP) && !ks.Put(EventId)) return false;
        if ((region.Region == RegionCode.JP) && !ks.Put(Category)) return false;
        if ((region.Region == RegionCode.JP) && !ks.PutList(VeciParamData, static (s, v) => s.Put(v))) return false;
        if ((region.Region == RegionCode.JP) && !ks.Put(EventType)) return false;
        if ((region.Region == RegionCode.JP) && !ks.Put(EventScriptId)) return false;
        if ((region.Region == RegionCode.JP) && !ks.Put(EventAccountType)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.JP)) { if (!ks.Get(out long v0)) return false; UserUid = v0; }
        if ((region.Region == RegionCode.JP)) { if (!ks.Get(out long v1)) return false; UnitUid = v1; }
        if ((region.Region == RegionCode.JP)) { if (!ks.Get(out int v2)) return false; EventId = v2; }
        if ((region.Region == RegionCode.JP)) { if (!ks.Get(out ushort v3)) return false; Category = v3; }
        if ((region.Region == RegionCode.JP)) { if (!ks.GetList(VeciParamData, static s => (s.Get(out int v), v))) return false; }
        if ((region.Region == RegionCode.JP)) { if (!ks.Get(out int v5)) return false; EventType = v5; }
        if ((region.Region == RegionCode.JP)) { if (!ks.Get(out int v6)) return false; EventScriptId = v6; }
        if ((region.Region == RegionCode.JP)) { if (!ks.Get(out sbyte v7)) return false; EventAccountType = v7; }
        return true;
    }
}

public sealed class KGlobalMissionInfo : IKSerializable
{
    public bool Active { get; set; }
    public string MissionStartTime { get; set; } = string.Empty;
    public int DurationMinute { get; set; }
    public int RemainSecond { get; set; }
    public int MissionType { get; set; }
    public int ClearCondition { get; set; }
    public int ClearCount { get; set; }
    public int NowCount { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Active)) return false;
        if (!ks.PutWideString(MissionStartTime)) return false;
        if (!ks.Put(DurationMinute)) return false;
        if (!ks.Put(RemainSecond)) return false;
        if (!ks.Put(MissionType)) return false;
        if (!ks.Put(ClearCondition)) return false;
        if (!ks.Put(ClearCount)) return false;
        if (!ks.Put(NowCount)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool v0)) return false; Active = v0;
        if (!ks.GetWideString(out string s1)) return false; MissionStartTime = s1;
        if (!ks.Get(out int v2)) return false; DurationMinute = v2;
        if (!ks.Get(out int v3)) return false; RemainSecond = v3;
        if (!ks.Get(out int v4)) return false; MissionType = v4;
        if (!ks.Get(out int v5)) return false; ClearCondition = v5;
        if (!ks.Get(out int v6)) return false; ClearCount = v6;
        if (!ks.Get(out int v7)) return false; NowCount = v7;
        return true;
    }
}

public sealed class KGlobalMissionReward : IKSerializable
{
    public int RewardType { get; set; }
    public int RewardDurationTime { get; set; }
    public int ScriptId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(RewardType)) return false;
        if (!ks.Put(RewardDurationTime)) return false;
        if (!ks.Put(ScriptId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; RewardType = v0;
        if (!ks.Get(out int v1)) return false; RewardDurationTime = v1;
        if (!ks.Get(out int v2)) return false; ScriptId = v2;
        return true;
    }
}

public sealed class KHenirHackUserData : IKSerializable
{
    public long UserUid { get; set; }
    public long UnitUid { get; set; }
    public int PlayTime { get; set; }
    public int MemberNum { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UserUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(PlayTime)) return false;
        if (!ks.Put(MemberNum)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UserUid = v0;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        if (!ks.Get(out int v2)) return false; PlayTime = v2;
        if (!ks.Get(out int v3)) return false; MemberNum = v3;
        return true;
    }
}

public sealed class KItemAbuserLogInfo : IKSerializable
{
    public int RewardState { get; set; }
    public int ItemId { get; set; }
    public int Quantity { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(RewardState)) return false;
        if (!ks.Put(ItemId)) return false;
        if (!ks.Put(Quantity)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; RewardState = v0;
        if (!ks.Get(out int v1)) return false; ItemId = v1;
        if (!ks.Get(out int v2)) return false; Quantity = v2;
        return true;
    }
}

public sealed class KItemPeriodInfo : IKSerializable
{
    public long ItemUid { get; set; }
    public short Period { get; set; }
    public string ExpirationDate { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ItemUid)) return false;
        if (!ks.Put(Period)) return false;
        if (!ks.PutWideString(ExpirationDate)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; ItemUid = v0;
        if (!ks.Get(out short v1)) return false; Period = v1;
        if (!ks.GetWideString(out string s2)) return false; ExpirationDate = s2;
        return true;
    }
}

public sealed class KItemPosition : IKSerializable
{
    // enum ITEM_POS_ENUM
    public const int IPE_INVALID_PET_UID = -1;

    public int SlotCategory { get; set; }
    public int SlotId { get; set; }
    public long PetUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(SlotCategory)) return false;
        if (!ks.Put(SlotId)) return false;
        if (!ks.Put(PetUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; SlotCategory = v0;
        if (!ks.Get(out int v1)) return false; SlotId = v1;
        if (!ks.Get(out long v2)) return false; PetUid = v2;
        return true;
    }
}

public sealed class KJPEventRewardInfo : IKSerializable
{
    public long RId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public long UserUid { get; set; }
    public long UnitUid { get; set; }
    public int RewardId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(RId)) return false;
        if (!ks.PutWideString(UserId)) return false;
        if (!ks.Put(UserUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(RewardId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; RId = v0;
        if (!ks.GetWideString(out string s1)) return false; UserId = s1;
        if (!ks.Get(out long v2)) return false; UserUid = v2;
        if (!ks.Get(out long v3)) return false; UnitUid = v3;
        if (!ks.Get(out int v4)) return false; RewardId = v4;
        return true;
    }
}

public sealed class KLogData : IKSerializable
{
    public byte LogLevel { get; set; }
    public string RegDate { get; set; } = string.Empty;
    public string FunctionName { get; set; } = string.Empty;
    public List<string> Content { get; set; } = new();
    public string FileName { get; set; } = string.Empty;
    public int FileLine { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(LogLevel)) return false;
        if (!ks.PutWideString(RegDate)) return false;
        if (!ks.PutWideString(FunctionName)) return false;
        if (!ks.PutList(Content, static (s, v) => s.PutWideString(v))) return false;
        if (!ks.PutWideString(FileName)) return false;
        if (!ks.Put(FileLine)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out byte v0)) return false; LogLevel = v0;
        if (!ks.GetWideString(out string s1)) return false; RegDate = s1;
        if (!ks.GetWideString(out string s2)) return false; FunctionName = s2;
        if (!ks.GetList(Content, static s => (s.GetWideString(out string v), v))) return false;
        if (!ks.GetWideString(out string s4)) return false; FileName = s4;
        if (!ks.Get(out int v5)) return false; FileLine = v5;
        return true;
    }
}

public sealed class KMapInfo : IKSerializable
{
    public short MinX { get; set; }
    public short MinY { get; set; }
    public short MinZ { get; set; }
    public short MaxX { get; set; }
    public short MaxY { get; set; }
    public short MaxZ { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(MinX)) return false;
        if (!ks.Put(MinY)) return false;
        if (!ks.Put(MinZ)) return false;
        if (!ks.Put(MaxX)) return false;
        if (!ks.Put(MaxY)) return false;
        if (!ks.Put(MaxZ)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out short v0)) return false; MinX = v0;
        if (!ks.Get(out short v1)) return false; MinY = v1;
        if (!ks.Get(out short v2)) return false; MinZ = v2;
        if (!ks.Get(out short v3)) return false; MaxX = v3;
        if (!ks.Get(out short v4)) return false; MaxY = v4;
        if (!ks.Get(out short v5)) return false; MaxZ = v5;
        return true;
    }
}

public sealed class KMatchUserInfo : IKSerializable
{
    public long GsUid { get; set; }
    public long UnitUid { get; set; }
    public long PartyUid { get; set; }
    public string NickName { get; set; } = string.Empty;
    public sbyte UnitClass { get; set; }
    public byte Level { get; set; }
    public sbyte CurrentRank { get; set; }
    public int OfficialMatchCnt { get; set; }
    public int Rating { get; set; }
    public int RPoint { get; set; }
    public int MatchWaitTime { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(GsUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(PartyUid)) return false;
        if (!ks.PutWideString(NickName)) return false;
        if (!ks.Put(UnitClass)) return false;
        if (!ks.Put(Level)) return false;
        if (!ks.Put(CurrentRank)) return false;
        if (!ks.Put(OfficialMatchCnt)) return false;
        if (!ks.Put(Rating)) return false;
        if (!ks.Put(RPoint)) return false;
        if (!ks.Put(MatchWaitTime)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; GsUid = v0;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        if (!ks.Get(out long v2)) return false; PartyUid = v2;
        if (!ks.GetWideString(out string s3)) return false; NickName = s3;
        if (!ks.Get(out sbyte v4)) return false; UnitClass = v4;
        if (!ks.Get(out byte v5)) return false; Level = v5;
        if (!ks.Get(out sbyte v6)) return false; CurrentRank = v6;
        if (!ks.Get(out int v7)) return false; OfficialMatchCnt = v7;
        if (!ks.Get(out int v8)) return false; Rating = v8;
        if (!ks.Get(out int v9)) return false; RPoint = v9;
        if (!ks.Get(out int v10)) return false; MatchWaitTime = v10;
        return true;
    }
}

public sealed class KMessengerInfo : IKSerializable
{
    public Dictionary<long, KFriendInfo> FriendInfo { get; set; } = new();
    public Dictionary<sbyte, string> Group { get; set; } = new();
    public List<KFriendMessageInfo> FriendMessageInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutMap(FriendInfo, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.PutMap(Group, static (s, k) => s.Put(k), static (s, v) => s.PutWideString(v))) return false;
        if (!ks.PutList(FriendMessageInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetMap(FriendInfo, static s => (s.Get(out long v), v), static s => { var v = new KFriendInfo(); return (s.Get(v), v); })) return false;
        if (!ks.GetMap(Group, static s => (s.Get(out sbyte v), v), static s => (s.GetWideString(out string v), v))) return false;
        if (!ks.GetList(FriendMessageInfo)) return false;
        return true;
    }
}

public sealed class KMissionInstance : IKSerializable
{
    public int Id { get; set; }
    public List<KSubMissionInstance> SubMissionInstance { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Id)) return false;
        if (!ks.PutList(SubMissionInstance)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Id = v0;
        if (!ks.GetList(SubMissionInstance)) return false;
        return true;
    }
}

public sealed class KMissionUpdate : IKSerializable
{
    public int TitleId { get; set; }
    public List<short> ClearData { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(TitleId)) return false;
        if (!ks.PutList(ClearData, static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; TitleId = v0;
        if (!ks.GetList(ClearData, static s => (s.Get(out short v), v))) return false;
        return true;
    }
}

public sealed class KModuleInfo : IKSerializable
{
    public int ModuleUid { get; set; }
    public string ModuleName { get; set; } = string.Empty;
    public long Count { get; set; }
    public int Flag { get; set; }
    public bool Allow { get; set; }
    public bool ReserveDBUpdate { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ModuleUid)) return false;
        if (!ks.PutWideString(ModuleName)) return false;
        if (!ks.Put(Count)) return false;
        if (!ks.Put(Flag)) return false;
        if (!ks.Put(Allow)) return false;
        if (!ks.Put(ReserveDBUpdate)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; ModuleUid = v0;
        if (!ks.GetWideString(out string s1)) return false; ModuleName = s1;
        if (!ks.Get(out long v2)) return false; Count = v2;
        if (!ks.Get(out int v3)) return false; Flag = v3;
        if (!ks.Get(out bool v4)) return false; Allow = v4;
        if (!ks.Get(out bool v5)) return false; ReserveDBUpdate = v5;
        return true;
    }
}

public sealed class KNXBTBonusProductInfoPickUp : IKSerializable
{
    public uint BonusProduct { get; set; }
    public uint BonusAmount { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(BonusProduct)) return false;
        if (!ks.Put(BonusAmount)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint v0)) return false; BonusProduct = v0;
        if (!ks.Get(out uint v1)) return false; BonusAmount = v1;
        return true;
    }
}

public sealed class KNXBTCategoryInfo : IKSerializable
{
    public uint CategoryNo { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public uint ParentCategoryNo { get; set; }
    public uint DisplayNo { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(CategoryNo)) return false;
        if (!ks.PutWideString(CategoryName)) return false;
        if (!ks.Put(ParentCategoryNo)) return false;
        if (!ks.Put(DisplayNo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint v0)) return false; CategoryNo = v0;
        if (!ks.GetWideString(out string s1)) return false; CategoryName = s1;
        if (!ks.Get(out uint v2)) return false; ParentCategoryNo = v2;
        if (!ks.Get(out uint v3)) return false; DisplayNo = v3;
        return true;
    }
}

public sealed class KNXBTCouponProductInfo : IKSerializable
{
    public uint ProductNo { get; set; }
    public string ExtendValue { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ProductNo)) return false;
        if (!ks.PutWideString(ExtendValue)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint v0)) return false; ProductNo = v0;
        if (!ks.GetWideString(out string s1)) return false; ExtendValue = s1;
        return true;
    }
}

public sealed class KNXBTLotteryInfo : IKSerializable
{
    public uint OrderNo { get; set; }
    public uint LotteryProductNo { get; set; }
    public uint ProductNo { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public short ProductExpire { get; set; }
    public ushort ProductPieces { get; set; }
    public string ProductAttribute0 { get; set; } = string.Empty;
    public string ProductAttribute1 { get; set; } = string.Empty;
    public string ProductAttribute2 { get; set; } = string.Empty;
    public string ProductAttribute3 { get; set; } = string.Empty;
    public string ProductAttribute4 { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(OrderNo)) return false;
        if (!ks.Put(LotteryProductNo)) return false;
        if (!ks.Put(ProductNo)) return false;
        if (!ks.PutWideString(ProductName)) return false;
        if (!ks.PutWideString(ProductId)) return false;
        if (!ks.Put(ProductExpire)) return false;
        if (!ks.Put(ProductPieces)) return false;
        if (!ks.PutWideString(ProductAttribute0)) return false;
        if (!ks.PutWideString(ProductAttribute1)) return false;
        if (!ks.PutWideString(ProductAttribute2)) return false;
        if (!ks.PutWideString(ProductAttribute3)) return false;
        if (!ks.PutWideString(ProductAttribute4)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint v0)) return false; OrderNo = v0;
        if (!ks.Get(out uint v1)) return false; LotteryProductNo = v1;
        if (!ks.Get(out uint v2)) return false; ProductNo = v2;
        if (!ks.GetWideString(out string s3)) return false; ProductName = s3;
        if (!ks.GetWideString(out string s4)) return false; ProductId = s4;
        if (!ks.Get(out short v5)) return false; ProductExpire = v5;
        if (!ks.Get(out ushort v6)) return false; ProductPieces = v6;
        if (!ks.GetWideString(out string s7)) return false; ProductAttribute0 = s7;
        if (!ks.GetWideString(out string s8)) return false; ProductAttribute1 = s8;
        if (!ks.GetWideString(out string s9)) return false; ProductAttribute2 = s9;
        if (!ks.GetWideString(out string s10)) return false; ProductAttribute3 = s10;
        if (!ks.GetWideString(out string s11)) return false; ProductAttribute4 = s11;
        return true;
    }
}

public sealed class KNXBTPurchaseAckInfo : IKSerializable
{
    public uint ProductNo { get; set; }
    public ushort OrderQuantity { get; set; }
    public string ExtendValue { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ProductNo)) return false;
        if (!ks.Put(OrderQuantity)) return false;
        if (!ks.PutWideString(ExtendValue)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint v0)) return false; ProductNo = v0;
        if (!ks.Get(out ushort v1)) return false; OrderQuantity = v1;
        if (!ks.GetWideString(out string s2)) return false; ExtendValue = s2;
        return true;
    }
}

public sealed class KNXBTSubProductInfo : IKSerializable
{
    public uint ProductNo { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public short ProductExpire { get; set; }
    public ushort ProductPieces { get; set; }
    public string ProductAttribute0 { get; set; } = string.Empty;
    public string ProductAttribute1 { get; set; } = string.Empty;
    public string ProductAttribute2 { get; set; } = string.Empty;
    public string ProductAttribute3 { get; set; } = string.Empty;
    public string ProductAttribute4 { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ProductNo)) return false;
        if (!ks.PutWideString(ProductName)) return false;
        if (!ks.PutWideString(ProductId)) return false;
        if (!ks.Put(ProductExpire)) return false;
        if (!ks.Put(ProductPieces)) return false;
        if (!ks.PutWideString(ProductAttribute0)) return false;
        if (!ks.PutWideString(ProductAttribute1)) return false;
        if (!ks.PutWideString(ProductAttribute2)) return false;
        if (!ks.PutWideString(ProductAttribute3)) return false;
        if (!ks.PutWideString(ProductAttribute4)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint v0)) return false; ProductNo = v0;
        if (!ks.GetWideString(out string s1)) return false; ProductName = s1;
        if (!ks.GetWideString(out string s2)) return false; ProductId = s2;
        if (!ks.Get(out short v3)) return false; ProductExpire = v3;
        if (!ks.Get(out ushort v4)) return false; ProductPieces = v4;
        if (!ks.GetWideString(out string s5)) return false; ProductAttribute0 = s5;
        if (!ks.GetWideString(out string s6)) return false; ProductAttribute1 = s6;
        if (!ks.GetWideString(out string s7)) return false; ProductAttribute2 = s7;
        if (!ks.GetWideString(out string s8)) return false; ProductAttribute3 = s8;
        if (!ks.GetWideString(out string s9)) return false; ProductAttribute4 = s9;
        return true;
    }
}

public sealed class KNexonAccountInfo : IKSerializable
{
    // enum CHANNELING_ENUM
    public const int CE_NONE = -1;
    public const int CE_NEXON_ACCOUNT = 0;
    public const int CE_TOONILAND_ACCOUNT = 3;
    public const int CE_NAVER_ACCOUNT = 6;
    public const int CE_NAVER_SESSION = 14;
    // enum JAPAN_CHANNELING_ENUM
    public const int JCE_NONE = -1;
    public const int JCE_NHN = 0;
    public const int JCE_MSN = 1;
    public const int JCE_NICO = 2;
    public const int JCE_NEXON = 3;
    public const int JCE_MAX = 4;

    public string Id { get; set; } = string.Empty;
    public string LoginIP { get; set; } = string.Empty;
    public bool Sex { get; set; }
    public uint Age { get; set; }
    public uint NexonSN { get; set; }
    public uint PwdHash { get; set; }
    public uint SsnHash { get; set; }
    public uint Flag0 { get; set; }
    public uint Flag1 { get; set; }
    public bool NewMembership { get; set; }
    public byte MainAuthLevel { get; set; }
    public byte SubAuthLevel { get; set; }
    public string NationCode { get; set; } = string.Empty;
    public string Meta { get; set; } = string.Empty;
    public byte SecureCode { get; set; }
    public byte ChannelCode { get; set; }
    public string ChannelUid { get; set; } = string.Empty;
    public byte GuestUser { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutWideString(Id)) return false;
        if (!ks.Put(NexonSN)) return false;
        if (!ks.PutWideString(LoginIP)) return false;
        if (!ks.Put(Sex)) return false;
        if (!ks.Put(Age)) return false;
        if (!ks.Put(PwdHash)) return false;
        if (!ks.Put(SsnHash)) return false;
        if (!ks.Put(Flag0)) return false;
        if (!ks.Put(Flag1)) return false;
        if (!ks.Put(NewMembership)) return false;
        if (!ks.Put(MainAuthLevel)) return false;
        if (!ks.Put(SubAuthLevel)) return false;
        if (!ks.PutWideString(NationCode)) return false;
        if (!ks.PutWideString(Meta)) return false;
        if (!ks.Put(SecureCode)) return false;
        if (!ks.Put(ChannelCode)) return false;
        if (!ks.PutWideString(ChannelUid)) return false;
        if (!ks.Put(GuestUser)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string s0)) return false; Id = s0;
        if (!ks.Get(out uint v1)) return false; NexonSN = v1;
        if (!ks.GetWideString(out string s2)) return false; LoginIP = s2;
        if (!ks.Get(out bool v3)) return false; Sex = v3;
        if (!ks.Get(out uint v4)) return false; Age = v4;
        if (!ks.Get(out uint v5)) return false; PwdHash = v5;
        if (!ks.Get(out uint v6)) return false; SsnHash = v6;
        if (!ks.Get(out uint v7)) return false; Flag0 = v7;
        if (!ks.Get(out uint v8)) return false; Flag1 = v8;
        if (!ks.Get(out bool v9)) return false; NewMembership = v9;
        if (!ks.Get(out byte v10)) return false; MainAuthLevel = v10;
        if (!ks.Get(out byte v11)) return false; SubAuthLevel = v11;
        if (!ks.GetWideString(out string s12)) return false; NationCode = s12;
        if (!ks.GetWideString(out string s13)) return false; Meta = s13;
        if (!ks.Get(out byte v14)) return false; SecureCode = v14;
        if (!ks.Get(out byte v15)) return false; ChannelCode = v15;
        if (!ks.GetWideString(out string s16)) return false; ChannelUid = s16;
        if (!ks.Get(out byte v17)) return false; GuestUser = v17;
        return true;
    }
}

public sealed class KOldPartyInfo : IKSerializable
{
    public long OldPartyUid { get; set; }
    public long OldHostUnitUid { get; set; }
    public HashSet<long> OldPartyMember { get; set; } = new();
    public short OldPartyFever { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(OldPartyUid)) return false;
        if (!ks.Put(OldHostUnitUid)) return false;
        if (!ks.PutList(OldPartyMember, static (s, v) => s.Put(v))) return false;
        if (!ks.Put(OldPartyFever)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; OldPartyUid = v0;
        if (!ks.Get(out long v1)) return false; OldHostUnitUid = v1;
        if (!ks.GetHashSet(OldPartyMember, static s => (s.Get(out long v), v))) return false;
        if (!ks.Get(out short v3)) return false; OldPartyFever = v3;
        return true;
    }
}

public sealed class KOpenRandomItemStorage : IKSerializable
{
    public int RessurectionCount { get; set; }
    public int RestoreSpirit { get; set; }
    public int RandomItemId { get; set; }
    public bool IsCharmItem { get; set; }
    public int SpendED { get; set; }
    public Dictionary<int, int> InsertedItem { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();
    public List<KItemInfo> ItemInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(RessurectionCount)) return false;
        if (!ks.Put(RestoreSpirit)) return false;
        if (!ks.Put(RandomItemId)) return false;
        if (!ks.Put(IsCharmItem)) return false;
        if (!ks.Put(SpendED)) return false;
        if (!ks.PutMap(InsertedItem, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        if (!ks.PutList(ItemInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; RessurectionCount = v0;
        if (!ks.Get(out int v1)) return false; RestoreSpirit = v1;
        if (!ks.Get(out int v2)) return false; RandomItemId = v2;
        if (!ks.Get(out bool v3)) return false; IsCharmItem = v3;
        if (!ks.Get(out int v4)) return false; SpendED = v4;
        if (!ks.GetMap(InsertedItem, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        if (!ks.GetList(ItemInfo)) return false;
        return true;
    }
}

public sealed class KPVPUserSimpleInfo : IKSerializable
{
    public long UnitUid { get; set; }
    public sbyte State { get; set; }
    public int StateCode { get; set; }
    public long RoomUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(State)) return false;
        if (!ks.Put(StateCode)) return false;
        if (!ks.Put(RoomUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out sbyte v1)) return false; State = v1;
        if (!ks.Get(out int v2)) return false; StateCode = v2;
        if (!ks.Get(out long v3)) return false; RoomUid = v3;
        return true;
    }
}

public sealed class KPacketProcessingCountInfo : IKSerializable
{
    public ushort EventId { get; set; }
    public uint Count { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(EventId)) return false;
        if (!ks.Put(Count)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out ushort v0)) return false; EventId = v0;
        if (!ks.Get(out uint v1)) return false; Count = v1;
        return true;
    }
}

public sealed class KPacketProcessingLatencyInfo : IKSerializable
{
    public ushort EventId { get; set; }
    public long UnitUid { get; set; }
    public uint MaxLatency { get; set; }
    public string MaxLatencyTime { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(EventId)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(MaxLatency)) return false;
        if (!ks.PutWideString(MaxLatencyTime)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out ushort v0)) return false; EventId = v0;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        if (!ks.Get(out uint v2)) return false; MaxLatency = v2;
        if (!ks.GetWideString(out string s3)) return false; MaxLatencyTime = s3;
        return true;
    }
}

public class KPacketReason : IKSerializable
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

public sealed class KPartyMemberStatus : IKSerializable
{
    public float HpPercent { get; set; }
    public float MpPercent { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(HpPercent)) return false;
        if (!ks.Put(MpPercent)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out float v0)) return false; HpPercent = v0;
        if (!ks.Get(out float v1)) return false; MpPercent = v1;
        return true;
    }
}

public sealed class KPartyRoomUserInfo : IKSerializable
{
    public KRoomUserInfo RoomUserInfo { get; set; } = new();
    public List<long> StudentUnitUid { get; set; } = new();
    public KOldPartyInfo OldPartyInfo { get; set; } = new();
    public bool AutoPartyBonus { get; set; }
    public string ChannelIP { get; set; } = string.Empty;
    public int CurEXP { get; set; }
    public int CurED { get; set; }
    public int AutoPartyWaitTime { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(RoomUserInfo)) return false;
        if (!ks.PutList(StudentUnitUid, static (s, v) => s.Put(v))) return false;
        if (!ks.Put(OldPartyInfo)) return false;
        if (!ks.Put(AutoPartyBonus)) return false;
        if (!ks.PutWideString(ChannelIP)) return false;
        if (!ks.Put(CurEXP)) return false;
        if (!ks.Put(CurED)) return false;
        if (!ks.Put(AutoPartyWaitTime)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(RoomUserInfo)) return false;
        if (!ks.GetList(StudentUnitUid, static s => (s.Get(out long v), v))) return false;
        if (!ks.Get(OldPartyInfo)) return false;
        if (!ks.Get(out bool v3)) return false; AutoPartyBonus = v3;
        if (!ks.GetWideString(out string s4)) return false; ChannelIP = s4;
        if (!ks.Get(out int v5)) return false; CurEXP = v5;
        if (!ks.Get(out int v6)) return false; CurED = v6;
        if (!ks.Get(out int v7)) return false; AutoPartyWaitTime = v7;
        return true;
    }
}

public sealed class KPartyUserJoinRoomInfo : IKSerializable
{
    public long RoomUid { get; set; }
    public int BattleFieldId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(RoomUid)) return false;
        if (!ks.Put(BattleFieldId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; RoomUid = v0;
        if (!ks.Get(out int v1)) return false; BattleFieldId = v1;
        return true;
    }
}

public sealed class KPersonalShopAgencyInfo : IKSerializable
{
    public long HostUnitUid { get; set; }
    public long PShopAgencyUid { get; set; }
    public string AgencyOpenDate { get; set; } = string.Empty;
    public string AgencyExpirationDate { get; set; } = string.Empty;
    public bool OnSale { get; set; }
    public List<KSellPersonalShopItemInfo> SellPShopAgencyItem { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(HostUnitUid)) return false;
        if (!ks.Put(PShopAgencyUid)) return false;
        if (!ks.PutWideString(AgencyOpenDate)) return false;
        if (!ks.PutWideString(AgencyExpirationDate)) return false;
        if (!ks.Put(OnSale)) return false;
        if (!ks.PutList(SellPShopAgencyItem)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; HostUnitUid = v0;
        if (!ks.Get(out long v1)) return false; PShopAgencyUid = v1;
        if (!ks.GetWideString(out string s2)) return false; AgencyOpenDate = s2;
        if (!ks.GetWideString(out string s3)) return false; AgencyExpirationDate = s3;
        if (!ks.Get(out bool v4)) return false; OnSale = v4;
        if (!ks.GetList(SellPShopAgencyItem)) return false;
        return true;
    }
}

public class KPersonalShopItemInfo : IKSerializable
{
    public long ItemUid { get; set; }
    public int Quantity { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ItemUid)) return false;
        if (!ks.Put(Quantity)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; ItemUid = v0;
        if (!ks.Get(out int v1)) return false; Quantity = v1;
        return true;
    }
}

public sealed class KPersonalShopUserInfo : IKSerializable
{
    public long GsUid { get; set; }
    public long UserUid { get; set; }
    public long UnitUid { get; set; }
    public string NickName { get; set; } = string.Empty;
    public bool IsSquare { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(GsUid)) return false;
        if (!ks.Put(UserUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutWideString(NickName)) return false;
        if (!ks.Put(IsSquare)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; GsUid = v0;
        if (!ks.Get(out long v1)) return false; UserUid = v1;
        if (!ks.Get(out long v2)) return false; UnitUid = v2;
        if (!ks.GetWideString(out string s3)) return false; NickName = s3;
        if (!ks.Get(out bool v4)) return false; IsSquare = v4;
        return true;
    }
}

public sealed class KPropertyValue : IKSerializable
{
    public byte PropertyValue { get; set; }
    public uint UintPropertyValue { get; set; }
    public uint UlongPropertyValue { get; set; }
    public string WstrPropertyValue { get; set; } = string.Empty;
    public List<uint> VecPropertyValue { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PropertyValue)) return false;
        if (!ks.Put(UintPropertyValue)) return false;
        if (!ks.Put(UlongPropertyValue)) return false;
        if (!ks.PutWideString(WstrPropertyValue)) return false;
        if (!ks.PutList(VecPropertyValue, static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out byte v0)) return false; PropertyValue = v0;
        if (!ks.Get(out uint v1)) return false; UintPropertyValue = v1;
        if (!ks.Get(out uint v2)) return false; UlongPropertyValue = v2;
        if (!ks.GetWideString(out string s3)) return false; WstrPropertyValue = s3;
        if (!ks.GetList(VecPropertyValue, static s => (s.Get(out uint v), v))) return false;
        return true;
    }
}

public sealed class KPunishmentInfo : IKSerializable
{
    public int TypeVal { get; set; }
    public int StartTime { get; set; }
    public int Duration { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.CN) && !ks.Put(TypeVal)) return false;
        if ((region.Region == RegionCode.CN) && !ks.Put(StartTime)) return false;
        if ((region.Region == RegionCode.CN) && !ks.Put(Duration)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out int v0)) return false; TypeVal = v0; }
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out int v1)) return false; StartTime = v1; }
        if ((region.Region == RegionCode.CN)) { if (!ks.Get(out int v2)) return false; Duration = v2; }
        return true;
    }
}

public sealed class KPurchaseCoupon : IKSerializable
{
    public uint NexonSN { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string CouponSerial { get; set; } = string.Empty;
    public int GameType { get; set; }
    public int ServerDomain { get; set; }
    public string UnitNickName { get; set; } = string.Empty;
    public int Reason { get; set; }
    public string CafeNo { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(NexonSN)) return false;
        if (!ks.PutWideString(UserId)) return false;
        if (!ks.PutWideString(UserName)) return false;
        if (!ks.PutWideString(CouponSerial)) return false;
        if (!ks.Put(GameType)) return false;
        if (!ks.Put(ServerDomain)) return false;
        if (!ks.PutWideString(UnitNickName)) return false;
        if (!ks.Put(Reason)) return false;
        if (!ks.PutWideString(CafeNo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint v0)) return false; NexonSN = v0;
        if (!ks.GetWideString(out string s1)) return false; UserId = s1;
        if (!ks.GetWideString(out string s2)) return false; UserName = s2;
        if (!ks.GetWideString(out string s3)) return false; CouponSerial = s3;
        if (!ks.Get(out int v4)) return false; GameType = v4;
        if (!ks.Get(out int v5)) return false; ServerDomain = v5;
        if (!ks.GetWideString(out string s6)) return false; UnitNickName = s6;
        if (!ks.Get(out int v7)) return false; Reason = v7;
        if (!ks.GetWideString(out string s8)) return false; CafeNo = s8;
        return true;
    }
}

public sealed class KPvpMatchUserResultLog : IKSerializable
{
    // enum END_TYPE
    public const int ET_EALRY_LEAVE = 0;
    public const int ET_ALL_KILL = 1;
    public const int ET_TIME_OUT = 2;
    public const int ET_LEAVE_ALL = 3;
    public const int ET_LEAVE_SELF = 4;
    public const int ET_ETC = 5;

    public long UnitUid { get; set; }
    public string NickName { get; set; } = string.Empty;
    public byte Level { get; set; }
    public sbyte UnitClass { get; set; }
    public int Rating { get; set; }
    public sbyte PvpRank { get; set; }
    public int RPoint { get; set; }
    public int APoint { get; set; }
    public int OfficialMatchCnt { get; set; }
    public sbyte Team { get; set; }
    public bool PartyApp { get; set; }
    public int WaitTime { get; set; }
    public int Win { get; set; }
    public int Kill { get; set; }
    public int Assist { get; set; }
    public int Death { get; set; }
    public int NetRatingInc { get; set; }
    public int NetRPInc { get; set; }
    public int NetAPInc { get; set; }
    public int EXPearned { get; set; }
    public sbyte EndType { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutWideString(NickName)) return false;
        if (!ks.Put(Level)) return false;
        if (!ks.Put(UnitClass)) return false;
        if (!ks.Put(Rating)) return false;
        if (!ks.Put(PvpRank)) return false;
        if (!ks.Put(RPoint)) return false;
        if (!ks.Put(APoint)) return false;
        if (!ks.Put(OfficialMatchCnt)) return false;
        if (!ks.Put(Team)) return false;
        if (!ks.Put(PartyApp)) return false;
        if (!ks.Put(WaitTime)) return false;
        if (!ks.Put(Win)) return false;
        if (!ks.Put(Kill)) return false;
        if (!ks.Put(Assist)) return false;
        if (!ks.Put(Death)) return false;
        if (!ks.Put(NetRatingInc)) return false;
        if (!ks.Put(NetRPInc)) return false;
        if (!ks.Put(NetAPInc)) return false;
        if (!ks.Put(EXPearned)) return false;
        if (!ks.Put(EndType)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.GetWideString(out string s1)) return false; NickName = s1;
        if (!ks.Get(out byte v2)) return false; Level = v2;
        if (!ks.Get(out sbyte v3)) return false; UnitClass = v3;
        if (!ks.Get(out int v4)) return false; Rating = v4;
        if (!ks.Get(out sbyte v5)) return false; PvpRank = v5;
        if (!ks.Get(out int v6)) return false; RPoint = v6;
        if (!ks.Get(out int v7)) return false; APoint = v7;
        if (!ks.Get(out int v8)) return false; OfficialMatchCnt = v8;
        if (!ks.Get(out sbyte v9)) return false; Team = v9;
        if (!ks.Get(out bool v10)) return false; PartyApp = v10;
        if (!ks.Get(out int v11)) return false; WaitTime = v11;
        if (!ks.Get(out int v12)) return false; Win = v12;
        if (!ks.Get(out int v13)) return false; Kill = v13;
        if (!ks.Get(out int v14)) return false; Assist = v14;
        if (!ks.Get(out int v15)) return false; Death = v15;
        if (!ks.Get(out int v16)) return false; NetRatingInc = v16;
        if (!ks.Get(out int v17)) return false; NetRPInc = v17;
        if (!ks.Get(out int v18)) return false; NetAPInc = v18;
        if (!ks.Get(out int v19)) return false; EXPearned = v19;
        if (!ks.Get(out sbyte v20)) return false; EndType = v20;
        return true;
    }
}

public sealed class KPvpPlayResultInfo : IKSerializable
{
    public int RPoint { get; set; }
    public int APoint { get; set; }
    public int WinCount { get; set; }
    public bool IsPlayWithParty { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(RPoint)) return false;
        if (!ks.Put(APoint)) return false;
        if (!ks.Put(WinCount)) return false;
        if (!ks.Put(IsPlayWithParty)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; RPoint = v0;
        if (!ks.Get(out int v1)) return false; APoint = v1;
        if (!ks.Get(out int v2)) return false; WinCount = v2;
        if (!ks.Get(out bool v3)) return false; IsPlayWithParty = v3;
        return true;
    }
}

public class KQuestReq : IKSerializable
{
    public long UnitUid { get; set; }
    public int QuestId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(QuestId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.Get(out int v1)) return false; QuestId = v1;
        return true;
    }
}

public sealed class KQuestUpdate : IKSerializable
{
    public int QuestId { get; set; }
    public List<byte> ClearData { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(QuestId)) return false;
        if (!ks.PutList(ClearData, static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; QuestId = v0;
        if (!ks.GetList(ClearData, static s => (s.Get(out byte v), v))) return false;
        return true;
    }
}

public sealed class KRandomItemMappingToInfoServer : IKSerializable
{
    public int Index { get; set; }
    public int CubeId { get; set; }
    public int KeyItemId { get; set; }
    public int BeforeGroupId { get; set; }
    public int AfterGroupId { get; set; }
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.JP) && !ks.Put(Index)) return false;
        if ((region.Region == RegionCode.JP) && !ks.Put(CubeId)) return false;
        if ((region.Region == RegionCode.JP) && !ks.Put(KeyItemId)) return false;
        if ((region.Region == RegionCode.JP) && !ks.Put(BeforeGroupId)) return false;
        if ((region.Region == RegionCode.JP) && !ks.Put(AfterGroupId)) return false;
        if ((region.Region == RegionCode.JP) && !ks.PutWideString(StartTime)) return false;
        if ((region.Region == RegionCode.JP) && !ks.PutWideString(EndTime)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if ((region.Region == RegionCode.JP)) { if (!ks.Get(out int v0)) return false; Index = v0; }
        if ((region.Region == RegionCode.JP)) { if (!ks.Get(out int v1)) return false; CubeId = v1; }
        if ((region.Region == RegionCode.JP)) { if (!ks.Get(out int v2)) return false; KeyItemId = v2; }
        if ((region.Region == RegionCode.JP)) { if (!ks.Get(out int v3)) return false; BeforeGroupId = v3; }
        if ((region.Region == RegionCode.JP)) { if (!ks.Get(out int v4)) return false; AfterGroupId = v4; }
        if ((region.Region == RegionCode.JP)) { if (!ks.GetWideString(out string s5)) return false; StartTime = s5; }
        if ((region.Region == RegionCode.JP)) { if (!ks.GetWideString(out string s6)) return false; EndTime = s6; }
        return true;
    }
}

public sealed class KRankerInfo : IKSerializable
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

public sealed class KRankingTitleReward : IKSerializable
{
    public long UnitUid { get; set; }
    public List<KTitleReward> Reward { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutList(Reward)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UnitUid = v0;
        if (!ks.GetList(Reward)) return false;
        return true;
    }
}

public sealed class KRecordBuffInfo : IKSerializable
{
    public int BuffId { get; set; }
    public string StartTime { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(BuffId)) return false;
        if (!ks.PutWideString(StartTime)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; BuffId = v0;
        if (!ks.GetWideString(out string s1)) return false; StartTime = s1;
        return true;
    }
}

public sealed class KRegUnitInfo : IKSerializable
{
    public long UserUid { get; set; }
    public long UnitUid { get; set; }
    public string UnitNickName { get; set; } = string.Empty;
    public long GsUid { get; set; }
    public int ChannelId { get; set; }
    public string Ip { get; set; } = string.Empty;
    public sbyte AuthLevel { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UserUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutWideString(UnitNickName)) return false;
        if (!ks.Put(GsUid)) return false;
        if (!ks.Put(ChannelId)) return false;
        if (!ks.PutWideString(Ip)) return false;
        if (!ks.Put(AuthLevel)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; UserUid = v0;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        if (!ks.GetWideString(out string s2)) return false; UnitNickName = s2;
        if (!ks.Get(out long v3)) return false; GsUid = v3;
        if (!ks.Get(out int v4)) return false; ChannelId = v4;
        if (!ks.GetWideString(out string s5)) return false; Ip = s5;
        if (!ks.Get(out sbyte v6)) return false; AuthLevel = v6;
        return true;
    }
}

public sealed class KRegroupPartyUserInfo : IKSerializable
{
    public KPartyUserInfo PartyUserInfo { get; set; } = new();
    public KPartyRoomUserInfo PartyRoomUserInfo { get; set; } = new();
    public KOldPartyInfo OldPartyInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PartyUserInfo)) return false;
        if (!ks.Put(PartyRoomUserInfo)) return false;
        if (!ks.Put(OldPartyInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(PartyUserInfo)) return false;
        if (!ks.Get(PartyRoomUserInfo)) return false;
        if (!ks.Get(OldPartyInfo)) return false;
        return true;
    }
}

public sealed class KRelationshipInfo : IKSerializable
{
    public sbyte RelationshipType { get; set; }
    public long OtherUnitUid { get; set; }
    public string OtherNickName { get; set; } = string.Empty;
    public string LoveWord { get; set; } = string.Empty;
    public long Date { get; set; }
    public long LastReward { get; set; }
    public sbyte RewardTitleType { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(RelationshipType)) return false;
        if (!ks.Put(OtherUnitUid)) return false;
        if (!ks.PutWideString(OtherNickName)) return false;
        if (!ks.PutWideString(LoveWord)) return false;
        if (!ks.Put(Date)) return false;
        if (!ks.Put(LastReward)) return false;
        if (!ks.Put(RewardTitleType)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte v0)) return false; RelationshipType = v0;
        if (!ks.Get(out long v1)) return false; OtherUnitUid = v1;
        if (!ks.GetWideString(out string s2)) return false; OtherNickName = s2;
        if (!ks.GetWideString(out string s3)) return false; LoveWord = s3;
        if (!ks.Get(out long v4)) return false; Date = v4;
        if (!ks.Get(out long v5)) return false; LastReward = v5;
        if (!ks.Get(out sbyte v6)) return false; RewardTitleType = v6;
        return true;
    }
}

public sealed class KRetrievedSkillPageData : IKSerializable
{
    public int RetrievedSPoint { get; set; }
    public List<KUserSkillData> UserSkillData { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(RetrievedSPoint)) return false;
        if (!ks.PutList(UserSkillData)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; RetrievedSPoint = v0;
        if (!ks.GetList(UserSkillData)) return false;
        return true;
    }
}

public sealed class KReturnToFieldInfo : IKSerializable
{
    public long ReturnToFieldUnitUid { get; set; }
    public long RoomUid { get; set; }
    public HashSet<long> RemainUnitUid { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ReturnToFieldUnitUid)) return false;
        if (!ks.Put(RoomUid)) return false;
        if (!ks.PutList(RemainUnitUid, static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; ReturnToFieldUnitUid = v0;
        if (!ks.Get(out long v1)) return false; RoomUid = v1;
        if (!ks.GetHashSet(RemainUnitUid, static s => (s.Get(out long v), v))) return false;
        return true;
    }
}

public sealed class KSellPShopItemBackupData : IKSerializable
{
    public KSellPersonalShopItemInfo SellPShopItemInfo { get; set; } = new();
    public int TotalSoldItemQuantity { get; set; }
    public int TotalSellCommissionED { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(SellPShopItemInfo)) return false;
        if (!ks.Put(TotalSoldItemQuantity)) return false;
        if (!ks.Put(TotalSellCommissionED)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(SellPShopItemInfo)) return false;
        if (!ks.Get(out int v1)) return false; TotalSoldItemQuantity = v1;
        if (!ks.Get(out int v2)) return false; TotalSellCommissionED = v2;
        return true;
    }
}

public sealed class KSellPShopItemDBUpdateInfo : IKSerializable
{
    public long ItemUid { get; set; }
    public int Quantity { get; set; }
    public int TotalSellEDIn { get; set; }
    public int TotalSoldItemQuantity { get; set; }
    public int TotalSellCommissionED { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ItemUid)) return false;
        if (!ks.Put(Quantity)) return false;
        if (!ks.Put(TotalSellEDIn)) return false;
        if (!ks.Put(TotalSoldItemQuantity)) return false;
        if (!ks.Put(TotalSellCommissionED)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; ItemUid = v0;
        if (!ks.Get(out int v1)) return false; Quantity = v1;
        if (!ks.Get(out int v2)) return false; TotalSellEDIn = v2;
        if (!ks.Get(out int v3)) return false; TotalSoldItemQuantity = v3;
        if (!ks.Get(out int v4)) return false; TotalSellCommissionED = v4;
        return true;
    }
}

public class KSquareCreateInfo : IKSerializable
{
    public int SquareType { get; set; }
    public string SquareName { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(SquareType)) return false;
        if (!ks.PutWideString(SquareName)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; SquareType = v0;
        if (!ks.GetWideString(out string s1)) return false; SquareName = s1;
        return true;
    }
}

public sealed class KStatIncreaseRate : IKSerializable
{
    public float IncreaseAtkPhysicRate { get; set; }
    public float IncreaseAtkMagicRate { get; set; }
    public float IncreaseDefPhysicRate { get; set; }
    public float IncreaseDefMagicRate { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(IncreaseAtkPhysicRate)) return false;
        if (!ks.Put(IncreaseAtkMagicRate)) return false;
        if (!ks.Put(IncreaseDefPhysicRate)) return false;
        if (!ks.Put(IncreaseDefMagicRate)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out float v0)) return false; IncreaseAtkPhysicRate = v0;
        if (!ks.Get(out float v1)) return false; IncreaseAtkMagicRate = v1;
        if (!ks.Get(out float v2)) return false; IncreaseDefPhysicRate = v2;
        if (!ks.Get(out float v3)) return false; IncreaseDefMagicRate = v3;
        return true;
    }
}

public sealed class KStatRelationLevel : IKSerializable
{
    public float BaseHPRelLV { get; set; }
    public float AtkPhysicRelLV { get; set; }
    public float AtkMagicRelLV { get; set; }
    public float DefPhysicRelLV { get; set; }
    public float DefMagicRelLV { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(BaseHPRelLV)) return false;
        if (!ks.Put(AtkPhysicRelLV)) return false;
        if (!ks.Put(AtkMagicRelLV)) return false;
        if (!ks.Put(DefPhysicRelLV)) return false;
        if (!ks.Put(DefMagicRelLV)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out float v0)) return false; BaseHPRelLV = v0;
        if (!ks.Get(out float v1)) return false; AtkPhysicRelLV = v1;
        if (!ks.Get(out float v2)) return false; AtkMagicRelLV = v2;
        if (!ks.Get(out float v3)) return false; DefPhysicRelLV = v3;
        if (!ks.Get(out float v4)) return false; DefMagicRelLV = v4;
        return true;
    }
}

public sealed class KStateChangeAck : IKSerializable
{
    public int Ok { get; set; }
    public uint MapId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(MapId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out uint v1)) return false; MapId = v1;
        return true;
    }
}

public sealed class KStateChangeReq : IKSerializable
{
    public uint MapId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(MapId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint v0)) return false; MapId = v0;
        return true;
    }
}

public sealed class KStatisticsDetailData : IKSerializable
{
    public int Num { get; set; }
    public string WszString { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Num)) return false;
        if (!ks.PutWideString(WszString)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Num = v0;
        if (!ks.GetWideString(out string s1)) return false; WszString = s1;
        return true;
    }
}

public sealed class KStatisticsInfo : IKSerializable
{
    public int StatisticsId { get; set; }
    public KStatisticsKey Key { get; set; } = new();
    public int Index { get; set; }
    public int Count { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(StatisticsId)) return false;
        if (!ks.Put(Key)) return false;
        if (!ks.Put(Index)) return false;
        if (!ks.Put(Count)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; StatisticsId = v0;
        if (!ks.Get(Key)) return false;
        if (!ks.Get(out int v2)) return false; Index = v2;
        if (!ks.Get(out int v3)) return false; Count = v3;
        return true;
    }
}

public sealed class KStatisticsKey : IKSerializable
{
    public List<int> IntKey { get; set; } = new();
    public List<string> StringKey { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutList(IntKey, static (s, v) => s.Put(v))) return false;
        if (!ks.PutList(StringKey, static (s, v) => s.PutWideString(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetList(IntKey, static s => (s.Get(out int v), v))) return false;
        if (!ks.GetList(StringKey, static s => (s.GetWideString(out string v), v))) return false;
        return true;
    }
}

public sealed class KSubMissionInstance : IKSerializable
{
    public int Id { get; set; }
    public short ClearData { get; set; }
    public bool IsSuccess { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Id)) return false;
        if (!ks.Put(ClearData)) return false;
        if (!ks.Put(IsSuccess)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Id = v0;
        if (!ks.Get(out short v1)) return false; ClearData = v1;
        if (!ks.Get(out bool v2)) return false; IsSuccess = v2;
        return true;
    }
}

public class KSuccessTradeData : IKSerializable
{
    public bool IsSuccess { get; set; }
    public List<KInventoryItemInfo> InventorySlotInfo { get; set; } = new();
    public KERM_CONFIRM_TRADE_NOT ConfirmTradeNot { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(IsSuccess)) return false;
        if (!ks.PutList(InventorySlotInfo)) return false;
        if (!ks.Put(ConfirmTradeNot)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool v0)) return false; IsSuccess = v0;
        if (!ks.GetList(InventorySlotInfo)) return false;
        if (!ks.Get(ConfirmTradeNot)) return false;
        return true;
    }
}

public sealed class KTempInventoryItemInfo : IKSerializable
{
    public int TempItemUid { get; set; }
    public int ItemId { get; set; }
    public int Quantity { get; set; }
    public byte SealData { get; set; }
    public sbyte EnchantLevel { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(TempItemUid)) return false;
        if (!ks.Put(ItemId)) return false;
        if (!ks.Put(Quantity)) return false;
        if (!ks.Put(SealData)) return false;
        if (!ks.Put(EnchantLevel)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; TempItemUid = v0;
        if (!ks.Get(out int v1)) return false; ItemId = v1;
        if (!ks.Get(out int v2)) return false; Quantity = v2;
        if (!ks.Get(out byte v3)) return false; SealData = v3;
        if (!ks.Get(out sbyte v4)) return false; EnchantLevel = v4;
        return true;
    }
}

public sealed class KTileInfo : IKSerializable
{
    public int NumX { get; set; }
    public int NumY { get; set; }
    public int NumZ { get; set; }
    public short SizeX { get; set; }
    public short SizeY { get; set; }
    public short SizeZ { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(NumX)) return false;
        if (!ks.Put(NumY)) return false;
        if (!ks.Put(NumZ)) return false;
        if (!ks.Put(SizeX)) return false;
        if (!ks.Put(SizeY)) return false;
        if (!ks.Put(SizeZ)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; NumX = v0;
        if (!ks.Get(out int v1)) return false; NumY = v1;
        if (!ks.Get(out int v2)) return false; NumZ = v2;
        if (!ks.Get(out short v3)) return false; SizeX = v3;
        if (!ks.Get(out short v4)) return false; SizeY = v4;
        if (!ks.Get(out short v5)) return false; SizeZ = v5;
        return true;
    }
}

public sealed class KTimeDropInfo : IKSerializable
{
    public int TimeDropId { get; set; }
    public int DropItemId { get; set; }
    public string DropTime { get; set; } = string.Empty;
    public string ServerIP { get; set; } = string.Empty;
    public bool NoticeMsg { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(TimeDropId)) return false;
        if (!ks.Put(DropItemId)) return false;
        if (!ks.PutWideString(DropTime)) return false;
        if (!ks.PutWideString(ServerIP)) return false;
        if (!ks.Put(NoticeMsg)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; TimeDropId = v0;
        if (!ks.Get(out int v1)) return false; DropItemId = v1;
        if (!ks.GetWideString(out string s2)) return false; DropTime = s2;
        if (!ks.GetWideString(out string s3)) return false; ServerIP = s3;
        if (!ks.Get(out bool v4)) return false; NoticeMsg = v4;
        return true;
    }
}

public sealed class KTimeEnchantEventInfo : IKSerializable
{
    public int EventUid { get; set; }
    public long BeginDate { get; set; }
    public long EndDate { get; set; }
    public sbyte EnchantLevel { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(EventUid)) return false;
        if (!ks.Put(BeginDate)) return false;
        if (!ks.Put(EndDate)) return false;
        if (!ks.Put(EnchantLevel)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; EventUid = v0;
        if (!ks.Get(out long v1)) return false; BeginDate = v1;
        if (!ks.Get(out long v2)) return false; EndDate = v2;
        if (!ks.Get(out sbyte v3)) return false; EnchantLevel = v3;
        return true;
    }
}

public sealed class KTimeEventSimpleInfo : IKSerializable
{
    public int EventUid { get; set; }
    public int EventId { get; set; }
    public string EventName { get; set; } = string.Empty;
    public float EventTime { get; set; }
    public string AdvertisementURL { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(EventUid)) return false;
        if (!ks.Put(EventId)) return false;
        if (!ks.PutWideString(EventName)) return false;
        if (!ks.Put(EventTime)) return false;
        if (!ks.PutWideString(AdvertisementURL)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; EventUid = v0;
        if (!ks.Get(out int v1)) return false; EventId = v1;
        if (!ks.GetWideString(out string s2)) return false; EventName = s2;
        if (!ks.Get(out float v3)) return false; EventTime = v3;
        if (!ks.GetWideString(out string s4)) return false; AdvertisementURL = s4;
        return true;
    }
}

public sealed class KTimeOpenRandomItemEventDBUPdate : IKSerializable
{
    public int Id { get; set; }
    public long UpdateTime { get; set; }
    public long GetUserUid { get; set; }
    public long GetUnitUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Id)) return false;
        if (!ks.Put(UpdateTime)) return false;
        if (!ks.Put(GetUserUid)) return false;
        if (!ks.Put(GetUnitUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Id = v0;
        if (!ks.Get(out long v1)) return false; UpdateTime = v1;
        if (!ks.Get(out long v2)) return false; GetUserUid = v2;
        if (!ks.Get(out long v3)) return false; GetUnitUid = v3;
        return true;
    }
}

public sealed class KTimeOpenRandomItemEventInfo : IKSerializable
{
    public int Id { get; set; }
    public int ItemId { get; set; }
    public string DropTime { get; set; } = string.Empty;
    public string DropEndTime { get; set; } = string.Empty;
    public int OpenItemId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Id)) return false;
        if (!ks.Put(ItemId)) return false;
        if (!ks.PutWideString(DropTime)) return false;
        if (!ks.PutWideString(DropEndTime)) return false;
        if (!ks.Put(OpenItemId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Id = v0;
        if (!ks.Get(out int v1)) return false; ItemId = v1;
        if (!ks.GetWideString(out string s2)) return false; DropTime = s2;
        if (!ks.GetWideString(out string s3)) return false; DropEndTime = s3;
        if (!ks.Get(out int v4)) return false; OpenItemId = v4;
        return true;
    }
}

public sealed class KTitleInfo : IKSerializable
{
    public int TitleId { get; set; }
    public string EndDate { get; set; } = string.Empty;
    public long CTime { get; set; }
    public bool Infinity { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(TitleId)) return false;
        if (!ks.PutWideString(EndDate)) return false;
        if (!ks.Put(CTime)) return false;
        if (!ks.Put(Infinity)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; TitleId = v0;
        if (!ks.GetWideString(out string s1)) return false; EndDate = s1;
        if (!ks.Get(out long v2)) return false; CTime = v2;
        if (!ks.Get(out bool v3)) return false; Infinity = v3;
        return true;
    }
}

public sealed class KTitleReward : IKSerializable
{
    public int TitleId { get; set; }
    public short Period { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(TitleId)) return false;
        if (!ks.Put(Period)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; TitleId = v0;
        if (!ks.Get(out short v1)) return false; Period = v1;
        return true;
    }
}

public sealed class KTradeShareItemInfo : IKSerializable
{
    public long ItemUid { get; set; }
    public long FromUnitUid { get; set; }
    public long ToUnitUid { get; set; }
    public sbyte InventoryCategory { get; set; }
    public short SlotId { get; set; }
    public byte SealCnt { get; set; }
    public byte DeleteReason { get; set; }
    public int ItemId { get; set; }
    public long NewItemUid { get; set; }
    public string RegDate { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ItemUid)) return false;
        if (!ks.Put(FromUnitUid)) return false;
        if (!ks.Put(ToUnitUid)) return false;
        if (!ks.Put(InventoryCategory)) return false;
        if (!ks.Put(SlotId)) return false;
        if (!ks.Put(SealCnt)) return false;
        if (!ks.Put(DeleteReason)) return false;
        if (!ks.Put(ItemId)) return false;
        if (!ks.Put(NewItemUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; ItemUid = v0;
        if (!ks.Get(out long v1)) return false; FromUnitUid = v1;
        if (!ks.Get(out long v2)) return false; ToUnitUid = v2;
        if (!ks.Get(out sbyte v3)) return false; InventoryCategory = v3;
        if (!ks.Get(out short v4)) return false; SlotId = v4;
        if (!ks.Get(out byte v5)) return false; SealCnt = v5;
        if (!ks.Get(out byte v6)) return false; DeleteReason = v6;
        if (!ks.Get(out int v7)) return false; ItemId = v7;
        if (!ks.Get(out long v8)) return false; NewItemUid = v8;
        return true;
    }
}

public sealed class KTradeShareItemResult : IKSerializable
{
    public long BeforeItemUid { get; set; }
    public long NewItemUid { get; set; }
    public string RegDate { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(BeforeItemUid)) return false;
        if (!ks.Put(NewItemUid)) return false;
        if (!ks.PutWideString(RegDate)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; BeforeItemUid = v0;
        if (!ks.Get(out long v1)) return false; NewItemUid = v1;
        if (!ks.GetWideString(out string s2)) return false; RegDate = s2;
        return true;
    }
}

public sealed class KTradeUserInfo : IKSerializable
{
    public long GsUid { get; set; }
    public long UserUid { get; set; }
    public long UnitUid { get; set; }
    public string NickName { get; set; } = string.Empty;
    public int Ed { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(GsUid)) return false;
        if (!ks.Put(UserUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutWideString(NickName)) return false;
        if (!ks.Put(Ed)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; GsUid = v0;
        if (!ks.Get(out long v1)) return false; UserUid = v1;
        if (!ks.Get(out long v2)) return false; UnitUid = v2;
        if (!ks.GetWideString(out string s3)) return false; NickName = s3;
        if (!ks.Get(out int v4)) return false; Ed = v4;
        return true;
    }
}

public sealed class KTutorialDBUnitInfo : IKSerializable
{
    public KTutorialUnitInfo TutorialUnitInfo { get; set; } = new();
    public string LastDate { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(TutorialUnitInfo)) return false;
        if (!ks.PutWideString(LastDate)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(TutorialUnitInfo)) return false;
        if (!ks.GetWideString(out string s1)) return false; LastDate = s1;
        return true;
    }
}

public sealed class KUnitBuffInfo : IKSerializable
{
    public long InsertTime { get; set; }
    public Dictionary<int, KBuffInfo> BuffInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(InsertTime)) return false;
        if (!ks.PutMap(BuffInfo, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; InsertTime = v0;
        if (!ks.GetMap(BuffInfo, static s => (s.Get(out int v), v), static s => { var v = new KBuffInfo(); return (s.Get(v), v); })) return false;
        return true;
    }
}

public sealed class KUpdateCCUInfo : IKSerializable
{
    public long ServerUid { get; set; }
    public int ConcurrentUser { get; set; }
    public int CurPartyCount { get; set; }
    public int PlayGamePartyCount { get; set; }
    public Dictionary<int, int> ChannelingUserList { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ServerUid)) return false;
        if (!ks.Put(ConcurrentUser)) return false;
        if (!ks.Put(CurPartyCount)) return false;
        if (!ks.Put(PlayGamePartyCount)) return false;
        if ((region.Region == RegionCode.JP) && !ks.PutMap(ChannelingUserList, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; ServerUid = v0;
        if (!ks.Get(out int v1)) return false; ConcurrentUser = v1;
        if (!ks.Get(out int v2)) return false; CurPartyCount = v2;
        if (!ks.Get(out int v3)) return false; PlayGamePartyCount = v3;
        if ((region.Region == RegionCode.JP)) { if (!ks.GetMap(ChannelingUserList, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false; }
        return true;
    }
}

public sealed class KUpdateShareItemInfo : IKSerializable
{
    public long ItemUid { get; set; }
    public long UnitUid { get; set; }
    public int UsageType { get; set; }
    public int Value { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ItemUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(UsageType)) return false;
        if (!ks.Put(Value)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false; ItemUid = v0;
        if (!ks.Get(out long v1)) return false; UnitUid = v1;
        if (!ks.Get(out int v2)) return false; UsageType = v2;
        if (!ks.Get(out int v3)) return false; Value = v3;
        return true;
    }
}

public sealed class KUserCumulativeTimeEventInfo : IKSerializable
{
    public int EventUid { get; set; }
    public int EventId { get; set; }
    public bool Init { get; set; }
    public string EventName { get; set; } = string.Empty;
    public float EventTime { get; set; }
    public bool DirectReward { get; set; }
    public bool AccountEvent { get; set; }
    public bool NewUnitEvent { get; set; }
    public bool NewUnitEvent2 { get; set; }
    public bool CurrentUnitEvent { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(EventUid)) return false;
        if (!ks.Put(EventId)) return false;
        if (!ks.Put(Init)) return false;
        if (!ks.PutWideString(EventName)) return false;
        if (!ks.Put(EventTime)) return false;
        if (!ks.Put(DirectReward)) return false;
        if (!ks.Put(AccountEvent)) return false;
        if ((region.Region == RegionCode.JP) && !ks.Put(NewUnitEvent)) return false;
        if ((region.Region == RegionCode.JP) && !ks.Put(NewUnitEvent2)) return false;
        if ((region.Region == RegionCode.JP) && !ks.Put(CurrentUnitEvent)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; EventUid = v0;
        if (!ks.Get(out int v1)) return false; EventId = v1;
        if (!ks.Get(out bool v2)) return false; Init = v2;
        if (!ks.GetWideString(out string s3)) return false; EventName = s3;
        if (!ks.Get(out float v4)) return false; EventTime = v4;
        if (!ks.Get(out bool v5)) return false; DirectReward = v5;
        if (!ks.Get(out bool v6)) return false; AccountEvent = v6;
        if ((region.Region == RegionCode.JP)) { if (!ks.Get(out bool v7)) return false; NewUnitEvent = v7; }
        if ((region.Region == RegionCode.JP)) { if (!ks.Get(out bool v8)) return false; NewUnitEvent2 = v8; }
        if ((region.Region == RegionCode.JP)) { if (!ks.Get(out bool v9)) return false; CurrentUnitEvent = v9; }
        return true;
    }
}

public sealed class KUserPShopAgencyInfo : IKSerializable
{
    public bool IsPShopOpen { get; set; }
    public string AgencyExpirationDate { get; set; } = string.Empty;
    public sbyte ShopType { get; set; }
    public List<KSellPShopItemBackupData> SellItemInfo { get; set; } = new();
    public long UserUid { get; set; }
    public long UnitUid { get; set; }
    public string NickName { get; set; } = string.Empty;
    public string AgencyOpenDate { get; set; } = string.Empty;
    public string PersonalShopName { get; set; } = string.Empty;
    public bool OnSale { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(IsPShopOpen)) return false;
        if (!ks.PutWideString(AgencyExpirationDate)) return false;
        if (!ks.Put(ShopType)) return false;
        if (!ks.PutList(SellItemInfo)) return false;
        if (!ks.Put(UserUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutWideString(NickName)) return false;
        if (!ks.PutWideString(AgencyOpenDate)) return false;
        if (!ks.PutWideString(PersonalShopName)) return false;
        if (!ks.Put(OnSale)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool v0)) return false; IsPShopOpen = v0;
        if (!ks.GetWideString(out string s1)) return false; AgencyExpirationDate = s1;
        if (!ks.Get(out sbyte v2)) return false; ShopType = v2;
        if (!ks.GetList(SellItemInfo)) return false;
        if (!ks.Get(out long v4)) return false; UserUid = v4;
        if (!ks.Get(out long v5)) return false; UnitUid = v5;
        if (!ks.GetWideString(out string s6)) return false; NickName = s6;
        if (!ks.GetWideString(out string s7)) return false; AgencyOpenDate = s7;
        if (!ks.GetWideString(out string s8)) return false; PersonalShopName = s8;
        if (!ks.Get(out bool v9)) return false; OnSale = v9;
        return true;
    }
}

public sealed class kRandomItemMappingToInfoClient : IKSerializable
{
    public int BeforeGroupId { get; set; }
    public int AfterGroupId { get; set; }
    public bool CheckMapping { get; set; }
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(BeforeGroupId)) return false;
        if (!ks.Put(AfterGroupId)) return false;
        if (!ks.Put(CheckMapping)) return false;
        if (!ks.PutWideString(StartTime)) return false;
        if (!ks.PutWideString(EndTime)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; BeforeGroupId = v0;
        if (!ks.Get(out int v1)) return false; AfterGroupId = v1;
        if (!ks.Get(out bool v2)) return false; CheckMapping = v2;
        if (!ks.GetWideString(out string s3)) return false; StartTime = s3;
        if (!ks.GetWideString(out string s4)) return false; EndTime = s4;
        return true;
    }
}

public sealed class KDBE_GIVE_UP_QUEST_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int QuestId { get; set; }
    public Dictionary<int, int> InsertedItem { get; set; } = new();
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();
    public Dictionary<long, KItemInfo> ItemInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(QuestId)) return false;
        if (!ks.PutMap(InsertedItem, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        if (!ks.PutMap(ItemInfo, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out int v1)) return false; QuestId = v1;
        if (!ks.GetMap(InsertedItem, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        if (!ks.GetMap(ItemInfo, static s => (s.Get(out long v), v), static s => { var v = new KItemInfo(); return (s.Get(v), v); })) return false;
        return true;
    }
}

public sealed class KDBE_GIVE_UP_QUEST_REQ : KQuestReq { }

public sealed class KDBE_NEW_QUEST_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int QuestId { get; set; }
    public Dictionary<int, int> InsertedItem { get; set; } = new();
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();
    public Dictionary<long, KItemInfo> ItemInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(QuestId)) return false;
        if (!ks.PutMap(InsertedItem, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        if (!ks.Put(ItemQuantityUpdate)) return false;
        if (!ks.PutList(UpdatedInventorySlot)) return false;
        if (!ks.PutMap(ItemInfo, static (s, k) => s.Put(k), static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false; Ok = v0;
        if (!ks.Get(out int v1)) return false; QuestId = v1;
        if (!ks.GetMap(InsertedItem, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false;
        if (!ks.Get(ItemQuantityUpdate)) return false;
        if (!ks.GetList(UpdatedInventorySlot)) return false;
        if (!ks.GetMap(ItemInfo, static s => (s.Get(out long v), v), static s => { var v = new KItemInfo(); return (s.Get(v), v); })) return false;
        return true;
    }
}

public sealed class KDBE_WRITE_LETTER_AND_TRADE_MOINTORING_ACK : KPacketOK { }

public sealed class KERM_CHANGE_PVP_ROOM_NAME_ACK : IKSerializable
{
    public string RoomName { get; set; } = string.Empty;
    public int Ok { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutWideString(RoomName)) return false;
        if (!ks.Put(Ok)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string s0)) return false; RoomName = s0;
        if (!ks.Get(out int v1)) return false; Ok = v1;
        return true;
    }
}

public sealed class KERM_CHANGE_PVP_ROOM_NAME_REQ : IKSerializable
{
    public string RoomName { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutWideString(RoomName)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string s0)) return false; RoomName = s0;
        return true;
    }
}

public sealed class KERM_CHANGE_PVP_ROOM_PUBLIC_ACK : IKSerializable
{
    public bool Public { get; set; }
    public int Ok { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Public)) return false;
        if (!ks.Put(Ok)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool v0)) return false; Public = v0;
        if (!ks.Get(out int v1)) return false; Ok = v1;
        return true;
    }
}

public sealed class KERM_CHANGE_PVP_ROOM_PUBLIC_REQ : IKSerializable
{
    public bool Public { get; set; }
    public string Password { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Public)) return false;
        if (!ks.PutWideString(Password)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool v0)) return false; Public = v0;
        if (!ks.GetWideString(out string s1)) return false; Password = s1;
        return true;
    }
}
