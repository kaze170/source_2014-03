using KncWX2Server.Core.Packets;

namespace KncWX2Server.GameServer;

/// <summary>
/// Faithful C# port of GameServer/UserGameOption.{h,cpp}:
/// validates and stores community deny options (friend/guild/party/trade/couple/practice-pvp)
/// and enforces the 30-entry chat blacklist limit.
/// </summary>
public sealed class KUserGameOption
{
    public const sbyte DosOn = 0;
    public const sbyte DosOff = 1;
    public const sbyte DosOnlyFriend = 2;
    public const int MaxChatBlackList = 30;

    private readonly Dictionary<long, string> _chatBlackList = new();

    public sbyte DenyFriendShip { get; private set; } = DosOff;
    public sbyte DenyInviteGuild { get; private set; } = DosOff;
    public sbyte DenyParty { get; private set; } = DosOff;
    public sbyte DenyPersonalTrade { get; private set; } = DosOff;
    public sbyte DenyRequestCouple { get; private set; } = DosOff;
    public sbyte DenyInvitePracticePvp { get; private set; } = DosOff;

    public bool IsDenyFriendShip => DenyFriendShip == DosOn;
    public bool IsDenyParty => DenyParty == DosOn;
    public bool IsDenyTrade => DenyPersonalTrade == DosOn;
    public bool IsDenyInviteGuild => DenyInviteGuild == DosOn;
    public bool IsDenyRequestCouple => DenyRequestCouple == DosOn;

    public KDenyOptions GetDenyOptions() => new()
    {
        DenyFriendShip = DenyFriendShip,
        DenyInviteGuild = DenyInviteGuild,
        DenyParty = DenyParty,
        DenyPersonalTrade = DenyPersonalTrade,
        DenyRequestCouple = DenyRequestCouple,
        DenyInvitePracticePvp = DenyInvitePracticePvp,
    };

    public bool SetDenyOptions(KDenyOptions options)
    {
        if (options.DenyFriendShip < 0 || options.DenyFriendShip > DosOff) return false;
        if (options.DenyInviteGuild < 0 || options.DenyInviteGuild > DosOnlyFriend) return false;
        if (options.DenyParty < 0 || options.DenyParty > DosOnlyFriend) return false;
        if (options.DenyPersonalTrade < 0 || options.DenyPersonalTrade > DosOnlyFriend) return false;
        if (options.DenyRequestCouple < 0 || options.DenyRequestCouple > DosOnlyFriend) return false;
        if (options.DenyInvitePracticePvp < 0 || options.DenyInvitePracticePvp > DosOnlyFriend) return false;

        DenyFriendShip = options.DenyFriendShip;
        DenyInviteGuild = options.DenyInviteGuild;
        DenyParty = options.DenyParty;
        DenyPersonalTrade = options.DenyPersonalTrade;
        DenyRequestCouple = options.DenyRequestCouple;
        DenyInvitePracticePvp = options.DenyInvitePracticePvp;
        return true;
    }

    public bool IsAddChatBlackListPossible => _chatBlackList.Count < MaxChatBlackList;

    public void SetChatBlackList(IEnumerable<KChatBlackListUnit> list)
    {
        _chatBlackList.Clear();
        foreach (var u in list)
            _chatBlackList[u.UnitUid] = u.NickName;
    }

    public bool AddChatBlackListUnit(long unitUid, string nickName)
    {
        if (!IsAddChatBlackListPossible && !_chatBlackList.ContainsKey(unitUid))
            return false;
        _chatBlackList[unitUid] = nickName;
        return true;
    }

    public bool DelChatBlackListUnit(long unitUid) => _chatBlackList.Remove(unitUid);
    public bool IsChatBlackListUnit(long unitUid) => _chatBlackList.ContainsKey(unitUid);
}
