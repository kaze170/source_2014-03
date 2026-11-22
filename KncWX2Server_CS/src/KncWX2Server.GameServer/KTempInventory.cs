using KncWX2Server.Core.Packets;

namespace KncWX2Server.GameServer;

/// <summary>
/// Faithful C# port of GameServer/TempInventory.{h,cpp}:
/// Henir/Dungeon temporary overflow inventory before claiming into permanent inventory or discarding.
/// </summary>
public sealed class KTempInventory
{
    public sealed class TempItemEntry
    {
        public int TempItemUid { get; init; }
        public int ItemId { get; init; }
        public byte SealData { get; init; }
        public KItemInfo ItemInfo { get; init; } = new();
    }

    private int _nextTempItemUid;
    private readonly Dictionary<int, TempItemEntry> _items = new();
    private readonly object _sync = new();

    public bool IsEmpty
    {
        get { lock (_sync) return _items.Count == 0; }
    }

    public void Clear()
    {
        lock (_sync)
        {
            _items.Clear();
            _nextTempItemUid = 0;
        }
    }

    public TempItemEntry AddTempInventoryItem(int itemId, byte sealData, KItemInfo? itemInfo = null)
    {
        lock (_sync)
        {
            int uid = ++_nextTempItemUid;
            var info = itemInfo ?? new KItemInfo { ItemId = itemId, Quantity = 1 };
            var entry = new TempItemEntry
            {
                TempItemUid = uid,
                ItemId = itemId,
                SealData = sealData,
                ItemInfo = info,
            };
            _items[uid] = entry;
            return entry;
        }
    }

    public bool PrepareTempItemToInventory(int tempItemUid, out TempItemEntry? entry)
    {
        lock (_sync)
            return _items.Remove(tempItemUid, out entry);
    }

    public bool DeleteTempItem(int tempItemUid)
    {
        lock (_sync)
            return _items.Remove(tempItemUid);
    }

    public List<TempItemEntry> GetAll()
    {
        lock (_sync) return _items.Values.ToList();
    }
}
