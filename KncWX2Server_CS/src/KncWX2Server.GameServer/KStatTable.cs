using System.Collections.Concurrent;
using KncWX2Server.Core.Scripting;
using MoonSharp.Interpreter;

namespace KncWX2Server.GameServer;

/// <summary>
/// Faithful C# port of GameServer/StatTable.{h,cpp} &amp; ServerResource/TW/StatTable.lua:
/// stores per-(UnitClass, Level) base combat stats (AtkPhysic, AtkMagic, DefPhysic, DefMagic, HP).
/// </summary>
public sealed class KStatTable
{
    public sealed record UnitStatInfo(
        float AtkPhysic,
        float AtkMagic,
        float DefPhysic,
        float DefMagic,
        float Hp);

    private readonly ConcurrentDictionary<(int UnitClass, int Level), UnitStatInfo> _stats = new();

    public KStatTable()
    {
        SeedDefaultClassCurves();
    }

    private void SeedDefaultClassCurves()
    {
        // Elsword(1), Aisha(2), Rena(3), Raven(4), Eve(5), Chung(6), Ara(7), Elesis(8), Add(9)
        for (int cls = 1; cls <= 12; cls++)
        {
            float physBias = cls is 1 or 4 or 6 or 7 or 8 ? 1.15f : 0.90f;
            float magBias = cls is 2 or 5 or 9 ? 1.18f : 0.92f;
            float hpBias = cls is 1 or 6 or 8 ? 1.12f : 1.0f;

            for (int lvl = 1; lvl <= 99; lvl++)
            {
                _stats[(cls, lvl)] = new UnitStatInfo(
                    AtkPhysic: (120f + lvl * 28f) * physBias,
                    AtkMagic: (120f + lvl * 28f) * magBias,
                    DefPhysic: (40f + lvl * 11f) * physBias,
                    DefMagic: (40f + lvl * 11f) * magBias,
                    Hp: (1800f + lvl * 420f) * hpBias);
            }
        }
    }

    public void AddStatInfo(int unitClass, int level, UnitStatInfo stat)
        => _stats[(unitClass, level)] = stat;

    public UnitStatInfo GetUnitStat(int unitClass, int level)
    {
        int clampedLevel = Math.Clamp(level, 1, 99);
        if (_stats.TryGetValue((unitClass, clampedLevel), out var exact))
            return exact;
        if (_stats.TryGetValue((1, clampedLevel), out var fallback))
            return fallback;
        return new UnitStatInfo(120f + clampedLevel * 28f, 120f + clampedLevel * 28f,
            40f + clampedLevel * 11f, 40f + clampedLevel * 11f, 1800f + clampedLevel * 420f);
    }

    public void BindToLua(KScriptManager script, string globalName = "StatTable")
    {
        Table? proxy = null;
        proxy = script.CreateLuaProxy(globalName, t =>
        {
            t["ReserveMemory"] = DynValue.NewCallback((_, _) => DynValue.Nil);
            t["SetUnitStat"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                int unitClass = KScriptManager.ArgInt(a, o);
                int level = KScriptManager.ArgInt(a, o + 1);
                var tbl = KScriptManager.ArgTable(a, o + 2);
                if (unitClass > 0 && level > 0 && tbl != null)
                {
                    AddStatInfo(unitClass, level, new UnitStatInfo(
                        AtkPhysic: (float)(tbl.Get("AtkPhysic").CastToNumber() ?? 0),
                        AtkMagic: (float)(tbl.Get("AtkMagic").CastToNumber() ?? 0),
                        DefPhysic: (float)(tbl.Get("DefPhysic").CastToNumber() ?? 0),
                        DefMagic: (float)(tbl.Get("DefMagic").CastToNumber() ?? 0),
                        Hp: (float)(tbl.Get("HP").CastToNumber() ?? 0)));
                }
                return DynValue.Nil;
            });
        });
    }
}
