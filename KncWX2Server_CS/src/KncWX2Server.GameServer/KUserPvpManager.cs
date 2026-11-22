namespace KncWX2Server.GameServer;

/// <summary>
/// Faithful C# port of GameServer/UserPvpManager.{h,cpp} and XSLUnit::ComputeNewPVPRank (SERV_2012_PVP_SEASON2):
/// maintains dirty-delta PvP Rating/MaxRating/RPoint/APoint/KFactor/Win/Lose counters,
/// hysteresis rank-up/rank-down bounds, rank-down RPoint compensation, and rollback recovery.
/// </summary>
public sealed class KUserPvpManager
{
    public const int ApMin = 0;
    public const float KFactorMin = 0f;
    public const float KFactorMax = 500f;
    public const int RatingMin = 0;
    public const int RatingMax = 3300; // CXSLUnit::PE_END

    public enum PvpRank
    {
        None = 0,
        Arrange = 1,
        E = 2,
        D = 3,
        C = 4,
        B = 5,
        A = 6,
        S = 7,
        Ss = 8,
        Sss = 9,
    }

    private struct CachedInt
    {
        public int Init;
        public int Current;
        public void Set(int v) { Init = v; Current = v; }
        public int FlushDelta() { int d = Current - Init; Init = Current; return d; }
        public void Rollback(int committedDelta) { Init -= committedDelta; }
    }

    private struct CachedFloat
    {
        public float Init;
        public float Current;
        public void Set(float v) { Init = v; Current = v; }
        public float FlushDelta() { float d = Current - Init; Init = Current; return d; }
        public void Rollback(float committedDelta) { Init -= committedDelta; }
    }

    private CachedInt _officialMatchCnt;
    private CachedInt _rating;
    private CachedInt _maxRating;
    private CachedInt _rPoint;
    private CachedInt _aPoint;
    private CachedFloat _kFactor;
    private CachedInt _win;
    private CachedInt _lose;

    public long MatchUid { get; set; }
    public bool IsWinBeforeMatch { get; private set; }
    public PvpRank Rank { get; private set; } = PvpRank.None;
    public bool IsRedistributionUser { get; private set; }
    public int PastSeasonWin { get; private set; }
    public int MaxRankingPoint { get; set; } = 1_000_000;
    public int MaxArenaPoint { get; set; } = 1_000_000;

    public int OfficialMatchCount => _officialMatchCnt.Current;
    public int Rating => _rating.Current;
    public int MaxRating => _maxRating.Current;
    public int RPoint => _rPoint.Current;
    public int APoint => _aPoint.Current;
    public float KFactor => _kFactor.Current;
    public int Win => _win.Current;
    public int Lose => _lose.Current;

    public void Clear()
    {
        MatchUid = 0;
        _officialMatchCnt.Set(0);
        _rating.Set(0);
        _maxRating.Set(0);
        _rPoint.Set(0);
        _aPoint.Set(0);
        _kFactor.Set(0f);
        IsRedistributionUser = false;
        PastSeasonWin = 0;
        IsWinBeforeMatch = false;
        _win.Set(0);
        _lose.Set(0);
        Rank = PvpRank.None;
    }

    public void Init(
        int officialMatchCnt,
        int rating,
        int maxRating,
        int rPoint,
        int aPoint,
        bool isWinBeforeMatch,
        int win,
        int lose,
        PvpRank rank,
        float kFactor,
        bool isRedistributionUser,
        int pastSeasonWin)
    {
        _officialMatchCnt.Set(officialMatchCnt);
        _rating.Set(Math.Clamp(rating, RatingMin, RatingMax));
        _maxRating.Set(maxRating);
        _rPoint.Set(rPoint);
        _aPoint.Set(Math.Max(ApMin, aPoint));
        IsWinBeforeMatch = isWinBeforeMatch;
        _win.Set(win);
        _lose.Set(lose);
        Rank = rank;
        _kFactor.Set(Math.Clamp(kFactor, KFactorMin, KFactorMax));
        IsRedistributionUser = isRedistributionUser;
        PastSeasonWin = pastSeasonWin;
    }

    public void IncreaseOfficialMatchCount() => _officialMatchCnt.Current++;

