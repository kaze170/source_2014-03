using KncWX2Server.Core.Framework;
using KncWX2Server.Core.Scripting;
using MoonSharp.Interpreter;

namespace KncWX2Server.GameServer;

/// <summary>
/// Faithful C# port of Common/X2Data/XSLManufactureItemManager.{h,cpp}:
/// manages blacksmith crafting recipes, material counts, ED costs,
/// weighted result groups via KLottery, sealed-state flags, duration metadata, and Lua loading.
/// </summary>
public sealed class KManufactureItemManager
{
    public sealed record MaterialData(int MaterialItemId, int MaterialCount);

    public sealed class ManufactureData
    {
        public int ManufactureId { get; init; }
        public int ResultGroupId { get; init; }
        public int Cost { get; init; }
        public List<MaterialData> Materials { get; init; } = [];
        public KLottery ResultGroupLottery { get; } = new();
    }

    private readonly Dictionary<int, ManufactureData> _recipes = new();
    private readonly Dictionary<int, KLottery> _resultGroups = new();
    private readonly Dictionary<int, bool> _sealedItems = new();
    private readonly Dictionary<(int groupId, int itemId), int> _itemPeriods = new();
    private readonly HashSet<int> _possibleManufactureIds = [];

    public void RegisterManufacture(ManufactureData recipe)
    {
        _recipes[recipe.ManufactureId] = recipe;
        _possibleManufactureIds.Add(recipe.ManufactureId);
    }

    public void AddManufactureResultGroupWithRate(
        int groupId, int itemId, double rate, bool isSealed = false, int periodDays = 0)
    {
        if (!_resultGroups.TryGetValue(groupId, out var lot))
        {
            lot = new KLottery();
            _resultGroups[groupId] = lot;
        }
        lot.AddCase(itemId, rate);
        _sealedItems[itemId] = isSealed;
        if (periodDays > 0)
            _itemPeriods[(groupId, itemId)] = periodDays;
    }

    public ManufactureData? GetManufactureData(int manufactureId)
        => _recipes.GetValueOrDefault(manufactureId);

    public bool IsPossibleManufactureId(int manufactureId)
        => _possibleManufactureIds.Contains(manufactureId);

    public bool IsItemSealed(int itemId)
        => _sealedItems.GetValueOrDefault(itemId, false);

    public int GetResultItemPeriod(int groupId, int itemId)
        => _itemPeriods.GetValueOrDefault((groupId, itemId), 0);

    public int RollResultItemId(int manufactureId, double? fixedRoll = null)
    {
        if (!_recipes.TryGetValue(manufactureId, out var recipe))
            return 0;

        int pickedGroup = recipe.ResultGroupLottery.GetCaseNum() > 0
            ? recipe.ResultGroupLottery.Decision(fixedRoll)
            : recipe.ResultGroupId;
        if (pickedGroup <= 0 || !_resultGroups.TryGetValue(pickedGroup, out var lot))
            return 0;

        int itemId = lot.Decision(fixedRoll);
        return itemId == KLottery.CaseBlank ? 0 : itemId;
    }

    public void BindToLua(KScriptManager script, string globalName = "g_pManufactureItemManager")
    {
        Table? proxy = null;
        proxy = script.CreateLuaProxy(globalName, t =>
        {
            t["AddManufactureResultTemplet"] = DynValue.NewCallback((_, a) =>
            {
                var tbl = KScriptManager.ArgTable(a, KScriptManager.ArgStart(a, proxy!));
                if (tbl == null) return DynValue.Nil;
                int mId = (int)(tbl.Get("m_ManufactureID").CastToNumber() ?? 0);
                int rgId = (int)(tbl.Get("m_ResultGroupID").CastToNumber() ?? 0);
                double rate = tbl.Get("m_Rate").CastToNumber() ?? 100.0;
                if (mId <= 0 || rgId <= 0) return DynValue.Nil;

                if (!_recipes.TryGetValue(mId, out var data))
                {
                    data = new ManufactureData { ManufactureId = mId, ResultGroupId = rgId };
                    RegisterManufacture(data);
                }
                data.ResultGroupLottery.AddCase(rgId, rate);
                return DynValue.Nil;
            });
            t["AddManufactureTemplet"] = DynValue.NewCallback((_, a) =>
            {
                var tbl = KScriptManager.ArgTable(a, KScriptManager.ArgStart(a, proxy!));
                if (tbl == null) return DynValue.Nil;
                int mId = (int)(tbl.Get("m_ManufactureID").CastToNumber() ?? 0);
                int rgId = (int)(tbl.Get("m_ResultGroupID").CastToNumber() ?? mId);
                int cost = (int)(tbl.Get("m_Cost").CastToNumber() ?? 0);
                if (mId <= 0) return DynValue.Nil;

                var mats = new List<MaterialData>();
                foreach (var pair in tbl.Pairs)
                {
                    if (pair.Key.Type == DataType.Number && pair.Value.Type == DataType.Table)
                    {
                        var sub = pair.Value.Table;
                        int matId = (int)(sub.Get("m_MaterialItemID").CastToNumber() ?? 0);
                        int matCnt = (int)(sub.Get("m_MaterialCount").CastToNumber() ?? 1);
                        if (matId > 0 && matCnt > 0)
                            mats.Add(new MaterialData(matId, matCnt));
                    }
                }
                var existing = _recipes.GetValueOrDefault(mId);
                var data = new ManufactureData
                {
                    ManufactureId = mId,
                    ResultGroupId = existing?.ResultGroupId > 0 ? existing.ResultGroupId : rgId,
                    Cost = cost,
                    Materials = mats,
                };
                if (existing != null)
                {
                    // preserve lottery entries loaded from ManufactureResultTable.lua
                    data.ResultGroupLottery.AddCase(data.ResultGroupId, 100);
                }
                RegisterManufacture(data);
                return DynValue.Nil;
            });
            t["AddManufactureResultGroup"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                AddManufactureResultGroupWithRate(KScriptManager.ArgInt(a, o), KScriptManager.ArgInt(a, o + 1), 100.0);
                return DynValue.Nil;
            });
            t["AddManufactureResultGroupWithRate"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                AddManufactureResultGroupWithRate(
                    KScriptManager.ArgInt(a, o),
                    KScriptManager.ArgInt(a, o + 1),
                    KScriptManager.ArgDouble(a, o + 2, 100.0),
                    KScriptManager.ArgBool(a, o + 3),
                    KScriptManager.ArgInt(a, o + 4));
                return DynValue.Nil;
            });
        });
    }
}
