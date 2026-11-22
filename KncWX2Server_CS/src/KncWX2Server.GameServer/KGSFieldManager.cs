using System.Collections.Concurrent;
using KncWX2Server.Core.Events;
using KncWX2Server.Core.Framework;
using KncWX2Server.Core.Logging;
using KncWX2Server.Core.Packets;
using KncWX2Server.Core.Region;
using KncWX2Server.Core.Scripting;
using KncWX2Server.Core.Serialization;
using MoonSharp.Interpreter;

namespace KncWX2Server.GameServer;

/// <summary>
/// Faithful C# port of GameServer/FieldFSM.{h,cpp}.
/// </summary>
public sealed class KFieldFSM
{
    public const int S_INIT = 1;
    public const int S_WAIT = 2;
    public const int S_CLOSE = 3;

    public const int I_TO_WAIT = 10;
    public const int I_TO_CLOSE = 11;

    public int StateId { get; private set; } = S_INIT;

    public bool TryStateTransition(int input)
    {
        int next = (StateId, input) switch
        {
            (S_INIT, I_TO_WAIT) => S_WAIT,
            (S_WAIT, I_TO_CLOSE) => S_CLOSE,
            (S_INIT, I_TO_CLOSE) => S_CLOSE,
            _ => 0,
        };
        if (next == 0) return false;
        StateId = next;
        return true;
    }
}

/// <summary>
/// Faithful C# port of GameServer/Field.{h,cpp}, FieldUser.{h,cpp}, &amp; FieldUserManager.{h,cpp}:
/// partitions each field map into 3D sectors via KSectorId, maintains per-sector user lists and
/// full field user state (options, party, friends, pets, riding pets, relationship),
/// and dispatches all 26 C++ Field.cpp ERM_*/DMP_* events.
/// </summary>
public sealed class KGSFieldZone
{
    public const int GridMaxX = 16;
    public const int GridMaxY = 16;
    public const int GridMaxZ = 16;
    public const int CommunityPageSize = 10;

    public sealed record SectorMoveDelta(
        List<KFieldUserInfo> EnteredUsers,
        List<long> LeftUserUids,
        List<long> NotifyEnterTargets,
        List<long> NotifyLeaveTargets,
        List<long> NearbyUsers);

    private sealed class FieldMemberState
    {
        public KFieldUserInfo Info { get; set; } = new();
        public KFieldOption Option { get; set; } = new();
        public HashSet<long> PartyUids { get; } = [];
        public HashSet<long> FriendUids { get; } = [];
        public int Ed { get; set; }
    }

    private readonly object _lock = new();
    private readonly Dictionary<long, KFieldUserInfo> _users = new();
    private readonly Dictionary<long, FieldMemberState> _memberStates = new();
    private readonly Dictionary<long, int> _userSectors = new();
    private readonly Dictionary<int, HashSet<long>> _sectorUsers = new();

    public int MapId { get; }
    public long FieldUid { get; set; }
    public KFieldFSM Fsm { get; } = new();

    public KGSFieldZone(int mapId)
    {
        MapId = mapId;
        FieldUid = mapId;
        Fsm.TryStateTransition(KFieldFSM.I_TO_WAIT);
    }

    public int UserCount
    {
        get { lock (_lock) return _users.Count; }
    }

    public List<KFieldUserInfo> Enter(KFieldUserInfo user)
    {
        lock (_lock)
        {
            _users[user.UnitUid] = user;
            if (!_memberStates.TryGetValue(user.UnitUid, out var st))
            {
                st = new FieldMemberState();
                _memberStates[user.UnitUid] = st;
            }
            st.Info = user;
            st.Ed = user.Ed;

            int sector = KSectorId.FromPosition(user.Pos.X, user.Pos.Y, user.Pos.Z, 1500f, GridMaxX, GridMaxY, GridMaxZ);
            _userSectors[user.UnitUid] = sector;
            if (!_sectorUsers.TryGetValue(sector, out var set))
            {
                set = new HashSet<long>();
                _sectorUsers[sector] = set;
            }
            set.Add(user.UnitUid);

            var adj = KSectorId.ExtractAdjacentSector(sector, GridMaxX, GridMaxY, GridMaxZ);
            var visible = new List<KFieldUserInfo>();
            foreach (int s in adj)
            {
                if (_sectorUsers.TryGetValue(s, out var uids))
                {
                    foreach (long uid in uids)
                    {
                        if (_users.TryGetValue(uid, out var info))
                            visible.Add(info);
                    }
                }
            }
            return visible;
        }
    }

    public bool Leave(long unitUid)
    {
        lock (_lock)
        {
            if (!_users.Remove(unitUid)) return false;
            _memberStates.Remove(unitUid);
            if (_userSectors.Remove(unitUid, out int oldSector) && _sectorUsers.TryGetValue(oldSector, out var set))
            {
                set.Remove(unitUid);
                if (set.Count == 0) _sectorUsers.Remove(oldSector);
            }
            return true;
        }
    }

    public bool IsExistUser(long unitUid)
    {
        lock (_lock) return _users.ContainsKey(unitUid);
    }

