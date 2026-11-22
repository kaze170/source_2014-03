using KncWX2Server.Core.Region;
using KncWX2Server.Core.Serialization;

namespace KncWX2Server.Core.Packets;

// ---------------- Connection & Account ----------------

public sealed class KEGS_CONNECT_REQ : IKSerializable
{
    public string Version { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region) => ks.PutWideString(Version);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string v)) return false;
        Version = v;
        return true;
    }
}

public sealed class KEGS_CONNECT_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int UdpPort { get; set; }
    public int ChannelId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(UdpPort) && ks.Put(ChannelId);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out int port) || !ks.Get(out int cid)) return false;
        Ok = ok; UdpPort = port; ChannelId = cid;
        return true;
    }
}

public sealed class KEGS_NEW_USER_JOIN_REQ : IKSerializable
{
    public string Id { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public byte GuestUser { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.PutWideString(Id) && ks.PutWideString(Password) && ks.PutWideString(Name) && ks.Put(GuestUser);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string id) || !ks.GetWideString(out string pwd) ||
            !ks.GetWideString(out string name) || !ks.Get(out byte g)) return false;
        Id = id; Password = pwd; Name = name; GuestUser = g;
        return true;
    }
}

public sealed class KEGS_NEW_USER_JOIN_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long UserUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.Put(UserUid);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out long uid)) return false;
        Ok = ok; UserUid = uid;
        return true;
    }
}

public sealed class KEGS_VERIFY_ACCOUNT_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KAccountInfo AccountInfo { get; set; } = new();
    public sbyte HackingUserType { get; set; }
    public string AdUrl { get; set; } = string.Empty;
    public bool Sex { get; set; } = true;
    public uint Age { get; set; } = 20;
    public uint NexonSn { get; set; }
    public string ClientIp { get; set; } = "127.0.0.1";
    public byte ChannelCode { get; set; }
    public string ChannelUserId { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(AccountInfo) && ks.Put(HackingUserType) && ks.PutWideString(AdUrl) &&
        ks.Put(Sex) && ks.Put(Age) && ks.Put(NexonSn) && ks.PutWideString(ClientIp) &&
        ks.Put(ChannelCode) && ks.PutWideString(ChannelUserId);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(AccountInfo) || !ks.Get(out sbyte hut) ||
            !ks.GetWideString(out string url) || !ks.Get(out bool sex) || !ks.Get(out uint age) ||
            !ks.Get(out uint sn) || !ks.GetWideString(out string cip) || !ks.Get(out byte cc) ||
            !ks.GetWideString(out string cuid))
            return false;
        Ok = ok; HackingUserType = hut; AdUrl = url; Sex = sex; Age = age; NexonSn = sn;
        ClientIp = cip; ChannelCode = cc; ChannelUserId = cuid;
        return true;
    }
}

// ---------------- Unit Selection & Management ----------------

public sealed class KEGS_MY_UNIT_AND_INVENTORY_INFO_LIST_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int UnitSlot { get; set; } = 8;
    public List<KUnitInfo> Units { get; set; } = new();
    public bool SharingBank { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(UnitSlot) && ks.PutList(Units) &&
        (!region.SharingBankQuestCash || ks.Put(SharingBank));

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out int slot) || !ks.GetList(Units))
            return false;
        bool sb = false;
        if (region.SharingBankQuestCash && !ks.Get(out sb))
            return false;
        Ok = ok; UnitSlot = slot; SharingBank = sb;
        return true;
    }
}

public sealed class KEGS_CREATE_UNIT_REQ : IKSerializable
{
    public string NickName { get; set; } = string.Empty;
    public int UnitClass { get; set; } = 1;

    public bool Put(KSerializer ks, RegionProfile region) => ks.PutWideString(NickName) && ks.Put(UnitClass);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string nick) || !ks.Get(out int uc)) return false;
        NickName = nick; UnitClass = uc;
        return true;
    }
}

public sealed class KEGS_CREATE_UNIT_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KUnitInfo UnitInfo { get; set; } = new();
    public List<int> SkillSlot { get; set; } = new();
    public string EnableDate { get; set; } = string.Empty;
    public bool ReqGiantDeleteName { get; set; }
    public string GiantNickName { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok) || !ks.Put(UnitInfo) ||
            !ks.PutList(SkillSlot, static (s, v) => s.Put(v)) ||
            !ks.PutWideString(EnableDate))
            return false;
        if (region.GuaranteeUniquenessOfNameCn)
            return ks.Put(ReqGiantDeleteName) && ks.PutWideString(GiantNickName);
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(UnitInfo) ||
            !ks.GetList(SkillSlot, static s => (s.Get(out int v), v)) ||
            !ks.GetWideString(out string ed))
            return false;
        if (region.GuaranteeUniquenessOfNameCn)
        {
            if (!ks.Get(out bool req) || !ks.GetWideString(out string gnick)) return false;
            ReqGiantDeleteName = req; GiantNickName = gnick;
        }
        Ok = ok; EnableDate = ed;
        return true;
    }
}

public sealed class KEGS_DELETE_UNIT_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public long UserUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(UnitUid) && ks.Put(UserUid);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long unitUid) || !ks.Get(out long userUid)) return false;
        UnitUid = unitUid; UserUid = userUid;
        return true;
    }
}

public sealed class KEGS_DELETE_UNIT_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long UnitUid { get; set; }
    public uint KnmSerialNum { get; set; }
    public int GuildUid { get; set; }
    public string NickName { get; set; } = string.Empty;
    public byte Level { get; set; }
    public sbyte UnitClass { get; set; }
    public long ReDelAbleDate { get; set; }
    public long DelAbleDate { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok) || !ks.Put(UnitUid) || !ks.Put(KnmSerialNum) || !ks.Put(GuildUid) ||
            !ks.PutWideString(NickName) || !ks.Put(Level) || !ks.Put(UnitClass))
            return false;
        if (region.UnitWaitDelete)
            return ks.Put(ReDelAbleDate) && ks.Put(DelAbleDate);
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out long uid) || !ks.Get(out uint knm) ||
            !ks.Get(out int guid) || !ks.GetWideString(out string nick) ||
            !ks.Get(out byte lvl) || !ks.Get(out sbyte uc))
            return false;
        if (region.UnitWaitDelete)
        {
            if (!ks.Get(out long rd) || !ks.Get(out long d)) return false;
            ReDelAbleDate = rd; DelAbleDate = d;
        }
        Ok = ok; UnitUid = uid; KnmSerialNum = knm; GuildUid = guid; NickName = nick; Level = lvl; UnitClass = uc;
        return true;
    }
}

public sealed class KEGS_SELECT_UNIT_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public List<int> CodeEventScriptIds { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(UnitUid) && ks.PutList(CodeEventScriptIds, static (s, v) => s.Put(v));

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.GetList(CodeEventScriptIds, static s => (s.Get(out int v), v)))
            return false;
        UnitUid = uid;
        return true;
    }
}

public sealed class KEGS_SELECT_UNIT_ACK : IKSerializable
{
    public int Ok { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok)) return false;
        Ok = ok;
        return true;
    }
}

public sealed class KEGS_SELECT_UNIT_INVENTORY_INFO_NOT : IKSerializable
{
    public int SlotCategory { get; set; }
    public Dictionary<long, KInventoryItemInfo> Items { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(SlotCategory) &&
        ks.PutMap(Items, static (s, k) => s.Put(k), static (s, v) => s.Put(v));

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int cat) ||
            !ks.GetMap(Items, static s => (s.Get(out long k), k), static s => { var v = new KInventoryItemInfo(); return (s.Get(v), v); }))
            return false;
        SlotCategory = cat;
        return true;
    }
}

public sealed class KEGS_GET_MY_INVENTORY_ACK : IKSerializable
{
    public int Ok { get; set; }
    public bool IsRecommend { get; set; }
    public bool SharingBank { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(IsRecommend) && (!region.SharingBankQuestCash || ks.Put(SharingBank));

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out bool rec)) return false;
        bool sb = false;
        if (region.SharingBankQuestCash && !ks.Get(out sb)) return false;
        Ok = ok; IsRecommend = rec; SharingBank = sb;
        return true;
    }
}

// ---------------- Field / Village & Chat ----------------

public sealed class KEGS_STATE_CHANGE_FIELD_REQ : IKSerializable
{
    public int MapId { get; set; } = 20001;
    public bool UseWarpItem { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(MapId) && ks.Put(UseWarpItem);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int map) || !ks.Get(out bool warp)) return false;
        MapId = map; UseWarpItem = warp;
        return true;
    }
}

public sealed class KEGS_STATE_CHANGE_FIELD_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int LimitLevel { get; set; }
    public int RequireDungeonId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(LimitLevel) && ks.Put(RequireDungeonId);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out int ll) || !ks.Get(out int rdid)) return false;
        Ok = ok; LimitLevel = ll; RequireDungeonId = rdid;
        return true;
    }
}

public sealed class KEGS_CHANGE_USER_LIST_IN_SECTOR_NOT : IKSerializable
{
    public List<KFieldUserInfo> NewUsers { get; set; } = new();
    public List<long> DelUsers { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.PutList(NewUsers) && ks.PutList(DelUsers, static (s, v) => s.Put(v));

    public bool Get(KSerializer ks, RegionProfile region) =>
        ks.GetList(NewUsers) && ks.GetList(DelUsers, static s => (s.Get(out long v), v));
}

public sealed class KEGS_FIELD_UNIT_SYNC_DATA_NOT : IKSerializable
{
    public KVector3 Pos { get; set; } = new();
    public List<KFieldFrameInfo> FieldFrames { get; set; } = new();
    public bool IsRight { get; set; } = true;
    public byte LastTouchLineIndex { get; set; }
    public ushort LastPosValue { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Pos) && ks.PutList(FieldFrames) && ks.Put(IsRight) &&
        ks.Put(LastTouchLineIndex) && ks.Put(LastPosValue);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(Pos) || !ks.GetList(FieldFrames) || !ks.Get(out bool ir) ||
            !ks.Get(out byte lt) || !ks.Get(out ushort lp))
            return false;
        IsRight = ir; LastTouchLineIndex = lt; LastPosValue = lp;
        return true;
    }
}

public sealed class KEGS_FIELD_UNIT_SYNC_DATA_BROAD_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public KVector3 Pos { get; set; } = new();
    public List<KFieldFrameInfo> FieldFrames { get; set; } = new();
    public bool IsRight { get; set; } = true;

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(UnitUid) && ks.Put(Pos) && ks.PutList(FieldFrames) && ks.Put(IsRight);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.Get(Pos) || !ks.GetList(FieldFrames) || !ks.Get(out bool ir))
            return false;
        UnitUid = uid; IsRight = ir;
        return true;
    }
}

public sealed class KEGS_CHAT_REQ : IKSerializable
{
    public sbyte ChatPacketType { get; set; }
    public sbyte RoomChatType { get; set; }
    public long ToUnitUid { get; set; }
    public string ReceiverNickName { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(ChatPacketType) && ks.Put(RoomChatType) && ks.Put(ToUnitUid) &&
        ks.PutWideString(ReceiverNickName) && ks.PutWideString(Message);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte cpt) || !ks.Get(out sbyte rct) || !ks.Get(out long tuid) ||
            !ks.GetWideString(out string rnick) || !ks.GetWideString(out string msg))
            return false;
        ChatPacketType = cpt; RoomChatType = rct; ToUnitUid = tuid;
        ReceiverNickName = rnick; Message = msg;
        return true;
    }
}

public sealed class KEGS_CHAT_NOT : IKSerializable
{
    public sbyte ChatPacketType { get; set; }
    public long SenderUnitUid { get; set; }
    public string SenderNickName { get; set; } = string.Empty;
    public string ReceiverNickName { get; set; } = string.Empty;
    public long ReceiverUnitUid { get; set; }
    public sbyte RoomChatType { get; set; }
    public string Message { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(ChatPacketType) && ks.Put(SenderUnitUid) && ks.PutWideString(SenderNickName) &&
        ks.PutWideString(ReceiverNickName) && ks.Put(ReceiverUnitUid) &&
        ks.Put(RoomChatType) && ks.PutWideString(Message);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte cpt) || !ks.Get(out long suid) || !ks.GetWideString(out string snick) ||
            !ks.GetWideString(out string rnick) || !ks.Get(out long ruid) ||
            !ks.Get(out sbyte rct) || !ks.GetWideString(out string msg))
            return false;
        ChatPacketType = cpt; SenderUnitUid = suid; SenderNickName = snick;
        ReceiverNickName = rnick; ReceiverUnitUid = ruid; RoomChatType = rct; Message = msg;
        return true;
    }
}

// ---------------- Room / Dungeon / PvP ----------------

public sealed class KEGS_ROOM_LIST_REQ : IKSerializable
{
    public int ViewPage { get; set; } = 1;
    public sbyte PvpGameType { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(ViewPage) && ks.Put(PvpGameType);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int vp) || !ks.Get(out sbyte pgt)) return false;
        ViewPage = vp; PvpGameType = pgt;
        return true;
    }
}

public sealed class KEGS_ROOM_LIST_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int TotalPage { get; set; } = 1;
    public int ViewPage { get; set; } = 1;
    public List<KRoomInfo> Rooms { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(TotalPage) && ks.Put(ViewPage) && ks.PutList(Rooms);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out int tp) || !ks.Get(out int vp) || !ks.GetList(Rooms))
            return false;
        Ok = ok; TotalPage = tp; ViewPage = vp;
        return true;
    }
}

public sealed class KEGS_CREATE_ROOM_REQ : IKSerializable
{
    public bool Observer { get; set; }
    public KRoomInfo RoomInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Observer) && ks.Put(RoomInfo);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool obs) || !ks.Get(RoomInfo)) return false;
        Observer = obs;
        return true;
    }
}

public sealed class KEGS_CREATE_ROOM_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KRoomInfo RoomInfo { get; set; } = new();
    public List<KRoomSlotInfo> Slots { get; set; } = new();
    public List<KRoomSlotInfo> ObserverSlots { get; set; } = new();
    public string CnIp { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(RoomInfo) && ks.PutList(Slots) && ks.PutList(ObserverSlots) && ks.PutWideString(CnIp);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(RoomInfo) || !ks.GetList(Slots) ||
            !ks.GetList(ObserverSlots) || !ks.GetWideString(out string cnip))
            return false;
        Ok = ok; CnIp = cnip;
        return true;
    }
}

public sealed class KEGS_JOIN_ROOM_REQ : IKSerializable
{
    public long RoomUid { get; set; }
    public string Password { get; set; } = string.Empty;
    public bool IsObserver { get; set; }
    public bool Invitation { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(RoomUid) && ks.PutWideString(Password) && ks.Put(IsObserver) && ks.Put(Invitation);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long ruid) || !ks.GetWideString(out string pwd) ||
            !ks.Get(out bool obs) || !ks.Get(out bool inv))
            return false;
        RoomUid = ruid; Password = pwd; IsObserver = obs; Invitation = inv;
        return true;
    }
}

public sealed class KEGS_LEAVE_ROOM_REQ : IKSerializable
{
    public int Reason { get; set; }
    public KDungeonPlayResultInfo ResultInfo { get; set; } = new();
    public int BattleFieldId { get; set; }
    public KGamePlayNetworkInfo GamePlayNetworkInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Reason) && ks.Put(ResultInfo) && ks.Put(BattleFieldId) && ks.Put(GamePlayNetworkInfo);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int r) || !ks.Get(ResultInfo) || !ks.Get(out int bfid) || !ks.Get(GamePlayNetworkInfo))
            return false;
        Reason = r;
        BattleFieldId = bfid;
        return true;
    }
}

public sealed class KEGS_CHANGE_READY_REQ : IKSerializable
{
    public bool Ready { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ready);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool r)) return false;
        Ready = r;
        return true;
    }
}

public sealed class KEGS_CHANGE_READY_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public bool Ready { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(UnitUid) && ks.Put(Ready);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.Get(out bool r)) return false;
        UnitUid = uid; Ready = r;
        return true;
    }
}

public sealed class KEGS_CHANGE_TEAM_REQ : IKSerializable
{
    public int TeamNum { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(TeamNum);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int tn)) return false;
        TeamNum = tn;
        return true;
    }
}

public sealed class KEGS_CHANGE_TEAM_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public int TeamNum { get; set; }
    public sbyte SwapSlotIndex { get; set; } = -1;

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(UnitUid) && ks.Put(TeamNum) && ks.Put(SwapSlotIndex);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.Get(out int tn) || !ks.Get(out sbyte idx)) return false;
        UnitUid = uid; TeamNum = tn; SwapSlotIndex = idx;
        return true;
    }
}

public sealed class KEGS_STATE_CHANGE_GAME_START_REQ : IKSerializable
{
    public short WorldId { get; set; }
    public int DungeonId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(WorldId) && ks.Put(DungeonId);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out short w) || !ks.Get(out int d)) return false;
        WorldId = w; DungeonId = d;
        return true;
    }
}

public sealed class KEGS_STATE_CHANGE_GAME_START_NOT : IKSerializable
{
    public sbyte RoomState { get; set; }
    public int GameType { get; set; }
    public int Spirit { get; set; } = 100;
    public short WorldId { get; set; }
    public List<KRoomSlotInfo> Slots { get; set; } = new();
    public List<KInventoryItemInfo> InventorySlotInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(RoomState) && ks.Put(GameType) && ks.Put(Spirit) && ks.Put(WorldId) &&
        ks.PutList(Slots) && ks.PutList(InventorySlotInfo);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte rs) || !ks.Get(out int gt) || !ks.Get(out int sp) || !ks.Get(out short wid) ||
            !ks.GetList(Slots) || !ks.GetList(InventorySlotInfo))
            return false;
        RoomState = rs; GameType = gt; Spirit = sp; WorldId = wid;
        return true;
    }
}

public sealed class KEGS_GAME_LOADING_REQ : IKSerializable
{
    public int LoadingProgress { get; set; }
    public int UnitLevel { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(LoadingProgress) && ks.Put(UnitLevel);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int lp) || !ks.Get(out int ul)) return false;
        LoadingProgress = lp; UnitLevel = ul;
        return true;
    }
}

public sealed class KEGS_GAME_LOADING_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public int LoadingProgress { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(UnitUid) && ks.Put(LoadingProgress);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.Get(out int lp)) return false;
        UnitUid = uid; LoadingProgress = lp;
        return true;
    }
}

public sealed class KEGS_PLAY_START_NOT : IKSerializable
{
    public sbyte RoomState { get; set; }
    public List<KRoomSlotInfo> Slots { get; set; } = new();
    public int DungeonId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(RoomState) && ks.PutList(Slots) && ks.Put(DungeonId);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte rs) || !ks.GetList(Slots) || !ks.Get(out int did)) return false;
        RoomState = rs; DungeonId = did;
        return true;
    }
}

public sealed class KEGS_END_GAME_REQ : IKSerializable
{
    public int WinTeam { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(WinTeam);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int wt)) return false;
        WinTeam = wt;
        return true;
    }
}

public sealed class KEGS_END_GAME_NOT : IKSerializable
{
    public sbyte RoomState { get; set; }
    public int WinTeam { get; set; }
    public List<KRoomSlotInfo> Slots { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(RoomState) && ks.Put(WinTeam) && ks.PutList(Slots);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte rs) || !ks.Get(out int wt) || !ks.GetList(Slots)) return false;
        RoomState = rs; WinTeam = wt;
        return true;
    }
}

// ---------------- Party ----------------

public sealed class KEGS_INVITE_PARTY_REQ : IKSerializable
{
    public long ReceiverUid { get; set; }
    public string NickName { get; set; } = string.Empty;
    public int ServerGroupId { get; set; }
    public KCreatePartyInfo CreatePartyInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(ReceiverUid) && ks.PutWideString(NickName) && ks.Put(ServerGroupId) && ks.Put(CreatePartyInfo);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long ruid) || !ks.GetWideString(out string nick) ||
            !ks.Get(out int sgid) || !ks.Get(CreatePartyInfo))
            return false;
        ReceiverUid = ruid; NickName = nick; ServerGroupId = sgid;
        return true;
    }
}

public sealed class KEGS_INVITE_PARTY_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long ReceiverUid { get; set; }
    public string UdpRelayIp { get; set; } = "127.0.0.1";
    public ushort UdpRelayPort { get; set; } = 9303;

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(ReceiverUid) && ks.PutWideString(UdpRelayIp) && ks.Put(UdpRelayPort);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out long ruid) ||
            !ks.GetWideString(out string ip) || !ks.Get(out ushort port))
            return false;
        Ok = ok; ReceiverUid = ruid; UdpRelayIp = ip; UdpRelayPort = port;
        return true;
    }
}

public sealed class KEGS_INVITE_PARTY_NOT : IKSerializable
{
    public long PartyUid { get; set; }
    public long HostUnitUid { get; set; }
    public string HostNickName { get; set; } = string.Empty;
    public sbyte PartyType { get; set; }
    public string UdpRelayIp { get; set; } = "127.0.0.1";
    public ushort UdpRelayPort { get; set; } = 9303;

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(PartyUid) && ks.Put(HostUnitUid) && ks.PutWideString(HostNickName) &&
        ks.Put(PartyType) && ks.PutWideString(UdpRelayIp) && ks.Put(UdpRelayPort);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long puid) || !ks.Get(out long huid) || !ks.GetWideString(out string hnick) ||
            !ks.Get(out sbyte pt) || !ks.GetWideString(out string ip) || !ks.Get(out ushort port))
            return false;
        PartyUid = puid; HostUnitUid = huid; HostNickName = hnick; PartyType = pt; UdpRelayIp = ip; UdpRelayPort = port;
        return true;
    }
}

public sealed class KEGS_INVITE_PARTY_REPLY_NOT : IKSerializable
{
    public long PartyUid { get; set; }
    public long HostUnitUid { get; set; }
    public sbyte ReplyReason { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(PartyUid) && ks.Put(HostUnitUid) && ks.Put(ReplyReason);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long puid) || !ks.Get(out long huid) || !ks.Get(out sbyte rr)) return false;
        PartyUid = puid; HostUnitUid = huid; ReplyReason = rr;
        return true;
    }
}

public sealed class KEGS_JOIN_PARTY_NOT : IKSerializable
{
    public int Ok { get; set; }
    public List<KPartyInfo> PartyInfos { get; set; } = new();
    public List<KPartyUserInfo> PartyUsers { get; set; } = new();
    public List<long> CurPartyUnitUidList { get; set; } = new();
    public string UdpRelayIp { get; set; } = "127.0.0.1";
    public ushort UdpRelayPort { get; set; } = 9303;

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.PutList(PartyInfos) && ks.PutList(PartyUsers) &&
        ks.PutList(CurPartyUnitUidList, static (s, v) => s.Put(v)) &&
        ks.PutWideString(UdpRelayIp) && ks.Put(UdpRelayPort);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.GetList(PartyInfos) || !ks.GetList(PartyUsers) ||
            !ks.GetList(CurPartyUnitUidList, static s => (s.Get(out long v), v)) ||
            !ks.GetWideString(out string ip) || !ks.Get(out ushort port))
            return false;
        Ok = ok; UdpRelayIp = ip; UdpRelayPort = port;
        return true;
    }
}

public sealed class KEGS_LEAVE_PARTY_REQ : IKSerializable
{
    public int Reason { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Reason);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int r)) return false;
        Reason = r;
        return true;
    }
}

public sealed class KEGS_LEAVE_PARTY_NOT : IKSerializable
{
    public long LeaveUnitUid { get; set; }
    public long NewHostUid { get; set; }
    public long PartyUid { get; set; }
    public List<long> PartyUnitUidList { get; set; } = new();
    public bool ExistComeBackUser { get; set; }
    public int Reason { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(LeaveUnitUid) && ks.Put(NewHostUid) && ks.Put(PartyUid) &&
        ks.PutList(PartyUnitUidList, static (s, v) => s.Put(v)) &&
        ks.Put(ExistComeBackUser) && ks.Put(Reason);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long luid) || !ks.Get(out long nhuid) || !ks.Get(out long puid) ||
            !ks.GetList(PartyUnitUidList, static s => (s.Get(out long v), v)) ||
            !ks.Get(out bool ecb) || !ks.Get(out int r))
            return false;
        LeaveUnitUid = luid; NewHostUid = nhuid; PartyUid = puid; ExistComeBackUser = ecb; Reason = r;
        return true;
    }
}

// ---------------- Inventory / Shop / Mail / Guild / Quest ----------------

public sealed class KEGS_CHANGE_INVENTORY_SLOT_ITEM_REQ : IKSerializable
{
    public sbyte FromSlotType { get; set; }
    public int FromSlotId { get; set; }
    public sbyte ToSlotType { get; set; }
    public int ToSlotId { get; set; }
    public long ShareUnitUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(FromSlotType) && ks.Put(FromSlotId) && ks.Put(ToSlotType) && ks.Put(ToSlotId) && ks.Put(ShareUnitUid);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte fst) || !ks.Get(out int fsi) || !ks.Get(out sbyte tst) ||
            !ks.Get(out int tsi) || !ks.Get(out long suid))
            return false;
        FromSlotType = fst; FromSlotId = fsi; ToSlotType = tst; ToSlotId = tsi; ShareUnitUid = suid;
        return true;
    }
}

public sealed class KEGS_CHANGE_INVENTORY_SLOT_ITEM_ACK : IKSerializable
{
    public List<KInventoryItemInfo> InventorySlotInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) => ks.PutList(InventorySlotInfo);
    public bool Get(KSerializer ks, RegionProfile region) => ks.GetList(InventorySlotInfo);
}

public sealed class KEGS_BUY_ED_ITEM_REQ : IKSerializable
{
    public bool Debug { get; set; }
    public short Period { get; set; }
    public Dictionary<int, int> Items { get; set; } = new(); // ItemID -> Quantity

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Debug) && ks.Put(Period) && ks.PutMap(Items, static (s, k) => s.Put(k), static (s, v) => s.Put(v));

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool dbg) || !ks.Get(out short per) ||
            !ks.GetMap(Items, static s => (s.Get(out int k), k), static s => (s.Get(out int v), v)))
            return false;
        Debug = dbg; Period = per;
        return true;
    }
}

public sealed class KEGS_BUY_ED_ITEM_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int Ed { get; set; }
    public int APoint { get; set; }
    public List<KInventoryItemInfo> InventorySlotInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(Ed) && ks.Put(APoint) && ks.PutList(InventorySlotInfo);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out int ed) || !ks.Get(out int ap) || !ks.GetList(InventorySlotInfo))
            return false;
        Ok = ok; Ed = ed; APoint = ap;
        return true;
    }
}

public sealed class KEGS_SELL_ED_ITEM_REQ : IKSerializable
{
    public long ItemUid { get; set; }
    public int Quantity { get; set; } = 1;

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(ItemUid) && ks.Put(Quantity);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.Get(out int qty)) return false;
        ItemUid = uid; Quantity = qty;
        return true;
    }
}

public sealed class KEGS_SELL_ED_ITEM_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int Ed { get; set; }
    public KInventoryItemInfo InventorySlotInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(Ed) && ks.Put(InventorySlotInfo);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out int ed) || !ks.Get(InventorySlotInfo)) return false;
        Ok = ok; Ed = ed;
        return true;
    }
}

public sealed class KEGS_GET_POST_LETTER_LIST_REQ : IKSerializable
{
    public sbyte LetterListType { get; set; }
    public int PostPageNo { get; set; } = 1;

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(LetterListType) && ks.Put(PostPageNo);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte t) || !ks.Get(out int p)) return false;
        LetterListType = t; PostPageNo = p;
        return true;
    }
}

public sealed class KEGS_GET_POST_LETTER_LIST_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int TotalLetterCount { get; set; }
    public List<KPostItemTitleInfo> PostItems { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(TotalLetterCount) && ks.PutList(PostItems);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out int total) || !ks.GetList(PostItems)) return false;
        Ok = ok; TotalLetterCount = total;
        return true;
    }
}

public sealed class KEGS_SEND_LETTER_REQ : IKSerializable
{
    public bool IsGmMessage { get; set; }
    public string ToNickName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public int Ed { get; set; }
    public long ItemUid { get; set; }
    public int Quantity { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(IsGmMessage) && ks.PutWideString(ToNickName) && ks.PutWideString(Title) &&
        ks.PutWideString(Message) && ks.Put(Ed) && ks.Put(ItemUid) && ks.Put(Quantity);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool gm) || !ks.GetWideString(out string to) || !ks.GetWideString(out string title) ||
            !ks.GetWideString(out string msg) || !ks.Get(out int ed) || !ks.Get(out long iuid) ||
            !ks.Get(out int qty))
            return false;
        IsGmMessage = gm; ToNickName = to; Title = title; Message = msg; Ed = ed; ItemUid = iuid; Quantity = qty;
        return true;
    }
}

public sealed class KEGS_SEND_LETTER_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int Ed { get; set; }
    public List<KInventoryItemInfo> InventorySlotInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(Ed) && ks.PutList(InventorySlotInfo);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out int ed) || !ks.GetList(InventorySlotInfo)) return false;
        Ok = ok; Ed = ed;
        return true;
    }
}

public sealed class KEGS_READ_LETTER_REQ : IKSerializable
{
    public long PostNo { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(PostNo);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long pno)) return false;
        PostNo = pno;
        return true;
    }
}

public sealed class KEGS_READ_LETTER_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KPostItemInfo Letter { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.Put(Letter);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(Letter)) return false;
        Ok = ok;
        return true;
    }
}

public sealed class KEGS_CREATE_GUILD_REQ : IKSerializable
{
    public long ItemUid { get; set; }
    public string GuildName { get; set; } = string.Empty;
    public string GuildMessage { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(ItemUid) && ks.PutWideString(GuildName) && ks.PutWideString(GuildMessage);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.GetWideString(out string name) || !ks.GetWideString(out string msg))
            return false;
        ItemUid = uid; GuildName = name; GuildMessage = msg;
        return true;
    }
}

public sealed class KEGS_CREATE_GUILD_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KGuildInfo GuildInfo { get; set; } = new();
    public List<KInventoryItemInfo> InventorySlotInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(GuildInfo) && ks.PutList(InventorySlotInfo);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(GuildInfo) || !ks.GetList(InventorySlotInfo)) return false;
        Ok = ok;
        return true;
    }
}

public sealed class KEGS_NEW_QUEST_REQ : IKSerializable
{
    public int QuestId { get; set; }
    public int TalkNpcId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(QuestId) && ks.Put(TalkNpcId);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int qid) || !ks.Get(out int npc)) return false;
        QuestId = qid; TalkNpcId = npc;
        return true;
    }
}

public sealed class KEGS_NEW_QUEST_NOT : IKSerializable
{
    public KQuestInstance QuestInst { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(QuestInst) && ks.PutList(UpdatedInventorySlot);

    public bool Get(KSerializer ks, RegionProfile region) =>
        ks.Get(QuestInst) && ks.GetList(UpdatedInventorySlot);
}

public sealed class KEGS_QUEST_COMPLETE_REQ : IKSerializable
{
    public int QuestId { get; set; }
    public int TalkNpcId { get; set; }
    public Dictionary<int, int> SelectItemId { get; set; } = new();
    public bool AutoComplete { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(QuestId) && ks.Put(TalkNpcId) &&
        ks.PutMap(SelectItemId, static (s, k) => s.Put(k), static (s, v) => s.Put(v)) &&
        ks.Put(AutoComplete);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int qid) || !ks.Get(out int npc) ||
            !ks.GetMap(SelectItemId, static s => (s.Get(out int k), k), static s => (s.Get(out int v), v)) ||
            !ks.Get(out bool ac))
            return false;
        QuestId = qid; TalkNpcId = npc; AutoComplete = ac;
        return true;
    }
}

public sealed class KEGS_UPDATE_UNIT_INFO_NOT : IKSerializable
{
    public KUnitInfo UnitInfo { get; set; } = new();
    public List<KInventoryItemInfo> InventorySlotInfo { get; set; } = new();
    public Dictionary<int, int> ItemObtained { get; set; } = new();
    public bool GameEnd { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(UnitInfo) && ks.PutList(InventorySlotInfo) &&
        ks.PutMap(ItemObtained, static (s, k) => s.Put(k), static (s, v) => s.Put(v)) &&
        ks.Put(GameEnd);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(UnitInfo) || !ks.GetList(InventorySlotInfo) ||
            !ks.GetMap(ItemObtained, static s => (s.Get(out int k), k), static s => (s.Get(out int v), v)) ||
            !ks.Get(out bool ge))
            return false;
        GameEnd = ge;
        return true;
    }
}

public sealed class KEGS_QUEST_COMPLETE_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KCompleteQuestInfo CompleteQuestInfo { get; set; } = new();
    public KEGS_UPDATE_UNIT_INFO_NOT UpdateUnitInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(CompleteQuestInfo) && ks.Put(UpdateUnitInfo);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(CompleteQuestInfo) || !ks.Get(UpdateUnitInfo))
            return false;
        Ok = ok;
        return true;
    }
}

// ---------------- TCP Relay ----------------

public sealed class KETR_REG_UID : IKSerializable
{
    public long UnitUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(UnitUid);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid)) return false;
        UnitUid = uid;
        return true;
    }
}

public sealed class KEGS_CHANGE_EQUIPPED_ITEM_IN_FIELD_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public List<KInventoryItemInfo> InventorySlotInfo { get; set; } = new();
    public int EquippedTitleId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(UnitUid) && ks.PutList(InventorySlotInfo) && ks.Put(EquippedTitleId);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.GetList(InventorySlotInfo) || !ks.Get(out int titleId))
            return false;
        UnitUid = uid; EquippedTitleId = titleId;
        return true;
    }
}

public sealed class KEGS_SUMMON_PET_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public List<KPetInfo> PetInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(UnitUid) && ks.PutList(PetInfo);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.GetList(PetInfo)) return false;
        UnitUid = uid;
        return true;
    }
}

public sealed class KEGS_PET_ACTION_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public sbyte ActionType { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(UnitUid) && ks.Put(ActionType);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.Get(out sbyte act)) return false;
        UnitUid = uid; ActionType = act;
        return true;
    }
}

public sealed class KEGS_SUMMON_RIDING_PET_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public long RidingPetUid { get; set; }
    public ushort RidingPetId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(UnitUid) && ks.Put(RidingPetUid) && ks.Put(RidingPetId);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.Get(out long rpUid) || !ks.Get(out ushort rpId))
            return false;
        UnitUid = uid; RidingPetUid = rpUid; RidingPetId = rpId;
        return true;
    }
}

public sealed class KEGS_UNSUMMON_RIDING_PET_NOT : IKSerializable
{
    public long UnitUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(UnitUid);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid)) return false;
        UnitUid = uid;
        return true;
    }
}

public sealed class KEGS_CHANGE_USER_INFO_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public sbyte UnitClass { get; set; }
    public string GuildName { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(UnitUid) && ks.Put(UnitClass) && ks.PutWideString(GuildName);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.Get(out sbyte cls) || !ks.GetWideString(out string gname))
            return false;
        UnitUid = uid; UnitClass = cls; GuildName = gname;
        return true;
    }
}

public sealed class KEGS_COUPLE_MAKING_SUCCESS_NOT : IKSerializable
{
    public string RequestUnitName { get; set; } = string.Empty;
    public string AcceptUnitName { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.PutWideString(RequestUnitName) && ks.PutWideString(AcceptUnitName);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string req) || !ks.GetWideString(out string acc)) return false;
        RequestUnitName = req; AcceptUnitName = acc;
        return true;
    }
}

// ---------------- PvP & Dungeon Result Packets ----------------

public sealed class KPVPUnitResultInfo : IKSerializable
{
    public long UnitUid { get; set; }
    public int Ranking { get; set; }
    public int KillNum { get; set; }
    public int MDKillNum { get; set; }
    public int DeathNum { get; set; }
    public int Rating { get; set; }
    public int RPoint { get; set; }
    public int APoint { get; set; }
    public float KFactorIncrement { get; set; }
    public int Exp { get; set; }
    public bool IsMvp { get; set; }
    public bool Win { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(UnitUid) && ks.Put(Ranking) && ks.Put(KillNum) && ks.Put(MDKillNum) && ks.Put(DeathNum) &&
        ks.Put(Rating) && ks.Put(RPoint) && ks.Put(APoint) && ks.Put(KFactorIncrement) &&
        ks.Put(Exp) && ks.Put(IsMvp) && ks.Put(Win);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.Get(out int rank) || !ks.Get(out int kills) ||
            !ks.Get(out int mdKills) || !ks.Get(out int deaths) || !ks.Get(out int rating) ||
            !ks.Get(out int rp) || !ks.Get(out int ap) || !ks.Get(out float kf) ||
            !ks.Get(out int exp) || !ks.Get(out bool mvp) || !ks.Get(out bool win))
            return false;
        UnitUid = uid; Ranking = rank; KillNum = kills; MDKillNum = mdKills; DeathNum = deaths;
        Rating = rating; RPoint = rp; APoint = ap; KFactorIncrement = kf; Exp = exp; IsMvp = mvp; Win = win;
        return true;
    }
}

public sealed class KEGS_END_GAME_PVP_RESULT_DATA_NOT : IKSerializable
{
    public sbyte WinTeam { get; set; }
    public bool IsDrawn { get; set; }
    public bool IsAbnormalEndPlay { get; set; }
    public int PlayTime { get; set; }
    public int GameType { get; set; }
    public int Spirit { get; set; }
    public uint ChannelId { get; set; }
    public List<KPVPUnitResultInfo> PvpUnitInfo { get; set; } = new();
    public bool OfficialMatch { get; set; }
    public bool IsHeroNpc { get; set; }
    public int RematchCount { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(WinTeam) && ks.Put(IsDrawn) && ks.Put(IsAbnormalEndPlay) && ks.Put(PlayTime) &&
        ks.Put(GameType) && ks.Put(Spirit) && ks.Put(ChannelId) && ks.PutList(PvpUnitInfo) &&
        ks.Put(OfficialMatch) && ks.Put(IsHeroNpc) && ks.Put(RematchCount);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte wt) || !ks.Get(out bool drawn) || !ks.Get(out bool abn) ||
            !ks.Get(out int pt) || !ks.Get(out int gt) || !ks.Get(out int sp) ||
            !ks.Get(out uint cid) || !ks.GetList(PvpUnitInfo) || !ks.Get(out bool om) ||
            !ks.Get(out bool hero) || !ks.Get(out int rc))
            return false;
        WinTeam = wt; IsDrawn = drawn; IsAbnormalEndPlay = abn; PlayTime = pt;
        GameType = gt; Spirit = sp; ChannelId = cid; OfficialMatch = om; IsHeroNpc = hero; RematchCount = rc;
        return true;
    }
}

public sealed class KDungeonUnitResultInfo : IKSerializable
{
    public long UnitUid { get; set; }
    public bool IsDie { get; set; }
    public int TotalScore { get; set; }
    public int ComboScore { get; set; }
    public int TechnicalScore { get; set; }
    public int Damaged { get; set; }
    public int OldEd { get; set; }
    public int OldExp { get; set; }
    public int Ed { get; set; }
    public int Exp { get; set; }
    public int ClearBonusExp { get; set; }
    public int RankBonusExp { get; set; }
    public int PartyBonusExp { get; set; }
    public int MvpBonusExp { get; set; }
    public int GameBangBonusExp { get; set; }
    public int PremiumBonusExp { get; set; }
    public int EventExp { get; set; }
    public int ChannelBonusExp { get; set; }
    public int KillNpcNum { get; set; }
    public sbyte ComboRank { get; set; } = 1;
    public sbyte TechnicalRank { get; set; } = 1;
    public sbyte DamagedRank { get; set; } = 1;
    public sbyte TimeRank { get; set; } = 1;
    public sbyte TotalRank { get; set; } = 1;
    public bool IsMvp { get; set; }

    public int SumExp() => Exp + ClearBonusExp + RankBonusExp + PartyBonusExp + MvpBonusExp +
                           GameBangBonusExp + PremiumBonusExp + EventExp + ChannelBonusExp;

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(UnitUid) && ks.Put(IsDie) && ks.Put(TotalScore) && ks.Put(ComboScore) &&
        ks.Put(TechnicalScore) && ks.Put(Damaged) && ks.Put(OldEd) && ks.Put(OldExp) &&
        ks.Put(Ed) && ks.Put(Exp) && ks.Put(ClearBonusExp) && ks.Put(RankBonusExp) &&
        ks.Put(PartyBonusExp) && ks.Put(MvpBonusExp) && ks.Put(GameBangBonusExp) &&
        ks.Put(PremiumBonusExp) && ks.Put(EventExp) && ks.Put(ChannelBonusExp) &&
        ks.Put(KillNpcNum) && ks.Put(ComboRank) && ks.Put(TechnicalRank) &&
        ks.Put(DamagedRank) && ks.Put(TimeRank) && ks.Put(TotalRank) && ks.Put(IsMvp);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.Get(out bool die) || !ks.Get(out int ts) || !ks.Get(out int cs) ||
            !ks.Get(out int techs) || !ks.Get(out int dmg) || !ks.Get(out int oed) || !ks.Get(out int oexp) ||
            !ks.Get(out int ed) || !ks.Get(out int exp) || !ks.Get(out int cbe) || !ks.Get(out int rbe) ||
            !ks.Get(out int pbe) || !ks.Get(out int mbe) || !ks.Get(out int gbe) ||
            !ks.Get(out int prembe) || !ks.Get(out int ee) || !ks.Get(out int chbe) ||
            !ks.Get(out int kills) || !ks.Get(out sbyte cr) || !ks.Get(out sbyte tr) ||
            !ks.Get(out sbyte dr) || !ks.Get(out sbyte timer) || !ks.Get(out sbyte totr) || !ks.Get(out bool mvp))
            return false;
        UnitUid = uid; IsDie = die; TotalScore = ts; ComboScore = cs; TechnicalScore = techs; Damaged = dmg;
        OldEd = oed; OldExp = oexp; Ed = ed; Exp = exp; ClearBonusExp = cbe; RankBonusExp = rbe;
        PartyBonusExp = pbe; MvpBonusExp = mbe; GameBangBonusExp = gbe; PremiumBonusExp = prembe;
        EventExp = ee; ChannelBonusExp = chbe; KillNpcNum = kills; ComboRank = cr; TechnicalRank = tr;
        DamagedRank = dr; TimeRank = timer; TotalRank = totr; IsMvp = mvp;
        return true;
    }
}

public sealed class KEGS_END_GAME_DUNGEON_RESULT_DATA_NOT : IKSerializable
{
    public int Sec { get; set; }
    public bool IsWin { get; set; }
    public int DungeonId { get; set; }
    public sbyte Difficulty { get; set; }
    public sbyte DungeonMode { get; set; }
    public List<KDungeonUnitResultInfo> DungeonUnitInfo { get; set; } = new();
    public Dictionary<long, Dictionary<int, KSubQuestInfo>> OngoingQuest { get; set; } = new();
    public bool IsHenirDungeon { get; set; }
    public int DungeonEventPoint { get; set; }
    public int HenirEventPoint { get; set; }
    public Dictionary<long, bool> HaveExpInDungeon { get; set; } = new();
    public bool IsClearDefenceDungeon { get; set; }
    public int ProtectedSmallCrystal { get; set; }
    public bool ComeBackUserInParty { get; set; }
    public Dictionary<long, bool> SuitableLevelInfo { get; set; } = new();
    public bool SpecChar1 { get; set; }
    public bool SpecChar2 { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Sec) && ks.Put(IsWin) && ks.Put(DungeonId) && ks.Put(Difficulty) && ks.Put(DungeonMode) &&
        ks.PutList(DungeonUnitInfo) &&
        ks.PutMap(OngoingQuest, static (s, k) => s.Put(k),
            static (s, inner) => s.PutMap(inner, static (s2, qid) => s2.Put(qid), static (s2, sq) => s2.Put(sq))) &&
        ks.Put(IsHenirDungeon) && ks.Put(DungeonEventPoint) && ks.Put(HenirEventPoint) &&
        ks.PutMap(HaveExpInDungeon, static (s, k) => s.Put(k), static (s, v) => s.Put(v)) &&
        ks.Put(IsClearDefenceDungeon) && ks.Put(ProtectedSmallCrystal) &&
        ks.Put(ComeBackUserInParty) &&
        ks.PutMap(SuitableLevelInfo, static (s, k) => s.Put(k), static (s, v) => s.Put(v)) &&
        ks.Put(SpecChar1) && ks.Put(SpecChar2);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int sec) || !ks.Get(out bool win) || !ks.Get(out int did) ||
            !ks.Get(out sbyte diff) || !ks.Get(out sbyte mode) || !ks.GetList(DungeonUnitInfo) ||
            !ks.GetMap(OngoingQuest, static s => (s.Get(out long u), u),
                static s =>
                {
                    var inner = new Dictionary<int, KSubQuestInfo>();
                    bool ok = s.GetMap(inner, static s2 => (s2.Get(out int qid), qid),
                        static s2 => { var sq = new KSubQuestInfo(); return (s2.Get(sq), sq); });
                    return (ok, inner);
                }) ||
            !ks.Get(out bool henir) || !ks.Get(out int dep) || !ks.Get(out int hep) ||
            !ks.GetMap(HaveExpInDungeon, static s => (s.Get(out long u), u), static s => (s.Get(out bool v), v)) ||
            !ks.Get(out bool defClr) || !ks.Get(out int crystal) ||
            !ks.Get(out bool comeback) ||
            !ks.GetMap(SuitableLevelInfo, static s => (s.Get(out long u), u), static s => (s.Get(out bool v), v)) ||
            !ks.Get(out bool sc1) || !ks.Get(out bool sc2))
            return false;
        Sec = sec; IsWin = win; DungeonId = did; Difficulty = diff; DungeonMode = mode;
        IsHenirDungeon = henir; DungeonEventPoint = dep; HenirEventPoint = hep;
        IsClearDefenceDungeon = defClr; ProtectedSmallCrystal = crystal; ComeBackUserInParty = comeback;
        SpecChar1 = sc1; SpecChar2 = sc2;
        return true;
    }
}

// ---------------- Second Security (2nd PIN) ----------------

public sealed class KEGS_SECOND_SECURITY_INFO_NOT : IKSerializable
{
    public int Ok { get; set; }
    public bool UseSecondPw { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.Put(UseSecondPw);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out bool use)) return false;
        Ok = ok; UseSecondPw = use;
        return true;
    }
}

public sealed class KEGS_AUTH_SECOND_SECURITY_REQ : IKSerializable
{
    public long UserUid { get; set; }
    public string SecondPw { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(UserUid) && ks.PutWideString(SecondPw);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.GetWideString(out string pw)) return false;
        UserUid = uid; SecondPw = pw;
        return true;
    }
}

public sealed class KEGS_AUTH_SECOND_SECURITY_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int FailedCount { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.Put(FailedCount);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out int fc)) return false;
        Ok = ok; FailedCount = fc;
        return true;
    }
}

public sealed class KEGS_CREATE_SECOND_SECURITY_REQ : IKSerializable
{
    public string SecondPw { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region) => ks.PutWideString(SecondPw);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string pw)) return false;
        SecondPw = pw;
        return true;
    }
}

// ---------------- Party Dungeon / Ready / Start ----------------

public sealed class KEGS_PARTY_CHANGE_DUNGEON_REQ : IKSerializable
{
    public int DungeonId { get; set; }
    public sbyte DifficultyLevel { get; set; }
    public sbyte DungeonMode { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(DungeonId) && ks.Put(DifficultyLevel) && ks.Put(DungeonMode);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int did) || !ks.Get(out sbyte diff) || !ks.Get(out sbyte mode)) return false;
        DungeonId = did; DifficultyLevel = diff; DungeonMode = mode;
        return true;
    }
}

public sealed class KEGS_PARTY_CHANGE_READY_REQ : IKSerializable
{
    public bool Ready { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ready);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool r)) return false;
        Ready = r;
        return true;
    }
}

public sealed class KEGS_PARTY_CHANGE_READY_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public bool Ready { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(UnitUid) && ks.Put(Ready);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.Get(out bool r)) return false;
        UnitUid = uid; Ready = r;
        return true;
    }
}

public sealed class KEGS_PARTY_GAME_START_REQ : IKSerializable
{
    public short WorldId { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(WorldId);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out short wid)) return false;
        WorldId = wid;
        return true;
    }
}

public sealed class KEGS_UPDATE_PARTY_UID_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public long PartyUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(UnitUid) && ks.Put(PartyUid);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.Get(out long pid)) return false;
        UnitUid = uid; PartyUid = pid;
        return true;
    }
}

// ---------------- Guild Operations & Admin Cheat ----------------

public sealed class KEGS_JOIN_GUILD_REQ : IKSerializable
{
    public int GuildUid { get; set; }
    public string GuildName { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(GuildUid) && ks.PutWideString(GuildName);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int gid) || !ks.GetWideString(out string name)) return false;
        GuildUid = gid; GuildName = name;
        return true;
    }
}

public sealed class KEGS_JOIN_GUILD_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KGuildInfo GuildInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.Put(GuildInfo);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(GuildInfo)) return false;
        Ok = ok;
        return true;
    }
}

public sealed class KEGS_QUIT_GUILD_REQ : IKSerializable
{
    public int GuildUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(GuildUid);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int gid)) return false;
        GuildUid = gid;
        return true;
    }
}

public sealed class KEGS_QUIT_GUILD_ACK : IKSerializable
{
    public int Ok { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok)) return false;
        Ok = ok;
        return true;
    }
}

public sealed class KEGS_KICK_GUILD_MEMBER_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(UnitUid);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid)) return false;
        UnitUid = uid;
        return true;
    }
}

public sealed class KEGS_KICK_GUILD_MEMBER_ACK : IKSerializable
{
    public int Ok { get; set; }
    public bool KickBySysop { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.Put(KickBySysop);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out bool kick)) return false;
        Ok = ok; KickBySysop = kick;
        return true;
    }
}

public sealed class KEGS_CHANGE_GUILD_MEMBER_GRADE_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public byte MembershipGrade { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(UnitUid) && ks.Put(MembershipGrade);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.Get(out byte g)) return false;
        UnitUid = uid; MembershipGrade = g;
        return true;
    }
}

public sealed class KEGS_CHANGE_GUILD_MEMBER_GRADE_ACK : IKSerializable
{
    public int Ok { get; set; }
    public string TargetNickName { get; set; } = string.Empty;
    public byte MembershipGrade { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.PutWideString(TargetNickName) && ks.Put(MembershipGrade);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.GetWideString(out string nick) || !ks.Get(out byte g)) return false;
        Ok = ok; TargetNickName = nick; MembershipGrade = g;
        return true;
    }
}

public sealed class KEGS_ADMIN_CHEAT_REQ : IKSerializable
{
    public string CheatCommand { get; set; } = string.Empty;
    public List<int> IntParams { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.PutWideString(CheatCommand) && ks.PutList(IntParams, static (s, v) => s.Put(v));

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string cmd) || !ks.GetList(IntParams, static s => (s.Get(out int v), v))) return false;
        CheatCommand = cmd;
        return true;
    }
}

public sealed class KEGS_ADMIN_CHEAT_ACK : IKSerializable
{
    public int Ok { get; set; }
    public string ResultMessage { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.PutWideString(ResultMessage);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.GetWideString(out string msg)) return false;
        Ok = ok; ResultMessage = msg;
        return true;
    }
}

// ---------------- Enchant / Restore / Title / Skill / Personal Shop ----------------

public sealed class KEGS_ENCHANT_ITEM_REQ : IKSerializable
{
    public long ItemUid { get; set; }
    public bool IsRareEnchantStone { get; set; }
    public bool IsNewEnchant { get; set; }
    public bool IsSupportMaterial { get; set; }
    public bool IsEnchantPlus { get; set; }
    public bool IsDestroyGuard { get; set; }
    public bool Debug { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(ItemUid) && ks.Put(IsRareEnchantStone) && ks.Put(IsNewEnchant) &&
        ks.Put(IsSupportMaterial) && ks.Put(IsEnchantPlus) && ks.Put(IsDestroyGuard) && ks.Put(Debug);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.Get(out bool rare) || !ks.Get(out bool isNew) ||
            !ks.Get(out bool sup) || !ks.Get(out bool plus) || !ks.Get(out bool guard) || !ks.Get(out bool dbg))
            return false;
        ItemUid = uid; IsRareEnchantStone = rare; IsNewEnchant = isNew;
        IsSupportMaterial = sup; IsEnchantPlus = plus; IsDestroyGuard = guard; Debug = dbg;
        return true;
    }
}

public sealed class KEGS_ENCHANT_ITEM_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int Ed { get; set; }
    public int EnchantResult { get; set; }
    public long EnchantedItemUid { get; set; }
    public int EnchantedItemId { get; set; }
    public List<KInventoryItemInfo> InventorySlotInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(Ed) && ks.Put(EnchantResult) &&
        ks.Put(EnchantedItemUid) && ks.Put(EnchantedItemId) && ks.PutList(InventorySlotInfo);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out int ed) || !ks.Get(out int res) ||
            !ks.Get(out long iuid) || !ks.Get(out int iid) || !ks.GetList(InventorySlotInfo))
            return false;
        Ok = ok; Ed = ed; EnchantResult = res; EnchantedItemUid = iuid; EnchantedItemId = iid;
        return true;
    }
}

public sealed class KEGS_RESTORE_ITEM_REQ : IKSerializable
{
    public long ItemUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(ItemUid);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid)) return false;
        ItemUid = uid;
        return true;
    }
}

public sealed class KEGS_RESTORE_ITEM_ACK : IKSerializable
{
    public int Ok { get; set; }
    public List<KInventoryItemInfo> InventorySlotInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.PutList(InventorySlotInfo);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.GetList(InventorySlotInfo)) return false;
        Ok = ok;
        return true;
    }
}

public sealed class KEGS_EQUIP_TITLE_REQ : IKSerializable
{
    public int TitleId { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(TitleId);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int tid)) return false;
        TitleId = tid;
        return true;
    }
}

public sealed class KEGS_EQUIP_TITLE_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int TitleId { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.Put(TitleId);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out int tid)) return false;
        Ok = ok; TitleId = tid;
        return true;
    }
}

public sealed class KEGS_CHANGE_SKILL_SLOT_REQ : IKSerializable
{
    public int SlotId { get; set; } = -1;
    public int SkillId { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(SlotId) && ks.Put(SkillId);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int sid) || !ks.Get(out int skid)) return false;
        SlotId = sid; SkillId = skid;
        return true;
    }
}

public sealed class KEGS_CHANGE_SKILL_SLOT_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int SlotId { get; set; } = -1;
    public int SkillId { get; set; }
    public int SlotId2 { get; set; } = -1;
    public int SkillId2 { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(SlotId) && ks.Put(SkillId) && ks.Put(SlotId2) && ks.Put(SkillId2);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out int s1) || !ks.Get(out int sk1) ||
            !ks.Get(out int s2) || !ks.Get(out int sk2))
            return false;
        Ok = ok; SlotId = s1; SkillId = sk1; SlotId2 = s2; SkillId2 = sk2;
        return true;
    }
}

public sealed class KEGS_CREATE_PERSONAL_SHOP_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long PersonalShopUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.Put(PersonalShopUid);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out long puid)) return false;
        Ok = ok; PersonalShopUid = puid;
        return true;
    }
}

public sealed class KEGS_BUY_PERSONAL_SHOP_ITEM_REQ : IKSerializable
{
    public long ItemUid { get; set; }
    public int ItemId { get; set; }
    public int Quantity { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(ItemUid) && ks.Put(ItemId) && ks.Put(Quantity);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long iuid) || !ks.Get(out int iid) || !ks.Get(out int qty)) return false;
        ItemUid = iuid; ItemId = iid; Quantity = qty;
        return true;
    }
}

// ---------------- Socket / Resolve / Attribute Enchant ----------------

public sealed class KEGS_SOCKET_ITEM_REQ : IKSerializable
{
    public long ItemUid { get; set; }
    public Dictionary<int, long> SocketInfo { get; set; } = new();
    public bool Cheat { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(ItemUid) &&
        ks.PutMap(SocketInfo, static (s, k) => s.Put(k), static (s, v) => s.Put(v)) &&
        ks.Put(Cheat);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) ||
            !ks.GetMap(SocketInfo, static s => (s.Get(out int k), k), static s => (s.Get(out long v), v)) ||
            !ks.Get(out bool c))
            return false;
        ItemUid = uid; Cheat = c;
        return true;
    }
}

public sealed class KEGS_SOCKET_ITEM_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int Ed { get; set; }
    public long ItemUid { get; set; }
    public List<KInventoryItemInfo> InventorySlotInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(Ed) && ks.Put(ItemUid) && ks.PutList(InventorySlotInfo);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out int ed) || !ks.Get(out long uid) || !ks.GetList(InventorySlotInfo))
            return false;
        Ok = ok; Ed = ed; ItemUid = uid;
        return true;
    }
}

public sealed class KEGS_RESOLVE_ITEM_REQ : IKSerializable
{
    public long ItemUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(ItemUid);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid)) return false;
        ItemUid = uid;
        return true;
    }
}

public sealed class KEGS_RESOLVE_ITEM_ACK : IKSerializable
{
    public int Ok { get; set; }
    public bool Jackpot { get; set; }
    public Dictionary<int, int> InsertedItems { get; set; } = new();
    public List<KInventoryItemInfo> InventorySlotInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(Jackpot) &&
        ks.PutMap(InsertedItems, static (s, k) => s.Put(k), static (s, v) => s.Put(v)) &&
        ks.PutList(InventorySlotInfo);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out bool jp) ||
            !ks.GetMap(InsertedItems, static s => (s.Get(out int k), k), static s => (s.Get(out int v), v)) ||
            !ks.GetList(InventorySlotInfo))
            return false;
        Ok = ok; Jackpot = jp;
        return true;
    }
}

public sealed class KEGS_ATTRIB_ENCHANT_ITEM_REQ : IKSerializable
{
    public long ItemUid { get; set; }
    public sbyte AttribEnchantSlotNo { get; set; }
    public sbyte AttribEnchantId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(ItemUid) && ks.Put(AttribEnchantSlotNo) && ks.Put(AttribEnchantId);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.Get(out sbyte slot) || !ks.Get(out sbyte id)) return false;
        ItemUid = uid; AttribEnchantSlotNo = slot; AttribEnchantId = id;
        return true;
    }
}

public sealed class KEGS_ATTRIB_ENCHANT_ITEM_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int Ed { get; set; }
    public long EnchantedItemUid { get; set; }
    public int EnchantedItemId { get; set; }
    public sbyte AttribEnchantSlotNo { get; set; }
    public sbyte AttribEnchantId { get; set; }
    public List<KInventoryItemInfo> InventorySlotInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(Ed) && ks.Put(EnchantedItemUid) && ks.Put(EnchantedItemId) &&
        ks.Put(AttribEnchantSlotNo) && ks.Put(AttribEnchantId) && ks.PutList(InventorySlotInfo);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out int ed) || !ks.Get(out long uid) || !ks.Get(out int iid) ||
            !ks.Get(out sbyte slot) || !ks.Get(out sbyte id) || !ks.GetList(InventorySlotInfo))
            return false;
        Ok = ok; Ed = ed; EnchantedItemUid = uid; EnchantedItemId = iid;
        AttribEnchantSlotNo = slot; AttribEnchantId = id;
        return true;
    }
}

// ---------------- Player Trade ----------------

public sealed class KEGS_REQUEST_TRADE_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(UnitUid);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid)) return false;
        UnitUid = uid;
        return true;
    }
}

public sealed class KEGS_REQUEST_TRADE_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long UnitUid { get; set; }
    public string NickName { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.Put(UnitUid) && ks.PutWideString(NickName);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out long uid) || !ks.GetWideString(out string nick)) return false;
        Ok = ok; UnitUid = uid; NickName = nick;
        return true;
    }
}

public sealed class KEGS_REQUEST_TRADE_NOT : IKSerializable
{
    public long SenderUid { get; set; }
    public string SenderNickName { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(SenderUid) && ks.PutWideString(SenderNickName);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.GetWideString(out string nick)) return false;
        SenderUid = uid; SenderNickName = nick;
        return true;
    }
}

public sealed class KEGS_REQUEST_TRADE_REPLY_NOT : IKSerializable
{
    public long ReceiverUid { get; set; }
    public bool Accept { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(ReceiverUid) && ks.Put(Accept);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.Get(out bool acc)) return false;
        ReceiverUid = uid; Accept = acc;
        return true;
    }
}

public sealed class KEGS_UPDATE_TRADE_ITEM_REQ : IKSerializable
{
    public long ItemUid { get; set; }
    public int Quantity { get; set; }
    public bool Delete { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(ItemUid) && ks.Put(Quantity) && ks.Put(Delete);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long iuid) || !ks.Get(out int qty) || !ks.Get(out bool del)) return false;
        ItemUid = iuid; Quantity = qty; Delete = del;
        return true;
    }
}

public sealed class KEGS_UPDATE_TRADE_ED_REQ : IKSerializable
{
    public int Ed { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ed);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ed)) return false;
        Ed = ed;
        return true;
    }
}

public sealed class KEGS_TRADE_COMPLETE_NOT : IKSerializable
{
    public List<KInventoryItemInfo> InventorySlotInfo { get; set; } = new();
    public int Ed { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.PutList(InventorySlotInfo) && ks.Put(Ed);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetList(InventorySlotInfo) || !ks.Get(out int ed)) return false;
        Ed = ed;
        return true;
    }
}

// ---------------- Pet & Riding Pet ----------------

public sealed class KPetInfo : IKSerializable
{
    public long PetUid { get; set; }
    public int PetId { get; set; }
    public string PetName { get; set; } = string.Empty;
    public sbyte EvolutionStep { get; set; }
    public short Satiety { get; set; } = 4800;
    public int Intimacy { get; set; }
    public short Extroversion { get; set; }
    public short Emotion { get; set; }
    public bool AutoFeed { get; set; }
    public string LastFeedDate { get; set; } = string.Empty;
    public string LastSummonDate { get; set; } = string.Empty;
    public string RegDate { get; set; } = string.Empty;
    public bool AutoLooting { get; set; }
    public bool FreeAutoLooting { get; set; }
    public string DestroyDate { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(PetUid) && ks.Put(PetId) && ks.PutWideString(PetName) &&
        ks.Put(EvolutionStep) && ks.Put(Satiety) && ks.Put(Intimacy) &&
        ks.Put(Extroversion) && ks.Put(Emotion) && ks.Put(AutoFeed) &&
        ks.PutWideString(LastFeedDate) && ks.PutWideString(LastSummonDate) && ks.PutWideString(RegDate) &&
        ks.Put(AutoLooting) && ks.Put(FreeAutoLooting) && ks.PutWideString(DestroyDate);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.Get(out int pid) || !ks.GetWideString(out string name) ||
            !ks.Get(out sbyte evo) || !ks.Get(out short sat) || !ks.Get(out int inti) ||
            !ks.Get(out short ext) || !ks.Get(out short emo) || !ks.Get(out bool af) ||
            !ks.GetWideString(out string lfd) || !ks.GetWideString(out string lsd) || !ks.GetWideString(out string rd) ||
            !ks.Get(out bool al) || !ks.Get(out bool fal) || !ks.GetWideString(out string dd))
            return false;
        PetUid = uid; PetId = pid; PetName = name; EvolutionStep = evo; Satiety = sat; Intimacy = inti;
        Extroversion = ext; Emotion = emo; AutoFeed = af; LastFeedDate = lfd; LastSummonDate = lsd;
        RegDate = rd; AutoLooting = al; FreeAutoLooting = fal; DestroyDate = dd;
        return true;
    }
}

public sealed class KEGS_SUMMON_PET_REQ : IKSerializable
{
    public long SummonPetUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(SummonPetUid);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid)) return false;
        SummonPetUid = uid;
        return true;
    }
}

public sealed class KEGS_SUMMON_PET_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KPetInfo SummonedPetInfo { get; set; } = new();
    public List<KInventoryItemInfo> PetInventorySlotInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(SummonedPetInfo) && ks.PutList(PetInventorySlotInfo);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(SummonedPetInfo) || !ks.GetList(PetInventorySlotInfo)) return false;
        Ok = ok;
        return true;
    }
}

public sealed class KRidingPetInfo : IKSerializable
{
    public long RidingPetUid { get; set; }
    public ushort RidingPetId { get; set; }
    public float Stamina { get; set; } = 100f;
    public string DestroyDate { get; set; } = string.Empty;
    public int LastUnSummonDate { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(RidingPetUid) && ks.Put(RidingPetId) && ks.Put(Stamina) &&
        ks.PutWideString(DestroyDate) && ks.Put(LastUnSummonDate);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.Get(out ushort id) || !ks.Get(out float stam) ||
            !ks.GetWideString(out string dd) || !ks.Get(out int lusd)) return false;
        RidingPetUid = uid; RidingPetId = id; Stamina = stam; DestroyDate = dd; LastUnSummonDate = lusd;
        return true;
    }
}

public sealed class KEGS_SUMMON_RIDING_PET_REQ : IKSerializable
{
    public long RidingPetUid { get; set; }
    public bool NeedAck { get; set; } = true;

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(RidingPetUid) && ks.Put(NeedAck);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.Get(out bool ack)) return false;
        RidingPetUid = uid; NeedAck = ack;
        return true;
    }
}

public sealed class KEGS_SUMMON_RIDING_PET_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KRidingPetInfo RidingPetInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.Put(RidingPetInfo);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(RidingPetInfo)) return false;
        Ok = ok;
        return true;
    }
}

// ---------------- Skill Tree / Quest GiveUp / Repair / PersonalShop / Pet Create&Feed / AutoParty ----------------

public sealed class KDeletedItemInfo : IKSerializable
{
    public long ItemUid { get; set; }
    public byte DeleteReason { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(ItemUid) && ks.Put(DeleteReason);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.Get(out byte reason)) return false;
        ItemUid = uid; DeleteReason = reason;
        return true;
    }
}

public sealed class KItemQuantityUpdate : IKSerializable
{
    public Dictionary<long, int> QuantityChange { get; set; } = new();
    public List<KDeletedItemInfo> Deleted { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.PutMap(QuantityChange, static (s, k) => s.Put(k), static (s, v) => s.Put(v)) &&
        ks.PutList(Deleted);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetMap(QuantityChange, static s => (s.Get(out long k), k), static s => (s.Get(out int v), v)) ||
            !ks.GetList(Deleted))
            return false;
        return true;
    }
}

public sealed class KEGS_GIVE_UP_QUEST_REQ : IKSerializable
{
    public int QuestId { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(QuestId);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int qid)) return false;
        QuestId = qid;
        return true;
    }
}

public class KQuestAck : IKSerializable
{
    public int Ok { get; set; }
    public int QuestId { get; set; }
    public Dictionary<int, int> InsertedItem { get; set; } = new();
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();
    public Dictionary<int, KItemInfo> ItemInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(QuestId) &&
        ks.PutMap(InsertedItem, static (s, k) => s.Put(k), static (s, v) => s.Put(v)) &&
        ks.Put(ItemQuantityUpdate) &&
        ks.PutList(UpdatedInventorySlot) &&
        ks.PutMap(ItemInfo, static (s, k) => s.Put(k), (s, v) => s.Put(v));

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out int qid) ||
            !ks.GetMap(InsertedItem, static s => (s.Get(out int k), k), static s => (s.Get(out int v), v)) ||
            !ks.Get(ItemQuantityUpdate) ||
            !ks.GetList(UpdatedInventorySlot) ||
            !ks.GetMap(ItemInfo, static s => (s.Get(out int k), k), static s => { var v = new KItemInfo(); return (s.Get(v), v); }))
            return false;
        Ok = ok; QuestId = qid;
        return true;
    }
}

public sealed class KGetSkillInfo : IKSerializable
{
    public int SkillId { get; set; }
    public int BeforeSkillLevel { get; set; }
    public int SkillLevel { get; set; }
    public int SpendSkillCsPoint { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(SkillId) && ks.Put(BeforeSkillLevel) && ks.Put(SkillLevel) && ks.Put(SpendSkillCsPoint);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int sid) || !ks.Get(out int before) || !ks.Get(out int lvl) || !ks.Get(out int csp))
            return false;
        SkillId = sid; BeforeSkillLevel = before; SkillLevel = lvl; SpendSkillCsPoint = csp;
        return true;
    }
}

public sealed class KEGS_GET_SKILL_REQ : IKSerializable
{
    public List<int> NowLearnSkill { get; set; } = new();
    public int ActiveSkillPageNumber { get; set; }
    public Dictionary<int, KGetSkillInfo> SkillList { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.PutList(NowLearnSkill, static (s, v) => s.Put(v)) &&
        ks.Put(ActiveSkillPageNumber) &&
        ks.PutMap(SkillList, static (s, k) => s.Put(k), static (s, v) => s.Put(v));

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetList(NowLearnSkill, static s => (s.Get(out int v), v)) ||
            !ks.Get(out int page) ||
            !ks.GetMap(SkillList, static s => (s.Get(out int k), k), static s => { var v = new KGetSkillInfo(); return (s.Get(v), v); }))
            return false;
        ActiveSkillPageNumber = page;
        return true;
    }
}

public sealed class KEGS_GET_SKILL_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int ActiveSkillPageNumber { get; set; }
    public Dictionary<int, KGetSkillInfo> SkillList { get; set; } = new();
    public int RemainSp { get; set; }
    public int RemainCsp { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(ActiveSkillPageNumber) &&
        ks.PutMap(SkillList, static (s, k) => s.Put(k), static (s, v) => s.Put(v)) &&
        ks.Put(RemainSp) && ks.Put(RemainCsp);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out int page) ||
            !ks.GetMap(SkillList, static s => (s.Get(out int k), k), static s => { var v = new KGetSkillInfo(); return (s.Get(v), v); }) ||
            !ks.Get(out int sp) || !ks.Get(out int csp))
            return false;
        Ok = ok; ActiveSkillPageNumber = page; RemainSp = sp; RemainCsp = csp;
        return true;
    }
}

public sealed class KEGS_RESET_SKILL_REQ : IKSerializable
{
    public int SkillId { get; set; }
    public int ActiveSkillPageNumber { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(SkillId) && ks.Put(ActiveSkillPageNumber);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int sid) || !ks.Get(out int page)) return false;
        SkillId = sid; ActiveSkillPageNumber = page;
        return true;
    }
}

public sealed class KEGS_RESET_SKILL_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int ActiveSkillPageNumber { get; set; }
    public int DelSkillId { get; set; }
    public int SPoint { get; set; }
    public int CSPoint { get; set; }
    public List<KInventoryItemInfo> InventorySlotInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(ActiveSkillPageNumber) && ks.Put(DelSkillId) &&
        ks.Put(SPoint) && ks.Put(CSPoint) && ks.PutList(InventorySlotInfo);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out int page) || !ks.Get(out int del) ||
            !ks.Get(out int sp) || !ks.Get(out int csp) || !ks.GetList(InventorySlotInfo))
            return false;
        Ok = ok; ActiveSkillPageNumber = page; DelSkillId = del; SPoint = sp; CSPoint = csp;
        return true;
    }
}

public sealed class KEGS_INIT_SKILL_TREE_REQ : IKSerializable
{
    public long ItemUid { get; set; }
    public int ActiveSkillPageNumber { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(ItemUid) && ks.Put(ActiveSkillPageNumber);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.Get(out int page)) return false;
        ItemUid = uid; ActiveSkillPageNumber = page;
        return true;
    }
}

public sealed class KEGS_INIT_SKILL_TREE_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int ActiveSkillPageNumber { get; set; }
    public int SPoint { get; set; }
    public int CSPoint { get; set; }
    public List<KInventoryItemInfo> InventorySlotInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(ActiveSkillPageNumber) && ks.Put(SPoint) && ks.Put(CSPoint) && ks.PutList(InventorySlotInfo);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out int page) || !ks.Get(out int sp) ||
            !ks.Get(out int csp) || !ks.GetList(InventorySlotInfo))
            return false;
        Ok = ok; ActiveSkillPageNumber = page; SPoint = sp; CSPoint = csp;
        return true;
    }
}

public sealed class KEGS_REPAIR_ITEM_REQ : IKSerializable
{
    public List<long> ItemUids { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.PutList(ItemUids, static (s, v) => s.Put(v));

    public bool Get(KSerializer ks, RegionProfile region) =>
        ks.GetList(ItemUids, static s => (s.Get(out long v), v));
}

public sealed class KEGS_REPAIR_ITEM_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int Ed { get; set; }
    public int APoint { get; set; }
    public List<KInventoryItemInfo> InventorySlotInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(Ed) && ks.Put(APoint) && ks.PutList(InventorySlotInfo);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out int ed) || !ks.Get(out int ap) || !ks.GetList(InventorySlotInfo))
            return false;
        Ok = ok; Ed = ed; APoint = ap;
        return true;
    }
}

public sealed class KRegPersonalShopItemInfo : IKSerializable
{
    public long ItemUid { get; set; }
    public int Quantity { get; set; }
    public int PricePerOne { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(ItemUid) && ks.Put(Quantity) && ks.Put(PricePerOne);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.Get(out int qty) || !ks.Get(out int price)) return false;
        ItemUid = uid; Quantity = qty; PricePerOne = price;
        return true;
    }
}

public sealed class KSellPersonalShopItemInfo : IKSerializable
{
    public KInventoryItemInfo InventoryItemInfo { get; set; } = new();
    public int PricePerOne { get; set; }
    public int FeePerOne { get; set; }
    public sbyte PShopItemType { get; set; }
    public int TotalSellEdIn { get; set; }
    public int TotalSoldItemQuantity { get; set; }
    public long InventoryItemUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(InventoryItemInfo) && ks.Put(PricePerOne) && ks.Put(FeePerOne) &&
        ks.Put(PShopItemType) && ks.Put(TotalSellEdIn) && ks.Put(TotalSoldItemQuantity) &&
        ks.Put(InventoryItemUid);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(InventoryItemInfo) || !ks.Get(out int price) || !ks.Get(out int fee) ||
            !ks.Get(out sbyte type) || !ks.Get(out int totalEd) || !ks.Get(out int totalQty) ||
            !ks.Get(out long invUid))
            return false;
        PricePerOne = price; FeePerOne = fee; PShopItemType = type;
        TotalSellEdIn = totalEd; TotalSoldItemQuantity = totalQty; InventoryItemUid = invUid;
        return true;
    }
}

public sealed class KEGS_REG_PERSONAL_SHOP_ITEM_REQ : IKSerializable
{
    public string PersonalShopName { get; set; } = string.Empty;
    public List<KRegPersonalShopItemInfo> RegItems { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.PutWideString(PersonalShopName) && ks.PutList(RegItems);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string name) || !ks.GetList(RegItems)) return false;
        PersonalShopName = name;
        return true;
    }
}

public sealed class KEGS_REG_PERSONAL_SHOP_ITEM_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int Ed { get; set; }
    public string PersonalShopName { get; set; } = string.Empty;
    public List<KSellPersonalShopItemInfo> SellItemInfo { get; set; } = new();
    public List<KInventoryItemInfo> InventorySlotInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(Ed) && ks.PutWideString(PersonalShopName) &&
        ks.PutList(SellItemInfo) && ks.PutList(InventorySlotInfo);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out int ed) || !ks.GetWideString(out string name) ||
            !ks.GetList(SellItemInfo) || !ks.GetList(InventorySlotInfo))
            return false;
        Ok = ok; Ed = ed; PersonalShopName = name;
        return true;
    }
}

public sealed class KEGS_CREATE_PET_REQ : IKSerializable
{
    public long ItemUid { get; set; }
    public string PetName { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(ItemUid) && ks.PutWideString(PetName);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.GetWideString(out string name)) return false;
        ItemUid = uid; PetName = name;
        return true;
    }
}

public sealed class KEGS_CREATE_PET_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KPetInfo CreatedPetInfo { get; set; } = new();
    public List<KInventoryItemInfo> InventorySlotInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(CreatedPetInfo) && ks.PutList(InventorySlotInfo);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(CreatedPetInfo) || !ks.GetList(InventorySlotInfo)) return false;
        Ok = ok;
        return true;
    }
}

public sealed class KEGS_FEED_PETS_REQ : IKSerializable
{
    public long ItemUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(ItemUid);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid)) return false;
        ItemUid = uid;
        return true;
    }
}

public sealed class KEGS_FEED_PETS_ACK : IKSerializable
{
    public int Ok { get; set; }
    public short Satiety { get; set; }
    public List<KInventoryItemInfo> InventorySlotInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(Satiety) && ks.PutList(InventorySlotInfo);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out short sat) || !ks.GetList(InventorySlotInfo)) return false;
        Ok = ok; Satiety = sat;
        return true;
    }
}

public sealed class KEGS_PET_EVOLUTION_REQ : IKSerializable
{
    public long PetUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(PetUid);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid)) return false;
        PetUid = uid;
        return true;
    }
}

public sealed class KEGS_PET_EVOLUTION_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KPetInfo EvolutionResultPetInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.Put(EvolutionResultPetInfo);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(EvolutionResultPetInfo)) return false;
        Ok = ok;
        return true;
    }
}

public sealed class KEGS_AUTO_PARTY_DUNGEON_GAME_REQ : IKSerializable
{
    public int DungeonId { get; set; }
    public sbyte DifficultyLevel { get; set; }
    public sbyte GetItemType { get; set; }
    public sbyte DungeonMode { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(DungeonId) && ks.Put(DifficultyLevel) && ks.Put(GetItemType) && ks.Put(DungeonMode);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int did) || !ks.Get(out sbyte diff) || !ks.Get(out sbyte git) || !ks.Get(out sbyte dm))
            return false;
        DungeonId = did; DifficultyLevel = diff; GetItemType = git; DungeonMode = dm;
        return true;
    }
}

public sealed class KEGS_AUTO_PARTY_DUNGEON_GAME_ACK : IKSerializable
{
    public int Ok { get; set; }
    public string FailUserNickName { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.PutWideString(FailUserNickName);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.GetWideString(out string nick)) return false;
        Ok = ok; FailUserNickName = nick;
        return true;
    }
}

public sealed class KEGS_AUTO_PARTY_MAKING_SUCCESS_NOT : IKSerializable
{
    public long AutoPartyUid { get; set; }
    public int MemberCount { get; set; }
    public int DungeonId { get; set; }
    public sbyte DifficultyLevel { get; set; }
    public sbyte DungeonMode { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(AutoPartyUid) && ks.Put(MemberCount) && ks.Put(DungeonId) &&
        ks.Put(DifficultyLevel) && ks.Put(DungeonMode);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long apUid) || !ks.Get(out int cnt) || !ks.Get(out int did) ||
            !ks.Get(out sbyte diff) || !ks.Get(out sbyte dm))
            return false;
        AutoPartyUid = apUid; MemberCount = cnt; DungeonId = did; DifficultyLevel = diff; DungeonMode = dm;
        return true;
    }
}

public sealed class KEGS_AUTO_PARTY_MAKING_SUCCESS_REPLY_NOT : IKSerializable
{
    public bool GameStartAgree { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(GameStartAgree);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool agree)) return false;
        GameStartAgree = agree;
        return true;
    }
}

// ---------------- Manufacture / Cash Shop / Guild Board / Couple Relationship ----------------

public sealed class KEGS_ITEM_MANUFACTURE_REQ : IKSerializable
{
    public int ManufactureId { get; set; }
    public int Quantity { get; set; } = 1;

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(ManufactureId) && ks.Put(Quantity);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int mid) || !ks.Get(out int qty)) return false;
        ManufactureId = mid; Quantity = qty;
        return true;
    }
}

public sealed class KEGS_ITEM_MANUFACTURE_ACK : IKSerializable
{
    public int Ok { get; set; }
    public Dictionary<int, int> InsertedItem { get; set; } = new();
    public int Ed { get; set; }
    public List<KInventoryItemInfo> InventorySlotInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) &&
        ks.PutMap(InsertedItem, static (s, k) => s.Put(k), static (s, v) => s.Put(v)) &&
        ks.Put(Ed) && ks.PutList(InventorySlotInfo);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) ||
            !ks.GetMap(InsertedItem, static s => (s.Get(out int k), k), static s => (s.Get(out int v), v)) ||
            !ks.Get(out int ed) || !ks.GetList(InventorySlotInfo))
            return false;
        Ok = ok; Ed = ed;
        return true;
    }
}

public sealed class KEGS_CHECK_BALANCE_ACK : IKSerializable
{
    public int Ok { get; set; }
    public uint Balance { get; set; }
    public bool OnlyType { get; set; }
    public int CashType { get; set; } = -1;
    public KGlobalCashInfo GlobalCashInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(Balance) && ks.Put(OnlyType) && ks.Put(CashType) && ks.Put(GlobalCashInfo);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out uint bal) ||
            !ks.Get(out bool only) || !ks.Get(out int ctype) || !ks.Get(GlobalCashInfo))
            return false;
        Ok = ok; Balance = bal; OnlyType = only; CashType = ctype;
        return true;
    }
}

public sealed class KNXBTPurchaseReqInfo : IKSerializable
{
    public uint ProductNo { get; set; }
    public ushort OrderQuantity { get; set; } = 1;

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(ProductNo) && ks.Put(OrderQuantity);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint pno) || !ks.Get(out ushort qty)) return false;
        ProductNo = pno; OrderQuantity = qty;
        return true;
    }
}

public sealed class KEGS_BUY_CASH_ITEM_REQ : IKSerializable
{
    public List<KNXBTPurchaseReqInfo> PurchaseReqInfo { get; set; } = new();
    public bool IsWishList { get; set; }
    public int UseCashType { get; set; } // SERV_SUPPORT_SEVERAL_CASH_TYPES

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.PutList(PurchaseReqInfo) && ks.Put(IsWishList) && ks.Put(UseCashType);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetList(PurchaseReqInfo) || !ks.Get(out bool wish) || !ks.Get(out int uct)) return false;
        IsWishList = wish; UseCashType = uct;
        return true;
    }
}

public sealed class KEGS_BUY_CASH_ITEM_ACK : IKSerializable
{
    public int Ok { get; set; }
    public List<KNXBTPurchaseReqInfo> PurchaseReqInfo { get; set; } = new();
    public bool IsWishList { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.PutList(PurchaseReqInfo) && ks.Put(IsWishList);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.GetList(PurchaseReqInfo) || !ks.Get(out bool wish)) return false;
        Ok = ok; IsWishList = wish;
        return true;
    }
}

public sealed class KGuildAdInfo : IKSerializable
{
    public int GuildUid { get; set; }
    public string GuildName { get; set; } = string.Empty;
    public byte GuildLevel { get; set; } = 1;
    public int GuildExp { get; set; }
    public string MasterNickName { get; set; } = string.Empty;
    public ushort CurNumMember { get; set; } = 1;
    public string AdMessage { get; set; } = string.Empty;
    public string FoundingDay { get; set; } = string.Empty;
    public string AdRegDate { get; set; } = string.Empty;
    public string AdEndDate { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(GuildUid) && ks.PutWideString(GuildName) && ks.Put(GuildLevel) && ks.Put(GuildExp) &&
        ks.PutWideString(MasterNickName) && ks.Put(CurNumMember) && ks.PutWideString(AdMessage) &&
        ks.PutWideString(FoundingDay) && ks.PutWideString(AdRegDate) && ks.PutWideString(AdEndDate);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int guid) || !ks.GetWideString(out string gname) || !ks.Get(out byte lvl) ||
            !ks.Get(out int exp) || !ks.GetWideString(out string mnick) || !ks.Get(out ushort cnt) ||
            !ks.GetWideString(out string msg) || !ks.GetWideString(out string fday) ||
            !ks.GetWideString(out string rdate) || !ks.GetWideString(out string edate))
            return false;
        GuildUid = guid; GuildName = gname; GuildLevel = lvl; GuildExp = exp; MasterNickName = mnick;
        CurNumMember = cnt; AdMessage = msg; FoundingDay = fday; AdRegDate = rdate; AdEndDate = edate;
        return true;
    }
}

public sealed class KEGS_GET_GUILD_AD_LIST_REQ : IKSerializable
{
    public sbyte SortType { get; set; }
    public uint ViewPage { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(SortType) && ks.Put(ViewPage);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte sort) || !ks.Get(out uint page)) return false;
        SortType = sort; ViewPage = page;
        return true;
    }
}

public sealed class KEGS_GET_GUILD_AD_LIST_ACK : IKSerializable
{
    public sbyte SortType { get; set; }
    public uint TotalPage { get; set; }
    public uint ViewPage { get; set; }
    public List<KGuildAdInfo> GuildAdList { get; set; } = new();
    public bool CanShowApplyList { get; set; }
    public bool RegMyGuildAd { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(SortType) && ks.Put(TotalPage) && ks.Put(ViewPage) &&
        ks.PutList(GuildAdList) && ks.Put(CanShowApplyList) && ks.Put(RegMyGuildAd);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte sort) || !ks.Get(out uint tp) || !ks.Get(out uint vp) ||
            !ks.GetList(GuildAdList) || !ks.Get(out bool canShow) || !ks.Get(out bool reg))
            return false;
        SortType = sort; TotalPage = tp; ViewPage = vp; CanShowApplyList = canShow; RegMyGuildAd = reg;
        return true;
    }
}

public sealed class KEGS_COUPLE_PROPOSE_REQ : IKSerializable
{
    public string UnitName { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region) => ks.PutWideString(UnitName);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string name)) return false;
        UnitName = name;
        return true;
    }
}

public sealed class KEGS_COUPLE_PROPOSE_ACK : IKSerializable
{
    public int Ok { get; set; }
    public string NickName { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.PutWideString(NickName);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.GetWideString(out string nick)) return false;
        Ok = ok; NickName = nick;
        return true;
    }
}

public sealed class KEGS_COUPLE_PROPOSE_NOT : IKSerializable
{
    public string RequestUnitName { get; set; } = string.Empty;
    public long RequestUnitUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.PutWideString(RequestUnitName) && ks.Put(RequestUnitUid);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string name) || !ks.Get(out long uid)) return false;
        RequestUnitName = name; RequestUnitUid = uid;
        return true;
    }
}

public sealed class KEGS_COUPLE_PROPOSE_AGREE_NOT : IKSerializable
{
    public sbyte Answer { get; set; }
    public long RequestUnitUid { get; set; }
    public string RequestUnitName { get; set; } = string.Empty;
    public long AcceptUserUid { get; set; }
    public byte AcceptUnitLevel { get; set; }
    public sbyte AcceptUnitClass { get; set; }
    public long AcceptUnitUid { get; set; }
    public string AcceptUnitName { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Answer) && ks.Put(RequestUnitUid) && ks.PutWideString(RequestUnitName) &&
        ks.Put(AcceptUserUid) && ks.Put(AcceptUnitLevel) && ks.Put(AcceptUnitClass) &&
        ks.Put(AcceptUnitUid) && ks.PutWideString(AcceptUnitName);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte ans) || !ks.Get(out long reqUid) || !ks.GetWideString(out string reqName) ||
            !ks.Get(out long accUserUid) || !ks.Get(out byte accLvl) || !ks.Get(out sbyte accCls) ||
            !ks.Get(out long accUnitUid) || !ks.GetWideString(out string accName))
            return false;
        Answer = ans; RequestUnitUid = reqUid; RequestUnitName = reqName;
        AcceptUserUid = accUserUid; AcceptUnitLevel = accLvl; AcceptUnitClass = accCls;
        AcceptUnitUid = accUnitUid; AcceptUnitName = accName;
        return true;
    }
}

public sealed class KEGS_COUPLE_PROPOSE_RESULT_NOT : IKSerializable
{
    public int Ok { get; set; }
    public sbyte Result { get; set; }
    public long UnitUid { get; set; }
    public string UnitName { get; set; } = string.Empty;
    public long DateUnix { get; set; }
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(Result) && ks.Put(UnitUid) && ks.PutWideString(UnitName) &&
        ks.Put(DateUnix) && ks.PutList(UpdatedInventorySlot);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out sbyte res) || !ks.Get(out long uid) ||
            !ks.GetWideString(out string name) || !ks.Get(out long dt) || !ks.GetList(UpdatedInventorySlot))
            return false;
        Ok = ok; Result = res; UnitUid = uid; UnitName = name; DateUnix = dt;
        return true;
    }
}

public sealed class KEGS_NEW_POST_LETTER_INFO_NOT : IKSerializable
{
    public int NewUserLetterCount { get; set; }
    public int NewSystemLetterCount { get; set; }
    public int NewRelationshipLetterCount { get; set; }
    public int TotalLetterCount { get; set; }
    public List<KPostSmallTitleInfo> NewLetterTitle { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(NewUserLetterCount) && ks.Put(NewSystemLetterCount) && ks.Put(NewRelationshipLetterCount) &&
        ks.Put(TotalLetterCount) && ks.PutList(NewLetterTitle);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int nu) || !ks.Get(out int ns) || !ks.Get(out int nr) ||
            !ks.Get(out int total) || !ks.GetList(NewLetterTitle))
            return false;
        NewUserLetterCount = nu; NewSystemLetterCount = ns; NewRelationshipLetterCount = nr; TotalLetterCount = total;
        return true;
    }
}

public sealed class KEGS_RECEIVE_LETTER_NOT : IKSerializable
{
    public string NickName { get; set; } = string.Empty;
    public sbyte ScriptType { get; set; } = 1;
    public string LetterTitle { get; set; } = string.Empty;
    public int ScriptIndex { get; set; }
    public long FromUnitUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.PutWideString(NickName) && ks.Put(ScriptType) && ks.PutWideString(LetterTitle) &&
        ks.Put(ScriptIndex) && ks.Put(FromUnitUid);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string nick) || !ks.Get(out sbyte st) || !ks.GetWideString(out string title) ||
            !ks.Get(out int si) || !ks.Get(out long fuid))
            return false;
        NickName = nick; ScriptType = st; LetterTitle = title; ScriptIndex = si; FromUnitUid = fuid;
        return true;
    }
}

public sealed class KEGS_GET_ITEM_FROM_LETTER_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long PostNo { get; set; }
    public sbyte LetterType { get; set; } = 1;
    public Dictionary<int, int> ResultItem { get; set; } = new();
    public List<KInventoryItemInfo> InventorySlotInfo { get; set; } = new();
    public int Ed { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(PostNo) && ks.Put(LetterType) &&
        ks.PutMap(ResultItem, static (s, k) => s.Put(k), static (s, v) => s.Put(v)) &&
        ks.PutList(InventorySlotInfo) && ks.Put(Ed);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out long pno) || !ks.Get(out sbyte lt) ||
            !ks.GetMap(ResultItem, static s => (s.Get(out int k), k), static s => (s.Get(out int v), v)) ||
            !ks.GetList(InventorySlotInfo) || !ks.Get(out int ed))
            return false;
        Ok = ok; PostNo = pno; LetterType = lt; Ed = ed;
        return true;
    }
}

public sealed class KEGS_DELETE_LETTER_REQ : IKSerializable
{
    public List<long> PostNos { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.PutList(PostNos, static (s, v) => s.Put(v));

    public bool Get(KSerializer ks, RegionProfile region) =>
        ks.GetList(PostNos, static s => (s.Get(out long v), v));
}

public sealed class KEGS_DELETE_LETTER_ACK : IKSerializable
{
    public int Ok { get; set; }
    public List<long> PostNos { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.PutList(PostNos, static (s, v) => s.Put(v));

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.GetList(PostNos, static s => (s.Get(out long v), v)))
            return false;
        Ok = ok;
        return true;
    }
}

public sealed class KEGS_NEW_BLACKLIST_USER_REQ : IKSerializable
{
    public string NickName { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region) => ks.PutWideString(NickName);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string nick)) return false;
        NickName = nick;
        return true;
    }
}

public sealed class KEGS_NEW_BLACKLIST_USER_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KChatBlackListUnit ChatBlackListUnit { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.Put(ChatBlackListUnit);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(ChatBlackListUnit)) return false;
        Ok = ok;
        return true;
    }
}

public sealed class KEGS_DEL_BLACKLIST_USER_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(UnitUid);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid)) return false;
        UnitUid = uid;
        return true;
    }
}

public sealed class KEGS_UPDATE_COMMUNITY_OPTION_REQ : IKSerializable
{
    public KDenyOptions DenyOptions { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(DenyOptions);
    public bool Get(KSerializer ks, RegionProfile region) => ks.Get(DenyOptions);
}

public sealed class KEGS_UPDATE_COMMUNITY_OPTION_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KDenyOptions DenyOptions { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.Put(DenyOptions);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(DenyOptions)) return false;
        Ok = ok;
        return true;
    }
}

public sealed class KEGS_COMMUNITY_USER_LIST_REQ : IKSerializable
{
    public uint ViewPage { get; set; } = 1;
    public bool Mode { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(ViewPage) && ks.Put(Mode);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint vp) || !ks.Get(out bool mode)) return false;
        ViewPage = vp; Mode = mode;
        return true;
    }
}

public sealed class KEGS_COMMUNITY_USER_LIST_ACK : IKSerializable
{
    public int Ok { get; set; }
    public uint TotalPage { get; set; } = 1;
    public uint ViewPage { get; set; } = 1;
    public List<KCommunityUserInfo> UserList { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(TotalPage) && ks.Put(ViewPage) && ks.PutList(UserList);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out uint tp) || !ks.Get(out uint vp) || !ks.GetList(UserList))
            return false;
        Ok = ok; TotalPage = tp; ViewPage = vp;
        return true;
    }
}

public sealed class KEGS_REQUEST_FRIEND_REQ : IKSerializable
{
    public string NickName { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region) => ks.PutWideString(NickName) && ks.PutWideString(Message);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string nick) || !ks.GetWideString(out string msg)) return false;
        NickName = nick; Message = msg;
        return true;
    }
}

public sealed class KEGS_REQUEST_FRIEND_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long UnitUid { get; set; }
    public string NickName { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.Put(UnitUid) && ks.PutWideString(NickName);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out long uid) || !ks.GetWideString(out string nick)) return false;
        Ok = ok; UnitUid = uid; NickName = nick;
        return true;
    }
}

public sealed class KEGS_REQUEST_FRIEND_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public string NickName { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(UnitUid) && ks.PutWideString(NickName) && ks.PutWideString(Message);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.GetWideString(out string nick) || !ks.GetWideString(out string msg)) return false;
        UnitUid = uid; NickName = nick; Message = msg;
        return true;
    }
}

public sealed class KEGS_FRIEND_UNIT_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long UnitUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.Put(UnitUid);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out long uid)) return false;
        Ok = ok; UnitUid = uid;
        return true;
    }
}

public sealed class KEGS_DENY_FRIEND_NOT : IKSerializable
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

public sealed class KEGS_AUTO_PARTY_CLOSE_NOT : IKSerializable
{
    public int Reason { get; set; }
    public long CancelUnitUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Reason) && ks.Put(CancelUnitUid);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int reason) || !ks.Get(out long uid)) return false;
        Reason = reason; CancelUnitUid = uid;
        return true;
    }
}

public sealed class KEGS_REGROUP_PARTY_NOT : IKSerializable
{
    public KPartyInfo PartyInfo { get; set; } = new();
    public List<KPartyUserInfo> PartyUserInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(PartyInfo) && ks.PutList(PartyUserInfo);
    public bool Get(KSerializer ks, RegionProfile region) => ks.Get(PartyInfo) && ks.GetList(PartyUserInfo);
}

public sealed class KEGS_MATCH_MAKING_SUCCESS_NOT : IKSerializable
{
    public long MatchUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(MatchUid);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid)) return false;
        MatchUid = uid;
        return true;
    }
}

public sealed class KEGS_MATCH_MAKING_SUCCESS_REPLY_NOT : IKSerializable
{
    public bool GameStartAgree { get; set; }
    public bool PvpPowerMode { get; set; }
    public bool PvpItemMode { get; set; }
    public List<short> SelectedPvpMap { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(GameStartAgree) && ks.Put(PvpPowerMode) && ks.Put(PvpItemMode) &&
        ks.PutList(SelectedPvpMap, static (s, v) => s.Put(v));

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool agree) || !ks.Get(out bool pwr) || !ks.Get(out bool item) ||
            !ks.GetList(SelectedPvpMap, static s => (s.Get(out short v), v)))
            return false;
        GameStartAgree = agree; PvpPowerMode = pwr; PvpItemMode = item;
        return true;
    }
}

public sealed class KEGS_MATCH_CLOSE_NOT : IKSerializable
{
    public int Reason { get; set; }
    public long CancelUnitUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Reason) && ks.Put(CancelUnitUid);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int reason) || !ks.Get(out long uid)) return false;
        Reason = reason; CancelUnitUid = uid;
        return true;
    }
}

public sealed class KPvpNpcInfo : IKSerializable
{
    public int NpcId { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(NpcId);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int id)) return false;
        NpcId = id;
        return true;
    }
}

public sealed class KEGS_GAME_START_PVP_MATCH_NOT : IKSerializable
{
    public KRoomInfo RoomInfo { get; set; } = new();
    public List<KRoomSlotInfo> Slots { get; set; } = new();
    public List<KRoomSlotInfo> ObserverSlots { get; set; } = new();
    public Dictionary<long, KPvpNpcInfo> PvpNpcInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(RoomInfo) && ks.PutList(Slots) && ks.PutList(ObserverSlots) &&
        ks.PutMap(PvpNpcInfo, static (s, k) => s.Put(k), static (s, v) => s.Put(v));

    public bool Get(KSerializer ks, RegionProfile region) =>
        ks.Get(RoomInfo) && ks.GetList(Slots) && ks.GetList(ObserverSlots) &&
        ks.GetMap(PvpNpcInfo, static s => (s.Get(out long k), k), static s => { var v = new KPvpNpcInfo(); return (s.Get(v), v); });
}

public sealed class KEGS_JOIN_ROOM_NOT : IKSerializable
{
    public KRoomSlotInfo JoinSlot { get; set; } = new();
    public int StartPosIndex { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(JoinSlot) && ks.Put(StartPosIndex);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(JoinSlot) || !ks.Get(out int idx)) return false;
        StartPosIndex = idx;
        return true;
    }
}

public sealed class KEGS_DELETE_ITEM_REQ : IKSerializable
{
    public long ItemUid { get; set; }
    public int Quantity { get; set; } = 1;

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(ItemUid) && ks.Put(Quantity);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.Get(out int qty)) return false;
        ItemUid = uid; Quantity = qty;
        return true;
    }
}

public sealed class KEGS_DELETE_ITEM_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KInventoryItemInfo InventoryItemInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.Put(InventoryItemInfo);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(InventoryItemInfo)) return false;
        Ok = ok;
        return true;
    }
}

public sealed class KEGS_DUNGEON_STAGE_LOAD_REQ : IKSerializable
{
    public int StageId { get; set; }
    public short PartyFever { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(StageId) && ks.Put(PartyFever);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int sid) || !ks.Get(out short pf)) return false;
        StageId = sid; PartyFever = pf;
        return true;
    }
}

public sealed class KEGS_DUNGEON_STAGE_LOAD_NOT : IKSerializable
{
    public int StageId { get; set; }
    public int NumMember { get; set; }
    public Dictionary<int, KNPCList> NpcData { get; set; } = new();
    public Dictionary<int, KAttribEnchantNpcInfo> AttribNpcInfo { get; set; } = new();
    public float ItemDropEventProbRate { get; set; } = 1.0f;
    public bool WithPlayPcBangEvent { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(StageId) && ks.Put(NumMember) &&
        ks.PutMap(NpcData, static (s, k) => s.Put(k), static (s, v) => s.Put(v)) &&
        ks.PutMap(AttribNpcInfo, static (s, k) => s.Put(k), static (s, v) => s.Put(v)) &&
        ks.Put(ItemDropEventProbRate) && ks.Put(WithPlayPcBangEvent);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int sid) || !ks.Get(out int nm) ||
            !ks.GetMap(NpcData, static s => (s.Get(out int k), k), static s => { var v = new KNPCList(); return (s.Get(v), v); }) ||
            !ks.GetMap(AttribNpcInfo, static s => (s.Get(out int k), k), static s => { var v = new KAttribEnchantNpcInfo(); return (s.Get(v), v); }) ||
            !ks.Get(out float rate) || !ks.Get(out bool pcbang))
            return false;
        StageId = sid; NumMember = nm; ItemDropEventProbRate = rate; WithPlayPcBangEvent = pcbang;
        return true;
    }
}

public sealed class KEGS_DUNGEON_STAGE_LOAD_COMPLETE_REQ : IKSerializable
{
    public bool IsSecretStage { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(IsSecretStage);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool sec)) return false;
        IsSecretStage = sec;
        return true;
    }
}

public sealed class KEGS_DUNGEON_STAGE_LOAD_COMPLETE_NOT : IKSerializable
{
    public int SlotId { get; set; }
    public long UnitUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(SlotId) && ks.Put(UnitUid);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int sid) || !ks.Get(out long uid)) return false;
        SlotId = sid; UnitUid = uid;
        return true;
    }
}

public sealed class KEGS_DUNGEON_STAGE_START_NOT : IKSerializable
{
    public int StageId { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(StageId);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int sid)) return false;
        StageId = sid;
        return true;
    }
}

public sealed class KEGS_DUNGEON_SUB_STAGE_ID_MSG : IKSerializable
{
    public sbyte SubStageId { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(SubStageId);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte sid)) return false;
        SubStageId = sid;
        return true;
    }
}

public sealed class KEGS_DUNGEON_SUB_STAGE_GO_NEXT_MSG : IKSerializable
{
    public sbyte BeforeStageId { get; set; }
    public sbyte NextStageId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(BeforeStageId) && ks.Put(NextStageId);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte b) || !ks.Get(out sbyte n)) return false;
        BeforeStageId = b; NextStageId = n;
        return true;
    }
}

public sealed class KEGS_DUNGEON_SUB_STAGE_CLEAR_REQ : IKSerializable
{
    public int ClearConditionIndex { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(ClearConditionIndex);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int idx)) return false;
        ClearConditionIndex = idx;
        return true;
    }
}

public sealed class KEGS_DUNGEON_SUB_STAGE_CLEAR_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int ClearType { get; set; }
    public int StageIndex { get; set; }
    public int SubStageIndex { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(ClearType) && ks.Put(StageIndex) && ks.Put(SubStageIndex);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out int ct) || !ks.Get(out int si) || !ks.Get(out int ssi))
            return false;
        Ok = ok; ClearType = ct; StageIndex = si; SubStageIndex = ssi;
        return true;
    }
}

public sealed class KNPCUnitNot : IKSerializable
{
    public KNPCUnitReq NpcUnitReq { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(NpcUnitReq);
    public bool Get(KSerializer ks, RegionProfile region) => ks.Get(NpcUnitReq);
}

public sealed class KEGS_NPC_UNIT_CREATE_REQ : IKSerializable
{
    public List<KNPCUnitReq> NpcUnitReqs { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region) => ks.PutList(NpcUnitReqs);
    public bool Get(KSerializer ks, RegionProfile region) => ks.GetList(NpcUnitReqs);
}

public sealed class KEGS_NPC_UNIT_CREATE_NOT : IKSerializable
{
    public List<KNPCUnitNot> NpcUnitAcks { get; set; } = new();
    public Dictionary<int, KAttribEnchantNpcInfo> AttribNpcInfo { get; set; } = new();
    public int BattleFieldId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.PutList(NpcUnitAcks) &&
        ks.PutMap(AttribNpcInfo, static (s, k) => s.Put(k), static (s, v) => s.Put(v)) &&
        ks.Put(BattleFieldId);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetList(NpcUnitAcks) ||
            !ks.GetMap(AttribNpcInfo, static s => (s.Get(out int k), k), static s => { var v = new KAttribEnchantNpcInfo(); return (s.Get(v), v); }) ||
            !ks.Get(out int bfid))
            return false;
        BattleFieldId = bfid;
        return true;
    }
}

public sealed class KEGS_NPC_UNIT_DIE_REQ : IKSerializable
{
    public int DieNpcUid { get; set; }
    public sbyte AttUnitType { get; set; }
    public long AttUnit { get; set; }
    public long MdAttUnit { get; set; }
    public KVector3 DiePos { get; set; } = new();
    public sbyte NpcDieState { get; set; }
    public int NpcMaxHp { get; set; }
    public Dictionary<long, float> DamageByUser { get; set; } = new();
    public long FinisherUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(DieNpcUid) && ks.Put(AttUnitType) && ks.Put(AttUnit) && ks.Put(MdAttUnit) &&
        ks.Put(DiePos) && ks.Put(NpcDieState) && ks.Put(NpcMaxHp) &&
        ks.PutMap(DamageByUser, static (s, k) => s.Put(k), static (s, v) => s.Put(v)) &&
        ks.Put(FinisherUid);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int nuid) || !ks.Get(out sbyte aut) || !ks.Get(out long au) || !ks.Get(out long md) ||
            !ks.Get(DiePos) || !ks.Get(out sbyte st) || !ks.Get(out int hp) ||
            !ks.GetMap(DamageByUser, static s => (s.Get(out long k), k), static s => (s.Get(out float v), v)) ||
            !ks.Get(out long fin))
            return false;
        DieNpcUid = nuid; AttUnitType = aut; AttUnit = au; MdAttUnit = md;
        NpcDieState = st; NpcMaxHp = hp; FinisherUid = fin;
        return true;
    }
}

public sealed class KEGS_NPC_UNIT_DIE_NOT : IKSerializable
{
    public int DieNpcUid { get; set; }
    public int NpcId { get; set; }
    public sbyte AttUnitType { get; set; }
    public long AttUnit { get; set; }
    public Dictionary<long, KEXPData> ExpList { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(DieNpcUid) && ks.Put(NpcId) && ks.Put(AttUnitType) && ks.Put(AttUnit) &&
        ks.PutMap(ExpList, static (s, k) => s.Put(k), static (s, v) => s.Put(v));

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int nuid) || !ks.Get(out int nid) || !ks.Get(out sbyte aut) || !ks.Get(out long au) ||
            !ks.GetMap(ExpList, static s => (s.Get(out long k), k), static s => { var v = new KEXPData(); return (s.Get(v), v); }))
            return false;
        DieNpcUid = nuid; NpcId = nid; AttUnitType = aut; AttUnit = au;
        return true;
    }
}

public sealed class KEGS_GET_ITEM_REQ : IKSerializable
{
    public int DropItemUid { get; set; }
    public KItemInfo ItemInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(DropItemUid) && ks.Put(ItemInfo);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int duid) || !ks.Get(ItemInfo)) return false;
        DropItemUid = duid;
        return true;
    }
}

public sealed class KEGS_GET_ITEM_NOT : IKSerializable
{
    public long GetUnitUid { get; set; }
    public int ItemId { get; set; }
    public int DropItemUid { get; set; }
    public sbyte GetItemType { get; set; }
    public bool IsItemSuccess { get; set; } = true;
    public int TotalEd { get; set; }
    public Dictionary<long, KDungeonRewardED> GetEd { get; set; } = new();
    public sbyte EnchantLevel { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(GetUnitUid) && ks.Put(ItemId) && ks.Put(DropItemUid) && ks.Put(GetItemType) &&
        ks.Put(IsItemSuccess) && ks.Put(TotalEd) &&
        ks.PutMap(GetEd, static (s, k) => s.Put(k), static (s, v) => s.Put(v)) &&
        ks.Put(EnchantLevel);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.Get(out int iid) || !ks.Get(out int duid) || !ks.Get(out sbyte git) ||
            !ks.Get(out bool ok) || !ks.Get(out int ted) ||
            !ks.GetMap(GetEd, static s => (s.Get(out long k), k), static s => { var v = new KDungeonRewardED(); return (s.Get(v), v); }) ||
            !ks.Get(out sbyte ench))
            return false;
        GetUnitUid = uid; ItemId = iid; DropItemUid = duid; GetItemType = git;
        IsItemSuccess = ok; TotalEd = ted; EnchantLevel = ench;
        return true;
    }
}

public sealed class KEGS_USER_UNIT_DIE_REQ : IKSerializable
{
    public int KillerNpcUid { get; set; }
    public long KillerUserUnitUid { get; set; }
    public long MaxDamageKillerUserUnitUid { get; set; }
    public byte DieReason { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(KillerNpcUid) && ks.Put(KillerUserUnitUid) && ks.Put(MaxDamageKillerUserUnitUid) && ks.Put(DieReason);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int kn) || !ks.Get(out long ku) || !ks.Get(out long mdk) || !ks.Get(out byte dr))
            return false;
        KillerNpcUid = kn; KillerUserUnitUid = ku; MaxDamageKillerUserUnitUid = mdk; DieReason = dr;
        return true;
    }
}

public sealed class KEGS_USER_UNIT_DIE_ACK : IKSerializable
{
    public float ReBirthTime { get; set; }
    public int Ok { get; set; }
    public short Vp { get; set; }
    public short Exp { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(ReBirthTime) && ks.Put(Ok) && ks.Put(Vp) && ks.Put(Exp);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out float rbt) || !ks.Get(out int ok) || !ks.Get(out short vp) || !ks.Get(out short exp))
            return false;
        ReBirthTime = rbt; Ok = ok; Vp = vp; Exp = exp;
        return true;
    }
}

public sealed class KEGS_USER_UNIT_DIE_NOT : IKSerializable
{
    public int KillerNpcUid { get; set; }
    public long KillerUserUnitUid { get; set; }
    public long MaxDamageKillerUserUnitUid { get; set; }
    public long KilledUserUnitUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(KillerNpcUid) && ks.Put(KillerUserUnitUid) && ks.Put(MaxDamageKillerUserUnitUid) && ks.Put(KilledUserUnitUid);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int kn) || !ks.Get(out long ku) || !ks.Get(out long mdk) || !ks.Get(out long kdu))
            return false;
        KillerNpcUid = kn; KillerUserUnitUid = ku; MaxDamageKillerUserUnitUid = mdk; KilledUserUnitUid = kdu;
        return true;
    }
}

public sealed class KEGS_RESURRECT_TO_CONTINUE_DUNGEON_REQ : IKSerializable
{
    public bool UseCash { get; set; }
    public long UsedByUnitUid { get; set; }
    public long UnitUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(UseCash) && ks.Put(UsedByUnitUid) && ks.Put(UnitUid);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool uc) || !ks.Get(out long ubu) || !ks.Get(out long uid)) return false;
        UseCash = uc; UsedByUnitUid = ubu; UnitUid = uid;
        return true;
    }
}

public sealed class KEGS_RESURRECT_TO_CONTINUE_DUNGEON_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int DungeonId { get; set; }
    public int NumResurrectionStone { get; set; }
    public int NumAutoPaymentResStone { get; set; }
    public bool UseCash { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(DungeonId) && ks.Put(NumResurrectionStone) && ks.Put(NumAutoPaymentResStone) && ks.Put(UseCash);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out int did) || !ks.Get(out int nrs) || !ks.Get(out int nap) || !ks.Get(out bool uc))
            return false;
        Ok = ok; DungeonId = did; NumResurrectionStone = nrs; NumAutoPaymentResStone = nap; UseCash = uc;
        return true;
    }
}

public sealed class KEGS_RESURRECT_TO_CONTINUE_DUNGEON_NOT : IKSerializable
{
    public long UsedByUnitUid { get; set; }
    public long UnitUid { get; set; }
    public int StartPosIndex { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(UsedByUnitUid) && ks.Put(UnitUid) && ks.Put(StartPosIndex);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long ubu) || !ks.Get(out long uid) || !ks.Get(out int spi)) return false;
        UsedByUnitUid = ubu; UnitUid = uid; StartPosIndex = spi;
        return true;
    }
}

public sealed class KEGS_JOIN_BATTLE_FIELD_REQ : IKSerializable
{
    public KBattleFieldJoinInfo BattleFieldJoinInfo { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(BattleFieldJoinInfo);
    public bool Get(KSerializer ks, RegionProfile region) => ks.Get(BattleFieldJoinInfo);
}

public sealed class KEGS_JOIN_BATTLE_FIELD_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KBattleFieldJoinInfo BattleFieldJoinInfo { get; set; } = new();
    public KRoomInfo RoomInfo { get; set; } = new();
    public List<KRoomSlotInfo> Slots { get; set; } = new();
    public string CnIp { get; set; } = string.Empty;
    public int RequireLevel { get; set; }
    public int RequireDungeonId { get; set; }
    public string UdpRelayIp { get; set; } = string.Empty;
    public ushort UdpRelayPort { get; set; }
    public List<long> EnterCashShopUser { get; set; } = new();
    public long RemainFieldHoldingTime { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(BattleFieldJoinInfo) && ks.Put(RoomInfo) && ks.PutList(Slots) &&
        ks.PutWideString(CnIp) && ks.Put(RequireLevel) && ks.Put(RequireDungeonId) &&
        ks.PutWideString(UdpRelayIp) && ks.Put(UdpRelayPort) &&
        ks.PutList(EnterCashShopUser, static (s, v) => s.Put(v)) &&
        ks.Put(RemainFieldHoldingTime);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(BattleFieldJoinInfo) || !ks.Get(RoomInfo) || !ks.GetList(Slots) ||
            !ks.GetWideString(out string cnip) || !ks.Get(out int rlvl) || !ks.Get(out int rdid) ||
            !ks.GetWideString(out string uip) || !ks.Get(out ushort uport) ||
            !ks.GetList(EnterCashShopUser, static s => (s.Get(out long v), v)) ||
            !ks.Get(out long rht))
            return false;
        Ok = ok; CnIp = cnip; RequireLevel = rlvl; RequireDungeonId = rdid;
        UdpRelayIp = uip; UdpRelayPort = uport; RemainFieldHoldingTime = rht;
        return true;
    }
}

public sealed class KEGS_UPDATE_BATTLE_FIELD_USER_POS_NOT : IKSerializable
{
    public KLastPositionInfo LastPos { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(LastPos);
    public bool Get(KSerializer ks, RegionProfile region) => ks.Get(LastPos);
}

public sealed class KEGS_BATTLE_FIELD_NPC_P2P_SYNC_NOT : IKSerializable
{
    public List<long> NonNpcSyncUserList { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region) => ks.PutList(NonNpcSyncUserList, static (s, v) => s.Put(v));
    public bool Get(KSerializer ks, RegionProfile region) => ks.GetList(NonNpcSyncUserList, static s => (s.Get(out long v), v));
}

// -------- Session / Channel / Field / Room Common / Party / PvP Room Packets --------

public sealed class KPacketLong : IKSerializable
{
    public long Value { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Value);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v)) return false;
        Value = v;
        return true;
    }
}

public sealed class KPacketSByte : IKSerializable
{
    public sbyte Value { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Value);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte v)) return false;
        Value = v;
        return true;
    }
}

public sealed class KPacketBool : IKSerializable
{
    public bool Value { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Value);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool v)) return false;
        Value = v;
        return true;
    }
}

public sealed class KEGS_FIELD_LOADING_COMPLETE_REQ : IKSerializable
{
    public KVector3 Pos { get; set; } = new();
    public byte LastTouchLineIndex { get; set; }
    public ushort LastPosValue { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Pos) && ks.Put(LastTouchLineIndex) && ks.Put(LastPosValue);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(Pos) || !ks.Get(out byte line) || !ks.Get(out ushort val)) return false;
        LastTouchLineIndex = line; LastPosValue = val;
        return true;
    }
}

public sealed class KEGS_OPTION_UPDATE_REQ : IKSerializable
{
    public KFieldOption FieldOption { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(FieldOption);
    public bool Get(KSerializer ks, RegionProfile region) => ks.Get(FieldOption);
}

public sealed class KEGS_GET_CHANNEL_LIST_ACK : IKSerializable
{
    public int ServerGroupId { get; set; }
    public List<KChannelInfo> ChannelInfo { get; set; } = new();
    public Dictionary<int, KChannelBonusInfo> ChannelBonusInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(ServerGroupId) && ks.PutList(ChannelInfo) &&
        ks.PutMap(ChannelBonusInfo, static (s, k) => s.Put(k), static (s, v) => v.Put(s, s.Region));

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int gid) || !ks.GetList(ChannelInfo) ||
            !ks.GetMap(ChannelBonusInfo, static s => (s.Get(out int k), k),
                static s => { var b = new KChannelBonusInfo(); return (b.Get(s, s.Region), b); }))
            return false;
        ServerGroupId = gid;
        return true;
    }
}

public sealed class KEGS_CHECK_CHANNEL_CHANGE_REQ : IKSerializable
{
    public int ChannelId { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(ChannelId);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int cid)) return false;
        ChannelId = cid;
        return true;
    }
}

public sealed class KEGS_CLIENT_QUIT_REQ : IKSerializable
{
    public int Reason { get; set; }
    public List<KErrorMsgInfo> ErrorMsgInfoList { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Reason) && ks.PutList(ErrorMsgInfoList);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int r) || !ks.GetList(ErrorMsgInfoList)) return false;
        Reason = r;
        return true;
    }
}

public sealed class KEGS_BAN_USER_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(UnitUid);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid)) return false;
        UnitUid = uid;
        return true;
    }
}

public sealed class KEGS_GET_TEMP_ITEM_REQ : IKSerializable
{
    public int TempItemUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(TempItemUid);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int uid)) return false;
        TempItemUid = uid;
        return true;
    }
}

public sealed class KEGS_GET_TEMP_ITEM_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int ItemId { get; set; }
    public int TempItemUid { get; set; }
    public List<KInventoryItemInfo> InventorySlotInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(ItemId) && ks.Put(TempItemUid) && ks.PutList(InventorySlotInfo);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out int itemId) || !ks.Get(out int tUid) ||
            !ks.GetList(InventorySlotInfo))
            return false;
        Ok = ok; ItemId = itemId; TempItemUid = tUid;
        return true;
    }
}

public sealed class KEGS_DELETE_TEMP_ITEM_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int TempItemUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.Put(TempItemUid);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out int tUid)) return false;
        Ok = ok; TempItemUid = tUid;
        return true;
    }
}

public sealed class KEGS_CHECK_EXIST_MONSTER_UID_REQ : IKSerializable
{
    public List<int> NpcUid { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region) => ks.PutList(NpcUid, static (s, v) => s.Put(v));
    public bool Get(KSerializer ks, RegionProfile region) => ks.GetList(NpcUid, static s => (s.Get(out int v), v));
}

public sealed class KEGS_CHECK_EXIST_MONSTER_UID_ACK : IKSerializable
{
    public int Ok { get; set; }
    public List<int> NotExistNpcUid { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.PutList(NotExistNpcUid, static (s, v) => s.Put(v));

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.GetList(NotExistNpcUid, static s => (s.Get(out int v), v))) return false;
        Ok = ok;
        return true;
    }
}

public sealed class KEGS_DUNGEON_PLAY_INFO_TO_SERVER_NOT : IKSerializable
{
    public int ComboScore { get; set; }
    public int TechScore { get; set; }
    public int DamageCount { get; set; }
    public int TotalAttackedDamage { get; set; }
    public int TotalRecoveryHp { get; set; }
    public int UsingKindOfSkill { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(ComboScore) && ks.Put(TechScore) && ks.Put(DamageCount) &&
        ks.Put(TotalAttackedDamage) && ks.Put(TotalRecoveryHp) && ks.Put(UsingKindOfSkill);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int cs) || !ks.Get(out int ts) || !ks.Get(out int dc) ||
            !ks.Get(out int tad) || !ks.Get(out int trh) || !ks.Get(out int uks))
            return false;
        ComboScore = cs; TechScore = ts; DamageCount = dc;
        TotalAttackedDamage = tad; TotalRecoveryHp = trh; UsingKindOfSkill = uks;
        return true;
    }
}

public sealed class KEGS_DUNGEON_SECRET_STAGE_ENTER_CHECK_ACK : IKSerializable
{
    public int Ok { get; set; }
    public bool CanEnterSecretStage { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.Put(CanEnterSecretStage);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out bool can)) return false;
        Ok = ok; CanEnterSecretStage = can;
        return true;
    }
}

public sealed class KEGS_SECRET_STAGE_LOAD_REQ : IKSerializable
{
    public int PadId { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(PadId);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int pid)) return false;
        PadId = pid;
        return true;
    }
}

public sealed class KEGS_DUNGEON_SECRET_STAGE_ENTER_CHECK_NOT : IKSerializable
{
    public int StartSecretStageEnteringEvent { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(StartSecretStageEnteringEvent);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ev)) return false;
        StartSecretStageEnteringEvent = ev;
        return true;
    }
}

public sealed class KEGS_FORCED_EXIT_VOTE_REQ : IKSerializable
{
    public long UserUid { get; set; }
    public bool WantForceExit { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(UserUid) && ks.Put(WantForceExit);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.Get(out bool want)) return false;
        UserUid = uid; WantForceExit = want;
        return true;
    }
}

public sealed class KEGS_FORCED_EXIT_VOTE_ACK : IKSerializable
{
    public long UserUid { get; set; }
    public int Ok { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(UserUid) && ks.Put(Ok);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.Get(out int ok)) return false;
        UserUid = uid; Ok = ok;
        return true;
    }
}

public sealed class KEGS_CREATE_TC_ROOM_REQ : IKSerializable
{
    public int TcId { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(TcId);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int tcId)) return false;
        TcId = tcId;
        return true;
    }
}

public sealed class KEGS_CREATE_TC_ROOM_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KTCInfo TcInfo { get; set; } = new();
    public KRoomUserInfo RoomUserInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(TcInfo) && ks.Put(RoomUserInfo);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(TcInfo) || !ks.Get(RoomUserInfo)) return false;
        Ok = ok;
        return true;
    }
}

public sealed class KEGS_END_TC_GAME_REQ : IKSerializable
{
    public bool IsSuccess { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(IsSuccess);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool s)) return false;
        IsSuccess = s;
        return true;
    }
}

public sealed class KEGS_END_TC_GAME_ACK : IKSerializable
{
    public int Ok { get; set; }
    public bool IsSuccess { get; set; }
    public KUnitInfo UnitInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(IsSuccess) && ks.Put(UnitInfo);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out bool s) || !ks.Get(UnitInfo)) return false;
        Ok = ok; IsSuccess = s;
        return true;
    }
}

public sealed class KEGS_QUICK_START_DUNGEON_GAME_REQ : IKSerializable
{
    public int DungeonId { get; set; }
    public sbyte DifficultyLevel { get; set; }
    public sbyte GetItemType { get; set; }
    public sbyte DungeonMode { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(DungeonId) && ks.Put(DifficultyLevel) && ks.Put(GetItemType) && ks.Put(DungeonMode);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int did) || !ks.Get(out sbyte diff) || !ks.Get(out sbyte git) || !ks.Get(out sbyte mode))
            return false;
        DungeonId = did; DifficultyLevel = diff; GetItemType = git; DungeonMode = mode;
        return true;
    }
}

public sealed class KEGS_QUICK_START_DUNGEON_GAME_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long FailUnitUid { get; set; }
    public string FailUserNickName { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(FailUnitUid) && ks.PutWideString(FailUserNickName);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out long fuid) || !ks.GetWideString(out string fnick)) return false;
        Ok = ok; FailUnitUid = fuid; FailUserNickName = fnick;
        return true;
    }
}

public sealed class KEGS_PARTY_CHANGE_HOST_NOT : IKSerializable
{
    public long BeforeUid { get; set; }
    public long AfterUid { get; set; }
    public bool RegroupParty { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(BeforeUid) && ks.Put(AfterUid) && ks.Put(RegroupParty);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long b) || !ks.Get(out long a) || !ks.Get(out bool r)) return false;
        BeforeUid = b; AfterUid = a; RegroupParty = r;
        return true;
    }
}

public sealed class KEGS_PARTY_GAME_START_FAILED_NOT : IKSerializable
{
    public int Reason { get; set; }
    public long FailUnitUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Reason) && ks.Put(FailUnitUid);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int r) || !ks.Get(out long uid)) return false;
        Reason = r; FailUnitUid = uid;
        return true;
    }
}

public sealed class KEGS_PARTY_GAME_START_OPINION_CHECK_REPLY_NOT : IKSerializable
{
    public bool Accept { get; set; }
    public int Reason { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Accept) && ks.Put(Reason);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool a) || !ks.Get(out int r)) return false;
        Accept = a; Reason = r;
        return true;
    }
}

public sealed class KEGS_CHANGE_MAP_REQ : IKSerializable
{
    public short WorldId { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(WorldId);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out short wid)) return false;
        WorldId = wid;
        return true;
    }
}

public sealed class KEGS_CHANGE_MAP_NOT : IKSerializable
{
    public short WorldId { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(WorldId);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out short wid)) return false;
        WorldId = wid;
        return true;
    }
}

public sealed class KEGS_CHANGE_PVP_ROOM_PUBLIC_REQ : IKSerializable
{
    public bool Public { get; set; }
    public string Password { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Public) && ks.PutWideString(Password);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool pub) || !ks.GetWideString(out string pw)) return false;
        Public = pub; Password = pw;
        return true;
    }
}

public sealed class KEGS_CHANGE_PVP_ROOM_PUBLIC_ACK : IKSerializable
{
    public bool Public { get; set; }
    public int Ok { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Public) && ks.Put(Ok);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool pub) || !ks.Get(out int ok)) return false;
        Public = pub; Ok = ok;
        return true;
    }
}

public sealed class KEGS_CHANGE_PVP_ROOM_PUBLIC_NOT : IKSerializable
{
    public bool Public { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Public);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool pub)) return false;
        Public = pub;
        return true;
    }
}

public sealed class KEGS_CHANGE_PVP_ROOM_NAME_REQ : IKSerializable
{
    public string RoomName { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region) => ks.PutWideString(RoomName);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string rn)) return false;
        RoomName = rn;
        return true;
    }
}

public sealed class KEGS_CHANGE_PVP_ROOM_NAME_ACK : IKSerializable
{
    public string RoomName { get; set; } = string.Empty;
    public int Ok { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.PutWideString(RoomName) && ks.Put(Ok);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string rn) || !ks.Get(out int ok)) return false;
        RoomName = rn; Ok = ok;
        return true;
    }
}

public sealed class KEGS_CHANGE_PVP_ROOM_NAME_NOT : IKSerializable
{
    public string RoomName { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region) => ks.PutWideString(RoomName);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string rn)) return false;
        RoomName = rn;
        return true;
    }
}

public sealed class KEGS_CHANGE_PLAY_TIME_LIMIT_REQ : IKSerializable
{
    public float PlayTime { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(PlayTime);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out float pt)) return false;
        PlayTime = pt;
        return true;
    }
}

public sealed class KEGS_CHANGE_PLAY_TIME_LIMIT_NOT : IKSerializable
{
    public float PlayTime { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(PlayTime);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out float pt)) return false;
        PlayTime = pt;
        return true;
    }
}

public sealed class KEGS_CHANGE_PITIN_REQ : IKSerializable
{
    public bool PitIn { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(PitIn);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool p)) return false;
        PitIn = p;
        return true;
    }
}

public sealed class KEGS_CHANGE_PITIN_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public bool PitIn { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(UnitUid) && ks.Put(PitIn);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.Get(out bool p)) return false;
        UnitUid = uid; PitIn = p;
        return true;
    }
}

public sealed class KEGS_CHANGE_MOTION_REQ : IKSerializable
{
    public sbyte MotionId { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(MotionId);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte mid)) return false;
        MotionId = mid;
        return true;
    }
}

public sealed class KEGS_CHANGE_MOTION_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public sbyte MotionId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(UnitUid) && ks.Put(MotionId);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.Get(out sbyte mid)) return false;
        UnitUid = uid; MotionId = mid;
        return true;
    }
}

// -------- Pet / Riding Pet / 2nd Security / Inventory & Bank Packets --------

public sealed class KPacketString : IKSerializable
{
    public string Value { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region) => ks.PutWideString(Value);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string s)) return false;
        Value = s;
        return true;
    }
}

public sealed class KEGS_GET_PET_LIST_REQ : IKSerializable
{
    public int ViewPage { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(ViewPage);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int vp)) return false;
        ViewPage = vp;
        return true;
    }
}

public sealed class KEGS_GET_PET_LIST_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int TotalPage { get; set; }
    public int ViewPage { get; set; }
    public List<KPetInfo> PetList { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(TotalPage) && ks.Put(ViewPage) && ks.PutList(PetList);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out int tp) || !ks.Get(out int vp) || !ks.GetList(PetList)) return false;
        Ok = ok; TotalPage = tp; ViewPage = vp;
        return true;
    }
}

public sealed class KEGS_COMMANDS_FOR_PETS_REQ : IKSerializable
{
    public sbyte ActionType { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(ActionType);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte at)) return false;
        ActionType = at;
        return true;
    }
}

public sealed class KEGS_CHANGE_PET_NAME_REQ : IKSerializable
{
    public long ItemUid { get; set; }
    public string PetName { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(ItemUid) && ks.PutWideString(PetName);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long iuid) || !ks.GetWideString(out string name)) return false;
        ItemUid = iuid; PetName = name;
        return true;
    }
}

public sealed class KEGS_CHANGE_PET_NAME_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KPetInfo ChangedPetInfo { get; set; } = new();
    public List<KInventoryItemInfo> InventorySlotInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(ChangedPetInfo) && ks.PutList(InventorySlotInfo);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(ChangedPetInfo) || !ks.GetList(InventorySlotInfo)) return false;
        Ok = ok;
        return true;
    }
}

public sealed class KEGS_SET_AUTO_FEED_PETS_REQ : IKSerializable
{
    public bool AutoFeed { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(AutoFeed);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool af)) return false;
        AutoFeed = af;
        return true;
    }
}

public sealed class KEGS_SET_AUTO_FEED_PETS_ACK : IKSerializable
{
    public int Ok { get; set; }
    public bool AutoFeed { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.Put(AutoFeed);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out bool af)) return false;
        Ok = ok; AutoFeed = af;
        return true;
    }
}

public sealed class KEGS_CREATE_RIDING_PET_REQ : IKSerializable
{
    public long ItemUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(ItemUid);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long iuid)) return false;
        ItemUid = iuid;
        return true;
    }
}

public sealed class KEGS_CREATE_RIDING_PET_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KRidingPetInfo CreatedPetInfo { get; set; } = new();
    public List<KInventoryItemInfo> InventorySlotInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(CreatedPetInfo) && ks.PutList(InventorySlotInfo);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(CreatedPetInfo) || !ks.GetList(InventorySlotInfo)) return false;
        Ok = ok;
        return true;
    }
}

public sealed class KEGS_GET_RIDING_PET_LIST_REQ : IKSerializable
{
    public byte ViewPage { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(ViewPage);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out byte vp)) return false;
        ViewPage = vp;
        return true;
    }
}

public sealed class KEGS_GET_RIDING_PET_LIST_ACK : IKSerializable
{
    public int Ok { get; set; }
    public List<KRidingPetInfo> RidingPetList { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.PutList(RidingPetList);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.GetList(RidingPetList)) return false;
        Ok = ok;
        return true;
    }
}

public sealed class KEGS_RELEASE_RIDING_PET_REQ : IKSerializable
{
    public long RidingPetUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(RidingPetUid);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid)) return false;
        RidingPetUid = uid;
        return true;
    }
}

public sealed class KEGS_RELEASE_RIDING_PET_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long RidingPetUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.Put(RidingPetUid);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out long uid)) return false;
        Ok = ok; RidingPetUid = uid;
        return true;
    }
}

public sealed class KEGS_UNSUMMON_RIDING_PET_REQ : IKSerializable
{
    public long RidingPetUid { get; set; }
    public float Stamina { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(RidingPetUid) && ks.Put(Stamina);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.Get(out float st)) return false;
        RidingPetUid = uid; Stamina = st;
        return true;
    }
}

public sealed class KEGS_UNSUMMON_RIDING_PET_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long RidingPetUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.Put(RidingPetUid);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out long uid)) return false;
        Ok = ok; RidingPetUid = uid;
        return true;
    }
}

public sealed class KEGS_UPDATE_RIDING_PET_NOT : IKSerializable
{
    public long RidingPetUid { get; set; }
    public float Stamina { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(RidingPetUid) && ks.Put(Stamina);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.Get(out float st)) return false;
        RidingPetUid = uid; Stamina = st;
        return true;
    }
}

public sealed class KEGS_CHANGE_SECOND_SECURITY_PW_REQ : IKSerializable
{
    public string OldSecondPw { get; set; } = string.Empty;
    public string NewSecondPw { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region) => ks.PutWideString(OldSecondPw) && ks.PutWideString(NewSecondPw);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string o) || !ks.GetWideString(out string n)) return false;
        OldSecondPw = o; NewSecondPw = n;
        return true;
    }
}

public sealed class KEGS_DELETE_SECOND_SECURITY_VERIFY_REQ : IKSerializable
{
    public string SecondPw { get; set; } = string.Empty;
    public bool Delete { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.PutWideString(SecondPw) && ks.Put(Delete);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string pw) || !ks.Get(out bool del)) return false;
        SecondPw = pw; Delete = del;
        return true;
    }
}

public sealed class KEGS_CHECK_MACHINE_ID_REQ : IKSerializable
{
    public string MachineId { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region) => ks.PutAsciiString(MachineId);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetAsciiString(out string mid)) return false;
        MachineId = mid;
        return true;
    }
}

public sealed class KEGS_CHECK_MACHINE_ID_ACK : IKSerializable
{
    public int Ok { get; set; }
    public string MachineId { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.PutAsciiString(MachineId);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.GetAsciiString(out string mid)) return false;
        Ok = ok; MachineId = mid;
        return true;
    }
}

public sealed class KEGS_GET_CLIENT_SCRIPT_REQ : IKSerializable
{
    public string ScriptName { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region) => ks.PutWideString(ScriptName);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string sn)) return false;
        ScriptName = sn;
        return true;
    }
}

public sealed class KEGS_GET_CLIENT_SCRIPT_ACK : IKSerializable
{
    public int Ok { get; set; }
    public string ScriptString { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.PutWideString(ScriptString);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.GetWideString(out string ss)) return false;
        Ok = ok; ScriptString = ss;
        return true;
    }
}

public sealed class KEGS_USE_QUICK_SLOT_REQ : IKSerializable
{
    public short SlotId { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(SlotId);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out short sid)) return false;
        SlotId = sid;
        return true;
    }
}

public sealed class KEGS_USE_QUICK_SLOT_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KInventoryItemInfo InventorySlotInfo { get; set; } = new();
    public int ItemId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(InventorySlotInfo) && ks.Put(ItemId);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(InventorySlotInfo) || !ks.Get(out int iid)) return false;
        Ok = ok; ItemId = iid;
        return true;
    }
}

public sealed class KEGS_USE_QUICK_SLOT_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public int ItemId { get; set; }
    public bool UseItemNeedPayment { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(UnitUid) && ks.Put(ItemId) && ks.Put(UseItemNeedPayment);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.Get(out int iid) || !ks.Get(out bool pay)) return false;
        UnitUid = uid; ItemId = iid; UseItemNeedPayment = pay;
        return true;
    }
}

public sealed class KEGS_USE_ITEM_IN_INVENTORY_REQ : IKSerializable
{
    public long ItemUid { get; set; }
    public int TempCode { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(ItemUid) && ks.Put(TempCode);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long iuid) || !ks.Get(out int tc)) return false;
        ItemUid = iuid; TempCode = tc;
        return true;
    }
}

public sealed class KEGS_USE_ITEM_IN_INVENTORY_ACK : IKSerializable
{
    public int Ok { get; set; }
    public List<KInventoryItemInfo> InventorySlotInfo { get; set; } = new();
    public int UsedItemId { get; set; }
    public int WarpPointMapId { get; set; }
    public int Ed { get; set; }
    public int TempCode { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.PutList(InventorySlotInfo) && ks.Put(UsedItemId) &&
        ks.Put(WarpPointMapId) && ks.Put(Ed) && ks.Put(TempCode);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.GetList(InventorySlotInfo) || !ks.Get(out int uiid) ||
            !ks.Get(out int wpm) || !ks.Get(out int ed) || !ks.Get(out int tc)) return false;
        Ok = ok; UsedItemId = uiid; WarpPointMapId = wpm; Ed = ed; TempCode = tc;
        return true;
    }
}

public sealed class KEGS_OPEN_RANDOM_ITEM_REQ : IKSerializable
{
    public long ItemUid { get; set; }
    public bool DelayedOk { get; set; }
    public bool NotifyGetItem { get; set; } = true;

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(ItemUid) && ks.Put(DelayedOk) && ks.Put(NotifyGetItem);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long iuid) || !ks.Get(out bool dok) || !ks.Get(out bool ngi)) return false;
        ItemUid = iuid; DelayedOk = dok; NotifyGetItem = ngi;
        return true;
    }
}

public sealed class KEGS_OPEN_RANDOM_ITEM_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int RessurectionCount { get; set; }
    public int RestoreSpirit { get; set; }
    public Dictionary<int, int> InsertedItem { get; set; } = new();
    public List<KInventoryItemInfo> InventorySlotInfo { get; set; } = new();
    public int Ed { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(RessurectionCount) && ks.Put(RestoreSpirit) &&
        ks.PutMap(InsertedItem, static (s, k) => s.Put(k), static (s, v) => s.Put(v)) &&
        ks.PutList(InventorySlotInfo) && ks.Put(Ed);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out int rc) || !ks.Get(out int rs) ||
            !ks.GetMap(InsertedItem, static s => (s.Get(out int k), k), static s => (s.Get(out int v), v)) ||
            !ks.GetList(InventorySlotInfo) || !ks.Get(out int ed))
            return false;
        Ok = ok; RessurectionCount = rc; RestoreSpirit = rs; Ed = ed;
        return true;
    }
}

public sealed class KEGS_IDENTIFY_ITEM_REQ : IKSerializable
{
    public long ItemUid { get; set; }
    public int Quantity { get; set; } = 1;

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(ItemUid) && ks.Put(Quantity);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long iuid) || !ks.Get(out int q)) return false;
        ItemUid = iuid; Quantity = q;
        return true;
    }
}

public sealed class KEGS_IDENTIFY_ITEM_ACK : IKSerializable
{
    public int Ok { get; set; }
    public Dictionary<int, int> InsertedItem { get; set; } = new();
    public List<KInventoryItemInfo> InventorySlotInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) &&
        ks.PutMap(InsertedItem, static (s, k) => s.Put(k), static (s, v) => s.Put(v)) &&
        ks.PutList(InventorySlotInfo);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) ||
            !ks.GetMap(InsertedItem, static s => (s.Get(out int k), k), static s => (s.Get(out int v), v)) ||
            !ks.GetList(InventorySlotInfo))
            return false;
        Ok = ok;
        return true;
    }
}

public sealed class KEGS_SEAL_ITEM_REQ : IKSerializable
{
    public long DestItemUid { get; set; }
    public long SealItemUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(DestItemUid) && ks.Put(SealItemUid);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long d) || !ks.Get(out long s)) return false;
        DestItemUid = d; SealItemUid = s;
        return true;
    }
}

public sealed class KEGS_SEAL_ITEM_ACK : IKSerializable
{
    public int Ok { get; set; }
    public List<KInventoryItemInfo> InventorySlotInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.PutList(InventorySlotInfo);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.GetList(InventorySlotInfo)) return false;
        Ok = ok;
        return true;
    }
}

public sealed class KEGS_UNSEAL_ITEM_REQ : IKSerializable
{
    public long DestItemUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(DestItemUid);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long d)) return false;
        DestItemUid = d;
        return true;
    }
}

public sealed class KEGS_ATTRIB_ATTACH_ITEM_REQ : IKSerializable
{
    public long DestItemUid { get; set; }
    public long AttachItemUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(DestItemUid) && ks.Put(AttachItemUid);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long d) || !ks.Get(out long a)) return false;
        DestItemUid = d; AttachItemUid = a;
        return true;
    }
}

public sealed class KEGS_ATTRIB_ATTACH_ITEM_ACK : IKSerializable
{
    public int Ok { get; set; }
    public List<KInventoryItemInfo> InventorySlotInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.PutList(InventorySlotInfo);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.GetList(InventorySlotInfo)) return false;
        Ok = ok;
        return true;
    }
}

public sealed class KEGS_ENCHANT_ATTACH_ITEM_REQ : IKSerializable
{
    public long DestItemUid { get; set; }
    public long AttachItemUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(DestItemUid) && ks.Put(AttachItemUid);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long d) || !ks.Get(out long a)) return false;
        DestItemUid = d; AttachItemUid = a;
        return true;
    }
}

public sealed class KEGS_ENCHANT_ATTACH_ITEM_ACK : IKSerializable
{
    public int Ok { get; set; }
    public List<KInventoryItemInfo> InventorySlotInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.PutList(InventorySlotInfo);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.GetList(InventorySlotInfo)) return false;
        Ok = ok;
        return true;
    }
}

public sealed class KEGS_SOCKET_EXPAND_ITEM_REQ : IKSerializable
{
    public long MaterialItemUid { get; set; }
    public long TargetItemUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(MaterialItemUid) && ks.Put(TargetItemUid);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long m) || !ks.Get(out long t)) return false;
        MaterialItemUid = m; TargetItemUid = t;
        return true;
    }
}

public sealed class KEGS_SOCKET_EXPAND_ITEM_ACK : IKSerializable
{
    public int Ok { get; set; }
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.PutList(UpdatedInventorySlot);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.GetList(UpdatedInventorySlot)) return false;
        Ok = ok;
        return true;
    }
}

public sealed class KEGS_SORT_CATEGORY_ITEM_REQ : IKSerializable
{
    public int CategoryType { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(CategoryType);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ct)) return false;
        CategoryType = ct;
        return true;
    }
}

public sealed class KEGS_SORT_CATEGORY_ITEM_ACK : IKSerializable
{
    public int CategoryType { get; set; }
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();
    public int Ok { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(CategoryType) && ks.PutList(UpdatedInventorySlot) && ks.Put(Ok);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ct) || !ks.GetList(UpdatedInventorySlot) || !ks.Get(out int ok)) return false;
        CategoryType = ct; Ok = ok;
        return true;
    }
}

public sealed class KEGS_ITEM_EXCHANGE_REQ : IKSerializable
{
    public int HouseId { get; set; }
    public long SourceItemUid { get; set; }
    public int SourceItemId { get; set; }
    public int SourceQuantity { get; set; } = 1;
    public int DestItemId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(HouseId) && ks.Put(SourceItemUid) && ks.Put(SourceItemId) &&
        ks.Put(SourceQuantity) && ks.Put(DestItemId);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int hid) || !ks.Get(out long suid) || !ks.Get(out int sid) ||
            !ks.Get(out int sq) || !ks.Get(out int did))
            return false;
        HouseId = hid; SourceItemUid = suid; SourceItemId = sid; SourceQuantity = sq; DestItemId = did;
        return true;
    }
}

public sealed class KEGS_ITEM_EXCHANGE_ACK : IKSerializable
{
    public int Ok { get; set; }
    public List<KInventoryItemInfo> InventorySlotInfo { get; set; } = new();
    public Dictionary<int, int> ResultItem { get; set; } = new();
    public int ExchangeCount { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.PutList(InventorySlotInfo) &&
        ks.PutMap(ResultItem, static (s, k) => s.Put(k), static (s, v) => s.Put(v)) &&
        ks.Put(ExchangeCount);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.GetList(InventorySlotInfo) ||
            !ks.GetMap(ResultItem, static s => (s.Get(out int k), k), static s => (s.Get(out int v), v)) ||
            !ks.Get(out int ec))
            return false;
        Ok = ok; ExchangeCount = ec;
        return true;
    }
}

public sealed class KEGS_GET_MY_BANK_INFO_ACK : IKSerializable
{
    public Dictionary<long, KInventoryItemInfo> MapItem { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.PutMap(MapItem, static (s, k) => s.Put(k), static (s, v) => v.Put(s, s.Region));

    public bool Get(KSerializer ks, RegionProfile region) =>
        ks.GetMap(MapItem, static s => (s.Get(out long k), k),
            static s => { var v = new KInventoryItemInfo(); return (v.Get(s, s.Region), v); });
}

public sealed class KEGS_EXPAND_BANK_SLOT_ED_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int Ed { get; set; }
    public Dictionary<int, int> ExpandedCategorySlot { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(Ed) &&
        ks.PutMap(ExpandedCategorySlot, static (s, k) => s.Put(k), static (s, v) => s.Put(v));

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out int ed) ||
            !ks.GetMap(ExpandedCategorySlot, static s => (s.Get(out int k), k), static s => (s.Get(out int v), v)))
            return false;
        Ok = ok; Ed = ed;
        return true;
    }
}

public sealed class KEGS_GET_NEXT_BANK_ED_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int Ed { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.Put(Ed);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out int ed)) return false;
        Ok = ok; Ed = ed;
        return true;
    }
}

// -------- Personal Shop / Agency / Trade Board & Guild Management Packets --------

public sealed class KTradeBoardItemInfo : IKSerializable
{
    public long PersonalShopUid { get; set; }
    public long HostUnitUid { get; set; }
    public string SellerNickName { get; set; } = string.Empty;
    public KSellPersonalShopItemInfo SellItemInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(PersonalShopUid) && ks.Put(HostUnitUid) && ks.PutWideString(SellerNickName) && ks.Put(SellItemInfo);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long puid) || !ks.Get(out long huid) || !ks.GetWideString(out string nick) ||
            !ks.Get(SellItemInfo))
            return false;
        PersonalShopUid = puid; HostUnitUid = huid; SellerNickName = nick;
        return true;
    }
}

public sealed class KEGS_JOIN_PERSONAL_SHOP_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public sbyte StageKey { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(UnitUid) && ks.Put(StageKey);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.Get(out sbyte sk)) return false;
        UnitUid = uid; StageKey = sk;
        return true;
    }
}

public sealed class KEGS_JOIN_PERSONAL_SHOP_ACK : IKSerializable
{
    public int Ok { get; set; }
    public string PersonalShopName { get; set; } = string.Empty;
    public string NickName { get; set; } = string.Empty;
    public List<KSellPersonalShopItemInfo> SellItemInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.PutWideString(PersonalShopName) && ks.PutWideString(NickName) && ks.PutList(SellItemInfo);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.GetWideString(out string sname) ||
            !ks.GetWideString(out string nick) || !ks.GetList(SellItemInfo))
            return false;
        Ok = ok; PersonalShopName = sname; NickName = nick;
        return true;
    }
}

public sealed class KEGS_JOIN_MY_PSHOP_AGENCY_REQ : IKSerializable
{
    public const sbyte JtShowMyItemList = 0;
    public const sbyte JtForPickUp = 1;

    public sbyte JoinType { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(JoinType);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte jt)) return false;
        JoinType = jt;
        return true;
    }
}

public sealed class KEGS_JOIN_MY_PSHOP_AGENCY_ACK : IKSerializable
{
    public int Ok { get; set; }
    public sbyte JoinType { get; set; }
    public string PersonalShopName { get; set; } = string.Empty;
    public string NickName { get; set; } = string.Empty;
    public List<KSellPersonalShopItemInfo> SellItemInfo { get; set; } = new();
    public int UsedItemId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(JoinType) && ks.PutWideString(PersonalShopName) &&
        ks.PutWideString(NickName) && ks.PutList(SellItemInfo) && ks.Put(UsedItemId);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out sbyte jt) || !ks.GetWideString(out string sname) ||
            !ks.GetWideString(out string nick) || !ks.GetList(SellItemInfo) || !ks.Get(out int uiid))
            return false;
        Ok = ok; JoinType = jt; PersonalShopName = sname; NickName = nick; UsedItemId = uiid;
        return true;
    }
}

public sealed class KEGS_PICK_UP_FROM_PSHOP_AGENCY_REQ : IKSerializable
{
    public List<long> PickUpItemList { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region) => ks.PutList(PickUpItemList, static (s, v) => s.Put(v));
    public bool Get(KSerializer ks, RegionProfile region) => ks.GetList(PickUpItemList, static s => (s.Get(out long v), v));
}

public sealed class KEGS_PICK_UP_FROM_PSHOP_AGENCY_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int Ed { get; set; }
    public List<KSellPersonalShopItemInfo> SellItemInfo { get; set; } = new();
    public List<KInventoryItemInfo> InventorySlotInfo { get; set; } = new();
    public bool RemainSellItem { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(Ed) && ks.PutList(SellItemInfo) &&
        ks.PutList(InventorySlotInfo) && ks.Put(RemainSellItem);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out int ed) || !ks.GetList(SellItemInfo) ||
            !ks.GetList(InventorySlotInfo) || !ks.Get(out bool rem))
            return false;
        Ok = ok; Ed = ed; RemainSellItem = rem;
        return true;
    }
}

public sealed class KEGS_SEARCH_TRADE_BOARD_REQ : IKSerializable
{
    public string Finder { get; set; } = string.Empty;
    public sbyte SlotCategory { get; set; }
    public sbyte EquipPosition { get; set; }
    public sbyte UnitType { get; set; }
    public sbyte UnitClass { get; set; }
    public sbyte MinLevel { get; set; }
    public sbyte MaxLevel { get; set; }
    public List<sbyte> ItemGrade { get; set; } = new();
    public bool StrictSearchOption { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.PutWideString(Finder) && ks.Put(SlotCategory) && ks.Put(EquipPosition) &&
        ks.Put(UnitType) && ks.Put(UnitClass) && ks.Put(MinLevel) && ks.Put(MaxLevel) &&
        ks.PutList(ItemGrade, static (s, v) => s.Put(v)) && ks.Put(StrictSearchOption);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string f) || !ks.Get(out sbyte sc) || !ks.Get(out sbyte ep) ||
            !ks.Get(out sbyte ut) || !ks.Get(out sbyte uc) || !ks.Get(out sbyte minl) || !ks.Get(out sbyte maxl) ||
            !ks.GetList(ItemGrade, static s => (s.Get(out sbyte v), v)) || !ks.Get(out bool sso))
            return false;
        Finder = f; SlotCategory = sc; EquipPosition = ep; UnitType = ut; UnitClass = uc;
        MinLevel = minl; MaxLevel = maxl; StrictSearchOption = sso;
        return true;
    }
}

public sealed class KEGS_SEARCH_TRADE_BOARD_ACK : IKSerializable
{
    public int Ok { get; set; }
    public List<KTradeBoardItemInfo> ItemInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.PutList(ItemInfo);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.GetList(ItemInfo)) return false;
        Ok = ok;
        return true;
    }
}

public sealed class KEGS_GET_GUILD_USER_LIST_REQ : IKSerializable
{
    public uint ViewPage { get; set; }
    public sbyte GuildUserListType { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(ViewPage) && ks.Put(GuildUserListType);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint vp) || !ks.Get(out sbyte t)) return false;
        ViewPage = vp; GuildUserListType = t;
        return true;
    }
}

public sealed class KEGS_GET_GUILD_USER_LIST_ACK : IKSerializable
{
    public int Ok { get; set; }
    public uint TotalPage { get; set; }
    public uint ViewPage { get; set; }
    public sbyte GuildUserListType { get; set; }
    public int LoginedMemberCount { get; set; }
    public List<KGuildMemberInfo> MemberPositionGradeList { get; set; } = new();
    public List<KGuildMemberMessageInfo> MemberMessageList { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(TotalPage) && ks.Put(ViewPage) && ks.Put(GuildUserListType) &&
        ks.Put(LoginedMemberCount) && ks.PutList(MemberPositionGradeList) && ks.PutList(MemberMessageList);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out uint tp) || !ks.Get(out uint vp) || !ks.Get(out sbyte gt) ||
            !ks.Get(out int lmc) || !ks.GetList(MemberPositionGradeList) || !ks.GetList(MemberMessageList))
            return false;
        Ok = ok; TotalPage = tp; ViewPage = vp; GuildUserListType = gt; LoginedMemberCount = lmc;
        return true;
    }
}

public sealed class KEGS_INVITE_GUILD_REQ : IKSerializable
{
    public string NickName { get; set; } = string.Empty;
    public long UnitUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.PutWideString(NickName) && ks.Put(UnitUid);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string n) || !ks.Get(out long uid)) return false;
        NickName = n; UnitUid = uid;
        return true;
    }
}

public sealed class KEGS_INVITE_GUILD_ACK : IKSerializable
{
    public int Ok { get; set; }
    public string ReceiverNickName { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.PutWideString(ReceiverNickName);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.GetWideString(out string n)) return false;
        Ok = ok; ReceiverNickName = n;
        return true;
    }
}

public sealed class KEGS_INVITE_GUILD_NOT : IKSerializable
{
    public KGuildInfo GuildInfo { get; set; } = new();
    public string SenderNickName { get; set; } = string.Empty;
    public bool AutoJoinGuild { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(GuildInfo) && ks.PutWideString(SenderNickName) && ks.Put(AutoJoinGuild);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(GuildInfo) || !ks.GetWideString(out string sn) || !ks.Get(out bool aj)) return false;
        SenderNickName = sn; AutoJoinGuild = aj;
        return true;
    }
}

public sealed class KEGS_INVITE_GUILD_REPLY_REQ : IKSerializable
{
    public int GuildUid { get; set; }
    public string SenderNickName { get; set; } = string.Empty;
    public int Reason { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(GuildUid) && ks.PutWideString(SenderNickName) && ks.Put(Reason);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int guid) || !ks.GetWideString(out string sn) || !ks.Get(out int r)) return false;
        GuildUid = guid; SenderNickName = sn; Reason = r;
        return true;
    }
}

public sealed class KEGS_INVITE_GUILD_REPLY_DENY_NOT : IKSerializable
{
    public string NickName { get; set; } = string.Empty;
    public int Reason { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.PutWideString(NickName) && ks.Put(Reason);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string n) || !ks.Get(out int r)) return false;
        NickName = n; Reason = r;
        return true;
    }
}

public sealed class KEGS_CHANGE_GUILD_MESSAGE_REQ : IKSerializable
{
    public string Message { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region) => ks.PutWideString(Message);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string msg)) return false;
        Message = msg;
        return true;
    }
}

public sealed class KEGS_CHANGE_GUILD_MEMBER_MESSAGE_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public string Message { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(UnitUid) && ks.PutWideString(Message);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.GetWideString(out string msg)) return false;
        UnitUid = uid; Message = msg;
        return true;
    }
}

public sealed class KEGS_CHANGE_GUILD_MEMBER_MESSAGE_ACK : IKSerializable
{
    public int Ok { get; set; }
    public bool Delete { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.Put(Delete);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out bool del)) return false;
        Ok = ok; Delete = del;
        return true;
    }
}

public sealed class KEGS_CHANGE_GUILD_NAME_CHECK_REQ : IKSerializable
{
    public string GuildName { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region) => ks.PutWideString(GuildName);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string gn)) return false;
        GuildName = gn;
        return true;
    }
}

public sealed class KEGS_CHANGE_GUILD_NAME_REQ : IKSerializable
{
    public int Ok { get; set; }
    public uint OrderNo { get; set; }
    public uint ProductNo { get; set; }
    public int Quantity { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(OrderNo) && ks.Put(ProductNo) && ks.Put(Quantity);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out uint on) || !ks.Get(out uint pn) || !ks.Get(out int q)) return false;
        Ok = ok; OrderNo = on; ProductNo = pn; Quantity = q;
        return true;
    }
}

public sealed class KEGS_CHANGE_GUILD_NAME_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KGuildInfo GuildInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.Put(GuildInfo);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(GuildInfo)) return false;
        Ok = ok;
        return true;
    }
}

public sealed class KEGS_GET_GUILD_SKILL_REQ : IKSerializable
{
    public int GuildSkillId { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(GuildSkillId);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int id)) return false;
        GuildSkillId = id;
        return true;
    }
}

public sealed class KEGS_RESET_GUILD_SKILL_REQ : IKSerializable
{
    public int GuildSkillId { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(GuildSkillId);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int id)) return false;
        GuildSkillId = id;
        return true;
    }
}

public sealed class KEGS_RESET_GUILD_SKILL_ACK : IKSerializable
{
    public int Ok { get; set; }
    public List<KInventoryItemInfo> InventorySlotInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.PutList(InventorySlotInfo);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.GetList(InventorySlotInfo)) return false;
        Ok = ok;
        return true;
    }
}

public sealed class KEGS_INIT_GUILD_SKILL_REQ : IKSerializable
{
    public long ItemUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(ItemUid);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long iuid)) return false;
        ItemUid = iuid;
        return true;
    }
}

public sealed class KEGS_INIT_GUILD_SKILL_ACK : IKSerializable
{
    public int Ok { get; set; }
    public List<KInventoryItemInfo> InventorySlotInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.PutList(InventorySlotInfo);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.GetList(InventorySlotInfo)) return false;
        Ok = ok;
        return true;
    }
}

public sealed class KEGS_GET_GUILD_SKILL_IN_BOARD_REQ : IKSerializable
{
    public int GuildUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(GuildUid);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int guid)) return false;
        GuildUid = guid;
        return true;
    }
}

public sealed class KEGS_GET_GUILD_SKILL_IN_BOARD_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KGuildSkillInfo GuildSkillInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.Put(GuildSkillInfo);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(GuildSkillInfo)) return false;
        Ok = ok;
        return true;
    }
}

public sealed class KEGS_REGISTRATION_GUILD_AD_REQ : IKSerializable
{
    public short Period { get; set; }
    public string AdMessage { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Period) && ks.PutWideString(AdMessage);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out short p) || !ks.GetWideString(out string msg)) return false;
        Period = p; AdMessage = msg;
        return true;
    }
}

public sealed class KEGS_REGISTRATION_GUILD_AD_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int Ed { get; set; }
    public short Period { get; set; }
    public string AdMessage { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(Ed) && ks.Put(Period) && ks.PutWideString(AdMessage);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out int ed) || !ks.Get(out short p) || !ks.GetWideString(out string msg))
            return false;
        Ok = ok; Ed = ed; Period = p; AdMessage = msg;
        return true;
    }
}

public sealed class KEGS_GET_APPLY_JOIN_GUILD_LIST_REQ : IKSerializable
{
    public uint ViewPage { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(ViewPage);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint vp)) return false;
        ViewPage = vp;
        return true;
    }
}

public sealed class KEGS_GET_APPLY_JOIN_GUILD_LIST_ACK : IKSerializable
{
    public int Ok { get; set; }
    public string GuildName { get; set; } = string.Empty;
    public uint TotalPage { get; set; }
    public uint ViewPage { get; set; }
    public List<KApplyJoinGuildInfo> ApplyJoinGuildList { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.PutWideString(GuildName) && ks.Put(TotalPage) &&
        ks.Put(ViewPage) && ks.PutList(ApplyJoinGuildList);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.GetWideString(out string gn) || !ks.Get(out uint tp) ||
            !ks.Get(out uint vp) || !ks.GetList(ApplyJoinGuildList))
            return false;
        Ok = ok; GuildName = gn; TotalPage = tp; ViewPage = vp;
        return true;
    }
}

public sealed class KEGS_ACCEPT_JOIN_GUILD_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(UnitUid);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid)) return false;
        UnitUid = uid;
        return true;
    }
}

public sealed class KEGS_ACCEPT_JOIN_GUILD_ACK : IKSerializable
{
    public int Ok { get; set; }
    public string ReceiverNickName { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.PutWideString(ReceiverNickName);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.GetWideString(out string n)) return false;
        Ok = ok; ReceiverNickName = n;
        return true;
    }
}

public sealed class KEGS_DELETE_APPLY_JOIN_GUILD_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(UnitUid);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid)) return false;
        UnitUid = uid;
        return true;
    }
}

// -------- Letter / Post BlackList / Love Letter Event --------

public sealed class KEGS_GET_POST_BLACK_LIST_ACK : IKSerializable
{
    public List<string> BlackList { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region) => ks.PutList(BlackList, static (s, v) => s.PutWideString(v));
    public bool Get(KSerializer ks, RegionProfile region) => ks.GetList(BlackList, static s => (s.GetWideString(out string v), v));
}

public sealed class KEGS_NEW_POST_BLACK_LIST_REQ : IKSerializable
{
    public string NickName { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region) => ks.PutWideString(NickName);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string n)) return false;
        NickName = n;
        return true;
    }
}

public sealed class KEGS_NEW_POST_BLACK_LIST_ACK : IKSerializable
{
    public int Ok { get; set; }
    public string NickName { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.PutWideString(NickName);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.GetWideString(out string n)) return false;
        Ok = ok; NickName = n;
        return true;
    }
}

public sealed class KEGS_SEND_LOVE_LETTER_EVENT_REQ : IKSerializable
{
    public string Message { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region) => ks.PutWideString(Message);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string msg)) return false;
        Message = msg;
        return true;
    }
}

public sealed class KEGS_SEND_LOVE_LETTER_EVENT_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int Ed { get; set; }
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(Ed) && ks.PutList(UpdatedInventorySlot);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out int ed) || !ks.GetList(UpdatedInventorySlot)) return false;
        Ok = ok; Ed = ed;
        return true;
    }
}

// -------- Messenger / Friend Groups / KNM --------

public sealed class KEGS_ACCEPT_FRIEND_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(UnitUid);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid)) return false;
        UnitUid = uid;
        return true;
    }
}

public sealed class KEGS_DELETE_FRIEND_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(UnitUid);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid)) return false;
        UnitUid = uid;
        return true;
    }
}

public sealed class KEGS_BLOCK_FRIEND_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(UnitUid);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid)) return false;
        UnitUid = uid;
        return true;
    }
}

public sealed class KEGS_UNBLOCK_FRIEND_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(UnitUid);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid)) return false;
        UnitUid = uid;
        return true;
    }
}

public sealed class KEGS_MOVE_FRIEND_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public sbyte TargetGroupId { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(UnitUid) && ks.Put(TargetGroupId);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.Get(out sbyte gid)) return false;
        UnitUid = uid; TargetGroupId = gid;
        return true;
    }
}

public sealed class KEGS_MOVE_FRIEND_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long UnitUid { get; set; }
    public sbyte TargetGroupId { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.Put(UnitUid) && ks.Put(TargetGroupId);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out long uid) || !ks.Get(out sbyte gid)) return false;
        Ok = ok; UnitUid = uid; TargetGroupId = gid;
        return true;
    }
}

public sealed class KEGS_MAKE_FRIEND_GROUP_REQ : IKSerializable
{
    public string GroupName { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region) => ks.PutWideString(GroupName);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string gn)) return false;
        GroupName = gn;
        return true;
    }
}

public sealed class KEGS_MAKE_FRIEND_GROUP_ACK : IKSerializable
{
    public int Ok { get; set; }
    public sbyte GroupId { get; set; }
    public string GroupName { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.Put(GroupId) && ks.PutWideString(GroupName);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out sbyte gid) || !ks.GetWideString(out string gn)) return false;
        Ok = ok; GroupId = gid; GroupName = gn;
        return true;
    }
}

public sealed class KEGS_RENAME_FRIEND_GROUP_REQ : IKSerializable
{
    public sbyte GroupId { get; set; }
    public string GroupName { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(GroupId) && ks.PutWideString(GroupName);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte gid) || !ks.GetWideString(out string gn)) return false;
        GroupId = gid; GroupName = gn;
        return true;
    }
}

public sealed class KEGS_RENAME_FRIEND_GROUP_ACK : IKSerializable
{
    public int Ok { get; set; }
    public sbyte GroupId { get; set; }
    public string GroupName { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.Put(GroupId) && ks.PutWideString(GroupName);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out sbyte gid) || !ks.GetWideString(out string gn)) return false;
        Ok = ok; GroupId = gid; GroupName = gn;
        return true;
    }
}

public sealed class KEGS_DELETE_FRIEND_GROUP_REQ : IKSerializable
{
    public sbyte GroupId { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(GroupId);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte gid)) return false;
        GroupId = gid;
        return true;
    }
}

public sealed class KEGS_DELETE_FRIEND_GROUP_ACK : IKSerializable
{
    public int Ok { get; set; }
    public sbyte GroupId { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.Put(GroupId);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out sbyte gid)) return false;
        Ok = ok; GroupId = gid;
        return true;
    }
}

public sealed class KEGS_FRIEND_POSITION_UPDATE_NOT : IKSerializable
{
    public Dictionary<long, sbyte> FriendPosition { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.PutMap(FriendPosition, static (s, k) => s.Put(k), static (s, v) => s.Put(v));
    public bool Get(KSerializer ks, RegionProfile region) =>
        ks.GetMap(FriendPosition, static s => (s.Get(out long k), k), static s => (s.Get(out sbyte v), v));
}

public sealed class KEGS_GET_MY_MESSENGER_SN_ACK : IKSerializable
{
    public int Ok { get; set; }
    public uint KnmSerialNum { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.Put(KnmSerialNum);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out uint sn)) return false;
        Ok = ok; KnmSerialNum = sn;
        return true;
    }
}

public sealed class KEGS_KNM_INVITE_NOT : IKSerializable
{
    public string NickName { get; set; } = string.Empty;
    public KNMUnitInfo UnitInfo { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region) => ks.PutWideString(NickName) && ks.Put(UnitInfo);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string n) || !ks.Get(UnitInfo)) return false;
        NickName = n;
        return true;
    }
}

public sealed class KEGS_KNM_REQUEST_NEW_FRIEND_INFO_REQ : IKSerializable
{
    public string UnitNickName { get; set; } = string.Empty;
    public long UnitUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.PutWideString(UnitNickName) && ks.Put(UnitUid);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string n) || !ks.Get(out long uid)) return false;
        UnitNickName = n; UnitUid = uid;
        return true;
    }
}

public sealed class KEGS_KNM_REQUEST_NEW_FRIEND_INFO_ACK : IKSerializable
{
    public int Ok { get; set; }
    public uint KnmSerialNum { get; set; }
    public string UnitNickName { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.Put(KnmSerialNum) && ks.PutWideString(UnitNickName);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out uint sn) || !ks.GetWideString(out string n)) return false;
        Ok = ok; KnmSerialNum = sn; UnitNickName = n;
        return true;
    }
}

public sealed class KEGS_KNM_TOGETHER_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KNMUnitInfo UnitInfo { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.Put(UnitInfo);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(UnitInfo)) return false;
        Ok = ok;
        return true;
    }
}

// -------- Relationship / Wedding / Call My Lover --------

public sealed class KEGS_CHECK_NICK_NAME_REQ : IKSerializable
{
    public string NickName { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region) => ks.PutWideString(NickName);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string n)) return false;
        NickName = n;
        return true;
    }
}

public sealed class KEGS_CHECK_NICK_NAME_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long UnitUid { get; set; }
    public string NickName { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.Put(UnitUid) && ks.PutWideString(NickName);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out long uid) || !ks.GetWideString(out string n)) return false;
        Ok = ok; UnitUid = uid; NickName = n;
        return true;
    }
}

public sealed class KEGS_WEDDING_PROPOSE_REQ : IKSerializable
{
    public sbyte WeddingHallType { get; set; }
    public sbyte OfficiantNpc { get; set; }
    public long WeddingDate { get; set; }
    public string WeddingMsg { get; set; } = string.Empty;
    public List<long> InviteUnitList { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(WeddingHallType) && ks.Put(OfficiantNpc) && ks.Put(WeddingDate) &&
        ks.PutWideString(WeddingMsg) && ks.PutList(InviteUnitList, static (s, v) => s.Put(v));

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte wht) || !ks.Get(out sbyte npc) || !ks.Get(out long wd) ||
            !ks.GetWideString(out string msg) || !ks.GetList(InviteUnitList, static s => (s.Get(out long v), v)))
            return false;
        WeddingHallType = wht; OfficiantNpc = npc; WeddingDate = wd; WeddingMsg = msg;
        return true;
    }
}

public sealed class KEGS_WEDDING_PROPOSE_ACK : IKSerializable
{
    public int Ok { get; set; }
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.PutList(UpdatedInventorySlot);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.GetList(UpdatedInventorySlot)) return false;
        Ok = ok;
        return true;
    }
}

public sealed class KEGS_WEDDING_ITEM_INFO_REQ : IKSerializable
{
    public List<long> WeddingItemList { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region) => ks.PutList(WeddingItemList, static (s, v) => s.Put(v));
    public bool Get(KSerializer ks, RegionProfile region) => ks.GetList(WeddingItemList, static s => (s.Get(out long v), v));
}

public sealed class KEGS_WEDDING_ITEM_INFO_ACK : IKSerializable
{
    public int Ok { get; set; }
    public Dictionary<long, KWeddingItemInfo> WeddingItemInfo { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) &&
        ks.PutMap(WeddingItemInfo, static (s, k) => s.Put(k), static (s, v) => s.Put(v)) &&
        ks.PutList(UpdatedInventorySlot);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) ||
            !ks.GetMap(WeddingItemInfo, static s => (s.Get(out long k), k), static s => { var v = new KWeddingItemInfo(); return (s.Get(v), v); }) ||
            !ks.GetList(UpdatedInventorySlot))
            return false;
        Ok = ok;
        return true;
    }
}

public sealed class KEGS_CHECK_BREAK_UP_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int Cost { get; set; }
    public sbyte RelationshipType { get; set; }
    public string LoverNickName { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(Cost) && ks.Put(RelationshipType) && ks.PutWideString(LoverNickName);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out int c) || !ks.Get(out sbyte rt) || !ks.GetWideString(out string ln))
            return false;
        Ok = ok; Cost = c; RelationshipType = rt; LoverNickName = ln;
        return true;
    }
}

public sealed class KEGS_START_WEDDING_ASK_ACK : IKSerializable
{
    public bool Agree { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Agree);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool a)) return false;
        Agree = a;
        return true;
    }
}

public sealed class KEGS_START_WEDDING_NOT : IKSerializable
{
    public int Ok { get; set; }
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.PutList(UpdatedInventorySlot);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.GetList(UpdatedInventorySlot)) return false;
        Ok = ok;
        return true;
    }
}

public sealed class KEGS_CHANGE_LOVE_WORD_REQ : IKSerializable
{
    public long ItemUid { get; set; }
    public string NewLoveWord { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(ItemUid) && ks.PutWideString(NewLoveWord);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.GetWideString(out string lw)) return false;
        ItemUid = uid; NewLoveWord = lw;
        return true;
    }
}

public sealed class KEGS_CHANGE_LOVE_WORD_NOT : IKSerializable
{
    public long UseItemUnitUid { get; set; }
    public string NewLoveWord { get; set; } = string.Empty;
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(UseItemUnitUid) && ks.PutWideString(NewLoveWord) && ks.PutList(UpdatedInventorySlot);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.GetWideString(out string lw) || !ks.GetList(UpdatedInventorySlot))
            return false;
        UseItemUnitUid = uid; NewLoveWord = lw;
        return true;
    }
}

public sealed class KEGS_JOIN_WEDDING_HALL_REQ : IKSerializable
{
    public long ItemUid { get; set; }
    public int WeddingUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(ItemUid) && ks.Put(WeddingUid);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long iuid) || !ks.Get(out int wuid)) return false;
        ItemUid = iuid; WeddingUid = wuid;
        return true;
    }
}

public sealed class KEGS_JOIN_WEDDING_HALL_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KWeddingHallInfo WeddingInfo { get; set; } = new();
    public KRoomInfo RoomInfo { get; set; } = new();
    public List<KRoomSlotInfo> Slots { get; set; } = new();
    public string CnIp { get; set; } = string.Empty;
    public string UdpRelayIp { get; set; } = string.Empty;
    public ushort UdpRelayPort { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(WeddingInfo) && ks.Put(RoomInfo) && ks.PutList(Slots) &&
        ks.PutWideString(CnIp) && ks.PutWideString(UdpRelayIp) && ks.Put(UdpRelayPort);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(WeddingInfo) || !ks.Get(RoomInfo) || !ks.GetList(Slots) ||
            !ks.GetWideString(out string cnip) || !ks.GetWideString(out string uip) || !ks.Get(out ushort uport))
            return false;
        Ok = ok; CnIp = cnip; UdpRelayIp = uip; UdpRelayPort = uport;
        return true;
    }
}

public sealed class KEGS_CALL_MY_LOVER_REQ : IKSerializable
{
    public int MapId { get; set; }
    public KVector3 Pos { get; set; } = new();
    public byte LastTouchLineIndex { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(MapId) && ks.Put(Pos) && ks.Put(LastTouchLineIndex);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int mid) || !ks.Get(Pos) || !ks.Get(out byte lti)) return false;
        MapId = mid; LastTouchLineIndex = lti;
        return true;
    }
}

public sealed class KEGS_CALL_MY_LOVER_VILLAGE_NOT : IKSerializable
{
    public int MapId { get; set; }
    public KVector3 Pos { get; set; } = new();
    public byte LastTouchLineIndex { get; set; }
    public long CalledUserUid { get; set; }
    public long CalledGsUid { get; set; }
    public sbyte CallType { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(MapId) && ks.Put(Pos) && ks.Put(LastTouchLineIndex) &&
        ks.Put(CalledUserUid) && ks.Put(CalledGsUid) && ks.Put(CallType);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int mid) || !ks.Get(Pos) || !ks.Get(out byte lti) ||
            !ks.Get(out long cu) || !ks.Get(out long cg) || !ks.Get(out sbyte ct))
            return false;
        MapId = mid; LastTouchLineIndex = lti; CalledUserUid = cu; CalledGsUid = cg; CallType = ct;
        return true;
    }
}

public sealed class KEGS_CALL_MY_LOVER_ROOM_NOT : IKSerializable
{
    public int BattleFieldId { get; set; }
    public long RoomUid { get; set; }
    public KVector3 Pos { get; set; } = new();
    public byte LastTouchLineIndex { get; set; }
    public long CalledUserUid { get; set; }
    public long CalledGsUid { get; set; }
    public sbyte CallType { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(BattleFieldId) && ks.Put(RoomUid) && ks.Put(Pos) && ks.Put(LastTouchLineIndex) &&
        ks.Put(CalledUserUid) && ks.Put(CalledGsUid) && ks.Put(CallType);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int bfid) || !ks.Get(out long ruid) || !ks.Get(Pos) || !ks.Get(out byte lti) ||
            !ks.Get(out long cu) || !ks.Get(out long cg) || !ks.Get(out sbyte ct))
            return false;
        BattleFieldId = bfid; RoomUid = ruid; LastTouchLineIndex = lti; CalledUserUid = cu; CalledGsUid = cg; CallType = ct;
        return true;
    }
}

public sealed class KEGS_CALL_MY_LOVER_AGREE_VILLAGE_NOT : IKSerializable
{
    public bool Agree { get; set; }
    public int MapId { get; set; }
    public KVector3 Pos { get; set; } = new();
    public byte LastTouchLineIndex { get; set; }
    public long CalledUserUid { get; set; }
    public long CalledGsUid { get; set; }
    public sbyte CallType { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Agree) && ks.Put(MapId) && ks.Put(Pos) && ks.Put(LastTouchLineIndex) &&
        ks.Put(CalledUserUid) && ks.Put(CalledGsUid) && ks.Put(CallType);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool ag) || !ks.Get(out int mid) || !ks.Get(Pos) || !ks.Get(out byte lti) ||
            !ks.Get(out long cu) || !ks.Get(out long cg) || !ks.Get(out sbyte ct))
            return false;
        Agree = ag; MapId = mid; LastTouchLineIndex = lti; CalledUserUid = cu; CalledGsUid = cg; CallType = ct;
        return true;
    }
}

public sealed class KEGS_CALL_MY_LOVER_AGREE_ROOM_NOT : IKSerializable
{
    public bool Agree { get; set; }
    public int BattleFieldId { get; set; }
    public long RoomUid { get; set; }
    public KVector3 Pos { get; set; } = new();
    public byte LastTouchLineIndex { get; set; }
    public long CalledUserUid { get; set; }
    public long CalledGsUid { get; set; }
    public sbyte CallType { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Agree) && ks.Put(BattleFieldId) && ks.Put(RoomUid) && ks.Put(Pos) && ks.Put(LastTouchLineIndex) &&
        ks.Put(CalledUserUid) && ks.Put(CalledGsUid) && ks.Put(CallType);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool ag) || !ks.Get(out int bfid) || !ks.Get(out long ruid) || !ks.Get(Pos) ||
            !ks.Get(out byte lti) || !ks.Get(out long cu) || !ks.Get(out long cg) || !ks.Get(out sbyte ct))
            return false;
        Agree = ag; BattleFieldId = bfid; RoomUid = ruid; LastTouchLineIndex = lti; CalledUserUid = cu; CalledGsUid = cg; CallType = ct;
        return true;
    }
}

public sealed class KEGS_CALL_MY_LOVER_STATE_CHANGE_FIELD_NOT : IKSerializable
{
    public int Ok { get; set; }
    public int MapId { get; set; }
    public KVector3 Pos { get; set; } = new();
    public byte LastTouchLineIndex { get; set; }
    public int LimitLevel { get; set; }
    public int RequireDungeonId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(MapId) && ks.Put(Pos) && ks.Put(LastTouchLineIndex) &&
        ks.Put(LimitLevel) && ks.Put(RequireDungeonId);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out int mid) || !ks.Get(Pos) || !ks.Get(out byte lti) ||
            !ks.Get(out int ll) || !ks.Get(out int rdid))
            return false;
        Ok = ok; MapId = mid; LastTouchLineIndex = lti; LimitLevel = ll; RequireDungeonId = rdid;
        return true;
    }
}

public sealed class KEGS_CALL_MY_LOVER_JOIN_BATTLE_FIELD_NOT : IKSerializable
{
    public int Ok { get; set; }
    public KBattleFieldJoinInfo BattleFieldJoinInfo { get; set; } = new();
    public KRoomInfo RoomInfo { get; set; } = new();
    public List<KRoomSlotInfo> Slots { get; set; } = new();
    public string CnIp { get; set; } = string.Empty;
    public KVector3 Pos { get; set; } = new();
    public int RequireLevel { get; set; }
    public int RequireDungeonId { get; set; }
    public string UdpRelayIp { get; set; } = string.Empty;
    public ushort UdpRelayPort { get; set; }
    public List<long> EnterCashShopUser { get; set; } = new();
    public long CalledUserUid { get; set; }
    public long CalledGsUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(BattleFieldJoinInfo) && ks.Put(RoomInfo) && ks.PutList(Slots) &&
        ks.PutWideString(CnIp) && ks.Put(Pos) && ks.Put(RequireLevel) && ks.Put(RequireDungeonId) &&
        ks.PutWideString(UdpRelayIp) && ks.Put(UdpRelayPort) &&
        ks.PutList(EnterCashShopUser, static (s, v) => s.Put(v)) &&
        ks.Put(CalledUserUid) && ks.Put(CalledGsUid);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(BattleFieldJoinInfo) || !ks.Get(RoomInfo) || !ks.GetList(Slots) ||
            !ks.GetWideString(out string cnip) || !ks.Get(Pos) || !ks.Get(out int rl) || !ks.Get(out int rdid) ||
            !ks.GetWideString(out string uip) || !ks.Get(out ushort up) ||
            !ks.GetList(EnterCashShopUser, static s => (s.Get(out long v), v)) ||
            !ks.Get(out long cu) || !ks.Get(out long cg))
            return false;
        Ok = ok; CnIp = cnip; RequireLevel = rl; RequireDungeonId = rdid;
        UdpRelayIp = uip; UdpRelayPort = up; CalledUserUid = cu; CalledGsUid = cg;
        return true;
    }
}

public sealed class KEGS_RELATIONSHIP_INFO_REQ : IKSerializable
{
    public long AcceptUnitUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(AcceptUnitUid);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid)) return false;
        AcceptUnitUid = uid;
        return true;
    }
}

public sealed class KEGS_RELATIONSHIP_INFO_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long RequestGsUid { get; set; }
    public long RequestUnitUid { get; set; }
    public long AcceptUnitUid { get; set; }
    public byte MyLevel { get; set; }
    public sbyte MyUnitClass { get; set; }
    public int MyMapId { get; set; }
    public sbyte MyPosition { get; set; } = -1;
    public string MyChannelName { get; set; } = string.Empty;
    public Dictionary<int, KInventoryItemInfo> OtherUnitEquippedItem { get; set; } = new();
    public byte Level { get; set; }
    public sbyte UnitClass { get; set; }
    public int MapId { get; set; }
    public long LastLogOutTime { get; set; }
    public bool IsConnect { get; set; }
    public string ChannelName { get; set; } = string.Empty;
    public sbyte Position { get; set; } = -1;

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(RequestGsUid) && ks.Put(RequestUnitUid) && ks.Put(AcceptUnitUid) &&
        ks.Put(MyLevel) && ks.Put(MyUnitClass) && ks.Put(MyMapId) && ks.Put(MyPosition) && ks.PutWideString(MyChannelName) &&
        ks.PutMap(OtherUnitEquippedItem, static (s, k) => s.Put(k), static (s, v) => s.Put(v)) &&
        ks.Put(Level) && ks.Put(UnitClass) && ks.Put(MapId) && ks.Put(LastLogOutTime) &&
        ks.Put(IsConnect) && ks.PutWideString(ChannelName) && ks.Put(Position);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out long rg) || !ks.Get(out long ru) || !ks.Get(out long au) ||
            !ks.Get(out byte ml) || !ks.Get(out sbyte muc) || !ks.Get(out int mm) || !ks.Get(out sbyte mp) || !ks.GetWideString(out string mcn) ||
            !ks.GetMap(OtherUnitEquippedItem, static s => (s.Get(out int k), k), static s => { var v = new KInventoryItemInfo(); return (s.Get(v), v); }) ||
            !ks.Get(out byte lvl) || !ks.Get(out sbyte uc) || !ks.Get(out int mid) || !ks.Get(out long llt) ||
            !ks.Get(out bool ic) || !ks.GetWideString(out string cn) || !ks.Get(out sbyte pos))
            return false;
        Ok = ok; RequestGsUid = rg; RequestUnitUid = ru; AcceptUnitUid = au;
        MyLevel = ml; MyUnitClass = muc; MyMapId = mm; MyPosition = mp; MyChannelName = mcn;
        Level = lvl; UnitClass = uc; MapId = mid; LastLogOutTime = llt; IsConnect = ic; ChannelName = cn; Position = pos;
        return true;
    }
}

public sealed class KEGS_RELATIONSHIP_EFFECT_NOT : IKSerializable
{
    public long ManUid { get; set; }
    public long GirlUid { get; set; }
    public int EffectIndex { get; set; }
    public string LoveWord { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(ManUid) && ks.Put(GirlUid) && ks.Put(EffectIndex) && ks.PutWideString(LoveWord);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long m) || !ks.Get(out long g) || !ks.Get(out int ei) || !ks.GetWideString(out string lw))
            return false;
        ManUid = m; GirlUid = g; EffectIndex = ei; LoveWord = lw;
        return true;
    }
}

// -------- Match / Party Type / AutoParty Bonus / Anti-Addiction --------

public sealed class KEGS_CHANGE_PARTY_TYPE_NOT : IKSerializable
{
    public KPartyInfo PartyInfo { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(PartyInfo);
    public bool Get(KSerializer ks, RegionProfile region) => ks.Get(PartyInfo);
}

public sealed class KEGS_PVP_PARTY_CHANGE_MATCH_INFO_REQ : IKSerializable
{
    public sbyte PvpGameType { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(PvpGameType);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte pgt)) return false;
        PvpGameType = pgt;
        return true;
    }
}

public sealed class KEGS_PVP_NPC_UNIT_RE_BIRTH_POS_REQ : IKSerializable
{
    public long PvpNpcUnitUid { get; set; }
    public int StartPosIndex { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(PvpNpcUnitUid) && ks.Put(StartPosIndex);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.Get(out int idx)) return false;
        PvpNpcUnitUid = uid; StartPosIndex = idx;
        return true;
    }
}

public sealed class KEGS_PVP_NPC_UNIT_INFO_TO_SERVER_REQ : IKSerializable
{
    public Dictionary<long, float> PvpNpcInfo { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.PutMap(PvpNpcInfo, static (s, k) => s.Put(k), static (s, v) => s.Put(v));
    public bool Get(KSerializer ks, RegionProfile region) =>
        ks.GetMap(PvpNpcInfo, static s => (s.Get(out long k), k), static s => (s.Get(out float v), v));
}

public sealed class KEGS_REQUEST_GET_AUTO_PARTY_BONUS_INFO_NOT : IKSerializable
{
    public int VillageMapId { get; set; }
    public List<KDungeonGameSimpleInfo> DungeonList { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(VillageMapId) && ks.PutList(DungeonList);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int vmid) || !ks.GetList(DungeonList)) return false;
        VillageMapId = vmid;
        return true;
    }
}

public sealed class KEGS_UPDATE_AUTO_PARTY_BONUS_INFO_NOT : IKSerializable
{
    public Dictionary<int, KAutoPartyDungeonBonusInfo> DungeonBonusList { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.PutMap(DungeonBonusList, static (s, k) => s.Put(k), static (s, v) => s.Put(v));

    public bool Get(KSerializer ks, RegionProfile region) =>
        ks.GetMap(DungeonBonusList, static s => (s.Get(out int k), k), static s => { var v = new KAutoPartyDungeonBonusInfo(); return (s.Get(v), v); });
}

public sealed class KEGS_ANTI_ADDICTION_NOT : IKSerializable
{
    public const int NtChat = 0;
    public const int NtCenter = 1;

    public int Type { get; set; }
    public int PlayTime { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Type) && ks.Put(PlayTime);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int t) || !ks.Get(out int pt)) return false;
        Type = t; PlayTime = pt;
        return true;
    }
}

// ===== Gameplay Parity Batch Packets (Dungeon, BattleField, Party, Field, PvP, RoomCommon, Square, TrainingCenter) =====

public sealed class KEGS_DUNGEON_SECRET_STAGE_ENTER_SELECT_NOT : IKSerializable
{
    public bool WantToEnterSecretStage { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(WantToEnterSecretStage);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool v)) return false;
        WantToEnterSecretStage = v;
        return true;
    }
}

public sealed class KEGS_STOP_DUNGEON_CONTINUE_TIME_REQ : IKSerializable
{
    public bool IsStop { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(IsStop);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool v)) return false;
        IsStop = v;
        return true;
    }
}

public sealed class KEGS_STOP_DUNGEON_CONTINUE_TIME_ACK : IKSerializable
{
    public int Ok { get; set; }
    public bool IsStop { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.Put(IsStop);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out bool v)) return false;
        Ok = ok; IsStop = v;
        return true;
    }
}

public sealed class KEGS_WORLD_TRIGGER_RELOCATION_REQ : IKSerializable
{
    public int WorldTrigger { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(WorldTrigger);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int wt)) return false;
        WorldTrigger = wt;
        return true;
    }
}

public sealed class KEGS_WORLD_TRIGGER_RELOCATION_ACK : IKSerializable
{
    public int Ok { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok);
    public bool Get(KSerializer ks, RegionProfile region) { if (!ks.Get(out int ok)) return false; Ok = ok; return true; }
}

public sealed class KEGS_WORLD_TRIGGER_RELOCATION_NOT : IKSerializable
{
    public int WorldTrigger { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(WorldTrigger);
    public bool Get(KSerializer ks, RegionProfile region) { if (!ks.Get(out int wt)) return false; WorldTrigger = wt; return true; }
}

public sealed class KEGS_BAD_ATTITUDE_USER_CHECK_INFO_NOT : IKSerializable
{
    public int StageId { get; set; }
    public int SubStageId { get; set; }
    public int ComboScore { get; set; }
    public int TechScore { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(StageId) && ks.Put(SubStageId) && ks.Put(ComboScore) && ks.Put(TechScore);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int st) || !ks.Get(out int ss) || !ks.Get(out int cs) || !ks.Get(out int ts)) return false;
        StageId = st; SubStageId = ss; ComboScore = cs; TechScore = ts;
        return true;
    }
}

public sealed class KEGS_BAD_ATTITUDE_USER_MSG_NOT : IKSerializable
{
    public const sbyte MtNone = 0;
    public const sbyte MtBadAttitudeUserWarning = 1;
    public const sbyte MtBadAttitudeUserExit = 2;
    public const sbyte MtForcedExit = 3;

    public long UserUid { get; set; }
    public sbyte MsgType { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(UserUid) && ks.Put(MsgType);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.Get(out sbyte mt)) return false;
        UserUid = uid; MsgType = mt;
        return true;
    }
}

public sealed class KEGS_DEFENCE_DUNGEON_WAVE_ID_NOT : IKSerializable
{
    public int WaveId { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(WaveId);
    public bool Get(KSerializer ks, RegionProfile region) { if (!ks.Get(out int wid)) return false; WaveId = wid; return true; }
}

public sealed class KEGS_ENTER_DEFENCE_DUNGEON_POPUP_REPLY_NOT : IKSerializable
{
    public bool Agree { get; set; } = true;
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Agree);
    public bool Get(KSerializer ks, RegionProfile region) { if (!ks.Get(out bool a)) return false; Agree = a; return true; }
}

public sealed class KEGS_SYNC_DUNGEON_TIMER_NOT : IKSerializable
{
    public int RemainTime { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(RemainTime);
    public bool Get(KSerializer ks, RegionProfile region) { if (!ks.Get(out int rt)) return false; RemainTime = rt; return true; }
}

public sealed class KEGS_SYNC_BATTLE_FIELD_DAMAGED_MAP_REQ : IKSerializable
{
    public int NpcUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(NpcUid);
    public bool Get(KSerializer ks, RegionProfile region) { if (!ks.Get(out int id)) return false; NpcUid = id; return true; }
}

public sealed class KEGS_SYNC_BATTLE_FIELD_DAMAGED_MAP_ACK : IKSerializable
{
    public int Ok { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok);
    public bool Get(KSerializer ks, RegionProfile region) { if (!ks.Get(out int ok)) return false; Ok = ok; return true; }
}

public sealed class KEGS_BATTLE_FIELD_NPC_DAMAGED_MAP_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public int NpcUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(UnitUid) && ks.Put(NpcUid);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long u) || !ks.Get(out int n)) return false;
        UnitUid = u; NpcUid = n;
        return true;
    }
}

public sealed class KEGS_BATTLE_FIELD_NPC_DAMAGED_MAP_ACK : IKSerializable
{
    public long UnitUid { get; set; }
    public int NpcUid { get; set; }
    public Dictionary<long, float> DamagedMap { get; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(UnitUid) && ks.Put(NpcUid) && ks.PutMap(DamagedMap, static (s, k) => s.Put(k), static (s, v) => s.Put(v));

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long u) || !ks.Get(out int n) ||
            !ks.GetMap(DamagedMap, static s => (s.Get(out long k), k), static s => (s.Get(out float v), v)))
            return false;
        UnitUid = u; NpcUid = n;
        return true;
    }
}

public sealed class KEGS_SYNC_BATTLE_FIELD_DAMAGED_MAP_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public int NpcUid { get; set; }
    public Dictionary<long, float> DamagedMap { get; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(UnitUid) && ks.Put(NpcUid) && ks.PutMap(DamagedMap, static (s, k) => s.Put(k), static (s, v) => s.Put(v));

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long u) || !ks.Get(out int n) ||
            !ks.GetMap(DamagedMap, static s => (s.Get(out long k), k), static s => (s.Get(out float v), v)))
            return false;
        UnitUid = u; NpcUid = n;
        return true;
    }
}

public sealed class KEGS_FIELD_WORKINGS_BLOCK_LOG_ACK : IKSerializable
{
    public sbyte X2State { get; set; }
    public int PhysicalMemory { get; set; }
    public int VirtualMemory { get; set; }
    public sbyte ProcessTerm { get; set; }
    public sbyte FieldUserNum { get; set; }
    public sbyte FieldNpcNum { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(X2State) && ks.Put(PhysicalMemory) && ks.Put(VirtualMemory) &&
        ks.Put(ProcessTerm) && ks.Put(FieldUserNum) && ks.Put(FieldNpcNum);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte st) || !ks.Get(out int pm) || !ks.Get(out int vm) ||
            !ks.Get(out sbyte pt) || !ks.Get(out sbyte fu) || !ks.Get(out sbyte fn))
            return false;
        X2State = st; PhysicalMemory = pm; VirtualMemory = vm; ProcessTerm = pt; FieldUserNum = fu; FieldNpcNum = fn;
        return true;
    }
}

public sealed class KEGS_BOSS_FIELD_INTRUDE_RESTRICTION_ACK : IKSerializable
{
    public int Ok { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok);
    public bool Get(KSerializer ks, RegionProfile region) { if (!ks.Get(out int ok)) return false; Ok = ok; return true; }
}

public sealed class KEGS_BOSS_FIELD_LOG_NOT : IKSerializable
{
    public Dictionary<long, long> GivenDamage { get; } = new();
    public Dictionary<long, long> AttackDamage { get; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.PutMap(GivenDamage, static (s, k) => s.Put(k), static (s, v) => s.Put(v)) &&
        ks.PutMap(AttackDamage, static (s, k) => s.Put(k), static (s, v) => s.Put(v));

    public bool Get(KSerializer ks, RegionProfile region) =>
        ks.GetMap(GivenDamage, static s => (s.Get(out long k), k), static s => (s.Get(out long v), v)) &&
        ks.GetMap(AttackDamage, static s => (s.Get(out long k), k), static s => (s.Get(out long v), v));
}

public sealed class KEGS_PARTY_CHANGE_CHECK_MIN_LEVEL_REQ : IKSerializable
{
    public bool CheckMinLevel { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(CheckMinLevel);
    public bool Get(KSerializer ks, RegionProfile region) { if (!ks.Get(out bool v)) return false; CheckMinLevel = v; return true; }
}

public sealed class KEGS_PARTY_CHANGE_CHECK_MIN_LEVEL_ACK : IKSerializable
{
    public int Ok { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok);
    public bool Get(KSerializer ks, RegionProfile region) { if (!ks.Get(out int ok)) return false; Ok = ok; return true; }
}

public sealed class KEGS_PARTY_CHANGE_CHECK_MIN_LEVEL_NOT : IKSerializable
{
    public bool CheckMinLevel { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(CheckMinLevel);
    public bool Get(KSerializer ks, RegionProfile region) { if (!ks.Get(out bool v)) return false; CheckMinLevel = v; return true; }
}

public sealed class KEGS_PARTY_LIST_REQ : IKSerializable
{
    public const sbyte PltDungeon = 0;
    public const sbyte PltGateTotal = 1;
    public const sbyte PltSecretDungeon = 2;

    public sbyte ListType { get; set; } = PltDungeon;
    public uint ViewPage { get; set; } = 1;
    public int Id { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(ListType) && ks.Put(ViewPage) && ks.Put(Id);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte lt) || !ks.Get(out uint vp) || !ks.Get(out int id)) return false;
        ListType = lt; ViewPage = vp; Id = id;
        return true;
    }
}

public sealed class KEGS_PARTY_LIST_ACK : IKSerializable
{
    public int Ok { get; set; }
    public uint TotalPage { get; set; } = 1;
    public uint ViewPage { get; set; } = 1;
    public List<KPartyListInfo> PartyInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(TotalPage) && ks.Put(ViewPage) && ks.PutList(PartyInfo);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out uint tp) || !ks.Get(out uint vp) || !ks.GetList(PartyInfo)) return false;
        Ok = ok; TotalPage = tp; ViewPage = vp;
        return true;
    }
}

public sealed class KEGS_UPDATE_FRIEND_INFO_REQ : IKSerializable
{
    public bool Delete { get; set; }
    public string FriendNickName { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Delete) && ks.PutWideString(FriendNickName);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool del) || !ks.GetWideString(out var nick)) return false;
        Delete = del; FriendNickName = nick;
        return true;
    }
}

public sealed class KEGS_UPDATE_FRIEND_INFO_ACK : IKSerializable
{
    public int Ok { get; set; }
    public string FriendNickName { get; set; } = string.Empty;
    public long FriendUnitUid { get; set; }
    public int ChannelId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.PutWideString(FriendNickName) && ks.Put(FriendUnitUid) && ks.Put(ChannelId);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.GetWideString(out var nick) || !ks.Get(out long fuid) || !ks.Get(out int ch)) return false;
        Ok = ok; FriendNickName = nick; FriendUnitUid = fuid; ChannelId = ch;
        return true;
    }
}

public sealed class KEGS_ED_MONITORING_BLOCK_REQ : IKSerializable
{
    public const byte EmbtNone = 0;
    public const byte EmbtSendMail = 1;
    public const byte EmbtAccountBlock = 2;
    public const byte EmbtMachineIdBlock = 3;

    public long UnitUid { get; set; }
    public byte Type { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(UnitUid) && ks.Put(Type);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.Get(out byte t)) return false;
        UnitUid = uid; Type = t;
        return true;
    }
}

public sealed class KEGS_ED_MONITORING_BLOCK_ACK : IKSerializable
{
    public int Ok { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok);
    public bool Get(KSerializer ks, RegionProfile region) { if (!ks.Get(out int ok)) return false; Ok = ok; return true; }
}

public sealed class KEGS_COMMUNITY_USER_LIST_MONITORING_MODE_REQ : IKSerializable
{
    public uint ViewPage { get; set; }
    public List<long> UnitUidList { get; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(ViewPage) && ks.PutList(UnitUidList, static (s, v) => s.Put(v));
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint vp) || !ks.GetList(UnitUidList, static s => (s.Get(out long v), v))) return false;
        ViewPage = vp;
        return true;
    }
}

public sealed class KEGS_TOGGLE_INVISIBLE_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public KFieldUserInfo FieldUserInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(UnitUid) && ks.Put(FieldUserInfo);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.Get(FieldUserInfo)) return false;
        UnitUid = uid;
        return true;
    }
}

public sealed class KEGS_OPEN_PVP_ROOM_LIST_ACK : IKSerializable
{
    public int Ok { get; set; }
    public uint ConnectChannelId { get; set; }
    public List<KPVPChannelInfo> Channel { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(ConnectChannelId) && ks.PutList(Channel);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out uint cid) || !ks.GetList(Channel)) return false;
        Ok = ok; ConnectChannelId = cid;
        return true;
    }
}

public sealed class KEGS_CHANGE_PVP_ROOM_LIST_REQ : IKSerializable
{
    public uint RoomListId { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(RoomListId);
    public bool Get(KSerializer ks, RegionProfile region) { if (!ks.Get(out uint id)) return false; RoomListId = id; return true; }
}

public sealed class KEGS_CHANGE_PVP_ROOM_LIST_ACK : IKSerializable
{
    public int Ok { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok);
    public bool Get(KSerializer ks, RegionProfile region) { if (!ks.Get(out int ok)) return false; Ok = ok; return true; }
}

public sealed class KEGS_INVITE_PVP_ROOM_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(UnitUid);
    public bool Get(KSerializer ks, RegionProfile region) { if (!ks.Get(out long uid)) return false; UnitUid = uid; return true; }
}

public sealed class KEGS_INVITE_PVP_ROOM_ACK : IKSerializable
{
    public int Ok { get; set; }
    public string NickName { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.PutWideString(NickName);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.GetWideString(out var nick)) return false;
        Ok = ok; NickName = nick;
        return true;
    }
}

public sealed class KEGS_INVITE_PVP_ROOM_NOT : IKSerializable
{
    public string NickName { get; set; } = string.Empty;
    public KRoomInfo RoomInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) => ks.PutWideString(NickName) && ks.Put(RoomInfo);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out var nick) || !ks.Get(RoomInfo)) return false;
        NickName = nick;
        return true;
    }
}

public sealed class KEGS_STATE_CHANGE_GAME_INTRUDE_ACK : IKSerializable
{
    public int Ok { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok);
    public bool Get(KSerializer ks, RegionProfile region) { if (!ks.Get(out int ok)) return false; Ok = ok; return true; }
}

public sealed class KEGS_STATE_CHANGE_GAME_INTRUDE_NOT : IKSerializable
{
    public sbyte RoomState { get; set; }
    public KRoomSlotInfo IntruderSlot { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(RoomState) && ks.Put(IntruderSlot);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte rs) || !ks.Get(IntruderSlot)) return false;
        RoomState = rs;
        return true;
    }
}

public sealed class KEGS_INTRUDE_START_REQ : IKSerializable
{
    public int StartPosIndex { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(StartPosIndex);
    public bool Get(KSerializer ks, RegionProfile region) { if (!ks.Get(out int idx)) return false; StartPosIndex = idx; return true; }
}

public sealed class KEGS_INTRUDE_START_ACK : IKSerializable
{
    public int Ok { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok);
    public bool Get(KSerializer ks, RegionProfile region) { if (!ks.Get(out int ok)) return false; Ok = ok; return true; }
}

public sealed class KEGS_INTRUDE_START_NOT : IKSerializable
{
    public sbyte RoomState { get; set; }
    public int StartPosIndex { get; set; }
    public KRoomSlotInfo IntruderSlot { get; set; } = new();
    public bool Observer { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(RoomState) && ks.Put(StartPosIndex) && ks.Put(IntruderSlot) && ks.Put(Observer);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte rs) || !ks.Get(out int idx) || !ks.Get(IntruderSlot) || !ks.Get(out bool obs)) return false;
        RoomState = rs; StartPosIndex = idx; Observer = obs;
        return true;
    }
}

public sealed class KEGS_CHANGE_SLOT_OPEN_REQ : IKSerializable
{
    public sbyte SlotIndex { get; set; }
    public sbyte SlotState { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(SlotIndex) && ks.Put(SlotState);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte idx) || !ks.Get(out sbyte st)) return false;
        SlotIndex = idx; SlotState = st;
        return true;
    }
}

public sealed class KEGS_CHANGE_SLOT_OPEN_ACK : IKSerializable
{
    public int Ok { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok);
    public bool Get(KSerializer ks, RegionProfile region) { if (!ks.Get(out int ok)) return false; Ok = ok; return true; }
}

public sealed class KEGS_CHANGE_SLOT_OPEN_NOT : IKSerializable
{
    public sbyte SlotIndex { get; set; }
    public sbyte TeamBalanceSlotIndex { get; set; } = -1;
    public sbyte SlotState { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(SlotIndex) && ks.Put(TeamBalanceSlotIndex) && ks.Put(SlotState);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte idx) || !ks.Get(out sbyte tb) || !ks.Get(out sbyte st)) return false;
        SlotIndex = idx; TeamBalanceSlotIndex = tb; SlotState = st;
        return true;
    }
}

public sealed class KEGS_CHANGE_WINNING_NUM_KILL_REQ : IKSerializable
{
    public int WinKillNum { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(WinKillNum);
    public bool Get(KSerializer ks, RegionProfile region) { if (!ks.Get(out int w)) return false; WinKillNum = w; return true; }
}

public sealed class KEGS_CHANGE_WINNING_NUM_KILL_ACK : IKSerializable
{
    public int Ok { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok);
    public bool Get(KSerializer ks, RegionProfile region) { if (!ks.Get(out int ok)) return false; Ok = ok; return true; }
}

public sealed class KEGS_CHANGE_WINNING_NUM_KILL_NOT : IKSerializable
{
    public int WinKillNum { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(WinKillNum);
    public bool Get(KSerializer ks, RegionProfile region) { if (!ks.Get(out int w)) return false; WinKillNum = w; return true; }
}

public sealed class KEGS_LEAVE_GAME_ACK : IKSerializable
{
    public int Ok { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok);
    public bool Get(KSerializer ks, RegionProfile region) { if (!ks.Get(out int ok)) return false; Ok = ok; return true; }
}

public sealed class KEGS_LEAVE_GAME_NOT : IKSerializable
{
    public sbyte RoomState { get; set; }
    public long UnitUid { get; set; }
    public List<KRoomSlotInfo> Slot { get; set; } = new();
    public List<KRoomSlotInfo> ObserverSlot { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(RoomState) && ks.Put(UnitUid) && ks.PutList(Slot) && ks.PutList(ObserverSlot);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte rs) || !ks.Get(out long uid) || !ks.GetList(Slot) || !ks.GetList(ObserverSlot)) return false;
        RoomState = rs; UnitUid = uid;
        return true;
    }
}

public sealed class KEGS_CREATE_PVP_ITEM_REQ : IKSerializable
{
    public int PositionIndex { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(PositionIndex);
    public bool Get(KSerializer ks, RegionProfile region) { if (!ks.Get(out int pi)) return false; PositionIndex = pi; return true; }
}

public sealed class KEGS_CREATE_PVP_ITEM_ACK : IKSerializable
{
    public int Ok { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok);
    public bool Get(KSerializer ks, RegionProfile region) { if (!ks.Get(out int ok)) return false; Ok = ok; return true; }
}

public sealed class KEGS_CREATE_PVP_ITEM_NOT : IKSerializable
{
    public int PositionIndex { get; set; }
    public KDropItemData DropItemData { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(PositionIndex) && ks.Put(DropItemData);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int pi) || !ks.Get(DropItemData)) return false;
        PositionIndex = pi;
        return true;
    }
}

public sealed class KEGS_CHANGE_PVP_ITEMMODE_ACK : IKSerializable
{
    public int Ok { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok);
    public bool Get(KSerializer ks, RegionProfile region) { if (!ks.Get(out int ok)) return false; Ok = ok; return true; }
}

public sealed class KEGS_CHANGE_PVP_ITEMMODE_NOT : IKSerializable
{
    public bool IsItemMode { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(IsItemMode);
    public bool Get(KSerializer ks, RegionProfile region) { if (!ks.Get(out bool v)) return false; IsItemMode = v; return true; }
}

public sealed class KEGS_CHANGE_PVP_GAME_OPTION_REQ : IKSerializable
{
    public bool BaseDefence { get; set; }
    public bool BossCombat { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(BaseDefence) && ks.Put(BossCombat);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool bd) || !ks.Get(out bool bc)) return false;
        BaseDefence = bd; BossCombat = bc;
        return true;
    }
}

public sealed class KEGS_CHANGE_PVP_GAME_OPTION_ACK : IKSerializable
{
    public int Ok { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok);
    public bool Get(KSerializer ks, RegionProfile region) { if (!ks.Get(out int ok)) return false; Ok = ok; return true; }
}

public sealed class KEGS_CHANGE_PVP_GAME_OPTION_NOT : IKSerializable
{
    public bool BaseDefence { get; set; }
    public bool BossCombat { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(BaseDefence) && ks.Put(BossCombat);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool bd) || !ks.Get(out bool bc)) return false;
        BaseDefence = bd; BossCombat = bc;
        return true;
    }
}

public sealed class KEGS_CHANGE_PVP_ITEM_POS_REQ : IKSerializable
{
    public long ItemUid { get; set; }
    public int Pos { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(ItemUid) && ks.Put(Pos);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.Get(out int pos)) return false;
        ItemUid = uid; Pos = pos;
        return true;
    }
}

public sealed class KEGS_CHANGE_PVP_ITEM_POS_ACK : IKSerializable
{
    public int Ok { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok);
    public bool Get(KSerializer ks, RegionProfile region) { if (!ks.Get(out int ok)) return false; Ok = ok; return true; }
}

public sealed class KEGS_CHANGE_PVP_ITEM_POS_NOT : IKSerializable
{
    public long ItemUid { get; set; }
    public int Pos { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(ItemUid) && ks.Put(Pos);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.Get(out int pos)) return false;
        ItemUid = uid; Pos = pos;
        return true;
    }
}

public sealed class KEGS_PVP_REMATCH_REQ : IKSerializable
{
    public bool AcceptRematch { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(AcceptRematch);
    public bool Get(KSerializer ks, RegionProfile region) { if (!ks.Get(out bool v)) return false; AcceptRematch = v; return true; }
}

public sealed class KEGS_PVP_REMATCH_ACK : IKSerializable
{
    public int Ok { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok);
    public bool Get(KSerializer ks, RegionProfile region) { if (!ks.Get(out int ok)) return false; Ok = ok; return true; }
}

public sealed class KEGS_PVP_REMATCH_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public bool AcceptRematch { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(UnitUid) && ks.Put(AcceptRematch);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.Get(out bool v)) return false;
        UnitUid = uid; AcceptRematch = v;
        return true;
    }
}

public sealed class KEGS_CREATE_TUTORIAL_ROOM_REQ : IKSerializable
{
    public KRoomInfo RoomInfo { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(RoomInfo);
    public bool Get(KSerializer ks, RegionProfile region) => ks.Get(RoomInfo);
}

public sealed class KEGS_CREATE_TUTORIAL_ROOM_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KRoomInfo RoomInfo { get; set; } = new();
    public List<KRoomSlotInfo> Slot { get; set; } = new();
    public string CnIp { get; set; } = "127.0.0.1";

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(RoomInfo) && ks.PutList(Slot) && ks.PutWideString(CnIp);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(RoomInfo) || !ks.GetList(Slot) || !ks.GetWideString(out var ip)) return false;
        Ok = ok; CnIp = ip;
        return true;
    }
}

public sealed class KEGS_QUICK_JOIN_REQ : IKSerializable
{
    public int Option { get; set; }
    public bool IsItemMode { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Option) && ks.Put(IsItemMode);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int opt) || !ks.Get(out bool im)) return false;
        Option = opt; IsItemMode = im;
        return true;
    }
}

public sealed class KEGS_QUICK_JOIN_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KRoomInfo RoomInfo { get; set; } = new();
    public List<KRoomSlotInfo> Slot { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.Put(RoomInfo) && ks.PutList(Slot);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(RoomInfo) || !ks.GetList(Slot)) return false;
        Ok = ok;
        return true;
    }
}

public sealed class KEGS_STATE_CHANGE_RESULT_REQ : IKSerializable
{
    public int Dummy { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Dummy);
    public bool Get(KSerializer ks, RegionProfile region) { if (!ks.Get(out int d)) return false; Dummy = d; return true; }
}

public sealed class KEGS_STATE_CHANGE_RESULT_ACK : IKSerializable
{
    public int Ok { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok);
    public bool Get(KSerializer ks, RegionProfile region) { if (!ks.Get(out int ok)) return false; Ok = ok; return true; }
}

public sealed class KEGS_STATE_CHANGE_RESULT_NOT : IKSerializable
{
    public sbyte RoomState { get; set; }
    public List<KRoomSlotInfo> Slot { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(RoomState) && ks.PutList(Slot);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte rs) || !ks.GetList(Slot)) return false;
        RoomState = rs;
        return true;
    }
}

public sealed class KEGS_USER_UNIT_RE_BIRTH_POS_REQ : IKSerializable
{
    public int StartPosIndex { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(StartPosIndex);
    public bool Get(KSerializer ks, RegionProfile region) { if (!ks.Get(out int idx)) return false; StartPosIndex = idx; return true; }
}

public sealed class KEGS_MY_USER_UNIT_INFO_TO_SERVER_REQ : IKSerializable
{
    public KDungeonPlayResultInfo MyPlayResult { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(MyPlayResult);
    public bool Get(KSerializer ks, RegionProfile region) => ks.Get(MyPlayResult);
}

public sealed class KEGS_REFRESH_ROOM_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KRoomInfo RoomInfo { get; set; } = new();
    public List<KRoomSlotInfo> Slot { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.Put(RoomInfo) && ks.PutList(Slot);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(RoomInfo) || !ks.GetList(Slot)) return false;
        Ok = ok;
        return true;
    }
}

public sealed class KEGS_RESULT_SUCCESS_REQ : IKSerializable
{
    public KGamePlayNetworkInfoPackage GamePlayNetworkInfo { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(GamePlayNetworkInfo);
    public bool Get(KSerializer ks, RegionProfile region) => ks.Get(GamePlayNetworkInfo);
}

public sealed class KEGS_RESULT_SUCCESS_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int DungeonId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.Put(DungeonId);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out int did)) return false;
        Ok = ok; DungeonId = did;
        return true;
    }
}

public sealed class KEGS_SQUARE_LIST_REQ : IKSerializable
{
    public uint ViewPage { get; set; } = 1;
    public sbyte SquareType { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(ViewPage) && ks.Put(SquareType);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint vp) || !ks.Get(out sbyte st)) return false;
        ViewPage = vp; SquareType = st;
        return true;
    }
}

public sealed class KEGS_SQUARE_LIST_ACK : IKSerializable
{
    public int Ok { get; set; }
    public uint TotalPage { get; set; } = 1;
    public uint ViewPage { get; set; } = 1;
    public List<KSquareInfo> SquareInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(TotalPage) && ks.Put(ViewPage) && ks.PutList(SquareInfo);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out uint tp) || !ks.Get(out uint vp) || !ks.GetList(SquareInfo)) return false;
        Ok = ok; TotalPage = tp; ViewPage = vp;
        return true;
    }
}

public sealed class KEGS_JOIN_SQUARE_REQ : IKSerializable
{
    public long SquareUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(SquareUid);
    public bool Get(KSerializer ks, RegionProfile region) { if (!ks.Get(out long uid)) return false; SquareUid = uid; return true; }
}

public sealed class KEGS_JOIN_SQUARE_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KSquareInfo SquareInfo { get; set; } = new();
    public List<KSquareUserInfo> UserInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.Put(SquareInfo) && ks.PutList(UserInfo);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(SquareInfo) || !ks.GetList(UserInfo)) return false;
        Ok = ok;
        return true;
    }
}

public sealed class KEGS_JOIN_SQUARE_NOT : IKSerializable
{
    public KSquareUserInfo UserInfo { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(UserInfo);
    public bool Get(KSerializer ks, RegionProfile region) => ks.Get(UserInfo);
}

public sealed class KEGS_LEAVE_SQUARE_REQ : IKSerializable
{
    public int Reason { get; set; }
    public int BattleFieldId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Reason) && ks.Put(BattleFieldId);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int r)) return false;
        Reason = r;
        if (ks.Get(out int bfid)) BattleFieldId = bfid;
        return true;
    }
}

public sealed class KEGS_LEAVE_SQUARE_ACK : IKSerializable
{
    public int Ok { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok);
    public bool Get(KSerializer ks, RegionProfile region) { if (!ks.Get(out int ok)) return false; Ok = ok; return true; }
}

public sealed class KEGS_LEAVE_SQUARE_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public int Reason { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(UnitUid) && ks.Put(Reason);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.Get(out int r)) return false;
        UnitUid = uid; Reason = r;
        return true;
    }
}

public sealed class KEGS_SQUARE_UNIT_SYNC_DATA_REQ : IKSerializable
{
    public sbyte StateId { get; set; }
    public sbyte StateChangeNum { get; set; }
    public KVector3 Pos { get; set; } = new();
    public byte LastTouchLineIndex { get; set; }
    public bool IsRight { get; set; } = true;

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(StateId) && ks.Put(StateChangeNum) && ks.Put(Pos) && ks.Put(LastTouchLineIndex) && ks.Put(IsRight);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte sid) || !ks.Get(out sbyte scn) || !ks.Get(Pos) ||
            !ks.Get(out byte lt) || !ks.Get(out bool ir)) return false;
        StateId = sid; StateChangeNum = scn; LastTouchLineIndex = lt; IsRight = ir;
        return true;
    }
}

public sealed class KEGS_SQUARE_UNIT_SYNC_DATA_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public sbyte StateId { get; set; }
    public sbyte StateChangeNum { get; set; }
    public KVector3 Pos { get; set; } = new();
    public byte LastTouchLineIndex { get; set; }
    public bool IsRight { get; set; } = true;
    public int Ed { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(UnitUid) && ks.Put(StateId) && ks.Put(StateChangeNum) && ks.Put(Pos) &&
        ks.Put(LastTouchLineIndex) && ks.Put(IsRight) && ks.Put(Ed);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.Get(out sbyte sid) || !ks.Get(out sbyte scn) || !ks.Get(Pos) ||
            !ks.Get(out byte lt) || !ks.Get(out bool ir) || !ks.Get(out int ed)) return false;
        UnitUid = uid; StateId = sid; StateChangeNum = scn; LastTouchLineIndex = lt; IsRight = ir; Ed = ed;
        return true;
    }
}

public sealed class KEGS_SET_TC_REMAINING_TIME_REQ : IKSerializable
{
    public bool Enable { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Enable);
    public bool Get(KSerializer ks, RegionProfile region) { if (!ks.Get(out bool v)) return false; Enable = v; return true; }
}

public sealed class KEGS_SET_TC_REMAINING_TIME_ACK : IKSerializable
{
    public int Ok { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok);
    public bool Get(KSerializer ks, RegionProfile region) { if (!ks.Get(out int ok)) return false; Ok = ok; return true; }
}

public sealed class KEGS_ADD_ON_STAT_REQ : IKSerializable
{
    public const sbyte AosNone = 0;
    public const sbyte AosAtkPhysic = 1;
    public const sbyte AosAtkMagic = 2;
    public const sbyte AosDefPhysic = 3;
    public const sbyte AosDefMagic = 4;

    public long Uid { get; set; }
    public bool IsUnit { get; set; }
    public sbyte StatType { get; set; }
    public float StatVal { get; set; }
    public float Time { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Uid) && ks.Put(IsUnit) && ks.Put(StatType) && ks.Put(StatVal) && ks.Put(Time);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.Get(out bool iu) || !ks.Get(out sbyte st) ||
            !ks.Get(out float sv) || !ks.Get(out float tm)) return false;
        Uid = uid; IsUnit = iu; StatType = st; StatVal = sv; Time = tm;
        return true;
    }
}

public sealed class KEGS_ADD_ON_STAT_ACK : IKSerializable
{
    public int Ok { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok);
    public bool Get(KSerializer ks, RegionProfile region) { if (!ks.Get(out int ok)) return false; Ok = ok; return true; }
}

public sealed class KEGS_ADD_ON_STAT_NOT : IKSerializable
{
    public long Uid { get; set; }
    public bool IsUnit { get; set; }
    public sbyte StatType { get; set; }
    public float StatVal { get; set; }
    public float Time { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Uid) && ks.Put(IsUnit) && ks.Put(StatType) && ks.Put(StatVal) && ks.Put(Time);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.Get(out bool iu) || !ks.Get(out sbyte st) ||
            !ks.Get(out float sv) || !ks.Get(out float tm)) return false;
        Uid = uid; IsUnit = iu; StatType = st; StatVal = sv; Time = tm;
        return true;
    }
}

public sealed class KEGS_BUFF_SKILL_INFO_CONVEY_REQ : IKSerializable
{
    public int BuffEnum { get; set; }
    public long CastingUnitUid { get; set; }
    public float BuffTime { get; set; }
    public bool IsBuffCasting { get; set; }
    public sbyte BuffLevel { get; set; }
    public List<long> CaughtUidList { get; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(BuffEnum) && ks.Put(CastingUnitUid) && ks.Put(BuffTime) &&
        ks.Put(IsBuffCasting) && ks.Put(BuffLevel) &&
        ks.PutList(CaughtUidList, static (s, v) => s.Put(v));

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int be) || !ks.Get(out long cu) || !ks.Get(out float bt) ||
            !ks.Get(out bool ibc) || !ks.Get(out sbyte bl) ||
            !ks.GetList(CaughtUidList, static s => (s.Get(out long v), v))) return false;
        BuffEnum = be; CastingUnitUid = cu; BuffTime = bt; IsBuffCasting = ibc; BuffLevel = bl;
        return true;
    }
}

public sealed class KEGS_BUFF_SKILL_INFO_CONVEY_ACK : IKSerializable
{
    public int Ok { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok);
    public bool Get(KSerializer ks, RegionProfile region) { if (!ks.Get(out int ok)) return false; Ok = ok; return true; }
}

public sealed class KEGS_BUFF_SKILL_INFO_CONVEY_NOT : IKSerializable
{
    public Dictionary<int, List<KBuffSkillInfo>> CaughtUidList { get; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.PutMap(CaughtUidList, static (s, k) => s.Put(k), static (s, list) => s.PutList(list));

    public bool Get(KSerializer ks, RegionProfile region) =>
        ks.GetMap(CaughtUidList, static s => (s.Get(out int k), k), static s =>
        {
            var list = new List<KBuffSkillInfo>();
            return (s.GetList(list), list);
        });
}

public sealed class KEGS_UDP_PING_SCORE_NOT : IKSerializable
{
    public uint PingScore { get; set; } = 99999;
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(PingScore);
    public bool Get(KSerializer ks, RegionProfile region) { if (!ks.Get(out uint ps)) return false; PingScore = ps; return true; }
}

public sealed class KEGS_GET_ITEM_REALTIME_NOT : IKSerializable
{
    public List<KInventoryItemInfo> InventorySlotInfo { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region) => ks.PutList(InventorySlotInfo);
    public bool Get(KSerializer ks, RegionProfile region) => ks.GetList(InventorySlotInfo);
}

public sealed class KEGS_PARTY_GAME_START_NOT : IKSerializable
{
    public short WorldId { get; set; }
    public KRoomInfo RoomInfo { get; set; } = new();
    public List<KRoomSlotInfo> Slots { get; set; } = new();
    public List<KRoomSlotInfo> ObserverSlot { get; set; } = new();
    public string CnIp { get; set; } = "127.0.0.1";
    public List<KInventoryItemInfo> InventorySlotInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(WorldId) && ks.Put(RoomInfo) && ks.PutList(Slots) && ks.PutList(ObserverSlot) &&
        ks.PutWideString(CnIp) && ks.PutList(InventorySlotInfo);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out short wid) || !ks.Get(RoomInfo) || !ks.GetList(Slots) ||
            !ks.GetList(ObserverSlot) || !ks.GetWideString(out var ip) || !ks.GetList(InventorySlotInfo))
            return false;
        WorldId = wid; CnIp = ip;
        return true;
    }
}

// -------- Session / Channel Change / Performance / LagCheck / PCBang (15-handler parity) --------

public sealed class KEGS_STATE_CHANGE_SERVER_SELECT_ACK : IKSerializable
{
    public int Ok { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok);
    public bool Get(KSerializer ks, RegionProfile region) { if (!ks.Get(out int ok)) return false; Ok = ok; return true; }
}

public sealed class KEGS_GET_SERVER_SET_DATA_ACK : IKSerializable
{
    public int Ok { get; set; }
    public List<KServerSetData> ServerSetList { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.PutList(ServerSetList);

    public bool Get(KSerializer ks, RegionProfile region) =>
        ks.Get(out int ok) && (Ok = ok) >= int.MinValue && ks.GetList(ServerSetList);
}

public sealed class KEGS_SELECT_SERVER_SET_REQ : IKSerializable
{
    public int ServerSetId { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(ServerSetId);
    public bool Get(KSerializer ks, RegionProfile region) { if (!ks.Get(out int id)) return false; ServerSetId = id; return true; }
}

public sealed class KEGS_SELECT_SERVER_SET_ACK : IKSerializable
{
    public int Ok { get; set; }
    public string Ip { get; set; } = string.Empty;
    public ushort Port { get; set; }
    public KAccountBlockInfo AccountBlockInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.PutWideString(Ip) && ks.Put(Port) && ks.Put(AccountBlockInfo);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.GetWideString(out var ip) || !ks.Get(out ushort port) || !ks.Get(AccountBlockInfo))
            return false;
        Ok = ok; Ip = ip; Port = port;
        return true;
    }
}

public sealed class KEGS_CONNECT_CHANNEL_CHANGE_REQ : IKSerializable
{
    public KEGS_CONNECT_REQ ConnectReq { get; set; } = new();
    public KUserAuthenticateReq VerifyAccountReq { get; set; } = new();
    public KEGS_SELECT_UNIT_REQ SelectUnitReq { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(ConnectReq) && ks.Put(VerifyAccountReq) && ks.Put(SelectUnitReq);

    public bool Get(KSerializer ks, RegionProfile region) =>
        ks.Get(ConnectReq) && ks.Get(VerifyAccountReq) && ks.Get(SelectUnitReq);
}

public sealed class KEGS_CONNECT_CHANNEL_CHANGE_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KEGS_CONNECT_ACK ConnectAck { get; set; } = new();
    public KEGS_VERIFY_ACCOUNT_ACK VerifyAccountAck { get; set; } = new();
    public KChannelChangeInfo ChangeUserInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(ConnectAck) && ks.Put(VerifyAccountAck) && ks.Put(ChangeUserInfo);

    public bool Get(KSerializer ks, RegionProfile region) =>
        ks.Get(out int ok) && (Ok = ok) >= int.MinValue &&
        ks.Get(ConnectAck) && ks.Get(VerifyAccountAck) && ks.Get(ChangeUserInfo);
}

public sealed class KEGS_JOIN_FIELD_CHANNEL_CHANGE_REQ : IKSerializable
{
    public int MapId { get; set; }
    public float PosX { get; set; }
    public float PosY { get; set; }
    public float PosZ { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(MapId) && ks.Put(PosX) && ks.Put(PosY) && ks.Put(PosZ);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int map) || !ks.Get(out float x) || !ks.Get(out float y) || !ks.Get(out float z))
            return false;
        MapId = map; PosX = x; PosY = y; PosZ = z;
        return true;
    }
}

public sealed class KEGS_ENTRY_POINT_GET_CHANNEL_LIST_ACK : IKSerializable
{
    public Dictionary<int, KChannelInfo> SolesChannelList { get; } = new();
    public Dictionary<int, KChannelInfo> GaiaChannelList { get; } = new();
    public Dictionary<int, KChannelBonusInfo> SolesChannelBonusList { get; } = new();
    public Dictionary<int, KChannelBonusInfo> GaiaChannelBonusList { get; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.PutMap(SolesChannelList, static (s, k) => s.Put(k), static (s, v) => v.Put(s, s.Region)) &&
        ks.PutMap(GaiaChannelList, static (s, k) => s.Put(k), static (s, v) => v.Put(s, s.Region)) &&
        ks.PutMap(SolesChannelBonusList, static (s, k) => s.Put(k), static (s, v) => v.Put(s, s.Region)) &&
        ks.PutMap(GaiaChannelBonusList, static (s, k) => s.Put(k), static (s, v) => v.Put(s, s.Region));

    public bool Get(KSerializer ks, RegionProfile region) =>
        ks.GetMap(SolesChannelList, static s => (s.Get(out int k), k), static s => { var v = new KChannelInfo(); return (v.Get(s, s.Region), v); }) &&
        ks.GetMap(GaiaChannelList, static s => (s.Get(out int k), k), static s => { var v = new KChannelInfo(); return (v.Get(s, s.Region), v); }) &&
        ks.GetMap(SolesChannelBonusList, static s => (s.Get(out int k), k), static s => { var v = new KChannelBonusInfo(); return (v.Get(s, s.Region), v); }) &&
        ks.GetMap(GaiaChannelBonusList, static s => (s.Get(out int k), k), static s => { var v = new KChannelBonusInfo(); return (v.Get(s, s.Region), v); });
}

public sealed class KEGS_SYSTEM_INFO : IKSerializable
{
    public long AccountUid { get; set; } = -1;
    public long SystemKey { get; set; } = -1;
    public string Cpu { get; set; } = string.Empty;
    public string Memory { get; set; } = string.Empty;
    public string Gpu { get; set; } = string.Empty;
    public string Os { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(AccountUid) && ks.Put(SystemKey) && ks.PutWideString(Cpu) &&
        ks.PutWideString(Memory) && ks.PutWideString(Gpu) && ks.PutWideString(Os);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long auid) || !ks.Get(out long sk) || !ks.GetWideString(out var cpu) ||
            !ks.GetWideString(out var mem) || !ks.GetWideString(out var gpu) || !ks.GetWideString(out var os))
            return false;
        AccountUid = auid; SystemKey = sk; Cpu = cpu; Memory = mem; Gpu = gpu; Os = os;
        return true;
    }
}

public sealed class KEGS_TCP_PING : IKSerializable
{
    public uint Sequence { get; set; }
    public uint StartTime { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Sequence) && ks.Put(StartTime);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint seq) || !ks.Get(out uint st)) return false;
        Sequence = seq; StartTime = st;
        return true;
    }
}

public sealed class KEGS_KOG_GAME_PERFORMANCE_VILLAGE_LOG : IKSerializable
{
    public long CharUid { get; set; } = -1;
    public long SystemKey { get; set; }
    public float AvgFps { get; set; } = -1f;
    public float AvgFpsStdDev { get; set; } = -1f;
    public bool FirstMemUsageCheck { get; set; } = true;
    public float MinWorkingSetSize { get; set; } = -1f;
    public float MaxWorkingSetSize { get; set; } = -1f;
    public float MinPagefileUsage { get; set; } = -1f;
    public float MaxPagefileUsage { get; set; } = -1f;
    public int PartyType { get; set; } = -1;
    public int PartyUserNum { get; set; } = -1;
    public int MapId { get; set; } = -1;
    public int VillageUserNum { get; set; } = -1;
    public KVillageLagInfo UnknownLagInfo { get; set; } = new();
    public KVillageLagInfo KnownLagInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(CharUid) && ks.Put(SystemKey) && ks.Put(AvgFps) && ks.Put(AvgFpsStdDev) &&
        ks.Put(FirstMemUsageCheck) && ks.Put(MinWorkingSetSize) && ks.Put(MaxWorkingSetSize) &&
        ks.Put(MinPagefileUsage) && ks.Put(MaxPagefileUsage) && ks.Put(PartyType) &&
        ks.Put(PartyUserNum) && ks.Put(MapId) && ks.Put(VillageUserNum) &&
        ks.Put(UnknownLagInfo) && ks.Put(KnownLagInfo);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long cuid) || !ks.Get(out long sk) || !ks.Get(out float fps) || !ks.Get(out float sd) ||
            !ks.Get(out bool fmuc) || !ks.Get(out float minWs) || !ks.Get(out float maxWs) ||
            !ks.Get(out float minPf) || !ks.Get(out float maxPf) || !ks.Get(out int pt) ||
            !ks.Get(out int pun) || !ks.Get(out int mid) || !ks.Get(out int vun) ||
            !ks.Get(UnknownLagInfo) || !ks.Get(KnownLagInfo))
            return false;
        CharUid = cuid; SystemKey = sk; AvgFps = fps; AvgFpsStdDev = sd; FirstMemUsageCheck = fmuc;
        MinWorkingSetSize = minWs; MaxWorkingSetSize = maxWs; MinPagefileUsage = minPf; MaxPagefileUsage = maxPf;
        PartyType = pt; PartyUserNum = pun; MapId = mid; VillageUserNum = vun;
        return true;
    }
}

public sealed class KEGS_KOG_GAME_PERFORMANCE_FIELD_LOG : IKSerializable
{
    public long CharUid { get; set; } = -1;
    public long SystemKey { get; set; }
    public float AvgFps { get; set; } = -1f;
    public float AvgFpsStdDev { get; set; } = -1f;
    public float P2PMinPingTime { get; set; } = -1f;
    public float P2PMaxPingTime { get; set; } = -1f;
    public float P2PAvgPingTime { get; set; } = -1f;
    public float P2PAvgPingTimeStdDev { get; set; } = -1f;
    public float P2PAvgPingLossRate { get; set; } = -1f;
    public float RelayMinPingTime { get; set; } = -1f;
    public float RelayMaxPingTime { get; set; } = -1f;
    public float RelayAvgPingTime { get; set; } = -1f;
    public float RelayAvgPingTimeStdDev { get; set; } = -1f;
    public float RelayAvgPingLossRate { get; set; } = -1f;
    public float UdpMinPingTime { get; set; } = -1f;
    public float UdpMaxPingTime { get; set; } = -1f;
    public float UdpAvgPingTime { get; set; } = -1f;
    public float UdpAvgPingTimeStdDev { get; set; } = -1f;
    public float UdpAvgPingLossRate { get; set; } = -1f;
    public float TcpMinPingTime { get; set; } = -1f;
    public float TcpMaxPingTime { get; set; } = -1f;
    public float TcpAvgPingTime { get; set; } = -1f;
    public float TcpAvgPingTimeStdDev { get; set; } = -1f;
    public float UdpSendCountPerSecHost { get; set; } = -1f;
    public float UdpSendAmountPerSecHost { get; set; } = -1f;
    public float UdpRecvCountPerSecHost { get; set; } = -1f;
    public float UdpRecvAmountPerSecHost { get; set; } = -1f;
    public float UdpSendCountPerSecNonHost { get; set; } = -1f;
    public float UdpSendAmountPerSecNonHost { get; set; } = -1f;
    public float UdpRecvCountPerSecNonHost { get; set; } = -1f;
    public float UdpRecvAmountPerSecNonHost { get; set; } = -1f;
    public float UdpUserSyncPacketLossRate { get; set; } = -1f;
    public float UdpNpcSyncPacketLossRate { get; set; } = -1f;
    public float NonHostNpcReactionHostUdpAvgPingTime { get; set; } = -1f;
    public float NonHostNpcReactionMinDelayTimeSimulOff { get; set; } = -1f;
    public float NonHostNpcReactionMaxDelayTimeSimulOff { get; set; } = -1f;
    public float NonHostNpcReactionAvgDelayTimeSimulOff { get; set; } = -1f;
    public float NonHostNpcReactionAvgDelayTimeStdDevSimulOff { get; set; } = -1f;
    public float NonHostNpcReactionMinDelayTimeSimulOn { get; set; } = -1f;
    public float NonHostNpcReactionMaxDelayTimeSimulOn { get; set; } = -1f;
    public float NonHostNpcReactionAvgDelayTimeSimulOn { get; set; } = -1f;
    public float NonHostNpcReactionAvgDelayTimeStdDevSimulOn { get; set; } = -1f;
    public bool FirstMemUsageCheck { get; set; } = true;
    public float MinWorkingSetSize { get; set; } = -1f;
    public float MaxWorkingSetSize { get; set; } = -1f;
    public float MinPagefileUsage { get; set; } = -1f;
    public float MaxPagefileUsage { get; set; } = -1f;
    public int PartyType { get; set; } = -1;
    public int PartyUserNum { get; set; } = -1;
    public int MapId { get; set; } = -1;
    public int FieldUserNum { get; set; } = -1;
    public KFieldLagInfo UnknownLagInfo { get; set; } = new();
    public KFieldLagInfo KnownLagInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(CharUid) && ks.Put(SystemKey) && ks.Put(AvgFps) && ks.Put(AvgFpsStdDev) &&
        ks.Put(P2PMinPingTime) && ks.Put(P2PMaxPingTime) && ks.Put(P2PAvgPingTime) && ks.Put(P2PAvgPingTimeStdDev) && ks.Put(P2PAvgPingLossRate) &&
        ks.Put(RelayMinPingTime) && ks.Put(RelayMaxPingTime) && ks.Put(RelayAvgPingTime) && ks.Put(RelayAvgPingTimeStdDev) && ks.Put(RelayAvgPingLossRate) &&
        ks.Put(UdpMinPingTime) && ks.Put(UdpMaxPingTime) && ks.Put(UdpAvgPingTime) && ks.Put(UdpAvgPingTimeStdDev) && ks.Put(UdpAvgPingLossRate) &&
        ks.Put(TcpMinPingTime) && ks.Put(TcpMaxPingTime) && ks.Put(TcpAvgPingTime) && ks.Put(TcpAvgPingTimeStdDev) &&
        ks.Put(UdpSendCountPerSecHost) && ks.Put(UdpSendAmountPerSecHost) && ks.Put(UdpRecvCountPerSecHost) && ks.Put(UdpRecvAmountPerSecHost) &&
        ks.Put(UdpSendCountPerSecNonHost) && ks.Put(UdpSendAmountPerSecNonHost) && ks.Put(UdpRecvCountPerSecNonHost) && ks.Put(UdpRecvAmountPerSecNonHost) &&
        ks.Put(UdpUserSyncPacketLossRate) && ks.Put(UdpNpcSyncPacketLossRate) &&
        ks.Put(NonHostNpcReactionHostUdpAvgPingTime) &&
        ks.Put(NonHostNpcReactionMinDelayTimeSimulOff) && ks.Put(NonHostNpcReactionMaxDelayTimeSimulOff) &&
        ks.Put(NonHostNpcReactionAvgDelayTimeSimulOff) && ks.Put(NonHostNpcReactionAvgDelayTimeStdDevSimulOff) &&
        ks.Put(NonHostNpcReactionMinDelayTimeSimulOn) && ks.Put(NonHostNpcReactionMaxDelayTimeSimulOn) &&
        ks.Put(NonHostNpcReactionAvgDelayTimeSimulOn) && ks.Put(NonHostNpcReactionAvgDelayTimeStdDevSimulOn) &&
        ks.Put(FirstMemUsageCheck) && ks.Put(MinWorkingSetSize) && ks.Put(MaxWorkingSetSize) &&
        ks.Put(MinPagefileUsage) && ks.Put(MaxPagefileUsage) && ks.Put(PartyType) &&
        ks.Put(PartyUserNum) && ks.Put(MapId) && ks.Put(FieldUserNum) &&
        ks.Put(UnknownLagInfo) && ks.Put(KnownLagInfo);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long cuid) || !ks.Get(out long sk) || !ks.Get(out float fps) || !ks.Get(out float sd) ||
            !ks.Get(out float p2pMin) || !ks.Get(out float p2pMax) || !ks.Get(out float p2pAvg) || !ks.Get(out float p2pSd) || !ks.Get(out float p2pLoss) ||
            !ks.Get(out float relMin) || !ks.Get(out float relMax) || !ks.Get(out float relAvg) || !ks.Get(out float relSd) || !ks.Get(out float relLoss) ||
            !ks.Get(out float udpMin) || !ks.Get(out float udpMax) || !ks.Get(out float udpAvg) || !ks.Get(out float udpSd) || !ks.Get(out float udpLoss) ||
            !ks.Get(out float tcpMin) || !ks.Get(out float tcpMax) || !ks.Get(out float tcpAvg) || !ks.Get(out float tcpSd) ||
            !ks.Get(out float uscH) || !ks.Get(out float usaH) || !ks.Get(out float urcH) || !ks.Get(out float uraH) ||
            !ks.Get(out float uscN) || !ks.Get(out float usaN) || !ks.Get(out float urcN) || !ks.Get(out float uraN) ||
            !ks.Get(out float uSync) || !ks.Get(out float nSync) ||
            !ks.Get(out float npcPing) ||
            !ks.Get(out float offMin) || !ks.Get(out float offMax) || !ks.Get(out float offAvg) || !ks.Get(out float offSd) ||
            !ks.Get(out float onMin) || !ks.Get(out float onMax) || !ks.Get(out float onAvg) || !ks.Get(out float onSd) ||
            !ks.Get(out bool fmuc) || !ks.Get(out float minWs) || !ks.Get(out float maxWs) ||
            !ks.Get(out float minPf) || !ks.Get(out float maxPf) || !ks.Get(out int pt) ||
            !ks.Get(out int pun) || !ks.Get(out int mid) || !ks.Get(out int fun) ||
            !ks.Get(UnknownLagInfo) || !ks.Get(KnownLagInfo))
            return false;
        CharUid = cuid; SystemKey = sk; AvgFps = fps; AvgFpsStdDev = sd;
        P2PMinPingTime = p2pMin; P2PMaxPingTime = p2pMax; P2PAvgPingTime = p2pAvg; P2PAvgPingTimeStdDev = p2pSd; P2PAvgPingLossRate = p2pLoss;
        RelayMinPingTime = relMin; RelayMaxPingTime = relMax; RelayAvgPingTime = relAvg; RelayAvgPingTimeStdDev = relSd; RelayAvgPingLossRate = relLoss;
        UdpMinPingTime = udpMin; UdpMaxPingTime = udpMax; UdpAvgPingTime = udpAvg; UdpAvgPingTimeStdDev = udpSd; UdpAvgPingLossRate = udpLoss;
        TcpMinPingTime = tcpMin; TcpMaxPingTime = tcpMax; TcpAvgPingTime = tcpAvg; TcpAvgPingTimeStdDev = tcpSd;
        UdpSendCountPerSecHost = uscH; UdpSendAmountPerSecHost = usaH; UdpRecvCountPerSecHost = urcH; UdpRecvAmountPerSecHost = uraH;
        UdpSendCountPerSecNonHost = uscN; UdpSendAmountPerSecNonHost = usaN; UdpRecvCountPerSecNonHost = urcN; UdpRecvAmountPerSecNonHost = uraN;
        UdpUserSyncPacketLossRate = uSync; UdpNpcSyncPacketLossRate = nSync;
        NonHostNpcReactionHostUdpAvgPingTime = npcPing;
        NonHostNpcReactionMinDelayTimeSimulOff = offMin; NonHostNpcReactionMaxDelayTimeSimulOff = offMax;
        NonHostNpcReactionAvgDelayTimeSimulOff = offAvg; NonHostNpcReactionAvgDelayTimeStdDevSimulOff = offSd;
        NonHostNpcReactionMinDelayTimeSimulOn = onMin; NonHostNpcReactionMaxDelayTimeSimulOn = onMax;
        NonHostNpcReactionAvgDelayTimeSimulOn = onAvg; NonHostNpcReactionAvgDelayTimeStdDevSimulOn = onSd;
        FirstMemUsageCheck = fmuc; MinWorkingSetSize = minWs; MaxWorkingSetSize = maxWs;
        MinPagefileUsage = minPf; MaxPagefileUsage = maxPf; PartyType = pt; PartyUserNum = pun; MapId = mid; FieldUserNum = fun;
        return true;
    }
}

public sealed class KEGS_KOG_GAME_PERFORMANCE_DUNGEON_LOG : IKSerializable
{
    public long CharUid { get; set; } = -1;
    public long SystemKey { get; set; }
    public float AvgFps { get; set; } = -1f;
    public float AvgFpsStdDev { get; set; } = -1f;
    public float P2PMinPingTime { get; set; } = -1f;
    public float P2PMaxPingTime { get; set; } = -1f;
    public float P2PAvgPingTime { get; set; } = -1f;
    public float P2PAvgPingTimeStdDev { get; set; } = -1f;
    public float P2PAvgPingLossRate { get; set; } = -1f;
    public float RelayMinPingTime { get; set; } = -1f;
    public float RelayMaxPingTime { get; set; } = -1f;
    public float RelayAvgPingTime { get; set; } = -1f;
    public float RelayAvgPingTimeStdDev { get; set; } = -1f;
    public float RelayAvgPingLossRate { get; set; } = -1f;
    public float UdpMinPingTime { get; set; } = -1f;
    public float UdpMaxPingTime { get; set; } = -1f;
    public float UdpAvgPingTime { get; set; } = -1f;
    public float UdpAvgPingTimeStdDev { get; set; } = -1f;
    public float UdpAvgPingLossRate { get; set; } = -1f;
    public float TcpMinPingTime { get; set; } = -1f;
    public float TcpMaxPingTime { get; set; } = -1f;
    public float TcpAvgPingTime { get; set; } = -1f;
    public float TcpAvgPingTimeStdDev { get; set; } = -1f;
    public float UdpSendCountPerSecHost { get; set; } = -1f;
    public float UdpSendAmountPerSecHost { get; set; } = -1f;
    public float UdpRecvCountPerSecHost { get; set; } = -1f;
    public float UdpRecvAmountPerSecHost { get; set; } = -1f;
    public float UdpSendCountPerSecNonHost { get; set; } = -1f;
    public float UdpSendAmountPerSecNonHost { get; set; } = -1f;
    public float UdpRecvCountPerSecNonHost { get; set; } = -1f;
    public float UdpRecvAmountPerSecNonHost { get; set; } = -1f;
    public float UdpUserSyncPacketLossRate { get; set; } = -1f;
    public float UdpNpcSyncPacketLossRate { get; set; } = -1f;
    public float NonHostNpcReactionHostUdpAvgPingTime { get; set; } = -1f;
    public float NonHostNpcReactionMinDelayTimeSimulOff { get; set; } = -1f;
    public float NonHostNpcReactionMaxDelayTimeSimulOff { get; set; } = -1f;
    public float NonHostNpcReactionAvgDelayTimeSimulOff { get; set; } = -1f;
    public float NonHostNpcReactionAvgDelayTimeStdDevSimulOff { get; set; } = -1f;
    public float NonHostNpcReactionMinDelayTimeSimulOn { get; set; } = -1f;
    public float NonHostNpcReactionMaxDelayTimeSimulOn { get; set; } = -1f;
    public float NonHostNpcReactionAvgDelayTimeSimulOn { get; set; } = -1f;
    public float NonHostNpcReactionAvgDelayTimeStdDevSimulOn { get; set; } = -1f;
    public bool FirstMemUsageCheck { get; set; } = true;
    public float MinWorkingSetSize { get; set; } = -1f;
    public float MaxWorkingSetSize { get; set; } = -1f;
    public float MinPagefileUsage { get; set; } = -1f;
    public float MaxPagefileUsage { get; set; } = -1f;
    public int PartyType { get; set; } = -1;
    public int PartyUserNum { get; set; } = -1;
    public int MapId { get; set; } = -1;
    public int Difficulty { get; set; } = -1;
    public bool Escape { get; set; } = true;
    public int Stage { get; set; } = -1;
    public int Block { get; set; } = -1;
    public KDungeonLagInfo UnknownLagInfo { get; set; } = new();
    public KDungeonLagInfo KnownLagInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(CharUid) && ks.Put(SystemKey) && ks.Put(AvgFps) && ks.Put(AvgFpsStdDev) &&
        ks.Put(P2PMinPingTime) && ks.Put(P2PMaxPingTime) && ks.Put(P2PAvgPingTime) && ks.Put(P2PAvgPingTimeStdDev) && ks.Put(P2PAvgPingLossRate) &&
        ks.Put(RelayMinPingTime) && ks.Put(RelayMaxPingTime) && ks.Put(RelayAvgPingTime) && ks.Put(RelayAvgPingTimeStdDev) && ks.Put(RelayAvgPingLossRate) &&
        ks.Put(UdpMinPingTime) && ks.Put(UdpMaxPingTime) && ks.Put(UdpAvgPingTime) && ks.Put(UdpAvgPingTimeStdDev) && ks.Put(UdpAvgPingLossRate) &&
        ks.Put(TcpMinPingTime) && ks.Put(TcpMaxPingTime) && ks.Put(TcpAvgPingTime) && ks.Put(TcpAvgPingTimeStdDev) &&
        ks.Put(UdpSendCountPerSecHost) && ks.Put(UdpSendAmountPerSecHost) && ks.Put(UdpRecvCountPerSecHost) && ks.Put(UdpRecvAmountPerSecHost) &&
        ks.Put(UdpSendCountPerSecNonHost) && ks.Put(UdpSendAmountPerSecNonHost) && ks.Put(UdpRecvCountPerSecNonHost) && ks.Put(UdpRecvAmountPerSecNonHost) &&
        ks.Put(UdpUserSyncPacketLossRate) && ks.Put(UdpNpcSyncPacketLossRate) &&
        ks.Put(NonHostNpcReactionHostUdpAvgPingTime) &&
        ks.Put(NonHostNpcReactionMinDelayTimeSimulOff) && ks.Put(NonHostNpcReactionMaxDelayTimeSimulOff) &&
        ks.Put(NonHostNpcReactionAvgDelayTimeSimulOff) && ks.Put(NonHostNpcReactionAvgDelayTimeStdDevSimulOff) &&
        ks.Put(NonHostNpcReactionMinDelayTimeSimulOn) && ks.Put(NonHostNpcReactionMaxDelayTimeSimulOn) &&
        ks.Put(NonHostNpcReactionAvgDelayTimeSimulOn) && ks.Put(NonHostNpcReactionAvgDelayTimeStdDevSimulOn) &&
        ks.Put(FirstMemUsageCheck) && ks.Put(MinWorkingSetSize) && ks.Put(MaxWorkingSetSize) &&
        ks.Put(MinPagefileUsage) && ks.Put(MaxPagefileUsage) && ks.Put(PartyType) &&
        ks.Put(PartyUserNum) && ks.Put(MapId) && ks.Put(Difficulty) && ks.Put(Escape) && ks.Put(Stage) && ks.Put(Block) &&
        ks.Put(UnknownLagInfo) && ks.Put(KnownLagInfo);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long cuid) || !ks.Get(out long sk) || !ks.Get(out float fps) || !ks.Get(out float sd) ||
            !ks.Get(out float p2pMin) || !ks.Get(out float p2pMax) || !ks.Get(out float p2pAvg) || !ks.Get(out float p2pSd) || !ks.Get(out float p2pLoss) ||
            !ks.Get(out float relMin) || !ks.Get(out float relMax) || !ks.Get(out float relAvg) || !ks.Get(out float relSd) || !ks.Get(out float relLoss) ||
            !ks.Get(out float udpMin) || !ks.Get(out float udpMax) || !ks.Get(out float udpAvg) || !ks.Get(out float udpSd) || !ks.Get(out float udpLoss) ||
            !ks.Get(out float tcpMin) || !ks.Get(out float tcpMax) || !ks.Get(out float tcpAvg) || !ks.Get(out float tcpSd) ||
            !ks.Get(out float uscH) || !ks.Get(out float usaH) || !ks.Get(out float urcH) || !ks.Get(out float uraH) ||
            !ks.Get(out float uscN) || !ks.Get(out float usaN) || !ks.Get(out float urcN) || !ks.Get(out float uraN) ||
            !ks.Get(out float uSync) || !ks.Get(out float nSync) ||
            !ks.Get(out float npcPing) ||
            !ks.Get(out float offMin) || !ks.Get(out float offMax) || !ks.Get(out float offAvg) || !ks.Get(out float offSd) ||
            !ks.Get(out float onMin) || !ks.Get(out float onMax) || !ks.Get(out float onAvg) || !ks.Get(out float onSd) ||
            !ks.Get(out bool fmuc) || !ks.Get(out float minWs) || !ks.Get(out float maxWs) ||
            !ks.Get(out float minPf) || !ks.Get(out float maxPf) || !ks.Get(out int pt) ||
            !ks.Get(out int pun) || !ks.Get(out int mid) || !ks.Get(out int diff) || !ks.Get(out bool esc) ||
            !ks.Get(out int stg) || !ks.Get(out int blk) ||
            !ks.Get(UnknownLagInfo) || !ks.Get(KnownLagInfo))
            return false;
        CharUid = cuid; SystemKey = sk; AvgFps = fps; AvgFpsStdDev = sd;
        P2PMinPingTime = p2pMin; P2PMaxPingTime = p2pMax; P2PAvgPingTime = p2pAvg; P2PAvgPingTimeStdDev = p2pSd; P2PAvgPingLossRate = p2pLoss;
        RelayMinPingTime = relMin; RelayMaxPingTime = relMax; RelayAvgPingTime = relAvg; RelayAvgPingTimeStdDev = relSd; RelayAvgPingLossRate = relLoss;
        UdpMinPingTime = udpMin; UdpMaxPingTime = udpMax; UdpAvgPingTime = udpAvg; UdpAvgPingTimeStdDev = udpSd; UdpAvgPingLossRate = udpLoss;
        TcpMinPingTime = tcpMin; TcpMaxPingTime = tcpMax; TcpAvgPingTime = tcpAvg; TcpAvgPingTimeStdDev = tcpSd;
        UdpSendCountPerSecHost = uscH; UdpSendAmountPerSecHost = usaH; UdpRecvCountPerSecHost = urcH; UdpRecvAmountPerSecHost = uraH;
        UdpSendCountPerSecNonHost = uscN; UdpSendAmountPerSecNonHost = usaN; UdpRecvCountPerSecNonHost = urcN; UdpRecvAmountPerSecNonHost = uraN;
        UdpUserSyncPacketLossRate = uSync; UdpNpcSyncPacketLossRate = nSync;
        NonHostNpcReactionHostUdpAvgPingTime = npcPing;
        NonHostNpcReactionMinDelayTimeSimulOff = offMin; NonHostNpcReactionMaxDelayTimeSimulOff = offMax;
        NonHostNpcReactionAvgDelayTimeSimulOff = offAvg; NonHostNpcReactionAvgDelayTimeStdDevSimulOff = offSd;
        NonHostNpcReactionMinDelayTimeSimulOn = onMin; NonHostNpcReactionMaxDelayTimeSimulOn = onMax;
        NonHostNpcReactionAvgDelayTimeSimulOn = onAvg; NonHostNpcReactionAvgDelayTimeStdDevSimulOn = onSd;
        FirstMemUsageCheck = fmuc; MinWorkingSetSize = minWs; MaxWorkingSetSize = maxWs;
        MinPagefileUsage = minPf; MaxPagefileUsage = maxPf; PartyType = pt; PartyUserNum = pun; MapId = mid;
        Difficulty = diff; Escape = esc; Stage = stg; Block = blk;
        return true;
    }
}

public sealed class KEGS_KOG_GAME_PERFORMANCE_PVP_LOG : IKSerializable
{
    public long CharUid { get; set; } = -1;
    public long SystemKey { get; set; }
    public float AvgFps { get; set; } = -1f;
    public float AvgFpsStdDev { get; set; } = -1f;
    public float P2PMinPingTime { get; set; } = -1f;
    public float P2PMaxPingTime { get; set; } = -1f;
    public float P2PAvgPingTime { get; set; } = -1f;
    public float P2PAvgPingTimeStdDev { get; set; } = -1f;
    public float P2PAvgPingLossRate { get; set; } = -1f;
    public float RelayMinPingTime { get; set; } = -1f;
    public float RelayMaxPingTime { get; set; } = -1f;
    public float RelayAvgPingTime { get; set; } = -1f;
    public float RelayAvgPingTimeStdDev { get; set; } = -1f;
    public float RelayAvgPingLossRate { get; set; } = -1f;
    public float UdpMinPingTime { get; set; } = -1f;
    public float UdpMaxPingTime { get; set; } = -1f;
    public float UdpAvgPingTime { get; set; } = -1f;
    public float UdpAvgPingTimeStdDev { get; set; } = -1f;
    public float UdpAvgPingLossRate { get; set; } = -1f;
    public float TcpMinPingTime { get; set; } = -1f;
    public float TcpMaxPingTime { get; set; } = -1f;
    public float TcpAvgPingTime { get; set; } = -1f;
    public float TcpAvgPingTimeStdDev { get; set; } = -1f;
    public float UdpSendCountPerSecHost { get; set; } = -1f;
    public float UdpSendAmountPerSecHost { get; set; } = -1f;
    public float UdpRecvCountPerSecHost { get; set; } = -1f;
    public float UdpRecvAmountPerSecHost { get; set; } = -1f;
    public float UdpSendCountPerSecNonHost { get; set; } = -1f;
    public float UdpSendAmountPerSecNonHost { get; set; } = -1f;
    public float UdpRecvCountPerSecNonHost { get; set; } = -1f;
    public float UdpRecvAmountPerSecNonHost { get; set; } = -1f;
    public float UdpUserSyncPacketLossRate { get; set; } = -1f;
    public float UdpNpcSyncPacketLossRate { get; set; } = -1f;
    public bool FirstMemUsageCheck { get; set; } = true;
    public float MinWorkingSetSize { get; set; } = -1f;
    public float MaxWorkingSetSize { get; set; } = -1f;
    public float MinPagefileUsage { get; set; } = -1f;
    public float MaxPagefileUsage { get; set; } = -1f;
    public int MapId { get; set; } = -1;
    public int PvpUserNum { get; set; } = -1;
    public int RuleType { get; set; } = -1;
    public bool Escape { get; set; } = true;
    public int Round { get; set; } = -1;
    public KPvPLagInfo UnknownLagInfo { get; set; } = new();
    public KPvPLagInfo KnownLagInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(CharUid) && ks.Put(SystemKey) && ks.Put(AvgFps) && ks.Put(AvgFpsStdDev) &&
        ks.Put(P2PMinPingTime) && ks.Put(P2PMaxPingTime) && ks.Put(P2PAvgPingTime) && ks.Put(P2PAvgPingTimeStdDev) && ks.Put(P2PAvgPingLossRate) &&
        ks.Put(RelayMinPingTime) && ks.Put(RelayMaxPingTime) && ks.Put(RelayAvgPingTime) && ks.Put(RelayAvgPingTimeStdDev) && ks.Put(RelayAvgPingLossRate) &&
        ks.Put(UdpMinPingTime) && ks.Put(UdpMaxPingTime) && ks.Put(UdpAvgPingTime) && ks.Put(UdpAvgPingTimeStdDev) && ks.Put(UdpAvgPingLossRate) &&
        ks.Put(TcpMinPingTime) && ks.Put(TcpMaxPingTime) && ks.Put(TcpAvgPingTime) && ks.Put(TcpAvgPingTimeStdDev) &&
        ks.Put(UdpSendCountPerSecHost) && ks.Put(UdpSendAmountPerSecHost) && ks.Put(UdpRecvCountPerSecHost) && ks.Put(UdpRecvAmountPerSecHost) &&
        ks.Put(UdpSendCountPerSecNonHost) && ks.Put(UdpSendAmountPerSecNonHost) && ks.Put(UdpRecvCountPerSecNonHost) && ks.Put(UdpRecvAmountPerSecNonHost) &&
        ks.Put(UdpUserSyncPacketLossRate) && ks.Put(UdpNpcSyncPacketLossRate) &&
        ks.Put(FirstMemUsageCheck) && ks.Put(MinWorkingSetSize) && ks.Put(MaxWorkingSetSize) &&
        ks.Put(MinPagefileUsage) && ks.Put(MaxPagefileUsage) &&
        ks.Put(MapId) && ks.Put(PvpUserNum) && ks.Put(RuleType) && ks.Put(Escape) && ks.Put(Round) &&
        ks.Put(UnknownLagInfo) && ks.Put(KnownLagInfo);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long cuid) || !ks.Get(out long sk) || !ks.Get(out float fps) || !ks.Get(out float sd) ||
            !ks.Get(out float p2pMin) || !ks.Get(out float p2pMax) || !ks.Get(out float p2pAvg) || !ks.Get(out float p2pSd) || !ks.Get(out float p2pLoss) ||
            !ks.Get(out float relMin) || !ks.Get(out float relMax) || !ks.Get(out float relAvg) || !ks.Get(out float relSd) || !ks.Get(out float relLoss) ||
            !ks.Get(out float udpMin) || !ks.Get(out float udpMax) || !ks.Get(out float udpAvg) || !ks.Get(out float udpSd) || !ks.Get(out float udpLoss) ||
            !ks.Get(out float tcpMin) || !ks.Get(out float tcpMax) || !ks.Get(out float tcpAvg) || !ks.Get(out float tcpSd) ||
            !ks.Get(out float uscH) || !ks.Get(out float usaH) || !ks.Get(out float urcH) || !ks.Get(out float uraH) ||
            !ks.Get(out float uscN) || !ks.Get(out float usaN) || !ks.Get(out float urcN) || !ks.Get(out float uraN) ||
            !ks.Get(out float uSync) || !ks.Get(out float nSync) ||
            !ks.Get(out bool fmuc) || !ks.Get(out float minWs) || !ks.Get(out float maxWs) ||
            !ks.Get(out float minPf) || !ks.Get(out float maxPf) ||
            !ks.Get(out int mid) || !ks.Get(out int pun) || !ks.Get(out int rt) || !ks.Get(out bool esc) || !ks.Get(out int rnd) ||
            !ks.Get(UnknownLagInfo) || !ks.Get(KnownLagInfo))
            return false;
        CharUid = cuid; SystemKey = sk; AvgFps = fps; AvgFpsStdDev = sd;
        P2PMinPingTime = p2pMin; P2PMaxPingTime = p2pMax; P2PAvgPingTime = p2pAvg; P2PAvgPingTimeStdDev = p2pSd; P2PAvgPingLossRate = p2pLoss;
        RelayMinPingTime = relMin; RelayMaxPingTime = relMax; RelayAvgPingTime = relAvg; RelayAvgPingTimeStdDev = relSd; RelayAvgPingLossRate = relLoss;
        UdpMinPingTime = udpMin; UdpMaxPingTime = udpMax; UdpAvgPingTime = udpAvg; UdpAvgPingTimeStdDev = udpSd; UdpAvgPingLossRate = udpLoss;
        TcpMinPingTime = tcpMin; TcpMaxPingTime = tcpMax; TcpAvgPingTime = tcpAvg; TcpAvgPingTimeStdDev = tcpSd;
        UdpSendCountPerSecHost = uscH; UdpSendAmountPerSecHost = usaH; UdpRecvCountPerSecHost = urcH; UdpRecvAmountPerSecHost = uraH;
        UdpSendCountPerSecNonHost = uscN; UdpSendAmountPerSecNonHost = usaN; UdpRecvCountPerSecNonHost = urcN; UdpRecvAmountPerSecNonHost = uraN;
        UdpUserSyncPacketLossRate = uSync; UdpNpcSyncPacketLossRate = nSync;
        FirstMemUsageCheck = fmuc; MinWorkingSetSize = minWs; MaxWorkingSetSize = maxWs;
        MinPagefileUsage = minPf; MaxPagefileUsage = maxPf;
        MapId = mid; PvpUserNum = pun; RuleType = rt; Escape = esc; Round = rnd;
        return true;
    }
}

public sealed class KEGS_GET_ACTIVE_LAGCHECK_ACK : IKSerializable
{
    public bool ActiveLagCheck { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(ActiveLagCheck);
    public bool Get(KSerializer ks, RegionProfile region) { if (!ks.Get(out bool b)) return false; ActiveLagCheck = b; return true; }
}

public sealed class KEGS_CLIENT_DIRECT_CONNECT_AUTH_PCBAG_REQ : IKSerializable
{
    public bool CheckClientDirectPcBang { get; set; }
    public int IsGameBangType { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(CheckClientDirectPcBang) && ks.Put(IsGameBangType);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool c) || !ks.Get(out int t)) return false;
        CheckClientDirectPcBang = c; IsGameBangType = t;
        return true;
    }
}

public sealed class KEGS_CLIENT_DIRECT_CONNECT_AUTH_PCBAG_ACK : IKSerializable
{
    public bool CheckClientDirectPcBang { get; set; }
    public int IsGameBangType { get; set; }
    public KPcBangReward PcBangReward { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(CheckClientDirectPcBang) && ks.Put(IsGameBangType) && ks.Put(PcBangReward);

    public bool Get(KSerializer ks, RegionProfile region) =>
        ks.Get(out bool c) && (CheckClientDirectPcBang = c || true) &&
        ks.Get(out int t) && (IsGameBangType = t) >= int.MinValue &&
        ks.Get(PcBangReward);
}

// -------- Security (13-handler parity) --------

public sealed class KEGS_AGREE_HACK_USER_ACK : IKSerializable
{
    public int Ok { get; set; }
    public sbyte HackingUserType { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.Put(HackingUserType);
    public bool Get(KSerializer ks, RegionProfile region) { if (!ks.Get(out int ok) || !ks.Get(out sbyte t)) return false; Ok = ok; HackingUserType = t; return true; }
}

public sealed class KEGS_DUNGEON_KILLALLNPC_CHECK_ACK : IKSerializable
{
    public bool Result { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Result);
    public bool Get(KSerializer ks, RegionProfile region) { if (!ks.Get(out bool r)) return false; Result = r; return true; }
}

public sealed class KEGS_HACKING_USER_PROCESS_NAME_RECORD_NOT : IKSerializable
{
    public byte Type { get; set; }
    public string Reason { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Type) && ks.PutWideString(Reason);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out byte t) || !ks.GetWideString(out var r)) return false;
        Type = t; Reason = r;
        return true;
    }
}

public sealed class KEGS_HACKSHIELD_CHECK_REQ : IKSerializable
{
    public List<byte> Data { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region) => ks.PutList(Data, static (s, b) => s.Put(b));
    public bool Get(KSerializer ks, RegionProfile region) => ks.GetList(Data, static s => (s.Get(out byte b), b));
}

public sealed class KEGS_HACKSHIELD_CHECK_ACK : IKSerializable
{
    public List<byte> Data { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region) => ks.PutList(Data, static (s, b) => s.Put(b));
    public bool Get(KSerializer ks, RegionProfile region) => ks.GetList(Data, static s => (s.Get(out byte b), b));
}

public sealed class KEGS_KNPROTECT_AUTH_CHECK_NOT : IKSerializable
{
    public uint Index { get; set; }
    public uint Value1 { get; set; }
    public uint Value2 { get; set; }
    public uint Value3 { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Index) && ks.Put(Value1) && ks.Put(Value2) && ks.Put(Value3);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint i) || !ks.Get(out uint v1) || !ks.Get(out uint v2) || !ks.Get(out uint v3))
            return false;
        Index = i; Value1 = v1; Value2 = v2; Value3 = v3;
        return true;
    }
}

public sealed class KEGS_MODULE_INFO_UPDATE_NOT : IKSerializable
{
    public List<string> ModuleName { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region) => ks.PutList(ModuleName, static (s, v) => s.PutWideString(v));
    public bool Get(KSerializer ks, RegionProfile region) => ks.GetList(ModuleName, static s => (s.GetWideString(out var v), v));
}

public sealed class KEGS_NPROTECT_CSAUTH3_CHECK_NOT : IKSerializable
{
    public List<byte> CsAuth3 { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region) => ks.PutList(CsAuth3, static (s, b) => s.Put(b));
    public bool Get(KSerializer ks, RegionProfile region) => ks.GetList(CsAuth3, static s => (s.Get(out byte b), b));
}

public sealed class KEGS_SERVER_CHECK_HACK_USER_ACK : IKSerializable
{
    public sbyte HackingCheckType { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(HackingCheckType);
    public bool Get(KSerializer ks, RegionProfile region) { if (!ks.Get(out sbyte t)) return false; HackingCheckType = t; return true; }
}

public sealed class KEGS_XTRAP_REQ : IKSerializable
{
    public List<byte> Data { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region) => ks.PutList(Data, static (s, b) => s.Put(b));
    public bool Get(KSerializer ks, RegionProfile region) => ks.GetList(Data, static s => (s.Get(out byte b), b));
}

public sealed class KEGS_XTRAP_ACK : IKSerializable
{
    public List<byte> Data { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region) => ks.PutList(Data, static (s, b) => s.Put(b));
    public bool Get(KSerializer ks, RegionProfile region) => ks.GetList(Data, static s => (s.Get(out byte b), b));
}

// -------- Inventory / Synthesis / Evaluate / ShareBank / Extract / Finality / Event Relationship (19-handler parity) --------

public sealed class KEGS_USE_DEFENCE_DUNGEON_QUICK_SLOT_REQ : IKSerializable
{
    public short SlotId { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(SlotId);
    public bool Get(KSerializer ks, RegionProfile region) { if (!ks.Get(out short s)) return false; SlotId = s; return true; }
}

public sealed class KEGS_USE_DEFENCE_DUNGEON_QUICK_SLOT_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KInventoryItemInfo InventorySlotInfo { get; set; } = new();
    public int ItemId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(InventorySlotInfo) && ks.Put(ItemId);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(InventorySlotInfo) || !ks.Get(out int iid)) return false;
        Ok = ok; ItemId = iid;
        return true;
    }
}

public sealed class KEGS_OUT_USER_RETAINING_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(UnitUid);
    public bool Get(KSerializer ks, RegionProfile region) { if (!ks.Get(out long u)) return false; UnitUid = u; return true; }
}

public sealed class KEGS_OUT_USER_RETAINING_ACK : IKSerializable
{
    public bool DepartureUser { get; set; }
    public Dictionary<int, KRewardItem> SlotItem { get; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(DepartureUser) && ks.PutMap(SlotItem, static (s, k) => s.Put(k), static (s, v) => s.Put(v));

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool dep)) return false;
        DepartureUser = dep;
        return ks.GetMap(SlotItem, static s => (s.Get(out int k), k), static s => { var v = new KRewardItem(); return (s.Get(v), v); });
    }
}

public sealed class KEGS_RETAINING_SELECT_REWARD_REQ : IKSerializable
{
    public int SlotId { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(SlotId);
    public bool Get(KSerializer ks, RegionProfile region) { if (!ks.Get(out int s)) return false; SlotId = s; return true; }
}

public sealed class KEGS_RETAINING_SELECT_REWARD_ACK : IKSerializable
{
    public int Ok { get; set; }
    public bool IsPost { get; set; }
    public KRewardItem RewardItem { get; set; } = new();
    public int ResurrectionStone { get; set; }
    public List<KInventoryItemInfo> InventorySlotInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(IsPost) && ks.Put(RewardItem) && ks.Put(ResurrectionStone) && ks.PutList(InventorySlotInfo);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out bool post) || !ks.Get(RewardItem) ||
            !ks.Get(out int rs) || !ks.GetList(InventorySlotInfo))
            return false;
        Ok = ok; IsPost = post; ResurrectionStone = rs;
        return true;
    }
}

public sealed class KEGS_RESOLVE_ITEM_AVATAR_REQ : IKSerializable
{
    public Dictionary<int, long> ResolveDataAvatar { get; } = new();
    public int AttachFee { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.PutMap(ResolveDataAvatar, static (s, k) => s.Put(k), static (s, v) => s.Put(v)) && ks.Put(AttachFee);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetMap(ResolveDataAvatar, static s => (s.Get(out int k), k), static s => (s.Get(out long v), v)) ||
            !ks.Get(out int fee))
            return false;
        AttachFee = fee;
        return true;
    }
}

public sealed class KEGS_GET_SHARE_BANK_REQ : IKSerializable
{
    public string NickName { get; set; } = string.Empty;
    public int NewUnitTradeBlockDay { get; set; }
    public int NewUnitTradeBlockUnitClass { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.PutWideString(NickName) && ks.Put(NewUnitTradeBlockDay) && ks.Put(NewUnitTradeBlockUnitClass);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out var nick) || !ks.Get(out int day) || !ks.Get(out int uc)) return false;
        NickName = nick; NewUnitTradeBlockDay = day; NewUnitTradeBlockUnitClass = uc;
        return true;
    }
}

public sealed class KEGS_GET_SHARE_BANK_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long UnitUid { get; set; }
    public string NickName { get; set; } = string.Empty;
    public int BankSize { get; set; }
    public Dictionary<long, KInventoryItemInfo> Item { get; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(UnitUid) && ks.PutWideString(NickName) && ks.Put(BankSize) &&
        ks.PutMap(Item, static (s, k) => s.Put(k), static (s, v) => s.Put(v));

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out long uid) || !ks.GetWideString(out var nick) || !ks.Get(out int bs) ||
            !ks.GetMap(Item, static s => (s.Get(out long k), k), static s => { var v = new KInventoryItemInfo(); return (s.Get(v), v); }))
            return false;
        Ok = ok; UnitUid = uid; NickName = nick; BankSize = bs;
        return true;
    }
}

public sealed class KEGS_ITEM_CONVERT_REQ : IKSerializable
{
    public long ItemUid { get; set; }
    public int Quantity { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(ItemUid) && ks.Put(Quantity);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.Get(out int qty)) return false;
        ItemUid = uid; Quantity = qty;
        return true;
    }
}

public sealed class KEGS_ITEM_CONVERT_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int Ed { get; set; }
    public List<KInventoryItemInfo> InventorySlotInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(Ed) && ks.PutList(InventorySlotInfo);

    public bool Get(KSerializer ks, RegionProfile region) =>
        ks.Get(out int ok) && (Ok = ok) >= int.MinValue &&
        ks.Get(out int ed) && (Ed = ed) >= int.MinValue &&
        ks.GetList(InventorySlotInfo);
}

public sealed class KEGS_ITEM_EVALUATE_REQ : IKSerializable
{
    public long ItemUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(ItemUid);
    public bool Get(KSerializer ks, RegionProfile region) { if (!ks.Get(out long uid)) return false; ItemUid = uid; return true; }
}

public sealed class KEGS_ITEM_EVALUATE_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int Ed { get; set; }
    public List<KInventoryItemInfo> InventorySlotInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(Ed) && ks.PutList(InventorySlotInfo);

    public bool Get(KSerializer ks, RegionProfile region) =>
        ks.Get(out int ok) && (Ok = ok) >= int.MinValue &&
        ks.Get(out int ed) && (Ed = ed) >= int.MinValue &&
        ks.GetList(InventorySlotInfo);
}

public sealed class KEGS_RESTORE_ITEM_EVALUATE_REQ : IKSerializable
{
    public long SupportItemUid { get; set; }
    public long TargetItemUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(SupportItemUid) && ks.Put(TargetItemUid);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long s) || !ks.Get(out long t)) return false;
        SupportItemUid = s; TargetItemUid = t;
        return true;
    }
}

public sealed class KEGS_RESTORE_ITEM_EVALUATE_ACK : IKSerializable
{
    public int Ok { get; set; }
    public List<KInventoryItemInfo> InventorySlotInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.PutList(InventorySlotInfo);
    public bool Get(KSerializer ks, RegionProfile region) =>
        ks.Get(out int ok) && (Ok = ok) >= int.MinValue && ks.GetList(InventorySlotInfo);
}

public sealed class KEGS_OPEN_RANDOM_ITEM_DEVELOPER_REQ : IKSerializable
{
    public int ItemId { get; set; }
    public int ItemKeyId { get; set; }
    public int OpenCount { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(ItemId) && ks.Put(ItemKeyId) && ks.Put(OpenCount);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int id) || !ks.Get(out int key) || !ks.Get(out int cnt)) return false;
        ItemId = id; ItemKeyId = key; OpenCount = cnt;
        return true;
    }
}

public sealed class KEGS_OPEN_RANDOM_ITEM_DEVELOPER_ACK : IKSerializable
{
    public int Ok { get; set; }
    public Dictionary<int, KRandomItemResult> RandomItemResult { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.PutMap(RandomItemResult, static (s, k) => s.Put(k), static (s, v) => s.Put(v));

    public bool Get(KSerializer ks, RegionProfile region) =>
        ks.Get(out int ok) && (Ok = ok) >= int.MinValue &&
        ks.GetMap(RandomItemResult, static s => (s.Get(out int k), k), static s => { var v = new KRandomItemResult(); return (s.Get(v), v); });
}

public sealed class KEGS_CUBE_IN_ITEM_MAPPING_NOT : IKSerializable
{
    public Dictionary<int, Dictionary<int, KRandomItemMappingToInfoClient>> CubeInItemMapping { get; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.PutMap(CubeInItemMapping, static (s, k) => s.Put(k),
            static (s, inner) => s.PutMap(inner, static (s2, k2) => s2.Put(k2), static (s2, v2) => s2.Put(v2)));

    public bool Get(KSerializer ks, RegionProfile region) =>
        ks.GetMap(CubeInItemMapping, static s => (s.Get(out int k), k), static s =>
        {
            var inner = new Dictionary<int, KRandomItemMappingToInfoClient>();
            bool ok = s.GetMap(inner, static s2 => (s2.Get(out int k2), k2),
                static s2 => { var v2 = new KRandomItemMappingToInfoClient(); return (s2.Get(v2), v2); });
            return (ok, inner);
        });
}

public sealed class KEGS_SYNTHESIS_ITEM_REQ : IKSerializable
{
    public Dictionary<int, long> SynthesisData { get; } = new();
    public long KeyData { get; set; }
    public int SocketNo { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.PutMap(SynthesisData, static (s, k) => s.Put(k), static (s, v) => s.Put(v)) &&
        ks.Put(KeyData) && ks.Put(SocketNo);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetMap(SynthesisData, static s => (s.Get(out int k), k), static s => (s.Get(out long v), v)) ||
            !ks.Get(out long kd) || !ks.Get(out int sn))
            return false;
        KeyData = kd; SocketNo = sn;
        return true;
    }
}

public sealed class KEGS_SYNTHESIS_ITEM_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int ResurrectionCount { get; set; }
    public int RestoreSpirit { get; set; }
    public Dictionary<int, int> InsertedItem { get; } = new();
    public List<KInventoryItemInfo> InventorySlotInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(ResurrectionCount) && ks.Put(RestoreSpirit) &&
        ks.PutMap(InsertedItem, static (s, k) => s.Put(k), static (s, v) => s.Put(v)) &&
        ks.PutList(InventorySlotInfo);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out int rc) || !ks.Get(out int rs) ||
            !ks.GetMap(InsertedItem, static s => (s.Get(out int k), k), static s => (s.Get(out int v), v)) ||
            !ks.GetList(InventorySlotInfo))
            return false;
        Ok = ok; ResurrectionCount = rc; RestoreSpirit = rs;
        return true;
    }
}

public sealed class KEGS_SYNTHESIS_SOCKET_GROUPID_ACK : IKSerializable
{
    public int Ok { get; set; }
    public Dictionary<int, int> AvatarSocketOptionGroupId { get; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.PutMap(AvatarSocketOptionGroupId, static (s, k) => s.Put(k), static (s, v) => s.Put(v));

    public bool Get(KSerializer ks, RegionProfile region) =>
        ks.Get(out int ok) && (Ok = ok) >= int.MinValue &&
        ks.GetMap(AvatarSocketOptionGroupId, static s => (s.Get(out int k), k), static s => (s.Get(out int v), v));
}

public sealed class KEGS_READY_TO_SOSUN_EVENT_ACK : IKSerializable
{
    public int Ok { get; set; }
    public List<KInventoryItemInfo> InventorySlotInfo { get; set; } = new();
    public int FirstUnitClass { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.PutList(InventorySlotInfo) && ks.Put(FirstUnitClass);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.GetList(InventorySlotInfo) || !ks.Get(out int fc)) return false;
        Ok = ok; FirstUnitClass = fc;
        return true;
    }
}

public sealed class KEGS_USE_PROPOSE_ITEM_REQ : IKSerializable
{
    public long UsedItemUid { get; set; }
    public string NickName { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(UsedItemUid) && ks.PutWideString(NickName);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.GetWideString(out var nick)) return false;
        UsedItemUid = uid; NickName = nick;
        return true;
    }
}

public sealed class KEGS_USE_PROPOSE_ITEM_ACK : IKSerializable
{
    public int Ok { get; set; }
    public string NickName { get; set; } = string.Empty;
    public List<KInventoryItemInfo> InventorySlotInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.PutWideString(NickName) && ks.PutList(InventorySlotInfo);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.GetWideString(out var nick) || !ks.GetList(InventorySlotInfo)) return false;
        Ok = ok; NickName = nick;
        return true;
    }
}

public sealed class KEGS_USE_DIVORCE_ITEM_ACK : IKSerializable
{
    public int Ok { get; set; }
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.PutList(UpdatedInventorySlot);
    public bool Get(KSerializer ks, RegionProfile region) =>
        ks.Get(out int ok) && (Ok = ok) >= int.MinValue && ks.GetList(UpdatedInventorySlot);
}

public sealed class KEGS_USE_RECRUIT_TICKET_REQ : IKSerializable
{
    public long ItemUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(ItemUid);
    public bool Get(KSerializer ks, RegionProfile region) { if (!ks.Get(out long u)) return false; ItemUid = u; return true; }
}

public sealed class KEGS_USE_RECRUIT_TICKET_ACK : IKSerializable
{
    public int Ok { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok);
    public bool Get(KSerializer ks, RegionProfile region) { if (!ks.Get(out int ok)) return false; Ok = ok; return true; }
}

public sealed class KEGS_REGISTER_RECRUITER_REQ : IKSerializable
{
    public string NickName { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region) => ks.PutWideString(NickName);
    public bool Get(KSerializer ks, RegionProfile region) { if (!ks.GetWideString(out var n)) return false; NickName = n; return true; }
}

public sealed class KEGS_REGISTER_RECRUITER_ACK : IKSerializable
{
    public int Ok { get; set; }
    public List<KInventoryItemInfo> InventorySlotInfo { get; set; } = new();
    public int UsedItemId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.PutList(InventorySlotInfo) && ks.Put(UsedItemId);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.GetList(InventorySlotInfo) || !ks.Get(out int iid)) return false;
        Ok = ok; UsedItemId = iid;
        return true;
    }
}

public sealed class KEGS_REGISTER_RECRUITER_NOT : IKSerializable
{
    public long RecruiterUnitUid { get; set; }
    public KRecommendUserInfo RecruitUnitInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(RecruiterUnitUid) && ks.Put(RecruitUnitInfo);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.Get(RecruitUnitInfo)) return false;
        RecruiterUnitUid = uid;
        return true;
    }
}

public sealed class KEGS_ITEM_EXTRACT_REQ : IKSerializable
{
    public long ItemUid { get; set; }
    public int Quantity { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(ItemUid) && ks.Put(Quantity);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.Get(out int qty)) return false;
        ItemUid = uid; Quantity = qty;
        return true;
    }
}

public sealed class KEGS_ITEM_EXTRACT_ACK : IKSerializable
{
    public int Ok { get; set; }
    public List<KInventoryItemInfo> InventorySlotInfo { get; set; } = new();
    public Dictionary<int, int> InsertedItem { get; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.PutList(InventorySlotInfo) &&
        ks.PutMap(InsertedItem, static (s, k) => s.Put(k), static (s, v) => s.Put(v));

    public bool Get(KSerializer ks, RegionProfile region) =>
        ks.Get(out int ok) && (Ok = ok) >= int.MinValue &&
        ks.GetList(InventorySlotInfo) &&
        ks.GetMap(InsertedItem, static s => (s.Get(out int k), k), static s => (s.Get(out int v), v));
}

public sealed class KEGS_USE_FINALITY_SKILL_REQ : IKSerializable
{
    public long ItemUid { get; set; }
    public bool NoConsume { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(ItemUid) && ks.Put(NoConsume);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.Get(out bool nc)) return false;
        ItemUid = uid; NoConsume = nc;
        return true;
    }
}

public sealed class KEGS_USE_FINALITY_SKILL_ACK : IKSerializable
{
    public int Ok { get; set; }
    public List<KInventoryItemInfo> InventorySlotInfo { get; set; } = new();
    public bool NoConsume { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.PutList(InventorySlotInfo) && ks.Put(NoConsume);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.GetList(InventorySlotInfo) || !ks.Get(out bool nc)) return false;
        Ok = ok; NoConsume = nc;
        return true;
    }
}

// =========================================================================
// CashShop + CashShop_Global Packets (TW ServerDefine Parity)
// =========================================================================

public sealed class KEGS_APPLY_COUPON_REQ : IKSerializable
{
    public string CouponSerial { get; set; } = string.Empty;
    public int CouponType { get; set; } // SERV_GLOBAL_BILLING

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.PutWideString(CouponSerial) && ks.Put(CouponType);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out var serial) || !ks.Get(out int typ)) return false;
        CouponSerial = serial; CouponType = typ;
        return true;
    }
}

public sealed class KEGS_PURCHASED_CASH_ITEM_LIST_REQ : IKSerializable
{
    public int ItemPerPage { get; set; }
    public int CurrentPage { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(ItemPerPage) && ks.Put(CurrentPage);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ipp) || !ks.Get(out int cp)) return false;
        ItemPerPage = ipp; CurrentPage = cp;
        return true;
    }
}

public sealed class KEGS_PURCHASED_CASH_ITEM_LIST_ACK : IKSerializable
{
    public int Ok { get; set; }
    public List<KNXBTOrderInfo> OrderInfo { get; set; } = new();
    public List<KNXBTPackageInfo> PackageInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.PutList(OrderInfo) && ks.PutList(PackageInfo);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.GetList(OrderInfo) || !ks.GetList(PackageInfo)) return false;
        Ok = ok;
        return true;
    }
}

public sealed class KEGS_GET_PURCHASED_CASH_ITEM_REQ : IKSerializable
{
    public uint OrderNo { get; set; }
    public uint ProductNo { get; set; }
    public string ProductId { get; set; } = string.Empty;
    public ushort ProductExpire { get; set; }
    public ushort ProductPieces { get; set; }
    public byte ProductKind { get; set; }
    public int Quantity { get; set; } = 1;
    public Dictionary<int, int> SocketForCash { get; set; } = new();
    public Dictionary<int, int> SocketGroupIdForCash { get; set; } = new(); // SERV_CASH_ITEM_SOCKET_OPTION

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(OrderNo) && ks.Put(ProductNo) && ks.PutWideString(ProductId) &&
        ks.Put(ProductExpire) && ks.Put(ProductPieces) && ks.Put(ProductKind) &&
        ks.Put(Quantity) &&
        ks.PutMap(SocketForCash, static (s, k) => s.Put(k), static (s, v) => s.Put(v)) &&
        ks.PutMap(SocketGroupIdForCash, static (s, k) => s.Put(k), static (s, v) => s.Put(v));

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint ono) || !ks.Get(out uint pno) || !ks.GetWideString(out var pid) ||
            !ks.Get(out ushort exp) || !ks.Get(out ushort pcs) || !ks.Get(out byte pk) ||
            !ks.Get(out int qty) ||
            !ks.GetMap(SocketForCash, static s => (s.Get(out int k), k), static s => (s.Get(out int v), v)) ||
            !ks.GetMap(SocketGroupIdForCash, static s => (s.Get(out int k), k), static s => (s.Get(out int v), v)))
            return false;
        OrderNo = ono; ProductNo = pno; ProductId = pid; ProductExpire = exp;
        ProductPieces = pcs; ProductKind = pk; Quantity = qty;
        return true;
    }
}

public sealed class KEGS_GET_PURCHASED_CASH_ITEM_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int NumResurrectionStone { get; set; }
    public Dictionary<int, int> ExpandedCategorySlot { get; set; } = new();
    public List<KInventoryItemInfo> InventorySlotInfo { get; set; } = new();
    public int ExceptionProcessItemId { get; set; } // SERV_GUILD_CHANGE_NAME

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(NumResurrectionStone) &&
        ks.PutMap(ExpandedCategorySlot, static (s, k) => s.Put(k), static (s, v) => s.Put(v)) &&
        ks.PutList(InventorySlotInfo) && ks.Put(ExceptionProcessItemId);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out int stone) ||
            !ks.GetMap(ExpandedCategorySlot, static s => (s.Get(out int k), k), static s => (s.Get(out int v), v)) ||
            !ks.GetList(InventorySlotInfo) || !ks.Get(out int exId))
            return false;
        Ok = ok; NumResurrectionStone = stone; ExceptionProcessItemId = exId;
        return true;
    }
}

public sealed class KEGS_GET_PURCHASED_PACKAGE_CASH_ITEM_REQ : IKSerializable
{
    public uint OrderNo { get; set; }
    public uint PackageProductNo { get; set; }
    public string PackageProductId { get; set; } = string.Empty;
    public byte ProductKind { get; set; }
    public List<KNXBTPickUpPackageInfo> PickUpPackageInfo { get; set; } = new();
    public Dictionary<int, int> SocketForCash { get; set; } = new();
    public Dictionary<int, int> SocketGroupIdForCash { get; set; } = new(); // SERV_CASH_ITEM_SOCKET_OPTION
    public int ProductId { get; set; } // SERV_GLOBAL_CASH_PACKAGE

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(OrderNo) && ks.Put(PackageProductNo) && ks.PutWideString(PackageProductId) &&
        ks.Put(ProductKind) && ks.PutList(PickUpPackageInfo) &&
        ks.PutMap(SocketForCash, static (s, k) => s.Put(k), static (s, v) => s.Put(v)) &&
        ks.PutMap(SocketGroupIdForCash, static (s, k) => s.Put(k), static (s, v) => s.Put(v)) &&
        ks.Put(ProductId);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint ono) || !ks.Get(out uint ppno) || !ks.GetWideString(out var ppid) ||
            !ks.Get(out byte pk) || !ks.GetList(PickUpPackageInfo) ||
            !ks.GetMap(SocketForCash, static s => (s.Get(out int k), k), static s => (s.Get(out int v), v)) ||
            !ks.GetMap(SocketGroupIdForCash, static s => (s.Get(out int k), k), static s => (s.Get(out int v), v)) ||
            !ks.Get(out int pid))
            return false;
        OrderNo = ono; PackageProductNo = ppno; PackageProductId = ppid; ProductKind = pk; ProductId = pid;
        return true;
    }
}

public sealed class KEGS_CASH_PRODUCT_INFO_REQ : IKSerializable
{
    public int Page { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Page);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int page)) return false;
        Page = page;
        return true;
    }
}

public sealed class KEGS_CASH_PRODUCT_INFO_ACK : IKSerializable
{
    public bool Final { get; set; }
    public List<KNXBTProductInfo> ProductInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Final) && ks.PutList(ProductInfo);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool fin) || !ks.GetList(ProductInfo)) return false;
        Final = fin;
        return true;
    }
}

public sealed class KEGS_PRESENT_CASH_ITEM_REQ : IKSerializable
{
    public string Password { get; set; } = string.Empty;
    public string ReceiverNickName { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public List<KNXBTPurchaseReqInfo> PurchaseReqInfo { get; set; } = new();
    public bool IsWishList { get; set; }
    public int UseCashType { get; set; } // SERV_SUPPORT_SEVERAL_CASH_TYPES

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.PutWideString(Password) && ks.PutWideString(ReceiverNickName) && ks.PutWideString(Message) &&
        ks.PutList(PurchaseReqInfo) && ks.Put(IsWishList) && ks.Put(UseCashType);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out var pw) || !ks.GetWideString(out var nick) || !ks.GetWideString(out var msg) ||
            !ks.GetList(PurchaseReqInfo) || !ks.Get(out bool wish) || !ks.Get(out int uct))
            return false;
        Password = pw; ReceiverNickName = nick; Message = msg; IsWishList = wish; UseCashType = uct;
        return true;
    }
}

public sealed class KEGS_BUY_CASH_ITEM_AUTOMATIC_PAYMENT_REQ : IKSerializable
{
    public uint ProductNo { get; set; }
    public ushort OrderQuantity { get; set; } = 1;

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(ProductNo) && ks.Put(OrderQuantity);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint pno) || !ks.Get(out ushort qty)) return false;
        ProductNo = pno; OrderQuantity = qty;
        return true;
    }
}

public sealed class KEGS_BUY_CASH_ITEM_AUTOMATIC_PAYMENT_ACK : IKSerializable
{
    public int Ok { get; set; }
    public uint UserSn { get; set; }
    public byte DomainType { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public uint ProductNo { get; set; }
    public uint SalePrice { get; set; }
    public ushort OrderQuantity { get; set; } = 1;

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(UserSn) && ks.Put(DomainType) && ks.PutWideString(TransactionId) &&
        ks.Put(ProductNo) && ks.Put(SalePrice) && ks.Put(OrderQuantity);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out uint usn) || !ks.Get(out byte dom) ||
            !ks.GetWideString(out var tid) || !ks.Get(out uint pno) || !ks.Get(out uint price) ||
            !ks.Get(out ushort qty))
            return false;
        Ok = ok; UserSn = usn; DomainType = dom; TransactionId = tid;
        ProductNo = pno; SalePrice = price; OrderQuantity = qty;
        return true;
    }
}

public sealed class KEGS_AUTO_PAYMENT_RESURRECTION_STONE_NOT : IKSerializable
{
    public int NumAutoPaymentResStone { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(NumAutoPaymentResStone);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v)) return false;
        NumAutoPaymentResStone = v;
        return true;
    }
}

public sealed class KEGS_PARTY_FEVER_POINT_NOT : IKSerializable
{
    public int PartyFever { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(PartyFever);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v)) return false;
        PartyFever = v;
        return true;
    }
}

public sealed class KEGS_CHECK_PRESENT_CASH_INVENTORY_NOT : IKSerializable
{
    public uint OrderNo { get; set; }
    public uint ProductNo { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(OrderNo) && ks.Put(ProductNo);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint ono) || !ks.Get(out uint pno)) return false;
        OrderNo = ono; ProductNo = pno;
        return true;
    }
}

public sealed class KEGS_PRESENT_MESSAGE_TO_RECEIVER_NOT : IKSerializable
{
    public string SenderNickName { get; set; } = string.Empty;
    public string ReceiverUserId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public KBillProductInfo BillProductInfo { get; set; } = new(); // SERV_GLOBAL_BILLING

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.PutWideString(SenderNickName) && ks.PutWideString(ReceiverUserId) &&
        ks.PutWideString(Message) && ks.Put(BillProductInfo);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out var sender) || !ks.GetWideString(out var receiver) ||
            !ks.GetWideString(out var msg) || !ks.Get(BillProductInfo))
            return false;
        SenderNickName = sender; ReceiverUserId = receiver; Message = msg;
        return true;
    }
}

public sealed class KEGS_UPDATE_CASH_SKILL_POINT_NOT : IKSerializable
{
    public int Ok { get; set; }
    public long UnitUid { get; set; }
    public int CsPoint { get; set; }
    public string CsPointEndDate { get; set; } = string.Empty;
    public bool UpdateEndDateOnly { get; set; }
    public int SkillPointItemId { get; set; } // SERV_EVENT_CASH_SKILL_POINT_ITEM

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(UnitUid) && ks.Put(CsPoint) &&
        ks.PutWideString(CsPointEndDate) && ks.Put(UpdateEndDateOnly) && ks.Put(SkillPointItemId);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out long uid) || !ks.Get(out int csp) ||
            !ks.GetWideString(out var end) || !ks.Get(out bool only) || !ks.Get(out int itemId))
            return false;
        Ok = ok; UnitUid = uid; CsPoint = csp; CsPointEndDate = end;
        UpdateEndDateOnly = only; SkillPointItemId = itemId;
        return true;
    }
}

public sealed class KEGS_EXPIRE_CASH_SKILL_POINT_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public List<int> RetrievedSPoint { get; set; } = new(); // SERV_SKILL_PAGE_SYSTEM
    public List<Dictionary<int, int>> MapHaveSkill { get; set; } = new(); // SERV_UPGRADE_SKILL_SYSTEM_2013 + SERV_SKILL_PAGE_SYSTEM

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(UnitUid) &&
        ks.PutList(RetrievedSPoint, static (s, v) => s.Put(v)) &&
        ks.PutList(MapHaveSkill, static (s, m) => s.PutMap(m, static (s2, k) => s2.Put(k), static (s2, v) => s2.Put(v)));

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) ||
            !ks.GetList(RetrievedSPoint, static s => (s.Get(out int v), v)) ||
            !ks.GetList(MapHaveSkill, static s =>
            {
                var d = new Dictionary<int, int>();
                bool ok = s.GetMap(d, static s2 => (s2.Get(out int k), k), static s2 => (s2.Get(out int v), v));
                return (ok, d);
            }))
            return false;
        UnitUid = uid;
        return true;
    }
}

public sealed class KEGS_BUY_UNIT_CLASS_CHANGE_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public int NewUnitClass { get; set; }
    public List<short> SkillUnsealed { get; set; } = new();
    public Dictionary<int, int> ChangeMemo { get; set; } = new();
    public List<KInventoryItemInfo> ChangeItem { get; set; } = new();
    public List<KCompleteQuestInfo> ChangeCompleteQuest { get; set; } = new();
    public List<KQuestInstance> ChangeInProgressQuest { get; set; } = new();
    public KUnitInfo UnitInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(UnitUid) && ks.Put(NewUnitClass) &&
        ks.PutList(SkillUnsealed, static (s, v) => s.Put(v)) &&
        ks.PutMap(ChangeMemo, static (s, k) => s.Put(k), static (s, v) => s.Put(v)) &&
        ks.PutList(ChangeItem) && ks.PutList(ChangeCompleteQuest) &&
        ks.PutList(ChangeInProgressQuest) && ks.Put(UnitInfo);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.Get(out int cls) ||
            !ks.GetList(SkillUnsealed, static s => (s.Get(out short v), v)) ||
            !ks.GetMap(ChangeMemo, static s => (s.Get(out int k), k), static s => (s.Get(out int v), v)) ||
            !ks.GetList(ChangeItem) || !ks.GetList(ChangeCompleteQuest) ||
            !ks.GetList(ChangeInProgressQuest) || !ks.Get(UnitInfo))
            return false;
        UnitUid = uid; NewUnitClass = cls;
        return true;
    }
}

public sealed class KEGS_GET_WISH_LIST_ACK : IKSerializable
{
    public Dictionary<int, int> WishList { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.PutMap(WishList, static (s, k) => s.Put(k), static (s, v) => s.Put(v));

    public bool Get(KSerializer ks, RegionProfile region) =>
        ks.GetMap(WishList, static s => (s.Get(out int k), k), static s => (s.Get(out int v), v));
}

public sealed class KEGS_MODIFY_WISH_LIST_REQ : IKSerializable
{
    public List<int> WishListItemId { get; set; } = new();
    public bool IsDelete { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.PutList(WishListItemId, static (s, v) => s.Put(v)) && ks.Put(IsDelete);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetList(WishListItemId, static s => (s.Get(out int v), v)) || !ks.Get(out bool del))
            return false;
        IsDelete = del;
        return true;
    }
}

public sealed class KEGS_MODIFY_WISH_LIST_ACK : IKSerializable
{
    public int Ok { get; set; }
    public List<int> WishListItemId { get; set; } = new();
    public bool IsDelete { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.PutList(WishListItemId, static (s, v) => s.Put(v)) && ks.Put(IsDelete);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.GetList(WishListItemId, static s => (s.Get(out int v), v)) || !ks.Get(out bool del))
            return false;
        Ok = ok; IsDelete = del;
        return true;
    }
}

public sealed class KEGS_CASH_ITEM_REFUND_REQ : IKSerializable
{
    public uint OrderNo { get; set; }
    public uint ProductNo { get; set; }
    public int Quantity { get; set; } = 1;

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(OrderNo) && ks.Put(ProductNo) && ks.Put(Quantity);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint ono) || !ks.Get(out uint pno) || !ks.Get(out int qty))
            return false;
        OrderNo = ono; ProductNo = pno; Quantity = qty;
        return true;
    }
}

public sealed class KEGS_VISIT_CASH_SHOP_NOT : IKSerializable
{
    public bool EnterCashShop { get; set; }
    public long UnitUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(EnterCashShop) && ks.Put(UnitUid);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool enter) || !ks.Get(out long uid)) return false;
        EnterCashShop = enter; UnitUid = uid;
        return true;
    }
}

public sealed class KEGS_BINGO_EVENT_INFO_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int LotteryChance { get; set; }
    public int LotteryRecord { get; set; }
    public int MixChance { get; set; }
    public int ResetChance { get; set; }
    public List<byte> BingoBoard { get; set; } = new();
    public List<(int PresentId, bool Acquired)> PresentInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(LotteryChance) && ks.Put(LotteryRecord) &&
        ks.Put(MixChance) && ks.Put(ResetChance) &&
        ks.PutList(BingoBoard, static (s, v) => s.Put(v)) &&
        ks.PutList(PresentInfo, static (s, p) => s.Put(p.PresentId) && s.Put(p.Acquired));

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out int lc) || !ks.Get(out int lr) ||
            !ks.Get(out int mc) || !ks.Get(out int rc) ||
            !ks.GetList(BingoBoard, static s => (s.Get(out byte b), b)) ||
            !ks.GetList(PresentInfo, static s => { bool ok1 = s.Get(out int pid); bool ok2 = s.Get(out bool acq); return (ok1 && ok2, (pid, acq)); }))
            return false;
        Ok = ok; LotteryChance = lc; LotteryRecord = lr; MixChance = mc; ResetChance = rc;
        return true;
    }
}

public sealed class KEGS_BINGO_EVENT_LOTTERY_ACK : IKSerializable
{
    public int Ok { get; set; }
    public byte Num { get; set; }
    public List<byte> AcquiredPresent { get; set; } = new();
    public int LotteryChance { get; set; }
    public int LotteryRecord { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(Num) &&
        ks.PutList(AcquiredPresent, static (s, v) => s.Put(v)) &&
        ks.Put(LotteryChance) && ks.Put(LotteryRecord);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out byte num) ||
            !ks.GetList(AcquiredPresent, static s => (s.Get(out byte v), v)) ||
            !ks.Get(out int lc) || !ks.Get(out int lr))
            return false;
        Ok = ok; Num = num; LotteryChance = lc; LotteryRecord = lr;
        return true;
    }
}

public sealed class KEGS_BINGO_EVENT_MIX_ACK : IKSerializable
{
    public int Ok { get; set; }
    public List<byte> BingoBoard { get; set; } = new();
    public List<byte> AcquiredPresent { get; set; } = new();
    public int MixChance { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) &&
        ks.PutList(BingoBoard, static (s, v) => s.Put(v)) &&
        ks.PutList(AcquiredPresent, static (s, v) => s.Put(v)) &&
        ks.Put(MixChance);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) ||
            !ks.GetList(BingoBoard, static s => (s.Get(out byte v), v)) ||
            !ks.GetList(AcquiredPresent, static s => (s.Get(out byte v), v)) ||
            !ks.Get(out int mc))
            return false;
        Ok = ok; MixChance = mc;
        return true;
    }
}

public sealed class KEGS_BINGO_EVENT_RESET_ACK : IKSerializable
{
    public int Ok { get; set; }
    public List<byte> BingoBoard { get; set; } = new();
    public List<int> PresentInfo { get; set; } = new();
    public int ResetChance { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) &&
        ks.PutList(BingoBoard, static (s, v) => s.Put(v)) &&
        ks.PutList(PresentInfo, static (s, v) => s.Put(v)) &&
        ks.Put(ResetChance);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) ||
            !ks.GetList(BingoBoard, static s => (s.Get(out byte v), v)) ||
            !ks.GetList(PresentInfo, static s => (s.Get(out int v), v)) ||
            !ks.Get(out int rc))
            return false;
        Ok = ok; ResetChance = rc;
        return true;
    }
}

public sealed class KEGS_UPDATE_WARP_VIP_NOT : IKSerializable
{
    public int Ok { get; set; }
    public long WarpVipEndDate { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.Put(WarpVipEndDate);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out long dt)) return false;
        Ok = ok; WarpVipEndDate = dt;
        return true;
    }
}

public sealed class KEGS_COUPON_LIST_REQ : IKSerializable
{
    public bool Used { get; set; }
    public short CouponBoxType { get; set; }
    public ushort PageNum { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Used) && ks.Put(CouponBoxType) && ks.Put(PageNum);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool used) || !ks.Get(out short cbt) || !ks.Get(out ushort pn)) return false;
        Used = used; CouponBoxType = cbt; PageNum = pn;
        return true;
    }
}

public sealed class KEGS_COUPON_LIST_ACK : IKSerializable
{
    public int Ok { get; set; }
    public bool Used { get; set; }
    public short CouponBoxType { get; set; }
    public ushort PageNum { get; set; }
    public int CouponCount { get; set; }
    public List<KClientCouponBox> CouponList { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(Used) && ks.Put(CouponBoxType) && ks.Put(PageNum) &&
        ks.Put(CouponCount) && ks.PutList(CouponList);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out bool used) || !ks.Get(out short cbt) ||
            !ks.Get(out ushort pn) || !ks.Get(out int cnt) || !ks.GetList(CouponList))
            return false;
        Ok = ok; Used = used; CouponBoxType = cbt; PageNum = pn; CouponCount = cnt;
        return true;
    }
}

public sealed class KEGS_REGIST_COUPON_REQ : IKSerializable
{
    public string CouponPin { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region) => ks.PutNarrowString(CouponPin);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetNarrowString(out var pin)) return false;
        CouponPin = pin;
        return true;
    }
}

public sealed class KEGS_USE_ITEM_COUPON_REQ : IKSerializable
{
    public int CouponCardNo { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(CouponCardNo);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int no)) return false;
        CouponCardNo = no;
        return true;
    }
}

public sealed class KEGS_COUPON_TARGET_ITEM_REQ : IKSerializable
{
    public int CouponCardNo { get; set; }
    public ushort PageNum { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(CouponCardNo) && ks.Put(PageNum);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int no) || !ks.Get(out ushort pn)) return false;
        CouponCardNo = no; PageNum = pn;
        return true;
    }
}

public sealed class KEGS_COUPON_TARGET_ITEM_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int CouponCardNo { get; set; }
    public ushort PageNum { get; set; }
    public int TargetItemCount { get; set; }
    public List<KCouponBoxTargetItem> TargetItem { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(CouponCardNo) && ks.Put(PageNum) &&
        ks.Put(TargetItemCount) && ks.PutList(TargetItem);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out int no) || !ks.Get(out ushort pn) ||
            !ks.Get(out int cnt) || !ks.GetList(TargetItem))
            return false;
        Ok = ok; CouponCardNo = no; PageNum = pn; TargetItemCount = cnt;
        return true;
    }
}

public sealed class KEGS_DISCOUNT_COUPON_INQUIRY_ACK : IKSerializable
{
    public int Ok { get; set; }
    public List<KDiscountCoupon> DiscountCoupon { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.PutList(DiscountCoupon);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.GetList(DiscountCoupon)) return false;
        Ok = ok;
        return true;
    }
}

public sealed class KEGS_DISCOUNT_COUPON_LIST_INQUIRY_REQ : IKSerializable
{
    public List<KDiscountCouponInquriyInfo> DiscountCouponInquiryInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) => ks.PutList(DiscountCouponInquiryInfo);
    public bool Get(KSerializer ks, RegionProfile region) => ks.GetList(DiscountCouponInquiryInfo);
}

public sealed class KEGS_GET_NAVER_ACCESS_TOKEN_ACK : IKSerializable
{
    public int Ok { get; set; }
    public string AccessToken { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.PutNarrowString(AccessToken);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.GetNarrowString(out var tok)) return false;
        Ok = ok; AccessToken = tok;
        return true;
    }
}

public sealed class KEGS_BUY_CASH_ITEM_COUPON_ACK : IKSerializable
{
    public int Ok { get; set; }
    public List<KNXBTPurchaseCouponAckInfo> PurchaseCouponAckInfo { get; set; } = new();
    public bool IsWishList { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.PutList(PurchaseCouponAckInfo) && ks.Put(IsWishList);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.GetList(PurchaseCouponAckInfo) || !ks.Get(out bool wish)) return false;
        Ok = ok; IsWishList = wish;
        return true;
    }
}

public sealed class KEGS_BILL_PRODUCT_INFO_REQ : IKSerializable
{
    public int Page { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Page);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int page)) return false;
        Page = page;
        return true;
    }
}

public sealed class KEGS_BILL_PRODUCT_INFO_ACK : IKSerializable
{
    public bool Final { get; set; }
    public List<KBillProductInfo> ProductInfo { get; set; } = new();
    // SERV_KEEP_ITEM_SHOW_CASHSHOP = True in TW; SERV_WISH_LIST_NO_ITEM = False in TW
    public Dictionary<int, List<int>> KeepShowItem { get; set; } = new();
    public Dictionary<int, kDisCountItemInfo> DisCountInfoMap { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Final) && ks.PutList(ProductInfo) &&
        ks.PutMap(KeepShowItem, static (s, k) => s.Put(k), static (s, v) => s.PutList(v, static (s2, x) => s2.Put(x))) &&
        ks.PutMap(DisCountInfoMap, static (s, k) => s.Put(k), static (s, v) => s.Put(v));

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool fin) || !ks.GetList(ProductInfo) ||
            !ks.GetMap(KeepShowItem, static s => (s.Get(out int k), k), static s =>
            {
                var lst = new List<int>();
                bool ok = s.GetList(lst, static s2 => (s2.Get(out int x), x));
                return (ok, lst);
            }) ||
            !ks.GetMap(DisCountInfoMap, static s => (s.Get(out int k), k), static s =>
            {
                var d = new kDisCountItemInfo();
                bool ok = s.Get(d);
                return (ok, d);
            }))
            return false;
        Final = fin;
        return true;
    }
}

public sealed class KEGS_BILL_INVENTORY_INQUIRY_REQ : IKSerializable
{
    public int ItemPerPage { get; set; } = 10;
    public int CurrentPage { get; set; } = 1;

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(ItemPerPage) && ks.Put(CurrentPage);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ipp) || !ks.Get(out int cp)) return false;
        ItemPerPage = ipp; CurrentPage = cp;
        return true;
    }
}

public sealed class KEGS_BILL_INVENTORY_INQUIRY_ACK : IKSerializable
{
    public int Ok { get; set; }
    public List<KBillOrderInfo> OrderInfo { get; set; } = new();
    // SERV_GLOBAL_CASH_PACKAGE = True in TW
    public List<KBillPackageInfo> PackageInfo { get; set; } = new();
    public List<int> SubPackageTrans { get; set; } = new();
    public List<int> SubPackageInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.PutList(OrderInfo) && ks.PutList(PackageInfo) &&
        ks.PutList(SubPackageTrans, static (s, v) => s.Put(v)) &&
        ks.PutList(SubPackageInfo, static (s, v) => s.Put(v));

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.GetList(OrderInfo) || !ks.GetList(PackageInfo) ||
            !ks.GetList(SubPackageTrans, static s => (s.Get(out int v), v)) ||
            !ks.GetList(SubPackageInfo, static s => (s.Get(out int v), v)))
            return false;
        Ok = ok;
        return true;
    }
}

public sealed class KEGS_BILL_GET_PURCHASED_CASH_ITEM_REQ : IKSerializable
{
    public int TransNo { get; set; }
    public int ProductNo { get; set; }
    public Dictionary<int, int> SocketForCash { get; set; } = new();
    public Dictionary<int, int> SocketGroupIdForCash { get; set; } = new(); // SERV_GLOBAL_CASH_PACKAGE + SERV_CASH_ITEM_SOCKET_OPTION
    public byte ProductKind { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(TransNo) && ks.Put(ProductNo) &&
        ks.PutMap(SocketForCash, static (s, k) => s.Put(k), static (s, v) => s.Put(v)) &&
        ks.PutMap(SocketGroupIdForCash, static (s, k) => s.Put(k), static (s, v) => s.Put(v)) &&
        ks.Put(ProductKind);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int tno) || !ks.Get(out int pno) ||
            !ks.GetMap(SocketForCash, static s => (s.Get(out int k), k), static s => (s.Get(out int v), v)) ||
            !ks.GetMap(SocketGroupIdForCash, static s => (s.Get(out int k), k), static s => (s.Get(out int v), v)) ||
            !ks.Get(out byte pk))
            return false;
        TransNo = tno; ProductNo = pno; ProductKind = pk;
        return true;
    }
}

public sealed class KEGS_EXCHANGE_CASH_REQ : IKSerializable
{
    public uint ExchangeValue { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(ExchangeValue);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint val)) return false;
        ExchangeValue = val;
        return true;
    }
}

public sealed class KEGS_EXCHANGE_CASH_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int Point { get; set; }
    public KGlobalCashInfo AddBonusCashInfo { get; set; } = new(); // SERV_SUPPORT_SEVERAL_CASH_TYPES

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(Point) && ks.Put(AddBonusCashInfo);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out int pt) || !ks.Get(AddBonusCashInfo)) return false;
        Ok = ok; Point = pt;
        return true;
    }
}

public sealed class KEGS_EXCHANGE_CASH_CLIENT_NOT : IKSerializable
{
    public int Ok { get; set; }
    public int Point { get; set; }
    public KGlobalCashInfo AddBonusCashInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(Point) && ks.Put(AddBonusCashInfo);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out int pt) || !ks.Get(AddBonusCashInfo)) return false;
        Ok = ok; Point = pt;
        return true;
    }
}

public sealed class KEGS_BONUS_CASH_EVENT_NOT : IKSerializable
{
    public bool EventStart { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(EventStart);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool ev)) return false;
        EventStart = ev;
        return true;
    }
}

public sealed class KEGS_CASH_DIRECT_CHARGE_CN_REQ : IKSerializable
{
    public long UserUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(UserUid);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid)) return false;
        UserUid = uid;
        return true;
    }
}

public sealed class KEGS_CASH_DIRECT_CHARGE_CN_ACK : IKSerializable
{
    public int Ok { get; set; }
    public string Token { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.PutWideString(Token);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.GetWideString(out var tok)) return false;
        Ok = ok; Token = tok;
        return true;
    }
}

// =========================================================================
// AdminCheat Packets (50 handlers - TW ServerDefine Parity)
// =========================================================================

public sealed class KEGS_NOTIFY_MSG_REQ : IKSerializable
{
    public const sbyte NtGmNotifyAllServers = 0;
    public const sbyte NtGmNotifyServerGroup = 1;
    public const sbyte NtRandomGetItem = 2;
    public const sbyte NtDropGetItem = 3;
    public const sbyte NtMaxLevelUp = 4;
    public const sbyte NtEnchantSuccess = 5;
    public const sbyte NtHenirClear = 6;
    public const sbyte NtGuildCreation = 7;
    public const sbyte NtGuildLevelUp = 8;
    public const sbyte NtRandomGetItemAllNotify = 9;
    public const sbyte NtShutDownAllNotify = 10;
    public const sbyte NtGuildNameChanged = 11;
    public const sbyte NtTheGateOfDarkness = 12;
    public const sbyte NtWeddingSuccess = 13;

    public sbyte NotifyType { get; set; } = NtGmNotifyAllServers;
    public sbyte Count { get; set; } = 1;
    public string Msg { get; set; } = string.Empty;
    public int RandomItem { get; set; } // SERV_GET_ITEM_NOTIFY

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(NotifyType) && ks.Put(Count) && ks.PutWideString(Msg) && ks.Put(RandomItem);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte nt) || !ks.Get(out sbyte cnt) || !ks.GetWideString(out var msg) || !ks.Get(out int ri))
            return false;
        NotifyType = nt; Count = cnt; Msg = msg; RandomItem = ri;
        return true;
    }
}

public sealed class KEGS_ADMIN_SERVER_STATUS_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int Ccu { get; set; }
    public Dictionary<int, int> NumPvpRoom { get; set; } = new();
    public int CurPartyCount { get; set; }
    public int PlayGamePartyCount { get; set; }
    // SERV_CHANNELING_USER_MANAGER = False in TW

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(Ccu) &&
        ks.PutMap(NumPvpRoom, static (s, k) => s.Put(k), static (s, v) => s.Put(v)) &&
        ks.Put(CurPartyCount) && ks.Put(PlayGamePartyCount);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out int ccu) ||
            !ks.GetMap(NumPvpRoom, static s => (s.Get(out int k), k), static s => (s.Get(out int v), v)) ||
            !ks.Get(out int cpc) || !ks.Get(out int pgc))
            return false;
        Ok = ok; Ccu = ccu; CurPartyCount = cpc; PlayGamePartyCount = pgc;
        return true;
    }
}

public sealed class KEGS_ADMIN_CHANGE_AUTH_LEVEL_REQ : IKSerializable
{
    public string UserId { get; set; } = string.Empty;
    public sbyte AuthLevel { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.PutWideString(UserId) && ks.Put(AuthLevel);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out var uid) || !ks.Get(out sbyte lv)) return false;
        UserId = uid; AuthLevel = lv;
        return true;
    }
}

public sealed class KEGS_ADMIN_GET_AUTH_LEVEL_LIST_REQ : IKSerializable
{
    public sbyte AuthLevel { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(AuthLevel);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte lv)) return false;
        AuthLevel = lv;
        return true;
    }
}

public sealed class KEGS_ADMIN_GET_AUTH_LEVEL_LIST_ACK : IKSerializable
{
    public int Ok { get; set; }
    public List<string> UserIdList { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.PutList(UserIdList, static (s, v) => s.PutWideString(v));

    public bool Get(KSerializer ks, RegionProfile region) =>
        ks.Get(out int ok) && ks.GetList(UserIdList, static s => (s.GetWideString(out var v), v)) && (Ok = ok) == ok;
}

public sealed class KEGS_ADMIN_CHECK_NICKNAME_REQ : IKSerializable
{
    public string UnitNickName { get; set; } = string.Empty;
    public bool IsUserId { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.PutWideString(UnitNickName) && ks.Put(IsUserId);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out var nick) || !ks.Get(out bool isUid)) return false;
        UnitNickName = nick; IsUserId = isUid;
        return true;
    }
}

public sealed class KEGS_ADMIN_KICK_USER_REQ : IKSerializable
{
    public string UnitNickName { get; set; } = string.Empty;
    public bool IsUserId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.PutWideString(UnitNickName) && ks.Put(IsUserId);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out var nick) || !ks.Get(out bool isUid)) return false;
        UnitNickName = nick; IsUserId = isUid;
        return true;
    }
}

public sealed class KEGS_ADMIN_CHANGE_UNIT_CLASS_REQ : IKSerializable
{
    public sbyte UnitClass { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(UnitClass);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte uc)) return false;
        UnitClass = uc;
        return true;
    }
}

public sealed class KEGS_ADMIN_CHANGE_UNIT_CLASS_ACK : IKSerializable
{
    public int Ok { get; set; }
    public sbyte UnitClass { get; set; }
    // SERV_UPGRADE_SKILL_SYSTEM_2013 = True in TW
    public int SPoint { get; set; }
    public int CsPoint { get; set; }
    public int DefaultSkill1 { get; set; }
    public int DefaultSkill2 { get; set; }
    public int DefaultSkill3 { get; set; }
    public int DefaultSkill4 { get; set; }
    public int DefaultSkill5 { get; set; }
    public int DefaultSkill6 { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(UnitClass) && ks.Put(SPoint) && ks.Put(CsPoint) &&
        ks.Put(DefaultSkill1) && ks.Put(DefaultSkill2) && ks.Put(DefaultSkill3) &&
        ks.Put(DefaultSkill4) && ks.Put(DefaultSkill5) && ks.Put(DefaultSkill6);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out sbyte uc) || !ks.Get(out int sp) || !ks.Get(out int csp) ||
            !ks.Get(out int d1) || !ks.Get(out int d2) || !ks.Get(out int d3) ||
            !ks.Get(out int d4) || !ks.Get(out int d5) || !ks.Get(out int d6))
            return false;
        Ok = ok; UnitClass = uc; SPoint = sp; CsPoint = csp;
        DefaultSkill1 = d1; DefaultSkill2 = d2; DefaultSkill3 = d3;
        DefaultSkill4 = d4; DefaultSkill5 = d5; DefaultSkill6 = d6;
        return true;
    }
}

public sealed class KEGS_ADMIN_GET_SKILL_REQ : IKSerializable
{
    public int SkillId { get; set; }
    public int SkillLevel { get; set; }
    public int ActiveSkillPageNumber { get; set; } // SERV_SKILL_PAGE_SYSTEM

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(SkillId) && ks.Put(SkillLevel) && ks.Put(ActiveSkillPageNumber);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int sid) || !ks.Get(out int lv) || !ks.Get(out int pg)) return false;
        SkillId = sid; SkillLevel = lv; ActiveSkillPageNumber = pg;
        return true;
    }
}

public sealed class KEGS_ADMIN_GET_SKILL_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int SkillId { get; set; }
    public int SkillLevel { get; set; }
    public int ActiveSkillPageNumber { get; set; } // SERV_SKILL_PAGE_SYSTEM
    public int CsPoint { get; set; }
    public bool Unsealed { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(SkillId) && ks.Put(SkillLevel) &&
        ks.Put(ActiveSkillPageNumber) && ks.Put(CsPoint) && ks.Put(Unsealed);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out int sid) || !ks.Get(out int lv) ||
            !ks.Get(out int pg) || !ks.Get(out int csp) || !ks.Get(out bool uns))
            return false;
        Ok = ok; SkillId = sid; SkillLevel = lv; ActiveSkillPageNumber = pg; CsPoint = csp; Unsealed = uns;
        return true;
    }
}

public sealed class KEGS_ADMIN_AUTO_GET_ALL_SKILL_REQ : IKSerializable
{
    public int ActiveSkillPageNumber { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(ActiveSkillPageNumber);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int pg)) return false;
        ActiveSkillPageNumber = pg;
        return true;
    }
}

public sealed class KEGS_ADMIN_AUTO_GET_ALL_SKILL_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int ActiveSkillPageNumber { get; set; } // SERV_SKILL_PAGE_SYSTEM
    public int SPoint { get; set; }
    public int CsPoint { get; set; }
    public Dictionary<int, int> SkillList { get; set; } = new();
    public List<short> UnsealedSkillId { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(ActiveSkillPageNumber) && ks.Put(SPoint) && ks.Put(CsPoint) &&
        ks.PutMap(SkillList, static (s, k) => s.Put(k), static (s, v) => s.Put(v)) &&
        ks.PutList(UnsealedSkillId, static (s, v) => s.Put(v));

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out int pg) || !ks.Get(out int sp) || !ks.Get(out int csp) ||
            !ks.GetMap(SkillList, static s => (s.Get(out int k), k), static s => (s.Get(out int v), v)) ||
            !ks.GetList(UnsealedSkillId, static s => (s.Get(out short v), v)))
            return false;
        Ok = ok; ActiveSkillPageNumber = pg; SPoint = sp; CsPoint = csp;
        return true;
    }
}

public sealed class KEGS_ADMIN_CHEAT_GET_ALL_SKILL_REQ : IKSerializable
{
    public int ActiveSkillPageNumber { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(ActiveSkillPageNumber);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int pg)) return false;
        ActiveSkillPageNumber = pg;
        return true;
    }
}

public sealed class KEGS_ADMIN_CHEAT_GET_ALL_SKILL_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int ActiveSkillPageNumber { get; set; } // SERV_SKILL_PAGE_SYSTEM
    public List<short> UnSealedSkill { get; set; } = new();
    public Dictionary<int, KAdminCheatSkill> SkillInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(ActiveSkillPageNumber) &&
        ks.PutList(UnSealedSkill, static (s, v) => s.Put(v)) &&
        ks.PutMap(SkillInfo, static (s, k) => s.Put(k), static (s, v) => s.Put(v));

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out int pg) ||
            !ks.GetList(UnSealedSkill, static s => (s.Get(out short v), v)) ||
            !ks.GetMap(SkillInfo, static s => (s.Get(out int k), k), static s =>
            {
                var item = new KAdminCheatSkill();
                bool got = s.Get(item);
                return (got, item);
            }))
            return false;
        Ok = ok; ActiveSkillPageNumber = pg;
        return true;
    }
}

public sealed class KEGS_ADMIN_CHANGE_SKILL_POINT_REQ : IKSerializable
{
    public int SPoint { get; set; }
    public int ActiveSkillPageNumber { get; set; } // SERV_SKILL_PAGE_SYSTEM

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(SPoint) && ks.Put(ActiveSkillPageNumber);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int sp) || !ks.Get(out int pg)) return false;
        SPoint = sp; ActiveSkillPageNumber = pg;
        return true;
    }
}

public sealed class KEGS_ADMIN_CHANGE_SKILL_POINT_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int SPoint { get; set; }
    public int ActiveSkillPageNumber { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(SPoint) && ks.Put(ActiveSkillPageNumber);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out int sp) || !ks.Get(out int pg)) return false;
        Ok = ok; SPoint = sp; ActiveSkillPageNumber = pg;
        return true;
    }
}

public sealed class KEGS_ADMIN_INIT_SKILL_TREE_REQ : IKSerializable
{
    public int ActiveSkillPageNumber { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(ActiveSkillPageNumber);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int pg)) return false;
        ActiveSkillPageNumber = pg;
        return true;
    }
}

public sealed class KEGS_ADMIN_INIT_SKILL_TREE_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int ActiveSkillPageNumber { get; set; } // SERV_SKILL_PAGE_SYSTEM
    public int SPoint { get; set; }
    public int CsPoint { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(ActiveSkillPageNumber) && ks.Put(SPoint) && ks.Put(CsPoint);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out int pg) || !ks.Get(out int sp) || !ks.Get(out int csp))
            return false;
        Ok = ok; ActiveSkillPageNumber = pg; SPoint = sp; CsPoint = csp;
        return true;
    }
}

public sealed class KEGS_ADMIN_CASH_SKILL_POINT_DATE_CHANGE_REQ : IKSerializable
{
    public int Year { get; set; }
    public int Month { get; set; }
    public int Day { get; set; }
    public int Hour { get; set; }
    public int Minute { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Year) && ks.Put(Month) && ks.Put(Day) && ks.Put(Hour) && ks.Put(Minute);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int y) || !ks.Get(out int m) || !ks.Get(out int d) ||
            !ks.Get(out int h) || !ks.Get(out int min))
            return false;
        Year = y; Month = m; Day = d; Hour = h; Minute = min;
        return true;
    }
}

public sealed class KEGS_ADMIN_CASH_SKILL_POINT_DATE_CHANGE_ACK : IKSerializable
{
    public int Ok { get; set; }
    public string CspEndDate { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.PutWideString(CspEndDate);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.GetWideString(out var dt)) return false;
        Ok = ok; CspEndDate = dt;
        return true;
    }
}

public sealed class KEGS_ADMIN_GET_TITLE_CHEAT_REQ : IKSerializable
{
    public int TitleId { get; set; }
    public int Period { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(TitleId) && ks.Put(Period);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int tid) || !ks.Get(out int per)) return false;
        TitleId = tid; Period = per;
        return true;
    }
}

public sealed class KEGS_ADMIN_CHANGE_VSPOINT_REQ : IKSerializable
{
    // SERV_PVP_NEW_SYSTEM = True in TW
    public int Rating { get; set; }
    public int RPoint { get; set; }
    public int APoint { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Rating) && ks.Put(RPoint) && ks.Put(APoint);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int r) || !ks.Get(out int rp) || !ks.Get(out int ap)) return false;
        Rating = r; RPoint = rp; APoint = ap;
        return true;
    }
}

public sealed class KEGS_ADMIN_CHANGE_VSPOINT_ACK : IKSerializable
{
    public int Ok { get; set; }
    public sbyte Rank { get; set; } // SERV_PVP_NEW_SYSTEM + SERV_2012_PVP_SEASON2
    public int ChangedRPoint { get; set; }
    public int ChangedAPoint { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(Rank) && ks.Put(ChangedRPoint) && ks.Put(ChangedAPoint);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out sbyte rk) || !ks.Get(out int rp) || !ks.Get(out int ap))
            return false;
        Ok = ok; Rank = rk; ChangedRPoint = rp; ChangedAPoint = ap;
        return true;
    }
}

public sealed class KEGS_ADMIN_OBSERVER_MODE_REQ : IKSerializable
{
    public bool ObserverMode { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(ObserverMode);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool om)) return false;
        ObserverMode = om;
        return true;
    }
}

public sealed class KEGS_ADMIN_SET_PC_BANG_REQ : IKSerializable
{
    public bool On { get; set; }
    public int PcBangType { get; set; } // SERV_PC_BANG_TYPE

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(On) && ks.Put(PcBangType);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool on) || !ks.Get(out int pt)) return false;
        On = on; PcBangType = pt;
        return true;
    }
}

public sealed class KEGS_ADMIN_SET_PC_BANG_ACK : IKSerializable
{
    public int Ok { get; set; }
    public bool On { get; set; }
    public int PcBangType { get; set; } // SERV_PC_BANG_TYPE
    public KPcBangReward PcBangReward { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(On) && ks.Put(PcBangType) && ks.Put(PcBangReward);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out bool on) || !ks.Get(out int pt) || !ks.Get(PcBangReward))
            return false;
        Ok = ok; On = on; PcBangType = pt;
        return true;
    }
}

public sealed class KEGS_ADMIN_GET_GUILD_SKILL_POINT_REQ : IKSerializable
{
    public int GuildSPoint { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(GuildSPoint);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int sp)) return false;
        GuildSPoint = sp;
        return true;
    }
}

public sealed class KEGS_ADMIN_GET_GUILD_SKILL_POINT_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int CurGuildSPoint { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.Put(CurGuildSPoint);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out int cur)) return false;
        Ok = ok; CurGuildSPoint = cur;
        return true;
    }
}

public sealed class KEGS_ADMIN_CHANGE_PET_INFO_NOT : IKSerializable
{
    public const int PctEvolutionStep = 0;
    public const int PctSatiety = 1;
    public const int PctIntimacy = 2;
    public const int PctExtroversion = 3;
    public const int PctEmotion = 4;

    public int CheatType { get; set; }
    public int Value { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(CheatType) && ks.Put(Value);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ct) || !ks.Get(out int v)) return false;
        CheatType = ct; Value = v;
        return true;
    }
}

public sealed class KEGS_ADMIN_NPC_UNIT_CREATE_REQ : IKSerializable
{
    public KNPCUnitReq NpcUnitReq { get; set; } = new();
    public sbyte Attrib1 { get; set; }
    public sbyte Attrib2 { get; set; }
    public sbyte Sa1 { get; set; }
    public sbyte Sa2 { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(NpcUnitReq) && ks.Put(Attrib1) && ks.Put(Attrib2) && ks.Put(Sa1) && ks.Put(Sa2);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(NpcUnitReq) || !ks.Get(out sbyte a1) || !ks.Get(out sbyte a2) ||
            !ks.Get(out sbyte s1) || !ks.Get(out sbyte s2))
            return false;
        Attrib1 = a1; Attrib2 = a2; Sa1 = s1; Sa2 = s2;
        return true;
    }
}

public sealed class KEGS_ADMIN_NPC_UNIT_CREATE_NOT : IKSerializable
{
    public KNPCUnitReq NpcUnitNot { get; set; } = new();
    public sbyte Attrib1 { get; set; }
    public sbyte Attrib2 { get; set; }
    public sbyte Sa1 { get; set; }
    public sbyte Sa2 { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(NpcUnitNot) && ks.Put(Attrib1) && ks.Put(Attrib2) && ks.Put(Sa1) && ks.Put(Sa2);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(NpcUnitNot) || !ks.Get(out sbyte a1) || !ks.Get(out sbyte a2) ||
            !ks.Get(out sbyte s1) || !ks.Get(out sbyte s2))
            return false;
        Attrib1 = a1; Attrib2 = a2; Sa1 = s1; Sa2 = s2;
        return true;
    }
}

public sealed class KEGS_ADMIN_DELETE_GUILD_ADD_NOT : IKSerializable
{
    public int GuildUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(GuildUid);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int gid)) return false;
        GuildUid = gid;
        return true;
    }
}

public sealed class KEGS_ADMIN_DELETE_HENIR_RANKING_RANK_NOT : IKSerializable
{
    public int RankingType { get; set; }
    public int Rank { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(RankingType) && ks.Put(Rank);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int rt) || !ks.Get(out int rk)) return false;
        RankingType = rt; Rank = rk;
        return true;
    }
}

public sealed class KEGS_ADMIN_DELETE_HENIR_RANKING_NICKNAME_NOT : IKSerializable
{
    public int RankingType { get; set; }
    public string NickName { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(RankingType) && ks.PutWideString(NickName);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int rt) || !ks.GetWideString(out var nick)) return false;
        RankingType = rt; NickName = nick;
        return true;
    }
}

public sealed class KEGS_ADMIN_DELETE_HENIR_RANKING_UNITUID_NOT : IKSerializable
{
    public int RankingType { get; set; }
    public long UnitUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(RankingType) && ks.Put(UnitUid);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int rt) || !ks.Get(out long uid)) return false;
        RankingType = rt; UnitUid = uid;
        return true;
    }
}

public sealed class KEGS_ADMIN_HENIR_REWARD_CHEAT_NOT : IKSerializable
{
    public int HenirRewardNormalCnt { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(HenirRewardNormalCnt);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int c)) return false;
        HenirRewardNormalCnt = c;
        return true;
    }
}

public sealed class KEGS_ADMIN_PSHOP_AGENCY_CHEAT_REQ : IKSerializable
{
    public short Period { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Period);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out short p)) return false;
        Period = p;
        return true;
    }
}

public sealed class KEGS_ADMIN_HERO_PVP_USER_LIST_ACK : IKSerializable
{
    public List<KHeroPvpUserInfo> UserList { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region) => ks.PutList(UserList);
    public bool Get(KSerializer ks, RegionProfile region) => ks.GetList(UserList);
}

public sealed class KEGS_ADMIN_HERO_PVP_USER_ADD_REQ : IKSerializable
{
    public List<(long ServerGroupId, string NickName)> UserList { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.PutList(UserList, static (s, item) => s.Put(item.ServerGroupId) && s.PutWideString(item.NickName));

    public bool Get(KSerializer ks, RegionProfile region) =>
        ks.GetList(UserList, static s => { bool ok1 = s.Get(out long sg); bool ok2 = s.GetWideString(out var nick); return (ok1 && ok2, (sg, nick)); });
}

public sealed class KEGS_ADMIN_HERO_PVP_USER_CONNECTION_NOT : IKSerializable
{
    public List<(bool Connected, KHeroPvpUserInfo Info)> UserList { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.PutList(UserList, static (s, item) => s.Put(item.Connected) && s.Put(item.Info));

    public bool Get(KSerializer ks, RegionProfile region) =>
        ks.GetList(UserList, static s =>
        {
            var info = new KHeroPvpUserInfo();
            bool ok = s.Get(out bool c) && s.Get(info);
            return (ok, (c, info));
        });
}

public sealed class KEGS_ADMIN_NOTIFY_HERO_PVP_USER_REQ : IKSerializable
{
    public long ServerGroupId { get; set; } = -1;
    public string NickName { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(ServerGroupId) && ks.PutWideString(NickName) && ks.PutWideString(Message);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long sg) || !ks.GetWideString(out var nick) || !ks.GetWideString(out var msg))
            return false;
        ServerGroupId = sg; NickName = nick; Message = msg;
        return true;
    }
}

public sealed class KEGS_ADMIN_CHANGE_COUPLE_DATE_REQ : IKSerializable
{
    public int AddHour { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(AddHour);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int h)) return false;
        AddHour = h;
        return true;
    }
}

public sealed class KEGS_ADMIN_SHOW_ED_CHEAT_REQ : IKSerializable
{
    public string NickName { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region) => ks.PutWideString(NickName);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out var nick)) return false;
        NickName = nick;
        return true;
    }
}

public sealed class KEGS_ADMIN_SHOW_ED_CHEAT_ACK : IKSerializable
{
    public int Ok { get; set; }
    public string NickName { get; set; } = string.Empty;
    public int Ed { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.PutWideString(NickName) && ks.Put(Ed);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.GetWideString(out var nick) || !ks.Get(out int ed)) return false;
        Ok = ok; NickName = nick; Ed = ed;
        return true;
    }
}

public sealed class KEGS_ADMIN_ED_MONITORING_CHEAT_NOT : IKSerializable
{
    public bool EdMonitoringMode { get; set; }
    public int MinEd { get; set; } = 100_000_000;

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(EdMonitoringMode) && ks.Put(MinEd);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool mode) || !ks.Get(out int minEd)) return false;
        EdMonitoringMode = mode; MinEd = minEd;
        return true;
    }
}

public sealed class KEGS_ADMIN_CHANNELING_CODE_CHEAT_NOT : IKSerializable
{
    public int ChannelingCode { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(ChannelingCode);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int cc)) return false;
        ChannelingCode = cc;
        return true;
    }
}

public sealed class KEGS_ADMIN_BOSS_FIELD_GATE_OPEN_REQ : IKSerializable
{
    public int BattleFieldId { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(BattleFieldId);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int bf)) return false;
        BattleFieldId = bf;
        return true;
    }
}

public sealed class KEGS_ADMIN_GET_TOTAL_DANGEROUS_VALUE_REQ : IKSerializable
{
    public int BattleFieldId { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(BattleFieldId);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int bf)) return false;
        BattleFieldId = bf;
        return true;
    }
}

public sealed class KEGS_ADMIN_GET_TOTAL_DANGEROUS_VALUE_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int BattleFieldId { get; set; }
    public byte TotalDangerousValue { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(BattleFieldId) && ks.Put(TotalDangerousValue);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out int bf) || !ks.Get(out byte val)) return false;
        Ok = ok; BattleFieldId = bf; TotalDangerousValue = val;
        return true;
    }
}

public sealed class KEGS_ADMIN_SET_TOTAL_DANGEROUS_VALUE_REQ : IKSerializable
{
    public int BattleFieldId { get; set; }
    public int TotalDangerousValue { get; set; }
    public bool Set { get; set; } = true;

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(BattleFieldId) && ks.Put(TotalDangerousValue) && ks.Put(Set);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int bf) || !ks.Get(out int val) || !ks.Get(out bool set)) return false;
        BattleFieldId = bf; TotalDangerousValue = val; Set = set;
        return true;
    }
}

public sealed class KEGS_ADMIN_BATTLE_FIELD_INCREASE_DANGER_REQ : IKSerializable
{
    public int Danger { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Danger);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int d)) return false;
        Danger = d;
        return true;
    }
}

public sealed class KEGS_ADMIN_BATTLE_FIELD_INCREASE_DANGER_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int Danger { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.Put(Danger);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out int d)) return false;
        Ok = ok; Danger = d;
        return true;
    }
}

public sealed class KEGS_BATTLE_FIELD_DANGEROUS_VALUE_NOT : IKSerializable
{
    public int DangerousValue { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(DangerousValue);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int dv)) return false;
        DangerousValue = dv;
        return true;
    }
}

public sealed class KEGS_SET_MULTIPLYER : IKSerializable
{
    public float M { get; set; } = 1.0f;
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(M);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out float m)) return false;
        M = m;
        return true;
    }
}

public sealed class KEGS_UDP_KICK_GAMEEDIT_NOT : IKSerializable
{
    public bool UdpSetKick { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(UdpSetKick);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool kick)) return false;
        UdpSetKick = kick;
        return true;
    }
}

public sealed class KEGS_QUEST_POINT_CHEAT_NOT : IKSerializable
{
    public int QuestPointCheat { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(QuestPointCheat);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int qp)) return false;
        QuestPointCheat = qp;
        return true;
    }
}

public sealed class KEGS_EVENT_COBO_ITEM_GIVE_CHEAT_NOT : IKSerializable
{
    public bool CoboEventItemGet { get; set; }
    public bool NextDay { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(CoboEventItemGet) && ks.Put(NextDay);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool itemGet) || !ks.Get(out bool nextDay)) return false;
        CoboEventItemGet = itemGet; NextDay = nextDay;
        return true;
    }
}

public sealed class KEGS_EVENT_VALENTINE_DUNGEON_GIVE_ITEM_CHEAT_NOT : IKSerializable
{
    public int ValentineItemCount { get; set; } = -1;
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(ValentineItemCount);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int cnt)) return false;
        ValentineItemCount = cnt;
        return true;
    }
}

// ---------------- Batch 2: GameCommon / Party / Pet / Inventory / BattleField Packets ----------------

public sealed class KEGS_2013_EVENT_MISSION_COMPLETE_REQ : IKSerializable
{
    public bool LevelUpEvent { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(LevelUpEvent);
    public bool Get(KSerializer ks, RegionProfile region) => ks.Get(out bool v) && ((LevelUpEvent = v) == v);
}

public sealed class KEGS_2013_EVENT_MISSION_COMPLETE_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int RewardedLevel { get; set; }
    public List<KInventoryItemInfo> InventorySlotInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(RewardedLevel) && ks.PutList(InventorySlotInfo);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out int lvl) || !ks.GetList(InventorySlotInfo)) return false;
        Ok = ok; RewardedLevel = lvl;
        return true;
    }
}

public sealed class KEGS_2014_EVENT_MISSION_COMPLETE_ACK : IKSerializable
{
    public int Ok { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok);
    public bool Get(KSerializer ks, RegionProfile region) => ks.Get(out int ok) && ((Ok = ok) == ok);
}

public sealed class KEGS_4TH_ANNIV_EVENT_REWARD_REQ : IKSerializable
{
    public int SelectedIndex { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(SelectedIndex);
    public bool Get(KSerializer ks, RegionProfile region) => ks.Get(out int idx) && ((SelectedIndex = idx) == idx);
}

public sealed class KEGS_4TH_ANNIV_EVENT_REWARD_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int SelectedIndex { get; set; } = -1;
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.Put(SelectedIndex);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out int idx)) return false;
        Ok = ok; SelectedIndex = idx;
        return true;
    }
}

public sealed class KEGS_ALL_COMPLETED_QUEST_COMPLETE_REQ : IKSerializable
{
    public List<KEGS_QUEST_COMPLETE_REQ> QuestCompleteReq { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region) => ks.PutList(QuestCompleteReq);
    public bool Get(KSerializer ks, RegionProfile region) => ks.GetList(QuestCompleteReq);
}

public sealed class KEGS_ALL_COMPLETED_QUEST_COMPLETE_ACK : IKSerializable
{
    public int Ok { get; set; }
    public List<KEGS_QUEST_COMPLETE_ACK> QuestCompleteAck { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.PutList(QuestCompleteAck);
    public bool Get(KSerializer ks, RegionProfile region) =>
        ks.Get(out int ok) && ks.GetList(QuestCompleteAck) && ((Ok = ok) == ok);
}

public sealed class KEGS_ATTENDANCE_CHECK_ACK : IKSerializable
{
    public int Ok { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok);
    public bool Get(KSerializer ks, RegionProfile region) => ks.Get(out int ok) && ((Ok = ok) == ok);
}

public sealed class KEGS_BATTLE_FIELD_EVENT_MESSAGE_NOT : IKSerializable
{
    public const sbyte BetNone = 0;
    public const sbyte BetWarningMessage = 1;
    public sbyte EventType { get; set; } = BetNone;
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(EventType);
    public bool Get(KSerializer ks, RegionProfile region) => ks.Get(out sbyte et) && ((EventType = et) == et);
}

public sealed class KEGS_BUY_UI_SETTING_REQ : IKSerializable
{
    public int TimeControlItemType { get; set; }
    public int HouseId { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(TimeControlItemType) && ks.Put(HouseId);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int t) || !ks.Get(out int h)) return false;
        TimeControlItemType = t; HouseId = h;
        return true;
    }
}

public sealed class KEGS_BUY_UI_SETTING_ACK : IKSerializable
{
    public int Ok { get; set; }
    public HashSet<int> GetItemOnOff { get; set; } = new();
    public int HouseId { get; set; }
    public int TimeControlItemType { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.PutList(GetItemOnOff, static (s, v) => s.Put(v)) &&
        ks.Put(HouseId) && ks.Put(TimeControlItemType);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.GetHashSet(GetItemOnOff, static s => (s.Get(out int v), v)) ||
            !ks.Get(out int hid) || !ks.Get(out int tcit))
            return false;
        Ok = ok; HouseId = hid; TimeControlItemType = tcit;
        return true;
    }
}

public sealed class KEGS_CATCH_HACKUSER_INFO_NOT : IKSerializable
{
    public long UserUid { get; set; }
    public long UnitUid { get; set; }
    public int CrashType { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(UserUid) && ks.Put(UnitUid) && ks.Put(CrashType);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long u) || !ks.Get(out long c) || !ks.Get(out int ct)) return false;
        UserUid = u; UnitUid = c; CrashType = ct;
        return true;
    }
}

public sealed class KEGS_CHANGED_PET_INTIMACY_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public long PetUid { get; set; }
    public int CurrentIntimacy { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(UnitUid) && ks.Put(PetUid) && ks.Put(CurrentIntimacy);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long u) || !ks.Get(out long p) || !ks.Get(out int ci)) return false;
        UnitUid = u; PetUid = p; CurrentIntimacy = ci;
        return true;
    }
}

public sealed class KEGS_CHANGE_NICK_NAME_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public string NickName { get; set; } = string.Empty;
    public bool CheckOnly { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(UnitUid) && ks.PutWideString(NickName) && ks.Put(CheckOnly);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long u) || !ks.GetWideString(out var n) || !ks.Get(out bool co)) return false;
        UnitUid = u; NickName = n; CheckOnly = co;
        return true;
    }
}

public sealed class KEGS_CHANGE_NICK_NAME_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long UnitUid { get; set; }
    public string NickName { get; set; } = string.Empty;
    public bool CheckOnly { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(UnitUid) && ks.PutWideString(NickName) && ks.Put(CheckOnly);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out long u) || !ks.GetWideString(out var n) || !ks.Get(out bool co)) return false;
        Ok = ok; UnitUid = u; NickName = n; CheckOnly = co;
        return true;
    }
}

public sealed class KEGS_CHANGE_OPTION_PLAY_GUIDE_REQ : IKSerializable
{
    public bool On { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(On);
    public bool Get(KSerializer ks, RegionProfile region) => ks.Get(out bool v) && ((On = v) == v);
}

public sealed class KEGS_CHANGE_OPTION_PLAY_GUIDE_ACK : IKSerializable
{
    public int Ok { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok);
    public bool Get(KSerializer ks, RegionProfile region) => ks.Get(out int ok) && ((Ok = ok) == ok);
}

public sealed class KEGS_CHARACTER_LIST_ACK : IKSerializable
{
    public int Ok { get; set; }
    public Dictionary<int, int> ServerGroupUnitSlot { get; set; } = new();
    public Dictionary<int, List<KUnitInfo>> ServerGroupUnitInfo { get; set; } = new();
    public string UserId { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) &&
        ks.PutMap(ServerGroupUnitSlot, static (s, k) => s.Put(k), static (s, v) => s.Put(v)) &&
        ks.PutMap(ServerGroupUnitInfo, static (s, k) => s.Put(k), static (s, v) => s.PutList(v)) &&
        ks.PutWideString(UserId);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) ||
            !ks.GetMap(ServerGroupUnitSlot, static s => (s.Get(out int k), k), static s => (s.Get(out int v), v)) ||
            !ks.GetMap(ServerGroupUnitInfo, static s => (s.Get(out int k), k), static s => { var lst = new List<KUnitInfo>(); return (s.GetList(lst), lst); }) ||
            !ks.GetWideString(out var uid))
            return false;
        Ok = ok; UserId = uid;
        return true;
    }
}

public sealed class KEGS_CHARACTER_LIST_1ST_ACK : IKSerializable
{
    public int Ok { get; set; }
    public Dictionary<int, int> ServerGroupUnitSlot { get; set; } = new();
    public Dictionary<int, List<KUnitInfo>> ServerGroupUnitInfo { get; set; } = new();
    public string UserId { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) &&
        ks.PutMap(ServerGroupUnitSlot, static (s, k) => s.Put(k), static (s, v) => s.Put(v)) &&
        ks.PutMap(ServerGroupUnitInfo, static (s, k) => s.Put(k), static (s, v) => s.PutList(v)) &&
        ks.PutWideString(UserId);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) ||
            !ks.GetMap(ServerGroupUnitSlot, static s => (s.Get(out int k), k), static s => (s.Get(out int v), v)) ||
            !ks.GetMap(ServerGroupUnitInfo, static s => (s.Get(out int k), k), static s => { var lst = new List<KUnitInfo>(); return (s.GetList(lst), lst); }) ||
            !ks.GetWideString(out var uid))
            return false;
        Ok = ok; UserId = uid;
        return true;
    }
}

public sealed class KEGS_CHAT_OPTION_INFO_WRITE_REQ : IKSerializable
{
    public byte Index { get; set; }
    public bool Value { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Index) && ks.Put(Value);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out byte idx) || !ks.Get(out bool val)) return false;
        Index = idx; Value = val;
        return true;
    }
}

public sealed class KEGS_CHAT_OPTION_INFO_WRITE_ACK : IKSerializable
{
    public int Ok { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok);
    public bool Get(KSerializer ks, RegionProfile region) => ks.Get(out int ok) && ((Ok = ok) == ok);
}

public sealed class KEGS_CHECK_TIME_EVENT_COMPLETE_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int RewardCount { get; set; }
    public List<KInventoryItemInfo> InventorySlotInfo { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.Put(RewardCount) && ks.PutList(InventorySlotInfo);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out int rc) || !ks.GetList(InventorySlotInfo)) return false;
        Ok = ok; RewardCount = rc;
        return true;
    }
}

public sealed class KEGS_CLIENT_POPRT_CHANGE_REQUEST_INFO_NOT : IKSerializable
{
    public int UdpPortSuccessType { get; set; }
    public int UseUdpPort { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(UdpPortSuccessType) && ks.Put(UseUdpPort);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int t) || !ks.Get(out int p)) return false;
        UdpPortSuccessType = t; UseUdpPort = p;
        return true;
    }
}

public sealed class KEGS_COUPON_ENTRY_REQ : IKSerializable
{
    public int CouponType { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(CouponType);
    public bool Get(KSerializer ks, RegionProfile region) => ks.Get(out int ct) && ((CouponType = ct) == ct);
}

public sealed class KEGS_COUPON_ENTRY_ACK : IKSerializable
{
    public int Ok { get; set; }
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.PutList(UpdatedInventorySlot);
    public bool Get(KSerializer ks, RegionProfile region) =>
        ks.Get(out int ok) && ks.GetList(UpdatedInventorySlot) && ((Ok = ok) == ok);
}

public sealed class KEGS_COUPON_ITEM_CHECK_REQ : IKSerializable
{
    public int ButtonType { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(ButtonType);
    public bool Get(KSerializer ks, RegionProfile region) => ks.Get(out int bt) && ((ButtonType = bt) == bt);
}

public sealed class KEGS_COUPON_ITEM_CHECK_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int CouponType { get; set; }
    public int ItemId { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.Put(CouponType) && ks.Put(ItemId);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out int ct) || !ks.Get(out int iid)) return false;
        Ok = ok; CouponType = ct; ItemId = iid;
        return true;
    }
}

public sealed class KEGS_CREATE_NEW_UNIT_REQ : IKSerializable
{
    public string NickName { get; set; } = string.Empty;
    public int Class { get; set; }
    public int ServerGroup { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.PutWideString(NickName) && ks.Put(Class) && ks.Put(ServerGroup);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out var n) || !ks.Get(out int c) || !ks.Get(out int sg)) return false;
        NickName = n; Class = c; ServerGroup = sg;
        return true;
    }
}

public sealed class KEGS_CREATE_PARTY_REQ : IKSerializable
{
    public KPartyInfo PartyInfo { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(PartyInfo);
    public bool Get(KSerializer ks, RegionProfile region) => ks.Get(PartyInfo);
}

public sealed class KEGS_CREATE_PARTY_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KPartyInfo PartyInfo { get; set; } = new();
    public KPartyUserInfo PartyUserInfo { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.Put(PartyInfo) && ks.Put(PartyUserInfo);
    public bool Get(KSerializer ks, RegionProfile region) =>
        ks.Get(out int ok) && ks.Get(PartyInfo) && ks.Get(PartyUserInfo) && ((Ok = ok) == ok);
}

public sealed class KEGS_CURRENT_TIME_ACK : IKSerializable
{
    public string CurrentTime { get; set; } = string.Empty;
    public long CurrentTimeUnix { get; set; } // SERVER_CURRENT_TIME_INT64
    public bool Put(KSerializer ks, RegionProfile region) => ks.PutWideString(CurrentTime) && ks.Put(CurrentTimeUnix);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out var ct) || !ks.Get(out long u)) return false;
        CurrentTime = ct; CurrentTimeUnix = u;
        return true;
    }
}

public sealed class KEGS_DECIDE_TO_USE_THIS_SKILL_PAGE_REQ : IKSerializable
{
    public int SkillPagesNumberDecidedToUse { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(SkillPagesNumberDecidedToUse);
    public bool Get(KSerializer ks, RegionProfile region) =>
        ks.Get(out int p) && ((SkillPagesNumberDecidedToUse = p) == p);
}

public sealed class KEGS_DECIDE_TO_USE_THIS_SKILL_PAGE_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int SkillPagesNumberDecidedToUse { get; set; }
    public int SPointAvailable { get; set; }
    public int CsPointAvailable { get; set; }
    public KUserSkillPageData UserSkillPageData { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(SkillPagesNumberDecidedToUse) && ks.Put(SPointAvailable) &&
        ks.Put(CsPointAvailable) && ks.Put(UserSkillPageData);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out int p) || !ks.Get(out int sp) ||
            !ks.Get(out int csp) || !ks.Get(UserSkillPageData))
            return false;
        Ok = ok; SkillPagesNumberDecidedToUse = p; SPointAvailable = sp; CsPointAvailable = csp;
        return true;
    }
}

public sealed class KEGS_DEL_TUTORIAL_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(UnitUid);
    public bool Get(KSerializer ks, RegionProfile region) => ks.Get(out long u) && ((UnitUid = u) == u);
}

public sealed class KEGS_DEL_TUTORIAL_ACK : IKSerializable
{
    public int Ok { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok);
    public bool Get(KSerializer ks, RegionProfile region) => ks.Get(out int ok) && ((Ok = ok) == ok);
}

public sealed class KEGS_DEL_TUTORIAL_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public int Reason { get; set; } // SERV_NETERROR_STR_GET_FROM_CLIENT
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(UnitUid) && ks.Put(Reason);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long u) || !ks.Get(out int r)) return false;
        UnitUid = u; Reason = r;
        return true;
    }
}

public sealed class KEGS_ENTER_NPC_HOUSE_REQ : IKSerializable
{
    public int NpcId { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(NpcId);
    public bool Get(KSerializer ks, RegionProfile region) => ks.Get(out int nid) && ((NpcId = nid) == nid);
}

public sealed class KEGS_ENTER_NPC_HOUSE_ACK : IKSerializable
{
    public int Ok { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok);
    public bool Get(KSerializer ks, RegionProfile region) => ks.Get(out int ok) && ((Ok = ok) == ok);
}

public sealed class KEGS_ENTER_EQUIP_SHOP_ACK : IKSerializable
{
    public int Ok { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok);
    public bool Get(KSerializer ks, RegionProfile region) => ks.Get(out int ok) && ((Ok = ok) == ok);
}

public sealed class KEGS_ENTRY_POINT_CHANGE_NICK_NAME_REQ : IKSerializable
{
    public int ServerGroup { get; set; }
    public long UnitUid { get; set; }
    public string NickName { get; set; } = string.Empty;
    public bool CheckOnly { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(ServerGroup) && ks.Put(UnitUid) && ks.PutWideString(NickName) && ks.Put(CheckOnly);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int sg) || !ks.Get(out long u) || !ks.GetWideString(out var n) || !ks.Get(out bool co)) return false;
        ServerGroup = sg; UnitUid = u; NickName = n; CheckOnly = co;
        return true;
    }
}

public sealed class KEGS_ENTRY_POINT_CHECK_NICK_NAME_REQ : IKSerializable
{
    public int ServerGroup { get; set; }
    public string NickName { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(ServerGroup) && ks.PutWideString(NickName);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int sg) || !ks.GetWideString(out var n)) return false;
        ServerGroup = sg; NickName = n;
        return true;
    }
}

public sealed class KEGS_ENTRY_POINT_CHECK_NICK_NAME_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long UnitUid { get; set; }
    public string NickName { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.Put(UnitUid) && ks.PutWideString(NickName);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out long u) || !ks.GetWideString(out var n)) return false;
        Ok = ok; UnitUid = u; NickName = n;
        return true;
    }
}

public sealed class KEGS_ENTRY_POINT_DELETE_UNIT_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public long UserUid { get; set; }
    public int ServerGroup { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(UnitUid) && ks.Put(UserUid) && ks.Put(ServerGroup);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long u) || !ks.Get(out long usr) || !ks.Get(out int sg)) return false;
        UnitUid = u; UserUid = usr; ServerGroup = sg;
        return true;
    }
}

public sealed class KEGS_ERROR_POPUP_NOT : IKSerializable
{
    public int Ok { get; set; }
    public int ErrorData1 { get; set; }
    public int ErrorData2 { get; set; }
    public bool PartyMember { get; set; } // SERV_CHANGE_ERROR_POPUP
    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(ErrorData1) && ks.Put(ErrorData2) && ks.Put(PartyMember);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out int d1) || !ks.Get(out int d2) || !ks.Get(out bool pm)) return false;
        Ok = ok; ErrorData1 = d1; ErrorData2 = d2; PartyMember = pm;
        return true;
    }
}

public sealed class KEGS_EVENT_CHUNG_GIVE_ITEM_REQ : IKSerializable
{
    public int Choice { get; set; }
    public bool TwoGiveItem { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Choice) && ks.Put(TwoGiveItem);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int c) || !ks.Get(out bool t)) return false;
        Choice = c; TwoGiveItem = t;
        return true;
    }
}

public sealed class KEGS_EVENT_CHUNG_GIVE_ITEM_ACK : IKSerializable
{
    public int Ok { get; set; }
    public string GetItemTime { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.PutWideString(GetItemTime);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.GetWideString(out var t)) return false;
        Ok = ok; GetItemTime = t;
        return true;
    }
}

public sealed class KEGS_EVENT_COBO_DUNGEON_FIELD_REQ : IKSerializable
{
    public int Ok { get; set; }
    public bool EventStart { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.Put(EventStart);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out bool es)) return false;
        Ok = ok; EventStart = es;
        return true;
    }
}

public sealed class KEGS_EVENT_COBO_DUNGEON_FIELD_NOT : IKSerializable
{
    public bool StartUi { get; set; }
    public bool DungeonCountUi { get; set; }
    public bool FieldCountUi { get; set; }
    public int DungeonCount { get; set; }
    public int FieldMonsterKillCount { get; set; }
    public int Ok { get; set; }
    public int RemaindTime { get; set; } = -1;
    public long PushTimeUnix { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(StartUi) && ks.Put(DungeonCountUi) && ks.Put(FieldCountUi) &&
        ks.Put(DungeonCount) && ks.Put(FieldMonsterKillCount) && ks.Put(Ok) &&
        ks.Put(RemaindTime) && ks.Put(PushTimeUnix);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool su) || !ks.Get(out bool dcu) || !ks.Get(out bool fcu) ||
            !ks.Get(out int dc) || !ks.Get(out int fkc) || !ks.Get(out int ok) ||
            !ks.Get(out int rt) || !ks.Get(out long ptu))
            return false;
        StartUi = su; DungeonCountUi = dcu; FieldCountUi = fcu;
        DungeonCount = dc; FieldMonsterKillCount = fkc; Ok = ok;
        RemaindTime = rt; PushTimeUnix = ptu;
        return true;
    }
}

public sealed class KEGS_GUILD_INFO_NOT : IKSerializable
{
    public KGuildInfo GuildInfo { get; set; } = new();
    public string GuildMasterName { get; set; } = string.Empty;
    public List<long> MemberUnitUid { get; set; } = new();
    public KGuildSkillInfo GuildSkillInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(GuildInfo) && ks.PutWideString(GuildMasterName) &&
        ks.PutList(MemberUnitUid, static (s, uid) => s.Put(uid)) &&
        ks.Put(GuildSkillInfo);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(GuildInfo) || !ks.GetWideString(out string master) ||
            !ks.GetList(MemberUnitUid, static s => (s.Get(out long uid), uid)) ||
            !ks.Get(GuildSkillInfo))
            return false;
        GuildMasterName = master;
        return true;
    }
}

public sealed class KEGS_EVENT_COBO_DUNGEON_FIELD_ACK : IKSerializable
{
    public bool StartUi { get; set; }
    public bool DungeonCountUi { get; set; }
    public bool FieldCountUi { get; set; }
    public int DungeonCount { get; set; }
    public int FieldMonsterKillCount { get; set; }
    public int Ok { get; set; }
    public int RemaindTime { get; set; } = -1;
    public string PushTime { get; set; } = string.Empty;
    public long PushTimeUnix { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(StartUi) && ks.Put(DungeonCountUi) && ks.Put(FieldCountUi) &&
        ks.Put(DungeonCount) && ks.Put(FieldMonsterKillCount) && ks.Put(Ok) &&
        ks.Put(RemaindTime) && ks.PutWideString(PushTime) && ks.Put(PushTimeUnix);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool su) || !ks.Get(out bool dcu) || !ks.Get(out bool fcu) ||
            !ks.Get(out int dc) || !ks.Get(out int fkc) || !ks.Get(out int ok) ||
            !ks.Get(out int rt) || !ks.GetWideString(out var pt) || !ks.Get(out long ptu))
            return false;
        StartUi = su; DungeonCountUi = dcu; FieldCountUi = fcu;
        DungeonCount = dc; FieldMonsterKillCount = fkc; Ok = ok;
        RemaindTime = rt; PushTime = pt; PushTimeUnix = ptu;
        return true;
    }
}

public sealed class KEGS_EVENT_NOTE_VIEW_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int TitleId { get; set; }
    public int NoteViewCount { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.Put(TitleId) && ks.Put(NoteViewCount);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out int tid) || !ks.Get(out int nvc)) return false;
        Ok = ok; TitleId = tid; NoteViewCount = nvc;
        return true;
    }
}

public sealed class KEGS_EXPAND_INVENTORY_ED_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int Ed { get; set; }
    public Dictionary<int, int> ExpandedCategorySlot { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(Ed) && ks.PutMap(ExpandedCategorySlot, static (s, k) => s.Put(k), static (s, v) => s.Put(v));
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out int ed) ||
            !ks.GetMap(ExpandedCategorySlot, static s => (s.Get(out int k), k), static s => (s.Get(out int v), v)))
            return false;
        Ok = ok; Ed = ed;
        return true;
    }
}

public sealed class KEGS_EXPAND_SKILL_PAGE_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int Ed { get; set; }
    public int TheNumberOfSkillPagesAvailable { get; set; }
    public int SPointAvailable { get; set; }
    public int CsPointAvailable { get; set; }
    public List<int> SkillListLearned { get; set; } = new();
    public List<short> UnsealedSkillId { get; set; } = new();
    public bool PayWithEd { get; set; } = true;

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(Ed) && ks.Put(TheNumberOfSkillPagesAvailable) &&
        ks.Put(SPointAvailable) && ks.Put(CsPointAvailable) &&
        ks.PutList(SkillListLearned, static (s, v) => s.Put(v)) &&
        ks.PutList(UnsealedSkillId, static (s, v) => s.Put(v)) &&
        ks.Put(PayWithEd);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out int ed) || !ks.Get(out int np) ||
            !ks.Get(out int sp) || !ks.Get(out int csp) ||
            !ks.GetList(SkillListLearned, static s => (s.Get(out int v), v)) ||
            !ks.GetList(UnsealedSkillId, static s => (s.Get(out short v), v)) ||
            !ks.Get(out bool pwe))
            return false;
        Ok = ok; Ed = ed; TheNumberOfSkillPagesAvailable = np;
        SPointAvailable = sp; CsPointAvailable = csp; PayWithEd = pwe;
        return true;
    }
}

public sealed class KEGS_FINAL_DELETE_UNIT_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public long UserUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(UnitUid) && ks.Put(UserUid);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long u) || !ks.Get(out long usr)) return false;
        UnitUid = u; UserUid = usr;
        return true;
    }
}

public sealed class KEGS_FINAL_DELETE_UNIT_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long UnitUid { get; set; }
    public uint KnmSerialNum { get; set; }
    public int GuildUid { get; set; } // GUILD_TEST
    public string NickName { get; set; } = string.Empty; // SERV_CHAR_LOG
    public byte Level { get; set; }
    public sbyte UnitClass { get; set; }
    public long ReDelAbleDate { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(UnitUid) && ks.Put(KnmSerialNum) && ks.Put(GuildUid) &&
        ks.PutWideString(NickName) && ks.Put(Level) && ks.Put(UnitClass) && ks.Put(ReDelAbleDate);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out long u) || !ks.Get(out uint knm) || !ks.Get(out int g) ||
            !ks.GetWideString(out var nick) || !ks.Get(out byte lvl) || !ks.Get(out sbyte uc) || !ks.Get(out long rd))
            return false;
        Ok = ok; UnitUid = u; KnmSerialNum = knm; GuildUid = g; NickName = nick; Level = lvl; UnitClass = uc; ReDelAbleDate = rd;
        return true;
    }
}

public sealed class KEGS_FORCE_RELAY_NOT : IKSerializable
{
    public long TargetUnitUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(TargetUnitUid);
    public bool Get(KSerializer ks, RegionProfile region) => ks.Get(out long u) && ((TargetUnitUid = u) == u);
}

public sealed class KEGS_FRAME_AVERAGE_REQ : IKSerializable
{
    public sbyte GameType { get; set; }
    public int TotalFrame { get; set; }
    public int FrameAverage { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(GameType) && ks.Put(TotalFrame) && ks.Put(FrameAverage);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte gt) || !ks.Get(out int tf) || !ks.Get(out int fa)) return false;
        GameType = gt; TotalFrame = tf; FrameAverage = fa;
        return true;
    }
}

public sealed class KEGS_FRAME_AVERAGE_ACK : IKSerializable
{
    public int Ok { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok);
    public bool Get(KSerializer ks, RegionProfile region) => ks.Get(out int ok) && ((Ok = ok) == ok);
}

public sealed class KEGS_GATHER_GIVE_UP_QUEST_REQ : IKSerializable
{
    public List<int> GatherQuestId { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region) => ks.PutList(GatherQuestId, static (s, v) => s.Put(v));
    public bool Get(KSerializer ks, RegionProfile region) => ks.GetList(GatherQuestId, static s => (s.Get(out int v), v));
}

public sealed class KEGS_GET_ADVERTISEMENT_EVENT_ACK : IKSerializable
{
    public List<string> AdvertisementUrl { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region) => ks.PutList(AdvertisementUrl, static (s, v) => s.PutWideString(v));
    public bool Get(KSerializer ks, RegionProfile region) => ks.GetList(AdvertisementUrl, static s => (s.GetWideString(out var v), v));
}

public sealed class KEGS_SEARCH_UNIT_REQ : IKSerializable
{
    public string NickName { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region) => ks.PutWideString(NickName);
    public bool Get(KSerializer ks, RegionProfile region) => ks.GetWideString(out var n) && ((NickName = n) == n);
}

public sealed class KEGS_SEARCH_UNIT_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KConnectionUnitInfo CUnitInfo { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.Put(CUnitInfo);
    public bool Get(KSerializer ks, RegionProfile region) =>
        ks.Get(out int ok) && ks.Get(CUnitInfo) && ((Ok = ok) == ok);
}

public sealed class KEGS_GET_CONNECTION_UNIT_INFO_REQ : IKSerializable
{
    public string NickName { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region) => ks.PutWideString(NickName);
    public bool Get(KSerializer ks, RegionProfile region) => ks.GetWideString(out var n) && ((NickName = n) == n);
}

public sealed class KEGS_GET_CONNECTION_UNIT_INFO_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KConnectionUnitInfo CUnitInfo { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.Put(CUnitInfo);
    public bool Get(KSerializer ks, RegionProfile region) =>
        ks.Get(out int ok) && ks.Get(CUnitInfo) && ((Ok = ok) == ok);
}

public sealed class KEGS_GET_CREATE_UNIT_TODAY_COUNT_ACK : IKSerializable
{
    public long UserUid { get; set; }
    public Dictionary<int, int> CreateCharCountToday { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(UserUid) && ks.PutMap(CreateCharCountToday, static (s, k) => s.Put(k), static (s, v) => s.Put(v));
    public bool Get(KSerializer ks, RegionProfile region) =>
        ks.Get(out long u) &&
        ks.GetMap(CreateCharCountToday, static s => (s.Get(out int k), k), static s => (s.Get(out int v), v)) &&
        ((UserUid = u) == u);
}

public sealed class KEGS_GET_EVENT_INFO_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KEventCharacterRanking EventCharacterRanking { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.Put(EventCharacterRanking);
    public bool Get(KSerializer ks, RegionProfile region) =>
        ks.Get(out int ok) && ks.Get(EventCharacterRanking) && ((Ok = ok) == ok);
}

public sealed class KEGS_GET_NEXT_SKILL_PAGE_ED_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int Ed { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.Put(Ed);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out int ed)) return false;
        Ok = ok; Ed = ed;
        return true;
    }
}

public sealed class KEGS_GET_RANKING_INFO_REQ : IKSerializable
{
    public int RankingType { get; set; }
    public uint ViewPage { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(RankingType) && ks.Put(ViewPage);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int rt) || !ks.Get(out uint vp)) return false;
        RankingType = rt; ViewPage = vp;
        return true;
    }
}

public sealed class KEGS_GET_RANKING_INFO_ACK : IKSerializable
{
    public int RankingType { get; set; }
    public uint ViewPage { get; set; }
    public uint TotalPage { get; set; }
    public List<KHenirRankingInfo> HenirRankingPage { get; set; } = new();
    public List<KDungeonRankingInfo> DungeonRankingPage { get; set; } = new();
    public List<KPvpRankingInfo> PvpRankingPage { get; set; } = new();
    public List<KHenirRankingInfo> MyHenirRanking { get; set; } = new();
    public List<KDungeonRankingInfo> MyDungeonRanking { get; set; } = new();
    public List<KPvpRankingInfo> MyPvpRanking { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(RankingType) && ks.Put(ViewPage) && ks.Put(TotalPage) &&
        ks.PutList(HenirRankingPage) && ks.PutList(DungeonRankingPage) && ks.PutList(PvpRankingPage) &&
        ks.PutList(MyHenirRanking) && ks.PutList(MyDungeonRanking) && ks.PutList(MyPvpRanking);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int rt) || !ks.Get(out uint vp) || !ks.Get(out uint tp) ||
            !ks.GetList(HenirRankingPage) || !ks.GetList(DungeonRankingPage) || !ks.GetList(PvpRankingPage) ||
            !ks.GetList(MyHenirRanking) || !ks.GetList(MyDungeonRanking) || !ks.GetList(MyPvpRanking))
            return false;
        RankingType = rt; ViewPage = vp; TotalPage = tp;
        return true;
    }
}

public sealed class KEGS_GET_RECOMMEND_USER_LIST_REQ : IKSerializable
{
    public uint ViewPage { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(ViewPage);
    public bool Get(KSerializer ks, RegionProfile region) => ks.Get(out uint vp) && ((ViewPage = vp) == vp);
}

public sealed class KEGS_GET_RECOMMEND_USER_LIST_ACK : IKSerializable
{
    public int Ok { get; set; }
    public uint TotalPage { get; set; }
    public uint ViewPage { get; set; }
    public List<KRecommendUserInfo> RecommendUserList { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(TotalPage) && ks.Put(ViewPage) && ks.PutList(RecommendUserList);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out uint tp) || !ks.Get(out uint vp) || !ks.GetList(RecommendUserList))
            return false;
        Ok = ok; TotalPage = tp; ViewPage = vp;
        return true;
    }
}

public sealed class KEGS_GET_RECRUIT_RECRUITER_LIST_ACK : IKSerializable
{
    public int Ok { get; set; }
    public List<KRecommendUserInfo> RecruitUnitInfo { get; set; } = new();
    public List<KRecommendUserInfo> RecruiterUnitInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.PutList(RecruitUnitInfo) && ks.PutList(RecruiterUnitInfo);

    public bool Get(KSerializer ks, RegionProfile region) =>
        ks.Get(out int ok) && ks.GetList(RecruitUnitInfo) && ks.GetList(RecruiterUnitInfo) && ((Ok = ok) == ok);
}

public sealed class KEGS_GET_TIME_CONTROL_ITME_TALK_LIST_REQ : IKSerializable
{
    public int Ok { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok);
    public bool Get(KSerializer ks, RegionProfile region) => ks.Get(out int ok) && ((Ok = ok) == ok);
}

public sealed class KEGS_GET_TIME_CONTROL_ITME_TALK_LIST_ACK : IKSerializable
{
    public Dictionary<int, List<KPacketGetItemOnOff>> MapGetItemOnOff { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.PutMap(MapGetItemOnOff, static (s, k) => s.Put(k), static (s, v) => s.PutList(v));

    public bool Get(KSerializer ks, RegionProfile region) =>
        ks.GetMap(MapGetItemOnOff, static s => (s.Get(out int k), k), static s => { var lst = new List<KPacketGetItemOnOff>(); return (s.GetList(lst), lst); });
}

public sealed class KEGS_JOIN_PARTY_REQ : IKSerializable
{
    public long PartyUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(PartyUid);
    public bool Get(KSerializer ks, RegionProfile region) => ks.Get(out long p) && ((PartyUid = p) == p);
}

public sealed class KEGS_JOIN_PARTY_ACK : IKSerializable
{
    public int Ok { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok);
    public bool Get(KSerializer ks, RegionProfile region) => ks.Get(out int ok) && ((Ok = ok) == ok);
}

public sealed class KEGS_JOIN_PARTY_INVITE_REPLY_NOT : IKSerializable
{
    public const sbyte RrAccept = 0;
    public const sbyte RrNotAccept = 1;
    public const sbyte RrBusy = 2;

    public long UnitUid { get; set; }
    public sbyte ReplyReason { get; set; } // SERV_PARTY_INVITE_REPLY_FIX

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(UnitUid) && ks.Put(ReplyReason);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long u) || !ks.Get(out sbyte r)) return false;
        UnitUid = u; ReplyReason = r;
        return true;
    }
}

public sealed class KEGS_JUMPING_CHARACTER_REQ : IKSerializable
{
    public int Class { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Class);
    public bool Get(KSerializer ks, RegionProfile region) => ks.Get(out int c) && ((Class = c) == c);
}

public sealed class KEGS_JUMPING_CHARACTER_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KUnitInfo UnitInfo { get; set; } = new();
    public sbyte ExpandedMaxPageNum { get; set; }
    public List<int> NewDefaultSkill { get; set; } = new(); // SERV_UPGRADE_SKILL_SYSTEM_2013
    public List<short> UnSealedSkill { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(UnitInfo) && ks.Put(ExpandedMaxPageNum) &&
        ks.PutList(NewDefaultSkill, static (s, v) => s.Put(v)) &&
        ks.PutList(UnSealedSkill, static (s, v) => s.Put(v));

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(UnitInfo) || !ks.Get(out sbyte emp) ||
            !ks.GetList(NewDefaultSkill, static s => (s.Get(out int v), v)) ||
            !ks.GetList(UnSealedSkill, static s => (s.Get(out short v), v)))
            return false;
        Ok = ok; ExpandedMaxPageNum = emp;
        return true;
    }
}

public sealed class KEGS_KEYBOARD_MAPPING_INFO_WRITE_REQ : IKSerializable
{
    public Dictionary<short, short> KeyboardMappingInfo { get; set; } = new();
    public Dictionary<short, short> GamePadMappingInfo { get; set; } = new(); // SERV_KEY_MAPPING_INT

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.PutMap(KeyboardMappingInfo, static (s, k) => s.Put(k), static (s, v) => s.Put(v)) &&
        ks.PutMap(GamePadMappingInfo, static (s, k) => s.Put(k), static (s, v) => s.Put(v));

    public bool Get(KSerializer ks, RegionProfile region) =>
        ks.GetMap(KeyboardMappingInfo, static s => (s.Get(out short k), k), static s => (s.Get(out short v), v)) &&
        ks.GetMap(GamePadMappingInfo, static s => (s.Get(out short k), k), static s => (s.Get(out short v), v));
}

public sealed class KEGS_KEYBOARD_MAPPING_INFO_WRITE_ACK : IKSerializable
{
    public int Ok { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok);
    public bool Get(KSerializer ks, RegionProfile region) => ks.Get(out int ok) && ((Ok = ok) == ok);
}

public sealed class KEGS_KOM_FILE_CHECK_LOG_REQ : IKSerializable
{
    public string InvalidKomName { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region) => ks.PutWideString(InvalidKomName);
    public bool Get(KSerializer ks, RegionProfile region) => ks.GetWideString(out var s) && ((InvalidKomName = s) == s);
}

public sealed class KEGS_KOM_FILE_CHECK_LOG_ACK : IKSerializable
{
    public string InvalidKomName { get; set; } = string.Empty;
    public int Ok { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.PutWideString(InvalidKomName) && ks.Put(Ok);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out var s) || !ks.Get(out int ok)) return false;
        InvalidKomName = s; Ok = ok;
        return true;
    }
}

public sealed class KEGS_LOCAL_RANKING_INQUIRY_REQ : IKSerializable
{
    public uint ViewPage { get; set; }
    public sbyte MainTabIndex { get; set; } = -1;
    public sbyte SubTabIndex { get; set; } = -1;
    public byte Filter { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(ViewPage) && ks.Put(MainTabIndex) && ks.Put(SubTabIndex) && ks.Put(Filter);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint vp) || !ks.Get(out sbyte m) || !ks.Get(out sbyte st) || !ks.Get(out byte f)) return false;
        ViewPage = vp; MainTabIndex = m; SubTabIndex = st; Filter = f;
        return true;
    }
}

public sealed class KEGS_LOCAL_RANKING_INQUIRY_ACK : IKSerializable
{
    public int Ok { get; set; }
    public uint TotalPage { get; set; }
    public uint ViewPage { get; set; }
    public sbyte MainTabIndex { get; set; } = -1;
    public sbyte SubTabIndex { get; set; } = -1;
    public byte Filter { get; set; }
    public List<KLocalRankingRankerInfo> RankerList { get; set; } = new();
    public KLocalRankingRankerInfo PrevRankerInfo { get; set; } = new();
    public KLocalRankingRankerInfo MyRankInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(TotalPage) && ks.Put(ViewPage) && ks.Put(MainTabIndex) &&
        ks.Put(SubTabIndex) && ks.Put(Filter) && ks.PutList(RankerList) &&
        ks.Put(PrevRankerInfo) && ks.Put(MyRankInfo);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out uint tp) || !ks.Get(out uint vp) ||
            !ks.Get(out sbyte m) || !ks.Get(out sbyte st) || !ks.Get(out byte f) ||
            !ks.GetList(RankerList) || !ks.Get(PrevRankerInfo) || !ks.Get(MyRankInfo))
            return false;
        Ok = ok; TotalPage = tp; ViewPage = vp; MainTabIndex = m; SubTabIndex = st; Filter = f;
        return true;
    }
}

public sealed class KEGS_LOCAL_RANKING_USER_INFO_READ_REQ : IKSerializable
{
    public string Password { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region) => ks.PutWideString(Password);
    public bool Get(KSerializer ks, RegionProfile region) => ks.GetWideString(out var pw) && ((Password = pw) == pw);
}

public sealed class KEGS_LOCAL_RANKING_USER_INFO_READ_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KLocalRankingUserInfo Info { get; set; } = new();
    public string Updated { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.Put(Info) && ks.PutWideString(Updated);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(Info) || !ks.GetWideString(out var u)) return false;
        Ok = ok; Updated = u;
        return true;
    }
}

public sealed class KEGS_LOCAL_RANKING_USER_INFO_WRITE_REQ : IKSerializable
{
    public string Password { get; set; } = string.Empty;
    public KLocalRankingUserInfo Info { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region) => ks.PutWideString(Password) && ks.Put(Info);
    public bool Get(KSerializer ks, RegionProfile region) =>
        ks.GetWideString(out var pw) && ks.Get(Info) && ((Password = pw) == pw);
}

public sealed class KEGS_LOCAL_RANKING_USER_INFO_WRITE_ACK : IKSerializable
{
    public int Ok { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok);
    public bool Get(KSerializer ks, RegionProfile region) => ks.Get(out int ok) && ((Ok = ok) == ok);
}

public sealed class KEGS_LOCAL_RANKING_WATCH_UNIT_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(UnitUid);
    public bool Get(KSerializer ks, RegionProfile region) => ks.Get(out long u) && ((UnitUid = u) == u);
}

public sealed class KEGS_LOCAL_RANKING_WATCH_UNIT_ACK : IKSerializable
{
    public int Ok { get; set; } = -1;
    public KLocalRankingUserInfo UserInfo { get; set; } = new();
    public KLocalRankingUnitInfo UnitInfo { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.Put(UserInfo) && ks.Put(UnitInfo);
    public bool Get(KSerializer ks, RegionProfile region) =>
        ks.Get(out int ok) && ks.Get(UserInfo) && ks.Get(UnitInfo) && ((Ok = ok) == ok);
}

public sealed class KEGS_MANUFACTURE_PERIOD_SETTING_REQ : IKSerializable
{
    public int HouseId { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(HouseId);
    public bool Get(KSerializer ks, RegionProfile region) => ks.Get(out int hid) && ((HouseId = hid) == hid);
}

public sealed class KEGS_MANUFACTURE_PERIOD_SETTING_ACK : IKSerializable
{
    public int Ok { get; set; }
    public Dictionary<int, int> PeriodGroup { get; set; } = new();
    public int HouseId { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.PutMap(PeriodGroup, static (s, k) => s.Put(k), static (s, v) => s.Put(v)) && ks.Put(HouseId);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.GetMap(PeriodGroup, static s => (s.Get(out int k), k), static s => (s.Get(out int v), v)) || !ks.Get(out int hid))
            return false;
        Ok = ok; HouseId = hid;
        return true;
    }
}

public sealed class KEGS_MEGAPHONE_MESSAGE_NOT : IKSerializable
{
    public string SenderNickName { get; set; } = string.Empty;
    public int SenderChannelId { get; set; }
    public int SenderLocation { get; set; }
    public byte MessageColor { get; set; }
    public string Message { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.PutWideString(SenderNickName) && ks.Put(SenderChannelId) && ks.Put(SenderLocation) &&
        ks.Put(MessageColor) && ks.PutWideString(Message);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out var sn) || !ks.Get(out int sc) || !ks.Get(out int sl) ||
            !ks.Get(out byte mc) || !ks.GetWideString(out var msg))
            return false;
        SenderNickName = sn; SenderChannelId = sc; SenderLocation = sl; MessageColor = mc; Message = msg;
        return true;
    }
}

public sealed class KEGS_MOMOTI_QUIZ_EVENT_REQ : IKSerializable
{
    public int Ok { get; set; }
    public int Reply { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.Put(Reply);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out int r)) return false;
        Ok = ok; Reply = r;
        return true;
    }
}

public sealed class KEGS_MOMOTI_QUIZ_EVENT_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int CheckReward { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.Put(CheckReward);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out int cr)) return false;
        Ok = ok; CheckReward = cr;
        return true;
    }
}

public sealed class KEGS_PARTY_CHANGE_NAME_REQ : IKSerializable
{
    public string PartyName { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region) => ks.PutWideString(PartyName);
    public bool Get(KSerializer ks, RegionProfile region) => ks.GetWideString(out var n) && ((PartyName = n) == n);
}

public sealed class KEGS_PARTY_CHANGE_NAME_ACK : IKSerializable
{
    public int Ok { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok);
    public bool Get(KSerializer ks, RegionProfile region) => ks.Get(out int ok) && ((Ok = ok) == ok);
}

public sealed class KEGS_PARTY_CHANGE_PUBLIC_REQ : IKSerializable
{
    public bool Public { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Public);
    public bool Get(KSerializer ks, RegionProfile region) => ks.Get(out bool p) && ((Public = p) == p);
}

public sealed class KEGS_PARTY_CHANGE_PUBLIC_ACK : IKSerializable
{
    public int Ok { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok);
    public bool Get(KSerializer ks, RegionProfile region) => ks.Get(out int ok) && ((Ok = ok) == ok);
}

public sealed class KEGS_PARTY_QUICK_JOIN_REQ : IKSerializable
{
    public int DungeonId { get; set; }
    public sbyte Difficulty { get; set; }
    public sbyte DungeonMode { get; set; }
    public bool CheckLowLevel { get; set; } // SERV_DUNGEON_DIFFICULTY_REVISION_ADD_OPTION

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(DungeonId) && ks.Put(Difficulty) && ks.Put(DungeonMode) && ks.Put(CheckLowLevel);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int did) || !ks.Get(out sbyte diff) || !ks.Get(out sbyte dm) || !ks.Get(out bool cll))
            return false;
        DungeonId = did; Difficulty = diff; DungeonMode = dm; CheckLowLevel = cll;
        return true;
    }
}

public sealed class KEGS_PARTY_QUICK_JOIN_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long PartyUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.Put(PartyUid);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out long puid)) return false;
        Ok = ok; PartyUid = puid;
        return true;
    }
}

public sealed class KEGS_PARTY_TO_FIELD_MAP_REQ : IKSerializable
{
    public const sbyte PtftNone = 0;
    public const sbyte PtftLoungeToField = 1;
    public const sbyte PtftShipDungeonToField = 2;

    public sbyte PartyToFieldType { get; set; } = PtftLoungeToField;
    public int FieldStartPos { get; set; } = -1;

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(PartyToFieldType) && ks.Put(FieldStartPos);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte t) || !ks.Get(out int pos)) return false;
        PartyToFieldType = t; FieldStartPos = pos;
        return true;
    }
}

public sealed class KEGS_PARTY_TO_FIELD_MAP_ACK : IKSerializable
{
    public int Ok { get; set; }
    public sbyte PartyToFieldType { get; set; }
    public int FieldStartPos { get; set; } = -1;

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.Put(PartyToFieldType) && ks.Put(FieldStartPos);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out sbyte t) || !ks.Get(out int pos)) return false;
        Ok = ok; PartyToFieldType = t; FieldStartPos = pos;
        return true;
    }
}

public sealed class KEGS_RECOMMEND_USER_REQ : IKSerializable
{
    public string NickName { get; set; } = string.Empty;
    public bool Cancel { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.PutWideString(NickName) && ks.Put(Cancel);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out var n) || !ks.Get(out bool c)) return false;
        NickName = n; Cancel = c;
        return true;
    }
}

public sealed class KEGS_RECOMMEND_USER_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int RewardResurrectionStone { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.Put(RewardResurrectionStone);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out int r)) return false;
        Ok = ok; RewardResurrectionStone = r;
        return true;
    }
}

public sealed class KEGS_REG_SKILL_NOTE_MEMO_REQ : IKSerializable
{
    public sbyte SkillNotePageNum { get; set; }
    public long ItemUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(SkillNotePageNum) && ks.Put(ItemUid);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte p) || !ks.Get(out long iid)) return false;
        SkillNotePageNum = p; ItemUid = iid;
        return true;
    }
}

public sealed class KEGS_REG_SKILL_NOTE_MEMO_ACK : IKSerializable
{
    public int Ok { get; set; }
    public sbyte SkillNotePageNum { get; set; }
    public int MemoId { get; set; }
    public List<KInventoryItemInfo> InventorySlotInfo { get; set; } = new();
    public List<int> SkillNote { get; set; } = new(); // SERV_BATTLE_FIELD_SYSTEM

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(SkillNotePageNum) && ks.Put(MemoId) &&
        ks.PutList(InventorySlotInfo) && ks.PutList(SkillNote, static (s, v) => s.Put(v));

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out sbyte p) || !ks.Get(out int mid) ||
            !ks.GetList(InventorySlotInfo) || !ks.GetList(SkillNote, static s => (s.Get(out int v), v)))
            return false;
        Ok = ok; SkillNotePageNum = p; MemoId = mid;
        return true;
    }
}

public sealed class KEGS_REQUEST_TUTORIAL_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(UnitUid);
    public bool Get(KSerializer ks, RegionProfile region) => ks.Get(out long u) && ((UnitUid = u) == u);
}

public sealed class KEGS_REQUEST_TUTORIAL_ACK : IKSerializable
{
    public int Ok { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok);
    public bool Get(KSerializer ks, RegionProfile region) => ks.Get(out int ok) && ((Ok = ok) == ok);
}

public sealed class KEGS_REQUEST_TUTORIAL_NOT : IKSerializable
{
    public KTutorialUnitInfo TutorialUnitInfo { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(TutorialUnitInfo);
    public bool Get(KSerializer ks, RegionProfile region) => ks.Get(TutorialUnitInfo);
}

public sealed class KEGS_REQUEST_TUTORIAL_REPLY_NOT : IKSerializable
{
    public long ReceiverUnitUid { get; set; }
    public KTutorialUnitInfo TutorialUnitInfo { get; set; } = new();
    public bool Accept { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(ReceiverUnitUid) && ks.Put(TutorialUnitInfo) && ks.Put(Accept);
    public bool Get(KSerializer ks, RegionProfile region) =>
        ks.Get(out long r) && ks.Get(TutorialUnitInfo) && ks.Get(out bool a) &&
        ((ReceiverUnitUid = r) == r) && ((Accept = a) == a);
}

public sealed class KEGS_RESTORE_UNIT_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public long UserUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(UnitUid) && ks.Put(UserUid);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long u) || !ks.Get(out long usr)) return false;
        UnitUid = u; UserUid = usr;
        return true;
    }
}

public sealed class KEGS_RESTORE_UNIT_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long UnitUid { get; set; }
    public long RestoreAbleDate { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.Put(UnitUid) && ks.Put(RestoreAbleDate);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out long u) || !ks.Get(out long rd)) return false;
        Ok = ok; UnitUid = u; RestoreAbleDate = rd;
        return true;
    }
}

public sealed class KEGS_SHARE_BANK_UPDATE_OK_NOT : IKSerializable
{
    public int Ok { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok);
    public bool Get(KSerializer ks, RegionProfile region) => ks.Get(out int ok) && ((Ok = ok) == ok);
}

public sealed class KEGS_SKILL_USE_REQ : IKSerializable
{
    public int SkillId { get; set; } = -1;
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(SkillId);
    public bool Get(KSerializer ks, RegionProfile region) => ks.Get(out int sid) && ((SkillId = sid) == sid);
}

public sealed class KEGS_START_CHECK_POWER_REQ : IKSerializable
{
    public bool Start { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Start);
    public bool Get(KSerializer ks, RegionProfile region) => ks.Get(out bool s) && ((Start = s) == s);
}

public sealed class KEGS_START_CHECK_POWER_ACK : IKSerializable
{
    public int Ok { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok);
    public bool Get(KSerializer ks, RegionProfile region) => ks.Get(out int ok) && ((Ok = ok) == ok);
}

public sealed class KEGS_SUPPORT_MATERIAL_EVENT_TIME_ACK : IKSerializable
{
    public bool IsEventTime { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(IsEventTime);
    public bool Get(KSerializer ks, RegionProfile region) => ks.Get(out bool v) && ((IsEventTime = v) == v);
}

public sealed class KEGS_TALK_WITH_NPC_REQ : IKSerializable
{
    public int NpcId { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(NpcId);
    public bool Get(KSerializer ks, RegionProfile region) => ks.Get(out int nid) && ((NpcId = nid) == nid);
}

public sealed class KEGS_TALK_WITH_NPC_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int QuestId { get; set; }
    public Dictionary<int, int> InsertedItem { get; set; } = new();
    public KItemQuantityUpdate ItemQuantityUpdate { get; set; } = new();
    public List<KInventoryItemInfo> UpdatedInventorySlot { get; set; } = new();
    public Dictionary<int, KItemInfo> ItemInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(QuestId) &&
        ks.PutMap(InsertedItem, static (s, k) => s.Put(k), static (s, v) => s.Put(v)) &&
        ks.Put(ItemQuantityUpdate) &&
        ks.PutList(UpdatedInventorySlot) &&
        ks.PutMap(ItemInfo, static (s, k) => s.Put(k), (s, v) => s.Put(v));

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out int qid) ||
            !ks.GetMap(InsertedItem, static s => (s.Get(out int k), k), static s => (s.Get(out int v), v)) ||
            !ks.Get(ItemQuantityUpdate) ||
            !ks.GetList(UpdatedInventorySlot) ||
            !ks.GetMap(ItemInfo, static s => (s.Get(out int k), k), static s => { var v = new KItemInfo(); return (s.Get(v), v); }))
            return false;
        Ok = ok; QuestId = qid;
        return true;
    }
}

public sealed class KEGS_TUTORIAL_STUDENT_LIST_REQ : IKSerializable
{
    public int ViewPage { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(ViewPage);
    public bool Get(KSerializer ks, RegionProfile region) => ks.Get(out int vp) && ((ViewPage = vp) == vp);
}

public sealed class KEGS_TUTORIAL_STUDENT_LIST_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int TotalPage { get; set; }
    public int ViewPage { get; set; }
    public List<KTutorialUnitInfo> TutorialStudentList { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(TotalPage) && ks.Put(ViewPage) && ks.PutList(TutorialStudentList);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out int tp) || !ks.Get(out int vp) || !ks.GetList(TutorialStudentList))
            return false;
        Ok = ok; TotalPage = tp; ViewPage = vp;
        return true;
    }
}

public sealed class KEGS_UDP_CHECK_KICK_USER_NOT : IKSerializable
{
    public long MyUnitUid { get; set; }
    public long AccusationUnitUid { get; set; }
    public bool NowKickOut { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(MyUnitUid) && ks.Put(AccusationUnitUid) && ks.Put(NowKickOut);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long m) || !ks.Get(out long a) || !ks.Get(out bool k)) return false;
        MyUnitUid = m; AccusationUnitUid = a; NowKickOut = k;
        return true;
    }
}

public sealed class KEGS_UNLIMITED_SECOND_CHANGE_JOB_NOT : IKSerializable
{
    public int UnlimitedSecondChangeJob { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(UnlimitedSecondChangeJob);
    public bool Get(KSerializer ks, RegionProfile region) =>
        ks.Get(out int v) && ((UnlimitedSecondChangeJob = v) == v);
}

public sealed class KEGS_UPDATE_NPC_UNIT_BUFF_INFO_NOT : IKSerializable
{
    public List<long> ToUnitUid { get; set; } = new();
    public List<KNpcUnitBuffInfo> NpcUnitBuff { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.PutList(ToUnitUid, static (s, v) => s.Put(v)) && ks.PutList(NpcUnitBuff);

    public bool Get(KSerializer ks, RegionProfile region) =>
        ks.GetList(ToUnitUid, static s => (s.Get(out long v), v)) && ks.GetList(NpcUnitBuff);
}

public sealed class KEGS_UPDATE_PLAY_STATUS_NOT : IKSerializable
{
    public KGamePlayStatus GamePlayStatus { get; set; } = new();
    public int Frame { get; set; } // SERV_FIELD_FRAME_LOG

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(GamePlayStatus) && ks.Put(Frame);
    public bool Get(KSerializer ks, RegionProfile region) =>
        ks.Get(GamePlayStatus) && ks.Get(out int f) && ((Frame = f) == f);
}

public sealed class KEGS_UPDATE_USER_UNIT_BUFF_INFO_NOT : IKSerializable
{
    public long ToUnitUid { get; set; }
    public List<KBuffFactor> BuffFactor { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(ToUnitUid) && ks.PutList(BuffFactor);
    public bool Get(KSerializer ks, RegionProfile region) =>
        ks.Get(out long u) && ks.GetList(BuffFactor) && ((ToUnitUid = u) == u);
}

public sealed class KEGS_USER_COMMUNITY_SURVEY_NEW_REQ : IKSerializable
{
    public const int TabNone = 0;
    public const int TabUserWatch = 1;
    public const int TabInviteGuild = 2;

    public string UnitNickName { get; set; } = string.Empty;
    public int CommunityTabType { get; set; } = TabNone;
    public int Ok { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.PutWideString(UnitNickName) && ks.Put(CommunityTabType) && ks.Put(Ok);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out var n) || !ks.Get(out int t) || !ks.Get(out int ok)) return false;
        UnitNickName = n; CommunityTabType = t; Ok = ok;
        return true;
    }
}

public sealed class KEGS_USER_COMMUNITY_SURVEY_NEW_ACK : IKSerializable
{
    public long UnitUid { get; set; }
    public int CommunityTabType { get; set; }
    public int Ok { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(UnitUid) && ks.Put(CommunityTabType) && ks.Put(Ok);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long u) || !ks.Get(out int t) || !ks.Get(out int ok)) return false;
        UnitUid = u; CommunityTabType = t; Ok = ok;
        return true;
    }
}

public sealed class KEGS_USE_INTIMACY_UP_ITEM_REQ : IKSerializable
{
    public long ItemUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(ItemUid);
    public bool Get(KSerializer ks, RegionProfile region) => ks.Get(out long id) && ((ItemUid = id) == id);
}

public sealed class KEGS_USE_INTIMACY_UP_ITEM_ACK : IKSerializable
{
    public int Ok { get; set; }
    public List<KInventoryItemInfo> InventorySlotInfo { get; set; } = new();
    public int UpPercent { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.PutList(InventorySlotInfo) && ks.Put(UpPercent);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.GetList(InventorySlotInfo) || !ks.Get(out int up)) return false;
        Ok = ok; UpPercent = up;
        return true;
    }
}

public sealed class KEGS_USE_MEGAPHONE_REQ : IKSerializable
{
    public long ItemUid { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(ItemUid) && ks.PutWideString(Message);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long id) || !ks.GetWideString(out var msg)) return false;
        ItemUid = id; Message = msg;
        return true;
    }
}

public sealed class KEGS_USE_MEGAPHONE_ACK : IKSerializable
{
    public int Ok { get; set; }
    public List<KInventoryItemInfo> InventorySlotInfo { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.PutList(InventorySlotInfo);
    public bool Get(KSerializer ks, RegionProfile region) =>
        ks.Get(out int ok) && ks.GetList(InventorySlotInfo) && ((Ok = ok) == ok);
}

public sealed class KEGS_USE_SKILL_BUFF_IN_PLAY_NOT : IKSerializable
{
    public int SkillId { get; set; }
    public bool On { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(SkillId) && ks.Put(On);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int sid) || !ks.Get(out bool on)) return false;
        SkillId = sid; On = on;
        return true;
    }
}

public sealed class KEGS_USE_SPIRIT_REWARD_REQ : IKSerializable
{
    public int LocationIndex { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(LocationIndex);
    public bool Get(KSerializer ks, RegionProfile region) => ks.Get(out int idx) && ((LocationIndex = idx) == idx);
}

public sealed class KEGS_USE_SPIRIT_REWARD_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int[] ChinaSpirit { get; } = new int[6];
    public int LocationIndex { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        for (int i = 0; i < 6; i++)
            if (!ks.Put(ChinaSpirit[i])) return false;
        return ks.Put(LocationIndex);
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok)) return false;
        for (int i = 0; i < 6; i++)
        {
            if (!ks.Get(out int v)) return false;
            ChinaSpirit[i] = v;
        }
        if (!ks.Get(out int idx)) return false;
        Ok = ok; LocationIndex = idx;
        return true;
    }
}

public sealed class KEGS_WARP_BY_BUTTON_REQ : IKSerializable
{
    public int CurrentVillageWarpIndex { get; set; }
    public bool FreeWarp { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(CurrentVillageWarpIndex) && ks.Put(FreeWarp);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int idx) || !ks.Get(out bool fw)) return false;
        CurrentVillageWarpIndex = idx; FreeWarp = fw;
        return true;
    }
}

public sealed class KEGS_WARP_BY_BUTTON_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int Ed { get; set; }
    public int WarpPointMapId { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.Put(Ed) && ks.Put(WarpPointMapId);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out int ed) || !ks.Get(out int map)) return false;
        Ok = ok; Ed = ed; WarpPointMapId = map;
        return true;
    }
}

public sealed class KEGS_WATCH_UNIT_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(UnitUid);
    public bool Get(KSerializer ks, RegionProfile region) => ks.Get(out long u) && ((UnitUid = u) == u);
}

public sealed class KEGS_WATCH_UNIT_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KConnectionUnitInfo CUnitInfo { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok) && ks.Put(CUnitInfo);
    public bool Get(KSerializer ks, RegionProfile region) =>
        ks.Get(out int ok) && ks.Get(CUnitInfo) && ((Ok = ok) == ok);
}

public sealed class KEGS_PET_EVOLUTION_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public KPetInfo PetInfo { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(UnitUid) && ks.Put(PetInfo);
    public bool Get(KSerializer ks, RegionProfile region) =>
        ks.Get(out long u) && ks.Get(PetInfo) && ((UnitUid = u) == u);
}

public sealed class KEGS_CHANGED_PET_SATIETY_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public long PetUid { get; set; }
    public short CurrentSatiety { get; set; }
    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(UnitUid) && ks.Put(PetUid) && ks.Put(CurrentSatiety);
    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long u) || !ks.Get(out long p) || !ks.Get(out short s)) return false;
        UnitUid = u; PetUid = p; CurrentSatiety = s;
        return true;
    }
}

public sealed class KEGS_GIVE_UP_QUEST_ACK : KQuestAck
{
}

public sealed class KEGS_APPLY_JOIN_GUILD_REQ : IKSerializable
{
    public int GuildUid { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool OldApplyDelete { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(GuildUid) && ks.PutWideString(Message) && ks.Put(OldApplyDelete);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int gid) || !ks.GetWideString(out string msg) || !ks.Get(out bool del))
            return false;
        GuildUid = gid; Message = msg; OldApplyDelete = del;
        return true;
    }
}

public sealed class KEGS_ADMIN_MODIFY_UNIT_LEVEL_REQ : IKSerializable
{
    public string UnitNickName { get; set; } = string.Empty;
    public int Level { get; set; } = 1;
    public int Exp { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.PutWideString(UnitNickName) && ks.Put(Level) && ks.Put(Exp);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string nick) || !ks.Get(out int lvl) || !ks.Get(out int exp))
            return false;
        UnitNickName = nick; Level = lvl; Exp = exp;
        return true;
    }
}

public sealed class KEGS_ADMIN_MODIFY_UNIT_LEVEL_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KUnitInfo UnitInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(UnitInfo);

    public bool Get(KSerializer ks, RegionProfile region) =>
        ks.Get(out int ok) && ks.Get(UnitInfo) && ((Ok = ok) == ok);
}

public sealed class KEGS_ADMIN_CHANGE_ED_REQ : IKSerializable
{
    public string UnitNickName { get; set; } = string.Empty;
    public int Ed { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.PutWideString(UnitNickName) && ks.Put(Ed);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string nick) || !ks.Get(out int ed))
            return false;
        UnitNickName = nick; Ed = ed;
        return true;
    }
}

public sealed class KEGS_ADMIN_CHANGE_SPIRIT_REQ : IKSerializable
{
    public int Spirit { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Spirit);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int sp)) return false;
        Spirit = sp;
        return true;
    }
}

public sealed class KEGS_ADMIN_CHANGE_SPIRIT_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int ChangedSpirit { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(Ok) && ks.Put(ChangedSpirit);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(out int sp)) return false;
        Ok = ok; ChangedSpirit = sp;
        return true;
    }
}

public sealed class KEGS_BUY_PERSONAL_SHOP_ITEM_COMPLETE_NOT : IKSerializable
{
    public List<KInventoryItemInfo> InventorySlotInfo { get; set; } = new();
    public int Ed { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.PutList(InventorySlotInfo) && ks.Put(Ed);

    public bool Get(KSerializer ks, RegionProfile region) =>
        ks.GetList(InventorySlotInfo) && ks.Get(out int ed) && ((Ed = ed) == ed);
}

public sealed class KEGS_USER_UNIT_DIE_COMPLETE_NOT : IKSerializable
{
    public long UnitUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(UnitUid);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid)) return false;
        UnitUid = uid;
        return true;
    }
}

public sealed class KEGS_CREATE_TRADE_NOT : IKSerializable
{
    public long TradeUid { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(TradeUid);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid)) return false;
        TradeUid = uid;
        return true;
    }
}

public sealed class KEGS_BREAK_UP_NOT : IKSerializable
{
    public int Cost { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Cost);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int cost)) return false;
        Cost = cost;
        return true;
    }
}

public sealed class KEGS_AUTO_PARTY_WAIT_USER_COUNT_NOT : IKSerializable
{
    public int AutoPartyWaitUserCount { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(AutoPartyWaitUserCount);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int count)) return false;
        AutoPartyWaitUserCount = count;
        return true;
    }
}



