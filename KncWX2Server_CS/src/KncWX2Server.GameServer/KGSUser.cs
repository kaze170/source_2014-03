using KncWX2Server.CenterServer;
using KncWX2Server.Core.Events;
using KncWX2Server.Core.Framework;
using KncWX2Server.Core.Logging;
using KncWX2Server.Core.Network;
using KncWX2Server.Core.Packets;
using KncWX2Server.Core.Region;
using KncWX2Server.Core.Serialization;
using KncWX2Server.GlobalServer;

namespace KncWX2Server.GameServer;

public sealed class KGSUser : KActor
{
    private readonly KGameServer _server;
    private readonly KRepeatEventFilter _repeatFilter = new();

    public KUserSpiritManager SpiritManager { get; } = new();
    public KAntiAddictionManager AntiAddictionManager { get; } = new();
    public KUserSkillTree SkillTree { get; } = new();
    public KUserTitleManager TitleManager { get; } = new();
    public KUserPersonalShopManager PersonalShopManager { get; } = new();
    public KUserPetManager PetManager { get; } = new();
    public KUserRidingPetManager RidingPetManager { get; } = new();
    public KUserRelationshipManager RelationshipManager { get; } = new();
    public KBlackList BlackList { get; } = new();
    public KUserServerBuffManager ServerBuffManager { get; } = new();
    public KUserPcBangManager PcBangManager { get; } = new();
    public KUserCashInventory CashInventory { get; } = new();
    public KPunishmentManager PunishmentManager { get; } = new();
    public KUserEDManager EdManager { get; } = new();
    public KTempInventory TempInventory { get; } = new();
    public KUserDungeonManager DungeonManager { get; } = new();
    public KUserQuestManager QuestManager { get; } = new();
    public KUserUnitSelectRewardManager UnitSelectRewardManager { get; } = new();
    public KUserJumpingCharacterManager JumpingCharacterManager { get; } = new();
    public KUserLetterBox LetterBox { get; } = new();
    public KUserAbuserManager AbuserManager { get; } = new();
    public KComeBackUserManager ComeBackUserManager { get; } = new();
    public KUserSecurityManager SecurityManager { get; } = new();
    public KUserHackingManager HackingManager { get; } = new();
    public KUserAccountQuestManager AccountQuestManager { get; } = new();
    public KUserRecommendManager RecommendManager { get; } = new();
    public KUserTutorialInfo TutorialInfo { get; } = new();
    public KUserWishList WishList { get; } = new();
    public KItemQuantityVariation ItemQuantityVariation { get; } = new();
    public KUserGameOption GameOption { get; } = new();
    public KUserLocalRankingInfo LocalRankingInfo { get; } = new();
    public KUserGuildManager GuildManager { get; } = new();
    public KUserStatistics Statistics { get; } = new();
    public KUserEventManager EventManager { get; } = new();
    public KUserPvpManager PvpManager { get; } = new();
    public KUserPartyManager PartyManager { get; } = new();
    public KUserCheatManager CheatManager { get; } = new();
    public KUserLogManager LogManager { get; } = new();
    public KUserRankingManager RankingManager { get; } = new();
    public KInventoryLocker InventoryLocker { get; } = new();
    public KUserUnitManager UnitManager { get; } = new();
    public KDenyOptions DenyOptions { get; set; } = new();
    public KFieldOption FieldOption { get; private set; } = new();
    private readonly HashSet<long> _friendUids = new();
    private readonly HashSet<long> _pendingFriendRequests = new();
    private readonly HashSet<long> _blockedFriendUids = new();
    private readonly Dictionary<long, KFriendInfo> _friends = new();
    private readonly Dictionary<sbyte, string> _friendGroups = new();
    private readonly Dictionary<int, DateTime> _quickSlotCooldowns = new();
    private readonly Dictionary<short, KInventoryItemInfo> _defenceQuickSlotItems = new();
    private readonly Dictionary<int, List<(long UnitUid, int State, string RegDate)>> _dailyGiftBoxList = new();
    public Dictionary<int, double> QuickSlotCooldownSeconds { get; } = new();
    public HashSet<int> BattleFieldBlockedQuickSlotItemIds { get; } = new();
    private EventId _pendingDisconnectEventId = EventId.EGS_DISCONNECT_FOR_CHANNEL_CHANGE_REQ;
    private DateTime _lastKnmRefreshUtc = DateTime.MinValue;
    private int _antiAddictionType;
    private int _antiAddictionPlayMinutes;
    public long AccountUid { get; private set; }
    public string UserId { get; private set; } = string.Empty;
    public int AuthLevel { get; private set; }
    public bool IsGuestUser { get; private set; }
    public bool SecondSecurityVerified { get; private set; }
    private string _pendingGuildName = string.Empty;
    public KUnitInfo? ActiveUnit { get; private set; }
    public int CurrentFieldMapId { get; private set; }
    public long CurrentFieldUid { get; private set; }
    public long CurrentRoomUid { get; private set; }
    public long CurrentPartyUid { get; private set; }
    public long CurrentTradeUid { get; private set; }
    public bool IsInvisible { get; private set; }
    public bool IsObserverMode { get; private set; }
    public bool IsEdMonitoringMode { get; set; }
    public int HenirRewardNormalCnt { get; private set; }
    public short PShopAgencyCheatPeriod { get; private set; }
    public long PShopAgencyUid { get; private set; }
    private bool _pshopAgencyRemoteOpened;
    public int QuestPointCheat { get; private set; }
    public uint PvpChannelId { get; private set; } = 1;
    public uint RoomListId { get; private set; } = 1;
    public sbyte HackingUserSendType { get; private set; }
    private sbyte _hackingUserType;
    private bool _useItemNeedPayment;
    private bool _reserveKickUser;
    private int _dungeonRequiredItemId;
    public int NumResurrectionStone { get; set; } = 3;
    public int NumAutoPaymentResStone { get; set; }

    // Batch 2 GameCommon / Event / Tutorial / LocalRanking / PlayStatus state
    private int _noteViewCount = 1;
    private int _oldYearMissionRewardedLevel;
    private int _newYearMissionStepId;
    private long _checkPowerTime;
    private byte _checkPowerCount;
    private byte _checkPowerScore;
    private DateTime _lastAnnivRewardUtc = DateTime.MinValue;
    private readonly bool[] _anniv4RewardFlags = new bool[12];
    private readonly int[] _chinaSpirit = new int[6];
    private DateTime _lastSearchUnitUtc = DateTime.MinValue;
    private bool _timeControlItemCheckDungeonPlay;
    private bool _coboItemGive;
    private bool _hasDirectTimeEventReward;
    private DateTime _lastEventTickUtc = DateTime.UtcNow;
    private double _sessionConnectedSeconds;
    private readonly Queue<int> _pendingTimeEventRewardItemIds = new();
    private readonly HashSet<int> _claimedEventRewardKeys = [];
    private KLocalRankingUserInfo _localRankingUserInfo = new();
    private KGamePlayStatus _gamePlayStatus = new();
    private int _communityTabType;
    private bool _isTutorialTeacher;
    private readonly List<KTutorialUnitInfo> _tutorialStudents = new();
    private string _jumpingRegDate = string.Empty;
    private int _valentineItemCount;
    private int _currentTcId;
    private bool _checkSequenceCount;
    private DateTime _releaseTradeBlockTimeUtc = DateTime.MinValue;
    private DateTime _lastLetterMonitorResetUtc = DateTime.UtcNow;
    private DateTime _lastTradeMonitorResetUtc = DateTime.UtcNow;
    private DateTime _lastDeserializeFailCheckUtc = DateTime.UtcNow;
    public bool DisableHeartbeatCheck { get; private set; }
    public bool IsOutUserRetaining { get; set; }

    public override uint PerformerTypeId => PerformerId.PI_GS_USER;

    public KGSUser(KTcpSession session, KGameServer server)
        : base(session, new KGSFSM(), server.Region)
    {
        _server = server;
        _repeatFilter.RegRepeatEvent(EventId.EGS_NEW_USER_JOIN_REQ, EventId.EGS_NEW_USER_JOIN_ACK);
        _repeatFilter.RegRepeatEvent(EventId.EGS_VERIFY_ACCOUNT_REQ, EventId.EGS_VERIFY_ACCOUNT_ACK);
        _repeatFilter.RegRepeatEvent(EventId.EGS_CREATE_UNIT_REQ, EventId.EGS_CREATE_UNIT_ACK);
        _repeatFilter.RegRepeatEvent(EventId.EGS_DELETE_UNIT_REQ, EventId.EGS_DELETE_UNIT_ACK);
        _repeatFilter.RegRepeatEvent(EventId.EGS_SELECT_UNIT_REQ, EventId.EGS_SELECT_UNIT_ACK);
        _repeatFilter.RegRepeatEvent(EventId.EGS_CREATE_ROOM_REQ, EventId.EGS_CREATE_ROOM_ACK);
        _repeatFilter.RegRepeatEvent(EventId.EGS_JOIN_ROOM_REQ, EventId.EGS_JOIN_ROOM_ACK);
        _repeatFilter.RegRepeatEvent(EventId.EGS_CREATE_TUTORIAL_ROOM_REQ, EventId.EGS_CREATE_TUTORIAL_ROOM_ACK);
        _repeatFilter.RegRepeatEvent(EventId.EGS_QUICK_JOIN_REQ, EventId.EGS_QUICK_JOIN_ACK);
        _repeatFilter.RegRepeatEvent(EventId.EGS_JOIN_SQUARE_REQ, EventId.EGS_JOIN_SQUARE_ACK);
        _repeatFilter.RegRepeatEvent(EventId.EGS_LEAVE_SQUARE_REQ, EventId.EGS_LEAVE_SQUARE_ACK);
        _repeatFilter.RegRepeatEvent(EventId.EGS_INVITE_PVP_ROOM_REQ, EventId.EGS_INVITE_PVP_ROOM_ACK);
        _repeatFilter.RegRepeatEvent(EventId.EGS_STATE_CHANGE_GAME_INTRUDE_REQ, EventId.EGS_STATE_CHANGE_GAME_INTRUDE_ACK);
        _repeatFilter.RegRepeatEvent(EventId.EGS_INTRUDE_START_REQ, EventId.EGS_INTRUDE_START_ACK);
        _repeatFilter.RegRepeatEvent(EventId.EGS_CHANGE_SLOT_OPEN_REQ, EventId.EGS_CHANGE_SLOT_OPEN_ACK);
        _repeatFilter.RegRepeatEvent(EventId.EGS_CHANGE_WINNING_NUM_KILL_REQ, EventId.EGS_CHANGE_WINNING_NUM_KILL_ACK);
        _repeatFilter.RegRepeatEvent(EventId.EGS_LEAVE_GAME_REQ, EventId.EGS_LEAVE_GAME_ACK);
        _repeatFilter.RegRepeatEvent(EventId.EGS_STATE_CHANGE_RESULT_REQ, EventId.EGS_STATE_CHANGE_RESULT_ACK);
        _repeatFilter.RegRepeatEvent(EventId.EGS_USER_UNIT_RE_BIRTH_POS_REQ, EventId.EGS_USER_UNIT_RE_BIRTH_POS_ACK);
        _repeatFilter.RegRepeatEvent(EventId.EGS_MY_USER_UNIT_INFO_TO_SERVER_REQ, EventId.EGS_MY_USER_UNIT_INFO_TO_SERVER_ACK);
        _repeatFilter.RegRepeatEvent(EventId.EGS_CONNECT_CHANNEL_CHANGE_REQ, EventId.EGS_CONNECT_CHANNEL_CHANGE_ACK);
        _repeatFilter.RegRepeatEvent(EventId.EGS_JOIN_FIELD_CHANNEL_CHANGE_REQ, EventId.EGS_JOIN_FIELD_CHANNEL_CHANGE_ACK);
        _repeatFilter.RegRepeatEvent(EventId.EGS_CHECK_THE_ARCHUANGEL_S_COIN_EVENT_LETTER_REQ, EventId.EGS_CHECK_THE_ARCHUANGEL_S_COIN_EVENT_LETTER_ACK);
        _repeatFilter.RegRepeatEvent(EventId.EGS_GET_SHARE_BANK_REQ, EventId.EGS_GET_SHARE_BANK_ACK);
        _repeatFilter.RegRepeatEvent(EventId.EGS_ITEM_CONVERT_REQ, EventId.EGS_ITEM_CONVERT_ACK);
        _repeatFilter.RegRepeatEvent(EventId.EGS_ITEM_EVALUATE_REQ, EventId.EGS_ITEM_EVALUATE_ACK);
        _repeatFilter.RegRepeatEvent(EventId.EGS_RESTORE_ITEM_EVALUATE_REQ, EventId.EGS_RESTORE_ITEM_EVALUATE_ACK);
        _repeatFilter.RegRepeatEvent(EventId.EGS_SYNTHESIS_ITEM_REQ, EventId.EGS_SYNTHESIS_ITEM_ACK);
        _repeatFilter.RegRepeatEvent(EventId.EGS_SYNTHESIS_SOCKET_GROUPID_REQ, EventId.EGS_SYNTHESIS_SOCKET_GROUPID_ACK);
        _repeatFilter.RegRepeatEvent(EventId.EGS_READY_TO_SOSUN_EVENT_REQ, EventId.EGS_READY_TO_SOSUN_EVENT_ACK);
        _repeatFilter.RegRepeatEvent(EventId.EGS_USE_PROPOSE_ITEM_REQ, EventId.EGS_USE_PROPOSE_ITEM_ACK);
        _repeatFilter.RegRepeatEvent(EventId.EGS_USE_DIVORCE_ITEM_REQ, EventId.EGS_USE_DIVORCE_ITEM_ACK);
        _repeatFilter.RegRepeatEvent(EventId.EGS_USE_RECRUIT_TICKET_REQ, EventId.EGS_USE_RECRUIT_TICKET_ACK);
        _repeatFilter.RegRepeatEvent(EventId.EGS_REGISTER_RECRUITER_REQ, EventId.EGS_REGISTER_RECRUITER_ACK);
        _repeatFilter.RegRepeatEvent(EventId.EGS_ITEM_EXTRACT_REQ, EventId.EGS_ITEM_EXTRACT_ACK);
        _repeatFilter.RegRepeatEvent(EventId.EGS_USE_FINALITY_SKILL_REQ, EventId.EGS_USE_FINALITY_SKILL_ACK);
        _repeatFilter.RegRepeatEvent(EventId.EGS_COUPON_ENTRY_REQ, EventId.EGS_COUPON_ENTRY_ACK);
        _repeatFilter.RegRepeatEvent(EventId.EGS_COUPON_ITEM_CHECK_REQ, EventId.EGS_COUPON_ITEM_CHECK_ACK);
        _repeatFilter.RegRepeatEvent(EventId.EGS_4TH_ANNIV_EVENT_REWARD_REQ, EventId.EGS_4TH_ANNIV_EVENT_REWARD_ACK);
        _repeatFilter.RegRepeatEvent(EventId.EGS_ALL_COMPLETED_QUEST_COMPLETE_REQ, EventId.EGS_ALL_COMPLETED_QUEST_COMPLETE_ACK);
        _repeatFilter.RegRepeatEvent(EventId.EGS_CHANGE_NICK_NAME_REQ, EventId.EGS_CHANGE_NICK_NAME_ACK);
        _repeatFilter.RegRepeatEvent(EventId.EGS_CHARACTER_LIST_REQ, EventId.EGS_CHARACTER_LIST_ACK);
        _repeatFilter.RegRepeatEvent(EventId.EGS_CHAT_OPTION_INFO_WRITE_REQ, EventId.EGS_CHAT_OPTION_INFO_WRITE_ACK);
        _repeatFilter.RegRepeatEvent(EventId.EGS_CREATE_NEW_UNIT_REQ, EventId.EGS_CREATE_UNIT_ACK);
        _repeatFilter.RegRepeatEvent(EventId.EGS_EVENT_CHUNG_GIVE_ITEM_REQ, EventId.EGS_EVENT_CHUNG_GIVE_ITEM_ACK);
        _repeatFilter.RegRepeatEvent(EventId.EGS_EVENT_COBO_DUNGEON_FIELD_REQ, EventId.EGS_EVENT_COBO_DUNGEON_FIELD_ACK);
        _repeatFilter.RegRepeatEvent(EventId.EGS_EVENT_NOTE_VIEW_REQ, EventId.EGS_EVENT_NOTE_VIEW_ACK);
        _repeatFilter.RegRepeatEvent(EventId.EGS_FINAL_DELETE_UNIT_REQ, EventId.EGS_FINAL_DELETE_UNIT_ACK);
        _repeatFilter.RegRepeatEvent(EventId.EGS_GET_EVENT_INFO_REQ, EventId.EGS_GET_EVENT_INFO_ACK);
        _repeatFilter.RegRepeatEvent(EventId.EGS_JUMPING_CHARACTER_REQ, EventId.EGS_JUMPING_CHARACTER_ACK);
        _repeatFilter.RegRepeatEvent(EventId.EGS_KEYBOARD_MAPPING_INFO_WRITE_REQ, EventId.EGS_KEYBOARD_MAPPING_INFO_WRITE_ACK);
        _repeatFilter.RegRepeatEvent(EventId.EGS_KOM_FILE_CHECK_LOG_REQ, EventId.EGS_KOM_FILE_CHECK_LOG_ACK);
        _repeatFilter.RegRepeatEvent(EventId.EGS_LOCAL_RANKING_INQUIRY_REQ, EventId.EGS_LOCAL_RANKING_INQUIRY_ACK);
        _repeatFilter.RegRepeatEvent(EventId.EGS_LOCAL_RANKING_USER_INFO_READ_REQ, EventId.EGS_LOCAL_RANKING_USER_INFO_READ_ACK);
        _repeatFilter.RegRepeatEvent(EventId.EGS_LOCAL_RANKING_USER_INFO_WRITE_REQ, EventId.EGS_LOCAL_RANKING_USER_INFO_WRITE_ACK);
        _repeatFilter.RegRepeatEvent(EventId.EGS_MANUFACTURE_PERIOD_SETTING_REQ, EventId.EGS_MANUFACTURE_PERIOD_SETTING_ACK);
        _repeatFilter.RegRepeatEvent(EventId.EGS_RECOMMEND_USER_REQ, EventId.EGS_RECOMMEND_USER_ACK);
        _repeatFilter.RegRepeatEvent(EventId.EGS_REG_SKILL_NOTE_MEMO_REQ, EventId.EGS_REG_SKILL_NOTE_MEMO_ACK);
        _repeatFilter.RegRepeatEvent(EventId.EGS_RESTORE_UNIT_REQ, EventId.EGS_RESTORE_UNIT_ACK);
        _repeatFilter.RegRepeatEvent(EventId.EGS_USE_SPIRIT_REWARD_REQ, EventId.EGS_USE_SPIRIT_REWARD_ACK);
        _repeatFilter.RegRepeatEvent(EventId.EGS_WARP_BY_BUTTON_REQ, EventId.EGS_WARP_BY_BUTTON_ACK);
    }

    private void SendUserLoginNotification()
    {
        // C++ GetPcBangReward returns the default -1 / zero reward for an unknown type.
        var profile = _server.GameSysValTable.FindPcBangReward(PcBangManager.PcBangType);
        var reward = profile == null ? new KPcBangReward() : new KPcBangReward
        {
            PcBangType = profile.PcBangType,
            ExpRate = profile.ExpRate,
            EdRate = profile.EdRate,
            VsPointRate = profile.VpRate,
            SpiritFree = profile.IsSpiritFree,
            PetExpRate = profile.PetExpRate,
            AdditionalHenirReward = profile.AdditionalHenirRewardCount,
            SecretDungeonFree = profile.IsSecretDungeonFree,
            Avatar = profile.IsUsingPcBangAvatar,
            Title = profile.IsUsingPcBangTitle,
        };
        SendPacket(EventId.ENX_USER_LOGIN_NOT, new KENX_USER_LOGIN_NOT
        {
            IsGameBang = PcBangManager.IsPcBang,
            PcBangReward = reward,
        });
    }

    public new bool SendPacket(EventId eventId, IKSerializable packet)
    {
        LogManager.SaveSendPacket((ushort)eventId);
        _repeatFilter.CheckAckEvent((ushort)eventId);
        return base.SendPacket(eventId, packet);
    }

    public override void Tick()
    {
        base.Tick();
        if (IsDestroyed || ActiveUnit == null)
        {
            _lastEventTickUtc = DateTime.UtcNow;
            return;
        }

        var now = DateTime.UtcNow;
        double deltaSeconds = (now - _lastEventTickUtc).TotalSeconds;
        _lastEventTickUtc = now;
        if (deltaSeconds <= 0) return;

        _sessionConnectedSeconds += deltaSeconds;
        var expiredBuffs = ServerBuffManager.CheckBuffPeriod(now);
        if (expiredBuffs.Count > 0)
        {
            ActiveUnit.BuffInfo = ServerBuffManager.ToPacketBuffInfos(now);
            _server.Router.Send(PerformerTypeId, ActiveUnit.UnitUid, PerformerId.PI_LOGIN_SERVER, _server.LoginServerUid,
                EventId.ELG_UPDATE_USER_UNIT_BUFF_INFO_REQ, new KELG_UPDATE_USER_UNIT_BUFF_INFO_REQ
                {
                    UnitUid = ActiveUnit.UnitUid,
                    DeactivateBuff = expiredBuffs,
                });
        }
        foreach (var completed in _server.GameEventManager.TickUserEvents(ActiveUnit.UnitUid, deltaSeconds, now))
        {
            if (completed.RewardItemId > 0)
                _pendingTimeEventRewardItemIds.Enqueue(completed.RewardItemId);
        }
        CheckTradeBlockUnit(now);
        CheckComeBackUser(now);
        CheckHackingAndMonitoringTimers(now);
    }

    private void CheckComeBackUser(DateTime nowUtc)
    {
        if (!ComeBackUserManager.IsComeBackUser) return;
        var buffEndUtc = ComeBackUserManager.ComeBackBuffEndUtc;
        if (ComeBackUserManager.CheckComeBackBuffEnd(nowUtc))
        {
            if (AccountUid > 0)
                _server.Db.ClearAccountComeBackState(AccountUid);

            const int returnOfHeroBuffId = (int)KBuffManager.BuffTempletId.ReturnOfHero;
            if (ActiveUnit != null && ServerBuffManager.DeactivateBuff(returnOfHeroBuffId))
            {
                ActiveUnit.BuffInfo = ServerBuffManager.ToPacketBuffInfos(nowUtc);
                _server.Router.Send(PerformerTypeId, ActiveUnit.UnitUid, PerformerId.PI_LOGIN_SERVER, _server.LoginServerUid,
                    EventId.ELG_UPDATE_USER_UNIT_BUFF_INFO_REQ, new KELG_UPDATE_USER_UNIT_BUFF_INFO_REQ
                    {
                        UnitUid = ActiveUnit.UnitUid,
                        DeactivateBuff = [returnOfHeroBuffId],
                    });
            }

            SendPacket(EventId.EGS_COME_BACK_BUFF_NOT, new KEGS_COME_BACK_BUFF_NOT
            {
                IsFinish = true,
                ComeBackBuffEnd = buffEndUtc != DateTime.MinValue
                    ? new DateTimeOffset(buffEndUtc).ToUnixTimeSeconds()
                    : 0L,
            });
        }
    }

    private void CheckHackingAndMonitoringTimers(DateTime nowUtc)
    {
        var gsv = _server.GameSysValTable;
        if ((nowUtc - _lastLetterMonitorResetUtc).TotalMinutes >= Math.Max(1, gsv.LetterMonitoringCheckMinutes))
        {
            _lastLetterMonitorResetUtc = nowUtc;
            HackingManager.ClearLetterMonitoringList();
        }
        if ((nowUtc - _lastTradeMonitorResetUtc).TotalMinutes >= Math.Max(1, gsv.TradeMonitoringCheckMinutes))
        {
            _lastTradeMonitorResetUtc = nowUtc;
            HackingManager.ClearTradeMonitoringList();
        }
        if (!HackingManager.CheckHackingUserReceivedPacket(out _, nowUtc))
        {
            SendPacket(EventId.EGS_CHECK_HACK_USER_PACKET_REQUEST_NOT, new KPacketOK { Ok = NetError.NET_OK });
            if (gsv.IsHackUserKick)
                ReserveDestroy();
        }
        CheckDeserializeFailCount(nowUtc);
    }

    private void CheckDeserializeFailCount(DateTime nowUtc)
    {
        if ((nowUtc - _lastDeserializeFailCheckUtc).TotalSeconds < 300.0)
            return;
        _lastDeserializeFailCheckUtc = nowUtc;

        var gsv = _server.GameSysValTable;
        if (!gsv.IsDeserializeFailCheck)
            return;

        if (HackingManager.HackingCheckDeserializeFailCount(gsv) && AuthLevel < 3)
        {
            AbuserManager.IsTradeBlockUser = true;
            HackingUserSendType = KEGS_SERVER_CHECK_HACK_USER_REQ.HctDeserializeFail;
            HackingManager.SetHackingUserSendType(HackingUserSendType, nowUtc);
            SendPacket(EventId.EGS_SERVER_CHECK_HACK_USER_REQ, new KEGS_SERVER_CHECK_HACK_USER_REQ
            {
                HackingCheckType = KEGS_SERVER_CHECK_HACK_USER_REQ.HctDeserializeFail,
            });
        }
    }

    private void InitializeUnitTradeBlockState(KUnitInfo unit)
    {
        var gsv = _server.GameSysValTable;
        var createdUtc = _server.Db.GetUnitCreatedAtUtc(unit.UnitUid);
        _releaseTradeBlockTimeUtc = createdUtc.AddDays(Math.Max(0, gsv.NewUnitTradeBlockDay));
        var now = DateTime.UtcNow;
        bool blocked = gsv.IsNewUnitTradeBlock &&
            ((gsv.NewUnitTradeBlockDay != 0 && now < _releaseTradeBlockTimeUtc) ||
             (gsv.NewUnitTradeBlockUnitClass != 0 && unit.UnitClass < gsv.NewUnitTradeBlockUnitClass));
        AbuserManager.IsTradeBlockUnit = blocked;
    }

    /// <summary>
    /// Faithful C# port of KGSUser::SendSelectUnitACK (GSUserFunction.cpp):
    /// emits KEGS_SELECT_UNIT_1_NOT .. KEGS_SELECT_UNIT_5_NOT followed by KEGS_SELECT_UNIT_ACK.
    /// </summary>
    private void SendSelectUnitACK(KUnitInfo unit)
    {
        var inv = _server.Db.GetInventory(unit.UnitUid);
        var itemMap = new Dictionary<long, KInventoryItemInfo>(inv.Count);
        foreach (var item in inv)
            itemMap[item.ItemUid] = item;

        var (expandGrade, bankSlots) = _server.Db.GetUnitExpandProgress(unit.UnitUid);
        var slotSize = new Dictionary<int, int>
        {
            [1] = 24 + (8 * Math.Max(0, expandGrade)),
            [2] = 24,
            [3] = 24,
            [4] = 24,
            [5] = 24,
            [6] = 24,
            [7] = 24,
            [8] = Math.Max(8, bankSlots),
        };

        var relInfo = new KRelationshipInfo
        {
            RelationshipType = (sbyte)RelationshipManager.CurrentType,
            OtherUnitUid = RelationshipManager.LoverUnitUid,
            OtherNickName = RelationshipManager.LoverNickName,
            LoveWord = RelationshipManager.LoveWord,
            Date = RelationshipManager.AnniversaryDayUtc.HasValue
                ? new DateTimeOffset(RelationshipManager.AnniversaryDayUtc.Value).ToUnixTimeSeconds()
                : 0,
            LastReward = RelationshipManager.LastRewardDateUtc.HasValue
                ? new DateTimeOffset(RelationshipManager.LastRewardDateUtc.Value).ToUnixTimeSeconds()
                : 0,
            RewardTitleType = RelationshipManager.RewardTitleType,
        };

        for (int i = 0; i < KUserSkillPageData.EquippedSkillSlotCount; i++)
        {
            if (unit.UnitSkillData.EquippedSkill[i].SkillId is KUserSkillTree.SiEtcWsCommonLove or KUserSkillTree.SiEtcWsCommonLoveLegacy)
                unit.UnitSkillData.EquippedSkill[i].SkillLevel = (byte)RelationshipManager.CurrentType;
            if (unit.UnitSkillData.EquippedSkillSlotB[i].SkillId is KUserSkillTree.SiEtcWsCommonLove or KUserSkillTree.SiEtcWsCommonLoveLegacy)
                unit.UnitSkillData.EquippedSkillSlotB[i].SkillLevel = (byte)RelationshipManager.CurrentType;
        }

        var gsv = _server.GameSysValTable;
        var createdUtc = _server.Db.GetUnitCreatedAtUtc(unit.UnitUid);
        var not1 = new KEGS_SELECT_UNIT_1_NOT
        {
            Ok = NetError.NET_OK,
            UnitInfo = unit,
            UnitCreateDate = createdUtc.ToString("yyyy-MM-dd HH:mm:ss"),
            RealDataED = unit.Ed,
            UnitLastLoginDate = string.IsNullOrEmpty(unit.LastLoginTime)
                ? DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
                : unit.LastLoginTime,
            InventorySlotSize = slotSize,
            Item = itemMap,
            NumResurrectionStone = NumResurrectionStone,
            NumAutoPaymentResStone = NumAutoPaymentResStone,
            UserSkillPageData = SkillTree.GetAllSkillPageDatas(),
            SkillUnsealed = SkillTree.GetUnsealedSkillIds(),
            SkillNoteMaxPageNum = SkillTree.SkillNoteMaxPageNum,
            SkillNote = SkillTree.GetSkillNotePacketMap(),
            IsNewUnitTradeBlock = AbuserManager.IsTradeBlockUnit,
            NewUnitTradeBlockDay = gsv.NewUnitTradeBlockDay,
            NewUnitTradeBlockUnitClass = gsv.NewUnitTradeBlockUnitClass,
            RelationshipInfo = relInfo,
        };
        SendPacket(EventId.EGS_SELECT_UNIT_1_NOT, not1);

        var not2 = new KEGS_SELECT_UNIT_2_NOT
        {
            Ok = NetError.NET_OK,
            Quest = QuestManager.GetActiveQuests(),
            CompletQuest = QuestManager.GetCompletedQuests(),
        };
        SendPacket(EventId.EGS_SELECT_UNIT_2_NOT, not2);

        var not3 = new KEGS_SELECT_UNIT_3_NOT
        {
            Ok = NetError.NET_OK,
            Mission = TitleManager.GetMissions(),
            Title = TitleManager.GetTitles(),
            SummonedPetUid = PetManager.SummonedPetUid,
            PetList = PetManager.GetAllPets(),
        };
        SendPacket(EventId.EGS_SELECT_UNIT_3_NOT, not3);

        var blackList = BlackList.GetBlackListSnapshot()
            .Select(kv => new KChatBlackListUnit { UnitUid = kv.Key, NickName = kv.Value })
            .ToList();
        var messenger = new KMessengerInfo
        {
            FriendInfo = new Dictionary<long, KFriendInfo>(_friends),
            Group = new Dictionary<sbyte, string>(_friendGroups),
        };
        var not4 = new KEGS_SELECT_UNIT_4_NOT
        {
            Ok = NetError.NET_OK,
            ChatBlackList = blackList,
            DenyOptions = DenyOptions,
            IsRecommend = RecommendManager.IsRecommend,
            RecommendUnitUid = RecommendManager.RecommendedUnitUid,
            MessengerInfo = messenger,
            GamePlayStatus = new KGamePlayStatusContainer { GamePlayStatus = [_gamePlayStatus] },
        };
        SendPacket(EventId.EGS_SELECT_UNIT_4_NOT, not4);

        var pshopAgency = new KUserPShopAgencyInfo
        {
            UserUid = AccountUid,
            UnitUid = unit.UnitUid,
            NickName = unit.NickName,
        };
        var henirRewardCount = new KEGS_HENIR_REWARD_COUNT_NOT
        {
            UnLimited = DungeonManager.UnlimitedHenirRewardEvent,
            Normal = DungeonManager.PossibleNormalCount,
            Premium = DungeonManager.PossiblePremiumCount,
            EventVal = DungeonManager.PossibleEventCount,
            PremiumMAX = gsv.GetAdditionalHenirRewardCount(PcBangManager.PcBangType),
            EventMAX = DungeonManager.HenirRewardEventLimitCount,
        };
        var not5 = new KEGS_SELECT_UNIT_5_NOT
        {
            Ok = NetError.NET_OK,
            IsSpiritUpdated = false,
            PShopAgencyInfo = pshopAgency,
            PacketHenirRewardCount = henirRewardCount,
            CashShopOpen = _server.ContentManager.IsEnableCashShop,
            FirstSelect = false,
            Couple = !RelationshipManager.IsSolo,
            RelationTargetUserUid = RelationshipManager.LoverUnitUid,
            RelationTargetUserNickname = RelationshipManager.LoverNickName,
            Vec4ThAnnivEventRewardInfo = _anniv4RewardFlags.ToList(),
            LastRewardTime = _lastAnnivRewardUtc > DateTime.MinValue
                ? new DateTimeOffset(_lastAnnivRewardUtc).ToUnixTimeSeconds()
                : 0,
        };
        SendPacket(EventId.EGS_SELECT_UNIT_5_NOT, not5);

        SendPacket(EventId.EGS_SELECT_UNIT_ACK, new KEGS_SELECT_UNIT_ACK { Ok = NetError.NET_OK });
    }

    private void CheckTradeBlockUnit(DateTime nowUtc)
    {
        if (ActiveUnit == null || !AbuserManager.IsTradeBlockUnit) return;
        if (Fsm.StateId <= KGSFSM.S_SERVER_SELECT) return;
        var gsv = _server.GameSysValTable;
        if (gsv.NewUnitTradeBlockDay != 0 && nowUtc < _releaseTradeBlockTimeUtc)
            return;
        if (gsv.NewUnitTradeBlockUnitClass != 0 && ActiveUnit.UnitClass < gsv.NewUnitTradeBlockUnitClass)
            return;

        AbuserManager.IsTradeBlockUnit = false;
        SendPacket(EventId.EGS_NEW_CHARACTER_TRADE_BLOCK_MSG_NOT, new KEGS_NEW_CHARACTER_TRADE_BLOCK_MSG_NOT
        {
            NewCharTBMsg = $"{ActiveUnit.NickName};",
        });

        const int tradeBlockBuffId = (int)KBuffManager.BuffTempletId.TradeBlock;
        if (ServerBuffManager.DeactivateBuff(tradeBlockBuffId))
        {
            ActiveUnit.BuffInfo = ServerBuffManager.ToPacketBuffInfos(nowUtc);
            _server.Router.Send(PerformerTypeId, ActiveUnit.UnitUid, PerformerId.PI_LOGIN_SERVER, _server.LoginServerUid,
                EventId.ELG_UPDATE_USER_UNIT_BUFF_INFO_REQ, new KELG_UPDATE_USER_UNIT_BUFF_INFO_REQ
                {
                    UnitUid = ActiveUnit.UnitUid,
                    DeactivateBuff = [tradeBlockBuffId],
                });
        }
    }

    public override void OnDestroy()
    {
        if (IsDestroyed) return;
        base.OnDestroy();
        LeaveCurrentField();
        if (ActiveUnit != null)
        {
            _server.EventDelaySystemManager.UnRegOpenRandomCubeEvent(ActiveUnit.UnitUid);
            if (CurrentRoomUid > 0)
                _server.Center.RoomManager.LeaveRoom(CurrentRoomUid, ActiveUnit.UnitUid);
            if (CurrentPartyUid > 0)
                _server.Center.PartyManager.LeaveParty(CurrentPartyUid, ActiveUnit.UnitUid);
            _server.TradeManager.CancelByUnit(ActiveUnit.UnitUid);
            _server.GameEventManager.ResetConnectTimersOnDisconnect(ActiveUnit.UnitUid);
            _server.Global.MatchMakingManager.Cancel(ActiveUnit.UnitUid);
            _server.Global.AutoPartyManager.Cancel(ActiveUnit.UnitUid);
        }
        InventoryLocker.Clear();
        UnitManager.Clear();
        if (AccountUid > 0)
            _server.ChannelingUserManager.UpdateChannelingUserList(login: false, (int)Region.DefaultChannelingCode, gameServerLoggedIn: true);
        Fsm.TryStateTransition(KGSFSM.I_TO_EXIT, out _);
        _server.OnUserDisconnected(this);
    }

    protected override void ProcessEvent(KEvent ev)
    {
        LogManager.SaveProcessEvent(ev.EventIdValue);
        if (ev.FromType == EventFromType.Client && !_repeatFilter.CheckReqEvent(ev.EventIdValue))
            return;

        switch (ev.EventId)
        {
            case EventId.E_HEART_BEAT:
                break;

            case EventId.E_DISABLE_HB_CHECK_REQ:
                DisableHeartbeatCheck = true;
                break;

            case EventId.E_RESERVE_DESTROY:
                ReserveDestroy();
                break;

            case EventId.E_KICK_USER_NOT:
            {
                var ks = new KSerializer(Region);
                ks.BeginReading(ev.Buffer);
                ks.Get(out int kickReason);
                ks.EndReading();
                if (kickReason == NetError.ERR_ADMIN_COMMAND_07)
                {
                    HackingUserSendType = KEGS_SERVER_CHECK_HACK_USER_REQ.HctKickByGm;
                    HackingManager.SetHackingUserSendType(HackingUserSendType);
                    SendPacket(EventId.EGS_SERVER_CHECK_HACK_USER_REQ, new KEGS_SERVER_CHECK_HACK_USER_REQ
                    {
                        HackingCheckType = KEGS_SERVER_CHECK_HACK_USER_REQ.HctKickByGm,
                    });
                }
                else if (kickReason == NetError.ERR_ACCOUNT_BLOCK_00)
                {
                    SendID(EventId.EGS_ACCOUNT_BLOCK_USER_POPUP_NOT);
                }
                ReserveDestroy();
                break;
            }

            case EventId.E_CHECK_SEQUENCE_COUNT_NOT:
            {
                if (!_checkSequenceCount)
                {
                    _checkSequenceCount = true;
                    HackingUserSendType = KEGS_SERVER_CHECK_HACK_USER_REQ.HctCheckSequenceCount;
                    HackingManager.SetHackingUserSendType(HackingUserSendType);
                    SendPacket(EventId.EGS_SERVER_CHECK_HACK_USER_REQ, new KEGS_SERVER_CHECK_HACK_USER_REQ
                    {
                        HackingCheckType = KEGS_SERVER_CHECK_HACK_USER_REQ.HctCheckSequenceCount,
                    });
                }
                break;
            }

            case EventId.EGS_CONNECT_REQ:
            {
                if (Fsm.StateId != KGSFSM.S_INIT) break;
                var req = ev.GetPayload<KEGS_CONNECT_REQ>(Region);
                bool versionOk = string.IsNullOrEmpty(_server.MainVersion) ||
                    string.Equals(req.Version, _server.MainVersion, StringComparison.Ordinal);
                SendPacket(EventId.EGS_CONNECT_ACK, new KEGS_CONNECT_ACK
                {
                    Ok = versionOk ? NetError.NET_OK : NetError.ERR_CONNECT_00,
                    UdpPort = _server.UdpEchoPort,
                    ChannelId = 1,
                });
                break;
            }

            case EventId.EGS_NEW_USER_JOIN_REQ:
            {
                var req = ev.GetPayload<KEGS_NEW_USER_JOIN_REQ>(Region);
                var (ok, uid, _) = _server.Db.CreateAccount(Region, req.Id, req.Password);
                SendPacket(EventId.EGS_NEW_USER_JOIN_ACK, new KEGS_NEW_USER_JOIN_ACK
                {
                    Ok = ok ? NetError.NET_OK : Region.ResolveNetError(NetErrorKey.DuplicateId),
                    UserUid = uid,
                });
                break;
            }

            case EventId.EGS_VERIFY_ACCOUNT_REQ:
            {
                var req = ev.GetPayload<KUserAuthenticateReq>(Region);
                var (netErr, userUid, authLevel, isGuest, _) = _server.Db.VerifyGameCredential(
                    Region.Code, req.UserId, req.Password);

                if (netErr == NetError.NET_OK && userUid > 0)
                {
                    AccountUid = userUid;
                    UserId = req.UserId;
                    AuthLevel = authLevel;
                    IsGuestUser = isGuest;

                    bool useSecondPw = false;
                    if (!Region.DisableSecondSecurity)
                    {
                        var sec = _server.Db.GetSecondSecurityStatus(userUid);
                        useSecondPw = sec.useSecondPw;
                    }

                    SecondSecurityVerified = !useSecondPw;
                    AntiAddictionManager.Initialize(Region, isAdult: true, TimeSpan.Zero, TimeSpan.Zero);

                    var (lastLoginUtc, savedStep, savedBuffEndStr) = _server.Db.GetAccountComeBackInfo(userUid);
                    if (!string.IsNullOrEmpty(savedBuffEndStr) &&
                        DateTime.TryParse(savedBuffEndStr, null, System.Globalization.DateTimeStyles.RoundtripKind, out var savedEndUtc) &&
                        savedEndUtc.ToUniversalTime() > DateTime.UtcNow)
                    {
                        ComeBackUserManager.IsComeBackUser = true;
                        ComeBackUserManager.IsComeBackFirst = false;
                        ComeBackUserManager.ComeBackRewardStep = (KGameSysValTable.ComeBackStep)savedStep;
                        ComeBackUserManager.SetComeBackBuffEndTime(savedBuffEndStr);
                    }
                    else
                    {
                        ComeBackUserManager.EvaluateFromLastLogin(lastLoginUtc);
                    }
                    _server.Db.UpdateAccountLoginTimestamp(userUid);

                    Fsm.TryStateTransition(KGSFSM.I_TO_LOGINED, out _);
                    if (useSecondPw)
                        Fsm.TryStateTransition(KGSFSM.I_TO_CHECK_SECOND_PW, out _);
                    else
                        Fsm.TryStateTransition(KGSFSM.I_TO_SERVER_SELECT, out _);

                    _server.RegisterAccountSession(this);
                    _server.ChannelingUserManager.UpdateChannelingUserList(login: true, (int)Region.DefaultChannelingCode);

                    SendPacket(EventId.EGS_VERIFY_ACCOUNT_ACK, new KEGS_VERIFY_ACCOUNT_ACK
                    {
                        Ok = NetError.NET_OK,
                        ClientIp = IpStr,
                        ChannelCode = (byte)Region.DefaultChannelingCode,
                        ChannelUserId = req.UserId,
                        AccountInfo = new KAccountInfo
                        {
                            UserUid = userUid,
                            Id = req.UserId,
                            Name = req.UserId,
                            AuthLevel = authLevel,
                            IsGuestUser = isGuest,
                            LastLogin = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                        },
                    });

                    SendUserLoginNotification();

                    if (!Region.DisableSecondSecurity)
                    {
                        SendPacket(EventId.EGS_SECOND_SECURITY_INFO_NOT, new KEGS_SECOND_SECURITY_INFO_NOT
                        {
                            Ok = NetError.NET_OK,
                            UseSecondPw = useSecondPw,
                        });
                    }
                }
                else
                {
                    SendPacket(EventId.EGS_VERIFY_ACCOUNT_ACK, new KEGS_VERIFY_ACCOUNT_ACK
                    {
                        Ok = netErr,
                        ClientIp = IpStr,
                    });
                }
                break;
            }

            case EventId.EGS_CREATE_SECOND_SECURITY_REQ:
            {
                if (AccountUid <= 0) break;
                var req = ev.GetPayload<KEGS_CREATE_SECOND_SECURITY_REQ>(Region);
                _server.Db.CreateSecondSecurityPin(AccountUid, req.SecondPw);
                SecondSecurityVerified = true;
                SendPacket(EventId.EGS_CREATE_SECOND_SECURITY_ACK, new KPacketOK { Ok = NetError.NET_OK });
                break;
            }

            case EventId.EGS_AUTH_SECOND_SECURITY_REQ:
            {
                if (AccountUid <= 0) break;
                var req = ev.GetPayload<KEGS_AUTH_SECOND_SECURITY_REQ>(Region);
                var (ok, failedCount) = _server.Db.AuthSecondSecurityPin(AccountUid, req.SecondPw);
                if (ok)
                {
                    SecondSecurityVerified = true;
                    Fsm.TryStateTransition(KGSFSM.I_TO_SERVER_SELECT, out _);
                }
                SendPacket(EventId.EGS_AUTH_SECOND_SECURITY_ACK, new KEGS_AUTH_SECOND_SECURITY_ACK
                {
                    Ok = ok ? NetError.NET_OK : NetError.ERR_VERIFY_14,
                    FailedCount = failedCount,
                });
                break;
            }

            // -------- Unit Selection / Management --------

            case EventId.EGS_MY_UNIT_AND_INVENTORY_INFO_LIST_REQ:
            {
                if (AccountUid <= 0) break;
                var units = _server.Db.GetUnitsForAccount(Region, AccountUid);
                foreach (var u in units)
                    u.Port = _server.UdpRelayPort;

                SendPacket(EventId.EGS_MY_UNIT_AND_INVENTORY_INFO_LIST_ACK,
                    new KEGS_MY_UNIT_AND_INVENTORY_INFO_LIST_ACK
                    {
                        Ok = NetError.NET_OK,
                        UnitSlot = 8,
                        Units = units,
                    });
                break;
            }

            case EventId.EGS_CREATE_UNIT_REQ:
            {
                if (AccountUid <= 0) break;
                var req = ev.GetPayload<KEGS_CREATE_UNIT_REQ>(Region);
                if (!_server.StringFilterManager.CheckIsValidString(KStringFilterManager.FilterType.FtNickName, req.NickName))
                {
                    SendPacket(EventId.EGS_CREATE_UNIT_ACK, new KEGS_CREATE_UNIT_ACK { Ok = NetError.ERR_CREATE_UNIT_02 });
                    break;
                }
                var (_, unit, err) = _server.Db.CreateUnit(Region, AccountUid, req.NickName, req.UnitClass);
                SendPacket(EventId.EGS_CREATE_UNIT_ACK, new KEGS_CREATE_UNIT_ACK
                {
                    Ok = err,
                    UnitInfo = unit ?? new KUnitInfo(),
                });
                break;
            }

            case EventId.EGS_DELETE_UNIT_REQ:
            {
                if (AccountUid <= 0) break;
                var req = ev.GetPayload<KEGS_DELETE_UNIT_REQ>(Region);
                var targetUnit = _server.Db.GetUnitByUid(Region, req.UnitUid);
                if (targetUnit == null || targetUnit.OwnerUserUid != AccountUid)
                {
                    SendPacket(EventId.EGS_DELETE_UNIT_ACK, new KEGS_DELETE_UNIT_ACK
                    {
                        Ok = NetError.ERR_DELETE_UNIT_00,
                        UnitUid = req.UnitUid,
                    });
                    break;
                }
                if (targetUnit.UserGuildInfo.GuildUid > 0)
                {
                    SendPacket(EventId.EGS_DELETE_UNIT_ACK, new KEGS_DELETE_UNIT_ACK
                    {
                        Ok = NetError.ERR_DELETE_UNIT_02,
                        UnitUid = req.UnitUid,
                    });
                    break;
                }
                bool ok = _server.Db.DeleteUnit(Region, AccountUid, req.UnitUid);
                SendPacket(EventId.EGS_DELETE_UNIT_ACK, new KEGS_DELETE_UNIT_ACK
                {
                    Ok = ok ? NetError.NET_OK : NetError.ERR_DELETE_UNIT_00,
                    UnitUid = req.UnitUid,
                    GuildUid = targetUnit.UserGuildInfo.GuildUid,
                    NickName = targetUnit.NickName,
                    Level = targetUnit.Level,
                    UnitClass = targetUnit.UnitClass,
                });
                break;
            }

            case EventId.EGS_SELECT_UNIT_REQ:
            {
                if (AccountUid <= 0) break;
                var req = ev.GetPayload<KEGS_SELECT_UNIT_REQ>(Region);
                var unit = _server.Db.GetUnitByUid(Region, req.UnitUid);
                if (unit == null || unit.OwnerUserUid != AccountUid)
                {
                    SendPacket(EventId.EGS_SELECT_UNIT_ACK, new KEGS_SELECT_UNIT_ACK { Ok = NetError.ERR_SELECT_UNIT_00 });
                    break;
                }

                ActiveUnit = unit;
                ActiveUnit.Ip = IpStr;
                ActiveUnit.Port = _server.UdpRelayPort;
                int resolvedMaxSpirit = Math.Max(unit.SpiritMax, _server.SpiritTable.GetSpiritOrDefault(unit.Level));
                SpiritManager.Init(unit.Spirit, resolvedMaxSpirit);
                EdManager.Init(unit.Ed);
                SkillTree.Init(unit.Level, _server.Db.GetUnitSkillSlots(unit.UnitUid));
                var (spCount, spActive, spNotes) = _server.Db.GetUnitSkillPageState(unit.UnitUid);
                SkillTree.InitSkillPageState(spCount, spActive, spNotes);
                var evState = _server.Db.GetUserEventState(unit.UnitUid);
                _oldYearMissionRewardedLevel = evState.oldYearRewardedLevel;
                _newYearMissionStepId = evState.newYearStepId;
                _checkPowerTime = evState.checkPowerTime;
                _checkPowerCount = evState.checkPowerCount;
                _checkPowerScore = evState.checkPowerScore;
                _noteViewCount = evState.noteViewCount;
                Array.Copy(evState.chinaSpirit, _chinaSpirit, 6);
                if (!string.IsNullOrEmpty(evState.jumpingDate))
                    _jumpingRegDate = evState.jumpingDate;
                AbuserManager.Clear(onlyCharAbuser: true);
                InitializeUnitTradeBlockState(unit);
                ComeBackUserManager.IsUnitSelect = true;
                if (ComeBackUserManager.IsComeBackUser)
                {
                    if (ComeBackUserManager.IsComeBackFirst &&
                        ComeBackUserManager.TryGiveComeBackRewardCube(out int cbCubeId, out var cbBuffEndUtc, out int cbDays))
                    {
                        _server.Db.GrantUnitItem(unit.UnitUid, cbCubeId, 1);
                        _server.Db.SaveAccountComeBackReward(AccountUid, (int)ComeBackUserManager.ComeBackRewardStep, cbBuffEndUtc.ToString("O"));
                    }
                    var cbEnd = ComeBackUserManager.ComeBackBuffEndUtc;
                    if (cbEnd > DateTime.UtcNow)
                    {
                        int remainSec = Math.Max(60, (int)(cbEnd - DateTime.UtcNow).TotalSeconds);
                        ServerBuffManager.ActivateBuff(new KUserServerBuffManager.ServerBuffInfo
                        {
                            BuffId = (int)KBuffManager.BuffTempletId.ReturnOfHero,
                            ExpRate = 0.2f,
                            ExpireUtc = cbEnd,
                        });
                        SendPacket(EventId.EGS_COME_BACK_BUFF_NOT, new KEGS_COME_BACK_BUFF_NOT
                        {
                            IsFinish = false,
                            ComeBackBuffEnd = new DateTimeOffset(cbEnd).ToUnixTimeSeconds(),
                        });
                    }
                }
                HackingManager.ClearLetterMonitoringList();
                HackingManager.ClearTradeMonitoringList();
                ItemQuantityVariation.Clear();
                TutorialInfo.ResetData();
                LocalRankingInfo.ResetUnitInfo();
                GuildManager.Clear();
                Statistics.Clear();
                EventManager.Clear();
                PvpManager.Clear();
                PartyManager.Clear();
                CheatManager.ObserverMode = false;
                LogManager.Clear();
                RankingManager.Clear();
                InventoryLocker.Clear();
                UnitManager.Init(null, unit.GameStat.BaseHp);
                _defenceQuickSlotItems.Clear();
                _server.Db.DeleteUnitCategoryItems(unit.UnitUid, 12); // ST_PC_BANG stale cleanup
                bool usePcBangTitle = PcBangManager.IsPcBang && _server.GameSysValTable.IsUsingPcBangTitle(PcBangManager.PcBangType);
                TitleManager.Init(unit.TitleId, usePcBangTitle: usePcBangTitle);
                _dailyGiftBoxList.Clear();
                foreach (var dgb in _server.Db.GetDailyGiftBoxList(AccountUid))
                {
                    if (!_dailyGiftBoxList.TryGetValue(dgb.ItemId, out var list))
                    {
                        list = [];
                        _dailyGiftBoxList[dgb.ItemId] = list;
                    }
                    list.Add((dgb.UnitUid, dgb.State, dgb.RegDate));
                }
                if (PcBangManager.IsPcBang)
                {
                    int unitType = _server.ClassChangeTable.GetClassGroupId(unit.UnitClass, out int cg) ? cg : (int)unit.UnitClass;
                    foreach (int pcItemId in _server.GameSysValTable.GetPcBangPremiumItems(unitType, PcBangManager.PcBangType))
                    {
                        if (_server.Db.GetTotalItemCountByItemId(unit.UnitUid, pcItemId) <= 0)
                            _server.Db.GrantUnitItem(unit.UnitUid, pcItemId, 1);
                    }
                }
                if (_server.ExpandPeriodItems.Count > 0)
                {
                    foreach (var invItem in _server.Db.GetInventory(unit.UnitUid))
                    {
                        if (_server.ExpandPeriodItems.TryGetValue(invItem.ItemInfo.ItemId, out int expandDays) && expandDays > 0)
                        {
                            string expDate = DateTime.UtcNow.AddDays(expandDays).ToString("yyyy-MM-dd HH:mm:ss");
                            _server.Db.UpdateItemPeriod(unit.UnitUid, invItem.ItemUid, (short)expandDays, expDate);
                        }
                    }
                }
                DungeonManager.InitClearedDungeons(unit.DungeonClear.Keys);
                DungeonManager.Init(
                    unlimitedEvent: false,
                    eventLimit: 0,
                    normalUsed: 0,
                    premiumUsed: 0,
                    eventUsed: 0,
                    challengeNormalUsed: 0,
                    isPcBang: PcBangManager.IsPcBang,
                    pcBangType: PcBangManager.PcBangType,
                    additionalHenirRewardCount: _server.GameSysValTable.GetAdditionalHenirRewardCount(PcBangManager.PcBangType));
                var (dbActiveQuests, dbCompletedQuests) = _server.Db.GetUnitQuests(unit.UnitUid);
                QuestManager.Init(dbActiveQuests, dbCompletedQuests);
                PetManager.Init(_server.Db.GetPets(unit.UnitUid));
                RidingPetManager.Init(_server.Db.GetRidingPets(unit.UnitUid), force: true);
                LetterBox.Init(unit.NickName, _server.Db.GetLettersForUnit(unit.UnitUid));
                foreach (string blackNick in _server.Db.GetPostBlackList(unit.UnitUid))
                    LetterBox.AddBlackList(blackNick);
                foreach (var (targetUid, targetNick) in _server.Db.GetBlackListUsers(unit.UnitUid))
                    BlackList.AddNewBlackListUser(targetUid, targetNick);
                _friendGroups.Clear();
                foreach (var kv in _server.Db.GetFriendGroups(unit.UnitUid))
                    _friendGroups[kv.Key] = kv.Value;
                _friendUids.Clear();
                _blockedFriendUids.Clear();
                _friends.Clear();
                foreach (var (friendUid, groupId, isBlocked) in _server.Db.GetFriendList(unit.UnitUid))
                {
                    _friendUids.Add(friendUid);
                    if (isBlocked) _blockedFriendUids.Add(friendUid);
                    var friendUnit = _server.Db.GetUnitByUid(Region, friendUid);
                    _friends[friendUid] = new KFriendInfo
                    {
                        UnitUid = friendUid,
                        NickName = friendUnit?.NickName ?? string.Empty,
                        GroupId = groupId,
                        Status = isBlocked ? KFriendInfo.FsBlocked : KFriendInfo.FsNormal,
                        Position = _server.GetUserByUnitUid(friendUid) != null ? KFriendInfo.FpOnLine : KFriendInfo.FpOffLine,
                    };
                }
                var rel = _server.Db.GetRelationship(unit.UnitUid);
                if (rel != null)
                {
                    RelationshipManager.SetRelationshipType((KUserRelationshipManager.RelationshipType)rel.Value.relType);
                    RelationshipManager.LoverUnitUid = rel.Value.loverUid;
                    RelationshipManager.LoverNickName = rel.Value.loverNick;
                    RelationshipManager.LoveWord = rel.Value.loveWord;
                    RelationshipManager.AnniversaryDayUtc = rel.Value.anniversaryUtc;
                    RelationshipManager.LastRewardDateUtc = rel.Value.lastRewardUtc;
                    RelationshipManager.RewardTitleType = rel.Value.rewardTitleType;
                }
                else
                {
                    RelationshipManager.SetBreakUp();
                }
                _ = _server.StatTable.GetUnitStat(unit.UnitClass, unit.Level);
                if (unit.AuthLevel < 2 && _server.MapDatas.Count > 0 &&
                    _server.CheckMapRequireData(unit.LastPos.MapId, unit.Level, unit.DungeonClear) != NetError.NET_OK)
                {
                    unit.LastPos.MapId = _server.GetPossibleEnterVillageMapId(unit.Level, unit.DungeonClear, unit.LastPos.MapId);
                }
                _server.RegisterActiveUnit(this);

                SendSelectUnitACK(unit);
                SendPacket(EventId.EGS_GET_MY_INVENTORY_ACK, new KEGS_GET_MY_INVENTORY_ACK
                {
                    Ok = NetError.NET_OK,
                    IsRecommend = RecommendManager.IsRecommend,
                    SharingBank = false,
                });
                break;
            }

            case EventId.EGS_GET_MY_INVENTORY_REQ:
            {
                if (ActiveUnit == null) break;
                var inv = _server.Db.GetInventory(ActiveUnit.UnitUid);
                foreach (var group in inv.GroupBy(i => (int)i.SlotCategory))
                {
                    var not = new KEGS_SELECT_UNIT_INVENTORY_INFO_NOT { SlotCategory = group.Key };
                    foreach (var item in group)
                        not.Items[item.ItemUid] = item;
                    SendPacket(EventId.EGS_SELECT_UNIT_INVENTORY_INFO_NOT, not);
                }
                SendPacket(EventId.EGS_GET_MY_INVENTORY_ACK, new KEGS_GET_MY_INVENTORY_ACK
                {
                    Ok = NetError.NET_OK,
                    IsRecommend = RecommendManager.IsRecommend,
                    SharingBank = false,
                });
                break;
            }

            // -------- Field / Village & Chat --------

            case EventId.EGS_STATE_CHANGE_FIELD_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_STATE_CHANGE_FIELD_REQ>(Region);
                int targetMapId = req.MapId > 0 ? req.MapId : ActiveUnit.LastPos.MapId;
                if (ActiveUnit.AuthLevel < 2)
                {
                    int mapCheckErr = _server.CheckMapRequireData(targetMapId, ActiveUnit.Level, ActiveUnit.DungeonClear);
                    if (mapCheckErr != NetError.NET_OK)
                    {
                        SendPacket(EventId.EGS_STATE_CHANGE_FIELD_ACK, new KEGS_STATE_CHANGE_FIELD_ACK
                        {
                            Ok = mapCheckErr,
                        });
                        break;
                    }
                }
                LeaveCurrentField();
                CurrentFieldMapId = targetMapId;
                ActiveUnit.LastPos.MapId = CurrentFieldMapId;
                Fsm.TryStateTransition(KGSFSM.I_TO_FIELD_MAP, out _);

                var zone = _server.FieldManager.GetOrCreateZone(CurrentFieldMapId);
                var myFieldInfo = BuildFieldUserInfo();
                var sectorUsers = zone.Enter(myFieldInfo);

                SendPacket(EventId.EGS_STATE_CHANGE_FIELD_ACK, new KEGS_STATE_CHANGE_FIELD_ACK
                {
                    Ok = NetError.NET_OK,
                });

                SendPacket(EventId.EGS_CHANGE_USER_LIST_IN_SECTOR_NOT,
                    new KEGS_CHANGE_USER_LIST_IN_SECTOR_NOT { NewUsers = sectorUsers });

                foreach (long otherUid in zone.GetOtherUnitUids(ActiveUnit.UnitUid))
                {
                    _server.SendToUnit(otherUid, EventId.EGS_CHANGE_USER_LIST_IN_SECTOR_NOT,
                        new KEGS_CHANGE_USER_LIST_IN_SECTOR_NOT { NewUsers = [myFieldInfo] });
                }
                break;
            }

            case EventId.EGS_FIELD_UNIT_SYNC_DATA_NOT:
            {
                if (ActiveUnit == null || CurrentFieldMapId <= 0) break;
                var req = ev.GetPayload<KEGS_FIELD_UNIT_SYNC_DATA_NOT>(Region);
                var zone = _server.FieldManager.GetZone(CurrentFieldMapId);
                if (zone == null) break;

                var delta = zone.UpdatePositionWithSectorDelta(ActiveUnit.UnitUid, req.Pos);

                if (delta.LeftUserUids.Count > 0 || delta.EnteredUsers.Count > 0)
                {
                    SendPacket(EventId.EGS_CHANGE_USER_LIST_IN_SECTOR_NOT, new KEGS_CHANGE_USER_LIST_IN_SECTOR_NOT
                    {
                        NewUsers = delta.EnteredUsers,
                        DelUsers = delta.LeftUserUids,
                    });

                    var myFieldInfo = BuildFieldUserInfo();
                    var enterNot = new KEGS_CHANGE_USER_LIST_IN_SECTOR_NOT { NewUsers = [myFieldInfo] };
                    foreach (long targetUid in delta.NotifyEnterTargets)
                        _server.SendToUnit(targetUid, EventId.EGS_CHANGE_USER_LIST_IN_SECTOR_NOT, enterNot);

                    var leaveNot = new KEGS_CHANGE_USER_LIST_IN_SECTOR_NOT { DelUsers = [ActiveUnit.UnitUid] };
                    foreach (long targetUid in delta.NotifyLeaveTargets)
                        _server.SendToUnit(targetUid, EventId.EGS_CHANGE_USER_LIST_IN_SECTOR_NOT, leaveNot);
                }

                var broad = new KEGS_FIELD_UNIT_SYNC_DATA_BROAD_NOT
                {
                    UnitUid = ActiveUnit.UnitUid,
                    Pos = req.Pos,
                    FieldFrames = req.FieldFrames,
                    IsRight = req.IsRight,
                };
                foreach (long otherUid in delta.NearbyUsers)
                    _server.SendToUnit(otherUid, EventId.EGS_FIELD_UNIT_SYNC_DATA_BROAD_NOT, broad);
                break;
            }

            case EventId.EGS_CHAT_REQ:
            {
                if (ActiveUnit == null) break;
                if (PunishmentManager.IsAppliedPunishment((int)KPunishmentManager.PunishmentType.PtChat))
                {
                    SendPacket(EventId.EGS_CHAT_ACK, new KPacketOK { Ok = NetError.ERR_GM_CHAR_DENY_00 });
                    break;
                }
                var req = ev.GetPayload<KEGS_CHAT_REQ>(Region);
                string filteredMsg = _server.StringFilterManager.FilteringChatString(req.Message);
                var chatNot = new KEGS_CHAT_NOT
                {
                    ChatPacketType = req.ChatPacketType,
                    SenderUnitUid = ActiveUnit.UnitUid,
                    SenderNickName = ActiveUnit.NickName,
                    ReceiverNickName = req.ReceiverNickName,
                    ReceiverUnitUid = req.ToUnitUid,
                    RoomChatType = req.RoomChatType,
                    Message = filteredMsg,
                };

                SendPacket(EventId.EGS_CHAT_ACK, new KPacketOK { Ok = NetError.NET_OK });

                if (req.ToUnitUid > 0)
                {
                    _server.SendToUnit(req.ToUnitUid, EventId.EGS_CHAT_NOT, chatNot);
                    SendPacket(EventId.EGS_CHAT_NOT, chatNot);
                }
                else if (CurrentRoomUid > 0)
                {
                    var room = _server.Center.RoomManager.GetRoom(CurrentRoomUid);
                    if (room != null)
                    {
                        foreach (long uid in room.GetMemberUnitUids())
                            _server.SendToUnit(uid, EventId.EGS_CHAT_NOT, chatNot);
                    }
                }
                else if (CurrentFieldMapId > 0)
                {
                    var zone = _server.FieldManager.GetZone(CurrentFieldMapId);
                    if (zone != null)
                    {
                        foreach (long uid in zone.GetAllUnitUids())
                            _server.SendToUnit(uid, EventId.EGS_CHAT_NOT, chatNot);
                    }
                }
                _server.Router.Send(PerformerTypeId, ActiveUnit.UnitUid, PerformerId.PI_GS_SERVER, _server.Uid,
                    EventId.DCHATLOG_RECORD_CHAT_NOT, new KDCHATLOG_RECORD_CHAT_NOT
                    {
                        AccountUid = AccountUid,
                        UnitUid = ActiveUnit.UnitUid,
                        ServerGroupId = _server.ServerGroupId,
                        ChatText = filteredMsg,
                        ChatType = req.ChatPacketType,
                    });
                break;
            }

            // -------- Room / Dungeon / PvP --------

            case EventId.EGS_ROOM_LIST_REQ:
            {
                var req = ev.GetPayload<KEGS_ROOM_LIST_REQ>(Region);
                var rooms = _server.Center.RoomManager.GetRoomList(Math.Max(1, req.ViewPage));
                SendPacket(EventId.EGS_ROOM_LIST_ACK, new KEGS_ROOM_LIST_ACK
                {
                    Ok = NetError.NET_OK,
                    TotalPage = 1,
                    ViewPage = Math.Max(1, req.ViewPage),
                    Rooms = rooms,
                });
                break;
            }

            case EventId.EGS_CREATE_ROOM_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_CREATE_ROOM_REQ>(Region);
                LeaveCurrentField();
                var room = _server.Center.RoomManager.CreateRoom(req.RoomInfo, BuildRoomUserInfo(), req.Observer);
                CurrentRoomUid = room.Info.RoomUid;
                Fsm.TryStateTransition(KGSFSM.I_TO_ROOM, out _);

                SendPacket(EventId.EGS_CREATE_ROOM_ACK, new KEGS_CREATE_ROOM_ACK
                {
                    Ok = NetError.NET_OK,
                    RoomInfo = room.Info,
                    Slots = room.SnapshotSlots(),
                    ObserverSlots = room.ObserverSlots,
                });
                break;
            }

            case EventId.EGS_JOIN_ROOM_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_JOIN_ROOM_REQ>(Region);
                LeaveCurrentField();
                var (ok, room) = _server.Center.RoomManager.JoinRoom(
                    req.RoomUid, BuildRoomUserInfo(), req.Password, req.IsObserver);

                if (ok != NetError.NET_OK || room == null)
                {
                    SendPacket(EventId.EGS_JOIN_ROOM_ACK, new KEGS_JOIN_ROOM_ACK { Ok = ok });
                    break;
                }

                CurrentRoomUid = room.Info.RoomUid;
                Fsm.TryStateTransition(KGSFSM.I_TO_ROOM, out _);

                var slots = room.SnapshotSlots();
                var ack = new KEGS_JOIN_ROOM_ACK
                {
                    Ok = NetError.NET_OK,
                    RoomInfo = room.Info,
                    Slot = slots,
                    ObserverSlot = room.ObserverSlots,
                };
                SendPacket(EventId.EGS_JOIN_ROOM_ACK, ack);

                var mySlot = slots.FirstOrDefault(s => s.RoomUserInfo.UnitUid == ActiveUnit.UnitUid)
                    ?? room.ObserverSlots.FirstOrDefault(s => s.RoomUserInfo.UnitUid == ActiveUnit.UnitUid)
                    ?? new KRoomSlotInfo();
                var joinNot = new KEGS_JOIN_ROOM_NOT
                {
                    JoinSlot = mySlot,
                    StartPosIndex = mySlot.Index,
                };
                foreach (long uid in room.GetMemberUnitUids().Where(u => u != ActiveUnit.UnitUid))
                    _server.SendToUnit(uid, EventId.EGS_JOIN_ROOM_NOT, joinNot);
                break;
            }

            case EventId.EGS_LEAVE_ROOM_REQ:
            {
                if (ActiveUnit == null || CurrentRoomUid <= 0) break;
                var room = _server.Center.RoomManager.GetRoom(CurrentRoomUid);
                long ruid = CurrentRoomUid;
                _server.Center.RoomManager.LeaveRoom(ruid, ActiveUnit.UnitUid);
                CurrentRoomUid = 0;
                Fsm.TryStateTransition(KGSFSM.I_TO_FIELD_MAP, out _);

                SendPacket(EventId.EGS_LEAVE_ROOM_ACK, new KEGS_LEAVE_ROOM_ACK { Ok = NetError.NET_OK });
                if (room != null)
                {
                    var not = new KEGS_LEAVE_ROOM_NOT
                    {
                        RoomState = room.Info.RoomState,
                        UnitUid = ActiveUnit.UnitUid,
                        Slot = room.SnapshotSlots(),
                        ObserverSlot = room.ObserverSlots,
                    };
                    foreach (long uid in room.GetMemberUnitUids())
                        _server.SendToUnit(uid, EventId.EGS_LEAVE_ROOM_NOT, not);
                }
                break;
            }

            case EventId.EGS_CHANGE_READY_REQ:
            {
                if (ActiveUnit == null || CurrentRoomUid <= 0) break;
                var req = ev.GetPayload<KEGS_CHANGE_READY_REQ>(Region);
                var room = _server.Center.RoomManager.GetRoom(CurrentRoomUid);
                bool ok = room != null && room.SetReady(ActiveUnit.UnitUid, req.Ready);
                SendPacket(EventId.EGS_CHANGE_READY_ACK, new KEGS_CHANGE_READY_ACK { Ok = ok ? NetError.NET_OK : NetError.ERR_ROOM_16 });
                if (ok && room != null)
                {
                    var not = new KEGS_CHANGE_READY_NOT { UnitUid = ActiveUnit.UnitUid, Ready = req.Ready };
                    foreach (long uid in room.GetMemberUnitUids())
                        _server.SendToUnit(uid, EventId.EGS_CHANGE_READY_NOT, not);
                }
                break;
            }

            case EventId.EGS_CHANGE_TEAM_REQ:
            {
                if (ActiveUnit == null || CurrentRoomUid <= 0) break;
                var req = ev.GetPayload<KEGS_CHANGE_TEAM_REQ>(Region);
                var room = _server.Center.RoomManager.GetRoom(CurrentRoomUid);
                if (room == null) break;
                var (ok, slotIdx) = room.SetTeam(ActiveUnit.UnitUid, req.TeamNum);
                SendPacket(EventId.EGS_CHANGE_TEAM_ACK, new KEGS_CHANGE_TEAM_ACK { Ok = ok ? NetError.NET_OK : NetError.ERR_ROOM_02 });
                if (ok)
                {
                    var not = new KEGS_CHANGE_TEAM_NOT { UnitUid = ActiveUnit.UnitUid, TeamNum = req.TeamNum, SwapSlotIndex = slotIdx };
                    foreach (long uid in room.GetMemberUnitUids())
                        _server.SendToUnit(uid, EventId.EGS_CHANGE_TEAM_NOT, not);
                }
                break;
            }

            case EventId.EGS_STATE_CHANGE_GAME_START_REQ:
            {
                if (ActiveUnit == null || CurrentRoomUid <= 0) break;
                var req = ev.GetPayload<KEGS_STATE_CHANGE_GAME_START_REQ>(Region);
                var room = _server.Center.RoomManager.GetRoom(CurrentRoomUid);
                if (room == null)
                {
                    SendPacket(EventId.EGS_STATE_CHANGE_GAME_START_ACK, new KPacketOK { Ok = NetError.ERR_ROOM_16 });
                    break;
                }

                bool isSpiritFree = PcBangManager.IsPcBang && _server.GameSysValTable.IsSpiritFree(PcBangManager.PcBangType);
                if (room.Info.RoomType == 1 && !SpiritManager.CheckingSpirit(req.DungeonId > 0 ? req.DungeonId : room.Info.DungeonId, isPcBang: isSpiritFree, isHenirDungeon: room.Info.DungeonMode == 1))
                {
                    SendPacket(EventId.EGS_STATE_CHANGE_GAME_START_ACK, new KPacketOK { Ok = NetError.ERR_ROOM_27 });
                    break;
                }

                bool ok = room.StartLoading(req.WorldId, req.DungeonId);
                SendPacket(EventId.EGS_STATE_CHANGE_GAME_START_ACK, new KPacketOK { Ok = ok ? NetError.NET_OK : NetError.ERR_ROOM_10 });
                if (ok)
                {
                    if (room.Info.RoomType == 1 && room.Info.DungeonId > 0)
                    {
                        foreach (long memberUid in room.GetMemberUnitUids())
                        {
                            _server.Db.RecordDungeonPlay(memberUid, room.Info.DungeonId, cleared: false);
                            var mu = _server.GetUserByUnitUid(memberUid);
                            if (mu?.ActiveUnit != null)
                            {
                                if (!mu.ActiveUnit.DungeonPlay.TryGetValue(room.Info.DungeonId, out var dp))
                                {
                                    dp = new KDungeonPlayInfo { DungeonId = room.Info.DungeonId, New = true };
                                    mu.ActiveUnit.DungeonPlay[room.Info.DungeonId] = dp;
                                }
                                dp.PlayTimes++;
                            }
                        }
                    }
                    var not = new KEGS_STATE_CHANGE_GAME_START_NOT
                    {
                        RoomState = room.Info.RoomState,
                        GameType = room.Info.RoomType,
                        Spirit = SpiritManager.Spirit,
                        WorldId = room.Info.WorldId,
                        Slots = room.SnapshotSlots(),
                    };
                    foreach (long uid in room.GetMemberUnitUids())
                        _server.SendToUnit(uid, EventId.EGS_STATE_CHANGE_GAME_START_NOT, not);
                }
                break;
            }

            case EventId.EGS_GAME_LOADING_REQ:
            {
                if (ActiveUnit == null || CurrentRoomUid <= 0) break;
                var req = ev.GetPayload<KEGS_GAME_LOADING_REQ>(Region);
                var room = _server.Center.RoomManager.GetRoom(CurrentRoomUid);
                if (room == null) break;

                room.UpdateLoading(ActiveUnit.UnitUid, req.LoadingProgress, out bool allLoaded);
                SendPacket(EventId.EGS_GAME_LOADING_ACK, new KPacketOK { Ok = NetError.NET_OK });

                var loadNot = new KEGS_GAME_LOADING_NOT { UnitUid = ActiveUnit.UnitUid, LoadingProgress = req.LoadingProgress };
                foreach (long uid in room.GetMemberUnitUids())
                    _server.SendToUnit(uid, EventId.EGS_GAME_LOADING_NOT, loadNot);

                if (allLoaded)
                {
                    var playNot = new KEGS_PLAY_START_NOT
                    {
                        RoomState = room.Info.RoomState,
                        Slots = room.SnapshotSlots(),
                        DungeonId = room.Info.DungeonId,
                    };
                    foreach (long uid in room.GetMemberUnitUids())
                        _server.SendToUnit(uid, EventId.EGS_PLAY_START_NOT, playNot);
                }
                break;
            }

            case EventId.EGS_END_GAME_REQ:
            {
                if (ActiveUnit == null || CurrentRoomUid <= 0) break;
                var req = ev.GetPayload<KEGS_END_GAME_REQ>(Region);
                var room = _server.Center.RoomManager.GetRoom(CurrentRoomUid);
                if (room == null) break;

                var evRates = _server.GameEventManager.GetActiveRates();
                var buffRates = ServerBuffManager.GetBuffBonusRates();
                var pcRates = PcBangManager.GetPcBangReward();
                float rewardMult = AntiAddictionManager.GetRewardMultiplier() *
                                   (evRates.expRate + buffRates.expBonus + pcRates.expBonus);
                bool isPvpRoom = room.Info.RoomType == 0;

                if (isPvpRoom)
                {
                    var pvpResult = room.FinishPvpGame();
                    foreach (var ures in pvpResult.PvpUnitInfo)
                    {
                        int adjExp = (int)(ures.Exp * rewardMult);
                        ures.Exp = adjExp;
                        _server.Db.UpdateUnitPvpResult(
                            ures.UnitUid, ures.Rating, ures.RPoint, ures.APoint,
                            ures.KFactorIncrement, adjExp, ures.Win);
                    }
                    QuestManager.OnPvpPlay();
                    PetManager.DecreaseSatietyOnRoom(room.Info.JoinSlot, haveToy: false);
                    _server.StatisticsManager.IncrementCounter("PvpFinished");
                    ActiveUnit = _server.Db.GetUnitByUid(Region, ActiveUnit.UnitUid);

                    SendPacket(EventId.EGS_END_GAME_ACK, new KPacketOK { Ok = NetError.NET_OK });
                    var endNot = new KEGS_END_GAME_NOT
                    {
                        RoomState = room.Info.RoomState,
                        WinTeam = pvpResult.WinTeam >= 0 ? pvpResult.WinTeam : req.WinTeam,
                        Slots = room.SnapshotSlots(),
                    };
                    foreach (long uid in room.GetMemberUnitUids())
                    {
                        _server.SendToUnit(uid, EventId.EGS_END_GAME_NOT, endNot);
                        _server.SendToUnit(uid, EventId.EGS_END_GAME_PVP_RESULT_DATA_NOT, pvpResult);
                    }
                }
                else
                {
                    var dgnResult = room.FinishDungeonGame(isWin: true);
                    bool isHenirDungeon = KUserDungeonManager.IsHenirDungeon(room.Info.DungeonId) || room.Info.DungeonMode is 1 or 2;
                    bool isSpiritFree = PcBangManager.IsPcBang && _server.GameSysValTable.IsSpiritFree(PcBangManager.PcBangType);
                    int nextSpirit = (pcRates.unlimitedSpirit || isSpiritFree)
                        ? ActiveUnit.Spirit
                        : SpiritManager.ConsumeSpirit(5, room.Info.DungeonId, isSpiritFree, isHenirDungeon: isHenirDungeon);
                    _server.Db.UpdateUnitSpirit(ActiveUnit.UnitUid, nextSpirit);

                    bool henirHackDetected = false;
                    var henirRankCandidates = new List<(KGSUser memberUser, KHenirRankingInfo rankInfo, bool isHeroMode)>();
                    foreach (var dres in dgnResult.DungeonUnitInfo)
                    {
                        int totalExp = (int)(dres.SumExp() * rewardMult);
                        int ed = (int)(dres.Ed * (evRates.edRate + buffRates.edBonus + pcRates.edBonus));
                        dres.Exp = totalExp;
                        dres.Ed = ed;
                        _server.Db.UpdateUnitProgress(dres.UnitUid, addExp: totalExp, addEd: ed, mapId: 0, win: dgnResult.IsWin);
                        if (dgnResult.IsWin)
                        {
                            _server.Db.RecordDungeonClear(dres.UnitUid, room.Info.DungeonId, dres.TotalScore, dres.TotalRank);
                            _server.Db.RecordDungeonPlay(dres.UnitUid, room.Info.DungeonId, cleared: true);
                        }

                        var memberUser = _server.GetUserByUnitUid(dres.UnitUid);
                        if (memberUser != null && dgnResult.IsWin)
                        {
                            memberUser._defenceQuickSlotItems.Clear();
                            memberUser.DungeonManager.RecordDungeonClear(room.Info.DungeonId);
                            if (memberUser.ActiveUnit != null)
                            {
                                memberUser.ActiveUnit.DungeonClear[room.Info.DungeonId] = new KDungeonClearInfo
                                {
                                    DungeonId = room.Info.DungeonId,
                                    MaxScore = dres.TotalScore,
                                    MaxTotalRank = dres.TotalRank,
                                    ClearTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                                };
                            }
                            bool suitable = dgnResult.SuitableLevelInfo.TryGetValue(dres.UnitUid, out bool sl) && sl;
                            memberUser.QuestManager.OnDungeonClear(
                                room.Info.DungeonId,
                                room.Info.DifficultyLevel,
                                dres.TotalRank,
                                dgnResult.Sec,
                                dres.Damaged,
                                suitable);

                            // SubTitleOpenMission.lua progression (TMCT_DUNGEON_CLEAR_COUNT == 8; skip Ruben Gate 30040 & tutorial)
                            if (room.Info.DungeonId != 30040 && room.Info.DungeonId != 30000)
                            {
                                foreach (var (missionId, mission) in _server.SubTitleOpenMissions)
                                {
                                    if (mission.ClearType == 8 &&
                                        mission.DungeonId == room.Info.DungeonId &&
                                        (mission.Difficulty == -1 || mission.Difficulty == room.Info.DifficultyLevel))
                                    {
                                        memberUser.TitleManager.RecordMissionProgress(_server.Db, dres.UnitUid, missionId, 1, Math.Max(1, mission.ClearCount));
                                    }
                                }
                            }

                            if (memberUser.ActiveUnit != null)
                            {
                                var paymentItems = _server.Center.DropTable.RollDungeonClearPaymentItems(
                                    room.Info.DungeonId,
                                    memberUser.ActiveUnit.Level,
                                    memberUser.QuestManager.GetKnownQuestIds());
                                foreach (var (payItemId, payQty) in paymentItems)
                                {
                                    _server.Db.GrantUnitItem(dres.UnitUid, payItemId, payQty);
                                    memberUser.ItemQuantityVariation.IncreaseQuantity(payItemId, payQty);
                                }

                                if (isHenirDungeon)
                                {
                                    int henirStage = Math.Max(1, room.CurrentStageId);
                                    sbyte henirRewardMode = (sbyte)(room.Info.DungeonMode == 2 ? 1 : 0);
                                    foreach (var (hItemId, hQty) in _server.Center.HenirResultTable.RollStageRewards(henirStage, henirRewardMode))
                                    {
                                        _server.Db.GrantUnitItem(dres.UnitUid, hItemId, hQty);
                                        memberUser.ItemQuantityVariation.IncreaseQuantity(hItemId, hQty);
                                    }
                                    foreach (var (cItemId, cQty) in _server.Center.HenirResultTable.GetHenirChallangeRewards(room.Info.DungeonMode, henirStage))
                                    {
                                        _server.Db.GrantUnitItem(dres.UnitUid, cItemId, cQty);
                                        memberUser.ItemQuantityVariation.IncreaseQuantity(cItemId, cQty);
                                    }
                                    if (_server.Center.HenirResultTable.EnableNotify &&
                                        _server.Center.HenirResultTable.IsClearNotifyStage(henirStage))
                                    {
                                        _server.BroadcastNotice(
                                            KEGS_NOTIFY_MSG_NOT.NT_HENIR_CLEAR,
                                            1,
                                            memberUser.ActiveUnit.NickName);
                                    }

                                    if (_server.Center.HackingCheckManager.CheckHenirClearTimeHackUser(
                                        (sbyte)memberUser.AuthLevel,
                                        memberUser.AccountUid,
                                        room.Info.DungeonId,
                                        henirStage,
                                        dgnResult.Sec))
                                    {
                                        henirHackDetected = true;
                                        memberUser.HackingUserSendType = KEGS_SERVER_CHECK_HACK_USER_REQ.HctDungeonClearTime;
                                        memberUser.HackingManager.SetHackingUserSendType(memberUser.HackingUserSendType);
                                        memberUser.SendPacket(EventId.EGS_SERVER_CHECK_HACK_USER_REQ, new KEGS_SERVER_CHECK_HACK_USER_REQ
                                        {
                                            HackingCheckType = KEGS_SERVER_CHECK_HACK_USER_REQ.HctDungeonClearTime,
                                        });
                                    }
                                    else
                                    {
                                        henirRankCandidates.Add((memberUser, new KHenirRankingInfo
                                        {
                                            StageCount = henirStage,
                                            PlayTime = (uint)Math.Max(0, dgnResult.Sec),
                                            RegDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                                            UnitUid = memberUser.ActiveUnit.UnitUid,
                                            NickName = memberUser.ActiveUnit.NickName,
                                            UnitClass = (sbyte)memberUser.ActiveUnit.UnitClass,
                                            Level = (byte)Math.Clamp((int)memberUser.ActiveUnit.Level, 0, 255),
                                        }, room.Info.DungeonMode == 2));
                                    }
                                }

                                if (_server.Center.HackingCheckManager.CheckDungeonResultAutoHackUser(
                                        room.Info.DungeonId,
                                        room.Info.DifficultyLevel,
                                        (byte)memberUser.ActiveUnit.UnitClass,
                                        memberUser.ActiveUnit.Level,
                                        dgnResult.IsWin,
                                        Math.Max(1, (int)room.Info.JoinSlot),
                                        (KCnHackingCheckManager.DungeonRankType)dres.ComboRank,
                                        (KCnHackingCheckManager.DungeonRankType)dres.TechnicalRank,
                                        (KCnHackingCheckManager.DungeonRankType)dres.TimeRank,
                                        (KCnHackingCheckManager.DungeonRankType)dres.DamagedRank,
                                        dres.TechnicalScore,
                                        0,
                                        memberUser.PetManager.SummonedPetUid > 0))
                                {
                                    memberUser.HackingUserSendType = KEGS_SERVER_CHECK_HACK_USER_REQ.HctDungeonClearResult;
                                    memberUser.HackingManager.SetHackingUserSendType(memberUser.HackingUserSendType);
                                    memberUser.SendPacket(EventId.EGS_SERVER_CHECK_HACK_USER_REQ, new KEGS_SERVER_CHECK_HACK_USER_REQ
                                    {
                                        HackingCheckType = KEGS_SERVER_CHECK_HACK_USER_REQ.HctDungeonClearResult,
                                    });
                                }

                                if (_server.Center.HackingCheckManager.CheckDungeonClearTimeAutoHackUser(
                                        (sbyte)memberUser.AuthLevel,
                                        memberUser.AccountUid,
                                        room.Info.DungeonId,
                                        room.Info.DifficultyLevel,
                                        (byte)memberUser.ActiveUnit.UnitClass,
                                        memberUser.ActiveUnit.Level,
                                        dgnResult.IsWin,
                                        Math.Max(1, (int)room.Info.JoinSlot),
                                        dgnResult.Sec))
                                {
                                    memberUser.HackingUserSendType = KEGS_SERVER_CHECK_HACK_USER_REQ.HctDungeonClearTime;
                                    memberUser.HackingManager.SetHackingUserSendType(memberUser.HackingUserSendType);
                                    memberUser.SendPacket(EventId.EGS_SERVER_CHECK_HACK_USER_REQ, new KEGS_SERVER_CHECK_HACK_USER_REQ
                                    {
                                        HackingCheckType = KEGS_SERVER_CHECK_HACK_USER_REQ.HctDungeonClearTime,
                                    });
                                }
                            }
                        }
                    }
                    if (isHenirDungeon && !henirHackDetected)
                    {
                        var recordedUtc = DateTime.UtcNow;
                        foreach (var (memberUser, rankInfo, isHeroMode) in henirRankCandidates)
                        {
                            memberUser.RankingManager.SetRankingInfo(rankInfo, forceHero: isHeroMode);
                            int rankingType = isHeroMode ? 1 : 0;
                            if (_server.RankingManager.CheckNewRecord(rankInfo))
                            {
                                _server.Db.UpsertHenirRanking(
                                    rankingType,
                                    rankInfo.UnitUid,
                                    rankInfo.NickName,
                                    (byte)rankInfo.UnitClass,
                                    rankInfo.Level,
                                    rankInfo.StageCount,
                                    (int)rankInfo.PlayTime,
                                    recordedUtc);
                                _server.Router.Send(PerformerTypeId, rankInfo.UnitUid, PerformerId.PI_LOGIN_SERVER, _server.LoginServerUid,
                                    EventId.ELG_NEW_RECORD_HENIR_RANKING_NOT, new KELG_NEW_RECORD_HENIR_RANKING_NOT
                                    {
                                        RankingInfo = rankInfo,
                                    });
                            }
                        }
                    }
                    ServerBuffManager.OnEndDungeonGame();
                    PetManager.DecreaseSatietyOnRoom(room.Info.JoinSlot, haveToy: false);
                    _server.StatisticsManager.IncrementCounter("DungeonFinished");
                    ActiveUnit = _server.Db.GetUnitByUid(Region, ActiveUnit.UnitUid);
                    if (ActiveUnit != null)
                    {
                        EdManager.SetEd(ActiveUnit.Ed, KUserEDManager.EdReason.ErInEdDungeonClearBonus);
                        ActiveUnit.BuffInfo = ServerBuffManager.ToPacketBuffInfos();
                    }

                    bool hasComeBackInParty = ComeBackUserManager.IsComeBackUser;
                    if (CurrentPartyUid > 0)
                    {
                        var party = _server.Center.PartyManager.GetParty(CurrentPartyUid);
                        if (party != null)
                        {
                            hasComeBackInParty = party.Members.Any(m =>
                                _server.GetUserByUnitUid(m.UnitUid)?.ComeBackUserManager.IsComeBackUser == true);
                            int feverGain = party.PartyFeverManager.CalcIncreaseFeverPoint(
                                party.Members.Count, isBattleField: false, subStageNpcCount: 3, hasComeBackUserInParty: hasComeBackInParty);
                            party.AddFever(Math.Max(1500, feverGain * 10));
                            party.ReturnToWait();
                        }
                    }
                    dgnResult.ComeBackUserInParty = hasComeBackInParty;

                    SendPacket(EventId.EGS_END_GAME_ACK, new KPacketOK { Ok = NetError.NET_OK });
                    var endNot = new KEGS_END_GAME_NOT
                    {
                        RoomState = room.Info.RoomState,
                        WinTeam = req.WinTeam,
                        Slots = room.SnapshotSlots(),
                    };
                    foreach (long uid in room.GetMemberUnitUids())
                    {
                        _server.SendToUnit(uid, EventId.EGS_END_GAME_NOT, endNot);
                        _server.SendToUnit(uid, EventId.EGS_END_GAME_DUNGEON_RESULT_DATA_NOT, dgnResult);
                    }
                }
                break;
            }

            // -------- Party --------

            case EventId.EGS_INVITE_PARTY_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_INVITE_PARTY_REQ>(Region);
                var party = CurrentPartyUid > 0
                    ? _server.Center.PartyManager.GetParty(CurrentPartyUid)
                    : _server.Center.PartyManager.CreateParty(req.CreatePartyInfo, BuildPartyUserInfo());
                if (party == null) break;

                CurrentPartyUid = party.Info.PartyUid;
                long targetUid = req.ReceiverUid > 0
                    ? req.ReceiverUid
                    : (_server.Db.FindUnitUidByNickName(Region, req.NickName) ?? 0);

                SendPacket(EventId.EGS_INVITE_PARTY_ACK, new KEGS_INVITE_PARTY_ACK
                {
                    Ok = targetUid > 0 ? NetError.NET_OK : NetError.ERR_PARTY_07,
                    ReceiverUid = targetUid,
                    UdpRelayPort = _server.UdpRelayPort,
                });

                if (targetUid > 0)
                {
                    _server.SendToUnit(targetUid, EventId.EGS_INVITE_PARTY_NOT, new KEGS_INVITE_PARTY_NOT
                    {
                        PartyUid = party.Info.PartyUid,
                        HostUnitUid = ActiveUnit.UnitUid,
                        HostNickName = ActiveUnit.NickName,
                        PartyType = party.Info.PartyType,
                        UdpRelayPort = _server.UdpRelayPort,
                    });
                }
                break;
            }

            case EventId.EGS_INVITE_PARTY_REPLY_NOT:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_INVITE_PARTY_REPLY_NOT>(Region);
                if (req.ReplyReason == 0 &&
                    _server.Center.PartyManager.TryJoinParty(req.PartyUid, BuildPartyUserInfo(), out var party) &&
                    party != null)
                {
                    CurrentPartyUid = party.Info.PartyUid;
                    var joinNot = new KEGS_JOIN_PARTY_NOT
                    {
                        Ok = NetError.NET_OK,
                        PartyInfos = [party.Info],
                        PartyUsers = party.SnapshotMembers(),
                        CurPartyUnitUidList = party.GetUnitUids(),
                        UdpRelayPort = _server.UdpRelayPort,
                    };
                    foreach (long uid in party.GetUnitUids())
                        _server.SendToUnit(uid, EventId.EGS_JOIN_PARTY_NOT, joinNot);
                }
                break;
            }

            case EventId.EGS_LEAVE_PARTY_REQ:
            {
                if (ActiveUnit == null || CurrentPartyUid <= 0) break;
                var req = ev.GetPayload<KEGS_LEAVE_PARTY_REQ>(Region);
                long puid = CurrentPartyUid;
                var party = _server.Center.PartyManager.GetParty(puid);
                var (_, newHostUid) = _server.Center.PartyManager.LeaveParty(puid, ActiveUnit.UnitUid);
                CurrentPartyUid = 0;

                SendPacket(EventId.EGS_LEAVE_PARTY_ACK, new KPacketOK { Ok = NetError.NET_OK });
                if (party != null)
                {
                    var leaveNot = new KEGS_LEAVE_PARTY_NOT
                    {
                        LeaveUnitUid = ActiveUnit.UnitUid,
                        NewHostUid = newHostUid,
                        PartyUid = puid,
                        PartyUnitUidList = party.GetUnitUids(),
                        Reason = req.Reason,
                    };
                    foreach (long uid in party.GetUnitUids())
                        _server.SendToUnit(uid, EventId.EGS_LEAVE_PARTY_NOT, leaveNot);
                }
                break;
            }

            case EventId.EGS_PARTY_CHANGE_DUNGEON_REQ:
            {
                if (ActiveUnit == null || CurrentPartyUid <= 0) break;
                var req = ev.GetPayload<KEGS_PARTY_CHANGE_DUNGEON_REQ>(Region);
                var party = _server.Center.PartyManager.GetParty(CurrentPartyUid);
                bool ok = party != null && party.ChangeDungeon(ActiveUnit.UnitUid, req.DungeonId, req.DifficultyLevel, req.DungeonMode);
                SendPacket(EventId.EGS_PARTY_CHANGE_DUNGEON_ACK, new KPacketOK { Ok = ok ? NetError.NET_OK : NetError.ERR_PARTY_09 });
                if (ok && party != null)
                {
                    foreach (long uid in party.GetUnitUids())
                        _server.SendToUnit(uid, EventId.EGS_PARTY_CHANGE_DUNGEON_NOT, req);
                }
                break;
            }

            case EventId.EGS_PARTY_CHANGE_READY_REQ:
            {
                if (ActiveUnit == null || CurrentPartyUid <= 0) break;
                var req = ev.GetPayload<KEGS_PARTY_CHANGE_READY_REQ>(Region);
                var party = _server.Center.PartyManager.GetParty(CurrentPartyUid);
                bool ok = party != null && party.SetReady(ActiveUnit.UnitUid, req.Ready);
                SendPacket(EventId.EGS_PARTY_CHANGE_READY_ACK, new KPacketOK { Ok = ok ? NetError.NET_OK : NetError.ERR_PARTY_00 });
                if (ok && party != null)
                {
                    var not = new KEGS_PARTY_CHANGE_READY_NOT { UnitUid = ActiveUnit.UnitUid, Ready = req.Ready };
                    foreach (long uid in party.GetUnitUids())
                        _server.SendToUnit(uid, EventId.EGS_PARTY_CHANGE_READY_NOT, not);
                }
                break;
            }

            case EventId.EGS_PARTY_GAME_START_REQ:
            {
                if (ActiveUnit == null || CurrentPartyUid <= 0) break;
                var req = ev.GetPayload<KEGS_PARTY_GAME_START_REQ>(Region);
                var party = _server.Center.PartyManager.GetParty(CurrentPartyUid);
                bool ok = party != null && party.CanStartGame(ActiveUnit.UnitUid);
                SendPacket(EventId.EGS_PARTY_GAME_START_ACK, new KPacketOK { Ok = ok ? NetError.NET_OK : NetError.ERR_PARTY_13 });
                if (ok && party != null)
                {
                    var template = new KRoomInfo
                    {
                        RoomType = 1,
                        RoomName = party.Info.PartyName,
                        DungeonId = party.Info.DungeonId,
                        DifficultyLevel = party.Info.DifficultyLevel,
                        DungeonMode = party.Info.DungeonMode,
                        WorldId = req.WorldId,
                        PartyUid = party.Info.PartyUid,
                        MaxSlot = 4,
                    };
                    var room = _server.Center.RoomManager.CreateRoom(template, BuildRoomUserInfo(), false);
                    foreach (var m in party.SnapshotMembers().Where(x => x.UnitUid != ActiveUnit.UnitUid))
                    {
                        room.TryAddUser(new KRoomUserInfo
                        {
                            UnitUid = m.UnitUid,
                            NickName = m.NickName,
                            UnitClass = m.UnitClass,
                            Level = m.Level,
                            PartyUid = party.Info.PartyUid,
                        }, false, "");
                    }
                    room.StartLoading(req.WorldId, party.Info.DungeonId);
                    foreach (long uid in party.GetUnitUids())
                    {
                        _server.SendToUnit(uid, EventId.EGS_JOIN_ROOM_NOT, new KEGS_CREATE_ROOM_ACK
                        {
                            Ok = NetError.NET_OK,
                            RoomInfo = room.Info,
                            Slots = room.SnapshotSlots(),
                        });
                    }
                }
                break;
            }

            // -------- Inventory / Shop / Mail / Guild / Quest --------

            case EventId.EGS_CHANGE_INVENTORY_SLOT_ITEM_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_CHANGE_INVENTORY_SLOT_ITEM_REQ>(Region);
                var updated = _server.Db.MoveInventorySlot(
                    ActiveUnit.UnitUid, req.FromSlotType, req.FromSlotId, req.ToSlotType, req.ToSlotId);
                SendPacket(EventId.EGS_CHANGE_INVENTORY_SLOT_ITEM_ACK,
                    new KEGS_CHANGE_INVENTORY_SLOT_ITEM_ACK { InventorySlotInfo = updated });
                break;
            }

            case EventId.EGS_BUY_ED_ITEM_REQ:
            {
                if (ActiveUnit == null) break;
                if (IsTradeOrShopOpen())
                {
                    SendPacket(EventId.EGS_BUY_ED_ITEM_ACK, new KEGS_BUY_ED_ITEM_ACK { Ok = NetError.ERR_ITEM_14, Ed = ActiveUnit.Ed, APoint = ActiveUnit.APoint });
                    break;
                }
                var req = ev.GetPayload<KEGS_BUY_ED_ITEM_REQ>(Region);
                var (ok, newEd, added) = _server.Db.BuyEdItems(ActiveUnit.UnitUid, req.Items);
                if (ok)
                {
                    ActiveUnit.Ed = newEd;
                    EdManager.SetEd(newEd, KUserEDManager.EdReason.ErOutEdBuyItemNpcShop);
                }
                SendPacket(EventId.EGS_BUY_ED_ITEM_ACK, new KEGS_BUY_ED_ITEM_ACK
                {
                    Ok = ok ? NetError.NET_OK : NetError.ERR_BUY_ED_ITEM_05,
                    Ed = newEd,
                    APoint = ActiveUnit.APoint,
                    InventorySlotInfo = added,
                });
                break;
            }

            case EventId.EGS_SELL_ED_ITEM_REQ:
            {
                if (ActiveUnit == null) break;
                if (IsTradeOrShopOpen())
                {
                    SendPacket(EventId.EGS_SELL_ED_ITEM_ACK, new KEGS_SELL_ED_ITEM_ACK { Ok = NetError.ERR_ITEM_14, Ed = ActiveUnit.Ed });
                    break;
                }
                var req = ev.GetPayload<KEGS_SELL_ED_ITEM_REQ>(Region);
                if (req.Quantity < 0)
                {
                    SendPacket(EventId.EGS_SELL_ED_ITEM_ACK, new KEGS_SELL_ED_ITEM_ACK { Ok = NetError.ERR_SELL_ED_ITEM_05, Ed = ActiveUnit.Ed });
                    break;
                }
                if (InventoryLocker.IsLocked())
                {
                    SendPacket(EventId.EGS_SELL_ED_ITEM_ACK, new KEGS_SELL_ED_ITEM_ACK { Ok = NetError.ERR_INVENTORY_LOCK_00, Ed = ActiveUnit.Ed });
                    break;
                }
                var sellSlot = _server.Db.GetInventory(ActiveUnit.UnitUid).FirstOrDefault(i => i.ItemUid == req.ItemUid);
                if (sellSlot != null && _server.ImpossibleSellItemIds.Contains(sellSlot.ItemInfo.ItemId))
                {
                    SendPacket(EventId.EGS_SELL_ED_ITEM_ACK, new KEGS_SELL_ED_ITEM_ACK { Ok = NetError.ERR_SELL_ED_ITEM_05, Ed = ActiveUnit.Ed });
                    HackingUserSendType = KEGS_SERVER_CHECK_HACK_USER_REQ.HctImpossibleSellItem;
                    HackingManager.SetHackingUserSendType(HackingUserSendType);
                    SendPacket(EventId.EGS_SERVER_CHECK_HACK_USER_REQ, new KEGS_SERVER_CHECK_HACK_USER_REQ
                    {
                        HackingCheckType = KEGS_SERVER_CHECK_HACK_USER_REQ.HctImpossibleSellItem,
                    });
                    SecurityManager.SetKickRandomTime(15, 5);
                    SecurityManager.SetKickState(KUserSecurityManager.KickType.KtAfterRandomTime);
                    break;
                }
                var (ok, newEd, sold) = _server.Db.SellEdItem(ActiveUnit.UnitUid, req.ItemUid, req.Quantity);
                if (ok)
                {
                    ActiveUnit.Ed = newEd;
                    EdManager.SetEd(newEd, KUserEDManager.EdReason.ErInEdSellItemNpcShop);
                }
                SendPacket(EventId.EGS_SELL_ED_ITEM_ACK, new KEGS_SELL_ED_ITEM_ACK
                {
                    Ok = ok ? NetError.NET_OK : NetError.ERR_SELL_ED_ITEM_05,
                    Ed = newEd,
                    InventorySlotInfo = sold,
                });
                break;
            }

            case EventId.EGS_ENCHANT_ITEM_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_ENCHANT_ITEM_REQ>(Region);
                if (!InventoryLocker.Lock(KInventoryLocker.LockType.TimeOut, 30.0))
                {
                    SendPacket(EventId.EGS_ENCHANT_ITEM_ACK, new KEGS_ENCHANT_ITEM_ACK { Ok = NetError.ERR_INVENTORY_LOCK_00, Ed = ActiveUnit.Ed });
                    break;
                }
                try
                {
                    if (IsTradeOrShopOpen())
                    {
                        SendPacket(EventId.EGS_ENCHANT_ITEM_ACK, new KEGS_ENCHANT_ITEM_ACK { Ok = NetError.ERR_ITEM_14, Ed = ActiveUnit.Ed });
                        break;
                    }
                    var (found, itemSlot, isBroken, isSealed) = _server.Db.GetItemFullDetails(ActiveUnit.UnitUid, req.ItemUid);
                    if (!found || itemSlot.SlotCategory == 9)
                    {
                        SendPacket(EventId.EGS_ENCHANT_ITEM_ACK, new KEGS_ENCHANT_ITEM_ACK { Ok = NetError.ERR_ENCHANT_ITEM_00, Ed = ActiveUnit.Ed });
                        break;
                    }
                    if (isSealed)
                    {
                        SendPacket(EventId.EGS_ENCHANT_ITEM_ACK, new KEGS_ENCHANT_ITEM_ACK { Ok = NetError.ERR_ITEM_00, Ed = ActiveUnit.Ed });
                        break;
                    }
                    if (isBroken)
                    {
                        SendPacket(EventId.EGS_ENCHANT_ITEM_ACK, new KEGS_ENCHANT_ITEM_ACK { Ok = NetError.ERR_ENCHANT_ITEM_02, Ed = ActiveUnit.Ed });
                        break;
                    }
                    if (!req.Debug && itemSlot.ItemInfo.EnchantLevel >= _server.EnchantItemManager.EnchantLimit)
                    {
                        SendPacket(EventId.EGS_ENCHANT_ITEM_ACK, new KEGS_ENCHANT_ITEM_ACK { Ok = NetError.ERR_ENCHANT_ITEM_05, Ed = ActiveUnit.Ed });
                        break;
                    }

                    int enchantCostEd = Math.Max(1000, (itemSlot.ItemInfo.EnchantLevel + 1) * 1000);
                    if (ActiveUnit.Ed < enchantCostEd)
                    {
                        SendPacket(EventId.EGS_ENCHANT_ITEM_ACK, new KEGS_ENCHANT_ITEM_ACK { Ok = NetError.ERR_ENCHANT_ITEM_01, Ed = ActiveUnit.Ed });
                        break;
                    }

                    var changedSlots = new List<KInventoryItemInfo>();
                    if (!req.Debug)
                    {
                        int itemType = (itemSlot.ItemInfo.ItemId % 10 == 0 || itemSlot.SlotId == 0) ? 1 : 2;
                        var stoneCandidates = _server.EnchantItemManager.GetMatchingStoneIds(itemType, ActiveUnit.Level, req.IsRareEnchantStone);
                        var (stoneOk, _, stoneSlots) = _server.Db.TryConsumeAnyItemId(ActiveUnit.UnitUid, stoneCandidates, 1);
                        if (!stoneOk)
                        {
                            SendPacket(EventId.EGS_ENCHANT_ITEM_ACK, new KEGS_ENCHANT_ITEM_ACK { Ok = NetError.ERR_ENCHANT_ITEM_01, Ed = ActiveUnit.Ed });
                            break;
                        }
                        changedSlots.AddRange(stoneSlots);

                        if (req.IsSupportMaterial)
                        {
                            var supIds = _server.EnchantItemManager.GetMatchingSupportMaterialIds(ActiveUnit.Level);
                            if (supIds.Count == 0) supIds.Add(109971);
                            var (supOk, _, supSlots) = _server.Db.TryConsumeAnyItemId(ActiveUnit.UnitUid, supIds, 1);
                            if (!supOk)
                            {
                                SendPacket(EventId.EGS_ENCHANT_ITEM_ACK, new KEGS_ENCHANT_ITEM_ACK { Ok = NetError.ERR_ENCHANT_ITEM_01, Ed = ActiveUnit.Ed });
                                break;
                            }
                            changedSlots.AddRange(supSlots);
                        }
                        if (req.IsEnchantPlus)
                        {
                            var plusIds = _server.EnchantItemManager.GetMatchingEnchantPlusIds(ActiveUnit.Level);
                            if (plusIds.Count == 0) plusIds.Add(75000300);
                            var (plusOk, _, plusSlots) = _server.Db.TryConsumeAnyItemId(ActiveUnit.UnitUid, plusIds, 1);
                            if (!plusOk)
                            {
                                SendPacket(EventId.EGS_ENCHANT_ITEM_ACK, new KEGS_ENCHANT_ITEM_ACK { Ok = NetError.ERR_ENCHANT_ITEM_01, Ed = ActiveUnit.Ed });
                                break;
                            }
                            changedSlots.AddRange(plusSlots);
                        }
                        if (req.IsDestroyGuard)
                        {
                            var guardIds = _server.EnchantItemManager.GetMatchingDestroyGuardIds(ActiveUnit.Level);
                            if (guardIds.Count == 0) guardIds.Add(75000310);
                            var (guardOk, _, guardSlots) = _server.Db.TryConsumeAnyItemId(ActiveUnit.UnitUid, guardIds, 1);
                            if (!guardOk)
                            {
                                SendPacket(EventId.EGS_ENCHANT_ITEM_ACK, new KEGS_ENCHANT_ITEM_ACK { Ok = NetError.ERR_ENCHANT_ITEM_01, Ed = ActiveUnit.Ed });
                                break;
                            }
                            changedSlots.AddRange(guardSlots);
                        }
                    }

                    var (eres, nextLvl, broken) = _server.EnchantItemManager.ExecuteInstanceEnchant(
                        (byte)Math.Max((sbyte)0, itemSlot.ItemInfo.EnchantLevel), req.IsSupportMaterial, req.IsEnchantPlus, req.IsDestroyGuard, req.IsRareEnchantStone);
                    EdManager.TryAddEd(-enchantCostEd, KUserEDManager.EdReason.ErOutEdItemEnchant, out int afterEd);
                    _server.Db.UpdateUnitProgress(ActiveUnit.UnitUid, 0, -enchantCostEd, 0);
                    ActiveUnit.Ed = afterEd;
                    var updatedItem = _server.Db.UpdateItemEnchant(ActiveUnit.UnitUid, req.ItemUid, nextLvl, broken);
                    changedSlots.Add(updatedItem);

                    SendPacket(EventId.EGS_ENCHANT_ITEM_ACK, new KEGS_ENCHANT_ITEM_ACK
                    {
                        Ok = NetError.NET_OK,
                        Ed = ActiveUnit.Ed,
                        EnchantResult = (int)eres,
                        EnchantedItemUid = req.ItemUid,
                        EnchantedItemId = itemSlot.ItemInfo.ItemId,
                        InventorySlotInfo = changedSlots,
                    });
                }
                finally
                {
                    InventoryLocker.Unlock();
                }
                break;
            }

            case EventId.EGS_RESTORE_ITEM_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_RESTORE_ITEM_REQ>(Region);
                if (!InventoryLocker.Lock(KInventoryLocker.LockType.TimeOut, 30.0))
                {
                    SendPacket(EventId.EGS_RESTORE_ITEM_ACK, new KEGS_RESTORE_ITEM_ACK { Ok = NetError.ERR_INVENTORY_LOCK_00 });
                    break;
                }
                try
                {
                    if (IsTradeOrShopOpen())
                    {
                        SendPacket(EventId.EGS_RESTORE_ITEM_ACK, new KEGS_RESTORE_ITEM_ACK { Ok = NetError.ERR_ITEM_14 });
                        break;
                    }
                    var (found, itemSlot, isBroken, _) = _server.Db.GetItemFullDetails(ActiveUnit.UnitUid, req.ItemUid);
                    if (!found || !isBroken)
                    {
                        SendPacket(EventId.EGS_RESTORE_ITEM_ACK, new KEGS_RESTORE_ITEM_ACK { Ok = NetError.ERR_ENCHANT_ITEM_00 });
                        break;
                    }
                    var restoreIds = _server.EnchantItemManager.GetMatchingRestoreItemIds(ActiveUnit.Level);
                    if (restoreIds.Count == 0) restoreIds.Add(206880);
                    var (rOk, _, rSlots) = _server.Db.TryConsumeAnyItemId(ActiveUnit.UnitUid, restoreIds, 1);
                    if (!rOk)
                    {
                        SendPacket(EventId.EGS_RESTORE_ITEM_ACK, new KEGS_RESTORE_ITEM_ACK { Ok = NetError.ERR_ENCHANT_ITEM_01 });
                        break;
                    }
                    var restored = _server.Db.UpdateItemEnchant(ActiveUnit.UnitUid, req.ItemUid, (byte)Math.Max((sbyte)0, itemSlot.ItemInfo.EnchantLevel), isBroken: false);
                    rSlots.Add(restored);
                    SendPacket(EventId.EGS_RESTORE_ITEM_ACK, new KEGS_RESTORE_ITEM_ACK
                    {
                        Ok = NetError.NET_OK,
                        InventorySlotInfo = rSlots,
                    });
                }
                finally
                {
                    InventoryLocker.Unlock();
                }
                break;
            }

            case EventId.EGS_EQUIP_TITLE_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_EQUIP_TITLE_REQ>(Region);
                bool ok = TitleManager.EquipTitle(_server.Db, ActiveUnit.UnitUid, req.TitleId);
                if (ok) ActiveUnit.TitleId = req.TitleId;
                SendPacket(EventId.EGS_EQUIP_TITLE_ACK, new KEGS_EQUIP_TITLE_ACK
                {
                    Ok = ok ? NetError.NET_OK : NetError.ERR_TITLE_00,
                    TitleId = ok ? req.TitleId : ActiveUnit.TitleId,
                });
                break;
            }

            case EventId.EGS_CHANGE_SKILL_SLOT_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_CHANGE_SKILL_SLOT_REQ>(Region);
                var (ok, slot2, skill2) = SkillTree.EquipSkillSlot(_server.Db, ActiveUnit.UnitUid, req.SlotId, req.SkillId);
                SendPacket(EventId.EGS_CHANGE_SKILL_SLOT_ACK, new KEGS_CHANGE_SKILL_SLOT_ACK
                {
                    Ok = ok ? NetError.NET_OK : NetError.ERR_SKILL_03,
                    SlotId = req.SlotId,
                    SkillId = req.SkillId,
                    SlotId2 = slot2,
                    SkillId2 = skill2,
                });
                break;
            }

            case EventId.EGS_BUY_PERSONAL_SHOP_ITEM_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_BUY_PERSONAL_SHOP_ITEM_REQ>(Region);
                var (ok, newEd, granted) = PersonalShopManager.BuyItem(
                    _server.Db, Region, ActiveUnit.UnitUid, req.ItemUid, req.Quantity);
                if (ok) ActiveUnit.Ed = newEd;
                SendPacket(EventId.EGS_BUY_PERSONAL_SHOP_ITEM_ACK, new KPacketOK
                {
                    Ok = ok ? NetError.NET_OK : NetError.ERR_PERSONAL_SHOP_04,
                });
                if (ok)
                {
                    SendPacket(EventId.EGS_BUY_PERSONAL_SHOP_ITEM_COMPLETE_NOT, new KEGS_BUY_PERSONAL_SHOP_ITEM_COMPLETE_NOT
                    {
                        InventorySlotInfo = granted != null ? [granted] : [],
                        Ed = newEd,
                    });
                }
                break;
            }

            case EventId.EGS_GET_POST_LETTER_LIST_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_GET_POST_LETTER_LIST_REQ>(Region);
                int page = Math.Max(1, req.PostPageNo);
                bool ok = LetterBox.GetLetterList(req.LetterListType, page, out var titles);
                SendPacket(EventId.EGS_GET_POST_LETTER_LIST_ACK, new KEGS_GET_POST_LETTER_LIST_ACK
                {
                    Ok = ok ? NetError.NET_OK : NetError.ERR_POST_LETTER_00,
                    TotalLetterCount = LetterBox.GetTotalLetterCount(req.LetterListType),
                    PostItems = titles,
                });
                break;
            }

            case EventId.EGS_RENEWAL_LETTER_REQ:
            {
                if (ActiveUnit == null) break;
                var (newUser, newSys, newRel, newTitles) = LetterBox.GetRenewalLetter();
                int total = LetterBox.GetTotalLetterCount((sbyte)KUserLetterBox.LetterListType.LltNormal)
                          + LetterBox.GetTotalLetterCount((sbyte)KUserLetterBox.LetterListType.LltSystem)
                          + LetterBox.GetTotalLetterCount((sbyte)KUserLetterBox.LetterListType.LltWedding);
                SendPacket(EventId.EGS_RENEWAL_LETTER_ACK, new KEGS_NEW_POST_LETTER_INFO_NOT
                {
                    NewUserLetterCount = newUser,
                    NewSystemLetterCount = newSys,
                    NewRelationshipLetterCount = newRel,
                    TotalLetterCount = total,
                    NewLetterTitle = newTitles,
                });
                break;
            }

            case EventId.EGS_SEND_LETTER_REQ:
            {
                if (ActiveUnit == null) break;
                if (_server.GameSysValTable.BlockSendLetter)
                {
                    SendPacket(EventId.EGS_SEND_LETTER_ACK, new KEGS_SEND_LETTER_ACK { Ok = NetError.ERR_BLOCK_TRADE_00, Ed = ActiveUnit.Ed });
                    break;
                }
                if (AbuserManager.IsTradeBlockUser || AbuserManager.IsTradeBlockUnit)
                {
                    SendPacket(EventId.EGS_SEND_LETTER_ACK, new KEGS_SEND_LETTER_ACK { Ok = NetError.ERR_TRADE_BLOCK_UNIT_01, Ed = ActiveUnit.Ed });
                    break;
                }
                if (IsGuestUser)
                {
                    SendPacket(EventId.EGS_SEND_LETTER_ACK, new KEGS_SEND_LETTER_ACK { Ok = NetError.ERR_GUEST_USER_00, Ed = ActiveUnit.Ed });
                    break;
                }
                var req = ev.GetPayload<KEGS_SEND_LETTER_REQ>(Region);
                if (string.Equals(req.ToNickName, ActiveUnit.NickName, StringComparison.OrdinalIgnoreCase))
                {
                    SendPacket(EventId.EGS_SEND_LETTER_ACK, new KEGS_SEND_LETTER_ACK { Ok = NetError.ERR_POST_LETTER_11, Ed = ActiveUnit.Ed });
                    break;
                }
                long? toUid = _server.Db.FindUnitUidByNickName(Region, req.ToNickName);
                if (toUid == null)
                {
                    SendPacket(EventId.EGS_SEND_LETTER_ACK, new KEGS_SEND_LETTER_ACK
                    {
                        Ok = NetError.ERR_POST_LETTER_01,
                        Ed = ActiveUnit.Ed,
                    });
                    break;
                }

                string title = AuthLevel < 5 ? _server.StringFilterManager.FilteringNoteString(req.Title) : req.Title;
                string message = AuthLevel < 5 ? _server.StringFilterManager.FilteringNoteString(req.Message) : req.Message;

                int attachedEd = Math.Max(0, req.Ed);
                int scriptIndex = 0;
                int quantity = Math.Max(0, req.Quantity);
                var updatedSlots = new List<KInventoryItemInfo>();

                int totalEdDeduct = attachedEd;
                if (req.ItemUid > 0)
                {
                    var meta = _server.Db.GetInventoryItemMeta(ActiveUnit.UnitUid, req.ItemUid);
                    if (!meta.found)
                    {
                        SendPacket(EventId.EGS_SEND_LETTER_ACK, new KEGS_SEND_LETTER_ACK
                        {
                            Ok = NetError.ERR_POST_LETTER_04,
                            Ed = ActiveUnit.Ed,
                        });
                        break;
                    }
                    int sendQty = Math.Max(1, quantity);
                    var (delOk, slotInfo) = _server.Db.DeleteInventoryItem(ActiveUnit.UnitUid, req.ItemUid, sendQty);
                    if (!delOk)
                    {
                        SendPacket(EventId.EGS_SEND_LETTER_ACK, new KEGS_SEND_LETTER_ACK
                        {
                            Ok = NetError.ERR_POST_LETTER_04,
                            Ed = ActiveUnit.Ed,
                        });
                        break;
                    }
                    scriptIndex = meta.itemId;
                    quantity = sendQty;
                    updatedSlots.Add(slotInfo);
                }
                else if (attachedEd > 0)
                {
                    scriptIndex = KUserLetterBox.EdiBronzeEd;
                    quantity = attachedEd;
                }

                if (totalEdDeduct > 0)
                {
                    if (!EdManager.TryAddEd(-totalEdDeduct, KUserEDManager.EdReason.ErOutEdSendLetterCommission, out _))
                    {
                        SendPacket(EventId.EGS_SEND_LETTER_ACK, new KEGS_SEND_LETTER_ACK
                        {
                            Ok = NetError.ERR_POST_LETTER_00,
                            Ed = ActiveUnit.Ed,
                        });
                        break;
                    }
                    _server.Db.UpdateUnitProgress(ActiveUnit.UnitUid, 0, -totalEdDeduct, 0);
                    ActiveUnit = _server.Db.GetUnitByUid(Region, ActiveUnit.UnitUid)!;
                }

                long postNo = _server.Db.SendLetter(Region, ActiveUnit.UnitUid, ActiveUnit.NickName, toUid.Value,
                    title, message, scriptIndex, quantity, attachedEd, (sbyte)KUserLetterBox.LetterType.LtPostOffice);

                var recipient = _server.GetUserByUnitUid(toUid.Value);
                if (recipient != null)
                {
                    recipient.LetterBox.AddLetter(new KPostItemInfo
                    {
                        PostNo = postNo,
                        FromUnitUid = ActiveUnit.UnitUid,
                        ToUnitUid = toUid.Value,
                        ScriptType = (sbyte)KUserLetterBox.LetterType.LtPostOffice,
                        ScriptIndex = scriptIndex,
                        Quantity = quantity,
                        RegDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                        FromNickName = ActiveUnit.NickName,
                        Title = title,
                        Message = message,
                        Read = false,
                    });
                    recipient.SendPacket(EventId.EGS_RECEIVE_LETTER_NOT, new KEGS_RECEIVE_LETTER_NOT
                    {
                        NickName = ActiveUnit.NickName,
                        ScriptType = (sbyte)KUserLetterBox.LetterType.LtPostOffice,
                        LetterTitle = title,
                        ScriptIndex = scriptIndex,
                        FromUnitUid = ActiveUnit.UnitUid,
                    });
                }

                SendPacket(EventId.EGS_SEND_LETTER_ACK, new KEGS_SEND_LETTER_ACK
                {
                    Ok = NetError.NET_OK,
                    Ed = ActiveUnit.Ed,
                    InventorySlotInfo = updatedSlots,
                });
                break;
            }

            case EventId.EGS_READ_LETTER_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_READ_LETTER_REQ>(Region);
                var letter = _server.Db.ReadLetter(ActiveUnit.UnitUid, req.PostNo);
                LetterBox.ReadLetter(req.PostNo, out _, out _);
                SendPacket(EventId.EGS_READ_LETTER_ACK, new KEGS_READ_LETTER_ACK
                {
                    Ok = letter != null ? NetError.NET_OK : NetError.ERR_POST_LETTER_04,
                    Letter = letter ?? new KPostItemInfo(),
                });
                break;
            }

            case EventId.EGS_GET_ITEM_FROM_LETTER_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_READ_LETTER_REQ>(Region);
                if (!LetterBox.TryGetLetter(req.PostNo, out var postItem) || postItem == null)
                {
                    SendPacket(EventId.EGS_GET_ITEM_FROM_LETTER_ACK, new KEGS_GET_ITEM_FROM_LETTER_ACK { Ok = NetError.ERR_POST_LETTER_04 });
                    break;
                }
                if (postItem.ScriptIndex <= 0)
                {
                    SendPacket(EventId.EGS_GET_ITEM_FROM_LETTER_ACK, new KEGS_GET_ITEM_FROM_LETTER_ACK { Ok = NetError.ERR_POST_LETTER_06 });
                    break;
                }
                if (!_server.Db.ClaimLetterAttachment(ActiveUnit.UnitUid, req.PostNo))
                {
                    SendPacket(EventId.EGS_GET_ITEM_FROM_LETTER_ACK, new KEGS_GET_ITEM_FROM_LETTER_ACK { Ok = NetError.ERR_POST_LETTER_06 });
                    break;
                }

                var resultItems = new Dictionary<int, int>();
                var slots = new List<KInventoryItemInfo>();
                if (postItem.ScriptIndex == KUserLetterBox.EdiBronzeEd)
                {
                    int edGain = Math.Max(0, postItem.Quantity);
                    EdManager.TryAddEd(edGain, KUserEDManager.EdReason.ErInEdGetEdFromLetter, out _);
                    _server.Db.UpdateUnitProgress(ActiveUnit.UnitUid, 0, edGain, 0);
                    ActiveUnit = _server.Db.GetUnitByUid(Region, ActiveUnit.UnitUid)!;

                    if (_server.GameSysValTable.LetterAndTradeMonitoringIsOn &&
                        edGain >= _server.GameSysValTable.MonitoringMinEd &&
                        postItem.FromUnitUid > 0 &&
                        HackingManager.AddLetterMonitoringList(postItem.FromUnitUid, _server.GameSysValTable))
                    {
                        var monNow = DateTime.UtcNow;
                        _server.Db.LogLetterAndTradeMonitoring(ActiveUnit.UnitUid, 0, monNow.ToString("yyyy-MM-dd HH:mm:ss"));
                        if (_server.GameSysValTable.IsMonitoringLetterAndTradeBlockTime(monNow.TimeOfDay))
                        {
                            HackingUserSendType = KEGS_SERVER_CHECK_HACK_USER_REQ.HctMonitoringLetterAndTrade;
                            HackingManager.SetHackingUserSendType(HackingUserSendType, monNow);
                            SendPacket(EventId.EGS_SERVER_CHECK_HACK_USER_REQ, new KEGS_SERVER_CHECK_HACK_USER_REQ
                            {
                                HackingCheckType = KEGS_SERVER_CHECK_HACK_USER_REQ.HctMonitoringLetterAndTrade,
                            });
                            ReserveDestroy();
                        }
                    }
                }
                else
                {
                    int qty = Math.Max(1, postItem.Quantity);
                    var granted = _server.Db.GrantUnitItem(ActiveUnit.UnitUid, postItem.ScriptIndex, qty);
                    slots.Add(granted);
                    resultItems[postItem.ScriptIndex] = qty;
                }
                LetterBox.SetReceivedFlag(req.PostNo);
                SendPacket(EventId.EGS_GET_ITEM_FROM_LETTER_ACK, new KEGS_GET_ITEM_FROM_LETTER_ACK
                {
                    Ok = NetError.NET_OK,
                    PostNo = req.PostNo,
                    LetterType = postItem.ScriptType,
                    ResultItem = resultItems,
                    InventorySlotInfo = slots,
                    Ed = ActiveUnit.Ed,
                });
                break;
            }

            case EventId.EGS_DELETE_LETTER_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_DELETE_LETTER_REQ>(Region);
                _server.Db.DeleteLetters(ActiveUnit.UnitUid, req.PostNos);
                LetterBox.DeleteLetter(req.PostNos);
                SendPacket(EventId.EGS_DELETE_LETTER_ACK, new KEGS_DELETE_LETTER_ACK
                {
                    Ok = NetError.NET_OK,
                    PostNos = req.PostNos,
                });
                break;
            }

            case EventId.EGS_CREATE_GUILD_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_CREATE_GUILD_REQ>(Region);
                var (ok, guild, err) = _server.Db.CreateGuild(Region, ActiveUnit.UnitUid, req.GuildName, req.GuildMessage);
                if (ok && guild != null)
                {
                    ActiveUnit.UserGuildInfo.GuildUid = guild.GuildUid;
                    ActiveUnit.UserGuildInfo.GuildName = guild.GuildName;
                    ActiveUnit.UserGuildInfo.MemberShipGrade = 1;
                }
                SendPacket(EventId.EGS_CREATE_GUILD_ACK, new KEGS_CREATE_GUILD_ACK
                {
                    Ok = err,
                    GuildInfo = guild ?? new KGuildInfo(),
                });
                break;
            }

            case EventId.EGS_APPLY_JOIN_GUILD_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_APPLY_JOIN_GUILD_REQ>(Region);
                int err = ActiveUnit.UserGuildInfo.GuildUid > 0
                    ? NetError.ERR_GUILD_05
                    : _server.Db.ApplyJoinGuild(Region, req.GuildUid, ActiveUnit, req.Message, req.OldApplyDelete);
                SendPacket(EventId.EGS_APPLY_JOIN_GUILD_ACK, new KPacketOK { Ok = err });
                break;
            }

            case EventId.EGS_KICK_GUILD_MEMBER_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_KICK_GUILD_MEMBER_REQ>(Region);
                if (ActiveUnit.UserGuildInfo.GuildUid <= 0)
                {
                    SendPacket(EventId.EGS_KICK_GUILD_MEMBER_ACK, new KEGS_KICK_GUILD_MEMBER_ACK
                    {
                        Ok = NetError.ERR_GUILD_19,
                        KickBySysop = false,
                    });
                    break;
                }
                long targetUnitUid = req.UnitUid > 0 ? req.UnitUid : ActiveUnit.UnitUid;
                bool isSelf = targetUnitUid == ActiveUnit.UnitUid;
                bool ok = _server.Db.QuitGuild(targetUnitUid);
                if (ok && isSelf)
                {
                    ActiveUnit.UserGuildInfo.GuildUid = 0;
                    ActiveUnit.UserGuildInfo.GuildName = string.Empty;
                }
                SendPacket(EventId.EGS_KICK_GUILD_MEMBER_ACK, new KEGS_KICK_GUILD_MEMBER_ACK
                {
                    Ok = ok ? NetError.NET_OK : NetError.ERR_GUILD_01,
                    KickBySysop = !isSelf,
                });
                break;
            }

            case EventId.EGS_CHANGE_GUILD_MEMBER_GRADE_REQ:
            {
                if (ActiveUnit == null || ActiveUnit.UserGuildInfo.GuildUid <= 0) break;
                var req = ev.GetPayload<KEGS_CHANGE_GUILD_MEMBER_GRADE_REQ>(Region);
                var (ok, targetNick) = _server.Db.ChangeGuildMemberGrade(
                    ActiveUnit.UserGuildInfo.GuildUid, req.UnitUid, req.MembershipGrade);
                SendPacket(EventId.EGS_CHANGE_GUILD_MEMBER_GRADE_ACK, new KEGS_CHANGE_GUILD_MEMBER_GRADE_ACK
                {
                    Ok = ok ? NetError.NET_OK : NetError.ERR_GUILD_01,
                    TargetNickName = targetNick,
                    MembershipGrade = req.MembershipGrade,
                });
                break;
            }

            case EventId.EGS_ADMIN_MODIFY_UNIT_LEVEL_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_ADMIN_MODIFY_UNIT_LEVEL_REQ>(Region);
                if (AuthLevel < 2 || !_server.CompanyIpTable.CheckIp(Session.RemoteIp))
                {
                    SendPacket(EventId.EGS_ADMIN_MODIFY_UNIT_LEVEL_ACK, new KEGS_ADMIN_MODIFY_UNIT_LEVEL_ACK
                    {
                        Ok = NetError.ERR_VERIFY_12,
                    });
                    break;
                }

                KUnitInfo? targetUnit = string.IsNullOrWhiteSpace(req.UnitNickName) ||
                    string.Equals(req.UnitNickName, ActiveUnit.NickName, StringComparison.OrdinalIgnoreCase)
                    ? ActiveUnit
                    : _server.Db.GetUnitByNickName(Region, req.UnitNickName);

                if (targetUnit == null)
                {
                    SendPacket(EventId.EGS_ADMIN_MODIFY_UNIT_LEVEL_ACK, new KEGS_ADMIN_MODIFY_UNIT_LEVEL_ACK
                    {
                        Ok = NetError.ERR_SEARCH_UNIT_04,
                    });
                    break;
                }

                _server.Db.SetUnitLevel(targetUnit.UnitUid, (byte)Math.Clamp(req.Level, 1, 99));
                var updated = _server.Db.GetUnitByUid(Region, targetUnit.UnitUid) ?? targetUnit;
                if (req.Exp > 0)
                    updated.Exp = req.Exp;
                if (updated.UnitUid == ActiveUnit.UnitUid)
                {
                    ActiveUnit = updated;
                    SkillTree.TryGetExpandSkillNotePage(ActiveUnit.Level, out _);
                }

                SendPacket(EventId.EGS_ADMIN_MODIFY_UNIT_LEVEL_ACK, new KEGS_ADMIN_MODIFY_UNIT_LEVEL_ACK
                {
                    Ok = NetError.NET_OK,
                    UnitInfo = updated,
                });
                break;
            }

            case EventId.EGS_ADMIN_CHANGE_ED_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_ADMIN_CHANGE_ED_REQ>(Region);
                if (AuthLevel < 2 || !_server.CompanyIpTable.CheckIp(Session.RemoteIp))
                {
                    SendPacket(EventId.EGS_ADMIN_CHANGE_ED_ACK, new KPacketOK { Ok = NetError.ERR_VERIFY_12 });
                    break;
                }

                KUnitInfo? targetUnit = string.IsNullOrWhiteSpace(req.UnitNickName) ||
                    string.Equals(req.UnitNickName, ActiveUnit.NickName, StringComparison.OrdinalIgnoreCase)
                    ? ActiveUnit
                    : _server.Db.GetUnitByNickName(Region, req.UnitNickName);

                if (targetUnit == null)
                {
                    SendPacket(EventId.EGS_ADMIN_CHANGE_ED_ACK, new KPacketOK { Ok = NetError.ERR_SEARCH_UNIT_04 });
                    break;
                }

                _server.Db.SetUnitEd(targetUnit.UnitUid, Math.Max(0, req.Ed));
                if (targetUnit.UnitUid == ActiveUnit.UnitUid)
                {
                    ActiveUnit = _server.Db.GetUnitByUid(Region, ActiveUnit.UnitUid);
                    if (ActiveUnit != null) EdManager.SetEd(ActiveUnit.Ed, KUserEDManager.EdReason.ErNoReason);
                }

                SendPacket(EventId.EGS_ADMIN_CHANGE_ED_ACK, new KPacketOK { Ok = NetError.NET_OK });
                break;
            }

            case EventId.EGS_ADMIN_CHANGE_SPIRIT_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_ADMIN_CHANGE_SPIRIT_REQ>(Region);
                if (AuthLevel < 2 || !_server.CompanyIpTable.CheckIp(Session.RemoteIp))
                {
                    SendPacket(EventId.EGS_ADMIN_CHANGE_SPIRIT_ACK, new KEGS_ADMIN_CHANGE_SPIRIT_ACK
                    {
                        Ok = NetError.ERR_VERIFY_12,
                    });
                    break;
                }

                int targetSpirit = Math.Clamp(req.Spirit, 0, Math.Max(100, ActiveUnit.SpiritMax));
                _server.Db.UpdateUnitSpirit(ActiveUnit.UnitUid, targetSpirit);
                ActiveUnit = _server.Db.GetUnitByUid(Region, ActiveUnit.UnitUid);
                int changedSpirit = ActiveUnit?.Spirit ?? targetSpirit;
                SpiritManager.RestoreSpirit(changedSpirit);

                SendPacket(EventId.EGS_ADMIN_CHANGE_SPIRIT_ACK, new KEGS_ADMIN_CHANGE_SPIRIT_ACK
                {
                    Ok = NetError.NET_OK,
                    ChangedSpirit = changedSpirit,
                });
                break;
            }

            case EventId.EGS_NEW_QUEST_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_NEW_QUEST_REQ>(Region);
                var (accepted, inst) = QuestManager.AcceptQuest(ActiveUnit.UnitUid, req.QuestId);
                if (!accepted || inst == null)
                {
                    SendPacket(EventId.EGS_NEW_QUEST_ACK, new KPacketOK { Ok = NetError.ERR_QUEST_02 });
                    break;
                }
                _server.Db.AcceptQuest(ActiveUnit.UnitUid, req.QuestId);
                SendPacket(EventId.EGS_NEW_QUEST_ACK, new KPacketOK { Ok = NetError.NET_OK });
                SendPacket(EventId.EGS_NEW_QUEST_NOT, new KEGS_NEW_QUEST_NOT { QuestInst = inst });
                break;
            }

            case EventId.EGS_QUEST_COMPLETE_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_QUEST_COMPLETE_REQ>(Region);
                var (canComplete, rewardEd, rewardExp) = QuestManager.CompleteQuest(req.QuestId);
                bool ok = canComplete && _server.Db.CompleteQuest(ActiveUnit.UnitUid, req.QuestId);
                if (ok)
                {
                    EdManager.TryAddEd(rewardEd, KUserEDManager.EdReason.ErInEdQuestReward, out _);
                    _server.Db.UpdateUnitProgress(ActiveUnit.UnitUid, addExp: rewardExp, addEd: rewardEd, mapId: 0);
                    ActiveUnit = _server.Db.GetUnitByUid(Region, ActiveUnit.UnitUid);
                    _server.StatisticsManager.IncrementCounter("QuestCompleted");
                }
                SendPacket(EventId.EGS_QUEST_COMPLETE_ACK, new KEGS_QUEST_COMPLETE_ACK
                {
                    Ok = ok ? NetError.NET_OK : NetError.ERR_QUEST_02,
                    CompleteQuestInfo = new KCompleteQuestInfo
                    {
                        QuestId = req.QuestId,
                        CompleteCount = ok ? 1 : 0,
                        CompleteDate = ok ? DateTimeOffset.UtcNow.ToUnixTimeSeconds() : 0,
                    },
                    UpdateUnitInfo = new KEGS_UPDATE_UNIT_INFO_NOT
                    {
                        UnitInfo = ActiveUnit ?? new KUnitInfo(),
                    },
                });
                break;
            }

            // -------- Matchmaking (via GlobalServer) --------

            case EventId.EGS_REQUEST_MATCH_MAKING_REQ:
            {
                if (ActiveUnit == null) break;
                var ks = new KSerializer(Region);
                ks.BeginReading(ev.Buffer);
                ks.Get(out sbyte matchType);
                ks.EndReading();
                _server.Global.MatchMakingManager.Enqueue(new MatchRequestEntry(
                    ActiveUnit.UnitUid, ActiveUnit.NickName, ActiveUnit.Rating, Math.Max((sbyte)1, matchType), DateTime.UtcNow));
                SendPacket(EventId.EGS_REQUEST_MATCH_MAKING_ACK, new KPacketOK { Ok = NetError.NET_OK });
                while (_server.Global.MatchMakingManager.TryDequeueMatch(out var match) && match != null)
                {
                    var matchNot = new KEGS_MATCH_MAKING_SUCCESS_NOT { MatchUid = match.MatchUid };
                    foreach (var m in match.TeamA.Concat(match.TeamB))
                        _server.SendToUnit(m.UnitUid, EventId.EGS_MATCH_MAKING_SUCCESS_NOT, matchNot);
                }
                break;
            }

            case EventId.EGS_MATCH_MAKING_SUCCESS_REPLY_NOT:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_MATCH_MAKING_SUCCESS_REPLY_NOT>(Region);
                if (!_server.Global.MatchMakingManager.SubmitReply(
                    ActiveUnit.UnitUid, req.GameStartAgree, out var readyMatch, out var cancelledMatch))
                    break;

                if (cancelledMatch != null)
                {
                    var closeNot = new KEGS_MATCH_CLOSE_NOT
                    {
                        Reason = NetError.ERR_MATCH_MAKING_08,
                        CancelUnitUid = ActiveUnit.UnitUid,
                    };
                    foreach (var m in cancelledMatch.TeamA.Concat(cancelledMatch.TeamB))
                        _server.SendToUnit(m.UnitUid, EventId.EGS_MATCH_CLOSE_NOT, closeNot);
                    break;
                }

                if (readyMatch != null)
                {
                    int totalSlots = readyMatch.TeamA.Count + readyMatch.TeamB.Count;
                    var template = new KRoomInfo
                    {
                        RoomType = 0, // RT_PVP
                        RoomName = $"Match #{readyMatch.MatchUid}",
                        Public = false,
                        MaxSlot = (sbyte)Math.Clamp(totalSlots, 2, 8),
                        PvpGameType = readyMatch.MatchType,
                        IsItemMode = req.PvpItemMode,
                        WorldId = req.SelectedPvpMap.Count > 0 ? req.SelectedPvpMap[0] : (short)10001,
                    };
                    var room = _server.Center.RoomManager.CreateRoom(template);
                    var slots = new List<KRoomSlotInfo>();
                    foreach (var m in readyMatch.TeamA.Concat(readyMatch.TeamB))
                    {
                        var u = _server.GetUserByUnitUid(m.UnitUid);
                        if (u?.ActiveUnit == null) continue;
                        u.LeaveCurrentField();
                        var (ok, slot) = room.TryAddUser(u.BuildRoomUserInfo(), asObserver: false, password: string.Empty);
                        if (ok == NetError.NET_OK && slot != null)
                        {
                            slot.TeamNum = readyMatch.TeamA.Any(a => a.UnitUid == m.UnitUid) ? 0 : 1;
                            slot.IsReady = true;
                            u.CurrentRoomUid = room.Info.RoomUid;
                            u.Fsm.TryStateTransition(KGSFSM.I_TO_ROOM, out _);
                            slots.Add(slot);
                        }
                    }
                    room.StartGame();
                    var startNot = new KEGS_GAME_START_PVP_MATCH_NOT
                    {
                        RoomInfo = room.Info,
                        Slots = slots,
                        ObserverSlots = room.ObserverSlots.ToList(),
                    };
                    foreach (var m in readyMatch.TeamA.Concat(readyMatch.TeamB))
                        _server.SendToUnit(m.UnitUid, EventId.EGS_GAME_START_PVP_MATCH_NOT, startNot);
                }
                break;
            }

            case EventId.EGS_CANCEL_MATCH_MAKING_REQ:
            {
                if (ActiveUnit == null) break;
                _server.Global.MatchMakingManager.Cancel(ActiveUnit.UnitUid);
                SendPacket(EventId.EGS_CANCEL_MATCH_MAKING_ACK, new KPacketOK { Ok = NetError.NET_OK });
                break;
            }

            case EventId.E_CHECK_DDOS_GUARD_REQ:
            {
                var evAck = new KEvent { EventId = EventId.E_CHECK_DDOS_GUARD_ACK };
                evAck.DestPerformer.PerformerId = PerformerId.PI_GS_PROXY;
                evAck.DestPerformer.AddUID(Uid);
                var ks = new KSerializer(Region);
                ks.BeginWriting(evAck.Buffer);
                ks.Put((sbyte)0);
                ks.EndWriting();
                _ = Session.SendEventAsync(evAck);
                break;
            }

            // -------- Socket / Resolve / Attribute Enchant --------

            case EventId.EGS_SOCKET_ITEM_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_SOCKET_ITEM_REQ>(Region);
                if (!InventoryLocker.Lock(KInventoryLocker.LockType.TimeOut, 30.0))
                {
                    SendPacket(EventId.EGS_SOCKET_ITEM_ACK, new KEGS_SOCKET_ITEM_ACK { Ok = NetError.ERR_INVENTORY_LOCK_00, ItemUid = req.ItemUid });
                    break;
                }
                try
                {
                    if (IsTradeOrShopOpen())
                    {
                        SendPacket(EventId.EGS_SOCKET_ITEM_ACK, new KEGS_SOCKET_ITEM_ACK { Ok = NetError.ERR_ITEM_14, ItemUid = req.ItemUid });
                        break;
                    }
                    var (found, targetSlot, isBroken, isSealed) = _server.Db.GetItemFullDetails(ActiveUnit.UnitUid, req.ItemUid);
                    if (!found || targetSlot.SlotCategory == 9 || isBroken || isSealed)
                    {
                        SendPacket(EventId.EGS_SOCKET_ITEM_ACK, new KEGS_SOCKET_ITEM_ACK { Ok = NetError.ERR_SOCKET_ITEM_00, ItemUid = req.ItemUid });
                        break;
                    }

                    int slotCount = Math.Max(1, req.SocketInfo.Count);
                    int socketCostEd = req.Cheat ? 0 : Math.Max(1000, slotCount * 2000);
                    if (socketCostEd > 0 && ActiveUnit.Ed < socketCostEd)
                    {
                        SendPacket(EventId.EGS_SOCKET_ITEM_ACK, new KEGS_SOCKET_ITEM_ACK { Ok = NetError.ERR_SOCKET_ITEM_05, ItemUid = req.ItemUid, Ed = ActiveUnit.Ed });
                        break;
                    }

                    bool isWeapon = targetSlot.ItemInfo.ItemId % 10 == 0 || targetSlot.SlotId == 0;
                    var changedSlots = new List<KInventoryItemInfo>();
                    var rolled = new List<int>(slotCount);

                    if (!req.Cheat && req.SocketInfo.Count > 0)
                    {
                        var stoneCounts = new Dictionary<long, int>();
                        var stoneItems = new Dictionary<long, int>();
                        foreach (long stoneUid in req.SocketInfo.Values)
                        {
                            if (stoneUid <= 0)
                            {
                                SendPacket(EventId.EGS_SOCKET_ITEM_ACK, new KEGS_SOCKET_ITEM_ACK { Ok = NetError.ERR_SOCKET_ITEM_01, ItemUid = req.ItemUid });
                                goto SocketDone;
                            }
                            var (sFound, sSlot, _, _) = _server.Db.GetItemFullDetails(ActiveUnit.UnitUid, stoneUid);
                            if (!sFound || sSlot.SlotCategory == 9)
                            {
                                SendPacket(EventId.EGS_SOCKET_ITEM_ACK, new KEGS_SOCKET_ITEM_ACK { Ok = NetError.ERR_SOCKET_ITEM_01, ItemUid = req.ItemUid });
                                goto SocketDone;
                            }
                            stoneCounts[stoneUid] = stoneCounts.GetValueOrDefault(stoneUid) + 1;
                            stoneItems[stoneUid] = sSlot.ItemInfo.ItemId;
                            if (sSlot.ItemInfo.Quantity < stoneCounts[stoneUid])
                            {
                                SendPacket(EventId.EGS_SOCKET_ITEM_ACK, new KEGS_SOCKET_ITEM_ACK { Ok = NetError.ERR_SOCKET_ITEM_01, ItemUid = req.ItemUid });
                                goto SocketDone;
                            }
                        }

                        foreach (var kv in stoneCounts)
                        {
                            var (delOk, delSlot) = _server.Db.DeleteInventoryItem(ActiveUnit.UnitUid, kv.Key, kv.Value);
                            if (delOk) changedSlots.Add(delSlot);
                        }

                        foreach (long stoneUid in req.SocketInfo.Values)
                        {
                            var sdt = KSocketItemManager.GetSocketDataTypeForMagicStone(stoneItems[stoneUid], isWeapon);
                            rolled.Add(_server.SocketItemManager.RollSocketOption(sdt));
                        }
                    }
                    else
                    {
                        for (int i = 0; i < slotCount; i++)
                            rolled.Add(_server.SocketItemManager.RollSocketOption(isWeapon
                                ? KSocketItemManager.SocketDataType.SdtWeaponNormal
                                : KSocketItemManager.SocketDataType.SdtDefenceNormal));
                    }

                    if (socketCostEd > 0 && EdManager.TryAddEd(-socketCostEd, KUserEDManager.EdReason.ErNoReason, out int afterEd))
                    {
                        _server.Db.UpdateUnitProgress(ActiveUnit.UnitUid, 0, -socketCostEd, 0);
                        ActiveUnit.Ed = afterEd;
                    }

                    var updated = _server.Db.UpdateItemSockets(ActiveUnit.UnitUid, req.ItemUid, rolled);
                    changedSlots.Add(updated);
                    SendPacket(EventId.EGS_SOCKET_ITEM_ACK, new KEGS_SOCKET_ITEM_ACK
                    {
                        Ok = NetError.NET_OK,
                        Ed = ActiveUnit.Ed,
                        ItemUid = req.ItemUid,
                        InventorySlotInfo = changedSlots,
                    });
                SocketDone:;
                }
                finally
                {
                    InventoryLocker.Unlock();
                }
                break;
            }

            case EventId.EGS_RESOLVE_ITEM_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_RESOLVE_ITEM_REQ>(Region);
                if (!InventoryLocker.Lock(KInventoryLocker.LockType.TimeOut, 30.0))
                {
                    SendPacket(EventId.EGS_RESOLVE_ITEM_ACK, new KEGS_RESOLVE_ITEM_ACK { Ok = NetError.ERR_INVENTORY_LOCK_00 });
                    break;
                }
                try
                {
                    if (IsTradeOrShopOpen())
                    {
                        SendPacket(EventId.EGS_RESOLVE_ITEM_ACK, new KEGS_RESOLVE_ITEM_ACK { Ok = NetError.ERR_ITEM_14 });
                        break;
                    }
                    var (found, itemSlot, isBroken, isSealed) = _server.Db.GetItemFullDetails(ActiveUnit.UnitUid, req.ItemUid);
                    if (!found || isSealed)
                    {
                        SendPacket(EventId.EGS_RESOLVE_ITEM_ACK, new KEGS_RESOLVE_ITEM_ACK { Ok = NetError.ERR_RESOLVE_ITEM_00 });
                        break;
                    }
                    if (itemSlot.SlotCategory == 9)
                    {
                        SendPacket(EventId.EGS_RESOLVE_ITEM_ACK, new KEGS_RESOLVE_ITEM_ACK { Ok = NetError.ERR_BANK_00 });
                        break;
                    }
                    if (isBroken)
                    {
                        SendPacket(EventId.EGS_RESOLVE_ITEM_ACK, new KEGS_RESOLVE_ITEM_ACK { Ok = NetError.ERR_RESTORE_ITEM_06 });
                        break;
                    }
                    Dictionary<int, int> outputs;
                    bool jackpot = false;
                    if (_server.CashItemPrices.TryGetValue(itemSlot.ItemInfo.ItemId, out uint cashPrice) && cashPrice > 0 &&
                        KResolveItemManager.ResolveFashionItem((int)cashPrice, out var fashionOutputs, out _))
                    {
                        outputs = new Dictionary<int, int>(fashionOutputs);
                        if (_server.TryResolveCashItemAdd(itemSlot.ItemInfo.ItemId, out var cashAddOutputs))
                        {
                            foreach (var kv in cashAddOutputs)
                                outputs[kv.Key] = outputs.GetValueOrDefault(kv.Key) + kv.Value;
                        }
                    }
                    else if (_server.TryResolveCashItemAdd(itemSlot.ItemInfo.ItemId, out var standaloneCashOutputs) && standaloneCashOutputs.Count > 0)
                    {
                        outputs = standaloneCashOutputs;
                    }
                    else
                    {
                        bool isWeapon = itemSlot.ItemInfo.ItemId % 2 == 1;
                        int inferredGrade = itemSlot.ItemInfo.ItemSockets.Count >= 2 ? 4 :
                            itemSlot.ItemInfo.EnchantLevel > 0 ? 3 : 2;
                        (outputs, jackpot) = _server.ResolveItemManager.ResolveItem(
                            ActiveUnit.Level, isWeapon, itemGrade: inferredGrade);
                    }
                    var (ok, updatedSlots) = _server.Db.ResolveInventoryItem(ActiveUnit.UnitUid, req.ItemUid, outputs);
                    SendPacket(EventId.EGS_RESOLVE_ITEM_ACK, new KEGS_RESOLVE_ITEM_ACK
                    {
                        Ok = ok ? NetError.NET_OK : NetError.ERR_RESOLVE_ITEM_04,
                        Jackpot = jackpot,
                        InsertedItems = outputs,
                        InventorySlotInfo = updatedSlots,
                    });
                }
                finally
                {
                    InventoryLocker.Unlock();
                }
                break;
            }

            case EventId.EGS_ATTRIB_ENCHANT_ITEM_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_ATTRIB_ENCHANT_ITEM_REQ>(Region);
                if (!InventoryLocker.Lock(KInventoryLocker.LockType.TimeOut, 30.0))
                {
                    SendPacket(EventId.EGS_ATTRIB_ENCHANT_ITEM_ACK, new KEGS_ATTRIB_ENCHANT_ITEM_ACK { Ok = NetError.ERR_INVENTORY_LOCK_00 });
                    break;
                }
                try
                {
                    if (IsTradeOrShopOpen())
                    {
                        SendPacket(EventId.EGS_ATTRIB_ENCHANT_ITEM_ACK, new KEGS_ATTRIB_ENCHANT_ITEM_ACK { Ok = NetError.ERR_ITEM_14 });
                        break;
                    }
                    if (req.AttribEnchantSlotNo is < 0 or > 2)
                    {
                        SendPacket(EventId.EGS_ATTRIB_ENCHANT_ITEM_ACK, new KEGS_ATTRIB_ENCHANT_ITEM_ACK { Ok = NetError.ERR_ATTRIB_ENCHANT_00 });
                        break;
                    }
                    var (found, itemSlot, isBroken, isSealed) = _server.Db.GetItemFullDetails(ActiveUnit.UnitUid, req.ItemUid);
                    if (!found || itemSlot.SlotCategory == 9 || isBroken || isSealed)
                    {
                        SendPacket(EventId.EGS_ATTRIB_ENCHANT_ITEM_ACK, new KEGS_ATTRIB_ENCHANT_ITEM_ACK { Ok = NetError.ERR_ATTRIB_ENCHANT_00 });
                        break;
                    }

                    bool isWeapon = itemSlot.ItemInfo.ItemId % 10 == 0 || itemSlot.SlotId == 0;
                    var enchantType = (isWeapon, req.AttribEnchantSlotNo) switch
                    {
                        (true, 0) => KSocketItemManager.AttribEnchantType.AetSingleWeapon,
                        (true, 1) => KSocketItemManager.AttribEnchantType.AetDualWeapon,
                        (true, 2) => KSocketItemManager.AttribEnchantType.AetTripleWeapon,
                        (false, 0) => KSocketItemManager.AttribEnchantType.AetArmor,
                        _ => KSocketItemManager.AttribEnchantType.AetDualArmor,
                    };
                    int shardItemId = KSocketItemManager.GetShardItemIdForElement(req.AttribEnchantId);
                    int requiredShards = KSocketItemManager.GetRequiredShardCount(enchantType, ActiveUnit.Level, KSocketItemManager.ItemGrade.IgNormal);
                    int edCost = Math.Max(1000, requiredShards * 500);
                    if (ActiveUnit.Ed < edCost)
                    {
                        SendPacket(EventId.EGS_ATTRIB_ENCHANT_ITEM_ACK, new KEGS_ATTRIB_ENCHANT_ITEM_ACK { Ok = NetError.ERR_ATTRIB_ENCHANT_05, Ed = ActiveUnit.Ed });
                        break;
                    }

                    var (shardOk, shardSlots) = _server.Db.TryConsumeItemsByItemId(ActiveUnit.UnitUid, shardItemId, requiredShards);
                    if (!shardOk)
                    {
                        SendPacket(EventId.EGS_ATTRIB_ENCHANT_ITEM_ACK, new KEGS_ATTRIB_ENCHANT_ITEM_ACK { Ok = NetError.ERR_ATTRIB_ENCHANT_01, Ed = ActiveUnit.Ed });
                        break;
                    }

                    if (EdManager.TryAddEd(-edCost, KUserEDManager.EdReason.ErOutEdItemAttribute, out int afterEd))
                    {
                        _server.Db.UpdateUnitProgress(ActiveUnit.UnitUid, 0, -edCost, 0);
                        ActiveUnit.Ed = afterEd;
                    }

                    sbyte resolvedAttrib = req.AttribEnchantId is <= 0 or 7
                        ? _server.SocketItemManager.RollOrResolveAttribute(
                            KSocketItemManager.AtiUnknown,
                            itemSlot.ItemInfo.AttribEnchantInfo.AttribEnchant0,
                            currentSlot1Attrib: itemSlot.ItemInfo.AttribEnchantInfo.AttribEnchant1)
                        : req.AttribEnchantId;
                    var updated = _server.Db.UpdateItemAttrib(ActiveUnit.UnitUid, req.ItemUid, req.AttribEnchantSlotNo, resolvedAttrib);
                    shardSlots.Add(updated);
                    SendPacket(EventId.EGS_ATTRIB_ENCHANT_ITEM_ACK, new KEGS_ATTRIB_ENCHANT_ITEM_ACK
                    {
                        Ok = NetError.NET_OK,
                        Ed = ActiveUnit.Ed,
                        EnchantedItemUid = req.ItemUid,
                        EnchantedItemId = itemSlot.ItemInfo.ItemId,
                        AttribEnchantSlotNo = req.AttribEnchantSlotNo,
                        AttribEnchantId = resolvedAttrib,
                        InventorySlotInfo = shardSlots,
                    });
                }
                finally
                {
                    InventoryLocker.Unlock();
                }
                break;
            }

            // -------- Player-to-Player Trade --------

            case EventId.EGS_REQUEST_TRADE_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_REQUEST_TRADE_REQ>(Region);
                if (_server.GameSysValTable.BlockPersonalTrade)
                {
                    SendPacket(EventId.EGS_REQUEST_TRADE_ACK, new KEGS_REQUEST_TRADE_ACK { Ok = NetError.ERR_BLOCK_TRADE_00, UnitUid = req.UnitUid });
                    break;
                }
                if (AbuserManager.IsTradeBlockUser ||
                    PunishmentManager.IsAppliedPunishment((int)KPunishmentManager.PunishmentType.PtTrade) ||
                    _server.Global.BlockListManager.IsTradeBlockUser(AccountUid))
                {
                    SendPacket(EventId.EGS_REQUEST_TRADE_ACK, new KEGS_REQUEST_TRADE_ACK { Ok = NetError.ERR_BLOCK_TRADE_07, UnitUid = req.UnitUid });
                    break;
                }
                if (AbuserManager.IsTradeBlockUnit)
                {
                    SendPacket(EventId.EGS_REQUEST_TRADE_ACK, new KEGS_REQUEST_TRADE_ACK { Ok = NetError.ERR_TRADE_BLOCK_UNIT_02, UnitUid = req.UnitUid });
                    break;
                }
                if (IsGuestUser)
                {
                    SendPacket(EventId.EGS_REQUEST_TRADE_ACK, new KEGS_REQUEST_TRADE_ACK { Ok = NetError.ERR_GUEST_USER_00, UnitUid = req.UnitUid });
                    break;
                }
                if (req.UnitUid == ActiveUnit.UnitUid)
                {
                    SendPacket(EventId.EGS_REQUEST_TRADE_ACK, new KEGS_REQUEST_TRADE_ACK { Ok = NetError.ERR_TRADE_13, UnitUid = req.UnitUid });
                    break;
                }
                var partner = _server.GetUserByUnitUid(req.UnitUid);
                if (partner?.ActiveUnit == null || partner.BlackList.VerifyBlackListUser(ActiveUnit.UnitUid))
                {
                    SendPacket(EventId.EGS_REQUEST_TRADE_ACK, new KEGS_REQUEST_TRADE_ACK { Ok = NetError.ERR_TRADE_00, UnitUid = req.UnitUid });
                    break;
                }
                if (AuthLevel >= 3 || partner.AuthLevel >= 3)
                {
                    SendPacket(EventId.EGS_REQUEST_TRADE_ACK, new KEGS_REQUEST_TRADE_ACK { Ok = NetError.ERR_GM_CHAR_DENY_00, UnitUid = req.UnitUid });
                    break;
                }
                if (partner.AbuserManager.IsTradeBlockUser || partner.AbuserManager.IsTradeBlockUnit || partner.IsGuestUser)
                {
                    SendPacket(EventId.EGS_REQUEST_TRADE_ACK, new KEGS_REQUEST_TRADE_ACK { Ok = NetError.NOT_REQUEST_TRADE_REPLY_02, UnitUid = req.UnitUid });
                    break;
                }
                var session = _server.TradeManager.CreateInvite(
                    ActiveUnit.UnitUid, ActiveUnit.NickName, partner.ActiveUnit.UnitUid, partner.ActiveUnit.NickName);
                if (session == null)
                {
                    SendPacket(EventId.EGS_REQUEST_TRADE_ACK, new KEGS_REQUEST_TRADE_ACK { Ok = NetError.ERR_TRADE_00, UnitUid = req.UnitUid });
                    break;
                }
                SendPacket(EventId.EGS_REQUEST_TRADE_ACK, new KEGS_REQUEST_TRADE_ACK
                {
                    Ok = NetError.NET_OK,
                    UnitUid = partner.ActiveUnit.UnitUid,
                    NickName = partner.ActiveUnit.NickName,
                });
                partner.SendPacket(EventId.EGS_REQUEST_TRADE_NOT, new KEGS_REQUEST_TRADE_NOT
                {
                    SenderUid = ActiveUnit.UnitUid,
                    SenderNickName = ActiveUnit.NickName,
                });
                break;
            }

            case EventId.EGS_REQUEST_TRADE_REPLY_NOT:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_REQUEST_TRADE_REPLY_NOT>(Region);
                var session = _server.TradeManager.GetSessionByUnit(ActiveUnit.UnitUid);
                if (session == null) break;
                long senderUid = session.Sender.UnitUid;
                bool accepted = _server.TradeManager.AcceptInvite(ActiveUnit.UnitUid, req.Accept);
                if (!accepted)
                    _server.SendToUnit(senderUid, EventId.EGS_REQUEST_TRADE_DENY_NOT, new KEGS_REQUEST_TRADE_DENY_NOT
                    {
                        Ok = NetError.ERR_TRADE_00,
                        UnitUid = ActiveUnit.UnitUid,
                    });
                else
                {
                    SendPacket(EventId.EGS_CREATE_TRADE_NOT, new KEGS_CREATE_TRADE_NOT { TradeUid = session.TradeUid });
                    _server.SendToUnit(senderUid, EventId.EGS_CREATE_TRADE_NOT, new KEGS_CREATE_TRADE_NOT { TradeUid = session.TradeUid });
                }
                break;
            }

            case EventId.EGS_UPDATE_TRADE_ITEM_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_UPDATE_TRADE_ITEM_REQ>(Region);
                bool ok = _server.TradeManager.UpdateItem(ActiveUnit.UnitUid, req.ItemUid, req.Quantity, req.Delete);
                SendPacket(EventId.EGS_UPDATE_TRADE_ITEM_ACK, new KPacketOK { Ok = ok ? NetError.NET_OK : NetError.ERR_TRADE_00 });
                break;
            }

            case EventId.EGS_UPDATE_TRADE_ED_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_UPDATE_TRADE_ED_REQ>(Region);
                bool ok = _server.TradeManager.UpdateEd(ActiveUnit.UnitUid, req.Ed);
                SendPacket(EventId.EGS_UPDATE_TRADE_ED_ACK, new KPacketOK { Ok = ok ? NetError.NET_OK : NetError.ERR_TRADE_00 });
                break;
            }

            case EventId.EGS_OFFER_TRADE_REQ:
            case EventId.EGS_APPROVE_TRADE_REQ:
            {
                if (ActiveUnit == null) break;
                var (bothConfirmed, session) = _server.TradeManager.Confirm(ActiveUnit.UnitUid);
                var ackEvent = ev.EventId == EventId.EGS_OFFER_TRADE_REQ ? EventId.EGS_OFFER_TRADE_ACK : EventId.EGS_APPROVE_TRADE_ACK;
                SendPacket(ackEvent, new KPacketOK { Ok = session != null ? NetError.NET_OK : NetError.ERR_TRADE_00 });
                if (bothConfirmed && session != null)
                {
                    var res = _server.Db.ExecutePlayerTrade(
                        session.Sender.UnitUid, session.Sender.OfferEd, session.Sender.OfferItems,
                        session.Receiver.UnitUid, session.Receiver.OfferEd, session.Receiver.OfferItems);
                    _server.TradeManager.CompleteSession(session, res.ok);
                    _server.SendToUnit(session.Sender.UnitUid, EventId.EGS_TRADE_COMPLETE_NOT, new KEGS_TRADE_COMPLETE_NOT
                    {
                        Ed = res.unitAEd,
                        InventorySlotInfo = res.unitAItems,
                    });
                    _server.SendToUnit(session.Receiver.UnitUid, EventId.EGS_TRADE_COMPLETE_NOT, new KEGS_TRADE_COMPLETE_NOT
                    {
                        Ed = res.unitBEd,
                        InventorySlotInfo = res.unitBItems,
                    });
                    ActiveUnit = _server.Db.GetUnitByUid(Region, ActiveUnit.UnitUid);
                    if (ActiveUnit != null) EdManager.SetEd(ActiveUnit.Ed, KUserEDManager.EdReason.ErSetEdInTrade);
                    var partnerOffer = session.GetPartnerOffer(ActiveUnit!.UnitUid);
                    var partner = partnerOffer != null ? _server.GetUserByUnitUid(partnerOffer.UnitUid) : null;
                    if (partner != null)
                    {
                        partner.ActiveUnit = _server.Db.GetUnitByUid(Region, partner.ActiveUnit!.UnitUid);
                        if (partner.ActiveUnit != null) partner.EdManager.SetEd(partner.ActiveUnit.Ed, KUserEDManager.EdReason.ErSetEdInTrade);
                    }
                    if (res.ok && _server.GameSysValTable.LetterAndTradeMonitoringIsOn)
                    {
                        int minEd = _server.GameSysValTable.MonitoringMinEd;
                        var monNow = DateTime.UtcNow;
                        bool inBlockWindow = _server.GameSysValTable.IsMonitoringLetterAndTradeBlockTime(monNow.TimeOfDay);
                        if (session.Sender.OfferEd >= minEd &&
                            partner != null && partner.HackingManager.AddTradeMonitoringList(session.Sender.UnitUid, _server.GameSysValTable))
                        {
                            _server.Db.LogLetterAndTradeMonitoring(session.Receiver.UnitUid, 1, monNow.ToString("yyyy-MM-dd HH:mm:ss"));
                            if (inBlockWindow)
                            {
                                partner.HackingUserSendType = KEGS_SERVER_CHECK_HACK_USER_REQ.HctMonitoringLetterAndTrade;
                                partner.HackingManager.SetHackingUserSendType(partner.HackingUserSendType, monNow);
                                partner.SendPacket(EventId.EGS_SERVER_CHECK_HACK_USER_REQ, new KEGS_SERVER_CHECK_HACK_USER_REQ
                                {
                                    HackingCheckType = KEGS_SERVER_CHECK_HACK_USER_REQ.HctMonitoringLetterAndTrade,
                                });
                                partner.ReserveDestroy();
                            }
                        }
                        if (session.Receiver.OfferEd >= minEd &&
                            HackingManager.AddTradeMonitoringList(session.Receiver.UnitUid, _server.GameSysValTable))
                        {
                            _server.Db.LogLetterAndTradeMonitoring(session.Sender.UnitUid, 1, monNow.ToString("yyyy-MM-dd HH:mm:ss"));
                            if (inBlockWindow)
                            {
                                HackingUserSendType = KEGS_SERVER_CHECK_HACK_USER_REQ.HctMonitoringLetterAndTrade;
                                HackingManager.SetHackingUserSendType(HackingUserSendType, monNow);
                                SendPacket(EventId.EGS_SERVER_CHECK_HACK_USER_REQ, new KEGS_SERVER_CHECK_HACK_USER_REQ
                                {
                                    HackingCheckType = KEGS_SERVER_CHECK_HACK_USER_REQ.HctMonitoringLetterAndTrade,
                                });
                                ReserveDestroy();
                            }
                        }
                    }
                }
                break;
            }

            case EventId.EGS_BREAK_TRADE_REQ:
            {
                if (ActiveUnit == null) break;
                var session = _server.TradeManager.GetSessionByUnit(ActiveUnit.UnitUid);
                long? partnerUid = session?.GetPartnerOffer(ActiveUnit.UnitUid)?.UnitUid;
                _server.TradeManager.CancelByUnit(ActiveUnit.UnitUid);
                SendPacket(EventId.EGS_BREAK_TRADE_ACK, new KPacketOK { Ok = NetError.NET_OK });
                if (partnerUid.HasValue)
                    _server.SendToUnit(partnerUid.Value, EventId.EGS_BREAK_TRADE_NOT, new KPacketOK { Ok = NetError.NET_OK });
                break;
            }

            // -------- Pets & Riding Pets --------

            case EventId.EGS_SUMMON_PET_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_SUMMON_PET_REQ>(Region);
                var (ok, pet) = PetManager.SummonOrUnsummon(req.SummonPetUid);
                SendPacket(EventId.EGS_SUMMON_PET_ACK, new KEGS_SUMMON_PET_ACK
                {
                    Ok = ok ? NetError.NET_OK : NetError.ERR_PET_00,
                    SummonedPetInfo = pet ?? new KPetInfo(),
                });
                break;
            }

            case EventId.EGS_SUMMON_RIDING_PET_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_SUMMON_RIDING_PET_REQ>(Region);
                var (res, rpet) = RidingPetManager.SummonRidingPet(req.RidingPetUid);
                if (req.NeedAck)
                {
                    SendPacket(EventId.EGS_SUMMON_RIDING_PET_ACK, new KEGS_SUMMON_RIDING_PET_ACK
                    {
                        Ok = res == KUserRidingPetManager.UserRidingPetManagerResult.RidingPetSucceed ? NetError.NET_OK : NetError.ERR_PET_00,
                        RidingPetInfo = rpet ?? new KRidingPetInfo(),
                    });
                }
                break;
            }

            // -------- Quest GiveUp / Skill Tree / Repair / PersonalShop Host / Pet Create&Feed / AutoParty --------

            case EventId.EGS_GIVE_UP_QUEST_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_GIVE_UP_QUEST_REQ>(Region);
                bool removed = QuestManager.GiveUpQuest(req.QuestId);
                if (removed) _server.Db.GiveUpQuest(ActiveUnit.UnitUid, req.QuestId);
                SendPacket(EventId.EGS_GIVE_UP_QUEST_ACK, new KEGS_GIVE_UP_QUEST_ACK
                {
                    Ok = removed ? NetError.NET_OK : NetError.ERR_QUEST_02,
                    QuestId = req.QuestId,
                });
                break;
            }

            case EventId.EGS_GET_SKILL_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_GET_SKILL_REQ>(Region);
                bool allOk = true;
                var changed = new Dictionary<int, KGetSkillInfo>();
                foreach (var kv in req.SkillList)
                {
                    int delta = Math.Max(1, kv.Value.SkillLevel - kv.Value.BeforeSkillLevel);
                    for (int i = 0; i < delta; i++)
                    {
                        if (!SkillTree.TryLearnSkill(kv.Key))
                        {
                            allOk = false;
                            break;
                        }
                    }
                    changed[kv.Key] = new KGetSkillInfo
                    {
                        SkillId = kv.Key,
                        BeforeSkillLevel = kv.Value.BeforeSkillLevel,
                        SkillLevel = SkillTree.GetSkillLevel(kv.Key),
                        SpendSkillCsPoint = 0,
                    };
                }
                SendPacket(EventId.EGS_GET_SKILL_ACK, new KEGS_GET_SKILL_ACK
                {
                    Ok = allOk ? NetError.NET_OK : NetError.ERR_SKILL_00,
                    ActiveSkillPageNumber = req.ActiveSkillPageNumber,
                    SkillList = changed,
                    RemainSp = SkillTree.AvailableSkillPoints,
                    RemainCsp = 0,
                });
                break;
            }

            case EventId.EGS_RESET_SKILL_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_RESET_SKILL_REQ>(Region);
                SkillTree.ResetSkillPage(_server.Db, ActiveUnit.UnitUid, ActiveUnit.Level);
                SendPacket(EventId.EGS_RESET_SKILL_ACK, new KEGS_RESET_SKILL_ACK
                {
                    Ok = NetError.NET_OK,
                    ActiveSkillPageNumber = req.ActiveSkillPageNumber,
                    DelSkillId = req.SkillId,
                    SPoint = SkillTree.AvailableSkillPoints,
                    CSPoint = 0,
                });
                break;
            }

            case EventId.EGS_INIT_SKILL_TREE_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_INIT_SKILL_TREE_REQ>(Region);
                SkillTree.ResetSkillPage(_server.Db, ActiveUnit.UnitUid, ActiveUnit.Level);
                SendPacket(EventId.EGS_INIT_SKILL_TREE_ACK, new KEGS_INIT_SKILL_TREE_ACK
                {
                    Ok = NetError.NET_OK,
                    ActiveSkillPageNumber = req.ActiveSkillPageNumber,
                    SPoint = SkillTree.AvailableSkillPoints,
                    CSPoint = 0,
                });
                break;
            }

            case EventId.EGS_REPAIR_ITEM_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_REPAIR_ITEM_REQ>(Region);
                int cost = Math.Max(0, req.ItemUids.Count * 150);
                if (!EdManager.TryAddEd(-cost, KUserEDManager.EdReason.ErOutEdItemRepair, out int nextEd))
                {
                    SendPacket(EventId.EGS_REPAIR_ITEM_ACK, new KEGS_REPAIR_ITEM_ACK
                    {
                        Ok = NetError.ERR_ITEM_04,
                        Ed = ActiveUnit.Ed,
                        APoint = ActiveUnit.APoint,
                    });
                    break;
                }
                _server.Db.RepairItems(ActiveUnit.UnitUid, req.ItemUids);
                _server.Db.UpdateUnitProgress(ActiveUnit.UnitUid, 0, -cost, 0);
                ActiveUnit = _server.Db.GetUnitByUid(Region, ActiveUnit.UnitUid);
                SendPacket(EventId.EGS_REPAIR_ITEM_ACK, new KEGS_REPAIR_ITEM_ACK
                {
                    Ok = NetError.NET_OK,
                    Ed = nextEd,
                    APoint = ActiveUnit?.APoint ?? 0,
                });
                break;
            }

            case EventId.EGS_CREATE_PERSONAL_SHOP_REQ:
            {
                if (ActiveUnit == null) break;
                if (_server.GameSysValTable.BlockPersonalShop)
                {
                    SendPacket(EventId.EGS_CREATE_PERSONAL_SHOP_ACK, new KEGS_CREATE_PERSONAL_SHOP_ACK
                    {
                        Ok = NetError.ERR_BLOCK_TRADE_00,
                    });
                    break;
                }
                if (AbuserManager.IsTradeBlockUser || PunishmentManager.IsAppliedPunishment((int)KPunishmentManager.PunishmentType.PtTrade))
                {
                    SendPacket(EventId.EGS_CREATE_PERSONAL_SHOP_ACK, new KEGS_CREATE_PERSONAL_SHOP_ACK
                    {
                        Ok = NetError.ERR_BLOCK_TRADE_07,
                    });
                    break;
                }
                if (AbuserManager.IsTradeBlockUnit)
                {
                    SendPacket(EventId.EGS_CREATE_PERSONAL_SHOP_ACK, new KEGS_CREATE_PERSONAL_SHOP_ACK
                    {
                        Ok = NetError.ERR_TRADE_BLOCK_UNIT_02,
                    });
                    break;
                }
                SendPacket(EventId.EGS_CREATE_PERSONAL_SHOP_ACK, new KEGS_CREATE_PERSONAL_SHOP_ACK
                {
                    Ok = NetError.NET_OK,
                    PersonalShopUid = ActiveUnit.UnitUid,
                });
                break;
            }

            case EventId.EGS_REG_PERSONAL_SHOP_ITEM_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_REG_PERSONAL_SHOP_ITEM_REQ>(Region);
                var sellItems = new List<KSellPersonalShopItemInfo>();
                var updatedInv = new List<KInventoryItemInfo>();
                foreach (var item in req.RegItems)
                {
                    var (found, slot) = _server.Db.GetUnitItemByUid(ActiveUnit.UnitUid, item.ItemUid);
                    int qty = Math.Max(1, item.Quantity);
                    if (!found || slot.ItemInfo.Quantity < qty) continue;
                    var (_, delSlot) = _server.Db.DeleteInventoryItem(ActiveUnit.UnitUid, item.ItemUid, qty);
                    updatedInv.Add(delSlot);
                    long listingUid = PersonalShopManager.OpenShopAndRegisterItem(
                        _server.Db, Region, ActiveUnit.UnitUid, ActiveUnit.NickName,
                        slot.ItemInfo.ItemId, qty, Math.Max(1, item.PricePerOne));
                    sellItems.Add(new KSellPersonalShopItemInfo
                    {
                        InventoryItemInfo = new KInventoryItemInfo
                        {
                            ItemUid = listingUid,
                            SlotCategory = slot.SlotCategory,
                            SlotId = slot.SlotId,
                            ItemInfo = new KItemInfo { ItemId = slot.ItemInfo.ItemId, Quantity = qty },
                        },
                        PricePerOne = Math.Max(1, item.PricePerOne),
                        FeePerOne = (int)Math.Floor(Math.Max(1, item.PricePerOne) * 0.05),
                        InventoryItemUid = item.ItemUid,
                    });
                }
                SendPacket(EventId.EGS_REG_PERSONAL_SHOP_ITEM_ACK, new KEGS_REG_PERSONAL_SHOP_ITEM_ACK
                {
                    Ok = sellItems.Count > 0 ? NetError.NET_OK : NetError.ERR_PERSONAL_SHOP_01,
                    Ed = ActiveUnit.Ed,
                    PersonalShopName = req.PersonalShopName,
                    SellItemInfo = sellItems,
                    InventorySlotInfo = updatedInv,
                });
                break;
            }

            case EventId.EGS_BREAK_PERSONAL_SHOP_REQ:
            {
                if (ActiveUnit == null) break;
                _ = _server.Db.PickUpPersonalShopAgencyListings(Region, ActiveUnit.UnitUid, []);
                PersonalShopManager.CloseShop();
                SendPacket(EventId.EGS_BREAK_PERSONAL_SHOP_ACK, new KPacketOK { Ok = NetError.NET_OK });
                break;
            }

            case EventId.EGS_CREATE_PET_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_CREATE_PET_REQ>(Region);
                if (!_server.StringFilterManager.CheckIsValidString(KStringFilterManager.FilterType.FtNickName, req.PetName))
                {
                    SendPacket(EventId.EGS_CREATE_PET_ACK, new KEGS_CREATE_PET_ACK { Ok = NetError.ERR_PET_00 });
                    break;
                }
                int petId = 50001;
                var updatedSlots = new List<KInventoryItemInfo>();
                if (req.ItemUid > 0)
                {
                    if (!InventoryLocker.TryLock(out int lockErr))
                    {
                        SendPacket(EventId.EGS_CREATE_PET_ACK, new KEGS_CREATE_PET_ACK { Ok = lockErr });
                        break;
                    }
                    try
                    {
                        var (foundItem, itemSlot) = _server.Db.GetUnitItemByUid(ActiveUnit.UnitUid, req.ItemUid);
                        if (!foundItem || itemSlot.ItemInfo.Quantity <= 0)
                        {
                            SendPacket(EventId.EGS_CREATE_PET_ACK, new KEGS_CREATE_PET_ACK { Ok = NetError.ERR_ITEM_04 });
                            break;
                        }
                        if (!_server.PetManager.TryGetPetByItemId(itemSlot.ItemInfo.ItemId, out petId, out _))
                            petId = itemSlot.ItemInfo.ItemId > 0 ? itemSlot.ItemInfo.ItemId : 50001;
                        var (delOk, delSlot) = _server.Db.DeleteInventoryItem(ActiveUnit.UnitUid, req.ItemUid, 1);
                        if (!delOk)
                        {
                            SendPacket(EventId.EGS_CREATE_PET_ACK, new KEGS_CREATE_PET_ACK { Ok = NetError.ERR_ITEM_04 });
                            break;
                        }
                        ItemQuantityVariation.DecreaseQuantity(itemSlot.ItemInfo.ItemId, 1);
                        updatedSlots.Add(delSlot);
                    }
                    finally { InventoryLocker.Unlock(); }
                }
                var created = _server.Db.CreatePet(ActiveUnit.UnitUid, petId, req.PetName);
                created.EvolutionStep = _server.PetManager.NewPetEvolutionStep;
                created.Satiety = _server.PetManager.NewPetSatiety;
                created.Intimacy = _server.PetManager.NewPetIntimacy;
                created.Extroversion = _server.PetManager.NewPetExtroversion;
                created.Emotion = _server.PetManager.NewPetEmotion;
                _server.Db.UpdatePetStats(created.PetUid, created.Intimacy, created.Satiety, created.EvolutionStep);
                PetManager.AddPet(created);
                SendPacket(EventId.EGS_CREATE_PET_ACK, new KEGS_CREATE_PET_ACK
                {
                    Ok = NetError.NET_OK,
                    CreatedPetInfo = created,
                    InventorySlotInfo = updatedSlots,
                });
                break;
            }

            case EventId.EGS_FEED_PETS_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_FEED_PETS_REQ>(Region);
                if (!InventoryLocker.TryLock(out int lockErr))
                {
                    SendPacket(EventId.EGS_FEED_PETS_ACK, new KEGS_FEED_PETS_ACK { Ok = lockErr });
                    break;
                }
                try
                {
                    var updated = new List<KInventoryItemInfo>();
                    short satietyGain = 600;
                    int intimacyGain = 50;
                    if (req.ItemUid > 0)
                    {
                        var (itemFound, foodSlot) = _server.Db.GetUnitItemByUid(ActiveUnit.UnitUid, req.ItemUid);
                        if (!itemFound || foodSlot.ItemInfo.Quantity <= 0)
                        {
                            SendPacket(EventId.EGS_FEED_PETS_ACK, new KEGS_FEED_PETS_ACK { Ok = NetError.ERR_ITEM_04 });
                            break;
                        }
                        if (_server.PetManager.TryGetCashFeedSatiety(foodSlot.ItemInfo.ItemId, out short cashSat) && cashSat > 0)
                            satietyGain = cashSat;
                        if (_server.PetManager.IsSpecialFeedItemId(foodSlot.ItemInfo.ItemId))
                            intimacyGain = Math.Max(intimacyGain, (int)Math.Round(1000 * _server.PetManager.SpecialFeedIncreaseIntimacyRate) + 50);
                        var (delOk, delSlot) = _server.Db.DeleteInventoryItem(ActiveUnit.UnitUid, req.ItemUid, 1);
                        if (!delOk)
                        {
                            SendPacket(EventId.EGS_FEED_PETS_ACK, new KEGS_FEED_PETS_ACK { Ok = NetError.ERR_ITEM_04 });
                            break;
                        }
                        ItemQuantityVariation.DecreaseQuantity(foodSlot.ItemInfo.ItemId, 1);
                        updated.Add(delSlot);
                    }
                    var (ok, sat, inti) = PetManager.FeedSummonedPet(satietyGain: satietyGain, intimacyGain: intimacyGain);
                    var summoned = PetManager.GetSummonedPet();
                    if (ok && summoned != null)
                        _server.Db.UpdatePetStats(summoned.PetUid, inti, sat, summoned.EvolutionStep);
                    SendPacket(EventId.EGS_FEED_PETS_ACK, new KEGS_FEED_PETS_ACK
                    {
                        Ok = ok ? NetError.NET_OK : NetError.ERR_PET_00,
                        Satiety = sat,
                        InventorySlotInfo = updated,
                    });
                }
                finally { InventoryLocker.Unlock(); }
                break;
            }

            case EventId.EGS_PET_EVOLUTION_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_PET_EVOLUTION_REQ>(Region);
                var (ok, _) = PetManager.EvolvePet(req.PetUid);
                var pet = PetManager.GetPetPage(0).FirstOrDefault(p => p.PetUid == req.PetUid);
                if (ok && pet != null)
                    _server.Db.UpdatePetStats(pet.PetUid, pet.Intimacy, pet.Satiety, pet.EvolutionStep);
                SendPacket(EventId.EGS_PET_EVOLUTION_ACK, new KEGS_PET_EVOLUTION_ACK
                {
                    Ok = ok ? NetError.NET_OK : NetError.ERR_PET_00,
                    EvolutionResultPetInfo = pet ?? new KPetInfo(),
                });
                break;
            }

            case EventId.EGS_AUTO_PARTY_DUNGEON_GAME_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_AUTO_PARTY_DUNGEON_GAME_REQ>(Region);
                _server.Global.AutoPartyManager.Enqueue(new AutoPartyEntry(
                    ActiveUnit.UnitUid, ActiveUnit.NickName, req.DungeonId, req.DifficultyLevel, DateTime.UtcNow, req.DungeonMode));
                SendPacket(EventId.EGS_AUTO_PARTY_DUNGEON_GAME_ACK, new KEGS_AUTO_PARTY_DUNGEON_GAME_ACK
                {
                    Ok = NetError.NET_OK,
                });
                while (_server.Global.AutoPartyManager.TryDequeueGroup(out var group) && group != null)
                {
                    var foundNot = new KEGS_AUTO_PARTY_MAKING_SUCCESS_NOT
                    {
                        AutoPartyUid = group.AutoPartyUid,
                        MemberCount = group.Members.Count,
                        DungeonId = group.DungeonId,
                        DifficultyLevel = group.DifficultyLevel,
                        DungeonMode = group.DungeonMode,
                    };
                    foreach (var m in group.Members)
                        _server.SendToUnit(m.UnitUid, EventId.EGS_AUTO_PARTY_MAKING_SUCCESS_NOT, foundNot);
                }
                break;
            }

            case EventId.EGS_CANCEL_AUTO_PARTY_MAKING_REQ:
            {
                if (ActiveUnit == null) break;
                _server.Global.AutoPartyManager.Cancel(ActiveUnit.UnitUid);
                SendPacket(EventId.EGS_CANCEL_AUTO_PARTY_MAKING_ACK, new KPacketOK { Ok = NetError.NET_OK });
                break;
            }

            case EventId.EGS_AUTO_PARTY_MAKING_SUCCESS_REPLY_NOT:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_AUTO_PARTY_MAKING_SUCCESS_REPLY_NOT>(Region);
                if (!_server.Global.AutoPartyManager.SubmitReply(
                    ActiveUnit.UnitUid, req.GameStartAgree, out var readyGroup, out var cancelledGroup))
                {
                    if (!req.GameStartAgree)
                        SendPacket(EventId.EGS_AUTO_PARTY_CLOSE_NOT, new KEGS_AUTO_PARTY_CLOSE_NOT
                        {
                            Reason = NetError.ERR_MATCH_MAKING_08,
                            CancelUnitUid = ActiveUnit.UnitUid,
                        });
                    break;
                }

                if (cancelledGroup != null)
                {
                    var closeNot = new KEGS_AUTO_PARTY_CLOSE_NOT
                    {
                        Reason = NetError.ERR_MATCH_MAKING_08,
                        CancelUnitUid = ActiveUnit.UnitUid,
                    };
                    foreach (var m in cancelledGroup.Members)
                        _server.SendToUnit(m.UnitUid, EventId.EGS_AUTO_PARTY_CLOSE_NOT, closeNot);
                    break;
                }

                if (readyGroup != null)
                {
                    var leader = _server.GetUserByUnitUid(readyGroup.Members[0].UnitUid);
                    if (leader?.ActiveUnit == null) break;

                    var party = _server.Center.PartyManager.CreateParty(new KCreatePartyInfo
                    {
                        PartyType = 0,
                        DungeonId = readyGroup.DungeonId,
                        DifficultyLevel = readyGroup.DifficultyLevel,
                        DungeonMode = readyGroup.DungeonMode,
                    }, leader.BuildPartyUserInfo());

                    foreach (var m in readyGroup.Members.Skip(1))
                    {
                        var u = _server.GetUserByUnitUid(m.UnitUid);
                        if (u?.ActiveUnit != null)
                            _server.Center.PartyManager.TryJoinParty(party.Info.PartyUid, u.BuildPartyUserInfo(), out _);
                    }

                    var regroupNot = new KEGS_REGROUP_PARTY_NOT
                    {
                        PartyInfo = party.Info,
                        PartyUserInfo = party.SnapshotMembers(),
                    };

                    var room = _server.Center.RoomManager.CreateRoom(new KRoomInfo
                    {
                        RoomType = 2, // RT_DUNGEON
                        RoomName = $"AutoParty #{readyGroup.AutoPartyUid}",
                        Public = false,
                        MaxSlot = (sbyte)Math.Clamp(readyGroup.Members.Count, 1, 4),
                        StartedByAutoParty = true,
                        DungeonId = readyGroup.DungeonId,
                        DifficultyLevel = readyGroup.DifficultyLevel,
                        DungeonMode = readyGroup.DungeonMode,
                        PartyUid = party.Info.PartyUid,
                    });

                    var joinedUsers = new List<(KGSUser user, KRoomSlotInfo slot)>();
                    foreach (var m in readyGroup.Members)
                    {
                        var u = _server.GetUserByUnitUid(m.UnitUid);
                        if (u?.ActiveUnit == null) continue;
                        u.CurrentPartyUid = party.Info.PartyUid;
                        u.LeaveCurrentField();
                        var (ok, slot) = room.TryAddUser(u.BuildRoomUserInfo(), asObserver: false, password: string.Empty);
                        if (ok == NetError.NET_OK && slot != null)
                        {
                            slot.IsReady = true;
                            u.CurrentRoomUid = room.Info.RoomUid;
                            u.Fsm.TryStateTransition(KGSFSM.I_TO_ROOM, out _);
                            joinedUsers.Add((u, slot));
                        }
                    }

                    room.StartGame();
                    foreach (var (u, slot) in joinedUsers)
                    {
                        u.SendPacket(EventId.EGS_REGROUP_PARTY_NOT, regroupNot);
                        u.SendPacket(EventId.EGS_AUTO_PARTY_CLOSE_NOT, new KEGS_AUTO_PARTY_CLOSE_NOT
                        {
                            Reason = NetError.NET_OK,
                            CancelUnitUid = 0,
                        });
                        if (u.ActiveUnit!.UnitUid == leader.ActiveUnit.UnitUid)
                        {
                            u.SendPacket(EventId.EGS_CREATE_ROOM_ACK, new KEGS_CREATE_ROOM_ACK
                            {
                                Ok = NetError.NET_OK,
                                RoomInfo = room.Info,
                                Slots = room.Slots.ToList(),
                                ObserverSlots = room.ObserverSlots.ToList(),
                            });
                        }
                        else
                        {
                            u.SendPacket(EventId.EGS_JOIN_ROOM_NOT, new KEGS_JOIN_ROOM_NOT
                            {
                                JoinSlot = slot,
                                StartPosIndex = slot.Index,
                            });
                        }
                    }
                }
                break;
            }

            // -------- Manufacture / Cash Shop / Guild Board / Couple Relationship --------

            case EventId.EGS_ITEM_MANUFACTURE_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_ITEM_MANUFACTURE_REQ>(Region);
                if (!InventoryLocker.Lock(KInventoryLocker.LockType.TimeOut, 30.0))
                {
                    SendPacket(EventId.EGS_ITEM_MANUFACTURE_ACK, new KEGS_ITEM_MANUFACTURE_ACK { Ok = NetError.ERR_INVENTORY_LOCK_00, Ed = ActiveUnit.Ed });
                    break;
                }
                try
                {
                    if (IsTradeOrShopOpen())
                    {
                        SendPacket(EventId.EGS_ITEM_MANUFACTURE_ACK, new KEGS_ITEM_MANUFACTURE_ACK { Ok = NetError.ERR_ITEM_14, Ed = ActiveUnit.Ed });
                        break;
                    }
                    int qty = Math.Max(1, req.Quantity);
                    var recipe = _server.ManufactureItemManager.GetManufactureData(req.ManufactureId);
                    int costEd = recipe != null ? recipe.Cost * qty : qty * 500;
                    if (ActiveUnit.Ed < costEd)
                    {
                        SendPacket(EventId.EGS_ITEM_MANUFACTURE_ACK, new KEGS_ITEM_MANUFACTURE_ACK { Ok = NetError.ERR_MANUFACTURE_05, Ed = ActiveUnit.Ed });
                        break;
                    }

                    if (recipe != null && recipe.Materials.Count > 0)
                    {
                        foreach (var mat in recipe.Materials)
                        {
                            if (_server.Db.GetTotalItemCountByItemId(ActiveUnit.UnitUid, mat.MaterialItemId) < mat.MaterialCount * qty)
                            {
                                SendPacket(EventId.EGS_ITEM_MANUFACTURE_ACK, new KEGS_ITEM_MANUFACTURE_ACK { Ok = NetError.ERR_MANUFACTURE_03, Ed = ActiveUnit.Ed });
                                goto ManufactureDone;
                            }
                        }
                    }

                    if (!EdManager.TryAddEd(-costEd, KUserEDManager.EdReason.ErOutEdItemManufacture, out int nextEd))
                    {
                        SendPacket(EventId.EGS_ITEM_MANUFACTURE_ACK, new KEGS_ITEM_MANUFACTURE_ACK { Ok = NetError.ERR_MANUFACTURE_05, Ed = ActiveUnit.Ed });
                        break;
                    }

                    var slotList = new List<KInventoryItemInfo>();
                    if (recipe != null && recipe.Materials.Count > 0)
                    {
                        foreach (var mat in recipe.Materials)
                        {
                            var (_, consumedSlots) = _server.Db.TryConsumeItemsByItemId(ActiveUnit.UnitUid, mat.MaterialItemId, mat.MaterialCount * qty);
                            ItemQuantityVariation.DecreaseQuantity(mat.MaterialItemId, mat.MaterialCount * qty);
                            slotList.AddRange(consumedSlots);
                        }
                    }

                    _server.Db.UpdateUnitProgress(ActiveUnit.UnitUid, 0, -costEd, 0);
                    int rolledId = recipe != null ? _server.ManufactureItemManager.RollResultItemId(req.ManufactureId) : 0;
                    int craftedItemId = rolledId > 0 ? rolledId : 100000 + Math.Max(1, req.ManufactureId);
                    var granted = _server.Db.GrantUnitItem(ActiveUnit.UnitUid, craftedItemId, qty);
                    if (_server.ManufactureItemManager.IsItemSealed(craftedItemId))
                        granted = _server.Db.SetItemSealed(ActiveUnit.UnitUid, granted.ItemUid, true);
                    if (_server.Center.DefenceDungeonManager.IsManufactureDefenceDungeonItem(craftedItemId))
                    {
                        var randomSockets = new List<int>();
                        if (_server.Center.DefenceDungeonManager.InsertSocketOptionForDefenceDungeonItem(craftedItemId, randomSockets)
                            && randomSockets.Count > 0)
                        {
                            granted = _server.Db.UpdateItemRandomSockets(ActiveUnit.UnitUid, granted.ItemUid, randomSockets);
                        }
                    }
                    slotList.Add(granted);

                    ItemQuantityVariation.IncreaseQuantity(craftedItemId, qty);
                    ActiveUnit = _server.Db.GetUnitByUid(Region, ActiveUnit.UnitUid);
                    _server.StatisticsManager.AddItemStat(KStatisticsManager.StatInventory.SiItemMake, craftedItemId, qty);

                    SendPacket(EventId.EGS_ITEM_MANUFACTURE_ACK, new KEGS_ITEM_MANUFACTURE_ACK
                    {
                        Ok = NetError.NET_OK,
                        InsertedItem = new Dictionary<int, int> { [craftedItemId] = qty },
                        Ed = nextEd,
                        InventorySlotInfo = slotList,
                    });
                ManufactureDone:;
                }
                finally
                {
                    InventoryLocker.Unlock();
                }
                break;
            }

            case EventId.EGS_CHECK_BALANCE_REQ:
            {
                if (AccountUid <= 0) break;
                uint bal = _server.GetAccountCash(AccountUid);
                CashInventory.GlobalCashInfo.Cash[KGlobalCashInfo.GctPublisherCash] = bal;
                CashInventory.GlobalCashInfo.Cash[KGlobalCashInfo.GctKogElswordCash] = bal;
                SendPacket(EventId.EGS_CHECK_BALANCE_ACK, new KEGS_CHECK_BALANCE_ACK
                {
                    Ok = NetError.NET_OK,
                    Balance = bal,
                    OnlyType = false,
                    CashType = KGlobalCashInfo.GctPublisherCash,
                    GlobalCashInfo = CashInventory.GlobalCashInfo,
                });
                break;
            }

            case EventId.EGS_BUY_CASH_ITEM_REQ:
            {
                if (ActiveUnit == null) break;
                if (!_server.ContentManager.IsEnableCashShop)
                {
                    SendPacket(EventId.EGS_BUY_CASH_ITEM_ACK, new KEGS_BUY_CASH_ITEM_ACK { Ok = NetError.ERR_BUY_CASH_ITEM_01 });
                    break;
                }
                var req = ev.GetPayload<KEGS_BUY_CASH_ITEM_REQ>(Region);
                if (req.PurchaseReqInfo.Count == 0)
                {
                    SendPacket(EventId.EGS_BUY_CASH_ITEM_ACK, new KEGS_BUY_CASH_ITEM_ACK { Ok = NetError.ERR_BUY_CASH_ITEM_01 });
                    break;
                }
                uint totalCost = (uint)req.PurchaseReqInfo.Sum(p => (long)Math.Max(1, (int)p.OrderQuantity) * 100L);
                if (!_server.TrySpendAccountCash(AccountUid, totalCost, out uint remaining))
                {
                    SendPacket(EventId.EGS_BUY_CASH_ITEM_ACK, new KEGS_BUY_CASH_ITEM_ACK { Ok = NetError.ERR_BUY_CASH_ITEM_08 });
                    break;
                }
                CashInventory.GlobalCashInfo.Cash[KGlobalCashInfo.GctPublisherCash] = remaining;
                foreach (var p in req.PurchaseReqInfo)
                {
                    int itemId = (int)p.ProductNo;
                    int qty = Math.Max(1, (int)p.OrderQuantity);
                    _server.Db.AddBillingOrder(AccountUid, (int)p.ProductNo, itemId, $"CashItem_{itemId}", qty, 100 * qty);
                    if (_server.DailyGiftBoxManager.IsDailyGiftBoxItemId(itemId))
                    {
                        string regDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                        _server.Db.UpsertDailyGiftBoxPurchase(AccountUid, ActiveUnit.UnitUid, itemId, (int)KDailyGiftBoxManager.PurchasedState.PurchasedBuy, regDate);
                        if (!_dailyGiftBoxList.TryGetValue(itemId, out var dgbEntries))
                        {
                            dgbEntries = [];
                            _dailyGiftBoxList[itemId] = dgbEntries;
                        }
                        int idx = dgbEntries.FindIndex(e => e.UnitUid == ActiveUnit.UnitUid);
                        var entry = (ActiveUnit.UnitUid, (int)KDailyGiftBoxManager.PurchasedState.PurchasedBuy, regDate);
                        if (idx >= 0) dgbEntries[idx] = entry;
                        else dgbEntries.Add(entry);
                    }
                    var granted = _server.Db.GrantUnitItem(ActiveUnit.UnitUid, itemId, qty);
                    CashInventory.AddPickUpPackageInfo(p.ProductNo, [granted]);
                    _server.StatisticsManager.AddItemStat(KStatisticsManager.StatInventory.SiItemMall, itemId, p.OrderQuantity);
                }
                SendPacket(EventId.EGS_BUY_CASH_ITEM_ACK, new KEGS_BUY_CASH_ITEM_ACK
                {
                    Ok = NetError.NET_OK,
                    PurchaseReqInfo = req.PurchaseReqInfo,
                    IsWishList = req.IsWishList,
                });
                break;
            }

            case EventId.EGS_GET_GUILD_AD_LIST_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_GET_GUILD_AD_LIST_REQ>(Region);
                var (ads, totalPages) = _server.GuildAdListProvider != null
                    ? _server.GuildAdListProvider(req.SortType, (int)req.ViewPage)
                    : ([], 1);
                SendPacket(EventId.EGS_GET_GUILD_AD_LIST_ACK, new KEGS_GET_GUILD_AD_LIST_ACK
                {
                    SortType = req.SortType,
                    TotalPage = (uint)Math.Max(1, totalPages),
                    ViewPage = req.ViewPage,
                    GuildAdList = ads,
                    CanShowApplyList = ActiveUnit.UserGuildInfo.GuildUid > 0,
                    RegMyGuildAd = ActiveUnit.UserGuildInfo.GuildUid > 0 &&
                        ads.Any(a => a.GuildUid == ActiveUnit.UserGuildInfo.GuildUid),
                });
                break;
            }

            case EventId.EGS_COUPLE_PROPOSE_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_COUPLE_PROPOSE_REQ>(Region);
                if (!RelationshipManager.IsSolo)
                {
                    SendPacket(EventId.EGS_COUPLE_PROPOSE_ACK, new KEGS_COUPLE_PROPOSE_ACK { Ok = NetError.ERR_RELATIONSHIP_00, NickName = req.UnitName });
                    break;
                }
                var target = _server.GetUserByNickName(req.UnitName);
                if (target?.ActiveUnit == null || !target.RelationshipManager.IsSolo)
                {
                    SendPacket(EventId.EGS_COUPLE_PROPOSE_ACK, new KEGS_COUPLE_PROPOSE_ACK { Ok = NetError.ERR_RELATIONSHIP_00, NickName = req.UnitName });
                    break;
                }
                int charErr = KUserRelationshipManager.CheckChar(ActiveUnit.UnitClass, target.ActiveUnit.UnitClass);
                if (charErr != NetError.NET_OK)
                {
                    SendPacket(EventId.EGS_COUPLE_PROPOSE_ACK, new KEGS_COUPLE_PROPOSE_ACK { Ok = charErr, NickName = req.UnitName });
                    break;
                }
                SendPacket(EventId.EGS_COUPLE_PROPOSE_ACK, new KEGS_COUPLE_PROPOSE_ACK { Ok = NetError.NET_OK, NickName = target.ActiveUnit.NickName });
                target.SendPacket(EventId.EGS_COUPLE_PROPOSE_NOT, new KEGS_COUPLE_PROPOSE_NOT
                {
                    RequestUnitUid = ActiveUnit.UnitUid,
                    RequestUnitName = ActiveUnit.NickName,
                });
                break;
            }

            case EventId.EGS_COUPLE_PROPOSE_AGREE_NOT:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_COUPLE_PROPOSE_AGREE_NOT>(Region);
                var requester = _server.GetUserByUnitUid(req.RequestUnitUid);
                if (requester?.ActiveUnit == null) break;

                long nowUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                if (req.Answer == 0)
                {
                    RelationshipManager.SetRelationshipType(KUserRelationshipManager.RelationshipType.RtCouple);
                    RelationshipManager.LoverUnitUid = requester.ActiveUnit.UnitUid;
                    RelationshipManager.LoverNickName = requester.ActiveUnit.NickName;
                    RelationshipManager.AnniversaryDayUtc = DateTime.UtcNow;

                    requester.RelationshipManager.SetRelationshipType(KUserRelationshipManager.RelationshipType.RtCouple);
                    requester.RelationshipManager.LoverUnitUid = ActiveUnit.UnitUid;
                    requester.RelationshipManager.LoverNickName = ActiveUnit.NickName;
                    requester.RelationshipManager.AnniversaryDayUtc = DateTime.UtcNow;

                    _server.Db.SaveRelationship(ActiveUnit.UnitUid, (sbyte)KUserRelationshipManager.RelationshipType.RtCouple,
                        requester.ActiveUnit.UnitUid, requester.ActiveUnit.NickName, string.Empty,
                        RelationshipManager.AnniversaryDayUtc, null, 0);
                    _server.Db.SaveRelationship(requester.ActiveUnit.UnitUid, (sbyte)KUserRelationshipManager.RelationshipType.RtCouple,
                        ActiveUnit.UnitUid, ActiveUnit.NickName, string.Empty,
                        requester.RelationshipManager.AnniversaryDayUtc, null, 0);
                }
                SendPacket(EventId.EGS_COUPLE_PROPOSE_RESULT_NOT, new KEGS_COUPLE_PROPOSE_RESULT_NOT
                {
                    Ok = NetError.NET_OK,
                    Result = req.Answer,
                    UnitUid = requester.ActiveUnit.UnitUid,
                    UnitName = requester.ActiveUnit.NickName,
                    DateUnix = nowUnix,
                });
                requester.SendPacket(EventId.EGS_COUPLE_PROPOSE_RESULT_NOT, new KEGS_COUPLE_PROPOSE_RESULT_NOT
                {
                    Ok = NetError.NET_OK,
                    Result = req.Answer,
                    UnitUid = ActiveUnit.UnitUid,
                    UnitName = ActiveUnit.NickName,
                    DateUnix = nowUnix,
                });
                break;
            }

            case EventId.EGS_BREAK_UP_REQ:
            {
                if (ActiveUnit == null) break;
                if (RelationshipManager.IsSolo)
                {
                    SendPacket(EventId.EGS_BREAK_UP_ACK, new KPacketOK { Ok = NetError.ERR_RELATIONSHIP_00 });
                    break;
                }
                int cost = RelationshipManager.GetBreakUpCost();
                if (!EdManager.TryAddEd(-cost, KUserEDManager.EdReason.ErOutEdBreakUpRelationship, out _))
                {
                    SendPacket(EventId.EGS_BREAK_UP_ACK, new KPacketOK { Ok = NetError.ERR_RELATIONSHIP_00 });
                    break;
                }
                _server.Db.UpdateUnitProgress(ActiveUnit.UnitUid, 0, -cost, 0);
                long exUid = RelationshipManager.LoverUnitUid;
                RelationshipManager.SetBreakUp();
                _server.Db.SaveRelationship(ActiveUnit.UnitUid, (sbyte)KUserRelationshipManager.RelationshipType.RtSolo,
                    0, string.Empty, string.Empty, null, null, 0);
                var exUser = _server.GetUserByUnitUid(exUid);
                exUser?.RelationshipManager.SetBreakUp();
                if (exUid > 0)
                {
                    _server.Db.SaveRelationship(exUid, (sbyte)KUserRelationshipManager.RelationshipType.RtSolo,
                        0, string.Empty, string.Empty, null, null, 0);
                }
                exUser?.SendPacket(EventId.EGS_BREAK_UP_NOT, new KEGS_BREAK_UP_NOT { Cost = cost });
                SendPacket(EventId.EGS_BREAK_UP_ACK, new KPacketOK { Ok = NetError.NET_OK });
                break;
            }

            // -------- Community / Friends / BlackList / Inventory Delete --------

            case EventId.EGS_COMMUNITY_USER_LIST_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_COMMUNITY_USER_LIST_REQ>(Region);
                const int pageSize = 8;
                var all = _server.GetActiveUsersSnapshot()
                    .Select(u => new KCommunityUserInfo
                    {
                        UnitUid = u.ActiveUnit!.UnitUid,
                        NickName = u.ActiveUnit.NickName,
                        UnitClass = u.ActiveUnit.UnitClass,
                        Level = u.ActiveUnit.Level,
                        State = u.CurrentRoomUid > 0 ? (sbyte)4 : (sbyte)1,
                        StateCode = u.CurrentPartyUid,
                        RoomUid = u.CurrentRoomUid,
                        Ed = u.ActiveUnit.Ed,
                    })
                    .ToList();
                uint totalPages = (uint)Math.Max(1, (all.Count + pageSize - 1) / pageSize);
                uint page = Math.Clamp(req.ViewPage, 1u, totalPages);
                var slice = all.Skip((int)((page - 1) * pageSize)).Take(pageSize).ToList();
                SendPacket(EventId.EGS_COMMUNITY_USER_LIST_ACK, new KEGS_COMMUNITY_USER_LIST_ACK
                {
                    Ok = NetError.NET_OK,
                    TotalPage = totalPages,
                    ViewPage = page,
                    UserList = slice,
                });
                break;
            }

            case EventId.EGS_UPDATE_COMMUNITY_OPTION_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_UPDATE_COMMUNITY_OPTION_REQ>(Region);
                if (!BlackList.IsUpdateOptPossible())
                {
                    SendPacket(EventId.EGS_UPDATE_COMMUNITY_OPTION_ACK, new KEGS_UPDATE_COMMUNITY_OPTION_ACK
                    {
                        Ok = NetError.ERR_BLACKLIST_02,
                        DenyOptions = DenyOptions,
                    });
                    break;
                }
                DenyOptions = req.DenyOptions;
                BlackList.SetInsertFriendDeny((KBlackList.BlackListDenyState)req.DenyOptions.DenyFriendShip);
                BlackList.PartyDeny = (KBlackList.BlackListDenyState)req.DenyOptions.DenyParty;
                BlackList.PrivateDealDeny = (KBlackList.BlackListDenyState)req.DenyOptions.DenyPersonalTrade;
                SendPacket(EventId.EGS_UPDATE_COMMUNITY_OPTION_ACK, new KEGS_UPDATE_COMMUNITY_OPTION_ACK
                {
                    Ok = NetError.NET_OK,
                    DenyOptions = DenyOptions,
                });
                break;
            }

            case EventId.EGS_NEW_BLACKLIST_USER_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_NEW_BLACKLIST_USER_REQ>(Region);
                long? targetUid = _server.Db.FindUnitUidByNickName(Region, req.NickName);
                if (targetUid == null || targetUid.Value == ActiveUnit.UnitUid ||
                    !BlackList.IsAddPossible() || !BlackList.AddNewBlackListUser(targetUid.Value, req.NickName))
                {
                    SendPacket(EventId.EGS_NEW_BLACKLIST_USER_ACK, new KEGS_NEW_BLACKLIST_USER_ACK
                    {
                        Ok = NetError.ERR_BLACKLIST_00,
                    });
                    break;
                }
                _server.Db.AddBlackListUser(ActiveUnit.UnitUid, targetUid.Value, req.NickName);
                SendPacket(EventId.EGS_NEW_BLACKLIST_USER_ACK, new KEGS_NEW_BLACKLIST_USER_ACK
                {
                    Ok = NetError.NET_OK,
                    ChatBlackListUnit = new KChatBlackListUnit { UnitUid = targetUid.Value, NickName = req.NickName },
                });
                break;
            }

            case EventId.EGS_DEL_BLACKLIST_USER_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_DEL_BLACKLIST_USER_REQ>(Region);
                var snap = BlackList.GetBlackListSnapshot();
                if (!snap.TryGetValue(req.UnitUid, out string? nick) ||
                    !BlackList.DeleteBlackListUser(nick, out long removedUid))
                {
                    SendPacket(EventId.EGS_DEL_BLACKLIST_USER_ACK, new KEGS_NEW_BLACKLIST_USER_ACK
                    {
                        Ok = NetError.ERR_BLACKLIST_01,
                    });
                    break;
                }
                _server.Db.RemoveBlackListUser(ActiveUnit.UnitUid, removedUid);
                SendPacket(EventId.EGS_DEL_BLACKLIST_USER_ACK, new KEGS_NEW_BLACKLIST_USER_ACK
                {
                    Ok = NetError.NET_OK,
                    ChatBlackListUnit = new KChatBlackListUnit { UnitUid = removedUid, NickName = nick },
                });
                break;
            }

            case EventId.EGS_REQUEST_FRIEND_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_REQUEST_FRIEND_REQ>(Region);
                var target = _server.GetUserByNickName(req.NickName);
                if (target?.ActiveUnit == null || target.ActiveUnit.UnitUid == ActiveUnit.UnitUid ||
                    _friendUids.Contains(target.ActiveUnit.UnitUid) ||
                    target.BlackList.InsertFriendDeny == KBlackList.BlackListDenyState.BlOn ||
                    target.BlackList.VerifyBlackListUser(ActiveUnit.UnitUid))
                {
                    SendPacket(EventId.EGS_REQUEST_FRIEND_ACK, new KEGS_REQUEST_FRIEND_ACK
                    {
                        Ok = NetError.ERR_MESSENGER_03,
                        NickName = req.NickName,
                    });
                    break;
                }
                target._pendingFriendRequests.Add(ActiveUnit.UnitUid);
                _friends[target.ActiveUnit.UnitUid] = new KFriendInfo
                {
                    UnitUid = target.ActiveUnit.UnitUid,
                    NickName = target.ActiveUnit.NickName,
                    GroupId = 0,
                    Status = KFriendInfo.FsRequested,
                    Position = KFriendInfo.FpOffLine,
                };
                target._friends[ActiveUnit.UnitUid] = new KFriendInfo
                {
                    UnitUid = ActiveUnit.UnitUid,
                    NickName = ActiveUnit.NickName,
                    GroupId = 0,
                    Status = KFriendInfo.FsWaiting,
                    Position = KFriendInfo.FpOffLine,
                };
                SendPacket(EventId.EGS_REQUEST_FRIEND_ACK, new KEGS_REQUEST_FRIEND_ACK
                {
                    Ok = NetError.NET_OK,
                    UnitUid = target.ActiveUnit.UnitUid,
                    NickName = target.ActiveUnit.NickName,
                });
                target.SendPacket(EventId.EGS_REQUEST_FRIEND_NOT, new KEGS_REQUEST_FRIEND_NOT
                {
                    UnitUid = ActiveUnit.UnitUid,
                    NickName = ActiveUnit.NickName,
                    Message = req.Message,
                });
                break;
            }

            case EventId.EGS_ACCEPT_FRIEND_REQ:
            {
                if (ActiveUnit == null) break;
                var ks = new KSerializer(Region);
                ks.BeginReading(ev.Buffer);
                ks.Get(out long requesterUid);
                ks.EndReading();
                if (!_pendingFriendRequests.Remove(requesterUid))
                {
                    SendPacket(EventId.EGS_ACCEPT_FRIEND_ACK, new KEGS_FRIEND_UNIT_ACK { Ok = NetError.ERR_MESSENGER_06, UnitUid = requesterUid });
                    break;
                }
                _friendUids.Add(requesterUid);
                _server.Db.AddFriendship(ActiveUnit.UnitUid, requesterUid);
                var requester = _server.GetUserByUnitUid(requesterUid);
                if (_friends.TryGetValue(requesterUid, out var fInfo))
                {
                    fInfo.Status = KFriendInfo.FsNormal;
                    fInfo.Position = requester != null ? KFriendInfo.FpOnLine : KFriendInfo.FpOffLine;
                }
                else
                {
                    _friends[requesterUid] = new KFriendInfo
                    {
                        UnitUid = requesterUid,
                        NickName = requester?.ActiveUnit?.NickName ?? string.Empty,
                        Status = KFriendInfo.FsNormal,
                        Position = requester != null ? KFriendInfo.FpOnLine : KFriendInfo.FpOffLine,
                    };
                }
                if (requester != null)
                {
                    requester._friendUids.Add(ActiveUnit.UnitUid);
                    if (requester._friends.TryGetValue(ActiveUnit.UnitUid, out var rf))
                    {
                        rf.Status = KFriendInfo.FsNormal;
                        rf.Position = KFriendInfo.FpOnLine;
                    }
                    else
                    {
                        requester._friends[ActiveUnit.UnitUid] = new KFriendInfo
                        {
                            UnitUid = ActiveUnit.UnitUid,
                            NickName = ActiveUnit.NickName,
                            Status = KFriendInfo.FsNormal,
                            Position = KFriendInfo.FpOnLine,
                        };
                    }
                }
                SendPacket(EventId.EGS_ACCEPT_FRIEND_ACK, new KEGS_FRIEND_UNIT_ACK { Ok = NetError.NET_OK, UnitUid = requesterUid });
                if (requester != null)
                    requester.SendPacket(EventId.EGS_ACCEPT_FRIEND_NOT, new KEGS_ACCEPT_FRIEND_NOT { UnitUid = ActiveUnit.UnitUid });
                break;
            }

            case EventId.EGS_DENY_FRIEND_REQ:
            {
                if (ActiveUnit == null) break;
                var ks = new KSerializer(Region);
                ks.BeginReading(ev.Buffer);
                ks.Get(out long requesterUid);
                ks.EndReading();
                _pendingFriendRequests.Remove(requesterUid);
                _friends.Remove(requesterUid);
                var requester = _server.GetUserByUnitUid(requesterUid);
                requester?._friends.Remove(ActiveUnit.UnitUid);
                SendPacket(EventId.EGS_DENY_FRIEND_ACK, new KEGS_FRIEND_UNIT_ACK { Ok = NetError.NET_OK, UnitUid = requesterUid });
                _server.SendToUnit(requesterUid, EventId.EGS_DENY_FRIEND_NOT, new KEGS_DENY_FRIEND_NOT
                {
                    UnitUid = ActiveUnit.UnitUid,
                    NickName = ActiveUnit.NickName,
                });
                break;
            }

            case EventId.EGS_DELETE_FRIEND_REQ:
            {
                if (ActiveUnit == null) break;
                var ks = new KSerializer(Region);
                ks.BeginReading(ev.Buffer);
                ks.Get(out long targetUid);
                ks.EndReading();
                _friendUids.Remove(targetUid);
                _blockedFriendUids.Remove(targetUid);
                _friends.Remove(targetUid);
                _server.Db.RemoveFriendship(ActiveUnit.UnitUid, targetUid);
                SendPacket(EventId.EGS_DELETE_FRIEND_ACK, new KEGS_FRIEND_UNIT_ACK { Ok = NetError.NET_OK, UnitUid = targetUid });
                var other = _server.GetUserByUnitUid(targetUid);
                if (other != null)
                {
                    other._friendUids.Remove(ActiveUnit.UnitUid);
                    other._friends.Remove(ActiveUnit.UnitUid);
                    other.SendPacket(EventId.EGS_DELETE_FRIEND_NOT, new KEGS_DELETE_FRIEND_NOT { UnitUid = ActiveUnit.UnitUid });
                }
                break;
            }

            case EventId.EGS_BLOCK_FRIEND_REQ:
            {
                if (ActiveUnit == null) break;
                var ks = new KSerializer(Region);
                ks.BeginReading(ev.Buffer);
                ks.Get(out long targetUid);
                ks.EndReading();
                _blockedFriendUids.Add(targetUid);
                if (_friends.TryGetValue(targetUid, out var fInfo))
                    fInfo.Status = KFriendInfo.FsBlocked;
                _server.Db.SetFriendshipBlock(ActiveUnit.UnitUid, targetUid, true);
                SendPacket(EventId.EGS_BLOCK_FRIEND_ACK, new KEGS_FRIEND_UNIT_ACK { Ok = NetError.NET_OK, UnitUid = targetUid });
                var other = _server.GetUserByUnitUid(targetUid);
                other?.SendPacket(EventId.EGS_BLOCK_FRIEND_NOT, new KEGS_BLOCK_FRIEND_NOT { UnitUid = ActiveUnit.UnitUid });
                break;
            }

            case EventId.EGS_UNBLOCK_FRIEND_REQ:
            {
                if (ActiveUnit == null) break;
                var ks = new KSerializer(Region);
                ks.BeginReading(ev.Buffer);
                ks.Get(out long targetUid);
                ks.EndReading();
                _blockedFriendUids.Remove(targetUid);
                if (_friends.TryGetValue(targetUid, out var fInfo))
                    fInfo.Status = KFriendInfo.FsNormal;
                _server.Db.SetFriendshipBlock(ActiveUnit.UnitUid, targetUid, false);
                SendPacket(EventId.EGS_UNBLOCK_FRIEND_ACK, new KEGS_FRIEND_UNIT_ACK { Ok = NetError.NET_OK, UnitUid = targetUid });
                var other = _server.GetUserByUnitUid(targetUid);
                other?.SendPacket(EventId.EGS_UNBLOCK_FRIEND_NOT, new KEGS_UNBLOCK_FRIEND_NOT { UnitUid = ActiveUnit.UnitUid });
                break;
            }

            case EventId.EGS_DELETE_ITEM_REQ:
            {
                if (ActiveUnit == null) break;
                if (InventoryLocker.IsLocked())
                {
                    SendPacket(EventId.EGS_DELETE_ITEM_ACK, new KEGS_DELETE_ITEM_ACK { Ok = NetError.ERR_INVENTORY_LOCK_00 });
                    break;
                }
                var req = ev.GetPayload<KEGS_DELETE_ITEM_REQ>(Region);
                var (itemFound, itemSlot) = _server.Db.GetUnitItemByUid(ActiveUnit.UnitUid, req.ItemUid);
                if (itemFound && AuthLevel < 3 && itemSlot.ItemInfo.Period > 0 && !_server.DeleteItemIds.Contains(itemSlot.ItemInfo.ItemId))
                {
                    SendPacket(EventId.EGS_DELETE_ITEM_ACK, new KEGS_DELETE_ITEM_ACK { Ok = NetError.ERR_ITEM_16 });
                    break;
                }
                var (ok, slotInfo) = _server.Db.DeleteInventoryItem(ActiveUnit.UnitUid, req.ItemUid, req.Quantity);
                if (ok && itemFound)
                    ItemQuantityVariation.DecreaseQuantity(itemSlot.ItemInfo.ItemId, Math.Max(1, req.Quantity));
                SendPacket(EventId.EGS_DELETE_ITEM_ACK, new KEGS_DELETE_ITEM_ACK
                {
                    Ok = ok ? NetError.NET_OK : NetError.ERR_ITEM_04,
                    InventoryItemInfo = slotInfo,
                });
                break;
            }

            // -------- Dungeon Stage / NPC Spawn & Kill / Loot / Resurrect --------

            case EventId.EGS_DUNGEON_STAGE_LOAD_REQ:
            {
                if (ActiveUnit == null || CurrentRoomUid <= 0) break;
                var room = _server.Center.RoomManager.GetRoom(CurrentRoomUid);
                if (room == null) break;
                var req = ev.GetPayload<KEGS_DUNGEON_STAGE_LOAD_REQ>(Region);
                room.BeginStageLoad(req.StageId);
                SendPacket(EventId.EGS_DUNGEON_STAGE_LOAD_ACK, new KPacketOK { Ok = NetError.NET_OK });
                var stageNot = new KEGS_DUNGEON_STAGE_LOAD_NOT
                {
                    StageId = req.StageId,
                    NumMember = Math.Max(1, (int)room.Info.JoinSlot),
                    ItemDropEventProbRate = _server.GameEventManager.GetActiveDropRateMultiplier(),
                    WithPlayPcBangEvent = PcBangManager.IsPcBang,
                };
                if (!KUserDungeonManager.IsHenirDungeon(room.Info.DungeonId) &&
                    !_server.TrainingCenterTable.IsTrainingCenterDungeon(room.Info.DungeonId) &&
                    _server.Center.DungeonEventManager.CheckTimeDropEventMonster(stageNot, out _, out int eventNpcId))
                {
                    room.MonsterManager.CreateMonster(
                        eventNpcId,
                        Math.Max((byte)1, ActiveUnit.Level),
                        KRoomMonsterManager.MonsterGrade.MG_NORMAL_NPC);
                }
                foreach (long uid in room.GetMemberUnitUids())
                    _server.SendToUnit(uid, EventId.EGS_DUNGEON_STAGE_LOAD_NOT, stageNot);
                break;
            }

            case EventId.EGS_DUNGEON_STAGE_LOAD_COMPLETE_REQ:
            {
                if (ActiveUnit == null || CurrentRoomUid <= 0 || Fsm.StateId != KGSFSM.S_ROOM) break;
                var room = _server.Center.RoomManager.GetRoom(CurrentRoomUid);
                if (room == null) break;
                var req = ev.GetPayload<KEGS_DUNGEON_STAGE_LOAD_COMPLETE_REQ>(Region);
                if (!room.MarkStageLoaded(ActiveUnit.UnitUid, out bool allLoaded, out sbyte slotIdx, req.IsSecretStage)) break;
                SendPacket(EventId.EGS_DUNGEON_STAGE_LOAD_COMPLETE_ACK, new KPacketOK { Ok = NetError.NET_OK });
                var compNot = new KEGS_DUNGEON_STAGE_LOAD_COMPLETE_NOT
                {
                    SlotId = slotIdx,
                    UnitUid = ActiveUnit.UnitUid,
                };
                foreach (long uid in room.GetMemberUnitUids())
                    _server.SendToUnit(uid, EventId.EGS_DUNGEON_STAGE_LOAD_COMPLETE_NOT, compNot);
                if (allLoaded)
                {
                    if (KUserDungeonManager.IsHenirDungeon(room.Info.DungeonId))
                    {
                        if (_server.Center.HenirResultTable.IsClearTempInventoryStage(room.CurrentStageId))
                        {
                            var clearNot = new KPacketOK { Ok = NetError.NET_OK };
                            foreach (long uid in room.GetMemberUnitUids())
                            {
                                _server.GetUserByUnitUid(uid)?.TempInventory.Clear();
                                _server.SendToUnit(uid, EventId.EGS_CLEAR_TEMP_INVENTORY_NOT, clearNot);
                            }
                        }

                        if (_server.Center.HenirResultTable.IsResurrectionStage(room.CurrentStageId))
                        {
                            foreach (long deadUid in room.GetDeadUnitUids())
                            {
                                room.SetUserDieState(deadUid, false);
                                var resNot = new KEGS_RESURRECT_TO_CONTINUE_DUNGEON_NOT
                                {
                                    UsedByUnitUid = deadUid,
                                    UnitUid = deadUid,
                                    StartPosIndex = 0,
                                };
                                foreach (long uid in room.GetMemberUnitUids())
                                    _server.SendToUnit(uid, EventId.EGS_RESURRECT_TO_CONTINUE_DUNGEON_NOT, resNot);
                            }
                        }
                    }

                    var startNot = new KEGS_DUNGEON_STAGE_START_NOT { StageId = room.CurrentStageId };
                    foreach (long uid in room.GetMemberUnitUids())
                        _server.SendToUnit(uid, EventId.EGS_DUNGEON_STAGE_START_NOT, startNot);
                }
                break;
            }

            case EventId.EGS_DUNGEON_SUB_STAGE_OPEN_REQ:
            {
                if (ActiveUnit == null || CurrentRoomUid <= 0) break;
                var room = _server.Center.RoomManager.GetRoom(CurrentRoomUid);
                if (room == null) break;
                var req = ev.GetPayload<KEGS_DUNGEON_SUB_STAGE_ID_MSG>(Region);
                room.BeginSubStageOpen(req.SubStageId);
                SendPacket(EventId.EGS_DUNGEON_SUB_STAGE_OPEN_ACK, new KPacketOK { Ok = NetError.NET_OK });
                var openNot = new KEGS_DUNGEON_SUB_STAGE_ID_MSG { SubStageId = req.SubStageId };
                foreach (long uid in room.GetMemberUnitUids())
                    _server.SendToUnit(uid, EventId.EGS_DUNGEON_SUB_STAGE_OPEN_NOT, openNot);
                break;
            }

            case EventId.EGS_DUNGEON_SUB_STAGE_LOAD_COMPLETE_REQ:
            {
                if (ActiveUnit == null || CurrentRoomUid <= 0) break;
                var room = _server.Center.RoomManager.GetRoom(CurrentRoomUid);
                if (room == null) break;
                var req = ev.GetPayload<KEGS_DUNGEON_SUB_STAGE_ID_MSG>(Region);
                room.MarkSubStageLoaded(ActiveUnit.UnitUid, out bool allLoaded);
                SendPacket(EventId.EGS_DUNGEON_SUB_STAGE_LOAD_COMPLETE_ACK, new KPacketOK { Ok = NetError.NET_OK });
                if (allLoaded)
                {
                    var startNot = new KEGS_DUNGEON_SUB_STAGE_ID_MSG { SubStageId = req.SubStageId };
                    foreach (long uid in room.GetMemberUnitUids())
                        _server.SendToUnit(uid, EventId.EGS_DUNGEON_SUB_STAGE_START_NOT, startNot);
                }
                break;
            }

            case EventId.EGS_DUNGEON_SUB_STAGE_GO_NEXT_REQ:
            {
                if (ActiveUnit == null || CurrentRoomUid <= 0) break;
                var room = _server.Center.RoomManager.GetRoom(CurrentRoomUid);
                if (room == null) break;
                var req = ev.GetPayload<KEGS_DUNGEON_SUB_STAGE_GO_NEXT_MSG>(Region);
                room.BeginSubStageOpen(req.NextStageId);
                SendPacket(EventId.EGS_DUNGEON_SUB_STAGE_GO_NEXT_ACK, new KPacketOK { Ok = NetError.NET_OK });
                foreach (long uid in room.GetMemberUnitUids())
                    _server.SendToUnit(uid, EventId.EGS_DUNGEON_SUB_STAGE_GO_NEXT_ALL_NOT, req);
                break;
            }

            case EventId.EGS_DUNGEON_SUB_STAGE_CLEAR_REQ:
            {
                if (ActiveUnit == null || CurrentRoomUid <= 0) break;
                var room = _server.Center.RoomManager.GetRoom(CurrentRoomUid);
                if (room == null) break;
                var req = ev.GetPayload<KEGS_DUNGEON_SUB_STAGE_CLEAR_REQ>(Region);
                SendPacket(EventId.EGS_DUNGEON_SUB_STAGE_CLEAR_ACK, new KEGS_DUNGEON_SUB_STAGE_CLEAR_ACK
                {
                    Ok = NetError.NET_OK,
                    ClearType = req.ClearConditionIndex,
                    StageIndex = room.CurrentStageId,
                    SubStageIndex = room.CurrentSubStageId,
                });
                break;
            }

            case EventId.EGS_NPC_UNIT_CREATE_REQ:
            {
                if (ActiveUnit == null || CurrentRoomUid <= 0) break;
                var room = _server.Center.RoomManager.GetRoom(CurrentRoomUid);
                if (room == null) break;
                var req = ev.GetPayload<KEGS_NPC_UNIT_CREATE_REQ>(Region);
                var ackList = new List<KNPCUnitNot>();
                var attribMap = new Dictionary<int, KAttribEnchantNpcInfo>();
                foreach (var npc in req.NpcUnitReqs)
                {
                    int assignedUid = room.MonsterManager.CreateMonster(
                        npc.NpcId, npc.Level, KRoomMonsterManager.MonsterGrade.MG_NORMAL_NPC,
                        groupId: npc.GroupId, explicitNpcUid: npc.Uid);
                    npc.Uid = assignedUid;
                    ackList.Add(new KNPCUnitNot { NpcUnitReq = npc });
                    var attrib = _server.Center.AttribNpcTable.RollNpcAttrib(npc.Level, room.Info.DifficultyLevel);
                    if (attrib != KAttribNpcTable.AttribType.AtNone)
                    {
                        attribMap[assignedUid] = new KAttribEnchantNpcInfo { AttribEnchant1 = (sbyte)attrib };
                        if (room.MonsterManager.GetNpcData(assignedUid, out var createdNpc) && createdNpc != null)
                            createdNpc.IsAttribNpc = true;
                    }
                }
                room.RecordSubStageActiveNpc(req.NpcUnitReqs.Count);
                SendPacket(EventId.EGS_NPC_UNIT_CREATE_ACK, new KPacketOK { Ok = NetError.NET_OK });
                var createNot = new KEGS_NPC_UNIT_CREATE_NOT
                {
                    NpcUnitAcks = ackList,
                    AttribNpcInfo = attribMap,
                    BattleFieldId = room.Info.BattleFieldId,
                };
                foreach (long uid in room.GetMemberUnitUids())
                    _server.SendToUnit(uid, EventId.EGS_NPC_UNIT_CREATE_NOT, createNot);
                break;
            }

            case EventId.EGS_NPC_UNIT_DIE_REQ:
            {
                if (ActiveUnit == null || CurrentRoomUid <= 0) break;
                var room = _server.Center.RoomManager.GetRoom(CurrentRoomUid);
                if (room == null) break;
                var req = ev.GetPayload<KEGS_NPC_UNIT_DIE_REQ>(Region);
                bool killedOk = room.MonsterManager.TryKillMonster(req.DieNpcUid, out var killed);
                int npcId = killed?.NpcId ?? 0;

                if (room.Info.RoomType == 3)
                {
                    room.BattleFieldMonsterManager.RecordNpcDie(req.DieNpcUid, (sbyte)(killed?.Grade ?? KRoomMonsterManager.MonsterGrade.MG_NORMAL_NPC), npcId);
                    room.BattleFieldGameManager.OnNpcUnitDie(
                        room.GetMemberUnitUids().Count(),
                        killed?.IsAttribNpc ?? false,
                        room.Info.DifficultyLevel,
                        (sbyte)(killed?.Grade ?? KRoomMonsterManager.MonsterGrade.MG_NORMAL_NPC));
                    _server.Global.FieldBossManager.UpdateTotalDangerousValue(room.Info.BattleFieldId, 1);
                }

                room.RecordDungeonStats(ActiveUnit.UnitUid, 0, 0, 0, killedOk ? 1 : 0);
                QuestManager.OnNpcUnitDie(room.Info.DungeonId, npcId);

                int expEach = (int)Math.Round(120 * _server.GameEventManager.GetActiveExpMultiplier());
                var expMap = new Dictionary<long, KEXPData>();
                foreach (long uid in room.GetMemberUnitUids())
                {
                    expMap[uid] = new KEXPData { Exp = expEach, PartyExp = room.Info.JoinSlot > 1 ? 25 : 0 };
                    var member = _server.GetUserByUnitUid(uid);
                    if (member?.ActiveUnit != null)
                    {
                        _server.Db.UpdateUnitProgress(uid, expEach, 0, 0);
                        member.ActiveUnit = _server.Db.GetUnitByUid(Region, uid);
                    }
                }

                if (killed != null && !killed.NoDrop)
                {
                    if (room.Info.RoomType == 3)
                    {
                        bool anyDropped = false;
                        var bfDrop = _server.Center.DropTable.RollBattleFieldDrop(room.Info.BattleFieldId, npcId);
                        if (bfDrop.dropped)
                        {
                            room.RegisterDropItem(bfDrop.itemId, bfDrop.quantity);
                            anyDropped = true;
                        }
                        if (KBattleFieldGameManager.MiddleBossDropInfos.TryGetValue((room.Info.BattleFieldId, npcId), out var mbDrop))
                        {
                            if (mbDrop.CommonBonusItemId > 0) { room.RegisterDropItem(mbDrop.CommonBonusItemId, Math.Max(1, mbDrop.CommonBonusItemCount)); anyDropped = true; }
                            if (mbDrop.BonusItemId > 0) { room.RegisterDropItem(mbDrop.BonusItemId, Math.Max(1, mbDrop.BonusItemCountHigh)); anyDropped = true; }
                        }
                        if (KBattleFieldGameManager.GetBattleFieldEventBossDropInfo(room.Info.BattleFieldId, npcId, out var ebDrop) && ebDrop != null)
                        {
                            if (ebDrop.CommonBonusItemId > 0) { room.RegisterDropItem(ebDrop.CommonBonusItemId, Math.Max(1, ebDrop.CommonBonusItemCount)); anyDropped = true; }
                            if (ebDrop.BonusItemId > 0) { room.RegisterDropItem(ebDrop.BonusItemId, Math.Max(1, ebDrop.BonusItemCountHigh)); anyDropped = true; }
                        }
                        if (room.BattleFieldGameManager.CheckAndDeleteReservedDangerousEvent(KBattleFieldGameManager.DangerousEvent.DeEliteMonsterDrop))
                        {
                            var eliteDrop = _server.Center.DropTable.RollBattleFieldDrop(room.Info.BattleFieldId, npcId, rateMultiplier: 2.0f);
                            if (eliteDrop.dropped) { room.RegisterDropItem(eliteDrop.itemId, eliteDrop.quantity); anyDropped = true; }
                        }
                        if (!anyDropped)
                            room.RegisterDropItem(KUserLetterBox.EdiBronzeEd, 1, ed: 150);
                    }
                    else
                    {
                        bool isHenirDrop = KUserDungeonManager.IsHenirDungeon(room.Info.DungeonId) || room.Info.DungeonMode is 1 or 2;
                        var (dropped, itemId, qty) = _server.Center.DropTable.RollMonsterDrop(
                            npcId, isHenir: isHenirDrop, dungeonId: room.Info.DungeonId, henirMode: room.Info.DungeonMode);
                        if (dropped)
                            room.RegisterDropItem(itemId, qty);
                        var staticDrop = _server.Center.DropTable.RollStaticDrop(room.Info.DungeonId);
                        if (staticDrop.dropped)
                            room.RegisterDropItem(staticDrop.itemId, staticDrop.quantity);
                        bool extraDropped = false;
                        if (room.IsSecretStage)
                        {
                            var extraDrop = _server.Center.DropTable.RollExtraStageDrop(npcId);
                            if (extraDrop.dropped) { room.RegisterDropItem(extraDrop.itemId, extraDrop.quantity); extraDropped = true; }
                        }
                        if (!dropped && !staticDrop.dropped && !extraDropped)
                            room.RegisterDropItem(KUserLetterBox.EdiBronzeEd, 1, ed: 150);
                    }
                }

                if (room.CheckSubStageNpcDieHack(isActiveNpc: killedOk))
                {
                    HackingUserSendType = KEGS_SERVER_CHECK_HACK_USER_REQ.HctCheckNpcDieTime;
                    HackingManager.SetHackingUserSendType(HackingUserSendType);
                    SendPacket(EventId.EGS_SERVER_CHECK_HACK_USER_REQ, new KEGS_SERVER_CHECK_HACK_USER_REQ
                    {
                        HackingCheckType = KEGS_SERVER_CHECK_HACK_USER_REQ.HctCheckNpcDieTime,
                    });
                }

                if (room.DefenceDungeonManager.IncreaseDefenceDungeonPhase(npcId))
                {
                    int nextPhase = room.DefenceDungeonManager.DefenceDungeonPhase;
                    foreach (long uid in room.GetMemberUnitUids())
                    {
                        var memberUser = _server.GetUserByUnitUid(uid);
                        if (memberUser == null) continue;
                        var slots = memberUser.SetupDefenceDungeonQuickSlotItems(nextPhase);
                        memberUser.SendPacket(EventId.EGS_UPDATE_INVENTORY_SLOT_INFO_NOT,
                            new KEGS_UPDATE_INVENTORY_SLOT_INFO_NOT { InventorySlotInfo = slots });
                    }
                }

                SendPacket(EventId.EGS_NPC_UNIT_DIE_ACK, new KPacketOK { Ok = NetError.NET_OK });
                var dieNot = new KEGS_NPC_UNIT_DIE_NOT
                {
                    DieNpcUid = req.DieNpcUid,
                    NpcId = npcId,
                    AttUnitType = req.AttUnitType,
                    AttUnit = req.AttUnit,
                    ExpList = expMap,
                };
                foreach (long uid in room.GetMemberUnitUids())
                    _server.SendToUnit(uid, EventId.EGS_NPC_UNIT_DIE_NOT, dieNot);
                break;
            }

            case EventId.EGS_GET_ITEM_REQ:
            {
                if (ActiveUnit == null || CurrentRoomUid <= 0) break;
                var room = _server.Center.RoomManager.GetRoom(CurrentRoomUid);
                if (room == null) break;
                var req = ev.GetPayload<KEGS_GET_ITEM_REQ>(Region);
                int itemId = req.ItemInfo.ItemId;
                int qty = Math.Max(1, req.ItemInfo.Quantity);
                int edAmount = 0;
                sbyte enchantLv = req.ItemInfo.EnchantLevel;
                if (room.TryClaimDropItem(req.DropItemUid, out var drop))
                {
                    itemId = drop.itemId;
                    qty = Math.Max(1, drop.quantity);
                    edAmount = drop.ed;
                    enchantLv = drop.enchantLevel;
                }

                var getEdMap = new Dictionary<long, KDungeonRewardED>();
                if (itemId == KUserLetterBox.EdiBronzeEd || edAmount > 0)
                {
                    int totalEd = Math.Max(100, edAmount);
                    var members = room.GetMemberUnitUids();
                    int share = Math.Max(1, totalEd / Math.Max(1, members.Count));
                    foreach (long uid in members)
                    {
                        var u = _server.GetUserByUnitUid(uid);
                        if (u?.ActiveUnit != null)
                        {
                            u.EdManager.TryAddEd(share, KUserEDManager.EdReason.ErInEdDungeonDrop, out _);
                            _server.Db.UpdateUnitProgress(uid, 0, share, 0);
                            u.ActiveUnit = _server.Db.GetUnitByUid(Region, uid);
                            getEdMap[uid] = new KDungeonRewardED { Ed = share };
                        }
                    }
                    SendPacket(EventId.EGS_GET_ITEM_ACK, new KPacketOK { Ok = NetError.NET_OK });
                    var edNot = new KEGS_GET_ITEM_NOT
                    {
                        GetUnitUid = ActiveUnit.UnitUid,
                        ItemId = KUserLetterBox.EdiBronzeEd,
                        DropItemUid = req.DropItemUid,
                        GetItemType = room.Info.GetItemType,
                        IsItemSuccess = true,
                        TotalEd = totalEd,
                        GetEd = getEdMap,
                        EnchantLevel = 0,
                    };
                    foreach (long uid in members)
                        _server.SendToUnit(uid, EventId.EGS_GET_ITEM_NOT, edNot);
                }
                else
                {
                    if (itemId > 0)
                    {
                        var granted = _server.Db.GrantUnitItem(ActiveUnit.UnitUid, itemId, qty);
                        SendPacket(EventId.EGS_GET_ITEM_REALTIME_NOT, new KEGS_GET_ITEM_REALTIME_NOT
                        {
                            InventorySlotInfo = [granted],
                        });
                    }
                    SendPacket(EventId.EGS_GET_ITEM_ACK, new KPacketOK { Ok = NetError.NET_OK });
                    var itemNot = new KEGS_GET_ITEM_NOT
                    {
                        GetUnitUid = ActiveUnit.UnitUid,
                        ItemId = itemId,
                        DropItemUid = req.DropItemUid,
                        GetItemType = room.Info.GetItemType,
                        IsItemSuccess = true,
                        EnchantLevel = enchantLv,
                    };
                    foreach (long uid in room.GetMemberUnitUids())
                        _server.SendToUnit(uid, EventId.EGS_GET_ITEM_NOT, itemNot);
                }
                break;
            }

            case EventId.EGS_USER_UNIT_DIE_REQ:
            {
                if (ActiveUnit == null || CurrentRoomUid <= 0) break;
                var room = _server.Center.RoomManager.GetRoom(CurrentRoomUid);
                if (room == null) break;
                var req = ev.GetPayload<KEGS_USER_UNIT_DIE_REQ>(Region);
                room.SetUserDieState(ActiveUnit.UnitUid, true);
                if (req.KillerUserUnitUid > 0)
                    room.RecordPvpKill(req.KillerUserUnitUid, ActiveUnit.UnitUid);
                SendPacket(EventId.EGS_USER_UNIT_DIE_ACK, new KEGS_USER_UNIT_DIE_ACK
                {
                    ReBirthTime = 5.0f,
                    Ok = NetError.NET_OK,
                });
                var dieNot = new KEGS_USER_UNIT_DIE_NOT
                {
                    KillerNpcUid = req.KillerNpcUid,
                    KillerUserUnitUid = req.KillerUserUnitUid,
                    MaxDamageKillerUserUnitUid = req.MaxDamageKillerUserUnitUid,
                    KilledUserUnitUid = ActiveUnit.UnitUid,
                };
                foreach (long uid in room.GetMemberUnitUids())
                    _server.SendToUnit(uid, EventId.EGS_USER_UNIT_DIE_NOT, dieNot);
                break;
            }

            case EventId.EGS_USER_UNIT_DIE_COMPLETE_REQ:
            {
                if (ActiveUnit == null) break;
                if (CurrentRoomUid <= 0)
                {
                    SendPacket(EventId.EGS_USER_UNIT_DIE_COMPLETE_ACK, new KPacketOK { Ok = NetError.ERR_ROOM_04 });
                    break;
                }
                var room = _server.Center.RoomManager.GetRoom(CurrentRoomUid);
                if (room == null)
                {
                    SendPacket(EventId.EGS_USER_UNIT_DIE_COMPLETE_ACK, new KPacketOK { Ok = NetError.ERR_ROOM_04 });
                    break;
                }
                SendPacket(EventId.EGS_USER_UNIT_DIE_COMPLETE_ACK, new KPacketOK { Ok = NetError.NET_OK });
                var dieCompleteNot = new KEGS_USER_UNIT_DIE_COMPLETE_NOT { UnitUid = ActiveUnit.UnitUid };
                foreach (long uid in room.GetMemberUnitUids())
                    _server.SendToUnit(uid, EventId.EGS_USER_UNIT_DIE_COMPLETE_NOT, dieCompleteNot);
                break;
            }

            case EventId.EGS_RESURRECT_TO_CONTINUE_DUNGEON_REQ:
            {
                if (ActiveUnit == null || CurrentRoomUid <= 0) break;
                var room = _server.Center.RoomManager.GetRoom(CurrentRoomUid);
                if (room == null) break;
                var req = ev.GetPayload<KEGS_RESURRECT_TO_CONTINUE_DUNGEON_REQ>(Region);
                long resurrectedUid = req.UnitUid > 0 ? req.UnitUid : ActiveUnit.UnitUid;
                room.SetUserDieState(resurrectedUid, false);
                SendPacket(EventId.EGS_RESURRECT_TO_CONTINUE_DUNGEON_ACK, new KEGS_RESURRECT_TO_CONTINUE_DUNGEON_ACK
                {
                    Ok = NetError.NET_OK,
                    DungeonId = room.Info.DungeonId,
                    NumResurrectionStone = 3,
                    UseCash = req.UseCash,
                });
                var resNot = new KEGS_RESURRECT_TO_CONTINUE_DUNGEON_NOT
                {
                    UsedByUnitUid = req.UsedByUnitUid > 0 ? req.UsedByUnitUid : ActiveUnit.UnitUid,
                    UnitUid = resurrectedUid,
                    StartPosIndex = 0,
                };
                foreach (long uid in room.GetMemberUnitUids())
                    _server.SendToUnit(uid, EventId.EGS_RESURRECT_TO_CONTINUE_DUNGEON_NOT, resNot);
                break;
            }

            // -------- BattleField Join & NPC Sync --------

            case EventId.EGS_JOIN_BATTLE_FIELD_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_JOIN_BATTLE_FIELD_REQ>(Region);
                LeaveCurrentField();
                if (CurrentRoomUid > 0)
                {
                    _server.Center.RoomManager.LeaveRoom(CurrentRoomUid, ActiveUnit.UnitUid);
                    CurrentRoomUid = 0;
                }

                int bfId = req.BattleFieldJoinInfo.BattleFieldId > 0 ? req.BattleFieldJoinInfo.BattleFieldId : 40001;
                var bfRoom = _server.Center.RoomManager.GetOrCreateBattleFieldRoom(bfId);
                var (joinOk, joinedSlot) = bfRoom.TryAddUser(BuildRoomUserInfo(), asObserver: false, password: string.Empty);
                if (joinOk != NetError.NET_OK || joinedSlot == null)
                {
                    SendPacket(EventId.EGS_JOIN_BATTLE_FIELD_ACK, new KEGS_JOIN_BATTLE_FIELD_ACK { Ok = joinOk });
                    break;
                }

                CurrentRoomUid = bfRoom.Info.RoomUid;
                ActiveUnit.LastPos.MapId = bfId;
                Fsm.TryStateTransition(KGSFSM.I_TO_ROOM, out _);

                SendPacket(EventId.EGS_JOIN_BATTLE_FIELD_ACK, new KEGS_JOIN_BATTLE_FIELD_ACK
                {
                    Ok = NetError.NET_OK,
                    BattleFieldJoinInfo = req.BattleFieldJoinInfo,
                    RoomInfo = bfRoom.Info,
                    Slots = bfRoom.SnapshotSlots(),
                    CnIp = bfRoom.Info.UdpRelayIp,
                    UdpRelayIp = bfRoom.Info.UdpRelayIp,
                    UdpRelayPort = bfRoom.Info.UdpRelayPort,
                });

                var joinNot = new KEGS_JOIN_ROOM_NOT { JoinSlot = joinedSlot, StartPosIndex = req.BattleFieldJoinInfo.StartPosIndex };
                foreach (long uid in bfRoom.GetMemberUnitUids().Where(id => id != ActiveUnit.UnitUid))
                    _server.SendToUnit(uid, EventId.EGS_JOIN_ROOM_NOT, joinNot);
                break;
            }

            case EventId.EGS_BATTLE_FIELD_JOIN_USER_LOAD_COMPLETE_NOT:
            {
                if (ActiveUnit == null || CurrentRoomUid <= 0) break;
                var room = _server.Center.RoomManager.GetRoom(CurrentRoomUid);
                if (room == null) break;
                SendPacket(EventId.EGS_BATTLE_FIELD_NPC_P2P_SYNC_NOT, new KEGS_BATTLE_FIELD_NPC_P2P_SYNC_NOT
                {
                    NonNpcSyncUserList = room.GetMemberUnitUids().Where(id => id != ActiveUnit.UnitUid).ToList(),
                });
                break;
            }

            case EventId.EGS_BATTLE_FIELD_NPC_LOAD_COMPLETE_REQ:
            {
                if (ActiveUnit == null) break;
                if (CurrentRoomUid <= 0)
                {
                    SendPacket(EventId.EGS_BATTLE_FIELD_NPC_LOAD_COMPLETE_ACK, new KPacketOK { Ok = NetError.ERR_BATTLEFIELD_16 });
                    break;
                }
                var bfRoom = _server.Center.RoomManager.GetRoom(CurrentRoomUid);
                if (bfRoom == null || bfRoom.Info.RoomType != 3) // RT_BATTLE_FIELD
                {
                    SendPacket(EventId.EGS_BATTLE_FIELD_NPC_LOAD_COMPLETE_ACK, new KPacketOK { Ok = NetError.ERR_BATTLEFIELD_17 });
                    break;
                }
                SendPacket(EventId.EGS_BATTLE_FIELD_NPC_LOAD_COMPLETE_ACK, new KPacketOK { Ok = NetError.NET_OK });
                break;
            }

            case EventId.EGS_UPDATE_BATTLE_FIELD_USER_POS_NOT:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_UPDATE_BATTLE_FIELD_USER_POS_NOT>(Region);
                ActiveUnit.LastPos = req.LastPos;
                break;
            }

            // -------- Session / Channel / Field --------

            case EventId.EGS_FIELD_LOADING_COMPLETE_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_FIELD_LOADING_COMPLETE_REQ>(Region);
                ActiveUnit.LastPos.LastTouchLineIndex = req.LastTouchLineIndex;
                ActiveUnit.LastPos.LastPosValue = req.LastPosValue;
                if (CurrentFieldMapId <= 0 && ActiveUnit.LastPos.MapId > 0)
                {
                    CurrentFieldMapId = ActiveUnit.LastPos.MapId;
                    Fsm.TryStateTransition(KGSFSM.I_TO_FIELD_MAP, out _);
                    var zone = _server.FieldManager.GetOrCreateZone(CurrentFieldMapId);
                    var myFieldInfo = BuildFieldUserInfo();
                    var sectorUsers = zone.Enter(myFieldInfo);
                    SendPacket(EventId.EGS_CHANGE_USER_LIST_IN_SECTOR_NOT,
                        new KEGS_CHANGE_USER_LIST_IN_SECTOR_NOT { NewUsers = sectorUsers });
                    foreach (long otherUid in zone.GetOtherUnitUids(ActiveUnit.UnitUid))
                    {
                        _server.SendToUnit(otherUid, EventId.EGS_CHANGE_USER_LIST_IN_SECTOR_NOT,
                            new KEGS_CHANGE_USER_LIST_IN_SECTOR_NOT { NewUsers = [myFieldInfo] });
                    }
                }
                SendPacket(EventId.EGS_FIELD_LOADING_COMPLETE_ACK, new KEGS_FIELD_LOADING_COMPLETE_ACK { Ok = NetError.NET_OK });
                break;
            }

            case EventId.EGS_LEAVE_FIELD_NOT:
            {
                LeaveCurrentField();
                break;
            }

            case EventId.EGS_OPTION_UPDATE_REQ:
            {
                var req = ev.GetPayload<KEGS_OPTION_UPDATE_REQ>(Region);
                bool ok = KFieldOption.VerifyLimitOption(req.FieldOption.LimitOption);
                if (ok)
                {
                    FieldOption = req.FieldOption;
                    if (ActiveUnit != null && CurrentFieldMapId > 0)
                        _server.FieldManager.GetOrCreateZone(CurrentFieldMapId).SetFieldOption(ActiveUnit.UnitUid, req.FieldOption);
                }
                SendPacket(EventId.EGS_OPTION_UPDATE_ACK, new KPacketOK { Ok = ok ? NetError.NET_OK : NetError.ERR_CLIENT_STATE });
                break;
            }

            case EventId.EGS_GET_CHANNEL_LIST_REQ:
            {
                var chInfo = new KChannelInfo
                {
                    ServerUid = 1,
                    ChannelId = 1,
                    ChannelName = $"{Region.Code}-CH1",
                    ServerGroupId = 1,
                    Ip = "127.0.0.1",
                    MasterPort = _server.ListenPort,
                    NcUdpPort = _server.UdpEchoPort,
                    MaxUser = 1000,
                    CurrentUser = _server.ConnectedUserCount,
                    CurPartyCount = _server.Center.PartyManager.PartyCount,
                };
                SendPacket(EventId.EGS_GET_CHANNEL_LIST_ACK, new KEGS_GET_CHANNEL_LIST_ACK
                {
                    ServerGroupId = 1,
                    ChannelInfo = [chInfo],
                    ChannelBonusInfo = new Dictionary<int, KChannelBonusInfo>
                    {
                        [1] = new KChannelBonusInfo { Enable = false },
                    },
                });
                break;
            }

            case EventId.EGS_CHECK_CHANNEL_CHANGE_REQ:
            {
                var req = ev.GetPayload<KEGS_CHECK_CHANNEL_CHANGE_REQ>(Region);
                SendPacket(EventId.EGS_CHECK_CHANNEL_CHANGE_ACK, new KPacketOK
                {
                    Ok = req.ChannelId > 0 ? NetError.NET_OK : NetError.ERR_CLIENT_STATE,
                });
                break;
            }

            case EventId.EGS_DISCONNECT_FOR_CHANNEL_CHANGE_REQ:
            {
                _pendingDisconnectEventId = EventId.EGS_DISCONNECT_FOR_CHANNEL_CHANGE_REQ;
                LeaveCurrentField();
                if (ActiveUnit != null)
                    _server.EventDelaySystemManager.UnRegOpenRandomCubeEvent(ActiveUnit.UnitUid);
                if (ActiveUnit != null && CurrentRoomUid > 0)
                {
                    _server.Center.RoomManager.LeaveRoom(CurrentRoomUid, ActiveUnit.UnitUid);
                    CurrentRoomUid = 0;
                }
                ActiveUnit = null;
                Fsm.TryStateTransition(KGSFSM.I_TO_SERVER_SELECT, out _);
                SendPacket(EventId.EGS_DISCONNECT_FOR_CHANNEL_CHANGE_ACK, new KPacketOK { Ok = NetError.NET_OK });
                break;
            }

            case EventId.EGS_DISCONNECT_FOR_SERVER_SELECT_REQ:
            {
                _pendingDisconnectEventId = EventId.EGS_DISCONNECT_FOR_SERVER_SELECT_REQ;
                LeaveCurrentField();
                if (ActiveUnit != null)
                    _server.EventDelaySystemManager.UnRegOpenRandomCubeEvent(ActiveUnit.UnitUid);
                if (ActiveUnit != null && CurrentRoomUid > 0)
                {
                    _server.Center.RoomManager.LeaveRoom(CurrentRoomUid, ActiveUnit.UnitUid);
                    CurrentRoomUid = 0;
                }
                ActiveUnit = null;
                Fsm.TryStateTransition(KGSFSM.I_TO_SERVER_SELECT, out _);
                SendPacket(EventId.EGS_DISCONNECT_FOR_SERVER_SELECT_ACK, new KPacketOK { Ok = NetError.NET_OK });
                break;
            }

            case EventId.EGS_CLIENT_QUIT_REQ:
            {
                var req = ev.GetPayload<KEGS_CLIENT_QUIT_REQ>(Region);
                _server.StatisticsManager.IncrementCounter($"DisconnectReason_{req.Reason}");
                if (req.ErrorMsgInfoList.Count > 0)
                    KLog.Cwarn($"KGSUser[{Uid}] client quit reason={req.Reason} errors={req.ErrorMsgInfoList.Count}");
                SendPacket(EventId.EGS_CLIENT_QUIT_ACK, new KPacketOK { Ok = NetError.NET_OK });
                ReserveDestroy();
                break;
            }

            // -------- Room Common / TempInventory / TC / Secret Stage / Forced Exit --------

            case EventId.EGS_BAN_USER_REQ:
            {
                if (ActiveUnit == null || CurrentRoomUid <= 0) break;
                var req = ev.GetPayload<KEGS_BAN_USER_REQ>(Region);
                var room = _server.Center.RoomManager.GetRoom(CurrentRoomUid);
                bool ok = room != null && room.BanUser(ActiveUnit.UnitUid, req.UnitUid);
                SendPacket(EventId.EGS_BAN_USER_ACK, new KPacketOK { Ok = ok ? NetError.NET_OK : NetError.ERR_ROOM_08 });
                if (ok)
                {
                    var target = _server.GetUserByUnitUid(req.UnitUid);
                    if (target != null)
                    {
                        target.CurrentRoomUid = 0;
                        target.Fsm.TryStateTransition(KGSFSM.I_TO_FIELD_MAP, out _);
                        target.SendPacket(EventId.EGS_BAN_USER_NOT, new KPacketOK { Ok = NetError.NET_OK });
                    }
                }
                break;
            }

            case EventId.EGS_GET_TEMP_ITEM_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_GET_TEMP_ITEM_REQ>(Region);
                if (!TempInventory.PrepareTempItemToInventory(req.TempItemUid, out var entry) || entry == null)
                {
                    SendPacket(EventId.EGS_GET_TEMP_ITEM_ACK, new KEGS_GET_TEMP_ITEM_ACK
                    {
                        Ok = NetError.ERR_ITEM_04,
                        TempItemUid = req.TempItemUid,
                    });
                    break;
                }
                var granted = _server.Db.GrantUnitItem(ActiveUnit.UnitUid, entry.ItemId, Math.Max(1, entry.ItemInfo.Quantity));
                SendPacket(EventId.EGS_GET_TEMP_ITEM_ACK, new KEGS_GET_TEMP_ITEM_ACK
                {
                    Ok = NetError.NET_OK,
                    ItemId = entry.ItemId,
                    TempItemUid = entry.TempItemUid,
                    InventorySlotInfo = [granted],
                });
                break;
            }

            case EventId.EGS_DELETE_TEMP_ITEM_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_GET_TEMP_ITEM_REQ>(Region);
                bool ok = TempInventory.DeleteTempItem(req.TempItemUid);
                SendPacket(EventId.EGS_DELETE_TEMP_ITEM_ACK, new KEGS_DELETE_TEMP_ITEM_ACK
                {
                    Ok = ok ? NetError.NET_OK : NetError.ERR_ITEM_04,
                    TempItemUid = req.TempItemUid,
                });
                break;
            }

            case EventId.EGS_CHECK_EXIST_MONSTER_UID_REQ:
            {
                if (ActiveUnit == null || CurrentRoomUid <= 0) break;
                var req = ev.GetPayload<KEGS_CHECK_EXIST_MONSTER_UID_REQ>(Region);
                var room = _server.Center.RoomManager.GetRoom(CurrentRoomUid);
                var missing = room?.GetNotExistNpcUids(req.NpcUid) ?? req.NpcUid;
                SendPacket(EventId.EGS_CHECK_EXIST_MONSTER_UID_ACK, new KEGS_CHECK_EXIST_MONSTER_UID_ACK
                {
                    Ok = NetError.NET_OK,
                    NotExistNpcUid = missing,
                });
                break;
            }

            case EventId.EGS_DUNGEON_PLAY_INFO_TO_SERVER_NOT:
            {
                if (ActiveUnit == null || CurrentRoomUid <= 0) break;
                if (!ev.TryReadPayload<KEGS_DUNGEON_PLAY_INFO_TO_SERVER_NOT>(Region, out var req))
                {
                    HackingManager.IncreaseDeserializeFailCount(ev.EventIdValue);
                    break;
                }
                var room = _server.Center.RoomManager.GetRoom(CurrentRoomUid);
                room?.RecordDungeonStats(ActiveUnit.UnitUid, req.ComboScore, req.TechScore, req.DamageCount, 0);
                break;
            }

            case EventId.EGS_DUNGEON_SECRET_STAGE_ENTER_CHECK_REQ:
            {
                if (ActiveUnit == null || CurrentRoomUid <= 0) break;
                var room = _server.Center.RoomManager.GetRoom(CurrentRoomUid);
                bool canEnter = room != null && room.Info.DungeonMode == 1;
                SendPacket(EventId.EGS_DUNGEON_SECRET_STAGE_ENTER_CHECK_ACK, new KEGS_DUNGEON_SECRET_STAGE_ENTER_CHECK_ACK
                {
                    Ok = NetError.NET_OK,
                    CanEnterSecretStage = canEnter,
                });
                break;
            }

            case EventId.EGS_SECRET_STAGE_LOAD_REQ:
            {
                if (ActiveUnit == null || CurrentRoomUid <= 0) break;
                var req = ev.GetPayload<KEGS_SECRET_STAGE_LOAD_REQ>(Region);
                var room = _server.Center.RoomManager.GetRoom(CurrentRoomUid);
                if (room == null) break;
                room.BeginStageLoad(req.PadId);
                var stageNot = new KEGS_DUNGEON_STAGE_LOAD_NOT
                {
                    StageId = req.PadId,
                    NumMember = Math.Max(1, (int)room.Info.JoinSlot),
                    ItemDropEventProbRate = _server.GameEventManager.GetActiveDropRateMultiplier(),
                    WithPlayPcBangEvent = PcBangManager.IsPcBang,
                };
                foreach (long uid in room.GetMemberUnitUids())
                    _server.SendToUnit(uid, EventId.EGS_DUNGEON_STAGE_LOAD_NOT, stageNot);
                break;
            }

            case EventId.EGS_FORCED_EXIT_VOTE_REQ:
            {
                if (ActiveUnit == null || CurrentRoomUid <= 0) break;
                var req = ev.GetPayload<KEGS_FORCED_EXIT_VOTE_REQ>(Region);
                var room = _server.Center.RoomManager.GetRoom(CurrentRoomUid);
                bool ok = room != null && room.RecordForcedExitVote(ActiveUnit.UnitUid, req.UserUid, req.WantForceExit);
                SendPacket(EventId.EGS_FORCED_EXIT_VOTE_ACK, new KEGS_FORCED_EXIT_VOTE_ACK
                {
                    UserUid = req.UserUid,
                    Ok = ok ? NetError.NET_OK : NetError.ERR_ROOM_09,
                });
                break;
            }

            case EventId.EGS_CREATE_TC_ROOM_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_CREATE_TC_ROOM_REQ>(Region);
                if (!_server.TrainingCenterTable.CheckIfEnter(ActiveUnit.TcClear, req.TcId, (byte)ActiveUnit.Level, (sbyte)ActiveUnit.UnitClass))
                {
                    SendPacket(EventId.EGS_CREATE_TC_ROOM_ACK, new KEGS_CREATE_TC_ROOM_ACK { Ok = NetError.ERR_TC_01 });
                    break;
                }
                int tcDungeonId = _server.TrainingCenterTable.GetDungeonId(req.TcId);
                if (tcDungeonId <= 0)
                {
                    SendPacket(EventId.EGS_CREATE_TC_ROOM_ACK, new KEGS_CREATE_TC_ROOM_ACK { Ok = NetError.ERR_TC_03 });
                    break;
                }
                float tcPlayTime = _server.TrainingCenterTable.GetPlayTime(req.TcId);
                if (tcPlayTime <= 0f) tcPlayTime = 180.0f;

                LeaveCurrentField();
                var tcRoom = _server.Center.RoomManager.CreateRoom(new KRoomInfo
                {
                    RoomType = 4, // RT_TRAINING_CENTER
                    RoomName = $"TC_{req.TcId}",
                    Public = false,
                    MaxSlot = 1,
                    DungeonId = tcDungeonId,
                    PlayTime = tcPlayTime,
                }, BuildRoomUserInfo());
                tcRoom.StartGame();
                CurrentRoomUid = tcRoom.Info.RoomUid;
                _currentTcId = req.TcId;
                Fsm.TryStateTransition(KGSFSM.I_TO_ROOM, out _);
                SendPacket(EventId.EGS_CREATE_TC_ROOM_ACK, new KEGS_CREATE_TC_ROOM_ACK
                {
                    Ok = NetError.NET_OK,
                    TcInfo = new KTCInfo
                    {
                        TcId = req.TcId,
                        DungeonId = tcDungeonId,
                        RoomUid = tcRoom.Info.RoomUid,
                        RoomType = tcRoom.Info.RoomType,
                        UdpRelayIp = tcRoom.Info.UdpRelayIp,
                        UdpRelayPort = tcRoom.Info.UdpRelayPort,
                        PlayTime = tcPlayTime,
                    },
                    RoomUserInfo = BuildRoomUserInfo(),
                });
                break;
            }

            case EventId.EGS_END_TC_GAME_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_END_TC_GAME_REQ>(Region);
                if (CurrentRoomUid > 0)
                {
                    _server.Center.RoomManager.LeaveRoom(CurrentRoomUid, ActiveUnit.UnitUid);
                    CurrentRoomUid = 0;
                }
                if (req.IsSuccess && _currentTcId > 0)
                {
                    bool firstClear = !ActiveUnit.TcClear.ContainsKey(_currentTcId);
                    _server.Db.RecordTcClear(ActiveUnit.UnitUid, _currentTcId);
                    ActiveUnit.TcClear[_currentTcId] = new KTCClearInfo
                    {
                        TcId = _currentTcId,
                        ClearTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                        New = true,
                    };
                    if (firstClear && _server.TrainingCenterTable.TryGetReward(_currentTcId, out int rewardEd, out int rewardExp))
                    {
                        _server.Db.UpdateUnitProgress(ActiveUnit.UnitUid, addExp: rewardExp, addEd: rewardEd, mapId: 0);
                        EdManager.TryAddEd(rewardEd, KUserEDManager.EdReason.ErInEdTrainingReward, out _);
                        Statistics.IncreaseCount(KUserStatistics.UsTable.EdData, 0, 1 /* US_ED_PTraining */, rewardEd);
                        ActiveUnit = _server.Db.GetUnitByUid(Region, ActiveUnit.UnitUid);
                    }
                }
                _currentTcId = 0;
                Fsm.TryStateTransition(KGSFSM.I_TO_FIELD_MAP, out _);
                SendPacket(EventId.EGS_END_TC_GAME_ACK, new KEGS_END_TC_GAME_ACK
                {
                    Ok = NetError.NET_OK,
                    IsSuccess = req.IsSuccess,
                    UnitInfo = ActiveUnit,
                });
                break;
            }

            // -------- Party Management & Quick Start --------

            case EventId.EGS_QUICK_START_DUNGEON_GAME_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_QUICK_START_DUNGEON_GAME_REQ>(Region);
                var party = CurrentPartyUid > 0 ? _server.Center.PartyManager.GetParty(CurrentPartyUid) : null;
                if (party == null)
                {
                    party = _server.Center.PartyManager.CreateParty(new KCreatePartyInfo
                    {
                        PartyType = 0,
                        DungeonId = req.DungeonId,
                        DifficultyLevel = req.DifficultyLevel,
                        GetItemType = req.GetItemType,
                        DungeonMode = req.DungeonMode,
                    }, BuildPartyUserInfo());
                    CurrentPartyUid = party.Info.PartyUid;
                }
                if (!party.CanStartGame(ActiveUnit.UnitUid))
                {
                    SendPacket(EventId.EGS_QUICK_START_DUNGEON_GAME_ACK, new KEGS_QUICK_START_DUNGEON_GAME_ACK
                    {
                        Ok = NetError.ERR_PARTY_13,
                        FailUnitUid = ActiveUnit.UnitUid,
                        FailUserNickName = ActiveUnit.NickName,
                    });
                    break;
                }
                var members = party.SnapshotMembers();
                var dRoom = _server.Center.RoomManager.CreateRoom(new KRoomInfo
                {
                    RoomType = 1, // RT_DUNGEON
                    RoomName = party.Info.PartyName,
                    Public = false,
                    MaxSlot = 4,
                    DungeonId = req.DungeonId,
                    DifficultyLevel = req.DifficultyLevel,
                    GetItemType = req.GetItemType,
                    DungeonMode = req.DungeonMode,
                    PartyUid = party.Info.PartyUid,
                });
                foreach (var m in members)
                {
                    var u = _server.GetUserByUnitUid(m.UnitUid);
                    if (u == null) continue;
                    u.LeaveCurrentField();
                    dRoom.TryAddUser(u.BuildRoomUserInfo(), asObserver: false, password: string.Empty);
                    u.CurrentRoomUid = dRoom.Info.RoomUid;
                    u.Fsm.TryStateTransition(KGSFSM.I_TO_ROOM, out _);
                }
                dRoom.StartLoading(0, req.DungeonId);
                SendPacket(EventId.EGS_QUICK_START_DUNGEON_GAME_ACK, new KEGS_QUICK_START_DUNGEON_GAME_ACK { Ok = NetError.NET_OK });
                var startNot = new KEGS_PARTY_GAME_START_NOT
                {
                    WorldId = 0,
                    RoomInfo = dRoom.Info,
                    Slots = dRoom.SnapshotSlots(),
                    CnIp = dRoom.Info.UdpRelayIp,
                };
                foreach (var m in members)
                    _server.SendToUnit(m.UnitUid, EventId.EGS_PARTY_GAME_START_NOT, startNot);
                break;
            }

            case EventId.EGS_PARTY_GAME_START_OPINION_CHECK_REPLY_NOT:
            {
                if (ActiveUnit == null || CurrentPartyUid <= 0) break;
                var req = ev.GetPayload<KEGS_PARTY_GAME_START_OPINION_CHECK_REPLY_NOT>(Region);
                var party = _server.Center.PartyManager.GetParty(CurrentPartyUid);
                if (party == null) break;
                if (!req.Accept)
                {
                    var failNot = new KEGS_PARTY_GAME_START_FAILED_NOT
                    {
                        Reason = NetError.NOT_PARTY_GAME_START_FAILED_00,
                        FailUnitUid = ActiveUnit.UnitUid,
                    };
                    foreach (long uid in party.GetUnitUids())
                        _server.SendToUnit(uid, EventId.EGS_PARTY_GAME_START_FAILED_NOT, failNot);
                }
                else
                {
                    party.SetReady(ActiveUnit.UnitUid, true);
                    var readyNot = new KEGS_PARTY_CHANGE_READY_NOT { UnitUid = ActiveUnit.UnitUid, Ready = true };
                    foreach (long uid in party.GetUnitUids())
                        _server.SendToUnit(uid, EventId.EGS_PARTY_CHANGE_READY_NOT, readyNot);
                }
                break;
            }

            case EventId.EGS_PARTY_CHANGE_HOST_REQ:
            {
                if (ActiveUnit == null || CurrentPartyUid <= 0) break;
                var req = ev.GetPayload<KPacketLong>(Region);
                var party = _server.Center.PartyManager.GetParty(CurrentPartyUid);
                long beforeUid = 0, afterUid = 0;
                bool ok = party != null && party.ChangeHost(ActiveUnit.UnitUid, req.Value, out beforeUid, out afterUid);
                SendPacket(EventId.EGS_PARTY_CHANGE_HOST_ACK, new KPacketOK { Ok = ok ? NetError.NET_OK : NetError.ERR_PARTY_09 });
                if (ok && party != null)
                {
                    var not = new KEGS_PARTY_CHANGE_HOST_NOT { BeforeUid = beforeUid, AfterUid = afterUid };
                    foreach (long uid in party.GetUnitUids())
                        _server.SendToUnit(uid, EventId.EGS_PARTY_CHANGE_HOST_NOT, not);
                }
                break;
            }

            case EventId.EGS_PARTY_BAN_USER_REQ:
            {
                if (ActiveUnit == null || CurrentPartyUid <= 0) break;
                var req = ev.GetPayload<KPacketLong>(Region);
                var party = _server.Center.PartyManager.GetParty(CurrentPartyUid);
                bool ok = party != null && party.BanUser(ActiveUnit.UnitUid, req.Value);
                SendPacket(EventId.EGS_PARTY_BAN_USER_ACK, new KPacketOK { Ok = ok ? NetError.NET_OK : NetError.ERR_PARTY_09 });
                if (ok && party != null)
                {
                    var banned = _server.GetUserByUnitUid(req.Value);
                    if (banned != null) banned.CurrentPartyUid = 0;
                    var leaveNot = new KEGS_LEAVE_PARTY_NOT
                    {
                        LeaveUnitUid = req.Value,
                        NewHostUid = ActiveUnit.UnitUid,
                        PartyUid = party.Info.PartyUid,
                        PartyUnitUidList = party.GetUnitUids(),
                    };
                    _server.SendToUnit(req.Value, EventId.EGS_LEAVE_PARTY_NOT, leaveNot);
                    foreach (long uid in party.GetUnitUids())
                        _server.SendToUnit(uid, EventId.EGS_LEAVE_PARTY_NOT, leaveNot);
                }
                break;
            }

            case EventId.EGS_PARTY_CHANGE_NUM_OF_PER_REQ:
            {
                if (ActiveUnit == null || CurrentPartyUid <= 0) break;
                var req = ev.GetPayload<KPacketSByte>(Region);
                var party = _server.Center.PartyManager.GetParty(CurrentPartyUid);
                bool ok = party != null && party.ChangeMaxSlot(ActiveUnit.UnitUid, req.Value);
                SendPacket(EventId.EGS_PARTY_CHANGE_NUM_OF_PER_ACK, new KPacketOK { Ok = ok ? NetError.NET_OK : NetError.ERR_PARTY_09 });
                if (ok && party != null)
                {
                    foreach (long uid in party.GetUnitUids())
                        _server.SendToUnit(uid, EventId.EGS_PARTY_CHANGE_NUM_OF_PER_NOT, req);
                }
                break;
            }

            case EventId.EGS_PARTY_CHANGE_GET_ITEM_TYPE_REQ:
            {
                if (ActiveUnit == null || CurrentPartyUid <= 0) break;
                var req = ev.GetPayload<KPacketSByte>(Region);
                var party = _server.Center.PartyManager.GetParty(CurrentPartyUid);
                bool ok = party != null && party.ChangeGetItemType(ActiveUnit.UnitUid, req.Value);
                SendPacket(EventId.EGS_PARTY_CHANGE_GET_ITEM_TYPE_ACK, new KPacketOK { Ok = ok ? NetError.NET_OK : NetError.ERR_PARTY_09 });
                if (ok && party != null)
                {
                    foreach (long uid in party.GetUnitUids())
                        _server.SendToUnit(uid, EventId.EGS_PARTY_CHANGE_GET_ITEM_TYPE_NOT, req);
                }
                break;
            }

            case EventId.EGS_PARTY_LIMIT_LEVEL_REQ:
            {
                if (ActiveUnit == null || CurrentPartyUid <= 0) break;
                var req = ev.GetPayload<KPacketBool>(Region);
                var party = _server.Center.PartyManager.GetParty(CurrentPartyUid);
                bool ok = party != null && party.SetLimitLevel(ActiveUnit.UnitUid, req.Value);
                SendPacket(EventId.EGS_PARTY_LIMIT_LEVEL_ACK, new KPacketOK { Ok = ok ? NetError.NET_OK : NetError.ERR_PARTY_09 });
                if (ok && party != null)
                {
                    foreach (long uid in party.GetUnitUids())
                        _server.SendToUnit(uid, EventId.EGS_PARTY_LIMIT_LEVEL_NOT, req);
                }
                break;
            }

            // -------- PvP Room Management --------

            case EventId.EGS_CHANGE_MAP_REQ:
            {
                if (ActiveUnit == null || CurrentRoomUid <= 0) break;
                var req = ev.GetPayload<KEGS_CHANGE_MAP_REQ>(Region);
                var room = _server.Center.RoomManager.GetRoom(CurrentRoomUid);
                bool ok = room != null && room.ChangeMap(ActiveUnit.UnitUid, req.WorldId);
                SendPacket(EventId.EGS_CHANGE_MAP_ACK, new KPacketOK { Ok = ok ? NetError.NET_OK : NetError.ERR_ROOM_08 });
                if (ok && room != null)
                {
                    var mapNot = new KEGS_CHANGE_MAP_NOT { WorldId = req.WorldId };
                    foreach (long uid in room.GetMemberUnitUids())
                        _server.SendToUnit(uid, EventId.EGS_CHANGE_MAP_NOT, mapNot);
                }
                break;
            }

            case EventId.EGS_CHANGE_PVP_ROOM_PUBLIC_REQ:
            {
                if (ActiveUnit == null || CurrentRoomUid <= 0) break;
                var req = ev.GetPayload<KEGS_CHANGE_PVP_ROOM_PUBLIC_REQ>(Region);
                var room = _server.Center.RoomManager.GetRoom(CurrentRoomUid);
                bool ok = room != null && room.ChangePublic(ActiveUnit.UnitUid, req.Public, req.Password);
                SendPacket(EventId.EGS_CHANGE_PVP_ROOM_PUBLIC_ACK, new KEGS_CHANGE_PVP_ROOM_PUBLIC_ACK
                {
                    Public = req.Public,
                    Ok = ok ? NetError.NET_OK : NetError.ERR_ROOM_08,
                });
                if (ok && room != null)
                {
                    var pubNot = new KEGS_CHANGE_PVP_ROOM_PUBLIC_NOT { Public = req.Public };
                    foreach (long uid in room.GetMemberUnitUids())
                        _server.SendToUnit(uid, EventId.EGS_CHANGE_PVP_ROOM_PUBLIC_NOT, pubNot);
                }
                break;
            }

            case EventId.EGS_CHANGE_PVP_ROOM_NAME_REQ:
            {
                if (ActiveUnit == null || CurrentRoomUid <= 0) break;
                var req = ev.GetPayload<KEGS_CHANGE_PVP_ROOM_NAME_REQ>(Region);
                var room = _server.Center.RoomManager.GetRoom(CurrentRoomUid);
                bool ok = room != null && room.ChangeRoomName(ActiveUnit.UnitUid, req.RoomName);
                SendPacket(EventId.EGS_CHANGE_PVP_ROOM_NAME_ACK, new KEGS_CHANGE_PVP_ROOM_NAME_ACK
                {
                    RoomName = req.RoomName,
                    Ok = ok ? NetError.NET_OK : NetError.ERR_ROOM_08,
                });
                if (ok && room != null)
                {
                    var nameNot = new KEGS_CHANGE_PVP_ROOM_NAME_NOT { RoomName = req.RoomName };
                    foreach (long uid in room.GetMemberUnitUids())
                        _server.SendToUnit(uid, EventId.EGS_CHANGE_PVP_ROOM_NAME_NOT, nameNot);
                }
                break;
            }

            case EventId.EGS_CHANGE_PLAY_TIME_LIMIT_REQ:
            {
                if (ActiveUnit == null || CurrentRoomUid <= 0) break;
                var req = ev.GetPayload<KEGS_CHANGE_PLAY_TIME_LIMIT_REQ>(Region);
                var room = _server.Center.RoomManager.GetRoom(CurrentRoomUid);
                bool ok = room != null && room.ChangePlayTime(ActiveUnit.UnitUid, req.PlayTime);
                SendPacket(EventId.EGS_CHANGE_PLAY_TIME_LIMIT_ACK, new KPacketOK { Ok = ok ? NetError.NET_OK : NetError.ERR_ROOM_08 });
                if (ok && room != null)
                {
                    var ptNot = new KEGS_CHANGE_PLAY_TIME_LIMIT_NOT { PlayTime = req.PlayTime };
                    foreach (long uid in room.GetMemberUnitUids())
                        _server.SendToUnit(uid, EventId.EGS_CHANGE_PLAY_TIME_LIMIT_NOT, ptNot);
                }
                break;
            }

            case EventId.EGS_CHANGE_PITIN_REQ:
            {
                if (ActiveUnit == null || CurrentRoomUid <= 0) break;
                var req = ev.GetPayload<KEGS_CHANGE_PITIN_REQ>(Region);
                var room = _server.Center.RoomManager.GetRoom(CurrentRoomUid);
                bool ok = room != null && room.SetPitIn(ActiveUnit.UnitUid, req.PitIn);
                SendPacket(EventId.EGS_CHANGE_PITIN_ACK, new KPacketOK { Ok = ok ? NetError.NET_OK : NetError.ERR_ROOM_16 });
                if (ok && room != null)
                {
                    var pitNot = new KEGS_CHANGE_PITIN_NOT { UnitUid = ActiveUnit.UnitUid, PitIn = req.PitIn };
                    foreach (long uid in room.GetMemberUnitUids())
                        _server.SendToUnit(uid, EventId.EGS_CHANGE_PITIN_NOT, pitNot);
                }
                break;
            }

            case EventId.EGS_CHANGE_MOTION_REQ:
            {
                if (ActiveUnit == null || CurrentRoomUid <= 0) break;
                var req = ev.GetPayload<KEGS_CHANGE_MOTION_REQ>(Region);
                var room = _server.Center.RoomManager.GetRoom(CurrentRoomUid);
                SendPacket(EventId.EGS_CHANGE_MOTION_ACK, new KPacketOK { Ok = room != null ? NetError.NET_OK : NetError.ERR_ROOM_16 });
                if (room != null)
                {
                    var motNot = new KEGS_CHANGE_MOTION_NOT { UnitUid = ActiveUnit.UnitUid, MotionId = req.MotionId };
                    foreach (long uid in room.GetMemberUnitUids())
                        _server.SendToUnit(uid, EventId.EGS_CHANGE_MOTION_NOT, motNot);
                }
                break;
            }

            // -------- Pet & Riding Pet Management --------

            case EventId.EGS_GET_PET_LIST_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_GET_PET_LIST_REQ>(Region);
                var pets = _server.Db.GetPets(ActiveUnit.UnitUid);
                PetManager.Init(pets, PetManager.SummonedPetUid);
                int totalPages = Math.Max(1, (pets.Count + KUserPetManager.PePagePerCount - 1) / KUserPetManager.PePagePerCount);
                SendPacket(EventId.EGS_GET_PET_LIST_ACK, new KEGS_GET_PET_LIST_ACK
                {
                    Ok = NetError.NET_OK,
                    TotalPage = totalPages,
                    ViewPage = req.ViewPage,
                    PetList = PetManager.GetPetPage(req.ViewPage),
                });
                break;
            }

            case EventId.EGS_COMMANDS_FOR_PETS_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_COMMANDS_FOR_PETS_REQ>(Region);
                bool hasPet = PetManager.GetSummonedPet() != null;
                SendPacket(EventId.EGS_COMMANDS_FOR_PETS_ACK, new KPacketOK
                {
                    Ok = hasPet && req.ActionType >= 0 ? NetError.NET_OK : NetError.ERR_PET_09,
                });
                break;
            }

            case EventId.EGS_CHANGE_PET_NAME_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_CHANGE_PET_NAME_REQ>(Region);
                var (ok, pet) = PetManager.RenameSummonedPet(req.PetName);
                if (!ok || pet == null)
                {
                    SendPacket(EventId.EGS_CHANGE_PET_NAME_ACK, new KEGS_CHANGE_PET_NAME_ACK { Ok = NetError.ERR_PET_01 });
                    break;
                }
                _server.Db.RenamePet(ActiveUnit.UnitUid, pet.PetUid, req.PetName);
                var updatedSlots = new List<KInventoryItemInfo>();
                if (req.ItemUid > 0)
                {
                    var (delOk, delSlot) = _server.Db.DeleteInventoryItem(ActiveUnit.UnitUid, req.ItemUid, 1);
                    if (delOk) updatedSlots.Add(delSlot);
                }
                SendPacket(EventId.EGS_CHANGE_PET_NAME_ACK, new KEGS_CHANGE_PET_NAME_ACK
                {
                    Ok = NetError.NET_OK,
                    ChangedPetInfo = pet,
                    InventorySlotInfo = updatedSlots,
                });
                break;
            }

            case EventId.EGS_SET_AUTO_FEED_PETS_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_SET_AUTO_FEED_PETS_REQ>(Region);
                var (ok, autoFeed) = PetManager.SetAutoFeedSummonedPet(req.AutoFeed);
                SendPacket(EventId.EGS_SET_AUTO_FEED_PETS_ACK, new KEGS_SET_AUTO_FEED_PETS_ACK
                {
                    Ok = ok ? NetError.NET_OK : NetError.ERR_PET_09,
                    AutoFeed = autoFeed,
                });
                break;
            }

            case EventId.EGS_CREATE_RIDING_PET_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_CREATE_RIDING_PET_REQ>(Region);
                var (itemOk, itemSlot) = _server.Db.GetUnitItemByUid(ActiveUnit.UnitUid, req.ItemUid);
                if (!itemOk)
                {
                    SendPacket(EventId.EGS_CREATE_RIDING_PET_ACK, new KEGS_CREATE_RIDING_PET_ACK { Ok = NetError.ERR_RIDING_PET_01 });
                    break;
                }
                var list = _server.Db.GetRidingPets(ActiveUnit.UnitUid);
                RidingPetManager.Init(list, force: true);
                if (RidingPetManager.RidingPetCreateCheck() != KUserRidingPetManager.UserRidingPetManagerResult.RidingPetSucceed)
                {
                    SendPacket(EventId.EGS_CREATE_RIDING_PET_ACK, new KEGS_CREATE_RIDING_PET_ACK { Ok = NetError.ERR_RIDING_PET_03 });
                    break;
                }
                var created = _server.Db.CreateRidingPet(ActiveUnit.UnitUid, itemSlot.ItemInfo.ItemId);
                RidingPetManager.AddRidingPet(created);
                var (_, consumedSlot) = _server.Db.DeleteInventoryItem(ActiveUnit.UnitUid, req.ItemUid, 1);
                SendPacket(EventId.EGS_CREATE_RIDING_PET_ACK, new KEGS_CREATE_RIDING_PET_ACK
                {
                    Ok = NetError.NET_OK,
                    CreatedPetInfo = created,
                    InventorySlotInfo = [consumedSlot],
                });
                break;
            }

            case EventId.EGS_GET_RIDING_PET_LIST_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_GET_RIDING_PET_LIST_REQ>(Region);
                var list = _server.Db.GetRidingPets(ActiveUnit.UnitUid);
                RidingPetManager.Init(list, force: true);
                var (res, pageItems) = RidingPetManager.GetUserRidingPetList(req.ViewPage);
                bool emptyOrOk = res is KUserRidingPetManager.UserRidingPetManagerResult.RidingPetSucceed
                    or KUserRidingPetManager.UserRidingPetManagerResult.RidingPetEmpty;
                SendPacket(EventId.EGS_GET_RIDING_PET_LIST_ACK, new KEGS_GET_RIDING_PET_LIST_ACK
                {
                    Ok = emptyOrOk ? NetError.NET_OK : NetError.ERR_RIDING_PET_06,
                    RidingPetList = pageItems,
                });
                break;
            }

            case EventId.EGS_RELEASE_RIDING_PET_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_RELEASE_RIDING_PET_REQ>(Region);
                var res = RidingPetManager.ReleaseRidingPet(req.RidingPetUid);
                bool dbOk = res == KUserRidingPetManager.UserRidingPetManagerResult.RidingPetSucceed &&
                    _server.Db.ReleaseRidingPet(ActiveUnit.UnitUid, req.RidingPetUid);
                SendPacket(EventId.EGS_RELEASE_RIDING_PET_ACK, new KEGS_RELEASE_RIDING_PET_ACK
                {
                    Ok = dbOk ? NetError.NET_OK : NetError.ERR_RIDING_PET_06,
                    RidingPetUid = req.RidingPetUid,
                });
                break;
            }

            case EventId.EGS_UNSUMMON_RIDING_PET_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_UNSUMMON_RIDING_PET_REQ>(Region);
                var res = RidingPetManager.UnSummonRidingPet(req.RidingPetUid, req.Stamina);
                bool ok = res == KUserRidingPetManager.UserRidingPetManagerResult.RidingPetSucceed;
                if (ok)
                    _server.Db.UpdateRidingPetStamina(ActiveUnit.UnitUid, req.RidingPetUid, req.Stamina);
                SendPacket(EventId.EGS_UNSUMMON_RIDING_PET_ACK, new KEGS_UNSUMMON_RIDING_PET_ACK
                {
                    Ok = ok ? NetError.NET_OK : NetError.ERR_RIDING_PET_09,
                    RidingPetUid = req.RidingPetUid,
                });
                break;
            }

            case EventId.EGS_UPDATE_RIDING_PET_NOT:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_UPDATE_RIDING_PET_NOT>(Region);
                _server.Db.UpdateRidingPetStamina(ActiveUnit.UnitUid, req.RidingPetUid, req.Stamina);
                break;
            }

            // -------- 2nd Security / MachineId / ClientScript --------

            case EventId.EGS_GET_SECOND_SECURITY_INFO_REQ:
            {
                if (AccountUid <= 0) break;
                var (useSecondPw, _) = _server.Db.GetSecondSecurityStatus(AccountUid);
                SendPacket(EventId.EGS_GET_SECOND_SECURITY_INFO_ACK, new KEGS_SECOND_SECURITY_INFO_NOT
                {
                    Ok = NetError.NET_OK,
                    UseSecondPw = useSecondPw,
                });
                break;
            }

            case EventId.EGS_COMPARE_SECOND_SECURITY_PW_REQ:
            {
                if (AccountUid <= 0) break;
                var req = ev.GetPayload<KPacketString>(Region);
                var (ok, _) = _server.Db.AuthSecondSecurityPin(AccountUid, req.Value);
                if (ok) SecondSecurityVerified = true;
                SendPacket(EventId.EGS_COMPARE_SECOND_SECURITY_PW_ACK, new KPacketOK
                {
                    Ok = ok ? NetError.NET_OK : NetError.ERR_SECOND_SECURITY_DIFFER_PW,
                });
                break;
            }

            case EventId.EGS_CHANGE_SECOND_SECURITY_PW_REQ:
            {
                if (AccountUid <= 0) break;
                var req = ev.GetPayload<KEGS_CHANGE_SECOND_SECURITY_PW_REQ>(Region);
                var (oldOk, _) = _server.Db.AuthSecondSecurityPin(AccountUid, req.OldSecondPw);
                if (!oldOk || string.IsNullOrWhiteSpace(req.NewSecondPw))
                {
                    SendPacket(EventId.EGS_CHANGE_SECOND_SECURITY_PW_ACK, new KPacketOK { Ok = NetError.ERR_SECOND_SECURITY_DIFFER_PW });
                    break;
                }
                _server.Db.CreateSecondSecurityPin(AccountUid, req.NewSecondPw);
                SecondSecurityVerified = true;
                SendPacket(EventId.EGS_CHANGE_SECOND_SECURITY_PW_ACK, new KPacketOK { Ok = NetError.NET_OK });
                break;
            }

            case EventId.EGS_CHECK_SECOND_SECURITY_PW_CHRACTER_LIST_REQ:
            {
                if (AccountUid <= 0) break;
                var req = ev.GetPayload<KPacketString>(Region);
                var (ok, _) = _server.Db.AuthSecondSecurityPin(AccountUid, req.Value);
                if (ok) SecondSecurityVerified = true;
                SendPacket(EventId.EGS_CHECK_SECOND_SECURITY_PW_CHRACTER_LIST_ACK, new KPacketOK
                {
                    Ok = ok ? NetError.NET_OK : NetError.ERR_SECOND_SECURITY_AUTH_FAILED,
                });
                break;
            }

            case EventId.EGS_DELETE_SECOND_SECURITY_REQ:
            {
                if (AccountUid <= 0) break;
                _server.Db.DeleteSecondSecurityPin(AccountUid);
                SecondSecurityVerified = true;
                SendPacket(EventId.EGS_DELETE_SECOND_SECURITY_ACK, new KPacketOK { Ok = NetError.NET_OK });
                break;
            }

            case EventId.EGS_DELETE_SECOND_SECURITY_VERIFY_REQ:
            {
                if (AccountUid <= 0) break;
                var req = ev.GetPayload<KEGS_DELETE_SECOND_SECURITY_VERIFY_REQ>(Region);
                var (ok, _) = _server.Db.AuthSecondSecurityPin(AccountUid, req.SecondPw);
                if (ok && req.Delete)
                {
                    _server.Db.DeleteSecondSecurityPin(AccountUid);
                    SecondSecurityVerified = true;
                }
                SendPacket(EventId.EGS_DELETE_SECOND_SECURITY_VERIFY_ACK, new KPacketOK
                {
                    Ok = ok ? NetError.NET_OK : NetError.ERR_SECOND_SECURITY_DIFFER_PW,
                });
                break;
            }

            case EventId.EGS_CHECK_MACHINE_ID_REQ:
            {
                var req = ev.GetPayload<KEGS_CHECK_MACHINE_ID_REQ>(Region);
                if (Region.MachineIdBlock || _server.GameSysValTable.MachineIdBlock)
                {
                    if (string.IsNullOrWhiteSpace(req.MachineId) || req.MachineId.Length > 24 ||
                        req.MachineId.Any(c => c is '\'' or '"' or ';' or '\\'))
                    {
                        SendPacket(EventId.EGS_SERVER_CHECK_HACK_USER_REQ, new KEGS_SERVER_CHECK_HACK_USER_REQ
                        {
                            HackingCheckType = KEGS_SERVER_CHECK_HACK_USER_REQ.HctCheckMachineId,
                        });
                        SendPacket(EventId.EGS_CHECK_MACHINE_ID_ACK, new KEGS_CHECK_MACHINE_ID_ACK
                        {
                            Ok = NetError.ERR_CHECK_MACHINE_ID_00,
                            MachineId = req.MachineId,
                        });
                        ReserveDestroy();
                        break;
                    }
                    if (_server.Db.IsMachineBlocked(req.MachineId))
                    {
                        SendPacket(EventId.EGS_SERVER_CHECK_HACK_USER_REQ, new KEGS_SERVER_CHECK_HACK_USER_REQ
                        {
                            HackingCheckType = KEGS_SERVER_CHECK_HACK_USER_REQ.HctCheckBlockMachineId,
                        });
                        SendPacket(EventId.EGS_CHECK_MACHINE_ID_ACK, new KEGS_CHECK_MACHINE_ID_ACK
                        {
                            Ok = NetError.ERR_CHECK_MACHINE_ID_00,
                            MachineId = req.MachineId,
                        });
                        ReserveDestroy();
                        break;
                    }
                }
                SendPacket(EventId.EGS_CHECK_MACHINE_ID_ACK, new KEGS_CHECK_MACHINE_ID_ACK
                {
                    Ok = NetError.NET_OK,
                    MachineId = req.MachineId,
                });
                break;
            }

            case EventId.EGS_GET_CLIENT_SCRIPT_REQ:
            {
                var req = ev.GetPayload<KEGS_GET_CLIENT_SCRIPT_REQ>(Region);
                bool hasScript = _server.ClientScripts.TryGetValue(0, out string? scriptStr) && !string.IsNullOrEmpty(scriptStr);
                SendPacket(EventId.EGS_GET_CLIENT_SCRIPT_ACK, new KEGS_GET_CLIENT_SCRIPT_ACK
                {
                    Ok = hasScript ? NetError.NET_OK : NetError.ERR_UNKNOWN,
                    ScriptString = hasScript ? scriptStr! : string.Empty,
                });
                break;
            }

            // -------- Inventory / Bank / QuickSlot / Item Operations --------

            case EventId.EGS_USE_QUICK_SLOT_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_USE_QUICK_SLOT_REQ>(Region);
                var currentRoom = CurrentRoomUid > 0 ? _server.Center.RoomManager.GetRoom(CurrentRoomUid) : null;
                if (currentRoom != null &&
                    KUserDungeonManager.IsHenirDungeon(currentRoom.Info.DungeonId) &&
                    currentRoom.Info.DungeonMode == KUserDungeonManager.ModeHenirChallenge)
                {
                    SendPacket(EventId.EGS_USE_QUICK_SLOT_ACK, new KEGS_USE_QUICK_SLOT_ACK { Ok = NetError.ERR_ITEM_09 });
                    break;
                }
                if (InventoryLocker.IsLocked())
                {
                    SendPacket(EventId.EGS_USE_QUICK_SLOT_ACK, new KEGS_USE_QUICK_SLOT_ACK { Ok = NetError.ERR_INVENTORY_LOCK_00 });
                    break;
                }
                var quickItems = _server.Db.GetUnitCategoryItems(ActiveUnit.UnitUid, 4); // SC_QUICK_SLOT
                var target = quickItems.FirstOrDefault(i => i.SlotId == req.SlotId)
                    ?? _server.Db.GetUnitCategoryItems(ActiveUnit.UnitUid, 1).FirstOrDefault(i => i.SlotId == req.SlotId);
                if (target == null)
                {
                    SendPacket(EventId.EGS_USE_QUICK_SLOT_ACK, new KEGS_USE_QUICK_SLOT_ACK { Ok = NetError.ERR_ITEM_04 });
                    break;
                }
                bool inBattleField = (currentRoom != null && currentRoom.Info.RoomType == 3) || CurrentFieldUid > 0;
                if (inBattleField && BattleFieldBlockedQuickSlotItemIds.Contains(target.ItemInfo.ItemId))
                {
                    SendPacket(EventId.EGS_USE_QUICK_SLOT_ACK, new KEGS_USE_QUICK_SLOT_ACK
                    {
                        Ok = NetError.ERR_USE_ITEM_IN_QUICKSLOT_00,
                        ItemId = target.ItemInfo.ItemId,
                    });
                    break;
                }
                if (QuickSlotCooldownSeconds.TryGetValue(target.ItemInfo.ItemId, out double cdSec) &&
                    cdSec > 0 &&
                    _quickSlotCooldowns.TryGetValue(target.ItemInfo.ItemId, out var lastUseUtc) &&
                    (DateTime.UtcNow - lastUseUtc).TotalSeconds < cdSec)
                {
                    SendPacket(EventId.EGS_USE_QUICK_SLOT_ACK, new KEGS_USE_QUICK_SLOT_ACK
                    {
                        Ok = NetError.ERR_ITEM_08,
                        ItemId = target.ItemInfo.ItemId,
                    });
                    break;
                }
                var (delOk, updatedSlot) = _server.Db.DeleteInventoryItem(ActiveUnit.UnitUid, target.ItemUid, 1);
                SendPacket(EventId.EGS_USE_QUICK_SLOT_ACK, new KEGS_USE_QUICK_SLOT_ACK
                {
                    Ok = delOk ? NetError.NET_OK : NetError.ERR_ITEM_04,
                    InventorySlotInfo = updatedSlot,
                    ItemId = target.ItemInfo.ItemId,
                });
                if (delOk)
                    _quickSlotCooldowns[target.ItemInfo.ItemId] = DateTime.UtcNow;
                if (delOk && CurrentRoomUid > 0)
                {
                    var room = _server.Center.RoomManager.GetRoom(CurrentRoomUid);
                    if (room != null)
                    {
                        var qNot = new KEGS_USE_QUICK_SLOT_NOT { UnitUid = ActiveUnit.UnitUid, ItemId = target.ItemInfo.ItemId };
                        foreach (long uid in room.GetMemberUnitUids())
                            _server.SendToUnit(uid, EventId.EGS_USE_QUICK_SLOT_NOT, qNot);
                    }
                }
                break;
            }

            case EventId.EGS_USE_ITEM_IN_INVENTORY_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_USE_ITEM_IN_INVENTORY_REQ>(Region);
                var (found, slot) = _server.Db.GetUnitItemByUid(ActiveUnit.UnitUid, req.ItemUid);
                if (!found)
                {
                    SendPacket(EventId.EGS_USE_ITEM_IN_INVENTORY_ACK, new KEGS_USE_ITEM_IN_INVENTORY_ACK { Ok = NetError.ERR_USE_ITEM_IN_INVENTORY_00 });
                    break;
                }
                var (_, updated) = _server.Db.DeleteInventoryItem(ActiveUnit.UnitUid, req.ItemUid, 1);
                SendPacket(EventId.EGS_USE_ITEM_IN_INVENTORY_ACK, new KEGS_USE_ITEM_IN_INVENTORY_ACK
                {
                    Ok = NetError.NET_OK,
                    InventorySlotInfo = [updated],
                    UsedItemId = slot.ItemInfo.ItemId,
                    Ed = ActiveUnit.Ed,
                    TempCode = req.TempCode,
                });
                break;
            }

            case EventId.EGS_OPEN_RANDOM_ITEM_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_OPEN_RANDOM_ITEM_REQ>(Region);
                if (_server.GameSysValTable.IsEnableOpenRandomCubeDelaySystem && !req.DelayedOk)
                {
                    _server.EventDelaySystemManager.RegOpenRandomCubeEvent(ActiveUnit.UnitUid, req);
                    break;
                }
                if (!InventoryLocker.Lock(KInventoryLocker.LockType.TimeOut, 30.0))
                {
                    SendPacket(EventId.EGS_OPEN_RANDOM_ITEM_ACK, new KEGS_OPEN_RANDOM_ITEM_ACK { Ok = NetError.ERR_INVENTORY_LOCK_00 });
                    break;
                }
                try
                {
                    if (IsTradeOrShopOpen())
                    {
                        SendPacket(EventId.EGS_OPEN_RANDOM_ITEM_ACK, new KEGS_OPEN_RANDOM_ITEM_ACK { Ok = NetError.ERR_ITEM_14 });
                        break;
                    }
                    var (found, cubeSlot) = _server.Db.GetUnitItemByUid(ActiveUnit.UnitUid, req.ItemUid);
                    if (!found || cubeSlot.SlotCategory == 9)
                    {
                        SendPacket(EventId.EGS_OPEN_RANDOM_ITEM_ACK, new KEGS_OPEN_RANDOM_ITEM_ACK { Ok = NetError.ERR_ITEM_10 });
                        break;
                    }

                    int cubeId = cubeSlot.ItemInfo.ItemId;
                    int effectiveCubeId = _server.GameSysValTable.ResolveAttractionItemTimeEvent(cubeId);
                    var cubeData = _server.RandomItemManager.GetRandomItemData(effectiveCubeId);
                    if (cubeData != null)
                    {
                        if (cubeData.KeyItemId > 0 && cubeData.RequiredKeyCount > 0 &&
                            _server.Db.GetTotalItemCountByItemId(ActiveUnit.UnitUid, cubeData.KeyItemId) < cubeData.RequiredKeyCount)
                        {
                            SendPacket(EventId.EGS_OPEN_RANDOM_ITEM_ACK, new KEGS_OPEN_RANDOM_ITEM_ACK { Ok = NetError.ERR_RANDOM_ITEM_04 });
                            break;
                        }
                        if (cubeData.RequiredEd > 0 && ActiveUnit.Ed < cubeData.RequiredEd)
                        {
                            SendPacket(EventId.EGS_OPEN_RANDOM_ITEM_ACK, new KEGS_OPEN_RANDOM_ITEM_ACK { Ok = NetError.ERR_RANDOM_ITEM_05, Ed = ActiveUnit.Ed });
                            break;
                        }
                    }

                    var (_, consumed) = _server.Db.DeleteInventoryItem(ActiveUnit.UnitUid, req.ItemUid, 1);
                    ItemQuantityVariation.DecreaseQuantity(cubeId, 1);

                    var insertedMap = new Dictionary<int, int>();
                    var slotList = new List<KInventoryItemInfo> { consumed };

                    if (cubeData != null && cubeData.KeyItemId > 0 && cubeData.RequiredKeyCount > 0)
                    {
                        var (_, keySlots) = _server.Db.TryConsumeItemsByItemId(ActiveUnit.UnitUid, cubeData.KeyItemId, cubeData.RequiredKeyCount);
                        ItemQuantityVariation.DecreaseQuantity(cubeData.KeyItemId, cubeData.RequiredKeyCount);
                        slotList.AddRange(keySlots);
                    }
                    if (cubeData != null && cubeData.RequiredEd > 0 &&
                        EdManager.TryAddEd(-cubeData.RequiredEd, KUserEDManager.EdReason.ErNoReason, out int afterEd))
                    {
                        _server.Db.UpdateUnitProgress(ActiveUnit.UnitUid, 0, -cubeData.RequiredEd, 0);
                        ActiveUnit.Ed = afterEd;
                    }

                    bool markSealed = _server.RandomItemManager.IsSealRandomItem(effectiveCubeId);
                    var (rmOk, rmRewards, resCnt, spRestore) = _server.RandomItemManager.OpenRandomItem(
                        (sbyte)ActiveUnit.UnitClass, effectiveCubeId);
                    if (rmOk == NetError.NET_OK && (rmRewards.Count > 0 || resCnt > 0 || spRestore > 0))
                    {
                        if (resCnt > 0) NumResurrectionStone += resCnt;
                        if (spRestore > 0) SpiritManager.RestoreSpirit(spRestore);
                        foreach (var rew in rmRewards)
                        {
                            var granted = _server.Db.GrantUnitItem(ActiveUnit.UnitUid, rew.ItemId, rew.Quantity);
                            if (markSealed)
                                granted = _server.Db.SetItemSealed(ActiveUnit.UnitUid, granted.ItemUid, true);
                            ItemQuantityVariation.IncreaseQuantity(rew.ItemId, rew.Quantity);
                            insertedMap[rew.ItemId] = insertedMap.GetValueOrDefault(rew.ItemId) + rew.Quantity;
                            slotList.Add(granted);
                        }
                    }
                    else
                    {
                        var (dropped, rewardId, rewardQty) = _server.Center.DropTable.RollMonsterDrop(cubeId);
                        int outItemId = dropped ? rewardId : 130001;
                        int outQty = dropped ? Math.Max(1, rewardQty) : 1;
                        var granted = _server.Db.GrantUnitItem(ActiveUnit.UnitUid, outItemId, outQty);
                        if (markSealed)
                            granted = _server.Db.SetItemSealed(ActiveUnit.UnitUid, granted.ItemUid, true);
                        ItemQuantityVariation.IncreaseQuantity(outItemId, outQty);
                        insertedMap[outItemId] = outQty;
                        slotList.Add(granted);
                    }
                    SendPacket(EventId.EGS_OPEN_RANDOM_ITEM_ACK, new KEGS_OPEN_RANDOM_ITEM_ACK
                    {
                        Ok = NetError.NET_OK,
                        RessurectionCount = resCnt,
                        RestoreSpirit = spRestore,
                        InsertedItem = insertedMap,
                        InventorySlotInfo = slotList,
                        Ed = ActiveUnit.Ed,
                    });
                }
                finally
                {
                    InventoryLocker.Unlock();
                }
                break;
            }

            case EventId.EGS_IDENTIFY_ITEM_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_IDENTIFY_ITEM_REQ>(Region);
                if (!InventoryLocker.Lock(KInventoryLocker.LockType.TimeOut, 30.0))
                {
                    SendPacket(EventId.EGS_IDENTIFY_ITEM_ACK, new KEGS_IDENTIFY_ITEM_ACK { Ok = NetError.ERR_INVENTORY_LOCK_00 });
                    break;
                }
                try
                {
                    if (IsTradeOrShopOpen())
                    {
                        SendPacket(EventId.EGS_IDENTIFY_ITEM_ACK, new KEGS_IDENTIFY_ITEM_ACK { Ok = NetError.ERR_ITEM_14 });
                        break;
                    }
                    var (found, slot) = _server.Db.GetUnitItemByUid(ActiveUnit.UnitUid, req.ItemUid);
                    if (!found || slot.SlotCategory == 9)
                    {
                        SendPacket(EventId.EGS_IDENTIFY_ITEM_ACK, new KEGS_IDENTIFY_ITEM_ACK { Ok = NetError.ERR_ITEM_10 });
                        break;
                    }
                    int qty = Math.Clamp(req.Quantity, 1, Math.Max(1, slot.ItemInfo.Quantity));
                    int stoneId = _server.SocketItemManager.GetIdentifyStoneItemId(slot.ItemInfo.ItemId);
                    var (stoneOk, stoneSlots) = _server.Db.TryConsumeItemsByItemId(ActiveUnit.UnitUid, stoneId, qty);
                    if (!stoneOk)
                    {
                        SendPacket(EventId.EGS_IDENTIFY_ITEM_ACK, new KEGS_IDENTIFY_ITEM_ACK { Ok = NetError.ERR_ITEM_08 });
                        break;
                    }

                    var (_, targetUpdated) = _server.Db.DeleteInventoryItem(ActiveUnit.UnitUid, req.ItemUid, qty);
                    ItemQuantityVariation.DecreaseQuantity(slot.ItemInfo.ItemId, qty);
                    ItemQuantityVariation.DecreaseQuantity(stoneId, qty);

                    var changedSlots = new List<KInventoryItemInfo>(stoneSlots) { targetUpdated };
                    var insertedMap = new Dictionary<int, int>();
                    for (int i = 0; i < qty; i++)
                    {
                        int rolledId = _server.SocketItemManager.RollIdentifyResult(slot.ItemInfo.ItemId);
                        if (rolledId <= 0)
                            rolledId = KSocketItemManager.GetShardItemIdForElement(
                                _server.SocketItemManager.RollOrResolveAttribute(KSocketItemManager.AtiUnknown, 0));
                        insertedMap[rolledId] = insertedMap.GetValueOrDefault(rolledId) + 1;
                    }
                    foreach (var kv in insertedMap)
                    {
                        var granted = _server.Db.GrantUnitItem(ActiveUnit.UnitUid, kv.Key, kv.Value);
                        ItemQuantityVariation.IncreaseQuantity(kv.Key, kv.Value);
                        changedSlots.Add(granted);
                    }

                    SendPacket(EventId.EGS_IDENTIFY_ITEM_ACK, new KEGS_IDENTIFY_ITEM_ACK
                    {
                        Ok = NetError.NET_OK,
                        InsertedItem = insertedMap,
                        InventorySlotInfo = changedSlots,
                    });
                }
                finally
                {
                    InventoryLocker.Unlock();
                }
                break;
            }

            case EventId.EGS_SEAL_ITEM_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_SEAL_ITEM_REQ>(Region);
                if (!InventoryLocker.TryLock(out int lockErr))
                {
                    SendPacket(EventId.EGS_SEAL_ITEM_ACK, new KEGS_SEAL_ITEM_ACK { Ok = lockErr });
                    break;
                }
                try
                {
                    if (IsTradeOrShopOpen())
                    {
                        SendPacket(EventId.EGS_SEAL_ITEM_ACK, new KEGS_SEAL_ITEM_ACK { Ok = NetError.ERR_ITEM_14 });
                        break;
                    }
                    var (found, destSlot, isBroken, isSealed) = _server.Db.GetItemFullDetails(ActiveUnit.UnitUid, req.DestItemUid);
                    if (!found || isBroken || isSealed)
                    {
                        SendPacket(EventId.EGS_SEAL_ITEM_ACK, new KEGS_SEAL_ITEM_ACK { Ok = isSealed ? NetError.ERR_ITEM_00 : (isBroken ? NetError.ERR_RESTORE_ITEM_06 : NetError.ERR_SEAL_ITEM_00) });
                        break;
                    }
                    var updated = new List<KInventoryItemInfo>();
                    if (req.SealItemUid > 0)
                    {
                        var (sFound, sSlot) = _server.Db.GetUnitItemByUid(ActiveUnit.UnitUid, req.SealItemUid);
                        if (!sFound || sSlot.ItemInfo.Quantity <= 0)
                        {
                            SendPacket(EventId.EGS_SEAL_ITEM_ACK, new KEGS_SEAL_ITEM_ACK { Ok = NetError.ERR_ITEM_08 });
                            break;
                        }
                        var (sOk, delSlot) = _server.Db.DeleteInventoryItem(ActiveUnit.UnitUid, req.SealItemUid, 1);
                        if (sOk) updated.Add(delSlot);
                    }
                    var sealedSlot = _server.Db.SetItemSealed(ActiveUnit.UnitUid, req.DestItemUid, true);
                    updated.Insert(0, sealedSlot);
                    SendPacket(EventId.EGS_SEAL_ITEM_ACK, new KEGS_SEAL_ITEM_ACK
                    {
                        Ok = NetError.NET_OK,
                        InventorySlotInfo = updated,
                    });
                }
                finally { InventoryLocker.Unlock(); }
                break;
            }

            case EventId.EGS_UNSEAL_ITEM_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_UNSEAL_ITEM_REQ>(Region);
                if (!InventoryLocker.TryLock(out int lockErr))
                {
                    SendPacket(EventId.EGS_UNSEAL_ITEM_ACK, new KEGS_SEAL_ITEM_ACK { Ok = lockErr });
                    break;
                }
                try
                {
                    if (IsTradeOrShopOpen())
                    {
                        SendPacket(EventId.EGS_UNSEAL_ITEM_ACK, new KEGS_SEAL_ITEM_ACK { Ok = NetError.ERR_ITEM_14 });
                        break;
                    }
                    var (found, destSlot, _, isSealed) = _server.Db.GetItemFullDetails(ActiveUnit.UnitUid, req.DestItemUid);
                    if (!found)
                    {
                        SendPacket(EventId.EGS_UNSEAL_ITEM_ACK, new KEGS_SEAL_ITEM_ACK { Ok = NetError.ERR_SEAL_ITEM_00 });
                        break;
                    }
                    var unsealed = isSealed ? _server.Db.SetItemSealed(ActiveUnit.UnitUid, req.DestItemUid, false) : destSlot;
                    SendPacket(EventId.EGS_UNSEAL_ITEM_ACK, new KEGS_SEAL_ITEM_ACK
                    {
                        Ok = NetError.NET_OK,
                        InventorySlotInfo = [unsealed],
                    });
                }
                finally { InventoryLocker.Unlock(); }
                break;
            }

            case EventId.EGS_ATTRIB_ATTACH_ITEM_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_ATTRIB_ATTACH_ITEM_REQ>(Region);
                if (!InventoryLocker.TryLock(out int lockErr))
                {
                    SendPacket(EventId.EGS_ATTRIB_ATTACH_ITEM_ACK, new KEGS_ATTRIB_ATTACH_ITEM_ACK { Ok = lockErr });
                    break;
                }
                try
                {
                    if (IsTradeOrShopOpen())
                    {
                        SendPacket(EventId.EGS_ATTRIB_ATTACH_ITEM_ACK, new KEGS_ATTRIB_ATTACH_ITEM_ACK { Ok = NetError.ERR_ITEM_14 });
                        break;
                    }
                    var (found, destSlot) = _server.Db.GetUnitItemByUid(ActiveUnit.UnitUid, req.DestItemUid);
                    if (!found)
                    {
                        SendPacket(EventId.EGS_ATTRIB_ATTACH_ITEM_ACK, new KEGS_ATTRIB_ATTACH_ITEM_ACK { Ok = NetError.ERR_ATTRIB_ENCHANT_00 });
                        break;
                    }
                    var slot0 = KSocketItemManager.EnchantElement.Blaze;
                    var slot1 = KSocketItemManager.EnchantElement.None;
                    var slot2 = KSocketItemManager.EnchantElement.None;
                    bool hasScrollMapping = false;
                    var updated = new List<KInventoryItemInfo>();
                    if (req.AttachItemUid > 0)
                    {
                        var (aFound, attachSlot) = _server.Db.GetUnitItemByUid(ActiveUnit.UnitUid, req.AttachItemUid);
                        if (!aFound || attachSlot.ItemInfo.Quantity <= 0)
                        {
                            SendPacket(EventId.EGS_ATTRIB_ATTACH_ITEM_ACK, new KEGS_ATTRIB_ATTACH_ITEM_ACK { Ok = NetError.ERR_ATTRIB_ENCHANT_01 });
                            break;
                        }
                        if (_server.SocketItemManager.TryGetAttribAttachInfo(attachSlot.ItemInfo.ItemId, out var attachData))
                        {
                            hasScrollMapping = true;
                            slot0 = attachData.Slot0;
                            slot1 = attachData.Slot1;
                            slot2 = attachData.Slot2;
                        }
                        else
                        {
                            slot0 = attachSlot.ItemInfo.ItemId switch
                            {
                                KSocketItemManager.AtiRed => KSocketItemManager.EnchantElement.Blaze,
                                KSocketItemManager.AtiBlue => KSocketItemManager.EnchantElement.Water,
                                KSocketItemManager.AtiGreen => KSocketItemManager.EnchantElement.Nature,
                                KSocketItemManager.AtiWind => KSocketItemManager.EnchantElement.Wind,
                                KSocketItemManager.AtiLight => KSocketItemManager.EnchantElement.Light,
                                KSocketItemManager.AtiDark => KSocketItemManager.EnchantElement.Dark,
                                _ => (KSocketItemManager.EnchantElement)(((attachSlot.ItemInfo.ItemId % 6) + 1)),
                            };
                        }
                        var (aOk, aSlot) = _server.Db.DeleteInventoryItem(ActiveUnit.UnitUid, req.AttachItemUid, 1);
                        if (aOk) updated.Add(aSlot);
                    }
                    var dbAttribSlot = _server.Db.UpdateItemAttrib(ActiveUnit.UnitUid, req.DestItemUid, 0, (sbyte)slot0);
                    if (hasScrollMapping)
                    {
                        if (slot1 != KSocketItemManager.EnchantElement.None)
                            dbAttribSlot = _server.Db.UpdateItemAttrib(ActiveUnit.UnitUid, req.DestItemUid, 1, (sbyte)slot1);
                        if (slot2 != KSocketItemManager.EnchantElement.None)
                            dbAttribSlot = _server.Db.UpdateItemAttrib(ActiveUnit.UnitUid, req.DestItemUid, 2, (sbyte)slot2);
                    }
                    updated.Insert(0, dbAttribSlot);
                    SendPacket(EventId.EGS_ATTRIB_ATTACH_ITEM_ACK, new KEGS_ATTRIB_ATTACH_ITEM_ACK
                    {
                        Ok = NetError.NET_OK,
                        InventorySlotInfo = updated,
                    });
                }
                finally { InventoryLocker.Unlock(); }
                break;
            }

            case EventId.EGS_ENCHANT_ATTACH_ITEM_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_ENCHANT_ATTACH_ITEM_REQ>(Region);
                if (!InventoryLocker.TryLock(out int lockErr))
                {
                    SendPacket(EventId.EGS_ENCHANT_ATTACH_ITEM_ACK, new KEGS_ENCHANT_ATTACH_ITEM_ACK { Ok = lockErr });
                    break;
                }
                try
                {
                    if (IsTradeOrShopOpen())
                    {
                        SendPacket(EventId.EGS_ENCHANT_ATTACH_ITEM_ACK, new KEGS_ENCHANT_ATTACH_ITEM_ACK { Ok = NetError.ERR_ITEM_14 });
                        break;
                    }
                    var (found, destSlot, isBroken, _) = _server.Db.GetItemFullDetails(ActiveUnit.UnitUid, req.DestItemUid);
                    if (!found || isBroken)
                    {
                        SendPacket(EventId.EGS_ENCHANT_ATTACH_ITEM_ACK, new KEGS_ENCHANT_ATTACH_ITEM_ACK { Ok = isBroken ? NetError.ERR_ENCHANT_ITEM_06 : NetError.ERR_ENCHANT_ITEM_00 });
                        break;
                    }
                    sbyte targetLv = (sbyte)Math.Min(12, Math.Max(destSlot.ItemInfo.EnchantLevel + 1, 7));
                    var updated = new List<KInventoryItemInfo>();
                    if (req.AttachItemUid > 0)
                    {
                        var (aFound, attachSlot) = _server.Db.GetUnitItemByUid(ActiveUnit.UnitUid, req.AttachItemUid);
                        if (!aFound || attachSlot.ItemInfo.Quantity <= 0)
                        {
                            SendPacket(EventId.EGS_ENCHANT_ATTACH_ITEM_ACK, new KEGS_ENCHANT_ATTACH_ITEM_ACK { Ok = NetError.ERR_ENCHANT_ITEM_03 });
                            break;
                        }
                        if (_server.EnchantItemManager.TryGetAttachItemInfo(attachSlot.ItemInfo.ItemId, out var attachInfo))
                        {
                            if (destSlot.ItemInfo.EnchantLevel >= attachInfo.EnchantLevel)
                            {
                                SendPacket(EventId.EGS_ENCHANT_ATTACH_ITEM_ACK, new KEGS_ENCHANT_ATTACH_ITEM_ACK { Ok = NetError.ERR_ENCHANT_ITEM_04 });
                                break;
                            }
                            targetLv = (sbyte)attachInfo.EnchantLevel;
                        }
                        var (aOk, aSlot) = _server.Db.DeleteInventoryItem(ActiveUnit.UnitUid, req.AttachItemUid, 1);
                        if (aOk) updated.Add(aSlot);
                    }
                    var dbEncSlot = _server.Db.UpdateItemEnchant(ActiveUnit.UnitUid, req.DestItemUid, (byte)Math.Clamp((int)targetLv, 0, byte.MaxValue), false);
                    updated.Insert(0, dbEncSlot);
                    SendPacket(EventId.EGS_ENCHANT_ATTACH_ITEM_ACK, new KEGS_ENCHANT_ATTACH_ITEM_ACK
                    {
                        Ok = NetError.NET_OK,
                        InventorySlotInfo = updated,
                    });
                }
                finally { InventoryLocker.Unlock(); }
                break;
            }

            case EventId.EGS_SOCKET_EXPAND_ITEM_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_SOCKET_EXPAND_ITEM_REQ>(Region);
                if (!InventoryLocker.TryLock(out int lockErr))
                {
                    SendPacket(EventId.EGS_SOCKET_EXPAND_ITEM_ACK, new KEGS_SOCKET_EXPAND_ITEM_ACK { Ok = lockErr });
                    break;
                }
                try
                {
                    if (IsTradeOrShopOpen())
                    {
                        SendPacket(EventId.EGS_SOCKET_EXPAND_ITEM_ACK, new KEGS_SOCKET_EXPAND_ITEM_ACK { Ok = NetError.ERR_ITEM_14 });
                        break;
                    }
                    var (found, targetSlot, isBroken, _) = _server.Db.GetItemFullDetails(ActiveUnit.UnitUid, req.TargetItemUid);
                    if (!found)
                    {
                        SendPacket(EventId.EGS_SOCKET_EXPAND_ITEM_ACK, new KEGS_SOCKET_EXPAND_ITEM_ACK { Ok = NetError.ERR_FIELD_BOSS_08 });
                        break;
                    }
                    if (isBroken)
                    {
                        SendPacket(EventId.EGS_SOCKET_EXPAND_ITEM_ACK, new KEGS_SOCKET_EXPAND_ITEM_ACK { Ok = NetError.ERR_FIELD_BOSS_10 });
                        break;
                    }
                    var updated = new List<KInventoryItemInfo>();
                    if (req.MaterialItemUid > 0)
                    {
                        var (mOk, mSlot) = _server.Db.DeleteInventoryItem(ActiveUnit.UnitUid, req.MaterialItemUid, 1);
                        if (!mOk)
                        {
                            SendPacket(EventId.EGS_SOCKET_EXPAND_ITEM_ACK, new KEGS_SOCKET_EXPAND_ITEM_ACK { Ok = NetError.ERR_ITEM_04 });
                            break;
                        }
                        updated.Add(mSlot);
                    }
                    targetSlot.ItemInfo.ItemSockets.Add(0);
                    targetSlot.ItemInfo.ExpandedSocketNum++;
                    var dbSocketSlot = _server.Db.UpdateItemSockets(ActiveUnit.UnitUid, req.TargetItemUid, targetSlot.ItemInfo.ItemSockets);
                    updated.Insert(0, dbSocketSlot);
                    SendPacket(EventId.EGS_SOCKET_EXPAND_ITEM_ACK, new KEGS_SOCKET_EXPAND_ITEM_ACK
                    {
                        Ok = NetError.NET_OK,
                        UpdatedInventorySlot = updated,
                    });
                }
                finally { InventoryLocker.Unlock(); }
                break;
            }

            case EventId.EGS_SORT_CATEGORY_ITEM_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_SORT_CATEGORY_ITEM_REQ>(Region);
                var items = _server.Db.GetUnitCategoryItems(ActiveUnit.UnitUid, (sbyte)req.CategoryType);
                SendPacket(EventId.EGS_SORT_CATEGORY_ITEM_ACK, new KEGS_SORT_CATEGORY_ITEM_ACK
                {
                    CategoryType = req.CategoryType,
                    UpdatedInventorySlot = items,
                    Ok = NetError.NET_OK,
                });
                break;
            }

            case EventId.EGS_ITEM_EXCHANGE_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_ITEM_EXCHANGE_REQ>(Region);
                if (!InventoryLocker.Lock(KInventoryLocker.LockType.TimeOut, 30.0))
                {
                    SendPacket(EventId.EGS_ITEM_EXCHANGE_ACK, new KEGS_ITEM_EXCHANGE_ACK { Ok = NetError.ERR_INVENTORY_LOCK_00 });
                    break;
                }
                try
                {
                    var updated = new List<KInventoryItemInfo>();
                    int srcQty = Math.Max(1, req.SourceQuantity);
                    KItemInfo? srcItemInfo = null;
                    int resolvedSrcItemId = req.SourceItemId;
                    if (req.SourceItemUid > 0)
                    {
                        var (srcFound, srcSlot) = _server.Db.GetUnitItemByUid(ActiveUnit.UnitUid, req.SourceItemUid);
                        if (srcFound)
                        {
                            srcItemInfo = srcSlot.ItemInfo;
                            if (resolvedSrcItemId <= 0) resolvedSrcItemId = srcSlot.ItemInfo.ItemId;
                        }
                        var (delOk, delSlot) = _server.Db.DeleteInventoryItem(ActiveUnit.UnitUid, req.SourceItemUid, srcQty);
                        if (!delOk)
                        {
                            SendPacket(EventId.EGS_ITEM_EXCHANGE_ACK, new KEGS_ITEM_EXCHANGE_ACK { Ok = NetError.ERR_ITEM_EXCHANGE_01 });
                            break;
                        }
                        if (resolvedSrcItemId > 0) ItemQuantityVariation.DecreaseQuantity(resolvedSrcItemId, srcQty);
                        updated.Add(delSlot);
                    }

                    var resultMap = new Dictionary<int, int>();
                    bool clearOldDefenceSockets = _server.Center.DefenceDungeonManager.IsOldDefenceItemId(resolvedSrcItemId);
                    if (_server.ItemExchangeManager.TryGetExchangeResult(
                        req.HouseId, resolvedSrcItemId, srcQty, srcItemInfo, req.DestItemId,
                        out var exchangeResults, out _, out _))
                    {
                        foreach (var kv in exchangeResults)
                        {
                            var granted = _server.Db.GrantUnitItem(ActiveUnit.UnitUid, kv.Key, kv.Value.Quantity);
                            if (clearOldDefenceSockets && granted.ItemInfo.ItemSockets.Count > 0)
                                granted = _server.Db.UpdateItemSockets(ActiveUnit.UnitUid, granted.ItemUid, Array.Empty<int>());
                            ItemQuantityVariation.IncreaseQuantity(kv.Key, kv.Value.Quantity);
                            updated.Add(granted);
                            resultMap[kv.Key] = resultMap.GetValueOrDefault(kv.Key) + kv.Value.Quantity;
                        }
                    }
                    else
                    {
                        int destId = req.DestItemId > 0 ? req.DestItemId : 130001;
                        var granted = _server.Db.GrantUnitItem(ActiveUnit.UnitUid, destId, 1);
                        if (clearOldDefenceSockets && granted.ItemInfo.ItemSockets.Count > 0)
                            granted = _server.Db.UpdateItemSockets(ActiveUnit.UnitUid, granted.ItemUid, Array.Empty<int>());
                        ItemQuantityVariation.IncreaseQuantity(destId, 1);
                        updated.Add(granted);
                        resultMap[destId] = 1;
                    }

                    ActiveUnit.ExchangeCount++;
                    SendPacket(EventId.EGS_ITEM_EXCHANGE_ACK, new KEGS_ITEM_EXCHANGE_ACK
                    {
                        Ok = NetError.NET_OK,
                        InventorySlotInfo = updated,
                        ResultItem = resultMap,
                        ExchangeCount = ActiveUnit.ExchangeCount,
                    });
                }
                finally
                {
                    InventoryLocker.Unlock();
                }
                break;
            }

            case EventId.EGS_GET_MY_BANK_INFO_REQ:
            {
                if (ActiveUnit == null) break;
                var bankItems = _server.Db.GetUnitCategoryItems(ActiveUnit.UnitUid, 9); // SC_BANK
                SendPacket(EventId.EGS_GET_MY_BANK_INFO_ACK, new KEGS_GET_MY_BANK_INFO_ACK
                {
                    MapItem = bankItems.ToDictionary(i => i.ItemUid, i => i),
                });
                break;
            }

            case EventId.EGS_EXPAND_INVENTORY_ED_REQ:
            {
                if (ActiveUnit == null) break;
                var (curGrade, _) = _server.Db.GetUnitExpandProgress(ActiveUnit.UnitUid);
                var (canUpgrade, nextGrade, configuredCost) = _server.EdInventoryExpand.GetInventoryUpgradeForCurrentGrade(curGrade);
                if (!canUpgrade && curGrade >= KEdInventoryExpand.MaxInventoryUpgradeGrade)
                {
                    SendPacket(EventId.EGS_EXPAND_INVENTORY_ED_ACK, new KEGS_EXPAND_INVENTORY_ED_ACK { Ok = NetError.ERR_EXPAND_INVENTORY_ED_02, Ed = ActiveUnit.Ed });
                    break;
                }
                int expandCost = configuredCost > 0 ? configuredCost : 100_000;
                if (!EdManager.TryAddEd(-expandCost, KUserEDManager.EdReason.ErNoReason, out int afterEd))
                {
                    SendPacket(EventId.EGS_EXPAND_INVENTORY_ED_ACK, new KEGS_EXPAND_INVENTORY_ED_ACK { Ok = NetError.ERR_EXPAND_INVENTORY_ED_01, Ed = ActiveUnit.Ed });
                    break;
                }
                _server.Db.UpdateUnitProgress(ActiveUnit.UnitUid, 0, -expandCost, 0);
                _server.Db.SetUnitInventoryExpandGrade(ActiveUnit.UnitUid, nextGrade);
                ActiveUnit.Ed = afterEd;
                SendPacket(EventId.EGS_EXPAND_INVENTORY_ED_ACK, new KEGS_EXPAND_INVENTORY_ED_ACK
                {
                    Ok = NetError.NET_OK,
                    Ed = afterEd,
                    ExpandedCategorySlot = new Dictionary<int, int> { [1] = 8 * nextGrade },
                });
                break;
            }

            case EventId.EGS_EXPAND_BANK_SLOT_ED_REQ:
            {
                if (ActiveUnit == null) break;
                var (_, curBankSlots) = _server.Db.GetUnitExpandProgress(ActiveUnit.UnitUid);
                var (canUpgrade, nextSlots, configuredBankCost) = _server.EdInventoryExpand.GetBankUpgradeForCurrentSlots(curBankSlots);
                if (!canUpgrade && curBankSlots >= KEdInventoryExpand.MaxBankSlots)
                {
                    SendPacket(EventId.EGS_EXPAND_BANK_SLOT_ED_ACK, new KEGS_EXPAND_BANK_SLOT_ED_ACK { Ok = NetError.ERR_EXPAND_INVENTORY_ED_02, Ed = ActiveUnit.Ed });
                    break;
                }
                int bankExpandCost = configuredBankCost > 0 ? configuredBankCost : 200_000;
                if (!EdManager.TryAddEd(-bankExpandCost, KUserEDManager.EdReason.ErNoReason, out int afterEd))
                {
                    SendPacket(EventId.EGS_EXPAND_BANK_SLOT_ED_ACK, new KEGS_EXPAND_BANK_SLOT_ED_ACK { Ok = NetError.ERR_EXPAND_INVENTORY_ED_01, Ed = ActiveUnit.Ed });
                    break;
                }
                _server.Db.UpdateUnitProgress(ActiveUnit.UnitUid, 0, -bankExpandCost, 0);
                _server.Db.SetUnitBankSlotCount(ActiveUnit.UnitUid, nextSlots);
                ActiveUnit.Ed = afterEd;
                SendPacket(EventId.EGS_EXPAND_BANK_SLOT_ED_ACK, new KEGS_EXPAND_BANK_SLOT_ED_ACK
                {
                    Ok = NetError.NET_OK,
                    Ed = afterEd,
                    ExpandedCategorySlot = new Dictionary<int, int> { [9] = nextSlots },
                });
                break;
            }

            case EventId.EGS_GET_NEXT_INVENTORY_ED_REQ:
            {
                if (ActiveUnit == null) break;
                var (curGrade, _) = _server.Db.GetUnitExpandProgress(ActiveUnit.UnitUid);
                var (_, _, configuredCost) = _server.EdInventoryExpand.GetInventoryUpgradeForCurrentGrade(curGrade);
                SendPacket(EventId.EGS_GET_NEXT_INVENTORY_ED_ACK, new KEGS_GET_NEXT_BANK_ED_ACK
                {
                    Ok = NetError.NET_OK,
                    Ed = configuredCost > 0 ? configuredCost : 100_000,
                });
                break;
            }

            case EventId.EGS_GET_NEXT_BANK_ED_REQ:
            {
                if (ActiveUnit == null) break;
                var (_, curBankSlots) = _server.Db.GetUnitExpandProgress(ActiveUnit.UnitUid);
                var (_, _, configuredBankCost) = _server.EdInventoryExpand.GetBankUpgradeForCurrentSlots(curBankSlots);
                SendPacket(EventId.EGS_GET_NEXT_BANK_ED_ACK, new KEGS_GET_NEXT_BANK_ED_ACK
                {
                    Ok = NetError.NET_OK,
                    Ed = configuredBankCost > 0 ? configuredBankCost : 200_000,
                });
                break;
            }

            // -------- Personal Shop / Agency / Trade Board --------

            case EventId.EGS_JOIN_PERSONAL_SHOP_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_JOIN_PERSONAL_SHOP_REQ>(Region);
                if (IsGuestUser)
                {
                    SendPacket(EventId.EGS_JOIN_PERSONAL_SHOP_ACK, new KEGS_JOIN_PERSONAL_SHOP_ACK { Ok = NetError.ERR_GUEST_USER_00 });
                    break;
                }
                if (_server.GameSysValTable.BlockPersonalShop)
                {
                    SendPacket(EventId.EGS_JOIN_PERSONAL_SHOP_ACK, new KEGS_JOIN_PERSONAL_SHOP_ACK { Ok = NetError.ERR_BLOCK_TRADE_00 });
                    break;
                }
                if (AbuserManager.IsTradeBlockUser)
                {
                    SendPacket(EventId.EGS_JOIN_PERSONAL_SHOP_ACK, new KEGS_JOIN_PERSONAL_SHOP_ACK { Ok = NetError.ERR_BLOCK_TRADE_07 });
                    break;
                }
                if (AbuserManager.IsTradeBlockUnit)
                {
                    SendPacket(EventId.EGS_JOIN_PERSONAL_SHOP_ACK, new KEGS_JOIN_PERSONAL_SHOP_ACK { Ok = NetError.ERR_TRADE_BLOCK_UNIT_02 });
                    break;
                }
                var listings = _server.Db.SearchPersonalShopListings(Region)
                    .Where(l => l.SellerUnitUid == req.UnitUid)
                    .Select(l => new KSellPersonalShopItemInfo
                    {
                        InventoryItemInfo = new KInventoryItemInfo
                        {
                            ItemUid = l.ListingUid,
                            ItemInfo = new KItemInfo { ItemId = l.ItemId, Quantity = l.Quantity },
                        },
                        PricePerOne = l.PriceEd,
                        InventoryItemUid = l.ListingUid,
                    }).ToList();
                var sellerNick = listings.Count > 0
                    ? _server.Db.SearchPersonalShopListings(Region).First(l => l.SellerUnitUid == req.UnitUid).SellerNickName
                    : string.Empty;
                if (listings.Count > 0)
                    PersonalShopManager.JoinShop(req.UnitUid);
                SendPacket(EventId.EGS_JOIN_PERSONAL_SHOP_ACK, new KEGS_JOIN_PERSONAL_SHOP_ACK
                {
                    Ok = listings.Count > 0 ? NetError.NET_OK : NetError.ERR_PERSONAL_SHOP_03,
                    PersonalShopName = listings.Count > 0 ? $"{sellerNick}'s Shop" : string.Empty,
                    NickName = sellerNick,
                    SellItemInfo = listings,
                });
                break;
            }

            case EventId.EGS_LEAVE_PERSONAL_SHOP_REQ:
            {
                if (ActiveUnit == null) break;
                if (PersonalShopManager.PersonalShopUid <= 0)
                {
                    SendPacket(EventId.EGS_LEAVE_PERSONAL_SHOP_ACK, new KPacketOK { Ok = NetError.ERR_PERSONAL_SHOP_12 });
                    break;
                }
                if (PersonalShopManager.JoinedShopUid > 0)
                    PersonalShopManager.LeaveJoinedShop();
                else
                    PersonalShopManager.CloseShop();
                SendPacket(EventId.EGS_LEAVE_PERSONAL_SHOP_ACK, new KPacketOK { Ok = NetError.NET_OK });
                break;
            }

            case EventId.EGS_CREATE_PSHOP_AGENCY_REQ:
            {
                if (ActiveUnit == null) break;
                if (_server.GameSysValTable.BlockPersonalShop)
                {
                    SendPacket(EventId.EGS_CREATE_PSHOP_AGENCY_ACK, new KEGS_CREATE_PERSONAL_SHOP_ACK
                    {
                        Ok = NetError.ERR_BLOCK_TRADE_00,
                    });
                    break;
                }
                if (AbuserManager.IsTradeBlockUser)
                {
                    SendPacket(EventId.EGS_CREATE_PSHOP_AGENCY_ACK, new KEGS_CREATE_PERSONAL_SHOP_ACK
                    {
                        Ok = NetError.ERR_BLOCK_TRADE_07,
                    });
                    break;
                }
                if (AbuserManager.IsTradeBlockUnit)
                {
                    SendPacket(EventId.EGS_CREATE_PSHOP_AGENCY_ACK, new KEGS_CREATE_PERSONAL_SHOP_ACK
                    {
                        Ok = NetError.ERR_TRADE_BLOCK_UNIT_02,
                    });
                    break;
                }
                PersonalShopManager.JoinShop(ActiveUnit.UnitUid);
                PShopAgencyUid = ActiveUnit.UnitUid;
                SendPacket(EventId.EGS_CREATE_PSHOP_AGENCY_ACK, new KEGS_CREATE_PERSONAL_SHOP_ACK
                {
                    Ok = NetError.NET_OK,
                    PersonalShopUid = ActiveUnit.UnitUid,
                });
                break;
            }

            case EventId.EGS_JOIN_MY_PSHOP_AGENCY_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_JOIN_MY_PSHOP_AGENCY_REQ>(Region);
                if (IsGuestUser)
                {
                    SendPacket(EventId.EGS_JOIN_MY_PSHOP_AGENCY_ACK, new KEGS_JOIN_MY_PSHOP_AGENCY_ACK { Ok = NetError.ERR_GUEST_USER_00 });
                    break;
                }
                if (_server.GameSysValTable.BlockPersonalShop)
                {
                    SendPacket(EventId.EGS_JOIN_MY_PSHOP_AGENCY_ACK, new KEGS_JOIN_MY_PSHOP_AGENCY_ACK { Ok = NetError.ERR_BLOCK_TRADE_00 });
                    break;
                }
                if (AbuserManager.IsTradeBlockUser)
                {
                    SendPacket(EventId.EGS_JOIN_MY_PSHOP_AGENCY_ACK, new KEGS_JOIN_MY_PSHOP_AGENCY_ACK { Ok = NetError.ERR_BLOCK_TRADE_07 });
                    break;
                }
                if (AbuserManager.IsTradeBlockUnit)
                {
                    SendPacket(EventId.EGS_JOIN_MY_PSHOP_AGENCY_ACK, new KEGS_JOIN_MY_PSHOP_AGENCY_ACK { Ok = NetError.ERR_TRADE_BLOCK_UNIT_02 });
                    break;
                }
                PersonalShopManager.JoinShop(ActiveUnit.UnitUid);
                PShopAgencyUid = ActiveUnit.UnitUid;
                var myListings = _server.Db.SearchPersonalShopListings(Region)
                    .Where(l => l.SellerUnitUid == ActiveUnit.UnitUid)
                    .Select(l => new KSellPersonalShopItemInfo
                    {
                        InventoryItemInfo = new KInventoryItemInfo
                        {
                            ItemUid = l.ListingUid,
                            ItemInfo = new KItemInfo { ItemId = l.ItemId, Quantity = l.Quantity },
                        },
                        PricePerOne = l.PriceEd,
                        PShopItemType = 1,
                        InventoryItemUid = l.ListingUid,
                    }).ToList();
                SendPacket(EventId.EGS_JOIN_MY_PSHOP_AGENCY_ACK, new KEGS_JOIN_MY_PSHOP_AGENCY_ACK
                {
                    Ok = NetError.NET_OK,
                    JoinType = req.JoinType,
                    PersonalShopName = $"{ActiveUnit.NickName}'s Agency",
                    NickName = ActiveUnit.NickName,
                    SellItemInfo = myListings,
                });
                break;
            }

            case EventId.EGS_LEAVE_PSHOP_AGENCY_REQ:
            {
                if (ActiveUnit == null) break;
                if (PersonalShopManager.PersonalShopUid <= 0)
                {
                    SendPacket(EventId.EGS_LEAVE_PSHOP_AGENCY_ACK, new KPacketOK { Ok = NetError.ERR_PERSONAL_SHOP_12 });
                    break;
                }
                PersonalShopManager.LeaveJoinedShop();
                SendPacket(EventId.EGS_LEAVE_PSHOP_AGENCY_ACK, new KPacketOK { Ok = NetError.NET_OK });
                break;
            }

            case EventId.EGS_REG_PSHOP_AGENCY_ITEM_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_REG_PERSONAL_SHOP_ITEM_REQ>(Region);
                var sellItems = new List<KSellPersonalShopItemInfo>();
                var updatedInv = new List<KInventoryItemInfo>();
                foreach (var reg in req.RegItems)
                {
                    var (found, slot) = _server.Db.GetUnitItemByUid(ActiveUnit.UnitUid, reg.ItemUid);
                    if (!found) continue;
                    var (_, delSlot) = _server.Db.DeleteInventoryItem(ActiveUnit.UnitUid, reg.ItemUid, Math.Max(1, reg.Quantity));
                    updatedInv.Add(delSlot);
                    long listingUid = PersonalShopManager.OpenShopAndRegisterItem(
                        _server.Db, Region, ActiveUnit.UnitUid, ActiveUnit.NickName,
                        slot.ItemInfo.ItemId, Math.Max(1, reg.Quantity), reg.PricePerOne);
                    sellItems.Add(new KSellPersonalShopItemInfo
                    {
                        InventoryItemInfo = new KInventoryItemInfo
                        {
                            ItemUid = listingUid,
                            ItemInfo = new KItemInfo { ItemId = slot.ItemInfo.ItemId, Quantity = Math.Max(1, reg.Quantity) },
                        },
                        PricePerOne = reg.PricePerOne,
                        PShopItemType = 1,
                        InventoryItemUid = reg.ItemUid,
                    });
                }
                SendPacket(EventId.EGS_REG_PSHOP_AGENCY_ITEM_ACK, new KEGS_REG_PERSONAL_SHOP_ITEM_ACK
                {
                    Ok = sellItems.Count > 0 ? NetError.NET_OK : NetError.ERR_PERSONAL_SHOP_01,
                    Ed = ActiveUnit.Ed,
                    PersonalShopName = req.PersonalShopName,
                    SellItemInfo = sellItems,
                    InventorySlotInfo = updatedInv,
                });
                break;
            }

            case EventId.EGS_STOP_SALE_PSHOP_AGENCY_REQ:
            {
                if (ActiveUnit == null) break;
                if (PShopAgencyUid == 0)
                {
                    SendPacket(EventId.EGS_STOP_SALE_PSHOP_AGENCY_ACK, new KPacketOK { Ok = NetError.ERR_PERSONAL_SHOP_34 });
                    break;
                }
                long agencyUid = PShopAgencyUid;
                PersonalShopManager.CloseShop();
                if (_pshopAgencyRemoteOpened && _server.LoginServerUid > 0 && _server.Router.HasRoute(PerformerId.PI_LOGIN_SERVER))
                {
                    _server.Router.Send(PerformerTypeId, ActiveUnit.UnitUid, PerformerId.PI_LOGIN_SERVER, _server.LoginServerUid,
                        EventId.ERM_STOP_SALE_PSHOP_AGENCY_REQ, new KPacketLong { Value = agencyUid });
                }
                else
                {
                    PShopAgencyUid = 0;
                    _pshopAgencyRemoteOpened = false;
                    SendPacket(EventId.EGS_STOP_SALE_PSHOP_AGENCY_ACK, new KPacketOK { Ok = NetError.NET_OK });
                }
                break;
            }

            case EventId.EGS_PICK_UP_FROM_PSHOP_AGENCY_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_PICK_UP_FROM_PSHOP_AGENCY_REQ>(Region);
                var (returnedItems, remainSell) = _server.Db.PickUpPersonalShopAgencyListings(
                    Region, ActiveUnit.UnitUid, req.PickUpItemList);
                ActiveUnit = _server.Db.GetUnitByUid(Region, ActiveUnit.UnitUid);
                SendPacket(EventId.EGS_PICK_UP_FROM_PSHOP_AGENCY_ACK, new KEGS_PICK_UP_FROM_PSHOP_AGENCY_ACK
                {
                    Ok = NetError.NET_OK,
                    Ed = ActiveUnit?.Ed ?? 0,
                    InventorySlotInfo = returnedItems,
                    RemainSellItem = remainSell,
                });
                break;
            }

            case EventId.EGS_SEARCH_TRADE_BOARD_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_SEARCH_TRADE_BOARD_REQ>(Region);
                if (_server.GameSysValTable.BlockPersonalShop)
                {
                    SendPacket(EventId.EGS_SEARCH_TRADE_BOARD_ACK, new KEGS_SEARCH_TRADE_BOARD_ACK
                    {
                        Ok = NetError.NET_OK,
                        ItemInfo = [],
                    });
                    break;
                }
                var rows = _server.Db.SearchPersonalShopListings(Region, req.Finder);
                var items = rows.Select(r => new KTradeBoardItemInfo
                {
                    PersonalShopUid = r.ListingUid,
                    HostUnitUid = r.SellerUnitUid,
                    SellerNickName = r.SellerNickName,
                    SellItemInfo = new KSellPersonalShopItemInfo
                    {
                        InventoryItemInfo = new KInventoryItemInfo
                        {
                            ItemUid = r.ListingUid,
                            ItemInfo = new KItemInfo { ItemId = r.ItemId, Quantity = r.Quantity },
                        },
                        PricePerOne = r.PriceEd,
                        InventoryItemUid = r.ListingUid,
                    },
                }).ToList();
                SendPacket(EventId.EGS_SEARCH_TRADE_BOARD_ACK, new KEGS_SEARCH_TRADE_BOARD_ACK
                {
                    Ok = NetError.NET_OK,
                    ItemInfo = items,
                });
                break;
            }

            // -------- Guild Management / Skills / Ads / Applications --------

            case EventId.EGS_GET_GUILD_USER_LIST_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_GET_GUILD_USER_LIST_REQ>(Region);
                int guildUid = ActiveUnit.UserGuildInfo.GuildUid;
                if (guildUid <= 0)
                {
                    SendPacket(EventId.EGS_GET_GUILD_USER_LIST_ACK, new KEGS_GET_GUILD_USER_LIST_ACK { Ok = NetError.ERR_GUILD_01 });
                    break;
                }
                var members = _server.Db.GetGuildMembers(guildUid);
                int onlineCount = members.Count(m => _server.GetUserByUnitUid(m.UnitUid) != null);
                var gradeList = members.Select(m =>
                {
                    var u = _server.GetUserByUnitUid(m.UnitUid);
                    return new KGuildMemberInfo
                    {
                        GsUid = 1,
                        UnitUid = m.UnitUid,
                        NickName = m.NickName,
                        UnitClass = m.UnitClass,
                        Level = m.Level,
                        MemberShipGrade = m.MemberGrade,
                        ChannelId = 1,
                        MapId = u?.CurrentFieldMapId ?? 0,
                        Position = (sbyte)(u != null ? 1 : -1),
                    };
                }).ToList();
                var msgList = members.Select(m => new KGuildMemberMessageInfo
                {
                    UnitUid = m.UnitUid,
                    NickName = m.NickName,
                    MemberShipGrade = m.MemberGrade,
                    UnitClass = m.UnitClass,
                    Level = m.Level,
                }).ToList();
                SendPacket(EventId.EGS_GET_GUILD_USER_LIST_ACK, new KEGS_GET_GUILD_USER_LIST_ACK
                {
                    Ok = NetError.NET_OK,
                    TotalPage = 1,
                    ViewPage = req.ViewPage,
                    GuildUserListType = req.GuildUserListType,
                    LoginedMemberCount = onlineCount,
                    MemberPositionGradeList = gradeList,
                    MemberMessageList = msgList,
                });
                break;
            }

            case EventId.EGS_INVITE_GUILD_REQ:
            {
                if (ActiveUnit == null || ActiveUnit.UserGuildInfo.GuildUid <= 0) break;
                var req = ev.GetPayload<KEGS_INVITE_GUILD_REQ>(Region);
                var target = req.UnitUid > 0 ? _server.GetUserByUnitUid(req.UnitUid) : null;
                var guild = _server.Db.GetGuildInfo(Region, ActiveUnit.UserGuildInfo.GuildUid);
                bool ok = target?.ActiveUnit != null && guild != null && target.ActiveUnit.UserGuildInfo.GuildUid == 0;
                SendPacket(EventId.EGS_INVITE_GUILD_ACK, new KEGS_INVITE_GUILD_ACK
                {
                    Ok = ok ? NetError.NET_OK : NetError.ERR_GUILD_01,
                    ReceiverNickName = target?.ActiveUnit?.NickName ?? req.NickName,
                });
                if (ok && target != null && guild != null)
                {
                    target.SendPacket(EventId.EGS_INVITE_GUILD_NOT, new KEGS_INVITE_GUILD_NOT
                    {
                        GuildInfo = guild,
                        SenderNickName = ActiveUnit.NickName,
                    });
                }
                break;
            }

            case EventId.EGS_INVITE_GUILD_REPLY_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_INVITE_GUILD_REPLY_REQ>(Region);
                if (req.Reason != 0)
                {
                    SendPacket(EventId.EGS_INVITE_GUILD_REPLY_ACK, new KPacketOK { Ok = NetError.NET_OK });
                    break;
                }
                var (ok, guild, err) = _server.Db.JoinGuild(Region, req.GuildUid, ActiveUnit.UnitUid);
                if (ok && guild != null)
                {
                    ActiveUnit.UserGuildInfo.GuildUid = guild.GuildUid;
                    ActiveUnit.UserGuildInfo.GuildName = guild.GuildName;
                    ActiveUnit.UserGuildInfo.MemberShipGrade = 5;
                }
                SendPacket(EventId.EGS_INVITE_GUILD_REPLY_ACK, new KPacketOK { Ok = ok ? NetError.NET_OK : err });
                break;
            }

            case EventId.EGS_CHANGE_GUILD_MESSAGE_REQ:
            {
                if (ActiveUnit == null || ActiveUnit.UserGuildInfo.GuildUid <= 0) break;
                var req = ev.GetPayload<KEGS_CHANGE_GUILD_MESSAGE_REQ>(Region);
                bool ok = _server.Db.UpdateGuildMessage(ActiveUnit.UserGuildInfo.GuildUid, req.Message);
                SendPacket(EventId.EGS_CHANGE_GUILD_MESSAGE_ACK, new KPacketOK
                {
                    Ok = ok ? NetError.NET_OK : NetError.ERR_GUILD_01,
                });
                break;
            }

            case EventId.EGS_CHANGE_GUILD_MEMBER_MESSAGE_REQ:
            {
                if (ActiveUnit == null || ActiveUnit.UserGuildInfo.GuildUid <= 0) break;
                var req = ev.GetPayload<KEGS_CHANGE_GUILD_MEMBER_MESSAGE_REQ>(Region);
                SendPacket(EventId.EGS_CHANGE_GUILD_MEMBER_MESSAGE_ACK, new KEGS_CHANGE_GUILD_MEMBER_MESSAGE_ACK
                {
                    Ok = NetError.NET_OK,
                    Delete = string.IsNullOrEmpty(req.Message),
                });
                break;
            }

            case EventId.EGS_CHANGE_GUILD_NAME_CHECK_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_CHANGE_GUILD_NAME_CHECK_REQ>(Region);
                bool avail = _server.Db.IsGuildNameAvailable(Region, req.GuildName);
                if (avail) _pendingGuildName = req.GuildName;
                SendPacket(EventId.EGS_CHANGE_GUILD_NAME_CHECK_ACK, new KPacketOK
                {
                    Ok = avail ? NetError.NET_OK : NetError.ERR_GUILD_01,
                });
                break;
            }

            case EventId.EGS_CHANGE_GUILD_NAME_REQ:
            {
                if (ActiveUnit == null || ActiveUnit.UserGuildInfo.GuildUid <= 0) break;
                var req = ev.GetPayload<KEGS_CHANGE_GUILD_NAME_REQ>(Region);
                var guild = _server.Db.GetGuildInfo(Region, ActiveUnit.UserGuildInfo.GuildUid);
                if (guild != null && !string.IsNullOrWhiteSpace(_pendingGuildName))
                {
                    guild.GuildName = _pendingGuildName;
                    ActiveUnit.UserGuildInfo.GuildName = _pendingGuildName;
                }
                SendPacket(EventId.EGS_CHANGE_GUILD_NAME_ACK, new KEGS_CHANGE_GUILD_NAME_ACK
                {
                    Ok = req.Ok == 0 && guild != null ? NetError.NET_OK : NetError.ERR_GUILD_01,
                    GuildInfo = guild ?? new KGuildInfo(),
                });
                break;
            }

            case EventId.EGS_DISBAND_GUILD_REQ:
            {
                if (ActiveUnit == null || ActiveUnit.UserGuildInfo.GuildUid <= 0) break;
                bool ok = _server.Db.DisbandGuild(Region, ActiveUnit.UserGuildInfo.GuildUid, ActiveUnit.UnitUid);
                if (ok)
                {
                    ActiveUnit.UserGuildInfo = new KUserGuildInfo();
                }
                SendPacket(EventId.EGS_DISBAND_GUILD_ACK, new KPacketOK
                {
                    Ok = ok ? NetError.NET_OK : NetError.ERR_GUILD_01,
                });
                break;
            }

            case EventId.EGS_GET_GUILD_SKILL_REQ:
            {
                if (ActiveUnit == null || ActiveUnit.UserGuildInfo.GuildUid <= 0) break;
                var req = ev.GetPayload<KEGS_GET_GUILD_SKILL_REQ>(Region);
                bool ok = _server.Db.LearnGuildSkill(ActiveUnit.UserGuildInfo.GuildUid, req.GuildSkillId);
                SendPacket(EventId.EGS_GET_GUILD_SKILL_ACK, new KPacketOK
                {
                    Ok = ok ? NetError.NET_OK : NetError.ERR_GUILD_01,
                });
                break;
            }

            case EventId.EGS_RESET_GUILD_SKILL_REQ:
            {
                if (ActiveUnit == null || ActiveUnit.UserGuildInfo.GuildUid <= 0) break;
                var req = ev.GetPayload<KEGS_RESET_GUILD_SKILL_REQ>(Region);
                _server.Db.ResetGuildSkill(ActiveUnit.UserGuildInfo.GuildUid, req.GuildSkillId);
                SendPacket(EventId.EGS_RESET_GUILD_SKILL_ACK, new KEGS_RESET_GUILD_SKILL_ACK
                {
                    Ok = NetError.NET_OK,
                });
                break;
            }

            case EventId.EGS_INIT_GUILD_SKILL_REQ:
            {
                if (ActiveUnit == null || ActiveUnit.UserGuildInfo.GuildUid <= 0) break;
                var req = ev.GetPayload<KEGS_INIT_GUILD_SKILL_REQ>(Region);
                _server.Db.InitGuildSkills(ActiveUnit.UserGuildInfo.GuildUid);
                var updated = new List<KInventoryItemInfo>();
                if (req.ItemUid > 0)
                {
                    var (delOk, delSlot) = _server.Db.DeleteInventoryItem(ActiveUnit.UnitUid, req.ItemUid, 1);
                    if (delOk) updated.Add(delSlot);
                }
                SendPacket(EventId.EGS_INIT_GUILD_SKILL_ACK, new KEGS_INIT_GUILD_SKILL_ACK
                {
                    Ok = NetError.NET_OK,
                    InventorySlotInfo = updated,
                });
                break;
            }

            case EventId.EGS_GET_GUILD_SKILL_IN_BOARD_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_GET_GUILD_SKILL_IN_BOARD_REQ>(Region);
                var skills = _server.Db.GetGuildSkills(req.GuildUid);
                SendPacket(EventId.EGS_GET_GUILD_SKILL_IN_BOARD_ACK, new KEGS_GET_GUILD_SKILL_IN_BOARD_ACK
                {
                    Ok = NetError.NET_OK,
                    GuildSkillInfo = new KGuildSkillInfo
                    {
                        GuildSkill = skills,
                        GuildSPoint = 10,
                    },
                });
                break;
            }

            case EventId.EGS_REGISTRATION_GUILD_AD_REQ:
            case EventId.EGS_MODIFY_REG_GUILD_AD_REQ:
            {
                if (ActiveUnit == null || ActiveUnit.UserGuildInfo.GuildUid <= 0) break;
                var req = ev.GetPayload<KEGS_REGISTRATION_GUILD_AD_REQ>(Region);
                _server.Db.UpsertGuildAd(Region, ActiveUnit.UserGuildInfo.GuildUid, req.Period, req.AdMessage);
                var ackId = ev.EventId == EventId.EGS_MODIFY_REG_GUILD_AD_REQ
                    ? EventId.EGS_MODIFY_REG_GUILD_AD_ACK
                    : EventId.EGS_REGISTRATION_GUILD_AD_ACK;
                SendPacket(ackId, new KEGS_REGISTRATION_GUILD_AD_ACK
                {
                    Ok = NetError.NET_OK,
                    Ed = ActiveUnit.Ed,
                    Period = req.Period,
                    AdMessage = req.AdMessage,
                });
                break;
            }

            case EventId.EGS_GET_APPLY_JOIN_GUILD_LIST_REQ:
            {
                if (ActiveUnit == null || ActiveUnit.UserGuildInfo.GuildUid <= 0) break;
                var req = ev.GetPayload<KEGS_GET_APPLY_JOIN_GUILD_LIST_REQ>(Region);
                var applies = _server.Db.GetGuildApplies(ActiveUnit.UserGuildInfo.GuildUid);
                SendPacket(EventId.EGS_GET_APPLY_JOIN_GUILD_LIST_ACK, new KEGS_GET_APPLY_JOIN_GUILD_LIST_ACK
                {
                    Ok = NetError.NET_OK,
                    GuildName = ActiveUnit.UserGuildInfo.GuildName,
                    TotalPage = 1,
                    ViewPage = req.ViewPage,
                    ApplyJoinGuildList = applies,
                });
                break;
            }

            case EventId.EGS_ACCEPT_JOIN_GUILD_REQ:
            {
                if (ActiveUnit == null || ActiveUnit.UserGuildInfo.GuildUid <= 0) break;
                var req = ev.GetPayload<KEGS_ACCEPT_JOIN_GUILD_REQ>(Region);
                if (ActiveUnit.UserGuildInfo.MemberShipGrade is > 2)
                {
                    SendPacket(EventId.EGS_ACCEPT_JOIN_GUILD_ACK, new KEGS_ACCEPT_JOIN_GUILD_ACK
                    {
                        Ok = NetError.ERR_GUILD_19,
                    });
                    break;
                }
                int guildUid = ActiveUnit.UserGuildInfo.GuildUid;
                var (ok, guild, err) = _server.Db.JoinGuild(Region, guildUid, req.UnitUid);
                if (ok) _server.Db.DeleteGuildApply(guildUid, req.UnitUid);
                var joinedUnit = _server.Db.GetUnitByUid(Region, req.UnitUid);
                var onlineTarget = _server.GetUserByUnitUid(req.UnitUid);
                if (ok && guild != null && onlineTarget?.ActiveUnit != null)
                {
                    onlineTarget.ActiveUnit.UserGuildInfo.GuildUid = guild.GuildUid;
                    onlineTarget.ActiveUnit.UserGuildInfo.GuildName = guild.GuildName;
                    onlineTarget.ActiveUnit.UserGuildInfo.MemberShipGrade = 5;
                    var members = _server.Db.GetGuildMembers(guild.GuildUid);
                    onlineTarget.SendPacket(EventId.EGS_JOIN_GUILD_SUCCESS_NOT, new KEGS_GUILD_INFO_NOT
                    {
                        GuildInfo = guild,
                        GuildMasterName = ActiveUnit.NickName,
                        MemberUnitUid = members.Select(m => m.UnitUid).ToList(),
                    });
                }
                SendPacket(EventId.EGS_ACCEPT_JOIN_GUILD_ACK, new KEGS_ACCEPT_JOIN_GUILD_ACK
                {
                    Ok = ok ? NetError.NET_OK : err,
                    ReceiverNickName = joinedUnit?.NickName ?? string.Empty,
                });
                break;
            }

            case EventId.EGS_DELETE_APPLY_JOIN_GUILD_REQ:
            {
                if (ActiveUnit == null || ActiveUnit.UserGuildInfo.GuildUid <= 0) break;
                var req = ev.GetPayload<KEGS_DELETE_APPLY_JOIN_GUILD_REQ>(Region);
                bool ok = _server.Db.DeleteGuildApply(ActiveUnit.UserGuildInfo.GuildUid, req.UnitUid);
                SendPacket(EventId.EGS_DELETE_APPLY_JOIN_GUILD_ACK, new KPacketOK
                {
                    Ok = ok ? NetError.NET_OK : NetError.ERR_GUILD_01,
                });
                break;
            }

            // ── Letter / PostBlackList / LoveLetter (GSUserLetter.cpp) ─────────
            case EventId.EGS_GET_POST_BLACK_LIST_REQ:
            {
                if (ActiveUnit == null) break;
                SendPacket(EventId.EGS_GET_POST_BLACK_LIST_ACK, new KEGS_GET_POST_BLACK_LIST_ACK
                {
                    BlackList = LetterBox.GetBlackList(),
                });
                break;
            }

            case EventId.EGS_NEW_POST_BLACK_LIST_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_NEW_POST_BLACK_LIST_REQ>(Region);
                if (KStringFilterManager.IsInvalidMarkIn(req.NickName))
                {
                    SendPacket(EventId.EGS_NEW_POST_BLACK_LIST_ACK, new KEGS_NEW_POST_BLACK_LIST_ACK
                    {
                        Ok = NetError.ERR_ODBC_00,
                        NickName = req.NickName,
                    });
                    break;
                }
                if (LetterBox.IsBlackList(req.NickName))
                {
                    SendPacket(EventId.EGS_NEW_POST_BLACK_LIST_ACK, new KEGS_NEW_POST_BLACK_LIST_ACK
                    {
                        Ok = NetError.ERR_POST_BLACK_LIST_01,
                        NickName = req.NickName,
                    });
                    break;
                }
                var targetUnit = _server.Db.GetUnitByNickName(Region, req.NickName);
                if (targetUnit == null)
                {
                    SendPacket(EventId.EGS_NEW_POST_BLACK_LIST_ACK, new KEGS_NEW_POST_BLACK_LIST_ACK
                    {
                        Ok = NetError.ERR_POST_BLACK_LIST_00,
                        NickName = req.NickName,
                    });
                    break;
                }
                _server.Db.AddPostBlackList(ActiveUnit.UnitUid, req.NickName);
                LetterBox.AddBlackList(req.NickName);
                SendPacket(EventId.EGS_NEW_POST_BLACK_LIST_ACK, new KEGS_NEW_POST_BLACK_LIST_ACK
                {
                    Ok = NetError.NET_OK,
                    NickName = req.NickName,
                });
                break;
            }

            case EventId.EGS_DEL_POST_BLACK_LIST_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_NEW_POST_BLACK_LIST_REQ>(Region);
                if (KStringFilterManager.IsInvalidMarkIn(req.NickName))
                {
                    SendPacket(EventId.EGS_DEL_POST_BLACK_LIST_ACK, new KEGS_NEW_POST_BLACK_LIST_ACK
                    {
                        Ok = NetError.ERR_ODBC_00,
                        NickName = req.NickName,
                    });
                    break;
                }
                if (!LetterBox.IsBlackList(req.NickName))
                {
                    SendPacket(EventId.EGS_DEL_POST_BLACK_LIST_ACK, new KEGS_NEW_POST_BLACK_LIST_ACK
                    {
                        Ok = NetError.ERR_POST_BLACK_LIST_03,
                        NickName = req.NickName,
                    });
                    break;
                }
                _server.Db.RemovePostBlackList(ActiveUnit.UnitUid, req.NickName);
                LetterBox.DelBlackList(req.NickName);
                SendPacket(EventId.EGS_DEL_POST_BLACK_LIST_ACK, new KEGS_NEW_POST_BLACK_LIST_ACK
                {
                    Ok = NetError.NET_OK,
                    NickName = req.NickName,
                });
                break;
            }

            case EventId.EGS_SEND_LOVE_LETTER_EVENT_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_SEND_LOVE_LETTER_EVENT_REQ>(Region);
                var reqItem = new Dictionary<int, int> { [KUserRelationshipManager.CiRequestOfLove] = 1 };
                if (!_server.Db.HasItemsInInventory(ActiveUnit.UnitUid, reqItem))
                {
                    SendPacket(EventId.EGS_SEND_LOVE_LETTER_EVENT_ACK, new KEGS_SEND_LOVE_LETTER_EVENT_ACK
                    {
                        Ok = NetError.ERR_REQUEST_OF_LOVE_02,
                    });
                    break;
                }
                if (ActiveUnit.Ed < 10_000)
                {
                    SendPacket(EventId.EGS_SEND_LOVE_LETTER_EVENT_ACK, new KEGS_SEND_LOVE_LETTER_EVENT_ACK
                    {
                        Ok = NetError.ERR_REQUEST_OF_LOVE_03,
                    });
                    break;
                }
                if (req.Message.Length > 700)
                {
                    SendPacket(EventId.EGS_SEND_LOVE_LETTER_EVENT_ACK, new KEGS_SEND_LOVE_LETTER_EVENT_ACK
                    {
                        Ok = NetError.ERR_POST_LETTER_19,
                    });
                    break;
                }
                if (KStringFilterManager.IsInvalidMarkInForLetter(req.Message))
                {
                    SendPacket(EventId.EGS_SEND_LOVE_LETTER_EVENT_ACK, new KEGS_SEND_LOVE_LETTER_EVENT_ACK
                    {
                        Ok = NetError.ERR_ODBC_00,
                    });
                    break;
                }
                var (consumed, updatedSlots) = _server.Db.ConsumeItemsByItemId(ActiveUnit.UnitUid, reqItem);
                if (!consumed)
                {
                    SendPacket(EventId.EGS_SEND_LOVE_LETTER_EVENT_ACK, new KEGS_SEND_LOVE_LETTER_EVENT_ACK
                    {
                        Ok = NetError.ERR_REQUEST_OF_LOVE_04,
                    });
                    break;
                }
                _server.Db.UpdateUnitProgress(ActiveUnit.UnitUid, 0, -10_000, 0);
                ActiveUnit.Ed -= 10_000;
                _server.Db.SaveLoveLetterEvent(Region, ActiveUnit.UnitUid, req.Message);
                SendPacket(EventId.EGS_SEND_LOVE_LETTER_EVENT_ACK, new KEGS_SEND_LOVE_LETTER_EVENT_ACK
                {
                    Ok = NetError.NET_OK,
                    Ed = ActiveUnit.Ed,
                    UpdatedInventorySlot = updatedSlots,
                });
                break;
            }

            // ── Messenger / FriendGroups / KNM (GSUserMessenger.cpp) ─────────
            case EventId.EGS_REQUEST_FRIEND_NOT:
            {
                if (ActiveUnit == null) break;
                var not = ev.GetPayload<KEGS_REQUEST_FRIEND_NOT>(Region);
                if (BlackList.VerifyBlackListUser(not.UnitUid)) break;
                _pendingFriendRequests.Add(not.UnitUid);
                _friends[not.UnitUid] = new KFriendInfo
                {
                    UnitUid = not.UnitUid,
                    NickName = not.NickName,
                    GroupId = 0,
                    Status = KFriendInfo.FsWaiting,
                    Position = KFriendInfo.FpOffLine,
                };
                SendPacket(EventId.EGS_REQUEST_FRIEND_NOT, not);
                break;
            }

            case EventId.EGS_ACCEPT_FRIEND_NOT:
            {
                if (ActiveUnit == null) break;
                var not = ev.GetPayload<KEGS_ACCEPT_FRIEND_NOT>(Region);
                _friendUids.Add(not.UnitUid);
                if (_friends.TryGetValue(not.UnitUid, out var fInfo))
                {
                    fInfo.Status = KFriendInfo.FsNormal;
                    fInfo.Position = KFriendInfo.FpOnLine;
                }
                else
                {
                    _friends[not.UnitUid] = new KFriendInfo
                    {
                        UnitUid = not.UnitUid,
                        NickName = _server.Db.GetUnitByUid(Region, not.UnitUid)?.NickName ?? string.Empty,
                        Status = KFriendInfo.FsNormal,
                        Position = KFriendInfo.FpOnLine,
                    };
                }
                SendPacket(EventId.EGS_ACCEPT_FRIEND_NOT, not);
                break;
            }

            case EventId.EGS_DENY_FRIEND_NOT:
            {
                if (ActiveUnit == null) break;
                var not = ev.GetPayload<KEGS_DENY_FRIEND_NOT>(Region);
                _friends.Remove(not.UnitUid);
                SendPacket(EventId.EGS_DENY_FRIEND_NOT, not);
                break;
            }

            case EventId.EGS_DELETE_FRIEND_NOT:
            {
                if (ActiveUnit == null) break;
                var not = ev.GetPayload<KEGS_DELETE_FRIEND_NOT>(Region);
                _friendUids.Remove(not.UnitUid);
                _blockedFriendUids.Remove(not.UnitUid);
                _friends.Remove(not.UnitUid);
                SendPacket(EventId.EGS_DELETE_FRIEND_NOT, not);
                break;
            }

            case EventId.EGS_BLOCK_FRIEND_NOT:
            {
                if (ActiveUnit == null) break;
                var not = ev.GetPayload<KEGS_BLOCK_FRIEND_NOT>(Region);
                if (_friends.TryGetValue(not.UnitUid, out var fInfo))
                    fInfo.Status = KFriendInfo.FsBlocked;
                SendPacket(EventId.EGS_BLOCK_FRIEND_NOT, not);
                break;
            }

            case EventId.EGS_UNBLOCK_FRIEND_NOT:
            {
                if (ActiveUnit == null) break;
                var not = ev.GetPayload<KEGS_UNBLOCK_FRIEND_NOT>(Region);
                if (_friends.TryGetValue(not.UnitUid, out var fInfo))
                    fInfo.Status = KFriendInfo.FsNormal;
                SendPacket(EventId.EGS_UNBLOCK_FRIEND_NOT, not);
                break;
            }

            case EventId.EGS_MOVE_FRIEND_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_MOVE_FRIEND_REQ>(Region);
                if (!_friends.TryGetValue(req.UnitUid, out var fInfo))
                {
                    SendPacket(EventId.EGS_MOVE_FRIEND_ACK, new KEGS_MOVE_FRIEND_ACK { Ok = NetError.ERR_MESSENGER_01 });
                    break;
                }
                if (req.TargetGroupId != 0 && !_friendGroups.ContainsKey(req.TargetGroupId))
                {
                    SendPacket(EventId.EGS_MOVE_FRIEND_ACK, new KEGS_MOVE_FRIEND_ACK { Ok = NetError.ERR_MESSENGER_08 });
                    break;
                }
                fInfo.GroupId = req.TargetGroupId;
                _server.Db.MoveFriendGroup(ActiveUnit.UnitUid, req.UnitUid, req.TargetGroupId);
                SendPacket(EventId.EGS_MOVE_FRIEND_ACK, new KEGS_MOVE_FRIEND_ACK
                {
                    Ok = NetError.NET_OK,
                    UnitUid = req.UnitUid,
                    TargetGroupId = req.TargetGroupId,
                });
                break;
            }

            case EventId.EGS_MAKE_FRIEND_GROUP_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_MAKE_FRIEND_GROUP_REQ>(Region);
                if (_friendGroups.Count >= 10)
                {
                    SendPacket(EventId.EGS_MAKE_FRIEND_GROUP_ACK, new KEGS_MAKE_FRIEND_GROUP_ACK { Ok = NetError.ERR_MESSENGER_12 });
                    break;
                }
                if (KStringFilterManager.IsInvalidMarkIn(req.GroupName))
                {
                    SendPacket(EventId.EGS_MAKE_FRIEND_GROUP_ACK, new KEGS_MAKE_FRIEND_GROUP_ACK { Ok = NetError.ERR_ODBC_00 });
                    break;
                }
                if (_friendGroups.Values.Any(g => string.Equals(g, req.GroupName, StringComparison.Ordinal)))
                {
                    SendPacket(EventId.EGS_MAKE_FRIEND_GROUP_ACK, new KEGS_MAKE_FRIEND_GROUP_ACK { Ok = NetError.ERR_MESSENGER_09 });
                    break;
                }
                sbyte newGroupId = 1;
                while (newGroupId <= 10 && _friendGroups.ContainsKey(newGroupId))
                    newGroupId++;
                _friendGroups[newGroupId] = req.GroupName;
                _server.Db.MakeFriendGroup(ActiveUnit.UnitUid, newGroupId, req.GroupName);
                SendPacket(EventId.EGS_MAKE_FRIEND_GROUP_ACK, new KEGS_MAKE_FRIEND_GROUP_ACK
                {
                    Ok = NetError.NET_OK,
                    GroupId = newGroupId,
                    GroupName = req.GroupName,
                });
                break;
            }

            case EventId.EGS_RENAME_FRIEND_GROUP_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_RENAME_FRIEND_GROUP_REQ>(Region);
                if (!_friendGroups.ContainsKey(req.GroupId))
                {
                    SendPacket(EventId.EGS_RENAME_FRIEND_GROUP_ACK, new KEGS_RENAME_FRIEND_GROUP_ACK { Ok = NetError.ERR_MESSENGER_08 });
                    break;
                }
                if (KStringFilterManager.IsInvalidMarkIn(req.GroupName))
                {
                    SendPacket(EventId.EGS_RENAME_FRIEND_GROUP_ACK, new KEGS_RENAME_FRIEND_GROUP_ACK { Ok = NetError.ERR_ODBC_00 });
                    break;
                }
                if (_friendGroups.Any(kv => kv.Key != req.GroupId && string.Equals(kv.Value, req.GroupName, StringComparison.Ordinal)))
                {
                    SendPacket(EventId.EGS_RENAME_FRIEND_GROUP_ACK, new KEGS_RENAME_FRIEND_GROUP_ACK { Ok = NetError.ERR_MESSENGER_09 });
                    break;
                }
                _friendGroups[req.GroupId] = req.GroupName;
                _server.Db.RenameFriendGroup(ActiveUnit.UnitUid, req.GroupId, req.GroupName);
                SendPacket(EventId.EGS_RENAME_FRIEND_GROUP_ACK, new KEGS_RENAME_FRIEND_GROUP_ACK
                {
                    Ok = NetError.NET_OK,
                    GroupId = req.GroupId,
                    GroupName = req.GroupName,
                });
                break;
            }

            case EventId.EGS_DELETE_FRIEND_GROUP_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_DELETE_FRIEND_GROUP_REQ>(Region);
                if (!_friendGroups.Remove(req.GroupId))
                {
                    SendPacket(EventId.EGS_DELETE_FRIEND_GROUP_ACK, new KEGS_DELETE_FRIEND_GROUP_ACK { Ok = NetError.ERR_MESSENGER_08 });
                    break;
                }
                foreach (var f in _friends.Values)
                {
                    if (f.GroupId == req.GroupId)
                        f.GroupId = 0;
                }
                _server.Db.DeleteFriendGroup(ActiveUnit.UnitUid, req.GroupId);
                SendPacket(EventId.EGS_DELETE_FRIEND_GROUP_ACK, new KEGS_DELETE_FRIEND_GROUP_ACK
                {
                    Ok = NetError.NET_OK,
                    GroupId = req.GroupId,
                });
                break;
            }

            case EventId.EGS_FRIEND_POSITION_UPDATE_NOT:
            {
                if (ActiveUnit == null) break;
                var not = ev.GetPayload<KEGS_FRIEND_POSITION_UPDATE_NOT>(Region);
                foreach (var kv in not.FriendPosition)
                {
                    if (_friends.TryGetValue(kv.Key, out var fInfo))
                        fInfo.Position = kv.Value;
                }
                SendPacket(EventId.EGS_FRIEND_POSITION_UPDATE_NOT, not);
                break;
            }

            case EventId.EGS_GET_MY_MESSENGER_SN_REQ:
            {
                if (ActiveUnit == null) break;
                var ks = new KSerializer(Region);
                ks.BeginReading(ev.Buffer);
                ks.Get(out long targetUnitUid);
                ks.EndReading();
                uint sn = _server.Db.GetMessengerSerialNum(targetUnitUid);
                SendPacket(EventId.EGS_GET_MY_MESSENGER_SN_ACK, new KEGS_GET_MY_MESSENGER_SN_ACK
                {
                    Ok = sn > 0 ? NetError.NET_OK : NetError.ERR_MESSENGER_01,
                    KnmSerialNum = sn,
                });
                break;
            }

            case EventId.EGS_KNM_INVITE_REQ:
            {
                if (ActiveUnit == null) break;
                var ks = new KSerializer(Region);
                ks.BeginReading(ev.Buffer);
                ks.GetWideString(out string targetNick);
                ks.EndReading();
                if (CurrentRoomUid <= 0)
                {
                    SendPacket(EventId.EGS_KNM_INVITE_ACK, new KPacketOK { Ok = NetError.ERR_KNM_03 });
                    break;
                }
                var room = _server.Center.RoomManager.GetRoom(CurrentRoomUid);
                if (room == null || room.Info.RoomType != 0)
                {
                    SendPacket(EventId.EGS_KNM_INVITE_ACK, new KPacketOK { Ok = NetError.ERR_KNM_03 });
                    break;
                }
                var target = _server.GetUserByNickName(targetNick);
                if (target?.ActiveUnit == null || target.ActiveUnit.UnitUid == ActiveUnit.UnitUid ||
                    target.BlackList.VerifyBlackListUser(ActiveUnit.UnitUid))
                {
                    SendPacket(EventId.EGS_KNM_INVITE_ACK, new KPacketOK { Ok = NetError.ERR_KNM_02 });
                    break;
                }
                if (target.Fsm.CurrentStateId != KGSFSM.S_FIELD_MAP)
                {
                    SendPacket(EventId.EGS_KNM_INVITE_ACK, new KPacketOK { Ok = NetError.ERR_ROOM_44 });
                    break;
                }
                SendPacket(EventId.EGS_KNM_INVITE_ACK, new KPacketOK { Ok = NetError.NET_OK });
                target.SendPacket(EventId.EGS_KNM_INVITE_NOT, new KEGS_KNM_INVITE_NOT
                {
                    NickName = ActiveUnit.NickName,
                    UnitInfo = new KNMUnitInfo
                    {
                        UnitUid = ActiveUnit.UnitUid,
                        State = (int)Fsm.CurrentStateId,
                        DungeonId = 0,
                        RoomUid = CurrentRoomUid,
                    },
                });
                break;
            }

            case EventId.EGS_KNM_INVITE_RESULT_NOT:
            {
                if (ActiveUnit == null) break;
                var not = ev.GetPayload<KPacketOK>(Region);
                SendPacket(EventId.EGS_KNM_INVITE_RESULT_NOT, not);
                break;
            }

            case EventId.EGS_KNM_REQUEST_NEW_FRIEND_INFO_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_KNM_REQUEST_NEW_FRIEND_INFO_REQ>(Region);
                if (KStringFilterManager.IsInvalidMarkIn(req.UnitNickName))
                {
                    SendPacket(EventId.EGS_KNM_REQUEST_NEW_FRIEND_INFO_ACK, new KEGS_KNM_REQUEST_NEW_FRIEND_INFO_ACK
                    {
                        Ok = NetError.ERR_ODBC_00,
                    });
                    break;
                }
                if (string.Equals(req.UnitNickName, ActiveUnit.NickName, StringComparison.Ordinal))
                {
                    SendPacket(EventId.EGS_KNM_REQUEST_NEW_FRIEND_INFO_ACK, new KEGS_KNM_REQUEST_NEW_FRIEND_INFO_ACK
                    {
                        Ok = NetError.ERR_KNM_00,
                    });
                    break;
                }
                if (_lastKnmRefreshUtc != DateTime.MinValue &&
                    (DateTime.UtcNow - _lastKnmRefreshUtc).TotalSeconds < 3.0)
                {
                    SendPacket(EventId.EGS_KNM_REQUEST_NEW_FRIEND_INFO_ACK, new KEGS_KNM_REQUEST_NEW_FRIEND_INFO_ACK
                    {
                        Ok = NetError.ERR_KNM_01,
                    });
                    break;
                }
                _lastKnmRefreshUtc = DateTime.UtcNow;
                var target = _server.GetUserByNickName(req.UnitNickName)
                    ?? (req.UnitUid > 0 ? _server.GetUserByUnitUid(req.UnitUid) : null);
                var dbTarget = target?.ActiveUnit ?? _server.Db.GetUnitByNickName(Region, req.UnitNickName);
                if (dbTarget == null)
                {
                    SendPacket(EventId.EGS_KNM_REQUEST_NEW_FRIEND_INFO_ACK, new KEGS_KNM_REQUEST_NEW_FRIEND_INFO_ACK
                    {
                        Ok = NetError.ERR_KNM_02,
                        UnitNickName = req.UnitNickName,
                    });
                    break;
                }
                uint sn = _server.Db.GetMessengerSerialNum(dbTarget.UnitUid);
                SendPacket(EventId.EGS_KNM_REQUEST_NEW_FRIEND_INFO_ACK, new KEGS_KNM_REQUEST_NEW_FRIEND_INFO_ACK
                {
                    Ok = NetError.NET_OK,
                    KnmSerialNum = sn,
                    UnitNickName = dbTarget.NickName,
                });
                break;
            }

            case EventId.EGS_KNM_TOGETHER_REQ:
            {
                if (ActiveUnit == null) break;
                var ks = new KSerializer(Region);
                ks.BeginReading(ev.Buffer);
                ks.GetWideString(out string targetNick);
                ks.EndReading();
                var target = _server.GetUserByNickName(targetNick);
                if (target?.ActiveUnit == null)
                {
                    SendPacket(EventId.EGS_KNM_TOGETHER_ACK, new KEGS_KNM_TOGETHER_ACK
                    {
                        Ok = NetError.ERR_KNM_02,
                    });
                    break;
                }
                SendPacket(EventId.EGS_KNM_TOGETHER_ACK, new KEGS_KNM_TOGETHER_ACK
                {
                    Ok = NetError.NET_OK,
                    UnitInfo = new KNMUnitInfo
                    {
                        UnitUid = target.ActiveUnit.UnitUid,
                        State = (int)target.Fsm.CurrentStateId,
                        DungeonId = 0,
                        RoomUid = target.CurrentRoomUid,
                    },
                });
                break;
            }

            // ── Relationship / Wedding / CallMyLover (GSUserRelationship.cpp) ─
            case EventId.EGS_CHECK_NICK_NAME_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_CHECK_NICK_NAME_REQ>(Region);
                if (KStringFilterManager.IsInvalidMarkIn(req.NickName))
                {
                    SendPacket(EventId.EGS_CHECK_NICK_NAME_ACK, new KEGS_CHECK_NICK_NAME_ACK
                    {
                        Ok = NetError.ERR_ODBC_00,
                        NickName = req.NickName,
                    });
                    break;
                }
                if (string.Equals(req.NickName, ActiveUnit.NickName, StringComparison.Ordinal))
                {
                    SendPacket(EventId.EGS_CHECK_NICK_NAME_ACK, new KEGS_CHECK_NICK_NAME_ACK
                    {
                        Ok = NetError.ERR_RELATIONSHIP_05,
                        NickName = req.NickName,
                    });
                    break;
                }
                var targetUnit = _server.Db.GetUnitByNickName(Region, req.NickName);
                SendPacket(EventId.EGS_CHECK_NICK_NAME_ACK, new KEGS_CHECK_NICK_NAME_ACK
                {
                    Ok = targetUnit != null ? NetError.NET_OK : NetError.ERR_SEARCH_UNIT_03,
                    UnitUid = targetUnit?.UnitUid ?? 0,
                    NickName = targetUnit?.NickName ?? req.NickName,
                });
                break;
            }

            case EventId.EGS_WEDDING_PROPOSE_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_WEDDING_PROPOSE_REQ>(Region);
                if (!RelationshipManager.IsCouple || RelationshipManager.LoverUnitUid <= 0)
                {
                    SendPacket(EventId.EGS_WEDDING_PROPOSE_ACK, new KEGS_WEDDING_PROPOSE_ACK { Ok = NetError.ERR_RELATIONSHIP_20 });
                    break;
                }
                if (RelationshipManager.IsMarried)
                {
                    SendPacket(EventId.EGS_WEDDING_PROPOSE_ACK, new KEGS_WEDDING_PROPOSE_ACK { Ok = NetError.ERR_RELATIONSHIP_22 });
                    break;
                }
                if (!KUserRelationshipManager.IsValidWeddingHall(req.WeddingHallType) ||
                    !KUserRelationshipManager.IsValidWeddingOfficiantNpc(req.OfficiantNpc))
                {
                    SendPacket(EventId.EGS_WEDDING_PROPOSE_ACK, new KEGS_WEDDING_PROPOSE_ACK { Ok = NetError.ERR_RELATIONSHIP_24 });
                    break;
                }
                if (KStringFilterManager.IsInvalidMarkInForLetter(req.WeddingMsg))
                {
                    SendPacket(EventId.EGS_WEDDING_PROPOSE_ACK, new KEGS_WEDDING_PROPOSE_ACK { Ok = NetError.ERR_ODBC_00 });
                    break;
                }
                if (!_server.StringFilterManager.CheckIsValidString(KStringFilterManager.FilterType.FtWord, req.WeddingMsg))
                {
                    SendPacket(EventId.EGS_WEDDING_PROPOSE_ACK, new KEGS_WEDDING_PROPOSE_ACK { Ok = NetError.ERR_RELATIONSHIP_29 });
                    break;
                }
                int hallItemId = KUserRelationshipManager.GetWeddingHallItem(req.WeddingHallType);
                var reqItems = new Dictionary<int, int>
                {
                    [KUserRelationshipManager.SiWeddingReservationItem] = 1,
                    [hallItemId] = 1,
                };
                if (!_server.Db.HasItemsInInventory(ActiveUnit.UnitUid, reqItems))
                {
                    SendPacket(EventId.EGS_WEDDING_PROPOSE_ACK, new KEGS_WEDDING_PROPOSE_ACK { Ok = NetError.ERR_RELATIONSHIP_23 });
                    break;
                }
                var (_, updatedSlots) = _server.Db.ConsumeItemsByItemId(ActiveUnit.UnitUid, reqItems);
                long weddingDateUnix = req.WeddingDate > 0 ? req.WeddingDate : DateTimeOffset.UtcNow.AddDays(1).ToUnixTimeSeconds();
                string weddingDateStr = DateTimeOffset.FromUnixTimeSeconds(weddingDateUnix).UtcDateTime.ToString("yyyy-MM-dd HH:mm:ss");
                int weddingUid = _server.Db.CreateWedding(
                    Region, ActiveUnit.UnitUid, RelationshipManager.LoverUnitUid,
                    req.WeddingHallType, req.OfficiantNpc, weddingDateStr, req.WeddingMsg);
                _server.RegisterWeddingHall(new KWeddingHallInfo
                {
                    WeddingUid = weddingUid,
                    WeddingHallType = req.WeddingHallType,
                    OfficiantNpc = req.OfficiantNpc,
                    Groom = ActiveUnit.UnitUid,
                    Bride = RelationshipManager.LoverUnitUid,
                    WeddingDateStr = weddingDateStr,
                    WeddingDate = weddingDateUnix,
                    WeddingMsg = req.WeddingMsg,
                });
                SendPacket(EventId.EGS_WEDDING_PROPOSE_ACK, new KEGS_WEDDING_PROPOSE_ACK
                {
                    Ok = NetError.NET_OK,
                    UpdatedInventorySlot = updatedSlots,
                });
                break;
            }

            case EventId.EGS_WEDDING_ITEM_INFO_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_WEDDING_ITEM_INFO_REQ>(Region);
                var invByUid = _server.Db.GetInventory(ActiveUnit.UnitUid).ToDictionary(i => i.ItemUid);
                var result = new Dictionary<long, KWeddingItemInfo>();
                foreach (long itemUid in req.WeddingItemList)
                {
                    if (!invByUid.TryGetValue(itemUid, out var invItem)) continue;
                    int weddingUid = invItem.SlotId > 0 ? invItem.SlotId : (int)(itemUid & 0x7FFF_FFFF);
                    var hall = _server.GetWeddingHall(weddingUid) ?? _server.Db.GetWeddingInfo(weddingUid);
                    if (hall == null) continue;
                    var groomUnit = _server.Db.GetUnitByUid(Region, hall.Groom);
                    var brideUnit = _server.Db.GetUnitByUid(Region, hall.Bride);
                    result[itemUid] = new KWeddingItemInfo
                    {
                        ItemUid = itemUid,
                        WeddingUid = hall.WeddingUid,
                        WeddingHallType = hall.WeddingHallType,
                        OfficiantNpc = hall.OfficiantNpc,
                        Groom = hall.Groom,
                        Bride = hall.Bride,
                        GroomNick = groomUnit?.NickName ?? string.Empty,
                        BrideNick = brideUnit?.NickName ?? string.Empty,
                        WeddingDate = hall.WeddingDateStr,
                        WeddingMsg = hall.WeddingMsg,
                    };
                }
                SendPacket(EventId.EGS_WEDDING_ITEM_INFO_ACK, new KEGS_WEDDING_ITEM_INFO_ACK
                {
                    Ok = NetError.NET_OK,
                    WeddingItemInfo = result,
                });
                break;
            }

            case EventId.EGS_CHECK_BREAK_UP_REQ:
            {
                if (ActiveUnit == null) break;
                if (!RelationshipManager.IsCouple && !RelationshipManager.IsMarried)
                {
                    SendPacket(EventId.EGS_CHECK_BREAK_UP_ACK, new KEGS_CHECK_BREAK_UP_ACK { Ok = NetError.ERR_RELATIONSHIP_14 });
                    break;
                }
                int cost = RelationshipManager.GetBreakUpCost();
                SendPacket(EventId.EGS_CHECK_BREAK_UP_ACK, new KEGS_CHECK_BREAK_UP_ACK
                {
                    Ok = NetError.NET_OK,
                    Cost = cost,
                    RelationshipType = (sbyte)RelationshipManager.CurrentType,
                    LoverNickName = RelationshipManager.LoverNickName,
                });
                break;
            }

            case EventId.EGS_START_WEDDING_REQ:
            {
                if (ActiveUnit == null) break;
                if (!RelationshipManager.IsCouple)
                {
                    SendPacket(EventId.EGS_START_WEDDING_ACK, new KPacketOK { Ok = NetError.ERR_RELATIONSHIP_36 });
                    break;
                }
                var room = CurrentRoomUid > 0 ? _server.Center.RoomManager.GetRoom(CurrentRoomUid) : null;
                if (room == null || room.Info.RoomType != 8)
                {
                    SendPacket(EventId.EGS_START_WEDDING_ACK, new KPacketOK { Ok = NetError.ERR_RELATIONSHIP_39 });
                    break;
                }
                var hall = _server.GetWeddingHallByRoom(CurrentRoomUid);
                if (hall == null || (hall.Groom != ActiveUnit.UnitUid && hall.Bride != ActiveUnit.UnitUid))
                {
                    SendPacket(EventId.EGS_START_WEDDING_ACK, new KPacketOK { Ok = NetError.ERR_RELATIONSHIP_41 });
                    break;
                }
                long partnerUid = hall.Groom == ActiveUnit.UnitUid ? hall.Bride : hall.Groom;
                var partner = _server.GetUserByUnitUid(partnerUid);
                if (partner == null || !room.GetMemberUnitUids().Contains(partnerUid))
                {
                    SendPacket(EventId.EGS_START_WEDDING_ACK, new KPacketOK { Ok = NetError.ERR_RELATIONSHIP_42 });
                    break;
                }
                SendPacket(EventId.EGS_START_WEDDING_ACK, new KPacketOK { Ok = NetError.NET_OK });
                partner.SendID(EventId.EGS_START_WEDDING_ASK_REQ);
                break;
            }

            case EventId.EGS_START_WEDDING_ASK_ACK:
            {
                if (ActiveUnit == null) break;
                if (!RelationshipManager.IsCouple) break;
                var ack = ev.GetPayload<KEGS_START_WEDDING_ASK_ACK>(Region);
                var room = CurrentRoomUid > 0 ? _server.Center.RoomManager.GetRoom(CurrentRoomUid) : null;
                if (room == null || room.Info.RoomType != 8) break;
                var hall = _server.GetWeddingHallByRoom(CurrentRoomUid);
                long partnerUid = hall != null
                    ? (hall.Groom == ActiveUnit.UnitUid ? hall.Bride : hall.Groom)
                    : RelationshipManager.LoverUnitUid;
                var partner = _server.GetUserByUnitUid(partnerUid);
                partner?.SendPacket(EventId.EGS_START_WEDDING_AGREE_NOT, new KEGS_START_WEDDING_ASK_ACK { Agree = ack.Agree });
                if (ack.Agree)
                {
                    DateTime now = DateTime.UtcNow;
                    RelationshipManager.SetRelationshipType(KUserRelationshipManager.RelationshipType.RtMarried);
                    RelationshipManager.AnniversaryDayUtc = now;
                    RelationshipManager.LastRewardDateUtc = now;
                    _server.Db.SaveRelationship(ActiveUnit.UnitUid, (sbyte)KUserRelationshipManager.RelationshipType.RtMarried,
                        partnerUid, partner?.ActiveUnit?.NickName ?? RelationshipManager.LoverNickName,
                        RelationshipManager.LoveWord, now, now, RelationshipManager.RewardTitleType);
                    if (partner?.ActiveUnit != null)
                    {
                        partner.RelationshipManager.SetRelationshipType(KUserRelationshipManager.RelationshipType.RtMarried);
                        partner.RelationshipManager.AnniversaryDayUtc = now;
                        partner.RelationshipManager.LastRewardDateUtc = now;
                        _server.Db.SaveRelationship(partner.ActiveUnit.UnitUid, (sbyte)KUserRelationshipManager.RelationshipType.RtMarried,
                            ActiveUnit.UnitUid, ActiveUnit.NickName,
                            partner.RelationshipManager.LoveWord, now, now, partner.RelationshipManager.RewardTitleType);
                    }
                    if (hall != null)
                    {
                        hall.Success = true;
                        _server.Db.CompleteWedding(hall.WeddingUid);
                    }
                    var startNot = new KEGS_START_WEDDING_NOT { Ok = NetError.NET_OK };
                    foreach (long mUid in room.GetMemberUnitUids())
                        _server.SendToUnit(mUid, EventId.EGS_START_WEDDING_NOT, startNot);
                }
                break;
            }

            case EventId.EGS_CHANGE_LOVE_WORD_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_CHANGE_LOVE_WORD_REQ>(Region);
                if (!RelationshipManager.IsCouple && !RelationshipManager.IsMarried)
                {
                    SendPacket(EventId.EGS_CHANGE_LOVE_WORD_ACK, new KPacketOK { Ok = NetError.ERR_RELATIONSHIP_14 });
                    break;
                }
                if (KStringFilterManager.IsInvalidMarkInForLetter(req.NewLoveWord))
                {
                    SendPacket(EventId.EGS_CHANGE_LOVE_WORD_ACK, new KPacketOK { Ok = NetError.ERR_ODBC_00 });
                    break;
                }
                if (!_server.StringFilterManager.CheckIsValidString(KStringFilterManager.FilterType.FtWord, req.NewLoveWord))
                {
                    SendPacket(EventId.EGS_CHANGE_LOVE_WORD_ACK, new KPacketOK { Ok = NetError.ERR_RELATIONSHIP_29 });
                    break;
                }
                if (req.NewLoveWord.Length > 40)
                {
                    SendPacket(EventId.EGS_CHANGE_LOVE_WORD_ACK, new KPacketOK { Ok = NetError.ERR_RELATIONSHIP_43 });
                    break;
                }
                var reqItem = new Dictionary<int, int> { [KUserRelationshipManager.SiLoveWordChangeItem] = 1 };
                if (!_server.Db.HasItemsInInventory(ActiveUnit.UnitUid, reqItem))
                {
                    SendPacket(EventId.EGS_CHANGE_LOVE_WORD_ACK, new KPacketOK { Ok = NetError.ERR_RELATIONSHIP_44 });
                    break;
                }
                var (_, updatedSlots) = _server.Db.ConsumeItemsByItemId(ActiveUnit.UnitUid, reqItem);
                RelationshipManager.LoveWord = req.NewLoveWord;
                _server.Db.UpdateLoveWord(ActiveUnit.UnitUid, RelationshipManager.LoverUnitUid, req.NewLoveWord);
                var partner = _server.GetUserByUnitUid(RelationshipManager.LoverUnitUid);
                if (partner?.ActiveUnit != null)
                    partner.RelationshipManager.LoveWord = req.NewLoveWord;
                SendPacket(EventId.EGS_CHANGE_LOVE_WORD_ACK, new KPacketOK { Ok = NetError.NET_OK });
                var wordNot = new KEGS_CHANGE_LOVE_WORD_NOT
                {
                    UseItemUnitUid = ActiveUnit.UnitUid,
                    NewLoveWord = req.NewLoveWord,
                    UpdatedInventorySlot = updatedSlots,
                };
                SendPacket(EventId.EGS_CHANGE_LOVE_WORD_NOT, wordNot);
                partner?.SendPacket(EventId.EGS_CHANGE_LOVE_WORD_NOT, wordNot);
                break;
            }

            case EventId.EGS_JOIN_WEDDING_HALL_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_JOIN_WEDDING_HALL_REQ>(Region);
                var invItem = _server.Db.GetInventory(ActiveUnit.UnitUid).FirstOrDefault(i => i.ItemUid == req.ItemUid);
                if (invItem == null)
                {
                    SendPacket(EventId.EGS_JOIN_WEDDING_HALL_ACK, new KEGS_JOIN_WEDDING_HALL_ACK { Ok = NetError.ERR_RELATIONSHIP_31 });
                    break;
                }
                if (!KUserRelationshipManager.IsWeddingItem(invItem.ItemInfo.ItemId, out _))
                {
                    SendPacket(EventId.EGS_JOIN_WEDDING_HALL_ACK, new KEGS_JOIN_WEDDING_HALL_ACK { Ok = NetError.ERR_RELATIONSHIP_32 });
                    break;
                }
                var hall = _server.GetWeddingHall(req.WeddingUid) ?? _server.Db.GetWeddingInfo(req.WeddingUid);
                if (hall == null)
                {
                    SendPacket(EventId.EGS_JOIN_WEDDING_HALL_ACK, new KEGS_JOIN_WEDDING_HALL_ACK { Ok = NetError.ERR_RELATIONSHIP_34 });
                    break;
                }
                _server.RegisterWeddingHall(hall);
                var room = hall.RoomUid > 0 ? _server.Center.RoomManager.GetRoom(hall.RoomUid) : null;
                if (room == null)
                {
                    room = _server.Center.RoomManager.CreateRoom(new KRoomInfo
                    {
                        RoomType = 8,
                        RoomName = $"WeddingHall_{hall.WeddingUid}",
                        MaxSlot = 30,
                        Public = true,
                    }, BuildRoomUserInfo());
                    _server.BindWeddingHallRoom(hall.WeddingUid, room.Info.RoomUid);
                }
                else
                {
                    if (CurrentRoomUid > 0 && CurrentRoomUid != room.Info.RoomUid)
                        _server.Center.RoomManager.LeaveRoom(CurrentRoomUid, ActiveUnit.UnitUid);
                    _server.Center.RoomManager.JoinRoom(room.Info.RoomUid, BuildRoomUserInfo(), string.Empty, false);
                }
                LeaveCurrentField();
                CurrentRoomUid = room.Info.RoomUid;
                Fsm.TryStateTransition(KGSFSM.I_TO_ROOM, out _);
                var slots = room.SnapshotSlots();
                SendPacket(EventId.EGS_JOIN_WEDDING_HALL_ACK, new KEGS_JOIN_WEDDING_HALL_ACK
                {
                    Ok = NetError.NET_OK,
                    WeddingInfo = hall,
                    RoomInfo = room.Info,
                    Slots = slots,
                    CnIp = "127.0.0.1",
                    UdpRelayIp = _server.Center.RoomManager.DefaultUdpRelayIp,
                    UdpRelayPort = _server.Center.RoomManager.DefaultUdpRelayPort,
                });
                var joinSlot = slots.FirstOrDefault(s => s.RoomUserInfo.UnitUid == ActiveUnit.UnitUid);
                if (joinSlot != null)
                {
                    var joinNot = new KEGS_JOIN_ROOM_NOT { JoinSlot = joinSlot };
                    foreach (long mUid in room.GetMemberUnitUids())
                        if (mUid != ActiveUnit.UnitUid)
                            _server.SendToUnit(mUid, EventId.EGS_JOIN_WEDDING_HALL_NOT, joinNot);
                }
                break;
            }

            case EventId.EGS_LEAVE_WEDDING_HALL_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_LEAVE_ROOM_REQ>(Region);
                if (CurrentRoomUid > 0)
                {
                    var room = _server.Center.RoomManager.GetRoom(CurrentRoomUid);
                    _server.Center.RoomManager.LeaveRoom(CurrentRoomUid, ActiveUnit.UnitUid);
                    CurrentRoomUid = 0;
                    Fsm.TryStateTransition(KGSFSM.I_TO_FIELD_MAP, out _);
                    if (room != null)
                    {
                        var leaveNot = new KEGS_CREATE_ROOM_ACK
                        {
                            Ok = NetError.NET_OK,
                            RoomInfo = room.Info,
                            Slots = room.SnapshotSlots(),
                        };
                        foreach (long mUid in room.GetMemberUnitUids())
                            _server.SendToUnit(mUid, EventId.EGS_LEAVE_ROOM_NOT, leaveNot);
                    }
                }
                SendPacket(EventId.EGS_LEAVE_WEDDING_HALL_ACK, new KPacketOK { Ok = NetError.NET_OK });
                break;
            }

            case EventId.EGS_CALL_MY_LOVER_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_CALL_MY_LOVER_REQ>(Region);
                if (RelationshipManager.IsSolo)
                {
                    SendPacket(EventId.EGS_CALL_MY_LOVER_ACK, new KPacketOK { Ok = NetError.ERR_RELATIONSHIP_27 });
                    break;
                }
                var room = CurrentRoomUid > 0 ? _server.Center.RoomManager.GetRoom(CurrentRoomUid) : null;
                if (room != null && room.Info.RoomType is not (3 or 7))
                {
                    SendPacket(EventId.EGS_CALL_MY_LOVER_ACK, new KPacketOK { Ok = NetError.ERR_RELATIONSHIP_28 });
                    break;
                }
                var partner = _server.GetUserByUnitUid(RelationshipManager.LoverUnitUid);
                if (partner?.ActiveUnit == null)
                {
                    SendPacket(EventId.EGS_CALL_MY_LOVER_ACK, new KPacketOK { Ok = NetError.ERR_RELATIONSHIP_03 });
                    break;
                }
                SendPacket(EventId.EGS_CALL_MY_LOVER_ACK, new KPacketOK { Ok = NetError.NET_OK });
                if (room != null)
                {
                    partner.SendPacket(EventId.EGS_CALL_MY_LOVER_ROOM_NOT, new KEGS_CALL_MY_LOVER_ROOM_NOT
                    {
                        BattleFieldId = room.Info.DungeonId > 0 ? room.Info.DungeonId : 1,
                        RoomUid = room.Info.RoomUid,
                        Pos = req.Pos,
                        LastTouchLineIndex = req.LastTouchLineIndex,
                        CalledUserUid = AccountUid,
                        CalledGsUid = _server.Uid,
                        CallType = 0,
                    });
                }
                else
                {
                    partner.SendPacket(EventId.EGS_CALL_MY_LOVER_VILLAGE_NOT, new KEGS_CALL_MY_LOVER_VILLAGE_NOT
                    {
                        MapId = req.MapId > 0 ? req.MapId : (CurrentFieldMapId > 0 ? CurrentFieldMapId : ActiveUnit.LastPos.MapId),
                        Pos = req.Pos,
                        LastTouchLineIndex = req.LastTouchLineIndex,
                        CalledUserUid = AccountUid,
                        CalledGsUid = _server.Uid,
                        CallType = 0,
                    });
                }
                break;
            }

            case EventId.EGS_CALL_MY_LOVER_AGREE_VILLAGE_NOT:
            {
                if (ActiveUnit == null) break;
                var not = ev.GetPayload<KEGS_CALL_MY_LOVER_AGREE_VILLAGE_NOT>(Region);
                var partner = _server.GetUserByUnitUid(RelationshipManager.LoverUnitUid);
                if (partner != null)
                {
                    var resEv = new KEvent { EventId = EventId.EGS_CALL_MY_LOVER_AGREE_RESULT_NOT };
                    resEv.DestPerformer.PerformerId = PerformerId.PI_GS_PROXY;
                    resEv.DestPerformer.AddUID(partner.Uid);
                    var w = new KSerializer(Region);
                    w.BeginWriting(resEv.Buffer);
                    w.Put(not.Agree);
                    w.EndWriting();
                    _ = partner.Session.SendEventAsync(resEv);
                }
                if (not.Agree)
                {
                    int targetMap = not.MapId > 0 ? not.MapId : (partner?.CurrentFieldMapId ?? ActiveUnit.LastPos.MapId);
                    LeaveCurrentField();
                    CurrentFieldMapId = targetMap;
                    var zone = _server.FieldManager.GetOrCreateZone(targetMap);
                    zone.Enter(BuildFieldUserInfo());
                    SendPacket(EventId.EGS_CALL_MY_LOVER_STATE_CHANGE_FIELD_NOT, new KEGS_CALL_MY_LOVER_STATE_CHANGE_FIELD_NOT
                    {
                        Ok = NetError.NET_OK,
                        MapId = targetMap,
                        Pos = not.Pos,
                        LastTouchLineIndex = not.LastTouchLineIndex,
                    });
                }
                break;
            }

            case EventId.EGS_CALL_MY_LOVER_AGREE_ROOM_NOT:
            {
                if (ActiveUnit == null) break;
                var not = ev.GetPayload<KEGS_CALL_MY_LOVER_AGREE_ROOM_NOT>(Region);
                var partner = _server.GetUserByUnitUid(RelationshipManager.LoverUnitUid);
                if (partner != null)
                {
                    var resEv = new KEvent { EventId = EventId.EGS_CALL_MY_LOVER_AGREE_RESULT_NOT };
                    resEv.DestPerformer.PerformerId = PerformerId.PI_GS_PROXY;
                    resEv.DestPerformer.AddUID(partner.Uid);
                    var w = new KSerializer(Region);
                    w.BeginWriting(resEv.Buffer);
                    w.Put(not.Agree);
                    w.EndWriting();
                    _ = partner.Session.SendEventAsync(resEv);
                }
                if (not.Agree && partner != null && partner.CurrentRoomUid > 0)
                {
                    var room = _server.Center.RoomManager.GetRoom(partner.CurrentRoomUid);
                    if (room != null)
                    {
                        if (CurrentRoomUid > 0 && CurrentRoomUid != room.Info.RoomUid)
                            _server.Center.RoomManager.LeaveRoom(CurrentRoomUid, ActiveUnit.UnitUid);
                        _server.Center.RoomManager.JoinRoom(room.Info.RoomUid, BuildRoomUserInfo(), string.Empty, false);
                        LeaveCurrentField();
                        CurrentRoomUid = room.Info.RoomUid;
                        Fsm.TryStateTransition(KGSFSM.I_TO_ROOM, out _);
                        int bfId = not.BattleFieldId > 0 ? not.BattleFieldId : (room.Info.DungeonId > 0 ? room.Info.DungeonId : 1);
                        SendPacket(EventId.EGS_CALL_MY_LOVER_JOIN_BATTLE_FIELD_NOT, new KEGS_CALL_MY_LOVER_JOIN_BATTLE_FIELD_NOT
                        {
                            Ok = NetError.NET_OK,
                            BattleFieldJoinInfo = new KBattleFieldJoinInfo
                            {
                                BattleFieldId = bfId,
                                StartPosIndex = not.LastTouchLineIndex,
                            },
                            RoomInfo = room.Info,
                            Slots = room.SnapshotSlots(),
                            CnIp = "127.0.0.1",
                            Pos = not.Pos,
                            UdpRelayIp = _server.Center.RoomManager.DefaultUdpRelayIp,
                            UdpRelayPort = _server.Center.RoomManager.DefaultUdpRelayPort,
                            CalledUserUid = not.CalledUserUid,
                            CalledGsUid = not.CalledGsUid,
                        });
                    }
                }
                break;
            }

            case EventId.EGS_RELATIONSHIP_INFO_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_RELATIONSHIP_INFO_REQ>(Region);
                if (RelationshipManager.IsSolo && req.AcceptUnitUid <= 0)
                {
                    SendPacket(EventId.EGS_RELATIONSHIP_INFO_ACK, new KEGS_RELATIONSHIP_INFO_ACK { Ok = NetError.ERR_RELATIONSHIP_14 });
                    break;
                }
                long partnerUid = req.AcceptUnitUid > 0 ? req.AcceptUnitUid : RelationshipManager.LoverUnitUid;
                var partnerOnline = _server.GetUserByUnitUid(partnerUid);
                var partnerUnit = partnerOnline?.ActiveUnit ?? _server.Db.GetUnitByUid(Region, partnerUid);
                var equipped = _server.Db.GetUnitEquippedItemsMap(partnerUid);
                SendPacket(EventId.EGS_RELATIONSHIP_INFO_ACK, new KEGS_RELATIONSHIP_INFO_ACK
                {
                    Ok = partnerUnit != null ? NetError.NET_OK : NetError.ERR_RELATIONSHIP_03,
                    RequestGsUid = _server.Uid,
                    RequestUnitUid = ActiveUnit.UnitUid,
                    AcceptUnitUid = partnerUid,
                    MyLevel = (byte)ActiveUnit.Level,
                    MyUnitClass = ActiveUnit.UnitClass,
                    MyMapId = CurrentFieldMapId > 0 ? CurrentFieldMapId : ActiveUnit.LastPos.MapId,
                    MyPosition = KFriendInfo.FpOnLine,
                    MyChannelName = _server.Name,
                    OtherUnitEquippedItem = equipped,
                    Level = (byte)(partnerUnit?.Level ?? 1),
                    UnitClass = partnerUnit?.UnitClass ?? 0,
                    MapId = partnerOnline != null
                        ? (partnerOnline.CurrentFieldMapId > 0 ? partnerOnline.CurrentFieldMapId : partnerOnline.ActiveUnit!.LastPos.MapId)
                        : (partnerUnit?.LastPos.MapId ?? 0),
                    IsConnect = partnerOnline != null,
                    ChannelName = partnerOnline != null ? _server.Name : string.Empty,
                    Position = partnerOnline != null ? KFriendInfo.FpOnLine : KFriendInfo.FpOffLine,
                });
                break;
            }

            case EventId.EGS_RELATIONSHIP_EFFECT_NOT:
            {
                if (ActiveUnit == null) break;
                var not = ev.GetPayload<KEGS_RELATIONSHIP_EFFECT_NOT>(Region);
                if (CurrentRoomUid > 0)
                {
                    var room = _server.Center.RoomManager.GetRoom(CurrentRoomUid);
                    if (room != null)
                        foreach (long mUid in room.GetMemberUnitUids())
                            _server.SendToUnit(mUid, EventId.EGS_RELATIONSHIP_EFFECT_NOT, not);
                }
                else if (CurrentFieldMapId > 0)
                {
                    var zone = _server.FieldManager.GetZone(CurrentFieldMapId);
                    if (zone != null)
                        foreach (long otherUid in zone.GetAllUnitUids())
                            _server.SendToUnit(otherUid, EventId.EGS_RELATIONSHIP_EFFECT_NOT, not);
                }
                else
                {
                    SendPacket(EventId.EGS_RELATIONSHIP_EFFECT_NOT, not);
                }
                break;
            }

            // ── Match / PartyType / PvP NPC (GSUserMatch.cpp) ────────────────
            case EventId.EGS_CHANGE_PARTY_TYPE_REQ:
            {
                if (ActiveUnit == null) break;
                var reqPartyInfo = ev.GetPayload<KPartyInfo>(Region);
                var party = CurrentPartyUid > 0 ? _server.Center.PartyManager.GetParty(CurrentPartyUid) : null;
                if (party == null)
                {
                    SendPacket(EventId.EGS_CHANGE_PARTY_TYPE_ACK, new KPacketOK { Ok = NetError.ERR_PARTY_03 });
                    break;
                }
                if (!party.ChangePartyType(ActiveUnit.UnitUid, reqPartyInfo))
                {
                    SendPacket(EventId.EGS_CHANGE_PARTY_TYPE_ACK, new KPacketOK { Ok = NetError.ERR_PARTY_01 });
                    break;
                }
                SendPacket(EventId.EGS_CHANGE_PARTY_TYPE_ACK, new KPacketOK { Ok = NetError.NET_OK });
                var partyTypeNot = new KEGS_CHANGE_PARTY_TYPE_NOT { PartyInfo = party.Info };
                foreach (long mUid in party.GetUnitUids())
                    _server.SendToUnit(mUid, EventId.EGS_CHANGE_PARTY_TYPE_NOT, partyTypeNot);
                break;
            }

            case EventId.EGS_PVP_PARTY_CHANGE_MATCH_INFO_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_PVP_PARTY_CHANGE_MATCH_INFO_REQ>(Region);
                var party = CurrentPartyUid > 0 ? _server.Center.PartyManager.GetParty(CurrentPartyUid) : null;
                if (party == null)
                {
                    SendPacket(EventId.EGS_PVP_PARTY_CHANGE_MATCH_INFO_ACK, new KPacketOK { Ok = NetError.ERR_PARTY_03 });
                    break;
                }
                if (!party.ChangePvpMatchInfo(ActiveUnit.UnitUid, req.PvpGameType))
                {
                    SendPacket(EventId.EGS_PVP_PARTY_CHANGE_MATCH_INFO_ACK, new KPacketOK { Ok = NetError.ERR_PARTY_01 });
                    break;
                }
                SendPacket(EventId.EGS_PVP_PARTY_CHANGE_MATCH_INFO_ACK, new KPacketOK { Ok = NetError.NET_OK });
                foreach (long mUid in party.GetUnitUids())
                    _server.SendToUnit(mUid, EventId.EGS_PVP_PARTY_CHANGE_MATCH_INFO_NOT, req);
                break;
            }

            case EventId.EGS_PVP_NPC_UNIT_RE_BIRTH_POS_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_PVP_NPC_UNIT_RE_BIRTH_POS_REQ>(Region);
                var room = CurrentRoomUid > 0 ? _server.Center.RoomManager.GetRoom(CurrentRoomUid) : null;
                if (room == null)
                {
                    SendPacket(EventId.EGS_PVP_NPC_UNIT_RE_BIRTH_POS_ACK, new KPacketOK { Ok = NetError.ERR_ROOM_00 });
                    break;
                }
                room.UpdatePvpNpcRebirthPos(req.PvpNpcUnitUid, req.StartPosIndex);
                SendPacket(EventId.EGS_PVP_NPC_UNIT_RE_BIRTH_POS_ACK, new KPacketOK { Ok = NetError.NET_OK });
                break;
            }

            case EventId.EGS_PVP_NPC_UNIT_INFO_TO_SERVER_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_PVP_NPC_UNIT_INFO_TO_SERVER_REQ>(Region);
                var room = CurrentRoomUid > 0 ? _server.Center.RoomManager.GetRoom(CurrentRoomUid) : null;
                if (room == null)
                {
                    SendPacket(EventId.EGS_PVP_NPC_UNIT_INFO_TO_SERVER_ACK, new KPacketOK { Ok = NetError.ERR_ROOM_00 });
                    break;
                }
                room.UpdatePvpNpcInfo(req.PvpNpcInfo);
                SendPacket(EventId.EGS_PVP_NPC_UNIT_INFO_TO_SERVER_ACK, new KPacketOK { Ok = NetError.NET_OK });
                break;
            }

            // ── AutoParty / Giant (GSUserAutoParty.cpp, GSUserGiant.cpp) ─────
            case EventId.EGS_REQUEST_GET_AUTO_PARTY_BONUS_INFO_NOT:
            {
                if (ActiveUnit == null) break;
                var not = ev.GetPayload<KEGS_REQUEST_GET_AUTO_PARTY_BONUS_INFO_NOT>(Region);
                var bonusMap = new Dictionary<int, KAutoPartyDungeonBonusInfo>();
                Func<sbyte, int, bool> isBonus = (mode, did) =>
                    _server.AutoPartyManager.IsAutoPartyBonus(mode, did)
                    || _server.Global.AutoPartyManager.IsAutoPartyBonus(mode, did);
                if (not.DungeonList.Count > 0)
                {
                    KUserDungeonManager.GetAutoPartyBonusDungeonList(
                        not.DungeonList,
                        isBonus,
                        bonusMap);
                }
                else
                {
                    KUserDungeonManager.GetAutoPartyBonusDungeonList(
                        not.VillageMapId > 0 ? not.VillageMapId : (CurrentFieldMapId > 0 ? CurrentFieldMapId : ActiveUnit.LastPos.MapId),
                        isBonus,
                        bonusMap);
                }
                SendPacket(EventId.EGS_UPDATE_AUTO_PARTY_BONUS_INFO_NOT, new KEGS_UPDATE_AUTO_PARTY_BONUS_INFO_NOT
                {
                    DungeonBonusList = bonusMap,
                });
                break;
            }

            case EventId.EGS_ANTI_ADDICTION_NOT:
            {
                if (ActiveUnit == null) break;
                var not = ev.GetPayload<KEGS_ANTI_ADDICTION_NOT>(Region);
                _antiAddictionType = not.Type;
                _antiAddictionPlayMinutes = not.PlayTime;
                SendPacket(EventId.EGS_ANTI_ADDICTION_NOT, not);
                break;
            }

            // ── Dungeon Gameplay (GSUserDungeon.cpp) ───────────────────────
            case EventId.EGS_DUNGEON_SECRET_STAGE_ENTER_SELECT_NOT:
            {
                if (ActiveUnit == null || CurrentRoomUid <= 0 || Fsm.StateId != KGSFSM.S_ROOM) break;
                var not = ev.GetPayload<KEGS_DUNGEON_SECRET_STAGE_ENTER_SELECT_NOT>(Region);
                var room = _server.Center.RoomManager.GetRoom(CurrentRoomUid);
                if (room == null) break;
                var (allVoted, enterSecret) = room.VoteSecretStage(ActiveUnit.UnitUid, not.WantToEnterSecretStage);
                if (allVoted)
                {
                    var result = new KEGS_DUNGEON_SECRET_STAGE_ENTER_SELECT_NOT { WantToEnterSecretStage = enterSecret };
                    foreach (long uid in room.GetMemberUnitUids())
                        _server.SendToUnit(uid, EventId.EGS_DUNGEON_SECRET_STAGE_ENTER_SELECT_NOT, result);
                }
                break;
            }

            case EventId.EGS_STOP_DUNGEON_CONTINUE_TIME_REQ:
            {
                if (ActiveUnit == null || CurrentRoomUid <= 0 || Fsm.StateId != KGSFSM.S_ROOM) break;
                var req = ev.GetPayload<KEGS_STOP_DUNGEON_CONTINUE_TIME_REQ>(Region);
                var room = _server.Center.RoomManager.GetRoom(CurrentRoomUid);
                int ok = room != null ? room.SetStopContinueTime(ActiveUnit.UnitUid, req.IsStop) : NetError.ERR_ROOM_16;
                SendPacket(EventId.EGS_STOP_DUNGEON_CONTINUE_TIME_ACK, new KEGS_STOP_DUNGEON_CONTINUE_TIME_ACK
                {
                    Ok = ok,
                    IsStop = req.IsStop,
                });
                break;
            }

            case EventId.EGS_SKIP_CONTINUE_DUNGEON_NOT:
                if (ActiveUnit != null && CurrentRoomUid > 0)
                    ForwardRoomPacket(ev, EventId.ERM_SKIP_CONTINUE_DUNGEON_NOT);
                break;

            case EventId.EGS_WORLD_TRIGGER_RELOCATION_REQ:
            {
                if (ActiveUnit == null || Fsm.StateId != KGSFSM.S_ROOM) break;
                var req = ev.GetPayload<KEGS_WORLD_TRIGGER_RELOCATION_REQ>(Region);
                var room = CurrentRoomUid > 0 ? _server.Center.RoomManager.GetRoom(CurrentRoomUid) : null;
                if (room == null || room.Info.RoomState != (sbyte)RoomState.S_PLAY)
                {
                    SendPacket(EventId.EGS_WORLD_TRIGGER_RELOCATION_ACK, new KEGS_WORLD_TRIGGER_RELOCATION_ACK
                    {
                        Ok = room == null ? NetError.ERR_ROOM_16 : NetError.ERR_CLIENT_STATE,
                    });
                    break;
                }
                SendPacket(EventId.EGS_WORLD_TRIGGER_RELOCATION_ACK, new KEGS_WORLD_TRIGGER_RELOCATION_ACK { Ok = NetError.NET_OK });
                var not = new KEGS_WORLD_TRIGGER_RELOCATION_NOT { WorldTrigger = req.WorldTrigger };
                foreach (long uid in room.GetMemberUnitUids())
                    _server.SendToUnit(uid, EventId.EGS_WORLD_TRIGGER_RELOCATION_NOT, not);
                break;
            }

            case EventId.EGS_BAD_ATTITUDE_USER_CHECK_INFO_NOT:
            {
                if (ActiveUnit == null || CurrentRoomUid <= 0 || Fsm.StateId != KGSFSM.S_ROOM) break;
                var req = ev.GetPayload<KEGS_BAD_ATTITUDE_USER_CHECK_INFO_NOT>(Region);
                var room = _server.Center.RoomManager.GetRoom(CurrentRoomUid);
                if (room == null) break;
                var (warnUsers, exitUsers) = room.RecordBadAttitudeCheckInfo(
                    ActiveUnit.UnitUid, req.StageId, req.SubStageId, req.ComboScore, req.TechScore);
                foreach (long badUid in warnUsers)
                {
                    _server.SendToUnit(badUid, EventId.EGS_BAD_ATTITUDE_USER_MSG_NOT, new KEGS_BAD_ATTITUDE_USER_MSG_NOT
                    {
                        UserUid = badUid,
                        MsgType = KEGS_BAD_ATTITUDE_USER_MSG_NOT.MtBadAttitudeUserWarning,
                    });
                }
                foreach (long exitUid in exitUsers)
                {
                    foreach (long memberUid in room.GetMemberUnitUids())
                    {
                        _server.SendToUnit(memberUid, EventId.EGS_BAD_ATTITUDE_USER_MSG_NOT, new KEGS_BAD_ATTITUDE_USER_MSG_NOT
                        {
                            UserUid = exitUid,
                            MsgType = memberUid == exitUid
                                ? KEGS_BAD_ATTITUDE_USER_MSG_NOT.MtForcedExit
                                : KEGS_BAD_ATTITUDE_USER_MSG_NOT.MtBadAttitudeUserExit,
                        });
                    }
                }
                break;
            }

            case EventId.EGS_DEFENCE_DUNGEON_WAVE_ID_NOT:
            {
                if (ActiveUnit == null || CurrentRoomUid <= 0 || Fsm.StateId != KGSFSM.S_ROOM) break;
                var not = ev.GetPayload<KEGS_DEFENCE_DUNGEON_WAVE_ID_NOT>(Region);
                var room = _server.Center.RoomManager.GetRoom(CurrentRoomUid);
                if (room != null)
                    room.CurrentSubStageId = (sbyte)Math.Clamp(not.WaveId, sbyte.MinValue, sbyte.MaxValue);
                break;
            }

            case EventId.EGS_ENTER_DEFENCE_DUNGEON_POPUP_REPLY_NOT:
            {
                if (ActiveUnit == null || CurrentRoomUid <= 0 || Fsm.StateId != KGSFSM.S_ROOM) break;
                var not = ev.GetPayload<KEGS_ENTER_DEFENCE_DUNGEON_POPUP_REPLY_NOT>(Region);
                var room = _server.Center.RoomManager.GetRoom(CurrentRoomUid);
                if (room == null || room.Info.RoomType is not (1 or 2)) break;
                var (allPrepared, leaveUnitUids) = room.VoteEnterDefenceDungeon(ActiveUnit.UnitUid, not.Agree);
                if (allPrepared)
                {
                    foreach (long leaveUid in leaveUnitUids)
                        _server.Center.RoomManager.LeaveRoom(room.Info.RoomUid, leaveUid);
                }
                break;
            }

            case EventId.EGS_SYNC_DUNGEON_TIMER_NOT:
            {
                if (ActiveUnit == null || CurrentRoomUid <= 0 || Fsm.StateId != KGSFSM.S_ROOM) break;
                var room = _server.Center.RoomManager.GetRoom(CurrentRoomUid);
                if (room == null || !room.IsHost(ActiveUnit.UnitUid)) break;
                room.ValentineManager.SetStartTimer(KValentineManager.TimerSlot.AfterDungeonStart, true);
                var syncNot = new KEGS_SYNC_DUNGEON_TIMER_NOT
                {
                    RemainTime = room.ValentineManager.GetRemainTime(KValentineManager.TimerSlot.AfterDungeonStart),
                };
                foreach (long uid in room.GetMemberUnitUids())
                    _server.SendToUnit(uid, EventId.EGS_SYNC_DUNGEON_TIMER_NOT, syncNot);
                break;
            }

            // ── BattleField Gameplay (GSUserBattleField.cpp) ───────────────
            case EventId.EGS_SYNC_BATTLE_FIELD_DAMAGED_MAP_REQ:
            {
                if (ActiveUnit == null || Fsm.StateId != KGSFSM.S_ROOM) break;
                var req = ev.GetPayload<KEGS_SYNC_BATTLE_FIELD_DAMAGED_MAP_REQ>(Region);
                SendPacket(EventId.EGS_SYNC_BATTLE_FIELD_DAMAGED_MAP_ACK, new KEGS_SYNC_BATTLE_FIELD_DAMAGED_MAP_ACK
                {
                    Ok = NetError.NET_OK,
                });
                var room = CurrentRoomUid > 0 ? _server.Center.RoomManager.GetRoom(CurrentRoomUid) : null;
                if (room != null && room.Info.RoomState == (sbyte)RoomState.S_PLAY)
                {
                    var hostSlot = room.SnapshotSlots().FirstOrDefault(s => s.SlotState == 2 && s.IsHost);
                    if (hostSlot != null)
                    {
                        _server.SendToUnit(hostSlot.RoomUserInfo.UnitUid, EventId.EGS_BATTLE_FIELD_NPC_DAMAGED_MAP_REQ,
                            new KEGS_BATTLE_FIELD_NPC_DAMAGED_MAP_REQ
                            {
                                UnitUid = ActiveUnit.UnitUid,
                                NpcUid = req.NpcUid,
                            });
                    }
                }
                break;
            }

            case EventId.EGS_BATTLE_FIELD_NPC_DAMAGED_MAP_ACK:
            {
                if (ActiveUnit == null || CurrentRoomUid <= 0) break;
                var ack = ev.GetPayload<KEGS_BATTLE_FIELD_NPC_DAMAGED_MAP_ACK>(Region);
                var room = _server.Center.RoomManager.GetRoom(CurrentRoomUid);
                if (room == null || room.Info.RoomState != (sbyte)RoomState.S_PLAY) break;
                var syncNot = new KEGS_SYNC_BATTLE_FIELD_DAMAGED_MAP_NOT
                {
                    UnitUid = ack.UnitUid,
                    NpcUid = ack.NpcUid,
                };
                foreach (var kv in ack.DamagedMap)
                    syncNot.DamagedMap[kv.Key] = kv.Value;
                _server.SendToUnit(ack.UnitUid, EventId.EGS_SYNC_BATTLE_FIELD_DAMAGED_MAP_NOT, syncNot);
                break;
            }

            case EventId.EGS_FIELD_WORKINGS_BLOCK_LOG_ACK:
            {
                if (ActiveUnit == null) break;
                var ack = ev.GetPayload<KEGS_FIELD_WORKINGS_BLOCK_LOG_ACK>(Region);
                _server.Db.RecordFieldWorkingsBlockLog(
                    ActiveUnit.UnitUid, ack.X2State, ack.PhysicalMemory, ack.VirtualMemory,
                    ack.ProcessTerm, ack.FieldUserNum, ack.FieldNpcNum);
                break;
            }

            case EventId.EGS_BOSS_FIELD_INTRUDE_RESTRICTION_REQ:
            {
                if (ActiveUnit == null || Fsm.StateId != KGSFSM.S_ROOM) break;
                var room = CurrentRoomUid > 0 ? _server.Center.RoomManager.GetRoom(CurrentRoomUid) : null;
                if (room == null || room.Info.RoomType is not (3 or 7)) break;
                int ok = room.RestrictBossFieldIntrude(ActiveUnit.UnitUid);
                SendPacket(EventId.EGS_BOSS_FIELD_INTRUDE_RESTRICTION_ACK, new KEGS_BOSS_FIELD_INTRUDE_RESTRICTION_ACK
                {
                    Ok = ok,
                });
                break;
            }

            case EventId.EGS_BOSS_FIELD_LOG_NOT:
            {
                if (ActiveUnit == null || CurrentRoomUid <= 0 || Fsm.StateId != KGSFSM.S_ROOM) break;
                var not = ev.GetPayload<KEGS_BOSS_FIELD_LOG_NOT>(Region);
                var room = _server.Center.RoomManager.GetRoom(CurrentRoomUid);
                room?.RecordBossFieldLog(not.GivenDamage, not.AttackDamage);
                break;
            }

            // ── Party (GSUserNewParty.cpp, GSUserParty.cpp) ────────────────
            case EventId.EGS_PARTY_CHANGE_CHECK_MIN_LEVEL_REQ:
            {
                if (ActiveUnit == null || (Fsm.StateId != KGSFSM.S_FIELD_MAP && Fsm.StateId != KGSFSM.S_ROOM)) break;
                var req = ev.GetPayload<KEGS_PARTY_CHANGE_CHECK_MIN_LEVEL_REQ>(Region);
                if (CurrentPartyUid <= 0)
                {
                    SendPacket(EventId.EGS_PARTY_CHANGE_CHECK_MIN_LEVEL_ACK, new KEGS_PARTY_CHANGE_CHECK_MIN_LEVEL_ACK { Ok = NetError.ERR_PARTY_03 });
                    break;
                }
                var party = _server.Center.PartyManager.GetParty(CurrentPartyUid);
                if (party == null)
                {
                    SendPacket(EventId.EGS_PARTY_CHANGE_CHECK_MIN_LEVEL_ACK, new KEGS_PARTY_CHANGE_CHECK_MIN_LEVEL_ACK { Ok = NetError.ERR_PARTY_03 });
                    break;
                }
                if (!party.SetLimitLevel(ActiveUnit.UnitUid, req.CheckMinLevel))
                {
                    SendPacket(EventId.EGS_PARTY_CHANGE_CHECK_MIN_LEVEL_ACK, new KEGS_PARTY_CHANGE_CHECK_MIN_LEVEL_ACK { Ok = NetError.ERR_CHANGE_PARTY_MIN_LEVEL_CHECK_FAILE });
                    break;
                }
                SendPacket(EventId.EGS_PARTY_CHANGE_CHECK_MIN_LEVEL_ACK, new KEGS_PARTY_CHANGE_CHECK_MIN_LEVEL_ACK { Ok = NetError.NET_OK });
                var not = new KEGS_PARTY_CHANGE_CHECK_MIN_LEVEL_NOT { CheckMinLevel = req.CheckMinLevel };
                foreach (long uid in party.GetUnitUids())
                    _server.SendToUnit(uid, EventId.EGS_PARTY_CHANGE_CHECK_MIN_LEVEL_NOT, not);
                break;
            }

            case EventId.EGS_PARTY_LIST_REQ:
            {
                if (ActiveUnit == null || Fsm.StateId != KGSFSM.S_FIELD_MAP) break;
                var req = ev.GetPayload<KEGS_PARTY_LIST_REQ>(Region);
                if (req.ListType is not (KEGS_PARTY_LIST_REQ.PltDungeon or KEGS_PARTY_LIST_REQ.PltGateTotal or KEGS_PARTY_LIST_REQ.PltSecretDungeon))
                {
                    SendPacket(EventId.EGS_PARTY_LIST_ACK, new KEGS_PARTY_LIST_ACK { Ok = NetError.ERR_ROOM_LIST_00 });
                    break;
                }
                const int pageSize = 12; // KPartyListManager::PLE_PARTY_PAGE
                var parties = _server.Center.PartyManager.GetPartiesSnapshot()
                    .Where(p => p.Info.Public)
                    .Where(p => req.ListType switch
                    {
                        KEGS_PARTY_LIST_REQ.PltDungeon => req.Id <= 0 || p.Info.DungeonId == req.Id,
                        KEGS_PARTY_LIST_REQ.PltSecretDungeon => p.Info.DungeonMode == 2 || p.Info.DungeonId is >= 34000 and < 35000,
                        _ => true,
                    })
                    .Select(p =>
                    {
                        var info = new KPartyListInfo { PartyInfo = p.Info };
                        foreach (var m in p.SnapshotMembers())
                        {
                            info.PartyListUserInfo.Add(new KPartyListUserInfo
                            {
                                ChannelId = m.ChannelId,
                                UnitUid = m.UnitUid,
                                NickName = m.NickName,
                                UnitClass = m.UnitClass,
                                Level = m.Level,
                                State = 1,
                                StateCode = (int)m.FieldUid,
                                Host = m.Host,
                            });
                        }
                        return info;
                    })
                    .ToList();
                uint totalPages = (uint)Math.Max(1, (parties.Count + pageSize - 1) / pageSize);
                uint page = Math.Clamp(req.ViewPage == 0 ? 1u : req.ViewPage, 1u, totalPages);
                SendPacket(EventId.EGS_PARTY_LIST_ACK, new KEGS_PARTY_LIST_ACK
                {
                    Ok = NetError.NET_OK,
                    TotalPage = totalPages,
                    ViewPage = page,
                    PartyInfo = parties.Skip((int)((page - 1) * pageSize)).Take(pageSize).ToList(),
                });
                break;
            }

            // ── Field / Monitoring / GM (GSUserField.cpp) ──────────────────
            case EventId.EGS_UPDATE_FRIEND_INFO_REQ:
            {
                var req = ev.GetPayload<KEGS_UPDATE_FRIEND_INFO_REQ>(Region);
                if (ActiveUnit == null || ActiveUnit.UnitUid <= 0)
                {
                    SendPacket(EventId.EGS_UPDATE_FRIEND_INFO_ACK, new KEGS_UPDATE_FRIEND_INFO_ACK
                    {
                        Ok = NetError.ERR_UPDATE_FRIEND_00,
                        FriendNickName = req.FriendNickName,
                    });
                    break;
                }
                if (req.Delete)
                {
                    var match = _friends.Values.FirstOrDefault(f => string.Equals(f.NickName, req.FriendNickName, StringComparison.OrdinalIgnoreCase));
                    if (match != null)
                    {
                        _friends.Remove(match.UnitUid);
                        _friendUids.Remove(match.UnitUid);
                    }
                    SendPacket(EventId.EGS_UPDATE_FRIEND_INFO_ACK, new KEGS_UPDATE_FRIEND_INFO_ACK
                    {
                        Ok = NetError.ERR_UPDATE_FRIEND_00,
                        FriendNickName = req.FriendNickName,
                    });
                    break;
                }
                var existing = _friends.Values.FirstOrDefault(f => string.Equals(f.NickName, req.FriendNickName, StringComparison.OrdinalIgnoreCase));
                if (existing != null)
                {
                    SendPacket(EventId.EGS_UPDATE_FRIEND_INFO_ACK, new KEGS_UPDATE_FRIEND_INFO_ACK
                    {
                        Ok = NetError.NET_OK,
                        FriendNickName = req.FriendNickName,
                        FriendUnitUid = existing.UnitUid,
                        ChannelId = 1,
                    });
                    break;
                }
                var target = _server.GetUserByNickName(req.FriendNickName);
                long foundUid = target?.ActiveUnit?.UnitUid ?? (_server.Db.GetUnitByNickName(Region, req.FriendNickName)?.UnitUid ?? 0);
                SendPacket(EventId.EGS_UPDATE_FRIEND_INFO_ACK, new KEGS_UPDATE_FRIEND_INFO_ACK
                {
                    Ok = foundUid > 0 ? NetError.NET_OK : NetError.ERR_UPDATE_FRIEND_00,
                    FriendNickName = req.FriendNickName,
                    FriendUnitUid = foundUid,
                    ChannelId = foundUid > 0 ? 1 : 0,
                });
                break;
            }

            case EventId.EGS_ED_MONITORING_BLOCK_REQ:
            {
                if (ActiveUnit == null || (Fsm.StateId != KGSFSM.S_FIELD_MAP && Fsm.StateId != KGSFSM.S_ROOM)) break;
                if (AuthLevel < 3) break; // UAL_DEVELOPER
                var req = ev.GetPayload<KEGS_ED_MONITORING_BLOCK_REQ>(Region);
                var target = _server.GetUserByUnitUid(req.UnitUid);
                if (target == null)
                {
                    SendPacket(EventId.EGS_ED_MONITORING_BLOCK_ACK, new KEGS_ED_MONITORING_BLOCK_ACK { Ok = NetError.ERR_ROOM_23 });
                    break;
                }
                switch (req.Type)
                {
                    case KEGS_ED_MONITORING_BLOCK_REQ.EmbtSendMail:
                        target.HackingUserSendType = KEGS_SERVER_CHECK_HACK_USER_REQ.HctEdMonitoringSendMail;
                        target.HackingManager.SetHackingUserSendType(target.HackingUserSendType);
                        target.SendPacket(EventId.EGS_SERVER_CHECK_HACK_USER_REQ, new KEGS_SERVER_CHECK_HACK_USER_REQ
                        {
                            HackingCheckType = KEGS_SERVER_CHECK_HACK_USER_REQ.HctEdMonitoringSendMail,
                        });
                        break;
                    case KEGS_ED_MONITORING_BLOCK_REQ.EmbtAccountBlock:
                        target.HackingUserSendType = KEGS_SERVER_CHECK_HACK_USER_REQ.HctEdMonitoringAccountBlock;
                        target.HackingManager.SetHackingUserSendType(target.HackingUserSendType);
                        target.PunishmentManager.SetPunishment(new KPunishmentManager.PunishmentEntry
                        {
                            Type = (int)KPunishmentManager.PunishmentType.PtTrade,
                            StartUtc = DateTime.UtcNow,
                            EndUtc = DateTime.UtcNow.AddDays(3650),
                            Reason = "ED_MONITORING_ACCOUNT_BLOCK",
                        });
                        target.SendPacket(EventId.EGS_SERVER_CHECK_HACK_USER_REQ, new KEGS_SERVER_CHECK_HACK_USER_REQ
                        {
                            HackingCheckType = KEGS_SERVER_CHECK_HACK_USER_REQ.HctEdMonitoringAccountBlock,
                        });
                        break;
                    case KEGS_ED_MONITORING_BLOCK_REQ.EmbtMachineIdBlock:
                        target.HackingUserSendType = KEGS_SERVER_CHECK_HACK_USER_REQ.HctEdMonitoringMachineIdBlock;
                        target.HackingManager.SetHackingUserSendType(target.HackingUserSendType);
                        target.SendPacket(EventId.EGS_SERVER_CHECK_HACK_USER_REQ, new KEGS_SERVER_CHECK_HACK_USER_REQ
                        {
                            HackingCheckType = KEGS_SERVER_CHECK_HACK_USER_REQ.HctEdMonitoringMachineIdBlock,
                        });
                        break;
                    default:
                        SendPacket(EventId.EGS_ED_MONITORING_BLOCK_ACK, new KEGS_ED_MONITORING_BLOCK_ACK { Ok = NetError.ERR_ADMIN_COMMAND_03 });
                        goto doneEdMon;
                }
                SendPacket(EventId.EGS_ED_MONITORING_BLOCK_ACK, new KEGS_ED_MONITORING_BLOCK_ACK { Ok = NetError.NET_OK });
            doneEdMon:
                break;
            }

            case EventId.EGS_COMMUNITY_USER_LIST_MONITORING_MODE_REQ:
            {
                if (ActiveUnit == null || (Fsm.StateId != KGSFSM.S_FIELD_MAP && Fsm.StateId != KGSFSM.S_ROOM)) break;
                var req = ev.GetPayload<KEGS_COMMUNITY_USER_LIST_MONITORING_MODE_REQ>(Region);
                if (AuthLevel < 2 || !IsEdMonitoringMode)
                {
                    SendPacket(EventId.EGS_COMMUNITY_USER_LIST_ACK, new KEGS_COMMUNITY_USER_LIST_ACK { Ok = NetError.ERR_FIELD_04 });
                    break;
                }
                var list = new List<KCommunityUserInfo>();
                foreach (long uid in req.UnitUidList)
                {
                    var u = _server.GetUserByUnitUid(uid);
                    if (u?.ActiveUnit == null) continue;
                    list.Add(new KCommunityUserInfo
                    {
                        UnitUid = u.ActiveUnit.UnitUid,
                        NickName = u.ActiveUnit.NickName,
                        UnitClass = u.ActiveUnit.UnitClass,
                        Level = u.ActiveUnit.Level,
                        State = u.CurrentRoomUid > 0 ? (sbyte)4 : (sbyte)1,
                        StateCode = u.CurrentPartyUid,
                        RoomUid = u.CurrentRoomUid,
                        Ed = u.ActiveUnit.Ed,
                    });
                }
                SendPacket(EventId.EGS_COMMUNITY_USER_LIST_ACK, new KEGS_COMMUNITY_USER_LIST_ACK
                {
                    Ok = NetError.NET_OK,
                    TotalPage = 1,
                    ViewPage = req.ViewPage == 0 ? 1u : req.ViewPage,
                    UserList = list,
                });
                break;
            }

            case EventId.EGS_TOGGLE_INVISIBLE_REQ:
            {
                if (ActiveUnit == null || Fsm.StateId != KGSFSM.S_FIELD_MAP || AuthLevel < 2) break;
                IsInvisible = !IsInvisible;
                var not = new KEGS_TOGGLE_INVISIBLE_NOT
                {
                    UnitUid = ActiveUnit.UnitUid,
                    FieldUserInfo = BuildFieldUserInfo(),
                };
                var zone = CurrentFieldMapId > 0 ? _server.FieldManager.GetZone(CurrentFieldMapId) : null;
                if (zone != null)
                {
                    foreach (long uid in zone.GetAllUnitUids())
                        _server.SendToUnit(uid, EventId.EGS_TOGGLE_INVISIBLE_NOT, not);
                }
                else
                {
                    SendPacket(EventId.EGS_TOGGLE_INVISIBLE_NOT, not);
                }
                break;
            }

            // ── PvP Room & Channel (GSUserPvp.cpp) ─────────────────────────
            case EventId.EGS_OPEN_PVP_ROOM_LIST_REQ:
            {
                if (ActiveUnit == null || Fsm.StateId != KGSFSM.S_FIELD_MAP) break;
                var channels = _server.Center.RoomManager.PvpChannels.ToList();
                foreach (var tpl in _server.RoomListTemplates.Values)
                {
                    if (tpl.RoomListId <= 0 || channels.Any(c => c.ChannelId == (uint)tpl.RoomListId))
                        continue;
                    channels.Add(new KPVPChannelInfo
                    {
                        ChannelId = (uint)tpl.RoomListId,
                        PvpChannelClass = (sbyte)tpl.PvpClass,
                        MinRating = tpl.MinVp,
                        MaxRating = tpl.MaxVp,
                    });
                }
                var ack = new KEGS_OPEN_PVP_ROOM_LIST_ACK
                {
                    Ok = NetError.NET_OK,
                    ConnectChannelId = PvpChannelId,
                    Channel = channels,
                };
                SendPacket(EventId.EGS_OPEN_PVP_ROOM_LIST_ACK, ack);
                LeaveCurrentField();
                if (CurrentPartyUid > 0)
                {
                    _server.Center.PartyManager.LeaveParty(CurrentPartyUid, ActiveUnit.UnitUid);
                    CurrentPartyUid = 0;
                }
                _server.Global.MatchMakingManager.Cancel(ActiveUnit.UnitUid);
                _server.Global.AutoPartyManager.Cancel(ActiveUnit.UnitUid);
                break;
            }

            case EventId.EGS_CHANGE_PVP_ROOM_LIST_REQ:
            {
                if (ActiveUnit == null || Fsm.StateId != KGSFSM.S_FIELD_MAP) break;
                var req = ev.GetPayload<KEGS_CHANGE_PVP_ROOM_LIST_REQ>(Region);
                bool exists = _server.Center.RoomManager.PvpChannels.Any(c => c.ChannelId == req.RoomListId) ||
                              _server.RoomListTemplates.ContainsKey((int)req.RoomListId);
                if (exists)
                {
                    PvpChannelId = req.RoomListId;
                    RoomListId = req.RoomListId;
                }
                SendPacket(EventId.EGS_CHANGE_PVP_ROOM_LIST_ACK, new KEGS_CHANGE_PVP_ROOM_LIST_ACK
                {
                    Ok = exists ? NetError.NET_OK : NetError.ERR_ROOM_LIST_00,
                });
                break;
            }

            case EventId.EGS_INVITE_PVP_ROOM_REQ:
            {
                if (ActiveUnit == null || Fsm.StateId != KGSFSM.S_ROOM) break;
                var req = ev.GetPayload<KEGS_INVITE_PVP_ROOM_REQ>(Region);
                var room = CurrentRoomUid > 0 ? _server.Center.RoomManager.GetRoom(CurrentRoomUid) : null;
                if (room == null)
                {
                    SendPacket(EventId.EGS_INVITE_PVP_ROOM_ACK, new KEGS_INVITE_PVP_ROOM_ACK { Ok = NetError.ERR_ROOM_16 });
                    break;
                }
                if (room.Info.RoomType != 0)
                {
                    SendPacket(EventId.EGS_INVITE_PVP_ROOM_ACK, new KEGS_INVITE_PVP_ROOM_ACK { Ok = NetError.ERR_ROOM_43 });
                    break;
                }
                var target = _server.GetUserByUnitUid(req.UnitUid);
                if (target?.ActiveUnit == null)
                {
                    SendPacket(EventId.EGS_INVITE_PVP_ROOM_ACK, new KEGS_INVITE_PVP_ROOM_ACK { Ok = NetError.ERR_ROOM_23 });
                    break;
                }
                target.SendPacket(EventId.EGS_INVITE_PVP_ROOM_NOT, new KEGS_INVITE_PVP_ROOM_NOT
                {
                    NickName = ActiveUnit.NickName,
                    RoomInfo = room.Info,
                });
                SendPacket(EventId.EGS_INVITE_PVP_ROOM_ACK, new KEGS_INVITE_PVP_ROOM_ACK
                {
                    Ok = NetError.NET_OK,
                    NickName = target.ActiveUnit.NickName,
                });
                break;
            }

            case EventId.EGS_STATE_CHANGE_GAME_INTRUDE_REQ:
            {
                if (ActiveUnit == null || Fsm.StateId != KGSFSM.S_ROOM) break;
                var room = CurrentRoomUid > 0 ? _server.Center.RoomManager.GetRoom(CurrentRoomUid) : null;
                if (room == null)
                {
                    SendPacket(EventId.EGS_STATE_CHANGE_GAME_INTRUDE_ACK, new KEGS_STATE_CHANGE_GAME_INTRUDE_ACK { Ok = NetError.ERR_ROOM_16 });
                    break;
                }
                var (ok, slot) = room.CheckGameIntrude(ActiveUnit.UnitUid, room.IsObserver(ActiveUnit.UnitUid));
                SendPacket(EventId.EGS_STATE_CHANGE_GAME_INTRUDE_ACK, new KEGS_STATE_CHANGE_GAME_INTRUDE_ACK { Ok = ok });
                if (ok == NetError.NET_OK && slot != null)
                {
                    var not = new KEGS_STATE_CHANGE_GAME_INTRUDE_NOT { RoomState = room.Info.RoomState, IntruderSlot = slot };
                    foreach (long uid in room.GetMemberUnitUids())
                        _server.SendToUnit(uid, EventId.EGS_STATE_CHANGE_GAME_INTRUDE_NOT, not);
                }
                break;
            }

            case EventId.EGS_INTRUDE_START_REQ:
            {
                if (ActiveUnit == null || Fsm.StateId != KGSFSM.S_ROOM) break;
                var req = ev.GetPayload<KEGS_INTRUDE_START_REQ>(Region);
                var room = CurrentRoomUid > 0 ? _server.Center.RoomManager.GetRoom(CurrentRoomUid) : null;
                if (room == null)
                {
                    SendPacket(EventId.EGS_INTRUDE_START_ACK, new KEGS_INTRUDE_START_ACK { Ok = NetError.ERR_ROOM_16 });
                    break;
                }
                var (ok, slot, isObs) = room.IntrudeStart(ActiveUnit.UnitUid);
                SendPacket(EventId.EGS_INTRUDE_START_ACK, new KEGS_INTRUDE_START_ACK { Ok = ok });
                if (ok == NetError.NET_OK && slot != null)
                {
                    var not = new KEGS_INTRUDE_START_NOT
                    {
                        RoomState = room.Info.RoomState,
                        StartPosIndex = req.StartPosIndex,
                        IntruderSlot = slot,
                        Observer = isObs,
                    };
                    foreach (long uid in room.GetMemberUnitUids())
                        _server.SendToUnit(uid, EventId.EGS_INTRUDE_START_NOT, not);
                }
                break;
            }

            case EventId.EGS_CHANGE_SLOT_OPEN_REQ:
            {
                if (ActiveUnit == null || Fsm.StateId != KGSFSM.S_ROOM) break;
                var req = ev.GetPayload<KEGS_CHANGE_SLOT_OPEN_REQ>(Region);
                var room = CurrentRoomUid > 0 ? _server.Center.RoomManager.GetRoom(CurrentRoomUid) : null;
                if (room == null)
                {
                    SendPacket(EventId.EGS_CHANGE_SLOT_OPEN_ACK, new KEGS_CHANGE_SLOT_OPEN_ACK { Ok = NetError.ERR_ROOM_16 });
                    break;
                }
                var (ok, tbIdx) = room.ChangeSlotOpen(ActiveUnit.UnitUid, req.SlotIndex, req.SlotState);
                SendPacket(EventId.EGS_CHANGE_SLOT_OPEN_ACK, new KEGS_CHANGE_SLOT_OPEN_ACK { Ok = ok });
                if (ok == NetError.NET_OK)
                {
                    var not = new KEGS_CHANGE_SLOT_OPEN_NOT
                    {
                        SlotIndex = req.SlotIndex,
                        TeamBalanceSlotIndex = tbIdx,
                        SlotState = req.SlotState,
                    };
                    foreach (long uid in room.GetMemberUnitUids())
                        _server.SendToUnit(uid, EventId.EGS_CHANGE_SLOT_OPEN_NOT, not);
                }
                break;
            }

            case EventId.EGS_CHANGE_WINNING_NUM_KILL_REQ:
            {
                if (ActiveUnit == null || Fsm.StateId != KGSFSM.S_ROOM) break;
                var req = ev.GetPayload<KEGS_CHANGE_WINNING_NUM_KILL_REQ>(Region);
                var room = CurrentRoomUid > 0 ? _server.Center.RoomManager.GetRoom(CurrentRoomUid) : null;
                int ok = room != null ? room.ChangeWinningNumKill(ActiveUnit.UnitUid, req.WinKillNum) : NetError.ERR_ROOM_16;
                SendPacket(EventId.EGS_CHANGE_WINNING_NUM_KILL_ACK, new KEGS_CHANGE_WINNING_NUM_KILL_ACK { Ok = ok });
                if (ok == NetError.NET_OK && room != null)
                {
                    var not = new KEGS_CHANGE_WINNING_NUM_KILL_NOT { WinKillNum = req.WinKillNum };
                    foreach (long uid in room.GetMemberUnitUids())
                        _server.SendToUnit(uid, EventId.EGS_CHANGE_WINNING_NUM_KILL_NOT, not);
                }
                break;
            }

            case EventId.EGS_LEAVE_GAME_REQ:
            {
                if (ActiveUnit == null || Fsm.StateId != KGSFSM.S_ROOM) break;
                var room = CurrentRoomUid > 0 ? _server.Center.RoomManager.GetRoom(CurrentRoomUid) : null;
                if (room == null)
                {
                    SendPacket(EventId.EGS_LEAVE_GAME_ACK, new KEGS_LEAVE_GAME_ACK { Ok = NetError.ERR_ROOM_19 });
                    break;
                }
                var (ok, _) = room.LeaveGame(ActiveUnit.UnitUid);
                SendPacket(EventId.EGS_LEAVE_GAME_ACK, new KEGS_LEAVE_GAME_ACK { Ok = ok });
                if (ok == NetError.NET_OK)
                {
                    var not = new KEGS_LEAVE_GAME_NOT
                    {
                        RoomState = room.Info.RoomState,
                        UnitUid = ActiveUnit.UnitUid,
                        Slot = room.SnapshotSlots(),
                        ObserverSlot = [.. room.ObserverSlots],
                    };
                    foreach (long uid in room.GetMemberUnitUids())
                        _server.SendToUnit(uid, EventId.EGS_LEAVE_GAME_NOT, not);
                }
                break;
            }

            case EventId.EGS_CREATE_PVP_ITEM_REQ:
            {
                if (ActiveUnit == null || Fsm.StateId != KGSFSM.S_ROOM) break;
                var req = ev.GetPayload<KEGS_CREATE_PVP_ITEM_REQ>(Region);
                var room = CurrentRoomUid > 0 ? _server.Center.RoomManager.GetRoom(CurrentRoomUid) : null;
                if (room == null)
                {
                    SendPacket(EventId.EGS_CREATE_PVP_ITEM_ACK, new KEGS_CREATE_PVP_ITEM_ACK { Ok = NetError.ERR_ROOM_16 });
                    break;
                }
                int ok = room.CreatePvpItem(req.PositionIndex, 130001, out var dropData);
                SendPacket(EventId.EGS_CREATE_PVP_ITEM_ACK, new KEGS_CREATE_PVP_ITEM_ACK { Ok = ok });
                if (ok == NetError.NET_OK && dropData != null)
                {
                    var not = new KEGS_CREATE_PVP_ITEM_NOT { PositionIndex = req.PositionIndex, DropItemData = dropData };
                    foreach (long uid in room.GetMemberUnitUids())
                        _server.SendToUnit(uid, EventId.EGS_CREATE_PVP_ITEM_NOT, not);
                }
                break;
            }

            case EventId.EGS_CHANGE_PVP_ITEMMODE_REQ:
            {
                if (ActiveUnit == null || Fsm.StateId != KGSFSM.S_ROOM) break;
                var room = CurrentRoomUid > 0 ? _server.Center.RoomManager.GetRoom(CurrentRoomUid) : null;
                if (room == null)
                {
                    SendPacket(EventId.EGS_CHANGE_PVP_ITEMMODE_ACK, new KEGS_CHANGE_PVP_ITEMMODE_ACK { Ok = NetError.ERR_ROOM_16 });
                    break;
                }
                int ok = room.TogglePvpItemMode(out bool newItemMode);
                SendPacket(EventId.EGS_CHANGE_PVP_ITEMMODE_ACK, new KEGS_CHANGE_PVP_ITEMMODE_ACK { Ok = ok });
                if (ok == NetError.NET_OK)
                {
                    var not = new KEGS_CHANGE_PVP_ITEMMODE_NOT { IsItemMode = newItemMode };
                    foreach (long uid in room.GetMemberUnitUids())
                        _server.SendToUnit(uid, EventId.EGS_CHANGE_PVP_ITEMMODE_NOT, not);
                }
                break;
            }

            case EventId.EGS_CHANGE_PVP_GAME_OPTION_REQ:
            {
                if (ActiveUnit == null || Fsm.StateId != KGSFSM.S_ROOM) break;
                var req = ev.GetPayload<KEGS_CHANGE_PVP_GAME_OPTION_REQ>(Region);
                var room = CurrentRoomUid > 0 ? _server.Center.RoomManager.GetRoom(CurrentRoomUid) : null;
                int ok = room != null ? room.ChangePvpGameOption(ActiveUnit.UnitUid, req.BaseDefence, req.BossCombat) : NetError.ERR_ROOM_16;
                SendPacket(EventId.EGS_CHANGE_PVP_GAME_OPTION_ACK, new KEGS_CHANGE_PVP_GAME_OPTION_ACK { Ok = ok });
                if (ok == NetError.NET_OK && room != null)
                {
                    var not = new KEGS_CHANGE_PVP_GAME_OPTION_NOT { BaseDefence = req.BaseDefence, BossCombat = req.BossCombat };
                    foreach (long uid in room.GetMemberUnitUids())
                        _server.SendToUnit(uid, EventId.EGS_CHANGE_PVP_GAME_OPTION_NOT, not);
                }
                break;
            }

            case EventId.EGS_CHANGE_PVP_ITEM_POS_REQ:
            {
                if (ActiveUnit == null || Fsm.StateId != KGSFSM.S_ROOM) break;
                var req = ev.GetPayload<KEGS_CHANGE_PVP_ITEM_POS_REQ>(Region);
                var room = CurrentRoomUid > 0 ? _server.Center.RoomManager.GetRoom(CurrentRoomUid) : null;
                int ok = room != null ? room.ChangePvpItemPos(req.ItemUid, req.Pos) : NetError.ERR_ROOM_16;
                SendPacket(EventId.EGS_CHANGE_PVP_ITEM_POS_ACK, new KEGS_CHANGE_PVP_ITEM_POS_ACK { Ok = ok });
                if (ok == NetError.NET_OK && room != null)
                {
                    var not = new KEGS_CHANGE_PVP_ITEM_POS_NOT { ItemUid = req.ItemUid, Pos = req.Pos };
                    foreach (long uid in room.GetMemberUnitUids())
                        _server.SendToUnit(uid, EventId.EGS_CHANGE_PVP_ITEM_POS_NOT, not);
                }
                break;
            }

            case EventId.EGS_PVP_REMATCH_REQ:
            {
                if (ActiveUnit == null || Fsm.StateId != KGSFSM.S_ROOM) break;
                var req = ev.GetPayload<KEGS_PVP_REMATCH_REQ>(Region);
                var room = CurrentRoomUid > 0 ? _server.Center.RoomManager.GetRoom(CurrentRoomUid) : null;
                if (room == null)
                {
                    SendPacket(EventId.EGS_PVP_REMATCH_ACK, new KEGS_PVP_REMATCH_ACK { Ok = NetError.ERR_ROOM_16 });
                    break;
                }
                var (ok, _) = room.VoteRematch(ActiveUnit.UnitUid, req.AcceptRematch);
                SendPacket(EventId.EGS_PVP_REMATCH_ACK, new KEGS_PVP_REMATCH_ACK { Ok = ok });
                if (ok == NetError.NET_OK)
                {
                    var not = new KEGS_PVP_REMATCH_NOT { UnitUid = ActiveUnit.UnitUid, AcceptRematch = req.AcceptRematch };
                    foreach (long uid in room.GetMemberUnitUids())
                        _server.SendToUnit(uid, EventId.EGS_PVP_REMATCH_NOT, not);
                }
                break;
            }

            // ── RoomCommon / Square / TrainingCenter (GSUserRoomCommon.cpp) ─
            case EventId.EGS_CREATE_TUTORIAL_ROOM_REQ:
            {
                if (ActiveUnit == null || (Fsm.StateId != KGSFSM.S_SERVER_SELECT && Fsm.StateId != KGSFSM.S_FIELD_MAP)) break;
                var req = ev.GetPayload<KEGS_CREATE_TUTORIAL_ROOM_REQ>(Region);
                if (IsGuestUser && ActiveUnit.Level > 19)
                {
                    SendPacket(EventId.EGS_CREATE_TUTORIAL_ROOM_ACK, new KEGS_CREATE_TUTORIAL_ROOM_ACK { Ok = NetError.ERR_GUEST_USER_02 });
                    break;
                }
                if (req.RoomInfo.RoomType is not (1 or 2))
                {
                    SendPacket(EventId.EGS_CREATE_TUTORIAL_ROOM_ACK, new KEGS_CREATE_TUTORIAL_ROOM_ACK { Ok = NetError.ERR_ROOM_00 });
                    break;
                }
                if (req.RoomInfo.DungeonId != 30000 && req.RoomInfo.DungeonId != 30010 && req.RoomInfo.DungeonId != 39000)
                {
                    SendPacket(EventId.EGS_CREATE_TUTORIAL_ROOM_ACK, new KEGS_CREATE_TUTORIAL_ROOM_ACK { Ok = NetError.ERR_ROOM_30 });
                    break;
                }
                LeaveCurrentField();
                if (Fsm.StateId == KGSFSM.S_SERVER_SELECT)
                    Fsm.TryStateTransition(KGSFSM.I_TO_FIELD_MAP, out _);
                req.RoomInfo.RoomListId = RoomListId;
                var room = _server.Center.RoomManager.CreateRoom(req.RoomInfo, BuildRoomUserInfo(), asObserver: false);
                CurrentRoomUid = room.Info.RoomUid;
                Fsm.TryStateTransition(KGSFSM.I_TO_ROOM, out _);
                SendPacket(EventId.EGS_CREATE_TUTORIAL_ROOM_ACK, new KEGS_CREATE_TUTORIAL_ROOM_ACK
                {
                    Ok = NetError.NET_OK,
                    RoomInfo = room.Info,
                    Slot = room.SnapshotSlots(),
                    CnIp = room.Info.UdpRelayIp,
                });
                break;
            }

            case EventId.EGS_QUICK_JOIN_REQ:
            {
                if (ActiveUnit == null || Fsm.StateId != KGSFSM.S_FIELD_MAP) break;
                var req = ev.GetPayload<KEGS_QUICK_JOIN_REQ>(Region);
                if (IsGuestUser && ActiveUnit.Level > 19)
                {
                    SendPacket(EventId.EGS_QUICK_JOIN_ACK, new KEGS_QUICK_JOIN_ACK { Ok = NetError.ERR_GUEST_USER_02 });
                    break;
                }
                long ruid = _server.Center.RoomManager.GetWaitingRoomUid(RoomListId, req.Option, req.IsItemMode);
                if (ruid <= 0)
                {
                    SendPacket(EventId.EGS_QUICK_JOIN_ACK, new KEGS_QUICK_JOIN_ACK { Ok = NetError.ERR_ROOM_07 });
                    break;
                }
                var (ok, room) = _server.Center.RoomManager.JoinRoom(ruid, BuildRoomUserInfo(), string.Empty, false);
                if (ok != NetError.NET_OK || room == null)
                {
                    SendPacket(EventId.EGS_QUICK_JOIN_ACK, new KEGS_QUICK_JOIN_ACK { Ok = ok });
                    break;
                }
                LeaveCurrentField();
                CurrentRoomUid = room.Info.RoomUid;
                Fsm.TryStateTransition(KGSFSM.I_TO_ROOM, out _);
                var slots = room.SnapshotSlots();
                SendPacket(EventId.EGS_QUICK_JOIN_ACK, new KEGS_QUICK_JOIN_ACK
                {
                    Ok = NetError.NET_OK,
                    RoomInfo = room.Info,
                    Slot = slots,
                });
                var joinNot = new KEGS_CREATE_ROOM_ACK
                {
                    Ok = NetError.NET_OK,
                    RoomInfo = room.Info,
                    Slots = slots,
                    ObserverSlots = room.ObserverSlots,
                };
                foreach (long uid in room.GetMemberUnitUids().Where(u => u != ActiveUnit.UnitUid))
                    _server.SendToUnit(uid, EventId.EGS_JOIN_ROOM_NOT, joinNot);
                break;
            }

            case EventId.EGS_STATE_CHANGE_RESULT_REQ:
            {
                if (ActiveUnit == null || Fsm.StateId != KGSFSM.S_ROOM) break;
                var req = ev.GetPayload<KEGS_STATE_CHANGE_RESULT_REQ>(Region);
                var room = CurrentRoomUid > 0 ? _server.Center.RoomManager.GetRoom(CurrentRoomUid) : null;
                int ok = room != null ? room.StateChangeResult(ActiveUnit.UnitUid) : NetError.ERR_ROOM_16;
                if (req.Dummy != 0)
                    KLog.Clog($"KGSUser[{Uid}] state change result param={req.Dummy}");
                SendPacket(EventId.EGS_STATE_CHANGE_RESULT_ACK, new KEGS_STATE_CHANGE_RESULT_ACK { Ok = ok });
                if (ok == NetError.NET_OK && room != null)
                {
                    var not = new KEGS_STATE_CHANGE_RESULT_NOT { RoomState = room.Info.RoomState, Slot = room.SnapshotSlots() };
                    foreach (long uid in room.GetMemberUnitUids())
                        _server.SendToUnit(uid, EventId.EGS_STATE_CHANGE_RESULT_NOT, not);
                }
                break;
            }

            case EventId.EGS_USER_UNIT_RE_BIRTH_POS_REQ:
            {
                if (ActiveUnit == null || Fsm.StateId != KGSFSM.S_ROOM) break;
                var req = ev.GetPayload<KEGS_USER_UNIT_RE_BIRTH_POS_REQ>(Region);
                var room = CurrentRoomUid > 0 ? _server.Center.RoomManager.GetRoom(CurrentRoomUid) : null;
                int ok = room != null ? room.SetUnitRebirthPos(ActiveUnit.UnitUid, req.StartPosIndex) : NetError.ERR_ROOM_16;
                SendPacket(EventId.EGS_USER_UNIT_RE_BIRTH_POS_ACK, new KPacketOK { Ok = ok });
                break;
            }

            case EventId.EGS_MY_USER_UNIT_INFO_TO_SERVER_REQ:
            {
                if (ActiveUnit == null || Fsm.StateId != KGSFSM.S_ROOM) break;
                var req = ev.GetPayload<KEGS_MY_USER_UNIT_INFO_TO_SERVER_REQ>(Region);
                var room = CurrentRoomUid > 0 ? _server.Center.RoomManager.GetRoom(CurrentRoomUid) : null;
                if (room == null)
                {
                    SendPacket(EventId.EGS_MY_USER_UNIT_INFO_TO_SERVER_ACK, new KPacketOK { Ok = NetError.ERR_ROOM_16 });
                    break;
                }
                var (ok, _) = room.SubmitDungeonPlayResult(ActiveUnit.UnitUid, req.MyPlayResult);
                SendPacket(EventId.EGS_MY_USER_UNIT_INFO_TO_SERVER_ACK, new KPacketOK { Ok = ok });
                break;
            }

            case EventId.EGS_REFRESH_ROOM_REQ:
            {
                if (ActiveUnit == null || Fsm.StateId != KGSFSM.S_ROOM) break;
                var room = CurrentRoomUid > 0 ? _server.Center.RoomManager.GetRoom(CurrentRoomUid) : null;
                if (room == null)
                {
                    SendPacket(EventId.EGS_REFRESH_ROOM_ACK, new KEGS_REFRESH_ROOM_ACK { Ok = NetError.ERR_ROOM_16 });
                    break;
                }
                SendPacket(EventId.EGS_REFRESH_ROOM_ACK, new KEGS_REFRESH_ROOM_ACK
                {
                    Ok = NetError.NET_OK,
                    RoomInfo = room.Info,
                    Slot = room.SnapshotSlots(),
                });
                break;
            }

            case EventId.EGS_RESULT_SUCCESS_REQ:
            {
                if (ActiveUnit == null || Fsm.StateId != KGSFSM.S_ROOM) break;
                var req = ev.GetPayload<KEGS_RESULT_SUCCESS_REQ>(Region);
                var room = CurrentRoomUid > 0 ? _server.Center.RoomManager.GetRoom(CurrentRoomUid) : null;
                if (room == null)
                {
                    SendPacket(EventId.EGS_RESULT_SUCCESS_ACK, new KEGS_RESULT_SUCCESS_ACK { Ok = NetError.ERR_ROOM_16 });
                    break;
                }
                var (ok, _) = room.ConfirmResultSuccess(ActiveUnit.UnitUid);
                if (ok == NetError.NET_OK && room.Info.PvpChannelClass == KPVPChannelInfo.PccOfficial)
                    _server.Db.RecordGamePlayNetworkInfo(ActiveUnit.UnitUid, req.GamePlayNetworkInfo.GamePlayNetworkInfo);
                SendPacket(EventId.EGS_RESULT_SUCCESS_ACK, new KEGS_RESULT_SUCCESS_ACK
                {
                    Ok = ok,
                    DungeonId = room.Info.DungeonId,
                });
                break;
            }

            case EventId.EGS_SQUARE_LIST_REQ:
            {
                if (ActiveUnit == null || Fsm.StateId != KGSFSM.S_FIELD_MAP) break;
                var req = ev.GetPayload<KEGS_SQUARE_LIST_REQ>(Region);
                if (_server.GameSysValTable.BlockPersonalShop)
                {
                    SendPacket(EventId.EGS_SQUARE_LIST_ACK, new KEGS_SQUARE_LIST_ACK
                    {
                        Ok = NetError.ERR_BLOCK_TRADE_06,
                        TotalPage = 1,
                        ViewPage = 1,
                    });
                    break;
                }
                if (AbuserManager.IsTradeBlockUser || PunishmentManager.IsAppliedPunishment((int)KPunishmentManager.PunishmentType.PtTrade))
                {
                    SendPacket(EventId.EGS_SQUARE_LIST_ACK, new KEGS_SQUARE_LIST_ACK
                    {
                        Ok = NetError.ERR_BLOCK_TRADE_07,
                    });
                    break;
                }
                if (AbuserManager.IsTradeBlockUnit)
                {
                    SendPacket(EventId.EGS_SQUARE_LIST_ACK, new KEGS_SQUARE_LIST_ACK
                    {
                        Ok = NetError.ERR_TRADE_BLOCK_UNIT_02,
                    });
                    break;
                }
                var (items, totalPage, viewPage) = _server.Center.RoomManager.GetSquareInfoPage(req.SquareType, req.ViewPage);
                SendPacket(EventId.EGS_SQUARE_LIST_ACK, new KEGS_SQUARE_LIST_ACK
                {
                    Ok = NetError.NET_OK,
                    TotalPage = totalPage,
                    ViewPage = viewPage,
                    SquareInfo = items,
                });
                break;
            }

            case EventId.EGS_JOIN_SQUARE_REQ:
            {
                if (ActiveUnit == null || Fsm.StateId != KGSFSM.S_FIELD_MAP) break;
                var req = ev.GetPayload<KEGS_JOIN_SQUARE_REQ>(Region);
                if (_server.GameSysValTable.BlockPersonalShop)
                {
                    SendPacket(EventId.EGS_JOIN_SQUARE_ACK, new KEGS_JOIN_SQUARE_ACK { Ok = NetError.ERR_BLOCK_TRADE_06 });
                    break;
                }
                if (AbuserManager.IsTradeBlockUser || PunishmentManager.IsAppliedPunishment((int)KPunishmentManager.PunishmentType.PtTrade))
                {
                    SendPacket(EventId.EGS_JOIN_SQUARE_ACK, new KEGS_JOIN_SQUARE_ACK { Ok = NetError.ERR_BLOCK_TRADE_07 });
                    break;
                }
                if (AbuserManager.IsTradeBlockUnit)
                {
                    SendPacket(EventId.EGS_JOIN_SQUARE_ACK, new KEGS_JOIN_SQUARE_ACK { Ok = NetError.ERR_TRADE_BLOCK_UNIT_02 });
                    break;
                }
                var mySquareUser = BuildSquareUserInfo();
                var (ok, squareInfo, users, room) = _server.Center.RoomManager.JoinSquare(req.SquareUid, mySquareUser);
                if (ok != NetError.NET_OK || squareInfo == null || room == null)
                {
                    SendPacket(EventId.EGS_JOIN_SQUARE_ACK, new KEGS_JOIN_SQUARE_ACK { Ok = ok });
                    break;
                }
                LeaveCurrentField();
                CurrentRoomUid = squareInfo.SquareUid;
                Fsm.TryStateTransition(KGSFSM.I_TO_ROOM, out _);
                SendPacket(EventId.EGS_JOIN_SQUARE_ACK, new KEGS_JOIN_SQUARE_ACK
                {
                    Ok = NetError.NET_OK,
                    SquareInfo = squareInfo,
                    UserInfo = users,
                });
                var joinNot = new KEGS_JOIN_SQUARE_NOT { UserInfo = mySquareUser };
                foreach (long uid in room.GetSquareUserUids().Where(u => u != ActiveUnit.UnitUid))
                    _server.SendToUnit(uid, EventId.EGS_JOIN_SQUARE_NOT, joinNot);
                break;
            }

            case EventId.EGS_LEAVE_SQUARE_REQ:
            {
                if (ActiveUnit == null || Fsm.StateId != KGSFSM.S_ROOM) break;
                var req = ev.GetPayload<KEGS_LEAVE_SQUARE_REQ>(Region);
                var (removed, room) = CurrentRoomUid > 0
                    ? _server.Center.RoomManager.LeaveSquare(CurrentRoomUid, ActiveUnit.UnitUid)
                    : (false, null);
                CurrentRoomUid = 0;
                Fsm.TryStateTransition(KGSFSM.I_TO_FIELD_MAP, out _);
                SendPacket(EventId.EGS_LEAVE_SQUARE_ACK, new KEGS_LEAVE_SQUARE_ACK
                {
                    Ok = removed ? NetError.NET_OK : NetError.ERR_SQUARE_01,
                });
                if (removed && room != null)
                {
                    var leaveNot = new KEGS_LEAVE_SQUARE_NOT { UnitUid = ActiveUnit.UnitUid, Reason = req.Reason };
                    foreach (long uid in room.GetSquareUserUids())
                        _server.SendToUnit(uid, EventId.EGS_LEAVE_SQUARE_NOT, leaveNot);
                }
                break;
            }

            case EventId.EGS_SQUARE_UNIT_SYNC_DATA_REQ:
            {
                if (ActiveUnit == null || CurrentRoomUid <= 0 || Fsm.StateId != KGSFSM.S_ROOM) break;
                var req = ev.GetPayload<KEGS_SQUARE_UNIT_SYNC_DATA_REQ>(Region);
                var room = _server.Center.RoomManager.GetRoom(CurrentRoomUid);
                if (room == null || room.Info.RoomType != 5) break;
                var not = new KEGS_SQUARE_UNIT_SYNC_DATA_NOT
                {
                    UnitUid = ActiveUnit.UnitUid,
                    StateId = req.StateId,
                    StateChangeNum = req.StateChangeNum,
                    Pos = req.Pos,
                    LastTouchLineIndex = req.LastTouchLineIndex,
                    IsRight = req.IsRight,
                    Ed = ActiveUnit.Ed,
                };
                foreach (long uid in room.GetSquareUserUids().Where(u => u != ActiveUnit.UnitUid))
                    _server.SendToUnit(uid, EventId.EGS_SQUARE_UNIT_SYNC_DATA_NOT, not);
                break;
            }

            case EventId.EGS_SET_TC_REMAINING_TIME_REQ:
            {
                if (ActiveUnit == null || Fsm.StateId != KGSFSM.S_ROOM) break;
                var req = ev.GetPayload<KEGS_SET_TC_REMAINING_TIME_REQ>(Region);
                var room = CurrentRoomUid > 0 ? _server.Center.RoomManager.GetRoom(CurrentRoomUid) : null;
                int ok = room != null ? room.SetTcRemainingTime(req.Enable) : NetError.ERR_ROOM_16;
                SendPacket(EventId.EGS_SET_TC_REMAINING_TIME_ACK, new KEGS_SET_TC_REMAINING_TIME_ACK { Ok = ok });
                break;
            }

            case EventId.EGS_ADD_ON_STAT_REQ:
            {
                if (ActiveUnit == null || Fsm.StateId != KGSFSM.S_ROOM) break;
                var req = ev.GetPayload<KEGS_ADD_ON_STAT_REQ>(Region);
                var room = CurrentRoomUid > 0 ? _server.Center.RoomManager.GetRoom(CurrentRoomUid) : null;
                if (room == null || room.Info.RoomType is not (0 or 1 or 2 or 3 or 4 or 7))
                {
                    SendPacket(EventId.EGS_ADD_ON_STAT_ACK, new KEGS_ADD_ON_STAT_ACK { Ok = NetError.ERR_ROOM_28 });
                    break;
                }
                SendPacket(EventId.EGS_ADD_ON_STAT_ACK, new KEGS_ADD_ON_STAT_ACK { Ok = NetError.NET_OK });
                var not = new KEGS_ADD_ON_STAT_NOT
                {
                    Uid = req.Uid,
                    IsUnit = req.IsUnit,
                    StatType = req.StatType,
                    StatVal = req.StatVal,
                    Time = req.Time,
                };
                foreach (long uid in room.GetMemberUnitUids())
                    _server.SendToUnit(uid, EventId.EGS_ADD_ON_STAT_NOT, not);
                break;
            }

            case EventId.EGS_BUFF_SKILL_INFO_CONVEY_REQ:
            {
                if (ActiveUnit == null || Fsm.StateId != KGSFSM.S_ROOM) break;
                var req = ev.GetPayload<KEGS_BUFF_SKILL_INFO_CONVEY_REQ>(Region);
                var room = CurrentRoomUid > 0 ? _server.Center.RoomManager.GetRoom(CurrentRoomUid) : null;
                if (room == null)
                {
                    SendPacket(EventId.EGS_BUFF_SKILL_INFO_CONVEY_ACK, new KEGS_BUFF_SKILL_INFO_CONVEY_ACK { Ok = NetError.ERR_ROOM_16 });
                    break;
                }
                var (ok, not) = room.ProcessBuffSkill(req);
                SendPacket(EventId.EGS_BUFF_SKILL_INFO_CONVEY_ACK, new KEGS_BUFF_SKILL_INFO_CONVEY_ACK { Ok = NetError.NET_OK });
                if (ok)
                {
                    foreach (long uid in room.GetMemberUnitUids())
                        _server.SendToUnit(uid, EventId.EGS_BUFF_SKILL_INFO_CONVEY_NOT, not);
                }
                break;
            }

            case EventId.EGS_UDP_PING_SCORE_NOT:
            {
                if (ActiveUnit == null || CurrentRoomUid <= 0 || Fsm.StateId != KGSFSM.S_ROOM) break;
                var not = ev.GetPayload<KEGS_UDP_PING_SCORE_NOT>(Region);
                var room = _server.Center.RoomManager.GetRoom(CurrentRoomUid);
                room?.UpdatePingScore(ActiveUnit.UnitUid, forceHost: false, not.PingScore);
                break;
            }

            // -------- Dungeon Resurrect Ack/Not (2) --------

            case EventId.EGS_RESURRECT_TO_CONTINUE_DUNGEON_ACK:
            {
                if (ActiveUnit == null || Fsm.StateId != KGSFSM.S_ROOM) break;
                var ack = ev.GetPayload<KEGS_RESURRECT_TO_CONTINUE_DUNGEON_ACK>(Region);
                if (ack.Ok == NetError.NET_OK)
                {
                    if (NumAutoPaymentResStone > 0)
                        NumAutoPaymentResStone--;
                    else if (NumResurrectionStone > 0)
                        NumResurrectionStone--;
                    ack.NumResurrectionStone = NumResurrectionStone;
                    ack.NumAutoPaymentResStone = NumAutoPaymentResStone;
                    ack.UseCash = CashInventory.IsCashResurrectionStone;
                }
                SendPacket(EventId.EGS_RESURRECT_TO_CONTINUE_DUNGEON_ACK, ack);
                break;
            }

            case EventId.EGS_RESURRECT_TO_CONTINUE_DUNGEON_NOT:
            {
                if (Fsm.StateId != KGSFSM.S_ROOM) break;
                var not = ev.GetPayload<KEGS_RESURRECT_TO_CONTINUE_DUNGEON_NOT>(Region);
                SendPacket(EventId.EGS_RESURRECT_TO_CONTINUE_DUNGEON_NOT, not);
                break;
            }

            // -------- Session / Server Set / Channel Change / Lag & Performance (15) --------

            case EventId.EGS_STATE_CHANGE_SERVER_SELECT_REQ:
            {
                if (Fsm.StateId is not (KGSFSM.S_LOGINED or KGSFSM.S_SERVER_SELECT or KGSFSM.S_FIELD_MAP or KGSFSM.S_ROOM)) break;
                if (Fsm.StateId == KGSFSM.S_ROOM)
                {
                    if (ActiveUnit != null && CurrentRoomUid > 0)
                    {
                        _server.Center.RoomManager.LeaveRoom(CurrentRoomUid, ActiveUnit.UnitUid);
                        CurrentRoomUid = 0;
                    }
                    Fsm.TryStateTransition(KGSFSM.I_TO_FIELD_MAP, out _);
                }

                if (_hackingUserType == 1)
                {
                    SendPacket(EventId.EGS_STATE_CHANGE_SERVER_SELECT_ACK,
                        new KEGS_STATE_CHANGE_SERVER_SELECT_ACK { Ok = NetError.ERR_HACK_USER_02 });
                    ReserveDestroy();
                    break;
                }

                if (ActiveUnit != null)
                {
                    LeaveCurrentField();
                    if (CurrentPartyUid > 0)
                    {
                        _server.Center.PartyManager.LeaveParty(CurrentPartyUid, ActiveUnit.UnitUid);
                        CurrentPartyUid = 0;
                    }
                    _server.Global.MatchMakingManager.Cancel(ActiveUnit.UnitUid);
                    _server.Global.AutoPartyManager.Cancel(ActiveUnit.UnitUid);
                    ActiveUnit = null;
                }

                if (Fsm.StateId != KGSFSM.S_SERVER_SELECT)
                    Fsm.TryStateTransition(KGSFSM.I_TO_SERVER_SELECT, out _);
                SendPacket(EventId.EGS_STATE_CHANGE_SERVER_SELECT_ACK,
                    new KEGS_STATE_CHANGE_SERVER_SELECT_ACK { Ok = NetError.NET_OK });
                break;
            }

            case EventId.EGS_GET_SERVER_SET_DATA_REQ:
            {
                if (Fsm.StateId is not (KGSFSM.S_LOGINED or KGSFSM.S_FIELD_MAP or KGSFSM.S_SERVER_SELECT)) break;
                var groups = _server.Db.GetServerGroupList(Region.Code);
                var servers = _server.Db.GetServerList(Region.Code);
                var ack = new KEGS_GET_SERVER_SET_DATA_ACK { Ok = NetError.NET_OK };
                foreach (var g in groups.OrderBy(k => k.Key))
                {
                    int maxCcu = 0, ccu = 0;
                    foreach (var s in servers.Where(sv => sv.ServerGroupId == g.Key && sv.ServerClass == 1))
                    {
                        maxCcu += s.MaxUser;
                        ccu += s.CurrentUser;
                    }
                    if (maxCcu <= 0) maxCcu = 1;
                    sbyte level = ccu < 1500 ? KServerSetData.UclFree
                        : ccu < 3000 ? KServerSetData.UclNormal
                        : ccu < maxCcu ? KServerSetData.UclBusy
                        : KServerSetData.UclFull;
                    ack.ServerSetList.Add(new KServerSetData
                    {
                        ServerSetId = g.Key,
                        ServerSetName = g.Value.ServerGroupName,
                        UserCountLevel = level,
                    });
                }
                SendPacket(EventId.EGS_GET_SERVER_SET_DATA_ACK, ack);
                break;
            }

            case EventId.EGS_SELECT_SERVER_SET_REQ:
            {
                if (Fsm.StateId != KGSFSM.S_SERVER_SELECT) break;
                var req = ev.GetPayload<KEGS_SELECT_SERVER_SET_REQ>(Region);
                if (AuthLevel is >= 2 and <= 9 && AuthLevel is not (2 or 3 or 4))
                {
                    // Guard only explicitly negative/blocked levels (>=10 or account-block entries)
                }
                bool isBlocked = _server.Db.IsAccountBlocked(Region.Code, UserId, out string blockReason);
                if (isBlocked)
                {
                    SendPacket(EventId.EGS_SELECT_SERVER_SET_ACK, new KEGS_SELECT_SERVER_SET_ACK
                    {
                        Ok = NetError.ERR_VERIFY_11,
                        AccountBlockInfo = new KAccountBlockInfo
                        {
                            BlockReason = blockReason,
                            EndTime = string.Empty,
                        },
                    });
                    ReserveDestroy();
                    break;
                }

                var server = _server.Db.GetServerList(Region.Code)
                    .FirstOrDefault(s => s.ServerGroupId == req.ServerSetId && s.ServerClass == 1);
                if (server == null)
                {
                    SendPacket(EventId.EGS_SELECT_SERVER_SET_ACK, new KEGS_SELECT_SERVER_SET_ACK { Ok = NetError.ERR_CHANNEL_CHANGE_09 });
                    break;
                }

                SendPacket(EventId.EGS_SELECT_SERVER_SET_ACK, new KEGS_SELECT_SERVER_SET_ACK
                {
                    Ok = NetError.NET_OK,
                    Ip = string.IsNullOrEmpty(server.PublicIp) ? server.Ip : server.PublicIp,
                    Port = server.MasterPort > 0 ? server.MasterPort : _server.ListenPort,
                });
                break;
            }

            case EventId.EGS_CONNECT_CHANNEL_CHANGE_REQ:
            {
                if (Fsm.StateId != KGSFSM.S_INIT) break;
                var req = ev.GetPayload<KEGS_CONNECT_CHANNEL_CHANGE_REQ>(Region);
                if (!_server.TryConsumeChannelChangeReservation(req.SelectUnitReq.UnitUid, out var changeInfo))
                {
                    SendPacket(EventId.EGS_CONNECT_CHANNEL_CHANGE_ACK, new KEGS_CONNECT_CHANNEL_CHANGE_ACK
                    {
                        Ok = NetError.ERR_CHANNEL_CHANGE_06,
                    });
                    break;
                }

                var (netErr, userUid, authLevel, isGuest, _) = _server.Db.VerifyGameCredential(
                    Region.Code, req.VerifyAccountReq.UserId, req.VerifyAccountReq.Password);
                if (netErr != NetError.NET_OK || userUid <= 0)
                {
                    SendPacket(EventId.EGS_CONNECT_CHANNEL_CHANGE_ACK, new KEGS_CONNECT_CHANNEL_CHANGE_ACK
                    {
                        Ok = netErr != NetError.NET_OK ? netErr : NetError.ERR_CHANNEL_CHANGE_06,
                    });
                    break;
                }

                var unit = _server.Db.GetUnitByUid(Region, req.SelectUnitReq.UnitUid);
                if (unit == null || unit.OwnerUserUid != userUid)
                {
                    SendPacket(EventId.EGS_CONNECT_CHANNEL_CHANGE_ACK, new KEGS_CONNECT_CHANNEL_CHANGE_ACK
                    {
                        Ok = NetError.ERR_SELECT_UNIT_06,
                    });
                    break;
                }

                AccountUid = userUid;
                UserId = req.VerifyAccountReq.UserId;
                AuthLevel = authLevel;
                IsGuestUser = isGuest;
                SecondSecurityVerified = true;
                PcBangManager.IsPcBang = changeInfo.IsPcBang;
                PcBangManager.PcBangType = changeInfo.PcBangType;

                ActiveUnit = unit;
                ActiveUnit.Ip = IpStr;
                ActiveUnit.Port = _server.UdpRelayPort;
                if (changeInfo.EquippedTitleId > 0)
                    ActiveUnit.TitleId = changeInfo.EquippedTitleId;
                InitializeUnitTradeBlockState(unit);
                DungeonManager.InitClearedDungeons(unit.DungeonClear.Keys);
                DungeonManager.Init(
                    unlimitedEvent: false,
                    eventLimit: 0,
                    normalUsed: 0,
                    premiumUsed: 0,
                    eventUsed: 0,
                    challengeNormalUsed: 0,
                    isPcBang: PcBangManager.IsPcBang,
                    pcBangType: PcBangManager.PcBangType,
                    additionalHenirRewardCount: _server.GameSysValTable.GetAdditionalHenirRewardCount(PcBangManager.PcBangType));

                Fsm.TryStateTransition(KGSFSM.I_TO_LOGINED, out _);
                Fsm.TryStateTransition(KGSFSM.I_TO_SERVER_SELECT, out _);
                Fsm.TryStateTransition(KGSFSM.I_TO_FIELD_MAP, out _);
                _server.RegisterAccountSession(this);
                _server.RegisterActiveUnit(this);

                SendPacket(EventId.EGS_CONNECT_CHANNEL_CHANGE_ACK, new KEGS_CONNECT_CHANNEL_CHANGE_ACK
                {
                    Ok = NetError.NET_OK,
                    ConnectAck = new KEGS_CONNECT_ACK { Ok = NetError.NET_OK, UdpPort = _server.UdpEchoPort, ChannelId = 1 },
                    VerifyAccountAck = new KEGS_VERIFY_ACCOUNT_ACK
                    {
                        Ok = NetError.NET_OK,
                        ClientIp = IpStr,
                        ChannelCode = (byte)Region.DefaultChannelingCode,
                        ChannelUserId = UserId,
                        AccountInfo = new KAccountInfo
                        {
                            UserUid = userUid,
                            Id = UserId,
                            Name = UserId,
                            AuthLevel = authLevel,
                            IsGuestUser = isGuest,
                            LastLogin = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                        },
                    },
                    ChangeUserInfo = changeInfo,
                });
                SendUserLoginNotification();
                break;
            }

            case EventId.EGS_JOIN_FIELD_CHANNEL_CHANGE_REQ:
            {
                if (ActiveUnit == null || Fsm.StateId != KGSFSM.S_FIELD_MAP) break;
                var req = ev.GetPayload<KEGS_JOIN_FIELD_CHANNEL_CHANGE_REQ>(Region);
                if (req.MapId <= 0 || (CurrentFieldMapId > 0 && CurrentFieldMapId != req.MapId))
                {
                    SendPacket(EventId.EGS_JOIN_FIELD_CHANNEL_CHANGE_ACK, new KPacketOK { Ok = NetError.ERR_FIELD_00 });
                    break;
                }
                CurrentFieldMapId = req.MapId;
                ActiveUnit.LastPos.MapId = req.MapId;
                var zone = _server.FieldManager.GetOrCreateZone(CurrentFieldMapId);
                var myFieldInfo = BuildFieldUserInfo();
                var sectorUsers = zone.Enter(myFieldInfo);
                SendPacket(EventId.EGS_JOIN_FIELD_CHANNEL_CHANGE_ACK, new KPacketOK { Ok = NetError.NET_OK });
                SendPacket(EventId.EGS_CHANGE_USER_LIST_IN_SECTOR_NOT,
                    new KEGS_CHANGE_USER_LIST_IN_SECTOR_NOT { NewUsers = sectorUsers });
                foreach (long otherUid in zone.GetOtherUnitUids(ActiveUnit.UnitUid))
                {
                    _server.SendToUnit(otherUid, EventId.EGS_CHANGE_USER_LIST_IN_SECTOR_NOT,
                        new KEGS_CHANGE_USER_LIST_IN_SECTOR_NOT { NewUsers = [myFieldInfo] });
                }
                break;
            }

            case EventId.EGS_ENTRY_POINT_GET_CHANNEL_LIST_REQ:
            {
                var ack = new KEGS_ENTRY_POINT_GET_CHANNEL_LIST_ACK();
                var soles = _server.Db.GetChannelList(Region.Code, 0);
                var gaia = _server.Db.GetChannelList(Region.Code, 1);
                if (_server.ServerGroupId == 1)
                    (soles, gaia) = (gaia, soles);
                foreach (var kv in soles)
                {
                    ack.SolesChannelList[kv.Key] = kv.Value;
                    ack.SolesChannelBonusList[kv.Key] = new KChannelBonusInfo();
                }
                foreach (var kv in gaia)
                {
                    ack.GaiaChannelList[kv.Key] = kv.Value;
                    ack.GaiaChannelBonusList[kv.Key] = new KChannelBonusInfo();
                }
                SendPacket(EventId.EGS_ENTRY_POINT_GET_CHANNEL_LIST_ACK, ack);
                break;
            }

            case EventId.EGS_CHECK_THE_ARCHUANGEL_S_COIN_EVENT_LETTER_REQ:
            {
                if (Fsm.StateId != KGSFSM.S_SERVER_SELECT) break;
                bool received = _server.Db.HasTodayEventLetter(ActiveUnit?.UnitUid ?? Uid);
                SendPacket(EventId.EGS_CHECK_THE_ARCHUANGEL_S_COIN_EVENT_LETTER_ACK,
                    new KEGS_CHECK_THE_ARCHUANGEL_S_COIN_EVENT_LETTER_ACK
                    {
                        Ok = NetError.NET_OK,
                        ReceiveTheLetter = received,
                    });
                break;
            }

            case EventId.EGS_SYSTEM_INFO:
            {
                var info = ev.GetPayload<KEGS_SYSTEM_INFO>(Region);
                long auid = AccountUid > 0 ? AccountUid : info.AccountUid;
                if (auid > 0)
                    _server.Db.RecordUserSystemInfo(auid, info.SystemKey, info.Cpu, info.Memory, info.Gpu, info.Os);
                break;
            }

            case EventId.EGS_TCP_PING:
            {
                var ping = ev.GetPayload<KEGS_TCP_PING>(Region);
                SendPacket(EventId.EGS_TCP_PING, ping);
                break;
            }

            case EventId.EGS_KOG_GAME_PERFORMANCE_VILLAGE_LOG:
            {
                var log = ev.GetPayload<KEGS_KOG_GAME_PERFORMANCE_VILLAGE_LOG>(Region);
                _server.Db.RecordGamePerformanceLog("Village", log.CharUid, log.SystemKey, log.MapId, log.AvgFps, log.PartyUserNum);
                break;
            }

            case EventId.EGS_KOG_GAME_PERFORMANCE_FIELD_LOG:
            {
                var log = ev.GetPayload<KEGS_KOG_GAME_PERFORMANCE_FIELD_LOG>(Region);
                _server.Db.RecordGamePerformanceLog("Field", log.CharUid, log.SystemKey, log.MapId, log.AvgFps, log.PartyUserNum);
                break;
            }

            case EventId.EGS_KOG_GAME_PERFORMANCE_DUNGEON_LOG:
            {
                var log = ev.GetPayload<KEGS_KOG_GAME_PERFORMANCE_DUNGEON_LOG>(Region);
                _server.Db.RecordGamePerformanceLog("Dungeon", log.CharUid, log.SystemKey, log.MapId, log.AvgFps, log.PartyUserNum);
                break;
            }

            case EventId.EGS_KOG_GAME_PERFORMANCE_PVP_LOG:
            {
                var log = ev.GetPayload<KEGS_KOG_GAME_PERFORMANCE_PVP_LOG>(Region);
                _server.Db.RecordGamePerformanceLog("PvP", log.CharUid, log.SystemKey, log.MapId, log.AvgFps, log.PvpUserNum);
                break;
            }

            case EventId.EGS_GET_ACTIVE_LAGCHECK_REQ:
            {
                SendPacket(EventId.EGS_GET_ACTIVE_LAGCHECK_ACK,
                    new KEGS_GET_ACTIVE_LAGCHECK_ACK { ActiveLagCheck = _server.ActiveLagCheck });
                break;
            }

            case EventId.EGS_CLIENT_DIRECT_CONNECT_AUTH_PCBAG_REQ:
            {
                var req = ev.GetPayload<KEGS_CLIENT_DIRECT_CONNECT_AUTH_PCBAG_REQ>(Region);
                if (req.IsGameBangType <= -1 || !req.CheckClientDirectPcBang)
                {
                    PcBangManager.IsPcBang = false;
                    PcBangManager.PcBangType = -1;
                }
                else
                {
                    PcBangManager.IsPcBang = true;
                    PcBangManager.PcBangType = req.IsGameBangType;
                }
                var (exp, ed, _, spiritFree) = PcBangManager.GetPcBangReward();
                SendPacket(EventId.EGS_CLIENT_DIRECT_CONNECT_AUTH_PCBAG_ACK, new KEGS_CLIENT_DIRECT_CONNECT_AUTH_PCBAG_ACK
                {
                    CheckClientDirectPcBang = PcBangManager.IsPcBang,
                    IsGameBangType = PcBangManager.PcBangType,
                    PcBangReward = new KPcBangReward
                    {
                        PcBangType = PcBangManager.PcBangType,
                        ExpRate = exp,
                        EdRate = ed,
                        SpiritFree = spiritFree,
                    },
                });
                break;
            }

            // -------- Security / Anti-Hack (13) --------

            case EventId.EGS_AGREE_HACK_USER_REQ:
            {
                if (_hackingUserType == 1)
                {
                    _hackingUserType = 2;
                    if (AccountUid > 0) _server.Db.UpdateHackingUserType(AccountUid, _hackingUserType);
                    SendPacket(EventId.EGS_AGREE_HACK_USER_ACK, new KEGS_AGREE_HACK_USER_ACK
                    {
                        Ok = NetError.NET_OK,
                        HackingUserType = _hackingUserType,
                    });
                }
                else
                {
                    SendPacket(EventId.EGS_AGREE_HACK_USER_ACK, new KEGS_AGREE_HACK_USER_ACK
                    {
                        Ok = NetError.ERR_HACK_USER_00,
                        HackingUserType = _hackingUserType,
                    });
                }
                break;
            }

            case EventId.EGS_CHECK_SUM_REQ:
            {
                var req = ev.GetPayload<KEGS_CHECK_SUM_REQ>(Region);
                bool valid = _server.CheckSumManager.GetIsValidCheckSum(req.CheckSum);
                SendPacket(EventId.EGS_CHECK_SUM_ACK, new KEGS_CHECK_SUM_ACK
                {
                    Ok = valid ? NetError.NET_OK : NetError.ERR_VERIFY_02,
                });
                break;
            }

            case EventId.EGS_DUNGEON_KILLALLNPC_CHECK_REQ:
            {
                if (ActiveUnit == null || Fsm.StateId != KGSFSM.S_ROOM || CurrentRoomUid <= 0) break;
                var room = _server.Center.RoomManager.GetRoom(CurrentRoomUid);
                if (room == null) break;
                bool result = room.CheckDungeonKillAllNpc(ActiveUnit.UnitUid);
                SendPacket(EventId.EGS_DUNGEON_KILLALLNPC_CHECK_ACK, new KEGS_DUNGEON_KILLALLNPC_CHECK_ACK { Result = result });
                break;
            }

            case EventId.EGS_HACKING_USER_PROCESS_NAME_RECORD_NOT:
            {
                var not = ev.GetPayload<KEGS_HACKING_USER_PROCESS_NAME_RECORD_NOT>(Region);
                if (not.Type is >= 1 and <= 6)
                    _server.Db.RecordHackingProcessLog(AccountUid, ActiveUnit?.UnitUid ?? 0, not.Type, not.Reason);
                break;
            }

            case EventId.EGS_HACKSHIELD_CHECK_ACK:
            {
                var ack = ev.GetPayload<KEGS_HACKSHIELD_CHECK_ACK>(Region);
                if (ack.Data.Count == 0)
                    KLog.Clog($"KGSUser[{Uid}] empty HackShield check ack");
                break;
            }

            case EventId.EGS_KNPROTECT_AUTH_CHECK_NOT:
            {
                var not = ev.GetPayload<KEGS_KNPROTECT_AUTH_CHECK_NOT>(Region);
                if (not.Index == 0 && not.Value1 == 0 && not.Value2 == 0 && not.Value3 == 0 && AccountUid > 0)
                    _server.Db.RecordHackingProcessLog(AccountUid, ActiveUnit?.UnitUid ?? 0, 4, "KnProtectZeroAuth");
                break;
            }

            case EventId.EGS_MODULE_INFO_UPDATE_NOT:
            {
                var not = ev.GetPayload<KEGS_MODULE_INFO_UPDATE_NOT>(Region);
                if (not.ModuleName.Count == 0 && AccountUid > 0)
                    _server.Db.RecordHackingProcessLog(AccountUid, ActiveUnit?.UnitUid ?? 0, 3, "EmptyModuleList");
                break;
            }

            case EventId.EGS_NPROTECT_CSAUTH3_GAMEHACK_REPORT_NOT:
            {
                var not = ev.GetPayload<KEGS_NPROTECT_CSAUTH3_CHECK_NOT>(Region);
                if (Fsm.StateId is not (KGSFSM.S_FIELD_MAP or KGSFSM.S_ROOM)) break;
                KLog.Clog($"KGSUser[{Uid}] nProtect CSAuth3 gamehack report bytes={not.CsAuth3.Count}");
                break;
            }

            case EventId.EGS_NPROTECT_CSAUTH3_RESPONE_NOT:
            {
                var not = ev.GetPayload<KEGS_NPROTECT_CSAUTH3_CHECK_NOT>(Region);
                if (Fsm.StateId is not (KGSFSM.S_FIELD_MAP or KGSFSM.S_ROOM)) break;
                if (not.CsAuth3.Count > 4096)
                    ReserveDestroy();
                break;
            }

            case EventId.EGS_NPROTECT_HACKING_USER_NOT:
            {
                _server.StatisticsManager.IncrementCounter("DisconnectReason_nProtect_Hacking");
                if (_hackingUserType == 0)
                {
                    _hackingUserType = 1;
                    if (AccountUid > 0) _server.Db.UpdateHackingUserType(AccountUid, _hackingUserType);
                }
                if (AccountUid > 0)
                    _server.Db.RecordHackingProcessLog(AccountUid, ActiveUnit?.UnitUid ?? 0, 1, "nProtect hacking user notification");
                KLog.Cwarn($"KGSUser[{Uid}] nProtect hacking user notification received (Unit={ActiveUnit?.UnitUid})");
                break;
            }

            case EventId.EGS_REPORT_HACK_USER_NOT:
            {
                if (_hackingUserType == 0)
                {
                    _hackingUserType = 1;
                    if (AccountUid > 0) _server.Db.UpdateHackingUserType(AccountUid, _hackingUserType);
                }
                break;
            }

            case EventId.EGS_REQUEST_HACKSHIELD_CHECK_NOT:
            {
                if (Fsm.StateId is not (KGSFSM.S_FIELD_MAP or KGSFSM.S_ROOM)) break;
                SendPacket(EventId.EGS_HACKSHIELD_CHECK_REQ, new KEGS_HACKSHIELD_CHECK_REQ { Data = [1, 0, 0, 0] });
                break;
            }

            case EventId.EGS_SERVER_CHECK_HACK_USER_ACK:
            {
                var ack = ev.GetPayload<KEGS_SERVER_CHECK_HACK_USER_ACK>(Region);
                bool hmMatch = HackingManager.SetHackingUserReceiveType((sbyte)ack.HackingCheckType, out _);
                if (!hmMatch || (HackingUserSendType != 0 && ack.HackingCheckType != HackingUserSendType))
                {
                    SendPacket(EventId.EGS_CHECK_HACK_USER_PACKET_REQUEST_NOT, new KPacketOK { Ok = NetError.NET_OK });
                    ReserveDestroy();
                }
                else
                {
                    HackingUserSendType = 0;
                }
                break;
            }

            case EventId.EGS_XTRAP_ACK:
            {
                var ack = ev.GetPayload<KEGS_XTRAP_ACK>(Region);
                if (ack.Data.Count == 0)
                    KLog.Cwarn($"KGSUser[{Uid}] empty XTrap ack payload");
                else if (ack.Data.Count > 4096)
                    ReserveDestroy();
                break;
            }

            // -------- Inventory / Synthesis / Evaluate / ShareBank / Extract / Finality / Event Relationship (19) --------

            case EventId.EGS_USE_DEFENCE_DUNGEON_QUICK_SLOT_REQ:
            {
                if (ActiveUnit == null || Fsm.StateId != KGSFSM.S_ROOM) break;
                var req = ev.GetPayload<KEGS_USE_DEFENCE_DUNGEON_QUICK_SLOT_REQ>(Region);
                var room = CurrentRoomUid > 0 ? _server.Center.RoomManager.GetRoom(CurrentRoomUid) : null;
                if (room != null && (room.Info.DungeonMode == 1 || room.Info.DungeonMode == 2))
                {
                    SendPacket(EventId.EGS_USE_DEFENCE_DUNGEON_QUICK_SLOT_ACK,
                        new KEGS_USE_DEFENCE_DUNGEON_QUICK_SLOT_ACK { Ok = NetError.ERR_ITEM_09 });
                    break;
                }

                if (_defenceQuickSlotItems.TryGetValue(req.SlotId, out var quickSlotItem) && quickSlotItem.ItemInfo.Quantity > 0)
                {
                    int usedItemId = quickSlotItem.ItemInfo.ItemId;
                    double cooldownSec = QuickSlotCooldownSeconds.GetValueOrDefault(usedItemId, 0.0);
                    if (cooldownSec > 0.0 &&
                        _quickSlotCooldowns.TryGetValue(usedItemId, out var lastUseUtc) &&
                        (DateTime.UtcNow - lastUseUtc).TotalSeconds < cooldownSec)
                    {
                        SendPacket(EventId.EGS_USE_DEFENCE_DUNGEON_QUICK_SLOT_ACK,
                            new KEGS_USE_DEFENCE_DUNGEON_QUICK_SLOT_ACK { Ok = NetError.ERR_ITEM_08 });
                        break;
                    }

                    _quickSlotCooldowns[usedItemId] = DateTime.UtcNow;
                    int nextQty = Math.Max(0, quickSlotItem.ItemInfo.Quantity - 1);
                    quickSlotItem.ItemInfo.Quantity = nextQty;
                    if (nextQty == 0)
                    {
                        _defenceQuickSlotItems.Remove(req.SlotId);
                        quickSlotItem = new KInventoryItemInfo
                        {
                            ItemUid = 0,
                            SlotCategory = 15, // ST_E_DEFENCE_QUICK_SLOT
                            SlotId = req.SlotId,
                            ItemInfo = new KItemInfo { ItemId = 0, Quantity = 0 },
                        };
                    }
                    SendPacket(EventId.EGS_USE_DEFENCE_DUNGEON_QUICK_SLOT_ACK, new KEGS_USE_DEFENCE_DUNGEON_QUICK_SLOT_ACK
                    {
                        Ok = NetError.NET_OK,
                        InventorySlotInfo = quickSlotItem,
                        ItemId = usedItemId,
                    });
                    break;
                }

                var inv = _server.Db.GetInventory(ActiveUnit.UnitUid);
                var slotItem = inv.FirstOrDefault(i => i.SlotId == req.SlotId && (i.SlotCategory == 15 || i.SlotCategory == 3))
                    ?? inv.FirstOrDefault(i => i.SlotId == req.SlotId);
                if (slotItem == null)
                {
                    SendPacket(EventId.EGS_USE_DEFENCE_DUNGEON_QUICK_SLOT_ACK,
                        new KEGS_USE_DEFENCE_DUNGEON_QUICK_SLOT_ACK { Ok = NetError.ERR_ITEM_08 });
                    break;
                }
                var (ok, updatedSlot) = _server.Db.DeleteInventoryItem(ActiveUnit.UnitUid, slotItem.ItemUid, 1);
                SendPacket(EventId.EGS_USE_DEFENCE_DUNGEON_QUICK_SLOT_ACK, new KEGS_USE_DEFENCE_DUNGEON_QUICK_SLOT_ACK
                {
                    Ok = ok ? NetError.NET_OK : NetError.ERR_ITEM_08,
                    InventorySlotInfo = updatedSlot,
                    ItemId = slotItem.ItemInfo.ItemId,
                });
                break;
            }

            case EventId.EGS_OUT_USER_RETAINING_REQ:
            {
                if (ActiveUnit == null || Fsm.StateId is not (KGSFSM.S_FIELD_MAP or KGSFSM.S_ROOM)) break;
                var req = ev.GetPayload<KEGS_OUT_USER_RETAINING_REQ>(Region);
                bool validUnit = req.UnitUid <= 0 || req.UnitUid == ActiveUnit.UnitUid;
                var ack = new KEGS_OUT_USER_RETAINING_ACK { DepartureUser = validUnit && IsOutUserRetaining };
                if (ack.DepartureUser)
                {
                    foreach (var kv in _server.RewardTable.GetAllOutUserRetainingRewards())
                        ack.SlotItem[kv.Key] = new KRewardItem { ItemId = kv.Value.ItemId, Quantity = kv.Value.Quantity };
                }
                SendPacket(EventId.EGS_OUT_USER_RETAINING_ACK, ack);
                break;
            }

            case EventId.EGS_RETAINING_SELECT_REWARD_REQ:
            {
                if (ActiveUnit == null || Fsm.StateId is not (KGSFSM.S_FIELD_MAP or KGSFSM.S_ROOM)) break;
                var req = ev.GetPayload<KEGS_RETAINING_SELECT_REWARD_REQ>(Region);
                if (!IsOutUserRetaining)
                {
                    SendPacket(EventId.EGS_RETAINING_SELECT_REWARD_ACK,
                        new KEGS_RETAINING_SELECT_REWARD_ACK { Ok = NetError.ERR_UNKNOWN });
                    break;
                }
                if (!_server.RewardTable.TryGetOutUserRetainingReward(req.SlotId, out int rewardItemId, out int rewardQty))
                {
                    SendPacket(EventId.EGS_RETAINING_SELECT_REWARD_ACK,
                        new KEGS_RETAINING_SELECT_REWARD_ACK { Ok = NetError.ERR_ITEM_04 });
                    break;
                }
                var ack = new KEGS_RETAINING_SELECT_REWARD_ACK
                {
                    Ok = NetError.NET_OK,
                    RewardItem = new KRewardItem { ItemId = rewardItemId, Quantity = rewardQty },
                };
                if (rewardItemId == 130040) // CI_RESURRECTION_STONE
                {
                    NumResurrectionStone += rewardQty;
                    ack.ResurrectionStone = NumResurrectionStone;
                }
                else
                {
                    ack.InventorySlotInfo.Add(_server.Db.GrantUnitItem(ActiveUnit.UnitUid, rewardItemId, rewardQty));
                }
                IsOutUserRetaining = false;
                SendPacket(EventId.EGS_RETAINING_SELECT_REWARD_ACK, ack);
                break;
            }

            case EventId.EGS_RESOLVE_ITEM_AVATAR_REQ:
            {
                if (ActiveUnit == null || Fsm.StateId is not (KGSFSM.S_FIELD_MAP or KGSFSM.S_ROOM)) break;
                var req = ev.GetPayload<KEGS_RESOLVE_ITEM_AVATAR_REQ>(Region);
                if (PersonalShopManager.PersonalShopUid > 0)
                {
                    SendPacket(EventId.EGS_RESOLVE_ITEM_ACK, new KEGS_RESOLVE_ITEM_ACK { Ok = NetError.ERR_ITEM_14 });
                    break;
                }
                if (req.ResolveDataAvatar.Count == 0)
                {
                    SendPacket(EventId.EGS_RESOLVE_ITEM_ACK, new KEGS_RESOLVE_ITEM_ACK { Ok = NetError.ERR_ITEM_10 });
                    break;
                }

                var ack = new KEGS_RESOLVE_ITEM_ACK { Ok = NetError.NET_OK };
                foreach (var kv in req.ResolveDataAvatar)
                {
                    var (consumed, upd) = _server.Db.DeleteInventoryItem(ActiveUnit.UnitUid, kv.Value, 1);
                    if (!consumed) continue;
                    ack.InventorySlotInfo.Add(upd);
                    var (outs, jp) = _server.ResolveItemManager.ResolveItem(ActiveUnit.Level, isWeapon: false);
                    ack.Jackpot |= jp;
                    foreach (var o in outs)
                    {
                        ack.InsertedItems[o.Key] = ack.InsertedItems.GetValueOrDefault(o.Key) + o.Value;
                        var granted = _server.Db.GrantUnitItem(ActiveUnit.UnitUid, o.Key, o.Value, 4);
                        if (granted != null) ack.InventorySlotInfo.Add(granted);
                    }
                }
                SendPacket(EventId.EGS_RESOLVE_ITEM_ACK, ack);
                break;
            }

            case EventId.EGS_GET_SHARE_BANK_REQ:
            {
                if (ActiveUnit == null || Fsm.StateId is not (KGSFSM.S_FIELD_MAP or KGSFSM.S_ROOM)) break;
                var req = ev.GetPayload<KEGS_GET_SHARE_BANK_REQ>(Region);
                var (ok, uid, nick, bankSize, items) = _server.Db.GetShareBankItems(Region, AccountUid, req.NickName);
                var ack = new KEGS_GET_SHARE_BANK_ACK
                {
                    Ok = ok,
                    UnitUid = uid,
                    NickName = nick,
                    BankSize = bankSize,
                };
                foreach (var kv in items)
                    ack.Item[kv.Key] = kv.Value;
                SendPacket(EventId.EGS_GET_SHARE_BANK_ACK, ack);
                break;
            }

            case EventId.EGS_ITEM_CONVERT_REQ:
            {
                if (ActiveUnit == null || Fsm.StateId is not (KGSFSM.S_FIELD_MAP or KGSFSM.S_ROOM)) break;
                var req = ev.GetPayload<KEGS_ITEM_CONVERT_REQ>(Region);
                if (!InventoryLocker.TryLock(out int lockErr))
                {
                    SendPacket(EventId.EGS_ITEM_CONVERT_ACK, new KEGS_ITEM_CONVERT_ACK { Ok = lockErr });
                    break;
                }
                try
                {
                    if (IsTradeOrShopOpen())
                    {
                        SendPacket(EventId.EGS_ITEM_CONVERT_ACK, new KEGS_ITEM_CONVERT_ACK { Ok = NetError.ERR_ITEM_14 });
                        break;
                    }
                    int qty = Math.Max(1, req.Quantity);
                    var (srcFound, srcSlot) = _server.Db.GetUnitItemByUid(ActiveUnit.UnitUid, req.ItemUid);
                    if (!srcFound || srcSlot.ItemInfo.Quantity < qty)
                    {
                        SendPacket(EventId.EGS_ITEM_CONVERT_ACK, new KEGS_ITEM_CONVERT_ACK { Ok = NetError.ERR_ITEM_EXCHANGE_01 });
                        break;
                    }
                    if (srcSlot.ItemInfo.Period != 0 ||
                        !_server.TryGetItemConvertResult(srcSlot.ItemInfo.ItemId, out int resultItemId, out int commissionEd))
                    {
                        SendPacket(EventId.EGS_ITEM_CONVERT_ACK, new KEGS_ITEM_CONVERT_ACK { Ok = NetError.ERR_EVALUATE_ITEM_07 });
                        break;
                    }
                    int totalCommission = (int)Math.Min(int.MaxValue, (long)commissionEd * qty);
                    if (totalCommission > 0 && !EdManager.TryAddEd(-totalCommission, KUserEDManager.EdReason.ErOutEdBuyItemNpcShop, out _))
                    {
                        SendPacket(EventId.EGS_ITEM_CONVERT_ACK, new KEGS_ITEM_CONVERT_ACK { Ok = NetError.ERR_EVALUATE_ITEM_08 });
                        break;
                    }
                    var (delOk, delUpd) = _server.Db.DeleteInventoryItem(ActiveUnit.UnitUid, req.ItemUid, qty);
                    if (!delOk)
                    {
                        if (totalCommission > 0) EdManager.TryAddEd(totalCommission, KUserEDManager.EdReason.ErInEdSellItemNpcShop, out _);
                        SendPacket(EventId.EGS_ITEM_CONVERT_ACK, new KEGS_ITEM_CONVERT_ACK { Ok = NetError.ERR_ITEM_EXCHANGE_00 });
                        break;
                    }
                    ItemQuantityVariation.DecreaseQuantity(srcSlot.ItemInfo.ItemId, qty);
                    var granted = _server.Db.GrantUnitItem(ActiveUnit.UnitUid, resultItemId, qty);
                    ItemQuantityVariation.IncreaseQuantity(resultItemId, qty);
                    if (totalCommission > 0)
                    {
                        ActiveUnit.Ed = EdManager.CurrentEd;
                        _server.Db.UpdateUnitProgress(ActiveUnit.UnitUid, 0, -totalCommission, 0);
                    }
                    SendPacket(EventId.EGS_ITEM_CONVERT_ACK, new KEGS_ITEM_CONVERT_ACK
                    {
                        Ok = NetError.NET_OK,
                        Ed = ActiveUnit.Ed,
                        InventorySlotInfo = [delUpd, granted],
                    });
                }
                finally { InventoryLocker.Unlock(); }
                break;
            }

            case EventId.EGS_ITEM_EVALUATE_REQ:
            {
                if (ActiveUnit == null || Fsm.StateId is not (KGSFSM.S_FIELD_MAP or KGSFSM.S_ROOM)) break;
                var req = ev.GetPayload<KEGS_ITEM_EVALUATE_REQ>(Region);
                if (!InventoryLocker.TryLock(out int lockErr))
                {
                    SendPacket(EventId.EGS_ITEM_EVALUATE_ACK, new KEGS_ITEM_EVALUATE_ACK { Ok = lockErr });
                    break;
                }
                try
                {
                    if (IsTradeOrShopOpen())
                    {
                        SendPacket(EventId.EGS_ITEM_EVALUATE_ACK, new KEGS_ITEM_EVALUATE_ACK { Ok = NetError.ERR_ITEM_14 });
                        break;
                    }
                    const int evalFee = 1000;
                    if (!EdManager.TryAddEd(-evalFee, KUserEDManager.EdReason.ErOutEdBuyItemNpcShop, out int newEd))
                    {
                        SendPacket(EventId.EGS_ITEM_EVALUATE_ACK, new KEGS_ITEM_EVALUATE_ACK { Ok = NetError.ERR_EVALUATE_ITEM_02 });
                        break;
                    }
                    var (ok, _, updated) = _server.Db.SetInventoryItemEvalState(ActiveUnit.UnitUid, req.ItemUid, 0, 1);
                    if (ok != NetError.NET_OK || updated == null)
                    {
                        EdManager.TryAddEd(evalFee, KUserEDManager.EdReason.ErInEdSellItemNpcShop, out _);
                        SendPacket(EventId.EGS_ITEM_EVALUATE_ACK, new KEGS_ITEM_EVALUATE_ACK { Ok = ok });
                        break;
                    }
                    var rolled = _server.SocketItemManager.EvaluateRandomSockets(1, 2);
                    if (rolled.Count > 0)
                        updated = _server.Db.UpdateItemSockets(ActiveUnit.UnitUid, req.ItemUid, rolled);
                    ActiveUnit.Ed = newEd;
                    _server.Db.UpdateUnitProgress(ActiveUnit.UnitUid, 0, -evalFee, 0);
                    SendPacket(EventId.EGS_ITEM_EVALUATE_ACK, new KEGS_ITEM_EVALUATE_ACK
                    {
                        Ok = NetError.NET_OK,
                        Ed = ActiveUnit.Ed,
                        InventorySlotInfo = [updated],
                    });
                }
                finally { InventoryLocker.Unlock(); }
                break;
            }

            case EventId.EGS_RESTORE_ITEM_EVALUATE_REQ:
            {
                if (ActiveUnit == null || Fsm.StateId is not (KGSFSM.S_FIELD_MAP or KGSFSM.S_ROOM)) break;
                var req = ev.GetPayload<KEGS_RESTORE_ITEM_EVALUATE_REQ>(Region);
                if (!InventoryLocker.TryLock(out int lockErr))
                {
                    SendPacket(EventId.EGS_RESTORE_ITEM_EVALUATE_ACK, new KEGS_RESTORE_ITEM_EVALUATE_ACK { Ok = lockErr });
                    break;
                }
                try
                {
                    if (IsTradeOrShopOpen())
                    {
                        SendPacket(EventId.EGS_RESTORE_ITEM_EVALUATE_ACK, new KEGS_RESTORE_ITEM_EVALUATE_ACK { Ok = NetError.ERR_ITEM_14 });
                        break;
                    }
                    var (supFound, supSlot) = _server.Db.GetUnitItemByUid(ActiveUnit.UnitUid, req.SupportItemUid);
                    if (!supFound || supSlot.ItemInfo.Quantity <= 0)
                    {
                        SendPacket(EventId.EGS_RESTORE_ITEM_EVALUATE_ACK, new KEGS_RESTORE_ITEM_EVALUATE_ACK { Ok = NetError.ERR_EVALUATE_ITEM_00 });
                        break;
                    }
                    if (_server.SocketItemManager.MiracleCubes.Count > 0 &&
                        !_server.SocketItemManager.TryGetMiracleCubeInfo(supSlot.ItemInfo.ItemId, out _))
                    {
                        SendPacket(EventId.EGS_RESTORE_ITEM_EVALUATE_ACK, new KEGS_RESTORE_ITEM_EVALUATE_ACK { Ok = NetError.ERR_EVALUATE_ITEM_01 });
                        break;
                    }
                    var (ok, _, updated) = _server.Db.SetInventoryItemEvalState(ActiveUnit.UnitUid, req.TargetItemUid, 1, 0);
                    if (ok != NetError.NET_OK || updated == null)
                    {
                        SendPacket(EventId.EGS_RESTORE_ITEM_EVALUATE_ACK, new KEGS_RESTORE_ITEM_EVALUATE_ACK { Ok = ok });
                        break;
                    }
                    var (supDelOk, supportUpd) = _server.Db.DeleteInventoryItem(ActiveUnit.UnitUid, req.SupportItemUid, 1);
                    if (supDelOk) ItemQuantityVariation.DecreaseQuantity(supSlot.ItemInfo.ItemId, 1);
                    var slots = new List<KInventoryItemInfo> { updated };
                    if (supportUpd.ItemUid > 0) slots.Add(supportUpd);
                    SendPacket(EventId.EGS_RESTORE_ITEM_EVALUATE_ACK, new KEGS_RESTORE_ITEM_EVALUATE_ACK
                    {
                        Ok = NetError.NET_OK,
                        InventorySlotInfo = slots,
                    });
                }
                finally { InventoryLocker.Unlock(); }
                break;
            }

            case EventId.EGS_OPEN_RANDOM_ITEM_DEVELOPER_REQ:
            {
                if (AuthLevel < 3) break;
                var req = ev.GetPayload<KEGS_OPEN_RANDOM_ITEM_DEVELOPER_REQ>(Region);
                sbyte unitClass = (sbyte)(ActiveUnit?.UnitClass ?? 1);
                var ack = new KEGS_OPEN_RANDOM_ITEM_DEVELOPER_ACK
                {
                    Ok = NetError.NET_OK,
                    RandomItemResult = _server.RandomItemManager.OpenRandomItemDeveloper(
                        req.ItemId, req.ItemKeyId, req.OpenCount, unitClass),
                };
                SendPacket(EventId.EGS_OPEN_RANDOM_ITEM_DEVELOPER_ACK, ack);
                break;
            }

            case EventId.EGS_SYNTHESIS_ITEM_REQ:
            {
                if (ActiveUnit == null || Fsm.StateId is not (KGSFSM.S_FIELD_MAP or KGSFSM.S_ROOM)) break;
                var req = ev.GetPayload<KEGS_SYNTHESIS_ITEM_REQ>(Region);
                if (!InventoryLocker.Lock(KInventoryLocker.LockType.TimeOut, 30.0))
                {
                    SendPacket(EventId.EGS_SYNTHESIS_ITEM_ACK, new KEGS_SYNTHESIS_ITEM_ACK { Ok = NetError.ERR_INVENTORY_LOCK_00 });
                    break;
                }
                try
                {
                    if (IsTradeOrShopOpen())
                    {
                        SendPacket(EventId.EGS_SYNTHESIS_ITEM_ACK, new KEGS_SYNTHESIS_ITEM_ACK { Ok = NetError.ERR_ITEM_14 });
                        break;
                    }
                    if (req.SynthesisData.Count != 4 || req.KeyData <= 0 || req.SocketNo <= 0)
                    {
                        SendPacket(EventId.EGS_SYNTHESIS_ITEM_ACK, new KEGS_SYNTHESIS_ITEM_ACK { Ok = NetError.ERR_ITEM_10 });
                        break;
                    }
                    var allUids = req.SynthesisData.Values.Append(req.KeyData).ToList();
                    if (allUids.Distinct().Count() != 5)
                    {
                        SendPacket(EventId.EGS_SYNTHESIS_ITEM_ACK, new KEGS_SYNTHESIS_ITEM_ACK { Ok = NetError.ERR_SYNTHESIS_01 });
                        break;
                    }
                    foreach (long uid in allUids)
                    {
                        var (fnd, slt, _, _) = _server.Db.GetItemFullDetails(ActiveUnit.UnitUid, uid);
                        if (!fnd || slt.SlotCategory == 9)
                        {
                            SendPacket(EventId.EGS_SYNTHESIS_ITEM_ACK, new KEGS_SYNTHESIS_ITEM_ACK { Ok = NetError.ERR_SYNTHESIS_01 });
                            goto SynthesisDone;
                        }
                    }

                    var ack = new KEGS_SYNTHESIS_ITEM_ACK { Ok = NetError.NET_OK };
                    foreach (var kv in req.SynthesisData)
                    {
                        var (ok, upd) = _server.Db.DeleteInventoryItem(ActiveUnit.UnitUid, kv.Value, 1);
                        if (ok) ack.InventorySlotInfo.Add(upd);
                    }
                    var (keyOk, keyUpd) = _server.Db.DeleteInventoryItem(ActiveUnit.UnitUid, req.KeyData, 1);
                    if (keyOk) ack.InventorySlotInfo.Add(keyUpd);

                    int unitType = KRandomItemManager.GetBaseUnitClass((sbyte)ActiveUnit.UnitClass);
                    int equipPos = req.SynthesisData.Keys.FirstOrDefault() is int keyPos &&
                        keyPos is KSynthesisManager.EquipWeaponHand or KSynthesisManager.EquipDefenceHair
                            or KSynthesisManager.EquipDefenceBody or KSynthesisManager.EquipDefenceLeg
                            or KSynthesisManager.EquipDefenceHand or KSynthesisManager.EquipDefenceFoot
                        ? keyPos
                        : KSynthesisManager.EquipWeaponHand;

                    var (synOk, synItemId, _) = _server.SynthesisManager.GetSynthesisResultItem(unitType, equipPos, req.SocketNo);
                    int resultItemId = synOk == NetError.NET_OK && synItemId > 0 ? synItemId : 99976;
                    var synthesized = _server.Db.GrantUnitItem(ActiveUnit.UnitUid, resultItemId, 1, 1);
                    if (synthesized != null)
                    {
                        synthesized = _server.Db.UpdateItemSockets(ActiveUnit.UnitUid, synthesized.ItemUid, [req.SocketNo]);
                        ItemQuantityVariation.IncreaseQuantity(resultItemId, 1);
                        ack.InsertedItem[resultItemId] = 1;
                        ack.InventorySlotInfo.Add(synthesized);
                    }
                    SendPacket(EventId.EGS_SYNTHESIS_ITEM_ACK, ack);
                SynthesisDone:;
                }
                finally
                {
                    InventoryLocker.Unlock();
                }
                break;
            }

            case EventId.EGS_SYNTHESIS_SOCKET_GROUPID_REQ:
            {
                if (Fsm.StateId is not (KGSFSM.S_FIELD_MAP or KGSFSM.S_ROOM)) break;
                var ack = new KEGS_SYNTHESIS_SOCKET_GROUPID_ACK { Ok = NetError.NET_OK };
                foreach (var kv in _server.SynthesisManager.GetAvatarSocketOptionGroups())
                    ack.AvatarSocketOptionGroupId[kv.Key] = kv.Value;
                SendPacket(EventId.EGS_SYNTHESIS_SOCKET_GROUPID_ACK, ack);
                break;
            }

            case EventId.EGS_READY_TO_SOSUN_EVENT_REQ:
            {
                if (ActiveUnit == null || Fsm.StateId != KGSFSM.S_FIELD_MAP) break;
                int firstClass = 10 + Random.Shared.Next(20);
                if (firstClass == 29) firstClass = 28;
                var inv = _server.Db.GetInventory(ActiveUnit.UnitUid);
                var eventItem = inv.FirstOrDefault(i => i.ItemInfo.ItemId == 85000010);
                if (eventItem == null)
                {
                    SendPacket(EventId.EGS_READY_TO_SOSUN_EVENT_ACK, new KEGS_READY_TO_SOSUN_EVENT_ACK
                    {
                        Ok = NetError.ERR_ITEM_04,
                        FirstUnitClass = firstClass,
                    });
                    break;
                }
                var (_, upd) = _server.Db.DeleteInventoryItem(ActiveUnit.UnitUid, eventItem.ItemUid, 1);
                var reward = _server.Db.GrantUnitItem(ActiveUnit.UnitUid, 85000011, 1, 3);
                var slots = new List<KInventoryItemInfo> { upd };
                if (reward != null) slots.Add(reward);
                SendPacket(EventId.EGS_READY_TO_SOSUN_EVENT_ACK, new KEGS_READY_TO_SOSUN_EVENT_ACK
                {
                    Ok = NetError.NET_OK,
                    InventorySlotInfo = slots,
                    FirstUnitClass = firstClass,
                });
                break;
            }

            case EventId.EGS_USE_PROPOSE_ITEM_REQ:
            {
                if (ActiveUnit == null || Fsm.StateId != KGSFSM.S_FIELD_MAP) break;
                var req = ev.GetPayload<KEGS_USE_PROPOSE_ITEM_REQ>(Region);
                if (!RelationshipManager.IsSolo)
                {
                    SendPacket(EventId.EGS_USE_PROPOSE_ITEM_ACK,
                        new KEGS_USE_PROPOSE_ITEM_ACK { Ok = NetError.ERR_RELATIONSHIP_01, NickName = req.NickName });
                    break;
                }
                var target = _server.GetUserByNickName(req.NickName);
                if (target?.ActiveUnit == null || !target.RelationshipManager.IsSolo)
                {
                    SendPacket(EventId.EGS_USE_PROPOSE_ITEM_ACK,
                        new KEGS_USE_PROPOSE_ITEM_ACK { Ok = NetError.ERR_RELATIONSHIP_00, NickName = req.NickName });
                    break;
                }
                int charErr = KUserRelationshipManager.CheckChar(ActiveUnit.UnitClass, target.ActiveUnit.UnitClass);
                if (charErr != NetError.NET_OK)
                {
                    SendPacket(EventId.EGS_USE_PROPOSE_ITEM_ACK,
                        new KEGS_USE_PROPOSE_ITEM_ACK { Ok = charErr, NickName = req.NickName });
                    break;
                }
                var (_, upd) = _server.Db.DeleteInventoryItem(ActiveUnit.UnitUid, req.UsedItemUid, 1);
                SendPacket(EventId.EGS_USE_PROPOSE_ITEM_ACK, new KEGS_USE_PROPOSE_ITEM_ACK
                {
                    Ok = NetError.NET_OK,
                    NickName = target.ActiveUnit.NickName,
                    InventorySlotInfo = upd.ItemUid > 0 ? [upd] : [],
                });
                target.SendPacket(EventId.EGS_COUPLE_PROPOSE_NOT, new KEGS_COUPLE_PROPOSE_NOT
                {
                    RequestUnitUid = ActiveUnit.UnitUid,
                    RequestUnitName = ActiveUnit.NickName,
                });
                break;
            }

            case EventId.EGS_EVENT_PROPOSE_AGREE_NOT:
            {
                if (ActiveUnit == null || Fsm.StateId != KGSFSM.S_FIELD_MAP) break;
                var req = ev.GetPayload<KEGS_COUPLE_PROPOSE_AGREE_NOT>(Region);
                var requester = _server.GetUserByUnitUid(req.RequestUnitUid);
                if (requester?.ActiveUnit == null) break;

                long nowUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                if (req.Answer == 0)
                {
                    RelationshipManager.SetRelationshipType(KUserRelationshipManager.RelationshipType.RtCouple);
                    RelationshipManager.LoverUnitUid = requester.ActiveUnit.UnitUid;
                    RelationshipManager.LoverNickName = requester.ActiveUnit.NickName;
                    RelationshipManager.AnniversaryDayUtc = DateTime.UtcNow;

                    requester.RelationshipManager.SetRelationshipType(KUserRelationshipManager.RelationshipType.RtCouple);
                    requester.RelationshipManager.LoverUnitUid = ActiveUnit.UnitUid;
                    requester.RelationshipManager.LoverNickName = ActiveUnit.NickName;
                    requester.RelationshipManager.AnniversaryDayUtc = DateTime.UtcNow;

                    _server.Db.SaveRelationship(ActiveUnit.UnitUid, (sbyte)KUserRelationshipManager.RelationshipType.RtCouple,
                        requester.ActiveUnit.UnitUid, requester.ActiveUnit.NickName, string.Empty,
                        RelationshipManager.AnniversaryDayUtc, null, 0);
                    _server.Db.SaveRelationship(requester.ActiveUnit.UnitUid, (sbyte)KUserRelationshipManager.RelationshipType.RtCouple,
                        ActiveUnit.UnitUid, ActiveUnit.NickName, string.Empty,
                        requester.RelationshipManager.AnniversaryDayUtc, null, 0);
                }
                SendPacket(EventId.EGS_COUPLE_PROPOSE_RESULT_NOT, new KEGS_COUPLE_PROPOSE_RESULT_NOT
                {
                    Ok = NetError.NET_OK,
                    Result = req.Answer,
                    UnitUid = requester.ActiveUnit.UnitUid,
                    UnitName = requester.ActiveUnit.NickName,
                    DateUnix = nowUnix,
                });
                requester.SendPacket(EventId.EGS_COUPLE_PROPOSE_RESULT_NOT, new KEGS_COUPLE_PROPOSE_RESULT_NOT
                {
                    Ok = NetError.NET_OK,
                    Result = req.Answer,
                    UnitUid = ActiveUnit.UnitUid,
                    UnitName = ActiveUnit.NickName,
                    DateUnix = nowUnix,
                });
                break;
            }

            case EventId.EGS_USE_DIVORCE_ITEM_REQ:
            {
                if (ActiveUnit == null || Fsm.StateId != KGSFSM.S_FIELD_MAP) break;
                if (!RelationshipManager.IsCouple || RelationshipManager.LoverUnitUid <= 0)
                {
                    SendPacket(EventId.EGS_USE_DIVORCE_ITEM_ACK,
                        new KEGS_USE_DIVORCE_ITEM_ACK { Ok = NetError.ERR_RELATIONSHIP_EVENT_02 });
                    break;
                }
                long exUid = RelationshipManager.LoverUnitUid;
                RelationshipManager.SetBreakUp();
                _server.Db.SaveRelationship(ActiveUnit.UnitUid, (sbyte)KUserRelationshipManager.RelationshipType.RtSolo,
                    0, string.Empty, string.Empty, null, null, 0);
                var exUser = _server.GetUserByUnitUid(exUid);
                exUser?.RelationshipManager.SetBreakUp();
                _server.Db.SaveRelationship(exUid, (sbyte)KUserRelationshipManager.RelationshipType.RtSolo,
                    0, string.Empty, string.Empty, null, null, 0);
                exUser?.SendPacket(EventId.EGS_DIVORCE_NOT, new KEGS_BREAK_UP_NOT { Cost = 0 });
                SendPacket(EventId.EGS_USE_DIVORCE_ITEM_ACK, new KEGS_USE_DIVORCE_ITEM_ACK { Ok = NetError.NET_OK });
                break;
            }

            case EventId.EGS_USE_RECRUIT_TICKET_REQ:
            {
                if (ActiveUnit == null || Fsm.StateId != KGSFSM.S_FIELD_MAP) break;
                var req = ev.GetPayload<KEGS_USE_RECRUIT_TICKET_REQ>(Region);
                var meta = _server.Db.GetInventoryItemMeta(ActiveUnit.UnitUid, req.ItemUid);
                if (!meta.found)
                {
                    SendPacket(EventId.EGS_USE_RECRUIT_TICKET_ACK,
                        new KEGS_USE_RECRUIT_TICKET_ACK { Ok = NetError.ERR_RECOMMEND_USER_05 });
                    break;
                }
                _server.Db.DeleteInventoryItem(ActiveUnit.UnitUid, req.ItemUid, 1);
                _server.Db.SetUnitRecruitTicketUsed(ActiveUnit.UnitUid);
                SendPacket(EventId.EGS_USE_RECRUIT_TICKET_ACK, new KEGS_USE_RECRUIT_TICKET_ACK { Ok = NetError.NET_OK });
                break;
            }

            case EventId.EGS_REGISTER_RECRUITER_REQ:
            {
                if (ActiveUnit == null || Fsm.StateId != KGSFSM.S_FIELD_MAP) break;
                var req = ev.GetPayload<KEGS_REGISTER_RECRUITER_REQ>(Region);
                if (string.Equals(ActiveUnit.NickName, req.NickName, StringComparison.Ordinal))
                {
                    SendPacket(EventId.EGS_REGISTER_RECRUITER_ACK,
                        new KEGS_REGISTER_RECRUITER_ACK { Ok = NetError.ERR_RECOMMEND_USER_01 });
                    break;
                }
                if (KStringFilterManager.IsInvalidMarkIn(req.NickName))
                {
                    SendPacket(EventId.EGS_REGISTER_RECRUITER_ACK,
                        new KEGS_REGISTER_RECRUITER_ACK { Ok = NetError.ERR_ODBC_00 });
                    break;
                }
                var (ok, recruiterUid) = _server.Db.RegisterUnitRecruiter(Region, ActiveUnit.UnitUid, req.NickName);
                SendPacket(EventId.EGS_REGISTER_RECRUITER_ACK, new KEGS_REGISTER_RECRUITER_ACK { Ok = ok });
                if (ok == NetError.NET_OK && recruiterUid > 0)
                {
                    _server.SendToUnit(recruiterUid, EventId.EGS_REGISTER_RECRUITER_NOT, new KEGS_REGISTER_RECRUITER_NOT
                    {
                        RecruiterUnitUid = recruiterUid,
                        RecruitUnitInfo = new KRecommendUserInfo
                        {
                            UnitUid = ActiveUnit.UnitUid,
                            NickName = ActiveUnit.NickName,
                            Level = ActiveUnit.Level,
                            RecommendDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                            IsOnline = true,
                        },
                    });
                }
                break;
            }

            case EventId.EGS_ITEM_EXTRACT_REQ:
            {
                if (ActiveUnit == null || Fsm.StateId is not (KGSFSM.S_FIELD_MAP or KGSFSM.S_ROOM)) break;
                var req = ev.GetPayload<KEGS_ITEM_EXTRACT_REQ>(Region);
                if (PersonalShopManager.PersonalShopUid > 0)
                {
                    SendPacket(EventId.EGS_ITEM_EXTRACT_ACK, new KEGS_ITEM_EXTRACT_ACK { Ok = NetError.ERR_EXTRACT_00 });
                    break;
                }
                var meta = _server.Db.GetInventoryItemMeta(ActiveUnit.UnitUid, req.ItemUid);
                if (!meta.found)
                {
                    SendPacket(EventId.EGS_ITEM_EXTRACT_ACK, new KEGS_ITEM_EXTRACT_ACK { Ok = NetError.ERR_EXTRACT_01 });
                    break;
                }
                int qty = Math.Max(1, req.Quantity);
                var (ok, upd) = _server.Db.DeleteInventoryItem(ActiveUnit.UnitUid, req.ItemUid, qty);
                if (!ok)
                {
                    SendPacket(EventId.EGS_ITEM_EXTRACT_ACK, new KEGS_ITEM_EXTRACT_ACK { Ok = NetError.ERR_EXTRACT_02 });
                    break;
                }
                var ack = new KEGS_ITEM_EXTRACT_ACK
                {
                    Ok = NetError.NET_OK,
                    InventorySlotInfo = [upd],
                };
                var extracted = _server.Db.GrantUnitItem(ActiveUnit.UnitUid, KResolveItemManager.SmallElPiece, qty, 4);
                ack.InsertedItem[KResolveItemManager.SmallElPiece] = qty;
                if (extracted != null) ack.InventorySlotInfo.Add(extracted);
                SendPacket(EventId.EGS_ITEM_EXTRACT_ACK, ack);
                break;
            }

            case EventId.EGS_USE_FINALITY_SKILL_REQ:
            {
                if (ActiveUnit == null || Fsm.StateId != KGSFSM.S_ROOM) break;
                var req = ev.GetPayload<KEGS_USE_FINALITY_SKILL_REQ>(Region);
                if (req.NoConsume)
                {
                    var inv = _server.Db.GetInventory(ActiveUnit.UnitUid);
                    bool hasEventItem = inv.Any(i => i.ItemInfo.ItemId == 85000370);
                    SendPacket(EventId.EGS_USE_FINALITY_SKILL_ACK, new KEGS_USE_FINALITY_SKILL_ACK
                    {
                        Ok = hasEventItem ? NetError.NET_OK : NetError.ERR_FINALITY_SKILL_00,
                        NoConsume = hasEventItem,
                    });
                    break;
                }
                var meta = _server.Db.GetInventoryItemMeta(ActiveUnit.UnitUid, req.ItemUid);
                if (!meta.found)
                {
                    SendPacket(EventId.EGS_USE_FINALITY_SKILL_ACK,
                        new KEGS_USE_FINALITY_SKILL_ACK { Ok = NetError.ERR_FINALITY_SKILL_01 });
                    break;
                }
                var (ok, upd) = _server.Db.DeleteInventoryItem(ActiveUnit.UnitUid, req.ItemUid, 1);
                SendPacket(EventId.EGS_USE_FINALITY_SKILL_ACK, new KEGS_USE_FINALITY_SKILL_ACK
                {
                    Ok = ok ? NetError.NET_OK : NetError.ERR_FINALITY_SKILL_02,
                    InventorySlotInfo = ok ? [upd] : [],
                });
                break;
            }

            // -------- CashShop (24) + CashShop_Global (6) --------

            case EventId.EGS_APPLY_COUPON_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_APPLY_COUPON_REQ>(Region);
                var (ok, pno, itemId, qty) = _server.Db.RedeemCoupon(AccountUid, req.CouponSerial, req.CouponType);
                if (ok == NetError.NET_OK && itemId > 0)
                    _server.Db.AddBillingOrder(AccountUid, pno, itemId, $"Coupon_{itemId}", qty, 0, KBillOrderInfo.BcitCoupon);
                SendPacket(EventId.EGS_APPLY_COUPON_ACK, new KPacketOK { Ok = ok });
                break;
            }

            case EventId.EGS_PURCHASED_CASH_ITEM_LIST_REQ:
            {
                if (AccountUid <= 0) break;
                var req = ev.GetPayload<KEGS_PURCHASED_CASH_ITEM_LIST_REQ>(Region);
                var orders = _server.Db.GetBillingOrders(AccountUid, req.ItemPerPage, req.CurrentPage);
                var nxOrders = orders.Select(o => new KNXBTOrderInfo
                {
                    OrderNo = (uint)o.TransNo,
                    ProductNo = (uint)o.BillProductInfo.ProductNo,
                    ProductKind = o.ProductKind,
                    ProductName = o.BillProductInfo.ProductName,
                    ProductId = o.BillProductInfo.ProductId.ToString(),
                    ProductPieces = (ushort)Math.Max(1, (int)o.BillProductInfo.Quantity),
                    OrderQuantity = (ushort)Math.Max(1, (int)o.BillProductInfo.Quantity),
                    RemainOrderQuantity = (ushort)Math.Max(1, (int)o.BillProductInfo.Quantity),
                    IsPresent = (byte)(o.Type == KBillOrderInfo.BcitGift ? 1 : 0),
                    SenderGameId = o.SenderNickName,
                    SenderPresentMessage = o.SenderPresentMessage,
                }).ToList();
                SendPacket(EventId.EGS_PURCHASED_CASH_ITEM_LIST_ACK, new KEGS_PURCHASED_CASH_ITEM_LIST_ACK
                {
                    Ok = NetError.NET_OK,
                    OrderInfo = nxOrders,
                });
                break;
            }

            case EventId.EGS_GET_PURCHASED_CASH_ITEM_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_GET_PURCHASED_CASH_ITEM_REQ>(Region);
                int reqItemId = int.TryParse(req.ProductId, out int parsedId) ? parsedId : (int)req.ProductNo;
                if (_server.DailyGiftBoxManager.IsDailyGiftBoxItemId(reqItemId) &&
                    _dailyGiftBoxList.TryGetValue(reqItemId, out var dgbCheck))
                {
                    var activeEntry = dgbCheck.FirstOrDefault(e => e.UnitUid == ActiveUnit.UnitUid);
                    if (activeEntry.UnitUid == ActiveUnit.UnitUid)
                    {
                        if (activeEntry.State == (int)KDailyGiftBoxManager.PurchasedState.PurchasedRefund)
                        {
                            SendPacket(EventId.EGS_GET_PURCHASED_CASH_ITEM_ACK, new KEGS_GET_PURCHASED_CASH_ITEM_ACK { Ok = NetError.ERR_BUY_CASH_ITEM_52 });
                            break;
                        }
                        if (activeEntry.State == (int)KDailyGiftBoxManager.PurchasedState.PurchasedConfirm)
                        {
                            SendPacket(EventId.EGS_GET_PURCHASED_CASH_ITEM_ACK, new KEGS_GET_PURCHASED_CASH_ITEM_ACK { Ok = NetError.ERR_BUY_CASH_ITEM_08 });
                            break;
                        }
                    }
                }
                CashInventory.SetSocketForCash(req.SocketForCash);
                var (found, pid, qty, _) = _server.Db.MarkBillingOrderPickedUp(AccountUid, req.OrderNo);
                int itemId = found && pid > 0 ? pid : reqItemId;
                int grantQty = found ? qty : Math.Max(1, req.Quantity);
                var granted = itemId > 0
                    ? new List<KInventoryItemInfo> { _server.Db.GrantUnitItem(ActiveUnit.UnitUid, itemId, grantQty) }
                    : [];
                if (granted.Count > 0 && _server.DailyGiftBoxManager.IsDailyGiftBoxItemId(itemId))
                {
                    string regDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                    _server.Db.UpsertDailyGiftBoxPurchase(AccountUid, ActiveUnit.UnitUid, itemId, (int)KDailyGiftBoxManager.PurchasedState.PurchasedConfirm, regDate);
                    if (!_dailyGiftBoxList.TryGetValue(itemId, out var dgbEntries))
                    {
                        dgbEntries = [];
                        _dailyGiftBoxList[itemId] = dgbEntries;
                    }
                    bool updatedSelf = false;
                    for (int i = 0; i < dgbEntries.Count; i++)
                    {
                        if (dgbEntries[i].UnitUid == ActiveUnit.UnitUid && dgbEntries[i].State == (int)KDailyGiftBoxManager.PurchasedState.PurchasedBuy)
                        {
                            dgbEntries[i] = (ActiveUnit.UnitUid, (int)KDailyGiftBoxManager.PurchasedState.PurchasedConfirm, regDate);
                            updatedSelf = true;
                            break;
                        }
                        if (dgbEntries[i].UnitUid != ActiveUnit.UnitUid && dgbEntries[i].State == (int)KDailyGiftBoxManager.PurchasedState.PurchasedBuy)
                        {
                            long otherUnitUid = dgbEntries[i].UnitUid;
                            _server.Db.UpsertDailyGiftBoxPurchase(AccountUid, otherUnitUid, itemId, (int)KDailyGiftBoxManager.PurchasedState.PurchasedNot, regDate);
                            dgbEntries[i] = (otherUnitUid, (int)KDailyGiftBoxManager.PurchasedState.PurchasedNot, regDate);
                        }
                    }
                    if (!updatedSelf)
                    {
                        int selfIdx = dgbEntries.FindIndex(e => e.UnitUid == ActiveUnit.UnitUid);
                        var confirmEntry = (ActiveUnit.UnitUid, (int)KDailyGiftBoxManager.PurchasedState.PurchasedConfirm, regDate);
                        if (selfIdx >= 0) dgbEntries[selfIdx] = confirmEntry;
                        else dgbEntries.Add(confirmEntry);
                    }
                }
                SendPacket(EventId.EGS_GET_PURCHASED_CASH_ITEM_ACK, new KEGS_GET_PURCHASED_CASH_ITEM_ACK
                {
                    Ok = granted.Count > 0 ? NetError.NET_OK : NetError.ERR_BUY_CASH_ITEM_17,
                    NumResurrectionStone = NumResurrectionStone,
                    InventorySlotInfo = granted,
                });
                break;
            }

            case EventId.EGS_GET_PURCHASED_PACKAGE_CASH_ITEM_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_GET_PURCHASED_PACKAGE_CASH_ITEM_REQ>(Region);
                CashInventory.SetSocketForCash(req.SocketForCash);
                _ = _server.Db.MarkBillingOrderPickedUp(AccountUid, req.OrderNo);
                var granted = new List<KInventoryItemInfo>();
                if (req.PickUpPackageInfo.Count > 0)
                {
                    foreach (var p in req.PickUpPackageInfo)
                    {
                        int itemId = p.SubProductNo > 0 ? p.SubProductNo : (int.TryParse(p.ProductId, out int pid) ? pid : (int)p.ProductNo);
                        if (itemId > 0)
                            granted.Add(_server.Db.GrantUnitItem(ActiveUnit.UnitUid, itemId, Math.Max(1, (int)p.ProductPieces)));
                    }
                }
                else if (req.ProductId > 0)
                {
                    granted.Add(_server.Db.GrantUnitItem(ActiveUnit.UnitUid, req.ProductId, 1));
                }
                SendPacket(EventId.EGS_GET_PURCHASED_PACKAGE_CASH_ITEM_ACK, new KEGS_GET_PURCHASED_CASH_ITEM_ACK
                {
                    Ok = NetError.NET_OK,
                    NumResurrectionStone = NumResurrectionStone,
                    InventorySlotInfo = granted,
                });
                break;
            }

            case EventId.EGS_CASH_PRODUCT_INFO_REQ:
            {
                var req = ev.GetPayload<KEGS_CASH_PRODUCT_INFO_REQ>(Region);
                if (!_server.ContentManager.IsEnableCashShop)
                {
                    SendPacket(EventId.EGS_CASH_PRODUCT_INFO_ACK, new KEGS_CASH_PRODUCT_INFO_ACK { Final = true });
                    break;
                }
                var pageItems = _server.GetBillProductPage(_server.ServerGroupId, req.Page, out bool isFinal);
                var products = pageItems.Select(b => new KNXBTProductInfo
                {
                    ProductNo = (uint)b.ProductNo,
                    ProductExpire = b.Period,
                    ProductPieces = (ushort)Math.Max(1, (int)b.Quantity),
                    ProductId = b.ProductId.ToString(),
                    SalePrice = (uint)Math.Max(0, b.SalePrice),
                    CategoryNo = (uint)Math.Max(0, (int)b.CategoryNo),
                }).ToList();
                SendPacket(EventId.EGS_CASH_PRODUCT_INFO_ACK, new KEGS_CASH_PRODUCT_INFO_ACK
                {
                    Final = isFinal,
                    ProductInfo = products,
                });
                break;
            }

            case EventId.EGS_PRESENT_CASH_ITEM_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_PRESENT_CASH_ITEM_REQ>(Region);
                if (string.IsNullOrWhiteSpace(req.ReceiverNickName) || req.PurchaseReqInfo.Count == 0)
                {
                    SendPacket(EventId.EGS_PRESENT_CASH_ITEM_ACK, new KEGS_PRESENT_CASH_ITEM_ACK { Ok = NetError.ERR_BUY_CASH_ITEM_20 });
                    break;
                }
                var receiverUnit = _server.Db.GetUnitByNickName(Region, req.ReceiverNickName);
                if (receiverUnit == null)
                {
                    SendPacket(EventId.EGS_PRESENT_CASH_ITEM_ACK, new KEGS_PRESENT_CASH_ITEM_ACK { Ok = NetError.ERR_SEARCH_UNIT_03 });
                    break;
                }
                long? receiverAccUid = _server.Db.GetAccountUidByUnitUid(receiverUnit.UnitUid);
                if (!receiverAccUid.HasValue)
                {
                    SendPacket(EventId.EGS_PRESENT_CASH_ITEM_ACK, new KEGS_PRESENT_CASH_ITEM_ACK { Ok = NetError.ERR_BUY_CASH_ITEM_23 });
                    break;
                }
                uint totalCost = (uint)req.PurchaseReqInfo.Sum(p => (long)Math.Max(1, (int)p.OrderQuantity) * 100L);
                if (!_server.TrySpendAccountCash(AccountUid, totalCost, out _))
                {
                    SendPacket(EventId.EGS_PRESENT_CASH_ITEM_ACK, new KEGS_PRESENT_CASH_ITEM_ACK { Ok = NetError.ERR_BUY_CASH_ITEM_08 });
                    break;
                }
                foreach (var p in req.PurchaseReqInfo)
                {
                    int itemId = (int)p.ProductNo;
                    int qty = Math.Max(1, (int)p.OrderQuantity);
                    _server.Db.AddBillingOrder(receiverAccUid.Value, (int)p.ProductNo, itemId, $"Gift_{itemId}", qty, 100 * qty,
                        KBillOrderInfo.BcitGift, ActiveUnit.NickName, req.Message, ActiveUnit.UnitUid);
                    _server.SendToUnit(receiverUnit.UnitUid, EventId.EGS_PRESENT_MESSAGE_TO_RECEIVER_NOT, new KEGS_PRESENT_MESSAGE_TO_RECEIVER_NOT
                    {
                        SenderNickName = ActiveUnit.NickName,
                        ReceiverUserId = req.ReceiverNickName,
                        Message = req.Message,
                        BillProductInfo = new KBillProductInfo { ProductNo = (int)p.ProductNo, ProductId = itemId, Quantity = (sbyte)Math.Clamp(qty, 1, 127) },
                    });
                }
                SendPacket(EventId.EGS_PRESENT_CASH_ITEM_ACK, new KEGS_PRESENT_CASH_ITEM_ACK
                {
                    Ok = NetError.NET_OK,
                    PurchaseReqInfo = req.PurchaseReqInfo,
                    IsWishList = req.IsWishList,
                });
                break;
            }

            case EventId.EGS_BUY_CASH_ITEM_AUTOMATIC_PAYMENT_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_BUY_CASH_ITEM_AUTOMATIC_PAYMENT_REQ>(Region);
                uint price = 100u * Math.Max(1u, (uint)req.OrderQuantity);
                if (!_server.TrySpendAccountCash(AccountUid, price, out _))
                {
                    SendPacket(EventId.EGS_BUY_CASH_ITEM_AUTOMATIC_PAYMENT_ACK, new KEGS_BUY_CASH_ITEM_AUTOMATIC_PAYMENT_ACK
                    {
                        Ok = NetError.ERR_BUY_CASH_ITEM_08,
                        ProductNo = req.ProductNo,
                        OrderQuantity = req.OrderQuantity,
                    });
                    break;
                }
                NumAutoPaymentResStone += Math.Max(1, (int)req.OrderQuantity);
                SendPacket(EventId.EGS_AUTO_PAYMENT_RESURRECTION_STONE_NOT, new KEGS_AUTO_PAYMENT_RESURRECTION_STONE_NOT
                {
                    NumAutoPaymentResStone = NumAutoPaymentResStone,
                });
                SendPacket(EventId.EGS_BUY_CASH_ITEM_AUTOMATIC_PAYMENT_ACK, new KEGS_BUY_CASH_ITEM_AUTOMATIC_PAYMENT_ACK
                {
                    Ok = NetError.NET_OK,
                    UserSn = (uint)AccountUid,
                    TransactionId = $"AP-{AccountUid}-{DateTime.UtcNow.Ticks}",
                    ProductNo = req.ProductNo,
                    SalePrice = price,
                    OrderQuantity = req.OrderQuantity,
                });
                break;
            }

            case EventId.EGS_CHECK_PRESENT_CASH_INVENTORY_NOT:
            {
                if (AccountUid <= 0) break;
                var req = ev.GetPayload<KEGS_CHECK_PRESENT_CASH_INVENTORY_NOT>(Region);
                CashInventory.CheckedPresentOrders.Add((req.OrderNo, req.ProductNo));
                _server.Db.MarkBillingOrderChecked(AccountUid, req.OrderNo, req.ProductNo);
                break;
            }

            case EventId.EGS_GET_WISH_LIST_REQ:
            {
                if (AccountUid <= 0) break;
                var dbWish = _server.Db.GetWishList(AccountUid);
                WishList.InitWishList(dbWish.ToDictionary(kv => kv.Value, kv => kv.Key));
                SendPacket(EventId.EGS_GET_WISH_LIST_ACK, new KEGS_GET_WISH_LIST_ACK
                {
                    WishList = dbWish,
                });
                break;
            }

            case EventId.EGS_MODIFY_WISH_LIST_REQ:
            {
                if (AccountUid <= 0) break;
                var req = ev.GetPayload<KEGS_MODIFY_WISH_LIST_REQ>(Region);
                int err = NetError.NET_OK;
                foreach (int itemId in req.WishListItemId)
                {
                    if (itemId <= 0) continue;
                    if (req.IsDelete)
                    {
                        WishList.DelWish(itemId);
                    }
                    else
                    {
                        var (okWish, wishErr) = WishList.AddWish(itemId);
                        if (!okWish)
                        {
                            err = wishErr;
                            break;
                        }
                    }
                }
                if (err == NetError.NET_OK)
                {
                    _server.Db.ModifyWishList(AccountUid, req.WishListItemId, req.IsDelete);
                    WishList.FlushWishListChange();
                }
                SendPacket(EventId.EGS_MODIFY_WISH_LIST_ACK, new KEGS_MODIFY_WISH_LIST_ACK
                {
                    Ok = err,
                    WishListItemId = req.WishListItemId,
                    IsDelete = req.IsDelete,
                });
                break;
            }

            case EventId.EGS_CASH_ITEM_REFUND_REQ:
            {
                if (AccountUid <= 0) break;
                var req = ev.GetPayload<KEGS_CASH_ITEM_REFUND_REQ>(Region);
                int refundItemId = _server.Db.GetBillingOrderProductId(AccountUid, req.OrderNo);
                var (ok, refundPrice) = _server.Db.RefundBillingOrder(AccountUid, req.OrderNo);
                if (ok && refundPrice > 0)
                    _server.AddAccountCash(AccountUid, (uint)refundPrice);
                if (ok && _server.DailyGiftBoxManager.IsDailyGiftBoxItemId(refundItemId) &&
                    _dailyGiftBoxList.TryGetValue(refundItemId, out var dgbEntries))
                {
                    string regDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                    for (int i = 0; i < dgbEntries.Count; i++)
                    {
                        if (dgbEntries[i].State == (int)KDailyGiftBoxManager.PurchasedState.PurchasedBuy ||
                            dgbEntries[i].State == (int)KDailyGiftBoxManager.PurchasedState.PurchasedRefund)
                        {
                            long targetUnitUid = dgbEntries[i].UnitUid;
                            _server.Db.UpsertDailyGiftBoxPurchase(AccountUid, targetUnitUid, refundItemId, (int)KDailyGiftBoxManager.PurchasedState.PurchasedNot, regDate);
                            dgbEntries[i] = (targetUnitUid, (int)KDailyGiftBoxManager.PurchasedState.PurchasedNot, regDate);
                            break;
                        }
                    }
                }
                SendPacket(EventId.EGS_CASH_ITEM_REFUND_ACK, new KPacketOK
                {
                    Ok = ok ? NetError.NET_OK : NetError.ERR_BUY_CASH_ITEM_30,
                });
                break;
            }

            case EventId.EGS_VISIT_CASH_SHOP_NOT:
            {
                var req = ev.GetPayload<KEGS_VISIT_CASH_SHOP_NOT>(Region);
                CashInventory.IsInCashShop = req.EnterCashShop;
                UnitManager.EnterCashShop = req.EnterCashShop;
                break;
            }

            case EventId.EGS_BINGO_EVENT_INFO_REQ:
            {
                CashInventory.EnsureBingoInitialized(_server.BingoBanSpots, _server.BingoPresentInfos);
                SendPacket(EventId.EGS_BINGO_EVENT_INFO_ACK, new KEGS_BINGO_EVENT_INFO_ACK
                {
                    Ok = NetError.NET_OK,
                    LotteryChance = CashInventory.BingoLotteryChance,
                    LotteryRecord = CashInventory.BingoLotteryRecord,
                    MixChance = CashInventory.BingoMixChance,
                    ResetChance = CashInventory.BingoResetChance,
                    BingoBoard = [.. CashInventory.BingoBoard],
                    PresentInfo = [.. CashInventory.BingoPresents],
                });
                break;
            }

            case EventId.EGS_BINGO_EVENT_LOTTERY_REQ:
            {
                CashInventory.EnsureBingoInitialized(_server.BingoBanSpots, _server.BingoPresentInfos);
                var (ok, num, acquired) = CashInventory.BingoLottery();
                if (ok == NetError.NET_OK && ActiveUnit != null)
                {
                    foreach (byte lineIdx in acquired)
                    {
                        if (lineIdx < CashInventory.BingoPresents.Count)
                        {
                            int rewardItemId = CashInventory.BingoPresents[lineIdx].PresentId;
                            if (rewardItemId > 0)
                                _server.Db.GrantUnitItem(ActiveUnit.UnitUid, rewardItemId, 1);
                        }
                    }
                }
                SendPacket(EventId.EGS_BINGO_EVENT_LOTTERY_ACK, new KEGS_BINGO_EVENT_LOTTERY_ACK
                {
                    Ok = ok,
                    Num = num,
                    AcquiredPresent = acquired,
                    LotteryChance = CashInventory.BingoLotteryChance,
                    LotteryRecord = CashInventory.BingoLotteryRecord,
                });
                break;
            }

            case EventId.EGS_BINGO_EVENT_MIX_REQ:
            {
                CashInventory.EnsureBingoInitialized(_server.BingoBanSpots, _server.BingoPresentInfos);
                var (ok, board, acquired) = CashInventory.BingoMix();
                if (ok == NetError.NET_OK && ActiveUnit != null)
                {
                    foreach (byte lineIdx in acquired)
                    {
                        if (lineIdx < CashInventory.BingoPresents.Count)
                        {
                            int rewardItemId = CashInventory.BingoPresents[lineIdx].PresentId;
                            if (rewardItemId > 0)
                                _server.Db.GrantUnitItem(ActiveUnit.UnitUid, rewardItemId, 1);
                        }
                    }
                }
                SendPacket(EventId.EGS_BINGO_EVENT_MIX_ACK, new KEGS_BINGO_EVENT_MIX_ACK
                {
                    Ok = ok,
                    BingoBoard = board,
                    AcquiredPresent = acquired,
                    MixChance = CashInventory.BingoMixChance,
                });
                break;
            }

            case EventId.EGS_BINGO_EVENT_RESET_REQ:
            {
                CashInventory.EnsureBingoInitialized(_server.BingoBanSpots, _server.BingoPresentInfos);
                var (ok, board, presentIds) = CashInventory.BingoReset(isInit: false, _server.BingoBanSpots, _server.BingoPresentInfos);
                SendPacket(EventId.EGS_BINGO_EVENT_RESET_ACK, new KEGS_BINGO_EVENT_RESET_ACK
                {
                    Ok = ok,
                    BingoBoard = board,
                    PresentInfo = presentIds,
                    ResetChance = CashInventory.BingoResetChance,
                });
                break;
            }

            case EventId.EGS_BINGO_EVENT_INIT_REQ:
            {
                var (ok, board, presentIds) = CashInventory.BingoReset(isInit: true, _server.BingoBanSpots, _server.BingoPresentInfos);
                SendPacket(EventId.EGS_BINGO_EVENT_INIT_ACK, new KEGS_BINGO_EVENT_RESET_ACK
                {
                    Ok = ok,
                    BingoBoard = board,
                    PresentInfo = presentIds,
                    ResetChance = CashInventory.BingoResetChance,
                });
                break;
            }

            case EventId.EGS_COUPON_LIST_REQ:
            {
                var req = ev.GetPayload<KEGS_COUPON_LIST_REQ>(Region);
                var coupons = AccountUid > 0
                    ? _server.Db.GetAccountCoupons(AccountUid, req.Used, req.CouponBoxType, req.PageNum)
                    : [];
                int totalCoupons = AccountUid > 0
                    ? _server.Db.CountAccountCoupons(AccountUid, req.Used, req.CouponBoxType)
                    : 0;
                SendPacket(EventId.EGS_COUPON_LIST_ACK, new KEGS_COUPON_LIST_ACK
                {
                    Ok = NetError.NET_OK,
                    Used = req.Used,
                    CouponBoxType = req.CouponBoxType,
                    PageNum = req.PageNum,
                    CouponCount = totalCoupons,
                    CouponList = coupons,
                });
                break;
            }

            case EventId.EGS_REGIST_COUPON_REQ:
            {
                var req = ev.GetPayload<KEGS_REGIST_COUPON_REQ>(Region);
                var (ok, pno, iid, qty) = _server.Db.RedeemCoupon(AccountUid, req.CouponPin, 0);
                if (ok == NetError.NET_OK && iid > 0)
                    _server.Db.AddBillingOrder(AccountUid, pno, iid, $"Coupon_{iid}", qty, 0, KBillOrderInfo.BcitCoupon);
                SendPacket(EventId.EGS_REGIST_COUPON_ACK, new KPacketOK { Ok = ok });
                break;
            }

            case EventId.EGS_USE_ITEM_COUPON_REQ:
            {
                var req = ev.GetPayload<KEGS_USE_ITEM_COUPON_REQ>(Region);
                var (ok, itemId, qty) = AccountUid > 0
                    ? _server.Db.UseAccountItemCoupon(AccountUid, req.CouponCardNo)
                    : (NetError.ERR_NX_COUPON_06, 0, 0);
                if (ok == NetError.NET_OK && ActiveUnit != null && itemId > 0)
                    _server.Db.GrantUnitItem(ActiveUnit.UnitUid, itemId, qty);
                SendPacket(EventId.EGS_USE_ITEM_COUPON_ACK, new KPacketOK { Ok = ok });
                break;
            }

            case EventId.EGS_COUPON_TARGET_ITEM_REQ:
            {
                var req = ev.GetPayload<KEGS_COUPON_TARGET_ITEM_REQ>(Region);
                var coupons = AccountUid > 0
                    ? _server.Db.GetAccountCoupons(AccountUid, used: false, couponBoxType: 0, pageNum: 1)
                    : [];
                var matched = coupons.FirstOrDefault(c => c.CouponCardNo == req.CouponCardNo);
                var allTargets = matched?.TargetItem ?? [];
                const int pageSize = 10;
                int page = Math.Max(1, (int)req.PageNum);
                var pagedTargets = allTargets.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                SendPacket(EventId.EGS_COUPON_TARGET_ITEM_ACK, new KEGS_COUPON_TARGET_ITEM_ACK
                {
                    Ok = NetError.NET_OK,
                    CouponCardNo = req.CouponCardNo,
                    PageNum = req.PageNum,
                    TargetItemCount = allTargets.Count,
                    TargetItem = pagedTargets,
                });
                break;
            }

            case EventId.EGS_DISCOUNT_COUPON_INQUIRY_REQ:
            {
                var req = ev.GetPayload<KDiscountCouponInquriyInfo>(Region);
                var discounts = AccountUid > 0
                    ? _server.Db.InquireDiscountCoupons(AccountUid, [req.ProductNo])
                    : [];
                SendPacket(EventId.EGS_DISCOUNT_COUPON_INQUIRY_ACK, new KEGS_DISCOUNT_COUPON_INQUIRY_ACK
                {
                    Ok = NetError.NET_OK,
                    DiscountCoupon = discounts,
                });
                break;
            }

            case EventId.EGS_DISCOUNT_COUPON_LIST_INQUIRY_REQ:
            {
                var req = ev.GetPayload<KEGS_DISCOUNT_COUPON_LIST_INQUIRY_REQ>(Region);
                var discounts = AccountUid > 0
                    ? _server.Db.InquireDiscountCoupons(AccountUid, req.DiscountCouponInquiryInfo.Select(x => x.ProductNo))
                    : [];
                SendPacket(EventId.EGS_DISCOUNT_COUPON_LIST_INQUIRY_ACK, new KEGS_DISCOUNT_COUPON_INQUIRY_ACK
                {
                    Ok = NetError.NET_OK,
                    DiscountCoupon = discounts,
                });
                break;
            }

            case EventId.EGS_GET_NAVER_ACCESS_TOKEN_REQ:
            {
                SendPacket(EventId.EGS_GET_NAVER_ACCESS_TOKEN_ACK, new KEGS_GET_NAVER_ACCESS_TOKEN_ACK
                {
                    Ok = Region.Region == RegionCode.TW ? NetError.ERR_CONTENT_00 : NetError.NET_OK,
                    AccessToken = string.Empty,
                });
                break;
            }

            case EventId.NAVER_SOAP_GET_TOKEN_ACK:
            {
                var ack = ev.GetPayload<KNAVER_SOAP_GET_TOKEN_ACK>(Region);
                SendPacket(EventId.EGS_GET_NAVER_ACCESS_TOKEN_ACK, new KEGS_GET_NAVER_ACCESS_TOKEN_ACK
                {
                    Ok = ack.Ok,
                    AccessToken = ack.AccessToken,
                });
                break;
            }

            case EventId.EGS_BILL_PRODUCT_INFO_REQ:
            {
                var req = ev.GetPayload<KEGS_BILL_PRODUCT_INFO_REQ>(Region);
                if (!_server.ContentManager.IsEnableCashShop)
                {
                    SendPacket(EventId.EGS_BILL_PRODUCT_INFO_ACK, new KEGS_BILL_PRODUCT_INFO_ACK { Final = true });
                    break;
                }
                SendPacket(EventId.EGS_BONUS_CASH_EVENT_NOT, new KEGS_BONUS_CASH_EVENT_NOT
                {
                    EventStart = _server.BonusCashEventActive,
                });
                var ack = new KEGS_BILL_PRODUCT_INFO_ACK
                {
                    ProductInfo = _server.GetBillProductPage(_server.ServerGroupId, req.Page, out bool isFinal),
                    Final = isFinal,
                };
                foreach (var kv in _server.KeepShowItems)
                    ack.KeepShowItem[kv.Key] = [.. kv.Value];
                foreach (var kv in _server.KeepDiscountItems)
                    ack.DisCountInfoMap[kv.Key] = kv.Value;
                SendPacket(EventId.EGS_BILL_PRODUCT_INFO_ACK, ack);
                break;
            }

            case EventId.EGS_BILL_INVENTORY_INQUIRY_REQ:
            {
                if (AccountUid <= 0) break;
                var req = ev.GetPayload<KEGS_BILL_INVENTORY_INQUIRY_REQ>(Region);
                var orders = _server.Db.GetBillingOrders(AccountUid, req.ItemPerPage, req.CurrentPage);
                SendPacket(EventId.EGS_BILL_INVENTORY_INQUIRY_ACK, new KEGS_BILL_INVENTORY_INQUIRY_ACK
                {
                    Ok = NetError.NET_OK,
                    OrderInfo = orders,
                });
                break;
            }

            case EventId.EGS_BILL_GET_PURCHASED_CASH_ITEM_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_BILL_GET_PURCHASED_CASH_ITEM_REQ>(Region);
                CashInventory.SetSocketForCash(req.SocketForCash);
                var (found, pid, qty, _) = _server.Db.MarkBillingOrderPickedUp(AccountUid, req.TransNo);
                int itemId = found && pid > 0 ? pid : req.ProductNo;
                int grantQty = found ? qty : 1;
                var granted = itemId > 0
                    ? new List<KInventoryItemInfo> { _server.Db.GrantUnitItem(ActiveUnit.UnitUid, itemId, grantQty) }
                    : [];
                SendPacket(EventId.EGS_BILL_GET_PURCHASED_CASH_ITEM_ACK, new KEGS_GET_PURCHASED_CASH_ITEM_ACK
                {
                    Ok = granted.Count > 0 ? NetError.NET_OK : NetError.ERR_BUY_CASH_ITEM_17,
                    NumResurrectionStone = NumResurrectionStone,
                    InventorySlotInfo = granted,
                });
                break;
            }

            case EventId.EGS_EXCHANGE_CASH_REQ:
            {
                if (AccountUid <= 0) break;
                var req = ev.GetPayload<KEGS_EXCHANGE_CASH_REQ>(Region);
                if (req.ExchangeValue == 0)
                {
                    SendPacket(EventId.EGS_EXCHANGE_CASH_ACK, new KEGS_EXCHANGE_CASH_ACK { Ok = NetError.ERR_EXCHANGE_CASH });
                    break;
                }
                _server.AddAccountCash(AccountUid, req.ExchangeValue);
                CashInventory.GlobalCashInfo.Cash[KGlobalCashInfo.GctKogElswordCash] += req.ExchangeValue;
                SendPacket(EventId.EGS_EXCHANGE_CASH_ACK, new KEGS_EXCHANGE_CASH_ACK
                {
                    Ok = NetError.NET_OK,
                    Point = (int)_server.GetAccountCash(AccountUid),
                    AddBonusCashInfo = CashInventory.GlobalCashInfo,
                });
                break;
            }

            case EventId.EGS_EXCHANGE_CASH_NOT:
            {
                var req = ev.GetPayload<KEGS_EXCHANGE_CASH_CLIENT_NOT>(Region);
                SendPacket(EventId.EGS_EXCHANGE_CASH_CLIENT_NOT, req);
                break;
            }

            case EventId.EGS_CASH_DIRECT_CHARGE_CN_REQ:
            {
                var req = ev.GetPayload<KEGS_CASH_DIRECT_CHARGE_CN_REQ>(Region);
                bool ok = Region.Region == RegionCode.CN && req.UserUid == AccountUid;
                SendPacket(EventId.EGS_CASH_DIRECT_CHARGE_CN_ACK, new KEGS_CASH_DIRECT_CHARGE_CN_ACK
                {
                    Ok = ok ? NetError.NET_OK : NetError.ERR_CONTENT_00,
                    Token = ok ? $"CN-{AccountUid}-{DateTime.UtcNow.Ticks}" : string.Empty,
                });
                break;
            }

            // -------- AdminCheat (50 unported handlers -> 100% GSUserAdminCheat.cpp parity) --------

            case EventId.EGS_NOTIFY_MSG_REQ:
            {
                var req = ev.GetPayload<KEGS_NOTIFY_MSG_REQ>(Region);
                if (AuthLevel < 2) break; // UAL_GM
                _server.BroadcastNotice(req.NotifyType, req.Count, req.Msg, req.RandomItem);
                _server.Db.WriteCheatLog(ActiveUnit?.UnitUid ?? 0, AccountUid, "/n", req.Msg);
                break;
            }

            case EventId.EGS_ADMIN_SERVER_STATUS_REQ:
            {
                if (AuthLevel < 2)
                {
                    SendPacket(EventId.EGS_ADMIN_SERVER_STATUS_ACK, new KEGS_ADMIN_SERVER_STATUS_ACK { Ok = NetError.ERR_VERIFY_12 });
                    break;
                }
                var (ccu, pvpRooms, curParty, playParty) = _server.GetServerStatusSnapshot();
                SendPacket(EventId.EGS_ADMIN_SERVER_STATUS_ACK, new KEGS_ADMIN_SERVER_STATUS_ACK
                {
                    Ok = NetError.NET_OK,
                    Ccu = ccu,
                    NumPvpRoom = pvpRooms,
                    CurPartyCount = curParty,
                    PlayGamePartyCount = playParty,
                });
                break;
            }

            case EventId.EGS_ADMIN_CHANGE_AUTH_LEVEL_REQ:
            {
                var req = ev.GetPayload<KEGS_ADMIN_CHANGE_AUTH_LEVEL_REQ>(Region);
                if (AuthLevel < 3) // UAL_DEVELOPER
                {
                    SendPacket(EventId.EGS_ADMIN_CHANGE_AUTH_LEVEL_ACK, new KPacketOK { Ok = NetError.ERR_VERIFY_12 });
                    break;
                }
                _server.Db.SetAccountAuthLevel(Region, req.UserId, req.AuthLevel);
                var online = _server.GetUserByUserId(req.UserId);
                if (online != null) online.AuthLevel = req.AuthLevel;
                _server.Db.WriteCheatLog(ActiveUnit?.UnitUid ?? 0, AccountUid, "/auth", $"{req.UserId}:{req.AuthLevel}");
                SendPacket(EventId.EGS_ADMIN_CHANGE_AUTH_LEVEL_ACK, new KPacketOK { Ok = NetError.NET_OK });
                break;
            }

            case EventId.EGS_ADMIN_GET_AUTH_LEVEL_LIST_REQ:
            {
                var req = ev.GetPayload<KEGS_ADMIN_GET_AUTH_LEVEL_LIST_REQ>(Region);
                if (AuthLevel < 3)
                {
                    SendPacket(EventId.EGS_ADMIN_GET_AUTH_LEVEL_LIST_ACK, new KEGS_ADMIN_GET_AUTH_LEVEL_LIST_ACK { Ok = NetError.ERR_VERIFY_12 });
                    break;
                }
                SendPacket(EventId.EGS_ADMIN_GET_AUTH_LEVEL_LIST_ACK, new KEGS_ADMIN_GET_AUTH_LEVEL_LIST_ACK
                {
                    Ok = NetError.NET_OK,
                    UserIdList = _server.Db.GetAuthLevelUsers(Region, req.AuthLevel),
                });
                break;
            }

            case EventId.EGS_ADMIN_KICK_USER_REQ:
            {
                var req = ev.GetPayload<KEGS_ADMIN_KICK_USER_REQ>(Region);
                if (AuthLevel < 2)
                {
                    SendPacket(EventId.EGS_ADMIN_KICK_USER_ACK, new KPacketOK { Ok = NetError.ERR_VERIFY_12 });
                    break;
                }
                bool kicked = _server.KickUserByAdmin(req.UnitNickName, req.IsUserId);
                _server.Db.WriteCheatLog(ActiveUnit?.UnitUid ?? 0, AccountUid, "/kick", req.UnitNickName);
                SendPacket(EventId.EGS_ADMIN_KICK_USER_ACK, new KPacketOK
                {
                    Ok = kicked ? NetError.NET_OK : NetError.ERR_SEARCH_UNIT_03,
                });
                break;
            }

            case EventId.EGS_ADMIN_CHANGE_UNIT_CLASS_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_ADMIN_CHANGE_UNIT_CLASS_REQ>(Region);
                if (AuthLevel < 3)
                {
                    SendPacket(EventId.EGS_ADMIN_CHANGE_UNIT_CLASS_ACK, new KEGS_ADMIN_CHANGE_UNIT_CLASS_ACK { Ok = NetError.ERR_VERIFY_12 });
                    break;
                }
                _server.Db.SetUnitClass(ActiveUnit.UnitUid, req.UnitClass);
                ActiveUnit = _server.Db.GetUnitByUid(Region, ActiveUnit.UnitUid);
                int baseSkill = Math.Max(1, (int)req.UnitClass) * 100;
                SendPacket(EventId.EGS_ADMIN_CHANGE_UNIT_CLASS_ACK, new KEGS_ADMIN_CHANGE_UNIT_CLASS_ACK
                {
                    Ok = NetError.NET_OK,
                    UnitClass = req.UnitClass,
                    SPoint = SkillTree.AvailableSkillPoints,
                    CsPoint = CashInventory.CashSkillPoint,
                    DefaultSkill1 = baseSkill + 1,
                    DefaultSkill2 = baseSkill + 2,
                    DefaultSkill3 = baseSkill + 3,
                    DefaultSkill4 = baseSkill + 4,
                    DefaultSkill5 = baseSkill + 5,
                    DefaultSkill6 = baseSkill + 6,
                });
                break;
            }

            case EventId.EGS_ADMIN_GET_SKILL_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_ADMIN_GET_SKILL_REQ>(Region);
                if (AuthLevel < 3)
                {
                    SendPacket(EventId.EGS_ADMIN_GET_SKILL_ACK, new KEGS_ADMIN_GET_SKILL_ACK { Ok = NetError.ERR_VERIFY_12 });
                    break;
                }
                SkillTree.ActiveSkillPage = req.ActiveSkillPageNumber;
                bool learned = SkillTree.LearnOrSetSkill(req.SkillId, req.SkillLevel);
                SendPacket(EventId.EGS_ADMIN_GET_SKILL_ACK, new KEGS_ADMIN_GET_SKILL_ACK
                {
                    Ok = learned ? NetError.NET_OK : NetError.ERR_SKILL_01,
                    SkillId = req.SkillId,
                    SkillLevel = SkillTree.GetSkillLevel(req.SkillId),
                    ActiveSkillPageNumber = req.ActiveSkillPageNumber,
                    CsPoint = CashInventory.CashSkillPoint,
                    Unsealed = true,
                });
                break;
            }

            case EventId.EGS_ADMIN_AUTO_GET_ALL_SKILL_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_ADMIN_AUTO_GET_ALL_SKILL_REQ>(Region);
                if (AuthLevel < 3)
                {
                    SendPacket(EventId.EGS_ADMIN_AUTO_GET_ALL_SKILL_ACK, new KEGS_ADMIN_AUTO_GET_ALL_SKILL_ACK { Ok = NetError.ERR_VERIFY_12 });
                    break;
                }
                SkillTree.ActiveSkillPage = req.ActiveSkillPageNumber;
                var skills = SkillTree.LearnAllDefaultSkills(ActiveUnit.UnitClass);
                SendPacket(EventId.EGS_ADMIN_AUTO_GET_ALL_SKILL_ACK, new KEGS_ADMIN_AUTO_GET_ALL_SKILL_ACK
                {
                    Ok = NetError.NET_OK,
                    ActiveSkillPageNumber = req.ActiveSkillPageNumber,
                    SPoint = SkillTree.AvailableSkillPoints,
                    CsPoint = CashInventory.CashSkillPoint,
                    SkillList = skills,
                    UnsealedSkillId = skills.Keys.Select(k => (short)k).ToList(),
                });
                break;
            }

            case EventId.EGS_ADMIN_CHEAT_GET_ALL_SKILL_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_ADMIN_CHEAT_GET_ALL_SKILL_REQ>(Region);
                if (AuthLevel < 3 || !_server.CompanyIpTable.CheckIp(Session.RemoteIp))
                {
                    SendPacket(EventId.EGS_ADMIN_CHEAT_GET_ALL_SKILL_ACK, new KEGS_ADMIN_CHEAT_GET_ALL_SKILL_ACK { Ok = NetError.ERR_VERIFY_12 });
                    break;
                }
                SkillTree.ActiveSkillPage = req.ActiveSkillPageNumber;
                var skills = SkillTree.LearnAllDefaultSkills(ActiveUnit.UnitClass);
                SendPacket(EventId.EGS_ADMIN_CHEAT_GET_ALL_SKILL_ACK, new KEGS_ADMIN_CHEAT_GET_ALL_SKILL_ACK
                {
                    Ok = NetError.NET_OK,
                    ActiveSkillPageNumber = req.ActiveSkillPageNumber,
                    UnSealedSkill = skills.Keys.Select(k => (short)k).ToList(),
                    SkillInfo = skills.ToDictionary(kv => kv.Key, kv => new KAdminCheatSkill { SkillLevel = kv.Value }),
                });
                break;
            }

            case EventId.EGS_ADMIN_CHANGE_SKILL_POINT_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_ADMIN_CHANGE_SKILL_POINT_REQ>(Region);
                if (AuthLevel < 3)
                {
                    SendPacket(EventId.EGS_ADMIN_CHANGE_SKILL_POINT_ACK, new KEGS_ADMIN_CHANGE_SKILL_POINT_ACK { Ok = NetError.ERR_VERIFY_12 });
                    break;
                }
                SkillTree.SetAvailableSkillPoints(req.ActiveSkillPageNumber, req.SPoint);
                _server.Db.SetUnitSkillPoint(ActiveUnit.UnitUid, SkillTree.AvailableSkillPoints);
                SendPacket(EventId.EGS_ADMIN_CHANGE_SKILL_POINT_ACK, new KEGS_ADMIN_CHANGE_SKILL_POINT_ACK
                {
                    Ok = NetError.NET_OK,
                    SPoint = SkillTree.AvailableSkillPoints,
                    ActiveSkillPageNumber = req.ActiveSkillPageNumber,
                });
                break;
            }

            case EventId.EGS_ADMIN_INIT_SKILL_TREE_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_ADMIN_INIT_SKILL_TREE_REQ>(Region);
                if (AuthLevel < 3)
                {
                    SendPacket(EventId.EGS_ADMIN_INIT_SKILL_TREE_ACK, new KEGS_ADMIN_INIT_SKILL_TREE_ACK { Ok = NetError.ERR_VERIFY_12 });
                    break;
                }
                SkillTree.ActiveSkillPage = req.ActiveSkillPageNumber;
                SkillTree.ResetSkillPage(_server.Db, ActiveUnit.UnitUid, ActiveUnit.Level);
                SendPacket(EventId.EGS_ADMIN_INIT_SKILL_TREE_ACK, new KEGS_ADMIN_INIT_SKILL_TREE_ACK
                {
                    Ok = NetError.NET_OK,
                    ActiveSkillPageNumber = req.ActiveSkillPageNumber,
                    SPoint = SkillTree.AvailableSkillPoints,
                    CsPoint = CashInventory.CashSkillPoint,
                });
                break;
            }

            case EventId.EGS_ADMIN_CASH_SKILL_POINT_DATE_CHANGE_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_ADMIN_CASH_SKILL_POINT_DATE_CHANGE_REQ>(Region);
                if (AuthLevel < 3)
                {
                    SendPacket(EventId.EGS_ADMIN_CASH_SKILL_POINT_DATE_CHANGE_ACK, new KEGS_ADMIN_CASH_SKILL_POINT_DATE_CHANGE_ACK { Ok = NetError.ERR_VERIFY_12 });
                    break;
                }
                var dt = new DateTime(
                    Math.Clamp(req.Year, 2000, 2099),
                    Math.Clamp(req.Month, 1, 12),
                    Math.Clamp(req.Day, 1, 28),
                    Math.Clamp(req.Hour, 0, 23),
                    Math.Clamp(req.Minute, 0, 59), 0);
                string endStr = dt.ToString("yyyy-MM-dd HH:mm:ss");
                CashInventory.CashSkillPointEndDate = endStr;
                _server.Db.SetUnitCashSkillPointEndDate(ActiveUnit.UnitUid, endStr);
                SendPacket(EventId.EGS_ADMIN_CASH_SKILL_POINT_DATE_CHANGE_ACK, new KEGS_ADMIN_CASH_SKILL_POINT_DATE_CHANGE_ACK
                {
                    Ok = NetError.NET_OK,
                    CspEndDate = endStr,
                });
                break;
            }

            case EventId.EGS_ADMIN_GET_TITLE_CHEAT_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_ADMIN_GET_TITLE_CHEAT_REQ>(Region);
                if (AuthLevel < 3)
                {
                    SendPacket(EventId.EGS_ADMIN_GET_TITLE_CHEAT_ACK, new KPacketOK { Ok = NetError.ERR_VERIFY_12 });
                    break;
                }
                TitleManager.UnlockTitle(_server.Db, ActiveUnit.UnitUid, req.TitleId);
                SendPacket(EventId.EGS_ADMIN_GET_TITLE_CHEAT_ACK, new KPacketOK { Ok = NetError.NET_OK });
                break;
            }

            case EventId.EGS_ADMIN_QUEST_COMPLETE_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_QUEST_COMPLETE_REQ>(Region);
                if (AuthLevel < 3) break;
                _server.Db.CompleteQuest(ActiveUnit.UnitUid, req.QuestId);
                SendPacket(EventId.EGS_QUEST_COMPLETE_ACK, new KEGS_QUEST_COMPLETE_ACK
                {
                    Ok = NetError.NET_OK,
                    CompleteQuestInfo = new KCompleteQuestInfo
                    {
                        QuestId = req.QuestId,
                        CompleteCount = 1,
                        CompleteDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    },
                    UpdateUnitInfo = new KEGS_UPDATE_UNIT_INFO_NOT
                    {
                        UnitInfo = ActiveUnit ?? new KUnitInfo(),
                    },
                });
                break;
            }

            case EventId.EGS_ADMIN_CHANGE_VSPOINT_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_ADMIN_CHANGE_VSPOINT_REQ>(Region);
                if (AuthLevel < 3)
                {
                    SendPacket(EventId.EGS_ADMIN_CHANGE_VSPOINT_ACK, new KEGS_ADMIN_CHANGE_VSPOINT_ACK { Ok = NetError.ERR_VERIFY_12 });
                    break;
                }
                _server.Db.SetUnitPvpStats(ActiveUnit.UnitUid, req.Rating, req.RPoint, req.APoint);
                ActiveUnit = _server.Db.GetUnitByUid(Region, ActiveUnit.UnitUid);
                sbyte rank = (sbyte)(req.Rating >= 2000 ? 6 : req.Rating >= 1600 ? 4 : req.Rating >= 1200 ? 2 : 1);
                SendPacket(EventId.EGS_ADMIN_CHANGE_VSPOINT_ACK, new KEGS_ADMIN_CHANGE_VSPOINT_ACK
                {
                    Ok = NetError.NET_OK,
                    Rank = rank,
                    ChangedRPoint = req.RPoint,
                    ChangedAPoint = req.APoint,
                });
                break;
            }

            case EventId.EGS_ADMIN_OBSERVER_MODE_REQ:
            {
                var req = ev.GetPayload<KEGS_ADMIN_OBSERVER_MODE_REQ>(Region);
                if (AuthLevel < 2)
                {
                    SendPacket(EventId.EGS_ADMIN_OBSERVER_MODE_ACK, new KPacketOK { Ok = NetError.ERR_VERIFY_12 });
                    break;
                }
                IsObserverMode = req.ObserverMode;
                SendPacket(EventId.EGS_ADMIN_OBSERVER_MODE_ACK, new KPacketOK { Ok = NetError.NET_OK });
                break;
            }

            case EventId.EGS_ADMIN_SET_PC_BANG_REQ:
            {
                var req = ev.GetPayload<KEGS_ADMIN_SET_PC_BANG_REQ>(Region);
                if (AuthLevel < 3)
                {
                    SendPacket(EventId.EGS_ADMIN_SET_PC_BANG_ACK, new KEGS_ADMIN_SET_PC_BANG_ACK { Ok = NetError.ERR_VERIFY_12 });
                    break;
                }
                PcBangManager.IsPcBang = req.On;
                PcBangManager.PcBangType = req.PcBangType;
                var (exp, ed, _, spiritFree) = PcBangManager.GetPcBangReward();
                SendPacket(EventId.EGS_ADMIN_SET_PC_BANG_ACK, new KEGS_ADMIN_SET_PC_BANG_ACK
                {
                    Ok = NetError.NET_OK,
                    On = req.On,
                    PcBangType = req.PcBangType,
                    PcBangReward = new KPcBangReward
                    {
                        PcBangType = req.PcBangType,
                        ExpRate = exp,
                        EdRate = ed,
                        SpiritFree = spiritFree,
                    },
                });
                break;
            }

            case EventId.EGS_ADMIN_GET_GUILD_SKILL_POINT_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_ADMIN_GET_GUILD_SKILL_POINT_REQ>(Region);
                if (AuthLevel < 3)
                {
                    SendPacket(EventId.EGS_ADMIN_GET_GUILD_SKILL_POINT_ACK, new KEGS_ADMIN_GET_GUILD_SKILL_POINT_ACK { Ok = NetError.ERR_VERIFY_12 });
                    break;
                }
                var (ok, cur) = _server.Db.AddGuildSkillPoints(ActiveUnit.UserGuildInfo.GuildUid, req.GuildSPoint);
                SendPacket(EventId.EGS_ADMIN_GET_GUILD_SKILL_POINT_ACK, new KEGS_ADMIN_GET_GUILD_SKILL_POINT_ACK
                {
                    Ok = ok,
                    CurGuildSPoint = cur,
                });
                break;
            }

            case EventId.EGS_ADMIN_CHANGE_PET_INFO_NOT:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_ADMIN_CHANGE_PET_INFO_NOT>(Region);
                if (AuthLevel < 3) break;
                var (ok, pet) = PetManager.AdminChangePetInfo(req.CheatType, req.Value);
                if (ok && pet != null)
                {
                    if (req.CheatType == KEGS_ADMIN_CHANGE_PET_INFO_NOT.PctEvolutionStep)
                    {
                        SendPacket(EventId.EGS_PET_EVOLUTION_NOT, new KEGS_PET_EVOLUTION_NOT
                        {
                            UnitUid = ActiveUnit.UnitUid,
                            PetInfo = pet,
                        });
                    }
                    else if (req.CheatType == KEGS_ADMIN_CHANGE_PET_INFO_NOT.PctSatiety)
                    {
                        SendPacket(EventId.EGS_CHANGED_PET_SATIETY_NOT, new KEGS_CHANGED_PET_SATIETY_NOT
                        {
                            UnitUid = ActiveUnit.UnitUid,
                            PetUid = pet.PetUid,
                            CurrentSatiety = pet.Satiety,
                        });
                    }
                    else if (req.CheatType == KEGS_ADMIN_CHANGE_PET_INFO_NOT.PctIntimacy)
                    {
                        SendPacket(EventId.EGS_CHANGED_PET_INTIMACY_NOT, new KEGS_CHANGED_PET_INTIMACY_NOT
                        {
                            UnitUid = ActiveUnit.UnitUid,
                            PetUid = pet.PetUid,
                            CurrentIntimacy = pet.Intimacy,
                        });
                    }
                }
                break;
            }

            case EventId.EGS_ADMIN_NPC_UNIT_CREATE_REQ:
            {
                if (ActiveUnit == null || CurrentRoomUid <= 0) break;
                var req = ev.GetPayload<KEGS_ADMIN_NPC_UNIT_CREATE_REQ>(Region);
                if (AuthLevel < 3)
                {
                    SendPacket(EventId.EGS_ADMIN_NPC_UNIT_CREATE_ACK, new KPacketOK { Ok = NetError.ERR_VERIFY_12 });
                    break;
                }
                var room = _server.Center.RoomManager.GetRoom(CurrentRoomUid);
                if (room != null)
                {
                    var createNot = new KEGS_ADMIN_NPC_UNIT_CREATE_NOT
                    {
                        NpcUnitNot = req.NpcUnitReq,
                        Attrib1 = req.Attrib1,
                        Attrib2 = req.Attrib2,
                        Sa1 = req.Sa1,
                        Sa2 = req.Sa2,
                    };
                    foreach (var uid in room.GetMemberUnitUids())
                        _server.SendToUnit(uid, EventId.EGS_ADMIN_NPC_UNIT_CREATE_NOT, createNot);
                }
                SendPacket(EventId.EGS_ADMIN_NPC_UNIT_CREATE_ACK, new KPacketOK { Ok = NetError.NET_OK });
                break;
            }

            case EventId.EGS_ADMIN_GUEST_USER_MODE_NOT:
            {
                if (AuthLevel < 3) break;
                var ks = new KSerializer(Region);
                ks.BeginReading(ev.Buffer);
                if (ks.Get(out bool guest))
                    IsGuestUser = guest;
                ks.EndReading();
                break;
            }

            case EventId.EGS_ADMIN_GET_ITEM_SET_NOT:
            {
                if (ActiveUnit == null || AuthLevel < 3) break;
                _server.Db.GrantUnitItem(ActiveUnit.UnitUid, 130001, 10);
                _server.Db.WriteCheatLog(ActiveUnit.UnitUid, AccountUid, "/itemset", ActiveUnit.NickName);
                break;
            }

            case EventId.EGS_ADMIN_INCREASE_PARTY_FEVER_POINT_NOT:
            {
                if (ActiveUnit == null || AuthLevel < 3 || CurrentPartyUid <= 0) break;
                var party = _server.Center.PartyManager.GetParty(CurrentPartyUid);
                if (party != null)
                {
                    int fever = party.IncreaseFeverByAdmin(1000);
                    var not = new KEGS_PARTY_FEVER_POINT_NOT { PartyFever = fever };
                    foreach (var uid in party.GetUnitUids())
                        _server.SendToUnit(uid, EventId.EGS_PARTY_FEVER_POINT_NOT, not);
                }
                break;
            }

            case EventId.EGS_ADMIN_DELETE_GUILD_ADD_NOT:
            {
                var req = ev.GetPayload<KEGS_ADMIN_DELETE_GUILD_ADD_NOT>(Region);
                if (AuthLevel < 3) break;
                _server.Db.DeleteGuildByAdmin(req.GuildUid);
                _server.Db.WriteCheatLog(ActiveUnit?.UnitUid ?? 0, AccountUid, "/deleteguild", req.GuildUid.ToString());
                break;
            }

            case EventId.EGS_ADMIN_DELETE_HENIR_RANKING_RANK_NOT:
            {
                var req = ev.GetPayload<KEGS_ADMIN_DELETE_HENIR_RANKING_RANK_NOT>(Region);
                if (AuthLevel < 3) break;
                _server.Router.Send(PerformerTypeId, Uid, PerformerId.PI_LOGIN_SERVER, 0,
                    EventId.ELG_ADMIN_DELETE_HENIR_RANKING_RANK_NOT, req);
                _server.Db.WriteCheatLog(ActiveUnit?.UnitUid ?? 0, AccountUid, "/delhenirrank", $"{req.RankingType}:{req.Rank}");
                break;
            }

            case EventId.EGS_ADMIN_DELETE_HENIR_RANKING_NICKNAME_NOT:
            {
                var req = ev.GetPayload<KEGS_ADMIN_DELETE_HENIR_RANKING_NICKNAME_NOT>(Region);
                if (AuthLevel < 3) break;
                _server.Router.Send(PerformerTypeId, Uid, PerformerId.PI_LOGIN_SERVER, 0,
                    EventId.ELG_ADMIN_DELETE_HENIR_RANKING_NICKNAME_NOT, req);
                _server.Db.WriteCheatLog(ActiveUnit?.UnitUid ?? 0, AccountUid, "/delhenirnick", $"{req.RankingType}:{req.NickName}");
                break;
            }

            case EventId.EGS_ADMIN_DELETE_HENIR_RANKING_UNITUID_NOT:
            {
                var req = ev.GetPayload<KEGS_ADMIN_DELETE_HENIR_RANKING_UNITUID_NOT>(Region);
                if (AuthLevel < 3) break;
                _server.Router.Send(PerformerTypeId, Uid, PerformerId.PI_LOGIN_SERVER, 0,
                    EventId.ELG_ADMIN_DELETE_HENIR_RANKING_UNITUID_NOT, req);
                _server.Db.WriteCheatLog(ActiveUnit?.UnitUid ?? 0, AccountUid, "/delheniruid", $"{req.RankingType}:{req.UnitUid}");
                break;
            }

            case EventId.EGS_ADMIN_HENIR_REWARD_CHEAT_NOT:
            {
                var req = ev.GetPayload<KEGS_ADMIN_HENIR_REWARD_CHEAT_NOT>(Region);
                if (AuthLevel < 3) break;
                HenirRewardNormalCnt = Math.Max(0, req.HenirRewardNormalCnt);
                break;
            }

            case EventId.EGS_ADMIN_PSHOP_AGENCY_CHEAT_REQ:
            {
                var req = ev.GetPayload<KEGS_ADMIN_PSHOP_AGENCY_CHEAT_REQ>(Region);
                if (AuthLevel < 3) break;
                PShopAgencyCheatPeriod = req.Period;
                break;
            }

            case EventId.EGS_ADMIN_HERO_PVP_USER_LIST_REQ:
            {
                if (AuthLevel < 2) break;
                SendPacket(EventId.EGS_ADMIN_HERO_PVP_USER_LIST_ACK, new KEGS_ADMIN_HERO_PVP_USER_LIST_ACK
                {
                    UserList = _server.GetHeroPvpUsers(),
                });
                break;
            }

            case EventId.EGS_ADMIN_HERO_PVP_USER_ADD_REQ:
            {
                var req = ev.GetPayload<KEGS_ADMIN_HERO_PVP_USER_ADD_REQ>(Region);
                if (AuthLevel < 2) break;
                _server.AddHeroPvpUsers(req.UserList);
                SendPacket(EventId.EGS_ADMIN_HERO_PVP_USER_ADD_ACK, req);
                break;
            }

            case EventId.EGS_ADMIN_HERO_PVP_USER_REMOVE_REQ:
            {
                var req = ev.GetPayload<KEGS_ADMIN_HERO_PVP_USER_ADD_REQ>(Region);
                if (AuthLevel < 2) break;
                _server.RemoveHeroPvpUsers(req.UserList);
                SendPacket(EventId.EGS_ADMIN_HERO_PVP_USER_REMOVE_ACK, req);
                break;
            }

            case EventId.EGS_ADMIN_NOTIFY_HERO_PVP_USER_REQ:
            {
                var req = ev.GetPayload<KEGS_ADMIN_NOTIFY_HERO_PVP_USER_REQ>(Region);
                if (AuthLevel < 2)
                {
                    SendPacket(EventId.EGS_ADMIN_NOTIFY_HERO_PVP_USER_ACK, new KPacketOK { Ok = NetError.ERR_VERIFY_12 });
                    break;
                }
                if (!string.IsNullOrEmpty(req.NickName) && (req.NickName.Length < 2 || req.NickName.Length > 13))
                {
                    SendPacket(EventId.EGS_ADMIN_NOTIFY_HERO_PVP_USER_ACK, new KPacketOK { Ok = NetError.ERR_HERO_PVP_USER_LIST_01 });
                    break;
                }
                var target = string.IsNullOrEmpty(req.NickName) ? null : _server.GetUserByNickName(req.NickName);
                if (target != null)
                {
                    target.SendPacket(EventId.EGS_ADMIN_NOTIFY_HERO_PVP_USER_NOT, req);
                }
                else
                {
                    _server.BroadcastNotice(KEGS_NOTIFY_MSG_REQ.NtGmNotifyAllServers, 1, req.Message);
                }
                SendPacket(EventId.EGS_ADMIN_NOTIFY_HERO_PVP_USER_ACK, new KPacketOK { Ok = NetError.NET_OK });
                break;
            }

            case EventId.EGS_ADMIN_CHANGE_COUPLE_DATE_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_ADMIN_CHANGE_COUPLE_DATE_REQ>(Region);
                if (AuthLevel < 3)
                {
                    SendPacket(EventId.EGS_ADMIN_CHANGE_COUPLE_DATE_ACK, new KPacketOK { Ok = NetError.ERR_VERIFY_12 });
                    break;
                }
                if (RelationshipManager.IsSolo)
                {
                    SendPacket(EventId.EGS_ADMIN_CHANGE_COUPLE_DATE_ACK, new KPacketOK { Ok = NetError.ERR_RELATIONSHIP_36 });
                    break;
                }
                RelationshipManager.AnniversaryDayUtc = (RelationshipManager.AnniversaryDayUtc ?? DateTime.UtcNow).AddHours(-req.AddHour);
                _server.Db.SaveRelationship(ActiveUnit.UnitUid, (sbyte)RelationshipManager.CurrentType,
                    RelationshipManager.LoverUnitUid, RelationshipManager.LoverNickName, RelationshipManager.LoveWord,
                    RelationshipManager.AnniversaryDayUtc, RelationshipManager.LastRewardDateUtc, RelationshipManager.RewardTitleType);
                SendPacket(EventId.EGS_ADMIN_CHANGE_COUPLE_DATE_ACK, new KPacketOK { Ok = NetError.NET_OK });
                break;
            }

            case EventId.EGS_ADMIN_SHOW_ED_CHEAT_REQ:
            {
                var req = ev.GetPayload<KEGS_ADMIN_SHOW_ED_CHEAT_REQ>(Region);
                if (AuthLevel < 2)
                {
                    SendPacket(EventId.EGS_ADMIN_SHOW_ED_CHEAT_ACK, new KEGS_ADMIN_SHOW_ED_CHEAT_ACK { Ok = NetError.ERR_VERIFY_12 });
                    break;
                }
                var targetUnit = _server.Db.GetUnitByNickName(Region, req.NickName);
                SendPacket(EventId.EGS_ADMIN_SHOW_ED_CHEAT_ACK, new KEGS_ADMIN_SHOW_ED_CHEAT_ACK
                {
                    Ok = targetUnit != null ? NetError.NET_OK : NetError.ERR_SEARCH_UNIT_03,
                    NickName = req.NickName,
                    Ed = targetUnit?.Ed ?? 0,
                });
                break;
            }

            case EventId.EGS_ADMIN_ED_MONITORING_CHEAT_NOT:
            {
                var req = ev.GetPayload<KEGS_ADMIN_ED_MONITORING_CHEAT_NOT>(Region);
                if (AuthLevel < 2) break;
                IsEdMonitoringMode = req.EdMonitoringMode;
                _server.Db.WriteCheatLog(ActiveUnit?.UnitUid ?? 0, AccountUid, "/edmon", $"{req.EdMonitoringMode}:{req.MinEd}");
                break;
            }

            case EventId.EGS_ADMIN_CHANNELING_CODE_CHEAT_NOT:
            {
                var req = ev.GetPayload<KEGS_ADMIN_CHANNELING_CODE_CHEAT_NOT>(Region);
                if (AuthLevel < 3 || AccountUid <= 0) break;
                _server.Db.SetAccountChannelingCode(AccountUid, req.ChannelingCode);
                _server.Db.WriteCheatLog(ActiveUnit?.UnitUid ?? 0, AccountUid, "/channeling", req.ChannelingCode.ToString());
                break;
            }

            case EventId.EGS_ADMIN_BOSS_FIELD_GATE_OPEN_REQ:
            {
                var req = ev.GetPayload<KEGS_ADMIN_BOSS_FIELD_GATE_OPEN_REQ>(Region);
                if (AuthLevel < 2)
                {
                    SendPacket(EventId.EGS_ADMIN_BOSS_FIELD_GATE_OPEN_ACK, new KPacketOK { Ok = NetError.ERR_ADMIN_COMMAND_10 });
                    break;
                }
                int openErr = _server.Global.FieldBossManager.BossFieldOpenProcess(
                    req.BattleFieldId, DateTimeOffset.UtcNow.ToUnixTimeSeconds());
                SendPacket(EventId.EGS_ADMIN_BOSS_FIELD_GATE_OPEN_ACK, new KPacketOK { Ok = openErr });
                break;
            }

            case EventId.EGS_ADMIN_BOSS_FIELD_GATE_CLOSE_NOT:
            {
                if (AuthLevel < 2) break;
                _server.Global.FieldBossManager.ClosePortal();
                _server.Global.FieldBossManager.SetCoolTimeState(false);
                _server.Global.FieldBossManager.ClearTotalDangerousValue();
                break;
            }

            case EventId.EGS_ADMIN_GET_TOTAL_DANGEROUS_VALUE_REQ:
            {
                var req = ev.GetPayload<KEGS_ADMIN_GET_TOTAL_DANGEROUS_VALUE_REQ>(Region);
                if (AuthLevel < 2)
                {
                    SendPacket(EventId.EGS_ADMIN_GET_TOTAL_DANGEROUS_VALUE_ACK, new KEGS_ADMIN_GET_TOTAL_DANGEROUS_VALUE_ACK { Ok = NetError.ERR_VERIFY_12 });
                    break;
                }
                SendPacket(EventId.EGS_ADMIN_GET_TOTAL_DANGEROUS_VALUE_ACK, new KEGS_ADMIN_GET_TOTAL_DANGEROUS_VALUE_ACK
                {
                    Ok = NetError.NET_OK,
                    BattleFieldId = req.BattleFieldId,
                    TotalDangerousValue = _server.Global.FieldBossManager.GetTotalDangerousValue(req.BattleFieldId),
                });
                break;
            }

            case EventId.EGS_ADMIN_SET_TOTAL_DANGEROUS_VALUE_REQ:
            {
                var req = ev.GetPayload<KEGS_ADMIN_SET_TOTAL_DANGEROUS_VALUE_REQ>(Region);
                if (AuthLevel < 2)
                {
                    SendPacket(EventId.EGS_ADMIN_SET_TOTAL_DANGEROUS_VALUE_ACK, new KPacketOK { Ok = NetError.ERR_VERIFY_12 });
                    break;
                }
                byte newVal = (byte)Math.Clamp(
                    req.Set ? req.TotalDangerousValue : req.TotalDangerousValue + _server.Global.FieldBossManager.GetTotalDangerousValue(req.BattleFieldId),
                    0, 255);
                int setErr = _server.Global.FieldBossManager.TotalDangerousValueUpdateProcess(
                    req.BattleFieldId, newVal, DateTimeOffset.UtcNow.ToUnixTimeSeconds());
                SendPacket(EventId.EGS_ADMIN_SET_TOTAL_DANGEROUS_VALUE_ACK, new KPacketOK { Ok = setErr });
                break;
            }

            case EventId.EGS_ADMIN_BATTLE_FIELD_INCREASE_DANGER_REQ:
            {
                var req = ev.GetPayload<KEGS_ADMIN_BATTLE_FIELD_INCREASE_DANGER_REQ>(Region);
                if (AuthLevel < 3)
                {
                    SendPacket(EventId.EGS_ADMIN_BATTLE_FIELD_INCREASE_DANGER_ACK, new KEGS_ADMIN_BATTLE_FIELD_INCREASE_DANGER_ACK { Ok = NetError.ERR_VERIFY_12 });
                    break;
                }
                int fieldId = CurrentFieldMapId > 0 ? CurrentFieldMapId : 20001;
                _server.Global.FieldBossManager.UpdateTotalDangerousValue(fieldId, (byte)Math.Clamp(req.Danger, 0, 255));
                SendPacket(EventId.EGS_ADMIN_BATTLE_FIELD_INCREASE_DANGER_ACK, new KEGS_ADMIN_BATTLE_FIELD_INCREASE_DANGER_ACK
                {
                    Ok = NetError.NET_OK,
                    Danger = _server.Global.FieldBossManager.GetTotalDangerousValue(fieldId),
                });
                break;
            }

            case EventId.EGS_BATTLE_FIELD_DANGEROUS_VALUE_NOT:
            {
                var req = ev.GetPayload<KEGS_BATTLE_FIELD_DANGEROUS_VALUE_NOT>(Region);
                if (AuthLevel < 2) break;
                int fieldId = CurrentFieldMapId > 0 ? CurrentFieldMapId : 20001;
                _server.Global.FieldBossManager.SetTotalDangerousValue(fieldId, (byte)Math.Clamp(req.DangerousValue, 0, 255), isSet: true);
                break;
            }

            case EventId.EGS_SET_MULTIPLYER:
            {
                var req = ev.GetPayload<KEGS_SET_MULTIPLYER>(Region);
                if (AuthLevel < 3) break;
                _server.DamageMultiplier = req.M;
                break;
            }

            case EventId.EGS_UDP_KICK_GAMEEDIT_NOT:
            {
                var req = ev.GetPayload<KEGS_UDP_KICK_GAMEEDIT_NOT>(Region);
                if (AuthLevel < 3) break;
                _server.UdpSetKick = req.UdpSetKick;
                break;
            }

            case EventId.EGS_SEND_STATISTICS_DBLOG_REQ:
            {
                if (AuthLevel < 3) break;
                var stats = _server.StatisticsManager.FlushAndResetCounters();
                _server.Db.WriteCheatLog(ActiveUnit?.UnitUid ?? 0, AccountUid, "/statdblog", $"counters={stats.Count}");
                break;
            }

            case EventId.EGS_SEND_STATISTICS_LOCLOG_REQ:
            {
                if (AuthLevel < 3) break;
                var stats = _server.StatisticsManager.FlushAndResetCounters();
                _server.Db.WriteCheatLog(ActiveUnit?.UnitUid ?? 0, AccountUid, "/statloclog", $"counters={stats.Count}");
                break;
            }

            case EventId.EGS_QUEST_POINT_CHEAT_NOT:
            {
                var req = ev.GetPayload<KEGS_QUEST_POINT_CHEAT_NOT>(Region);
                if (AuthLevel < 3 || req.QuestPointCheat < 0) break;
                QuestPointCheat = Math.Min(200, req.QuestPointCheat);
                _server.Db.WriteCheatLog(ActiveUnit?.UnitUid ?? 0, AccountUid, "/questpoint", QuestPointCheat.ToString());
                break;
            }

            case EventId.EGS_EVENT_COBO_ITEM_GIVE_CHEAT_NOT:
            {
                if (AuthLevel < 3 || ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_EVENT_COBO_ITEM_GIVE_CHEAT_NOT>(Region);
                _coboItemGive = req.CoboEventItemGet;
                _server.Db.WriteCheatLog(ActiveUnit.UnitUid, AccountUid, "/coboitemgive", $"{req.CoboEventItemGet}:{req.NextDay}");
                if (req.NextDay)
                {
                    _coboItemGive = false;
                    var now = DateTime.UtcNow;
                    SendPacket(EventId.EGS_EVENT_COBO_DUNGEON_FIELD_NOT, new KEGS_EVENT_COBO_DUNGEON_FIELD_NOT
                    {
                        Ok = NetError.NET_OK,
                        StartUi = true,
                        DungeonCountUi = true,
                        FieldCountUi = false,
                        DungeonCount = 0,
                        FieldMonsterKillCount = 0,
                        RemaindTime = -1,
                        PushTimeUnix = new DateTimeOffset(now).ToUnixTimeSeconds(),
                    });
                }
                break;
            }

            case EventId.EGS_EVENT_VALENTINE_DUNGEON_GIVE_ITEM_CHEAT_NOT:
            {
                if (AuthLevel < 3 || Fsm.StateId is not (KGSFSM.S_FIELD_MAP or KGSFSM.S_ROOM)) break;
                var req = ev.GetPayload<KEGS_EVENT_VALENTINE_DUNGEON_GIVE_ITEM_CHEAT_NOT>(Region);
                _valentineItemCount = req.ValentineItemCount;
                SendPacket(EventId.EGS_EVENT_VALENTINE_DUNGEON_GIVE_ITEM_NOT,
                    new KEGS_EVENT_VALENTINE_DUNGEON_GIVE_ITEM_CHEAT_NOT { ValentineItemCount = _valentineItemCount });
                break;
            }

            // ================================================================================
            // Batch 2 (420 -> 510): 100% GSUser*.cpp Parity
            // (BattleField: 1, Inventory: 3, Party: 7, Pet: 1, GameCommon: 78)
            // ================================================================================

            // --- GSUserBattleField.cpp (1) ---
            case EventId.EGS_BATTLE_FIELD_NPC_P2P_SYNC_COMPLETE_NOT:
            {
                if (ActiveUnit == null || CurrentRoomUid <= 0) break;
                var room = _server.Center.RoomManager.GetRoom(CurrentRoomUid);
                if (room == null) break;
                _server.Center.RoomManager.SendBattleFieldSyncComplete(CurrentRoomUid, ActiveUnit.UnitUid);
                break;
            }

            // --- GSUserInventory.cpp (3) ---
            case EventId.EGS_CLOSE_BANK_NOT:
            {
                if (ActiveUnit == null) break;
                SendPacket(EventId.EGS_SHARE_BANK_UPDATE_OK_NOT, new KEGS_SHARE_BANK_UPDATE_OK_NOT { Ok = NetError.NET_OK });
                break;
            }

            case EventId.EGS_COUPON_ENTRY_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_COUPON_ENTRY_REQ>(Region);
                int itemId = req.CouponType switch
                {
                    1 => 60006512,
                    2 => 60006519,
                    3 => 60006513,
                    4 => 60006520,
                    _ => 0,
                };
                if (itemId == 0)
                {
                    SendPacket(EventId.EGS_COUPON_ENTRY_ACK, new KEGS_COUPON_ENTRY_ACK { Ok = NetError.ERR_DANO_EVENT_02 });
                    break;
                }
                var (ok, updatedSlots) = _server.Db.ConsumeItemsByItemId(
                    ActiveUnit.UnitUid,
                    new Dictionary<int, int> { [itemId] = 1 });
                if (!ok)
                {
                    SendPacket(EventId.EGS_COUPON_ENTRY_ACK, new KEGS_COUPON_ENTRY_ACK { Ok = NetError.ERR_DANO_EVENT_02 });
                    break;
                }
                _server.StatisticsManager.IncrementCounter($"CouponEntry_{itemId}");
                SendPacket(EventId.EGS_COUPON_ENTRY_ACK, new KEGS_COUPON_ENTRY_ACK
                {
                    Ok = NetError.NET_OK,
                    UpdatedInventorySlot = updatedSlots,
                });
                break;
            }

            case EventId.EGS_COUPON_ITEM_CHECK_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_COUPON_ITEM_CHECK_REQ>(Region);
                var ack = new KEGS_COUPON_ITEM_CHECK_ACK { Ok = NetError.NET_OK };
                bool isWeekend = DateTime.UtcNow.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;
                if (!isWeekend)
                {
                    if (req.ButtonType == 1) { ack.CouponType = 1; ack.ItemId = 60006512; }
                    else if (req.ButtonType == 2) { ack.CouponType = 2; ack.ItemId = 60006519; }
                    else ack.Ok = NetError.ERR_DANO_EVENT_02;
                }
                else
                {
                    if (req.ButtonType == 1) { ack.CouponType = 3; ack.ItemId = 60006513; }
                    else if (req.ButtonType == 2) { ack.CouponType = 4; ack.ItemId = 60006520; }
                    else ack.Ok = NetError.ERR_DANO_EVENT_02;
                }
                SendPacket(EventId.EGS_COUPON_ITEM_CHECK_ACK, ack);
                break;
            }

            // --- GSUserParty.cpp (7 legacy #ifndef SERV_BATTLE_FIELD_SYSTEM handlers) ---
            case EventId.EGS_CREATE_PARTY_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_CREATE_PARTY_REQ>(Region);
                if (CurrentPartyUid > 0)
                {
                    SendPacket(EventId.EGS_CREATE_PARTY_ACK, new KEGS_CREATE_PARTY_ACK { Ok = NetError.ERR_PARTY_12 });
                    break;
                }
                if (req.PartyInfo.PartyType is not (KPartyInfo.PtDungeonParty or KPartyInfo.PtPvpParty))
                {
                    SendPacket(EventId.EGS_CREATE_PARTY_ACK, new KEGS_CREATE_PARTY_ACK { Ok = NetError.ERR_PARTY_00 });
                    break;
                }
                var host = BuildPartyUserInfo();
                var party = _server.Center.PartyManager.CreateParty(req.PartyInfo, host);
                CurrentPartyUid = party.Info.PartyUid;
                SendPacket(EventId.EGS_CREATE_PARTY_ACK, new KEGS_CREATE_PARTY_ACK
                {
                    Ok = NetError.NET_OK,
                    PartyInfo = party.Info,
                    PartyUserInfo = host,
                });
                break;
            }

            case EventId.EGS_JOIN_PARTY_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_JOIN_PARTY_REQ>(Region);
                if (req.PartyUid <= 0)
                {
                    SendPacket(EventId.EGS_JOIN_PARTY_ACK, new KEGS_JOIN_PARTY_ACK { Ok = NetError.ERR_PARTY_17 });
                    break;
                }
                if (CurrentPartyUid != 0)
                {
                    SendPacket(EventId.EGS_JOIN_PARTY_ACK, new KEGS_JOIN_PARTY_ACK { Ok = NetError.ERR_PARTY_12 });
                    break;
                }
                if (_server.Global.MatchMakingManager.IsRegistered(ActiveUnit.UnitUid))
                {
                    SendPacket(EventId.EGS_JOIN_PARTY_ACK, new KEGS_JOIN_PARTY_ACK { Ok = NetError.ERR_MATCH_MAKING_18 });
                    break;
                }
                var party = _server.Center.PartyManager.GetParty(req.PartyUid);
                if (party == null)
                {
                    SendPacket(EventId.EGS_JOIN_PARTY_ACK, new KEGS_JOIN_PARTY_ACK { Ok = NetError.ERR_PARTY_17 });
                    break;
                }
                if (IsGuestUser && KUserDungeonManager.IsHenirDungeon(party.Info.DungeonId))
                {
                    SendPacket(EventId.EGS_JOIN_PARTY_ACK, new KEGS_JOIN_PARTY_ACK { Ok = NetError.ERR_GUEST_USER_00 });
                    break;
                }
                if (!SpiritManager.CheckingSpirit(
                    party.Info.DungeonId,
                    PcBangManager.IsPcBang && _server.GameSysValTable.IsSpiritFree(PcBangManager.PcBangType),
                    isHenirDungeon: KUserDungeonManager.IsHenirDungeon(party.Info.DungeonId),
                    isSpiritEvent: _server.GameEventManager.IsChinaSpiritEventEnabled))
                {
                    SendPacket(EventId.EGS_JOIN_PARTY_ACK, new KEGS_JOIN_PARTY_ACK { Ok = NetError.ERR_ROOM_27 });
                    break;
                }
                var joiner = BuildPartyUserInfo();
                if (!_server.Center.PartyManager.TryJoinParty(req.PartyUid, joiner, out var joinedParty) || joinedParty == null)
                {
                    SendPacket(EventId.EGS_JOIN_PARTY_ACK, new KEGS_JOIN_PARTY_ACK { Ok = NetError.ERR_PARTY_17 });
                    break;
                }
                CurrentPartyUid = joinedParty.Info.PartyUid;
                SendPacket(EventId.EGS_JOIN_PARTY_ACK, new KEGS_JOIN_PARTY_ACK { Ok = NetError.NET_OK });
                break;
            }

            case EventId.EGS_JOIN_PARTY_INVITE_REPLY_NOT:
            {
                if (ActiveUnit == null || CurrentPartyUid == 0) break;
                var req = ev.GetPayload<KEGS_JOIN_PARTY_INVITE_REPLY_NOT>(Region);
                if (req.ReplyReason == KEGS_JOIN_PARTY_INVITE_REPLY_NOT.RrAccept)
                {
                    var target = _server.GetOnlineUserByUnitUid(req.UnitUid);
                    if (target != null && target.ActiveUnit != null && target.CurrentPartyUid == 0)
                    {
                        var joiner = target.BuildPartyUserInfo();
                        if (_server.Center.PartyManager.TryJoinParty(CurrentPartyUid, joiner, out _))
                            target.CurrentPartyUid = CurrentPartyUid;
                    }
                }
                break;
            }

            case EventId.EGS_PARTY_CHANGE_NAME_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_PARTY_CHANGE_NAME_REQ>(Region);
                if (CurrentPartyUid == 0)
                {
                    SendPacket(EventId.EGS_PARTY_CHANGE_NAME_ACK, new KEGS_PARTY_CHANGE_NAME_ACK { Ok = NetError.ERR_PARTY_03 });
                    break;
                }
                bool ok = _server.Center.PartyManager.ChangePartyName(CurrentPartyUid, ActiveUnit.UnitUid, req.PartyName);
                SendPacket(EventId.EGS_PARTY_CHANGE_NAME_ACK, new KEGS_PARTY_CHANGE_NAME_ACK
                {
                    Ok = ok ? NetError.NET_OK : NetError.ERR_PARTY_03,
                });
                break;
            }

            case EventId.EGS_PARTY_CHANGE_PUBLIC_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_PARTY_CHANGE_PUBLIC_REQ>(Region);
                if (CurrentPartyUid == 0)
                {
                    SendPacket(EventId.EGS_PARTY_CHANGE_PUBLIC_ACK, new KEGS_PARTY_CHANGE_PUBLIC_ACK { Ok = NetError.ERR_PARTY_03 });
                    break;
                }
                bool ok = _server.Center.PartyManager.ChangePartyPublic(CurrentPartyUid, ActiveUnit.UnitUid, req.Public);
                SendPacket(EventId.EGS_PARTY_CHANGE_PUBLIC_ACK, new KEGS_PARTY_CHANGE_PUBLIC_ACK
                {
                    Ok = ok ? NetError.NET_OK : NetError.ERR_PARTY_03,
                });
                break;
            }

            case EventId.EGS_PARTY_QUICK_JOIN_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_PARTY_QUICK_JOIN_REQ>(Region);
                if (CurrentPartyUid != 0)
                {
                    SendPacket(EventId.EGS_PARTY_QUICK_JOIN_ACK, new KEGS_PARTY_QUICK_JOIN_ACK { Ok = NetError.ERR_PARTY_12 });
                    break;
                }
                if (!SpiritManager.CheckingSpirit(
                    req.DungeonId,
                    PcBangManager.IsPcBang && _server.GameSysValTable.IsSpiritFree(PcBangManager.PcBangType),
                    isHenirDungeon: KUserDungeonManager.IsHenirDungeon(req.DungeonId),
                    isSpiritEvent: _server.GameEventManager.IsChinaSpiritEventEnabled))
                {
                    SendPacket(EventId.EGS_PARTY_QUICK_JOIN_ACK, new KEGS_PARTY_QUICK_JOIN_ACK { Ok = NetError.ERR_ROOM_27 });
                    break;
                }
                var quickJoiner = BuildPartyUserInfo();
                var matched = _server.Center.PartyManager.QuickJoinParty(req.DungeonId, req.Difficulty, req.DungeonMode, quickJoiner);
                if (matched == null)
                {
                    SendPacket(EventId.EGS_PARTY_QUICK_JOIN_ACK, new KEGS_PARTY_QUICK_JOIN_ACK { Ok = NetError.ERR_PARTY_17 });
                    break;
                }
                CurrentPartyUid = matched.Info.PartyUid;
                SendPacket(EventId.EGS_PARTY_QUICK_JOIN_ACK, new KEGS_PARTY_QUICK_JOIN_ACK
                {
                    Ok = NetError.NET_OK,
                    PartyUid = matched.Info.PartyUid,
                });
                break;
            }

            case EventId.EGS_PARTY_TO_FIELD_MAP_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_PARTY_TO_FIELD_MAP_REQ>(Region);
                if (CurrentPartyUid == 0)
                {
                    SendPacket(EventId.EGS_PARTY_TO_FIELD_MAP_ACK, new KEGS_PARTY_TO_FIELD_MAP_ACK { Ok = NetError.ERR_PARTY_03 });
                    break;
                }
                if (CurrentRoomUid > 0)
                {
                    _server.Center.RoomManager.LeaveRoom(CurrentRoomUid, ActiveUnit.UnitUid);
                    CurrentRoomUid = 0;
                }
                Fsm.TryStateTransition(KGSFSM.I_TO_FIELD_MAP, out _);
                SendPacket(EventId.EGS_PARTY_TO_FIELD_MAP_ACK, new KEGS_PARTY_TO_FIELD_MAP_ACK
                {
                    Ok = NetError.NET_OK,
                    PartyToFieldType = req.PartyToFieldType,
                    FieldStartPos = req.FieldStartPos,
                });
                break;
            }

            // --- GSUserPet.cpp (1) ---
            case EventId.EGS_USE_INTIMACY_UP_ITEM_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_USE_INTIMACY_UP_ITEM_REQ>(Region);
                var meta = _server.Db.GetInventoryItemMeta(ActiveUnit.UnitUid, req.ItemUid);
                if (!meta.found)
                {
                    SendPacket(EventId.EGS_USE_INTIMACY_UP_ITEM_ACK, new KEGS_USE_INTIMACY_UP_ITEM_ACK { Ok = NetError.ERR_ITEM_04 });
                    break;
                }
                var (ok, err, upPct, pet) = PetManager.UseIntimacyUpItem();
                if (!ok || pet == null)
                {
                    SendPacket(EventId.EGS_USE_INTIMACY_UP_ITEM_ACK, new KEGS_USE_INTIMACY_UP_ITEM_ACK { Ok = err });
                    break;
                }
                var (delOk, updatedSlot) = _server.Db.DeleteInventoryItem(ActiveUnit.UnitUid, req.ItemUid, 1);
                if (!delOk)
                {
                    SendPacket(EventId.EGS_USE_INTIMACY_UP_ITEM_ACK, new KEGS_USE_INTIMACY_UP_ITEM_ACK { Ok = NetError.ERR_ITEM_04 });
                    break;
                }
                _server.Db.UpdatePetStats(pet.PetUid, pet.Intimacy, pet.Satiety, pet.EvolutionStep);
                SendPacket(EventId.EGS_USE_INTIMACY_UP_ITEM_ACK, new KEGS_USE_INTIMACY_UP_ITEM_ACK
                {
                    Ok = NetError.NET_OK,
                    InventorySlotInfo = [updatedSlot],
                    UpPercent = upPct,
                });
                if (CurrentRoomUid > 0)
                {
                    var notPkt = new KEGS_CHANGED_PET_INTIMACY_NOT
                    {
                        UnitUid = ActiveUnit.UnitUid,
                        PetUid = pet.PetUid,
                        CurrentIntimacy = pet.Intimacy,
                    };
                    var room = _server.Center.RoomManager.GetRoom(CurrentRoomUid);
                    if (room != null)
                    {
                        foreach (long uid in room.GetMemberUnitUids())
                            _server.SendToUnit(uid, EventId.EGS_CHANGED_PET_INTIMACY_NOT, notPkt);
                    }
                }
                break;
            }

            // --- GSUserGameCommon.cpp (78) ---
            case EventId.EGS_2013_EVENT_MISSION_COMPLETE_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_2013_EVENT_MISSION_COMPLETE_REQ>(Region);
                if (!_server.GameEventManager.IsOldYearEvent2013Enabled)
                {
                    SendPacket(EventId.EGS_2013_EVENT_MISSION_COMPLETE_ACK, new KEGS_2013_EVENT_MISSION_COMPLETE_ACK { Ok = NetError.ERR_QUEST_09 });
                    break;
                }
                int rewardedLevel = -1;
                if (req.LevelUpEvent)
                {
                    if (ActiveUnit.Level > _oldYearMissionRewardedLevel && ActiveUnit.Level >= 26 && ActiveUnit.UnitClass >= 10)
                    {
                        rewardedLevel = ActiveUnit.Level == 65 && _oldYearMissionRewardedLevel != ActiveUnit.Level - 1
                            ? ActiveUnit.Level - 1
                            : ActiveUnit.Level;
                    }
                }
                else
                {
                    rewardedLevel = 1;
                }
                if (rewardedLevel < 0)
                {
                    SendPacket(EventId.EGS_2013_EVENT_MISSION_COMPLETE_ACK, new KEGS_2013_EVENT_MISSION_COMPLETE_ACK { Ok = NetError.ERR_QUEST_09 });
                    break;
                }
                _oldYearMissionRewardedLevel = Math.Max(_oldYearMissionRewardedLevel, rewardedLevel);
                var granted = _server.Db.GrantUnitItem(ActiveUnit.UnitUid, 130048, 1);
                _server.Db.SaveUserEventState(ActiveUnit.UnitUid, _oldYearMissionRewardedLevel, _newYearMissionStepId,
                    _checkPowerTime, _checkPowerCount, _checkPowerScore, _noteViewCount, _chinaSpirit);
                SendPacket(EventId.EGS_2013_EVENT_MISSION_COMPLETE_ACK, new KEGS_2013_EVENT_MISSION_COMPLETE_ACK
                {
                    Ok = NetError.NET_OK,
                    RewardedLevel = rewardedLevel,
                    InventorySlotInfo = [granted],
                });
                break;
            }

            case EventId.EGS_2014_EVENT_MISSION_COMPLETE_REQ:
            {
                if (ActiveUnit == null) break;
                if (!_server.GameEventManager.IsNewYearEvent2014Enabled)
                {
                    SendPacket(EventId.EGS_2014_EVENT_MISSION_COMPLETE_ACK, new KEGS_2014_EVENT_MISSION_COMPLETE_ACK { Ok = NetError.ERR_QUEST_09 });
                    break;
                }
                bool completed = _newYearMissionStepId switch
                {
                    0 or 2 or 4 or 6 or 8 => true,
                    1 => ActiveUnit.UnitClass >= 10,
                    3 => ActiveUnit.Level >= 20,
                    5 => ActiveUnit.Level >= 30,
                    7 => ActiveUnit.Level >= 35,
                    9 => ActiveUnit.Level >= 45,
                    _ => false,
                };
                if (!completed)
                {
                    SendPacket(EventId.EGS_2014_EVENT_MISSION_COMPLETE_ACK, new KEGS_2014_EVENT_MISSION_COMPLETE_ACK { Ok = NetError.ERR_QUEST_09 });
                    break;
                }
                _newYearMissionStepId++;
                _server.Db.SaveUserEventState(ActiveUnit.UnitUid, _oldYearMissionRewardedLevel, _newYearMissionStepId,
                    _checkPowerTime, _checkPowerCount, _checkPowerScore, _noteViewCount, _chinaSpirit);
                _server.Db.SendLetter(Region, ActiveUnit.UnitUid, "System", ActiveUnit.UnitUid,
                    "2014 New Year Event", "Mission reward", 130048, 1, 0);
                SendPacket(EventId.EGS_2014_EVENT_MISSION_COMPLETE_ACK, new KEGS_2014_EVENT_MISSION_COMPLETE_ACK { Ok = NetError.NET_OK });
                break;
            }

            case EventId.EGS_4TH_ANNIV_EVENT_REWARD_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_4TH_ANNIV_EVENT_REWARD_REQ>(Region);
                if (req.SelectedIndex < 0 || req.SelectedIndex > 11)
                {
                    SendPacket(EventId.EGS_4TH_ANNIV_EVENT_REWARD_ACK, new KEGS_4TH_ANNIV_EVENT_REWARD_ACK
                    {
                        Ok = NetError.ERR_4TH_ANNIV_EVENT_02,
                        SelectedIndex = -1,
                    });
                    break;
                }
                var now = DateTime.UtcNow;
                if (_lastAnnivRewardUtc.Date == now.Date || _anniv4RewardFlags[req.SelectedIndex])
                {
                    SendPacket(EventId.EGS_4TH_ANNIV_EVENT_REWARD_ACK, new KEGS_4TH_ANNIV_EVENT_REWARD_ACK
                    {
                        Ok = NetError.ERR_4TH_ANNIV_EVENT_01,
                        SelectedIndex = -1,
                    });
                    break;
                }
                _anniv4RewardFlags[req.SelectedIndex] = true;
                _lastAnnivRewardUtc = now;
                _server.Db.SendLetter(Region, ActiveUnit.UnitUid, "System", ActiveUnit.UnitUid,
                    "4th Anniversary Reward", "Event reward", 60000 + req.SelectedIndex, 1, 0);
                SendPacket(EventId.EGS_4TH_ANNIV_EVENT_REWARD_ACK, new KEGS_4TH_ANNIV_EVENT_REWARD_ACK
                {
                    Ok = NetError.NET_OK,
                    SelectedIndex = req.SelectedIndex,
                });
                break;
            }

            case EventId.EGS_ALL_COMPLETED_QUEST_COMPLETE_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_ALL_COMPLETED_QUEST_COMPLETE_REQ>(Region);
                var ack = new KEGS_ALL_COMPLETED_QUEST_COMPLETE_ACK { Ok = NetError.NET_OK };
                foreach (var qReq in req.QuestCompleteReq)
                {
                    var (canComplete, rewardEd, rewardExp) = QuestManager.CompleteQuest(qReq.QuestId);
                    bool ok = canComplete && _server.Db.CompleteQuest(ActiveUnit.UnitUid, qReq.QuestId);
                    if (ok)
                    {
                        EdManager.TryAddEd(rewardEd, KUserEDManager.EdReason.ErInEdQuestReward, out _);
                        _server.Db.UpdateUnitProgress(ActiveUnit.UnitUid, addExp: rewardExp, addEd: rewardEd, mapId: 0);
                        _server.StatisticsManager.IncrementCounter("QuestCompleted");
                    }
                    ack.QuestCompleteAck.Add(new KEGS_QUEST_COMPLETE_ACK
                    {
                        Ok = ok ? NetError.NET_OK : NetError.ERR_QUEST_02,
                        CompleteQuestInfo = new KCompleteQuestInfo
                        {
                            QuestId = qReq.QuestId,
                            CompleteCount = ok ? 1 : 0,
                            CompleteDate = ok ? DateTimeOffset.UtcNow.ToUnixTimeSeconds() : 0,
                        },
                        UpdateUnitInfo = new KEGS_UPDATE_UNIT_INFO_NOT
                        {
                            UnitInfo = ActiveUnit ?? new KUnitInfo(),
                        },
                    });
                }
                ActiveUnit = _server.Db.GetUnitByUid(Region, ActiveUnit.UnitUid);
                SendPacket(EventId.EGS_ALL_COMPLETED_QUEST_COMPLETE_ACK, ack);
                break;
            }

            case EventId.EGS_ATTENDANCE_CHECK_REQ:
            {
                if (ActiveUnit == null) break;
                bool ok = _server.Db.CheckAttendanceToday(ActiveUnit.UnitUid);
                SendPacket(EventId.EGS_ATTENDANCE_CHECK_ACK, new KEGS_ATTENDANCE_CHECK_ACK
                {
                    Ok = ok ? NetError.NET_OK : NetError.ERR_WEB_POINT_EVENT_00,
                });
                break;
            }

            case EventId.EGS_BUY_UI_SETTING_REQ:
            {
                var req = ev.GetPayload<KEGS_BUY_UI_SETTING_REQ>(Region);
                if (_timeControlItemCheckDungeonPlay || InventoryLocker.IsLocked())
                {
                    SendPacket(EventId.EGS_BUY_UI_SETTING_ACK, new KEGS_BUY_UI_SETTING_ACK { Ok = NetError.ERR_INVENTORY_LOCK_00 });
                    break;
                }
                var banSet = new HashSet<int>();
                var now = DateTime.UtcNow;
                if (_server.GameEventManager.GetTimeControlItemsSnapshot().TryGetValue(req.TimeControlItemType, out var items))
                {
                    foreach (var it in items)
                    {
                        if (DateTime.TryParse(it.StartTime, out var st) && DateTime.TryParse(it.EndTime, out var et) &&
                            now >= st && now <= et)
                        {
                            banSet.Add(it.ItemId);
                        }
                    }
                }
                SendPacket(EventId.EGS_BUY_UI_SETTING_ACK, new KEGS_BUY_UI_SETTING_ACK
                {
                    Ok = NetError.NET_OK,
                    GetItemOnOff = banSet,
                    HouseId = req.HouseId,
                    TimeControlItemType = req.TimeControlItemType,
                });
                break;
            }

            case EventId.EGS_CATCH_HACKUSER_INFO_NOT:
            {
                var req = ev.GetPayload<KEGS_CATCH_HACKUSER_INFO_NOT>(Region);
                _server.Db.RecordHackOrPortOrKomLog(req.UserUid, req.UnitUid, "CatchHackUser", req.CrashType);
                break;
            }

            case EventId.EGS_CHANGE_NICK_NAME_REQ:
            {
                var req = ev.GetPayload<KEGS_CHANGE_NICK_NAME_REQ>(Region);
                int err = ValidateNickNameForUnit(req.NickName, NetError.ERR_RESTORE_NICK_NAME_06, NetError.ERR_RESTORE_NICK_NAME_05);
                if (err == NetError.NET_OK)
                    err = _server.Db.ChangeUnitNickName(Region, AccountUid, req.UnitUid, req.NickName, req.CheckOnly);
                if (err == NetError.NET_OK && !req.CheckOnly && ActiveUnit != null && ActiveUnit.UnitUid == req.UnitUid)
                    ActiveUnit.NickName = req.NickName;
                SendPacket(EventId.EGS_CHANGE_NICK_NAME_ACK, new KEGS_CHANGE_NICK_NAME_ACK
                {
                    Ok = err,
                    UnitUid = req.UnitUid,
                    NickName = req.NickName,
                    CheckOnly = req.CheckOnly,
                });
                break;
            }

            case EventId.EGS_CHANGE_OPTION_PLAY_GUIDE_REQ:
            {
                var req = ev.GetPayload<KEGS_CHANGE_OPTION_PLAY_GUIDE_REQ>(Region);
                _server.Db.SetPlayGuideOption(AccountUid, req.On);
                SendPacket(EventId.EGS_CHANGE_OPTION_PLAY_GUIDE_ACK, new KEGS_CHANGE_OPTION_PLAY_GUIDE_ACK { Ok = NetError.NET_OK });
                break;
            }

            case EventId.EGS_CHANGE_OPTION_PLAY_GUIDE_ACK:
            {
                var ack = ev.GetPayload<KEGS_CHANGE_OPTION_PLAY_GUIDE_ACK>(Region);
                SendPacket(EventId.EGS_CHANGE_OPTION_PLAY_GUIDE_ACK, ack);
                break;
            }

            case EventId.EGS_CHARACTER_LIST_REQ:
            {
                var units = _server.Db.GetUnitsForAccount(Region, AccountUid);
                var ack = new KEGS_CHARACTER_LIST_ACK
                {
                    Ok = NetError.NET_OK,
                    UserId = UserId,
                    ServerGroupUnitSlot = new Dictionary<int, int> { [_server.ServerGroupId] = Math.Max(4, units.Count) },
                    ServerGroupUnitInfo = new Dictionary<int, List<KUnitInfo>> { [_server.ServerGroupId] = units },
                };
                SendPacket(EventId.EGS_CHARACTER_LIST_ACK, ack);
                break;
            }

            case EventId.EGS_CHARACTER_LIST_1ST_ACK:
            {
                var ack = ev.GetPayload<KEGS_CHARACTER_LIST_1ST_ACK>(Region);
                SendPacket(EventId.EGS_CHARACTER_LIST_ACK, new KEGS_CHARACTER_LIST_ACK
                {
                    Ok = ack.Ok,
                    ServerGroupUnitSlot = ack.ServerGroupUnitSlot,
                    ServerGroupUnitInfo = ack.ServerGroupUnitInfo,
                    UserId = ack.UserId,
                });
                break;
            }

            case EventId.EGS_CHARACTER_LIST_ACK:
            {
                var ack = ev.GetPayload<KEGS_CHARACTER_LIST_ACK>(Region);
                SendPacket(EventId.EGS_CHARACTER_LIST_ACK, ack);
                break;
            }

            case EventId.EGS_CHAT_OPTION_INFO_WRITE_REQ:
            {
                var req = ev.GetPayload<KEGS_CHAT_OPTION_INFO_WRITE_REQ>(Region);
                _server.Db.SetChatOption(AccountUid, req.Index, req.Value);
                SendPacket(EventId.EGS_CHAT_OPTION_INFO_WRITE_ACK, new KEGS_CHAT_OPTION_INFO_WRITE_ACK { Ok = NetError.NET_OK });
                break;
            }

            case EventId.EGS_CHECK_TIME_EVENT_COMPLETE_REQ:
            {
                if (ActiveUnit == null) break;
                int rewardItemId = 0;

                if (_pendingTimeEventRewardItemIds.Count > 0)
                {
                    rewardItemId = _pendingTimeEventRewardItemIds.Dequeue();
                }
                else
                {
                    foreach (var kv in _server.GameEventManager.EventRewards)
                    {
                        var evtReward = kv.Value;
                        if (evtReward.RewardId <= 0 || evtReward.EventTimeMinutes <= 0) continue;
                        if (evtReward.MinLevel > 0 && ActiveUnit.Level < evtReward.MinLevel) continue;
                        if (evtReward.MaxLevel > 0 && ActiveUnit.Level > evtReward.MaxLevel) continue;
                        if (evtReward.GiveOnce && _claimedEventRewardKeys.Contains(kv.Key)) continue;
                        if (_sessionConnectedSeconds >= evtReward.EventTimeMinutes * 60.0)
                        {
                            rewardItemId = evtReward.RewardId;
                            _claimedEventRewardKeys.Add(kv.Key);
                            break;
                        }
                    }
                }

                if (rewardItemId == 0 && _hasDirectTimeEventReward)
                {
                    _hasDirectTimeEventReward = false;
                    rewardItemId = 130048;
                    foreach (var evtReward in _server.GameEventManager.EventRewards.Values)
                    {
                        if (evtReward.RewardId > 0 &&
                            (evtReward.MinLevel <= 0 || ActiveUnit.Level >= evtReward.MinLevel) &&
                            (evtReward.MaxLevel <= 0 || ActiveUnit.Level <= evtReward.MaxLevel))
                        {
                            rewardItemId = evtReward.RewardId;
                            break;
                        }
                    }
                }

                if (rewardItemId <= 0)
                {
                    SendPacket(EventId.EGS_CHECK_TIME_EVENT_COMPLETE_ACK, new KEGS_CHECK_TIME_EVENT_COMPLETE_ACK
                    {
                        Ok = NetError.ERR_CUMULATIVE_TIME_EVENT_00,
                    });
                    break;
                }

                var granted = _server.Db.GrantUnitItem(ActiveUnit.UnitUid, rewardItemId, 1);
                SendPacket(EventId.EGS_CHECK_TIME_EVENT_COMPLETE_ACK, new KEGS_CHECK_TIME_EVENT_COMPLETE_ACK
                {
                    Ok = NetError.NET_OK,
                    RewardCount = 1,
                    InventorySlotInfo = [granted],
                });
                break;
            }

            case EventId.EGS_CLIENT_POPRT_CHANGE_REQUEST_INFO_NOT:
            {
                var req = ev.GetPayload<KEGS_CLIENT_POPRT_CHANGE_REQUEST_INFO_NOT>(Region);
                _server.Db.RecordHackOrPortOrKomLog(AccountUid, ActiveUnit?.UnitUid ?? 0, "UdpPortChange", req.UdpPortSuccessType, req.UseUdpPort);
                break;
            }

            case EventId.EGS_CREATE_NEW_UNIT_REQ:
            {
                if (AccountUid <= 0) break;
                var req = ev.GetPayload<KEGS_CREATE_NEW_UNIT_REQ>(Region);
                if (req.Class <= 0 || req.Class >= 100)
                {
                    SendPacket(EventId.EGS_CREATE_UNIT_ACK, new KEGS_CREATE_UNIT_ACK { Ok = NetError.ERR_CREATE_UNIT_03 });
                    break;
                }
                int nickErr = ValidateNickNameForUnit(req.NickName, NetError.ERR_CREATE_UNIT_04, NetError.ERR_CREATE_UNIT_08);
                if (nickErr != NetError.NET_OK)
                {
                    SendPacket(EventId.EGS_CREATE_UNIT_ACK, new KEGS_CREATE_UNIT_ACK { Ok = nickErr });
                    break;
                }
                var (ok, unit, err) = _server.Db.CreateUnitInServerGroup(Region, AccountUid, req.NickName, req.Class, req.ServerGroup);
                SendPacket(EventId.EGS_CREATE_UNIT_ACK, new KEGS_CREATE_UNIT_ACK
                {
                    Ok = ok ? NetError.NET_OK : err,
                    UnitInfo = unit ?? new KUnitInfo(),
                });
                break;
            }

            case EventId.EGS_CURRENT_TIME_REQ:
            {
                var now = DateTime.UtcNow;
                SendPacket(EventId.EGS_CURRENT_TIME_ACK, new KEGS_CURRENT_TIME_ACK
                {
                    CurrentTime = now.ToString("yyyy-MM-dd HH:mm:ss"),
                    CurrentTimeUnix = new DateTimeOffset(now).ToUnixTimeSeconds(),
                });
                break;
            }

            case EventId.EGS_DECIDE_TO_USE_THIS_SKILL_PAGE_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_DECIDE_TO_USE_THIS_SKILL_PAGE_REQ>(Region);
                var (ok, pageData) = SkillTree.SelectSkillPage(req.SkillPagesNumberDecidedToUse, ActiveUnit.Level);
                if (!ok)
                {
                    SendPacket(EventId.EGS_DECIDE_TO_USE_THIS_SKILL_PAGE_ACK, new KEGS_DECIDE_TO_USE_THIS_SKILL_PAGE_ACK
                    {
                        Ok = NetError.ERR_SKILL_PAGE_03,
                    });
                    break;
                }
                _server.Db.SaveUnitSkillPageState(ActiveUnit.UnitUid, SkillTree.TheNumberOfSkillPagesAvailable,
                    SkillTree.ActiveSkillPage, SkillTree.GetSkillNotePagesSnapshot());
                SendPacket(EventId.EGS_DECIDE_TO_USE_THIS_SKILL_PAGE_ACK, new KEGS_DECIDE_TO_USE_THIS_SKILL_PAGE_ACK
                {
                    Ok = NetError.NET_OK,
                    SkillPagesNumberDecidedToUse = SkillTree.ActiveSkillPage,
                    SPointAvailable = SkillTree.AvailableSkillPoints,
                    CsPointAvailable = SkillTree.AvailableCashSkillPoints,
                    UserSkillPageData = pageData,
                });
                break;
            }

            case EventId.EGS_DEL_TUTORIAL_REQ:
            {
                var req = ev.GetPayload<KEGS_DEL_TUTORIAL_REQ>(Region);
                bool removed = _tutorialStudents.RemoveAll(s => s.UnitUid == req.UnitUid) > 0;
                SendPacket(EventId.EGS_DEL_TUTORIAL_ACK, new KEGS_DEL_TUTORIAL_ACK
                {
                    Ok = removed ? NetError.NET_OK : NetError.ERR_TUTORIAL_01,
                });
                if (removed && ActiveUnit != null)
                {
                    _server.SendToUnit(req.UnitUid, EventId.EGS_DEL_TUTORIAL_NOT, new KEGS_DEL_TUTORIAL_NOT
                    {
                        UnitUid = ActiveUnit.UnitUid,
                        Reason = NetError.NET_OK,
                    });
                }
                break;
            }

            case EventId.EGS_ENTER_EQUIP_SHOP_REQ:
            {
                if (ActiveUnit == null || Fsm.StateId != KGSFSM.S_FIELD_MAP) break;
                var req = ev.GetPayload<KEGS_ENTER_NPC_HOUSE_REQ>(Region);
                SendPacket(EventId.EGS_ENTER_EQUIP_SHOP_ACK, new KEGS_ENTER_EQUIP_SHOP_ACK
                {
                    Ok = req.NpcId > 0 ? NetError.NET_OK : NetError.ERR_UNKNOWN,
                });
                break;
            }

            case EventId.EGS_ENTER_NPC_HOUSE_REQ:
            {
                if (ActiveUnit == null || Fsm.StateId != KGSFSM.S_FIELD_MAP) break;
                var req = ev.GetPayload<KEGS_ENTER_NPC_HOUSE_REQ>(Region);
                SendPacket(EventId.EGS_ENTER_NPC_HOUSE_ACK, new KEGS_ENTER_NPC_HOUSE_ACK
                {
                    Ok = req.NpcId > 0 ? NetError.NET_OK : NetError.ERR_UNKNOWN,
                });
                break;
            }

            case EventId.EGS_ENTRY_POINT_CHANGE_NICK_NAME_REQ:
            {
                var req = ev.GetPayload<KEGS_ENTRY_POINT_CHANGE_NICK_NAME_REQ>(Region);
                int err = ValidateNickNameForUnit(req.NickName, NetError.ERR_RESTORE_NICK_NAME_06, NetError.ERR_RESTORE_NICK_NAME_05);
                if (err == NetError.NET_OK)
                    err = _server.Db.ChangeUnitNickName(Region, AccountUid, req.UnitUid, req.NickName, req.CheckOnly);
                SendPacket(EventId.EGS_CHANGE_NICK_NAME_ACK, new KEGS_CHANGE_NICK_NAME_ACK
                {
                    Ok = err,
                    UnitUid = req.UnitUid,
                    NickName = req.NickName,
                    CheckOnly = req.CheckOnly,
                });
                break;
            }

            case EventId.EGS_ENTRY_POINT_CHECK_NICK_NAME_REQ:
            {
                var req = ev.GetPayload<KEGS_ENTRY_POINT_CHECK_NICK_NAME_REQ>(Region);
                int err = ValidateNickNameForUnit(req.NickName, NetError.ERR_RESTORE_NICK_NAME_06, NetError.ERR_RESTORE_NICK_NAME_05);
                if (err == NetError.NET_OK)
                    err = _server.Db.FindUnitUidByNickName(Region, req.NickName) != null ? NetError.ERR_CREATE_UNIT_01 : NetError.NET_OK;
                SendPacket(EventId.EGS_ENTRY_POINT_CHECK_NICK_NAME_ACK, new KEGS_ENTRY_POINT_CHECK_NICK_NAME_ACK
                {
                    Ok = err,
                    NickName = req.NickName,
                });
                break;
            }

            case EventId.EGS_ENTRY_POINT_CHECK_NICK_NAME_ACK:
            {
                var ack = ev.GetPayload<KEGS_ENTRY_POINT_CHECK_NICK_NAME_ACK>(Region);
                SendPacket(EventId.EGS_ENTRY_POINT_CHECK_NICK_NAME_ACK, ack);
                break;
            }

            case EventId.EGS_ENTRY_POINT_DELETE_UNIT_REQ:
            {
                var req = ev.GetPayload<KEGS_ENTRY_POINT_DELETE_UNIT_REQ>(Region);
                if (_server.Db.GetSecondSecurityStatus(AccountUid).useSecondPw && !SecondSecurityVerified)
                {
                    SendPacket(EventId.EGS_DELETE_UNIT_ACK, new KEGS_DELETE_UNIT_ACK { Ok = NetError.ERR_VERIFY_21, UnitUid = req.UnitUid });
                    break;
                }
                if (RelationshipManager.CurrentType != KUserRelationshipManager.RelationshipType.RtSolo)
                {
                    SendPacket(EventId.EGS_DELETE_UNIT_ACK, new KEGS_DELETE_UNIT_ACK { Ok = NetError.ERR_DELETE_UNIT_06, UnitUid = req.UnitUid });
                    break;
                }
                bool deleted = _server.Db.DeleteUnit(Region, AccountUid, req.UnitUid);
                SendPacket(EventId.EGS_DELETE_UNIT_ACK, new KEGS_DELETE_UNIT_ACK
                {
                    Ok = deleted ? NetError.NET_OK : NetError.ERR_DELETE_UNIT_00,
                    UnitUid = req.UnitUid,
                });
                break;
            }

            case EventId.EGS_EVENT_CHUNG_GIVE_ITEM_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_EVENT_CHUNG_GIVE_ITEM_REQ>(Region);
                string timeStr = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                _server.Db.GrantUnitItem(ActiveUnit.UnitUid, 130001 + Math.Max(0, req.Choice), req.TwoGiveItem ? 2 : 1);
                SendPacket(EventId.EGS_EVENT_CHUNG_GIVE_ITEM_ACK, new KEGS_EVENT_CHUNG_GIVE_ITEM_ACK
                {
                    Ok = NetError.NET_OK,
                    GetItemTime = timeStr,
                });
                break;
            }

            case EventId.EGS_EVENT_COBO_DUNGEON_FIELD_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_EVENT_COBO_DUNGEON_FIELD_REQ>(Region);
                var now = DateTime.UtcNow;
                if (ActiveUnit.Level < 10 || (now.Hour == 23 && now.Minute >= 30))
                {
                    SendPacket(EventId.EGS_EVENT_COBO_DUNGEON_FIELD_ACK, new KEGS_EVENT_COBO_DUNGEON_FIELD_ACK
                    {
                        Ok = NetError.ERR_CLIENT_STATE,
                    });
                    break;
                }
                _coboItemGive = false;
                SendPacket(EventId.EGS_EVENT_COBO_DUNGEON_FIELD_ACK, new KEGS_EVENT_COBO_DUNGEON_FIELD_ACK
                {
                    Ok = NetError.NET_OK,
                    StartUi = false,
                    DungeonCountUi = true,
                    FieldCountUi = false,
                    RemaindTime = 1800,
                    PushTime = now.ToString("yyyy-MM-dd HH:mm:ss"),
                    PushTimeUnix = new DateTimeOffset(now).ToUnixTimeSeconds(),
                });
                break;
            }

            case EventId.EGS_EVENT_COBO_DUNGEON_FIELD_ACK:
            {
                var ack = ev.GetPayload<KEGS_EVENT_COBO_DUNGEON_FIELD_ACK>(Region);
                ack.RemaindTime = ack.RemaindTime < 0 ? 1800 : ack.RemaindTime;
                SendPacket(EventId.EGS_EVENT_COBO_DUNGEON_FIELD_ACK, ack);
                break;
            }

            case EventId.EGS_EVENT_NOTE_VIEW_REQ:
            {
                if (ActiveUnit == null) break;
                if (_noteViewCount > 0)
                {
                    _noteViewCount--;
                    int titleId = 5730 + (Random.Shared.Next(10) * 10);
                    TitleManager.UnlockTitle(_server.Db, ActiveUnit.UnitUid, titleId);
                    _server.Db.SaveUserEventState(ActiveUnit.UnitUid, _oldYearMissionRewardedLevel, _newYearMissionStepId,
                        _checkPowerTime, _checkPowerCount, _checkPowerScore, _noteViewCount, _chinaSpirit);
                    SendPacket(EventId.EGS_EVENT_NOTE_VIEW_ACK, new KEGS_EVENT_NOTE_VIEW_ACK
                    {
                        Ok = NetError.NET_OK,
                        TitleId = titleId,
                        NoteViewCount = _noteViewCount,
                    });
                }
                else
                {
                    SendPacket(EventId.EGS_EVENT_NOTE_VIEW_ACK, new KEGS_EVENT_NOTE_VIEW_ACK
                    {
                        Ok = NetError.ERR_CLIENT_STATE,
                    });
                }
                break;
            }

            case EventId.EGS_EXPAND_SKILL_PAGE_REQ:
            {
                if (ActiveUnit == null) break;
                if (!SkillTree.CanExpandSkillPage())
                {
                    SendPacket(EventId.EGS_EXPAND_SKILL_PAGE_ACK, new KEGS_EXPAND_SKILL_PAGE_ACK { Ok = NetError.ERR_SKILL_PAGE_02 });
                    break;
                }
                if (!EdManager.TryConsumeEd(KUserSkillTree.EdNeededToExpandSecondSkillPage, KUserEDManager.EdReason.ErOutEdExpandSkillPage, out int newEd))
                {
                    SendPacket(EventId.EGS_EXPAND_SKILL_PAGE_ACK, new KEGS_EXPAND_SKILL_PAGE_ACK { Ok = NetError.ERR_EXPAND_INVENTORY_ED_01 });
                    break;
                }
                var (ok, pageCount, pageData) = SkillTree.ExpandSkillPage(ActiveUnit.Level, ActiveUnit.UnitClass);
                if (!ok)
                {
                    SendPacket(EventId.EGS_EXPAND_SKILL_PAGE_ACK, new KEGS_EXPAND_SKILL_PAGE_ACK { Ok = NetError.ERR_SKILL_PAGE_02 });
                    break;
                }
                _server.Db.UpdateUnitProgress(ActiveUnit.UnitUid, addExp: 0, addEd: -KUserSkillTree.EdNeededToExpandSecondSkillPage, mapId: 0);
                ActiveUnit = _server.Db.GetUnitByUid(Region, ActiveUnit.UnitUid);
                _server.Db.SaveUnitSkillPageState(ActiveUnit!.UnitUid, pageCount, SkillTree.ActiveSkillPage, SkillTree.GetSkillNotePagesSnapshot());
                SendPacket(EventId.EGS_EXPAND_SKILL_PAGE_ACK, new KEGS_EXPAND_SKILL_PAGE_ACK
                {
                    Ok = NetError.NET_OK,
                    Ed = newEd,
                    TheNumberOfSkillPagesAvailable = pageCount,
                    SPointAvailable = pageData.SkillPoint,
                    CsPointAvailable = pageData.CashSkillPoint,
                    SkillListLearned = pageData.UserSkillData.Select(s => (int)s.SkillId).ToList(),
                    PayWithEd = true,
                });
                break;
            }

            case EventId.EGS_FINAL_DELETE_UNIT_REQ:
            {
                if (AccountUid <= 0) break;
                var req = ev.GetPayload<KEGS_FINAL_DELETE_UNIT_REQ>(Region);
                if (_server.Db.GetSecondSecurityStatus(AccountUid).useSecondPw && !SecondSecurityVerified)
                {
                    SendPacket(EventId.EGS_FINAL_DELETE_UNIT_ACK, new KEGS_FINAL_DELETE_UNIT_ACK { Ok = NetError.ERR_VERIFY_21, UnitUid = req.UnitUid });
                    break;
                }
                var (ok, guildUid, nick, lvl, ucls, reDelDate) = _server.Db.FinalDeleteUnit(Region, AccountUid, req.UnitUid);
                SendPacket(EventId.EGS_FINAL_DELETE_UNIT_ACK, new KEGS_FINAL_DELETE_UNIT_ACK
                {
                    Ok = ok,
                    UnitUid = req.UnitUid,
                    GuildUid = guildUid,
                    NickName = nick,
                    Level = lvl,
                    UnitClass = ucls,
                    ReDelAbleDate = reDelDate,
                });
                break;
            }

            case EventId.EGS_FORCE_RELAY_NOT:
            {
                if (ActiveUnit == null || CurrentRoomUid <= 0) break;
                var notPkt = ev.GetPayload<KEGS_FORCE_RELAY_NOT>(Region);
                var room = _server.Center.RoomManager.GetRoom(CurrentRoomUid);
                if (room != null)
                {
                    foreach (long uid in room.GetMemberUnitUids())
                        _server.SendToUnit(uid, EventId.EGS_FORCE_RELAY_NOT, notPkt);
                }
                break;
            }

            case EventId.EGS_FRAME_AVERAGE_REQ:
            {
                var req = ev.GetPayload<KEGS_FRAME_AVERAGE_REQ>(Region);
                if (req.FrameAverage < 0)
                {
                    SendPacket(EventId.EGS_FRAME_AVERAGE_ACK, new KEGS_FRAME_AVERAGE_ACK { Ok = NetError.ERR_STATISTICS_00 });
                    break;
                }
                int fpsBucket = Math.Min(12, req.FrameAverage / 10);
                _server.StatisticsManager.IncrementCounter($"FpsAvg_{req.GameType}_{fpsBucket}");
                SendPacket(EventId.EGS_FRAME_AVERAGE_ACK, new KEGS_FRAME_AVERAGE_ACK { Ok = NetError.NET_OK });
                break;
            }

            case EventId.EGS_GATHER_GIVE_UP_QUEST_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_GATHER_GIVE_UP_QUEST_REQ>(Region);
                foreach (int questId in req.GatherQuestId)
                {
                    bool removed = QuestManager.GiveUpQuest(questId);
                    if (removed) _server.Db.GiveUpQuest(ActiveUnit.UnitUid, questId);
                    SendPacket(EventId.EGS_GIVE_UP_QUEST_ACK, new KEGS_GIVE_UP_QUEST_ACK
                    {
                        Ok = removed ? NetError.NET_OK : NetError.ERR_QUEST_02,
                        QuestId = questId,
                    });
                }
                break;
            }

            case EventId.EGS_GET_ADVERTISEMENT_EVENT_REQ:
            {
                SendPacket(EventId.EGS_GET_ADVERTISEMENT_EVENT_ACK, new KEGS_GET_ADVERTISEMENT_EVENT_ACK
                {
                    AdvertisementUrl = _server.GameEventManager.GetAdvertisementUrls(),
                });
                break;
            }

            case EventId.EGS_GET_CONNECTION_UNIT_INFO_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_GET_CONNECTION_UNIT_INFO_REQ>(Region);
                if ((DateTime.UtcNow - _lastSearchUnitUtc).TotalSeconds < 3.0 &&
                    !string.Equals(req.NickName, ActiveUnit.NickName, StringComparison.OrdinalIgnoreCase))
                {
                    SendPacket(EventId.EGS_GET_CONNECTION_UNIT_INFO_ACK, new KEGS_GET_CONNECTION_UNIT_INFO_ACK { Ok = NetError.ERR_SEARCH_UNIT_01 });
                    break;
                }
                _lastSearchUnitUtc = DateTime.UtcNow;
                var target = _server.GetUserByNickName(req.NickName);
                var unit = target?.ActiveUnit ?? _server.Db.GetUnitByNickName(Region, req.NickName);
                SendPacket(EventId.EGS_GET_CONNECTION_UNIT_INFO_ACK, new KEGS_GET_CONNECTION_UNIT_INFO_ACK
                {
                    Ok = unit != null ? NetError.NET_OK : NetError.ERR_SEARCH_UNIT_03,
                    CUnitInfo = unit != null ? BuildConnectionUnitInfo(unit, target) : new KConnectionUnitInfo(),
                });
                break;
            }

            case EventId.EGS_GET_CREATE_UNIT_TODAY_COUNT_REQ:
            {
                var counts = _server.Db.GetCreateUnitCountTodayByServerGroup(Region, AccountUid);
                SendPacket(EventId.EGS_GET_CREATE_UNIT_TODAY_COUNT_ACK, new KEGS_GET_CREATE_UNIT_TODAY_COUNT_ACK
                {
                    UserUid = AccountUid,
                    CreateCharCountToday = counts,
                });
                break;
            }

            case EventId.EGS_GET_CREATE_UNIT_TODAY_COUNT_1ST_ACK:
            case EventId.EGS_GET_CREATE_UNIT_TODAY_COUNT_ACK:
            {
                var ack = ev.GetPayload<KEGS_GET_CREATE_UNIT_TODAY_COUNT_ACK>(Region);
                SendPacket(EventId.EGS_GET_CREATE_UNIT_TODAY_COUNT_ACK, ack);
                break;
            }

            case EventId.EGS_GET_EVENT_INFO_REQ:
            {
                SendPacket(EventId.EGS_GET_EVENT_INFO_ACK, new KEGS_GET_EVENT_INFO_ACK
                {
                    Ok = NetError.NET_OK,
                    EventCharacterRanking = new KEventCharacterRanking(),
                });
                break;
            }

            case EventId.EGS_GET_NEXT_SKILL_PAGE_ED_REQ:
            {
                int nextEd = SkillTree.GetNextSkillPageExpandEd();
                SendPacket(EventId.EGS_GET_NEXT_SKILL_PAGE_ED_ACK, new KEGS_GET_NEXT_SKILL_PAGE_ED_ACK
                {
                    Ok = nextEd >= 0 ? NetError.NET_OK : NetError.ERR_SKILL_PAGE_02,
                    Ed = nextEd >= 0 ? nextEd : 0,
                });
                break;
            }

            case EventId.EGS_GET_RANKING_INFO_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_GET_RANKING_INFO_REQ>(Region);
                var ack = new KEGS_GET_RANKING_INFO_ACK();
                _server.PopulateRankingInfo(req, ActiveUnit.UnitUid, ack);
                SendPacket(EventId.EGS_GET_RANKING_INFO_ACK, ack);
                break;
            }

            case EventId.EGS_GET_RECOMMEND_USER_LIST_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_GET_RECOMMEND_USER_LIST_REQ>(Region);
                var list = _server.Db.GetRecommendedByUsers(Region, ActiveUnit.UnitUid);
                foreach (var item in list)
                    item.IsOnline = _server.GetOnlineUserByUnitUid(item.UnitUid) != null;
                const int pageSize = 10;
                uint totalPage = (uint)Math.Max(1, (list.Count + pageSize - 1) / pageSize);
                uint viewPage = Math.Clamp(req.ViewPage == 0 ? 1u : req.ViewPage, 1u, totalPage);
                SendPacket(EventId.EGS_GET_RECOMMEND_USER_LIST_ACK, new KEGS_GET_RECOMMEND_USER_LIST_ACK
                {
                    Ok = NetError.NET_OK,
                    TotalPage = totalPage,
                    ViewPage = viewPage,
                    RecommendUserList = list.Skip(((int)viewPage - 1) * pageSize).Take(pageSize).ToList(),
                });
                break;
            }

            case EventId.EGS_GET_RECRUIT_RECRUITER_LIST_REQ:
            {
                if (ActiveUnit == null) break;
                var list = _server.Db.GetRecommendedByUsers(Region, ActiveUnit.UnitUid);
                foreach (var item in list)
                    item.IsOnline = _server.GetOnlineUserByUnitUid(item.UnitUid) != null;
                SendPacket(EventId.EGS_GET_RECRUIT_RECRUITER_LIST_ACK, new KEGS_GET_RECRUIT_RECRUITER_LIST_ACK
                {
                    Ok = NetError.NET_OK,
                    RecruitUnitInfo = list,
                });
                break;
            }

            case EventId.EGS_GET_TIME_CONTROL_ITME_TALK_LIST_REQ:
            {
                SendPacket(EventId.EGS_GET_TIME_CONTROL_ITME_TALK_LIST_ACK, new KEGS_GET_TIME_CONTROL_ITME_TALK_LIST_ACK
                {
                    MapGetItemOnOff = _timeControlItemCheckDungeonPlay
                        ? new Dictionary<int, List<KPacketGetItemOnOff>>()
                        : _server.GameEventManager.GetTimeControlItemsSnapshot(),
                });
                break;
            }

            case EventId.EGS_JUMPING_CHARACTER_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_JUMPING_CHARACTER_REQ>(Region);
                int checkErr = JumpingCharacterCheck(out int targetLevel, out var currentDate);
                if (checkErr != NetError.NET_OK)
                {
                    SendPacket(EventId.EGS_JUMPING_CHARACTER_ACK, new KEGS_JUMPING_CHARACTER_ACK { Ok = checkErr });
                    break;
                }
                if (req.Class < 10)
                {
                    SendPacket(EventId.EGS_JUMPING_CHARACTER_ACK, new KEGS_JUMPING_CHARACTER_ACK { Ok = NetError.ERR_JUMPING_07 });
                    break;
                }
                _server.Db.SetUnitClass(ActiveUnit.UnitUid, (sbyte)req.Class);
                _server.Db.SetUnitLevel(ActiveUnit.UnitUid, (byte)Math.Clamp(targetLevel, 1, 99));
                _jumpingRegDate = currentDate.ToString("yyyy-MM-dd HH:mm:ss");
                _server.Db.SaveUserEventState(ActiveUnit.UnitUid, _oldYearMissionRewardedLevel, _newYearMissionStepId,
                    _checkPowerTime, _checkPowerCount, _checkPowerScore, _noteViewCount, _chinaSpirit, _jumpingRegDate);
                SkillTree.TryGetExpandSkillNotePage(targetLevel, out sbyte expandedNote);
                var defSkills = SkillTree.LearnAllDefaultSkills((sbyte)req.Class);
                ActiveUnit = _server.Db.GetUnitByUid(Region, ActiveUnit.UnitUid);
                SendPacket(EventId.EGS_JUMPING_CHARACTER_ACK, new KEGS_JUMPING_CHARACTER_ACK
                {
                    Ok = NetError.NET_OK,
                    UnitInfo = ActiveUnit ?? new KUnitInfo(),
                    ExpandedMaxPageNum = expandedNote,
                    NewDefaultSkill = defSkills.Keys.ToList(),
                });
                break;
            }

            case EventId.EGS_KEYBOARD_MAPPING_INFO_WRITE_REQ:
            {
                var req = ev.GetPayload<KEGS_KEYBOARD_MAPPING_INFO_WRITE_REQ>(Region);
                _server.Db.SetKeyboardMapping(AccountUid, req.KeyboardMappingInfo, req.GamePadMappingInfo);
                SendPacket(EventId.EGS_KEYBOARD_MAPPING_INFO_WRITE_ACK, new KEGS_KEYBOARD_MAPPING_INFO_WRITE_ACK { Ok = NetError.NET_OK });
                break;
            }

            case EventId.EGS_KOM_FILE_CHECK_LOG_REQ:
            {
                var req = ev.GetPayload<KEGS_KOM_FILE_CHECK_LOG_REQ>(Region);
                _server.Db.RecordHackOrPortOrKomLog(AccountUid, ActiveUnit?.UnitUid ?? 0, "KomCheck", 0, 0, req.InvalidKomName);
                SendPacket(EventId.EGS_KOM_FILE_CHECK_LOG_ACK, new KEGS_KOM_FILE_CHECK_LOG_ACK
                {
                    InvalidKomName = req.InvalidKomName,
                    Ok = NetError.NET_OK,
                });
                break;
            }

            case EventId.EGS_LOCAL_RANKING_INQUIRY_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_LOCAL_RANKING_INQUIRY_REQ>(Region);
                if (req.MainTabIndex < 0 || req.MainTabIndex >= 4) break;
                var (myInfo, _) = _server.Db.GetLocalRankingUserInfo(AccountUid);
                _localRankingUserInfo = myInfo;
                var rankers = _server.Db.QueryLocalRankings(Region, myInfo.Category, 100);
                const int pageSize = 10;
                uint totalPage = (uint)Math.Max(1, (rankers.Count + pageSize - 1) / pageSize);
                uint viewPage = Math.Clamp(req.ViewPage == 0 ? 1u : req.ViewPage, 1u, totalPage);
                var pageList = rankers.Skip(((int)viewPage - 1) * pageSize).Take(pageSize).ToList();
                var myRank = rankers.FirstOrDefault(r => r.UnitInfo.UnitUid == ActiveUnit.UnitUid) ?? new KLocalRankingRankerInfo
                {
                    UserInfo = myInfo,
                    UnitInfo = new KLocalRankingUnitInfo
                    {
                        UnitUid = ActiveUnit.UnitUid,
                        NickName = ActiveUnit.NickName,
                        Level = ActiveUnit.Level,
                        UnitClass = ActiveUnit.UnitClass,
                    },
                };
                SendPacket(EventId.EGS_LOCAL_RANKING_INQUIRY_ACK, new KEGS_LOCAL_RANKING_INQUIRY_ACK
                {
                    Ok = NetError.NET_OK,
                    TotalPage = totalPage,
                    ViewPage = viewPage,
                    MainTabIndex = req.MainTabIndex,
                    SubTabIndex = req.SubTabIndex,
                    Filter = req.Filter,
                    RankerList = pageList,
                    MyRankInfo = myRank,
                });
                break;
            }

            case EventId.EGS_LOCAL_RANKING_INQUIRY_ACK:
            {
                var ack = ev.GetPayload<KEGS_LOCAL_RANKING_INQUIRY_ACK>(Region);
                SendPacket(EventId.EGS_LOCAL_RANKING_INQUIRY_ACK, ack);
                break;
            }

            case EventId.EGS_LOCAL_RANKING_USER_INFO_READ_REQ:
            {
                var req = ev.GetPayload<KEGS_LOCAL_RANKING_USER_INFO_READ_REQ>(Region);
                if (req.Password.Length > 50)
                {
                    SendPacket(EventId.EGS_LOCAL_RANKING_USER_INFO_READ_ACK, new KEGS_LOCAL_RANKING_USER_INFO_READ_ACK { Ok = NetError.ERR_LOCALRANKING_08 });
                    break;
                }
                var (info, updated) = _server.Db.GetLocalRankingUserInfo(AccountUid);
                _localRankingUserInfo = info;
                SendPacket(EventId.EGS_LOCAL_RANKING_USER_INFO_READ_ACK, new KEGS_LOCAL_RANKING_USER_INFO_READ_ACK
                {
                    Ok = NetError.NET_OK,
                    Info = info,
                    Updated = updated,
                });
                break;
            }

            case EventId.EGS_LOCAL_RANKING_USER_INFO_WRITE_REQ:
            {
                var req = ev.GetPayload<KEGS_LOCAL_RANKING_USER_INFO_WRITE_REQ>(Region);
                if (AuthLevel < 5 && !_server.StringFilterManager.CheckIsValidString(KStringFilterManager.FilterType.FtWord, req.Info.Profile))
                {
                    SendPacket(EventId.EGS_LOCAL_RANKING_USER_INFO_WRITE_ACK, new KEGS_LOCAL_RANKING_USER_INFO_WRITE_ACK { Ok = NetError.ERR_STRING_FILTER_02 });
                    break;
                }
                if (req.Password.Length > 50)
                {
                    SendPacket(EventId.EGS_LOCAL_RANKING_USER_INFO_WRITE_ACK, new KEGS_LOCAL_RANKING_USER_INFO_WRITE_ACK { Ok = NetError.ERR_LOCALRANKING_08 });
                    break;
                }
                req.Info.UserUid = AccountUid;
                _localRankingUserInfo = req.Info;
                _server.Db.SaveLocalRankingUserInfo(AccountUid, ActiveUnit?.UnitUid ?? 0, req.Info, ActiveUnit?.Level ?? 0, ActiveUnit?.Exp ?? 0);
                SendPacket(EventId.EGS_LOCAL_RANKING_USER_INFO_WRITE_ACK, new KEGS_LOCAL_RANKING_USER_INFO_WRITE_ACK { Ok = NetError.NET_OK });
                break;
            }

            case EventId.EGS_LOCAL_RANKING_WATCH_UNIT_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_LOCAL_RANKING_WATCH_UNIT_REQ>(Region);
                if (req.UnitUid != ActiveUnit.UnitUid && (DateTime.UtcNow - _lastSearchUnitUtc).TotalSeconds < 3.0)
                {
                    SendPacket(EventId.EGS_LOCAL_RANKING_WATCH_UNIT_ACK, new KEGS_LOCAL_RANKING_WATCH_UNIT_ACK { Ok = NetError.ERR_SEARCH_UNIT_01 });
                    break;
                }
                _lastSearchUnitUtc = DateTime.UtcNow;
                var target = req.UnitUid == ActiveUnit.UnitUid ? ActiveUnit : _server.Db.GetUnitByUid(Region, req.UnitUid);
                if (target == null)
                {
                    SendPacket(EventId.EGS_LOCAL_RANKING_WATCH_UNIT_ACK, new KEGS_LOCAL_RANKING_WATCH_UNIT_ACK { Ok = NetError.ERR_SEARCH_UNIT_03 });
                    break;
                }
                long? acc = _server.Db.GetAccountUidByUnitUid(target.UnitUid);
                var (lrInfo, _) = acc.HasValue ? _server.Db.GetLocalRankingUserInfo(acc.Value) : (new KLocalRankingUserInfo(), string.Empty);
                SendPacket(EventId.EGS_LOCAL_RANKING_WATCH_UNIT_ACK, new KEGS_LOCAL_RANKING_WATCH_UNIT_ACK
                {
                    Ok = NetError.NET_OK,
                    UserInfo = lrInfo,
                    UnitInfo = new KLocalRankingUnitInfo
                    {
                        UnitUid = target.UnitUid,
                        NickName = target.NickName,
                        Level = target.Level,
                        UnitClass = target.UnitClass,
                    },
                });
                break;
            }

            case EventId.EGS_MANUFACTURE_PERIOD_SETTING_REQ:
            {
                var req = ev.GetPayload<KEGS_MANUFACTURE_PERIOD_SETTING_REQ>(Region);
                SendPacket(EventId.EGS_MANUFACTURE_PERIOD_SETTING_ACK, new KEGS_MANUFACTURE_PERIOD_SETTING_ACK
                {
                    Ok = NetError.NET_OK,
                    PeriodGroup = new Dictionary<int, int>(_server.GetManufacturePeriodGroups()),
                    HouseId = req.HouseId,
                });
                break;
            }

            case EventId.EGS_MOMOTI_QUIZ_EVENT_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_MOMOTI_QUIZ_EVENT_REQ>(Region);
                SendPacket(EventId.EGS_MOMOTI_QUIZ_EVENT_ACK, new KEGS_MOMOTI_QUIZ_EVENT_ACK
                {
                    Ok = NetError.NET_OK,
                    CheckReward = req.Reply > 0 ? 1 : 0,
                });
                break;
            }

            case EventId.EGS_RECOMMEND_USER_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_RECOMMEND_USER_REQ>(Region);
                if (AuthLevel < 5 && !_server.StringFilterManager.CheckIsValidString(KStringFilterManager.FilterType.FtNickName, req.NickName))
                {
                    SendPacket(EventId.EGS_RECOMMEND_USER_ACK, new KEGS_RECOMMEND_USER_ACK { Ok = NetError.ERR_STRING_FILTER_01 });
                    break;
                }
                if (req.Cancel || IsGuestUser)
                {
                    SendPacket(EventId.EGS_RECOMMEND_USER_ACK, new KEGS_RECOMMEND_USER_ACK { Ok = NetError.NET_OK });
                    break;
                }
                if (string.Equals(ActiveUnit.NickName, req.NickName, StringComparison.OrdinalIgnoreCase))
                {
                    SendPacket(EventId.EGS_RECOMMEND_USER_ACK, new KEGS_RECOMMEND_USER_ACK { Ok = NetError.ERR_RECOMMEND_USER_01 });
                    break;
                }
                var (err, _) = _server.Db.RecommendUser(Region, ActiveUnit.UnitUid, req.NickName);
                int rewardStones = err == NetError.NET_OK ? 3 : 0;
                if (rewardStones > 0) NumResurrectionStone += rewardStones;
                SendPacket(EventId.EGS_RECOMMEND_USER_ACK, new KEGS_RECOMMEND_USER_ACK
                {
                    Ok = err,
                    RewardResurrectionStone = rewardStones,
                });
                break;
            }

            case EventId.EGS_REG_SKILL_NOTE_MEMO_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_REG_SKILL_NOTE_MEMO_REQ>(Region);
                var meta = _server.Db.GetInventoryItemMeta(ActiveUnit.UnitUid, req.ItemUid);
                if (!meta.found)
                {
                    SendPacket(EventId.EGS_REG_SKILL_NOTE_MEMO_ACK, new KEGS_REG_SKILL_NOTE_MEMO_ACK { Ok = NetError.ERR_SKILL_NOTE_00 });
                    break;
                }
                if (!SkillTree.HasSkillNote)
                {
                    SendPacket(EventId.EGS_REG_SKILL_NOTE_MEMO_ACK, new KEGS_REG_SKILL_NOTE_MEMO_ACK { Ok = NetError.ERR_SKILL_NOTE_04 });
                    break;
                }
                if (!SkillTree.IsExistSkillNotePage(req.SkillNotePageNum))
                {
                    SendPacket(EventId.EGS_REG_SKILL_NOTE_MEMO_ACK, new KEGS_REG_SKILL_NOTE_MEMO_ACK { Ok = NetError.ERR_SKILL_NOTE_01 });
                    break;
                }
                if (SkillTree.IsExistSkillNoteMemoId(meta.itemId))
                {
                    SendPacket(EventId.EGS_REG_SKILL_NOTE_MEMO_ACK, new KEGS_REG_SKILL_NOTE_MEMO_ACK { Ok = NetError.ERR_SKILL_NOTE_02 });
                    break;
                }
                var (delOk, updatedSlot) = _server.Db.DeleteInventoryItem(ActiveUnit.UnitUid, req.ItemUid, 1);
                if (!delOk)
                {
                    SendPacket(EventId.EGS_REG_SKILL_NOTE_MEMO_ACK, new KEGS_REG_SKILL_NOTE_MEMO_ACK { Ok = NetError.ERR_SKILL_NOTE_00 });
                    break;
                }
                SkillTree.RegisterSkillNoteMemo(req.SkillNotePageNum, meta.itemId);
                _server.Db.SaveUnitSkillPageState(ActiveUnit.UnitUid, SkillTree.TheNumberOfSkillPagesAvailable,
                    SkillTree.ActiveSkillPage, SkillTree.GetSkillNotePagesSnapshot());
                SendPacket(EventId.EGS_REG_SKILL_NOTE_MEMO_ACK, new KEGS_REG_SKILL_NOTE_MEMO_ACK
                {
                    Ok = NetError.NET_OK,
                    SkillNotePageNum = req.SkillNotePageNum,
                    MemoId = meta.itemId,
                    InventorySlotInfo = [updatedSlot],
                    SkillNote = SkillTree.GetSkillNoteMemoIds(),
                });
                break;
            }

            case EventId.EGS_REQUEST_TUTORIAL_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_REQUEST_TUTORIAL_REQ>(Region);
                if (IsGuestUser)
                {
                    SendPacket(EventId.EGS_REQUEST_TUTORIAL_ACK, new KEGS_REQUEST_TUTORIAL_ACK { Ok = NetError.ERR_GUEST_USER_00 });
                    break;
                }
                if (!_isTutorialTeacher || _tutorialStudents.Count >= 3 || _tutorialStudents.Any(s => s.UnitUid == req.UnitUid))
                {
                    SendPacket(EventId.EGS_REQUEST_TUTORIAL_ACK, new KEGS_REQUEST_TUTORIAL_ACK { Ok = NetError.ERR_TUTORIAL_02 });
                    break;
                }
                _server.SendToUnit(req.UnitUid, EventId.EGS_REQUEST_TUTORIAL_NOT, new KEGS_REQUEST_TUTORIAL_NOT
                {
                    TutorialUnitInfo = new KTutorialUnitInfo
                    {
                        UnitUid = ActiveUnit.UnitUid,
                        Level = ActiveUnit.Level,
                        NickName = ActiveUnit.NickName,
                        IsOnline = true,
                    },
                });
                SendPacket(EventId.EGS_REQUEST_TUTORIAL_ACK, new KEGS_REQUEST_TUTORIAL_ACK { Ok = NetError.NET_OK });
                break;
            }

            case EventId.EGS_REQUEST_TUTORIAL_REPLY_NOT:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_REQUEST_TUTORIAL_REPLY_NOT>(Region);
                if (req.Accept)
                {
                    var teacher = _server.GetOnlineUserByUnitUid(req.ReceiverUnitUid);
                    if (teacher != null && teacher._tutorialStudents.Count < 3 &&
                        !teacher._tutorialStudents.Any(s => s.UnitUid == ActiveUnit.UnitUid))
                    {
                        teacher._tutorialStudents.Add(new KTutorialUnitInfo
                        {
                            UnitUid = ActiveUnit.UnitUid,
                            Level = ActiveUnit.Level,
                            NickName = ActiveUnit.NickName,
                            IsOnline = true,
                        });
                    }
                }
                break;
            }

            case EventId.EGS_RESTORE_UNIT_REQ:
            {
                if (AccountUid <= 0) break;
                var req = ev.GetPayload<KEGS_RESTORE_UNIT_REQ>(Region);
                if (_server.Db.GetSecondSecurityStatus(AccountUid).useSecondPw && !SecondSecurityVerified)
                {
                    SendPacket(EventId.EGS_RESTORE_UNIT_ACK, new KEGS_RESTORE_UNIT_ACK { Ok = NetError.ERR_VERIFY_21, UnitUid = req.UnitUid });
                    break;
                }
                var (ok, restoreDate) = _server.Db.RestoreUnit(Region, AccountUid, req.UnitUid);
                SendPacket(EventId.EGS_RESTORE_UNIT_ACK, new KEGS_RESTORE_UNIT_ACK
                {
                    Ok = ok,
                    UnitUid = req.UnitUid,
                    RestoreAbleDate = restoreDate,
                });
                break;
            }

            case EventId.EGS_SEARCH_UNIT_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_SEARCH_UNIT_REQ>(Region);
                if ((DateTime.UtcNow - _lastSearchUnitUtc).TotalSeconds < 3.0 &&
                    !string.Equals(req.NickName, ActiveUnit.NickName, StringComparison.OrdinalIgnoreCase))
                {
                    SendPacket(EventId.EGS_SEARCH_UNIT_ACK, new KEGS_SEARCH_UNIT_ACK { Ok = NetError.ERR_SEARCH_UNIT_01 });
                    break;
                }
                _lastSearchUnitUtc = DateTime.UtcNow;
                var target = _server.GetUserByNickName(req.NickName);
                var unit = target?.ActiveUnit ?? _server.Db.GetUnitByNickName(Region, req.NickName);
                SendPacket(EventId.EGS_SEARCH_UNIT_ACK, new KEGS_SEARCH_UNIT_ACK
                {
                    Ok = unit != null ? NetError.NET_OK : NetError.ERR_SEARCH_UNIT_03,
                    CUnitInfo = unit != null ? BuildConnectionUnitInfo(unit, target) : new KConnectionUnitInfo(),
                });
                break;
            }

            case EventId.EGS_SKILL_USE_REQ:
            {
                if (ActiveUnit == null || CurrentRoomUid <= 0) break;
                var req = ev.GetPayload<KEGS_SKILL_USE_REQ>(Region);
                if (req.SkillId == -1) break;
                _server.StatisticsManager.IncrementCounter($"SkillUse_{req.SkillId}");
                break;
            }

            case EventId.EGS_START_CHECK_POWER_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_START_CHECK_POWER_REQ>(Region);
                if (!_server.GameEventManager.IsCheckPowerEventEnabled)
                {
                    SendPacket(EventId.EGS_START_CHECK_POWER_ACK, new KEGS_START_CHECK_POWER_ACK { Ok = NetError.ERR_QUEST_01 });
                    break;
                }
                var now = DateTime.UtcNow;
                long nowUnix = new DateTimeOffset(now).ToUnixTimeSeconds();
                if (req.Start)
                {
                    byte maxCount = 2;
                    if (_checkPowerCount >= maxCount)
                    {
                        SendPacket(EventId.EGS_START_CHECK_POWER_ACK, new KEGS_START_CHECK_POWER_ACK { Ok = NetError.ERR_QUEST_01 });
                        break;
                    }
                    var prevDate = DateTimeOffset.FromUnixTimeSeconds(_checkPowerTime).UtcDateTime;
                    if (prevDate.Date != now.Date)
                        _checkPowerCount = 0;
                    _checkPowerTime = nowUnix;
                    _checkPowerCount++;
                    _checkPowerScore = 0;
                }
                else
                {
                    long cutoff = new DateTimeOffset(2013, 12, 1, 0, 0, 0, TimeSpan.Zero).ToUnixTimeSeconds();
                    if (_checkPowerTime < cutoff)
                    {
                        _checkPowerTime = new DateTimeOffset(2013, 12, 17, 0, 0, 0, TimeSpan.Zero).ToUnixTimeSeconds();
                        _checkPowerScore = 255;
                    }
                    else
                    {
                        SendPacket(EventId.EGS_START_CHECK_POWER_ACK, new KEGS_START_CHECK_POWER_ACK { Ok = NetError.ERR_QUEST_01 });
                        break;
                    }
                }
                _server.Db.SaveUserEventState(ActiveUnit.UnitUid, _oldYearMissionRewardedLevel, _newYearMissionStepId,
                    _checkPowerTime, _checkPowerCount, _checkPowerScore, _noteViewCount, _chinaSpirit);
                SendPacket(EventId.EGS_START_CHECK_POWER_ACK, new KEGS_START_CHECK_POWER_ACK { Ok = NetError.NET_OK });
                break;
            }

            case EventId.EGS_SUPPORT_MATERIAL_EVENT_TIME_REQ:
            {
                SendPacket(EventId.EGS_SUPPORT_MATERIAL_EVENT_TIME_ACK, new KEGS_SUPPORT_MATERIAL_EVENT_TIME_ACK
                {
                    IsEventTime = _server.GameEventManager.IsEnchantEventActive,
                });
                break;
            }

            case EventId.EGS_TALK_WITH_NPC_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_TALK_WITH_NPC_REQ>(Region);
                if (_server.TradeManager.GetSessionByUnit(ActiveUnit.UnitUid) != null || PersonalShopManager.IsOpened)
                {
                    SendPacket(EventId.EGS_TALK_WITH_NPC_ACK, new KEGS_TALK_WITH_NPC_ACK { Ok = NetError.ERR_ITEM_14 });
                    break;
                }
                QuestManager.OnTalkWithNpc(req.NpcId);
                SendPacket(EventId.EGS_TALK_WITH_NPC_ACK, new KEGS_TALK_WITH_NPC_ACK { Ok = NetError.NET_OK });
                break;
            }

            case EventId.EGS_TUTORIAL_STUDENT_LIST_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_TUTORIAL_STUDENT_LIST_REQ>(Region);
                if (ActiveUnit.Level < KTutorialManager.UnitLevelTeacherMin)
                {
                    SendPacket(EventId.EGS_TUTORIAL_STUDENT_LIST_ACK, new KEGS_TUTORIAL_STUDENT_LIST_ACK { Ok = NetError.ERR_TUTORIAL_02 });
                    break;
                }
                if (_server.TutorialManager.StudentPoolCount > 0)
                {
                    SendPacket(EventId.EGS_TUTORIAL_STUDENT_LIST_ACK, _server.TutorialManager.GetTutorPageInfo(req.ViewPage));
                }
                else
                {
                    const int pageSize = KTutorialManager.StudentPageNum;
                    int totalPage = Math.Max(1, (_tutorialStudents.Count + pageSize - 1) / pageSize);
                    int viewPage = Math.Clamp(req.ViewPage <= 0 ? 1 : req.ViewPage, 1, totalPage);
                    SendPacket(EventId.EGS_TUTORIAL_STUDENT_LIST_ACK, new KEGS_TUTORIAL_STUDENT_LIST_ACK
                    {
                        Ok = NetError.NET_OK,
                        TotalPage = totalPage,
                        ViewPage = viewPage,
                        TutorialStudentList = _tutorialStudents.Skip((viewPage - 1) * pageSize).Take(pageSize).ToList(),
                    });
                }
                break;
            }

            case EventId.EGS_UDP_CHECK_KICK_USER_NOT:
            {
                if (ActiveUnit == null || CurrentRoomUid <= 0) break;
                var notPkt = ev.GetPayload<KEGS_UDP_CHECK_KICK_USER_NOT>(Region);
                var room = _server.Center.RoomManager.GetRoom(CurrentRoomUid);
                if (room != null && notPkt.NowKickOut)
                    _server.Center.RoomManager.LeaveRoom(CurrentRoomUid, notPkt.AccusationUnitUid);
                break;
            }

            case EventId.EGS_UNLIMITED_SECOND_CHANGE_JOB_NOT:
            {
                if (ActiveUnit == null) break;
                var notPkt = ev.GetPayload<KEGS_UNLIMITED_SECOND_CHANGE_JOB_NOT>(Region);
                if (GetUnlimitedClassChangeInfo(notPkt.UnlimitedSecondChangeJob, out int retrievedSp, out int retrievedCsp))
                {
                    _server.Db.SetUnitClass(ActiveUnit.UnitUid, (sbyte)notPkt.UnlimitedSecondChangeJob);
                    _server.Db.SetUnitSkillPoint(ActiveUnit.UnitUid, retrievedSp);
                    SkillTree.SetAvailableSkillPoints(SkillTree.ActiveSkillPage, retrievedSp + retrievedCsp);
                    ActiveUnit = _server.Db.GetUnitByUid(Region, ActiveUnit.UnitUid);
                    SendPacket(EventId.EGS_BUY_UNIT_CLASS_CHANGE_NOT, new KEGS_BUY_UNIT_CLASS_CHANGE_NOT
                    {
                        UnitUid = ActiveUnit.UnitUid,
                        NewUnitClass = notPkt.UnlimitedSecondChangeJob,
                        UnitInfo = ActiveUnit ?? new KUnitInfo(),
                    });
                }
                break;
            }

            case EventId.EGS_UPDATE_NPC_UNIT_BUFF_INFO_NOT:
            {
                if (ActiveUnit == null || CurrentRoomUid <= 0) break;
                var notPkt = ev.GetPayload<KEGS_UPDATE_NPC_UNIT_BUFF_INFO_NOT>(Region);
                var room = _server.Center.RoomManager.GetRoom(CurrentRoomUid);
                if (room != null)
                {
                    foreach (long uid in room.GetMemberUnitUids())
                        if (uid != ActiveUnit.UnitUid)
                            _server.SendToUnit(uid, EventId.EGS_UPDATE_NPC_UNIT_BUFF_INFO_NOT, notPkt);
                }
                break;
            }

            case EventId.EGS_UPDATE_PLAY_STATUS_NOT:
            {
                if (ActiveUnit == null) break;
                var notPkt = ev.GetPayload<KEGS_UPDATE_PLAY_STATUS_NOT>(Region);
                var st = notPkt.GamePlayStatus;
                st.MaxHp = Math.Max(ActiveUnit.Level * 3000, st.MaxHp);
                st.CurHp = Math.Clamp(st.CurHp, 0, st.MaxHp);
                st.MaxMp = Math.Clamp(st.MaxMp, 0, 500);
                st.CurMp = Math.Clamp(st.CurMp, 0, st.MaxMp);
                st.CurHyperGage = Math.Max(0, st.CurHyperGage);
                st.CharAbilType = Math.Max((sbyte)0, st.CharAbilType);
                _gamePlayStatus = st;
                break;
            }

            case EventId.EGS_UPDATE_USER_UNIT_BUFF_INFO_NOT:
            {
                if (ActiveUnit == null || CurrentRoomUid <= 0) break;
                var notPkt = ev.GetPayload<KEGS_UPDATE_USER_UNIT_BUFF_INFO_NOT>(Region);
                if (notPkt.ToUnitUid > 0)
                {
                    _server.SendToUnit(notPkt.ToUnitUid, EventId.EGS_UPDATE_USER_UNIT_BUFF_INFO_NOT, notPkt);
                }
                else
                {
                    var room = _server.Center.RoomManager.GetRoom(CurrentRoomUid);
                    if (room != null)
                    {
                        foreach (long uid in room.GetMemberUnitUids())
                            if (uid != ActiveUnit.UnitUid)
                                _server.SendToUnit(uid, EventId.EGS_UPDATE_USER_UNIT_BUFF_INFO_NOT, notPkt);
                    }
                }
                break;
            }

            case EventId.EGS_USER_COMMUNITY_SURVEY_NEW_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_USER_COMMUNITY_SURVEY_NEW_REQ>(Region);
                if (string.Equals(ActiveUnit.NickName, req.UnitNickName, StringComparison.OrdinalIgnoreCase))
                {
                    _communityTabType = 0;
                    SendPacket(EventId.EGS_USER_COMMUNITY_SURVEY_NEW_ACK, new KEGS_USER_COMMUNITY_SURVEY_NEW_ACK { Ok = NetError.NET_OK });
                    break;
                }
                if ((DateTime.UtcNow - _lastSearchUnitUtc).TotalSeconds < 3.0)
                {
                    _communityTabType = 0;
                    _lastSearchUnitUtc = DateTime.UtcNow;
                    SendPacket(EventId.EGS_USER_COMMUNITY_SURVEY_NEW_ACK, new KEGS_USER_COMMUNITY_SURVEY_NEW_ACK { Ok = NetError.ERR_SEARCH_UNIT_01 });
                    break;
                }
                _communityTabType = req.CommunityTabType;
                long? foundUid = _server.Db.FindUnitUidByNickName(Region, req.UnitNickName);
                SendPacket(EventId.EGS_USER_COMMUNITY_SURVEY_NEW_ACK, new KEGS_USER_COMMUNITY_SURVEY_NEW_ACK
                {
                    Ok = foundUid.HasValue ? NetError.NET_OK : NetError.ERR_SEARCH_UNIT_03,
                    UnitUid = foundUid ?? 0,
                    CommunityTabType = _communityTabType,
                });
                break;
            }

            case EventId.EGS_USE_MEGAPHONE_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_USE_MEGAPHONE_REQ>(Region);
                if (AuthLevel < 5 && !_server.StringFilterManager.CheckIsValidString(KStringFilterManager.FilterType.FtWord, req.Message))
                {
                    SendPacket(EventId.EGS_USE_MEGAPHONE_ACK, new KEGS_USE_MEGAPHONE_ACK { Ok = NetError.ERR_STRING_FILTER_00 });
                    break;
                }
                var meta = _server.Db.GetInventoryItemMeta(ActiveUnit.UnitUid, req.ItemUid);
                if (!meta.found)
                {
                    SendPacket(EventId.EGS_USE_MEGAPHONE_ACK, new KEGS_USE_MEGAPHONE_ACK { Ok = NetError.ERR_ITEM_04 });
                    break;
                }
                if (System.Text.Encoding.UTF8.GetByteCount(req.Message) > 90)
                {
                    SendPacket(EventId.EGS_USE_MEGAPHONE_ACK, new KEGS_USE_MEGAPHONE_ACK { Ok = NetError.ERR_ITEM_13 });
                    break;
                }
                var (delOk, updatedSlot) = _server.Db.DeleteInventoryItem(ActiveUnit.UnitUid, req.ItemUid, 1);
                if (!delOk)
                {
                    SendPacket(EventId.EGS_USE_MEGAPHONE_ACK, new KEGS_USE_MEGAPHONE_ACK { Ok = NetError.ERR_ITEM_04 });
                    break;
                }
                SendPacket(EventId.EGS_USE_MEGAPHONE_ACK, new KEGS_USE_MEGAPHONE_ACK
                {
                    Ok = NetError.NET_OK,
                    InventorySlotInfo = [updatedSlot],
                });
                _server.BroadcastMegaphone(allServers: true, new KEGS_MEGAPHONE_MESSAGE_NOT
                {
                    SenderNickName = ActiveUnit.NickName,
                    SenderChannelId = 1,
                    SenderLocation = CurrentFieldMapId,
                    Message = req.Message,
                });
                _server.StatisticsManager.IncrementCounter("MegaphoneUsed");
                break;
            }

            case EventId.EGS_USE_SKILL_BUFF_IN_PLAY_NOT:
            {
                if (ActiveUnit == null || CurrentRoomUid <= 0) break;
                var notPkt = ev.GetPayload<KEGS_USE_SKILL_BUFF_IN_PLAY_NOT>(Region);
                var room = _server.Center.RoomManager.GetRoom(CurrentRoomUid);
                if (room != null)
                {
                    foreach (long uid in room.GetMemberUnitUids())
                        if (uid != ActiveUnit.UnitUid)
                            _server.SendToUnit(uid, EventId.EGS_USE_SKILL_BUFF_IN_PLAY_NOT, notPkt);
                }
                break;
            }

            case EventId.EGS_USE_SPIRIT_REWARD_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_USE_SPIRIT_REWARD_REQ>(Region);
                if (!_server.GameEventManager.IsChinaSpiritEventEnabled ||
                    req.LocationIndex < 0 || req.LocationIndex >= 6 ||
                    _chinaSpirit[req.LocationIndex] < 2350)
                {
                    SendPacket(EventId.EGS_USE_SPIRIT_REWARD_ACK, new KEGS_USE_SPIRIT_REWARD_ACK { Ok = NetError.ERR_UNKNOWN });
                    break;
                }
                _chinaSpirit[req.LocationIndex] = 0;
                _server.Db.SendLetter(Region, ActiveUnit.UnitUid, "System", ActiveUnit.UnitUid,
                    "Spirit Reward", "Event reward", 1492 + req.LocationIndex, 1, 0);
                _server.Db.SaveUserEventState(ActiveUnit.UnitUid, _oldYearMissionRewardedLevel, _newYearMissionStepId,
                    _checkPowerTime, _checkPowerCount, _checkPowerScore, _noteViewCount, _chinaSpirit);
                var ack = new KEGS_USE_SPIRIT_REWARD_ACK
                {
                    Ok = NetError.NET_OK,
                    LocationIndex = req.LocationIndex,
                };
                Array.Copy(_chinaSpirit, ack.ChinaSpirit, 6);
                SendPacket(EventId.EGS_USE_SPIRIT_REWARD_ACK, ack);
                break;
            }

            case EventId.EGS_WARP_BY_BUTTON_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_WARP_BY_BUTTON_REQ>(Region);
                int destMapId = req.CurrentVillageWarpIndex;
                if (destMapId <= 0)
                {
                    SendPacket(EventId.EGS_WARP_BY_BUTTON_ACK, new KEGS_WARP_BY_BUTTON_ACK { Ok = NetError.ERR_USE_ITEM_IN_INVENTORY_05 });
                    break;
                }
                if (!req.FreeWarp)
                {
                    if (CurrentFieldMapId == destMapId)
                    {
                        SendPacket(EventId.EGS_WARP_BY_BUTTON_ACK, new KEGS_WARP_BY_BUTTON_ACK { Ok = NetError.ERR_UNKNOWN });
                        break;
                    }
                    int cost = Math.Max(500, ActiveUnit.Level * 200);
                    if (!EdManager.TryConsumeEd(cost, KUserEDManager.EdReason.ErOutEdWarp, out _))
                    {
                        SendPacket(EventId.EGS_WARP_BY_BUTTON_ACK, new KEGS_WARP_BY_BUTTON_ACK { Ok = NetError.ERR_USE_ITEM_IN_INVENTORY_08 });
                        break;
                    }
                    _server.Db.UpdateUnitProgress(ActiveUnit.UnitUid, addExp: 0, addEd: -cost, mapId: destMapId);
                }
                else
                {
                    _server.Db.UpdateUnitProgress(ActiveUnit.UnitUid, addExp: 0, addEd: 0, mapId: destMapId);
                }
                CurrentFieldMapId = destMapId;
                ActiveUnit = _server.Db.GetUnitByUid(Region, ActiveUnit.UnitUid);
                SendPacket(EventId.EGS_WARP_BY_BUTTON_ACK, new KEGS_WARP_BY_BUTTON_ACK
                {
                    Ok = NetError.NET_OK,
                    Ed = ActiveUnit?.Ed ?? EdManager.CurrentEd,
                    WarpPointMapId = destMapId,
                });
                break;
            }

            case EventId.EGS_WATCH_UNIT_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGS_WATCH_UNIT_REQ>(Region);
                if (req.UnitUid != ActiveUnit.UnitUid && (DateTime.UtcNow - _lastSearchUnitUtc).TotalSeconds < 3.0)
                {
                    SendPacket(EventId.EGS_WATCH_UNIT_ACK, new KEGS_WATCH_UNIT_ACK { Ok = NetError.ERR_SEARCH_UNIT_01 });
                    break;
                }
                _lastSearchUnitUtc = DateTime.UtcNow;
                var target = _server.GetOnlineUserByUnitUid(req.UnitUid);
                var unit = target?.ActiveUnit ?? _server.Db.GetUnitByUid(Region, req.UnitUid);
                SendPacket(EventId.EGS_WATCH_UNIT_ACK, new KEGS_WATCH_UNIT_ACK
                {
                    Ok = unit != null ? NetError.NET_OK : NetError.ERR_SEARCH_UNIT_03,
                    CUnitInfo = unit != null ? BuildConnectionUnitInfo(unit, target) : new KConnectionUnitInfo(),
                });
                break;
            }

            case EventId.EGB_ADMIN_BOSS_FIELD_GATE_OPEN_ACK:
            {
                var ack = ev.GetPayload<KPacketOK>(Region);
                SendPacket(EventId.EGS_ADMIN_BOSS_FIELD_GATE_OPEN_ACK, ack);
                break;
            }

            case EventId.EGB_ADMIN_GET_TOTAL_DANGEROUS_VALUE_ACK:
            {
                var ack = ev.GetPayload<KEGS_ADMIN_GET_TOTAL_DANGEROUS_VALUE_ACK>(Region);
                SendPacket(EventId.EGS_ADMIN_GET_TOTAL_DANGEROUS_VALUE_ACK, ack);
                break;
            }

            case EventId.EGB_ADMIN_SET_TOTAL_DANGEROUS_VALUE_ACK:
            {
                var ack = ev.GetPayload<KPacketOK>(Region);
                SendPacket(EventId.EGS_ADMIN_SET_TOTAL_DANGEROUS_VALUE_ACK, ack);
                break;
            }

            case EventId.EGB_ADMIN_HERO_PVP_USER_LIST_ACK:
            {
                var ack = ev.GetPayload<KEGS_ADMIN_HERO_PVP_USER_LIST_ACK>(Region);
                SendPacket(EventId.EGS_ADMIN_HERO_PVP_USER_LIST_ACK, ack);
                break;
            }

            case EventId.EGB_ADMIN_HERO_PVP_USER_ADD_ACK:
            {
                var ack = ev.GetPayload<KEGS_ADMIN_HERO_PVP_USER_ADD_REQ>(Region);
                SendPacket(EventId.EGS_ADMIN_HERO_PVP_USER_ADD_ACK, ack);
                break;
            }

            case EventId.EGB_ADMIN_HERO_PVP_USER_REMOVE_ACK:
            {
                var ack = ev.GetPayload<KEGS_ADMIN_HERO_PVP_USER_ADD_REQ>(Region);
                SendPacket(EventId.EGS_ADMIN_HERO_PVP_USER_REMOVE_ACK, ack);
                break;
            }

            case EventId.ELG_ACCEPT_FRIEND_NOT:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KELG_ACCEPT_FRIEND_NOT>(Region);
                _friendUids.Add(req.UnitUid);
                SendPacket(EventId.EGS_ACCEPT_FRIEND_NOT, new KEGS_ACCEPT_FRIEND_NOT { UnitUid = req.UnitUid });
                break;
            }

            case EventId.ELG_BLOCK_FRIEND_NOT:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KELG_BLOCK_FRIEND_NOT>(Region);
                _blockedFriendUids.Add(req.UnitUid);
                SendPacket(EventId.EGS_BLOCK_FRIEND_NOT, new KEGS_BLOCK_FRIEND_NOT { UnitUid = req.UnitUid });
                break;
            }

            case EventId.ELG_CALL_MY_LOVER_FIND_USER_VILLAGE_ACK:
            {
                var ack = ev.GetPayload<KELG_CALL_MY_LOVER_FIND_USER_VILLAGE_ACK>(Region);
                SendPacket(EventId.EGS_CALL_MY_LOVER_ACK, new KPacketOK { Ok = ack.Ok });
                break;
            }

            case EventId.ELG_CALL_MY_LOVER_VILLAGE_NOT:
            {
                var not = ev.GetPayload<KELG_CALL_MY_LOVER_FIND_USER_VILLAGE_REQ>(Region);
                SendPacket(EventId.EGS_CALL_MY_LOVER_VILLAGE_NOT, new KEGS_CALL_MY_LOVER_VILLAGE_NOT
                {
                    MapId = not.MapId,
                    Pos = not.Pos,
                    LastTouchLineIndex = (byte)not.LastTouchLineIndex,
                    CalledUserUid = not.CalledUserUid,
                    CalledGsUid = not.CalledGsUid,
                    CallType = not.CallType,
                });
                break;
            }

            case EventId.ELG_CALL_MY_LOVER_AGREE_RESULT_VILLAGE_NOT:
            {
                var not = ev.GetPayload<KELG_CALL_MY_LOVER_AGREE_RESULT_VILLAGE_NOT>(Region);
                var resEv = new KEvent { EventId = EventId.EGS_CALL_MY_LOVER_AGREE_RESULT_NOT };
                resEv.DestPerformer.PerformerId = PerformerId.PI_GS_PROXY;
                resEv.DestPerformer.AddUID(Uid);
                var w = new KSerializer(Region);
                w.BeginWriting(resEv.Buffer);
                w.Put(not.Agree);
                w.EndWriting();
                _ = Session.SendEventAsync(resEv);
                break;
            }

            case EventId.ELG_CALL_MY_LOVER_STATE_CHANGE_FIELD_NOT:
            {
                if (ActiveUnit == null) break;
                var not = ev.GetPayload<KELG_CALL_MY_LOVER_STATE_CHANGE_FIELD_NOT>(Region);
                int targetMap = not.MapId > 0 ? not.MapId : ActiveUnit.LastPos.MapId;
                LeaveCurrentField();
                CurrentFieldMapId = targetMap;
                ActiveUnit.LastPos.MapId = targetMap;
                Fsm.TryStateTransition(KGSFSM.I_TO_FIELD_MAP, out _);
                SendPacket(EventId.EGS_CALL_MY_LOVER_STATE_CHANGE_FIELD_NOT, new KEGS_CALL_MY_LOVER_STATE_CHANGE_FIELD_NOT
                {
                    Ok = NetError.NET_OK,
                    MapId = targetMap,
                    Pos = not.Pos,
                    LastTouchLineIndex = (byte)not.LastTouchLineIndex,
                });
                break;
            }

            case EventId.ELG_CALL_MY_LOVER_FIND_USER_ROOM_ACK:
            {
                var ack = ev.GetPayload<KELG_CALL_MY_LOVER_FIND_USER_ROOM_ACK>(Region);
                SendPacket(EventId.EGS_CALL_MY_LOVER_ACK, new KPacketOK { Ok = ack.Ok });
                break;
            }

            case EventId.ELG_CALL_MY_LOVER_ROOM_NOT:
            {
                var not = ev.GetPayload<KELG_CALL_MY_LOVER_FIND_USER_ROOM_REQ>(Region);
                SendPacket(EventId.EGS_CALL_MY_LOVER_ROOM_NOT, new KEGS_CALL_MY_LOVER_ROOM_NOT
                {
                    BattleFieldId = not.BtInfo.BattleFieldId,
                    RoomUid = not.ChannelId > 0 ? not.ChannelId : not.BtInfo.BattleFieldId,
                    Pos = not.Pos,
                    LastTouchLineIndex = (byte)not.LastTouchLineIndex,
                    CalledUserUid = not.CalledUserUid,
                    CalledGsUid = not.CalledGsUid,
                    CallType = not.CallType,
                });
                break;
            }

            case EventId.ELG_CALL_MY_LOVER_AGREE_RESULT_ROOM_NOT:
            {
                var not = ev.GetPayload<KELG_CALL_MY_LOVER_AGREE_RESULT_ROOM_NOT>(Region);
                var resEv = new KEvent { EventId = EventId.EGS_CALL_MY_LOVER_AGREE_RESULT_NOT };
                resEv.DestPerformer.PerformerId = PerformerId.PI_GS_PROXY;
                resEv.DestPerformer.AddUID(Uid);
                var w = new KSerializer(Region);
                w.BeginWriting(resEv.Buffer);
                w.Put(not.Agree);
                w.EndWriting();
                _ = Session.SendEventAsync(resEv);
                break;
            }

            case EventId.ELG_CALL_MY_LOVER_JOIN_BATTLE_FIELD_NOT:
            {
                if (ActiveUnit == null) break;
                var not = ev.GetPayload<KELG_CALL_MY_LOVER_JOIN_BATTLE_FIELD_NOT>(Region);
                SendPacket(EventId.EGS_CALL_MY_LOVER_JOIN_BATTLE_FIELD_NOT, new KEGS_CALL_MY_LOVER_JOIN_BATTLE_FIELD_NOT
                {
                    Ok = NetError.NET_OK,
                    BattleFieldJoinInfo = new KBattleFieldJoinInfo { BattleFieldId = not.BattleFieldId },
                    RoomInfo = new KRoomInfo { RoomUid = not.RoomUid },
                    Pos = not.Pos,
                });
                break;
            }

            case EventId.ELG_CALL_MY_LOVER_JOIN_BATTLE_FIELD_FAILD_NOT:
            {
                var not = ev.GetPayload<KELG_CALL_MY_LOVER_JOIN_BATTLE_FIELD_FAILD_NOT>(Region);
                SendPacket(EventId.EGS_CALL_MY_LOVER_JOIN_BATTLE_FIELD_FAILD_NOT, new KPacketOK { Ok = not.Ok });
                break;
            }

            case EventId.ELG_SEARCH_UNIT_ACK:
            {
                var ack = ev.GetPayload<KELG_SEARCH_UNIT_ACK>(Region);
                SendPacket(EventId.EGS_SEARCH_UNIT_ACK, new KEGS_SEARCH_UNIT_ACK
                {
                    Ok = ack.Ok,
                    CUnitInfo = ack.CUnitInfo,
                });
                break;
            }

            case EventId.ELG_CHAT_ACK:
            {
                var ack = ev.GetPayload<KELG_CHAT_ACK>(Region);
                SendPacket(EventId.EGS_CHAT_ACK, new KPacketOK { Ok = ack.Ok });
                break;
            }

            case EventId.ELG_CHAT_NOT:
            {
                var not = ev.GetPayload<KELG_CHAT_REQ>(Region);
                SendPacket(EventId.EGS_CHAT_NOT, new KEGS_CHAT_NOT
                {
                    ChatPacketType = not.ChatPacketType,
                    SenderUnitUid = not.SenderUnitUid,
                    SenderNickName = not.SenderNickName,
                    ReceiverNickName = not.ReceiverNickName,
                    Message = not.Message,
                });
                break;
            }

            case EventId.ELG_REQUEST_FRIEND_NOT:
            {
                if (ActiveUnit == null) break;
                var not = ev.GetPayload<KELG_FRIEND_UNIT_NOT>(Region);
                SendPacket(EventId.EGS_REQUEST_FRIEND_NOT, new KEGS_REQUEST_FRIEND_NOT
                {
                    UnitUid = not.SrcUnitUid,
                    NickName = not.SrcNickName,
                });
                break;
            }

            case EventId.ELG_DELETE_FRIEND_NOT:
            {
                if (ActiveUnit == null) break;
                var not = ev.GetPayload<KELG_FRIEND_UNIT_NOT>(Region);
                _friendUids.Remove(not.SrcUnitUid);
                _blockedFriendUids.Remove(not.SrcUnitUid);
                _friends.Remove(not.SrcUnitUid);
                SendPacket(EventId.EGS_DELETE_FRIEND_NOT, new KEGS_DELETE_FRIEND_NOT { UnitUid = not.SrcUnitUid });
                break;
            }

            case EventId.ELG_DENY_FRIEND_NOT:
            {
                if (ActiveUnit == null) break;
                var not = ev.GetPayload<KELG_FRIEND_UNIT_NOT>(Region);
                _friends.Remove(not.SrcUnitUid);
                SendPacket(EventId.EGS_DENY_FRIEND_NOT, new KEGS_DENY_FRIEND_NOT
                {
                    UnitUid = not.SrcUnitUid,
                    NickName = not.SrcNickName,
                });
                break;
            }

            case EventId.ELG_UNBLOCK_FRIEND_NOT:
            {
                if (ActiveUnit == null) break;
                var not = ev.GetPayload<KELG_FRIEND_UNIT_NOT>(Region);
                _blockedFriendUids.Remove(not.SrcUnitUid);
                if (_friends.TryGetValue(not.SrcUnitUid, out var fInfo))
                    fInfo.Status = KFriendInfo.FsNormal;
                SendPacket(EventId.EGS_UNBLOCK_FRIEND_NOT, new KEGS_UNBLOCK_FRIEND_NOT { UnitUid = not.SrcUnitUid });
                break;
            }

            case EventId.ELG_FRIEND_POSITION_UPDATE_NOT:
            {
                if (ActiveUnit == null) break;
                var not = ev.GetPayload<KELG_FRIEND_POSITION_UPDATE_NOT>(Region);
                var pkt = new KEGS_FRIEND_POSITION_UPDATE_NOT();
                pkt.FriendPosition[not.UnitUid] = not.Position;
                SendPacket(EventId.EGS_FRIEND_POSITION_UPDATE_NOT, pkt);
                break;
            }

            case EventId.ELG_INVITE_PVP_ROOM_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KELG_INVITE_PVP_ROOM_REQ>(Region);
                SendPacket(EventId.EGS_INVITE_PVP_ROOM_NOT, new KEGS_INVITE_PVP_ROOM_NOT
                {
                    NickName = req.FromNickName,
                    RoomInfo = req.RoomInfo,
                });
                break;
            }

            case EventId.ELG_INVITE_PVP_ROOM_ACK:
            {
                var ack = ev.GetPayload<KELG_INVITE_PVP_ROOM_ACK>(Region);
                SendPacket(EventId.EGS_INVITE_PVP_ROOM_ACK, new KEGS_INVITE_PVP_ROOM_ACK { Ok = ack.Ok });
                break;
            }

            case EventId.ELG_CHECK_WEDDING_HALL_INFO_ACK:
            {
                var ack = ev.GetPayload<KELG_CHECK_WEDDING_HALL_INFO_ACK>(Region);
                var weddingInfo = _server.GetWeddingHall(ack.WeddingUid) ?? _server.Db.GetWeddingInfo(ack.WeddingUid) ?? new KWeddingHallInfo { WeddingUid = ack.WeddingUid, RoomUid = ack.RoomUid };
                SendPacket(EventId.EGS_JOIN_WEDDING_HALL_ACK, new KEGS_JOIN_WEDDING_HALL_ACK
                {
                    Ok = ack.Ok,
                    WeddingInfo = weddingInfo,
                    RoomInfo = new KRoomInfo { RoomUid = ack.RoomUid, RoomType = 6 },
                    UdpRelayIp = _server.PublicIp,
                    UdpRelayPort = _server.UdpRelayPort,
                });
                break;
            }

            case EventId.ELG_BREAK_UP_NOT:
            {
                if (ActiveUnit == null) break;
                var now = DateTime.UtcNow;
                RelationshipManager.AnniversaryDayUtc = now;
                RelationshipManager.SetRelationshipType(KUserRelationshipManager.RelationshipType.RtSolo);
                RelationshipManager.LoverUnitUid = 0;
                RelationshipManager.LoverNickName = string.Empty;
                RelationshipManager.LoveWord = string.Empty;
                RelationshipManager.LastRewardDateUtc = now;
                RelationshipManager.RewardTitleType = 0;
                SendPacket(EventId.EGS_BREAK_UP_NOT, new KEGS_BREAK_UP_NOT { Cost = 0 });
                if (CurrentRoomUid > 0)
                    _server.Router.Send(PerformerTypeId, ActiveUnit.UnitUid, PerformerId.PI_CN_ROOM, CurrentRoomUid,
                        EventId.ERM_BREAK_UP_SUCCESS_NOT, new KPacketOK { Ok = NetError.NET_OK });
                else if (CurrentFieldUid > 0)
                    _server.Router.Send(PerformerTypeId, ActiveUnit.UnitUid, PerformerId.PI_GS_FIELD, CurrentFieldUid,
                        EventId.ERM_BREAK_UP_SUCCESS_NOT, new KPacketOK { Ok = NetError.NET_OK });
                break;
            }

            case EventId.ELG_DIVORCE_NOT:
            {
                if (ActiveUnit == null) break;
                var now = DateTime.UtcNow;
                RelationshipManager.AnniversaryDayUtc = now;
                RelationshipManager.SetRelationshipType(KUserRelationshipManager.RelationshipType.RtSolo);
                RelationshipManager.LoverUnitUid = 0;
                RelationshipManager.LoverNickName = string.Empty;
                RelationshipManager.LoveWord = string.Empty;
                RelationshipManager.LastRewardDateUtc = now;
                RelationshipManager.RewardTitleType = 0;
                SendPacket(EventId.EGS_DIVORCE_NOT, new KEGS_BREAK_UP_NOT { Cost = 0 });
                if (CurrentRoomUid > 0)
                    _server.Router.Send(PerformerTypeId, ActiveUnit.UnitUid, PerformerId.PI_CN_ROOM, CurrentRoomUid,
                        EventId.ERM_BREAK_UP_SUCCESS_NOT, new KPacketOK { Ok = NetError.NET_OK });
                else if (CurrentFieldUid > 0)
                    _server.Router.Send(PerformerTypeId, ActiveUnit.UnitUid, PerformerId.PI_GS_FIELD, CurrentFieldUid,
                        EventId.ERM_BREAK_UP_SUCCESS_NOT, new KPacketOK { Ok = NetError.NET_OK });
                break;
            }

            case EventId.ELG_CHANGE_LOVE_WORD_NOT:
            {
                if (ActiveUnit == null) break;
                var not = ev.GetPayload<KELG_RELATIONSHIP_EVENT_NOT>(Region);
                SendPacket(EventId.EGS_CHANGE_LOVE_WORD_NOT, new KEGS_CHANGE_LOVE_WORD_NOT
                {
                    UseItemUnitUid = not.UnitUid,
                    NewLoveWord = not.LoveWord,
                });
                break;
            }

            case EventId.ELG_UPDATE_USER_UNIT_BUFF_INFO_ACK:
            {
                if (ActiveUnit == null) break;
                var ack = ev.GetPayload<KELG_UPDATE_USER_UNIT_BUFF_INFO_ACK>(Region);
                ActiveUnit.BuffInfo = ack.ActiveBuffs;
                break;
            }

            case EventId.EGB_REGISTER_SERVERSN_ACK:
            {
                var ack = ev.GetPayload<KEGB_REGISTER_SERVERSN_ACK>(Region);
                if (ack.Ok != NetError.NET_OK)
                    ReserveDestroy();
                break;
            }

            case EventId.EGB_REGISTER_SERVERSN_NOT:
            {
                var not = ev.GetPayload<KEGB_REGISTER_SERVERSN_ACK>(Region);
                if (not.Ok != NetError.NET_OK)
                {
                    SendPacket(EventId.E_KICK_USER_NOT, new KPacketOK { Ok = not.Ok });
                    ReserveDestroy();
                }
                break;
            }

            case EventId.EGB_EXCHANGE_LIMIT_ACK:
            {
                var ack = ev.GetPayload<KEGB_EXCHANGE_LIMIT_ACK>(Region);
                if (ack.Ok != NetError.NET_OK)
                    SendPacket(EventId.EGS_ITEM_EXCHANGE_ACK, new KEGS_ITEM_EXCHANGE_ACK { Ok = ack.Ok });
                break;
            }

            case EventId.ECN_CHECK_CHANNEL_CHANGE_ACK:
            {
                var ack = ev.GetPayload<KECN_CHECK_CHANNEL_CHANGE_ACK>(Region);
                SendPacket(EventId.EGS_CHECK_CHANNEL_CHANGE_ACK, new KPacketOK { Ok = ack.Ok });
                break;
            }

            case EventId.ERM_OPEN_ROOM_ACK:
            {
                if (ActiveUnit == null) break;
                var ack = ev.GetPayload<KERM_OPEN_ROOM_ACK>(Region);
                if (ack.Ok == NetError.NET_OK)
                {
                    CurrentRoomUid = ack.RoomInfo.RoomUid;
                    Fsm.TryStateTransition(KGSFSM.I_TO_ROOM, out _);
                }
                SendPacket(EventId.EGS_CREATE_ROOM_ACK, new KEGS_CREATE_ROOM_ACK
                {
                    Ok = ack.Ok,
                    RoomInfo = ack.RoomInfo,
                    Slots = ack.Slots,
                });
                break;
            }

            case EventId.ERM_JOIN_ROOM_ACK:
            {
                if (ActiveUnit == null) break;
                var ack = ev.GetPayload<KERM_JOIN_ROOM_ACK>(Region);
                if (ack.Ok == NetError.NET_OK)
                {
                    CurrentRoomUid = ack.RoomInfo.RoomUid;
                    Fsm.TryStateTransition(KGSFSM.I_TO_ROOM, out _);
                }
                SendPacket(EventId.EGS_JOIN_ROOM_ACK, new KEGS_CREATE_ROOM_ACK
                {
                    Ok = ack.Ok,
                    RoomInfo = ack.RoomInfo,
                    Slots = ack.Slots,
                });
                break;
            }

            // ---- Cross-Server Destination Handlers (Global / Center / Login -> GSUser) ----
            case EventId.EGB_AUTO_PARTY_MAKING_SUCCESS_NOT:
            {
                var not = ev.GetPayload<KEGS_AUTO_PARTY_MAKING_SUCCESS_NOT>(Region);
                SendPacket(EventId.EGS_AUTO_PARTY_MAKING_SUCCESS_NOT, not);
                break;
            }

            case EventId.EGB_AUTO_PARTY_CLOSE_NOT:
            {
                var not = ev.GetPayload<KEGS_AUTO_PARTY_CLOSE_NOT>(Region);
                SendPacket(EventId.EGS_AUTO_PARTY_CLOSE_NOT, not);
                break;
            }

            case EventId.EGB_CANCEL_AUTO_PARTY_MAKING_ACK:
            {
                var ack = ev.GetPayload<KPacketOK>(Region);
                SendPacket(EventId.EGS_CANCEL_AUTO_PARTY_MAKING_ACK, ack);
                break;
            }

            case EventId.EGB_AUTO_PARTY_WAIT_USER_COUNT_NOT:
            {
                var pkt = ev.GetPayload<KEGS_AUTO_PARTY_WAIT_USER_COUNT_NOT>(Region);
                SendPacket(EventId.EGS_AUTO_PARTY_WAIT_USER_COUNT_NOT, pkt);
                break;
            }

            case EventId.EGB_MATCH_MAKING_SUCCESS_NOT:
            {
                var not = ev.GetPayload<KEGS_MATCH_MAKING_SUCCESS_NOT>(Region);
                SendPacket(EventId.EGS_MATCH_MAKING_SUCCESS_NOT, not);
                break;
            }

            case EventId.EGB_CANCEL_MATCH_MAKING_ACK:
            {
                var ack = ev.GetPayload<KPacketOK>(Region);
                SendPacket(EventId.EGS_CANCEL_MATCH_MAKING_ACK, ack);
                break;
            }

            case EventId.EGB_ADMIN_NOTIFY_HERO_PVP_USER_ACK:
            {
                var ack = ev.GetPayload<KPacketOK>(Region);
                SendPacket(EventId.EGS_ADMIN_NOTIFY_HERO_PVP_USER_ACK, ack);
                break;
            }

            case EventId.EGB_ADMIN_NOTIFY_HERO_PVP_USER_NOT:
            {
                var not = ev.GetPayload<KEGS_ADMIN_NOTIFY_HERO_PVP_USER_REQ>(Region);
                SendPacket(EventId.EGS_ADMIN_NOTIFY_HERO_PVP_USER_NOT, not);
                break;
            }

            case EventId.EGB_INVITE_PARTY_FIND_RECEIVER_ACK:
            {
                var ack = ev.GetPayload<KELG_INVITE_PARTY_FIND_RECEIVER_ACK>(Region);
                SendPacket(EventId.EGS_INVITE_PARTY_ACK, new KEGS_INVITE_PARTY_ACK
                {
                    Ok = ack.Ok,
                    ReceiverUid = ack.ReceiverUnitUid,
                    UdpRelayIp = _server.PublicIp,
                    UdpRelayPort = _server.UdpRelayPort,
                });
                break;
            }

            case EventId.ELG_ADMIN_SHOW_ED_CHEAT_ACK:
                SendPacket(EventId.EGS_ADMIN_SHOW_ED_CHEAT_ACK, ev.GetPayload<KEGS_ADMIN_SHOW_ED_CHEAT_ACK>(Region));
                break;

            case EventId.ELG_GET_GUILD_USER_LIST_ACK:
                SendPacket(EventId.EGS_GET_GUILD_USER_LIST_ACK, ev.GetPayload<KEGS_GET_GUILD_USER_LIST_ACK>(Region));
                break;

            case EventId.ELG_INVITE_GUILD_ACK:
                SendPacket(EventId.EGS_INVITE_GUILD_ACK, ev.GetPayload<KEGS_INVITE_GUILD_ACK>(Region));
                break;

            case EventId.ELG_INVITE_GUILD_FIND_USER_REQ:
            {
                if (ActiveUnit == null) break;
                var inviteNot = ev.GetPayload<KEGS_INVITE_GUILD_NOT>(Region);
                if (ActiveUnit.UserGuildInfo.GuildUid > 0 || GameOption.IsDenyInviteGuild)
                    break;
                SendPacket(EventId.EGS_INVITE_GUILD_NOT, inviteNot);
                break;
            }

            case EventId.ELG_INVITE_GUILD_REPLY_ACK:
                SendPacket(EventId.EGS_INVITE_GUILD_REPLY_ACK, ev.GetPayload<KEGS_JOIN_GUILD_ACK>(Region));
                break;

            case EventId.ELG_INVITE_GUILD_REPLY_DENY_NOT:
                SendPacket(EventId.EGS_INVITE_GUILD_REPLY_DENY_NOT, ev.GetPayload<KEGS_INVITE_GUILD_REPLY_DENY_NOT>(Region));
                break;

            case EventId.ELG_JOIN_GUILD_ACK:
            {
                var ack = ev.GetPayload<KEGS_JOIN_GUILD_ACK>(Region);
                if (ActiveUnit != null && ack.Ok == NetError.NET_OK && ack.GuildInfo.GuildUid > 0)
                {
                    ActiveUnit.UserGuildInfo.GuildUid = ack.GuildInfo.GuildUid;
                    ActiveUnit.UserGuildInfo.GuildName = ack.GuildInfo.GuildName;
                }
                SendPacket(EventId.EGS_JOIN_GUILD_SUCCESS_NOT, ack);
                break;
            }

            case EventId.ELG_CHECK_CHANGE_GUILD_MEMBER_GRADE_ACK:
                SendPacket(EventId.EGS_CHANGE_GUILD_MEMBER_GRADE_ACK, ev.GetPayload<KEGS_CHANGE_GUILD_MEMBER_GRADE_ACK>(Region));
                break;

            case EventId.ELG_CHANGE_GUILD_NAME_CHECK_ACK:
                SendPacket(EventId.EGS_CHANGE_GUILD_NAME_CHECK_ACK, ev.GetPayload<KPacketOK>(Region));
                break;

            case EventId.ELG_CHANGE_GUILD_NAME_ACK:
                SendPacket(EventId.EGS_CHANGE_GUILD_NAME_ACK, ev.GetPayload<KEGS_CHANGE_GUILD_NAME_ACK>(Region));
                break;

            case EventId.ELG_GET_GUILD_SKILL_ACK:
                SendPacket(EventId.EGS_GET_GUILD_SKILL_ACK, ev.GetPayload<KPacketOK>(Region));
                break;

            case EventId.ELG_RESET_GUILD_SKILL_ACK:
                SendPacket(EventId.EGS_RESET_GUILD_SKILL_ACK, ev.GetPayload<KEGS_RESET_GUILD_SKILL_ACK>(Region));
                break;

            case EventId.ELG_INIT_GUILD_SKILL_ACK:
                SendPacket(EventId.EGS_INIT_GUILD_SKILL_ACK, ev.GetPayload<KEGS_INIT_GUILD_SKILL_ACK>(Region));
                break;

            case EventId.ELG_ADMIN_GET_GUILD_SKILL_POINT_ACK:
                SendPacket(EventId.EGS_ADMIN_GET_GUILD_SKILL_POINT_ACK, ev.GetPayload<KEGS_ADMIN_GET_GUILD_SKILL_POINT_ACK>(Region));
                break;

            case EventId.ELG_APPLY_JOIN_GUILD_ACK:
                SendPacket(EventId.EGS_APPLY_JOIN_GUILD_ACK, ev.GetPayload<KPacketOK>(Region));
                break;

            case EventId.ELG_GET_APPLY_JOIN_GUILD_LIST_ACK:
                SendPacket(EventId.EGS_GET_APPLY_JOIN_GUILD_LIST_ACK, ev.GetPayload<KEGS_GET_APPLY_JOIN_GUILD_LIST_ACK>(Region));
                break;

            case EventId.ELG_ACCEPT_JOIN_GUILD_ACK:
                SendPacket(EventId.EGS_ACCEPT_JOIN_GUILD_ACK, ev.GetPayload<KEGS_ACCEPT_JOIN_GUILD_ACK>(Region));
                break;

            case EventId.ELG_DELETE_APPLY_JOIN_GUILD_ACK:
                SendPacket(EventId.EGS_DELETE_APPLY_JOIN_GUILD_ACK, ev.GetPayload<KPacketOK>(Region));
                break;

            case EventId.EPM_CHAT_NOT:
            {
                var not = ev.GetPayload<KEPM_CHAT_REQ>(Region);
                SendPacket(EventId.EGS_CHAT_NOT, new KEGS_CHAT_NOT
                {
                    SenderUnitUid = not.SenderUnitUid,
                    SenderNickName = not.SenderNickName,
                    Message = not.Message,
                });
                break;
            }

            case EventId.EPM_CHAT_ACK:
                SendPacket(EventId.EGS_CHAT_ACK, ev.GetPayload<KPacketOK>(Region));
                break;

            case EventId.EPM_PARTY_BAN_USER_NOT:
                CurrentPartyUid = 0;
                SendPacket(EventId.EGS_PARTY_BAN_USER_NOT, ev.GetPayload<KPacketOK>(Region));
                break;

            case EventId.EPM_PARTY_BAN_USER_ACK:
                SendPacket(EventId.EGS_PARTY_BAN_USER_ACK, ev.GetPayload<KPacketOK>(Region));
                break;

            case EventId.EPM_PARTY_CHANGE_DUNGEON_NOT:
            {
                var not = ev.GetPayload<KEPM_PARTY_CHANGE_DUNGEON_REQ>(Region);
                SendPacket(EventId.EGS_PARTY_CHANGE_DUNGEON_NOT, new KEGS_PARTY_CHANGE_DUNGEON_REQ
                {
                    DungeonId = not.DungeonId,
                    DifficultyLevel = not.DifficultyLevel,
                    DungeonMode = not.DungeonMode,
                });
                break;
            }

            case EventId.EPM_PARTY_CHANGE_DUNGEON_ACK:
                SendPacket(EventId.EGS_PARTY_CHANGE_DUNGEON_ACK, ev.GetPayload<KPacketOK>(Region));
                break;

            case EventId.EPM_PARTY_CHANGE_READY_NOT:
                SendPacket(EventId.EGS_PARTY_CHANGE_READY_NOT, ev.GetPayload<KEGS_PARTY_CHANGE_READY_NOT>(Region));
                break;

            case EventId.EPM_PARTY_CHANGE_READY_ACK:
                SendPacket(EventId.EGS_PARTY_CHANGE_READY_ACK, ev.GetPayload<KPacketOK>(Region));
                break;

            case EventId.EPM_PARTY_GAME_START_ACK:
                SendPacket(EventId.EGS_PARTY_GAME_START_ACK, ev.GetPayload<KPacketOK>(Region));
                break;

            case EventId.EPM_CHANGE_PARTY_FEVER_STATE_NOT:
            {
                var not = ev.GetPayload<KEGS_CHANGE_PARTY_FEVER_STATE_NOT>(Region);
                SendPacket(EventId.EGS_CHANGE_PARTY_FEVER_STATE_NOT, not);
                break;
            }

            case EventId.ERM_BAN_USER_NOT:
                CurrentRoomUid = 0;
                SendPacket(EventId.EGS_BAN_USER_NOT, ev.GetPayload<KPacketOK>(Region));
                break;

            case EventId.ERM_BAN_USER_ACK:
                SendPacket(EventId.EGS_BAN_USER_ACK, ev.GetPayload<KPacketOK>(Region));
                break;

            case EventId.ERM_CHANGE_MAP_NOT:
                SendPacket(EventId.EGS_CHANGE_MAP_NOT, new KEGS_CHANGE_MAP_NOT { WorldId = ev.GetPayload<KEGS_CHANGE_MAP_REQ>(Region).WorldId });
                break;

            case EventId.ERM_CHANGE_MAP_ACK:
                SendPacket(EventId.EGS_CHANGE_MAP_ACK, ev.GetPayload<KPacketOK>(Region));
                break;

            case EventId.ERM_CHANGE_READY_NOT:
            {
                var not = ev.GetPayload<KEGS_CHANGE_READY_REQ>(Region);
                SendPacket(EventId.EGS_CHANGE_READY_NOT, new KEGS_CHANGE_READY_NOT { UnitUid = ActiveUnit?.UnitUid ?? 0, Ready = not.Ready });
                break;
            }

            case EventId.ERM_CHANGE_READY_ACK:
                SendPacket(EventId.EGS_CHANGE_READY_ACK, ev.GetPayload<KPacketOK>(Region));
                break;

            case EventId.ERM_CHANGE_TEAM_NOT:
            {
                var not = ev.GetPayload<KEGS_CHANGE_TEAM_REQ>(Region);
                SendPacket(EventId.EGS_CHANGE_TEAM_NOT, new KEGS_CHANGE_TEAM_NOT { UnitUid = ActiveUnit?.UnitUid ?? 0, TeamNum = not.TeamNum });
                break;
            }

            case EventId.ERM_CHANGE_TEAM_ACK:
                SendPacket(EventId.EGS_CHANGE_TEAM_ACK, ev.GetPayload<KPacketOK>(Region));
                break;

            case EventId.ERM_BUFF_SKILL_INFO_CONVEY_NOT:
                SendPacket(EventId.EGS_BUFF_SKILL_INFO_CONVEY_NOT, ev.GetPayload<KEGS_BUFF_SKILL_INFO_CONVEY_NOT>(Region));
                break;

            case EventId.ERM_OPEN_PSHOP_AGENCY_ACK:
            {
                var ack = ev.GetPayload<KERM_OPEN_PSHOP_AGENCY_ACK>(Region);
                if (ack.Ok == NetError.NET_OK && ack.ShopUid > 0)
                {
                    PShopAgencyUid = ack.ShopUid;
                    _pshopAgencyRemoteOpened = true;
                }
                SendPacket(EventId.EGS_CREATE_PSHOP_AGENCY_ACK, new KEGS_CREATE_PSHOP_AGENCY_ACK
                {
                    Ok = ack.Ok,
                    PersonalShopUid = ack.ShopUid,
                });
                break;
            }

            case EventId.ERM_PICK_UP_FROM_PSHOP_AGENCY_ACK:
            {
                var ack = ev.GetPayload<KERM_PICK_UP_FROM_PSHOP_AGENCY_ACK>(Region);
                if (ActiveUnit != null && ack.Ok == NetError.NET_OK && ack.PickedEd > 0)
                    ActiveUnit.Ed = (int)Math.Min(int.MaxValue, ActiveUnit.Ed + ack.PickedEd);
                SendPacket(EventId.EGS_PICK_UP_FROM_PSHOP_AGENCY_ACK, new KEGS_PICK_UP_FROM_PSHOP_AGENCY_ACK
                {
                    Ok = ack.Ok,
                    Ed = ActiveUnit?.Ed ?? 0,
                });
                break;
            }

            // ---- Additional Cross-Server Destination Handlers (Login / Center / Global -> GSUser) ----
            case EventId.EGB_REQUEST_AUTO_PARTY_MAKING_ACK:
                SendPacket(EventId.EGS_AUTO_PARTY_DUNGEON_GAME_ACK, new KEGS_AUTO_PARTY_DUNGEON_GAME_ACK
                {
                    Ok = ev.GetPayload<KPacketOK>(Region).Ok,
                });
                break;

            case EventId.EGB_REQUEST_MATCH_MAKING_ACK:
                SendPacket(EventId.EGS_REQUEST_MATCH_MAKING_ACK, ev.GetPayload<KPacketOK>(Region));
                break;

            case EventId.EGB_MATCH_CLOSE_NOT:
                SendPacket(EventId.EGS_MATCH_CLOSE_NOT, ev.GetPayload<KEGS_MATCH_CLOSE_NOT>(Region));
                break;

            case EventId.ELG_CHECK_MACHINE_ID_ACK:
            {
                var ack = ev.GetPayload<KEGS_CHECK_MACHINE_ID_ACK>(Region);
                SendPacket(EventId.EGS_CHECK_MACHINE_ID_ACK, ack);
                if (ack.Ok != NetError.NET_OK)
                    ReserveDestroy();
                break;
            }

            case EventId.ELG_GET_GUILD_SKILL_IN_BOARD_ACK:
                SendPacket(EventId.EGS_GET_GUILD_SKILL_IN_BOARD_ACK, ev.GetPayload<KEGS_GET_GUILD_SKILL_IN_BOARD_ACK>(Region));
                break;

            case EventId.ELG_MODIFY_REG_GUILD_AD_ACK:
            {
                var ack = ev.GetPayload<KELG_MODIFY_REG_GUILD_AD_ACK>(Region);
                if (ack.Ok != NetError.NET_OK && ack.Cost > 0 && ActiveUnit != null)
                    ActiveUnit.Ed += ack.Cost;
                SendPacket(EventId.EGS_MODIFY_REG_GUILD_AD_ACK, new KEGS_MODIFY_REG_GUILD_AD_ACK
                {
                    Ok = ack.Ok,
                    Ed = ActiveUnit?.Ed ?? 0,
                    Period = ack.Period,
                    AdMessage = ack.AdMessage,
                });
                break;
            }

            case EventId.ELG_CREATE_GUILD_RESULT_ACK:
                SendPacket(EventId.EGS_CREATE_GUILD_ACK, new KEGS_CREATE_GUILD_ACK { Ok = ev.GetPayload<KPacketOK>(Region).Ok });
                break;

            case EventId.ELG_GET_RECOMMEND_USER_LIST_ACK:
                SendPacket(EventId.EGS_GET_RECOMMEND_USER_LIST_ACK, ev.GetPayload<KEGS_GET_RECOMMEND_USER_LIST_ACK>(Region));
                break;

            case EventId.ELG_GET_RECRUIT_RECRUITER_LIST_ACK:
                SendPacket(EventId.EGS_GET_RECRUIT_RECRUITER_LIST_ACK, ev.GetPayload<KEGS_GET_RECRUIT_RECRUITER_LIST_ACK>(Region));
                break;

            case EventId.ELG_RELATIONSHIP_INFO_ACK:
                SendPacket(EventId.EGS_RELATIONSHIP_INFO_ACK, ev.GetPayload<KEGS_RELATIONSHIP_INFO_ACK>(Region));
                break;

            case EventId.ELG_COUPLE_PROPOSE_CHECK_CONNECT_USER_ACK:
                SendPacket(EventId.EGS_COUPLE_PROPOSE_ACK, ev.GetPayload<KEGS_COUPLE_PROPOSE_ACK>(Region));
                break;

            case EventId.ELG_COUPLE_PROPOSE_NOT:
                SendPacket(EventId.EGS_COUPLE_PROPOSE_NOT, ev.GetPayload<KEGS_COUPLE_PROPOSE_REQ>(Region));
                break;

            case EventId.ELG_EVENT_PROPOSE_CHECK_CONNECT_USER_ACK:
                SendPacket(EventId.EGS_EVENT_PROPOSE_ACK, ev.GetPayload<KEGS_COUPLE_PROPOSE_ACK>(Region));
                break;

            case EventId.ELG_EVENT_PROPOSE_NOT:
                SendPacket(EventId.EGS_EVENT_PROPOSE_NOT, ev.GetPayload<KEGS_COUPLE_PROPOSE_REQ>(Region));
                break;

            case EventId.EBILL_USE_COUPON_ACK:
            {
                var ack = ev.GetPayload<KEBILL_USE_COUPON_ACK>(Region);
                SendPacket(EventId.EGS_APPLY_COUPON_ACK, new KEGS_APPLY_COUPON_ACK { Ok = ack.Ok });
                break;
            }

            case EventId.ELG_ADMIN_KICK_USER_ACK:
                SendPacket(EventId.EGS_ADMIN_KICK_USER_ACK, ev.GetPayload<KEGS_ADMIN_KICK_USER_ACK>(Region));
                break;

            case EventId.ELG_CREATE_GUILD_ACK:
            {
                var ack = ev.GetPayload<KELG_CREATE_GUILD_ACK>(Region);
                if (ack.Ok == NetError.NET_OK && ActiveUnit != null)
                {
                    ActiveUnit.UserGuildInfo.GuildUid = ack.GuildInfo.GuildUid;
                    ActiveUnit.UserGuildInfo.GuildName = ack.GuildInfo.GuildName;
                }
                SendPacket(EventId.EGS_CREATE_GUILD_ACK, new KEGS_CREATE_GUILD_ACK
                {
                    Ok = ack.Ok,
                    GuildInfo = ack.GuildInfo,
                });
                break;
            }

            case EventId.ELG_GET_GUILD_INFO_ACK:
            {
                KEGS_GUILD_INFO_NOT not;
                if (ev.InProcessPayload is KEGS_GUILD_INFO_NOT directNot)
                {
                    not = directNot;
                }
                else
                {
                    var ack = ev.GetPayload<KELG_GET_GUILD_INFO_ACK>(Region);
                    not = new KEGS_GUILD_INFO_NOT
                    {
                        GuildInfo = ack.GuildInfo,
                        GuildSkillInfo = ack.GuildSkillInfo,
                    };
                }
                if (ActiveUnit != null)
                {
                    ActiveUnit.UserGuildInfo.GuildUid = not.GuildInfo.GuildUid;
                    ActiveUnit.UserGuildInfo.GuildName = not.GuildInfo.GuildName;
                }
                SendPacket(EventId.EGS_GUILD_INFO_NOT, not);
                break;
            }

            case EventId.ELG_LOCAL_RANKING_RESET_NOT:
                LocalRankingInfo.ResetUnitInfo();
                break;

            case EventId.ELG_CHECK_MODULE_INFO_UPDATE_NOT:
            {
                var res = ev.GetPayload<KPacketOK>(Region);
                if (res.Ok != NetError.NET_OK)
                {
                    SendPacket(EventId.EGS_SERVER_CHECK_HACK_USER_REQ, new KEGS_SERVER_CHECK_HACK_USER_REQ
                    {
                        HackingCheckType = KEGS_SERVER_CHECK_HACK_USER_REQ.HctCheckMachineId,
                    });
                }
                break;
            }

            case EventId.ELG_USER_DISCONNECT_ACK:
            {
                int ackOk = NetError.NET_OK;
                if (ev.InProcessPayload is KELG_USER_DISCONNECT_ACK dack)
                    ackOk = dack.Ok;
                else if (ev.Buffer.Length > 0)
                    ackOk = ev.GetPayload<KELG_USER_DISCONNECT_ACK>(Region).Ok;

                EventId ackEventId = _pendingDisconnectEventId == EventId.EGS_DISCONNECT_FOR_SERVER_SELECT_REQ
                    ? EventId.EGS_DISCONNECT_FOR_SERVER_SELECT_ACK
                    : EventId.EGS_DISCONNECT_FOR_CHANNEL_CHANGE_ACK;
                SendPacket(ackEventId, new KPacketOK { Ok = ackOk });
                ReserveDestroy();
                break;
            }

            case EventId.ECN_MEGAPHONE_MESSAGE_NOT:
                SendPacket(EventId.EGS_MEGAPHONE_MESSAGE_NOT, ev.GetPayload<KEGS_MEGAPHONE_MESSAGE_NOT>(Region));
                break;

            case EventId.ECN_NOTIFY_MSG_NOT:
                SendPacket(EventId.EGS_NOTIFY_MSG_NOT, ev.GetPayload<KEGS_NOTIFY_MSG_NOT>(Region));
                break;

            case EventId.ECN_UDP_KICK_GAMEEDIT_NOT:
                SendPacket(EventId.EGS_UDP_KICK_GAMEEDIT_NOT, ev.GetPayload<KEGS_UDP_KICK_GAMEEDIT_NOT>(Region));
                break;

            case EventId.ECN_WORLD_TRIGGER_RELOCATION_NOT:
                SendPacket(EventId.EGS_WORLD_TRIGGER_RELOCATION_NOT, ev.GetPayload<KEGS_WORLD_TRIGGER_RELOCATION_NOT>(Region));
                break;

            case EventId.ECN_PARTY_BUFF_UPDATE_NOT:
            {
                var buffNot = ev.GetPayload<KECN_PARTY_BUFF_UPDATE_NOT>(Region);
                foreach (var b in buffNot.BuffInfos)
                {
                    if (b.FactorInfo.FactorId > 0)
                        ServerBuffManager.ActivateBuff(new KUserServerBuffManager.ServerBuffInfo { BuffId = b.FactorInfo.FactorId });
                }
                if (ActiveUnit != null && buffNot.BuffInfos.Count > 0)
                {
                    ActiveUnit.BuffInfo = ServerBuffManager.ToPacketBuffInfos();
                    _server.Router.Send(PerformerTypeId, ActiveUnit.UnitUid, PerformerId.PI_LOGIN_SERVER, _server.LoginServerUid,
                        EventId.ELG_UPDATE_USER_UNIT_BUFF_INFO_REQ, new KELG_UPDATE_USER_UNIT_BUFF_INFO_REQ
                        {
                            UnitUid = ActiveUnit.UnitUid,
                            ActivateBuff = buffNot.BuffInfos,
                        });
                }
                break;
            }

            case EventId.EGB_THE_GATE_OF_DARKNESS_INDIVIDUAL_BUFF_NOT:
            {
                var buffNot = ev.GetPayload<KEGB_THE_GATE_OF_DARKNESS_INDIVIDUAL_BUFF_NOT>(Region);
                var deactivated = new List<int>
                {
                    (int)KBuffManager.BuffTempletId.HeroOfEliosLevel1,
                    (int)KBuffManager.BuffTempletId.HeroOfEliosLevel2,
                    (int)KBuffManager.BuffTempletId.HeroOfEliosLevel3,
                    (int)KBuffManager.BuffTempletId.HeroOfEliosLevel4,
                };
                foreach (int bid in deactivated)
                    ServerBuffManager.DeactivateBuff(bid);

                int mappedBuffId = buffNot.BuffId switch
                {
                    1 => (int)KBuffManager.BuffTempletId.HeroOfEliosLevel1,
                    2 => (int)KBuffManager.BuffTempletId.HeroOfEliosLevel2,
                    3 => (int)KBuffManager.BuffTempletId.HeroOfEliosLevel3,
                    4 => (int)KBuffManager.BuffTempletId.HeroOfEliosLevel4,
                    _ => buffNot.BuffId,
                };
                if (mappedBuffId > 0)
                {
                    ServerBuffManager.ActivateBuff(new KUserServerBuffManager.ServerBuffInfo
                    {
                        BuffId = mappedBuffId,
                        ExpireUtc = buffNot.RemainSeconds > 0
                            ? DateTime.UtcNow.AddSeconds(buffNot.RemainSeconds)
                            : DateTime.UtcNow.AddHours(1),
                    });
                }
                if (ActiveUnit != null)
                {
                    ActiveUnit.BuffInfo = ServerBuffManager.ToPacketBuffInfos();
                    _server.Router.Send(PerformerTypeId, ActiveUnit.UnitUid, PerformerId.PI_LOGIN_SERVER, _server.LoginServerUid,
                        EventId.ELG_UPDATE_USER_UNIT_BUFF_INFO_REQ, new KELG_UPDATE_USER_UNIT_BUFF_INFO_REQ
                        {
                            UnitUid = ActiveUnit.UnitUid,
                            ActivateBuff = ActiveUnit.BuffInfo.Where(b => b.FactorInfo.FactorId == mappedBuffId).ToList(),
                            DeactivateBuff = deactivated,
                        });
                }
                break;
            }

            case EventId.EGB_UPDATE_WORLD_BUFF_NOT:
            {
                var buff = ev.GetPayload<KEGS_UPDATE_WORLD_BUFF_NOT>(Region);
                if (buff.SuccessWorldBuff == 0)
                {
                    ServerBuffManager.DeactivateBuff((int)KBuffManager.BuffTempletId.ThanksOfResidents);
                    ServerBuffManager.DeactivateBuff((int)KBuffManager.BuffTempletId.BreathOfDragon);
                    ServerBuffManager.DeactivateBuff((int)KBuffManager.BuffTempletId.BlessingOfEl);
                    ServerBuffManager.DeactivateBuff((int)KBuffManager.BuffTempletId.Defense2013Debuff);
                }
                else
                {
                    ServerBuffManager.ActivateBuff(new KUserServerBuffManager.ServerBuffInfo
                    {
                        BuffId = buff.SuccessWorldBuff,
                        ExpireUtc = buff.WorldBuffDurationTime > 0
                            ? DateTime.UtcNow.AddSeconds(buff.WorldBuffDurationTime)
                            : DateTime.UtcNow.AddHours(1),
                    });
                }
                if (ActiveUnit != null)
                    ActiveUnit.BuffInfo = ServerBuffManager.ToPacketBuffInfos();
                SendPacket(EventId.EGS_UPDATE_WORLD_BUFF_NOT, buff);
                break;
            }

            case EventId.ELG_PRESENT_CASH_ITEM_ACK:
            {
                var ack = ev.GetPayload<KELG_PRESENT_CASH_ITEM_ACK>(Region);
                SendPacket(EventId.EGS_PRESENT_CASH_ITEM_ACK, new KEGS_PRESENT_CASH_ITEM_ACK
                {
                    Ok = ack.Ok,
                    PurchaseReqInfo = ack.PurchaseReqInfo,
                });
                break;
            }

            case EventId.ELG_REGISTER_USER_ACK:
            {
                var ok = ev.GetPayload<KPacketOK>(Region).Ok;
                if (ok != NetError.NET_OK)
                {
                    ReserveDestroy();
                }
                else if (Fsm.CurrentStateId != KGSFSM.S_SERVER_SELECT)
                {
                    Fsm.TryStateTransition(KGSFSM.I_TO_SERVER_SELECT, out _);
                }
                break;
            }

            case EventId.ELG_CHANNEL_CHANGE_REGISTER_USER_ACK:
            {
                var ok = ev.GetPayload<KPacketOK>(Region).Ok;
                if (ok != NetError.NET_OK)
                {
                    SendPacket(EventId.EGS_CONNECT_CHANNEL_CHANGE_ACK, new KEGS_CONNECT_CHANNEL_CHANGE_ACK { Ok = ok });
                    ReserveDestroy();
                }
                else
                {
                    Fsm.TryStateTransition(KGSFSM.I_TO_SERVER_SELECT, out _);
                    SendPacket(EventId.EGS_CONNECT_CHANNEL_CHANGE_ACK, new KEGS_CONNECT_CHANNEL_CHANGE_ACK { Ok = NetError.NET_OK });
                }
                break;
            }

            case EventId.ELG_CHANGE_GUILD_NAME_TIME_CHECK_ACK:
            {
                var ok = ev.GetPayload<KPacketOK>(Region).Ok;
                if (ok != NetError.NET_OK)
                {
                    GuildManager.IsGuildNameChanging = false;
                    SendPacket(EventId.EGS_CHANGE_GUILD_NAME_ACK, new KEGS_CHANGE_GUILD_NAME_ACK { Ok = ok });
                }
                break;
            }

            case EventId.ELG_FIND_FRIEND_UNIT_INFO_ACK:
            {
                var fack = ev.GetPayload<KELG_FIND_FRIEND_UNIT_INFO_ACK>(Region);
                if (fack.Ok == NetError.NET_OK)
                {
                    foreach (var cu in fack.FriendUnitInfos)
                    {
                        if (cu.UnitUid > 0)
                        {
                            _friendUids.Add(cu.UnitUid);
                            _friends[cu.UnitUid] = new KFriendInfo
                            {
                                UnitUid = cu.UnitUid,
                                NickName = cu.CharName,
                                Status = 1,
                            };
                        }
                    }
                }
                var first = fack.FriendUnitInfos.FirstOrDefault();
                SendPacket(EventId.EGS_UPDATE_FRIEND_INFO_ACK, new KEGS_UPDATE_FRIEND_INFO_ACK
                {
                    Ok = fack.Ok,
                    FriendUnitUid = first?.UnitUid ?? 0,
                    FriendNickName = first?.CharName ?? string.Empty,
                    ChannelId = first?.ChannelId ?? 0,
                });
                break;
            }

            case EventId.ELG_SEARCH_BLACK_LIST_UNIT_ACK:
            {
                if (ActiveUnit == null) break;
                var ack = ev.GetPayload<KELG_SEARCH_BLACK_LIST_UNIT_ACK>(Region);
                if (ack.Ok != NetError.NET_OK)
                {
                    SendPacket(EventId.EGS_NEW_BLACKLIST_USER_ACK, new KEGS_NEW_BLACKLIST_USER_ACK { Ok = ack.Ok });
                    break;
                }
                if (BlackList.VerifyBlackListUser(ack.UnitUid))
                {
                    SendPacket(EventId.EGS_NEW_BLACKLIST_USER_ACK, new KEGS_NEW_BLACKLIST_USER_ACK { Ok = NetError.ERR_BLACKLIST_01 });
                    break;
                }
                var target = _server.Db.GetUnitByUid(Region, ack.UnitUid);
                string nick = target?.NickName ?? string.Empty;
                if (ack.UnitUid == ActiveUnit.UnitUid || !BlackList.IsAddPossible() || !BlackList.AddNewBlackListUser(ack.UnitUid, nick))
                {
                    SendPacket(EventId.EGS_NEW_BLACKLIST_USER_ACK, new KEGS_NEW_BLACKLIST_USER_ACK { Ok = NetError.ERR_BLACKLIST_00 });
                    break;
                }
                _server.Db.AddBlackListUser(ActiveUnit.UnitUid, ack.UnitUid, nick);
                SendPacket(EventId.EGS_NEW_BLACKLIST_USER_ACK, new KEGS_NEW_BLACKLIST_USER_ACK
                {
                    Ok = NetError.NET_OK,
                    ChatBlackListUnit = new KChatBlackListUnit { UnitUid = ack.UnitUid, NickName = nick },
                });
                break;
            }

            case EventId.ELG_WEDDING_ITEM_FIND_INFO_ACK:
                if (ev.InProcessPayload is IKSerializable wia)
                    SendPacket(EventId.EGS_WEDDING_ITEM_INFO_ACK, wia);
                break;

            case EventId.EPM_CHECK_DUNGEON_GAME_PLAY_CONDITION_ACK:
            {
                var dpc = ev.GetPayload<KEPM_CHECK_DUNGEON_GAME_PLAY_CONDITION_ACK>(Region);
                if (dpc.Ok != NetError.NET_OK)
                    SendPacket(EventId.EGS_ERROR_POPUP_NOT, new KEGS_ERROR_POPUP_NOT { Ok = dpc.Ok });
                break;
            }

            case EventId.EPM_CHECK_FOR_AUTO_PARTY_MAKING_ACK:
            {
                var apm = ev.GetPayload<KEPM_CHECK_FOR_AUTO_PARTY_MAKING_ACK>(Region);
                if (apm.Ok != NetError.NET_OK)
                    SendPacket(EventId.EGS_AUTO_PARTY_DUNGEON_GAME_ACK, new KEGS_AUTO_PARTY_DUNGEON_GAME_ACK
                    {
                        Ok = apm.Ok,
                        FailUserNickName = apm.FailUserNickName,
                    });
                break;
            }

            case EventId.EPM_PARTY_GAME_START_OPINION_CHECK_REPLY_NOT:
                SendPacket(EventId.EGS_PARTY_GAME_START_OPINION_CHECK_REPLY_NOT,
                    ev.GetPayload<KEGS_PARTY_GAME_START_OPINION_CHECK_REPLY_NOT>(Region));
                break;

            case EventId.EPM_PARTY_CHANGE_HOST_NOT:
            {
                var not = ev.GetPayload<KEPM_CHECK_PARTY_CHANGE_HOST_ACK>(Region);
                SendPacket(EventId.EGS_PARTY_CHANGE_HOST_NOT, new KEGS_PARTY_CHANGE_HOST_NOT
                {
                    BeforeUid = not.OldHostUnitUid,
                    AfterUid = not.NewHostUnitUid,
                    RegroupParty = not.ForRegroupParty,
                });
                break;
            }

            case EventId.ERM_CHAT_ACK:
                SendPacket(EventId.EGS_CHAT_ACK, ev.GetPayload<KPacketOK>(Region));
                break;

            case EventId.ERM_CHAT_NOT:
                SendPacket(EventId.EGS_CHAT_NOT, ev.GetPayload<KEGS_CHAT_NOT>(Region));
                break;

            case EventId.ERM_CHANGE_MOTION_ACK:
                SendPacket(EventId.EGS_CHANGE_MOTION_ACK, ev.GetPayload<KPacketOK>(Region));
                break;

            case EventId.ERM_CHANGE_MOTION_NOT:
                SendPacket(EventId.EGS_CHANGE_MOTION_NOT, ev.GetPayload<KEGS_CHANGE_MOTION_REQ>(Region));
                break;

            case EventId.ERM_CHANGE_PITIN_ACK:
                SendPacket(EventId.EGS_CHANGE_PITIN_ACK, ev.GetPayload<KPacketOK>(Region));
                break;

            case EventId.ERM_CHANGE_PITIN_NOT:
                SendPacket(EventId.EGS_CHANGE_PITIN_NOT, ev.GetPayload<KEGS_CHANGE_PITIN_REQ>(Region));
                break;

            case EventId.ERM_CHANGE_PLAY_TIME_LIMIT_ACK:
                SendPacket(EventId.EGS_CHANGE_PLAY_TIME_LIMIT_ACK, ev.GetPayload<KPacketOK>(Region));
                break;

            case EventId.ERM_CHANGE_PLAY_TIME_LIMIT_NOT:
                SendPacket(EventId.EGS_CHANGE_PLAY_TIME_LIMIT_NOT, ev.GetPayload<KEGS_CHANGE_PLAY_TIME_LIMIT_REQ>(Region));
                break;

            case EventId.ERM_CHANGE_PVP_ROOM_NAME_ACK:
                SendPacket(EventId.EGS_CHANGE_PVP_ROOM_NAME_ACK, ev.GetPayload<KPacketOK>(Region));
                break;

            case EventId.ERM_CHANGE_PVP_ROOM_NAME_NOT:
                SendPacket(EventId.EGS_CHANGE_PVP_ROOM_NAME_NOT, ev.GetPayload<KEGS_CHANGE_PVP_ROOM_NAME_REQ>(Region));
                break;

            case EventId.ERM_CHANGE_PVP_ROOM_PUBLIC_ACK:
                SendPacket(EventId.EGS_CHANGE_PVP_ROOM_PUBLIC_ACK, ev.GetPayload<KPacketOK>(Region));
                break;

            case EventId.ERM_CHANGE_PVP_ROOM_PUBLIC_NOT:
                SendPacket(EventId.EGS_CHANGE_PVP_ROOM_PUBLIC_NOT, ev.GetPayload<KEGS_CHANGE_PVP_ROOM_PUBLIC_REQ>(Region));
                break;

            case EventId.ERM_DUNGEON_STAGE_LOAD_ACK:
                SendPacket(EventId.EGS_DUNGEON_STAGE_LOAD_ACK, ev.GetPayload<KPacketOK>(Region));
                break;

            case EventId.ERM_DUNGEON_STAGE_LOAD_NOT:
                SendPacket(EventId.EGS_DUNGEON_STAGE_LOAD_NOT, ev.GetPayload<KEGS_DUNGEON_STAGE_LOAD_NOT>(Region));
                break;

            case EventId.ERM_DUNGEON_SUB_STAGE_CLEAR_ACK:
                SendPacket(EventId.EGS_DUNGEON_SUB_STAGE_CLEAR_ACK, ev.GetPayload<KEGS_DUNGEON_SUB_STAGE_CLEAR_ACK>(Region));
                break;

            case EventId.ERM_DUNGEON_SUB_STAGE_OPEN_ACK:
                SendPacket(EventId.EGS_DUNGEON_SUB_STAGE_OPEN_ACK, ev.GetPayload<KPacketOK>(Region));
                break;

            case EventId.ERM_DUNGEON_SUB_STAGE_OPEN_NOT:
                SendPacket(EventId.EGS_DUNGEON_SUB_STAGE_OPEN_NOT, ev.GetPayload<KERM_DUNGEON_SUB_STAGE_OPEN_REQ>(Region));
                break;

            case EventId.ERM_NPC_UNIT_CREATE_ACK:
                SendPacket(EventId.EGS_NPC_UNIT_CREATE_ACK, ev.GetPayload<KPacketOK>(Region));
                break;

            case EventId.ERM_NPC_UNIT_CREATE_NOT:
                SendPacket(EventId.EGS_NPC_UNIT_CREATE_NOT, ev.GetPayload<KEGS_NPC_UNIT_CREATE_NOT>(Region));
                break;

            case EventId.ERM_NPC_UNIT_DIE_ACK:
            {
                var ack = ev.GetPayload<KERM_NPC_UNIT_DIE_ACK>(Region);
                SendPacket(EventId.EGS_NPC_UNIT_DIE_ACK, new KEGS_NPC_UNIT_DIE_ACK { Ok = ack.Ok });
                break;
            }

            case EventId.ERM_NPC_UNIT_DIE_NOT:
                SendPacket(EventId.EGS_NPC_UNIT_DIE_NOT, ev.GetPayload<KEGS_NPC_UNIT_DIE_NOT>(Region));
                break;

            case EventId.ERM_USER_UNIT_DIE_ACK:
                SendPacket(EventId.EGS_USER_UNIT_DIE_ACK, ev.GetPayload<KEGS_USER_UNIT_DIE_ACK>(Region));
                break;

            case EventId.ERM_USER_UNIT_DIE_NOT:
                SendPacket(EventId.EGS_USER_UNIT_DIE_NOT, ev.GetPayload<KEGS_USER_UNIT_DIE_NOT>(Region));
                break;

            case EventId.ERM_FORCED_EXIT_VOTE_ACK:
                SendPacket(EventId.EGS_FORCED_EXIT_VOTE_ACK, ev.GetPayload<KEGS_FORCED_EXIT_VOTE_ACK>(Region));
                break;

            case EventId.ERM_PVP_REMATCH_ACK:
                SendPacket(EventId.EGS_PVP_REMATCH_ACK, ev.GetPayload<KEGS_PVP_REMATCH_ACK>(Region));
                break;

            case EventId.ERM_PVP_REMATCH_NOT:
                SendPacket(EventId.EGS_PVP_REMATCH_NOT, ev.GetPayload<KEGS_PVP_REMATCH_NOT>(Region));
                break;

            case EventId.ERM_RESURRECT_TO_CONTINUE_DUNGEON_ACK:
                SendPacket(EventId.EGS_RESURRECT_TO_CONTINUE_DUNGEON_ACK, ev.GetPayload<KEGS_RESURRECT_TO_CONTINUE_DUNGEON_ACK>(Region));
                break;

            case EventId.ERM_RESURRECT_TO_CONTINUE_DUNGEON_NOT:
                SendPacket(EventId.EGS_RESURRECT_TO_CONTINUE_DUNGEON_NOT, ev.GetPayload<KEGS_RESURRECT_TO_CONTINUE_DUNGEON_NOT>(Region));
                break;

            case EventId.ERM_STOP_DUNGEON_CONTINUE_TIME_ACK:
                SendPacket(EventId.EGS_STOP_DUNGEON_CONTINUE_TIME_ACK, ev.GetPayload<KEGS_STOP_DUNGEON_CONTINUE_TIME_ACK>(Region));
                break;

            case EventId.ERM_STATE_CHANGE_RESULT_ACK:
                SendPacket(EventId.EGS_STATE_CHANGE_RESULT_ACK, ev.GetPayload<KPacketOK>(Region));
                break;

            case EventId.ERM_STATE_CHANGE_RESULT_NOT:
                SendPacket(EventId.EGS_STATE_CHANGE_RESULT_NOT, ev.GetPayload<KPacketOK>(Region));
                break;

            case EventId.ERM_LEAVE_SQUARE_ACK:
                CurrentRoomUid = 0;
                SendPacket(EventId.EGS_LEAVE_SQUARE_ACK, ev.GetPayload<KPacketOK>(Region));
                break;

            case EventId.ERM_LEAVE_WEDDING_HALL_ACK:
                CurrentRoomUid = 0;
                SendPacket(EventId.EGS_LEAVE_WEDDING_HALL_ACK, ev.GetPayload<KPacketOK>(Region));
                break;

            case EventId.ERM_END_TC_GAME_ACK:
                SendPacket(EventId.EGS_END_TC_GAME_ACK, ev.GetPayload<KEGS_END_TC_GAME_ACK>(Region));
                break;

            // ---- Batch 3: Remaining Non-ERM Cross-Server Destination Handlers (ECN / ESG / EGB / EPM / ELG / EBILL / Publisher) ----
            case EventId.ECN_WORLD_TRIGGER_RELOCATION_ACK:
                SendPacket(EventId.EGS_WORLD_TRIGGER_RELOCATION_ACK, ev.GetPayload<KPacketOK>(Region));
                break;

            case EventId.ESG_REWARD_DB_DATA_NOT:
                if (ev.InProcessPayload is IKSerializable sgRew)
                    SendPacket(EventId.ESG_REWARD_DB_DATA_NOT, sgRew);
                break;

            case EventId.EBILL_CHECK_BALANCE_ACK:
            {
                var ok = ev.GetPayload<KPacketOK>(Region);
                SendPacket(EventId.EGS_CHECK_BALANCE_ACK, new KEGS_CHECK_BALANCE_ACK { Ok = ok.Ok });
                break;
            }

            case EventId.EBILL_EXCHANGE_CASH_ACK:
            {
                var ack = ev.GetPayload<KEBILL_EXCHANGE_CASH_ACK>(Region);
                SendPacket(EventId.EGS_EXCHANGE_CASH_ACK, new KEGS_EXCHANGE_CASH_ACK
                {
                    Ok = ack.Ok,
                    Point = ack.Point,
                });
                break;
            }

            case EventId.EBILL_USE_COUPON_RESERVE_ACK:
            {
                var ack = ev.GetPayload<KEBILL_USE_COUPON_RESERVE_ACK>(Region);
                SendPacket(EventId.EGS_APPLY_COUPON_ACK, new KPacketOK { Ok = ack.Ok });
                break;
            }

            case EventId.EBILL_USE_COUPON_RESULT_ACK:
            {
                var ack = ev.GetPayload<KEBILL_USE_COUPON_RESULT_ACK>(Region);
                if (ack.Ok == NetError.NET_OK && ActiveUnit != null && ack.ChargeItem > 0)
                    _server.Db.GrantUnitItem(ActiveUnit.UnitUid, ack.ChargeItem, Math.Max(1, ack.ChargeItemCnt));
                SendPacket(EventId.EGS_APPLY_COUPON_ACK, new KPacketOK { Ok = ack.Ok });
                break;
            }

            case EventId.EBILL_BUY_PRODUCT_ACK:
            case EventId.EBILL_PREPARE_BUY_PRODUCT_ACK:
                SendPacket(EventId.EGS_BUY_CASH_ITEM_ACK, ev.GetPayload<KEGS_BUY_CASH_ITEM_ACK>(Region));
                break;

            case EventId.EBILL_PICK_UP_ACK:
                SendPacket(EventId.EGS_BILL_GET_PURCHASED_CASH_ITEM_ACK, ev.InProcessPayload is IKSerializable pua
                    ? pua
                    : ev.GetPayload<KPacketOK>(Region));
                break;

            case EventId.EBILL_INVENTORY_INQUIRY_ACK:
                SendPacket(EventId.EGS_BILL_INVENTORY_INQUIRY_ACK, ev.GetPayload<KEGS_BILL_INVENTORY_INQUIRY_ACK>(Region));
                break;

            case EventId.EBILL_GIFT_ITEM_ACK:
            case EventId.EBILL_PREPARE_GIFT_ITEM_ACK:
            case EventId.EBILL_GARENA_PREPARE_PRESENT_CHECK_ACK:
                SendPacket(EventId.EGS_PRESENT_CASH_ITEM_ACK, ev.InProcessPayload is IKSerializable gfa
                    ? gfa
                    : ev.GetPayload<KPacketOK>(Region));
                break;

            case EventId.EBILL_CHECK_BUY_FAKE_ITEM_ACK:
            {
                var ack = ev.GetPayload<KPacketOK>(Region);
                if (ack.Ok == NetError.NET_OK && ActiveUnit != null)
                {
                    _server.Db.SendLetter(Region, ActiveUnit.UnitUid, ActiveUnit.NickName, ActiveUnit.UnitUid,
                        "Event Reward", "Event Item", 834, 1, 0, KPostItemInfo.LtEvent);
                }
                SendPacket(EventId.EGS_BUY_CASH_ITEM_ACK, new KEGS_BUY_CASH_ITEM_ACK { Ok = ack.Ok });
                break;
            }

            case EventId.EBILL_REWARD_COUPON_JP_EVENT_ACK:
            {
                var ack = ev.GetPayload<KEBILL_REWARD_COUPON_JP_EVENT_ACK>(Region);
                if (ack.UseCouponPacket.Ok != NetError.NET_OK)
                {
                    KLog.Cerr($"[EBILL_REWARD_COUPON_JP_EVENT_ACK] Failed Ok={ack.UseCouponPacket.Ok}, RId={ack.RewardInfo.RId}");
                    break;
                }
                _server.Router.Send(PerformerTypeId, ActiveUnit?.UnitUid ?? 0, PerformerId.PI_GS_SERVER, _server.Uid,
                    EventId.DBE_UPDATE_REWARD_COMPLETE_JP_EVENT_NOT,
                    new KDBE_UPDATE_REWARD_COMPLETE_JP_EVENT_NOT { RId = ack.RewardInfo.RId });
                break;
            }

            case EventId.EPUBLISHER_BILLING_BALANCE_ACK:
            {
                var pba = ev.GetPayload<KEPUBLISHER_BILLING_BALANCE_ACK>(Region);
                SendPacket(EventId.EGS_CHECK_BALANCE_ACK, new KEGS_CHECK_BALANCE_ACK
                {
                    Ok = pba.Ok,
                    Balance = (uint)Math.Max(0, pba.Balance),
                });
                break;
            }

            case EventId.EPUBLISHER_CHARGE_POINT_ACK:
            {
                var cpa = ev.GetPayload<KEPUBLISHER_CHARGE_POINT_ACK>(Region);
                SendPacket(EventId.EGS_CHARGE_POINT_NOT, new KEGS_CHARGE_POINT_NOT
                {
                    Ok = cpa.Ok,
                    ChargedCash = cpa.ChargedCash,
                });
                break;
            }

            case EventId.EJSON_GN_CHANGE_GAME_CURRENCY_REQ:
                _server.Router.Send(PerformerTypeId, ActiveUnit?.UnitUid ?? AccountUid, PerformerId.PI_LOGIN_SERVER, _server.LoginServerUid,
                    EventId.EBILL_GN_CHANGE_GAME_CURRENCY_REQ, new KPacketOK { Ok = NetError.NET_OK });
                break;

            case EventId.EGIANT_INFO_PUNISH_REQ:
                // Empty in C++ GSUserGiant.cpp
                break;

            case EventId.EGIANT_INFO_REQUESTID_REQ:
                if (ActiveUnit != null)
                {
                    _server.Router.Send(PerformerTypeId, ActiveUnit.UnitUid, PerformerId.PI_LOGIN_SERVER, _server.LoginServerUid,
                        EventId.EGIANT_INFO_REQUESTID_ACK, new KEGIANT_INFO_REQUESTID_ACK
                        {
                            UserName = ActiveUnit.NickName,
                            Account = UserId,
                            GiantUid = (uint)ActiveUnit.UnitUid,
                        });
                }
                break;

            case EventId.EGIANT_ROLEREG_CREATE_UNIT_ACK:
            {
                if (AccountUid <= 0) break;
                var ack = ev.GetPayload<KEGIANT_ROLEREG_CREATE_UNIT_ACK>(Region);
                if (ack.Ok != NetError.NET_OK)
                {
                    SendPacket(EventId.EGS_CREATE_UNIT_ACK, new KEGS_CREATE_UNIT_ACK { Ok = ack.Ok });
                    break;
                }
                var (_, unit, err) = _server.Db.CreateUnit(Region, AccountUid, ack.CreateUnitReqPacket.NickName, ack.CreateUnitReqPacket.UnitClass);
                SendPacket(EventId.EGS_CREATE_UNIT_ACK, new KEGS_CREATE_UNIT_ACK
                {
                    Ok = err,
                    UnitInfo = unit ?? new KUnitInfo(),
                });
                break;
            }

            case EventId.EGIANT_ROLEREG_CREATE_GUILD_ACK:
            {
                if (ActiveUnit == null) break;
                var ack = ev.GetPayload<KEGIANT_ROLEREG_CREATE_GUILD_ACK>(Region);
                if (ack.Ok != NetError.NET_OK)
                {
                    SendPacket(EventId.EGS_CREATE_GUILD_ACK, new KEGS_CREATE_GUILD_ACK { Ok = ack.Ok });
                    break;
                }
                var (ok, guild, err) = _server.Db.CreateGuild(Region, ActiveUnit.UnitUid, ack.CreateGuildReqPacket.GuildName, ack.CreateGuildReqPacket.GuildMessage);
                if (ok && guild != null)
                {
                    ActiveUnit.UserGuildInfo.GuildUid = guild.GuildUid;
                    ActiveUnit.UserGuildInfo.GuildName = guild.GuildName;
                    ActiveUnit.UserGuildInfo.MemberShipGrade = 1;
                }
                SendPacket(EventId.EGS_CREATE_GUILD_ACK, new KEGS_CREATE_GUILD_ACK
                {
                    Ok = err,
                    GuildInfo = guild ?? new KGuildInfo(),
                });
                break;
            }

            case EventId.EGB_ADMIN_HERO_PVP_USER_CONNECTION_NOT:
                SendPacket(EventId.EGS_ADMIN_HERO_PVP_USER_CONNECTION_NOT, ev.GetPayload<KEGS_ADMIN_HERO_PVP_USER_CONNECTION_NOT>(Region));
                break;

            case EventId.EGB_CHECK_INVALID_MATCH_NOT:
                if (PvpManager.MatchUid != 0)
                {
                    PvpManager.MatchUid = 0;
                    SendPacket(EventId.EGS_UNREG_MATCH_WAIT_LIST_NOT, new KEGS_UNREG_MATCH_WAIT_LIST_NOT
                    {
                        Reason = NetError.NET_OK,
                        CancelUnitNickName = ActiveUnit?.NickName ?? string.Empty,
                    });
                }
                break;

            case EventId.EGB_REMATCH_MAKING_USER_NOT:
            {
                if (ActiveUnit == null || PvpManager.MatchUid != 0) break;
                var rematch = ev.GetPayload<KEGB_REMATCH_MAKING_USER_NOT>(Region);
                if (rematch.PvpPlayerCount is < 1 or > 3) break;
                if (rematch.PvpGameType != 0 && rematch.PvpGameType != 1) break;
                if (ActiveUnit.Level < 10) break;

                if (CurrentPartyUid == 0)
                {
                    _server.Router.Send(PerformerTypeId, ActiveUnit.UnitUid, PerformerId.PI_GLOBAL_SERVER, _server.Global.Uid,
                        EventId.EGB_REQUEST_MATCH_MAKING_REQ, new KEGB_REQUEST_MATCH_MAKING_REQ
                        {
                            RegFrontOfList = true,
                            PvpPlayerCount = rematch.PvpPlayerCount,
                            PvpGameType = rematch.PvpGameType,
                            MatchUserInfo = new List<KMatchUserInfo>
                            {
                                new KMatchUserInfo
                                {
                                    GsUid = _server.Uid,
                                    UnitUid = ActiveUnit.UnitUid,
                                    PartyUid = 0,
                                    NickName = ActiveUnit.NickName,
                                    UnitClass = ActiveUnit.UnitClass,
                                    Level = ActiveUnit.Level,
                                    CurrentRank = ActiveUnit.Rank,
                                    OfficialMatchCnt = ActiveUnit.OfficialMatchCnt,
                                    Rating = ActiveUnit.Rating,
                                    RPoint = ActiveUnit.RPoint,
                                    MatchWaitTime = 0,
                                },
                            },
                        });
                }
                else
                {
                    _server.Router.Send(PerformerTypeId, ActiveUnit.UnitUid, PerformerId.PI_CENTER_SERVER, _server.Center.Uid,
                        EventId.EPM_GET_PARTY_INFO_FOR_MATCH_MAKING_REQ, new KEPM_GET_PARTY_INFO_FOR_MATCH_MAKING_REQ
                        {
                            RegFrontOfList = true,
                            PvpPlayerCount = rematch.PvpPlayerCount,
                            PvpGameType = rematch.PvpGameType,
                        });
                }
                break;
            }

            case EventId.EGB_REMAKING_AUTO_PARTY_NOT:
                SendPacket(EventId.EGS_REG_AUTO_PARTY_WAIT_LIST_SUCCESS_NOT, new KEGS_REG_AUTO_PARTY_WAIT_LIST_SUCCESS_NOT
                {
                    RemakingAutoParty = true,
                });
                break;

            case EventId.EGB_UPDATE_BLOCK_RESULT_NOT:
                if (ev.InProcessPayload is KPacketBool tb)
                {
                    AbuserManager.IsTradeBlockUser = tb.Value;
                }
                else
                {
                    var blockRes = ev.GetPayload<KEGB_UPDATE_BLOCK_RESULT_NOT>(Region);
                    if (blockRes.BlockResult.TryGetValue(0, out bool isTradeBlocked) && isTradeBlocked)
                        AbuserManager.IsTradeBlockUser = true;
                    if ((blockRes.BlockResult.TryGetValue(1, out bool isAcctBlocked) && isAcctBlocked) ||
                        (blockRes.BlockResult.TryGetValue(2, out bool isIpBlocked) && isIpBlocked))
                        ReserveDestroy();
                }
                break;

            case EventId.EGB_ADMIN_HERO_PVP_USER_TOURNAMENT_CONNECTION_CHECK_NOT:
            case EventId.EGB_TIME_ENCHANT_EVENT_CUBE_NOT:
            case EventId.EGB_TIME_ENCHANT_EVENT_DROP_NOT:
                if (ev.EventId != EventId.EGB_ADMIN_HERO_PVP_USER_TOURNAMENT_CONNECTION_CHECK_NOT && ActiveUnit != null)
                    _hasDirectTimeEventReward = true;
                break;

            case EventId.EGB_REG_MATCH_WAIT_LIST_SUCCESS_NOT:
                SendPacket(EventId.EGS_REG_MATCH_WAIT_LIST_SUCCESS_NOT, ev.GetPayload<KEGS_REG_MATCH_WAIT_LIST_SUCCESS_NOT>(Region));
                break;

            case EventId.EGB_UNREG_MATCH_WAIT_LIST_NOT:
                SendPacket(EventId.EGS_UNREG_MATCH_WAIT_LIST_NOT, ev.GetPayload<KEGS_UNREG_MATCH_WAIT_LIST_NOT>(Region));
                break;

            case EventId.EGB_REG_AUTO_PARTY_WAIT_LIST_SUCCESS_NOT:
                SendPacket(EventId.EGS_REG_AUTO_PARTY_WAIT_LIST_SUCCESS_NOT, ev.GetPayload<KEGS_REG_AUTO_PARTY_WAIT_LIST_SUCCESS_NOT>(Region));
                break;

            case EventId.EGB_UNREG_AUTO_PARTY_WAIT_LIST_NOT:
                SendPacket(EventId.EGS_UNREG_AUTO_PARTY_WAIT_LIST_NOT, ev.GetPayload<KEGS_UNREG_AUTO_PARTY_WAIT_LIST_NOT>(Region));
                break;

            case EventId.EGB_GET_ROOM_USER_INFO_FOR_MATCH_REQ:
            {
                if (ActiveUnit == null) break;
                _server.Router.Send(PerformerTypeId, ActiveUnit.UnitUid, PerformerId.PI_GLOBAL_SERVER, _server.Global.Uid,
                    EventId.EGB_GET_ROOM_USER_INFO_FOR_MATCH_ACK,
                    new KEGB_GET_ROOM_USER_INFO_FOR_MATCH_ACK
                    {
                        RoomUserInfo = BuildRoomUserInfo(),
                    });
                break;
            }

            case EventId.EGB_OPEN_PVP_ROOM_FOR_MATCH_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGB_OPEN_PVP_ROOM_FOR_MATCH_REQ>(Region);
                var room = _server.Center.RoomManager.CreateRoom(new KRoomInfo
                {
                    RoomType = 2,
                    RoomName = $"Match_{req.MatchUid}",
                    WorldId = (short)Math.Max(1, (int)req.RoomInfo.WorldId),
                    PvpGameType = req.RoomInfo.PvpGameType,
                    MaxSlot = Math.Max((sbyte)2, req.RoomInfo.MaxSlot),
                }, BuildRoomUserInfo(), asObserver: false);
                CurrentRoomUid = room.Info.RoomUid;
                Fsm.TryStateTransition(KGSFSM.I_TO_ROOM, out _);
                _server.Router.Send(PerformerTypeId, ActiveUnit.UnitUid, PerformerId.PI_GLOBAL_SERVER, _server.Global.Uid,
                    EventId.EGB_OPEN_PVP_ROOM_FOR_MATCH_ACK, new KPacketOK { Ok = NetError.NET_OK });
                break;
            }

            case EventId.EGB_GET_REGROUP_PARTY_INFO_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEGB_GET_REGROUP_PARTY_INFO_REQ>(Region);
                _server.Router.Send(PerformerTypeId, ActiveUnit.UnitUid, PerformerId.PI_GLOBAL_SERVER, _server.Global.Uid,
                    EventId.EGB_GET_REGROUP_PARTY_INFO_ACK, new KEGB_GET_REGROUP_PARTY_INFO_ACK
                    {
                        AutoPartyUid = req.AutoPartyUid,
                        PartyUserInfo = BuildPartyUserInfo(),
                        RoomUserInfo = BuildRoomUserInfo(),
                    });
                break;
            }

            case EventId.EGB_REGROUP_PARTY_FOR_AUTO_PARTY_DUNGEON_GAME_REQ:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KEPM_REGROUP_PARTY_FOR_AUTO_PARTY_DUNGEON_GAME_REQ>(Region);
                var createInfo = req.CreatePartyInfo ?? new KCreatePartyInfo
                {
                    DungeonId = 30000,
                    PartyType = 0,
                };
                if (createInfo.DungeonId <= 0)
                    createInfo.DungeonId = 30000;
                var party = _server.Center.PartyManager.CreateParty(createInfo, BuildPartyUserInfo());
                foreach (var m in req.Members)
                    if (m.UnitUid != ActiveUnit.UnitUid)
                        _server.Center.PartyManager.TryJoinParty(party.Info.PartyUid, m, out _);
                CurrentPartyUid = party.Info.PartyUid;
                _server.Router.Send(PerformerTypeId, ActiveUnit.UnitUid, PerformerId.PI_GLOBAL_SERVER, _server.Global.Uid,
                    EventId.EGB_REGROUP_PARTY_FOR_AUTO_PARTY_DUNGEON_GAME_ACK, new KEPM_REGROUP_PARTY_FOR_AUTO_PARTY_DUNGEON_GAME_ACK
                    {
                        Ok = NetError.NET_OK,
                        PartyUid = party.Info.PartyUid,
                        AutoPartyUid = req.AutoPartyUid,
                    });
                break;
            }

            case EventId.EGB_INVITE_PARTY_GET_RECEIVER_USER_INFO_REQ:
            case EventId.ELG_INVITE_PARTY_GET_RECEIVER_USER_INFO_REQ:
            {
                if (ActiveUnit == null) break;
                _server.Router.Send(PerformerTypeId, ActiveUnit.UnitUid, PerformerId.PI_CENTER_SERVER, _server.Center.Uid,
                    EventId.ELG_INVITE_PARTY_GET_RECEIVER_USER_INFO_ACK, BuildPartyUserInfo());
                break;
            }

            // ---- EPM_* Party Destination Handlers ----
            case EventId.EPM_CREATE_PARTY_BY_INVITE_ACK:
            {
                var ok = ev.GetPayload<KPacketOK>(Region);
                SendPacket(EventId.EGS_CREATE_PARTY_ACK, ok);
                break;
            }

            case EventId.EPM_INVITE_PARTY_ACK:
            {
                var ack = ev.GetPayload<KEGS_INVITE_PARTY_ACK>(Region);
                if (string.IsNullOrEmpty(ack.UdpRelayIp))
                    ack.UdpRelayIp = _server.PublicIp;
                if (ack.UdpRelayPort == 0)
                    ack.UdpRelayPort = _server.UdpRelayPort;
                SendPacket(EventId.EGS_INVITE_PARTY_ACK, ack);
                break;
            }

            case EventId.EPM_INVITE_PARTY_NOT:
            {
                var not = ev.GetPayload<KEPM_INVITE_PARTY_REQ>(Region);
                SendPacket(EventId.EGS_INVITE_PARTY_NOT, new KEGS_INVITE_PARTY_NOT
                {
                    PartyUid = not.PartyUid,
                    HostUnitUid = not.PartyUserInfo.UnitUid,
                    HostNickName = not.PartyUserInfo.NickName,
                    UdpRelayIp = _server.PublicIp,
                    UdpRelayPort = _server.UdpRelayPort,
                });
                break;
            }

            case EventId.EPM_INVITE_PARTY_DENY_NOT:
                SendPacket(EventId.EGS_INVITE_PARTY_DENY_NOT, ev.GetPayload<KEGS_INVITE_PARTY_DENY_NOT>(Region));
                break;

            case EventId.EPM_JOIN_PARTY_NOT:
            {
                var party = ActiveUnit != null ? _server.Center.PartyManager.GetPartyByUnitUid(ActiveUnit.UnitUid) : null;
                if (party != null) CurrentPartyUid = party.Info.PartyUid;
                SendPacket(EventId.EGS_JOIN_PARTY_NOT, new KEGS_JOIN_PARTY_NOT
                {
                    Ok = NetError.NET_OK,
                    PartyInfos = party != null ? [party.Info] : [],
                    PartyUsers = party != null ? party.SnapshotMembers() : [],
                    CurPartyUnitUidList = party != null ? party.GetUnitUids() : [],
                    UdpRelayIp = _server.PublicIp,
                    UdpRelayPort = _server.UdpRelayPort,
                });
                break;
            }

            case EventId.EPM_LEAVE_PARTY_ACK:
                CurrentPartyUid = 0;
                SendPacket(EventId.EGS_LEAVE_PARTY_ACK,
                    ev.InProcessPayload is IKSerializable lpa ? lpa : new KPacketOK { Ok = NetError.NET_OK });
                break;

            case EventId.EPM_LEAVE_PARTY_NOT:
            {
                var ok = ev.GetPayload<KPacketOK>(Region);
                SendPacket(EventId.EGS_LEAVE_PARTY_NOT, new KEGS_LEAVE_PARTY_NOT
                {
                    PartyUid = CurrentPartyUid,
                    Reason = ok.Ok,
                });
                break;
            }

            case EventId.EPM_LEAVE_PARTY_BY_AUTO_PARTY_NOT:
                CurrentPartyUid = 0;
                SendPacket(EventId.EGS_LEAVE_PARTY_ACK, new KPacketOK { Ok = NetError.NET_OK });
                break;

            case EventId.EPM_CLOSE_PARTY_NOT:
                CurrentPartyUid = 0;
                SendPacket(EventId.EGS_CLOSE_PARTY_NOT,
                    ev.InProcessPayload is IKSerializable cpn ? cpn : new KPacketOK { Ok = NetError.NET_OK });
                break;

            case EventId.EPM_CHANGE_PARTY_TYPE_ACK:
                SendPacket(EventId.EGS_CHANGE_PARTY_TYPE_ACK, ev.GetPayload<KPacketOK>(Region));
                break;

            case EventId.EPM_CHANGE_PARTY_TYPE_NOT:
                SendPacket(EventId.EGS_CHANGE_PARTY_TYPE_NOT, new KEGS_CHANGE_PARTY_TYPE_NOT
                {
                    PartyInfo = ev.GetPayload<KPartyInfo>(Region),
                });
                break;

            case EventId.EPM_PARTY_CHANGE_HOST_ACK:
                SendPacket(EventId.EGS_PARTY_CHANGE_HOST_ACK, ev.GetPayload<KPacketOK>(Region));
                break;

            case EventId.EPM_PARTY_CHANGE_NUM_OF_PER_ACK:
                SendPacket(EventId.EGS_PARTY_CHANGE_NUM_OF_PER_ACK, ev.GetPayload<KPacketOK>(Region));
                break;

            case EventId.EPM_PARTY_CHANGE_NUM_OF_PER_NOT:
            {
                var not = ev.GetPayload<KEPM_PARTY_CHANGE_NUM_OF_PER_REQ>(Region);
                SendPacket(EventId.EGS_PARTY_CHANGE_NUM_OF_PER_NOT, new KPacketSByte { Value = not.NumOfPer });
                break;
            }

            case EventId.EPM_PARTY_CHANGE_GET_ITEM_TYPE_ACK:
                SendPacket(EventId.EGS_PARTY_CHANGE_GET_ITEM_TYPE_ACK, ev.GetPayload<KPacketOK>(Region));
                break;

            case EventId.EPM_PARTY_CHANGE_GET_ITEM_TYPE_NOT:
            {
                var not = ev.GetPayload<KEPM_PARTY_CHANGE_GET_ITEM_TYPE_REQ>(Region);
                SendPacket(EventId.EGS_PARTY_CHANGE_GET_ITEM_TYPE_NOT, new KPacketSByte { Value = not.GetItemType });
                break;
            }

            case EventId.EPM_PARTY_CHANGE_CHECK_MIN_LEVEL_ACK:
                SendPacket(EventId.EGS_PARTY_CHANGE_CHECK_MIN_LEVEL_ACK, new KEGS_PARTY_CHANGE_CHECK_MIN_LEVEL_ACK
                {
                    Ok = ev.GetPayload<KPacketOK>(Region).Ok,
                });
                break;

            case EventId.EPM_PARTY_CHANGE_CHECK_MIN_LEVEL_NOT:
                SendPacket(EventId.EGS_PARTY_CHANGE_CHECK_MIN_LEVEL_NOT, new KEGS_PARTY_CHANGE_CHECK_MIN_LEVEL_NOT
                {
                    CheckMinLevel = ev.GetPayload<KEPM_PARTY_CHANGE_CHECK_MIN_LEVEL_REQ>(Region).Check,
                });
                break;

            case EventId.EPM_PVP_PARTY_CHANGE_MATCH_INFO_ACK:
                SendPacket(EventId.EGS_PVP_PARTY_CHANGE_MATCH_INFO_ACK, ev.GetPayload<KPacketOK>(Region));
                break;

            case EventId.EPM_PVP_PARTY_CHANGE_MATCH_INFO_NOT:
            {
                var not = ev.GetPayload<KEGS_PVP_PARTY_CHANGE_MATCH_INFO_REQ>(Region);
                SendPacket(EventId.EGS_PVP_PARTY_CHANGE_MATCH_INFO_NOT, new KPacketSByte { Value = not.PvpGameType });
                break;
            }

            case EventId.EPM_PARTY_FEVER_POINT_NOT:
                SendPacket(EventId.EGS_PARTY_FEVER_POINT_NOT, ev.GetPayload<KEGS_PARTY_FEVER_POINT_NOT>(Region));
                break;

            case EventId.EPM_DUNGEON_SECRET_STAGE_ENTER_CHECK_ACK:
                SendPacket(EventId.EGS_DUNGEON_SECRET_STAGE_ENTER_CHECK_ACK,
                    ev.GetPayload<KEGS_DUNGEON_SECRET_STAGE_ENTER_CHECK_ACK>(Region));
                break;

            case EventId.EPM_PARTY_GAME_START_FAILED_NOT:
                SendPacket(EventId.EGS_PARTY_GAME_START_FAILED_NOT, new KEGS_PARTY_GAME_START_FAILED_NOT
                {
                    Reason = NetError.ERR_PARTY_17,
                    FailUnitUid = ActiveUnit?.UnitUid ?? 0,
                });
                break;

            case EventId.EPM_PARTY_GAME_START_OPINION_CHECK_NOT:
                SendPacket(EventId.EGS_PARTY_GAME_START_OPINION_CHECK_NOT, ev.GetPayload<KEGS_PARTY_GAME_START_OPINION_CHECK_NOT>(Region));
                break;

            case EventId.EPM_REGROUP_PARTY_NOT:
            {
                var party = ActiveUnit != null ? _server.Center.PartyManager.GetPartyByUnitUid(ActiveUnit.UnitUid) : null;
                if (party != null) CurrentPartyUid = party.Info.PartyUid;
                SendPacket(EventId.EGS_REGROUP_PARTY_NOT, new KEGS_REGROUP_PARTY_NOT
                {
                    PartyInfo = party?.Info ?? new KPartyInfo(),
                    PartyUserInfo = party?.SnapshotMembers() ?? [],
                });
                break;
            }

            case EventId.EPM_UPDATE_MOVE_TO_PARTY_MEMBER_INFO_NOT:
                SendPacket(EventId.EGS_UPDATE_MOVE_TO_PARTY_MEMBER_INFO_NOT, ev.GetPayload<KEGS_UPDATE_MOVE_TO_PARTY_MEMBER_INFO_NOT>(Region));
                break;

            case EventId.EPM_CHECK_FOR_PARTY_GAME_START_REQ:
            {
                if (ActiveUnit == null) break;
                _server.Router.Send(PerformerTypeId, ActiveUnit.UnitUid, PerformerId.PI_CENTER_SERVER, _server.Center.Uid,
                    EventId.EPM_CHECK_FOR_PARTY_GAME_START_ACK, new KEPM_CHECK_FOR_PARTY_GAME_START_ACK
                    {
                        Ok = NetError.NET_OK,
                        PartyUid = CurrentPartyUid,
                    });
                break;
            }

            case EventId.EPM_CHECK_DUNGEON_GAME_PLAY_CONDITION_REQ:
            {
                if (ActiveUnit == null) break;
                _server.Router.Send(PerformerTypeId, ActiveUnit.UnitUid, PerformerId.PI_CENTER_SERVER, _server.Center.Uid,
                    EventId.EPM_CHECK_DUNGEON_GAME_PLAY_CONDITION_ACK, new KEPM_CHECK_DUNGEON_GAME_PLAY_CONDITION_ACK
                    {
                        Ok = NetError.NET_OK,
                    });
                break;
            }

            case EventId.EPM_CHECK_FOR_AUTO_PARTY_MAKING_REQ:
            {
                if (ActiveUnit == null) break;
                _server.Router.Send(PerformerTypeId, ActiveUnit.UnitUid, PerformerId.PI_CENTER_SERVER, _server.Center.Uid,
                    EventId.EPM_CHECK_FOR_AUTO_PARTY_MAKING_ACK, new KEPM_CHECK_FOR_AUTO_PARTY_MAKING_ACK
                    {
                        Ok = NetError.NET_OK,
                    });
                break;
            }

            case EventId.EPM_CHECK_PARTY_CHANGE_HOST_REQ:
            {
                if (ActiveUnit == null) break;
                _server.Router.Send(PerformerTypeId, ActiveUnit.UnitUid, PerformerId.PI_CENTER_SERVER, _server.Center.Uid,
                    EventId.EPM_CHECK_PARTY_CHANGE_HOST_ACK, new KEPM_CHECK_PARTY_CHANGE_HOST_ACK
                    {
                        Ok = NetError.NET_OK,
                        PartyUid = CurrentPartyUid,
                        NewHostUnitUid = ActiveUnit.UnitUid,
                    });
                break;
            }

            case EventId.EPM_GET_PARTY_INFO_FOR_AUTO_PARTY_MAKING_ACK:
                if (ev.TryReadPayload<KPacketOK>(Region, out var apOk) && apOk.Ok != NetError.NET_OK)
                    SendPacket(EventId.EGS_AUTO_PARTY_DUNGEON_GAME_ACK, new KEGS_AUTO_PARTY_DUNGEON_GAME_ACK { Ok = apOk.Ok });
                break;

            case EventId.EPM_GET_PARTY_INFO_FOR_JOIN_BATTLE_FIELD_ACK:
                if (ev.TryReadPayload<KPacketOK>(Region, out var bfOk) && bfOk.Ok != NetError.NET_OK)
                    SendPacket(EventId.EGS_JOIN_BATTLE_FIELD_ACK, new KEGS_JOIN_BATTLE_FIELD_ACK { Ok = bfOk.Ok });
                break;

            case EventId.EPM_GET_PARTY_INFO_FOR_MATCH_MAKING_ACK:
                if (ev.TryReadPayload<KPacketOK>(Region, out var mmOk) && mmOk.Ok != NetError.NET_OK)
                    SendPacket(EventId.EGS_REQUEST_MATCH_MAKING_ACK, mmOk);
                break;

            case EventId.EPM_BROADCAST_PARTY_MEMBER_STATUS_NOT:
                if (ev.InProcessPayload is IKSerializable pms)
                    SendPacket(EventId.EGS_UPDATE_PARTY_MEMBER_STATUS_NOT, pms);
                break;

            case EventId.EPM_CHANGE_COME_BACK_BUFF_NOT:
                if (ActiveUnit != null)
                {
                    if (ev.InProcessPayload is KPacketBool cb)
                    {
                        if (cb.Value)
                            ServerBuffManager.ActivateBuff(new KUserServerBuffManager.ServerBuffInfo
                            {
                                BuffId = 30001,
                                ExpRate = 0.1f,
                                ExpireUtc = DateTime.UtcNow.AddHours(1),
                            });
                        else
                            ServerBuffManager.DeactivateBuff(30001);
                    }
                    else if (ev.TryReadPayload<KEPM_CHANGE_COME_BACK_BUFF_NOT>(Region, out var cbPkt))
                    {
                        if (!cbPkt.IsBuffFinish)
                        {
                            DateTime.TryParse(cbPkt.ComeBackBuffEnd, out var endUtc);
                            ServerBuffManager.ActivateBuff(new KUserServerBuffManager.ServerBuffInfo
                            {
                                BuffId = 30001,
                                ExpRate = 0.1f,
                                ExpireUtc = endUtc > DateTime.UtcNow ? endUtc : DateTime.UtcNow.AddHours(1),
                            });
                        }
                        else
                        {
                            ServerBuffManager.DeactivateBuff(30001);
                        }
                    }
                }
                break;

            case EventId.EPM_LEAVE_CANDIDATE_PARTY_NOT:
                PartyManager.CandidatePartyUid = 0;
                break;

            case EventId.EPM_PARTY_OPEN_DUNGEON_ROOM_NOT:
                if (ActiveUnit != null)
                {
                    var req = ev.GetPayload<KERM_PARTY_OPEN_DUNGEON_ROOM_REQ>(Region);
                    req.RoomInfo.DefenceDungeonOpen = _server.WorldMissionManager.IsActive;
                    _server.Router.Send(PerformerTypeId, ActiveUnit.UnitUid, PerformerId.PI_CENTER_SERVER, _server.Center.Uid,
                        EventId.ERM_PARTY_OPEN_DUNGEON_ROOM_REQ, req);
                }
                break;

            case EventId.EPM_PREPARE_FOR_REGROUP_PARTY_AND_CHECK_NEW_HOST_NOT:
                if (ActiveUnit != null && CurrentPartyUid > 0)
                    _server.Router.Send(PerformerTypeId, ActiveUnit.UnitUid, PerformerId.PI_CENTER_SERVER, _server.Center.Uid,
                        EventId.EPM_CHECK_PARTY_CHANGE_HOST_REQ, new KEPM_PARTY_CHANGE_HOST_REQ
                        {
                            PartyUid = CurrentPartyUid,
                            NewHostUnitUid = ActiveUnit.UnitUid,
                        });
                break;

            case EventId.EPM_QUICK_SLOT_COOL_TIME_RESET_NOT:
                _quickSlotCooldowns.Clear();
                break;

            case EventId.EPM_REGROUP_PARTY_FOR_AUTO_PARTY_DUNGEON_GAME_ACK:
            {
                var ack = ev.GetPayload<KEPM_REGROUP_PARTY_FOR_AUTO_PARTY_DUNGEON_GAME_ACK>(Region);
                if (ack.Ok == NetError.NET_OK && ack.PartyUid > 0)
                    CurrentPartyUid = ack.PartyUid;
                break;
            }

            case EventId.EPM_SEND_UPDATE_PARTY_MEMBER_LIST_NOT:
                if (ev.InProcessPayload is IKSerializable pml)
                    SendPacket(EventId.EGS_UPDATE_PARTY_USER_INFO_NOT, pml);
                break;

            case EventId.EPM_UPDATE_PARTY_USER_INFO_BROAD_NOT:
                if (ev.InProcessPayload is IKSerializable pui)
                    SendPacket(EventId.EGS_UPDATE_PARTY_USER_INFO_NOT, pui);
                break;

            // ---- ELG_* LoginServer Destination Handlers ----
            case EventId.ELG_ADMIN_SHOW_ED_CHEAT_REQ:
                SendPacket(EventId.EGS_ADMIN_SHOW_ED_CHEAT_REQ, new KEGS_ADMIN_SHOW_ED_CHEAT_REQ { NickName = ActiveUnit?.NickName ?? string.Empty });
                break;

            case EventId.ELG_CASH_DIRECT_CHARGE_CN_ACK:
                SendPacket(EventId.EGS_CASH_DIRECT_CHARGE_CN_ACK, ev.GetPayload<KEGS_CASH_DIRECT_CHARGE_CN_ACK>(Region));
                break;

            case EventId.ELG_CHECK_EXPAND_GUILD_MAX_MEMBER_ACK:
            {
                var ack = ev.GetPayload<KELG_CHECK_EXPAND_GUILD_MAX_MEMBER_ACK>(Region);
                if (ack.Ok != NetError.NET_OK || ActiveUnit == null)
                {
                    SendPacket(EventId.EGS_USE_ITEM_IN_INVENTORY_ACK, new KEGS_USE_ITEM_IN_INVENTORY_ACK { Ok = ack.Ok });
                    break;
                }
                var updatedSlots = new List<KInventoryItemInfo>();
                int usedItemId = 0;
                if (ack.ItemUid > 0)
                {
                    var (consumed, updatedSlot) = _server.Db.DeleteInventoryItem(ActiveUnit.UnitUid, ack.ItemUid, 1);
                    if (!consumed)
                    {
                        SendPacket(EventId.EGS_USE_ITEM_IN_INVENTORY_ACK, new KEGS_USE_ITEM_IN_INVENTORY_ACK { Ok = NetError.ERR_USE_ITEM_IN_INVENTORY_00 });
                        break;
                    }
                    updatedSlots.Add(updatedSlot);
                    usedItemId = updatedSlot.ItemInfo.ItemId;
                }
                if (ActiveUnit.UserGuildInfo.GuildUid > 0)
                {
                    var (expanded, newMax) = _server.Db.ExpandGuildMaxMembers(Region, ActiveUnit.UserGuildInfo.GuildUid, 10, 200);
                    if (expanded)
                    {
                        _server.Router.Send(PerformerTypeId, ActiveUnit.UnitUid, PerformerId.PI_LOGIN_SERVER, _server.LoginServerUid,
                            EventId.DLG_UPDATE_EXPAND_GUILD_MAX_MEMBER_NOT, new KDBE_EXPAND_GUILD_MAX_MEMBER_ACK
                            {
                                Ok = NetError.NET_OK,
                                GuildUid = ActiveUnit.UserGuildInfo.GuildUid,
                                MaxNumMember = newMax,
                            });
                    }
                }
                SendPacket(EventId.EGS_USE_ITEM_IN_INVENTORY_ACK, new KEGS_USE_ITEM_IN_INVENTORY_ACK
                {
                    Ok = NetError.NET_OK,
                    InventorySlotInfo = updatedSlots,
                    UsedItemId = usedItemId,
                    Ed = ActiveUnit.Ed,
                });
                break;
            }

            case EventId.ELG_CHECK_TIME_RANDOM_ITEM_EVENT_ACK:
                if (ev.InProcessPayload is KPacketOK randOk)
                    _hasDirectTimeEventReward = randOk.Ok == NetError.NET_OK;
                else
                    _hasDirectTimeEventReward = ev.GetPayload<KELG_CHECK_TIME_RANDOM_ITEM_EVENT_ACK>(Region).EventItemId > 0;
                break;

            case EventId.ELG_INVITE_PARTY_FIND_RECEIVER_ACK:
                SendPacket(EventId.EGS_INVITE_PARTY_ACK, new KEGS_INVITE_PARTY_ACK
                {
                    Ok = ev.GetPayload<KPacketOK>(Region).Ok,
                    UdpRelayIp = _server.PublicIp,
                    UdpRelayPort = _server.UdpRelayPort,
                });
                break;

            case EventId.ELG_COUPLE_PROPOSE_AGREE_CHECK_NOT:
            {
                if (ev.InProcessPayload is KEGS_COUPLE_PROPOSE_RESULT_NOT cpr)
                {
                    SendPacket(EventId.EGS_COUPLE_PROPOSE_RESULT_NOT, cpr);
                }
                else if (ev.TryReadPayload<KELG_COUPLE_PROPOSE_RESULT_NOT>(out var lgRes, Region))
                {
                    SendPacket(EventId.EGS_COUPLE_PROPOSE_RESULT_NOT, new KEGS_COUPLE_PROPOSE_RESULT_NOT
                    {
                        Ok = lgRes.Ok,
                        UnitUid = lgRes.AcceptUnitUid,
                        UnitName = lgRes.AcceptNickName,
                    });
                }
                else
                {
                    SendPacket(EventId.EGS_COUPLE_PROPOSE_RESULT_NOT, new KEGS_COUPLE_PROPOSE_RESULT_NOT { Ok = NetError.NET_OK });
                }
                break;
            }

            case EventId.ELG_COUPLE_PROPOSE_RESULT_SUCCESS_NOT:
            {
                RelationshipManager.SetRelationshipType(KUserRelationshipManager.RelationshipType.RtCouple);
                if (ev.InProcessPayload is KEGS_COUPLE_PROPOSE_RESULT_NOT csucc)
                {
                    RelationshipManager.LoverUnitUid = csucc.UnitUid;
                    RelationshipManager.LoverNickName = csucc.UnitName;
                    RelationshipManager.AnniversaryDayUtc = DateTime.UtcNow;
                    SendPacket(EventId.EGS_COUPLE_PROPOSE_RESULT_NOT, csucc);
                }
                else if (ev.TryReadPayload<KELG_COUPLE_PROPOSE_RESULT_NOT>(out var lgSucc, Region))
                {
                    RelationshipManager.LoverUnitUid = lgSucc.AcceptUnitUid;
                    RelationshipManager.LoverNickName = lgSucc.AcceptNickName;
                    RelationshipManager.AnniversaryDayUtc = DateTime.UtcNow;
                    SendPacket(EventId.EGS_COUPLE_PROPOSE_RESULT_NOT, new KEGS_COUPLE_PROPOSE_RESULT_NOT
                    {
                        Ok = lgSucc.Ok,
                        Result = 1,
                        UnitUid = lgSucc.AcceptUnitUid,
                        UnitName = lgSucc.AcceptNickName,
                        DateUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    });
                }
                else
                {
                    SendPacket(EventId.EGS_COUPLE_PROPOSE_RESULT_NOT, new KEGS_COUPLE_PROPOSE_RESULT_NOT { Ok = NetError.NET_OK, Result = 1 });
                }
                break;
            }

            case EventId.ELG_EVENT_PROPOSE_AGREE_CHECK_NOT:
            case EventId.ELG_EVENT_PROPOSE_RESULT_SUCCESS_NOT:
            {
                if (ev.InProcessPayload is KEGS_COUPLE_PROPOSE_RESULT_NOT evpr)
                {
                    SendPacket(EventId.EGS_COUPLE_PROPOSE_RESULT_NOT, evpr);
                }
                else if (ev.TryReadPayload<KELG_COUPLE_PROPOSE_RESULT_NOT>(out var lgEv, Region))
                {
                    SendPacket(EventId.EGS_COUPLE_PROPOSE_RESULT_NOT, new KEGS_COUPLE_PROPOSE_RESULT_NOT
                    {
                        Ok = lgEv.Ok,
                        Result = ev.EventId == EventId.ELG_EVENT_PROPOSE_RESULT_SUCCESS_NOT ? (sbyte)1 : (sbyte)0,
                        UnitUid = lgEv.AcceptUnitUid,
                        UnitName = lgEv.AcceptNickName,
                    });
                }
                else
                {
                    SendPacket(EventId.EGS_COUPLE_PROPOSE_RESULT_NOT, new KEGS_COUPLE_PROPOSE_RESULT_NOT { Ok = NetError.NET_OK, Result = 1 });
                }
                break;
            }

            case EventId.ELG_INIT_GUILD_SKILL_DEL_ITEM_REQ:
                if (ActiveUnit != null && GuildManager.GuildUid > 0)
                {
                    var updated = new List<KInventoryItemInfo>();
                    if (ev.TryReadPayload<KDBE_INIT_GUILD_SKILL_REQ>(Region, out var initReq))
                    {
                        if (initReq.ItemUid > 0)
                        {
                            var (delOk, delSlot) = _server.Db.DeleteInventoryItem(ActiveUnit.UnitUid, initReq.ItemUid, 1);
                            if (delOk) updated.Add(delSlot);
                        }
                        else if (initReq.UpdatedInventorySlot.Count > 0)
                        {
                            updated.AddRange(initReq.UpdatedInventorySlot);
                        }
                    }
                    _server.Db.InitGuildSkills(GuildManager.GuildUid);
                    SendPacket(EventId.EGS_INIT_GUILD_SKILL_ACK, new KEGS_INIT_GUILD_SKILL_ACK
                    {
                        Ok = NetError.NET_OK,
                        InventorySlotInfo = updated,
                    });
                }
                break;

            case EventId.ELG_RESET_GUILD_SKILL_DEL_ITEM_REQ:
                if (ActiveUnit != null && GuildManager.GuildUid > 0)
                {
                    var updated = new List<KInventoryItemInfo>();
                    int skillId = 0;
                    if (ev.TryReadPayload<KDBE_RESET_GUILD_SKILL_REQ>(Region, out var resetReq))
                    {
                        skillId = resetReq.GuildSkillId;
                        foreach (var delItem in resetReq.ItemQuantityUpdate.Deleted)
                        {
                            var (delOk, delSlot) = _server.Db.DeleteInventoryItem(ActiveUnit.UnitUid, delItem.ItemUid, 1);
                            if (delOk) updated.Add(delSlot);
                        }
                        if (updated.Count == 0 && resetReq.UpdatedInventorySlot.Count > 0)
                            updated.AddRange(resetReq.UpdatedInventorySlot);
                    }
                    if (skillId > 0)
                        _server.Db.ResetGuildSkill(GuildManager.GuildUid, skillId);
                    SendPacket(EventId.EGS_RESET_GUILD_SKILL_ACK, new KEGS_RESET_GUILD_SKILL_ACK
                    {
                        Ok = NetError.NET_OK,
                        InventorySlotInfo = updated,
                    });
                }
                break;

            case EventId.ELG_KNM_TOGETHER_BY_UNIT_INFO_REQ:
                if (ActiveUnit != null)
                    _server.Router.Send(PerformerTypeId, ActiveUnit.UnitUid, PerformerId.PI_LOGIN_SERVER, _server.LoginServerUid,
                        EventId.ELG_KNM_TOGETHER_BY_UNIT_INFO_ACK, new KEGS_KNM_TOGETHER_ACK
                        {
                            Ok = AuthLevel >= 5 ? NetError.ERR_GM_CHAR_DENY_00 : NetError.NET_OK,
                        });
                break;

            case EventId.ELG_RELATIONSHIP_INFO_GAME_SERVER_REQ:
                if (ActiveUnit != null)
                {
                    var req = ev.GetPayload<KEGS_RELATIONSHIP_INFO_REQ>(Region);
                    _server.Router.Send(PerformerTypeId, ActiveUnit.UnitUid, PerformerId.PI_LOGIN_SERVER, _server.LoginServerUid,
                        EventId.ELG_RELATIONSHIP_INFO_GAME_SERVER_ACK, new KEGS_RELATIONSHIP_INFO_ACK
                        {
                            Ok = NetError.NET_OK,
                            RequestUnitUid = req.AcceptUnitUid,
                            AcceptUnitUid = ActiveUnit.UnitUid,
                            Level = ActiveUnit.Level,
                            UnitClass = ActiveUnit.UnitClass,
                            MapId = CurrentFieldMapId,
                            IsConnect = true,
                        });
                }
                break;

            case EventId.ELG_CHANNEL_CHANGE_NEXON_USER_AUTHENTICATE_ACK:
            case EventId.ELG_NEXON_USER_AUTHENTICATE_ACK:
            case EventId.ELG_REGISTER_NEXON_CHANNEL_CHANGE_USER_ACK:
            case EventId.ELG_REGISTER_NEXON_USER_ACK:
                if (ev.GetPayload<KPacketOK>(Region).Ok != NetError.NET_OK)
                    ReserveDestroy();
                break;

            case EventId.ELG_REGISTER_USER_GLOBAL_PUBLISHER_ACK:
            {
                var pubAck = ev.GetPayload<KELG_REGISTER_USER_GLOBAL_PUBLISHER_ACK>(Region);
                if (pubAck.Ok != NetError.NET_OK)
                {
                    ReserveDestroy();
                    break;
                }
                if (pubAck.IsPcBang)
                {
                    PcBangManager.IsPcBang = true;
                    PcBangManager.PcBangType = pubAck.PCBangType;
                }
                break;
            }

            case EventId.ELG_TRADE_BLOCK_NOT:
                ReserveDestroy();
                break;

            case EventId.ELG_INSERT_TITLE_NOT:
            {
                if (ActiveUnit != null)
                {
                    int tid = ev.TryReadPayload<KELG_INSERT_TITLE_NOT>(Region, out var titNot)
                        ? titNot.TitleId
                        : (ev.TryReadPayload<KPacketOK>(Region, out var titOk) ? titOk.Ok : 0);
                    if (tid > 0) _server.Db.GrantUnitTitle(ActiveUnit.UnitUid, tid);
                }
                break;
            }

            case EventId.ELG_FIND_USER_SEND_LETTER_NOT:
            {
                var not = ev.GetPayload<KELG_FIND_USER_SEND_LETTER_NOT>(Region);
                if (not.TargetUnitUid > 0)
                {
                    _server.Db.SendLetter(
                        Region,
                        not.Letter.FromUnitUid,
                        not.Letter.FromNickName,
                        not.TargetUnitUid,
                        not.Letter.Title,
                        not.Letter.Message,
                        not.Letter.ScriptIndex,
                        not.Letter.Quantity,
                        0,
                        not.Letter.ScriptType);
                }
                break;
            }

            case EventId.ELG_GET_GUILD_AD_LIST_ACK:
            {
                var ack = ev.GetPayload<KELG_GET_GUILD_AD_LIST_ACK>(Region);
                SendPacket(EventId.EGS_GET_GUILD_AD_LIST_ACK, new KEGS_GET_GUILD_AD_LIST_ACK
                {
                    TotalPage = (uint)Math.Max(1, ack.TotalPage),
                    ViewPage = (uint)Math.Max(1, ack.CurrentPage),
                    GuildAdList = ack.AdList,
                });
                break;
            }

            case EventId.ELG_REGISTRATION_GUILD_AD_ACK:
            {
                var ack = ev.GetPayload<KELG_REGISTRATION_GUILD_AD_ACK>(Region);
                SendPacket(EventId.EGS_REGISTRATION_GUILD_AD_ACK, new KEGS_REGISTRATION_GUILD_AD_ACK { Ok = ack.Ok });
                break;
            }

            case EventId.ELG_GUILD_INFO_NOT:
                SendPacket(EventId.EGS_GUILD_INFO_NOT, ev.GetPayload<KEGS_GUILD_INFO_NOT>(Region));
                break;

            case EventId.ELG_GUILD_MESSAGE_NOT:
                SendPacket(EventId.EGS_GUILD_MESSAGE_NOT, ev.GetPayload<KEGS_GUILD_MESSAGE_NOT>(Region));
                break;

            case EventId.ELG_GET_GUILD_SKILL_NOT:
                SendPacket(EventId.EGS_GET_GUILD_SKILL_NOT, ev.GetPayload<KEGS_GET_GUILD_SKILL_NOT>(Region));
                break;

            case EventId.ELG_INIT_GUILD_SKILL_NOT:
                SendPacket(EventId.EGS_INIT_GUILD_SKILL_NOT, ev.GetPayload<KEGS_INIT_GUILD_SKILL_NOT>(Region));
                break;

            case EventId.ELG_RESET_GUILD_SKILL_NOT:
                SendPacket(EventId.EGS_RESET_GUILD_SKILL_NOT, ev.GetPayload<KEGS_RESET_GUILD_SKILL_NOT>(Region));
                break;

            case EventId.ELG_EXPIRE_GUILD_CASH_SKILL_POINT_NOT:
                SendPacket(EventId.EGS_EXPIRE_GUILD_CASH_SKILL_POINT_NOT, ev.GetPayload<KEGS_EXPIRE_GUILD_CASH_SKILL_POINT_NOT>(Region));
                break;

            case EventId.ELG_DEL_TUTORIAL_NOT:
            {
                var delNot = ev.GetPayload<KEGS_DEL_TUTORIAL_NOT>(Region);
                if (delNot.UnitUid <= 0 && ActiveUnit != null)
                    delNot.UnitUid = ActiveUnit.UnitUid;
                if (delNot.UnitUid > 0)
                    _server.TutorialManager.DeleteUnit(delNot.UnitUid);
                SendPacket(EventId.EGS_DEL_TUTORIAL_NOT, delNot);
                break;
            }

            case EventId.ELG_REQUEST_TUTORIAL_NOT:
                SendPacket(EventId.EGS_REQUEST_TUTORIAL_NOT, ev.GetPayload<KEGS_REQUEST_TUTORIAL_NOT>(Region));
                break;

            case EventId.ELG_REQUEST_TUTORIAL_REPLY_NOT:
                SendPacket(EventId.EGS_REQUEST_TUTORIAL_REPLY_NOT, ev.GetPayload<KEGS_REQUEST_TUTORIAL_REPLY_NOT>(Region));
                break;

            case EventId.ELG_TUTORIAL_UPDATE_UNIT_INFO_NOT:
                SendPacket(EventId.EGS_TUTORIAL_UPDATE_UNIT_INFO_NOT, ev.GetPayload<KEGS_TUTORIAL_UPDATE_UNIT_INFO_NOT>(Region));
                break;

            case EventId.ELG_INVITE_PVP_ROOM_NOT:
                SendPacket(EventId.EGS_INVITE_PVP_ROOM_NOT, ev.GetPayload<KEGS_INVITE_PVP_ROOM_NOT>(Region));
                break;

            case EventId.ELG_KNM_INVITE_ACK:
                SendPacket(EventId.EGS_KNM_INVITE_ACK, ev.GetPayload<KEGS_KNM_INVITE_ACK>(Region));
                break;

            case EventId.ELG_KNM_INVITE_NOT:
                SendPacket(EventId.EGS_KNM_INVITE_NOT, ev.GetPayload<KEGS_KNM_INVITE_NOT>(Region));
                break;

            case EventId.ELG_KNM_REQUEST_NEW_FRIEND_INFO_ACK:
                SendPacket(EventId.EGS_KNM_REQUEST_NEW_FRIEND_INFO_ACK, ev.GetPayload<KEGS_KNM_REQUEST_NEW_FRIEND_INFO_ACK>(Region));
                break;

            case EventId.ELG_KNM_TOGETHER_ACK:
                SendPacket(EventId.EGS_KNM_TOGETHER_ACK, ev.GetPayload<KEGS_KNM_TOGETHER_ACK>(Region));
                break;

            case EventId.ELG_LOCAL_RANKING_WATCH_UNIT_ACK:
            {
                var ack = ev.GetPayload<KELG_LOCAL_RANKING_WATCH_UNIT_ACK>(Region);
                SendPacket(EventId.EGS_LOCAL_RANKING_WATCH_UNIT_ACK, new KEGS_LOCAL_RANKING_WATCH_UNIT_ACK
                {
                    Ok = ack.Ok,
                    UserInfo = ack.UserInfo,
                    UnitInfo = ack.UnitInfo,
                });
                break;
            }

            case EventId.ELG_LOCAL_RANKING_WATCH_UNIT_REQ:
            {
                var req = ev.GetPayload<KELG_LOCAL_RANKING_WATCH_UNIT_REQ>(Region);
                SendPacket(EventId.EGS_LOCAL_RANKING_WATCH_UNIT_REQ, new KEGS_LOCAL_RANKING_WATCH_UNIT_REQ { UnitUid = req.TargetUnitUid });
                break;
            }

            case EventId.ELG_LOCAL_RANKING_RANK_UP_NOT:
                SendPacket(EventId.EGS_LOCAL_RANKING_RANK_UP_NOT, ev.GetPayload<KELG_LOCAL_RANKING_RANK_UP_NOT>(Region));
                break;

            case EventId.ELG_NEW_RECORD_RANKING_SUCCESS_NOT:
                SendPacket(EventId.EGS_NEW_RECORD_RANKING_SUCCESS_NOT, new KPacketOK { Ok = NetError.NET_OK });
                break;

            case EventId.ELG_PRESENT_MESSAGE_TO_RECEIVER_NOT:
                SendPacket(EventId.EGS_PRESENT_MESSAGE_TO_RECEIVER_NOT, ev.GetPayload<KEGS_PRESENT_MESSAGE_TO_RECEIVER_NOT>(Region));
                break;

            case EventId.ELG_PSHOP_AGENCY_MESSAGE_NOT:
                SendPacket(EventId.EGS_PSHOP_AGENCY_MESSAGE_NOT, ev.GetPayload<KEGS_PSHOP_AGENCY_MESSAGE_NOT>(Region));
                break;

            case EventId.ELG_RECOMMEND_USER_INFO_NOT:
                SendPacket(EventId.EGS_RECOMMEND_USER_INFO_NOT, ev.GetPayload<KEGS_RECOMMEND_USER_INFO_NOT>(Region));
                break;

            case EventId.ELG_RECOMMEND_USER_RESULT_NOT:
                SendPacket(EventId.EGS_RECOMMEND_USER_RESULT_NOT, ev.GetPayload<KEGS_RECOMMEND_USER_RESULT_NOT>(Region));
                break;

            case EventId.ELG_RECRUIT_RECRUITER_INFO_NOT:
            {
                var info = ev.GetPayload<KELG_RECRUIT_RECRUITER_INFO_NOT>(Region);
                RecommendManager.ModifyRecruitRecruiterState(info);
                SendPacket(EventId.EGS_RECRUIT_RECRUITER_INFO_NOT, new KEGS_RECRUIT_RECRUITER_INFO_NOT
                {
                    SenderUnitUid = info.SenderUnitUid,
                    Type = info.Type,
                    Data = info.Data,
                });
                break;
            }

            case EventId.ELG_REGISTER_RECRUITER_NOT:
                SendPacket(EventId.EGS_REGISTER_RECRUITER_NOT, ev.GetPayload<KEGS_REGISTER_RECRUITER_NOT>(Region));
                break;

            case EventId.ELG_GET_UNIT_INFO_REQ:
            {
                if (ActiveUnit == null) break;
                _server.Router.Send(PerformerTypeId, ActiveUnit.UnitUid, PerformerId.PI_LOGIN_SERVER, 0,
                    EventId.ELG_GET_UNIT_INFO_ACK, new KELG_GET_UNIT_INFO_ACK
                    {
                        Ok = NetError.NET_OK,
                        DemandUserUid = ActiveUnit.UnitUid,
                    });
                break;
            }


            // ---- Batch 4: Remaining 241 ERM_* Room / Dungeon / PvP / Field / Square / Trade / Shop / Wedding / Pet Destination Handlers ----
            case EventId.ERM_ADMIN_NPC_UNIT_CREATE_NOT:
            {
                if (ev.InProcessPayload is KEGS_ADMIN_NPC_UNIT_CREATE_NOT direct)
                    SendPacket(EventId.EGS_ADMIN_NPC_UNIT_CREATE_NOT, direct);
                else
                {
                    var req = ev.GetPayload<KEGS_ADMIN_NPC_UNIT_CREATE_REQ>(Region);
                    SendPacket(EventId.EGS_ADMIN_NPC_UNIT_CREATE_NOT, new KEGS_ADMIN_NPC_UNIT_CREATE_NOT
                    {
                        NpcUnitNot = req.NpcUnitReq,
                        Attrib1 = req.Attrib1,
                        Attrib2 = req.Attrib2,
                        Sa1 = req.Sa1,
                        Sa2 = req.Sa2,
                    });
                }
                break;
            }

            case EventId.ERM_CHANGE_SLOT_OPEN_NOT:
            {
                if (ev.InProcessPayload is KEGS_CHANGE_SLOT_OPEN_NOT direct)
                    SendPacket(EventId.EGS_CHANGE_SLOT_OPEN_NOT, direct);
                else
                {
                    var req = ev.GetPayload<KEGS_CHANGE_SLOT_OPEN_REQ>(Region);
                    SendPacket(EventId.EGS_CHANGE_SLOT_OPEN_NOT, new KEGS_CHANGE_SLOT_OPEN_NOT
                    {
                        SlotIndex = req.SlotIndex,
                        SlotState = req.SlotState,
                    });
                }
                break;
            }

            case EventId.ERM_SQUARE_UNIT_SYNC_DATA_NOT:
            {
                if (ev.InProcessPayload is KEGS_SQUARE_UNIT_SYNC_DATA_NOT direct)
                    SendPacket(EventId.EGS_SQUARE_UNIT_SYNC_DATA_NOT, direct);
                else
                {
                    var req = ev.GetPayload<KEGS_SQUARE_UNIT_SYNC_DATA_REQ>(Region);
                    SendPacket(EventId.EGS_SQUARE_UNIT_SYNC_DATA_NOT, new KEGS_SQUARE_UNIT_SYNC_DATA_NOT
                    {
                        UnitUid = ActiveUnit?.UnitUid ?? 0,
                        StateId = req.StateId,
                        StateChangeNum = req.StateChangeNum,
                        Pos = req.Pos,
                        LastTouchLineIndex = req.LastTouchLineIndex,
                        IsRight = req.IsRight,
                        Ed = ActiveUnit?.Ed ?? 0,
                    });
                }
                break;
            }

            case EventId.ERM_REQUEST_TRADE_NOT:
            {
                if (ev.InProcessPayload is KEGS_REQUEST_TRADE_NOT direct)
                {
                    if (AuthLevel >= 3 || AbuserManager.IsTradeBlockUser || AbuserManager.IsTradeBlockUnit || IsGuestUser)
                    {
                        if (ActiveUnit != null)
                        {
                            _server.SendToUnit(direct.SenderUid, EventId.EGS_REQUEST_TRADE_DENY_NOT, new KEGS_REQUEST_TRADE_DENY_NOT
                            {
                                Ok = AuthLevel >= 3 ? NetError.ERR_GM_CHAR_DENY_00 : NetError.NOT_REQUEST_TRADE_REPLY_02,
                                UnitUid = ActiveUnit.UnitUid,
                            });
                        }
                        break;
                    }
                    SendPacket(EventId.EGS_REQUEST_TRADE_NOT, direct);
                }
                else
                {
                    var req = ev.GetPayload<KERM_REQUEST_TRADE_NOT>(Region);
                    if (AuthLevel >= 3 || AbuserManager.IsTradeBlockUser || AbuserManager.IsTradeBlockUnit || IsGuestUser)
                    {
                        if (ActiveUnit != null)
                        {
                            _server.SendToUnit(req.SenderUid, EventId.EGS_REQUEST_TRADE_DENY_NOT, new KEGS_REQUEST_TRADE_DENY_NOT
                            {
                                Ok = AuthLevel >= 3 ? NetError.ERR_GM_CHAR_DENY_00 : NetError.NOT_REQUEST_TRADE_REPLY_02,
                                UnitUid = ActiveUnit.UnitUid,
                            });
                        }
                        break;
                    }
                    SendPacket(EventId.EGS_REQUEST_TRADE_NOT, new KEGS_REQUEST_TRADE_NOT
                    {
                        SenderUid = req.SenderUid,
                        SenderNickName = req.SenderNickName,
                    });
                }
                break;
            }

            case EventId.ERM_REQUEST_TRADE_REPLY_NOT:
            {
                if (ev.InProcessPayload is KEGS_REQUEST_TRADE_REPLY_NOT direct)
                    SendPacket(EventId.EGS_REQUEST_TRADE_REPLY_NOT, direct);
                else
                {
                    var req = ev.GetPayload<KERM_REQUEST_TRADE_REPLY_NOT>(Region);
                    SendPacket(EventId.EGS_REQUEST_TRADE_REPLY_NOT, new KEGS_REQUEST_TRADE_REPLY_NOT
                    {
                        ReceiverUid = req.ReceiverUid,
                        Accept = req.Ok == NetError.NET_OK,
                    });
                }
                break;
            }

            case EventId.ERM_JOIN_BATTLE_FIELD_ACK:
            {
                if (ev.InProcessPayload is KEGS_JOIN_BATTLE_FIELD_ACK direct)
                {
                    if (direct.Ok == NetError.NET_OK) { CurrentRoomUid = direct.RoomInfo.RoomUid; Fsm.TryStateTransition(KGSFSM.I_TO_ROOM, out _); }
                    SendPacket(EventId.EGS_JOIN_BATTLE_FIELD_ACK, direct);
                }
                else
                {
                    var ack = ev.GetPayload<KERM_JOIN_ROOM_ACK>(Region);
                    if (ack.Ok == NetError.NET_OK) { CurrentRoomUid = ack.RoomInfo.RoomUid; Fsm.TryStateTransition(KGSFSM.I_TO_ROOM, out _); }
                    SendPacket(EventId.EGS_JOIN_BATTLE_FIELD_ACK, new KEGS_JOIN_BATTLE_FIELD_ACK
                    {
                        Ok = ack.Ok,
                        RoomInfo = ack.RoomInfo,
                        Slots = ack.Slots,
                        UdpRelayIp = _server.PublicIp,
                        UdpRelayPort = _server.UdpRelayPort,
                    });
                }
                break;
            }

            case EventId.ERM_LOCAL_RANKING_WATCH_UNIT_ACK:
            {
                var ack = ev.GetPayload<KELG_LOCAL_RANKING_WATCH_UNIT_ACK>(Region);
                SendPacket(EventId.EGS_LOCAL_RANKING_WATCH_UNIT_ACK, new KEGS_LOCAL_RANKING_WATCH_UNIT_ACK
                {
                    Ok = ack.Ok,
                    UserInfo = ack.UserInfo,
                    UnitInfo = ack.UnitInfo,
                });
                break;
            }

            case EventId.ERM_LOCAL_RANKING_WATCH_UNIT_REQ:
            {
                var req = ev.GetPayload<KELG_LOCAL_RANKING_WATCH_UNIT_REQ>(Region);
                SendPacket(EventId.EGS_LOCAL_RANKING_WATCH_UNIT_REQ, new KEGS_LOCAL_RANKING_WATCH_UNIT_REQ { UnitUid = req.TargetUnitUid });
                break;
            }

            case EventId.ERM_BUY_PERSONAL_SHOP_ITEM_ACK:
                SendPacket(EventId.EGS_BUY_PERSONAL_SHOP_ITEM_ACK,
                    new KPacketOK { Ok = ev.GetPayload<KERM_BUY_PERSONAL_SHOP_ITEM_ACK>(Region).Ok });
                break;

            case EventId.ERM_CHAR_LEVEL_UP_NOT:
            {
                if (ev.TryReadPayload<KERM_CHAR_LEVEL_UP_NOT>(Region, out var not))
                    SendPacket(EventId.EGS_CHAR_LEVEL_UP_NOT, new KEGS_CHAR_LEVEL_UP_NOT
                    {
                        UnitUid = not.RoomUserInfo.UnitUid,
                        Level = not.RoomUserInfo.Level,
                    });
                break;
            }

            case EventId.ERM_END_GAME_NOT:
            {
                if (ev.InProcessPayload is KEGS_END_GAME_NOT direct)
                    SendPacket(EventId.EGS_END_GAME_NOT, direct);
                else
                    SendPacket(EventId.EGS_END_GAME_NOT, new KEGS_END_GAME_NOT { RoomState = 1 });
                break;
            }

            case EventId.ERM_LEAVE_GAME_ACK:
                CurrentRoomUid = 0;
                Fsm.TryStateTransition(KGSFSM.I_TO_FIELD_MAP, out _);
                SendPacket(EventId.EGS_LEAVE_GAME_ACK, new KEGS_LEAVE_GAME_ACK { Ok = ev.GetPayload<KPacketOK>(Region).Ok });
                break;

            case EventId.ERM_LEAVE_ROOM_ACK:
                CurrentRoomUid = 0;
                Fsm.TryStateTransition(KGSFSM.I_TO_FIELD_MAP, out _);
                SendPacket(EventId.EGS_LEAVE_ROOM_ACK, ev.InProcessPayload is IKSerializable lra ? lra : new KPacketOK { Ok = NetError.NET_OK });
                break;

            case EventId.ERM_LEAVE_TC_ROOM_ACK:
                CurrentRoomUid = 0;
                Fsm.TryStateTransition(KGSFSM.I_TO_FIELD_MAP, out _);
                SendPacket(EventId.EGS_LEAVE_TC_ROOM_ACK, ev.InProcessPayload is IKSerializable lta ? lta : new KPacketOK { Ok = NetError.NET_OK });
                break;

            case EventId.ERM_LEAVE_BATTLE_FIELD_NOT:
                SendPacket(EventId.EGS_LEAVE_BATTLE_FIELD_NOT, ev.GetPayload<KEGS_LEAVE_BATTLE_FIELD_NOT>(Region));
                break;

            case EventId.ERM_OPEN_BATTLE_FIELD_ACK:
            {
                var ack = ev.GetPayload<KERM_JOIN_ROOM_ACK>(Region);
                if (ack.Ok == NetError.NET_OK && ack.RoomInfo.RoomUid > 0)
                {
                    CurrentRoomUid = ack.RoomInfo.RoomUid;
                    Fsm.TryStateTransition(KGSFSM.I_TO_ROOM, out _);
                }
                SendPacket(EventId.EGS_JOIN_BATTLE_FIELD_ACK, new KEGS_JOIN_BATTLE_FIELD_ACK
                {
                    Ok = ack.Ok,
                    RoomInfo = ack.RoomInfo,
                    Slots = ack.Slots,
                    UdpRelayIp = _server.PublicIp,
                    UdpRelayPort = _server.UdpRelayPort,
                });
                break;
            }

            case EventId.ERM_OPEN_PVP_ROOM_FOR_MATCH_ACK:
            {
                var openAck = ev.GetPayload<KERM_OPEN_ROOM_ACK>(Region);
                if (openAck.Ok == NetError.NET_OK && openAck.RoomInfo.RoomUid > 0)
                {
                    CurrentRoomUid = openAck.RoomInfo.RoomUid;
                    Fsm.TryStateTransition(KGSFSM.I_TO_ROOM, out _);
                }
                break;
            }

            case EventId.ERM_OPEN_TC_ROOM_ACK:
            {
                var openAck = ev.GetPayload<KERM_OPEN_ROOM_ACK>(Region);
                if (openAck.Ok == NetError.NET_OK && openAck.RoomInfo.RoomUid > 0)
                {
                    CurrentRoomUid = openAck.RoomInfo.RoomUid;
                    Fsm.TryStateTransition(KGSFSM.I_TO_ROOM, out _);
                }
                SendPacket(EventId.EGS_CREATE_TC_ROOM_ACK, ev.InProcessPayload is KEGS_CREATE_TC_ROOM_ACK tcAck
                    ? tcAck
                    : new KEGS_CREATE_TC_ROOM_ACK { Ok = openAck.Ok });
                break;
            }

            case EventId.ERM_SINGLE_OPEN_DUNGEON_ROOM_ACK:
            case EventId.ERM_PARTY_OPEN_DUNGEON_ROOM_ACK:
            {
                var openAck = ev.GetPayload<KERM_OPEN_ROOM_ACK>(Region);
                if (openAck.Ok == NetError.NET_OK && openAck.RoomInfo.RoomUid > 0)
                {
                    CurrentRoomUid = openAck.RoomInfo.RoomUid;
                    Fsm.TryStateTransition(KGSFSM.I_TO_ROOM, out _);
                }
                SendPacket(EventId.EGS_CREATE_ROOM_ACK, new KEGS_CREATE_ROOM_ACK
                {
                    Ok = openAck.Ok,
                    RoomInfo = openAck.RoomInfo,
                    Slots = openAck.Slots,
                });
                break;
            }

            case EventId.ERM_JOIN_SQUARE_ACK:
            {
                var joinAck = ev.GetPayload<KERM_JOIN_ROOM_ACK>(Region);
                if (joinAck.Ok == NetError.NET_OK && joinAck.RoomInfo.RoomUid > 0)
                {
                    CurrentRoomUid = joinAck.RoomInfo.RoomUid;
                    Fsm.TryStateTransition(KGSFSM.I_TO_ROOM, out _);
                }
                SendPacket(EventId.EGS_JOIN_SQUARE_ACK, ev.InProcessPayload is KEGS_JOIN_SQUARE_ACK sqAck
                    ? sqAck
                    : new KEGS_JOIN_SQUARE_ACK
                    {
                        Ok = joinAck.Ok,
                        SquareInfo = new KSquareInfo
                        {
                            SquareUid = joinAck.RoomInfo.RoomUid,
                            RoomName = joinAck.RoomInfo.RoomName,
                            MaxSlot = joinAck.RoomInfo.MaxSlot,
                            JoinSlot = joinAck.RoomInfo.JoinSlot,
                            RelayIp = _server.PublicIp,
                            Port = _server.UdpRelayPort,
                        },
                    });
                break;
            }

            case EventId.ERM_OPEN_WEDDING_HALL_ACK:
            case EventId.ERM_JOIN_WEDDING_HALL_ACK:
            {
                var joinAck = ev.GetPayload<KERM_JOIN_ROOM_ACK>(Region);
                if (joinAck.Ok == NetError.NET_OK && joinAck.RoomInfo.RoomUid > 0)
                {
                    CurrentRoomUid = joinAck.RoomInfo.RoomUid;
                    Fsm.TryStateTransition(KGSFSM.I_TO_ROOM, out _);
                }
                SendPacket(EventId.EGS_JOIN_WEDDING_HALL_ACK, ev.InProcessPayload is KEGS_JOIN_WEDDING_HALL_ACK whAck
                    ? whAck
                    : new KEGS_JOIN_WEDDING_HALL_ACK
                    {
                        Ok = joinAck.Ok,
                        RoomInfo = joinAck.RoomInfo,
                        Slots = joinAck.Slots,
                        UdpRelayIp = _server.PublicIp,
                        UdpRelayPort = _server.UdpRelayPort,
                    });
                break;
            }

            case EventId.ERM_COMMUNITY_IN_BATTLE_FIELD_ACK:
                if (ev.InProcessPayload is KEGS_COMMUNITY_USER_LIST_ACK cul)
                    SendPacket(EventId.EGS_COMMUNITY_USER_LIST_ACK, cul);
                break;

            case EventId.ERM_CHECK_ZOMBIE_USER_REQ:
            {
                if (ActiveUnit != null)
                    _server.Router.Send(PerformerTypeId, ActiveUnit.UnitUid, PerformerId.PI_CENTER_SERVER, _server.Center.Uid,
                        EventId.ERM_CHECK_ZOMBIE_USER_ACK, new KERM_CHECK_ZOMBIE_USER_ACK
                        {
                            ZombieCheckType = KERM_CHECK_ZOMBIE_USER_ACK.ZCT_CONNECT_NORMAL,
                        });
                break;
            }

            case EventId.ERM_GET_UNIT_INFO_REQ:
            {
                if (ActiveUnit != null)
                    _server.Router.Send(PerformerTypeId, ActiveUnit.UnitUid, PerformerId.PI_CENTER_SERVER, _server.Center.Uid,
                        EventId.ERM_GET_UNIT_INFO_ACK, new KPacketOK { Ok = NetError.NET_OK });
                break;
            }
            case EventId.ERM_ADD_ON_STAT_ACK: ForwardRoomPacketWithOk(ev, EventId.EGS_ADD_ON_STAT_ACK, static ok => new KEGS_ADD_ON_STAT_ACK { Ok = ok }); break;
            case EventId.ERM_ADD_ON_STAT_NOT: ForwardRoomPacket<KEGS_ADD_ON_STAT_NOT>(ev, EventId.EGS_ADD_ON_STAT_NOT); break;
            case EventId.ERM_ADMIN_BATTLE_FIELD_INCREASE_DANGER_ACK: ForwardRoomPacketWithOk(ev, EventId.EGS_ADMIN_BATTLE_FIELD_INCREASE_DANGER_ACK, static ok => new KEGS_ADMIN_BATTLE_FIELD_INCREASE_DANGER_ACK { Ok = ok }); break;
            case EventId.ERM_ADMIN_NPC_UNIT_CREATE_ACK: ForwardRoomPacket(ev, EventId.EGS_ADMIN_NPC_UNIT_CREATE_ACK); break;
            case EventId.ERM_APPROVE_TRADE_ACK: ForwardRoomPacket(ev, EventId.EGS_APPROVE_TRADE_ACK); break;
            case EventId.ERM_APPROVE_TRADE_NOT: ForwardRoomPacket(ev, EventId.EGS_APPROVE_TRADE_NOT); break;
            case EventId.ERM_BAD_ATTITUDE_USER_MSG_NOT: ForwardRoomPacket<KEGS_BAD_ATTITUDE_USER_MSG_NOT>(ev, EventId.EGS_BAD_ATTITUDE_USER_MSG_NOT); break;
            case EventId.ERM_BAD_ATTITUDE_USER_NOT: ForwardRoomPacket(ev, EventId.EGS_BAD_ATTITUDE_USER_NOT); break;
            case EventId.ERM_BATTLE_FIELD_EVENT_MESSAGE_NOT: ForwardRoomPacket<KEGS_BATTLE_FIELD_EVENT_MESSAGE_NOT>(ev, EventId.EGS_BATTLE_FIELD_EVENT_MESSAGE_NOT); break;
            case EventId.ERM_BATTLE_FIELD_NPC_DAMAGED_MAP_ACK: ForwardRoomPacket<KEGS_BATTLE_FIELD_NPC_DAMAGED_MAP_ACK>(ev, EventId.EGS_BATTLE_FIELD_NPC_DAMAGED_MAP_ACK); break;
            case EventId.ERM_BATTLE_FIELD_NPC_LOAD_COMPLETE_ACK: ForwardRoomPacket(ev, EventId.EGS_BATTLE_FIELD_NPC_LOAD_COMPLETE_ACK); break;
            case EventId.ERM_BATTLE_FIELD_NPC_LOAD_NOT: ForwardRoomPacket(ev, EventId.EGS_BATTLE_FIELD_NPC_LOAD_NOT); break;
            case EventId.ERM_BATTLE_FIELD_NPC_P2P_SYNC_NOT: ForwardRoomPacket<KEGS_BATTLE_FIELD_NPC_P2P_SYNC_NOT>(ev, EventId.EGS_BATTLE_FIELD_NPC_P2P_SYNC_NOT); break;
            case EventId.ERM_BOSS_FIELD_CLOSE_PROCESS_NOT: ForwardRoomPacket(ev, EventId.EGS_BOSS_FIELD_CLOSE_PROCESS_NOT); break;
            case EventId.ERM_BOSS_FIELD_INTRUDE_RESTRICTION_ACK: ForwardRoomPacketWithOk(ev, EventId.EGS_BOSS_FIELD_INTRUDE_RESTRICTION_ACK, static ok => new KEGS_BOSS_FIELD_INTRUDE_RESTRICTION_ACK { Ok = ok }); break;
            case EventId.ERM_BOSS_FIELD_RETURN_PROCESS_NOT:
            {
                if (ev.InProcessPayload is KEGS_BOSS_FIELD_RETURN_PROCESS_NOT bfPkt)
                {
                    SendPacket(EventId.EGS_BOSS_FIELD_RETURN_PROCESS_NOT, bfPkt);
                }
                else
                {
                    int curMapId = CurrentFieldMapId > 0 ? CurrentFieldMapId : (ActiveUnit?.LastPos.MapId ?? 0);
                    bool isBossField = curMapId == _server.Global.FieldBossManager.PortalDestination || curMapId >= 50000;
                    int returnMapId = isBossField && ActiveUnit != null
                        ? _server.GetPossibleEnterVillageMapId(ActiveUnit.Level, ActiveUnit.DungeonClear, curMapId)
                        : curMapId;
                    SendPacket(EventId.EGS_BOSS_FIELD_RETURN_PROCESS_NOT, new KEGS_BOSS_FIELD_RETURN_PROCESS_NOT
                    {
                        ReturnMapId = returnMapId,
                        LastTouchLineIndex = ActiveUnit?.LastPos.LastTouchLineIndex ?? 0,
                        LastPosValue = ActiveUnit?.LastPos.LastPosValue ?? 0,
                        IgnoreLastTouch = isBossField,
                    });
                }
                break;
            }
            case EventId.ERM_BREAK_PERSONAL_SHOP_ACK:
            {
                var ack = ev.TryReadPayload<KPacketOK>(Region, out var okPkt) ? okPkt : new KPacketOK { Ok = NetError.NET_OK };
                if (ack.Ok == NetError.NET_OK)
                {
                    PersonalShopManager.CloseShop();
                    PersonalShopManager.LeaveJoinedShop();
                    if (CurrentRoomUid > 0)
                    {
                        var room = _server.Center.RoomManager.GetRoom(CurrentRoomUid);
                        if (room?.Info.RoomType == 4) // RT_SQUARE
                        {
                            _server.Router.Send(PerformerTypeId, ActiveUnit?.UnitUid ?? Uid, PerformerId.PI_CENTER_SERVER, _server.Center.Uid,
                                EventId.ERM_UPDATE_SQUARE_PERSONAL_SHOP_INFO_NOT, new KPacketOK { Ok = NetError.NET_OK });
                        }
                    }
                }
                SendPacket(EventId.EGS_BREAK_PERSONAL_SHOP_ACK, ack);
                break;
            }
            case EventId.ERM_BREAK_PERSONAL_SHOP_NOT:
            {
                int reason = NetError.NOT_LEAVE_ROOM_REASON_01;
                if (ev.InProcessPayload is KEGS_BREAK_PERSONAL_SHOP_NOT clientBreak)
                {
                    reason = clientBreak.Reason;
                }
                else if (ev.TryReadPayload<KERM_BREAK_PERSONAL_SHOP_NOT>(Region, out var srvBreak))
                {
                    reason = srvBreak.Reason;
                    if (ActiveUnit != null && srvBreak.HostUid == ActiveUnit.UnitUid && CurrentRoomUid > 0)
                    {
                        var room = _server.Center.RoomManager.GetRoom(CurrentRoomUid);
                        if (room?.Info.RoomType == 4)
                        {
                            _server.Router.Send(PerformerTypeId, ActiveUnit.UnitUid, PerformerId.PI_CENTER_SERVER, _server.Center.Uid,
                                EventId.ERM_UPDATE_SQUARE_PERSONAL_SHOP_INFO_NOT, new KPacketOK { Ok = NetError.NET_OK });
                        }
                    }
                }
                else if (ev.TryReadPayload<KPacketReason>(Region, out var pr))
                {
                    reason = pr.Reason;
                }
                PersonalShopManager.CloseShop();
                PersonalShopManager.LeaveJoinedShop();
                SendPacket(EventId.EGS_BREAK_PERSONAL_SHOP_NOT, new KEGS_BREAK_PERSONAL_SHOP_NOT { Reason = reason });
                break;
            }
            case EventId.ERM_BREAK_TRADE_ACK:
            {
                var ack = ev.TryReadPayload<KPacketOK>(Region, out var okPkt) ? okPkt : new KPacketOK { Ok = NetError.NET_OK };
                if (ack.Ok == NetError.NET_OK)
                {
                    CurrentTradeUid = 0;
                    if (Fsm.StateId == KGSFSM.S_ROOM && ActiveUnit != null && CurrentRoomUid > 0)
                    {
                        _server.Router.Send(PerformerTypeId, ActiveUnit.UnitUid, PerformerId.PI_CENTER_SERVER, _server.Center.Uid,
                            EventId.ERM_TRADE_FINISH_NOT, new KPacketOK { Ok = NetError.NET_OK });
                    }
                }
                SendPacket(EventId.EGS_BREAK_TRADE_ACK, ack);
                break;
            }
            case EventId.ERM_BREAK_TRADE_NOT:
            {
                CurrentTradeUid = 0;
                int reason = ev.TryReadPayload<KPacketReason>(Region, out var pr) ? pr.Reason : NetError.NOT_LEAVE_ROOM_REASON_16;
                if (reason == NetError.NOT_LEAVE_ROOM_REASON_00)
                    reason = NetError.NOT_LEAVE_ROOM_REASON_16;
                if (reason != NetError.NOT_LEAVE_ROOM_REASON_06)
                    SendPacket(EventId.EGS_BREAK_TRADE_NOT, new KPacketReason { Reason = reason });
                if (Fsm.StateId == KGSFSM.S_ROOM && ActiveUnit != null && CurrentRoomUid > 0)
                {
                    _server.Router.Send(PerformerTypeId, ActiveUnit.UnitUid, PerformerId.PI_CENTER_SERVER, _server.Center.Uid,
                        EventId.ERM_TRADE_FINISH_NOT, new KPacketOK { Ok = NetError.NET_OK });
                }
                break;
            }
            case EventId.ERM_BREAK_TRADE_REQ:
            {
                if (CurrentTradeUid > 0 && ActiveUnit != null)
                {
                    var req = ev.TryReadPayload<KPacketReason>(Region, out var pr) ? pr : new KPacketReason { Reason = NetError.NOT_LEAVE_ROOM_REASON_16 };
                    _server.Router.Send(PerformerTypeId, ActiveUnit.UnitUid, PerformerId.PI_CENTER_SERVER, _server.Center.Uid,
                        EventId.ERM_BREAK_TRADE_REQ, req);
                }
                break;
            }
            case EventId.ERM_BUFF_SKILL_INFO_CONVEY_ACK: ForwardRoomPacketWithOk(ev, EventId.EGS_BUFF_SKILL_INFO_CONVEY_ACK, static ok => new KEGS_BUFF_SKILL_INFO_CONVEY_ACK { Ok = ok }); break;
            case EventId.ERM_BUY_PERSONAL_SHOP_ITEM_COMPLETE_NOT:
            {
                if (ev.InProcessPayload is KEGS_BUY_PERSONAL_SHOP_ITEM_COMPLETE_NOT directBuy)
                {
                    SendPacket(EventId.EGS_BUY_PERSONAL_SHOP_ITEM_COMPLETE_NOT, directBuy);
                    break;
                }
                if (ev.InProcessPayload is KDBE_BUY_PERSONAL_SHOP_ITEM_COMPLETE_REQ dbReq ||
                    (ev.Buffer.Length > 0 && ev.TryReadPayload<KDBE_BUY_PERSONAL_SHOP_ITEM_COMPLETE_REQ>(Region, out dbReq!)))
                {
                    if (ActiveUnit != null)
                    {
                        var updatedSlots = new List<KInventoryItemInfo>();
                        if (dbReq.SellUnitUid == ActiveUnit.UnitUid)
                        {
                            EdManager.TryAddEd(dbReq.SellUnitEDIN, KUserEDManager.EdReason.ErSetEdPersonalShopSellItem, out int nextEd);
                            ActiveUnit.Ed = nextEd;
                            _server.Db.SetUnitEd(ActiveUnit.UnitUid, nextEd);
                            if (dbReq.SellItemInfo.ItemUid > 0)
                            {
                                var delRes = _server.Db.DeleteInventoryItem(ActiveUnit.UnitUid, dbReq.SellItemInfo.ItemUid, dbReq.SellItemInfo.ItemInfo.Quantity);
                                if (delRes.ok) updatedSlots.Add(delRes.updatedSlot);
                            }
                            SendPacket(EventId.EGS_BUY_PERSONAL_SHOP_ITEM_COMPLETE_NOT, new KEGS_BUY_PERSONAL_SHOP_ITEM_COMPLETE_NOT
                            {
                                Ed = ActiveUnit.Ed,
                                InventorySlotInfo = updatedSlots,
                            });
                            break;
                        }
                        if (dbReq.BuyUnitUid == ActiveUnit.UnitUid)
                        {
                            EdManager.TryAddEd(dbReq.BuyUnitEDOUT, KUserEDManager.EdReason.ErSetEdPersonalShopBuyItem, out int nextEd);
                            ActiveUnit.Ed = nextEd;
                            _server.Db.SetUnitEd(ActiveUnit.UnitUid, nextEd);
                            if (dbReq.BuyItemInfo.ItemUid > 0)
                            {
                                var granted = _server.Db.GrantUnitItem(
                                    ActiveUnit.UnitUid,
                                    dbReq.BuyItemInfo.ItemInfo.ItemId,
                                    Math.Max(1, dbReq.BuyItemInfo.ItemInfo.Quantity),
                                    dbReq.BuyItemInfo.SlotCategory > 0 ? dbReq.BuyItemInfo.SlotCategory : (sbyte)1);
                                updatedSlots.Add(granted);
                            }
                            SendPacket(EventId.EGS_BUY_PERSONAL_SHOP_ITEM_COMPLETE_NOT, new KEGS_BUY_PERSONAL_SHOP_ITEM_COMPLETE_NOT
                            {
                                Ed = ActiveUnit.Ed,
                                InventorySlotInfo = updatedSlots,
                            });
                            break;
                        }
                    }
                }
                if (ev.TryReadPayload<KERM_BUY_PERSONAL_SHOP_ITEM_COMPLETE_NOT>(Region, out var ermBuy) && ActiveUnit != null)
                {
                    var updatedSlots = new List<KInventoryItemInfo>();
                    if (ermBuy.SellUnitUid == ActiveUnit.UnitUid)
                    {
                        EdManager.TryAddEd(ermBuy.SellUnitEDIN, KUserEDManager.EdReason.ErSetEdPersonalShopSellItem, out int nextEd);
                        ActiveUnit.Ed = nextEd;
                        _server.Db.SetUnitEd(ActiveUnit.UnitUid, nextEd);
                        if (ermBuy.SellIntemInfoOUT.ItemUid > 0)
                        {
                            var delRes = _server.Db.DeleteInventoryItem(ActiveUnit.UnitUid, ermBuy.SellIntemInfoOUT.ItemUid, ermBuy.SellIntemInfoOUT.ItemInfo.Quantity);
                            if (delRes.ok) updatedSlots.Add(delRes.updatedSlot);
                        }
                    }
                    else if (ermBuy.PriceUnitUid == ActiveUnit.UnitUid)
                    {
                        EdManager.TryAddEd(ermBuy.PriceUnitEDOUT, KUserEDManager.EdReason.ErSetEdPersonalShopBuyItem, out int nextEd);
                        ActiveUnit.Ed = nextEd;
                        _server.Db.SetUnitEd(ActiveUnit.UnitUid, nextEd);
                        if (ermBuy.PriceIntemInfoIN.ItemUid > 0)
                        {
                            var granted = _server.Db.GrantUnitItem(
                                ActiveUnit.UnitUid,
                                ermBuy.PriceIntemInfoIN.ItemInfo.ItemId,
                                Math.Max(1, ermBuy.PriceIntemInfoIN.ItemInfo.Quantity),
                                ermBuy.PriceIntemInfoIN.SlotCategory > 0 ? ermBuy.PriceIntemInfoIN.SlotCategory : (sbyte)1);
                            updatedSlots.Add(granted);
                        }
                    }
                    SendPacket(EventId.EGS_BUY_PERSONAL_SHOP_ITEM_COMPLETE_NOT, new KEGS_BUY_PERSONAL_SHOP_ITEM_COMPLETE_NOT
                    {
                        Ed = ActiveUnit.Ed,
                        InventorySlotInfo = updatedSlots,
                    });
                    break;
                }
                ForwardRoomPacket<KEGS_BUY_PERSONAL_SHOP_ITEM_COMPLETE_NOT>(ev, EventId.EGS_BUY_PERSONAL_SHOP_ITEM_COMPLETE_NOT);
                break;
            }
            case EventId.ERM_CHANGED_PET_INTIMACY_NOT: ForwardRoomPacket<KEGS_CHANGED_PET_INTIMACY_NOT>(ev, EventId.EGS_CHANGED_PET_INTIMACY_NOT); break;
            case EventId.ERM_CHANGED_PET_SATIETY_INTIMACY_NOT: ForwardRoomPacket(ev, EventId.EGS_CHANGED_PET_SATIETY_INTIMACY_NOT); break;
            case EventId.ERM_CHANGED_PET_SATIETY_NOT: ForwardRoomPacket<KEGS_CHANGED_PET_SATIETY_NOT>(ev, EventId.EGS_CHANGED_PET_SATIETY_NOT); break;
            case EventId.ERM_CHANGE_EQUIPPED_ITEM_IN_FIELD_NOT: ForwardRoomPacket<KEGS_CHANGE_EQUIPPED_ITEM_IN_FIELD_NOT>(ev, EventId.EGS_CHANGE_EQUIPPED_ITEM_IN_FIELD_NOT); break;
            case EventId.ERM_CHANGE_EQUIPPED_ITEM_IN_ROOM_NOT:
            {
                if (ev.InProcessPayload is KEGS_CHANGE_EQUIPPED_ITEM_IN_ROOM_NOT eqDirect)
                {
                    SendPacket(EventId.EGS_CHANGE_EQUIPPED_ITEM_IN_ROOM_NOT, eqDirect);
                }
                else if (ev.TryReadPayload<KERM_CHANGE_EQUIPPED_ITEM_IN_ROOM_NOT>(Region, out var eqRm))
                {
                    var targetUser = _server.GetUserByUnitUid(eqRm.UnitUid);
                    SendPacket(EventId.EGS_CHANGE_EQUIPPED_ITEM_IN_ROOM_NOT, new KEGS_CHANGE_EQUIPPED_ITEM_IN_ROOM_NOT
                    {
                        ChangeEquippedPacketType = eqRm.ChangeEquippedPacketType,
                        UnitUid = eqRm.UnitUid,
                        InventorySlotInfo = eqRm.InventorySlotInfo,
                        GameStat = targetUser?.ActiveUnit?.Stat ?? new KStat(),
                        EquippedTitleId = eqRm.EquippedTitleId,
                    });
                }
                else
                {
                    ForwardRoomPacket(ev, EventId.EGS_CHANGE_EQUIPPED_ITEM_IN_ROOM_NOT);
                }
                break;
            }
            case EventId.ERM_CHANGE_EQUIPPED_ITEM_IN_SQUARE_NOT: ForwardRoomPacket(ev, EventId.EGS_CHANGE_EQUIPPED_ITEM_IN_SQUARE_NOT); break;
            case EventId.ERM_CHANGE_GAME_STAT_INFO_IN_ROOM_NOT: ForwardRoomPacket(ev, EventId.EGS_CHANGE_GAME_STAT_INFO_IN_ROOM_NOT); break;
            case EventId.ERM_CHANGE_PVP_GAME_OPTION_ACK: ForwardRoomPacketWithOk(ev, EventId.EGS_CHANGE_PVP_GAME_OPTION_ACK, static ok => new KEGS_CHANGE_PVP_GAME_OPTION_ACK { Ok = ok }); break;
            case EventId.ERM_CHANGE_PVP_GAME_OPTION_NOT: ForwardRoomPacket<KEGS_CHANGE_PVP_GAME_OPTION_NOT>(ev, EventId.EGS_CHANGE_PVP_GAME_OPTION_NOT); break;
            case EventId.ERM_CHANGE_PVP_ITEMMODE_ACK: ForwardRoomPacketWithOk(ev, EventId.EGS_CHANGE_PVP_ITEMMODE_ACK, static ok => new KEGS_CHANGE_PVP_ITEMMODE_ACK { Ok = ok }); break;
            case EventId.ERM_CHANGE_PVP_ITEMMODE_NOT: ForwardRoomPacket<KEGS_CHANGE_PVP_ITEMMODE_NOT>(ev, EventId.EGS_CHANGE_PVP_ITEMMODE_NOT); break;
            case EventId.ERM_CHANGE_PVP_ITEM_NOT: ForwardRoomPacket(ev, EventId.EGS_CHANGE_PVP_ITEM_NOT); break;
            case EventId.ERM_CHANGE_PVP_ITEM_POS_ACK: ForwardRoomPacketWithOk(ev, EventId.EGS_CHANGE_PVP_ITEM_POS_ACK, static ok => new KEGS_CHANGE_PVP_ITEM_POS_ACK { Ok = ok }); break;
            case EventId.ERM_CHANGE_PVP_ITEM_POS_NOT: ForwardRoomPacket<KEGS_CHANGE_PVP_ITEM_POS_NOT>(ev, EventId.EGS_CHANGE_PVP_ITEM_POS_NOT); break;
            case EventId.ERM_CHANGE_SKILL_INFO_IN_ROOM_NOT:
            {
                if (ev.InProcessPayload is KEGS_CHANGE_SKILL_INFO_IN_ROOM_NOT skDirect)
                {
                    SendPacket(EventId.EGS_CHANGE_SKILL_INFO_IN_ROOM_NOT, skDirect);
                }
                else if (ev.InProcessPayload is KRoomUserInfo rui ||
                         (ev.Buffer.Length > 0 && ev.TryReadPayload<KRoomUserInfo>(Region, out rui!)))
                {
                    SendPacket(EventId.EGS_CHANGE_SKILL_INFO_IN_ROOM_NOT, new KEGS_CHANGE_SKILL_INFO_IN_ROOM_NOT
                    {
                        UnitUid = rui.UnitUid,
                        MemberShipGrade = rui.MemberShipGrade,
                        GameStat = rui.GameStat,
                        UnitSkillData = rui.UnitSkillData,
                    });
                }
                else
                {
                    ForwardRoomPacket(ev, EventId.EGS_CHANGE_SKILL_INFO_IN_ROOM_NOT);
                }
                break;
            }
            case EventId.ERM_CHANGE_SLOT_OPEN_ACK: ForwardRoomPacketWithOk(ev, EventId.EGS_CHANGE_SLOT_OPEN_ACK, static ok => new KEGS_CHANGE_SLOT_OPEN_ACK { Ok = ok }); break;
            case EventId.ERM_CHANGE_TRADE_NOT: ForwardRoomPacket(ev, EventId.EGS_CHANGE_TRADE_NOT); break;
            case EventId.ERM_CHANGE_WINNING_NUM_KILL_ACK: ForwardRoomPacketWithOk(ev, EventId.EGS_CHANGE_WINNING_NUM_KILL_ACK, static ok => new KEGS_CHANGE_WINNING_NUM_KILL_ACK { Ok = ok }); break;
            case EventId.ERM_CHANGE_WINNING_NUM_KILL_NOT: ForwardRoomPacket<KEGS_CHANGE_WINNING_NUM_KILL_NOT>(ev, EventId.EGS_CHANGE_WINNING_NUM_KILL_NOT); break;
            case EventId.ERM_CHECK_EXIST_MONSTER_UID_ACK: ForwardRoomPacketWithOk(ev, EventId.EGS_CHECK_EXIST_MONSTER_UID_ACK, static ok => new KEGS_CHECK_EXIST_MONSTER_UID_ACK { Ok = ok }); break;
            case EventId.ERM_CHECK_INVALID_ROOM_NOT:
            {
                if (ev.InProcessPayload is KEGS_CHECK_INVALID_ROOM_NOT invDirect)
                {
                    CurrentRoomUid = 0;
                    if (Fsm.StateId == KGSFSM.S_ROOM)
                        Fsm.TryStateTransition(KGSFSM.I_TO_FIELD_MAP, out _);
                    SendPacket(EventId.EGS_CHECK_INVALID_ROOM_NOT, invDirect);
                    break;
                }
                long deadServerUid = ev.TryReadPayload<KPacketLong>(Region, out var srvPkt) ? srvPkt.Value : 0;
                if (CurrentRoomUid > 0 && (deadServerUid == 0 || CurrentRoomUid > 0))
                {
                    int mapId = ActiveUnit != null
                        ? _server.GetPossibleEnterVillageMapId(ActiveUnit.Level, ActiveUnit.DungeonClear, CurrentFieldMapId > 0 ? CurrentFieldMapId : ActiveUnit.LastPos.MapId)
                        : CurrentFieldMapId;
                    CurrentRoomUid = 0;
                    if (Fsm.StateId == KGSFSM.S_ROOM)
                        Fsm.TryStateTransition(KGSFSM.I_TO_FIELD_MAP, out _);
                    SendPacket(EventId.EGS_CHECK_INVALID_ROOM_NOT, new KEGS_CHECK_INVALID_ROOM_NOT
                    {
                        Ok = NetError.NET_OK,
                        MapId = mapId,
                    });
                    if (CurrentPartyUid > 0 && ActiveUnit != null)
                        _server.Center.PartyManager.LeaveParty(CurrentPartyUid, ActiveUnit.UnitUid);
                }
                if (CurrentTradeUid > 0 && deadServerUid > 0)
                {
                    CurrentTradeUid = 0;
                    SendPacket(EventId.EGS_BREAK_TRADE_NOT, new KPacketReason { Reason = NetError.NOT_REQUEST_TRADE_REPLY_04 });
                }
                if (PersonalShopManager.PersonalShopUid > 0 && deadServerUid > 0)
                {
                    PersonalShopManager.CloseShop();
                    PersonalShopManager.LeaveJoinedShop();
                    SendPacket(EventId.EGS_BREAK_PERSONAL_SHOP_NOT, new KEGS_BREAK_PERSONAL_SHOP_NOT { Reason = NetError.NOT_LEAVE_ROOM_REASON_01 });
                }
                break;
            }
            case EventId.ERM_CHECK_PING_SCORE_CHANGE_HOST_NOT: ForwardRoomPacket(ev, EventId.EGS_CHECK_PING_SCORE_CHANGE_HOST_NOT); break;
            case EventId.ERM_CLEAR_TEMP_INVENTORY_NOT:
                TempInventory.Clear();
                break;
            case EventId.ERM_COUPLE_MAKING_SUCCESS_NOT:
            {
                if (ev.InProcessPayload is KEGS_COUPLE_MAKING_SUCCESS_NOT coupleDirect)
                {
                    SendPacket(EventId.EGS_COUPLE_MAKING_SUCCESS_NOT, coupleDirect);
                }
                else if (ev.TryReadPayload<KERM_COUPLE_MAKING_SUCCESS_NOT>(Region, out var coupleErm))
                {
                    SendPacket(EventId.EGS_COUPLE_MAKING_SUCCESS_NOT, new KEGS_COUPLE_MAKING_SUCCESS_NOT
                    {
                        RequestUnitName = coupleErm.RequestUnitName,
                        AcceptUnitName = coupleErm.AcceptUnitName,
                    });
                }
                else
                {
                    ForwardRoomPacket<KEGS_COUPLE_MAKING_SUCCESS_NOT>(ev, EventId.EGS_COUPLE_MAKING_SUCCESS_NOT);
                }
                break;
            }
            case EventId.ERM_CREATE_ATTRIB_NPC_NOT: ForwardRoomPacket(ev, EventId.EGS_CREATE_ATTRIB_NPC_NOT); break;
            case EventId.ERM_CREATE_PVP_ITEM_ACK: ForwardRoomPacketWithOk(ev, EventId.EGS_CREATE_PVP_ITEM_ACK, static ok => new KEGS_CREATE_PVP_ITEM_ACK { Ok = ok }); break;
            case EventId.ERM_CREATE_PVP_ITEM_NOT: ForwardRoomPacket<KEGS_CREATE_PVP_ITEM_NOT>(ev, EventId.EGS_CREATE_PVP_ITEM_NOT); break;
            case EventId.ERM_CURRENT_KILL_SCORE_NOT: ForwardRoomPacket(ev, EventId.EGS_CURRENT_KILL_SCORE_NOT); break;
            case EventId.ERM_CURRENT_TEAM_KILL_SCORE_NOT: ForwardRoomPacket(ev, EventId.EGS_CURRENT_TEAM_KILL_SCORE_NOT); break;
            case EventId.ERM_DECISION_ENTER_DEFENCE_DUNGEON_NOT: ForwardRoomPacket(ev, EventId.EGS_DECISION_ENTER_DEFENCE_DUNGEON_NOT); break;
            case EventId.ERM_DEFENCE_DUNGEON_GAME_START_NOT:
            {
                if (ev.TryReadPayload<KEGS_DEFENCE_DUNGEON_GAME_START_NOT>(Region, out var ddStart))
                {
                    if (ddStart.InventorySlotInfo.Count == 0 && ActiveUnit != null)
                        ddStart.InventorySlotInfo = SetupDefenceDungeonQuickSlotItems(KDefenceDungeonManager.DdpPhase1);
                    SendPacket(EventId.EGS_DEFENCE_DUNGEON_GAME_START_NOT, ddStart);
                }
                else
                {
                    ForwardRoomPacket(ev, EventId.EGS_DEFENCE_DUNGEON_GAME_START_NOT);
                }
                break;
            }
            case EventId.ERM_DROP_ITEM_NOT:
            {
                if (ev.TryReadPayload<KEGS_DROP_ITEM_NOT>(Region, out var drop))
                {
                    if (CurrentRoomUid > 0)
                    {
                        var room = _server.Center.RoomManager.GetRoom(CurrentRoomUid);
                        if (room != null && room.Info.RoomType == 3 && SpiritManager.Spirit <= 0)
                            drop.DropItemDataList.Clear();
                    }
                    SendPacket(EventId.EGS_DROP_ITEM_NOT, drop);
                }
                else
                {
                    ForwardRoomPacket(ev, EventId.EGS_DROP_ITEM_NOT);
                }
                break;
            }
            case EventId.ERM_DUNGEON_EFFECT_REMAINING_TIME_NOT: ForwardRoomPacket(ev, EventId.EGS_DUNGEON_EFFECT_REMAINING_TIME_NOT); break;
            case EventId.ERM_DUNGEON_EFFECT_TIME_OUT_NOT: ForwardRoomPacket(ev, EventId.EGS_DUNGEON_EFFECT_TIME_OUT_NOT); break;
            case EventId.ERM_DUNGEON_KILLALLNPC_CHECK_ACK: ForwardRoomPacket<KEGS_DUNGEON_KILLALLNPC_CHECK_ACK>(ev, EventId.EGS_DUNGEON_KILLALLNPC_CHECK_ACK); break;
            case EventId.ERM_DUNGEON_KILLALLNPC_CHECK_NOT: ForwardRoomPacket(ev, EventId.EGS_DUNGEON_KILLALLNPC_CHECK_NOT); break;
            case EventId.ERM_DUNGEON_SECRET_STAGE_ENTER_CHECK_ACK: ForwardRoomPacketWithOk(ev, EventId.EGS_DUNGEON_SECRET_STAGE_ENTER_CHECK_ACK, static ok => new KEGS_DUNGEON_SECRET_STAGE_ENTER_CHECK_ACK { Ok = ok }); break;
            case EventId.ERM_DUNGEON_SECRET_STAGE_ENTER_CHECK_NOT: ForwardRoomPacket<KEGS_DUNGEON_SECRET_STAGE_ENTER_CHECK_NOT>(ev, EventId.EGS_DUNGEON_SECRET_STAGE_ENTER_CHECK_NOT); break;
            case EventId.ERM_DUNGEON_SECRET_STAGE_ENTER_RESULT_NOT: ForwardRoomPacket(ev, EventId.EGS_DUNGEON_SECRET_STAGE_ENTER_RESULT_NOT); break;
            case EventId.ERM_DUNGEON_STAGE_LOAD_COMPLETE_ACK: ForwardRoomPacket(ev, EventId.EGS_DUNGEON_STAGE_LOAD_COMPLETE_ACK); break;
            case EventId.ERM_DUNGEON_STAGE_LOAD_COMPLETE_NOT: ForwardRoomPacket<KEGS_DUNGEON_STAGE_LOAD_COMPLETE_NOT>(ev, EventId.EGS_DUNGEON_STAGE_LOAD_COMPLETE_NOT); break;
            case EventId.ERM_DUNGEON_STAGE_START_NOT: ForwardRoomPacket<KEGS_DUNGEON_STAGE_START_NOT>(ev, EventId.EGS_DUNGEON_STAGE_START_NOT); break;
            case EventId.ERM_DUNGEON_SUB_STAGE_GO_NEXT_ACK: ForwardRoomPacket(ev, EventId.EGS_DUNGEON_SUB_STAGE_GO_NEXT_ACK); break;
            case EventId.ERM_DUNGEON_SUB_STAGE_GO_NEXT_ALL_NOT: ForwardRoomPacket(ev, EventId.EGS_DUNGEON_SUB_STAGE_GO_NEXT_ALL_NOT); break;
            case EventId.ERM_DUNGEON_SUB_STAGE_LOAD_COMPLETE_ACK: ForwardRoomPacket(ev, EventId.EGS_DUNGEON_SUB_STAGE_LOAD_COMPLETE_ACK); break;
            case EventId.ERM_DUNGEON_SUB_STAGE_START_NOT:
            {
                ForwardRoomPacket(ev, EventId.EGS_DUNGEON_SUB_STAGE_START_NOT);
                if (_reserveKickUser && ActiveUnit != null && CurrentRoomUid > 0)
                {
                    _reserveKickUser = false;
                    int reqItem = _dungeonRequiredItemId;
                    _dungeonRequiredItemId = 0;
                    _server.Router.Send(PerformerTypeId, ActiveUnit.UnitUid, PerformerId.PI_CENTER_SERVER, _server.Center.Uid,
                        EventId.ERM_LEAVE_ROOM_REQ, new KERM_LEAVE_ROOM_REQ
                        {
                            RoomUid = CurrentRoomUid,
                            UnitUid = ActiveUnit.UnitUid,
                            Reason = NetError.NOT_LEAVE_ROOM_REASON_22,
                        });
                }
                break;
            }
            case EventId.ERM_END_GAME_ACK: ForwardRoomPacket(ev, EventId.EGS_END_GAME_ACK); break;
            case EventId.ERM_END_GAME_DUNGEON_RESULT_DATA_NOT: ForwardRoomPacket<KEGS_END_GAME_DUNGEON_RESULT_DATA_NOT>(ev, EventId.EGS_END_GAME_DUNGEON_RESULT_DATA_NOT); break;
            case EventId.ERM_END_GAME_PVP_RESULT_DATA_NOT: ForwardRoomPacket<KEGS_END_GAME_PVP_RESULT_DATA_NOT>(ev, EventId.EGS_END_GAME_PVP_RESULT_DATA_NOT); break;
            case EventId.ERM_FIELD_UNIT_SYNC_DATA_BROAD_NOT: ForwardRoomPacket<KEGS_FIELD_UNIT_SYNC_DATA_BROAD_NOT>(ev, EventId.EGS_FIELD_UNIT_SYNC_DATA_BROAD_NOT); break;
            case EventId.ERM_FORCE_RELAY_NOT: ForwardRoomPacket<KEGS_FORCE_RELAY_NOT>(ev, EventId.EGS_FORCE_RELAY_NOT); break;
            case EventId.ERM_GAME_LOADING_ACK: ForwardRoomPacket(ev, EventId.EGS_GAME_LOADING_ACK); break;
            case EventId.ERM_GAME_LOADING_ALL_UNIT_OK_NOT: ForwardRoomPacket(ev, EventId.EGS_GAME_LOADING_ALL_UNIT_OK_NOT); break;
            case EventId.ERM_GAME_LOADING_NOT: ForwardRoomPacket<KEGS_GAME_LOADING_NOT>(ev, EventId.EGS_GAME_LOADING_NOT); break;
            case EventId.ERM_GAME_LOADING_TIME_OUT_NOT:
            {
                if (CurrentRoomUid > 0)
                {
                    var room = _server.Center.RoomManager.GetRoom(CurrentRoomUid);
                    if (room?.Info.RoomType == 1 && CurrentPartyUid > 0 && ActiveUnit != null)
                        _server.Center.PartyManager.LeaveParty(CurrentPartyUid, ActiveUnit.UnitUid);
                    CurrentRoomUid = 0;
                }
                Fsm.TryStateTransition(KGSFSM.I_TO_FIELD_MAP, out _);
                SendID(EventId.EGS_GAME_LOADING_TIME_OUT_NOT);
                break;
            }
            case EventId.ERM_GAME_START_PVP_MATCH_NOT:
            {
                var not = ev.GetPayload<KEGS_GAME_START_PVP_MATCH_NOT>(Region);
                if (CurrentRoomUid > 0)
                {
                    var oldRoom = _server.Center.RoomManager.GetRoom(CurrentRoomUid);
                    if (oldRoom?.Info.RoomType == 3 && ActiveUnit != null)
                    {
                        _server.Router.Send(PerformerTypeId, ActiveUnit.UnitUid, PerformerId.PI_CENTER_SERVER, _server.Center.Uid,
                            EventId.ERM_LEAVE_BATTLE_FIELD_FOR_PVP_GAME_START_REQ, new KERM_LEAVE_ROOM_REQ
                            {
                                RoomUid = CurrentRoomUid,
                                UnitUid = ActiveUnit.UnitUid,
                                Reason = NetError.NOT_LEAVE_ROOM_REASON_32,
                            });
                        CurrentRoomUid = 0;
                        if (Fsm.StateId == KGSFSM.S_ROOM)
                            Fsm.TryStateTransition(KGSFSM.I_TO_FIELD_MAP, out _);
                    }
                }
                CurrentRoomUid = not.RoomInfo.RoomUid;
                Fsm.TryStateTransition(KGSFSM.I_TO_ROOM, out _);
                LeaveCurrentField();
                SendPacket(EventId.EGS_GAME_START_PVP_MATCH_NOT, not);
                break;
            }
            case EventId.ERM_GAME_START_TIME_COUNT_NOT: ForwardRoomPacket(ev, EventId.EGS_GAME_START_TIME_COUNT_NOT); break;
            case EventId.ERM_GET_ITEM_ACK: ForwardRoomPacket(ev, EventId.EGS_GET_ITEM_ACK); break;
            case EventId.ERM_GET_ITEM_NOT: ForwardRoomPacket<KEGS_GET_ITEM_NOT>(ev, EventId.EGS_GET_ITEM_NOT); break;
            case EventId.ERM_INTRUDE_START_ACK: ForwardRoomPacketWithOk(ev, EventId.EGS_INTRUDE_START_ACK, static ok => new KEGS_INTRUDE_START_ACK { Ok = ok }); break;
            case EventId.ERM_INTRUDE_START_NOT: ForwardRoomPacket<KEGS_INTRUDE_START_NOT>(ev, EventId.EGS_INTRUDE_START_NOT); break;
            case EventId.ERM_JOIN_BATTLE_FIELD_NOT: ForwardRoomPacket(ev, EventId.EGS_JOIN_BATTLE_FIELD_NOT); break;
            case EventId.ERM_JOIN_FIELD_CHANNEL_CHANGE_ACK:
            {
                int ok = NetError.NET_OK;
                if (ev.InProcessPayload is KERM_JOIN_FIELD_ACK jfIn)
                {
                    ok = jfIn.Ok;
                    if (jfIn.Ok == NetError.NET_OK && jfIn.FieldUid > 0)
                        CurrentFieldUid = jfIn.FieldUid;
                }
                else if (ev.InProcessPayload is KEGS_JOIN_FIELD_CHANNEL_CHANGE_ACK directCh)
                {
                    ok = directCh.Ok;
                }
                else if (ev.TryReadPayload<KERM_JOIN_FIELD_ACK>(Region, out var jfWire))
                {
                    ok = jfWire.Ok;
                    if (jfWire.Ok == NetError.NET_OK && jfWire.FieldUid > 0)
                        CurrentFieldUid = jfWire.FieldUid;
                }
                else if (ev.TryReadPayload<KPacketOK>(Region, out var okPkt))
                {
                    ok = okPkt.Ok;
                }
                SendPacket(EventId.EGS_JOIN_FIELD_CHANNEL_CHANGE_ACK, new KEGS_JOIN_FIELD_CHANNEL_CHANGE_ACK { Ok = ok });
                break;
            }
            case EventId.ERM_JOIN_MY_PSHOP_AGENCY_ACK:
            {
                var ack = ev.TryReadPayload<KEGS_JOIN_MY_PSHOP_AGENCY_ACK>(Region, out var jmp)
                    ? jmp
                    : new KEGS_JOIN_MY_PSHOP_AGENCY_ACK { Ok = ev.TryReadPayload<KPacketOK>(Region, out var okp) ? okp.Ok : NetError.NET_OK };
                if (ack.Ok == NetError.NET_OK)
                {
                    if (PShopAgencyUid > 0)
                        PersonalShopManager.JoinShop(PShopAgencyUid);
                    if (ack.JoinType == KEGS_JOIN_MY_PSHOP_AGENCY_REQ.JtForPickUp)
                    {
                        foreach (var sellItem in ack.SellItemInfo)
                            sellItem.TotalSellEdIn = (int)(sellItem.TotalSellEdIn * 0.9);
                    }
                }
                else
                {
                    PersonalShopManager.LeaveJoinedShop();
                }
                SendPacket(EventId.EGS_JOIN_MY_PSHOP_AGENCY_ACK, ack);
                break;
            }
            case EventId.ERM_JOIN_PERSONAL_SHOP_ACK:
            {
                if (ev.InProcessPayload is KEGS_JOIN_PERSONAL_SHOP_ACK directJoin)
                {
                    SendPacket(EventId.EGS_JOIN_PERSONAL_SHOP_ACK, directJoin);
                }
                else if (ev.InProcessPayload is KERM_JOIN_PERSONAL_SHOP_ACK ermJoin ||
                         (ev.Buffer.Length > 0 && ev.TryReadPayload<KERM_JOIN_PERSONAL_SHOP_ACK>(Region, out ermJoin!)))
                {
                    if (ermJoin.Ok == NetError.NET_OK && ermJoin.PersonalShopUid > 0)
                        PersonalShopManager.JoinShop(ermJoin.PersonalShopUid);
                    else
                        PersonalShopManager.LeaveJoinedShop();
                    SendPacket(EventId.EGS_JOIN_PERSONAL_SHOP_ACK, new KEGS_JOIN_PERSONAL_SHOP_ACK
                    {
                        Ok = ermJoin.Ok,
                        PersonalShopName = ermJoin.PersonalShopName,
                        NickName = ermJoin.NickName,
                        SellItemInfo = ermJoin.SellItemInfo,
                    });
                }
                else
                {
                    ForwardRoomPacketWithOk(ev, EventId.EGS_JOIN_PERSONAL_SHOP_ACK, static ok => new KEGS_JOIN_PERSONAL_SHOP_ACK { Ok = ok });
                }
                break;
            }
            case EventId.ERM_JOIN_ROOM_NOT: ForwardRoomPacket<KEGS_JOIN_ROOM_NOT>(ev, EventId.EGS_JOIN_ROOM_NOT); break;
            case EventId.ERM_JOIN_SQUARE_NOT: ForwardRoomPacket<KEGS_JOIN_SQUARE_NOT>(ev, EventId.EGS_JOIN_SQUARE_NOT); break;
            case EventId.ERM_JOIN_TRADE_NOT:
            {
                if (ev.TryReadPayload<KEGS_JOIN_TRADE_NOT>(Region, out var jt))
                {
                    if (ActiveUnit != null && jt.TradeUserInfo.UnitUid == ActiveUnit.UnitUid)
                        break;
                    SendPacket(EventId.EGS_JOIN_TRADE_NOT, jt);
                }
                else
                {
                    ForwardRoomPacket(ev, EventId.EGS_JOIN_TRADE_NOT);
                }
                break;
            }
            case EventId.ERM_JOIN_WEDDING_HALL_NOT: ForwardRoomPacket(ev, EventId.EGS_JOIN_WEDDING_HALL_NOT); break;
            case EventId.ERM_LEAVE_GAME_NOT: ForwardRoomPacket<KEGS_LEAVE_GAME_NOT>(ev, EventId.EGS_LEAVE_GAME_NOT); break;
            case EventId.ERM_LEAVE_PERSONAL_SHOP_ACK:
            {
                var ack = ev.TryReadPayload<KPacketOK>(Region, out var okp) ? okp : new KPacketOK { Ok = NetError.NET_OK };
                if (ack.Ok == NetError.NET_OK)
                {
                    PersonalShopManager.LeaveJoinedShop();
                    PersonalShopManager.CloseShop();
                }
                SendPacket(EventId.EGS_LEAVE_PERSONAL_SHOP_ACK, ack);
                break;
            }
            case EventId.ERM_LEAVE_PSHOP_AGENCY_ACK:
            {
                var ack = ev.TryReadPayload<KPacketOK>(Region, out var okp) ? okp : new KPacketOK { Ok = NetError.NET_OK };
                if (ack.Ok == NetError.NET_OK)
                    PersonalShopManager.LeaveJoinedShop();
                SendPacket(EventId.EGS_LEAVE_PSHOP_AGENCY_ACK, ack);
                break;
            }
            case EventId.ERM_LEAVE_ROOM_BEFORE_DEFENCE_DUNGEON_START_NOT:
            {
                if (CurrentPartyUid > 0 && ActiveUnit != null)
                    _server.Center.PartyManager.LeaveParty(CurrentPartyUid, ActiveUnit.UnitUid);
                TempInventory.Clear();
                CurrentRoomUid = 0;
                Fsm.TryStateTransition(KGSFSM.I_TO_FIELD_MAP, out _);
                SendID(EventId.EGS_LEAVE_ROOM_BEFORE_DEFENCE_DUNGEON_START_NOT);
                break;
            }
            case EventId.ERM_LEAVE_ROOM_NOT: ForwardRoomPacket(ev, EventId.EGS_LEAVE_ROOM_NOT); break;
            case EventId.ERM_LEAVE_SQUARE_NOT: ForwardRoomPacket<KEGS_LEAVE_SQUARE_NOT>(ev, EventId.EGS_LEAVE_SQUARE_NOT); break;
            case EventId.ERM_LEAVE_WEDDING_HALL_NOT: ForwardRoomPacket(ev, EventId.EGS_LEAVE_WEDDING_HALL_NOT); break;
            case EventId.ERM_MY_USER_UNIT_INFO_TO_SERVER_ACK: ForwardRoomPacket(ev, EventId.EGS_MY_USER_UNIT_INFO_TO_SERVER_ACK); break;
            case EventId.ERM_NPC_UNIT_CREATE_MIDDLE_BOSS_NOT: ForwardRoomPacket(ev, EventId.EGS_NPC_UNIT_CREATE_MIDDLE_BOSS_NOT); break;
            case EventId.ERM_OFFER_TRADE_ACK: ForwardRoomPacket(ev, EventId.EGS_OFFER_TRADE_ACK); break;
            case EventId.ERM_OFFER_TRADE_NOT: ForwardRoomPacket(ev, EventId.EGS_OFFER_TRADE_NOT); break;
            case EventId.ERM_PARTY_BAN_USER_ACK:
            {
                if (CurrentRoomUid > 0)
                {
                    var room = _server.Center.RoomManager.GetRoom(CurrentRoomUid);
                    if (room?.Info.RoomType == 1)
                        TempInventory.Clear();
                    CurrentRoomUid = 0;
                }
                Fsm.TryStateTransition(KGSFSM.I_TO_FIELD_MAP, out _);
                SendID(EventId.EGS_PARTY_BAN_USER_NOT);
                if (CurrentTradeUid > 0 && ActiveUnit != null)
                {
                    _server.Router.Send(PerformerTypeId, ActiveUnit.UnitUid, PerformerId.PI_CENTER_SERVER, _server.Center.Uid,
                        EventId.ERM_BREAK_TRADE_REQ, new KPacketReason { Reason = NetError.NOT_LEAVE_ROOM_REASON_16 });
                }
                break;
            }
            case EventId.ERM_PARTY_TO_FIELD_MAP_ACK: ForwardRoomPacketWithOk(ev, EventId.EGS_PARTY_TO_FIELD_MAP_ACK, static ok => new KEGS_PARTY_TO_FIELD_MAP_ACK { Ok = ok }); break;
            case EventId.ERM_PET_ACTION_NOT: ForwardRoomPacket<KEGS_PET_ACTION_NOT>(ev, EventId.EGS_PET_ACTION_NOT); break;
            case EventId.ERM_PET_EVOLUTION_NOT: ForwardRoomPacket<KEGS_PET_EVOLUTION_NOT>(ev, EventId.EGS_PET_EVOLUTION_NOT); break;
            case EventId.ERM_PLAY_START_NOT:
            {
                var not = ev.GetPayload<KEGS_PLAY_START_NOT>(Region);
                if (CurrentRoomUid > 0)
                {
                    var room = _server.Center.RoomManager.GetRoom(CurrentRoomUid);
                    if (room?.Info.RoomType == 1)
                    {
                        Statistics.IncreaseCount(KUserStatistics.UsTable.EtcData, 0, 1, 1);
                        SendPacket(EventId.EGS_HENIR_REWARD_COUNT_NOT, new KEGS_HENIR_REWARD_COUNT_NOT
                        {
                            UnLimited = DungeonManager.UnlimitedHenirRewardEvent,
                            Normal = DungeonManager.PossibleNormalCount,
                            Premium = DungeonManager.PossiblePremiumCount,
                            EventVal = DungeonManager.PossibleEventCount,
                            PremiumMAX = _server.GameSysValTable.GetAdditionalHenirRewardCount(PcBangManager.PcBangType),
                            EventMAX = DungeonManager.HenirRewardEventLimitCount,
                        });
                    }
                }
                SendPacket(EventId.EGS_PLAY_START_NOT, not);
                break;
            }
            case EventId.ERM_PLAY_TIME_OUT_NOT: ForwardRoomPacket(ev, EventId.EGS_PLAY_TIME_OUT_NOT); break;
            case EventId.ERM_PVP_NPC_UNIT_INFO_TO_SERVER_ACK: ForwardRoomPacket(ev, EventId.EGS_PVP_NPC_UNIT_INFO_TO_SERVER_ACK); break;
            case EventId.ERM_PVP_NPC_UNIT_RE_BIRTH_POS_ACK: ForwardRoomPacket(ev, EventId.EGS_PVP_NPC_UNIT_RE_BIRTH_POS_ACK); break;
            case EventId.ERM_QUICK_JOIN_ACK:
            {
                if (ev.TryReadPayload<KEGS_QUICK_JOIN_ACK>(Region, out var qj))
                {
                    if (qj.Ok == NetError.NET_OK && qj.RoomInfo.RoomUid > 0)
                    {
                        CurrentRoomUid = qj.RoomInfo.RoomUid;
                        Fsm.TryStateTransition(KGSFSM.I_TO_ROOM, out _);
                        LeaveCurrentField();
                    }
                    SendPacket(EventId.EGS_QUICK_JOIN_ACK, qj);
                }
                else
                {
                    ForwardRoomPacketWithOk(ev, EventId.EGS_QUICK_JOIN_ACK, static ok => new KEGS_QUICK_JOIN_ACK { Ok = ok });
                }
                break;
            }
            case EventId.ERM_REFRESH_ROOM_ACK: ForwardRoomPacketWithOk(ev, EventId.EGS_REFRESH_ROOM_ACK, static ok => new KEGS_REFRESH_ROOM_ACK { Ok = ok }); break;
            case EventId.ERM_REG_PERSONAL_SHOP_ITEM_ACK:
            {
                if (ev.InProcessPayload is KEGS_REG_PERSONAL_SHOP_ITEM_ACK regDirect)
                {
                    SendPacket(EventId.EGS_REG_PERSONAL_SHOP_ITEM_ACK, regDirect);
                }
                else if (ev.InProcessPayload is KERM_REG_PERSONAL_SHOP_ITEM_ACK regErm ||
                         (ev.Buffer.Length > 0 && ev.TryReadPayload<KERM_REG_PERSONAL_SHOP_ITEM_ACK>(Region, out regErm!)))
                {
                    if (regErm.Ok == NetError.NET_OK && ActiveUnit != null)
                    {
                        if (regErm.OpeningED > 0)
                        {
                            EdManager.TryAddEd(-regErm.OpeningED, KUserEDManager.EdReason.ErOutEdPersonalShopOpenCommission, out int nextEd);
                            ActiveUnit.Ed = nextEd;
                            _server.Db.SetUnitEd(ActiveUnit.UnitUid, nextEd);
                        }
                        if (CurrentRoomUid > 0)
                        {
                            var room = _server.Center.RoomManager.GetRoom(CurrentRoomUid);
                            if (room?.Info.RoomType == 4)
                            {
                                _server.Router.Send(PerformerTypeId, ActiveUnit.UnitUid, PerformerId.PI_CENTER_SERVER, _server.Center.Uid,
                                    EventId.ERM_UPDATE_SQUARE_PERSONAL_SHOP_INFO_NOT, new KPacketOK { Ok = NetError.NET_OK });
                            }
                        }
                    }
                    SendPacket(EventId.EGS_REG_PERSONAL_SHOP_ITEM_ACK, new KEGS_REG_PERSONAL_SHOP_ITEM_ACK
                    {
                        Ok = regErm.Ok,
                        Ed = ActiveUnit?.Ed ?? 0,
                        PersonalShopName = regErm.PersonalShopName,
                        SellItemInfo = regErm.SellItemInfo,
                        InventorySlotInfo = regErm.InventorySlotInfo,
                    });
                }
                else
                {
                    ForwardRoomPacketWithOk(ev, EventId.EGS_REG_PERSONAL_SHOP_ITEM_ACK, static ok => new KEGS_REG_PERSONAL_SHOP_ITEM_ACK { Ok = ok });
                }
                break;
            }
            case EventId.ERM_REG_PSHOP_AGENCY_ITEM_ACK:
            {
                if (ev.InProcessPayload is KEGS_REG_PSHOP_AGENCY_ITEM_ACK regAgDirect)
                {
                    SendPacket(EventId.EGS_REG_PSHOP_AGENCY_ITEM_ACK, regAgDirect);
                }
                else if (ev.InProcessPayload is KERM_REG_PERSONAL_SHOP_ITEM_ACK regAgErm ||
                         (ev.Buffer.Length > 0 && ev.TryReadPayload<KERM_REG_PERSONAL_SHOP_ITEM_ACK>(Region, out regAgErm!)))
                {
                    if (regAgErm.Ok != NetError.NET_OK)
                    {
                        SendPacket(EventId.EGS_REG_PSHOP_AGENCY_ITEM_ACK, new KEGS_REG_PSHOP_AGENCY_ITEM_ACK
                        {
                            Ok = regAgErm.Ok,
                            SellItemInfo = regAgErm.SellItemInfo,
                            InventorySlotInfo = regAgErm.InventorySlotInfo,
                        });
                        break;
                    }
                    PersonalShopManager.LeaveJoinedShop();
                    if (ActiveUnit != null && regAgErm.AddCompleteItemInfo.Count > 0)
                    {
                        int chargeEd = regAgErm.AddCompleteItemInfo.Count * 1000;
                        if (ActiveUnit.Ed >= chargeEd)
                        {
                            EdManager.TryAddEd(-chargeEd, KUserEDManager.EdReason.ErOutEdPersonalShopOpenCommission, out int nextEd);
                            ActiveUnit.Ed = nextEd;
                            _server.Db.SetUnitEd(ActiveUnit.UnitUid, nextEd);
                        }
                    }
                    SendPacket(EventId.EGS_REG_PSHOP_AGENCY_ITEM_ACK, new KEGS_REG_PSHOP_AGENCY_ITEM_ACK
                    {
                        Ok = NetError.NET_OK,
                        Ed = ActiveUnit?.Ed ?? 0,
                        PersonalShopName = regAgErm.PersonalShopName,
                        SellItemInfo = regAgErm.AddCompleteItemInfo.Count > 0 ? regAgErm.AddCompleteItemInfo : regAgErm.SellItemInfo,
                        InventorySlotInfo = regAgErm.InventorySlotInfo,
                    });
                }
                else
                {
                    ForwardRoomPacket(ev, EventId.EGS_REG_PSHOP_AGENCY_ITEM_ACK);
                }
                break;
            }
            case EventId.ERM_RELATIONSHIP_EFFECT_NOT: ForwardRoomPacket<KEGS_RELATIONSHIP_EFFECT_NOT>(ev, EventId.EGS_RELATIONSHIP_EFFECT_NOT); break;
            case EventId.ERM_REMAINING_PLAY_TIME_NOT: ForwardRoomPacket(ev, EventId.EGS_REMAINING_PLAY_TIME_NOT); break;
            case EventId.ERM_REPAIR_ITEM_NOT: ForwardRoomPacket(ev, EventId.EGS_REPAIR_ITEM_NOT); break;
            case EventId.ERM_RESTORE_SPIRIT_NOT: ForwardRoomPacket(ev, EventId.EGS_RESTORE_SPIRIT_NOT); break;
            case EventId.ERM_RESULT_ALL_UNIT_SUCCESS_NOT:
            {
                if (ev.TryReadPayload<KEGS_RESULT_ALL_UNIT_SUCCESS_NOT>(Region, out var resAll))
                {
                    int curMap = CurrentFieldMapId > 0 ? CurrentFieldMapId : (ActiveUnit?.LastPos.MapId ?? 0);
                    if (curMap >= 50000)
                    {
                        if (ActiveUnit != null)
                        {
                            UnitManager.ReserveReturnToFieldInfo(new KUserUnitManager.KReturnToFieldInfo(
                                ActiveUnit.LastPos.MapId,
                                ActiveUnit.LastPos.LastTouchLineIndex,
                                0f, 0f, 0f));
                        }
                    }
                    else if (ActiveUnit != null && CurrentRoomUid > 0)
                    {
                        _server.Router.Send(PerformerTypeId, ActiveUnit.UnitUid, PerformerId.PI_CENTER_SERVER, _server.Center.Uid,
                            EventId.ERM_NEXT_RETURN_TO_FIELD_NOT, new KERM_COMPLETE_RETURN_TO_FIELD_NOT
                            {
                                UnitUid = ActiveUnit.UnitUid,
                                FieldMapId = ActiveUnit.LastPos.MapId,
                            });
                    }
                    SendPacket(EventId.EGS_RESULT_ALL_UNIT_SUCCESS_NOT, resAll);
                }
                else
                {
                    ForwardRoomPacket(ev, EventId.EGS_RESULT_ALL_UNIT_SUCCESS_NOT);
                }
                break;
            }
            case EventId.ERM_RESULT_SUCCESS_ACK:
            {
                var ack = ev.TryReadPayload<KEGS_RESULT_SUCCESS_ACK>(Region, out var rsa)
                    ? rsa
                    : new KEGS_RESULT_SUCCESS_ACK { Ok = ev.TryReadPayload<KPacketOK>(Region, out var okp) ? okp.Ok : NetError.NET_OK };
                if (CurrentRoomUid > 0 && ack.Ok == NetError.NET_OK)
                {
                    var room = _server.Center.RoomManager.GetRoom(CurrentRoomUid);
                    if (room?.Info.RoomType == 1 && !SpiritManager.CheckingSpirit(ack.DungeonId > 0 ? ack.DungeonId : room.Info.DungeonId,
                        isPcBang: PcBangManager.IsPcBang, isHenirDungeon: room.Info.DungeonMode == 1))
                    {
                        ack.Ok = NetError.NOT_LEAVE_ROOM_REASON_14;
                    }
                }
                SendPacket(EventId.EGS_RESULT_SUCCESS_ACK, ack);
                break;
            }
            case EventId.ERM_SEARCH_UNIT_ACK:
            {
                if (ev.InProcessPayload is KEGS_SEARCH_UNIT_ACK suDirect)
                {
                    SendPacket(EventId.EGS_SEARCH_UNIT_ACK, suDirect);
                }
                else if (ev.InProcessPayload is KELG_SEARCH_UNIT_ACK su ||
                         (ev.Buffer.Length > 0 && ev.TryReadPayload<KELG_SEARCH_UNIT_ACK>(Region, out su!)))
                {
                    if (su.EventIdValue == (ushort)EventId.EGS_GET_CONNECTION_UNIT_INFO_REQ)
                    {
                        SendPacket(EventId.EGS_GET_CONNECTION_UNIT_INFO_ACK, new KEGS_GET_CONNECTION_UNIT_INFO_ACK
                        {
                            Ok = su.Ok,
                            CUnitInfo = su.Ok == NetError.NET_OK ? su.CUnitInfo : new KConnectionUnitInfo(),
                        });
                    }
                    else if (su.EventIdValue == (ushort)EventId.EGS_WATCH_UNIT_REQ)
                    {
                        SendPacket(EventId.EGS_WATCH_UNIT_ACK, new KEGS_WATCH_UNIT_ACK
                        {
                            Ok = su.Ok,
                            CUnitInfo = su.Ok == NetError.NET_OK ? su.CUnitInfo : new KConnectionUnitInfo(),
                        });
                    }
                    else
                    {
                        SendPacket(EventId.EGS_SEARCH_UNIT_ACK, new KEGS_SEARCH_UNIT_ACK { Ok = su.Ok });
                    }
                }
                else
                {
                    ForwardRoomPacketWithOk(ev, EventId.EGS_SEARCH_UNIT_ACK, static ok => new KEGS_SEARCH_UNIT_ACK { Ok = ok });
                }
                break;
            }
            case EventId.ERM_SERVER_CHECK_HACK_USER_NOT:
            {
                if (AuthLevel >= 2) break;
                var hackNot = ev.TryReadPayload<KEGS_SERVER_CHECK_HACK_USER_NOT>(Region, out var hPkt)
                    ? hPkt
                    : new KEGS_SERVER_CHECK_HACK_USER_NOT();
                if (hackNot.HackingCheckType is not (3 or 5)) // HCT_DUNGEON_CANT_KILL_NPC_FIND / HCT_DUNGEON_PLAY_START
                {
                    HackingUserSendType = hackNot.HackingCheckType;
                    HackingManager.SetHackingUserSendType(HackingUserSendType);
                    if (AccountUid > 0)
                        _server.Db.RecordHackingProcessLog(AccountUid, ActiveUnit?.UnitUid ?? 0, (byte)hackNot.HackingCheckType, "ERM_SERVER_CHECK_HACK_USER_NOT");
                    SendPacket(EventId.EGS_SERVER_CHECK_HACK_USER_NOT, hackNot);
                }
                break;
            }
            case EventId.ERM_SET_TC_REMAINING_TIME_ACK: ForwardRoomPacketWithOk(ev, EventId.EGS_SET_TC_REMAINING_TIME_ACK, static ok => new KEGS_SET_TC_REMAINING_TIME_ACK { Ok = ok }); break;
            case EventId.ERM_START_WEDDING_AGREE_NOT: ForwardRoomPacket(ev, EventId.EGS_START_WEDDING_AGREE_NOT); break;
            case EventId.ERM_START_WEDDING_NOT:
            {
                if (ev.InProcessPayload is KEGS_START_WEDDING_NOT wedDirect)
                {
                    SendPacket(EventId.EGS_START_WEDDING_NOT, wedDirect);
                    break;
                }
                if (ev.InProcessPayload is KERM_START_WEDDING_NOT wedErm ||
                    (ev.Buffer.Length > 0 && ev.TryReadPayload<KERM_START_WEDDING_NOT>(Region, out wedErm!)))
                {
                    var updatedInv = new List<KInventoryItemInfo>();
                    if (wedErm.Ok == NetError.NET_OK && ActiveUnit != null)
                    {
                        if (wedErm.Groom == ActiveUnit.UnitUid || wedErm.Bride == ActiveUnit.UnitUid)
                        {
                            var nowUtc = DateTime.UtcNow;
                            RelationshipManager.SetRelationshipType(KUserRelationshipManager.RelationshipType.RtMarried);
                            RelationshipManager.AnniversaryDayUtc = nowUtc;
                            RelationshipManager.LastRewardDateUtc = nowUtc;
                            RelationshipManager.RewardTitleType = 0;
                            _server.Db.SaveRelationship(
                                ActiveUnit.UnitUid,
                                (sbyte)RelationshipManager.CurrentType,
                                RelationshipManager.LoverUnitUid,
                                RelationshipManager.LoverNickName,
                                RelationshipManager.LoveWord,
                                nowUtc,
                                nowUtc,
                                0);
                        }
                        foreach (var kv in wedErm.RewardItem)
                        {
                            if (kv.Key > 0 && kv.Value > 0)
                            {
                                _server.Db.SendLetter(Region, ActiveUnit.UnitUid, ActiveUnit.NickName, ActiveUnit.UnitUid,
                                    "Wedding Reward", "Wedding reward item", kv.Key, kv.Value, 0, KPostItemInfo.LtEvent);
                            }
                        }
                        if (wedErm.Groom == ActiveUnit.UnitUid)
                        {
                            _server.BroadcastNotice(KEGS_NOTIFY_MSG_NOT.NT_WEDDING_SUCCESS, 1,
                                $"{ActiveUnit.NickName} & {RelationshipManager.LoverNickName}");
                        }
                    }
                    SendPacket(EventId.EGS_START_WEDDING_NOT, new KEGS_START_WEDDING_NOT
                    {
                        Ok = wedErm.Ok,
                        UpdatedInventorySlot = updatedInv,
                    });
                    break;
                }
                ForwardRoomPacketWithOk(ev, EventId.EGS_START_WEDDING_NOT, static ok => new KEGS_START_WEDDING_NOT { Ok = ok });
                break;
            }
            case EventId.ERM_STATE_CHANGE_GAME_INTRUDE_ACK: ForwardRoomPacketWithOk(ev, EventId.EGS_STATE_CHANGE_GAME_INTRUDE_ACK, static ok => new KEGS_STATE_CHANGE_GAME_INTRUDE_ACK { Ok = ok }); break;
            case EventId.ERM_STATE_CHANGE_GAME_INTRUDE_NOT: ForwardRoomPacket<KEGS_STATE_CHANGE_GAME_INTRUDE_NOT>(ev, EventId.EGS_STATE_CHANGE_GAME_INTRUDE_NOT); break;
            case EventId.ERM_STOP_SALE_PSHOP_AGENCY_ACK:
            {
                PShopAgencyUid = 0;
                _pshopAgencyRemoteOpened = false;
                PersonalShopManager.CloseShop();
                int stopOk = ev.TryReadPayload<KPacketOK>(Region, out var okPkt) ? okPkt.Ok : NetError.NET_OK;
                SendPacket(EventId.EGS_STOP_SALE_PSHOP_AGENCY_ACK, new KPacketOK { Ok = stopOk });
                break;
            }
            case EventId.ERM_SUMMON_PET_NOT: ForwardRoomPacket<KEGS_SUMMON_PET_NOT>(ev, EventId.EGS_SUMMON_PET_NOT); break;
            case EventId.ERM_SUMMON_RIDING_PET_NOT: ForwardRoomPacket<KEGS_SUMMON_RIDING_PET_NOT>(ev, EventId.EGS_SUMMON_RIDING_PET_NOT); break;
            case EventId.ERM_SYNC_BATTLE_FIELD_DAMAGED_MAP_NOT: ForwardRoomPacket<KEGS_SYNC_BATTLE_FIELD_DAMAGED_MAP_NOT>(ev, EventId.EGS_SYNC_BATTLE_FIELD_DAMAGED_MAP_NOT); break;
            case EventId.ERM_SYNC_DUNGEON_TIMER_NOT: ForwardRoomPacket<KEGS_SYNC_DUNGEON_TIMER_NOT>(ev, EventId.EGS_SYNC_DUNGEON_TIMER_NOT); break;
            case EventId.ERM_UDP_KICK_STATE_CHANGE_FIELD_NOT:
            {
                var room = CurrentRoomUid > 0 ? _server.Center.RoomManager.GetRoom(CurrentRoomUid) : null;
                if (room?.Info.RoomType == 3 && ActiveUnit != null)
                {
                    _server.Router.Send(PerformerTypeId, ActiveUnit.UnitUid, PerformerId.PI_CENTER_SERVER, _server.Center.Uid,
                        EventId.ERM_LEAVE_ROOM_FOR_WORKINGS_BLOCK_REQ, new KERM_LEAVE_ROOM_REQ
                        {
                            RoomUid = CurrentRoomUid,
                            UnitUid = ActiveUnit.UnitUid,
                        });
                }
                else
                {
                    long accUnitUid = ev.TryReadPayload<KEGS_UDP_KICK_STATE_CHANGE_FIELD_NOT>(Region, out var udpNot)
                        ? udpNot.AccusationUnitUid
                        : (ev.TryReadPayload<KPacketLong>(Region, out var pl) ? pl.Value : 0);
                    SendPacket(EventId.EGS_UDP_KICK_STATE_CHANGE_FIELD_NOT, new KEGS_UDP_KICK_STATE_CHANGE_FIELD_NOT
                    {
                        AccusationUnitUid = accUnitUid,
                    });
                }
                break;
            }
            case EventId.ERM_UNSUMMON_RIDING_PET_NOT: ForwardRoomPacket<KEGS_UNSUMMON_RIDING_PET_NOT>(ev, EventId.EGS_UNSUMMON_RIDING_PET_NOT); break;
            case EventId.ERM_UPDATE_BUFF_INFO_IN_ROOM_NOT: ForwardRoomPacket(ev, EventId.EGS_UPDATE_BUFF_INFO_IN_ROOM_NOT); break;
            case EventId.ERM_UPDATE_BUFF_INFO_NOT:
            {
                if (ev.InProcessPayload is KEGS_UPDATE_BUFF_INFO_NOT || ev.Buffer.Length > 1)
                {
                    ForwardRoomPacket<KEGS_UPDATE_BUFF_INFO_NOT>(ev, EventId.EGS_UPDATE_BUFF_INFO_NOT);
                }
                else
                {
                    SendPacket(EventId.EGS_UPDATE_BUFF_INFO_NOT, new KEGS_UPDATE_BUFF_INFO_NOT
                    {
                        ActivateBuffList = ServerBuffManager.ToPacketBuffInfos(),
                    });
                }
                break;
            }
            case EventId.ERM_UPDATE_NPC_UNIT_BUFF_INFO_BROAD_NOT: ForwardRoomPacket(ev, EventId.EGS_UPDATE_NPC_UNIT_BUFF_INFO_BROAD_NOT); break;
            case EventId.ERM_UPDATE_PERSONAL_SHOP_ITEM_INFO_NOT: ForwardRoomPacket(ev, EventId.EGS_UPDATE_PERSONAL_SHOP_ITEM_INFO_NOT); break;
            case EventId.ERM_UPDATE_SQUARE_PERSONAL_SHOP_INFO_NOT: ForwardRoomPacket(ev, EventId.EGS_UPDATE_SQUARE_PERSONAL_SHOP_INFO_NOT); break;
            case EventId.ERM_UPDATE_TRADE_ED_ACK: ForwardRoomPacket(ev, EventId.EGS_UPDATE_TRADE_ED_ACK); break;
            case EventId.ERM_UPDATE_TRADE_ED_NOT: ForwardRoomPacket(ev, EventId.EGS_UPDATE_TRADE_ED_NOT); break;
            case EventId.ERM_UPDATE_TRADE_ITEM_ACK: ForwardRoomPacket(ev, EventId.EGS_UPDATE_TRADE_ITEM_ACK); break;
            case EventId.ERM_UPDATE_TRADE_ITEM_NOT: ForwardRoomPacket(ev, EventId.EGS_UPDATE_TRADE_ITEM_NOT); break;
            case EventId.ERM_UPDATE_USER_UNIT_BUFF_INFO_BROAD_NOT: ForwardRoomPacket(ev, EventId.EGS_UPDATE_USER_UNIT_BUFF_INFO_BROAD_NOT); break;
            case EventId.ERM_USER_UNIT_DIE_COMPLETE_ACK: ForwardRoomPacket(ev, EventId.EGS_USER_UNIT_DIE_COMPLETE_ACK); break;
            case EventId.ERM_USER_UNIT_DIE_COMPLETE_NOT: ForwardRoomPacket<KEGS_USER_UNIT_DIE_COMPLETE_NOT>(ev, EventId.EGS_USER_UNIT_DIE_COMPLETE_NOT); break;
            case EventId.ERM_USER_UNIT_RE_BIRTH_NOT: ForwardRoomPacket(ev, EventId.EGS_USER_UNIT_RE_BIRTH_NOT); break;
            case EventId.ERM_USER_UNIT_RE_BIRTH_POS_ACK: ForwardRoomPacket(ev, EventId.EGS_USER_UNIT_RE_BIRTH_POS_ACK); break;
            case EventId.ERM_USE_QUICK_SLOT_NOT:
            {
                var qnot = ev.GetPayload<KEGS_USE_QUICK_SLOT_NOT>(Region);
                if (qnot.UseItemNeedPayment && ActiveUnit != null && qnot.UnitUid == ActiveUnit.UnitUid)
                    _useItemNeedPayment = true;
                SendPacket(EventId.EGS_USE_QUICK_SLOT_NOT, qnot);
                break;
            }
            case EventId.ERM_USE_SKILL_BUFF_IN_PLAY_BROAD_NOT: ForwardRoomPacket(ev, EventId.EGS_USE_SKILL_BUFF_IN_PLAY_BROAD_NOT); break;
            case EventId.ERM_ZERO_ENDURANCE_ITEM_IN_ROOM_NOT:
            {
                if (ev.InProcessPayload is KEGS_ZERO_ENDURANCE_ITEM_IN_ROOM_NOT zeDirect)
                {
                    SendPacket(EventId.EGS_ZERO_ENDURANCE_ITEM_IN_ROOM_NOT, zeDirect);
                }
                else if (ev.InProcessPayload is KERM_ZERO_ENDURANCE_ITEM_IN_ROOM_NOT zeErm ||
                         (ev.Buffer.Length > 0 && ev.TryReadPayload<KERM_ZERO_ENDURANCE_ITEM_IN_ROOM_NOT>(Region, out zeErm!)))
                {
                    SendPacket(EventId.EGS_ZERO_ENDURANCE_ITEM_IN_ROOM_NOT, new KEGS_ZERO_ENDURANCE_ITEM_IN_ROOM_NOT
                    {
                        UnitUid = zeErm.UnitUid,
                        InventorySlotInfo = zeErm.InventorySlotInfo,
                        GameStat = zeErm.RoomUserInfo.GameStat,
                    });
                }
                else
                {
                    ForwardRoomPacket(ev, EventId.EGS_ZERO_ENDURANCE_ITEM_IN_ROOM_NOT);
                }
                break;
            }


            // ---- Full C++ Internal ERM_* Handlers (Trade / Shop / Agency / Buff / Endurance / Pet / Stats / Field / Room) ----
            case EventId.ERM_ACCEPT_JOIN_TRADE_NOT:
            {
                if (ActiveUnit != null && CurrentTradeUid == 0)
                {
                    long tradeUid = ev.TryReadPayload<KPacketLong>(Region, out var pl) ? pl.Value : ev.DestPerformer.GetFirstUID();
                    if (tradeUid > 0)
                        _server.Router.Send(PerformerTypeId, ActiveUnit.UnitUid, PerformerId.PI_CENTER_SERVER, _server.Center.Uid,
                            EventId.ERM_JOIN_TRADE_REQ, new KERM_JOIN_TRADE_REQ
                            {
                                TradeUid = tradeUid,
                                UnitUid = ActiveUnit.UnitUid,
                            });
                }
                break;
            }

            case EventId.ERM_ACTIVATE_BUFF_NOT:
                if (ev.TryReadPayload<KPacketLong>(Region, out var ab) && ab.Value > 0)
                    ServerBuffManager.ActivateBuff(new KUserServerBuffManager.ServerBuffInfo { BuffId = (int)ab.Value });
                break;

            case EventId.ERM_BAD_ATTITUDE_USER_LOG_NOT:
            {
                long unitUid = ActiveUnit?.UnitUid ?? 0;
                _server.Db.InsertGameLog(AccountUid, unitUid, "BAD_ATTITUDE_EXIT", (int)(CurrentRoomUid & 0x7FFFFFFF), 0, "Bad attitude report");
                KLog.Cwarn($"KGSUser[{Uid}] bad attitude report logged for Unit={unitUid}");
                break;
            }

            case EventId.ERM_BATTLE_FIELD_DANGEROUS_VALUE_ACK:
                SendPacket(EventId.EGS_BATTLE_FIELD_DANGEROUS_VALUE_NOT, new KEGS_BATTLE_FIELD_DANGEROUS_VALUE_NOT
                {
                    DangerousValue = ev.GetPayload<KPacketOK>(Region).Ok,
                });
                break;

            case EventId.ERM_BATTLE_FIELD_GAME_LOADING_ACK:
                SendPacket(EventId.EGS_GAME_LOADING_ACK, ev.InProcessPayload is IKSerializable bgl ? bgl : new KPacketOK { Ok = NetError.NET_OK });
                break;

            case EventId.ERM_BATTLE_FIELD_NPC_UNIT_DIE_NOT:
                SendPacket(EventId.EGS_NPC_UNIT_DIE_NOT, ev.TryReadPayload<KEGS_NPC_UNIT_DIE_NOT>(Region, out var nud)
                    ? nud
                    : new KEGS_NPC_UNIT_DIE_NOT());
                break;

            case EventId.ERM_BATTLE_FIELD_ZU_ECHO_REQ:
                if (ActiveUnit != null && CurrentRoomUid > 0)
                    _server.Router.Send(PerformerTypeId, ActiveUnit.UnitUid, PerformerId.PI_CENTER_SERVER, _server.Center.Uid,
                        EventId.ERM_BATTLE_FIELD_ZU_ECHO_ACK, new KPacketOK { Ok = NetError.NET_OK });
                break;

            case EventId.ERM_BREAK_PSHOP_AGENCY_NOT:
                PShopAgencyUid = 0;
                _pshopAgencyRemoteOpened = false;
                PersonalShopManager.CloseShop();
                break;

            case EventId.ERM_BUY_PSHOP_AGENCY_ITEM_COMPLETE_NOT:
                if (ev.InProcessPayload is IKSerializable bpa)
                    SendPacket(EventId.EGS_BUY_PERSONAL_SHOP_ITEM_COMPLETE_NOT, bpa);
                break;

            case EventId.ERM_CALL_MY_LOVER_CHECK_ROOM_ACK:
                SendPacket(EventId.EGS_CALL_MY_LOVER_ACK, ev.InProcessPayload is IKSerializable cml
                    ? cml
                    : new KPacketOK { Ok = NetError.NET_OK });
                break;

            case EventId.ERM_CALL_MY_LOVER_JOIN_BATTLE_FIELD_ACK:
            {
                var cmlJoin = ev.GetPayload<KERM_JOIN_ROOM_ACK>(Region);
                if (cmlJoin.Ok == NetError.NET_OK && cmlJoin.RoomInfo.RoomUid > 0)
                {
                    CurrentRoomUid = cmlJoin.RoomInfo.RoomUid;
                    Fsm.TryStateTransition(KGSFSM.I_TO_ROOM, out _);
                }
                SendPacket(EventId.EGS_JOIN_BATTLE_FIELD_ACK, new KEGS_JOIN_BATTLE_FIELD_ACK
                {
                    Ok = cmlJoin.Ok,
                    RoomInfo = cmlJoin.RoomInfo,
                    Slots = cmlJoin.Slots,
                    UdpRelayIp = _server.PublicIp,
                    UdpRelayPort = _server.UdpRelayPort,
                });
                break;
            }

            case EventId.ERM_CALL_MY_LOVER_LEAVE_FIELD_ACK:
                CurrentFieldUid = 0;
                break;

            case EventId.ERM_CHANGE_DEFENCE_DUNGEON_PHASE_NOT:
            {
                if (ev.TryReadPayload<KERM_CHANGE_DEFENCE_DUNGEON_PHASE_NOT>(Region, out var ddpReq))
                {
                    var slots = SetupDefenceDungeonQuickSlotItems(ddpReq.DefenceDungeonPhase);
                    SendPacket(EventId.EGS_UPDATE_INVENTORY_SLOT_INFO_NOT,
                        new KEGS_UPDATE_INVENTORY_SLOT_INFO_NOT { InventorySlotInfo = slots });
                }
                else
                {
                    SendPacket(EventId.EGS_UPDATE_INVENTORY_SLOT_INFO_NOT, ev.InProcessPayload is IKSerializable ddp
                        ? ddp
                        : new KEGS_UPDATE_INVENTORY_SLOT_INFO_NOT());
                }
                break;
            }

            case EventId.ERM_CHANGE_ROOM_USER_INFO_NOT:
            case EventId.ERM_CHANGE_SQUARE_USER_INFO_NOT:
                SendPacket(EventId.EGS_CHANGE_USER_INFO_NOT, ev.TryReadPayload<KEGS_CHANGE_USER_INFO_NOT>(Region, out var cui)
                    ? cui
                    : new KEGS_CHANGE_USER_INFO_NOT { UnitUid = ActiveUnit?.UnitUid ?? 0 });
                break;

            case EventId.ERM_CHECK_FOR_REG_PSHOP_AGENCY_ITEM_ACK:
                if (ev.TryReadPayload<KPacketOK>(Region, out var rpaOk) && rpaOk.Ok != NetError.NET_OK)
                    SendPacket(EventId.EGS_REG_PSHOP_AGENCY_ITEM_ACK, new KEGS_REG_PSHOP_AGENCY_ITEM_ACK
                    {
                        Ok = rpaOk.Ok,
                        Ed = ActiveUnit?.Ed ?? 0,
                    });
                break;

            case EventId.ERM_CHECK_INVALID_ZOMBIE_USER_NOT:
                ReserveDestroy();
                break;

            case EventId.ERM_CHECK_JOIN_PERSONAL_SHOP_ACK:
                if (ev.TryReadPayload<KPacketOK>(Region, out var jpsOk) && jpsOk.Ok != NetError.NET_OK)
                    SendPacket(EventId.EGS_JOIN_PERSONAL_SHOP_ACK, jpsOk);
                break;

            case EventId.ERM_CHECK_MY_PSHOP_AGENCY_INFO_ACK:
                if (ev.TryReadPayload<KPacketLong>(Region, out var psa) && psa.Value > 0)
                    PersonalShopManager.JoinShop(psa.Value);
                break;

            case EventId.ERM_CHECK_OPEN_PERSONAL_SHOP_ACK:
                if (ev.TryReadPayload<KPacketOK>(Region, out var opsOk) && opsOk.Ok != NetError.NET_OK)
                    SendPacket(EventId.EGS_CREATE_PERSONAL_SHOP_ACK, new KEGS_CREATE_PERSONAL_SHOP_ACK { Ok = opsOk.Ok });
                break;

            case EventId.ERM_CHECK_OPEN_PSHOP_AGENCY_ACK:
                SendPacket(EventId.EGS_CREATE_PERSONAL_SHOP_ACK, new KEGS_CREATE_PERSONAL_SHOP_ACK
                {
                    Ok = ev.GetPayload<KPacketOK>(Region).Ok,
                });
                break;

            case EventId.ERM_CHECK_SELL_ED_ITEM_ACK:
                if (ActiveUnit != null && ev.TryReadPayload<KERM_CHECK_SELL_ED_ITEM_ACK>(Region, out var sellAck))
                {
                    if (sellAck.Ok == NetError.NET_OK && sellAck.ItemUid > 0)
                    {
                        var (ok, newEd, sold) = _server.Db.SellEdItem(ActiveUnit.UnitUid, sellAck.ItemUid, Math.Max(1, sellAck.Quantity));
                        if (ok) ActiveUnit.Ed = newEd;
                        SendPacket(EventId.EGS_SELL_ED_ITEM_ACK, new KEGS_SELL_ED_ITEM_ACK
                        {
                            Ok = ok ? NetError.NET_OK : NetError.ERR_SELL_ED_ITEM_05,
                            Ed = ActiveUnit.Ed,
                            InventorySlotInfo = sold,
                        });
                    }
                    else
                    {
                        SendPacket(EventId.EGS_SELL_ED_ITEM_ACK, new KEGS_SELL_ED_ITEM_ACK
                        {
                            Ok = sellAck.Ok != NetError.NET_OK ? sellAck.Ok : NetError.ERR_SELL_ED_ITEM_05,
                            Ed = ActiveUnit.Ed,
                        });
                    }
                }
                else
                {
                    SendPacket(EventId.EGS_SELL_ED_ITEM_ACK, new KEGS_SELL_ED_ITEM_ACK
                    {
                        Ok = NetError.ERR_SELL_ED_ITEM_05,
                        Ed = ActiveUnit?.Ed ?? 0,
                    });
                }
                break;

            case EventId.ERM_CHECK_START_WEDDING_ACK:
                SendPacket(EventId.EGS_START_WEDDING_ACK, ev.InProcessPayload is IKSerializable swa
                    ? swa
                    : new KPacketOK { Ok = NetError.NET_OK });
                break;

            case EventId.ERM_CHECK_START_WEDDING_NOT:
                SendPacket(EventId.EGS_START_WEDDING_ASK_REQ, new KPacketOK { Ok = NetError.NET_OK });
                break;

            case EventId.ERM_CONFIRM_BUY_PERSONAL_SHOP_ITEM_REQ:
            {
                if (ActiveUnit != null)
                {
                    var req = ev.GetPayload<KERM_CONFIRM_BUY_PERSONAL_SHOP_ITEM_REQ>(Region);
                    _server.Router.Send(PerformerTypeId, ActiveUnit.UnitUid, PerformerId.PI_CENTER_SERVER, _server.Center.Uid,
                        EventId.ERM_CONFIRM_BUY_PERSONAL_SHOP_ITEM_ACK, new KERM_CONFIRM_BUY_PERSONAL_SHOP_ITEM_ACK
                        {
                            Ok = NetError.NET_OK,
                            BuyUnitUid = req.BuyUnitUid > 0 ? req.BuyUnitUid : ActiveUnit.UnitUid,
                            BuyItemUid = req.SellItemInfo.ItemUid,
                            BuyQuantity = Math.Max(1, req.BuyQuantity),
                        });
                }
                break;
            }

            case EventId.ERM_CONFIRM_TRADE_NOT:
                if (ActiveUnit != null && CurrentTradeUid > 0)
                    _server.Router.Send(PerformerTypeId, ActiveUnit.UnitUid, PerformerId.PI_CENTER_SERVER, _server.Center.Uid,
                        EventId.ERM_CONFIRM_TRADE_REQ, new KERM_CONFIRM_TRADE_REQ());
                break;

            case EventId.ERM_CONFIRM_TRADE_REQ:
                if (ActiveUnit != null)
                    _server.Router.Send(PerformerTypeId, ActiveUnit.UnitUid, PerformerId.PI_CENTER_SERVER, _server.Center.Uid,
                        EventId.ERM_CONFIRM_TRADE_ACK, new KERM_CONFIRM_TRADE_ACK { Ok = NetError.NET_OK });
                break;

            case EventId.ERM_COUPLE_PROPOSE_RESULT_FAIL_NOT:
            {
                int failOk = ev.GetPayload<KPacketOK>(Region).Ok;
                SendPacket(EventId.EGS_COUPLE_PROPOSE_RESULT_NOT, new KEGS_COUPLE_PROPOSE_RESULT_NOT
                {
                    Ok = failOk != 0 ? failOk : NetError.ERR_RELATIONSHIP_00,
                    Result = 0,
                });
                break;
            }

            case EventId.ERM_DB_UPDATE_TRADE_COMPLETE_NOT:
                CurrentTradeUid = 0;
                SendPacket(EventId.EGS_TRADE_COMPLETE_NOT, ev.InProcessPayload is IKSerializable tcn
                    ? tcn
                    : new KPacketOK { Ok = NetError.NET_OK });
                break;

            case EventId.ERM_DEACTIVATE_BUFF_NOT:
                if (ev.TryReadPayload<KPacketLong>(Region, out var db) && db.Value > 0)
                    ServerBuffManager.DeactivateBuff((int)db.Value);
                break;

            case EventId.ERM_DECREASE_ENDURANCE_DUNGEON_NOT:
            {
                if (ActiveUnit == null) break;
                var req = ev.GetPayload<KERM_DECREASE_ENDURANCE_DUNGEON_NOT>(Region);
                var updated = _server.Db.DecreaseEquippedItemEndurance(
                    ActiveUnit.UnitUid, req.EnduranceDamage, expDropNpcNum: 0, partyNumMember: Math.Max(1, (int)req.PartyNumMember));
                if (updated.Count > 0)
                    SendPacket(EventId.EGS_DECREASE_ENDURANCE_NOT, new KEGS_DECREASE_ENDURANCE_NOT { EnduranceUpdate = updated });
                break;
            }

            case EventId.ERM_DECREASE_ENDURANCE_PVP_NOT:
            {
                if (ActiveUnit == null) break;
                var updated = _server.Db.DecreaseEquippedItemEndurance(ActiveUnit.UnitUid, enduranceDamage: 1);
                if (updated.Count > 0)
                    SendPacket(EventId.EGS_DECREASE_ENDURANCE_NOT, new KEGS_DECREASE_ENDURANCE_NOT { EnduranceUpdate = updated });
                break;
            }

            case EventId.ERM_DECREASE_PET_SATIETY_NOT:
                SendPacket(EventId.EGS_CHANGED_PET_SATIETY_INTIMACY_NOT, new KEGS_CHANGED_PET_SATIETY_INTIMACY_NOT
                {
                    UnitUid = ActiveUnit?.UnitUid ?? 0,
                });
                break;

            case EventId.ERM_DUNGEON_USER_STATISTICS_NOT:
                if (ev.TryReadPayload<KPacketOK>(Region, out var dstat))
                    Statistics.AddPlayTime(KUserStatistics.PlayTimeType.DungeonPlayTime, Math.Max(0, dstat.Ok));
                break;

            case EventId.ERM_FIELD_USER_LIST_ACK:
                if (ev.InProcessPayload is IKSerializable ful)
                    SendPacket(EventId.EGS_COMMUNITY_USER_LIST_ACK, ful);
                break;

            case EventId.ERM_FIELD_USER_STATISTICS_NOT:
                if (ev.TryReadPayload<KPacketOK>(Region, out var fstat))
                    Statistics.AddPlayTime(KUserStatistics.PlayTimeType.FieldPlayTime, Math.Max(0, fstat.Ok));
                break;

            case EventId.ERM_GAME_START_ACK:
                SendPacket(EventId.EGS_STATE_CHANGE_GAME_START_ACK, new KPacketOK
                {
                    Ok = ev.GetPayload<KPacketOK>(Region).Ok,
                });
                break;

            case EventId.ERM_GAME_START_NOT:
                SendPacket(EventId.EGS_STATE_CHANGE_GAME_START_NOT, ev.TryReadPayload<KEGS_STATE_CHANGE_GAME_START_NOT>(Region, out var gsn)
                    ? gsn
                    : new KEGS_STATE_CHANGE_GAME_START_NOT { RoomState = 2 });
                break;

            case EventId.ERM_GET_ITEM_INSERT_TO_INVENTORY_NOT:
                if (ActiveUnit != null && ev.TryReadPayload<KPacketLong>(Region, out var gi) && gi.Value > 0)
                    _server.Db.GrantUnitItem(ActiveUnit.UnitUid, (int)gi.Value, 1);
                break;

            case EventId.ERM_JOIN_FIELD_ACK:
                if (ev.TryReadPayload<KPacketLong>(Region, out var jf) && jf.Value > 0)
                {
                    CurrentFieldUid = jf.Value;
                    if (Fsm.CurrentStateId != KGSFSM.S_FIELD_MAP)
                        Fsm.TryStateTransition(KGSFSM.I_TO_FIELD_MAP, out _);
                }
                break;

            case EventId.ERM_JOIN_TRADE_ACK:
            {
                if (ev.TryReadPayload<KPacketLong>(Region, out var jtAck) && jtAck.Value > 0)
                {
                    CurrentTradeUid = jtAck.Value;
                    if (Fsm.CurrentStateId == KGSFSM.S_ROOM && ActiveUnit != null)
                        _server.Router.Send(PerformerTypeId, ActiveUnit.UnitUid, PerformerId.PI_CENTER_SERVER, _server.Center.Uid,
                            EventId.ERM_TRADE_START_NOT, new KPacketOK { Ok = NetError.NET_OK });
                    SendPacket(EventId.EGS_CREATE_TRADE_NOT, new KEGS_CREATE_TRADE_NOT { TradeUid = CurrentTradeUid });
                }
                break;
            }

            case EventId.ERM_LEAVE_BATTLE_FIELD_FOR_CALL_MY_LOVER_ACK:
            case EventId.ERM_LEAVE_BATTLE_FIELD_FOR_DUNGEON_GAME_START_ACK:
            case EventId.ERM_LEAVE_BATTLE_FIELD_FOR_JOIN_BATTLE_FIELD_ACK:
            case EventId.ERM_LEAVE_BATTLE_FIELD_FOR_JOIN_WEDDING_HALL_ACK:
            case EventId.ERM_LEAVE_BATTLE_FIELD_FOR_PVP_GAME_START_ACK:
            case EventId.ERM_LEAVE_ROOM_FOR_ENTER_THE_VILLAGE_ACK:
            case EventId.ERM_LEAVE_ROOM_FOR_JOIN_BATTLE_FIELD_ACK:
            case EventId.ERM_LEAVE_ROOM_FOR_WORKINGS_BLOCK_ACK:
                if (ev.GetPayload<KPacketOK>(Region).Ok == NetError.NET_OK)
                {
                    CurrentRoomUid = 0;
                    if (Fsm.CurrentStateId == KGSFSM.S_ROOM)
                        Fsm.TryStateTransition(KGSFSM.I_TO_FIELD_MAP, out _);
                }
                break;

            case EventId.ERM_LEAVE_FIELD_ACK:
                CurrentFieldUid = 0;
                SendPacket(EventId.EGS_STATE_CHANGE_FIELD_ACK, new KEGS_STATE_CHANGE_FIELD_ACK
                {
                    Ok = ev.GetPayload<KPacketOK>(Region).Ok,
                });
                break;

            case EventId.ERM_OPEN_PERSONAL_SHOP_ACK:
            {
                long shopUid = ev.InProcessPayload is KPacketLong ops ? ops.Value : 0;
                int ok = shopUid > 0 ? NetError.NET_OK : NetError.ERR_PERSONAL_SHOP_09;
                if (shopUid <= 0 && ev.TryReadPayload<KERM_OPEN_PERSONAL_SHOP_ACK>(Region, out var opsAck))
                {
                    ok = opsAck.Ok;
                    shopUid = opsAck.PersonalShopUid;
                }
                if (ok == NetError.NET_OK && shopUid > 0)
                {
                    PersonalShopManager.JoinShop(shopUid);
                    SendPacket(EventId.EGS_CREATE_PERSONAL_SHOP_ACK, new KEGS_CREATE_PERSONAL_SHOP_ACK
                    {
                        Ok = NetError.NET_OK,
                        PersonalShopUid = shopUid,
                    });
                }
                else
                {
                    SendPacket(EventId.EGS_CREATE_PERSONAL_SHOP_ACK, new KEGS_CREATE_PERSONAL_SHOP_ACK { Ok = ok != NetError.NET_OK ? ok : NetError.ERR_PERSONAL_SHOP_09 });
                }
                break;
            }

            case EventId.ERM_OPEN_TRADE_ACK:
            {
                long tradeUid = ev.InProcessPayload is KPacketLong ot ? ot.Value : 0;
                if (tradeUid <= 0 && ev.TryReadPayload<KERM_OPEN_TRADE_ACK>(Region, out var otAck) && otAck.Ok == NetError.NET_OK)
                    tradeUid = otAck.TradeUid;
                if (tradeUid > 0)
                {
                    CurrentTradeUid = tradeUid;
                    SendPacket(EventId.EGS_CREATE_TRADE_NOT, new KEGS_CREATE_TRADE_NOT { TradeUid = tradeUid });
                }
                break;
            }

            case EventId.ERM_PARTY_OPEN_DUNGEON_ROOM_NOT:
                if (CurrentRoomUid > 0 && ActiveUnit != null)
                {
                    _server.Router.Send(PerformerTypeId, ActiveUnit.UnitUid, PerformerId.PI_CENTER_SERVER, _server.Center.Uid,
                        EventId.ERM_LEAVE_ROOM_REQ, new KERM_LEAVE_ROOM_REQ
                        {
                            RoomUid = CurrentRoomUid,
                            UnitUid = ActiveUnit.UnitUid,
                        });
                    CurrentRoomUid = 0;
                    if (Fsm.CurrentStateId == KGSFSM.S_ROOM)
                        Fsm.TryStateTransition(KGSFSM.I_TO_FIELD_MAP, out _);
                }
                break;

            case EventId.ERM_PARTY_TO_FIELD_MAP_NOT:
                CurrentRoomUid = 0;
                if (Fsm.CurrentStateId == KGSFSM.S_ROOM)
                    Fsm.TryStateTransition(KGSFSM.I_TO_FIELD_MAP, out _);
                if (ActiveUnit != null)
                {
                    var activeTrade = _server.TradeManager.GetSessionByUnit(ActiveUnit.UnitUid);
                    if (activeTrade != null)
                    {
                        long otherUid = activeTrade.Sender.UnitUid == ActiveUnit.UnitUid
                            ? activeTrade.Receiver.UnitUid
                            : activeTrade.Sender.UnitUid;
                        _server.TradeManager.CancelByUnit(ActiveUnit.UnitUid);
                        CurrentTradeUid = 0;
                        var breakNot = new KEGS_BREAK_TRADE_NOT { Reason = NetError.NOT_LEAVE_ROOM_REASON_16 };
                        SendPacket(EventId.EGS_BREAK_TRADE_NOT, breakNot);
                        if (otherUid > 0)
                            _server.SendToUnit(otherUid, EventId.EGS_BREAK_TRADE_NOT, breakNot);
                    }
                }
                SendPacket(EventId.EGS_PARTY_TO_FIELD_MAP_ACK, ev.InProcessPayload is IKSerializable ptf
                    ? ptf
                    : new KPacketOK { Ok = NetError.NET_OK });
                break;

            case EventId.ERM_PREPARE_FOR_DEFENCE_DUNGEON_REQ:
                if (ActiveUnit != null && CurrentRoomUid > 0)
                    _server.Router.Send(PerformerTypeId, ActiveUnit.UnitUid, PerformerId.PI_CENTER_SERVER, _server.Center.Uid,
                        EventId.ERM_CHANGE_DEFENCE_DUNGEON_PHASE_NOT, new KERM_CHANGE_DEFENCE_DUNGEON_PHASE_NOT { DefenceDungeonPhase = 1 });
                break;

            case EventId.ERM_PREPARE_PICK_UP_FROM_PSHOP_AGENCY_REQ:
                if (ActiveUnit != null)
                    _server.Router.Send(PerformerTypeId, ActiveUnit.UnitUid, PerformerId.PI_LOGIN_SERVER, _server.LoginServerUid,
                        EventId.ERM_PREPARE_PICK_UP_FROM_PSHOP_AGENCY_ACK, new KERM_PREPARE_PICK_UP_FROM_PSHOP_AGENCY_ACK());
                break;

            case EventId.ERM_PVP_USER_STATISTICS_NOT:
                if (ev.TryReadPayload<KPacketOK>(Region, out var pstat))
                    Statistics.AddPlayTime(KUserStatistics.PlayTimeType.PvpPlayTime, Math.Max(0, pstat.Ok));
                break;

            case EventId.ERM_SECRET_STAGE_LOAD_ACK:
                if (ActiveUnit != null && CurrentRoomUid > 0)
                    _server.Router.Send(PerformerTypeId, ActiveUnit.UnitUid, PerformerId.PI_CENTER_SERVER, _server.Center.Uid,
                        EventId.ERM_DUNGEON_STAGE_LOAD_REQ, ev.InProcessPayload ?? new KPacketOK { Ok = NetError.NET_OK });
                break;

            case EventId.ERM_TC_USER_STATISTICS_NOT:
                if (ev.TryReadPayload<KPacketOK>(Region, out var tstat))
                    Statistics.AddPlayTime(KUserStatistics.PlayTimeType.TotalPlayTime, Math.Max(0, tstat.Ok));
                break;

            case EventId.ERM_UPDATE_DUNGEON_UNIT_INFO_NOT:
                if (ActiveUnit != null && ev.TryReadPayload<KPacketOK>(Region, out var dReward) && dReward.Ok > 0)
                {
                    EdManager.TryAddEd(dReward.Ok, KUserEDManager.EdReason.ErInEdDungeonDrop, out int nextEd);
                    ActiveUnit.Ed = nextEd;
                }
                break;

            case EventId.ERM_UPDATE_PVP_UNIT_INFO_NOT:
                if (ActiveUnit != null && ev.TryReadPayload<KPacketOK>(Region, out var pvpPts) && pvpPts.Ok > 0)
                    ActiveUnit.RPoint += pvpPts.Ok;
                break;

            case EventId.ERM_UPDATE_QUEST_INFO_NOT:
                if (ActiveUnit != null && ev.TryReadPayload<KPacketLong>(Region, out var qNpc) && qNpc.Value > 0)
                    Statistics.IncreaseCount(KUserStatistics.UsTable.EtcData, 0, (int)qNpc.Value, 1);
                break;

            case EventId.ERM_UPDATE_VISIT_CASH_SHOP_NOT:
                if (ev.TryReadPayload<KEGS_VISIT_CASH_SHOP_NOT>(Region, out var vcs) && vcs.UnitUid != (ActiveUnit?.UnitUid ?? 0))
                    SendPacket(EventId.EGS_VISIT_CASH_SHOP_NOT, vcs);
                break;

            default:
                KLog.Clog($"KGSUser[{Uid}] unhandled event {ev.EventId} ({ev.EventIdValue})");
                break;
        }
    }

    private void ForwardRoomPacket(KEvent ev, EventId clientEvent)
    {
        SendPacket(clientEvent, ev.InProcessPayload is IKSerializable pkt ? pkt : new KPacketOK { Ok = NetError.NET_OK });
    }

    private void ForwardRoomPacket<TPkt>(KEvent ev, EventId clientEvent) where TPkt : class, IKSerializable, new()
    {
        if (ev.InProcessPayload is TPkt typed)
            SendPacket(clientEvent, typed);
        else if (ev.InProcessPayload is IKSerializable || ev.Buffer.Length > 0)
            SendPacket(clientEvent, ev.GetPayload<TPkt>(Region));
        else
            SendPacket(clientEvent, new TPkt());
    }

    private void ForwardRoomPacketWithOk<TPkt>(KEvent ev, EventId clientEvent, Func<int, TPkt> fromOk) where TPkt : class, IKSerializable, new()
    {
        if (ev.InProcessPayload is TPkt typed)
            SendPacket(clientEvent, typed);
        else if (ev.InProcessPayload is KPacketOK ok)
            SendPacket(clientEvent, fromOk(ok.Ok));
        else if (ev.InProcessPayload is IKSerializable || ev.Buffer.Length > 0)
            SendPacket(clientEvent, ev.GetPayload<TPkt>(Region));
        else
            SendPacket(clientEvent, fromOk(NetError.NET_OK));
    }

    private void LeaveCurrentField()
    {
        if (ActiveUnit == null || CurrentFieldMapId <= 0) return;
        var zone = _server.FieldManager.GetZone(CurrentFieldMapId);
        if (zone != null)
        {
            zone.Leave(ActiveUnit.UnitUid);
            var delNot = new KEGS_CHANGE_USER_LIST_IN_SECTOR_NOT { DelUsers = [ActiveUnit.UnitUid] };
            foreach (long otherUid in zone.GetAllUnitUids())
                _server.SendToUnit(otherUid, EventId.EGS_CHANGE_USER_LIST_IN_SECTOR_NOT, delNot);
        }
        CurrentFieldMapId = 0;
    }

    private KFieldUserInfo BuildFieldUserInfo() => new()
    {
        UnitUid = ActiveUnit!.UnitUid,
        NickName = ActiveUnit.NickName,
        UnitClass = ActiveUnit.UnitClass,
        Level = ActiveUnit.Level,
        Rank = ActiveUnit.Rank,
        TitleId = ActiveUnit.TitleId,
        PartyUid = CurrentPartyUid,
        GuildName = ActiveUnit.UserGuildInfo.GuildName,
        Ed = ActiveUnit.Ed,
        Invisible = IsInvisible,
    };

    private KSquareUserInfo BuildSquareUserInfo() => new()
    {
        GsUid = _server.Uid,
        UnitUid = ActiveUnit!.UnitUid,
        NickName = ActiveUnit.NickName,
        Ip = IpStr,
        Port = _server.UdpRelayPort,
        UnitClass = ActiveUnit.UnitClass,
        Level = ActiveUnit.Level,
        Rank = ActiveUnit.Rank,
        IsParty = CurrentPartyUid > 0,
        TitleId = ActiveUnit.TitleId,
        GuildName = ActiveUnit.UserGuildInfo.GuildName,
        Ed = ActiveUnit.Ed,
    };

    private KRoomUserInfo BuildRoomUserInfo()
    {
        var summonedPet = PetManager.GetSummonedPet();
        return new KRoomUserInfo
        {
            GsUid = _server.Uid,
            OwnerUserUid = AccountUid,
            AuthLevel = (sbyte)AuthLevel,
            UnitUid = ActiveUnit!.UnitUid,
            UnitClass = ActiveUnit.UnitClass,
            NickName = ActiveUnit.NickName,
            Ip = IpStr,
            Port = _server.UdpRelayPort,
            InternalIp = IpStr,
            InternalPort = _server.UdpRelayPort,
            PartyUid = CurrentPartyUid,
            GamePlayStatus = [_gamePlayStatus],
            BuffInfo = ServerBuffManager.ToPacketBuffInfos() is { Count: > 0 } sb ? sb : ActiveUnit.BuffInfo.ToList(),
            Level = ActiveUnit.Level,
            Rating = ActiveUnit.Rating,
            MaxRating = ActiveUnit.MaxRating,
            Rank = ActiveUnit.Rank,
            TitleId = ActiveUnit.TitleId,
            GuildName = ActiveUnit.UserGuildInfo.GuildName,
            GuildUid = ActiveUnit.UserGuildInfo.GuildUid,
            Pet = summonedPet != null ? [summonedPet] : [],
            GoingQuestInfo = QuestManager.GetActiveQuests().Select(q => q.Id).ToHashSet(),
        };
    }

    private KPartyUserInfo BuildPartyUserInfo() => new()
    {
        GsUid = _server.Uid,
        ChannelId = 1,
        FieldUid = CurrentFieldMapId,
        UnitUid = ActiveUnit!.UnitUid,
        UnitClass = ActiveUnit.UnitClass,
        NickName = ActiveUnit.NickName,
        Level = ActiveUnit.Level,
        Rank = ActiveUnit.Rank,
        Rating = ActiveUnit.Rating,
        GamePlayStatus = _gamePlayStatus,
        Ip = IpStr,
        Port = _server.UdpRelayPort,
        InternalIp = IpStr,
        InternalPort = _server.UdpRelayPort,
    };

    private KConnectionUnitInfo BuildConnectionUnitInfo(KUnitInfo unit, KGSUser? onlineUser)
    {
        var (lrInfo, _) = _server.Db.GetLocalRankingUserInfo(unit.OwnerUserUid);
        return new KConnectionUnitInfo
        {
            UnitUid = unit.UnitUid,
            KnmSn = (int)unit.KnmSerialNum,
            UnitClass = unit.UnitClass,
            Level = unit.Level,
            CharName = unit.NickName,
            Rank = unit.Rank,
            RPoint = unit.RPoint,
            Win = unit.Win,
            Lose = unit.Lose,
            ChannelId = onlineUser != null ? 1 : 0,
            State = onlineUser != null ? (sbyte)1 : (sbyte)0,
            PartyUid = onlineUser?.CurrentPartyUid ?? 0,
            RoomUid = onlineUser?.CurrentRoomUid ?? 0,
            StateCode = onlineUser?.CurrentFieldMapId ?? unit.LastPos.MapId,
            GuildName = unit.UserGuildInfo.GuildName,
            LocalRankingUserInfo = lrInfo,
        };
    }

    private int ValidateNickNameForUnit(string nickName, int invalidLenErr, int invalidWordErr)
    {
        if (string.IsNullOrWhiteSpace(nickName)) return invalidLenErr;
        if (nickName.Length < 2 || nickName.Length > 12) return invalidLenErr;
        if (AuthLevel < 5 && !_server.StringFilterManager.CheckIsValidString(KStringFilterManager.FilterType.FtNickName, nickName))
            return invalidWordErr;
        return NetError.NET_OK;
    }

    private int JumpingCharacterCheck(out int targetLevel, out DateTime currentDate)
    {
        currentDate = DateTime.UtcNow;
        targetLevel = 70;
        if (ActiveUnit == null) return NetError.ERR_JUMPING_01;
        if (!_server.GameEventManager.JumpingEventEnabled) return NetError.ERR_JUMPING_01;
        if (!string.IsNullOrEmpty(_jumpingRegDate)) return NetError.ERR_JUMPING_02;
        if (ActiveUnit.Level >= targetLevel) return NetError.ERR_JUMPING_03;
        return NetError.NET_OK;
    }

    private bool IsTradeOrShopOpen()
        => ActiveUnit != null &&
           (PersonalShopManager.PersonalShopUid > 0 || _server.TradeManager.GetSessionByUnit(ActiveUnit.UnitUid) != null);

    private bool GetUnlimitedClassChangeInfo(int newUnitClass, out int retrievedSp, out int retrievedCsp)
    {
        retrievedSp = 0;
        retrievedCsp = 0;
        if (ActiveUnit == null) return false;
        if (!_server.ClassChangeTable.CompareUnitClass(ActiveUnit.UnitClass, newUnitClass)) return false;
        retrievedSp = Math.Max(0, ActiveUnit.Level * 2);
        return true;
    }

    public List<KInventoryItemInfo> SetupDefenceDungeonQuickSlotItems(int defenceDungeonPhase)
    {
        var result = new List<KInventoryItemInfo>();
        _defenceQuickSlotItems.Clear();
        if (ActiveUnit == null)
            return result;

        var ddm = _server.Center.DefenceDungeonManager;
        int specialCardItemId = 0;
        foreach (var item in _server.Db.GetInventory(ActiveUnit.UnitUid))
        {
            if (item.SlotCategory != 9) // ST_E_EQUIP
                continue;
            var sockets = item.ItemInfo.RandomSockets.Count > 0
                ? item.ItemInfo.RandomSockets
                : item.ItemInfo.ItemSockets;
            if (sockets.Count > 0 &&
                ddm.GetResultSpecialCardByDefenceDungeonItem(defenceDungeonPhase, item.ItemInfo.ItemId, sockets, out specialCardItemId))
            {
                break;
            }
        }

        var quickSlotItemIds = new List<int>();
        ddm.GetResultQuickSlotItem(defenceDungeonPhase, quickSlotItemIds);

        short slotId = 0;
        foreach (int qItemId in quickSlotItemIds)
        {
            var slot = new KInventoryItemInfo
            {
                ItemUid = _server.GameSysValTable.NextTempItemUid(),
                SlotCategory = 15, // ST_E_DEFENCE_QUICK_SLOT
                SlotId = slotId++,
                ItemInfo = new KItemInfo
                {
                    ItemId = qItemId,
                    UsageType = 1,
                    Quantity = 1,
                },
            };
            _defenceQuickSlotItems[slot.SlotId] = slot;
            result.Add(slot);
        }

        int slot3ItemId = specialCardItemId > 0 ? specialCardItemId : ddm.DefenceDungeonQuickSlotPotion;
        if (slot3ItemId > 0)
        {
            var slot3 = new KInventoryItemInfo
            {
                ItemUid = _server.GameSysValTable.NextTempItemUid(),
                SlotCategory = 15, // ST_E_DEFENCE_QUICK_SLOT
                SlotId = 3,
                ItemInfo = new KItemInfo
                {
                    ItemId = slot3ItemId,
                    UsageType = 1,
                    Quantity = 1,
                },
            };
            _defenceQuickSlotItems[slot3.SlotId] = slot3;
            result.Add(slot3);
        }

        return result;
    }
}
