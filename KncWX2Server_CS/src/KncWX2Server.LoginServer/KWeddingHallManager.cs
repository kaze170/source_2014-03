using System.Collections.Concurrent;

namespace KncWX2Server.LoginServer;

/// <summary>
/// Faithful C# port of LoginServer/WeddingHallManager.{h,cpp} (SERV_RELATIONSHIP_SYSTEM):
/// tracks active/reserved wedding halls, room bindings, and item ticket lookups across GameServers.
/// </summary>
public sealed class KWeddingHallManager
{
    public sealed class WeddingHallEntry
    {
        public int WeddingUid { get; init; }
        public long RoomUid { get; set; }
        public long BrideUnitUid { get; init; }
        public long GroomUnitUid { get; init; }
        public string BrideNickName { get; init; } = string.Empty;
        public string GroomNickName { get; init; } = string.Empty;
        public sbyte WeddingHallType { get; init; }
        public sbyte OfficiantNpc { get; init; }
        public DateTime WeddingDateUtc { get; init; } = DateTime.UtcNow;
    }

    private readonly ConcurrentDictionary<int, WeddingHallEntry> _halls = new();

    public bool AddWeddingInfo(WeddingHallEntry info) => _halls.TryAdd(info.WeddingUid, info);

    public bool UpdateWeddingInfo(int weddingUid, long roomUid)
    {
        if (!_halls.TryGetValue(weddingUid, out var existing))
            return false;
        existing.RoomUid = roomUid;
        return true;
    }

    public bool DeleteWeddingInfo(int weddingUid) => _halls.TryRemove(weddingUid, out _);

    public bool TryGetWeddingInfo(int weddingUid, out WeddingHallEntry? info)
        => _halls.TryGetValue(weddingUid, out info);

    public IReadOnlyDictionary<int, WeddingHallEntry> GetAllWeddingInfo() => _halls;
}
