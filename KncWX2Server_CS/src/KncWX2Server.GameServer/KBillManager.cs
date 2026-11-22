using System.Globalization;
using KncWX2Server.Core.Events;
using KncWX2Server.Core.Framework;
using KncWX2Server.Core.Logging;
using KncWX2Server.Core.Packets;
using KncWX2Server.Core.Region;

namespace KncWX2Server.GameServer;

/// <summary>
/// Faithful C# port of GameServer/BillManager.{h,cpp}:
/// manages cash shop product catalog, packages, attributes, categories,
/// reverse-ordered product pagination, bonus-cash time windows, and release ticks.
/// </summary>
public sealed class KBillManager : KPerformer
{
    public const int DefaultNumProductPerPage = 800;
    public const sbyte CategoryCoupon = 71;           // CSDC_COUPON
    public const sbyte CategoryNoSellInPackage = 72;  // CSDC_NO_SELL_IN_PACKAGE

    public enum CashShopReleaseTickType
    {
        Normal = 0,          // CSRTT_NORMAL
        BonusEvent = 1,      // CSRTT_BONUS_EVENT
        CashShopOnOff = 2,   // CSRTT_CASHSHOP_ON_OFF
    }

    private readonly object _sync = new();
    private readonly Dictionary<int, int> _releaseTicks = new();
    private readonly List<KBillProductCategory> _categories = [];
    private readonly Dictionary<int, KBillProductInfo> _products = new();
    private readonly Dictionary<int, KBillProductAttribute> _attributes = new();
    private readonly Dictionary<int, KBillPackageInfo> _packages = new();
    private readonly List<List<KBillProductInfo>> _productPages = [];
    private readonly List<string> _bonusStartTimes = [];
    private readonly List<string> _bonusEndTimes = [];
    private uint _currentPurchaseNo;
    private long _lastProductCheckTickMs;
    private long _lastCashEventCheckTickMs;

    public const int ProductCheckGapMs = 60_000;
    public const int CashEventCheckGapMs = 10_000;

    public bool BonusCashEventActive { get; set; }
    public int PageSize { get; set; } = DefaultNumProductPerPage;
    public override uint PerformerTypeId => PerformerId.PI_GS_KOG_BILLING_MANAGER;

    public KBillManager(RegionProfile? region = null)
        : base(region ?? RegionProfile.Current)
    {
        long now = Environment.TickCount64;
        _lastProductCheckTickMs = now - ProductCheckGapMs;
        _lastCashEventCheckTickMs = now - CashEventCheckGapMs;
    }

    public int GetReleaseTick(CashShopReleaseTickType type = CashShopReleaseTickType.Normal)
    {
        lock (_sync) return _releaseTicks.GetValueOrDefault((int)type, 0);
    }

    public void SetReleaseTick(CashShopReleaseTickType type, int tick)
    {
        lock (_sync) _releaseTicks[(int)type] = tick;
    }

    public void SetReleaseTick(CashShopReleaseTickType type, IReadOnlyDictionary<int, int> mapReleaseTick)
    {
        if (!mapReleaseTick.TryGetValue((int)type, out int tick)) return;
        lock (_sync) _releaseTicks[(int)type] = tick;
    }

    public List<KBillProductCategory> GetBillProductCategory()
    {
        lock (_sync) return [.. _categories];
    }

    public void SetBillProductCategory(IEnumerable<KBillProductCategory> categories)
    {
        lock (_sync)
        {
            _categories.Clear();
            _categories.AddRange(categories);
        }
    }

    public Dictionary<int, KBillProductInfo> GetBillProductInfo()
    {
        lock (_sync) return new Dictionary<int, KBillProductInfo>(_products);
    }

    public void SetBillProductInfo(IEnumerable<KeyValuePair<int, KBillProductInfo>> products)
    {
        lock (_sync)
        {
            _products.Clear();
            foreach (var kv in products)
                _products[kv.Key] = kv.Value;
        }
    }

    public Dictionary<int, KBillProductAttribute> GetBillProductAttribute()
    {
        lock (_sync) return new Dictionary<int, KBillProductAttribute>(_attributes);
    }

    public void SetBillProductAttribute(IEnumerable<KeyValuePair<int, KBillProductAttribute>> attributes)
    {
        lock (_sync)
        {
            _attributes.Clear();
            foreach (var kv in attributes)
                _attributes[kv.Key] = kv.Value;
        }
    }

    public Dictionary<int, KBillPackageInfo> GetBillPackageInfo()
    {
        lock (_sync) return new Dictionary<int, KBillPackageInfo>(_packages);
    }

    public void SetBillPackageInfo(IEnumerable<KeyValuePair<int, KBillPackageInfo>> packages)
    {
        lock (_sync)
        {
            _packages.Clear();
            foreach (var kv in packages)
                _packages[kv.Key] = kv.Value;
        }
    }

    /// <summary>
    /// Builds cash shop product pages in reverse ProductNo order matching BillManager.cpp,
    /// skipping non-sale, hidden, coupon, and package-only items.
    /// </summary>
    public void SetBillProductPage()
    {
        lock (_sync)
        {
            _productPages.Clear();
            var page = new List<KBillProductInfo>(Math.Max(1, PageSize));
            foreach (var kv in _products.OrderByDescending(x => x.Key))
            {
                var p = kv.Value;
                if (!p.Sale || !p.Show) continue;
                if (p.CategoryNo == CategoryCoupon || p.CategoryNo == CategoryNoSellInPackage) continue;

                page.Add(p);
                if (page.Count >= PageSize)
                {
                    _productPages.Add(page);
                    page = new List<KBillProductInfo>(Math.Max(1, PageSize));
                }
            }
            if (page.Count > 0)
                _productPages.Add(page);
        }
    }

