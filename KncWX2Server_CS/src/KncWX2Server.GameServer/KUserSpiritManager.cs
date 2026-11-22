namespace KncWX2Server.GameServer;

/// <summary>
/// Faithful C# port of GameServer/UserSpiritManager.{h,cpp}:
/// manages character stamina/willpower (Spirit), exemptions (Tutorial, Event, Henir, PC-Bang, Spirit Event),
/// consumption, and restoration.
/// </summary>
public sealed class KUserSpiritManager
{
    public int Spirit { get; private set; }
    public int SpiritMax { get; private set; }
    public int AccumulationSpirit { get; private set; }
    public bool IsSpiritUpdated { get; private set; }

    public void Clear()
    {
        SpiritMax = 0;
        Spirit = 0;
        AccumulationSpirit = 0;
        IsSpiritUpdated = false;
    }

    public void Init(int spirit, int spiritMax, int accumulationSpirit = 0)
    {
        SpiritMax = Math.Max(0, spiritMax);
        Spirit = Math.Clamp(spirit, 0, Math.Max(SpiritMax, spirit));
        AccumulationSpirit = Math.Max(0, accumulationSpirit);
        IsSpiritUpdated = false;
    }

    public int RestoreSpirit(int restoreAmount)
    {
        if (restoreAmount <= 0) return Spirit;
        int prev = Spirit;
        Spirit = Math.Min(SpiritMax, Spirit + restoreAmount);
        if (Spirit != prev) IsSpiritUpdated = true;
        return Spirit;
    }

    public bool CheckingSpirit(int dungeonId, bool isPcBang, bool isHenirDungeon = false, bool isSpiritEvent = false)
    {
        if (dungeonId == 0) return false;

        // Tutorial (10000..10999), Event (39000..39999), Henir (35000..35999)
        bool isTutorial = dungeonId is >= 10000 and < 11000;
        bool isEvent = dungeonId is >= 39000 and < 40000;
        if (isTutorial || isEvent || isHenirDungeon || isPcBang || isSpiritEvent)
            return true;

        return Spirit > 0;
    }

    public int ConsumeSpirit(int amount, int dungeonId, bool isPcBang, bool isHenirDungeon = false, bool isSpiritEvent = false)
    {
        if (dungeonId is >= 10000 and < 11000 || dungeonId is >= 39000 and < 40000 ||
            isHenirDungeon || isPcBang || isSpiritEvent)
            return Spirit;

        int next = Math.Max(0, Spirit - Math.Max(0, amount));
        if (next != Spirit)
        {
            AccumulationSpirit += (Spirit - next);
            Spirit = next;
            IsSpiritUpdated = true;
        }
        return Spirit;
    }
}
