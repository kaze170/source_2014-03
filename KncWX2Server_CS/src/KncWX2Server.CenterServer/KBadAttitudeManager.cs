namespace KncWX2Server.CenterServer;

/// <summary>
/// Faithful C# port of CenterServer/BadAttitudeManager.{h,cpp}:
/// tracks AFK/forced-exit vote points per unit in dungeon rooms and cleans up votes when voters leave.
/// </summary>
public sealed class KBadAttitudeManager
{
    public sealed class BadAttitudeInfo
    {
        public long UnitUid { get; init; }
        public int BadAttitudePoint { get; set; }
        public int VotePoint { get; set; }
        public int LastStageId { get; set; } = -1;
        public int LastSubStageId { get; set; } = -1;
        public int LastComboScore { get; set; }
        public int LastTechScore { get; set; }
        public HashSet<long> VotedUnitUids { get; } = new();
    }

    private readonly object _lock = new();
    private readonly Dictionary<long, BadAttitudeInfo> _units = new();

    public void Init(IEnumerable<long> memberUnitUids)
    {
        lock (_lock)
        {
            _units.Clear();
            foreach (long uid in memberUnitUids)
                _units[uid] = new BadAttitudeInfo { UnitUid = uid };
        }
    }

    public bool RemoveUnit(long unitUid, out BadAttitudeInfo? removed)
    {
        lock (_lock)
        {
            if (!_units.Remove(unitUid, out removed))
                return false;

            // Remove any vote points cast by the departing unit (exact C++ logic)
            foreach (var other in _units.Values)
            {
                if (other.VotedUnitUids.Remove(unitUid) && other.VotePoint > 0)
                    other.VotePoint--;
            }
            return true;
        }
    }

    public void IncreaseBadAttitudePoint(long unitUid, int delta = 1)
    {
        lock (_lock)
        {
            if (_units.TryGetValue(unitUid, out var info))
                info.BadAttitudePoint += Math.Max(1, delta);
        }
    }

    public void UpdateUserCheckInfo(long unitUid, int stageId, int subStageId, int comboScore, int techScore)
    {
        if (stageId < 0 || subStageId < 0) return;
        lock (_lock)
        {
            if (!_units.TryGetValue(unitUid, out var info))
            {
                info = new BadAttitudeInfo { UnitUid = unitUid };
                _units[unitUid] = info;
            }
            if (stageId < info.LastStageId && subStageId < info.LastSubStageId)
                return;
            info.LastStageId = stageId;
            info.LastSubStageId = subStageId;
            info.LastComboScore = comboScore;
            info.LastTechScore = techScore;
            if (comboScore == 0 && techScore == 0)
                info.BadAttitudePoint++;
        }
    }

    public (bool ok, bool shouldForceKick) VoteForcedExit(long voterUnitUid, long targetUnitUid)
    {
        lock (_lock)
        {
            if (voterUnitUid == targetUnitUid ||
                !_units.ContainsKey(voterUnitUid) ||
                !_units.TryGetValue(targetUnitUid, out var target))
                return (false, false);

            if (!target.VotedUnitUids.Add(voterUnitUid))
                return (false, false);

            target.VotePoint++;
            int activeCount = _units.Count;
            bool kick = activeCount >= 3 && target.VotePoint >= (activeCount - 1);
            return (true, kick);
        }
    }
}
