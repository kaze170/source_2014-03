using KncWX2Server.Core.Packets;
using KncWX2Server.Core.Region;

namespace KncWX2Server.CenterServer;

/// <summary>
/// Faithful C# port of CenterServer/Trade.{h,cpp} and TradeUserManager.{h,cpp}:
/// Center-side 2-party trade room FSM (Init -> Wait -> Confirm -> DbUpdate -> Close),
/// item slot cap / duplicate checks, MaxED overflow protection, Offer -> Approve -> Confirm handshake,
/// and 5-second opponent join timeout.
/// </summary>
public sealed class KCnTradeRoom
{
    public const int MaxTradeMember = 2;
    public const int MaxTradeItemSlots = 12;
    public const int MaxHoldEd = 2_000_000_000;

    public enum TradeRoomState
    {
        Init = 0,
        Wait = 1,
        Confirm = 2,
        DbUpdate = 3,
        Close = 4,
    }

    public sealed class TradeParticipant
    {
        public long UserUid { get; init; }
        public long UnitUid { get; init; }
        public long GsUid { get; init; }
        public string NickName { get; init; } = string.Empty;
        public long CurrentEd { get; set; }
        public int OfferEd { get; set; }
        public Dictionary<long, KInventoryItemInfo> OfferItems { get; } = new();
        public bool Offered { get; set; }
        public bool Approved { get; set; }
        public bool Confirmed { get; set; }
        public bool DbUpdateReady { get; set; }

        public bool CheckMaxEd(int incomingEd)
            => incomingEd >= 0 && CurrentEd + incomingEd <= MaxHoldEd;
    }

    private readonly Dictionary<long, TradeParticipant> _users = new();
    private DateTime _waitStartUtc;

    public long TradeUid { get; init; }
    public TradeRoomState State { get; private set; } = TradeRoomState.Init;
    public int BreakReason { get; private set; }

    public int MemberCount => _users.Count;
    public bool IsEmpty => _users.Count <= 0;
    public bool IsFull => _users.Count >= MaxTradeMember;

    public (int ok, long tradeUid) OpenTrade(TradeParticipant initiator, DateTime? nowUtc = null)
    {
        if (State != TradeRoomState.Init)
            return (NetError.ERR_ROOM_09, 0);

        _users.Clear();
        _users[initiator.UnitUid] = initiator;
        State = TradeRoomState.Wait;
        _waitStartUtc = nowUtc ?? DateTime.UtcNow;
        return (NetError.NET_OK, TradeUid);
    }

    public int JoinTrade(TradeParticipant joiner)
    {
        if (State != TradeRoomState.Wait)
            return NetError.ERR_ROOM_09;
        if (_users.Count >= MaxTradeMember || _users.ContainsKey(joiner.UnitUid))
        {
            if (_users.Count <= 1) BreakTrade(NetError.NOT_LEAVE_ROOM_REASON_07);
            return NetError.ERR_TRADE_01;
        }
        _users[joiner.UnitUid] = joiner;
        return NetError.NET_OK;
    }

    public int UpdateTradeItem(long unitUid, KInventoryItemInfo item, bool isDelete)
    {
        if (State != TradeRoomState.Wait)
            return NetError.ERR_TRADE_03;
        if (!_users.TryGetValue(unitUid, out var user) || user.Offered)
            return NetError.ERR_TRADE_03;

        if (!isDelete)
        {
            if (user.OfferItems.Count >= MaxTradeItemSlots)
                return NetError.ERR_TRADE_17;
            if (user.OfferItems.ContainsKey(item.ItemUid))
                return NetError.ERR_TRADE_15;
            user.OfferItems[item.ItemUid] = item;
        }
        else
        {
            if (!user.OfferItems.Remove(item.ItemUid))
                return NetError.ERR_TRADE_16;
        }

        ResetHandshakeFlags();
        return NetError.NET_OK;
    }

