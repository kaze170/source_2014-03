using KncWX2Server.Core.Packets;

namespace KncWX2Server.CenterServer;

/// <summary>
/// Faithful C# port of CenterServer/DungeonMonsterManager.{h,cpp}:
/// manages per-(StageId, SubStageId) NPC spawn lists, boss/normal sub-stage clear validation,
/// and monster die-count tracking by monster type factor.
/// </summary>
public sealed class KDungeonMonsterManager
{
    public enum MonsterTypeFactor : sbyte
    {
        NormalNpc = 0,     // MTF_NORMAL_NPC
        LowEliteNpc = 1,   // MTF_LOW_ELITE_NPC
        HighEliteNpc = 2,  // MTF_HIGH_ELITE_NPC
        MiddleBossNpc = 3, // MTF_MIDDLE_BOSS_NPC
        BossNpc = 4,       // MTF_BOSS_NPC
    }

    public sealed class SubStageNpcInfo
    {
        public HashSet<int> NormalNpcUids { get; } = [];
        public HashSet<int> MiddleBossNpcUids { get; } = [];
        public HashSet<int> BossNpcUids { get; } = [];
    }

    private readonly KRoomMonsterManager _roomMonsters;
    private readonly Dictionary<(int StageId, int SubStageId), SubStageNpcInfo> _subStageNpcInfo = new();

    public int NormalNpcDieCount { get; private set; }
    public int LowEliteNpcDieCount { get; private set; }
    public int HighEliteNpcDieCount { get; private set; }
    public int MiddleBossDieCount { get; private set; }
    public int BossDieCount { get; private set; }

    public KDungeonMonsterManager(KRoomMonsterManager roomMonsters)
    {
        _roomMonsters = roomMonsters;
    }

    public void StartGame()
    {
        ClearDieCount();
        _subStageNpcInfo.Clear();
    }

    public void EndGame()
    {
        ClearDieCount();
        _subStageNpcInfo.Clear();
    }

    public void OnCloseRoom()
    {
        ClearDieCount();
        _subStageNpcInfo.Clear();
    }

    private void ClearDieCount()
    {
        NormalNpcDieCount = 0;
        LowEliteNpcDieCount = 0;
        HighEliteNpcDieCount = 0;
        MiddleBossDieCount = 0;
        BossDieCount = 0;
    }

    public void IncreaseMonsterDieCount(MonsterTypeFactor factor)
    {
        switch (factor)
        {
            case MonsterTypeFactor.NormalNpc: NormalNpcDieCount++; break;
            case MonsterTypeFactor.LowEliteNpc: LowEliteNpcDieCount++; break;
            case MonsterTypeFactor.HighEliteNpc: HighEliteNpcDieCount++; break;
            case MonsterTypeFactor.MiddleBossNpc: MiddleBossDieCount++; break;
            case MonsterTypeFactor.BossNpc: BossDieCount++; break;
        }
    }

    public bool InsertSubStageNpcList(int stageId, int subStageId, IEnumerable<KNPCUnitReq> subStageNpcs)
    {
        var info = new SubStageNpcInfo();
        foreach (var npc in subStageNpcs)
        {
            var grade = (KRoomMonsterManager.MonsterGrade)npc.MonsterGrade;
            if (grade == KRoomMonsterManager.MonsterGrade.MG_BOSS_NPC)
                info.BossNpcUids.Add(npc.Uid);
            else if (grade == KRoomMonsterManager.MonsterGrade.MG_MIDDLE_BOSS_NPC)
                info.MiddleBossNpcUids.Add(npc.Uid);
            else if (npc.Active)
                info.NormalNpcUids.Add(npc.Uid);
        }

        _subStageNpcInfo[(stageId, subStageId)] = info;
        return true;
    }

    public List<int> GetSubStageNpcList(int stageId, int subStageId)
    {
        var result = new List<int>();
        if (!_subStageNpcInfo.TryGetValue((stageId, subStageId), out var info))
            return result;

        foreach (int uid in info.NormalNpcUids)
        {
            if (_roomMonsters.GetNpcData(uid, out var data) && data != null)
                result.Add(data.NpcId);
        }
        foreach (int uid in info.MiddleBossNpcUids)
        {
            if (_roomMonsters.GetNpcData(uid, out var data) && data != null)
                result.Add(data.NpcId);
        }
        foreach (int uid in info.BossNpcUids)
        {
            if (_roomMonsters.GetNpcData(uid, out var data) && data != null)
                result.Add(data.NpcId);
        }
        return result;
    }

    public bool IsAllSubStageNpcDie(int stageId, int subStageId)
    {
        if (!_subStageNpcInfo.TryGetValue((stageId, subStageId), out var info))
            return false;

        var targetSet = info.BossNpcUids.Count > 0 ? info.BossNpcUids : info.NormalNpcUids;
        foreach (int npcUid in targetSet)
        {
            if (!_roomMonsters.IsExistMonster(npcUid, out bool isDie))
                continue;
            if (!isDie)
                return false;
        }
        return true;
    }
}
