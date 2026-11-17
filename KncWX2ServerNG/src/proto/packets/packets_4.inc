// ---------------------------------------------------------------------------
// packets_4.inc - GENERATED FILE, DO NOT EDIT BY HAND.
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

struct KELG_REGISTER_USER_REQ {
    std::u16string m_wstrUserID{};  // std::wstring
    std::int32_t   m_iChannelID{};  // int
    std::uint32_t  m_uiIP{};  // unsigned int
    std::uint8_t   m_byteCanTry{};  // BYTE
    std::uint8_t   m_byteGuestUser{};  // BYTE
    char           m_cAuthLevel{};
    std::uint8_t   m_ucChannelCode{};  // unsigned char
    std::string    m_strMachineID{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrUserID)>>::put(w, m_wstrUserID);
        Codec<std::remove_cvref_t<decltype(m_iChannelID)>>::put(w, m_iChannelID);
        Codec<std::remove_cvref_t<decltype(m_uiIP)>>::put(w, m_uiIP);
        Codec<std::remove_cvref_t<decltype(m_byteCanTry)>>::put(w, m_byteCanTry);
        Codec<std::remove_cvref_t<decltype(m_byteGuestUser)>>::put(w, m_byteGuestUser);
        Codec<std::remove_cvref_t<decltype(m_cAuthLevel)>>::put(w, m_cAuthLevel);
        Codec<std::remove_cvref_t<decltype(m_ucChannelCode)>>::put(w, m_ucChannelCode);
        Codec<std::remove_cvref_t<decltype(m_strMachineID)>>::put(w, m_strMachineID);
    }

    static KELG_REGISTER_USER_REQ read_from(Reader& r) {
        KELG_REGISTER_USER_REQ v;
        v.m_wstrUserID = Codec<std::remove_cvref_t<decltype(v.m_wstrUserID)>>::get(r);
        v.m_iChannelID = Codec<std::remove_cvref_t<decltype(v.m_iChannelID)>>::get(r);
        v.m_uiIP = Codec<std::remove_cvref_t<decltype(v.m_uiIP)>>::get(r);
        v.m_byteCanTry = Codec<std::remove_cvref_t<decltype(v.m_byteCanTry)>>::get(r);
        v.m_byteGuestUser = Codec<std::remove_cvref_t<decltype(v.m_byteGuestUser)>>::get(r);
        v.m_cAuthLevel = Codec<std::remove_cvref_t<decltype(v.m_cAuthLevel)>>::get(r);
        v.m_ucChannelCode = Codec<std::remove_cvref_t<decltype(v.m_ucChannelCode)>>::get(r);
        v.m_strMachineID = Codec<std::remove_cvref_t<decltype(v.m_strMachineID)>>::get(r);
        return v;
    }
};

struct KELG_REGISTRATION_GUILD_AD_ACK {
    std::int32_t   m_iOK{};  // int
    std::int16_t   m_sPeriod{};  // short
    std::u16string m_wstrAdMessage{};  // std::wstring
    std::int32_t   m_iCost{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_sPeriod)>>::put(w, m_sPeriod);
        Codec<std::remove_cvref_t<decltype(m_wstrAdMessage)>>::put(w, m_wstrAdMessage);
        Codec<std::remove_cvref_t<decltype(m_iCost)>>::put(w, m_iCost);
    }

    static KELG_REGISTRATION_GUILD_AD_ACK read_from(Reader& r) {
        KELG_REGISTRATION_GUILD_AD_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_sPeriod = Codec<std::remove_cvref_t<decltype(v.m_sPeriod)>>::get(r);
        v.m_wstrAdMessage = Codec<std::remove_cvref_t<decltype(v.m_wstrAdMessage)>>::get(r);
        v.m_iCost = Codec<std::remove_cvref_t<decltype(v.m_iCost)>>::get(r);
        return v;
    }
};

struct KELG_REGISTRATION_GUILD_AD_REQ {
    std::int64_t   m_iUnitUID{};  // UidType
    std::int32_t   m_iGuildUID{};  // int
    std::int16_t   m_sPeriod{};  // short
    std::u16string m_wstrAdMessage{};  // std::wstring
    std::int32_t   m_iCost{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iGuildUID)>>::put(w, m_iGuildUID);
        Codec<std::remove_cvref_t<decltype(m_sPeriod)>>::put(w, m_sPeriod);
        Codec<std::remove_cvref_t<decltype(m_wstrAdMessage)>>::put(w, m_wstrAdMessage);
        Codec<std::remove_cvref_t<decltype(m_iCost)>>::put(w, m_iCost);
    }

    static KELG_REGISTRATION_GUILD_AD_REQ read_from(Reader& r) {
        KELG_REGISTRATION_GUILD_AD_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iGuildUID = Codec<std::remove_cvref_t<decltype(v.m_iGuildUID)>>::get(r);
        v.m_sPeriod = Codec<std::remove_cvref_t<decltype(v.m_sPeriod)>>::get(r);
        v.m_wstrAdMessage = Codec<std::remove_cvref_t<decltype(v.m_wstrAdMessage)>>::get(r);
        v.m_iCost = Codec<std::remove_cvref_t<decltype(v.m_iCost)>>::get(r);
        return v;
    }
};

struct KELG_REG_REJECTED_USER_NOT {
    std::int64_t m_iUserUID{};  // UidType
    char         m_cRejectedReason{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
        Codec<std::remove_cvref_t<decltype(m_cRejectedReason)>>::put(w, m_cRejectedReason);
    }

    static KELG_REG_REJECTED_USER_NOT read_from(Reader& r) {
        KELG_REG_REJECTED_USER_NOT v;
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        v.m_cRejectedReason = Codec<std::remove_cvref_t<decltype(v.m_cRejectedReason)>>::get(r);
        return v;
    }
};

struct KELG_RELATIONSHIP_INFO_REQ {
    std::int64_t m_iRequestGSUID{};  // UidType
    std::int64_t m_iRequestUnitUID{};  // UidType
    std::int64_t m_iAcceptUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iRequestGSUID)>>::put(w, m_iRequestGSUID);
        Codec<std::remove_cvref_t<decltype(m_iRequestUnitUID)>>::put(w, m_iRequestUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iAcceptUnitUID)>>::put(w, m_iAcceptUnitUID);
    }

    static KELG_RELATIONSHIP_INFO_REQ read_from(Reader& r) {
        KELG_RELATIONSHIP_INFO_REQ v;
        v.m_iRequestGSUID = Codec<std::remove_cvref_t<decltype(v.m_iRequestGSUID)>>::get(r);
        v.m_iRequestUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iRequestUnitUID)>>::get(r);
        v.m_iAcceptUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iAcceptUnitUID)>>::get(r);
        return v;
    }
};

struct KELG_REQUEST_FRIEND_NOT {
    std::int64_t   m_iDestUnitUID{};  // UidType
    std::u16string m_wstrDestNickName{};  // std::wstring
    std::int64_t   m_iUnitUID{};  // UidType
    std::u16string m_wstrNickName{};  // std::wstring
    std::u16string m_wstrMessage{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iDestUnitUID)>>::put(w, m_iDestUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrDestNickName)>>::put(w, m_wstrDestNickName);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_wstrMessage)>>::put(w, m_wstrMessage);
    }

    static KELG_REQUEST_FRIEND_NOT read_from(Reader& r) {
        KELG_REQUEST_FRIEND_NOT v;
        v.m_iDestUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iDestUnitUID)>>::get(r);
        v.m_wstrDestNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrDestNickName)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_wstrMessage = Codec<std::remove_cvref_t<decltype(v.m_wstrMessage)>>::get(r);
        return v;
    }
};

struct KELG_SEARCH_BLACK_LIST_UNIT_ACK {
    std::int32_t m_iOK{};  // int
    std::int64_t m_iUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
    }

    static KELG_SEARCH_BLACK_LIST_UNIT_ACK read_from(Reader& r) {
        KELG_SEARCH_BLACK_LIST_UNIT_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        return v;
    }
};

struct KELG_SEARCH_BLACK_LIST_UNIT_REQ {
    std::u16string m_wstrNickName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
    }

    static KELG_SEARCH_BLACK_LIST_UNIT_REQ read_from(Reader& r) {
        KELG_SEARCH_BLACK_LIST_UNIT_REQ v;
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        return v;
    }
};

struct KELG_SEARCH_UNIT_ACK {
    std::int32_t        m_iOK{};  // int
    std::uint16_t       m_usEventID{};  // unsigned short
    KConnectionUnitInfo m_kCUnitInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_usEventID)>>::put(w, m_usEventID);
        Codec<std::remove_cvref_t<decltype(m_kCUnitInfo)>>::put(w, m_kCUnitInfo);
    }

    static KELG_SEARCH_UNIT_ACK read_from(Reader& r) {
        KELG_SEARCH_UNIT_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_usEventID = Codec<std::remove_cvref_t<decltype(v.m_usEventID)>>::get(r);
        v.m_kCUnitInfo = Codec<std::remove_cvref_t<decltype(v.m_kCUnitInfo)>>::get(r);
        return v;
    }
};

struct KELG_SEARCH_UNIT_REQ {
    std::uint16_t  m_usEventID{};  // unsigned short
    std::u16string m_wstrNickName{};  // std::wstring
    std::int64_t   m_iUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_usEventID)>>::put(w, m_usEventID);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
    }

    static KELG_SEARCH_UNIT_REQ read_from(Reader& r) {
        KELG_SEARCH_UNIT_REQ v;
        v.m_usEventID = Codec<std::remove_cvref_t<decltype(v.m_usEventID)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        return v;
    }
};

struct KELG_TRADE_BLOCK_NOT {
    bool m_bIsTradeBlock{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bIsTradeBlock)>>::put(w, m_bIsTradeBlock);
    }

    static KELG_TRADE_BLOCK_NOT read_from(Reader& r) {
        KELG_TRADE_BLOCK_NOT v;
        v.m_bIsTradeBlock = Codec<std::remove_cvref_t<decltype(v.m_bIsTradeBlock)>>::get(r);
        return v;
    }
};

struct KELG_UNBLOCK_FRIEND_NOT {
    std::int64_t m_iDestUnitUID{};  // UidType
    std::int64_t m_iUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iDestUnitUID)>>::put(w, m_iDestUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
    }

    static KELG_UNBLOCK_FRIEND_NOT read_from(Reader& r) {
        KELG_UNBLOCK_FRIEND_NOT v;
        v.m_iDestUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iDestUnitUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        return v;
    }
};

struct KELG_UNIT_DISCONNECT_NOT {
    std::int64_t           m_iUnitUID{};  // UidType
    std::set<std::int64_t> m_setFriendUID{};  // std::set< UidType >
    std::int32_t           m_iGuildUID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_setFriendUID)>>::put(w, m_setFriendUID);
        Codec<std::remove_cvref_t<decltype(m_iGuildUID)>>::put(w, m_iGuildUID);
    }

    static KELG_UNIT_DISCONNECT_NOT read_from(Reader& r) {
        KELG_UNIT_DISCONNECT_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_setFriendUID = Codec<std::remove_cvref_t<decltype(v.m_setFriendUID)>>::get(r);
        v.m_iGuildUID = Codec<std::remove_cvref_t<decltype(v.m_iGuildUID)>>::get(r);
        return v;
    }
};

struct KELG_UPDATE_DENY_FRIEND_SHIP_NOT {
    std::int64_t m_iUnitUID{};  // UidType
    bool         m_bDenyFriendShip{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_bDenyFriendShip)>>::put(w, m_bDenyFriendShip);
    }

    static KELG_UPDATE_DENY_FRIEND_SHIP_NOT read_from(Reader& r) {
        KELG_UPDATE_DENY_FRIEND_SHIP_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_bDenyFriendShip = Codec<std::remove_cvref_t<decltype(v.m_bDenyFriendShip)>>::get(r);
        return v;
    }
};

struct KELG_UPDATE_USER_UNIT_BUFF_INFO_REQ {
    std::int64_t              m_iUnitUID{};  // UidType
    std::vector<KBuffInfo>    m_vecActivateBuff{};  // std::vector< KBuffInfo >
    std::vector<std::int32_t> m_vecDeActivateBuff{};  // std::vector< int >
    bool                      m_bSendAck{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_vecActivateBuff)>>::put(w, m_vecActivateBuff);
        Codec<std::remove_cvref_t<decltype(m_vecDeActivateBuff)>>::put(w, m_vecDeActivateBuff);
        Codec<std::remove_cvref_t<decltype(m_bSendAck)>>::put(w, m_bSendAck);
    }

    static KELG_UPDATE_USER_UNIT_BUFF_INFO_REQ read_from(Reader& r) {
        KELG_UPDATE_USER_UNIT_BUFF_INFO_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_vecActivateBuff = Codec<std::remove_cvref_t<decltype(v.m_vecActivateBuff)>>::get(r);
        v.m_vecDeActivateBuff = Codec<std::remove_cvref_t<decltype(v.m_vecDeActivateBuff)>>::get(r);
        v.m_bSendAck = Codec<std::remove_cvref_t<decltype(v.m_bSendAck)>>::get(r);
        return v;
    }
};

struct KELG_USER_DISCONNECT_REQ {
    std::uint16_t                        m_usEventID{};  // u_short
    std::map<std::int32_t, std::int32_t> m_mapAccCountInfo{};  // std::map< int, int >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_usEventID)>>::put(w, m_usEventID);
        Codec<std::remove_cvref_t<decltype(m_mapAccCountInfo)>>::put(w, m_mapAccCountInfo);
    }

    static KELG_USER_DISCONNECT_REQ read_from(Reader& r) {
        KELG_USER_DISCONNECT_REQ v;
        v.m_usEventID = Codec<std::remove_cvref_t<decltype(v.m_usEventID)>>::get(r);
        v.m_mapAccCountInfo = Codec<std::remove_cvref_t<decltype(v.m_mapAccCountInfo)>>::get(r);
        return v;
    }
};

struct KELG_VERIFY_GIANT_MATRIX_CARD_REQ {
    std::u16string m_wstrID{};  // std::wstring
    std::u16string m_wstrMatrix{};  // std::wstring
    std::uint32_t  m_uiGiantMatrixKey{};  // unsigned int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrID)>>::put(w, m_wstrID);
        Codec<std::remove_cvref_t<decltype(m_wstrMatrix)>>::put(w, m_wstrMatrix);
        Codec<std::remove_cvref_t<decltype(m_uiGiantMatrixKey)>>::put(w, m_uiGiantMatrixKey);
    }

    static KELG_VERIFY_GIANT_MATRIX_CARD_REQ read_from(Reader& r) {
        KELG_VERIFY_GIANT_MATRIX_CARD_REQ v;
        v.m_wstrID = Codec<std::remove_cvref_t<decltype(v.m_wstrID)>>::get(r);
        v.m_wstrMatrix = Codec<std::remove_cvref_t<decltype(v.m_wstrMatrix)>>::get(r);
        v.m_uiGiantMatrixKey = Codec<std::remove_cvref_t<decltype(v.m_uiGiantMatrixKey)>>::get(r);
        return v;
    }
};

struct KELOG_ABNORMAL_DISCONNECT {
    std::int64_t                          m_iServerUID{};  // UidType
    std::map<std::uint32_t, std::int32_t> m_mapAbnormalDisconnect{};  // std::map< DWORD, int >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iServerUID)>>::put(w, m_iServerUID);
        Codec<std::remove_cvref_t<decltype(m_mapAbnormalDisconnect)>>::put(w, m_mapAbnormalDisconnect);
    }

    static KELOG_ABNORMAL_DISCONNECT read_from(Reader& r) {
        KELOG_ABNORMAL_DISCONNECT v;
        v.m_iServerUID = Codec<std::remove_cvref_t<decltype(v.m_iServerUID)>>::get(r);
        v.m_mapAbnormalDisconnect = Codec<std::remove_cvref_t<decltype(v.m_mapAbnormalDisconnect)>>::get(r);
        return v;
    }
};

struct KELOG_ATTACH_ITEM_ENCHANT_LOG_NOT {
    std::int64_t m_iUnitUID{};  // UidType
    std::int32_t m_iAttachItemID{};  // int
    std::int64_t m_iDestItemUID{};  // UidType
    std::int32_t m_iBeforeEnchantLv{};  // int
    std::int32_t m_iAfterEnchantLv{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iAttachItemID)>>::put(w, m_iAttachItemID);
        Codec<std::remove_cvref_t<decltype(m_iDestItemUID)>>::put(w, m_iDestItemUID);
        Codec<std::remove_cvref_t<decltype(m_iBeforeEnchantLv)>>::put(w, m_iBeforeEnchantLv);
        Codec<std::remove_cvref_t<decltype(m_iAfterEnchantLv)>>::put(w, m_iAfterEnchantLv);
    }

    static KELOG_ATTACH_ITEM_ENCHANT_LOG_NOT read_from(Reader& r) {
        KELOG_ATTACH_ITEM_ENCHANT_LOG_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iAttachItemID = Codec<std::remove_cvref_t<decltype(v.m_iAttachItemID)>>::get(r);
        v.m_iDestItemUID = Codec<std::remove_cvref_t<decltype(v.m_iDestItemUID)>>::get(r);
        v.m_iBeforeEnchantLv = Codec<std::remove_cvref_t<decltype(v.m_iBeforeEnchantLv)>>::get(r);
        v.m_iAfterEnchantLv = Codec<std::remove_cvref_t<decltype(v.m_iAfterEnchantLv)>>::get(r);
        return v;
    }
};

struct KELOG_BATTLE_FIELD_LEAVE_LOG_NOT {
    std::u16string m_wstrServerIP{};  // std::wstring
    std::int32_t   m_iBattleFieldID{};  // int
    std::int64_t   m_iRoomUID{};  // UidType
    char           m_cLeaveType{};
    std::int64_t   m_iUnitUID{};  // UidType
    std::u16string m_wstrNickName{};  // std::wstring
    char           m_cUnitClass{};
    std::uint8_t   m_ucLevel{};  // u_char
    std::int32_t   m_iPlayTime{};  // int
    std::int32_t   m_iGetEXP{};  // int
    std::int32_t   m_iGetED{};  // int
    std::u16string m_wstrRegDate{};  // std::wstring
    std::int32_t   m_iFieldFrame{};  // int
    std::int16_t   m_sNpcDieCount{};  // short

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrServerIP)>>::put(w, m_wstrServerIP);
        Codec<std::remove_cvref_t<decltype(m_iBattleFieldID)>>::put(w, m_iBattleFieldID);
        Codec<std::remove_cvref_t<decltype(m_iRoomUID)>>::put(w, m_iRoomUID);
        Codec<std::remove_cvref_t<decltype(m_cLeaveType)>>::put(w, m_cLeaveType);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_cUnitClass)>>::put(w, m_cUnitClass);
        Codec<std::remove_cvref_t<decltype(m_ucLevel)>>::put(w, m_ucLevel);
        Codec<std::remove_cvref_t<decltype(m_iPlayTime)>>::put(w, m_iPlayTime);
        Codec<std::remove_cvref_t<decltype(m_iGetEXP)>>::put(w, m_iGetEXP);
        Codec<std::remove_cvref_t<decltype(m_iGetED)>>::put(w, m_iGetED);
        Codec<std::remove_cvref_t<decltype(m_wstrRegDate)>>::put(w, m_wstrRegDate);
        Codec<std::remove_cvref_t<decltype(m_iFieldFrame)>>::put(w, m_iFieldFrame);
        Codec<std::remove_cvref_t<decltype(m_sNpcDieCount)>>::put(w, m_sNpcDieCount);
    }

    static KELOG_BATTLE_FIELD_LEAVE_LOG_NOT read_from(Reader& r) {
        KELOG_BATTLE_FIELD_LEAVE_LOG_NOT v;
        v.m_wstrServerIP = Codec<std::remove_cvref_t<decltype(v.m_wstrServerIP)>>::get(r);
        v.m_iBattleFieldID = Codec<std::remove_cvref_t<decltype(v.m_iBattleFieldID)>>::get(r);
        v.m_iRoomUID = Codec<std::remove_cvref_t<decltype(v.m_iRoomUID)>>::get(r);
        v.m_cLeaveType = Codec<std::remove_cvref_t<decltype(v.m_cLeaveType)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_cUnitClass = Codec<std::remove_cvref_t<decltype(v.m_cUnitClass)>>::get(r);
        v.m_ucLevel = Codec<std::remove_cvref_t<decltype(v.m_ucLevel)>>::get(r);
        v.m_iPlayTime = Codec<std::remove_cvref_t<decltype(v.m_iPlayTime)>>::get(r);
        v.m_iGetEXP = Codec<std::remove_cvref_t<decltype(v.m_iGetEXP)>>::get(r);
        v.m_iGetED = Codec<std::remove_cvref_t<decltype(v.m_iGetED)>>::get(r);
        v.m_wstrRegDate = Codec<std::remove_cvref_t<decltype(v.m_wstrRegDate)>>::get(r);
        v.m_iFieldFrame = Codec<std::remove_cvref_t<decltype(v.m_iFieldFrame)>>::get(r);
        v.m_sNpcDieCount = Codec<std::remove_cvref_t<decltype(v.m_sNpcDieCount)>>::get(r);
        return v;
    }
};

struct KELOG_BUY_CASH_ITEM_LOG_NOT {
    std::int64_t   m_iUserUID{};  // UidType
    std::int32_t   m_iServerGroupID{};  // int
    std::int64_t   m_iUnitUID{};  // UidType
    char           m_cUnitClass{};
    std::uint8_t   m_ucLevel{};  // u_char
    bool           m_bIsFirstBuy{};
    char           m_cBuyType{};
    std::int32_t   m_iProductNo{};  // int
    std::int32_t   m_iBuyQuantity{};  // int
    std::int32_t   m_iTotalSalePrice{};  // int
    std::u16string m_wstrRegDate{};  // std::wstring
    std::u16string m_wstrReceiverUserID{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
        Codec<std::remove_cvref_t<decltype(m_iServerGroupID)>>::put(w, m_iServerGroupID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_cUnitClass)>>::put(w, m_cUnitClass);
        Codec<std::remove_cvref_t<decltype(m_ucLevel)>>::put(w, m_ucLevel);
        Codec<std::remove_cvref_t<decltype(m_bIsFirstBuy)>>::put(w, m_bIsFirstBuy);
        Codec<std::remove_cvref_t<decltype(m_cBuyType)>>::put(w, m_cBuyType);
        Codec<std::remove_cvref_t<decltype(m_iProductNo)>>::put(w, m_iProductNo);
        Codec<std::remove_cvref_t<decltype(m_iBuyQuantity)>>::put(w, m_iBuyQuantity);
        Codec<std::remove_cvref_t<decltype(m_iTotalSalePrice)>>::put(w, m_iTotalSalePrice);
        Codec<std::remove_cvref_t<decltype(m_wstrRegDate)>>::put(w, m_wstrRegDate);
        Codec<std::remove_cvref_t<decltype(m_wstrReceiverUserID)>>::put(w, m_wstrReceiverUserID);
    }

    static KELOG_BUY_CASH_ITEM_LOG_NOT read_from(Reader& r) {
        KELOG_BUY_CASH_ITEM_LOG_NOT v;
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        v.m_iServerGroupID = Codec<std::remove_cvref_t<decltype(v.m_iServerGroupID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_cUnitClass = Codec<std::remove_cvref_t<decltype(v.m_cUnitClass)>>::get(r);
        v.m_ucLevel = Codec<std::remove_cvref_t<decltype(v.m_ucLevel)>>::get(r);
        v.m_bIsFirstBuy = Codec<std::remove_cvref_t<decltype(v.m_bIsFirstBuy)>>::get(r);
        v.m_cBuyType = Codec<std::remove_cvref_t<decltype(v.m_cBuyType)>>::get(r);
        v.m_iProductNo = Codec<std::remove_cvref_t<decltype(v.m_iProductNo)>>::get(r);
        v.m_iBuyQuantity = Codec<std::remove_cvref_t<decltype(v.m_iBuyQuantity)>>::get(r);
        v.m_iTotalSalePrice = Codec<std::remove_cvref_t<decltype(v.m_iTotalSalePrice)>>::get(r);
        v.m_wstrRegDate = Codec<std::remove_cvref_t<decltype(v.m_wstrRegDate)>>::get(r);
        v.m_wstrReceiverUserID = Codec<std::remove_cvref_t<decltype(v.m_wstrReceiverUserID)>>::get(r);
        return v;
    }
};

struct KELOG_CASH_ITEM_RESOLVE_LOG_NOT {
    std::int64_t m_iUnitUID{};  // UidType
    std::int64_t m_iItemUID{};  // UidType
    std::int32_t m_iResultQuantity{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_iResultQuantity)>>::put(w, m_iResultQuantity);
    }

    static KELOG_CASH_ITEM_RESOLVE_LOG_NOT read_from(Reader& r) {
        KELOG_CASH_ITEM_RESOLVE_LOG_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_iResultQuantity = Codec<std::remove_cvref_t<decltype(v.m_iResultQuantity)>>::get(r);
        return v;
    }
};

struct KELOG_CASH_PRODUCT_LIST_UPDATE_NOT {
    std::vector<KCashPruductInfo> m_vecCashProductList{};  // std::vector< KCashPruductInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecCashProductList)>>::put(w, m_vecCashProductList);
    }

    static KELOG_CASH_PRODUCT_LIST_UPDATE_NOT read_from(Reader& r) {
        KELOG_CASH_PRODUCT_LIST_UPDATE_NOT v;
        v.m_vecCashProductList = Codec<std::remove_cvref_t<decltype(v.m_vecCashProductList)>>::get(r);
        return v;
    }
};

struct KELOG_CHARGE_POINT_NOT {
    std::int32_t m_iOK{};  // int
    std::int32_t m_iChargedCash{};  // int
    std::int32_t m_iNowCash{};  // int
    std::int64_t m_iCharUID{};  // UidType
    std::int32_t m_iItemID{};  // int
    std::int64_t m_iItemUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iChargedCash)>>::put(w, m_iChargedCash);
        Codec<std::remove_cvref_t<decltype(m_iNowCash)>>::put(w, m_iNowCash);
        Codec<std::remove_cvref_t<decltype(m_iCharUID)>>::put(w, m_iCharUID);
        Codec<std::remove_cvref_t<decltype(m_iItemID)>>::put(w, m_iItemID);
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
    }

    static KELOG_CHARGE_POINT_NOT read_from(Reader& r) {
        KELOG_CHARGE_POINT_NOT v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iChargedCash = Codec<std::remove_cvref_t<decltype(v.m_iChargedCash)>>::get(r);
        v.m_iNowCash = Codec<std::remove_cvref_t<decltype(v.m_iNowCash)>>::get(r);
        v.m_iCharUID = Codec<std::remove_cvref_t<decltype(v.m_iCharUID)>>::get(r);
        v.m_iItemID = Codec<std::remove_cvref_t<decltype(v.m_iItemID)>>::get(r);
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        return v;
    }
};

struct KELOG_CHARM_ITEM_LOG_NOT {
    std::int64_t m_iUnitUID{};  // UidType
    std::int32_t m_iCharmItemID{};  // int
    std::int32_t m_iResultItemID{};  // int
    std::int32_t m_iQuantity{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iCharmItemID)>>::put(w, m_iCharmItemID);
        Codec<std::remove_cvref_t<decltype(m_iResultItemID)>>::put(w, m_iResultItemID);
        Codec<std::remove_cvref_t<decltype(m_iQuantity)>>::put(w, m_iQuantity);
    }

    static KELOG_CHARM_ITEM_LOG_NOT read_from(Reader& r) {
        KELOG_CHARM_ITEM_LOG_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iCharmItemID = Codec<std::remove_cvref_t<decltype(v.m_iCharmItemID)>>::get(r);
        v.m_iResultItemID = Codec<std::remove_cvref_t<decltype(v.m_iResultItemID)>>::get(r);
        v.m_iQuantity = Codec<std::remove_cvref_t<decltype(v.m_iQuantity)>>::get(r);
        return v;
    }
};

struct KELOG_CHAT_LOG_NOT {
    std::int64_t   m_nRoomUID{};  // UidType
    std::int64_t   m_nCharUID{};  // UidType
    std::int64_t   m_nUserUID{};  // UidType
    char           m_cChatType{};
    std::u16string m_strMsg{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_nRoomUID)>>::put(w, m_nRoomUID);
        Codec<std::remove_cvref_t<decltype(m_nCharUID)>>::put(w, m_nCharUID);
        Codec<std::remove_cvref_t<decltype(m_nUserUID)>>::put(w, m_nUserUID);
        Codec<std::remove_cvref_t<decltype(m_cChatType)>>::put(w, m_cChatType);
        Codec<std::remove_cvref_t<decltype(m_strMsg)>>::put(w, m_strMsg);
    }

    static KELOG_CHAT_LOG_NOT read_from(Reader& r) {
        KELOG_CHAT_LOG_NOT v;
        v.m_nRoomUID = Codec<std::remove_cvref_t<decltype(v.m_nRoomUID)>>::get(r);
        v.m_nCharUID = Codec<std::remove_cvref_t<decltype(v.m_nCharUID)>>::get(r);
        v.m_nUserUID = Codec<std::remove_cvref_t<decltype(v.m_nUserUID)>>::get(r);
        v.m_cChatType = Codec<std::remove_cvref_t<decltype(v.m_cChatType)>>::get(r);
        v.m_strMsg = Codec<std::remove_cvref_t<decltype(v.m_strMsg)>>::get(r);
        return v;
    }
};

struct KELOG_CHEAT_LOG_NOT {
    std::int64_t   m_iUserUID{};  // UidType
    std::int64_t   m_iUnitUID{};  // UidType
    char           m_cAuthLevel{};
    std::u16string m_wsCheatCode{};  // std::wstring
    std::u16string m_wsCheatContents{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_cAuthLevel)>>::put(w, m_cAuthLevel);
        Codec<std::remove_cvref_t<decltype(m_wsCheatCode)>>::put(w, m_wsCheatCode);
        Codec<std::remove_cvref_t<decltype(m_wsCheatContents)>>::put(w, m_wsCheatContents);
    }

    static KELOG_CHEAT_LOG_NOT read_from(Reader& r) {
        KELOG_CHEAT_LOG_NOT v;
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_cAuthLevel = Codec<std::remove_cvref_t<decltype(v.m_cAuthLevel)>>::get(r);
        v.m_wsCheatCode = Codec<std::remove_cvref_t<decltype(v.m_wsCheatCode)>>::get(r);
        v.m_wsCheatContents = Codec<std::remove_cvref_t<decltype(v.m_wsCheatContents)>>::get(r);
        return v;
    }
};

struct KELOG_COLLECTION_OF_RELAY_AND_P2P_INFO_NOT {
    std::vector<KDBUpdateGamePlayNetworkInfo> m_vecDBUpdateInfo{};  // std::vector< KDBUpdateGamePlayNetworkInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecDBUpdateInfo)>>::put(w, m_vecDBUpdateInfo);
    }

    static KELOG_COLLECTION_OF_RELAY_AND_P2P_INFO_NOT read_from(Reader& r) {
        KELOG_COLLECTION_OF_RELAY_AND_P2P_INFO_NOT v;
        v.m_vecDBUpdateInfo = Codec<std::remove_cvref_t<decltype(v.m_vecDBUpdateInfo)>>::get(r);
        return v;
    }
};

struct KELOG_ED_ABUSER_LOG_NOT {
    std::int64_t m_iUnitUID{};  // UidType
    std::int32_t m_iState{};  // int
    std::int32_t m_iBeforeED{};  // int
    std::int32_t m_iAfterED{};  // int
    std::int32_t m_iED{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iState)>>::put(w, m_iState);
        Codec<std::remove_cvref_t<decltype(m_iBeforeED)>>::put(w, m_iBeforeED);
        Codec<std::remove_cvref_t<decltype(m_iAfterED)>>::put(w, m_iAfterED);
        Codec<std::remove_cvref_t<decltype(m_iED)>>::put(w, m_iED);
    }

    static KELOG_ED_ABUSER_LOG_NOT read_from(Reader& r) {
        KELOG_ED_ABUSER_LOG_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iState = Codec<std::remove_cvref_t<decltype(v.m_iState)>>::get(r);
        v.m_iBeforeED = Codec<std::remove_cvref_t<decltype(v.m_iBeforeED)>>::get(r);
        v.m_iAfterED = Codec<std::remove_cvref_t<decltype(v.m_iAfterED)>>::get(r);
        v.m_iED = Codec<std::remove_cvref_t<decltype(v.m_iED)>>::get(r);
        return v;
    }
};

struct KELOG_ENCHANT_ABUSER_LOG_NOT {
    std::int64_t m_iUserUID{};  // UidType
    std::int64_t m_iUnitUID{};  // UidType
    std::int64_t m_iItemUID{};  // UidType
    std::int32_t m_iItemID{};  // int
    char         m_cEnchantLevel{};
    std::int32_t m_iEnchantResult{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_iItemID)>>::put(w, m_iItemID);
        Codec<std::remove_cvref_t<decltype(m_cEnchantLevel)>>::put(w, m_cEnchantLevel);
        Codec<std::remove_cvref_t<decltype(m_iEnchantResult)>>::put(w, m_iEnchantResult);
    }

    static KELOG_ENCHANT_ABUSER_LOG_NOT read_from(Reader& r) {
        KELOG_ENCHANT_ABUSER_LOG_NOT v;
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_iItemID = Codec<std::remove_cvref_t<decltype(v.m_iItemID)>>::get(r);
        v.m_cEnchantLevel = Codec<std::remove_cvref_t<decltype(v.m_cEnchantLevel)>>::get(r);
        v.m_iEnchantResult = Codec<std::remove_cvref_t<decltype(v.m_iEnchantResult)>>::get(r);
        return v;
    }
};

struct KELOG_ENCHANT_ITEM_BREAK_LOG_NOT {
    std::int64_t m_iUnitUID{};  // UidType
    std::int64_t m_iItemUID{};  // UidType
    std::int32_t m_iItemID{};  // int
    char         m_cEnchantLevel{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_iItemID)>>::put(w, m_iItemID);
        Codec<std::remove_cvref_t<decltype(m_cEnchantLevel)>>::put(w, m_cEnchantLevel);
    }

    static KELOG_ENCHANT_ITEM_BREAK_LOG_NOT read_from(Reader& r) {
        KELOG_ENCHANT_ITEM_BREAK_LOG_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_iItemID = Codec<std::remove_cvref_t<decltype(v.m_iItemID)>>::get(r);
        v.m_cEnchantLevel = Codec<std::remove_cvref_t<decltype(v.m_cEnchantLevel)>>::get(r);
        return v;
    }
};

struct KELOG_ENCHANT_SUPPORT_MATERIAL_LOG_NOT {
    std::int64_t m_iItemUID{};  // UidType
    char         m_cBeforeLevel{};
    char         m_cAfterLevel{};
    bool         m_bUseSupport{};
    bool         m_bUsePlus{};
    bool         m_bUseDestroyGuard{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_cBeforeLevel)>>::put(w, m_cBeforeLevel);
        Codec<std::remove_cvref_t<decltype(m_cAfterLevel)>>::put(w, m_cAfterLevel);
        Codec<std::remove_cvref_t<decltype(m_bUseSupport)>>::put(w, m_bUseSupport);
        Codec<std::remove_cvref_t<decltype(m_bUsePlus)>>::put(w, m_bUsePlus);
        Codec<std::remove_cvref_t<decltype(m_bUseDestroyGuard)>>::put(w, m_bUseDestroyGuard);
    }

    static KELOG_ENCHANT_SUPPORT_MATERIAL_LOG_NOT read_from(Reader& r) {
        KELOG_ENCHANT_SUPPORT_MATERIAL_LOG_NOT v;
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_cBeforeLevel = Codec<std::remove_cvref_t<decltype(v.m_cBeforeLevel)>>::get(r);
        v.m_cAfterLevel = Codec<std::remove_cvref_t<decltype(v.m_cAfterLevel)>>::get(r);
        v.m_bUseSupport = Codec<std::remove_cvref_t<decltype(v.m_bUseSupport)>>::get(r);
        v.m_bUsePlus = Codec<std::remove_cvref_t<decltype(v.m_bUsePlus)>>::get(r);
        v.m_bUseDestroyGuard = Codec<std::remove_cvref_t<decltype(v.m_bUseDestroyGuard)>>::get(r);
        return v;
    }
};

struct KELOG_EXPAND_BANK_INVENTORY_NOT {
    std::int64_t m_iUserUID{};  // UidType
    std::int64_t m_iUnitUID{};  // UidType
    std::int32_t m_iUnitLevel{};  // int
    std::int32_t m_iBeforeED{};  // int
    std::int32_t m_iUseED{};  // int
    std::int32_t m_iCategory{};  // int
    std::int32_t m_iNumSlot{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitLevel)>>::put(w, m_iUnitLevel);
        Codec<std::remove_cvref_t<decltype(m_iBeforeED)>>::put(w, m_iBeforeED);
        Codec<std::remove_cvref_t<decltype(m_iUseED)>>::put(w, m_iUseED);
        Codec<std::remove_cvref_t<decltype(m_iCategory)>>::put(w, m_iCategory);
        Codec<std::remove_cvref_t<decltype(m_iNumSlot)>>::put(w, m_iNumSlot);
    }

    static KELOG_EXPAND_BANK_INVENTORY_NOT read_from(Reader& r) {
        KELOG_EXPAND_BANK_INVENTORY_NOT v;
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iUnitLevel = Codec<std::remove_cvref_t<decltype(v.m_iUnitLevel)>>::get(r);
        v.m_iBeforeED = Codec<std::remove_cvref_t<decltype(v.m_iBeforeED)>>::get(r);
        v.m_iUseED = Codec<std::remove_cvref_t<decltype(v.m_iUseED)>>::get(r);
        v.m_iCategory = Codec<std::remove_cvref_t<decltype(v.m_iCategory)>>::get(r);
        v.m_iNumSlot = Codec<std::remove_cvref_t<decltype(v.m_iNumSlot)>>::get(r);
        return v;
    }
};

struct KELOG_EXPAND_SKILL_PAGE_NOT {
    std::int64_t   m_iUnitUID{};  // UidType
    std::uint8_t   m_ucLevel{};  // UCHAR
    std::uint8_t   m_ucUnitClass{};  // UCHAR
    std::uint8_t   m_ucBeforeTheNumberOfSkillPagesAvailable{};  // UCHAR
    std::uint8_t   m_ucPaymentType{};  // UCHAR
    std::u16string m_wstrSenderNickName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_ucLevel)>>::put(w, m_ucLevel);
        Codec<std::remove_cvref_t<decltype(m_ucUnitClass)>>::put(w, m_ucUnitClass);
        Codec<std::remove_cvref_t<decltype(m_ucBeforeTheNumberOfSkillPagesAvailable)>>::put(w, m_ucBeforeTheNumberOfSkillPagesAvailable);
        Codec<std::remove_cvref_t<decltype(m_ucPaymentType)>>::put(w, m_ucPaymentType);
        Codec<std::remove_cvref_t<decltype(m_wstrSenderNickName)>>::put(w, m_wstrSenderNickName);
    }

    static KELOG_EXPAND_SKILL_PAGE_NOT read_from(Reader& r) {
        KELOG_EXPAND_SKILL_PAGE_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_ucLevel = Codec<std::remove_cvref_t<decltype(v.m_ucLevel)>>::get(r);
        v.m_ucUnitClass = Codec<std::remove_cvref_t<decltype(v.m_ucUnitClass)>>::get(r);
        v.m_ucBeforeTheNumberOfSkillPagesAvailable = Codec<std::remove_cvref_t<decltype(v.m_ucBeforeTheNumberOfSkillPagesAvailable)>>::get(r);
        v.m_ucPaymentType = Codec<std::remove_cvref_t<decltype(v.m_ucPaymentType)>>::get(r);
        v.m_wstrSenderNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrSenderNickName)>>::get(r);
        return v;
    }
};

