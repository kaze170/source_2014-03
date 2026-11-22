using KncWX2Server.Core.Framework;
using KncWX2Server.Core.Scripting;
using MoonSharp.Interpreter;

namespace KncWX2Server.GameServer;

/// <summary>
/// Faithful C# port of ServerResource/TW/EnchantTable.lua &amp; XSLEnchantItemManager:
/// exact +1..+20 probabilities (Normal, EnchantPlus, Rare), stat multipliers (1.00..8.75),
/// Fluorite Ore protection (max +10 normal / +11 event), Destruction Guard protection,
/// enchant stone/support/restore item tables, and MoonSharp Lua loading.
/// </summary>
public sealed class KEnchantItemManager
{
    public enum EnchantResult
    {
        Up1 = 0,
        NoChange = 1,
        Down1 = 2,
        DownTo0 = 3,
        Break = 4,
    }

    public readonly record struct EnchantProb(double Up1, double NoChange, double Down1, double DownTo0, double Break);
    public readonly record struct EnchantStoneInfo(int ItemId, int ItemType, int BeginLv, int EndLv);
    public readonly record struct LevelRangeItemInfo(int ItemId, int BeginLv, int EndLv);
    public readonly record struct EnchantAttachItemInfo(int ItemId, int ItemType, byte EnchantLevel, int MinItemLevel, int MaxItemLevel);

    public const int SupportMaterialLimit = 10;
    public const int EventSupportMaterialLimit = 11;

    private static readonly float[] DefaultEnchantRates =
    [
        1.00f, 1.03f, 1.06f, 1.09f, 1.16f, 1.23f, 1.30f, 1.45f, 1.60f, 1.75f,
        2.15f, 2.55f, 2.95f, 3.35f, 3.75f, 4.25f, 4.75f, 5.25f, 5.75f, 6.75f, 8.75f,
    ];

    private static readonly Dictionary<int, EnchantProb> NormalProbs = new()
    {
        [1]  = new(100,   0,   0,  0,  0),
        [2]  = new(100,   0,   0,  0,  0),
        [3]  = new( 80,  20,   0,  0,  0),
        [4]  = new( 60,  40,   0,  0,  0),
        [5]  = new( 20,  50,  30,  0,  0),
        [6]  = new( 10,  40,  50,  0,  0),
        [7]  = new( 10,  20,  40, 20, 10),
        [8]  = new(  5,  15,  30, 40, 10),
        [9]  = new(  2,  10,  28, 40, 20),
        [10] = new(0.7,   7,  27, 40, 25.3),
        [11] = new(0.6,   7,  27, 30, 35.4),
        [12] = new(  0,   8,  27, 40, 25),
        [13] = new(  0,   4,  27, 40, 29),
        [14] = new(  1,   7,  27, 40, 25),
        [15] = new(0.5, 5.5,  27, 40, 27),
        [16] = new(0.5, 5.5,  27, 40, 27),
        [17] = new(0.3,   5,  27, 40, 27.7),
        [18] = new(0.3,   5,  27, 40, 27.7),
        [19] = new(0.1,   4,  27, 40, 28.9),
        [20] = new(  0,   4,  27, 40, 29),
    };

    private static readonly Dictionary<int, EnchantProb> PlusProbs = new()
    {
        [1]  = new(100,   0,    0,  0,  0),
        [2]  = new(100,   0,    0,  0,  0),
        [3]  = new( 88,  12,    0,  0,  0),
        [4]  = new( 66,  34,    0,  0,  0),
        [5]  = new( 22,  48,   30,  0,  0),
        [6]  = new( 11,  39,   50,  0,  0),
        [7]  = new( 11,  19,   40, 20, 10),
        [8]  = new(5.5, 14.5,  30, 40, 10),
        [9]  = new(2.2,  9.8,  28, 40, 20),
        [10] = new(0.77, 6.93, 27, 40, 25.3),
        [11] = new(0.66, 6.94, 27, 30, 35.4),
    };

    public int WeaponEnchantStoneId { get; private set; } = 109950;
    public int RareWeaponEnchantStoneId { get; private set; } = 152000121;
    public int ArmorEnchantStoneId { get; private set; } = 109965;
    public int RareArmorEnchantStoneId { get; private set; } = 152000122;
    public int InstanceSupportMaterialLimit { get; private set; } = SupportMaterialLimit;
    public int InstanceEventSupportMaterialLimit { get; private set; } = EventSupportMaterialLimit;
    public int EnchantLimit { get; private set; } = 11;

