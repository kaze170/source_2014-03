using System.Globalization;
using KncWX2Server.Core.Logging;

namespace KncWX2Server.GlobalServer;

/// <summary>
/// Faithful C# port of KJackPotEventManager / KJackPotEvent (Common/Temp.h, Common/Temp.cpp,
/// SERV_EVENT_JACKPOT in GlobalServer.cpp / GlobalUser.cpp).
/// </summary>
public sealed class KJackPotEventManager
{
    public enum CouponState
    {
        Unused = 0,
        Used = 1,
    }

    public sealed class KJackPotEventInitInfo
    {
        public string CouponNumber { get; set; } = string.Empty;
        public string BeginDate { get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;
        public long UserUid { get; set; }
        public long UnitUid { get; set; }
        public int State { get; set; }
    }

    public sealed class KJackPotEventWinInfo
    {
        public string CouponNumber { get; set; } = string.Empty;
        public long UserUid { get; set; }
        public long UnitUid { get; set; }
        public int State { get; set; }
    }

    public sealed class KJackPotEndProcessInfo
    {
        public string BeginDate { get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;
        public int ValidCouponCount { get; set; }
    }

    public sealed class KJackPotCouponState
    {
        public string CouponNumber { get; set; } = string.Empty;
        public long UserUid { get; set; }
        public long UnitUid { get; set; }
        public int State { get; set; }
    }

    public sealed class KJackPotEvent
    {
        private readonly List<KJackPotCouponState> _coupons = new();
        private readonly string _beginDateStr;
        private readonly string _endDateStr;
        private readonly DateTime _beginDate;
        private readonly DateTime _endDate;
        private int _validCouponCount;
        private long _beginProcessGapSeconds;
        private DateTime _lastWinTime;
        private bool _eventStarted;

        public KJackPotEvent(string beginDateStr, string endDateStr, DateTime beginDate, DateTime endDate)
        {
            _beginDateStr = beginDateStr;
            _endDateStr = endDateStr;
            _beginDate = beginDate;
            _endDate = endDate;
        }

        public int ValidCouponCount => _validCouponCount;

        public bool CheckTheBeginEndDate(string beginDate, string endDate)
            => string.Equals(_beginDateStr, beginDate, StringComparison.Ordinal)
            && string.Equals(_endDateStr, endDate, StringComparison.Ordinal);

        public void EndProcess(List<KJackPotEndProcessInfo> outEndInfos)
        {
            if (_validCouponCount > 0)
            {
                outEndInfos.Add(new KJackPotEndProcessInfo
                {
                    BeginDate = _beginDateStr,
                    EndDate = _endDateStr,
                    ValidCouponCount = _validCouponCount,
                });
            }
        }

        public void AddCoupon(string couponNumber, long userUid, long unitUid, int state)
        {
            _coupons.Add(new KJackPotCouponState
            {
                CouponNumber = couponNumber,
                UserUid = userUid,
                UnitUid = unitUid,
                State = state,
            });

            if (state != (int)CouponState.Unused)
                return;

            _validCouponCount++;
            long totalSeconds = Math.Max(0, (long)(_endDate - _beginDate).TotalSeconds);
            _beginProcessGapSeconds = totalSeconds / _validCouponCount;
        }

        public bool IsValidEvent(DateTime now)
        {
            if (_validCouponCount < 1)
                return false;

            if (_endDate <= now)
            {
                int remaining = _coupons.Count(c => c.State == (int)CouponState.Unused);
                KLog.Cout($"Expired jackpot event: {_beginDateStr} ~ {_endDateStr}, remaining={remaining}");
                return false;
            }

            return true;
        }

        public bool ExtractCouponNumber(long userUid, long unitUid, out string couponNumber, out int state)
        {
            foreach (var coupon in _coupons)
            {
                if (coupon.State != (int)CouponState.Unused)
                    continue;

                coupon.UserUid = userUid;
                coupon.UnitUid = unitUid;
                coupon.State = (int)CouponState.Used;
                _validCouponCount--;

                couponNumber = coupon.CouponNumber;
                state = coupon.State;
                return true;
            }

            couponNumber = string.Empty;
            state = 0;
            return false;
        }

        public bool BeginProcess(DateTime now)
        {
            if (!_eventStarted)
            {
                if (now < _beginDate)
                    return false;
                _eventStarted = true;
                _lastWinTime = now;
                return true;
            }

            return (now - _lastWinTime).TotalSeconds >= _beginProcessGapSeconds;
        }

        public void ConfirmProcess(DateTime now) => _lastWinTime = now;

        public void ReInsertCoupon(string couponNumber)
        {
            foreach (var coupon in _coupons)
            {
                if (string.Equals(coupon.CouponNumber, couponNumber, StringComparison.Ordinal) &&
                    coupon.State == (int)CouponState.Used)
                {
                    coupon.UserUid = 0;
                    coupon.UnitUid = 0;
                    coupon.State = (int)CouponState.Unused;
                    _validCouponCount++;
                    return;
                }
            }
        }
    }

    private readonly object _sync = new();
    private readonly List<KJackPotEvent> _events = new();
    private readonly Dictionary<long, long> _connectUserAndUnitUid = new();
    private readonly HashSet<long> _winnerUserUids = new();
    private readonly Dictionary<long, long> _connectUserInfoExceptWinner = new();
    private DateTime _lastTickTime = DateTime.MinValue;

    public int EventCount { get { lock (_sync) return _events.Count; } }
    public int EligibleOnlineCount { get { lock (_sync) return _connectUserInfoExceptWinner.Count; } }

    private static bool TryParseEventDate(string raw, out DateTime parsed)
        => DateTime.TryParse(raw, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out parsed)
        || DateTime.TryParse(raw, out parsed);

