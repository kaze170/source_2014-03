using KncWX2Server.Core.Framework;
using KncWX2Server.Core.Packets;
using KncWX2Server.Core.Scripting;
using MoonSharp.Interpreter;

namespace KncWX2Server.CenterServer;

/// <summary>
/// Faithful C# port of CenterServer/ResultProcess.{h,cpp} and ServerResource/TW/ResultData_new.lua:
/// computes PvP win-team resolution, Season-2 Elo/RPoint/APoint/EXP/MVP rewards, and
/// exact Dungeon ranks (SS..F), monster/dungeon weights, rank bonus caps, and clear EXP/ED rewards.
/// </summary>
public static class KResultProcess
{
    public enum RankType : sbyte
    {
        RT_NONE = 0,
        RT_F = 1,
        RT_E = 2,
        RT_D = 3,
        RT_C = 4,
        RT_B = 5,
        RT_A = 6,
        RT_S = 7,
        RT_SS = 8,
    }

    public const int TeamNone = -1;
    public const int TeamRed = 0;
    public const int TeamBlue = 1;

    // Exact values from ResultData_new.lua
    private static readonly float[] DungeonPartyBonus = [0f, 0.5f, 1.0f, 1.5f];        // 1..4 members
    private static readonly float[] DungeonDifficultyBonus = [1.0f, 2.0f, 2.25f];     // Normal, Hard, VeryHard
    private static readonly float[] DungeonModeBonus = [2.25f, 2.25f, 2.25f];         // Normal, Henir, Secret

