using KncWX2Server.Core.Scripting;
using MoonSharp.Interpreter;

namespace KncWX2Server.CenterServer;

/// <summary>
/// Exact C# port of CenterServer/PvpMatchResultTable.{h,cpp} and ServerResource/TW/PvpMatchResultTable.lua
/// (2012 PvP Season 2 Elo KFactor, ExpectOfVictory, Rating delta, Ranking Point, Arena Point, PvP EXP, and penalties).
/// </summary>
public static class KPvpMatchResultTable
{
    public enum MatchType
    {
        MT_TEAM_1_ON_1 = 0,
        MT_TEAM_2_ON_2 = 1,
        MT_TEAM_3_ON_3 = 2,
        MT_TEAM_DEATH_1_ON_1 = 3,
        MT_TEAM_DEATH_2_ON_2 = 4,
        MT_TEAM_DEATH_3_ON_3 = 5,
    }

    public const int PvpNpcTypeNone = 0;
    public const int PvpNpcTypeTutorial = 1;
    public const int PvpNpcTypeHero = 2; // PVP_NPC_TYPE["PNT_HERO_NPC"]

    // Default level -> PvP EXP table matching ServerResource/TW/PvpMatchResultTable.lua (updated by BindToLua)
    private static readonly Dictionary<int, int> PvpExpRewardByLevel = new()
    {
        [1] = 0, [2] = 74, [3] = 157, [4] = 234, [5] = 314, [6] = 404, [7] = 492, [8] = 584, [9] = 678, [10] = 789,
        [11] = 882, [12] = 1000, [13] = 1108, [14] = 1217, [15] = 1341, [16] = 1469, [17] = 1601, [18] = 1733, [19] = 1865, [20] = 2013,
        [21] = 2163, [22] = 2313, [23] = 2472, [24] = 2638, [25] = 2815, [26] = 2990, [27] = 3176, [28] = 3371, [29] = 3572, [30] = 3775,
        [31] = 3995, [32] = 4214, [33] = 4452, [34] = 4690, [35] = 4936, [36] = 5200, [37] = 5473, [38] = 5746, [39] = 6036, [40] = 6345,
        [41] = 6653, [42] = 6979, [43] = 7322, [44] = 7673, [45] = 8035, [46] = 8413, [47] = 8809, [48] = 9214, [49] = 9636, [50] = 10076,
        [51] = 10534, [52] = 11018, [53] = 11510, [54] = 12021, [55] = 12557, [56] = 13112, [57] = 13693, [58] = 14291, [59] = 14916, [60] = 15559,
        [61] = 16236, [62] = 16940, [63] = 17662, [64] = 18419, [65] = 19211, [66] = 20029, [67] = 20873, [68] = 21763, [69] = 22678, [70] = 23628,
        [71] = 24623, [72] = 25661, [73] = 26726, [74] = 27843, [75] = 29005, [76] = 30210, [77] = 31460, [78] = 32762, [79] = 34118, [80] = 35525,
    };

