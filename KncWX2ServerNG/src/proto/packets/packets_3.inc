// ---------------------------------------------------------------------------
// packets_3.inc - GENERATED FILE, DO NOT EDIT BY HAND.
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

struct KEGS_NEW_USER_JOIN_REQ {
    std::u16string m_wstrID{};  // std::wstring
    std::u16string m_wstrPassword{};  // std::wstring
    std::u16string m_wstrName{};  // std::wstring
    std::uint8_t   m_byteGuestUser{};  // byte

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrID)>>::put(w, m_wstrID);
        Codec<std::remove_cvref_t<decltype(m_wstrPassword)>>::put(w, m_wstrPassword);
        Codec<std::remove_cvref_t<decltype(m_wstrName)>>::put(w, m_wstrName);
        Codec<std::remove_cvref_t<decltype(m_byteGuestUser)>>::put(w, m_byteGuestUser);
    }

    static KEGS_NEW_USER_JOIN_REQ read_from(Reader& r) {
        KEGS_NEW_USER_JOIN_REQ v;
        v.m_wstrID = Codec<std::remove_cvref_t<decltype(v.m_wstrID)>>::get(r);
        v.m_wstrPassword = Codec<std::remove_cvref_t<decltype(v.m_wstrPassword)>>::get(r);
        v.m_wstrName = Codec<std::remove_cvref_t<decltype(v.m_wstrName)>>::get(r);
        v.m_byteGuestUser = Codec<std::remove_cvref_t<decltype(v.m_byteGuestUser)>>::get(r);
        return v;
    }
};

struct KEGS_NOTIFY_MSG_REQ {
    char           m_cNotifyType{};
    char           m_Count{};
    std::u16string m_wstrMSG{};  // std::wstring
    std::int32_t   m_iRandomItem{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cNotifyType)>>::put(w, m_cNotifyType);
        Codec<std::remove_cvref_t<decltype(m_Count)>>::put(w, m_Count);
        Codec<std::remove_cvref_t<decltype(m_wstrMSG)>>::put(w, m_wstrMSG);
        Codec<std::remove_cvref_t<decltype(m_iRandomItem)>>::put(w, m_iRandomItem);
    }

    static KEGS_NOTIFY_MSG_REQ read_from(Reader& r) {
        KEGS_NOTIFY_MSG_REQ v;
        v.m_cNotifyType = Codec<std::remove_cvref_t<decltype(v.m_cNotifyType)>>::get(r);
        v.m_Count = Codec<std::remove_cvref_t<decltype(v.m_Count)>>::get(r);
        v.m_wstrMSG = Codec<std::remove_cvref_t<decltype(v.m_wstrMSG)>>::get(r);
        v.m_iRandomItem = Codec<std::remove_cvref_t<decltype(v.m_iRandomItem)>>::get(r);
        return v;
    }
};

struct KEGS_NPC_UNIT_DIE_ACK {
    std::int32_t m_iOK{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
    }

    static KEGS_NPC_UNIT_DIE_ACK read_from(Reader& r) {
        KEGS_NPC_UNIT_DIE_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        return v;
    }
};

struct KEGS_OPEN_RANDOM_ITEM_REQ {
    std::int64_t m_iItemUID{};  // UidType
    bool         m_bDelayedOK{};
    bool         m_bNotifyGetItem{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_bDelayedOK)>>::put(w, m_bDelayedOK);
        Codec<std::remove_cvref_t<decltype(m_bNotifyGetItem)>>::put(w, m_bNotifyGetItem);
    }

    static KEGS_OPEN_RANDOM_ITEM_REQ read_from(Reader& r) {
        KEGS_OPEN_RANDOM_ITEM_REQ v;
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_bDelayedOK = Codec<std::remove_cvref_t<decltype(v.m_bDelayedOK)>>::get(r);
        v.m_bNotifyGetItem = Codec<std::remove_cvref_t<decltype(v.m_bNotifyGetItem)>>::get(r);
        return v;
    }
};

struct KEGS_PARTY_CHANGE_DUNGEON_REQ {
    std::int32_t m_iDungeonID{};  // int
    char         m_DifficultyLevel{};
    char         m_cDungeonMode{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iDungeonID)>>::put(w, m_iDungeonID);
        Codec<std::remove_cvref_t<decltype(m_DifficultyLevel)>>::put(w, m_DifficultyLevel);
        Codec<std::remove_cvref_t<decltype(m_cDungeonMode)>>::put(w, m_cDungeonMode);
    }

    static KEGS_PARTY_CHANGE_DUNGEON_REQ read_from(Reader& r) {
        KEGS_PARTY_CHANGE_DUNGEON_REQ v;
        v.m_iDungeonID = Codec<std::remove_cvref_t<decltype(v.m_iDungeonID)>>::get(r);
        v.m_DifficultyLevel = Codec<std::remove_cvref_t<decltype(v.m_DifficultyLevel)>>::get(r);
        v.m_cDungeonMode = Codec<std::remove_cvref_t<decltype(v.m_cDungeonMode)>>::get(r);
        return v;
    }
};

struct KEGS_PARTY_CHANGE_HOST_NOT {
    std::int64_t m_iBeforeUID{};  // UidType
    std::int64_t m_iAfterUID{};  // UidType
    bool         m_bRegroupParty{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iBeforeUID)>>::put(w, m_iBeforeUID);
        Codec<std::remove_cvref_t<decltype(m_iAfterUID)>>::put(w, m_iAfterUID);
        Codec<std::remove_cvref_t<decltype(m_bRegroupParty)>>::put(w, m_bRegroupParty);
    }

    static KEGS_PARTY_CHANGE_HOST_NOT read_from(Reader& r) {
        KEGS_PARTY_CHANGE_HOST_NOT v;
        v.m_iBeforeUID = Codec<std::remove_cvref_t<decltype(v.m_iBeforeUID)>>::get(r);
        v.m_iAfterUID = Codec<std::remove_cvref_t<decltype(v.m_iAfterUID)>>::get(r);
        v.m_bRegroupParty = Codec<std::remove_cvref_t<decltype(v.m_bRegroupParty)>>::get(r);
        return v;
    }
};

struct KEGS_PARTY_CHANGE_READY_NOT {
    std::int64_t m_UnitUID{};  // UidType
    bool         m_bReady{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_UnitUID)>>::put(w, m_UnitUID);
        Codec<std::remove_cvref_t<decltype(m_bReady)>>::put(w, m_bReady);
    }

    static KEGS_PARTY_CHANGE_READY_NOT read_from(Reader& r) {
        KEGS_PARTY_CHANGE_READY_NOT v;
        v.m_UnitUID = Codec<std::remove_cvref_t<decltype(v.m_UnitUID)>>::get(r);
        v.m_bReady = Codec<std::remove_cvref_t<decltype(v.m_bReady)>>::get(r);
        return v;
    }
};

struct KEGS_PARTY_CHANGE_READY_REQ {
    bool m_bReady{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bReady)>>::put(w, m_bReady);
    }

    static KEGS_PARTY_CHANGE_READY_REQ read_from(Reader& r) {
        KEGS_PARTY_CHANGE_READY_REQ v;
        v.m_bReady = Codec<std::remove_cvref_t<decltype(v.m_bReady)>>::get(r);
        return v;
    }
};

struct KEGS_PARTY_FEVER_POINT_NOT {
    std::int32_t m_iPartyFever{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iPartyFever)>>::put(w, m_iPartyFever);
    }

    static KEGS_PARTY_FEVER_POINT_NOT read_from(Reader& r) {
        KEGS_PARTY_FEVER_POINT_NOT v;
        v.m_iPartyFever = Codec<std::remove_cvref_t<decltype(v.m_iPartyFever)>>::get(r);
        return v;
    }
};

struct KEGS_PARTY_GAME_START_FAILED_NOT {
    std::int32_t m_iReason{};  // int
    std::int64_t m_iFailUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iReason)>>::put(w, m_iReason);
        Codec<std::remove_cvref_t<decltype(m_iFailUnitUID)>>::put(w, m_iFailUnitUID);
    }

    static KEGS_PARTY_GAME_START_FAILED_NOT read_from(Reader& r) {
        KEGS_PARTY_GAME_START_FAILED_NOT v;
        v.m_iReason = Codec<std::remove_cvref_t<decltype(v.m_iReason)>>::get(r);
        v.m_iFailUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iFailUnitUID)>>::get(r);
        return v;
    }
};

struct KEGS_PARTY_GAME_START_OPINION_CHECK_NOT {
    std::int32_t m_iDungeonID{};  // int
    char         m_cDifficultyLevel{};
    char         m_cDungeonMode{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iDungeonID)>>::put(w, m_iDungeonID);
        Codec<std::remove_cvref_t<decltype(m_cDifficultyLevel)>>::put(w, m_cDifficultyLevel);
        Codec<std::remove_cvref_t<decltype(m_cDungeonMode)>>::put(w, m_cDungeonMode);
    }

    static KEGS_PARTY_GAME_START_OPINION_CHECK_NOT read_from(Reader& r) {
        KEGS_PARTY_GAME_START_OPINION_CHECK_NOT v;
        v.m_iDungeonID = Codec<std::remove_cvref_t<decltype(v.m_iDungeonID)>>::get(r);
        v.m_cDifficultyLevel = Codec<std::remove_cvref_t<decltype(v.m_cDifficultyLevel)>>::get(r);
        v.m_cDungeonMode = Codec<std::remove_cvref_t<decltype(v.m_cDungeonMode)>>::get(r);
        return v;
    }
};

struct KEGS_PARTY_GAME_START_OPINION_CHECK_REPLY_NOT {
    bool         m_bAccept{};
    std::int32_t m_iReason{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bAccept)>>::put(w, m_bAccept);
        Codec<std::remove_cvref_t<decltype(m_iReason)>>::put(w, m_iReason);
    }

    static KEGS_PARTY_GAME_START_OPINION_CHECK_REPLY_NOT read_from(Reader& r) {
        KEGS_PARTY_GAME_START_OPINION_CHECK_REPLY_NOT v;
        v.m_bAccept = Codec<std::remove_cvref_t<decltype(v.m_bAccept)>>::get(r);
        v.m_iReason = Codec<std::remove_cvref_t<decltype(v.m_iReason)>>::get(r);
        return v;
    }
};

struct KEGS_PARTY_GAME_START_REQ {
    std::int16_t m_sWorldID{};  // short

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_sWorldID)>>::put(w, m_sWorldID);
    }

    static KEGS_PARTY_GAME_START_REQ read_from(Reader& r) {
        KEGS_PARTY_GAME_START_REQ v;
        v.m_sWorldID = Codec<std::remove_cvref_t<decltype(v.m_sWorldID)>>::get(r);
        return v;
    }
};

struct KEGS_PC_BANG_AUTH_RESULT_NOT {
    char          m_cType{};
    std::uint32_t m_uiData{};  // u_int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cType)>>::put(w, m_cType);
        Codec<std::remove_cvref_t<decltype(m_uiData)>>::put(w, m_uiData);
    }

    static KEGS_PC_BANG_AUTH_RESULT_NOT read_from(Reader& r) {
        KEGS_PC_BANG_AUTH_RESULT_NOT v;
        v.m_cType = Codec<std::remove_cvref_t<decltype(v.m_cType)>>::get(r);
        v.m_uiData = Codec<std::remove_cvref_t<decltype(v.m_uiData)>>::get(r);
        return v;
    }
};

struct KEGS_PET_ACTION_NOT {
    std::int64_t m_iUnitUID{};  // UidType
    char         m_cActionType{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_cActionType)>>::put(w, m_cActionType);
    }

    static KEGS_PET_ACTION_NOT read_from(Reader& r) {
        KEGS_PET_ACTION_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_cActionType = Codec<std::remove_cvref_t<decltype(v.m_cActionType)>>::get(r);
        return v;
    }
};

struct KEGS_PET_EVOLUTION_REQ {
    std::int64_t m_iPetUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iPetUID)>>::put(w, m_iPetUID);
    }

    static KEGS_PET_EVOLUTION_REQ read_from(Reader& r) {
        KEGS_PET_EVOLUTION_REQ v;
        v.m_iPetUID = Codec<std::remove_cvref_t<decltype(v.m_iPetUID)>>::get(r);
        return v;
    }
};

struct KEGS_PICK_UP_FROM_PSHOP_AGENCY_REQ {
    std::vector<std::int64_t> m_vecPickUpItemList{};  // std::vector< UidType >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecPickUpItemList)>>::put(w, m_vecPickUpItemList);
    }

    static KEGS_PICK_UP_FROM_PSHOP_AGENCY_REQ read_from(Reader& r) {
        KEGS_PICK_UP_FROM_PSHOP_AGENCY_REQ v;
        v.m_vecPickUpItemList = Codec<std::remove_cvref_t<decltype(v.m_vecPickUpItemList)>>::get(r);
        return v;
    }
};

struct KEGS_PRESENT_MESSAGE_TO_RECEIVER_NOT {
    std::u16string   m_wstrSenderNickName{};  // std::wstring
    std::u16string   m_wstrReceiverUserID{};  // std::wstring
    std::u16string   m_wstrMessage{};  // std::wstring
    KBillProductInfo m_kBillProductInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrSenderNickName)>>::put(w, m_wstrSenderNickName);
        Codec<std::remove_cvref_t<decltype(m_wstrReceiverUserID)>>::put(w, m_wstrReceiverUserID);
        Codec<std::remove_cvref_t<decltype(m_wstrMessage)>>::put(w, m_wstrMessage);
        Codec<std::remove_cvref_t<decltype(m_kBillProductInfo)>>::put(w, m_kBillProductInfo);
    }

    static KEGS_PRESENT_MESSAGE_TO_RECEIVER_NOT read_from(Reader& r) {
        KEGS_PRESENT_MESSAGE_TO_RECEIVER_NOT v;
        v.m_wstrSenderNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrSenderNickName)>>::get(r);
        v.m_wstrReceiverUserID = Codec<std::remove_cvref_t<decltype(v.m_wstrReceiverUserID)>>::get(r);
        v.m_wstrMessage = Codec<std::remove_cvref_t<decltype(v.m_wstrMessage)>>::get(r);
        v.m_kBillProductInfo = Codec<std::remove_cvref_t<decltype(v.m_kBillProductInfo)>>::get(r);
        return v;
    }
};

struct KEGS_PSHOP_AGENCY_MESSAGE_NOT {
    char         m_cMessageType{};
    std::int64_t m_iHostUnitUID{};  // UidType
    std::int32_t m_iSellItemID{};  // int
    std::int32_t m_iSellQuantity{};  // int
    std::int32_t m_iSellED{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cMessageType)>>::put(w, m_cMessageType);
        Codec<std::remove_cvref_t<decltype(m_iHostUnitUID)>>::put(w, m_iHostUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iSellItemID)>>::put(w, m_iSellItemID);
        Codec<std::remove_cvref_t<decltype(m_iSellQuantity)>>::put(w, m_iSellQuantity);
        Codec<std::remove_cvref_t<decltype(m_iSellED)>>::put(w, m_iSellED);
    }

    static KEGS_PSHOP_AGENCY_MESSAGE_NOT read_from(Reader& r) {
        KEGS_PSHOP_AGENCY_MESSAGE_NOT v;
        v.m_cMessageType = Codec<std::remove_cvref_t<decltype(v.m_cMessageType)>>::get(r);
        v.m_iHostUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iHostUnitUID)>>::get(r);
        v.m_iSellItemID = Codec<std::remove_cvref_t<decltype(v.m_iSellItemID)>>::get(r);
        v.m_iSellQuantity = Codec<std::remove_cvref_t<decltype(v.m_iSellQuantity)>>::get(r);
        v.m_iSellED = Codec<std::remove_cvref_t<decltype(v.m_iSellED)>>::get(r);
        return v;
    }
};

struct KEGS_PURCHASED_CASH_ITEM_LIST_REQ {
    std::int32_t m_nItemPerPage{};  // int
    std::int32_t m_iCurrentPage{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_nItemPerPage)>>::put(w, m_nItemPerPage);
        Codec<std::remove_cvref_t<decltype(m_iCurrentPage)>>::put(w, m_iCurrentPage);
    }

    static KEGS_PURCHASED_CASH_ITEM_LIST_REQ read_from(Reader& r) {
        KEGS_PURCHASED_CASH_ITEM_LIST_REQ v;
        v.m_nItemPerPage = Codec<std::remove_cvref_t<decltype(v.m_nItemPerPage)>>::get(r);
        v.m_iCurrentPage = Codec<std::remove_cvref_t<decltype(v.m_iCurrentPage)>>::get(r);
        return v;
    }
};

struct KEGS_PVP_KILL_DIE_INFO_NOT {
    bool         m_bShowKillCount{};
    std::int32_t m_iKillCount{};  // int
    std::int32_t m_iDieCount{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bShowKillCount)>>::put(w, m_bShowKillCount);
        Codec<std::remove_cvref_t<decltype(m_iKillCount)>>::put(w, m_iKillCount);
        Codec<std::remove_cvref_t<decltype(m_iDieCount)>>::put(w, m_iDieCount);
    }

    static KEGS_PVP_KILL_DIE_INFO_NOT read_from(Reader& r) {
        KEGS_PVP_KILL_DIE_INFO_NOT v;
        v.m_bShowKillCount = Codec<std::remove_cvref_t<decltype(v.m_bShowKillCount)>>::get(r);
        v.m_iKillCount = Codec<std::remove_cvref_t<decltype(v.m_iKillCount)>>::get(r);
        v.m_iDieCount = Codec<std::remove_cvref_t<decltype(v.m_iDieCount)>>::get(r);
        return v;
    }
};

struct KEGS_PVP_NPC_UNIT_INFO_TO_SERVER_REQ {
    std::map<std::int64_t, float> m_mapPvpNpcInfo{};  // std::map< UidType, float >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_mapPvpNpcInfo)>>::put(w, m_mapPvpNpcInfo);
    }

    static KEGS_PVP_NPC_UNIT_INFO_TO_SERVER_REQ read_from(Reader& r) {
        KEGS_PVP_NPC_UNIT_INFO_TO_SERVER_REQ v;
        v.m_mapPvpNpcInfo = Codec<std::remove_cvref_t<decltype(v.m_mapPvpNpcInfo)>>::get(r);
        return v;
    }
};

struct KEGS_PVP_NPC_UNIT_RE_BIRTH_POS_REQ {
    std::int64_t m_iPvpNpcUnitUID{};  // UidType
    std::int32_t m_iStartPosIndex{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iPvpNpcUnitUID)>>::put(w, m_iPvpNpcUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iStartPosIndex)>>::put(w, m_iStartPosIndex);
    }

    static KEGS_PVP_NPC_UNIT_RE_BIRTH_POS_REQ read_from(Reader& r) {
        KEGS_PVP_NPC_UNIT_RE_BIRTH_POS_REQ v;
        v.m_iPvpNpcUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iPvpNpcUnitUID)>>::get(r);
        v.m_iStartPosIndex = Codec<std::remove_cvref_t<decltype(v.m_iStartPosIndex)>>::get(r);
        return v;
    }
};

struct KEGS_PVP_PARTY_CHANGE_MATCH_INFO_REQ {
    char m_cPvpGameType{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cPvpGameType)>>::put(w, m_cPvpGameType);
    }

    static KEGS_PVP_PARTY_CHANGE_MATCH_INFO_REQ read_from(Reader& r) {
        KEGS_PVP_PARTY_CHANGE_MATCH_INFO_REQ v;
        v.m_cPvpGameType = Codec<std::remove_cvref_t<decltype(v.m_cPvpGameType)>>::get(r);
        return v;
    }
};

struct KEGS_PVP_REMATCH_ACK {
    std::int32_t m_iOK{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
    }

    static KEGS_PVP_REMATCH_ACK read_from(Reader& r) {
        KEGS_PVP_REMATCH_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        return v;
    }
};

struct KEGS_PVP_REMATCH_NOT {
    std::int64_t m_iUnitUID{};  // UidType
    bool         m_bAcceptRematch{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_bAcceptRematch)>>::put(w, m_bAcceptRematch);
    }

    static KEGS_PVP_REMATCH_NOT read_from(Reader& r) {
        KEGS_PVP_REMATCH_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_bAcceptRematch = Codec<std::remove_cvref_t<decltype(v.m_bAcceptRematch)>>::get(r);
        return v;
    }
};

struct KEGS_PVP_REMATCH_REQ {
    bool m_bAcceptRematch{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bAcceptRematch)>>::put(w, m_bAcceptRematch);
    }

    static KEGS_PVP_REMATCH_REQ read_from(Reader& r) {
        KEGS_PVP_REMATCH_REQ v;
        v.m_bAcceptRematch = Codec<std::remove_cvref_t<decltype(v.m_bAcceptRematch)>>::get(r);
        return v;
    }
};

struct KEGS_QUEST_COMPLETE_REQ {
    std::int32_t                         m_iQuestID{};  // int
    std::int32_t                         m_iTalkNPCID{};  // int
    std::map<std::int32_t, std::int32_t> m_mapSelectItemID{};  // std::map< int, int >
    bool                                 m_bAutoComplete{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iQuestID)>>::put(w, m_iQuestID);
        Codec<std::remove_cvref_t<decltype(m_iTalkNPCID)>>::put(w, m_iTalkNPCID);
        Codec<std::remove_cvref_t<decltype(m_mapSelectItemID)>>::put(w, m_mapSelectItemID);
        Codec<std::remove_cvref_t<decltype(m_bAutoComplete)>>::put(w, m_bAutoComplete);
    }

    static KEGS_QUEST_COMPLETE_REQ read_from(Reader& r) {
        KEGS_QUEST_COMPLETE_REQ v;
        v.m_iQuestID = Codec<std::remove_cvref_t<decltype(v.m_iQuestID)>>::get(r);
        v.m_iTalkNPCID = Codec<std::remove_cvref_t<decltype(v.m_iTalkNPCID)>>::get(r);
        v.m_mapSelectItemID = Codec<std::remove_cvref_t<decltype(v.m_mapSelectItemID)>>::get(r);
        v.m_bAutoComplete = Codec<std::remove_cvref_t<decltype(v.m_bAutoComplete)>>::get(r);
        return v;
    }
};

struct KEGS_ALL_COMPLETED_QUEST_COMPLETE_REQ {
    std::vector<KEGS_QUEST_COMPLETE_REQ> m_vecQuestCompleteReq{};  // std::vector< KEGS_QUEST_COMPLETE_REQ >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecQuestCompleteReq)>>::put(w, m_vecQuestCompleteReq);
    }

    static KEGS_ALL_COMPLETED_QUEST_COMPLETE_REQ read_from(Reader& r) {
        KEGS_ALL_COMPLETED_QUEST_COMPLETE_REQ v;
        v.m_vecQuestCompleteReq = Codec<std::remove_cvref_t<decltype(v.m_vecQuestCompleteReq)>>::get(r);
        return v;
    }
};

