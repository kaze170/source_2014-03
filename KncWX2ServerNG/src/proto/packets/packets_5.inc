// ---------------------------------------------------------------------------
// packets_5.inc - GENERATED FILE, DO NOT EDIT BY HAND.
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

struct KE_LOCAL_LOG_TITLE_NOT {
    std::int32_t   m_iTitleID{};  // int
    std::u16string m_wstrCharName{};  // std::wstring
    std::uint8_t   m_ucLevel{};  // u_char

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iTitleID)>>::put(w, m_iTitleID);
        Codec<std::remove_cvref_t<decltype(m_wstrCharName)>>::put(w, m_wstrCharName);
        Codec<std::remove_cvref_t<decltype(m_ucLevel)>>::put(w, m_ucLevel);
    }

    static KE_LOCAL_LOG_TITLE_NOT read_from(Reader& r) {
        KE_LOCAL_LOG_TITLE_NOT v;
        v.m_iTitleID = Codec<std::remove_cvref_t<decltype(v.m_iTitleID)>>::get(r);
        v.m_wstrCharName = Codec<std::remove_cvref_t<decltype(v.m_wstrCharName)>>::get(r);
        v.m_ucLevel = Codec<std::remove_cvref_t<decltype(v.m_ucLevel)>>::get(r);
        return v;
    }
};

struct KE_TOOL_CHECK_LOGIN_ACK {
    std::int32_t m_iAuthLevel{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iAuthLevel)>>::put(w, m_iAuthLevel);
    }

    static KE_TOOL_CHECK_LOGIN_ACK read_from(Reader& r) {
        KE_TOOL_CHECK_LOGIN_ACK v;
        v.m_iAuthLevel = Codec<std::remove_cvref_t<decltype(v.m_iAuthLevel)>>::get(r);
        return v;
    }
};

struct KE_TOOL_CHECK_LOGIN_REQ {
    std::u16string m_wstrID{};  // std::wstring
    std::u16string m_wstrPW{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrID)>>::put(w, m_wstrID);
        Codec<std::remove_cvref_t<decltype(m_wstrPW)>>::put(w, m_wstrPW);
    }

    static KE_TOOL_CHECK_LOGIN_REQ read_from(Reader& r) {
        KE_TOOL_CHECK_LOGIN_REQ v;
        v.m_wstrID = Codec<std::remove_cvref_t<decltype(v.m_wstrID)>>::get(r);
        v.m_wstrPW = Codec<std::remove_cvref_t<decltype(v.m_wstrPW)>>::get(r);
        return v;
    }
};

struct KE_TOOL_GET_CCU_INFO_ACK {
    std::int32_t          m_iOK{};  // int
    char                  m_cCCUType{};
    std::int64_t          m_iServerUID{};  // UidType
    std::u16string        m_wstrDate{};  // std::wstring
    std::vector<KCCUInfo> m_vecCCUInfo{};  // std::vector< KCCUInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_cCCUType)>>::put(w, m_cCCUType);
        Codec<std::remove_cvref_t<decltype(m_iServerUID)>>::put(w, m_iServerUID);
        Codec<std::remove_cvref_t<decltype(m_wstrDate)>>::put(w, m_wstrDate);
        Codec<std::remove_cvref_t<decltype(m_vecCCUInfo)>>::put(w, m_vecCCUInfo);
    }

    static KE_TOOL_GET_CCU_INFO_ACK read_from(Reader& r) {
        KE_TOOL_GET_CCU_INFO_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_cCCUType = Codec<std::remove_cvref_t<decltype(v.m_cCCUType)>>::get(r);
        v.m_iServerUID = Codec<std::remove_cvref_t<decltype(v.m_iServerUID)>>::get(r);
        v.m_wstrDate = Codec<std::remove_cvref_t<decltype(v.m_wstrDate)>>::get(r);
        v.m_vecCCUInfo = Codec<std::remove_cvref_t<decltype(v.m_vecCCUInfo)>>::get(r);
        return v;
    }
};

struct KE_TOOL_GET_CCU_INFO_REQ {
    char           m_cCCUType{};
    std::int64_t   m_iServerUID{};  // UidType
    std::u16string m_wstrDate{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cCCUType)>>::put(w, m_cCCUType);
        Codec<std::remove_cvref_t<decltype(m_iServerUID)>>::put(w, m_iServerUID);
        Codec<std::remove_cvref_t<decltype(m_wstrDate)>>::put(w, m_wstrDate);
    }

    static KE_TOOL_GET_CCU_INFO_REQ read_from(Reader& r) {
        KE_TOOL_GET_CCU_INFO_REQ v;
        v.m_cCCUType = Codec<std::remove_cvref_t<decltype(v.m_cCCUType)>>::get(r);
        v.m_iServerUID = Codec<std::remove_cvref_t<decltype(v.m_iServerUID)>>::get(r);
        v.m_wstrDate = Codec<std::remove_cvref_t<decltype(v.m_wstrDate)>>::get(r);
        return v;
    }
};

struct KErrorMsgInfo {
    std::int32_t   m_iErrorMsgID{};  // int
    std::u16string m_wstrErrorMsg{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iErrorMsgID)>>::put(w, m_iErrorMsgID);
        Codec<std::remove_cvref_t<decltype(m_wstrErrorMsg)>>::put(w, m_wstrErrorMsg);
    }

    static KErrorMsgInfo read_from(Reader& r) {
        KErrorMsgInfo v;
        v.m_iErrorMsgID = Codec<std::remove_cvref_t<decltype(v.m_iErrorMsgID)>>::get(r);
        v.m_wstrErrorMsg = Codec<std::remove_cvref_t<decltype(v.m_wstrErrorMsg)>>::get(r);
        return v;
    }
};

struct KEGS_CLIENT_QUIT_REQ {
    std::int32_t               m_iReason{};  // int
    std::vector<KErrorMsgInfo> m_ErrorMsgInfoList{};  // std::vector< KErrorMsgInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iReason)>>::put(w, m_iReason);
        Codec<std::remove_cvref_t<decltype(m_ErrorMsgInfoList)>>::put(w, m_ErrorMsgInfoList);
    }

    static KEGS_CLIENT_QUIT_REQ read_from(Reader& r) {
        KEGS_CLIENT_QUIT_REQ v;
        v.m_iReason = Codec<std::remove_cvref_t<decltype(v.m_iReason)>>::get(r);
        v.m_ErrorMsgInfoList = Codec<std::remove_cvref_t<decltype(v.m_ErrorMsgInfoList)>>::get(r);
        return v;
    }
};

struct KEventInfo {
    std::int32_t   m_iEventUID{};  // int
    std::int32_t   m_iEventType{};  // int
    std::u16string m_wstrEventName{};  // std::wstring
    std::u16string m_wstrBeginDate{};  // std::wstring
    std::u16string m_wstrEndDate{};  // std::wstring
    std::int32_t   m_iScriptID{};  // int
    bool           m_bEnable{};
    bool           m_bOn{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iEventUID)>>::put(w, m_iEventUID);
        Codec<std::remove_cvref_t<decltype(m_iEventType)>>::put(w, m_iEventType);
        Codec<std::remove_cvref_t<decltype(m_wstrEventName)>>::put(w, m_wstrEventName);
        Codec<std::remove_cvref_t<decltype(m_wstrBeginDate)>>::put(w, m_wstrBeginDate);
        Codec<std::remove_cvref_t<decltype(m_wstrEndDate)>>::put(w, m_wstrEndDate);
        Codec<std::remove_cvref_t<decltype(m_iScriptID)>>::put(w, m_iScriptID);
        Codec<std::remove_cvref_t<decltype(m_bEnable)>>::put(w, m_bEnable);
        Codec<std::remove_cvref_t<decltype(m_bOn)>>::put(w, m_bOn);
    }

    static KEventInfo read_from(Reader& r) {
        KEventInfo v;
        v.m_iEventUID = Codec<std::remove_cvref_t<decltype(v.m_iEventUID)>>::get(r);
        v.m_iEventType = Codec<std::remove_cvref_t<decltype(v.m_iEventType)>>::get(r);
        v.m_wstrEventName = Codec<std::remove_cvref_t<decltype(v.m_wstrEventName)>>::get(r);
        v.m_wstrBeginDate = Codec<std::remove_cvref_t<decltype(v.m_wstrBeginDate)>>::get(r);
        v.m_wstrEndDate = Codec<std::remove_cvref_t<decltype(v.m_wstrEndDate)>>::get(r);
        v.m_iScriptID = Codec<std::remove_cvref_t<decltype(v.m_iScriptID)>>::get(r);
        v.m_bEnable = Codec<std::remove_cvref_t<decltype(v.m_bEnable)>>::get(r);
        v.m_bOn = Codec<std::remove_cvref_t<decltype(v.m_bOn)>>::get(r);
        return v;
    }
};

struct KDBE_EVENT_UPDATE_ACK {
    std::vector<KEventInfo> m_vecEventList{};  // std::vector< KEventInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecEventList)>>::put(w, m_vecEventList);
    }

    static KDBE_EVENT_UPDATE_ACK read_from(Reader& r) {
        KDBE_EVENT_UPDATE_ACK v;
        v.m_vecEventList = Codec<std::remove_cvref_t<decltype(v.m_vecEventList)>>::get(r);
        return v;
    }
};

struct KEventQuestInfo {
    bool         m_bInit{};
    std::int32_t m_iEventUID{};  // int
    std::int32_t m_iQuestID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bInit)>>::put(w, m_bInit);
        Codec<std::remove_cvref_t<decltype(m_iEventUID)>>::put(w, m_iEventUID);
        Codec<std::remove_cvref_t<decltype(m_iQuestID)>>::put(w, m_iQuestID);
    }

    static KEventQuestInfo read_from(Reader& r) {
        KEventQuestInfo v;
        v.m_bInit = Codec<std::remove_cvref_t<decltype(v.m_bInit)>>::get(r);
        v.m_iEventUID = Codec<std::remove_cvref_t<decltype(v.m_iEventUID)>>::get(r);
        v.m_iQuestID = Codec<std::remove_cvref_t<decltype(v.m_iQuestID)>>::get(r);
        return v;
    }
};

struct KExchangeLimitInfo {
    std::int32_t           m_iLimitUID{};  // int
    std::int64_t           m_tBeginDate{};  // __time64_t
    std::int64_t           m_tEndDate{};  // __time64_t
    std::set<std::int32_t> m_setDestItemID{};  // std::set<int>
    std::int32_t           m_iLimitQuantity{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iLimitUID)>>::put(w, m_iLimitUID);
        Codec<std::remove_cvref_t<decltype(m_tBeginDate)>>::put(w, m_tBeginDate);
        Codec<std::remove_cvref_t<decltype(m_tEndDate)>>::put(w, m_tEndDate);
        Codec<std::remove_cvref_t<decltype(m_setDestItemID)>>::put(w, m_setDestItemID);
        Codec<std::remove_cvref_t<decltype(m_iLimitQuantity)>>::put(w, m_iLimitQuantity);
    }

    static KExchangeLimitInfo read_from(Reader& r) {
        KExchangeLimitInfo v;
        v.m_iLimitUID = Codec<std::remove_cvref_t<decltype(v.m_iLimitUID)>>::get(r);
        v.m_tBeginDate = Codec<std::remove_cvref_t<decltype(v.m_tBeginDate)>>::get(r);
        v.m_tEndDate = Codec<std::remove_cvref_t<decltype(v.m_tEndDate)>>::get(r);
        v.m_setDestItemID = Codec<std::remove_cvref_t<decltype(v.m_setDestItemID)>>::get(r);
        v.m_iLimitQuantity = Codec<std::remove_cvref_t<decltype(v.m_iLimitQuantity)>>::get(r);
        return v;
    }
};

struct KDBE_EXCHANGE_LIMIT_INFO_ACK {
    std::list<KExchangeLimitInfo> m_listExchangeLimitInfo{};  // std::list< KExchangeLimitInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_listExchangeLimitInfo)>>::put(w, m_listExchangeLimitInfo);
    }

    static KDBE_EXCHANGE_LIMIT_INFO_ACK read_from(Reader& r) {
        KDBE_EXCHANGE_LIMIT_INFO_ACK v;
        v.m_listExchangeLimitInfo = Codec<std::remove_cvref_t<decltype(v.m_listExchangeLimitInfo)>>::get(r);
        return v;
    }
};

struct KFieldBossGenKillStat {
    std::int32_t m_nFieldID{};  // int
    std::int32_t m_nNumUser{};  // int
    std::int32_t m_nSpawnID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_nFieldID)>>::put(w, m_nFieldID);
        Codec<std::remove_cvref_t<decltype(m_nNumUser)>>::put(w, m_nNumUser);
        Codec<std::remove_cvref_t<decltype(m_nSpawnID)>>::put(w, m_nSpawnID);
    }

    static KFieldBossGenKillStat read_from(Reader& r) {
        KFieldBossGenKillStat v;
        v.m_nFieldID = Codec<std::remove_cvref_t<decltype(v.m_nFieldID)>>::get(r);
        v.m_nNumUser = Codec<std::remove_cvref_t<decltype(v.m_nNumUser)>>::get(r);
        v.m_nSpawnID = Codec<std::remove_cvref_t<decltype(v.m_nSpawnID)>>::get(r);
        return v;
    }
};

struct KFieldCoordinates {
    std::int16_t m_PosX{};  // short
    std::int16_t m_PosY{};  // short
    std::int16_t m_PosZ{};  // short

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_PosX)>>::put(w, m_PosX);
        Codec<std::remove_cvref_t<decltype(m_PosY)>>::put(w, m_PosY);
        Codec<std::remove_cvref_t<decltype(m_PosZ)>>::put(w, m_PosZ);
    }

    static KFieldCoordinates read_from(Reader& r) {
        KFieldCoordinates v;
        v.m_PosX = Codec<std::remove_cvref_t<decltype(v.m_PosX)>>::get(r);
        v.m_PosY = Codec<std::remove_cvref_t<decltype(v.m_PosY)>>::get(r);
        v.m_PosZ = Codec<std::remove_cvref_t<decltype(v.m_PosZ)>>::get(r);
        return v;
    }
};

struct KFieldFrameInfo {
    std::uint8_t m_StateID{};  // u_char
    std::uint8_t m_FrameCount{};  // u_char

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_StateID)>>::put(w, m_StateID);
        Codec<std::remove_cvref_t<decltype(m_FrameCount)>>::put(w, m_FrameCount);
    }

    static KFieldFrameInfo read_from(Reader& r) {
        KFieldFrameInfo v;
        v.m_StateID = Codec<std::remove_cvref_t<decltype(v.m_StateID)>>::get(r);
        v.m_FrameCount = Codec<std::remove_cvref_t<decltype(v.m_FrameCount)>>::get(r);
        return v;
    }
};

struct KFieldOption {
    std::int16_t m_sLimitOption{};  // short

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_sLimitOption)>>::put(w, m_sLimitOption);
    }

    static KFieldOption read_from(Reader& r) {
        KFieldOption v;
        v.m_sLimitOption = Codec<std::remove_cvref_t<decltype(v.m_sLimitOption)>>::get(r);
        return v;
    }
};

struct KEGS_OPTION_UPDATE_REQ {
    KFieldOption m_kFieldOption{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kFieldOption)>>::put(w, m_kFieldOption);
    }

    static KEGS_OPTION_UPDATE_REQ read_from(Reader& r) {
        KEGS_OPTION_UPDATE_REQ v;
        v.m_kFieldOption = Codec<std::remove_cvref_t<decltype(v.m_kFieldOption)>>::get(r);
        return v;
    }
};

struct KERM_OPTION_UPDATE_NOT {
    KFieldOption m_kFieldOption{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kFieldOption)>>::put(w, m_kFieldOption);
    }

    static KERM_OPTION_UPDATE_NOT read_from(Reader& r) {
        KERM_OPTION_UPDATE_NOT v;
        v.m_kFieldOption = Codec<std::remove_cvref_t<decltype(v.m_kFieldOption)>>::get(r);
        return v;
    }
};

struct KFieldPetInfo {
    std::int64_t   m_iPetUID{};  // UidType
    std::int32_t   m_iPetID{};  // int
    std::u16string m_wstrPetName{};  // std::wstring
    char           m_cEvolutionStep{};
    std::int32_t   m_iIntimacy{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iPetUID)>>::put(w, m_iPetUID);
        Codec<std::remove_cvref_t<decltype(m_iPetID)>>::put(w, m_iPetID);
        Codec<std::remove_cvref_t<decltype(m_wstrPetName)>>::put(w, m_wstrPetName);
        Codec<std::remove_cvref_t<decltype(m_cEvolutionStep)>>::put(w, m_cEvolutionStep);
        Codec<std::remove_cvref_t<decltype(m_iIntimacy)>>::put(w, m_iIntimacy);
    }

    static KFieldPetInfo read_from(Reader& r) {
        KFieldPetInfo v;
        v.m_iPetUID = Codec<std::remove_cvref_t<decltype(v.m_iPetUID)>>::get(r);
        v.m_iPetID = Codec<std::remove_cvref_t<decltype(v.m_iPetID)>>::get(r);
        v.m_wstrPetName = Codec<std::remove_cvref_t<decltype(v.m_wstrPetName)>>::get(r);
        v.m_cEvolutionStep = Codec<std::remove_cvref_t<decltype(v.m_cEvolutionStep)>>::get(r);
        v.m_iIntimacy = Codec<std::remove_cvref_t<decltype(v.m_iIntimacy)>>::get(r);
        return v;
    }
};

struct KFriendInfo {
    std::int64_t   m_iUnitUID{};  // UidType
    std::u16string m_wstrNickName{};  // std::wstring
    char           m_cGroupID{};
    char           m_cStatus{};
    char           m_cPosition{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_cGroupID)>>::put(w, m_cGroupID);
        Codec<std::remove_cvref_t<decltype(m_cStatus)>>::put(w, m_cStatus);
        Codec<std::remove_cvref_t<decltype(m_cPosition)>>::put(w, m_cPosition);
    }

    static KFriendInfo read_from(Reader& r) {
        KFriendInfo v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_cGroupID = Codec<std::remove_cvref_t<decltype(v.m_cGroupID)>>::get(r);
        v.m_cStatus = Codec<std::remove_cvref_t<decltype(v.m_cStatus)>>::get(r);
        v.m_cPosition = Codec<std::remove_cvref_t<decltype(v.m_cPosition)>>::get(r);
        return v;
    }
};

struct KFriendMessageInfo {
    std::int64_t   m_iUnitUID{};  // UidType
    std::u16string m_wstrNickName{};  // std::wstring
    char           m_cMessageType{};
    std::u16string m_wstrMessage{};  // std::wstring
    std::u16string m_wstrRegDate{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_cMessageType)>>::put(w, m_cMessageType);
        Codec<std::remove_cvref_t<decltype(m_wstrMessage)>>::put(w, m_wstrMessage);
        Codec<std::remove_cvref_t<decltype(m_wstrRegDate)>>::put(w, m_wstrRegDate);
    }

    static KFriendMessageInfo read_from(Reader& r) {
        KFriendMessageInfo v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_cMessageType = Codec<std::remove_cvref_t<decltype(v.m_cMessageType)>>::get(r);
        v.m_wstrMessage = Codec<std::remove_cvref_t<decltype(v.m_wstrMessage)>>::get(r);
        v.m_wstrRegDate = Codec<std::remove_cvref_t<decltype(v.m_wstrRegDate)>>::get(r);
        return v;
    }
};

struct KGamePlayNetworkInfo {
    std::int64_t m_iOpponentUnitUID{};  // UidType
    std::int32_t m_iHolePunchingTryCount{};  // int
    float        m_fP2PPlayTime{};
    float        m_fRelayPlayTime{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOpponentUnitUID)>>::put(w, m_iOpponentUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iHolePunchingTryCount)>>::put(w, m_iHolePunchingTryCount);
        Codec<std::remove_cvref_t<decltype(m_fP2PPlayTime)>>::put(w, m_fP2PPlayTime);
        Codec<std::remove_cvref_t<decltype(m_fRelayPlayTime)>>::put(w, m_fRelayPlayTime);
    }

    static KGamePlayNetworkInfo read_from(Reader& r) {
        KGamePlayNetworkInfo v;
        v.m_iOpponentUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iOpponentUnitUID)>>::get(r);
        v.m_iHolePunchingTryCount = Codec<std::remove_cvref_t<decltype(v.m_iHolePunchingTryCount)>>::get(r);
        v.m_fP2PPlayTime = Codec<std::remove_cvref_t<decltype(v.m_fP2PPlayTime)>>::get(r);
        v.m_fRelayPlayTime = Codec<std::remove_cvref_t<decltype(v.m_fRelayPlayTime)>>::get(r);
        return v;
    }
};

struct KGamePlayNetworkInfoPackage {
    std::vector<KGamePlayNetworkInfo> m_vecGamePlayNetworkInfo{};  // std::vector< KGamePlayNetworkInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecGamePlayNetworkInfo)>>::put(w, m_vecGamePlayNetworkInfo);
    }

    static KGamePlayNetworkInfoPackage read_from(Reader& r) {
        KGamePlayNetworkInfoPackage v;
        v.m_vecGamePlayNetworkInfo = Codec<std::remove_cvref_t<decltype(v.m_vecGamePlayNetworkInfo)>>::get(r);
        return v;
    }
};

struct KEGS_LEAVE_ROOM_REQ {
    std::int32_t                m_iReason{};  // int
    KDungeonPlayResultInfo      m_kResultInfo{};
    std::int32_t                m_iBattleFieldID{};  // int
    KGamePlayNetworkInfoPackage m_kGamePlayNetworkInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iReason)>>::put(w, m_iReason);
        Codec<std::remove_cvref_t<decltype(m_kResultInfo)>>::put(w, m_kResultInfo);
        Codec<std::remove_cvref_t<decltype(m_iBattleFieldID)>>::put(w, m_iBattleFieldID);
        Codec<std::remove_cvref_t<decltype(m_kGamePlayNetworkInfo)>>::put(w, m_kGamePlayNetworkInfo);
    }

    static KEGS_LEAVE_ROOM_REQ read_from(Reader& r) {
        KEGS_LEAVE_ROOM_REQ v;
        v.m_iReason = Codec<std::remove_cvref_t<decltype(v.m_iReason)>>::get(r);
        v.m_kResultInfo = Codec<std::remove_cvref_t<decltype(v.m_kResultInfo)>>::get(r);
        v.m_iBattleFieldID = Codec<std::remove_cvref_t<decltype(v.m_iBattleFieldID)>>::get(r);
        v.m_kGamePlayNetworkInfo = Codec<std::remove_cvref_t<decltype(v.m_kGamePlayNetworkInfo)>>::get(r);
        return v;
    }
};

struct KEGS_RESULT_SUCCESS_REQ {
    KGamePlayNetworkInfoPackage m_kGamePlayNetworkInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kGamePlayNetworkInfo)>>::put(w, m_kGamePlayNetworkInfo);
    }

    static KEGS_RESULT_SUCCESS_REQ read_from(Reader& r) {
        KEGS_RESULT_SUCCESS_REQ v;
        v.m_kGamePlayNetworkInfo = Codec<std::remove_cvref_t<decltype(v.m_kGamePlayNetworkInfo)>>::get(r);
        return v;
    }
};

struct KERM_LEAVE_ROOM_REQ {
    std::int32_t                m_iReason{};  // int
    std::int32_t                m_iItemID{};  // int
    bool                        m_bSendAck{};
    KBattleFieldJoinInfo        m_kBattleFieldJoinInfo{};
    KDungeonPlayResultInfo      m_kResultInfo{};
    KGamePlayNetworkInfoPackage m_kGamePlayNetworkInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iReason)>>::put(w, m_iReason);
        Codec<std::remove_cvref_t<decltype(m_iItemID)>>::put(w, m_iItemID);
        Codec<std::remove_cvref_t<decltype(m_bSendAck)>>::put(w, m_bSendAck);
        Codec<std::remove_cvref_t<decltype(m_kBattleFieldJoinInfo)>>::put(w, m_kBattleFieldJoinInfo);
        Codec<std::remove_cvref_t<decltype(m_kResultInfo)>>::put(w, m_kResultInfo);
        Codec<std::remove_cvref_t<decltype(m_kGamePlayNetworkInfo)>>::put(w, m_kGamePlayNetworkInfo);
    }

    static KERM_LEAVE_ROOM_REQ read_from(Reader& r) {
        KERM_LEAVE_ROOM_REQ v;
        v.m_iReason = Codec<std::remove_cvref_t<decltype(v.m_iReason)>>::get(r);
        v.m_iItemID = Codec<std::remove_cvref_t<decltype(v.m_iItemID)>>::get(r);
        v.m_bSendAck = Codec<std::remove_cvref_t<decltype(v.m_bSendAck)>>::get(r);
        v.m_kBattleFieldJoinInfo = Codec<std::remove_cvref_t<decltype(v.m_kBattleFieldJoinInfo)>>::get(r);
        v.m_kResultInfo = Codec<std::remove_cvref_t<decltype(v.m_kResultInfo)>>::get(r);
        v.m_kGamePlayNetworkInfo = Codec<std::remove_cvref_t<decltype(v.m_kGamePlayNetworkInfo)>>::get(r);
        return v;
    }
};

struct KGamePlayStatus {
    std::int32_t                         m_iMaxHP{};  // int
    std::int32_t                         m_iCurHP{};  // int
    std::int32_t                         m_iMaxMP{};  // int
    std::int32_t                         m_iCurMP{};  // int
    std::int32_t                         m_iCurHyperGage{};  // int
    char                                 m_cCurHyperCount{};
    char                                 m_cCharAbilType{};
    std::int32_t                         m_iCharAbilCount{};  // int
    std::map<std::int32_t, std::int32_t> m_mapSkillCoolTime{};  // std::map< int, int >
    std::map<std::int32_t, std::int32_t> m_mapQuickSlotCoolTime{};  // std::map< int, int >
    std::set<std::int32_t>               m_mapPetMP{};  // std::set< int >
    std::map<std::int32_t, std::int32_t> m_mapRidingPetCoolTime{};  // std::map< int, int >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iMaxHP)>>::put(w, m_iMaxHP);
        Codec<std::remove_cvref_t<decltype(m_iCurHP)>>::put(w, m_iCurHP);
        Codec<std::remove_cvref_t<decltype(m_iMaxMP)>>::put(w, m_iMaxMP);
        Codec<std::remove_cvref_t<decltype(m_iCurMP)>>::put(w, m_iCurMP);
        Codec<std::remove_cvref_t<decltype(m_iCurHyperGage)>>::put(w, m_iCurHyperGage);
        Codec<std::remove_cvref_t<decltype(m_cCurHyperCount)>>::put(w, m_cCurHyperCount);
        Codec<std::remove_cvref_t<decltype(m_cCharAbilType)>>::put(w, m_cCharAbilType);
        Codec<std::remove_cvref_t<decltype(m_iCharAbilCount)>>::put(w, m_iCharAbilCount);
        Codec<std::remove_cvref_t<decltype(m_mapSkillCoolTime)>>::put(w, m_mapSkillCoolTime);
        Codec<std::remove_cvref_t<decltype(m_mapQuickSlotCoolTime)>>::put(w, m_mapQuickSlotCoolTime);
        Codec<std::remove_cvref_t<decltype(m_mapPetMP)>>::put(w, m_mapPetMP);
        Codec<std::remove_cvref_t<decltype(m_mapRidingPetCoolTime)>>::put(w, m_mapRidingPetCoolTime);
    }

    static KGamePlayStatus read_from(Reader& r) {
        KGamePlayStatus v;
        v.m_iMaxHP = Codec<std::remove_cvref_t<decltype(v.m_iMaxHP)>>::get(r);
        v.m_iCurHP = Codec<std::remove_cvref_t<decltype(v.m_iCurHP)>>::get(r);
        v.m_iMaxMP = Codec<std::remove_cvref_t<decltype(v.m_iMaxMP)>>::get(r);
        v.m_iCurMP = Codec<std::remove_cvref_t<decltype(v.m_iCurMP)>>::get(r);
        v.m_iCurHyperGage = Codec<std::remove_cvref_t<decltype(v.m_iCurHyperGage)>>::get(r);
        v.m_cCurHyperCount = Codec<std::remove_cvref_t<decltype(v.m_cCurHyperCount)>>::get(r);
        v.m_cCharAbilType = Codec<std::remove_cvref_t<decltype(v.m_cCharAbilType)>>::get(r);
        v.m_iCharAbilCount = Codec<std::remove_cvref_t<decltype(v.m_iCharAbilCount)>>::get(r);
        v.m_mapSkillCoolTime = Codec<std::remove_cvref_t<decltype(v.m_mapSkillCoolTime)>>::get(r);
        v.m_mapQuickSlotCoolTime = Codec<std::remove_cvref_t<decltype(v.m_mapQuickSlotCoolTime)>>::get(r);
        v.m_mapPetMP = Codec<std::remove_cvref_t<decltype(v.m_mapPetMP)>>::get(r);
        v.m_mapRidingPetCoolTime = Codec<std::remove_cvref_t<decltype(v.m_mapRidingPetCoolTime)>>::get(r);
        return v;
    }
};

struct KEGS_UPDATE_PLAY_STATUS_NOT {
    KGamePlayStatus m_kGamePlayStatus{};
    std::int32_t    m_iFrame{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kGamePlayStatus)>>::put(w, m_kGamePlayStatus);
        Codec<std::remove_cvref_t<decltype(m_iFrame)>>::put(w, m_iFrame);
    }

    static KEGS_UPDATE_PLAY_STATUS_NOT read_from(Reader& r) {
        KEGS_UPDATE_PLAY_STATUS_NOT v;
        v.m_kGamePlayStatus = Codec<std::remove_cvref_t<decltype(v.m_kGamePlayStatus)>>::get(r);
        v.m_iFrame = Codec<std::remove_cvref_t<decltype(v.m_iFrame)>>::get(r);
        return v;
    }
};

struct KGamePlayStatusContainer {
    std::vector<KGamePlayStatus> m_vecGamePlayStatus{};  // std::vector< KGamePlayStatus >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecGamePlayStatus)>>::put(w, m_vecGamePlayStatus);
    }

    static KGamePlayStatusContainer read_from(Reader& r) {
        KGamePlayStatusContainer v;
        v.m_vecGamePlayStatus = Codec<std::remove_cvref_t<decltype(v.m_vecGamePlayStatus)>>::get(r);
        return v;
    }
};

struct KGashResponseData {
    std::string m_strResultCode{};
    std::string m_strOutString{};
    std::string m_strAccountID{};
    std::string m_strPacketID{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_strResultCode)>>::put(w, m_strResultCode);
        Codec<std::remove_cvref_t<decltype(m_strOutString)>>::put(w, m_strOutString);
        Codec<std::remove_cvref_t<decltype(m_strAccountID)>>::put(w, m_strAccountID);
        Codec<std::remove_cvref_t<decltype(m_strPacketID)>>::put(w, m_strPacketID);
    }

    static KGashResponseData read_from(Reader& r) {
        KGashResponseData v;
        v.m_strResultCode = Codec<std::remove_cvref_t<decltype(v.m_strResultCode)>>::get(r);
        v.m_strOutString = Codec<std::remove_cvref_t<decltype(v.m_strOutString)>>::get(r);
        v.m_strAccountID = Codec<std::remove_cvref_t<decltype(v.m_strAccountID)>>::get(r);
        v.m_strPacketID = Codec<std::remove_cvref_t<decltype(v.m_strPacketID)>>::get(r);
        return v;
    }
};

struct KGetSkillInfo {
    std::int32_t m_iSkillID{};  // int
    std::int32_t m_iBeforeSkillLevel{};  // int
    std::int32_t m_iSkillLevel{};  // int
    std::int32_t m_iSpendSkillCSPoint{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iSkillID)>>::put(w, m_iSkillID);
        Codec<std::remove_cvref_t<decltype(m_iBeforeSkillLevel)>>::put(w, m_iBeforeSkillLevel);
        Codec<std::remove_cvref_t<decltype(m_iSkillLevel)>>::put(w, m_iSkillLevel);
        Codec<std::remove_cvref_t<decltype(m_iSpendSkillCSPoint)>>::put(w, m_iSpendSkillCSPoint);
    }

    static KGetSkillInfo read_from(Reader& r) {
        KGetSkillInfo v;
        v.m_iSkillID = Codec<std::remove_cvref_t<decltype(v.m_iSkillID)>>::get(r);
        v.m_iBeforeSkillLevel = Codec<std::remove_cvref_t<decltype(v.m_iBeforeSkillLevel)>>::get(r);
        v.m_iSkillLevel = Codec<std::remove_cvref_t<decltype(v.m_iSkillLevel)>>::get(r);
        v.m_iSpendSkillCSPoint = Codec<std::remove_cvref_t<decltype(v.m_iSpendSkillCSPoint)>>::get(r);
        return v;
    }
};

struct KDBE_INSERT_SKILL_ACK {
    std::int32_t                          m_iOK{};  // int
    std::int32_t                          m_iActiveSkillPageNumber{};  // int
    std::map<std::int32_t, KGetSkillInfo> m_mapSkillList{};  // std::map< int, KGetSkillInfo >
    std::int32_t                          m_iCSPoint{};  // int
    std::int32_t                          m_iTotalSpendSkillPoint{};  // int
    std::int32_t                          m_iBeforeCSPoint{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iActiveSkillPageNumber)>>::put(w, m_iActiveSkillPageNumber);
        Codec<std::remove_cvref_t<decltype(m_mapSkillList)>>::put(w, m_mapSkillList);
        Codec<std::remove_cvref_t<decltype(m_iCSPoint)>>::put(w, m_iCSPoint);
        Codec<std::remove_cvref_t<decltype(m_iTotalSpendSkillPoint)>>::put(w, m_iTotalSpendSkillPoint);
        Codec<std::remove_cvref_t<decltype(m_iBeforeCSPoint)>>::put(w, m_iBeforeCSPoint);
    }

    static KDBE_INSERT_SKILL_ACK read_from(Reader& r) {
        KDBE_INSERT_SKILL_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iActiveSkillPageNumber = Codec<std::remove_cvref_t<decltype(v.m_iActiveSkillPageNumber)>>::get(r);
        v.m_mapSkillList = Codec<std::remove_cvref_t<decltype(v.m_mapSkillList)>>::get(r);
        v.m_iCSPoint = Codec<std::remove_cvref_t<decltype(v.m_iCSPoint)>>::get(r);
        v.m_iTotalSpendSkillPoint = Codec<std::remove_cvref_t<decltype(v.m_iTotalSpendSkillPoint)>>::get(r);
        v.m_iBeforeCSPoint = Codec<std::remove_cvref_t<decltype(v.m_iBeforeCSPoint)>>::get(r);
        return v;
    }
};

struct KDBE_INSERT_SKILL_REQ {
    std::int64_t                          m_iUnitUID{};  // UidType
    std::int32_t                          m_iActiveSkillPageNumber{};  // int
    std::map<std::int32_t, KGetSkillInfo> m_mapSkillList{};  // std::map< int, KGetSkillInfo >
    std::int32_t                          m_iCSPoint{};  // int
    std::int32_t                          m_iTotalSpendSkillPoint{};  // int
    std::int32_t                          m_iBeforeCSPoint{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iActiveSkillPageNumber)>>::put(w, m_iActiveSkillPageNumber);
        Codec<std::remove_cvref_t<decltype(m_mapSkillList)>>::put(w, m_mapSkillList);
        Codec<std::remove_cvref_t<decltype(m_iCSPoint)>>::put(w, m_iCSPoint);
        Codec<std::remove_cvref_t<decltype(m_iTotalSpendSkillPoint)>>::put(w, m_iTotalSpendSkillPoint);
        Codec<std::remove_cvref_t<decltype(m_iBeforeCSPoint)>>::put(w, m_iBeforeCSPoint);
    }

    static KDBE_INSERT_SKILL_REQ read_from(Reader& r) {
        KDBE_INSERT_SKILL_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iActiveSkillPageNumber = Codec<std::remove_cvref_t<decltype(v.m_iActiveSkillPageNumber)>>::get(r);
        v.m_mapSkillList = Codec<std::remove_cvref_t<decltype(v.m_mapSkillList)>>::get(r);
        v.m_iCSPoint = Codec<std::remove_cvref_t<decltype(v.m_iCSPoint)>>::get(r);
        v.m_iTotalSpendSkillPoint = Codec<std::remove_cvref_t<decltype(v.m_iTotalSpendSkillPoint)>>::get(r);
        v.m_iBeforeCSPoint = Codec<std::remove_cvref_t<decltype(v.m_iBeforeCSPoint)>>::get(r);
        return v;
    }
};

struct KEGS_GET_SKILL_ACK {
    std::int32_t                          m_iOK{};  // int
    std::int32_t                          m_iActiveSkillPageNumber{};  // int
    std::map<std::int32_t, KGetSkillInfo> m_mapSkillList{};  // std::map< int, KGetSkillInfo >
    std::int32_t                          m_iRemainSP{};  // int
    std::int32_t                          m_iRemainCSP{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iActiveSkillPageNumber)>>::put(w, m_iActiveSkillPageNumber);
        Codec<std::remove_cvref_t<decltype(m_mapSkillList)>>::put(w, m_mapSkillList);
        Codec<std::remove_cvref_t<decltype(m_iRemainSP)>>::put(w, m_iRemainSP);
        Codec<std::remove_cvref_t<decltype(m_iRemainCSP)>>::put(w, m_iRemainCSP);
    }

    static KEGS_GET_SKILL_ACK read_from(Reader& r) {
        KEGS_GET_SKILL_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iActiveSkillPageNumber = Codec<std::remove_cvref_t<decltype(v.m_iActiveSkillPageNumber)>>::get(r);
        v.m_mapSkillList = Codec<std::remove_cvref_t<decltype(v.m_mapSkillList)>>::get(r);
        v.m_iRemainSP = Codec<std::remove_cvref_t<decltype(v.m_iRemainSP)>>::get(r);
        v.m_iRemainCSP = Codec<std::remove_cvref_t<decltype(v.m_iRemainCSP)>>::get(r);
        return v;
    }
};