    public void UpdatePosition(long unitUid, KVector3 pos)
        => _ = UpdatePositionWithSectorDelta(unitUid, pos);

    public SectorMoveDelta UpdatePositionWithSectorDelta(long unitUid, KVector3 pos)
    {
        lock (_lock)
        {
            var enteredUsers = new List<KFieldUserInfo>();
            var leftUids = new List<long>();
            var notifyEnterTargets = new List<long>();
            var notifyLeaveTargets = new List<long>();

            if (!_users.TryGetValue(unitUid, out var u))
                return new(enteredUsers, leftUids, notifyEnterTargets, notifyLeaveTargets, []);

            u.Pos = pos;
            int oldSector = _userSectors.GetValueOrDefault(unitUid, KSectorId.InvalidSector);
            int newSector = KSectorId.FromPosition(pos.X, pos.Y, pos.Z, 1500f, GridMaxX, GridMaxY, GridMaxZ);

            var newAdj = KSectorId.ExtractAdjacentSector(newSector, GridMaxX, GridMaxY, GridMaxZ);
            if (oldSector != newSector)
            {
                if (oldSector != KSectorId.InvalidSector && _sectorUsers.TryGetValue(oldSector, out var oldSet))
                {
                    oldSet.Remove(unitUid);
                    if (oldSet.Count == 0) _sectorUsers.Remove(oldSector);
                }
                if (!_sectorUsers.TryGetValue(newSector, out var newSet))
                {
                    newSet = new HashSet<long>();
                    _sectorUsers[newSector] = newSet;
                }
                newSet.Add(unitUid);
                _userSectors[unitUid] = newSector;

                var oldAdj = KSectorId.ExtractAdjacentSector(oldSector, GridMaxX, GridMaxY, GridMaxZ);
                KSectorId.FindRemainder(oldAdj, newAdj, out var leftSectors, out var enteredSectors);

                foreach (int ls in leftSectors)
                {
                    if (_sectorUsers.TryGetValue(ls, out var uids))
                    {
                        foreach (long other in uids)
                        {
                            if (other != unitUid)
                            {
                                leftUids.Add(other);
                                notifyLeaveTargets.Add(other);
                            }
                        }
                    }
                }

                foreach (int es in enteredSectors)
                {
                    if (_sectorUsers.TryGetValue(es, out var uids))
                    {
                        foreach (long other in uids)
                        {
                            if (other != unitUid && _users.TryGetValue(other, out var info))
                            {
                                enteredUsers.Add(info);
                                notifyEnterTargets.Add(other);
                            }
                        }
                    }
                }
            }

            var nearby = new List<long>();
            foreach (int s in newAdj)
            {
                if (_sectorUsers.TryGetValue(s, out var uids))
                {
                    foreach (long other in uids)
                    {
                        if (other != unitUid) nearby.Add(other);
                    }
                }
            }

            return new SectorMoveDelta(enteredUsers, leftUids, notifyEnterTargets, notifyLeaveTargets, nearby);
        }
    }

    public List<long> GetOtherUnitUids(long exceptUnitUid)
    {
        lock (_lock)
        {
            if (!_userSectors.TryGetValue(exceptUnitUid, out int sector))
                return _users.Keys.Where(id => id != exceptUnitUid).ToList();

            var adj = KSectorId.ExtractAdjacentSector(sector, GridMaxX, GridMaxY, GridMaxZ);
            var list = new List<long>();
            foreach (int s in adj)
            {
                if (_sectorUsers.TryGetValue(s, out var uids))
                {
                    foreach (long id in uids)
                    {
                        if (id != exceptUnitUid) list.Add(id);
                    }
                }
            }
            return list;
        }
    }

    public List<long> GetAllUnitUids()
    {
        lock (_lock) return _users.Keys.ToList();
    }

    public bool SetFieldOption(long unitUid, KFieldOption option)
    {
        lock (_lock)
        {
            if (!_memberStates.TryGetValue(unitUid, out var st)) return false;
            st.Option = option;
            return true;
        }
    }

    public bool SetParty(long unitUid, long partyUid, IEnumerable<long> partyUids)
    {
        lock (_lock)
        {
            if (!_memberStates.TryGetValue(unitUid, out var st)) return false;
            st.Info.PartyUid = partyUid;
            st.PartyUids.Clear();
            foreach (long id in partyUids) st.PartyUids.Add(id);
            return true;
        }
    }

    public bool SetFriend(long unitUid, IEnumerable<long> friendUids)
    {
        lock (_lock)
        {
            if (!_memberStates.TryGetValue(unitUid, out var st)) return false;
            st.FriendUids.Clear();
            foreach (long id in friendUids) st.FriendUids.Add(id);
            return true;
        }
    }

    public bool SetFieldUserInfo(KFieldUserInfo info)
    {
        lock (_lock)
        {
            if (!_users.ContainsKey(info.UnitUid)) return false;
            _users[info.UnitUid] = info;
            if (_memberStates.TryGetValue(info.UnitUid, out var st))
                st.Info = info;
            return true;
        }
    }

    public void RefreshUserEd(IDictionary<long, int> unitEdMap)
    {
        lock (_lock)
        {
            foreach (var (uid, ed) in unitEdMap)
            {
                if (_memberStates.TryGetValue(uid, out var st))
                {
                    st.Ed = ed;
                    st.Info.Ed = ed;
                }
            }
        }
    }

