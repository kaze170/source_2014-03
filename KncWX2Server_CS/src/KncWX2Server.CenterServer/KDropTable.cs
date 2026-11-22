using System.Collections.Concurrent;
using KncWX2Server.Core.Framework;
using KncWX2Server.Core.Scripting;
using MoonSharp.Interpreter;

namespace KncWX2Server.CenterServer;

/// <summary>
/// Faithful C# port of CenterServer/KDropTable.{h,cpp}:
/// weighted drop pools via KLottery for Dungeon/Field monsters, Static drops, Henir drops,
/// PvP Camilla's Coin (17.0%) drop, and MoonSharp Lua loading.
/// </summary>
public sealed class KDropTable
{
    public const int ItemCamillasCoin = 130046; // EI_CAMILLAS_COIN

    public sealed record PaymentItemRule(
        int DungeonOrKind,
        int ItemId,
        double Rate,
        int QuestId = 0,
        int MinLv = 0,
        int MaxLv = 999,
        int Quantity = 1,
        string BeginDate = "",
        string EndDate = "");

    public sealed record TimeDropItemInfo(
        int TimeDropId,
        int ItemId,
        string ItemName,
        DateTime DropTimeUtc,
        bool NoticeMsg)
    {
        public bool IsDropped { get; set; }
    }

    public sealed record MonsterDropMeta(int Exp, int Ed, float EdProperty, bool KillCheck);
    public const int DcpUnaryItem = 1;
    public const int DcpItemGroup = 2;

    private readonly ConcurrentDictionary<int, KLottery> _monsterDrops = new();
    private readonly ConcurrentDictionary<(int dungeonId, int npcId), KLottery> _dungeonMonsterDrops = new();
    private readonly ConcurrentDictionary<(int dungeonId, int npcId), MonsterDropMeta> _monsterDropMeta = new();
    private readonly ConcurrentDictionary<int, KLottery> _dropGroups = new();
    private readonly ConcurrentDictionary<int, KLottery> _staticDrops = new();
    private readonly ConcurrentDictionary<int, KLottery> _henirNpcDrops = new();
    private readonly ConcurrentDictionary<(int mode, int npcId), KLottery> _henirModeDrops = new();
    private readonly ConcurrentDictionary<(int mode, int npcId), MonsterDropMeta> _henirDropMeta = new();
    private readonly ConcurrentDictionary<(int bfId, int npcId), KLottery> _battleFieldMonsterDrops = new();
    private readonly ConcurrentDictionary<(int bfId, int npcId), MonsterDropMeta> _battleFieldDropMeta = new();
    private readonly ConcurrentDictionary<int, KLottery> _battleFieldStaticDrops = new();
    private readonly ConcurrentDictionary<int, KLottery> _extraStageNpcDrops = new();
    private readonly ConcurrentDictionary<int, MonsterDropMeta> _extraStageDropMeta = new();
    private readonly ConcurrentDictionary<int, (float multiplyEd, float multiplyCount)> _manyEdDropNpcs = new();
    private readonly ConcurrentDictionary<(int difficulty, int itemId), int> _attribNpcSpecialDrops = new();
    private readonly ConcurrentDictionary<int, double> _pvpFailRates = new();
    private readonly List<TimeDropItemInfo> _timeDropItems = [];
    private readonly ConcurrentDictionary<int, int> _npcExpTable = new();
    private readonly List<PaymentItemRule> _paymentDungeons = [];
    private readonly List<PaymentItemRule> _unPaymentConditions = [];
    private readonly KLottery _pvpDrop = new();
    private readonly HashSet<int> _notifyDropItems = [];
    private readonly object _timeDropLock = new();
    public bool NotifyDropItemEnabled { get; set; }
    public bool EnchantItemDropEventEnabled { get; set; }
    public bool ManyEdDropNpcEventEnabled { get; set; }
    public int PvpItemChangeTimeSec { get; set; } = 3;
    public IReadOnlyDictionary<(int difficulty, int itemId), int> AttribNpcSpecialDrops => _attribNpcSpecialDrops;
    public IReadOnlyDictionary<int, double> PvpFailRates => _pvpFailRates;

