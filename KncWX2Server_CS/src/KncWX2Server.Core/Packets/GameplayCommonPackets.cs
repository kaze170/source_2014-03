using KncWX2Server.Core.Region;
using KncWX2Server.Core.Serialization;

namespace KncWX2Server.Core.Packets;

public sealed class KVector3 : IKSerializable
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
        => ks.Put(X) && ks.Put(Y) && ks.Put(Z);

    public bool Get(KSerializer ks, RegionProfile region)
        => ks.Get(out float x) && ks.Get(out float y) && ks.Get(out float z)
        && Set(x, y, z);

    private bool Set(float x, float y, float z) { X = x; Y = y; Z = z; return true; }
}

public sealed class KAccountOption : IKSerializable
{
    public bool PlayGuide { get; set; } = true;

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(PlayGuide);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool v)) return false;
        PlayGuide = v;
        return true;
    }
}

public sealed class KAccountBlockInfo : IKSerializable
{
    public string EndTime { get; set; } = string.Empty;
    public string BlockReason { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
        => ks.PutWideString(EndTime) && ks.PutWideString(BlockReason);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string end) || !ks.GetWideString(out string reason)) return false;
        EndTime = end;
        BlockReason = reason;
        return true;
    }
}

public sealed class KAccountInfo : IKSerializable
{
    public long UserUid { get; set; }
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int AuthLevel { get; set; }
    public bool InternalUser { get; set; }
    public KAccountOption AccountOption { get; set; } = new();
    public KAccountBlockInfo AccountBlockInfo { get; set; } = new();
    public bool IsRecommend { get; set; }
    public bool IsGuestUser { get; set; }
    public string Otp { get; set; } = string.Empty;
    public string RegDate { get; set; } = string.Empty;
    public string LastLogin { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(UserUid) &&
        ks.PutWideString(Id) &&
        ks.PutWideString(Name) &&
        ks.Put(AuthLevel) &&
        ks.Put(InternalUser) &&
        ks.Put(AccountOption) &&
        ks.Put(AccountBlockInfo) &&
        ks.Put(IsRecommend) &&
        ks.Put(IsGuestUser) &&
        ks.PutWideString(Otp) &&
        ks.PutWideString(RegDate) &&
        ks.PutWideString(LastLogin);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.GetWideString(out string id) || !ks.GetWideString(out string name) ||
            !ks.Get(out int auth) || !ks.Get(out bool intern) || !ks.Get(AccountOption) || !ks.Get(AccountBlockInfo) ||
            !ks.Get(out bool rec) || !ks.Get(out bool guest) || !ks.GetWideString(out string otp) ||
            !ks.GetWideString(out string reg) || !ks.GetWideString(out string last))
            return false;

        UserUid = uid; Id = id; Name = name; AuthLevel = auth; InternalUser = intern;
        IsRecommend = rec; IsGuestUser = guest; Otp = otp; RegDate = reg; LastLogin = last;
        return true;
    }
}

public sealed class KStat : IKSerializable
{
    public int BaseHp { get; set; } = 10000;
    public int AtkPhysic { get; set; } = 500;
    public int AtkMagic { get; set; } = 500;
    public int DefPhysic { get; set; } = 250;
    public int DefMagic { get; set; } = 250;

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(BaseHp) && ks.Put(AtkPhysic) && ks.Put(AtkMagic) && ks.Put(DefPhysic) && ks.Put(DefMagic);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int hp) || !ks.Get(out int ap) || !ks.Get(out int am) ||
            !ks.Get(out int dp) || !ks.Get(out int dm)) return false;
        BaseHp = hp; AtkPhysic = ap; AtkMagic = am; DefPhysic = dp; DefMagic = dm;
        return true;
    }
}

public sealed class KItemAttributeEnchantInfo : IKSerializable
{
    public sbyte AttribEnchant0 { get; set; }
    public sbyte AttribEnchant1 { get; set; }
    public sbyte AttribEnchant2 { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(AttribEnchant0) && ks.Put(AttribEnchant1) && ks.Put(AttribEnchant2);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte a0) || !ks.Get(out sbyte a1) || !ks.Get(out sbyte a2)) return false;
        AttribEnchant0 = a0; AttribEnchant1 = a1; AttribEnchant2 = a2;
        return true;
    }
}

public sealed class KItemInfo : IKSerializable
{
    public int ItemId { get; set; }
    public sbyte UsageType { get; set; }
    public int Quantity { get; set; } = 1;
    public short Endurance { get; set; } = 100;
    public byte SealData { get; set; }
    public sbyte EnchantLevel { get; set; }
    public KItemAttributeEnchantInfo AttribEnchantInfo { get; set; } = new();
    public List<int> ItemSockets { get; set; } = new();
    public List<int> RandomSockets { get; set; } = new();
    public sbyte ItemState { get; set; } = 1;
    public short Period { get; set; }
    public string ExpirationDate { get; set; } = string.Empty;
    public long GoldTicketKeyUid { get; set; }
    public byte ExpandedSocketNum { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ItemId) || !ks.Put(UsageType) || !ks.Put(Quantity) || !ks.Put(Endurance) ||
            !ks.Put(SealData) || !ks.Put(EnchantLevel) || !ks.Put(AttribEnchantInfo) ||
            !ks.PutList(ItemSockets, static (s, v) => s.Put(v)) ||
            !ks.PutList(RandomSockets, static (s, v) => s.Put(v)) ||
            !ks.Put(ItemState) || !ks.Put(Period) || !ks.PutWideString(ExpirationDate))
            return false;
        if (region.GoldTicket && !ks.Put(GoldTicketKeyUid)) return false;
        return ks.Put(ExpandedSocketNum);
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int itemId) || !ks.Get(out sbyte usage) || !ks.Get(out int qty) ||
            !ks.Get(out short end) || !ks.Get(out byte seal) || !ks.Get(out sbyte ench) ||
            !ks.Get(AttribEnchantInfo) ||
            !ks.GetList(ItemSockets, static s => (s.Get(out int v), v)) ||
            !ks.GetList(RandomSockets, static s => (s.Get(out int v), v)) ||
            !ks.Get(out sbyte state) || !ks.Get(out short period) || !ks.GetWideString(out string exp))
            return false;
        long gold = 0;
        if (region.GoldTicket && !ks.Get(out gold)) return false;
        if (!ks.Get(out byte expSock)) return false;

        ItemId = itemId; UsageType = usage; Quantity = qty; Endurance = end; SealData = seal;
        EnchantLevel = ench; ItemState = state; Period = period; ExpirationDate = exp;
        GoldTicketKeyUid = gold; ExpandedSocketNum = expSock;
        return true;
    }
}

public sealed class KInventoryItemInfo : IKSerializable
{
    public long ItemUid { get; set; }
    public sbyte SlotCategory { get; set; }
    public short SlotId { get; set; }
    public KItemInfo ItemInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(ItemUid) && ks.Put(SlotCategory) && ks.Put(SlotId) && ks.Put(ItemInfo);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.Get(out sbyte cat) || !ks.Get(out short slotId) || !ks.Get(ItemInfo))
            return false;
        ItemUid = uid; SlotCategory = cat; SlotId = slotId;
        return true;
    }
}

public sealed class KInventoryItemSimpleInfo : IKSerializable
{
    public long ItemUid { get; set; }
    public int ItemId { get; set; }
    public sbyte SlotCategory { get; set; }
    public short SlotId { get; set; }
    public sbyte EnchantLevel { get; set; }
    public KItemAttributeEnchantInfo AttribEnchantInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(ItemUid) && ks.Put(ItemId) && ks.Put(SlotCategory) && ks.Put(SlotId) &&
        ks.Put(EnchantLevel) && ks.Put(AttribEnchantInfo);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.Get(out int id) || !ks.Get(out sbyte cat) ||
            !ks.Get(out short slot) || !ks.Get(out sbyte ench) || !ks.Get(AttribEnchantInfo))
            return false;
        ItemUid = uid; ItemId = id; SlotCategory = cat; SlotId = slot; EnchantLevel = ench;
        return true;
    }
}

public sealed class KSkillData : IKSerializable
{
    public short SkillId { get; set; }
    public byte SkillLevel { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(SkillId) && ks.Put(SkillLevel);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out short id) || !ks.Get(out byte lvl)) return false;
        SkillId = id; SkillLevel = lvl;
        return true;
    }
}

public sealed class KUnitSkillData : IKSerializable
{
    public KSkillData[] EquippedSkill { get; } = [new(), new(), new(), new()];
    public KSkillData[] EquippedSkillSlotB { get; } = [new(), new(), new(), new()];
    public string SkillSlotBEndDate { get; set; } = string.Empty;
    public sbyte SkillSlotBExpirationState { get; set; }
    public List<KSkillData> PassiveSkill { get; set; } = new();
    public List<KSkillData> GuildPassiveSkill { get; set; } = new();
    public List<int> SkillNote { get; set; } = new();
    public byte ActiveSkillPagesNumber { get; set; } = 1;
    public byte TheNumberOfSkillPagesAvailable { get; set; } = 1;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        for (int i = 0; i < 4; i++) if (!ks.Put(EquippedSkill[i])) return false;
        for (int i = 0; i < 4; i++) if (!ks.Put(EquippedSkillSlotB[i])) return false;
        return ks.PutWideString(SkillSlotBEndDate) &&
               ks.Put(SkillSlotBExpirationState) &&
               ks.PutList(PassiveSkill) &&
               ks.PutList(GuildPassiveSkill) &&
               ks.PutList(SkillNote, static (s, v) => s.Put(v)) &&
               ks.Put(ActiveSkillPagesNumber) &&
               ks.Put(TheNumberOfSkillPagesAvailable);
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        for (int i = 0; i < 4; i++) if (!ks.Get(EquippedSkill[i])) return false;
        for (int i = 0; i < 4; i++) if (!ks.Get(EquippedSkillSlotB[i])) return false;
        if (!ks.GetWideString(out string endDate) || !ks.Get(out sbyte expState) ||
            !ks.GetList(PassiveSkill) || !ks.GetList(GuildPassiveSkill) ||
            !ks.GetList(SkillNote, static s => (s.Get(out int v), v)) ||
            !ks.Get(out byte activePage) || !ks.Get(out byte availPages))
            return false;
        SkillSlotBEndDate = endDate;
        SkillSlotBExpirationState = expState;
        ActiveSkillPagesNumber = activePage;
        TheNumberOfSkillPagesAvailable = availPages;
        return true;
    }
}

public sealed class KUserGuildInfo : IKSerializable
{
    public int GuildUid { get; set; }
    public string GuildName { get; set; } = string.Empty;
    public byte MemberShipGrade { get; set; }
    public int HonorPoint { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(GuildUid) && ks.PutWideString(GuildName) && ks.Put(MemberShipGrade) && ks.Put(HonorPoint);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int uid) || !ks.GetWideString(out string name) ||
            !ks.Get(out byte grade) || !ks.Get(out int honor)) return false;
        GuildUid = uid; GuildName = name; MemberShipGrade = grade; HonorPoint = honor;
        return true;
    }
}

public sealed class KLastPositionInfo : IKSerializable
{
    public int MapId { get; set; } = 20001; // Ruben Village
    public byte LastTouchLineIndex { get; set; }
    public ushort LastPosValue { get; set; }
    public bool IgnoreLastTouch { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(MapId) && ks.Put(LastTouchLineIndex) && ks.Put(LastPosValue) && ks.Put(IgnoreLastTouch);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int map) || !ks.Get(out byte line) || !ks.Get(out ushort pos) || !ks.Get(out bool ign))
            return false;
        MapId = map; LastTouchLineIndex = line; LastPosValue = pos; IgnoreLastTouch = ign;
        return true;
    }
}

public sealed class KDungeonClearInfo : IKSerializable
{
    public int DungeonId { get; set; }
    public int MaxScore { get; set; }
    public sbyte MaxTotalRank { get; set; }
    public string ClearTime { get; set; } = string.Empty;
    public bool New { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(DungeonId) && ks.Put(MaxScore) && ks.Put(MaxTotalRank) && ks.PutWideString(ClearTime) && ks.Put(New);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int id) || !ks.Get(out int score) || !ks.Get(out sbyte rank) ||
            !ks.GetWideString(out string ct) || !ks.Get(out bool nw)) return false;
        DungeonId = id; MaxScore = score; MaxTotalRank = rank; ClearTime = ct; New = nw;
        return true;
    }
}

public sealed class KTCClearInfo : IKSerializable
{
    public int TcId { get; set; }
    public string ClearTime { get; set; } = string.Empty;
    public bool New { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(TcId) && ks.PutWideString(ClearTime) && ks.Put(New);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int id) || !ks.GetWideString(out string ct) || !ks.Get(out bool nw)) return false;
        TcId = id; ClearTime = ct; New = nw;
        return true;
    }
}

public sealed class KDungeonPlayInfo : IKSerializable
{
    public int DungeonId { get; set; }
    public int PlayTimes { get; set; }
    public int ClearTimes { get; set; }
    public bool New { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(DungeonId) && ks.Put(PlayTimes) && ks.Put(ClearTimes) && ks.Put(New);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int id) || !ks.Get(out int pt) || !ks.Get(out int ct) || !ks.Get(out bool nw)) return false;
        DungeonId = id; PlayTimes = pt; ClearTimes = ct; New = nw;
        return true;
    }
}

public sealed class KUnitInfo : IKSerializable
{
    public long OwnerUserUid { get; set; }
    public sbyte AuthLevel { get; set; }
    public long UnitUid { get; set; }
    public uint KnmSerialNum { get; set; }
    public sbyte UnitClass { get; set; } = 1; // Elsword
    public string NickName { get; set; } = string.Empty;
    public string Ip { get; set; } = "127.0.0.1";
    public ushort Port { get; set; }
    public int Ed { get; set; } = 100000;
    public byte Level { get; set; } = 1;
    public int Exp { get; set; }
    public int OfficialMatchCnt { get; set; }
    public int Rating { get; set; } = 1000;
    public int MaxRating { get; set; } = 1000;
    public int RPoint { get; set; }
    public int APoint { get; set; }
    public bool IsWinBeforeMatch { get; set; }
    public sbyte Rank { get; set; }
    public float KFactor { get; set; } = 1.0f;
    public bool IsRedistributionUser { get; set; }
    public int PastSeasonWin { get; set; }
    public int SPoint { get; set; }
    public int CsPoint { get; set; }
    public int MaxCsPoint { get; set; }
    public string CsPointEndDate { get; set; } = string.Empty;
    public int NowBaseLevelExp { get; set; }
    public int NextBaseLevelExp { get; set; } = 1000;
    public int StraightVictories { get; set; }
    public KStat Stat { get; set; } = new();
    public KStat GameStat { get; set; } = new();
    public KLastPositionInfo LastPos { get; set; } = new();
    public List<KBuffInfo> BuffInfo { get; set; } = new();
    public int Win { get; set; }
    public int Lose { get; set; }
    public Dictionary<int, KDungeonClearInfo> DungeonClear { get; set; } = new();
    public Dictionary<int, KTCClearInfo> TcClear { get; set; } = new();
    public Dictionary<int, KDungeonPlayInfo> DungeonPlay { get; set; } = new();
    public Dictionary<int, KInventoryItemInfo> EquippedItem { get; set; } = new();
    public KUnitSkillData UnitSkillData { get; set; } = new();
    public bool IsParty { get; set; }
    public int SpiritMax { get; set; } = 100;
    public int Spirit { get; set; } = 100;
    public bool IsGameBang { get; set; }
    public int PcBangType { get; set; } = -1;
    public int TitleId { get; set; }
    public KUserGuildInfo UserGuildInfo { get; set; } = new();
    public string LastLoginTime { get; set; } = string.Empty;
    public long WarpVipEndDate { get; set; }
    public int EventQuestClearCount { get; set; }
    public int ExchangeCount { get; set; }
    public int AccumulationSpirit { get; set; }
    public byte OldYearMissionRewardedLevel { get; set; }
    public int NewYearMissionStepId { get; set; } = -1;

    // TW/HK
    public byte CheckPowerCount { get; set; }
    public long CheckPowerTime { get; set; }
    public bool CheckPowerShowPopUp { get; set; }
    public byte CheckPowerScore { get; set; }

    // CN
    public string LastDate { get; set; } = string.Empty;
    public bool Deleted { get; set; }
    public long DelAbleDate { get; set; }
    public long RestoreAbleDate { get; set; }

    // JP
    public int AccountPvpLoseCount { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(OwnerUserUid) || !ks.Put(AuthLevel) || !ks.Put(UnitUid) || !ks.Put(KnmSerialNum) ||
            !ks.Put(UnitClass) || !ks.PutWideString(NickName) || !ks.PutWideString(Ip) || !ks.Put(Port) ||
            !ks.Put(Ed) || !ks.Put(Level) || !ks.Put(Exp) || !ks.Put(OfficialMatchCnt) || !ks.Put(Rating) ||
            !ks.Put(MaxRating) || !ks.Put(RPoint) || !ks.Put(APoint) || !ks.Put(IsWinBeforeMatch) ||
            !ks.Put(Rank) || !ks.Put(KFactor) || !ks.Put(IsRedistributionUser) || !ks.Put(PastSeasonWin) ||
            !ks.Put(SPoint) || !ks.Put(CsPoint) || !ks.Put(MaxCsPoint) || !ks.PutWideString(CsPointEndDate) ||
            !ks.Put(NowBaseLevelExp) || !ks.Put(NextBaseLevelExp) || !ks.Put(StraightVictories) ||
            !ks.Put(Stat) || !ks.Put(GameStat) || !ks.Put(LastPos) ||
            !ks.PutList(BuffInfo) ||
            !ks.Put(Win) || !ks.Put(Lose) ||
            !ks.PutMap(DungeonClear, static (s, k) => s.Put(k), static (s, v) => s.Put(v)) ||
            !ks.PutMap(TcClear, static (s, k) => s.Put(k), static (s, v) => s.Put(v)) ||
            !ks.PutMap(DungeonPlay, static (s, k) => s.Put(k), static (s, v) => s.Put(v)) ||
            !ks.PutMap(EquippedItem, static (s, k) => s.Put(k), static (s, v) => s.Put(v)) ||
            !ks.Put(UnitSkillData) || !ks.Put(IsParty) || !ks.Put(SpiritMax) || !ks.Put(Spirit) ||
            !ks.Put(IsGameBang) || !ks.Put(PcBangType) || !ks.Put(TitleId) || !ks.Put(UserGuildInfo) ||
            !ks.PutWideString(LastLoginTime))
            return false;