    public static void BindToLua(KScriptManager script, string globalName = "ResultProcess")
    {
        Table? proxy = null;
        proxy = script.CreateLuaProxy(globalName, t =>
        {
            t["AddDungeonPartyBonus"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                DungeonPartyBonus[1] = KScriptManager.ArgFloat(a, o, DungeonPartyBonus[1]);
                DungeonPartyBonus[2] = KScriptManager.ArgFloat(a, o + 1, DungeonPartyBonus[2]);
                DungeonPartyBonus[3] = KScriptManager.ArgFloat(a, o + 2, DungeonPartyBonus[3]);
                return DynValue.Nil;
            });
            t["AddDungeonPartyDifficultyBonus"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                DungeonDifficultyBonus[0] = KScriptManager.ArgFloat(a, o, DungeonDifficultyBonus[0]);
                DungeonDifficultyBonus[1] = KScriptManager.ArgFloat(a, o + 1, DungeonDifficultyBonus[1]);
                DungeonDifficultyBonus[2] = KScriptManager.ArgFloat(a, o + 2, DungeonDifficultyBonus[2]);
                return DynValue.Nil;
            });
            t["AddDungeonModePartyBonus"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                DungeonModeBonus[0] = KScriptManager.ArgFloat(a, o, DungeonModeBonus[0]);
                DungeonModeBonus[1] = KScriptManager.ArgFloat(a, o + 1, DungeonModeBonus[1]);
                DungeonModeBonus[2] = KScriptManager.ArgFloat(a, o + 2, DungeonModeBonus[2]);
                return DynValue.Nil;
            });
        });
    }

    public static int DecidePvpWinTeam(IReadOnlyList<KCnRoom.SlotCombatStats> activeStats)
    {
        int redKills = activeStats.Where(s => s.TeamNum == TeamRed).Sum(s => s.KillNum);
        int blueKills = activeStats.Where(s => s.TeamNum == TeamBlue).Sum(s => s.KillNum);
        if (redKills > blueKills) return TeamRed;
        if (blueKills > redKills) return TeamBlue;

        int redDeaths = activeStats.Where(s => s.TeamNum == TeamRed).Sum(s => s.DeathNum);
        int blueDeaths = activeStats.Where(s => s.TeamNum == TeamBlue).Sum(s => s.DeathNum);
        if (redDeaths < blueDeaths) return TeamRed;
        if (blueDeaths < redDeaths) return TeamBlue;

        return TeamNone;
    }

    // Exact KRoomUserInfo::BONUS_TYPE ordinals from CommonPacket.h
    private const int BtPremiumExpRate = 0;
    private const int BtChannelBonusExpRate = 2;
    private const int BtGevExp = 5;
    private const int BtGevVp = 6;
    private const int BtGevEd = 7;
    private const int BtGevPartyExp = 8;
    private const int BtGevWithCharExp = 11;

    public static KEGS_END_GAME_PVP_RESULT_DATA_NOT BuildPvpResult(
        KCnRoom room,
        int playTimeSec,
        bool abnormalEnd = false,
        KGameSysValTable? sysVal = null)
    {
        var activeSlots = room.SnapshotActiveCombatStats();
        int winTeam = DecidePvpWinTeam(activeSlots);
        bool isDrawn = winTeam == TeamNone || abnormalEnd;

        var redRatings = activeSlots.Where(s => s.TeamNum == TeamRed).Select(s => s.Rating).ToList();
        var blueRatings = activeSlots.Where(s => s.TeamNum == TeamBlue).Select(s => s.Rating).ToList();
        int redTeamRating = redRatings.Count > 0 ? (int)redRatings.Average() : 1000;
        int blueTeamRating = blueRatings.Count > 0 ? (int)blueRatings.Average() : 1000;

        bool arrangeMatch = activeSlots.Any(s => s.PvpPlayCount < 10);

        var sorted = activeSlots
            .OrderByDescending(s => s.TeamNum == winTeam ? 1 : 0)
            .ThenByDescending(s => s.KillNum * 2 + s.MDKillNum - s.DeathNum)
            .ToList();

        var result = new KEGS_END_GAME_PVP_RESULT_DATA_NOT
        {
            WinTeam = (sbyte)(isDrawn ? -1 : winTeam),
            IsDrawn = isDrawn,
            IsAbnormalEndPlay = abnormalEnd,
            PlayTime = Math.Max(1, playTimeSec),
            GameType = room.Info.PvpGameType,
            OfficialMatch = room.Info.PvpChannelClass >= 2,
            ChannelId = 1,
        };

        for (int i = 0; i < sorted.Count; i++)
        {
            var s = sorted[i];
            bool isWin = !isDrawn && s.TeamNum == winTeam;
            int myTeamRating = s.TeamNum == TeamRed ? redTeamRating : blueTeamRating;
            int enemyTeamRating = s.TeamNum == TeamRed ? blueTeamRating : redTeamRating;

            float kDelta = KPvpMatchResultTable.GetKFactorIncrement(
                s.PvpPlayCount, s.WasWinBeforeMatch, isWin, s.Rating, s.CurrentKFactor);
            float newK = Math.Max(12.0f, s.CurrentKFactor + kDelta);

            int deltaRating = result.OfficialMatch && !abnormalEnd
                ? KPvpMatchResultTable.GetRatingPvpResult(myTeamRating, enemyTeamRating, s.Rating, isWin, newK, arrangeMatch)
                : 0;
            int deltaRPoint = result.OfficialMatch && !abnormalEnd
                ? KPvpMatchResultTable.GetRPointPvpResult(myTeamRating, enemyTeamRating, isWin, s.KillNum, s.MDKillNum, s.DeathNum, room.Info.PvpGameType)
                : 0;
            int deltaAPoint = abnormalEnd
                ? 0
                : KPvpMatchResultTable.GetAPointPvpResult(s.Rating, isWin, s.KillNum, s.MDKillNum, room.Info.PvpGameType);
            int exp = abnormalEnd
                ? 0
                : KPvpMatchResultTable.GetPvpExpReward(s.Level, isWin, s.KillNum, s.MDKillNum, room.Info.PvpGameType);

            if (!abnormalEnd)
            {
                int baseAp = deltaAPoint;
                int baseExp = exp;
                if (s.IsGameBang && sysVal != null)
                {
                    deltaAPoint += (int)(baseAp * sysVal.GetGbVpRate(s.PcBangType));
                    exp += (int)(baseExp * sysVal.GetGbExpRate(s.PcBangType));
                }
                if (s.BonusRate.TryGetValue(BtGevVp, out float evVp) && evVp > 0f)
                    deltaAPoint += (int)(baseAp * evVp);
                if (s.BonusRate.TryGetValue(BtGevExp, out float evExp) && evExp > 0f)
                    exp += (int)(baseExp * evExp);
            }

            result.PvpUnitInfo.Add(new KPVPUnitResultInfo
            {
                UnitUid = s.UnitUid,
                Ranking = i + 1,
                KillNum = s.KillNum,
                MDKillNum = s.MDKillNum,
                DeathNum = s.DeathNum,
                Rating = deltaRating,
                RPoint = deltaRPoint,
                APoint = deltaAPoint,
                KFactorIncrement = kDelta,
                Exp = exp,
                IsMvp = i == 0 && !isDrawn,
                Win = isWin,
            });
        }

        return result;
    }

    /// <summary>
    /// Exact port of ResultData_new.lua WEIGHT_BY_DUNGEONID(nDungeonID).
    /// </summary>
    public static double WeightByDungeonId(int dungeonComboId) => dungeonComboId switch
    {
        38100 or 38101 or 38102 or 38103 or 38104 or 38105 or 30400 => 9.0, // Henir's Time and Space
        33030 or 33040 or 33050 => 1.5, // Secret Dungeon Common
        33031 or 33041 or 33051 => 3.0, // Secret Dungeon Hell
        _ => 1.0,
    };

    /// <summary>
    /// Computes ResultData_new.lua monster weights for time, combo, and technical ranks.
    /// </summary>
    public static (double TimeWeight, double ComboWeight, double TechWeight) ComputeMonsterWeights(
        KRoomMonsterManager? monsterManager,
        int fallbackKills = 0)
    {
        if (monsterManager == null)
        {
            double fb = Math.Max(1, fallbackKills);
            return (fb, fb * 30.0, fb);
        }

        int normal = monsterManager.NormalNpcDieCount;
        int lowElite = monsterManager.LowEliteNpcDieCount;
        int highElite = monsterManager.HighEliteNpcDieCount;
        int midBoss = monsterManager.MiddleBossDieCount;
        int boss = monsterManager.BossDieCount;

        double timeW = normal * 1.0 + lowElite * 2.0 + highElite * 3.0 + midBoss * 4.0 + boss * 12.0;
        double comboW = normal * 30.0 + lowElite * 60.0 + highElite * 90.0 + midBoss * 120.0 + boss * 360.0;
        double techW = timeW;

        if (timeW <= 0)
        {
            double fb = Math.Max(1, fallbackKills);
            timeW = fb;
            comboW = fb * 30.0;
            techW = fb;
        }

        return (timeW, comboW, techW);
    }

    /// <summary>
    /// Exact port of ResultData_new.lua DUNGEON_TIME_RESULT.
    /// </summary>
    public static (RankType Rank, float BonusPoint) ComputeDungeonTimeResult(
        int dungeonComboId,
        KRoomMonsterManager? monsterManager,
        int clearTimeSec,
        int fallbackKills = 0)
    {
        double fConstDungeon = WeightByDungeonId(dungeonComboId);
        var (timeWeight, _, _) = ComputeMonsterWeights(monsterManager, fallbackKills);
        double fResult = (Math.Max(1, clearTimeSec) / timeWeight) * fConstDungeon;

        return fResult switch
        {
            <= 3.5 => (RankType.RT_SS, 0.03f),
            <= 4.2 => (RankType.RT_S, 0.02f),
            <= 5.0 => (RankType.RT_A, 0.01f),
            <= 6.0 => (RankType.RT_B, 0f),
            <= 10.0 => (RankType.RT_C, 0f),
            _ => (RankType.RT_F, 0f),
        };
    }

    /// <summary>
    /// Exact port of ResultData_new.lua DUNGEON_DAMAGE_RESULT.
    /// </summary>
    public static (RankType Rank, float BonusPoint) ComputeDungeonDamageResult(
        int totalAttackedDamage,
        int totalRecoveryHp,
        int baseHp)
    {
        double denom = Math.Max(1, baseHp);
        double fDamagedResult = ((double)(totalAttackedDamage - totalRecoveryHp) / denom) * 100.0;

        return fDamagedResult switch
        {
            <= 20.0 => (RankType.RT_SS, 0.03f),
            <= 50.0 => (RankType.RT_S, 0.02f),
            <= 90.0 => (RankType.RT_A, 0.01f),
            <= 150.0 => (RankType.RT_B, 0f),
            <= 900.0 => (RankType.RT_C, 0f),
            _ => (RankType.RT_F, 0f),
        };
    }

    /// <summary>
    /// Exact port of ResultData_new.lua DUNGEON_COMBO_RESULT.
    /// </summary>
    public static (RankType Rank, float BonusPoint) ComputeDungeonComboResult(
        int comboScore,
        int dungeonComboId,
        KRoomMonsterManager? monsterManager,
        int endNumMember,
        int fallbackKills = 0)
    {
        double fConstDungeon = WeightByDungeonId(dungeonComboId);
        var (_, comboWeight, _) = ComputeMonsterWeights(monsterManager, fallbackKills);
        double fComboResult = (comboScore / (comboWeight * fConstDungeon)) * Math.Sqrt(Math.Max(1, endNumMember));

        return fComboResult switch
        {
            > 65.0 => (RankType.RT_SS, 0.05f),
            >= 45.0 => (RankType.RT_S, 0.03f),
            >= 30.0 => (RankType.RT_A, 0.01f),
            >= 15.0 => (RankType.RT_B, 0f),
            >= 2.0 => (RankType.RT_C, 0f),
            _ => (RankType.RT_F, 0f),
        };
    }

    /// <summary>
    /// Exact port of ResultData_new.lua DUNGEON_TECHNICAL_RESULT.
    /// </summary>
    public static (RankType Rank, float BonusPoint) ComputeDungeonTechnicalResult(
        int techScore,
        int usingKindOfSkill,
        int dungeonComboId,
        KRoomMonsterManager? monsterManager,
        int endNumMember,
        int fallbackKills = 0)
    {
        double fConstDungeon = WeightByDungeonId(dungeonComboId);
        var (_, _, techWeight) = ComputeMonsterWeights(monsterManager, fallbackKills);
        double nSkillBonus = usingKindOfSkill switch
        {
            > 7 => 1.21,
            > 6 => 1.18,
            > 5 => 1.15,
            > 4 => 1.12,
            > 3 => 1.08,
            > 2 => 1.02,
            _ => 1.0,
        };

        double fTechResult = ((techScore * nSkillBonus) / (techWeight * fConstDungeon)) * Math.Sqrt(Math.Max(1, endNumMember));

        return fTechResult switch
        {
            > 750.0 => (RankType.RT_SS, 0.05f),
            >= 500.0 => (RankType.RT_S, 0.03f),
            >= 350.0 => (RankType.RT_A, 0.01f),
            >= 250.0 => (RankType.RT_B, 0f),
            >= 50.0 => (RankType.RT_C, 0f),
            _ => (RankType.RT_F, 0f),
        };
    }

    /// <summary>
    /// Exact port of ResultData_new.lua SUB_STAGE_COMBO_RESULT.
    /// </summary>
    public static RankType SubStageComboResult(int dungeonComboId, int endNumMember, int comboScore, double monsterWghtCombo)
    {
        double fConstDungeon = WeightByDungeonId(dungeonComboId);
        double denom = Math.Max(1.0, monsterWghtCombo) * fConstDungeon;
        double fComboResult = (comboScore / denom) * Math.Sqrt(Math.Max(1, endNumMember));

        return fComboResult switch
        {
            > 65.0 => RankType.RT_SS,
            >= 45.0 => RankType.RT_S,
            >= 30.0 => RankType.RT_A,
            >= 15.0 => RankType.RT_B,
            >= 2.0 => RankType.RT_C,
            _ => RankType.RT_F,
        };
    }

    /// <summary>
    /// Exact port of ResultData_new.lua SUB_STAGE_TECHNICAL_RESULT.
    /// </summary>
    public static RankType SubStageTechnicalResult(int dungeonComboId, int endNumMember, int techScore, double monsterWghtTech)
    {
        double fConstDungeon = WeightByDungeonId(dungeonComboId);
        double denom = Math.Max(1.0, monsterWghtTech) * fConstDungeon;
        double fTechResult = (techScore / denom) * Math.Sqrt(Math.Max(1, endNumMember));

        return fTechResult switch
        {
            > 750.0 => RankType.RT_SS,
            >= 500.0 => RankType.RT_S,
            >= 350.0 => RankType.RT_A,
            >= 250.0 => RankType.RT_B,
            >= 50.0 => RankType.RT_C,
            _ => RankType.RT_F,
        };
    }

    public static KEGS_END_GAME_DUNGEON_RESULT_DATA_NOT BuildDungeonResult(
        KCnRoom room,
        int playTimeSec,
        bool isWin,
        KGameSysValTable? sysVal = null)
    {
        var activeSlots = room.SnapshotActiveCombatStats();
        int endNumMember = Math.Max(1, activeSlots.Count);
        int partyCount = Math.Clamp(activeSlots.Count, 1, 4);
        int diffIdx = Math.Clamp((int)room.Info.DifficultyLevel, 0, 2);
        int modeIdx = Math.Clamp((int)room.Info.DungeonMode, 0, 2);
        bool isHenir = room.Info.DungeonId == 38100 || room.Info.DungeonMode is 1 or 2;
        int dungeonComboId = room.Info.DungeonId + room.Info.DifficultyLevel;

        float partyBonusRate = DungeonPartyBonus[partyCount - 1];
        float diffBonusRate = DungeonDifficultyBonus[diffIdx];
        float modeBonusRate = DungeonModeBonus[modeIdx];
        int totalRoomKills = activeSlots.Sum(s => s.KillNpcNum);

        var (timeRank, timeBonus) = ComputeDungeonTimeResult(
            dungeonComboId,
            room.MonsterManager,
            Math.Max(1, playTimeSec),
            totalRoomKills);

        var result = new KEGS_END_GAME_DUNGEON_RESULT_DATA_NOT
        {
            Sec = Math.Max(1, playTimeSec),
            IsWin = isWin,
            DungeonId = room.Info.DungeonId,
            Difficulty = room.Info.DifficultyLevel,
            DungeonMode = room.Info.DungeonMode,
            IsHenirDungeon = isHenir,
        };

        var playResults = room.SnapshotDungeonPlayResults();

        // SERV_DUNGEON_MVP: select MVP only when >= 3 active members, by highest damage/contribution
        long mvpUid = activeSlots.Count >= 3
            ? activeSlots
                .OrderByDescending(s =>
                {
                    if (playResults.TryGetValue(s.UnitUid, out var pr) && pr.TotalGivenDamage > 0)
                        return pr.TotalGivenDamage + pr.ComboScore + pr.TechScore - pr.TotalAttackedDamage;
                    return s.ComboScore + s.TechnicalScore - s.Damaged;
                })
                .Select(s => s.UnitUid)
                .FirstOrDefault()
            : 0L;

        foreach (var s in activeSlots)
        {
            playResults.TryGetValue(s.UnitUid, out var pr);
            int comboScore = pr != null && pr.ComboScore > 0 ? pr.ComboScore : s.ComboScore;
            int techScore = pr != null && pr.TechScore > 0 ? pr.TechScore : s.TechnicalScore;
            int attackedDmg = pr != null && pr.TotalAttackedDamage > 0 ? pr.TotalAttackedDamage : s.Damaged;
            int recoveryHp = pr?.TotalRecoveryHp ?? 0;
            int baseHp = pr != null && pr.Hp > 1f
                ? (int)pr.Hp
                : (s.BaseHp > 1 ? s.BaseHp : Math.Max(1000, s.Level * 600));
            int usingKindOfSkill = pr?.UsingKindOfSkill ?? 0;

            var (comboRank, comboBonus) = ComputeDungeonComboResult(
                comboScore, dungeonComboId, room.MonsterManager, endNumMember, totalRoomKills);
            var (techRank, techBonus) = ComputeDungeonTechnicalResult(
                techScore, usingKindOfSkill, dungeonComboId, room.MonsterManager, endNumMember, totalRoomKills);
            var (dmgRank, dmgBonus) = ComputeDungeonDamageResult(attackedDmg, recoveryHp, baseHp);
            var totalRank = TotalRankSumToRank((int)comboRank, (int)techRank, (int)dmgRank, (int)timeRank);

            // ResultProcess.cpp:2038: cap rank bonus rate at 0.15f (15%)
            float rankBonusRate = Math.Min(0.15f, comboBonus + techBonus + dmgBonus + timeBonus);

            int totalScore = pr != null && pr.TotalScore > 0
                ? pr.TotalScore
                : comboScore + techScore;

            int baseExp = isWin ? Math.Max(400, (int)((s.Level * 280 + s.KillNpcNum * 45) * diffBonusRate)) : s.KillNpcNum * 25;
            int partyBonus = isWin ? (int)(baseExp * partyBonusRate * 0.15f) : 0;
            int expPlusParty = baseExp + partyBonus;

            // ResultProcess.cpp:2049-2058: clear (+30%), rank (+rankBonusRate), MVP (+10%) apply to (EXP + PartyBonusEXP)
            int clearBonus = isWin ? (int)(expPlusParty * 0.3f) : 0;
            int rankBonus = isWin ? (int)(expPlusParty * rankBonusRate) : 0;
            bool isMvp = isWin && s.UnitUid == mvpUid;
            int mvpBonus = isMvp ? (int)(expPlusParty * 0.1f) : 0;
            int baseEd = isWin ? (int)((500 + s.Level * 120 + s.KillNpcNum * 30) * modeBonusRate * 0.5f) : 0;
            int edReward = baseEd;

            // ResultProcess.cpp:2073-2098: PC Bang ED/EXP/PetEXP rates
            int gameBangBonusExp = 0;
            if (isWin && s.IsGameBang && sysVal != null)
            {
                edReward += (int)(baseEd * sysVal.GetGbEdRate(s.PcBangType));
                gameBangBonusExp = (int)(expPlusParty * sysVal.GetGbExpRate(s.PcBangType));
                if (s.HasPet)
                    gameBangBonusExp += (int)(expPlusParty * sysVal.GetGbPetExpRate(s.PcBangType));
            }

            // ResultProcess.cpp:2104-2110: GameEvent, Premium & Channel bonus rates
            int eventExp = 0;
            int premiumBonusExp = 0;
            int channelBonusExp = 0;
            if (isWin)
            {
                if (s.BonusRate.TryGetValue(BtGevEd, out float evEd) && evEd > 0f)
                    edReward += (int)(baseEd * evEd);
                if (s.BonusRate.TryGetValue(BtGevExp, out float evExp) && evExp > 0f)
                    eventExp += (int)(expPlusParty * evExp);
                if (s.BonusRate.TryGetValue(BtGevPartyExp, out float evPartyExp) && evPartyExp > 0f)
                    eventExp += (int)(partyBonus * evPartyExp);
                if (s.BonusRate.TryGetValue(BtGevWithCharExp, out float evWithCharExp) && evWithCharExp > 0f)
                    eventExp += (int)(expPlusParty * evWithCharExp);
                if (s.BonusRate.TryGetValue(BtPremiumExpRate, out float premExp) && premExp > 0f)
                    premiumBonusExp = (int)(expPlusParty * premExp);
                if (s.BonusRate.TryGetValue(BtChannelBonusExpRate, out float chExp) && chExp > 0f)
                    channelBonusExp = (int)(expPlusParty * chExp);
            }

            result.HaveExpInDungeon[s.UnitUid] = isWin || s.KillNpcNum > 0;
            result.SuitableLevelInfo[s.UnitUid] = true;
            var slot = room.SnapshotSlots().FirstOrDefault(sl => sl.SlotState == 2 && sl.RoomUserInfo.UnitUid == s.UnitUid);
            if (slot != null && slot.RoomUserInfo.OngoingQuest.Count > 0)
                result.OngoingQuest[s.UnitUid] = new Dictionary<int, KSubQuestInfo>(slot.RoomUserInfo.OngoingQuest);

            result.DungeonUnitInfo.Add(new KDungeonUnitResultInfo
            {
                UnitUid = s.UnitUid,
                IsDie = s.DeathNum > 0,
                TotalScore = totalScore,
                ComboScore = comboScore,
                TechnicalScore = techScore,
                Damaged = attackedDmg,
                OldEd = s.Ed,
                OldExp = s.Exp,
                Ed = edReward,
                Exp = baseExp,
                ClearBonusExp = clearBonus,
                RankBonusExp = rankBonus,
                PartyBonusExp = partyBonus,
                MvpBonusExp = mvpBonus,
                GameBangBonusExp = gameBangBonusExp,
                PremiumBonusExp = premiumBonusExp,
                EventExp = eventExp,
                ChannelBonusExp = channelBonusExp,
                KillNpcNum = s.KillNpcNum,
                ComboRank = (sbyte)comboRank,
                TechnicalRank = (sbyte)techRank,
                DamagedRank = (sbyte)dmgRank,
                TimeRank = (sbyte)timeRank,
                TotalRank = (sbyte)totalRank,
                IsMvp = isMvp,
            });
        }

        return result;
    }

    public static RankType TotalRankSumToRank(int comboRank, int techRank, int dmgRank, int timeRank)
    {
        // Exact thresholds from ServerResource/TW/ResultData_new.lua (DUNGEON_TOTAL_RANK_RESULT)
        int sum = comboRank + techRank + dmgRank + timeRank;
        return sum switch
        {
            >= 29 => RankType.RT_SS,
            >= 27 => RankType.RT_S,
            >= 20 => RankType.RT_A,
            >= 14 => RankType.RT_B,
            >= 10 => RankType.RT_C,
            >= 4 => RankType.RT_D,
            _ => RankType.RT_F,
        };
    }

    public static RankType ScoreToRank(int score0To100) => score0To100 switch
    {
        >= 95 => RankType.RT_SS,
        >= 85 => RankType.RT_S,
        >= 70 => RankType.RT_A,
        >= 55 => RankType.RT_B,
        >= 40 => RankType.RT_C,
        >= 25 => RankType.RT_D,
        >= 10 => RankType.RT_E,
        _ => RankType.RT_F,
    };
}