struct KEGS_QUICK_JOIN_REQ {
    std::int32_t m_iOption{};  // int
    bool         m_bIsItemMode{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOption)>>::put(w, m_iOption);
        Codec<std::remove_cvref_t<decltype(m_bIsItemMode)>>::put(w, m_bIsItemMode);
    }

    static KEGS_QUICK_JOIN_REQ read_from(Reader& r) {
        KEGS_QUICK_JOIN_REQ v;
        v.m_iOption = Codec<std::remove_cvref_t<decltype(v.m_iOption)>>::get(r);
        v.m_bIsItemMode = Codec<std::remove_cvref_t<decltype(v.m_bIsItemMode)>>::get(r);
        return v;
    }
};

struct KEGS_QUICK_START_DUNGEON_GAME_ACK {
    std::int32_t   m_iOK{};  // int
    std::int64_t   m_iFailUnitUID{};  // UidType
    std::u16string m_wstrFailUserNickName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iFailUnitUID)>>::put(w, m_iFailUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrFailUserNickName)>>::put(w, m_wstrFailUserNickName);
    }

    static KEGS_QUICK_START_DUNGEON_GAME_ACK read_from(Reader& r) {
        KEGS_QUICK_START_DUNGEON_GAME_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iFailUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iFailUnitUID)>>::get(r);
        v.m_wstrFailUserNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrFailUserNickName)>>::get(r);
        return v;
    }
};

struct KEGS_QUICK_START_DUNGEON_GAME_REQ {
    std::int32_t m_iDungeonID{};  // int
    char         m_DifficultyLevel{};
    char         m_cGetItemType{};
    char         m_cDungeonMode{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iDungeonID)>>::put(w, m_iDungeonID);
        Codec<std::remove_cvref_t<decltype(m_DifficultyLevel)>>::put(w, m_DifficultyLevel);
        Codec<std::remove_cvref_t<decltype(m_cGetItemType)>>::put(w, m_cGetItemType);
        Codec<std::remove_cvref_t<decltype(m_cDungeonMode)>>::put(w, m_cDungeonMode);
    }

    static KEGS_QUICK_START_DUNGEON_GAME_REQ read_from(Reader& r) {
        KEGS_QUICK_START_DUNGEON_GAME_REQ v;
        v.m_iDungeonID = Codec<std::remove_cvref_t<decltype(v.m_iDungeonID)>>::get(r);
        v.m_DifficultyLevel = Codec<std::remove_cvref_t<decltype(v.m_DifficultyLevel)>>::get(r);
        v.m_cGetItemType = Codec<std::remove_cvref_t<decltype(v.m_cGetItemType)>>::get(r);
        v.m_cDungeonMode = Codec<std::remove_cvref_t<decltype(v.m_cDungeonMode)>>::get(r);
        return v;
    }
};

struct KEGS_READ_LETTER_REQ {
    std::int64_t m_iPostNo{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iPostNo)>>::put(w, m_iPostNo);
    }

    static KEGS_READ_LETTER_REQ read_from(Reader& r) {
        KEGS_READ_LETTER_REQ v;
        v.m_iPostNo = Codec<std::remove_cvref_t<decltype(v.m_iPostNo)>>::get(r);
        return v;
    }
};

struct KEGS_RECEIVE_LETTER_NOT {
    std::u16string m_wstrNickName{};  // std::wstring
    char           m_cScriptType{};
    std::u16string m_wstrLetterTitle{};  // std::wstring
    std::int32_t   m_iScriptIndex{};  // int
    std::int64_t   m_iFromUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_cScriptType)>>::put(w, m_cScriptType);
        Codec<std::remove_cvref_t<decltype(m_wstrLetterTitle)>>::put(w, m_wstrLetterTitle);
        Codec<std::remove_cvref_t<decltype(m_iScriptIndex)>>::put(w, m_iScriptIndex);
        Codec<std::remove_cvref_t<decltype(m_iFromUnitUID)>>::put(w, m_iFromUnitUID);
    }

    static KEGS_RECEIVE_LETTER_NOT read_from(Reader& r) {
        KEGS_RECEIVE_LETTER_NOT v;
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_cScriptType = Codec<std::remove_cvref_t<decltype(v.m_cScriptType)>>::get(r);
        v.m_wstrLetterTitle = Codec<std::remove_cvref_t<decltype(v.m_wstrLetterTitle)>>::get(r);
        v.m_iScriptIndex = Codec<std::remove_cvref_t<decltype(v.m_iScriptIndex)>>::get(r);
        v.m_iFromUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iFromUnitUID)>>::get(r);
        return v;
    }
};

struct KEGS_RECOMMEND_USER_ACK {
    std::int32_t m_iOK{};  // int
    std::int32_t m_iRewardResurrectionStone{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iRewardResurrectionStone)>>::put(w, m_iRewardResurrectionStone);
    }

    static KEGS_RECOMMEND_USER_ACK read_from(Reader& r) {
        KEGS_RECOMMEND_USER_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iRewardResurrectionStone = Codec<std::remove_cvref_t<decltype(v.m_iRewardResurrectionStone)>>::get(r);
        return v;
    }
};

struct KEGS_RECOMMEND_USER_INFO_NOT {
    std::int64_t m_iRecommendUnitUID{};  // UidType
    char         m_cType{};
    std::int32_t m_iData{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iRecommendUnitUID)>>::put(w, m_iRecommendUnitUID);
        Codec<std::remove_cvref_t<decltype(m_cType)>>::put(w, m_cType);
        Codec<std::remove_cvref_t<decltype(m_iData)>>::put(w, m_iData);
    }

    static KEGS_RECOMMEND_USER_INFO_NOT read_from(Reader& r) {
        KEGS_RECOMMEND_USER_INFO_NOT v;
        v.m_iRecommendUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iRecommendUnitUID)>>::get(r);
        v.m_cType = Codec<std::remove_cvref_t<decltype(v.m_cType)>>::get(r);
        v.m_iData = Codec<std::remove_cvref_t<decltype(v.m_iData)>>::get(r);
        return v;
    }
};

struct KEGS_RECOMMEND_USER_REQ {
    std::u16string m_wstrNickName{};  // std::wstring
    bool           m_bCancel{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_bCancel)>>::put(w, m_bCancel);
    }

    static KEGS_RECOMMEND_USER_REQ read_from(Reader& r) {
        KEGS_RECOMMEND_USER_REQ v;
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_bCancel = Codec<std::remove_cvref_t<decltype(v.m_bCancel)>>::get(r);
        return v;
    }
};

struct KEGS_RECOMMEND_USER_RESULT_NOT {
    std::int64_t   m_iRecommendedUnitUID{};  // UidType
    std::int64_t   m_iRecommendUnitUID{};  // UidType
    std::u16string m_wstrFromNickName{};  // std::wstring
    std::uint8_t   m_ucLevel{};  // u_char
    std::u16string m_wstrRecommendDate{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iRecommendedUnitUID)>>::put(w, m_iRecommendedUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iRecommendUnitUID)>>::put(w, m_iRecommendUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrFromNickName)>>::put(w, m_wstrFromNickName);
        Codec<std::remove_cvref_t<decltype(m_ucLevel)>>::put(w, m_ucLevel);
        Codec<std::remove_cvref_t<decltype(m_wstrRecommendDate)>>::put(w, m_wstrRecommendDate);
    }

    static KEGS_RECOMMEND_USER_RESULT_NOT read_from(Reader& r) {
        KEGS_RECOMMEND_USER_RESULT_NOT v;
        v.m_iRecommendedUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iRecommendedUnitUID)>>::get(r);
        v.m_iRecommendUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iRecommendUnitUID)>>::get(r);
        v.m_wstrFromNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrFromNickName)>>::get(r);
        v.m_ucLevel = Codec<std::remove_cvref_t<decltype(v.m_ucLevel)>>::get(r);
        v.m_wstrRecommendDate = Codec<std::remove_cvref_t<decltype(v.m_wstrRecommendDate)>>::get(r);
        return v;
    }
};

struct KEGS_REGISTRATION_GUILD_AD_ACK {
    std::int32_t   m_iOK{};  // int
    std::int32_t   m_iED{};  // int
    std::int16_t   m_sPeriod{};  // short
    std::u16string m_wstrAdMessage{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iED)>>::put(w, m_iED);
        Codec<std::remove_cvref_t<decltype(m_sPeriod)>>::put(w, m_sPeriod);
        Codec<std::remove_cvref_t<decltype(m_wstrAdMessage)>>::put(w, m_wstrAdMessage);
    }

    static KEGS_REGISTRATION_GUILD_AD_ACK read_from(Reader& r) {
        KEGS_REGISTRATION_GUILD_AD_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iED = Codec<std::remove_cvref_t<decltype(v.m_iED)>>::get(r);
        v.m_sPeriod = Codec<std::remove_cvref_t<decltype(v.m_sPeriod)>>::get(r);
        v.m_wstrAdMessage = Codec<std::remove_cvref_t<decltype(v.m_wstrAdMessage)>>::get(r);
        return v;
    }
};

struct KEGS_REGISTRATION_GUILD_AD_REQ {
    std::int16_t   m_sPeriod{};  // short
    std::u16string m_wstrAdMessage{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_sPeriod)>>::put(w, m_sPeriod);
        Codec<std::remove_cvref_t<decltype(m_wstrAdMessage)>>::put(w, m_wstrAdMessage);
    }

    static KEGS_REGISTRATION_GUILD_AD_REQ read_from(Reader& r) {
        KEGS_REGISTRATION_GUILD_AD_REQ v;
        v.m_sPeriod = Codec<std::remove_cvref_t<decltype(v.m_sPeriod)>>::get(r);
        v.m_wstrAdMessage = Codec<std::remove_cvref_t<decltype(v.m_wstrAdMessage)>>::get(r);
        return v;
    }
};

struct KEGS_REG_AUTO_PARTY_WAIT_LIST_SUCCESS_NOT {
    std::int64_t m_iWaitNumber{};  // UidType
    std::int32_t m_iEstimatedTime{};  // int
    bool         m_bRemakingAutoParty{};
    bool         m_bAutoPartyBonus{};
    std::int32_t m_iAutoPartyWaitUserCount{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iWaitNumber)>>::put(w, m_iWaitNumber);
        Codec<std::remove_cvref_t<decltype(m_iEstimatedTime)>>::put(w, m_iEstimatedTime);
        Codec<std::remove_cvref_t<decltype(m_bRemakingAutoParty)>>::put(w, m_bRemakingAutoParty);
        Codec<std::remove_cvref_t<decltype(m_bAutoPartyBonus)>>::put(w, m_bAutoPartyBonus);
        Codec<std::remove_cvref_t<decltype(m_iAutoPartyWaitUserCount)>>::put(w, m_iAutoPartyWaitUserCount);
    }

    static KEGS_REG_AUTO_PARTY_WAIT_LIST_SUCCESS_NOT read_from(Reader& r) {
        KEGS_REG_AUTO_PARTY_WAIT_LIST_SUCCESS_NOT v;
        v.m_iWaitNumber = Codec<std::remove_cvref_t<decltype(v.m_iWaitNumber)>>::get(r);
        v.m_iEstimatedTime = Codec<std::remove_cvref_t<decltype(v.m_iEstimatedTime)>>::get(r);
        v.m_bRemakingAutoParty = Codec<std::remove_cvref_t<decltype(v.m_bRemakingAutoParty)>>::get(r);
        v.m_bAutoPartyBonus = Codec<std::remove_cvref_t<decltype(v.m_bAutoPartyBonus)>>::get(r);
        v.m_iAutoPartyWaitUserCount = Codec<std::remove_cvref_t<decltype(v.m_iAutoPartyWaitUserCount)>>::get(r);
        return v;
    }
};

struct KEGS_REG_MATCH_WAIT_LIST_SUCCESS_NOT {
    std::int32_t m_iEstimatedTime{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iEstimatedTime)>>::put(w, m_iEstimatedTime);
    }

    static KEGS_REG_MATCH_WAIT_LIST_SUCCESS_NOT read_from(Reader& r) {
        KEGS_REG_MATCH_WAIT_LIST_SUCCESS_NOT v;
        v.m_iEstimatedTime = Codec<std::remove_cvref_t<decltype(v.m_iEstimatedTime)>>::get(r);
        return v;
    }
};

struct KEGS_REG_SKILL_NOTE_MEMO_REQ {
    char         m_cSkillNotePageNum{};
    std::int64_t m_iItemUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cSkillNotePageNum)>>::put(w, m_cSkillNotePageNum);
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
    }

    static KEGS_REG_SKILL_NOTE_MEMO_REQ read_from(Reader& r) {
        KEGS_REG_SKILL_NOTE_MEMO_REQ v;
        v.m_cSkillNotePageNum = Codec<std::remove_cvref_t<decltype(v.m_cSkillNotePageNum)>>::get(r);
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        return v;
    }
};

struct KEGS_RELATIONSHIP_EFFECT_NOT {
    std::int64_t   m_iManUID{};  // UidType
    std::int64_t   m_iGirlUID{};  // UidType
    std::int32_t   m_iEffectIndex{};  // int
    std::u16string m_wstrLoveWord{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iManUID)>>::put(w, m_iManUID);
        Codec<std::remove_cvref_t<decltype(m_iGirlUID)>>::put(w, m_iGirlUID);
        Codec<std::remove_cvref_t<decltype(m_iEffectIndex)>>::put(w, m_iEffectIndex);
        Codec<std::remove_cvref_t<decltype(m_wstrLoveWord)>>::put(w, m_wstrLoveWord);
    }

    static KEGS_RELATIONSHIP_EFFECT_NOT read_from(Reader& r) {
        KEGS_RELATIONSHIP_EFFECT_NOT v;
        v.m_iManUID = Codec<std::remove_cvref_t<decltype(v.m_iManUID)>>::get(r);
        v.m_iGirlUID = Codec<std::remove_cvref_t<decltype(v.m_iGirlUID)>>::get(r);
        v.m_iEffectIndex = Codec<std::remove_cvref_t<decltype(v.m_iEffectIndex)>>::get(r);
        v.m_wstrLoveWord = Codec<std::remove_cvref_t<decltype(v.m_wstrLoveWord)>>::get(r);
        return v;
    }
};

struct KEGS_RELATIONSHIP_INFO_REQ {
    std::int64_t m_iAcceptUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iAcceptUnitUID)>>::put(w, m_iAcceptUnitUID);
    }

    static KEGS_RELATIONSHIP_INFO_REQ read_from(Reader& r) {
        KEGS_RELATIONSHIP_INFO_REQ v;
        v.m_iAcceptUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iAcceptUnitUID)>>::get(r);
        return v;
    }
};

struct KEGS_RELEASE_PET_ACK {
    std::int64_t m_iOK{};  // UidType
    std::int64_t m_iPetUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iPetUID)>>::put(w, m_iPetUID);
    }

    static KEGS_RELEASE_PET_ACK read_from(Reader& r) {
        KEGS_RELEASE_PET_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iPetUID = Codec<std::remove_cvref_t<decltype(v.m_iPetUID)>>::get(r);
        return v;
    }
};

struct KEGS_RELEASE_PET_NOT {
    std::int64_t m_iPetUID{};  // UidType
    std::int64_t m_iUnitUID{};  // UidType
    std::int32_t m_iPetID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iPetUID)>>::put(w, m_iPetUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iPetID)>>::put(w, m_iPetID);
    }

    static KEGS_RELEASE_PET_NOT read_from(Reader& r) {
        KEGS_RELEASE_PET_NOT v;
        v.m_iPetUID = Codec<std::remove_cvref_t<decltype(v.m_iPetUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iPetID = Codec<std::remove_cvref_t<decltype(v.m_iPetID)>>::get(r);
        return v;
    }
};

struct KEGS_RELEASE_PET_REQ {
    std::int64_t m_iPetUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iPetUID)>>::put(w, m_iPetUID);
    }

    static KEGS_RELEASE_PET_REQ read_from(Reader& r) {
        KEGS_RELEASE_PET_REQ v;
        v.m_iPetUID = Codec<std::remove_cvref_t<decltype(v.m_iPetUID)>>::get(r);
        return v;
    }
};

struct KEGS_RELEASE_RIDING_PET_ACK {
    std::int32_t m_iOK{};  // int
    std::int64_t m_iRidingPetUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iRidingPetUID)>>::put(w, m_iRidingPetUID);
    }

    static KEGS_RELEASE_RIDING_PET_ACK read_from(Reader& r) {
        KEGS_RELEASE_RIDING_PET_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iRidingPetUID = Codec<std::remove_cvref_t<decltype(v.m_iRidingPetUID)>>::get(r);
        return v;
    }
};

struct KEGS_RELEASE_RIDING_PET_NOT {
    std::int64_t  m_iRidingPetUID{};  // UidType
    std::uint16_t m_usRindingPetID{};  // USHORT

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iRidingPetUID)>>::put(w, m_iRidingPetUID);
        Codec<std::remove_cvref_t<decltype(m_usRindingPetID)>>::put(w, m_usRindingPetID);
    }

    static KEGS_RELEASE_RIDING_PET_NOT read_from(Reader& r) {
        KEGS_RELEASE_RIDING_PET_NOT v;
        v.m_iRidingPetUID = Codec<std::remove_cvref_t<decltype(v.m_iRidingPetUID)>>::get(r);
        v.m_usRindingPetID = Codec<std::remove_cvref_t<decltype(v.m_usRindingPetID)>>::get(r);
        return v;
    }
};

struct KEGS_RELEASE_RIDING_PET_REQ {
    std::int64_t m_iRidingPetUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iRidingPetUID)>>::put(w, m_iRidingPetUID);
    }

    static KEGS_RELEASE_RIDING_PET_REQ read_from(Reader& r) {
        KEGS_RELEASE_RIDING_PET_REQ v;
        v.m_iRidingPetUID = Codec<std::remove_cvref_t<decltype(v.m_iRidingPetUID)>>::get(r);
        return v;
    }
};

struct KEGS_RENAME_FRIEND_GROUP_ACK {
    std::int32_t   m_iOK{};  // int
    char           m_cGroupID{};
    std::u16string m_wstrGroupName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_cGroupID)>>::put(w, m_cGroupID);
        Codec<std::remove_cvref_t<decltype(m_wstrGroupName)>>::put(w, m_wstrGroupName);
    }

    static KEGS_RENAME_FRIEND_GROUP_ACK read_from(Reader& r) {
        KEGS_RENAME_FRIEND_GROUP_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_cGroupID = Codec<std::remove_cvref_t<decltype(v.m_cGroupID)>>::get(r);
        v.m_wstrGroupName = Codec<std::remove_cvref_t<decltype(v.m_wstrGroupName)>>::get(r);
        return v;
    }
};

struct KEGS_RENAME_FRIEND_GROUP_REQ {
    char           m_cGroupID{};
    std::u16string m_wstrGroupName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cGroupID)>>::put(w, m_cGroupID);
        Codec<std::remove_cvref_t<decltype(m_wstrGroupName)>>::put(w, m_wstrGroupName);
    }

    static KEGS_RENAME_FRIEND_GROUP_REQ read_from(Reader& r) {
        KEGS_RENAME_FRIEND_GROUP_REQ v;
        v.m_cGroupID = Codec<std::remove_cvref_t<decltype(v.m_cGroupID)>>::get(r);
        v.m_wstrGroupName = Codec<std::remove_cvref_t<decltype(v.m_wstrGroupName)>>::get(r);
        return v;
    }
};

struct KEGS_REPAIR_ITEM_REQ {
    std::vector<std::int64_t> m_vecItemUID{};  // std::vector< UidType >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecItemUID)>>::put(w, m_vecItemUID);
    }

    static KEGS_REPAIR_ITEM_REQ read_from(Reader& r) {
        KEGS_REPAIR_ITEM_REQ v;
        v.m_vecItemUID = Codec<std::remove_cvref_t<decltype(v.m_vecItemUID)>>::get(r);
        return v;
    }
};

struct KEGS_REQUEST_FRIEND_ACK {
    std::int32_t   m_iOK{};  // int
    std::int64_t   m_iUnitUID{};  // UidType
    std::u16string m_wstrNickName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
    }

    static KEGS_REQUEST_FRIEND_ACK read_from(Reader& r) {
        KEGS_REQUEST_FRIEND_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        return v;
    }
};

struct KEGS_REQUEST_FRIEND_NOT {
    std::int64_t   m_iUnitUID{};  // UidType
    std::u16string m_wstrNickName{};  // std::wstring
    std::u16string m_wstrMessage{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_wstrMessage)>>::put(w, m_wstrMessage);
    }

    static KEGS_REQUEST_FRIEND_NOT read_from(Reader& r) {
        KEGS_REQUEST_FRIEND_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_wstrMessage = Codec<std::remove_cvref_t<decltype(v.m_wstrMessage)>>::get(r);
        return v;
    }
};

struct KEGS_REQUEST_FRIEND_REQ {
    std::u16string m_wstrNickName{};  // std::wstring
    std::u16string m_wstrMessage{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_wstrMessage)>>::put(w, m_wstrMessage);
    }

    static KEGS_REQUEST_FRIEND_REQ read_from(Reader& r) {
        KEGS_REQUEST_FRIEND_REQ v;
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_wstrMessage = Codec<std::remove_cvref_t<decltype(v.m_wstrMessage)>>::get(r);
        return v;
    }
};

struct KEGS_REQUEST_GET_AUTO_PARTY_BONUS_INFO_NOT {
    std::int32_t                        m_iVillageMapID{};  // int
    std::vector<KDungeonGameSimpleInfo> m_vecDungeonList{};  // std::vector< KDungeonGameSimpleInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iVillageMapID)>>::put(w, m_iVillageMapID);
        Codec<std::remove_cvref_t<decltype(m_vecDungeonList)>>::put(w, m_vecDungeonList);
    }

    static KEGS_REQUEST_GET_AUTO_PARTY_BONUS_INFO_NOT read_from(Reader& r) {
        KEGS_REQUEST_GET_AUTO_PARTY_BONUS_INFO_NOT v;
        v.m_iVillageMapID = Codec<std::remove_cvref_t<decltype(v.m_iVillageMapID)>>::get(r);
        v.m_vecDungeonList = Codec<std::remove_cvref_t<decltype(v.m_vecDungeonList)>>::get(r);
        return v;
    }
};

struct KEGS_REQUEST_MATCH_MAKING_REQ {
    char m_cPvpPlayerCount{};
    char m_cPvpGameType{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cPvpPlayerCount)>>::put(w, m_cPvpPlayerCount);
        Codec<std::remove_cvref_t<decltype(m_cPvpGameType)>>::put(w, m_cPvpGameType);
    }

    static KEGS_REQUEST_MATCH_MAKING_REQ read_from(Reader& r) {
        KEGS_REQUEST_MATCH_MAKING_REQ v;
        v.m_cPvpPlayerCount = Codec<std::remove_cvref_t<decltype(v.m_cPvpPlayerCount)>>::get(r);
        v.m_cPvpGameType = Codec<std::remove_cvref_t<decltype(v.m_cPvpGameType)>>::get(r);
        return v;
    }
};

struct KEGS_REQUEST_TRADE_ACK {
    std::int32_t   m_iOK{};  // int
    std::int64_t   m_iUnitUID{};  // UidType
    std::u16string m_wstrNickName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
    }

    static KEGS_REQUEST_TRADE_ACK read_from(Reader& r) {
        KEGS_REQUEST_TRADE_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        return v;
    }
};

