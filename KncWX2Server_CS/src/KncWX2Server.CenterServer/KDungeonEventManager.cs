using KncWX2Server.Core.Packets;

namespace KncWX2Server.CenterServer;

/// <summary>
/// Faithful C# port of CenterServer/DungeonEventManager.{h,cpp} (SERV_TIME_DROP_MONSTER_EVENT):
/// schedules timed dungeon event monsters and injects them into a random active substage
/// of KEGS_DUNGEON_STAGE_LOAD_NOT.
/// </summary>
public sealed class KDungeonEventManager
{
    public sealed record TimeDropEventMonsterInfo(int EventUid, int NpcId, DateTime DropDateUtc, byte Level = 1);

    private readonly SortedList<DateTime, TimeDropEventMonsterInfo> _scheduledMonsters = new();
    private readonly Random _rng = new();
    private readonly object _sync = new();
    private int _nextEventNpcUid = 900_000;

    public int PendingCount
    {
        get { lock (_sync) return _scheduledMonsters.Count; }
    }

    public void UpdateTimeDropEventMonsterInfo(IEnumerable<TimeDropEventMonsterInfo> entries)
    {
        lock (_sync)
        {
            foreach (var e in entries)
            {
                var key = e.DropDateUtc;
                while (_scheduledMonsters.ContainsKey(key))
                    key = key.AddTicks(1);
                _scheduledMonsters[key] = e;
            }
        }
    }

    public bool CheckTimeDropEventMonster(
        KEGS_DUNGEON_STAGE_LOAD_NOT stageNot,
        out int eventUid,
        out int npcId,
        DateTime? nowUtc = null)
    {
        eventUid = 0;
        npcId = 0;
        if (stageNot.NpcData.Count == 0)
            return false;

        var now = nowUtc ?? DateTime.UtcNow;
        TimeDropEventMonsterInfo? ready = null;
        lock (_sync)
        {
            if (_scheduledMonsters.Count == 0) return false;
            var first = _scheduledMonsters.ElementAt(0);
            if (now < first.Key) return false;
            ready = first.Value;
            _scheduledMonsters.RemoveAt(0);
        }

        eventUid = ready.EventUid;
        npcId = ready.NpcId;

        var subStages = stageNot.NpcData.Keys.ToList();
        int pickedSubStage = subStages[_rng.Next(subStages.Count)];
        var npc = new KNPCUnitReq
        {
            Uid = Interlocked.Increment(ref _nextEventNpcUid),
            NpcId = ready.NpcId,
            Level = ready.Level,
            StartPos = 1,
            Aggressive = true,
        };
        stageNot.NpcData[pickedSubStage].NpcList.Add(npc);
        return true;
    }
}