    public bool GetFieldUserList(uint viewPage, out uint totalPage, out List<KCommunityUserInfo> userList, bool monitoringMode = false)
    {
        lock (_lock)
        {
            userList = new List<KCommunityUserInfo>();
            var visible = _memberStates.Values
                .Where(m => !m.Info.Invisible)
                .OrderBy(m => m.Info.UnitUid)
                .ToList();
            if (visible.Count == 0)
            {
                totalPage = 1;
                return viewPage <= 1;
            }

            totalPage = (uint)((visible.Count + CommunityPageSize - 1) / CommunityPageSize);
            if (viewPage == 0) viewPage = 1;
            if (viewPage > totalPage) return false;

            int skip = (int)((viewPage - 1) * CommunityPageSize);
            foreach (var m in visible.Skip(skip).Take(CommunityPageSize))
            {
                userList.Add(new KCommunityUserInfo
                {
                    UnitUid = m.Info.UnitUid,
                    NickName = m.Info.NickName,
                    UnitClass = m.Info.UnitClass,
                    Level = m.Info.Level,
                    State = 1,
                    StateCode = MapId,
                    Ed = monitoringMode ? m.Ed : 0,
                });
            }
            return true;
        }
    }

    /// <summary>
    /// Full port of C++ Field.cpp::ProcessEvent (all 26 ERM_*/DMP_* handlers).
    /// </summary>
    public void ProcessEvent(KEvent ev, KGameServer server)
    {
        long senderUid = ev.FirstSenderUid;

        switch (ev.EventId)
        {
            case EventId.ERM_OPEN_FIELD_NOT:
            {
                if (Fsm.StateId == KFieldFSM.S_INIT)
                    Fsm.TryStateTransition(KFieldFSM.I_TO_WAIT);
                server.Router.Send(PerformerId.PI_GS_FIELD, FieldUid, PerformerId.PI_GS_SERVER, server.Uid,
                    EventId.ERM_UPDATE_FIELD_INFO_NOT, new KERM_UPDATE_FIELD_INFO_NOT
                    {
                        MapId = MapId,
                        FieldCount = 1,
                        UserCount = UserCount,
                    });
                break;
            }

            case EventId.ERM_JOIN_FIELD_REQ:
            case EventId.ERM_JOIN_FIELD_CHANNEL_CHANGE_REQ:
            {
                EventId ackId = ev.EventId == EventId.ERM_JOIN_FIELD_CHANNEL_CHANGE_REQ
                    ? EventId.ERM_JOIN_FIELD_CHANNEL_CHANGE_ACK
                    : EventId.ERM_JOIN_FIELD_ACK;

                var req = ev.GetPayload<KERM_JOIN_FIELD_REQ>(server.Region);
                if (req.FieldUserInfo.UnitUid <= 0 && senderUid > 0)
                    req.FieldUserInfo.UnitUid = senderUid;

                if (req.FieldUid != 0 && req.FieldUid != FieldUid)
                {
                    server.Router.Send(PerformerId.PI_GS_FIELD, FieldUid, PerformerId.PI_GS_USER, senderUid,
                        ackId, new KERM_JOIN_FIELD_ACK { Ok = NetError.ERR_FIELD_00 });
                    break;
                }

                // Zombie cleanup if already present in this field
                if (IsExistUser(senderUid))
                {
                    var zombieOthers = GetOtherUnitUids(senderUid);
                    Leave(senderUid);
                    if (zombieOthers.Count > 0)
                    {
                        RelayFieldEvent(server, zombieOthers, EventId.ETR_CHANGE_USER_LIST_IN_SECTOR_NOT,
                            EventId.EGS_CHANGE_USER_LIST_IN_SECTOR_NOT,
                            new KETR_CHANGE_USER_LIST_IN_SECTOR_NOT { DelUserList = [senderUid] });
                    }
                }

                var visible = Enter(req.FieldUserInfo);
                SetFieldOption(req.FieldUserInfo.UnitUid, req.FieldOption);
                SetParty(req.FieldUserInfo.UnitUid, req.FieldUserInfo.PartyUid, req.PartyUIDList);
                SetFriend(req.FieldUserInfo.UnitUid, req.FriendUIDList);

                server.Router.Send(PerformerId.PI_GS_FIELD, FieldUid, PerformerId.PI_GS_USER, senderUid,
                    ackId, new KERM_JOIN_FIELD_ACK { Ok = NetError.NET_OK, FieldUid = FieldUid });

                var others = visible.Where(u => u.UnitUid != senderUid).ToList();
                if (others.Count > 0)
                {
                    RelayFieldEvent(server, [senderUid], EventId.ETR_CHANGE_USER_LIST_IN_SECTOR_NOT,
                        EventId.EGS_CHANGE_USER_LIST_IN_SECTOR_NOT,
                        new KETR_CHANGE_USER_LIST_IN_SECTOR_NOT { NewUserList = others });
                    RelayFieldEvent(server, others.Select(o => o.UnitUid).ToList(), EventId.ETR_CHANGE_USER_LIST_IN_SECTOR_NOT,
                        EventId.EGS_CHANGE_USER_LIST_IN_SECTOR_NOT,
                        new KETR_CHANGE_USER_LIST_IN_SECTOR_NOT { NewUserList = [req.FieldUserInfo] });
                }
                break;
            }

            case EventId.ERM_LEAVE_FIELD_REQ:
            {
                var req = ev.GetPayload<KERM_LEAVE_FIELD_REQ>(server.Region);
                var others = GetOtherUnitUids(senderUid);
                bool left = Leave(senderUid);
                if (!left)
                {
                    server.Router.Send(PerformerId.PI_GS_FIELD, FieldUid, PerformerId.PI_GS_USER, senderUid,
                        EventId.ERM_LEAVE_FIELD_ACK, new KERM_LEAVE_FIELD_ACK
                        {
                            Ok = NetError.ERR_FIELD_01,
                            Reason = req.Reason,
                        });
                    break;
                }

                if (req.Reason == 0)
                {
                    server.Router.Send(PerformerId.PI_GS_FIELD, FieldUid, PerformerId.PI_GS_USER, senderUid,
                        EventId.ERM_LEAVE_FIELD_ACK, new KERM_LEAVE_FIELD_ACK
                        {
                            Ok = NetError.NET_OK,
                            Reason = req.Reason,
                            MapId = req.MapId,
                        });
                }

                if (others.Count > 0)
                {
                    RelayFieldEvent(server, others, EventId.ETR_CHANGE_USER_LIST_IN_SECTOR_NOT,
                        EventId.EGS_CHANGE_USER_LIST_IN_SECTOR_NOT,
                        new KETR_CHANGE_USER_LIST_IN_SECTOR_NOT { DelUserList = [senderUid] });
                }
                break;
            }

            case EventId.ERM_FIELD_UNIT_SYNC_DATA_NOT:
            {
                var req = ev.GetPayload<KEGS_FIELD_UNIT_SYNC_DATA_NOT>(server.Region);
                var delta = UpdatePositionWithSectorDelta(senderUid, req.Pos);
                if (delta.EnteredUsers.Count > 0 || delta.LeftUserUids.Count > 0)
                {
                    RelayFieldEvent(server, [senderUid], EventId.ETR_CHANGE_USER_LIST_IN_SECTOR_NOT,
                        EventId.EGS_CHANGE_USER_LIST_IN_SECTOR_NOT,
                        new KETR_CHANGE_USER_LIST_IN_SECTOR_NOT
                        {
                            NewUserList = delta.EnteredUsers,
                            DelUserList = delta.LeftUserUids,
                        });
                }
                if (delta.NotifyEnterTargets.Count > 0 && _users.TryGetValue(senderUid, out var selfInfo))
                {
                    RelayFieldEvent(server, delta.NotifyEnterTargets, EventId.ETR_CHANGE_USER_LIST_IN_SECTOR_NOT,
                        EventId.EGS_CHANGE_USER_LIST_IN_SECTOR_NOT,
                        new KETR_CHANGE_USER_LIST_IN_SECTOR_NOT { NewUserList = [selfInfo] });
                }
                if (delta.NotifyLeaveTargets.Count > 0)
                {
                    RelayFieldEvent(server, delta.NotifyLeaveTargets, EventId.ETR_CHANGE_USER_LIST_IN_SECTOR_NOT,
                        EventId.EGS_CHANGE_USER_LIST_IN_SECTOR_NOT,
                        new KETR_CHANGE_USER_LIST_IN_SECTOR_NOT { DelUserList = [senderUid] });
                }
                if (delta.NearbyUsers.Count > 0)
                {
                    RelayFieldEvent(server, delta.NearbyUsers, EventId.ETR_FIELD_UNIT_SYNC_DATA_BROAD_NOT,
                        EventId.EGS_FIELD_UNIT_SYNC_DATA_BROAD_NOT,
                        new KETR_FIELD_UNIT_SYNC_DATA_BROAD_NOT
                        {
                            UnitUid = senderUid,
                            VPos = req.Pos,
                            FieldFrame = req.FieldFrames,
                            IsRight = req.IsRight,
                        });
                }
                break;
            }

            case EventId.ERM_CHANGE_EQUIPPED_ITEM_IN_FIELD_NOT:
            {
                var req = ev.GetPayload<KERM_CHANGE_EQUIPPED_ITEM_IN_FIELD_NOT>(server.Region);
                lock (_lock)
                {
                    if (_users.TryGetValue(req.UnitUid, out var info))
                    {
                        info.TitleId = req.EquippedTitleId;
                        info.EquippedItem = req.InventorySlotInfo.Select(i => new KInventoryItemSimpleInfo
                        {
                            ItemUid = i.ItemUid,
                            ItemId = i.ItemId,
                            SlotCategory = i.SlotCategory,
                            SlotId = i.SlotId,
                            EnchantLevel = i.EnchantLevel,
                        }).ToList();
                    }
                }
                var targets = GetOtherUnitUids(senderUid);
                targets.Add(senderUid);
                RelayFieldEvent(server, targets, EventId.ETR_CHANGE_EQUIPPED_ITEM_IN_FIELD_NOT,
                    EventId.EGS_CHANGE_EQUIPPED_ITEM_IN_FIELD_NOT,
                    new KETR_CHANGE_EQUIPPED_ITEM_IN_FIELD_NOT
                    {
                        UnitUid = req.UnitUid,
                        InventorySlotInfo = req.InventorySlotInfo,
                        EquippedTitleId = req.EquippedTitleId,
                    });
                break;
            }

            case EventId.ERM_OPTION_UPDATE_NOT:
            {
                var req = ev.GetPayload<KERM_OPTION_UPDATE_NOT>(server.Region);
                SetFieldOption(senderUid, req.FieldOption);
                break;
            }

            case EventId.ERM_UPDATE_PARTY_NOT:
            {
                var req = ev.GetPayload<KERM_UPDATE_PARTY_NOT>(server.Region);
                SetParty(senderUid, req.PartyUid, req.PartyUIDList);
                var targets = GetOtherUnitUids(senderUid);
                targets.Add(senderUid);
                RelayFieldEvent(server, targets, EventId.ETR_UPDATE_PARTY_NOT,
                    EventId.EGS_UPDATE_PARTY_UID_NOT,
                    new KETR_UPDATE_PARTY_NOT { UnitUid = senderUid, PartyUid = req.PartyUid });
                break;
            }

            case EventId.ERM_CHAT_REQ:
            {
                var req = ev.GetPayload<KEGS_CHAT_REQ>(server.Region);
                if (req.ChatPacketType != 1) // CPT_TOTAL
                {
                    server.Router.Send(PerformerId.PI_GS_FIELD, FieldUid, PerformerId.PI_GS_USER, senderUid,
                        EventId.ERM_CHAT_ACK, new KERM_CHAT_ACK { Ok = NetError.ERR_ROOM_24, ChatPacketType = req.ChatPacketType });
                    break;
                }

                var ack = new KERM_CHAT_ACK { Ok = NetError.NET_OK, ChatPacketType = req.ChatPacketType };
                if (req.RoomChatType == 0) // RCT_ALL
                {
                    server.Router.Send(PerformerId.PI_GS_FIELD, FieldUid, PerformerId.PI_GS_USER, senderUid,
                        EventId.ERM_CHAT_ACK, ack);
                    var targets = GetOtherUnitUids(senderUid);
                    targets.Add(senderUid);
                    RelayFieldEvent(server, targets, EventId.ETR_CHAT_NOT, EventId.EGS_CHAT_NOT,
                        new KETR_CHAT_NOT
                        {
                            SenderUnitUid = senderUid,
                            RecieverUnitUid = req.ToUnitUid,
                            ChatPacketType = req.ChatPacketType,
                            RoomChatType = req.RoomChatType,
                            Msg = req.Message,
                        });
                }
                else if (req.RoomChatType == 1) // RCT_WHISPER
                {
                    if (IsExistUser(req.ToUnitUid))
                    {
                        server.Router.Send(PerformerId.PI_GS_FIELD, FieldUid, PerformerId.PI_GS_USER, senderUid,
                            EventId.ERM_CHAT_ACK, ack);
                        var whisper = new KEGS_CHAT_NOT
                        {
                            SenderUnitUid = senderUid,
                            ReceiverUnitUid = req.ToUnitUid,
                            ChatPacketType = req.ChatPacketType,
                            RoomChatType = req.RoomChatType,
                            Message = req.Message,
                        };
                        server.SendToUnit(senderUid, EventId.EGS_CHAT_NOT, whisper);
                        if (req.ToUnitUid != senderUid)
                            server.SendToUnit(req.ToUnitUid, EventId.EGS_CHAT_NOT, whisper);
                    }
                    else
                    {
                        ack.Ok = NetError.ERR_ROOM_23;
                        server.Router.Send(PerformerId.PI_GS_FIELD, FieldUid, PerformerId.PI_GS_USER, senderUid,
                            EventId.ERM_CHAT_ACK, ack);
                    }
                }
                else
                {
                    ack.Ok = NetError.ERR_ROOM_24;
                    server.Router.Send(PerformerId.PI_GS_FIELD, FieldUid, PerformerId.PI_GS_USER, senderUid,
                        EventId.ERM_CHAT_ACK, ack);
                }
                break;
            }

            case EventId.ERM_REQUEST_TRADE_NOT:
            {
                var req = ev.GetPayload<KERM_REQUEST_TRADE_NOT>(server.Region);
                if (IsExistUser(req.ReceiverUid))
                {
                    server.Router.Send(PerformerId.PI_GS_FIELD, FieldUid, PerformerId.PI_GS_USER, req.ReceiverUid,
                        EventId.ERM_REQUEST_TRADE_NOT, req);
                }
                break;
            }

            case EventId.ERM_REQUEST_TRADE_REPLY_NOT:
            {
                var req = ev.GetPayload<KERM_REQUEST_TRADE_REPLY_NOT>(server.Region);
                if (IsExistUser(req.ReceiverUid))
                {
                    server.Router.Send(PerformerId.PI_GS_FIELD, FieldUid, PerformerId.PI_GS_USER, req.ReceiverUid,
                        EventId.ERM_REQUEST_TRADE_REPLY_NOT, req);
                }
                break;
            }

            case EventId.DMP_DUMP_EVENT_NOT:
                KLog.Cout($"[KGSFieldZone] MapID={MapId} FieldUid={FieldUid} Users={UserCount}");
                break;

            case EventId.ERM_FIELD_USER_LIST_REQ:
            {
                var req = ev.GetPayload<KEGS_COMMUNITY_USER_LIST_REQ>(server.Region);
                bool ok = GetFieldUserList(req.ViewPage, out uint totalPage, out var userList, req.Mode);
                server.Router.Send(PerformerId.PI_GS_FIELD, FieldUid, PerformerId.PI_GS_USER, senderUid,
                    EventId.ERM_FIELD_USER_LIST_ACK, new KEGS_COMMUNITY_USER_LIST_ACK
                    {
                        Ok = ok ? NetError.NET_OK : NetError.ERR_FIELD_04,
                        TotalPage = totalPage,
                        ViewPage = req.ViewPage,
                        UserList = userList,
                    });
                break;
            }

            case EventId.ERM_FIELD_USER_LIST_MONITORING_MODE_REQ:
            {
                var req = ev.GetPayload<KERM_FIELD_USER_LIST_MONITORING_MODE_REQ>(server.Region);
                RefreshUserEd(req.Unit_ED);
                bool ok = GetFieldUserList(req.ViewPage, out uint totalPage, out var userList, monitoringMode: true);
                server.Router.Send(PerformerId.PI_GS_FIELD, FieldUid, PerformerId.PI_GS_USER, senderUid,
                    EventId.ERM_FIELD_USER_LIST_ACK, new KEGS_COMMUNITY_USER_LIST_ACK
                    {
                        Ok = ok ? NetError.NET_OK : NetError.ERR_FIELD_04,
                        TotalPage = totalPage,
                        ViewPage = req.ViewPage,
                        UserList = userList,
                    });
                break;
            }

            case EventId.ERM_UPDATE_FRIEND_NOT:
            {
                var req = ev.GetPayload<KERM_UPDATE_FRIEND_NOT>(server.Region);
                SetFriend(senderUid, req.FriendUIDList);
                break;
            }

            case EventId.ERM_CHANGE_FIELD_USER_INFO_NOT:
            {
                var req = ev.GetPayload<KERM_CHANGE_FIELD_USER_INFO_NOT>(server.Region);
                SetFieldUserInfo(req.Info);
                var targets = GetOtherUnitUids(senderUid);
                targets.Add(senderUid);
                RelayFieldEvent(server, targets, EventId.ETR_CHANGE_FIELD_USER_INFO_NOT,
                    EventId.EGS_CHANGE_USER_INFO_NOT,
                    new KETR_CHANGE_FIELD_USER_INFO_NOT
                    {
                        UnitUid = senderUid,
                        UnitClass = req.Info.UnitClass,
                    });
                break;
            }

            case EventId.ERM_SUMMON_PET_NOT:
            {
                var req = ev.GetPayload<KERM_SUMMON_PET_NOT>(server.Region);
                lock (_lock)
                {
                    if (_users.TryGetValue(senderUid, out var info))
                    {
                        info.Pets = req.PetInfo.Select(p => new KFieldPetInfo
                        {
                            PetUid = p.PetUid,
                            PetId = p.PetId,
                            PetName = p.PetName,
                            EvolutionStep = p.EvolutionStep,
                            Intimacy = p.Intimacy,
                        }).ToList();
                    }
                }
                var targets = GetOtherUnitUids(senderUid);
                targets.Add(senderUid);
                RelayFieldEvent(server, targets, EventId.ETR_SUMMON_PET_NOT, EventId.EGS_SUMMON_PET_NOT,
                    new KETR_SUMMON_PET_NOT { UnitUid = senderUid, PetInfo = req.PetInfo });
                break;
            }

            case EventId.ERM_PET_ACTION_NOT:
            {
                var req = ev.GetPayload<KERM_PET_ACTION_NOT>(server.Region);
                var targets = GetOtherUnitUids(senderUid);
                targets.Add(senderUid);
                RelayFieldEvent(server, targets, EventId.ETR_PET_ACTION_NOT, EventId.EGS_PET_ACTION_NOT,
                    new KETR_PET_ACTION_NOT { UnitUid = req.UnitUid, ActionType = req.ActionType });
                break;
            }

            case EventId.ERM_PET_EVOLUTION_NOT:
            {
                var req = ev.GetPayload<KERM_PET_EVOLUTION_NOT>(server.Region);
                lock (_lock)
                {
                    if (_users.TryGetValue(senderUid, out var info))
                    {
                        info.Pets = [new KFieldPetInfo
                        {
                            PetUid = req.EvolutionPetInfo.PetUid,
                            PetId = req.EvolutionPetInfo.PetId,
                            PetName = req.EvolutionPetInfo.PetName,
                            EvolutionStep = req.EvolutionPetInfo.EvolutionStep,
                            Intimacy = req.EvolutionPetInfo.Intimacy,
                        }];
                    }
                }
                var targets = GetOtherUnitUids(senderUid);
                targets.Add(senderUid);
                RelayFieldEvent(server, targets, EventId.ETR_PET_EVOLUTION_NOT, EventId.EGS_PET_EVOLUTION_NOT,
                    new KETR_PET_EVOLUTION_NOT { UnitUid = senderUid, EvolutionPetInfo = req.EvolutionPetInfo });
                break;
            }

            case EventId.ERM_TOGGLE_INVISIBLE_NOT:
            {
                var req = ev.GetPayload<KERM_TOGGLE_INVISIBLE_NOT>(server.Region);
                SetFieldUserInfo(req.FieldUserInfo);
                var targets = GetOtherUnitUids(senderUid);
                targets.Add(senderUid);
                RelayFieldEvent(server, targets, EventId.ETR_TOGGLE_INVISIBLE_NOT, EventId.EGS_TOGGLE_INVISIBLE_NOT,
                    new KETR_TOGGLE_INVISIBLE_NOT { UnitUid = req.UnitUid, FieldUserInfo = req.FieldUserInfo });
                break;
            }

            case EventId.ERM_SUMMON_RIDING_PET_NOT:
            {
                var req = ev.GetPayload<KEGS_SUMMON_RIDING_PET_NOT>(server.Region);
                lock (_lock)
                {
                    if (_users.TryGetValue(req.UnitUid, out var info))
                    {
                        info.RidingPetUid = req.RidingPetUid;
                        info.RidingPetId = req.RidingPetId;
                    }
                }
                var targets = GetOtherUnitUids(senderUid);
                targets.Add(senderUid);
                RelayFieldEvent(server, targets, EventId.ETR_SUMMON_RIDING_PET_NOT, EventId.EGS_SUMMON_RIDING_PET_NOT,
                    new KETR_SUMMON_RIDING_PET_NOT
                    {
                        UnitUid = req.UnitUid,
                        RidingPetUid = req.RidingPetUid,
                        RidingPetId = req.RidingPetId,
                    });
                break;
            }

            case EventId.ERM_UNSUMMON_RIDING_PET_NOT:
            {
                var req = ev.GetPayload<KEGS_UNSUMMON_RIDING_PET_NOT>(server.Region);
                lock (_lock)
                {
                    if (_users.TryGetValue(req.UnitUid, out var info))
                    {
                        info.RidingPetUid = 0;
                        info.RidingPetId = 0;
                    }
                }
                var targets = GetOtherUnitUids(senderUid);
                targets.Add(senderUid);
                RelayFieldEvent(server, targets, EventId.ETR_UNSUMMON_RIDING_PET_NOT, EventId.EGS_UNSUMMON_RIDING_PET_NOT,
                    new KETR_UNSUMMON_RIDING_PET_NOT { UnitUid = req.UnitUid });
                break;
            }

            case EventId.ERM_COUPLE_MAKING_SUCCESS_NOT:
            {
                var req = ev.GetPayload<KERM_COUPLE_MAKING_SUCCESS_NOT>(server.Region);
                lock (_lock)
                {
                    if (_users.TryGetValue(senderUid, out var info))
                        info.LoverUnitUid = senderUid == req.RequestUnitUid ? req.AcceptUnitUid : req.RequestUnitUid;
                }
                var targets = GetOtherUnitUids(senderUid);
                targets.Add(senderUid);
                RelayFieldEvent(server, targets, EventId.ETR_COUPLE_MAKING_SUCCESS_NOT, EventId.EGS_COUPLE_MAKING_SUCCESS_NOT,
                    new KETR_COUPLE_MAKING_SUCCESS_NOT
                    {
                        RequestUnitName = req.RequestUnitName,
                        AcceptUnitName = req.AcceptUnitName,
                    });
                break;
            }

            case EventId.ERM_BREAK_UP_SUCCESS_NOT:
            {
                lock (_lock)
                {
                    if (_users.TryGetValue(senderUid, out var info))
                        info.LoverUnitUid = 0;
                }
                break;
            }

            case EventId.ERM_RELATIONSHIP_EFFECT_NOT:
            {
                var req = ev.GetPayload<KEGS_RELATIONSHIP_EFFECT_NOT>(server.Region);
                var targets = GetOtherUnitUids(senderUid);
                targets.Add(senderUid);
                RelayFieldEvent(server, targets, EventId.ETR_RELATIONSHIP_EFFECT_NOT, EventId.EGS_RELATIONSHIP_EFFECT_NOT,
                    new KETR_RELATIONSHIP_EFFECT_NOT
                    {
                        ManUid = req.ManUid,
                        GirlUid = req.GirlUid,
                        LoveWord = req.LoveWord,
                        EffectIndex = req.EffectIndex,
                    });
                break;
            }

            case EventId.ERM_CALL_MY_LOVER_LEAVE_FIELD_REQ:
            {
                var req = ev.GetPayload<KERM_CALL_MY_LOVER_LEAVE_FIELD_REQ>(server.Region);
                var others = GetOtherUnitUids(senderUid);
                bool left = Leave(senderUid);
                if (!left)
                {
                    server.Router.Send(PerformerId.PI_GS_FIELD, FieldUid, PerformerId.PI_GS_USER, senderUid,
                        EventId.ERM_CALL_MY_LOVER_LEAVE_FIELD_ACK, new KERM_CALL_MY_LOVER_LEAVE_FIELD_ACK
                        {
                            Ok = NetError.ERR_FIELD_01,
                            Reason = req.Reason,
                            MapId = req.MapId,
                            VPos = req.VPos,
                            LastTouchLineIndex = req.LastTouchLineIndex,
                            LimitLevel = req.LimitLevel,
                            RequireDungeonId = req.RequireDungeonId,
                        });
                    break;
                }

                if (req.Reason == 0)
                {
                    server.Router.Send(PerformerId.PI_GS_FIELD, FieldUid, PerformerId.PI_GS_USER, senderUid,
                        EventId.ERM_CALL_MY_LOVER_LEAVE_FIELD_ACK, new KERM_CALL_MY_LOVER_LEAVE_FIELD_ACK
                        {
                            Ok = NetError.NET_OK,
                            Reason = req.Reason,
                            MapId = req.MapId,
                            VPos = req.VPos,
                            LastTouchLineIndex = req.LastTouchLineIndex,
                            LimitLevel = req.LimitLevel,
                            RequireDungeonId = req.RequireDungeonId,
                        });
                }

                if (others.Count > 0)
                {
                    RelayFieldEvent(server, others, EventId.ETR_CHANGE_USER_LIST_IN_SECTOR_NOT,
                        EventId.EGS_CHANGE_USER_LIST_IN_SECTOR_NOT,
                        new KETR_CHANGE_USER_LIST_IN_SECTOR_NOT { DelUserList = [senderUid] });
                }
                break;
            }
        }
    }