    private readonly List<EnchantStoneInfo> _stones = [];
    private readonly List<EnchantStoneInfo> _eventStones = [];
    private readonly List<LevelRangeItemInfo> _supportMaterials = [];
    private readonly List<LevelRangeItemInfo> _eventSupportMaterials = [];
    private readonly List<LevelRangeItemInfo> _enchantPlusItems = [];
    private readonly List<LevelRangeItemInfo> _destroyGuards = [];
    private readonly List<EnchantStoneInfo> _restoreItems = [];
    private readonly List<EnchantStoneInfo> _eventRestoreItems = [];
    private readonly Dictionary<int, EnchantAttachItemInfo> _attachItems = new();
    private readonly Dictionary<int, float> _dynamicEnchantRates = new();
    private readonly Dictionary<int, EnchantProb> _dynamicNormalProbs = new(NormalProbs);
    private readonly Dictionary<int, EnchantProb> _dynamicEventProbs = new(NormalProbs);
    private readonly Dictionary<int, EnchantProb> _dynamicPlusProbs = new(PlusProbs);
    private readonly Dictionary<int, EnchantProb> _dynamicRareProbs = new();
    private readonly KLottery _dropRandomEnchant = new();
    private readonly KLottery _cubeRandomEnchant = new();

    public IReadOnlyList<EnchantStoneInfo> EnchantStones => _stones;
    public IReadOnlyList<LevelRangeItemInfo> SupportMaterials => _supportMaterials;
    public IReadOnlyList<LevelRangeItemInfo> EventSupportMaterials => _eventSupportMaterials;
    public IReadOnlyList<LevelRangeItemInfo> EnchantPlusItems => _enchantPlusItems;
    public IReadOnlyList<LevelRangeItemInfo> DestroyGuards => _destroyGuards;
    public IReadOnlyList<EnchantStoneInfo> RestoreItems => _restoreItems;
    public IReadOnlyList<EnchantStoneInfo> EventRestoreItems => _eventRestoreItems;
    public IReadOnlyDictionary<int, EnchantAttachItemInfo> AttachItems => _attachItems;

    public bool TryGetAttachItemInfo(int itemId, out EnchantAttachItemInfo info)
        => _attachItems.TryGetValue(itemId, out info);

    public static float GetEnchantStatMultiplier(int enchantLevel)
        => DefaultEnchantRates[Math.Clamp(enchantLevel, 0, DefaultEnchantRates.Length - 1)];

    public float GetInstanceEnchantStatMultiplier(int enchantLevel)
        => _dynamicEnchantRates.TryGetValue(enchantLevel, out float rate)
            ? rate
            : GetEnchantStatMultiplier(enchantLevel);

    public int RollDropRandomEnchantLevel(double? deterministicRoll = null)
    {
        int res = _dropRandomEnchant.Decision(deterministicRoll);
        return res == KLottery.CaseBlank ? 0 : res;
    }

    public int RollCubeRandomEnchantLevel(double? deterministicRoll = null)
    {
        int res = _cubeRandomEnchant.Decision(deterministicRoll);
        return res == KLottery.CaseBlank ? 0 : res;
    }

    public static EnchantProb GetProbabilities(int targetLevel, bool isEnchantPlus)
    {
        int clamped = Math.Clamp(targetLevel, 1, 20);
        if (isEnchantPlus && PlusProbs.TryGetValue(clamped, out var p))
            return p;
        return NormalProbs.GetValueOrDefault(clamped, NormalProbs[20]);
    }

    public EnchantProb GetInstanceProbabilities(int targetLevel, bool isEnchantPlus, bool isRareStone = false, bool isEnchantEvent = false)
    {
        int clamped = Math.Clamp(targetLevel, 1, 20);
        if (isRareStone && _dynamicRareProbs.TryGetValue(clamped, out var rare))
            return rare;
        if (isEnchantPlus && _dynamicPlusProbs.TryGetValue(clamped, out var plus))
            return plus;
        if (isEnchantEvent && _dynamicEventProbs.TryGetValue(clamped, out var evProb))
            return evProb;
        return _dynamicNormalProbs.GetValueOrDefault(clamped, NormalProbs[20]);
    }

