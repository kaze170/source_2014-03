using KncWX2Server.Core.Framework;
using KncWX2Server.Core.Scripting;
using MoonSharp.Interpreter;

namespace KncWX2Server.CenterServer;

/// <summary>
/// Faithful C# port of CenterServer/BattleFieldGameManager.{h,cpp} &amp; ServerResource/TW/BattleFieldServerData.lua:
/// tracks battlefield dangerousness progression, warning triggers, 20%-chance elite monster drops,
/// middle-boss / event-boss / boss drop reservations, and event consumption.
/// </summary>
public sealed class KBattleFieldGameManager
{
    public enum DangerousEvent
    {
        DeWarningMessage = 0,
        DeEliteMonsterDrop = 1,
        DeMiddleBossMonsterDrop = 2,
        DeEventBossMonsterDrop = 3,
        DeBossMonsterDrop = 4,
    }

    private readonly HashSet<DangerousEvent> _reservedEvents = [];
    private readonly KLottery _eliteMonsterLottery = new();
    private readonly object _sync = new();

    public static int DefaultDangerousValueMax { get; private set; } = 10_000;
    public static int DefaultDangerousValueWarning { get; private set; } = 3_000;
    public static int DefaultEliteMonsterDropValue { get; private set; } = 500;
    public static int DefaultBossCheckUserCount { get; private set; } = 1;
    public static int DefaultDangerousValueEventRate { get; private set; } = 1;
    public static float BattleFieldEdFactor { get; private set; } = 1.0f;
    public static float BattleFieldExpFactor { get; private set; } = 1.0f;
    public static float BattleFieldFactor { get; private set; } = 1.0f;
    public static int MonsterRespawnFactorByUserCount { get; private set; } = 3;
    public static int MonsterRespawnTimeMin { get; private set; } = 15;
    public static int MonsterRespawnTimeMax { get; private set; } = 25;
    public static List<(int MinUser, int MaxUser, int Difficulty, int DropCount)> EliteMonsterDropInfos { get; } = new();
    public sealed record MiddleBossDropInfo(
        int BattleFieldId,
        int MonsterId,
        int CommonBonusItemId,
        int CommonBonusItemCount,
        int BonusItemId,
        int BonusItemCountHigh,
        int BonusItemCountLow);
    public static Dictionary<(int BattleFieldId, int MonsterId), MiddleBossDropInfo> MiddleBossDropInfos { get; } = new();

    public sealed record EventBossDropInfo(
        int BattleFieldId,
        int MonsterId,
        int CommonBonusItemId,
        int CommonBonusItemCount,
        int BonusItemId,
        int BonusItemCountHigh,
        int BonusItemCountLow);
    public static Dictionary<(int BattleFieldId, int MonsterId), EventBossDropInfo> EventBossDropInfos { get; } = new();

    public static bool GetBattleFieldEventBossDropInfo(int battleFieldId, int monsterId, out EventBossDropInfo? info)
        => EventBossDropInfos.TryGetValue((battleFieldId, monsterId), out info);

    public int DangerousValue { get; private set; }
    public int OldDangerousValue { get; private set; }

    public int DangerousValueMax { get; set; } = DefaultDangerousValueMax;
    public int DangerousValueWarning { get; set; } = DefaultDangerousValueWarning;
    public int EliteMonsterDropValue { get; set; } = DefaultEliteMonsterDropValue;
    public int BossCheckUserCount { get; set; } = DefaultBossCheckUserCount;
    public int DangerousValueEventRate { get; set; } = DefaultDangerousValueEventRate;

    public void StartGame()
    {
        lock (_sync)
        {
            DangerousValue = 0;
            OldDangerousValue = 0;
            _reservedEvents.Clear();
            _eliteMonsterLottery.Clear();
        }
    }

    public void EndGame() => StartGame();

    public void ResetDangerousValue()
    {
        lock (_sync)
        {
            DangerousValue = 0;
            OldDangerousValue = 0;
        }
    }

    public void UpdateDangerousValue(int value)
    {
        lock (_sync)
        {
            OldDangerousValue = DangerousValue;
            DangerousValue = value;
        }
    }

    public void IncreaseDangerousValue(int delta)
    {
        lock (_sync)
        {
            OldDangerousValue = DangerousValue;
            DangerousValue += delta * Math.Max(1, DangerousValueEventRate);
            if (DangerousValue < 0 || DangerousValue >= DangerousValueMax)
            {
                DangerousValue = 0;
                OldDangerousValue = 0;
            }
        }
    }