    private static void RelayFieldEvent(
        KGameServer server,
        IReadOnlyList<long> targetUnitUids,
        EventId trEventId,
        EventId gsEventId,
        IKSerializable trPayload)
    {
        if (targetUnitUids.Count == 0) return;
        if (!server.TrServer.TryRelayToTargets(targetUnitUids, trEventId, trPayload))
        {
            foreach (long uid in targetUnitUids)
                server.SendToUnit(uid, gsEventId, trPayload);
        }
    }
}

/// <summary>
/// Manages village/field instances, 3D sector user lists, and sector-delta position broadcasts.
/// </summary>
public sealed class KGSFieldManager
{
    public sealed record FieldTemplate(int MapId, int NumX, int NumY, int NumZ, float SizeX, float SizeY, float SizeZ, float MinX, float MinY, float MinZ, float MaxX, float MaxY, float MaxZ);
    private readonly ConcurrentDictionary<int, KGSFieldZone> _zones = new();
    public Dictionary<int, FieldTemplate> FieldTemplates { get; } = new();

    public int ActiveZoneCount => _zones.Count;

    public KGSFieldZone GetOrCreateZone(int mapId)
        => _zones.GetOrAdd(mapId, static id => new KGSFieldZone(id));

    public KGSFieldZone? GetZone(int mapId)
        => _zones.TryGetValue(mapId, out var z) ? z : null;

