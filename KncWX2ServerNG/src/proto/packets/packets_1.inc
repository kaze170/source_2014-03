// ---------------------------------------------------------------------------
// packets_1.inc - GENERATED FILE, DO NOT EDIT BY HAND.
//
// Produced by tools/gen_packets.py from the original KncWX2Server headers:
//     Common/{Common,Client,Server}Packet.{h,cpp}
// Region: SERV_COUNTRY_TW
//
// Field ORDER comes from the SERIALIZE_DEFINE_PUT bodies in the .cpp files -
// that is the wire order, and it is not always the member declaration order.
// Field TYPES come from the struct bodies in the .h files.
//
// Legacy names are preserved exactly (KEGS_CONNECT_ACK, m_iOK, ...) so that
// any struct here can be diffed against the 2014 source by name.
//
// Regenerate with:
//     python3 tools/gen_packets.py --legacy ../KncWX2Server/Common
//                                  --region SERV_COUNTRY_TW
//                                  --out src/proto/packets
// ---------------------------------------------------------------------------
// clang-format off

struct KDBE_UPDATE_IS_LOGIN_NOT {
    std::int64_t                         m_iUserUID{};  // UidType
    bool                                 m_bIsLogin{};
    std::map<std::int32_t, std::int32_t> m_mapAccCountInfo{};  // std::map< int, int >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
        Codec<std::remove_cvref_t<decltype(m_bIsLogin)>>::put(w, m_bIsLogin);
        Codec<std::remove_cvref_t<decltype(m_mapAccCountInfo)>>::put(w, m_mapAccCountInfo);
    }

    static KDBE_UPDATE_IS_LOGIN_NOT read_from(Reader& r) {
        KDBE_UPDATE_IS_LOGIN_NOT v;
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        v.m_bIsLogin = Codec<std::remove_cvref_t<decltype(v.m_bIsLogin)>>::get(r);
        v.m_mapAccCountInfo = Codec<std::remove_cvref_t<decltype(v.m_mapAccCountInfo)>>::get(r);
        return v;
    }
};

struct KDBE_UPDATE_REWARD_COMPLETE_JP_EVENT_NOT {
    std::int64_t m_iRID{};  // __int64

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iRID)>>::put(w, m_iRID);
    }

    static KDBE_UPDATE_REWARD_COMPLETE_JP_EVENT_NOT read_from(Reader& r) {
        KDBE_UPDATE_REWARD_COMPLETE_JP_EVENT_NOT v;
        v.m_iRID = Codec<std::remove_cvref_t<decltype(v.m_iRID)>>::get(r);
        return v;
    }
};

struct KDBE_UPDATE_SECURITY_TYPE_NOT {
    std::int64_t m_iUserUID{};  // UidType
    std::int32_t m_iSecurityType{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
        Codec<std::remove_cvref_t<decltype(m_iSecurityType)>>::put(w, m_iSecurityType);
    }

    static KDBE_UPDATE_SECURITY_TYPE_NOT read_from(Reader& r) {
        KDBE_UPDATE_SECURITY_TYPE_NOT v;
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        v.m_iSecurityType = Codec<std::remove_cvref_t<decltype(v.m_iSecurityType)>>::get(r);
        return v;
    }
};

struct KDBE_UPDATE_SERVER_INFO_REQ {
    std::u16string m_wstrRegDate{};  // std::wstring
    std::int64_t   m_iServerUID{};  // UidType
    std::int32_t   m_iConcurrentUser{};  // int
    std::int32_t   m_iCurPartyCount{};  // int
    std::int32_t   m_iPlayGamePartyCount{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrRegDate)>>::put(w, m_wstrRegDate);
        Codec<std::remove_cvref_t<decltype(m_iServerUID)>>::put(w, m_iServerUID);
        Codec<std::remove_cvref_t<decltype(m_iConcurrentUser)>>::put(w, m_iConcurrentUser);
        Codec<std::remove_cvref_t<decltype(m_iCurPartyCount)>>::put(w, m_iCurPartyCount);
        Codec<std::remove_cvref_t<decltype(m_iPlayGamePartyCount)>>::put(w, m_iPlayGamePartyCount);
    }

    static KDBE_UPDATE_SERVER_INFO_REQ read_from(Reader& r) {
        KDBE_UPDATE_SERVER_INFO_REQ v;
        v.m_wstrRegDate = Codec<std::remove_cvref_t<decltype(v.m_wstrRegDate)>>::get(r);
        v.m_iServerUID = Codec<std::remove_cvref_t<decltype(v.m_iServerUID)>>::get(r);
        v.m_iConcurrentUser = Codec<std::remove_cvref_t<decltype(v.m_iConcurrentUser)>>::get(r);
        v.m_iCurPartyCount = Codec<std::remove_cvref_t<decltype(v.m_iCurPartyCount)>>::get(r);
        v.m_iPlayGamePartyCount = Codec<std::remove_cvref_t<decltype(v.m_iPlayGamePartyCount)>>::get(r);
        return v;
    }
};

struct KDBE_UPDATE_UNIT_CLASS_NOT {
    std::int64_t m_iUnitUID{};  // UidType
    char         m_cUnitClass{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_cUnitClass)>>::put(w, m_cUnitClass);
    }

    static KDBE_UPDATE_UNIT_CLASS_NOT read_from(Reader& r) {
        KDBE_UPDATE_UNIT_CLASS_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_cUnitClass = Codec<std::remove_cvref_t<decltype(v.m_cUnitClass)>>::get(r);
        return v;
    }
};

struct KDBE_UPDATE_USER_INFO_TO_RELAY_DB_JP_EVENT_NOT {
    std::u16string m_wstrUserID{};  // std::wstring
    std::int64_t   m_iUserUID{};  // UidType
    std::int64_t   m_iUnitUID{};  // UidType
    std::u16string m_wstrNickName{};  // std::wstring
    char           m_cLevel{};
    char           m_cUnitClass{};
    bool           m_bDeletedUnit{};
    std::u16string m_wstrUserRegDate{};  // std::wstring
    std::u16string m_wstrUnitRegDate{};  // std::wstring
    std::int32_t   m_iUpdateType{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrUserID)>>::put(w, m_wstrUserID);
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_cLevel)>>::put(w, m_cLevel);
        Codec<std::remove_cvref_t<decltype(m_cUnitClass)>>::put(w, m_cUnitClass);
        Codec<std::remove_cvref_t<decltype(m_bDeletedUnit)>>::put(w, m_bDeletedUnit);
        Codec<std::remove_cvref_t<decltype(m_wstrUserRegDate)>>::put(w, m_wstrUserRegDate);
        Codec<std::remove_cvref_t<decltype(m_wstrUnitRegDate)>>::put(w, m_wstrUnitRegDate);
        Codec<std::remove_cvref_t<decltype(m_iUpdateType)>>::put(w, m_iUpdateType);
    }

    static KDBE_UPDATE_USER_INFO_TO_RELAY_DB_JP_EVENT_NOT read_from(Reader& r) {
        KDBE_UPDATE_USER_INFO_TO_RELAY_DB_JP_EVENT_NOT v;
        v.m_wstrUserID = Codec<std::remove_cvref_t<decltype(v.m_wstrUserID)>>::get(r);
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_cLevel = Codec<std::remove_cvref_t<decltype(v.m_cLevel)>>::get(r);
        v.m_cUnitClass = Codec<std::remove_cvref_t<decltype(v.m_cUnitClass)>>::get(r);
        v.m_bDeletedUnit = Codec<std::remove_cvref_t<decltype(v.m_bDeletedUnit)>>::get(r);
        v.m_wstrUserRegDate = Codec<std::remove_cvref_t<decltype(v.m_wstrUserRegDate)>>::get(r);
        v.m_wstrUnitRegDate = Codec<std::remove_cvref_t<decltype(v.m_wstrUnitRegDate)>>::get(r);
        v.m_iUpdateType = Codec<std::remove_cvref_t<decltype(v.m_iUpdateType)>>::get(r);
        return v;
    }
};

struct KDBE_WEDDING_COMPLETE_ACK {
    std::int32_t   m_iOK{};  // int
    std::u16string m_wstrWeddingDate{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_wstrWeddingDate)>>::put(w, m_wstrWeddingDate);
    }

    static KDBE_WEDDING_COMPLETE_ACK read_from(Reader& r) {
        KDBE_WEDDING_COMPLETE_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_wstrWeddingDate = Codec<std::remove_cvref_t<decltype(v.m_wstrWeddingDate)>>::get(r);
        return v;
    }
};

struct KDBE_WEDDING_COMPLETE_REQ {
    std::int32_t   m_iWeddingUID{};  // int
    std::int64_t   m_iGroom{};  // UidType
    std::int64_t   m_iBride{};  // UidType
    std::u16string m_wstrWeddingDate{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iWeddingUID)>>::put(w, m_iWeddingUID);
        Codec<std::remove_cvref_t<decltype(m_iGroom)>>::put(w, m_iGroom);
        Codec<std::remove_cvref_t<decltype(m_iBride)>>::put(w, m_iBride);
        Codec<std::remove_cvref_t<decltype(m_wstrWeddingDate)>>::put(w, m_wstrWeddingDate);
    }

    static KDBE_WEDDING_COMPLETE_REQ read_from(Reader& r) {
        KDBE_WEDDING_COMPLETE_REQ v;
        v.m_iWeddingUID = Codec<std::remove_cvref_t<decltype(v.m_iWeddingUID)>>::get(r);
        v.m_iGroom = Codec<std::remove_cvref_t<decltype(v.m_iGroom)>>::get(r);
        v.m_iBride = Codec<std::remove_cvref_t<decltype(v.m_iBride)>>::get(r);
        v.m_wstrWeddingDate = Codec<std::remove_cvref_t<decltype(v.m_wstrWeddingDate)>>::get(r);
        return v;
    }
};

struct KDBE_WRITE_COME_BACK_LOGOUT_NOT {
    std::int64_t   m_iUserUID{};  // UidType
    std::u16string m_wsrtLogOutDate{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
        Codec<std::remove_cvref_t<decltype(m_wsrtLogOutDate)>>::put(w, m_wsrtLogOutDate);
    }

    static KDBE_WRITE_COME_BACK_LOGOUT_NOT read_from(Reader& r) {
        KDBE_WRITE_COME_BACK_LOGOUT_NOT v;
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        v.m_wsrtLogOutDate = Codec<std::remove_cvref_t<decltype(v.m_wsrtLogOutDate)>>::get(r);
        return v;
    }
};

struct KDBE_WRITE_COME_BACK_REWARD_NOT {
    std::int64_t   m_iUserUID{};  // UidType
    std::int32_t   m_iRewardStep{};  // int
    std::u16string m_wstrComeBackBuffEndDate{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
        Codec<std::remove_cvref_t<decltype(m_iRewardStep)>>::put(w, m_iRewardStep);
        Codec<std::remove_cvref_t<decltype(m_wstrComeBackBuffEndDate)>>::put(w, m_wstrComeBackBuffEndDate);
    }

    static KDBE_WRITE_COME_BACK_REWARD_NOT read_from(Reader& r) {
        KDBE_WRITE_COME_BACK_REWARD_NOT v;
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        v.m_iRewardStep = Codec<std::remove_cvref_t<decltype(v.m_iRewardStep)>>::get(r);
        v.m_wstrComeBackBuffEndDate = Codec<std::remove_cvref_t<decltype(v.m_wstrComeBackBuffEndDate)>>::get(r);
        return v;
    }
};

struct KDBE_WRITE_LETTER_AND_TRADE_MOINTORING_REQ {
    char           m_cType{};
    std::int64_t   m_iRecvUnitUID{};  // UidType
    std::u16string m_wstrRegDate{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cType)>>::put(w, m_cType);
        Codec<std::remove_cvref_t<decltype(m_iRecvUnitUID)>>::put(w, m_iRecvUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrRegDate)>>::put(w, m_wstrRegDate);
    }

    static KDBE_WRITE_LETTER_AND_TRADE_MOINTORING_REQ read_from(Reader& r) {
        KDBE_WRITE_LETTER_AND_TRADE_MOINTORING_REQ v;
        v.m_cType = Codec<std::remove_cvref_t<decltype(v.m_cType)>>::get(r);
        v.m_iRecvUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iRecvUnitUID)>>::get(r);
        v.m_wstrRegDate = Codec<std::remove_cvref_t<decltype(v.m_wstrRegDate)>>::get(r);
        return v;
    }
};

struct KDBE_WRITE_RELATIONSHIP_INFO_LOG_ACK {
    std::int32_t m_iOK{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
    }

    static KDBE_WRITE_RELATIONSHIP_INFO_LOG_ACK read_from(Reader& r) {
        KDBE_WRITE_RELATIONSHIP_INFO_LOG_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        return v;
    }
};

struct KDBE_WRITE_RELATIONSHIP_INFO_LOG_REQ {
    std::int64_t   m_iUserUID{};  // UidType
    std::int64_t   m_iUnitUID{};  // UidType
    std::uint8_t   m_ucLevel{};  // u_char
    char           m_cUnitClass{};
    char           m_cCoupleType{};
    std::u16string m_wstrRegDate{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_ucLevel)>>::put(w, m_ucLevel);
        Codec<std::remove_cvref_t<decltype(m_cUnitClass)>>::put(w, m_cUnitClass);
        Codec<std::remove_cvref_t<decltype(m_cCoupleType)>>::put(w, m_cCoupleType);
        Codec<std::remove_cvref_t<decltype(m_wstrRegDate)>>::put(w, m_wstrRegDate);
    }

    static KDBE_WRITE_RELATIONSHIP_INFO_LOG_REQ read_from(Reader& r) {
        KDBE_WRITE_RELATIONSHIP_INFO_LOG_REQ v;
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_ucLevel = Codec<std::remove_cvref_t<decltype(v.m_ucLevel)>>::get(r);
        v.m_cUnitClass = Codec<std::remove_cvref_t<decltype(v.m_cUnitClass)>>::get(r);
        v.m_cCoupleType = Codec<std::remove_cvref_t<decltype(v.m_cCoupleType)>>::get(r);
        v.m_wstrRegDate = Codec<std::remove_cvref_t<decltype(v.m_wstrRegDate)>>::get(r);
        return v;
    }
};

struct KDBUpdateGamePlayNetworkInfo {
    std::int32_t   m_iGameType{};  // int
    std::int32_t   m_iHolePunchingTryCount{};  // int
    std::int32_t   m_iP2PPlayTime{};  // int
    std::int32_t   m_iRelayPlayTime{};  // int
    std::u16string m_wstrRelayServerIP{};  // std::wstring
    std::u16string m_wstrRegDate{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iGameType)>>::put(w, m_iGameType);
        Codec<std::remove_cvref_t<decltype(m_iHolePunchingTryCount)>>::put(w, m_iHolePunchingTryCount);
        Codec<std::remove_cvref_t<decltype(m_iP2PPlayTime)>>::put(w, m_iP2PPlayTime);
        Codec<std::remove_cvref_t<decltype(m_iRelayPlayTime)>>::put(w, m_iRelayPlayTime);
        Codec<std::remove_cvref_t<decltype(m_wstrRelayServerIP)>>::put(w, m_wstrRelayServerIP);
        Codec<std::remove_cvref_t<decltype(m_wstrRegDate)>>::put(w, m_wstrRegDate);
    }

    static KDBUpdateGamePlayNetworkInfo read_from(Reader& r) {
        KDBUpdateGamePlayNetworkInfo v;
        v.m_iGameType = Codec<std::remove_cvref_t<decltype(v.m_iGameType)>>::get(r);
        v.m_iHolePunchingTryCount = Codec<std::remove_cvref_t<decltype(v.m_iHolePunchingTryCount)>>::get(r);
        v.m_iP2PPlayTime = Codec<std::remove_cvref_t<decltype(v.m_iP2PPlayTime)>>::get(r);
        v.m_iRelayPlayTime = Codec<std::remove_cvref_t<decltype(v.m_iRelayPlayTime)>>::get(r);
        v.m_wstrRelayServerIP = Codec<std::remove_cvref_t<decltype(v.m_wstrRelayServerIP)>>::get(r);
        v.m_wstrRegDate = Codec<std::remove_cvref_t<decltype(v.m_wstrRegDate)>>::get(r);
        return v;
    }
};

struct KDangerousEventInfo {
    std::set<std::int32_t> m_setEventInfo{};  // std::set< int >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_setEventInfo)>>::put(w, m_setEventInfo);
    }

    static KDangerousEventInfo read_from(Reader& r) {
        KDangerousEventInfo v;
        v.m_setEventInfo = Codec<std::remove_cvref_t<decltype(v.m_setEventInfo)>>::get(r);
        return v;
    }
};

struct KDecreaseEnduranceInfo {
    std::int64_t m_iItemUID{};  // UidType
    std::int32_t m_iEndurance{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_iEndurance)>>::put(w, m_iEndurance);
    }

    static KDecreaseEnduranceInfo read_from(Reader& r) {
        KDecreaseEnduranceInfo v;
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_iEndurance = Codec<std::remove_cvref_t<decltype(v.m_iEndurance)>>::get(r);
        return v;
    }
};

struct KDefenseOpenTime {
    std::int32_t m_iStartHour{};  // int
    std::int32_t m_iStartMinute{};  // int
    std::int32_t m_iDurationTime{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iStartHour)>>::put(w, m_iStartHour);
        Codec<std::remove_cvref_t<decltype(m_iStartMinute)>>::put(w, m_iStartMinute);
        Codec<std::remove_cvref_t<decltype(m_iDurationTime)>>::put(w, m_iDurationTime);
    }

    static KDefenseOpenTime read_from(Reader& r) {
        KDefenseOpenTime v;
        v.m_iStartHour = Codec<std::remove_cvref_t<decltype(v.m_iStartHour)>>::get(r);
        v.m_iStartMinute = Codec<std::remove_cvref_t<decltype(v.m_iStartMinute)>>::get(r);
        v.m_iDurationTime = Codec<std::remove_cvref_t<decltype(v.m_iDurationTime)>>::get(r);
        return v;
    }
};

struct KDeletedItemInfo {
    std::int64_t m_iItemUID{};  // UidType
    std::uint8_t m_ucDeleteReason{};  // u_char

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_ucDeleteReason)>>::put(w, m_ucDeleteReason);
    }

    static KDeletedItemInfo read_from(Reader& r) {
        KDeletedItemInfo v;
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_ucDeleteReason = Codec<std::remove_cvref_t<decltype(v.m_ucDeleteReason)>>::get(r);
        return v;
    }
};

struct KDenyOptions {
    char m_cDenyFriendShip{};
    char m_cDenyInviteGuild{};
    char m_cDenyParty{};
    char m_cDenyPersonalTrade{};
    char m_cDenyRequestCouple{};
    char m_cDenyInvitePracticePVP{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cDenyFriendShip)>>::put(w, m_cDenyFriendShip);
        Codec<std::remove_cvref_t<decltype(m_cDenyInviteGuild)>>::put(w, m_cDenyInviteGuild);
        Codec<std::remove_cvref_t<decltype(m_cDenyParty)>>::put(w, m_cDenyParty);
        Codec<std::remove_cvref_t<decltype(m_cDenyPersonalTrade)>>::put(w, m_cDenyPersonalTrade);
        Codec<std::remove_cvref_t<decltype(m_cDenyRequestCouple)>>::put(w, m_cDenyRequestCouple);
        Codec<std::remove_cvref_t<decltype(m_cDenyInvitePracticePVP)>>::put(w, m_cDenyInvitePracticePVP);
    }

    static KDenyOptions read_from(Reader& r) {
        KDenyOptions v;
        v.m_cDenyFriendShip = Codec<std::remove_cvref_t<decltype(v.m_cDenyFriendShip)>>::get(r);
        v.m_cDenyInviteGuild = Codec<std::remove_cvref_t<decltype(v.m_cDenyInviteGuild)>>::get(r);
        v.m_cDenyParty = Codec<std::remove_cvref_t<decltype(v.m_cDenyParty)>>::get(r);
        v.m_cDenyPersonalTrade = Codec<std::remove_cvref_t<decltype(v.m_cDenyPersonalTrade)>>::get(r);
        v.m_cDenyRequestCouple = Codec<std::remove_cvref_t<decltype(v.m_cDenyRequestCouple)>>::get(r);
        v.m_cDenyInvitePracticePVP = Codec<std::remove_cvref_t<decltype(v.m_cDenyInvitePracticePVP)>>::get(r);
        return v;
    }
};

struct KDropItemData {
    std::int32_t           m_iItemID{};  // int
    std::int32_t           m_iDropItemUID{};  // int
    bool                   m_bLeft{};
    float                  m_fSpeed{};
    std::set<std::int64_t> m_setGetPossibleUnitUID{};  // std::set< UidType >
    char                   m_cEnchantLevel{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iItemID)>>::put(w, m_iItemID);
        Codec<std::remove_cvref_t<decltype(m_iDropItemUID)>>::put(w, m_iDropItemUID);
        Codec<std::remove_cvref_t<decltype(m_bLeft)>>::put(w, m_bLeft);
        Codec<std::remove_cvref_t<decltype(m_fSpeed)>>::put(w, m_fSpeed);
        Codec<std::remove_cvref_t<decltype(m_setGetPossibleUnitUID)>>::put(w, m_setGetPossibleUnitUID);
        Codec<std::remove_cvref_t<decltype(m_cEnchantLevel)>>::put(w, m_cEnchantLevel);
    }

    static KDropItemData read_from(Reader& r) {
        KDropItemData v;
        v.m_iItemID = Codec<std::remove_cvref_t<decltype(v.m_iItemID)>>::get(r);
        v.m_iDropItemUID = Codec<std::remove_cvref_t<decltype(v.m_iDropItemUID)>>::get(r);
        v.m_bLeft = Codec<std::remove_cvref_t<decltype(v.m_bLeft)>>::get(r);
        v.m_fSpeed = Codec<std::remove_cvref_t<decltype(v.m_fSpeed)>>::get(r);
        v.m_setGetPossibleUnitUID = Codec<std::remove_cvref_t<decltype(v.m_setGetPossibleUnitUID)>>::get(r);
        v.m_cEnchantLevel = Codec<std::remove_cvref_t<decltype(v.m_cEnchantLevel)>>::get(r);
        return v;
    }
};

struct KDungeonClearInfo {
    std::int32_t   m_iDungeonID{};  // int
    std::int32_t   m_iMaxScore{};  // int
    char           m_cMaxTotalRank{};
    std::u16string m_wstrClearTime{};  // std::wstring
    bool           m_bNew{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iDungeonID)>>::put(w, m_iDungeonID);
        Codec<std::remove_cvref_t<decltype(m_iMaxScore)>>::put(w, m_iMaxScore);
        Codec<std::remove_cvref_t<decltype(m_cMaxTotalRank)>>::put(w, m_cMaxTotalRank);
        Codec<std::remove_cvref_t<decltype(m_wstrClearTime)>>::put(w, m_wstrClearTime);
        Codec<std::remove_cvref_t<decltype(m_bNew)>>::put(w, m_bNew);
    }

    static KDungeonClearInfo read_from(Reader& r) {
        KDungeonClearInfo v;
        v.m_iDungeonID = Codec<std::remove_cvref_t<decltype(v.m_iDungeonID)>>::get(r);
        v.m_iMaxScore = Codec<std::remove_cvref_t<decltype(v.m_iMaxScore)>>::get(r);
        v.m_cMaxTotalRank = Codec<std::remove_cvref_t<decltype(v.m_cMaxTotalRank)>>::get(r);
        v.m_wstrClearTime = Codec<std::remove_cvref_t<decltype(v.m_wstrClearTime)>>::get(r);
        v.m_bNew = Codec<std::remove_cvref_t<decltype(v.m_bNew)>>::get(r);
        return v;
    }
};

struct KDungeonDropEventNpcInfo {
    std::int32_t m_iEventUID{};  // int
    std::int32_t m_iNpcID{};  // int
    std::int64_t m_tDropDate{};  // __int64

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iEventUID)>>::put(w, m_iEventUID);
        Codec<std::remove_cvref_t<decltype(m_iNpcID)>>::put(w, m_iNpcID);
        Codec<std::remove_cvref_t<decltype(m_tDropDate)>>::put(w, m_tDropDate);
    }

    static KDungeonDropEventNpcInfo read_from(Reader& r) {
        KDungeonDropEventNpcInfo v;
        v.m_iEventUID = Codec<std::remove_cvref_t<decltype(v.m_iEventUID)>>::get(r);
        v.m_iNpcID = Codec<std::remove_cvref_t<decltype(v.m_iNpcID)>>::get(r);
        v.m_tDropDate = Codec<std::remove_cvref_t<decltype(v.m_tDropDate)>>::get(r);
        return v;
    }
};

struct KDBE_GET_DUNGEON_EVENT_INFO_ACK {
    std::vector<KDungeonDropEventNpcInfo> m_vecEventInfo{};  // std::vector< KDungeonDropEventNpcInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecEventInfo)>>::put(w, m_vecEventInfo);
    }

    static KDBE_GET_DUNGEON_EVENT_INFO_ACK read_from(Reader& r) {
        KDBE_GET_DUNGEON_EVENT_INFO_ACK v;
        v.m_vecEventInfo = Codec<std::remove_cvref_t<decltype(v.m_vecEventInfo)>>::get(r);
        return v;
    }
};

struct KDungeonGameInfo {
    std::int32_t m_iDungeonID{};  // int
    char         m_cDungeonMode{};
    char         m_cDifficulty{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iDungeonID)>>::put(w, m_iDungeonID);
        Codec<std::remove_cvref_t<decltype(m_cDungeonMode)>>::put(w, m_cDungeonMode);
        Codec<std::remove_cvref_t<decltype(m_cDifficulty)>>::put(w, m_cDifficulty);
    }

    static KDungeonGameInfo read_from(Reader& r) {
        KDungeonGameInfo v;
        v.m_iDungeonID = Codec<std::remove_cvref_t<decltype(v.m_iDungeonID)>>::get(r);
        v.m_cDungeonMode = Codec<std::remove_cvref_t<decltype(v.m_cDungeonMode)>>::get(r);
        v.m_cDifficulty = Codec<std::remove_cvref_t<decltype(v.m_cDifficulty)>>::get(r);
        return v;
    }
};

struct KDungeonGameSimpleInfo {
    std::int32_t m_iDungeonID{};  // int
    char         m_cDungeonMode{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iDungeonID)>>::put(w, m_iDungeonID);
        Codec<std::remove_cvref_t<decltype(m_cDungeonMode)>>::put(w, m_cDungeonMode);
    }

    static KDungeonGameSimpleInfo read_from(Reader& r) {
        KDungeonGameSimpleInfo v;
        v.m_iDungeonID = Codec<std::remove_cvref_t<decltype(v.m_iDungeonID)>>::get(r);
        v.m_cDungeonMode = Codec<std::remove_cvref_t<decltype(v.m_cDungeonMode)>>::get(r);
        return v;
    }
};

struct KDungeonPlayInfo {
    std::int32_t m_iDungeonID{};  // int
    std::int32_t m_iPlayTimes{};  // int
    std::int32_t m_iClearTimes{};  // int
    bool         m_bNew{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iDungeonID)>>::put(w, m_iDungeonID);
        Codec<std::remove_cvref_t<decltype(m_iPlayTimes)>>::put(w, m_iPlayTimes);
        Codec<std::remove_cvref_t<decltype(m_iClearTimes)>>::put(w, m_iClearTimes);
        Codec<std::remove_cvref_t<decltype(m_bNew)>>::put(w, m_bNew);
    }

    static KDungeonPlayInfo read_from(Reader& r) {
        KDungeonPlayInfo v;
        v.m_iDungeonID = Codec<std::remove_cvref_t<decltype(v.m_iDungeonID)>>::get(r);
        v.m_iPlayTimes = Codec<std::remove_cvref_t<decltype(v.m_iPlayTimes)>>::get(r);
        v.m_iClearTimes = Codec<std::remove_cvref_t<decltype(v.m_iClearTimes)>>::get(r);
        v.m_bNew = Codec<std::remove_cvref_t<decltype(v.m_bNew)>>::get(r);
        return v;
    }
};

struct KDungeonPlayResultInfo {
    float        m_fHP{};
    std::int32_t m_nDamageCount{};  // int
    std::int32_t m_TotalScore{};  // int
    std::int32_t m_ComboScore{};  // int
    std::int32_t m_TechScore{};  // int
    std::int32_t m_iTotalGivenDamamge{};  // int
    std::int32_t m_iTotalAttackedDamage{};  // int
    std::int32_t m_iTotalSumNpcHP{};  // int
    std::int32_t m_iTotalRecoveryHP{};  // int
    std::int32_t m_iUsingKindOfSkill{};  // int
    std::int32_t m_iCommandTechScore{};  // int
    std::int32_t m_iSkillTechScore{};  // int
    std::int32_t m_iDashCount{};  // int
    std::int32_t m_iFrame{};  // int
    bool         m_bSkillSlotType{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_fHP)>>::put(w, m_fHP);
        Codec<std::remove_cvref_t<decltype(m_nDamageCount)>>::put(w, m_nDamageCount);
        Codec<std::remove_cvref_t<decltype(m_TotalScore)>>::put(w, m_TotalScore);
        Codec<std::remove_cvref_t<decltype(m_ComboScore)>>::put(w, m_ComboScore);
        Codec<std::remove_cvref_t<decltype(m_TechScore)>>::put(w, m_TechScore);
        Codec<std::remove_cvref_t<decltype(m_iTotalGivenDamamge)>>::put(w, m_iTotalGivenDamamge);
        Codec<std::remove_cvref_t<decltype(m_iTotalAttackedDamage)>>::put(w, m_iTotalAttackedDamage);
        Codec<std::remove_cvref_t<decltype(m_iTotalSumNpcHP)>>::put(w, m_iTotalSumNpcHP);
        Codec<std::remove_cvref_t<decltype(m_iTotalRecoveryHP)>>::put(w, m_iTotalRecoveryHP);
        Codec<std::remove_cvref_t<decltype(m_iUsingKindOfSkill)>>::put(w, m_iUsingKindOfSkill);
        Codec<std::remove_cvref_t<decltype(m_iCommandTechScore)>>::put(w, m_iCommandTechScore);
        Codec<std::remove_cvref_t<decltype(m_iSkillTechScore)>>::put(w, m_iSkillTechScore);
        Codec<std::remove_cvref_t<decltype(m_iDashCount)>>::put(w, m_iDashCount);
        Codec<std::remove_cvref_t<decltype(m_iFrame)>>::put(w, m_iFrame);
        Codec<std::remove_cvref_t<decltype(m_bSkillSlotType)>>::put(w, m_bSkillSlotType);
    }

    static KDungeonPlayResultInfo read_from(Reader& r) {
        KDungeonPlayResultInfo v;
        v.m_fHP = Codec<std::remove_cvref_t<decltype(v.m_fHP)>>::get(r);
        v.m_nDamageCount = Codec<std::remove_cvref_t<decltype(v.m_nDamageCount)>>::get(r);
        v.m_TotalScore = Codec<std::remove_cvref_t<decltype(v.m_TotalScore)>>::get(r);
        v.m_ComboScore = Codec<std::remove_cvref_t<decltype(v.m_ComboScore)>>::get(r);
        v.m_TechScore = Codec<std::remove_cvref_t<decltype(v.m_TechScore)>>::get(r);
        v.m_iTotalGivenDamamge = Codec<std::remove_cvref_t<decltype(v.m_iTotalGivenDamamge)>>::get(r);
        v.m_iTotalAttackedDamage = Codec<std::remove_cvref_t<decltype(v.m_iTotalAttackedDamage)>>::get(r);
        v.m_iTotalSumNpcHP = Codec<std::remove_cvref_t<decltype(v.m_iTotalSumNpcHP)>>::get(r);
        v.m_iTotalRecoveryHP = Codec<std::remove_cvref_t<decltype(v.m_iTotalRecoveryHP)>>::get(r);
        v.m_iUsingKindOfSkill = Codec<std::remove_cvref_t<decltype(v.m_iUsingKindOfSkill)>>::get(r);
        v.m_iCommandTechScore = Codec<std::remove_cvref_t<decltype(v.m_iCommandTechScore)>>::get(r);
        v.m_iSkillTechScore = Codec<std::remove_cvref_t<decltype(v.m_iSkillTechScore)>>::get(r);
        v.m_iDashCount = Codec<std::remove_cvref_t<decltype(v.m_iDashCount)>>::get(r);
        v.m_iFrame = Codec<std::remove_cvref_t<decltype(v.m_iFrame)>>::get(r);
        v.m_bSkillSlotType = Codec<std::remove_cvref_t<decltype(v.m_bSkillSlotType)>>::get(r);
        return v;
    }
};

