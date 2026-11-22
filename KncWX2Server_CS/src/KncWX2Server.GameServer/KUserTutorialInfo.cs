using KncWX2Server.Core.Packets;

namespace KncWX2Server.GameServer;

/// <summary>
/// Faithful C# port of GameServer/UserTutorialInfo.{h,cpp}:
/// stores a character's teacher or student relationships (max 3 students / 1 teacher)
/// and processes graduation at level 20.
/// </summary>
public sealed class KUserTutorialInfo
{
    public enum TutorialUnitType
    {
        TutNone = -1,
        TutTeacher = 0,
        TutStudent = 1,
    }

    private readonly List<KTutorialUnitInfo> _tutorInfos = [];

    public TutorialUnitType TutorUnitType { get; private set; } = TutorialUnitType.TutNone;
    public long DeleteUnitUid { get; set; }

    public bool IsExistUnit(long unitUid) => _tutorInfos.Any(u => u.UnitUid == unitUid);
    public bool IsExistByTutorData() => _tutorInfos.Count > 0;
    public int GetTutorialUnitNum() => _tutorInfos.Count;
    public List<long> GetTutorUnitUidList() => _tutorInfos.Select(u => u.UnitUid).ToList();
    public IReadOnlyList<KTutorialUnitInfo> SnapshotRelations() => _tutorInfos;

    public void ResetData()
    {
        _tutorInfos.Clear();
        TutorUnitType = TutorialUnitType.TutNone;
        DeleteUnitUid = 0;
    }

    public void InitFromDb(byte selfLevel, IEnumerable<KTutorialUnitInfo> relations)
    {
        ResetData();
        _tutorInfos.AddRange(relations);
        if (_tutorInfos.Count > 0)
        {
            TutorUnitType = selfLevel >= KTutorialManager.UnitLevelTeacherMin
                ? TutorialUnitType.TutStudent
                : TutorialUnitType.TutTeacher;
        }
    }

    public bool AddRelation(TutorialUnitType roleOfPartner, KTutorialUnitInfo info)
    {
        if (IsExistUnit(info.UnitUid)) return false;
        int limit = roleOfPartner == TutorialUnitType.TutStudent
            ? KTutorialManager.StudentNum
            : KTutorialManager.TeacherNum;
        if (_tutorInfos.Count >= limit) return false;

        TutorUnitType = roleOfPartner;
        _tutorInfos.Add(info);
        return true;
    }

    public bool RemoveRelation(long unitUid)
    {
        bool removed = _tutorInfos.RemoveAll(u => u.UnitUid == unitUid) > 0;
        if (_tutorInfos.Count == 0) TutorUnitType = TutorialUnitType.TutNone;
        return removed;
    }

    public bool CheckGraduationOnLevelUp(byte beforeLevel, byte newLevel)
    {
        return beforeLevel < KTutorialManager.UnitLevelTeacherMin
            && newLevel >= KTutorialManager.UnitLevelTeacherMin
            && TutorUnitType == TutorialUnitType.TutTeacher
            && _tutorInfos.Count > 0;
    }
}