        // Preserve the C++ CommonPacket.cpp feature-field order. The options are
        // independent: JP has check-power and anniversary fields.
        if (region.UnitWaitDelete &&
            (!ks.PutWideString(LastDate) || !ks.Put(Deleted) ||
             !ks.Put(DelAbleDate) || !ks.Put(RestoreAbleDate))) return false;
        if (!ks.Put(WarpVipEndDate) || !ks.Put(EventQuestClearCount) || !ks.Put(ExchangeCount)) return false;
        if (region.AccumulationSpiritSystem)
            if (!ks.Put(AccumulationSpirit)) return false;
        if (!ks.Put(OldYearMissionRewardedLevel) || !ks.Put(NewYearMissionStepId)) return false;
        if (region.Region is RegionCode.TW or RegionCode.HK or RegionCode.JP)
            if (!ks.Put(CheckPowerCount) || !ks.Put(CheckPowerTime) ||
                !ks.Put(CheckPowerShowPopUp) || !ks.Put(CheckPowerScore)) return false;
        if (region.Region == RegionCode.JP && !ks.Put(AccountPvpLoseCount)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long ownerUid) || !ks.Get(out sbyte auth) || !ks.Get(out long unitUid) || !ks.Get(out uint knm) ||
            !ks.Get(out sbyte unitClass) || !ks.GetWideString(out string nick) || !ks.GetWideString(out string ip) || !ks.Get(out ushort port) ||
            !ks.Get(out int ed) || !ks.Get(out byte lvl) || !ks.Get(out int exp) || !ks.Get(out int omc) || !ks.Get(out int rat) ||
            !ks.Get(out int maxRat) || !ks.Get(out int rp) || !ks.Get(out int ap) || !ks.Get(out bool winBefore) ||
            !ks.Get(out sbyte rank) || !ks.Get(out float kf) || !ks.Get(out bool redist) || !ks.Get(out int psw) ||
            !ks.Get(out int sp) || !ks.Get(out int csp) || !ks.Get(out int mcsp) || !ks.GetWideString(out string csend) ||
            !ks.Get(out int nowExp) || !ks.Get(out int nextExp) || !ks.Get(out int sv) ||
            !ks.Get(Stat) || !ks.Get(GameStat) || !ks.Get(LastPos) ||
            !ks.GetList(BuffInfo) ||
            !ks.Get(out int win) || !ks.Get(out int lose) ||
            !ks.GetMap(DungeonClear, static s => (s.Get(out int k), k), static s => { var v = new KDungeonClearInfo(); return (s.Get(v), v); }) ||
            !ks.GetMap(TcClear, static s => (s.Get(out int k), k), static s => { var v = new KTCClearInfo(); return (s.Get(v), v); }) ||
            !ks.GetMap(DungeonPlay, static s => (s.Get(out int k), k), static s => { var v = new KDungeonPlayInfo(); return (s.Get(v), v); }) ||
            !ks.GetMap(EquippedItem, static s => (s.Get(out int k), k), static s => { var v = new KInventoryItemInfo(); return (s.Get(v), v); }) ||
            !ks.Get(UnitSkillData) || !ks.Get(out bool isParty) || !ks.Get(out int spMax) || !ks.Get(out int spCur) ||
            !ks.Get(out bool gb) || !ks.Get(out int pcb) || !ks.Get(out int titleId) || !ks.Get(UserGuildInfo) ||
            !ks.GetWideString(out string llt))
            return false;

        if (region.UnitWaitDelete)
        {
            if (!ks.GetWideString(out string ld) || !ks.Get(out bool del) ||
                !ks.Get(out long dad) || !ks.Get(out long rad)) return false;
            LastDate = ld; Deleted = del; DelAbleDate = dad; RestoreAbleDate = rad;
        }
        if (!ks.Get(out long wvip) || !ks.Get(out int evCnt) || !ks.Get(out int exCnt)) return false;
        if (region.AccumulationSpiritSystem)
        {
            if (!ks.Get(out int spirit)) return false;
            AccumulationSpirit = spirit;
        }
        if (!ks.Get(out byte oldYr) || !ks.Get(out int newYr)) return false;
        if (region.Region is RegionCode.TW or RegionCode.HK or RegionCode.JP)
        {
            if (!ks.Get(out byte cpc) || !ks.Get(out long cpt) || !ks.Get(out bool cpp) || !ks.Get(out byte cps)) return false;
            CheckPowerCount = cpc; CheckPowerTime = cpt; CheckPowerShowPopUp = cpp; CheckPowerScore = cps;
        }
        if (region.Region == RegionCode.JP)
        {
            if (!ks.Get(out int aplc)) return false;
            AccountPvpLoseCount = aplc;
        }

        OwnerUserUid = ownerUid; AuthLevel = auth; UnitUid = unitUid; KnmSerialNum = knm;
        UnitClass = unitClass; NickName = nick; Ip = ip; Port = port; Ed = ed; Level = lvl; Exp = exp;
        OfficialMatchCnt = omc; Rating = rat; MaxRating = maxRat; RPoint = rp; APoint = ap;
        IsWinBeforeMatch = winBefore; Rank = rank; KFactor = kf; IsRedistributionUser = redist;
        PastSeasonWin = psw; SPoint = sp; CsPoint = csp; MaxCsPoint = mcsp; CsPointEndDate = csend;
        NowBaseLevelExp = nowExp; NextBaseLevelExp = nextExp; StraightVictories = sv;
        Win = win; Lose = lose; IsParty = isParty; SpiritMax = spMax; Spirit = spCur;
        IsGameBang = gb; PcBangType = pcb; TitleId = titleId; LastLoginTime = llt; WarpVipEndDate = wvip;
        EventQuestClearCount = evCnt; ExchangeCount = exCnt; OldYearMissionRewardedLevel = oldYr; NewYearMissionStepId = newYr;
        return true;
    }
}

public sealed class KRoomUserInfo : IKSerializable
{
    public long GsUid { get; set; }
    public long OwnerUserUid { get; set; }
    public short ServerGroupId { get; set; }
    public sbyte AuthLevel { get; set; }
    public bool Male { get; set; } = true;
    public byte Age { get; set; } = 20;
    public long UnitUid { get; set; }
    public uint KnmSerialNum { get; set; }
    public sbyte UnitClass { get; set; } = 1;
    public string NickName { get; set; } = string.Empty;
    public int NumResurrectionStone { get; set; } = 10;
    public string Ip { get; set; } = "127.0.0.1";
    public ushort Port { get; set; }
    public string InternalIp { get; set; } = "127.0.0.1";
    public ushort InternalPort { get; set; }
    public long PartyUid { get; set; }
    public List<KGamePlayStatus> GamePlayStatus { get; set; } = new();
    public List<KBuffInfo> BuffInfo { get; set; } = new();
    public byte Level { get; set; } = 1;
    public KStat GameStat { get; set; } = new();
    public Dictionary<int, KInventoryItemInfo> EquippedItem { get; set; } = new();
    public Dictionary<int, KInventoryItemInfo> SpecialItem { get; set; } = new();
    public KUnitSkillData UnitSkillData { get; set; } = new();
    public bool IsPvpNpc { get; set; }
    public int OfficialMatchCnt { get; set; }
    public int Rating { get; set; } = 1000;
    public int MaxRating { get; set; } = 1000;
    public bool IsWinBeforeMatch { get; set; }
    public sbyte Rank { get; set; }
    public sbyte RankForServer { get; set; }
    public float KFactor { get; set; } = 1.0f;
    public bool IsRedistributionUser { get; set; }
    public bool IsGameBang { get; set; }
    public int PcBangType { get; set; } = -1;
    public bool IsObserver { get; set; }
    public Dictionary<int, float> BonusRate { get; set; } = new();
    public bool IsRingOfPvpRebirth { get; set; }
    public bool IsGuestUser { get; set; }
    public Dictionary<int, KSubQuestInfo> OngoingQuest { get; set; } = new();
    public int TitleId { get; set; }
    public string GuildName { get; set; } = string.Empty;
    public int GuildUid { get; set; }
    public byte MemberShipGrade { get; set; }
    public List<KPetInfo> Pet { get; set; } = new();
    public HashSet<int> QuestInfo { get; set; } = [];
    public bool UseItem { get; set; }
    public HashSet<int> GoingQuestInfo { get; set; } = [];
    public bool ComeBackUser { get; set; }
    public bool HenirReward { get; set; }
    public HashSet<int> UseSkillBuffInPlay { get; set; } = [];
    public bool EnterCashShop { get; set; }
    public long RidingPetUid { get; set; }
    public ushort RidingPetId { get; set; }
    public sbyte WeddingStatus { get; set; }
    public long LoverUnitUid { get; set; }
    public int EventQuestClearCount { get; set; }
    public int ExchangeCount { get; set; }

    // CN & JP extensions
    public bool Couple { get; set; }
    public long RelationTargetUserUid { get; set; }
    public string RelationTargetUserNickname { get; set; } = string.Empty;
    public int AccountPvpLoseCount { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(GsUid) || !ks.Put(OwnerUserUid) || !ks.Put(ServerGroupId) || !ks.Put(AuthLevel) ||
            !ks.Put(Male) || !ks.Put(Age) || !ks.Put(UnitUid) || !ks.Put(KnmSerialNum) ||
            !ks.Put(UnitClass) || !ks.PutWideString(NickName) || !ks.Put(NumResurrectionStone) ||
            !ks.PutWideString(Ip) || !ks.Put(Port) || !ks.PutWideString(InternalIp) || !ks.Put(InternalPort) ||
            !ks.Put(PartyUid) || !ks.PutList(GamePlayStatus) || !ks.PutList(BuffInfo) ||
            !ks.Put(Level) || !ks.Put(GameStat) ||
            !ks.PutMap(EquippedItem, static (s, k) => s.Put(k), static (s, v) => s.Put(v)) ||
            !ks.PutMap(SpecialItem, static (s, k) => s.Put(k), static (s, v) => s.Put(v)) ||
            !ks.Put(UnitSkillData) || !ks.Put(IsPvpNpc) || !ks.Put(OfficialMatchCnt) ||
            !ks.Put(Rating) || !ks.Put(MaxRating) || !ks.Put(IsWinBeforeMatch) ||
            !ks.Put(Rank) || !ks.Put(RankForServer) || !ks.Put(KFactor) || !ks.Put(IsRedistributionUser) ||
            !ks.Put(IsGameBang) || !ks.Put(PcBangType) || !ks.Put(IsObserver) ||
            !ks.PutMap(BonusRate, static (s, k) => s.Put(k), static (s, v) => s.Put(v)) ||
            !ks.Put(IsRingOfPvpRebirth) || !ks.Put(IsGuestUser) ||
            !ks.PutMap(OngoingQuest, static (s, k) => s.Put(k), static (s, v) => s.Put(v)) ||
            !ks.Put(TitleId) || !ks.PutWideString(GuildName) || !ks.Put(GuildUid) || !ks.Put(MemberShipGrade) ||
            !ks.PutList(Pet) || !ks.PutList(QuestInfo, static (s, v) => s.Put(v)) ||
            !ks.Put(UseItem) || !ks.PutList(GoingQuestInfo, static (s, v) => s.Put(v)) ||
            !ks.Put(ComeBackUser) || !ks.Put(HenirReward) || !ks.PutList(UseSkillBuffInPlay, static (s, v) => s.Put(v)) ||
            !ks.Put(EnterCashShop) || !ks.Put(RidingPetUid) || !ks.Put(RidingPetId) ||
            !ks.Put(WeddingStatus) || !ks.Put(LoverUnitUid) || !ks.Put(EventQuestClearCount) || !ks.Put(ExchangeCount))
            return false;

        if (region.Region == RegionCode.CN)
            return ks.Put(Couple) && ks.Put(RelationTargetUserUid) && ks.PutWideString(RelationTargetUserNickname);
        if (region.Region == RegionCode.JP)
            return ks.Put(AccountPvpLoseCount);
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long gs) || !ks.Get(out long own) || !ks.Get(out short sg) || !ks.Get(out sbyte auth) ||
            !ks.Get(out bool male) || !ks.Get(out byte age) || !ks.Get(out long unitUid) || !ks.Get(out uint knm) ||
            !ks.Get(out sbyte uc) || !ks.GetWideString(out string nick) || !ks.Get(out int resStone) ||
            !ks.GetWideString(out string ip) || !ks.Get(out ushort port) || !ks.GetWideString(out string iip) || !ks.Get(out ushort iport) ||
            !ks.Get(out long puid) || !ks.GetList(GamePlayStatus) || !ks.GetList(BuffInfo) ||
            !ks.Get(out byte lvl) || !ks.Get(GameStat) ||
            !ks.GetMap(EquippedItem, static s => (s.Get(out int k), k), static s => { var v = new KInventoryItemInfo(); return (s.Get(v), v); }) ||
            !ks.GetMap(SpecialItem, static s => (s.Get(out int k), k), static s => { var v = new KInventoryItemInfo(); return (s.Get(v), v); }) ||
            !ks.Get(UnitSkillData) || !ks.Get(out bool npc) || !ks.Get(out int omc) ||
            !ks.Get(out int rat) || !ks.Get(out int mrat) || !ks.Get(out bool wbm) ||
            !ks.Get(out sbyte rank) || !ks.Get(out sbyte rfs) || !ks.Get(out float kf) || !ks.Get(out bool redist) ||
            !ks.Get(out bool gb) || !ks.Get(out int pcb) || !ks.Get(out bool obs) ||
            !ks.GetMap(BonusRate, static s => (s.Get(out int k), k), static s => (s.Get(out float v), v)) ||
            !ks.Get(out bool rpr) || !ks.Get(out bool guest) ||
            !ks.GetMap(OngoingQuest, static s => (s.Get(out int k), k), static s => { var v = new KSubQuestInfo(); return (s.Get(v), v); }) ||
            !ks.Get(out int tid) || !ks.GetWideString(out string gname) || !ks.Get(out int guid) || !ks.Get(out byte mgrade) ||
            !ks.GetList(Pet) || !ks.GetHashSet(QuestInfo, static s => (s.Get(out int v), v)) ||
            !ks.Get(out bool uitem) || !ks.GetHashSet(GoingQuestInfo, static s => (s.Get(out int v), v)) ||
            !ks.Get(out bool cbu) || !ks.Get(out bool hr) || !ks.GetHashSet(UseSkillBuffInPlay, static s => (s.Get(out int v), v)) ||
            !ks.Get(out bool ecs) || !ks.Get(out long rpu) || !ks.Get(out ushort rpi) ||
            !ks.Get(out sbyte ws) || !ks.Get(out long luid) || !ks.Get(out int eqcc) || !ks.Get(out int exc))
            return false;

        if (region.Region == RegionCode.CN)
        {
            if (!ks.Get(out bool cpl) || !ks.Get(out long rtuid) || !ks.GetWideString(out string rtnick)) return false;
            Couple = cpl; RelationTargetUserUid = rtuid; RelationTargetUserNickname = rtnick;
        }
        else if (region.Region == RegionCode.JP)
        {
            if (!ks.Get(out int aplc)) return false;
            AccountPvpLoseCount = aplc;
        }

        GsUid = gs; OwnerUserUid = own; ServerGroupId = sg; AuthLevel = auth; Male = male; Age = age;
        UnitUid = unitUid; KnmSerialNum = knm; UnitClass = uc; NickName = nick; NumResurrectionStone = resStone;
        Ip = ip; Port = port; InternalIp = iip; InternalPort = iport; PartyUid = puid; Level = lvl;
        IsPvpNpc = npc; OfficialMatchCnt = omc; Rating = rat; MaxRating = mrat; IsWinBeforeMatch = wbm;
        Rank = rank; RankForServer = rfs; KFactor = kf; IsRedistributionUser = redist; IsGameBang = gb;
        PcBangType = pcb; IsObserver = obs; IsRingOfPvpRebirth = rpr; IsGuestUser = guest; TitleId = tid;
        GuildName = gname; GuildUid = guid; MemberShipGrade = mgrade; UseItem = uitem; ComeBackUser = cbu;
        HenirReward = hr; EnterCashShop = ecs; RidingPetUid = rpu; RidingPetId = rpi; WeddingStatus = ws;
        LoverUnitUid = luid; EventQuestClearCount = eqcc; ExchangeCount = exc;
        return true;
    }
}

public sealed class KRoomSlotInfo : IKSerializable
{
    public sbyte Index { get; set; }
    public sbyte SlotState { get; set; }
    public bool IsHost { get; set; }
    public bool IsReady { get; set; }
    public bool IsPitIn { get; set; }
    public bool IsTrade { get; set; }
    public int TeamNum { get; set; }
    public KRoomUserInfo RoomUserInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Index) && ks.Put(SlotState) && ks.Put(IsHost) && ks.Put(IsReady) &&
        ks.Put(IsPitIn) && ks.Put(IsTrade) && ks.Put(TeamNum) && ks.Put(RoomUserInfo);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte idx) || !ks.Get(out sbyte state) || !ks.Get(out bool host) || !ks.Get(out bool ready) ||
            !ks.Get(out bool pitIn) || !ks.Get(out bool trade) || !ks.Get(out int team) || !ks.Get(RoomUserInfo))
            return false;
        Index = idx; SlotState = state; IsHost = host; IsReady = ready;
        IsPitIn = pitIn; IsTrade = trade; TeamNum = team;
        return true;
    }
}