struct KDungeonRankingInfo {
    std::int32_t   m_iRank{};  // int
    std::int64_t   m_iUnitUID{};  // UidType
    std::u16string m_wstrNickName{};  // std::wstring
    char           m_cUnitClass{};
    std::uint8_t   m_ucLevel{};  // UCHAR
    std::int32_t   m_iEXP{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iRank)>>::put(w, m_iRank);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_cUnitClass)>>::put(w, m_cUnitClass);
        Codec<std::remove_cvref_t<decltype(m_ucLevel)>>::put(w, m_ucLevel);
        Codec<std::remove_cvref_t<decltype(m_iEXP)>>::put(w, m_iEXP);
    }

    static KDungeonRankingInfo read_from(Reader& r) {
        KDungeonRankingInfo v;
        v.m_iRank = Codec<std::remove_cvref_t<decltype(v.m_iRank)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_cUnitClass = Codec<std::remove_cvref_t<decltype(v.m_cUnitClass)>>::get(r);
        v.m_ucLevel = Codec<std::remove_cvref_t<decltype(v.m_ucLevel)>>::get(r);
        v.m_iEXP = Codec<std::remove_cvref_t<decltype(v.m_iEXP)>>::get(r);
        return v;
    }
};

struct KDungeonRewardED {
    std::int32_t m_iED{};  // int
    std::int32_t m_iBonusED{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iED)>>::put(w, m_iED);
        Codec<std::remove_cvref_t<decltype(m_iBonusED)>>::put(w, m_iBonusED);
    }

    static KDungeonRewardED read_from(Reader& r) {
        KDungeonRewardED v;
        v.m_iED = Codec<std::remove_cvref_t<decltype(v.m_iED)>>::get(r);
        v.m_iBonusED = Codec<std::remove_cvref_t<decltype(v.m_iBonusED)>>::get(r);
        return v;
    }
};

struct KDungeonUnitResultInfo {
    std::int64_t m_UnitUID{};  // UidType
    bool         m_bIsDie{};
    std::int32_t m_nTotalScore{};  // int
    std::int32_t m_nComboScore{};  // int
    std::int32_t m_nTechnicalScore{};  // int
    std::int32_t m_nDamaged{};  // int
    std::int32_t m_nOldED{};  // int
    std::int32_t m_nOldEXP{};  // int
    std::int32_t m_nED{};  // int
    std::int32_t m_nEXP{};  // int
    std::int32_t m_nClearBonusEXP{};  // int
    std::int32_t m_nRankBonusEXP{};  // int
    std::int32_t m_nPartyBonusEXP{};  // int
    std::int32_t m_nMvpBonusEXP{};  // int
    std::int32_t m_nGameBangBonusEXP{};  // int
    std::int32_t m_nPremiumBonusEXP{};  // int
    std::int32_t m_nEventEXP{};  // int
    std::int32_t m_nChannelBonusEXP{};  // int
    std::int32_t m_nKillNPCNum{};  // int
    char         m_cComboRank{};
    char         m_cTechnicalRank{};
    char         m_cDamagedRank{};
    char         m_cTimeRank{};
    char         m_cTotalRank{};
    bool         m_bIsMVP{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_UnitUID)>>::put(w, m_UnitUID);
        Codec<std::remove_cvref_t<decltype(m_bIsDie)>>::put(w, m_bIsDie);
        Codec<std::remove_cvref_t<decltype(m_nTotalScore)>>::put(w, m_nTotalScore);
        Codec<std::remove_cvref_t<decltype(m_nComboScore)>>::put(w, m_nComboScore);
        Codec<std::remove_cvref_t<decltype(m_nTechnicalScore)>>::put(w, m_nTechnicalScore);
        Codec<std::remove_cvref_t<decltype(m_nDamaged)>>::put(w, m_nDamaged);
        Codec<std::remove_cvref_t<decltype(m_nOldED)>>::put(w, m_nOldED);
        Codec<std::remove_cvref_t<decltype(m_nOldEXP)>>::put(w, m_nOldEXP);
        Codec<std::remove_cvref_t<decltype(m_nED)>>::put(w, m_nED);
        Codec<std::remove_cvref_t<decltype(m_nEXP)>>::put(w, m_nEXP);
        Codec<std::remove_cvref_t<decltype(m_nClearBonusEXP)>>::put(w, m_nClearBonusEXP);
        Codec<std::remove_cvref_t<decltype(m_nRankBonusEXP)>>::put(w, m_nRankBonusEXP);
        Codec<std::remove_cvref_t<decltype(m_nPartyBonusEXP)>>::put(w, m_nPartyBonusEXP);
        Codec<std::remove_cvref_t<decltype(m_nMvpBonusEXP)>>::put(w, m_nMvpBonusEXP);
        Codec<std::remove_cvref_t<decltype(m_nGameBangBonusEXP)>>::put(w, m_nGameBangBonusEXP);
        Codec<std::remove_cvref_t<decltype(m_nPremiumBonusEXP)>>::put(w, m_nPremiumBonusEXP);
        Codec<std::remove_cvref_t<decltype(m_nEventEXP)>>::put(w, m_nEventEXP);
        Codec<std::remove_cvref_t<decltype(m_nChannelBonusEXP)>>::put(w, m_nChannelBonusEXP);
        Codec<std::remove_cvref_t<decltype(m_nKillNPCNum)>>::put(w, m_nKillNPCNum);
        Codec<std::remove_cvref_t<decltype(m_cComboRank)>>::put(w, m_cComboRank);
        Codec<std::remove_cvref_t<decltype(m_cTechnicalRank)>>::put(w, m_cTechnicalRank);
        Codec<std::remove_cvref_t<decltype(m_cDamagedRank)>>::put(w, m_cDamagedRank);
        Codec<std::remove_cvref_t<decltype(m_cTimeRank)>>::put(w, m_cTimeRank);
        Codec<std::remove_cvref_t<decltype(m_cTotalRank)>>::put(w, m_cTotalRank);
        Codec<std::remove_cvref_t<decltype(m_bIsMVP)>>::put(w, m_bIsMVP);
    }

    static KDungeonUnitResultInfo read_from(Reader& r) {
        KDungeonUnitResultInfo v;
        v.m_UnitUID = Codec<std::remove_cvref_t<decltype(v.m_UnitUID)>>::get(r);
        v.m_bIsDie = Codec<std::remove_cvref_t<decltype(v.m_bIsDie)>>::get(r);
        v.m_nTotalScore = Codec<std::remove_cvref_t<decltype(v.m_nTotalScore)>>::get(r);
        v.m_nComboScore = Codec<std::remove_cvref_t<decltype(v.m_nComboScore)>>::get(r);
        v.m_nTechnicalScore = Codec<std::remove_cvref_t<decltype(v.m_nTechnicalScore)>>::get(r);
        v.m_nDamaged = Codec<std::remove_cvref_t<decltype(v.m_nDamaged)>>::get(r);
        v.m_nOldED = Codec<std::remove_cvref_t<decltype(v.m_nOldED)>>::get(r);
        v.m_nOldEXP = Codec<std::remove_cvref_t<decltype(v.m_nOldEXP)>>::get(r);
        v.m_nED = Codec<std::remove_cvref_t<decltype(v.m_nED)>>::get(r);
        v.m_nEXP = Codec<std::remove_cvref_t<decltype(v.m_nEXP)>>::get(r);
        v.m_nClearBonusEXP = Codec<std::remove_cvref_t<decltype(v.m_nClearBonusEXP)>>::get(r);
        v.m_nRankBonusEXP = Codec<std::remove_cvref_t<decltype(v.m_nRankBonusEXP)>>::get(r);
        v.m_nPartyBonusEXP = Codec<std::remove_cvref_t<decltype(v.m_nPartyBonusEXP)>>::get(r);
        v.m_nMvpBonusEXP = Codec<std::remove_cvref_t<decltype(v.m_nMvpBonusEXP)>>::get(r);
        v.m_nGameBangBonusEXP = Codec<std::remove_cvref_t<decltype(v.m_nGameBangBonusEXP)>>::get(r);
        v.m_nPremiumBonusEXP = Codec<std::remove_cvref_t<decltype(v.m_nPremiumBonusEXP)>>::get(r);
        v.m_nEventEXP = Codec<std::remove_cvref_t<decltype(v.m_nEventEXP)>>::get(r);
        v.m_nChannelBonusEXP = Codec<std::remove_cvref_t<decltype(v.m_nChannelBonusEXP)>>::get(r);
        v.m_nKillNPCNum = Codec<std::remove_cvref_t<decltype(v.m_nKillNPCNum)>>::get(r);
        v.m_cComboRank = Codec<std::remove_cvref_t<decltype(v.m_cComboRank)>>::get(r);
        v.m_cTechnicalRank = Codec<std::remove_cvref_t<decltype(v.m_cTechnicalRank)>>::get(r);
        v.m_cDamagedRank = Codec<std::remove_cvref_t<decltype(v.m_cDamagedRank)>>::get(r);
        v.m_cTimeRank = Codec<std::remove_cvref_t<decltype(v.m_cTimeRank)>>::get(r);
        v.m_cTotalRank = Codec<std::remove_cvref_t<decltype(v.m_cTotalRank)>>::get(r);
        v.m_bIsMVP = Codec<std::remove_cvref_t<decltype(v.m_bIsMVP)>>::get(r);
        return v;
    }
};

struct KEBILL_BUY_PRODUCT_REQ {
    std::int32_t              m_iServerGroupID{};  // int
    std::int32_t              m_iChannelingCode{};  // int
    std::u16string            m_wstrUserID{};  // std::wstring
    std::uint32_t             m_uiPublisherUID{};  // unsigned int
    std::int64_t              m_iUserUID{};  // UidType
    std::int64_t              m_iUnitUID{};  // UidType
    std::u16string            m_wstrNickName{};  // std::wstring
    std::string               m_strUserIP{};
    std::vector<KBillBuyInfo> m_vecBillBuyInfo{};  // std::vector< KBillBuyInfo >
    KBuyCharInfo              m_BuyCharInfo{};
    std::int32_t              m_iUseCashType{};  // int
    std::int32_t              m_iUserIP{};  // int
    std::u16string            m_wstrTotalOrderID{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iServerGroupID)>>::put(w, m_iServerGroupID);
        Codec<std::remove_cvref_t<decltype(m_iChannelingCode)>>::put(w, m_iChannelingCode);
        Codec<std::remove_cvref_t<decltype(m_wstrUserID)>>::put(w, m_wstrUserID);
        Codec<std::remove_cvref_t<decltype(m_uiPublisherUID)>>::put(w, m_uiPublisherUID);
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_strUserIP)>>::put(w, m_strUserIP);
        Codec<std::remove_cvref_t<decltype(m_vecBillBuyInfo)>>::put(w, m_vecBillBuyInfo);
        Codec<std::remove_cvref_t<decltype(m_BuyCharInfo)>>::put(w, m_BuyCharInfo);
        Codec<std::remove_cvref_t<decltype(m_iUseCashType)>>::put(w, m_iUseCashType);
        Codec<std::remove_cvref_t<decltype(m_iUserIP)>>::put(w, m_iUserIP);
        Codec<std::remove_cvref_t<decltype(m_wstrTotalOrderID)>>::put(w, m_wstrTotalOrderID);
    }

    static KEBILL_BUY_PRODUCT_REQ read_from(Reader& r) {
        KEBILL_BUY_PRODUCT_REQ v;
        v.m_iServerGroupID = Codec<std::remove_cvref_t<decltype(v.m_iServerGroupID)>>::get(r);
        v.m_iChannelingCode = Codec<std::remove_cvref_t<decltype(v.m_iChannelingCode)>>::get(r);
        v.m_wstrUserID = Codec<std::remove_cvref_t<decltype(v.m_wstrUserID)>>::get(r);
        v.m_uiPublisherUID = Codec<std::remove_cvref_t<decltype(v.m_uiPublisherUID)>>::get(r);
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_strUserIP = Codec<std::remove_cvref_t<decltype(v.m_strUserIP)>>::get(r);
        v.m_vecBillBuyInfo = Codec<std::remove_cvref_t<decltype(v.m_vecBillBuyInfo)>>::get(r);
        v.m_BuyCharInfo = Codec<std::remove_cvref_t<decltype(v.m_BuyCharInfo)>>::get(r);
        v.m_iUseCashType = Codec<std::remove_cvref_t<decltype(v.m_iUseCashType)>>::get(r);
        v.m_iUserIP = Codec<std::remove_cvref_t<decltype(v.m_iUserIP)>>::get(r);
        v.m_wstrTotalOrderID = Codec<std::remove_cvref_t<decltype(v.m_wstrTotalOrderID)>>::get(r);
        return v;
    }
};

struct KEBILL_CHECK_BALANCE_REQ {
    std::int32_t m_iPublisherCashBalance{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iPublisherCashBalance)>>::put(w, m_iPublisherCashBalance);
    }

    static KEBILL_CHECK_BALANCE_REQ read_from(Reader& r) {
        KEBILL_CHECK_BALANCE_REQ v;
        v.m_iPublisherCashBalance = Codec<std::remove_cvref_t<decltype(v.m_iPublisherCashBalance)>>::get(r);
        return v;
    }
};

struct KEBILL_CHECK_PRODUCT_INFO_ACK {
    std::map<std::int32_t, std::int32_t>          m_mapReleaseTick{};  // std::map<int, int>
    std::map<std::int32_t, KBillProductInfo>      m_mapBillProductInfo{};  // std::map< int, KBillProductInfo >
    std::map<std::int32_t, KBillProductAttribute> m_mapBillProductAttribute{};  // std::map< int, KBillProductAttribute >
    std::map<std::int32_t, KBillPackageInfo>      m_mapBillPackageInfo{};  // std::map< int, KBillPackageInfo >
    std::vector<std::u16string>                   m_vecStartEvent{};  // std::vector<std::wstring>
    std::vector<std::u16string>                   m_vecEndEvent{};  // std::vector<std::wstring>

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_mapReleaseTick)>>::put(w, m_mapReleaseTick);
        Codec<std::remove_cvref_t<decltype(m_mapBillProductInfo)>>::put(w, m_mapBillProductInfo);
        Codec<std::remove_cvref_t<decltype(m_mapBillProductAttribute)>>::put(w, m_mapBillProductAttribute);
        Codec<std::remove_cvref_t<decltype(m_mapBillPackageInfo)>>::put(w, m_mapBillPackageInfo);
        Codec<std::remove_cvref_t<decltype(m_vecStartEvent)>>::put(w, m_vecStartEvent);
        Codec<std::remove_cvref_t<decltype(m_vecEndEvent)>>::put(w, m_vecEndEvent);
    }

    static KEBILL_CHECK_PRODUCT_INFO_ACK read_from(Reader& r) {
        KEBILL_CHECK_PRODUCT_INFO_ACK v;
        v.m_mapReleaseTick = Codec<std::remove_cvref_t<decltype(v.m_mapReleaseTick)>>::get(r);
        v.m_mapBillProductInfo = Codec<std::remove_cvref_t<decltype(v.m_mapBillProductInfo)>>::get(r);
        v.m_mapBillProductAttribute = Codec<std::remove_cvref_t<decltype(v.m_mapBillProductAttribute)>>::get(r);
        v.m_mapBillPackageInfo = Codec<std::remove_cvref_t<decltype(v.m_mapBillPackageInfo)>>::get(r);
        v.m_vecStartEvent = Codec<std::remove_cvref_t<decltype(v.m_vecStartEvent)>>::get(r);
        v.m_vecEndEvent = Codec<std::remove_cvref_t<decltype(v.m_vecEndEvent)>>::get(r);
        return v;
    }
};

struct KEBILL_CHECK_PRODUCT_INFO_REQ {
    std::map<std::int32_t, std::int32_t> m_mapReleaseTick{};  // std::map<int, int>

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_mapReleaseTick)>>::put(w, m_mapReleaseTick);
    }

    static KEBILL_CHECK_PRODUCT_INFO_REQ read_from(Reader& r) {
        KEBILL_CHECK_PRODUCT_INFO_REQ v;
        v.m_mapReleaseTick = Codec<std::remove_cvref_t<decltype(v.m_mapReleaseTick)>>::get(r);
        return v;
    }
};

struct KEBILL_EXCHANGE_CASH_REQ {
    std::int64_t   m_iGSUID{};  // UidType
    std::int64_t   m_iUserUID{};  // UidType
    std::u16string m_wstrTransaction{};  // std::wstring
    std::int32_t   m_iRet{};  // int
    std::int32_t   m_iPoint{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iGSUID)>>::put(w, m_iGSUID);
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
        Codec<std::remove_cvref_t<decltype(m_wstrTransaction)>>::put(w, m_wstrTransaction);
        Codec<std::remove_cvref_t<decltype(m_iRet)>>::put(w, m_iRet);
        Codec<std::remove_cvref_t<decltype(m_iPoint)>>::put(w, m_iPoint);
    }

    static KEBILL_EXCHANGE_CASH_REQ read_from(Reader& r) {
        KEBILL_EXCHANGE_CASH_REQ v;
        v.m_iGSUID = Codec<std::remove_cvref_t<decltype(v.m_iGSUID)>>::get(r);
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        v.m_wstrTransaction = Codec<std::remove_cvref_t<decltype(v.m_wstrTransaction)>>::get(r);
        v.m_iRet = Codec<std::remove_cvref_t<decltype(v.m_iRet)>>::get(r);
        v.m_iPoint = Codec<std::remove_cvref_t<decltype(v.m_iPoint)>>::get(r);
        return v;
    }
};

struct KEBILL_GIFT_ITEM_REQ {
    std::int32_t              m_iServerGroupID{};  // int
    std::int32_t              m_iChannelingCode{};  // int
    std::u16string            m_wstrSenderUserID{};  // std::wstring
    std::uint32_t             m_uiSenderPublisherUID{};  // unsigned int
    std::u16string            m_wstrSenderNickName{};  // std::wstring
    std::u16string            m_wstrReceiverUserID{};  // std::wstring
    std::u16string            m_wstrReceiverNickName{};  // std::wstring
    std::vector<KBillBuyInfo> m_vecBillBuyInfo{};  // std::vector< KBillBuyInfo >
    std::int64_t              m_iSenderUserUID{};  // UidType
    std::int64_t              m_iSenderUnitUID{};  // UidType
    std::int64_t              m_iReceiverUserUID{};  // UidType
    std::int64_t              m_iReceiverUnitUID{};  // UidType
    std::u16string            m_wstrPresentMessage{};  // std::wstring
    KBuyCharInfo              m_BuyCharInfo{};
    KBuyCharInfo              m_RecvCharInfo{};
    std::int32_t              m_iUseCashType{};  // int
    std::u16string            m_wstrUserIP{};  // std::wstring
    std::u16string            m_wstrTotalOrderID{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iServerGroupID)>>::put(w, m_iServerGroupID);
        Codec<std::remove_cvref_t<decltype(m_iChannelingCode)>>::put(w, m_iChannelingCode);
        Codec<std::remove_cvref_t<decltype(m_wstrSenderUserID)>>::put(w, m_wstrSenderUserID);
        Codec<std::remove_cvref_t<decltype(m_uiSenderPublisherUID)>>::put(w, m_uiSenderPublisherUID);
        Codec<std::remove_cvref_t<decltype(m_wstrSenderNickName)>>::put(w, m_wstrSenderNickName);
        Codec<std::remove_cvref_t<decltype(m_wstrReceiverUserID)>>::put(w, m_wstrReceiverUserID);
        Codec<std::remove_cvref_t<decltype(m_wstrReceiverNickName)>>::put(w, m_wstrReceiverNickName);
        Codec<std::remove_cvref_t<decltype(m_vecBillBuyInfo)>>::put(w, m_vecBillBuyInfo);
        Codec<std::remove_cvref_t<decltype(m_iSenderUserUID)>>::put(w, m_iSenderUserUID);
        Codec<std::remove_cvref_t<decltype(m_iSenderUnitUID)>>::put(w, m_iSenderUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iReceiverUserUID)>>::put(w, m_iReceiverUserUID);
        Codec<std::remove_cvref_t<decltype(m_iReceiverUnitUID)>>::put(w, m_iReceiverUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrPresentMessage)>>::put(w, m_wstrPresentMessage);
        Codec<std::remove_cvref_t<decltype(m_BuyCharInfo)>>::put(w, m_BuyCharInfo);
        Codec<std::remove_cvref_t<decltype(m_RecvCharInfo)>>::put(w, m_RecvCharInfo);
        Codec<std::remove_cvref_t<decltype(m_iUseCashType)>>::put(w, m_iUseCashType);
        Codec<std::remove_cvref_t<decltype(m_wstrUserIP)>>::put(w, m_wstrUserIP);
        Codec<std::remove_cvref_t<decltype(m_wstrTotalOrderID)>>::put(w, m_wstrTotalOrderID);
    }

    static KEBILL_GIFT_ITEM_REQ read_from(Reader& r) {
        KEBILL_GIFT_ITEM_REQ v;
        v.m_iServerGroupID = Codec<std::remove_cvref_t<decltype(v.m_iServerGroupID)>>::get(r);
        v.m_iChannelingCode = Codec<std::remove_cvref_t<decltype(v.m_iChannelingCode)>>::get(r);
        v.m_wstrSenderUserID = Codec<std::remove_cvref_t<decltype(v.m_wstrSenderUserID)>>::get(r);
        v.m_uiSenderPublisherUID = Codec<std::remove_cvref_t<decltype(v.m_uiSenderPublisherUID)>>::get(r);
        v.m_wstrSenderNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrSenderNickName)>>::get(r);
        v.m_wstrReceiverUserID = Codec<std::remove_cvref_t<decltype(v.m_wstrReceiverUserID)>>::get(r);
        v.m_wstrReceiverNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrReceiverNickName)>>::get(r);
        v.m_vecBillBuyInfo = Codec<std::remove_cvref_t<decltype(v.m_vecBillBuyInfo)>>::get(r);
        v.m_iSenderUserUID = Codec<std::remove_cvref_t<decltype(v.m_iSenderUserUID)>>::get(r);
        v.m_iSenderUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iSenderUnitUID)>>::get(r);
        v.m_iReceiverUserUID = Codec<std::remove_cvref_t<decltype(v.m_iReceiverUserUID)>>::get(r);
        v.m_iReceiverUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iReceiverUnitUID)>>::get(r);
        v.m_wstrPresentMessage = Codec<std::remove_cvref_t<decltype(v.m_wstrPresentMessage)>>::get(r);
        v.m_BuyCharInfo = Codec<std::remove_cvref_t<decltype(v.m_BuyCharInfo)>>::get(r);
        v.m_RecvCharInfo = Codec<std::remove_cvref_t<decltype(v.m_RecvCharInfo)>>::get(r);
        v.m_iUseCashType = Codec<std::remove_cvref_t<decltype(v.m_iUseCashType)>>::get(r);
        v.m_wstrUserIP = Codec<std::remove_cvref_t<decltype(v.m_wstrUserIP)>>::get(r);
        v.m_wstrTotalOrderID = Codec<std::remove_cvref_t<decltype(v.m_wstrTotalOrderID)>>::get(r);
        return v;
    }
};

struct KEBILL_PICK_UP_ACK {
    std::int32_t                         m_iOK{};  // int
    std::int32_t                         m_iProductNo{};  // int
    std::map<std::int32_t, std::int32_t> m_mapSocketForCash{};  // std::map< int, int >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iProductNo)>>::put(w, m_iProductNo);
        Codec<std::remove_cvref_t<decltype(m_mapSocketForCash)>>::put(w, m_mapSocketForCash);
    }

    static KEBILL_PICK_UP_ACK read_from(Reader& r) {
        KEBILL_PICK_UP_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iProductNo = Codec<std::remove_cvref_t<decltype(v.m_iProductNo)>>::get(r);
        v.m_mapSocketForCash = Codec<std::remove_cvref_t<decltype(v.m_mapSocketForCash)>>::get(r);
        return v;
    }
};

struct KEBILL_PREPARE_BUY_PRODUCT_ACK {
    std::int32_t           m_iOK{};  // int
    KEBILL_BUY_PRODUCT_REQ m_kEBILL_BUY_PRODUCT_REQ{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_kEBILL_BUY_PRODUCT_REQ)>>::put(w, m_kEBILL_BUY_PRODUCT_REQ);
    }

    static KEBILL_PREPARE_BUY_PRODUCT_ACK read_from(Reader& r) {
        KEBILL_PREPARE_BUY_PRODUCT_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_kEBILL_BUY_PRODUCT_REQ = Codec<std::remove_cvref_t<decltype(v.m_kEBILL_BUY_PRODUCT_REQ)>>::get(r);
        return v;
    }
};

struct KEBILL_PREPARE_GIFT_ITEM_ACK {
    std::int32_t         m_iOK{};  // int
    KEBILL_GIFT_ITEM_REQ m_kEBILL_GIFT_ITEM_REQ{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_kEBILL_GIFT_ITEM_REQ)>>::put(w, m_kEBILL_GIFT_ITEM_REQ);
    }

    static KEBILL_PREPARE_GIFT_ITEM_ACK read_from(Reader& r) {
        KEBILL_PREPARE_GIFT_ITEM_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_kEBILL_GIFT_ITEM_REQ = Codec<std::remove_cvref_t<decltype(v.m_kEBILL_GIFT_ITEM_REQ)>>::get(r);
        return v;
    }
};

struct KEBILL_USE_COUPON_ACK {
    std::int32_t                     m_iOK{};  // int
    std::vector<KBillCouponItemInfo> m_vecBillCouponItemInfo{};  // std::vector< KBillCouponItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_vecBillCouponItemInfo)>>::put(w, m_vecBillCouponItemInfo);
    }

    static KEBILL_USE_COUPON_ACK read_from(Reader& r) {
        KEBILL_USE_COUPON_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_vecBillCouponItemInfo = Codec<std::remove_cvref_t<decltype(v.m_vecBillCouponItemInfo)>>::get(r);
        return v;
    }
};

struct KEBILL_USE_COUPON_RESULT_ACK {
    std::int32_t                     m_iOK{};  // int
    std::int32_t                     m_iTotalCash{};  // int
    std::vector<KBillCouponItemInfo> m_vecBillCouponItemInfo{};  // std::vector< KBillCouponItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iTotalCash)>>::put(w, m_iTotalCash);
        Codec<std::remove_cvref_t<decltype(m_vecBillCouponItemInfo)>>::put(w, m_vecBillCouponItemInfo);
    }

    static KEBILL_USE_COUPON_RESULT_ACK read_from(Reader& r) {
        KEBILL_USE_COUPON_RESULT_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iTotalCash = Codec<std::remove_cvref_t<decltype(v.m_iTotalCash)>>::get(r);
        v.m_vecBillCouponItemInfo = Codec<std::remove_cvref_t<decltype(v.m_vecBillCouponItemInfo)>>::get(r);
        return v;
    }
};

struct KEBILL_USE_COUPON_RESULT_REQ {
    std::int64_t   m_iGSUID{};  // UidType
    std::int64_t   m_iUserUID{};  // UidType
    std::u16string m_wstrTransaction{};  // std::wstring
    std::int32_t   m_iRet{};  // int
    std::int32_t   m_iChargeItem{};  // int
    std::int32_t   m_iChargeItemCnt{};  // int
    std::int32_t   m_iChargeCash{};  // int
    std::int32_t   m_iChargeBonus{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iGSUID)>>::put(w, m_iGSUID);
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
        Codec<std::remove_cvref_t<decltype(m_wstrTransaction)>>::put(w, m_wstrTransaction);
        Codec<std::remove_cvref_t<decltype(m_iRet)>>::put(w, m_iRet);
        Codec<std::remove_cvref_t<decltype(m_iChargeItem)>>::put(w, m_iChargeItem);
        Codec<std::remove_cvref_t<decltype(m_iChargeItemCnt)>>::put(w, m_iChargeItemCnt);
        Codec<std::remove_cvref_t<decltype(m_iChargeCash)>>::put(w, m_iChargeCash);
        Codec<std::remove_cvref_t<decltype(m_iChargeBonus)>>::put(w, m_iChargeBonus);
    }

    static KEBILL_USE_COUPON_RESULT_REQ read_from(Reader& r) {
        KEBILL_USE_COUPON_RESULT_REQ v;
        v.m_iGSUID = Codec<std::remove_cvref_t<decltype(v.m_iGSUID)>>::get(r);
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        v.m_wstrTransaction = Codec<std::remove_cvref_t<decltype(v.m_wstrTransaction)>>::get(r);
        v.m_iRet = Codec<std::remove_cvref_t<decltype(v.m_iRet)>>::get(r);
        v.m_iChargeItem = Codec<std::remove_cvref_t<decltype(v.m_iChargeItem)>>::get(r);
        v.m_iChargeItemCnt = Codec<std::remove_cvref_t<decltype(v.m_iChargeItemCnt)>>::get(r);
        v.m_iChargeCash = Codec<std::remove_cvref_t<decltype(v.m_iChargeCash)>>::get(r);
        v.m_iChargeBonus = Codec<std::remove_cvref_t<decltype(v.m_iChargeBonus)>>::get(r);
        return v;
    }
};

struct KECH_GET_CHANNEL_LIST_NOT {
    std::int32_t                              m_iServerGroupID{};  // int
    std::map<std::int32_t, KChannelInfo>      m_mapChannelList{};  // std::map< int, KChannelInfo >
    std::map<std::int32_t, KChannelBonusInfo> m_mapChannelBonusList{};  // std::map< int, KChannelBonusInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iServerGroupID)>>::put(w, m_iServerGroupID);
        Codec<std::remove_cvref_t<decltype(m_mapChannelList)>>::put(w, m_mapChannelList);
        Codec<std::remove_cvref_t<decltype(m_mapChannelBonusList)>>::put(w, m_mapChannelBonusList);
    }

    static KECH_GET_CHANNEL_LIST_NOT read_from(Reader& r) {
        KECH_GET_CHANNEL_LIST_NOT v;
        v.m_iServerGroupID = Codec<std::remove_cvref_t<decltype(v.m_iServerGroupID)>>::get(r);
        v.m_mapChannelList = Codec<std::remove_cvref_t<decltype(v.m_mapChannelList)>>::get(r);
        v.m_mapChannelBonusList = Codec<std::remove_cvref_t<decltype(v.m_mapChannelBonusList)>>::get(r);
        return v;
    }
};

struct KECH_SECURITY_AUTH_ACK {
    std::int32_t m_iOK{};  // int
    std::string  m_strSecondSecurityURL{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_strSecondSecurityURL)>>::put(w, m_strSecondSecurityURL);
    }

    static KECH_SECURITY_AUTH_ACK read_from(Reader& r) {
        KECH_SECURITY_AUTH_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_strSecondSecurityURL = Codec<std::remove_cvref_t<decltype(v.m_strSecondSecurityURL)>>::get(r);
        return v;
    }
};

struct KECH_SECURITY_AUTH_REQ {
    std::int32_t   m_iSecurityAuthType{};  // int
    std::int64_t   m_iUserUID{};  // UidType
    std::u16string m_wstrUserID{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iSecurityAuthType)>>::put(w, m_iSecurityAuthType);
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
        Codec<std::remove_cvref_t<decltype(m_wstrUserID)>>::put(w, m_wstrUserID);
    }

    static KECH_SECURITY_AUTH_REQ read_from(Reader& r) {
        KECH_SECURITY_AUTH_REQ v;
        v.m_iSecurityAuthType = Codec<std::remove_cvref_t<decltype(v.m_iSecurityAuthType)>>::get(r);
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        v.m_wstrUserID = Codec<std::remove_cvref_t<decltype(v.m_wstrUserID)>>::get(r);
        return v;
    }
};