struct KEGS_REQUEST_TRADE_DENY_NOT {
    std::int32_t m_iOK{};  // int
    std::int64_t m_iUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
    }

    static KEGS_REQUEST_TRADE_DENY_NOT read_from(Reader& r) {
        KEGS_REQUEST_TRADE_DENY_NOT v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        return v;
    }
};

struct KEGS_REQUEST_TRADE_NOT {
    std::int64_t   m_iSenderUID{};  // UidType
    std::u16string m_wstrSenderNickName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iSenderUID)>>::put(w, m_iSenderUID);
        Codec<std::remove_cvref_t<decltype(m_wstrSenderNickName)>>::put(w, m_wstrSenderNickName);
    }

    static KEGS_REQUEST_TRADE_NOT read_from(Reader& r) {
        KEGS_REQUEST_TRADE_NOT v;
        v.m_iSenderUID = Codec<std::remove_cvref_t<decltype(v.m_iSenderUID)>>::get(r);
        v.m_wstrSenderNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrSenderNickName)>>::get(r);
        return v;
    }
};

struct KEGS_REQUEST_TRADE_REPLY_NOT {
    std::int64_t m_iReceiverUID{};  // UidType
    bool         m_bAccept{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iReceiverUID)>>::put(w, m_iReceiverUID);
        Codec<std::remove_cvref_t<decltype(m_bAccept)>>::put(w, m_bAccept);
    }

    static KEGS_REQUEST_TRADE_REPLY_NOT read_from(Reader& r) {
        KEGS_REQUEST_TRADE_REPLY_NOT v;
        v.m_iReceiverUID = Codec<std::remove_cvref_t<decltype(v.m_iReceiverUID)>>::get(r);
        v.m_bAccept = Codec<std::remove_cvref_t<decltype(v.m_bAccept)>>::get(r);
        return v;
    }
};

struct KEGS_REQUEST_TRADE_REQ {
    std::int64_t m_iUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
    }

    static KEGS_REQUEST_TRADE_REQ read_from(Reader& r) {
        KEGS_REQUEST_TRADE_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        return v;
    }
};

struct KEGS_REQUEST_TUTORIAL_REQ {
    std::int64_t m_iUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
    }

    static KEGS_REQUEST_TUTORIAL_REQ read_from(Reader& r) {
        KEGS_REQUEST_TUTORIAL_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        return v;
    }
};

struct KEGS_RESET_GUILD_SKILL_NOT {
    std::int32_t m_iDelGuildSkillID{};  // int
    std::int32_t m_iRestoreGuildSPoint{};  // int
    std::int32_t m_iRestoreGuildCSPoint{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iDelGuildSkillID)>>::put(w, m_iDelGuildSkillID);
        Codec<std::remove_cvref_t<decltype(m_iRestoreGuildSPoint)>>::put(w, m_iRestoreGuildSPoint);
        Codec<std::remove_cvref_t<decltype(m_iRestoreGuildCSPoint)>>::put(w, m_iRestoreGuildCSPoint);
    }

    static KEGS_RESET_GUILD_SKILL_NOT read_from(Reader& r) {
        KEGS_RESET_GUILD_SKILL_NOT v;
        v.m_iDelGuildSkillID = Codec<std::remove_cvref_t<decltype(v.m_iDelGuildSkillID)>>::get(r);
        v.m_iRestoreGuildSPoint = Codec<std::remove_cvref_t<decltype(v.m_iRestoreGuildSPoint)>>::get(r);
        v.m_iRestoreGuildCSPoint = Codec<std::remove_cvref_t<decltype(v.m_iRestoreGuildCSPoint)>>::get(r);
        return v;
    }
};

struct KEGS_RESET_GUILD_SKILL_REQ {
    std::int32_t m_iGuildSkillID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iGuildSkillID)>>::put(w, m_iGuildSkillID);
    }

    static KEGS_RESET_GUILD_SKILL_REQ read_from(Reader& r) {
        KEGS_RESET_GUILD_SKILL_REQ v;
        v.m_iGuildSkillID = Codec<std::remove_cvref_t<decltype(v.m_iGuildSkillID)>>::get(r);
        return v;
    }
};

struct KEGS_RESET_SKILL_REQ {
    std::int32_t m_iSkillID{};  // int
    std::int32_t m_iActiveSkillPageNumber{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iSkillID)>>::put(w, m_iSkillID);
        Codec<std::remove_cvref_t<decltype(m_iActiveSkillPageNumber)>>::put(w, m_iActiveSkillPageNumber);
    }

    static KEGS_RESET_SKILL_REQ read_from(Reader& r) {
        KEGS_RESET_SKILL_REQ v;
        v.m_iSkillID = Codec<std::remove_cvref_t<decltype(v.m_iSkillID)>>::get(r);
        v.m_iActiveSkillPageNumber = Codec<std::remove_cvref_t<decltype(v.m_iActiveSkillPageNumber)>>::get(r);
        return v;
    }
};

struct KEGS_RESTORE_ITEM_EVALUATE_REQ {
    std::int64_t m_iSupportItemUID{};  // UidType
    std::int64_t m_iTargetItemUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iSupportItemUID)>>::put(w, m_iSupportItemUID);
        Codec<std::remove_cvref_t<decltype(m_iTargetItemUID)>>::put(w, m_iTargetItemUID);
    }

    static KEGS_RESTORE_ITEM_EVALUATE_REQ read_from(Reader& r) {
        KEGS_RESTORE_ITEM_EVALUATE_REQ v;
        v.m_iSupportItemUID = Codec<std::remove_cvref_t<decltype(v.m_iSupportItemUID)>>::get(r);
        v.m_iTargetItemUID = Codec<std::remove_cvref_t<decltype(v.m_iTargetItemUID)>>::get(r);
        return v;
    }
};

struct KEGS_RESTORE_ITEM_REQ {
    std::int64_t m_iItemUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
    }

    static KEGS_RESTORE_ITEM_REQ read_from(Reader& r) {
        KEGS_RESTORE_ITEM_REQ v;
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        return v;
    }
};

struct KEGS_RESTORE_SPIRIT_NOT {
    std::int64_t m_iUnitUID{};  // UidType
    std::int32_t m_iSpirit{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iSpirit)>>::put(w, m_iSpirit);
    }

    static KEGS_RESTORE_SPIRIT_NOT read_from(Reader& r) {
        KEGS_RESTORE_SPIRIT_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iSpirit = Codec<std::remove_cvref_t<decltype(v.m_iSpirit)>>::get(r);
        return v;
    }
};

struct KEGS_RESULT_SUCCESS_ACK {
    std::int32_t m_iOK{};  // int
    std::int32_t m_iDungeonID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iDungeonID)>>::put(w, m_iDungeonID);
    }

    static KEGS_RESULT_SUCCESS_ACK read_from(Reader& r) {
        KEGS_RESULT_SUCCESS_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iDungeonID = Codec<std::remove_cvref_t<decltype(v.m_iDungeonID)>>::get(r);
        return v;
    }
};

struct KEGS_RESURRECT_TO_CONTINUE_DUNGEON_ACK {
    std::int32_t m_iOK{};  // int
    std::int32_t m_iDungeonID{};  // int
    std::int32_t m_iNumResurrectionStone{};  // int
    std::int32_t m_iNumAutoPaymentResStone{};  // int
    bool         m_bUseCash{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iDungeonID)>>::put(w, m_iDungeonID);
        Codec<std::remove_cvref_t<decltype(m_iNumResurrectionStone)>>::put(w, m_iNumResurrectionStone);
        Codec<std::remove_cvref_t<decltype(m_iNumAutoPaymentResStone)>>::put(w, m_iNumAutoPaymentResStone);
        Codec<std::remove_cvref_t<decltype(m_bUseCash)>>::put(w, m_bUseCash);
    }

    static KEGS_RESURRECT_TO_CONTINUE_DUNGEON_ACK read_from(Reader& r) {
        KEGS_RESURRECT_TO_CONTINUE_DUNGEON_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iDungeonID = Codec<std::remove_cvref_t<decltype(v.m_iDungeonID)>>::get(r);
        v.m_iNumResurrectionStone = Codec<std::remove_cvref_t<decltype(v.m_iNumResurrectionStone)>>::get(r);
        v.m_iNumAutoPaymentResStone = Codec<std::remove_cvref_t<decltype(v.m_iNumAutoPaymentResStone)>>::get(r);
        v.m_bUseCash = Codec<std::remove_cvref_t<decltype(v.m_bUseCash)>>::get(r);
        return v;
    }
};

struct KEGS_RESURRECT_TO_CONTINUE_DUNGEON_NOT {
    std::int64_t m_iUsedByUnitUID{};  // UidType
    std::int64_t m_iUnitUID{};  // UidType
    std::int32_t m_iStartPosIndex{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUsedByUnitUID)>>::put(w, m_iUsedByUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iStartPosIndex)>>::put(w, m_iStartPosIndex);
    }

    static KEGS_RESURRECT_TO_CONTINUE_DUNGEON_NOT read_from(Reader& r) {
        KEGS_RESURRECT_TO_CONTINUE_DUNGEON_NOT v;
        v.m_iUsedByUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUsedByUnitUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iStartPosIndex = Codec<std::remove_cvref_t<decltype(v.m_iStartPosIndex)>>::get(r);
        return v;
    }
};

struct KEGS_RESURRECT_TO_CONTINUE_DUNGEON_REQ {
    bool         m_bUseCash{};
    std::int64_t m_iUsedByUnitUID{};  // UidType
    std::int64_t m_iUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bUseCash)>>::put(w, m_bUseCash);
        Codec<std::remove_cvref_t<decltype(m_iUsedByUnitUID)>>::put(w, m_iUsedByUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
    }

    static KEGS_RESURRECT_TO_CONTINUE_DUNGEON_REQ read_from(Reader& r) {
        KEGS_RESURRECT_TO_CONTINUE_DUNGEON_REQ v;
        v.m_bUseCash = Codec<std::remove_cvref_t<decltype(v.m_bUseCash)>>::get(r);
        v.m_iUsedByUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUsedByUnitUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        return v;
    }
};

struct KEGS_REWARD_TO_RECOMMEND_USER_NOT {
    std::u16string                       m_wstrNickName{};  // std::wstring
    char                                 m_cLevel{};
    std::map<std::int32_t, std::int32_t> m_mapRewardItem{};  // std::map< int, int >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_cLevel)>>::put(w, m_cLevel);
        Codec<std::remove_cvref_t<decltype(m_mapRewardItem)>>::put(w, m_mapRewardItem);
    }

    static KEGS_REWARD_TO_RECOMMEND_USER_NOT read_from(Reader& r) {
        KEGS_REWARD_TO_RECOMMEND_USER_NOT v;
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_cLevel = Codec<std::remove_cvref_t<decltype(v.m_cLevel)>>::get(r);
        v.m_mapRewardItem = Codec<std::remove_cvref_t<decltype(v.m_mapRewardItem)>>::get(r);
        return v;
    }
};

struct KEGS_REWARD_TO_TUTORIAL_USER_NOT {
    std::u16string                       m_wstrNickName{};  // std::wstring
    char                                 m_cLevel{};
    std::map<std::int32_t, std::int32_t> m_mapRewardItem{};  // std::map< int, int >
    bool                                 m_bGraduation{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_cLevel)>>::put(w, m_cLevel);
        Codec<std::remove_cvref_t<decltype(m_mapRewardItem)>>::put(w, m_mapRewardItem);
        Codec<std::remove_cvref_t<decltype(m_bGraduation)>>::put(w, m_bGraduation);
    }

    static KEGS_REWARD_TO_TUTORIAL_USER_NOT read_from(Reader& r) {
        KEGS_REWARD_TO_TUTORIAL_USER_NOT v;
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_cLevel = Codec<std::remove_cvref_t<decltype(v.m_cLevel)>>::get(r);
        v.m_mapRewardItem = Codec<std::remove_cvref_t<decltype(v.m_mapRewardItem)>>::get(r);
        v.m_bGraduation = Codec<std::remove_cvref_t<decltype(v.m_bGraduation)>>::get(r);
        return v;
    }
};

struct KEGS_ROOM_LIST_REQ {
    std::uint32_t m_nViewPage{};  // UINT
    char          m_cPVPGameType{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_nViewPage)>>::put(w, m_nViewPage);
        Codec<std::remove_cvref_t<decltype(m_cPVPGameType)>>::put(w, m_cPVPGameType);
    }

    static KEGS_ROOM_LIST_REQ read_from(Reader& r) {
        KEGS_ROOM_LIST_REQ v;
        v.m_nViewPage = Codec<std::remove_cvref_t<decltype(v.m_nViewPage)>>::get(r);
        v.m_cPVPGameType = Codec<std::remove_cvref_t<decltype(v.m_cPVPGameType)>>::get(r);
        return v;
    }
};

struct KEGS_SEAL_ITEM_REQ {
    std::int64_t m_iDestItemUID{};  // UidType
    std::int64_t m_iSealItemUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iDestItemUID)>>::put(w, m_iDestItemUID);
        Codec<std::remove_cvref_t<decltype(m_iSealItemUID)>>::put(w, m_iSealItemUID);
    }

    static KEGS_SEAL_ITEM_REQ read_from(Reader& r) {
        KEGS_SEAL_ITEM_REQ v;
        v.m_iDestItemUID = Codec<std::remove_cvref_t<decltype(v.m_iDestItemUID)>>::get(r);
        v.m_iSealItemUID = Codec<std::remove_cvref_t<decltype(v.m_iSealItemUID)>>::get(r);
        return v;
    }
};

struct KEGS_SEARCH_TRADE_BOARD_REQ {
    std::u16string    m_wstrFinder{};  // std::wstring
    char              m_cSlotCategory{};
    char              m_cEqipPosition{};
    char              m_cUnitType{};
    char              m_cUnitClass{};
    char              m_cMinLevel{};
    char              m_cMaxLevel{};
    std::vector<char> m_vecItemGrade{};
    bool              m_bStrictSearchOption{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrFinder)>>::put(w, m_wstrFinder);
        Codec<std::remove_cvref_t<decltype(m_cSlotCategory)>>::put(w, m_cSlotCategory);
        Codec<std::remove_cvref_t<decltype(m_cEqipPosition)>>::put(w, m_cEqipPosition);
        Codec<std::remove_cvref_t<decltype(m_cUnitType)>>::put(w, m_cUnitType);
        Codec<std::remove_cvref_t<decltype(m_cUnitClass)>>::put(w, m_cUnitClass);
        Codec<std::remove_cvref_t<decltype(m_cMinLevel)>>::put(w, m_cMinLevel);
        Codec<std::remove_cvref_t<decltype(m_cMaxLevel)>>::put(w, m_cMaxLevel);
        Codec<std::remove_cvref_t<decltype(m_vecItemGrade)>>::put(w, m_vecItemGrade);
        Codec<std::remove_cvref_t<decltype(m_bStrictSearchOption)>>::put(w, m_bStrictSearchOption);
    }

    static KEGS_SEARCH_TRADE_BOARD_REQ read_from(Reader& r) {
        KEGS_SEARCH_TRADE_BOARD_REQ v;
        v.m_wstrFinder = Codec<std::remove_cvref_t<decltype(v.m_wstrFinder)>>::get(r);
        v.m_cSlotCategory = Codec<std::remove_cvref_t<decltype(v.m_cSlotCategory)>>::get(r);
        v.m_cEqipPosition = Codec<std::remove_cvref_t<decltype(v.m_cEqipPosition)>>::get(r);
        v.m_cUnitType = Codec<std::remove_cvref_t<decltype(v.m_cUnitType)>>::get(r);
        v.m_cUnitClass = Codec<std::remove_cvref_t<decltype(v.m_cUnitClass)>>::get(r);
        v.m_cMinLevel = Codec<std::remove_cvref_t<decltype(v.m_cMinLevel)>>::get(r);
        v.m_cMaxLevel = Codec<std::remove_cvref_t<decltype(v.m_cMaxLevel)>>::get(r);
        v.m_vecItemGrade = Codec<std::remove_cvref_t<decltype(v.m_vecItemGrade)>>::get(r);
        v.m_bStrictSearchOption = Codec<std::remove_cvref_t<decltype(v.m_bStrictSearchOption)>>::get(r);
        return v;
    }
};

struct KEGS_SEARCH_UNIT_ACK {
    std::int32_t        m_iOK{};  // int
    KConnectionUnitInfo m_kCUnitInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_kCUnitInfo)>>::put(w, m_kCUnitInfo);
    }

    static KEGS_SEARCH_UNIT_ACK read_from(Reader& r) {
        KEGS_SEARCH_UNIT_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_kCUnitInfo = Codec<std::remove_cvref_t<decltype(v.m_kCUnitInfo)>>::get(r);
        return v;
    }
};

struct KEGS_SECOND_SECURITY_INFO_NOT {
    std::int32_t m_iOK{};  // int
    bool         m_bUseSecondPW{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_bUseSecondPW)>>::put(w, m_bUseSecondPW);
    }

    static KEGS_SECOND_SECURITY_INFO_NOT read_from(Reader& r) {
        KEGS_SECOND_SECURITY_INFO_NOT v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_bUseSecondPW = Codec<std::remove_cvref_t<decltype(v.m_bUseSecondPW)>>::get(r);
        return v;
    }
};

struct KEGS_SECRET_STAGE_LOAD_REQ {
    std::int32_t m_iPadID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iPadID)>>::put(w, m_iPadID);
    }

    static KEGS_SECRET_STAGE_LOAD_REQ read_from(Reader& r) {
        KEGS_SECRET_STAGE_LOAD_REQ v;
        v.m_iPadID = Codec<std::remove_cvref_t<decltype(v.m_iPadID)>>::get(r);
        return v;
    }
};

struct KEGS_SELECT_REWARD_BOX_ACK {
    std::int32_t m_iOK{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
    }

    static KEGS_SELECT_REWARD_BOX_ACK read_from(Reader& r) {
        KEGS_SELECT_REWARD_BOX_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        return v;
    }
};

struct KEGS_SELECT_REWARD_BOX_NOT {
    char         m_BoxNum{};
    std::int64_t m_UnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_BoxNum)>>::put(w, m_BoxNum);
        Codec<std::remove_cvref_t<decltype(m_UnitUID)>>::put(w, m_UnitUID);
    }

    static KEGS_SELECT_REWARD_BOX_NOT read_from(Reader& r) {
        KEGS_SELECT_REWARD_BOX_NOT v;
        v.m_BoxNum = Codec<std::remove_cvref_t<decltype(v.m_BoxNum)>>::get(r);
        v.m_UnitUID = Codec<std::remove_cvref_t<decltype(v.m_UnitUID)>>::get(r);
        return v;
    }
};

struct KEGS_SELECT_REWARD_BOX_REQ {
    char m_BoxNum{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_BoxNum)>>::put(w, m_BoxNum);
    }

    static KEGS_SELECT_REWARD_BOX_REQ read_from(Reader& r) {
        KEGS_SELECT_REWARD_BOX_REQ v;
        v.m_BoxNum = Codec<std::remove_cvref_t<decltype(v.m_BoxNum)>>::get(r);
        return v;
    }
};

struct KEGS_SELECT_SERVER_SET_ACK {
    std::int32_t      m_iOK{};  // int
    std::u16string    m_wstrIP{};  // std::wstring
    std::uint16_t     m_usPort{};  // unsigned short
    KAccountBlockInfo m_kAccountBlockInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_wstrIP)>>::put(w, m_wstrIP);
        Codec<std::remove_cvref_t<decltype(m_usPort)>>::put(w, m_usPort);
        Codec<std::remove_cvref_t<decltype(m_kAccountBlockInfo)>>::put(w, m_kAccountBlockInfo);
    }

    static KEGS_SELECT_SERVER_SET_ACK read_from(Reader& r) {
        KEGS_SELECT_SERVER_SET_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_wstrIP = Codec<std::remove_cvref_t<decltype(v.m_wstrIP)>>::get(r);
        v.m_usPort = Codec<std::remove_cvref_t<decltype(v.m_usPort)>>::get(r);
        v.m_kAccountBlockInfo = Codec<std::remove_cvref_t<decltype(v.m_kAccountBlockInfo)>>::get(r);
        return v;
    }
};

struct KEGS_SELECT_SERVER_SET_REQ {
    std::int32_t m_ServerSetID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_ServerSetID)>>::put(w, m_ServerSetID);
    }

    static KEGS_SELECT_SERVER_SET_REQ read_from(Reader& r) {
        KEGS_SELECT_SERVER_SET_REQ v;
        v.m_ServerSetID = Codec<std::remove_cvref_t<decltype(v.m_ServerSetID)>>::get(r);
        return v;
    }
};

struct KEGS_SELECT_UNIT_ACK {
    std::int32_t m_iOK{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
    }

    static KEGS_SELECT_UNIT_ACK read_from(Reader& r) {
        KEGS_SELECT_UNIT_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        return v;
    }
};

struct KEGS_SELECT_UNIT_REQ {
    std::int64_t           m_iUnitUID{};  // UidType
    std::set<std::int32_t> m_setCodeEventScriptID{};  // std::set< int >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_setCodeEventScriptID)>>::put(w, m_setCodeEventScriptID);
    }

    static KEGS_SELECT_UNIT_REQ read_from(Reader& r) {
        KEGS_SELECT_UNIT_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_setCodeEventScriptID = Codec<std::remove_cvref_t<decltype(v.m_setCodeEventScriptID)>>::get(r);
        return v;
    }
};

struct KEGS_SELL_ED_ITEM_REQ {
    std::int64_t m_iItemUID{};  // UidType
    std::int32_t m_iQuantity{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_iQuantity)>>::put(w, m_iQuantity);
    }

    static KEGS_SELL_ED_ITEM_REQ read_from(Reader& r) {
        KEGS_SELL_ED_ITEM_REQ v;
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_iQuantity = Codec<std::remove_cvref_t<decltype(v.m_iQuantity)>>::get(r);
        return v;
    }
};

struct KEGS_SEND_LETTER_REQ {
    bool           m_bIsGmMessage{};
    std::u16string m_wstrToNickName{};  // std::wstring
    std::u16string m_wstrTitle{};  // std::wstring
    std::u16string m_wstrMessage{};  // std::wstring
    std::int32_t   m_iED{};  // int
    std::int64_t   m_iItemUID{};  // UidType
    std::int32_t   m_iQuantity{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bIsGmMessage)>>::put(w, m_bIsGmMessage);
        Codec<std::remove_cvref_t<decltype(m_wstrToNickName)>>::put(w, m_wstrToNickName);
        Codec<std::remove_cvref_t<decltype(m_wstrTitle)>>::put(w, m_wstrTitle);
        Codec<std::remove_cvref_t<decltype(m_wstrMessage)>>::put(w, m_wstrMessage);
        Codec<std::remove_cvref_t<decltype(m_iED)>>::put(w, m_iED);
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_iQuantity)>>::put(w, m_iQuantity);
    }

    static KEGS_SEND_LETTER_REQ read_from(Reader& r) {
        KEGS_SEND_LETTER_REQ v;
        v.m_bIsGmMessage = Codec<std::remove_cvref_t<decltype(v.m_bIsGmMessage)>>::get(r);
        v.m_wstrToNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrToNickName)>>::get(r);
        v.m_wstrTitle = Codec<std::remove_cvref_t<decltype(v.m_wstrTitle)>>::get(r);
        v.m_wstrMessage = Codec<std::remove_cvref_t<decltype(v.m_wstrMessage)>>::get(r);
        v.m_iED = Codec<std::remove_cvref_t<decltype(v.m_iED)>>::get(r);
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_iQuantity = Codec<std::remove_cvref_t<decltype(v.m_iQuantity)>>::get(r);
        return v;
    }
};