    public int GetNpcExp(int npcLevel) => _npcExpTable.GetValueOrDefault(npcLevel, 0);
    public int GetAttribNpcSpecialDropCount(int difficulty, int itemId) => _attribNpcSpecialDrops.GetValueOrDefault((difficulty, itemId), 0);
    public double GetPvpFailRate(int failType) => _pvpFailRates.GetValueOrDefault(failType, 0.0);
    public IReadOnlyList<PaymentItemRule> PaymentDungeons => _paymentDungeons;
    public IReadOnlyList<PaymentItemRule> UnPaymentConditions => _unPaymentConditions;
    public bool ShouldNotifyDropItem(int itemId) => NotifyDropItemEnabled && _notifyDropItems.Contains(itemId);

    public KDropTable()
    {
        // Exact C++ constructor initialization: Camilla's Coin 17%
        _pvpDrop.AddCase(ItemCamillasCoin, 17.0);

        // Default dungeon monster drop group (potions, magic stones, cubes)
        var common = new KLottery();
        common.AddCase(130001, 35.0, 1); // Intermediate HP Potion
        common.AddCase(130010, 20.0, 1); // Magic Stone
        common.AddCase(130077, 10.0, 1); // Weapon Enchant Stone Lv.1
        common.AddCase(130086, 10.0, 1); // Armor Enchant Stone Lv.1
        _dropGroups[1] = common;
    }

    public void RegisterMonsterDrop(int npcId, int itemId, double probability, int quantity = 1)
    {
        var lot = _monsterDrops.GetOrAdd(npcId, _ => new KLottery());
        lot.AddCase(itemId, probability, quantity, DcpUnaryItem);
    }

    public void RegisterDropGroup(int groupId, int itemId, double probability, int quantity = 1)
    {
        var lot = _dropGroups.GetOrAdd(groupId, _ => new KLottery());
        lot.AddCase(itemId, probability, quantity, DcpUnaryItem);
    }

    public void RegisterHenirNpcDrop(int npcId, int itemId, double probability, int quantity = 1)
    {
        var lot = _henirNpcDrops.GetOrAdd(npcId, _ => new KLottery());
        lot.AddCase(itemId, probability, quantity, DcpUnaryItem);
    }

    public void RegisterStaticDrop(int dungeonModeOrId, int itemId, double probability, int quantity = 1)
    {
        var lot = _staticDrops.GetOrAdd(dungeonModeOrId, _ => new KLottery());
        lot.AddCase(itemId, probability, quantity, DcpUnaryItem);
    }

    public (bool dropped, int itemId, int quantity) RollPvpDrop()
    {
        int itemId = _pvpDrop.Decision();
        return itemId == KLottery.CaseBlank ? (false, 0, 0) : (true, itemId, 1);
    }

    private (bool dropped, int itemId, int quantity) ResolveLotteryPick(KLottery lot, float rateMultiplier = 1.0f)
    {
        int picked = lot.DecisionWithRate(rateMultiplier);
        if (picked == KLottery.CaseBlank)
            return (false, 0, 0);

        if (lot.GetParam2(picked) == DcpItemGroup)
        {
            if (_dropGroups.TryGetValue(picked, out var grp))
            {
                int grpItem = grp.Decision();
                if (grpItem != KLottery.CaseBlank)
                    return (true, grpItem, Math.Max(1, grp.GetParam1(grpItem)));
            }
            return (false, 0, 0);
        }

        return (true, picked, Math.Max(1, lot.GetParam1(picked)));
    }