struct KELOG_GET_PURCHASED_CASH_ITEM_NOT {
    std::int64_t m_iUnitUID{};  // UidType
    std::int32_t m_iItemID{};  // int
    std::int16_t m_sPeriod{};  // short
    std::int32_t m_iQuantity{};  // int
    std::uint8_t m_ucLevel{};  // u_char
    char         m_cUnitClass{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iItemID)>>::put(w, m_iItemID);
        Codec<std::remove_cvref_t<decltype(m_sPeriod)>>::put(w, m_sPeriod);
        Codec<std::remove_cvref_t<decltype(m_iQuantity)>>::put(w, m_iQuantity);
        Codec<std::remove_cvref_t<decltype(m_ucLevel)>>::put(w, m_ucLevel);
        Codec<std::remove_cvref_t<decltype(m_cUnitClass)>>::put(w, m_cUnitClass);
    }

    static KELOG_GET_PURCHASED_CASH_ITEM_NOT read_from(Reader& r) {
        KELOG_GET_PURCHASED_CASH_ITEM_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iItemID = Codec<std::remove_cvref_t<decltype(v.m_iItemID)>>::get(r);
        v.m_sPeriod = Codec<std::remove_cvref_t<decltype(v.m_sPeriod)>>::get(r);
        v.m_iQuantity = Codec<std::remove_cvref_t<decltype(v.m_iQuantity)>>::get(r);
        v.m_ucLevel = Codec<std::remove_cvref_t<decltype(v.m_ucLevel)>>::get(r);
        v.m_cUnitClass = Codec<std::remove_cvref_t<decltype(v.m_cUnitClass)>>::get(r);
        return v;
    }
};

struct KELOG_HENIR_DUNGEON_CLEAR_NOT {
    std::int64_t m_iUserUID{};  // UidType
    std::int64_t m_iUnitUID{};  // UidType
    std::int32_t m_iPlayTime{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iPlayTime)>>::put(w, m_iPlayTime);
    }

    static KELOG_HENIR_DUNGEON_CLEAR_NOT read_from(Reader& r) {
        KELOG_HENIR_DUNGEON_CLEAR_NOT v;
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iPlayTime = Codec<std::remove_cvref_t<decltype(v.m_iPlayTime)>>::get(r);
        return v;
    }
};

struct KELOG_HIGH_ENCHANT_ITEM_LOG_NOT {
    std::int64_t   m_iUnitUID{};  // UidType
    std::int64_t   m_iItemUID{};  // UidType
    std::int32_t   m_iItemID{};  // int
    std::int32_t   m_iBeforeLevel{};  // int
    std::int32_t   m_iAfterLevel{};  // int
    bool           m_bSupportMaterial{};
    std::int32_t   m_iEnchantResult{};  // int
    std::u16string m_wstrTime{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_iItemID)>>::put(w, m_iItemID);
        Codec<std::remove_cvref_t<decltype(m_iBeforeLevel)>>::put(w, m_iBeforeLevel);
        Codec<std::remove_cvref_t<decltype(m_iAfterLevel)>>::put(w, m_iAfterLevel);
        Codec<std::remove_cvref_t<decltype(m_bSupportMaterial)>>::put(w, m_bSupportMaterial);
        Codec<std::remove_cvref_t<decltype(m_iEnchantResult)>>::put(w, m_iEnchantResult);
        Codec<std::remove_cvref_t<decltype(m_wstrTime)>>::put(w, m_wstrTime);
    }

    static KELOG_HIGH_ENCHANT_ITEM_LOG_NOT read_from(Reader& r) {
        KELOG_HIGH_ENCHANT_ITEM_LOG_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_iItemID = Codec<std::remove_cvref_t<decltype(v.m_iItemID)>>::get(r);
        v.m_iBeforeLevel = Codec<std::remove_cvref_t<decltype(v.m_iBeforeLevel)>>::get(r);
        v.m_iAfterLevel = Codec<std::remove_cvref_t<decltype(v.m_iAfterLevel)>>::get(r);
        v.m_bSupportMaterial = Codec<std::remove_cvref_t<decltype(v.m_bSupportMaterial)>>::get(r);
        v.m_iEnchantResult = Codec<std::remove_cvref_t<decltype(v.m_iEnchantResult)>>::get(r);
        v.m_wstrTime = Codec<std::remove_cvref_t<decltype(v.m_wstrTime)>>::get(r);
        return v;
    }
};

struct KELOG_INIT_SKILL_TREE_NOT {
    std::int64_t m_iUnitUID{};  // UidType
    std::int32_t m_iSPoint{};  // int
    std::int32_t m_iCSPoint{};  // int
    std::int64_t m_iItemUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iSPoint)>>::put(w, m_iSPoint);
        Codec<std::remove_cvref_t<decltype(m_iCSPoint)>>::put(w, m_iCSPoint);
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
    }

    static KELOG_INIT_SKILL_TREE_NOT read_from(Reader& r) {
        KELOG_INIT_SKILL_TREE_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iSPoint = Codec<std::remove_cvref_t<decltype(v.m_iSPoint)>>::get(r);
        v.m_iCSPoint = Codec<std::remove_cvref_t<decltype(v.m_iCSPoint)>>::get(r);
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        return v;
    }
};

struct KELOG_INSERT_ABUSER_LOG_NOT {
    std::int64_t   m_iUserUID{};  // UidType
    std::int64_t   m_iUnitUID{};  // UidType
    std::u16string m_wstrIP{};  // std::wstring
    std::int32_t   m_iGameType{};  // int
    float          m_fPlayTimeLimit{};
    bool           m_bPublic{};
    std::int32_t   m_iPlayWithMember{};  // int
    std::u16string m_wstrEndGameTime{};  // std::wstring
    std::int32_t   m_iPlayTime{};  // int
    std::int32_t   m_iRewardVP{};  // int
    std::int32_t   m_iRewardEXP{};  // int
    std::int32_t   m_iKillCount{};  // int
    std::int32_t   m_iDeathCount{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrIP)>>::put(w, m_wstrIP);
        Codec<std::remove_cvref_t<decltype(m_iGameType)>>::put(w, m_iGameType);
        Codec<std::remove_cvref_t<decltype(m_fPlayTimeLimit)>>::put(w, m_fPlayTimeLimit);
        Codec<std::remove_cvref_t<decltype(m_bPublic)>>::put(w, m_bPublic);
        Codec<std::remove_cvref_t<decltype(m_iPlayWithMember)>>::put(w, m_iPlayWithMember);
        Codec<std::remove_cvref_t<decltype(m_wstrEndGameTime)>>::put(w, m_wstrEndGameTime);
        Codec<std::remove_cvref_t<decltype(m_iPlayTime)>>::put(w, m_iPlayTime);
        Codec<std::remove_cvref_t<decltype(m_iRewardVP)>>::put(w, m_iRewardVP);
        Codec<std::remove_cvref_t<decltype(m_iRewardEXP)>>::put(w, m_iRewardEXP);
        Codec<std::remove_cvref_t<decltype(m_iKillCount)>>::put(w, m_iKillCount);
        Codec<std::remove_cvref_t<decltype(m_iDeathCount)>>::put(w, m_iDeathCount);
    }

    static KELOG_INSERT_ABUSER_LOG_NOT read_from(Reader& r) {
        KELOG_INSERT_ABUSER_LOG_NOT v;
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_wstrIP = Codec<std::remove_cvref_t<decltype(v.m_wstrIP)>>::get(r);
        v.m_iGameType = Codec<std::remove_cvref_t<decltype(v.m_iGameType)>>::get(r);
        v.m_fPlayTimeLimit = Codec<std::remove_cvref_t<decltype(v.m_fPlayTimeLimit)>>::get(r);
        v.m_bPublic = Codec<std::remove_cvref_t<decltype(v.m_bPublic)>>::get(r);
        v.m_iPlayWithMember = Codec<std::remove_cvref_t<decltype(v.m_iPlayWithMember)>>::get(r);
        v.m_wstrEndGameTime = Codec<std::remove_cvref_t<decltype(v.m_wstrEndGameTime)>>::get(r);
        v.m_iPlayTime = Codec<std::remove_cvref_t<decltype(v.m_iPlayTime)>>::get(r);
        v.m_iRewardVP = Codec<std::remove_cvref_t<decltype(v.m_iRewardVP)>>::get(r);
        v.m_iRewardEXP = Codec<std::remove_cvref_t<decltype(v.m_iRewardEXP)>>::get(r);
        v.m_iKillCount = Codec<std::remove_cvref_t<decltype(v.m_iKillCount)>>::get(r);
        v.m_iDeathCount = Codec<std::remove_cvref_t<decltype(v.m_iDeathCount)>>::get(r);
        return v;
    }
};

struct KELOG_INSERT_CASH_SKILL_POINT_NOT {
    std::int64_t m_iUnitUID{};  // UidType
    std::int32_t m_iUnitLevel{};  // int
    std::int32_t m_iMaxCSPoint{};  // int
    std::int32_t m_iPeriod{};  // int
    bool         m_bUpdateEndDateOnly{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitLevel)>>::put(w, m_iUnitLevel);
        Codec<std::remove_cvref_t<decltype(m_iMaxCSPoint)>>::put(w, m_iMaxCSPoint);
        Codec<std::remove_cvref_t<decltype(m_iPeriod)>>::put(w, m_iPeriod);
        Codec<std::remove_cvref_t<decltype(m_bUpdateEndDateOnly)>>::put(w, m_bUpdateEndDateOnly);
    }

    static KELOG_INSERT_CASH_SKILL_POINT_NOT read_from(Reader& r) {
        KELOG_INSERT_CASH_SKILL_POINT_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iUnitLevel = Codec<std::remove_cvref_t<decltype(v.m_iUnitLevel)>>::get(r);
        v.m_iMaxCSPoint = Codec<std::remove_cvref_t<decltype(v.m_iMaxCSPoint)>>::get(r);
        v.m_iPeriod = Codec<std::remove_cvref_t<decltype(v.m_iPeriod)>>::get(r);
        v.m_bUpdateEndDateOnly = Codec<std::remove_cvref_t<decltype(v.m_bUpdateEndDateOnly)>>::get(r);
        return v;
    }
};

struct KELOG_INSERT_PERSONAL_SHOP_LOG_NOT {
    bool         m_bIsSquare{};
    char         m_cPShopType{};
    std::int64_t m_iFromUnitUID{};  // UidType
    std::int64_t m_iToUnitUID{};  // UidType
    std::int64_t m_iItemUID{};  // UidType
    std::int32_t m_iItemID{};  // int
    std::int32_t m_iQuantity{};  // int
    std::int32_t m_iED{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bIsSquare)>>::put(w, m_bIsSquare);
        Codec<std::remove_cvref_t<decltype(m_cPShopType)>>::put(w, m_cPShopType);
        Codec<std::remove_cvref_t<decltype(m_iFromUnitUID)>>::put(w, m_iFromUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iToUnitUID)>>::put(w, m_iToUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_iItemID)>>::put(w, m_iItemID);
        Codec<std::remove_cvref_t<decltype(m_iQuantity)>>::put(w, m_iQuantity);
        Codec<std::remove_cvref_t<decltype(m_iED)>>::put(w, m_iED);
    }

    static KELOG_INSERT_PERSONAL_SHOP_LOG_NOT read_from(Reader& r) {
        KELOG_INSERT_PERSONAL_SHOP_LOG_NOT v;
        v.m_bIsSquare = Codec<std::remove_cvref_t<decltype(v.m_bIsSquare)>>::get(r);
        v.m_cPShopType = Codec<std::remove_cvref_t<decltype(v.m_cPShopType)>>::get(r);
        v.m_iFromUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iFromUnitUID)>>::get(r);
        v.m_iToUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iToUnitUID)>>::get(r);
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_iItemID = Codec<std::remove_cvref_t<decltype(v.m_iItemID)>>::get(r);
        v.m_iQuantity = Codec<std::remove_cvref_t<decltype(v.m_iQuantity)>>::get(r);
        v.m_iED = Codec<std::remove_cvref_t<decltype(v.m_iED)>>::get(r);
        return v;
    }
};

struct KELOG_INSERT_TRADE_LOG_NOT {
    std::int64_t   m_iFromUserUID{};  // UidType
    std::int64_t   m_iFromUnitUID{};  // UidType
    std::int64_t   m_iToUserUID{};  // UidType
    std::int64_t   m_iToUnitUID{};  // UidType
    std::int64_t   m_iItemUID{};  // UidType
    std::int32_t   m_iItemID{};  // int
    std::int32_t   m_iQuantity{};  // int
    std::u16string m_wstrRegDate{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iFromUserUID)>>::put(w, m_iFromUserUID);
        Codec<std::remove_cvref_t<decltype(m_iFromUnitUID)>>::put(w, m_iFromUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iToUserUID)>>::put(w, m_iToUserUID);
        Codec<std::remove_cvref_t<decltype(m_iToUnitUID)>>::put(w, m_iToUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_iItemID)>>::put(w, m_iItemID);
        Codec<std::remove_cvref_t<decltype(m_iQuantity)>>::put(w, m_iQuantity);
        Codec<std::remove_cvref_t<decltype(m_wstrRegDate)>>::put(w, m_wstrRegDate);
    }

    static KELOG_INSERT_TRADE_LOG_NOT read_from(Reader& r) {
        KELOG_INSERT_TRADE_LOG_NOT v;
        v.m_iFromUserUID = Codec<std::remove_cvref_t<decltype(v.m_iFromUserUID)>>::get(r);
        v.m_iFromUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iFromUnitUID)>>::get(r);
        v.m_iToUserUID = Codec<std::remove_cvref_t<decltype(v.m_iToUserUID)>>::get(r);
        v.m_iToUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iToUnitUID)>>::get(r);
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_iItemID = Codec<std::remove_cvref_t<decltype(v.m_iItemID)>>::get(r);
        v.m_iQuantity = Codec<std::remove_cvref_t<decltype(v.m_iQuantity)>>::get(r);
        v.m_wstrRegDate = Codec<std::remove_cvref_t<decltype(v.m_wstrRegDate)>>::get(r);
        return v;
    }
};

struct KELOG_ITEM_EXCHANGE_LOG_NOT {
    std::int64_t m_iUnitUID{};  // UidType
    std::int64_t m_iDeletedItemUID{};  // UidType
    std::int32_t m_iDeletedItemID{};  // int
    std::int64_t m_iChangedItemUID{};  // UidType
    std::int32_t m_iChangedItemID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iDeletedItemUID)>>::put(w, m_iDeletedItemUID);
        Codec<std::remove_cvref_t<decltype(m_iDeletedItemID)>>::put(w, m_iDeletedItemID);
        Codec<std::remove_cvref_t<decltype(m_iChangedItemUID)>>::put(w, m_iChangedItemUID);
        Codec<std::remove_cvref_t<decltype(m_iChangedItemID)>>::put(w, m_iChangedItemID);
    }

    static KELOG_ITEM_EXCHANGE_LOG_NOT read_from(Reader& r) {
        KELOG_ITEM_EXCHANGE_LOG_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iDeletedItemUID = Codec<std::remove_cvref_t<decltype(v.m_iDeletedItemUID)>>::get(r);
        v.m_iDeletedItemID = Codec<std::remove_cvref_t<decltype(v.m_iDeletedItemID)>>::get(r);
        v.m_iChangedItemUID = Codec<std::remove_cvref_t<decltype(v.m_iChangedItemUID)>>::get(r);
        v.m_iChangedItemID = Codec<std::remove_cvref_t<decltype(v.m_iChangedItemID)>>::get(r);
        return v;
    }
};

struct KELOG_ITEM_TRADE_ERROR_NOT {
    std::int64_t   m_iUnitUID{};  // UidType
    std::int32_t   m_iType{};  // int
    std::u16string m_wsrtRegDate{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iType)>>::put(w, m_iType);
        Codec<std::remove_cvref_t<decltype(m_wsrtRegDate)>>::put(w, m_wsrtRegDate);
    }

    static KELOG_ITEM_TRADE_ERROR_NOT read_from(Reader& r) {
        KELOG_ITEM_TRADE_ERROR_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iType = Codec<std::remove_cvref_t<decltype(v.m_iType)>>::get(r);
        v.m_wsrtRegDate = Codec<std::remove_cvref_t<decltype(v.m_wsrtRegDate)>>::get(r);
        return v;
    }
};

struct KELOG_KOM_FILE_CHECK_LOG_NOT {
    std::u16string m_wstrInvaildKomName{};  // std::wstring
    std::int64_t   m_iUserUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrInvaildKomName)>>::put(w, m_wstrInvaildKomName);
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
    }

    static KELOG_KOM_FILE_CHECK_LOG_NOT read_from(Reader& r) {
        KELOG_KOM_FILE_CHECK_LOG_NOT v;
        v.m_wstrInvaildKomName = Codec<std::remove_cvref_t<decltype(v.m_wstrInvaildKomName)>>::get(r);
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        return v;
    }
};

struct KELOG_MEGAPHONE_LOG_NOT {
    std::int64_t m_iUnitUID{};  // UidType
    std::int32_t m_iItemID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iItemID)>>::put(w, m_iItemID);
    }

    static KELOG_MEGAPHONE_LOG_NOT read_from(Reader& r) {
        KELOG_MEGAPHONE_LOG_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iItemID = Codec<std::remove_cvref_t<decltype(v.m_iItemID)>>::get(r);
        return v;
    }
};

struct KELOG_RESOLVE_JACKPOT_LOG_NOT {
    std::int64_t   m_UnitUID{};  // UidType
    std::int32_t   m_iResolveItemID{};  // int
    std::int32_t   m_iElapsedTime{};  // int
    std::u16string m_wstrRegDate{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_UnitUID)>>::put(w, m_UnitUID);
        Codec<std::remove_cvref_t<decltype(m_iResolveItemID)>>::put(w, m_iResolveItemID);
        Codec<std::remove_cvref_t<decltype(m_iElapsedTime)>>::put(w, m_iElapsedTime);
        Codec<std::remove_cvref_t<decltype(m_wstrRegDate)>>::put(w, m_wstrRegDate);
    }

    static KELOG_RESOLVE_JACKPOT_LOG_NOT read_from(Reader& r) {
        KELOG_RESOLVE_JACKPOT_LOG_NOT v;
        v.m_UnitUID = Codec<std::remove_cvref_t<decltype(v.m_UnitUID)>>::get(r);
        v.m_iResolveItemID = Codec<std::remove_cvref_t<decltype(v.m_iResolveItemID)>>::get(r);
        v.m_iElapsedTime = Codec<std::remove_cvref_t<decltype(v.m_iElapsedTime)>>::get(r);
        v.m_wstrRegDate = Codec<std::remove_cvref_t<decltype(v.m_wstrRegDate)>>::get(r);
        return v;
    }
};

struct KELOG_RESTORE_ITEM_LOG_NOT {
    std::int64_t m_iUnitUID{};  // UidType
    std::int64_t m_iItemUID{};  // UidType
    std::int32_t m_iBeforeEnchantLv{};  // int
    std::int32_t m_iAfterEnchantLv{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_iBeforeEnchantLv)>>::put(w, m_iBeforeEnchantLv);
        Codec<std::remove_cvref_t<decltype(m_iAfterEnchantLv)>>::put(w, m_iAfterEnchantLv);
    }

    static KELOG_RESTORE_ITEM_LOG_NOT read_from(Reader& r) {
        KELOG_RESTORE_ITEM_LOG_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_iBeforeEnchantLv = Codec<std::remove_cvref_t<decltype(v.m_iBeforeEnchantLv)>>::get(r);
        v.m_iAfterEnchantLv = Codec<std::remove_cvref_t<decltype(v.m_iAfterEnchantLv)>>::get(r);
        return v;
    }
};

struct KELOG_RESURRECTION_STONE_LOG_NOT {
    std::int64_t   m_UnitUID{};  // UidType
    std::int32_t   m_iDungeonID{};  // int
    std::u16string m_wstrRegDate{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_UnitUID)>>::put(w, m_UnitUID);
        Codec<std::remove_cvref_t<decltype(m_iDungeonID)>>::put(w, m_iDungeonID);
        Codec<std::remove_cvref_t<decltype(m_wstrRegDate)>>::put(w, m_wstrRegDate);
    }

    static KELOG_RESURRECTION_STONE_LOG_NOT read_from(Reader& r) {
        KELOG_RESURRECTION_STONE_LOG_NOT v;
        v.m_UnitUID = Codec<std::remove_cvref_t<decltype(v.m_UnitUID)>>::get(r);
        v.m_iDungeonID = Codec<std::remove_cvref_t<decltype(v.m_iDungeonID)>>::get(r);
        v.m_wstrRegDate = Codec<std::remove_cvref_t<decltype(v.m_wstrRegDate)>>::get(r);
        return v;
    }
};

struct KELOG_STAT_PLAY_TIME {
    std::int64_t   m_iUserUID{};  // UidType
    std::u16string m_wstrIP{};  // std::wstring
    std::u16string m_wstrLoginTime{};  // std::wstring
    std::u16string m_wstrLogoutTime{};  // std::wstring
    bool           m_bLoginFail{};
    bool           m_bLogoutFail{};
    std::int32_t   m_iTotalPlayTime{};  // int
    std::int32_t   m_iPvpPlayTime{};  // int
    std::int32_t   m_iDungeonPlayTime{};  // int
    std::int32_t   m_iFieldPlayTime{};  // int
    std::int32_t   m_iDisconnectReason{};  // int
    std::int32_t   m_iDisconnectUserFSM{};  // int
    std::int32_t   m_iServerGroupID{};  // int
    std::u16string m_wstrMachineID{};  // std::wstring
    bool           m_bIsPcbang{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
        Codec<std::remove_cvref_t<decltype(m_wstrIP)>>::put(w, m_wstrIP);
        Codec<std::remove_cvref_t<decltype(m_wstrLoginTime)>>::put(w, m_wstrLoginTime);
        Codec<std::remove_cvref_t<decltype(m_wstrLogoutTime)>>::put(w, m_wstrLogoutTime);
        Codec<std::remove_cvref_t<decltype(m_bLoginFail)>>::put(w, m_bLoginFail);
        Codec<std::remove_cvref_t<decltype(m_bLogoutFail)>>::put(w, m_bLogoutFail);
        Codec<std::remove_cvref_t<decltype(m_iTotalPlayTime)>>::put(w, m_iTotalPlayTime);
        Codec<std::remove_cvref_t<decltype(m_iPvpPlayTime)>>::put(w, m_iPvpPlayTime);
        Codec<std::remove_cvref_t<decltype(m_iDungeonPlayTime)>>::put(w, m_iDungeonPlayTime);
        Codec<std::remove_cvref_t<decltype(m_iFieldPlayTime)>>::put(w, m_iFieldPlayTime);
        Codec<std::remove_cvref_t<decltype(m_iDisconnectReason)>>::put(w, m_iDisconnectReason);
        Codec<std::remove_cvref_t<decltype(m_iDisconnectUserFSM)>>::put(w, m_iDisconnectUserFSM);
        Codec<std::remove_cvref_t<decltype(m_iServerGroupID)>>::put(w, m_iServerGroupID);
        Codec<std::remove_cvref_t<decltype(m_wstrMachineID)>>::put(w, m_wstrMachineID);
        Codec<std::remove_cvref_t<decltype(m_bIsPcbang)>>::put(w, m_bIsPcbang);
    }

    static KELOG_STAT_PLAY_TIME read_from(Reader& r) {
        KELOG_STAT_PLAY_TIME v;
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        v.m_wstrIP = Codec<std::remove_cvref_t<decltype(v.m_wstrIP)>>::get(r);
        v.m_wstrLoginTime = Codec<std::remove_cvref_t<decltype(v.m_wstrLoginTime)>>::get(r);
        v.m_wstrLogoutTime = Codec<std::remove_cvref_t<decltype(v.m_wstrLogoutTime)>>::get(r);
        v.m_bLoginFail = Codec<std::remove_cvref_t<decltype(v.m_bLoginFail)>>::get(r);
        v.m_bLogoutFail = Codec<std::remove_cvref_t<decltype(v.m_bLogoutFail)>>::get(r);
        v.m_iTotalPlayTime = Codec<std::remove_cvref_t<decltype(v.m_iTotalPlayTime)>>::get(r);
        v.m_iPvpPlayTime = Codec<std::remove_cvref_t<decltype(v.m_iPvpPlayTime)>>::get(r);
        v.m_iDungeonPlayTime = Codec<std::remove_cvref_t<decltype(v.m_iDungeonPlayTime)>>::get(r);
        v.m_iFieldPlayTime = Codec<std::remove_cvref_t<decltype(v.m_iFieldPlayTime)>>::get(r);
        v.m_iDisconnectReason = Codec<std::remove_cvref_t<decltype(v.m_iDisconnectReason)>>::get(r);
        v.m_iDisconnectUserFSM = Codec<std::remove_cvref_t<decltype(v.m_iDisconnectUserFSM)>>::get(r);
        v.m_iServerGroupID = Codec<std::remove_cvref_t<decltype(v.m_iServerGroupID)>>::get(r);
        v.m_wstrMachineID = Codec<std::remove_cvref_t<decltype(v.m_wstrMachineID)>>::get(r);
        v.m_bIsPcbang = Codec<std::remove_cvref_t<decltype(v.m_bIsPcbang)>>::get(r);
        return v;
    }
};

struct KELOG_TRAFFIC_ATTACKER_LOG_NOT {
    std::u16string m_wstrUserID{};  // std::wstring
    std::int32_t   m_iChannelID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrUserID)>>::put(w, m_wstrUserID);
        Codec<std::remove_cvref_t<decltype(m_iChannelID)>>::put(w, m_iChannelID);
    }

    static KELOG_TRAFFIC_ATTACKER_LOG_NOT read_from(Reader& r) {
        KELOG_TRAFFIC_ATTACKER_LOG_NOT v;
        v.m_wstrUserID = Codec<std::remove_cvref_t<decltype(v.m_wstrUserID)>>::get(r);
        v.m_iChannelID = Codec<std::remove_cvref_t<decltype(v.m_iChannelID)>>::get(r);
        return v;
    }
};

struct KELOG_UPDATE_STONE_NOT {
    std::int32_t m_iSupplyCnt{};  // int
    std::int32_t m_iQuantity{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iSupplyCnt)>>::put(w, m_iSupplyCnt);
        Codec<std::remove_cvref_t<decltype(m_iQuantity)>>::put(w, m_iQuantity);
    }

    static KELOG_UPDATE_STONE_NOT read_from(Reader& r) {
        KELOG_UPDATE_STONE_NOT v;
        v.m_iSupplyCnt = Codec<std::remove_cvref_t<decltype(v.m_iSupplyCnt)>>::get(r);
        v.m_iQuantity = Codec<std::remove_cvref_t<decltype(v.m_iQuantity)>>::get(r);
        return v;
    }
};

struct KELOG_USER_CHARACTER_LOG_NOT {
    std::int64_t   m_iUserUID{};  // UidType
    std::int64_t   m_iUnitUID{};  // UidType
    std::u16string m_wstrNickName{};  // std::wstring
    bool           m_bOnlyOneUnit{};
    std::int64_t   m_iServerUID{};  // UidType
    std::int16_t   m_sLogType{};  // short
    std::uint8_t   m_ucLevel{};  // u_char
    char           m_cUnitClass{};
    std::int32_t   m_iDungeonID{};  // int
    std::int32_t   m_iProductNo{};  // int
    std::int32_t   m_iTotalDungeonPlayTime{};  // int
    std::int32_t   m_iTotalPvpPlayTime{};  // int
    std::int32_t   m_iDurationTime{};  // int
    std::u16string m_wstrAccountCreateDate{};  // std::wstring
    std::u16string m_wstrUnitCreateDate{};  // std::wstring
    std::u16string m_wstrRegDate{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_bOnlyOneUnit)>>::put(w, m_bOnlyOneUnit);
        Codec<std::remove_cvref_t<decltype(m_iServerUID)>>::put(w, m_iServerUID);
        Codec<std::remove_cvref_t<decltype(m_sLogType)>>::put(w, m_sLogType);
        Codec<std::remove_cvref_t<decltype(m_ucLevel)>>::put(w, m_ucLevel);
        Codec<std::remove_cvref_t<decltype(m_cUnitClass)>>::put(w, m_cUnitClass);
        Codec<std::remove_cvref_t<decltype(m_iDungeonID)>>::put(w, m_iDungeonID);
        Codec<std::remove_cvref_t<decltype(m_iProductNo)>>::put(w, m_iProductNo);
        Codec<std::remove_cvref_t<decltype(m_iTotalDungeonPlayTime)>>::put(w, m_iTotalDungeonPlayTime);
        Codec<std::remove_cvref_t<decltype(m_iTotalPvpPlayTime)>>::put(w, m_iTotalPvpPlayTime);
        Codec<std::remove_cvref_t<decltype(m_iDurationTime)>>::put(w, m_iDurationTime);
        Codec<std::remove_cvref_t<decltype(m_wstrAccountCreateDate)>>::put(w, m_wstrAccountCreateDate);
        Codec<std::remove_cvref_t<decltype(m_wstrUnitCreateDate)>>::put(w, m_wstrUnitCreateDate);
        Codec<std::remove_cvref_t<decltype(m_wstrRegDate)>>::put(w, m_wstrRegDate);
    }

    static KELOG_USER_CHARACTER_LOG_NOT read_from(Reader& r) {
        KELOG_USER_CHARACTER_LOG_NOT v;
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_bOnlyOneUnit = Codec<std::remove_cvref_t<decltype(v.m_bOnlyOneUnit)>>::get(r);
        v.m_iServerUID = Codec<std::remove_cvref_t<decltype(v.m_iServerUID)>>::get(r);
        v.m_sLogType = Codec<std::remove_cvref_t<decltype(v.m_sLogType)>>::get(r);
        v.m_ucLevel = Codec<std::remove_cvref_t<decltype(v.m_ucLevel)>>::get(r);
        v.m_cUnitClass = Codec<std::remove_cvref_t<decltype(v.m_cUnitClass)>>::get(r);
        v.m_iDungeonID = Codec<std::remove_cvref_t<decltype(v.m_iDungeonID)>>::get(r);
        v.m_iProductNo = Codec<std::remove_cvref_t<decltype(v.m_iProductNo)>>::get(r);
        v.m_iTotalDungeonPlayTime = Codec<std::remove_cvref_t<decltype(v.m_iTotalDungeonPlayTime)>>::get(r);
        v.m_iTotalPvpPlayTime = Codec<std::remove_cvref_t<decltype(v.m_iTotalPvpPlayTime)>>::get(r);
        v.m_iDurationTime = Codec<std::remove_cvref_t<decltype(v.m_iDurationTime)>>::get(r);
        v.m_wstrAccountCreateDate = Codec<std::remove_cvref_t<decltype(v.m_wstrAccountCreateDate)>>::get(r);
        v.m_wstrUnitCreateDate = Codec<std::remove_cvref_t<decltype(v.m_wstrUnitCreateDate)>>::get(r);
        v.m_wstrRegDate = Codec<std::remove_cvref_t<decltype(v.m_wstrRegDate)>>::get(r);
        return v;
    }
};

struct KELOG_USER_STATISTICS {
    std::int64_t                                                               m_iUserUID{};  // UidType
    std::int64_t                                                               m_iUnitUID{};  // UidType
    std::map<std::pair<std::int32_t, std::int32_t>, std::vector<std::int32_t>> m_mapPvpStatistics{};  // std::map< std::pair< int, int >, std::vector< int > >
    std::map<std::pair<std::int32_t, std::int32_t>, std::vector<std::int32_t>> m_mapDungeonStatistics{};  // std::map< std::pair< int, int >, std::vector< int > >
    std::map<std::pair<std::int32_t, std::int32_t>, std::vector<std::int32_t>> m_mapTCenterStatistics{};  // std::map< std::pair< int, int >, std::vector< int > >
    std::map<std::pair<std::int32_t, std::int32_t>, std::vector<std::int32_t>> m_mapEtcStatistics{};  // std::map< std::pair< int, int >, std::vector< int > >
    std::map<std::pair<std::int32_t, std::int32_t>, std::vector<std::int32_t>> m_mapEDStatistics{};  // std::map< std::pair< int, int >, std::vector< int > >
    std::int32_t                                                               m_iAuthLevel{};  // int
    char                                                                       m_cUnitClass{};
    std::uint8_t                                                               m_ucLevel{};  // u_char
    std::int32_t                                                               m_iED{};  // int
    std::int32_t                                                               m_iVSPoint{};  // int
    std::int32_t                                                               m_iVSPointMax{};  // int
    std::u16string                                                             m_wstrLoginTime{};  // std::wstring
    std::u16string                                                             m_wstrLogoutTime{};  // std::wstring
    std::u16string                                                             m_wstrNickName{};  // std::wstring
    std::uint32_t                                                              m_uiGiantUID{};  // UINT32
    std::u16string                                                             m_wstrAccountCreateDate{};  // std::wstring
    std::int32_t                                                               m_iEXP{};  // int
    std::u16string                                                             m_wstrUnitCreateDate{};  // std::wstring
    std::int32_t                                                               m_iiDurationTime{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_mapPvpStatistics)>>::put(w, m_mapPvpStatistics);
        Codec<std::remove_cvref_t<decltype(m_mapDungeonStatistics)>>::put(w, m_mapDungeonStatistics);
        Codec<std::remove_cvref_t<decltype(m_mapTCenterStatistics)>>::put(w, m_mapTCenterStatistics);
        Codec<std::remove_cvref_t<decltype(m_mapEtcStatistics)>>::put(w, m_mapEtcStatistics);
        Codec<std::remove_cvref_t<decltype(m_mapEDStatistics)>>::put(w, m_mapEDStatistics);
        Codec<std::remove_cvref_t<decltype(m_iAuthLevel)>>::put(w, m_iAuthLevel);
        Codec<std::remove_cvref_t<decltype(m_cUnitClass)>>::put(w, m_cUnitClass);
        Codec<std::remove_cvref_t<decltype(m_ucLevel)>>::put(w, m_ucLevel);
        Codec<std::remove_cvref_t<decltype(m_iED)>>::put(w, m_iED);
        Codec<std::remove_cvref_t<decltype(m_iVSPoint)>>::put(w, m_iVSPoint);
        Codec<std::remove_cvref_t<decltype(m_iVSPointMax)>>::put(w, m_iVSPointMax);
        Codec<std::remove_cvref_t<decltype(m_wstrLoginTime)>>::put(w, m_wstrLoginTime);
        Codec<std::remove_cvref_t<decltype(m_wstrLogoutTime)>>::put(w, m_wstrLogoutTime);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_uiGiantUID)>>::put(w, m_uiGiantUID);
        Codec<std::remove_cvref_t<decltype(m_wstrAccountCreateDate)>>::put(w, m_wstrAccountCreateDate);
        Codec<std::remove_cvref_t<decltype(m_iEXP)>>::put(w, m_iEXP);
        Codec<std::remove_cvref_t<decltype(m_wstrUnitCreateDate)>>::put(w, m_wstrUnitCreateDate);
        Codec<std::remove_cvref_t<decltype(m_iiDurationTime)>>::put(w, m_iiDurationTime);
    }

    static KELOG_USER_STATISTICS read_from(Reader& r) {
        KELOG_USER_STATISTICS v;
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_mapPvpStatistics = Codec<std::remove_cvref_t<decltype(v.m_mapPvpStatistics)>>::get(r);
        v.m_mapDungeonStatistics = Codec<std::remove_cvref_t<decltype(v.m_mapDungeonStatistics)>>::get(r);
        v.m_mapTCenterStatistics = Codec<std::remove_cvref_t<decltype(v.m_mapTCenterStatistics)>>::get(r);
        v.m_mapEtcStatistics = Codec<std::remove_cvref_t<decltype(v.m_mapEtcStatistics)>>::get(r);
        v.m_mapEDStatistics = Codec<std::remove_cvref_t<decltype(v.m_mapEDStatistics)>>::get(r);
        v.m_iAuthLevel = Codec<std::remove_cvref_t<decltype(v.m_iAuthLevel)>>::get(r);
        v.m_cUnitClass = Codec<std::remove_cvref_t<decltype(v.m_cUnitClass)>>::get(r);
        v.m_ucLevel = Codec<std::remove_cvref_t<decltype(v.m_ucLevel)>>::get(r);
        v.m_iED = Codec<std::remove_cvref_t<decltype(v.m_iED)>>::get(r);
        v.m_iVSPoint = Codec<std::remove_cvref_t<decltype(v.m_iVSPoint)>>::get(r);
        v.m_iVSPointMax = Codec<std::remove_cvref_t<decltype(v.m_iVSPointMax)>>::get(r);
        v.m_wstrLoginTime = Codec<std::remove_cvref_t<decltype(v.m_wstrLoginTime)>>::get(r);
        v.m_wstrLogoutTime = Codec<std::remove_cvref_t<decltype(v.m_wstrLogoutTime)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_uiGiantUID = Codec<std::remove_cvref_t<decltype(v.m_uiGiantUID)>>::get(r);
        v.m_wstrAccountCreateDate = Codec<std::remove_cvref_t<decltype(v.m_wstrAccountCreateDate)>>::get(r);
        v.m_iEXP = Codec<std::remove_cvref_t<decltype(v.m_iEXP)>>::get(r);
        v.m_wstrUnitCreateDate = Codec<std::remove_cvref_t<decltype(v.m_wstrUnitCreateDate)>>::get(r);
        v.m_iiDurationTime = Codec<std::remove_cvref_t<decltype(v.m_iiDurationTime)>>::get(r);
        return v;
    }
};

struct KELOG_USE_SEAL_ITEM_NOT {
    std::int64_t m_iUnitUID{};  // UidType
    std::int64_t m_iDestItemUID{};  // UidType
    std::int64_t m_iSealItemUID{};  // UidType
    std::uint8_t m_ucSealCount{};  // u_char

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iDestItemUID)>>::put(w, m_iDestItemUID);
        Codec<std::remove_cvref_t<decltype(m_iSealItemUID)>>::put(w, m_iSealItemUID);
        Codec<std::remove_cvref_t<decltype(m_ucSealCount)>>::put(w, m_ucSealCount);
    }

    static KELOG_USE_SEAL_ITEM_NOT read_from(Reader& r) {
        KELOG_USE_SEAL_ITEM_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iDestItemUID = Codec<std::remove_cvref_t<decltype(v.m_iDestItemUID)>>::get(r);
        v.m_iSealItemUID = Codec<std::remove_cvref_t<decltype(v.m_iSealItemUID)>>::get(r);
        v.m_ucSealCount = Codec<std::remove_cvref_t<decltype(v.m_ucSealCount)>>::get(r);
        return v;
    }
};

