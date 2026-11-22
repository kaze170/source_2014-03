using System.Collections.Concurrent;

namespace KncWX2Server.LoginServer;

/// <summary>
/// Faithful C# port of LoginServer/GuildBoardManager.{h,cpp} (SERV_GUILD_AD):
/// guild advertisement board, sorting modes (REG_DATE, NEW_GUILD, HOT_GUILD, MEMBER_COUNT, GUILD_LEVEL),
/// pagination (6 per page), expiration tracking, and join-application lifecycle.
/// </summary>
public sealed class KGuildBoardManager
{
    public const int PagePerCount = 6;

    public enum AdSortType
    {
        RegDate = 0,
        NewGuild = 1,
        HotGuild = 2,
        MemberCount = 3,
        GuildLevel = 4,
    }

    public sealed class GuildAdEntry
    {
        public int GuildUid { get; init; }
        public string GuildName { get; set; } = string.Empty;
        public string MasterNickName { get; set; } = string.Empty;
        public byte GuildLevel { get; set; } = 1;
        public int GuildExp { get; set; }
        public ushort CurMemberCount { get; set; } = 1;
        public string AdMessage { get; set; } = string.Empty;
        public DateTime RegDateUtc { get; init; } = DateTime.UtcNow;
        public DateTime EndDateUtc { get; set; } = DateTime.UtcNow.AddDays(7);
        public int ApplyCount { get; set; }
        public bool IsExpired => DateTime.UtcNow >= EndDateUtc;
    }

    public sealed class ApplyJoinEntry
    {
        public int GuildUid { get; init; }
        public long UnitUid { get; init; }
        public string NickName { get; init; } = string.Empty;
        public byte UnitClass { get; init; }
        public byte Level { get; init; }
        public string Message { get; init; } = string.Empty;
        public DateTime AppliedAtUtc { get; init; } = DateTime.UtcNow;
    }

    private readonly ConcurrentDictionary<int, GuildAdEntry> _ads = new();
    private readonly ConcurrentDictionary<long, ApplyJoinEntry> _unitApplies = new(); // 1 active apply per UnitUid
    private readonly object _sync = new();

    public int ActiveAdCount => _ads.Values.Count(a => !a.IsExpired);

    public void NewGuildAd(GuildAdEntry ad)
    {
        lock (_sync)
            _ads[ad.GuildUid] = ad;
    }

    public bool IsExistGuildAd(int guildUid) => _ads.ContainsKey(guildUid);

    public bool IsExpiredGuildAd(int guildUid)
        => !_ads.TryGetValue(guildUid, out var ad) || ad.IsExpired;

    public void DeleteGuildAd(int guildUid)
    {
        lock (_sync)
        {
            _ads.TryRemove(guildUid, out _);
            foreach (var kv in _unitApplies.Where(kv => kv.Value.GuildUid == guildUid).ToList())
                _unitApplies.TryRemove(kv.Key, out _);
        }
    }

    public (List<GuildAdEntry> pageItems, int totalPages) GetGuildAdList(AdSortType sortType, int pageIndex)
    {
        var active = _ads.Values.Where(a => !a.IsExpired);
        IEnumerable<GuildAdEntry> sorted = sortType switch
        {
            AdSortType.NewGuild => active.OrderByDescending(a => a.RegDateUtc),
            AdSortType.HotGuild => active.OrderByDescending(a => a.ApplyCount).ThenByDescending(a => a.CurMemberCount),
            AdSortType.MemberCount => active.OrderByDescending(a => a.CurMemberCount).ThenByDescending(a => a.GuildLevel),
            AdSortType.GuildLevel => active.OrderByDescending(a => a.GuildLevel).ThenByDescending(a => a.GuildExp),
            _ => active.OrderByDescending(a => a.RegDateUtc),
        };
        var all = sorted.ToList();
        int totalPages = Math.Max(1, (all.Count + PagePerCount - 1) / PagePerCount);
        int clampedPage = Math.Clamp(pageIndex, 0, totalPages - 1);
        return (all.Skip(clampedPage * PagePerCount).Take(PagePerCount).ToList(), totalPages);
    }

    public bool ApplyJoinGuild(ApplyJoinEntry apply, out ApplyJoinEntry? replacedPrevious)
    {
        lock (_sync)
        {
            replacedPrevious = null;
            if (!_ads.TryGetValue(apply.GuildUid, out var ad) || ad.IsExpired)
                return false;

            if (_unitApplies.TryGetValue(apply.UnitUid, out var prev))
                replacedPrevious = prev;

            _unitApplies[apply.UnitUid] = apply;
            ad.ApplyCount = _unitApplies.Values.Count(a => a.GuildUid == apply.GuildUid);
            return true;
        }
    }

    public List<ApplyJoinEntry> GetApplyJoinGuildList(int guildUid)
        => _unitApplies.Values.Where(a => a.GuildUid == guildUid).OrderBy(a => a.AppliedAtUtc).ToList();

    public bool DeleteApplyJoinGuild(long unitUid)
        => _unitApplies.TryRemove(unitUid, out _);

    public int PruneExpiredAds(DateTime? nowUtc = null)
    {
        var now = nowUtc ?? DateTime.UtcNow;
        int removed = 0;
        foreach (var kv in _ads.Where(kv => now >= kv.Value.EndDateUtc).ToList())
        {
            if (_ads.TryRemove(kv.Key, out _))
                removed++;
        }
        return removed;
    }

    public void UpdateGuildLevel(int guildUid, byte newLevel)
    {
        if (_ads.TryGetValue(guildUid, out var ad)) ad.GuildLevel = newLevel;
    }

    public void UpdateGuildExp(int guildUid, int exp)
    {
        if (_ads.TryGetValue(guildUid, out var ad)) ad.GuildExp = exp;
    }

    public void UpdateGuildName(int guildUid, string name)
    {
        if (_ads.TryGetValue(guildUid, out var ad)) ad.GuildName = name;
    }

    public void UpdateGuildMasterNickName(int guildUid, string nick)
    {
        if (_ads.TryGetValue(guildUid, out var ad)) ad.MasterNickName = nick;
    }

    public void UpdateGuildCurMemberCount(int guildUid, ushort count)
    {
        if (_ads.TryGetValue(guildUid, out var ad)) ad.CurMemberCount = count;
    }
}
