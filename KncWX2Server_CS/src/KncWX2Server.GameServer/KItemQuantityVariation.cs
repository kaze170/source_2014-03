namespace KncWX2Server.GameServer;

/// <summary>
/// Faithful C# port of GameServer/ItemQuantityVariation.{h,cpp} (SERV_ED_AND_ITEM_MORNITORING_SYSTEM):
/// tracks cumulative item gains and consumptions per session for audit monitoring.
/// </summary>
public sealed class KItemQuantityVariation
{
    private readonly Dictionary<int, int> _increase = new();
    private readonly Dictionary<int, int> _decrease = new();

    public void Clear()
    {
        _increase.Clear();
        _decrease.Clear();
    }

    public void IncreaseQuantity(int itemId, int quantity)
    {
        if (itemId <= 0 || quantity <= 0) return;
        _increase[itemId] = _increase.GetValueOrDefault(itemId) + quantity;
    }

    public void DecreaseQuantity(int itemId, int quantity)
    {
        if (itemId <= 0 || quantity <= 0) return;
        _decrease[itemId] = _decrease.GetValueOrDefault(itemId) + quantity;
    }

    public IReadOnlyDictionary<int, int> GetIncreaseSnapshot() => new Dictionary<int, int>(_increase);
    public IReadOnlyDictionary<int, int> GetDecreaseSnapshot() => new Dictionary<int, int>(_decrease);
}
