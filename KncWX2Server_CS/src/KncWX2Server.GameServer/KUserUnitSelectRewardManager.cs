namespace KncWX2Server.GameServer;

/// <summary>
/// Faithful C# port of GameServer/UserUnitSelectRewardManager.{h,cpp}:
/// manages 2nd-security login rewards and first-unit-select event rewards.
/// </summary>
public sealed class KUserUnitSelectRewardManager
{
    private readonly List<int> _accessRewards = [];
    private readonly Dictionary<int, (int itemId, int quantity, long claimedUnitUid)> _firstSelectRewards = new();

    public void AddRewardItemInfo(int rewardId) => _accessRewards.Add(rewardId);

    public List<int> ConsumeAccessRewards()
    {
        var copy = _accessRewards.ToList();
        _accessRewards.Clear();
        return copy;
    }

    public void RegisterFirstSelectReward(int dbIndexId, int itemId, int quantity)
        => _firstSelectRewards[dbIndexId] = (itemId, quantity, 0);

    public bool TryClaimFirstSelectReward(int dbIndexId, long unitUid, out int itemId, out int quantity)
    {
        if (_firstSelectRewards.TryGetValue(dbIndexId, out var r) && r.claimedUnitUid == 0)
        {
            itemId = r.itemId;
            quantity = r.quantity;
            _firstSelectRewards.Remove(dbIndexId);
            return true;
        }
        itemId = 0;
        quantity = 0;
        return false;
    }
}