public sealed class KRoomInfo : IKSerializable
{
    public sbyte RoomType { get; set; }
    public long RoomUid { get; set; }
    public uint RoomListId { get; set; }
    public string RoomName { get; set; } = string.Empty;
    public sbyte RoomState { get; set; }
    public bool Public { get; set; } = true;
    public bool TeamBalance { get; set; }
    public string Password { get; set; } = string.Empty;
    public sbyte MaxSlot { get; set; } = 4;
    public sbyte JoinSlot { get; set; }
    public string UdpRelayIp { get; set; } = "127.0.0.1";
    public ushort UdpRelayPort { get; set; } = 9303;
    public bool StartedByAutoParty { get; set; }
    public sbyte PvpGameType { get; set; }
    public bool IsItemMode { get; set; }
    public int PvpChannelClass { get; set; }
    public sbyte WinKillNum { get; set; } = 3;
    public bool CanIntrude { get; set; } = true;
    public float PlayTime { get; set; } = 300.0f;
    public short WorldId { get; set; }
    public short ShowPvpMapWorldId { get; set; }
    public sbyte DifficultyLevel { get; set; }
    public int DungeonId { get; set; } = 30000;
    public sbyte GetItemType { get; set; }
    public sbyte DungeonMode { get; set; }
    public long PartyUid { get; set; }
    public int BattleFieldId { get; set; }
    public Dictionary<long, short> AllPlayersSelectedMap { get; set; } = new();
    public bool DefenceDungeonOpen { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(RoomType) && ks.Put(RoomUid) && ks.Put(RoomListId) && ks.PutWideString(RoomName) &&
        ks.Put(RoomState) && ks.Put(Public) && ks.Put(TeamBalance) && ks.PutWideString(Password) &&
        ks.Put(MaxSlot) && ks.Put(JoinSlot) && ks.PutWideString(UdpRelayIp) && ks.Put(UdpRelayPort) &&
        ks.Put(StartedByAutoParty) && ks.Put(PvpGameType) && ks.Put(IsItemMode) && ks.Put(PvpChannelClass) &&
        ks.Put(WinKillNum) && ks.Put(CanIntrude) && ks.Put(PlayTime) && ks.Put(WorldId) &&
        ks.Put(ShowPvpMapWorldId) && ks.Put(DifficultyLevel) && ks.Put(DungeonId) && ks.Put(GetItemType) &&
        ks.Put(DungeonMode) && ks.Put(PartyUid) && ks.Put(BattleFieldId) &&
        ks.PutMap(AllPlayersSelectedMap, static (s, k) => s.Put(k), static (s, v) => s.Put(v)) &&
        ks.Put(DefenceDungeonOpen);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte rt) || !ks.Get(out long ruid) || !ks.Get(out uint rlid) || !ks.GetWideString(out string rname) ||
            !ks.Get(out sbyte rstate) || !ks.Get(out bool pub) || !ks.Get(out bool tb) || !ks.GetWideString(out string pwd) ||
            !ks.Get(out sbyte mslot) || !ks.Get(out sbyte jslot) || !ks.GetWideString(out string rip) || !ks.Get(out ushort rport) ||
            !ks.Get(out bool sba) || !ks.Get(out sbyte pgt) || !ks.Get(out bool im) || !ks.Get(out int pcc) ||
            !ks.Get(out sbyte wkn) || !ks.Get(out bool ci) || !ks.Get(out float pt) || !ks.Get(out short wid) ||
            !ks.Get(out short swid) || !ks.Get(out sbyte diff) || !ks.Get(out int did) || !ks.Get(out sbyte git) ||
            !ks.Get(out sbyte dm) || !ks.Get(out long puid) || !ks.Get(out int bfid) ||
            !ks.GetMap(AllPlayersSelectedMap, static s => (s.Get(out long k), k), static s => (s.Get(out short v), v)) ||
            !ks.Get(out bool ddo))
            return false;

        RoomType = rt; RoomUid = ruid; RoomListId = rlid; RoomName = rname; RoomState = rstate;
        Public = pub; TeamBalance = tb; Password = pwd; MaxSlot = mslot; JoinSlot = jslot;
        UdpRelayIp = rip; UdpRelayPort = rport; StartedByAutoParty = sba; PvpGameType = pgt;
        IsItemMode = im; PvpChannelClass = pcc; WinKillNum = wkn; CanIntrude = ci; PlayTime = pt;
        WorldId = wid; ShowPvpMapWorldId = swid; DifficultyLevel = diff; DungeonId = did;
        GetItemType = git; DungeonMode = dm; PartyUid = puid; BattleFieldId = bfid; DefenceDungeonOpen = ddo;
        return true;
    }
}

public sealed class KFieldFrameInfo : IKSerializable
{
    public int StateId { get; set; }
    public ushort FrameCount { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(StateId) && ks.Put(FrameCount);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int sid) || !ks.Get(out ushort fc)) return false;
        StateId = sid; FrameCount = fc;
        return true;
    }
}

public sealed class KFieldPetInfo : IKSerializable
{
    public long PetUid { get; set; }
    public int PetId { get; set; }
    public string PetName { get; set; } = string.Empty;
    public sbyte EvolutionStep { get; set; }
    public int Intimacy { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(PetUid) && ks.Put(PetId) && ks.PutWideString(PetName) && ks.Put(EvolutionStep) && ks.Put(Intimacy);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.Get(out int id) || !ks.GetWideString(out string name) ||
            !ks.Get(out sbyte step) || !ks.Get(out int intim)) return false;
        PetUid = uid; PetId = id; PetName = name; EvolutionStep = step; Intimacy = intim;
        return true;
    }
}

public sealed class KFieldUserInfo : IKSerializable
{
    public long UnitUid { get; set; }
    public string NickName { get; set; } = string.Empty;
    public sbyte UnitClass { get; set; } = 1;
    public byte Level { get; set; } = 1;
    public sbyte Rank { get; set; }
    public List<KInventoryItemSimpleInfo> EquippedItem { get; set; } = new();
    public int TitleId { get; set; }
    public KVector3 Pos { get; set; } = new();
    public long PartyUid { get; set; }
    public string GuildName { get; set; } = string.Empty;
    public List<KFieldPetInfo> Pets { get; set; } = new();
    public int Ed { get; set; }
    public bool Invisible { get; set; }
    public long RidingPetUid { get; set; }
    public ushort RidingPetId { get; set; }
    public long LoverUnitUid { get; set; }
    public int EventQuestClearCount { get; set; }
    public int ExchangeCount { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(UnitUid) && ks.PutWideString(NickName) && ks.Put(UnitClass) && ks.Put(Level) && ks.Put(Rank) &&
        ks.PutList(EquippedItem) && ks.Put(TitleId) && ks.Put(Pos) && ks.Put(PartyUid) &&
        ks.PutWideString(GuildName) && ks.PutList(Pets) && ks.Put(Ed) && ks.Put(Invisible) &&
        ks.Put(RidingPetUid) && ks.Put(RidingPetId) && ks.Put(LoverUnitUid) &&
        ks.Put(EventQuestClearCount) && ks.Put(ExchangeCount);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.GetWideString(out string nick) || !ks.Get(out sbyte uc) ||
            !ks.Get(out byte lvl) || !ks.Get(out sbyte rank) || !ks.GetList(EquippedItem) ||
            !ks.Get(out int tid) || !ks.Get(Pos) || !ks.Get(out long puid) || !ks.GetWideString(out string gname) ||
            !ks.GetList(Pets) || !ks.Get(out int ed) || !ks.Get(out bool inv) || !ks.Get(out long rpu) ||
            !ks.Get(out ushort rpi) || !ks.Get(out long luid) || !ks.Get(out int eqcc) || !ks.Get(out int exc))
            return false;
        UnitUid = uid; NickName = nick; UnitClass = uc; Level = lvl; Rank = rank; TitleId = tid;
        PartyUid = puid; GuildName = gname; Ed = ed; Invisible = inv; RidingPetUid = rpu;
        RidingPetId = rpi; LoverUnitUid = luid; EventQuestClearCount = eqcc; ExchangeCount = exc;
        return true;
    }
}

public sealed class KCreatePartyInfo : IKSerializable
{
    public sbyte PartyType { get; set; }
    public int DungeonId { get; set; } = 30000;
    public sbyte DifficultyLevel { get; set; }
    public sbyte GetItemType { get; set; }
    public sbyte DungeonMode { get; set; }
    public bool CheckLowLevel { get; set; }
    public sbyte PvpGameType { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(PartyType) && ks.Put(DungeonId) && ks.Put(DifficultyLevel) && ks.Put(GetItemType) &&
        ks.Put(DungeonMode) && ks.Put(CheckLowLevel) && ks.Put(PvpGameType);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte pt) || !ks.Get(out int did) || !ks.Get(out sbyte diff) ||
            !ks.Get(out sbyte git) || !ks.Get(out sbyte dm) || !ks.Get(out bool cll) || !ks.Get(out sbyte pgt))
            return false;
        PartyType = pt; DungeonId = did; DifficultyLevel = diff; GetItemType = git;
        DungeonMode = dm; CheckLowLevel = cll; PvpGameType = pgt;
        return true;
    }
}

public sealed class KPartyInfo : IKSerializable
{
    public const sbyte PtNone = 0;
    public const sbyte PtDungeonParty = 1;
    public const sbyte PtPvpParty = 2;

    public long PartyUid { get; set; }
    public sbyte PartyType { get; set; }
    public string PartyName { get; set; } = string.Empty;
    public sbyte JoinSlot { get; set; } = 1;
    public sbyte MaxSlot { get; set; } = 4;
    public bool Public { get; set; } = true;
    public int DungeonId { get; set; } = 30000;
    public sbyte DifficultyLevel { get; set; }
    public sbyte GetItemType { get; set; }
    public sbyte DungeonMode { get; set; }
    public short PartyFever { get; set; }
    public bool CheckLowLevel { get; set; }
    public sbyte PvpGameType { get; set; }
    public bool ComeBackBuff { get; set; }
    public string ComeBackBuffEnd { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(PartyUid) && ks.Put(PartyType) && ks.PutWideString(PartyName) && ks.Put(JoinSlot) &&
        ks.Put(MaxSlot) && ks.Put(Public) && ks.Put(DungeonId) && ks.Put(DifficultyLevel) &&
        ks.Put(GetItemType) && ks.Put(DungeonMode) && ks.Put(PartyFever) && ks.Put(CheckLowLevel) &&
        ks.Put(PvpGameType) && ks.Put(ComeBackBuff) && ks.PutWideString(ComeBackBuffEnd);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long puid) || !ks.Get(out sbyte pt) || !ks.GetWideString(out string pname) ||
            !ks.Get(out sbyte jslot) || !ks.Get(out sbyte mslot) || !ks.Get(out bool pub) ||
            !ks.Get(out int did) || !ks.Get(out sbyte diff) || !ks.Get(out sbyte git) ||
            !ks.Get(out sbyte dm) || !ks.Get(out short fever) || !ks.Get(out bool cll) ||
            !ks.Get(out sbyte pgt) || !ks.Get(out bool cbb) || !ks.GetWideString(out string cbbe))
            return false;
        PartyUid = puid; PartyType = pt; PartyName = pname; JoinSlot = jslot; MaxSlot = mslot;
        Public = pub; DungeonId = did; DifficultyLevel = diff; GetItemType = git; DungeonMode = dm;
        PartyFever = fever; CheckLowLevel = cll; PvpGameType = pgt; ComeBackBuff = cbb; ComeBackBuffEnd = cbbe;
        return true;
    }
}

public sealed class KPartyUserInfo : IKSerializable
{
    public long GsUid { get; set; }
    public int ChannelId { get; set; }
    public long FieldUid { get; set; }
    public long UnitUid { get; set; }
    public sbyte UnitClass { get; set; } = 1;
    public string NickName { get; set; } = string.Empty;
    public byte Level { get; set; } = 1;
    public bool IsGuestUser { get; set; }
    public sbyte State { get; set; }
    public int StateCode { get; set; }
    public bool Ready { get; set; }
    public bool Host { get; set; }
    public bool ComeBack { get; set; }
    public string ComeBackEnd { get; set; } = string.Empty;
    public sbyte Rank { get; set; }
    public int OfficialMatchCnt { get; set; }
    public int Rating { get; set; } = 1000;
    public int RPoint { get; set; }
    public KGamePlayStatus GamePlayStatus { get; set; } = new();
    public string Ip { get; set; } = "127.0.0.1";
    public ushort Port { get; set; }
    public uint PingScore { get; set; }
    public string InternalIp { get; set; } = "127.0.0.1";
    public ushort InternalPort { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(GsUid) && ks.Put(ChannelId) && ks.Put(FieldUid) && ks.Put(UnitUid) && ks.Put(UnitClass) &&
        ks.PutWideString(NickName) && ks.Put(Level) && ks.Put(IsGuestUser) && ks.Put(State) &&
        ks.Put(StateCode) && ks.Put(Ready) && ks.Put(Host) && ks.Put(ComeBack) &&
        ks.PutWideString(ComeBackEnd) && ks.Put(Rank) && ks.Put(OfficialMatchCnt) &&
        ks.Put(Rating) && ks.Put(RPoint) && ks.Put(GamePlayStatus) &&
        ks.PutWideString(Ip) && ks.Put(Port) && ks.Put(PingScore) &&
        ks.PutWideString(InternalIp) && ks.Put(InternalPort);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long gs) || !ks.Get(out int cid) || !ks.Get(out long fuid) || !ks.Get(out long uid) ||
            !ks.Get(out sbyte uc) || !ks.GetWideString(out string nick) || !ks.Get(out byte lvl) ||
            !ks.Get(out bool guest) || !ks.Get(out sbyte st) || !ks.Get(out int sc) ||
            !ks.Get(out bool ready) || !ks.Get(out bool host) || !ks.Get(out bool cb) ||
            !ks.GetWideString(out string cbe) || !ks.Get(out sbyte rank) || !ks.Get(out int omc) ||
            !ks.Get(out int rat) || !ks.Get(out int rp) || !ks.Get(GamePlayStatus) ||
            !ks.GetWideString(out string ip) || !ks.Get(out ushort port) || !ks.Get(out uint ping) ||
            !ks.GetWideString(out string iip) || !ks.Get(out ushort iport))
            return false;
        GsUid = gs; ChannelId = cid; FieldUid = fuid; UnitUid = uid; UnitClass = uc;
        NickName = nick; Level = lvl; IsGuestUser = guest; State = st; StateCode = sc;
        Ready = ready; Host = host; ComeBack = cb; ComeBackEnd = cbe; Rank = rank;
        OfficialMatchCnt = omc; Rating = rat; RPoint = rp; Ip = ip; Port = port;
        PingScore = ping; InternalIp = iip; InternalPort = iport;
        return true;
    }
}

public sealed class KPostItemInfo : IKSerializable
{
    public const sbyte LtNone = 0;
    public const sbyte LtPostOffice = 1;
    public const sbyte LtRecommend = 2;
    public const sbyte LtTutol = 3;
    public const sbyte LtEvent = 4;
    public const sbyte LtMessage = 5;
    public const sbyte LtRankingTitle = 6;
    public const sbyte LtBrokenItem = 7;

    public long PostNo { get; set; }
    public long FromUnitUid { get; set; }
    public long ToUnitUid { get; set; }
    public sbyte ScriptType { get; set; }
    public int ScriptIndex { get; set; }
    public int Quantity { get; set; }
    public sbyte EnchantLevel { get; set; }
    public KItemAttributeEnchantInfo AttribEnchantInfo { get; set; } = new();
    public List<int> ItemSocket { get; set; } = new();
    public List<int> RandomSocket { get; set; } = new();
    public sbyte ItemState { get; set; } = 1;
    public byte SealData { get; set; }
    public string RegDate { get; set; } = string.Empty;
    public string FromNickName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool Read { get; set; }
    public byte ExpandedSocketNum { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(PostNo) && ks.Put(FromUnitUid) && ks.Put(ToUnitUid) && ks.Put(ScriptType) &&
        ks.Put(ScriptIndex) && ks.Put(Quantity) && ks.Put(EnchantLevel) && ks.Put(AttribEnchantInfo) &&
        ks.PutList(ItemSocket, static (s, v) => s.Put(v)) &&
        ks.PutList(RandomSocket, static (s, v) => s.Put(v)) &&
        ks.Put(ItemState) && ks.Put(SealData) && ks.PutWideString(RegDate) &&
        ks.PutWideString(FromNickName) && ks.PutWideString(Title) && ks.PutWideString(Message) &&
        ks.Put(Read) && ks.Put(ExpandedSocketNum);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long pno) || !ks.Get(out long fuid) || !ks.Get(out long tuid) || !ks.Get(out sbyte st) ||
            !ks.Get(out int si) || !ks.Get(out int qty) || !ks.Get(out sbyte ench) || !ks.Get(AttribEnchantInfo) ||
            !ks.GetList(ItemSocket, static s => (s.Get(out int v), v)) ||
            !ks.GetList(RandomSocket, static s => (s.Get(out int v), v)) ||
            !ks.Get(out sbyte istate) || !ks.Get(out byte seal) || !ks.GetWideString(out string rdate) ||
            !ks.GetWideString(out string fnick) || !ks.GetWideString(out string title) ||
            !ks.GetWideString(out string msg) || !ks.Get(out bool rd) || !ks.Get(out byte esn))
            return false;
        PostNo = pno; FromUnitUid = fuid; ToUnitUid = tuid; ScriptType = st; ScriptIndex = si;
        Quantity = qty; EnchantLevel = ench; ItemState = istate; SealData = seal; RegDate = rdate;
        FromNickName = fnick; Title = title; Message = msg; Read = rd; ExpandedSocketNum = esn;
        return true;
    }
}

public sealed class KGuildInfo : IKSerializable
{
    public int GuildUid { get; set; }
    public string GuildName { get; set; } = string.Empty;
    public ushort MaxNumMember { get; set; } = 30;
    public byte GuildLevel { get; set; } = 1;
    public int GuildExp { get; set; }
    public string GuildMessage { get; set; } = string.Empty;
    public string FoundingDay { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(GuildUid) && ks.PutWideString(GuildName) && ks.Put(MaxNumMember) &&
        ks.Put(GuildLevel) && ks.Put(GuildExp) && ks.PutWideString(GuildMessage) && ks.PutWideString(FoundingDay);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int uid) || !ks.GetWideString(out string name) || !ks.Get(out ushort maxm) ||
            !ks.Get(out byte lvl) || !ks.Get(out int exp) || !ks.GetWideString(out string msg) ||
            !ks.GetWideString(out string fd))
            return false;
        GuildUid = uid; GuildName = name; MaxNumMember = maxm; GuildLevel = lvl;
        GuildExp = exp; GuildMessage = msg; FoundingDay = fd;
        return true;
    }
}

