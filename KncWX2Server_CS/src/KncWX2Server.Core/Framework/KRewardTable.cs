using KncWX2Server.Core.Logging;
using KncWX2Server.Core.Packets;
using KncWX2Server.Core.Scripting;
using MoonSharp.Interpreter;

namespace KncWX2Server.Core.Framework;

/// <summary>
/// Faithful C# port of GameServer/RewardTable.{h,cpp}:
/// manages script &amp; DB reward tables, ComeBack user reward cubes,
/// OutUser retaining slot selections, and level-up reward packages.
/// </summary>
public sealed class KRewardTable
{
    public const int EriComeBackUserItemCubeOne = 130101;
    public const int EriComeBackUserItemCubeTwo = 130102;
    public const int EriComeBackUserItemCubeThree = 130103;

    public sealed record RewardEntry(int ItemId, int Quantity, short PeriodDays);

    private readonly Dictionary<int, List<RewardEntry>> _scriptRewards = new();
    private readonly Dictionary<int, List<RewardEntry>> _dbRewards = new();
    private readonly Dictionary<int, RewardEntry> _retainingRewards = new();
    private readonly Dictionary<byte, List<int>> _levelUpRewards = new();
    private readonly object _sync = new();

    public KRewardTable()
    {
        // Default ComeBack reward cubes & retaining slots
        AddScriptReward(EriComeBackUserItemCubeOne, 130048, 5, 15);
        AddScriptReward(EriComeBackUserItemCubeTwo, 130048, 10, 30);
        AddScriptReward(EriComeBackUserItemCubeThree, 130048, 20, 60);

        SetOutUserRetainingReward(0, 130048, 5);
        SetOutUserRetainingReward(1, 130001, 10);
        SetOutUserRetainingReward(2, 130040, 3);

        AddLevelUpReward(10, 10010);
        AddLevelUpReward(20, 10020);
        AddLevelUpReward(35, 10035);
        AddLevelUpReward(50, 10050);
        AddLevelUpReward(60, 10060);
    }

    public bool AddScriptReward(int rewardId, int itemId, int quantity, short periodDays = 0)
    {
        if (rewardId <= 0 || itemId <= 0 || quantity <= 0) return false;
        lock (_sync)
        {
            if (!_scriptRewards.TryGetValue(rewardId, out var list))
            {
                list = [];
                _scriptRewards[rewardId] = list;
            }
            list.Add(new RewardEntry(itemId, quantity, periodDays));
            return true;
        }
    }

    public void SetDbRewards(IReadOnlyDictionary<int, IReadOnlyList<RewardEntry>> dbRewards)
    {
        lock (_sync)
        {
            _dbRewards.Clear();
            foreach (var kv in dbRewards)
                _dbRewards[kv.Key] = kv.Value.ToList();
        }
    }

    public bool GetRewardInfo(int rewardId, out Dictionary<int, int> items)
    {
        items = new Dictionary<int, int>();
        lock (_sync)
        {
            if (!_dbRewards.TryGetValue(rewardId, out var list) &&
                !_scriptRewards.TryGetValue(rewardId, out list))
            {
                return false;
            }

            foreach (var entry in list)
                items[entry.ItemId] = items.GetValueOrDefault(entry.ItemId) + entry.Quantity;
            return items.Count > 0;
        }
    }

    public IReadOnlyList<RewardEntry> GetRewardEntries(int rewardId)
    {
        lock (_sync)
        {
            if (_dbRewards.TryGetValue(rewardId, out var dbList)) return dbList.ToList();
            if (_scriptRewards.TryGetValue(rewardId, out var scList)) return scList.ToList();
            return [];
        }
    }

    public void SetOutUserRetainingReward(int slotIndex, int itemId, int quantity)
    {
        lock (_sync) _retainingRewards[slotIndex] = new RewardEntry(itemId, quantity, 0);
    }

    public bool TryGetOutUserRetainingReward(int slotIndex, out int itemId, out int quantity)
    {
        lock (_sync)
        {
            if (_retainingRewards.TryGetValue(slotIndex, out var entry))
            {
                itemId = entry.ItemId;
                quantity = entry.Quantity;
                return true;
            }
            itemId = 0;
            quantity = 0;
            return false;
        }
    }

    public IReadOnlyDictionary<int, RewardEntry> GetAllOutUserRetainingRewards()
    {
        lock (_sync) return new Dictionary<int, RewardEntry>(_retainingRewards);
    }

    public bool AddLevelUpReward(byte level, int rewardId)
    {
        if (level == 0 || rewardId <= 0) return false;
        lock (_sync)
        {
            if (!_levelUpRewards.TryGetValue(level, out var list))
            {
                list = [];
                _levelUpRewards[level] = list;
            }
            if (!list.Contains(rewardId)) list.Add(rewardId);
            return true;
        }
    }

    public IReadOnlyList<int> GetLevelUpRewardIds(byte level)
    {
        lock (_sync)
            return _levelUpRewards.TryGetValue(level, out var list) ? list.ToList() : [];
    }

    public static int GetComeBackCubeItemId(KGameSysValTable.ComeBackStep step) => step switch
    {
        KGameSysValTable.ComeBackStep.CbsOne => EriComeBackUserItemCubeOne,
        KGameSysValTable.ComeBackStep.CbsTwo => EriComeBackUserItemCubeTwo,
        KGameSysValTable.ComeBackStep.CbsThree => EriComeBackUserItemCubeThree,
        _ => 0,
    };

    public void BindToLua(KScriptManager script, string globalName = "g_pKRewardTable")
    {
        Table? proxy = null;
        proxy = script.CreateLuaProxy(globalName, t =>
        {
            t["AddRewardData"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                int rewardId = KScriptManager.ArgInt(a, o);
                int itemId = KScriptManager.ArgInt(a, o + 1);
                int quantity = KScriptManager.ArgInt(a, o + 2);
                short period = (short)Math.Max(0, KScriptManager.ArgInt(a, o + 3));
                bool ok = AddScriptReward(rewardId, itemId, quantity, period);
                if (!ok)
                    KLog.Cerr($"KRewardTable::AddRewardData invalid args: rewardId={rewardId}, itemId={itemId}, qty={quantity}, period={period}");
                return DynValue.NewBoolean(ok);
            });
            t["AddLevelUpRewardData"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                byte level = (byte)Math.Clamp(KScriptManager.ArgInt(a, o), 0, byte.MaxValue);
                int rewardId = KScriptManager.ArgInt(a, o + 1);
                return DynValue.NewBoolean(AddLevelUpReward(level, rewardId));
            });
            t["SetOutUserRetainingRewardItemInfo"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                int slot = KScriptManager.ArgInt(a, o);
                int itemId = KScriptManager.ArgInt(a, o + 1);
                int quantity = KScriptManager.ArgInt(a, o + 2);
                SetOutUserRetainingReward(slot, itemId, quantity);
                return DynValue.Nil;
            });
            t["dump"] = DynValue.NewCallback((_, _) =>
            {
                lock (_sync)
                {
                    KLog.Cout($"[KRewardTable] ScriptRewards={_scriptRewards.Count}, DbRewards={_dbRewards.Count}, LevelUp={_levelUpRewards.Count}, Retaining={_retainingRewards.Count}");
                }
                return DynValue.Nil;
            });
        });
    }
}