struct KEGS_GET_SKILL_REQ {
    std::vector<std::int32_t>             m_vecNowLearnSkill{};  // std::vector<int>
    std::int32_t                          m_iActiveSkillPageNumber{};  // int
    std::map<std::int32_t, KGetSkillInfo> m_mapSkillList{};  // std::map< int, KGetSkillInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecNowLearnSkill)>>::put(w, m_vecNowLearnSkill);
        Codec<std::remove_cvref_t<decltype(m_iActiveSkillPageNumber)>>::put(w, m_iActiveSkillPageNumber);
        Codec<std::remove_cvref_t<decltype(m_mapSkillList)>>::put(w, m_mapSkillList);
    }

    static KEGS_GET_SKILL_REQ read_from(Reader& r) {
        KEGS_GET_SKILL_REQ v;
        v.m_vecNowLearnSkill = Codec<std::remove_cvref_t<decltype(v.m_vecNowLearnSkill)>>::get(r);
        v.m_iActiveSkillPageNumber = Codec<std::remove_cvref_t<decltype(v.m_iActiveSkillPageNumber)>>::get(r);
        v.m_mapSkillList = Codec<std::remove_cvref_t<decltype(v.m_mapSkillList)>>::get(r);
        return v;
    }
};

struct KGiantDeletedUnitInfo {
    std::u16string m_wstrNickName{};  // std::wstring
    std::int64_t   m_iUnitUID{};  // UidType
    std::uint32_t  m_uiGiantUID{};  // unsigned int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_uiGiantUID)>>::put(w, m_uiGiantUID);
    }

    static KGiantDeletedUnitInfo read_from(Reader& r) {
        KGiantDeletedUnitInfo v;
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_uiGiantUID = Codec<std::remove_cvref_t<decltype(v.m_uiGiantUID)>>::get(r);
        return v;
    }
};

struct KGlobalCashInfo {
    std::array<std::uint32_t, 4> m_ulCash{};  // unsigned long[GCT_TOTAL_NUM]

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_ulCash[0])>>::put(w, m_ulCash[0]);
        Codec<std::remove_cvref_t<decltype(m_ulCash[1])>>::put(w, m_ulCash[1]);
        Codec<std::remove_cvref_t<decltype(m_ulCash[2])>>::put(w, m_ulCash[2]);
        Codec<std::remove_cvref_t<decltype(m_ulCash[3])>>::put(w, m_ulCash[3]);
    }

    static KGlobalCashInfo read_from(Reader& r) {
        KGlobalCashInfo v;
        v.m_ulCash[0] = Codec<std::remove_cvref_t<decltype(v.m_ulCash[0])>>::get(r);
        v.m_ulCash[1] = Codec<std::remove_cvref_t<decltype(v.m_ulCash[1])>>::get(r);
        v.m_ulCash[2] = Codec<std::remove_cvref_t<decltype(v.m_ulCash[2])>>::get(r);
        v.m_ulCash[3] = Codec<std::remove_cvref_t<decltype(v.m_ulCash[3])>>::get(r);
        return v;
    }
};

struct KEBILL_BUY_PRODUCT_ACK {
    std::int32_t                m_iOK{};  // int
    std::vector<KBillOrderInfo> m_vecBillOrderInfo{};  // std::vector< KBillOrderInfo >
    std::vector<std::int32_t>   m_vecPackageID{};  // std::vector< int >
    std::int32_t                m_iTotalCash{};  // int
    std::int32_t                m_iCashType{};  // int
    KGlobalCashInfo             m_RemainCashInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_vecBillOrderInfo)>>::put(w, m_vecBillOrderInfo);
        Codec<std::remove_cvref_t<decltype(m_vecPackageID)>>::put(w, m_vecPackageID);
        Codec<std::remove_cvref_t<decltype(m_iTotalCash)>>::put(w, m_iTotalCash);
        Codec<std::remove_cvref_t<decltype(m_iCashType)>>::put(w, m_iCashType);
        Codec<std::remove_cvref_t<decltype(m_RemainCashInfo)>>::put(w, m_RemainCashInfo);
    }

    static KEBILL_BUY_PRODUCT_ACK read_from(Reader& r) {
        KEBILL_BUY_PRODUCT_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_vecBillOrderInfo = Codec<std::remove_cvref_t<decltype(v.m_vecBillOrderInfo)>>::get(r);
        v.m_vecPackageID = Codec<std::remove_cvref_t<decltype(v.m_vecPackageID)>>::get(r);
        v.m_iTotalCash = Codec<std::remove_cvref_t<decltype(v.m_iTotalCash)>>::get(r);
        v.m_iCashType = Codec<std::remove_cvref_t<decltype(v.m_iCashType)>>::get(r);
        v.m_RemainCashInfo = Codec<std::remove_cvref_t<decltype(v.m_RemainCashInfo)>>::get(r);
        return v;
    }
};

struct KEBILL_EXCHANGE_CASH_ACK {
    std::int32_t    m_iOK{};  // int
    std::int64_t    m_iUnitUID{};  // UidType
    std::int32_t    m_iTotalCash{};  // int
    std::int32_t    m_iChargeCash{};  // int
    std::int32_t    m_iChargeBonus{};  // int
    std::int32_t    m_iBonusItem{};  // int
    KGlobalCashInfo m_RemainCashInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iTotalCash)>>::put(w, m_iTotalCash);
        Codec<std::remove_cvref_t<decltype(m_iChargeCash)>>::put(w, m_iChargeCash);
        Codec<std::remove_cvref_t<decltype(m_iChargeBonus)>>::put(w, m_iChargeBonus);
        Codec<std::remove_cvref_t<decltype(m_iBonusItem)>>::put(w, m_iBonusItem);
        Codec<std::remove_cvref_t<decltype(m_RemainCashInfo)>>::put(w, m_RemainCashInfo);
    }

    static KEBILL_EXCHANGE_CASH_ACK read_from(Reader& r) {
        KEBILL_EXCHANGE_CASH_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iTotalCash = Codec<std::remove_cvref_t<decltype(v.m_iTotalCash)>>::get(r);
        v.m_iChargeCash = Codec<std::remove_cvref_t<decltype(v.m_iChargeCash)>>::get(r);
        v.m_iChargeBonus = Codec<std::remove_cvref_t<decltype(v.m_iChargeBonus)>>::get(r);
        v.m_iBonusItem = Codec<std::remove_cvref_t<decltype(v.m_iBonusItem)>>::get(r);
        v.m_RemainCashInfo = Codec<std::remove_cvref_t<decltype(v.m_RemainCashInfo)>>::get(r);
        return v;
    }
};

struct KEBILL_GIFT_ITEM_ACK {
    std::int32_t                m_iOK{};  // int
    std::vector<KBillOrderInfo> m_vecBillOrderInfo{};  // std::vector< KBillOrderInfo >
    std::int32_t                m_iTotalCash{};  // int
    std::int32_t                m_iCashType{};  // int
    KGlobalCashInfo             m_RemainCashInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_vecBillOrderInfo)>>::put(w, m_vecBillOrderInfo);
        Codec<std::remove_cvref_t<decltype(m_iTotalCash)>>::put(w, m_iTotalCash);
        Codec<std::remove_cvref_t<decltype(m_iCashType)>>::put(w, m_iCashType);
        Codec<std::remove_cvref_t<decltype(m_RemainCashInfo)>>::put(w, m_RemainCashInfo);
    }

    static KEBILL_GIFT_ITEM_ACK read_from(Reader& r) {
        KEBILL_GIFT_ITEM_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_vecBillOrderInfo = Codec<std::remove_cvref_t<decltype(v.m_vecBillOrderInfo)>>::get(r);
        v.m_iTotalCash = Codec<std::remove_cvref_t<decltype(v.m_iTotalCash)>>::get(r);
        v.m_iCashType = Codec<std::remove_cvref_t<decltype(v.m_iCashType)>>::get(r);
        v.m_RemainCashInfo = Codec<std::remove_cvref_t<decltype(v.m_RemainCashInfo)>>::get(r);
        return v;
    }
};

struct KEGS_CHECK_BALANCE_ACK {
    std::int32_t    m_iOK{};  // int
    std::uint32_t   m_ulBalance{};  // unsigned long
    bool            m_bOnlyType{};
    std::int32_t    m_iCashType{};  // int
    KGlobalCashInfo m_GlobalCashInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_ulBalance)>>::put(w, m_ulBalance);
        Codec<std::remove_cvref_t<decltype(m_bOnlyType)>>::put(w, m_bOnlyType);
        Codec<std::remove_cvref_t<decltype(m_iCashType)>>::put(w, m_iCashType);
        Codec<std::remove_cvref_t<decltype(m_GlobalCashInfo)>>::put(w, m_GlobalCashInfo);
    }

    static KEGS_CHECK_BALANCE_ACK read_from(Reader& r) {
        KEGS_CHECK_BALANCE_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_ulBalance = Codec<std::remove_cvref_t<decltype(v.m_ulBalance)>>::get(r);
        v.m_bOnlyType = Codec<std::remove_cvref_t<decltype(v.m_bOnlyType)>>::get(r);
        v.m_iCashType = Codec<std::remove_cvref_t<decltype(v.m_iCashType)>>::get(r);
        v.m_GlobalCashInfo = Codec<std::remove_cvref_t<decltype(v.m_GlobalCashInfo)>>::get(r);
        return v;
    }
};

struct KEGS_EXCHANGE_CASH_ACK {
    std::int32_t    m_iOK{};  // int
    std::int32_t    m_iPoint{};  // int
    KGlobalCashInfo m_AddBonusCashInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iPoint)>>::put(w, m_iPoint);
        Codec<std::remove_cvref_t<decltype(m_AddBonusCashInfo)>>::put(w, m_AddBonusCashInfo);
    }

    static KEGS_EXCHANGE_CASH_ACK read_from(Reader& r) {
        KEGS_EXCHANGE_CASH_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iPoint = Codec<std::remove_cvref_t<decltype(v.m_iPoint)>>::get(r);
        v.m_AddBonusCashInfo = Codec<std::remove_cvref_t<decltype(v.m_AddBonusCashInfo)>>::get(r);
        return v;
    }
};

struct KGlobalMissionInfo {
    bool           m_bActive{};
    std::u16string m_wstrMissionStartTime{};  // std::wstring
    std::int32_t   m_iDurationMinute{};  // int
    std::int32_t   m_iRemainSecond{};  // int
    std::int32_t   m_iMissionType{};  // int
    std::int32_t   m_iClearCondition{};  // int
    std::int32_t   m_iClearCount{};  // int
    std::int32_t   m_iNowCount{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bActive)>>::put(w, m_bActive);
        Codec<std::remove_cvref_t<decltype(m_wstrMissionStartTime)>>::put(w, m_wstrMissionStartTime);
        Codec<std::remove_cvref_t<decltype(m_iDurationMinute)>>::put(w, m_iDurationMinute);
        Codec<std::remove_cvref_t<decltype(m_iRemainSecond)>>::put(w, m_iRemainSecond);
        Codec<std::remove_cvref_t<decltype(m_iMissionType)>>::put(w, m_iMissionType);
        Codec<std::remove_cvref_t<decltype(m_iClearCondition)>>::put(w, m_iClearCondition);
        Codec<std::remove_cvref_t<decltype(m_iClearCount)>>::put(w, m_iClearCount);
        Codec<std::remove_cvref_t<decltype(m_iNowCount)>>::put(w, m_iNowCount);
    }

    static KGlobalMissionInfo read_from(Reader& r) {
        KGlobalMissionInfo v;
        v.m_bActive = Codec<std::remove_cvref_t<decltype(v.m_bActive)>>::get(r);
        v.m_wstrMissionStartTime = Codec<std::remove_cvref_t<decltype(v.m_wstrMissionStartTime)>>::get(r);
        v.m_iDurationMinute = Codec<std::remove_cvref_t<decltype(v.m_iDurationMinute)>>::get(r);
        v.m_iRemainSecond = Codec<std::remove_cvref_t<decltype(v.m_iRemainSecond)>>::get(r);
        v.m_iMissionType = Codec<std::remove_cvref_t<decltype(v.m_iMissionType)>>::get(r);
        v.m_iClearCondition = Codec<std::remove_cvref_t<decltype(v.m_iClearCondition)>>::get(r);
        v.m_iClearCount = Codec<std::remove_cvref_t<decltype(v.m_iClearCount)>>::get(r);
        v.m_iNowCount = Codec<std::remove_cvref_t<decltype(v.m_iNowCount)>>::get(r);
        return v;
    }
};

struct KDBE_GET_GLOBAL_MISSION_INFO_REQ {
    std::map<std::u16string, KGlobalMissionInfo> m_mapGlobalMissionInfo{};  // std::map< std::wstring, KGlobalMissionInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_mapGlobalMissionInfo)>>::put(w, m_mapGlobalMissionInfo);
    }

    static KDBE_GET_GLOBAL_MISSION_INFO_REQ read_from(Reader& r) {
        KDBE_GET_GLOBAL_MISSION_INFO_REQ v;
        v.m_mapGlobalMissionInfo = Codec<std::remove_cvref_t<decltype(v.m_mapGlobalMissionInfo)>>::get(r);
        return v;
    }
};

struct KEGB_GET_GLOBAL_MISSION_INFO_ACK {
    std::int32_t                                 m_iFlag{};  // int
    std::u16string                               m_wstrGlobalMissionDay{};  // std::wstring
    std::map<std::u16string, KGlobalMissionInfo> m_mapGlobalMissionInfo{};  // std::map< std::wstring, KGlobalMissionInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iFlag)>>::put(w, m_iFlag);
        Codec<std::remove_cvref_t<decltype(m_wstrGlobalMissionDay)>>::put(w, m_wstrGlobalMissionDay);
        Codec<std::remove_cvref_t<decltype(m_mapGlobalMissionInfo)>>::put(w, m_mapGlobalMissionInfo);
    }

    static KEGB_GET_GLOBAL_MISSION_INFO_ACK read_from(Reader& r) {
        KEGB_GET_GLOBAL_MISSION_INFO_ACK v;
        v.m_iFlag = Codec<std::remove_cvref_t<decltype(v.m_iFlag)>>::get(r);
        v.m_wstrGlobalMissionDay = Codec<std::remove_cvref_t<decltype(v.m_wstrGlobalMissionDay)>>::get(r);
        v.m_mapGlobalMissionInfo = Codec<std::remove_cvref_t<decltype(v.m_mapGlobalMissionInfo)>>::get(r);
        return v;
    }
};

struct KEGS_GLOBAL_MISSION_UPDATE_NOT {
    std::map<std::u16string, KGlobalMissionInfo> m_mapGlobalMissionInfo{};  // std::map< std::wstring, KGlobalMissionInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_mapGlobalMissionInfo)>>::put(w, m_mapGlobalMissionInfo);
    }

    static KEGS_GLOBAL_MISSION_UPDATE_NOT read_from(Reader& r) {
        KEGS_GLOBAL_MISSION_UPDATE_NOT v;
        v.m_mapGlobalMissionInfo = Codec<std::remove_cvref_t<decltype(v.m_mapGlobalMissionInfo)>>::get(r);
        return v;
    }
};

struct KGlobalMissionReward {
    std::int32_t m_iRewardType{};  // int
    std::int32_t m_iRewardDurationTime{};  // int
    std::int32_t m_iScriptID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iRewardType)>>::put(w, m_iRewardType);
        Codec<std::remove_cvref_t<decltype(m_iRewardDurationTime)>>::put(w, m_iRewardDurationTime);
        Codec<std::remove_cvref_t<decltype(m_iScriptID)>>::put(w, m_iScriptID);
    }

    static KGlobalMissionReward read_from(Reader& r) {
        KGlobalMissionReward v;
        v.m_iRewardType = Codec<std::remove_cvref_t<decltype(v.m_iRewardType)>>::get(r);
        v.m_iRewardDurationTime = Codec<std::remove_cvref_t<decltype(v.m_iRewardDurationTime)>>::get(r);
        v.m_iScriptID = Codec<std::remove_cvref_t<decltype(v.m_iScriptID)>>::get(r);
        return v;
    }
};

struct KGuildAdInfo {
    std::int32_t   m_iGuildUID{};  // int
    std::u16string m_wstrGuildName{};  // std::wstring
    std::uint8_t   m_ucGuildLevel{};  // u_char
    std::int32_t   m_iGuildEXP{};  // int
    std::u16string m_wstrMasterNickName{};  // std::wstring
    std::uint16_t  m_usCurNumMember{};  // u_short
    std::u16string m_wstrAdMessage{};  // std::wstring
    std::u16string m_wstrFoundingDay{};  // std::wstring
    std::u16string m_wstrAdRegDate{};  // std::wstring
    std::u16string m_wstrAdEndDate{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iGuildUID)>>::put(w, m_iGuildUID);
        Codec<std::remove_cvref_t<decltype(m_wstrGuildName)>>::put(w, m_wstrGuildName);
        Codec<std::remove_cvref_t<decltype(m_ucGuildLevel)>>::put(w, m_ucGuildLevel);
        Codec<std::remove_cvref_t<decltype(m_iGuildEXP)>>::put(w, m_iGuildEXP);
        Codec<std::remove_cvref_t<decltype(m_wstrMasterNickName)>>::put(w, m_wstrMasterNickName);
        Codec<std::remove_cvref_t<decltype(m_usCurNumMember)>>::put(w, m_usCurNumMember);
        Codec<std::remove_cvref_t<decltype(m_wstrAdMessage)>>::put(w, m_wstrAdMessage);
        Codec<std::remove_cvref_t<decltype(m_wstrFoundingDay)>>::put(w, m_wstrFoundingDay);
        Codec<std::remove_cvref_t<decltype(m_wstrAdRegDate)>>::put(w, m_wstrAdRegDate);
        Codec<std::remove_cvref_t<decltype(m_wstrAdEndDate)>>::put(w, m_wstrAdEndDate);
    }

    static KGuildAdInfo read_from(Reader& r) {
        KGuildAdInfo v;
        v.m_iGuildUID = Codec<std::remove_cvref_t<decltype(v.m_iGuildUID)>>::get(r);
        v.m_wstrGuildName = Codec<std::remove_cvref_t<decltype(v.m_wstrGuildName)>>::get(r);
        v.m_ucGuildLevel = Codec<std::remove_cvref_t<decltype(v.m_ucGuildLevel)>>::get(r);
        v.m_iGuildEXP = Codec<std::remove_cvref_t<decltype(v.m_iGuildEXP)>>::get(r);
        v.m_wstrMasterNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrMasterNickName)>>::get(r);
        v.m_usCurNumMember = Codec<std::remove_cvref_t<decltype(v.m_usCurNumMember)>>::get(r);
        v.m_wstrAdMessage = Codec<std::remove_cvref_t<decltype(v.m_wstrAdMessage)>>::get(r);
        v.m_wstrFoundingDay = Codec<std::remove_cvref_t<decltype(v.m_wstrFoundingDay)>>::get(r);
        v.m_wstrAdRegDate = Codec<std::remove_cvref_t<decltype(v.m_wstrAdRegDate)>>::get(r);
        v.m_wstrAdEndDate = Codec<std::remove_cvref_t<decltype(v.m_wstrAdEndDate)>>::get(r);
        return v;
    }
};

struct KDBE_GET_GUILD_BOARD_INFO_ACK {
    std::vector<KGuildAdInfo>        m_vecGuildAdList{};  // std::vector< KGuildAdInfo >
    std::vector<KApplyJoinGuildInfo> m_vecApplyJoinGuildList{};  // std::vector< KApplyJoinGuildInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecGuildAdList)>>::put(w, m_vecGuildAdList);
        Codec<std::remove_cvref_t<decltype(m_vecApplyJoinGuildList)>>::put(w, m_vecApplyJoinGuildList);
    }

    static KDBE_GET_GUILD_BOARD_INFO_ACK read_from(Reader& r) {
        KDBE_GET_GUILD_BOARD_INFO_ACK v;
        v.m_vecGuildAdList = Codec<std::remove_cvref_t<decltype(v.m_vecGuildAdList)>>::get(r);
        v.m_vecApplyJoinGuildList = Codec<std::remove_cvref_t<decltype(v.m_vecApplyJoinGuildList)>>::get(r);
        return v;
    }
};

struct KEGS_GET_GUILD_AD_LIST_ACK {
    char                      m_cSortType{};
    std::uint32_t             m_uiTotalPage{};  // u_int
    std::uint32_t             m_uiViewPage{};  // u_int
    std::vector<KGuildAdInfo> m_vecGuildAdList{};  // std::vector< KGuildAdInfo >
    bool                      m_bCanShowApplyList{};
    bool                      m_bRegMyGuildAd{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cSortType)>>::put(w, m_cSortType);
        Codec<std::remove_cvref_t<decltype(m_uiTotalPage)>>::put(w, m_uiTotalPage);
        Codec<std::remove_cvref_t<decltype(m_uiViewPage)>>::put(w, m_uiViewPage);
        Codec<std::remove_cvref_t<decltype(m_vecGuildAdList)>>::put(w, m_vecGuildAdList);
        Codec<std::remove_cvref_t<decltype(m_bCanShowApplyList)>>::put(w, m_bCanShowApplyList);
        Codec<std::remove_cvref_t<decltype(m_bRegMyGuildAd)>>::put(w, m_bRegMyGuildAd);
    }

    static KEGS_GET_GUILD_AD_LIST_ACK read_from(Reader& r) {
        KEGS_GET_GUILD_AD_LIST_ACK v;
        v.m_cSortType = Codec<std::remove_cvref_t<decltype(v.m_cSortType)>>::get(r);
        v.m_uiTotalPage = Codec<std::remove_cvref_t<decltype(v.m_uiTotalPage)>>::get(r);
        v.m_uiViewPage = Codec<std::remove_cvref_t<decltype(v.m_uiViewPage)>>::get(r);
        v.m_vecGuildAdList = Codec<std::remove_cvref_t<decltype(v.m_vecGuildAdList)>>::get(r);
        v.m_bCanShowApplyList = Codec<std::remove_cvref_t<decltype(v.m_bCanShowApplyList)>>::get(r);
        v.m_bRegMyGuildAd = Codec<std::remove_cvref_t<decltype(v.m_bRegMyGuildAd)>>::get(r);
        return v;
    }
};

struct KGuildInfo {
    std::int32_t   m_iGuildUID{};  // int
    std::u16string m_wstrGuildName{};  // std::wstring
    std::uint16_t  m_usMaxNumMember{};  // u_short
    std::uint8_t   m_ucGuildLevel{};  // u_char
    std::int32_t   m_iGuildEXP{};  // int
    std::u16string m_wstrGuildMessage{};  // std::wstring
    std::u16string m_wstrFoundingDay{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iGuildUID)>>::put(w, m_iGuildUID);
        Codec<std::remove_cvref_t<decltype(m_wstrGuildName)>>::put(w, m_wstrGuildName);
        Codec<std::remove_cvref_t<decltype(m_usMaxNumMember)>>::put(w, m_usMaxNumMember);
        Codec<std::remove_cvref_t<decltype(m_ucGuildLevel)>>::put(w, m_ucGuildLevel);
        Codec<std::remove_cvref_t<decltype(m_iGuildEXP)>>::put(w, m_iGuildEXP);
        Codec<std::remove_cvref_t<decltype(m_wstrGuildMessage)>>::put(w, m_wstrGuildMessage);
        Codec<std::remove_cvref_t<decltype(m_wstrFoundingDay)>>::put(w, m_wstrFoundingDay);
    }

    static KGuildInfo read_from(Reader& r) {
        KGuildInfo v;
        v.m_iGuildUID = Codec<std::remove_cvref_t<decltype(v.m_iGuildUID)>>::get(r);
        v.m_wstrGuildName = Codec<std::remove_cvref_t<decltype(v.m_wstrGuildName)>>::get(r);
        v.m_usMaxNumMember = Codec<std::remove_cvref_t<decltype(v.m_usMaxNumMember)>>::get(r);
        v.m_ucGuildLevel = Codec<std::remove_cvref_t<decltype(v.m_ucGuildLevel)>>::get(r);
        v.m_iGuildEXP = Codec<std::remove_cvref_t<decltype(v.m_iGuildEXP)>>::get(r);
        v.m_wstrGuildMessage = Codec<std::remove_cvref_t<decltype(v.m_wstrGuildMessage)>>::get(r);
        v.m_wstrFoundingDay = Codec<std::remove_cvref_t<decltype(v.m_wstrFoundingDay)>>::get(r);
        return v;
    }
};

struct KChangeGuildNameACK {
    std::int32_t   m_iOK{};  // int
    std::int64_t   m_iUnitUID{};  // UidType
    KGuildInfo     m_kGuildInfo{};
    std::u16string m_wstrOldGuildName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_kGuildInfo)>>::put(w, m_kGuildInfo);
        Codec<std::remove_cvref_t<decltype(m_wstrOldGuildName)>>::put(w, m_wstrOldGuildName);
    }

    static KChangeGuildNameACK read_from(Reader& r) {
        KChangeGuildNameACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_kGuildInfo = Codec<std::remove_cvref_t<decltype(v.m_kGuildInfo)>>::get(r);
        v.m_wstrOldGuildName = Codec<std::remove_cvref_t<decltype(v.m_wstrOldGuildName)>>::get(r);
        return v;
    }
};

struct KEGS_CHANGE_GUILD_NAME_ACK {
    std::int32_t m_iOK{};  // int
    KGuildInfo   m_kGuildInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_kGuildInfo)>>::put(w, m_kGuildInfo);
    }

    static KEGS_CHANGE_GUILD_NAME_ACK read_from(Reader& r) {
        KEGS_CHANGE_GUILD_NAME_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_kGuildInfo = Codec<std::remove_cvref_t<decltype(v.m_kGuildInfo)>>::get(r);
        return v;
    }
};

struct KEGS_INVITE_GUILD_NOT {
    KGuildInfo     m_kGuildInfo{};
    std::u16string m_wstrSenderNickName{};  // std::wstring
    bool           m_bAutoJoinGuild{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kGuildInfo)>>::put(w, m_kGuildInfo);
        Codec<std::remove_cvref_t<decltype(m_wstrSenderNickName)>>::put(w, m_wstrSenderNickName);
        Codec<std::remove_cvref_t<decltype(m_bAutoJoinGuild)>>::put(w, m_bAutoJoinGuild);
    }

    static KEGS_INVITE_GUILD_NOT read_from(Reader& r) {
        KEGS_INVITE_GUILD_NOT v;
        v.m_kGuildInfo = Codec<std::remove_cvref_t<decltype(v.m_kGuildInfo)>>::get(r);
        v.m_wstrSenderNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrSenderNickName)>>::get(r);
        v.m_bAutoJoinGuild = Codec<std::remove_cvref_t<decltype(v.m_bAutoJoinGuild)>>::get(r);
        return v;
    }
};

struct KELG_INVITE_GUILD_FIND_USER_REQ {
    std::uint16_t  m_usEventID{};  // u_short
    KGuildInfo     m_kGuildInfo{};
    std::int64_t   m_iSenderUnitUID{};  // UidType
    std::u16string m_wstrSenderNickName{};  // std::wstring
    std::int64_t   m_iReceiverUnitUID{};  // UidType
    bool           m_bAutoJoinGuild{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_usEventID)>>::put(w, m_usEventID);
        Codec<std::remove_cvref_t<decltype(m_kGuildInfo)>>::put(w, m_kGuildInfo);
        Codec<std::remove_cvref_t<decltype(m_iSenderUnitUID)>>::put(w, m_iSenderUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrSenderNickName)>>::put(w, m_wstrSenderNickName);
        Codec<std::remove_cvref_t<decltype(m_iReceiverUnitUID)>>::put(w, m_iReceiverUnitUID);
        Codec<std::remove_cvref_t<decltype(m_bAutoJoinGuild)>>::put(w, m_bAutoJoinGuild);
    }

    static KELG_INVITE_GUILD_FIND_USER_REQ read_from(Reader& r) {
        KELG_INVITE_GUILD_FIND_USER_REQ v;
        v.m_usEventID = Codec<std::remove_cvref_t<decltype(v.m_usEventID)>>::get(r);
        v.m_kGuildInfo = Codec<std::remove_cvref_t<decltype(v.m_kGuildInfo)>>::get(r);
        v.m_iSenderUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iSenderUnitUID)>>::get(r);
        v.m_wstrSenderNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrSenderNickName)>>::get(r);
        v.m_iReceiverUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iReceiverUnitUID)>>::get(r);
        v.m_bAutoJoinGuild = Codec<std::remove_cvref_t<decltype(v.m_bAutoJoinGuild)>>::get(r);
        return v;
    }
};

struct KGuildMemberInfo {
    std::int64_t   m_iGSUID{};  // UidType
    std::int64_t   m_iUnitUID{};  // UidType
    std::u16string m_wstrNickName{};  // std::wstring
    char           m_cUnitClass{};
    std::uint8_t   m_ucLevel{};  // UCHAR
    std::uint8_t   m_ucMemberShipGrade{};  // u_char
    std::int32_t   m_iChannelID{};  // int
    std::int32_t   m_iMapID{};  // int
    char           m_cPosition{};
    std::int64_t   m_tLogInTime{};  // __int64
    std::int64_t   m_tLogOutTime{};  // __int64
    std::u16string m_wstrMessage{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iGSUID)>>::put(w, m_iGSUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_cUnitClass)>>::put(w, m_cUnitClass);
        Codec<std::remove_cvref_t<decltype(m_ucLevel)>>::put(w, m_ucLevel);
        Codec<std::remove_cvref_t<decltype(m_ucMemberShipGrade)>>::put(w, m_ucMemberShipGrade);
        Codec<std::remove_cvref_t<decltype(m_iChannelID)>>::put(w, m_iChannelID);
        Codec<std::remove_cvref_t<decltype(m_iMapID)>>::put(w, m_iMapID);
        Codec<std::remove_cvref_t<decltype(m_cPosition)>>::put(w, m_cPosition);
        Codec<std::remove_cvref_t<decltype(m_tLogInTime)>>::put(w, m_tLogInTime);
        Codec<std::remove_cvref_t<decltype(m_tLogOutTime)>>::put(w, m_tLogOutTime);
        Codec<std::remove_cvref_t<decltype(m_wstrMessage)>>::put(w, m_wstrMessage);
    }

    static KGuildMemberInfo read_from(Reader& r) {
        KGuildMemberInfo v;
        v.m_iGSUID = Codec<std::remove_cvref_t<decltype(v.m_iGSUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_cUnitClass = Codec<std::remove_cvref_t<decltype(v.m_cUnitClass)>>::get(r);
        v.m_ucLevel = Codec<std::remove_cvref_t<decltype(v.m_ucLevel)>>::get(r);
        v.m_ucMemberShipGrade = Codec<std::remove_cvref_t<decltype(v.m_ucMemberShipGrade)>>::get(r);
        v.m_iChannelID = Codec<std::remove_cvref_t<decltype(v.m_iChannelID)>>::get(r);
        v.m_iMapID = Codec<std::remove_cvref_t<decltype(v.m_iMapID)>>::get(r);
        v.m_cPosition = Codec<std::remove_cvref_t<decltype(v.m_cPosition)>>::get(r);
        v.m_tLogInTime = Codec<std::remove_cvref_t<decltype(v.m_tLogInTime)>>::get(r);
        v.m_tLogOutTime = Codec<std::remove_cvref_t<decltype(v.m_tLogOutTime)>>::get(r);
        v.m_wstrMessage = Codec<std::remove_cvref_t<decltype(v.m_wstrMessage)>>::get(r);
        return v;
    }
};

struct KDBE_ACCEPT_JOIN_GUILD_ACK {
    std::int32_t     m_iOK{};  // int
    std::int32_t     m_iGuildUID{};  // int
    KGuildMemberInfo m_kJoinGuildMemberInfo{};
    std::u16string   m_wstrGuildName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iGuildUID)>>::put(w, m_iGuildUID);
        Codec<std::remove_cvref_t<decltype(m_kJoinGuildMemberInfo)>>::put(w, m_kJoinGuildMemberInfo);
        Codec<std::remove_cvref_t<decltype(m_wstrGuildName)>>::put(w, m_wstrGuildName);
    }

    static KDBE_ACCEPT_JOIN_GUILD_ACK read_from(Reader& r) {
        KDBE_ACCEPT_JOIN_GUILD_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iGuildUID = Codec<std::remove_cvref_t<decltype(v.m_iGuildUID)>>::get(r);
        v.m_kJoinGuildMemberInfo = Codec<std::remove_cvref_t<decltype(v.m_kJoinGuildMemberInfo)>>::get(r);
        v.m_wstrGuildName = Codec<std::remove_cvref_t<decltype(v.m_wstrGuildName)>>::get(r);
        return v;
    }
};

struct KDBE_ACCEPT_JOIN_GUILD_REQ {
    std::int32_t     m_iGuildUID{};  // int
    KGuildMemberInfo m_kJoinGuildMemberInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iGuildUID)>>::put(w, m_iGuildUID);
        Codec<std::remove_cvref_t<decltype(m_kJoinGuildMemberInfo)>>::put(w, m_kJoinGuildMemberInfo);
    }

    static KDBE_ACCEPT_JOIN_GUILD_REQ read_from(Reader& r) {
        KDBE_ACCEPT_JOIN_GUILD_REQ v;
        v.m_iGuildUID = Codec<std::remove_cvref_t<decltype(v.m_iGuildUID)>>::get(r);
        v.m_kJoinGuildMemberInfo = Codec<std::remove_cvref_t<decltype(v.m_kJoinGuildMemberInfo)>>::get(r);
        return v;
    }
};

struct KDBE_JOIN_GUILD_ACK {
    std::int32_t     m_iOK{};  // int
    std::int32_t     m_iGuildUID{};  // int
    KGuildMemberInfo m_kJoinGuildMember{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iGuildUID)>>::put(w, m_iGuildUID);
        Codec<std::remove_cvref_t<decltype(m_kJoinGuildMember)>>::put(w, m_kJoinGuildMember);
    }

    static KDBE_JOIN_GUILD_ACK read_from(Reader& r) {
        KDBE_JOIN_GUILD_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iGuildUID = Codec<std::remove_cvref_t<decltype(v.m_iGuildUID)>>::get(r);
        v.m_kJoinGuildMember = Codec<std::remove_cvref_t<decltype(v.m_kJoinGuildMember)>>::get(r);
        return v;
    }
};

struct KDBE_JOIN_GUILD_REQ {
    std::int32_t     m_iGuildUID{};  // int
    KGuildMemberInfo m_kJoinGuildMember{};
    std::int64_t     m_iGuildMasterUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iGuildUID)>>::put(w, m_iGuildUID);
        Codec<std::remove_cvref_t<decltype(m_kJoinGuildMember)>>::put(w, m_kJoinGuildMember);
        Codec<std::remove_cvref_t<decltype(m_iGuildMasterUnitUID)>>::put(w, m_iGuildMasterUnitUID);
    }

    static KDBE_JOIN_GUILD_REQ read_from(Reader& r) {
        KDBE_JOIN_GUILD_REQ v;
        v.m_iGuildUID = Codec<std::remove_cvref_t<decltype(v.m_iGuildUID)>>::get(r);
        v.m_kJoinGuildMember = Codec<std::remove_cvref_t<decltype(v.m_kJoinGuildMember)>>::get(r);
        v.m_iGuildMasterUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iGuildMasterUnitUID)>>::get(r);
        return v;
    }
};

struct KEGS_GUILD_MEMBER_INFO_NOT {
    KGuildMemberInfo m_kMemberInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kMemberInfo)>>::put(w, m_kMemberInfo);
    }

    static KEGS_GUILD_MEMBER_INFO_NOT read_from(Reader& r) {
        KEGS_GUILD_MEMBER_INFO_NOT v;
        v.m_kMemberInfo = Codec<std::remove_cvref_t<decltype(v.m_kMemberInfo)>>::get(r);
        return v;
    }
};

struct KELG_GET_GUILD_INFO_REQ {
    std::int64_t     m_iUnitUID{};  // UidType
    std::int32_t     m_iGuildUID{};  // int
    KGuildMemberInfo m_kLoginGuildMember{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iGuildUID)>>::put(w, m_iGuildUID);
        Codec<std::remove_cvref_t<decltype(m_kLoginGuildMember)>>::put(w, m_kLoginGuildMember);
    }

    static KELG_GET_GUILD_INFO_REQ read_from(Reader& r) {
        KELG_GET_GUILD_INFO_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iGuildUID = Codec<std::remove_cvref_t<decltype(v.m_iGuildUID)>>::get(r);
        v.m_kLoginGuildMember = Codec<std::remove_cvref_t<decltype(v.m_kLoginGuildMember)>>::get(r);
        return v;
    }
};

struct KELG_JOIN_GUILD_REQ {
    std::int32_t     m_iOK{};  // int
    std::int64_t     m_iUnitUID{};  // UidType
    std::int32_t     m_iGuildUID{};  // int
    KGuildMemberInfo m_kLoginGuildMember{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iGuildUID)>>::put(w, m_iGuildUID);
        Codec<std::remove_cvref_t<decltype(m_kLoginGuildMember)>>::put(w, m_kLoginGuildMember);
    }

    static KELG_JOIN_GUILD_REQ read_from(Reader& r) {
        KELG_JOIN_GUILD_REQ v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iGuildUID = Codec<std::remove_cvref_t<decltype(v.m_iGuildUID)>>::get(r);
        v.m_kLoginGuildMember = Codec<std::remove_cvref_t<decltype(v.m_kLoginGuildMember)>>::get(r);
        return v;
    }
};

struct KELG_UPDATE_GUILD_MEMBER_INFO_NOT {
    std::int32_t     m_iGuildUID{};  // int
    KGuildMemberInfo m_kMemberInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iGuildUID)>>::put(w, m_iGuildUID);
        Codec<std::remove_cvref_t<decltype(m_kMemberInfo)>>::put(w, m_kMemberInfo);
    }

    static KELG_UPDATE_GUILD_MEMBER_INFO_NOT read_from(Reader& r) {
        KELG_UPDATE_GUILD_MEMBER_INFO_NOT v;
        v.m_iGuildUID = Codec<std::remove_cvref_t<decltype(v.m_iGuildUID)>>::get(r);
        v.m_kMemberInfo = Codec<std::remove_cvref_t<decltype(v.m_kMemberInfo)>>::get(r);
        return v;
    }
};

