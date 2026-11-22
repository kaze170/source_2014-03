namespace KncWX2Server.GameServer;

/// <summary>
/// Faithful C# port of GameServer/CorruptionMonitor.{h,cpp} (SERV_ED_AND_ITEM_MORNITORING_SYSTEM):
/// maintains ED and item quantity monitoring thresholds and evaluates abnormal variations.
/// </summary>
public sealed class KCorruptionMonitor
{
    public bool MonitoringOn { get; set; }

    private readonly Dictionary<int, long> _edThresholds = new();
    private readonly Dictionary<int, int> _itemThresholds = new();

    public void SetEdThreshold(int reasonCode, long maxDeltaEd) => _edThresholds[reasonCode] = maxDeltaEd;
    public void SetItemThreshold(int itemId, int maxDeltaQty) => _itemThresholds[itemId] = maxDeltaQty;

    public bool IsEdAbnormal(int reasonCode, long deltaEd)
    {
        if (!MonitoringOn) return false;
        return _edThresholds.TryGetValue(reasonCode, out long limit) && deltaEd >= limit;
    }

    public bool IsItemAbnormal(int itemId, int deltaQty)
    {
        if (!MonitoringOn) return false;
        return _itemThresholds.TryGetValue(itemId, out int limit) && deltaQty >= limit;
    }
}