struct KECH_SECURITY_INFO_NOT {
    std::int32_t   m_iSecurityInfoType{};  // int
    std::uint32_t  m_uiMatrixKey{};  // unsigned int
    std::u16string m_wstrCoordinate{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iSecurityInfoType)>>::put(w, m_iSecurityInfoType);
        Codec<std::remove_cvref_t<decltype(m_uiMatrixKey)>>::put(w, m_uiMatrixKey);
        Codec<std::remove_cvref_t<decltype(m_wstrCoordinate)>>::put(w, m_wstrCoordinate);
    }

    static KECH_SECURITY_INFO_NOT read_from(Reader& r) {
        KECH_SECURITY_INFO_NOT v;
        v.m_iSecurityInfoType = Codec<std::remove_cvref_t<decltype(v.m_iSecurityInfoType)>>::get(r);
        v.m_uiMatrixKey = Codec<std::remove_cvref_t<decltype(v.m_uiMatrixKey)>>::get(r);
        v.m_wstrCoordinate = Codec<std::remove_cvref_t<decltype(v.m_wstrCoordinate)>>::get(r);
        return v;
    }
};

struct KECH_VERIFY_ACCOUNT_ACK {
    std::int32_t   m_iOK{};  // int
    std::int64_t   m_iUserUID{};  // UidType
    std::u16string m_wstrPassport{};  // std::wstring
    std::int32_t   m_iChannelingCode{};  // int
    std::u16string m_wstrCurrentTime{};  // std::wstring
    std::u16string m_wstrUserID{};  // std::wstring
    std::u16string m_wstrPurchaseTok{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
        Codec<std::remove_cvref_t<decltype(m_wstrPassport)>>::put(w, m_wstrPassport);
        Codec<std::remove_cvref_t<decltype(m_iChannelingCode)>>::put(w, m_iChannelingCode);
        Codec<std::remove_cvref_t<decltype(m_wstrCurrentTime)>>::put(w, m_wstrCurrentTime);
        Codec<std::remove_cvref_t<decltype(m_wstrUserID)>>::put(w, m_wstrUserID);
        Codec<std::remove_cvref_t<decltype(m_wstrPurchaseTok)>>::put(w, m_wstrPurchaseTok);
    }

    static KECH_VERIFY_ACCOUNT_ACK read_from(Reader& r) {
        KECH_VERIFY_ACCOUNT_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        v.m_wstrPassport = Codec<std::remove_cvref_t<decltype(v.m_wstrPassport)>>::get(r);
        v.m_iChannelingCode = Codec<std::remove_cvref_t<decltype(v.m_iChannelingCode)>>::get(r);
        v.m_wstrCurrentTime = Codec<std::remove_cvref_t<decltype(v.m_wstrCurrentTime)>>::get(r);
        v.m_wstrUserID = Codec<std::remove_cvref_t<decltype(v.m_wstrUserID)>>::get(r);
        v.m_wstrPurchaseTok = Codec<std::remove_cvref_t<decltype(v.m_wstrPurchaseTok)>>::get(r);
        return v;
    }
};

struct KECH_VERIFY_ACCOUNT_REQ {
    bool           m_bLogin{};
    std::u16string m_wstrID{};  // std::wstring
    std::u16string m_wstrPassword{};  // std::wstring
    std::int32_t   m_iChannelingCode{};  // int
    std::string    m_strMachineID{};
    bool           m_bIDVerified{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bLogin)>>::put(w, m_bLogin);
        Codec<std::remove_cvref_t<decltype(m_wstrID)>>::put(w, m_wstrID);
        Codec<std::remove_cvref_t<decltype(m_wstrPassword)>>::put(w, m_wstrPassword);
        Codec<std::remove_cvref_t<decltype(m_iChannelingCode)>>::put(w, m_iChannelingCode);
        Codec<std::remove_cvref_t<decltype(m_strMachineID)>>::put(w, m_strMachineID);
        Codec<std::remove_cvref_t<decltype(m_bIDVerified)>>::put(w, m_bIDVerified);
    }

    static KECH_VERIFY_ACCOUNT_REQ read_from(Reader& r) {
        KECH_VERIFY_ACCOUNT_REQ v;
        v.m_bLogin = Codec<std::remove_cvref_t<decltype(v.m_bLogin)>>::get(r);
        v.m_wstrID = Codec<std::remove_cvref_t<decltype(v.m_wstrID)>>::get(r);
        v.m_wstrPassword = Codec<std::remove_cvref_t<decltype(v.m_wstrPassword)>>::get(r);
        v.m_iChannelingCode = Codec<std::remove_cvref_t<decltype(v.m_iChannelingCode)>>::get(r);
        v.m_strMachineID = Codec<std::remove_cvref_t<decltype(v.m_strMachineID)>>::get(r);
        v.m_bIDVerified = Codec<std::remove_cvref_t<decltype(v.m_bIDVerified)>>::get(r);
        return v;
    }
};

struct KECH_WAIT_QUEUE_MESSAGE_NOT {
    std::int32_t m_iWaitUserCount{};  // int
    bool         m_bPassed{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iWaitUserCount)>>::put(w, m_iWaitUserCount);
        Codec<std::remove_cvref_t<decltype(m_bPassed)>>::put(w, m_bPassed);
    }

    static KECH_WAIT_QUEUE_MESSAGE_NOT read_from(Reader& r) {
        KECH_WAIT_QUEUE_MESSAGE_NOT v;
        v.m_iWaitUserCount = Codec<std::remove_cvref_t<decltype(v.m_iWaitUserCount)>>::get(r);
        v.m_bPassed = Codec<std::remove_cvref_t<decltype(v.m_bPassed)>>::get(r);
        return v;
    }
};

struct KECN_ADD_COMMON_FLAG_NOT {
    std::uint32_t dwFlag{};  // DWORD

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(dwFlag)>>::put(w, dwFlag);
    }

    static KECN_ADD_COMMON_FLAG_NOT read_from(Reader& r) {
        KECN_ADD_COMMON_FLAG_NOT v;
        v.dwFlag = Codec<std::remove_cvref_t<decltype(v.dwFlag)>>::get(r);
        return v;
    }
};

struct KECN_CHECK_CHANNEL_CHANGE_ACK {
    std::int32_t m_iOK{};  // int
    std::int64_t m_iSourGSUID{};  // UidType
    std::int64_t m_iCharUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iSourGSUID)>>::put(w, m_iSourGSUID);
        Codec<std::remove_cvref_t<decltype(m_iCharUID)>>::put(w, m_iCharUID);
    }

    static KECN_CHECK_CHANNEL_CHANGE_ACK read_from(Reader& r) {
        KECN_CHECK_CHANNEL_CHANGE_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iSourGSUID = Codec<std::remove_cvref_t<decltype(v.m_iSourGSUID)>>::get(r);
        v.m_iCharUID = Codec<std::remove_cvref_t<decltype(v.m_iCharUID)>>::get(r);
        return v;
    }
};

struct KECN_DEL_COMMON_FLAG_NOT {
    std::uint32_t dwFlag{};  // DWORD

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(dwFlag)>>::put(w, dwFlag);
    }

    static KECN_DEL_COMMON_FLAG_NOT read_from(Reader& r) {
        KECN_DEL_COMMON_FLAG_NOT v;
        v.dwFlag = Codec<std::remove_cvref_t<decltype(v.dwFlag)>>::get(r);
        return v;
    }
};

struct KECN_NEW_PVP_USER_INFO_NOT {
    std::int64_t       m_nGSUID{};  // UidType
    KCommunityUserInfo m_kUserInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_nGSUID)>>::put(w, m_nGSUID);
        Codec<std::remove_cvref_t<decltype(m_kUserInfo)>>::put(w, m_kUserInfo);
    }

    static KECN_NEW_PVP_USER_INFO_NOT read_from(Reader& r) {
        KECN_NEW_PVP_USER_INFO_NOT v;
        v.m_nGSUID = Codec<std::remove_cvref_t<decltype(v.m_nGSUID)>>::get(r);
        v.m_kUserInfo = Codec<std::remove_cvref_t<decltype(v.m_kUserInfo)>>::get(r);
        return v;
    }
};

struct KECN_PARTY_BUFF_UPDATE_NOT {
    std::map<std::int32_t, KBuffInfo> m_mapActivateBuffList{};  // std::map<int, KBuffInfo>
    std::set<std::int32_t>            m_setDeactivateBuffList{};  // std::set<int>

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_mapActivateBuffList)>>::put(w, m_mapActivateBuffList);
        Codec<std::remove_cvref_t<decltype(m_setDeactivateBuffList)>>::put(w, m_setDeactivateBuffList);
    }

    static KECN_PARTY_BUFF_UPDATE_NOT read_from(Reader& r) {
        KECN_PARTY_BUFF_UPDATE_NOT v;
        v.m_mapActivateBuffList = Codec<std::remove_cvref_t<decltype(v.m_mapActivateBuffList)>>::get(r);
        v.m_setDeactivateBuffList = Codec<std::remove_cvref_t<decltype(v.m_setDeactivateBuffList)>>::get(r);
        return v;
    }
};

struct KEGASH_ACCOUNTING_REQ {
    std::int64_t m_iActorUID{};  // UidType
    std::string  m_strServiceCode{};
    std::string  m_strServiceRegion{};
    std::string  m_strServiceAccountID{};
    std::string  m_strType{};
    std::string  m_strLoginTime{};
    std::string  m_strLogoutTime{};
    std::string  m_strIPAddress{};
    std::string  m_strPacketID{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iActorUID)>>::put(w, m_iActorUID);
        Codec<std::remove_cvref_t<decltype(m_strServiceCode)>>::put(w, m_strServiceCode);
        Codec<std::remove_cvref_t<decltype(m_strServiceRegion)>>::put(w, m_strServiceRegion);
        Codec<std::remove_cvref_t<decltype(m_strServiceAccountID)>>::put(w, m_strServiceAccountID);
        Codec<std::remove_cvref_t<decltype(m_strType)>>::put(w, m_strType);
        Codec<std::remove_cvref_t<decltype(m_strLoginTime)>>::put(w, m_strLoginTime);
        Codec<std::remove_cvref_t<decltype(m_strLogoutTime)>>::put(w, m_strLogoutTime);
        Codec<std::remove_cvref_t<decltype(m_strIPAddress)>>::put(w, m_strIPAddress);
        Codec<std::remove_cvref_t<decltype(m_strPacketID)>>::put(w, m_strPacketID);
    }

    static KEGASH_ACCOUNTING_REQ read_from(Reader& r) {
        KEGASH_ACCOUNTING_REQ v;
        v.m_iActorUID = Codec<std::remove_cvref_t<decltype(v.m_iActorUID)>>::get(r);
        v.m_strServiceCode = Codec<std::remove_cvref_t<decltype(v.m_strServiceCode)>>::get(r);
        v.m_strServiceRegion = Codec<std::remove_cvref_t<decltype(v.m_strServiceRegion)>>::get(r);
        v.m_strServiceAccountID = Codec<std::remove_cvref_t<decltype(v.m_strServiceAccountID)>>::get(r);
        v.m_strType = Codec<std::remove_cvref_t<decltype(v.m_strType)>>::get(r);
        v.m_strLoginTime = Codec<std::remove_cvref_t<decltype(v.m_strLoginTime)>>::get(r);
        v.m_strLogoutTime = Codec<std::remove_cvref_t<decltype(v.m_strLogoutTime)>>::get(r);
        v.m_strIPAddress = Codec<std::remove_cvref_t<decltype(v.m_strIPAddress)>>::get(r);
        v.m_strPacketID = Codec<std::remove_cvref_t<decltype(v.m_strPacketID)>>::get(r);
        return v;
    }
};

struct KEGASH_AUTHENTICATION_REQ {
    std::int64_t m_iActorUID{};  // UidType
    std::string  m_strServiceCode{};
    std::string  m_strServiceRegion{};
    std::string  m_strServiceAccountID{};
    std::string  m_strServicePassword{};
    std::string  m_strLockFlag{};
    std::string  m_strIPAddress{};
    std::string  m_strPacketID{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iActorUID)>>::put(w, m_iActorUID);
        Codec<std::remove_cvref_t<decltype(m_strServiceCode)>>::put(w, m_strServiceCode);
        Codec<std::remove_cvref_t<decltype(m_strServiceRegion)>>::put(w, m_strServiceRegion);
        Codec<std::remove_cvref_t<decltype(m_strServiceAccountID)>>::put(w, m_strServiceAccountID);
        Codec<std::remove_cvref_t<decltype(m_strServicePassword)>>::put(w, m_strServicePassword);
        Codec<std::remove_cvref_t<decltype(m_strLockFlag)>>::put(w, m_strLockFlag);
        Codec<std::remove_cvref_t<decltype(m_strIPAddress)>>::put(w, m_strIPAddress);
        Codec<std::remove_cvref_t<decltype(m_strPacketID)>>::put(w, m_strPacketID);
    }

    static KEGASH_AUTHENTICATION_REQ read_from(Reader& r) {
        KEGASH_AUTHENTICATION_REQ v;
        v.m_iActorUID = Codec<std::remove_cvref_t<decltype(v.m_iActorUID)>>::get(r);
        v.m_strServiceCode = Codec<std::remove_cvref_t<decltype(v.m_strServiceCode)>>::get(r);
        v.m_strServiceRegion = Codec<std::remove_cvref_t<decltype(v.m_strServiceRegion)>>::get(r);
        v.m_strServiceAccountID = Codec<std::remove_cvref_t<decltype(v.m_strServiceAccountID)>>::get(r);
        v.m_strServicePassword = Codec<std::remove_cvref_t<decltype(v.m_strServicePassword)>>::get(r);
        v.m_strLockFlag = Codec<std::remove_cvref_t<decltype(v.m_strLockFlag)>>::get(r);
        v.m_strIPAddress = Codec<std::remove_cvref_t<decltype(v.m_strIPAddress)>>::get(r);
        v.m_strPacketID = Codec<std::remove_cvref_t<decltype(v.m_strPacketID)>>::get(r);
        return v;
    }
};

struct KEGASH_AUTHORIZATION_REQ {
    std::int64_t m_iActorUID{};  // UidType
    std::string  m_strServiceCode{};
    std::string  m_strServiceRegion{};
    std::string  m_strServiceAccountID{};
    std::string  m_strServicePassword{};
    std::string  m_strAuthenticationFlag{};
    std::string  m_strType{};
    std::string  m_strDuplicateCheck{};
    std::string  m_strIPAddress{};
    std::string  m_strPacketID{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iActorUID)>>::put(w, m_iActorUID);
        Codec<std::remove_cvref_t<decltype(m_strServiceCode)>>::put(w, m_strServiceCode);
        Codec<std::remove_cvref_t<decltype(m_strServiceRegion)>>::put(w, m_strServiceRegion);
        Codec<std::remove_cvref_t<decltype(m_strServiceAccountID)>>::put(w, m_strServiceAccountID);
        Codec<std::remove_cvref_t<decltype(m_strServicePassword)>>::put(w, m_strServicePassword);
        Codec<std::remove_cvref_t<decltype(m_strAuthenticationFlag)>>::put(w, m_strAuthenticationFlag);
        Codec<std::remove_cvref_t<decltype(m_strType)>>::put(w, m_strType);
        Codec<std::remove_cvref_t<decltype(m_strDuplicateCheck)>>::put(w, m_strDuplicateCheck);
        Codec<std::remove_cvref_t<decltype(m_strIPAddress)>>::put(w, m_strIPAddress);
        Codec<std::remove_cvref_t<decltype(m_strPacketID)>>::put(w, m_strPacketID);
    }

    static KEGASH_AUTHORIZATION_REQ read_from(Reader& r) {
        KEGASH_AUTHORIZATION_REQ v;
        v.m_iActorUID = Codec<std::remove_cvref_t<decltype(v.m_iActorUID)>>::get(r);
        v.m_strServiceCode = Codec<std::remove_cvref_t<decltype(v.m_strServiceCode)>>::get(r);
        v.m_strServiceRegion = Codec<std::remove_cvref_t<decltype(v.m_strServiceRegion)>>::get(r);
        v.m_strServiceAccountID = Codec<std::remove_cvref_t<decltype(v.m_strServiceAccountID)>>::get(r);
        v.m_strServicePassword = Codec<std::remove_cvref_t<decltype(v.m_strServicePassword)>>::get(r);
        v.m_strAuthenticationFlag = Codec<std::remove_cvref_t<decltype(v.m_strAuthenticationFlag)>>::get(r);
        v.m_strType = Codec<std::remove_cvref_t<decltype(v.m_strType)>>::get(r);
        v.m_strDuplicateCheck = Codec<std::remove_cvref_t<decltype(v.m_strDuplicateCheck)>>::get(r);
        v.m_strIPAddress = Codec<std::remove_cvref_t<decltype(v.m_strIPAddress)>>::get(r);
        v.m_strPacketID = Codec<std::remove_cvref_t<decltype(v.m_strPacketID)>>::get(r);
        return v;
    }
};

struct KEGASH_DECREASE_GASHPOINT_REQ {
    std::int64_t m_iActorUID{};  // UidType
    std::string  m_strServiceCode{};
    std::string  m_strServiceRegion{};
    std::string  m_strServiceAccountID{};
    std::string  m_strType{};
    std::string  m_strShopTime{};
    std::string  m_strPoints{};
    std::string  m_strIPAddress{};
    std::string  m_strMemo{};
    std::string  m_strPacketID{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iActorUID)>>::put(w, m_iActorUID);
        Codec<std::remove_cvref_t<decltype(m_strServiceCode)>>::put(w, m_strServiceCode);
        Codec<std::remove_cvref_t<decltype(m_strServiceRegion)>>::put(w, m_strServiceRegion);
        Codec<std::remove_cvref_t<decltype(m_strServiceAccountID)>>::put(w, m_strServiceAccountID);
        Codec<std::remove_cvref_t<decltype(m_strType)>>::put(w, m_strType);
        Codec<std::remove_cvref_t<decltype(m_strShopTime)>>::put(w, m_strShopTime);
        Codec<std::remove_cvref_t<decltype(m_strPoints)>>::put(w, m_strPoints);
        Codec<std::remove_cvref_t<decltype(m_strIPAddress)>>::put(w, m_strIPAddress);
        Codec<std::remove_cvref_t<decltype(m_strMemo)>>::put(w, m_strMemo);
        Codec<std::remove_cvref_t<decltype(m_strPacketID)>>::put(w, m_strPacketID);
    }

    static KEGASH_DECREASE_GASHPOINT_REQ read_from(Reader& r) {
        KEGASH_DECREASE_GASHPOINT_REQ v;
        v.m_iActorUID = Codec<std::remove_cvref_t<decltype(v.m_iActorUID)>>::get(r);
        v.m_strServiceCode = Codec<std::remove_cvref_t<decltype(v.m_strServiceCode)>>::get(r);
        v.m_strServiceRegion = Codec<std::remove_cvref_t<decltype(v.m_strServiceRegion)>>::get(r);
        v.m_strServiceAccountID = Codec<std::remove_cvref_t<decltype(v.m_strServiceAccountID)>>::get(r);
        v.m_strType = Codec<std::remove_cvref_t<decltype(v.m_strType)>>::get(r);
        v.m_strShopTime = Codec<std::remove_cvref_t<decltype(v.m_strShopTime)>>::get(r);
        v.m_strPoints = Codec<std::remove_cvref_t<decltype(v.m_strPoints)>>::get(r);
        v.m_strIPAddress = Codec<std::remove_cvref_t<decltype(v.m_strIPAddress)>>::get(r);
        v.m_strMemo = Codec<std::remove_cvref_t<decltype(v.m_strMemo)>>::get(r);
        v.m_strPacketID = Codec<std::remove_cvref_t<decltype(v.m_strPacketID)>>::get(r);
        return v;
    }
};

struct KEGASH_GET_GASHPOINT_REQ {
    std::int64_t m_iActorUID{};  // UidType
    std::string  m_strServiceCode{};
    std::string  m_strServiceRegion{};
    std::string  m_strServiceAccountID{};
    std::string  m_strServicePassword{};
    std::string  m_strAuthenticationFlag{};
    std::string  m_strType{};
    std::string  m_strDuplicateCheck{};
    std::string  m_strIPAddress{};
    std::string  m_strPacketID{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iActorUID)>>::put(w, m_iActorUID);
        Codec<std::remove_cvref_t<decltype(m_strServiceCode)>>::put(w, m_strServiceCode);
        Codec<std::remove_cvref_t<decltype(m_strServiceRegion)>>::put(w, m_strServiceRegion);
        Codec<std::remove_cvref_t<decltype(m_strServiceAccountID)>>::put(w, m_strServiceAccountID);
        Codec<std::remove_cvref_t<decltype(m_strServicePassword)>>::put(w, m_strServicePassword);
        Codec<std::remove_cvref_t<decltype(m_strAuthenticationFlag)>>::put(w, m_strAuthenticationFlag);
        Codec<std::remove_cvref_t<decltype(m_strType)>>::put(w, m_strType);
        Codec<std::remove_cvref_t<decltype(m_strDuplicateCheck)>>::put(w, m_strDuplicateCheck);
        Codec<std::remove_cvref_t<decltype(m_strIPAddress)>>::put(w, m_strIPAddress);
        Codec<std::remove_cvref_t<decltype(m_strPacketID)>>::put(w, m_strPacketID);
    }

    static KEGASH_GET_GASHPOINT_REQ read_from(Reader& r) {
        KEGASH_GET_GASHPOINT_REQ v;
        v.m_iActorUID = Codec<std::remove_cvref_t<decltype(v.m_iActorUID)>>::get(r);
        v.m_strServiceCode = Codec<std::remove_cvref_t<decltype(v.m_strServiceCode)>>::get(r);
        v.m_strServiceRegion = Codec<std::remove_cvref_t<decltype(v.m_strServiceRegion)>>::get(r);
        v.m_strServiceAccountID = Codec<std::remove_cvref_t<decltype(v.m_strServiceAccountID)>>::get(r);
        v.m_strServicePassword = Codec<std::remove_cvref_t<decltype(v.m_strServicePassword)>>::get(r);
        v.m_strAuthenticationFlag = Codec<std::remove_cvref_t<decltype(v.m_strAuthenticationFlag)>>::get(r);
        v.m_strType = Codec<std::remove_cvref_t<decltype(v.m_strType)>>::get(r);
        v.m_strDuplicateCheck = Codec<std::remove_cvref_t<decltype(v.m_strDuplicateCheck)>>::get(r);
        v.m_strIPAddress = Codec<std::remove_cvref_t<decltype(v.m_strIPAddress)>>::get(r);
        v.m_strPacketID = Codec<std::remove_cvref_t<decltype(v.m_strPacketID)>>::get(r);
        return v;
    }
};

struct KEGASH_USE_GASH_LOG_NOT {
    std::u16string m_wstrTotalOrderID{};  // std::wstring
    std::int32_t   m_iServerGroupID{};  // int
    std::u16string m_wstrUserID{};  // std::wstring
    std::int64_t   m_iUserUID{};  // UidType
    std::u16string m_wstrNickName{};  // std::wstring
    std::int32_t   m_iBeforeCash{};  // int
    std::int32_t   m_iAfterGash{};  // int
    std::int32_t   m_iGashPoint{};  // int
    std::int32_t   m_iItemID{};  // int
    std::int32_t   m_iProductID{};  // int
    std::int32_t   m_iFlag{};  // int
    std::u16string m_wstrMemo{};  // std::wstring
    std::u16string m_wstrUserIP{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrTotalOrderID)>>::put(w, m_wstrTotalOrderID);
        Codec<std::remove_cvref_t<decltype(m_iServerGroupID)>>::put(w, m_iServerGroupID);
        Codec<std::remove_cvref_t<decltype(m_wstrUserID)>>::put(w, m_wstrUserID);
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_iBeforeCash)>>::put(w, m_iBeforeCash);
        Codec<std::remove_cvref_t<decltype(m_iAfterGash)>>::put(w, m_iAfterGash);
        Codec<std::remove_cvref_t<decltype(m_iGashPoint)>>::put(w, m_iGashPoint);
        Codec<std::remove_cvref_t<decltype(m_iItemID)>>::put(w, m_iItemID);
        Codec<std::remove_cvref_t<decltype(m_iProductID)>>::put(w, m_iProductID);
        Codec<std::remove_cvref_t<decltype(m_iFlag)>>::put(w, m_iFlag);
        Codec<std::remove_cvref_t<decltype(m_iAfterGash)>>::put(w, m_iAfterGash);
        Codec<std::remove_cvref_t<decltype(m_wstrMemo)>>::put(w, m_wstrMemo);
        Codec<std::remove_cvref_t<decltype(m_wstrUserIP)>>::put(w, m_wstrUserIP);
    }

    static KEGASH_USE_GASH_LOG_NOT read_from(Reader& r) {
        KEGASH_USE_GASH_LOG_NOT v;
        v.m_wstrTotalOrderID = Codec<std::remove_cvref_t<decltype(v.m_wstrTotalOrderID)>>::get(r);
        v.m_iServerGroupID = Codec<std::remove_cvref_t<decltype(v.m_iServerGroupID)>>::get(r);
        v.m_wstrUserID = Codec<std::remove_cvref_t<decltype(v.m_wstrUserID)>>::get(r);
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_iBeforeCash = Codec<std::remove_cvref_t<decltype(v.m_iBeforeCash)>>::get(r);
        v.m_iAfterGash = Codec<std::remove_cvref_t<decltype(v.m_iAfterGash)>>::get(r);
        v.m_iGashPoint = Codec<std::remove_cvref_t<decltype(v.m_iGashPoint)>>::get(r);
        v.m_iItemID = Codec<std::remove_cvref_t<decltype(v.m_iItemID)>>::get(r);
        v.m_iProductID = Codec<std::remove_cvref_t<decltype(v.m_iProductID)>>::get(r);
        v.m_iFlag = Codec<std::remove_cvref_t<decltype(v.m_iFlag)>>::get(r);
        v.m_iAfterGash = Codec<std::remove_cvref_t<decltype(v.m_iAfterGash)>>::get(r);
        v.m_wstrMemo = Codec<std::remove_cvref_t<decltype(v.m_wstrMemo)>>::get(r);
        v.m_wstrUserIP = Codec<std::remove_cvref_t<decltype(v.m_wstrUserIP)>>::get(r);
        return v;
    }
};

struct KEGB_ADMIN_HERO_PVP_USER_TOURNAMENT_CONNECTION_NOT {
    std::int64_t   m_iServerGroupID{};  // UidType
    std::u16string m_wstrNickName{};  // std::wstring
    bool           m_bConnect{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iServerGroupID)>>::put(w, m_iServerGroupID);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_bConnect)>>::put(w, m_bConnect);
    }

    static KEGB_ADMIN_HERO_PVP_USER_TOURNAMENT_CONNECTION_NOT read_from(Reader& r) {
        KEGB_ADMIN_HERO_PVP_USER_TOURNAMENT_CONNECTION_NOT v;
        v.m_iServerGroupID = Codec<std::remove_cvref_t<decltype(v.m_iServerGroupID)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_bConnect = Codec<std::remove_cvref_t<decltype(v.m_bConnect)>>::get(r);
        return v;
    }
};

struct KEGB_AUTO_PARTY_MAKING_ADMIN_COMMAND_NOT {
    char m_cType{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cType)>>::put(w, m_cType);
    }

    static KEGB_AUTO_PARTY_MAKING_ADMIN_COMMAND_NOT read_from(Reader& r) {
        KEGB_AUTO_PARTY_MAKING_ADMIN_COMMAND_NOT v;
        v.m_cType = Codec<std::remove_cvref_t<decltype(v.m_cType)>>::get(r);
        return v;
    }
};

struct KEGB_BATTLE_FIELD_BOSS_INFO_NOT {
    std::int64_t m_tRemainPortalTime{};  // __time64_t
    std::int64_t m_tRemainCoolTime{};  // __time64_t
    std::int32_t m_iPortalAppearanceMap{};  // int
    std::int32_t m_iPortalDestination{};  // int
    bool         m_bPortalOpen{};
    bool         m_bCoolTime{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_tRemainPortalTime)>>::put(w, m_tRemainPortalTime);
        Codec<std::remove_cvref_t<decltype(m_tRemainCoolTime)>>::put(w, m_tRemainCoolTime);
        Codec<std::remove_cvref_t<decltype(m_iPortalAppearanceMap)>>::put(w, m_iPortalAppearanceMap);
        Codec<std::remove_cvref_t<decltype(m_iPortalDestination)>>::put(w, m_iPortalDestination);
        Codec<std::remove_cvref_t<decltype(m_bPortalOpen)>>::put(w, m_bPortalOpen);
        Codec<std::remove_cvref_t<decltype(m_bCoolTime)>>::put(w, m_bCoolTime);
    }

    static KEGB_BATTLE_FIELD_BOSS_INFO_NOT read_from(Reader& r) {
        KEGB_BATTLE_FIELD_BOSS_INFO_NOT v;
        v.m_tRemainPortalTime = Codec<std::remove_cvref_t<decltype(v.m_tRemainPortalTime)>>::get(r);
        v.m_tRemainCoolTime = Codec<std::remove_cvref_t<decltype(v.m_tRemainCoolTime)>>::get(r);
        v.m_iPortalAppearanceMap = Codec<std::remove_cvref_t<decltype(v.m_iPortalAppearanceMap)>>::get(r);
        v.m_iPortalDestination = Codec<std::remove_cvref_t<decltype(v.m_iPortalDestination)>>::get(r);
        v.m_bPortalOpen = Codec<std::remove_cvref_t<decltype(v.m_bPortalOpen)>>::get(r);
        v.m_bCoolTime = Codec<std::remove_cvref_t<decltype(v.m_bCoolTime)>>::get(r);
        return v;
    }
};

struct KEGB_CANCEL_AUTO_PARTY_MAKING_REQ {
    std::int64_t m_iAutoPartyUID{};  // UidType
    std::int64_t m_iAutoPartyWaitNumber{};  // UidType
    std::int32_t m_iReason{};  // int
    bool         m_bSendBySystem{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iAutoPartyUID)>>::put(w, m_iAutoPartyUID);
        Codec<std::remove_cvref_t<decltype(m_iAutoPartyWaitNumber)>>::put(w, m_iAutoPartyWaitNumber);
        Codec<std::remove_cvref_t<decltype(m_iReason)>>::put(w, m_iReason);
        Codec<std::remove_cvref_t<decltype(m_bSendBySystem)>>::put(w, m_bSendBySystem);
    }

    static KEGB_CANCEL_AUTO_PARTY_MAKING_REQ read_from(Reader& r) {
        KEGB_CANCEL_AUTO_PARTY_MAKING_REQ v;
        v.m_iAutoPartyUID = Codec<std::remove_cvref_t<decltype(v.m_iAutoPartyUID)>>::get(r);
        v.m_iAutoPartyWaitNumber = Codec<std::remove_cvref_t<decltype(v.m_iAutoPartyWaitNumber)>>::get(r);
        v.m_iReason = Codec<std::remove_cvref_t<decltype(v.m_iReason)>>::get(r);
        v.m_bSendBySystem = Codec<std::remove_cvref_t<decltype(v.m_bSendBySystem)>>::get(r);
        return v;
    }
};

struct KEGB_CANCEL_MATCH_MAKING_REQ {
    std::int64_t m_iMatchUID{};  // UidType
    std::int64_t m_iPartyUID{};  // UidType
    std::int32_t m_iReason{};  // int
    bool         m_bSendBySystem{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iMatchUID)>>::put(w, m_iMatchUID);
        Codec<std::remove_cvref_t<decltype(m_iPartyUID)>>::put(w, m_iPartyUID);
        Codec<std::remove_cvref_t<decltype(m_iReason)>>::put(w, m_iReason);
        Codec<std::remove_cvref_t<decltype(m_bSendBySystem)>>::put(w, m_bSendBySystem);
    }

    static KEGB_CANCEL_MATCH_MAKING_REQ read_from(Reader& r) {
        KEGB_CANCEL_MATCH_MAKING_REQ v;
        v.m_iMatchUID = Codec<std::remove_cvref_t<decltype(v.m_iMatchUID)>>::get(r);
        v.m_iPartyUID = Codec<std::remove_cvref_t<decltype(v.m_iPartyUID)>>::get(r);
        v.m_iReason = Codec<std::remove_cvref_t<decltype(v.m_iReason)>>::get(r);
        v.m_bSendBySystem = Codec<std::remove_cvref_t<decltype(v.m_bSendBySystem)>>::get(r);
        return v;
    }
};

