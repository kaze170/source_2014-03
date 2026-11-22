using KncWX2Server.Core.Packets;

namespace KncWX2Server.GameServer;

/// <summary>
/// Faithful C# port of GameServer/UserRankingManager.{h,cpp} (SERV_HENIR_RENEWAL_2013):
/// caches per-unit standard Henir and Hero Henir rankings with dirty flags for DB persistence.
/// </summary>
public sealed class KUserRankingManager
{
    public const int HeroQualificationStage = 31;

    private KHenirRankingInfo _henirRanking = new();
    private bool _henirInited;
    private bool _henirChanged;

    private KHenirRankingInfo _henirHeroRanking = new();
    private bool _henirHeroInited;
    private bool _henirHeroChanged;

    public bool HasPendingHenirUpdate => _henirInited && _henirChanged;
    public bool HasPendingHeroUpdate => _henirHeroInited && _henirHeroChanged;

    public void Clear()
    {
        _henirRanking = new();
        _henirInited = false;
        _henirChanged = false;
        _henirHeroRanking = new();
        _henirHeroInited = false;
        _henirHeroChanged = false;
    }

    public void Init(IEnumerable<KHenirRankingInfo> henirRanking, IEnumerable<KHenirRankingInfo>? henirHeroRanking = null)
    {
        Clear();
        var first = henirRanking.FirstOrDefault();
        if (first != null)
        {
            _henirRanking = first;
            _henirInited = true;
        }
        var heroFirst = henirHeroRanking?.FirstOrDefault();
        if (heroFirst != null)
        {
            _henirHeroRanking = heroFirst;
            _henirHeroInited = true;
        }
    }

    public (List<KHenirRankingInfo> henir, List<KHenirRankingInfo> hero) GetUpdateInfo()
    {
        var henir = _henirInited && _henirChanged ? new List<KHenirRankingInfo> { _henirRanking } : [];
        var hero = _henirHeroInited && _henirHeroChanged ? new List<KHenirRankingInfo> { _henirHeroRanking } : [];
        return (henir, hero);
    }

    public void MarkPersisted()
    {
        _henirChanged = false;
        _henirHeroChanged = false;
    }

    public void SetRankingInfo(KHenirRankingInfo info, bool forceHero = false)
    {
        if (!_henirInited || IsBetterRecord(info, _henirRanking))
        {
            _henirRanking = info;
            _henirInited = true;
            _henirChanged = true;
        }

        if ((forceHero || info.StageCount >= HeroQualificationStage) &&
            (!_henirHeroInited || IsBetterRecord(info, _henirHeroRanking)))
        {
            _henirHeroRanking = info;
            _henirHeroInited = true;
            _henirHeroChanged = true;
        }
    }

    public KHenirRankingInfo? GetStandardRanking() => _henirInited ? _henirRanking : null;
    public KHenirRankingInfo? GetHeroRanking() => _henirHeroInited ? _henirHeroRanking : null;

    private static bool IsBetterRecord(KHenirRankingInfo candidate, KHenirRankingInfo current)
    {
        if (candidate.StageCount != current.StageCount)
            return candidate.StageCount > current.StageCount;
        return candidate.PlayTime < current.PlayTime;
    }
}
