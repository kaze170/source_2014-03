namespace KncWX2Server.GameServer;

/// <summary>
/// Faithful C# port of GameServer/UserLocalRankingInfo.{h,cpp} (SERV_LOCAL_RANKING_SYSTEM):
/// tracks Spirit and AP ranking points with dirty-delta extraction (KCacheData equivalent)
/// and rollback adjustment when DB persistence fails.
/// </summary>
public sealed class KUserLocalRankingInfo
{
    public enum MainTabCategory
    {
        Spirit = 0,
        Ap = 1,
    }

    private struct CachedCounter
    {
        public int InitValue;
        public int CurrentValue;

        public void SetValue(int value)
        {
            InitValue = value;
            CurrentValue = value;
        }

        public void Add(int delta) => CurrentValue += delta;
        public int GetChangeValue() => CurrentValue - InitValue;
        public void AdjustInitValue(int newInit) => InitValue = newInit;
    }

    private CachedCounter _spirit;
    private CachedCounter _ap;

    public int Uid { get; private set; }
    public long UserUid { get; private set; }
    public long UnitUid { get; private set; }
    public int Category { get; private set; }
    public string UpdatedTimestamp { get; private set; } = string.Empty;

    public bool InitCheck(long userUid, long unitUid) => UserUid == userUid && UnitUid == unitUid;
    public bool InitCheckUserInfo(long userUid) => UserUid == userUid;
    public bool IsValidCategory => Category > 0;

    public void InitInfo(int uid, long userUid, long unitUid, int category, int initialSpirit, int initialAp, string updatedTimestamp)
    {
        Uid = uid;
        UserUid = userUid;
        UnitUid = unitUid;
        Category = category;
        UpdatedTimestamp = updatedTimestamp;
        _spirit.SetValue(initialSpirit);
        _ap.SetValue(initialAp);
    }

    public void ResetUnitInfo()
    {
        UnitUid = 0;
        _spirit.SetValue(0);
        _ap.SetValue(0);
    }

    public void IncreasedUnitInfo(int increasedSpirit, int increasedAp)
    {
        _spirit.Add(increasedSpirit);
        _ap.Add(increasedAp);
    }

    public (int spirit, int ap) GetValue() => (_spirit.CurrentValue, _ap.CurrentValue);

    public (int changedSpirit, int changedAp) GetChangedValue()
    {
        int ds = _spirit.GetChangeValue();
        int da = _ap.GetChangeValue();
        _spirit.SetValue(_spirit.CurrentValue);
        _ap.SetValue(_ap.CurrentValue);
        return (ds, da);
    }

    public void AdjustInitValue(int failedChangedSpirit, int failedChangedAp)
    {
        _spirit.AdjustInitValue(_spirit.InitValue - failedChangedSpirit);
        _ap.AdjustInitValue(_ap.InitValue - failedChangedAp);
    }
}