struct KEGB_CHECK_BLOCK_USER_NOT {
    std::vector<std::int32_t> m_vecBlockType{};  // std::vector< int >
    std::int64_t              m_iUserUID{};  // UidType
    std::int64_t              m_iUnitUID{};  // UidType
    std::u16string            m_wstrIP{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecBlockType)>>::put(w, m_vecBlockType);
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrIP)>>::put(w, m_wstrIP);
    }

    static KEGB_CHECK_BLOCK_USER_NOT read_from(Reader& r) {
        KEGB_CHECK_BLOCK_USER_NOT v;
        v.m_vecBlockType = Codec<std::remove_cvref_t<decltype(v.m_vecBlockType)>>::get(r);
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_wstrIP = Codec<std::remove_cvref_t<decltype(v.m_wstrIP)>>::get(r);
        return v;
    }
};

struct KEGB_EXCHANGE_LIMIT_ACK {
    std::int32_t m_iOK{};  // int
    std::int32_t m_iExchangeLimitUID{};  // int
    std::int32_t m_iLimitUID{};  // int
    std::int32_t m_iRollBackUID{};  // int
    std::int32_t m_iAfterWorkStorageKey{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iExchangeLimitUID)>>::put(w, m_iExchangeLimitUID);
        Codec<std::remove_cvref_t<decltype(m_iLimitUID)>>::put(w, m_iLimitUID);
        Codec<std::remove_cvref_t<decltype(m_iRollBackUID)>>::put(w, m_iRollBackUID);
        Codec<std::remove_cvref_t<decltype(m_iAfterWorkStorageKey)>>::put(w, m_iAfterWorkStorageKey);
    }

    static KEGB_EXCHANGE_LIMIT_ACK read_from(Reader& r) {
        KEGB_EXCHANGE_LIMIT_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iExchangeLimitUID = Codec<std::remove_cvref_t<decltype(v.m_iExchangeLimitUID)>>::get(r);
        v.m_iLimitUID = Codec<std::remove_cvref_t<decltype(v.m_iLimitUID)>>::get(r);
        v.m_iRollBackUID = Codec<std::remove_cvref_t<decltype(v.m_iRollBackUID)>>::get(r);
        v.m_iAfterWorkStorageKey = Codec<std::remove_cvref_t<decltype(v.m_iAfterWorkStorageKey)>>::get(r);
        return v;
    }
};

struct KEGB_EXCHANGE_LIMIT_INFO_NOT {
    std::int32_t                         m_iExchangeLimitUID{};  // int
    std::map<std::int32_t, std::int32_t> m_mapExchangeLimitInfo{};  // std::map< int, int >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iExchangeLimitUID)>>::put(w, m_iExchangeLimitUID);
        Codec<std::remove_cvref_t<decltype(m_mapExchangeLimitInfo)>>::put(w, m_mapExchangeLimitInfo);
    }

    static KEGB_EXCHANGE_LIMIT_INFO_NOT read_from(Reader& r) {
        KEGB_EXCHANGE_LIMIT_INFO_NOT v;
        v.m_iExchangeLimitUID = Codec<std::remove_cvref_t<decltype(v.m_iExchangeLimitUID)>>::get(r);
        v.m_mapExchangeLimitInfo = Codec<std::remove_cvref_t<decltype(v.m_mapExchangeLimitInfo)>>::get(r);
        return v;
    }
};

struct KEGB_EXCHANGE_LIMIT_INFO_ROLLBACK_NOT {
    std::int64_t m_iUnitUID{};  // UidType
    std::int32_t m_iExchangeLimitUID{};  // int
    std::int32_t m_iLimitUID{};  // int
    std::int32_t m_iRollBackUID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iExchangeLimitUID)>>::put(w, m_iExchangeLimitUID);
        Codec<std::remove_cvref_t<decltype(m_iLimitUID)>>::put(w, m_iLimitUID);
        Codec<std::remove_cvref_t<decltype(m_iRollBackUID)>>::put(w, m_iRollBackUID);
    }

    static KEGB_EXCHANGE_LIMIT_INFO_ROLLBACK_NOT read_from(Reader& r) {
        KEGB_EXCHANGE_LIMIT_INFO_ROLLBACK_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iExchangeLimitUID = Codec<std::remove_cvref_t<decltype(v.m_iExchangeLimitUID)>>::get(r);
        v.m_iLimitUID = Codec<std::remove_cvref_t<decltype(v.m_iLimitUID)>>::get(r);
        v.m_iRollBackUID = Codec<std::remove_cvref_t<decltype(v.m_iRollBackUID)>>::get(r);
        return v;
    }
};

struct KEGB_EXCHANGE_LIMIT_REQ {
    std::int64_t m_iGSUID{};  // UidType
    std::int64_t m_iUnitUID{};  // UidType
    std::int32_t m_iAfterWorkStorageKey{};  // int
    std::int32_t m_iDestItemID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iGSUID)>>::put(w, m_iGSUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iAfterWorkStorageKey)>>::put(w, m_iAfterWorkStorageKey);
        Codec<std::remove_cvref_t<decltype(m_iDestItemID)>>::put(w, m_iDestItemID);
    }

    static KEGB_EXCHANGE_LIMIT_REQ read_from(Reader& r) {
        KEGB_EXCHANGE_LIMIT_REQ v;
        v.m_iGSUID = Codec<std::remove_cvref_t<decltype(v.m_iGSUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iAfterWorkStorageKey = Codec<std::remove_cvref_t<decltype(v.m_iAfterWorkStorageKey)>>::get(r);
        v.m_iDestItemID = Codec<std::remove_cvref_t<decltype(v.m_iDestItemID)>>::get(r);
        return v;
    }
};

struct KEGB_GET_AUTO_PARTY_BONUS_INFO_ACK {
    std::vector<KAutoPartyBonusInfo> m_vecAutoPartyBonusInfo{};  // std::vector< KAutoPartyBonusInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecAutoPartyBonusInfo)>>::put(w, m_vecAutoPartyBonusInfo);
    }

    static KEGB_GET_AUTO_PARTY_BONUS_INFO_ACK read_from(Reader& r) {
        KEGB_GET_AUTO_PARTY_BONUS_INFO_ACK v;
        v.m_vecAutoPartyBonusInfo = Codec<std::remove_cvref_t<decltype(v.m_vecAutoPartyBonusInfo)>>::get(r);
        return v;
    }
};

struct KEGB_GET_GLOBAL_MISSION_INFO_REQ {
    std::int64_t m_iServerUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iServerUID)>>::put(w, m_iServerUID);
    }

    static KEGB_GET_GLOBAL_MISSION_INFO_REQ read_from(Reader& r) {
        KEGB_GET_GLOBAL_MISSION_INFO_REQ v;
        v.m_iServerUID = Codec<std::remove_cvref_t<decltype(v.m_iServerUID)>>::get(r);
        return v;
    }
};

struct KEGB_GET_MISSION_INFO_ACK {
    bool                                                  m_bActive{};
    std::int32_t                                          m_iFlag{};  // int
    std::int32_t                                          m_iWorldCrystalCount{};  // int
    std::int32_t                                          m_iWorldMaxCrystalCount{};  // int
    std::int32_t                                          m_iMissionTime{};  // int
    std::int32_t                                          m_iDungeonPlayTime{};  // int
    std::map<std::int32_t, std::vector<KDefenseOpenTime>> m_mapDefenseOpenTime{};  // std::map< int, std::vector< KDefenseOpenTime > >
    std::uint16_t                                         m_usWorldBuffDurationTime{};  // u_short

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bActive)>>::put(w, m_bActive);
        Codec<std::remove_cvref_t<decltype(m_iFlag)>>::put(w, m_iFlag);
        Codec<std::remove_cvref_t<decltype(m_iWorldCrystalCount)>>::put(w, m_iWorldCrystalCount);
        Codec<std::remove_cvref_t<decltype(m_iWorldMaxCrystalCount)>>::put(w, m_iWorldMaxCrystalCount);
        Codec<std::remove_cvref_t<decltype(m_iMissionTime)>>::put(w, m_iMissionTime);
        Codec<std::remove_cvref_t<decltype(m_iDungeonPlayTime)>>::put(w, m_iDungeonPlayTime);
        Codec<std::remove_cvref_t<decltype(m_mapDefenseOpenTime)>>::put(w, m_mapDefenseOpenTime);
        Codec<std::remove_cvref_t<decltype(m_usWorldBuffDurationTime)>>::put(w, m_usWorldBuffDurationTime);
    }

    static KEGB_GET_MISSION_INFO_ACK read_from(Reader& r) {
        KEGB_GET_MISSION_INFO_ACK v;
        v.m_bActive = Codec<std::remove_cvref_t<decltype(v.m_bActive)>>::get(r);
        v.m_iFlag = Codec<std::remove_cvref_t<decltype(v.m_iFlag)>>::get(r);
        v.m_iWorldCrystalCount = Codec<std::remove_cvref_t<decltype(v.m_iWorldCrystalCount)>>::get(r);
        v.m_iWorldMaxCrystalCount = Codec<std::remove_cvref_t<decltype(v.m_iWorldMaxCrystalCount)>>::get(r);
        v.m_iMissionTime = Codec<std::remove_cvref_t<decltype(v.m_iMissionTime)>>::get(r);
        v.m_iDungeonPlayTime = Codec<std::remove_cvref_t<decltype(v.m_iDungeonPlayTime)>>::get(r);
        v.m_mapDefenseOpenTime = Codec<std::remove_cvref_t<decltype(v.m_mapDefenseOpenTime)>>::get(r);
        v.m_usWorldBuffDurationTime = Codec<std::remove_cvref_t<decltype(v.m_usWorldBuffDurationTime)>>::get(r);
        return v;
    }
};

struct KEGB_GET_MISSION_INFO_REQ {
    std::int64_t m_iServerUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iServerUID)>>::put(w, m_iServerUID);
    }

    static KEGB_GET_MISSION_INFO_REQ read_from(Reader& r) {
        KEGB_GET_MISSION_INFO_REQ v;
        v.m_iServerUID = Codec<std::remove_cvref_t<decltype(v.m_iServerUID)>>::get(r);
        return v;
    }
};

struct KEGB_GET_REGROUP_PARTY_INFO_REQ {
    std::int64_t     m_iAutoPartyUID{};  // UidType
    KDungeonGameInfo m_kDungeonGameInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iAutoPartyUID)>>::put(w, m_iAutoPartyUID);
        Codec<std::remove_cvref_t<decltype(m_kDungeonGameInfo)>>::put(w, m_kDungeonGameInfo);
    }

    static KEGB_GET_REGROUP_PARTY_INFO_REQ read_from(Reader& r) {
        KEGB_GET_REGROUP_PARTY_INFO_REQ v;
        v.m_iAutoPartyUID = Codec<std::remove_cvref_t<decltype(v.m_iAutoPartyUID)>>::get(r);
        v.m_kDungeonGameInfo = Codec<std::remove_cvref_t<decltype(v.m_kDungeonGameInfo)>>::get(r);
        return v;
    }
};

struct KEGB_MATCH_MAKING_ADMIN_COMMAND_NOT {
    char m_cType{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cType)>>::put(w, m_cType);
    }

    static KEGB_MATCH_MAKING_ADMIN_COMMAND_NOT read_from(Reader& r) {
        KEGB_MATCH_MAKING_ADMIN_COMMAND_NOT v;
        v.m_cType = Codec<std::remove_cvref_t<decltype(v.m_cType)>>::get(r);
        return v;
    }
};

struct KEGB_REMAKING_AUTO_PARTY_NOT {
    KAutoPartyConditionInfo m_kAutoPartyInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kAutoPartyInfo)>>::put(w, m_kAutoPartyInfo);
    }

    static KEGB_REMAKING_AUTO_PARTY_NOT read_from(Reader& r) {
        KEGB_REMAKING_AUTO_PARTY_NOT v;
        v.m_kAutoPartyInfo = Codec<std::remove_cvref_t<decltype(v.m_kAutoPartyInfo)>>::get(r);
        return v;
    }
};

struct KEGB_REMATCH_MAKING_USER_NOT {
    char m_cPvpPlayerCount{};
    char m_cPvpGameType{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cPvpPlayerCount)>>::put(w, m_cPvpPlayerCount);
        Codec<std::remove_cvref_t<decltype(m_cPvpGameType)>>::put(w, m_cPvpGameType);
    }

    static KEGB_REMATCH_MAKING_USER_NOT read_from(Reader& r) {
        KEGB_REMATCH_MAKING_USER_NOT v;
        v.m_cPvpPlayerCount = Codec<std::remove_cvref_t<decltype(v.m_cPvpPlayerCount)>>::get(r);
        v.m_cPvpGameType = Codec<std::remove_cvref_t<decltype(v.m_cPvpGameType)>>::get(r);
        return v;
    }
};

struct KEGB_REQUEST_AUTO_PARTY_WAIT_USER_COUNT_NOT {
    std::int64_t m_iAutoPartyWaitNumber{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iAutoPartyWaitNumber)>>::put(w, m_iAutoPartyWaitNumber);
    }

    static KEGB_REQUEST_AUTO_PARTY_WAIT_USER_COUNT_NOT read_from(Reader& r) {
        KEGB_REQUEST_AUTO_PARTY_WAIT_USER_COUNT_NOT v;
        v.m_iAutoPartyWaitNumber = Codec<std::remove_cvref_t<decltype(v.m_iAutoPartyWaitNumber)>>::get(r);
        return v;
    }
};

struct KEGB_THE_GATE_OF_DARKNESS_INDIVIDUAL_BUFF_NOT {
    std::int32_t m_iTheGateOfDarknessIndividualBuffLevel{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iTheGateOfDarknessIndividualBuffLevel)>>::put(w, m_iTheGateOfDarknessIndividualBuffLevel);
    }

    static KEGB_THE_GATE_OF_DARKNESS_INDIVIDUAL_BUFF_NOT read_from(Reader& r) {
        KEGB_THE_GATE_OF_DARKNESS_INDIVIDUAL_BUFF_NOT v;
        v.m_iTheGateOfDarknessIndividualBuffLevel = Codec<std::remove_cvref_t<decltype(v.m_iTheGateOfDarknessIndividualBuffLevel)>>::get(r);
        return v;
    }
};

struct KEGB_TIME_ENCHANT_EVENT_INFO_NOT {
    std::int32_t m_iTimeEnchantEventUID{};  // int
    std::int32_t m_iNumOfTimeEnchantEvent{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iTimeEnchantEventUID)>>::put(w, m_iTimeEnchantEventUID);
        Codec<std::remove_cvref_t<decltype(m_iNumOfTimeEnchantEvent)>>::put(w, m_iNumOfTimeEnchantEvent);
    }

    static KEGB_TIME_ENCHANT_EVENT_INFO_NOT read_from(Reader& r) {
        KEGB_TIME_ENCHANT_EVENT_INFO_NOT v;
        v.m_iTimeEnchantEventUID = Codec<std::remove_cvref_t<decltype(v.m_iTimeEnchantEventUID)>>::get(r);
        v.m_iNumOfTimeEnchantEvent = Codec<std::remove_cvref_t<decltype(v.m_iNumOfTimeEnchantEvent)>>::get(r);
        return v;
    }
};

struct KEGB_TIME_ENCHANT_EVENT_NOT {
    std::int64_t  m_iGSUID{};  // UidType
    std::int64_t  m_iUnitUID{};  // UidType
    std::int32_t  m_iAfterWorkStorageKey{};  // int
    std::uint16_t m_usEventID{};  // unsigned short

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iGSUID)>>::put(w, m_iGSUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iAfterWorkStorageKey)>>::put(w, m_iAfterWorkStorageKey);
        Codec<std::remove_cvref_t<decltype(m_usEventID)>>::put(w, m_usEventID);
    }

    static KEGB_TIME_ENCHANT_EVENT_NOT read_from(Reader& r) {
        KEGB_TIME_ENCHANT_EVENT_NOT v;
        v.m_iGSUID = Codec<std::remove_cvref_t<decltype(v.m_iGSUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iAfterWorkStorageKey = Codec<std::remove_cvref_t<decltype(v.m_iAfterWorkStorageKey)>>::get(r);
        v.m_usEventID = Codec<std::remove_cvref_t<decltype(v.m_usEventID)>>::get(r);
        return v;
    }
};

struct KEGB_TODAY_RANDOM_QUEST_ACK {
    std::map<std::int32_t, std::int32_t> m_mapTodayRandomQuest{};  // std::map< int, int >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_mapTodayRandomQuest)>>::put(w, m_mapTodayRandomQuest);
    }

    static KEGB_TODAY_RANDOM_QUEST_ACK read_from(Reader& r) {
        KEGB_TODAY_RANDOM_QUEST_ACK v;
        v.m_mapTodayRandomQuest = Codec<std::remove_cvref_t<decltype(v.m_mapTodayRandomQuest)>>::get(r);
        return v;
    }
};

struct KEGB_UNIT_DISCONNECT_NOT {
    std::int64_t m_iUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
    }

    static KEGB_UNIT_DISCONNECT_NOT read_from(Reader& r) {
        KEGB_UNIT_DISCONNECT_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        return v;
    }
};

struct KEGB_UPDATE_AUTO_PARTY_BONUS_INFO_NOT {
    char         m_cDungeonMode{};
    std::int32_t m_iDungeonIDWithDif{};  // int
    bool         m_bActivatedBonus{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cDungeonMode)>>::put(w, m_cDungeonMode);
        Codec<std::remove_cvref_t<decltype(m_iDungeonIDWithDif)>>::put(w, m_iDungeonIDWithDif);
        Codec<std::remove_cvref_t<decltype(m_bActivatedBonus)>>::put(w, m_bActivatedBonus);
    }

    static KEGB_UPDATE_AUTO_PARTY_BONUS_INFO_NOT read_from(Reader& r) {
        KEGB_UPDATE_AUTO_PARTY_BONUS_INFO_NOT v;
        v.m_cDungeonMode = Codec<std::remove_cvref_t<decltype(v.m_cDungeonMode)>>::get(r);
        v.m_iDungeonIDWithDif = Codec<std::remove_cvref_t<decltype(v.m_iDungeonIDWithDif)>>::get(r);
        v.m_bActivatedBonus = Codec<std::remove_cvref_t<decltype(v.m_bActivatedBonus)>>::get(r);
        return v;
    }
};

struct KEGB_UPDATE_BLOCK_RESULT_NOT {
    std::map<std::int32_t, bool> m_mapBlockResult{};  // std::map< int, bool >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_mapBlockResult)>>::put(w, m_mapBlockResult);
    }

    static KEGB_UPDATE_BLOCK_RESULT_NOT read_from(Reader& r) {
        KEGB_UPDATE_BLOCK_RESULT_NOT v;
        v.m_mapBlockResult = Codec<std::remove_cvref_t<decltype(v.m_mapBlockResult)>>::get(r);
        return v;
    }
};

struct KEGB_UPDATE_GLOBAL_MISSION_START_TIME_NOT {
    std::map<std::int32_t, std::u16string> m_mapGlobalMissionStartTime{};  // std::map< int, std::wstring >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_mapGlobalMissionStartTime)>>::put(w, m_mapGlobalMissionStartTime);
    }

    static KEGB_UPDATE_GLOBAL_MISSION_START_TIME_NOT read_from(Reader& r) {
        KEGB_UPDATE_GLOBAL_MISSION_START_TIME_NOT v;
        v.m_mapGlobalMissionStartTime = Codec<std::remove_cvref_t<decltype(v.m_mapGlobalMissionStartTime)>>::get(r);
        return v;
    }
};

struct KEGB_UPDATE_MISSION_START_TIME_NOT {
    std::u16string m_wsrtStartTime{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wsrtStartTime)>>::put(w, m_wsrtStartTime);
    }

    static KEGB_UPDATE_MISSION_START_TIME_NOT read_from(Reader& r) {
        KEGB_UPDATE_MISSION_START_TIME_NOT v;
        v.m_wsrtStartTime = Codec<std::remove_cvref_t<decltype(v.m_wsrtStartTime)>>::get(r);
        return v;
    }
};

struct KEGB_UPDATE_TOTAL_DANGEROUS_VALUE_NOT {
    std::int32_t m_iBattleFieldID{};  // int
    std::uint8_t m_byteIncreasedValue{};  // byte

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iBattleFieldID)>>::put(w, m_iBattleFieldID);
        Codec<std::remove_cvref_t<decltype(m_byteIncreasedValue)>>::put(w, m_byteIncreasedValue);
    }

    static KEGB_UPDATE_TOTAL_DANGEROUS_VALUE_NOT read_from(Reader& r) {
        KEGB_UPDATE_TOTAL_DANGEROUS_VALUE_NOT v;
        v.m_iBattleFieldID = Codec<std::remove_cvref_t<decltype(v.m_iBattleFieldID)>>::get(r);
        v.m_byteIncreasedValue = Codec<std::remove_cvref_t<decltype(v.m_byteIncreasedValue)>>::get(r);
        return v;
    }
};

struct KEGB_UPDATE_WORLD_BUFF_NOT {
    bool          m_bActive{};
    std::uint16_t m_usSuccessWorldBuff{};  // u_short
    std::uint16_t m_usWorldBuffDurationTime{};  // u_short

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bActive)>>::put(w, m_bActive);
        Codec<std::remove_cvref_t<decltype(m_usSuccessWorldBuff)>>::put(w, m_usSuccessWorldBuff);
        Codec<std::remove_cvref_t<decltype(m_usWorldBuffDurationTime)>>::put(w, m_usWorldBuffDurationTime);
    }

    static KEGB_UPDATE_WORLD_BUFF_NOT read_from(Reader& r) {
        KEGB_UPDATE_WORLD_BUFF_NOT v;
        v.m_bActive = Codec<std::remove_cvref_t<decltype(v.m_bActive)>>::get(r);
        v.m_usSuccessWorldBuff = Codec<std::remove_cvref_t<decltype(v.m_usSuccessWorldBuff)>>::get(r);
        v.m_usWorldBuffDurationTime = Codec<std::remove_cvref_t<decltype(v.m_usWorldBuffDurationTime)>>::get(r);
        return v;
    }
};

struct KEGIANT_AUTH_LOGIN_FAIL {
    std::uint32_t  m_uiRequestID{};  // unsigned int
    std::int32_t   m_iReturnCode{};  // int
    std::u16string m_wstrName{};  // std::wstring
    std::uint32_t  m_uiKey{};  // unsigned int
    std::u16string m_wstrCoordinate{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_uiRequestID)>>::put(w, m_uiRequestID);
        Codec<std::remove_cvref_t<decltype(m_iReturnCode)>>::put(w, m_iReturnCode);
        Codec<std::remove_cvref_t<decltype(m_wstrName)>>::put(w, m_wstrName);
        Codec<std::remove_cvref_t<decltype(m_wstrName)>>::put(w, m_wstrName);
        Codec<std::remove_cvref_t<decltype(m_uiKey)>>::put(w, m_uiKey);
        Codec<std::remove_cvref_t<decltype(m_wstrCoordinate)>>::put(w, m_wstrCoordinate);
    }

    static KEGIANT_AUTH_LOGIN_FAIL read_from(Reader& r) {
        KEGIANT_AUTH_LOGIN_FAIL v;
        v.m_uiRequestID = Codec<std::remove_cvref_t<decltype(v.m_uiRequestID)>>::get(r);
        v.m_iReturnCode = Codec<std::remove_cvref_t<decltype(v.m_iReturnCode)>>::get(r);
        v.m_wstrName = Codec<std::remove_cvref_t<decltype(v.m_wstrName)>>::get(r);
        v.m_wstrName = Codec<std::remove_cvref_t<decltype(v.m_wstrName)>>::get(r);
        v.m_uiKey = Codec<std::remove_cvref_t<decltype(v.m_uiKey)>>::get(r);
        v.m_wstrCoordinate = Codec<std::remove_cvref_t<decltype(v.m_wstrCoordinate)>>::get(r);
        return v;
    }
};

struct KEGIANT_AUTH_LOGIN_SUCCESS {
    std::uint32_t  m_uiRequestID{};  // unsigned int
    std::uint32_t  m_uiUID{};  // unsigned int
    std::u16string m_wstrName{};  // std::wstring
    std::uint32_t  m_uiType{};  // unsigned int
    std::u16string m_wstrEMail{};  // std::wstring
    std::uint32_t  m_uiState{};  // unsigned int
    std::uint32_t  m_uiBindType{};  // unsigned int
    std::u16string m_wstrSuperpasswd{};  // std::wstring
    std::u16string m_wstrMobileAccount{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_uiRequestID)>>::put(w, m_uiRequestID);
        Codec<std::remove_cvref_t<decltype(m_uiUID)>>::put(w, m_uiUID);
        Codec<std::remove_cvref_t<decltype(m_wstrName)>>::put(w, m_wstrName);
        Codec<std::remove_cvref_t<decltype(m_uiType)>>::put(w, m_uiType);
        Codec<std::remove_cvref_t<decltype(m_wstrEMail)>>::put(w, m_wstrEMail);
        Codec<std::remove_cvref_t<decltype(m_uiState)>>::put(w, m_uiState);
        Codec<std::remove_cvref_t<decltype(m_uiBindType)>>::put(w, m_uiBindType);
        Codec<std::remove_cvref_t<decltype(m_wstrSuperpasswd)>>::put(w, m_wstrSuperpasswd);
        Codec<std::remove_cvref_t<decltype(m_wstrMobileAccount)>>::put(w, m_wstrMobileAccount);
    }

    static KEGIANT_AUTH_LOGIN_SUCCESS read_from(Reader& r) {
        KEGIANT_AUTH_LOGIN_SUCCESS v;
        v.m_uiRequestID = Codec<std::remove_cvref_t<decltype(v.m_uiRequestID)>>::get(r);
        v.m_uiUID = Codec<std::remove_cvref_t<decltype(v.m_uiUID)>>::get(r);
        v.m_wstrName = Codec<std::remove_cvref_t<decltype(v.m_wstrName)>>::get(r);
        v.m_uiType = Codec<std::remove_cvref_t<decltype(v.m_uiType)>>::get(r);
        v.m_wstrEMail = Codec<std::remove_cvref_t<decltype(v.m_wstrEMail)>>::get(r);
        v.m_uiState = Codec<std::remove_cvref_t<decltype(v.m_uiState)>>::get(r);
        v.m_uiBindType = Codec<std::remove_cvref_t<decltype(v.m_uiBindType)>>::get(r);
        v.m_wstrSuperpasswd = Codec<std::remove_cvref_t<decltype(v.m_wstrSuperpasswd)>>::get(r);
        v.m_wstrMobileAccount = Codec<std::remove_cvref_t<decltype(v.m_wstrMobileAccount)>>::get(r);
        return v;
    }
};

struct KEGIANT_BILLING_CONSUME_ACK {
    std::int32_t  m_iOK{};  // int
    std::int32_t  m_iBalance{};  // int
    std::int32_t  m_iBonus{};  // int
    bool          m_bHasFilled{};
    std::int32_t  m_iPoint{};  // int
    std::uint32_t m_uiGiantUID{};  // unsigned int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iBalance)>>::put(w, m_iBalance);
        Codec<std::remove_cvref_t<decltype(m_iBonus)>>::put(w, m_iBonus);
        Codec<std::remove_cvref_t<decltype(m_bHasFilled)>>::put(w, m_bHasFilled);
        Codec<std::remove_cvref_t<decltype(m_iPoint)>>::put(w, m_iPoint);
        Codec<std::remove_cvref_t<decltype(m_uiGiantUID)>>::put(w, m_uiGiantUID);
    }

    static KEGIANT_BILLING_CONSUME_ACK read_from(Reader& r) {
        KEGIANT_BILLING_CONSUME_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iBalance = Codec<std::remove_cvref_t<decltype(v.m_iBalance)>>::get(r);
        v.m_iBonus = Codec<std::remove_cvref_t<decltype(v.m_iBonus)>>::get(r);
        v.m_bHasFilled = Codec<std::remove_cvref_t<decltype(v.m_bHasFilled)>>::get(r);
        v.m_iPoint = Codec<std::remove_cvref_t<decltype(v.m_iPoint)>>::get(r);
        v.m_uiGiantUID = Codec<std::remove_cvref_t<decltype(v.m_uiGiantUID)>>::get(r);
        return v;
    }
};

struct KEGIANT_BILLING_CONSUME_REQ {
    std::uint32_t  m_uiGiantUID{};  // unsigned int
    std::int32_t   m_iPoint{};  // int
    std::int32_t   m_iSource{};  // int
    std::u16string m_wstrTID{};  // std::wstring
    std::u16string m_wstrRemark{};  // std::wstring
    std::u16string m_wstrIP{};  // std::wstring
    std::int32_t   m_iAT{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_uiGiantUID)>>::put(w, m_uiGiantUID);
        Codec<std::remove_cvref_t<decltype(m_iPoint)>>::put(w, m_iPoint);
        Codec<std::remove_cvref_t<decltype(m_iSource)>>::put(w, m_iSource);
        Codec<std::remove_cvref_t<decltype(m_wstrTID)>>::put(w, m_wstrTID);
        Codec<std::remove_cvref_t<decltype(m_wstrRemark)>>::put(w, m_wstrRemark);
        Codec<std::remove_cvref_t<decltype(m_wstrIP)>>::put(w, m_wstrIP);
        Codec<std::remove_cvref_t<decltype(m_iAT)>>::put(w, m_iAT);
    }

    static KEGIANT_BILLING_CONSUME_REQ read_from(Reader& r) {
        KEGIANT_BILLING_CONSUME_REQ v;
        v.m_uiGiantUID = Codec<std::remove_cvref_t<decltype(v.m_uiGiantUID)>>::get(r);
        v.m_iPoint = Codec<std::remove_cvref_t<decltype(v.m_iPoint)>>::get(r);
        v.m_iSource = Codec<std::remove_cvref_t<decltype(v.m_iSource)>>::get(r);
        v.m_wstrTID = Codec<std::remove_cvref_t<decltype(v.m_wstrTID)>>::get(r);
        v.m_wstrRemark = Codec<std::remove_cvref_t<decltype(v.m_wstrRemark)>>::get(r);
        v.m_wstrIP = Codec<std::remove_cvref_t<decltype(v.m_wstrIP)>>::get(r);
        v.m_iAT = Codec<std::remove_cvref_t<decltype(v.m_iAT)>>::get(r);
        return v;
    }
};

struct KEGIANT_BILLING_PCARD_CMD {
    std::uint32_t  m_uiGiantUID{};  // unsigned int
    std::int32_t   m_iSource{};  // int
    std::u16string m_wstrTID{};  // std::wstring
    std::u16string m_wstrPCardID{};  // std::wstring
    std::u16string m_wstrIP{};  // std::wstring
    std::int32_t   m_iAT{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_uiGiantUID)>>::put(w, m_uiGiantUID);
        Codec<std::remove_cvref_t<decltype(m_iSource)>>::put(w, m_iSource);
        Codec<std::remove_cvref_t<decltype(m_wstrTID)>>::put(w, m_wstrTID);
        Codec<std::remove_cvref_t<decltype(m_wstrPCardID)>>::put(w, m_wstrPCardID);
        Codec<std::remove_cvref_t<decltype(m_wstrIP)>>::put(w, m_wstrIP);
        Codec<std::remove_cvref_t<decltype(m_iAT)>>::put(w, m_iAT);
    }

    static KEGIANT_BILLING_PCARD_CMD read_from(Reader& r) {
        KEGIANT_BILLING_PCARD_CMD v;
        v.m_uiGiantUID = Codec<std::remove_cvref_t<decltype(v.m_uiGiantUID)>>::get(r);
        v.m_iSource = Codec<std::remove_cvref_t<decltype(v.m_iSource)>>::get(r);
        v.m_wstrTID = Codec<std::remove_cvref_t<decltype(v.m_wstrTID)>>::get(r);
        v.m_wstrPCardID = Codec<std::remove_cvref_t<decltype(v.m_wstrPCardID)>>::get(r);
        v.m_wstrIP = Codec<std::remove_cvref_t<decltype(v.m_wstrIP)>>::get(r);
        v.m_iAT = Codec<std::remove_cvref_t<decltype(v.m_iAT)>>::get(r);
        return v;
    }
};