    public bool GetBillProductPage(int page, out List<KBillProductInfo> products, out bool isFinal)
    {
        lock (_sync)
        {
            products = [];
            isFinal = true;
            if (page <= 0 || page > _productPages.Count)
                return false;

            isFinal = page >= _productPages.Count;
            products = [.. _productPages[page - 1]];
            return true;
        }
    }

    public int GetItemId(int productNo)
    {
        lock (_sync) return _products.TryGetValue(productNo, out var p) ? p.ProductId : 0;
    }

    public bool GetProductInfo(int productNo, out KBillProductInfo info)
    {
        lock (_sync)
        {
            if (_products.TryGetValue(productNo, out var found))
            {
                info = found;
                return true;
            }
            info = new KBillProductInfo();
            return false;
        }
    }

    public bool GetPackageInfo(int productNo, out KBillPackageInfo info)
    {
        lock (_sync)
        {
            if (_packages.TryGetValue(productNo, out var found))
            {
                info = found;
                return true;
            }
            info = new KBillPackageInfo();
            return false;
        }
    }

    public bool GetProductAttribute(int productNo, out KBillProductAttribute info)
    {
        lock (_sync)
        {
            if (_attributes.TryGetValue(productNo, out var found))
            {
                info = found;
                return true;
            }
            info = new KBillProductAttribute();
            return false;
        }
    }

    public bool IsPackage(int productNo)
    {
        lock (_sync) return _packages.ContainsKey(productNo);
    }

    public bool GetPrice(int productNo, out int price)
    {
        lock (_sync)
        {
            if (_products.TryGetValue(productNo, out var p))
            {
                price = p.SalePrice;
                return true;
            }
            price = 0;
            return false;
        }
    }

    public bool GetTotalPrice(IEnumerable<KNXBTPurchaseReqInfo> items, out int totalPrice)
    {
        totalPrice = 0;
        int sum = 0;
        foreach (var item in items)
        {
            if (!GetPrice((int)item.ProductNo, out int current))
            {
                totalPrice = 0;
                return false;
            }
            sum += current * Math.Max(1, (int)item.OrderQuantity);
        }
        totalPrice = sum;
        return true;
    }

    public uint GetNextPurchaseNo()
    {
        lock (_sync)
        {
            _currentPurchaseNo++;
            if (_currentPurchaseNo > 0xfffffff0)
                _currentPurchaseNo = 1;
            return _currentPurchaseNo;
        }
    }

    public HashSet<int> GetSellCashItemList()
    {
        lock (_sync)
        {
            var result = new HashSet<int>();
            foreach (var p in _products.Values)
            {
                if (p.Show && p.Sale)
                    result.Add(p.ProductId);
            }
            return result;
        }
    }

    public void SetBonusWindows(IEnumerable<string> startTimes, IEnumerable<string> endTimes)
    {
        lock (_sync)
        {
            _bonusStartTimes.Clear();
            _bonusEndTimes.Clear();
            _bonusStartTimes.AddRange(startTimes);
            _bonusEndTimes.AddRange(endTimes);
        }
    }

    public bool CheckBonusPeriodTime(IList<string> startTimes, IList<string> endTimes, DateTime? nowOverride = null)
    {
        DateTime now = nowOverride ?? DateTime.UtcNow;
        int count = Math.Min(startTimes.Count, endTimes.Count);
        for (int i = count - 1; i >= 0; i--)
        {
            if (!DateTime.TryParse(startTimes[i], CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var begin))
                return false;
            if (!DateTime.TryParse(endTimes[i], CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var end))
                return false;

            if (end < now)
            {
                startTimes.RemoveAt(i);
                endTimes.RemoveAt(i);
            }
            else if (begin <= now && end >= now)
            {
                return true;
            }
        }
        return false;
    }

    public override void Tick()
    {
        base.Tick();
        long now = Environment.TickCount64;
        if (now - _lastCashEventCheckTickMs >= CashEventCheckGapMs)
        {
            lock (_sync)
            {
                if (_bonusStartTimes.Count > 0 && _bonusEndTimes.Count > 0)
                    BonusCashEventActive = CheckBonusPeriodTime(_bonusStartTimes, _bonusEndTimes);
            }
            _lastCashEventCheckTickMs = now;
        }
    }

    protected override void ProcessEvent(KEvent ev)
    {
        switch (ev.EventId)
        {
            case EventId.EBILL_CHECK_PRODUCT_INFO_ACK:
            {
                var ack = ev.GetPayload<KEBILL_CHECK_PRODUCT_INFO_ACK>(Region);
                if (ack.ReleaseTick != GetReleaseTick(CashShopReleaseTickType.Normal))
                {
                    SetReleaseTick(CashShopReleaseTickType.Normal, ack.ReleaseTick);
                    SetBillProductPage();
                }
                break;
            }
            default:
                KLog.Cwarn($"[KBillManager] Unhandled event: {ev.EventId}");
                break;
        }
    }
}