    public (bool dropped, int itemId, int quantity) RollMonsterDrop(
        int npcId, bool isHenir = false, int fallbackGroupId = 1, int dungeonId = 0, int henirMode = 0)
    {
        if (isHenir)
        {
            if (_henirModeDrops.TryGetValue((henirMode, npcId), out var hmLot))
            {
                var r = ResolveLotteryPick(hmLot);
                if (r.dropped) return r;
            }
            if (henirMode != 0 && _henirModeDrops.TryGetValue((0, npcId), out var hDefLot))
            {
                var r = ResolveLotteryPick(hDefLot);
                if (r.dropped) return r;
            }
            if (_henirNpcDrops.TryGetValue(npcId, out var hLot))
            {
                var r = ResolveLotteryPick(hLot);
                if (r.dropped) return r;
            }
        }

        if (_dungeonMonsterDrops.TryGetValue((dungeonId, npcId), out var dmLot))
        {
            var r = ResolveLotteryPick(dmLot);
            if (r.dropped) return r;
        }
        if (dungeonId != 0 && _dungeonMonsterDrops.TryGetValue((0, npcId), out var dDefLot))
        {
            var r = ResolveLotteryPick(dDefLot);
            if (r.dropped) return r;
        }

        if (_monsterDrops.TryGetValue(npcId, out var mLot))
        {
            var r = ResolveLotteryPick(mLot);
            if (r.dropped) return r;
        }

        if (_dropGroups.TryGetValue(fallbackGroupId, out var gLot))
        {
            var r = ResolveLotteryPick(gLot);
            if (r.dropped) return r;
        }

        return (false, 0, 0);
    }

    public void RegisterBattleFieldMonsterDrop(int battleFieldId, int npcId, int itemId, double probability, int quantity = 1)
    {
        var lot = _battleFieldMonsterDrops.GetOrAdd((battleFieldId, npcId), _ => new KLottery());
        lot.AddCase(itemId, probability, quantity, DcpUnaryItem);
    }

    public void RegisterBattleFieldStaticDrop(int battleFieldId, int itemId, double probability, int quantity = 1)
    {
        var lot = _battleFieldStaticDrops.GetOrAdd(battleFieldId, _ => new KLottery());
        lot.AddCase(itemId, probability, quantity, DcpUnaryItem);
    }

    public void RegisterExtraStageNpcDrop(int npcId, int itemId, double probability, int quantity = 1)
    {
        var lot = _extraStageNpcDrops.GetOrAdd(npcId, _ => new KLottery());
        lot.AddCase(itemId, probability, quantity, DcpUnaryItem);
    }

    public void RegisterTimeDrop(TimeDropItemInfo info)
    {
        lock (_timeDropLock) _timeDropItems.Add(info);
    }

    public void RegisterManyEdDropNpc(int npcId, float multiplyEd, float multiplyCount)
        => _manyEdDropNpcs[npcId] = (multiplyEd, multiplyCount);

    public bool TryGetManyEdDropMultiplier(int npcId, out float multiplyEd, out float multiplyCount)
    {
        if (_manyEdDropNpcs.TryGetValue(npcId, out var m))
        {
            multiplyEd = m.multiplyEd;
            multiplyCount = m.multiplyCount;
            return true;
        }
        multiplyEd = 1.0f;
        multiplyCount = 1.0f;
        return false;
    }

    public (bool dropped, int itemId, int quantity) RollStaticDrop(int dungeonId, float rateMultiplier = 1.0f)
    {
        if (_staticDrops.TryGetValue(dungeonId, out var lot))
            return ResolveLotteryPick(lot, rateMultiplier);
        if (dungeonId != 0 && _staticDrops.TryGetValue(0, out var defLot))
            return ResolveLotteryPick(defLot, rateMultiplier);
        return (false, 0, 0);
    }