struct KEGIANT_BILLING_RETURN {
    std::u16string m_wstrTID{};  // std::wstring
    std::int32_t   m_iBalance{};  // int
    std::int32_t   m_iBonus{};  // int
    std::int32_t   m_iHadFilled{};  // int
    std::int32_t   m_iRet{};  // int
    std::int32_t   m_iSubat{};  // int
    std::int32_t   m_iAT{};  // int
    std::int32_t   m_iPoint{};  // int
    std::uint32_t  m_uiGiantUID{};  // unsigned int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrTID)>>::put(w, m_wstrTID);
        Codec<std::remove_cvref_t<decltype(m_iBalance)>>::put(w, m_iBalance);
        Codec<std::remove_cvref_t<decltype(m_iBonus)>>::put(w, m_iBonus);
        Codec<std::remove_cvref_t<decltype(m_iHadFilled)>>::put(w, m_iHadFilled);
        Codec<std::remove_cvref_t<decltype(m_iRet)>>::put(w, m_iRet);
        Codec<std::remove_cvref_t<decltype(m_iSubat)>>::put(w, m_iSubat);
        Codec<std::remove_cvref_t<decltype(m_iAT)>>::put(w, m_iAT);
        Codec<std::remove_cvref_t<decltype(m_iPoint)>>::put(w, m_iPoint);
        Codec<std::remove_cvref_t<decltype(m_uiGiantUID)>>::put(w, m_uiGiantUID);
    }

    static KEGIANT_BILLING_RETURN read_from(Reader& r) {
        KEGIANT_BILLING_RETURN v;
        v.m_wstrTID = Codec<std::remove_cvref_t<decltype(v.m_wstrTID)>>::get(r);
        v.m_iBalance = Codec<std::remove_cvref_t<decltype(v.m_iBalance)>>::get(r);
        v.m_iBonus = Codec<std::remove_cvref_t<decltype(v.m_iBonus)>>::get(r);
        v.m_iHadFilled = Codec<std::remove_cvref_t<decltype(v.m_iHadFilled)>>::get(r);
        v.m_iRet = Codec<std::remove_cvref_t<decltype(v.m_iRet)>>::get(r);
        v.m_iSubat = Codec<std::remove_cvref_t<decltype(v.m_iSubat)>>::get(r);
        v.m_iAT = Codec<std::remove_cvref_t<decltype(v.m_iAT)>>::get(r);
        v.m_iPoint = Codec<std::remove_cvref_t<decltype(v.m_iPoint)>>::get(r);
        v.m_uiGiantUID = Codec<std::remove_cvref_t<decltype(v.m_uiGiantUID)>>::get(r);
        return v;
    }
};

struct KEGIANT_BILLING_SCARD_CMD {
    std::uint32_t  m_uiGiantUID{};  // unsigned int
    std::int32_t   m_iSource{};  // int
    std::u16string m_wstrTID{};  // std::wstring
    std::u16string m_wstrSCardID{};  // std::wstring
    std::u16string m_wstrIP{};  // std::wstring
    std::int32_t   m_iAT{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_uiGiantUID)>>::put(w, m_uiGiantUID);
        Codec<std::remove_cvref_t<decltype(m_iSource)>>::put(w, m_iSource);
        Codec<std::remove_cvref_t<decltype(m_wstrTID)>>::put(w, m_wstrTID);
        Codec<std::remove_cvref_t<decltype(m_wstrSCardID)>>::put(w, m_wstrSCardID);
        Codec<std::remove_cvref_t<decltype(m_wstrIP)>>::put(w, m_wstrIP);
        Codec<std::remove_cvref_t<decltype(m_iAT)>>::put(w, m_iAT);
    }

    static KEGIANT_BILLING_SCARD_CMD read_from(Reader& r) {
        KEGIANT_BILLING_SCARD_CMD v;
        v.m_uiGiantUID = Codec<std::remove_cvref_t<decltype(v.m_uiGiantUID)>>::get(r);
        v.m_iSource = Codec<std::remove_cvref_t<decltype(v.m_iSource)>>::get(r);
        v.m_wstrTID = Codec<std::remove_cvref_t<decltype(v.m_wstrTID)>>::get(r);
        v.m_wstrSCardID = Codec<std::remove_cvref_t<decltype(v.m_wstrSCardID)>>::get(r);
        v.m_wstrIP = Codec<std::remove_cvref_t<decltype(v.m_wstrIP)>>::get(r);
        v.m_iAT = Codec<std::remove_cvref_t<decltype(v.m_iAT)>>::get(r);
        return v;
    }
};

struct KEGIANT_BILLING_USE_CARD_CMD {
    std::int16_t   m_sSource{};  // short
    std::u16string m_wstrAccount{};  // std::wstring
    std::u16string m_wstrTID{};  // std::wstring
    std::u16string m_wstrCardID{};  // std::wstring
    std::u16string m_wstrPasswd{};  // std::wstring
    std::u16string m_wstrIP{};  // std::wstring
    std::uint32_t  m_uiUID{};  // unsigned int
    std::int32_t   m_iAT{};  // int
    std::uint32_t  m_uiConTempID{};  // unsigned int
    std::uint32_t  m_uiOperateUID{};  // unsigned int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_sSource)>>::put(w, m_sSource);
        Codec<std::remove_cvref_t<decltype(m_wstrAccount)>>::put(w, m_wstrAccount);
        Codec<std::remove_cvref_t<decltype(m_wstrTID)>>::put(w, m_wstrTID);
        Codec<std::remove_cvref_t<decltype(m_wstrCardID)>>::put(w, m_wstrCardID);
        Codec<std::remove_cvref_t<decltype(m_wstrPasswd)>>::put(w, m_wstrPasswd);
        Codec<std::remove_cvref_t<decltype(m_wstrIP)>>::put(w, m_wstrIP);
        Codec<std::remove_cvref_t<decltype(m_uiUID)>>::put(w, m_uiUID);
        Codec<std::remove_cvref_t<decltype(m_iAT)>>::put(w, m_iAT);
        Codec<std::remove_cvref_t<decltype(m_uiConTempID)>>::put(w, m_uiConTempID);
        Codec<std::remove_cvref_t<decltype(m_uiOperateUID)>>::put(w, m_uiOperateUID);
    }

    static KEGIANT_BILLING_USE_CARD_CMD read_from(Reader& r) {
        KEGIANT_BILLING_USE_CARD_CMD v;
        v.m_sSource = Codec<std::remove_cvref_t<decltype(v.m_sSource)>>::get(r);
        v.m_wstrAccount = Codec<std::remove_cvref_t<decltype(v.m_wstrAccount)>>::get(r);
        v.m_wstrTID = Codec<std::remove_cvref_t<decltype(v.m_wstrTID)>>::get(r);
        v.m_wstrCardID = Codec<std::remove_cvref_t<decltype(v.m_wstrCardID)>>::get(r);
        v.m_wstrPasswd = Codec<std::remove_cvref_t<decltype(v.m_wstrPasswd)>>::get(r);
        v.m_wstrIP = Codec<std::remove_cvref_t<decltype(v.m_wstrIP)>>::get(r);
        v.m_uiUID = Codec<std::remove_cvref_t<decltype(v.m_uiUID)>>::get(r);
        v.m_iAT = Codec<std::remove_cvref_t<decltype(v.m_iAT)>>::get(r);
        v.m_uiConTempID = Codec<std::remove_cvref_t<decltype(v.m_uiConTempID)>>::get(r);
        v.m_uiOperateUID = Codec<std::remove_cvref_t<decltype(v.m_uiOperateUID)>>::get(r);
        return v;
    }
};

struct KEGIANT_BILLING_USE_CARD_RET_CMD {
    std::int16_t   m_sSource{};  // short
    std::u16string m_wstrAccount{};  // std::wstring
    std::u16string m_wstrTID{};  // std::wstring
    std::u16string m_wstrCardID{};  // std::wstring
    std::u16string m_wstrPasswd{};  // std::wstring
    std::u16string m_wstrIP{};  // std::wstring
    std::int32_t   m_iPoint{};  // int
    std::int32_t   m_iRet{};  // int
    std::uint32_t  m_uiUID{};  // unsigned int
    std::int32_t   m_iAT{};  // int
    std::uint32_t  m_uiConTempID{};  // unsigned int
    std::uint32_t  m_uiOperateUID{};  // unsigned int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_sSource)>>::put(w, m_sSource);
        Codec<std::remove_cvref_t<decltype(m_wstrAccount)>>::put(w, m_wstrAccount);
        Codec<std::remove_cvref_t<decltype(m_wstrTID)>>::put(w, m_wstrTID);
        Codec<std::remove_cvref_t<decltype(m_wstrCardID)>>::put(w, m_wstrCardID);
        Codec<std::remove_cvref_t<decltype(m_wstrPasswd)>>::put(w, m_wstrPasswd);
        Codec<std::remove_cvref_t<decltype(m_wstrIP)>>::put(w, m_wstrIP);
        Codec<std::remove_cvref_t<decltype(m_iPoint)>>::put(w, m_iPoint);
        Codec<std::remove_cvref_t<decltype(m_iRet)>>::put(w, m_iRet);
        Codec<std::remove_cvref_t<decltype(m_uiUID)>>::put(w, m_uiUID);
        Codec<std::remove_cvref_t<decltype(m_iAT)>>::put(w, m_iAT);
        Codec<std::remove_cvref_t<decltype(m_uiConTempID)>>::put(w, m_uiConTempID);
        Codec<std::remove_cvref_t<decltype(m_uiOperateUID)>>::put(w, m_uiOperateUID);
    }

    static KEGIANT_BILLING_USE_CARD_RET_CMD read_from(Reader& r) {
        KEGIANT_BILLING_USE_CARD_RET_CMD v;
        v.m_sSource = Codec<std::remove_cvref_t<decltype(v.m_sSource)>>::get(r);
        v.m_wstrAccount = Codec<std::remove_cvref_t<decltype(v.m_wstrAccount)>>::get(r);
        v.m_wstrTID = Codec<std::remove_cvref_t<decltype(v.m_wstrTID)>>::get(r);
        v.m_wstrCardID = Codec<std::remove_cvref_t<decltype(v.m_wstrCardID)>>::get(r);
        v.m_wstrPasswd = Codec<std::remove_cvref_t<decltype(v.m_wstrPasswd)>>::get(r);
        v.m_wstrIP = Codec<std::remove_cvref_t<decltype(v.m_wstrIP)>>::get(r);
        v.m_iPoint = Codec<std::remove_cvref_t<decltype(v.m_iPoint)>>::get(r);
        v.m_iRet = Codec<std::remove_cvref_t<decltype(v.m_iRet)>>::get(r);
        v.m_uiUID = Codec<std::remove_cvref_t<decltype(v.m_uiUID)>>::get(r);
        v.m_iAT = Codec<std::remove_cvref_t<decltype(v.m_iAT)>>::get(r);
        v.m_uiConTempID = Codec<std::remove_cvref_t<decltype(v.m_uiConTempID)>>::get(r);
        v.m_uiOperateUID = Codec<std::remove_cvref_t<decltype(v.m_uiOperateUID)>>::get(r);
        return v;
    }
};

struct KEGIANT_COMMON_INITIALIZE_ACK {
    std::uint16_t  m_usZone{};  // unsigned short
    std::uint16_t  m_usGame{};  // unsigned short
    std::u16string m_wstrZoneName{};  // std::wstring
    std::uint8_t   m_byteNetType{};  // BYTE

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_usZone)>>::put(w, m_usZone);
        Codec<std::remove_cvref_t<decltype(m_usGame)>>::put(w, m_usGame);
        Codec<std::remove_cvref_t<decltype(m_wstrZoneName)>>::put(w, m_wstrZoneName);
        Codec<std::remove_cvref_t<decltype(m_byteNetType)>>::put(w, m_byteNetType);
    }

    static KEGIANT_COMMON_INITIALIZE_ACK read_from(Reader& r) {
        KEGIANT_COMMON_INITIALIZE_ACK v;
        v.m_usZone = Codec<std::remove_cvref_t<decltype(v.m_usZone)>>::get(r);
        v.m_usGame = Codec<std::remove_cvref_t<decltype(v.m_usGame)>>::get(r);
        v.m_wstrZoneName = Codec<std::remove_cvref_t<decltype(v.m_wstrZoneName)>>::get(r);
        v.m_byteNetType = Codec<std::remove_cvref_t<decltype(v.m_byteNetType)>>::get(r);
        return v;
    }
};

struct KEGIANT_COMMON_INITIALIZE_REQ {
    std::u16string m_wstrIP{};  // std::wstring
    std::uint16_t  m_usPort{};  // unsigned short

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrIP)>>::put(w, m_wstrIP);
        Codec<std::remove_cvref_t<decltype(m_usPort)>>::put(w, m_usPort);
    }

    static KEGIANT_COMMON_INITIALIZE_REQ read_from(Reader& r) {
        KEGIANT_COMMON_INITIALIZE_REQ v;
        v.m_wstrIP = Codec<std::remove_cvref_t<decltype(v.m_wstrIP)>>::get(r);
        v.m_usPort = Codec<std::remove_cvref_t<decltype(v.m_usPort)>>::get(r);
        return v;
    }
};

struct KEGIANT_COMMON_NULL_CLIENT {
    // no serialised fields

    void write_to(Writer& w) const {
        (void)w;
    }

    static KEGIANT_COMMON_NULL_CLIENT read_from(Reader& r) {
        KEGIANT_COMMON_NULL_CLIENT v;
        (void)r;
        return v;
    }
};

struct KEGIANT_COMMON_NULL_SERVER {
    // no serialised fields

    void write_to(Writer& w) const {
        (void)w;
    }

    static KEGIANT_COMMON_NULL_SERVER read_from(Reader& r) {
        KEGIANT_COMMON_NULL_SERVER v;
        (void)r;
        return v;
    }
};

struct KEGIANT_INFO_CHAT_NOT {
    std::uint16_t  m_usZone{};  // unsigned short
    std::uint16_t  m_usGame{};  // unsigned short
    std::u16string m_wstrUserName{};  // std::wstring
    std::u16string m_wstrGMName{};  // std::wstring
    std::uint32_t  m_uiCountryID{};  // unsigned int
    std::uint32_t  m_uiSceneID{};  // unsigned int
    std::uint32_t  m_uiType{};  // unsigned int
    std::u16string m_wstrContent{};  // std::wstring
    std::u16string m_wstrServer{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_usZone)>>::put(w, m_usZone);
        Codec<std::remove_cvref_t<decltype(m_usGame)>>::put(w, m_usGame);
        Codec<std::remove_cvref_t<decltype(m_wstrUserName)>>::put(w, m_wstrUserName);
        Codec<std::remove_cvref_t<decltype(m_wstrGMName)>>::put(w, m_wstrGMName);
        Codec<std::remove_cvref_t<decltype(m_uiCountryID)>>::put(w, m_uiCountryID);
        Codec<std::remove_cvref_t<decltype(m_uiSceneID)>>::put(w, m_uiSceneID);
        Codec<std::remove_cvref_t<decltype(m_uiType)>>::put(w, m_uiType);
        Codec<std::remove_cvref_t<decltype(m_wstrContent)>>::put(w, m_wstrContent);
        Codec<std::remove_cvref_t<decltype(m_wstrServer)>>::put(w, m_wstrServer);
    }

    static KEGIANT_INFO_CHAT_NOT read_from(Reader& r) {
        KEGIANT_INFO_CHAT_NOT v;
        v.m_usZone = Codec<std::remove_cvref_t<decltype(v.m_usZone)>>::get(r);
        v.m_usGame = Codec<std::remove_cvref_t<decltype(v.m_usGame)>>::get(r);
        v.m_wstrUserName = Codec<std::remove_cvref_t<decltype(v.m_wstrUserName)>>::get(r);
        v.m_wstrGMName = Codec<std::remove_cvref_t<decltype(v.m_wstrGMName)>>::get(r);
        v.m_uiCountryID = Codec<std::remove_cvref_t<decltype(v.m_uiCountryID)>>::get(r);
        v.m_uiSceneID = Codec<std::remove_cvref_t<decltype(v.m_uiSceneID)>>::get(r);
        v.m_uiType = Codec<std::remove_cvref_t<decltype(v.m_uiType)>>::get(r);
        v.m_wstrContent = Codec<std::remove_cvref_t<decltype(v.m_wstrContent)>>::get(r);
        v.m_wstrServer = Codec<std::remove_cvref_t<decltype(v.m_wstrServer)>>::get(r);
        return v;
    }
};

struct KEGIANT_INFO_GET_CCU_REQ {
    std::int64_t m_TimeStamp{};  // __int64

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_TimeStamp)>>::put(w, m_TimeStamp);
    }

    static KEGIANT_INFO_GET_CCU_REQ read_from(Reader& r) {
        KEGIANT_INFO_GET_CCU_REQ v;
        v.m_TimeStamp = Codec<std::remove_cvref_t<decltype(v.m_TimeStamp)>>::get(r);
        return v;
    }
};

struct KEGIANT_INFO_GET_CHANNEL_CCU_ACK {
    std::int64_t                          m_TimeStamp{};  // __int64
    std::uint16_t                         m_usZone{};  // unsigned short
    std::uint16_t                         m_usGame{};  // unsigned short
    std::map<std::int64_t, std::uint32_t> m_mapChannelCCU{};  // std::map<UidType, UINT>

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_TimeStamp)>>::put(w, m_TimeStamp);
        Codec<std::remove_cvref_t<decltype(m_usZone)>>::put(w, m_usZone);
        Codec<std::remove_cvref_t<decltype(m_usGame)>>::put(w, m_usGame);
        Codec<std::remove_cvref_t<decltype(m_mapChannelCCU)>>::put(w, m_mapChannelCCU);
    }

    static KEGIANT_INFO_GET_CHANNEL_CCU_ACK read_from(Reader& r) {
        KEGIANT_INFO_GET_CHANNEL_CCU_ACK v;
        v.m_TimeStamp = Codec<std::remove_cvref_t<decltype(v.m_TimeStamp)>>::get(r);
        v.m_usZone = Codec<std::remove_cvref_t<decltype(v.m_usZone)>>::get(r);
        v.m_usGame = Codec<std::remove_cvref_t<decltype(v.m_usGame)>>::get(r);
        v.m_mapChannelCCU = Codec<std::remove_cvref_t<decltype(v.m_mapChannelCCU)>>::get(r);
        return v;
    }
};

struct KEGIANT_INFO_GET_TOTAL_CCU_ACK {
    std::int64_t   m_TimeStamp{};  // __int64
    std::uint16_t  m_usServerID{};  // unsigned short
    std::uint16_t  m_usServerType{};  // unsigned short
    std::uint16_t  m_usZone{};  // unsigned short
    std::uint16_t  m_usGame{};  // unsigned short
    std::u16string m_strZoneName{};  // std::wstring
    std::uint32_t  m_uiCCU{};  // unsigned int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_TimeStamp)>>::put(w, m_TimeStamp);
        Codec<std::remove_cvref_t<decltype(m_usServerID)>>::put(w, m_usServerID);
        Codec<std::remove_cvref_t<decltype(m_usServerType)>>::put(w, m_usServerType);
        Codec<std::remove_cvref_t<decltype(m_usZone)>>::put(w, m_usZone);
        Codec<std::remove_cvref_t<decltype(m_usGame)>>::put(w, m_usGame);
        Codec<std::remove_cvref_t<decltype(m_strZoneName)>>::put(w, m_strZoneName);
        Codec<std::remove_cvref_t<decltype(m_uiCCU)>>::put(w, m_uiCCU);
    }

    static KEGIANT_INFO_GET_TOTAL_CCU_ACK read_from(Reader& r) {
        KEGIANT_INFO_GET_TOTAL_CCU_ACK v;
        v.m_TimeStamp = Codec<std::remove_cvref_t<decltype(v.m_TimeStamp)>>::get(r);
        v.m_usServerID = Codec<std::remove_cvref_t<decltype(v.m_usServerID)>>::get(r);
        v.m_usServerType = Codec<std::remove_cvref_t<decltype(v.m_usServerType)>>::get(r);
        v.m_usZone = Codec<std::remove_cvref_t<decltype(v.m_usZone)>>::get(r);
        v.m_usGame = Codec<std::remove_cvref_t<decltype(v.m_usGame)>>::get(r);
        v.m_strZoneName = Codec<std::remove_cvref_t<decltype(v.m_strZoneName)>>::get(r);
        v.m_uiCCU = Codec<std::remove_cvref_t<decltype(v.m_uiCCU)>>::get(r);
        return v;
    }
};

struct KEGIANT_INFO_PUNISH_REQ {
    std::uint16_t  m_usZone{};  // unsigned short
    std::uint16_t  m_usGame{};  // unsigned short
    std::u16string m_wstrUserName{};  // std::wstring
    std::u16string m_wstrCountryName{};  // std::wstring
    std::u16string m_wstrServerName{};  // std::wstring
    std::u16string m_wstrGMName{};  // std::wstring
    std::u16string m_wstrReason{};  // std::wstring
    std::uint16_t  m_usOperation{};  // unsigned short
    std::int32_t   m_iDelay{};  // int
    std::int16_t   m_sLevel{};  // short
    std::uint32_t  m_uiGiantUID{};  // unsigned int
    std::uint32_t  m_uiStartTime{};  // unsigned int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_usZone)>>::put(w, m_usZone);
        Codec<std::remove_cvref_t<decltype(m_usGame)>>::put(w, m_usGame);
        Codec<std::remove_cvref_t<decltype(m_wstrUserName)>>::put(w, m_wstrUserName);
        Codec<std::remove_cvref_t<decltype(m_wstrCountryName)>>::put(w, m_wstrCountryName);
        Codec<std::remove_cvref_t<decltype(m_wstrServerName)>>::put(w, m_wstrServerName);
        Codec<std::remove_cvref_t<decltype(m_wstrGMName)>>::put(w, m_wstrGMName);
        Codec<std::remove_cvref_t<decltype(m_wstrReason)>>::put(w, m_wstrReason);
        Codec<std::remove_cvref_t<decltype(m_usOperation)>>::put(w, m_usOperation);
        Codec<std::remove_cvref_t<decltype(m_iDelay)>>::put(w, m_iDelay);
        Codec<std::remove_cvref_t<decltype(m_sLevel)>>::put(w, m_sLevel);
        Codec<std::remove_cvref_t<decltype(m_uiGiantUID)>>::put(w, m_uiGiantUID);
        Codec<std::remove_cvref_t<decltype(m_uiStartTime)>>::put(w, m_uiStartTime);
    }

    static KEGIANT_INFO_PUNISH_REQ read_from(Reader& r) {
        KEGIANT_INFO_PUNISH_REQ v;
        v.m_usZone = Codec<std::remove_cvref_t<decltype(v.m_usZone)>>::get(r);
        v.m_usGame = Codec<std::remove_cvref_t<decltype(v.m_usGame)>>::get(r);
        v.m_wstrUserName = Codec<std::remove_cvref_t<decltype(v.m_wstrUserName)>>::get(r);
        v.m_wstrCountryName = Codec<std::remove_cvref_t<decltype(v.m_wstrCountryName)>>::get(r);
        v.m_wstrServerName = Codec<std::remove_cvref_t<decltype(v.m_wstrServerName)>>::get(r);
        v.m_wstrGMName = Codec<std::remove_cvref_t<decltype(v.m_wstrGMName)>>::get(r);
        v.m_wstrReason = Codec<std::remove_cvref_t<decltype(v.m_wstrReason)>>::get(r);
        v.m_usOperation = Codec<std::remove_cvref_t<decltype(v.m_usOperation)>>::get(r);
        v.m_iDelay = Codec<std::remove_cvref_t<decltype(v.m_iDelay)>>::get(r);
        v.m_sLevel = Codec<std::remove_cvref_t<decltype(v.m_sLevel)>>::get(r);
        v.m_uiGiantUID = Codec<std::remove_cvref_t<decltype(v.m_uiGiantUID)>>::get(r);
        v.m_uiStartTime = Codec<std::remove_cvref_t<decltype(v.m_uiStartTime)>>::get(r);
        return v;
    }
};

struct KEGIANT_ROLEREG {
    std::uint16_t  m_usRegType{};  // unsigned short
    std::uint16_t  m_usServerID{};  // unsigned short
    std::uint32_t  m_uiGiantUID{};  // unsigned int
    std::uint16_t  m_usZone{};  // unsigned short
    std::uint16_t  m_usGame{};  // unsigned short
    std::u16string m_wstrName{};  // std::wstring
    std::uint32_t  m_uiRoleUID{};  // unsigned int
    std::uint16_t  m_usState{};  // unsigned short

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_usRegType)>>::put(w, m_usRegType);
        Codec<std::remove_cvref_t<decltype(m_usServerID)>>::put(w, m_usServerID);
        Codec<std::remove_cvref_t<decltype(m_uiGiantUID)>>::put(w, m_uiGiantUID);
        Codec<std::remove_cvref_t<decltype(m_usZone)>>::put(w, m_usZone);
        Codec<std::remove_cvref_t<decltype(m_usGame)>>::put(w, m_usGame);
        Codec<std::remove_cvref_t<decltype(m_wstrName)>>::put(w, m_wstrName);
        Codec<std::remove_cvref_t<decltype(m_uiRoleUID)>>::put(w, m_uiRoleUID);
        Codec<std::remove_cvref_t<decltype(m_usState)>>::put(w, m_usState);
    }

    static KEGIANT_ROLEREG read_from(Reader& r) {
        KEGIANT_ROLEREG v;
        v.m_usRegType = Codec<std::remove_cvref_t<decltype(v.m_usRegType)>>::get(r);
        v.m_usServerID = Codec<std::remove_cvref_t<decltype(v.m_usServerID)>>::get(r);
        v.m_uiGiantUID = Codec<std::remove_cvref_t<decltype(v.m_uiGiantUID)>>::get(r);
        v.m_usZone = Codec<std::remove_cvref_t<decltype(v.m_usZone)>>::get(r);
        v.m_usGame = Codec<std::remove_cvref_t<decltype(v.m_usGame)>>::get(r);
        v.m_wstrName = Codec<std::remove_cvref_t<decltype(v.m_wstrName)>>::get(r);
        v.m_uiRoleUID = Codec<std::remove_cvref_t<decltype(v.m_uiRoleUID)>>::get(r);
        v.m_usState = Codec<std::remove_cvref_t<decltype(v.m_usState)>>::get(r);
        return v;
    }
};

struct KEGIANT_ROLEREG_DELETE_GUILD_ACK {
    std::int32_t m_iOK{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
    }

    static KEGIANT_ROLEREG_DELETE_GUILD_ACK read_from(Reader& r) {
        KEGIANT_ROLEREG_DELETE_GUILD_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        return v;
    }
};

struct KEGIANT_ROLEREG_DELETE_GUILD_REQ {
    std::u16string m_wstrGuildName{};  // std::wstring
    std::uint32_t  m_uiGiantUID{};  // unsigned int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrGuildName)>>::put(w, m_wstrGuildName);
        Codec<std::remove_cvref_t<decltype(m_uiGiantUID)>>::put(w, m_uiGiantUID);
    }

    static KEGIANT_ROLEREG_DELETE_GUILD_REQ read_from(Reader& r) {
        KEGIANT_ROLEREG_DELETE_GUILD_REQ v;
        v.m_wstrGuildName = Codec<std::remove_cvref_t<decltype(v.m_wstrGuildName)>>::get(r);
        v.m_uiGiantUID = Codec<std::remove_cvref_t<decltype(v.m_uiGiantUID)>>::get(r);
        return v;
    }
};

struct KEGIANT_ROLEREG_DELETE_UNIT_ACK {
    std::int32_t m_iOK{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
    }

    static KEGIANT_ROLEREG_DELETE_UNIT_ACK read_from(Reader& r) {
        KEGIANT_ROLEREG_DELETE_UNIT_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        return v;
    }
};

struct KEGIANT_ROLEREG_DELETE_UNIT_REQ {
    std::u16string m_wstrNickName{};  // std::wstring
    std::uint32_t  m_uiGiantUID{};  // unsigned int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_uiGiantUID)>>::put(w, m_uiGiantUID);
    }

    static KEGIANT_ROLEREG_DELETE_UNIT_REQ read_from(Reader& r) {
        KEGIANT_ROLEREG_DELETE_UNIT_REQ v;
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_uiGiantUID = Codec<std::remove_cvref_t<decltype(v.m_uiGiantUID)>>::get(r);
        return v;
    }
};

struct KEGS_2013_EVENT_MISSION_COMPLETE_REQ {
    bool m_bLevelUpEvent{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bLevelUpEvent)>>::put(w, m_bLevelUpEvent);
    }

    static KEGS_2013_EVENT_MISSION_COMPLETE_REQ read_from(Reader& r) {
        KEGS_2013_EVENT_MISSION_COMPLETE_REQ v;
        v.m_bLevelUpEvent = Codec<std::remove_cvref_t<decltype(v.m_bLevelUpEvent)>>::get(r);
        return v;
    }
};

struct KEGS_ACCEPT_FRIEND_ACK {
    std::int32_t m_iOK{};  // int
    std::int64_t m_iUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
    }

    static KEGS_ACCEPT_FRIEND_ACK read_from(Reader& r) {
        KEGS_ACCEPT_FRIEND_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        return v;
    }
};

struct KEGS_ACCEPT_FRIEND_NOT {
    std::int64_t m_iUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
    }

    static KEGS_ACCEPT_FRIEND_NOT read_from(Reader& r) {
        KEGS_ACCEPT_FRIEND_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        return v;
    }
};

struct KEGS_ACCEPT_JOIN_GUILD_ACK {
    std::int32_t   m_iOK{};  // int
    std::u16string m_wstrReceiverNickName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_wstrReceiverNickName)>>::put(w, m_wstrReceiverNickName);
    }

    static KEGS_ACCEPT_JOIN_GUILD_ACK read_from(Reader& r) {
        KEGS_ACCEPT_JOIN_GUILD_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_wstrReceiverNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrReceiverNickName)>>::get(r);
        return v;
    }
};

struct KEGS_ACCEPT_JOIN_GUILD_REQ {
    std::int64_t m_iUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
    }

    static KEGS_ACCEPT_JOIN_GUILD_REQ read_from(Reader& r) {
        KEGS_ACCEPT_JOIN_GUILD_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        return v;
    }
};

struct KEGS_ADD_COMMON_FLAG_NOT {
    std::uint32_t dwFlag{};  // DWORD

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(dwFlag)>>::put(w, dwFlag);
    }

    static KEGS_ADD_COMMON_FLAG_NOT read_from(Reader& r) {
        KEGS_ADD_COMMON_FLAG_NOT v;
        v.dwFlag = Codec<std::remove_cvref_t<decltype(v.dwFlag)>>::get(r);
        return v;
    }
};

struct KEGS_ADD_ON_STAT_NOT {
    std::int64_t m_UID{};  // UidType
    bool         m_bIsUnit{};
    char         m_cStatType{};
    float        m_fStatVal{};
    float        m_fTime{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_UID)>>::put(w, m_UID);
        Codec<std::remove_cvref_t<decltype(m_bIsUnit)>>::put(w, m_bIsUnit);
        Codec<std::remove_cvref_t<decltype(m_cStatType)>>::put(w, m_cStatType);
        Codec<std::remove_cvref_t<decltype(m_fStatVal)>>::put(w, m_fStatVal);
        Codec<std::remove_cvref_t<decltype(m_fTime)>>::put(w, m_fTime);
    }

    static KEGS_ADD_ON_STAT_NOT read_from(Reader& r) {
        KEGS_ADD_ON_STAT_NOT v;
        v.m_UID = Codec<std::remove_cvref_t<decltype(v.m_UID)>>::get(r);
        v.m_bIsUnit = Codec<std::remove_cvref_t<decltype(v.m_bIsUnit)>>::get(r);
        v.m_cStatType = Codec<std::remove_cvref_t<decltype(v.m_cStatType)>>::get(r);
        v.m_fStatVal = Codec<std::remove_cvref_t<decltype(v.m_fStatVal)>>::get(r);
        v.m_fTime = Codec<std::remove_cvref_t<decltype(v.m_fTime)>>::get(r);
        return v;
    }
};