    public IReadOnlyList<EnchantStoneInfo> EventStones => _eventStones;

    public List<int> GetMatchingStoneIds(int itemType, int equipLevel, bool isRare, bool includeEvent = true)
    {
        var ids = new List<int>();
        if (isRare)
            ids.Add(itemType == 1 ? RareWeaponEnchantStoneId : RareArmorEnchantStoneId);
        else
            ids.Add(itemType == 1 ? WeaponEnchantStoneId : ArmorEnchantStoneId);

        foreach (var s in _stones)
        {
            if (s.ItemType == itemType && equipLevel >= s.BeginLv && equipLevel <= s.EndLv)
                ids.Add(s.ItemId);
        }
        if (includeEvent && !isRare)
        {
            foreach (var es in _eventStones)
            {
                if (es.ItemType == itemType && equipLevel >= es.BeginLv && equipLevel <= es.EndLv)
                    ids.Add(es.ItemId);
            }
        }
        return ids;
    }

    public List<int> GetMatchingSupportMaterialIds(int equipLevel, bool includeEvent = true)
    {
        var list = _supportMaterials.Where(m => equipLevel >= m.BeginLv && equipLevel <= m.EndLv).Select(m => m.ItemId).ToList();
        if (includeEvent)
            list.AddRange(_eventSupportMaterials.Where(m => equipLevel >= m.BeginLv && equipLevel <= m.EndLv).Select(m => m.ItemId));
        return list;
    }

    public List<int> GetMatchingEnchantPlusIds(int equipLevel)
        => _enchantPlusItems.Where(m => equipLevel >= m.BeginLv && equipLevel <= m.EndLv).Select(m => m.ItemId).ToList();

    public List<int> GetMatchingDestroyGuardIds(int equipLevel)
        => _destroyGuards.Where(m => equipLevel >= m.BeginLv && equipLevel <= m.EndLv).Select(m => m.ItemId).ToList();

    public List<int> GetMatchingRestoreItemIds(int equipLevel, bool includeEvent = true)
    {
        var list = _restoreItems.Where(m => equipLevel >= m.BeginLv && equipLevel <= m.EndLv).Select(m => m.ItemId).ToList();
        if (includeEvent)
            list.AddRange(_eventRestoreItems.Where(m => equipLevel >= m.BeginLv && equipLevel <= m.EndLv).Select(m => m.ItemId));
        return list;
    }

    public static (EnchantResult result, byte nextLevel, bool isBroken) ExecuteEnchant(
        byte currentLevel,
        bool isSupportMaterial,
        bool isEnchantPlus,
        bool isDestroyGuard,
        int? seed = null)
        => ExecuteWithProbs(currentLevel, GetProbabilities(Math.Clamp(currentLevel + 1, 1, 20), isEnchantPlus),
            isSupportMaterial, isDestroyGuard, SupportMaterialLimit, seed);

    public (EnchantResult result, byte nextLevel, bool isBroken) ExecuteInstanceEnchant(
        byte currentLevel,
        bool isSupportMaterial,
        bool isEnchantPlus,
        bool isDestroyGuard,
        bool isRareStone = false,
        int? seed = null)
        => ExecuteWithProbs(currentLevel, GetInstanceProbabilities(Math.Clamp(currentLevel + 1, 1, 20), isEnchantPlus, isRareStone),
            isSupportMaterial, isDestroyGuard, InstanceSupportMaterialLimit, seed);