struct KEGS_SERVER_CHECK_HACK_USER_ACK {
    char m_cHackingCheckType{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cHackingCheckType)>>::put(w, m_cHackingCheckType);
    }

    static KEGS_SERVER_CHECK_HACK_USER_ACK read_from(Reader& r) {
        KEGS_SERVER_CHECK_HACK_USER_ACK v;
        v.m_cHackingCheckType = Codec<std::remove_cvref_t<decltype(v.m_cHackingCheckType)>>::get(r);
        return v;
    }
};

struct KEGS_SERVER_CHECK_HACK_USER_REQ {
    char m_cHackingCheckType{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cHackingCheckType)>>::put(w, m_cHackingCheckType);
    }

    static KEGS_SERVER_CHECK_HACK_USER_REQ read_from(Reader& r) {
        KEGS_SERVER_CHECK_HACK_USER_REQ v;
        v.m_cHackingCheckType = Codec<std::remove_cvref_t<decltype(v.m_cHackingCheckType)>>::get(r);
        return v;
    }
};

struct KEGS_SET_AUTO_FEED_PETS_ACK {
    std::int32_t m_iOK{};  // int
    bool         m_bAutoFeed{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_bAutoFeed)>>::put(w, m_bAutoFeed);
    }

    static KEGS_SET_AUTO_FEED_PETS_ACK read_from(Reader& r) {
        KEGS_SET_AUTO_FEED_PETS_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_bAutoFeed = Codec<std::remove_cvref_t<decltype(v.m_bAutoFeed)>>::get(r);
        return v;
    }
};

struct KEGS_SET_AUTO_FEED_PETS_REQ {
    bool m_bAutoFeed{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bAutoFeed)>>::put(w, m_bAutoFeed);
    }

    static KEGS_SET_AUTO_FEED_PETS_REQ read_from(Reader& r) {
        KEGS_SET_AUTO_FEED_PETS_REQ v;
        v.m_bAutoFeed = Codec<std::remove_cvref_t<decltype(v.m_bAutoFeed)>>::get(r);
        return v;
    }
};

struct KEGS_SET_MULTIPLYER {
    float fM{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(fM)>>::put(w, fM);
    }

    static KEGS_SET_MULTIPLYER read_from(Reader& r) {
        KEGS_SET_MULTIPLYER v;
        v.fM = Codec<std::remove_cvref_t<decltype(v.fM)>>::get(r);
        return v;
    }
};

struct KEGS_SET_TC_REMAINING_TIME_REQ {
    bool m_bEnable{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bEnable)>>::put(w, m_bEnable);
    }

    static KEGS_SET_TC_REMAINING_TIME_REQ read_from(Reader& r) {
        KEGS_SET_TC_REMAINING_TIME_REQ v;
        v.m_bEnable = Codec<std::remove_cvref_t<decltype(v.m_bEnable)>>::get(r);
        return v;
    }
};

struct KEGS_SHARING_BACK_OPEN_NOT {
    std::int32_t m_iOK{};  // int
    std::int32_t m_iOpenType{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iOpenType)>>::put(w, m_iOpenType);
    }

    static KEGS_SHARING_BACK_OPEN_NOT read_from(Reader& r) {
        KEGS_SHARING_BACK_OPEN_NOT v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iOpenType = Codec<std::remove_cvref_t<decltype(v.m_iOpenType)>>::get(r);
        return v;
    }
};

struct KEGS_SKILL_SLOT_CHANGE_ITEM_NOT {
    char           m_cSkillSlotBExpirationState{};
    std::u16string m_wstrSkillSlotBEndDate{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cSkillSlotBExpirationState)>>::put(w, m_cSkillSlotBExpirationState);
        Codec<std::remove_cvref_t<decltype(m_wstrSkillSlotBEndDate)>>::put(w, m_wstrSkillSlotBEndDate);
    }

    static KEGS_SKILL_SLOT_CHANGE_ITEM_NOT read_from(Reader& r) {
        KEGS_SKILL_SLOT_CHANGE_ITEM_NOT v;
        v.m_cSkillSlotBExpirationState = Codec<std::remove_cvref_t<decltype(v.m_cSkillSlotBExpirationState)>>::get(r);
        v.m_wstrSkillSlotBEndDate = Codec<std::remove_cvref_t<decltype(v.m_wstrSkillSlotBEndDate)>>::get(r);
        return v;
    }
};

struct KEGS_SKILL_USE_REQ {
    std::int32_t m_iSkillID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iSkillID)>>::put(w, m_iSkillID);
    }

    static KEGS_SKILL_USE_REQ read_from(Reader& r) {
        KEGS_SKILL_USE_REQ v;
        v.m_iSkillID = Codec<std::remove_cvref_t<decltype(v.m_iSkillID)>>::get(r);
        return v;
    }
};

struct KEGS_SOCKET_EXPAND_ITEM_REQ {
    std::int64_t m_iMaterialItemUID{};  // UidType
    std::int64_t m_iTargetItemUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iMaterialItemUID)>>::put(w, m_iMaterialItemUID);
        Codec<std::remove_cvref_t<decltype(m_iTargetItemUID)>>::put(w, m_iTargetItemUID);
    }

    static KEGS_SOCKET_EXPAND_ITEM_REQ read_from(Reader& r) {
        KEGS_SOCKET_EXPAND_ITEM_REQ v;
        v.m_iMaterialItemUID = Codec<std::remove_cvref_t<decltype(v.m_iMaterialItemUID)>>::get(r);
        v.m_iTargetItemUID = Codec<std::remove_cvref_t<decltype(v.m_iTargetItemUID)>>::get(r);
        return v;
    }
};

struct KEGS_SOCKET_ITEM_REQ {
    std::int64_t                         m_iItemUID{};  // UidType
    std::map<std::int32_t, std::int64_t> m_mapSocketInfo{};  // std::map< int, UidType >
    bool                                 m_bCheat{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_mapSocketInfo)>>::put(w, m_mapSocketInfo);
        Codec<std::remove_cvref_t<decltype(m_bCheat)>>::put(w, m_bCheat);
    }

    static KEGS_SOCKET_ITEM_REQ read_from(Reader& r) {
        KEGS_SOCKET_ITEM_REQ v;
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_mapSocketInfo = Codec<std::remove_cvref_t<decltype(v.m_mapSocketInfo)>>::get(r);
        v.m_bCheat = Codec<std::remove_cvref_t<decltype(v.m_bCheat)>>::get(r);
        return v;
    }
};

struct KEGS_SORT_CATEGORY_ITEM_ACK {
    std::int32_t              m_iCategoryType{};  // int
    std::vector<std::int64_t> m_vecUpdatedInventorySlot{};  // std::vector< UidType >
    std::int32_t              m_iOK{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iCategoryType)>>::put(w, m_iCategoryType);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
    }

    static KEGS_SORT_CATEGORY_ITEM_ACK read_from(Reader& r) {
        KEGS_SORT_CATEGORY_ITEM_ACK v;
        v.m_iCategoryType = Codec<std::remove_cvref_t<decltype(v.m_iCategoryType)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        return v;
    }
};

struct KEGS_SORT_CATEGORY_ITEM_REQ {
    std::int32_t m_iCategoryType{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iCategoryType)>>::put(w, m_iCategoryType);
    }

    static KEGS_SORT_CATEGORY_ITEM_REQ read_from(Reader& r) {
        KEGS_SORT_CATEGORY_ITEM_REQ v;
        v.m_iCategoryType = Codec<std::remove_cvref_t<decltype(v.m_iCategoryType)>>::get(r);
        return v;
    }
};

struct KEGS_SQUARE_LIST_REQ {
    std::uint32_t m_nViewPage{};  // UINT
    char          m_cSquareType{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_nViewPage)>>::put(w, m_nViewPage);
        Codec<std::remove_cvref_t<decltype(m_cSquareType)>>::put(w, m_cSquareType);
    }

    static KEGS_SQUARE_LIST_REQ read_from(Reader& r) {
        KEGS_SQUARE_LIST_REQ v;
        v.m_nViewPage = Codec<std::remove_cvref_t<decltype(v.m_nViewPage)>>::get(r);
        v.m_cSquareType = Codec<std::remove_cvref_t<decltype(v.m_cSquareType)>>::get(r);
        return v;
    }
};

struct KEGS_START_CHECK_POWER_REQ {
    bool m_bStart{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bStart)>>::put(w, m_bStart);
    }

    static KEGS_START_CHECK_POWER_REQ read_from(Reader& r) {
        KEGS_START_CHECK_POWER_REQ v;
        v.m_bStart = Codec<std::remove_cvref_t<decltype(v.m_bStart)>>::get(r);
        return v;
    }
};

struct KEGS_START_WEDDING_ASK_ACK {
    bool m_bAgree{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bAgree)>>::put(w, m_bAgree);
    }

    static KEGS_START_WEDDING_ASK_ACK read_from(Reader& r) {
        KEGS_START_WEDDING_ASK_ACK v;
        v.m_bAgree = Codec<std::remove_cvref_t<decltype(v.m_bAgree)>>::get(r);
        return v;
    }
};

struct KEGS_STATE_CHANGE_FIELD_ACK {
    std::int32_t m_iOK{};  // int
    std::int32_t m_iLimitLevel{};  // int
    std::int32_t m_iRequireDungeonID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iLimitLevel)>>::put(w, m_iLimitLevel);
        Codec<std::remove_cvref_t<decltype(m_iRequireDungeonID)>>::put(w, m_iRequireDungeonID);
    }

    static KEGS_STATE_CHANGE_FIELD_ACK read_from(Reader& r) {
        KEGS_STATE_CHANGE_FIELD_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iLimitLevel = Codec<std::remove_cvref_t<decltype(v.m_iLimitLevel)>>::get(r);
        v.m_iRequireDungeonID = Codec<std::remove_cvref_t<decltype(v.m_iRequireDungeonID)>>::get(r);
        return v;
    }
};

struct KEGS_STATE_CHANGE_FIELD_REQ {
    std::int32_t m_iMapID{};  // int
    bool         m_bUseWarpItem{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iMapID)>>::put(w, m_iMapID);
        Codec<std::remove_cvref_t<decltype(m_bUseWarpItem)>>::put(w, m_bUseWarpItem);
    }

    static KEGS_STATE_CHANGE_FIELD_REQ read_from(Reader& r) {
        KEGS_STATE_CHANGE_FIELD_REQ v;
        v.m_iMapID = Codec<std::remove_cvref_t<decltype(v.m_iMapID)>>::get(r);
        v.m_bUseWarpItem = Codec<std::remove_cvref_t<decltype(v.m_bUseWarpItem)>>::get(r);
        return v;
    }
};

struct KEGS_STATE_CHANGE_GAME_START_REQ {
    std::int16_t m_sWorldID{};  // short
    std::int32_t m_iDungeonID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_sWorldID)>>::put(w, m_sWorldID);
        Codec<std::remove_cvref_t<decltype(m_iDungeonID)>>::put(w, m_iDungeonID);
    }

    static KEGS_STATE_CHANGE_GAME_START_REQ read_from(Reader& r) {
        KEGS_STATE_CHANGE_GAME_START_REQ v;
        v.m_sWorldID = Codec<std::remove_cvref_t<decltype(v.m_sWorldID)>>::get(r);
        v.m_iDungeonID = Codec<std::remove_cvref_t<decltype(v.m_iDungeonID)>>::get(r);
        return v;
    }
};

struct KEGS_STATE_CHANGE_RESULT_ACK {
    std::int32_t m_iOK{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
    }

    static KEGS_STATE_CHANGE_RESULT_ACK read_from(Reader& r) {
        KEGS_STATE_CHANGE_RESULT_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        return v;
    }
};

struct KEGS_STATE_CHANGE_RESULT_REQ {
    std::int32_t m_Dummy{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_Dummy)>>::put(w, m_Dummy);
    }

    static KEGS_STATE_CHANGE_RESULT_REQ read_from(Reader& r) {
        KEGS_STATE_CHANGE_RESULT_REQ v;
        v.m_Dummy = Codec<std::remove_cvref_t<decltype(v.m_Dummy)>>::get(r);
        return v;
    }
};

struct KEGS_STATE_CHANGE_SERVER_SELECT_ACK {
    std::int32_t m_iOK{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
    }

    static KEGS_STATE_CHANGE_SERVER_SELECT_ACK read_from(Reader& r) {
        KEGS_STATE_CHANGE_SERVER_SELECT_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        return v;
    }
};

struct KEGS_STOP_DUNGEON_CONTINUE_TIME_ACK {
    std::int32_t m_iOK{};  // int
    bool         m_bIsStop{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_bIsStop)>>::put(w, m_bIsStop);
    }

    static KEGS_STOP_DUNGEON_CONTINUE_TIME_ACK read_from(Reader& r) {
        KEGS_STOP_DUNGEON_CONTINUE_TIME_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_bIsStop = Codec<std::remove_cvref_t<decltype(v.m_bIsStop)>>::get(r);
        return v;
    }
};

struct KEGS_STOP_DUNGEON_CONTINUE_TIME_REQ {
    bool m_bIsStop{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bIsStop)>>::put(w, m_bIsStop);
    }

    static KEGS_STOP_DUNGEON_CONTINUE_TIME_REQ read_from(Reader& r) {
        KEGS_STOP_DUNGEON_CONTINUE_TIME_REQ v;
        v.m_bIsStop = Codec<std::remove_cvref_t<decltype(v.m_bIsStop)>>::get(r);
        return v;
    }
};

struct KEGS_SUMMON_PET_REQ {
    std::int64_t m_iSummonPetUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iSummonPetUID)>>::put(w, m_iSummonPetUID);
    }

    static KEGS_SUMMON_PET_REQ read_from(Reader& r) {
        KEGS_SUMMON_PET_REQ v;
        v.m_iSummonPetUID = Codec<std::remove_cvref_t<decltype(v.m_iSummonPetUID)>>::get(r);
        return v;
    }
};

struct KEGS_SUMMON_RIDING_PET_NOT {
    std::int64_t  m_iUnitUID{};  // UidType
    std::int64_t  m_iRidingPetUID{};  // UidType
    std::uint16_t m_usRidingPetID{};  // USHORT

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iRidingPetUID)>>::put(w, m_iRidingPetUID);
        Codec<std::remove_cvref_t<decltype(m_usRidingPetID)>>::put(w, m_usRidingPetID);
    }

    static KEGS_SUMMON_RIDING_PET_NOT read_from(Reader& r) {
        KEGS_SUMMON_RIDING_PET_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iRidingPetUID = Codec<std::remove_cvref_t<decltype(v.m_iRidingPetUID)>>::get(r);
        v.m_usRidingPetID = Codec<std::remove_cvref_t<decltype(v.m_usRidingPetID)>>::get(r);
        return v;
    }
};

struct KEGS_SUMMON_RIDING_PET_REQ {
    std::int64_t m_iRidingPetUID{};  // UidType
    bool         m_bNeedAck{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iRidingPetUID)>>::put(w, m_iRidingPetUID);
        Codec<std::remove_cvref_t<decltype(m_bNeedAck)>>::put(w, m_bNeedAck);
    }

    static KEGS_SUMMON_RIDING_PET_REQ read_from(Reader& r) {
        KEGS_SUMMON_RIDING_PET_REQ v;
        v.m_iRidingPetUID = Codec<std::remove_cvref_t<decltype(v.m_iRidingPetUID)>>::get(r);
        v.m_bNeedAck = Codec<std::remove_cvref_t<decltype(v.m_bNeedAck)>>::get(r);
        return v;
    }
};

struct KEGS_SYNC_BATTLE_FIELD_DAMAGED_MAP_REQ {
    std::int32_t m_iNPCUID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iNPCUID)>>::put(w, m_iNPCUID);
    }

    static KEGS_SYNC_BATTLE_FIELD_DAMAGED_MAP_REQ read_from(Reader& r) {
        KEGS_SYNC_BATTLE_FIELD_DAMAGED_MAP_REQ v;
        v.m_iNPCUID = Codec<std::remove_cvref_t<decltype(v.m_iNPCUID)>>::get(r);
        return v;
    }
};

struct KEGS_TALK_WITH_NPC_REQ {
    std::int32_t m_iNPCID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iNPCID)>>::put(w, m_iNPCID);
    }

    static KEGS_TALK_WITH_NPC_REQ read_from(Reader& r) {
        KEGS_TALK_WITH_NPC_REQ v;
        v.m_iNPCID = Codec<std::remove_cvref_t<decltype(v.m_iNPCID)>>::get(r);
        return v;
    }
};

struct KEGS_TITLE_EXPIRATION_NOT {
    std::int32_t m_iTitleID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iTitleID)>>::put(w, m_iTitleID);
    }

    static KEGS_TITLE_EXPIRATION_NOT read_from(Reader& r) {
        KEGS_TITLE_EXPIRATION_NOT v;
        v.m_iTitleID = Codec<std::remove_cvref_t<decltype(v.m_iTitleID)>>::get(r);
        return v;
    }
};

struct KEGS_TUTORIAL_STUDENT_LIST_REQ {
    std::int32_t m_iViewPage{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iViewPage)>>::put(w, m_iViewPage);
    }

    static KEGS_TUTORIAL_STUDENT_LIST_REQ read_from(Reader& r) {
        KEGS_TUTORIAL_STUDENT_LIST_REQ v;
        v.m_iViewPage = Codec<std::remove_cvref_t<decltype(v.m_iViewPage)>>::get(r);
        return v;
    }
};

struct KEGS_UDP_PING_SCORE_NOT {
    std::uint32_t m_dwPingScore{};  // DWORD

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_dwPingScore)>>::put(w, m_dwPingScore);
    }

    static KEGS_UDP_PING_SCORE_NOT read_from(Reader& r) {
        KEGS_UDP_PING_SCORE_NOT v;
        v.m_dwPingScore = Codec<std::remove_cvref_t<decltype(v.m_dwPingScore)>>::get(r);
        return v;
    }
};

struct KEGS_UNBLOCK_FRIEND_ACK {
    std::int32_t m_iOK{};  // int
    std::int64_t m_iUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
    }

    static KEGS_UNBLOCK_FRIEND_ACK read_from(Reader& r) {
        KEGS_UNBLOCK_FRIEND_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        return v;
    }
};

struct KEGS_UNBLOCK_FRIEND_NOT {
    std::int64_t m_iUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
    }

    static KEGS_UNBLOCK_FRIEND_NOT read_from(Reader& r) {
        KEGS_UNBLOCK_FRIEND_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        return v;
    }
};

struct KEGS_UNLIMITED_SECOND_CHANGE_JOB_NOT {
    std::int32_t m_iUnlimitedSecondChangeJob{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnlimitedSecondChangeJob)>>::put(w, m_iUnlimitedSecondChangeJob);
    }

    static KEGS_UNLIMITED_SECOND_CHANGE_JOB_NOT read_from(Reader& r) {
        KEGS_UNLIMITED_SECOND_CHANGE_JOB_NOT v;
        v.m_iUnlimitedSecondChangeJob = Codec<std::remove_cvref_t<decltype(v.m_iUnlimitedSecondChangeJob)>>::get(r);
        return v;
    }
};

struct KEGS_UNREG_AUTO_PARTY_WAIT_LIST_NOT {
    std::int32_t   m_iReason{};  // int
    std::u16string m_wstrCancelUnitNickName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iReason)>>::put(w, m_iReason);
        Codec<std::remove_cvref_t<decltype(m_wstrCancelUnitNickName)>>::put(w, m_wstrCancelUnitNickName);
    }

    static KEGS_UNREG_AUTO_PARTY_WAIT_LIST_NOT read_from(Reader& r) {
        KEGS_UNREG_AUTO_PARTY_WAIT_LIST_NOT v;
        v.m_iReason = Codec<std::remove_cvref_t<decltype(v.m_iReason)>>::get(r);
        v.m_wstrCancelUnitNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrCancelUnitNickName)>>::get(r);
        return v;
    }
};

struct KEGS_UNREG_MATCH_WAIT_LIST_NOT {
    std::int32_t   m_iReason{};  // int
    std::u16string m_wstrCancelUnitNickName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iReason)>>::put(w, m_iReason);
        Codec<std::remove_cvref_t<decltype(m_wstrCancelUnitNickName)>>::put(w, m_wstrCancelUnitNickName);
    }

    static KEGS_UNREG_MATCH_WAIT_LIST_NOT read_from(Reader& r) {
        KEGS_UNREG_MATCH_WAIT_LIST_NOT v;
        v.m_iReason = Codec<std::remove_cvref_t<decltype(v.m_iReason)>>::get(r);
        v.m_wstrCancelUnitNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrCancelUnitNickName)>>::get(r);
        return v;
    }
};

struct KEGS_UNSEAL_ITEM_REQ {
    std::int64_t m_iDestItemUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iDestItemUID)>>::put(w, m_iDestItemUID);
    }

    static KEGS_UNSEAL_ITEM_REQ read_from(Reader& r) {
        KEGS_UNSEAL_ITEM_REQ v;
        v.m_iDestItemUID = Codec<std::remove_cvref_t<decltype(v.m_iDestItemUID)>>::get(r);
        return v;
    }
};

struct KEGS_UNSEAL_SKILL_NOT {
    std::int32_t m_iSkillID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iSkillID)>>::put(w, m_iSkillID);
    }

    static KEGS_UNSEAL_SKILL_NOT read_from(Reader& r) {
        KEGS_UNSEAL_SKILL_NOT v;
        v.m_iSkillID = Codec<std::remove_cvref_t<decltype(v.m_iSkillID)>>::get(r);
        return v;
    }
};

struct KEGS_UNSUMMON_RIDING_PET_ACK {
    std::int32_t m_iOK{};  // int
    std::int64_t m_iRidingPetUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iRidingPetUID)>>::put(w, m_iRidingPetUID);
    }

    static KEGS_UNSUMMON_RIDING_PET_ACK read_from(Reader& r) {
        KEGS_UNSUMMON_RIDING_PET_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iRidingPetUID = Codec<std::remove_cvref_t<decltype(v.m_iRidingPetUID)>>::get(r);
        return v;
    }
};

struct KEGS_UNSUMMON_RIDING_PET_NOT {
    std::int64_t m_iUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
    }

    static KEGS_UNSUMMON_RIDING_PET_NOT read_from(Reader& r) {
        KEGS_UNSUMMON_RIDING_PET_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        return v;
    }
};

struct KEGS_UNSUMMON_RIDING_PET_REQ {
    std::int64_t m_iRidingPetUID{};  // UidType
    float        m_fStamina{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iRidingPetUID)>>::put(w, m_iRidingPetUID);
        Codec<std::remove_cvref_t<decltype(m_fStamina)>>::put(w, m_fStamina);
    }

    static KEGS_UNSUMMON_RIDING_PET_REQ read_from(Reader& r) {
        KEGS_UNSUMMON_RIDING_PET_REQ v;
        v.m_iRidingPetUID = Codec<std::remove_cvref_t<decltype(v.m_iRidingPetUID)>>::get(r);
        v.m_fStamina = Codec<std::remove_cvref_t<decltype(v.m_fStamina)>>::get(r);
        return v;
    }
};