    public KGSFieldZone? GetZoneByFieldUid(long fieldUid)
        => _zones.Values.FirstOrDefault(z => z.FieldUid == fieldUid) ?? GetZone((int)fieldUid);

    public int TotalFieldUsers => _zones.Values.Sum(z => z.UserCount);

    public void BindToLua(KScriptManager script, string globalName = "FieldManager")
    {
        Table? proxy = null;
        proxy = script.CreateLuaProxy(globalName, t =>
        {
            t["AddFieldInfo"] = DynValue.NewCallback((_, a) =>
            {
                var tbl = KScriptManager.ArgTable(a, KScriptManager.ArgStart(a, proxy!));
                if (tbl != null)
                {
                    int id = (int)(tbl.Get("ID").CastToNumber() ?? 0);
                    var tile = tbl.Get("TILE_INFO").Table;
                    var map = tbl.Get("MAP_INFO").Table;
                    if (id > 0)
                    {
                        FieldTemplates[id] = new FieldTemplate(
                            id,
                            (int)(tile?.Get("NUM_X").CastToNumber() ?? 1),
                            (int)(tile?.Get("NUM_Y").CastToNumber() ?? 1),
                            (int)(tile?.Get("NUM_Z").CastToNumber() ?? 1),
                            (float)(tile?.Get("SIZE_X").CastToNumber() ?? 1500),
                            (float)(tile?.Get("SIZE_Y").CastToNumber() ?? 800),
                            (float)(tile?.Get("SIZE_Z").CastToNumber() ?? 1500),
                            (float)(map?.Get("MIN_X").CastToNumber() ?? 0),
                            (float)(map?.Get("MIN_Y").CastToNumber() ?? 0),
                            (float)(map?.Get("MIN_Z").CastToNumber() ?? 0),
                            (float)(map?.Get("MAX_X").CastToNumber() ?? 0),
                            (float)(map?.Get("MAX_Y").CastToNumber() ?? 0),
                            (float)(map?.Get("MAX_Z").CastToNumber() ?? 0));
                    }
                }
                return DynValue.Nil;
            });
        });
    }
}
