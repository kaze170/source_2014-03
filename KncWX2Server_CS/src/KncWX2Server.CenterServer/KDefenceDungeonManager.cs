using KncWX2Server.Core.Framework;
using KncWX2Server.Core.Scripting;
using MoonSharp.Interpreter;

namespace KncWX2Server.CenterServer;

/// <summary>
/// Faithful C# port of CenterServer/DefenceDungeonManager.{h,cpp} and
/// Common/X2Data/XSLDefenceDungeonManager.{h,cpp} (SERV_NEW_DEFENCE_DUNGEON):
/// manages Dark Portal (Gate of Darkness) spawn probability, level-gap scaling, phase transitions,
/// quick-slot random pools, special cards, and defence equipment random socket options.
/// </summary>
public sealed class KDefenceDungeonManager
{
    public const int DdpInit = 0;
    public const int DdpPhase1 = 1;
    public const int DdpPhase2 = 2;
    public const int DdpPhase3 = 3;
    public const int DdpPhaseEnd = 4;

    // Default C++ exclusions for non-eligible dungeons (Ruben tutorial, Henir, etc.)
    private static readonly HashSet<int> ExcludedDefenceDungeonIds = [30000, 30010, 39000];
    private readonly Random _rng = new();

    public float NotOpenGateMultiply { get; set; } = 0.25f;
    public int DefenceDungeonQuickSlotPotion { get; set; } = 78902;
    public Dictionary<int, double> EnterLevelConditions { get; } = new();
    public HashSet<int> NotBeginDefenceDungeonIds { get; } = [.. ExcludedDefenceDungeonIds];
    public HashSet<int> DefenceGuardianMonsters { get; } = [];
    public Dictionary<int, (int Item1, int Item2, int Item3)> DefenceDungeonQuickSlotItems { get; } = new();
    public Dictionary<int, List<(int Item1, int Item2, int Item3)>> QuickSlotItemGroups { get; } = new();
    public Dictionary<int, List<(int SocketId, double Rate)>> SocketOptionRandomGroups { get; } = new();
    public Dictionary<int, List<int>> ManufactureSocketOptionGroups { get; } = new();
    public Dictionary<int, Dictionary<int, (int Phase1, int Phase2, int Phase3)>> SpecialCardGroups { get; } = new();
    public List<(int GroupId, int ItemId, int MaterialId, int Count)> ManufactureItems { get; } = [];
    public HashSet<int> OldDefenceItemIds { get; } = [];

    public bool IsOpenDefenceDungeon { get; set; }
    public bool IsEnterDefenceDungeon { get; private set; }
    public int DefenceDungeonPhase { get; private set; } = DdpInit;

    public void Clear()
    {
        IsOpenDefenceDungeon = false;
        IsEnterDefenceDungeon = false;
        DefenceDungeonPhase = DdpInit;
    }

    public void CopyConfigFrom(KDefenceDungeonManager src)
    {
        NotOpenGateMultiply = src.NotOpenGateMultiply;
        DefenceDungeonQuickSlotPotion = src.DefenceDungeonQuickSlotPotion;

        EnterLevelConditions.Clear();
        foreach (var kv in src.EnterLevelConditions) EnterLevelConditions[kv.Key] = kv.Value;

        NotBeginDefenceDungeonIds.Clear();
        foreach (int id in src.NotBeginDefenceDungeonIds) NotBeginDefenceDungeonIds.Add(id);

        DefenceGuardianMonsters.Clear();
        foreach (int id in src.DefenceGuardianMonsters) DefenceGuardianMonsters.Add(id);

        DefenceDungeonQuickSlotItems.Clear();
        foreach (var kv in src.DefenceDungeonQuickSlotItems) DefenceDungeonQuickSlotItems[kv.Key] = kv.Value;

        QuickSlotItemGroups.Clear();
        foreach (var kv in src.QuickSlotItemGroups) QuickSlotItemGroups[kv.Key] = [.. kv.Value];

        SocketOptionRandomGroups.Clear();
        foreach (var kv in src.SocketOptionRandomGroups) SocketOptionRandomGroups[kv.Key] = [.. kv.Value];

        ManufactureSocketOptionGroups.Clear();
        foreach (var kv in src.ManufactureSocketOptionGroups) ManufactureSocketOptionGroups[kv.Key] = [.. kv.Value];

        SpecialCardGroups.Clear();
        foreach (var kv in src.SpecialCardGroups)
            SpecialCardGroups[kv.Key] = new Dictionary<int, (int, int, int)>(kv.Value);

        ManufactureItems.Clear();
        ManufactureItems.AddRange(src.ManufactureItems);

        OldDefenceItemIds.Clear();
        foreach (int id in src.OldDefenceItemIds) OldDefenceItemIds.Add(id);
    }