struct KGuildMemberMessageInfo {
    std::int64_t   m_iUnitUID{};  // UidType
    std::u16string m_wstrNickName{};  // std::wstring
    std::uint8_t   m_ucMemberShipGrade{};  // u_char
    char           m_cUnitClass{};
    std::uint8_t   m_ucLevel{};  // UCHAR
    std::u16string m_wstrMessage{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_ucMemberShipGrade)>>::put(w, m_ucMemberShipGrade);
        Codec<std::remove_cvref_t<decltype(m_cUnitClass)>>::put(w, m_cUnitClass);
        Codec<std::remove_cvref_t<decltype(m_ucLevel)>>::put(w, m_ucLevel);
        Codec<std::remove_cvref_t<decltype(m_wstrMessage)>>::put(w, m_wstrMessage);
    }

    static KGuildMemberMessageInfo read_from(Reader& r) {
        KGuildMemberMessageInfo v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_ucMemberShipGrade = Codec<std::remove_cvref_t<decltype(v.m_ucMemberShipGrade)>>::get(r);
        v.m_cUnitClass = Codec<std::remove_cvref_t<decltype(v.m_cUnitClass)>>::get(r);
        v.m_ucLevel = Codec<std::remove_cvref_t<decltype(v.m_ucLevel)>>::get(r);
        v.m_wstrMessage = Codec<std::remove_cvref_t<decltype(v.m_wstrMessage)>>::get(r);
        return v;
    }
};

struct KEGS_GET_GUILD_USER_LIST_ACK {
    std::int32_t                         m_iOK{};  // int
    std::uint32_t                        m_uiTotalPage{};  // UINT
    std::uint32_t                        m_uiViewPage{};  // UINT
    char                                 m_cGuildUserListType{};
    std::int32_t                         m_iLoginedMemberCount{};  // int
    std::vector<KGuildMemberInfo>        m_vecMemberPositionGradeList{};  // std::vector< KGuildMemberInfo >
    std::vector<KGuildMemberMessageInfo> m_vecMemberMessageList{};  // std::vector< KGuildMemberMessageInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_uiTotalPage)>>::put(w, m_uiTotalPage);
        Codec<std::remove_cvref_t<decltype(m_uiViewPage)>>::put(w, m_uiViewPage);
        Codec<std::remove_cvref_t<decltype(m_cGuildUserListType)>>::put(w, m_cGuildUserListType);
        Codec<std::remove_cvref_t<decltype(m_iLoginedMemberCount)>>::put(w, m_iLoginedMemberCount);
        Codec<std::remove_cvref_t<decltype(m_vecMemberPositionGradeList)>>::put(w, m_vecMemberPositionGradeList);
        Codec<std::remove_cvref_t<decltype(m_vecMemberMessageList)>>::put(w, m_vecMemberMessageList);
    }

    static KEGS_GET_GUILD_USER_LIST_ACK read_from(Reader& r) {
        KEGS_GET_GUILD_USER_LIST_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_uiTotalPage = Codec<std::remove_cvref_t<decltype(v.m_uiTotalPage)>>::get(r);
        v.m_uiViewPage = Codec<std::remove_cvref_t<decltype(v.m_uiViewPage)>>::get(r);
        v.m_cGuildUserListType = Codec<std::remove_cvref_t<decltype(v.m_cGuildUserListType)>>::get(r);
        v.m_iLoginedMemberCount = Codec<std::remove_cvref_t<decltype(v.m_iLoginedMemberCount)>>::get(r);
        v.m_vecMemberPositionGradeList = Codec<std::remove_cvref_t<decltype(v.m_vecMemberPositionGradeList)>>::get(r);
        v.m_vecMemberMessageList = Codec<std::remove_cvref_t<decltype(v.m_vecMemberMessageList)>>::get(r);
        return v;
    }
};

struct KGuildSkillData {
    std::int16_t m_iSkillID{};  // short
    std::uint8_t m_cSkillLevel{};  // u_char
    std::uint8_t m_cSkillCSPoint{};  // u_char

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iSkillID)>>::put(w, m_iSkillID);
        Codec<std::remove_cvref_t<decltype(m_cSkillLevel)>>::put(w, m_cSkillLevel);
        Codec<std::remove_cvref_t<decltype(m_cSkillCSPoint)>>::put(w, m_cSkillCSPoint);
    }

    static KGuildSkillData read_from(Reader& r) {
        KGuildSkillData v;
        v.m_iSkillID = Codec<std::remove_cvref_t<decltype(v.m_iSkillID)>>::get(r);
        v.m_cSkillLevel = Codec<std::remove_cvref_t<decltype(v.m_cSkillLevel)>>::get(r);
        v.m_cSkillCSPoint = Codec<std::remove_cvref_t<decltype(v.m_cSkillCSPoint)>>::get(r);
        return v;
    }
};

struct KDBE_EXPIRE_GUILD_CASH_SKILL_POINT_REQ {
    std::int32_t                 m_iGuildUID{};  // int
    std::int32_t                 m_iRetrievedSPoint{};  // int
    std::int32_t                 m_iGuildSPoint{};  // int
    std::vector<KGuildSkillData> m_vecGuildSkillData{};  // std::vector< KGuildSkillData >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iGuildUID)>>::put(w, m_iGuildUID);
        Codec<std::remove_cvref_t<decltype(m_iRetrievedSPoint)>>::put(w, m_iRetrievedSPoint);
        Codec<std::remove_cvref_t<decltype(m_iGuildSPoint)>>::put(w, m_iGuildSPoint);
        Codec<std::remove_cvref_t<decltype(m_vecGuildSkillData)>>::put(w, m_vecGuildSkillData);
    }

    static KDBE_EXPIRE_GUILD_CASH_SKILL_POINT_REQ read_from(Reader& r) {
        KDBE_EXPIRE_GUILD_CASH_SKILL_POINT_REQ v;
        v.m_iGuildUID = Codec<std::remove_cvref_t<decltype(v.m_iGuildUID)>>::get(r);
        v.m_iRetrievedSPoint = Codec<std::remove_cvref_t<decltype(v.m_iRetrievedSPoint)>>::get(r);
        v.m_iGuildSPoint = Codec<std::remove_cvref_t<decltype(v.m_iGuildSPoint)>>::get(r);
        v.m_vecGuildSkillData = Codec<std::remove_cvref_t<decltype(v.m_vecGuildSkillData)>>::get(r);
        return v;
    }
};

struct KEGS_EXPIRE_GUILD_CASH_SKILL_POINT_NOT {
    std::int32_t                 m_iGuildSPoint{};  // int
    std::int32_t                 m_iGuildCSPoint{};  // int
    std::vector<KGuildSkillData> m_vecGuildSkillData{};  // std::vector< KGuildSkillData >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iGuildSPoint)>>::put(w, m_iGuildSPoint);
        Codec<std::remove_cvref_t<decltype(m_iGuildCSPoint)>>::put(w, m_iGuildCSPoint);
        Codec<std::remove_cvref_t<decltype(m_vecGuildSkillData)>>::put(w, m_vecGuildSkillData);
    }

    static KEGS_EXPIRE_GUILD_CASH_SKILL_POINT_NOT read_from(Reader& r) {
        KEGS_EXPIRE_GUILD_CASH_SKILL_POINT_NOT v;
        v.m_iGuildSPoint = Codec<std::remove_cvref_t<decltype(v.m_iGuildSPoint)>>::get(r);
        v.m_iGuildCSPoint = Codec<std::remove_cvref_t<decltype(v.m_iGuildCSPoint)>>::get(r);
        v.m_vecGuildSkillData = Codec<std::remove_cvref_t<decltype(v.m_vecGuildSkillData)>>::get(r);
        return v;
    }
};

struct KGuildSkillInfo {
    std::vector<KGuildSkillData> m_vecGuildSkill{};  // std::vector< KGuildSkillData >
    std::int32_t                 m_iGuildSPoint{};  // int
    std::int32_t                 m_iGuildCSPoint{};  // int
    std::int32_t                 m_iMaxGuildCSPoint{};  // int
    std::u16string               m_wstrGuildCSPointEndDate{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecGuildSkill)>>::put(w, m_vecGuildSkill);
        Codec<std::remove_cvref_t<decltype(m_iGuildSPoint)>>::put(w, m_iGuildSPoint);
        Codec<std::remove_cvref_t<decltype(m_iGuildCSPoint)>>::put(w, m_iGuildCSPoint);
        Codec<std::remove_cvref_t<decltype(m_iMaxGuildCSPoint)>>::put(w, m_iMaxGuildCSPoint);
        Codec<std::remove_cvref_t<decltype(m_wstrGuildCSPointEndDate)>>::put(w, m_wstrGuildCSPointEndDate);
    }

    static KGuildSkillInfo read_from(Reader& r) {
        KGuildSkillInfo v;
        v.m_vecGuildSkill = Codec<std::remove_cvref_t<decltype(v.m_vecGuildSkill)>>::get(r);
        v.m_iGuildSPoint = Codec<std::remove_cvref_t<decltype(v.m_iGuildSPoint)>>::get(r);
        v.m_iGuildCSPoint = Codec<std::remove_cvref_t<decltype(v.m_iGuildCSPoint)>>::get(r);
        v.m_iMaxGuildCSPoint = Codec<std::remove_cvref_t<decltype(v.m_iMaxGuildCSPoint)>>::get(r);
        v.m_wstrGuildCSPointEndDate = Codec<std::remove_cvref_t<decltype(v.m_wstrGuildCSPointEndDate)>>::get(r);
        return v;
    }
};

struct KCreateGuildACK {
    std::int32_t    m_iOK{};  // int
    std::int64_t    m_iUnitUID{};  // UidType
    std::int64_t    m_iItemUID{};  // UidType
    KGuildInfo      m_kCreatedGuildInfo{};
    KGuildSkillInfo m_kGuildSkillInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_kCreatedGuildInfo)>>::put(w, m_kCreatedGuildInfo);
        Codec<std::remove_cvref_t<decltype(m_kGuildSkillInfo)>>::put(w, m_kGuildSkillInfo);
    }

    static KCreateGuildACK read_from(Reader& r) {
        KCreateGuildACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_kCreatedGuildInfo = Codec<std::remove_cvref_t<decltype(v.m_kCreatedGuildInfo)>>::get(r);
        v.m_kGuildSkillInfo = Codec<std::remove_cvref_t<decltype(v.m_kGuildSkillInfo)>>::get(r);
        return v;
    }
};

struct KDBE_GET_GUILD_INFO_ACK {
    std::int32_t                  m_iOK{};  // int
    KGuildInfo                    m_kGuildInfo{};
    std::vector<KGuildMemberInfo> m_vecMemberList{};  // std::vector< KGuildMemberInfo >
    KGuildMemberInfo              m_kLoginGuildMember{};
    KGuildSkillInfo               m_kGuildSkillInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_kGuildInfo)>>::put(w, m_kGuildInfo);
        Codec<std::remove_cvref_t<decltype(m_vecMemberList)>>::put(w, m_vecMemberList);
        Codec<std::remove_cvref_t<decltype(m_kLoginGuildMember)>>::put(w, m_kLoginGuildMember);
        Codec<std::remove_cvref_t<decltype(m_kGuildSkillInfo)>>::put(w, m_kGuildSkillInfo);
    }

    static KDBE_GET_GUILD_INFO_ACK read_from(Reader& r) {
        KDBE_GET_GUILD_INFO_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_kGuildInfo = Codec<std::remove_cvref_t<decltype(v.m_kGuildInfo)>>::get(r);
        v.m_vecMemberList = Codec<std::remove_cvref_t<decltype(v.m_vecMemberList)>>::get(r);
        v.m_kLoginGuildMember = Codec<std::remove_cvref_t<decltype(v.m_kLoginGuildMember)>>::get(r);
        v.m_kGuildSkillInfo = Codec<std::remove_cvref_t<decltype(v.m_kGuildSkillInfo)>>::get(r);
        return v;
    }
};

struct KDBE_GET_GUILD_SKILL_IN_BOARD_ACK {
    std::int32_t    m_iGuildUID{};  // int
    std::int64_t    m_iUnitUID{};  // UidType
    KGuildSkillInfo m_kGuildSkillInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iGuildUID)>>::put(w, m_iGuildUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_kGuildSkillInfo)>>::put(w, m_kGuildSkillInfo);
    }

    static KDBE_GET_GUILD_SKILL_IN_BOARD_ACK read_from(Reader& r) {
        KDBE_GET_GUILD_SKILL_IN_BOARD_ACK v;
        v.m_iGuildUID = Codec<std::remove_cvref_t<decltype(v.m_iGuildUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_kGuildSkillInfo = Codec<std::remove_cvref_t<decltype(v.m_kGuildSkillInfo)>>::get(r);
        return v;
    }
};

struct KEGS_GET_GUILD_SKILL_IN_BOARD_ACK {
    std::int32_t    m_iOK{};  // int
    KGuildSkillInfo m_kGuildSkillInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_kGuildSkillInfo)>>::put(w, m_kGuildSkillInfo);
    }

    static KEGS_GET_GUILD_SKILL_IN_BOARD_ACK read_from(Reader& r) {
        KEGS_GET_GUILD_SKILL_IN_BOARD_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_kGuildSkillInfo = Codec<std::remove_cvref_t<decltype(v.m_kGuildSkillInfo)>>::get(r);
        return v;
    }
};

struct KEGS_GUILD_INFO_NOT {
    KGuildInfo                m_kGuildInfo{};
    std::u16string            m_wstrGuildMasterName{};  // std::wstring
    std::vector<std::int64_t> m_vecMemberUnitUID{};  // std::vector< UidType >
    KGuildSkillInfo           m_kGuildSkillInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kGuildInfo)>>::put(w, m_kGuildInfo);
        Codec<std::remove_cvref_t<decltype(m_wstrGuildMasterName)>>::put(w, m_wstrGuildMasterName);
        Codec<std::remove_cvref_t<decltype(m_vecMemberUnitUID)>>::put(w, m_vecMemberUnitUID);
        Codec<std::remove_cvref_t<decltype(m_kGuildSkillInfo)>>::put(w, m_kGuildSkillInfo);
    }

    static KEGS_GUILD_INFO_NOT read_from(Reader& r) {
        KEGS_GUILD_INFO_NOT v;
        v.m_kGuildInfo = Codec<std::remove_cvref_t<decltype(v.m_kGuildInfo)>>::get(r);
        v.m_wstrGuildMasterName = Codec<std::remove_cvref_t<decltype(v.m_wstrGuildMasterName)>>::get(r);
        v.m_vecMemberUnitUID = Codec<std::remove_cvref_t<decltype(v.m_vecMemberUnitUID)>>::get(r);
        v.m_kGuildSkillInfo = Codec<std::remove_cvref_t<decltype(v.m_kGuildSkillInfo)>>::get(r);
        return v;
    }
};

struct KELG_CREATE_GUILD_RESULT_REQ {
    KGuildInfo                    m_kGuildInfo{};
    std::vector<KGuildMemberInfo> m_vecMemberList{};  // std::vector< KGuildMemberInfo >
    KGuildMemberInfo              m_kLoginGuildMember{};
    KGuildSkillInfo               m_kGuildSkillInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kGuildInfo)>>::put(w, m_kGuildInfo);
        Codec<std::remove_cvref_t<decltype(m_vecMemberList)>>::put(w, m_vecMemberList);
        Codec<std::remove_cvref_t<decltype(m_kLoginGuildMember)>>::put(w, m_kLoginGuildMember);
        Codec<std::remove_cvref_t<decltype(m_kGuildSkillInfo)>>::put(w, m_kGuildSkillInfo);
    }

    static KELG_CREATE_GUILD_RESULT_REQ read_from(Reader& r) {
        KELG_CREATE_GUILD_RESULT_REQ v;
        v.m_kGuildInfo = Codec<std::remove_cvref_t<decltype(v.m_kGuildInfo)>>::get(r);
        v.m_vecMemberList = Codec<std::remove_cvref_t<decltype(v.m_vecMemberList)>>::get(r);
        v.m_kLoginGuildMember = Codec<std::remove_cvref_t<decltype(v.m_kLoginGuildMember)>>::get(r);
        v.m_kGuildSkillInfo = Codec<std::remove_cvref_t<decltype(v.m_kGuildSkillInfo)>>::get(r);
        return v;
    }
};

struct KELG_JOIN_GUILD_ACK {
    std::int32_t        m_iOK{};  // int
    KEGS_GUILD_INFO_NOT m_kGuildInfoNot{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_kGuildInfoNot)>>::put(w, m_kGuildInfoNot);
    }

    static KELG_JOIN_GUILD_ACK read_from(Reader& r) {
        KELG_JOIN_GUILD_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_kGuildInfoNot = Codec<std::remove_cvref_t<decltype(v.m_kGuildInfoNot)>>::get(r);
        return v;
    }
};

struct KHackingToolInfo {
    std::u16string m_wstrProcessName{};  // std::wstring
    std::u16string m_wstrWindowClassName{};  // std::wstring
    char           m_cFlag{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrProcessName)>>::put(w, m_wstrProcessName);
        Codec<std::remove_cvref_t<decltype(m_wstrWindowClassName)>>::put(w, m_wstrWindowClassName);
        Codec<std::remove_cvref_t<decltype(m_cFlag)>>::put(w, m_cFlag);
    }

    static KHackingToolInfo read_from(Reader& r) {
        KHackingToolInfo v;
        v.m_wstrProcessName = Codec<std::remove_cvref_t<decltype(v.m_wstrProcessName)>>::get(r);
        v.m_wstrWindowClassName = Codec<std::remove_cvref_t<decltype(v.m_wstrWindowClassName)>>::get(r);
        v.m_cFlag = Codec<std::remove_cvref_t<decltype(v.m_cFlag)>>::get(r);
        return v;
    }
};

struct KDBE_CHECK_HACKING_TOOL_LIST_ACK {
    std::int32_t                  m_iReleaseTick{};  // int
    std::vector<KHackingToolInfo> m_vecHackingToolList{};  // std::vector< KHackingToolInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iReleaseTick)>>::put(w, m_iReleaseTick);
        Codec<std::remove_cvref_t<decltype(m_vecHackingToolList)>>::put(w, m_vecHackingToolList);
    }

    static KDBE_CHECK_HACKING_TOOL_LIST_ACK read_from(Reader& r) {
        KDBE_CHECK_HACKING_TOOL_LIST_ACK v;
        v.m_iReleaseTick = Codec<std::remove_cvref_t<decltype(v.m_iReleaseTick)>>::get(r);
        v.m_vecHackingToolList = Codec<std::remove_cvref_t<decltype(v.m_vecHackingToolList)>>::get(r);
        return v;
    }
};

struct KEGS_HACKING_TOOL_LIST_NOT {
    std::int32_t                  m_iRandomKey{};  // int
    std::vector<KHackingToolInfo> m_vecHackingToolList{};  // std::vector< KHackingToolInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iRandomKey)>>::put(w, m_iRandomKey);
        Codec<std::remove_cvref_t<decltype(m_vecHackingToolList)>>::put(w, m_vecHackingToolList);
    }

    static KEGS_HACKING_TOOL_LIST_NOT read_from(Reader& r) {
        KEGS_HACKING_TOOL_LIST_NOT v;
        v.m_iRandomKey = Codec<std::remove_cvref_t<decltype(v.m_iRandomKey)>>::get(r);
        v.m_vecHackingToolList = Codec<std::remove_cvref_t<decltype(v.m_vecHackingToolList)>>::get(r);
        return v;
    }
};

struct KHenirHackUserData {
    std::int64_t m_iUserUID{};  // UidType
    std::int64_t m_iUnitUID{};  // UidType
    std::int32_t m_iPlayTime{};  // int
    std::int32_t m_iMemberNum{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iPlayTime)>>::put(w, m_iPlayTime);
        Codec<std::remove_cvref_t<decltype(m_iMemberNum)>>::put(w, m_iMemberNum);
    }

    static KHenirHackUserData read_from(Reader& r) {
        KHenirHackUserData v;
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iPlayTime = Codec<std::remove_cvref_t<decltype(v.m_iPlayTime)>>::get(r);
        v.m_iMemberNum = Codec<std::remove_cvref_t<decltype(v.m_iMemberNum)>>::get(r);
        return v;
    }
};

struct KELOG_HENIR_HACKING_LOG_NOT {
    std::vector<KHenirHackUserData> m_vecHackUser{};  // std::vector< KHenirHackUserData >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecHackUser)>>::put(w, m_vecHackUser);
    }

    static KELOG_HENIR_HACKING_LOG_NOT read_from(Reader& r) {
        KELOG_HENIR_HACKING_LOG_NOT v;
        v.m_vecHackUser = Codec<std::remove_cvref_t<decltype(v.m_vecHackUser)>>::get(r);
        return v;
    }
};

struct KHenirRankingInfo {
    std::int32_t   m_iRank{};  // int
    std::int32_t   m_iStageCount{};  // int
    std::uint32_t  m_ulPlayTime{};  // UINT
    std::int64_t   m_tRegDate{};  // __int64
    std::int64_t   m_iUnitUID{};  // UidType
    std::u16string m_wstrNickName{};  // std::wstring
    char           m_cUnitClass{};
    std::uint8_t   m_ucLevel{};  // UCHAR

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iRank)>>::put(w, m_iRank);
        Codec<std::remove_cvref_t<decltype(m_iStageCount)>>::put(w, m_iStageCount);
        Codec<std::remove_cvref_t<decltype(m_ulPlayTime)>>::put(w, m_ulPlayTime);
        Codec<std::remove_cvref_t<decltype(m_tRegDate)>>::put(w, m_tRegDate);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_cUnitClass)>>::put(w, m_cUnitClass);
        Codec<std::remove_cvref_t<decltype(m_ucLevel)>>::put(w, m_ucLevel);
    }

    static KHenirRankingInfo read_from(Reader& r) {
        KHenirRankingInfo v;
        v.m_iRank = Codec<std::remove_cvref_t<decltype(v.m_iRank)>>::get(r);
        v.m_iStageCount = Codec<std::remove_cvref_t<decltype(v.m_iStageCount)>>::get(r);
        v.m_ulPlayTime = Codec<std::remove_cvref_t<decltype(v.m_ulPlayTime)>>::get(r);
        v.m_tRegDate = Codec<std::remove_cvref_t<decltype(v.m_tRegDate)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_cUnitClass = Codec<std::remove_cvref_t<decltype(v.m_cUnitClass)>>::get(r);
        v.m_ucLevel = Codec<std::remove_cvref_t<decltype(v.m_ucLevel)>>::get(r);
        return v;
    }
};

struct KELG_ADMIN_NEW_RECORD_NOT {
    std::int32_t      m_iRankingType{};  // int
    KHenirRankingInfo m_kNewRecordRanking{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iRankingType)>>::put(w, m_iRankingType);
        Codec<std::remove_cvref_t<decltype(m_kNewRecordRanking)>>::put(w, m_kNewRecordRanking);
    }

    static KELG_ADMIN_NEW_RECORD_NOT read_from(Reader& r) {
        KELG_ADMIN_NEW_RECORD_NOT v;
        v.m_iRankingType = Codec<std::remove_cvref_t<decltype(v.m_iRankingType)>>::get(r);
        v.m_kNewRecordRanking = Codec<std::remove_cvref_t<decltype(v.m_kNewRecordRanking)>>::get(r);
        return v;
    }
};

struct KELG_ADMIN_NEW_RECORD_TEST_NOT {
    std::vector<KHenirRankingInfo> m_vecNewRecordTest{};  // std::vector< KHenirRankingInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecNewRecordTest)>>::put(w, m_vecNewRecordTest);
    }

    static KELG_ADMIN_NEW_RECORD_TEST_NOT read_from(Reader& r) {
        KELG_ADMIN_NEW_RECORD_TEST_NOT v;
        v.m_vecNewRecordTest = Codec<std::remove_cvref_t<decltype(v.m_vecNewRecordTest)>>::get(r);
        return v;
    }
};

struct KELG_GET_RANKING_FOR_GS_RANKING_MANAGER_ACK {
    std::map<std::int32_t, std::vector<KHenirRankingInfo>> m_mapHenirRanking{};  // std::map< int, std::vector< KHenirRankingInfo > >
    std::map<std::int32_t, std::uint32_t>                  m_mapLastRank{};  // std::map< int, u_int >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_mapHenirRanking)>>::put(w, m_mapHenirRanking);
        Codec<std::remove_cvref_t<decltype(m_mapLastRank)>>::put(w, m_mapLastRank);
    }

    static KELG_GET_RANKING_FOR_GS_RANKING_MANAGER_ACK read_from(Reader& r) {
        KELG_GET_RANKING_FOR_GS_RANKING_MANAGER_ACK v;
        v.m_mapHenirRanking = Codec<std::remove_cvref_t<decltype(v.m_mapHenirRanking)>>::get(r);
        v.m_mapLastRank = Codec<std::remove_cvref_t<decltype(v.m_mapLastRank)>>::get(r);
        return v;
    }
};

struct KELG_HENIR_RANKING_REFRESH_NOT {
    std::map<std::int32_t, std::vector<KHenirRankingInfo>> m_mapHenirRanking{};  // std::map< int, std::vector< KHenirRankingInfo > >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_mapHenirRanking)>>::put(w, m_mapHenirRanking);
    }

    static KELG_HENIR_RANKING_REFRESH_NOT read_from(Reader& r) {
        KELG_HENIR_RANKING_REFRESH_NOT v;
        v.m_mapHenirRanking = Codec<std::remove_cvref_t<decltype(v.m_mapHenirRanking)>>::get(r);
        return v;
    }
};

struct KELG_NEW_RECORD_HENIR_RANKING_NOT {
    KHenirRankingInfo m_kNewRecordRanking{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kNewRecordRanking)>>::put(w, m_kNewRecordRanking);
    }

    static KELG_NEW_RECORD_HENIR_RANKING_NOT read_from(Reader& r) {
        KELG_NEW_RECORD_HENIR_RANKING_NOT v;
        v.m_kNewRecordRanking = Codec<std::remove_cvref_t<decltype(v.m_kNewRecordRanking)>>::get(r);
        return v;
    }
};

struct KE_LOCAL_LOG_HENIR_RANKING_NOT {
    std::int32_t                   m_iRankingType{};  // int
    std::vector<KHenirRankingInfo> m_vecRankingInfo{};  // std::vector< KHenirRankingInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iRankingType)>>::put(w, m_iRankingType);
        Codec<std::remove_cvref_t<decltype(m_vecRankingInfo)>>::put(w, m_vecRankingInfo);
    }

    static KE_LOCAL_LOG_HENIR_RANKING_NOT read_from(Reader& r) {
        KE_LOCAL_LOG_HENIR_RANKING_NOT v;
        v.m_iRankingType = Codec<std::remove_cvref_t<decltype(v.m_iRankingType)>>::get(r);
        v.m_vecRankingInfo = Codec<std::remove_cvref_t<decltype(v.m_vecRankingInfo)>>::get(r);
        return v;
    }
};

struct KHeroPvpUserInfo {
    std::int64_t   m_iServerGroupID{};  // UidType
    std::u16string m_wstrNickName{};  // std::wstring
    bool           m_bOn{};
    bool           m_bConnect{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iServerGroupID)>>::put(w, m_iServerGroupID);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_bOn)>>::put(w, m_bOn);
        Codec<std::remove_cvref_t<decltype(m_bConnect)>>::put(w, m_bConnect);
    }

    static KHeroPvpUserInfo read_from(Reader& r) {
        KHeroPvpUserInfo v;
        v.m_iServerGroupID = Codec<std::remove_cvref_t<decltype(v.m_iServerGroupID)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_bOn = Codec<std::remove_cvref_t<decltype(v.m_bOn)>>::get(r);
        v.m_bConnect = Codec<std::remove_cvref_t<decltype(v.m_bConnect)>>::get(r);
        return v;
    }
};

struct KEGS_ADMIN_HERO_PVP_USER_CONNECTION_NOT {
    std::vector<std::pair<bool, KHeroPvpUserInfo>> m_vecUserList{};  // std::vector< PairHeroPvpUserInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecUserList)>>::put(w, m_vecUserList);
    }

    static KEGS_ADMIN_HERO_PVP_USER_CONNECTION_NOT read_from(Reader& r) {
        KEGS_ADMIN_HERO_PVP_USER_CONNECTION_NOT v;
        v.m_vecUserList = Codec<std::remove_cvref_t<decltype(v.m_vecUserList)>>::get(r);
        return v;
    }
};

struct KEGS_ADMIN_HERO_PVP_USER_LIST_ACK {
    std::vector<KHeroPvpUserInfo> m_vecUserList{};  // std::vector< KHeroPvpUserInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecUserList)>>::put(w, m_vecUserList);
    }

    static KEGS_ADMIN_HERO_PVP_USER_LIST_ACK read_from(Reader& r) {
        KEGS_ADMIN_HERO_PVP_USER_LIST_ACK v;
        v.m_vecUserList = Codec<std::remove_cvref_t<decltype(v.m_vecUserList)>>::get(r);
        return v;
    }
};

struct KItemAbuserLogInfo {
    std::int32_t m_iRewardState{};  // int
    std::int32_t m_iItemID{};  // int
    std::int32_t m_iQuantity{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iRewardState)>>::put(w, m_iRewardState);
        Codec<std::remove_cvref_t<decltype(m_iItemID)>>::put(w, m_iItemID);
        Codec<std::remove_cvref_t<decltype(m_iQuantity)>>::put(w, m_iQuantity);
    }

    static KItemAbuserLogInfo read_from(Reader& r) {
        KItemAbuserLogInfo v;
        v.m_iRewardState = Codec<std::remove_cvref_t<decltype(v.m_iRewardState)>>::get(r);
        v.m_iItemID = Codec<std::remove_cvref_t<decltype(v.m_iItemID)>>::get(r);
        v.m_iQuantity = Codec<std::remove_cvref_t<decltype(v.m_iQuantity)>>::get(r);
        return v;
    }
};

struct KELOG_ITEM_ABUSER_LOG_NOT {
    std::int64_t                    m_iUnitUID{};  // UidType
    std::vector<KItemAbuserLogInfo> m_vecItemAbuserLog{};  // std::vector< KItemAbuserLogInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_vecItemAbuserLog)>>::put(w, m_vecItemAbuserLog);
    }

    static KELOG_ITEM_ABUSER_LOG_NOT read_from(Reader& r) {
        KELOG_ITEM_ABUSER_LOG_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_vecItemAbuserLog = Codec<std::remove_cvref_t<decltype(v.m_vecItemAbuserLog)>>::get(r);
        return v;
    }
};

struct KItemAttributeEnchantInfo {
    char m_cAttribEnchant0{};
    char m_cAttribEnchant1{};
    char m_cAttribEnchant2{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cAttribEnchant0)>>::put(w, m_cAttribEnchant0);
        Codec<std::remove_cvref_t<decltype(m_cAttribEnchant1)>>::put(w, m_cAttribEnchant1);
        Codec<std::remove_cvref_t<decltype(m_cAttribEnchant2)>>::put(w, m_cAttribEnchant2);
    }

    static KItemAttributeEnchantInfo read_from(Reader& r) {
        KItemAttributeEnchantInfo v;
        v.m_cAttribEnchant0 = Codec<std::remove_cvref_t<decltype(v.m_cAttribEnchant0)>>::get(r);
        v.m_cAttribEnchant1 = Codec<std::remove_cvref_t<decltype(v.m_cAttribEnchant1)>>::get(r);
        v.m_cAttribEnchant2 = Codec<std::remove_cvref_t<decltype(v.m_cAttribEnchant2)>>::get(r);
        return v;
    }
};

struct KELOG_ATTRIB_ATTACH_ITEM_LOG_NOT {
    std::int64_t              m_iUnitUID{};  // UidType
    std::int32_t              m_iAttachItemID{};  // int
    std::int64_t              m_iDestItemUID{};  // UidType
    KItemAttributeEnchantInfo m_kBeforeAttribEnchantInfo{};
    KItemAttributeEnchantInfo m_kAfterAttribEnchantInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iAttachItemID)>>::put(w, m_iAttachItemID);
        Codec<std::remove_cvref_t<decltype(m_iDestItemUID)>>::put(w, m_iDestItemUID);
        Codec<std::remove_cvref_t<decltype(m_kBeforeAttribEnchantInfo)>>::put(w, m_kBeforeAttribEnchantInfo);
        Codec<std::remove_cvref_t<decltype(m_kAfterAttribEnchantInfo)>>::put(w, m_kAfterAttribEnchantInfo);
    }

    static KELOG_ATTRIB_ATTACH_ITEM_LOG_NOT read_from(Reader& r) {
        KELOG_ATTRIB_ATTACH_ITEM_LOG_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iAttachItemID = Codec<std::remove_cvref_t<decltype(v.m_iAttachItemID)>>::get(r);
        v.m_iDestItemUID = Codec<std::remove_cvref_t<decltype(v.m_iDestItemUID)>>::get(r);
        v.m_kBeforeAttribEnchantInfo = Codec<std::remove_cvref_t<decltype(v.m_kBeforeAttribEnchantInfo)>>::get(r);
        v.m_kAfterAttribEnchantInfo = Codec<std::remove_cvref_t<decltype(v.m_kAfterAttribEnchantInfo)>>::get(r);
        return v;
    }
};

struct KInventoryItemSimpleInfo {
    std::int64_t              m_iItemUID{};  // UidType
    std::int32_t              m_iItemID{};  // int
    char                      m_cSlotCategory{};
    std::int16_t              m_sSlotID{};  // short
    char                      m_EnchantLevel{};
    KItemAttributeEnchantInfo m_kAttribEnchantInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_iItemID)>>::put(w, m_iItemID);
        Codec<std::remove_cvref_t<decltype(m_cSlotCategory)>>::put(w, m_cSlotCategory);
        Codec<std::remove_cvref_t<decltype(m_sSlotID)>>::put(w, m_sSlotID);
        Codec<std::remove_cvref_t<decltype(m_EnchantLevel)>>::put(w, m_EnchantLevel);
        Codec<std::remove_cvref_t<decltype(m_kAttribEnchantInfo)>>::put(w, m_kAttribEnchantInfo);
    }

    static KInventoryItemSimpleInfo read_from(Reader& r) {
        KInventoryItemSimpleInfo v;
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_iItemID = Codec<std::remove_cvref_t<decltype(v.m_iItemID)>>::get(r);
        v.m_cSlotCategory = Codec<std::remove_cvref_t<decltype(v.m_cSlotCategory)>>::get(r);
        v.m_sSlotID = Codec<std::remove_cvref_t<decltype(v.m_sSlotID)>>::get(r);
        v.m_EnchantLevel = Codec<std::remove_cvref_t<decltype(v.m_EnchantLevel)>>::get(r);
        v.m_kAttribEnchantInfo = Codec<std::remove_cvref_t<decltype(v.m_kAttribEnchantInfo)>>::get(r);
        return v;
    }
};

struct KItemInfo {
    std::int32_t              m_iItemID{};  // int
    char                      m_cUsageType{};
    std::int32_t              m_iQuantity{};  // int
    std::int16_t              m_sEndurance{};  // short
    std::uint8_t              m_ucSealData{};  // unsigned char
    char                      m_cEnchantLevel{};
    KItemAttributeEnchantInfo m_kAttribEnchantInfo{};
    std::vector<std::int32_t> m_vecItemSocket{};  // std::vector< int >
    std::vector<std::int32_t> m_vecRandomSocket{};  // std::vector< int >
    char                      m_cItemState{};
    std::int16_t              m_sPeriod{};  // short
    std::u16string            m_wstrExpirationDate{};  // std::wstring
    std::int64_t              m_iGoldTicketKeyUID{};  // UidType
    std::uint8_t              m_byteExpandedSocketNum{};  // byte

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iItemID)>>::put(w, m_iItemID);
        Codec<std::remove_cvref_t<decltype(m_cUsageType)>>::put(w, m_cUsageType);
        Codec<std::remove_cvref_t<decltype(m_iQuantity)>>::put(w, m_iQuantity);
        Codec<std::remove_cvref_t<decltype(m_sEndurance)>>::put(w, m_sEndurance);
        Codec<std::remove_cvref_t<decltype(m_ucSealData)>>::put(w, m_ucSealData);
        Codec<std::remove_cvref_t<decltype(m_cEnchantLevel)>>::put(w, m_cEnchantLevel);
        Codec<std::remove_cvref_t<decltype(m_kAttribEnchantInfo)>>::put(w, m_kAttribEnchantInfo);
        Codec<std::remove_cvref_t<decltype(m_vecItemSocket)>>::put(w, m_vecItemSocket);
        Codec<std::remove_cvref_t<decltype(m_vecRandomSocket)>>::put(w, m_vecRandomSocket);
        Codec<std::remove_cvref_t<decltype(m_cItemState)>>::put(w, m_cItemState);
        Codec<std::remove_cvref_t<decltype(m_sPeriod)>>::put(w, m_sPeriod);
        Codec<std::remove_cvref_t<decltype(m_wstrExpirationDate)>>::put(w, m_wstrExpirationDate);
        Codec<std::remove_cvref_t<decltype(m_iGoldTicketKeyUID)>>::put(w, m_iGoldTicketKeyUID);
        Codec<std::remove_cvref_t<decltype(m_byteExpandedSocketNum)>>::put(w, m_byteExpandedSocketNum);
    }

    static KItemInfo read_from(Reader& r) {
        KItemInfo v;
        v.m_iItemID = Codec<std::remove_cvref_t<decltype(v.m_iItemID)>>::get(r);
        v.m_cUsageType = Codec<std::remove_cvref_t<decltype(v.m_cUsageType)>>::get(r);
        v.m_iQuantity = Codec<std::remove_cvref_t<decltype(v.m_iQuantity)>>::get(r);
        v.m_sEndurance = Codec<std::remove_cvref_t<decltype(v.m_sEndurance)>>::get(r);
        v.m_ucSealData = Codec<std::remove_cvref_t<decltype(v.m_ucSealData)>>::get(r);
        v.m_cEnchantLevel = Codec<std::remove_cvref_t<decltype(v.m_cEnchantLevel)>>::get(r);
        v.m_kAttribEnchantInfo = Codec<std::remove_cvref_t<decltype(v.m_kAttribEnchantInfo)>>::get(r);
        v.m_vecItemSocket = Codec<std::remove_cvref_t<decltype(v.m_vecItemSocket)>>::get(r);
        v.m_vecRandomSocket = Codec<std::remove_cvref_t<decltype(v.m_vecRandomSocket)>>::get(r);
        v.m_cItemState = Codec<std::remove_cvref_t<decltype(v.m_cItemState)>>::get(r);
        v.m_sPeriod = Codec<std::remove_cvref_t<decltype(v.m_sPeriod)>>::get(r);
        v.m_wstrExpirationDate = Codec<std::remove_cvref_t<decltype(v.m_wstrExpirationDate)>>::get(r);
        v.m_iGoldTicketKeyUID = Codec<std::remove_cvref_t<decltype(v.m_iGoldTicketKeyUID)>>::get(r);
        v.m_byteExpandedSocketNum = Codec<std::remove_cvref_t<decltype(v.m_byteExpandedSocketNum)>>::get(r);
        return v;
    }
};

