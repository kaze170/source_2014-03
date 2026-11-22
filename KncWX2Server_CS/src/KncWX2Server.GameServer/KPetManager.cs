using System.Collections.Concurrent;
using KncWX2Server.Core.Framework;
using KncWX2Server.Core.Scripting;
using MoonSharp.Interpreter;

namespace KncWX2Server.GameServer;

/// <summary>
/// Faithful C# port of Common/X2Data/XSLPetManager.{h,cpp} and ServerResource/TW/PetData.lua:
/// pet initial stats, create-item &amp; random-create-item mappings, transform-pet rules,
/// cash/special feed satiety &amp; intimacy modifiers, and satiety decrease factors.
/// </summary>
public sealed class KPetManager
{
    public readonly record struct PetCreateItemEntry(int ItemId, int PetId, short PeriodDays);
    public readonly record struct TransformPetRule(int TransformItemId, int CreateItemId, int RarePetId, double RareProbability);

    public sbyte NewPetEvolutionStep { get; private set; }
    public short NewPetSatiety { get; private set; } = 3840;
    public int NewPetIntimacy { get; private set; }
    public short NewPetExtroversion { get; private set; }
    public short NewPetEmotion { get; private set; }

    public float SpecialFeedIncreaseIntimacyRate { get; private set; } = 0.0201f;
    public float SpecialFeedDecreaseIntimacyRate { get; private set; } = 0.0501f;

    private readonly ConcurrentDictionary<int, PetCreateItemEntry> _createItems = new();
    private readonly ConcurrentDictionary<int, KLottery> _randomCreateItems = new();
    private readonly ConcurrentDictionary<int, TransformPetRule> _transformItems = new();
    private readonly ConcurrentDictionary<int, short> _cashFeedItems = new();
    private readonly ConcurrentDictionary<int, byte> _specialFeedItemIds = new();
    private readonly ConcurrentDictionary<int, double> _satietyDecreaseFactors = new();
    private readonly ConcurrentDictionary<int, string> _petTempletNames = new();

    public IReadOnlyDictionary<int, PetCreateItemEntry> CreateItems => _createItems;
    public IReadOnlyDictionary<int, short> CashFeedItems => _cashFeedItems;
    public IReadOnlyDictionary<int, double> SatietyDecreaseFactors => _satietyDecreaseFactors;

    public bool TryGetPetByItemId(int itemId, out int petId, out short periodDays)
    {
        if (_createItems.TryGetValue(itemId, out var entry))
        {
            petId = entry.PetId;
            periodDays = entry.PeriodDays;
            return true;
        }
        if (_randomCreateItems.TryGetValue(itemId, out var lot))
        {
            int rolled = lot.Decision();
            if (rolled != KLottery.CaseBlank && rolled > 0)
            {
                petId = rolled;
                periodDays = -1;
                return true;
            }
        }
        petId = 0;
        periodDays = -1;
        return false;
    }

    public bool TryGetCashFeedSatiety(int itemId, out short increaseSatiety)
        => _cashFeedItems.TryGetValue(itemId, out increaseSatiety);

    public bool IsSpecialFeedItemId(int itemId) => _specialFeedItemIds.ContainsKey(itemId);

    public double GetSatietyDecreaseFactor(int evolutionStep)
        => _satietyDecreaseFactors.TryGetValue(evolutionStep, out double f) ? f : 1.0;

    public bool TryGetTransformRule(int transformItemId, out TransformPetRule rule)
        => _transformItems.TryGetValue(transformItemId, out rule);

