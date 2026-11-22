using KncWX2Server.Core.Framework;
using KncWX2Server.Core.Packets;

namespace KncWX2Server.GameServer;

/// <summary>
/// Faithful C# port of Common/X2Data/XSLItemManager.{h,cpp} ItemExchange subsystem:
/// supports NPC house-indexed exchanges keyed by (HouseId, SrcItemId, SrcQuantity)
/// across IET_ALL (grant all), IET_RANDOM (KLottery weighted roll), and IET_SELECT (player choice),
/// preserving equipment enchant/attribute/socket metadata and period duration.
/// </summary>
public sealed class KItemExchangeManager
{
    public enum ItemExchangeType : sbyte
    {
        Invalid = 0,
        All = 1,
        Random = 2,
        Select = 3,
    }

    public sealed class ItemExchangeData
    {
        public ItemExchangeType ExchangeType { get; set; } = ItemExchangeType.Invalid;
        public Dictionary<int, int> DestItemQuantities { get; } = new();
        public Dictionary<int, short> DestItemPeriods { get; } = new();
        public KLottery Lottery { get; } = new();

        public bool AddExchangeData(
            ItemExchangeType exchangeType,
            int destItemId,
            float destItemRate,
            int destQuantity,
            short period = 0)
        {
            if (destItemId <= 0 || destQuantity <= 0)
                return false;

            if (ExchangeType == ItemExchangeType.Invalid)
                ExchangeType = exchangeType;
            else if (ExchangeType != exchangeType)
                return false;

            switch (exchangeType)
            {
                case ItemExchangeType.All:
                case ItemExchangeType.Select:
                    DestItemQuantities[destItemId] = destQuantity;
                    DestItemPeriods[destItemId] = period;
                    return true;
                case ItemExchangeType.Random:
                    DestItemPeriods[destItemId] = period;
                    return Lottery.AddCase(destItemId, destItemRate, destQuantity);
                default:
                    return false;
            }
        }
    }

    // (HouseId, SrcItemId, SrcQuantity) -> ItemExchangeData
    private readonly Dictionary<(int houseId, int srcItemId, int srcQuantity), ItemExchangeData> _table = new();

    public bool HasTableForHouse(int houseId)
        => _table.Keys.Any(k => k.houseId == houseId);

    public bool AddExchangeEntry(
        int houseId,
        int srcItemId,
        int srcQuantity,
        ItemExchangeType exchangeType,
        int destItemId,
        float rate,
        int destQuantity,
        short period = 0)
    {
        if (houseId <= 0 || srcItemId <= 0 || srcQuantity <= 0)
            return false;

        var key = (houseId, srcItemId, srcQuantity);
        if (!_table.TryGetValue(key, out var data))
        {
            data = new ItemExchangeData();
            _table[key] = data;
        }
        return data.AddExchangeData(exchangeType, destItemId, rate, destQuantity, period);
    }

    public bool TryGetExchangeResult(
        int houseId,
        int srcItemId,
        int srcQuantity,
        KItemInfo? srcItemInfo,
        int destItemId,
        out Dictionary<int, KItemInfo> resultItems,
        out bool randomExchange,
        out ItemExchangeType exchangeType,
        double? fixedRoll = null)
    {
        resultItems = new Dictionary<int, KItemInfo>();
        randomExchange = false;
        exchangeType = ItemExchangeType.Invalid;

        if (!_table.TryGetValue((houseId, srcItemId, srcQuantity), out var data))
            return false;

        exchangeType = data.ExchangeType;
        switch (data.ExchangeType)
        {
            case ItemExchangeType.Random:
            {
                randomExchange = true;
                int pickedId = data.Lottery.Decision(fixedRoll);
                if (pickedId == KLottery.CaseBlank || pickedId <= 0)
                    return false;

                int qty = Math.Max(1, data.Lottery.GetParam1(pickedId));
                short period = data.DestItemPeriods.GetValueOrDefault(pickedId, (short)0);
                resultItems[pickedId] = BuildResultItem(pickedId, qty, period, srcItemInfo);
                return true;
            }
            case ItemExchangeType.All:
            {
                foreach (var kv in data.DestItemQuantities)
                {
                    short period = data.DestItemPeriods.GetValueOrDefault(kv.Key, (short)0);
                    resultItems[kv.Key] = BuildResultItem(kv.Key, kv.Value, period, srcItemInfo);
                }
                return resultItems.Count > 0;
            }
            case ItemExchangeType.Select:
            {
                if (!data.DestItemQuantities.TryGetValue(destItemId, out int qty))
                    return false;
                short period = data.DestItemPeriods.GetValueOrDefault(destItemId, (short)0);
                resultItems[destItemId] = BuildResultItem(destItemId, qty, period, srcItemInfo);
                return true;
            }
            default:
                return false;
        }
    }

    private static KItemInfo BuildResultItem(int itemId, int quantity, short period, KItemInfo? src)
    {
        var info = new KItemInfo
        {
            ItemId = itemId,
            Quantity = Math.Max(1, quantity),
            Period = period,
        };
        if (src != null)
        {
            info.EnchantLevel = src.EnchantLevel;
            info.AttribEnchantInfo = new KItemAttributeEnchantInfo
            {
                AttribEnchant0 = src.AttribEnchantInfo.AttribEnchant0,
                AttribEnchant1 = src.AttribEnchantInfo.AttribEnchant1,
                AttribEnchant2 = src.AttribEnchantInfo.AttribEnchant2,
            };
            info.ItemSockets = [.. src.ItemSockets];
            info.ExpandedSocketNum = src.ExpandedSocketNum;
        }
        return info;
    }
}