struct KEGS_GET_ITEM_REQ {
    std::int32_t m_iDropItemUID{};  // int
    KItemInfo    m_kItemInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iDropItemUID)>>::put(w, m_iDropItemUID);
        Codec<std::remove_cvref_t<decltype(m_kItemInfo)>>::put(w, m_kItemInfo);
    }

    static KEGS_GET_ITEM_REQ read_from(Reader& r) {
        KEGS_GET_ITEM_REQ v;
        v.m_iDropItemUID = Codec<std::remove_cvref_t<decltype(v.m_iDropItemUID)>>::get(r);
        v.m_kItemInfo = Codec<std::remove_cvref_t<decltype(v.m_kItemInfo)>>::get(r);
        return v;
    }
};

struct KEGS_UPDATE_TRADE_ITEM_NOT {
    std::int64_t                 m_iUnitUID{};  // UidType
    std::int64_t                 m_iItemUID{};  // UidType
    bool                         m_bDelete{};
    KItemInfo                    m_kItemInfo{};
    std::map<std::int64_t, bool> m_mapOfferState{};  // std::map< UidType, bool >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_bDelete)>>::put(w, m_bDelete);
        Codec<std::remove_cvref_t<decltype(m_kItemInfo)>>::put(w, m_kItemInfo);
        Codec<std::remove_cvref_t<decltype(m_mapOfferState)>>::put(w, m_mapOfferState);
    }

    static KEGS_UPDATE_TRADE_ITEM_NOT read_from(Reader& r) {
        KEGS_UPDATE_TRADE_ITEM_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_bDelete = Codec<std::remove_cvref_t<decltype(v.m_bDelete)>>::get(r);
        v.m_kItemInfo = Codec<std::remove_cvref_t<decltype(v.m_kItemInfo)>>::get(r);
        v.m_mapOfferState = Codec<std::remove_cvref_t<decltype(v.m_mapOfferState)>>::get(r);
        return v;
    }
};

struct KERM_GET_ITEM_ACK {
    std::int32_t m_iOK{};  // int
    KItemInfo    m_kItemInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_kItemInfo)>>::put(w, m_kItemInfo);
    }

    static KERM_GET_ITEM_ACK read_from(Reader& r) {
        KERM_GET_ITEM_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_kItemInfo = Codec<std::remove_cvref_t<decltype(v.m_kItemInfo)>>::get(r);
        return v;
    }
};

struct KERM_UPDATE_DUNGEON_UNIT_INFO_NOT {
    std::int32_t                         m_iEXP{};  // int
    std::int32_t                         m_iED{};  // int
    std::int32_t                         m_iPvPGameType{};  // int
    std::int32_t                         m_iDungeonID{};  // int
    std::int32_t                         m_iUserCount{};  // int
    bool                                 m_bDungeonClear{};
    std::map<std::int32_t, std::int32_t> m_mapGetItem{};  // std::map< int, int >
    std::map<std::int32_t, KItemInfo>    m_mapResultItem{};  // std::map< int, KItemInfo >
    std::int32_t                         m_iDropItemUID{};  // int
    bool                                 m_bOutRoom{};
    std::int32_t                         m_iPlayTime{};  // int
    std::int32_t                         m_iMaxScore{};  // int
    char                                 m_cMaxTotalRank{};
    bool                                 m_bIsTutorialSuccess{};
    bool                                 m_bGetWorldMissionReward{};
    std::vector<KHenirRankingInfo>       m_kHenirRankingInfo{};  // std::vector< KHenirRankingInfo >
    bool                                 m_bStartedByAutoParty{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iEXP)>>::put(w, m_iEXP);
        Codec<std::remove_cvref_t<decltype(m_iED)>>::put(w, m_iED);
        Codec<std::remove_cvref_t<decltype(m_iPvPGameType)>>::put(w, m_iPvPGameType);
        Codec<std::remove_cvref_t<decltype(m_iDungeonID)>>::put(w, m_iDungeonID);
        Codec<std::remove_cvref_t<decltype(m_iUserCount)>>::put(w, m_iUserCount);
        Codec<std::remove_cvref_t<decltype(m_bDungeonClear)>>::put(w, m_bDungeonClear);
        Codec<std::remove_cvref_t<decltype(m_mapGetItem)>>::put(w, m_mapGetItem);
        Codec<std::remove_cvref_t<decltype(m_mapResultItem)>>::put(w, m_mapResultItem);
        Codec<std::remove_cvref_t<decltype(m_iDropItemUID)>>::put(w, m_iDropItemUID);
        Codec<std::remove_cvref_t<decltype(m_bOutRoom)>>::put(w, m_bOutRoom);
        Codec<std::remove_cvref_t<decltype(m_iPlayTime)>>::put(w, m_iPlayTime);
        Codec<std::remove_cvref_t<decltype(m_iMaxScore)>>::put(w, m_iMaxScore);
        Codec<std::remove_cvref_t<decltype(m_cMaxTotalRank)>>::put(w, m_cMaxTotalRank);
        Codec<std::remove_cvref_t<decltype(m_bIsTutorialSuccess)>>::put(w, m_bIsTutorialSuccess);
        Codec<std::remove_cvref_t<decltype(m_bGetWorldMissionReward)>>::put(w, m_bGetWorldMissionReward);
        Codec<std::remove_cvref_t<decltype(m_kHenirRankingInfo)>>::put(w, m_kHenirRankingInfo);
        Codec<std::remove_cvref_t<decltype(m_bStartedByAutoParty)>>::put(w, m_bStartedByAutoParty);
    }

    static KERM_UPDATE_DUNGEON_UNIT_INFO_NOT read_from(Reader& r) {
        KERM_UPDATE_DUNGEON_UNIT_INFO_NOT v;
        v.m_iEXP = Codec<std::remove_cvref_t<decltype(v.m_iEXP)>>::get(r);
        v.m_iED = Codec<std::remove_cvref_t<decltype(v.m_iED)>>::get(r);
        v.m_iPvPGameType = Codec<std::remove_cvref_t<decltype(v.m_iPvPGameType)>>::get(r);
        v.m_iDungeonID = Codec<std::remove_cvref_t<decltype(v.m_iDungeonID)>>::get(r);
        v.m_iUserCount = Codec<std::remove_cvref_t<decltype(v.m_iUserCount)>>::get(r);
        v.m_bDungeonClear = Codec<std::remove_cvref_t<decltype(v.m_bDungeonClear)>>::get(r);
        v.m_mapGetItem = Codec<std::remove_cvref_t<decltype(v.m_mapGetItem)>>::get(r);
        v.m_mapResultItem = Codec<std::remove_cvref_t<decltype(v.m_mapResultItem)>>::get(r);
        v.m_iDropItemUID = Codec<std::remove_cvref_t<decltype(v.m_iDropItemUID)>>::get(r);
        v.m_bOutRoom = Codec<std::remove_cvref_t<decltype(v.m_bOutRoom)>>::get(r);
        v.m_iPlayTime = Codec<std::remove_cvref_t<decltype(v.m_iPlayTime)>>::get(r);
        v.m_iMaxScore = Codec<std::remove_cvref_t<decltype(v.m_iMaxScore)>>::get(r);
        v.m_cMaxTotalRank = Codec<std::remove_cvref_t<decltype(v.m_cMaxTotalRank)>>::get(r);
        v.m_bIsTutorialSuccess = Codec<std::remove_cvref_t<decltype(v.m_bIsTutorialSuccess)>>::get(r);
        v.m_bGetWorldMissionReward = Codec<std::remove_cvref_t<decltype(v.m_bGetWorldMissionReward)>>::get(r);
        v.m_kHenirRankingInfo = Codec<std::remove_cvref_t<decltype(v.m_kHenirRankingInfo)>>::get(r);
        v.m_bStartedByAutoParty = Codec<std::remove_cvref_t<decltype(v.m_bStartedByAutoParty)>>::get(r);
        return v;
    }
};

struct KERM_UPDATE_PVP_UNIT_INFO_NOT {
    std::int32_t                      m_iEXP{};  // int
    std::int32_t                      m_iCurrentRating{};  // int
    std::int32_t                      m_iCurrentMaxRating{};  // int
    std::int32_t                      m_iCurrentRPoint{};  // int
    std::int32_t                      m_iCurrentAPoint{};  // int
    bool                              m_bIsOfficialMatch{};
    std::int32_t                      m_iRating{};  // int
    std::int32_t                      m_iRPoint{};  // int
    std::int32_t                      m_iAPoint{};  // int
    char                              m_cRank{};
    char                              m_cRankForServer{};
    float                             m_fKFactorIncrement{};
    std::int32_t                      m_iWin{};  // int
    std::int32_t                      m_iLose{};  // int
    bool                              m_bOutRoom{};
    std::int32_t                      m_iMemberCount{};  // int
    std::int32_t                      m_iPlayTime{};  // int
    std::map<std::int32_t, KItemInfo> m_mapItem{};  // std::map< int, KItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iEXP)>>::put(w, m_iEXP);
        Codec<std::remove_cvref_t<decltype(m_iCurrentRating)>>::put(w, m_iCurrentRating);
        Codec<std::remove_cvref_t<decltype(m_iCurrentMaxRating)>>::put(w, m_iCurrentMaxRating);
        Codec<std::remove_cvref_t<decltype(m_iCurrentRPoint)>>::put(w, m_iCurrentRPoint);
        Codec<std::remove_cvref_t<decltype(m_iCurrentAPoint)>>::put(w, m_iCurrentAPoint);
        Codec<std::remove_cvref_t<decltype(m_bIsOfficialMatch)>>::put(w, m_bIsOfficialMatch);
        Codec<std::remove_cvref_t<decltype(m_iRating)>>::put(w, m_iRating);
        Codec<std::remove_cvref_t<decltype(m_iRPoint)>>::put(w, m_iRPoint);
        Codec<std::remove_cvref_t<decltype(m_iAPoint)>>::put(w, m_iAPoint);
        Codec<std::remove_cvref_t<decltype(m_cRank)>>::put(w, m_cRank);
        Codec<std::remove_cvref_t<decltype(m_cRankForServer)>>::put(w, m_cRankForServer);
        Codec<std::remove_cvref_t<decltype(m_fKFactorIncrement)>>::put(w, m_fKFactorIncrement);
        Codec<std::remove_cvref_t<decltype(m_iWin)>>::put(w, m_iWin);
        Codec<std::remove_cvref_t<decltype(m_iLose)>>::put(w, m_iLose);
        Codec<std::remove_cvref_t<decltype(m_bOutRoom)>>::put(w, m_bOutRoom);
        Codec<std::remove_cvref_t<decltype(m_iMemberCount)>>::put(w, m_iMemberCount);
        Codec<std::remove_cvref_t<decltype(m_iPlayTime)>>::put(w, m_iPlayTime);
        Codec<std::remove_cvref_t<decltype(m_mapItem)>>::put(w, m_mapItem);
    }

    static KERM_UPDATE_PVP_UNIT_INFO_NOT read_from(Reader& r) {
        KERM_UPDATE_PVP_UNIT_INFO_NOT v;
        v.m_iEXP = Codec<std::remove_cvref_t<decltype(v.m_iEXP)>>::get(r);
        v.m_iCurrentRating = Codec<std::remove_cvref_t<decltype(v.m_iCurrentRating)>>::get(r);
        v.m_iCurrentMaxRating = Codec<std::remove_cvref_t<decltype(v.m_iCurrentMaxRating)>>::get(r);
        v.m_iCurrentRPoint = Codec<std::remove_cvref_t<decltype(v.m_iCurrentRPoint)>>::get(r);
        v.m_iCurrentAPoint = Codec<std::remove_cvref_t<decltype(v.m_iCurrentAPoint)>>::get(r);
        v.m_bIsOfficialMatch = Codec<std::remove_cvref_t<decltype(v.m_bIsOfficialMatch)>>::get(r);
        v.m_iRating = Codec<std::remove_cvref_t<decltype(v.m_iRating)>>::get(r);
        v.m_iRPoint = Codec<std::remove_cvref_t<decltype(v.m_iRPoint)>>::get(r);
        v.m_iAPoint = Codec<std::remove_cvref_t<decltype(v.m_iAPoint)>>::get(r);
        v.m_cRank = Codec<std::remove_cvref_t<decltype(v.m_cRank)>>::get(r);
        v.m_cRankForServer = Codec<std::remove_cvref_t<decltype(v.m_cRankForServer)>>::get(r);
        v.m_fKFactorIncrement = Codec<std::remove_cvref_t<decltype(v.m_fKFactorIncrement)>>::get(r);
        v.m_iWin = Codec<std::remove_cvref_t<decltype(v.m_iWin)>>::get(r);
        v.m_iLose = Codec<std::remove_cvref_t<decltype(v.m_iLose)>>::get(r);
        v.m_bOutRoom = Codec<std::remove_cvref_t<decltype(v.m_bOutRoom)>>::get(r);
        v.m_iMemberCount = Codec<std::remove_cvref_t<decltype(v.m_iMemberCount)>>::get(r);
        v.m_iPlayTime = Codec<std::remove_cvref_t<decltype(v.m_iPlayTime)>>::get(r);
        v.m_mapItem = Codec<std::remove_cvref_t<decltype(v.m_mapItem)>>::get(r);
        return v;
    }
};

struct KInventoryItemInfo {
    std::int64_t m_iItemUID{};  // UidType
    char         m_cSlotCategory{};
    std::int16_t m_sSlotID{};  // short
    KItemInfo    m_kItemInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_cSlotCategory)>>::put(w, m_cSlotCategory);
        Codec<std::remove_cvref_t<decltype(m_sSlotID)>>::put(w, m_sSlotID);
        Codec<std::remove_cvref_t<decltype(m_kItemInfo)>>::put(w, m_kItemInfo);
    }

    static KInventoryItemInfo read_from(Reader& r) {
        KInventoryItemInfo v;
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_cSlotCategory = Codec<std::remove_cvref_t<decltype(v.m_cSlotCategory)>>::get(r);
        v.m_sSlotID = Codec<std::remove_cvref_t<decltype(v.m_sSlotID)>>::get(r);
        v.m_kItemInfo = Codec<std::remove_cvref_t<decltype(v.m_kItemInfo)>>::get(r);
        return v;
    }
};

struct KDBE_BUY_PERSONAL_SHOP_ITEM_COMPLETE_REQ {
    std::int64_t       m_iSellUnitUID{};  // UidType
    std::int64_t       m_iSellUnitGSUID{};  // UidType
    std::int32_t       m_iSellUnitEDIN{};  // int
    std::int32_t       m_iSellCommissionED{};  // int
    std::int64_t       m_iBuyUnitUID{};  // UidType
    std::int64_t       m_iBuyUnitGSUID{};  // UidType
    std::int32_t       m_iBuyUnitEDOUT{};  // int
    std::int32_t       m_iBuyCommissionED{};  // int
    bool               m_bBuyUnitIsSquare{};
    KInventoryItemInfo m_kSellItemInfo{};
    KInventoryItemInfo m_kBuyItemInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iSellUnitUID)>>::put(w, m_iSellUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iSellUnitGSUID)>>::put(w, m_iSellUnitGSUID);
        Codec<std::remove_cvref_t<decltype(m_iSellUnitEDIN)>>::put(w, m_iSellUnitEDIN);
        Codec<std::remove_cvref_t<decltype(m_iSellCommissionED)>>::put(w, m_iSellCommissionED);
        Codec<std::remove_cvref_t<decltype(m_iBuyUnitUID)>>::put(w, m_iBuyUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iBuyUnitGSUID)>>::put(w, m_iBuyUnitGSUID);
        Codec<std::remove_cvref_t<decltype(m_iBuyUnitEDOUT)>>::put(w, m_iBuyUnitEDOUT);
        Codec<std::remove_cvref_t<decltype(m_iBuyCommissionED)>>::put(w, m_iBuyCommissionED);
        Codec<std::remove_cvref_t<decltype(m_bBuyUnitIsSquare)>>::put(w, m_bBuyUnitIsSquare);
        Codec<std::remove_cvref_t<decltype(m_kSellItemInfo)>>::put(w, m_kSellItemInfo);
        Codec<std::remove_cvref_t<decltype(m_kBuyItemInfo)>>::put(w, m_kBuyItemInfo);
    }

    static KDBE_BUY_PERSONAL_SHOP_ITEM_COMPLETE_REQ read_from(Reader& r) {
        KDBE_BUY_PERSONAL_SHOP_ITEM_COMPLETE_REQ v;
        v.m_iSellUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iSellUnitUID)>>::get(r);
        v.m_iSellUnitGSUID = Codec<std::remove_cvref_t<decltype(v.m_iSellUnitGSUID)>>::get(r);
        v.m_iSellUnitEDIN = Codec<std::remove_cvref_t<decltype(v.m_iSellUnitEDIN)>>::get(r);
        v.m_iSellCommissionED = Codec<std::remove_cvref_t<decltype(v.m_iSellCommissionED)>>::get(r);
        v.m_iBuyUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iBuyUnitUID)>>::get(r);
        v.m_iBuyUnitGSUID = Codec<std::remove_cvref_t<decltype(v.m_iBuyUnitGSUID)>>::get(r);
        v.m_iBuyUnitEDOUT = Codec<std::remove_cvref_t<decltype(v.m_iBuyUnitEDOUT)>>::get(r);
        v.m_iBuyCommissionED = Codec<std::remove_cvref_t<decltype(v.m_iBuyCommissionED)>>::get(r);
        v.m_bBuyUnitIsSquare = Codec<std::remove_cvref_t<decltype(v.m_bBuyUnitIsSquare)>>::get(r);
        v.m_kSellItemInfo = Codec<std::remove_cvref_t<decltype(v.m_kSellItemInfo)>>::get(r);
        v.m_kBuyItemInfo = Codec<std::remove_cvref_t<decltype(v.m_kBuyItemInfo)>>::get(r);
        return v;
    }
};

struct KDBE_BUY_PERSONAL_SHOP_ITEM_COMPLETE_ACK {
    std::int32_t                             m_iOK{};  // int
    KDBE_BUY_PERSONAL_SHOP_ITEM_COMPLETE_REQ m_kResult{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_kResult)>>::put(w, m_kResult);
    }

    static KDBE_BUY_PERSONAL_SHOP_ITEM_COMPLETE_ACK read_from(Reader& r) {
        KDBE_BUY_PERSONAL_SHOP_ITEM_COMPLETE_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_kResult = Codec<std::remove_cvref_t<decltype(v.m_kResult)>>::get(r);
        return v;
    }
};

struct KDBE_COUPLE_MAKING_SUCCESS_REQ {
    std::int64_t                    m_iRequestUnitUID{};  // UidType
    std::u16string                  m_wstrRequestUnitName{};  // std::wstring
    std::int64_t                    m_iAcceptUserUID{};  // UidType
    std::uint8_t                    m_ucAcceptUnitLevel{};  // u_char
    char                            m_cAcceptUnitClass{};
    std::int64_t                    m_iAcceptUnitUID{};  // UidType
    std::u16string                  m_wstrAcceptUnitName{};  // std::wstring
    std::vector<KInventoryItemInfo> m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >
    std::vector<KDeletedItemInfo>   m_vecDeleted{};  // std::vector< KDeletedItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iRequestUnitUID)>>::put(w, m_iRequestUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrRequestUnitName)>>::put(w, m_wstrRequestUnitName);
        Codec<std::remove_cvref_t<decltype(m_iAcceptUserUID)>>::put(w, m_iAcceptUserUID);
        Codec<std::remove_cvref_t<decltype(m_ucAcceptUnitLevel)>>::put(w, m_ucAcceptUnitLevel);
        Codec<std::remove_cvref_t<decltype(m_cAcceptUnitClass)>>::put(w, m_cAcceptUnitClass);
        Codec<std::remove_cvref_t<decltype(m_iAcceptUnitUID)>>::put(w, m_iAcceptUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrAcceptUnitName)>>::put(w, m_wstrAcceptUnitName);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
        Codec<std::remove_cvref_t<decltype(m_vecDeleted)>>::put(w, m_vecDeleted);
    }

    static KDBE_COUPLE_MAKING_SUCCESS_REQ read_from(Reader& r) {
        KDBE_COUPLE_MAKING_SUCCESS_REQ v;
        v.m_iRequestUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iRequestUnitUID)>>::get(r);
        v.m_wstrRequestUnitName = Codec<std::remove_cvref_t<decltype(v.m_wstrRequestUnitName)>>::get(r);
        v.m_iAcceptUserUID = Codec<std::remove_cvref_t<decltype(v.m_iAcceptUserUID)>>::get(r);
        v.m_ucAcceptUnitLevel = Codec<std::remove_cvref_t<decltype(v.m_ucAcceptUnitLevel)>>::get(r);
        v.m_cAcceptUnitClass = Codec<std::remove_cvref_t<decltype(v.m_cAcceptUnitClass)>>::get(r);
        v.m_iAcceptUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iAcceptUnitUID)>>::get(r);
        v.m_wstrAcceptUnitName = Codec<std::remove_cvref_t<decltype(v.m_wstrAcceptUnitName)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        v.m_vecDeleted = Codec<std::remove_cvref_t<decltype(v.m_vecDeleted)>>::get(r);
        return v;
    }
};

struct KDBE_GET_ITEM_FROM_LETTER_ACK {
    std::int32_t                         m_iOK{};  // int
    std::int64_t                         m_iPostNo{};  // UidType
    char                                 m_cLetterType{};
    std::int32_t                         m_iWeddingUID{};  // int
    std::int64_t                         m_iWeddingItemUID{};  // UidType
    std::map<std::int32_t, std::int32_t> m_mapInsertedItem{};  // std::map< int, int >
    std::vector<KInventoryItemInfo>      m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >
    std::map<std::int64_t, KItemInfo>    m_mapItemInfo{};  // std::map< UidType, KItemInfo >
    std::int32_t                         m_iED{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iPostNo)>>::put(w, m_iPostNo);
        Codec<std::remove_cvref_t<decltype(m_cLetterType)>>::put(w, m_cLetterType);
        Codec<std::remove_cvref_t<decltype(m_iWeddingUID)>>::put(w, m_iWeddingUID);
        Codec<std::remove_cvref_t<decltype(m_iWeddingItemUID)>>::put(w, m_iWeddingItemUID);
        Codec<std::remove_cvref_t<decltype(m_mapInsertedItem)>>::put(w, m_mapInsertedItem);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
        Codec<std::remove_cvref_t<decltype(m_mapItemInfo)>>::put(w, m_mapItemInfo);
        Codec<std::remove_cvref_t<decltype(m_iED)>>::put(w, m_iED);
    }

    static KDBE_GET_ITEM_FROM_LETTER_ACK read_from(Reader& r) {
        KDBE_GET_ITEM_FROM_LETTER_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iPostNo = Codec<std::remove_cvref_t<decltype(v.m_iPostNo)>>::get(r);
        v.m_cLetterType = Codec<std::remove_cvref_t<decltype(v.m_cLetterType)>>::get(r);
        v.m_iWeddingUID = Codec<std::remove_cvref_t<decltype(v.m_iWeddingUID)>>::get(r);
        v.m_iWeddingItemUID = Codec<std::remove_cvref_t<decltype(v.m_iWeddingItemUID)>>::get(r);
        v.m_mapInsertedItem = Codec<std::remove_cvref_t<decltype(v.m_mapInsertedItem)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        v.m_mapItemInfo = Codec<std::remove_cvref_t<decltype(v.m_mapItemInfo)>>::get(r);
        v.m_iED = Codec<std::remove_cvref_t<decltype(v.m_iED)>>::get(r);
        return v;
    }
};

struct KDBE_GET_ITEM_FROM_LETTER_REQ {
    std::int64_t                         m_iUnitUID{};  // UidType
    std::int64_t                         m_iPostNo{};  // UidType
    char                                 m_cLetterType{};
    std::map<std::int32_t, std::int32_t> m_mapInsertedItem{};  // std::map< int, int >
    std::vector<KInventoryItemInfo>      m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >
    std::vector<KItemInfo>               m_vecItemInfo{};  // std::vector< KItemInfo >
    std::int32_t                         m_iED{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iPostNo)>>::put(w, m_iPostNo);
        Codec<std::remove_cvref_t<decltype(m_cLetterType)>>::put(w, m_cLetterType);
        Codec<std::remove_cvref_t<decltype(m_mapInsertedItem)>>::put(w, m_mapInsertedItem);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
        Codec<std::remove_cvref_t<decltype(m_vecItemInfo)>>::put(w, m_vecItemInfo);
        Codec<std::remove_cvref_t<decltype(m_iED)>>::put(w, m_iED);
    }

    static KDBE_GET_ITEM_FROM_LETTER_REQ read_from(Reader& r) {
        KDBE_GET_ITEM_FROM_LETTER_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iPostNo = Codec<std::remove_cvref_t<decltype(v.m_iPostNo)>>::get(r);
        v.m_cLetterType = Codec<std::remove_cvref_t<decltype(v.m_cLetterType)>>::get(r);
        v.m_mapInsertedItem = Codec<std::remove_cvref_t<decltype(v.m_mapInsertedItem)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        v.m_vecItemInfo = Codec<std::remove_cvref_t<decltype(v.m_vecItemInfo)>>::get(r);
        v.m_iED = Codec<std::remove_cvref_t<decltype(v.m_iED)>>::get(r);
        return v;
    }
};

struct KDBE_REG_SKILL_NOTE_MEMO_REQ {
    std::int64_t                    m_iUnitUID{};  // UidType
    char                            m_cPageNum{};
    std::int32_t                    m_iMemoID{};  // int
    std::vector<KInventoryItemInfo> m_vecInventorySlotInfo{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_cPageNum)>>::put(w, m_cPageNum);
        Codec<std::remove_cvref_t<decltype(m_iMemoID)>>::put(w, m_iMemoID);
        Codec<std::remove_cvref_t<decltype(m_vecInventorySlotInfo)>>::put(w, m_vecInventorySlotInfo);
    }

    static KDBE_REG_SKILL_NOTE_MEMO_REQ read_from(Reader& r) {
        KDBE_REG_SKILL_NOTE_MEMO_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_cPageNum = Codec<std::remove_cvref_t<decltype(v.m_cPageNum)>>::get(r);
        v.m_iMemoID = Codec<std::remove_cvref_t<decltype(v.m_iMemoID)>>::get(r);
        v.m_vecInventorySlotInfo = Codec<std::remove_cvref_t<decltype(v.m_vecInventorySlotInfo)>>::get(r);
        return v;
    }
};

struct KEGS_2013_EVENT_MISSION_COMPLETE_ACK {
    std::int32_t                    m_iOK{};  // int
    std::int32_t                    m_iRewardedLevel{};  // int
    std::vector<KInventoryItemInfo> m_vecInventorySlotInfo{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iRewardedLevel)>>::put(w, m_iRewardedLevel);
        Codec<std::remove_cvref_t<decltype(m_vecInventorySlotInfo)>>::put(w, m_vecInventorySlotInfo);
    }

    static KEGS_2013_EVENT_MISSION_COMPLETE_ACK read_from(Reader& r) {
        KEGS_2013_EVENT_MISSION_COMPLETE_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iRewardedLevel = Codec<std::remove_cvref_t<decltype(v.m_iRewardedLevel)>>::get(r);
        v.m_vecInventorySlotInfo = Codec<std::remove_cvref_t<decltype(v.m_vecInventorySlotInfo)>>::get(r);
        return v;
    }
};

struct KEGS_ATTRIB_ATTACH_ITEM_ACK {
    std::int32_t                    m_iOK{};  // int
    std::vector<KInventoryItemInfo> m_vecInventorySlotInfo{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_vecInventorySlotInfo)>>::put(w, m_vecInventorySlotInfo);
    }

    static KEGS_ATTRIB_ATTACH_ITEM_ACK read_from(Reader& r) {
        KEGS_ATTRIB_ATTACH_ITEM_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_vecInventorySlotInfo = Codec<std::remove_cvref_t<decltype(v.m_vecInventorySlotInfo)>>::get(r);
        return v;
    }
};

struct KEGS_ATTRIB_ENCHANT_ITEM_ACK {
    std::int32_t                    m_iOK{};  // int
    std::int32_t                    m_iED{};  // int
    std::int64_t                    m_iEnchantedItemUID{};  // UidType
    std::int32_t                    m_iEnchantedItemID{};  // int
    char                            m_cAttribEnchantSlotNo{};
    char                            m_cAttribEnchantID{};
    std::vector<KInventoryItemInfo> m_vecInventorySlotInfo{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iED)>>::put(w, m_iED);
        Codec<std::remove_cvref_t<decltype(m_iEnchantedItemUID)>>::put(w, m_iEnchantedItemUID);
        Codec<std::remove_cvref_t<decltype(m_iEnchantedItemID)>>::put(w, m_iEnchantedItemID);
        Codec<std::remove_cvref_t<decltype(m_cAttribEnchantSlotNo)>>::put(w, m_cAttribEnchantSlotNo);
        Codec<std::remove_cvref_t<decltype(m_cAttribEnchantID)>>::put(w, m_cAttribEnchantID);
        Codec<std::remove_cvref_t<decltype(m_vecInventorySlotInfo)>>::put(w, m_vecInventorySlotInfo);
    }

    static KEGS_ATTRIB_ENCHANT_ITEM_ACK read_from(Reader& r) {
        KEGS_ATTRIB_ENCHANT_ITEM_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iED = Codec<std::remove_cvref_t<decltype(v.m_iED)>>::get(r);
        v.m_iEnchantedItemUID = Codec<std::remove_cvref_t<decltype(v.m_iEnchantedItemUID)>>::get(r);
        v.m_iEnchantedItemID = Codec<std::remove_cvref_t<decltype(v.m_iEnchantedItemID)>>::get(r);
        v.m_cAttribEnchantSlotNo = Codec<std::remove_cvref_t<decltype(v.m_cAttribEnchantSlotNo)>>::get(r);
        v.m_cAttribEnchantID = Codec<std::remove_cvref_t<decltype(v.m_cAttribEnchantID)>>::get(r);
        v.m_vecInventorySlotInfo = Codec<std::remove_cvref_t<decltype(v.m_vecInventorySlotInfo)>>::get(r);
        return v;
    }
};

struct KEGS_AUTO_FEED_NOT {
    std::int32_t                    m_iFeedItemID{};  // int
    std::vector<KInventoryItemInfo> m_vecPetInventorySlotInfo{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iFeedItemID)>>::put(w, m_iFeedItemID);
        Codec<std::remove_cvref_t<decltype(m_vecPetInventorySlotInfo)>>::put(w, m_vecPetInventorySlotInfo);
    }

    static KEGS_AUTO_FEED_NOT read_from(Reader& r) {
        KEGS_AUTO_FEED_NOT v;
        v.m_iFeedItemID = Codec<std::remove_cvref_t<decltype(v.m_iFeedItemID)>>::get(r);
        v.m_vecPetInventorySlotInfo = Codec<std::remove_cvref_t<decltype(v.m_vecPetInventorySlotInfo)>>::get(r);
        return v;
    }
};

struct KEGS_BUY_ED_ITEM_ACK {
    std::int32_t                    m_iOK{};  // int
    std::int32_t                    m_iED{};  // int
    std::int32_t                    m_iAPoint{};  // int
    std::vector<KInventoryItemInfo> m_vecInventorySlotInfo{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iED)>>::put(w, m_iED);
        Codec<std::remove_cvref_t<decltype(m_iAPoint)>>::put(w, m_iAPoint);
        Codec<std::remove_cvref_t<decltype(m_vecInventorySlotInfo)>>::put(w, m_vecInventorySlotInfo);
    }

    static KEGS_BUY_ED_ITEM_ACK read_from(Reader& r) {
        KEGS_BUY_ED_ITEM_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iED = Codec<std::remove_cvref_t<decltype(v.m_iED)>>::get(r);
        v.m_iAPoint = Codec<std::remove_cvref_t<decltype(v.m_iAPoint)>>::get(r);
        v.m_vecInventorySlotInfo = Codec<std::remove_cvref_t<decltype(v.m_vecInventorySlotInfo)>>::get(r);
        return v;
    }
};

struct KEGS_BUY_PERSONAL_SHOP_ITEM_COMPLETE_NOT {
    std::vector<KInventoryItemInfo> m_vecInventorySlotInfo{};  // std::vector< KInventoryItemInfo >
    std::int32_t                    m_iED{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecInventorySlotInfo)>>::put(w, m_vecInventorySlotInfo);
        Codec<std::remove_cvref_t<decltype(m_iED)>>::put(w, m_iED);
    }

    static KEGS_BUY_PERSONAL_SHOP_ITEM_COMPLETE_NOT read_from(Reader& r) {
        KEGS_BUY_PERSONAL_SHOP_ITEM_COMPLETE_NOT v;
        v.m_vecInventorySlotInfo = Codec<std::remove_cvref_t<decltype(v.m_vecInventorySlotInfo)>>::get(r);
        v.m_iED = Codec<std::remove_cvref_t<decltype(v.m_iED)>>::get(r);
        return v;
    }
};

struct KEGS_CHANGE_EQUIPPED_ITEM_IN_FIELD_NOT {
    std::int64_t                    m_iUnitUID{};  // UidType
    std::vector<KInventoryItemInfo> m_vecInventorySlotInfo{};  // std::vector< KInventoryItemInfo >
    std::int32_t                    m_iEquippedTitleID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_vecInventorySlotInfo)>>::put(w, m_vecInventorySlotInfo);
        Codec<std::remove_cvref_t<decltype(m_iEquippedTitleID)>>::put(w, m_iEquippedTitleID);
    }

    static KEGS_CHANGE_EQUIPPED_ITEM_IN_FIELD_NOT read_from(Reader& r) {
        KEGS_CHANGE_EQUIPPED_ITEM_IN_FIELD_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_vecInventorySlotInfo = Codec<std::remove_cvref_t<decltype(v.m_vecInventorySlotInfo)>>::get(r);
        v.m_iEquippedTitleID = Codec<std::remove_cvref_t<decltype(v.m_iEquippedTitleID)>>::get(r);
        return v;
    }
};

struct KEGS_CHANGE_EQUIPPED_ITEM_IN_SQUARE_NOT {
    std::int64_t                    m_UnitUID{};  // UidType
    std::vector<KInventoryItemInfo> m_vecInventorySlotInfo{};  // std::vector< KInventoryItemInfo >
    std::int32_t                    m_iEquippedTitleID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_UnitUID)>>::put(w, m_UnitUID);
        Codec<std::remove_cvref_t<decltype(m_vecInventorySlotInfo)>>::put(w, m_vecInventorySlotInfo);
        Codec<std::remove_cvref_t<decltype(m_iEquippedTitleID)>>::put(w, m_iEquippedTitleID);
    }

    static KEGS_CHANGE_EQUIPPED_ITEM_IN_SQUARE_NOT read_from(Reader& r) {
        KEGS_CHANGE_EQUIPPED_ITEM_IN_SQUARE_NOT v;
        v.m_UnitUID = Codec<std::remove_cvref_t<decltype(v.m_UnitUID)>>::get(r);
        v.m_vecInventorySlotInfo = Codec<std::remove_cvref_t<decltype(v.m_vecInventorySlotInfo)>>::get(r);
        v.m_iEquippedTitleID = Codec<std::remove_cvref_t<decltype(v.m_iEquippedTitleID)>>::get(r);
        return v;
    }
};

struct KEGS_CHANGE_INVENTORY_SLOT_ITEM_ACK {
    std::vector<KInventoryItemInfo> m_vecInventorySlotInfo{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecInventorySlotInfo)>>::put(w, m_vecInventorySlotInfo);
    }

    static KEGS_CHANGE_INVENTORY_SLOT_ITEM_ACK read_from(Reader& r) {
        KEGS_CHANGE_INVENTORY_SLOT_ITEM_ACK v;
        v.m_vecInventorySlotInfo = Codec<std::remove_cvref_t<decltype(v.m_vecInventorySlotInfo)>>::get(r);
        return v;
    }
};

struct KEGS_CHANGE_LOVE_WORD_NOT {
    std::int64_t                    m_iUseItemUnitUID{};  // UidType
    std::u16string                  m_wstrNewLoveWord{};  // std::wstring
    std::vector<KInventoryItemInfo> m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUseItemUnitUID)>>::put(w, m_iUseItemUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrNewLoveWord)>>::put(w, m_wstrNewLoveWord);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
    }

    static KEGS_CHANGE_LOVE_WORD_NOT read_from(Reader& r) {
        KEGS_CHANGE_LOVE_WORD_NOT v;
        v.m_iUseItemUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUseItemUnitUID)>>::get(r);
        v.m_wstrNewLoveWord = Codec<std::remove_cvref_t<decltype(v.m_wstrNewLoveWord)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        return v;
    }
};

struct KEGS_CHECK_TIME_EVENT_COMPLETE_ACK {
    std::int32_t                    m_iOK{};  // int
    std::int32_t                    m_iRewardCount{};  // int
    std::vector<KInventoryItemInfo> m_vecInventorySlotInfo{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iRewardCount)>>::put(w, m_iRewardCount);
        Codec<std::remove_cvref_t<decltype(m_vecInventorySlotInfo)>>::put(w, m_vecInventorySlotInfo);
    }

    static KEGS_CHECK_TIME_EVENT_COMPLETE_ACK read_from(Reader& r) {
        KEGS_CHECK_TIME_EVENT_COMPLETE_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iRewardCount = Codec<std::remove_cvref_t<decltype(v.m_iRewardCount)>>::get(r);
        v.m_vecInventorySlotInfo = Codec<std::remove_cvref_t<decltype(v.m_vecInventorySlotInfo)>>::get(r);
        return v;
    }
};

struct KEGS_COUPLE_PROPOSE_RESULT_NOT {
    std::int32_t                    m_iOK{};  // int
    char                            m_cResult{};
    std::int64_t                    m_iUnitUID{};  // UidType
    std::u16string                  m_wstrUnitName{};  // std::wstring
    std::int64_t                    m_tDate{};  // __int64
    std::vector<KInventoryItemInfo> m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_cResult)>>::put(w, m_cResult);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrUnitName)>>::put(w, m_wstrUnitName);
        Codec<std::remove_cvref_t<decltype(m_tDate)>>::put(w, m_tDate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
    }

    static KEGS_COUPLE_PROPOSE_RESULT_NOT read_from(Reader& r) {
        KEGS_COUPLE_PROPOSE_RESULT_NOT v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_cResult = Codec<std::remove_cvref_t<decltype(v.m_cResult)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_wstrUnitName = Codec<std::remove_cvref_t<decltype(v.m_wstrUnitName)>>::get(r);
        v.m_tDate = Codec<std::remove_cvref_t<decltype(v.m_tDate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        return v;
    }
};

