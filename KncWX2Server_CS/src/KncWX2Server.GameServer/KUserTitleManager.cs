using KncWX2Server.Core.Database;

namespace KncWX2Server.GameServer;

/// <summary>
/// Faithful C# port of GameServer/UserTitleManager.{h,cpp}:
/// tracks active title missions, unlocks titles upon mission completion, and equips/clears titles.
/// </summary>
public sealed class KUserTitleManager
{
    private readonly Dictionary<int, int> _missionProgress = new(); // MissionId -> ClearCount
    private readonly HashSet<int> _unlockedTitles = new();
    public int EquippedTitleId { get; private set; }
    public bool UsePcBangTitle { get; private set; }

    public void Init(int equippedTitleId, IEnumerable<int>? unlockedTitles = null, bool usePcBangTitle = false)
    {
        EquippedTitleId = equippedTitleId;
        UsePcBangTitle = usePcBangTitle;
        _unlockedTitles.Clear();
        if (unlockedTitles != null)
        {
            foreach (int t in unlockedTitles)
                _unlockedTitles.Add(t);
        }
    }

    public void UnlockTitle(SqliteDatabaseManager db, long unitUid, int titleId)
    {
        if (titleId <= 0) return;
        _unlockedTitles.Add(titleId);
        db.GrantUnitTitle(unitUid, titleId);
    }

    public bool RecordMissionProgress(SqliteDatabaseManager db, long unitUid, int titleId, int delta, int requiredCount)
    {
        int next = _missionProgress.GetValueOrDefault(titleId, 0) + Math.Max(1, delta);
        _missionProgress[titleId] = next;
        if (next >= requiredCount && !_unlockedTitles.Contains(titleId))
        {
            UnlockTitle(db, unitUid, titleId);
            return true;
        }
        return false;
    }

    public bool EquipTitle(SqliteDatabaseManager db, long unitUid, int titleId)
    {
        if (titleId > 0 && !_unlockedTitles.Contains(titleId))
        {
            // Check DB in case granted externally
            if (!db.EquipUnitTitle(unitUid, titleId))
                return false;
            _unlockedTitles.Add(titleId);
        }
        else if (!db.EquipUnitTitle(unitUid, titleId))
        {
            return false;
        }
        EquippedTitleId = titleId;
        return true;
    }

    public bool HasTitle(int titleId) => _unlockedTitles.Contains(titleId);

    public List<KncWX2Server.Core.Packets.KTitleInfo> GetTitles()
        => _unlockedTitles.Select(id => new KncWX2Server.Core.Packets.KTitleInfo
        {
            TitleId = id,
            Infinity = true,
        }).ToList();

    public List<KncWX2Server.Core.Packets.KMissionInstance> GetMissions()
        => _missionProgress.Select(kv => new KncWX2Server.Core.Packets.KMissionInstance
        {
            Id = kv.Key,
            SubMissionInstance =
            [
                new KncWX2Server.Core.Packets.KSubMissionInstance
                {
                    Id = kv.Key,
                    ClearData = (short)Math.Clamp(kv.Value, 0, short.MaxValue),
                },
            ],
        }).ToList();
}