    public int UpdateTradeEd(long unitUid, int ed)
    {
        if (State != TradeRoomState.Wait)
            return NetError.ERR_TRADE_02;
        if (ed < 0 || !_users.TryGetValue(unitUid, out var user))
            return NetError.ERR_TRADE_02;

        if (user.OfferEd == ed)
            return NetError.NET_OK;

        user.OfferEd = ed;
        ResetHandshakeFlags();
        return NetError.NET_OK;
    }

    public (int ok, bool bothOffered) OfferTrade(long unitUid)
    {
        if (State != TradeRoomState.Wait || !_users.TryGetValue(unitUid, out var user))
            return (NetError.ERR_TRADE_02, false);

        var other = _users.Values.FirstOrDefault(u => u.UnitUid != unitUid);
        if (other == null)
            return (NetError.ERR_TRADE_02, false);

        if (!other.CheckMaxEd(user.OfferEd))
            return (NetError.ERR_TRADE_18, false);

        user.Offered = true;
        bool both = _users.Count == MaxTradeMember && _users.Values.All(u => u.Offered);
        return (NetError.NET_OK, both);
    }

    public (int ok, bool bothApproved) ApproveTrade(long unitUid)
    {
        if (State != TradeRoomState.Wait || !_users.TryGetValue(unitUid, out var user))
            return (NetError.ERR_TRADE_02, false);
        if (!_users.Values.All(u => u.Offered))
            return (NetError.ERR_TRADE_02, false);

        user.Approved = true;
        bool both = _users.Count == MaxTradeMember && _users.Values.All(u => u.Approved);
        if (both)
            State = TradeRoomState.Confirm;
        return (NetError.NET_OK, both);
    }

    public (int ok, bool bothConfirmed) ConfirmTrade(long unitUid)
    {
        if (State != TradeRoomState.Confirm || !_users.TryGetValue(unitUid, out var user))
            return (NetError.ERR_TRADE_02, false);

        user.Confirmed = true;
        bool both = _users.Count == MaxTradeMember && _users.Values.All(u => u.Confirmed);
        if (both)
            State = TradeRoomState.DbUpdate;
        return (NetError.NET_OK, both);
    }

    public bool TickWaitTimeout(DateTime? nowUtc = null)
    {
        if (State != TradeRoomState.Wait || _users.Count > 1) return false;
        var now = nowUtc ?? DateTime.UtcNow;
        if ((now - _waitStartUtc).TotalSeconds >= 5.0)
        {
            BreakTrade(NetError.NOT_LEAVE_ROOM_REASON_08);
            return true;
        }
        return false;
    }

    public int HandleGameServerDisconnect(long gsUid)
    {
        var disconnected = _users.Values.Where(u => u.GsUid == gsUid).Select(u => u.UnitUid).ToList();
        if (disconnected.Count == 0) return 0;
        foreach (long uid in disconnected)
            _users.Remove(uid);
        BreakTrade(NetError.NOT_LEAVE_ROOM_REASON_09);
        return disconnected.Count;
    }

    public void BreakTrade(int reason)
    {
        BreakReason = reason;
        State = TradeRoomState.Close;
    }

    public bool HasParticipant(long unitUid) => _users.ContainsKey(unitUid);

    public bool MarkDbUpdateReady(long unitUid, out bool allReady)
    {
        allReady = false;
        if (!_users.TryGetValue(unitUid, out var user))
            return false;
        user.DbUpdateReady = true;
        allReady = _users.Count == MaxTradeMember && _users.Values.All(u => u.DbUpdateReady);
        if (allReady)
            State = TradeRoomState.Close;
        return true;
    }

    public List<long> GetParticipantUnitUids() => _users.Keys.ToList();

    private void ResetHandshakeFlags()
    {
        foreach (var u in _users.Values)
        {
            u.Offered = false;
            u.Approved = false;
            u.Confirmed = false;
        }
    }
}
