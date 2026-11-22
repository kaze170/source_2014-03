namespace KncWX2Server.CenterServer;

/// <summary>
/// Faithful C# port of CenterServer/RoomMonsterManager.{h,cpp}:
/// assigns per-room NPC UIDs, tracks alive vs defeated monsters, and aggregates
/// grade counts (Normal, LowElite, HighElite, MiddleBoss, Boss) used by ResultData_new.lua rank math.
/// </summary>
public sealed class KRoomMonsterManager
{
    public enum MonsterGrade : sbyte
    {
        MG_NORMAL_NPC = 0,
        MG_LOW_ELITE_NPC = 1,
        MG_HIGH_ELITE_NPC = 2,
        MG_MIDDLE_BOSS_NPC = 3,
        MG_BOSS_NPC = 4,
    }

    public sealed class NpcData
    {
        public int NpcUid { get; init; }
        public int NpcId { get; init; }
        public byte Level { get; init; }
        public MonsterGrade Grade { get; init; }
        public bool IsBoss => Grade == MonsterGrade.MG_BOSS_NPC;
        public bool NoDrop { get; init; }
        public int GroupId { get; init; }
        public bool IsAttribNpc { get; set; }
    }

    private readonly object _lock = new();
    private readonly Dictionary<int, NpcData> _aliveNpcs = new();
    private readonly Dictionary<int, NpcData> _deadNpcs = new();
    private int _nextNpcUid = 1;

    public int NormalNpcDieCount { get; private set; }
    public int LowEliteNpcDieCount { get; private set; }
    public int HighEliteNpcDieCount { get; private set; }
    public int MiddleBossDieCount { get; private set; }
    public int BossDieCount { get; private set; }
    public int AliveCount { get { lock (_lock) return _aliveNpcs.Count; } }

    public bool IsAliveMonster(int npcUid) { lock (_lock) return _aliveNpcs.ContainsKey(npcUid); }

    public bool GetNpcData(int npcUid, out NpcData? data)
    {
        lock (_lock)
        {
            return _aliveNpcs.TryGetValue(npcUid, out data) || _deadNpcs.TryGetValue(npcUid, out data);
        }
    }

    public bool IsExistMonster(int npcUid, out bool isDie)
    {
        lock (_lock)
        {
            if (_aliveNpcs.ContainsKey(npcUid))
            {
                isDie = false;
                return true;
            }
            if (_deadNpcs.ContainsKey(npcUid))
            {
                isDie = true;
                return true;
            }
            isDie = false;
            return false;
        }
    }

    public void OnCloseRoom() => EndGame();

    public void StartGame()
    {
        lock (_lock)
        {
            _nextNpcUid = 1;
            _aliveNpcs.Clear();
            _deadNpcs.Clear();
            NormalNpcDieCount = 0;
            LowEliteNpcDieCount = 0;
            HighEliteNpcDieCount = 0;
            MiddleBossDieCount = 0;
            BossDieCount = 0;
        }
    }

    public void EndGame()
    {
        lock (_lock)
        {
            _aliveNpcs.Clear();
            _deadNpcs.Clear();
        }
    }

    public int CreateMonster(int npcId, byte level, MonsterGrade grade, bool noDrop = false, int groupId = 0, int explicitNpcUid = 0)
    {
        lock (_lock)
        {
            int uid = explicitNpcUid != 0 ? explicitNpcUid : _nextNpcUid++;
            _aliveNpcs[uid] = new NpcData
            {
                NpcUid = uid,
                NpcId = npcId,
                Level = level,
                Grade = grade,
                NoDrop = noDrop,
                GroupId = groupId,
            };
            return uid;
        }
    }

    public bool TryKillMonster(int npcUid, out NpcData? killed)
    {
        lock (_lock)
        {
            if (!_aliveNpcs.Remove(npcUid, out killed))
                return false;

            _deadNpcs[npcUid] = killed;
            switch (killed.Grade)
            {
                case MonsterGrade.MG_NORMAL_NPC: NormalNpcDieCount++; break;
                case MonsterGrade.MG_LOW_ELITE_NPC: LowEliteNpcDieCount++; break;
                case MonsterGrade.MG_HIGH_ELITE_NPC: HighEliteNpcDieCount++; break;
                case MonsterGrade.MG_MIDDLE_BOSS_NPC: MiddleBossDieCount++; break;
                case MonsterGrade.MG_BOSS_NPC: BossDieCount++; break;
            }
            return true;
        }
    }

    /// <summary>
    /// Exact weighted time factor from ResultData_new.lua:
    /// Normal*1 + LowElite*2 + HighElite*3 + MiddleBoss*4 + Boss*12.
    /// </summary>
    public int GetMonsterWeightScore()
    {
        lock (_lock)
        {
            return (NormalNpcDieCount * 1) +
                   (LowEliteNpcDieCount * 2) +
                   (HighEliteNpcDieCount * 3) +
                   (MiddleBossDieCount * 4) +
                   (BossDieCount * 12);
        }
    }
}