struct KEMAIL_SEND_EMAIL_NOT {
    std::vector<std::u16string> m_vecRecvMailAddrList{};  // std::vector< std::wstring >
    std::u16string              m_wstrTitle{};  // std::wstring
    std::u16string              m_wstrDesc{};  // std::wstring
    std::vector<std::u16string> m_vecAttachFileNameList{};  // std::vector< std::wstring >
    std::vector<std::u16string> m_vecAttachFileFullPathList{};  // std::vector< std::wstring >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecRecvMailAddrList)>>::put(w, m_vecRecvMailAddrList);
        Codec<std::remove_cvref_t<decltype(m_wstrTitle)>>::put(w, m_wstrTitle);
        Codec<std::remove_cvref_t<decltype(m_wstrDesc)>>::put(w, m_wstrDesc);
        Codec<std::remove_cvref_t<decltype(m_vecAttachFileNameList)>>::put(w, m_vecAttachFileNameList);
        Codec<std::remove_cvref_t<decltype(m_vecAttachFileFullPathList)>>::put(w, m_vecAttachFileFullPathList);
    }

    static KEMAIL_SEND_EMAIL_NOT read_from(Reader& r) {
        KEMAIL_SEND_EMAIL_NOT v;
        v.m_vecRecvMailAddrList = Codec<std::remove_cvref_t<decltype(v.m_vecRecvMailAddrList)>>::get(r);
        v.m_wstrTitle = Codec<std::remove_cvref_t<decltype(v.m_wstrTitle)>>::get(r);
        v.m_wstrDesc = Codec<std::remove_cvref_t<decltype(v.m_wstrDesc)>>::get(r);
        v.m_vecAttachFileNameList = Codec<std::remove_cvref_t<decltype(v.m_vecAttachFileNameList)>>::get(r);
        v.m_vecAttachFileFullPathList = Codec<std::remove_cvref_t<decltype(v.m_vecAttachFileFullPathList)>>::get(r);
        return v;
    }
};

struct KENX_ADM_SEARCH_ACK {
    std::uint32_t m_dwTransactionID{};  // DWORD
    std::uint8_t  m_byteResult{};  // BYTE

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_dwTransactionID)>>::put(w, m_dwTransactionID);
        Codec<std::remove_cvref_t<decltype(m_byteResult)>>::put(w, m_byteResult);
    }

    static KENX_ADM_SEARCH_ACK read_from(Reader& r) {
        KENX_ADM_SEARCH_ACK v;
        v.m_dwTransactionID = Codec<std::remove_cvref_t<decltype(v.m_dwTransactionID)>>::get(r);
        v.m_byteResult = Codec<std::remove_cvref_t<decltype(v.m_byteResult)>>::get(r);
        return v;
    }
};

struct KENX_ADM_SEARCH_REQ {
    std::uint32_t  m_dwTransactionID{};  // DWORD
    std::u16string m_wstrUserID{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_dwTransactionID)>>::put(w, m_dwTransactionID);
        Codec<std::remove_cvref_t<decltype(m_wstrUserID)>>::put(w, m_wstrUserID);
    }

    static KENX_ADM_SEARCH_REQ read_from(Reader& r) {
        KENX_ADM_SEARCH_REQ v;
        v.m_dwTransactionID = Codec<std::remove_cvref_t<decltype(v.m_dwTransactionID)>>::get(r);
        v.m_wstrUserID = Codec<std::remove_cvref_t<decltype(v.m_wstrUserID)>>::get(r);
        return v;
    }
};

struct KENX_ALIVE_NOT {
    std::uint8_t m_bytePacketType{};  // BYTE

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bytePacketType)>>::put(w, m_bytePacketType);
    }

    static KENX_ALIVE_NOT read_from(Reader& r) {
        KENX_ALIVE_NOT v;
        v.m_bytePacketType = Codec<std::remove_cvref_t<decltype(v.m_bytePacketType)>>::get(r);
        return v;
    }
};

struct KENX_AUTH_ALIVE_NOT {
    std::uint8_t m_bytePacketType{};  // BYTE

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bytePacketType)>>::put(w, m_bytePacketType);
    }

    static KENX_AUTH_ALIVE_NOT read_from(Reader& r) {
        KENX_AUTH_ALIVE_NOT v;
        v.m_bytePacketType = Codec<std::remove_cvref_t<decltype(v.m_bytePacketType)>>::get(r);
        return v;
    }
};

struct KENX_AUTH_INITIALIZE_ACK {
    std::uint8_t   m_bytePacketType{};  // BYTE
    std::uint8_t   m_byteInitializeType{};  // BYTE
    std::uint8_t   m_byteResult{};  // BYTE
    std::uint8_t   m_byteDomainSN{};  // BYTE
    std::u16string m_wstrMessage{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bytePacketType)>>::put(w, m_bytePacketType);
        Codec<std::remove_cvref_t<decltype(m_byteInitializeType)>>::put(w, m_byteInitializeType);
        Codec<std::remove_cvref_t<decltype(m_byteResult)>>::put(w, m_byteResult);
        Codec<std::remove_cvref_t<decltype(m_byteDomainSN)>>::put(w, m_byteDomainSN);
        Codec<std::remove_cvref_t<decltype(m_wstrMessage)>>::put(w, m_wstrMessage);
    }

    static KENX_AUTH_INITIALIZE_ACK read_from(Reader& r) {
        KENX_AUTH_INITIALIZE_ACK v;
        v.m_bytePacketType = Codec<std::remove_cvref_t<decltype(v.m_bytePacketType)>>::get(r);
        v.m_byteInitializeType = Codec<std::remove_cvref_t<decltype(v.m_byteInitializeType)>>::get(r);
        v.m_byteResult = Codec<std::remove_cvref_t<decltype(v.m_byteResult)>>::get(r);
        v.m_byteDomainSN = Codec<std::remove_cvref_t<decltype(v.m_byteDomainSN)>>::get(r);
        v.m_wstrMessage = Codec<std::remove_cvref_t<decltype(v.m_wstrMessage)>>::get(r);
        return v;
    }
};

struct KENX_AUTH_INITIALIZE_REQ {
    std::uint8_t   m_bytePacketType{};  // BYTE
    std::uint8_t   m_byteInitializeType{};  // BYTE
    std::uint8_t   m_byteGameSN{};  // BYTE
    std::uint8_t   m_byteDomainSN{};  // BYTE
    std::u16string m_wstrDomainName{};  // std::wstring
    std::uint8_t   m_byteSynchronizeType{};  // BYTE
    std::uint32_t  m_ulSynchronizeCount{};  // unsigned long

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bytePacketType)>>::put(w, m_bytePacketType);
        Codec<std::remove_cvref_t<decltype(m_byteInitializeType)>>::put(w, m_byteInitializeType);
        Codec<std::remove_cvref_t<decltype(m_byteGameSN)>>::put(w, m_byteGameSN);
        Codec<std::remove_cvref_t<decltype(m_byteDomainSN)>>::put(w, m_byteDomainSN);
        Codec<std::remove_cvref_t<decltype(m_wstrDomainName)>>::put(w, m_wstrDomainName);
        Codec<std::remove_cvref_t<decltype(m_byteSynchronizeType)>>::put(w, m_byteSynchronizeType);
        Codec<std::remove_cvref_t<decltype(m_ulSynchronizeCount)>>::put(w, m_ulSynchronizeCount);
    }

    static KENX_AUTH_INITIALIZE_REQ read_from(Reader& r) {
        KENX_AUTH_INITIALIZE_REQ v;
        v.m_bytePacketType = Codec<std::remove_cvref_t<decltype(v.m_bytePacketType)>>::get(r);
        v.m_byteInitializeType = Codec<std::remove_cvref_t<decltype(v.m_byteInitializeType)>>::get(r);
        v.m_byteGameSN = Codec<std::remove_cvref_t<decltype(v.m_byteGameSN)>>::get(r);
        v.m_byteDomainSN = Codec<std::remove_cvref_t<decltype(v.m_byteDomainSN)>>::get(r);
        v.m_wstrDomainName = Codec<std::remove_cvref_t<decltype(v.m_wstrDomainName)>>::get(r);
        v.m_byteSynchronizeType = Codec<std::remove_cvref_t<decltype(v.m_byteSynchronizeType)>>::get(r);
        v.m_ulSynchronizeCount = Codec<std::remove_cvref_t<decltype(v.m_ulSynchronizeCount)>>::get(r);
        return v;
    }
};

struct KENX_AUTH_LOGOUT_NOT {
    std::uint8_t   m_bytePacketType{};  // BYTE
    std::uint8_t   m_byteLogoutType{};  // BYTE
    std::u16string m_wstrUserID{};  // std::wstring
    std::int64_t   m_iSessionNo{};  // __int64

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bytePacketType)>>::put(w, m_bytePacketType);
        Codec<std::remove_cvref_t<decltype(m_byteLogoutType)>>::put(w, m_byteLogoutType);
        Codec<std::remove_cvref_t<decltype(m_wstrUserID)>>::put(w, m_wstrUserID);
        Codec<std::remove_cvref_t<decltype(m_iSessionNo)>>::put(w, m_iSessionNo);
    }

    static KENX_AUTH_LOGOUT_NOT read_from(Reader& r) {
        KENX_AUTH_LOGOUT_NOT v;
        v.m_bytePacketType = Codec<std::remove_cvref_t<decltype(v.m_bytePacketType)>>::get(r);
        v.m_byteLogoutType = Codec<std::remove_cvref_t<decltype(v.m_byteLogoutType)>>::get(r);
        v.m_wstrUserID = Codec<std::remove_cvref_t<decltype(v.m_wstrUserID)>>::get(r);
        v.m_iSessionNo = Codec<std::remove_cvref_t<decltype(v.m_iSessionNo)>>::get(r);
        return v;
    }
};

struct KENX_AUTH_MESSAGE_NOT {
    std::uint8_t   m_bytePacketType{};  // BYTE
    std::uint8_t   m_byteMessageType{};  // BYTE
    std::int64_t   m_iSessionNo{};  // __int64
    std::u16string m_wstrUserID{};  // std::wstring
    std::u16string m_wstrCharacterName{};  // std::wstring
    std::uint8_t   m_byteOption{};  // BYTE
    std::uint32_t  m_ulArgument{};  // unsigned long
    std::uint32_t  m_ulSessionCount{};  // unsigned long

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bytePacketType)>>::put(w, m_bytePacketType);
        Codec<std::remove_cvref_t<decltype(m_byteMessageType)>>::put(w, m_byteMessageType);
        Codec<std::remove_cvref_t<decltype(m_iSessionNo)>>::put(w, m_iSessionNo);
        Codec<std::remove_cvref_t<decltype(m_wstrUserID)>>::put(w, m_wstrUserID);
        Codec<std::remove_cvref_t<decltype(m_wstrCharacterName)>>::put(w, m_wstrCharacterName);
        Codec<std::remove_cvref_t<decltype(m_byteOption)>>::put(w, m_byteOption);
        Codec<std::remove_cvref_t<decltype(m_ulArgument)>>::put(w, m_ulArgument);
        Codec<std::remove_cvref_t<decltype(m_ulSessionCount)>>::put(w, m_ulSessionCount);
    }

    static KENX_AUTH_MESSAGE_NOT read_from(Reader& r) {
        KENX_AUTH_MESSAGE_NOT v;
        v.m_bytePacketType = Codec<std::remove_cvref_t<decltype(v.m_bytePacketType)>>::get(r);
        v.m_byteMessageType = Codec<std::remove_cvref_t<decltype(v.m_byteMessageType)>>::get(r);
        v.m_iSessionNo = Codec<std::remove_cvref_t<decltype(v.m_iSessionNo)>>::get(r);
        v.m_wstrUserID = Codec<std::remove_cvref_t<decltype(v.m_wstrUserID)>>::get(r);
        v.m_wstrCharacterName = Codec<std::remove_cvref_t<decltype(v.m_wstrCharacterName)>>::get(r);
        v.m_byteOption = Codec<std::remove_cvref_t<decltype(v.m_byteOption)>>::get(r);
        v.m_ulArgument = Codec<std::remove_cvref_t<decltype(v.m_ulArgument)>>::get(r);
        v.m_ulSessionCount = Codec<std::remove_cvref_t<decltype(v.m_ulSessionCount)>>::get(r);
        return v;
    }
};

struct KENX_AUTH_SYNCHRONIZE_ACK {
    std::uint8_t                         m_bytePacketType{};  // BYTE
    std::uint8_t                         m_byteIsMonitoring{};  // BYTE
    std::map<std::int64_t, std::uint8_t> m_mapSynchronizeAck{};  // std::map< __int64, BYTE >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bytePacketType)>>::put(w, m_bytePacketType);
        Codec<std::remove_cvref_t<decltype(m_byteIsMonitoring)>>::put(w, m_byteIsMonitoring);
        Codec<std::remove_cvref_t<decltype(m_mapSynchronizeAck)>>::put(w, m_mapSynchronizeAck);
    }

    static KENX_AUTH_SYNCHRONIZE_ACK read_from(Reader& r) {
        KENX_AUTH_SYNCHRONIZE_ACK v;
        v.m_bytePacketType = Codec<std::remove_cvref_t<decltype(v.m_bytePacketType)>>::get(r);
        v.m_byteIsMonitoring = Codec<std::remove_cvref_t<decltype(v.m_byteIsMonitoring)>>::get(r);
        v.m_mapSynchronizeAck = Codec<std::remove_cvref_t<decltype(v.m_mapSynchronizeAck)>>::get(r);
        return v;
    }
};

struct KENX_AUTH_SYNCHRONIZE_REQ {
    std::uint8_t                           m_bytePacketType{};  // BYTE
    std::uint8_t                           m_byteIsMonitoring{};  // BYTE
    std::map<std::int64_t, std::u16string> m_mapSynchronizeReq{};  // std::map< __int64, std::wstring >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bytePacketType)>>::put(w, m_bytePacketType);
        Codec<std::remove_cvref_t<decltype(m_byteIsMonitoring)>>::put(w, m_byteIsMonitoring);
        Codec<std::remove_cvref_t<decltype(m_mapSynchronizeReq)>>::put(w, m_mapSynchronizeReq);
    }

    static KENX_AUTH_SYNCHRONIZE_REQ read_from(Reader& r) {
        KENX_AUTH_SYNCHRONIZE_REQ v;
        v.m_bytePacketType = Codec<std::remove_cvref_t<decltype(v.m_bytePacketType)>>::get(r);
        v.m_byteIsMonitoring = Codec<std::remove_cvref_t<decltype(v.m_byteIsMonitoring)>>::get(r);
        v.m_mapSynchronizeReq = Codec<std::remove_cvref_t<decltype(v.m_mapSynchronizeReq)>>::get(r);
        return v;
    }
};

struct KENX_AUTH_TERMINATE_NOT {
    std::uint8_t   m_bytePacketType{};  // BYTE
    std::uint8_t   m_byteTerminateType{};  // BYTE
    std::int64_t   m_iSessionNo{};  // __int64
    std::u16string m_wstrUserID{};  // std::wstring
    std::u16string m_wstrCharacterName{};  // std::wstring
    std::uint8_t   m_byteOption{};  // BYTE

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bytePacketType)>>::put(w, m_bytePacketType);
        Codec<std::remove_cvref_t<decltype(m_byteTerminateType)>>::put(w, m_byteTerminateType);
        Codec<std::remove_cvref_t<decltype(m_iSessionNo)>>::put(w, m_iSessionNo);
        Codec<std::remove_cvref_t<decltype(m_wstrUserID)>>::put(w, m_wstrUserID);
        Codec<std::remove_cvref_t<decltype(m_wstrCharacterName)>>::put(w, m_wstrCharacterName);
        Codec<std::remove_cvref_t<decltype(m_byteOption)>>::put(w, m_byteOption);
    }

    static KENX_AUTH_TERMINATE_NOT read_from(Reader& r) {
        KENX_AUTH_TERMINATE_NOT v;
        v.m_bytePacketType = Codec<std::remove_cvref_t<decltype(v.m_bytePacketType)>>::get(r);
        v.m_byteTerminateType = Codec<std::remove_cvref_t<decltype(v.m_byteTerminateType)>>::get(r);
        v.m_iSessionNo = Codec<std::remove_cvref_t<decltype(v.m_iSessionNo)>>::get(r);
        v.m_wstrUserID = Codec<std::remove_cvref_t<decltype(v.m_wstrUserID)>>::get(r);
        v.m_wstrCharacterName = Codec<std::remove_cvref_t<decltype(v.m_wstrCharacterName)>>::get(r);
        v.m_byteOption = Codec<std::remove_cvref_t<decltype(v.m_byteOption)>>::get(r);
        return v;
    }
};

struct KENX_BT_CATEGORY_INQUIRY_REQ {
    std::uint32_t m_ulPacketNo{};  // unsigned long
    std::uint8_t  m_bytePacketType{};  // BYTE

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_ulPacketNo)>>::put(w, m_ulPacketNo);
        Codec<std::remove_cvref_t<decltype(m_bytePacketType)>>::put(w, m_bytePacketType);
    }

    static KENX_BT_CATEGORY_INQUIRY_REQ read_from(Reader& r) {
        KENX_BT_CATEGORY_INQUIRY_REQ v;
        v.m_ulPacketNo = Codec<std::remove_cvref_t<decltype(v.m_ulPacketNo)>>::get(r);
        v.m_bytePacketType = Codec<std::remove_cvref_t<decltype(v.m_bytePacketType)>>::get(r);
        return v;
    }
};

struct KENX_BT_CHECK_AUTO_PAYMENT_USER_ACK {
    std::uint32_t m_ulPacketNo{};  // unsigned long
    std::uint8_t  m_bytePacketType{};  // BYTE
    std::uint32_t m_ulResult{};  // unsigned long

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_ulPacketNo)>>::put(w, m_ulPacketNo);
        Codec<std::remove_cvref_t<decltype(m_bytePacketType)>>::put(w, m_bytePacketType);
        Codec<std::remove_cvref_t<decltype(m_ulResult)>>::put(w, m_ulResult);
    }

    static KENX_BT_CHECK_AUTO_PAYMENT_USER_ACK read_from(Reader& r) {
        KENX_BT_CHECK_AUTO_PAYMENT_USER_ACK v;
        v.m_ulPacketNo = Codec<std::remove_cvref_t<decltype(v.m_ulPacketNo)>>::get(r);
        v.m_bytePacketType = Codec<std::remove_cvref_t<decltype(v.m_bytePacketType)>>::get(r);
        v.m_ulResult = Codec<std::remove_cvref_t<decltype(v.m_ulResult)>>::get(r);
        return v;
    }
};

struct KENX_BT_CHECK_AUTO_PAYMENT_USER_REQ {
    std::uint32_t  m_ulPacketNo{};  // unsigned long
    std::uint8_t   m_bytePacketType{};  // BYTE
    std::u16string m_wstrGameID{};  // std::wstring
    std::u16string m_wstrUserID{};  // std::wstring
    std::uint32_t  m_ulProductNo{};  // unsigned long

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_ulPacketNo)>>::put(w, m_ulPacketNo);
        Codec<std::remove_cvref_t<decltype(m_bytePacketType)>>::put(w, m_bytePacketType);
        Codec<std::remove_cvref_t<decltype(m_wstrGameID)>>::put(w, m_wstrGameID);
        Codec<std::remove_cvref_t<decltype(m_wstrUserID)>>::put(w, m_wstrUserID);
        Codec<std::remove_cvref_t<decltype(m_ulProductNo)>>::put(w, m_ulProductNo);
    }

    static KENX_BT_CHECK_AUTO_PAYMENT_USER_REQ read_from(Reader& r) {
        KENX_BT_CHECK_AUTO_PAYMENT_USER_REQ v;
        v.m_ulPacketNo = Codec<std::remove_cvref_t<decltype(v.m_ulPacketNo)>>::get(r);
        v.m_bytePacketType = Codec<std::remove_cvref_t<decltype(v.m_bytePacketType)>>::get(r);
        v.m_wstrGameID = Codec<std::remove_cvref_t<decltype(v.m_wstrGameID)>>::get(r);
        v.m_wstrUserID = Codec<std::remove_cvref_t<decltype(v.m_wstrUserID)>>::get(r);
        v.m_ulProductNo = Codec<std::remove_cvref_t<decltype(v.m_ulProductNo)>>::get(r);
        return v;
    }
};

struct KENX_BT_CHECK_BALANCE_ACK {
    std::uint32_t m_ulPacketNo{};  // unsigned long
    std::uint8_t  m_bytePacketType{};  // BYTE
    std::uint32_t m_ulResult{};  // unsigned long
    std::uint32_t m_ulBalance{};  // unsigned long

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_ulPacketNo)>>::put(w, m_ulPacketNo);
        Codec<std::remove_cvref_t<decltype(m_bytePacketType)>>::put(w, m_bytePacketType);
        Codec<std::remove_cvref_t<decltype(m_ulResult)>>::put(w, m_ulResult);
        Codec<std::remove_cvref_t<decltype(m_ulBalance)>>::put(w, m_ulBalance);
    }

    static KENX_BT_CHECK_BALANCE_ACK read_from(Reader& r) {
        KENX_BT_CHECK_BALANCE_ACK v;
        v.m_ulPacketNo = Codec<std::remove_cvref_t<decltype(v.m_ulPacketNo)>>::get(r);
        v.m_bytePacketType = Codec<std::remove_cvref_t<decltype(v.m_bytePacketType)>>::get(r);
        v.m_ulResult = Codec<std::remove_cvref_t<decltype(v.m_ulResult)>>::get(r);
        v.m_ulBalance = Codec<std::remove_cvref_t<decltype(v.m_ulBalance)>>::get(r);
        return v;
    }
};

struct KENX_BT_CHECK_BALANCE_REQ {
    std::uint32_t  m_ulPacketNo{};  // unsigned long
    std::uint8_t   m_bytePacketType{};  // BYTE
    std::uint32_t  m_ulPaymentType{};  // unsigned long
    std::u16string m_wstrUserID{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_ulPacketNo)>>::put(w, m_ulPacketNo);
        Codec<std::remove_cvref_t<decltype(m_bytePacketType)>>::put(w, m_bytePacketType);
        Codec<std::remove_cvref_t<decltype(m_ulPaymentType)>>::put(w, m_ulPaymentType);
        Codec<std::remove_cvref_t<decltype(m_wstrUserID)>>::put(w, m_wstrUserID);
    }

    static KENX_BT_CHECK_BALANCE_REQ read_from(Reader& r) {
        KENX_BT_CHECK_BALANCE_REQ v;
        v.m_ulPacketNo = Codec<std::remove_cvref_t<decltype(v.m_ulPacketNo)>>::get(r);
        v.m_bytePacketType = Codec<std::remove_cvref_t<decltype(v.m_bytePacketType)>>::get(r);
        v.m_ulPaymentType = Codec<std::remove_cvref_t<decltype(v.m_ulPaymentType)>>::get(r);
        v.m_wstrUserID = Codec<std::remove_cvref_t<decltype(v.m_wstrUserID)>>::get(r);
        return v;
    }
};

struct KENX_BT_COUPON_INQUIRY_NEW_REQ {
    std::uint32_t  m_ulPacketNo{};  // unsigned long
    std::uint8_t   m_bytePacketType{};  // BYTE
    std::uint32_t  m_ulUserIP{};  // unsigned long
    std::u16string m_wstrGameID{};  // std::wstring
    std::uint8_t   m_byteIsCafe{};  // BYTE
    std::u16string m_wstrUserID{};  // std::wstring
    std::uint32_t  m_ulUserOID{};  // unsigned long
    std::u16string m_wstrCouponString{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_ulPacketNo)>>::put(w, m_ulPacketNo);
        Codec<std::remove_cvref_t<decltype(m_bytePacketType)>>::put(w, m_bytePacketType);
        Codec<std::remove_cvref_t<decltype(m_ulUserIP)>>::put(w, m_ulUserIP);
        Codec<std::remove_cvref_t<decltype(m_wstrGameID)>>::put(w, m_wstrGameID);
        Codec<std::remove_cvref_t<decltype(m_byteIsCafe)>>::put(w, m_byteIsCafe);
        Codec<std::remove_cvref_t<decltype(m_wstrUserID)>>::put(w, m_wstrUserID);
        Codec<std::remove_cvref_t<decltype(m_ulUserOID)>>::put(w, m_ulUserOID);
        Codec<std::remove_cvref_t<decltype(m_wstrCouponString)>>::put(w, m_wstrCouponString);
    }

    static KENX_BT_COUPON_INQUIRY_NEW_REQ read_from(Reader& r) {
        KENX_BT_COUPON_INQUIRY_NEW_REQ v;
        v.m_ulPacketNo = Codec<std::remove_cvref_t<decltype(v.m_ulPacketNo)>>::get(r);
        v.m_bytePacketType = Codec<std::remove_cvref_t<decltype(v.m_bytePacketType)>>::get(r);
        v.m_ulUserIP = Codec<std::remove_cvref_t<decltype(v.m_ulUserIP)>>::get(r);
        v.m_wstrGameID = Codec<std::remove_cvref_t<decltype(v.m_wstrGameID)>>::get(r);
        v.m_byteIsCafe = Codec<std::remove_cvref_t<decltype(v.m_byteIsCafe)>>::get(r);
        v.m_wstrUserID = Codec<std::remove_cvref_t<decltype(v.m_wstrUserID)>>::get(r);
        v.m_ulUserOID = Codec<std::remove_cvref_t<decltype(v.m_ulUserOID)>>::get(r);
        v.m_wstrCouponString = Codec<std::remove_cvref_t<decltype(v.m_wstrCouponString)>>::get(r);
        return v;
    }
};

struct KENX_BT_COUPON_INQUIRY_REQ {
    std::uint32_t  m_ulPacketNo{};  // unsigned long
    std::uint8_t   m_bytePacketType{};  // BYTE
    std::uint8_t   m_byteIsCafe{};  // BYTE
    std::u16string m_wstrCouponString{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_ulPacketNo)>>::put(w, m_ulPacketNo);
        Codec<std::remove_cvref_t<decltype(m_bytePacketType)>>::put(w, m_bytePacketType);
        Codec<std::remove_cvref_t<decltype(m_byteIsCafe)>>::put(w, m_byteIsCafe);
        Codec<std::remove_cvref_t<decltype(m_wstrCouponString)>>::put(w, m_wstrCouponString);
    }

    static KENX_BT_COUPON_INQUIRY_REQ read_from(Reader& r) {
        KENX_BT_COUPON_INQUIRY_REQ v;
        v.m_ulPacketNo = Codec<std::remove_cvref_t<decltype(v.m_ulPacketNo)>>::get(r);
        v.m_bytePacketType = Codec<std::remove_cvref_t<decltype(v.m_bytePacketType)>>::get(r);
        v.m_byteIsCafe = Codec<std::remove_cvref_t<decltype(v.m_byteIsCafe)>>::get(r);
        v.m_wstrCouponString = Codec<std::remove_cvref_t<decltype(v.m_wstrCouponString)>>::get(r);
        return v;
    }
};

struct KENX_BT_COUPON_USING_REQ {
    std::uint32_t  m_ulPacketNo{};  // unsigned long
    std::uint8_t   m_bytePacketType{};  // BYTE
    std::uint32_t  m_ulRemoteIP{};  // unsigned long
    std::uint8_t   m_byteReason{};  // BYTE
    std::u16string m_wstrGameID{};  // std::wstring
    std::uint8_t   m_byteIsCafe{};  // BYTE
    std::u16string m_wstrUserID{};  // std::wstring
    std::uint32_t  m_ulUserOID{};  // unsigned long
    std::u16string m_wstrUserName{};  // std::wstring
    std::u16string m_wstrCouponString{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_ulPacketNo)>>::put(w, m_ulPacketNo);
        Codec<std::remove_cvref_t<decltype(m_bytePacketType)>>::put(w, m_bytePacketType);
        Codec<std::remove_cvref_t<decltype(m_ulRemoteIP)>>::put(w, m_ulRemoteIP);
        Codec<std::remove_cvref_t<decltype(m_byteReason)>>::put(w, m_byteReason);
        Codec<std::remove_cvref_t<decltype(m_wstrGameID)>>::put(w, m_wstrGameID);
        Codec<std::remove_cvref_t<decltype(m_byteIsCafe)>>::put(w, m_byteIsCafe);
        Codec<std::remove_cvref_t<decltype(m_wstrUserID)>>::put(w, m_wstrUserID);
        Codec<std::remove_cvref_t<decltype(m_ulUserOID)>>::put(w, m_ulUserOID);
        Codec<std::remove_cvref_t<decltype(m_wstrUserName)>>::put(w, m_wstrUserName);
        Codec<std::remove_cvref_t<decltype(m_wstrCouponString)>>::put(w, m_wstrCouponString);
    }

    static KENX_BT_COUPON_USING_REQ read_from(Reader& r) {
        KENX_BT_COUPON_USING_REQ v;
        v.m_ulPacketNo = Codec<std::remove_cvref_t<decltype(v.m_ulPacketNo)>>::get(r);
        v.m_bytePacketType = Codec<std::remove_cvref_t<decltype(v.m_bytePacketType)>>::get(r);
        v.m_ulRemoteIP = Codec<std::remove_cvref_t<decltype(v.m_ulRemoteIP)>>::get(r);
        v.m_byteReason = Codec<std::remove_cvref_t<decltype(v.m_byteReason)>>::get(r);
        v.m_wstrGameID = Codec<std::remove_cvref_t<decltype(v.m_wstrGameID)>>::get(r);
        v.m_byteIsCafe = Codec<std::remove_cvref_t<decltype(v.m_byteIsCafe)>>::get(r);
        v.m_wstrUserID = Codec<std::remove_cvref_t<decltype(v.m_wstrUserID)>>::get(r);
        v.m_ulUserOID = Codec<std::remove_cvref_t<decltype(v.m_ulUserOID)>>::get(r);
        v.m_wstrUserName = Codec<std::remove_cvref_t<decltype(v.m_wstrUserName)>>::get(r);
        v.m_wstrCouponString = Codec<std::remove_cvref_t<decltype(v.m_wstrCouponString)>>::get(r);
        return v;
    }
};

struct KENX_BT_HEART_BEAT_ACK {
    std::uint32_t m_ulPacketNo{};  // unsigned long
    std::uint8_t  m_bytePacketType{};  // BYTE
    std::uint32_t m_ulResult{};  // unsigned long

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_ulPacketNo)>>::put(w, m_ulPacketNo);
        Codec<std::remove_cvref_t<decltype(m_bytePacketType)>>::put(w, m_bytePacketType);
        Codec<std::remove_cvref_t<decltype(m_ulResult)>>::put(w, m_ulResult);
    }

    static KENX_BT_HEART_BEAT_ACK read_from(Reader& r) {
        KENX_BT_HEART_BEAT_ACK v;
        v.m_ulPacketNo = Codec<std::remove_cvref_t<decltype(v.m_ulPacketNo)>>::get(r);
        v.m_bytePacketType = Codec<std::remove_cvref_t<decltype(v.m_bytePacketType)>>::get(r);
        v.m_ulResult = Codec<std::remove_cvref_t<decltype(v.m_ulResult)>>::get(r);
        return v;
    }
};

struct KENX_BT_HEART_BEAT_REQ {
    std::uint32_t m_ulPacketNo{};  // unsigned long
    std::uint8_t  m_bytePacketType{};  // BYTE
    std::int64_t  m_iReleaseTick{};  // __int64

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_ulPacketNo)>>::put(w, m_ulPacketNo);
        Codec<std::remove_cvref_t<decltype(m_bytePacketType)>>::put(w, m_bytePacketType);
        Codec<std::remove_cvref_t<decltype(m_iReleaseTick)>>::put(w, m_iReleaseTick);
    }

    static KENX_BT_HEART_BEAT_REQ read_from(Reader& r) {
        KENX_BT_HEART_BEAT_REQ v;
        v.m_ulPacketNo = Codec<std::remove_cvref_t<decltype(v.m_ulPacketNo)>>::get(r);
        v.m_bytePacketType = Codec<std::remove_cvref_t<decltype(v.m_bytePacketType)>>::get(r);
        v.m_iReleaseTick = Codec<std::remove_cvref_t<decltype(v.m_iReleaseTick)>>::get(r);
        return v;
    }
};

struct KENX_BT_INITIALIZE_ACK {
    std::uint32_t  m_ulPacketNo{};  // unsigned long
    std::uint8_t   m_bytePacketType{};  // BYTE
    std::u16string m_wstrServiceCode{};  // std::wstring
    std::uint8_t   m_byteDomain{};  // BYTE
    std::uint32_t  m_ulResult{};  // unsigned long

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_ulPacketNo)>>::put(w, m_ulPacketNo);
        Codec<std::remove_cvref_t<decltype(m_bytePacketType)>>::put(w, m_bytePacketType);
        Codec<std::remove_cvref_t<decltype(m_wstrServiceCode)>>::put(w, m_wstrServiceCode);
        Codec<std::remove_cvref_t<decltype(m_byteDomain)>>::put(w, m_byteDomain);
        Codec<std::remove_cvref_t<decltype(m_ulResult)>>::put(w, m_ulResult);
    }

    static KENX_BT_INITIALIZE_ACK read_from(Reader& r) {
        KENX_BT_INITIALIZE_ACK v;
        v.m_ulPacketNo = Codec<std::remove_cvref_t<decltype(v.m_ulPacketNo)>>::get(r);
        v.m_bytePacketType = Codec<std::remove_cvref_t<decltype(v.m_bytePacketType)>>::get(r);
        v.m_wstrServiceCode = Codec<std::remove_cvref_t<decltype(v.m_wstrServiceCode)>>::get(r);
        v.m_byteDomain = Codec<std::remove_cvref_t<decltype(v.m_byteDomain)>>::get(r);
        v.m_ulResult = Codec<std::remove_cvref_t<decltype(v.m_ulResult)>>::get(r);
        return v;
    }
};

struct KENX_BT_INITIALIZE_REQ {
    std::uint32_t  m_ulPacketNo{};  // unsigned long
    std::uint8_t   m_bytePacketType{};  // BYTE
    std::u16string m_wstrServiceCode{};  // std::wstring
    std::uint8_t   m_byteDomain{};  // BYTE

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_ulPacketNo)>>::put(w, m_ulPacketNo);
        Codec<std::remove_cvref_t<decltype(m_bytePacketType)>>::put(w, m_bytePacketType);
        Codec<std::remove_cvref_t<decltype(m_wstrServiceCode)>>::put(w, m_wstrServiceCode);
        Codec<std::remove_cvref_t<decltype(m_byteDomain)>>::put(w, m_byteDomain);
    }

    static KENX_BT_INITIALIZE_REQ read_from(Reader& r) {
        KENX_BT_INITIALIZE_REQ v;
        v.m_ulPacketNo = Codec<std::remove_cvref_t<decltype(v.m_ulPacketNo)>>::get(r);
        v.m_bytePacketType = Codec<std::remove_cvref_t<decltype(v.m_bytePacketType)>>::get(r);
        v.m_wstrServiceCode = Codec<std::remove_cvref_t<decltype(v.m_wstrServiceCode)>>::get(r);
        v.m_byteDomain = Codec<std::remove_cvref_t<decltype(v.m_byteDomain)>>::get(r);
        return v;
    }
};

struct KENX_BT_NISMS_INVENTORY_CHECK_ACK {
    std::uint32_t m_ulPacketNo{};  // unsigned long
    std::uint8_t  m_bytePacketType{};  // BYTE
    std::uint32_t m_ulResult{};  // unsigned long
    std::uint8_t  m_byteOrderType{};  // BYTE
    std::uint32_t m_ulTotalCount{};  // unsigned long

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_ulPacketNo)>>::put(w, m_ulPacketNo);
        Codec<std::remove_cvref_t<decltype(m_bytePacketType)>>::put(w, m_bytePacketType);
        Codec<std::remove_cvref_t<decltype(m_ulResult)>>::put(w, m_ulResult);
        Codec<std::remove_cvref_t<decltype(m_byteOrderType)>>::put(w, m_byteOrderType);
        Codec<std::remove_cvref_t<decltype(m_ulTotalCount)>>::put(w, m_ulTotalCount);
    }

    static KENX_BT_NISMS_INVENTORY_CHECK_ACK read_from(Reader& r) {
        KENX_BT_NISMS_INVENTORY_CHECK_ACK v;
        v.m_ulPacketNo = Codec<std::remove_cvref_t<decltype(v.m_ulPacketNo)>>::get(r);
        v.m_bytePacketType = Codec<std::remove_cvref_t<decltype(v.m_bytePacketType)>>::get(r);
        v.m_ulResult = Codec<std::remove_cvref_t<decltype(v.m_ulResult)>>::get(r);
        v.m_byteOrderType = Codec<std::remove_cvref_t<decltype(v.m_byteOrderType)>>::get(r);
        v.m_ulTotalCount = Codec<std::remove_cvref_t<decltype(v.m_ulTotalCount)>>::get(r);
        return v;
    }
};

struct KENX_BT_NISMS_INVENTORY_CHECK_REQ {
    std::uint32_t  m_ulPacketNo{};  // unsigned long
    std::uint8_t   m_bytePacketType{};  // BYTE
    std::u16string m_wstrGameID{};  // std::wstring
    std::uint8_t   m_byteShowInventory{};  // BYTE
    std::uint8_t   m_byteIsPresent{};  // BYTE

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_ulPacketNo)>>::put(w, m_ulPacketNo);
        Codec<std::remove_cvref_t<decltype(m_bytePacketType)>>::put(w, m_bytePacketType);
        Codec<std::remove_cvref_t<decltype(m_wstrGameID)>>::put(w, m_wstrGameID);
        Codec<std::remove_cvref_t<decltype(m_byteShowInventory)>>::put(w, m_byteShowInventory);
        Codec<std::remove_cvref_t<decltype(m_byteIsPresent)>>::put(w, m_byteIsPresent);
    }

    static KENX_BT_NISMS_INVENTORY_CHECK_REQ read_from(Reader& r) {
        KENX_BT_NISMS_INVENTORY_CHECK_REQ v;
        v.m_ulPacketNo = Codec<std::remove_cvref_t<decltype(v.m_ulPacketNo)>>::get(r);
        v.m_bytePacketType = Codec<std::remove_cvref_t<decltype(v.m_bytePacketType)>>::get(r);
        v.m_wstrGameID = Codec<std::remove_cvref_t<decltype(v.m_wstrGameID)>>::get(r);
        v.m_byteShowInventory = Codec<std::remove_cvref_t<decltype(v.m_byteShowInventory)>>::get(r);
        v.m_byteIsPresent = Codec<std::remove_cvref_t<decltype(v.m_byteIsPresent)>>::get(r);
        return v;
    }
};

