using KncWX2Server.Core.Framework;
using KncWX2Server.Core.Scripting;
using MoonSharp.Interpreter;

namespace KncWX2Server.GameServer;

/// <summary>
/// Faithful C# port of ServerResource/TW/SocketItemTable.lua &amp; AttribEnchantTable.lua:
/// weighted socket option rolls via KLottery, socket group counts,
/// item identification (130054 -> 130048..130053), and single/dual/triple attribute enchant selection.
/// </summary>
public sealed class KSocketItemManager
{
    public enum SocketDataType
    {
        SdtNone = 0,
        SdtWeaponNormal = 1,
        SdtWeaponRare = 2,
        SdtDefenceNormal = 3,
        SdtDefenceRare = 4,
        SdtSetItem = 5,
        SdtAccessory = 6,
        SdtTitle = 7,
        SdtWeaponHarmony = 8,
        SdtDefenceHarmony = 9,
    }

    public enum EnchantElement : sbyte
    {
        None = 0,
        Blaze = 1,   // ATI_RED 130048
        Water = 2,   // ATI_BLUE 130049
        Nature = 3,  // ATI_GREEN 130051
        Wind = 4,    // ATI_WIND 130050
        Light = 5,   // ATI_LIGHT 130053
        Dark = 6,    // ATI_DARK 130052
    }

    public const int AtiUnknown = 130047;
    public const int AtiRed = 130048;
    public const int AtiBlue = 130049;
    public const int AtiWind = 130050;
    public const int AtiGreen = 130051;
    public const int AtiDark = 130052;
    public const int AtiLight = 130053;
    public const int AtiIdentifyStone = 130054;
    public const int AtiUnknownStone = 130055;
    public const int AtiEssence = 130208;

    public enum AttribEnchantType
    {
        AetNone = 0,
        AetSingleWeapon = 1,
        AetDualWeapon = 2,
        AetArmor = 3,
        AetTripleWeapon = 4,
        AetDualArmor = 5,
    }

    public enum ItemGrade
    {
        IgLow = 0,
        IgNormal = 1,
        IgRare = 2,
        IgElite = 3,
        IgUnique = 4,
    }

    public readonly record struct AttribAttachData(int ItemId, EnchantElement Slot0, EnchantElement Slot1, EnchantElement Slot2);
    public readonly record struct MiracleCubeInfo(int ItemId, int MinLevel, int MaxLevel);

    private readonly Dictionary<int, KLottery> _socketLotteries = new();
    private readonly Dictionary<int, KLottery> _randomSocketGroups = new();
    private readonly Dictionary<(int group, int itemType, int grade), KLottery> _socketGroupCounts = new();
    private readonly Dictionary<int, (int StoneItemId, KLottery ResultLottery)> _identifyTables = new();
    private readonly Dictionary<int, AttribAttachData> _attribAttachItems = new();
    private readonly Dictionary<int, MiracleCubeInfo> _miracleCubes = new();
    private readonly KLottery _singleAttribLottery = new();
    private readonly Dictionary<EnchantElement, KLottery> _dualAttribLotteries = new();
    private readonly Dictionary<(EnchantElement s1, EnchantElement s2), KLottery> _tripleAttribLotteries = new();
    private readonly Random _rng = new();

    public IReadOnlyDictionary<int, AttribAttachData> AttribAttachItems => _attribAttachItems;
    public IReadOnlyDictionary<int, MiracleCubeInfo> MiracleCubes => _miracleCubes;

    public bool TryGetAttribAttachInfo(int itemId, out AttribAttachData data)
        => _attribAttachItems.TryGetValue(itemId, out data);

    public bool TryGetMiracleCubeInfo(int itemId, out MiracleCubeInfo info)
        => _miracleCubes.TryGetValue(itemId, out info);

    public int RollRandomSocketOption(int groupId, double? fixedUnitRoll = null)
    {
        if (!_randomSocketGroups.TryGetValue(groupId, out var lottery))
            return 0;
        int res = lottery.Decision(fixedUnitRoll);
        return res == KLottery.CaseBlank ? 0 : res;
    }

    public List<int> EvaluateRandomSockets(int groupId = 1, int count = 2, double? fixedUnitRoll = null)
    {
        var result = new List<int>(Math.Max(0, count));
        if (!_randomSocketGroups.TryGetValue(groupId, out var lottery))
        {
            if (_randomSocketGroups.Count == 0) return result;
            lottery = _randomSocketGroups.Values.First();
        }
        for (int i = 0; i < count; i++)
        {
            int opt = lottery.Decision(fixedUnitRoll);
            if (opt != KLottery.CaseBlank && opt > 0)
                result.Add(opt);
        }
        return result;
    }