    public static void BindToLua(KScriptManager script, string globalName = "KPvpMatchResultTable")
    {
        Table? proxy = null;
        proxy = script.CreateLuaProxy(globalName, t =>
        {
            t["AddPvpExpRewardInfo"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                int level = KScriptManager.ArgInt(a, o, -1);
                int exp = KScriptManager.ArgInt(a, o + 1, -1);
                if (level >= 0 && exp >= 0)
                    PvpExpRewardByLevel[level] = exp;
                return DynValue.Nil;
            });
        });
    }

    public static int GetExpByLevel(int level)
        => PvpExpRewardByLevel.TryGetValue(level, out int exp) ? exp : 0;

    /// <summary>
    /// Exact port of Lua GET_K_FACTOR: returns the KFactor delta (KFactorResult - currentKFactor).
    /// </summary>
    public static float GetKFactorIncrement(
        int pvpPlayCount,
        bool isWinBeforeMatch,
        bool isWin,
        int userRating,
        float currentKFactor,
        bool isRedistributionUser = false)
    {
        float kFactorResult = currentKFactor <= 0 ? 300.0f : currentKFactor;

        if (pvpPlayCount < 10 && !isRedistributionUser)
        {
            if (pvpPlayCount > 0 && isWinBeforeMatch != isWin)
                kFactorResult *= 0.8f;
        }
        else
        {
            if (pvpPlayCount > 29 && userRating < 1851)
                kFactorResult = 32.0f;
            else if (userRating > 2350)
                kFactorResult = 12.0f;
            else if (userRating > 1850)
                kFactorResult = 24.0f;
            else if (userRating > 1350)
                kFactorResult = 36.0f;
            else if (userRating > 950)
                kFactorResult = 48.0f;
            else if (userRating >= 0)
                kFactorResult = 60.0f;
            else
                kFactorResult = 32.0f;
        }

        return kFactorResult - currentKFactor;
    }

    /// <summary>
    /// Exact port of Lua GET_EXPECT_OF_VICTORY.
    /// </summary>
    public static double GetExpectOfVictory(int myTeamRating, int enemyTeamRating)
    {
        double teamRatingGap = enemyTeamRating - myTeamRating;
        double middleResult = Math.Pow(10.0, teamRatingGap / 400.0);
        return 1.0 / (1.0 + middleResult);
    }

    /// <summary>
    /// Exact port of Lua GET_RATING_PVP_RESULT (Season 2).
    /// </summary>
    public static int GetRatingPvpResult(
        int myTeamRating,
        int enemyTeamRating,
        int userRating,
        bool isWin,
        double kFactor,
        bool arrangeTeamsMatch)
    {
        double expect = GetExpectOfVictory(myTeamRating, enemyTeamRating);
        double myTeamResultFactor = isWin ? 1.0 : 0.0;
        double ratingResult = userRating + (kFactor * (myTeamResultFactor - expect));
        double increment = ratingResult - userRating;

        if (arrangeTeamsMatch && !isWin)
            increment *= 0.5;

        return (int)Math.Round(increment);
    }

    /// <summary>
    /// Exact port of Lua GET_RANKING_POINT_PVP_RESULT.
    /// </summary>
    public static int GetRPointPvpResult(
        int myTeamRating,
        int enemyTeamRating,
        bool isWin,
        int killNum,
        int mdKillNum,
        int deathNum,
        int matchType,
        int pvpNpcType = PvpNpcTypeNone)
    {
        double expect = GetExpectOfVictory(myTeamRating, enemyTeamRating);

        int winLoseConstant = (MatchType)matchType switch
        {
            MatchType.MT_TEAM_DEATH_3_ON_3 => 16,
            MatchType.MT_TEAM_DEATH_2_ON_2 => 14,
            MatchType.MT_TEAM_DEATH_1_ON_1 => 14,
            MatchType.MT_TEAM_3_ON_3 => 14,
            MatchType.MT_TEAM_2_ON_2 => 14,
            _ => 12,
        };

        double winLoseFactor = isWin ? 1.0 : 0.0;
        double winLoseMPoint = winLoseConstant * (winLoseFactor - expect);
        double winLoseRPoint = Math.Max(winLoseMPoint, -6.0);

        int kdMiddle = killNum + mdKillNum - deathNum;
        int kdClamped = Math.Clamp(kdMiddle, -4, 12);

        double finalResult = winLoseRPoint + kdClamped;
        if (pvpNpcType == PvpNpcTypeHero)
            finalResult *= 2.0;

        return (int)Math.Round(finalResult);
    }

    public static int GetAPointPvpResult(bool isWin, int killNum, int mdKillNum, int matchType)
        => GetAPointPvpResult(1000, isWin, killNum, mdKillNum, matchType, PvpNpcTypeNone);

    /// <summary>
    /// Exact port of Lua GET_ARENA_POINT_PVP_RESULT.
    /// </summary>
    public static int GetAPointPvpResult(
        int userRating,
        bool isWin,
        int killNum,
        int mdKillNum,
        int matchType,
        int pvpNpcType = PvpNpcTypeNone)
    {
        double winLoseAPoint = isWin
            ? (MatchType)matchType switch
            {
                MatchType.MT_TEAM_DEATH_3_ON_3 => 8.0,
                MatchType.MT_TEAM_DEATH_2_ON_2 => 6.0,
                MatchType.MT_TEAM_DEATH_1_ON_1 => 4.0,
                MatchType.MT_TEAM_3_ON_3 => 5.0,
                MatchType.MT_TEAM_2_ON_2 => 5.0,
                _ => 2.0,
            }
            : (MatchType)matchType switch
            {
                MatchType.MT_TEAM_DEATH_3_ON_3 => 2.0,
                MatchType.MT_TEAM_DEATH_2_ON_2 => 1.5,
                MatchType.MT_TEAM_DEATH_1_ON_1 => 1.0,
                MatchType.MT_TEAM_3_ON_3 => 1.25,
                MatchType.MT_TEAM_2_ON_2 => 1.25,
                _ => 0.5,
            };

        int maxKillDeath = (MatchType)matchType switch
        {
            MatchType.MT_TEAM_DEATH_3_ON_3 => 10,
            MatchType.MT_TEAM_DEATH_2_ON_2 => 10,
            MatchType.MT_TEAM_DEATH_1_ON_1 => 3,
            MatchType.MT_TEAM_3_ON_3 => 10,
            MatchType.MT_TEAM_2_ON_2 => 10,
            _ => 0,
        };

        int killDeathAPoint = Math.Min(maxKillDeath, killNum + mdKillNum);
        double ratingAPoint = Math.Floor(userRating * 0.01) + 10.0;
        int finalResult = (int)Math.Floor(((winLoseAPoint + killDeathAPoint) * ratingAPoint) + 0.5);

        if (pvpNpcType == PvpNpcTypeHero)
            finalResult *= 10;

        return finalResult;
    }

    public static int GetPvpExpReward(int level, bool isWin, int killNum)
        => GetPvpExpReward(level, isWin, killNum, 0, (int)MatchType.MT_TEAM_1_ON_1, PvpNpcTypeNone);

    /// <summary>
    /// Exact port of Lua GET_EXP_PVP_RESULT using m_mapPvpExpReward (AddPvpExpRewardInfo).
    /// </summary>
    public static int GetPvpExpReward(
        int level,
        bool isWin,
        int killNum,
        int mdKillNum,
        int matchType,
        int pvpNpcType = PvpNpcTypeNone)
    {
        double expByLevel = GetExpByLevel(level);
        if (expByLevel <= 0)
            expByLevel = Math.Max(74, level * 120);

        double winLoseFactor = isWin ? 1.0 : 0.2;
        double killDeathExp = 0.2 * Math.Min(killNum + mdKillNum, 3);
        double matchTypeBonus = (MatchType)matchType switch
        {
            MatchType.MT_TEAM_DEATH_3_ON_3 => 2.0,
            MatchType.MT_TEAM_DEATH_2_ON_2 => 2.0,
            MatchType.MT_TEAM_DEATH_1_ON_1 => 1.5,
            MatchType.MT_TEAM_3_ON_3 => 1.25,
            MatchType.MT_TEAM_2_ON_2 => 1.1,
            _ => 1.0,
        };

        _ = pvpNpcType;
        return (int)(expByLevel * (winLoseFactor + killDeathExp) * matchTypeBonus);
    }

    /// <summary>
    /// Exact port of Lua GET_RANKING_POINT_PVP_PENALTY.
    /// </summary>
    public static int GetRPointPvpPenalty() => -60;

    /// <summary>
    /// Exact port of Lua GET_ARENA_POINT_PVP_PENALTY.
    /// </summary>
    public static int GetAPointPvpPenalty(int userRating, int matchType = 0, int pvpNpcType = PvpNpcTypeNone)
        => -(GetAPointPvpResult(userRating, isWin: true, killNum: 0, mdKillNum: 0, matchType: matchType, pvpNpcType: pvpNpcType) * 5);
}
