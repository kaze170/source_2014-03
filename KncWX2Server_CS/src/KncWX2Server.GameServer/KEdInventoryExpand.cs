using KncWX2Server.Core.Scripting;
using MoonSharp.Interpreter;

namespace KncWX2Server.GameServer;

/// <summary>
/// Faithful C# port of Common/X2Data/XSLEDInventoryExpand.{h,cpp} and ServerResource/TW/EDInventoryExpand.lua:
/// maps bank slot grades (8,16,24,32,40,48) and inventory expansion steps (1..4) to ED costs.
/// </summary>
public sealed class KEdInventoryExpand
{
    public const int BankGradeNormal = 8;
    public const int BankGradeSilver = 16;
    public const int BankGradeGold = 24;
    public const int BankGradeEmerald = 32;
    public const int BankGradeDiamond = 40;
    public const int BankGradePlatinum = 48;

    public const int MaxInventoryUpgradeGrade = 4;
    public const int MaxBankSlots = BankGradePlatinum;

    private readonly Dictionary<int, int> _bankGradeEd = new();
    private readonly Dictionary<int, int> _inventoryGradeEd = new();

    public void SetBankGradeEd(int grade, int ed) => _bankGradeEd[grade] = ed;
    public void SetInventoryGradeEd(int grade, int ed) => _inventoryGradeEd[grade] = ed;

    public int GetNextBankUpgradeEd(int gradeOrNextSlots) => _bankGradeEd.GetValueOrDefault(gradeOrNextSlots, 0);
    public int GetNextInventoryUpgradeEd(int nextGrade) => _inventoryGradeEd.GetValueOrDefault(nextGrade, 0);

    public (bool canUpgrade, int nextGrade, int costEd) GetInventoryUpgradeForCurrentGrade(int currentGrade)
    {
        int nextGrade = currentGrade + 1;
        if (nextGrade > MaxInventoryUpgradeGrade) return (false, currentGrade, 0);
        if (_inventoryGradeEd.TryGetValue(nextGrade, out int ed) && ed > 0)
            return (true, nextGrade, ed);
        return (false, nextGrade, 0);
    }

    public (bool canUpgrade, int nextSlotCount, int costEd) GetBankUpgradeForCurrentSlots(int currentSlots)
    {
        int current = Math.Max(BankGradeNormal, currentSlots);
        int nextSlots = current + 8;
        if (nextSlots > MaxBankSlots) return (false, current, 0);
        if (_bankGradeEd.TryGetValue(nextSlots, out int edBySlots) && edBySlots > 0)
            return (true, nextSlots, edBySlots);
        int step = Math.Max(1, current / 8);
        if (_bankGradeEd.TryGetValue(step, out int edByStep) && edByStep > 0)
            return (true, nextSlots, edByStep);
        return (false, nextSlots, 0);
    }

    // ---- Lua bindings for EDInventoryExpand.lua ----
    public void AddBankGradeED(Table table) => ParseGradeEdTable(table, _bankGradeEd);
    public void AddInventoryGradeED(Table table) => ParseGradeEdTable(table, _inventoryGradeEd);

    private static void ParseGradeEdTable(Table root, Dictionary<int, int> target)
    {
        var list = root.Get("GRADE_ED").Table;
        if (list == null) return;
        foreach (var pair in list.Pairs)
        {
            var entry = pair.Value.Table;
            if (entry == null) continue;
            int grade = (int)(entry.Get("GRADE").CastToNumber() ?? 0);
            int ed = (int)(entry.Get("ED").CastToNumber() ?? 0);
            if (grade > 0) target[grade] = ed;
        }
    }

    public void BindToLua(KScriptManager script, string globalName = "g_pEDInventoryExpand")
    {
        Table? proxy = null;
        proxy = script.CreateLuaProxy(globalName, t =>
        {
            t["AddBankGradeED"] = DynValue.NewCallback((_, a) =>
            {
                var tbl = KScriptManager.ArgTable(a, KScriptManager.ArgStart(a, proxy!));
                if (tbl != null) AddBankGradeED(tbl);
                return DynValue.Nil;
            });
            t["AddInventoryGradeED"] = DynValue.NewCallback((_, a) =>
            {
                var tbl = KScriptManager.ArgTable(a, KScriptManager.ArgStart(a, proxy!));
                if (tbl != null) AddInventoryGradeED(tbl);
                return DynValue.Nil;
            });
        });
    }
}
