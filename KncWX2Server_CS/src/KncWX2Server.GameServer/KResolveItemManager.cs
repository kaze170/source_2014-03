using KncWX2Server.Core.Framework;
using KncWX2Server.Core.Scripting;
using MoonSharp.Interpreter;

namespace KncWX2Server.GameServer;

/// <summary>
/// Faithful C# port of ServerResource/TW/ResolveTable.lua &amp; Common/X2Data/XSLResolveItemManager:
/// resolves equipment into El Flour (99825..99829/99976), Weapon/Armor broken pieces,
/// EnchantStone rules (AddResolveData), Fashion cash avatar shards, and server-wide jackpot windows.
/// </summary>
public sealed class KResolveItemManager
{
    public const int DimElPiece = 99825;
    public const int MinuteElPiece = 99826;
    public const int SmallElPiece = 99827;
    public const int NormalElPiece = 99828;
    public const int BigElPiece = 99829;
    public const int ShiningElPiece = 99976;

    public const int RustyWeaponPiece = 99799;
    public const int WornArmorPiece = 99800;
    public const int BluntWeaponPiece = 99801;
    public const int ThinArmorPiece = 99802;
    public const int NormalWeaponPiece = 99803;
    public const int PatchedArmorPiece = 99804;
    public const int PolishedWeaponPiece = 99805;
    public const int DependableArmorPiece = 99806;
    public const int SolidWeaponPiece = 99807;
    public const int SturdyArmorPiece = 99808;
    public const int ExquisiteWeaponPiece = 99980;
    public const int StrongArmorPiece = 99982;

    public int MinJackpotTimeSec { get; private set; } = 10800;
    public int MaxJackpotTimeSec { get; private set; } = 21600;
    public float JackpotProb { get; private set; } = 100f;
    public DateTime NextJackpotReadyUtc { get; private set; } = DateTime.UtcNow;

    public readonly record struct ResolveDataRule(
        int ItemGrade,
        int ResultItemId,
        double SuccRate,
        int RandomCount,
        int ResultCount,
        int BeginLv,
        int EndLv);

    public readonly record struct BrokenPieceRule(
        int ResType,
        int BeginLv,
        int EndLv,
        int UnitCost,
        int ResultItemId);

    private readonly KLottery _weaponBrokenPieceRate = new();
    private readonly KLottery _armorBrokenPieceRate = new();
    private readonly Dictionary<(int resType, int beginLv, int endLv), BrokenPieceRule> _brokenPieceTiers = new();
    private readonly List<ResolveDataRule> _resolveDataRules = [];
    private readonly Random _rng = new();
    private bool _hasLuaWeaponRate;
    private bool _hasLuaArmorRate;

    public IReadOnlyList<ResolveDataRule> ResolveDataRules => _resolveDataRules;

    public KResolveItemManager()
    {
        _weaponBrokenPieceRate.AddCase(1, 100);
        _armorBrokenPieceRate.AddCase(1, 30);
        _armorBrokenPieceRate.AddCase(2, 70);
    }

    public void SetMinMaxOfJackpotTime(int minSec, int maxSec)
    {
        MinJackpotTimeSec = Math.Max(0, minSec);
        MaxJackpotTimeSec = Math.Max(MinJackpotTimeSec, maxSec);
    }

    public void SetJackpotProb(float prob) => JackpotProb = Math.Clamp(prob, 0f, 100f);

    public static bool ResolveFashionItem(int cashPrice, out Dictionary<int, int> outputItems, out int resolveQuantity)
    {
        outputItems = new Dictionary<int, int>();
        resolveQuantity = 0;
        if (cashPrice <= 0) return false;
        resolveQuantity = Math.Max(1, cashPrice / 200);
        outputItems[KSocketItemManager.AtiUnknown] = resolveQuantity;
        return true;
    }