struct KEGS_UPDATE_AUTO_PARTY_BONUS_INFO_NOT {
    std::map<std::int32_t, KAutoPartyDungeonBonusInfo> m_mapDungeonBonusList{};  // std::map< int, KAutoPartyDungeonBonusInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_mapDungeonBonusList)>>::put(w, m_mapDungeonBonusList);
    }

    static KEGS_UPDATE_AUTO_PARTY_BONUS_INFO_NOT read_from(Reader& r) {
        KEGS_UPDATE_AUTO_PARTY_BONUS_INFO_NOT v;
        v.m_mapDungeonBonusList = Codec<std::remove_cvref_t<decltype(v.m_mapDungeonBonusList)>>::get(r);
        return v;
    }
};

struct KEGS_UPDATE_BUFF_INFO_NOT {
    std::vector<KBuffInfo> m_vecActivateBuffList{};  // std::vector< KBuffInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecActivateBuffList)>>::put(w, m_vecActivateBuffList);
    }

    static KEGS_UPDATE_BUFF_INFO_NOT read_from(Reader& r) {
        KEGS_UPDATE_BUFF_INFO_NOT v;
        v.m_vecActivateBuffList = Codec<std::remove_cvref_t<decltype(v.m_vecActivateBuffList)>>::get(r);
        return v;
    }
};

struct KEGS_UPDATE_CASH_SKILL_POINT_NOT {
    std::int32_t   m_iOK{};  // int
    std::int64_t   m_iUnitUID{};  // UidType
    std::int32_t   m_iCSPoint{};  // int
    std::u16string m_wstrCSPointEndDate{};  // std::wstring
    bool           m_bUpdateEndDateOnly{};
    std::int32_t   m_iSkillPointItemID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iCSPoint)>>::put(w, m_iCSPoint);
        Codec<std::remove_cvref_t<decltype(m_wstrCSPointEndDate)>>::put(w, m_wstrCSPointEndDate);
        Codec<std::remove_cvref_t<decltype(m_bUpdateEndDateOnly)>>::put(w, m_bUpdateEndDateOnly);
        Codec<std::remove_cvref_t<decltype(m_iSkillPointItemID)>>::put(w, m_iSkillPointItemID);
    }

    static KEGS_UPDATE_CASH_SKILL_POINT_NOT read_from(Reader& r) {
        KEGS_UPDATE_CASH_SKILL_POINT_NOT v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iCSPoint = Codec<std::remove_cvref_t<decltype(v.m_iCSPoint)>>::get(r);
        v.m_wstrCSPointEndDate = Codec<std::remove_cvref_t<decltype(v.m_wstrCSPointEndDate)>>::get(r);
        v.m_bUpdateEndDateOnly = Codec<std::remove_cvref_t<decltype(v.m_bUpdateEndDateOnly)>>::get(r);
        v.m_iSkillPointItemID = Codec<std::remove_cvref_t<decltype(v.m_iSkillPointItemID)>>::get(r);
        return v;
    }
};

struct KEGS_UPDATE_CHECK_POWER_NOT {
    std::uint8_t m_ucCheckPowerCount{};  // unsigned char
    std::int64_t m_iCheckPowerTime{};  // __int64
    std::uint8_t m_ucCheckPowerScore{};  // unsigned char

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_ucCheckPowerCount)>>::put(w, m_ucCheckPowerCount);
        Codec<std::remove_cvref_t<decltype(m_iCheckPowerTime)>>::put(w, m_iCheckPowerTime);
        Codec<std::remove_cvref_t<decltype(m_ucCheckPowerScore)>>::put(w, m_ucCheckPowerScore);
    }

    static KEGS_UPDATE_CHECK_POWER_NOT read_from(Reader& r) {
        KEGS_UPDATE_CHECK_POWER_NOT v;
        v.m_ucCheckPowerCount = Codec<std::remove_cvref_t<decltype(v.m_ucCheckPowerCount)>>::get(r);
        v.m_iCheckPowerTime = Codec<std::remove_cvref_t<decltype(v.m_iCheckPowerTime)>>::get(r);
        v.m_ucCheckPowerScore = Codec<std::remove_cvref_t<decltype(v.m_ucCheckPowerScore)>>::get(r);
        return v;
    }
};

struct KEGS_UPDATE_CODE_EVENT_NOT {
    std::map<std::int32_t, bool> m_mapEnableCodeEnum{};  // std::map< int, bool >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_mapEnableCodeEnum)>>::put(w, m_mapEnableCodeEnum);
    }

    static KEGS_UPDATE_CODE_EVENT_NOT read_from(Reader& r) {
        KEGS_UPDATE_CODE_EVENT_NOT v;
        v.m_mapEnableCodeEnum = Codec<std::remove_cvref_t<decltype(v.m_mapEnableCodeEnum)>>::get(r);
        return v;
    }
};

struct KEGS_UPDATE_COMMUNITY_OPTION_ACK {
    std::int32_t m_iOK{};  // int
    KDenyOptions m_kDenyOptions{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_kDenyOptions)>>::put(w, m_kDenyOptions);
    }

    static KEGS_UPDATE_COMMUNITY_OPTION_ACK read_from(Reader& r) {
        KEGS_UPDATE_COMMUNITY_OPTION_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_kDenyOptions = Codec<std::remove_cvref_t<decltype(v.m_kDenyOptions)>>::get(r);
        return v;
    }
};

struct KEGS_UPDATE_COMMUNITY_OPTION_REQ {
    KDenyOptions m_kDenyOptions{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kDenyOptions)>>::put(w, m_kDenyOptions);
    }

    static KEGS_UPDATE_COMMUNITY_OPTION_REQ read_from(Reader& r) {
        KEGS_UPDATE_COMMUNITY_OPTION_REQ v;
        v.m_kDenyOptions = Codec<std::remove_cvref_t<decltype(v.m_kDenyOptions)>>::get(r);
        return v;
    }
};

struct KEGS_UPDATE_DUNGEON_STATUS_NOT {
    std::map<std::int32_t, bool> m_mapEnableDungeon{};  // std::map< int, bool >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_mapEnableDungeon)>>::put(w, m_mapEnableDungeon);
    }

    static KEGS_UPDATE_DUNGEON_STATUS_NOT read_from(Reader& r) {
        KEGS_UPDATE_DUNGEON_STATUS_NOT v;
        v.m_mapEnableDungeon = Codec<std::remove_cvref_t<decltype(v.m_mapEnableDungeon)>>::get(r);
        return v;
    }
};

struct KEGS_UPDATE_FRIEND_INFO_ACK {
    std::int32_t   m_iOK{};  // int
    std::u16string m_wstrFriendNickName{};  // std::wstring
    std::int64_t   m_iFriendUnitUID{};  // UidType
    std::int32_t   m_iChannelID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_wstrFriendNickName)>>::put(w, m_wstrFriendNickName);
        Codec<std::remove_cvref_t<decltype(m_iFriendUnitUID)>>::put(w, m_iFriendUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iChannelID)>>::put(w, m_iChannelID);
    }

    static KEGS_UPDATE_FRIEND_INFO_ACK read_from(Reader& r) {
        KEGS_UPDATE_FRIEND_INFO_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_wstrFriendNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrFriendNickName)>>::get(r);
        v.m_iFriendUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iFriendUnitUID)>>::get(r);
        v.m_iChannelID = Codec<std::remove_cvref_t<decltype(v.m_iChannelID)>>::get(r);
        return v;
    }
};

struct KEGS_UPDATE_FRIEND_INFO_REQ {
    bool           m_bDelete{};
    std::u16string m_wstrFriendNickName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bDelete)>>::put(w, m_bDelete);
        Codec<std::remove_cvref_t<decltype(m_wstrFriendNickName)>>::put(w, m_wstrFriendNickName);
    }

    static KEGS_UPDATE_FRIEND_INFO_REQ read_from(Reader& r) {
        KEGS_UPDATE_FRIEND_INFO_REQ v;
        v.m_bDelete = Codec<std::remove_cvref_t<decltype(v.m_bDelete)>>::get(r);
        v.m_wstrFriendNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrFriendNickName)>>::get(r);
        return v;
    }
};

struct KEGS_UPDATE_MAX_LEVEL_NOT {
    std::int32_t m_iMaxLevel{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iMaxLevel)>>::put(w, m_iMaxLevel);
    }

    static KEGS_UPDATE_MAX_LEVEL_NOT read_from(Reader& r) {
        KEGS_UPDATE_MAX_LEVEL_NOT v;
        v.m_iMaxLevel = Codec<std::remove_cvref_t<decltype(v.m_iMaxLevel)>>::get(r);
        return v;
    }
};

struct KEGS_UPDATE_MOVE_TO_PARTY_MEMBER_INFO_NOT {
    bool m_bIsOn{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bIsOn)>>::put(w, m_bIsOn);
    }

    static KEGS_UPDATE_MOVE_TO_PARTY_MEMBER_INFO_NOT read_from(Reader& r) {
        KEGS_UPDATE_MOVE_TO_PARTY_MEMBER_INFO_NOT v;
        v.m_bIsOn = Codec<std::remove_cvref_t<decltype(v.m_bIsOn)>>::get(r);
        return v;
    }
};

struct KEGS_UPDATE_PARTY_UID_NOT {
    std::int64_t m_iUnitUID{};  // UidType
    std::int64_t m_iPartyUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iPartyUID)>>::put(w, m_iPartyUID);
    }

    static KEGS_UPDATE_PARTY_UID_NOT read_from(Reader& r) {
        KEGS_UPDATE_PARTY_UID_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iPartyUID = Codec<std::remove_cvref_t<decltype(v.m_iPartyUID)>>::get(r);
        return v;
    }
};

struct KEGS_UPDATE_PARTY_USER_INFO_NOT {
    std::int64_t m_iUnitUID{};  // UidType
    std::uint8_t m_cState{};  // UCHAR
    std::int32_t m_iStateCode{};  // int
    char         m_cUnitClass{};
    std::uint8_t m_ucLevel{};  // UCHAR

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_cState)>>::put(w, m_cState);
        Codec<std::remove_cvref_t<decltype(m_iStateCode)>>::put(w, m_iStateCode);
        Codec<std::remove_cvref_t<decltype(m_cUnitClass)>>::put(w, m_cUnitClass);
        Codec<std::remove_cvref_t<decltype(m_ucLevel)>>::put(w, m_ucLevel);
    }

    static KEGS_UPDATE_PARTY_USER_INFO_NOT read_from(Reader& r) {
        KEGS_UPDATE_PARTY_USER_INFO_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_cState = Codec<std::remove_cvref_t<decltype(v.m_cState)>>::get(r);
        v.m_iStateCode = Codec<std::remove_cvref_t<decltype(v.m_iStateCode)>>::get(r);
        v.m_cUnitClass = Codec<std::remove_cvref_t<decltype(v.m_cUnitClass)>>::get(r);
        v.m_ucLevel = Codec<std::remove_cvref_t<decltype(v.m_ucLevel)>>::get(r);
        return v;
    }
};

struct KEGS_UPDATE_QUEST_ACK {
    std::int32_t m_iOK{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
    }

    static KEGS_UPDATE_QUEST_ACK read_from(Reader& r) {
        KEGS_UPDATE_QUEST_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        return v;
    }
};

struct KEGS_UPDATE_RIDING_PET_NOT {
    std::int64_t m_iRidingPetUID{};  // UidType
    float        m_fStamina{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iRidingPetUID)>>::put(w, m_iRidingPetUID);
        Codec<std::remove_cvref_t<decltype(m_fStamina)>>::put(w, m_fStamina);
    }

    static KEGS_UPDATE_RIDING_PET_NOT read_from(Reader& r) {
        KEGS_UPDATE_RIDING_PET_NOT v;
        v.m_iRidingPetUID = Codec<std::remove_cvref_t<decltype(v.m_iRidingPetUID)>>::get(r);
        v.m_fStamina = Codec<std::remove_cvref_t<decltype(v.m_fStamina)>>::get(r);
        return v;
    }
};

struct KEGS_UPDATE_SQUARE_PERSONAL_SHOP_INFO_NOT {
    std::int64_t   m_iUnitUID{};  // UidType
    char           m_cPersonalShopState{};
    char           m_cPersonalShopType{};
    std::u16string m_wstrPersonalShopName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_cPersonalShopState)>>::put(w, m_cPersonalShopState);
        Codec<std::remove_cvref_t<decltype(m_cPersonalShopType)>>::put(w, m_cPersonalShopType);
        Codec<std::remove_cvref_t<decltype(m_wstrPersonalShopName)>>::put(w, m_wstrPersonalShopName);
    }

    static KEGS_UPDATE_SQUARE_PERSONAL_SHOP_INFO_NOT read_from(Reader& r) {
        KEGS_UPDATE_SQUARE_PERSONAL_SHOP_INFO_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_cPersonalShopState = Codec<std::remove_cvref_t<decltype(v.m_cPersonalShopState)>>::get(r);
        v.m_cPersonalShopType = Codec<std::remove_cvref_t<decltype(v.m_cPersonalShopType)>>::get(r);
        v.m_wstrPersonalShopName = Codec<std::remove_cvref_t<decltype(v.m_wstrPersonalShopName)>>::get(r);
        return v;
    }
};

struct KEGS_UPDATE_TRADE_ED_NOT {
    std::int64_t                 m_iUnitUID{};  // UidType
    std::int32_t                 m_iED{};  // int
    std::map<std::int64_t, bool> m_mapOfferState{};  // std::map< UidType, bool >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iED)>>::put(w, m_iED);
        Codec<std::remove_cvref_t<decltype(m_mapOfferState)>>::put(w, m_mapOfferState);
    }

    static KEGS_UPDATE_TRADE_ED_NOT read_from(Reader& r) {
        KEGS_UPDATE_TRADE_ED_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iED = Codec<std::remove_cvref_t<decltype(v.m_iED)>>::get(r);
        v.m_mapOfferState = Codec<std::remove_cvref_t<decltype(v.m_mapOfferState)>>::get(r);
        return v;
    }
};

struct KEGS_UPDATE_TRADE_ED_REQ {
    std::int32_t m_iED{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iED)>>::put(w, m_iED);
    }

    static KEGS_UPDATE_TRADE_ED_REQ read_from(Reader& r) {
        KEGS_UPDATE_TRADE_ED_REQ v;
        v.m_iED = Codec<std::remove_cvref_t<decltype(v.m_iED)>>::get(r);
        return v;
    }
};

struct KEGS_UPDATE_TRADE_ITEM_REQ {
    std::int64_t m_iItemUID{};  // UidType
    std::int32_t m_iQuantity{};  // int
    bool         m_bDelete{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_iQuantity)>>::put(w, m_iQuantity);
        Codec<std::remove_cvref_t<decltype(m_bDelete)>>::put(w, m_bDelete);
    }

    static KEGS_UPDATE_TRADE_ITEM_REQ read_from(Reader& r) {
        KEGS_UPDATE_TRADE_ITEM_REQ v;
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_iQuantity = Codec<std::remove_cvref_t<decltype(v.m_iQuantity)>>::get(r);
        v.m_bDelete = Codec<std::remove_cvref_t<decltype(v.m_bDelete)>>::get(r);
        return v;
    }
};

struct KEGS_UPDATE_USER_UNIT_BUFF_INFO_BROAD_NOT {
    std::int64_t             m_iUnitUID{};  // UidType
    std::vector<KBuffFactor> m_vecBuffFactor{};  // std::vector< KBuffFactor >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_vecBuffFactor)>>::put(w, m_vecBuffFactor);
    }

    static KEGS_UPDATE_USER_UNIT_BUFF_INFO_BROAD_NOT read_from(Reader& r) {
        KEGS_UPDATE_USER_UNIT_BUFF_INFO_BROAD_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_vecBuffFactor = Codec<std::remove_cvref_t<decltype(v.m_vecBuffFactor)>>::get(r);
        return v;
    }
};

struct KEGS_UPDATE_USER_UNIT_BUFF_INFO_NOT {
    std::int64_t             m_iToUnitUID{};  // UidType
    std::vector<KBuffFactor> m_vecBuffFactor{};  // std::vector< KBuffFactor >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iToUnitUID)>>::put(w, m_iToUnitUID);
        Codec<std::remove_cvref_t<decltype(m_vecBuffFactor)>>::put(w, m_vecBuffFactor);
    }

    static KEGS_UPDATE_USER_UNIT_BUFF_INFO_NOT read_from(Reader& r) {
        KEGS_UPDATE_USER_UNIT_BUFF_INFO_NOT v;
        v.m_iToUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iToUnitUID)>>::get(r);
        v.m_vecBuffFactor = Codec<std::remove_cvref_t<decltype(v.m_vecBuffFactor)>>::get(r);
        return v;
    }
};

struct KEGS_UPDATE_WARP_VIP_NOT {
    std::int32_t m_iOK{};  // int
    std::int64_t m_trWarpVipEndDate{};  // __int64

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_trWarpVipEndDate)>>::put(w, m_trWarpVipEndDate);
    }

    static KEGS_UPDATE_WARP_VIP_NOT read_from(Reader& r) {
        KEGS_UPDATE_WARP_VIP_NOT v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_trWarpVipEndDate = Codec<std::remove_cvref_t<decltype(v.m_trWarpVipEndDate)>>::get(r);
        return v;
    }
};

struct KEGS_UPDATE_WORLD_BUFF_NOT {
    std::uint16_t m_usSuccessWorldBuff{};  // u_short
    std::uint16_t m_usWorldBuffDurationTime{};  // u_short

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_usSuccessWorldBuff)>>::put(w, m_usSuccessWorldBuff);
        Codec<std::remove_cvref_t<decltype(m_usWorldBuffDurationTime)>>::put(w, m_usWorldBuffDurationTime);
    }

    static KEGS_UPDATE_WORLD_BUFF_NOT read_from(Reader& r) {
        KEGS_UPDATE_WORLD_BUFF_NOT v;
        v.m_usSuccessWorldBuff = Codec<std::remove_cvref_t<decltype(v.m_usSuccessWorldBuff)>>::get(r);
        v.m_usWorldBuffDurationTime = Codec<std::remove_cvref_t<decltype(v.m_usWorldBuffDurationTime)>>::get(r);
        return v;
    }
};

struct KEGS_USER_COMMUNITY_SURVEY_NEW_ACK {
    std::int64_t unitUID{};  // UidType
    std::int32_t m_cCommunity_Tab_Type{};  // int
    std::int32_t m_iOK{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(unitUID)>>::put(w, unitUID);
        Codec<std::remove_cvref_t<decltype(m_cCommunity_Tab_Type)>>::put(w, m_cCommunity_Tab_Type);
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
    }

    static KEGS_USER_COMMUNITY_SURVEY_NEW_ACK read_from(Reader& r) {
        KEGS_USER_COMMUNITY_SURVEY_NEW_ACK v;
        v.unitUID = Codec<std::remove_cvref_t<decltype(v.unitUID)>>::get(r);
        v.m_cCommunity_Tab_Type = Codec<std::remove_cvref_t<decltype(v.m_cCommunity_Tab_Type)>>::get(r);
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        return v;
    }
};

struct KEGS_USER_COMMUNITY_SURVEY_NEW_REQ {
    std::u16string m_wUnitNickName{};  // std::wstring
    std::int32_t   m_cCommunity_Tab_Type{};  // int
    std::int32_t   m_iOK{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wUnitNickName)>>::put(w, m_wUnitNickName);
        Codec<std::remove_cvref_t<decltype(m_cCommunity_Tab_Type)>>::put(w, m_cCommunity_Tab_Type);
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
    }

    static KEGS_USER_COMMUNITY_SURVEY_NEW_REQ read_from(Reader& r) {
        KEGS_USER_COMMUNITY_SURVEY_NEW_REQ v;
        v.m_wUnitNickName = Codec<std::remove_cvref_t<decltype(v.m_wUnitNickName)>>::get(r);
        v.m_cCommunity_Tab_Type = Codec<std::remove_cvref_t<decltype(v.m_cCommunity_Tab_Type)>>::get(r);
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        return v;
    }
};

struct KEGS_USER_UNIT_DIE_ACK {
    float        m_fReBirthTime{};
    std::int32_t m_iOK{};  // int
    std::int16_t m_VP{};  // short
    std::int16_t m_EXP{};  // short

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_fReBirthTime)>>::put(w, m_fReBirthTime);
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_VP)>>::put(w, m_VP);
        Codec<std::remove_cvref_t<decltype(m_EXP)>>::put(w, m_EXP);
    }

    static KEGS_USER_UNIT_DIE_ACK read_from(Reader& r) {
        KEGS_USER_UNIT_DIE_ACK v;
        v.m_fReBirthTime = Codec<std::remove_cvref_t<decltype(v.m_fReBirthTime)>>::get(r);
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_VP = Codec<std::remove_cvref_t<decltype(v.m_VP)>>::get(r);
        v.m_EXP = Codec<std::remove_cvref_t<decltype(v.m_EXP)>>::get(r);
        return v;
    }
};

struct KEGS_USER_UNIT_DIE_COMPLETE_NOT {
    std::int64_t m_UnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_UnitUID)>>::put(w, m_UnitUID);
    }

    static KEGS_USER_UNIT_DIE_COMPLETE_NOT read_from(Reader& r) {
        KEGS_USER_UNIT_DIE_COMPLETE_NOT v;
        v.m_UnitUID = Codec<std::remove_cvref_t<decltype(v.m_UnitUID)>>::get(r);
        return v;
    }
};

struct KEGS_USER_UNIT_DIE_NOT {
    std::int32_t m_KillerNPCUID{};  // int
    std::int64_t m_KillerUserUnitUID{};  // UidType
    std::int64_t m_MaxDamageKillerUserUnitUID{};  // UidType
    std::int64_t m_KilledUserUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_KillerNPCUID)>>::put(w, m_KillerNPCUID);
        Codec<std::remove_cvref_t<decltype(m_KillerUserUnitUID)>>::put(w, m_KillerUserUnitUID);
        Codec<std::remove_cvref_t<decltype(m_MaxDamageKillerUserUnitUID)>>::put(w, m_MaxDamageKillerUserUnitUID);
        Codec<std::remove_cvref_t<decltype(m_KilledUserUnitUID)>>::put(w, m_KilledUserUnitUID);
    }

    static KEGS_USER_UNIT_DIE_NOT read_from(Reader& r) {
        KEGS_USER_UNIT_DIE_NOT v;
        v.m_KillerNPCUID = Codec<std::remove_cvref_t<decltype(v.m_KillerNPCUID)>>::get(r);
        v.m_KillerUserUnitUID = Codec<std::remove_cvref_t<decltype(v.m_KillerUserUnitUID)>>::get(r);
        v.m_MaxDamageKillerUserUnitUID = Codec<std::remove_cvref_t<decltype(v.m_MaxDamageKillerUserUnitUID)>>::get(r);
        v.m_KilledUserUnitUID = Codec<std::remove_cvref_t<decltype(v.m_KilledUserUnitUID)>>::get(r);
        return v;
    }
};

