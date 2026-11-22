using System.Globalization;
using KncWX2Server.Core.Framework;
using KncWX2Server.Core.Packets;
using KncWX2Server.Core.Region;
using KncWX2Server.Core.Scripting;
using MoonSharp.Interpreter;

namespace KncWX2Server.GameServer;

/// <summary>
/// Faithful C# port of Common/X2Data/XSLRandomItemManager.{h,cpp}:
/// supports random cubes / ice-burners with UseCondition filtering (Anyone / OneUnit / OneClass),
/// key-item / ED requirements, GiveAll vs KLottery single-roll, resurrection/spirit cubes,
/// attraction broadcast strings, sealed random cubes, time-windowed group remapping, and Lua loading.
/// </summary>
public sealed class KRandomItemManager
{
    public enum UseCondition : byte
    {
        Anyone = 0,
        OneUnit = 1,
        OneClass = 2,
    }

    public sealed record RandomUnitData(sbyte UnitClass, int ItemGroupId);

    public sealed class RandomItemData
    {
        public int ItemId { get; init; }
        public int KeyItemId { get; init; }
        public int RequiredKeyCount { get; init; }
        public int RequiredEd { get; init; }
        public int ResurrectionCount { get; init; }
        public int RestoreSpirit { get; init; }
        public bool GiveAll { get; init; }
        public UseCondition Condition { get; init; } = UseCondition.Anyone;
        public List<RandomUnitData> UnitData { get; init; } = [];
    }

    public sealed record ItemGroupEntry(int ItemId, float Rate, int Period, int Quantity);

    public sealed class ItemGroupDataList
    {
        public int GroupId { get; init; }
        public KLottery Lottery { get; } = new();
        public List<ItemGroupEntry> Items { get; } = [];
    }

    public sealed record MappingRule(int CubeId, int KeyItemId, int BeforeGroupId, int AfterGroupId, DateTime StartUtc, DateTime EndUtc);

    private readonly Dictionary<(int itemId, int keyItemId), RandomItemData> _randomItems = new();
    private readonly Dictionary<int, ItemGroupDataList> _itemGroups = new();
    private readonly Dictionary<int, (bool notify, bool sealedGlobal)> _attractionItems = new();
    private readonly HashSet<int> _sealRandomItems = [];
    private readonly HashSet<int> _notifyResultItems = [];
    private readonly HashSet<int> _charmItems = [];
    private readonly List<MappingRule> _mappingRules = [];
    private readonly Dictionary<int, (int CubeId, int KeyItemId, int BeforeGroupId, int AfterGroupId)> _pendingMapData = new();

    public bool NotifyMsgEnabled { get; set; }

    public void RegisterRandomItem(RandomItemData data)
        => _randomItems[(data.ItemId, data.KeyItemId)] = data;

    public bool AddRandomItemGroup(int groupId, int itemId, float rate, int period = 0, int quantity = 1)
    {
        if (groupId <= 0 || itemId <= 0) return false;
        if (!_itemGroups.TryGetValue(groupId, out var group))
        {
            group = new ItemGroupDataList { GroupId = groupId };
            _itemGroups[groupId] = group;
        }
        group.Items.Add(new ItemGroupEntry(itemId, rate, period, quantity));
        return group.Lottery.AddCase(itemId, rate, quantity, period);
    }

    public void AddAttractionItemInfo(int itemId, bool notify, bool sealedGlobal = false)
        => _attractionItems[itemId] = (notify, sealedGlobal);

    public void AddSealRandomItem(int itemId) => _sealRandomItems.Add(itemId);
    public void AddMappingRule(MappingRule rule) => _mappingRules.Add(rule);
    public bool IsSealRandomItem(int itemId) => _sealRandomItems.Contains(itemId);
    public bool IsAttractionItem(int itemId) => _attractionItems.ContainsKey(itemId);

    public RandomItemData? GetRandomItemData(int randomItemId, int keyItemId = 0)
    {
        if (_randomItems.TryGetValue((randomItemId, keyItemId), out var exact))
            return exact;
        if (keyItemId == 0)
            return _randomItems.Values.FirstOrDefault(r => r.ItemId == randomItemId);
        return null;
    }

