using System.Collections.Concurrent;

namespace KncWX2Server.GameServer;

/// <summary>
/// Faithful C# port of GameServer/GSUserTrade.cpp:
/// two-party player-to-player trade session state machine (Invite -> Active -> Confirm -> Atomic Commit).
/// </summary>
public sealed class KTradeManager
{
    public enum TradeState
    {
        PendingInvite = 0,
        Active = 1,
        Completed = 2,
        Cancelled = 3,
    }

    public sealed class TradeOffer
    {
        public long UnitUid { get; init; }
        public string NickName { get; init; } = string.Empty;
        public int OfferEd { get; set; }
        public Dictionary<long, int> OfferItems { get; } = new(); // ItemUID -> Qty
        public bool Registered { get; set; }
        public bool Confirmed { get; set; }
    }

    public sealed class TradeSession
    {
        public long TradeUid { get; init; }
        public TradeState State { get; set; } = TradeState.PendingInvite;
        public TradeOffer Sender { get; init; } = new();
        public TradeOffer Receiver { get; init; } = new();
        public object SyncRoot { get; } = new();

        public TradeOffer? GetOffer(long unitUid)
            => Sender.UnitUid == unitUid ? Sender : Receiver.UnitUid == unitUid ? Receiver : null;

        public TradeOffer? GetPartnerOffer(long unitUid)
            => Sender.UnitUid == unitUid ? Receiver : Receiver.UnitUid == unitUid ? Sender : null;
    }

    private long _nextTradeUid = 1;
    private readonly ConcurrentDictionary<long, TradeSession> _trades = new();
    private readonly ConcurrentDictionary<long, long> _unitToTrade = new();

    public TradeSession? CreateInvite(long senderUnitUid, string senderNick, long receiverUnitUid, string receiverNick)
    {
        if (senderUnitUid == receiverUnitUid) return null;
        if (_unitToTrade.ContainsKey(senderUnitUid) || _unitToTrade.ContainsKey(receiverUnitUid))
            return null;

        long uid = Interlocked.Increment(ref _nextTradeUid);
        var session = new TradeSession
        {
            TradeUid = uid,
            State = TradeState.PendingInvite,
            Sender = new TradeOffer { UnitUid = senderUnitUid, NickName = senderNick },
            Receiver = new TradeOffer { UnitUid = receiverUnitUid, NickName = receiverNick },
        };
        _trades[uid] = session;
        _unitToTrade[senderUnitUid] = uid;
        _unitToTrade[receiverUnitUid] = uid;
        return session;
    }

    public TradeSession? GetSessionByUnit(long unitUid)
        => _unitToTrade.TryGetValue(unitUid, out long tid) && _trades.TryGetValue(tid, out var s) ? s : null;

    public bool AcceptInvite(long receiverUnitUid, bool accept)
    {
        var session = GetSessionByUnit(receiverUnitUid);
        if (session == null) return false;
        lock (session.SyncRoot)
        {
            if (session.Receiver.UnitUid != receiverUnitUid || session.State != TradeState.PendingInvite)
                return false;
            if (!accept)
            {
                CloseSessionInternal(session, TradeState.Cancelled);
                return false;
            }
            session.State = TradeState.Active;
            return true;
        }
    }

    public bool UpdateItem(long unitUid, long itemUid, int quantity, bool isDelete)
    {
        var session = GetSessionByUnit(unitUid);
        if (session == null) return false;
        lock (session.SyncRoot)
        {
            if (session.State != TradeState.Active) return false;
            var offer = session.GetOffer(unitUid);
            if (offer == null) return false;

            if (isDelete || quantity <= 0)
                offer.OfferItems.Remove(itemUid);
            else
                offer.OfferItems[itemUid] = quantity;

            // Modifying offer resets both players' registration/confirmation flags (anti-scam)
            session.Sender.Registered = false;
            session.Receiver.Registered = false;
            session.Sender.Confirmed = false;
            session.Receiver.Confirmed = false;
            return true;
        }
    }

    public bool UpdateEd(long unitUid, int ed)
    {
        var session = GetSessionByUnit(unitUid);
        if (session == null || ed < 0) return false;
        lock (session.SyncRoot)
        {
            if (session.State != TradeState.Active) return false;
            var offer = session.GetOffer(unitUid);
            if (offer == null) return false;
            offer.OfferEd = ed;
            session.Sender.Registered = false;
            session.Receiver.Registered = false;
            session.Sender.Confirmed = false;
            session.Receiver.Confirmed = false;
            return true;
        }
    }

    public (bool readyToExecute, TradeSession? session) Confirm(long unitUid)
    {
        var session = GetSessionByUnit(unitUid);
        if (session == null) return (false, null);
        lock (session.SyncRoot)
        {
            if (session.State != TradeState.Active) return (false, null);
            var offer = session.GetOffer(unitUid);
            if (offer == null) return (false, null);
            offer.Registered = true;
            offer.Confirmed = true;
            return (session.Sender.Confirmed && session.Receiver.Confirmed, session);
        }
    }

    public void CompleteSession(TradeSession session, bool committed)
    {
        lock (session.SyncRoot)
            CloseSessionInternal(session, committed ? TradeState.Completed : TradeState.Cancelled);
    }

    public void CancelByUnit(long unitUid)
    {
        var session = GetSessionByUnit(unitUid);
        if (session == null) return;
        lock (session.SyncRoot)
            CloseSessionInternal(session, TradeState.Cancelled);
    }

    private void CloseSessionInternal(TradeSession session, TradeState finalState)
    {
        session.State = finalState;
        _unitToTrade.TryRemove(session.Sender.UnitUid, out _);
        _unitToTrade.TryRemove(session.Receiver.UnitUid, out _);
        _trades.TryRemove(session.TradeUid, out _);
    }
}
