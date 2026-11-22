using KncWX2Server.Core.Packets;
using KncWX2Server.Core.Region;

namespace KncWX2Server.GameServer;

/// <summary>
/// Faithful C# port of GameServer/TutorialManager.{h,cpp}:
/// maintains the open student pool (Level &lt;= 10), serves 5-per-page queries,
/// removes students upon reaching Level 20, and maps dismissal reasons to NetError codes.
/// </summary>
public sealed class KTutorialManager
{
    public const int UnitLevelStudentMax = 10;
    public const int UnitLevelTeacherMin = 20;
    public const int StudentDisconnectDays = 7;
    public const int StudentPageNum = 5;
    public const int StudentNum = 3;
    public const int TeacherNum = 1;
    public const int RewardGraduation = 6;

    public enum TutorialDelReason
    {
        TdrDisconnectDay = 0,
        TdrLevelUpEnd = 1,
        TdrDeleteUnit = 2,
        TdrRefusalUser = 3,
    }

    private readonly List<KTutorialUnitInfo> _studentPool = [];
    private readonly object _sync = new();

    public int StudentPoolCount
    {
        get { lock (_sync) return _studentPool.Count; }
    }

    public bool IsExistByList(long unitUid)
    {
        lock (_sync) return _studentPool.Any(u => u.UnitUid == unitUid);
    }

    public bool AddUnit(KTutorialUnitInfo info)
    {
        if (info.Level > UnitLevelStudentMax)
            return false;
        lock (_sync)
        {
            if (_studentPool.Any(u => u.UnitUid == info.UnitUid))
                return false;
            _studentPool.Add(info);
            return true;
        }
    }

    public bool DeleteUnit(long unitUid)
    {
        lock (_sync)
            return _studentPool.RemoveAll(u => u.UnitUid == unitUid) > 0;
    }

    public KEGS_TUTORIAL_STUDENT_LIST_ACK GetTutorPageInfo(int viewPage)
    {
        lock (_sync)
        {
            if (_studentPool.Count == 0)
            {
                return new KEGS_TUTORIAL_STUDENT_LIST_ACK
                {
                    Ok = NetError.NET_OK,
                    TotalPage = 1,
                    ViewPage = 1,
                };
            }

            int totalPage = _studentPool.Count / StudentPageNum;
            if (totalPage <= 0) totalPage = 1;
            if (_studentPool.Count % StudentPageNum > 0 && _studentPool.Count > StudentPageNum)
                totalPage++;

            int clampedView = Math.Clamp(viewPage, 1, totalPage);
            int start = StudentPageNum * (clampedView - 1);
            var slice = _studentPool.Skip(start).Take(StudentPageNum).ToList();

            var ack = new KEGS_TUTORIAL_STUDENT_LIST_ACK
            {
                Ok = NetError.NET_OK,
                TotalPage = totalPage,
                ViewPage = clampedView,
            };
            ack.TutorialStudentList.AddRange(slice);
            return ack;
        }
    }

    public bool SetLevelData(long unitUid, byte level)
    {
        lock (_sync)
        {
            int idx = _studentPool.FindIndex(u => u.UnitUid == unitUid);
            if (idx < 0) return false;
            _studentPool[idx].Level = level;
            if (level >= UnitLevelTeacherMin)
                _studentPool.RemoveAt(idx);
            return true;
        }
    }

    public static int GetDelReason(TutorialDelReason reason) => reason switch
    {
        TutorialDelReason.TdrDisconnectDay => NetError.ERR_TUTORIAL_07,
        TutorialDelReason.TdrLevelUpEnd => NetError.ERR_TUTORIAL_08,
        TutorialDelReason.TdrDeleteUnit => NetError.ERR_TUTORIAL_09,
        TutorialDelReason.TdrRefusalUser => NetError.ERR_TUTORIAL_10,
        _ => NetError.ERR_UNKNOWN,
    };
}