public sealed class KSubQuestInstance : IKSerializable
{
    public int Id { get; set; }
    public byte ClearData { get; set; }
    public bool IsSuccess { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Id) && ks.Put(ClearData) && ks.Put(IsSuccess);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int id) || !ks.Get(out byte cd) || !ks.Get(out bool ok)) return false;
        Id = id; ClearData = cd; IsSuccess = ok;
        return true;
    }
}

public sealed class KQuestInstance : IKSerializable
{
    public int Id { get; set; }
    public long OwnerUnitUid { get; set; }
    public List<KSubQuestInstance> SubQuests { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Id) && ks.Put(OwnerUnitUid) && ks.PutList(SubQuests);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int id) || !ks.Get(out long uid) || !ks.GetList(SubQuests)) return false;
        Id = id; OwnerUnitUid = uid;
        return true;
    }
}

public sealed class KPostItemTitleInfo : IKSerializable
{
    public long PostNo { get; set; }
    public sbyte ScriptType { get; set; }
    public int ScriptIndex { get; set; }
    public sbyte EnchantLevel { get; set; }
    public string RegDate { get; set; } = string.Empty;
    public string FromNickName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public bool Read { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(PostNo) && ks.Put(ScriptType) && ks.Put(ScriptIndex) && ks.Put(EnchantLevel) &&
        ks.PutWideString(RegDate) && ks.PutWideString(FromNickName) && ks.PutWideString(Title) && ks.Put(Read);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long pno) || !ks.Get(out sbyte st) || !ks.Get(out int si) || !ks.Get(out sbyte ench) ||
            !ks.GetWideString(out string rdate) || !ks.GetWideString(out string fnick) ||
            !ks.GetWideString(out string title) || !ks.Get(out bool rd))
            return false;
        PostNo = pno; ScriptType = st; ScriptIndex = si; EnchantLevel = ench;
        RegDate = rdate; FromNickName = fnick; Title = title; Read = rd;
        return true;
    }
}

public sealed class KPostSmallTitleInfo : IKSerializable
{
    public sbyte ScriptType { get; set; }
    public string LetterTitle { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(ScriptType) && ks.PutWideString(LetterTitle);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte st) || !ks.GetWideString(out string title)) return false;
        ScriptType = st; LetterTitle = title;
        return true;
    }
}

public sealed class KChatBlackListUnit : IKSerializable
{
    public long UnitUid { get; set; }
    public string NickName { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(UnitUid) && ks.PutWideString(NickName);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.GetWideString(out string nick)) return false;
        UnitUid = uid; NickName = nick;
        return true;
    }
}

public sealed class KDenyOptions : IKSerializable
{
    public sbyte DenyFriendShip { get; set; }
    public sbyte DenyInviteGuild { get; set; }
    public sbyte DenyParty { get; set; }
    public sbyte DenyPersonalTrade { get; set; }
    public sbyte DenyRequestCouple { get; set; }
    public sbyte DenyInvitePracticePvp { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(DenyFriendShip) && ks.Put(DenyInviteGuild) && ks.Put(DenyParty) &&
        ks.Put(DenyPersonalTrade) && ks.Put(DenyRequestCouple) && ks.Put(DenyInvitePracticePvp);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte f) || !ks.Get(out sbyte g) || !ks.Get(out sbyte p) ||
            !ks.Get(out sbyte t) || !ks.Get(out sbyte c) || !ks.Get(out sbyte pvp))
            return false;
        DenyFriendShip = f; DenyInviteGuild = g; DenyParty = p;
        DenyPersonalTrade = t; DenyRequestCouple = c; DenyInvitePracticePvp = pvp;
        return true;
    }
}

public sealed class KCommunityUserInfo : IKSerializable
{
    public long UnitUid { get; set; }
    public string NickName { get; set; } = string.Empty;
    public sbyte UnitClass { get; set; }
    public byte Level { get; set; }
    public sbyte State { get; set; }
    public long StateCode { get; set; }
    public long RoomUid { get; set; }
    public int Ed { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(UnitUid) && ks.PutWideString(NickName) && ks.Put(UnitClass) && ks.Put(Level) &&
        ks.Put(State) && ks.Put(StateCode) && ks.Put(RoomUid) && ks.Put(Ed);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.GetWideString(out string nick) || !ks.Get(out sbyte cls) ||
            !ks.Get(out byte lvl) || !ks.Get(out sbyte st) || !ks.Get(out long sc) ||
            !ks.Get(out long ruid) || !ks.Get(out int ed))
            return false;
        UnitUid = uid; NickName = nick; UnitClass = cls; Level = lvl;
        State = st; StateCode = sc; RoomUid = ruid; Ed = ed;
        return true;
    }
}

public sealed class KNPCUnitReq : IKSerializable
{
    public int Uid { get; set; } = -1;
    public int NpcId { get; set; } = -1;
    public byte Level { get; set; } = 1;
    public KVector3 Pos { get; set; } = new();
    public int StartPos { get; set; }
    public List<int> PatrolLineIndex { get; set; } = new();
    public List<int> PlayLineIndex { get; set; } = new();
    public int GroupId { get; set; }
    public bool Aggressive { get; set; } = true;
    public bool Active { get; set; }
    public float DelayTime { get; set; }
    public sbyte KeyCode { get; set; } = -1;
    public bool FocusCamera { get; set; }
    public bool ShowGage { get; set; }
    public KVector3 AddPos { get; set; } = new();
    public bool HasBossGage { get; set; }
    public bool ShowBossName { get; set; }
    public bool ShowSubBossName { get; set; }
    public bool SiegeMode { get; set; }
    public bool IsRight { get; set; }
    public bool NoDrop { get; set; }
    public float UnitScale { get; set; } = -1.0f;
    public sbyte TeamNum { get; set; } = 2;
    public sbyte AiType { get; set; }
    public long AllyUid { get; set; } = -1;
    public sbyte AllyTeam { get; set; } = 2;
    public sbyte MonsterGrade { get; set; }
    public ushort BossGaugeHpLines { get; set; }
    public bool AllyNpc { get; set; }
    public bool AttributeNpc { get; set; }
    public sbyte CreateMonsterType { get; set; }
    public float ShowBossNameDelayTime { get; set; } = -1.0f;
    public bool StopAtStartState { get; set; }
    public int BossGroupId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Uid) && ks.Put(NpcId) && ks.Put(Level) && ks.Put(Pos) && ks.Put(StartPos) &&
        ks.PutList(PatrolLineIndex, static (s, v) => s.Put(v)) &&
        ks.PutList(PlayLineIndex, static (s, v) => s.Put(v)) &&
        ks.Put(GroupId) && ks.Put(Aggressive) &&
        ks.Put(Active) && ks.Put(DelayTime) && ks.Put(KeyCode) && ks.Put(FocusCamera) && ks.Put(ShowGage) &&
        ks.Put(AddPos) && ks.Put(HasBossGage) && ks.Put(ShowBossName) && ks.Put(ShowSubBossName) &&
        ks.Put(SiegeMode) && ks.Put(IsRight) && ks.Put(NoDrop) && ks.Put(UnitScale) &&
        ks.Put(TeamNum) && ks.Put(AiType) && ks.Put(AllyUid) && ks.Put(AllyTeam) &&
        ks.Put(MonsterGrade) && ks.Put(BossGaugeHpLines) && ks.Put(AllyNpc) && ks.Put(AttributeNpc) &&
        ks.Put(CreateMonsterType) && ks.Put(ShowBossNameDelayTime) && ks.Put(StopAtStartState) &&
        ks.Put(BossGroupId);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int uid) || !ks.Get(out int nid) || !ks.Get(out byte lvl) ||
            !ks.Get(Pos) || !ks.Get(out int sp) ||
            !ks.GetList(PatrolLineIndex, static s => (s.Get(out int v), v)) ||
            !ks.GetList(PlayLineIndex, static s => (s.Get(out int v), v)) ||
            !ks.Get(out int gid) || !ks.Get(out bool agg) ||
            !ks.Get(out bool active) || !ks.Get(out float delay) || !ks.Get(out sbyte keyCode) ||
            !ks.Get(out bool focusCam) || !ks.Get(out bool showGage) || !ks.Get(AddPos) ||
            !ks.Get(out bool hasBossGage) || !ks.Get(out bool showBossName) || !ks.Get(out bool showSubBossName) ||
            !ks.Get(out bool siegeMode) || !ks.Get(out bool isRight) || !ks.Get(out bool noDrop) ||
            !ks.Get(out float unitScale) || !ks.Get(out sbyte teamNum) || !ks.Get(out sbyte aiType) ||
            !ks.Get(out long allyUid) || !ks.Get(out sbyte allyTeam) || !ks.Get(out sbyte monsterGrade) ||
            !ks.Get(out ushort bossGaugeHpLines) || !ks.Get(out bool allyNpc) || !ks.Get(out bool attrNpc) ||
            !ks.Get(out sbyte createMonsterType) || !ks.Get(out float showBossDelay) ||
            !ks.Get(out bool stopAtStart) || !ks.Get(out int bossGroupId))
            return false;
        Uid = uid; NpcId = nid; Level = lvl; StartPos = sp; GroupId = gid; Aggressive = agg;
        Active = active; DelayTime = delay; KeyCode = keyCode; FocusCamera = focusCam; ShowGage = showGage;
        HasBossGage = hasBossGage; ShowBossName = showBossName; ShowSubBossName = showSubBossName;
        SiegeMode = siegeMode; IsRight = isRight; NoDrop = noDrop; UnitScale = unitScale;
        TeamNum = teamNum; AiType = aiType; AllyUid = allyUid; AllyTeam = allyTeam;
        MonsterGrade = monsterGrade; BossGaugeHpLines = bossGaugeHpLines; AllyNpc = allyNpc;
        AttributeNpc = attrNpc; CreateMonsterType = createMonsterType;
        ShowBossNameDelayTime = showBossDelay; StopAtStartState = stopAtStart; BossGroupId = bossGroupId;
        return true;
    }
}

public sealed class KNPCList : IKSerializable
{
    public List<KNPCUnitReq> NpcList { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) => ks.PutList(NpcList);
    public bool Get(KSerializer ks, RegionProfile region) => ks.GetList(NpcList);
}

public sealed class KAttribEnchantNpcInfo : IKSerializable
{
    public sbyte AttribEnchant1 { get; set; }
    public sbyte AttribEnchant2 { get; set; }
    public sbyte ExtraSpecialAbility1 { get; set; }
    public sbyte ExtraSpecialAbility2 { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(AttribEnchant1) && ks.Put(AttribEnchant2) &&
        ks.Put(ExtraSpecialAbility1) && ks.Put(ExtraSpecialAbility2);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte a1) || !ks.Get(out sbyte a2) || !ks.Get(out sbyte e1) || !ks.Get(out sbyte e2))
            return false;
        AttribEnchant1 = a1; AttribEnchant2 = a2; ExtraSpecialAbility1 = e1; ExtraSpecialAbility2 = e2;
        return true;
    }
}

public sealed class KEXPData : IKSerializable
{
    public int Exp { get; set; }
    public int PartyExp { get; set; }
    public int SocketOptExp { get; set; }
    public int EventBonusExp { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Exp) && ks.Put(PartyExp) && ks.Put(SocketOptExp) && ks.Put(EventBonusExp);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int exp) || !ks.Get(out int pexp) || !ks.Get(out int sexp) || !ks.Get(out int eexp))
            return false;
        Exp = exp; PartyExp = pexp; SocketOptExp = sexp; EventBonusExp = eexp;
        return true;
    }
}

public sealed class KDungeonRewardED : IKSerializable
{
    public int Ed { get; set; }
    public int BonusEd { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ed) && ks.Put(BonusEd);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ed) || !ks.Get(out int bed)) return false;
        Ed = ed; BonusEd = bed;
        return true;
    }
}

public sealed class KBattleFieldJoinInfo : IKSerializable
{
    public int BattleFieldId { get; set; }
    public int StartPosIndex { get; set; }
    public List<long> PartyMemberList { get; set; } = new();
    public bool MoveForMyParty { get; set; }
    public bool NowBattleFieldPositionInfoStartPosition { get; set; }
    public ushort BattleFieldPositionValue { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(BattleFieldId) && ks.Put(StartPosIndex) &&
        ks.PutList(PartyMemberList, static (s, v) => s.Put(v)) &&
        ks.Put(MoveForMyParty) && ks.Put(NowBattleFieldPositionInfoStartPosition) && ks.Put(BattleFieldPositionValue);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int bfid) || !ks.Get(out int spi) ||
            !ks.GetList(PartyMemberList, static s => (s.Get(out long v), v)) ||
            !ks.Get(out bool mfmp) || !ks.Get(out bool nowStart) || !ks.Get(out ushort posVal))
            return false;
        BattleFieldId = bfid; StartPosIndex = spi; MoveForMyParty = mfmp;
        NowBattleFieldPositionInfoStartPosition = nowStart; BattleFieldPositionValue = posVal;
        return true;
    }
}

public sealed class KFieldOption : IKSerializable
{
    public short LimitOption { get; set; } = 35;

    public static bool VerifyLimitOption(short sOption) => sOption is 10 or 25 or 35;

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(LimitOption);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out short opt)) return false;
        LimitOption = opt;
        return true;
    }
}

public sealed class KTCInfo : IKSerializable
{
    public int TcId { get; set; }
    public int DungeonId { get; set; }
    public long RoomUid { get; set; }
    public sbyte RoomType { get; set; }
    public string UdpRelayIp { get; set; } = string.Empty;
    public ushort UdpRelayPort { get; set; }
    public float PlayTime { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(TcId) && ks.Put(DungeonId) && ks.Put(RoomUid) && ks.Put(RoomType) &&
        ks.PutWideString(UdpRelayIp) && ks.Put(UdpRelayPort) && ks.Put(PlayTime);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int tcId) || !ks.Get(out int dId) || !ks.Get(out long ruid) ||
            !ks.Get(out sbyte rType) || !ks.GetWideString(out string ip) ||
            !ks.Get(out ushort port) || !ks.Get(out float pt))
            return false;
        TcId = tcId; DungeonId = dId; RoomUid = ruid; RoomType = rType;
        UdpRelayIp = ip; UdpRelayPort = port; PlayTime = pt;
        return true;
    }
}

public sealed class KErrorMsgInfo : IKSerializable
{
    public int ErrorMsgId { get; set; }
    public string ErrorMsg { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(ErrorMsgId) && ks.PutWideString(ErrorMsg);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int id) || !ks.GetWideString(out string msg)) return false;
        ErrorMsgId = id; ErrorMsg = msg;
        return true;
    }
}

public sealed class KGuildMemberInfo : IKSerializable
{
    public long GsUid { get; set; }
    public long UnitUid { get; set; }
    public string NickName { get; set; } = string.Empty;
    public sbyte UnitClass { get; set; }
    public byte Level { get; set; } = 1;
    public byte MemberShipGrade { get; set; } = 5;
    public int ChannelId { get; set; } = 1;
    public int MapId { get; set; }
    public sbyte Position { get; set; } = -1;
    public long LogInTime { get; set; }
    public long LogOutTime { get; set; }
    public string Message { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(GsUid) && ks.Put(UnitUid) && ks.PutWideString(NickName) &&
        ks.Put(UnitClass) && ks.Put(Level) && ks.Put(MemberShipGrade) &&
        ks.Put(ChannelId) && ks.Put(MapId) && ks.Put(Position) &&
        ks.Put(LogInTime) && ks.Put(LogOutTime) && ks.PutWideString(Message);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long gs) || !ks.Get(out long uid) || !ks.GetWideString(out string nick) ||
            !ks.Get(out sbyte uc) || !ks.Get(out byte lvl) || !ks.Get(out byte grade) ||
            !ks.Get(out int cid) || !ks.Get(out int mid) || !ks.Get(out sbyte pos) ||
            !ks.Get(out long tin) || !ks.Get(out long tout) || !ks.GetWideString(out string msg))
            return false;
        GsUid = gs; UnitUid = uid; NickName = nick; UnitClass = uc; Level = lvl;
        MemberShipGrade = grade; ChannelId = cid; MapId = mid; Position = pos;
        LogInTime = tin; LogOutTime = tout; Message = msg;
        return true;
    }
}

public sealed class KGuildMemberMessageInfo : IKSerializable
{
    public long UnitUid { get; set; }
    public string NickName { get; set; } = string.Empty;
    public byte MemberShipGrade { get; set; } = 5;
    public sbyte UnitClass { get; set; }
    public byte Level { get; set; } = 1;
    public string Message { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(UnitUid) && ks.PutWideString(NickName) && ks.Put(MemberShipGrade) &&
        ks.Put(UnitClass) && ks.Put(Level) && ks.PutWideString(Message);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.GetWideString(out string nick) || !ks.Get(out byte grade) ||
            !ks.Get(out sbyte uc) || !ks.Get(out byte lvl) || !ks.GetWideString(out string msg))
            return false;
        UnitUid = uid; NickName = nick; MemberShipGrade = grade; UnitClass = uc; Level = lvl; Message = msg;
        return true;
    }
}

public sealed class KGuildSkillData : IKSerializable
{
    public short SkillId { get; set; }
    public byte SkillLevel { get; set; }
    public byte SkillCsPoint { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(SkillId) && ks.Put(SkillLevel) && ks.Put(SkillCsPoint);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out short sid) || !ks.Get(out byte lvl) || !ks.Get(out byte csp)) return false;
        SkillId = sid; SkillLevel = lvl; SkillCsPoint = csp;
        return true;
    }
}

public sealed class KGuildSkillInfo : IKSerializable
{
    public List<KGuildSkillData> GuildSkill { get; set; } = new();
    public int GuildSPoint { get; set; }
    public int GuildCsPoint { get; set; }
    public int MaxGuildCsPoint { get; set; }
    public string GuildCsPointEndDate { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.PutList(GuildSkill) && ks.Put(GuildSPoint) && ks.Put(GuildCsPoint) &&
        ks.Put(MaxGuildCsPoint) && ks.PutWideString(GuildCsPointEndDate);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetList(GuildSkill) || !ks.Get(out int sp) || !ks.Get(out int csp) ||
            !ks.Get(out int mcsp) || !ks.GetWideString(out string end))
            return false;
        GuildSPoint = sp; GuildCsPoint = csp; MaxGuildCsPoint = mcsp; GuildCsPointEndDate = end;
        return true;
    }
}