    public (Dictionary<int, int> outputItems, bool isJackpot) ResolveItem(
        int eqipLevel, bool isWeapon, DateTime? nowUtc = null, double? deterministicRoll = null, int itemGrade = 0, int sellPrice = 0)
    {
        var now = nowUtc ?? DateTime.UtcNow;
        var result = new Dictionary<int, int>();

        int tier = Math.Clamp((eqipLevel - 1) / 12, 0, 5);
        var elRule = FindBrokenPieceRule(0, eqipLevel);
        int elId = elRule?.ResultItemId ?? (tier switch
        {
            0 => DimElPiece,
            1 => MinuteElPiece,
            2 => SmallElPiece,
            3 => NormalElPiece,
            4 => BigElPiece,
            _ => ShiningElPiece,
        });
        int elCount = sellPrice > 0 && elRule is { UnitCost: > 0 }
            ? Math.Max(1, sellPrice / elRule.Value.UnitCost)
            : Math.Max(1, tier + 1);
        result[elId] = elCount;

        int pieceType;
        if (isWeapon)
        {
            int weaponOrArmor = _weaponBrokenPieceRate.Decision(deterministicRoll);
            pieceType = weaponOrArmor == 2 ? 2 : 1;
        }
        else
        {
            int armorOrWeapon = _armorBrokenPieceRate.Decision(deterministicRoll);
            pieceType = armorOrWeapon == 1 ? 1 : 2;
        }

        var pieceRule = FindBrokenPieceRule(pieceType, eqipLevel);
        int pieceId = pieceRule?.ResultItemId ?? (pieceType == 2 ? DefaultArmorPiece(tier) : DefaultWeaponPiece(tier));
        int pieceCount = sellPrice > 0 && pieceRule is { UnitCost: > 0 }
            ? Math.Max(1, sellPrice / pieceRule.Value.UnitCost)
            : Math.Max(1, tier + 1);
        result[pieceId] = pieceCount;

        bool jackpot = false;
        if (now >= NextJackpotReadyUtc)
        {
            double roll = (deterministicRoll ?? _rng.NextDouble()) * 100.0;
            if (roll < JackpotProb)
            {
                jackpot = true;
                result[elId] += 5;
                int span = Math.Max(1, MaxJackpotTimeSec - MinJackpotTimeSec);
                int nextSec = MinJackpotTimeSec + _rng.Next(span);
                NextJackpotReadyUtc = now.AddSeconds(nextSec);
            }
        }

        // Exact C++ ResolveResult_EnchantStone loop for Lua-loaded AddResolveData rules
        foreach (var rule in _resolveDataRules)
        {
            if (itemGrade > 0 && rule.ItemGrade != itemGrade)
                continue;
            if (eqipLevel < rule.BeginLv || eqipLevel > rule.EndLv)
                continue;
            if (rule.RandomCount <= 0 || rule.ResultCount <= 0)
                continue;

            for (int c = 0; c < rule.RandomCount; c++)
            {
                bool hit = jackpot || ((deterministicRoll ?? _rng.NextDouble()) * 100.0 < rule.SuccRate);
                if (!hit) continue;
                if (!result.TryGetValue(rule.ResultItemId, out int cur))
                    result[rule.ResultItemId] = jackpot ? rule.ResultCount * 2 : rule.ResultCount;
                else
                    result[rule.ResultItemId] = cur + rule.ResultCount;
            }
        }

        return (result, jackpot);
    }

    private BrokenPieceRule? FindBrokenPieceRule(int resType, int level)
    {
        foreach (var kv in _brokenPieceTiers)
        {
            if (kv.Key.resType == resType && level >= kv.Key.beginLv && level <= kv.Key.endLv)
                return kv.Value;
        }
        return null;
    }

    private static int DefaultWeaponPiece(int tier) => tier switch
    {
        0 => RustyWeaponPiece,
        1 => BluntWeaponPiece,
        2 => NormalWeaponPiece,
        3 => PolishedWeaponPiece,
        4 => SolidWeaponPiece,
        _ => ExquisiteWeaponPiece,
    };

