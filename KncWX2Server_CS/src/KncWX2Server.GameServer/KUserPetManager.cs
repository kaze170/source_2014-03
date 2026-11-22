using KncWX2Server.Core.Packets;

namespace KncWX2Server.GameServer;

/// <summary>
/// Faithful C# port of GameServer/UserPetManager.{h,cpp} &amp; UserPet.{h,cpp}:
/// pet collection (PE_MAX_PET_COUNT=3 per page=6), summon/unsummon, feeding, satiety/intimacy,
/// hunger aura threshold, and evolution step progression.
/// </summary>
public sealed class KUserPetManager
{
    public const int PeMaxPetCount = 3;
    public const int PePagePerCount = 6;
    public const short MaxSatiety = 4800;
    public const int HungrySatietyThreshold = 1200; // below ~25% disables aura skill

    private readonly Dictionary<long, KPetInfo> _pets = new();
    private readonly object _sync = new();

    public long SummonedPetUid { get; private set; }
    public long LastSummonedPetUid { get; private set; }
    public int SummonCount { get; private set; }

    public void Clear()
    {
        lock (_sync)
        {
            _pets.Clear();
            SummonedPetUid = 0;
            LastSummonedPetUid = 0;
            SummonCount = 0;
        }
    }

    public void Init(IEnumerable<KPetInfo> pets, long summonedPetUid = 0)
    {
        lock (_sync)
        {
            _pets.Clear();
            foreach (var p in pets)
                _pets[p.PetUid] = p;
            SummonedPetUid = summonedPetUid > 0 && _pets.ContainsKey(summonedPetUid) ? summonedPetUid : 0;
            LastSummonedPetUid = SummonedPetUid;
        }
    }

    public void AddPet(KPetInfo pet)
    {
        lock (_sync) _pets[pet.PetUid] = pet;
    }

    public List<KPetInfo> GetPetPage(int pageIndex)
    {
        lock (_sync)
            return _pets.Values.Skip(Math.Max(0, pageIndex) * PePagePerCount).Take(PePagePerCount).ToList();
    }

    public List<KPetInfo> GetAllPets()
    {
        lock (_sync) return _pets.Values.ToList();
    }

    public int TotalCount { get { lock (_sync) return _pets.Count; } }

    public (bool ok, KPetInfo? pet) RenameSummonedPet(string newName)
    {
        lock (_sync)
        {
            var pet = GetSummonedPet();
            if (pet == null || string.IsNullOrWhiteSpace(newName)) return (false, null);
            pet.PetName = newName;
            return (true, pet);
        }
    }

    public (bool ok, bool autoFeed) SetAutoFeedSummonedPet(bool autoFeed)
    {
        lock (_sync)
        {
            var pet = GetSummonedPet();
            if (pet == null) return (false, false);
            pet.AutoFeed = autoFeed;
            return (true, pet.AutoFeed);
        }
    }

    public KPetInfo? GetSummonedPet()
    {
        lock (_sync)
            return SummonedPetUid > 0 && _pets.TryGetValue(SummonedPetUid, out var p) ? p : null;
    }

    public bool IsSummonedPetHungry()
    {
        var pet = GetSummonedPet();
        return pet != null && pet.Satiety < HungrySatietyThreshold;
    }

    public (bool ok, KPetInfo? pet) SummonPet(long petUid) => SummonOrUnsummon(petUid);

    public (bool ok, KPetInfo? pet) SummonOrUnsummon(long petUid)
    {
        lock (_sync)
        {
            if (petUid == 0 || SummonedPetUid == petUid)
            {
                SummonedPetUid = 0;
                return (true, null);
            }
            if (!_pets.TryGetValue(petUid, out var pet))
                return (false, null);

            SummonedPetUid = petUid;
            LastSummonedPetUid = petUid;
            SummonCount++;
            return (true, pet);
        }
    }

    public (bool ok, short newSatiety, int newIntimacy) FeedSummonedPet(int satietyGain, int intimacyGain)
    {
        lock (_sync)
        {
            var pet = GetSummonedPet();
            if (pet == null) return (false, 0, 0);
            pet.Satiety = (short)Math.Clamp(pet.Satiety + satietyGain, 0, MaxSatiety);
            pet.Intimacy = Math.Max(0, pet.Intimacy + intimacyGain);
            return (true, pet.Satiety, pet.Intimacy);
        }
    }

    public (bool ok, short newSatiety) DecreaseSatietyOnRoom(int userCount, bool haveToy)
    {
        lock (_sync)
        {
            var pet = GetSummonedPet();
            if (pet == null) return (false, 0);
            int drain = Math.Max(1, (haveToy ? 5 : 10) * Math.Max(1, userCount));
            pet.Satiety = (short)Math.Max(0, pet.Satiety - drain);
            return (true, pet.Satiety);
        }
    }

    public (bool ok, sbyte newStep) EvolvePet(long petUid)
    {
        lock (_sync)
        {
            if (!_pets.TryGetValue(petUid, out var pet)) return (false, 0);
            if (pet.EvolutionStep >= 3) return (false, pet.EvolutionStep);
            pet.EvolutionStep++;
            return (true, pet.EvolutionStep);
        }
    }

    public (bool ok, KPetInfo? pet) AdminChangePetInfo(int cheatType, int value)
    {
        lock (_sync)
        {
            var pet = GetSummonedPet();
            if (pet == null) return (false, null);
            switch (cheatType)
            {
                case 0: // PCT_EVOLUTION_STEP
                    pet.EvolutionStep = (sbyte)Math.Clamp(value, 0, 3);
                    break;
                case 1: // PCT_SATIETY
                    pet.Satiety = (short)Math.Clamp(value, 0, MaxSatiety);
                    break;
                case 2: // PCT_INTIMACY
                    pet.Intimacy = Math.Max(0, value);
                    break;
                case 3: // PCT_EXTROVERSION
                    pet.Extroversion = (short)Math.Clamp(value, -1000, 1000);
                    break;
                case 4: // PCT_EMOTION
                    pet.Emotion = (short)Math.Clamp(value, -1000, 1000);
                    break;
                default:
                    return (false, null);
            }
            return (true, pet);
        }
    }

    public (bool ok, int netError, int upPercent, KPetInfo? pet) UseIntimacyUpItem(int intimacyGainPercent = 20)
    {
        lock (_sync)
        {
            var pet = GetSummonedPet();
            if (pet == null)
                return (false, KncWX2Server.Core.Region.NetError.ERR_PET_05, 0, null);

            int add = Math.Max(100, intimacyGainPercent * 100);
            pet.Intimacy = Math.Min(10_000, pet.Intimacy + add);
            return (true, KncWX2Server.Core.Region.NetError.NET_OK, intimacyGainPercent, pet);
        }
    }
}
