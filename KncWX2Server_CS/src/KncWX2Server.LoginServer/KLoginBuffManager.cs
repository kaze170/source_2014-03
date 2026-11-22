namespace KncWX2Server.LoginServer;

/// <summary>
/// Faithful C# port of LoginServer/BuffManager.{h,cpp} (SERV_SERVER_BUFF_SYSTEM):
/// caches per-unit active server buffs across server transitions and purges entries older than
/// UBKT_UNIT_RESET_TIME_HOUR (6 hours).
/// </summary>
public sealed class KLoginBuffManager
{
    public const int UnitResetTimeHours = 6; // CXSLBuffManager::UBKT_UNIT_RESET_TIME_HOUR

    public sealed class CachedBuffInfo
    {
        public int BuffTempletId { get; init; }
        public int FactorId { get; init; }
        public long BuffStartTime { get; init; }
        public long BuffEndTime { get; init; }
    }

    public sealed class UnitBuffEntry
    {
        public long InsertUnixTime { get; set; }
        public Dictionary<int, CachedBuffInfo> Buffs { get; } = new();
    }

    private readonly Dictionary<long, UnitBuffEntry> _unitBuffs = new();
    private readonly SortedDictionary<long, List<long>> _timeIndex = new();
    private readonly object _sync = new();

    public int CachedUnitCount
    {
        get { lock (_sync) return _unitBuffs.Count; }
    }

    public bool AddUnitBuffInfo(long unitUid, IEnumerable<CachedBuffInfo> buffs, long? unixTimeOverride = null)
    {
        long nowUnix = unixTimeOverride ?? DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        lock (_sync)
        {
            if (!_unitBuffs.TryGetValue(unitUid, out var entry))
            {
                entry = new UnitBuffEntry { InsertUnixTime = nowUnix };
                foreach (var b in buffs)
                    entry.Buffs[b.BuffTempletId] = b;
                _unitBuffs[unitUid] = entry;
                AddRenewBuffTimeInternal(unitUid, nowUnix, 0);
                return true;
            }

            foreach (var b in buffs)
                entry.Buffs[b.BuffTempletId] = b;

            long oldTime = entry.InsertUnixTime;
            if (AddRenewBuffTimeInternal(unitUid, nowUnix, oldTime))
                entry.InsertUnixTime = nowUnix;
            return true;
        }
    }

    public bool DeleteUnitBuffInfo(long unitUid, IEnumerable<int> buffIds)
    {
        lock (_sync)
        {
            if (!_unitBuffs.TryGetValue(unitUid, out var entry))
                return false;

            foreach (int id in buffIds)
                entry.Buffs.Remove(id);

            if (entry.Buffs.Count <= 0)
            {
                DeleteBuffTimeInternal(entry.InsertUnixTime, unitUid);
                _unitBuffs.Remove(unitUid);
            }
            return true;
        }
    }

    public bool DeleteUnitAllBuffInfo(long unitUid)
    {
        lock (_sync)
        {
            if (!_unitBuffs.TryGetValue(unitUid, out var entry))
                return false;
            DeleteBuffTimeInternal(entry.InsertUnixTime, unitUid);
            _unitBuffs.Remove(unitUid);
            return true;
        }
    }

    public bool GetUnitAllBuffInfo(long unitUid, out UnitBuffEntry? entry, long? unixTimeOverride = null)
    {
        long nowUnix = unixTimeOverride ?? DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        lock (_sync)
        {
            if (!_unitBuffs.TryGetValue(unitUid, out entry))
                return false;

            long oldTime = entry.InsertUnixTime;
            if (AddRenewBuffTimeInternal(unitUid, nowUnix, oldTime))
                entry.InsertUnixTime = nowUnix;
            return true;
        }
    }

    public int CheckEndBuff(long? unixTimeOverride = null)
    {
        long cutoffUnix = (unixTimeOverride ?? DateTimeOffset.UtcNow.ToUnixTimeSeconds()) - (UnitResetTimeHours * 3600L);
        var toDelete = new HashSet<long>();

        lock (_sync)
        {
            foreach (var kv in _timeIndex)
            {
                if (kv.Key > cutoffUnix) break;
                foreach (long uid in kv.Value)
                    toDelete.Add(uid);
            }

            foreach (long uid in toDelete)
            {
                if (_unitBuffs.TryGetValue(uid, out var entry))
                {
                    DeleteBuffTimeInternal(entry.InsertUnixTime, uid);
                    _unitBuffs.Remove(uid);
                }
            }
        }
        return toDelete.Count;
    }

    private bool AddRenewBuffTimeInternal(long unitUid, long newTime, long oldTime)
    {
        if (oldTime > 0 && !DeleteBuffTimeInternal(oldTime, unitUid))
            return false;

        if (!_timeIndex.TryGetValue(newTime, out var list))
        {
            list = [];
            _timeIndex[newTime] = list;
        }
        if (!list.Contains(unitUid))
            list.Add(unitUid);
        return true;
    }

    private bool DeleteBuffTimeInternal(long time, long unitUid)
    {
        if (!_timeIndex.TryGetValue(time, out var list))
            return false;
        list.Remove(unitUid);
        if (list.Count == 0)
            _timeIndex.Remove(time);
        return true;
    }
}