    public static PvpRank ComputeNewPvpRank(PvpRank currentRank, int nowRating)
    {
        int cur = (int)currentRank;
        while (true)
        {
            int lower, upper;
            switch ((PvpRank)cur)
            {
                case PvpRank.E: lower = 0; upper = 250; break;
                case PvpRank.D: lower = 150; upper = 550; break;
                case PvpRank.C: lower = 450; upper = 950; break;
                case PvpRank.B: lower = 850; upper = 1350; break;
                case PvpRank.A: lower = 1250; upper = 1850; break;
                case PvpRank.S: lower = 1750; upper = 2350; break;
                case PvpRank.Ss: lower = 2250; upper = 2950; break;
                case PvpRank.Sss:
                    if (nowRating <= RatingMax)
                    {
                        lower = 2850;
                        upper = RatingMax + 1;
                        break;
                    }
                    return PvpRank.None;
                default:
                    return PvpRank.None;
            }

            if (nowRating < lower) cur--;
            else if (upper <= nowRating) cur++;
            else return (PvpRank)cur;
        }
    }

    public bool AddRatingAndRankUpDownProcess(int deltaRating)
    {
        int add = deltaRating;
        if (_rating.Current + add > RatingMax)
            add = RatingMax - _rating.Current;
        if (_rating.Current + add < 0)
            add = -_rating.Current;

        _rating.Current += add;
        if (_maxRating.Current < _rating.Current)
            _maxRating.Current = _rating.Current;

        if (add == 0) return false;

        var newRank = ComputeNewPvpRank(Rank, _rating.Current);
        if (newRank == PvpRank.None) return false;

        if (Rank < newRank)
        {
            _rPoint.Current = 0;
        }
        else if (newRank < Rank)
        {
            _rPoint.Current = newRank switch
            {
                PvpRank.E => 67,
                PvpRank.D => 111,
                PvpRank.C => 156,
                PvpRank.B => 156,
                PvpRank.A => 200,
                PvpRank.S => 200,
                PvpRank.Ss => 289,
                _ => 0,
            };
        }

        if (Rank != newRank)
        {
            Rank = newRank;
            return true;
        }
        return false;
    }

    public void AddRPoint(int delta)
    {
        int add = delta;
        if (_rPoint.Current + add > MaxRankingPoint) add = MaxRankingPoint - _rPoint.Current;
        if (_rPoint.Current + add < 0) add = -_rPoint.Current;
        _rPoint.Current += add;
    }

    public void AddAPoint(int delta)
    {
        int add = delta;
        if (_aPoint.Current + add > MaxArenaPoint) add = MaxArenaPoint - _aPoint.Current;
        if (_aPoint.Current + add < 0) add = -_aPoint.Current;
        _aPoint.Current += add;
    }

    public void AddKFactor(float delta)
    {
        float next = Math.Clamp(_kFactor.Current + delta, KFactorMin, KFactorMax);
        _kFactor.Current = next;
    }

    public bool UpdateWinLose(int win, int lose, bool isOfficialMatch)
    {
        if (win > 1 || lose > 1 || win < 0 || lose < 0) return false;
        _win.Current += win;
        _lose.Current += lose;
        if (isOfficialMatch)
        {
            if (win == 1) IsWinBeforeMatch = true;
            else if (lose == 1) IsWinBeforeMatch = false;
        }
        return true;
    }

    public (int dMatchCnt, int dRating, int dMaxRating, int dRPoint, int dAPoint, int dWin, int dLose, float dKFactor) FlushDbDeltas()
        => (_officialMatchCnt.FlushDelta(), _rating.FlushDelta(), _maxRating.FlushDelta(),
            _rPoint.FlushDelta(), _aPoint.FlushDelta(), _win.FlushDelta(), _lose.FlushDelta(), _kFactor.FlushDelta());

    public void RollbackDbDeltas(int dMatchCnt, int dRating, int dMaxRating, int dRPoint, int dAPoint, int dWin, int dLose, float dKFactor)
    {
        _officialMatchCnt.Rollback(dMatchCnt);
        _rating.Rollback(dRating);
        _maxRating.Rollback(dMaxRating);
        _rPoint.Rollback(dRPoint);
        _aPoint.Rollback(dAPoint);
        _win.Rollback(dWin);
        _lose.Rollback(dLose);
        _kFactor.Rollback(dKFactor);
    }
}