struct KEGS_CREATE_GUILD_ACK {
    std::int32_t                    m_iOK{};  // int
    KGuildInfo                      m_kGuildInfo{};
    std::vector<KInventoryItemInfo> m_vecInventorySlotInfo{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_kGuildInfo)>>::put(w, m_kGuildInfo);
        Codec<std::remove_cvref_t<decltype(m_vecInventorySlotInfo)>>::put(w, m_vecInventorySlotInfo);
    }

    static KEGS_CREATE_GUILD_ACK read_from(Reader& r) {
        KEGS_CREATE_GUILD_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_kGuildInfo = Codec<std::remove_cvref_t<decltype(v.m_kGuildInfo)>>::get(r);
        v.m_vecInventorySlotInfo = Codec<std::remove_cvref_t<decltype(v.m_vecInventorySlotInfo)>>::get(r);
        return v;
    }
};

struct KEGS_DELETE_ITEM_ACK {
    std::int32_t       m_iOK{};  // int
    KInventoryItemInfo m_kInventoryItemInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_kInventoryItemInfo)>>::put(w, m_kInventoryItemInfo);
    }

    static KEGS_DELETE_ITEM_ACK read_from(Reader& r) {
        KEGS_DELETE_ITEM_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_kInventoryItemInfo = Codec<std::remove_cvref_t<decltype(v.m_kInventoryItemInfo)>>::get(r);
        return v;
    }
};

struct KEGS_ENCHANT_ATTACH_ITEM_ACK {
    std::int32_t                    m_iOK{};  // int
    std::vector<KInventoryItemInfo> m_vecKInventorySlotInfo{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_vecKInventorySlotInfo)>>::put(w, m_vecKInventorySlotInfo);
    }

    static KEGS_ENCHANT_ATTACH_ITEM_ACK read_from(Reader& r) {
        KEGS_ENCHANT_ATTACH_ITEM_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_vecKInventorySlotInfo = Codec<std::remove_cvref_t<decltype(v.m_vecKInventorySlotInfo)>>::get(r);
        return v;
    }
};

struct KEGS_ENCHANT_ITEM_ACK {
    std::int32_t                    m_iOK{};  // int
    std::int32_t                    m_iED{};  // int
    std::int32_t                    m_iEnchantResult{};  // int
    std::int64_t                    m_iEnchantedItemUID{};  // UidType
    std::int32_t                    m_iEnchantedItemID{};  // int
    std::vector<KInventoryItemInfo> m_vecInventorySlotInfo{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iED)>>::put(w, m_iED);
        Codec<std::remove_cvref_t<decltype(m_iEnchantResult)>>::put(w, m_iEnchantResult);
        Codec<std::remove_cvref_t<decltype(m_iEnchantedItemUID)>>::put(w, m_iEnchantedItemUID);
        Codec<std::remove_cvref_t<decltype(m_iEnchantedItemID)>>::put(w, m_iEnchantedItemID);
        Codec<std::remove_cvref_t<decltype(m_vecInventorySlotInfo)>>::put(w, m_vecInventorySlotInfo);
    }

    static KEGS_ENCHANT_ITEM_ACK read_from(Reader& r) {
        KEGS_ENCHANT_ITEM_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iED = Codec<std::remove_cvref_t<decltype(v.m_iED)>>::get(r);
        v.m_iEnchantResult = Codec<std::remove_cvref_t<decltype(v.m_iEnchantResult)>>::get(r);
        v.m_iEnchantedItemUID = Codec<std::remove_cvref_t<decltype(v.m_iEnchantedItemUID)>>::get(r);
        v.m_iEnchantedItemID = Codec<std::remove_cvref_t<decltype(v.m_iEnchantedItemID)>>::get(r);
        v.m_vecInventorySlotInfo = Codec<std::remove_cvref_t<decltype(v.m_vecInventorySlotInfo)>>::get(r);
        return v;
    }
};

struct KEGS_FEED_PETS_ACK {
    std::int32_t                    m_iOK{};  // int
    std::int16_t                    m_sSatiety{};  // short
    std::vector<KInventoryItemInfo> m_vecInventorySlotInfo{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_sSatiety)>>::put(w, m_sSatiety);
        Codec<std::remove_cvref_t<decltype(m_vecInventorySlotInfo)>>::put(w, m_vecInventorySlotInfo);
    }

    static KEGS_FEED_PETS_ACK read_from(Reader& r) {
        KEGS_FEED_PETS_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_sSatiety = Codec<std::remove_cvref_t<decltype(v.m_sSatiety)>>::get(r);
        v.m_vecInventorySlotInfo = Codec<std::remove_cvref_t<decltype(v.m_vecInventorySlotInfo)>>::get(r);
        return v;
    }
};

struct KEGS_GET_ITEM_FROM_LETTER_ACK {
    std::int32_t                         m_iOK{};  // int
    std::int64_t                         m_iPostNo{};  // UidType
    char                                 m_cLetterType{};
    std::map<std::int32_t, std::int32_t> m_mapResultItem{};  // std::map< int, int >
    std::vector<KInventoryItemInfo>      m_vecKInventorySlotInfo{};  // std::vector< KInventoryItemInfo >
    std::int32_t                         m_iED{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iPostNo)>>::put(w, m_iPostNo);
        Codec<std::remove_cvref_t<decltype(m_cLetterType)>>::put(w, m_cLetterType);
        Codec<std::remove_cvref_t<decltype(m_mapResultItem)>>::put(w, m_mapResultItem);
        Codec<std::remove_cvref_t<decltype(m_vecKInventorySlotInfo)>>::put(w, m_vecKInventorySlotInfo);
        Codec<std::remove_cvref_t<decltype(m_iED)>>::put(w, m_iED);
    }

    static KEGS_GET_ITEM_FROM_LETTER_ACK read_from(Reader& r) {
        KEGS_GET_ITEM_FROM_LETTER_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iPostNo = Codec<std::remove_cvref_t<decltype(v.m_iPostNo)>>::get(r);
        v.m_cLetterType = Codec<std::remove_cvref_t<decltype(v.m_cLetterType)>>::get(r);
        v.m_mapResultItem = Codec<std::remove_cvref_t<decltype(v.m_mapResultItem)>>::get(r);
        v.m_vecKInventorySlotInfo = Codec<std::remove_cvref_t<decltype(v.m_vecKInventorySlotInfo)>>::get(r);
        v.m_iED = Codec<std::remove_cvref_t<decltype(v.m_iED)>>::get(r);
        return v;
    }
};

struct KEGS_GET_ITEM_REALTIME_NOT {
    std::vector<KInventoryItemInfo> m_vecKInventorySlotInfo{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecKInventorySlotInfo)>>::put(w, m_vecKInventorySlotInfo);
    }

    static KEGS_GET_ITEM_REALTIME_NOT read_from(Reader& r) {
        KEGS_GET_ITEM_REALTIME_NOT v;
        v.m_vecKInventorySlotInfo = Codec<std::remove_cvref_t<decltype(v.m_vecKInventorySlotInfo)>>::get(r);
        return v;
    }
};

struct KEGS_GET_MY_BANK_INFO_ACK {
    std::map<std::int64_t, KInventoryItemInfo> m_mapItem{};  // std::map< UidType, KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_mapItem)>>::put(w, m_mapItem);
    }

    static KEGS_GET_MY_BANK_INFO_ACK read_from(Reader& r) {
        KEGS_GET_MY_BANK_INFO_ACK v;
        v.m_mapItem = Codec<std::remove_cvref_t<decltype(v.m_mapItem)>>::get(r);
        return v;
    }
};

struct KEGS_GET_PURCHASED_CASH_ITEM_ACK {
    std::int32_t                         m_iOK{};  // int
    std::int32_t                         m_iNumResurrectionStone{};  // int
    std::map<std::int32_t, std::int32_t> m_mapExpandedCategorySlot{};  // std::map< int, int >
    std::vector<KInventoryItemInfo>      m_vecInventorySlotInfo{};  // std::vector< KInventoryItemInfo >
    std::int32_t                         m_iExceptionProcessItemID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iNumResurrectionStone)>>::put(w, m_iNumResurrectionStone);
        Codec<std::remove_cvref_t<decltype(m_mapExpandedCategorySlot)>>::put(w, m_mapExpandedCategorySlot);
        Codec<std::remove_cvref_t<decltype(m_vecInventorySlotInfo)>>::put(w, m_vecInventorySlotInfo);
        Codec<std::remove_cvref_t<decltype(m_iExceptionProcessItemID)>>::put(w, m_iExceptionProcessItemID);
    }

    static KEGS_GET_PURCHASED_CASH_ITEM_ACK read_from(Reader& r) {
        KEGS_GET_PURCHASED_CASH_ITEM_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iNumResurrectionStone = Codec<std::remove_cvref_t<decltype(v.m_iNumResurrectionStone)>>::get(r);
        v.m_mapExpandedCategorySlot = Codec<std::remove_cvref_t<decltype(v.m_mapExpandedCategorySlot)>>::get(r);
        v.m_vecInventorySlotInfo = Codec<std::remove_cvref_t<decltype(v.m_vecInventorySlotInfo)>>::get(r);
        v.m_iExceptionProcessItemID = Codec<std::remove_cvref_t<decltype(v.m_iExceptionProcessItemID)>>::get(r);
        return v;
    }
};

struct KEGS_GET_SHARE_BANK_ACK {
    std::int32_t                               m_iOK{};  // int
    std::int64_t                               m_iUnitUID{};  // UidType
    std::u16string                             m_wstrNickName{};  // std::wstring
    std::int32_t                               m_iBankSize{};  // int
    std::map<std::int64_t, KInventoryItemInfo> m_mapItem{};  // std::map< UidType, KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_iBankSize)>>::put(w, m_iBankSize);
        Codec<std::remove_cvref_t<decltype(m_mapItem)>>::put(w, m_mapItem);
    }

    static KEGS_GET_SHARE_BANK_ACK read_from(Reader& r) {
        KEGS_GET_SHARE_BANK_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_iBankSize = Codec<std::remove_cvref_t<decltype(v.m_iBankSize)>>::get(r);
        v.m_mapItem = Codec<std::remove_cvref_t<decltype(v.m_mapItem)>>::get(r);
        return v;
    }
};

struct KEGS_GET_TEMP_ITEM_ACK {
    std::int32_t                    m_iOK{};  // int
    std::int32_t                    m_iItemID{};  // int
    std::int32_t                    m_iTempItemUID{};  // int
    std::vector<KInventoryItemInfo> m_vecInventorySlotInfo{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iItemID)>>::put(w, m_iItemID);
        Codec<std::remove_cvref_t<decltype(m_iTempItemUID)>>::put(w, m_iTempItemUID);
        Codec<std::remove_cvref_t<decltype(m_vecInventorySlotInfo)>>::put(w, m_vecInventorySlotInfo);
    }

    static KEGS_GET_TEMP_ITEM_ACK read_from(Reader& r) {
        KEGS_GET_TEMP_ITEM_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iItemID = Codec<std::remove_cvref_t<decltype(v.m_iItemID)>>::get(r);
        v.m_iTempItemUID = Codec<std::remove_cvref_t<decltype(v.m_iTempItemUID)>>::get(r);
        v.m_vecInventorySlotInfo = Codec<std::remove_cvref_t<decltype(v.m_vecInventorySlotInfo)>>::get(r);
        return v;
    }
};

struct KEGS_IDENTIFY_ITEM_ACK {
    std::int32_t                         m_iOK{};  // int
    std::map<std::int32_t, std::int32_t> m_mapInsertedItem{};  // std::map< int, int >
    std::vector<KInventoryItemInfo>      m_vecKInventorySlotInfo{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_mapInsertedItem)>>::put(w, m_mapInsertedItem);
        Codec<std::remove_cvref_t<decltype(m_vecKInventorySlotInfo)>>::put(w, m_vecKInventorySlotInfo);
    }

    static KEGS_IDENTIFY_ITEM_ACK read_from(Reader& r) {
        KEGS_IDENTIFY_ITEM_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_mapInsertedItem = Codec<std::remove_cvref_t<decltype(v.m_mapInsertedItem)>>::get(r);
        v.m_vecKInventorySlotInfo = Codec<std::remove_cvref_t<decltype(v.m_vecKInventorySlotInfo)>>::get(r);
        return v;
    }
};

struct KEGS_INIT_GUILD_SKILL_ACK {
    std::int32_t                    m_iOK{};  // int
    std::vector<KInventoryItemInfo> m_vecInventorySlotInfo{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_vecInventorySlotInfo)>>::put(w, m_vecInventorySlotInfo);
    }

    static KEGS_INIT_GUILD_SKILL_ACK read_from(Reader& r) {
        KEGS_INIT_GUILD_SKILL_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_vecInventorySlotInfo = Codec<std::remove_cvref_t<decltype(v.m_vecInventorySlotInfo)>>::get(r);
        return v;
    }
};

struct KEGS_INIT_SKILL_TREE_ACK {
    std::int32_t                    m_iOK{};  // int
    std::int32_t                    m_iActiveSkillPageNumber{};  // int
    std::int32_t                    m_iSPoint{};  // int
    std::int32_t                    m_iCSPoint{};  // int
    std::vector<KInventoryItemInfo> m_vecInventorySlotInfo{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iActiveSkillPageNumber)>>::put(w, m_iActiveSkillPageNumber);
        Codec<std::remove_cvref_t<decltype(m_iSPoint)>>::put(w, m_iSPoint);
        Codec<std::remove_cvref_t<decltype(m_iCSPoint)>>::put(w, m_iCSPoint);
        Codec<std::remove_cvref_t<decltype(m_vecInventorySlotInfo)>>::put(w, m_vecInventorySlotInfo);
    }

    static KEGS_INIT_SKILL_TREE_ACK read_from(Reader& r) {
        KEGS_INIT_SKILL_TREE_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iActiveSkillPageNumber = Codec<std::remove_cvref_t<decltype(v.m_iActiveSkillPageNumber)>>::get(r);
        v.m_iSPoint = Codec<std::remove_cvref_t<decltype(v.m_iSPoint)>>::get(r);
        v.m_iCSPoint = Codec<std::remove_cvref_t<decltype(v.m_iCSPoint)>>::get(r);
        v.m_vecInventorySlotInfo = Codec<std::remove_cvref_t<decltype(v.m_vecInventorySlotInfo)>>::get(r);
        return v;
    }
};

struct KEGS_ITEM_CONVERT_ACK {
    std::int32_t                    m_iOK{};  // int
    std::int32_t                    m_iED{};  // int
    std::vector<KInventoryItemInfo> m_vecInventorySlotInfo{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iED)>>::put(w, m_iED);
        Codec<std::remove_cvref_t<decltype(m_vecInventorySlotInfo)>>::put(w, m_vecInventorySlotInfo);
    }

    static KEGS_ITEM_CONVERT_ACK read_from(Reader& r) {
        KEGS_ITEM_CONVERT_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iED = Codec<std::remove_cvref_t<decltype(v.m_iED)>>::get(r);
        v.m_vecInventorySlotInfo = Codec<std::remove_cvref_t<decltype(v.m_vecInventorySlotInfo)>>::get(r);
        return v;
    }
};

struct KEGS_ITEM_EVALUATE_ACK {
    std::int32_t                    m_iOK{};  // int
    std::int32_t                    m_iED{};  // int
    std::vector<KInventoryItemInfo> m_vecInventorySlotInfo{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iED)>>::put(w, m_iED);
        Codec<std::remove_cvref_t<decltype(m_vecInventorySlotInfo)>>::put(w, m_vecInventorySlotInfo);
    }

    static KEGS_ITEM_EVALUATE_ACK read_from(Reader& r) {
        KEGS_ITEM_EVALUATE_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iED = Codec<std::remove_cvref_t<decltype(v.m_iED)>>::get(r);
        v.m_vecInventorySlotInfo = Codec<std::remove_cvref_t<decltype(v.m_vecInventorySlotInfo)>>::get(r);
        return v;
    }
};

struct KEGS_ITEM_EXCHANGE_ACK {
    std::int32_t                         m_iOK{};  // int
    std::vector<KInventoryItemInfo>      m_vecKInventorySlotInfo{};  // std::vector< KInventoryItemInfo >
    std::map<std::int32_t, std::int32_t> m_mapResultItem{};  // std::map< int, int >
    std::int32_t                         m_iExchangeCount{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_vecKInventorySlotInfo)>>::put(w, m_vecKInventorySlotInfo);
        Codec<std::remove_cvref_t<decltype(m_mapResultItem)>>::put(w, m_mapResultItem);
        Codec<std::remove_cvref_t<decltype(m_iExchangeCount)>>::put(w, m_iExchangeCount);
    }

    static KEGS_ITEM_EXCHANGE_ACK read_from(Reader& r) {
        KEGS_ITEM_EXCHANGE_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_vecKInventorySlotInfo = Codec<std::remove_cvref_t<decltype(v.m_vecKInventorySlotInfo)>>::get(r);
        v.m_mapResultItem = Codec<std::remove_cvref_t<decltype(v.m_mapResultItem)>>::get(r);
        v.m_iExchangeCount = Codec<std::remove_cvref_t<decltype(v.m_iExchangeCount)>>::get(r);
        return v;
    }
};

struct KEGS_ITEM_EXTRACT_ACK {
    std::int32_t                         m_iOK{};  // int
    std::vector<KInventoryItemInfo>      m_vecKInventorySlotInfo{};  // std::vector< KInventoryItemInfo >
    std::map<std::int32_t, std::int32_t> m_mapInsertedItem{};  // std::map< int, int >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_vecKInventorySlotInfo)>>::put(w, m_vecKInventorySlotInfo);
        Codec<std::remove_cvref_t<decltype(m_mapInsertedItem)>>::put(w, m_mapInsertedItem);
    }

    static KEGS_ITEM_EXTRACT_ACK read_from(Reader& r) {
        KEGS_ITEM_EXTRACT_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_vecKInventorySlotInfo = Codec<std::remove_cvref_t<decltype(v.m_vecKInventorySlotInfo)>>::get(r);
        v.m_mapInsertedItem = Codec<std::remove_cvref_t<decltype(v.m_mapInsertedItem)>>::get(r);
        return v;
    }
};

struct KEGS_ITEM_MANUFACTURE_ACK {
    std::int32_t                         m_iOK{};  // int
    std::map<std::int32_t, std::int32_t> m_mapInsertedItem{};  // std::map< int, int >
    std::int32_t                         m_iED{};  // int
    std::vector<KInventoryItemInfo>      m_vecKInventorySlotInfo{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_mapInsertedItem)>>::put(w, m_mapInsertedItem);
        Codec<std::remove_cvref_t<decltype(m_iED)>>::put(w, m_iED);
        Codec<std::remove_cvref_t<decltype(m_vecKInventorySlotInfo)>>::put(w, m_vecKInventorySlotInfo);
    }

    static KEGS_ITEM_MANUFACTURE_ACK read_from(Reader& r) {
        KEGS_ITEM_MANUFACTURE_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_mapInsertedItem = Codec<std::remove_cvref_t<decltype(v.m_mapInsertedItem)>>::get(r);
        v.m_iED = Codec<std::remove_cvref_t<decltype(v.m_iED)>>::get(r);
        v.m_vecKInventorySlotInfo = Codec<std::remove_cvref_t<decltype(v.m_vecKInventorySlotInfo)>>::get(r);
        return v;
    }
};

struct KEGS_OPEN_RANDOM_ITEM_ACK {
    std::int32_t                         m_iOK{};  // int
    std::int32_t                         m_iRessurectionCount{};  // int
    std::int32_t                         m_iRestoreSpirit{};  // int
    std::map<std::int32_t, std::int32_t> m_mapInsertedItem{};  // std::map< int, int >
    std::vector<KInventoryItemInfo>      m_vecKInventorySlotInfo{};  // std::vector< KInventoryItemInfo >
    std::int32_t                         m_iED{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iRessurectionCount)>>::put(w, m_iRessurectionCount);
        Codec<std::remove_cvref_t<decltype(m_iRestoreSpirit)>>::put(w, m_iRestoreSpirit);
        Codec<std::remove_cvref_t<decltype(m_mapInsertedItem)>>::put(w, m_mapInsertedItem);
        Codec<std::remove_cvref_t<decltype(m_vecKInventorySlotInfo)>>::put(w, m_vecKInventorySlotInfo);
        Codec<std::remove_cvref_t<decltype(m_iED)>>::put(w, m_iED);
    }

    static KEGS_OPEN_RANDOM_ITEM_ACK read_from(Reader& r) {
        KEGS_OPEN_RANDOM_ITEM_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iRessurectionCount = Codec<std::remove_cvref_t<decltype(v.m_iRessurectionCount)>>::get(r);
        v.m_iRestoreSpirit = Codec<std::remove_cvref_t<decltype(v.m_iRestoreSpirit)>>::get(r);
        v.m_mapInsertedItem = Codec<std::remove_cvref_t<decltype(v.m_mapInsertedItem)>>::get(r);
        v.m_vecKInventorySlotInfo = Codec<std::remove_cvref_t<decltype(v.m_vecKInventorySlotInfo)>>::get(r);
        v.m_iED = Codec<std::remove_cvref_t<decltype(v.m_iED)>>::get(r);
        return v;
    }
};

struct KEGS_REG_SKILL_NOTE_MEMO_ACK {
    std::int32_t                    m_iOK{};  // int
    char                            m_cSkillNotePageNum{};
    std::int32_t                    m_iMemoID{};  // int
    std::vector<KInventoryItemInfo> m_vecInventorySlotInfo{};  // std::vector< KInventoryItemInfo >
    std::vector<std::int32_t>       m_vecSkillNote{};  // std::vector<int>

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_cSkillNotePageNum)>>::put(w, m_cSkillNotePageNum);
        Codec<std::remove_cvref_t<decltype(m_iMemoID)>>::put(w, m_iMemoID);
        Codec<std::remove_cvref_t<decltype(m_vecInventorySlotInfo)>>::put(w, m_vecInventorySlotInfo);
        Codec<std::remove_cvref_t<decltype(m_vecSkillNote)>>::put(w, m_vecSkillNote);
    }

    static KEGS_REG_SKILL_NOTE_MEMO_ACK read_from(Reader& r) {
        KEGS_REG_SKILL_NOTE_MEMO_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_cSkillNotePageNum = Codec<std::remove_cvref_t<decltype(v.m_cSkillNotePageNum)>>::get(r);
        v.m_iMemoID = Codec<std::remove_cvref_t<decltype(v.m_iMemoID)>>::get(r);
        v.m_vecInventorySlotInfo = Codec<std::remove_cvref_t<decltype(v.m_vecInventorySlotInfo)>>::get(r);
        v.m_vecSkillNote = Codec<std::remove_cvref_t<decltype(v.m_vecSkillNote)>>::get(r);
        return v;
    }
};

struct KEGS_RELATIONSHIP_INFO_ACK {
    std::int32_t                               m_iOK{};  // int
    std::int64_t                               m_iRequestGSUID{};  // UidType
    std::int64_t                               m_iRequestUnitUID{};  // UidType
    std::int64_t                               m_iAcceptUnitUID{};  // UidType
    std::uint8_t                               m_ucMyLevel{};  // u_char
    char                                       m_cMyUnitClass{};
    std::int32_t                               m_iMyMapID{};  // int
    char                                       m_cMyPosition{};
    std::u16string                             m_wstrMyChannelName{};  // std::wstring
    std::map<std::int32_t, KInventoryItemInfo> m_mapOtherUnitEquippedItem{};  // std::map< int, KInventoryItemInfo >
    std::uint8_t                               m_ucLevel{};  // u_char
    char                                       m_cUnitClass{};
    std::int32_t                               m_iMapID{};  // int
    std::int64_t                               m_tLastLogOutTime{};  // __int64
    bool                                       m_bIsConnect{};
    std::u16string                             m_wstrChannelName{};  // std::wstring
    char                                       m_cPosition{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iRequestGSUID)>>::put(w, m_iRequestGSUID);
        Codec<std::remove_cvref_t<decltype(m_iRequestUnitUID)>>::put(w, m_iRequestUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iAcceptUnitUID)>>::put(w, m_iAcceptUnitUID);
        Codec<std::remove_cvref_t<decltype(m_ucMyLevel)>>::put(w, m_ucMyLevel);
        Codec<std::remove_cvref_t<decltype(m_cMyUnitClass)>>::put(w, m_cMyUnitClass);
        Codec<std::remove_cvref_t<decltype(m_iMyMapID)>>::put(w, m_iMyMapID);
        Codec<std::remove_cvref_t<decltype(m_cMyPosition)>>::put(w, m_cMyPosition);
        Codec<std::remove_cvref_t<decltype(m_wstrMyChannelName)>>::put(w, m_wstrMyChannelName);
        Codec<std::remove_cvref_t<decltype(m_mapOtherUnitEquippedItem)>>::put(w, m_mapOtherUnitEquippedItem);
        Codec<std::remove_cvref_t<decltype(m_ucLevel)>>::put(w, m_ucLevel);
        Codec<std::remove_cvref_t<decltype(m_cUnitClass)>>::put(w, m_cUnitClass);
        Codec<std::remove_cvref_t<decltype(m_iMapID)>>::put(w, m_iMapID);
        Codec<std::remove_cvref_t<decltype(m_tLastLogOutTime)>>::put(w, m_tLastLogOutTime);
        Codec<std::remove_cvref_t<decltype(m_bIsConnect)>>::put(w, m_bIsConnect);
        Codec<std::remove_cvref_t<decltype(m_wstrChannelName)>>::put(w, m_wstrChannelName);
        Codec<std::remove_cvref_t<decltype(m_cPosition)>>::put(w, m_cPosition);
    }

    static KEGS_RELATIONSHIP_INFO_ACK read_from(Reader& r) {
        KEGS_RELATIONSHIP_INFO_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iRequestGSUID = Codec<std::remove_cvref_t<decltype(v.m_iRequestGSUID)>>::get(r);
        v.m_iRequestUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iRequestUnitUID)>>::get(r);
        v.m_iAcceptUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iAcceptUnitUID)>>::get(r);
        v.m_ucMyLevel = Codec<std::remove_cvref_t<decltype(v.m_ucMyLevel)>>::get(r);
        v.m_cMyUnitClass = Codec<std::remove_cvref_t<decltype(v.m_cMyUnitClass)>>::get(r);
        v.m_iMyMapID = Codec<std::remove_cvref_t<decltype(v.m_iMyMapID)>>::get(r);
        v.m_cMyPosition = Codec<std::remove_cvref_t<decltype(v.m_cMyPosition)>>::get(r);
        v.m_wstrMyChannelName = Codec<std::remove_cvref_t<decltype(v.m_wstrMyChannelName)>>::get(r);
        v.m_mapOtherUnitEquippedItem = Codec<std::remove_cvref_t<decltype(v.m_mapOtherUnitEquippedItem)>>::get(r);
        v.m_ucLevel = Codec<std::remove_cvref_t<decltype(v.m_ucLevel)>>::get(r);
        v.m_cUnitClass = Codec<std::remove_cvref_t<decltype(v.m_cUnitClass)>>::get(r);
        v.m_iMapID = Codec<std::remove_cvref_t<decltype(v.m_iMapID)>>::get(r);
        v.m_tLastLogOutTime = Codec<std::remove_cvref_t<decltype(v.m_tLastLogOutTime)>>::get(r);
        v.m_bIsConnect = Codec<std::remove_cvref_t<decltype(v.m_bIsConnect)>>::get(r);
        v.m_wstrChannelName = Codec<std::remove_cvref_t<decltype(v.m_wstrChannelName)>>::get(r);
        v.m_cPosition = Codec<std::remove_cvref_t<decltype(v.m_cPosition)>>::get(r);
        return v;
    }
};

struct KEGS_REPAIR_ITEM_ACK {
    std::int32_t                    m_iOK{};  // int
    std::int32_t                    m_iED{};  // int
    std::int32_t                    m_iAPoint{};  // int
    std::vector<KInventoryItemInfo> m_vecInventorySlotInfo{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iED)>>::put(w, m_iED);
        Codec<std::remove_cvref_t<decltype(m_iAPoint)>>::put(w, m_iAPoint);
        Codec<std::remove_cvref_t<decltype(m_vecInventorySlotInfo)>>::put(w, m_vecInventorySlotInfo);
    }

    static KEGS_REPAIR_ITEM_ACK read_from(Reader& r) {
        KEGS_REPAIR_ITEM_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iED = Codec<std::remove_cvref_t<decltype(v.m_iED)>>::get(r);
        v.m_iAPoint = Codec<std::remove_cvref_t<decltype(v.m_iAPoint)>>::get(r);
        v.m_vecInventorySlotInfo = Codec<std::remove_cvref_t<decltype(v.m_vecInventorySlotInfo)>>::get(r);
        return v;
    }
};

struct KEGS_RESET_GUILD_SKILL_ACK {
    std::int32_t                    m_iOK{};  // int
    std::vector<KInventoryItemInfo> m_vecInventorySlotInfo{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_vecInventorySlotInfo)>>::put(w, m_vecInventorySlotInfo);
    }

    static KEGS_RESET_GUILD_SKILL_ACK read_from(Reader& r) {
        KEGS_RESET_GUILD_SKILL_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_vecInventorySlotInfo = Codec<std::remove_cvref_t<decltype(v.m_vecInventorySlotInfo)>>::get(r);
        return v;
    }
};

struct KEGS_RESET_PERIOD_ITEM_NOT {
    std::vector<KInventoryItemInfo> m_vecInventorySlotInfo{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecInventorySlotInfo)>>::put(w, m_vecInventorySlotInfo);
    }

    static KEGS_RESET_PERIOD_ITEM_NOT read_from(Reader& r) {
        KEGS_RESET_PERIOD_ITEM_NOT v;
        v.m_vecInventorySlotInfo = Codec<std::remove_cvref_t<decltype(v.m_vecInventorySlotInfo)>>::get(r);
        return v;
    }
};

struct KEGS_RESET_SKILL_ACK {
    std::int32_t                    m_iOK{};  // int
    std::int32_t                    m_iActiveSkillPageNumber{};  // int
    std::int32_t                    m_iDelSkillID{};  // int
    std::int32_t                    m_iSPoint{};  // int
    std::int32_t                    m_iCSPoint{};  // int
    std::vector<KInventoryItemInfo> m_vecInventorySlotInfo{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iActiveSkillPageNumber)>>::put(w, m_iActiveSkillPageNumber);
        Codec<std::remove_cvref_t<decltype(m_iDelSkillID)>>::put(w, m_iDelSkillID);
        Codec<std::remove_cvref_t<decltype(m_iSPoint)>>::put(w, m_iSPoint);
        Codec<std::remove_cvref_t<decltype(m_iCSPoint)>>::put(w, m_iCSPoint);
        Codec<std::remove_cvref_t<decltype(m_vecInventorySlotInfo)>>::put(w, m_vecInventorySlotInfo);
    }

    static KEGS_RESET_SKILL_ACK read_from(Reader& r) {
        KEGS_RESET_SKILL_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iActiveSkillPageNumber = Codec<std::remove_cvref_t<decltype(v.m_iActiveSkillPageNumber)>>::get(r);
        v.m_iDelSkillID = Codec<std::remove_cvref_t<decltype(v.m_iDelSkillID)>>::get(r);
        v.m_iSPoint = Codec<std::remove_cvref_t<decltype(v.m_iSPoint)>>::get(r);
        v.m_iCSPoint = Codec<std::remove_cvref_t<decltype(v.m_iCSPoint)>>::get(r);
        v.m_vecInventorySlotInfo = Codec<std::remove_cvref_t<decltype(v.m_vecInventorySlotInfo)>>::get(r);
        return v;
    }
};

struct KEGS_RESOLVE_ITEM_ACK {
    std::int32_t                         m_iOK{};  // int
    bool                                 m_bJackpot{};
    std::map<std::int32_t, std::int32_t> m_mapInsertedItem{};  // std::map< int, int >
    std::vector<KInventoryItemInfo>      m_vecKInventorySlotInfo{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_bJackpot)>>::put(w, m_bJackpot);
        Codec<std::remove_cvref_t<decltype(m_mapInsertedItem)>>::put(w, m_mapInsertedItem);
        Codec<std::remove_cvref_t<decltype(m_vecKInventorySlotInfo)>>::put(w, m_vecKInventorySlotInfo);
    }

    static KEGS_RESOLVE_ITEM_ACK read_from(Reader& r) {
        KEGS_RESOLVE_ITEM_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_bJackpot = Codec<std::remove_cvref_t<decltype(v.m_bJackpot)>>::get(r);
        v.m_mapInsertedItem = Codec<std::remove_cvref_t<decltype(v.m_mapInsertedItem)>>::get(r);
        v.m_vecKInventorySlotInfo = Codec<std::remove_cvref_t<decltype(v.m_vecKInventorySlotInfo)>>::get(r);
        return v;
    }
};

struct KEGS_RESTORE_ITEM_ACK {
    std::int32_t                    m_iOK{};  // int
    std::vector<KInventoryItemInfo> m_vecKInventorySlotInfo{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_vecKInventorySlotInfo)>>::put(w, m_vecKInventorySlotInfo);
    }

    static KEGS_RESTORE_ITEM_ACK read_from(Reader& r) {
        KEGS_RESTORE_ITEM_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_vecKInventorySlotInfo = Codec<std::remove_cvref_t<decltype(v.m_vecKInventorySlotInfo)>>::get(r);
        return v;
    }
};

struct KEGS_RESTORE_ITEM_EVALUATE_ACK {
    std::int32_t                    m_iOK{};  // int
    std::vector<KInventoryItemInfo> m_vecInventorySlotInfo{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_vecInventorySlotInfo)>>::put(w, m_vecInventorySlotInfo);
    }

    static KEGS_RESTORE_ITEM_EVALUATE_ACK read_from(Reader& r) {
        KEGS_RESTORE_ITEM_EVALUATE_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_vecInventorySlotInfo = Codec<std::remove_cvref_t<decltype(v.m_vecInventorySlotInfo)>>::get(r);
        return v;
    }
};

struct KEGS_SEAL_ITEM_ACK {
    std::int32_t                    m_iOK{};  // int
    std::vector<KInventoryItemInfo> m_vecInventorySlotInfo{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_vecInventorySlotInfo)>>::put(w, m_vecInventorySlotInfo);
    }

    static KEGS_SEAL_ITEM_ACK read_from(Reader& r) {
        KEGS_SEAL_ITEM_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_vecInventorySlotInfo = Codec<std::remove_cvref_t<decltype(v.m_vecInventorySlotInfo)>>::get(r);
        return v;
    }
};

struct KEGS_SELECT_UNIT_INVENTORY_INFO_NOT {
    std::int32_t                               m_iSlotCategory{};  // int
    std::map<std::int64_t, KInventoryItemInfo> m_mapItem{};  // std::map< UidType, KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iSlotCategory)>>::put(w, m_iSlotCategory);
        Codec<std::remove_cvref_t<decltype(m_mapItem)>>::put(w, m_mapItem);
    }

    static KEGS_SELECT_UNIT_INVENTORY_INFO_NOT read_from(Reader& r) {
        KEGS_SELECT_UNIT_INVENTORY_INFO_NOT v;
        v.m_iSlotCategory = Codec<std::remove_cvref_t<decltype(v.m_iSlotCategory)>>::get(r);
        v.m_mapItem = Codec<std::remove_cvref_t<decltype(v.m_mapItem)>>::get(r);
        return v;
    }
};

struct KEGS_SELL_ED_ITEM_ACK {
    std::int32_t       m_iOK{};  // int
    std::int32_t       m_iED{};  // int
    KInventoryItemInfo m_kInventorySlotInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iED)>>::put(w, m_iED);
        Codec<std::remove_cvref_t<decltype(m_kInventorySlotInfo)>>::put(w, m_kInventorySlotInfo);
    }

    static KEGS_SELL_ED_ITEM_ACK read_from(Reader& r) {
        KEGS_SELL_ED_ITEM_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iED = Codec<std::remove_cvref_t<decltype(v.m_iED)>>::get(r);
        v.m_kInventorySlotInfo = Codec<std::remove_cvref_t<decltype(v.m_kInventorySlotInfo)>>::get(r);
        return v;
    }
};

struct KEGS_SEND_LETTER_ACK {
    std::int32_t                    m_iOK{};  // int
    std::int32_t                    m_iED{};  // int
    std::vector<KInventoryItemInfo> m_vecKInventorySlotInfo{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iED)>>::put(w, m_iED);
        Codec<std::remove_cvref_t<decltype(m_vecKInventorySlotInfo)>>::put(w, m_vecKInventorySlotInfo);
    }

    static KEGS_SEND_LETTER_ACK read_from(Reader& r) {
        KEGS_SEND_LETTER_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iED = Codec<std::remove_cvref_t<decltype(v.m_iED)>>::get(r);
        v.m_vecKInventorySlotInfo = Codec<std::remove_cvref_t<decltype(v.m_vecKInventorySlotInfo)>>::get(r);
        return v;
    }
};

struct KEGS_SOCKET_EXPAND_ITEM_ACK {
    std::int32_t                    m_iOK{};  // int
    std::vector<KInventoryItemInfo> m_vecUpdatedInventorySlot{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
    }

    static KEGS_SOCKET_EXPAND_ITEM_ACK read_from(Reader& r) {
        KEGS_SOCKET_EXPAND_ITEM_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        return v;
    }
};

struct KEGS_SOCKET_ITEM_ACK {
    std::int32_t                    m_iOK{};  // int
    std::int32_t                    m_iED{};  // int
    std::int64_t                    m_iItemUID{};  // UidType
    std::vector<KInventoryItemInfo> m_vecInventorySlotInfo{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iED)>>::put(w, m_iED);
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_vecInventorySlotInfo)>>::put(w, m_vecInventorySlotInfo);
    }

    static KEGS_SOCKET_ITEM_ACK read_from(Reader& r) {
        KEGS_SOCKET_ITEM_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iED = Codec<std::remove_cvref_t<decltype(v.m_iED)>>::get(r);
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_vecInventorySlotInfo = Codec<std::remove_cvref_t<decltype(v.m_vecInventorySlotInfo)>>::get(r);
        return v;
    }
};

