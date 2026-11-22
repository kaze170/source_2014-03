using KncWX2Server.Core.Scripting;
using MoonSharp.Interpreter;

namespace KncWX2Server.GameServer;

/// <summary>
/// Faithful C# port of GameServer/RecommendUserTable.{h,cpp} (SERV_RECOMMEND_LIST_EVENT):
/// level-milestone rewards for recommended users and event bonus rewards.
/// </summary>
public sealed class KRecommendUserTable
{
    private readonly Dictionary<byte, int> _recommendData = new();
    private readonly Dictionary<byte, int> _recommendEventData = new();

    public int RecommendEventReward { get; set; }
    public int RecommendedEventReward1 { get; set; }
    public int RecommendedEventReward2 { get; set; }

    public KRecommendUserTable()
    {
        // Standard Elsword milestone rewards (Lv 10, 20, 30, 40, 50)
        AddRecommendUserData(10, 10010);
        AddRecommendUserData(20, 10020);
        AddRecommendUserData(30, 10030);
        AddRecommendUserData(40, 10040);
        AddRecommendUserData(50, 10050);
    }

    public bool AddRecommendUserData(byte level, int rewardId)
    {
        if (level <= 1 || rewardId <= 0) return false;
        _recommendData[level] = rewardId;
        return true;
    }

    public int GetRecommendUserReward(byte level)
        => _recommendData.GetValueOrDefault(level, 0);

    public bool AddRecommendUserEventReward(byte level, int rewardId)
    {
        if (level <= 1 || rewardId <= 0) return false;
        _recommendEventData[level] = rewardId;
        return true;
    }

    public int GetRecommendUserEventReward(byte level)
        => _recommendEventData.GetValueOrDefault(level, 0);

    public void BindToLua(KScriptManager script, string globalName = "g_pKRecommendUserTable")
    {
        Table? proxy = null;
        proxy = script.CreateLuaProxy(globalName, t =>
        {
            t["AddRecommendEventReward"] = DynValue.NewCallback((_, a) =>
            {
                RecommendEventReward = KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, proxy!), RecommendEventReward);
                return DynValue.Nil;
            });
            t["AddRecommendedEventReward1"] = DynValue.NewCallback((_, a) =>
            {
                RecommendedEventReward1 = KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, proxy!), RecommendedEventReward1);
                return DynValue.Nil;
            });
            t["AddRecommendedEventReward2"] = DynValue.NewCallback((_, a) =>
            {
                RecommendedEventReward2 = KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, proxy!), RecommendedEventReward2);
                return DynValue.Nil;
            });
            t["AddRecommendUserData"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                AddRecommendUserData((byte)Math.Clamp(KScriptManager.ArgInt(a, o), 0, 255), KScriptManager.ArgInt(a, o + 1));
                return DynValue.Nil;
            });
            t["AddRecommendUserEventReward"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                AddRecommendUserEventReward((byte)Math.Clamp(KScriptManager.ArgInt(a, o), 0, 255), KScriptManager.ArgInt(a, o + 1));
                return DynValue.Nil;
            });
        });
    }
}
