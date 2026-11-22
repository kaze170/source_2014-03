namespace KncWX2Server.CenterServer;

/// <summary>
/// Faithful C# port of CenterServer/BattleFieldMonsterManager.{h,cpp} (SERV_BATTLE_FIELD_SYSTEM &amp; SERV_BATTLE_FIELD_BOSS):
/// tracks BattleField monster death counts (Normal/LowElite/HighElite/MiddleBoss/Boss),
/// respawn timers, dangerousness progression, and alive field-boss NPCs.
/// </summary>
public sealed class KBattleFieldMonsterManager
{
    public sealed class NpcDieCount
    {
        public int NormalNpcDieCount { get; set; }
        public int LowEliteNpcDieCount { get; set; }
        public int HighEliteNpcDieCount { get; set; }
        public int MiddleBossDieCount { get; set; }
        public int BossDieCount { get; set; }

        public void Clear()
        {
            NormalNpcDieCount = 0;
            LowEliteNpcDieCount = 0;
            HighEliteNpcDieCount = 0;
            MiddleBossDieCount = 0;
            BossDieCount = 0;
        }
    }

    public sealed class MonsterRespawnInfo
    {
        public int MonsterGroupId { get; init; }
        public double RespawnSeconds { get; init; }
        public DateTime QueuedAtUtc { get; init; } = DateTime.UtcNow;

        public bool IsRespawnTimeOver(DateTime? nowUtc = null)
            => ((nowUtc ?? DateTime.UtcNow) - QueuedAtUtc).TotalSeconds > RespawnSeconds;
    }

    private readonly List<MonsterRespawnInfo> _respawnQueue = [];
    private readonly HashSet<int> _aliveBossNpcUids = [];
    private readonly object _sync = new();

    public int BattleFieldId { get; private set; }
    public int DangerousValue { get; private set; }
    public bool IsBossField { get; private set; }
    public NpcDieCount DieCounts { get; } = new();

    public void StartGame(int battleFieldId, int playerCount, int dangerousValue, bool isBossField)
    {
        lock (_sync)
        {
            BattleFieldId = battleFieldId;
            DangerousValue = Math.Clamp(dangerousValue, 0, byte.MaxValue);
            IsBossField = isBossField;
            DieCounts.Clear();
            _respawnQueue.Clear();
            _aliveBossNpcUids.Clear();
            if (isBossField)
                _aliveBossNpcUids.Add(battleFieldId * 10 + 1);
        }
    }

    public void EndGame()
    {
        lock (_sync)
        {
            DieCounts.Clear();
            _respawnQueue.Clear();
            _aliveBossNpcUids.Clear();
        }
    }

    public bool IsAliveBossMonster(int npcUid)
    {
        lock (_sync) return _aliveBossNpcUids.Contains(npcUid);
    }

    public void RecordNpcDie(int npcUid, int gradeTier, int monsterGroupId, double respawnSeconds = 8.0)
    {
        lock (_sync)
        {
            switch (gradeTier)
            {
                case 1: DieCounts.LowEliteNpcDieCount++; DangerousValue = Math.Min(255, DangerousValue + 2); break;
                case 2: DieCounts.HighEliteNpcDieCount++; DangerousValue = Math.Min(255, DangerousValue + 4); break;
                case 3: DieCounts.MiddleBossDieCount++; DangerousValue = Math.Min(255, DangerousValue + 10); break;
                case 4:
                    DieCounts.BossDieCount++;
                    _aliveBossNpcUids.Remove(npcUid);
                    break;
                default: DieCounts.NormalNpcDieCount++; DangerousValue = Math.Min(255, DangerousValue + 1); break;
            }
            if (monsterGroupId > 0)
                _respawnQueue.Add(new MonsterRespawnInfo { MonsterGroupId = monsterGroupId, RespawnSeconds = respawnSeconds });
        }
    }

    public List<int> PopReadyRespawnGroups(DateTime? nowUtc = null)
    {
        var now = nowUtc ?? DateTime.UtcNow;
        lock (_sync)
        {
            var ready = new List<int>();
            for (int i = _respawnQueue.Count - 1; i >= 0; i--)
            {
                if (_respawnQueue[i].IsRespawnTimeOver(now))
                {
                    ready.Add(_respawnQueue[i].MonsterGroupId);
                    _respawnQueue.RemoveAt(i);
                }
            }
            return ready;
        }
    }
}
