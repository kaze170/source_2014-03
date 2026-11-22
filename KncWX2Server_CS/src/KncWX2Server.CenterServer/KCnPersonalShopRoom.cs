using KncWX2Server.Core.Region;

namespace KncWX2Server.CenterServer;

/// <summary>
/// Faithful C# port of CenterServer/PersonalShop.{h,cpp}:
/// manages Center-side personal shop room FSM (Init -> Wait -> Shop -> Close),
/// item listing registration, buyer join/purchase with commission calculation
/// (TD_COMMISSION = 0.03, TD_HOST_COMMISSION = 0.10), and host/all-sold auto-close.
/// </summary>
public sealed class KCnPersonalShopRoom
{
    public const int OpeningEd = 1000;
    public const double TradeBoardCommissionRate = 0.03;
    public const double NonPremiumHostCommissionRate = 0.10;
    public const int MaxShopMember = 10;

    public enum ShopState
    {
        Init = 0,
        Wait = 1,
        Shop = 2,
        Close = 3,
    }

    public sealed class ShopListing
    {
        public long ItemUid { get; init; }
        public int ItemId { get; init; }
        public int Quantity { get; set; }
        public int PricePerOne { get; init; }
    }

    private readonly Dictionary<long, ShopListing> _listings = new();
    private readonly HashSet<long> _participants = [];

    public long ShopUid { get; init; }
    public long HostUnitUid { get; private set; }
    public string ShopName { get; private set; } = string.Empty;
    public bool IsHostPremium { get; private set; }
    public ShopState State { get; private set; } = ShopState.Init;
    public int BreakReason { get; private set; }

    public int MemberCount => _participants.Count;
    public int ListingCount => _listings.Count;

    public int OpenShop(long hostUnitUid, string shopName, bool isHostPremium)
    {
        if (State != ShopState.Init)
            return NetError.ERR_PERSONAL_SHOP_00;

        HostUnitUid = hostUnitUid;
        ShopName = shopName;
        IsHostPremium = isHostPremium;
        _participants.Clear();
        _participants.Add(hostUnitUid);
        _listings.Clear();
        State = ShopState.Wait;
        return NetError.NET_OK;
    }

    public int RegisterItems(long requesterUnitUid, IEnumerable<ShopListing> items)
    {
        if (State != ShopState.Wait || requesterUnitUid != HostUnitUid)
            return NetError.ERR_PERSONAL_SHOP_07;

        foreach (var item in items)
        {
            if (item.Quantity <= 0 || item.PricePerOne <= 0)
                return NetError.ERR_PERSONAL_SHOP_08;
            _listings[item.ItemUid] = item;
        }
        State = ShopState.Shop;
        return NetError.NET_OK;
    }

    public int JoinShop(long buyerUnitUid)
    {
        if (State != ShopState.Shop)
            return NetError.ERR_PERSONAL_SHOP_09;
        if (_participants.Count >= MaxShopMember)
            return NetError.ERR_PERSONAL_SHOP_10;

        _participants.Add(buyerUnitUid);
        return NetError.NET_OK;
    }

    public bool LeaveShop(long unitUid)
    {
        if (unitUid == HostUnitUid)
        {
            BreakShop(NetError.NOT_LEAVE_ROOM_REASON_08);
            return true;
        }
        return _participants.Remove(unitUid);
    }

    public (int ok, int totalCost, int netSellerProceeds, int commissionFee, bool shopSoldOut) BuyItem(
        long buyerUnitUid, long itemUid, int buyQty, bool viaTradeBoard = false)
    {
        if (State != ShopState.Shop)
            return (NetError.ERR_PERSONAL_SHOP_12, 0, 0, 0, false);
        if (!viaTradeBoard && !_participants.Contains(buyerUnitUid))
            return (NetError.ERR_PERSONAL_SHOP_14, 0, 0, 0, false);
        if (buyerUnitUid == HostUnitUid)
            return (NetError.ERR_PERSONAL_SHOP_15, 0, 0, 0, false);
        if (!_listings.TryGetValue(itemUid, out var listing) || buyQty <= 0)
            return (NetError.ERR_PERSONAL_SHOP_16, 0, 0, 0, false);
        if (listing.Quantity < buyQty)
            return (NetError.ERR_PERSONAL_SHOP_17, 0, 0, 0, false);

        long totalCostLong = (long)listing.PricePerOne * buyQty;
        if (totalCostLong > int.MaxValue)
            return (NetError.ERR_PERSONAL_SHOP_20, 0, 0, 0, false);

        int totalCost = (int)totalCostLong;
        double rate = viaTradeBoard
            ? TradeBoardCommissionRate
            : (IsHostPremium ? 0.0 : NonPremiumHostCommissionRate);
        int fee = (int)(totalCost * rate);
        int proceeds = totalCost - fee;

        listing.Quantity -= buyQty;
        if (listing.Quantity <= 0)
            _listings.Remove(itemUid);

        bool soldOut = _listings.Count == 0;
        if (soldOut)
            BreakShop(NetError.NET_OK);

        return (NetError.NET_OK, totalCost, proceeds, fee, soldOut);
    }

    public void BreakShop(int reason)
    {
        BreakReason = reason;
        _listings.Clear();
        _participants.Clear();
        State = ShopState.Close;
    }
}
