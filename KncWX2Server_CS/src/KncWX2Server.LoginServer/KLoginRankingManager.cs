using KncWX2Server.Core.Database;
using KncWX2Server.Core.Region;
using KncWX2Server.Core.Scripting;
using MoonSharp.Interpreter;

namespace KncWX2Server.LoginServer;

/// <summary>
/// Faithful C# port of LoginServer/RankingManager.{h,cpp} &amp; HenirRanking.{h,cpp}:
/// maintains cached PvP Rating and Level/Dungeon leaderboards refreshed on interval.
/// </summary>
public sealed class KLoginRankingManager
{
    public sealed record PvpRankRow(int Rank, long UnitUid, string NickName, int Rating, int RPoint, int Win, int Lose);
    public sealed record LevelRankRow(int Rank, long UnitUid, string NickName, int Level, int Exp);
    public sealed record HenirRankingConfig(int RankingType, int MaxRankCount, int RefreshHour, int DayOfWeek, int DayOfMonth);
    public sealed record RankingRewardRule(int RankingType, int Rank, int MailTitleOrItemId, int DaysOrCount);

    private IReadOnlyList<PvpRankRow> _pvpRankings = Array.Empty<PvpRankRow>();
    private IReadOnlyList<LevelRankRow> _levelRankings = Array.Empty<LevelRankRow>();
    private DateTime _lastRefreshUtc = DateTime.MinValue;
    public TimeSpan RefreshInterval { get; set; } = TimeSpan.FromSeconds(30);

    public Dictionary<int, HenirRankingConfig> HenirRankingConfigs { get; } = new();
    public Dictionary<int, List<RankingRewardRule>> RankingRewards { get; } = new();
    public string RankingRewardBeginDate { get; set; } = "";

    public IReadOnlyList<PvpRankRow> PvpRankings => _pvpRankings;
    public IReadOnlyList<LevelRankRow> LevelRankings => _levelRankings;

    public void Tick(SqliteDatabaseManager db, RegionProfile region)
    {
        if (DateTime.UtcNow - _lastRefreshUtc < RefreshInterval) return;
        RefreshNow(db, region);
    }

    public void RefreshNow(SqliteDatabaseManager db, RegionProfile region)
    {
        _lastRefreshUtc = DateTime.UtcNow;

        var pvp = db.GetTopPvpRankings(region, 50);
        var pvpList = new List<PvpRankRow>(pvp.Count);
        for (int i = 0; i < pvp.Count; i++)
        {
            var r = pvp[i];
            pvpList.Add(new PvpRankRow(i + 1, r.UnitUid, r.NickName, r.Rating, r.RPoint, r.Win, r.Lose));
        }
        _pvpRankings = pvpList;

        var lvls = db.GetTopLevelRankings(region, 50);
        var lvlList = new List<LevelRankRow>(lvls.Count);
        for (int i = 0; i < lvls.Count; i++)
        {
            var r = lvls[i];
            lvlList.Add(new LevelRankRow(i + 1, r.UnitUid, r.NickName, r.Level, r.Exp));
        }
        _levelRankings = lvlList;
    }

    public void BindToLua(KScriptManager script, string globalName = "RankingManager")
    {
        Table? proxy = null;
        proxy = script.CreateLuaProxy(globalName, t =>
        {
            t["AddHenirRankingInfo"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                int rType = KScriptManager.ArgInt(a, o);
                HenirRankingConfigs[rType] = new HenirRankingConfig(
                    rType,
                    KScriptManager.ArgInt(a, o + 1, 100),
                    KScriptManager.ArgInt(a, o + 2, 6),
                    KScriptManager.ArgInt(a, o + 3, 0),
                    KScriptManager.ArgInt(a, o + 4, 0));
                return DynValue.Nil;
            });
            t["AddRankingRewardInfo"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                int rType = KScriptManager.ArgInt(a, o);
                if (!RankingRewards.TryGetValue(rType, out var list))
                    RankingRewards[rType] = list = [];
                list.Add(new RankingRewardRule(
                    rType,
                    KScriptManager.ArgInt(a, o + 1),
                    KScriptManager.ArgInt(a, o + 2),
                    KScriptManager.ArgInt(a, o + 3, 1)));
                return DynValue.Nil;
            });
            t["SetRankingRewardBeginDate"] = DynValue.NewCallback((_, a) =>
            {
                RankingRewardBeginDate = KScriptManager.ArgString(a, KScriptManager.ArgStart(a, proxy!), RankingRewardBeginDate);
                return DynValue.Nil;
            });
        });
    }
}
