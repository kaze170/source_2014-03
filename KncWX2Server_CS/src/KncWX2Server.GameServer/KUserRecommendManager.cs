using KncWX2Server.Core.Packets;

namespace KncWX2Server.GameServer;

/// <summary>
/// Faithful C# port of GameServer/UserRecommendManager.{h,cpp}:
/// manages pagination (`PAGE_PER_COUNT = 8`), real-time online/level sync of recommended
/// and recruit/recruiter lists.
/// </summary>
public sealed class KUserRecommendManager
{
    public const int PagePerCount = 8;

    private readonly List<KRecommendUserInfo> _recommendUsers = [];
    private readonly List<KRecommendUserInfo> _recruitUnits = [];
    private readonly List<KRecommendUserInfo> _recruiterUnits = [];

    public bool IsRecommend { get; set; }
    public long RecommendedUnitUid { get; set; }
    public bool IsInitRecommendUserList { get; private set; }
    public bool IsInitRecruitRecruiterList { get; private set; }
    public bool IsRecruit => _recruiterUnits.Count > 0;

    public void Clear()
    {
        RecommendedUnitUid = 0;
        IsInitRecommendUserList = false;
        _recommendUsers.Clear();
        IsInitRecruitRecruiterList = false;
        _recruitUnits.Clear();
        _recruiterUnits.Clear();
    }

    public void SetRecommendUserList(IEnumerable<KRecommendUserInfo> users)
    {
        IsInitRecommendUserList = true;
        _recommendUsers.Clear();
        _recommendUsers.AddRange(users);
    }

    public (uint viewPage, uint totalPage, List<KRecommendUserInfo> pageItems) GetRecommendUserList(uint viewPage)
    {
        if (viewPage == 0) viewPage = 1;
        if (_recommendUsers.Count == 0)
            return (1, 1, []);

        uint totalPage = (uint)((_recommendUsers.Count + PagePerCount - 1) / PagePerCount);
        uint clamped = Math.Clamp(viewPage, 1u, totalPage);
        int start = (int)((clamped - 1) * PagePerCount);
        var slice = _recommendUsers.Skip(start).Take(PagePerCount).ToList();
        return (clamped, totalPage, slice);
    }

    public void AddRecommendUser(KRecommendUserInfo info)
    {
        if (!IsInitRecommendUserList) return;
        _recommendUsers.Add(info);
    }

    public bool UpdateRecommendStatus(long unitUid, bool? isOnline = null, byte? newLevel = null)
    {
        var item = _recommendUsers.FirstOrDefault(u => u.UnitUid == unitUid);
        if (item == null) return false;
        if (isOnline.HasValue) item.IsOnline = isOnline.Value;
        if (newLevel.HasValue) item.Level = newLevel.Value;
        return true;
    }

    public void SetRecruitUnitList(IEnumerable<KRecommendUserInfo> list)
    {
        IsInitRecruitRecruiterList = true;
        _recruitUnits.Clear();
        _recruitUnits.AddRange(list);
    }

    public void SetRecruiterUnitList(IEnumerable<KRecommendUserInfo> list)
    {
        IsInitRecruitRecruiterList = true;
        _recruiterUnits.Clear();
        _recruiterUnits.AddRange(list);
    }

    public IReadOnlyList<KRecommendUserInfo> GetRecruitUnitList() => _recruitUnits;
    public IReadOnlyList<KRecommendUserInfo> GetRecruiterUnitList() => _recruiterUnits;

    public void ModifyRecruitRecruiterState(KELG_RECRUIT_RECRUITER_INFO_NOT info)
    {
        if (!IsInitRecruitRecruiterList) return;

        foreach (var list in new[] { _recruitUnits, _recruiterUnits })
        {
            var entry = list.FirstOrDefault(u => u.UnitUid == info.ReceiverUnitUid);
            if (entry == null) continue;
            switch (info.Type)
            {
                case KRecommendUserInfo.RuitLogin: entry.IsOnline = true; break;
                case KRecommendUserInfo.RuitLogout: entry.IsOnline = false; break;
                case KRecommendUserInfo.RuitLevelUp: entry.Level = (byte)Math.Clamp(info.Data, 0, byte.MaxValue); break;
            }
        }
    }
}