struct KENX_BT_NISMS_INVENTORY_INQUIRY_REQ {
    std::uint32_t  m_ulPacketNo{};  // unsigned long
    std::uint8_t   m_bytePacketType{};  // BYTE
    std::u16string m_wstrGameID{};  // std::wstring
    std::uint8_t   m_byteShowInventory{};  // BYTE
    std::uint32_t  m_ulPageIndex{};  // unsigned long
    std::uint32_t  m_ulRowPerPage{};  // unsigned long

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_ulPacketNo)>>::put(w, m_ulPacketNo);
        Codec<std::remove_cvref_t<decltype(m_bytePacketType)>>::put(w, m_bytePacketType);
        Codec<std::remove_cvref_t<decltype(m_wstrGameID)>>::put(w, m_wstrGameID);
        Codec<std::remove_cvref_t<decltype(m_byteShowInventory)>>::put(w, m_byteShowInventory);
        Codec<std::remove_cvref_t<decltype(m_ulPageIndex)>>::put(w, m_ulPageIndex);
        Codec<std::remove_cvref_t<decltype(m_ulRowPerPage)>>::put(w, m_ulRowPerPage);
    }

    static KENX_BT_NISMS_INVENTORY_INQUIRY_REQ read_from(Reader& r) {
        KENX_BT_NISMS_INVENTORY_INQUIRY_REQ v;
        v.m_ulPacketNo = Codec<std::remove_cvref_t<decltype(v.m_ulPacketNo)>>::get(r);
        v.m_bytePacketType = Codec<std::remove_cvref_t<decltype(v.m_bytePacketType)>>::get(r);
        v.m_wstrGameID = Codec<std::remove_cvref_t<decltype(v.m_wstrGameID)>>::get(r);
        v.m_byteShowInventory = Codec<std::remove_cvref_t<decltype(v.m_byteShowInventory)>>::get(r);
        v.m_ulPageIndex = Codec<std::remove_cvref_t<decltype(v.m_ulPageIndex)>>::get(r);
        v.m_ulRowPerPage = Codec<std::remove_cvref_t<decltype(v.m_ulRowPerPage)>>::get(r);
        return v;
    }
};

struct KENX_BT_NISMS_INVENTORY_PICK_UP_ACK {
    std::uint32_t  m_ulPacketNo{};  // unsigned long
    std::uint8_t   m_bytePacketType{};  // BYTE
    std::uint32_t  m_ulResult{};  // unsigned long
    std::uint32_t  m_ulOrderNo{};  // unsigned long
    std::uint32_t  m_ulProductNo{};  // unsigned long
    std::uint16_t  m_usOrderQuantity{};  // unsigned short
    std::u16string m_wstrExtendValue{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_ulPacketNo)>>::put(w, m_ulPacketNo);
        Codec<std::remove_cvref_t<decltype(m_bytePacketType)>>::put(w, m_bytePacketType);
        Codec<std::remove_cvref_t<decltype(m_ulResult)>>::put(w, m_ulResult);
        Codec<std::remove_cvref_t<decltype(m_ulOrderNo)>>::put(w, m_ulOrderNo);
        Codec<std::remove_cvref_t<decltype(m_ulProductNo)>>::put(w, m_ulProductNo);
        Codec<std::remove_cvref_t<decltype(m_usOrderQuantity)>>::put(w, m_usOrderQuantity);
        Codec<std::remove_cvref_t<decltype(m_wstrExtendValue)>>::put(w, m_wstrExtendValue);
    }

    static KENX_BT_NISMS_INVENTORY_PICK_UP_ACK read_from(Reader& r) {
        KENX_BT_NISMS_INVENTORY_PICK_UP_ACK v;
        v.m_ulPacketNo = Codec<std::remove_cvref_t<decltype(v.m_ulPacketNo)>>::get(r);
        v.m_bytePacketType = Codec<std::remove_cvref_t<decltype(v.m_bytePacketType)>>::get(r);
        v.m_ulResult = Codec<std::remove_cvref_t<decltype(v.m_ulResult)>>::get(r);
        v.m_ulOrderNo = Codec<std::remove_cvref_t<decltype(v.m_ulOrderNo)>>::get(r);
        v.m_ulProductNo = Codec<std::remove_cvref_t<decltype(v.m_ulProductNo)>>::get(r);
        v.m_usOrderQuantity = Codec<std::remove_cvref_t<decltype(v.m_usOrderQuantity)>>::get(r);
        v.m_wstrExtendValue = Codec<std::remove_cvref_t<decltype(v.m_wstrExtendValue)>>::get(r);
        return v;
    }
};

struct KENX_BT_NISMS_INVENTORY_PICK_UP_FOR_PACKAGE_ACK {
    std::uint32_t m_ulPacketNo{};  // unsigned long
    std::uint8_t  m_bytePacketType{};  // BYTE
    std::uint32_t m_ulResult{};  // unsigned long
    std::uint32_t m_ulOrderNo{};  // unsigned long
    std::uint32_t m_ulPackageProductNo{};  // unsigned long
    std::uint32_t m_ulProductNo{};  // unsigned long
    std::uint32_t m_ulSubProductNo{};  // unsigned long

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_ulPacketNo)>>::put(w, m_ulPacketNo);
        Codec<std::remove_cvref_t<decltype(m_bytePacketType)>>::put(w, m_bytePacketType);
        Codec<std::remove_cvref_t<decltype(m_ulResult)>>::put(w, m_ulResult);
        Codec<std::remove_cvref_t<decltype(m_ulOrderNo)>>::put(w, m_ulOrderNo);
        Codec<std::remove_cvref_t<decltype(m_ulPackageProductNo)>>::put(w, m_ulPackageProductNo);
        Codec<std::remove_cvref_t<decltype(m_ulProductNo)>>::put(w, m_ulProductNo);
        Codec<std::remove_cvref_t<decltype(m_ulSubProductNo)>>::put(w, m_ulSubProductNo);
    }

    static KENX_BT_NISMS_INVENTORY_PICK_UP_FOR_PACKAGE_ACK read_from(Reader& r) {
        KENX_BT_NISMS_INVENTORY_PICK_UP_FOR_PACKAGE_ACK v;
        v.m_ulPacketNo = Codec<std::remove_cvref_t<decltype(v.m_ulPacketNo)>>::get(r);
        v.m_bytePacketType = Codec<std::remove_cvref_t<decltype(v.m_bytePacketType)>>::get(r);
        v.m_ulResult = Codec<std::remove_cvref_t<decltype(v.m_ulResult)>>::get(r);
        v.m_ulOrderNo = Codec<std::remove_cvref_t<decltype(v.m_ulOrderNo)>>::get(r);
        v.m_ulPackageProductNo = Codec<std::remove_cvref_t<decltype(v.m_ulPackageProductNo)>>::get(r);
        v.m_ulProductNo = Codec<std::remove_cvref_t<decltype(v.m_ulProductNo)>>::get(r);
        v.m_ulSubProductNo = Codec<std::remove_cvref_t<decltype(v.m_ulSubProductNo)>>::get(r);
        return v;
    }
};

struct KENX_BT_NISMS_INVENTORY_PICK_UP_FOR_PACKAGE_REQ {
    std::uint32_t m_ulPacketNo{};  // unsigned long
    std::uint8_t  m_bytePacketType{};  // BYTE
    std::uint32_t m_ulOrderNo{};  // unsigned long
    std::uint32_t m_ulPackageProductNo{};  // unsigned long
    std::uint32_t m_ulProductNo{};  // unsigned long

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_ulPacketNo)>>::put(w, m_ulPacketNo);
        Codec<std::remove_cvref_t<decltype(m_bytePacketType)>>::put(w, m_bytePacketType);
        Codec<std::remove_cvref_t<decltype(m_ulOrderNo)>>::put(w, m_ulOrderNo);
        Codec<std::remove_cvref_t<decltype(m_ulPackageProductNo)>>::put(w, m_ulPackageProductNo);
        Codec<std::remove_cvref_t<decltype(m_ulProductNo)>>::put(w, m_ulProductNo);
    }

    static KENX_BT_NISMS_INVENTORY_PICK_UP_FOR_PACKAGE_REQ read_from(Reader& r) {
        KENX_BT_NISMS_INVENTORY_PICK_UP_FOR_PACKAGE_REQ v;
        v.m_ulPacketNo = Codec<std::remove_cvref_t<decltype(v.m_ulPacketNo)>>::get(r);
        v.m_bytePacketType = Codec<std::remove_cvref_t<decltype(v.m_bytePacketType)>>::get(r);
        v.m_ulOrderNo = Codec<std::remove_cvref_t<decltype(v.m_ulOrderNo)>>::get(r);
        v.m_ulPackageProductNo = Codec<std::remove_cvref_t<decltype(v.m_ulPackageProductNo)>>::get(r);
        v.m_ulProductNo = Codec<std::remove_cvref_t<decltype(v.m_ulProductNo)>>::get(r);
        return v;
    }
};

struct KENX_BT_NISMS_INVENTORY_PICK_UP_ONCE_REQ {
    std::uint32_t  m_ulPacketNo{};  // unsigned long
    std::uint8_t   m_bytePacketType{};  // BYTE
    std::uint32_t  m_ulOrderNo{};  // unsigned long
    std::uint32_t  m_ulProductNo{};  // unsigned long
    std::uint16_t  m_usOrderQuantity{};  // unsigned short
    std::u16string m_wstrExtendValue{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_ulPacketNo)>>::put(w, m_ulPacketNo);
        Codec<std::remove_cvref_t<decltype(m_bytePacketType)>>::put(w, m_bytePacketType);
        Codec<std::remove_cvref_t<decltype(m_ulOrderNo)>>::put(w, m_ulOrderNo);
        Codec<std::remove_cvref_t<decltype(m_ulProductNo)>>::put(w, m_ulProductNo);
        Codec<std::remove_cvref_t<decltype(m_usOrderQuantity)>>::put(w, m_usOrderQuantity);
        Codec<std::remove_cvref_t<decltype(m_wstrExtendValue)>>::put(w, m_wstrExtendValue);
    }

    static KENX_BT_NISMS_INVENTORY_PICK_UP_ONCE_REQ read_from(Reader& r) {
        KENX_BT_NISMS_INVENTORY_PICK_UP_ONCE_REQ v;
        v.m_ulPacketNo = Codec<std::remove_cvref_t<decltype(v.m_ulPacketNo)>>::get(r);
        v.m_bytePacketType = Codec<std::remove_cvref_t<decltype(v.m_bytePacketType)>>::get(r);
        v.m_ulOrderNo = Codec<std::remove_cvref_t<decltype(v.m_ulOrderNo)>>::get(r);
        v.m_ulProductNo = Codec<std::remove_cvref_t<decltype(v.m_ulProductNo)>>::get(r);
        v.m_usOrderQuantity = Codec<std::remove_cvref_t<decltype(v.m_usOrderQuantity)>>::get(r);
        v.m_wstrExtendValue = Codec<std::remove_cvref_t<decltype(v.m_wstrExtendValue)>>::get(r);
        return v;
    }
};

struct KENX_BT_NISMS_INVENTORY_PICK_UP_REQ {
    std::uint32_t  m_ulPacketNo{};  // unsigned long
    std::uint8_t   m_bytePacketType{};  // BYTE
    std::uint32_t  m_ulOrderNo{};  // unsigned long
    std::uint32_t  m_ulProductNo{};  // unsigned long
    std::uint16_t  m_usOrderQuantity{};  // unsigned short
    std::u16string m_wstrExtendValue{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_ulPacketNo)>>::put(w, m_ulPacketNo);
        Codec<std::remove_cvref_t<decltype(m_bytePacketType)>>::put(w, m_bytePacketType);
        Codec<std::remove_cvref_t<decltype(m_ulOrderNo)>>::put(w, m_ulOrderNo);
        Codec<std::remove_cvref_t<decltype(m_ulProductNo)>>::put(w, m_ulProductNo);
        Codec<std::remove_cvref_t<decltype(m_usOrderQuantity)>>::put(w, m_usOrderQuantity);
        Codec<std::remove_cvref_t<decltype(m_wstrExtendValue)>>::put(w, m_wstrExtendValue);
    }

    static KENX_BT_NISMS_INVENTORY_PICK_UP_REQ read_from(Reader& r) {
        KENX_BT_NISMS_INVENTORY_PICK_UP_REQ v;
        v.m_ulPacketNo = Codec<std::remove_cvref_t<decltype(v.m_ulPacketNo)>>::get(r);
        v.m_bytePacketType = Codec<std::remove_cvref_t<decltype(v.m_bytePacketType)>>::get(r);
        v.m_ulOrderNo = Codec<std::remove_cvref_t<decltype(v.m_ulOrderNo)>>::get(r);
        v.m_ulProductNo = Codec<std::remove_cvref_t<decltype(v.m_ulProductNo)>>::get(r);
        v.m_usOrderQuantity = Codec<std::remove_cvref_t<decltype(v.m_usOrderQuantity)>>::get(r);
        v.m_wstrExtendValue = Codec<std::remove_cvref_t<decltype(v.m_wstrExtendValue)>>::get(r);
        return v;
    }
};

struct KENX_BT_NISMS_INVENTORY_PICK_UP_ROLLBACK_ACK {
    std::uint32_t  m_ulPacketNo{};  // unsigned long
    std::uint8_t   m_bytePacketType{};  // BYTE
    std::uint32_t  m_ulResult{};  // unsigned long
    std::uint32_t  m_ulOrderNo{};  // unsigned long
    std::uint32_t  m_ulProductNo{};  // unsigned long
    std::u16string m_wstrExtendValue{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_ulPacketNo)>>::put(w, m_ulPacketNo);
        Codec<std::remove_cvref_t<decltype(m_bytePacketType)>>::put(w, m_bytePacketType);
        Codec<std::remove_cvref_t<decltype(m_ulResult)>>::put(w, m_ulResult);
        Codec<std::remove_cvref_t<decltype(m_ulOrderNo)>>::put(w, m_ulOrderNo);
        Codec<std::remove_cvref_t<decltype(m_ulProductNo)>>::put(w, m_ulProductNo);
        Codec<std::remove_cvref_t<decltype(m_wstrExtendValue)>>::put(w, m_wstrExtendValue);
    }

    static KENX_BT_NISMS_INVENTORY_PICK_UP_ROLLBACK_ACK read_from(Reader& r) {
        KENX_BT_NISMS_INVENTORY_PICK_UP_ROLLBACK_ACK v;
        v.m_ulPacketNo = Codec<std::remove_cvref_t<decltype(v.m_ulPacketNo)>>::get(r);
        v.m_bytePacketType = Codec<std::remove_cvref_t<decltype(v.m_bytePacketType)>>::get(r);
        v.m_ulResult = Codec<std::remove_cvref_t<decltype(v.m_ulResult)>>::get(r);
        v.m_ulOrderNo = Codec<std::remove_cvref_t<decltype(v.m_ulOrderNo)>>::get(r);
        v.m_ulProductNo = Codec<std::remove_cvref_t<decltype(v.m_ulProductNo)>>::get(r);
        v.m_wstrExtendValue = Codec<std::remove_cvref_t<decltype(v.m_wstrExtendValue)>>::get(r);
        return v;
    }
};

struct KENX_BT_NISMS_INVENTORY_PICK_UP_ROLLBACK_REQ {
    std::uint32_t  m_ulPacketNo{};  // unsigned long
    std::uint8_t   m_bytePacketType{};  // BYTE
    std::uint32_t  m_ulOrderNo{};  // unsigned long
    std::uint32_t  m_ulProductNo{};  // unsigned long
    std::u16string m_wstrExtendValue{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_ulPacketNo)>>::put(w, m_ulPacketNo);
        Codec<std::remove_cvref_t<decltype(m_bytePacketType)>>::put(w, m_bytePacketType);
        Codec<std::remove_cvref_t<decltype(m_ulOrderNo)>>::put(w, m_ulOrderNo);
        Codec<std::remove_cvref_t<decltype(m_ulProductNo)>>::put(w, m_ulProductNo);
        Codec<std::remove_cvref_t<decltype(m_wstrExtendValue)>>::put(w, m_wstrExtendValue);
    }

    static KENX_BT_NISMS_INVENTORY_PICK_UP_ROLLBACK_REQ read_from(Reader& r) {
        KENX_BT_NISMS_INVENTORY_PICK_UP_ROLLBACK_REQ v;
        v.m_ulPacketNo = Codec<std::remove_cvref_t<decltype(v.m_ulPacketNo)>>::get(r);
        v.m_bytePacketType = Codec<std::remove_cvref_t<decltype(v.m_bytePacketType)>>::get(r);
        v.m_ulOrderNo = Codec<std::remove_cvref_t<decltype(v.m_ulOrderNo)>>::get(r);
        v.m_ulProductNo = Codec<std::remove_cvref_t<decltype(v.m_ulProductNo)>>::get(r);
        v.m_wstrExtendValue = Codec<std::remove_cvref_t<decltype(v.m_wstrExtendValue)>>::get(r);
        return v;
    }
};

struct KENX_BT_NISMS_INVENTORY_READABLE_ACK {
    std::uint32_t m_ulPacketNo{};  // unsigned long
    std::uint8_t  m_bytePacketType{};  // BYTE
    std::uint32_t m_ulResult{};  // unsigned long
    std::uint8_t  m_byteOrderType{};  // BYTE
    std::uint32_t m_ulTotalCount{};  // unsigned long

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_ulPacketNo)>>::put(w, m_ulPacketNo);
        Codec<std::remove_cvref_t<decltype(m_bytePacketType)>>::put(w, m_bytePacketType);
        Codec<std::remove_cvref_t<decltype(m_ulResult)>>::put(w, m_ulResult);
        Codec<std::remove_cvref_t<decltype(m_byteOrderType)>>::put(w, m_byteOrderType);
        Codec<std::remove_cvref_t<decltype(m_ulTotalCount)>>::put(w, m_ulTotalCount);
    }

    static KENX_BT_NISMS_INVENTORY_READABLE_ACK read_from(Reader& r) {
        KENX_BT_NISMS_INVENTORY_READABLE_ACK v;
        v.m_ulPacketNo = Codec<std::remove_cvref_t<decltype(v.m_ulPacketNo)>>::get(r);
        v.m_bytePacketType = Codec<std::remove_cvref_t<decltype(v.m_bytePacketType)>>::get(r);
        v.m_ulResult = Codec<std::remove_cvref_t<decltype(v.m_ulResult)>>::get(r);
        v.m_byteOrderType = Codec<std::remove_cvref_t<decltype(v.m_byteOrderType)>>::get(r);
        v.m_ulTotalCount = Codec<std::remove_cvref_t<decltype(v.m_ulTotalCount)>>::get(r);
        return v;
    }
};

struct KENX_BT_NISMS_INVENTORY_READABLE_REQ {
    std::uint32_t  m_ulPacketNo{};  // unsigned long
    std::uint8_t   m_bytePacketType{};  // BYTE
    std::u16string m_wstrGameID{};  // std::wstring
    std::uint8_t   m_byteShowInventory{};  // BYTE
    std::uint8_t   m_byteIsPresent{};  // BYTE

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_ulPacketNo)>>::put(w, m_ulPacketNo);
        Codec<std::remove_cvref_t<decltype(m_bytePacketType)>>::put(w, m_bytePacketType);
        Codec<std::remove_cvref_t<decltype(m_wstrGameID)>>::put(w, m_wstrGameID);
        Codec<std::remove_cvref_t<decltype(m_byteShowInventory)>>::put(w, m_byteShowInventory);
        Codec<std::remove_cvref_t<decltype(m_byteIsPresent)>>::put(w, m_byteIsPresent);
    }

    static KENX_BT_NISMS_INVENTORY_READABLE_REQ read_from(Reader& r) {
        KENX_BT_NISMS_INVENTORY_READABLE_REQ v;
        v.m_ulPacketNo = Codec<std::remove_cvref_t<decltype(v.m_ulPacketNo)>>::get(r);
        v.m_bytePacketType = Codec<std::remove_cvref_t<decltype(v.m_bytePacketType)>>::get(r);
        v.m_wstrGameID = Codec<std::remove_cvref_t<decltype(v.m_wstrGameID)>>::get(r);
        v.m_byteShowInventory = Codec<std::remove_cvref_t<decltype(v.m_byteShowInventory)>>::get(r);
        v.m_byteIsPresent = Codec<std::remove_cvref_t<decltype(v.m_byteIsPresent)>>::get(r);
        return v;
    }
};

struct KENX_BT_NISMS_INVENTORY_READ_ACK {
    std::uint32_t m_ulPacketNo{};  // unsigned long
    std::uint8_t  m_bytePacketType{};  // BYTE
    std::uint32_t m_ulResult{};  // unsigned long

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_ulPacketNo)>>::put(w, m_ulPacketNo);
        Codec<std::remove_cvref_t<decltype(m_bytePacketType)>>::put(w, m_bytePacketType);
        Codec<std::remove_cvref_t<decltype(m_ulResult)>>::put(w, m_ulResult);
    }

    static KENX_BT_NISMS_INVENTORY_READ_ACK read_from(Reader& r) {
        KENX_BT_NISMS_INVENTORY_READ_ACK v;
        v.m_ulPacketNo = Codec<std::remove_cvref_t<decltype(v.m_ulPacketNo)>>::get(r);
        v.m_bytePacketType = Codec<std::remove_cvref_t<decltype(v.m_bytePacketType)>>::get(r);
        v.m_ulResult = Codec<std::remove_cvref_t<decltype(v.m_ulResult)>>::get(r);
        return v;
    }
};

struct KENX_BT_NISMS_INVENTORY_READ_REQ {
    std::uint32_t m_ulPacketNo{};  // unsigned long
    std::uint8_t  m_bytePacketType{};  // BYTE
    std::uint32_t m_ulOrderNo{};  // unsigned long
    std::uint32_t m_ulProductNo{};  // unsigned long

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_ulPacketNo)>>::put(w, m_ulPacketNo);
        Codec<std::remove_cvref_t<decltype(m_bytePacketType)>>::put(w, m_bytePacketType);
        Codec<std::remove_cvref_t<decltype(m_ulOrderNo)>>::put(w, m_ulOrderNo);
        Codec<std::remove_cvref_t<decltype(m_ulProductNo)>>::put(w, m_ulProductNo);
    }

    static KENX_BT_NISMS_INVENTORY_READ_REQ read_from(Reader& r) {
        KENX_BT_NISMS_INVENTORY_READ_REQ v;
        v.m_ulPacketNo = Codec<std::remove_cvref_t<decltype(v.m_ulPacketNo)>>::get(r);
        v.m_bytePacketType = Codec<std::remove_cvref_t<decltype(v.m_bytePacketType)>>::get(r);
        v.m_ulOrderNo = Codec<std::remove_cvref_t<decltype(v.m_ulOrderNo)>>::get(r);
        v.m_ulProductNo = Codec<std::remove_cvref_t<decltype(v.m_ulProductNo)>>::get(r);
        return v;
    }
};

struct KENX_BT_PRODUCT_INQUIRY_REQ {
    std::uint32_t m_ulPacketNo{};  // unsigned long
    std::uint8_t  m_bytePacketType{};  // BYTE
    std::uint32_t m_ulPageIndex{};  // unsigned long
    std::uint32_t m_ulRowPerPage{};  // unsigned long

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_ulPacketNo)>>::put(w, m_ulPacketNo);
        Codec<std::remove_cvref_t<decltype(m_bytePacketType)>>::put(w, m_bytePacketType);
        Codec<std::remove_cvref_t<decltype(m_ulPageIndex)>>::put(w, m_ulPageIndex);
        Codec<std::remove_cvref_t<decltype(m_ulRowPerPage)>>::put(w, m_ulRowPerPage);
    }

    static KENX_BT_PRODUCT_INQUIRY_REQ read_from(Reader& r) {
        KENX_BT_PRODUCT_INQUIRY_REQ v;
        v.m_ulPacketNo = Codec<std::remove_cvref_t<decltype(v.m_ulPacketNo)>>::get(r);
        v.m_bytePacketType = Codec<std::remove_cvref_t<decltype(v.m_bytePacketType)>>::get(r);
        v.m_ulPageIndex = Codec<std::remove_cvref_t<decltype(v.m_ulPageIndex)>>::get(r);
        v.m_ulRowPerPage = Codec<std::remove_cvref_t<decltype(v.m_ulRowPerPage)>>::get(r);
        return v;
    }
};

struct KENX_BT_PRODUCT_INQUIRY_XML_ACK {
    std::uint32_t  m_ulPacketNo{};  // unsigned long
    std::uint8_t   m_bytePacketType{};  // BYTE
    std::uint32_t  m_ulResult{};  // unsigned long
    std::int64_t   m_iReleaseTick{};  // __int64
    std::uint32_t  m_ulTotalProductCount{};  // unsigned long
    std::u16string m_wstrResultXML{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_ulPacketNo)>>::put(w, m_ulPacketNo);
        Codec<std::remove_cvref_t<decltype(m_bytePacketType)>>::put(w, m_bytePacketType);
        Codec<std::remove_cvref_t<decltype(m_ulResult)>>::put(w, m_ulResult);
        Codec<std::remove_cvref_t<decltype(m_iReleaseTick)>>::put(w, m_iReleaseTick);
        Codec<std::remove_cvref_t<decltype(m_ulTotalProductCount)>>::put(w, m_ulTotalProductCount);
        Codec<std::remove_cvref_t<decltype(m_wstrResultXML)>>::put(w, m_wstrResultXML);
    }

    static KENX_BT_PRODUCT_INQUIRY_XML_ACK read_from(Reader& r) {
        KENX_BT_PRODUCT_INQUIRY_XML_ACK v;
        v.m_ulPacketNo = Codec<std::remove_cvref_t<decltype(v.m_ulPacketNo)>>::get(r);
        v.m_bytePacketType = Codec<std::remove_cvref_t<decltype(v.m_bytePacketType)>>::get(r);
        v.m_ulResult = Codec<std::remove_cvref_t<decltype(v.m_ulResult)>>::get(r);
        v.m_iReleaseTick = Codec<std::remove_cvref_t<decltype(v.m_iReleaseTick)>>::get(r);
        v.m_ulTotalProductCount = Codec<std::remove_cvref_t<decltype(v.m_ulTotalProductCount)>>::get(r);
        v.m_wstrResultXML = Codec<std::remove_cvref_t<decltype(v.m_wstrResultXML)>>::get(r);
        return v;
    }
};

struct KENX_BT_PRODUCT_INQUIRY_XML_REQ {
    std::uint32_t m_ulPacketNo{};  // unsigned long
    std::uint8_t  m_bytePacketType{};  // BYTE
    std::uint32_t m_ulPageIndex{};  // unsigned long
    std::uint32_t m_ulRowPerPage{};  // unsigned long

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_ulPacketNo)>>::put(w, m_ulPacketNo);
        Codec<std::remove_cvref_t<decltype(m_bytePacketType)>>::put(w, m_bytePacketType);
        Codec<std::remove_cvref_t<decltype(m_ulPageIndex)>>::put(w, m_ulPageIndex);
        Codec<std::remove_cvref_t<decltype(m_ulRowPerPage)>>::put(w, m_ulRowPerPage);
    }

    static KENX_BT_PRODUCT_INQUIRY_XML_REQ read_from(Reader& r) {
        KENX_BT_PRODUCT_INQUIRY_XML_REQ v;
        v.m_ulPacketNo = Codec<std::remove_cvref_t<decltype(v.m_ulPacketNo)>>::get(r);
        v.m_bytePacketType = Codec<std::remove_cvref_t<decltype(v.m_bytePacketType)>>::get(r);
        v.m_ulPageIndex = Codec<std::remove_cvref_t<decltype(v.m_ulPageIndex)>>::get(r);
        v.m_ulRowPerPage = Codec<std::remove_cvref_t<decltype(v.m_ulRowPerPage)>>::get(r);
        return v;
    }
};

struct KENX_BT_REFUND_ACK {
    std::uint32_t m_ulPacketNo{};  // unsigned long
    std::uint8_t  m_bytePacketType{};  // BYTE
    std::uint32_t m_ulResult{};  // unsigned long

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_ulPacketNo)>>::put(w, m_ulPacketNo);
        Codec<std::remove_cvref_t<decltype(m_bytePacketType)>>::put(w, m_bytePacketType);
        Codec<std::remove_cvref_t<decltype(m_ulResult)>>::put(w, m_ulResult);
    }

    static KENX_BT_REFUND_ACK read_from(Reader& r) {
        KENX_BT_REFUND_ACK v;
        v.m_ulPacketNo = Codec<std::remove_cvref_t<decltype(v.m_ulPacketNo)>>::get(r);
        v.m_bytePacketType = Codec<std::remove_cvref_t<decltype(v.m_bytePacketType)>>::get(r);
        v.m_ulResult = Codec<std::remove_cvref_t<decltype(v.m_ulResult)>>::get(r);
        return v;
    }
};

struct KENX_BT_REFUND_REQ {
    std::uint32_t  m_ulPacketNo{};  // unsigned long
    std::uint8_t   m_bytePacketType{};  // BYTE
    std::uint32_t  m_ulRemoteIP{};  // unsigned long
    std::u16string m_wstrRequestID{};  // std::wstring
    std::u16string m_wstrGameID{};  // std::wstring
    std::uint32_t  m_ulOrderNo{};  // unsigned long
    std::uint32_t  m_ulProductNo{};  // unsigned long
    std::uint16_t  m_usQuantity{};  // unsigned short

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_ulPacketNo)>>::put(w, m_ulPacketNo);
        Codec<std::remove_cvref_t<decltype(m_bytePacketType)>>::put(w, m_bytePacketType);
        Codec<std::remove_cvref_t<decltype(m_ulRemoteIP)>>::put(w, m_ulRemoteIP);
        Codec<std::remove_cvref_t<decltype(m_wstrRequestID)>>::put(w, m_wstrRequestID);
        Codec<std::remove_cvref_t<decltype(m_wstrGameID)>>::put(w, m_wstrGameID);
        Codec<std::remove_cvref_t<decltype(m_ulOrderNo)>>::put(w, m_ulOrderNo);
        Codec<std::remove_cvref_t<decltype(m_ulProductNo)>>::put(w, m_ulProductNo);
        Codec<std::remove_cvref_t<decltype(m_usQuantity)>>::put(w, m_usQuantity);
    }

    static KENX_BT_REFUND_REQ read_from(Reader& r) {
        KENX_BT_REFUND_REQ v;
        v.m_ulPacketNo = Codec<std::remove_cvref_t<decltype(v.m_ulPacketNo)>>::get(r);
        v.m_bytePacketType = Codec<std::remove_cvref_t<decltype(v.m_bytePacketType)>>::get(r);
        v.m_ulRemoteIP = Codec<std::remove_cvref_t<decltype(v.m_ulRemoteIP)>>::get(r);
        v.m_wstrRequestID = Codec<std::remove_cvref_t<decltype(v.m_wstrRequestID)>>::get(r);
        v.m_wstrGameID = Codec<std::remove_cvref_t<decltype(v.m_wstrGameID)>>::get(r);
        v.m_ulOrderNo = Codec<std::remove_cvref_t<decltype(v.m_ulOrderNo)>>::get(r);
        v.m_ulProductNo = Codec<std::remove_cvref_t<decltype(v.m_ulProductNo)>>::get(r);
        v.m_usQuantity = Codec<std::remove_cvref_t<decltype(v.m_usQuantity)>>::get(r);
        return v;
    }
};

struct KENX_CHAR_SEARCH_ACK {
    std::uint32_t m_dwTransactionID{};  // DWORD
    std::uint8_t  m_byteResult{};  // BYTE

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_dwTransactionID)>>::put(w, m_dwTransactionID);
        Codec<std::remove_cvref_t<decltype(m_byteResult)>>::put(w, m_byteResult);
    }

    static KENX_CHAR_SEARCH_ACK read_from(Reader& r) {
        KENX_CHAR_SEARCH_ACK v;
        v.m_dwTransactionID = Codec<std::remove_cvref_t<decltype(v.m_dwTransactionID)>>::get(r);
        v.m_byteResult = Codec<std::remove_cvref_t<decltype(v.m_byteResult)>>::get(r);
        return v;
    }
};

struct KENX_CHAR_SEARCH_REQ {
    std::uint32_t  m_dwTransactionID{};  // DWORD
    std::u16string m_wstrUnitNickName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_dwTransactionID)>>::put(w, m_dwTransactionID);
        Codec<std::remove_cvref_t<decltype(m_wstrUnitNickName)>>::put(w, m_wstrUnitNickName);
    }

    static KENX_CHAR_SEARCH_REQ read_from(Reader& r) {
        KENX_CHAR_SEARCH_REQ v;
        v.m_dwTransactionID = Codec<std::remove_cvref_t<decltype(v.m_dwTransactionID)>>::get(r);
        v.m_wstrUnitNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrUnitNickName)>>::get(r);
        return v;
    }
};

struct KENX_COMMON_ACK_NOT {
    std::uint8_t   m_bytePacketType{};  // BYTE
    std::u16string m_wstrUserID{};  // std::wstring
    std::u16string m_wstrUnitNickName{};  // std::wstring
    std::uint8_t   m_byteResult{};  // BYTE
    std::uint8_t   m_byteAddressDesc{};  // BYTE
    std::uint8_t   m_byteAccountDesc{};  // BYTE
    std::uint8_t   m_byteOption{};  // BYTE
    std::uint32_t  m_uiArgument{};  // unsigned int
    std::uint8_t   m_byteIsGameBangBillType{};  // BYTE

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bytePacketType)>>::put(w, m_bytePacketType);
        Codec<std::remove_cvref_t<decltype(m_wstrUserID)>>::put(w, m_wstrUserID);
        Codec<std::remove_cvref_t<decltype(m_wstrUnitNickName)>>::put(w, m_wstrUnitNickName);
        Codec<std::remove_cvref_t<decltype(m_byteResult)>>::put(w, m_byteResult);
        Codec<std::remove_cvref_t<decltype(m_byteAddressDesc)>>::put(w, m_byteAddressDesc);
        Codec<std::remove_cvref_t<decltype(m_byteAccountDesc)>>::put(w, m_byteAccountDesc);
        Codec<std::remove_cvref_t<decltype(m_byteOption)>>::put(w, m_byteOption);
        Codec<std::remove_cvref_t<decltype(m_uiArgument)>>::put(w, m_uiArgument);
        Codec<std::remove_cvref_t<decltype(m_byteIsGameBangBillType)>>::put(w, m_byteIsGameBangBillType);
    }

    static KENX_COMMON_ACK_NOT read_from(Reader& r) {
        KENX_COMMON_ACK_NOT v;
        v.m_bytePacketType = Codec<std::remove_cvref_t<decltype(v.m_bytePacketType)>>::get(r);
        v.m_wstrUserID = Codec<std::remove_cvref_t<decltype(v.m_wstrUserID)>>::get(r);
        v.m_wstrUnitNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrUnitNickName)>>::get(r);
        v.m_byteResult = Codec<std::remove_cvref_t<decltype(v.m_byteResult)>>::get(r);
        v.m_byteAddressDesc = Codec<std::remove_cvref_t<decltype(v.m_byteAddressDesc)>>::get(r);
        v.m_byteAccountDesc = Codec<std::remove_cvref_t<decltype(v.m_byteAccountDesc)>>::get(r);
        v.m_byteOption = Codec<std::remove_cvref_t<decltype(v.m_byteOption)>>::get(r);
        v.m_uiArgument = Codec<std::remove_cvref_t<decltype(v.m_uiArgument)>>::get(r);
        v.m_byteIsGameBangBillType = Codec<std::remove_cvref_t<decltype(v.m_byteIsGameBangBillType)>>::get(r);
        return v;
    }
};

struct KENX_COMMON_REQ {
    std::uint8_t   m_bytePacketType{};  // BYTE
    std::uint8_t   m_byteLogin{};  // BYTE
    std::u16string m_wstrUserID{};  // std::wstring
    std::u16string m_wstrUnitNickName{};  // std::wstring
    std::uint32_t  m_uiIP{};  // unsigned int
    std::uint8_t   m_byteCanTry{};  // BYTE
    std::uint8_t   m_byteGuestUser{};  // BYTE

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bytePacketType)>>::put(w, m_bytePacketType);
        Codec<std::remove_cvref_t<decltype(m_byteLogin)>>::put(w, m_byteLogin);
        Codec<std::remove_cvref_t<decltype(m_wstrUserID)>>::put(w, m_wstrUserID);
        Codec<std::remove_cvref_t<decltype(m_wstrUnitNickName)>>::put(w, m_wstrUnitNickName);
        Codec<std::remove_cvref_t<decltype(m_uiIP)>>::put(w, m_uiIP);
        Codec<std::remove_cvref_t<decltype(m_byteCanTry)>>::put(w, m_byteCanTry);
        Codec<std::remove_cvref_t<decltype(m_byteGuestUser)>>::put(w, m_byteGuestUser);
    }

    static KENX_COMMON_REQ read_from(Reader& r) {
        KENX_COMMON_REQ v;
        v.m_bytePacketType = Codec<std::remove_cvref_t<decltype(v.m_bytePacketType)>>::get(r);
        v.m_byteLogin = Codec<std::remove_cvref_t<decltype(v.m_byteLogin)>>::get(r);
        v.m_wstrUserID = Codec<std::remove_cvref_t<decltype(v.m_wstrUserID)>>::get(r);
        v.m_wstrUnitNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrUnitNickName)>>::get(r);
        v.m_uiIP = Codec<std::remove_cvref_t<decltype(v.m_uiIP)>>::get(r);
        v.m_byteCanTry = Codec<std::remove_cvref_t<decltype(v.m_byteCanTry)>>::get(r);
        v.m_byteGuestUser = Codec<std::remove_cvref_t<decltype(v.m_byteGuestUser)>>::get(r);
        return v;
    }
};

struct KENX_INITIALIZE_BILLING_AUTH_NOT {
    std::uint8_t   m_bytePacketType{};  // BYTE
    std::uint8_t   m_byteGameSN{};  // BYTE
    std::uint8_t   m_byteDomainSN{};  // BYTE
    std::u16string m_wstrDomainName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bytePacketType)>>::put(w, m_bytePacketType);
        Codec<std::remove_cvref_t<decltype(m_byteGameSN)>>::put(w, m_byteGameSN);
        Codec<std::remove_cvref_t<decltype(m_byteDomainSN)>>::put(w, m_byteDomainSN);
        Codec<std::remove_cvref_t<decltype(m_wstrDomainName)>>::put(w, m_wstrDomainName);
    }

    static KENX_INITIALIZE_BILLING_AUTH_NOT read_from(Reader& r) {
        KENX_INITIALIZE_BILLING_AUTH_NOT v;
        v.m_bytePacketType = Codec<std::remove_cvref_t<decltype(v.m_bytePacketType)>>::get(r);
        v.m_byteGameSN = Codec<std::remove_cvref_t<decltype(v.m_byteGameSN)>>::get(r);
        v.m_byteDomainSN = Codec<std::remove_cvref_t<decltype(v.m_byteDomainSN)>>::get(r);
        v.m_wstrDomainName = Codec<std::remove_cvref_t<decltype(v.m_wstrDomainName)>>::get(r);
        return v;
    }
};

struct KENX_NEW_SHOP_GIFT_NOT {
    std::uint32_t  m_dwTransactionID{};  // DWORD
    std::u16string m_wstrUnitNickName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_dwTransactionID)>>::put(w, m_dwTransactionID);
        Codec<std::remove_cvref_t<decltype(m_wstrUnitNickName)>>::put(w, m_wstrUnitNickName);
    }

    static KENX_NEW_SHOP_GIFT_NOT read_from(Reader& r) {
        KENX_NEW_SHOP_GIFT_NOT v;
        v.m_dwTransactionID = Codec<std::remove_cvref_t<decltype(v.m_dwTransactionID)>>::get(r);
        v.m_wstrUnitNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrUnitNickName)>>::get(r);
        return v;
    }
};

