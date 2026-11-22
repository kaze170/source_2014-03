using KncWX2Server.Core.Database;
using KncWX2Server.Core.Packets;
using KncWX2Server.Core.Region;

namespace KncWX2Server.GameServer;

/// <summary>
/// Faithful C# port of GameServer/UserPersonalShopManager.{h,cpp} &amp; LoginServer/PersonalShopAgency.{h,cpp}:
/// manages player open-market/personal-shop listings and ED transactions with 5% market fee.
/// </summary>
public sealed class KUserPersonalShopManager
{
    public long ActiveShopUid { get; private set; }
    public long JoinedShopUid { get; private set; }
    public long PersonalShopUid => ActiveShopUid > 0 ? ActiveShopUid : JoinedShopUid;
    public bool IsShopOpen => ActiveShopUid > 0;
    public bool IsOpened => IsShopOpen;

    public void JoinShop(long shopUid) => JoinedShopUid = shopUid;

    public void LeaveJoinedShop() => JoinedShopUid = 0;

    public long OpenShopAndRegisterItem(
        SqliteDatabaseManager db,
        RegionProfile region,
        long sellerUnitUid,
        string sellerNick,
        int itemId,
        int quantity,
        int priceEd)
    {
        long listingUid = db.CreatePersonalShopListing(region, sellerUnitUid, sellerNick, itemId, quantity, priceEd);
        ActiveShopUid = listingUid;
        return listingUid;
    }

    public (bool ok, int buyerNewEd, KInventoryItemInfo? granted) BuyItem(
        SqliteDatabaseManager db,
        RegionProfile region,
        long buyerUnitUid,
        long listingUid,
        int quantity)
        => db.BuyPersonalShopListing(region, buyerUnitUid, listingUid, quantity);

    public void CloseShop() => ActiveShopUid = 0;
}
