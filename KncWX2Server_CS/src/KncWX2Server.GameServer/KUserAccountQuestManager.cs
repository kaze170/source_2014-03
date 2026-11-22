using KncWX2Server.Core.Packets;

namespace KncWX2Server.GameServer;

/// <summary>
/// Faithful C# port of GameServer/UserAccountQuestManager.{h,cpp} (SERV_ACCOUNT_MISSION_SYSTEM):
/// manages account-wide quests shared across all characters on the account,
/// day-of-week auto-accepted account quests, and suitable-level party dungeon clear progression.
/// </summary>
public sealed class KUserAccountQuestManager
{
    private readonly Dictionary<int, KQuestInstance> _activeAccountQuests = new();
    private readonly Dictionary<int, DateTime> _completedAccountQuests = new();
    private int _rewardItemCount;

    public int ConsumeQuestRewardItemCount()
    {
        int count = _rewardItemCount;
        _rewardItemCount = 0;
        return count;
    }

    public void AddRewardItemCount(int count) => _rewardItemCount += Math.Max(0, count);

    public void ClearAccountQuest()
    {
        _activeAccountQuests.Clear();
        _completedAccountQuests.Clear();
    }

    public bool IsAccountQuesting(int questId) => _activeAccountQuests.ContainsKey(questId);
    public bool IsCompletedAccountQuest(int questId) => _completedAccountQuests.ContainsKey(questId);

    public KQuestInstance? GetAccountQuestInstance(int questId)
        => _activeAccountQuests.TryGetValue(questId, out var q) ? q : null;

    public bool AddAccountQuest(KQuestInstance quest)
    {
        if (IsAccountQuesting(quest.Id))
            return false;
        _activeAccountQuests[quest.Id] = quest;
        return true;
    }

    public bool RemoveAccountQuest(int questId) => _activeAccountQuests.Remove(questId);

    public bool CompleteAccountQuest(int questId, DateTime? completedUtc = null)
    {
        if (!_activeAccountQuests.Remove(questId))
            return false;
        _completedAccountQuests[questId] = completedUtc ?? DateTime.UtcNow;
        return true;
    }

    public DateTime? GetCompletedDate(int questId)
        => _completedAccountQuests.TryGetValue(questId, out var dt) ? dt : null;

    public static bool IsSuitableLevelUser(long charUid, IReadOnlyDictionary<long, bool> suitableLevelMap)
        => suitableLevelMap.TryGetValue(charUid, out bool ok) && ok;

    public IReadOnlyCollection<KQuestInstance> GetActiveAccountQuests() => _activeAccountQuests.Values;
}