    public void BindToLua(KScriptManager script, string globalName = "g_pPetManager")
    {
        Table? proxy = null;
        proxy = script.CreateLuaProxy(globalName, t =>
        {
            t["SetNewPetInfo"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                NewPetEvolutionStep = (sbyte)KScriptManager.ArgInt(a, o, NewPetEvolutionStep);
                NewPetSatiety = (short)KScriptManager.ArgInt(a, o + 1, NewPetSatiety);
                NewPetIntimacy = KScriptManager.ArgInt(a, o + 2, NewPetIntimacy);
                NewPetExtroversion = (short)KScriptManager.ArgInt(a, o + 3, NewPetExtroversion);
                NewPetEmotion = (short)KScriptManager.ArgInt(a, o + 4, NewPetEmotion);
                return DynValue.Nil;
            });

            t["AddPetTemplet"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                var tbl = KScriptManager.ArgTable(a, o);
                if (tbl != null)
                {
                    int petId = (int)(tbl.Get("m_PetID").CastToNumber() ?? 0);
                    string name = tbl.Get("m_wstrName").CastToString() ?? string.Empty;
                    if (petId > 0)
                        _petTempletNames[petId] = name;
                }
                return DynValue.NewBoolean(true);
            });

            t["AddPetCreateItemInfo"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                int itemId = KScriptManager.ArgInt(a, o);
                int petId = KScriptManager.ArgInt(a, o + 1);
                short period = (short)KScriptManager.ArgInt(a, o + 2, -1);
                if (itemId > 0 && petId > 0)
                    _createItems[itemId] = new PetCreateItemEntry(itemId, petId, period);
                return DynValue.NewBoolean(true);
            });

            t["AddRandomPetCreateItemInfo"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                int itemId = KScriptManager.ArgInt(a, o);
                int petId = KScriptManager.ArgInt(a, o + 1);
                double prob = KScriptManager.ArgDouble(a, o + 2);
                if (itemId > 0 && petId > 0 && prob > 0)
                    _randomCreateItems.GetOrAdd(itemId, _ => new KLottery()).AddCase(petId, prob);
                return DynValue.NewBoolean(true);
            });

            t["AddTransformPetItemInfo"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                int transformItemId = KScriptManager.ArgInt(a, o);
                int createItemId = KScriptManager.ArgInt(a, o + 1);
                int rarePetId = KScriptManager.ArgInt(a, o + 2);
                double rareProb = KScriptManager.ArgDouble(a, o + 3);
                if (transformItemId > 0 && createItemId > 0)
                    _transformItems[transformItemId] = new TransformPetRule(transformItemId, createItemId, rarePetId, rareProb);
                return DynValue.NewBoolean(true);
            });

            t["AddPetCashFeedItemInfo"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                int itemId = KScriptManager.ArgInt(a, o);
                short satiety = (short)KScriptManager.ArgInt(a, o + 1);
                if (itemId > 0)
                    _cashFeedItems[itemId] = satiety;
                return DynValue.NewBoolean(true);
            });

            t["AddSpecialFeedItemID"] = DynValue.NewCallback((_, a) =>
            {
                int itemId = KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, proxy!));
                if (itemId > 0)
                    _specialFeedItemIds[itemId] = 1;
                return DynValue.NewBoolean(true);
            });

            t["AddPetSatietyDecreaseFactor"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                int step = KScriptManager.ArgInt(a, o);
                double factor = KScriptManager.ArgDouble(a, o + 1, 1.0);
                _satietyDecreaseFactors[step] = factor;
                return DynValue.NewBoolean(true);
            });

            t["IncreaseSpecialFeedIntimacyRate"] = DynValue.NewCallback((_, a) =>
            {
                SpecialFeedIncreaseIntimacyRate = KScriptManager.ArgFloat(a, KScriptManager.ArgStart(a, proxy!), SpecialFeedIncreaseIntimacyRate);
                return DynValue.Nil;
            });

            t["DecreaseSpecialFeedIntimacyRate"] = DynValue.NewCallback((_, a) =>
            {
                SpecialFeedDecreaseIntimacyRate = KScriptManager.ArgFloat(a, KScriptManager.ArgStart(a, proxy!), SpecialFeedDecreaseIntimacyRate);
                return DynValue.Nil;
            });
        });
    }
}