struct KENX_NEW_SHOP_ITEM_NOT {
    std::uint32_t  m_dwTransactionID{};  // DWORD
    std::u16string m_wstrUnitNickName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_dwTransactionID)>>::put(w, m_dwTransactionID);
        Codec<std::remove_cvref_t<decltype(m_wstrUnitNickName)>>::put(w, m_wstrUnitNickName);
    }

    static KENX_NEW_SHOP_ITEM_NOT read_from(Reader& r) {
        KENX_NEW_SHOP_ITEM_NOT v;
        v.m_dwTransactionID = Codec<std::remove_cvref_t<decltype(v.m_dwTransactionID)>>::get(r);
        v.m_wstrUnitNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrUnitNickName)>>::get(r);
        return v;
    }
};

struct KENX_UPDATE_PRODUCT_LIST_NOT {
    std::vector<std::int32_t>         m_vecProductItemID{};  // std::vector< int >
    std::vector<KCashItemResolveInfo> m_vecCashItemRessolveInfo{};  // std::vector< KCashItemResolveInfo >
    std::vector<KCashPruductInfo>     m_vecCashProductList{};  // std::vector< KCashPruductInfo >
    std::uint32_t                     m_ulProductNoResStone{};  // unsigned long

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecProductItemID)>>::put(w, m_vecProductItemID);
        Codec<std::remove_cvref_t<decltype(m_vecCashItemRessolveInfo)>>::put(w, m_vecCashItemRessolveInfo);
        Codec<std::remove_cvref_t<decltype(m_vecCashProductList)>>::put(w, m_vecCashProductList);
        Codec<std::remove_cvref_t<decltype(m_ulProductNoResStone)>>::put(w, m_ulProductNoResStone);
    }

    static KENX_UPDATE_PRODUCT_LIST_NOT read_from(Reader& r) {
        KENX_UPDATE_PRODUCT_LIST_NOT v;
        v.m_vecProductItemID = Codec<std::remove_cvref_t<decltype(v.m_vecProductItemID)>>::get(r);
        v.m_vecCashItemRessolveInfo = Codec<std::remove_cvref_t<decltype(v.m_vecCashItemRessolveInfo)>>::get(r);
        v.m_vecCashProductList = Codec<std::remove_cvref_t<decltype(v.m_vecCashProductList)>>::get(r);
        v.m_ulProductNoResStone = Codec<std::remove_cvref_t<decltype(v.m_ulProductNoResStone)>>::get(r);
        return v;
    }
};

struct KEPM_CHANGE_COME_BACK_BUFF_NOT {
    bool           m_bIsBuffFinish{};
    std::u16string m_wstrComeBackBuffEnd{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bIsBuffFinish)>>::put(w, m_bIsBuffFinish);
        Codec<std::remove_cvref_t<decltype(m_wstrComeBackBuffEnd)>>::put(w, m_wstrComeBackBuffEnd);
    }

    static KEPM_CHANGE_COME_BACK_BUFF_NOT read_from(Reader& r) {
        KEPM_CHANGE_COME_BACK_BUFF_NOT v;
        v.m_bIsBuffFinish = Codec<std::remove_cvref_t<decltype(v.m_bIsBuffFinish)>>::get(r);
        v.m_wstrComeBackBuffEnd = Codec<std::remove_cvref_t<decltype(v.m_wstrComeBackBuffEnd)>>::get(r);
        return v;
    }
};

struct KEPM_CHAT_REQ {
    std::int64_t   m_iPartyUID{};  // UidType
    char           m_cChatPacketType{};
    char           m_cRoomChatType{};
    std::int64_t   m_ToUnitUID{};  // UidType
    std::u16string m_wstrRecieverUnitNickName{};  // std::wstring
    std::u16string m_wstrMsg{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iPartyUID)>>::put(w, m_iPartyUID);
        Codec<std::remove_cvref_t<decltype(m_cChatPacketType)>>::put(w, m_cChatPacketType);
        Codec<std::remove_cvref_t<decltype(m_cRoomChatType)>>::put(w, m_cRoomChatType);
        Codec<std::remove_cvref_t<decltype(m_ToUnitUID)>>::put(w, m_ToUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrRecieverUnitNickName)>>::put(w, m_wstrRecieverUnitNickName);
        Codec<std::remove_cvref_t<decltype(m_wstrMsg)>>::put(w, m_wstrMsg);
    }

    static KEPM_CHAT_REQ read_from(Reader& r) {
        KEPM_CHAT_REQ v;
        v.m_iPartyUID = Codec<std::remove_cvref_t<decltype(v.m_iPartyUID)>>::get(r);
        v.m_cChatPacketType = Codec<std::remove_cvref_t<decltype(v.m_cChatPacketType)>>::get(r);
        v.m_cRoomChatType = Codec<std::remove_cvref_t<decltype(v.m_cRoomChatType)>>::get(r);
        v.m_ToUnitUID = Codec<std::remove_cvref_t<decltype(v.m_ToUnitUID)>>::get(r);
        v.m_wstrRecieverUnitNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrRecieverUnitNickName)>>::get(r);
        v.m_wstrMsg = Codec<std::remove_cvref_t<decltype(v.m_wstrMsg)>>::get(r);
        return v;
    }
};

struct KEPM_CHECK_DUNGEON_GAME_PLAY_CONDITION_ACK {
    std::int32_t   m_iOK{};  // int
    std::u16string m_wstrFailUserNickName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_wstrFailUserNickName)>>::put(w, m_wstrFailUserNickName);
    }

    static KEPM_CHECK_DUNGEON_GAME_PLAY_CONDITION_ACK read_from(Reader& r) {
        KEPM_CHECK_DUNGEON_GAME_PLAY_CONDITION_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_wstrFailUserNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrFailUserNickName)>>::get(r);
        return v;
    }
};

struct KEPM_CHECK_DUNGEON_GAME_PLAY_CONDITION_REQ {
    KDungeonGameInfo m_kDungeonGameInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kDungeonGameInfo)>>::put(w, m_kDungeonGameInfo);
    }

    static KEPM_CHECK_DUNGEON_GAME_PLAY_CONDITION_REQ read_from(Reader& r) {
        KEPM_CHECK_DUNGEON_GAME_PLAY_CONDITION_REQ v;
        v.m_kDungeonGameInfo = Codec<std::remove_cvref_t<decltype(v.m_kDungeonGameInfo)>>::get(r);
        return v;
    }
};

struct KEPM_CHECK_FOR_AUTO_PARTY_MAKING_ACK {
    std::int32_t   m_iOK{};  // int
    std::u16string m_wstrFailUserNickName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_wstrFailUserNickName)>>::put(w, m_wstrFailUserNickName);
    }

    static KEPM_CHECK_FOR_AUTO_PARTY_MAKING_ACK read_from(Reader& r) {
        KEPM_CHECK_FOR_AUTO_PARTY_MAKING_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_wstrFailUserNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrFailUserNickName)>>::get(r);
        return v;
    }
};

struct KEPM_CHECK_FOR_PARTY_GAME_START_REQ {
    std::int64_t     m_iPartyUID{};  // UidType
    std::uint16_t    m_usEventID{};  // unsigned short
    std::int16_t     m_sWorldID{};  // short
    KDungeonGameInfo m_kDungeonGameInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iPartyUID)>>::put(w, m_iPartyUID);
        Codec<std::remove_cvref_t<decltype(m_usEventID)>>::put(w, m_usEventID);
        Codec<std::remove_cvref_t<decltype(m_sWorldID)>>::put(w, m_sWorldID);
        Codec<std::remove_cvref_t<decltype(m_kDungeonGameInfo)>>::put(w, m_kDungeonGameInfo);
    }

    static KEPM_CHECK_FOR_PARTY_GAME_START_REQ read_from(Reader& r) {
        KEPM_CHECK_FOR_PARTY_GAME_START_REQ v;
        v.m_iPartyUID = Codec<std::remove_cvref_t<decltype(v.m_iPartyUID)>>::get(r);
        v.m_usEventID = Codec<std::remove_cvref_t<decltype(v.m_usEventID)>>::get(r);
        v.m_sWorldID = Codec<std::remove_cvref_t<decltype(v.m_sWorldID)>>::get(r);
        v.m_kDungeonGameInfo = Codec<std::remove_cvref_t<decltype(v.m_kDungeonGameInfo)>>::get(r);
        return v;
    }
};

struct KEPM_CHECK_PARTY_CHANGE_HOST_ACK {
    std::int32_t m_iOK{};  // int
    std::int64_t m_iPartyUID{};  // UidType
    std::int64_t m_iOldHostUnitUID{};  // UidType
    std::int64_t m_iNewHostUnitUID{};  // UidType
    bool         m_bForRegroupParty{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iPartyUID)>>::put(w, m_iPartyUID);
        Codec<std::remove_cvref_t<decltype(m_iOldHostUnitUID)>>::put(w, m_iOldHostUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iNewHostUnitUID)>>::put(w, m_iNewHostUnitUID);
        Codec<std::remove_cvref_t<decltype(m_bForRegroupParty)>>::put(w, m_bForRegroupParty);
    }

    static KEPM_CHECK_PARTY_CHANGE_HOST_ACK read_from(Reader& r) {
        KEPM_CHECK_PARTY_CHANGE_HOST_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iPartyUID = Codec<std::remove_cvref_t<decltype(v.m_iPartyUID)>>::get(r);
        v.m_iOldHostUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iOldHostUnitUID)>>::get(r);
        v.m_iNewHostUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iNewHostUnitUID)>>::get(r);
        v.m_bForRegroupParty = Codec<std::remove_cvref_t<decltype(v.m_bForRegroupParty)>>::get(r);
        return v;
    }
};

struct KEPM_CHECK_PARTY_CHANGE_HOST_REQ {
    std::int64_t m_iPartyUID{};  // UidType
    char         m_cPartyType{};
    std::int64_t m_iOldHostUnitUID{};  // UidType
    std::int64_t m_iNewHostUnitUID{};  // UidType
    std::int32_t m_iDungeonID{};  // int
    char         m_cDifficulty{};
    char         m_cDungeonMode{};
    bool         m_bForRegroupParty{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iPartyUID)>>::put(w, m_iPartyUID);
        Codec<std::remove_cvref_t<decltype(m_cPartyType)>>::put(w, m_cPartyType);
        Codec<std::remove_cvref_t<decltype(m_iOldHostUnitUID)>>::put(w, m_iOldHostUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iNewHostUnitUID)>>::put(w, m_iNewHostUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iDungeonID)>>::put(w, m_iDungeonID);
        Codec<std::remove_cvref_t<decltype(m_cDifficulty)>>::put(w, m_cDifficulty);
        Codec<std::remove_cvref_t<decltype(m_cDungeonMode)>>::put(w, m_cDungeonMode);
        Codec<std::remove_cvref_t<decltype(m_bForRegroupParty)>>::put(w, m_bForRegroupParty);
    }

    static KEPM_CHECK_PARTY_CHANGE_HOST_REQ read_from(Reader& r) {
        KEPM_CHECK_PARTY_CHANGE_HOST_REQ v;
        v.m_iPartyUID = Codec<std::remove_cvref_t<decltype(v.m_iPartyUID)>>::get(r);
        v.m_cPartyType = Codec<std::remove_cvref_t<decltype(v.m_cPartyType)>>::get(r);
        v.m_iOldHostUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iOldHostUnitUID)>>::get(r);
        v.m_iNewHostUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iNewHostUnitUID)>>::get(r);
        v.m_iDungeonID = Codec<std::remove_cvref_t<decltype(v.m_iDungeonID)>>::get(r);
        v.m_cDifficulty = Codec<std::remove_cvref_t<decltype(v.m_cDifficulty)>>::get(r);
        v.m_cDungeonMode = Codec<std::remove_cvref_t<decltype(v.m_cDungeonMode)>>::get(r);
        v.m_bForRegroupParty = Codec<std::remove_cvref_t<decltype(v.m_bForRegroupParty)>>::get(r);
        return v;
    }
};

struct KEPM_CLOSE_PARTY_LIST_NOT {
    std::int64_t m_iPartyUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iPartyUID)>>::put(w, m_iPartyUID);
    }

    static KEPM_CLOSE_PARTY_LIST_NOT read_from(Reader& r) {
        KEPM_CLOSE_PARTY_LIST_NOT v;
        v.m_iPartyUID = Codec<std::remove_cvref_t<decltype(v.m_iPartyUID)>>::get(r);
        return v;
    }
};

struct KEPM_DEL_PARTY_MEMBER_IN_PARTY_LIST_NOT {
    std::int64_t m_iPartyUID{};  // UidType
    std::int64_t m_iUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iPartyUID)>>::put(w, m_iPartyUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
    }

    static KEPM_DEL_PARTY_MEMBER_IN_PARTY_LIST_NOT read_from(Reader& r) {
        KEPM_DEL_PARTY_MEMBER_IN_PARTY_LIST_NOT v;
        v.m_iPartyUID = Codec<std::remove_cvref_t<decltype(v.m_iPartyUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        return v;
    }
};

struct KEPM_DUNGEON_SECRET_STAGE_ENTER_CHECK_REQ {
    std::int64_t m_iPartyUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iPartyUID)>>::put(w, m_iPartyUID);
    }

    static KEPM_DUNGEON_SECRET_STAGE_ENTER_CHECK_REQ read_from(Reader& r) {
        KEPM_DUNGEON_SECRET_STAGE_ENTER_CHECK_REQ v;
        v.m_iPartyUID = Codec<std::remove_cvref_t<decltype(v.m_iPartyUID)>>::get(r);
        return v;
    }
};

struct KEPM_END_GAME_NOT {
    std::int64_t m_iPartyUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iPartyUID)>>::put(w, m_iPartyUID);
    }

    static KEPM_END_GAME_NOT read_from(Reader& r) {
        KEPM_END_GAME_NOT v;
        v.m_iPartyUID = Codec<std::remove_cvref_t<decltype(v.m_iPartyUID)>>::get(r);
        return v;
    }
};

struct KEPM_GET_PARTY_INFO_FOR_AUTO_PARTY_MAKING_REQ {
    bool         m_bRemakingAutoParty{};
    std::int64_t m_iPartyUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bRemakingAutoParty)>>::put(w, m_bRemakingAutoParty);
        Codec<std::remove_cvref_t<decltype(m_iPartyUID)>>::put(w, m_iPartyUID);
    }

    static KEPM_GET_PARTY_INFO_FOR_AUTO_PARTY_MAKING_REQ read_from(Reader& r) {
        KEPM_GET_PARTY_INFO_FOR_AUTO_PARTY_MAKING_REQ v;
        v.m_bRemakingAutoParty = Codec<std::remove_cvref_t<decltype(v.m_bRemakingAutoParty)>>::get(r);
        v.m_iPartyUID = Codec<std::remove_cvref_t<decltype(v.m_iPartyUID)>>::get(r);
        return v;
    }
};

struct KEPM_GET_PARTY_INFO_FOR_JOIN_BATTLE_FIELD_ACK {
    std::int32_t         m_iOK{};  // int
    KBattleFieldJoinInfo m_kBattleFieldJoinInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_kBattleFieldJoinInfo)>>::put(w, m_kBattleFieldJoinInfo);
    }

    static KEPM_GET_PARTY_INFO_FOR_JOIN_BATTLE_FIELD_ACK read_from(Reader& r) {
        KEPM_GET_PARTY_INFO_FOR_JOIN_BATTLE_FIELD_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_kBattleFieldJoinInfo = Codec<std::remove_cvref_t<decltype(v.m_kBattleFieldJoinInfo)>>::get(r);
        return v;
    }
};

struct KEPM_GET_PARTY_INFO_FOR_JOIN_BATTLE_FIELD_REQ {
    KBattleFieldJoinInfo m_kBattleFieldJoinInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kBattleFieldJoinInfo)>>::put(w, m_kBattleFieldJoinInfo);
    }

    static KEPM_GET_PARTY_INFO_FOR_JOIN_BATTLE_FIELD_REQ read_from(Reader& r) {
        KEPM_GET_PARTY_INFO_FOR_JOIN_BATTLE_FIELD_REQ v;
        v.m_kBattleFieldJoinInfo = Codec<std::remove_cvref_t<decltype(v.m_kBattleFieldJoinInfo)>>::get(r);
        return v;
    }
};

struct KEPM_GET_PARTY_INFO_FOR_MATCH_MAKING_REQ {
    bool m_bRegFrontOfList{};
    char m_cPvpPlayerCount{};
    char m_cPvpGameType{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bRegFrontOfList)>>::put(w, m_bRegFrontOfList);
        Codec<std::remove_cvref_t<decltype(m_cPvpPlayerCount)>>::put(w, m_cPvpPlayerCount);
        Codec<std::remove_cvref_t<decltype(m_cPvpGameType)>>::put(w, m_cPvpGameType);
    }

    static KEPM_GET_PARTY_INFO_FOR_MATCH_MAKING_REQ read_from(Reader& r) {
        KEPM_GET_PARTY_INFO_FOR_MATCH_MAKING_REQ v;
        v.m_bRegFrontOfList = Codec<std::remove_cvref_t<decltype(v.m_bRegFrontOfList)>>::get(r);
        v.m_cPvpPlayerCount = Codec<std::remove_cvref_t<decltype(v.m_cPvpPlayerCount)>>::get(r);
        v.m_cPvpGameType = Codec<std::remove_cvref_t<decltype(v.m_cPvpGameType)>>::get(r);
        return v;
    }
};

struct KEPM_LEAVE_PARTY_ACK {
    std::int32_t m_iOK{};  // int
    std::int32_t m_iReason{};  // int
    std::int64_t m_iPartyUID{};  // UidType
    std::int64_t m_iNewJoinPartyUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iReason)>>::put(w, m_iReason);
        Codec<std::remove_cvref_t<decltype(m_iPartyUID)>>::put(w, m_iPartyUID);
        Codec<std::remove_cvref_t<decltype(m_iNewJoinPartyUID)>>::put(w, m_iNewJoinPartyUID);
    }

    static KEPM_LEAVE_PARTY_ACK read_from(Reader& r) {
        KEPM_LEAVE_PARTY_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iReason = Codec<std::remove_cvref_t<decltype(v.m_iReason)>>::get(r);
        v.m_iPartyUID = Codec<std::remove_cvref_t<decltype(v.m_iPartyUID)>>::get(r);
        v.m_iNewJoinPartyUID = Codec<std::remove_cvref_t<decltype(v.m_iNewJoinPartyUID)>>::get(r);
        return v;
    }
};

struct KEPM_LEAVE_PARTY_BY_AUTO_PARTY_NOT {
    std::int64_t m_iPartyUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iPartyUID)>>::put(w, m_iPartyUID);
    }

    static KEPM_LEAVE_PARTY_BY_AUTO_PARTY_NOT read_from(Reader& r) {
        KEPM_LEAVE_PARTY_BY_AUTO_PARTY_NOT v;
        v.m_iPartyUID = Codec<std::remove_cvref_t<decltype(v.m_iPartyUID)>>::get(r);
        return v;
    }
};

struct KEPM_LEAVE_PARTY_REQ {
    std::int64_t m_iPartyUID{};  // UidType
    std::int32_t m_iReason{};  // int
    std::int64_t m_iNewJoinPartyUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iPartyUID)>>::put(w, m_iPartyUID);
        Codec<std::remove_cvref_t<decltype(m_iReason)>>::put(w, m_iReason);
        Codec<std::remove_cvref_t<decltype(m_iNewJoinPartyUID)>>::put(w, m_iNewJoinPartyUID);
    }

    static KEPM_LEAVE_PARTY_REQ read_from(Reader& r) {
        KEPM_LEAVE_PARTY_REQ v;
        v.m_iPartyUID = Codec<std::remove_cvref_t<decltype(v.m_iPartyUID)>>::get(r);
        v.m_iReason = Codec<std::remove_cvref_t<decltype(v.m_iReason)>>::get(r);
        v.m_iNewJoinPartyUID = Codec<std::remove_cvref_t<decltype(v.m_iNewJoinPartyUID)>>::get(r);
        return v;
    }
};

struct KEPM_PARTY_BAN_USER_NOT {
    std::int64_t m_iPartyUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iPartyUID)>>::put(w, m_iPartyUID);
    }

    static KEPM_PARTY_BAN_USER_NOT read_from(Reader& r) {
        KEPM_PARTY_BAN_USER_NOT v;
        v.m_iPartyUID = Codec<std::remove_cvref_t<decltype(v.m_iPartyUID)>>::get(r);
        return v;
    }
};

struct KEPM_PARTY_BAN_USER_REQ {
    std::int64_t m_iPartyUID{};  // UidType
    std::int64_t m_iUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iPartyUID)>>::put(w, m_iPartyUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
    }

    static KEPM_PARTY_BAN_USER_REQ read_from(Reader& r) {
        KEPM_PARTY_BAN_USER_REQ v;
        v.m_iPartyUID = Codec<std::remove_cvref_t<decltype(v.m_iPartyUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        return v;
    }
};

struct KEPM_PARTY_CHANGE_CHECK_MIN_LEVEL_REQ {
    std::int64_t m_iPartyUID{};  // UidType
    bool         m_bCheck{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iPartyUID)>>::put(w, m_iPartyUID);
        Codec<std::remove_cvref_t<decltype(m_bCheck)>>::put(w, m_bCheck);
    }

    static KEPM_PARTY_CHANGE_CHECK_MIN_LEVEL_REQ read_from(Reader& r) {
        KEPM_PARTY_CHANGE_CHECK_MIN_LEVEL_REQ v;
        v.m_iPartyUID = Codec<std::remove_cvref_t<decltype(v.m_iPartyUID)>>::get(r);
        v.m_bCheck = Codec<std::remove_cvref_t<decltype(v.m_bCheck)>>::get(r);
        return v;
    }
};

struct KEPM_PARTY_CHANGE_DUNGEON_REQ {
    std::int64_t m_iPartyUID{};  // UidType
    std::int32_t m_iDungeonID{};  // int
    char         m_DifficultyLevel{};
    char         m_cDungeonMode{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iPartyUID)>>::put(w, m_iPartyUID);
        Codec<std::remove_cvref_t<decltype(m_iDungeonID)>>::put(w, m_iDungeonID);
        Codec<std::remove_cvref_t<decltype(m_DifficultyLevel)>>::put(w, m_DifficultyLevel);
        Codec<std::remove_cvref_t<decltype(m_cDungeonMode)>>::put(w, m_cDungeonMode);
    }

    static KEPM_PARTY_CHANGE_DUNGEON_REQ read_from(Reader& r) {
        KEPM_PARTY_CHANGE_DUNGEON_REQ v;
        v.m_iPartyUID = Codec<std::remove_cvref_t<decltype(v.m_iPartyUID)>>::get(r);
        v.m_iDungeonID = Codec<std::remove_cvref_t<decltype(v.m_iDungeonID)>>::get(r);
        v.m_DifficultyLevel = Codec<std::remove_cvref_t<decltype(v.m_DifficultyLevel)>>::get(r);
        v.m_cDungeonMode = Codec<std::remove_cvref_t<decltype(v.m_cDungeonMode)>>::get(r);
        return v;
    }
};

struct KEPM_PARTY_CHANGE_GET_ITEM_TYPE_REQ {
    std::int64_t m_iPartyUID{};  // UidType
    char         m_cGetItemType{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iPartyUID)>>::put(w, m_iPartyUID);
        Codec<std::remove_cvref_t<decltype(m_cGetItemType)>>::put(w, m_cGetItemType);
    }

    static KEPM_PARTY_CHANGE_GET_ITEM_TYPE_REQ read_from(Reader& r) {
        KEPM_PARTY_CHANGE_GET_ITEM_TYPE_REQ v;
        v.m_iPartyUID = Codec<std::remove_cvref_t<decltype(v.m_iPartyUID)>>::get(r);
        v.m_cGetItemType = Codec<std::remove_cvref_t<decltype(v.m_cGetItemType)>>::get(r);
        return v;
    }
};

struct KEPM_PARTY_CHANGE_HOST_REQ {
    std::int64_t m_iPartyUID{};  // UidType
    std::int64_t m_iNewHostUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iPartyUID)>>::put(w, m_iPartyUID);
        Codec<std::remove_cvref_t<decltype(m_iNewHostUnitUID)>>::put(w, m_iNewHostUnitUID);
    }

    static KEPM_PARTY_CHANGE_HOST_REQ read_from(Reader& r) {
        KEPM_PARTY_CHANGE_HOST_REQ v;
        v.m_iPartyUID = Codec<std::remove_cvref_t<decltype(v.m_iPartyUID)>>::get(r);
        v.m_iNewHostUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iNewHostUnitUID)>>::get(r);
        return v;
    }
};

struct KEPM_PARTY_CHANGE_NUM_OF_PER_REQ {
    std::int64_t m_iPartyUID{};  // UidType
    char         m_cNumOfPer{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iPartyUID)>>::put(w, m_iPartyUID);
        Codec<std::remove_cvref_t<decltype(m_cNumOfPer)>>::put(w, m_cNumOfPer);
    }

    static KEPM_PARTY_CHANGE_NUM_OF_PER_REQ read_from(Reader& r) {
        KEPM_PARTY_CHANGE_NUM_OF_PER_REQ v;
        v.m_iPartyUID = Codec<std::remove_cvref_t<decltype(v.m_iPartyUID)>>::get(r);
        v.m_cNumOfPer = Codec<std::remove_cvref_t<decltype(v.m_cNumOfPer)>>::get(r);
        return v;
    }
};

struct KEPM_PARTY_CHANGE_READY_REQ {
    std::int64_t m_iPartyUID{};  // UidType
    bool         m_bReady{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iPartyUID)>>::put(w, m_iPartyUID);
        Codec<std::remove_cvref_t<decltype(m_bReady)>>::put(w, m_bReady);
    }

    static KEPM_PARTY_CHANGE_READY_REQ read_from(Reader& r) {
        KEPM_PARTY_CHANGE_READY_REQ v;
        v.m_iPartyUID = Codec<std::remove_cvref_t<decltype(v.m_iPartyUID)>>::get(r);
        v.m_bReady = Codec<std::remove_cvref_t<decltype(v.m_bReady)>>::get(r);
        return v;
    }
};

struct KEPM_PARTY_GAME_START_ACK {
    std::int32_t   m_iOK{};  // int
    std::uint16_t  m_usEventID{};  // unsigned short
    std::int32_t   m_iDungeonMode{};  // int
    std::int32_t   m_iDungeonID{};  // int
    std::int64_t   m_iFailUnitUID{};  // UidType
    std::u16string m_wstrFailUserNickName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_usEventID)>>::put(w, m_usEventID);
        Codec<std::remove_cvref_t<decltype(m_iDungeonMode)>>::put(w, m_iDungeonMode);
        Codec<std::remove_cvref_t<decltype(m_iDungeonID)>>::put(w, m_iDungeonID);
        Codec<std::remove_cvref_t<decltype(m_iFailUnitUID)>>::put(w, m_iFailUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrFailUserNickName)>>::put(w, m_wstrFailUserNickName);
    }

    static KEPM_PARTY_GAME_START_ACK read_from(Reader& r) {
        KEPM_PARTY_GAME_START_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_usEventID = Codec<std::remove_cvref_t<decltype(v.m_usEventID)>>::get(r);
        v.m_iDungeonMode = Codec<std::remove_cvref_t<decltype(v.m_iDungeonMode)>>::get(r);
        v.m_iDungeonID = Codec<std::remove_cvref_t<decltype(v.m_iDungeonID)>>::get(r);
        v.m_iFailUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iFailUnitUID)>>::get(r);
        v.m_wstrFailUserNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrFailUserNickName)>>::get(r);
        return v;
    }
};

struct KEPM_PARTY_GAME_START_REQ {
    std::int64_t  m_iPartyUID{};  // UidType
    std::int16_t  m_sWorldID{};  // short
    std::uint16_t m_usEventID{};  // unsigned short

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iPartyUID)>>::put(w, m_iPartyUID);
        Codec<std::remove_cvref_t<decltype(m_sWorldID)>>::put(w, m_sWorldID);
        Codec<std::remove_cvref_t<decltype(m_usEventID)>>::put(w, m_usEventID);
    }

    static KEPM_PARTY_GAME_START_REQ read_from(Reader& r) {
        KEPM_PARTY_GAME_START_REQ v;
        v.m_iPartyUID = Codec<std::remove_cvref_t<decltype(v.m_iPartyUID)>>::get(r);
        v.m_sWorldID = Codec<std::remove_cvref_t<decltype(v.m_sWorldID)>>::get(r);
        v.m_usEventID = Codec<std::remove_cvref_t<decltype(v.m_usEventID)>>::get(r);
        return v;
    }
};

struct KEPM_REGROUP_PARTY_FOR_AUTO_PARTY_DUNGEON_GAME_ACK {
    std::int32_t m_iOK{};  // int
    std::int64_t m_iAutoPartyUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iAutoPartyUID)>>::put(w, m_iAutoPartyUID);
    }

    static KEPM_REGROUP_PARTY_FOR_AUTO_PARTY_DUNGEON_GAME_ACK read_from(Reader& r) {
        KEPM_REGROUP_PARTY_FOR_AUTO_PARTY_DUNGEON_GAME_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iAutoPartyUID = Codec<std::remove_cvref_t<decltype(v.m_iAutoPartyUID)>>::get(r);
        return v;
    }
};

struct KEPM_REQUEST_UPDATE_PARTY_MEMBER_LIST_NOT {
    std::int64_t m_iPartyUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iPartyUID)>>::put(w, m_iPartyUID);
    }

    static KEPM_REQUEST_UPDATE_PARTY_MEMBER_LIST_NOT read_from(Reader& r) {
        KEPM_REQUEST_UPDATE_PARTY_MEMBER_LIST_NOT v;
        v.m_iPartyUID = Codec<std::remove_cvref_t<decltype(v.m_iPartyUID)>>::get(r);
        return v;
    }
};

struct KEPM_SEND_UPDATE_PARTY_MEMBER_LIST_NOT {
    std::int64_t           m_iPartyUID{};  // UidType
    std::set<std::int64_t> m_setPartyUIDList{};  // std::set< UidType >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iPartyUID)>>::put(w, m_iPartyUID);
        Codec<std::remove_cvref_t<decltype(m_setPartyUIDList)>>::put(w, m_setPartyUIDList);
    }

    static KEPM_SEND_UPDATE_PARTY_MEMBER_LIST_NOT read_from(Reader& r) {
        KEPM_SEND_UPDATE_PARTY_MEMBER_LIST_NOT v;
        v.m_iPartyUID = Codec<std::remove_cvref_t<decltype(v.m_iPartyUID)>>::get(r);
        v.m_setPartyUIDList = Codec<std::remove_cvref_t<decltype(v.m_setPartyUIDList)>>::get(r);
        return v;
    }
};

struct KEPM_UPDATE_PARTY_FEVER_IN_PARTY_LIST_NOT {
    std::int64_t m_iPartyUID{};  // UidType
    std::int16_t m_sPartyFever{};  // short

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iPartyUID)>>::put(w, m_iPartyUID);
        Codec<std::remove_cvref_t<decltype(m_sPartyFever)>>::put(w, m_sPartyFever);
    }

    static KEPM_UPDATE_PARTY_FEVER_IN_PARTY_LIST_NOT read_from(Reader& r) {
        KEPM_UPDATE_PARTY_FEVER_IN_PARTY_LIST_NOT v;
        v.m_iPartyUID = Codec<std::remove_cvref_t<decltype(v.m_iPartyUID)>>::get(r);
        v.m_sPartyFever = Codec<std::remove_cvref_t<decltype(v.m_sPartyFever)>>::get(r);
        return v;
    }
};

struct KEPM_UPDATE_PARTY_USER_INFO_NOT {
    std::int64_t m_iPartyUID{};  // UidType
    bool         m_bChangeField{};
    std::int64_t m_iUnitUID{};  // UidType
    std::uint8_t m_cState{};  // UCHAR
    std::int32_t m_iStateCode{};  // int
    char         m_cUnitClass{};
    std::uint8_t m_ucLevel{};  // UCHAR

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iPartyUID)>>::put(w, m_iPartyUID);
        Codec<std::remove_cvref_t<decltype(m_bChangeField)>>::put(w, m_bChangeField);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_cState)>>::put(w, m_cState);
        Codec<std::remove_cvref_t<decltype(m_iStateCode)>>::put(w, m_iStateCode);
        Codec<std::remove_cvref_t<decltype(m_cUnitClass)>>::put(w, m_cUnitClass);
        Codec<std::remove_cvref_t<decltype(m_ucLevel)>>::put(w, m_ucLevel);
    }

    static KEPM_UPDATE_PARTY_USER_INFO_NOT read_from(Reader& r) {
        KEPM_UPDATE_PARTY_USER_INFO_NOT v;
        v.m_iPartyUID = Codec<std::remove_cvref_t<decltype(v.m_iPartyUID)>>::get(r);
        v.m_bChangeField = Codec<std::remove_cvref_t<decltype(v.m_bChangeField)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_cState = Codec<std::remove_cvref_t<decltype(v.m_cState)>>::get(r);
        v.m_iStateCode = Codec<std::remove_cvref_t<decltype(v.m_iStateCode)>>::get(r);
        v.m_cUnitClass = Codec<std::remove_cvref_t<decltype(v.m_cUnitClass)>>::get(r);
        v.m_ucLevel = Codec<std::remove_cvref_t<decltype(v.m_ucLevel)>>::get(r);
        return v;
    }
};

struct KEPM_UPDATE_PVP_INFO_NOT {
    char         m_cRank{};
    std::int32_t m_iOfficialMatchCnt{};  // int
    std::int32_t m_iRating{};  // int
    std::int32_t m_iRPoint{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cRank)>>::put(w, m_cRank);
        Codec<std::remove_cvref_t<decltype(m_iOfficialMatchCnt)>>::put(w, m_iOfficialMatchCnt);
        Codec<std::remove_cvref_t<decltype(m_iRating)>>::put(w, m_iRating);
        Codec<std::remove_cvref_t<decltype(m_iRPoint)>>::put(w, m_iRPoint);
    }

    static KEPM_UPDATE_PVP_INFO_NOT read_from(Reader& r) {
        KEPM_UPDATE_PVP_INFO_NOT v;
        v.m_cRank = Codec<std::remove_cvref_t<decltype(v.m_cRank)>>::get(r);
        v.m_iOfficialMatchCnt = Codec<std::remove_cvref_t<decltype(v.m_iOfficialMatchCnt)>>::get(r);
        v.m_iRating = Codec<std::remove_cvref_t<decltype(v.m_iRating)>>::get(r);
        v.m_iRPoint = Codec<std::remove_cvref_t<decltype(v.m_iRPoint)>>::get(r);
        return v;
    }
};

struct KEPUBLISHER_AUTHENTICATION_ACK {
    std::int32_t   m_iOK{};  // int
    std::int32_t   m_iResult{};  // int
    std::u16string m_wstrID{};  // std::wstring
    std::uint32_t  m_uiPublisherUID{};  // unsigned int
    std::u16string m_wstrIP{};  // std::wstring
    std::int32_t   m_iChannelingCode{};  // int
    std::u16string wstrPurchTok{};  // std::wstring
    std::uint32_t  m_uiAccountType{};  // unsigned int
    std::uint32_t  m_uiAccountState{};  // unsigned int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iResult)>>::put(w, m_iResult);
        Codec<std::remove_cvref_t<decltype(m_wstrID)>>::put(w, m_wstrID);
        Codec<std::remove_cvref_t<decltype(m_uiPublisherUID)>>::put(w, m_uiPublisherUID);
        Codec<std::remove_cvref_t<decltype(m_wstrIP)>>::put(w, m_wstrIP);
        Codec<std::remove_cvref_t<decltype(m_iChannelingCode)>>::put(w, m_iChannelingCode);
        Codec<std::remove_cvref_t<decltype(wstrPurchTok)>>::put(w, wstrPurchTok);
        Codec<std::remove_cvref_t<decltype(m_uiAccountType)>>::put(w, m_uiAccountType);
        Codec<std::remove_cvref_t<decltype(m_uiAccountState)>>::put(w, m_uiAccountState);
    }

    static KEPUBLISHER_AUTHENTICATION_ACK read_from(Reader& r) {
        KEPUBLISHER_AUTHENTICATION_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iResult = Codec<std::remove_cvref_t<decltype(v.m_iResult)>>::get(r);
        v.m_wstrID = Codec<std::remove_cvref_t<decltype(v.m_wstrID)>>::get(r);
        v.m_uiPublisherUID = Codec<std::remove_cvref_t<decltype(v.m_uiPublisherUID)>>::get(r);
        v.m_wstrIP = Codec<std::remove_cvref_t<decltype(v.m_wstrIP)>>::get(r);
        v.m_iChannelingCode = Codec<std::remove_cvref_t<decltype(v.m_iChannelingCode)>>::get(r);
        v.wstrPurchTok = Codec<std::remove_cvref_t<decltype(v.wstrPurchTok)>>::get(r);
        v.m_uiAccountType = Codec<std::remove_cvref_t<decltype(v.m_uiAccountType)>>::get(r);
        v.m_uiAccountState = Codec<std::remove_cvref_t<decltype(v.m_uiAccountState)>>::get(r);
        return v;
    }
};

struct KEPUBLISHER_AUTHENTICATION_REQ {
    std::int64_t   m_iActorUID{};  // UidType
    std::u16string m_wstrIP{};  // std::wstring
    std::u16string m_wstrServiceAccountID{};  // std::wstring
    std::u16string m_wstrServicePassword{};  // std::wstring
    std::int32_t   m_iChannelingCode{};  // int
    bool           m_bServerUseKogOTP{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iActorUID)>>::put(w, m_iActorUID);
        Codec<std::remove_cvref_t<decltype(m_wstrIP)>>::put(w, m_wstrIP);
        Codec<std::remove_cvref_t<decltype(m_wstrServiceAccountID)>>::put(w, m_wstrServiceAccountID);
        Codec<std::remove_cvref_t<decltype(m_wstrServicePassword)>>::put(w, m_wstrServicePassword);
        Codec<std::remove_cvref_t<decltype(m_iChannelingCode)>>::put(w, m_iChannelingCode);
        Codec<std::remove_cvref_t<decltype(m_bServerUseKogOTP)>>::put(w, m_bServerUseKogOTP);
    }

    static KEPUBLISHER_AUTHENTICATION_REQ read_from(Reader& r) {
        KEPUBLISHER_AUTHENTICATION_REQ v;
        v.m_iActorUID = Codec<std::remove_cvref_t<decltype(v.m_iActorUID)>>::get(r);
        v.m_wstrIP = Codec<std::remove_cvref_t<decltype(v.m_wstrIP)>>::get(r);
        v.m_wstrServiceAccountID = Codec<std::remove_cvref_t<decltype(v.m_wstrServiceAccountID)>>::get(r);
        v.m_wstrServicePassword = Codec<std::remove_cvref_t<decltype(v.m_wstrServicePassword)>>::get(r);
        v.m_iChannelingCode = Codec<std::remove_cvref_t<decltype(v.m_iChannelingCode)>>::get(r);
        v.m_bServerUseKogOTP = Codec<std::remove_cvref_t<decltype(v.m_bServerUseKogOTP)>>::get(r);
        return v;
    }
};

