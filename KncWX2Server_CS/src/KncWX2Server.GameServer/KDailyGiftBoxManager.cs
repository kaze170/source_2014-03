namespace KncWX2Server.GameServer;

/// <summary>
/// Faithful C# port of Common/GameEvent/DailyGiftBoxManager.{h,cpp}:
/// tracks Daily Gift Box package script-to-item mappings and per-unit purchase states.
/// </summary>
public sealed class KDailyGiftBoxManager
{
    public enum PurchasedState
    {
        PurchasedNot = 0,
        PurchasedBuy = 1,
        PurchasedRefund = 2,
        PurchasedConfirm = 3,
    }

    public HashSet<int> ItemIds { get; } = new();
    public Dictionary<int, int> PackageMap { get; } = new(); // ScriptID -> ItemID

    public void AddPackageInfo(int itemId, int scriptId)
    {
        if (itemId <= 0 || scriptId <= 0) return;
        if (!PackageMap.TryAdd(scriptId, itemId))
            return;
        ItemIds.Add(itemId);
    }

    public bool IsDailyGiftBoxItemId(int itemId) => ItemIds.Contains(itemId);

    public bool IsDailyGiftBoxScriptId(int scriptId, out int itemId) =>
        PackageMap.TryGetValue(scriptId, out itemId);
}