struct KEGS_USER_UNIT_DIE_REQ {
    std::int32_t m_KillerNPCUID{};  // int
    std::int64_t m_KillerUserUnitUID{};  // UidType
    std::int64_t m_MaxDamageKillerUserUnitUID{};  // UidType
    std::uint8_t m_ucDieReason{};  // unsigned char

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_KillerNPCUID)>>::put(w, m_KillerNPCUID);
        Codec<std::remove_cvref_t<decltype(m_KillerUserUnitUID)>>::put(w, m_KillerUserUnitUID);
        Codec<std::remove_cvref_t<decltype(m_MaxDamageKillerUserUnitUID)>>::put(w, m_MaxDamageKillerUserUnitUID);
        Codec<std::remove_cvref_t<decltype(m_ucDieReason)>>::put(w, m_ucDieReason);
    }

    static KEGS_USER_UNIT_DIE_REQ read_from(Reader& r) {
        KEGS_USER_UNIT_DIE_REQ v;
        v.m_KillerNPCUID = Codec<std::remove_cvref_t<decltype(v.m_KillerNPCUID)>>::get(r);
        v.m_KillerUserUnitUID = Codec<std::remove_cvref_t<decltype(v.m_KillerUserUnitUID)>>::get(r);
        v.m_MaxDamageKillerUserUnitUID = Codec<std::remove_cvref_t<decltype(v.m_MaxDamageKillerUserUnitUID)>>::get(r);
        v.m_ucDieReason = Codec<std::remove_cvref_t<decltype(v.m_ucDieReason)>>::get(r);
        return v;
    }
};

struct KEGS_USER_UNIT_RE_BIRTH_NOT {
    std::int64_t m_UserUnitUID{};  // UidType
    std::int32_t m_StartPosIndex{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_UserUnitUID)>>::put(w, m_UserUnitUID);
        Codec<std::remove_cvref_t<decltype(m_StartPosIndex)>>::put(w, m_StartPosIndex);
    }

    static KEGS_USER_UNIT_RE_BIRTH_NOT read_from(Reader& r) {
        KEGS_USER_UNIT_RE_BIRTH_NOT v;
        v.m_UserUnitUID = Codec<std::remove_cvref_t<decltype(v.m_UserUnitUID)>>::get(r);
        v.m_StartPosIndex = Codec<std::remove_cvref_t<decltype(v.m_StartPosIndex)>>::get(r);
        return v;
    }
};

struct KEGS_USER_UNIT_RE_BIRTH_POS_REQ {
    std::int32_t m_StartPosIndex{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_StartPosIndex)>>::put(w, m_StartPosIndex);
    }

    static KEGS_USER_UNIT_RE_BIRTH_POS_REQ read_from(Reader& r) {
        KEGS_USER_UNIT_RE_BIRTH_POS_REQ v;
        v.m_StartPosIndex = Codec<std::remove_cvref_t<decltype(v.m_StartPosIndex)>>::get(r);
        return v;
    }
};

struct KEGS_USE_DEFENCE_DUNGEON_QUICK_SLOT_REQ {
    std::int16_t m_sSlotID{};  // short

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_sSlotID)>>::put(w, m_sSlotID);
    }

    static KEGS_USE_DEFENCE_DUNGEON_QUICK_SLOT_REQ read_from(Reader& r) {
        KEGS_USE_DEFENCE_DUNGEON_QUICK_SLOT_REQ v;
        v.m_sSlotID = Codec<std::remove_cvref_t<decltype(v.m_sSlotID)>>::get(r);
        return v;
    }
};

struct KEGS_USE_FINALITY_SKILL_REQ {
    std::int64_t m_iItemUID{};  // UidType
    bool         m_bNoConsume{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_bNoConsume)>>::put(w, m_bNoConsume);
    }

    static KEGS_USE_FINALITY_SKILL_REQ read_from(Reader& r) {
        KEGS_USE_FINALITY_SKILL_REQ v;
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_bNoConsume = Codec<std::remove_cvref_t<decltype(v.m_bNoConsume)>>::get(r);
        return v;
    }
};

struct KEGS_USE_ITEM_IN_INVENTORY_REQ {
    std::int64_t m_iItemUID{};  // UidType
    std::int64_t m_iTempCode{};  // __int64

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_iTempCode)>>::put(w, m_iTempCode);
    }

    static KEGS_USE_ITEM_IN_INVENTORY_REQ read_from(Reader& r) {
        KEGS_USE_ITEM_IN_INVENTORY_REQ v;
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_iTempCode = Codec<std::remove_cvref_t<decltype(v.m_iTempCode)>>::get(r);
        return v;
    }
};

struct KEGS_USE_MEGAPHONE_REQ {
    std::int64_t   m_iItemUID{};  // UidType
    std::u16string m_wstrMessage{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_wstrMessage)>>::put(w, m_wstrMessage);
    }

    static KEGS_USE_MEGAPHONE_REQ read_from(Reader& r) {
        KEGS_USE_MEGAPHONE_REQ v;
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_wstrMessage = Codec<std::remove_cvref_t<decltype(v.m_wstrMessage)>>::get(r);
        return v;
    }
};

struct KEGS_USE_QUICK_SLOT_NOT {
    std::int64_t m_UnitUID{};  // UidType
    std::int32_t m_ItemID{};  // int
    bool         m_bUseItemNeedPayment{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_UnitUID)>>::put(w, m_UnitUID);
        Codec<std::remove_cvref_t<decltype(m_ItemID)>>::put(w, m_ItemID);
        Codec<std::remove_cvref_t<decltype(m_bUseItemNeedPayment)>>::put(w, m_bUseItemNeedPayment);
    }

    static KEGS_USE_QUICK_SLOT_NOT read_from(Reader& r) {
        KEGS_USE_QUICK_SLOT_NOT v;
        v.m_UnitUID = Codec<std::remove_cvref_t<decltype(v.m_UnitUID)>>::get(r);
        v.m_ItemID = Codec<std::remove_cvref_t<decltype(v.m_ItemID)>>::get(r);
        v.m_bUseItemNeedPayment = Codec<std::remove_cvref_t<decltype(v.m_bUseItemNeedPayment)>>::get(r);
        return v;
    }
};

struct KEGS_USE_QUICK_SLOT_REQ {
    std::int16_t m_sSlotID{};  // short

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_sSlotID)>>::put(w, m_sSlotID);
    }

    static KEGS_USE_QUICK_SLOT_REQ read_from(Reader& r) {
        KEGS_USE_QUICK_SLOT_REQ v;
        v.m_sSlotID = Codec<std::remove_cvref_t<decltype(v.m_sSlotID)>>::get(r);
        return v;
    }
};

struct KEGS_USE_SKILL_BUFF_IN_PLAY_BROAD_NOT {
    std::int64_t m_iUnitUID{};  // UidType
    std::int32_t m_iSkillID{};  // int
    bool         m_bOn{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iSkillID)>>::put(w, m_iSkillID);
        Codec<std::remove_cvref_t<decltype(m_bOn)>>::put(w, m_bOn);
    }

    static KEGS_USE_SKILL_BUFF_IN_PLAY_BROAD_NOT read_from(Reader& r) {
        KEGS_USE_SKILL_BUFF_IN_PLAY_BROAD_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iSkillID = Codec<std::remove_cvref_t<decltype(v.m_iSkillID)>>::get(r);
        v.m_bOn = Codec<std::remove_cvref_t<decltype(v.m_bOn)>>::get(r);
        return v;
    }
};

struct KEGS_USE_SKILL_BUFF_IN_PLAY_NOT {
    std::int32_t m_iSkillID{};  // int
    bool         m_bOn{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iSkillID)>>::put(w, m_iSkillID);
        Codec<std::remove_cvref_t<decltype(m_bOn)>>::put(w, m_bOn);
    }

    static KEGS_USE_SKILL_BUFF_IN_PLAY_NOT read_from(Reader& r) {
        KEGS_USE_SKILL_BUFF_IN_PLAY_NOT v;
        v.m_iSkillID = Codec<std::remove_cvref_t<decltype(v.m_iSkillID)>>::get(r);
        v.m_bOn = Codec<std::remove_cvref_t<decltype(v.m_bOn)>>::get(r);
        return v;
    }
};

struct KEGS_VERIFY_ACCOUNT_ACK {
    std::int32_t   m_iOK{};  // int
    KAccountInfo   m_kAccountInfo{};
    char           m_cHackingUserType{};
    std::u16string m_wstrAdURL{};  // std::wstring
    bool           m_bSex{};
    std::uint32_t  m_uiAge{};  // UINT32
    std::uint32_t  m_uiNexonSN{};  // UINT32
    std::u16string m_wstrClientIP{};  // std::wstring
    std::uint8_t   m_uChannelCode{};  // UINT8
    std::u16string m_wstrChannelUserID{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_kAccountInfo)>>::put(w, m_kAccountInfo);
        Codec<std::remove_cvref_t<decltype(m_cHackingUserType)>>::put(w, m_cHackingUserType);
        Codec<std::remove_cvref_t<decltype(m_wstrAdURL)>>::put(w, m_wstrAdURL);
        Codec<std::remove_cvref_t<decltype(m_bSex)>>::put(w, m_bSex);
        Codec<std::remove_cvref_t<decltype(m_uiAge)>>::put(w, m_uiAge);
        Codec<std::remove_cvref_t<decltype(m_uiNexonSN)>>::put(w, m_uiNexonSN);
        Codec<std::remove_cvref_t<decltype(m_wstrClientIP)>>::put(w, m_wstrClientIP);
        Codec<std::remove_cvref_t<decltype(m_uChannelCode)>>::put(w, m_uChannelCode);
        Codec<std::remove_cvref_t<decltype(m_wstrChannelUserID)>>::put(w, m_wstrChannelUserID);
    }

    static KEGS_VERIFY_ACCOUNT_ACK read_from(Reader& r) {
        KEGS_VERIFY_ACCOUNT_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_kAccountInfo = Codec<std::remove_cvref_t<decltype(v.m_kAccountInfo)>>::get(r);
        v.m_cHackingUserType = Codec<std::remove_cvref_t<decltype(v.m_cHackingUserType)>>::get(r);
        v.m_wstrAdURL = Codec<std::remove_cvref_t<decltype(v.m_wstrAdURL)>>::get(r);
        v.m_bSex = Codec<std::remove_cvref_t<decltype(v.m_bSex)>>::get(r);
        v.m_uiAge = Codec<std::remove_cvref_t<decltype(v.m_uiAge)>>::get(r);
        v.m_uiNexonSN = Codec<std::remove_cvref_t<decltype(v.m_uiNexonSN)>>::get(r);
        v.m_wstrClientIP = Codec<std::remove_cvref_t<decltype(v.m_wstrClientIP)>>::get(r);
        v.m_uChannelCode = Codec<std::remove_cvref_t<decltype(v.m_uChannelCode)>>::get(r);
        v.m_wstrChannelUserID = Codec<std::remove_cvref_t<decltype(v.m_wstrChannelUserID)>>::get(r);
        return v;
    }
};

struct KEGS_VISIT_CASH_SHOP_NOT {
    bool         m_bEnterCashShop{};
    std::int64_t m_iUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bEnterCashShop)>>::put(w, m_bEnterCashShop);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
    }

    static KEGS_VISIT_CASH_SHOP_NOT read_from(Reader& r) {
        KEGS_VISIT_CASH_SHOP_NOT v;
        v.m_bEnterCashShop = Codec<std::remove_cvref_t<decltype(v.m_bEnterCashShop)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        return v;
    }
};

struct KEGS_WARP_BY_BUTTON_ACK {
    std::int32_t m_iOK{};  // int
    std::int32_t m_iED{};  // int
    std::int32_t m_iWarpPointMapID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iED)>>::put(w, m_iED);
        Codec<std::remove_cvref_t<decltype(m_iWarpPointMapID)>>::put(w, m_iWarpPointMapID);
    }

    static KEGS_WARP_BY_BUTTON_ACK read_from(Reader& r) {
        KEGS_WARP_BY_BUTTON_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iED = Codec<std::remove_cvref_t<decltype(v.m_iED)>>::get(r);
        v.m_iWarpPointMapID = Codec<std::remove_cvref_t<decltype(v.m_iWarpPointMapID)>>::get(r);
        return v;
    }
};

struct KEGS_WARP_BY_BUTTON_REQ {
    std::int32_t m_iCurrentVillageWarpIndex{};  // int
    bool         m_bFreeWarp{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iCurrentVillageWarpIndex)>>::put(w, m_iCurrentVillageWarpIndex);
        Codec<std::remove_cvref_t<decltype(m_bFreeWarp)>>::put(w, m_bFreeWarp);
    }

    static KEGS_WARP_BY_BUTTON_REQ read_from(Reader& r) {
        KEGS_WARP_BY_BUTTON_REQ v;
        v.m_iCurrentVillageWarpIndex = Codec<std::remove_cvref_t<decltype(v.m_iCurrentVillageWarpIndex)>>::get(r);
        v.m_bFreeWarp = Codec<std::remove_cvref_t<decltype(v.m_bFreeWarp)>>::get(r);
        return v;
    }
};

struct KEGS_WATCH_UNIT_REQ {
    std::int64_t m_iUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
    }

    static KEGS_WATCH_UNIT_REQ read_from(Reader& r) {
        KEGS_WATCH_UNIT_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        return v;
    }
};

struct KEGS_WEDDING_ITEM_INFO_REQ {
    std::vector<std::int64_t> m_vecWeddingItemList{};  // std::vector< UidType >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecWeddingItemList)>>::put(w, m_vecWeddingItemList);
    }

    static KEGS_WEDDING_ITEM_INFO_REQ read_from(Reader& r) {
        KEGS_WEDDING_ITEM_INFO_REQ v;
        v.m_vecWeddingItemList = Codec<std::remove_cvref_t<decltype(v.m_vecWeddingItemList)>>::get(r);
        return v;
    }
};

struct KEGS_WEDDING_PROPOSE_REQ {
    char                      m_cWeddingHallType{};
    char                      m_cOfficiantNPC{};
    std::int64_t              m_tWeddingDate{};  // __int64
    std::u16string            m_wstrWeddingMsg{};  // std::wstring
    std::vector<std::int64_t> m_vecInviteUnitList{};  // std::vector<UidType>

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cWeddingHallType)>>::put(w, m_cWeddingHallType);
        Codec<std::remove_cvref_t<decltype(m_cOfficiantNPC)>>::put(w, m_cOfficiantNPC);
        Codec<std::remove_cvref_t<decltype(m_tWeddingDate)>>::put(w, m_tWeddingDate);
        Codec<std::remove_cvref_t<decltype(m_wstrWeddingMsg)>>::put(w, m_wstrWeddingMsg);
        Codec<std::remove_cvref_t<decltype(m_vecInviteUnitList)>>::put(w, m_vecInviteUnitList);
    }

    static KEGS_WEDDING_PROPOSE_REQ read_from(Reader& r) {
        KEGS_WEDDING_PROPOSE_REQ v;
        v.m_cWeddingHallType = Codec<std::remove_cvref_t<decltype(v.m_cWeddingHallType)>>::get(r);
        v.m_cOfficiantNPC = Codec<std::remove_cvref_t<decltype(v.m_cOfficiantNPC)>>::get(r);
        v.m_tWeddingDate = Codec<std::remove_cvref_t<decltype(v.m_tWeddingDate)>>::get(r);
        v.m_wstrWeddingMsg = Codec<std::remove_cvref_t<decltype(v.m_wstrWeddingMsg)>>::get(r);
        v.m_vecInviteUnitList = Codec<std::remove_cvref_t<decltype(v.m_vecInviteUnitList)>>::get(r);
        return v;
    }
};

struct KEGS_WORLD_MISSION_UPDATE_NOT {
    bool         m_bActive{};
    std::int32_t m_iRemainTime{};  // int
    std::int32_t m_iWorldCrystalCount{};  // int
    std::int32_t m_iWorldMaxCrystalCount{};  // int
    std::int32_t m_iMissionStartRemainTime{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bActive)>>::put(w, m_bActive);
        Codec<std::remove_cvref_t<decltype(m_iRemainTime)>>::put(w, m_iRemainTime);
        Codec<std::remove_cvref_t<decltype(m_iWorldCrystalCount)>>::put(w, m_iWorldCrystalCount);
        Codec<std::remove_cvref_t<decltype(m_iWorldMaxCrystalCount)>>::put(w, m_iWorldMaxCrystalCount);
        Codec<std::remove_cvref_t<decltype(m_iMissionStartRemainTime)>>::put(w, m_iMissionStartRemainTime);
    }

    static KEGS_WORLD_MISSION_UPDATE_NOT read_from(Reader& r) {
        KEGS_WORLD_MISSION_UPDATE_NOT v;
        v.m_bActive = Codec<std::remove_cvref_t<decltype(v.m_bActive)>>::get(r);
        v.m_iRemainTime = Codec<std::remove_cvref_t<decltype(v.m_iRemainTime)>>::get(r);
        v.m_iWorldCrystalCount = Codec<std::remove_cvref_t<decltype(v.m_iWorldCrystalCount)>>::get(r);
        v.m_iWorldMaxCrystalCount = Codec<std::remove_cvref_t<decltype(v.m_iWorldMaxCrystalCount)>>::get(r);
        v.m_iMissionStartRemainTime = Codec<std::remove_cvref_t<decltype(v.m_iMissionStartRemainTime)>>::get(r);
        return v;
    }
};

struct KEGS_WORLD_TRIGGER_RELOCATION_REQ {
    std::int32_t m_iWorldTrigger{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iWorldTrigger)>>::put(w, m_iWorldTrigger);
    }

    static KEGS_WORLD_TRIGGER_RELOCATION_REQ read_from(Reader& r) {
        KEGS_WORLD_TRIGGER_RELOCATION_REQ v;
        v.m_iWorldTrigger = Codec<std::remove_cvref_t<decltype(v.m_iWorldTrigger)>>::get(r);
        return v;
    }
};

struct KELG_ACCEPT_FRIEND_NOT {
    std::int64_t m_iDestUnitUID{};  // UidType
    std::int64_t m_iUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iDestUnitUID)>>::put(w, m_iDestUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
    }

    static KELG_ACCEPT_FRIEND_NOT read_from(Reader& r) {
        KELG_ACCEPT_FRIEND_NOT v;
        v.m_iDestUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iDestUnitUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        return v;
    }
};

struct KELG_ACCEPT_JOIN_GUILD_REQ {
    std::int32_t m_iGuildUID{};  // int
    std::int64_t m_iUnitUID{};  // UidType
    std::int64_t m_iApplyJoinUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iGuildUID)>>::put(w, m_iGuildUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iApplyJoinUnitUID)>>::put(w, m_iApplyJoinUnitUID);
    }

    static KELG_ACCEPT_JOIN_GUILD_REQ read_from(Reader& r) {
        KELG_ACCEPT_JOIN_GUILD_REQ v;
        v.m_iGuildUID = Codec<std::remove_cvref_t<decltype(v.m_iGuildUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iApplyJoinUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iApplyJoinUnitUID)>>::get(r);
        return v;
    }
};

struct KELG_ADMIN_DELETE_GUILD_ADD_NOT {
    std::int32_t m_iGuildUID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iGuildUID)>>::put(w, m_iGuildUID);
    }

    static KELG_ADMIN_DELETE_GUILD_ADD_NOT read_from(Reader& r) {
        KELG_ADMIN_DELETE_GUILD_ADD_NOT v;
        v.m_iGuildUID = Codec<std::remove_cvref_t<decltype(v.m_iGuildUID)>>::get(r);
        return v;
    }
};

struct KELG_ADMIN_DELETE_HENIR_RANKING_NICKNAME_NOT {
    std::int32_t   m_iRankingType{};  // int
    std::u16string m_wstrNickName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iRankingType)>>::put(w, m_iRankingType);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
    }

    static KELG_ADMIN_DELETE_HENIR_RANKING_NICKNAME_NOT read_from(Reader& r) {
        KELG_ADMIN_DELETE_HENIR_RANKING_NICKNAME_NOT v;
        v.m_iRankingType = Codec<std::remove_cvref_t<decltype(v.m_iRankingType)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        return v;
    }
};

struct KELG_ADMIN_DELETE_HENIR_RANKING_RANK_NOT {
    std::int32_t m_iRankingType{};  // int
    std::int32_t m_iRank{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iRankingType)>>::put(w, m_iRankingType);
        Codec<std::remove_cvref_t<decltype(m_iRank)>>::put(w, m_iRank);
    }

    static KELG_ADMIN_DELETE_HENIR_RANKING_RANK_NOT read_from(Reader& r) {
        KELG_ADMIN_DELETE_HENIR_RANKING_RANK_NOT v;
        v.m_iRankingType = Codec<std::remove_cvref_t<decltype(v.m_iRankingType)>>::get(r);
        v.m_iRank = Codec<std::remove_cvref_t<decltype(v.m_iRank)>>::get(r);
        return v;
    }
};

struct KELG_ADMIN_DELETE_HENIR_RANKING_UNITUID_NOT {
    std::int32_t m_iRankingType{};  // int
    std::int64_t m_iUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iRankingType)>>::put(w, m_iRankingType);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
    }

    static KELG_ADMIN_DELETE_HENIR_RANKING_UNITUID_NOT read_from(Reader& r) {
        KELG_ADMIN_DELETE_HENIR_RANKING_UNITUID_NOT v;
        v.m_iRankingType = Codec<std::remove_cvref_t<decltype(v.m_iRankingType)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        return v;
    }
};

struct KELG_ADMIN_DELETE_RECORD_NOT {
    std::int32_t   m_iRankingType{};  // int
    std::u16string m_wstrNickName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iRankingType)>>::put(w, m_iRankingType);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
    }

    static KELG_ADMIN_DELETE_RECORD_NOT read_from(Reader& r) {
        KELG_ADMIN_DELETE_RECORD_NOT v;
        v.m_iRankingType = Codec<std::remove_cvref_t<decltype(v.m_iRankingType)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        return v;
    }
};

struct KELG_ADMIN_GET_GUILD_SKILL_POINT_ACK {
    std::int32_t m_iOK{};  // int
    std::int32_t m_iGuildUID{};  // int
    std::int64_t m_iUnitUID{};  // UidType
    std::int32_t m_iGuildSPoint{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iGuildUID)>>::put(w, m_iGuildUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iGuildSPoint)>>::put(w, m_iGuildSPoint);
    }

    static KELG_ADMIN_GET_GUILD_SKILL_POINT_ACK read_from(Reader& r) {
        KELG_ADMIN_GET_GUILD_SKILL_POINT_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iGuildUID = Codec<std::remove_cvref_t<decltype(v.m_iGuildUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iGuildSPoint = Codec<std::remove_cvref_t<decltype(v.m_iGuildSPoint)>>::get(r);
        return v;
    }
};