public sealed class KApplyJoinGuildInfo : IKSerializable
{
    public long ApplyJoinGuildUid { get; set; }
    public long UnitUid { get; set; }
    public string NickName { get; set; } = string.Empty;
    public sbyte UnitClass { get; set; }
    public byte Level { get; set; } = 1;
    public string Message { get; set; } = string.Empty;
    public string RegDate { get; set; } = string.Empty;
    public long RegDateTimestamp { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(ApplyJoinGuildUid) && ks.Put(UnitUid) && ks.PutWideString(NickName) &&
        ks.Put(UnitClass) && ks.Put(Level) && ks.PutWideString(Message) &&
        ks.PutWideString(RegDate) && ks.Put(RegDateTimestamp);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long auid) || !ks.Get(out long uid) || !ks.GetWideString(out string nick) ||
            !ks.Get(out sbyte uc) || !ks.Get(out byte lvl) || !ks.GetWideString(out string msg) ||
            !ks.GetWideString(out string reg) || !ks.Get(out long rts))
            return false;
        ApplyJoinGuildUid = auid; UnitUid = uid; NickName = nick; UnitClass = uc;
        Level = lvl; Message = msg; RegDate = reg; RegDateTimestamp = rts;
        return true;
    }
}

public sealed class KFriendInfo : IKSerializable
{
    public const sbyte FsNormal = 0;
    public const sbyte FsRequested = 1;
    public const sbyte FsWaiting = 2;
    public const sbyte FsBlocked = 3;

    public const sbyte FpOffLine = 0;
    public const sbyte FpOnLine = 1;
    public const sbyte FpSquare = 2;
    public const sbyte FpDungeon = 3;
    public const sbyte FpPvp = 4;
    public const sbyte FpVillage = 5;

    public long UnitUid { get; set; }
    public string NickName { get; set; } = string.Empty;
    public sbyte GroupId { get; set; }
    public sbyte Status { get; set; }
    public sbyte Position { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(UnitUid) && ks.PutWideString(NickName) && ks.Put(GroupId) && ks.Put(Status) && ks.Put(Position);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.GetWideString(out string nick) ||
            !ks.Get(out sbyte gid) || !ks.Get(out sbyte st) || !ks.Get(out sbyte pos))
            return false;
        UnitUid = uid; NickName = nick; GroupId = gid; Status = st; Position = pos;
        return true;
    }
}

public sealed class KNMUnitInfo : IKSerializable
{
    public long UnitUid { get; set; }
    public int State { get; set; }
    public int DungeonId { get; set; }
    public long RoomUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(UnitUid) && ks.Put(State) && ks.Put(DungeonId) && ks.Put(RoomUid);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.Get(out int st) || !ks.Get(out int did) || !ks.Get(out long ruid))
            return false;
        UnitUid = uid; State = st; DungeonId = did; RoomUid = ruid;
        return true;
    }
}

public sealed class KWeddingItemInfo : IKSerializable
{
    public long ItemUid { get; set; }
    public int WeddingUid { get; set; }
    public sbyte WeddingHallType { get; set; }
    public sbyte OfficiantNpc { get; set; }
    public long Groom { get; set; }
    public long Bride { get; set; }
    public string GroomNick { get; set; } = string.Empty;
    public string BrideNick { get; set; } = string.Empty;
    public string WeddingDate { get; set; } = string.Empty;
    public string WeddingMsg { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(ItemUid) && ks.Put(WeddingUid) && ks.Put(WeddingHallType) && ks.Put(OfficiantNpc) &&
        ks.Put(Groom) && ks.Put(Bride) && ks.PutWideString(GroomNick) && ks.PutWideString(BrideNick) &&
        ks.PutWideString(WeddingDate) && ks.PutWideString(WeddingMsg);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long iuid) || !ks.Get(out int wuid) || !ks.Get(out sbyte wht) || !ks.Get(out sbyte npc) ||
            !ks.Get(out long g) || !ks.Get(out long b) || !ks.GetWideString(out string gn) ||
            !ks.GetWideString(out string bn) || !ks.GetWideString(out string wd) || !ks.GetWideString(out string wm))
            return false;
        ItemUid = iuid; WeddingUid = wuid; WeddingHallType = wht; OfficiantNpc = npc;
        Groom = g; Bride = b; GroomNick = gn; BrideNick = bn; WeddingDate = wd; WeddingMsg = wm;
        return true;
    }
}

public sealed class KWeddingHallInfo : IKSerializable
{
    public int WeddingUid { get; set; }
    public sbyte WeddingHallType { get; set; }
    public sbyte OfficiantNpc { get; set; }
    public long Groom { get; set; }
    public long Bride { get; set; }
    public string WeddingDateStr { get; set; } = string.Empty;
    public long WeddingDate { get; set; }
    public string WeddingMsg { get; set; } = string.Empty;
    public long RoomUid { get; set; }
    public bool Success { get; set; }
    public bool Delete { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(WeddingUid) && ks.Put(WeddingHallType) && ks.Put(OfficiantNpc) &&
        ks.Put(Groom) && ks.Put(Bride) && ks.PutWideString(WeddingDateStr) &&
        ks.Put(WeddingDate) && ks.PutWideString(WeddingMsg) && ks.Put(RoomUid) &&
        ks.Put(Success) && ks.Put(Delete);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int wuid) || !ks.Get(out sbyte wht) || !ks.Get(out sbyte npc) ||
            !ks.Get(out long g) || !ks.Get(out long b) || !ks.GetWideString(out string wds) ||
            !ks.Get(out long wd) || !ks.GetWideString(out string wm) || !ks.Get(out long ruid) ||
            !ks.Get(out bool succ) || !ks.Get(out bool del))
            return false;
        WeddingUid = wuid; WeddingHallType = wht; OfficiantNpc = npc;
        Groom = g; Bride = b; WeddingDateStr = wds; WeddingDate = wd;
        WeddingMsg = wm; RoomUid = ruid; Success = succ; Delete = del;
        return true;
    }
}

public sealed class KAutoPartyDungeonBonusInfo : IKSerializable
{
    public int DungeonId { get; set; }
    public sbyte DungeonMode { get; set; }
    public bool ActivatedBonus { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(DungeonId) && ks.Put(DungeonMode) && ks.Put(ActivatedBonus);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int did) || !ks.Get(out sbyte dm) || !ks.Get(out bool ab)) return false;
        DungeonId = did; DungeonMode = dm; ActivatedBonus = ab;
        return true;
    }
}

public sealed class KDungeonGameSimpleInfo : IKSerializable
{
    public int DungeonId { get; set; }
    public sbyte DungeonMode { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(DungeonId) && ks.Put(DungeonMode);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int did) || !ks.Get(out sbyte dm)) return false;
        DungeonId = did; DungeonMode = dm;
        return true;
    }
}

public sealed class KPVPChannelInfo : IKSerializable
{
    public const sbyte PccNone = -1;
    public const sbyte PccRecruit = 0;
    public const sbyte PccE1 = 1;
    public const sbyte PccE2 = 2;
    public const sbyte PccE3 = 3;
    public const sbyte PccE4 = 4;
    public const sbyte PccE5 = 5;
    public const sbyte PccE6 = 6;
    public const sbyte PccE7 = 7;
    public const sbyte PccE8 = 8;
    public const sbyte PccFree = 9;
    public const sbyte PccTournament = 10;
    public const sbyte PccPlay = 11;
    public const sbyte PccOfficial = 12;

    public int MinRating { get; set; }
    public int MaxRating { get; set; }
    public uint ChannelId { get; set; }
    public sbyte PvpChannelClass { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(MinRating) && ks.Put(MaxRating) && ks.Put(ChannelId) && ks.Put(PvpChannelClass);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int minR) || !ks.Get(out int maxR) || !ks.Get(out uint chId) || !ks.Get(out sbyte cls))
            return false;
        MinRating = minR; MaxRating = maxR; ChannelId = chId; PvpChannelClass = cls;
        return true;
    }
}

public sealed class KDropItemData : IKSerializable
{
    public int ItemId { get; set; }
    public int DropItemUid { get; set; }
    public bool Left { get; set; }
    public float Speed { get; set; }
    public HashSet<long> GetPossibleUnitUid { get; } = new();
    public sbyte EnchantLevel { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(ItemId) && ks.Put(DropItemUid) && ks.Put(Left) && ks.Put(Speed) &&
        ks.PutList(GetPossibleUnitUid, static (s, v) => s.Put(v)) &&
        ks.Put(EnchantLevel);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int iid) || !ks.Get(out int duid) || !ks.Get(out bool left) || !ks.Get(out float spd) ||
            !ks.GetHashSet(GetPossibleUnitUid, static s => (s.Get(out long v), v)) ||
            !ks.Get(out sbyte enc))
            return false;
        ItemId = iid; DropItemUid = duid; Left = left; Speed = spd; EnchantLevel = enc;
        return true;
    }
}

public sealed class KSquareInfo : IKSerializable
{
    public sbyte SquareType { get; set; }
    public long SquareUid { get; set; }
    public string RoomName { get; set; } = string.Empty;
    public sbyte MaxSlot { get; set; }
    public sbyte JoinSlot { get; set; }
    public string RelayIp { get; set; } = "127.0.0.1";
    public int Port { get; set; } = 9303;

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(SquareType) && ks.Put(SquareUid) && ks.PutWideString(RoomName) &&
        ks.Put(MaxSlot) && ks.Put(JoinSlot) && ks.PutWideString(RelayIp) && ks.Put(Port);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte st) || !ks.Get(out long suid) || !ks.GetWideString(out var rn) ||
            !ks.Get(out sbyte maxS) || !ks.Get(out sbyte joinS) || !ks.GetWideString(out var ip) ||
            !ks.Get(out int port))
            return false;
        SquareType = st; SquareUid = suid; RoomName = rn; MaxSlot = maxS; JoinSlot = joinS; RelayIp = ip; Port = port;
        return true;
    }
}

public sealed class KSquareUserInfo : IKSerializable
{
    public long GsUid { get; set; }
    public long UnitUid { get; set; }
    public string NickName { get; set; } = string.Empty;
    public string Ip { get; set; } = string.Empty;
    public ushort Port { get; set; }
    public sbyte UnitClass { get; set; }
    public byte Level { get; set; }
    public sbyte Rank { get; set; }
    public bool IsParty { get; set; }
    public sbyte PersonalShopState { get; set; }
    public sbyte PersonalShopType { get; set; }
    public string PersonalShopName { get; set; } = string.Empty;
    public List<KInventoryItemSimpleInfo> EquippedItem { get; } = new();
    public int TitleId { get; set; }
    public string GuildName { get; set; } = string.Empty;
    public int Ed { get; set; }
    public int EventQuestClearCount { get; set; }
    public int ExchangeCount { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(GsUid) && ks.Put(UnitUid) && ks.PutWideString(NickName) &&
        ks.PutWideString(Ip) && ks.Put(Port) && ks.Put(UnitClass) && ks.Put(Level) &&
        ks.Put(Rank) && ks.Put(IsParty) && ks.Put(PersonalShopState) && ks.Put(PersonalShopType) &&
        ks.PutWideString(PersonalShopName) && ks.PutList(EquippedItem) && ks.Put(TitleId) &&
        ks.PutWideString(GuildName) && ks.Put(Ed) &&
        ks.Put(EventQuestClearCount) && ks.Put(ExchangeCount);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long gs) || !ks.Get(out long uid) || !ks.GetWideString(out var nick) ||
            !ks.GetWideString(out var ip) || !ks.Get(out ushort port) || !ks.Get(out sbyte uc) || !ks.Get(out byte lv) ||
            !ks.Get(out sbyte rank) || !ks.Get(out bool party) || !ks.Get(out sbyte pss) || !ks.Get(out sbyte pst) ||
            !ks.GetWideString(out var psn) || !ks.GetList(EquippedItem) || !ks.Get(out int title) ||
            !ks.GetWideString(out var gname) || !ks.Get(out int ed) ||
            !ks.Get(out int eqcc) || !ks.Get(out int exc))
            return false;
        GsUid = gs; UnitUid = uid; NickName = nick; Ip = ip; Port = port; UnitClass = uc; Level = lv;
        Rank = rank; IsParty = party; PersonalShopState = pss; PersonalShopType = pst;
        PersonalShopName = psn; TitleId = title; GuildName = gname; Ed = ed;
        EventQuestClearCount = eqcc; ExchangeCount = exc;
        return true;
    }
}

public sealed class KDungeonPlayResultInfo : IKSerializable
{
    public float Hp { get; set; }
    public int DamageCount { get; set; }
    public int TotalScore { get; set; }
    public int ComboScore { get; set; }
    public int TechScore { get; set; }
    public int TotalGivenDamage { get; set; }
    public int TotalAttackedDamage { get; set; }
    public int TotalSumNpcHp { get; set; }
    public int TotalRecoveryHp { get; set; }
    public int UsingKindOfSkill { get; set; }
    public int CommandTechScore { get; set; }
    public int SkillTechScore { get; set; }
    public int DashCount { get; set; }
    public int Frame { get; set; }
    public bool SkillSlotType { get; set; } = true;

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Hp) && ks.Put(DamageCount) && ks.Put(TotalScore) && ks.Put(ComboScore) && ks.Put(TechScore) &&
        ks.Put(TotalGivenDamage) && ks.Put(TotalAttackedDamage) && ks.Put(TotalSumNpcHp) && ks.Put(TotalRecoveryHp) &&
        ks.Put(UsingKindOfSkill) && ks.Put(CommandTechScore) && ks.Put(SkillTechScore) &&
        ks.Put(DashCount) && ks.Put(Frame) && ks.Put(SkillSlotType);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out float hp) || !ks.Get(out int dc) || !ks.Get(out int ts) || !ks.Get(out int cs) || !ks.Get(out int tech) ||
            !ks.Get(out int tgd) || !ks.Get(out int tad) || !ks.Get(out int tsnh) || !ks.Get(out int trhp) ||
            !ks.Get(out int uks) || !ks.Get(out int cts) || !ks.Get(out int sts) ||
            !ks.Get(out int dash) || !ks.Get(out int frame) || !ks.Get(out bool sst))
            return false;
        Hp = hp; DamageCount = dc; TotalScore = ts; ComboScore = cs; TechScore = tech;
        TotalGivenDamage = tgd; TotalAttackedDamage = tad; TotalSumNpcHp = tsnh; TotalRecoveryHp = trhp;
        UsingKindOfSkill = uks; CommandTechScore = cts; SkillTechScore = sts;
        DashCount = dash; Frame = frame; SkillSlotType = sst;
        return true;
    }
}

public sealed class KGamePlayNetworkInfo : IKSerializable
{
    public long OpponentUnitUid { get; set; }
    public int HolePunchingTryCount { get; set; }
    public float P2PPlayTime { get; set; }
    public float RelayPlayTime { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(OpponentUnitUid) && ks.Put(HolePunchingTryCount) && ks.Put(P2PPlayTime) && ks.Put(RelayPlayTime);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long ouid) || !ks.Get(out int hp) || !ks.Get(out float p2p) || !ks.Get(out float relay))
            return false;
        OpponentUnitUid = ouid; HolePunchingTryCount = hp; P2PPlayTime = p2p; RelayPlayTime = relay;
        return true;
    }
}

public sealed class KGamePlayNetworkInfoPackage : IKSerializable
{
    public List<KGamePlayNetworkInfo> GamePlayNetworkInfo { get; } = new();

    public bool Put(KSerializer ks, RegionProfile region) => ks.PutList(GamePlayNetworkInfo);

    public bool Get(KSerializer ks, RegionProfile region) => ks.GetList(GamePlayNetworkInfo);
}

public sealed class KBuffSkillInfo : IKSerializable
{
    public long CaughtUnitUid { get; set; }
    public int BuffEnum { get; set; }
    public long CastingUnitUid { get; set; }
    public bool IsBuffCasting { get; set; }
    public sbyte BuffLevel { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(CaughtUnitUid) && ks.Put(BuffEnum) && ks.Put(CastingUnitUid) && ks.Put(IsBuffCasting) && ks.Put(BuffLevel);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long caught) || !ks.Get(out int be) || !ks.Get(out long cast) ||
            !ks.Get(out bool ibc) || !ks.Get(out sbyte bl))
            return false;
        CaughtUnitUid = caught; BuffEnum = be; CastingUnitUid = cast; IsBuffCasting = ibc; BuffLevel = bl;
        return true;
    }
}

public sealed class KPartyListUserInfo : IKSerializable
{
    public int ChannelId { get; set; }
    public long UnitUid { get; set; }
    public string NickName { get; set; } = string.Empty;
    public sbyte UnitClass { get; set; }
    public byte Level { get; set; }
    public byte State { get; set; }
    public int StateCode { get; set; }
    public bool Host { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(ChannelId) && ks.Put(UnitUid) && ks.PutWideString(NickName) &&
        ks.Put(UnitClass) && ks.Put(Level) && ks.Put(State) && ks.Put(StateCode) && ks.Put(Host);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int cid) || !ks.Get(out long uid) || !ks.GetWideString(out var nick) ||
            !ks.Get(out sbyte uc) || !ks.Get(out byte lv) || !ks.Get(out byte st) ||
            !ks.Get(out int sc) || !ks.Get(out bool host))
            return false;
        ChannelId = cid; UnitUid = uid; NickName = nick; UnitClass = uc; Level = lv;
        State = st; StateCode = sc; Host = host;
        return true;
    }
}

public sealed class KPartyListInfo : IKSerializable
{
    public KPartyInfo PartyInfo { get; set; } = new();
    public List<KPartyListUserInfo> PartyListUserInfo { get; } = new();

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(PartyInfo) && ks.PutList(PartyListUserInfo);

    public bool Get(KSerializer ks, RegionProfile region) => ks.Get(PartyInfo) && ks.GetList(PartyListUserInfo);
}

public sealed class KUserAuthenticateReq : IKSerializable
{
    public const int ServerSnLength = 12;