    private static (EnchantResult result, byte nextLevel, bool isBroken) ExecuteWithProbs(
        byte currentLevel,
        EnchantProb prob,
        bool isSupportMaterial,
        bool isDestroyGuard,
        int supportLimit,
        int? seed)
    {
        int targetLevel = Math.Clamp(currentLevel + 1, 1, 20);
        var lottery = new KLottery(seed);
        lottery.AddCase((int)EnchantResult.Up1, prob.Up1);
        lottery.AddCase((int)EnchantResult.NoChange, prob.NoChange);
        lottery.AddCase((int)EnchantResult.Down1, prob.Down1);
        lottery.AddCase((int)EnchantResult.DownTo0, prob.DownTo0);
        lottery.AddCase((int)EnchantResult.Break, prob.Break);

        int picked = lottery.Decision();
        var res = picked == KLottery.CaseBlank ? EnchantResult.NoChange : (EnchantResult)picked;

        if (isSupportMaterial && targetLevel <= supportLimit &&
            (res == EnchantResult.Down1 || res == EnchantResult.DownTo0))
        {
            res = EnchantResult.NoChange;
        }

        if ((isDestroyGuard || (isSupportMaterial && targetLevel <= supportLimit)) &&
            res == EnchantResult.Break)
        {
            res = EnchantResult.NoChange;
        }

        return res switch
        {
            EnchantResult.Up1 => (res, (byte)Math.Min(20, currentLevel + 1), false),
            EnchantResult.Down1 => (res, (byte)Math.Max(0, currentLevel - 1), false),
            EnchantResult.DownTo0 => (res, 0, false),
            EnchantResult.Break => (res, currentLevel, true),
            _ => (EnchantResult.NoChange, currentLevel, false),
        };
    }

