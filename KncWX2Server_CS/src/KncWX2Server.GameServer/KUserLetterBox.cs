using KncWX2Server.Core.Packets;

namespace KncWX2Server.GameServer;

/// <summary>
/// Faithful C# port of GameServer/UserLetterBox.{h,cpp}:
/// separates user (LT_POST_OFFICE=1), relationship/wedding (LT_WEDDING_REWARD/INVITATION/RESERVE),
/// and system letters, paginates 6 per page (newest first), extracts top-5 unread notifications,
/// handles read/attachment-claim/delete state, and maintains spam-sender blacklist.
/// </summary>
public sealed class KUserLetterBox
{
    public const int OnePageLetterCount = 6;
    public const int NewLetterTitleCount = 5;
    public const int EdiBronzeEd = 127000; // CXSLItem::EDI_BRONZE_ED

    public enum LetterListType : sbyte
    {
        LltSystem = 0,
        LltNormal = 1,
        LltWedding = 2,
    }

    public enum LetterType : sbyte
    {
        LtNone = 0,
        LtPostOffice = 1,
        LtRecommend = 2,
        LtTutol = 3,
        LtEvent = 4,
        LtMessage = 5,
        LtRankingTitle = 6,
        LtBrokenItem = 7,
        LtGuildDisband = 8,
        LtGuildKick = 9,
        LtGuildJoin = 10,
        LtWebPointEvent = 11,
        LtStopSalePshop = 12,
        LtSellItemPshop = 13,
        LtComeBackUserNotify = 14,
        LtOutUserRetainingReward = 15,
        LtDungeonRewardItem = 16,
        LtQuestReward = 17,
        LtIndemnificationDbOnly = 18,
        LtJackpotEventReward = 19,
        LtDbReward = 20,
        LtSystemErrorReward = 21,
        LtWeddingReward = 22,
        LtWeddingInvitation = 23,
        LtWeddingReserve = 24,
        LtBreakUp = 25,
    }

    private readonly object _sync = new();
    private readonly Dictionary<long, KPostItemInfo> _letters = new();
    private readonly List<KPostItemTitleInfo> _userLetters = new();
    private readonly List<KPostItemTitleInfo> _systemLetters = new();
    private readonly List<KPostItemTitleInfo> _relationshipLetters = new();
    private readonly HashSet<string> _spamBlackList = new(StringComparer.OrdinalIgnoreCase);

    public string OwnerNickName { get; private set; } = string.Empty;

    public static bool IsSystemLetterType(sbyte scriptType) => scriptType != (sbyte)LetterType.LtPostOffice;

    public static bool IsItemIdLetterType(sbyte scriptType) => scriptType is
        (sbyte)LetterType.LtPostOffice or
        (sbyte)LetterType.LtMessage or
        (sbyte)LetterType.LtBrokenItem or
        (sbyte)LetterType.LtWebPointEvent or
        (sbyte)LetterType.LtQuestReward or
        (sbyte)LetterType.LtDbReward or
        (sbyte)LetterType.LtSystemErrorReward or
        (sbyte)LetterType.LtWeddingReward or
        (sbyte)LetterType.LtWeddingInvitation or
        (sbyte)LetterType.LtWeddingReserve;

    public void Clear()
    {
        lock (_sync)
        {
            _letters.Clear();
            _userLetters.Clear();
            _systemLetters.Clear();
            _relationshipLetters.Clear();
            _spamBlackList.Clear();
            OwnerNickName = string.Empty;
        }
    }

    public (int newUserCount, int newSystemCount, int newRelationshipCount, List<KPostSmallTitleInfo> newTitles) Init(
        string ownerNickName, IEnumerable<KPostItemInfo> items)
    {
        lock (_sync)
        {
            _letters.Clear();
            _userLetters.Clear();
            _systemLetters.Clear();
            _relationshipLetters.Clear();
            OwnerNickName = ownerNickName;
            foreach (var item in items)
                AddLetterLocked(item);
            return GetRenewalLetterLocked();
        }
    }

    public (int newUserCount, int newSystemCount, int newRelationshipCount, List<KPostSmallTitleInfo> newTitles) GetRenewalLetter()
    {
        lock (_sync)
            return GetRenewalLetterLocked();
    }

