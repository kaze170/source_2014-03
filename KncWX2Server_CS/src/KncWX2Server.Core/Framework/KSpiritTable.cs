using KncWX2Server.Core.Logging;
using KncWX2Server.Core.Scripting;
using MoonSharp.Interpreter;

namespace KncWX2Server.Core.Framework;

/// <summary>
/// Faithful C# port of GameServer/SpiritTable.{h,cpp}:
/// maps character level to daily spirit capacity and enforces global MaxSpirit cap.
/// </summary>
public sealed class KSpiritTable
{
    private readonly Dictionary<int, int> _spiritByLevel = new();
    private readonly object _sync = new();

    public int MaxSpirit { get; set; } = 100;

    public KSpiritTable()
    {
        // Standard Elsword spirit table scaling (levels 1..99, capped at 100%)
        for (int lvl = 1; lvl <= 99; lvl++)
            _spiritByLevel[lvl] = Math.Min(MaxSpirit, 50 + ((lvl - 1) / 2) * 2);
    }

    public void BindToLua(KScriptManager script, string globalName = "g_pkSpiritTable")
    {
        Table? proxy = null;
        proxy = script.CreateLuaProxy(globalName, t =>
        {
            t["SetMaxSpirit"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                int maxVal = a.Count - o >= 1 && a[o].Type == DataType.Number
                    ? KScriptManager.ArgInt(a, o)
                    : (int)(script.Lua.Globals.Get("MAX_SPIRIT").CastToNumber() ?? 0);
                if (maxVal <= 0)
                    return DynValue.False;
                MaxSpirit = maxVal;
                return DynValue.True;
            });
            t["AddSpiritTable_LUA"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                int level, spirit;
                if (a.Count - o >= 2 && a[o].Type == DataType.Number)
                {
                    level = KScriptManager.ArgInt(a, o);
                    spirit = KScriptManager.ArgInt(a, o + 1);
                }
                else
                {
                    level = (int)(script.Lua.Globals.Get("LEVEL").CastToNumber() ?? 0);
                    spirit = (int)(script.Lua.Globals.Get("SPIRIT").CastToNumber() ?? 0);
                }
                if (!AddSpiritEntry(level, spirit))
                {
                    KLog.Cerr($"KSpiritTable::AddSpiritTable_LUA read fail: Level={level}, Spirit={spirit}");
                    return DynValue.False;
                }
                return DynValue.True;
            });
        });
    }

    public bool AddSpiritEntry(int level, int spirit)
    {
        if (level <= 0 || spirit < 0) return false;
        lock (_sync)
        {
            _spiritByLevel[level] = Math.Min(MaxSpirit, spirit);
            return true;
        }
    }

    public bool TryGetSpiritByLevel(int level, out int spirit)
    {
        lock (_sync)
            return _spiritByLevel.TryGetValue(level, out spirit);
    }

    public int GetSpiritOrDefault(int level)
    {
        lock (_sync)
            return _spiritByLevel.TryGetValue(level, out int s) ? s : MaxSpirit;
    }
}