    public void BindToLua(KScriptManager script, string globalName = "EnchantItemManager")
    {
        Table? proxy = null;
        proxy = script.CreateLuaProxy(globalName, t =>
        {
            t["SetWeaponEnchantStone"] = DynValue.NewCallback((_, a) => { WeaponEnchantStoneId = KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, proxy!), WeaponEnchantStoneId); return DynValue.Nil; });
            t["SetRareWeaponEnchantStone"] = DynValue.NewCallback((_, a) => { RareWeaponEnchantStoneId = KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, proxy!), RareWeaponEnchantStoneId); return DynValue.Nil; });
            t["SetArmorEnchantStone"] = DynValue.NewCallback((_, a) => { ArmorEnchantStoneId = KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, proxy!), ArmorEnchantStoneId); return DynValue.Nil; });
            t["SetRareArmorEnchantStone"] = DynValue.NewCallback((_, a) => { RareArmorEnchantStoneId = KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, proxy!), RareArmorEnchantStoneId); return DynValue.Nil; });
            t["SetSupportMaterialLimit"] = DynValue.NewCallback((_, a) => { InstanceSupportMaterialLimit = KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, proxy!), SupportMaterialLimit); return DynValue.Nil; });
            t["SetEventSupportMaterialLimit"] = DynValue.NewCallback((_, a) => { InstanceEventSupportMaterialLimit = KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, proxy!), EventSupportMaterialLimit); return DynValue.Nil; });
            t["SetEnchantLimit"] = DynValue.NewCallback((_, a) => { EnchantLimit = KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, proxy!), EnchantLimit); return DynValue.Nil; });

            t["AddEnchantStoneInfo"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                _stones.Add(new EnchantStoneInfo(KScriptManager.ArgInt(a, o), KScriptManager.ArgInt(a, o + 1), KScriptManager.ArgInt(a, o + 2), KScriptManager.ArgInt(a, o + 3)));
                return DynValue.Nil;
            });
            t["AddEventEnchantStoneInfo"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                int itemId = KScriptManager.ArgInt(a, o);
                int itemType = KScriptManager.ArgInt(a, o + 1);
                int lvMin = KScriptManager.ArgInt(a, o + 2);
                int lvMax = KScriptManager.ArgInt(a, o + 3);
                if (itemId > 0 && itemType > 0 && lvMin >= 0 && lvMax <= 100)
                    _eventStones.Add(new EnchantStoneInfo(itemId, itemType, lvMin, lvMax));
                return DynValue.Nil;
            });
            t["AddSupportMaterialInfo"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                _supportMaterials.Add(new LevelRangeItemInfo(KScriptManager.ArgInt(a, o), KScriptManager.ArgInt(a, o + 1), KScriptManager.ArgInt(a, o + 2)));
                return DynValue.Nil;
            });
            t["AddEventSupportMaterialInfo"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                _eventSupportMaterials.Add(new LevelRangeItemInfo(KScriptManager.ArgInt(a, o), KScriptManager.ArgInt(a, o + 1), KScriptManager.ArgInt(a, o + 2)));
                return DynValue.Nil;
            });
            t["AddEnchantPlusInfo"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                _enchantPlusItems.Add(new LevelRangeItemInfo(KScriptManager.ArgInt(a, o), KScriptManager.ArgInt(a, o + 1), KScriptManager.ArgInt(a, o + 2)));
                return DynValue.Nil;
            });
            t["AddDestroyGuardInfo"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                _destroyGuards.Add(new LevelRangeItemInfo(KScriptManager.ArgInt(a, o), KScriptManager.ArgInt(a, o + 1), KScriptManager.ArgInt(a, o + 2)));
                return DynValue.Nil;
            });
            t["AddRestoreItemInfo"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                _restoreItems.Add(new EnchantStoneInfo(KScriptManager.ArgInt(a, o), KScriptManager.ArgInt(a, o + 1), KScriptManager.ArgInt(a, o + 2), KScriptManager.ArgInt(a, o + 3)));
                return DynValue.Nil;
            });
            t["AddEventRestoreItemInfo"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                _eventRestoreItems.Add(new EnchantStoneInfo(KScriptManager.ArgInt(a, o), KScriptManager.ArgInt(a, o + 1), KScriptManager.ArgInt(a, o + 2), KScriptManager.ArgInt(a, o + 3)));
                return DynValue.Nil;
            });
            t["SetEnchantRate"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                int lvl = KScriptManager.ArgInt(a, o);
                float rate = KScriptManager.ArgFloat(a, o + 1, 1.0f);
                _dynamicEnchantRates[lvl] = rate;
                return DynValue.Nil;
            });
            t["AddDropRandomEnchantInfo"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                int lvl = KScriptManager.ArgInt(a, o);
                double prob = KScriptManager.ArgDouble(a, o + 1);
                if (prob > 0) _dropRandomEnchant.AddCase(lvl, prob);
                return DynValue.Nil;
            });
            t["AddCubeRandomEnchantInfo"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                int lvl = KScriptManager.ArgInt(a, o);
                double prob = KScriptManager.ArgDouble(a, o + 1);
                if (prob > 0) _cubeRandomEnchant.AddCase(lvl, prob);
                return DynValue.Nil;
            });
            t["AddAttachItemInfo"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                int itemId = KScriptManager.ArgInt(a, o);
                var tbl = KScriptManager.ArgTable(a, o + 1);
                if (itemId > 0 && tbl != null)
                {
                    int itemType = (int)(tbl.Get("iItemType").CastToNumber() ?? 0);
                    byte enchLv = (byte)Math.Clamp((int)(tbl.Get("iEnchantLevel").CastToNumber() ?? 0), 1, 20);
                    int minLv = (int)(tbl.Get("iMinItemLevel").CastToNumber() ?? 0);
                    int maxLv = (int)(tbl.Get("iMaxItemLevel").CastToNumber() ?? 999);
                    _attachItems[itemId] = new EnchantAttachItemInfo(itemId, itemType, enchLv, minLv, maxLv);
                }
                return DynValue.Nil;
            });
            t["SetEnchantProbability"] = DynValue.NewCallback((_, a) => ParseProb(_dynamicNormalProbs, a, proxy!));
            t["SetEnchantProbability_Event"] = DynValue.NewCallback((_, a) => ParseProb(_dynamicEventProbs, a, proxy!));
            t["SetEnchantPlusProbability"] = DynValue.NewCallback((_, a) => ParseProb(_dynamicPlusProbs, a, proxy!));
            t["SetEnchantRareProbability"] = DynValue.NewCallback((_, a) => ParseProb(_dynamicRareProbs, a, proxy!));
        });
    }

    private static DynValue ParseProb(Dictionary<int, EnchantProb> target, CallbackArguments a, Table proxy)
    {
        int o = KScriptManager.ArgStart(a, proxy);
        int lvl = KScriptManager.ArgInt(a, o);
        var tbl = KScriptManager.ArgTable(a, o + 1);
        if (lvl > 0 && tbl != null)
        {
            target[lvl] = new EnchantProb(
                tbl.Get("Up1").CastToNumber() ?? 0,
                tbl.Get("NoChange").CastToNumber() ?? 0,
                tbl.Get("Down1").CastToNumber() ?? 0,
                tbl.Get("DownTo0").CastToNumber() ?? 0,
                tbl.Get("Break").CastToNumber() ?? 0);
        }
        return DynValue.Nil;
    }
}
