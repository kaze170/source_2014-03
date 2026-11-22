using KncWX2Server.Core.Packets;
using KncWX2Server.Core.Region;

namespace KncWX2Server.CenterServer;

/// <summary>
/// Faithful C# port of CenterServer/WeddingHall.{h,cpp} (SERV_RELATIONSHIP_SYSTEM):
/// manages wedding hall room lifecycle (Init -> Wait -> Play -> Close),
/// groom/bride mutual confirmation handshake, wedding completion flags,
/// and 10-second empty-hall cleanup timer.
/// </summary>
public sealed class KCnWeddingHall
{
    public const double WaitEmptyTimeoutSeconds = 10.0;
    public const int MaxWeddingHallSlot = 30;

    public enum HallState
    {
        Init = 0,
        Wait = 1,
        Play = 2,
        Close = 3,
    }

    private readonly Dictionary<long, KRoomSlotInfo> _slots = new();
    private DateTime _waitStartedUtc;

    public long RoomUid { get; init; }
    public KWeddingHallInfo HallInfo { get; private set; } = new();
    public HallState State { get; private set; } = HallState.Init;

    public int MemberCount => _slots.Count;
    public bool IsEmpty => _slots.Count == 0;
    public bool IsGroom(long unitUid) => HallInfo.Groom == unitUid;
    public bool IsBride(long unitUid) => HallInfo.Bride == unitUid;
    public bool IsGroomOrBride(long unitUid) => IsGroom(unitUid) || IsBride(unitUid);

    public (int ok, KWeddingHallInfo info) OpenHall(KWeddingHallInfo info, KRoomSlotInfo openerSlot, DateTime? nowUtc = null)
    {
        if (State != HallState.Init)
            return (NetError.ERR_ROOM_09, HallInfo);

        info.RoomUid = RoomUid;
        HallInfo = info;
        _slots.Clear();
        _slots[openerSlot.RoomUserInfo.UnitUid] = openerSlot;
        State = HallState.Play;
        _waitStartedUtc = nowUtc ?? DateTime.UtcNow;
        return (NetError.NET_OK, HallInfo);
    }

    public int JoinHall(KRoomSlotInfo guestSlot)
    {
        if (State != HallState.Wait && State != HallState.Play)
            return NetError.ERR_ROOM_09;
        if (_slots.Count >= MaxWeddingHallSlot)
            return NetError.ERR_ROOM_09;

        _slots[guestSlot.RoomUserInfo.UnitUid] = guestSlot;
        State = HallState.Play;
        return NetError.NET_OK;
    }

    public int LeaveHall(long unitUid, DateTime? nowUtc = null)
    {
        if (!_slots.Remove(unitUid))
            return NetError.ERR_SLOT_04;

        if (_slots.Count == 0)
        {
            State = HallState.Wait;
            _waitStartedUtc = nowUtc ?? DateTime.UtcNow;
        }
        return NetError.NET_OK;
    }

    public (int ok, long partnerUnitUid) CheckStartWedding(long requesterUnitUid)
    {
        if (!IsGroomOrBride(requesterUnitUid))
            return (NetError.ERR_RELATIONSHIP_41, 0);

        long partnerUid = IsGroom(requesterUnitUid) ? HallInfo.Bride : HallInfo.Groom;
        if (!_slots.ContainsKey(partnerUid))
            return (NetError.ERR_RELATIONSHIP_42, 0);

        return (NetError.NET_OK, partnerUid);
    }

    public bool RespondStartWedding(long responderUnitUid, bool agree)
    {
        if (!IsGroomOrBride(responderUnitUid))
            return false;
        if (!agree)
            return false;

        HallInfo.Success = true;
        return true;
    }

    public bool CheckWeddingHallInfo(KWeddingHallInfo candidate)
        => HallInfo.WeddingUid == candidate.WeddingUid && !HallInfo.Success;

    public bool Tick(DateTime? nowUtc = null)
    {
        if (State != HallState.Wait) return false;
        var now = nowUtc ?? DateTime.UtcNow;
        if ((now - _waitStartedUtc).TotalSeconds > WaitEmptyTimeoutSeconds)
        {
            State = HallState.Close;
            return true;
        }
        return false;
    }

    public List<KRoomSlotInfo> SnapshotSlots() => _slots.Values.ToList();
}
