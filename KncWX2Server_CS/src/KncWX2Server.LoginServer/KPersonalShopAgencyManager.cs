using System.Collections.Concurrent;

namespace KncWX2Server.LoginServer;

/// <summary>
/// Faithful C# port of LoginServer/PersonalShopAgency.{h,cpp} &amp; PersonalShopItemManager.{h,cpp}:
/// offline consignment/agency shop hosting up to 9 items, 10% sales commission, purchase locking,
/// ED/item pickup, and expiration handling.
/// </summary>
public sealed class KPersonalShopAgencyManager
{
    public const int SellItemLimitNum = 9;
    public const double AgencyCommissionRate = 0.10;

    public enum ItemListType
    {
        AllItems = 0,
        OnlyOnSaleItems = 1,
        OnlySoldOutItems = 2,
    }

    public sealed class AgencyShopItem
    {
        public long ItemUid { get; init; }
        public int ItemId { get; init; }
        public int Quantity { get; set; }
        public int PricePerUnit { get; init; }
        public int SoldQuantity { get; set; }
    }

    public sealed class AgencyShop
    {
        public long ShopUid { get; init; }
        public long HostUnitUid { get; init; }
        public string HostNickName { get; init; } = string.Empty;
        public string ShopName { get; init; } = string.Empty;
        public DateTime ExpirationUtc { get; set; }
        public long TotalEarnedEd { get; set; }
        public List<AgencyShopItem> Items { get; } = [];
        public HashSet<long> ActiveBuyerUnitUids { get; } = [];
        public object SyncRoot { get; } = new();

        public bool IsExpired(DateTime? nowUtc = null) => (nowUtc ?? DateTime.UtcNow) >= ExpirationUtc;
    }

    private long _nextShopUid = 1;
    private readonly ConcurrentDictionary<long, AgencyShop> _shops = new();
    private readonly ConcurrentDictionary<long, long> _hostUnitToShop = new();

    public AgencyShop? CreateAgencyShop(
        long hostUnitUid, string hostNick, string shopName,
        IEnumerable<AgencyShopItem> items, TimeSpan duration)
    {
        if (_hostUnitToShop.ContainsKey(hostUnitUid)) return null;
        var itemList = items.Take(SellItemLimitNum).ToList();
        if (itemList.Count == 0) return null;

        long shopUid = Interlocked.Increment(ref _nextShopUid);
        var shop = new AgencyShop
        {
            ShopUid = shopUid,
            HostUnitUid = hostUnitUid,
            HostNickName = hostNick,
            ShopName = shopName,
            ExpirationUtc = DateTime.UtcNow.Add(duration),
        };
        shop.Items.AddRange(itemList);
        _shops[shopUid] = shop;
        _hostUnitToShop[hostUnitUid] = shopUid;
        return shop;
    }

    public (bool ok, int totalPaid, int sellerNetEd, int itemId) BuyFromAgencyShop(
        long shopUid, long itemUid, int buyQty, int buyerEd, DateTime? nowUtc = null)
    {
        if (buyQty <= 0 || !_shops.TryGetValue(shopUid, out var shop))
            return (false, 0, 0, 0);

        lock (shop.SyncRoot)
        {
            if (shop.IsExpired(nowUtc)) return (false, 0, 0, 0);
            var item = shop.Items.FirstOrDefault(i => i.ItemUid == itemUid);
            if (item == null || item.Quantity < buyQty) return (false, 0, 0, 0);

            long totalCostL = (long)item.PricePerUnit * buyQty;
            if (totalCostL > int.MaxValue || buyerEd < totalCostL)
                return (false, 0, 0, 0);

            int totalPaid = (int)totalCostL;
            int tax = (int)Math.Floor(totalPaid * AgencyCommissionRate);
            int sellerNet = Math.Max(0, totalPaid - tax);

            item.Quantity -= buyQty;
            item.SoldQuantity += buyQty;
            shop.TotalEarnedEd += sellerNet;
            return (true, totalPaid, sellerNet, item.ItemId);
        }
    }

    public (bool ok, long pickedEd, List<AgencyShopItem> remainingItems) PickUpAndCloseShop(long hostUnitUid)
    {
        if (!_hostUnitToShop.TryRemove(hostUnitUid, out long shopUid) ||
            !_shops.TryRemove(shopUid, out var shop))
            return (false, 0, []);

        lock (shop.SyncRoot)
        {
            var rem = shop.Items.Where(i => i.Quantity > 0).ToList();
            return (true, shop.TotalEarnedEd, rem);
        }
    }

    public AgencyShop? GetShopByHost(long hostUnitUid)
        => _hostUnitToShop.TryGetValue(hostUnitUid, out long sid) && _shops.TryGetValue(sid, out var s) ? s : null;

    public AgencyShop? GetShopByUid(long shopUid)
        => _shops.TryGetValue(shopUid, out var s) ? s : null;

    public bool ExtendExpiration(long hostUnitUid, TimeSpan addDuration)
    {
        var shop = GetShopByHost(hostUnitUid);
        if (shop == null) return false;
        lock (shop.SyncRoot)
        {
            var baseline = shop.ExpirationUtc > DateTime.UtcNow ? shop.ExpirationUtc : DateTime.UtcNow;
            shop.ExpirationUtc = baseline.Add(addDuration);
            return true;
        }
    }

    public int RemoveBuyerFromAllShops(long unitUid)
    {
        int removed = 0;
        foreach (var shop in _shops.Values)
        {
            lock (shop.SyncRoot)
            {
                if (shop.ActiveBuyerUnitUids.Remove(unitUid))
                    removed++;
            }
        }
        return removed;
    }

    public List<AgencyShopItem> GetSellPersonalShopItemInfoList(
        long hostUnitUid, ItemListType listType = ItemListType.OnlyOnSaleItems)
    {
        var shop = GetShopByHost(hostUnitUid);
        if (shop == null) return [];
        lock (shop.SyncRoot)
        {
            return listType switch
            {
                ItemListType.OnlyOnSaleItems => shop.Items.Where(i => i.Quantity > 0).ToList(),
                ItemListType.OnlySoldOutItems => shop.Items.Where(i => i.Quantity <= 0).ToList(),
                _ => shop.Items.ToList(),
            };
        }
    }
}