    public void StartGame()
    {
        IsEnterDefenceDungeon = false;
        DefenceDungeonPhase = DdpInit;
    }

    public void EndGame() => Clear();

    public void OnCloseRoom() => Clear();

    public bool IsDefenceGuardianMonster(int npcId) => DefenceGuardianMonsters.Contains(npcId);
    public bool IsNotBeginDefenceDungeonId(int dungeonId) => NotBeginDefenceDungeonIds.Contains(dungeonId);
    public bool IsManufactureDefenceDungeonItem(int itemId) => ManufactureSocketOptionGroups.ContainsKey(itemId);
    public bool IsOldDefenceItemId(int itemId) => OldDefenceItemIds.Contains(itemId);

    public bool DecisionEnterDefenceDungeon(
        int dungeonId,
        int npcLevel,
        IReadOnlyDictionary<long, int> unitLevelList,
        float multipleRate,
        bool is100PercentEventOn = false,
        double? deterministicRoll = null)
    {
        if (IsEnterDefenceDungeon) return true;
        if (!IsOpenDefenceDungeon || NotBeginDefenceDungeonIds.Contains(dungeonId) || unitLevelList.Count == 0)
            return false;

        if (is100PercentEventOn)
        {
            IsEnterDefenceDungeon = true;
            DefenceDungeonPhase = DdpPhase1;
            return true;
        }

        // C++ checks party average vs NPC level; severe over-leveling prevents gate entry
        double avgLevel = unitLevelList.Values.Average();
        double diff = avgLevel - npcLevel;
        if (diff >= 10.0)
            return false;

        double baseRate = 15.0;
        if (EnterLevelConditions.Count > 0)
        {
            foreach (var kv in EnterLevelConditions.OrderBy(k => k.Key))
            {
                if (diff <= kv.Key) { baseRate = kv.Value; break; }
            }
        }
        baseRate = Math.Clamp(baseRate * Math.Max(0.1f, multipleRate), 0.0, 100.0);
        double roll = (deterministicRoll ?? _rng.NextDouble()) * 100.0;
        if (roll < baseRate)
        {
            IsEnterDefenceDungeon = true;
            DefenceDungeonPhase = DdpPhase1;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Exact C++ CXSLDefenceDungeonManager::GetEnterDefenceDungeonRate(iDungeonNpcLevel, mapUnitLevelList).
    /// </summary>
    public float GetEnterDefenceDungeonRate(int dungeonNpcLevel, IReadOnlyDictionary<long, int> unitLevelList)
    {
        float resultRate = 0f;
        var orderedConditions = EnterLevelConditions.OrderBy(k => k.Key).ToList();
        foreach (var kv in unitLevelList)
        {
            int levelGap = Math.Abs(dungeonNpcLevel - kv.Value);
            foreach (var cond in orderedConditions)
            {
                if (cond.Key >= levelGap)
                {
                    resultRate += (float)cond.Value;
                    break;
                }
            }
        }
        return resultRate;
    }

    public bool IncreaseDefenceDungeonPhase(int npcId)
    {
        if (!IsEnterDefenceDungeon || npcId <= 0) return false;
        if (DefenceGuardianMonsters.Count > 0 && !DefenceGuardianMonsters.Contains(npcId))
            return false;

        if (DefenceDungeonPhase < DdpPhase3)
        {
            DefenceDungeonPhase++;
            return true;
        }
        return false;
    }

    public bool GetResultRandomSocketOption(int groupId, IReadOnlyList<int> notIncludeOptionIds, out int socketOptionId)
    {
        socketOptionId = 0;
        if (!SocketOptionRandomGroups.TryGetValue(groupId, out var entries) || entries.Count == 0)
            return false;

        var lottery = new KLottery();
        foreach (var (optId, rate) in entries)
            lottery.AddCase(optId, rate);

        foreach (int excluded in notIncludeOptionIds)
            lottery.DelProbability(excluded);

        lottery.MakeHundredProbability();
        int result = lottery.Decision();
        if (result == KLottery.CaseBlank)
        {
            result = lottery.GetFirstCase();
            if (result == KLottery.CaseBlank)
                return false;
        }
        socketOptionId = result;
        return true;
    }

    public bool InsertSocketOptionForDefenceDungeonItem(int itemId, List<int> socketOptions)
    {
        if (!ManufactureSocketOptionGroups.TryGetValue(itemId, out var groupIds))
            return false;

        foreach (int groupId in groupIds)
        {
            if (GetResultRandomSocketOption(groupId, socketOptions, out int socketOptionId))
                socketOptions.Add(socketOptionId);
        }
        return true;
    }

    public bool GetResultSpecialCardByDefenceDungeonItem(
        int defenceDungeonPhase,
        int itemId,
        IReadOnlyList<int> socketOptions,
        out int specialCardItemId)
    {
        specialCardItemId = 0;
        if (defenceDungeonPhase < DdpPhase1 || defenceDungeonPhase >= DdpPhaseEnd)
            return false;

        if (!SpecialCardGroups.TryGetValue(itemId, out var bySocket))
            return false;

        foreach (int socketOptionId in socketOptions)
        {
            if (!bySocket.TryGetValue(socketOptionId, out var cards))
                continue;

            specialCardItemId = defenceDungeonPhase switch
            {
                DdpPhase1 => cards.Phase1,
                DdpPhase2 => cards.Phase2,
                DdpPhase3 => cards.Phase3,
                _ => 0,
            };
            if (specialCardItemId > 0)
                return true;
        }
        return false;
    }

    public void GetResultQuickSlotItem(int defenceDungeonPhase, List<int> quickSlotItemIds)
    {
        quickSlotItemIds.Clear();
        if (defenceDungeonPhase < DdpPhase1 || defenceDungeonPhase >= DdpPhaseEnd)
            return;

        var remainingGroups = QuickSlotItemGroups.Keys.ToList();
        for (int cnt = 0; cnt < 3 && remainingGroups.Count > 0; cnt++)
        {
            int idx = _rng.Next(remainingGroups.Count);
            int groupId = remainingGroups[idx];
            remainingGroups.RemoveAt(idx);

            if (!QuickSlotItemGroups.TryGetValue(groupId, out var pool) || pool.Count == 0)
                continue;

            var chosen = pool[_rng.Next(pool.Count)];
            int itemId = defenceDungeonPhase switch
            {
                DdpPhase1 => chosen.Item1,
                DdpPhase2 => chosen.Item2,
                DdpPhase3 => chosen.Item3,
                _ => chosen.Item1,
            };
            if (itemId > 0)
                quickSlotItemIds.Add(itemId);
        }
    }

    public void BindToLua(KScriptManager script, string globalName = "DefenceDungeonManager")
    {
        Table? proxy = null;
        proxy = script.CreateLuaProxy(globalName, t =>
        {
            t["SetNotOpenGateMultiply"] = DynValue.NewCallback((_, a) =>
            {
                NotOpenGateMultiply = KScriptManager.ArgFloat(a, KScriptManager.ArgStart(a, proxy!), NotOpenGateMultiply);
                return DynValue.Nil;
            });
            t["AddEnterLevelCondition"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                EnterLevelConditions[KScriptManager.ArgInt(a, o)] = KScriptManager.ArgDouble(a, o + 1);
                return DynValue.Nil;
            });
            t["AddNotBeginDefenceDungeonID"] = DynValue.NewCallback((_, a) =>
            {
                int id = KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, proxy!));
                if (id > 0) NotBeginDefenceDungeonIds.Add(id);
                return DynValue.Nil;
            });
            t["AddDefenceGuardianMonster"] = DynValue.NewCallback((_, a) =>
            {
                int id = KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, proxy!));
                if (id > 0) DefenceGuardianMonsters.Add(id);
                return DynValue.Nil;
            });
            t["SetDefenceDungeonQuickSlotPotion"] = DynValue.NewCallback((_, a) =>
            {
                DefenceDungeonQuickSlotPotion = KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, proxy!), DefenceDungeonQuickSlotPotion);
                return DynValue.Nil;
            });
            t["AddDefenceDungeonQuickSlotItem"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                int grp = KScriptManager.ArgInt(a, o);
                var tuple = (
                    KScriptManager.ArgInt(a, o + 1),
                    KScriptManager.ArgInt(a, o + 2),
                    KScriptManager.ArgInt(a, o + 3));
                DefenceDungeonQuickSlotItems[grp] = tuple;
                if (!QuickSlotItemGroups.TryGetValue(grp, out var pool))
                    QuickSlotItemGroups[grp] = pool = [];
                pool.Add(tuple);
                return DynValue.Nil;
            });
            t["AddSocketOptionRandomGroup"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                int grp = KScriptManager.ArgInt(a, o);
                if (!SocketOptionRandomGroups.TryGetValue(grp, out var list))
                    SocketOptionRandomGroups[grp] = list = [];
                list.Add((KScriptManager.ArgInt(a, o + 1), KScriptManager.ArgDouble(a, o + 2)));
                return DynValue.Nil;
            });
            t["AddItemInfoForManufacture"] = DynValue.NewCallback((_, a) =>
            {
                var tbl = KScriptManager.ArgTable(a, KScriptManager.ArgStart(a, proxy!));
                if (tbl != null)
                {
                    int itemId = (int)(tbl.Get("m_ItemID").CastToNumber() ?? tbl.Get("m_iItemID").CastToNumber() ?? tbl.Get("ItemID").CastToNumber() ?? 0);
                    if (itemId > 0)
                    {
                        var optGroups = new List<int>();
                        var optTbl = tbl.Get("OPTION_GROUP").Table;
                        if (optTbl != null)
                        {
                            for (int i = 1; i <= optTbl.Length; i++)
                            {
                                int grpId = (int)(optTbl.Get(i).CastToNumber() ?? 0);
                                if (grpId > 0) optGroups.Add(grpId);
                            }
                        }
                        ManufactureSocketOptionGroups[itemId] = optGroups;

                        var cardTbl = tbl.Get("SPECIAL_CARD_GROUP").Table;
                        if (cardTbl != null)
                        {
                            if (!SpecialCardGroups.TryGetValue(itemId, out var cardMap))
                                SpecialCardGroups[itemId] = cardMap = new();
                            for (int i = 1; i <= cardTbl.Length; i++)
                            {
                                var entry = cardTbl.Get(i).Table;
                                if (entry == null) continue;
                                int sockId = (int)(entry.Get("m_SocketOptionID").CastToNumber() ?? 0);
                                int sc1 = (int)(entry.Get("m_SpecialCardItemID1").CastToNumber() ?? 0);
                                int sc2 = (int)(entry.Get("m_SpecialCardItemID2").CastToNumber() ?? 0);
                                int sc3 = (int)(entry.Get("m_SpecialCardItemID3").CastToNumber() ?? 0);
                                if (sockId > 0)
                                    cardMap[sockId] = (sc1, sc2, sc3);
                            }
                        }
                    }

                    ManufactureItems.Add((
                        (int)(tbl.Get("m_iGroupID").CastToNumber() ?? tbl.Get("GroupID").CastToNumber() ?? 0),
                        itemId,
                        (int)(tbl.Get("m_iMaterialItemID").CastToNumber() ?? tbl.Get("MaterialItemID").CastToNumber() ?? 0),
                        (int)(tbl.Get("m_iCount").CastToNumber() ?? tbl.Get("Count").CastToNumber() ?? 1)));
                }
                return DynValue.Nil;
            });
            t["AddOldDefenceItemID"] = DynValue.NewCallback((_, a) =>
            {
                int id = KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, proxy!));
                if (id > 0) OldDefenceItemIds.Add(id);
                return DynValue.Nil;
            });
        });
    }
}
