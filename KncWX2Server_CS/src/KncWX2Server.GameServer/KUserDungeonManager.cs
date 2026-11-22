using KncWX2Server.Core.Packets;

namespace KncWX2Server.GameServer;

/// <summary>
/// Faithful C# port of GameServer/UserDungeonManager.{h,cpp}:
/// tracks Henir Normal/Challenge/Premium/Event reward limits, per-dungeon clear history,
/// and AutoParty bonus dungeon lists by village map or explicit dungeon list.
/// </summary>
public sealed class KUserDungeonManager
{
    public const int HrlcNormal = 3;
    public const int HrlcNormalChallenge = 1;
    public const int HrlcPremium = 2;

    public const sbyte ModeHenirNormal = 0;
    public const sbyte ModeHenirChallenge = 1;
    public const int HenirDungeonId = 38100; // DI_ELDER_HENIR_SPACE

    public static bool IsHenirDungeon(int dungeonId) => dungeonId == HenirDungeonId || dungeonId == 30400;

    public sealed class DungeonTemplate
    {
        public int DungeonId { get; init; }
        public byte MinLevel { get; init; } = 1;
        public byte MaxLevel { get; init; } = 99;
        public int PrerequisiteDungeonId { get; init; }
        public int RequiredItemId { get; init; }
        public int RequiredSpirit { get; init; } = 5;
        public bool IsSecretDungeon { get; init; }
    }

    private readonly HashSet<int> _clearedDungeons = [];
    private readonly Dictionary<int, DungeonTemplate> _dungeonTemplates = new();

    public bool IsPcBang { get; private set; }
    public int PcBangType { get; private set; }
    public bool UnlimitedHenirRewardEvent { get; set; }
    public int HenirRewardEventLimitCount { get; set; }

    public int PossibleNormalCount { get; private set; }
    public int PossibleChallengeNormalCount { get; private set; }
    public int PossiblePremiumCount { get; private set; }
    public int PossibleEventCount { get; private set; }

    public void Clear()
    {
        _clearedDungeons.Clear();
        IsPcBang = false;
        PcBangType = 0;
        UnlimitedHenirRewardEvent = false;
        HenirRewardEventLimitCount = 0;
        PossibleNormalCount = 0;
        PossibleChallengeNormalCount = 0;
        PossiblePremiumCount = 0;
        PossibleEventCount = 0;
    }

    public void Init(
        bool unlimitedEvent, int eventLimit,
        int normalUsed, int premiumUsed, int eventUsed,
        int challengeNormalUsed, bool isPcBang, int pcBangType = 0,
        int additionalHenirRewardCount = HrlcPremium)
    {
        UnlimitedHenirRewardEvent = unlimitedEvent;
        HenirRewardEventLimitCount = eventLimit;
        IsPcBang = isPcBang;
        PcBangType = pcBangType;

        int maxPremium = Math.Max(0, additionalHenirRewardCount);
        PossibleNormalCount = Math.Max(0, HrlcNormal - normalUsed);
        PossibleChallengeNormalCount = Math.Max(0, HrlcNormalChallenge - challengeNormalUsed);
        PossiblePremiumCount = isPcBang ? Math.Max(0, maxPremium - premiumUsed) : 0;
        PossibleEventCount = Math.Max(0, eventLimit - eventUsed);
    }

    public bool TryConsumeHenirReward(sbyte dungeonMode)
    {
        if (UnlimitedHenirRewardEvent) return true;

        if (dungeonMode == ModeHenirChallenge)
        {
            if (PossibleChallengeNormalCount > 0) { PossibleChallengeNormalCount--; return true; }
            if (PossibleEventCount > 0) { PossibleEventCount--; return true; }
            return false;
        }

        if (PossibleNormalCount > 0) { PossibleNormalCount--; return true; }
        if (IsPcBang && PossiblePremiumCount > 0) { PossiblePremiumCount--; return true; }
        if (PossibleEventCount > 0) { PossibleEventCount--; return true; }
        return false;
    }

    public void RegisterDungeonTemplate(DungeonTemplate tpl) => _dungeonTemplates[tpl.DungeonId] = tpl;

