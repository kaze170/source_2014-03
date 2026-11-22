using KncWX2Server.Core.Scripting;
using MoonSharp.Interpreter;

namespace KncWX2Server.CenterServer;

/// <summary>
/// Faithful C# port of CenterServer/CnPartyFeverManager.{h,cpp} &amp; ServerResource/TW/PartyServerData.lua:
/// exact Fever accumulation, member-count scaling, BattleField factor, 4800 activation -> 4000 cap,
/// and 800-point member-leave penalty.
/// </summary>
public sealed class KCnPartyFeverManager
{
    public const short FeverDecreasePointByLeaveParty = 800;
    public const short Fever100PercentPoint = 4000;
    public const short ActivateFeverPoint = 4800;

    public static short ConfiguredActivateFeverPoint { get; private set; } = ActivateFeverPoint;
    public static float DungeonSecretStageEnterFactor { get; private set; } = 1.0f;
    public static Dictionary<int, short> MaxFeverByMemberCount { get; } = new() { [2] = 2000, [3] = 3000, [4] = Fever100PercentPoint };
    public static Dictionary<int, (int Min, int Max)> IncreaseFeverRangeByMemberCount { get; } = new();

    public enum UpdateFeverPointResult
    {
        UfpNone = 0,
        UfpActiveFever = 1,
        UfpChangeFever = 2,
    }

    public short RealFeverPoint { get; private set; }

    public void Reset() => RealFeverPoint = 0;

    public void OnMemberLeaveParty()
        => RealFeverPoint = (short)Math.Max(0, RealFeverPoint - FeverDecreasePointByLeaveParty);

    public short GetMaxFeverPointByNumMember(int feverPoint, int numMember)
    {
        if (numMember <= 1) return 0;
        int cap = MaxFeverByMemberCount.TryGetValue(numMember, out short configured)
            ? configured
            : numMember switch
            {
                2 => 2000,
                3 => 3000,
                _ => Fever100PercentPoint,
            };
        return (short)Math.Min(feverPoint, cap);
    }

    public (UpdateFeverPointResult result, int displayFeverPoint) UpdateFeverPoint(int newFeverPoint, int numMember)
    {
        if (numMember <= 1)
            return (UpdateFeverPointResult.UfpNone, 0);

        RealFeverPoint = (short)Math.Max(0, newFeverPoint);

        if (RealFeverPoint >= ConfiguredActivateFeverPoint)
        {
            RealFeverPoint = GetMaxFeverPointByNumMember(RealFeverPoint, numMember);
            return (UpdateFeverPointResult.UfpActiveFever, RealFeverPoint);
        }

        int display = Math.Min((int)RealFeverPoint, Fever100PercentPoint);
        return (UpdateFeverPointResult.UfpChangeFever, display);
    }

    public int CalcIncreaseFeverPoint(
        int numMember,
        bool isBattleField,
        short subStageNpcCount,
        bool hasComeBackUserInParty,
        float battleFieldExpFactor = 1.0f)
    {
        if (subStageNpcCount <= 0 || numMember <= 1)
            return 0;

        int increase = IncreaseFeverRangeByMemberCount.TryGetValue(numMember, out var range) && range.Max >= range.Min
            ? Random.Shared.Next(range.Min, range.Max + 1)
            : numMember * 30;
        if (hasComeBackUserInParty)
            increase += (int)(increase * 0.5f);

        if (isBattleField)
        {
            float scaled = increase * Math.Max(0f, battleFieldExpFactor);
            if (scaled > 0f && scaled < 1f) scaled = 1f;
            increase = (int)scaled;
        }

        return increase / subStageNpcCount;
    }

    public static void BindToLua(KScriptManager script, string globalName = "g_pPartyManager")
    {
        Table? proxy = null;
        proxy = script.CreateLuaProxy(globalName, t =>
        {
            t["SetActivateFeverPoint"] = DynValue.NewCallback((_, a) =>
            {
                ConfiguredActivateFeverPoint = (short)KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, proxy!), ActivateFeverPoint);
                return DynValue.Nil;
            });
            t["SetMaxFeverPointByNumMember"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                MaxFeverByMemberCount[KScriptManager.ArgInt(a, o)] = (short)KScriptManager.ArgInt(a, o + 1, Fever100PercentPoint);
                return DynValue.Nil;
            });
            t["SetIncreasePartyFeverByNumMember"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                IncreaseFeverRangeByMemberCount[KScriptManager.ArgInt(a, o)] = (KScriptManager.ArgInt(a, o + 1), KScriptManager.ArgInt(a, o + 2));
                return DynValue.Nil;
            });
            t["SetDungeonSecretStageEnterFactor"] = DynValue.NewCallback((_, a) =>
            {
                DungeonSecretStageEnterFactor = KScriptManager.ArgFloat(a, KScriptManager.ArgStart(a, proxy!), 1.0f);
                return DynValue.Nil;
            });
        });
    }
}