struct KEGS_ADD_ON_STAT_REQ {
    std::int64_t m_UID{};  // UidType
    bool         m_bIsUnit{};
    char         m_cStatType{};
    float        m_fStatVal{};
    float        m_fTime{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_UID)>>::put(w, m_UID);
        Codec<std::remove_cvref_t<decltype(m_bIsUnit)>>::put(w, m_bIsUnit);
        Codec<std::remove_cvref_t<decltype(m_cStatType)>>::put(w, m_cStatType);
        Codec<std::remove_cvref_t<decltype(m_fStatVal)>>::put(w, m_fStatVal);
        Codec<std::remove_cvref_t<decltype(m_fTime)>>::put(w, m_fTime);
    }

    static KEGS_ADD_ON_STAT_REQ read_from(Reader& r) {
        KEGS_ADD_ON_STAT_REQ v;
        v.m_UID = Codec<std::remove_cvref_t<decltype(v.m_UID)>>::get(r);
        v.m_bIsUnit = Codec<std::remove_cvref_t<decltype(v.m_bIsUnit)>>::get(r);
        v.m_cStatType = Codec<std::remove_cvref_t<decltype(v.m_cStatType)>>::get(r);
        v.m_fStatVal = Codec<std::remove_cvref_t<decltype(v.m_fStatVal)>>::get(r);
        v.m_fTime = Codec<std::remove_cvref_t<decltype(v.m_fTime)>>::get(r);
        return v;
    }
};

struct KEGS_ADMIN_AUTO_GET_ALL_SKILL_ACK {
    std::int32_t                         m_iOK{};  // int
    std::int32_t                         m_iActiveSkillPageNumber{};  // int
    std::int32_t                         m_iSPoint{};  // int
    std::int32_t                         m_iCSPoint{};  // int
    std::map<std::int32_t, std::int32_t> m_mapSkillList{};  // std::map< int, int>
    std::vector<std::int16_t>            m_vecUnsealedSkillID{};  // std::vector< short >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iActiveSkillPageNumber)>>::put(w, m_iActiveSkillPageNumber);
        Codec<std::remove_cvref_t<decltype(m_iSPoint)>>::put(w, m_iSPoint);
        Codec<std::remove_cvref_t<decltype(m_iCSPoint)>>::put(w, m_iCSPoint);
        Codec<std::remove_cvref_t<decltype(m_mapSkillList)>>::put(w, m_mapSkillList);
        Codec<std::remove_cvref_t<decltype(m_vecUnsealedSkillID)>>::put(w, m_vecUnsealedSkillID);
    }

    static KEGS_ADMIN_AUTO_GET_ALL_SKILL_ACK read_from(Reader& r) {
        KEGS_ADMIN_AUTO_GET_ALL_SKILL_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iActiveSkillPageNumber = Codec<std::remove_cvref_t<decltype(v.m_iActiveSkillPageNumber)>>::get(r);
        v.m_iSPoint = Codec<std::remove_cvref_t<decltype(v.m_iSPoint)>>::get(r);
        v.m_iCSPoint = Codec<std::remove_cvref_t<decltype(v.m_iCSPoint)>>::get(r);
        v.m_mapSkillList = Codec<std::remove_cvref_t<decltype(v.m_mapSkillList)>>::get(r);
        v.m_vecUnsealedSkillID = Codec<std::remove_cvref_t<decltype(v.m_vecUnsealedSkillID)>>::get(r);
        return v;
    }
};

struct KEGS_ADMIN_AUTO_GET_ALL_SKILL_REQ {
    std::int32_t m_iActiveSkillPageNumber{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iActiveSkillPageNumber)>>::put(w, m_iActiveSkillPageNumber);
    }

    static KEGS_ADMIN_AUTO_GET_ALL_SKILL_REQ read_from(Reader& r) {
        KEGS_ADMIN_AUTO_GET_ALL_SKILL_REQ v;
        v.m_iActiveSkillPageNumber = Codec<std::remove_cvref_t<decltype(v.m_iActiveSkillPageNumber)>>::get(r);
        return v;
    }
};

struct KEGS_ADMIN_BATTLE_FIELD_INCREASE_DANGER_ACK {
    std::int32_t m_iOK{};  // int
    std::int32_t m_iDanger{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iDanger)>>::put(w, m_iDanger);
    }

    static KEGS_ADMIN_BATTLE_FIELD_INCREASE_DANGER_ACK read_from(Reader& r) {
        KEGS_ADMIN_BATTLE_FIELD_INCREASE_DANGER_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iDanger = Codec<std::remove_cvref_t<decltype(v.m_iDanger)>>::get(r);
        return v;
    }
};

struct KEGS_ADMIN_BATTLE_FIELD_INCREASE_DANGER_REQ {
    std::int32_t m_iDanger{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iDanger)>>::put(w, m_iDanger);
    }

    static KEGS_ADMIN_BATTLE_FIELD_INCREASE_DANGER_REQ read_from(Reader& r) {
        KEGS_ADMIN_BATTLE_FIELD_INCREASE_DANGER_REQ v;
        v.m_iDanger = Codec<std::remove_cvref_t<decltype(v.m_iDanger)>>::get(r);
        return v;
    }
};

struct KEGS_ADMIN_BOSS_FIELD_GATE_OPEN_REQ {
    std::int32_t m_iBattleFieldID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iBattleFieldID)>>::put(w, m_iBattleFieldID);
    }

    static KEGS_ADMIN_BOSS_FIELD_GATE_OPEN_REQ read_from(Reader& r) {
        KEGS_ADMIN_BOSS_FIELD_GATE_OPEN_REQ v;
        v.m_iBattleFieldID = Codec<std::remove_cvref_t<decltype(v.m_iBattleFieldID)>>::get(r);
        return v;
    }
};

struct KEGS_ADMIN_CASH_SKILL_POINT_DATE_CHANGE_ACK {
    std::int32_t   m_iOK{};  // int
    std::u16string m_wstrCspEndDate{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_wstrCspEndDate)>>::put(w, m_wstrCspEndDate);
    }

    static KEGS_ADMIN_CASH_SKILL_POINT_DATE_CHANGE_ACK read_from(Reader& r) {
        KEGS_ADMIN_CASH_SKILL_POINT_DATE_CHANGE_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_wstrCspEndDate = Codec<std::remove_cvref_t<decltype(v.m_wstrCspEndDate)>>::get(r);
        return v;
    }
};

struct KEGS_ADMIN_CASH_SKILL_POINT_DATE_CHANGE_REQ {
    std::int32_t m_iYear{};  // int
    std::int32_t m_iMonth{};  // int
    std::int32_t m_iDay{};  // int
    std::int32_t m_iHour{};  // int
    std::int32_t m_iMinute{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iYear)>>::put(w, m_iYear);
        Codec<std::remove_cvref_t<decltype(m_iMonth)>>::put(w, m_iMonth);
        Codec<std::remove_cvref_t<decltype(m_iDay)>>::put(w, m_iDay);
        Codec<std::remove_cvref_t<decltype(m_iHour)>>::put(w, m_iHour);
        Codec<std::remove_cvref_t<decltype(m_iMinute)>>::put(w, m_iMinute);
    }

    static KEGS_ADMIN_CASH_SKILL_POINT_DATE_CHANGE_REQ read_from(Reader& r) {
        KEGS_ADMIN_CASH_SKILL_POINT_DATE_CHANGE_REQ v;
        v.m_iYear = Codec<std::remove_cvref_t<decltype(v.m_iYear)>>::get(r);
        v.m_iMonth = Codec<std::remove_cvref_t<decltype(v.m_iMonth)>>::get(r);
        v.m_iDay = Codec<std::remove_cvref_t<decltype(v.m_iDay)>>::get(r);
        v.m_iHour = Codec<std::remove_cvref_t<decltype(v.m_iHour)>>::get(r);
        v.m_iMinute = Codec<std::remove_cvref_t<decltype(v.m_iMinute)>>::get(r);
        return v;
    }
};

struct KEGS_ADMIN_CHANGE_AUTH_LEVEL_REQ {
    std::u16string m_wstrUserID{};  // std::wstring
    char           m_cAuthLevel{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrUserID)>>::put(w, m_wstrUserID);
        Codec<std::remove_cvref_t<decltype(m_cAuthLevel)>>::put(w, m_cAuthLevel);
    }

    static KEGS_ADMIN_CHANGE_AUTH_LEVEL_REQ read_from(Reader& r) {
        KEGS_ADMIN_CHANGE_AUTH_LEVEL_REQ v;
        v.m_wstrUserID = Codec<std::remove_cvref_t<decltype(v.m_wstrUserID)>>::get(r);
        v.m_cAuthLevel = Codec<std::remove_cvref_t<decltype(v.m_cAuthLevel)>>::get(r);
        return v;
    }
};

struct KEGS_ADMIN_CHANGE_COUPLE_DATE_REQ {
    std::int32_t m_iAddHour{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iAddHour)>>::put(w, m_iAddHour);
    }

    static KEGS_ADMIN_CHANGE_COUPLE_DATE_REQ read_from(Reader& r) {
        KEGS_ADMIN_CHANGE_COUPLE_DATE_REQ v;
        v.m_iAddHour = Codec<std::remove_cvref_t<decltype(v.m_iAddHour)>>::get(r);
        return v;
    }
};

struct KEGS_ADMIN_CHANGE_ED_REQ {
    std::u16string m_wstrUnitNickName{};  // std::wstring
    std::int32_t   m_nED{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrUnitNickName)>>::put(w, m_wstrUnitNickName);
        Codec<std::remove_cvref_t<decltype(m_nED)>>::put(w, m_nED);
    }

    static KEGS_ADMIN_CHANGE_ED_REQ read_from(Reader& r) {
        KEGS_ADMIN_CHANGE_ED_REQ v;
        v.m_wstrUnitNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrUnitNickName)>>::get(r);
        v.m_nED = Codec<std::remove_cvref_t<decltype(v.m_nED)>>::get(r);
        return v;
    }
};

struct KEGS_ADMIN_CHANGE_PET_INFO_NOT {
    std::int32_t m_iCheatType{};  // int
    std::int32_t m_iValue{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iCheatType)>>::put(w, m_iCheatType);
        Codec<std::remove_cvref_t<decltype(m_iValue)>>::put(w, m_iValue);
    }

    static KEGS_ADMIN_CHANGE_PET_INFO_NOT read_from(Reader& r) {
        KEGS_ADMIN_CHANGE_PET_INFO_NOT v;
        v.m_iCheatType = Codec<std::remove_cvref_t<decltype(v.m_iCheatType)>>::get(r);
        v.m_iValue = Codec<std::remove_cvref_t<decltype(v.m_iValue)>>::get(r);
        return v;
    }
};

struct KEGS_ADMIN_CHANGE_SKILL_POINT_ACK {
    std::int32_t m_iOK{};  // int
    std::int32_t m_iSPoint{};  // int
    std::int32_t m_iActiveSkillPageNumber{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iSPoint)>>::put(w, m_iSPoint);
        Codec<std::remove_cvref_t<decltype(m_iActiveSkillPageNumber)>>::put(w, m_iActiveSkillPageNumber);
    }

    static KEGS_ADMIN_CHANGE_SKILL_POINT_ACK read_from(Reader& r) {
        KEGS_ADMIN_CHANGE_SKILL_POINT_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iSPoint = Codec<std::remove_cvref_t<decltype(v.m_iSPoint)>>::get(r);
        v.m_iActiveSkillPageNumber = Codec<std::remove_cvref_t<decltype(v.m_iActiveSkillPageNumber)>>::get(r);
        return v;
    }
};

struct KEGS_ADMIN_CHANGE_SKILL_POINT_REQ {
    std::int32_t m_iSPoint{};  // int
    std::int32_t m_iActiveSkillPageNumber{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iSPoint)>>::put(w, m_iSPoint);
        Codec<std::remove_cvref_t<decltype(m_iActiveSkillPageNumber)>>::put(w, m_iActiveSkillPageNumber);
    }

    static KEGS_ADMIN_CHANGE_SKILL_POINT_REQ read_from(Reader& r) {
        KEGS_ADMIN_CHANGE_SKILL_POINT_REQ v;
        v.m_iSPoint = Codec<std::remove_cvref_t<decltype(v.m_iSPoint)>>::get(r);
        v.m_iActiveSkillPageNumber = Codec<std::remove_cvref_t<decltype(v.m_iActiveSkillPageNumber)>>::get(r);
        return v;
    }
};

struct KEGS_ADMIN_CHANGE_SPIRIT_ACK {
    std::int32_t m_iOK{};  // int
    std::int32_t m_iChangedSpirit{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iChangedSpirit)>>::put(w, m_iChangedSpirit);
    }

    static KEGS_ADMIN_CHANGE_SPIRIT_ACK read_from(Reader& r) {
        KEGS_ADMIN_CHANGE_SPIRIT_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iChangedSpirit = Codec<std::remove_cvref_t<decltype(v.m_iChangedSpirit)>>::get(r);
        return v;
    }
};

struct KEGS_ADMIN_CHANGE_SPIRIT_REQ {
    std::int32_t m_iSpirit{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iSpirit)>>::put(w, m_iSpirit);
    }

    static KEGS_ADMIN_CHANGE_SPIRIT_REQ read_from(Reader& r) {
        KEGS_ADMIN_CHANGE_SPIRIT_REQ v;
        v.m_iSpirit = Codec<std::remove_cvref_t<decltype(v.m_iSpirit)>>::get(r);
        return v;
    }
};

struct KEGS_ADMIN_CHANGE_UNIT_CLASS_ACK {
    std::int32_t m_iOK{};  // int
    char         m_cUnitClass{};
    std::int32_t m_iSPoint{};  // int
    std::int32_t m_iCSPoint{};  // int
    std::int32_t m_iDefaultSkill1{};  // int
    std::int32_t m_iDefaultSkill2{};  // int
    std::int32_t m_iDefaultSkill3{};  // int
    std::int32_t m_iDefaultSkill4{};  // int
    std::int32_t m_iDefaultSkill5{};  // int
    std::int32_t m_iDefaultSkill6{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_cUnitClass)>>::put(w, m_cUnitClass);
        Codec<std::remove_cvref_t<decltype(m_iSPoint)>>::put(w, m_iSPoint);
        Codec<std::remove_cvref_t<decltype(m_iCSPoint)>>::put(w, m_iCSPoint);
        Codec<std::remove_cvref_t<decltype(m_iDefaultSkill1)>>::put(w, m_iDefaultSkill1);
        Codec<std::remove_cvref_t<decltype(m_iDefaultSkill2)>>::put(w, m_iDefaultSkill2);
        Codec<std::remove_cvref_t<decltype(m_iDefaultSkill3)>>::put(w, m_iDefaultSkill3);
        Codec<std::remove_cvref_t<decltype(m_iDefaultSkill4)>>::put(w, m_iDefaultSkill4);
        Codec<std::remove_cvref_t<decltype(m_iDefaultSkill5)>>::put(w, m_iDefaultSkill5);
        Codec<std::remove_cvref_t<decltype(m_iDefaultSkill6)>>::put(w, m_iDefaultSkill6);
    }

    static KEGS_ADMIN_CHANGE_UNIT_CLASS_ACK read_from(Reader& r) {
        KEGS_ADMIN_CHANGE_UNIT_CLASS_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_cUnitClass = Codec<std::remove_cvref_t<decltype(v.m_cUnitClass)>>::get(r);
        v.m_iSPoint = Codec<std::remove_cvref_t<decltype(v.m_iSPoint)>>::get(r);
        v.m_iCSPoint = Codec<std::remove_cvref_t<decltype(v.m_iCSPoint)>>::get(r);
        v.m_iDefaultSkill1 = Codec<std::remove_cvref_t<decltype(v.m_iDefaultSkill1)>>::get(r);
        v.m_iDefaultSkill2 = Codec<std::remove_cvref_t<decltype(v.m_iDefaultSkill2)>>::get(r);
        v.m_iDefaultSkill3 = Codec<std::remove_cvref_t<decltype(v.m_iDefaultSkill3)>>::get(r);
        v.m_iDefaultSkill4 = Codec<std::remove_cvref_t<decltype(v.m_iDefaultSkill4)>>::get(r);
        v.m_iDefaultSkill5 = Codec<std::remove_cvref_t<decltype(v.m_iDefaultSkill5)>>::get(r);
        v.m_iDefaultSkill6 = Codec<std::remove_cvref_t<decltype(v.m_iDefaultSkill6)>>::get(r);
        return v;
    }
};

struct KEGS_ADMIN_CHANGE_UNIT_CLASS_REQ {
    char m_cUnitClass{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cUnitClass)>>::put(w, m_cUnitClass);
    }

    static KEGS_ADMIN_CHANGE_UNIT_CLASS_REQ read_from(Reader& r) {
        KEGS_ADMIN_CHANGE_UNIT_CLASS_REQ v;
        v.m_cUnitClass = Codec<std::remove_cvref_t<decltype(v.m_cUnitClass)>>::get(r);
        return v;
    }
};

struct KEGS_ADMIN_CHANGE_VSPOINT_ACK {
    std::int32_t m_iOK{};  // int
    char         m_cRank{};
    std::int32_t m_iChangedRPoint{};  // int
    std::int32_t m_iChangedAPoint{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_cRank)>>::put(w, m_cRank);
        Codec<std::remove_cvref_t<decltype(m_iChangedRPoint)>>::put(w, m_iChangedRPoint);
        Codec<std::remove_cvref_t<decltype(m_iChangedAPoint)>>::put(w, m_iChangedAPoint);
    }

    static KEGS_ADMIN_CHANGE_VSPOINT_ACK read_from(Reader& r) {
        KEGS_ADMIN_CHANGE_VSPOINT_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_cRank = Codec<std::remove_cvref_t<decltype(v.m_cRank)>>::get(r);
        v.m_iChangedRPoint = Codec<std::remove_cvref_t<decltype(v.m_iChangedRPoint)>>::get(r);
        v.m_iChangedAPoint = Codec<std::remove_cvref_t<decltype(v.m_iChangedAPoint)>>::get(r);
        return v;
    }
};

struct KEGS_ADMIN_CHANGE_VSPOINT_REQ {
    std::int32_t m_iRating{};  // int
    std::int32_t m_iRPoint{};  // int
    std::int32_t m_iAPoint{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iRating)>>::put(w, m_iRating);
        Codec<std::remove_cvref_t<decltype(m_iRPoint)>>::put(w, m_iRPoint);
        Codec<std::remove_cvref_t<decltype(m_iAPoint)>>::put(w, m_iAPoint);
    }

    static KEGS_ADMIN_CHANGE_VSPOINT_REQ read_from(Reader& r) {
        KEGS_ADMIN_CHANGE_VSPOINT_REQ v;
        v.m_iRating = Codec<std::remove_cvref_t<decltype(v.m_iRating)>>::get(r);
        v.m_iRPoint = Codec<std::remove_cvref_t<decltype(v.m_iRPoint)>>::get(r);
        v.m_iAPoint = Codec<std::remove_cvref_t<decltype(v.m_iAPoint)>>::get(r);
        return v;
    }
};

struct KEGS_ADMIN_CHANNELING_CODE_CHEAT_NOT {
    std::int32_t m_iChannelingCode{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iChannelingCode)>>::put(w, m_iChannelingCode);
    }

    static KEGS_ADMIN_CHANNELING_CODE_CHEAT_NOT read_from(Reader& r) {
        KEGS_ADMIN_CHANNELING_CODE_CHEAT_NOT v;
        v.m_iChannelingCode = Codec<std::remove_cvref_t<decltype(v.m_iChannelingCode)>>::get(r);
        return v;
    }
};

struct KEGS_ADMIN_CHEAT_GET_ALL_SKILL_ACK {
    std::int32_t                             m_iOK{};  // int
    std::int32_t                             m_iActiveSkillPageNumber{};  // int
    std::vector<std::int16_t>                m_vecUnSealedSkill{};  // std::vector<short>
    std::map<std::int32_t, KAdminCheatSkill> m_mapSkillInfo{};  // std::map<int, KAdminCheatSkill>

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iActiveSkillPageNumber)>>::put(w, m_iActiveSkillPageNumber);
        Codec<std::remove_cvref_t<decltype(m_vecUnSealedSkill)>>::put(w, m_vecUnSealedSkill);
        Codec<std::remove_cvref_t<decltype(m_mapSkillInfo)>>::put(w, m_mapSkillInfo);
    }

    static KEGS_ADMIN_CHEAT_GET_ALL_SKILL_ACK read_from(Reader& r) {
        KEGS_ADMIN_CHEAT_GET_ALL_SKILL_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iActiveSkillPageNumber = Codec<std::remove_cvref_t<decltype(v.m_iActiveSkillPageNumber)>>::get(r);
        v.m_vecUnSealedSkill = Codec<std::remove_cvref_t<decltype(v.m_vecUnSealedSkill)>>::get(r);
        v.m_mapSkillInfo = Codec<std::remove_cvref_t<decltype(v.m_mapSkillInfo)>>::get(r);
        return v;
    }
};

struct KEGS_ADMIN_CHEAT_GET_ALL_SKILL_REQ {
    std::int32_t m_iActiveSkillPageNumber{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iActiveSkillPageNumber)>>::put(w, m_iActiveSkillPageNumber);
    }

    static KEGS_ADMIN_CHEAT_GET_ALL_SKILL_REQ read_from(Reader& r) {
        KEGS_ADMIN_CHEAT_GET_ALL_SKILL_REQ v;
        v.m_iActiveSkillPageNumber = Codec<std::remove_cvref_t<decltype(v.m_iActiveSkillPageNumber)>>::get(r);
        return v;
    }
};

struct KEGS_ADMIN_DELETE_GUILD_ADD_NOT {
    std::int32_t m_iGuildUID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iGuildUID)>>::put(w, m_iGuildUID);
    }

    static KEGS_ADMIN_DELETE_GUILD_ADD_NOT read_from(Reader& r) {
        KEGS_ADMIN_DELETE_GUILD_ADD_NOT v;
        v.m_iGuildUID = Codec<std::remove_cvref_t<decltype(v.m_iGuildUID)>>::get(r);
        return v;
    }
};

struct KEGS_ADMIN_DELETE_HENIR_RANKING_NICKNAME_NOT {
    std::int32_t   m_iRankingType{};  // int
    std::u16string m_wstrNickName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iRankingType)>>::put(w, m_iRankingType);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
    }

    static KEGS_ADMIN_DELETE_HENIR_RANKING_NICKNAME_NOT read_from(Reader& r) {
        KEGS_ADMIN_DELETE_HENIR_RANKING_NICKNAME_NOT v;
        v.m_iRankingType = Codec<std::remove_cvref_t<decltype(v.m_iRankingType)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        return v;
    }
};

struct KEGS_ADMIN_DELETE_HENIR_RANKING_RANK_NOT {
    std::int32_t m_iRankingType{};  // int
    std::int32_t m_iRank{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iRankingType)>>::put(w, m_iRankingType);
        Codec<std::remove_cvref_t<decltype(m_iRank)>>::put(w, m_iRank);
    }

    static KEGS_ADMIN_DELETE_HENIR_RANKING_RANK_NOT read_from(Reader& r) {
        KEGS_ADMIN_DELETE_HENIR_RANKING_RANK_NOT v;
        v.m_iRankingType = Codec<std::remove_cvref_t<decltype(v.m_iRankingType)>>::get(r);
        v.m_iRank = Codec<std::remove_cvref_t<decltype(v.m_iRank)>>::get(r);
        return v;
    }
};

struct KEGS_ADMIN_DELETE_HENIR_RANKING_UNITUID_NOT {
    std::int32_t m_iRankingType{};  // int
    std::int64_t m_iUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iRankingType)>>::put(w, m_iRankingType);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
    }

    static KEGS_ADMIN_DELETE_HENIR_RANKING_UNITUID_NOT read_from(Reader& r) {
        KEGS_ADMIN_DELETE_HENIR_RANKING_UNITUID_NOT v;
        v.m_iRankingType = Codec<std::remove_cvref_t<decltype(v.m_iRankingType)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        return v;
    }
};

struct KEGS_ADMIN_ED_MONITORING_CHEAT_NOT {
    bool         m_bEDMonitoringMode{};
    std::int32_t m_iMinED{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bEDMonitoringMode)>>::put(w, m_bEDMonitoringMode);
        Codec<std::remove_cvref_t<decltype(m_iMinED)>>::put(w, m_iMinED);
    }

    static KEGS_ADMIN_ED_MONITORING_CHEAT_NOT read_from(Reader& r) {
        KEGS_ADMIN_ED_MONITORING_CHEAT_NOT v;
        v.m_bEDMonitoringMode = Codec<std::remove_cvref_t<decltype(v.m_bEDMonitoringMode)>>::get(r);
        v.m_iMinED = Codec<std::remove_cvref_t<decltype(v.m_iMinED)>>::get(r);
        return v;
    }
};

struct KEGS_ADMIN_GET_AUTH_LEVEL_LIST_ACK {
    std::int32_t                m_iOK{};  // int
    std::vector<std::u16string> m_vecUserIDList{};  // std::vector< std::wstring >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_vecUserIDList)>>::put(w, m_vecUserIDList);
    }

    static KEGS_ADMIN_GET_AUTH_LEVEL_LIST_ACK read_from(Reader& r) {
        KEGS_ADMIN_GET_AUTH_LEVEL_LIST_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_vecUserIDList = Codec<std::remove_cvref_t<decltype(v.m_vecUserIDList)>>::get(r);
        return v;
    }
};

struct KEGS_ADMIN_GET_AUTH_LEVEL_LIST_REQ {
    char m_cAuthLevel{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cAuthLevel)>>::put(w, m_cAuthLevel);
    }

    static KEGS_ADMIN_GET_AUTH_LEVEL_LIST_REQ read_from(Reader& r) {
        KEGS_ADMIN_GET_AUTH_LEVEL_LIST_REQ v;
        v.m_cAuthLevel = Codec<std::remove_cvref_t<decltype(v.m_cAuthLevel)>>::get(r);
        return v;
    }
};

struct KEGS_ADMIN_GET_GUILD_SKILL_POINT_ACK {
    std::int32_t m_iOK{};  // int
    std::int32_t m_iCurGuildSPoint{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iCurGuildSPoint)>>::put(w, m_iCurGuildSPoint);
    }

    static KEGS_ADMIN_GET_GUILD_SKILL_POINT_ACK read_from(Reader& r) {
        KEGS_ADMIN_GET_GUILD_SKILL_POINT_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iCurGuildSPoint = Codec<std::remove_cvref_t<decltype(v.m_iCurGuildSPoint)>>::get(r);
        return v;
    }
};

struct KEGS_ADMIN_GET_GUILD_SKILL_POINT_REQ {
    std::int32_t m_iGuildSPoint{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iGuildSPoint)>>::put(w, m_iGuildSPoint);
    }

    static KEGS_ADMIN_GET_GUILD_SKILL_POINT_REQ read_from(Reader& r) {
        KEGS_ADMIN_GET_GUILD_SKILL_POINT_REQ v;
        v.m_iGuildSPoint = Codec<std::remove_cvref_t<decltype(v.m_iGuildSPoint)>>::get(r);
        return v;
    }
};

struct KEGS_ADMIN_GET_SKILL_ACK {
    std::int32_t m_iOK{};  // int
    std::int32_t m_iSkillID{};  // int
    std::int32_t m_iSkillLevel{};  // int
    std::int32_t m_iActiveSkillPageNumber{};  // int
    std::int32_t m_iCSPoint{};  // int
    bool         m_bUnsealed{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iSkillID)>>::put(w, m_iSkillID);
        Codec<std::remove_cvref_t<decltype(m_iSkillLevel)>>::put(w, m_iSkillLevel);
        Codec<std::remove_cvref_t<decltype(m_iActiveSkillPageNumber)>>::put(w, m_iActiveSkillPageNumber);
        Codec<std::remove_cvref_t<decltype(m_iCSPoint)>>::put(w, m_iCSPoint);
        Codec<std::remove_cvref_t<decltype(m_bUnsealed)>>::put(w, m_bUnsealed);
    }

    static KEGS_ADMIN_GET_SKILL_ACK read_from(Reader& r) {
        KEGS_ADMIN_GET_SKILL_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iSkillID = Codec<std::remove_cvref_t<decltype(v.m_iSkillID)>>::get(r);
        v.m_iSkillLevel = Codec<std::remove_cvref_t<decltype(v.m_iSkillLevel)>>::get(r);
        v.m_iActiveSkillPageNumber = Codec<std::remove_cvref_t<decltype(v.m_iActiveSkillPageNumber)>>::get(r);
        v.m_iCSPoint = Codec<std::remove_cvref_t<decltype(v.m_iCSPoint)>>::get(r);
        v.m_bUnsealed = Codec<std::remove_cvref_t<decltype(v.m_bUnsealed)>>::get(r);
        return v;
    }
};

struct KEGS_ADMIN_GET_SKILL_REQ {
    std::int32_t m_iSkillID{};  // int
    std::int32_t m_iSkillLevel{};  // int
    std::int32_t m_iActiveSkillPageNumber{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iSkillID)>>::put(w, m_iSkillID);
        Codec<std::remove_cvref_t<decltype(m_iSkillLevel)>>::put(w, m_iSkillLevel);
        Codec<std::remove_cvref_t<decltype(m_iActiveSkillPageNumber)>>::put(w, m_iActiveSkillPageNumber);
    }

    static KEGS_ADMIN_GET_SKILL_REQ read_from(Reader& r) {
        KEGS_ADMIN_GET_SKILL_REQ v;
        v.m_iSkillID = Codec<std::remove_cvref_t<decltype(v.m_iSkillID)>>::get(r);
        v.m_iSkillLevel = Codec<std::remove_cvref_t<decltype(v.m_iSkillLevel)>>::get(r);
        v.m_iActiveSkillPageNumber = Codec<std::remove_cvref_t<decltype(v.m_iActiveSkillPageNumber)>>::get(r);
        return v;
    }
};

struct KEGS_ADMIN_GET_TITLE_CHEAT_REQ {
    std::int32_t m_iTitleID{};  // int
    std::int32_t m_iPeriod{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iTitleID)>>::put(w, m_iTitleID);
        Codec<std::remove_cvref_t<decltype(m_iPeriod)>>::put(w, m_iPeriod);
    }

    static KEGS_ADMIN_GET_TITLE_CHEAT_REQ read_from(Reader& r) {
        KEGS_ADMIN_GET_TITLE_CHEAT_REQ v;
        v.m_iTitleID = Codec<std::remove_cvref_t<decltype(v.m_iTitleID)>>::get(r);
        v.m_iPeriod = Codec<std::remove_cvref_t<decltype(v.m_iPeriod)>>::get(r);
        return v;
    }
};

struct KEGS_ADMIN_GET_TOTAL_DANGEROUS_VALUE_ACK {
    std::int32_t m_iOK{};  // int
    std::int32_t m_iBattleFieldID{};  // int
    std::uint8_t m_byteTotalDangerousValue{};  // byte

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iBattleFieldID)>>::put(w, m_iBattleFieldID);
        Codec<std::remove_cvref_t<decltype(m_byteTotalDangerousValue)>>::put(w, m_byteTotalDangerousValue);
    }

    static KEGS_ADMIN_GET_TOTAL_DANGEROUS_VALUE_ACK read_from(Reader& r) {
        KEGS_ADMIN_GET_TOTAL_DANGEROUS_VALUE_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iBattleFieldID = Codec<std::remove_cvref_t<decltype(v.m_iBattleFieldID)>>::get(r);
        v.m_byteTotalDangerousValue = Codec<std::remove_cvref_t<decltype(v.m_byteTotalDangerousValue)>>::get(r);
        return v;
    }
};

struct KEGS_ADMIN_GET_TOTAL_DANGEROUS_VALUE_REQ {
    std::int32_t m_iBattleFieldID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iBattleFieldID)>>::put(w, m_iBattleFieldID);
    }

    static KEGS_ADMIN_GET_TOTAL_DANGEROUS_VALUE_REQ read_from(Reader& r) {
        KEGS_ADMIN_GET_TOTAL_DANGEROUS_VALUE_REQ v;
        v.m_iBattleFieldID = Codec<std::remove_cvref_t<decltype(v.m_iBattleFieldID)>>::get(r);
        return v;
    }
};

struct KEGS_ADMIN_HENIR_REWARD_CHEAT_NOT {
    std::int32_t m_iHenirRewardNormalCnt{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iHenirRewardNormalCnt)>>::put(w, m_iHenirRewardNormalCnt);
    }

    static KEGS_ADMIN_HENIR_REWARD_CHEAT_NOT read_from(Reader& r) {
        KEGS_ADMIN_HENIR_REWARD_CHEAT_NOT v;
        v.m_iHenirRewardNormalCnt = Codec<std::remove_cvref_t<decltype(v.m_iHenirRewardNormalCnt)>>::get(r);
        return v;
    }
};