struct KEPUBLISHER_BILLING_BALANCE_ACK {
    std::int32_t  m_iOK{};  // int
    std::uint32_t m_ulBalance{};  // unsigned long
    std::int32_t  m_iCashType{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_ulBalance)>>::put(w, m_ulBalance);
        Codec<std::remove_cvref_t<decltype(m_iCashType)>>::put(w, m_iCashType);
    }

    static KEPUBLISHER_BILLING_BALANCE_ACK read_from(Reader& r) {
        KEPUBLISHER_BILLING_BALANCE_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_ulBalance = Codec<std::remove_cvref_t<decltype(v.m_ulBalance)>>::get(r);
        v.m_iCashType = Codec<std::remove_cvref_t<decltype(v.m_iCashType)>>::get(r);
        return v;
    }
};

struct KEPUBLISHER_BILLING_BALANCE_REQ {
    std::uint32_t  m_uiPublisherUID{};  // unsigned int
    std::int64_t   m_uiActorUID{};  // UidType
    std::u16string m_wstrAccount{};  // std::wstring
    std::u16string m_wstrIP{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_uiPublisherUID)>>::put(w, m_uiPublisherUID);
        Codec<std::remove_cvref_t<decltype(m_uiActorUID)>>::put(w, m_uiActorUID);
        Codec<std::remove_cvref_t<decltype(m_wstrAccount)>>::put(w, m_wstrAccount);
        Codec<std::remove_cvref_t<decltype(m_wstrIP)>>::put(w, m_wstrIP);
    }

    static KEPUBLISHER_BILLING_BALANCE_REQ read_from(Reader& r) {
        KEPUBLISHER_BILLING_BALANCE_REQ v;
        v.m_uiPublisherUID = Codec<std::remove_cvref_t<decltype(v.m_uiPublisherUID)>>::get(r);
        v.m_uiActorUID = Codec<std::remove_cvref_t<decltype(v.m_uiActorUID)>>::get(r);
        v.m_wstrAccount = Codec<std::remove_cvref_t<decltype(v.m_wstrAccount)>>::get(r);
        v.m_wstrIP = Codec<std::remove_cvref_t<decltype(v.m_wstrIP)>>::get(r);
        return v;
    }
};

struct KEBILL_GET_TRANS_FOR_CHECK_BALANCE_ACK {
    std::int32_t                    m_iOK{};  // int
    std::u16string                  m_wstrTransaction{};  // std::wstring
    KEPUBLISHER_BILLING_BALANCE_REQ m_kCheckGiantBalanceReq{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_wstrTransaction)>>::put(w, m_wstrTransaction);
        Codec<std::remove_cvref_t<decltype(m_kCheckGiantBalanceReq)>>::put(w, m_kCheckGiantBalanceReq);
    }

    static KEBILL_GET_TRANS_FOR_CHECK_BALANCE_ACK read_from(Reader& r) {
        KEBILL_GET_TRANS_FOR_CHECK_BALANCE_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_wstrTransaction = Codec<std::remove_cvref_t<decltype(v.m_wstrTransaction)>>::get(r);
        v.m_kCheckGiantBalanceReq = Codec<std::remove_cvref_t<decltype(v.m_kCheckGiantBalanceReq)>>::get(r);
        return v;
    }
};

struct KEBILL_GET_TRANS_FOR_CHECK_BALANCE_REQ {
    KEPUBLISHER_BILLING_BALANCE_REQ m_kCheckGiantBalanceReq{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kCheckGiantBalanceReq)>>::put(w, m_kCheckGiantBalanceReq);
    }

    static KEBILL_GET_TRANS_FOR_CHECK_BALANCE_REQ read_from(Reader& r) {
        KEBILL_GET_TRANS_FOR_CHECK_BALANCE_REQ v;
        v.m_kCheckGiantBalanceReq = Codec<std::remove_cvref_t<decltype(v.m_kCheckGiantBalanceReq)>>::get(r);
        return v;
    }
};

struct KEPUBLISHER_CHARGE_POINT_ACK {
    std::int32_t m_iOK{};  // int
    std::int32_t m_iChargedCash{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iChargedCash)>>::put(w, m_iChargedCash);
    }

    static KEPUBLISHER_CHARGE_POINT_ACK read_from(Reader& r) {
        KEPUBLISHER_CHARGE_POINT_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iChargedCash = Codec<std::remove_cvref_t<decltype(v.m_iChargedCash)>>::get(r);
        return v;
    }
};

struct KEPUBLISHER_CHARGE_POINT_REQ {
    std::int32_t   m_iServerGroupID{};  // int
    std::u16string m_wstrUserID{};  // std::wstring
    std::int64_t   m_iCharUID{};  // UidType
    std::int64_t   m_iItemUID{};  // UidType
    std::int32_t   m_iItemID{};  // int
    std::u16string m_wstrUserIP{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iServerGroupID)>>::put(w, m_iServerGroupID);
        Codec<std::remove_cvref_t<decltype(m_wstrUserID)>>::put(w, m_wstrUserID);
        Codec<std::remove_cvref_t<decltype(m_iCharUID)>>::put(w, m_iCharUID);
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_iItemID)>>::put(w, m_iItemID);
        Codec<std::remove_cvref_t<decltype(m_wstrUserIP)>>::put(w, m_wstrUserIP);
    }

    static KEPUBLISHER_CHARGE_POINT_REQ read_from(Reader& r) {
        KEPUBLISHER_CHARGE_POINT_REQ v;
        v.m_iServerGroupID = Codec<std::remove_cvref_t<decltype(v.m_iServerGroupID)>>::get(r);
        v.m_wstrUserID = Codec<std::remove_cvref_t<decltype(v.m_wstrUserID)>>::get(r);
        v.m_iCharUID = Codec<std::remove_cvref_t<decltype(v.m_iCharUID)>>::get(r);
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_iItemID = Codec<std::remove_cvref_t<decltype(v.m_iItemID)>>::get(r);
        v.m_wstrUserIP = Codec<std::remove_cvref_t<decltype(v.m_wstrUserIP)>>::get(r);
        return v;
    }
};

struct KEPUBLISHER_REG_USER_ACK {
    std::int32_t   m_iOK{};  // int
    std::u16string m_wstrServiceAccountID{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_wstrServiceAccountID)>>::put(w, m_wstrServiceAccountID);
    }

    static KEPUBLISHER_REG_USER_ACK read_from(Reader& r) {
        KEPUBLISHER_REG_USER_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_wstrServiceAccountID = Codec<std::remove_cvref_t<decltype(v.m_wstrServiceAccountID)>>::get(r);
        return v;
    }
};

struct KEPUBLISHER_REG_USER_REQ {
    std::int64_t   m_iActorUID{};  // UidType
    std::u16string m_wstrServiceAccountID{};  // std::wstring
    std::u16string m_wstrServicePassword{};  // std::wstring
    std::string    m_strIP{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iActorUID)>>::put(w, m_iActorUID);
        Codec<std::remove_cvref_t<decltype(m_wstrServiceAccountID)>>::put(w, m_wstrServiceAccountID);
        Codec<std::remove_cvref_t<decltype(m_wstrServicePassword)>>::put(w, m_wstrServicePassword);
        Codec<std::remove_cvref_t<decltype(m_strIP)>>::put(w, m_strIP);
    }

    static KEPUBLISHER_REG_USER_REQ read_from(Reader& r) {
        KEPUBLISHER_REG_USER_REQ v;
        v.m_iActorUID = Codec<std::remove_cvref_t<decltype(v.m_iActorUID)>>::get(r);
        v.m_wstrServiceAccountID = Codec<std::remove_cvref_t<decltype(v.m_wstrServiceAccountID)>>::get(r);
        v.m_wstrServicePassword = Codec<std::remove_cvref_t<decltype(v.m_wstrServicePassword)>>::get(r);
        v.m_strIP = Codec<std::remove_cvref_t<decltype(v.m_strIP)>>::get(r);
        return v;
    }
};

struct KEPUBLISHER_REPORT_HACK_USER_NOT {
    std::int64_t   m_iUserUID{};  // UidType
    std::u16string m_wstrUserID{};  // std::wstring
    std::int64_t   m_iUnitUID{};  // UidType
    std::u16string m_wstrNickName{};  // std::wstring
    std::int64_t   m_iServerUID{};  // UidType
    std::string    m_strIP{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
        Codec<std::remove_cvref_t<decltype(m_wstrUserID)>>::put(w, m_wstrUserID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_iServerUID)>>::put(w, m_iServerUID);
        Codec<std::remove_cvref_t<decltype(m_strIP)>>::put(w, m_strIP);
    }

    static KEPUBLISHER_REPORT_HACK_USER_NOT read_from(Reader& r) {
        KEPUBLISHER_REPORT_HACK_USER_NOT v;
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        v.m_wstrUserID = Codec<std::remove_cvref_t<decltype(v.m_wstrUserID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_iServerUID = Codec<std::remove_cvref_t<decltype(v.m_iServerUID)>>::get(r);
        v.m_strIP = Codec<std::remove_cvref_t<decltype(v.m_strIP)>>::get(r);
        return v;
    }
};

struct KEPUBLISHER_SECURITY_AUTH_ACK {
    std::int32_t m_iOK{};  // int
    std::string  m_strSecondSecurityURL{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_strSecondSecurityURL)>>::put(w, m_strSecondSecurityURL);
    }

    static KEPUBLISHER_SECURITY_AUTH_ACK read_from(Reader& r) {
        KEPUBLISHER_SECURITY_AUTH_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_strSecondSecurityURL = Codec<std::remove_cvref_t<decltype(v.m_strSecondSecurityURL)>>::get(r);
        return v;
    }
};

struct KEPUBLISHER_SECURITY_AUTH_REQ {
    std::int32_t   m_iSecurityAuthType{};  // int
    std::int64_t   m_iUserUID{};  // UidType
    std::u16string m_wstrUserID{};  // std::wstring
    std::uint32_t  m_uiPublisherUID{};  // unsigned int
    std::u16string m_wstrIP{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iSecurityAuthType)>>::put(w, m_iSecurityAuthType);
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
        Codec<std::remove_cvref_t<decltype(m_wstrUserID)>>::put(w, m_wstrUserID);
        Codec<std::remove_cvref_t<decltype(m_uiPublisherUID)>>::put(w, m_uiPublisherUID);
        Codec<std::remove_cvref_t<decltype(m_wstrIP)>>::put(w, m_wstrIP);
    }

    static KEPUBLISHER_SECURITY_AUTH_REQ read_from(Reader& r) {
        KEPUBLISHER_SECURITY_AUTH_REQ v;
        v.m_iSecurityAuthType = Codec<std::remove_cvref_t<decltype(v.m_iSecurityAuthType)>>::get(r);
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        v.m_wstrUserID = Codec<std::remove_cvref_t<decltype(v.m_wstrUserID)>>::get(r);
        v.m_uiPublisherUID = Codec<std::remove_cvref_t<decltype(v.m_uiPublisherUID)>>::get(r);
        v.m_wstrIP = Codec<std::remove_cvref_t<decltype(v.m_wstrIP)>>::get(r);
        return v;
    }
};

struct KEPUBLISHER_SECURITY_INFO_NOT {
    std::int32_t   m_iSecurityInfoType{};  // int
    std::uint32_t  m_uiMatrixKey{};  // unsigned int
    std::u16string m_wstrCoordinate{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iSecurityInfoType)>>::put(w, m_iSecurityInfoType);
        Codec<std::remove_cvref_t<decltype(m_uiMatrixKey)>>::put(w, m_uiMatrixKey);
        Codec<std::remove_cvref_t<decltype(m_wstrCoordinate)>>::put(w, m_wstrCoordinate);
    }

    static KEPUBLISHER_SECURITY_INFO_NOT read_from(Reader& r) {
        KEPUBLISHER_SECURITY_INFO_NOT v;
        v.m_iSecurityInfoType = Codec<std::remove_cvref_t<decltype(v.m_iSecurityInfoType)>>::get(r);
        v.m_uiMatrixKey = Codec<std::remove_cvref_t<decltype(v.m_uiMatrixKey)>>::get(r);
        v.m_wstrCoordinate = Codec<std::remove_cvref_t<decltype(v.m_wstrCoordinate)>>::get(r);
        return v;
    }
};

struct KEPUBLISHER_UNREG_USER_ACK {
    std::int32_t   m_iOK{};  // int
    std::u16string m_wstrServiceAccountID{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_wstrServiceAccountID)>>::put(w, m_wstrServiceAccountID);
    }

    static KEPUBLISHER_UNREG_USER_ACK read_from(Reader& r) {
        KEPUBLISHER_UNREG_USER_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_wstrServiceAccountID = Codec<std::remove_cvref_t<decltype(v.m_wstrServiceAccountID)>>::get(r);
        return v;
    }
};

struct KEPUBLISHER_UNREG_USER_REQ {
    std::int64_t   m_iActorUID{};  // UidType
    std::u16string m_wstrServiceAccountID{};  // std::wstring
    std::u16string m_wstrServicePassword{};  // std::wstring
    std::string    m_strIP{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iActorUID)>>::put(w, m_iActorUID);
        Codec<std::remove_cvref_t<decltype(m_wstrServiceAccountID)>>::put(w, m_wstrServiceAccountID);
        Codec<std::remove_cvref_t<decltype(m_wstrServicePassword)>>::put(w, m_wstrServicePassword);
        Codec<std::remove_cvref_t<decltype(m_strIP)>>::put(w, m_strIP);
    }

    static KEPUBLISHER_UNREG_USER_REQ read_from(Reader& r) {
        KEPUBLISHER_UNREG_USER_REQ v;
        v.m_iActorUID = Codec<std::remove_cvref_t<decltype(v.m_iActorUID)>>::get(r);
        v.m_wstrServiceAccountID = Codec<std::remove_cvref_t<decltype(v.m_wstrServiceAccountID)>>::get(r);
        v.m_wstrServicePassword = Codec<std::remove_cvref_t<decltype(v.m_wstrServicePassword)>>::get(r);
        v.m_strIP = Codec<std::remove_cvref_t<decltype(v.m_strIP)>>::get(r);
        return v;
    }
};

struct KERM_ACTIVATE_BUFF_NOT {
    std::vector<KBuffInfo> m_vecActivateBuffList{};  // std::vector< KBuffInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecActivateBuffList)>>::put(w, m_vecActivateBuffList);
    }

    static KERM_ACTIVATE_BUFF_NOT read_from(Reader& r) {
        KERM_ACTIVATE_BUFF_NOT v;
        v.m_vecActivateBuffList = Codec<std::remove_cvref_t<decltype(v.m_vecActivateBuffList)>>::get(r);
        return v;
    }
};

struct KERM_BAD_ATTITUDE_USER_LOG_NOT {
    std::int64_t   m_iUnitUID{};  // UidType
    std::int32_t   m_iDungeonIDAndDif{};  // int
    std::int16_t   m_sStartMember{};  // short
    std::int16_t   m_sNowMember{};  // short
    std::int16_t   m_sVoteCount{};  // short
    std::int16_t   m_sExitStage{};  // short
    std::int16_t   m_sExitSubStage{};  // short
    std::int16_t   m_sBadAttitudePoint{};  // short
    std::u16string m_wstrRegDate{};  // std::wstring
    bool           m_bExitResult{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iDungeonIDAndDif)>>::put(w, m_iDungeonIDAndDif);
        Codec<std::remove_cvref_t<decltype(m_sStartMember)>>::put(w, m_sStartMember);
        Codec<std::remove_cvref_t<decltype(m_sNowMember)>>::put(w, m_sNowMember);
        Codec<std::remove_cvref_t<decltype(m_sVoteCount)>>::put(w, m_sVoteCount);
        Codec<std::remove_cvref_t<decltype(m_sExitStage)>>::put(w, m_sExitStage);
        Codec<std::remove_cvref_t<decltype(m_sExitSubStage)>>::put(w, m_sExitSubStage);
        Codec<std::remove_cvref_t<decltype(m_sBadAttitudePoint)>>::put(w, m_sBadAttitudePoint);
        Codec<std::remove_cvref_t<decltype(m_wstrRegDate)>>::put(w, m_wstrRegDate);
        Codec<std::remove_cvref_t<decltype(m_bExitResult)>>::put(w, m_bExitResult);
    }

    static KERM_BAD_ATTITUDE_USER_LOG_NOT read_from(Reader& r) {
        KERM_BAD_ATTITUDE_USER_LOG_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iDungeonIDAndDif = Codec<std::remove_cvref_t<decltype(v.m_iDungeonIDAndDif)>>::get(r);
        v.m_sStartMember = Codec<std::remove_cvref_t<decltype(v.m_sStartMember)>>::get(r);
        v.m_sNowMember = Codec<std::remove_cvref_t<decltype(v.m_sNowMember)>>::get(r);
        v.m_sVoteCount = Codec<std::remove_cvref_t<decltype(v.m_sVoteCount)>>::get(r);
        v.m_sExitStage = Codec<std::remove_cvref_t<decltype(v.m_sExitStage)>>::get(r);
        v.m_sExitSubStage = Codec<std::remove_cvref_t<decltype(v.m_sExitSubStage)>>::get(r);
        v.m_sBadAttitudePoint = Codec<std::remove_cvref_t<decltype(v.m_sBadAttitudePoint)>>::get(r);
        v.m_wstrRegDate = Codec<std::remove_cvref_t<decltype(v.m_wstrRegDate)>>::get(r);
        v.m_bExitResult = Codec<std::remove_cvref_t<decltype(v.m_bExitResult)>>::get(r);
        return v;
    }
};

struct KERM_BATTLE_FIELD_ROOM_INFO_NOT {
    char                 m_cType{};
    KBattleFieldRoomInfo m_kBFRoomInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cType)>>::put(w, m_cType);
        Codec<std::remove_cvref_t<decltype(m_kBFRoomInfo)>>::put(w, m_kBFRoomInfo);
    }

    static KERM_BATTLE_FIELD_ROOM_INFO_NOT read_from(Reader& r) {
        KERM_BATTLE_FIELD_ROOM_INFO_NOT v;
        v.m_cType = Codec<std::remove_cvref_t<decltype(v.m_cType)>>::get(r);
        v.m_kBFRoomInfo = Codec<std::remove_cvref_t<decltype(v.m_kBFRoomInfo)>>::get(r);
        return v;
    }
};

struct KERM_BATTLE_FIELD_ZU_ECHO_REQ {
    std::int64_t m_iUnitUID{};  // UidType
    std::int64_t m_iRoomUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iRoomUID)>>::put(w, m_iRoomUID);
    }

    static KERM_BATTLE_FIELD_ZU_ECHO_REQ read_from(Reader& r) {
        KERM_BATTLE_FIELD_ZU_ECHO_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iRoomUID = Codec<std::remove_cvref_t<decltype(v.m_iRoomUID)>>::get(r);
        return v;
    }
};

struct KERM_BREAK_PERSONAL_SHOP_NOT {
    std::int32_t m_iReason{};  // int
    std::int64_t m_iHostUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iReason)>>::put(w, m_iReason);
        Codec<std::remove_cvref_t<decltype(m_iHostUID)>>::put(w, m_iHostUID);
    }

    static KERM_BREAK_PERSONAL_SHOP_NOT read_from(Reader& r) {
        KERM_BREAK_PERSONAL_SHOP_NOT v;
        v.m_iReason = Codec<std::remove_cvref_t<decltype(v.m_iReason)>>::get(r);
        v.m_iHostUID = Codec<std::remove_cvref_t<decltype(v.m_iHostUID)>>::get(r);
        return v;
    }
};

struct KERM_BREAK_PSHOP_AGENCY_NOT {
    std::int32_t m_iReason{};  // int
    std::int64_t m_iHostGSUID{};  // UidType
    std::int64_t m_iHostUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iReason)>>::put(w, m_iReason);
        Codec<std::remove_cvref_t<decltype(m_iHostGSUID)>>::put(w, m_iHostGSUID);
        Codec<std::remove_cvref_t<decltype(m_iHostUID)>>::put(w, m_iHostUID);
    }

    static KERM_BREAK_PSHOP_AGENCY_NOT read_from(Reader& r) {
        KERM_BREAK_PSHOP_AGENCY_NOT v;
        v.m_iReason = Codec<std::remove_cvref_t<decltype(v.m_iReason)>>::get(r);
        v.m_iHostGSUID = Codec<std::remove_cvref_t<decltype(v.m_iHostGSUID)>>::get(r);
        v.m_iHostUID = Codec<std::remove_cvref_t<decltype(v.m_iHostUID)>>::get(r);
        return v;
    }
};

struct KERM_CHANGE_DEFENCE_DUNGEON_PHASE_NOT {
    std::int32_t m_iDefenceDungeonPhase{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iDefenceDungeonPhase)>>::put(w, m_iDefenceDungeonPhase);
    }

    static KERM_CHANGE_DEFENCE_DUNGEON_PHASE_NOT read_from(Reader& r) {
        KERM_CHANGE_DEFENCE_DUNGEON_PHASE_NOT v;
        v.m_iDefenceDungeonPhase = Codec<std::remove_cvref_t<decltype(v.m_iDefenceDungeonPhase)>>::get(r);
        return v;
    }
};

struct KERM_CHANGE_READY_ACK {
    std::int32_t m_iOK{};  // int
    std::int32_t m_iDungeonID{};  // int
    char         m_cDungeonMode{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iDungeonID)>>::put(w, m_iDungeonID);
        Codec<std::remove_cvref_t<decltype(m_cDungeonMode)>>::put(w, m_cDungeonMode);
    }

    static KERM_CHANGE_READY_ACK read_from(Reader& r) {
        KERM_CHANGE_READY_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iDungeonID = Codec<std::remove_cvref_t<decltype(v.m_iDungeonID)>>::get(r);
        v.m_cDungeonMode = Codec<std::remove_cvref_t<decltype(v.m_cDungeonMode)>>::get(r);
        return v;
    }
};

struct KERM_CHAT_ACK {
    std::int32_t m_iOK{};  // int
    char         m_cChatPacketType{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_cChatPacketType)>>::put(w, m_cChatPacketType);
    }

    static KERM_CHAT_ACK read_from(Reader& r) {
        KERM_CHAT_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_cChatPacketType = Codec<std::remove_cvref_t<decltype(v.m_cChatPacketType)>>::get(r);
        return v;
    }
};

struct KERM_CHECK_INVALID_ZOMBIE_USER_NOT {
    std::int64_t m_iRoomUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iRoomUID)>>::put(w, m_iRoomUID);
    }

    static KERM_CHECK_INVALID_ZOMBIE_USER_NOT read_from(Reader& r) {
        KERM_CHECK_INVALID_ZOMBIE_USER_NOT v;
        v.m_iRoomUID = Codec<std::remove_cvref_t<decltype(v.m_iRoomUID)>>::get(r);
        return v;
    }
};

struct KERM_CHECK_JOIN_PERSONAL_SHOP_ACK {
    std::int32_t m_iOK{};  // int
    std::int64_t m_iPersonalShopUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iPersonalShopUID)>>::put(w, m_iPersonalShopUID);
    }

    static KERM_CHECK_JOIN_PERSONAL_SHOP_ACK read_from(Reader& r) {
        KERM_CHECK_JOIN_PERSONAL_SHOP_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iPersonalShopUID = Codec<std::remove_cvref_t<decltype(v.m_iPersonalShopUID)>>::get(r);
        return v;
    }
};

struct KERM_CHECK_MY_PSHOP_AGENCY_INFO_ACK {
    std::int64_t m_iPShopAgencyUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iPShopAgencyUID)>>::put(w, m_iPShopAgencyUID);
    }

    static KERM_CHECK_MY_PSHOP_AGENCY_INFO_ACK read_from(Reader& r) {
        KERM_CHECK_MY_PSHOP_AGENCY_INFO_ACK v;
        v.m_iPShopAgencyUID = Codec<std::remove_cvref_t<decltype(v.m_iPShopAgencyUID)>>::get(r);
        return v;
    }
};

struct KERM_CHECK_SELL_ED_ITEM_ACK {
    std::int32_t m_iOK{};  // int
    std::int64_t m_iItemUID{};  // UidType
    std::int32_t m_iQuantity{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_iQuantity)>>::put(w, m_iQuantity);
    }

    static KERM_CHECK_SELL_ED_ITEM_ACK read_from(Reader& r) {
        KERM_CHECK_SELL_ED_ITEM_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_iQuantity = Codec<std::remove_cvref_t<decltype(v.m_iQuantity)>>::get(r);
        return v;
    }
};

struct KERM_CHECK_ZOMBIE_USER_ACK {
    char m_cZombieCheckType{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cZombieCheckType)>>::put(w, m_cZombieCheckType);
    }

    static KERM_CHECK_ZOMBIE_USER_ACK read_from(Reader& r) {
        KERM_CHECK_ZOMBIE_USER_ACK v;
        v.m_cZombieCheckType = Codec<std::remove_cvref_t<decltype(v.m_cZombieCheckType)>>::get(r);
        return v;
    }
};

struct KERM_CHECK_ZOMBIE_USER_REQ {
    std::int64_t m_iRoomUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iRoomUID)>>::put(w, m_iRoomUID);
    }

    static KERM_CHECK_ZOMBIE_USER_REQ read_from(Reader& r) {
        KERM_CHECK_ZOMBIE_USER_REQ v;
        v.m_iRoomUID = Codec<std::remove_cvref_t<decltype(v.m_iRoomUID)>>::get(r);
        return v;
    }
};

struct KERM_COUPLE_MAKING_SUCCESS_NOT {
    std::int64_t   m_iRequestUnitUID{};  // UidType
    std::u16string m_wstrRequestUnitName{};  // std::wstring
    std::int64_t   m_iAcceptUnitUID{};  // UidType
    std::u16string m_wstrAcceptUnitName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iRequestUnitUID)>>::put(w, m_iRequestUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrRequestUnitName)>>::put(w, m_wstrRequestUnitName);
        Codec<std::remove_cvref_t<decltype(m_iAcceptUnitUID)>>::put(w, m_iAcceptUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrAcceptUnitName)>>::put(w, m_wstrAcceptUnitName);
    }

    static KERM_COUPLE_MAKING_SUCCESS_NOT read_from(Reader& r) {
        KERM_COUPLE_MAKING_SUCCESS_NOT v;
        v.m_iRequestUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iRequestUnitUID)>>::get(r);
        v.m_wstrRequestUnitName = Codec<std::remove_cvref_t<decltype(v.m_wstrRequestUnitName)>>::get(r);
        v.m_iAcceptUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iAcceptUnitUID)>>::get(r);
        v.m_wstrAcceptUnitName = Codec<std::remove_cvref_t<decltype(v.m_wstrAcceptUnitName)>>::get(r);
        return v;
    }
};

struct KERM_COUPLE_PROPOSE_RESULT_FAIL_NOT {
    std::int32_t   m_iOK{};  // int
    std::int64_t   m_iRequestUnitUID{};  // UidType
    std::u16string m_wstrRequestUnitName{};  // std::wstring
    std::int64_t   m_iAcceptUnitUID{};  // UidType
    std::u16string m_wstrAcceptUnitName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iRequestUnitUID)>>::put(w, m_iRequestUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrRequestUnitName)>>::put(w, m_wstrRequestUnitName);
        Codec<std::remove_cvref_t<decltype(m_iAcceptUnitUID)>>::put(w, m_iAcceptUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrAcceptUnitName)>>::put(w, m_wstrAcceptUnitName);
    }

    static KERM_COUPLE_PROPOSE_RESULT_FAIL_NOT read_from(Reader& r) {
        KERM_COUPLE_PROPOSE_RESULT_FAIL_NOT v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iRequestUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iRequestUnitUID)>>::get(r);
        v.m_wstrRequestUnitName = Codec<std::remove_cvref_t<decltype(v.m_wstrRequestUnitName)>>::get(r);
        v.m_iAcceptUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iAcceptUnitUID)>>::get(r);
        v.m_wstrAcceptUnitName = Codec<std::remove_cvref_t<decltype(v.m_wstrAcceptUnitName)>>::get(r);
        return v;
    }
};

struct KERM_COUPLE_PROPOSE_RESULT_SUCCESS_NOT {
    std::int32_t   m_iOK{};  // int
    std::int64_t   m_iRequestUnitUID{};  // UidType
    std::u16string m_wstrRequestUnitName{};  // std::wstring
    std::int64_t   m_iAcceptUnitUID{};  // UidType
    std::u16string m_wstrAcceptUnitName{};  // std::wstring
    std::int64_t   m_tDate{};  // __int64

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iRequestUnitUID)>>::put(w, m_iRequestUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrRequestUnitName)>>::put(w, m_wstrRequestUnitName);
        Codec<std::remove_cvref_t<decltype(m_iAcceptUnitUID)>>::put(w, m_iAcceptUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrAcceptUnitName)>>::put(w, m_wstrAcceptUnitName);
        Codec<std::remove_cvref_t<decltype(m_tDate)>>::put(w, m_tDate);
    }

    static KERM_COUPLE_PROPOSE_RESULT_SUCCESS_NOT read_from(Reader& r) {
        KERM_COUPLE_PROPOSE_RESULT_SUCCESS_NOT v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iRequestUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iRequestUnitUID)>>::get(r);
        v.m_wstrRequestUnitName = Codec<std::remove_cvref_t<decltype(v.m_wstrRequestUnitName)>>::get(r);
        v.m_iAcceptUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iAcceptUnitUID)>>::get(r);
        v.m_wstrAcceptUnitName = Codec<std::remove_cvref_t<decltype(v.m_wstrAcceptUnitName)>>::get(r);
        v.m_tDate = Codec<std::remove_cvref_t<decltype(v.m_tDate)>>::get(r);
        return v;
    }
};

struct KERM_DECREASE_ENDURANCE_DUNGEON_NOT {
    char         m_cPartyNumMember{};
    std::int32_t m_iEnduranceDamage{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cPartyNumMember)>>::put(w, m_cPartyNumMember);
        Codec<std::remove_cvref_t<decltype(m_iEnduranceDamage)>>::put(w, m_iEnduranceDamage);
    }

    static KERM_DECREASE_ENDURANCE_DUNGEON_NOT read_from(Reader& r) {
        KERM_DECREASE_ENDURANCE_DUNGEON_NOT v;
        v.m_cPartyNumMember = Codec<std::remove_cvref_t<decltype(v.m_cPartyNumMember)>>::get(r);
        v.m_iEnduranceDamage = Codec<std::remove_cvref_t<decltype(v.m_iEnduranceDamage)>>::get(r);
        return v;
    }
};

struct KERM_DECREASE_PET_SATIETY_NOT {
    char m_cTeamNumMember{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cTeamNumMember)>>::put(w, m_cTeamNumMember);
    }

    static KERM_DECREASE_PET_SATIETY_NOT read_from(Reader& r) {
        KERM_DECREASE_PET_SATIETY_NOT v;
        v.m_cTeamNumMember = Codec<std::remove_cvref_t<decltype(v.m_cTeamNumMember)>>::get(r);
        return v;
    }
};

struct KERM_DUMP_BATTLE_FIELD_NOT {
    std::u16string m_wstrNickName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
    }

    static KERM_DUMP_BATTLE_FIELD_NOT read_from(Reader& r) {
        KERM_DUMP_BATTLE_FIELD_NOT v;
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        return v;
    }
};

struct KERM_DUMP_ROOM_MONSTER_NOT {
    std::u16string m_wstrNickName{};  // std::wstring
    std::int32_t   m_iNpcUID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_iNpcUID)>>::put(w, m_iNpcUID);
    }

    static KERM_DUMP_ROOM_MONSTER_NOT read_from(Reader& r) {
        KERM_DUMP_ROOM_MONSTER_NOT v;
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_iNpcUID = Codec<std::remove_cvref_t<decltype(v.m_iNpcUID)>>::get(r);
        return v;
    }
};

struct KERM_DUNGEON_USER_STATISTICS_NOT {
    std::int32_t m_iDungeonID{};  // int
    char         m_cGameResult{};
    bool         m_bIsPartyPlay{};
    std::int32_t m_iGamePlayTime{};  // int
    std::int32_t m_iUsedResurrectionStone{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iDungeonID)>>::put(w, m_iDungeonID);
        Codec<std::remove_cvref_t<decltype(m_cGameResult)>>::put(w, m_cGameResult);
        Codec<std::remove_cvref_t<decltype(m_bIsPartyPlay)>>::put(w, m_bIsPartyPlay);
        Codec<std::remove_cvref_t<decltype(m_iGamePlayTime)>>::put(w, m_iGamePlayTime);
        Codec<std::remove_cvref_t<decltype(m_iUsedResurrectionStone)>>::put(w, m_iUsedResurrectionStone);
    }

    static KERM_DUNGEON_USER_STATISTICS_NOT read_from(Reader& r) {
        KERM_DUNGEON_USER_STATISTICS_NOT v;
        v.m_iDungeonID = Codec<std::remove_cvref_t<decltype(v.m_iDungeonID)>>::get(r);
        v.m_cGameResult = Codec<std::remove_cvref_t<decltype(v.m_cGameResult)>>::get(r);
        v.m_bIsPartyPlay = Codec<std::remove_cvref_t<decltype(v.m_bIsPartyPlay)>>::get(r);
        v.m_iGamePlayTime = Codec<std::remove_cvref_t<decltype(v.m_iGamePlayTime)>>::get(r);
        v.m_iUsedResurrectionStone = Codec<std::remove_cvref_t<decltype(v.m_iUsedResurrectionStone)>>::get(r);
        return v;
    }
};

struct KERM_END_GAME_NOT {
    std::int32_t m_iWinTeam{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iWinTeam)>>::put(w, m_iWinTeam);
    }

    static KERM_END_GAME_NOT read_from(Reader& r) {
        KERM_END_GAME_NOT v;
        v.m_iWinTeam = Codec<std::remove_cvref_t<decltype(v.m_iWinTeam)>>::get(r);
        return v;
    }
};

struct KERM_END_GAME_REGROUP_PARTY_NOT {
    std::int64_t m_iGSUID{};  // UidType
    std::int64_t m_iPartyUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iGSUID)>>::put(w, m_iGSUID);
        Codec<std::remove_cvref_t<decltype(m_iPartyUID)>>::put(w, m_iPartyUID);
    }

    static KERM_END_GAME_REGROUP_PARTY_NOT read_from(Reader& r) {
        KERM_END_GAME_REGROUP_PARTY_NOT v;
        v.m_iGSUID = Codec<std::remove_cvref_t<decltype(v.m_iGSUID)>>::get(r);
        v.m_iPartyUID = Codec<std::remove_cvref_t<decltype(v.m_iPartyUID)>>::get(r);
        return v;
    }
};

struct KERM_END_TC_GAME_ACK {
    std::int32_t m_iOK{};  // int
    std::int32_t m_iTCID{};  // int
    bool         m_bIsSuccess{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iTCID)>>::put(w, m_iTCID);
        Codec<std::remove_cvref_t<decltype(m_bIsSuccess)>>::put(w, m_bIsSuccess);
    }

    static KERM_END_TC_GAME_ACK read_from(Reader& r) {
        KERM_END_TC_GAME_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iTCID = Codec<std::remove_cvref_t<decltype(v.m_iTCID)>>::get(r);
        v.m_bIsSuccess = Codec<std::remove_cvref_t<decltype(v.m_bIsSuccess)>>::get(r);
        return v;
    }
};

struct KERM_FIELD_USER_LIST_MONITORING_MODE_REQ {
    std::uint32_t                        m_uiViewPage{};  // UINT
    std::map<std::int64_t, std::int32_t> m_mapUnit_ED{};  // std::map< UidType, int >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_uiViewPage)>>::put(w, m_uiViewPage);
        Codec<std::remove_cvref_t<decltype(m_mapUnit_ED)>>::put(w, m_mapUnit_ED);
    }

    static KERM_FIELD_USER_LIST_MONITORING_MODE_REQ read_from(Reader& r) {
        KERM_FIELD_USER_LIST_MONITORING_MODE_REQ v;
        v.m_uiViewPage = Codec<std::remove_cvref_t<decltype(v.m_uiViewPage)>>::get(r);
        v.m_mapUnit_ED = Codec<std::remove_cvref_t<decltype(v.m_mapUnit_ED)>>::get(r);
        return v;
    }
};

struct KERM_FIELD_USER_STATISTICS_NOT {
    std::int32_t m_iGamePlayTime{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iGamePlayTime)>>::put(w, m_iGamePlayTime);
    }

    static KERM_FIELD_USER_STATISTICS_NOT read_from(Reader& r) {
        KERM_FIELD_USER_STATISTICS_NOT v;
        v.m_iGamePlayTime = Codec<std::remove_cvref_t<decltype(v.m_iGamePlayTime)>>::get(r);
        return v;
    }
};

struct KERM_GAME_START_REQ {
    std::int16_t m_sWorldID{};  // short

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_sWorldID)>>::put(w, m_sWorldID);
    }

    static KERM_GAME_START_REQ read_from(Reader& r) {
        KERM_GAME_START_REQ v;
        v.m_sWorldID = Codec<std::remove_cvref_t<decltype(v.m_sWorldID)>>::get(r);
        return v;
    }
};

struct KERM_GET_ITEM_COMPLETE_NOT {
    std::int64_t                         m_iUnitUID{};  // UidType
    std::map<std::int32_t, std::int32_t> m_mapGetItem{};  // std::map< int, int >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_mapGetItem)>>::put(w, m_mapGetItem);
    }

    static KERM_GET_ITEM_COMPLETE_NOT read_from(Reader& r) {
        KERM_GET_ITEM_COMPLETE_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_mapGetItem = Codec<std::remove_cvref_t<decltype(v.m_mapGetItem)>>::get(r);
        return v;
    }
};

struct KERM_INCREASE_PARTY_FEVER_NOT {
    std::int64_t m_iPartyUID{};  // UidType
    std::int16_t m_sSubStageNpcCount{};  // short
    char         m_cRoomType{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iPartyUID)>>::put(w, m_iPartyUID);
        Codec<std::remove_cvref_t<decltype(m_sSubStageNpcCount)>>::put(w, m_sSubStageNpcCount);
        Codec<std::remove_cvref_t<decltype(m_cRoomType)>>::put(w, m_cRoomType);
    }

    static KERM_INCREASE_PARTY_FEVER_NOT read_from(Reader& r) {
        KERM_INCREASE_PARTY_FEVER_NOT v;
        v.m_iPartyUID = Codec<std::remove_cvref_t<decltype(v.m_iPartyUID)>>::get(r);
        v.m_sSubStageNpcCount = Codec<std::remove_cvref_t<decltype(v.m_sSubStageNpcCount)>>::get(r);
        v.m_cRoomType = Codec<std::remove_cvref_t<decltype(v.m_cRoomType)>>::get(r);
        return v;
    }
};

struct KERM_JOIN_FIELD_ACK {
    std::int32_t m_iOK{};  // int
    std::int64_t m_iFieldUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iFieldUID)>>::put(w, m_iFieldUID);
    }

    static KERM_JOIN_FIELD_ACK read_from(Reader& r) {
        KERM_JOIN_FIELD_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iFieldUID = Codec<std::remove_cvref_t<decltype(v.m_iFieldUID)>>::get(r);
        return v;
    }
};

