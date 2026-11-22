using KncWX2Server.Core.Region;
using KncWX2Server.Core.Serialization;

namespace KncWX2Server.Core.Events;

/// <summary>
/// Wire-exact port of KPerformerInfo (Common/Event.{h,cpp}).
/// </summary>
public sealed class KPerformerInfo : IKSerializable
{
    public const int MaxUidNum = 2000;

    public uint PerformerId { get; set; }
    private readonly SortedSet<long> _uids = [];

    public IReadOnlyCollection<long> Uids => _uids;

    public bool FindUID(long uid) => _uids.Contains(uid);

    public bool AddUID(long uid)
    {
        if (_uids.Count >= MaxUidNum)
            return false;
        return _uids.Add(uid);
    }

    public long GetFirstUID() => _uids.Count > 0 ? _uids.Min : 0;

    public void ClearUIDs() => _uids.Clear();

    public KPerformerInfo Clone()
    {
        var copy = new KPerformerInfo { PerformerId = PerformerId };
        foreach (var u in _uids)
            copy._uids.Add(u);
        return copy;
    }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PerformerId)) return false;
        if (!ks.Put((uint)_uids.Count)) return false;
        foreach (long uid in _uids)
        {
            if (!ks.Put(uid)) return false;
        }
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        _uids.Clear();
        if (!ks.Get(out uint pid)) return false;
        PerformerId = pid;

        if (!ks.Get(out uint count) || count > MaxUidNum) return false;
        for (uint i = 0; i < count; i++)
        {
            if (!ks.Get(out long uid)) return false;
            _uids.Add(uid);
        }
        return true;
    }
}