    private static int DefaultArmorPiece(int tier) => tier switch
    {
        0 => WornArmorPiece,
        1 => ThinArmorPiece,
        2 => PatchedArmorPiece,
        3 => DependableArmorPiece,
        4 => SturdyArmorPiece,
        _ => StrongArmorPiece,
    };

    public void BindToLua(KScriptManager script, string globalName = "g_pResolveItemManager")
    {
        _hasLuaWeaponRate = false;
        _hasLuaArmorRate = false;
        Table? proxy = null;
        proxy = script.CreateLuaProxy(globalName, t =>
        {
            t["SetJackpotProb"] = DynValue.NewCallback((_, a) =>
            {
                SetJackpotProb(KScriptManager.ArgFloat(a, KScriptManager.ArgStart(a, proxy!), 100f));
                return DynValue.Nil;
            });
            t["SetMinMaxOfJackpotTime"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                SetMinMaxOfJackpotTime(KScriptManager.ArgInt(a, o), KScriptManager.ArgInt(a, o + 1));
                return DynValue.Nil;
            });
            t["AddWeaponResolveBrokenPieceRate"] = DynValue.NewCallback((_, a) =>
            {
                if (!_hasLuaWeaponRate)
                {
                    _weaponBrokenPieceRate.Clear();
                    _hasLuaWeaponRate = true;
                }
                int o = KScriptManager.ArgStart(a, proxy!);
                _weaponBrokenPieceRate.AddCase(KScriptManager.ArgInt(a, o), KScriptManager.ArgFloat(a, o + 1));
                return DynValue.Nil;
            });
            t["AddArmorResolveBrokenPieceRate"] = DynValue.NewCallback((_, a) =>
            {
                if (!_hasLuaArmorRate)
                {
                    _armorBrokenPieceRate.Clear();
                    _hasLuaArmorRate = true;
                }
                int o = KScriptManager.ArgStart(a, proxy!);
                _armorBrokenPieceRate.AddCase(KScriptManager.ArgInt(a, o), KScriptManager.ArgFloat(a, o + 1));
                return DynValue.Nil;
            });
            t["AddResolveData"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                int itemGrade = KScriptManager.ArgInt(a, o);
                int itemId = KScriptManager.ArgInt(a, o + 1);
                var tbl = KScriptManager.ArgTable(a, o + 2);
                if (itemId > 0 && tbl != null)
                {
                    double succRate = tbl.Get("fSuccRate").CastToNumber() ?? 0.0;
                    int randCnt = (int)(tbl.Get("iRandomCnt").CastToNumber() ?? 0);
                    int resCnt = (int)(tbl.Get("iResultCnt").CastToNumber() ?? 0);
                    int b = (int)(tbl.Get("iBeginLv").CastToNumber() ?? 0);
                    int e = (int)(tbl.Get("iEndLv").CastToNumber() ?? 100);
                    _resolveDataRules.Add(new ResolveDataRule(itemGrade, itemId, succRate, randCnt, resCnt, b, e));
                }
                return DynValue.Nil;
            });
            t["AddResolveDataBrokenPiece"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                int resType = KScriptManager.ArgInt(a, o);
                int itemId = KScriptManager.ArgInt(a, o + 1);
                var tbl = KScriptManager.ArgTable(a, o + 2);
                if (itemId > 0 && tbl != null)
                {
                    int unitCost = (int)(tbl.Get("iUnitCost").CastToNumber() ?? 0);
                    int b = (int)(tbl.Get("iBeginLv").CastToNumber() ?? 0);
                    int e = (int)(tbl.Get("iEndLv").CastToNumber() ?? 100);
                    _brokenPieceTiers[(resType, b, e)] = new BrokenPieceRule(resType, b, e, unitCost, itemId);
                }
                return DynValue.Nil;
            });
        });
    }
}
