using KncWX2Server.Core.Region;

namespace KncWX2Server.GameServer;

/// <summary>
/// Faithful C# port of GameServer/UserWishList.{h,cpp}:
/// 21-slot cash-shop wishlist with duplicate detection, left-compacting deletion,
/// and dirty-slot diff tracking for incremental DB persistence.
/// </summary>
public sealed class KUserWishList
{
    public const int SlotMax = 21;

    private sealed class WishSlotData
    {
        public int ItemId { get; private set; }
        private int _beforeItemId;

        public bool IsChanged => ItemId != _beforeItemId;

        public void SetCommitted(int itemId)
        {
            ItemId = itemId;
            _beforeItemId = itemId;
        }

        public void Assign(int itemId)
        {
            _beforeItemId = ItemId;
            ItemId = itemId;
        }
    }

    private readonly WishSlotData[] _slots = Enumerable.Range(0, SlotMax).Select(_ => new WishSlotData()).ToArray();
    public bool BuyCashItemInWishList { get; set; }

    public void InitWishList(IReadOnlyDictionary<int, int> mapWishList, Func<int, bool>? isItemAvailable = null)
    {
        foreach (var s in _slots) s.SetCommitted(0);
        var unavailable = new List<int>();
        foreach (var kv in mapWishList)
        {
            if (kv.Key < 0 || kv.Key >= SlotMax) continue;
            _slots[kv.Key].SetCommitted(kv.Value);
            if (kv.Value > 0 && isItemAvailable != null && !isItemAvailable(kv.Value))
                unavailable.Add(kv.Value);
        }
        foreach (int itemId in unavailable)
            DelWish(itemId);
    }

    public Dictionary<int, int> GetWishList()
    {
        var result = new Dictionary<int, int>();
        for (int i = 0; i < SlotMax; i++)
        {
            if (_slots[i].ItemId > 0)
                result[i] = _slots[i].ItemId;
        }
        return result;
    }

    public Dictionary<int, int> FlushWishListChange()
    {
        var changed = new Dictionary<int, int>();
        for (int i = 0; i < SlotMax; i++)
        {
            if (_slots[i].IsChanged)
            {
                int val = _slots[i].ItemId;
                _slots[i].SetCommitted(val);
                changed[i] = val;
            }
        }
        return changed;
    }

    public (bool ok, int netError) AddWish(int itemId)
    {
        for (int i = 0; i < SlotMax; i++)
        {
            if (_slots[i].ItemId == itemId)
                return (true, NetError.NET_OK);
            if (_slots[i].ItemId == 0)
            {
                _slots[i].Assign(itemId);
                return (true, NetError.NET_OK);
            }
        }
        return (false, NetError.ERR_WISH_LIST_02);
    }

    public bool DelWish(int itemId)
    {
        for (int i = 0; i < SlotMax; i++)
        {
            if (_slots[i].ItemId == itemId)
            {
                for (int j = i; j < SlotMax; j++)
                {
                    if (j == SlotMax - 1)
                    {
                        if (_slots[j].ItemId == 0) break;
                        _slots[j].Assign(0);
                        break;
                    }
                    if (_slots[j].ItemId == 0) break;
                    _slots[j].Assign(_slots[j + 1].ItemId);
                }
                return true;
            }
        }
        return false;
    }
}