struct KELG_ADMIN_GET_GUILD_SKILL_POINT_REQ {
    std::int32_t m_iGuildUID{};  // int
    std::int64_t m_iUnitUID{};  // UidType
    std::int32_t m_iGuildSPoint{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iGuildUID)>>::put(w, m_iGuildUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iGuildSPoint)>>::put(w, m_iGuildSPoint);
    }

    static KELG_ADMIN_GET_GUILD_SKILL_POINT_REQ read_from(Reader& r) {
        KELG_ADMIN_GET_GUILD_SKILL_POINT_REQ v;
        v.m_iGuildUID = Codec<std::remove_cvref_t<decltype(v.m_iGuildUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iGuildSPoint = Codec<std::remove_cvref_t<decltype(v.m_iGuildSPoint)>>::get(r);
        return v;
    }
};

struct KELG_ADMIN_SHOW_ED_CHEAT_ACK {
    std::int32_t   m_iOK{};  // int
    std::int64_t   m_iSenderUserUID{};  // UidType
    std::u16string m_wstrNickName{};  // std::wstring
    std::int32_t   m_iED{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iSenderUserUID)>>::put(w, m_iSenderUserUID);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_iED)>>::put(w, m_iED);
    }

    static KELG_ADMIN_SHOW_ED_CHEAT_ACK read_from(Reader& r) {
        KELG_ADMIN_SHOW_ED_CHEAT_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iSenderUserUID = Codec<std::remove_cvref_t<decltype(v.m_iSenderUserUID)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_iED = Codec<std::remove_cvref_t<decltype(v.m_iED)>>::get(r);
        return v;
    }
};

struct KELG_ADMIN_SHOW_ED_CHEAT_REQ {
    std::int64_t m_iSenderUserUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iSenderUserUID)>>::put(w, m_iSenderUserUID);
    }

    static KELG_ADMIN_SHOW_ED_CHEAT_REQ read_from(Reader& r) {
        KELG_ADMIN_SHOW_ED_CHEAT_REQ v;
        v.m_iSenderUserUID = Codec<std::remove_cvref_t<decltype(v.m_iSenderUserUID)>>::get(r);
        return v;
    }
};

struct KELG_APPLY_JOIN_GUILD_REQ {
    KApplyJoinGuildInfo m_kApplyJoinGuildInfo{};
    bool                m_bOldApplyDelete{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kApplyJoinGuildInfo)>>::put(w, m_kApplyJoinGuildInfo);
        Codec<std::remove_cvref_t<decltype(m_bOldApplyDelete)>>::put(w, m_bOldApplyDelete);
    }

    static KELG_APPLY_JOIN_GUILD_REQ read_from(Reader& r) {
        KELG_APPLY_JOIN_GUILD_REQ v;
        v.m_kApplyJoinGuildInfo = Codec<std::remove_cvref_t<decltype(v.m_kApplyJoinGuildInfo)>>::get(r);
        v.m_bOldApplyDelete = Codec<std::remove_cvref_t<decltype(v.m_bOldApplyDelete)>>::get(r);
        return v;
    }
};

struct KELG_BLOCK_FRIEND_NOT {
    std::int64_t m_iDestUnitUID{};  // UidType
    std::int64_t m_iUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iDestUnitUID)>>::put(w, m_iDestUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
    }

    static KELG_BLOCK_FRIEND_NOT read_from(Reader& r) {
        KELG_BLOCK_FRIEND_NOT v;
        v.m_iDestUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iDestUnitUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        return v;
    }
};

struct KELG_BREAK_UP_NOT {
    std::int64_t m_iLoverUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iLoverUnitUID)>>::put(w, m_iLoverUnitUID);
    }

    static KELG_BREAK_UP_NOT read_from(Reader& r) {
        KELG_BREAK_UP_NOT v;
        v.m_iLoverUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iLoverUnitUID)>>::get(r);
        return v;
    }
};

struct KELG_CALL_MY_LOVER_JOIN_BATTLE_FIELD_FAILD_NOT {
    std::int32_t m_iOK{};  // int
    std::int64_t m_iCalledUserUID{};  // UidType
    std::int64_t m_iCalledGSUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iCalledUserUID)>>::put(w, m_iCalledUserUID);
        Codec<std::remove_cvref_t<decltype(m_iCalledGSUID)>>::put(w, m_iCalledGSUID);
    }

    static KELG_CALL_MY_LOVER_JOIN_BATTLE_FIELD_FAILD_NOT read_from(Reader& r) {
        KELG_CALL_MY_LOVER_JOIN_BATTLE_FIELD_FAILD_NOT v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iCalledUserUID = Codec<std::remove_cvref_t<decltype(v.m_iCalledUserUID)>>::get(r);
        v.m_iCalledGSUID = Codec<std::remove_cvref_t<decltype(v.m_iCalledGSUID)>>::get(r);
        return v;
    }
};

struct KELG_CHANGE_GUILD_NAME_RESULT_REQ {
    std::int32_t   m_iGuildUID{};  // int
    std::u16string m_wstrGuildName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iGuildUID)>>::put(w, m_iGuildUID);
        Codec<std::remove_cvref_t<decltype(m_wstrGuildName)>>::put(w, m_wstrGuildName);
    }

    static KELG_CHANGE_GUILD_NAME_RESULT_REQ read_from(Reader& r) {
        KELG_CHANGE_GUILD_NAME_RESULT_REQ v;
        v.m_iGuildUID = Codec<std::remove_cvref_t<decltype(v.m_iGuildUID)>>::get(r);
        v.m_wstrGuildName = Codec<std::remove_cvref_t<decltype(v.m_wstrGuildName)>>::get(r);
        return v;
    }
};

struct KELG_CHANGE_GUILD_NAME_TIME_CHECK_ACK {
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

    static KELG_CHANGE_GUILD_NAME_TIME_CHECK_ACK read_from(Reader& r) {
        KELG_CHANGE_GUILD_NAME_TIME_CHECK_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_ulOrderNo = Codec<std::remove_cvref_t<decltype(v.m_ulOrderNo)>>::get(r);
        v.m_ulProductNo = Codec<std::remove_cvref_t<decltype(v.m_ulProductNo)>>::get(r);
        v.m_iQuantity = Codec<std::remove_cvref_t<decltype(v.m_iQuantity)>>::get(r);
        return v;
    }
};

struct KELG_CHANGE_GUILD_NAME_TIME_CHECK_REQ {
    std::int64_t  m_iUnitUID{};  // UidType
    std::uint32_t m_ulOrderNo{};  // unsigned long
    std::uint32_t m_ulProductNo{};  // unsigned long
    std::int32_t  m_iQuantity{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_ulOrderNo)>>::put(w, m_ulOrderNo);
        Codec<std::remove_cvref_t<decltype(m_ulProductNo)>>::put(w, m_ulProductNo);
        Codec<std::remove_cvref_t<decltype(m_iQuantity)>>::put(w, m_iQuantity);
    }

    static KELG_CHANGE_GUILD_NAME_TIME_CHECK_REQ read_from(Reader& r) {
        KELG_CHANGE_GUILD_NAME_TIME_CHECK_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_ulOrderNo = Codec<std::remove_cvref_t<decltype(v.m_ulOrderNo)>>::get(r);
        v.m_ulProductNo = Codec<std::remove_cvref_t<decltype(v.m_ulProductNo)>>::get(r);
        v.m_iQuantity = Codec<std::remove_cvref_t<decltype(v.m_iQuantity)>>::get(r);
        return v;
    }
};

struct KELG_CHANGE_LOVE_WORD_NOT {
    std::int64_t   m_iUseUnitUID{};  // UidType
    std::int64_t   m_iLoverUnitUID{};  // UidType
    std::u16string m_wstrNewLoveWord{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUseUnitUID)>>::put(w, m_iUseUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iLoverUnitUID)>>::put(w, m_iLoverUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrNewLoveWord)>>::put(w, m_wstrNewLoveWord);
    }

    static KELG_CHANGE_LOVE_WORD_NOT read_from(Reader& r) {
        KELG_CHANGE_LOVE_WORD_NOT v;
        v.m_iUseUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUseUnitUID)>>::get(r);
        v.m_iLoverUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iLoverUnitUID)>>::get(r);
        v.m_wstrNewLoveWord = Codec<std::remove_cvref_t<decltype(v.m_wstrNewLoveWord)>>::get(r);
        return v;
    }
};

struct KELG_CHAT_NOT {
    char           m_cChatPacketType{};
    std::int64_t   m_iSenderUnitUID{};  // UidType
    std::u16string m_wstrSenderUnitNickName{};  // std::wstring
    std::int64_t   m_iReceiverUnitUID{};  // UidType
    std::u16string m_wstrRecieverUnitNickName{};  // std::wstring
    std::u16string m_wstrMsg{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cChatPacketType)>>::put(w, m_cChatPacketType);
        Codec<std::remove_cvref_t<decltype(m_iSenderUnitUID)>>::put(w, m_iSenderUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrSenderUnitNickName)>>::put(w, m_wstrSenderUnitNickName);
        Codec<std::remove_cvref_t<decltype(m_iReceiverUnitUID)>>::put(w, m_iReceiverUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrRecieverUnitNickName)>>::put(w, m_wstrRecieverUnitNickName);
        Codec<std::remove_cvref_t<decltype(m_wstrMsg)>>::put(w, m_wstrMsg);
    }

    static KELG_CHAT_NOT read_from(Reader& r) {
        KELG_CHAT_NOT v;
        v.m_cChatPacketType = Codec<std::remove_cvref_t<decltype(v.m_cChatPacketType)>>::get(r);
        v.m_iSenderUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iSenderUnitUID)>>::get(r);
        v.m_wstrSenderUnitNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrSenderUnitNickName)>>::get(r);
        v.m_iReceiverUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iReceiverUnitUID)>>::get(r);
        v.m_wstrRecieverUnitNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrRecieverUnitNickName)>>::get(r);
        v.m_wstrMsg = Codec<std::remove_cvref_t<decltype(v.m_wstrMsg)>>::get(r);
        return v;
    }
};

struct KELG_CHAT_REQ {
    char           m_cChatPacketType{};
    std::int32_t   m_iChatData{};  // int
    std::int64_t   m_iSenderUnitUID{};  // UidType
    std::u16string m_wstrSenderUnitNickName{};  // std::wstring
    std::u16string m_wstrRecieverUnitNickName{};  // std::wstring
    std::u16string m_wstrMsg{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cChatPacketType)>>::put(w, m_cChatPacketType);
        Codec<std::remove_cvref_t<decltype(m_iChatData)>>::put(w, m_iChatData);
        Codec<std::remove_cvref_t<decltype(m_iSenderUnitUID)>>::put(w, m_iSenderUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrSenderUnitNickName)>>::put(w, m_wstrSenderUnitNickName);
        Codec<std::remove_cvref_t<decltype(m_wstrRecieverUnitNickName)>>::put(w, m_wstrRecieverUnitNickName);
        Codec<std::remove_cvref_t<decltype(m_wstrMsg)>>::put(w, m_wstrMsg);
    }

    static KELG_CHAT_REQ read_from(Reader& r) {
        KELG_CHAT_REQ v;
        v.m_cChatPacketType = Codec<std::remove_cvref_t<decltype(v.m_cChatPacketType)>>::get(r);
        v.m_iChatData = Codec<std::remove_cvref_t<decltype(v.m_iChatData)>>::get(r);
        v.m_iSenderUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iSenderUnitUID)>>::get(r);
        v.m_wstrSenderUnitNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrSenderUnitNickName)>>::get(r);
        v.m_wstrRecieverUnitNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrRecieverUnitNickName)>>::get(r);
        v.m_wstrMsg = Codec<std::remove_cvref_t<decltype(v.m_wstrMsg)>>::get(r);
        return v;
    }
};

struct KELG_CHECK_EXPAND_GUILD_MAX_MEMBER_ACK {
    std::int32_t m_iOK{};  // int
    std::int64_t m_iItemUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
    }

    static KELG_CHECK_EXPAND_GUILD_MAX_MEMBER_ACK read_from(Reader& r) {
        KELG_CHECK_EXPAND_GUILD_MAX_MEMBER_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        return v;
    }
};

struct KELG_CHECK_EXPAND_GUILD_MAX_MEMBER_REQ {
    std::int64_t m_iUnitUID{};  // UidType
    std::int32_t m_iGuildUID{};  // int
    std::int64_t m_iItemUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iGuildUID)>>::put(w, m_iGuildUID);
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
    }

    static KELG_CHECK_EXPAND_GUILD_MAX_MEMBER_REQ read_from(Reader& r) {
        KELG_CHECK_EXPAND_GUILD_MAX_MEMBER_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iGuildUID = Codec<std::remove_cvref_t<decltype(v.m_iGuildUID)>>::get(r);
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        return v;
    }
};

struct KELG_CHECK_TIME_RANDOM_ITEM_EVENT_ACK {
    std::int64_t m_iItemUID{};  // UidType
    std::int32_t m_iEventItemID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_iEventItemID)>>::put(w, m_iEventItemID);
    }

    static KELG_CHECK_TIME_RANDOM_ITEM_EVENT_ACK read_from(Reader& r) {
        KELG_CHECK_TIME_RANDOM_ITEM_EVENT_ACK v;
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_iEventItemID = Codec<std::remove_cvref_t<decltype(v.m_iEventItemID)>>::get(r);
        return v;
    }
};

struct KELG_CHECK_TIME_RANDOM_ITEM_EVENT_REQ {
    std::int64_t m_iItemUID{};  // UidType
    std::int64_t m_iUserUID{};  // UidType
    std::int64_t m_iUnitUID{};  // UidType
    std::int32_t m_iRandomItemID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iRandomItemID)>>::put(w, m_iRandomItemID);
    }

    static KELG_CHECK_TIME_RANDOM_ITEM_EVENT_REQ read_from(Reader& r) {
        KELG_CHECK_TIME_RANDOM_ITEM_EVENT_REQ v;
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iRandomItemID = Codec<std::remove_cvref_t<decltype(v.m_iRandomItemID)>>::get(r);
        return v;
    }
};

struct KELG_CHECK_WEDDING_HALL_INFO_REQ {
    std::int64_t m_iGSUID{};  // UidType
    std::int64_t m_iGSUserUID{};  // UidType
    std::int32_t m_iWeddingUID{};  // int
    char         m_cUnitWeddingStatus{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iGSUID)>>::put(w, m_iGSUID);
        Codec<std::remove_cvref_t<decltype(m_iGSUserUID)>>::put(w, m_iGSUserUID);
        Codec<std::remove_cvref_t<decltype(m_iWeddingUID)>>::put(w, m_iWeddingUID);
        Codec<std::remove_cvref_t<decltype(m_cUnitWeddingStatus)>>::put(w, m_cUnitWeddingStatus);
    }

    static KELG_CHECK_WEDDING_HALL_INFO_REQ read_from(Reader& r) {
        KELG_CHECK_WEDDING_HALL_INFO_REQ v;
        v.m_iGSUID = Codec<std::remove_cvref_t<decltype(v.m_iGSUID)>>::get(r);
        v.m_iGSUserUID = Codec<std::remove_cvref_t<decltype(v.m_iGSUserUID)>>::get(r);
        v.m_iWeddingUID = Codec<std::remove_cvref_t<decltype(v.m_iWeddingUID)>>::get(r);
        v.m_cUnitWeddingStatus = Codec<std::remove_cvref_t<decltype(v.m_cUnitWeddingStatus)>>::get(r);
        return v;
    }
};

struct KELG_COUPLE_PROPOSE_CHECK_CONNECT_USER_ACK {
    std::int32_t   m_iOK{};  // int
    std::u16string m_wstrNickName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
    }

    static KELG_COUPLE_PROPOSE_CHECK_CONNECT_USER_ACK read_from(Reader& r) {
        KELG_COUPLE_PROPOSE_CHECK_CONNECT_USER_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        return v;
    }
};

struct KELG_COUPLE_PROPOSE_CHECK_CONNECT_USER_REQ {
    std::int64_t   m_iOtherUserUID{};  // UidType
    std::int64_t   m_iOtherUnitUID{};  // UidType
    std::u16string m_wstrRequestUnitName{};  // std::wstring
    std::int64_t   m_iRequestUserUID{};  // UidType
    std::int64_t   m_iRequestUnitUID{};  // UidType
    std::u16string m_wstrAcceptUnitName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOtherUserUID)>>::put(w, m_iOtherUserUID);
        Codec<std::remove_cvref_t<decltype(m_iOtherUnitUID)>>::put(w, m_iOtherUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrRequestUnitName)>>::put(w, m_wstrRequestUnitName);
        Codec<std::remove_cvref_t<decltype(m_iRequestUserUID)>>::put(w, m_iRequestUserUID);
        Codec<std::remove_cvref_t<decltype(m_iRequestUnitUID)>>::put(w, m_iRequestUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrAcceptUnitName)>>::put(w, m_wstrAcceptUnitName);
    }

    static KELG_COUPLE_PROPOSE_CHECK_CONNECT_USER_REQ read_from(Reader& r) {
        KELG_COUPLE_PROPOSE_CHECK_CONNECT_USER_REQ v;
        v.m_iOtherUserUID = Codec<std::remove_cvref_t<decltype(v.m_iOtherUserUID)>>::get(r);
        v.m_iOtherUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iOtherUnitUID)>>::get(r);
        v.m_wstrRequestUnitName = Codec<std::remove_cvref_t<decltype(v.m_wstrRequestUnitName)>>::get(r);
        v.m_iRequestUserUID = Codec<std::remove_cvref_t<decltype(v.m_iRequestUserUID)>>::get(r);
        v.m_iRequestUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iRequestUnitUID)>>::get(r);
        v.m_wstrAcceptUnitName = Codec<std::remove_cvref_t<decltype(v.m_wstrAcceptUnitName)>>::get(r);
        return v;
    }
};

struct KELG_COUPLE_PROPOSE_NOT {
    std::u16string m_wstrRequestUnitName{};  // std::wstring
    std::int64_t   m_iRequestUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrRequestUnitName)>>::put(w, m_wstrRequestUnitName);
        Codec<std::remove_cvref_t<decltype(m_iRequestUnitUID)>>::put(w, m_iRequestUnitUID);
    }

    static KELG_COUPLE_PROPOSE_NOT read_from(Reader& r) {
        KELG_COUPLE_PROPOSE_NOT v;
        v.m_wstrRequestUnitName = Codec<std::remove_cvref_t<decltype(v.m_wstrRequestUnitName)>>::get(r);
        v.m_iRequestUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iRequestUnitUID)>>::get(r);
        return v;
    }
};

struct KELG_DELETE_APPLY_JOIN_GUILD_REQ {
    std::int32_t m_iGuildUID{};  // int
    std::int64_t m_iUnitUID{};  // UidType
    std::int64_t m_iDeletedUnitUID{};  // UidType
    bool         m_bDeleteChar{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iGuildUID)>>::put(w, m_iGuildUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iDeletedUnitUID)>>::put(w, m_iDeletedUnitUID);
        Codec<std::remove_cvref_t<decltype(m_bDeleteChar)>>::put(w, m_bDeleteChar);
    }

    static KELG_DELETE_APPLY_JOIN_GUILD_REQ read_from(Reader& r) {
        KELG_DELETE_APPLY_JOIN_GUILD_REQ v;
        v.m_iGuildUID = Codec<std::remove_cvref_t<decltype(v.m_iGuildUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iDeletedUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iDeletedUnitUID)>>::get(r);
        v.m_bDeleteChar = Codec<std::remove_cvref_t<decltype(v.m_bDeleteChar)>>::get(r);
        return v;
    }
};

struct KELG_DELETE_FRIEND_NOT {
    std::int64_t m_iDestUnitUID{};  // UidType
    std::int64_t m_iUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iDestUnitUID)>>::put(w, m_iDestUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
    }

    static KELG_DELETE_FRIEND_NOT read_from(Reader& r) {
        KELG_DELETE_FRIEND_NOT v;
        v.m_iDestUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iDestUnitUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        return v;
    }
};

struct KELG_DEL_TUTORIAL_NOT {
    std::int64_t   m_iReceiverUnitUID{};  // UidType
    std::int64_t   m_iDeleteUnitUID{};  // UidType
    std::u16string m_wstrReason{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iReceiverUnitUID)>>::put(w, m_iReceiverUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iDeleteUnitUID)>>::put(w, m_iDeleteUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrReason)>>::put(w, m_wstrReason);
    }

    static KELG_DEL_TUTORIAL_NOT read_from(Reader& r) {
        KELG_DEL_TUTORIAL_NOT v;
        v.m_iReceiverUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iReceiverUnitUID)>>::get(r);
        v.m_iDeleteUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iDeleteUnitUID)>>::get(r);
        v.m_wstrReason = Codec<std::remove_cvref_t<decltype(v.m_wstrReason)>>::get(r);
        return v;
    }
};

struct KELG_DENY_FRIEND_NOT {
    std::int64_t m_iDestUnitUID{};  // UidType
    std::int64_t m_iUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iDestUnitUID)>>::put(w, m_iDestUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
    }

    static KELG_DENY_FRIEND_NOT read_from(Reader& r) {
        KELG_DENY_FRIEND_NOT v;
        v.m_iDestUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iDestUnitUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        return v;
    }
};

struct KELG_EXCHANGE_CASH_ACK {
    std::int32_t m_iOK{};  // int
    std::int32_t m_iPoint{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iPoint)>>::put(w, m_iPoint);
    }

    static KELG_EXCHANGE_CASH_ACK read_from(Reader& r) {
        KELG_EXCHANGE_CASH_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iPoint = Codec<std::remove_cvref_t<decltype(v.m_iPoint)>>::get(r);
        return v;
    }
};

struct KELG_FIND_DELETE_CHAR_IN_RANKING_NOT {
    std::int64_t              m_iUnitUID{};  // UidType
    std::vector<std::int32_t> m_vecRankingType{};  // std::vector< int >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_vecRankingType)>>::put(w, m_vecRankingType);
    }

    static KELG_FIND_DELETE_CHAR_IN_RANKING_NOT read_from(Reader& r) {
        KELG_FIND_DELETE_CHAR_IN_RANKING_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_vecRankingType = Codec<std::remove_cvref_t<decltype(v.m_vecRankingType)>>::get(r);
        return v;
    }
};

struct KELG_FRIEND_POSITION_UPDATE_NOT {
    std::int64_t           m_iUnitUID{};  // UidType
    std::set<std::int64_t> m_setFriendUID{};  // std::set< UidType >
    char                   m_cPosition{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_setFriendUID)>>::put(w, m_setFriendUID);
        Codec<std::remove_cvref_t<decltype(m_cPosition)>>::put(w, m_cPosition);
    }

    static KELG_FRIEND_POSITION_UPDATE_NOT read_from(Reader& r) {
        KELG_FRIEND_POSITION_UPDATE_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_setFriendUID = Codec<std::remove_cvref_t<decltype(v.m_setFriendUID)>>::get(r);
        v.m_cPosition = Codec<std::remove_cvref_t<decltype(v.m_cPosition)>>::get(r);
        return v;
    }
};

struct KELG_GET_APPLY_JOIN_GUILD_LIST_REQ {
    std::int32_t  m_iGuildUID{};  // int
    std::int64_t  m_iUnitUID{};  // UidType
    std::uint32_t m_uiViewPage{};  // u_int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iGuildUID)>>::put(w, m_iGuildUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_uiViewPage)>>::put(w, m_uiViewPage);
    }

    static KELG_GET_APPLY_JOIN_GUILD_LIST_REQ read_from(Reader& r) {
        KELG_GET_APPLY_JOIN_GUILD_LIST_REQ v;
        v.m_iGuildUID = Codec<std::remove_cvref_t<decltype(v.m_iGuildUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_uiViewPage = Codec<std::remove_cvref_t<decltype(v.m_uiViewPage)>>::get(r);
        return v;
    }
};