    public bool DebugAuth { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Hwid { get; set; } = string.Empty;
    public string MachineId { get; set; } = string.Empty;
    public int ChannelRandomKey { get; set; }
    public byte[] ServerSN { get; set; } = new byte[ServerSnLength];
    public int ChannelingCode { get; set; }
    public bool ManualLogin { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(DebugAuth) || !ks.PutWideString(UserId) || !ks.PutWideString(Password) ||
            !ks.PutWideString(Hwid) || !ks.PutNarrowString(MachineId))
            return false;
        if (region.DllListCheckBeforeLoading && !ks.Put(ChannelRandomKey))
            return false;
        if (region.SerialNumberCheckInGameServer)
        {
            for (int i = 0; i < ServerSnLength; i++)
            {
                byte b = i < ServerSN.Length ? ServerSN[i] : (byte)0;
                if (!ks.Put(b)) return false;
            }
        }
        return ks.Put(ChannelingCode) && ks.Put(ManualLogin);
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool da) || !ks.GetWideString(out var uid) || !ks.GetWideString(out var pw) ||
            !ks.GetWideString(out var hw) || !ks.GetNarrowString(out var mid))
            return false;
        int rk = 0;
        if (region.DllListCheckBeforeLoading && !ks.Get(out rk))
            return false;
        ServerSN = new byte[ServerSnLength];
        if (region.SerialNumberCheckInGameServer)
        {
            for (int i = 0; i < ServerSnLength; i++)
            {
                if (!ks.Get(out byte b)) return false;
                ServerSN[i] = b;
            }
        }
        if (!ks.Get(out int cc) || !ks.Get(out bool ml)) return false;
        DebugAuth = da; UserId = uid; Password = pw; Hwid = hw; MachineId = mid;
        ChannelRandomKey = rk; ChannelingCode = cc; ManualLogin = ml;
        return true;
    }
}

public sealed class KServerSetData : IKSerializable
{
    public const sbyte UclInvalid = 0;
    public const sbyte UclFree = 1;
    public const sbyte UclNormal = 2;
    public const sbyte UclBusy = 3;
    public const sbyte UclFull = 4;

    public int ServerSetId { get; set; }
    public string ServerSetName { get; set; } = string.Empty;
    public sbyte UserCountLevel { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(ServerSetId) && ks.PutWideString(ServerSetName) && ks.Put(UserCountLevel);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int id) || !ks.GetWideString(out var name) || !ks.Get(out sbyte lvl))
            return false;
        ServerSetId = id; ServerSetName = name; UserCountLevel = lvl;
        return true;
    }
}

public sealed class KUserConnectTimeEventInfo : IKSerializable
{
    public int EventId { get; set; }
    public float EventTime { get; set; }
    public long CTime { get; set; }
    public string EventName { get; set; } = string.Empty;
    public bool Init { get; set; }
    public bool GiveOnce { get; set; } = true;
    public bool AccountEvent { get; set; }
    public double BackUpTime { get; set; }
    public bool RepeatRewardEvent { get; set; }
    public int BeforeEventId { get; set; }
    public int EventItemId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(EventId) && ks.Put(EventTime) && ks.Put(CTime) && ks.PutWideString(EventName) &&
        ks.Put(Init) && ks.Put(GiveOnce) && ks.Put(AccountEvent) && ks.Put(BackUpTime) &&
        ks.Put(RepeatRewardEvent) && ks.Put(BeforeEventId) && ks.Put(EventItemId);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int eid) || !ks.Get(out float et) || !ks.Get(out long ct) ||
            !ks.GetWideString(out var ename) || !ks.Get(out bool init) || !ks.Get(out bool go) ||
            !ks.Get(out bool ae) || !ks.Get(out double bt) || !ks.Get(out bool rre) ||
            !ks.Get(out int bei) || !ks.Get(out int eii))
            return false;
        EventId = eid; EventTime = et; CTime = ct; EventName = ename; Init = init; GiveOnce = go;
        AccountEvent = ae; BackUpTime = bt; RepeatRewardEvent = rre; BeforeEventId = bei; EventItemId = eii;
        return true;
    }
}

public sealed class KChannelChangeInfo : IKSerializable
{
    public Dictionary<int, KUserConnectTimeEventInfo> EventDurationTime { get; } = new();
    public int EquippedTitleId { get; set; }
    public bool IsPcBang { get; set; }
    public int PcBangType { get; set; } = -1;
    public List<KInventoryItemInfo> PcBangItemInfo { get; set; } = new();
    public int UnitSlot { get; set; }
    public long SummonedRidingPetUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.PutMap(EventDurationTime, static (s, k) => s.Put(k), static (s, v) => s.Put(v)) &&
        ks.Put(EquippedTitleId) && ks.Put(IsPcBang) && ks.Put(PcBangType) &&
        ks.PutList(PcBangItemInfo) && ks.Put(UnitSlot) && ks.Put(SummonedRidingPetUid);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetMap(EventDurationTime, static s => (s.Get(out int k), k),
                static s => { var v = new KUserConnectTimeEventInfo(); return (s.Get(v), v); }) ||
            !ks.Get(out int tid) || !ks.Get(out bool ipb) || !ks.Get(out int pbt) ||
            !ks.GetList(PcBangItemInfo) || !ks.Get(out int slot) || !ks.Get(out long rpet))
            return false;
        EquippedTitleId = tid; IsPcBang = ipb; PcBangType = pbt; UnitSlot = slot; SummonedRidingPetUid = rpet;
        return true;
    }
}

public sealed class KPcBangReward : IKSerializable
{
    public int PcBangType { get; set; } = -1;
    public float ExpRate { get; set; }
    public float EdRate { get; set; }
    public float VsPointRate { get; set; }
    public bool SpiritFree { get; set; }
    public float PetExpRate { get; set; }
    public int AdditionalHenirReward { get; set; }
    public bool SecretDungeonFree { get; set; }
    public bool Avatar { get; set; }
    public bool Title { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(PcBangType) && ks.Put(ExpRate) && ks.Put(EdRate) && ks.Put(VsPointRate) &&
        ks.Put(SpiritFree) && ks.Put(PetExpRate) && ks.Put(AdditionalHenirReward) &&
        ks.Put(SecretDungeonFree) && ks.Put(Avatar) && ks.Put(Title);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int pbt) || !ks.Get(out float exp) || !ks.Get(out float ed) ||
            !ks.Get(out float vsp) || !ks.Get(out bool sf) || !ks.Get(out float pexp) ||
            !ks.Get(out int henir) || !ks.Get(out bool sdf) || !ks.Get(out bool av) || !ks.Get(out bool tit))
            return false;
        PcBangType = pbt; ExpRate = exp; EdRate = ed; VsPointRate = vsp; SpiritFree = sf;
        PetExpRate = pexp; AdditionalHenirReward = henir; SecretDungeonFree = sdf; Avatar = av; Title = tit;
        return true;
    }
}

public sealed class KVillageLagInfo : IKSerializable
{
    public int LagType { get; set; } = -1;
    public int PartyType { get; set; } = -1;
    public int PartyUserNum { get; set; } = -1;
    public int MapId { get; set; } = -1;
    public int VillageUserNum { get; set; } = -1;
    public float LagTime { get; set; } = -1f;
    public float PlayTime { get; set; } = -1f;
    public int LagTotalNum { get; set; } = -1;
    public float LagTotalTime { get; set; } = -1f;

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(LagType) && ks.Put(PartyType) && ks.Put(PartyUserNum) && ks.Put(MapId) &&
        ks.Put(VillageUserNum) && ks.Put(LagTime) && ks.Put(PlayTime) && ks.Put(LagTotalNum) && ks.Put(LagTotalTime);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int lt) || !ks.Get(out int pt) || !ks.Get(out int pun) || !ks.Get(out int mid) ||
            !ks.Get(out int vun) || !ks.Get(out float ltime) || !ks.Get(out float ptime) ||
            !ks.Get(out int ltn) || !ks.Get(out float ltt))
            return false;
        LagType = lt; PartyType = pt; PartyUserNum = pun; MapId = mid; VillageUserNum = vun;
        LagTime = ltime; PlayTime = ptime; LagTotalNum = ltn; LagTotalTime = ltt;
        return true;
    }
}

public sealed class KFieldLagInfo : IKSerializable
{
    public int LagType { get; set; } = -1;
    public int PartyType { get; set; } = -1;
    public int PartyUserNum { get; set; } = -1;
    public int MapId { get; set; } = -1;
    public int FieldUserNum { get; set; } = -1;
    public float LagTime { get; set; } = -1f;
    public float PlayTime { get; set; } = -1f;
    public int LagTotalNum { get; set; } = -1;
    public float LagTotalTime { get; set; } = -1f;

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(LagType) && ks.Put(PartyType) && ks.Put(PartyUserNum) && ks.Put(MapId) &&
        ks.Put(FieldUserNum) && ks.Put(LagTime) && ks.Put(PlayTime) && ks.Put(LagTotalNum) && ks.Put(LagTotalTime);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int lt) || !ks.Get(out int pt) || !ks.Get(out int pun) || !ks.Get(out int mid) ||
            !ks.Get(out int fun) || !ks.Get(out float ltime) || !ks.Get(out float ptime) ||
            !ks.Get(out int ltn) || !ks.Get(out float ltt))
            return false;
        LagType = lt; PartyType = pt; PartyUserNum = pun; MapId = mid; FieldUserNum = fun;
        LagTime = ltime; PlayTime = ptime; LagTotalNum = ltn; LagTotalTime = ltt;
        return true;
    }
}

public sealed class KDungeonLagInfo : IKSerializable
{
    public int LagType { get; set; } = -1;
    public int PartyType { get; set; } = -1;
    public int PartyUserNum { get; set; } = -1;
    public int MapId { get; set; } = -1;
    public int Difficulty { get; set; } = -1;
    public int Stage { get; set; } = -1;
    public int Block { get; set; } = -1;
    public float LagTime { get; set; } = -1f;
    public float PlayTime { get; set; } = -1f;
    public int LagTotalNum { get; set; } = -1;
    public float LagTotalTime { get; set; } = -1f;

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(LagType) && ks.Put(PartyType) && ks.Put(PartyUserNum) && ks.Put(MapId) &&
        ks.Put(Difficulty) && ks.Put(Stage) && ks.Put(Block) &&
        ks.Put(LagTime) && ks.Put(PlayTime) && ks.Put(LagTotalNum) && ks.Put(LagTotalTime);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int lt) || !ks.Get(out int pt) || !ks.Get(out int pun) || !ks.Get(out int mid) ||
            !ks.Get(out int diff) || !ks.Get(out int stg) || !ks.Get(out int blk) ||
            !ks.Get(out float ltime) || !ks.Get(out float ptime) ||
            !ks.Get(out int ltn) || !ks.Get(out float ltt))
            return false;
        LagType = lt; PartyType = pt; PartyUserNum = pun; MapId = mid; Difficulty = diff; Stage = stg; Block = blk;
        LagTime = ltime; PlayTime = ptime; LagTotalNum = ltn; LagTotalTime = ltt;
        return true;
    }
}

public sealed class KPvPLagInfo : IKSerializable
{
    public int LagType { get; set; } = -1;
    public int MapId { get; set; } = -1;
    public int PvpUserNum { get; set; } = -1;
    public int RuleType { get; set; } = -1;
    public int Round { get; set; } = -1;
    public float LagTime { get; set; } = -1f;
    public float PlayTime { get; set; } = -1f;
    public int LagTotalNum { get; set; } = -1;
    public float LagTotalTime { get; set; } = -1f;

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(LagType) && ks.Put(MapId) && ks.Put(PvpUserNum) && ks.Put(RuleType) && ks.Put(Round) &&
        ks.Put(LagTime) && ks.Put(PlayTime) && ks.Put(LagTotalNum) && ks.Put(LagTotalTime);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int lt) || !ks.Get(out int mid) || !ks.Get(out int pun) || !ks.Get(out int rt) || !ks.Get(out int rnd) ||
            !ks.Get(out float ltime) || !ks.Get(out float ptime) ||
            !ks.Get(out int ltn) || !ks.Get(out float ltt))
            return false;
        LagType = lt; MapId = mid; PvpUserNum = pun; RuleType = rt; Round = rnd;
        LagTime = ltime; PlayTime = ptime; LagTotalNum = ltn; LagTotalTime = ltt;
        return true;
    }
}

public sealed class KRewardItem : IKSerializable
{
    public int ItemId { get; set; }
    public int Quantity { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(ItemId) && ks.Put(Quantity);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int id) || !ks.Get(out int qty)) return false;
        ItemId = id; Quantity = qty;
        return true;
    }
}

public sealed class KRandomItemResult : IKSerializable
{
    public int ItemCount { get; set; }
    public bool IsSealItem { get; set; }
    public double Rate { get; set; }
    public double ItemTotalCount { get; set; }
    public bool IsNotifyMsgAttration { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(ItemCount) && ks.Put(IsSealItem) && ks.Put(Rate) && ks.Put(ItemTotalCount) && ks.Put(IsNotifyMsgAttration);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ic) || !ks.Get(out bool seal) || !ks.Get(out double rate) ||
            !ks.Get(out double tot) || !ks.Get(out bool notify))
            return false;
        ItemCount = ic; IsSealItem = seal; Rate = rate; ItemTotalCount = tot; IsNotifyMsgAttration = notify;
        return true;
    }
}

public sealed class KRandomItemMappingToInfoClient : IKSerializable
{
    public int BeforeGroupId { get; set; }
    public int AfterGroupId { get; set; }
    public bool CheckMapping { get; set; }
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(BeforeGroupId) && ks.Put(AfterGroupId) && ks.Put(CheckMapping) &&
        ks.PutWideString(StartTime) && ks.PutWideString(EndTime);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int bg) || !ks.Get(out int ag) || !ks.Get(out bool cm) ||
            !ks.GetWideString(out var st) || !ks.GetWideString(out var et))
            return false;
        BeforeGroupId = bg; AfterGroupId = ag; CheckMapping = cm; StartTime = st; EndTime = et;
        return true;
    }
}

public sealed class KRecommendUserInfo : IKSerializable
{
    public const int RuitLogin = 0;
    public const int RuitLogout = 1;
    public const int RuitLevelUp = 2;

    public long UnitUid { get; set; }
    public string NickName { get; set; } = string.Empty;
    public byte Level { get; set; }
    public string RecommendDate { get; set; } = string.Empty;
    public bool IsOnline { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(UnitUid) && ks.PutWideString(NickName) && ks.Put(Level) &&
        ks.PutWideString(RecommendDate) && ks.Put(IsOnline);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.GetWideString(out var nick) || !ks.Get(out byte lv) ||
            !ks.GetWideString(out var rd) || !ks.Get(out bool onl))
            return false;
        UnitUid = uid; NickName = nick; Level = lv; RecommendDate = rd; IsOnline = onl;
        return true;
    }
}

// =========================================================================
// CashShop / Global Billing / AdminCheat Common DTOs (TW ServerDefine Parity)
// =========================================================================

public sealed class KGlobalCashInfo : IKSerializable
{
    public const int GctPublisherCash = 0;
    public const int GctKogElswordCash = 1;
    public const int GctKogElswordBonusPoint = 2;
    public const int GctPublisherElswordCash = 3;
    public const int TotalNum = 4;

    public uint[] Cash { get; set; } = new uint[TotalNum];

    public bool Put(KSerializer ks, RegionProfile region)
    {
        for (int i = 0; i < TotalNum; i++)
        {
            uint val = Cash != null && i < Cash.Length ? Cash[i] : 0u;
            if (!ks.Put(val)) return false;
        }
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (Cash == null || Cash.Length < TotalNum) Cash = new uint[TotalNum];
        for (int i = 0; i < TotalNum; i++)
        {
            if (!ks.Get(out uint v)) return false;
            Cash[i] = v;
        }
        return true;
    }
}

public sealed class KNXBTBonusProductInfo : IKSerializable
{
    public uint ProductNo { get; set; }
    public string Extend { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(ProductNo) && ks.PutWideString(Extend);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint pno) || !ks.GetWideString(out var ext)) return false;
        ProductNo = pno; Extend = ext;
        return true;
    }
}

public sealed class KNXBTProductInfo : IKSerializable
{
    public uint ProductNo { get; set; }
    public uint RelationProductNo { get; set; }
    public short ProductExpire { get; set; }
    public ushort ProductPieces { get; set; }
    public string ProductId { get; set; } = string.Empty;
    public string ProductGuid { get; set; } = string.Empty;
    public uint PaymentType { get; set; }
    public string ProductType { get; set; } = string.Empty;
    public uint SalePrice { get; set; }
    public uint CategoryNo { get; set; }
    public List<KNXBTBonusProductInfo> BonusProductInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(ProductNo) && ks.Put(RelationProductNo) && ks.Put(ProductExpire) &&
        ks.Put(ProductPieces) && ks.PutWideString(ProductId) && ks.PutWideString(ProductGuid) &&
        ks.Put(PaymentType) && ks.PutWideString(ProductType) && ks.Put(SalePrice) &&
        ks.Put(CategoryNo) && ks.PutList(BonusProductInfo);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint pno) || !ks.Get(out uint rel) || !ks.Get(out short exp) ||
            !ks.Get(out ushort pcs) || !ks.GetWideString(out var pid) || !ks.GetWideString(out var guid) ||
            !ks.Get(out uint pay) || !ks.GetWideString(out var ptype) || !ks.Get(out uint price) ||
            !ks.Get(out uint cat) || !ks.GetList(BonusProductInfo))
            return false;
        ProductNo = pno; RelationProductNo = rel; ProductExpire = exp; ProductPieces = pcs;
        ProductId = pid; ProductGuid = guid; PaymentType = pay; ProductType = ptype;
        SalePrice = price; CategoryNo = cat;
        return true;
    }
}