    public KSocketItemManager()
    {
        SeedDefaultTwTables();
    }

    public void AddSocketRandomData(SocketDataType type, int optionId, float prob)
        => AddSocketRandomData((int)type, optionId, prob);

    public void AddSocketRandomData(int typeInt, int optionId, float prob)
    {
        if (!_socketLotteries.TryGetValue(typeInt, out var lottery))
        {
            lottery = new KLottery();
            _socketLotteries[typeInt] = lottery;
        }
        lottery.AddCase(optionId, prob);
    }

    public int RollSocketOption(SocketDataType type, double? fixedUnitRoll = null)
        => RollSocketOption((int)type, fixedUnitRoll);

    public int RollSocketOption(int typeInt, double? fixedUnitRoll = null)
    {
        if (!_socketLotteries.TryGetValue(typeInt, out var lottery))
            return 0;
        int res = lottery.Decision(fixedUnitRoll);
        return res == KLottery.CaseBlank ? 0 : res;
    }

    public void AddIdentifyInfo(int unknownItemId, int stoneItemId, int resultItemId, float prob)
    {
        if (!_identifyTables.TryGetValue(unknownItemId, out var entry))
        {
            entry = (stoneItemId, new KLottery());
            _identifyTables[unknownItemId] = entry;
        }
        entry.ResultLottery.AddCase(resultItemId, prob);
    }

    public int GetIdentifyStoneItemId(int unknownItemId)
        => _identifyTables.TryGetValue(unknownItemId, out var entry) ? entry.StoneItemId : AtiIdentifyStone;

    public bool IsIdentifiableItem(int itemId) => _identifyTables.ContainsKey(itemId);

    public int RollIdentifyResult(int unknownItemId, double? fixedUnitRoll = null)
    {
        if (!_identifyTables.TryGetValue(unknownItemId, out var entry))
            return 0;
        int res = entry.ResultLottery.Decision(fixedUnitRoll);
        return res == KLottery.CaseBlank ? 0 : res;
    }

    public sbyte RollOrResolveAttribute(int shardItemId, sbyte currentSlot0Attrib, double? fixedUnitRoll = null, sbyte currentSlot1Attrib = 0)
    {
        switch (shardItemId)
        {
            case AtiRed: return (sbyte)EnchantElement.Blaze;
            case AtiBlue: return (sbyte)EnchantElement.Water;
            case AtiGreen: return (sbyte)EnchantElement.Nature;
            case AtiWind: return (sbyte)EnchantElement.Wind;
            case AtiLight: return (sbyte)EnchantElement.Light;
            case AtiDark: return (sbyte)EnchantElement.Dark;
            case AtiUnknown:
            default:
                if (currentSlot0Attrib > 0 && currentSlot1Attrib > 0 &&
                    _tripleAttribLotteries.TryGetValue(((EnchantElement)currentSlot0Attrib, (EnchantElement)currentSlot1Attrib), out var triple))
                {
                    int picked3 = triple.Decision(fixedUnitRoll);
                    if (picked3 != KLottery.CaseBlank) return (sbyte)picked3;
                }
                if (currentSlot0Attrib > 0 &&
                    _dualAttribLotteries.TryGetValue((EnchantElement)currentSlot0Attrib, out var dual))
                {
                    int picked = dual.Decision(fixedUnitRoll);
                    if (picked != KLottery.CaseBlank) return (sbyte)picked;
                }
                int single = _singleAttribLottery.Decision(fixedUnitRoll);
                return single == KLottery.CaseBlank
                    ? (sbyte)(_rng.Next(1, 7))
                    : (sbyte)single;
        }
    }

    public static int GetRequiredShardCount(AttribEnchantType enchantType, int equipLevel, ItemGrade grade)
    {
        int levelTier = Math.Max(1, (equipLevel + 9) / 10);
        int baseCost = enchantType switch
        {
            AttribEnchantType.AetSingleWeapon => levelTier * 2,
            AttribEnchantType.AetDualWeapon => levelTier * 4,
            AttribEnchantType.AetTripleWeapon => levelTier * 8,
            AttribEnchantType.AetArmor => levelTier,
            AttribEnchantType.AetDualArmor => levelTier * 2,
            _ => levelTier,
        };
        int gradeMult = grade switch
        {
            ItemGrade.IgUnique => 4,
            ItemGrade.IgElite => 3,
            ItemGrade.IgRare => 2,
            _ => 1,
        };
        return Math.Max(1, baseCost * gradeMult);
    }