    public void OnNpcUnitDie(
        int playerCount,
        bool isAttribNpc,
        sbyte difficultyLevel,
        sbyte monsterGrade,
        bool increaseDanger = true,
        double? fixedEliteRoll = null,
        double? fixedBossRoll = null)
    {
        if (!increaseDanger) return;

        int factor = ComputeMonsterTypeFactor(isAttribNpc, difficultyLevel, monsterGrade);
        lock (_sync)
        {
            int before = DangerousValue;
            IncreaseDangerousValue(factor);
            CheckReserveWarningEventLocked(before);
            CheckReserveEliteMonsterDropEventLocked(before, fixedEliteRoll);
            CheckReserveMiddleBossDropEventLocked(playerCount, fixedBossRoll);
            CheckReserveBossDropEventLocked(playerCount, fixedBossRoll);
        }
    }

    public bool IsEventReserved(DangerousEvent ev)
    {
        lock (_sync) return _reservedEvents.Contains(ev);
    }

    public bool CheckAndDeleteReservedDangerousEvent(DangerousEvent ev)
    {
        lock (_sync) return _reservedEvents.Remove(ev);
    }

    public static int ComputeMonsterTypeFactor(bool isAttribNpc, sbyte difficulty, sbyte grade)
    {
        int baseFactor = Math.Max(1, grade + 1) * 10 + Math.Max(0, (int)difficulty) * 5;
        return isAttribNpc ? baseFactor * 2 : baseFactor;
    }

    private void CheckReserveWarningEventLocked(int before)
    {
        if (before < DangerousValueWarning && DangerousValue >= DangerousValueWarning)
            _reservedEvents.Add(DangerousEvent.DeWarningMessage);
    }

    private void CheckReserveEliteMonsterDropEventLocked(int before, double? fixedRoll)
    {
        if (EliteMonsterDropValue <= 0 || DangerousValue < EliteMonsterDropValue) return;
        bool isMultiple = (DangerousValue % EliteMonsterDropValue) == 0;
        bool isGap = (DangerousValue - before) > EliteMonsterDropValue;
        if (!isMultiple && !isGap) return;

        // C++ BattleFieldGameManager.cpp L150: add 20% probability each step until triggered
        _eliteMonsterLottery.AddCase(1, 20.0);
        if (_eliteMonsterLottery.Decision(fixedRoll) == 1)
        {
            _eliteMonsterLottery.Clear();
            _reservedEvents.Add(DangerousEvent.DeEliteMonsterDrop);
        }
    }

    private void CheckReserveMiddleBossDropEventLocked(int playerCount, double? fixedRoll)
    {
        if (playerCount < BossCheckUserCount || DangerousValue < DangerousValueWarning) return;
        if (_reservedEvents.Contains(DangerousEvent.DeMiddleBossMonsterDrop)) return;

        int diff = Math.Max(0, DangerousValue - DangerousValueWarning);
        double rate = Math.Clamp(5.0 + (diff / 500.0) * 10.0, 5.0, 100.0);
        var lot = new KLottery();
        lot.AddCase(1, rate);
        if (lot.Decision(fixedRoll) == 1)
            _reservedEvents.Add(DangerousEvent.DeMiddleBossMonsterDrop);
    }

    private void CheckReserveBossDropEventLocked(int playerCount, double? fixedRoll)
    {
        if (playerCount < BossCheckUserCount || DangerousValue < DangerousValueWarning) return;
        if (_reservedEvents.Contains(DangerousEvent.DeBossMonsterDrop)) return;

        int diff = Math.Max(0, DangerousValue - DangerousValueWarning);
        double rate = Math.Clamp(2.0 + (diff / 500.0) * 10.0, 2.0, 100.0);
        var lot = new KLottery();
        lot.AddCase(1, rate);
        if (lot.Decision(fixedRoll) == 1)
            _reservedEvents.Add(DangerousEvent.DeBossMonsterDrop);
    }