public sealed class KNXBTOrderInfo : IKSerializable
{
    public uint OrderNo { get; set; }
    public uint ProductNo { get; set; }
    public byte ProductKind { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public short ProductExpire { get; set; }
    public ushort ProductPieces { get; set; }
    public ushort OrderQuantity { get; set; }
    public ushort RemainOrderQuantity { get; set; }
    public byte IsPresent { get; set; }
    public byte IsRead { get; set; }
    public byte SenderServerNo { get; set; }
    public string SenderGameId { get; set; } = string.Empty;
    public string SenderPresentMessage { get; set; } = string.Empty;
    public string ProductAttribute0 { get; set; } = string.Empty;
    public string ProductAttribute1 { get; set; } = string.Empty;
    public string ProductAttribute2 { get; set; } = string.Empty;
    public string ProductAttribute3 { get; set; } = string.Empty;
    public string ProductAttribute4 { get; set; } = string.Empty;
    public string ExtendValue { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(OrderNo) && ks.Put(ProductNo) && ks.Put(ProductKind) &&
        ks.PutWideString(ProductName) && ks.PutWideString(ProductId) &&
        ks.Put(ProductExpire) && ks.Put(ProductPieces) && ks.Put(OrderQuantity) &&
        ks.Put(RemainOrderQuantity) && ks.Put(IsPresent) && ks.Put(IsRead) &&
        ks.Put(SenderServerNo) && ks.PutWideString(SenderGameId) &&
        ks.PutWideString(SenderPresentMessage) && ks.PutWideString(ProductAttribute0) &&
        ks.PutWideString(ProductAttribute1) && ks.PutWideString(ProductAttribute2) &&
        ks.PutWideString(ProductAttribute3) && ks.PutWideString(ProductAttribute4) &&
        ks.PutWideString(ExtendValue);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint ono) || !ks.Get(out uint pno) || !ks.Get(out byte pk) ||
            !ks.GetWideString(out var pname) || !ks.GetWideString(out var pid) ||
            !ks.Get(out short exp) || !ks.Get(out ushort pcs) || !ks.Get(out ushort oq) ||
            !ks.Get(out ushort roq) || !ks.Get(out byte ip) || !ks.Get(out byte ir) ||
            !ks.Get(out byte ssn) || !ks.GetWideString(out var sgi) ||
            !ks.GetWideString(out var spm) || !ks.GetWideString(out var a0) ||
            !ks.GetWideString(out var a1) || !ks.GetWideString(out var a2) ||
            !ks.GetWideString(out var a3) || !ks.GetWideString(out var a4) ||
            !ks.GetWideString(out var ext))
            return false;
        OrderNo = ono; ProductNo = pno; ProductKind = pk; ProductName = pname; ProductId = pid;
        ProductExpire = exp; ProductPieces = pcs; OrderQuantity = oq; RemainOrderQuantity = roq;
        IsPresent = ip; IsRead = ir; SenderServerNo = ssn; SenderGameId = sgi; SenderPresentMessage = spm;
        ProductAttribute0 = a0; ProductAttribute1 = a1; ProductAttribute2 = a2;
        ProductAttribute3 = a3; ProductAttribute4 = a4; ExtendValue = ext;
        return true;
    }
}

public sealed class KNXBTPackageInfo : IKSerializable
{
    public uint OrderNo { get; set; }
    public uint PackageProductNo { get; set; }
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

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(OrderNo) && ks.Put(PackageProductNo) && ks.Put(ProductNo) &&
        ks.PutWideString(ProductName) && ks.PutWideString(ProductId) &&
        ks.Put(ProductExpire) && ks.Put(ProductPieces) &&
        ks.PutWideString(ProductAttribute0) && ks.PutWideString(ProductAttribute1) &&
        ks.PutWideString(ProductAttribute2) && ks.PutWideString(ProductAttribute3) &&
        ks.PutWideString(ProductAttribute4);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint ono) || !ks.Get(out uint ppno) || !ks.Get(out uint pno) ||
            !ks.GetWideString(out var pname) || !ks.GetWideString(out var pid) ||
            !ks.Get(out short exp) || !ks.Get(out ushort pcs) ||
            !ks.GetWideString(out var a0) || !ks.GetWideString(out var a1) ||
            !ks.GetWideString(out var a2) || !ks.GetWideString(out var a3) ||
            !ks.GetWideString(out var a4))
            return false;
        OrderNo = ono; PackageProductNo = ppno; ProductNo = pno; ProductName = pname; ProductId = pid;
        ProductExpire = exp; ProductPieces = pcs;
        ProductAttribute0 = a0; ProductAttribute1 = a1; ProductAttribute2 = a2;
        ProductAttribute3 = a3; ProductAttribute4 = a4;
        return true;
    }
}

public sealed class KNXBTPickUpPackageInfo : IKSerializable
{
    public uint ProductNo { get; set; }
    public string ProductId { get; set; } = string.Empty;
    public short ProductExpire { get; set; }
    public ushort ProductPieces { get; set; }
    public int SubProductNo { get; set; } // SERV_GLOBAL_CASH_PACKAGE

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(ProductNo) && ks.PutWideString(ProductId) && ks.Put(ProductExpire) &&
        ks.Put(ProductPieces) && ks.Put(SubProductNo);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint pno) || !ks.GetWideString(out var pid) || !ks.Get(out short exp) ||
            !ks.Get(out ushort pcs) || !ks.Get(out int sub))
            return false;
        ProductNo = pno; ProductId = pid; ProductExpire = exp; ProductPieces = pcs; SubProductNo = sub;
        return true;
    }
}

public sealed class KNXBTPurchaseCouponAckInfo : IKSerializable
{
    public uint ProductNo { get; set; }
    public ushort OrderQuantity { get; set; }
    public int CouponUseApproval { get; set; }
    public string ExtendValue { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(ProductNo) && ks.Put(OrderQuantity) && ks.Put(CouponUseApproval) && ks.PutWideString(ExtendValue);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint pno) || !ks.Get(out ushort qty) || !ks.Get(out int app) ||
            !ks.GetWideString(out var ext))
            return false;
        ProductNo = pno; OrderQuantity = qty; CouponUseApproval = app; ExtendValue = ext;
        return true;
    }
}

public sealed class KBillProductInfo : IKSerializable
{
    public int ProductNo { get; set; }
    public int ProductId { get; set; }
    public bool Sale { get; set; }
    public bool Event { get; set; }
    public bool Show { get; set; } = true;
    public bool IsCashInvenSkip { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public sbyte CategoryNo { get; set; }
    public int SalePrice { get; set; }
    public int RealPrice { get; set; }
    public sbyte Period { get; set; }
    public sbyte Quantity { get; set; } = 1;
    public bool EnableGift { get; set; } = true;
    public sbyte LimitLevel { get; set; }
    public string EndDate { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(ProductNo) && ks.Put(ProductId) && ks.Put(Sale) && ks.Put(Event) &&
        ks.Put(Show) && ks.Put(IsCashInvenSkip) && ks.PutWideString(ProductName) &&
        ks.Put(CategoryNo) && ks.Put(SalePrice) && ks.Put(RealPrice) && ks.Put(Period) &&
        ks.Put(Quantity) && ks.Put(EnableGift) && ks.Put(LimitLevel) && ks.PutWideString(EndDate);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int pno) || !ks.Get(out int pid) || !ks.Get(out bool sale) || !ks.Get(out bool ev) ||
            !ks.Get(out bool show) || !ks.Get(out bool skip) || !ks.GetWideString(out var pname) ||
            !ks.Get(out sbyte cat) || !ks.Get(out int sprice) || !ks.Get(out int rprice) || !ks.Get(out sbyte per) ||
            !ks.Get(out sbyte qty) || !ks.Get(out bool gift) || !ks.Get(out sbyte lim) || !ks.GetWideString(out var end))
            return false;
        ProductNo = pno; ProductId = pid; Sale = sale; Event = ev; Show = show; IsCashInvenSkip = skip;
        ProductName = pname; CategoryNo = cat; SalePrice = sprice; RealPrice = rprice; Period = per;
        Quantity = qty; EnableGift = gift; LimitLevel = lim; EndDate = end;
        return true;
    }
}

public sealed class KBillOrderInfo : IKSerializable
{
    public const byte BcitInvalid = 0;
    public const byte BcitBuyItem = 1;
    public const byte BcitCoupon = 2;
    public const byte BcitGift = 3;

    public long TransNo { get; set; } = -1;
    public string OrderId { get; set; } = string.Empty;
    public KBillProductInfo BillProductInfo { get; set; } = new();
    public List<int> SocketOption { get; set; } = new();
    public byte Type { get; set; }
    public string SenderNickName { get; set; } = string.Empty;
    public string SenderPresentMessage { get; set; } = string.Empty;
    public long FromUnitUid { get; set; }
    public byte ProductKind { get; set; } // SERV_GLOBAL_CASH_PACKAGE

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(TransNo) && ks.PutWideString(OrderId) && ks.Put(BillProductInfo) &&
        ks.PutList(SocketOption, static (s, v) => s.Put(v)) && ks.Put(Type) &&
        ks.PutWideString(SenderNickName) && ks.PutWideString(SenderPresentMessage) &&
        ks.Put(FromUnitUid) && ks.Put(ProductKind);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long tno) || !ks.GetWideString(out var oid) || !ks.Get(BillProductInfo) ||
            !ks.GetList(SocketOption, static s => (s.Get(out int v), v)) || !ks.Get(out byte typ) ||
            !ks.GetWideString(out var snick) || !ks.GetWideString(out var smsg) ||
            !ks.Get(out long fromUid) || !ks.Get(out byte pkind))
            return false;
        TransNo = tno; OrderId = oid; Type = typ; SenderNickName = snick;
        SenderPresentMessage = smsg; FromUnitUid = fromUid; ProductKind = pkind;
        return true;
    }
}

public sealed class KBillPackageInfo : IKSerializable
{
    public int PackageNo { get; set; }
    public List<int> VecProductNo { get; set; } = new();
    public uint ProductNo { get; set; }
    public int ProductId { get; set; }
    public string WstrProductId { get; set; } = string.Empty;
    public short ProductExpire { get; set; }
    public ushort ProductPieces { get; set; }
    public List<int> SocketOption { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(PackageNo) && ks.PutList(VecProductNo, static (s, v) => s.Put(v)) &&
        ks.Put(ProductNo) && ks.Put(ProductId) && ks.PutWideString(WstrProductId) &&
        ks.Put(ProductExpire) && ks.Put(ProductPieces) &&
        ks.PutList(SocketOption, static (s, v) => s.Put(v));

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int pkg) || !ks.GetList(VecProductNo, static s => (s.Get(out int v), v)) ||
            !ks.Get(out uint pno) || !ks.Get(out int pid) || !ks.GetWideString(out var wp) ||
            !ks.Get(out short exp) || !ks.Get(out ushort pcs) ||
            !ks.GetList(SocketOption, static s => (s.Get(out int v), v)))
            return false;
        PackageNo = pkg; ProductNo = pno; ProductId = pid; WstrProductId = wp;
        ProductExpire = exp; ProductPieces = pcs;
        return true;
    }
}

public sealed class kDisCountItemInfo : IKSerializable
{
    public int DisCountProNo { get; set; }
    public int BasicProNo { get; set; }
    public List<int> DisCountKeepItem { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(DisCountProNo) && ks.Put(BasicProNo) && ks.PutList(DisCountKeepItem, static (s, v) => s.Put(v));

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int d) || !ks.Get(out int b) || !ks.GetList(DisCountKeepItem, static s => (s.Get(out int v), v)))
            return false;
        DisCountProNo = d; BasicProNo = b;
        return true;
    }
}

public sealed class KCouponBoxTargetItem : IKSerializable
{
    public int ItemQuantity { get; set; }
    public short ItemUseDuration { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(ItemQuantity) && ks.Put(ItemUseDuration) && ks.PutWideString(ItemCode) && ks.PutWideString(ItemName);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int qty) || !ks.Get(out short dur) || !ks.GetWideString(out var code) || !ks.GetWideString(out var name))
            return false;
        ItemQuantity = qty; ItemUseDuration = dur; ItemCode = code; ItemName = name;
        return true;
    }
}

public sealed class KClientCouponBox : IKSerializable
{
    public string ContractDetailName { get; set; } = string.Empty;
    public int ContractDetailNo { get; set; }
    public string CouponBoxType { get; set; } = string.Empty;
    public string CouponBoxTypeText { get; set; } = string.Empty;
    public int CouponCardNo { get; set; }
    public int CouponCardStatus { get; set; }
    public string CouponCardStatusText { get; set; } = string.Empty;
    public string DiscountValue { get; set; } = string.Empty;
    public string ExpireDateTime { get; set; } = string.Empty;
    public string ObtainDateTime { get; set; } = string.Empty;
    public string ValidDateRange { get; set; } = string.Empty;
    public string UsedDateTime { get; set; } = string.Empty;
    public List<KCouponBoxTargetItem> TargetItem { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.PutWideString(ContractDetailName) && ks.Put(ContractDetailNo) &&
        ks.PutWideString(CouponBoxType) && ks.PutWideString(CouponBoxTypeText) &&
        ks.Put(CouponCardNo) && ks.Put(CouponCardStatus) && ks.PutWideString(CouponCardStatusText) &&
        ks.PutWideString(DiscountValue) && ks.PutWideString(ExpireDateTime) &&
        ks.PutWideString(ObtainDateTime) && ks.PutWideString(ValidDateRange) &&
        ks.PutWideString(UsedDateTime) && ks.PutList(TargetItem);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out var cdn) || !ks.Get(out int cdno) ||
            !ks.GetWideString(out var cbt) || !ks.GetWideString(out var cbtt) ||
            !ks.Get(out int ccno) || !ks.Get(out int ccs) || !ks.GetWideString(out var ccst) ||
            !ks.GetWideString(out var dv) || !ks.GetWideString(out var exp) ||
            !ks.GetWideString(out var obt) || !ks.GetWideString(out var vdr) ||
            !ks.GetWideString(out var used) || !ks.GetList(TargetItem))
            return false;
        ContractDetailName = cdn; ContractDetailNo = cdno; CouponBoxType = cbt; CouponBoxTypeText = cbtt;
        CouponCardNo = ccno; CouponCardStatus = ccs; CouponCardStatusText = ccst;
        DiscountValue = dv; ExpireDateTime = exp; ObtainDateTime = obt; ValidDateRange = vdr; UsedDateTime = used;
        return true;
    }
}

public sealed class KDiscountCoupon : IKSerializable
{
    public int CouponCardNo { get; set; }
    public string ContractDetailName { get; set; } = string.Empty;
    public string DiscountValue { get; set; } = string.Empty;
    public string ExpireDateTime { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(CouponCardNo) && ks.PutWideString(ContractDetailName) &&
        ks.PutWideString(DiscountValue) && ks.PutWideString(ExpireDateTime);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int no) || !ks.GetWideString(out var name) ||
            !ks.GetWideString(out var dv) || !ks.GetWideString(out var exp))
            return false;
        CouponCardNo = no; ContractDetailName = name; DiscountValue = dv; ExpireDateTime = exp;
        return true;
    }
}

public sealed class KDiscountCouponInquriyInfo : IKSerializable
{
    public uint ProductNo { get; set; }
    public ushort OrderQuantity { get; set; } = 1;

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(ProductNo) && ks.Put(OrderQuantity);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint pno) || !ks.Get(out ushort qty)) return false;
        ProductNo = pno; OrderQuantity = qty;
        return true;
    }
}

public sealed class KAdminCheatSkill : IKSerializable
{
    public int SkillLevel { get; set; }
    public int SkillCsPoint { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(SkillLevel) && ks.Put(SkillCsPoint);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int lv) || !ks.Get(out int csp)) return false;
        SkillLevel = lv; SkillCsPoint = csp;
        return true;
    }
}

public sealed class KHeroPvpUserInfo : IKSerializable
{
    public long ServerGroupId { get; set; } = -1;
    public string NickName { get; set; } = string.Empty;
    public bool On { get; set; }
    public bool Connect { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(ServerGroupId) && ks.PutWideString(NickName) && ks.Put(On) && ks.Put(Connect);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long sg) || !ks.GetWideString(out var nick) || !ks.Get(out bool on) || !ks.Get(out bool conn))
            return false;
        ServerGroupId = sg; NickName = nick; On = on; Connect = conn;
        return true;
    }
}

public sealed class KCompleteQuestInfo : IKSerializable
{
    public int QuestId { get; set; }
    public int CompleteCount { get; set; }
    public long CompleteDate { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(QuestId) && ks.Put(CompleteCount) && ks.Put(CompleteDate);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int qid) || !ks.Get(out int cnt) || !ks.Get(out long dt))
            return false;
        QuestId = qid; CompleteCount = cnt; CompleteDate = dt;
        return true;
    }
}

public sealed class KUserSkillData : IKSerializable
{
    public short SkillId { get; set; }
    public byte SkillLevel { get; set; }
    public byte SkillCsPoint { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(SkillId) && ks.Put(SkillLevel) && ks.Put(SkillCsPoint);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out short sid) || !ks.Get(out byte lvl) || !ks.Get(out byte csp))
            return false;
        SkillId = sid; SkillLevel = lvl; SkillCsPoint = csp;
        return true;
    }
}

public sealed class KUserSkillPageData : IKSerializable
{
    public const int EquippedSkillSlotCount = 4;
    public KSkillData[] EquippedSkill { get; } = [new(), new(), new(), new()];
    public KSkillData[] EquippedSkillSlotB { get; } = [new(), new(), new(), new()];
    public ushort SkillPoint { get; set; }
    public ushort CashSkillPoint { get; set; }
    public List<KUserSkillData> UserSkillData { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        for (int i = 0; i < EquippedSkillSlotCount; i++)
            if (!ks.Put(EquippedSkill[i])) return false;
        for (int i = 0; i < EquippedSkillSlotCount; i++)
            if (!ks.Put(EquippedSkillSlotB[i])) return false;
        return ks.Put(SkillPoint) && ks.Put(CashSkillPoint) && ks.PutList(UserSkillData);
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        for (int i = 0; i < EquippedSkillSlotCount; i++)
            if (!ks.Get(EquippedSkill[i])) return false;
        for (int i = 0; i < EquippedSkillSlotCount; i++)
            if (!ks.Get(EquippedSkillSlotB[i])) return false;
        if (!ks.Get(out ushort sp) || !ks.Get(out ushort csp) || !ks.GetList(UserSkillData))
            return false;
        SkillPoint = sp; CashSkillPoint = csp;
        return true;
    }
}