    public static int GetShardItemIdForElement(sbyte attribId) => (EnchantElement)attribId switch
    {
        EnchantElement.Blaze => AtiRed,
        EnchantElement.Water => AtiBlue,
        EnchantElement.Nature => AtiGreen,
        EnchantElement.Wind => AtiWind,
        EnchantElement.Light => AtiLight,
        EnchantElement.Dark => AtiDark,
        _ => AtiUnknown,
    };

    public static SocketDataType GetSocketDataTypeForMagicStone(int magicStoneItemId, bool isWeapon)
    {
        if (magicStoneItemId == 109955) // MSI_RARE_MAGIC_STONE
            return isWeapon ? SocketDataType.SdtWeaponRare : SocketDataType.SdtDefenceRare;
        if (magicStoneItemId == 132110) // MSI_HARMONY_MAGIC_STONE
            return isWeapon ? SocketDataType.SdtWeaponHarmony : SocketDataType.SdtDefenceHarmony;
        return isWeapon ? SocketDataType.SdtWeaponNormal : SocketDataType.SdtDefenceNormal;
    }

    public void BindSocketToLua(KScriptManager script, string globalName = "g_pCX2SocketItem")
    {
        Table? proxy = null;
        proxy = script.CreateLuaProxy(globalName, t =>
        {
            t["AddSocketRandomData"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                AddSocketRandomData(KScriptManager.ArgInt(a, o), KScriptManager.ArgInt(a, o + 1), KScriptManager.ArgFloat(a, o + 2));
                return DynValue.Nil;
            });
            t["AddSocketGroup"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                var key = (KScriptManager.ArgInt(a, o), KScriptManager.ArgInt(a, o + 1), KScriptManager.ArgInt(a, o + 2));
                if (!_socketGroupCounts.TryGetValue(key, out var lot))
                {
                    lot = new KLottery();
                    _socketGroupCounts[key] = lot;
                }
                lot.AddCase(KScriptManager.ArgInt(a, o + 3), KScriptManager.ArgFloat(a, o + 4));
                return DynValue.Nil;
            });
            t["AddRandomSocketGroupInfo"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                int groupId = KScriptManager.ArgInt(a, o);
                int socketId = KScriptManager.ArgInt(a, o + 1);
                float rate = KScriptManager.ArgFloat(a, o + 2);
                if (!_randomSocketGroups.TryGetValue(groupId, out var lot))
                {
                    lot = new KLottery();
                    _randomSocketGroups[groupId] = lot;
                }
                if (socketId > 0 && rate > 0) lot.AddCase(socketId, rate);
                return DynValue.Nil;
            });
            t["AddMiracleCubeInfo"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                int itemId = KScriptManager.ArgInt(a, o);
                if (itemId > 0)
                    _miracleCubes[itemId] = new MiracleCubeInfo(itemId, KScriptManager.ArgInt(a, o + 1), KScriptManager.ArgInt(a, o + 2));
                return DynValue.Nil;
            });
        });
    }

    public void BindAttribToLua(KScriptManager script, string globalName = "g_pCX2EnchantItem")
    {
        Table? proxy = null;
        proxy = script.CreateLuaProxy(globalName, t =>
        {
            t["AddRandomAttribSingle"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                _singleAttribLottery.AddCase(KScriptManager.ArgInt(a, o), KScriptManager.ArgFloat(a, o + 1));
                return DynValue.Nil;
            });
            t["AddRandomAttribDual"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                var el1 = (EnchantElement)KScriptManager.ArgInt(a, o);
                int el2 = KScriptManager.ArgInt(a, o + 1);
                float prob = KScriptManager.ArgFloat(a, o + 2);
                if (!_dualAttribLotteries.TryGetValue(el1, out var lot))
                {
                    lot = new KLottery();
                    _dualAttribLotteries[el1] = lot;
                }
                lot.AddCase(el2, prob);
                return DynValue.Nil;
            });
            t["AddRandomAttribTriple"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                var key = ((EnchantElement)KScriptManager.ArgInt(a, o), (EnchantElement)KScriptManager.ArgInt(a, o + 1));
                int el3 = KScriptManager.ArgInt(a, o + 2);
                float prob = KScriptManager.ArgFloat(a, o + 3);
                if (!_tripleAttribLotteries.TryGetValue(key, out var lot))
                {
                    lot = new KLottery();
                    _tripleAttribLotteries[key] = lot;
                }
                lot.AddCase(el3, prob);
                return DynValue.Nil;
            });
            t["AddIdentifyInfo"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                AddIdentifyInfo(KScriptManager.ArgInt(a, o), KScriptManager.ArgInt(a, o + 1), KScriptManager.ArgInt(a, o + 2), KScriptManager.ArgFloat(a, o + 3));
                return DynValue.Nil;
            });
            t["AddAttribAttachInfo"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                int itemId = KScriptManager.ArgInt(a, o);
                if (itemId > 0)
                {
                    _attribAttachItems[itemId] = new AttribAttachData(
                        itemId,
                        (EnchantElement)KScriptManager.ArgInt(a, o + 1),
                        (EnchantElement)KScriptManager.ArgInt(a, o + 2),
                        (EnchantElement)KScriptManager.ArgInt(a, o + 3));
                }
                return DynValue.Nil;
            });
        });
    }

    private void SeedDefaultTwTables()
    {
        (int opt, float p)[] weaponNormal =
        [
            (10000, 12), (10010, 10), (10020, 12), (10030, 16), (10040, 16),
            (10050, 4), (10060, 4), (10070, 5), (10080, 7), (10090, 7), (10100, 7),
        ];
        foreach (var (opt, p) in weaponNormal)
        {
            AddSocketRandomData(SocketDataType.SdtWeaponNormal, opt, p);
            AddSocketRandomData(SocketDataType.SdtWeaponRare, opt + 10000, p);
        }

        (int opt, float p)[] defenceNormal =
        [
            (30000, 6), (30010, 6), (30020, 6), (30030, 8), (30040, 8), (30050, 8),
            (30060, 8), (30070, 8), (30080, 14), (30090, 14), (30100, 14),
        ];
        foreach (var (opt, p) in defenceNormal)
        {
            AddSocketRandomData(SocketDataType.SdtDefenceNormal, opt, p);
            AddSocketRandomData(SocketDataType.SdtDefenceRare, opt + 10000, p);
        }

        AddSocketRandomData(SocketDataType.SdtWeaponHarmony, 50000, 5);
        AddSocketRandomData(SocketDataType.SdtWeaponHarmony, 50010, 15);
        AddSocketRandomData(SocketDataType.SdtWeaponHarmony, 50020, 30);
        AddSocketRandomData(SocketDataType.SdtWeaponHarmony, 50030, 40);
        AddSocketRandomData(SocketDataType.SdtWeaponHarmony, 50040, 10);

        // AttribEnchantTable.lua identify defaults (130047 + 130054 -> 130048..130053)
        AddIdentifyInfo(AtiUnknown, AtiIdentifyStone, AtiRed, 22);
        AddIdentifyInfo(AtiUnknown, AtiIdentifyStone, AtiBlue, 15);
        AddIdentifyInfo(AtiUnknown, AtiIdentifyStone, AtiWind, 22);
        AddIdentifyInfo(AtiUnknown, AtiIdentifyStone, AtiGreen, 15);
        AddIdentifyInfo(AtiUnknown, AtiIdentifyStone, AtiLight, 13);
        AddIdentifyInfo(AtiUnknown, AtiIdentifyStone, AtiDark, 13);

        // AttribEnchantTable.lua single unknown shard probabilities
        _singleAttribLottery.AddCase((int)EnchantElement.Blaze, 23);
        _singleAttribLottery.AddCase((int)EnchantElement.Water, 15);
        _singleAttribLottery.AddCase((int)EnchantElement.Nature, 23);
        _singleAttribLottery.AddCase((int)EnchantElement.Wind, 15);
        _singleAttribLottery.AddCase((int)EnchantElement.Light, 12);
        _singleAttribLottery.AddCase((int)EnchantElement.Dark, 11);

        // Dual & Triple attribute weights from AttribEnchantTable.lua
        foreach (var primary in new[] { EnchantElement.Wind, EnchantElement.Light, EnchantElement.Dark })
        {
            var l = new KLottery();
            l.AddCase((int)EnchantElement.Blaze, 31);
            l.AddCase((int)EnchantElement.Water, 21);
            l.AddCase((int)EnchantElement.Nature, 29);
            l.AddCase((int)primary, 19);
            _dualAttribLotteries[primary] = l;

            var t = new KLottery();
            t.AddCase((int)EnchantElement.Wind, 34);
            t.AddCase((int)EnchantElement.Light, 33);
            t.AddCase((int)EnchantElement.Dark, 33);
            _tripleAttribLotteries[(primary, primary)] = t;
            _tripleAttribLotteries[(primary, EnchantElement.Blaze)] = t;
            _tripleAttribLotteries[(primary, EnchantElement.Water)] = t;
            _tripleAttribLotteries[(primary, EnchantElement.Nature)] = t;
        }
    }
}
