using KncWX2Server.Core.Packets;

namespace KncWX2Server.GameServer;

/// <summary>
/// Faithful C# port of GameServer/UserRidingPetManager.{h,cpp} (SERV_RIDING_PET_SYSTM):
/// riding pet inventory, summon/unsummon with stamina validation, pagination, and release.
/// </summary>
public sealed class KUserRidingPetManager
{
    public enum UserRidingPetManagerResult
    {
        RidingPetSucceed = 0,
        RidingPetFailed = 1,
        RidingPetNotInited = 2,
        RidingPetCreateCountLimited = 3,
        RidingPetAlreadyInited = 4,
        RidingPetAlreadySummoned = 5,
        RidingPetNotExist = 6,
        RidingPetSomethingSummoned = 7,
        RidingPetAlreadyUnsummoned = 8,
        RidingPetNotSummoned = 9,
        RidingPetEmpty = 10,
        RidingPetIncorrectedStamina = 11,
    }

    public const int MaxRidingPetCount = 30;
    public const int PageSize = 6;

    private readonly List<KRidingPetInfo> _list = new();
    private readonly object _sync = new();
    private bool _inited;
    private KRidingPetInfo? _summoned;

    public void Clear()
    {
        lock (_sync)
        {
            _inited = false;
            _list.Clear();
            _summoned = null;
        }
    }

    public UserRidingPetManagerResult Init(IEnumerable<KRidingPetInfo> ridingPets, bool force = false)
    {
        lock (_sync)
        {
            if (_inited && !force)
                return UserRidingPetManagerResult.RidingPetAlreadyInited;
            _list.Clear();
            _list.AddRange(ridingPets);
            _summoned = null;
            _inited = true;
            return UserRidingPetManagerResult.RidingPetSucceed;
        }
    }

    public (UserRidingPetManagerResult res, List<KRidingPetInfo> pageItems) GetUserRidingPetList(byte viewPage)
    {
        lock (_sync)
        {
            if (!_inited) return (UserRidingPetManagerResult.RidingPetNotInited, []);
            if (_list.Count == 0) return (UserRidingPetManagerResult.RidingPetEmpty, []);
            return (UserRidingPetManagerResult.RidingPetSucceed,
                _list.Skip(viewPage * PageSize).Take(PageSize).ToList());
        }
    }

    public UserRidingPetManagerResult RidingPetCreateCheck()
    {
        lock (_sync)
        {
            if (!_inited) return UserRidingPetManagerResult.RidingPetNotInited;
            return _list.Count >= MaxRidingPetCount
                ? UserRidingPetManagerResult.RidingPetCreateCountLimited
                : UserRidingPetManagerResult.RidingPetSucceed;
        }
    }

    public UserRidingPetManagerResult AddRidingPet(KRidingPetInfo info)
    {
        lock (_sync)
        {
            if (!_inited) return UserRidingPetManagerResult.RidingPetNotInited;
            if (_list.Count >= MaxRidingPetCount) return UserRidingPetManagerResult.RidingPetCreateCountLimited;
            _list.Add(info);
            return UserRidingPetManagerResult.RidingPetSucceed;
        }
    }

    public (UserRidingPetManagerResult res, KRidingPetInfo? pet) SummonRidingPet(long ridingPetUid)
    {
        lock (_sync)
        {
            if (!_inited) return (UserRidingPetManagerResult.RidingPetNotInited, null);
            if (_summoned != null)
            {
                return _summoned.RidingPetUid == ridingPetUid
                    ? (UserRidingPetManagerResult.RidingPetAlreadySummoned, _summoned)
                    : (UserRidingPetManagerResult.RidingPetSomethingSummoned, null);
            }
            var target = _list.FirstOrDefault(p => p.RidingPetUid == ridingPetUid);
            if (target == null) return (UserRidingPetManagerResult.RidingPetNotExist, null);
            _summoned = target;
            return (UserRidingPetManagerResult.RidingPetSucceed, target);
        }
    }

    public UserRidingPetManagerResult UnSummonRidingPet(long ridingPetUid, float stamina)
    {
        lock (_sync)
        {
            if (!_inited) return UserRidingPetManagerResult.RidingPetNotInited;
            if (_summoned == null) return UserRidingPetManagerResult.RidingPetNotSummoned;
            if (_summoned.RidingPetUid != ridingPetUid) return UserRidingPetManagerResult.RidingPetNotExist;
            if (stamina < 0f || stamina > 100f) return UserRidingPetManagerResult.RidingPetIncorrectedStamina;
            _summoned.Stamina = stamina;
            _summoned = null;
            return UserRidingPetManagerResult.RidingPetSucceed;
        }
    }

    public UserRidingPetManagerResult ReleaseRidingPet(long ridingPetUid)
    {
        lock (_sync)
        {
            if (!_inited) return UserRidingPetManagerResult.RidingPetNotInited;
            if (_summoned != null && _summoned.RidingPetUid == ridingPetUid)
                return UserRidingPetManagerResult.RidingPetAlreadySummoned;
            int idx = _list.FindIndex(p => p.RidingPetUid == ridingPetUid);
            if (idx < 0) return UserRidingPetManagerResult.RidingPetNotExist;
            _list.RemoveAt(idx);
            return UserRidingPetManagerResult.RidingPetSucceed;
        }
    }

    public KRidingPetInfo? GetSummonedPet()
    {
        lock (_sync) return _summoned;
    }
}