    public (bool dropped, int itemId, int quantity) RollBattleFieldDrop(int battleFieldId, int npcId, float rateMultiplier = 1.0f)
    {
        if (_battleFieldMonsterDrops.TryGetValue((battleFieldId, npcId), out var lot))
        {
            var r = ResolveLotteryPick(lot, rateMultiplier);
            if (r.dropped) return r;
        }
        if (battleFieldId != 0 && _battleFieldMonsterDrops.TryGetValue((0, npcId), out var defLot))
        {
            var r = ResolveLotteryPick(defLot, rateMultiplier);
            if (r.dropped) return r;
        }
        if (_battleFieldStaticDrops.TryGetValue(battleFieldId, out var sLot))
            return ResolveLotteryPick(sLot, rateMultiplier);
        return (false, 0, 0);
    }

    public (bool dropped, int itemId, int quantity) RollExtraStageDrop(int npcId, float rateMultiplier = 1.0f)
    {
        if (_extraStageNpcDrops.TryGetValue(npcId, out var lot))
            return ResolveLotteryPick(lot, rateMultiplier);
        return (false, 0, 0);
    }

    public TimeDropItemInfo? TryClaimTimeDrop(DateTime? nowUtc = null)
    {
        var now = nowUtc ?? DateTime.UtcNow;
        lock (_timeDropLock)
        {
            foreach (var td in _timeDropItems)
            {
                if (!td.IsDropped && now >= td.DropTimeUtc)
                {
                    td.IsDropped = true;
                    return td;
                }
            }
        }
        return null;
    }

    public List<(int itemId, int quantity)> RollDungeonClearPaymentItems(
        int dungeonId,
        int unitLevel,
        HashSet<int>? clearedOrOngoingQuests = null,
        DateTime? nowUtc = null)
    {
        var now = nowUtc ?? DateTime.UtcNow;
        var results = new List<(int, int)>();
        foreach (var rule in _paymentDungeons)
        {
            if (rule.DungeonOrKind != dungeonId) continue;
            if (unitLevel < rule.MinLv || unitLevel > rule.MaxLv) continue;
            if (rule.QuestId > 0 && (clearedOrOngoingQuests == null || !clearedOrOngoingQuests.Contains(rule.QuestId)))
                continue;
            if (!string.IsNullOrEmpty(rule.BeginDate) && DateTime.TryParse(rule.BeginDate, out var begin) && now < begin)
                continue;
            if (!string.IsNullOrEmpty(rule.EndDate) && DateTime.TryParse(rule.EndDate, out var end) && now > end)
                continue;

            bool blocked = _unPaymentConditions.Any(u =>
                u.DungeonOrKind == dungeonId && u.ItemId == rule.ItemId &&
                unitLevel >= u.MinLv && unitLevel <= u.MaxLv);
            if (blocked) continue;

            if (Random.Shared.NextDouble() * 100.0 < rule.Rate)
                results.Add((rule.ItemId, rule.Quantity));
        }
        return results;
    }