struct KEGS_START_WEDDING_NOT {
    std::int32_t                    m_iOK{};  // int
    std::vector<KInventoryItemInfo> m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
    }

    static KEGS_START_WEDDING_NOT read_from(Reader& r) {
        KEGS_START_WEDDING_NOT v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        return v;
    }
};

struct KEGS_TRADE_COMPLETE_NOT {
    std::vector<KInventoryItemInfo> m_vecInventorySlotInfo{};  // std::vector< KInventoryItemInfo >
    std::int32_t                    m_iED{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecInventorySlotInfo)>>::put(w, m_vecInventorySlotInfo);
        Codec<std::remove_cvref_t<decltype(m_iED)>>::put(w, m_iED);
    }

    static KEGS_TRADE_COMPLETE_NOT read_from(Reader& r) {
        KEGS_TRADE_COMPLETE_NOT v;
        v.m_vecInventorySlotInfo = Codec<std::remove_cvref_t<decltype(v.m_vecInventorySlotInfo)>>::get(r);
        v.m_iED = Codec<std::remove_cvref_t<decltype(v.m_iED)>>::get(r);
        return v;
    }
};

struct KEGS_UPDATE_INVENTORY_SLOT_INFO_NOT {
    std::vector<KInventoryItemInfo> m_vecInventorySlotInfo{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecInventorySlotInfo)>>::put(w, m_vecInventorySlotInfo);
    }

    static KEGS_UPDATE_INVENTORY_SLOT_INFO_NOT read_from(Reader& r) {
        KEGS_UPDATE_INVENTORY_SLOT_INFO_NOT v;
        v.m_vecInventorySlotInfo = Codec<std::remove_cvref_t<decltype(v.m_vecInventorySlotInfo)>>::get(r);
        return v;
    }
};

struct KEGS_USE_DEFENCE_DUNGEON_QUICK_SLOT_ACK {
    std::int32_t       m_iOK{};  // int
    KInventoryItemInfo m_InventorySlotInfo{};
    std::int32_t       m_ItemID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_InventorySlotInfo)>>::put(w, m_InventorySlotInfo);
        Codec<std::remove_cvref_t<decltype(m_ItemID)>>::put(w, m_ItemID);
    }

    static KEGS_USE_DEFENCE_DUNGEON_QUICK_SLOT_ACK read_from(Reader& r) {
        KEGS_USE_DEFENCE_DUNGEON_QUICK_SLOT_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_InventorySlotInfo = Codec<std::remove_cvref_t<decltype(v.m_InventorySlotInfo)>>::get(r);
        v.m_ItemID = Codec<std::remove_cvref_t<decltype(v.m_ItemID)>>::get(r);
        return v;
    }
};

struct KEGS_USE_FINALITY_SKILL_ACK {
    std::int32_t                    m_iOK{};  // int
    std::vector<KInventoryItemInfo> m_vecKInventorySlotInfo{};  // std::vector< KInventoryItemInfo >
    bool                            m_bNoConsume{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_vecKInventorySlotInfo)>>::put(w, m_vecKInventorySlotInfo);
        Codec<std::remove_cvref_t<decltype(m_bNoConsume)>>::put(w, m_bNoConsume);
    }

    static KEGS_USE_FINALITY_SKILL_ACK read_from(Reader& r) {
        KEGS_USE_FINALITY_SKILL_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_vecKInventorySlotInfo = Codec<std::remove_cvref_t<decltype(v.m_vecKInventorySlotInfo)>>::get(r);
        v.m_bNoConsume = Codec<std::remove_cvref_t<decltype(v.m_bNoConsume)>>::get(r);
        return v;
    }
};

struct KEGS_USE_ITEM_IN_INVENTORY_ACK {
    std::int32_t                    m_iOK{};  // int
    std::vector<KInventoryItemInfo> m_vecKInventorySlotInfo{};  // std::vector< KInventoryItemInfo >
    std::int32_t                    m_iUsedItemID{};  // int
    std::int32_t                    m_iWarpPointMapID{};  // int
    std::int32_t                    m_iED{};  // int
    std::int64_t                    m_iTempCode{};  // __int64

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_vecKInventorySlotInfo)>>::put(w, m_vecKInventorySlotInfo);
        Codec<std::remove_cvref_t<decltype(m_iUsedItemID)>>::put(w, m_iUsedItemID);
        Codec<std::remove_cvref_t<decltype(m_iWarpPointMapID)>>::put(w, m_iWarpPointMapID);
        Codec<std::remove_cvref_t<decltype(m_iED)>>::put(w, m_iED);
        Codec<std::remove_cvref_t<decltype(m_iTempCode)>>::put(w, m_iTempCode);
    }

    static KEGS_USE_ITEM_IN_INVENTORY_ACK read_from(Reader& r) {
        KEGS_USE_ITEM_IN_INVENTORY_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_vecKInventorySlotInfo = Codec<std::remove_cvref_t<decltype(v.m_vecKInventorySlotInfo)>>::get(r);
        v.m_iUsedItemID = Codec<std::remove_cvref_t<decltype(v.m_iUsedItemID)>>::get(r);
        v.m_iWarpPointMapID = Codec<std::remove_cvref_t<decltype(v.m_iWarpPointMapID)>>::get(r);
        v.m_iED = Codec<std::remove_cvref_t<decltype(v.m_iED)>>::get(r);
        v.m_iTempCode = Codec<std::remove_cvref_t<decltype(v.m_iTempCode)>>::get(r);
        return v;
    }
};

struct KEGS_USE_MEGAPHONE_ACK {
    std::int32_t                    m_iOK{};  // int
    std::vector<KInventoryItemInfo> m_vecInventorySlotInfo{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_vecInventorySlotInfo)>>::put(w, m_vecInventorySlotInfo);
    }

    static KEGS_USE_MEGAPHONE_ACK read_from(Reader& r) {
        KEGS_USE_MEGAPHONE_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_vecInventorySlotInfo = Codec<std::remove_cvref_t<decltype(v.m_vecInventorySlotInfo)>>::get(r);
        return v;
    }
};

struct KEGS_USE_QUICK_SLOT_ACK {
    std::int32_t       m_iOK{};  // int
    KInventoryItemInfo m_InventorySlotInfo{};
    std::int32_t       m_ItemID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_InventorySlotInfo)>>::put(w, m_InventorySlotInfo);
        Codec<std::remove_cvref_t<decltype(m_ItemID)>>::put(w, m_ItemID);
    }

    static KEGS_USE_QUICK_SLOT_ACK read_from(Reader& r) {
        KEGS_USE_QUICK_SLOT_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_InventorySlotInfo = Codec<std::remove_cvref_t<decltype(v.m_InventorySlotInfo)>>::get(r);
        v.m_ItemID = Codec<std::remove_cvref_t<decltype(v.m_ItemID)>>::get(r);
        return v;
    }
};

struct KEGS_WEDDING_PROPOSE_ACK {
    std::int32_t                    m_iOK{};  // int
    std::vector<KInventoryItemInfo> m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
    }

    static KEGS_WEDDING_PROPOSE_ACK read_from(Reader& r) {
        KEGS_WEDDING_PROPOSE_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        return v;
    }
};

struct KERM_CHANGE_EQUIPPED_ITEM_IN_FIELD_NOT {
    std::int64_t                    m_iUnitUID{};  // UidType
    std::vector<KInventoryItemInfo> m_vecInventorySlotInfo{};  // std::vector< KInventoryItemInfo >
    std::int32_t                    m_iEquippedTitleID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_vecInventorySlotInfo)>>::put(w, m_vecInventorySlotInfo);
        Codec<std::remove_cvref_t<decltype(m_iEquippedTitleID)>>::put(w, m_iEquippedTitleID);
    }

    static KERM_CHANGE_EQUIPPED_ITEM_IN_FIELD_NOT read_from(Reader& r) {
        KERM_CHANGE_EQUIPPED_ITEM_IN_FIELD_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_vecInventorySlotInfo = Codec<std::remove_cvref_t<decltype(v.m_vecInventorySlotInfo)>>::get(r);
        v.m_iEquippedTitleID = Codec<std::remove_cvref_t<decltype(v.m_iEquippedTitleID)>>::get(r);
        return v;
    }
};

struct KERM_CHANGE_EQUIPPED_ITEM_IN_SQUARE_NOT {
    std::int64_t                    m_UnitUID{};  // UidType
    std::vector<KInventoryItemInfo> m_vecInventorySlotInfo{};  // std::vector< KInventoryItemInfo >
    std::int32_t                    m_iEquippedTitleID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_UnitUID)>>::put(w, m_UnitUID);
        Codec<std::remove_cvref_t<decltype(m_vecInventorySlotInfo)>>::put(w, m_vecInventorySlotInfo);
        Codec<std::remove_cvref_t<decltype(m_iEquippedTitleID)>>::put(w, m_iEquippedTitleID);
    }

    static KERM_CHANGE_EQUIPPED_ITEM_IN_SQUARE_NOT read_from(Reader& r) {
        KERM_CHANGE_EQUIPPED_ITEM_IN_SQUARE_NOT v;
        v.m_UnitUID = Codec<std::remove_cvref_t<decltype(v.m_UnitUID)>>::get(r);
        v.m_vecInventorySlotInfo = Codec<std::remove_cvref_t<decltype(v.m_vecInventorySlotInfo)>>::get(r);
        v.m_iEquippedTitleID = Codec<std::remove_cvref_t<decltype(v.m_iEquippedTitleID)>>::get(r);
        return v;
    }
};

struct KERM_CONFIRM_BUY_PERSONAL_SHOP_ITEM_REQ {
    KInventoryItemInfo m_kSellItemInfo{};
    std::int32_t       m_iBuyQuantity{};  // int
    std::int64_t       m_iBuyUnitUID{};  // UidType
    std::int32_t       m_iTotalPrice{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kSellItemInfo)>>::put(w, m_kSellItemInfo);
        Codec<std::remove_cvref_t<decltype(m_iBuyQuantity)>>::put(w, m_iBuyQuantity);
        Codec<std::remove_cvref_t<decltype(m_iBuyUnitUID)>>::put(w, m_iBuyUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iTotalPrice)>>::put(w, m_iTotalPrice);
    }

    static KERM_CONFIRM_BUY_PERSONAL_SHOP_ITEM_REQ read_from(Reader& r) {
        KERM_CONFIRM_BUY_PERSONAL_SHOP_ITEM_REQ v;
        v.m_kSellItemInfo = Codec<std::remove_cvref_t<decltype(v.m_kSellItemInfo)>>::get(r);
        v.m_iBuyQuantity = Codec<std::remove_cvref_t<decltype(v.m_iBuyQuantity)>>::get(r);
        v.m_iBuyUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iBuyUnitUID)>>::get(r);
        v.m_iTotalPrice = Codec<std::remove_cvref_t<decltype(v.m_iTotalPrice)>>::get(r);
        return v;
    }
};

struct KERM_CONFIRM_TRADE_REQ {
    std::vector<KInventoryItemInfo> m_vecItemIn{};  // std::vector< KInventoryItemInfo >
    std::vector<KInventoryItemInfo> m_vecItemOut{};  // std::vector< KInventoryItemInfo >
    std::int32_t                    m_iEDIn{};  // int
    std::int32_t                    m_iEDOut{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecItemIn)>>::put(w, m_vecItemIn);
        Codec<std::remove_cvref_t<decltype(m_vecItemOut)>>::put(w, m_vecItemOut);
        Codec<std::remove_cvref_t<decltype(m_iEDIn)>>::put(w, m_iEDIn);
        Codec<std::remove_cvref_t<decltype(m_iEDOut)>>::put(w, m_iEDOut);
    }

    static KERM_CONFIRM_TRADE_REQ read_from(Reader& r) {
        KERM_CONFIRM_TRADE_REQ v;
        v.m_vecItemIn = Codec<std::remove_cvref_t<decltype(v.m_vecItemIn)>>::get(r);
        v.m_vecItemOut = Codec<std::remove_cvref_t<decltype(v.m_vecItemOut)>>::get(r);
        v.m_iEDIn = Codec<std::remove_cvref_t<decltype(v.m_iEDIn)>>::get(r);
        v.m_iEDOut = Codec<std::remove_cvref_t<decltype(v.m_iEDOut)>>::get(r);
        return v;
    }
};

struct KERM_UPDATE_TRADE_ITEM_REQ {
    KInventoryItemInfo m_kInventoryItemInfo{};
    bool               m_bDelete{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kInventoryItemInfo)>>::put(w, m_kInventoryItemInfo);
        Codec<std::remove_cvref_t<decltype(m_bDelete)>>::put(w, m_bDelete);
    }

    static KERM_UPDATE_TRADE_ITEM_REQ read_from(Reader& r) {
        KERM_UPDATE_TRADE_ITEM_REQ v;
        v.m_kInventoryItemInfo = Codec<std::remove_cvref_t<decltype(v.m_kInventoryItemInfo)>>::get(r);
        v.m_bDelete = Codec<std::remove_cvref_t<decltype(v.m_bDelete)>>::get(r);
        return v;
    }
};

struct KETR_CHANGE_EQUIPPED_ITEM_IN_FIELD_NOT {
    std::vector<std::int64_t>       m_vecUID{};  // std::vector< UidType >
    std::int64_t                    m_iUnitUID{};  // UidType
    std::vector<KInventoryItemInfo> m_vecInventorySlotInfo{};  // std::vector< KInventoryItemInfo >
    std::int32_t                    m_iEquippedTitleID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecUID)>>::put(w, m_vecUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_vecInventorySlotInfo)>>::put(w, m_vecInventorySlotInfo);
        Codec<std::remove_cvref_t<decltype(m_iEquippedTitleID)>>::put(w, m_iEquippedTitleID);
    }

    static KETR_CHANGE_EQUIPPED_ITEM_IN_FIELD_NOT read_from(Reader& r) {
        KETR_CHANGE_EQUIPPED_ITEM_IN_FIELD_NOT v;
        v.m_vecUID = Codec<std::remove_cvref_t<decltype(v.m_vecUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_vecInventorySlotInfo = Codec<std::remove_cvref_t<decltype(v.m_vecInventorySlotInfo)>>::get(r);
        v.m_iEquippedTitleID = Codec<std::remove_cvref_t<decltype(v.m_iEquippedTitleID)>>::get(r);
        return v;
    }
};

struct KItemPeriodInfo {
    std::int64_t   m_iItemUID{};  // UidType
    std::int16_t   m_sPeriod{};  // short
    std::u16string m_wstrExpirationDate{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_sPeriod)>>::put(w, m_sPeriod);
        Codec<std::remove_cvref_t<decltype(m_wstrExpirationDate)>>::put(w, m_wstrExpirationDate);
    }

    static KItemPeriodInfo read_from(Reader& r) {
        KItemPeriodInfo v;
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_sPeriod = Codec<std::remove_cvref_t<decltype(v.m_sPeriod)>>::get(r);
        v.m_wstrExpirationDate = Codec<std::remove_cvref_t<decltype(v.m_wstrExpirationDate)>>::get(r);
        return v;
    }
};

struct KDBE_RESET_PERIOD_ITEM_ACK {
    std::int32_t                 m_iOK{};  // int
    std::vector<KItemPeriodInfo> m_vecItemPeriodInfo{};  // std::vector< KItemPeriodInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_vecItemPeriodInfo)>>::put(w, m_vecItemPeriodInfo);
    }

    static KDBE_RESET_PERIOD_ITEM_ACK read_from(Reader& r) {
        KDBE_RESET_PERIOD_ITEM_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_vecItemPeriodInfo = Codec<std::remove_cvref_t<decltype(v.m_vecItemPeriodInfo)>>::get(r);
        return v;
    }
};

struct KItemPosition {
    std::int32_t m_iSlotCategory{};  // int
    std::int32_t m_iSlotID{};  // int
    std::int64_t m_iPetUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iSlotCategory)>>::put(w, m_iSlotCategory);
        Codec<std::remove_cvref_t<decltype(m_iSlotID)>>::put(w, m_iSlotID);
        Codec<std::remove_cvref_t<decltype(m_iPetUID)>>::put(w, m_iPetUID);
    }

    static KItemPosition read_from(Reader& r) {
        KItemPosition v;
        v.m_iSlotCategory = Codec<std::remove_cvref_t<decltype(v.m_iSlotCategory)>>::get(r);
        v.m_iSlotID = Codec<std::remove_cvref_t<decltype(v.m_iSlotID)>>::get(r);
        v.m_iPetUID = Codec<std::remove_cvref_t<decltype(v.m_iPetUID)>>::get(r);
        return v;
    }
};

struct KDBE_UPDATE_INVENTORY_ITEM_POS_NOT {
    std::int64_t                          m_iUnitUID{};  // UidType
    std::map<std::int64_t, KItemPosition> m_kItemPositionUpdate{};  // KItemPositionUpdate

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_kItemPositionUpdate)>>::put(w, m_kItemPositionUpdate);
    }

    static KDBE_UPDATE_INVENTORY_ITEM_POS_NOT read_from(Reader& r) {
        KDBE_UPDATE_INVENTORY_ITEM_POS_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_kItemPositionUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemPositionUpdate)>>::get(r);
        return v;
    }
};

struct KItemQuantityUpdate {
    std::map<std::int64_t, std::int32_t> m_mapQuantityChange{};  // std::map< UidType, int >
    std::vector<KDeletedItemInfo>        m_vecDeleted{};  // std::vector< KDeletedItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_mapQuantityChange)>>::put(w, m_mapQuantityChange);
        Codec<std::remove_cvref_t<decltype(m_vecDeleted)>>::put(w, m_vecDeleted);
    }

    static KItemQuantityUpdate read_from(Reader& r) {
        KItemQuantityUpdate v;
        v.m_mapQuantityChange = Codec<std::remove_cvref_t<decltype(v.m_mapQuantityChange)>>::get(r);
        v.m_vecDeleted = Codec<std::remove_cvref_t<decltype(v.m_vecDeleted)>>::get(r);
        return v;
    }
};

struct KDBE_ATTRIB_ATTACH_ITEM_ACK {
    std::int32_t                    m_iOK{};  // int
    std::int64_t                    m_iItemUID{};  // UidType
    KItemAttributeEnchantInfo       m_kAttribEnchantInfo{};
    KItemQuantityUpdate             m_kItemQuantityUpdate{};
    std::vector<KInventoryItemInfo> m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_kAttribEnchantInfo)>>::put(w, m_kAttribEnchantInfo);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
    }

    static KDBE_ATTRIB_ATTACH_ITEM_ACK read_from(Reader& r) {
        KDBE_ATTRIB_ATTACH_ITEM_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_kAttribEnchantInfo = Codec<std::remove_cvref_t<decltype(v.m_kAttribEnchantInfo)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        return v;
    }
};

struct KDBE_ATTRIB_ATTACH_ITEM_REQ {
    std::int64_t                    m_iUnitUID{};  // UidType
    std::int64_t                    m_iItemUID{};  // UidType
    KItemAttributeEnchantInfo       m_kAttribEnchantInfo{};
    KItemQuantityUpdate             m_kItemQuantityUpdate{};
    std::vector<KInventoryItemInfo> m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_kAttribEnchantInfo)>>::put(w, m_kAttribEnchantInfo);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
    }

    static KDBE_ATTRIB_ATTACH_ITEM_REQ read_from(Reader& r) {
        KDBE_ATTRIB_ATTACH_ITEM_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_kAttribEnchantInfo = Codec<std::remove_cvref_t<decltype(v.m_kAttribEnchantInfo)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        return v;
    }
};

struct KDBE_ATTRIB_ENCHANT_ITEM_ACK {
    std::int32_t                    m_iOK{};  // int
    std::int64_t                    m_iItemUID{};  // UidType
    std::int32_t                    m_iItemID{};  // int
    char                            m_cAttribEnchantSlotNo{};
    char                            m_cAttribEnchantID{};
    KItemQuantityUpdate             m_kItemQuantityUpdate{};
    std::vector<KInventoryItemInfo> m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_iItemID)>>::put(w, m_iItemID);
        Codec<std::remove_cvref_t<decltype(m_cAttribEnchantSlotNo)>>::put(w, m_cAttribEnchantSlotNo);
        Codec<std::remove_cvref_t<decltype(m_cAttribEnchantID)>>::put(w, m_cAttribEnchantID);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
    }

    static KDBE_ATTRIB_ENCHANT_ITEM_ACK read_from(Reader& r) {
        KDBE_ATTRIB_ENCHANT_ITEM_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_iItemID = Codec<std::remove_cvref_t<decltype(v.m_iItemID)>>::get(r);
        v.m_cAttribEnchantSlotNo = Codec<std::remove_cvref_t<decltype(v.m_cAttribEnchantSlotNo)>>::get(r);
        v.m_cAttribEnchantID = Codec<std::remove_cvref_t<decltype(v.m_cAttribEnchantID)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        return v;
    }
};

struct KDBE_ATTRIB_ENCHANT_ITEM_REQ {
    std::int64_t                    m_iUnitUID{};  // UidType
    std::int64_t                    m_iItemUID{};  // UidType
    std::int32_t                    m_iItemID{};  // int
    char                            m_cAttribEnchantSlotNo{};
    char                            m_cAttribEnchantID{};
    KItemQuantityUpdate             m_kItemQuantityUpdate{};
    std::vector<KInventoryItemInfo> m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_iItemID)>>::put(w, m_iItemID);
        Codec<std::remove_cvref_t<decltype(m_cAttribEnchantSlotNo)>>::put(w, m_cAttribEnchantSlotNo);
        Codec<std::remove_cvref_t<decltype(m_cAttribEnchantID)>>::put(w, m_cAttribEnchantID);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
    }

    static KDBE_ATTRIB_ENCHANT_ITEM_REQ read_from(Reader& r) {
        KDBE_ATTRIB_ENCHANT_ITEM_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_iItemID = Codec<std::remove_cvref_t<decltype(v.m_iItemID)>>::get(r);
        v.m_cAttribEnchantSlotNo = Codec<std::remove_cvref_t<decltype(v.m_cAttribEnchantSlotNo)>>::get(r);
        v.m_cAttribEnchantID = Codec<std::remove_cvref_t<decltype(v.m_cAttribEnchantID)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        return v;
    }
};

struct KDBE_BUY_ED_ITEM_ACK {
    std::int32_t                         m_iED{};  // int
    std::int32_t                         m_iAPoint{};  // int
    KItemQuantityUpdate                  m_kItemQuantityUpdate{};
    std::map<std::int64_t, std::int32_t> m_kItemEnduranceUpdate{};  // std::map< UidType, int >
    std::vector<KInventoryItemInfo>      m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >
    std::map<std::int64_t, KItemInfo>    m_mapItemInfo{};  // std::map< UidType, KItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iED)>>::put(w, m_iED);
        Codec<std::remove_cvref_t<decltype(m_iAPoint)>>::put(w, m_iAPoint);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_kItemEnduranceUpdate)>>::put(w, m_kItemEnduranceUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
        Codec<std::remove_cvref_t<decltype(m_mapItemInfo)>>::put(w, m_mapItemInfo);
    }

    static KDBE_BUY_ED_ITEM_ACK read_from(Reader& r) {
        KDBE_BUY_ED_ITEM_ACK v;
        v.m_iED = Codec<std::remove_cvref_t<decltype(v.m_iED)>>::get(r);
        v.m_iAPoint = Codec<std::remove_cvref_t<decltype(v.m_iAPoint)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_kItemEnduranceUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemEnduranceUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        v.m_mapItemInfo = Codec<std::remove_cvref_t<decltype(v.m_mapItemInfo)>>::get(r);
        return v;
    }
};

struct KDBE_BUY_ED_ITEM_REQ {
    std::int64_t                         m_iUnitUID{};  // UidType
    KItemQuantityUpdate                  m_kItemQuantityUpdate{};
    std::map<std::int64_t, std::int32_t> m_kItemEnduranceUpdate{};  // std::map< UidType, int >
    std::vector<KInventoryItemInfo>      m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >
    std::vector<KBuyGPItemInfo>          m_vecBuyGPItemInfo{};  // std::vector< KBuyGPItemInfo >
    bool                                 m_bCheat{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_kItemEnduranceUpdate)>>::put(w, m_kItemEnduranceUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
        Codec<std::remove_cvref_t<decltype(m_vecBuyGPItemInfo)>>::put(w, m_vecBuyGPItemInfo);
        Codec<std::remove_cvref_t<decltype(m_bCheat)>>::put(w, m_bCheat);
    }

    static KDBE_BUY_ED_ITEM_REQ read_from(Reader& r) {
        KDBE_BUY_ED_ITEM_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_kItemEnduranceUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemEnduranceUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        v.m_vecBuyGPItemInfo = Codec<std::remove_cvref_t<decltype(v.m_vecBuyGPItemInfo)>>::get(r);
        v.m_bCheat = Codec<std::remove_cvref_t<decltype(v.m_bCheat)>>::get(r);
        return v;
    }
};

struct KDBE_BUY_PSHOP_AGENCY_ITEM_REQ {
    std::int32_t           m_iOK{};  // int
    std::int64_t           m_iHostUnitUID{};  // UidType
    std::int32_t           m_iSellUnitEDIn{};  // int
    KInventoryItemInfo     m_kSellItemDBUpdate{};
    std::int64_t           m_iPriceGSUID{};  // UidType
    std::int64_t           m_iPriceUnitUID{};  // UidType
    std::int32_t           m_iPriceUnitEDOUT{};  // int
    std::int32_t           m_iPriceCommissionED{};  // int
    KInventoryItemInfo     m_kPriceIntemInfoIN{};
    KItemQuantityUpdate    m_kItemQuantityUpdateTemp{};
    std::vector<KItemInfo> m_vecItemInfoTemp{};  // std::vector< KItemInfo >
    std::int32_t           m_iEDTemp{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iHostUnitUID)>>::put(w, m_iHostUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iSellUnitEDIn)>>::put(w, m_iSellUnitEDIn);
        Codec<std::remove_cvref_t<decltype(m_kSellItemDBUpdate)>>::put(w, m_kSellItemDBUpdate);
        Codec<std::remove_cvref_t<decltype(m_iPriceGSUID)>>::put(w, m_iPriceGSUID);
        Codec<std::remove_cvref_t<decltype(m_iPriceUnitUID)>>::put(w, m_iPriceUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iPriceUnitEDOUT)>>::put(w, m_iPriceUnitEDOUT);
        Codec<std::remove_cvref_t<decltype(m_iPriceCommissionED)>>::put(w, m_iPriceCommissionED);
        Codec<std::remove_cvref_t<decltype(m_kPriceIntemInfoIN)>>::put(w, m_kPriceIntemInfoIN);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdateTemp)>>::put(w, m_kItemQuantityUpdateTemp);
        Codec<std::remove_cvref_t<decltype(m_vecItemInfoTemp)>>::put(w, m_vecItemInfoTemp);
        Codec<std::remove_cvref_t<decltype(m_iEDTemp)>>::put(w, m_iEDTemp);
    }

    static KDBE_BUY_PSHOP_AGENCY_ITEM_REQ read_from(Reader& r) {
        KDBE_BUY_PSHOP_AGENCY_ITEM_REQ v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iHostUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iHostUnitUID)>>::get(r);
        v.m_iSellUnitEDIn = Codec<std::remove_cvref_t<decltype(v.m_iSellUnitEDIn)>>::get(r);
        v.m_kSellItemDBUpdate = Codec<std::remove_cvref_t<decltype(v.m_kSellItemDBUpdate)>>::get(r);
        v.m_iPriceGSUID = Codec<std::remove_cvref_t<decltype(v.m_iPriceGSUID)>>::get(r);
        v.m_iPriceUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iPriceUnitUID)>>::get(r);
        v.m_iPriceUnitEDOUT = Codec<std::remove_cvref_t<decltype(v.m_iPriceUnitEDOUT)>>::get(r);
        v.m_iPriceCommissionED = Codec<std::remove_cvref_t<decltype(v.m_iPriceCommissionED)>>::get(r);
        v.m_kPriceIntemInfoIN = Codec<std::remove_cvref_t<decltype(v.m_kPriceIntemInfoIN)>>::get(r);
        v.m_kItemQuantityUpdateTemp = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdateTemp)>>::get(r);
        v.m_vecItemInfoTemp = Codec<std::remove_cvref_t<decltype(v.m_vecItemInfoTemp)>>::get(r);
        v.m_iEDTemp = Codec<std::remove_cvref_t<decltype(v.m_iEDTemp)>>::get(r);
        return v;
    }
};

struct KDBE_CHANGE_INVENTORY_SLOT_ITEM_ACK {
    char                                  m_cFromSlotType{};
    std::int32_t                          m_iFromSlotID{};  // int
    char                                  m_cToSlotType{};
    std::int32_t                          m_iToSlotID{};  // int
    KItemQuantityUpdate                   m_kItemQuantityUpdate{};
    std::map<std::int64_t, std::int32_t>  m_kItemEnduranceUpdate{};  // KItemEnduranceUpdate
    std::map<std::int64_t, KItemPosition> m_kItemPositionUpdate{};  // KItemPositionUpdate

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cFromSlotType)>>::put(w, m_cFromSlotType);
        Codec<std::remove_cvref_t<decltype(m_iFromSlotID)>>::put(w, m_iFromSlotID);
        Codec<std::remove_cvref_t<decltype(m_cToSlotType)>>::put(w, m_cToSlotType);
        Codec<std::remove_cvref_t<decltype(m_iToSlotID)>>::put(w, m_iToSlotID);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_kItemEnduranceUpdate)>>::put(w, m_kItemEnduranceUpdate);
        Codec<std::remove_cvref_t<decltype(m_kItemPositionUpdate)>>::put(w, m_kItemPositionUpdate);
    }

    static KDBE_CHANGE_INVENTORY_SLOT_ITEM_ACK read_from(Reader& r) {
        KDBE_CHANGE_INVENTORY_SLOT_ITEM_ACK v;
        v.m_cFromSlotType = Codec<std::remove_cvref_t<decltype(v.m_cFromSlotType)>>::get(r);
        v.m_iFromSlotID = Codec<std::remove_cvref_t<decltype(v.m_iFromSlotID)>>::get(r);
        v.m_cToSlotType = Codec<std::remove_cvref_t<decltype(v.m_cToSlotType)>>::get(r);
        v.m_iToSlotID = Codec<std::remove_cvref_t<decltype(v.m_iToSlotID)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_kItemEnduranceUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemEnduranceUpdate)>>::get(r);
        v.m_kItemPositionUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemPositionUpdate)>>::get(r);
        return v;
    }
};

struct KDBE_CHANGE_INVENTORY_SLOT_ITEM_REQ {
    char                                  m_cFromSlotType{};
    std::int32_t                          m_iFromSlotID{};  // int
    char                                  m_cToSlotType{};
    std::int32_t                          m_iToSlotID{};  // int
    std::int64_t                          m_iUnitUID{};  // UidType
    KItemQuantityUpdate                   m_kItemQuantityUpdate{};
    std::map<std::int64_t, std::int32_t>  m_kItemEnduranceUpdate{};  // KItemEnduranceUpdate
    std::map<std::int64_t, KItemPosition> m_kItemPositionUpdate{};  // KItemPositionUpdate

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cFromSlotType)>>::put(w, m_cFromSlotType);
        Codec<std::remove_cvref_t<decltype(m_iFromSlotID)>>::put(w, m_iFromSlotID);
        Codec<std::remove_cvref_t<decltype(m_cToSlotType)>>::put(w, m_cToSlotType);
        Codec<std::remove_cvref_t<decltype(m_iToSlotID)>>::put(w, m_iToSlotID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_kItemEnduranceUpdate)>>::put(w, m_kItemEnduranceUpdate);
        Codec<std::remove_cvref_t<decltype(m_kItemPositionUpdate)>>::put(w, m_kItemPositionUpdate);
    }

    static KDBE_CHANGE_INVENTORY_SLOT_ITEM_REQ read_from(Reader& r) {
        KDBE_CHANGE_INVENTORY_SLOT_ITEM_REQ v;
        v.m_cFromSlotType = Codec<std::remove_cvref_t<decltype(v.m_cFromSlotType)>>::get(r);
        v.m_iFromSlotID = Codec<std::remove_cvref_t<decltype(v.m_iFromSlotID)>>::get(r);
        v.m_cToSlotType = Codec<std::remove_cvref_t<decltype(v.m_cToSlotType)>>::get(r);
        v.m_iToSlotID = Codec<std::remove_cvref_t<decltype(v.m_iToSlotID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_kItemEnduranceUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemEnduranceUpdate)>>::get(r);
        v.m_kItemPositionUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemPositionUpdate)>>::get(r);
        return v;
    }
};

struct KDBE_CHANGE_LOVE_WORD_ACK {
    std::int32_t                    m_iOK{};  // int
    std::int64_t                    m_iUseUnitUID{};  // UidType
    std::int64_t                    m_iLoverUnitUID{};  // UidType
    std::u16string                  m_wstrNewLoveWord{};  // std::wstring
    KItemQuantityUpdate             m_kItemQuantityUpdate{};
    std::vector<KInventoryItemInfo> m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iUseUnitUID)>>::put(w, m_iUseUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iLoverUnitUID)>>::put(w, m_iLoverUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrNewLoveWord)>>::put(w, m_wstrNewLoveWord);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
    }

    static KDBE_CHANGE_LOVE_WORD_ACK read_from(Reader& r) {
        KDBE_CHANGE_LOVE_WORD_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iUseUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUseUnitUID)>>::get(r);
        v.m_iLoverUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iLoverUnitUID)>>::get(r);
        v.m_wstrNewLoveWord = Codec<std::remove_cvref_t<decltype(v.m_wstrNewLoveWord)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        return v;
    }
};

struct KDBE_CHANGE_LOVE_WORD_REQ {
    std::int64_t                    m_iUseUnitUID{};  // UidType
    std::int64_t                    m_iLoverUnitUID{};  // UidType
    std::u16string                  m_wstrNewLoveWord{};  // std::wstring
    std::vector<KInventoryItemInfo> m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >
    KItemQuantityUpdate             m_kItemQuantityUpdate{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUseUnitUID)>>::put(w, m_iUseUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iLoverUnitUID)>>::put(w, m_iLoverUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrNewLoveWord)>>::put(w, m_wstrNewLoveWord);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
    }

    static KDBE_CHANGE_LOVE_WORD_REQ read_from(Reader& r) {
        KDBE_CHANGE_LOVE_WORD_REQ v;
        v.m_iUseUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUseUnitUID)>>::get(r);
        v.m_iLoverUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iLoverUnitUID)>>::get(r);
        v.m_wstrNewLoveWord = Codec<std::remove_cvref_t<decltype(v.m_wstrNewLoveWord)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        return v;
    }
};

struct KDBE_CHECK_TIME_EVENT_COMPLETE_ACK {
    std::int32_t                      m_iOK{};  // int
    std::int32_t                      m_iRewardCount{};  // int
    std::vector<std::int32_t>         m_vecRestartEvent{};  // std::vector< int >
    KItemQuantityUpdate               m_kItemQuantityUpdate{};
    std::vector<KInventoryItemInfo>   m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >
    std::map<std::int64_t, KItemInfo> m_mapItemInfo{};  // std::map< UidType, KItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iRewardCount)>>::put(w, m_iRewardCount);
        Codec<std::remove_cvref_t<decltype(m_vecRestartEvent)>>::put(w, m_vecRestartEvent);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
        Codec<std::remove_cvref_t<decltype(m_mapItemInfo)>>::put(w, m_mapItemInfo);
    }

    static KDBE_CHECK_TIME_EVENT_COMPLETE_ACK read_from(Reader& r) {
        KDBE_CHECK_TIME_EVENT_COMPLETE_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iRewardCount = Codec<std::remove_cvref_t<decltype(v.m_iRewardCount)>>::get(r);
        v.m_vecRestartEvent = Codec<std::remove_cvref_t<decltype(v.m_vecRestartEvent)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        v.m_mapItemInfo = Codec<std::remove_cvref_t<decltype(v.m_mapItemInfo)>>::get(r);
        return v;
    }
};

struct KDBE_CHECK_TIME_EVENT_COMPLETE_REQ {
    std::int64_t                          m_iUnitUID{};  // UidType
    std::vector<KCumulativeTimeEventInfo> m_vecCompletedEvent{};  // std::vector< KCumulativeTimeEventInfo >
    std::map<std::int32_t, std::int32_t>  m_mapInsertedItem{};  // std::map< int, int >
    KItemQuantityUpdate                   m_kItemQuantityUpdate{};
    std::vector<KInventoryItemInfo>       m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >
    std::vector<KItemInfo>                m_vecItemInfo{};  // std::vector< KItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_vecCompletedEvent)>>::put(w, m_vecCompletedEvent);
        Codec<std::remove_cvref_t<decltype(m_mapInsertedItem)>>::put(w, m_mapInsertedItem);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
        Codec<std::remove_cvref_t<decltype(m_vecItemInfo)>>::put(w, m_vecItemInfo);
    }

    static KDBE_CHECK_TIME_EVENT_COMPLETE_REQ read_from(Reader& r) {
        KDBE_CHECK_TIME_EVENT_COMPLETE_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_vecCompletedEvent = Codec<std::remove_cvref_t<decltype(v.m_vecCompletedEvent)>>::get(r);
        v.m_mapInsertedItem = Codec<std::remove_cvref_t<decltype(v.m_mapInsertedItem)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        v.m_vecItemInfo = Codec<std::remove_cvref_t<decltype(v.m_vecItemInfo)>>::get(r);
        return v;
    }
};