struct KERM_JOIN_PERSONAL_SHOP_REQ {
    std::int64_t   m_iUserUID{};  // UidType
    std::int64_t   m_iUnitUID{};  // UidType
    std::u16string m_wstrNickName{};  // std::wstring
    bool           m_bIsSquare{};
    char           m_cJoinType{};
    std::int32_t   m_iUsedItemID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_bIsSquare)>>::put(w, m_bIsSquare);
        Codec<std::remove_cvref_t<decltype(m_cJoinType)>>::put(w, m_cJoinType);
        Codec<std::remove_cvref_t<decltype(m_iUsedItemID)>>::put(w, m_iUsedItemID);
    }

    static KERM_JOIN_PERSONAL_SHOP_REQ read_from(Reader& r) {
        KERM_JOIN_PERSONAL_SHOP_REQ v;
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_bIsSquare = Codec<std::remove_cvref_t<decltype(v.m_bIsSquare)>>::get(r);
        v.m_cJoinType = Codec<std::remove_cvref_t<decltype(v.m_cJoinType)>>::get(r);
        v.m_iUsedItemID = Codec<std::remove_cvref_t<decltype(v.m_iUsedItemID)>>::get(r);
        return v;
    }
};

struct KERM_KILL_ZOMBIE_USER_IN_BATTLE_FIELD_NOT {
    std::u16string m_wstrNickName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
    }

    static KERM_KILL_ZOMBIE_USER_IN_BATTLE_FIELD_NOT read_from(Reader& r) {
        KERM_KILL_ZOMBIE_USER_IN_BATTLE_FIELD_NOT v;
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        return v;
    }
};

struct KERM_LEAVE_FIELD_ACK {
    std::int32_t m_iOK{};  // int
    std::int32_t m_iReason{};  // int
    std::int32_t m_iMapID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iReason)>>::put(w, m_iReason);
        Codec<std::remove_cvref_t<decltype(m_iMapID)>>::put(w, m_iMapID);
    }

    static KERM_LEAVE_FIELD_ACK read_from(Reader& r) {
        KERM_LEAVE_FIELD_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iReason = Codec<std::remove_cvref_t<decltype(v.m_iReason)>>::get(r);
        v.m_iMapID = Codec<std::remove_cvref_t<decltype(v.m_iMapID)>>::get(r);
        return v;
    }
};

struct KERM_LEAVE_FIELD_REQ {
    std::int32_t m_iReason{};  // int
    std::int32_t m_iMapID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iReason)>>::put(w, m_iReason);
        Codec<std::remove_cvref_t<decltype(m_iMapID)>>::put(w, m_iMapID);
    }

    static KERM_LEAVE_FIELD_REQ read_from(Reader& r) {
        KERM_LEAVE_FIELD_REQ v;
        v.m_iReason = Codec<std::remove_cvref_t<decltype(v.m_iReason)>>::get(r);
        v.m_iMapID = Codec<std::remove_cvref_t<decltype(v.m_iMapID)>>::get(r);
        return v;
    }
};

struct KERM_LEAVE_PERSONAL_SHOP_REQ {
    std::int32_t m_iReason{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iReason)>>::put(w, m_iReason);
    }

    static KERM_LEAVE_PERSONAL_SHOP_REQ read_from(Reader& r) {
        KERM_LEAVE_PERSONAL_SHOP_REQ v;
        v.m_iReason = Codec<std::remove_cvref_t<decltype(v.m_iReason)>>::get(r);
        return v;
    }
};

struct KERM_LOAD_PSHOP_BOARD_INFO_NOT {
    std::int64_t m_iGSUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iGSUID)>>::put(w, m_iGSUID);
    }

    static KERM_LOAD_PSHOP_BOARD_INFO_NOT read_from(Reader& r) {
        KERM_LOAD_PSHOP_BOARD_INFO_NOT v;
        v.m_iGSUID = Codec<std::remove_cvref_t<decltype(v.m_iGSUID)>>::get(r);
        return v;
    }
};

struct KERM_MAX_PARTY_FEVER_NOT {
    std::int64_t m_iPartyUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iPartyUID)>>::put(w, m_iPartyUID);
    }

    static KERM_MAX_PARTY_FEVER_NOT read_from(Reader& r) {
        KERM_MAX_PARTY_FEVER_NOT v;
        v.m_iPartyUID = Codec<std::remove_cvref_t<decltype(v.m_iPartyUID)>>::get(r);
        return v;
    }
};

struct KERM_NPC_UNIT_DIE_ACK {
    std::int32_t m_iOK{};  // int
    std::int32_t m_iNPCID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iNPCID)>>::put(w, m_iNPCID);
    }

    static KERM_NPC_UNIT_DIE_ACK read_from(Reader& r) {
        KERM_NPC_UNIT_DIE_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iNPCID = Codec<std::remove_cvref_t<decltype(v.m_iNPCID)>>::get(r);
        return v;
    }
};

struct KERM_OPEN_PERSONAL_SHOP_ACK {
    std::int32_t   m_iOK{};  // int
    std::int64_t   m_iPersonalShopUID{};  // UidType
    char           m_cPersonalShopType{};
    std::int64_t   m_iGSUID{};  // UidType
    std::int64_t   m_iUserUID{};  // UidType
    std::int64_t   m_iUnitUID{};  // UidType
    std::u16string m_wstrAgencyOpenDate{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iPersonalShopUID)>>::put(w, m_iPersonalShopUID);
        Codec<std::remove_cvref_t<decltype(m_cPersonalShopType)>>::put(w, m_cPersonalShopType);
        Codec<std::remove_cvref_t<decltype(m_iGSUID)>>::put(w, m_iGSUID);
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrAgencyOpenDate)>>::put(w, m_wstrAgencyOpenDate);
    }

    static KERM_OPEN_PERSONAL_SHOP_ACK read_from(Reader& r) {
        KERM_OPEN_PERSONAL_SHOP_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iPersonalShopUID = Codec<std::remove_cvref_t<decltype(v.m_iPersonalShopUID)>>::get(r);
        v.m_cPersonalShopType = Codec<std::remove_cvref_t<decltype(v.m_cPersonalShopType)>>::get(r);
        v.m_iGSUID = Codec<std::remove_cvref_t<decltype(v.m_iGSUID)>>::get(r);
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_wstrAgencyOpenDate = Codec<std::remove_cvref_t<decltype(v.m_wstrAgencyOpenDate)>>::get(r);
        return v;
    }
};

struct KERM_OPEN_PERSONAL_SHOP_REQ {
    std::int64_t   m_iUserUID{};  // UidType
    std::int32_t   m_iED{};  // int
    std::u16string m_wstrNickName{};  // std::wstring
    char           m_cPersonalShopType{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
        Codec<std::remove_cvref_t<decltype(m_iED)>>::put(w, m_iED);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_cPersonalShopType)>>::put(w, m_cPersonalShopType);
    }

    static KERM_OPEN_PERSONAL_SHOP_REQ read_from(Reader& r) {
        KERM_OPEN_PERSONAL_SHOP_REQ v;
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        v.m_iED = Codec<std::remove_cvref_t<decltype(v.m_iED)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_cPersonalShopType = Codec<std::remove_cvref_t<decltype(v.m_cPersonalShopType)>>::get(r);
        return v;
    }
};

struct KERM_OPEN_PSHOP_AGENCY_REQ {
    std::int64_t   m_iUserUID{};  // UidType
    std::int32_t   m_iED{};  // int
    std::u16string m_wstrNickName{};  // std::wstring
    char           m_cPersonalShopType{};
    std::u16string m_wstrAgencyExpirationDate{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
        Codec<std::remove_cvref_t<decltype(m_iED)>>::put(w, m_iED);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_cPersonalShopType)>>::put(w, m_cPersonalShopType);
        Codec<std::remove_cvref_t<decltype(m_wstrAgencyExpirationDate)>>::put(w, m_wstrAgencyExpirationDate);
    }

    static KERM_OPEN_PSHOP_AGENCY_REQ read_from(Reader& r) {
        KERM_OPEN_PSHOP_AGENCY_REQ v;
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        v.m_iED = Codec<std::remove_cvref_t<decltype(v.m_iED)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_cPersonalShopType = Codec<std::remove_cvref_t<decltype(v.m_cPersonalShopType)>>::get(r);
        v.m_wstrAgencyExpirationDate = Codec<std::remove_cvref_t<decltype(v.m_wstrAgencyExpirationDate)>>::get(r);
        return v;
    }
};

struct KERM_OPEN_TRADE_ACK {
    std::int32_t m_iOK{};  // int
    std::int64_t m_iTradeUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iTradeUID)>>::put(w, m_iTradeUID);
    }

    static KERM_OPEN_TRADE_ACK read_from(Reader& r) {
        KERM_OPEN_TRADE_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iTradeUID = Codec<std::remove_cvref_t<decltype(v.m_iTradeUID)>>::get(r);
        return v;
    }
};

struct KERM_OPEN_TRADE_REQ {
    std::int64_t   m_iUserUID{};  // UidType
    std::u16string m_wstrSenderNickName{};  // std::wstring
    std::int64_t   m_iMemberUID{};  // UidType
    std::int64_t   m_iMemberGSUID{};  // UidType
    std::int32_t   m_iED{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
        Codec<std::remove_cvref_t<decltype(m_wstrSenderNickName)>>::put(w, m_wstrSenderNickName);
        Codec<std::remove_cvref_t<decltype(m_iMemberUID)>>::put(w, m_iMemberUID);
        Codec<std::remove_cvref_t<decltype(m_iMemberGSUID)>>::put(w, m_iMemberGSUID);
        Codec<std::remove_cvref_t<decltype(m_iED)>>::put(w, m_iED);
    }

    static KERM_OPEN_TRADE_REQ read_from(Reader& r) {
        KERM_OPEN_TRADE_REQ v;
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        v.m_wstrSenderNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrSenderNickName)>>::get(r);
        v.m_iMemberUID = Codec<std::remove_cvref_t<decltype(v.m_iMemberUID)>>::get(r);
        v.m_iMemberGSUID = Codec<std::remove_cvref_t<decltype(v.m_iMemberGSUID)>>::get(r);
        v.m_iED = Codec<std::remove_cvref_t<decltype(v.m_iED)>>::get(r);
        return v;
    }
};

struct KERM_PET_ACTION_NOT {
    std::int64_t m_iUnitUID{};  // UidType
    char         m_cActionType{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_cActionType)>>::put(w, m_cActionType);
    }

    static KERM_PET_ACTION_NOT read_from(Reader& r) {
        KERM_PET_ACTION_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_cActionType = Codec<std::remove_cvref_t<decltype(v.m_cActionType)>>::get(r);
        return v;
    }
};

struct KERM_PREPARE_FOR_DEFENCE_DUNGEON_REQ {
    KDungeonGameInfo m_kDefenceDungeonInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kDefenceDungeonInfo)>>::put(w, m_kDefenceDungeonInfo);
    }

    static KERM_PREPARE_FOR_DEFENCE_DUNGEON_REQ read_from(Reader& r) {
        KERM_PREPARE_FOR_DEFENCE_DUNGEON_REQ v;
        v.m_kDefenceDungeonInfo = Codec<std::remove_cvref_t<decltype(v.m_kDefenceDungeonInfo)>>::get(r);
        return v;
    }
};

struct KERM_PREPARE_PICK_UP_FROM_PSHOP_AGENCY_ACK {
    char                      m_cShopType{};
    std::vector<std::int64_t> m_vecPickUpItemList{};  // std::vector< UidType >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cShopType)>>::put(w, m_cShopType);
        Codec<std::remove_cvref_t<decltype(m_vecPickUpItemList)>>::put(w, m_vecPickUpItemList);
    }

    static KERM_PREPARE_PICK_UP_FROM_PSHOP_AGENCY_ACK read_from(Reader& r) {
        KERM_PREPARE_PICK_UP_FROM_PSHOP_AGENCY_ACK v;
        v.m_cShopType = Codec<std::remove_cvref_t<decltype(v.m_cShopType)>>::get(r);
        v.m_vecPickUpItemList = Codec<std::remove_cvref_t<decltype(v.m_vecPickUpItemList)>>::get(r);
        return v;
    }
};

struct KERM_PREPARE_PICK_UP_FROM_PSHOP_AGENCY_REQ {
    std::vector<std::int64_t>            m_vecPickUpItemList{};  // std::vector< UidType >
    std::map<std::int32_t, std::int32_t> m_mapPickUpItemInfo{};  // std::map< int, int >
    std::int32_t                         m_iTotalAddED{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecPickUpItemList)>>::put(w, m_vecPickUpItemList);
        Codec<std::remove_cvref_t<decltype(m_mapPickUpItemInfo)>>::put(w, m_mapPickUpItemInfo);
        Codec<std::remove_cvref_t<decltype(m_iTotalAddED)>>::put(w, m_iTotalAddED);
    }

    static KERM_PREPARE_PICK_UP_FROM_PSHOP_AGENCY_REQ read_from(Reader& r) {
        KERM_PREPARE_PICK_UP_FROM_PSHOP_AGENCY_REQ v;
        v.m_vecPickUpItemList = Codec<std::remove_cvref_t<decltype(v.m_vecPickUpItemList)>>::get(r);
        v.m_mapPickUpItemInfo = Codec<std::remove_cvref_t<decltype(v.m_mapPickUpItemInfo)>>::get(r);
        v.m_iTotalAddED = Codec<std::remove_cvref_t<decltype(v.m_iTotalAddED)>>::get(r);
        return v;
    }
};

struct KERM_PVP_USER_STATISTICS_NOT {
    char         m_cGameType{};
    char         m_cGameResult{};
    std::int16_t m_sWorldID{};  // short
    bool         m_bIntrude{};
    std::int32_t m_iGamePlayTime{};  // int
    std::int32_t m_iKillNum{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cGameType)>>::put(w, m_cGameType);
        Codec<std::remove_cvref_t<decltype(m_cGameResult)>>::put(w, m_cGameResult);
        Codec<std::remove_cvref_t<decltype(m_sWorldID)>>::put(w, m_sWorldID);
        Codec<std::remove_cvref_t<decltype(m_bIntrude)>>::put(w, m_bIntrude);
        Codec<std::remove_cvref_t<decltype(m_iGamePlayTime)>>::put(w, m_iGamePlayTime);
        Codec<std::remove_cvref_t<decltype(m_iKillNum)>>::put(w, m_iKillNum);
    }

    static KERM_PVP_USER_STATISTICS_NOT read_from(Reader& r) {
        KERM_PVP_USER_STATISTICS_NOT v;
        v.m_cGameType = Codec<std::remove_cvref_t<decltype(v.m_cGameType)>>::get(r);
        v.m_cGameResult = Codec<std::remove_cvref_t<decltype(v.m_cGameResult)>>::get(r);
        v.m_sWorldID = Codec<std::remove_cvref_t<decltype(v.m_sWorldID)>>::get(r);
        v.m_bIntrude = Codec<std::remove_cvref_t<decltype(v.m_bIntrude)>>::get(r);
        v.m_iGamePlayTime = Codec<std::remove_cvref_t<decltype(v.m_iGamePlayTime)>>::get(r);
        v.m_iKillNum = Codec<std::remove_cvref_t<decltype(v.m_iKillNum)>>::get(r);
        return v;
    }
};

struct KERM_REQUEST_TRADE_NOT {
    std::int64_t   m_iSenderUID{};  // UidType
    std::u16string m_wstrSenderNickName{};  // std::wstring
    std::int64_t   m_iReceiverUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iSenderUID)>>::put(w, m_iSenderUID);
        Codec<std::remove_cvref_t<decltype(m_wstrSenderNickName)>>::put(w, m_wstrSenderNickName);
        Codec<std::remove_cvref_t<decltype(m_iReceiverUID)>>::put(w, m_iReceiverUID);
    }

    static KERM_REQUEST_TRADE_NOT read_from(Reader& r) {
        KERM_REQUEST_TRADE_NOT v;
        v.m_iSenderUID = Codec<std::remove_cvref_t<decltype(v.m_iSenderUID)>>::get(r);
        v.m_wstrSenderNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrSenderNickName)>>::get(r);
        v.m_iReceiverUID = Codec<std::remove_cvref_t<decltype(v.m_iReceiverUID)>>::get(r);
        return v;
    }
};

struct KERM_REQUEST_TRADE_REPLY_NOT {
    std::int64_t m_iReceiverUID{};  // UidType
    std::int32_t m_iOK{};  // int
    std::int64_t m_iMemberUID{};  // UidType
    std::int64_t m_iMemberGSUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iReceiverUID)>>::put(w, m_iReceiverUID);
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iMemberUID)>>::put(w, m_iMemberUID);
        Codec<std::remove_cvref_t<decltype(m_iMemberGSUID)>>::put(w, m_iMemberGSUID);
    }

    static KERM_REQUEST_TRADE_REPLY_NOT read_from(Reader& r) {
        KERM_REQUEST_TRADE_REPLY_NOT v;
        v.m_iReceiverUID = Codec<std::remove_cvref_t<decltype(v.m_iReceiverUID)>>::get(r);
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iMemberUID = Codec<std::remove_cvref_t<decltype(v.m_iMemberUID)>>::get(r);
        v.m_iMemberGSUID = Codec<std::remove_cvref_t<decltype(v.m_iMemberGSUID)>>::get(r);
        return v;
    }
};

struct KERM_ROOM_COUNT_INFO_NOT {
    char m_cRoomType{};
    bool m_bIsOpen{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cRoomType)>>::put(w, m_cRoomType);
        Codec<std::remove_cvref_t<decltype(m_bIsOpen)>>::put(w, m_bIsOpen);
    }

    static KERM_ROOM_COUNT_INFO_NOT read_from(Reader& r) {
        KERM_ROOM_COUNT_INFO_NOT v;
        v.m_cRoomType = Codec<std::remove_cvref_t<decltype(v.m_cRoomType)>>::get(r);
        v.m_bIsOpen = Codec<std::remove_cvref_t<decltype(v.m_bIsOpen)>>::get(r);
        return v;
    }
};

struct KERM_SQUARE_UNIT_REFRESH_ED_NOT {
    std::int64_t m_iUnitUID{};  // UidType
    std::int32_t m_iED{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iED)>>::put(w, m_iED);
    }

    static KERM_SQUARE_UNIT_REFRESH_ED_NOT read_from(Reader& r) {
        KERM_SQUARE_UNIT_REFRESH_ED_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iED = Codec<std::remove_cvref_t<decltype(v.m_iED)>>::get(r);
        return v;
    }
};

struct KERM_START_WEDDING_NOT {
    std::int32_t                         m_iOK{};  // int
    std::int64_t                         m_iGroom{};  // UidType
    std::int64_t                         m_iBride{};  // UidType
    std::map<std::int32_t, std::int32_t> m_mapRewardItem{};  // std::map< int, int >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iGroom)>>::put(w, m_iGroom);
        Codec<std::remove_cvref_t<decltype(m_iBride)>>::put(w, m_iBride);
        Codec<std::remove_cvref_t<decltype(m_mapRewardItem)>>::put(w, m_mapRewardItem);
    }

    static KERM_START_WEDDING_NOT read_from(Reader& r) {
        KERM_START_WEDDING_NOT v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iGroom = Codec<std::remove_cvref_t<decltype(v.m_iGroom)>>::get(r);
        v.m_iBride = Codec<std::remove_cvref_t<decltype(v.m_iBride)>>::get(r);
        v.m_mapRewardItem = Codec<std::remove_cvref_t<decltype(v.m_mapRewardItem)>>::get(r);
        return v;
    }
};

struct KERM_TC_USER_STATISTICS_NOT {
    std::int32_t m_iTCID{};  // int
    char         m_cGameResult{};
    std::int32_t m_iGamePlayTime{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iTCID)>>::put(w, m_iTCID);
        Codec<std::remove_cvref_t<decltype(m_cGameResult)>>::put(w, m_cGameResult);
        Codec<std::remove_cvref_t<decltype(m_iGamePlayTime)>>::put(w, m_iGamePlayTime);
    }

    static KERM_TC_USER_STATISTICS_NOT read_from(Reader& r) {
        KERM_TC_USER_STATISTICS_NOT v;
        v.m_iTCID = Codec<std::remove_cvref_t<decltype(v.m_iTCID)>>::get(r);
        v.m_cGameResult = Codec<std::remove_cvref_t<decltype(v.m_cGameResult)>>::get(r);
        v.m_iGamePlayTime = Codec<std::remove_cvref_t<decltype(v.m_iGamePlayTime)>>::get(r);
        return v;
    }
};

struct KERM_TUTORIAL_DUNGEON_ROOM_UPDATE_NOT {
    std::vector<std::int64_t> m_vecStudentUnitUID{};  // std::vector<UidType>

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecStudentUnitUID)>>::put(w, m_vecStudentUnitUID);
    }

    static KERM_TUTORIAL_DUNGEON_ROOM_UPDATE_NOT read_from(Reader& r) {
        KERM_TUTORIAL_DUNGEON_ROOM_UPDATE_NOT v;
        v.m_vecStudentUnitUID = Codec<std::remove_cvref_t<decltype(v.m_vecStudentUnitUID)>>::get(r);
        return v;
    }
};

struct KERM_UPDATE_FIELD_INFO_NOT {
    std::int32_t m_iMapID{};  // int
    std::int64_t m_iFieldUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iMapID)>>::put(w, m_iMapID);
        Codec<std::remove_cvref_t<decltype(m_iFieldUID)>>::put(w, m_iFieldUID);
    }

    static KERM_UPDATE_FIELD_INFO_NOT read_from(Reader& r) {
        KERM_UPDATE_FIELD_INFO_NOT v;
        v.m_iMapID = Codec<std::remove_cvref_t<decltype(v.m_iMapID)>>::get(r);
        v.m_iFieldUID = Codec<std::remove_cvref_t<decltype(v.m_iFieldUID)>>::get(r);
        return v;
    }
};

struct KERM_UPDATE_FRIEND_NOT {
    std::set<std::int64_t> m_setFriendUIDList{};  // std::set< UidType >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_setFriendUIDList)>>::put(w, m_setFriendUIDList);
    }

    static KERM_UPDATE_FRIEND_NOT read_from(Reader& r) {
        KERM_UPDATE_FRIEND_NOT v;
        v.m_setFriendUIDList = Codec<std::remove_cvref_t<decltype(v.m_setFriendUIDList)>>::get(r);
        return v;
    }
};

struct KERM_UPDATE_PARTY_INFO_TO_BATTLE_FIELD_NOT {
    std::int64_t              m_iPartyUID{};  // UidType
    std::vector<std::int64_t> m_vecPartyUnitUIDList{};  // std::vector< UidType >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iPartyUID)>>::put(w, m_iPartyUID);
        Codec<std::remove_cvref_t<decltype(m_vecPartyUnitUIDList)>>::put(w, m_vecPartyUnitUIDList);
    }

    static KERM_UPDATE_PARTY_INFO_TO_BATTLE_FIELD_NOT read_from(Reader& r) {
        KERM_UPDATE_PARTY_INFO_TO_BATTLE_FIELD_NOT v;
        v.m_iPartyUID = Codec<std::remove_cvref_t<decltype(v.m_iPartyUID)>>::get(r);
        v.m_vecPartyUnitUIDList = Codec<std::remove_cvref_t<decltype(v.m_vecPartyUnitUIDList)>>::get(r);
        return v;
    }
};

struct KERM_UPDATE_PARTY_NOT {
    std::int64_t           m_iPartyUID{};  // UidType
    std::set<std::int64_t> m_setPartyUIDList{};  // std::set< UidType >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iPartyUID)>>::put(w, m_iPartyUID);
        Codec<std::remove_cvref_t<decltype(m_setPartyUIDList)>>::put(w, m_setPartyUIDList);
    }

    static KERM_UPDATE_PARTY_NOT read_from(Reader& r) {
        KERM_UPDATE_PARTY_NOT v;
        v.m_iPartyUID = Codec<std::remove_cvref_t<decltype(v.m_iPartyUID)>>::get(r);
        v.m_setPartyUIDList = Codec<std::remove_cvref_t<decltype(v.m_setPartyUIDList)>>::get(r);
        return v;
    }
};

struct KERM_UPDATE_PERIOD_PSHOP_AGENCY_NOT {
    std::u16string m_wstrAgencyExpirationDate{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrAgencyExpirationDate)>>::put(w, m_wstrAgencyExpirationDate);
    }

    static KERM_UPDATE_PERIOD_PSHOP_AGENCY_NOT read_from(Reader& r) {
        KERM_UPDATE_PERIOD_PSHOP_AGENCY_NOT v;
        v.m_wstrAgencyExpirationDate = Codec<std::remove_cvref_t<decltype(v.m_wstrAgencyExpirationDate)>>::get(r);
        return v;
    }
};

struct KERM_UPDATE_QUEST_INFO_NOT {
    std::vector<std::int32_t> m_vecNPC{};  // std::vector< int >
    bool                      m_isDungeon{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecNPC)>>::put(w, m_vecNPC);
        Codec<std::remove_cvref_t<decltype(m_isDungeon)>>::put(w, m_isDungeon);
    }

    static KERM_UPDATE_QUEST_INFO_NOT read_from(Reader& r) {
        KERM_UPDATE_QUEST_INFO_NOT v;
        v.m_vecNPC = Codec<std::remove_cvref_t<decltype(v.m_vecNPC)>>::get(r);
        v.m_isDungeon = Codec<std::remove_cvref_t<decltype(v.m_isDungeon)>>::get(r);
        return v;
    }
};

struct KERM_UPDATE_TUTORIAL_INFO_NOT {
    std::vector<std::int64_t> m_vecStudentUnitUID{};  // std::vector< UidType >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecStudentUnitUID)>>::put(w, m_vecStudentUnitUID);
    }

    static KERM_UPDATE_TUTORIAL_INFO_NOT read_from(Reader& r) {
        KERM_UPDATE_TUTORIAL_INFO_NOT v;
        v.m_vecStudentUnitUID = Codec<std::remove_cvref_t<decltype(v.m_vecStudentUnitUID)>>::get(r);
        return v;
    }
};

struct KERM_UPDATE_VISIT_CASH_SHOP_NOT {
    bool         m_bEnterCashShop{};
    std::int64_t m_iUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bEnterCashShop)>>::put(w, m_bEnterCashShop);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
    }

    static KERM_UPDATE_VISIT_CASH_SHOP_NOT read_from(Reader& r) {
        KERM_UPDATE_VISIT_CASH_SHOP_NOT v;
        v.m_bEnterCashShop = Codec<std::remove_cvref_t<decltype(v.m_bEnterCashShop)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        return v;
    }
};

struct KERM_USER_UNIT_DIE_ACK {
    std::int32_t m_iOK{};  // int
    float        m_fReBirthTime{};
    std::int16_t m_VP{};  // short
    std::int16_t m_EXP{};  // short
    std::uint8_t m_ucDieReason{};  // unsigned char

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_fReBirthTime)>>::put(w, m_fReBirthTime);
        Codec<std::remove_cvref_t<decltype(m_VP)>>::put(w, m_VP);
        Codec<std::remove_cvref_t<decltype(m_EXP)>>::put(w, m_EXP);
        Codec<std::remove_cvref_t<decltype(m_ucDieReason)>>::put(w, m_ucDieReason);
    }

    static KERM_USER_UNIT_DIE_ACK read_from(Reader& r) {
        KERM_USER_UNIT_DIE_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_fReBirthTime = Codec<std::remove_cvref_t<decltype(v.m_fReBirthTime)>>::get(r);
        v.m_VP = Codec<std::remove_cvref_t<decltype(v.m_VP)>>::get(r);
        v.m_EXP = Codec<std::remove_cvref_t<decltype(v.m_EXP)>>::get(r);
        v.m_ucDieReason = Codec<std::remove_cvref_t<decltype(v.m_ucDieReason)>>::get(r);
        return v;
    }
};

struct KERM_USER_UNIT_DIE_NOT {
    std::int32_t m_iGameType{};  // int
    std::int32_t m_KillerNPCUID{};  // int
    std::int64_t m_KillerUserUnitUID{};  // UidType
    std::int64_t m_MaxDamageKillerUserUnitUID{};  // UidType
    std::int64_t m_KilledUserUnitUID{};  // UidType
    bool         m_bOfficialMatch{};
    bool         m_bHeroNPC{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iGameType)>>::put(w, m_iGameType);
        Codec<std::remove_cvref_t<decltype(m_KillerNPCUID)>>::put(w, m_KillerNPCUID);
        Codec<std::remove_cvref_t<decltype(m_KillerUserUnitUID)>>::put(w, m_KillerUserUnitUID);
        Codec<std::remove_cvref_t<decltype(m_MaxDamageKillerUserUnitUID)>>::put(w, m_MaxDamageKillerUserUnitUID);
        Codec<std::remove_cvref_t<decltype(m_KilledUserUnitUID)>>::put(w, m_KilledUserUnitUID);
        Codec<std::remove_cvref_t<decltype(m_bOfficialMatch)>>::put(w, m_bOfficialMatch);
        Codec<std::remove_cvref_t<decltype(m_bHeroNPC)>>::put(w, m_bHeroNPC);
    }

    static KERM_USER_UNIT_DIE_NOT read_from(Reader& r) {
        KERM_USER_UNIT_DIE_NOT v;
        v.m_iGameType = Codec<std::remove_cvref_t<decltype(v.m_iGameType)>>::get(r);
        v.m_KillerNPCUID = Codec<std::remove_cvref_t<decltype(v.m_KillerNPCUID)>>::get(r);
        v.m_KillerUserUnitUID = Codec<std::remove_cvref_t<decltype(v.m_KillerUserUnitUID)>>::get(r);
        v.m_MaxDamageKillerUserUnitUID = Codec<std::remove_cvref_t<decltype(v.m_MaxDamageKillerUserUnitUID)>>::get(r);
        v.m_KilledUserUnitUID = Codec<std::remove_cvref_t<decltype(v.m_KilledUserUnitUID)>>::get(r);
        v.m_bOfficialMatch = Codec<std::remove_cvref_t<decltype(v.m_bOfficialMatch)>>::get(r);
        v.m_bHeroNPC = Codec<std::remove_cvref_t<decltype(v.m_bHeroNPC)>>::get(r);
        return v;
    }
};

struct KESR_SCRIPT_REFRESH_ORDER_NOT {
    std::int32_t m_iOrderType{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOrderType)>>::put(w, m_iOrderType);
    }

    static KESR_SCRIPT_REFRESH_ORDER_NOT read_from(Reader& r) {
        KESR_SCRIPT_REFRESH_ORDER_NOT v;
        v.m_iOrderType = Codec<std::remove_cvref_t<decltype(v.m_iOrderType)>>::get(r);
        return v;
    }
};

struct KETR_CHANGE_FIELD_USER_INFO_NOT {
    std::vector<std::int64_t> m_vecUID{};  // std::vector< UidType >
    std::int64_t              m_iUnitUID{};  // UidType
    char                      m_cUnitClass{};
    std::u16string            m_wstrGuildName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecUID)>>::put(w, m_vecUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_cUnitClass)>>::put(w, m_cUnitClass);
        Codec<std::remove_cvref_t<decltype(m_wstrGuildName)>>::put(w, m_wstrGuildName);
    }

    static KETR_CHANGE_FIELD_USER_INFO_NOT read_from(Reader& r) {
        KETR_CHANGE_FIELD_USER_INFO_NOT v;
        v.m_vecUID = Codec<std::remove_cvref_t<decltype(v.m_vecUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_cUnitClass = Codec<std::remove_cvref_t<decltype(v.m_cUnitClass)>>::get(r);
        v.m_wstrGuildName = Codec<std::remove_cvref_t<decltype(v.m_wstrGuildName)>>::get(r);
        return v;
    }
};

struct KETR_CHAT_NOT {
    std::vector<std::int64_t> m_vecUID{};  // std::vector< UidType >
    char                      m_cChatPacketType{};
    std::int64_t              m_SenderUnitUID{};  // UidType
    std::u16string            m_wstrSenderUnitNickName{};  // std::wstring
    std::u16string            m_wstrRecieverUnitNickName{};  // std::wstring
    std::int64_t              m_RecieverUnitUID{};  // UidType
    char                      m_cRoomChatType{};
    std::u16string            m_wstrMsg{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecUID)>>::put(w, m_vecUID);
        Codec<std::remove_cvref_t<decltype(m_cChatPacketType)>>::put(w, m_cChatPacketType);
        Codec<std::remove_cvref_t<decltype(m_SenderUnitUID)>>::put(w, m_SenderUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrSenderUnitNickName)>>::put(w, m_wstrSenderUnitNickName);
        Codec<std::remove_cvref_t<decltype(m_wstrRecieverUnitNickName)>>::put(w, m_wstrRecieverUnitNickName);
        Codec<std::remove_cvref_t<decltype(m_RecieverUnitUID)>>::put(w, m_RecieverUnitUID);
        Codec<std::remove_cvref_t<decltype(m_cRoomChatType)>>::put(w, m_cRoomChatType);
        Codec<std::remove_cvref_t<decltype(m_wstrMsg)>>::put(w, m_wstrMsg);
    }

    static KETR_CHAT_NOT read_from(Reader& r) {
        KETR_CHAT_NOT v;
        v.m_vecUID = Codec<std::remove_cvref_t<decltype(v.m_vecUID)>>::get(r);
        v.m_cChatPacketType = Codec<std::remove_cvref_t<decltype(v.m_cChatPacketType)>>::get(r);
        v.m_SenderUnitUID = Codec<std::remove_cvref_t<decltype(v.m_SenderUnitUID)>>::get(r);
        v.m_wstrSenderUnitNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrSenderUnitNickName)>>::get(r);
        v.m_wstrRecieverUnitNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrRecieverUnitNickName)>>::get(r);
        v.m_RecieverUnitUID = Codec<std::remove_cvref_t<decltype(v.m_RecieverUnitUID)>>::get(r);
        v.m_cRoomChatType = Codec<std::remove_cvref_t<decltype(v.m_cRoomChatType)>>::get(r);
        v.m_wstrMsg = Codec<std::remove_cvref_t<decltype(v.m_wstrMsg)>>::get(r);
        return v;
    }
};

struct KETR_COUPLE_MAKING_SUCCESS_NOT {
    std::vector<std::int64_t> m_vecUID{};  // std::vector< UidType >
    std::u16string            m_wstrRequestUnitName{};  // std::wstring
    std::u16string            m_wstrAcceptUnitName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecUID)>>::put(w, m_vecUID);
        Codec<std::remove_cvref_t<decltype(m_wstrRequestUnitName)>>::put(w, m_wstrRequestUnitName);
        Codec<std::remove_cvref_t<decltype(m_wstrAcceptUnitName)>>::put(w, m_wstrAcceptUnitName);
    }

    static KETR_COUPLE_MAKING_SUCCESS_NOT read_from(Reader& r) {
        KETR_COUPLE_MAKING_SUCCESS_NOT v;
        v.m_vecUID = Codec<std::remove_cvref_t<decltype(v.m_vecUID)>>::get(r);
        v.m_wstrRequestUnitName = Codec<std::remove_cvref_t<decltype(v.m_wstrRequestUnitName)>>::get(r);
        v.m_wstrAcceptUnitName = Codec<std::remove_cvref_t<decltype(v.m_wstrAcceptUnitName)>>::get(r);
        return v;
    }
};

struct KETR_PET_ACTION_NOT {
    std::vector<std::int64_t> m_vecUID{};  // std::vector< UidType >
    std::int64_t              m_iUnitUID{};  // UidType
    char                      m_cActionType{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecUID)>>::put(w, m_vecUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_cActionType)>>::put(w, m_cActionType);
    }

    static KETR_PET_ACTION_NOT read_from(Reader& r) {
        KETR_PET_ACTION_NOT v;
        v.m_vecUID = Codec<std::remove_cvref_t<decltype(v.m_vecUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_cActionType = Codec<std::remove_cvref_t<decltype(v.m_cActionType)>>::get(r);
        return v;
    }
};

struct KETR_RELATIONSHIP_EFFECT_NOT {
    std::vector<std::int64_t> m_vecUID{};  // std::vector< UidType >
    std::int64_t              m_iManUID{};  // UidType
    std::int64_t              m_iGirlUID{};  // UidType
    std::u16string            m_wstrLoveWord{};  // std::wstring
    std::int32_t              m_iEffectIndex{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecUID)>>::put(w, m_vecUID);
        Codec<std::remove_cvref_t<decltype(m_iManUID)>>::put(w, m_iManUID);
        Codec<std::remove_cvref_t<decltype(m_iGirlUID)>>::put(w, m_iGirlUID);
        Codec<std::remove_cvref_t<decltype(m_wstrLoveWord)>>::put(w, m_wstrLoveWord);
        Codec<std::remove_cvref_t<decltype(m_iEffectIndex)>>::put(w, m_iEffectIndex);
    }

    static KETR_RELATIONSHIP_EFFECT_NOT read_from(Reader& r) {
        KETR_RELATIONSHIP_EFFECT_NOT v;
        v.m_vecUID = Codec<std::remove_cvref_t<decltype(v.m_vecUID)>>::get(r);
        v.m_iManUID = Codec<std::remove_cvref_t<decltype(v.m_iManUID)>>::get(r);
        v.m_iGirlUID = Codec<std::remove_cvref_t<decltype(v.m_iGirlUID)>>::get(r);
        v.m_wstrLoveWord = Codec<std::remove_cvref_t<decltype(v.m_wstrLoveWord)>>::get(r);
        v.m_iEffectIndex = Codec<std::remove_cvref_t<decltype(v.m_iEffectIndex)>>::get(r);
        return v;
    }
};

struct KETR_SUMMON_RIDING_PET_NOT {
    std::vector<std::int64_t> m_vecUID{};  // std::vector<UidType>
    std::int64_t              m_iUnitUID{};  // UidType
    std::int64_t              m_iRidingPetUID{};  // UidType
    std::uint16_t             m_usRidingPetID{};  // USHORT

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecUID)>>::put(w, m_vecUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iRidingPetUID)>>::put(w, m_iRidingPetUID);
        Codec<std::remove_cvref_t<decltype(m_usRidingPetID)>>::put(w, m_usRidingPetID);
    }

    static KETR_SUMMON_RIDING_PET_NOT read_from(Reader& r) {
        KETR_SUMMON_RIDING_PET_NOT v;
        v.m_vecUID = Codec<std::remove_cvref_t<decltype(v.m_vecUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iRidingPetUID = Codec<std::remove_cvref_t<decltype(v.m_iRidingPetUID)>>::get(r);
        v.m_usRidingPetID = Codec<std::remove_cvref_t<decltype(v.m_usRidingPetID)>>::get(r);
        return v;
    }
};

struct KETR_UNSUMMON_RIDING_PET_NOT {
    std::vector<std::int64_t> m_vecUID{};  // std::vector<UidType>
    std::int64_t              m_iUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecUID)>>::put(w, m_vecUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
    }

    static KETR_UNSUMMON_RIDING_PET_NOT read_from(Reader& r) {
        KETR_UNSUMMON_RIDING_PET_NOT v;
        v.m_vecUID = Codec<std::remove_cvref_t<decltype(v.m_vecUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        return v;
    }
};