struct KEGS_ADMIN_HERO_PVP_USER_ADD_REQ {
    std::vector<std::pair<std::int64_t, std::u16string>> m_vecUserList{};  // std::vector< std::pair< UidType, std::wstring > >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecUserList)>>::put(w, m_vecUserList);
    }

    static KEGS_ADMIN_HERO_PVP_USER_ADD_REQ read_from(Reader& r) {
        KEGS_ADMIN_HERO_PVP_USER_ADD_REQ v;
        v.m_vecUserList = Codec<std::remove_cvref_t<decltype(v.m_vecUserList)>>::get(r);
        return v;
    }
};

struct KEGS_ADMIN_INIT_SKILL_TREE_ACK {
    std::int32_t m_iOK{};  // int
    std::int32_t m_iActiveSkillPageNumber{};  // int
    std::int32_t m_iSPoint{};  // int
    std::int32_t m_iCSPoint{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iActiveSkillPageNumber)>>::put(w, m_iActiveSkillPageNumber);
        Codec<std::remove_cvref_t<decltype(m_iSPoint)>>::put(w, m_iSPoint);
        Codec<std::remove_cvref_t<decltype(m_iCSPoint)>>::put(w, m_iCSPoint);
    }

    static KEGS_ADMIN_INIT_SKILL_TREE_ACK read_from(Reader& r) {
        KEGS_ADMIN_INIT_SKILL_TREE_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iActiveSkillPageNumber = Codec<std::remove_cvref_t<decltype(v.m_iActiveSkillPageNumber)>>::get(r);
        v.m_iSPoint = Codec<std::remove_cvref_t<decltype(v.m_iSPoint)>>::get(r);
        v.m_iCSPoint = Codec<std::remove_cvref_t<decltype(v.m_iCSPoint)>>::get(r);
        return v;
    }
};

struct KEGS_ADMIN_INIT_SKILL_TREE_REQ {
    std::int32_t m_iActiveSkillPageNumber{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iActiveSkillPageNumber)>>::put(w, m_iActiveSkillPageNumber);
    }

    static KEGS_ADMIN_INIT_SKILL_TREE_REQ read_from(Reader& r) {
        KEGS_ADMIN_INIT_SKILL_TREE_REQ v;
        v.m_iActiveSkillPageNumber = Codec<std::remove_cvref_t<decltype(v.m_iActiveSkillPageNumber)>>::get(r);
        return v;
    }
};

struct KEGS_ADMIN_KICK_USER_REQ {
    std::u16string m_wstrUnitNickName{};  // std::wstring
    bool           m_bIsUserID{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrUnitNickName)>>::put(w, m_wstrUnitNickName);
        Codec<std::remove_cvref_t<decltype(m_bIsUserID)>>::put(w, m_bIsUserID);
    }

    static KEGS_ADMIN_KICK_USER_REQ read_from(Reader& r) {
        KEGS_ADMIN_KICK_USER_REQ v;
        v.m_wstrUnitNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrUnitNickName)>>::get(r);
        v.m_bIsUserID = Codec<std::remove_cvref_t<decltype(v.m_bIsUserID)>>::get(r);
        return v;
    }
};

struct KEGS_ADMIN_MODIFY_UNIT_LEVEL_REQ {
    std::u16string m_wstrUnitNickName{};  // std::wstring
    std::int32_t   m_iLevel{};  // int
    std::int32_t   m_iEXP{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrUnitNickName)>>::put(w, m_wstrUnitNickName);
        Codec<std::remove_cvref_t<decltype(m_iLevel)>>::put(w, m_iLevel);
        Codec<std::remove_cvref_t<decltype(m_iEXP)>>::put(w, m_iEXP);
    }

    static KEGS_ADMIN_MODIFY_UNIT_LEVEL_REQ read_from(Reader& r) {
        KEGS_ADMIN_MODIFY_UNIT_LEVEL_REQ v;
        v.m_wstrUnitNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrUnitNickName)>>::get(r);
        v.m_iLevel = Codec<std::remove_cvref_t<decltype(v.m_iLevel)>>::get(r);
        v.m_iEXP = Codec<std::remove_cvref_t<decltype(v.m_iEXP)>>::get(r);
        return v;
    }
};

struct KEGS_ADMIN_NOTIFY_HERO_PVP_USER_REQ {
    std::int64_t   m_iServerGroupID{};  // UidType
    std::u16string m_wstrNickName{};  // std::wstring
    std::u16string m_wstrMessage{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iServerGroupID)>>::put(w, m_iServerGroupID);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_wstrMessage)>>::put(w, m_wstrMessage);
    }

    static KEGS_ADMIN_NOTIFY_HERO_PVP_USER_REQ read_from(Reader& r) {
        KEGS_ADMIN_NOTIFY_HERO_PVP_USER_REQ v;
        v.m_iServerGroupID = Codec<std::remove_cvref_t<decltype(v.m_iServerGroupID)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_wstrMessage = Codec<std::remove_cvref_t<decltype(v.m_wstrMessage)>>::get(r);
        return v;
    }
};

struct KEGS_ADMIN_OBSERVER_MODE_REQ {
    bool m_bObserverMode{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bObserverMode)>>::put(w, m_bObserverMode);
    }

    static KEGS_ADMIN_OBSERVER_MODE_REQ read_from(Reader& r) {
        KEGS_ADMIN_OBSERVER_MODE_REQ v;
        v.m_bObserverMode = Codec<std::remove_cvref_t<decltype(v.m_bObserverMode)>>::get(r);
        return v;
    }
};

struct KEGS_ADMIN_PSHOP_AGENCY_CHEAT_REQ {
    std::int16_t m_sPeriod{};  // short

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_sPeriod)>>::put(w, m_sPeriod);
    }

    static KEGS_ADMIN_PSHOP_AGENCY_CHEAT_REQ read_from(Reader& r) {
        KEGS_ADMIN_PSHOP_AGENCY_CHEAT_REQ v;
        v.m_sPeriod = Codec<std::remove_cvref_t<decltype(v.m_sPeriod)>>::get(r);
        return v;
    }
};

struct KEGS_ADMIN_SERVER_STATUS_ACK {
    std::int32_t                         m_iOK{};  // int
    std::int32_t                         m_iCCU{};  // int
    std::map<std::int32_t, std::int32_t> m_mapNumPvPRoom{};  // std::map< int, int>
    std::int32_t                         m_iCurPartyCount{};  // int
    std::int32_t                         m_iPlayGamePartyCount{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iCCU)>>::put(w, m_iCCU);
        Codec<std::remove_cvref_t<decltype(m_mapNumPvPRoom)>>::put(w, m_mapNumPvPRoom);
        Codec<std::remove_cvref_t<decltype(m_iCurPartyCount)>>::put(w, m_iCurPartyCount);
        Codec<std::remove_cvref_t<decltype(m_iPlayGamePartyCount)>>::put(w, m_iPlayGamePartyCount);
    }

    static KEGS_ADMIN_SERVER_STATUS_ACK read_from(Reader& r) {
        KEGS_ADMIN_SERVER_STATUS_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iCCU = Codec<std::remove_cvref_t<decltype(v.m_iCCU)>>::get(r);
        v.m_mapNumPvPRoom = Codec<std::remove_cvref_t<decltype(v.m_mapNumPvPRoom)>>::get(r);
        v.m_iCurPartyCount = Codec<std::remove_cvref_t<decltype(v.m_iCurPartyCount)>>::get(r);
        v.m_iPlayGamePartyCount = Codec<std::remove_cvref_t<decltype(v.m_iPlayGamePartyCount)>>::get(r);
        return v;
    }
};

struct KEGS_ADMIN_SET_PC_BANG_REQ {
    bool         m_bOn{};
    std::int32_t m_iPcBangType{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bOn)>>::put(w, m_bOn);
        Codec<std::remove_cvref_t<decltype(m_iPcBangType)>>::put(w, m_iPcBangType);
    }

    static KEGS_ADMIN_SET_PC_BANG_REQ read_from(Reader& r) {
        KEGS_ADMIN_SET_PC_BANG_REQ v;
        v.m_bOn = Codec<std::remove_cvref_t<decltype(v.m_bOn)>>::get(r);
        v.m_iPcBangType = Codec<std::remove_cvref_t<decltype(v.m_iPcBangType)>>::get(r);
        return v;
    }
};

struct KEGS_ADMIN_SET_TOTAL_DANGEROUS_VALUE_REQ {
    std::int32_t m_iBattleFieldID{};  // int
    std::int32_t m_iTotalDangerousValue{};  // int
    bool         m_bSet{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iBattleFieldID)>>::put(w, m_iBattleFieldID);
        Codec<std::remove_cvref_t<decltype(m_iTotalDangerousValue)>>::put(w, m_iTotalDangerousValue);
        Codec<std::remove_cvref_t<decltype(m_bSet)>>::put(w, m_bSet);
    }

    static KEGS_ADMIN_SET_TOTAL_DANGEROUS_VALUE_REQ read_from(Reader& r) {
        KEGS_ADMIN_SET_TOTAL_DANGEROUS_VALUE_REQ v;
        v.m_iBattleFieldID = Codec<std::remove_cvref_t<decltype(v.m_iBattleFieldID)>>::get(r);
        v.m_iTotalDangerousValue = Codec<std::remove_cvref_t<decltype(v.m_iTotalDangerousValue)>>::get(r);
        v.m_bSet = Codec<std::remove_cvref_t<decltype(v.m_bSet)>>::get(r);
        return v;
    }
};

struct KEGS_ADMIN_SHOW_ED_CHEAT_ACK {
    std::int32_t   m_iOK{};  // int
    std::u16string m_wstrNickName{};  // std::wstring
    std::int32_t   m_iED{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_iED)>>::put(w, m_iED);
    }

    static KEGS_ADMIN_SHOW_ED_CHEAT_ACK read_from(Reader& r) {
        KEGS_ADMIN_SHOW_ED_CHEAT_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_iED = Codec<std::remove_cvref_t<decltype(v.m_iED)>>::get(r);
        return v;
    }
};

struct KEGS_ADMIN_SHOW_ED_CHEAT_REQ {
    std::u16string m_wstrNickName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
    }

    static KEGS_ADMIN_SHOW_ED_CHEAT_REQ read_from(Reader& r) {
        KEGS_ADMIN_SHOW_ED_CHEAT_REQ v;
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        return v;
    }
};

struct KEGS_AGREE_HACK_USER_ACK {
    std::int32_t m_iOK{};  // int
    char         m_cHackingUserType{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_cHackingUserType)>>::put(w, m_cHackingUserType);
    }

    static KEGS_AGREE_HACK_USER_ACK read_from(Reader& r) {
        KEGS_AGREE_HACK_USER_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_cHackingUserType = Codec<std::remove_cvref_t<decltype(v.m_cHackingUserType)>>::get(r);
        return v;
    }
};

struct KEGS_APPLY_COUPON_REQ {
    std::u16string m_wstrCouponSerial{};  // std::wstring
    std::int32_t   m_iCouponType{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrCouponSerial)>>::put(w, m_wstrCouponSerial);
        Codec<std::remove_cvref_t<decltype(m_iCouponType)>>::put(w, m_iCouponType);
    }

    static KEGS_APPLY_COUPON_REQ read_from(Reader& r) {
        KEGS_APPLY_COUPON_REQ v;
        v.m_wstrCouponSerial = Codec<std::remove_cvref_t<decltype(v.m_wstrCouponSerial)>>::get(r);
        v.m_iCouponType = Codec<std::remove_cvref_t<decltype(v.m_iCouponType)>>::get(r);
        return v;
    }
};

struct KEGS_APPLY_JOIN_GUILD_REQ {
    std::int32_t   m_iGuildUID{};  // int
    std::u16string m_wstrMessage{};  // std::wstring
    bool           m_bOldApplyDelete{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iGuildUID)>>::put(w, m_iGuildUID);
        Codec<std::remove_cvref_t<decltype(m_wstrMessage)>>::put(w, m_wstrMessage);
        Codec<std::remove_cvref_t<decltype(m_bOldApplyDelete)>>::put(w, m_bOldApplyDelete);
    }

    static KEGS_APPLY_JOIN_GUILD_REQ read_from(Reader& r) {
        KEGS_APPLY_JOIN_GUILD_REQ v;
        v.m_iGuildUID = Codec<std::remove_cvref_t<decltype(v.m_iGuildUID)>>::get(r);
        v.m_wstrMessage = Codec<std::remove_cvref_t<decltype(v.m_wstrMessage)>>::get(r);
        v.m_bOldApplyDelete = Codec<std::remove_cvref_t<decltype(v.m_bOldApplyDelete)>>::get(r);
        return v;
    }
};

struct KEGS_ATTRIB_ATTACH_ITEM_REQ {
    std::int64_t m_iDestItemUID{};  // UidType
    std::int64_t m_iAttachItemUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iDestItemUID)>>::put(w, m_iDestItemUID);
        Codec<std::remove_cvref_t<decltype(m_iAttachItemUID)>>::put(w, m_iAttachItemUID);
    }

    static KEGS_ATTRIB_ATTACH_ITEM_REQ read_from(Reader& r) {
        KEGS_ATTRIB_ATTACH_ITEM_REQ v;
        v.m_iDestItemUID = Codec<std::remove_cvref_t<decltype(v.m_iDestItemUID)>>::get(r);
        v.m_iAttachItemUID = Codec<std::remove_cvref_t<decltype(v.m_iAttachItemUID)>>::get(r);
        return v;
    }
};

struct KEGS_ATTRIB_ENCHANT_ITEM_REQ {
    std::int64_t m_iItemUID{};  // UidType
    char         m_cAttribEnchantSlotNo{};
    char         m_cAttribEnchantID{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_cAttribEnchantSlotNo)>>::put(w, m_cAttribEnchantSlotNo);
        Codec<std::remove_cvref_t<decltype(m_cAttribEnchantID)>>::put(w, m_cAttribEnchantID);
    }

    static KEGS_ATTRIB_ENCHANT_ITEM_REQ read_from(Reader& r) {
        KEGS_ATTRIB_ENCHANT_ITEM_REQ v;
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_cAttribEnchantSlotNo = Codec<std::remove_cvref_t<decltype(v.m_cAttribEnchantSlotNo)>>::get(r);
        v.m_cAttribEnchantID = Codec<std::remove_cvref_t<decltype(v.m_cAttribEnchantID)>>::get(r);
        return v;
    }
};

struct KEGS_AUTH_SECOND_SECURITY_ACK {
    std::int32_t m_iOK{};  // int
    std::int32_t m_iFailedCount{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iFailedCount)>>::put(w, m_iFailedCount);
    }

    static KEGS_AUTH_SECOND_SECURITY_ACK read_from(Reader& r) {
        KEGS_AUTH_SECOND_SECURITY_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iFailedCount = Codec<std::remove_cvref_t<decltype(v.m_iFailedCount)>>::get(r);
        return v;
    }
};

struct KEGS_AUTH_SECOND_SECURITY_REQ {
    std::int64_t   m_iUserUID{};  // UidType
    std::u16string m_wstrSecondPW{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
        Codec<std::remove_cvref_t<decltype(m_wstrSecondPW)>>::put(w, m_wstrSecondPW);
    }

    static KEGS_AUTH_SECOND_SECURITY_REQ read_from(Reader& r) {
        KEGS_AUTH_SECOND_SECURITY_REQ v;
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        v.m_wstrSecondPW = Codec<std::remove_cvref_t<decltype(v.m_wstrSecondPW)>>::get(r);
        return v;
    }
};

struct KEGS_AUTO_PARTY_CLOSE_NOT {
    std::int32_t m_iReason{};  // int
    std::int64_t m_iCancelUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iReason)>>::put(w, m_iReason);
        Codec<std::remove_cvref_t<decltype(m_iCancelUnitUID)>>::put(w, m_iCancelUnitUID);
    }

    static KEGS_AUTO_PARTY_CLOSE_NOT read_from(Reader& r) {
        KEGS_AUTO_PARTY_CLOSE_NOT v;
        v.m_iReason = Codec<std::remove_cvref_t<decltype(v.m_iReason)>>::get(r);
        v.m_iCancelUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iCancelUnitUID)>>::get(r);
        return v;
    }
};

struct KEGS_AUTO_PARTY_DUNGEON_GAME_ACK {
    std::int32_t   m_iOK{};  // int
    std::u16string m_wstrFailUserNickName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_wstrFailUserNickName)>>::put(w, m_wstrFailUserNickName);
    }

    static KEGS_AUTO_PARTY_DUNGEON_GAME_ACK read_from(Reader& r) {
        KEGS_AUTO_PARTY_DUNGEON_GAME_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_wstrFailUserNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrFailUserNickName)>>::get(r);
        return v;
    }
};

struct KEGS_AUTO_PARTY_MAKING_SUCCESS_NOT {
    std::int64_t m_iAutoPartyUID{};  // UidType
    std::int32_t m_iMemberCount{};  // int
    std::int32_t m_iDungeonID{};  // int
    char         m_cDifficultyLevel{};
    char         m_cDungeonMode{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iAutoPartyUID)>>::put(w, m_iAutoPartyUID);
        Codec<std::remove_cvref_t<decltype(m_iMemberCount)>>::put(w, m_iMemberCount);
        Codec<std::remove_cvref_t<decltype(m_iDungeonID)>>::put(w, m_iDungeonID);
        Codec<std::remove_cvref_t<decltype(m_cDifficultyLevel)>>::put(w, m_cDifficultyLevel);
        Codec<std::remove_cvref_t<decltype(m_cDungeonMode)>>::put(w, m_cDungeonMode);
    }

    static KEGS_AUTO_PARTY_MAKING_SUCCESS_NOT read_from(Reader& r) {
        KEGS_AUTO_PARTY_MAKING_SUCCESS_NOT v;
        v.m_iAutoPartyUID = Codec<std::remove_cvref_t<decltype(v.m_iAutoPartyUID)>>::get(r);
        v.m_iMemberCount = Codec<std::remove_cvref_t<decltype(v.m_iMemberCount)>>::get(r);
        v.m_iDungeonID = Codec<std::remove_cvref_t<decltype(v.m_iDungeonID)>>::get(r);
        v.m_cDifficultyLevel = Codec<std::remove_cvref_t<decltype(v.m_cDifficultyLevel)>>::get(r);
        v.m_cDungeonMode = Codec<std::remove_cvref_t<decltype(v.m_cDungeonMode)>>::get(r);
        return v;
    }
};

struct KEGS_AUTO_PARTY_MAKING_SUCCESS_REPLY_NOT {
    bool m_bGameStartAgree{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bGameStartAgree)>>::put(w, m_bGameStartAgree);
    }

    static KEGS_AUTO_PARTY_MAKING_SUCCESS_REPLY_NOT read_from(Reader& r) {
        KEGS_AUTO_PARTY_MAKING_SUCCESS_REPLY_NOT v;
        v.m_bGameStartAgree = Codec<std::remove_cvref_t<decltype(v.m_bGameStartAgree)>>::get(r);
        return v;
    }
};

struct KEGS_AUTO_PARTY_WAIT_USER_COUNT_NOT {
    std::int32_t m_iAutoPartyWaitUserCount{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iAutoPartyWaitUserCount)>>::put(w, m_iAutoPartyWaitUserCount);
    }

    static KEGS_AUTO_PARTY_WAIT_USER_COUNT_NOT read_from(Reader& r) {
        KEGS_AUTO_PARTY_WAIT_USER_COUNT_NOT v;
        v.m_iAutoPartyWaitUserCount = Codec<std::remove_cvref_t<decltype(v.m_iAutoPartyWaitUserCount)>>::get(r);
        return v;
    }
};

struct KEGS_AUTO_PAYMENT_RESURRECTION_STONE_NOT {
    std::int32_t m_iNumAutoPaymentResStone{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iNumAutoPaymentResStone)>>::put(w, m_iNumAutoPaymentResStone);
    }

    static KEGS_AUTO_PAYMENT_RESURRECTION_STONE_NOT read_from(Reader& r) {
        KEGS_AUTO_PAYMENT_RESURRECTION_STONE_NOT v;
        v.m_iNumAutoPaymentResStone = Codec<std::remove_cvref_t<decltype(v.m_iNumAutoPaymentResStone)>>::get(r);
        return v;
    }
};

struct KEGS_BAD_ATTITUDE_USER_CHECK_INFO_NOT {
    std::int32_t m_iStageID{};  // int
    std::int32_t m_iSubStageID{};  // int
    std::int32_t m_iComboScore{};  // int
    std::int32_t m_iTechScore{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iStageID)>>::put(w, m_iStageID);
        Codec<std::remove_cvref_t<decltype(m_iSubStageID)>>::put(w, m_iSubStageID);
        Codec<std::remove_cvref_t<decltype(m_iComboScore)>>::put(w, m_iComboScore);
        Codec<std::remove_cvref_t<decltype(m_iTechScore)>>::put(w, m_iTechScore);
    }

    static KEGS_BAD_ATTITUDE_USER_CHECK_INFO_NOT read_from(Reader& r) {
        KEGS_BAD_ATTITUDE_USER_CHECK_INFO_NOT v;
        v.m_iStageID = Codec<std::remove_cvref_t<decltype(v.m_iStageID)>>::get(r);
        v.m_iSubStageID = Codec<std::remove_cvref_t<decltype(v.m_iSubStageID)>>::get(r);
        v.m_iComboScore = Codec<std::remove_cvref_t<decltype(v.m_iComboScore)>>::get(r);
        v.m_iTechScore = Codec<std::remove_cvref_t<decltype(v.m_iTechScore)>>::get(r);
        return v;
    }
};

struct KEGS_BAD_ATTITUDE_USER_MSG_NOT {
    std::int64_t m_iUserUID{};  // UidType
    char         m_MsgType{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
        Codec<std::remove_cvref_t<decltype(m_MsgType)>>::put(w, m_MsgType);
    }

    static KEGS_BAD_ATTITUDE_USER_MSG_NOT read_from(Reader& r) {
        KEGS_BAD_ATTITUDE_USER_MSG_NOT v;
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        v.m_MsgType = Codec<std::remove_cvref_t<decltype(v.m_MsgType)>>::get(r);
        return v;
    }
};

struct KEGS_BAD_ATTITUDE_USER_NOT {
    std::int64_t m_iUserUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
    }

    static KEGS_BAD_ATTITUDE_USER_NOT read_from(Reader& r) {
        KEGS_BAD_ATTITUDE_USER_NOT v;
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        return v;
    }
};

struct KEGS_BAN_USER_REQ {
    std::int64_t m_iUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
    }

    static KEGS_BAN_USER_REQ read_from(Reader& r) {
        KEGS_BAN_USER_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        return v;
    }
};

struct KEGS_BATTLE_FIELD_BOSS_GATE_CLOSE_NOT {
    std::int32_t m_iBattleFieldID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iBattleFieldID)>>::put(w, m_iBattleFieldID);
    }

    static KEGS_BATTLE_FIELD_BOSS_GATE_CLOSE_NOT read_from(Reader& r) {
        KEGS_BATTLE_FIELD_BOSS_GATE_CLOSE_NOT v;
        v.m_iBattleFieldID = Codec<std::remove_cvref_t<decltype(v.m_iBattleFieldID)>>::get(r);
        return v;
    }
};

struct KEGS_BATTLE_FIELD_BOSS_GATE_OPEN_NOT {
    std::int32_t m_iBattleFieldID{};  // int
    std::int32_t m_iBossFieldID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iBattleFieldID)>>::put(w, m_iBattleFieldID);
        Codec<std::remove_cvref_t<decltype(m_iBossFieldID)>>::put(w, m_iBossFieldID);
    }

    static KEGS_BATTLE_FIELD_BOSS_GATE_OPEN_NOT read_from(Reader& r) {
        KEGS_BATTLE_FIELD_BOSS_GATE_OPEN_NOT v;
        v.m_iBattleFieldID = Codec<std::remove_cvref_t<decltype(v.m_iBattleFieldID)>>::get(r);
        v.m_iBossFieldID = Codec<std::remove_cvref_t<decltype(v.m_iBossFieldID)>>::get(r);
        return v;
    }
};

struct KEGS_BATTLE_FIELD_DANGEROUS_VALUE_NOT {
    std::int32_t m_iDangerousValue{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iDangerousValue)>>::put(w, m_iDangerousValue);
    }

    static KEGS_BATTLE_FIELD_DANGEROUS_VALUE_NOT read_from(Reader& r) {
        KEGS_BATTLE_FIELD_DANGEROUS_VALUE_NOT v;
        v.m_iDangerousValue = Codec<std::remove_cvref_t<decltype(v.m_iDangerousValue)>>::get(r);
        return v;
    }
};

struct KEGS_BATTLE_FIELD_EVENT_MESSAGE_NOT {
    char m_cEventType{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cEventType)>>::put(w, m_cEventType);
    }

    static KEGS_BATTLE_FIELD_EVENT_MESSAGE_NOT read_from(Reader& r) {
        KEGS_BATTLE_FIELD_EVENT_MESSAGE_NOT v;
        v.m_cEventType = Codec<std::remove_cvref_t<decltype(v.m_cEventType)>>::get(r);
        return v;
    }
};

struct KEGS_BATTLE_FIELD_JOIN_USER_LOAD_COMPLETE_NOT {
    std::int64_t m_iLoadedUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iLoadedUnitUID)>>::put(w, m_iLoadedUnitUID);
    }

    static KEGS_BATTLE_FIELD_JOIN_USER_LOAD_COMPLETE_NOT read_from(Reader& r) {
        KEGS_BATTLE_FIELD_JOIN_USER_LOAD_COMPLETE_NOT v;
        v.m_iLoadedUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iLoadedUnitUID)>>::get(r);
        return v;
    }
};

struct KEGS_BATTLE_FIELD_NPC_DAMAGED_MAP_ACK {
    std::int64_t                  m_iUnitUID{};  // UidType
    std::int32_t                  m_iNPCUID{};  // int
    std::map<std::int64_t, float> m_mapDamagedMap{};  // std::map< UidType, float >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iNPCUID)>>::put(w, m_iNPCUID);
        Codec<std::remove_cvref_t<decltype(m_mapDamagedMap)>>::put(w, m_mapDamagedMap);
    }

    static KEGS_BATTLE_FIELD_NPC_DAMAGED_MAP_ACK read_from(Reader& r) {
        KEGS_BATTLE_FIELD_NPC_DAMAGED_MAP_ACK v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iNPCUID = Codec<std::remove_cvref_t<decltype(v.m_iNPCUID)>>::get(r);
        v.m_mapDamagedMap = Codec<std::remove_cvref_t<decltype(v.m_mapDamagedMap)>>::get(r);
        return v;
    }
};

struct KEGS_BATTLE_FIELD_NPC_DAMAGED_MAP_REQ {
    std::int64_t m_iUnitUID{};  // UidType
    std::int32_t m_iNPCUID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iNPCUID)>>::put(w, m_iNPCUID);
    }

    static KEGS_BATTLE_FIELD_NPC_DAMAGED_MAP_REQ read_from(Reader& r) {
        KEGS_BATTLE_FIELD_NPC_DAMAGED_MAP_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iNPCUID = Codec<std::remove_cvref_t<decltype(v.m_iNPCUID)>>::get(r);
        return v;
    }
};

struct KEGS_BATTLE_FIELD_NPC_P2P_SYNC_NOT {
    std::vector<std::int64_t> m_vecNonNpcSyncUserList{};  // std::vector< UidType >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecNonNpcSyncUserList)>>::put(w, m_vecNonNpcSyncUserList);
    }

    static KEGS_BATTLE_FIELD_NPC_P2P_SYNC_NOT read_from(Reader& r) {
        KEGS_BATTLE_FIELD_NPC_P2P_SYNC_NOT v;
        v.m_vecNonNpcSyncUserList = Codec<std::remove_cvref_t<decltype(v.m_vecNonNpcSyncUserList)>>::get(r);
        return v;
    }
};

struct KEGS_BILL_GET_PURCHASED_CASH_ITEM_REQ {
    std::int32_t                         m_iTransNo{};  // int
    std::int32_t                         m_iProductNo{};  // int
    std::map<std::int32_t, std::int32_t> m_mapSocketForCash{};  // std::map< int, int >
    std::map<std::int32_t, std::int32_t> m_mapSocketGroupIDForCash{};  // std::map< int, int >
    std::uint8_t                         m_byteProductKind{};  // BYTE

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iTransNo)>>::put(w, m_iTransNo);
        Codec<std::remove_cvref_t<decltype(m_iProductNo)>>::put(w, m_iProductNo);
        Codec<std::remove_cvref_t<decltype(m_mapSocketForCash)>>::put(w, m_mapSocketForCash);
        Codec<std::remove_cvref_t<decltype(m_mapSocketGroupIDForCash)>>::put(w, m_mapSocketGroupIDForCash);
        Codec<std::remove_cvref_t<decltype(m_byteProductKind)>>::put(w, m_byteProductKind);
    }

    static KEGS_BILL_GET_PURCHASED_CASH_ITEM_REQ read_from(Reader& r) {
        KEGS_BILL_GET_PURCHASED_CASH_ITEM_REQ v;
        v.m_iTransNo = Codec<std::remove_cvref_t<decltype(v.m_iTransNo)>>::get(r);
        v.m_iProductNo = Codec<std::remove_cvref_t<decltype(v.m_iProductNo)>>::get(r);
        v.m_mapSocketForCash = Codec<std::remove_cvref_t<decltype(v.m_mapSocketForCash)>>::get(r);
        v.m_mapSocketGroupIDForCash = Codec<std::remove_cvref_t<decltype(v.m_mapSocketGroupIDForCash)>>::get(r);
        v.m_byteProductKind = Codec<std::remove_cvref_t<decltype(v.m_byteProductKind)>>::get(r);
        return v;
    }
};

struct KEGS_BILL_INVENTORY_INQUIRY_ACK {
    std::int32_t                  m_iOK{};  // int
    std::vector<KBillOrderInfo>   m_vecOrderInfo{};  // std::vector< KBillOrderInfo >
    std::vector<KBillPackageInfo> m_vecPackageInfo{};  // std::vector< KBillPackageInfo >
    std::vector<std::int32_t>     m_vecSubPackageTrans{};  // std::vector< int >
    std::vector<std::int32_t>     m_vecSubPackageInfo{};  // std::vector< int >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_vecOrderInfo)>>::put(w, m_vecOrderInfo);
        Codec<std::remove_cvref_t<decltype(m_vecPackageInfo)>>::put(w, m_vecPackageInfo);
        Codec<std::remove_cvref_t<decltype(m_vecSubPackageTrans)>>::put(w, m_vecSubPackageTrans);
        Codec<std::remove_cvref_t<decltype(m_vecSubPackageInfo)>>::put(w, m_vecSubPackageInfo);
    }

    static KEGS_BILL_INVENTORY_INQUIRY_ACK read_from(Reader& r) {
        KEGS_BILL_INVENTORY_INQUIRY_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_vecOrderInfo = Codec<std::remove_cvref_t<decltype(v.m_vecOrderInfo)>>::get(r);
        v.m_vecPackageInfo = Codec<std::remove_cvref_t<decltype(v.m_vecPackageInfo)>>::get(r);
        v.m_vecSubPackageTrans = Codec<std::remove_cvref_t<decltype(v.m_vecSubPackageTrans)>>::get(r);
        v.m_vecSubPackageInfo = Codec<std::remove_cvref_t<decltype(v.m_vecSubPackageInfo)>>::get(r);
        return v;
    }
};

struct KEGS_BILL_INVENTORY_INQUIRY_REQ {
    std::int32_t m_nItemPerPage{};  // int
    std::int32_t m_iCurrentPage{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_nItemPerPage)>>::put(w, m_nItemPerPage);
        Codec<std::remove_cvref_t<decltype(m_iCurrentPage)>>::put(w, m_iCurrentPage);
    }

    static KEGS_BILL_INVENTORY_INQUIRY_REQ read_from(Reader& r) {
        KEGS_BILL_INVENTORY_INQUIRY_REQ v;
        v.m_nItemPerPage = Codec<std::remove_cvref_t<decltype(v.m_nItemPerPage)>>::get(r);
        v.m_iCurrentPage = Codec<std::remove_cvref_t<decltype(v.m_iCurrentPage)>>::get(r);
        return v;
    }
};