    public void BindToLua(KScriptManager script, string globalName = "DropTable")
    {
        Table? proxy = null;
        proxy = script.CreateLuaProxy(globalName, t =>
        {
            t["AddToGroup"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                RegisterDropGroup(KScriptManager.ArgInt(a, o), KScriptManager.ArgInt(a, o + 1), KScriptManager.ArgDouble(a, o + 2));
                return DynValue.Nil;
            });
            t["AddMonsterDropInfo"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                var tbl = KScriptManager.ArgTable(a, o);
                if (tbl != null)
                {
                    int dungeonId = (int)(tbl.Get("DungeonID").CastToNumber() ?? -1);
                    int monsterId = (int)(tbl.Get("MonsterID").CastToNumber() ?? -1);
                    if (!(dungeonId == -1 && monsterId == -1) && monsterId != 0)
                    {
                        var lot = new KLottery();
                        PopulateLotteryFromDropLists(tbl, lot);
                        _dungeonMonsterDrops[(dungeonId, monsterId)] = lot;
                        _monsterDropMeta[(dungeonId, monsterId)] = ParseDropMeta(tbl);
                    }
                }
                else
                {
                    RegisterMonsterDrop(KScriptManager.ArgInt(a, o), KScriptManager.ArgInt(a, o + 1), KScriptManager.ArgDouble(a, o + 2));
                }
                return DynValue.Nil;
            });
            t["AddHenirMonsterDropInfo"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                var tbl = KScriptManager.ArgTable(a, o);
                if (tbl != null)
                {
                    int mode = (int)(tbl.Get("HenirDungeonMode").CastToNumber() ?? -1);
                    int monsterId = (int)(tbl.Get("MonsterID").CastToNumber() ?? -1);
                    if (!(mode == -1 && monsterId == -1) && monsterId != 0)
                    {
                        var lot = new KLottery();
                        PopulateLotteryFromDropLists(tbl, lot);
                        _henirModeDrops[(mode, monsterId)] = lot;
                        _henirDropMeta[(mode, monsterId)] = ParseDropMeta(tbl);
                    }
                }
                else
                {
                    RegisterHenirNpcDrop(KScriptManager.ArgInt(a, o), KScriptManager.ArgInt(a, o + 1), KScriptManager.ArgDouble(a, o + 2));
                }
                return DynValue.Nil;
            });
            t["AddExtraStageMonsterDropInfo"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                var tbl = KScriptManager.ArgTable(a, o);
                if (tbl != null)
                {
                    int monsterId = (int)(tbl.Get("MonsterID").CastToNumber() ?? -1);
                    if (monsterId > 0)
                    {
                        var lot = new KLottery();
                        PopulateLotteryFromDropLists(tbl, lot);
                        _extraStageNpcDrops[monsterId] = lot;
                        _extraStageDropMeta[monsterId] = ParseDropMeta(tbl);
                    }
                }
                else
                {
                    RegisterExtraStageNpcDrop(KScriptManager.ArgInt(a, o), KScriptManager.ArgInt(a, o + 1), KScriptManager.ArgDouble(a, o + 2));
                }
                return DynValue.Nil;
            });
            t["AddBattleFieldNpcDropInfo"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                var tbl = KScriptManager.ArgTable(a, o);
                if (tbl != null)
                {
                    int bfId = (int)(tbl.Get("BattleFieldID").CastToNumber() ?? -1);
                    int monsterId = (int)(tbl.Get("MonsterID").CastToNumber() ?? -1);
                    if (!(bfId == -1 && monsterId == -1) && monsterId != 0)
                    {
                        var lot = new KLottery();
                        PopulateLotteryFromDropLists(tbl, lot);
                        _battleFieldMonsterDrops[(bfId, monsterId)] = lot;
                        _battleFieldDropMeta[(bfId, monsterId)] = ParseDropMeta(tbl);
                    }
                }
                return DynValue.Nil;
            });
            t["AddStaticDropInfo"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                var tbl = KScriptManager.ArgTable(a, o);
                if (tbl != null)
                {
                    var enVal = tbl.Get("Enable");
                    bool enabled = enVal.IsNil() || enVal.CastToBool();
                    int dungeonId = (int)(tbl.Get("DungeonID").CastToNumber() ?? -1);
                    if (enabled && dungeonId >= 0)
                    {
                        var lot = new KLottery();
                        PopulateLotteryFromDropLists(tbl, lot);
                        _staticDrops[dungeonId] = lot;
                    }
                }
                else
                {
                    RegisterStaticDrop(KScriptManager.ArgInt(a, o), KScriptManager.ArgInt(a, o + 1), KScriptManager.ArgDouble(a, o + 2));
                }
                return DynValue.Nil;
            });
            t["AddPVPDropInfo"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                _pvpDrop.AddCase(KScriptManager.ArgInt(a, o), KScriptManager.ArgDouble(a, o + 1));
                return DynValue.Nil;
            });
            t["addPVPDropInfo"] = t["AddPVPDropInfo"];
            t["SetNotifyDropItem"] = DynValue.NewCallback((_, a) =>
            {
                NotifyDropItemEnabled = KScriptManager.ArgBool(a, KScriptManager.ArgStart(a, proxy!));
                return DynValue.Nil;
            });
            t["AddNotifyDropItemInfo"] = DynValue.NewCallback((_, a) =>
            {
                int id = KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, proxy!));
                if (id > 0) _notifyDropItems.Add(id);
                return DynValue.Nil;
            });
            t["AddHenirSpecialNpcDropInfo"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                RegisterHenirNpcDrop(KScriptManager.ArgInt(a, o), KScriptManager.ArgInt(a, o + 1), 100.0, KScriptManager.ArgInt(a, o + 2, 1));
                return DynValue.Nil;
            });
            t["AddNpcExpTable"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                int lv = KScriptManager.ArgInt(a, o);
                int exp = KScriptManager.ArgInt(a, o + 1);
                if (lv > 0) _npcExpTable[lv] = exp;
                return DynValue.Nil;
            });
            t["AddPaymentDungeon"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                int key = KScriptManager.ArgInt(a, o);
                var tbl = KScriptManager.ArgTable(a, o + 1);
                if (tbl != null)
                    _paymentDungeons.Add(ParsePaymentRule(key, tbl));
                return DynValue.Nil;
            });
            t["AddUnPaymentCondition"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                int key = KScriptManager.ArgInt(a, o);
                var tbl = KScriptManager.ArgTable(a, o + 1);
                if (tbl != null)
                    _unPaymentConditions.Add(ParsePaymentRule(key, tbl));
                return DynValue.Nil;
            });
            t["SetEnchantItemDropEvent"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                var tbl = KScriptManager.ArgTable(a, o);
                EnchantItemDropEventEnabled = tbl != null ? true : KScriptManager.ArgBool(a, o, true);
                return DynValue.Nil;
            });
            t["AddBattleFieldMonsterDropInfo"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                RegisterBattleFieldMonsterDrop(KScriptManager.ArgInt(a, o), KScriptManager.ArgInt(a, o + 1),
                    KScriptManager.ArgInt(a, o + 2), KScriptManager.ArgDouble(a, o + 3), KScriptManager.ArgInt(a, o + 4, 1));
                return DynValue.Nil;
            });
            t["AddBattleFieldStaticDropInfo"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                var tbl = KScriptManager.ArgTable(a, o);
                if (tbl != null)
                {
                    var enVal = tbl.Get("Enable");
                    bool enabled = enVal.IsNil() || enVal.CastToBool();
                    int bfId = (int)(tbl.Get("BattleFieldID").CastToNumber() ?? -1);
                    if (enabled && bfId >= 0)
                    {
                        var lot = new KLottery();
                        PopulateLotteryFromDropLists(tbl, lot);
                        _battleFieldStaticDrops[bfId] = lot;
                    }
                }
                else
                {
                    RegisterBattleFieldStaticDrop(KScriptManager.ArgInt(a, o), KScriptManager.ArgInt(a, o + 1),
                        KScriptManager.ArgDouble(a, o + 2), KScriptManager.ArgInt(a, o + 3, 1));
                }
                return DynValue.Nil;
            });
            t["AddExtraStageNpcDropInfo"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                RegisterExtraStageNpcDrop(KScriptManager.ArgInt(a, o), KScriptManager.ArgInt(a, o + 1),
                    KScriptManager.ArgDouble(a, o + 2), KScriptManager.ArgInt(a, o + 3, 1));
                return DynValue.Nil;
            });
            t["AddManyEDDropNpcInfo"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                RegisterManyEdDropNpc(KScriptManager.ArgInt(a, o), (float)KScriptManager.ArgDouble(a, o + 1, 1.0),
                    (float)KScriptManager.ArgDouble(a, o + 2, 1.0));
                return DynValue.Nil;
            });
            t["AddManyEDDropNpc"] = t["AddManyEDDropNpcInfo"];
            t["SetManyEDDropNpcEvent"] = DynValue.NewCallback((_, a) =>
            {
                ManyEdDropNpcEventEnabled = KScriptManager.ArgBool(a, KScriptManager.ArgStart(a, proxy!));
                return DynValue.Nil;
            });
            t["AddAttribNpcSpecialDrop"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                int diff = KScriptManager.ArgInt(a, o);
                int itemId = KScriptManager.ArgInt(a, o + 1);
                int rateOrCount = KScriptManager.ArgInt(a, o + 2);
                if (itemId > 0)
                    _attribNpcSpecialDrops[(diff, itemId)] = rateOrCount;
                return DynValue.Nil;
            });
            t["SetPVPFailRate"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                int failType = KScriptManager.ArgInt(a, o);
                double rate = KScriptManager.ArgDouble(a, o + 1);
                _pvpFailRates[failType] = rate;
                return DynValue.Nil;
            });
            t["SetPVPItemChangeTime"] = DynValue.NewCallback((_, a) =>
            {
                PvpItemChangeTimeSec = Math.Max(1, KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, proxy!), PvpItemChangeTimeSec));
                return DynValue.Nil;
            });
            t["AddTimeDropInfo"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                int id = KScriptManager.ArgInt(a, o);
                int itemId = KScriptManager.ArgInt(a, o + 1);
                string itemName = KScriptManager.ArgString(a, o + 2);
                string dtStr = KScriptManager.ArgString(a, o + 3);
                bool notice = KScriptManager.ArgBool(a, o + 4);
                var dt = DateTime.TryParse(dtStr, out var parsed) ? parsed.ToUniversalTime() : DateTime.UtcNow;
                RegisterTimeDrop(new TimeDropItemInfo(id, itemId, itemName, dt, notice));
                return DynValue.Nil;
            });
        });
    }

    private static MonsterDropMeta ParseDropMeta(Table tbl) => new(
        Exp: (int)(tbl.Get("Exp").CastToNumber() ?? -1),
        Ed: (int)(tbl.Get("ED").CastToNumber() ?? -1),
        EdProperty: (float)(tbl.Get("EDProperty").CastToNumber() ?? 0.0),
        KillCheck: tbl.Get("KillCheck").CastToBool());

    private static void PopulateLotteryFromDropLists(Table tbl, KLottery lot)
    {
        var itemList = tbl.Get("DropItemList").Table;
        if (itemList != null)
        {
            foreach (var entry in itemList.Values)
            {
                var sub = entry.Table;
                if (sub == null) continue;
                int itemId = (int)(sub.Get(1).CastToNumber() ?? 0);
                double prob = sub.Get(2).CastToNumber() ?? 0.0;
                if (itemId > 0 && prob > 0.0)
                    lot.AddCase(itemId, prob, 1, DcpUnaryItem);
            }
        }

        var groupList = tbl.Get("DropGroupList").Table;
        if (groupList != null)
        {
            foreach (var entry in groupList.Values)
            {
                var sub = entry.Table;
                if (sub == null) continue;
                int grpId = (int)(sub.Get(1).CastToNumber() ?? 0);
                double prob = sub.Get(2).CastToNumber() ?? 0.0;
                if (grpId > 0 && prob > 0.0)
                    lot.AddCase(grpId, prob, 1, DcpItemGroup);
            }
        }
    }

    private static PaymentItemRule ParsePaymentRule(int key, Table tbl) => new(
        DungeonOrKind: key,
        ItemId: (int)(tbl.Get("iItemID").CastToNumber() ?? 0),
        Rate: tbl.Get("fRate").CastToNumber() ?? 0.0,
        QuestId: (int)(tbl.Get("iQuestID").CastToNumber() ?? 0),
        MinLv: (int)(tbl.Get("iMinLv").CastToNumber() ?? 0),
        MaxLv: (int)(tbl.Get("iMaxLv").CastToNumber() ?? 999),
        Quantity: Math.Max(1, (int)(tbl.Get("iQuantity").CastToNumber() ?? 1)),
        BeginDate: tbl.Get("wstrBeginDate").CastToString() ?? "",
        EndDate: tbl.Get("wstrEndDate").CastToString() ?? "");
}