struct KDBE_COUPLE_MAKING_SUCCESS_ACK {
    std::int32_t                    m_iOK{};  // int
    std::int64_t                    m_iRequestUnitUID{};  // UidType
    std::u16string                  m_wstrRequestUnitName{};  // std::wstring
    std::int64_t                    m_iAcceptUserUID{};  // UidType
    std::uint8_t                    m_ucAcceptUnitLevel{};  // u_char
    char                            m_cAcceptUnitClass{};
    std::int64_t                    m_iAcceptUnitUID{};  // UidType
    std::u16string                  m_wstrAcceptUnitName{};  // std::wstring
    KItemQuantityUpdate             m_kItemQuantityUpdate{};
    std::vector<KInventoryItemInfo> m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >
    std::int64_t                    m_tDate{};  // __int64

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iRequestUnitUID)>>::put(w, m_iRequestUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrRequestUnitName)>>::put(w, m_wstrRequestUnitName);
        Codec<std::remove_cvref_t<decltype(m_iAcceptUserUID)>>::put(w, m_iAcceptUserUID);
        Codec<std::remove_cvref_t<decltype(m_ucAcceptUnitLevel)>>::put(w, m_ucAcceptUnitLevel);
        Codec<std::remove_cvref_t<decltype(m_cAcceptUnitClass)>>::put(w, m_cAcceptUnitClass);
        Codec<std::remove_cvref_t<decltype(m_iAcceptUnitUID)>>::put(w, m_iAcceptUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrAcceptUnitName)>>::put(w, m_wstrAcceptUnitName);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
        Codec<std::remove_cvref_t<decltype(m_tDate)>>::put(w, m_tDate);
    }

    static KDBE_COUPLE_MAKING_SUCCESS_ACK read_from(Reader& r) {
        KDBE_COUPLE_MAKING_SUCCESS_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iRequestUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iRequestUnitUID)>>::get(r);
        v.m_wstrRequestUnitName = Codec<std::remove_cvref_t<decltype(v.m_wstrRequestUnitName)>>::get(r);
        v.m_iAcceptUserUID = Codec<std::remove_cvref_t<decltype(v.m_iAcceptUserUID)>>::get(r);
        v.m_ucAcceptUnitLevel = Codec<std::remove_cvref_t<decltype(v.m_ucAcceptUnitLevel)>>::get(r);
        v.m_cAcceptUnitClass = Codec<std::remove_cvref_t<decltype(v.m_cAcceptUnitClass)>>::get(r);
        v.m_iAcceptUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iAcceptUnitUID)>>::get(r);
        v.m_wstrAcceptUnitName = Codec<std::remove_cvref_t<decltype(v.m_wstrAcceptUnitName)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        v.m_tDate = Codec<std::remove_cvref_t<decltype(v.m_tDate)>>::get(r);
        return v;
    }
};

struct KDBE_CREATE_RIDING_PET_REQ {
    std::int64_t                    m_iUnitUID{};  // UidType
    std::uint16_t                   m_usCreatePetID{};  // USHORT
    std::int16_t                    m_sPeriod{};  // short
    KItemQuantityUpdate             m_kItemQuantityUpdate{};
    std::vector<KInventoryItemInfo> m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_usCreatePetID)>>::put(w, m_usCreatePetID);
        Codec<std::remove_cvref_t<decltype(m_sPeriod)>>::put(w, m_sPeriod);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
    }

    static KDBE_CREATE_RIDING_PET_REQ read_from(Reader& r) {
        KDBE_CREATE_RIDING_PET_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_usCreatePetID = Codec<std::remove_cvref_t<decltype(v.m_usCreatePetID)>>::get(r);
        v.m_sPeriod = Codec<std::remove_cvref_t<decltype(v.m_sPeriod)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        return v;
    }
};

struct KDBE_ENCHANT_ATTACH_ITEM_ACK {
    std::int32_t                    m_iOK{};  // int
    std::int64_t                    m_iItemUID{};  // UidType
    std::int32_t                    m_iLevelAfterEnchant{};  // int
    KItemQuantityUpdate             m_kItemQuantityUpdate{};
    std::vector<KInventoryItemInfo> m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_iLevelAfterEnchant)>>::put(w, m_iLevelAfterEnchant);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
    }

    static KDBE_ENCHANT_ATTACH_ITEM_ACK read_from(Reader& r) {
        KDBE_ENCHANT_ATTACH_ITEM_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_iLevelAfterEnchant = Codec<std::remove_cvref_t<decltype(v.m_iLevelAfterEnchant)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        return v;
    }
};

struct KDBE_ENCHANT_ATTACH_ITEM_REQ {
    std::int64_t                    m_iUnitUID{};  // UidType
    std::int64_t                    m_iItemUID{};  // UidType
    std::int32_t                    m_iLevelAfterEnchant{};  // int
    KItemQuantityUpdate             m_kItemQuantityUpdate{};
    std::vector<KInventoryItemInfo> m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_iLevelAfterEnchant)>>::put(w, m_iLevelAfterEnchant);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
    }

    static KDBE_ENCHANT_ATTACH_ITEM_REQ read_from(Reader& r) {
        KDBE_ENCHANT_ATTACH_ITEM_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_iLevelAfterEnchant = Codec<std::remove_cvref_t<decltype(v.m_iLevelAfterEnchant)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        return v;
    }
};

struct KDBE_ENCHANT_ITEM_ACK {
    std::int32_t                    m_iOK{};  // int
    std::int32_t                    m_iEnchantResult{};  // int
    std::int64_t                    m_iItemUID{};  // UidType
    std::int32_t                    m_iItemID{};  // int
    std::int32_t                    m_iLevelAfterEnchant{};  // int
    KItemQuantityUpdate             m_kItemQuantityUpdate{};
    std::vector<KInventoryItemInfo> m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iEnchantResult)>>::put(w, m_iEnchantResult);
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_iItemID)>>::put(w, m_iItemID);
        Codec<std::remove_cvref_t<decltype(m_iLevelAfterEnchant)>>::put(w, m_iLevelAfterEnchant);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
    }

    static KDBE_ENCHANT_ITEM_ACK read_from(Reader& r) {
        KDBE_ENCHANT_ITEM_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iEnchantResult = Codec<std::remove_cvref_t<decltype(v.m_iEnchantResult)>>::get(r);
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_iItemID = Codec<std::remove_cvref_t<decltype(v.m_iItemID)>>::get(r);
        v.m_iLevelAfterEnchant = Codec<std::remove_cvref_t<decltype(v.m_iLevelAfterEnchant)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        return v;
    }
};

struct KDBE_ENCHANT_ITEM_REQ {
    std::int64_t                    m_iUnitUID{};  // UidType
    std::int32_t                    m_iEnchantResult{};  // int
    std::int64_t                    m_iItemUID{};  // UidType
    std::int32_t                    m_iItemID{};  // int
    std::int32_t                    m_iLevelAfterEnchant{};  // int
    KItemQuantityUpdate             m_kItemQuantityUpdate{};
    std::vector<KInventoryItemInfo> m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iEnchantResult)>>::put(w, m_iEnchantResult);
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_iItemID)>>::put(w, m_iItemID);
        Codec<std::remove_cvref_t<decltype(m_iLevelAfterEnchant)>>::put(w, m_iLevelAfterEnchant);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
    }

    static KDBE_ENCHANT_ITEM_REQ read_from(Reader& r) {
        KDBE_ENCHANT_ITEM_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iEnchantResult = Codec<std::remove_cvref_t<decltype(v.m_iEnchantResult)>>::get(r);
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_iItemID = Codec<std::remove_cvref_t<decltype(v.m_iItemID)>>::get(r);
        v.m_iLevelAfterEnchant = Codec<std::remove_cvref_t<decltype(v.m_iLevelAfterEnchant)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        return v;
    }
};

struct KDBE_GET_ITEM_INSERT_TO_INVENTORY_ACK {
    std::int32_t                         m_iOK{};  // int
    std::map<std::int32_t, std::int32_t> m_mapInsertItem{};  // std::map< int, int >
    KItemQuantityUpdate                  m_kItemQuantityUpdate{};
    std::vector<KInventoryItemInfo>      m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >
    std::map<std::int64_t, KItemInfo>    m_mapItemInfo{};  // std::map< UidType, KItemInfo >
    KEGS_GET_ITEM_NOT                    m_kBroadInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_mapInsertItem)>>::put(w, m_mapInsertItem);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
        Codec<std::remove_cvref_t<decltype(m_mapItemInfo)>>::put(w, m_mapItemInfo);
        Codec<std::remove_cvref_t<decltype(m_kBroadInfo)>>::put(w, m_kBroadInfo);
    }

    static KDBE_GET_ITEM_INSERT_TO_INVENTORY_ACK read_from(Reader& r) {
        KDBE_GET_ITEM_INSERT_TO_INVENTORY_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_mapInsertItem = Codec<std::remove_cvref_t<decltype(v.m_mapInsertItem)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        v.m_mapItemInfo = Codec<std::remove_cvref_t<decltype(v.m_mapItemInfo)>>::get(r);
        v.m_kBroadInfo = Codec<std::remove_cvref_t<decltype(v.m_kBroadInfo)>>::get(r);
        return v;
    }
};

struct KDBE_GET_ITEM_INSERT_TO_INVENTORY_REQ {
    std::int64_t                         m_iUnitUID{};  // UidType
    std::map<std::int32_t, std::int32_t> m_mapInsertItem{};  // std::map< int, int >
    KItemQuantityUpdate                  m_kItemQuantityUpdate{};
    std::vector<KInventoryItemInfo>      m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >
    std::vector<KItemInfo>               m_vecItemInfo{};  // std::vector< KItemInfo >
    KEGS_GET_ITEM_NOT                    m_kBroadInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_mapInsertItem)>>::put(w, m_mapInsertItem);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
        Codec<std::remove_cvref_t<decltype(m_vecItemInfo)>>::put(w, m_vecItemInfo);
        Codec<std::remove_cvref_t<decltype(m_kBroadInfo)>>::put(w, m_kBroadInfo);
    }

    static KDBE_GET_ITEM_INSERT_TO_INVENTORY_REQ read_from(Reader& r) {
        KDBE_GET_ITEM_INSERT_TO_INVENTORY_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_mapInsertItem = Codec<std::remove_cvref_t<decltype(v.m_mapInsertItem)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        v.m_vecItemInfo = Codec<std::remove_cvref_t<decltype(v.m_vecItemInfo)>>::get(r);
        v.m_kBroadInfo = Codec<std::remove_cvref_t<decltype(v.m_kBroadInfo)>>::get(r);
        return v;
    }
};

struct KDBE_GET_TEMP_ITEM_ACK {
    std::int32_t                         m_iOK{};  // int
    std::int32_t                         m_iTempItemUID{};  // int
    std::map<std::int32_t, std::int32_t> m_mapInsertItem{};  // std::map< int, int >
    KItemQuantityUpdate                  m_kItemQuantityUpdate{};
    std::vector<KInventoryItemInfo>      m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >
    std::map<std::int64_t, KItemInfo>    m_mapItemInfo{};  // std::map< UidType, KItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iTempItemUID)>>::put(w, m_iTempItemUID);
        Codec<std::remove_cvref_t<decltype(m_mapInsertItem)>>::put(w, m_mapInsertItem);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
        Codec<std::remove_cvref_t<decltype(m_mapItemInfo)>>::put(w, m_mapItemInfo);
    }

    static KDBE_GET_TEMP_ITEM_ACK read_from(Reader& r) {
        KDBE_GET_TEMP_ITEM_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iTempItemUID = Codec<std::remove_cvref_t<decltype(v.m_iTempItemUID)>>::get(r);
        v.m_mapInsertItem = Codec<std::remove_cvref_t<decltype(v.m_mapInsertItem)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        v.m_mapItemInfo = Codec<std::remove_cvref_t<decltype(v.m_mapItemInfo)>>::get(r);
        return v;
    }
};

struct KDBE_GET_TEMP_ITEM_REQ {
    std::int64_t                         m_iUnitUID{};  // UidType
    std::int32_t                         m_iTempItemUID{};  // int
    std::map<std::int32_t, std::int32_t> m_mapInsertItem{};  // std::map< int, int >
    KItemQuantityUpdate                  m_kItemQuantityUpdate{};
    std::vector<KInventoryItemInfo>      m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >
    std::vector<KItemInfo>               m_vecItemInfo{};  // std::vector< KItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iTempItemUID)>>::put(w, m_iTempItemUID);
        Codec<std::remove_cvref_t<decltype(m_mapInsertItem)>>::put(w, m_mapInsertItem);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
        Codec<std::remove_cvref_t<decltype(m_vecItemInfo)>>::put(w, m_vecItemInfo);
    }

    static KDBE_GET_TEMP_ITEM_REQ read_from(Reader& r) {
        KDBE_GET_TEMP_ITEM_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iTempItemUID = Codec<std::remove_cvref_t<decltype(v.m_iTempItemUID)>>::get(r);
        v.m_mapInsertItem = Codec<std::remove_cvref_t<decltype(v.m_mapInsertItem)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        v.m_vecItemInfo = Codec<std::remove_cvref_t<decltype(v.m_vecItemInfo)>>::get(r);
        return v;
    }
};

struct KDBE_INIT_GUILD_SKILL_ACK {
    std::int32_t                    m_iOK{};  // int
    std::int32_t                    m_iGuildUID{};  // int
    std::int32_t                    m_iGuildSPoint{};  // int
    std::int32_t                    m_iGuildCSPoint{};  // int
    KItemQuantityUpdate             m_kItemQuantityUpdate{};
    std::vector<KInventoryItemInfo> m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iGuildUID)>>::put(w, m_iGuildUID);
        Codec<std::remove_cvref_t<decltype(m_iGuildSPoint)>>::put(w, m_iGuildSPoint);
        Codec<std::remove_cvref_t<decltype(m_iGuildCSPoint)>>::put(w, m_iGuildCSPoint);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
    }

    static KDBE_INIT_GUILD_SKILL_ACK read_from(Reader& r) {
        KDBE_INIT_GUILD_SKILL_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iGuildUID = Codec<std::remove_cvref_t<decltype(v.m_iGuildUID)>>::get(r);
        v.m_iGuildSPoint = Codec<std::remove_cvref_t<decltype(v.m_iGuildSPoint)>>::get(r);
        v.m_iGuildCSPoint = Codec<std::remove_cvref_t<decltype(v.m_iGuildCSPoint)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        return v;
    }
};

struct KDBE_INIT_GUILD_SKILL_REQ {
    std::int32_t                    m_iGuildUID{};  // int
    std::int64_t                    m_iUnitUID{};  // UidType
    std::int64_t                    m_iItemUID{};  // UidType
    std::int32_t                    m_iGuildSPoint{};  // int
    std::int32_t                    m_iGuildCSPoint{};  // int
    KItemQuantityUpdate             m_kItemQuantityUpdate{};
    std::vector<KInventoryItemInfo> m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iGuildUID)>>::put(w, m_iGuildUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_iGuildSPoint)>>::put(w, m_iGuildSPoint);
        Codec<std::remove_cvref_t<decltype(m_iGuildCSPoint)>>::put(w, m_iGuildCSPoint);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
    }

    static KDBE_INIT_GUILD_SKILL_REQ read_from(Reader& r) {
        KDBE_INIT_GUILD_SKILL_REQ v;
        v.m_iGuildUID = Codec<std::remove_cvref_t<decltype(v.m_iGuildUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_iGuildSPoint = Codec<std::remove_cvref_t<decltype(v.m_iGuildSPoint)>>::get(r);
        v.m_iGuildCSPoint = Codec<std::remove_cvref_t<decltype(v.m_iGuildCSPoint)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        return v;
    }
};

struct KDBE_INSERT_ITEM_ACK {
    std::map<std::int32_t, std::int32_t> m_mapInsertedItem{};  // std::map< int, int >
    KItemQuantityUpdate                  m_kItemQuantityUpdate{};
    std::map<std::int64_t, std::int32_t> m_kItemEnduranceUpdate{};  // std::map< UidType, int >
    std::vector<KInventoryItemInfo>      m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >
    std::map<std::int64_t, KItemInfo>    m_mapItemInfo{};  // std::map< UidType, KItemInfo >
    bool                                 m_bOutRoom{};
    std::map<std::int32_t, std::int32_t> m_mapGetItem{};  // std::map< int, int >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_mapInsertedItem)>>::put(w, m_mapInsertedItem);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_kItemEnduranceUpdate)>>::put(w, m_kItemEnduranceUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
        Codec<std::remove_cvref_t<decltype(m_mapItemInfo)>>::put(w, m_mapItemInfo);
        Codec<std::remove_cvref_t<decltype(m_bOutRoom)>>::put(w, m_bOutRoom);
        Codec<std::remove_cvref_t<decltype(m_mapGetItem)>>::put(w, m_mapGetItem);
    }

    static KDBE_INSERT_ITEM_ACK read_from(Reader& r) {
        KDBE_INSERT_ITEM_ACK v;
        v.m_mapInsertedItem = Codec<std::remove_cvref_t<decltype(v.m_mapInsertedItem)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_kItemEnduranceUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemEnduranceUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        v.m_mapItemInfo = Codec<std::remove_cvref_t<decltype(v.m_mapItemInfo)>>::get(r);
        v.m_bOutRoom = Codec<std::remove_cvref_t<decltype(v.m_bOutRoom)>>::get(r);
        v.m_mapGetItem = Codec<std::remove_cvref_t<decltype(v.m_mapGetItem)>>::get(r);
        return v;
    }
};

struct KDBE_INSERT_ITEM_REQ {
    std::int64_t                         m_iUnitUID{};  // UidType
    char                                 m_cGetItemReason{};
    std::map<std::int32_t, std::int32_t> m_mapInsertedItem{};  // std::map< int, int >
    KItemQuantityUpdate                  m_kItemQuantityUpdate{};
    std::map<std::int64_t, std::int32_t> m_kItemEnduranceUpdate{};  // std::map< UidType, int >
    std::vector<KInventoryItemInfo>      m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >
    std::vector<KItemInfo>               m_vecItemInfo{};  // std::vector< KItemInfo >
    bool                                 m_bOutRoom{};
    std::set<std::int32_t>               m_setSealCashItem{};  // std::set< int >
    std::map<std::int32_t, std::int32_t> m_mapGetItem{};  // std::map< int, int >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_cGetItemReason)>>::put(w, m_cGetItemReason);
        Codec<std::remove_cvref_t<decltype(m_mapInsertedItem)>>::put(w, m_mapInsertedItem);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_kItemEnduranceUpdate)>>::put(w, m_kItemEnduranceUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
        Codec<std::remove_cvref_t<decltype(m_vecItemInfo)>>::put(w, m_vecItemInfo);
        Codec<std::remove_cvref_t<decltype(m_bOutRoom)>>::put(w, m_bOutRoom);
        Codec<std::remove_cvref_t<decltype(m_setSealCashItem)>>::put(w, m_setSealCashItem);
        Codec<std::remove_cvref_t<decltype(m_mapGetItem)>>::put(w, m_mapGetItem);
    }

    static KDBE_INSERT_ITEM_REQ read_from(Reader& r) {
        KDBE_INSERT_ITEM_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_cGetItemReason = Codec<std::remove_cvref_t<decltype(v.m_cGetItemReason)>>::get(r);
        v.m_mapInsertedItem = Codec<std::remove_cvref_t<decltype(v.m_mapInsertedItem)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_kItemEnduranceUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemEnduranceUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        v.m_vecItemInfo = Codec<std::remove_cvref_t<decltype(v.m_vecItemInfo)>>::get(r);
        v.m_bOutRoom = Codec<std::remove_cvref_t<decltype(v.m_bOutRoom)>>::get(r);
        v.m_setSealCashItem = Codec<std::remove_cvref_t<decltype(v.m_setSealCashItem)>>::get(r);
        v.m_mapGetItem = Codec<std::remove_cvref_t<decltype(v.m_mapGetItem)>>::get(r);
        return v;
    }
};

struct KDBE_INSERT_PURCHASED_CASH_PACKAGE_ACK {
    std::map<std::int32_t, std::int32_t> m_mapInsertedItem{};  // std::map< int, int >
    KItemQuantityUpdate                  m_kItemQuantityUpdate{};
    std::map<std::int64_t, std::int32_t> m_kItemEnduranceUpdate{};  // std::map< UidType, int >
    std::vector<KInventoryItemInfo>      m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >
    std::map<std::int64_t, KItemInfo>    m_mapItemInfo{};  // std::map< UidType, KItemInfo >
    std::uint32_t                        m_ulProductNo{};  // unsigned long

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_mapInsertedItem)>>::put(w, m_mapInsertedItem);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_kItemEnduranceUpdate)>>::put(w, m_kItemEnduranceUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
        Codec<std::remove_cvref_t<decltype(m_mapItemInfo)>>::put(w, m_mapItemInfo);
        Codec<std::remove_cvref_t<decltype(m_ulProductNo)>>::put(w, m_ulProductNo);
    }

    static KDBE_INSERT_PURCHASED_CASH_PACKAGE_ACK read_from(Reader& r) {
        KDBE_INSERT_PURCHASED_CASH_PACKAGE_ACK v;
        v.m_mapInsertedItem = Codec<std::remove_cvref_t<decltype(v.m_mapInsertedItem)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_kItemEnduranceUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemEnduranceUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        v.m_mapItemInfo = Codec<std::remove_cvref_t<decltype(v.m_mapItemInfo)>>::get(r);
        v.m_ulProductNo = Codec<std::remove_cvref_t<decltype(v.m_ulProductNo)>>::get(r);
        return v;
    }
};

struct KDBE_INSERT_PURCHASED_CASH_PACKAGE_REQ {
    std::int64_t                         m_iUnitUID{};  // UidType
    std::map<std::int32_t, std::int32_t> m_mapInsertedItem{};  // std::map< int, int >
    KItemQuantityUpdate                  m_kItemQuantityUpdate{};
    std::map<std::int64_t, std::int32_t> m_kItemEnduranceUpdate{};  // std::map< UidType, int >
    std::vector<KInventoryItemInfo>      m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >
    std::vector<KItemInfo>               m_vecItemInfo{};  // std::vector< KItemInfo >
    std::uint32_t                        m_ulProductNo{};  // unsigned long
    std::set<std::int32_t>               m_setSealCashItem{};  // std::set< int >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_mapInsertedItem)>>::put(w, m_mapInsertedItem);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_kItemEnduranceUpdate)>>::put(w, m_kItemEnduranceUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
        Codec<std::remove_cvref_t<decltype(m_vecItemInfo)>>::put(w, m_vecItemInfo);
        Codec<std::remove_cvref_t<decltype(m_ulProductNo)>>::put(w, m_ulProductNo);
        Codec<std::remove_cvref_t<decltype(m_setSealCashItem)>>::put(w, m_setSealCashItem);
    }

    static KDBE_INSERT_PURCHASED_CASH_PACKAGE_REQ read_from(Reader& r) {
        KDBE_INSERT_PURCHASED_CASH_PACKAGE_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_mapInsertedItem = Codec<std::remove_cvref_t<decltype(v.m_mapInsertedItem)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_kItemEnduranceUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemEnduranceUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        v.m_vecItemInfo = Codec<std::remove_cvref_t<decltype(v.m_vecItemInfo)>>::get(r);
        v.m_ulProductNo = Codec<std::remove_cvref_t<decltype(v.m_ulProductNo)>>::get(r);
        v.m_setSealCashItem = Codec<std::remove_cvref_t<decltype(v.m_setSealCashItem)>>::get(r);
        return v;
    }
};

struct KDBE_INSERT_TRADE_ITEM_ACK {
    std::map<std::int32_t, std::int32_t> m_mapInsertedItem{};  // std::map< int, int >
    KItemQuantityUpdate                  m_kItemQuantityUpdate{};
    std::vector<KInventoryItemInfo>      m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >
    std::map<std::int64_t, KItemInfo>    m_mapItemInfo{};  // std::map< UidType, KItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_mapInsertedItem)>>::put(w, m_mapInsertedItem);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
        Codec<std::remove_cvref_t<decltype(m_mapItemInfo)>>::put(w, m_mapItemInfo);
    }

    static KDBE_INSERT_TRADE_ITEM_ACK read_from(Reader& r) {
        KDBE_INSERT_TRADE_ITEM_ACK v;
        v.m_mapInsertedItem = Codec<std::remove_cvref_t<decltype(v.m_mapInsertedItem)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        v.m_mapItemInfo = Codec<std::remove_cvref_t<decltype(v.m_mapItemInfo)>>::get(r);
        return v;
    }
};

struct KDBE_INSERT_TRADE_ITEM_BY_SERVER_NOT {
    std::int64_t           m_iUnitUID{};  // UidType
    std::int32_t           m_iED{};  // int
    KItemQuantityUpdate    m_kItemQuantityUpdate{};
    std::vector<KItemInfo> m_vecItemInfo{};  // std::vector< KItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iED)>>::put(w, m_iED);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecItemInfo)>>::put(w, m_vecItemInfo);
    }

    static KDBE_INSERT_TRADE_ITEM_BY_SERVER_NOT read_from(Reader& r) {
        KDBE_INSERT_TRADE_ITEM_BY_SERVER_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iED = Codec<std::remove_cvref_t<decltype(v.m_iED)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_vecItemInfo = Codec<std::remove_cvref_t<decltype(v.m_vecItemInfo)>>::get(r);
        return v;
    }
};

struct KDBE_INSERT_TRADE_ITEM_REQ {
    std::int64_t                         m_iUnitUID{};  // UidType
    std::map<std::int32_t, std::int32_t> m_mapInsertedItem{};  // std::map< int, int >
    KItemQuantityUpdate                  m_kItemQuantityUpdate{};
    std::vector<KInventoryItemInfo>      m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >
    std::vector<KItemInfo>               m_vecItemInfo{};  // std::vector< KItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_mapInsertedItem)>>::put(w, m_mapInsertedItem);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
        Codec<std::remove_cvref_t<decltype(m_vecItemInfo)>>::put(w, m_vecItemInfo);
    }

    static KDBE_INSERT_TRADE_ITEM_REQ read_from(Reader& r) {
        KDBE_INSERT_TRADE_ITEM_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_mapInsertedItem = Codec<std::remove_cvref_t<decltype(v.m_mapInsertedItem)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        v.m_vecItemInfo = Codec<std::remove_cvref_t<decltype(v.m_vecItemInfo)>>::get(r);
        return v;
    }
};

struct KDBE_ITEM_CONVERT_ACK {
    std::int32_t                      m_iOK{};  // int
    std::int32_t                      m_iCommissionED{};  // int
    std::map<std::int64_t, KItemInfo> m_mapItemInfo{};  // std::map< UidType, KItemInfo >
    KItemQuantityUpdate               m_kItemQuantityUpdate{};
    std::vector<KInventoryItemInfo>   m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iCommissionED)>>::put(w, m_iCommissionED);
        Codec<std::remove_cvref_t<decltype(m_mapItemInfo)>>::put(w, m_mapItemInfo);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
    }

    static KDBE_ITEM_CONVERT_ACK read_from(Reader& r) {
        KDBE_ITEM_CONVERT_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iCommissionED = Codec<std::remove_cvref_t<decltype(v.m_iCommissionED)>>::get(r);
        v.m_mapItemInfo = Codec<std::remove_cvref_t<decltype(v.m_mapItemInfo)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        return v;
    }
};

struct KDBE_ITEM_CONVERT_REQ {
    std::int64_t                    m_iUnitUID{};  // UidType
    std::int32_t                    m_iCommissionED{};  // int
    std::vector<KItemInfo>          m_vecItemInfo{};  // std::vector< KItemInfo >
    KItemQuantityUpdate             m_kItemQuantityUpdate{};
    std::vector<KInventoryItemInfo> m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iCommissionED)>>::put(w, m_iCommissionED);
        Codec<std::remove_cvref_t<decltype(m_vecItemInfo)>>::put(w, m_vecItemInfo);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
    }

    static KDBE_ITEM_CONVERT_REQ read_from(Reader& r) {
        KDBE_ITEM_CONVERT_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iCommissionED = Codec<std::remove_cvref_t<decltype(v.m_iCommissionED)>>::get(r);
        v.m_vecItemInfo = Codec<std::remove_cvref_t<decltype(v.m_vecItemInfo)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        return v;
    }
};

struct KDBE_ITEM_EXCHANGE_ACK {
    std::int32_t                         m_iOK{};  // int
    KItemQuantityUpdate                  m_kItemQuantityUpdate{};
    std::vector<KInventoryItemInfo>      m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >
    std::map<std::int64_t, KItemInfo>    m_mapItemInfo{};  // std::map< UidType, KItemInfo >
    std::map<std::int32_t, std::int32_t> m_mapResultItem{};  // std::map< int, int >
    std::int32_t                         m_iSourceItemID{};  // int
    std::int32_t                         m_iSourceItemQuantity{};  // int
    char                                 m_cExchangeType{};
    std::int32_t                         m_iExchangeCount{};  // int
    bool                                 m_b12TimesRewarded{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
        Codec<std::remove_cvref_t<decltype(m_mapItemInfo)>>::put(w, m_mapItemInfo);
        Codec<std::remove_cvref_t<decltype(m_mapResultItem)>>::put(w, m_mapResultItem);
        Codec<std::remove_cvref_t<decltype(m_iSourceItemID)>>::put(w, m_iSourceItemID);
        Codec<std::remove_cvref_t<decltype(m_iSourceItemQuantity)>>::put(w, m_iSourceItemQuantity);
        Codec<std::remove_cvref_t<decltype(m_cExchangeType)>>::put(w, m_cExchangeType);
        Codec<std::remove_cvref_t<decltype(m_iExchangeCount)>>::put(w, m_iExchangeCount);
        Codec<std::remove_cvref_t<decltype(m_b12TimesRewarded)>>::put(w, m_b12TimesRewarded);
    }

    static KDBE_ITEM_EXCHANGE_ACK read_from(Reader& r) {
        KDBE_ITEM_EXCHANGE_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        v.m_mapItemInfo = Codec<std::remove_cvref_t<decltype(v.m_mapItemInfo)>>::get(r);
        v.m_mapResultItem = Codec<std::remove_cvref_t<decltype(v.m_mapResultItem)>>::get(r);
        v.m_iSourceItemID = Codec<std::remove_cvref_t<decltype(v.m_iSourceItemID)>>::get(r);
        v.m_iSourceItemQuantity = Codec<std::remove_cvref_t<decltype(v.m_iSourceItemQuantity)>>::get(r);
        v.m_cExchangeType = Codec<std::remove_cvref_t<decltype(v.m_cExchangeType)>>::get(r);
        v.m_iExchangeCount = Codec<std::remove_cvref_t<decltype(v.m_iExchangeCount)>>::get(r);
        v.m_b12TimesRewarded = Codec<std::remove_cvref_t<decltype(v.m_b12TimesRewarded)>>::get(r);
        return v;
    }
};

struct KDBE_ITEM_EXCHANGE_REQ {
    std::int64_t                         m_iUnitUID{};  // UidType
    KItemQuantityUpdate                  m_kItemQuantityUpdate{};
    std::vector<KInventoryItemInfo>      m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >
    std::vector<KItemInfo>               m_vecItemInfo{};  // std::vector< KItemInfo >
    std::map<std::int32_t, std::int32_t> m_mapResultItem{};  // std::map< int, int >
    std::int32_t                         m_iSourceItemID{};  // int
    std::int32_t                         m_iSourceItemQuantity{};  // int
    char                                 m_cExchangeType{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
        Codec<std::remove_cvref_t<decltype(m_vecItemInfo)>>::put(w, m_vecItemInfo);
        Codec<std::remove_cvref_t<decltype(m_mapResultItem)>>::put(w, m_mapResultItem);
        Codec<std::remove_cvref_t<decltype(m_iSourceItemID)>>::put(w, m_iSourceItemID);
        Codec<std::remove_cvref_t<decltype(m_iSourceItemQuantity)>>::put(w, m_iSourceItemQuantity);
        Codec<std::remove_cvref_t<decltype(m_cExchangeType)>>::put(w, m_cExchangeType);
    }

    static KDBE_ITEM_EXCHANGE_REQ read_from(Reader& r) {
        KDBE_ITEM_EXCHANGE_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        v.m_vecItemInfo = Codec<std::remove_cvref_t<decltype(v.m_vecItemInfo)>>::get(r);
        v.m_mapResultItem = Codec<std::remove_cvref_t<decltype(v.m_mapResultItem)>>::get(r);
        v.m_iSourceItemID = Codec<std::remove_cvref_t<decltype(v.m_iSourceItemID)>>::get(r);
        v.m_iSourceItemQuantity = Codec<std::remove_cvref_t<decltype(v.m_iSourceItemQuantity)>>::get(r);
        v.m_cExchangeType = Codec<std::remove_cvref_t<decltype(v.m_cExchangeType)>>::get(r);
        return v;
    }
};

struct KDBE_ITEM_EXCHANGE_LIMIT_CHECK_ACK {
    std::int32_t           m_iOK{};  // int
    KDBE_ITEM_EXCHANGE_REQ m_kDB{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_kDB)>>::put(w, m_kDB);
    }

    static KDBE_ITEM_EXCHANGE_LIMIT_CHECK_ACK read_from(Reader& r) {
        KDBE_ITEM_EXCHANGE_LIMIT_CHECK_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_kDB = Codec<std::remove_cvref_t<decltype(v.m_kDB)>>::get(r);
        return v;
    }
};

struct KDBE_ITEM_EXCHANGE_LIMIT_CHECK_REQ {
    KEGS_ITEM_EXCHANGE_REQ m_kReq{};
    KDBE_ITEM_EXCHANGE_REQ m_kToDB{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kReq)>>::put(w, m_kReq);
        Codec<std::remove_cvref_t<decltype(m_kToDB)>>::put(w, m_kToDB);
    }

    static KDBE_ITEM_EXCHANGE_LIMIT_CHECK_REQ read_from(Reader& r) {
        KDBE_ITEM_EXCHANGE_LIMIT_CHECK_REQ v;
        v.m_kReq = Codec<std::remove_cvref_t<decltype(v.m_kReq)>>::get(r);
        v.m_kToDB = Codec<std::remove_cvref_t<decltype(v.m_kToDB)>>::get(r);
        return v;
    }
};

struct KDBE_ITEM_EXTRACT_ACK {
    std::int32_t                         m_iOK{};  // int
    KItemQuantityUpdate                  m_kItemQuantityUpdate{};
    std::vector<KInventoryItemInfo>      m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >
    std::map<std::int64_t, KItemInfo>    m_mapItemInfo{};  // std::map< UidType, KItemInfo >
    std::map<std::int32_t, std::int32_t> m_mapResultItem{};  // std::map< int, int >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
        Codec<std::remove_cvref_t<decltype(m_mapItemInfo)>>::put(w, m_mapItemInfo);
        Codec<std::remove_cvref_t<decltype(m_mapResultItem)>>::put(w, m_mapResultItem);
    }

    static KDBE_ITEM_EXTRACT_ACK read_from(Reader& r) {
        KDBE_ITEM_EXTRACT_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        v.m_mapItemInfo = Codec<std::remove_cvref_t<decltype(v.m_mapItemInfo)>>::get(r);
        v.m_mapResultItem = Codec<std::remove_cvref_t<decltype(v.m_mapResultItem)>>::get(r);
        return v;
    }
};

struct KDBE_ITEM_EXTRACT_REQ {
    std::int64_t                         m_iUnitUID{};  // UidType
    KItemQuantityUpdate                  m_kItemQuantityUpdate{};
    std::vector<KInventoryItemInfo>      m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >
    std::vector<KItemInfo>               m_vecItemInfo{};  // std::vector< KItemInfo >
    std::map<std::int32_t, std::int32_t> m_mapResultItem{};  // std::map< int, int >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
        Codec<std::remove_cvref_t<decltype(m_vecItemInfo)>>::put(w, m_vecItemInfo);
        Codec<std::remove_cvref_t<decltype(m_mapResultItem)>>::put(w, m_mapResultItem);
    }

    static KDBE_ITEM_EXTRACT_REQ read_from(Reader& r) {
        KDBE_ITEM_EXTRACT_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        v.m_vecItemInfo = Codec<std::remove_cvref_t<decltype(v.m_vecItemInfo)>>::get(r);
        v.m_mapResultItem = Codec<std::remove_cvref_t<decltype(v.m_mapResultItem)>>::get(r);
        return v;
    }
};

struct KDBE_ITEM_MANUFACTURE_ACK {
    std::int32_t                         m_iOK{};  // int
    std::map<std::int32_t, std::int32_t> m_mapInsertedItem{};  // std::map< int, int >
    KItemQuantityUpdate                  m_kItemQuantityUpdate{};
    std::vector<KInventoryItemInfo>      m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >
    std::map<std::int64_t, KItemInfo>    m_mapItemInfo{};  // std::map< UidType, KItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_mapInsertedItem)>>::put(w, m_mapInsertedItem);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
        Codec<std::remove_cvref_t<decltype(m_mapItemInfo)>>::put(w, m_mapItemInfo);
    }

    static KDBE_ITEM_MANUFACTURE_ACK read_from(Reader& r) {
        KDBE_ITEM_MANUFACTURE_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_mapInsertedItem = Codec<std::remove_cvref_t<decltype(v.m_mapInsertedItem)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        v.m_mapItemInfo = Codec<std::remove_cvref_t<decltype(v.m_mapItemInfo)>>::get(r);
        return v;
    }
};

struct KDBE_ITEM_MANUFACTURE_REQ {
    std::int64_t                         m_iUnitUID{};  // UidType
    std::map<std::int32_t, std::int32_t> m_mapInsertedItem{};  // std::map< int, int >
    KItemQuantityUpdate                  m_kItemQuantityUpdate{};
    std::vector<KInventoryItemInfo>      m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >
    std::vector<KItemInfo>               m_vecItemInfo{};  // std::vector< KItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_mapInsertedItem)>>::put(w, m_mapInsertedItem);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
        Codec<std::remove_cvref_t<decltype(m_vecItemInfo)>>::put(w, m_vecItemInfo);
    }

    static KDBE_ITEM_MANUFACTURE_REQ read_from(Reader& r) {
        KDBE_ITEM_MANUFACTURE_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_mapInsertedItem = Codec<std::remove_cvref_t<decltype(v.m_mapInsertedItem)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        v.m_vecItemInfo = Codec<std::remove_cvref_t<decltype(v.m_vecItemInfo)>>::get(r);
        return v;
    }
};