struct KEGS_BILL_PRODUCT_INFO_REQ {
    std::int32_t m_iPage{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iPage)>>::put(w, m_iPage);
    }

    static KEGS_BILL_PRODUCT_INFO_REQ read_from(Reader& r) {
        KEGS_BILL_PRODUCT_INFO_REQ v;
        v.m_iPage = Codec<std::remove_cvref_t<decltype(v.m_iPage)>>::get(r);
        return v;
    }
};

struct KEGS_BLOCK_FRIEND_ACK {
    std::int32_t m_iOK{};  // int
    std::int64_t m_iUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
    }

    static KEGS_BLOCK_FRIEND_ACK read_from(Reader& r) {
        KEGS_BLOCK_FRIEND_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        return v;
    }
};

struct KEGS_BLOCK_FRIEND_NOT {
    std::int64_t m_iUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
    }

    static KEGS_BLOCK_FRIEND_NOT read_from(Reader& r) {
        KEGS_BLOCK_FRIEND_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        return v;
    }
};

struct KEGS_BONUS_CASH_EVENT_NOT {
    bool m_bEventStart{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bEventStart)>>::put(w, m_bEventStart);
    }

    static KEGS_BONUS_CASH_EVENT_NOT read_from(Reader& r) {
        KEGS_BONUS_CASH_EVENT_NOT v;
        v.m_bEventStart = Codec<std::remove_cvref_t<decltype(v.m_bEventStart)>>::get(r);
        return v;
    }
};

struct KEGS_BOSS_FIELD_LOG_NOT {
    std::map<std::int64_t, std::int64_t> m_mapGivenDamage{};  // std::map<UidType, __int64>
    std::map<std::int64_t, std::int64_t> m_mapAttackDamage{};  // std::map<UidType, __int64>

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_mapGivenDamage)>>::put(w, m_mapGivenDamage);
        Codec<std::remove_cvref_t<decltype(m_mapAttackDamage)>>::put(w, m_mapAttackDamage);
    }

    static KEGS_BOSS_FIELD_LOG_NOT read_from(Reader& r) {
        KEGS_BOSS_FIELD_LOG_NOT v;
        v.m_mapGivenDamage = Codec<std::remove_cvref_t<decltype(v.m_mapGivenDamage)>>::get(r);
        v.m_mapAttackDamage = Codec<std::remove_cvref_t<decltype(v.m_mapAttackDamage)>>::get(r);
        return v;
    }
};

struct KEGS_BREAK_PERSONAL_SHOP_NOT {
    std::int32_t m_iReason{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iReason)>>::put(w, m_iReason);
    }

    static KEGS_BREAK_PERSONAL_SHOP_NOT read_from(Reader& r) {
        KEGS_BREAK_PERSONAL_SHOP_NOT v;
        v.m_iReason = Codec<std::remove_cvref_t<decltype(v.m_iReason)>>::get(r);
        return v;
    }
};

struct KEGS_BREAK_PERSONAL_SHOP_REQ {
    std::int32_t m_iReason{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iReason)>>::put(w, m_iReason);
    }

    static KEGS_BREAK_PERSONAL_SHOP_REQ read_from(Reader& r) {
        KEGS_BREAK_PERSONAL_SHOP_REQ v;
        v.m_iReason = Codec<std::remove_cvref_t<decltype(v.m_iReason)>>::get(r);
        return v;
    }
};

struct KEGS_BREAK_UP_NOT {
    std::int32_t m_iCost{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iCost)>>::put(w, m_iCost);
    }

    static KEGS_BREAK_UP_NOT read_from(Reader& r) {
        KEGS_BREAK_UP_NOT v;
        v.m_iCost = Codec<std::remove_cvref_t<decltype(v.m_iCost)>>::get(r);
        return v;
    }
};

struct KEGS_BUFF_SKILL_INFO_CONVEY_NOT {
    std::map<std::int32_t, std::vector<KBuffSkillInfo>> m_mapCaughtUIDList{};  // std::map< int, std::vector< KBuffSkillInfo > >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_mapCaughtUIDList)>>::put(w, m_mapCaughtUIDList);
    }

    static KEGS_BUFF_SKILL_INFO_CONVEY_NOT read_from(Reader& r) {
        KEGS_BUFF_SKILL_INFO_CONVEY_NOT v;
        v.m_mapCaughtUIDList = Codec<std::remove_cvref_t<decltype(v.m_mapCaughtUIDList)>>::get(r);
        return v;
    }
};

struct KEGS_BUFF_SKILL_INFO_CONVEY_REQ {
    std::int32_t              m_iBuffEnum{};  // int
    std::int64_t              m_iCastingUnitUID{};  // UidType
    float                     m_fBuffTime{};
    bool                      m_bIsBuffCasting{};
    char                      m_cBuffLevel{};
    std::vector<std::int64_t> m_vecCaughtUIDList{};  // std::vector< UidType >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iBuffEnum)>>::put(w, m_iBuffEnum);
        Codec<std::remove_cvref_t<decltype(m_iCastingUnitUID)>>::put(w, m_iCastingUnitUID);
        Codec<std::remove_cvref_t<decltype(m_fBuffTime)>>::put(w, m_fBuffTime);
        Codec<std::remove_cvref_t<decltype(m_bIsBuffCasting)>>::put(w, m_bIsBuffCasting);
        Codec<std::remove_cvref_t<decltype(m_cBuffLevel)>>::put(w, m_cBuffLevel);
        Codec<std::remove_cvref_t<decltype(m_vecCaughtUIDList)>>::put(w, m_vecCaughtUIDList);
    }

    static KEGS_BUFF_SKILL_INFO_CONVEY_REQ read_from(Reader& r) {
        KEGS_BUFF_SKILL_INFO_CONVEY_REQ v;
        v.m_iBuffEnum = Codec<std::remove_cvref_t<decltype(v.m_iBuffEnum)>>::get(r);
        v.m_iCastingUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iCastingUnitUID)>>::get(r);
        v.m_fBuffTime = Codec<std::remove_cvref_t<decltype(v.m_fBuffTime)>>::get(r);
        v.m_bIsBuffCasting = Codec<std::remove_cvref_t<decltype(v.m_bIsBuffCasting)>>::get(r);
        v.m_cBuffLevel = Codec<std::remove_cvref_t<decltype(v.m_cBuffLevel)>>::get(r);
        v.m_vecCaughtUIDList = Codec<std::remove_cvref_t<decltype(v.m_vecCaughtUIDList)>>::get(r);
        return v;
    }
};

struct KEGS_BUY_CASH_ITEM_AUTOMATIC_PAYMENT_ACK {
    std::int32_t   m_iOK{};  // int
    std::uint32_t  m_uiUserSN{};  // UINT
    std::uint8_t   m_byDomainType{};  // BYTE
    std::u16string m_wstrTransactionID{};  // std::wstring
    std::uint32_t  m_ulProductNo{};  // unsigned long
    std::uint32_t  m_ulSalePrice{};  // unsigned long
    std::uint16_t  m_usOrderQuantity{};  // unsigned short

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_uiUserSN)>>::put(w, m_uiUserSN);
        Codec<std::remove_cvref_t<decltype(m_byDomainType)>>::put(w, m_byDomainType);
        Codec<std::remove_cvref_t<decltype(m_wstrTransactionID)>>::put(w, m_wstrTransactionID);
        Codec<std::remove_cvref_t<decltype(m_ulProductNo)>>::put(w, m_ulProductNo);
        Codec<std::remove_cvref_t<decltype(m_ulSalePrice)>>::put(w, m_ulSalePrice);
        Codec<std::remove_cvref_t<decltype(m_usOrderQuantity)>>::put(w, m_usOrderQuantity);
    }

    static KEGS_BUY_CASH_ITEM_AUTOMATIC_PAYMENT_ACK read_from(Reader& r) {
        KEGS_BUY_CASH_ITEM_AUTOMATIC_PAYMENT_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_uiUserSN = Codec<std::remove_cvref_t<decltype(v.m_uiUserSN)>>::get(r);
        v.m_byDomainType = Codec<std::remove_cvref_t<decltype(v.m_byDomainType)>>::get(r);
        v.m_wstrTransactionID = Codec<std::remove_cvref_t<decltype(v.m_wstrTransactionID)>>::get(r);
        v.m_ulProductNo = Codec<std::remove_cvref_t<decltype(v.m_ulProductNo)>>::get(r);
        v.m_ulSalePrice = Codec<std::remove_cvref_t<decltype(v.m_ulSalePrice)>>::get(r);
        v.m_usOrderQuantity = Codec<std::remove_cvref_t<decltype(v.m_usOrderQuantity)>>::get(r);
        return v;
    }
};

struct KEGS_BUY_CASH_ITEM_AUTOMATIC_PAYMENT_REQ {
    std::uint32_t m_ulProductNo{};  // unsigned long
    std::uint16_t m_usOrderQuantity{};  // unsigned short

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_ulProductNo)>>::put(w, m_ulProductNo);
        Codec<std::remove_cvref_t<decltype(m_usOrderQuantity)>>::put(w, m_usOrderQuantity);
    }

    static KEGS_BUY_CASH_ITEM_AUTOMATIC_PAYMENT_REQ read_from(Reader& r) {
        KEGS_BUY_CASH_ITEM_AUTOMATIC_PAYMENT_REQ v;
        v.m_ulProductNo = Codec<std::remove_cvref_t<decltype(v.m_ulProductNo)>>::get(r);
        v.m_usOrderQuantity = Codec<std::remove_cvref_t<decltype(v.m_usOrderQuantity)>>::get(r);
        return v;
    }
};

struct KEGS_BUY_ED_ITEM_REQ {
    bool                                 m_bDebug{};
    std::int16_t                         m_sPeriod{};  // short
    std::map<std::int32_t, std::int32_t> m_mapItem{};  // std::map< int, int >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bDebug)>>::put(w, m_bDebug);
        Codec<std::remove_cvref_t<decltype(m_sPeriod)>>::put(w, m_sPeriod);
        Codec<std::remove_cvref_t<decltype(m_mapItem)>>::put(w, m_mapItem);
    }

    static KEGS_BUY_ED_ITEM_REQ read_from(Reader& r) {
        KEGS_BUY_ED_ITEM_REQ v;
        v.m_bDebug = Codec<std::remove_cvref_t<decltype(v.m_bDebug)>>::get(r);
        v.m_sPeriod = Codec<std::remove_cvref_t<decltype(v.m_sPeriod)>>::get(r);
        v.m_mapItem = Codec<std::remove_cvref_t<decltype(v.m_mapItem)>>::get(r);
        return v;
    }
};

struct KEGS_BUY_PERSONAL_SHOP_ITEM_REQ {
    std::int64_t m_iItemUID{};  // UidType
    std::int32_t m_iItemID{};  // int
    std::int32_t m_iQuantity{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_iItemID)>>::put(w, m_iItemID);
        Codec<std::remove_cvref_t<decltype(m_iQuantity)>>::put(w, m_iQuantity);
    }

    static KEGS_BUY_PERSONAL_SHOP_ITEM_REQ read_from(Reader& r) {
        KEGS_BUY_PERSONAL_SHOP_ITEM_REQ v;
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_iItemID = Codec<std::remove_cvref_t<decltype(v.m_iItemID)>>::get(r);
        v.m_iQuantity = Codec<std::remove_cvref_t<decltype(v.m_iQuantity)>>::get(r);
        return v;
    }
};

struct KEGS_CASH_ITEM_REFUND_REQ {
    std::uint32_t m_ulOrderNo{};  // unsigned long
    std::uint32_t m_ulProductNo{};  // unsigned long
    std::int32_t  m_iQuantity{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_ulOrderNo)>>::put(w, m_ulOrderNo);
        Codec<std::remove_cvref_t<decltype(m_ulProductNo)>>::put(w, m_ulProductNo);
        Codec<std::remove_cvref_t<decltype(m_iQuantity)>>::put(w, m_iQuantity);
    }

    static KEGS_CASH_ITEM_REFUND_REQ read_from(Reader& r) {
        KEGS_CASH_ITEM_REFUND_REQ v;
        v.m_ulOrderNo = Codec<std::remove_cvref_t<decltype(v.m_ulOrderNo)>>::get(r);
        v.m_ulProductNo = Codec<std::remove_cvref_t<decltype(v.m_ulProductNo)>>::get(r);
        v.m_iQuantity = Codec<std::remove_cvref_t<decltype(v.m_iQuantity)>>::get(r);
        return v;
    }
};

struct KEGS_CASH_PRODUCT_INFO_REQ {
    std::int32_t m_iPage{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iPage)>>::put(w, m_iPage);
    }

    static KEGS_CASH_PRODUCT_INFO_REQ read_from(Reader& r) {
        KEGS_CASH_PRODUCT_INFO_REQ v;
        v.m_iPage = Codec<std::remove_cvref_t<decltype(v.m_iPage)>>::get(r);
        return v;
    }
};

struct KEGS_CASH_SHOP_OPEN_NOT {
    bool m_bCashShopOpen{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bCashShopOpen)>>::put(w, m_bCashShopOpen);
    }

    static KEGS_CASH_SHOP_OPEN_NOT read_from(Reader& r) {
        KEGS_CASH_SHOP_OPEN_NOT v;
        v.m_bCashShopOpen = Codec<std::remove_cvref_t<decltype(v.m_bCashShopOpen)>>::get(r);
        return v;
    }
};

struct KEGS_CATCH_HACKUSER_INFO_NOT {
    std::int64_t m_iUserUID{};  // UidType
    std::int64_t m_iUnitUID{};  // UidType
    std::int32_t m_iCrashType{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iCrashType)>>::put(w, m_iCrashType);
    }

    static KEGS_CATCH_HACKUSER_INFO_NOT read_from(Reader& r) {
        KEGS_CATCH_HACKUSER_INFO_NOT v;
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iCrashType = Codec<std::remove_cvref_t<decltype(v.m_iCrashType)>>::get(r);
        return v;
    }
};

struct KEGS_CHANGED_PET_INTIMACY_NOT {
    std::int64_t m_iUnitUID{};  // UidType
    std::int64_t m_iPetUID{};  // UidType
    std::int32_t m_iCurrentIntimacy{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iPetUID)>>::put(w, m_iPetUID);
        Codec<std::remove_cvref_t<decltype(m_iCurrentIntimacy)>>::put(w, m_iCurrentIntimacy);
    }

    static KEGS_CHANGED_PET_INTIMACY_NOT read_from(Reader& r) {
        KEGS_CHANGED_PET_INTIMACY_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iPetUID = Codec<std::remove_cvref_t<decltype(v.m_iPetUID)>>::get(r);
        v.m_iCurrentIntimacy = Codec<std::remove_cvref_t<decltype(v.m_iCurrentIntimacy)>>::get(r);
        return v;
    }
};

struct KEGS_CHANGED_PET_SATIETY_INTIMACY_NOT {
    std::int64_t m_iUnitUID{};  // UidType
    std::int64_t m_iPetUID{};  // UidType
    std::int16_t m_sCurrentSatiety{};  // short
    std::int32_t m_iCurrentIntimacy{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iPetUID)>>::put(w, m_iPetUID);
        Codec<std::remove_cvref_t<decltype(m_sCurrentSatiety)>>::put(w, m_sCurrentSatiety);
        Codec<std::remove_cvref_t<decltype(m_iCurrentIntimacy)>>::put(w, m_iCurrentIntimacy);
    }

    static KEGS_CHANGED_PET_SATIETY_INTIMACY_NOT read_from(Reader& r) {
        KEGS_CHANGED_PET_SATIETY_INTIMACY_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iPetUID = Codec<std::remove_cvref_t<decltype(v.m_iPetUID)>>::get(r);
        v.m_sCurrentSatiety = Codec<std::remove_cvref_t<decltype(v.m_sCurrentSatiety)>>::get(r);
        v.m_iCurrentIntimacy = Codec<std::remove_cvref_t<decltype(v.m_iCurrentIntimacy)>>::get(r);
        return v;
    }
};

struct KEGS_CHANGED_PET_SATIETY_NOT {
    std::int64_t m_iUnitUID{};  // UidType
    std::int64_t m_iPetUID{};  // UidType
    std::int16_t m_sCurrentSatiety{};  // short

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iPetUID)>>::put(w, m_iPetUID);
        Codec<std::remove_cvref_t<decltype(m_sCurrentSatiety)>>::put(w, m_sCurrentSatiety);
    }

    static KEGS_CHANGED_PET_SATIETY_NOT read_from(Reader& r) {
        KEGS_CHANGED_PET_SATIETY_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iPetUID = Codec<std::remove_cvref_t<decltype(v.m_iPetUID)>>::get(r);
        v.m_sCurrentSatiety = Codec<std::remove_cvref_t<decltype(v.m_sCurrentSatiety)>>::get(r);
        return v;
    }
};

struct KEGS_CHANGE_GUILD_MEMBER_GRADE_ACK {
    std::int32_t   m_iOK{};  // int
    std::u16string m_wstrTargetNickName{};  // std::wstring
    std::uint8_t   m_ucMemberShipGrade{};  // u_char

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_wstrTargetNickName)>>::put(w, m_wstrTargetNickName);
        Codec<std::remove_cvref_t<decltype(m_ucMemberShipGrade)>>::put(w, m_ucMemberShipGrade);
    }

    static KEGS_CHANGE_GUILD_MEMBER_GRADE_ACK read_from(Reader& r) {
        KEGS_CHANGE_GUILD_MEMBER_GRADE_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_wstrTargetNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrTargetNickName)>>::get(r);
        v.m_ucMemberShipGrade = Codec<std::remove_cvref_t<decltype(v.m_ucMemberShipGrade)>>::get(r);
        return v;
    }
};

struct KEGS_CHANGE_GUILD_MEMBER_GRADE_REQ {
    std::int64_t m_iUnitUID{};  // UidType
    std::uint8_t m_ucMemberShipGrade{};  // u_char

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_ucMemberShipGrade)>>::put(w, m_ucMemberShipGrade);
    }

    static KEGS_CHANGE_GUILD_MEMBER_GRADE_REQ read_from(Reader& r) {
        KEGS_CHANGE_GUILD_MEMBER_GRADE_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_ucMemberShipGrade = Codec<std::remove_cvref_t<decltype(v.m_ucMemberShipGrade)>>::get(r);
        return v;
    }
};

struct KEGS_CHANGE_GUILD_MEMBER_MESSAGE_ACK {
    std::int32_t m_iOK{};  // int
    bool         m_bDelete{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_bDelete)>>::put(w, m_bDelete);
    }

    static KEGS_CHANGE_GUILD_MEMBER_MESSAGE_ACK read_from(Reader& r) {
        KEGS_CHANGE_GUILD_MEMBER_MESSAGE_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_bDelete = Codec<std::remove_cvref_t<decltype(v.m_bDelete)>>::get(r);
        return v;
    }
};

struct KEGS_CHANGE_GUILD_MEMBER_MESSAGE_REQ {
    std::int64_t   m_iUnitUID{};  // UidType
    std::u16string m_wstrMessage{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrMessage)>>::put(w, m_wstrMessage);
    }

    static KEGS_CHANGE_GUILD_MEMBER_MESSAGE_REQ read_from(Reader& r) {
        KEGS_CHANGE_GUILD_MEMBER_MESSAGE_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_wstrMessage = Codec<std::remove_cvref_t<decltype(v.m_wstrMessage)>>::get(r);
        return v;
    }
};

struct KEGS_CHANGE_GUILD_MESSAGE_REQ {
    std::u16string m_wstrMessage{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrMessage)>>::put(w, m_wstrMessage);
    }

    static KEGS_CHANGE_GUILD_MESSAGE_REQ read_from(Reader& r) {
        KEGS_CHANGE_GUILD_MESSAGE_REQ v;
        v.m_wstrMessage = Codec<std::remove_cvref_t<decltype(v.m_wstrMessage)>>::get(r);
        return v;
    }
};

struct KEGS_CHANGE_GUILD_NAME_CHECK_REQ {
    std::u16string m_wstrGuildName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrGuildName)>>::put(w, m_wstrGuildName);
    }

    static KEGS_CHANGE_GUILD_NAME_CHECK_REQ read_from(Reader& r) {
        KEGS_CHANGE_GUILD_NAME_CHECK_REQ v;
        v.m_wstrGuildName = Codec<std::remove_cvref_t<decltype(v.m_wstrGuildName)>>::get(r);
        return v;
    }
};

struct KEGS_CHANGE_GUILD_NAME_REQ {
    std::int32_t  m_iOK{};  // int
    std::uint32_t m_ulOrderNo{};  // unsigned long
    std::uint32_t m_ulProductNo{};  // unsigned long
    std::int32_t  m_iQuantity{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_ulOrderNo)>>::put(w, m_ulOrderNo);
        Codec<std::remove_cvref_t<decltype(m_ulProductNo)>>::put(w, m_ulProductNo);
        Codec<std::remove_cvref_t<decltype(m_iQuantity)>>::put(w, m_iQuantity);
    }

    static KEGS_CHANGE_GUILD_NAME_REQ read_from(Reader& r) {
        KEGS_CHANGE_GUILD_NAME_REQ v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_ulOrderNo = Codec<std::remove_cvref_t<decltype(v.m_ulOrderNo)>>::get(r);
        v.m_ulProductNo = Codec<std::remove_cvref_t<decltype(v.m_ulProductNo)>>::get(r);
        v.m_iQuantity = Codec<std::remove_cvref_t<decltype(v.m_iQuantity)>>::get(r);
        return v;
    }
};

struct KEGS_CHANGE_INVENTORY_SLOT_ITEM_REQ {
    char         m_cFromSlotType{};
    std::int32_t m_iFromSlotID{};  // int
    char         m_cToSlotType{};
    std::int32_t m_iToSlotID{};  // int
    std::int64_t m_iShareUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cFromSlotType)>>::put(w, m_cFromSlotType);
        Codec<std::remove_cvref_t<decltype(m_iFromSlotID)>>::put(w, m_iFromSlotID);
        Codec<std::remove_cvref_t<decltype(m_cToSlotType)>>::put(w, m_cToSlotType);
        Codec<std::remove_cvref_t<decltype(m_iToSlotID)>>::put(w, m_iToSlotID);
        Codec<std::remove_cvref_t<decltype(m_iShareUnitUID)>>::put(w, m_iShareUnitUID);
    }

    static KEGS_CHANGE_INVENTORY_SLOT_ITEM_REQ read_from(Reader& r) {
        KEGS_CHANGE_INVENTORY_SLOT_ITEM_REQ v;
        v.m_cFromSlotType = Codec<std::remove_cvref_t<decltype(v.m_cFromSlotType)>>::get(r);
        v.m_iFromSlotID = Codec<std::remove_cvref_t<decltype(v.m_iFromSlotID)>>::get(r);
        v.m_cToSlotType = Codec<std::remove_cvref_t<decltype(v.m_cToSlotType)>>::get(r);
        v.m_iToSlotID = Codec<std::remove_cvref_t<decltype(v.m_iToSlotID)>>::get(r);
        v.m_iShareUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iShareUnitUID)>>::get(r);
        return v;
    }
};

struct KEGS_CHANGE_LOVE_WORD_REQ {
    std::int64_t   m_iItemUID{};  // UidType
    std::u16string m_wstrNewLoveWord{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_wstrNewLoveWord)>>::put(w, m_wstrNewLoveWord);
    }

    static KEGS_CHANGE_LOVE_WORD_REQ read_from(Reader& r) {
        KEGS_CHANGE_LOVE_WORD_REQ v;
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_wstrNewLoveWord = Codec<std::remove_cvref_t<decltype(v.m_wstrNewLoveWord)>>::get(r);
        return v;
    }
};

struct KEGS_CHANGE_MAP_ACK {
    std::int32_t m_iOK{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
    }

    static KEGS_CHANGE_MAP_ACK read_from(Reader& r) {
        KEGS_CHANGE_MAP_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        return v;
    }
};

struct KEGS_CHANGE_MAP_NOT {
    std::int16_t m_WorldID{};  // short

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_WorldID)>>::put(w, m_WorldID);
    }

    static KEGS_CHANGE_MAP_NOT read_from(Reader& r) {
        KEGS_CHANGE_MAP_NOT v;
        v.m_WorldID = Codec<std::remove_cvref_t<decltype(v.m_WorldID)>>::get(r);
        return v;
    }
};

struct KEGS_CHANGE_MAP_REQ {
    std::int16_t m_WorldID{};  // short

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_WorldID)>>::put(w, m_WorldID);
    }

    static KEGS_CHANGE_MAP_REQ read_from(Reader& r) {
        KEGS_CHANGE_MAP_REQ v;
        v.m_WorldID = Codec<std::remove_cvref_t<decltype(v.m_WorldID)>>::get(r);
        return v;
    }
};

struct KEGS_CHANGE_MOTION_ACK {
    std::int32_t m_iOK{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
    }

    static KEGS_CHANGE_MOTION_ACK read_from(Reader& r) {
        KEGS_CHANGE_MOTION_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        return v;
    }
};

struct KEGS_CHANGE_MOTION_NOT {
    std::int64_t m_UnitUID{};  // UidType
    char         m_cMotionID{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_UnitUID)>>::put(w, m_UnitUID);
        Codec<std::remove_cvref_t<decltype(m_cMotionID)>>::put(w, m_cMotionID);
    }

    static KEGS_CHANGE_MOTION_NOT read_from(Reader& r) {
        KEGS_CHANGE_MOTION_NOT v;
        v.m_UnitUID = Codec<std::remove_cvref_t<decltype(v.m_UnitUID)>>::get(r);
        v.m_cMotionID = Codec<std::remove_cvref_t<decltype(v.m_cMotionID)>>::get(r);
        return v;
    }
};

struct KEGS_CHANGE_MOTION_REQ {
    char m_cMotionID{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cMotionID)>>::put(w, m_cMotionID);
    }

    static KEGS_CHANGE_MOTION_REQ read_from(Reader& r) {
        KEGS_CHANGE_MOTION_REQ v;
        v.m_cMotionID = Codec<std::remove_cvref_t<decltype(v.m_cMotionID)>>::get(r);
        return v;
    }
};

struct KEGS_CHANGE_MY_UNIT_INFO_NOT {
    char         m_cUnitClass{};
    std::int32_t m_iNewDefaultSkill_1{};  // int
    std::int32_t m_iNewDefaultSkill_2{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cUnitClass)>>::put(w, m_cUnitClass);
        Codec<std::remove_cvref_t<decltype(m_iNewDefaultSkill_1)>>::put(w, m_iNewDefaultSkill_1);
        Codec<std::remove_cvref_t<decltype(m_iNewDefaultSkill_2)>>::put(w, m_iNewDefaultSkill_2);
    }

    static KEGS_CHANGE_MY_UNIT_INFO_NOT read_from(Reader& r) {
        KEGS_CHANGE_MY_UNIT_INFO_NOT v;
        v.m_cUnitClass = Codec<std::remove_cvref_t<decltype(v.m_cUnitClass)>>::get(r);
        v.m_iNewDefaultSkill_1 = Codec<std::remove_cvref_t<decltype(v.m_iNewDefaultSkill_1)>>::get(r);
        v.m_iNewDefaultSkill_2 = Codec<std::remove_cvref_t<decltype(v.m_iNewDefaultSkill_2)>>::get(r);
        return v;
    }
};

struct KEGS_CHANGE_NICK_NAME_ACK {
    std::int32_t   m_iOK{};  // int
    std::int64_t   m_iUnitUID{};  // UidType
    std::u16string m_wstrNickName{};  // std::wstring
    bool           m_bCheckOnly{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_bCheckOnly)>>::put(w, m_bCheckOnly);
    }

    static KEGS_CHANGE_NICK_NAME_ACK read_from(Reader& r) {
        KEGS_CHANGE_NICK_NAME_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_bCheckOnly = Codec<std::remove_cvref_t<decltype(v.m_bCheckOnly)>>::get(r);
        return v;
    }
};

struct KEGS_CHANGE_NICK_NAME_REQ {
    std::int64_t   m_iUnitUID{};  // UidType
    std::u16string m_wstrNickName{};  // std::wstring
    bool           m_bCheckOnly{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_bCheckOnly)>>::put(w, m_bCheckOnly);
    }

    static KEGS_CHANGE_NICK_NAME_REQ read_from(Reader& r) {
        KEGS_CHANGE_NICK_NAME_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_bCheckOnly = Codec<std::remove_cvref_t<decltype(v.m_bCheckOnly)>>::get(r);
        return v;
    }
};

struct KEGS_CHANGE_OPTION_PLAY_GUIDE_REQ {
    bool m_bOn{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bOn)>>::put(w, m_bOn);
    }

    static KEGS_CHANGE_OPTION_PLAY_GUIDE_REQ read_from(Reader& r) {
        KEGS_CHANGE_OPTION_PLAY_GUIDE_REQ v;
        v.m_bOn = Codec<std::remove_cvref_t<decltype(v.m_bOn)>>::get(r);
        return v;
    }
};

struct KEGS_CHANGE_PARTY_FEVER_STATE_NOT {
    bool         m_bIsStart{};
    std::int32_t m_iPartyFever{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bIsStart)>>::put(w, m_bIsStart);
        Codec<std::remove_cvref_t<decltype(m_iPartyFever)>>::put(w, m_iPartyFever);
    }

    static KEGS_CHANGE_PARTY_FEVER_STATE_NOT read_from(Reader& r) {
        KEGS_CHANGE_PARTY_FEVER_STATE_NOT v;
        v.m_bIsStart = Codec<std::remove_cvref_t<decltype(v.m_bIsStart)>>::get(r);
        v.m_iPartyFever = Codec<std::remove_cvref_t<decltype(v.m_iPartyFever)>>::get(r);
        return v;
    }
};

struct KEGS_CHANGE_PET_ID_NOT {
    std::int32_t m_iOK{};  // int
    std::int64_t m_iPetUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iPetUID)>>::put(w, m_iPetUID);
    }

    static KEGS_CHANGE_PET_ID_NOT read_from(Reader& r) {
        KEGS_CHANGE_PET_ID_NOT v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iPetUID = Codec<std::remove_cvref_t<decltype(v.m_iPetUID)>>::get(r);
        return v;
    }
};

struct KEGS_CHANGE_PET_NAME_REQ {
    std::int64_t   m_iItemUID{};  // UidType
    std::u16string m_wstrPetName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_wstrPetName)>>::put(w, m_wstrPetName);
    }

    static KEGS_CHANGE_PET_NAME_REQ read_from(Reader& r) {
        KEGS_CHANGE_PET_NAME_REQ v;
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_wstrPetName = Codec<std::remove_cvref_t<decltype(v.m_wstrPetName)>>::get(r);
        return v;
    }
};

struct KEGS_CHANGE_PITIN_NOT {
    std::int64_t m_UnitUID{};  // UidType
    bool         m_bPitIn{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_UnitUID)>>::put(w, m_UnitUID);
        Codec<std::remove_cvref_t<decltype(m_bPitIn)>>::put(w, m_bPitIn);
    }

    static KEGS_CHANGE_PITIN_NOT read_from(Reader& r) {
        KEGS_CHANGE_PITIN_NOT v;
        v.m_UnitUID = Codec<std::remove_cvref_t<decltype(v.m_UnitUID)>>::get(r);
        v.m_bPitIn = Codec<std::remove_cvref_t<decltype(v.m_bPitIn)>>::get(r);
        return v;
    }
};

struct KEGS_CHANGE_PITIN_REQ {
    bool m_bPitIn{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bPitIn)>>::put(w, m_bPitIn);
    }

    static KEGS_CHANGE_PITIN_REQ read_from(Reader& r) {
        KEGS_CHANGE_PITIN_REQ v;
        v.m_bPitIn = Codec<std::remove_cvref_t<decltype(v.m_bPitIn)>>::get(r);
        return v;
    }
};

struct KEGS_CHANGE_PLAY_TIME_LIMIT_NOT {
    float m_fPlayTime{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_fPlayTime)>>::put(w, m_fPlayTime);
    }

    static KEGS_CHANGE_PLAY_TIME_LIMIT_NOT read_from(Reader& r) {
        KEGS_CHANGE_PLAY_TIME_LIMIT_NOT v;
        v.m_fPlayTime = Codec<std::remove_cvref_t<decltype(v.m_fPlayTime)>>::get(r);
        return v;
    }
};