struct KELG_GET_GUILD_AD_LIST_REQ {
    std::int32_t  m_iGuildUID{};  // int
    std::int64_t  m_iUnitUID{};  // UidType
    char          m_cSortType{};
    std::uint32_t m_uiViewPage{};  // u_int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iGuildUID)>>::put(w, m_iGuildUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_cSortType)>>::put(w, m_cSortType);
        Codec<std::remove_cvref_t<decltype(m_uiViewPage)>>::put(w, m_uiViewPage);
    }

    static KELG_GET_GUILD_AD_LIST_REQ read_from(Reader& r) {
        KELG_GET_GUILD_AD_LIST_REQ v;
        v.m_iGuildUID = Codec<std::remove_cvref_t<decltype(v.m_iGuildUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_cSortType = Codec<std::remove_cvref_t<decltype(v.m_cSortType)>>::get(r);
        v.m_uiViewPage = Codec<std::remove_cvref_t<decltype(v.m_uiViewPage)>>::get(r);
        return v;
    }
};

struct KELG_GET_GUILD_SKILL_IN_BOARD_REQ {
    std::int32_t m_iGuildUID{};  // int
    std::int64_t m_iUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iGuildUID)>>::put(w, m_iGuildUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
    }

    static KELG_GET_GUILD_SKILL_IN_BOARD_REQ read_from(Reader& r) {
        KELG_GET_GUILD_SKILL_IN_BOARD_REQ v;
        v.m_iGuildUID = Codec<std::remove_cvref_t<decltype(v.m_iGuildUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        return v;
    }
};

struct KELG_GET_GUILD_SKILL_REQ {
    std::int32_t m_iGuildUID{};  // int
    std::int64_t m_iUnitUID{};  // UidType
    std::int32_t m_iGuildSkillID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iGuildUID)>>::put(w, m_iGuildUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iGuildSkillID)>>::put(w, m_iGuildSkillID);
    }

    static KELG_GET_GUILD_SKILL_REQ read_from(Reader& r) {
        KELG_GET_GUILD_SKILL_REQ v;
        v.m_iGuildUID = Codec<std::remove_cvref_t<decltype(v.m_iGuildUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iGuildSkillID = Codec<std::remove_cvref_t<decltype(v.m_iGuildSkillID)>>::get(r);
        return v;
    }
};

struct KELG_GET_GUILD_USER_LIST_REQ {
    std::int32_t  m_iGuildUID{};  // int
    std::uint32_t m_uiViewPage{};  // UINT
    char          m_cGuildUserListType{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iGuildUID)>>::put(w, m_iGuildUID);
        Codec<std::remove_cvref_t<decltype(m_uiViewPage)>>::put(w, m_uiViewPage);
        Codec<std::remove_cvref_t<decltype(m_cGuildUserListType)>>::put(w, m_cGuildUserListType);
    }

    static KELG_GET_GUILD_USER_LIST_REQ read_from(Reader& r) {
        KELG_GET_GUILD_USER_LIST_REQ v;
        v.m_iGuildUID = Codec<std::remove_cvref_t<decltype(v.m_iGuildUID)>>::get(r);
        v.m_uiViewPage = Codec<std::remove_cvref_t<decltype(v.m_uiViewPage)>>::get(r);
        v.m_cGuildUserListType = Codec<std::remove_cvref_t<decltype(v.m_cGuildUserListType)>>::get(r);
        return v;
    }
};

struct KELG_GET_RANKING_FOR_GS_RANKING_MANAGER_REQ {
    std::int64_t m_iServerUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iServerUID)>>::put(w, m_iServerUID);
    }

    static KELG_GET_RANKING_FOR_GS_RANKING_MANAGER_REQ read_from(Reader& r) {
        KELG_GET_RANKING_FOR_GS_RANKING_MANAGER_REQ v;
        v.m_iServerUID = Codec<std::remove_cvref_t<decltype(v.m_iServerUID)>>::get(r);
        return v;
    }
};

struct KELG_GET_UNIT_INFO_ACK {
    std::uint16_t       m_usEventID{};  // unsigned short
    std::int64_t        m_iDemandUserUID{};  // UidType
    std::int64_t        m_iDemandGSUID{};  // UidType
    std::int32_t        m_iOK{};  // int
    KConnectionUnitInfo m_kCUnitInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_usEventID)>>::put(w, m_usEventID);
        Codec<std::remove_cvref_t<decltype(m_iDemandUserUID)>>::put(w, m_iDemandUserUID);
        Codec<std::remove_cvref_t<decltype(m_iDemandGSUID)>>::put(w, m_iDemandGSUID);
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_kCUnitInfo)>>::put(w, m_kCUnitInfo);
    }

    static KELG_GET_UNIT_INFO_ACK read_from(Reader& r) {
        KELG_GET_UNIT_INFO_ACK v;
        v.m_usEventID = Codec<std::remove_cvref_t<decltype(v.m_usEventID)>>::get(r);
        v.m_iDemandUserUID = Codec<std::remove_cvref_t<decltype(v.m_iDemandUserUID)>>::get(r);
        v.m_iDemandGSUID = Codec<std::remove_cvref_t<decltype(v.m_iDemandGSUID)>>::get(r);
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_kCUnitInfo = Codec<std::remove_cvref_t<decltype(v.m_kCUnitInfo)>>::get(r);
        return v;
    }
};

struct KELG_GET_UNIT_INFO_REQ {
    std::uint16_t  m_usEventID{};  // unsigned short
    std::int64_t   m_iDemandUserUID{};  // UidType
    std::int64_t   m_iDemandGSUID{};  // UidType
    std::int32_t   m_nKNMSN{};  // int
    std::u16string m_wstrNickName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_usEventID)>>::put(w, m_usEventID);
        Codec<std::remove_cvref_t<decltype(m_iDemandUserUID)>>::put(w, m_iDemandUserUID);
        Codec<std::remove_cvref_t<decltype(m_iDemandGSUID)>>::put(w, m_iDemandGSUID);
        Codec<std::remove_cvref_t<decltype(m_nKNMSN)>>::put(w, m_nKNMSN);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
    }

    static KELG_GET_UNIT_INFO_REQ read_from(Reader& r) {
        KELG_GET_UNIT_INFO_REQ v;
        v.m_usEventID = Codec<std::remove_cvref_t<decltype(v.m_usEventID)>>::get(r);
        v.m_iDemandUserUID = Codec<std::remove_cvref_t<decltype(v.m_iDemandUserUID)>>::get(r);
        v.m_iDemandGSUID = Codec<std::remove_cvref_t<decltype(v.m_iDemandGSUID)>>::get(r);
        v.m_nKNMSN = Codec<std::remove_cvref_t<decltype(v.m_nKNMSN)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        return v;
    }
};

struct KELG_GET_WEB_RANKING_REFRESH_NOT {
    std::int64_t  m_iServerUID{};  // UidType
    char          m_cRankingRefreshType{};
    std::uint32_t m_uiLastIndex{};  // u_int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iServerUID)>>::put(w, m_iServerUID);
        Codec<std::remove_cvref_t<decltype(m_cRankingRefreshType)>>::put(w, m_cRankingRefreshType);
        Codec<std::remove_cvref_t<decltype(m_uiLastIndex)>>::put(w, m_uiLastIndex);
    }

    static KELG_GET_WEB_RANKING_REFRESH_NOT read_from(Reader& r) {
        KELG_GET_WEB_RANKING_REFRESH_NOT v;
        v.m_iServerUID = Codec<std::remove_cvref_t<decltype(v.m_iServerUID)>>::get(r);
        v.m_cRankingRefreshType = Codec<std::remove_cvref_t<decltype(v.m_cRankingRefreshType)>>::get(r);
        v.m_uiLastIndex = Codec<std::remove_cvref_t<decltype(v.m_uiLastIndex)>>::get(r);
        return v;
    }
};

struct KELG_INIT_GUILD_SKILL_NOT {
    std::int32_t m_iGuildSPoint{};  // int
    std::int32_t m_iGuildCSPoint{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iGuildSPoint)>>::put(w, m_iGuildSPoint);
        Codec<std::remove_cvref_t<decltype(m_iGuildCSPoint)>>::put(w, m_iGuildCSPoint);
    }

    static KELG_INIT_GUILD_SKILL_NOT read_from(Reader& r) {
        KELG_INIT_GUILD_SKILL_NOT v;
        v.m_iGuildSPoint = Codec<std::remove_cvref_t<decltype(v.m_iGuildSPoint)>>::get(r);
        v.m_iGuildCSPoint = Codec<std::remove_cvref_t<decltype(v.m_iGuildCSPoint)>>::get(r);
        return v;
    }
};

struct KELG_INIT_GUILD_SKILL_REQ {
    std::int32_t m_iGuildUID{};  // int
    std::int64_t m_iUnitUID{};  // UidType
    std::int64_t m_iItemUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iGuildUID)>>::put(w, m_iGuildUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
    }

    static KELG_INIT_GUILD_SKILL_REQ read_from(Reader& r) {
        KELG_INIT_GUILD_SKILL_REQ v;
        v.m_iGuildUID = Codec<std::remove_cvref_t<decltype(v.m_iGuildUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        return v;
    }
};

struct KELG_INSERT_GUILD_CASH_SKILL_POINT_NOT {
    std::int32_t m_iGuildUID{};  // int
    std::int64_t m_iUnitUID{};  // UidType
    std::int32_t m_iGuildCSPoint{};  // int
    std::int32_t m_iPeriod{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iGuildUID)>>::put(w, m_iGuildUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iGuildCSPoint)>>::put(w, m_iGuildCSPoint);
        Codec<std::remove_cvref_t<decltype(m_iPeriod)>>::put(w, m_iPeriod);
    }

    static KELG_INSERT_GUILD_CASH_SKILL_POINT_NOT read_from(Reader& r) {
        KELG_INSERT_GUILD_CASH_SKILL_POINT_NOT v;
        v.m_iGuildUID = Codec<std::remove_cvref_t<decltype(v.m_iGuildUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iGuildCSPoint = Codec<std::remove_cvref_t<decltype(v.m_iGuildCSPoint)>>::get(r);
        v.m_iPeriod = Codec<std::remove_cvref_t<decltype(v.m_iPeriod)>>::get(r);
        return v;
    }
};

struct KELG_INVITE_GUILD_ACK {
    std::int32_t   m_iOK{};  // int
    std::u16string m_wstrReceiverNickName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_wstrReceiverNickName)>>::put(w, m_wstrReceiverNickName);
    }

    static KELG_INVITE_GUILD_ACK read_from(Reader& r) {
        KELG_INVITE_GUILD_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_wstrReceiverNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrReceiverNickName)>>::get(r);
        return v;
    }
};

struct KELG_INVITE_GUILD_FIND_USER_ACK {
    std::int32_t   m_iOK{};  // int
    std::uint16_t  m_usEventID{};  // u_short
    std::int32_t   m_iGuildUID{};  // int
    std::int64_t   m_iSenderUnitUID{};  // UidType
    std::int64_t   m_iReceiverUnitUID{};  // UidType
    std::u16string m_wstrReceiverNickName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_usEventID)>>::put(w, m_usEventID);
        Codec<std::remove_cvref_t<decltype(m_iGuildUID)>>::put(w, m_iGuildUID);
        Codec<std::remove_cvref_t<decltype(m_iSenderUnitUID)>>::put(w, m_iSenderUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iReceiverUnitUID)>>::put(w, m_iReceiverUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrReceiverNickName)>>::put(w, m_wstrReceiverNickName);
    }

    static KELG_INVITE_GUILD_FIND_USER_ACK read_from(Reader& r) {
        KELG_INVITE_GUILD_FIND_USER_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_usEventID = Codec<std::remove_cvref_t<decltype(v.m_usEventID)>>::get(r);
        v.m_iGuildUID = Codec<std::remove_cvref_t<decltype(v.m_iGuildUID)>>::get(r);
        v.m_iSenderUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iSenderUnitUID)>>::get(r);
        v.m_iReceiverUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iReceiverUnitUID)>>::get(r);
        v.m_wstrReceiverNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrReceiverNickName)>>::get(r);
        return v;
    }
};

struct KELG_INVITE_GUILD_REPLY_ACK {
    std::int32_t m_iOK{};  // int
    std::int32_t m_iGuildUID{};  // int
    std::int64_t m_iGuildMasterUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iGuildUID)>>::put(w, m_iGuildUID);
        Codec<std::remove_cvref_t<decltype(m_iGuildMasterUnitUID)>>::put(w, m_iGuildMasterUnitUID);
    }

    static KELG_INVITE_GUILD_REPLY_ACK read_from(Reader& r) {
        KELG_INVITE_GUILD_REPLY_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iGuildUID = Codec<std::remove_cvref_t<decltype(v.m_iGuildUID)>>::get(r);
        v.m_iGuildMasterUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iGuildMasterUnitUID)>>::get(r);
        return v;
    }
};

struct KELG_INVITE_GUILD_REPLY_REQ {
    std::int32_t   m_iGuildUID{};  // int
    std::u16string m_wstrSenderNickName{};  // std::wstring
    std::int32_t   m_iReason{};  // int
    std::int64_t   m_iUnitUID{};  // UidType
    std::u16string m_wstrNickName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iGuildUID)>>::put(w, m_iGuildUID);
        Codec<std::remove_cvref_t<decltype(m_wstrSenderNickName)>>::put(w, m_wstrSenderNickName);
        Codec<std::remove_cvref_t<decltype(m_iReason)>>::put(w, m_iReason);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
    }

    static KELG_INVITE_GUILD_REPLY_REQ read_from(Reader& r) {
        KELG_INVITE_GUILD_REPLY_REQ v;
        v.m_iGuildUID = Codec<std::remove_cvref_t<decltype(v.m_iGuildUID)>>::get(r);
        v.m_wstrSenderNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrSenderNickName)>>::get(r);
        v.m_iReason = Codec<std::remove_cvref_t<decltype(v.m_iReason)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        return v;
    }
};

struct KELG_INVITE_GUILD_REQ {
    std::int32_t   m_iGuildUID{};  // int
    std::int64_t   m_iSenderUnitUID{};  // UidType
    std::u16string m_wstrReceiverNickName{};  // std::wstring
    std::int64_t   m_iReceiverUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iGuildUID)>>::put(w, m_iGuildUID);
        Codec<std::remove_cvref_t<decltype(m_iSenderUnitUID)>>::put(w, m_iSenderUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrReceiverNickName)>>::put(w, m_wstrReceiverNickName);
        Codec<std::remove_cvref_t<decltype(m_iReceiverUnitUID)>>::put(w, m_iReceiverUnitUID);
    }

    static KELG_INVITE_GUILD_REQ read_from(Reader& r) {
        KELG_INVITE_GUILD_REQ v;
        v.m_iGuildUID = Codec<std::remove_cvref_t<decltype(v.m_iGuildUID)>>::get(r);
        v.m_iSenderUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iSenderUnitUID)>>::get(r);
        v.m_wstrReceiverNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrReceiverNickName)>>::get(r);
        v.m_iReceiverUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iReceiverUnitUID)>>::get(r);
        return v;
    }
};

struct KELG_INVITE_PARTY_FIND_RECEIVER_REQ {
    std::int64_t     m_iPartyUID{};  // UidType
    std::int64_t     m_iHostUnitUID{};  // UidType
    std::u16string   m_wstrHostNickName{};  // std::wstring
    std::int64_t     m_iReceiverUnitUID{};  // UidType
    std::int32_t     m_iServerGroupID{};  // int
    std::u16string   m_wstrReceiverNickName{};  // std::wstring
    KCreatePartyInfo m_kCreatePartyInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iPartyUID)>>::put(w, m_iPartyUID);
        Codec<std::remove_cvref_t<decltype(m_iHostUnitUID)>>::put(w, m_iHostUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrHostNickName)>>::put(w, m_wstrHostNickName);
        Codec<std::remove_cvref_t<decltype(m_iReceiverUnitUID)>>::put(w, m_iReceiverUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iServerGroupID)>>::put(w, m_iServerGroupID);
        Codec<std::remove_cvref_t<decltype(m_wstrReceiverNickName)>>::put(w, m_wstrReceiverNickName);
        Codec<std::remove_cvref_t<decltype(m_kCreatePartyInfo)>>::put(w, m_kCreatePartyInfo);
    }

    static KELG_INVITE_PARTY_FIND_RECEIVER_REQ read_from(Reader& r) {
        KELG_INVITE_PARTY_FIND_RECEIVER_REQ v;
        v.m_iPartyUID = Codec<std::remove_cvref_t<decltype(v.m_iPartyUID)>>::get(r);
        v.m_iHostUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iHostUnitUID)>>::get(r);
        v.m_wstrHostNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrHostNickName)>>::get(r);
        v.m_iReceiverUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iReceiverUnitUID)>>::get(r);
        v.m_iServerGroupID = Codec<std::remove_cvref_t<decltype(v.m_iServerGroupID)>>::get(r);
        v.m_wstrReceiverNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrReceiverNickName)>>::get(r);
        v.m_kCreatePartyInfo = Codec<std::remove_cvref_t<decltype(v.m_kCreatePartyInfo)>>::get(r);
        return v;
    }
};

struct KELG_INVITE_PVP_ROOM_ACK {
    std::int32_t   m_iOK{};  // int
    std::u16string m_wstrNickName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
    }

    static KELG_INVITE_PVP_ROOM_ACK read_from(Reader& r) {
        KELG_INVITE_PVP_ROOM_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        return v;
    }
};

struct KELG_KICK_RESULT_LETTER_NOT {
    std::int64_t   m_iUnitUID{};  // UidType
    std::u16string m_wstrGuildName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrGuildName)>>::put(w, m_wstrGuildName);
    }

    static KELG_KICK_RESULT_LETTER_NOT read_from(Reader& r) {
        KELG_KICK_RESULT_LETTER_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_wstrGuildName = Codec<std::remove_cvref_t<decltype(v.m_wstrGuildName)>>::get(r);
        return v;
    }
};

struct KELG_KNM_TOGETHER_BY_UNIT_INFO_REQ {
    std::int64_t m_iDemandGSUID{};  // UidType
    std::int64_t m_iDemandUserUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iDemandGSUID)>>::put(w, m_iDemandGSUID);
        Codec<std::remove_cvref_t<decltype(m_iDemandUserUID)>>::put(w, m_iDemandUserUID);
    }

    static KELG_KNM_TOGETHER_BY_UNIT_INFO_REQ read_from(Reader& r) {
        KELG_KNM_TOGETHER_BY_UNIT_INFO_REQ v;
        v.m_iDemandGSUID = Codec<std::remove_cvref_t<decltype(v.m_iDemandGSUID)>>::get(r);
        v.m_iDemandUserUID = Codec<std::remove_cvref_t<decltype(v.m_iDemandUserUID)>>::get(r);
        return v;
    }
};

struct KELG_NEXON_USER_LOGIN_TIME_OUT_NOT {
    std::u16string m_wstrUserID{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrUserID)>>::put(w, m_wstrUserID);
    }

    static KELG_NEXON_USER_LOGIN_TIME_OUT_NOT read_from(Reader& r) {
        KELG_NEXON_USER_LOGIN_TIME_OUT_NOT v;
        v.m_wstrUserID = Codec<std::remove_cvref_t<decltype(v.m_wstrUserID)>>::get(r);
        return v;
    }
};

struct KELG_RECOMMEND_USER_INFO_NOT {
    std::int64_t m_iRecommendedUnitUID{};  // UidType
    std::int64_t m_iRecommendUnitUID{};  // UidType
    char         m_cType{};
    std::int32_t m_iData{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iRecommendedUnitUID)>>::put(w, m_iRecommendedUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iRecommendUnitUID)>>::put(w, m_iRecommendUnitUID);
        Codec<std::remove_cvref_t<decltype(m_cType)>>::put(w, m_cType);
        Codec<std::remove_cvref_t<decltype(m_iData)>>::put(w, m_iData);
    }

    static KELG_RECOMMEND_USER_INFO_NOT read_from(Reader& r) {
        KELG_RECOMMEND_USER_INFO_NOT v;
        v.m_iRecommendedUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iRecommendedUnitUID)>>::get(r);
        v.m_iRecommendUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iRecommendUnitUID)>>::get(r);
        v.m_cType = Codec<std::remove_cvref_t<decltype(v.m_cType)>>::get(r);
        v.m_iData = Codec<std::remove_cvref_t<decltype(v.m_iData)>>::get(r);
        return v;
    }
};

struct KELG_REGISTER_UNIT_NOT {
    std::int64_t           m_nUnitUID{};  // UidType
    std::uint32_t          m_uiKNMSerialNum{};  // u_int
    std::u16string         m_wstrUnitNickName{};  // std::wstring
    bool                   m_bDenyFriendShip{};
    std::set<std::int64_t> m_setFriendUID{};  // std::set< UidType >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_nUnitUID)>>::put(w, m_nUnitUID);
        Codec<std::remove_cvref_t<decltype(m_uiKNMSerialNum)>>::put(w, m_uiKNMSerialNum);
        Codec<std::remove_cvref_t<decltype(m_wstrUnitNickName)>>::put(w, m_wstrUnitNickName);
        Codec<std::remove_cvref_t<decltype(m_bDenyFriendShip)>>::put(w, m_bDenyFriendShip);
        Codec<std::remove_cvref_t<decltype(m_setFriendUID)>>::put(w, m_setFriendUID);
    }

    static KELG_REGISTER_UNIT_NOT read_from(Reader& r) {
        KELG_REGISTER_UNIT_NOT v;
        v.m_nUnitUID = Codec<std::remove_cvref_t<decltype(v.m_nUnitUID)>>::get(r);
        v.m_uiKNMSerialNum = Codec<std::remove_cvref_t<decltype(v.m_uiKNMSerialNum)>>::get(r);
        v.m_wstrUnitNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrUnitNickName)>>::get(r);
        v.m_bDenyFriendShip = Codec<std::remove_cvref_t<decltype(v.m_bDenyFriendShip)>>::get(r);
        v.m_setFriendUID = Codec<std::remove_cvref_t<decltype(v.m_setFriendUID)>>::get(r);
        return v;
    }
};

struct KELG_REGISTER_USER_ACK {
    std::int32_t m_iOK{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
    }

    static KELG_REGISTER_USER_ACK read_from(Reader& r) {
        KELG_REGISTER_USER_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        return v;
    }
};

struct KELG_REGISTER_USER_GLOBAL_PUBLISHER_ACK {
    std::int32_t   m_iOK{};  // int
    std::u16string m_wstrServiceAccountID{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_wstrServiceAccountID)>>::put(w, m_wstrServiceAccountID);
    }

    static KELG_REGISTER_USER_GLOBAL_PUBLISHER_ACK read_from(Reader& r) {
        KELG_REGISTER_USER_GLOBAL_PUBLISHER_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_wstrServiceAccountID = Codec<std::remove_cvref_t<decltype(v.m_wstrServiceAccountID)>>::get(r);
        return v;
    }
};

