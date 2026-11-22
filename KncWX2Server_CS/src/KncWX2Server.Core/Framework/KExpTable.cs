using KncWX2Server.Core.Logging;
using KncWX2Server.Core.Scripting;
using MoonSharp.Interpreter;

namespace KncWX2Server.Core.Framework;

/// <summary>
/// Faithful C# port of Common/ExpTable.{h,cpp}:
/// cumulative/level-up EXP table with CheckLevelUp clamped by server max level.
/// </summary>
public sealed class KExpTable
{
    public readonly record struct ExpEntry(int NeedExp, int TotalExp);

    private readonly SortedDictionary<int, ExpEntry> _table = new();
    private readonly object _sync = new();

    public KExpTable(bool seedDefaultCurve = true)
    {
        if (seedDefaultCurve)
            SeedDefaultTable();
    }

    public int Count { get { lock (_sync) return _table.Count; } }

    public void Clear()
    {
        lock (_sync) _table.Clear();
    }

    public void BindToLua(KScriptManager script, string globalName = "g_pkExpTable")
    {
        Table? proxy = null;
        proxy = script.CreateLuaProxy(globalName, t =>
        {
            t["AddExpTable_LUA"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                int level, needExp, totalExp;
                if (a.Count - o >= 3 && a[o].Type == DataType.Number)
                {
                    level = KScriptManager.ArgInt(a, o);
                    needExp = KScriptManager.ArgInt(a, o + 1);
                    totalExp = KScriptManager.ArgInt(a, o + 2);
                }
                else
                {
                    level = (int)(script.Lua.Globals.Get("LEVEL").CastToNumber() ?? 0);
                    needExp = (int)(script.Lua.Globals.Get("NEED_EXP").CastToNumber() ?? 0);
                    totalExp = (int)(script.Lua.Globals.Get("TOTAL_EXP").CastToNumber() ?? 0);
                }
                if (level != 1 && (level <= 0 || needExp <= 0 || totalExp <= 0))
                {
                    KLog.Cerr($"KExpTable::AddExpTable_LUA read fail: Level={level}, NeedExp={needExp}, TotalExp={totalExp}");
                    return DynValue.False;
                }
                AddExpTable(level, needExp, totalExp);
                return DynValue.True;
            });
        });
    }

    public void AddExpTable(int level, int needExp, int totalExp)
    {
        if (level != 1 && (level <= 0 || needExp <= 0 || totalExp <= 0))
            return;
        lock (_sync) _table[level] = new ExpEntry(needExp, totalExp);
    }

    public int GetRequireNeedExpByLevel(int level)
    {
        lock (_sync)
        {
            if (_table.TryGetValue(level, out var entry))
                return entry.NeedExp;
            if (_table.Count > 0 && _table.Keys.Max() < level)
                return _table[_table.Keys.Max()].NeedExp;
            return 0;
        }
    }

    public int GetRequireTotalExpByLevel(int level)
    {
        lock (_sync)
        {
            if (_table.TryGetValue(level, out var entry))
                return entry.TotalExp;
            if (_table.Count > 0 && _table.Keys.Max() < level)
                return _table[_table.Keys.Max()].TotalExp;
            return 0;
        }
    }

    public byte CheckLevelUp(byte currentLevel, int totalExp, byte maxLevelLimit = 70)
    {
        if (currentLevel < 1) currentLevel = 1;
        if (totalExp < 0) totalExp = 0;

        lock (_sync)
        {
            if (_table.Count == 0) return currentLevel;

            byte resultLevel = currentLevel;
            bool foundBoundary = false;

            foreach (var kv in _table)
            {
                if (kv.Key < currentLevel) continue;
                if (totalExp < kv.Value.TotalExp)
                {
                    resultLevel = kv.Key == currentLevel ? currentLevel : (byte)(kv.Key - 1);
                    foundBoundary = true;
                    break;
                }
            }

            if (!foundBoundary)
            {
                int highestLevel = _table.Keys.Max();
                if (_table[highestLevel].TotalExp <= totalExp)
                    resultLevel = (byte)highestLevel;
            }

            return Math.Min(resultLevel, maxLevelLimit);
        }
    }

    private void SeedDefaultTable()
    {
        int total = 0;
        _table[1] = new ExpEntry(0, 0);
        for (int lvl = 2; lvl <= 70; lvl++)
        {
            int need = 800 + (lvl - 1) * (lvl - 1) * 180;
            total += need;
            _table[lvl] = new ExpEntry(need, total);
        }
    }
}