    public static sbyte GetBaseUnitClass(sbyte unitClass)
    {
        int baseUnit = unitClass switch
        {
            1 or 10 or 11 or 22 or 100 or 101 or 112 => 1,
            2 or 14 or 15 or 23 or 104 or 105 or 113 => 2,
            3 or 12 or 13 or 24 or 102 or 103 or 114 => 3,
            4 or 16 or 17 or 25 or 106 or 107 or 115 => 4,
            5 or 18 or 19 or 26 or 108 or 109 or 116 => 5,
            6 or 20 or 21 or 27 or 110 or 111 or 117 => 6,
            7 or 28 or 31 or 118 or 119 => 7,
            8 or 29 or 30 or 120 or 121 => 8,
            9 or 32 or 122 => 9,
            _ => unitClass,
        };
        return (sbyte)baseUnit;
    }

    public (int ok, List<KItemInfo> rewardItems, int resurrectionCount, int restoreSpirit) OpenRandomItem(
        sbyte unitClass,
        int randomItemId,
        int keyItemId = 0,
        DateTime? nowUtcOverride = null,
        double? fixedRoll = null)
    {
        var data = GetRandomItemData(randomItemId, keyItemId);
        if (data == null || data.UnitData.Count == 0)
            return (NetError.ERR_RANDOM_ITEM_02, [], 0, 0);

        int groupId = 0;
        if (data.Condition == UseCondition.Anyone)
        {
            groupId = data.UnitData[0].ItemGroupId;
        }
        else
        {
            sbyte lookupClass = data.Condition == UseCondition.OneUnit ? GetBaseUnitClass(unitClass) : unitClass;
            groupId = data.UnitData.FirstOrDefault(u => u.UnitClass == lookupClass)?.ItemGroupId ?? 0;
        }

        var now = nowUtcOverride ?? DateTime.UtcNow;
        var activeMap = _mappingRules.FirstOrDefault(m =>
            m.CubeId == randomItemId && (m.KeyItemId == 0 || m.KeyItemId == data.KeyItemId) &&
            m.BeforeGroupId == groupId && m.StartUtc <= now && now <= m.EndUtc);
        if (activeMap != null)
            groupId = activeMap.AfterGroupId;

        if (groupId == 0 && (data.ResurrectionCount > 0 || data.RestoreSpirit > 0))
            return (NetError.NET_OK, [], data.ResurrectionCount, data.RestoreSpirit);

        if (groupId == 0 || !_itemGroups.TryGetValue(groupId, out var group))
            return (NetError.ERR_RANDOM_ITEM_02, [], 0, 0);

        var rewards = new List<KItemInfo>();
        if (data.GiveAll)
        {
            foreach (var entry in group.Items)
            {
                rewards.Add(new KItemInfo
                {
                    ItemId = entry.ItemId,
                    Quantity = entry.Quantity,
                    Period = (short)entry.Period,
                });
            }
        }
        else
        {
            int pickedId = group.Lottery.Decision(fixedRoll);
            if (pickedId == KLottery.CaseBlank || pickedId <= 0)
                return (NetError.ERR_RANDOM_ITEM_02, [], 0, 0);
            var matched = group.Items.First(i => i.ItemId == pickedId);
            rewards.Add(new KItemInfo
            {
                ItemId = matched.ItemId,
                Quantity = matched.Quantity,
                Period = (short)matched.Period,
            });
        }

        return (NetError.NET_OK, rewards, data.ResurrectionCount, data.RestoreSpirit);
    }

    public Dictionary<int, KRandomItemResult> OpenRandomItemDeveloper(
        int itemId, int keyItemId, int openCount, sbyte unitClass = 1)
    {
        var results = new Dictionary<int, KRandomItemResult>();
        int count = Math.Clamp(openCount, 1, 10000);
        for (int i = 0; i < count; i++)
        {
            var (ok, rewards, _, _) = OpenRandomItem(unitClass, itemId, keyItemId);
            if (ok != NetError.NET_OK)
            {
                if (i == 0)
                {
                    results[itemId] = new KRandomItemResult { ItemCount = count, ItemTotalCount = count, Rate = 100.0 };
                }
                break;
            }
            foreach (var r in rewards)
            {
                if (!results.TryGetValue(r.ItemId, out var stat))
                {
                    stat = new KRandomItemResult();
                    results[r.ItemId] = stat;
                }
                stat.ItemCount++;
                stat.ItemTotalCount += Math.Max(1, r.Quantity);
            }
        }
        foreach (var kv in results)
            kv.Value.Rate = Math.Round(kv.Value.ItemCount * 100.0 / count, 4);
        return results;
    }

    public string? MakeNotifyMsgAttractionItem(int rewardItemId, string nickName, string itemName)
    {
        if (!NotifyMsgEnabled || !_attractionItems.TryGetValue(rewardItemId, out var info) || !info.notify)
            return null;
        return $"{nickName};{itemName}";
    }