    public (bool ok, int netError) CanEnterDungeon(
        int dungeonId, byte unitLevel, int currentSpirit, Func<int, bool>? hasRequiredItem = null,
        KncWX2Server.Core.Framework.KGameSysValTable? sysVal = null)
    {
        if (!_dungeonTemplates.TryGetValue(dungeonId, out var tpl))
            return (true, KncWX2Server.Core.Region.NetError.NET_OK);

        if (unitLevel < tpl.MinLevel || unitLevel > tpl.MaxLevel)
            return (false, KncWX2Server.Core.Region.NetError.ERR_ROOM_26);

        if (tpl.PrerequisiteDungeonId > 0 && !_clearedDungeons.Contains(tpl.PrerequisiteDungeonId))
            return (false, KncWX2Server.Core.Region.NetError.ERR_ROOM_26);

        bool spiritFree = IsPcBang && (sysVal?.IsSpiritFree(PcBangType) ?? true);
        if (tpl.RequiredSpirit > 0 && !spiritFree && currentSpirit < tpl.RequiredSpirit)
            return (false, KncWX2Server.Core.Region.NetError.ERR_ROOM_27);

        bool secretFree = IsPcBang && (sysVal?.IsSecretDungeonFree(PcBangType) ?? true);
        if (tpl.RequiredItemId > 0 && !(tpl.IsSecretDungeon && secretFree) && !(hasRequiredItem?.Invoke(tpl.RequiredItemId) ?? false))
            return (false, KncWX2Server.Core.Region.NetError.ERR_ITEM_04);

        return (true, KncWX2Server.Core.Region.NetError.NET_OK);
    }

    public void InitClearedDungeons(IEnumerable<int> clearedDungeonIds)
    {
        _clearedDungeons.Clear();
        foreach (int did in clearedDungeonIds)
            _clearedDungeons.Add(did);
    }

    public void RecordDungeonClear(int dungeonId) => _clearedDungeons.Add(dungeonId);
    public bool HasClearedDungeon(int dungeonId) => _clearedDungeons.Contains(dungeonId);

    private static readonly Dictionary<int, IReadOnlyList<(sbyte mode, int dungeonId)>> VillageLocalDungeons = new()
    {
        [20001] = [(0, 30000), (0, 30001), (0, 30002)], // Ruben
        [20002] = [(0, 30010), (0, 30011), (0, 30012)], // Elder
        [20003] = [(0, 30020), (0, 30021), (0, 30022)], // Besma
        [20004] = [(0, 30030), (0, 30031), (0, 30032)], // Altera
        [20005] = [(0, 30040), (0, 30041), (0, 30042)], // Peita
        [20006] = [(0, 30050), (0, 30051), (0, 30052)], // Velder
        [20007] = [(0, 30060), (0, 30061), (0, 30062)], // Hamel
        [20008] = [(0, 30070), (0, 30071), (0, 30072)], // Sander
    };

    public static void GetAutoPartyBonusDungeonList(
        int villageMapId,
        Func<sbyte, int, bool> isBonusActive,
        IDictionary<int, KAutoPartyDungeonBonusInfo> outBonusList)
    {
        if (!VillageLocalDungeons.TryGetValue(villageMapId, out var list))
            return;
        foreach (var (mode, did) in list)
        {
            outBonusList.TryAdd(did, new KAutoPartyDungeonBonusInfo
            {
                DungeonId = did,
                DungeonMode = mode,
                ActivatedBonus = isBonusActive(mode, did),
            });
        }
    }

    public static void GetAutoPartyBonusDungeonList(
        IEnumerable<KDungeonGameSimpleInfo> dungeonList,
        Func<sbyte, int, bool> isBonusActive,
        IDictionary<int, KAutoPartyDungeonBonusInfo> outBonusList)
    {
        foreach (var info in dungeonList)
        {
            outBonusList.TryAdd(info.DungeonId, new KAutoPartyDungeonBonusInfo
            {
                DungeonId = info.DungeonId,
                DungeonMode = info.DungeonMode,
                ActivatedBonus = isBonusActive(info.DungeonMode, info.DungeonId),
            });
        }
    }
}