    public bool Init(IEnumerable<KJackPotEventInitInfo> initInfos)
    {
        var list = initInfos?.ToList() ?? new List<KJackPotEventInitInfo>();
        if (list.Count == 0)
        {
            KLog.Cerr("JackPotEventManager.Init: empty event list.");
            return false;
        }

        lock (_sync)
        {
            _events.Clear();
            var seenCoupons = new HashSet<string>(StringComparer.Ordinal);
            foreach (var item in list)
            {
                if (!TryParseEventDate(item.BeginDate, out var beginDate) ||
                    !TryParseEventDate(item.EndDate, out var endDate))
                {
                    KLog.Cerr($"JackPotEventManager.Init: invalid date range '{item.BeginDate}' ~ '{item.EndDate}'.");
                    _events.Clear();
                    return false;
                }

                if (!seenCoupons.Add(item.CouponNumber))
                {
                    KLog.Cerr($"JackPotEventManager.Init: duplicate coupon '{item.CouponNumber}'.");
                    _events.Clear();
                    return false;
                }

                if (item.UserUid != 0)
                {
                    if (!_winnerUserUids.Add(item.UserUid))
                        KLog.Cerr($"JackPotEventManager.Init: duplicate winner UserUid={item.UserUid}.");
                }

                var ev = _events.FirstOrDefault(e => e.CheckTheBeginEndDate(item.BeginDate, item.EndDate));
                if (ev == null)
                {
                    ev = new KJackPotEvent(item.BeginDate, item.EndDate, beginDate, endDate);
                    _events.Add(ev);
                }

                ev.AddCoupon(item.CouponNumber, item.UserUid, item.UnitUid, item.State);
            }

            return true;
        }
    }

    public void Tick(
        out List<KJackPotEventWinInfo> winInfos,
        out List<KJackPotEndProcessInfo> endInfos,
        DateTime? nowLocal = null,
        bool bypassOneSecondThrottle = false)
    {
        winInfos = new List<KJackPotEventWinInfo>();
        endInfos = new List<KJackPotEndProcessInfo>();
        var now = nowLocal ?? DateTime.Now;

        lock (_sync)
        {
            if (!bypassOneSecondThrottle && _lastTickTime != DateTime.MinValue && (now - _lastTickTime).TotalSeconds < 1.0)
                return;
            _lastTickTime = now;

            for (int i = _events.Count - 1; i >= 0; i--)
            {
                var ev = _events[i];
                if (!ev.IsValidEvent(now))
                {
                    ev.EndProcess(endInfos);
                    _events.RemoveAt(i);
                }
            }

            foreach (var ev in _events)
            {
                if (!ev.BeginProcess(now))
                    continue;

                if (!LotteryUserNoLock(out long userUid, out long unitUid))
                    continue;

                if (!ev.ExtractCouponNumber(userUid, unitUid, out string coupon, out int state))
                {
                    ReInsertUserNoLock(userUid);
                    continue;
                }

                winInfos.Add(new KJackPotEventWinInfo
                {
                    UserUid = userUid,
                    UnitUid = unitUid,
                    CouponNumber = coupon,
                    State = state,
                });
                ev.ConfirmProcess(now);
            }
        }
    }

    public void AddUser(long userUid, long unitUid)
    {
        if (userUid <= 0 || unitUid <= 0) return;
        lock (_sync)
        {
            _connectUserAndUnitUid[userUid] = unitUid;
            if (_winnerUserUids.Contains(userUid))
                return;
            _connectUserInfoExceptWinner[userUid] = unitUid;
        }
    }

    public void RemoveUser(long userUid, long unitUid)
    {
        if (userUid <= 0) return;
        lock (_sync)
        {
            _connectUserAndUnitUid.Remove(userUid);
            _connectUserInfoExceptWinner.Remove(userUid);
        }
    }

    public bool LotteryUser(out long userUid, out long unitUid)
    {
        lock (_sync)
            return LotteryUserNoLock(out userUid, out unitUid);
    }

    public void ReInsertUser(long userUid)
    {
        lock (_sync)
            ReInsertUserNoLock(userUid);
    }

    public void ReInsert(KJackPotEventWinInfo winInfo)
    {
        if (winInfo == null) return;
        lock (_sync)
        {
            foreach (var ev in _events)
                ev.ReInsertCoupon(winInfo.CouponNumber);
            _winnerUserUids.Remove(winInfo.UserUid);
            ReInsertUserNoLock(winInfo.UserUid);
        }
    }

    public void FaildProcess(long userUid, long unitUid, string couponNumber)
    {
        lock (_sync)
        {
            foreach (var ev in _events)
                ev.ReInsertCoupon(couponNumber);
            _winnerUserUids.Remove(userUid);
            ReInsertUserNoLock(userUid);
        }
    }

    private bool LotteryUserNoLock(out long userUid, out long unitUid)
    {
        if (_connectUserInfoExceptWinner.Count == 0)
        {
            userUid = 0;
            unitUid = 0;
            return false;
        }

        int idx = Random.Shared.Next(_connectUserInfoExceptWinner.Count);
        var chosen = _connectUserInfoExceptWinner.ElementAt(idx);
        userUid = chosen.Key;
        unitUid = chosen.Value;

        _winnerUserUids.Add(userUid);
        _connectUserInfoExceptWinner.Remove(userUid);
        return true;
    }

    private void ReInsertUserNoLock(long userUid)
    {
        if (_connectUserAndUnitUid.TryGetValue(userUid, out long unitUid))
            _connectUserInfoExceptWinner[userUid] = unitUid;
    }
}