public sealed class KKeyboardMappingInfo : IKSerializable
{
    public Dictionary<short, short> KeyboardMappingInfo { get; set; } = new();
    public Dictionary<short, short> GamePadMappingInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutMap(KeyboardMappingInfo, static (s, k) => s.Put(k), static (s, v) => s.Put(v)))
            return false;
        if (region.KeyMappingInt &&
            !ks.PutMap(GamePadMappingInfo, static (s, k) => s.Put(k), static (s, v) => s.Put(v)))
            return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetMap(KeyboardMappingInfo, static s => (s.Get(out short k), k), static s => (s.Get(out short v), v)))
            return false;
        if (region.KeyMappingInt &&
            !ks.GetMap(GamePadMappingInfo, static s => (s.Get(out short k), k), static s => (s.Get(out short v), v)))
            return false;
        return true;
    }
}

public sealed class KLocalRankingUserInfo : IKSerializable
{
    public long UserUid { get; set; }
    public int Category { get; set; } = -1;
    public long Birth { get; set; }
    public string Profile { get; set; } = string.Empty;
    public byte Filter { get; set; }
    public byte Gender { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(UserUid) && ks.Put(Category) && ks.Put(Birth) && ks.PutWideString(Profile) &&
        ks.Put(Filter) && ks.Put(Gender);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.Get(out int cat) || !ks.Get(out long birth) ||
            !ks.GetWideString(out var prof) || !ks.Get(out byte filter) || !ks.Get(out byte gender))
            return false;
        UserUid = uid; Category = cat; Birth = birth; Profile = prof; Filter = filter; Gender = gender;
        return true;
    }
}

public sealed class KLocalRankingUnitInfo : IKSerializable
{
    public long UnitUid { get; set; }
    public string NickName { get; set; } = string.Empty;
    public byte Level { get; set; }
    public sbyte UnitClass { get; set; }
    public int[] Point { get; } = new int[2];

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(UnitUid) && ks.PutWideString(NickName) && ks.Put(Level) && ks.Put(UnitClass) &&
        ks.Put(Point[0]) && ks.Put(Point[1]);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.GetWideString(out var nick) || !ks.Get(out byte lvl) ||
            !ks.Get(out sbyte uc) || !ks.Get(out int p0) || !ks.Get(out int p1))
            return false;
        UnitUid = uid; NickName = nick; Level = lvl; UnitClass = uc; Point[0] = p0; Point[1] = p1;
        return true;
    }
}

public sealed class KLocalRankingRankerInfo : IKSerializable
{
    public int Rank { get; set; }
    public KLocalRankingUserInfo UserInfo { get; set; } = new();
    public KLocalRankingUnitInfo UnitInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Rank) && ks.Put(UserInfo) && ks.Put(UnitInfo);

    public bool Get(KSerializer ks, RegionProfile region) =>
        ks.Get(out int r) && ks.Get(UserInfo) && ks.Get(UnitInfo) && ((Rank = r) == r);
}

public sealed class KConnectionUnitInfo : IKSerializable
{
    public long UnitUid { get; set; }
    public int KnmSn { get; set; }
    public sbyte UnitClass { get; set; }
    public byte Level { get; set; }
    public string CharName { get; set; } = string.Empty;
    public sbyte Rank { get; set; } // SERV_PVP_NEW_SYSTEM + SERV_2012_PVP_SEASON2
    public int RPoint { get; set; }
    public uint PvpRanking { get; set; }
    public int Win { get; set; }
    public int Lose { get; set; }
    public uint KillNum { get; set; }
    public int ChannelId { get; set; }
    public sbyte State { get; set; }
    public int StateCode { get; set; }
    public long PartyUid { get; set; }
    public long RoomUid { get; set; }
    public bool Public { get; set; }
    public bool CanIntrude { get; set; }
    public int DungeonId { get; set; }
    public sbyte RoomListId { get; set; }
    public string GuildName { get; set; } = string.Empty; // GUILD_TEST
    public KLocalRankingUserInfo LocalRankingUserInfo { get; set; } = new(); // SERV_LOCAL_RANKING_SYSTEM

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(UnitUid) && ks.Put(KnmSn) && ks.Put(UnitClass) && ks.Put(Level) && ks.PutWideString(CharName) &&
        ks.Put(Rank) && ks.Put(RPoint) && ks.Put(PvpRanking) && ks.Put(Win) && ks.Put(Lose) && ks.Put(KillNum) &&
        ks.Put(ChannelId) && ks.Put(State) && ks.Put(StateCode) && ks.Put(PartyUid) && ks.Put(RoomUid) &&
        ks.Put(Public) && ks.Put(CanIntrude) && ks.Put(DungeonId) && ks.Put(RoomListId) &&
        ks.PutWideString(GuildName) &&
        (!region.LocalRankingSystem || ks.Put(LocalRankingUserInfo));

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.Get(out int knm) || !ks.Get(out sbyte uc) || !ks.Get(out byte lvl) ||
            !ks.GetWideString(out var cname) || !ks.Get(out sbyte rk) || !ks.Get(out int rp) ||
            !ks.Get(out uint pvpRank) || !ks.Get(out int win) || !ks.Get(out int lose) || !ks.Get(out uint kill) ||
            !ks.Get(out int chId) || !ks.Get(out sbyte st) || !ks.Get(out int stCode) ||
            !ks.Get(out long puid) || !ks.Get(out long ruid) || !ks.Get(out bool pub) ||
            !ks.Get(out bool intr) || !ks.Get(out int did) || !ks.Get(out sbyte rlid) ||
            !ks.GetWideString(out var gname))
            return false;
        if (region.LocalRankingSystem && !ks.Get(LocalRankingUserInfo))
            return false;
        UnitUid = uid; KnmSn = knm; UnitClass = uc; Level = lvl; CharName = cname; Rank = rk; RPoint = rp;
        PvpRanking = pvpRank; Win = win; Lose = lose; KillNum = kill; ChannelId = chId; State = st; StateCode = stCode;
        PartyUid = puid; RoomUid = ruid; Public = pub; CanIntrude = intr; DungeonId = did; RoomListId = rlid;
        GuildName = gname;
        return true;
    }
}

public sealed class KTutorialUnitInfo : IKSerializable
{
    public long UnitUid { get; set; }
    public byte Level { get; set; }
    public string NickName { get; set; } = string.Empty;
    public bool IsOnline { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(UnitUid) && ks.Put(Level) && ks.PutWideString(NickName) && ks.Put(IsOnline);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.Get(out byte lvl) || !ks.GetWideString(out var nick) || !ks.Get(out bool on))
            return false;
        UnitUid = uid; Level = lvl; NickName = nick; IsOnline = on;
        return true;
    }
}

public sealed class KBuffBehaviorFactor : IKSerializable
{
    public uint Type { get; set; }
    public List<float> Values { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Type) && ks.PutList(Values, static (s, v) => s.Put(v));

    public bool Get(KSerializer ks, RegionProfile region) =>
        ks.Get(out uint t) && ks.GetList(Values, static s => (s.Get(out float v), v)) && ((Type = t) == t);
}

public sealed class KBuffFinalizerFactor : IKSerializable
{
    public uint Type { get; set; }
    public List<float> Values { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Type) && ks.PutList(Values, static (s, v) => s.Put(v));

    public bool Get(KSerializer ks, RegionProfile region) =>
        ks.Get(out uint t) && ks.GetList(Values, static s => (s.Get(out float v), v)) && ((Type = t) == t);
}

public sealed class KBuffIdentity : IKSerializable
{
    public int BuffTempletId { get; set; }
    public uint UniqueNum { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(BuffTempletId) && ks.Put(UniqueNum);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int tid) || !ks.Get(out uint unum)) return false;
        BuffTempletId = tid; UniqueNum = unum;
        return true;
    }
}

public sealed class KBuffFactor : IKSerializable
{
    public List<KBuffBehaviorFactor> BuffBehaviorFactors { get; set; } = new();
    public List<KBuffFinalizerFactor> BuffFinalizerFactors { get; set; } = new();
    public KBuffIdentity BuffIdentity { get; set; } = new();
    public long MesGameUnitUid { get; set; }
    public float AccumulationMultiplier { get; set; }
    public byte AccumulationCountNow { get; set; }
    public bool IsMesGameUnitNpc { get; set; }
    public int FactorId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.PutList(BuffBehaviorFactors) && ks.PutList(BuffFinalizerFactors) && ks.Put(BuffIdentity) &&
        ks.Put(AccumulationMultiplier) && ks.Put(AccumulationCountNow) && ks.Put(FactorId);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetList(BuffBehaviorFactors) || !ks.GetList(BuffFinalizerFactors) || !ks.Get(BuffIdentity) ||
            !ks.Get(out float mult) || !ks.Get(out byte cnt) || !ks.Get(out int fid))
            return false;
        AccumulationMultiplier = mult; AccumulationCountNow = cnt; FactorId = fid;
        return true;
    }
}

public sealed class KNpcUnitBuffInfo : IKSerializable
{
    public int NpcUid { get; set; }
    public List<KBuffFactor> BuffFactors { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(NpcUid) && ks.PutList(BuffFactors);

    public bool Get(KSerializer ks, RegionProfile region) =>
        ks.Get(out int uid) && ks.GetList(BuffFactors) && ((NpcUid = uid) == uid);
}

public sealed class KBuffInfo : IKSerializable
{
    public KBuffFactor FactorInfo { get; set; } = new();
    public long BuffStartTime { get; set; }
    public long BuffEndTime { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(FactorInfo) && ks.Put(BuffStartTime) && ks.Put(BuffEndTime);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(FactorInfo) || !ks.Get(out long st) || !ks.Get(out long et)) return false;
        BuffStartTime = st; BuffEndTime = et;
        return true;
    }
}

public sealed class KSubQuestData : IKSerializable
{
    public int InInventoryItemCount { get; set; }
    public bool Success { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(InInventoryItemCount) && ks.Put(Success);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int cnt) || !ks.Get(out bool ok)) return false;
        InInventoryItemCount = cnt; Success = ok;
        return true;
    }
}

public sealed class KSubQuestInfo : IKSerializable
{
    public Dictionary<int, KSubQuestData> SubQuestInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.PutMap(SubQuestInfo, static (s, k) => s.Put(k), static (s, v) => s.Put(v));

    public bool Get(KSerializer ks, RegionProfile region) =>
        ks.GetMap(SubQuestInfo, static s => (s.Get(out int k), k),
            static s => { var v = new KSubQuestData(); return (s.Get(v), v); });
}

public sealed class KGamePlayStatus : IKSerializable
{
    public int MaxHp { get; set; }
    public int CurHp { get; set; }
    public int MaxMp { get; set; }
    public int CurMp { get; set; }
    public int CurHyperGage { get; set; }
    public sbyte CurHyperCount { get; set; }
    public sbyte CharAbilType { get; set; }
    public int CharAbilCount { get; set; }
    public Dictionary<int, int> SkillCoolTime { get; set; } = new();
    public Dictionary<int, int> QuickSlotCoolTime { get; set; } = new();
    public HashSet<int> PetMp { get; set; } = new();
    public Dictionary<int, int> RidingPetCoolTime { get; set; } = new(); // SERV_RIDING_PET_SYSTM

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(MaxHp) && ks.Put(CurHp) && ks.Put(MaxMp) && ks.Put(CurMp) &&
        ks.Put(CurHyperGage) && ks.Put(CurHyperCount) && ks.Put(CharAbilType) && ks.Put(CharAbilCount) &&
        ks.PutMap(SkillCoolTime, static (s, k) => s.Put(k), static (s, v) => s.Put(v)) &&
        ks.PutMap(QuickSlotCoolTime, static (s, k) => s.Put(k), static (s, v) => s.Put(v)) &&
        ks.PutList(PetMp, static (s, v) => s.Put(v)) &&
        ks.PutMap(RidingPetCoolTime, static (s, k) => s.Put(k), static (s, v) => s.Put(v));

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int mhp) || !ks.Get(out int chp) || !ks.Get(out int mmp) || !ks.Get(out int cmp) ||
            !ks.Get(out int chg) || !ks.Get(out sbyte chc) || !ks.Get(out sbyte cat) || !ks.Get(out int cac) ||
            !ks.GetMap(SkillCoolTime, static s => (s.Get(out int k), k), static s => (s.Get(out int v), v)) ||
            !ks.GetMap(QuickSlotCoolTime, static s => (s.Get(out int k), k), static s => (s.Get(out int v), v)) ||
            !ks.GetHashSet(PetMp, static s => (s.Get(out int v), v)) ||
            !ks.GetMap(RidingPetCoolTime, static s => (s.Get(out int k), k), static s => (s.Get(out int v), v)))
            return false;
        MaxHp = mhp; CurHp = chp; MaxMp = mmp; CurMp = cmp;
        CurHyperGage = chg; CurHyperCount = chc; CharAbilType = cat; CharAbilCount = cac;
        return true;
    }
}

public sealed class KPacketGetItemOnOff : IKSerializable
{
    public int Type { get; set; }
    public int ItemId { get; set; }
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Type) && ks.Put(ItemId) && ks.PutWideString(StartTime) && ks.PutWideString(EndTime);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int t) || !ks.Get(out int iid) || !ks.GetWideString(out var st) || !ks.GetWideString(out var et))
            return false;
        Type = t; ItemId = iid; StartTime = st; EndTime = et;
        return true;
    }
}

public sealed class KHenirRankingInfo : IKSerializable
{
    public int Rank { get; set; }
    public int StageCount { get; set; }
    public uint PlayTime { get; set; }
    public long RegDate { get; set; }
    public long UnitUid { get; set; }
    public string NickName { get; set; } = string.Empty;
    public sbyte UnitClass { get; set; }
    public byte Level { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Rank) && ks.Put(StageCount) && ks.Put(PlayTime) && ks.Put(RegDate) &&
        ks.Put(UnitUid) && ks.PutWideString(NickName) && ks.Put(UnitClass) && ks.Put(Level);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int r) || !ks.Get(out int sc) || !ks.Get(out uint pt) || !ks.Get(out long rd) ||
            !ks.Get(out long uid) || !ks.GetWideString(out var nick) || !ks.Get(out sbyte uc) || !ks.Get(out byte lvl))
            return false;
        Rank = r; StageCount = sc; PlayTime = pt; RegDate = rd;
        UnitUid = uid; NickName = nick; UnitClass = uc; Level = lvl;
        return true;
    }
}

public sealed class KDungeonRankingInfo : IKSerializable
{
    public int Rank { get; set; }
    public long UnitUid { get; set; }
    public string NickName { get; set; } = string.Empty;
    public sbyte UnitClass { get; set; }
    public byte Level { get; set; }
    public int Exp { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Rank) && ks.Put(UnitUid) && ks.PutWideString(NickName) && ks.Put(UnitClass) &&
        ks.Put(Level) && ks.Put(Exp);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int r) || !ks.Get(out long uid) || !ks.GetWideString(out var nick) ||
            !ks.Get(out sbyte uc) || !ks.Get(out byte lvl) || !ks.Get(out int exp))
            return false;
        Rank = r; UnitUid = uid; NickName = nick; UnitClass = uc; Level = lvl; Exp = exp;
        return true;
    }
}

public sealed class KPvpRankingInfo : IKSerializable
{
    public int Rank { get; set; }
    public long UnitUid { get; set; }
    public string NickName { get; set; } = string.Empty;
    public sbyte UnitClass { get; set; }
    public byte Level { get; set; }
    public sbyte PvpRank { get; set; } // SERV_PVP_NEW_SYSTEM + SERV_2012_PVP_SEASON2
    public int RPoint { get; set; }
    public int Win { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Rank) && ks.Put(UnitUid) && ks.PutWideString(NickName) && ks.Put(UnitClass) &&
        ks.Put(Level) && ks.Put(PvpRank) && ks.Put(RPoint) && ks.Put(Win);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int r) || !ks.Get(out long uid) || !ks.GetWideString(out var nick) ||
            !ks.Get(out sbyte uc) || !ks.Get(out byte lvl) || !ks.Get(out sbyte pr) ||
            !ks.Get(out int rp) || !ks.Get(out int win))
            return false;
        Rank = r; UnitUid = uid; NickName = nick; UnitClass = uc; Level = lvl; PvpRank = pr; RPoint = rp; Win = win;
        return true;
    }
}

public sealed class KEventCharacterRanking : IKSerializable
{
    public int DungeonCharacter1 { get; set; }
    public int DungeonCharacter2 { get; set; }
    public int DungeonCharacter3 { get; set; }
    public int DungeonCharacter4 { get; set; }
    public int PvpCharacter1 { get; set; }
    public int PvpCharacter2 { get; set; }
    public int PvpCharacter3 { get; set; }
    public int PvpCharacter4 { get; set; }
    public int FieldCharacter1 { get; set; }
    public int FieldCharacter2 { get; set; }
    public int FieldCharacter3 { get; set; }
    public int FieldCharacter4 { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(DungeonCharacter1) && ks.Put(DungeonCharacter2) && ks.Put(DungeonCharacter3) && ks.Put(DungeonCharacter4) &&
        ks.Put(PvpCharacter1) && ks.Put(PvpCharacter2) && ks.Put(PvpCharacter3) && ks.Put(PvpCharacter4) &&
        ks.Put(FieldCharacter1) && ks.Put(FieldCharacter2) && ks.Put(FieldCharacter3) && ks.Put(FieldCharacter4);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int d1) || !ks.Get(out int d2) || !ks.Get(out int d3) || !ks.Get(out int d4) ||
            !ks.Get(out int p1) || !ks.Get(out int p2) || !ks.Get(out int p3) || !ks.Get(out int p4) ||
            !ks.Get(out int f1) || !ks.Get(out int f2) || !ks.Get(out int f3) || !ks.Get(out int f4))
            return false;
        DungeonCharacter1 = d1; DungeonCharacter2 = d2; DungeonCharacter3 = d3; DungeonCharacter4 = d4;
        PvpCharacter1 = p1; PvpCharacter2 = p2; PvpCharacter3 = p3; PvpCharacter4 = p4;
        FieldCharacter1 = f1; FieldCharacter2 = f2; FieldCharacter3 = f3; FieldCharacter4 = f4;
        return true;
    }
}