    public void BindToLua(KScriptManager script, string globalName = "g_pRandomItemManager")
    {
        Table? proxy = null;
        proxy = script.CreateLuaProxy(globalName, t =>
        {
            t["SetNotifyMsg"] = DynValue.NewCallback((_, a) =>
            {
                NotifyMsgEnabled = KScriptManager.ArgBool(a, KScriptManager.ArgStart(a, proxy!), true);
                return DynValue.Nil;
            });
            t["AddSealRandomItemInfo"] = DynValue.NewCallback((_, a) =>
            {
                int id = KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, proxy!));
                if (id > 0) AddSealRandomItem(id);
                return DynValue.Nil;
            });
            t["AddAttractionItemInfo"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                int id = KScriptManager.ArgInt(a, o);
                if (id > 0)
                    AddAttractionItemInfo(id, KScriptManager.ArgBool(a, o + 1), KScriptManager.ArgBool(a, o + 2));
                return DynValue.Nil;
            });
            t["AddNotifyResultItemInfo"] = DynValue.NewCallback((_, a) =>
            {
                int id = KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, proxy!));
                if (id > 0) _notifyResultItems.Add(id);
                return DynValue.Nil;
            });
            t["AddCharmItem"] = DynValue.NewCallback((_, a) =>
            {
                int id = KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, proxy!));
                if (id > 0) _charmItems.Add(id);
                return DynValue.Nil;
            });
            t["AddRandomItemGroup"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                AddRandomItemGroup(
                    KScriptManager.ArgInt(a, o),
                    KScriptManager.ArgInt(a, o + 1),
                    KScriptManager.ArgFloat(a, o + 2),
                    KScriptManager.ArgInt(a, o + 3),
                    Math.Max(1, KScriptManager.ArgInt(a, o + 4, 1)));
                return DynValue.Nil;
            });
            t["AddRandomItemTemplet"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                var tbl = KScriptManager.ArgTable(a, o);
                if (tbl == null) return DynValue.Nil;
                int itemId = (int)(tbl.Get("m_ItemID").CastToNumber() ?? 0);
                if (itemId <= 0) return DynValue.Nil;

                var unitData = new List<RandomUnitData>();
                foreach (var pair in tbl.Pairs)
                {
                    if (pair.Key.Type == DataType.Number && pair.Value.Type == DataType.Table)
                    {
                        var sub = pair.Value.Table;
                        sbyte uc = (sbyte)(sub.Get("m_cUnitClass").CastToNumber() ?? 0);
                        int gid = (int)(sub.Get("m_iItemGroupID").CastToNumber() ?? 0);
                        unitData.Add(new RandomUnitData(uc, gid));
                    }
                }

                RegisterRandomItem(new RandomItemData
                {
                    ItemId = itemId,
                    KeyItemId = (int)(tbl.Get("m_KeyItemID").CastToNumber() ?? 0),
                    RequiredKeyCount = (int)(tbl.Get("m_RequiredKeyCount").CastToNumber() ?? 0),
                    RequiredEd = (int)(tbl.Get("m_RequiredED").CastToNumber() ?? 0),
                    ResurrectionCount = (int)(tbl.Get("m_iRessurectionCount").CastToNumber() ?? 0),
                    RestoreSpirit = (int)(tbl.Get("m_iRestoreSpirit").CastToNumber() ?? 0),
                    GiveAll = tbl.Get("m_bGiveAll").Type == DataType.Boolean && tbl.Get("m_bGiveAll").Boolean,
                    Condition = (UseCondition)(int)(tbl.Get("m_UseCondition").CastToNumber() ?? 0),
                    UnitData = unitData,
                });
                return DynValue.Nil;
            });
            t["AddRandomItemMappingData"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                int idx = KScriptManager.ArgInt(a, o);
                _pendingMapData[idx] = (
                    KScriptManager.ArgInt(a, o + 1),
                    KScriptManager.ArgInt(a, o + 2),
                    KScriptManager.ArgInt(a, o + 3),
                    KScriptManager.ArgInt(a, o + 4));
                return DynValue.Nil;
            });
            t["AddRandomItemMappingTime"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                int idx = KScriptManager.ArgInt(a, o);
                if (_pendingMapData.TryGetValue(idx, out var d))
                {
                    DateTime.TryParse(KScriptManager.ArgString(a, o + 1), CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var startUtc);
                    DateTime.TryParse(KScriptManager.ArgString(a, o + 2), CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var endUtc);
                    AddMappingRule(new MappingRule(d.CubeId, d.KeyItemId, d.BeforeGroupId, d.AfterGroupId, startUtc, endUtc));
                }
                return DynValue.Nil;
            });
        });
    }
}
