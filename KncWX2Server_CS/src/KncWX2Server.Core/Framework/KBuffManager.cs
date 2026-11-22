namespace KncWX2Server.Core.Framework;

/// <summary>
/// Faithful C# port of Common/X2Data/XSLBuffManager.{h,cpp}:
/// maintains server-authoritative BuffTemplet definitions, room-type applicability,
/// expiration durations, and leave/end/death/logout lifecycle flags.
/// </summary>
public sealed class KBuffManager
{
    public const int UnitResetTimeHours = 6;
    public const int UnitResetTimeSeconds = 21_600;

    public enum BuffTempletId
    {
        None = 0,
        ProtectionOfNut = 109,
        ProtectionOfPtah = 110,
        ProtectionOfLahel = 111,
        ProtectionOfZachiel = 112,
        BlessingOfSeraphim = 113,
        BlessingOfGeb = 114,
        BlessingOfAmon = 115,
        BlessingOfCronos = 116,
        StrengtheningBody = 117,
        RallyOfHeroLevel1 = 118,
        RallyOfHeroLevel2 = 119,
        RallyOfHeroLevel3 = 120,
        DebuffRestOfRelly = 121,
        WarmEnergyOfEl = 122,
        ThanksOfResidents = 123,
        BreathOfDragon = 124,
        BlessingOfEl = 125,
        HeroOfEliosLevel1 = 126,
        HeroOfEliosLevel2 = 127,
        HeroOfEliosLevel3 = 128,
        HeroOfEliosLevel4 = 129,
        PvpRevengeModeLevel1 = 130,
        PvpRevengeModeLevel2 = 131,
        PvpRevengeModeLevel3 = 132,
        ReturnOfHero = 133,
        PremiumPcRoom = 138,
        PetAuraSkill = 139,
        MechanizationSpeedUp = 140,
        SuperArmor = 143,
        HenirFire = 147,
        HenirWater = 148,
        HenirNature = 149,
        HenirWind = 150,
        HenirLight = 151,
        FieldDefence = 187,
        AprilFoolsDay = 188,
        GateDefence = 189,
        EnemyGateDefence = 190,
        Defense2013FakeBuff = 201,
        Defense2013Buff = 202,
        Defense2013Debuff = 203,
        Defense2013BuffEvent = 204,
        FestivalEventBuff = 207,
        TradeBlock = 213,
        Defense2013Enter100PercentBuff = 300,
        DebuffRestOfRechallenge = 313,
        SecretOfThousandYearsFox = 327,
        Christmas2013 = 330,
        NaverBuff = 339,
        WeakHereticPotion = 1318,
        HereticPotion = 1319,
        RurielManaEnergizePotion = 1320,
    }

    public sealed class BuffTemplet
    {
        public int BuffId { get; init; }
        public string BuffName { get; init; } = string.Empty;
        public List<int> SocketOptions { get; init; } = [];
        public double BuffPeriodSecond { get; init; }
        public bool EndBuffAtTheLeaveDungeonRoom { get; init; }
        public bool EndBuffAtTheEndDungeonGame { get; init; }
        public bool EndBuffAtTheLeavePvpRoom { get; init; }
        public bool EndBuffAtTheEndPvpGame { get; init; }
        public bool EndBuffAtTheLeaveBattleFieldRoom { get; init; }
        public bool EndBuffAtTheUserDied { get; init; }
        public bool KeepBuffAtTheUserLogOut { get; init; }
        public List<int> RoomTypes { get; init; } = [];
        public int FactorId { get; init; }

        public bool CheckRoomType(int checkRoomType)
            => RoomTypes.Count == 0 || RoomTypes.Contains(checkRoomType);
    }

    private readonly Dictionary<int, BuffTemplet> _buffList = new();
    private readonly object _sync = new();

    public KBuffManager()
    {
        SeedDefaultTemplets();
    }

    public bool AddBuffInfo(BuffTemplet templet)
    {
        if (templet.BuffId <= 0) return false;
        lock (_sync)
        {
            _buffList[templet.BuffId] = templet;
            return true;
        }
    }

    public BuffTemplet? GetBuffTemplet(int buffId)
    {
        lock (_sync) return _buffList.GetValueOrDefault(buffId);
    }

    public bool IsExistBuffId(int buffId)
    {
        lock (_sync) return _buffList.ContainsKey(buffId);
    }

    private void SeedDefaultTemplets()
    {
        AddBuffInfo(new BuffTemplet
        {
            BuffId = (int)BuffTempletId.WarmEnergyOfEl,
            BuffName = "WarmEnergyOfEl",
            BuffPeriodSecond = 1800,
            KeepBuffAtTheUserLogOut = true,
        });
        AddBuffInfo(new BuffTemplet
        {
            BuffId = (int)BuffTempletId.ReturnOfHero,
            BuffName = "ReturnOfHero",
            BuffPeriodSecond = 3600,
            KeepBuffAtTheUserLogOut = true,
        });
        AddBuffInfo(new BuffTemplet
        {
            BuffId = (int)BuffTempletId.PremiumPcRoom,
            BuffName = "PremiumPcRoom",
            BuffPeriodSecond = UnitResetTimeSeconds,
        });
        AddBuffInfo(new BuffTemplet
        {
            BuffId = (int)BuffTempletId.Defense2013Buff,
            BuffName = "Defense2013Buff",
            BuffPeriodSecond = 7200,
            KeepBuffAtTheUserLogOut = true,
        });
        AddBuffInfo(new BuffTemplet
        {
            BuffId = (int)BuffTempletId.Defense2013Debuff,
            BuffName = "Defense2013Debuff",
            BuffPeriodSecond = 3600,
            KeepBuffAtTheUserLogOut = true,
        });
        AddBuffInfo(new BuffTemplet
        {
            BuffId = (int)BuffTempletId.DebuffRestOfRechallenge,
            BuffName = "DebuffRestOfRechallenge",
            BuffPeriodSecond = 180,
            KeepBuffAtTheUserLogOut = true,
        });
    }
}