    public static void BindToLua(KScriptManager script, string globalName = "g_pBattleFieldManager")
    {
        Table? proxy = null;
        proxy = script.CreateLuaProxy(globalName, t =>
        {
            t["SetMonsterRespawnFactorByUserCount"] = DynValue.NewCallback((_, a) => { MonsterRespawnFactorByUserCount = KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, proxy!), MonsterRespawnFactorByUserCount); return DynValue.Nil; });
            t["SetMonsterRespawnTimeMinMax"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                MonsterRespawnTimeMin = KScriptManager.ArgInt(a, o, MonsterRespawnTimeMin);
                MonsterRespawnTimeMax = KScriptManager.ArgInt(a, o + 1, MonsterRespawnTimeMax);
                return DynValue.Nil;
            });
            t["SetBattleFieldEDFactor"] = DynValue.NewCallback((_, a) => { BattleFieldEdFactor = KScriptManager.ArgFloat(a, KScriptManager.ArgStart(a, proxy!), BattleFieldEdFactor); return DynValue.Nil; });
            t["SetBattleFieldEXPFactor"] = DynValue.NewCallback((_, a) => { BattleFieldExpFactor = KScriptManager.ArgFloat(a, KScriptManager.ArgStart(a, proxy!), BattleFieldExpFactor); return DynValue.Nil; });
            t["SetBattleFieldFactor"] = DynValue.NewCallback((_, a) => { BattleFieldFactor = KScriptManager.ArgFloat(a, KScriptManager.ArgStart(a, proxy!), BattleFieldFactor); return DynValue.Nil; });
            t["SetDangerousValueMax"] = DynValue.NewCallback((_, a) => { DefaultDangerousValueMax = KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, proxy!), DefaultDangerousValueMax); return DynValue.Nil; });
            t["SetDangerousValueWarning"] = DynValue.NewCallback((_, a) => { DefaultDangerousValueWarning = KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, proxy!), DefaultDangerousValueWarning); return DynValue.Nil; });
            t["SetEliteMonsterDropValue"] = DynValue.NewCallback((_, a) => { DefaultEliteMonsterDropValue = KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, proxy!), DefaultEliteMonsterDropValue); return DynValue.Nil; });
            t["SetBossCheckUserCount"] = DynValue.NewCallback((_, a) => { DefaultBossCheckUserCount = KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, proxy!), DefaultBossCheckUserCount); return DynValue.Nil; });
            t["SetDangerousValueEventRate"] = DynValue.NewCallback((_, a) => { DefaultDangerousValueEventRate = KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, proxy!), DefaultDangerousValueEventRate); return DynValue.Nil; });
            t["AddEliteMonsterDropInfo"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                EliteMonsterDropInfos.Add((KScriptManager.ArgInt(a, o), KScriptManager.ArgInt(a, o + 1), KScriptManager.ArgInt(a, o + 2), KScriptManager.ArgInt(a, o + 3)));
                return DynValue.Nil;
            });
            t["BattleFieldMiddleBossDropInfo"] = DynValue.NewCallback((_, a) =>
            {
                var tbl = KScriptManager.ArgTable(a, KScriptManager.ArgStart(a, proxy!));
                if (tbl != null)
                {
                    int bfId = (int)(tbl.Get("BattleFieldID").CastToNumber() ?? 0);
                    int monId = (int)(tbl.Get("MonsterID").CastToNumber() ?? 0);
                    if (bfId > 0 && monId > 0)
                    {
                        MiddleBossDropInfos[(bfId, monId)] = new MiddleBossDropInfo(
                            bfId,
                            monId,
                            (int)(tbl.Get("SetMiddleBossCommonBonusItemID").CastToNumber() ?? 0),
                            (int)(tbl.Get("SetMiddleBossCommonBonusItemCount").CastToNumber() ?? 1),
                            (int)(tbl.Get("SetMiddleBossBonusItemID").CastToNumber() ?? 0),
                            (int)(tbl.Get("SetMiddleBossBonusItemIDCount_High").CastToNumber() ?? 1),
                            (int)(tbl.Get("SetMiddleBossBonusItemIDCount_Low").CastToNumber() ?? 1));
                    }
                }
                return DynValue.Nil;
            });
            t["BattleFieldEventBossDropInfo"] = DynValue.NewCallback((_, a) =>
            {
                var tbl = KScriptManager.ArgTable(a, KScriptManager.ArgStart(a, proxy!));
                if (tbl != null)
                {
                    int bfId = (int)(tbl.Get("BattleFieldID").CastToNumber() ?? 0);
                    int monId = (int)(tbl.Get("MonsterID").CastToNumber() ?? 0);
                    if (bfId > 0 && monId > 0)
                    {
                        EventBossDropInfos[(bfId, monId)] = new EventBossDropInfo(
                            bfId,
                            monId,
                            (int)(tbl.Get("SetEventBossCommonBonusItemID").CastToNumber() ?? 0),
                            (int)(tbl.Get("SetEventBossCommonBonusItemCount").CastToNumber() ?? 1),
                            (int)(tbl.Get("SetEventBossBonusItemID").CastToNumber() ?? 0),
                            (int)(tbl.Get("SetEventBossBonusItemIDCount_High").CastToNumber() ?? 1),
                            (int)(tbl.Get("SetEventBossBonusItemIDCount_Low").CastToNumber() ?? 1));
                    }
                }
                return DynValue.Nil;
            });
        });
    }
}