struct KETR_UPDATE_PARTY_NOT {
    std::vector<std::int64_t> m_vecUID{};  // std::vector< UidType >
    std::int64_t              m_iUnitUID{};  // UidType
    std::int64_t              m_iPartyUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecUID)>>::put(w, m_vecUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iPartyUID)>>::put(w, m_iPartyUID);
    }

    static KETR_UPDATE_PARTY_NOT read_from(Reader& r) {
        KETR_UPDATE_PARTY_NOT v;
        v.m_vecUID = Codec<std::remove_cvref_t<decltype(v.m_vecUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iPartyUID = Codec<std::remove_cvref_t<decltype(v.m_iPartyUID)>>::get(r);
        return v;
    }
};

struct KEXPData {
    std::int32_t m_iEXP{};  // int
    std::int32_t m_iPartyEXP{};  // int
    std::int32_t m_iSocketOptEXP{};  // int
    std::int32_t m_iEventBonusEXP{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iEXP)>>::put(w, m_iEXP);
        Codec<std::remove_cvref_t<decltype(m_iPartyEXP)>>::put(w, m_iPartyEXP);
        Codec<std::remove_cvref_t<decltype(m_iSocketOptEXP)>>::put(w, m_iSocketOptEXP);
        Codec<std::remove_cvref_t<decltype(m_iEventBonusEXP)>>::put(w, m_iEventBonusEXP);
    }

    static KEXPData read_from(Reader& r) {
        KEXPData v;
        v.m_iEXP = Codec<std::remove_cvref_t<decltype(v.m_iEXP)>>::get(r);
        v.m_iPartyEXP = Codec<std::remove_cvref_t<decltype(v.m_iPartyEXP)>>::get(r);
        v.m_iSocketOptEXP = Codec<std::remove_cvref_t<decltype(v.m_iSocketOptEXP)>>::get(r);
        v.m_iEventBonusEXP = Codec<std::remove_cvref_t<decltype(v.m_iEventBonusEXP)>>::get(r);
        return v;
    }
};

struct KEGS_NPC_UNIT_DIE_NOT {
    std::int32_t                     m_nDieNPCUID{};  // int
    std::int32_t                     m_iNPCID{};  // int
    char                             m_cAttUnitType{};
    std::int64_t                     m_uiAttUnit{};  // UidType
    std::map<std::int64_t, KEXPData> m_EXPList{};  // std::map< UidType, KEXPData >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_nDieNPCUID)>>::put(w, m_nDieNPCUID);
        Codec<std::remove_cvref_t<decltype(m_iNPCID)>>::put(w, m_iNPCID);
        Codec<std::remove_cvref_t<decltype(m_cAttUnitType)>>::put(w, m_cAttUnitType);
        Codec<std::remove_cvref_t<decltype(m_uiAttUnit)>>::put(w, m_uiAttUnit);
        Codec<std::remove_cvref_t<decltype(m_EXPList)>>::put(w, m_EXPList);
    }

    static KEGS_NPC_UNIT_DIE_NOT read_from(Reader& r) {
        KEGS_NPC_UNIT_DIE_NOT v;
        v.m_nDieNPCUID = Codec<std::remove_cvref_t<decltype(v.m_nDieNPCUID)>>::get(r);
        v.m_iNPCID = Codec<std::remove_cvref_t<decltype(v.m_iNPCID)>>::get(r);
        v.m_cAttUnitType = Codec<std::remove_cvref_t<decltype(v.m_cAttUnitType)>>::get(r);
        v.m_uiAttUnit = Codec<std::remove_cvref_t<decltype(v.m_uiAttUnit)>>::get(r);
        v.m_EXPList = Codec<std::remove_cvref_t<decltype(v.m_EXPList)>>::get(r);
        return v;
    }
};

struct KERM_NPC_UNIT_DIE_NOT {
    std::int32_t                     m_iDungeonID{};  // int
    char                             m_cDifficulty{};
    std::int32_t                     m_nDieNPCUID{};  // int
    std::int32_t                     m_iNPCID{};  // int
    char                             m_cAttUnitType{};
    std::int64_t                     m_uiAttUnit{};  // UidType
    std::map<std::int64_t, KEXPData> m_EXPList{};  // std::map< UidType, KEXPData >
    std::int16_t                     m_sSubStageExpNpcNum{};  // short
    char                             m_cUserCount{};
    bool                             m_bNoDrop{};
    bool                             m_bDecreaseEndurance{};
    bool                             m_bIsPvpNpc{};
    std::int32_t                     m_iGameType{};  // int
    char                             m_cDungeonMode{};
    bool                             m_bQuestComplete{};
    bool                             m_bHeroNPC{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iDungeonID)>>::put(w, m_iDungeonID);
        Codec<std::remove_cvref_t<decltype(m_cDifficulty)>>::put(w, m_cDifficulty);
        Codec<std::remove_cvref_t<decltype(m_nDieNPCUID)>>::put(w, m_nDieNPCUID);
        Codec<std::remove_cvref_t<decltype(m_iNPCID)>>::put(w, m_iNPCID);
        Codec<std::remove_cvref_t<decltype(m_cAttUnitType)>>::put(w, m_cAttUnitType);
        Codec<std::remove_cvref_t<decltype(m_uiAttUnit)>>::put(w, m_uiAttUnit);
        Codec<std::remove_cvref_t<decltype(m_EXPList)>>::put(w, m_EXPList);
        Codec<std::remove_cvref_t<decltype(m_sSubStageExpNpcNum)>>::put(w, m_sSubStageExpNpcNum);
        Codec<std::remove_cvref_t<decltype(m_cUserCount)>>::put(w, m_cUserCount);
        Codec<std::remove_cvref_t<decltype(m_bNoDrop)>>::put(w, m_bNoDrop);
        Codec<std::remove_cvref_t<decltype(m_bDecreaseEndurance)>>::put(w, m_bDecreaseEndurance);
        Codec<std::remove_cvref_t<decltype(m_bIsPvpNpc)>>::put(w, m_bIsPvpNpc);
        Codec<std::remove_cvref_t<decltype(m_iGameType)>>::put(w, m_iGameType);
        Codec<std::remove_cvref_t<decltype(m_cDungeonMode)>>::put(w, m_cDungeonMode);
        Codec<std::remove_cvref_t<decltype(m_bQuestComplete)>>::put(w, m_bQuestComplete);
        Codec<std::remove_cvref_t<decltype(m_bHeroNPC)>>::put(w, m_bHeroNPC);
    }

    static KERM_NPC_UNIT_DIE_NOT read_from(Reader& r) {
        KERM_NPC_UNIT_DIE_NOT v;
        v.m_iDungeonID = Codec<std::remove_cvref_t<decltype(v.m_iDungeonID)>>::get(r);
        v.m_cDifficulty = Codec<std::remove_cvref_t<decltype(v.m_cDifficulty)>>::get(r);
        v.m_nDieNPCUID = Codec<std::remove_cvref_t<decltype(v.m_nDieNPCUID)>>::get(r);
        v.m_iNPCID = Codec<std::remove_cvref_t<decltype(v.m_iNPCID)>>::get(r);
        v.m_cAttUnitType = Codec<std::remove_cvref_t<decltype(v.m_cAttUnitType)>>::get(r);
        v.m_uiAttUnit = Codec<std::remove_cvref_t<decltype(v.m_uiAttUnit)>>::get(r);
        v.m_EXPList = Codec<std::remove_cvref_t<decltype(v.m_EXPList)>>::get(r);
        v.m_sSubStageExpNpcNum = Codec<std::remove_cvref_t<decltype(v.m_sSubStageExpNpcNum)>>::get(r);
        v.m_cUserCount = Codec<std::remove_cvref_t<decltype(v.m_cUserCount)>>::get(r);
        v.m_bNoDrop = Codec<std::remove_cvref_t<decltype(v.m_bNoDrop)>>::get(r);
        v.m_bDecreaseEndurance = Codec<std::remove_cvref_t<decltype(v.m_bDecreaseEndurance)>>::get(r);
        v.m_bIsPvpNpc = Codec<std::remove_cvref_t<decltype(v.m_bIsPvpNpc)>>::get(r);
        v.m_iGameType = Codec<std::remove_cvref_t<decltype(v.m_iGameType)>>::get(r);
        v.m_cDungeonMode = Codec<std::remove_cvref_t<decltype(v.m_cDungeonMode)>>::get(r);
        v.m_bQuestComplete = Codec<std::remove_cvref_t<decltype(v.m_bQuestComplete)>>::get(r);
        v.m_bHeroNPC = Codec<std::remove_cvref_t<decltype(v.m_bHeroNPC)>>::get(r);
        return v;
    }
};

struct KE_HENIR_RANKING_LOG_NOT {
    std::int32_t m_iRankingType{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iRankingType)>>::put(w, m_iRankingType);
    }

    static KE_HENIR_RANKING_LOG_NOT read_from(Reader& r) {
        KE_HENIR_RANKING_LOG_NOT v;
        v.m_iRankingType = Codec<std::remove_cvref_t<decltype(v.m_iRankingType)>>::get(r);
        return v;
    }
};

struct KE_LOCAL_LOG_ABUSER_MORNITORING_NOT {
    std::vector<KAbuserEventLog> m_vecAbuserEventLog{};  // std::vector< KAbuserEventLog >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecAbuserEventLog)>>::put(w, m_vecAbuserEventLog);
    }

    static KE_LOCAL_LOG_ABUSER_MORNITORING_NOT read_from(Reader& r) {
        KE_LOCAL_LOG_ABUSER_MORNITORING_NOT v;
        v.m_vecAbuserEventLog = Codec<std::remove_cvref_t<decltype(v.m_vecAbuserEventLog)>>::get(r);
        return v;
    }
};

struct KE_LOCAL_LOG_BILLING_PACKET_NOT {
    char           m_cLogType{};
    std::int64_t   m_iUserUID{};  // UidType
    std::uint32_t  m_ulOrderNo{};  // unsigned long
    std::u16string m_wstrRegDate{};  // std::wstring
    std::uint16_t  m_usEventID{};  // u_short

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cLogType)>>::put(w, m_cLogType);
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
        Codec<std::remove_cvref_t<decltype(m_ulOrderNo)>>::put(w, m_ulOrderNo);
        Codec<std::remove_cvref_t<decltype(m_wstrRegDate)>>::put(w, m_wstrRegDate);
        Codec<std::remove_cvref_t<decltype(m_usEventID)>>::put(w, m_usEventID);
    }

    static KE_LOCAL_LOG_BILLING_PACKET_NOT read_from(Reader& r) {
        KE_LOCAL_LOG_BILLING_PACKET_NOT v;
        v.m_cLogType = Codec<std::remove_cvref_t<decltype(v.m_cLogType)>>::get(r);
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        v.m_ulOrderNo = Codec<std::remove_cvref_t<decltype(v.m_ulOrderNo)>>::get(r);
        v.m_wstrRegDate = Codec<std::remove_cvref_t<decltype(v.m_wstrRegDate)>>::get(r);
        v.m_usEventID = Codec<std::remove_cvref_t<decltype(v.m_usEventID)>>::get(r);
        return v;
    }
};

struct KE_LOCAL_LOG_DUNGEON_NOT {
    std::int32_t   m_iDungeonID{};  // int
    std::int64_t   m_nUnitUID{};  // UidType
    std::u16string m_wstrNickName{};  // std::wstring
    std::int32_t   m_iDifficultyLevel{};  // int
    std::int32_t   m_iIsChallenge{};  // int
    std::int32_t   m_iUnitClass{};  // int
    std::int32_t   m_iClear{};  // int
    std::int32_t   m_iStartNumMember{};  // int
    std::int32_t   m_iEndNumMember{};  // int
    std::uint8_t   m_ucLevel{};  // u_char
    std::int32_t   m_iPlayTime{};  // int
    std::int32_t   m_iEXP{};  // int
    std::int32_t   m_iED{};  // int
    char           m_cComboRank{};
    std::int32_t   m_iComboScore{};  // int
    char           m_cTechnicalRank{};
    std::int32_t   m_iTechnicalScore{};  // int
    char           m_cTimeRank{};
    char           m_cDamagedRank{};
    std::int32_t   m_iDamageNum{};  // int
    char           m_cTotalRank{};
    std::int32_t   m_iRessurectionStoneCount{};  // int
    std::int32_t   m_iPassedStageCount{};  // int
    std::int32_t   m_iPassedSubStageCount{};  // int
    bool           m_bIsWithPet{};
    std::u16string m_wstrRegDate{};  // std::wstring
    std::int16_t   m_sNpcDieCount{};  // short
    std::int64_t   m_iGivenDamage{};  // __int64
    std::int64_t   m_iAttackDamage{};  // __int64
    std::int16_t   m_sUsingSkillKind{};  // short
    std::int32_t   m_iTotalEXP{};  // int
    std::int32_t   m_iPetID{};  // int
    char           m_cPetEvoStep{};
    std::int32_t   m_iDashCount{};  // int
    std::int32_t   m_iBaseHP{};  // int
    std::int32_t   m_iEndHP{};  // int
    std::int32_t   m_iRecovHP{};  // int
    std::int32_t   m_iContinue{};  // int
    std::int32_t   m_iStartContinue{};  // int
    std::int32_t   m_iContinueUseStage{};  // int
    std::int32_t   m_iContinueUseSub{};  // int
    std::int32_t   m_iFrame{};  // int
    std::u16string m_wstrChannelIP{};  // std::wstring
    std::int64_t   m_iUserUID{};  // UidType
    char           m_cAutoPartyPlay{};
    std::int32_t   m_iAutoPartyWaitTime{};  // int
    std::int32_t   m_iLeaveReason{};  // int
    char           m_cRoomState{};
    bool           m_bSkillSlotType{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iDungeonID)>>::put(w, m_iDungeonID);
        Codec<std::remove_cvref_t<decltype(m_nUnitUID)>>::put(w, m_nUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_iDifficultyLevel)>>::put(w, m_iDifficultyLevel);
        Codec<std::remove_cvref_t<decltype(m_iIsChallenge)>>::put(w, m_iIsChallenge);
        Codec<std::remove_cvref_t<decltype(m_iUnitClass)>>::put(w, m_iUnitClass);
        Codec<std::remove_cvref_t<decltype(m_iClear)>>::put(w, m_iClear);
        Codec<std::remove_cvref_t<decltype(m_iStartNumMember)>>::put(w, m_iStartNumMember);
        Codec<std::remove_cvref_t<decltype(m_iEndNumMember)>>::put(w, m_iEndNumMember);
        Codec<std::remove_cvref_t<decltype(m_ucLevel)>>::put(w, m_ucLevel);
        Codec<std::remove_cvref_t<decltype(m_iPlayTime)>>::put(w, m_iPlayTime);
        Codec<std::remove_cvref_t<decltype(m_iEXP)>>::put(w, m_iEXP);
        Codec<std::remove_cvref_t<decltype(m_iED)>>::put(w, m_iED);
        Codec<std::remove_cvref_t<decltype(m_cComboRank)>>::put(w, m_cComboRank);
        Codec<std::remove_cvref_t<decltype(m_iComboScore)>>::put(w, m_iComboScore);
        Codec<std::remove_cvref_t<decltype(m_cTechnicalRank)>>::put(w, m_cTechnicalRank);
        Codec<std::remove_cvref_t<decltype(m_iTechnicalScore)>>::put(w, m_iTechnicalScore);
        Codec<std::remove_cvref_t<decltype(m_cTimeRank)>>::put(w, m_cTimeRank);
        Codec<std::remove_cvref_t<decltype(m_cDamagedRank)>>::put(w, m_cDamagedRank);
        Codec<std::remove_cvref_t<decltype(m_iDamageNum)>>::put(w, m_iDamageNum);
        Codec<std::remove_cvref_t<decltype(m_cTotalRank)>>::put(w, m_cTotalRank);
        Codec<std::remove_cvref_t<decltype(m_iRessurectionStoneCount)>>::put(w, m_iRessurectionStoneCount);
        Codec<std::remove_cvref_t<decltype(m_iPassedStageCount)>>::put(w, m_iPassedStageCount);
        Codec<std::remove_cvref_t<decltype(m_iPassedSubStageCount)>>::put(w, m_iPassedSubStageCount);
        Codec<std::remove_cvref_t<decltype(m_bIsWithPet)>>::put(w, m_bIsWithPet);
        Codec<std::remove_cvref_t<decltype(m_wstrRegDate)>>::put(w, m_wstrRegDate);
        Codec<std::remove_cvref_t<decltype(m_sNpcDieCount)>>::put(w, m_sNpcDieCount);
        Codec<std::remove_cvref_t<decltype(m_iGivenDamage)>>::put(w, m_iGivenDamage);
        Codec<std::remove_cvref_t<decltype(m_iAttackDamage)>>::put(w, m_iAttackDamage);
        Codec<std::remove_cvref_t<decltype(m_sUsingSkillKind)>>::put(w, m_sUsingSkillKind);
        Codec<std::remove_cvref_t<decltype(m_iTotalEXP)>>::put(w, m_iTotalEXP);
        Codec<std::remove_cvref_t<decltype(m_iPetID)>>::put(w, m_iPetID);
        Codec<std::remove_cvref_t<decltype(m_cPetEvoStep)>>::put(w, m_cPetEvoStep);
        Codec<std::remove_cvref_t<decltype(m_iDashCount)>>::put(w, m_iDashCount);
        Codec<std::remove_cvref_t<decltype(m_iBaseHP)>>::put(w, m_iBaseHP);
        Codec<std::remove_cvref_t<decltype(m_iEndHP)>>::put(w, m_iEndHP);
        Codec<std::remove_cvref_t<decltype(m_iRecovHP)>>::put(w, m_iRecovHP);
        Codec<std::remove_cvref_t<decltype(m_iContinue)>>::put(w, m_iContinue);
        Codec<std::remove_cvref_t<decltype(m_iStartContinue)>>::put(w, m_iStartContinue);
        Codec<std::remove_cvref_t<decltype(m_iContinueUseStage)>>::put(w, m_iContinueUseStage);
        Codec<std::remove_cvref_t<decltype(m_iContinueUseSub)>>::put(w, m_iContinueUseSub);
        Codec<std::remove_cvref_t<decltype(m_iFrame)>>::put(w, m_iFrame);
        Codec<std::remove_cvref_t<decltype(m_wstrChannelIP)>>::put(w, m_wstrChannelIP);
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
        Codec<std::remove_cvref_t<decltype(m_cAutoPartyPlay)>>::put(w, m_cAutoPartyPlay);
        Codec<std::remove_cvref_t<decltype(m_iAutoPartyWaitTime)>>::put(w, m_iAutoPartyWaitTime);
        Codec<std::remove_cvref_t<decltype(m_iLeaveReason)>>::put(w, m_iLeaveReason);
        Codec<std::remove_cvref_t<decltype(m_cRoomState)>>::put(w, m_cRoomState);
        Codec<std::remove_cvref_t<decltype(m_bSkillSlotType)>>::put(w, m_bSkillSlotType);
    }

    static KE_LOCAL_LOG_DUNGEON_NOT read_from(Reader& r) {
        KE_LOCAL_LOG_DUNGEON_NOT v;
        v.m_iDungeonID = Codec<std::remove_cvref_t<decltype(v.m_iDungeonID)>>::get(r);
        v.m_nUnitUID = Codec<std::remove_cvref_t<decltype(v.m_nUnitUID)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_iDifficultyLevel = Codec<std::remove_cvref_t<decltype(v.m_iDifficultyLevel)>>::get(r);
        v.m_iIsChallenge = Codec<std::remove_cvref_t<decltype(v.m_iIsChallenge)>>::get(r);
        v.m_iUnitClass = Codec<std::remove_cvref_t<decltype(v.m_iUnitClass)>>::get(r);
        v.m_iClear = Codec<std::remove_cvref_t<decltype(v.m_iClear)>>::get(r);
        v.m_iStartNumMember = Codec<std::remove_cvref_t<decltype(v.m_iStartNumMember)>>::get(r);
        v.m_iEndNumMember = Codec<std::remove_cvref_t<decltype(v.m_iEndNumMember)>>::get(r);
        v.m_ucLevel = Codec<std::remove_cvref_t<decltype(v.m_ucLevel)>>::get(r);
        v.m_iPlayTime = Codec<std::remove_cvref_t<decltype(v.m_iPlayTime)>>::get(r);
        v.m_iEXP = Codec<std::remove_cvref_t<decltype(v.m_iEXP)>>::get(r);
        v.m_iED = Codec<std::remove_cvref_t<decltype(v.m_iED)>>::get(r);
        v.m_cComboRank = Codec<std::remove_cvref_t<decltype(v.m_cComboRank)>>::get(r);
        v.m_iComboScore = Codec<std::remove_cvref_t<decltype(v.m_iComboScore)>>::get(r);
        v.m_cTechnicalRank = Codec<std::remove_cvref_t<decltype(v.m_cTechnicalRank)>>::get(r);
        v.m_iTechnicalScore = Codec<std::remove_cvref_t<decltype(v.m_iTechnicalScore)>>::get(r);
        v.m_cTimeRank = Codec<std::remove_cvref_t<decltype(v.m_cTimeRank)>>::get(r);
        v.m_cDamagedRank = Codec<std::remove_cvref_t<decltype(v.m_cDamagedRank)>>::get(r);
        v.m_iDamageNum = Codec<std::remove_cvref_t<decltype(v.m_iDamageNum)>>::get(r);
        v.m_cTotalRank = Codec<std::remove_cvref_t<decltype(v.m_cTotalRank)>>::get(r);
        v.m_iRessurectionStoneCount = Codec<std::remove_cvref_t<decltype(v.m_iRessurectionStoneCount)>>::get(r);
        v.m_iPassedStageCount = Codec<std::remove_cvref_t<decltype(v.m_iPassedStageCount)>>::get(r);
        v.m_iPassedSubStageCount = Codec<std::remove_cvref_t<decltype(v.m_iPassedSubStageCount)>>::get(r);
        v.m_bIsWithPet = Codec<std::remove_cvref_t<decltype(v.m_bIsWithPet)>>::get(r);
        v.m_wstrRegDate = Codec<std::remove_cvref_t<decltype(v.m_wstrRegDate)>>::get(r);
        v.m_sNpcDieCount = Codec<std::remove_cvref_t<decltype(v.m_sNpcDieCount)>>::get(r);
        v.m_iGivenDamage = Codec<std::remove_cvref_t<decltype(v.m_iGivenDamage)>>::get(r);
        v.m_iAttackDamage = Codec<std::remove_cvref_t<decltype(v.m_iAttackDamage)>>::get(r);
        v.m_sUsingSkillKind = Codec<std::remove_cvref_t<decltype(v.m_sUsingSkillKind)>>::get(r);
        v.m_iTotalEXP = Codec<std::remove_cvref_t<decltype(v.m_iTotalEXP)>>::get(r);
        v.m_iPetID = Codec<std::remove_cvref_t<decltype(v.m_iPetID)>>::get(r);
        v.m_cPetEvoStep = Codec<std::remove_cvref_t<decltype(v.m_cPetEvoStep)>>::get(r);
        v.m_iDashCount = Codec<std::remove_cvref_t<decltype(v.m_iDashCount)>>::get(r);
        v.m_iBaseHP = Codec<std::remove_cvref_t<decltype(v.m_iBaseHP)>>::get(r);
        v.m_iEndHP = Codec<std::remove_cvref_t<decltype(v.m_iEndHP)>>::get(r);
        v.m_iRecovHP = Codec<std::remove_cvref_t<decltype(v.m_iRecovHP)>>::get(r);
        v.m_iContinue = Codec<std::remove_cvref_t<decltype(v.m_iContinue)>>::get(r);
        v.m_iStartContinue = Codec<std::remove_cvref_t<decltype(v.m_iStartContinue)>>::get(r);
        v.m_iContinueUseStage = Codec<std::remove_cvref_t<decltype(v.m_iContinueUseStage)>>::get(r);
        v.m_iContinueUseSub = Codec<std::remove_cvref_t<decltype(v.m_iContinueUseSub)>>::get(r);
        v.m_iFrame = Codec<std::remove_cvref_t<decltype(v.m_iFrame)>>::get(r);
        v.m_wstrChannelIP = Codec<std::remove_cvref_t<decltype(v.m_wstrChannelIP)>>::get(r);
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        v.m_cAutoPartyPlay = Codec<std::remove_cvref_t<decltype(v.m_cAutoPartyPlay)>>::get(r);
        v.m_iAutoPartyWaitTime = Codec<std::remove_cvref_t<decltype(v.m_iAutoPartyWaitTime)>>::get(r);
        v.m_iLeaveReason = Codec<std::remove_cvref_t<decltype(v.m_iLeaveReason)>>::get(r);
        v.m_cRoomState = Codec<std::remove_cvref_t<decltype(v.m_cRoomState)>>::get(r);
        v.m_bSkillSlotType = Codec<std::remove_cvref_t<decltype(v.m_bSkillSlotType)>>::get(r);
        return v;
    }
};

struct KE_LOCAL_LOG_DUNGEON_ROOM_NOT {
    std::int32_t   m_iDungeonID{};  // int
    std::int32_t   m_iDifficultyLevel{};  // int
    std::int32_t   m_iIsChallenge{};  // int
    std::int32_t   m_iWin{};  // int
    std::int32_t   m_iLose{};  // int
    std::int32_t   m_iStartNumMember{};  // int
    std::int32_t   m_iEndNumMember{};  // int
    std::int32_t   m_iTotalPlayTime{};  // int
    std::u16string m_wstrEndGameTime{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iDungeonID)>>::put(w, m_iDungeonID);
        Codec<std::remove_cvref_t<decltype(m_iDifficultyLevel)>>::put(w, m_iDifficultyLevel);
        Codec<std::remove_cvref_t<decltype(m_iIsChallenge)>>::put(w, m_iIsChallenge);
        Codec<std::remove_cvref_t<decltype(m_iWin)>>::put(w, m_iWin);
        Codec<std::remove_cvref_t<decltype(m_iLose)>>::put(w, m_iLose);
        Codec<std::remove_cvref_t<decltype(m_iStartNumMember)>>::put(w, m_iStartNumMember);
        Codec<std::remove_cvref_t<decltype(m_iEndNumMember)>>::put(w, m_iEndNumMember);
        Codec<std::remove_cvref_t<decltype(m_iTotalPlayTime)>>::put(w, m_iTotalPlayTime);
        Codec<std::remove_cvref_t<decltype(m_wstrEndGameTime)>>::put(w, m_wstrEndGameTime);
    }

    static KE_LOCAL_LOG_DUNGEON_ROOM_NOT read_from(Reader& r) {
        KE_LOCAL_LOG_DUNGEON_ROOM_NOT v;
        v.m_iDungeonID = Codec<std::remove_cvref_t<decltype(v.m_iDungeonID)>>::get(r);
        v.m_iDifficultyLevel = Codec<std::remove_cvref_t<decltype(v.m_iDifficultyLevel)>>::get(r);
        v.m_iIsChallenge = Codec<std::remove_cvref_t<decltype(v.m_iIsChallenge)>>::get(r);
        v.m_iWin = Codec<std::remove_cvref_t<decltype(v.m_iWin)>>::get(r);
        v.m_iLose = Codec<std::remove_cvref_t<decltype(v.m_iLose)>>::get(r);
        v.m_iStartNumMember = Codec<std::remove_cvref_t<decltype(v.m_iStartNumMember)>>::get(r);
        v.m_iEndNumMember = Codec<std::remove_cvref_t<decltype(v.m_iEndNumMember)>>::get(r);
        v.m_iTotalPlayTime = Codec<std::remove_cvref_t<decltype(v.m_iTotalPlayTime)>>::get(r);
        v.m_wstrEndGameTime = Codec<std::remove_cvref_t<decltype(v.m_wstrEndGameTime)>>::get(r);
        return v;
    }
};

struct KE_LOCAL_LOG_PET_SUMMON_NOT {
    std::u16string m_wstrNickName{};  // std::wstring
    std::uint8_t   m_ucLevel{};  // u_char
    std::int64_t   m_iPetUID{};  // UidType
    std::int32_t   m_iPetID{};  // int
    char           m_cEvolutionStep{};
    std::int32_t   m_iIntimacy{};  // int
    std::int16_t   m_sExtroversion{};  // short
    std::int16_t   m_sEmotion{};  // short
    std::u16string m_wstrRegDate{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_ucLevel)>>::put(w, m_ucLevel);
        Codec<std::remove_cvref_t<decltype(m_iPetUID)>>::put(w, m_iPetUID);
        Codec<std::remove_cvref_t<decltype(m_iPetID)>>::put(w, m_iPetID);
        Codec<std::remove_cvref_t<decltype(m_cEvolutionStep)>>::put(w, m_cEvolutionStep);
        Codec<std::remove_cvref_t<decltype(m_iIntimacy)>>::put(w, m_iIntimacy);
        Codec<std::remove_cvref_t<decltype(m_sExtroversion)>>::put(w, m_sExtroversion);
        Codec<std::remove_cvref_t<decltype(m_sEmotion)>>::put(w, m_sEmotion);
        Codec<std::remove_cvref_t<decltype(m_wstrRegDate)>>::put(w, m_wstrRegDate);
    }

    static KE_LOCAL_LOG_PET_SUMMON_NOT read_from(Reader& r) {
        KE_LOCAL_LOG_PET_SUMMON_NOT v;
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_ucLevel = Codec<std::remove_cvref_t<decltype(v.m_ucLevel)>>::get(r);
        v.m_iPetUID = Codec<std::remove_cvref_t<decltype(v.m_iPetUID)>>::get(r);
        v.m_iPetID = Codec<std::remove_cvref_t<decltype(v.m_iPetID)>>::get(r);
        v.m_cEvolutionStep = Codec<std::remove_cvref_t<decltype(v.m_cEvolutionStep)>>::get(r);
        v.m_iIntimacy = Codec<std::remove_cvref_t<decltype(v.m_iIntimacy)>>::get(r);
        v.m_sExtroversion = Codec<std::remove_cvref_t<decltype(v.m_sExtroversion)>>::get(r);
        v.m_sEmotion = Codec<std::remove_cvref_t<decltype(v.m_sEmotion)>>::get(r);
        v.m_wstrRegDate = Codec<std::remove_cvref_t<decltype(v.m_wstrRegDate)>>::get(r);
        return v;
    }
};

struct KE_LOCAL_LOG_PVP_NOT {
    std::int32_t   m_iPVPChannelClass{};  // int
    std::int64_t   m_iOwnerUserUID{};  // UidType
    std::u16string m_wstrIP{};  // std::wstring
    std::int64_t   m_nUnitUID{};  // UidType
    std::u16string m_wstrNickName{};  // std::wstring
    std::int32_t   m_iUnitClass{};  // int
    std::int64_t   m_iRoomUID{};  // UidType
    std::int32_t   m_iPlayMode{};  // int
    bool           m_bIsItemMode{};
    bool           m_bPublic{};
    std::int32_t   m_iNumMember{};  // int
    float          m_fPlayTimeLimit{};
    std::u16string m_wstrEndGameTime{};  // std::wstring
    std::int32_t   m_iPlayTime{};  // int
    std::int32_t   m_iVSPoint{};  // int
    std::int32_t   m_iEXP{};  // int
    std::int32_t   m_iNumMDKill{};  // int
    std::int32_t   m_iNumDie{};  // int
    std::int32_t   m_iResult{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iPVPChannelClass)>>::put(w, m_iPVPChannelClass);
        Codec<std::remove_cvref_t<decltype(m_iOwnerUserUID)>>::put(w, m_iOwnerUserUID);
        Codec<std::remove_cvref_t<decltype(m_wstrIP)>>::put(w, m_wstrIP);
        Codec<std::remove_cvref_t<decltype(m_nUnitUID)>>::put(w, m_nUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_iUnitClass)>>::put(w, m_iUnitClass);
        Codec<std::remove_cvref_t<decltype(m_iRoomUID)>>::put(w, m_iRoomUID);
        Codec<std::remove_cvref_t<decltype(m_iPlayMode)>>::put(w, m_iPlayMode);
        Codec<std::remove_cvref_t<decltype(m_bIsItemMode)>>::put(w, m_bIsItemMode);
        Codec<std::remove_cvref_t<decltype(m_bPublic)>>::put(w, m_bPublic);
        Codec<std::remove_cvref_t<decltype(m_iNumMember)>>::put(w, m_iNumMember);
        Codec<std::remove_cvref_t<decltype(m_fPlayTimeLimit)>>::put(w, m_fPlayTimeLimit);
        Codec<std::remove_cvref_t<decltype(m_wstrEndGameTime)>>::put(w, m_wstrEndGameTime);
        Codec<std::remove_cvref_t<decltype(m_iPlayTime)>>::put(w, m_iPlayTime);
        Codec<std::remove_cvref_t<decltype(m_iVSPoint)>>::put(w, m_iVSPoint);
        Codec<std::remove_cvref_t<decltype(m_iEXP)>>::put(w, m_iEXP);
        Codec<std::remove_cvref_t<decltype(m_iNumMDKill)>>::put(w, m_iNumMDKill);
        Codec<std::remove_cvref_t<decltype(m_iNumDie)>>::put(w, m_iNumDie);
        Codec<std::remove_cvref_t<decltype(m_iResult)>>::put(w, m_iResult);
    }

    static KE_LOCAL_LOG_PVP_NOT read_from(Reader& r) {
        KE_LOCAL_LOG_PVP_NOT v;
        v.m_iPVPChannelClass = Codec<std::remove_cvref_t<decltype(v.m_iPVPChannelClass)>>::get(r);
        v.m_iOwnerUserUID = Codec<std::remove_cvref_t<decltype(v.m_iOwnerUserUID)>>::get(r);
        v.m_wstrIP = Codec<std::remove_cvref_t<decltype(v.m_wstrIP)>>::get(r);
        v.m_nUnitUID = Codec<std::remove_cvref_t<decltype(v.m_nUnitUID)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_iUnitClass = Codec<std::remove_cvref_t<decltype(v.m_iUnitClass)>>::get(r);
        v.m_iRoomUID = Codec<std::remove_cvref_t<decltype(v.m_iRoomUID)>>::get(r);
        v.m_iPlayMode = Codec<std::remove_cvref_t<decltype(v.m_iPlayMode)>>::get(r);
        v.m_bIsItemMode = Codec<std::remove_cvref_t<decltype(v.m_bIsItemMode)>>::get(r);
        v.m_bPublic = Codec<std::remove_cvref_t<decltype(v.m_bPublic)>>::get(r);
        v.m_iNumMember = Codec<std::remove_cvref_t<decltype(v.m_iNumMember)>>::get(r);
        v.m_fPlayTimeLimit = Codec<std::remove_cvref_t<decltype(v.m_fPlayTimeLimit)>>::get(r);
        v.m_wstrEndGameTime = Codec<std::remove_cvref_t<decltype(v.m_wstrEndGameTime)>>::get(r);
        v.m_iPlayTime = Codec<std::remove_cvref_t<decltype(v.m_iPlayTime)>>::get(r);
        v.m_iVSPoint = Codec<std::remove_cvref_t<decltype(v.m_iVSPoint)>>::get(r);
        v.m_iEXP = Codec<std::remove_cvref_t<decltype(v.m_iEXP)>>::get(r);
        v.m_iNumMDKill = Codec<std::remove_cvref_t<decltype(v.m_iNumMDKill)>>::get(r);
        v.m_iNumDie = Codec<std::remove_cvref_t<decltype(v.m_iNumDie)>>::get(r);
        v.m_iResult = Codec<std::remove_cvref_t<decltype(v.m_iResult)>>::get(r);
        return v;
    }
};

struct KE_LOCAL_LOG_PVP_ROOM_NOT {
    std::int32_t   m_iPlayMode{};  // int
    bool           m_bIsItemMode{};
    std::int16_t   m_sWorldID{};  // short
    std::int32_t   m_iNumMember{};  // int
    std::int32_t   m_iPlayTime{};  // int
    std::u16string m_wstrEndGameTime{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iPlayMode)>>::put(w, m_iPlayMode);
        Codec<std::remove_cvref_t<decltype(m_bIsItemMode)>>::put(w, m_bIsItemMode);
        Codec<std::remove_cvref_t<decltype(m_sWorldID)>>::put(w, m_sWorldID);
        Codec<std::remove_cvref_t<decltype(m_iNumMember)>>::put(w, m_iNumMember);
        Codec<std::remove_cvref_t<decltype(m_iPlayTime)>>::put(w, m_iPlayTime);
        Codec<std::remove_cvref_t<decltype(m_wstrEndGameTime)>>::put(w, m_wstrEndGameTime);
    }

    static KE_LOCAL_LOG_PVP_ROOM_NOT read_from(Reader& r) {
        KE_LOCAL_LOG_PVP_ROOM_NOT v;
        v.m_iPlayMode = Codec<std::remove_cvref_t<decltype(v.m_iPlayMode)>>::get(r);
        v.m_bIsItemMode = Codec<std::remove_cvref_t<decltype(v.m_bIsItemMode)>>::get(r);
        v.m_sWorldID = Codec<std::remove_cvref_t<decltype(v.m_sWorldID)>>::get(r);
        v.m_iNumMember = Codec<std::remove_cvref_t<decltype(v.m_iNumMember)>>::get(r);
        v.m_iPlayTime = Codec<std::remove_cvref_t<decltype(v.m_iPlayTime)>>::get(r);
        v.m_wstrEndGameTime = Codec<std::remove_cvref_t<decltype(v.m_wstrEndGameTime)>>::get(r);
        return v;
    }
};

struct KE_LOCAL_LOG_SERVER_DISCONNECT_NOT {
    char           m_cLogType{};
    std::u16string m_wstrSourceName{};  // std::wstring
    std::u16string m_wstrDestinationName{};  // std::wstring
    std::u16string m_wstrDestinationIP{};  // std::wstring
    std::u16string m_wstrReason{};  // std::wstring
    std::u16string m_wstrRegDate{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cLogType)>>::put(w, m_cLogType);
        Codec<std::remove_cvref_t<decltype(m_wstrSourceName)>>::put(w, m_wstrSourceName);
        Codec<std::remove_cvref_t<decltype(m_wstrDestinationName)>>::put(w, m_wstrDestinationName);
        Codec<std::remove_cvref_t<decltype(m_wstrDestinationIP)>>::put(w, m_wstrDestinationIP);
        Codec<std::remove_cvref_t<decltype(m_wstrReason)>>::put(w, m_wstrReason);
        Codec<std::remove_cvref_t<decltype(m_wstrRegDate)>>::put(w, m_wstrRegDate);
    }

    static KE_LOCAL_LOG_SERVER_DISCONNECT_NOT read_from(Reader& r) {
        KE_LOCAL_LOG_SERVER_DISCONNECT_NOT v;
        v.m_cLogType = Codec<std::remove_cvref_t<decltype(v.m_cLogType)>>::get(r);
        v.m_wstrSourceName = Codec<std::remove_cvref_t<decltype(v.m_wstrSourceName)>>::get(r);
        v.m_wstrDestinationName = Codec<std::remove_cvref_t<decltype(v.m_wstrDestinationName)>>::get(r);
        v.m_wstrDestinationIP = Codec<std::remove_cvref_t<decltype(v.m_wstrDestinationIP)>>::get(r);
        v.m_wstrReason = Codec<std::remove_cvref_t<decltype(v.m_wstrReason)>>::get(r);
        v.m_wstrRegDate = Codec<std::remove_cvref_t<decltype(v.m_wstrRegDate)>>::get(r);
        return v;
    }
};