struct KDBE_NEW_QUEST_REQ {
    std::int64_t                         m_UnitUID{};  // UidType
    std::int32_t                         m_iQuestID{};  // int
    std::int32_t                         m_iTalkNPCID{};  // int
    std::map<std::int32_t, std::int32_t> m_mapInsertedItem{};  // std::map< int, int >
    KItemQuantityUpdate                  m_kItemQuantityUpdate{};
    std::vector<KInventoryItemInfo>      m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >
    std::vector<KItemInfo>               m_vecItemInfo{};  // std::vector< KItemInfo >
    std::int64_t                         m_UserUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_UnitUID)>>::put(w, m_UnitUID);
        Codec<std::remove_cvref_t<decltype(m_iQuestID)>>::put(w, m_iQuestID);
        Codec<std::remove_cvref_t<decltype(m_iTalkNPCID)>>::put(w, m_iTalkNPCID);
        Codec<std::remove_cvref_t<decltype(m_mapInsertedItem)>>::put(w, m_mapInsertedItem);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
        Codec<std::remove_cvref_t<decltype(m_vecItemInfo)>>::put(w, m_vecItemInfo);
        Codec<std::remove_cvref_t<decltype(m_UserUID)>>::put(w, m_UserUID);
    }

    static KDBE_NEW_QUEST_REQ read_from(Reader& r) {
        KDBE_NEW_QUEST_REQ v;
        v.m_UnitUID = Codec<std::remove_cvref_t<decltype(v.m_UnitUID)>>::get(r);
        v.m_iQuestID = Codec<std::remove_cvref_t<decltype(v.m_iQuestID)>>::get(r);
        v.m_iTalkNPCID = Codec<std::remove_cvref_t<decltype(v.m_iTalkNPCID)>>::get(r);
        v.m_mapInsertedItem = Codec<std::remove_cvref_t<decltype(v.m_mapInsertedItem)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        v.m_vecItemInfo = Codec<std::remove_cvref_t<decltype(v.m_vecItemInfo)>>::get(r);
        v.m_UserUID = Codec<std::remove_cvref_t<decltype(v.m_UserUID)>>::get(r);
        return v;
    }
};

struct KDBE_NEW_ACCOUNT_QUEST_ACK {
    std::int32_t       m_iOK{};  // int
    KDBE_NEW_QUEST_REQ m_kQuestReq{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_kQuestReq)>>::put(w, m_kQuestReq);
    }

    static KDBE_NEW_ACCOUNT_QUEST_ACK read_from(Reader& r) {
        KDBE_NEW_ACCOUNT_QUEST_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_kQuestReq = Codec<std::remove_cvref_t<decltype(v.m_kQuestReq)>>::get(r);
        return v;
    }
};

struct KDBE_OPEN_RANDOM_ITEM_ACK {
    std::int32_t                         m_iOK{};  // int
    std::map<std::int32_t, std::int32_t> m_mapInsertedItem{};  // std::map< int, int >
    KItemQuantityUpdate                  m_kItemQuantityUpdate{};
    std::vector<KInventoryItemInfo>      m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >
    std::map<std::int64_t, KItemInfo>    m_mapItemInfo{};  // std::map< UidType, KItemInfo >
    std::int32_t                         m_iNumResurrectionStone{};  // int
    std::int32_t                         m_iSpendED{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_mapInsertedItem)>>::put(w, m_mapInsertedItem);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
        Codec<std::remove_cvref_t<decltype(m_mapItemInfo)>>::put(w, m_mapItemInfo);
        Codec<std::remove_cvref_t<decltype(m_iNumResurrectionStone)>>::put(w, m_iNumResurrectionStone);
        Codec<std::remove_cvref_t<decltype(m_iSpendED)>>::put(w, m_iSpendED);
    }

    static KDBE_OPEN_RANDOM_ITEM_ACK read_from(Reader& r) {
        KDBE_OPEN_RANDOM_ITEM_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_mapInsertedItem = Codec<std::remove_cvref_t<decltype(v.m_mapInsertedItem)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        v.m_mapItemInfo = Codec<std::remove_cvref_t<decltype(v.m_mapItemInfo)>>::get(r);
        v.m_iNumResurrectionStone = Codec<std::remove_cvref_t<decltype(v.m_iNumResurrectionStone)>>::get(r);
        v.m_iSpendED = Codec<std::remove_cvref_t<decltype(v.m_iSpendED)>>::get(r);
        return v;
    }
};

struct KDBE_OPEN_RANDOM_ITEM_REQ {
    std::int64_t                         m_iUnitUID{};  // UidType
    std::map<std::int32_t, std::int32_t> m_mapInsertedItem{};  // std::map< int, int >
    KItemQuantityUpdate                  m_kItemQuantityUpdate{};
    std::vector<KInventoryItemInfo>      m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >
    std::vector<KItemInfo>               m_vecItemInfo{};  // std::vector< KItemInfo >
    std::set<std::int32_t>               m_setSealRandomItem{};  // std::set< int >
    std::int32_t                         m_iNumResurrectionStone{};  // int
    std::int32_t                         m_iSpendED{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_mapInsertedItem)>>::put(w, m_mapInsertedItem);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
        Codec<std::remove_cvref_t<decltype(m_vecItemInfo)>>::put(w, m_vecItemInfo);
        Codec<std::remove_cvref_t<decltype(m_setSealRandomItem)>>::put(w, m_setSealRandomItem);
        Codec<std::remove_cvref_t<decltype(m_iNumResurrectionStone)>>::put(w, m_iNumResurrectionStone);
        Codec<std::remove_cvref_t<decltype(m_iSpendED)>>::put(w, m_iSpendED);
    }

    static KDBE_OPEN_RANDOM_ITEM_REQ read_from(Reader& r) {
        KDBE_OPEN_RANDOM_ITEM_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_mapInsertedItem = Codec<std::remove_cvref_t<decltype(v.m_mapInsertedItem)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        v.m_vecItemInfo = Codec<std::remove_cvref_t<decltype(v.m_vecItemInfo)>>::get(r);
        v.m_setSealRandomItem = Codec<std::remove_cvref_t<decltype(v.m_setSealRandomItem)>>::get(r);
        v.m_iNumResurrectionStone = Codec<std::remove_cvref_t<decltype(v.m_iNumResurrectionStone)>>::get(r);
        v.m_iSpendED = Codec<std::remove_cvref_t<decltype(v.m_iSpendED)>>::get(r);
        return v;
    }
};

struct KDBE_QUEST_COMPLETE_ACK {
    std::int32_t                      m_iOK{};  // int
    KCompleteQuestInfo                m_kCompleteQuestInfo{};
    bool                              m_bIsChangeJob{};
    char                              m_cChangeUnitClass{};
    bool                              m_bIsNew{};
    std::map<std::int32_t, KItemInfo> m_mapInsertedItem{};  // std::map< int, KItemInfo >
    KItemQuantityUpdate               m_kItemQuantityUpdate{};
    std::vector<KInventoryItemInfo>   m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >
    std::map<std::int64_t, KItemInfo> m_mapItemInfo{};  // std::map< UidType, KItemInfo >
    bool                              m_bAutoComplete{};
    std::int32_t                      m_iNewDefaultSkill1{};  // int
    std::int32_t                      m_iNewDefaultSkill2{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_kCompleteQuestInfo)>>::put(w, m_kCompleteQuestInfo);
        Codec<std::remove_cvref_t<decltype(m_bIsChangeJob)>>::put(w, m_bIsChangeJob);
        Codec<std::remove_cvref_t<decltype(m_cChangeUnitClass)>>::put(w, m_cChangeUnitClass);
        Codec<std::remove_cvref_t<decltype(m_bIsNew)>>::put(w, m_bIsNew);
        Codec<std::remove_cvref_t<decltype(m_mapInsertedItem)>>::put(w, m_mapInsertedItem);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
        Codec<std::remove_cvref_t<decltype(m_mapItemInfo)>>::put(w, m_mapItemInfo);
        Codec<std::remove_cvref_t<decltype(m_bAutoComplete)>>::put(w, m_bAutoComplete);
        Codec<std::remove_cvref_t<decltype(m_iNewDefaultSkill1)>>::put(w, m_iNewDefaultSkill1);
        Codec<std::remove_cvref_t<decltype(m_iNewDefaultSkill2)>>::put(w, m_iNewDefaultSkill2);
    }

    static KDBE_QUEST_COMPLETE_ACK read_from(Reader& r) {
        KDBE_QUEST_COMPLETE_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_kCompleteQuestInfo = Codec<std::remove_cvref_t<decltype(v.m_kCompleteQuestInfo)>>::get(r);
        v.m_bIsChangeJob = Codec<std::remove_cvref_t<decltype(v.m_bIsChangeJob)>>::get(r);
        v.m_cChangeUnitClass = Codec<std::remove_cvref_t<decltype(v.m_cChangeUnitClass)>>::get(r);
        v.m_bIsNew = Codec<std::remove_cvref_t<decltype(v.m_bIsNew)>>::get(r);
        v.m_mapInsertedItem = Codec<std::remove_cvref_t<decltype(v.m_mapInsertedItem)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        v.m_mapItemInfo = Codec<std::remove_cvref_t<decltype(v.m_mapItemInfo)>>::get(r);
        v.m_bAutoComplete = Codec<std::remove_cvref_t<decltype(v.m_bAutoComplete)>>::get(r);
        v.m_iNewDefaultSkill1 = Codec<std::remove_cvref_t<decltype(v.m_iNewDefaultSkill1)>>::get(r);
        v.m_iNewDefaultSkill2 = Codec<std::remove_cvref_t<decltype(v.m_iNewDefaultSkill2)>>::get(r);
        return v;
    }
};

struct KDBE_QUEST_COMPLETE_REQ {
    std::int64_t                      m_UnitUID{};  // UidType
    std::int32_t                      m_iQuestID{};  // int
    bool                              m_bIsRepeat{};
    bool                              m_bIsChangeJob{};
    char                              m_cChangeUnitClass{};
    bool                              m_bIsNew{};
    std::map<std::int32_t, KItemInfo> m_mapInsertedItem{};  // std::map< int, KItemInfo >
    KItemQuantityUpdate               m_kItemQuantityUpdate{};
    std::vector<KInventoryItemInfo>   m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >
    std::vector<KItemInfo>            m_vecItemInfo{};  // std::vector< KItemInfo >
    bool                              m_bAutoComplete{};
    std::int32_t                      m_iNewDefaultSkill1{};  // int
    std::int32_t                      m_iNewDefaultSkill2{};  // int
    std::int32_t                      m_iTheNumberOfSkillPagesAvailable{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_UnitUID)>>::put(w, m_UnitUID);
        Codec<std::remove_cvref_t<decltype(m_iQuestID)>>::put(w, m_iQuestID);
        Codec<std::remove_cvref_t<decltype(m_bIsRepeat)>>::put(w, m_bIsRepeat);
        Codec<std::remove_cvref_t<decltype(m_bIsChangeJob)>>::put(w, m_bIsChangeJob);
        Codec<std::remove_cvref_t<decltype(m_cChangeUnitClass)>>::put(w, m_cChangeUnitClass);
        Codec<std::remove_cvref_t<decltype(m_bIsNew)>>::put(w, m_bIsNew);
        Codec<std::remove_cvref_t<decltype(m_mapInsertedItem)>>::put(w, m_mapInsertedItem);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
        Codec<std::remove_cvref_t<decltype(m_vecItemInfo)>>::put(w, m_vecItemInfo);
        Codec<std::remove_cvref_t<decltype(m_bAutoComplete)>>::put(w, m_bAutoComplete);
        Codec<std::remove_cvref_t<decltype(m_iNewDefaultSkill1)>>::put(w, m_iNewDefaultSkill1);
        Codec<std::remove_cvref_t<decltype(m_iNewDefaultSkill2)>>::put(w, m_iNewDefaultSkill2);
        Codec<std::remove_cvref_t<decltype(m_iTheNumberOfSkillPagesAvailable)>>::put(w, m_iTheNumberOfSkillPagesAvailable);
    }

    static KDBE_QUEST_COMPLETE_REQ read_from(Reader& r) {
        KDBE_QUEST_COMPLETE_REQ v;
        v.m_UnitUID = Codec<std::remove_cvref_t<decltype(v.m_UnitUID)>>::get(r);
        v.m_iQuestID = Codec<std::remove_cvref_t<decltype(v.m_iQuestID)>>::get(r);
        v.m_bIsRepeat = Codec<std::remove_cvref_t<decltype(v.m_bIsRepeat)>>::get(r);
        v.m_bIsChangeJob = Codec<std::remove_cvref_t<decltype(v.m_bIsChangeJob)>>::get(r);
        v.m_cChangeUnitClass = Codec<std::remove_cvref_t<decltype(v.m_cChangeUnitClass)>>::get(r);
        v.m_bIsNew = Codec<std::remove_cvref_t<decltype(v.m_bIsNew)>>::get(r);
        v.m_mapInsertedItem = Codec<std::remove_cvref_t<decltype(v.m_mapInsertedItem)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        v.m_vecItemInfo = Codec<std::remove_cvref_t<decltype(v.m_vecItemInfo)>>::get(r);
        v.m_bAutoComplete = Codec<std::remove_cvref_t<decltype(v.m_bAutoComplete)>>::get(r);
        v.m_iNewDefaultSkill1 = Codec<std::remove_cvref_t<decltype(v.m_iNewDefaultSkill1)>>::get(r);
        v.m_iNewDefaultSkill2 = Codec<std::remove_cvref_t<decltype(v.m_iNewDefaultSkill2)>>::get(r);
        v.m_iTheNumberOfSkillPagesAvailable = Codec<std::remove_cvref_t<decltype(v.m_iTheNumberOfSkillPagesAvailable)>>::get(r);
        return v;
    }
};

struct KDBE_ACCOUNT_QUEST_COMPLETE_ACK {
    std::int32_t            m_iOK{};  // int
    KDBE_QUEST_COMPLETE_REQ m_kQuestReq{};
    std::int64_t            m_tCompleteTime{};  // __int64

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_kQuestReq)>>::put(w, m_kQuestReq);
        Codec<std::remove_cvref_t<decltype(m_tCompleteTime)>>::put(w, m_tCompleteTime);
    }

    static KDBE_ACCOUNT_QUEST_COMPLETE_ACK read_from(Reader& r) {
        KDBE_ACCOUNT_QUEST_COMPLETE_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_kQuestReq = Codec<std::remove_cvref_t<decltype(v.m_kQuestReq)>>::get(r);
        v.m_tCompleteTime = Codec<std::remove_cvref_t<decltype(v.m_tCompleteTime)>>::get(r);
        return v;
    }
};

struct KDBE_RESET_GUILD_SKILL_ACK {
    std::int32_t                    m_iOK{};  // int
    std::int32_t                    m_iGuildUID{};  // int
    std::int32_t                    m_iGuildSkillID{};  // int
    std::int32_t                    m_iGuildSkillLevel{};  // int
    std::int32_t                    m_iGuildSkillCSPoint{};  // int
    std::int32_t                    m_iGuildSPoint{};  // int
    std::int32_t                    m_iGuildCSPoint{};  // int
    KItemQuantityUpdate             m_kItemQuantityUpdate{};
    std::vector<KInventoryItemInfo> m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iGuildUID)>>::put(w, m_iGuildUID);
        Codec<std::remove_cvref_t<decltype(m_iGuildSkillID)>>::put(w, m_iGuildSkillID);
        Codec<std::remove_cvref_t<decltype(m_iGuildSkillLevel)>>::put(w, m_iGuildSkillLevel);
        Codec<std::remove_cvref_t<decltype(m_iGuildSkillCSPoint)>>::put(w, m_iGuildSkillCSPoint);
        Codec<std::remove_cvref_t<decltype(m_iGuildSPoint)>>::put(w, m_iGuildSPoint);
        Codec<std::remove_cvref_t<decltype(m_iGuildCSPoint)>>::put(w, m_iGuildCSPoint);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
    }

    static KDBE_RESET_GUILD_SKILL_ACK read_from(Reader& r) {
        KDBE_RESET_GUILD_SKILL_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iGuildUID = Codec<std::remove_cvref_t<decltype(v.m_iGuildUID)>>::get(r);
        v.m_iGuildSkillID = Codec<std::remove_cvref_t<decltype(v.m_iGuildSkillID)>>::get(r);
        v.m_iGuildSkillLevel = Codec<std::remove_cvref_t<decltype(v.m_iGuildSkillLevel)>>::get(r);
        v.m_iGuildSkillCSPoint = Codec<std::remove_cvref_t<decltype(v.m_iGuildSkillCSPoint)>>::get(r);
        v.m_iGuildSPoint = Codec<std::remove_cvref_t<decltype(v.m_iGuildSPoint)>>::get(r);
        v.m_iGuildCSPoint = Codec<std::remove_cvref_t<decltype(v.m_iGuildCSPoint)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        return v;
    }
};

struct KDBE_RESET_GUILD_SKILL_REQ {
    std::int32_t                    m_iGuildUID{};  // int
    std::int64_t                    m_iUnitUID{};  // UidType
    std::int32_t                    m_iGuildSkillID{};  // int
    std::int32_t                    m_iGuildSkillLevel{};  // int
    std::int32_t                    m_iGuildSkillCSPoint{};  // int
    std::int32_t                    m_iGuildSPoint{};  // int
    std::int32_t                    m_iGuildCSPoint{};  // int
    std::int32_t                    m_iMaxGuildCSPoint{};  // int
    KItemQuantityUpdate             m_kItemQuantityUpdate{};
    std::vector<KInventoryItemInfo> m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iGuildUID)>>::put(w, m_iGuildUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iGuildSkillID)>>::put(w, m_iGuildSkillID);
        Codec<std::remove_cvref_t<decltype(m_iGuildSkillLevel)>>::put(w, m_iGuildSkillLevel);
        Codec<std::remove_cvref_t<decltype(m_iGuildSkillCSPoint)>>::put(w, m_iGuildSkillCSPoint);
        Codec<std::remove_cvref_t<decltype(m_iGuildSPoint)>>::put(w, m_iGuildSPoint);
        Codec<std::remove_cvref_t<decltype(m_iGuildCSPoint)>>::put(w, m_iGuildCSPoint);
        Codec<std::remove_cvref_t<decltype(m_iMaxGuildCSPoint)>>::put(w, m_iMaxGuildCSPoint);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
    }

    static KDBE_RESET_GUILD_SKILL_REQ read_from(Reader& r) {
        KDBE_RESET_GUILD_SKILL_REQ v;
        v.m_iGuildUID = Codec<std::remove_cvref_t<decltype(v.m_iGuildUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iGuildSkillID = Codec<std::remove_cvref_t<decltype(v.m_iGuildSkillID)>>::get(r);
        v.m_iGuildSkillLevel = Codec<std::remove_cvref_t<decltype(v.m_iGuildSkillLevel)>>::get(r);
        v.m_iGuildSkillCSPoint = Codec<std::remove_cvref_t<decltype(v.m_iGuildSkillCSPoint)>>::get(r);
        v.m_iGuildSPoint = Codec<std::remove_cvref_t<decltype(v.m_iGuildSPoint)>>::get(r);
        v.m_iGuildCSPoint = Codec<std::remove_cvref_t<decltype(v.m_iGuildCSPoint)>>::get(r);
        v.m_iMaxGuildCSPoint = Codec<std::remove_cvref_t<decltype(v.m_iMaxGuildCSPoint)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        return v;
    }
};

struct KDBE_RESET_SKILL_ACK {
    std::int32_t                    m_iOK{};  // int
    std::int32_t                    m_iActiveSkillPageNumber{};  // int
    std::int32_t                    m_iDelSkillID{};  // int
    std::int32_t                    m_iDelSkillLevel{};  // int
    std::int32_t                    m_iCSPoint{};  // int
    std::int32_t                    m_iBeforeSPoint{};  // int
    std::int32_t                    m_iBeforeCSPoint{};  // int
    KItemQuantityUpdate             m_kItemQuantityUpdate{};
    std::vector<KInventoryItemInfo> m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iActiveSkillPageNumber)>>::put(w, m_iActiveSkillPageNumber);
        Codec<std::remove_cvref_t<decltype(m_iDelSkillID)>>::put(w, m_iDelSkillID);
        Codec<std::remove_cvref_t<decltype(m_iDelSkillLevel)>>::put(w, m_iDelSkillLevel);
        Codec<std::remove_cvref_t<decltype(m_iCSPoint)>>::put(w, m_iCSPoint);
        Codec<std::remove_cvref_t<decltype(m_iBeforeSPoint)>>::put(w, m_iBeforeSPoint);
        Codec<std::remove_cvref_t<decltype(m_iBeforeCSPoint)>>::put(w, m_iBeforeCSPoint);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
    }

    static KDBE_RESET_SKILL_ACK read_from(Reader& r) {
        KDBE_RESET_SKILL_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iActiveSkillPageNumber = Codec<std::remove_cvref_t<decltype(v.m_iActiveSkillPageNumber)>>::get(r);
        v.m_iDelSkillID = Codec<std::remove_cvref_t<decltype(v.m_iDelSkillID)>>::get(r);
        v.m_iDelSkillLevel = Codec<std::remove_cvref_t<decltype(v.m_iDelSkillLevel)>>::get(r);
        v.m_iCSPoint = Codec<std::remove_cvref_t<decltype(v.m_iCSPoint)>>::get(r);
        v.m_iBeforeSPoint = Codec<std::remove_cvref_t<decltype(v.m_iBeforeSPoint)>>::get(r);
        v.m_iBeforeCSPoint = Codec<std::remove_cvref_t<decltype(v.m_iBeforeCSPoint)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        return v;
    }
};

struct KDBE_RESET_SKILL_REQ {
    std::int64_t                    m_iUnitUID{};  // UidType
    std::int32_t                    m_iActiveSkillPageNumber{};  // int
    std::int32_t                    m_iDelSkillID{};  // int
    std::int32_t                    m_iDelSkillLevel{};  // int
    std::int32_t                    m_iCSPoint{};  // int
    std::int32_t                    m_iBeforeSPoint{};  // int
    std::int32_t                    m_iBeforeCSPoint{};  // int
    KItemQuantityUpdate             m_kItemQuantityUpdate{};
    std::vector<KInventoryItemInfo> m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iActiveSkillPageNumber)>>::put(w, m_iActiveSkillPageNumber);
        Codec<std::remove_cvref_t<decltype(m_iDelSkillID)>>::put(w, m_iDelSkillID);
        Codec<std::remove_cvref_t<decltype(m_iDelSkillLevel)>>::put(w, m_iDelSkillLevel);
        Codec<std::remove_cvref_t<decltype(m_iCSPoint)>>::put(w, m_iCSPoint);
        Codec<std::remove_cvref_t<decltype(m_iBeforeSPoint)>>::put(w, m_iBeforeSPoint);
        Codec<std::remove_cvref_t<decltype(m_iBeforeCSPoint)>>::put(w, m_iBeforeCSPoint);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
    }

    static KDBE_RESET_SKILL_REQ read_from(Reader& r) {
        KDBE_RESET_SKILL_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iActiveSkillPageNumber = Codec<std::remove_cvref_t<decltype(v.m_iActiveSkillPageNumber)>>::get(r);
        v.m_iDelSkillID = Codec<std::remove_cvref_t<decltype(v.m_iDelSkillID)>>::get(r);
        v.m_iDelSkillLevel = Codec<std::remove_cvref_t<decltype(v.m_iDelSkillLevel)>>::get(r);
        v.m_iCSPoint = Codec<std::remove_cvref_t<decltype(v.m_iCSPoint)>>::get(r);
        v.m_iBeforeSPoint = Codec<std::remove_cvref_t<decltype(v.m_iBeforeSPoint)>>::get(r);
        v.m_iBeforeCSPoint = Codec<std::remove_cvref_t<decltype(v.m_iBeforeCSPoint)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        return v;
    }
};

struct KDBE_RESOLVE_ITEM_ACK {
    std::int32_t                         m_iOK{};  // int
    bool                                 m_bJackpot{};
    std::map<std::int32_t, std::int32_t> m_mapInsertedItem{};  // std::map< int, int >
    KItemQuantityUpdate                  m_kItemQuantityUpdate{};
    std::vector<KInventoryItemInfo>      m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >
    std::map<std::int64_t, KItemInfo>    m_mapItemInfo{};  // std::map< UidType, KItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_bJackpot)>>::put(w, m_bJackpot);
        Codec<std::remove_cvref_t<decltype(m_mapInsertedItem)>>::put(w, m_mapInsertedItem);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
        Codec<std::remove_cvref_t<decltype(m_mapItemInfo)>>::put(w, m_mapItemInfo);
    }

    static KDBE_RESOLVE_ITEM_ACK read_from(Reader& r) {
        KDBE_RESOLVE_ITEM_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_bJackpot = Codec<std::remove_cvref_t<decltype(v.m_bJackpot)>>::get(r);
        v.m_mapInsertedItem = Codec<std::remove_cvref_t<decltype(v.m_mapInsertedItem)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        v.m_mapItemInfo = Codec<std::remove_cvref_t<decltype(v.m_mapItemInfo)>>::get(r);
        return v;
    }
};

struct KDBE_RESOLVE_ITEM_REQ {
    std::int64_t                         m_iUnitUID{};  // UidType
    bool                                 m_bJackpot{};
    std::map<std::int32_t, std::int32_t> m_mapInsertedItem{};  // std::map< int, int >
    KItemQuantityUpdate                  m_kItemQuantityUpdate{};
    std::vector<KInventoryItemInfo>      m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >
    std::vector<KItemInfo>               m_vecItemInfo{};  // std::vector< KItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_bJackpot)>>::put(w, m_bJackpot);
        Codec<std::remove_cvref_t<decltype(m_mapInsertedItem)>>::put(w, m_mapInsertedItem);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
        Codec<std::remove_cvref_t<decltype(m_vecItemInfo)>>::put(w, m_vecItemInfo);
    }

    static KDBE_RESOLVE_ITEM_REQ read_from(Reader& r) {
        KDBE_RESOLVE_ITEM_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_bJackpot = Codec<std::remove_cvref_t<decltype(v.m_bJackpot)>>::get(r);
        v.m_mapInsertedItem = Codec<std::remove_cvref_t<decltype(v.m_mapInsertedItem)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        v.m_vecItemInfo = Codec<std::remove_cvref_t<decltype(v.m_vecItemInfo)>>::get(r);
        return v;
    }
};

struct KDBE_RESTORE_ITEM_ACK {
    std::int32_t                    m_iOK{};  // int
    std::int64_t                    m_iItemUID{};  // UidType
    std::int32_t                    m_iLevelAfterEnchant{};  // int
    KItemQuantityUpdate             m_kItemQuantityUpdate{};
    std::vector<KInventoryItemInfo> m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_iLevelAfterEnchant)>>::put(w, m_iLevelAfterEnchant);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
    }

    static KDBE_RESTORE_ITEM_ACK read_from(Reader& r) {
        KDBE_RESTORE_ITEM_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_iLevelAfterEnchant = Codec<std::remove_cvref_t<decltype(v.m_iLevelAfterEnchant)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        return v;
    }
};

struct KDBE_RESTORE_ITEM_EVALUATE_ACK {
    std::int32_t                    m_iOK{};  // int
    std::int64_t                    m_iTargetItemUID{};  // UidType
    KItemQuantityUpdate             m_kItemQuantityUpdate{};
    std::vector<KInventoryItemInfo> m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iTargetItemUID)>>::put(w, m_iTargetItemUID);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
    }

    static KDBE_RESTORE_ITEM_EVALUATE_ACK read_from(Reader& r) {
        KDBE_RESTORE_ITEM_EVALUATE_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iTargetItemUID = Codec<std::remove_cvref_t<decltype(v.m_iTargetItemUID)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        return v;
    }
};

struct KDBE_RESTORE_ITEM_EVALUATE_REQ {
    std::int64_t                    m_iUnitUID{};  // UidType
    std::int64_t                    m_iTargetItemUID{};  // UidType
    KItemQuantityUpdate             m_kItemQuantityUpdate{};
    std::vector<KInventoryItemInfo> m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iTargetItemUID)>>::put(w, m_iTargetItemUID);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
    }

    static KDBE_RESTORE_ITEM_EVALUATE_REQ read_from(Reader& r) {
        KDBE_RESTORE_ITEM_EVALUATE_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iTargetItemUID = Codec<std::remove_cvref_t<decltype(v.m_iTargetItemUID)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        return v;
    }
};

struct KDBE_RESTORE_ITEM_REQ {
    std::int64_t                    m_iUnitUID{};  // UidType
    std::int64_t                    m_iItemUID{};  // UidType
    std::int32_t                    m_iLevelAfterEnchant{};  // int
    KItemQuantityUpdate             m_kItemQuantityUpdate{};
    std::vector<KInventoryItemInfo> m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_iLevelAfterEnchant)>>::put(w, m_iLevelAfterEnchant);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
    }

    static KDBE_RESTORE_ITEM_REQ read_from(Reader& r) {
        KDBE_RESTORE_ITEM_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_iLevelAfterEnchant = Codec<std::remove_cvref_t<decltype(v.m_iLevelAfterEnchant)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        return v;
    }
};

struct KDBE_SEAL_ITEM_ACK {
    std::int32_t                    m_iOK{};  // int
    std::uint8_t                    m_ucSealResult{};  // u_char
    KItemQuantityUpdate             m_kItemQuantityUpdate{};
    std::vector<KInventoryItemInfo> m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_ucSealResult)>>::put(w, m_ucSealResult);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
    }

    static KDBE_SEAL_ITEM_ACK read_from(Reader& r) {
        KDBE_SEAL_ITEM_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_ucSealResult = Codec<std::remove_cvref_t<decltype(v.m_ucSealResult)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        return v;
    }
};

struct KDBE_SEAL_ITEM_REQ {
    std::int64_t                    m_iUnitUID{};  // UidType
    std::int64_t                    m_iItemUID{};  // UidType
    std::uint8_t                    m_ucSealResult{};  // u_char
    KItemQuantityUpdate             m_kItemQuantityUpdate{};
    std::vector<KInventoryItemInfo> m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_ucSealResult)>>::put(w, m_ucSealResult);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
    }

    static KDBE_SEAL_ITEM_REQ read_from(Reader& r) {
        KDBE_SEAL_ITEM_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_ucSealResult = Codec<std::remove_cvref_t<decltype(v.m_ucSealResult)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        return v;
    }
};

struct KDBE_SOCKET_EXPAND_ITEM_ACK {
    std::int32_t                    m_iOK{};  // int
    std::int64_t                    m_iItemUID{};  // UidType
    std::uint8_t                    m_byteExpandedSocketNum{};  // byte
    KItemQuantityUpdate             m_kItemQuantityUpdate{};
    std::vector<KInventoryItemInfo> m_vecUpdatedInventorySlot{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_byteExpandedSocketNum)>>::put(w, m_byteExpandedSocketNum);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
    }

    static KDBE_SOCKET_EXPAND_ITEM_ACK read_from(Reader& r) {
        KDBE_SOCKET_EXPAND_ITEM_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_byteExpandedSocketNum = Codec<std::remove_cvref_t<decltype(v.m_byteExpandedSocketNum)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        return v;
    }
};

struct KDBE_SOCKET_EXPAND_ITEM_REQ {
    std::int64_t                    m_iUnitUID{};  // UidType
    std::int64_t                    m_iItemUID{};  // UidType
    std::vector<std::int32_t>       m_vecItemSocket{};  // std::vector<int>
    std::uint8_t                    m_byteExpandedSocketNum{};  // byte
    KItemQuantityUpdate             m_kItemQuantityUpdate{};
    std::vector<KInventoryItemInfo> m_vecUpdatedInventorySlot{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_vecItemSocket)>>::put(w, m_vecItemSocket);
        Codec<std::remove_cvref_t<decltype(m_byteExpandedSocketNum)>>::put(w, m_byteExpandedSocketNum);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
    }

    static KDBE_SOCKET_EXPAND_ITEM_REQ read_from(Reader& r) {
        KDBE_SOCKET_EXPAND_ITEM_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_vecItemSocket = Codec<std::remove_cvref_t<decltype(v.m_vecItemSocket)>>::get(r);
        v.m_byteExpandedSocketNum = Codec<std::remove_cvref_t<decltype(v.m_byteExpandedSocketNum)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        return v;
    }
};

struct KDBE_SOCKET_ITEM_ACK {
    std::int32_t                    m_iOK{};  // int
    std::vector<std::int32_t>       m_vecSocketInfo{};  // std::vector< int >
    std::int64_t                    m_iItemUID{};  // UidType
    std::int32_t                    m_iItemID{};  // int
    KItemQuantityUpdate             m_kItemQuantityUpdate{};
    std::vector<KInventoryItemInfo> m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >
    std::int32_t                    m_iSocketUseCount{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_vecSocketInfo)>>::put(w, m_vecSocketInfo);
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_iItemID)>>::put(w, m_iItemID);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
        Codec<std::remove_cvref_t<decltype(m_iSocketUseCount)>>::put(w, m_iSocketUseCount);
    }

    static KDBE_SOCKET_ITEM_ACK read_from(Reader& r) {
        KDBE_SOCKET_ITEM_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_vecSocketInfo = Codec<std::remove_cvref_t<decltype(v.m_vecSocketInfo)>>::get(r);
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_iItemID = Codec<std::remove_cvref_t<decltype(v.m_iItemID)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        v.m_iSocketUseCount = Codec<std::remove_cvref_t<decltype(v.m_iSocketUseCount)>>::get(r);
        return v;
    }
};

struct KDBE_SOCKET_ITEM_REQ {
    std::int64_t                    m_iUnitUID{};  // UidType
    std::vector<std::int32_t>       m_vecSocketInfo{};  // std::vector< int >
    std::int64_t                    m_iItemUID{};  // UidType
    std::int32_t                    m_iItemID{};  // int
    KItemQuantityUpdate             m_kItemQuantityUpdate{};
    std::vector<KInventoryItemInfo> m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >
    std::int32_t                    m_iSocketUseCount{};  // int
    std::uint8_t                    m_byteExpandedSocketNum{};  // byte

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_vecSocketInfo)>>::put(w, m_vecSocketInfo);
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_iItemID)>>::put(w, m_iItemID);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
        Codec<std::remove_cvref_t<decltype(m_iSocketUseCount)>>::put(w, m_iSocketUseCount);
        Codec<std::remove_cvref_t<decltype(m_byteExpandedSocketNum)>>::put(w, m_byteExpandedSocketNum);
    }

    static KDBE_SOCKET_ITEM_REQ read_from(Reader& r) {
        KDBE_SOCKET_ITEM_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_vecSocketInfo = Codec<std::remove_cvref_t<decltype(v.m_vecSocketInfo)>>::get(r);
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_iItemID = Codec<std::remove_cvref_t<decltype(v.m_iItemID)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        v.m_iSocketUseCount = Codec<std::remove_cvref_t<decltype(v.m_iSocketUseCount)>>::get(r);
        v.m_byteExpandedSocketNum = Codec<std::remove_cvref_t<decltype(v.m_byteExpandedSocketNum)>>::get(r);
        return v;
    }
};

struct KDBE_SUMMON_PET_ACK {
    std::int32_t                          m_iOK{};  // int
    std::int64_t                          m_iBeforeSummonPetUID{};  // UidType
    std::int64_t                          m_iSummonPetUID{};  // UidType
    KItemQuantityUpdate                   m_kItemQuantityUpdate{};
    std::map<std::int64_t, std::int32_t>  m_kItemEnduranceUpdate{};  // KItemEnduranceUpdate
    std::map<std::int64_t, KItemPosition> m_kItemPositionUpdate{};  // KItemPositionUpdate

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iBeforeSummonPetUID)>>::put(w, m_iBeforeSummonPetUID);
        Codec<std::remove_cvref_t<decltype(m_iSummonPetUID)>>::put(w, m_iSummonPetUID);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_kItemEnduranceUpdate)>>::put(w, m_kItemEnduranceUpdate);
        Codec<std::remove_cvref_t<decltype(m_kItemPositionUpdate)>>::put(w, m_kItemPositionUpdate);
    }

    static KDBE_SUMMON_PET_ACK read_from(Reader& r) {
        KDBE_SUMMON_PET_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iBeforeSummonPetUID = Codec<std::remove_cvref_t<decltype(v.m_iBeforeSummonPetUID)>>::get(r);
        v.m_iSummonPetUID = Codec<std::remove_cvref_t<decltype(v.m_iSummonPetUID)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_kItemEnduranceUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemEnduranceUpdate)>>::get(r);
        v.m_kItemPositionUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemPositionUpdate)>>::get(r);
        return v;
    }
};

struct KDBE_SUMMON_PET_REQ {
    std::int64_t                          m_iUnitUID{};  // UidType
    std::int64_t                          m_iBeforeSummonPetUID{};  // UidType
    std::int64_t                          m_iSummonPetUID{};  // UidType
    KItemQuantityUpdate                   m_kItemQuantityUpdate{};
    std::map<std::int64_t, std::int32_t>  m_kItemEnduranceUpdate{};  // KItemEnduranceUpdate
    std::map<std::int64_t, KItemPosition> m_kItemPositionUpdate{};  // KItemPositionUpdate

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iBeforeSummonPetUID)>>::put(w, m_iBeforeSummonPetUID);
        Codec<std::remove_cvref_t<decltype(m_iSummonPetUID)>>::put(w, m_iSummonPetUID);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_kItemEnduranceUpdate)>>::put(w, m_kItemEnduranceUpdate);
        Codec<std::remove_cvref_t<decltype(m_kItemPositionUpdate)>>::put(w, m_kItemPositionUpdate);
    }

    static KDBE_SUMMON_PET_REQ read_from(Reader& r) {
        KDBE_SUMMON_PET_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iBeforeSummonPetUID = Codec<std::remove_cvref_t<decltype(v.m_iBeforeSummonPetUID)>>::get(r);
        v.m_iSummonPetUID = Codec<std::remove_cvref_t<decltype(v.m_iSummonPetUID)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_kItemEnduranceUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemEnduranceUpdate)>>::get(r);
        v.m_kItemPositionUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemPositionUpdate)>>::get(r);
        return v;
    }
};

struct KDBE_TRADE_COMPLETE_ACK {
    std::int32_t                         m_iOK{};  // int
    std::int64_t                         m_iUnitUID{};  // UidType
    std::int32_t                         m_iED{};  // int
    KItemQuantityUpdate                  m_kItemQuantityUpdate{};
    std::vector<KInventoryItemInfo>      m_vecInventorySlotInfo{};  // std::vector< KInventoryItemInfo >
    std::map<std::int64_t, std::int32_t> m_mapUpdateFailed{};  // std::map< UidType, int >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iED)>>::put(w, m_iED);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecInventorySlotInfo)>>::put(w, m_vecInventorySlotInfo);
        Codec<std::remove_cvref_t<decltype(m_mapUpdateFailed)>>::put(w, m_mapUpdateFailed);
    }

    static KDBE_TRADE_COMPLETE_ACK read_from(Reader& r) {
        KDBE_TRADE_COMPLETE_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iED = Codec<std::remove_cvref_t<decltype(v.m_iED)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_vecInventorySlotInfo = Codec<std::remove_cvref_t<decltype(v.m_vecInventorySlotInfo)>>::get(r);
        v.m_mapUpdateFailed = Codec<std::remove_cvref_t<decltype(v.m_mapUpdateFailed)>>::get(r);
        return v;
    }
};