    private (int, int, int, List<KPostSmallTitleInfo>) GetRenewalLetterLocked()
    {
        int newUser = 0, newSys = 0, newRel = 0;
        var unread = new List<KPostItemTitleInfo>();

        foreach (var t in _userLetters)
            if (!t.Read) { unread.Add(t); newUser++; }
        foreach (var t in _systemLetters)
            if (!t.Read) { unread.Add(t); newSys++; }
        foreach (var t in _relationshipLetters)
            if (!t.Read) { unread.Add(t); newRel++; }

        unread.Sort((a, b) => string.Compare(b.RegDate, a.RegDate, StringComparison.Ordinal));
        var titles = unread.Take(NewLetterTitleCount)
            .Select(t => new KPostSmallTitleInfo { ScriptType = t.ScriptType, LetterTitle = t.Title })
            .ToList();
        return (newUser, newSys, newRel, titles);
    }

    public void AddLetter(KPostItemInfo postItem)
    {
        lock (_sync)
            AddLetterLocked(postItem);
    }

    private void AddLetterLocked(KPostItemInfo postItem)
    {
        if (postItem.ScriptType == 0)
            postItem.ScriptType = (sbyte)LetterType.LtPostOffice;
        if (postItem.ScriptType == (sbyte)LetterType.LtBrokenItem)
            postItem.EnchantLevel = (sbyte)(-postItem.EnchantLevel);

        _letters[postItem.PostNo] = postItem;

        var title = new KPostItemTitleInfo
        {
            PostNo = postItem.PostNo,
            ScriptType = postItem.ScriptType,
            ScriptIndex = postItem.ScriptIndex,
            EnchantLevel = postItem.EnchantLevel,
            RegDate = postItem.RegDate,
            FromNickName = postItem.FromNickName,
            Title = postItem.Title,
            Read = postItem.Read,
        };

        GetCategoryListLocked(postItem.ScriptType).Add(title);
    }

    private List<KPostItemTitleInfo> GetCategoryListLocked(sbyte scriptType) => scriptType switch
    {
        (sbyte)LetterType.LtPostOffice => _userLetters,
        (sbyte)LetterType.LtWeddingReward or (sbyte)LetterType.LtWeddingInvitation or (sbyte)LetterType.LtWeddingReserve => _relationshipLetters,
        _ => _systemLetters,
    };

    public bool IsExist(long postNo)
    {
        lock (_sync) return _letters.ContainsKey(postNo);
    }

    public bool IsExist(IEnumerable<long> postNos)
    {
        lock (_sync) return postNos.All(_letters.ContainsKey);
    }

    public bool IsSystemLetter(long postNo)
    {
        lock (_sync) return _letters.TryGetValue(postNo, out var post) && IsSystemLetterType(post.ScriptType);
    }

    public bool IsItemIdLetter(long postNo)
    {
        lock (_sync) return _letters.TryGetValue(postNo, out var post) && IsItemIdLetterType(post.ScriptType);
    }

    public bool IsExistRewardInLetter(int rewardId)
    {
        lock (_sync)
            return _letters.Values.Any(p => !IsItemIdLetterType(p.ScriptType) && p.ScriptIndex == rewardId);
    }

    public sbyte GetLetterType(long postNo)
    {
        lock (_sync) return _letters.TryGetValue(postNo, out var post) ? post.ScriptType : (sbyte)LetterType.LtNone;
    }

    public bool GetItemInfo(long postNo, out KItemInfo itemInfo)
    {
        lock (_sync)
        {
            itemInfo = new KItemInfo();
            if (!_letters.TryGetValue(postNo, out var post)) return false;
            if (!IsItemIdLetterType(post.ScriptType) || post.ScriptIndex <= 0) return false;
            itemInfo.ItemId = post.ScriptIndex;
            itemInfo.Quantity = Math.Max(1, post.Quantity);
            itemInfo.EnchantLevel = post.EnchantLevel;
            itemInfo.AttribEnchantInfo = post.AttribEnchantInfo;
            itemInfo.ItemSockets = [.. post.ItemSocket];
            itemInfo.SealData = post.SealData;
            return true;
        }
    }

