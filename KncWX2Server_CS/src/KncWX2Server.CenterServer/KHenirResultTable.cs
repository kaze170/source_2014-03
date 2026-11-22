using System.Collections.Concurrent;
using KncWX2Server.Core.Framework;
using KncWX2Server.Core.Scripting;
using MoonSharp.Interpreter;

namespace KncWX2Server.CenterServer;

/// <summary>
/// Faithful C# port of CenterServer/HenirResultTable.{h,cpp} &amp; ServerResource/TW/HenirResultTable.lua / HenirData.lua:
/// maps Henir Time-Space cleared stage counts &amp; challenge mode to reward item groups,
/// resurrection checkpoint stages, and temp-inventory clear stages.
/// </summary>
public sealed class KHenirResultTable
{
    public sealed record HenirRewardEntry(int StageCount, sbyte HenirMode, int ItemGroupId, int RandomCount);

    private readonly ConcurrentDictionary<(int StageCount, sbyte Mode), List<HenirRewardEntry>> _rewards = new();
    private readonly ConcurrentDictionary<(sbyte Mode, int StageCount), List<(int ItemId, int Quantity)>> _challengeRewards = new();
    private readonly ConcurrentDictionary<int, KLottery> _rewardGroups = new();
    private readonly HashSet<int> _resurrectionStages = [10, 20, 30, 40, 50];
    private readonly HashSet<int> _clearTempInventoryStages = [10, 20, 30, 40, 50];
    private readonly HashSet<int> _notifyStages = [40, 50];
    public bool EnableNotify { get; set; } = true;

    public KHenirResultTable()
    {
        var g1 = new KLottery();
        g1.AddCase(130010, 60.0, 2); // Magic Stone
        g1.AddCase(130077, 40.0, 1); // Weapon Enchant Stone
        _rewardGroups[1] = g1;

        AddHenirReward(5, 0, 1, 1);
        AddHenirReward(10, 0, 1, 2);
        AddHenirReward(20, 1, 1, 3);
    }

    public void AddHenirReward(int stageCount, sbyte mode, int itemGroupId, int randomCount)
    {
        var list = _rewards.GetOrAdd((stageCount, mode), _ => []);
        list.Add(new HenirRewardEntry(stageCount, mode, itemGroupId, randomCount));
    }

    public void AddRewardGroupItem(int groupId, int itemId, double probability, int quantity = 1)
    {
        var lot = _rewardGroups.GetOrAdd(groupId, _ => new KLottery());
        lot.AddCase(itemId, probability, quantity);
    }

    public void AddHenirChallangeReward(sbyte mode, int stageCount, int itemId, int quantity)
    {
        if (quantity <= 0 || itemId <= 0) return;
        var list = _challengeRewards.GetOrAdd((mode, stageCount), _ => []);
        list.Add((itemId, quantity));
    }

    public IReadOnlyList<(int ItemId, int Quantity)> GetHenirChallangeRewards(sbyte mode, int stageCount)
        => _challengeRewards.TryGetValue((mode, stageCount), out var list) ? list : Array.Empty<(int, int)>();

    public bool IsResurrectionStage(int stage) => _resurrectionStages.Contains(stage);
    public bool IsClearTempInventoryStage(int stage) => _clearTempInventoryStages.Contains(stage);
    public bool IsClearNotifyStage(int stage) => _notifyStages.Contains(stage);

    public List<(int ItemId, int Quantity)> RollStageRewards(int stageCount, sbyte mode)
    {
        var output = new List<(int, int)>();
        if (!_rewards.TryGetValue((stageCount, mode), out var entries))
            return output;

        foreach (var e in entries)
        {
            if (!_rewardGroups.TryGetValue(e.ItemGroupId, out var lot)) continue;
            for (int i = 0; i < e.RandomCount; i++)
            {
                int itemId = lot.Decision();
                if (itemId != KLottery.CaseBlank)
                    output.Add((itemId, Math.Max(1, lot.GetParam1(itemId))));
            }
        }
        return output;
    }

    public void BindToLua(KScriptManager script, string globalName = "HenirResultTable")
    {
        Table? proxy = null;
        bool clearedRes = false, clearedTemp = false, clearedNotify = false;
        proxy = script.CreateLuaProxy(globalName, t =>
        {
            t["AddHenirResultItemInfo"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                AddHenirReward(KScriptManager.ArgInt(a, o), (sbyte)KScriptManager.ArgInt(a, o + 1),
                    KScriptManager.ArgInt(a, o + 2), KScriptManager.ArgInt(a, o + 3, 1));
                return DynValue.Nil;
            });
            t["AddHenirResultItemGroup"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                AddRewardGroupItem(KScriptManager.ArgInt(a, o), KScriptManager.ArgInt(a, o + 1),
                    KScriptManager.ArgDouble(a, o + 3, 100.0), KScriptManager.ArgInt(a, o + 2, 1));
                return DynValue.Nil;
            });
            t["AddResurrectionStage"] = DynValue.NewCallback((_, a) =>
            {
                if (!clearedRes) { _resurrectionStages.Clear(); clearedRes = true; }
                int st = KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, proxy!));
                if (st > 0) _resurrectionStages.Add(st);
                return DynValue.Nil;
            });
            t["AddClearTempInventoryStage"] = DynValue.NewCallback((_, a) =>
            {
                if (!clearedTemp) { _clearTempInventoryStages.Clear(); clearedTemp = true; }
                int st = KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, proxy!));
                if (st > 0) _clearTempInventoryStages.Add(st);
                return DynValue.Nil;
            });
            t["AddClearNotifyStage"] = DynValue.NewCallback((_, a) =>
            {
                if (!clearedNotify) { _notifyStages.Clear(); clearedNotify = true; }
                int st = KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, proxy!));
                if (st > 0) _notifyStages.Add(st);
                return DynValue.Nil;
            });
            t["SetEnbleNotify"] = DynValue.NewCallback((_, a) =>
            {
                EnableNotify = KScriptManager.ArgBool(a, KScriptManager.ArgStart(a, proxy!), EnableNotify);
                return DynValue.Nil;
            });
            t["SetEnableNotify"] = t["SetEnbleNotify"];
            t["AddHenirChallangeRewardInfo"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                AddHenirChallangeReward(
                    (sbyte)KScriptManager.ArgInt(a, o),
                    KScriptManager.ArgInt(a, o + 1),
                    KScriptManager.ArgInt(a, o + 2),
                    KScriptManager.ArgInt(a, o + 3));
                return DynValue.Nil;
            });
        });
    }
}