    public int GetTotalLetterCount(bool systemLetter)
        => GetTotalLetterCount(systemLetter ? (sbyte)LetterListType.LltSystem : (sbyte)LetterListType.LltNormal);

    public bool GetLetterList(bool systemLetter, int pageNo, out List<KPostItemTitleInfo> result)
        => GetLetterList(systemLetter ? (sbyte)LetterListType.LltSystem : (sbyte)LetterListType.LltNormal, pageNo, out result);

    public void InitBlackList(IEnumerable<string> blackList)
    {
        lock (_sync)
        {
            _spamBlackList.Clear();
            foreach (string name in blackList)
            {
                if (!string.IsNullOrWhiteSpace(name))
                    _spamBlackList.Add(name);
            }
        }
    }

    public bool TryGetLetter(long postNo, out KPostItemInfo? postItem)
    {
        lock (_sync) return _letters.TryGetValue(postNo, out postItem);
    }

    public bool ReadLetter(long postNo, out KPostItemInfo? postItem, out bool wasAlreadyRead)
    {
        lock (_sync)
        {
            if (!_letters.TryGetValue(postNo, out postItem))
            {
                wasAlreadyRead = false;
                return false;
            }
            wasAlreadyRead = postItem.Read;
            postItem.Read = true;
            var list = GetCategoryListLocked(postItem.ScriptType);
            var title = list.FirstOrDefault(t => t.PostNo == postNo);
            if (title != null) title.Read = true;
            return true;
        }
    }

    public int GetTotalLetterCount(sbyte letterListType)
    {
        lock (_sync)
        {
            return letterListType switch
            {
                (sbyte)LetterListType.LltSystem => _systemLetters.Count,
                (sbyte)LetterListType.LltWedding => _relationshipLetters.Count,
                _ => _userLetters.Count,
            };
        }
    }

    public bool GetLetterList(sbyte letterListType, int pageNo, out List<KPostItemTitleInfo> result)
    {
        result = new List<KPostItemTitleInfo>();
        if (pageNo <= 0) return false;

        lock (_sync)
        {
            var source = letterListType switch
            {
                (sbyte)LetterListType.LltSystem => _systemLetters,
                (sbyte)LetterListType.LltWedding => _relationshipLetters,
                _ => _userLetters,
            };
            int skip = (pageNo - 1) * OnePageLetterCount;
            for (int i = source.Count - 1 - skip, taken = 0; i >= 0 && taken < OnePageLetterCount; i--, taken++)
                result.Add(source[i]);
            return true;
        }
    }

    public void SetReceivedFlag(long postNo)
    {
        lock (_sync)
        {
            if (!_letters.TryGetValue(postNo, out var post)) return;
            post.ScriptIndex = 0;
            post.Quantity = 0;
            post.EnchantLevel = 0;
            post.AttribEnchantInfo = new KItemAttributeEnchantInfo();
            post.ItemSocket.Clear();
            post.SealData = 0;

            var title = GetCategoryListLocked(post.ScriptType).FirstOrDefault(t => t.PostNo == postNo);
            if (title != null) title.ScriptIndex = 0;
        }
    }

    public void DeleteLetter(long postNo)
    {
        lock (_sync)
        {
            if (!_letters.Remove(postNo, out var removed)) return;
            GetCategoryListLocked(removed.ScriptType).RemoveAll(t => t.PostNo == postNo);
        }
    }

    public void DeleteLetter(IEnumerable<long> postNos)
    {
        lock (_sync)
        {
            foreach (long p in postNos)
            {
                if (_letters.Remove(p, out var removed))
                    GetCategoryListLocked(removed.ScriptType).RemoveAll(t => t.PostNo == p);
            }
        }
    }

    public void AddBlackList(string nickName)
    {
        lock (_sync) _spamBlackList.Add(nickName);
    }

    public void DelBlackList(string nickName)
    {
        lock (_sync) _spamBlackList.Remove(nickName);
    }

    public bool IsBlackList(string nickName)
    {
        lock (_sync) return _spamBlackList.Contains(nickName);
    }

    public List<string> GetBlackList()
    {
        lock (_sync) return _spamBlackList.ToList();
    }
}
