// ---------------------------------------------------------------------------
// packets_2.inc - GENERATED FILE, DO NOT EDIT BY HAND.
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

struct KEGS_CHANGE_PLAY_TIME_LIMIT_REQ {
    float m_fPlayTime{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_fPlayTime)>>::put(w, m_fPlayTime);
    }

    static KEGS_CHANGE_PLAY_TIME_LIMIT_REQ read_from(Reader& r) {
        KEGS_CHANGE_PLAY_TIME_LIMIT_REQ v;
        v.m_fPlayTime = Codec<std::remove_cvref_t<decltype(v.m_fPlayTime)>>::get(r);
        return v;
    }
};

struct KEGS_CHANGE_PSHOP_AGENCY_EXPIRATION_NOT {
    char           m_cShopType{};
    std::u16string m_wstrAgencyExpirationDate{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cShopType)>>::put(w, m_cShopType);
        Codec<std::remove_cvref_t<decltype(m_wstrAgencyExpirationDate)>>::put(w, m_wstrAgencyExpirationDate);
    }

    static KEGS_CHANGE_PSHOP_AGENCY_EXPIRATION_NOT read_from(Reader& r) {
        KEGS_CHANGE_PSHOP_AGENCY_EXPIRATION_NOT v;
        v.m_cShopType = Codec<std::remove_cvref_t<decltype(v.m_cShopType)>>::get(r);
        v.m_wstrAgencyExpirationDate = Codec<std::remove_cvref_t<decltype(v.m_wstrAgencyExpirationDate)>>::get(r);
        return v;
    }
};

struct KEGS_CHANGE_PVP_ITEM_NOT {
    std::int32_t m_iDropItemUID{};  // int
    std::int32_t m_iItemID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iDropItemUID)>>::put(w, m_iDropItemUID);
        Codec<std::remove_cvref_t<decltype(m_iItemID)>>::put(w, m_iItemID);
    }

    static KEGS_CHANGE_PVP_ITEM_NOT read_from(Reader& r) {
        KEGS_CHANGE_PVP_ITEM_NOT v;
        v.m_iDropItemUID = Codec<std::remove_cvref_t<decltype(v.m_iDropItemUID)>>::get(r);
        v.m_iItemID = Codec<std::remove_cvref_t<decltype(v.m_iItemID)>>::get(r);
        return v;
    }
};

struct KEGS_CHANGE_PVP_ITEM_POS_REQ {
    std::int64_t m_iItemUID{};  // UidType
    std::int32_t m_iPos{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_iPos)>>::put(w, m_iPos);
    }

    static KEGS_CHANGE_PVP_ITEM_POS_REQ read_from(Reader& r) {
        KEGS_CHANGE_PVP_ITEM_POS_REQ v;
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_iPos = Codec<std::remove_cvref_t<decltype(v.m_iPos)>>::get(r);
        return v;
    }
};

struct KEGS_CHANGE_PVP_ROOM_NAME_ACK {
    std::u16string m_wstrRoomName{};  // std::wstring
    std::int32_t   m_iOK{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrRoomName)>>::put(w, m_wstrRoomName);
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
    }

    static KEGS_CHANGE_PVP_ROOM_NAME_ACK read_from(Reader& r) {
        KEGS_CHANGE_PVP_ROOM_NAME_ACK v;
        v.m_wstrRoomName = Codec<std::remove_cvref_t<decltype(v.m_wstrRoomName)>>::get(r);
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        return v;
    }
};

struct KEGS_CHANGE_PVP_ROOM_NAME_NOT {
    std::u16string m_wstrRoomName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrRoomName)>>::put(w, m_wstrRoomName);
    }

    static KEGS_CHANGE_PVP_ROOM_NAME_NOT read_from(Reader& r) {
        KEGS_CHANGE_PVP_ROOM_NAME_NOT v;
        v.m_wstrRoomName = Codec<std::remove_cvref_t<decltype(v.m_wstrRoomName)>>::get(r);
        return v;
    }
};

struct KEGS_CHANGE_PVP_ROOM_NAME_REQ {
    std::u16string m_wstrRoomName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrRoomName)>>::put(w, m_wstrRoomName);
    }

    static KEGS_CHANGE_PVP_ROOM_NAME_REQ read_from(Reader& r) {
        KEGS_CHANGE_PVP_ROOM_NAME_REQ v;
        v.m_wstrRoomName = Codec<std::remove_cvref_t<decltype(v.m_wstrRoomName)>>::get(r);
        return v;
    }
};

struct KEGS_CHANGE_PVP_ROOM_PUBLIC_ACK {
    bool         m_bPublic{};
    std::int32_t m_iOK{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bPublic)>>::put(w, m_bPublic);
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
    }

    static KEGS_CHANGE_PVP_ROOM_PUBLIC_ACK read_from(Reader& r) {
        KEGS_CHANGE_PVP_ROOM_PUBLIC_ACK v;
        v.m_bPublic = Codec<std::remove_cvref_t<decltype(v.m_bPublic)>>::get(r);
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        return v;
    }
};

struct KEGS_CHANGE_PVP_ROOM_PUBLIC_NOT {
    bool m_bPublic{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bPublic)>>::put(w, m_bPublic);
    }

    static KEGS_CHANGE_PVP_ROOM_PUBLIC_NOT read_from(Reader& r) {
        KEGS_CHANGE_PVP_ROOM_PUBLIC_NOT v;
        v.m_bPublic = Codec<std::remove_cvref_t<decltype(v.m_bPublic)>>::get(r);
        return v;
    }
};

struct KEGS_CHANGE_PVP_ROOM_PUBLIC_REQ {
    bool           m_bPublic{};
    std::u16string m_wstrPassword{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bPublic)>>::put(w, m_bPublic);
        Codec<std::remove_cvref_t<decltype(m_wstrPassword)>>::put(w, m_wstrPassword);
    }

    static KEGS_CHANGE_PVP_ROOM_PUBLIC_REQ read_from(Reader& r) {
        KEGS_CHANGE_PVP_ROOM_PUBLIC_REQ v;
        v.m_bPublic = Codec<std::remove_cvref_t<decltype(v.m_bPublic)>>::get(r);
        v.m_wstrPassword = Codec<std::remove_cvref_t<decltype(v.m_wstrPassword)>>::get(r);
        return v;
    }
};

struct KEGS_CHANGE_RANDOM_QUEST_NOT {
    std::map<std::int32_t, std::int32_t> m_mapRandomQuestList{};  // std::map< int, int >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_mapRandomQuestList)>>::put(w, m_mapRandomQuestList);
    }

    static KEGS_CHANGE_RANDOM_QUEST_NOT read_from(Reader& r) {
        KEGS_CHANGE_RANDOM_QUEST_NOT v;
        v.m_mapRandomQuestList = Codec<std::remove_cvref_t<decltype(v.m_mapRandomQuestList)>>::get(r);
        return v;
    }
};

struct KEGS_CHANGE_READY_ACK {
    std::int32_t m_iOK{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
    }

    static KEGS_CHANGE_READY_ACK read_from(Reader& r) {
        KEGS_CHANGE_READY_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        return v;
    }
};

struct KEGS_CHANGE_READY_NOT {
    std::int64_t m_UnitUID{};  // UidType
    bool         m_bReady{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_UnitUID)>>::put(w, m_UnitUID);
        Codec<std::remove_cvref_t<decltype(m_bReady)>>::put(w, m_bReady);
    }

    static KEGS_CHANGE_READY_NOT read_from(Reader& r) {
        KEGS_CHANGE_READY_NOT v;
        v.m_UnitUID = Codec<std::remove_cvref_t<decltype(v.m_UnitUID)>>::get(r);
        v.m_bReady = Codec<std::remove_cvref_t<decltype(v.m_bReady)>>::get(r);
        return v;
    }
};

struct KEGS_CHANGE_READY_REQ {
    bool m_bReady{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bReady)>>::put(w, m_bReady);
    }

    static KEGS_CHANGE_READY_REQ read_from(Reader& r) {
        KEGS_CHANGE_READY_REQ v;
        v.m_bReady = Codec<std::remove_cvref_t<decltype(v.m_bReady)>>::get(r);
        return v;
    }
};

struct KEGS_CHANGE_SECOND_SECURITY_PW_REQ {
    std::u16string m_wstrOldSecondPW{};  // std::wstring
    std::u16string m_wstrNewSecondPW{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrOldSecondPW)>>::put(w, m_wstrOldSecondPW);
        Codec<std::remove_cvref_t<decltype(m_wstrNewSecondPW)>>::put(w, m_wstrNewSecondPW);
    }

    static KEGS_CHANGE_SECOND_SECURITY_PW_REQ read_from(Reader& r) {
        KEGS_CHANGE_SECOND_SECURITY_PW_REQ v;
        v.m_wstrOldSecondPW = Codec<std::remove_cvref_t<decltype(v.m_wstrOldSecondPW)>>::get(r);
        v.m_wstrNewSecondPW = Codec<std::remove_cvref_t<decltype(v.m_wstrNewSecondPW)>>::get(r);
        return v;
    }
};

struct KEGS_CHANGE_SKILL_SLOT_ACK {
    std::int32_t m_iOK{};  // int
    std::int32_t m_iSlotID{};  // int
    std::int32_t m_iSkillID{};  // int
    std::int32_t m_iSlotID2{};  // int
    std::int32_t m_iSkillID2{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iSlotID)>>::put(w, m_iSlotID);
        Codec<std::remove_cvref_t<decltype(m_iSkillID)>>::put(w, m_iSkillID);
        Codec<std::remove_cvref_t<decltype(m_iSlotID2)>>::put(w, m_iSlotID2);
        Codec<std::remove_cvref_t<decltype(m_iSkillID2)>>::put(w, m_iSkillID2);
    }

    static KEGS_CHANGE_SKILL_SLOT_ACK read_from(Reader& r) {
        KEGS_CHANGE_SKILL_SLOT_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iSlotID = Codec<std::remove_cvref_t<decltype(v.m_iSlotID)>>::get(r);
        v.m_iSkillID = Codec<std::remove_cvref_t<decltype(v.m_iSkillID)>>::get(r);
        v.m_iSlotID2 = Codec<std::remove_cvref_t<decltype(v.m_iSlotID2)>>::get(r);
        v.m_iSkillID2 = Codec<std::remove_cvref_t<decltype(v.m_iSkillID2)>>::get(r);
        return v;
    }
};

struct KEGS_CHANGE_SKILL_SLOT_NOT {
    std::int32_t m_iSlotID{};  // int
    std::int32_t m_iSkillID{};  // int
    std::int32_t m_iSlotID2{};  // int
    std::int32_t m_iSkillID2{};  // int
    std::int64_t m_iUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iSlotID)>>::put(w, m_iSlotID);
        Codec<std::remove_cvref_t<decltype(m_iSkillID)>>::put(w, m_iSkillID);
        Codec<std::remove_cvref_t<decltype(m_iSlotID2)>>::put(w, m_iSlotID2);
        Codec<std::remove_cvref_t<decltype(m_iSkillID2)>>::put(w, m_iSkillID2);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
    }

    static KEGS_CHANGE_SKILL_SLOT_NOT read_from(Reader& r) {
        KEGS_CHANGE_SKILL_SLOT_NOT v;
        v.m_iSlotID = Codec<std::remove_cvref_t<decltype(v.m_iSlotID)>>::get(r);
        v.m_iSkillID = Codec<std::remove_cvref_t<decltype(v.m_iSkillID)>>::get(r);
        v.m_iSlotID2 = Codec<std::remove_cvref_t<decltype(v.m_iSlotID2)>>::get(r);
        v.m_iSkillID2 = Codec<std::remove_cvref_t<decltype(v.m_iSkillID2)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        return v;
    }
};

struct KEGS_CHANGE_SKILL_SLOT_REQ {
    std::int32_t m_iSlotID{};  // int
    std::int32_t m_iSkillID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iSlotID)>>::put(w, m_iSlotID);
        Codec<std::remove_cvref_t<decltype(m_iSkillID)>>::put(w, m_iSkillID);
    }

    static KEGS_CHANGE_SKILL_SLOT_REQ read_from(Reader& r) {
        KEGS_CHANGE_SKILL_SLOT_REQ v;
        v.m_iSlotID = Codec<std::remove_cvref_t<decltype(v.m_iSlotID)>>::get(r);
        v.m_iSkillID = Codec<std::remove_cvref_t<decltype(v.m_iSkillID)>>::get(r);
        return v;
    }
};

struct KEGS_CHANGE_SLOT_OPEN_ACK {
    std::int32_t m_iOK{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
    }

    static KEGS_CHANGE_SLOT_OPEN_ACK read_from(Reader& r) {
        KEGS_CHANGE_SLOT_OPEN_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        return v;
    }
};

struct KEGS_CHANGE_SLOT_OPEN_NOT {
    char m_SlotIndex{};
    char m_TeamBalanceSlotIndex{};
    char m_SlotState{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_SlotIndex)>>::put(w, m_SlotIndex);
        Codec<std::remove_cvref_t<decltype(m_TeamBalanceSlotIndex)>>::put(w, m_TeamBalanceSlotIndex);
        Codec<std::remove_cvref_t<decltype(m_SlotState)>>::put(w, m_SlotState);
    }

    static KEGS_CHANGE_SLOT_OPEN_NOT read_from(Reader& r) {
        KEGS_CHANGE_SLOT_OPEN_NOT v;
        v.m_SlotIndex = Codec<std::remove_cvref_t<decltype(v.m_SlotIndex)>>::get(r);
        v.m_TeamBalanceSlotIndex = Codec<std::remove_cvref_t<decltype(v.m_TeamBalanceSlotIndex)>>::get(r);
        v.m_SlotState = Codec<std::remove_cvref_t<decltype(v.m_SlotState)>>::get(r);
        return v;
    }
};

struct KEGS_CHANGE_SLOT_OPEN_REQ {
    char m_SlotIndex{};
    char m_SlotState{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_SlotIndex)>>::put(w, m_SlotIndex);
        Codec<std::remove_cvref_t<decltype(m_SlotState)>>::put(w, m_SlotState);
    }

    static KEGS_CHANGE_SLOT_OPEN_REQ read_from(Reader& r) {
        KEGS_CHANGE_SLOT_OPEN_REQ v;
        v.m_SlotIndex = Codec<std::remove_cvref_t<decltype(v.m_SlotIndex)>>::get(r);
        v.m_SlotState = Codec<std::remove_cvref_t<decltype(v.m_SlotState)>>::get(r);
        return v;
    }
};

struct KEGS_CHANGE_TEAM_ACK {
    std::int32_t m_iOK{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
    }

    static KEGS_CHANGE_TEAM_ACK read_from(Reader& r) {
        KEGS_CHANGE_TEAM_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        return v;
    }
};

struct KEGS_CHANGE_TEAM_NOT {
    std::int64_t m_UnitUID{};  // UidType
    char         m_TeamNum{};
    char         m_SwapSlotIndex{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_UnitUID)>>::put(w, m_UnitUID);
        Codec<std::remove_cvref_t<decltype(m_TeamNum)>>::put(w, m_TeamNum);
        Codec<std::remove_cvref_t<decltype(m_SwapSlotIndex)>>::put(w, m_SwapSlotIndex);
    }

    static KEGS_CHANGE_TEAM_NOT read_from(Reader& r) {
        KEGS_CHANGE_TEAM_NOT v;
        v.m_UnitUID = Codec<std::remove_cvref_t<decltype(v.m_UnitUID)>>::get(r);
        v.m_TeamNum = Codec<std::remove_cvref_t<decltype(v.m_TeamNum)>>::get(r);
        v.m_SwapSlotIndex = Codec<std::remove_cvref_t<decltype(v.m_SwapSlotIndex)>>::get(r);
        return v;
    }
};

struct KEGS_CHANGE_TEAM_REQ {
    char m_TeamNum{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_TeamNum)>>::put(w, m_TeamNum);
    }

    static KEGS_CHANGE_TEAM_REQ read_from(Reader& r) {
        KEGS_CHANGE_TEAM_REQ v;
        v.m_TeamNum = Codec<std::remove_cvref_t<decltype(v.m_TeamNum)>>::get(r);
        return v;
    }
};

struct KEGS_CHANGE_TRADE_NOT {
    std::int64_t m_iUnitUID{};  // UidType
    bool         m_bTrade{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_bTrade)>>::put(w, m_bTrade);
    }

    static KEGS_CHANGE_TRADE_NOT read_from(Reader& r) {
        KEGS_CHANGE_TRADE_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_bTrade = Codec<std::remove_cvref_t<decltype(v.m_bTrade)>>::get(r);
        return v;
    }
};

struct KEGS_CHANGE_USER_INFO_NOT {
    std::int64_t   m_iUnitUID{};  // UidType
    char           m_cUnitClass{};
    std::u16string m_wstrGuildName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_cUnitClass)>>::put(w, m_cUnitClass);
        Codec<std::remove_cvref_t<decltype(m_wstrGuildName)>>::put(w, m_wstrGuildName);
    }

    static KEGS_CHANGE_USER_INFO_NOT read_from(Reader& r) {
        KEGS_CHANGE_USER_INFO_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_cUnitClass = Codec<std::remove_cvref_t<decltype(v.m_cUnitClass)>>::get(r);
        v.m_wstrGuildName = Codec<std::remove_cvref_t<decltype(v.m_wstrGuildName)>>::get(r);
        return v;
    }
};

struct KEGS_CHANGE_WINNING_NUM_KILL_NOT {
    std::int32_t m_iWinKillNum{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iWinKillNum)>>::put(w, m_iWinKillNum);
    }

    static KEGS_CHANGE_WINNING_NUM_KILL_NOT read_from(Reader& r) {
        KEGS_CHANGE_WINNING_NUM_KILL_NOT v;
        v.m_iWinKillNum = Codec<std::remove_cvref_t<decltype(v.m_iWinKillNum)>>::get(r);
        return v;
    }
};

struct KEGS_CHANGE_WINNING_NUM_KILL_REQ {
    std::int32_t m_iWinKillNum{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iWinKillNum)>>::put(w, m_iWinKillNum);
    }

    static KEGS_CHANGE_WINNING_NUM_KILL_REQ read_from(Reader& r) {
        KEGS_CHANGE_WINNING_NUM_KILL_REQ v;
        v.m_iWinKillNum = Codec<std::remove_cvref_t<decltype(v.m_iWinKillNum)>>::get(r);
        return v;
    }
};

struct KEGS_CHARGE_POINT_NOT {
    std::int32_t m_iOK{};  // int
    std::int32_t m_iChargedCash{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iChargedCash)>>::put(w, m_iChargedCash);
    }

    static KEGS_CHARGE_POINT_NOT read_from(Reader& r) {
        KEGS_CHARGE_POINT_NOT v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iChargedCash = Codec<std::remove_cvref_t<decltype(v.m_iChargedCash)>>::get(r);
        return v;
    }
};

struct KEGS_CHAT_NOT {
    char           m_cChatPacketType{};
    std::int64_t   m_SenderUnitUID{};  // UidType
    std::u16string m_wstrSenderUnitNickName{};  // std::wstring
    std::u16string m_wstrRecieverUnitNickName{};  // std::wstring
    std::int64_t   m_RecieverUnitUID{};  // UidType
    char           m_cRoomChatType{};
    std::u16string m_wstrMsg{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cChatPacketType)>>::put(w, m_cChatPacketType);
        Codec<std::remove_cvref_t<decltype(m_SenderUnitUID)>>::put(w, m_SenderUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrSenderUnitNickName)>>::put(w, m_wstrSenderUnitNickName);
        Codec<std::remove_cvref_t<decltype(m_wstrRecieverUnitNickName)>>::put(w, m_wstrRecieverUnitNickName);
        Codec<std::remove_cvref_t<decltype(m_RecieverUnitUID)>>::put(w, m_RecieverUnitUID);
        Codec<std::remove_cvref_t<decltype(m_cRoomChatType)>>::put(w, m_cRoomChatType);
        Codec<std::remove_cvref_t<decltype(m_wstrMsg)>>::put(w, m_wstrMsg);
    }

    static KEGS_CHAT_NOT read_from(Reader& r) {
        KEGS_CHAT_NOT v;
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

struct KEGS_CHAT_OPTION_INFO_NOT {
    std::int32_t                 m_iOK{};  // int
    std::map<std::uint8_t, bool> m_mapChatOptionSetInfo{};  // std::map<byte, bool>

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_mapChatOptionSetInfo)>>::put(w, m_mapChatOptionSetInfo);
    }

    static KEGS_CHAT_OPTION_INFO_NOT read_from(Reader& r) {
        KEGS_CHAT_OPTION_INFO_NOT v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_mapChatOptionSetInfo = Codec<std::remove_cvref_t<decltype(v.m_mapChatOptionSetInfo)>>::get(r);
        return v;
    }
};

struct KEGS_CHAT_OPTION_INFO_WRITE_REQ {
    std::uint8_t m_cIndex{};  // byte
    bool         m_bValue{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cIndex)>>::put(w, m_cIndex);
        Codec<std::remove_cvref_t<decltype(m_bValue)>>::put(w, m_bValue);
    }

    static KEGS_CHAT_OPTION_INFO_WRITE_REQ read_from(Reader& r) {
        KEGS_CHAT_OPTION_INFO_WRITE_REQ v;
        v.m_cIndex = Codec<std::remove_cvref_t<decltype(v.m_cIndex)>>::get(r);
        v.m_bValue = Codec<std::remove_cvref_t<decltype(v.m_bValue)>>::get(r);
        return v;
    }
};

struct KEGS_CHAT_REQ {
    char           m_cChatPacketType{};
    char           m_cRoomChatType{};
    std::int64_t   m_ToUnitUID{};  // UidType
    std::u16string m_wstrRecieverUnitNickName{};  // std::wstring
    std::u16string m_wstrMsg{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cChatPacketType)>>::put(w, m_cChatPacketType);
        Codec<std::remove_cvref_t<decltype(m_cRoomChatType)>>::put(w, m_cRoomChatType);
        Codec<std::remove_cvref_t<decltype(m_ToUnitUID)>>::put(w, m_ToUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrRecieverUnitNickName)>>::put(w, m_wstrRecieverUnitNickName);
        Codec<std::remove_cvref_t<decltype(m_wstrMsg)>>::put(w, m_wstrMsg);
    }

    static KEGS_CHAT_REQ read_from(Reader& r) {
        KEGS_CHAT_REQ v;
        v.m_cChatPacketType = Codec<std::remove_cvref_t<decltype(v.m_cChatPacketType)>>::get(r);
        v.m_cRoomChatType = Codec<std::remove_cvref_t<decltype(v.m_cRoomChatType)>>::get(r);
        v.m_ToUnitUID = Codec<std::remove_cvref_t<decltype(v.m_ToUnitUID)>>::get(r);
        v.m_wstrRecieverUnitNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrRecieverUnitNickName)>>::get(r);
        v.m_wstrMsg = Codec<std::remove_cvref_t<decltype(v.m_wstrMsg)>>::get(r);
        return v;
    }
};

struct KEGS_CHECK_BREAK_UP_ACK {
    std::int32_t   m_iOK{};  // int
    std::int32_t   m_iCost{};  // int
    char           m_cRelationshipType{};
    std::u16string m_wstrLoverNickName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iCost)>>::put(w, m_iCost);
        Codec<std::remove_cvref_t<decltype(m_cRelationshipType)>>::put(w, m_cRelationshipType);
        Codec<std::remove_cvref_t<decltype(m_wstrLoverNickName)>>::put(w, m_wstrLoverNickName);
    }

    static KEGS_CHECK_BREAK_UP_ACK read_from(Reader& r) {
        KEGS_CHECK_BREAK_UP_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iCost = Codec<std::remove_cvref_t<decltype(v.m_iCost)>>::get(r);
        v.m_cRelationshipType = Codec<std::remove_cvref_t<decltype(v.m_cRelationshipType)>>::get(r);
        v.m_wstrLoverNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrLoverNickName)>>::get(r);
        return v;
    }
};

struct KEGS_CHECK_CHANNEL_CHANGE_REQ {
    std::int32_t m_iChannelID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iChannelID)>>::put(w, m_iChannelID);
    }

    static KEGS_CHECK_CHANNEL_CHANGE_REQ read_from(Reader& r) {
        KEGS_CHECK_CHANNEL_CHANGE_REQ v;
        v.m_iChannelID = Codec<std::remove_cvref_t<decltype(v.m_iChannelID)>>::get(r);
        return v;
    }
};

struct KEGS_CHECK_EXIST_MONSTER_UID_ACK {
    std::int32_t              m_iOK{};  // int
    std::vector<std::int32_t> m_vecNotExistNpcUID{};  // std::vector< int >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_vecNotExistNpcUID)>>::put(w, m_vecNotExistNpcUID);
    }

    static KEGS_CHECK_EXIST_MONSTER_UID_ACK read_from(Reader& r) {
        KEGS_CHECK_EXIST_MONSTER_UID_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_vecNotExistNpcUID = Codec<std::remove_cvref_t<decltype(v.m_vecNotExistNpcUID)>>::get(r);
        return v;
    }
};

struct KEGS_CHECK_EXIST_MONSTER_UID_REQ {
    std::vector<std::int32_t> m_vecNpcUID{};  // std::vector< int >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecNpcUID)>>::put(w, m_vecNpcUID);
    }

    static KEGS_CHECK_EXIST_MONSTER_UID_REQ read_from(Reader& r) {
        KEGS_CHECK_EXIST_MONSTER_UID_REQ v;
        v.m_vecNpcUID = Codec<std::remove_cvref_t<decltype(v.m_vecNpcUID)>>::get(r);
        return v;
    }
};

struct KEGS_CHECK_INVALID_ROOM_NOT {
    std::int32_t m_iOK{};  // int
    std::int32_t m_iMapID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iMapID)>>::put(w, m_iMapID);
    }

    static KEGS_CHECK_INVALID_ROOM_NOT read_from(Reader& r) {
        KEGS_CHECK_INVALID_ROOM_NOT v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iMapID = Codec<std::remove_cvref_t<decltype(v.m_iMapID)>>::get(r);
        return v;
    }
};

struct KEGS_CHECK_MACHINE_ID_ACK {
    std::int32_t m_iOK{};  // int
    std::string  m_strMachineID{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_strMachineID)>>::put(w, m_strMachineID);
    }

    static KEGS_CHECK_MACHINE_ID_ACK read_from(Reader& r) {
        KEGS_CHECK_MACHINE_ID_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_strMachineID = Codec<std::remove_cvref_t<decltype(v.m_strMachineID)>>::get(r);
        return v;
    }
};

struct KEGS_CHECK_MACHINE_ID_REQ {
    std::string m_strMachineID{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_strMachineID)>>::put(w, m_strMachineID);
    }

    static KEGS_CHECK_MACHINE_ID_REQ read_from(Reader& r) {
        KEGS_CHECK_MACHINE_ID_REQ v;
        v.m_strMachineID = Codec<std::remove_cvref_t<decltype(v.m_strMachineID)>>::get(r);
        return v;
    }
};

struct KEGS_CHECK_NICK_NAME_ACK {
    std::int32_t   m_iOK{};  // int
    std::int64_t   m_iUnitUID{};  // UidType
    std::u16string m_wstrNickName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
    }

    static KEGS_CHECK_NICK_NAME_ACK read_from(Reader& r) {
        KEGS_CHECK_NICK_NAME_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        return v;
    }
};

struct KEGS_CHECK_NICK_NAME_REQ {
    std::u16string m_wstrNickName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
    }

    static KEGS_CHECK_NICK_NAME_REQ read_from(Reader& r) {
        KEGS_CHECK_NICK_NAME_REQ v;
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        return v;
    }
};

struct KEGS_CHECK_PING_SCORE_CHANGE_HOST_NOT {
    std::int64_t m_iOldHostUID{};  // UidType
    std::int64_t m_iNewHostUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOldHostUID)>>::put(w, m_iOldHostUID);
        Codec<std::remove_cvref_t<decltype(m_iNewHostUID)>>::put(w, m_iNewHostUID);
    }

    static KEGS_CHECK_PING_SCORE_CHANGE_HOST_NOT read_from(Reader& r) {
        KEGS_CHECK_PING_SCORE_CHANGE_HOST_NOT v;
        v.m_iOldHostUID = Codec<std::remove_cvref_t<decltype(v.m_iOldHostUID)>>::get(r);
        v.m_iNewHostUID = Codec<std::remove_cvref_t<decltype(v.m_iNewHostUID)>>::get(r);
        return v;
    }
};

struct KEGS_CHECK_PRESENT_CASH_INVENTORY_NOT {
    std::uint32_t m_ulOrderNo{};  // unsigned long
    std::uint32_t m_ulProductNo{};  // unsigned long

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_ulOrderNo)>>::put(w, m_ulOrderNo);
        Codec<std::remove_cvref_t<decltype(m_ulProductNo)>>::put(w, m_ulProductNo);
    }

    static KEGS_CHECK_PRESENT_CASH_INVENTORY_NOT read_from(Reader& r) {
        KEGS_CHECK_PRESENT_CASH_INVENTORY_NOT v;
        v.m_ulOrderNo = Codec<std::remove_cvref_t<decltype(v.m_ulOrderNo)>>::get(r);
        v.m_ulProductNo = Codec<std::remove_cvref_t<decltype(v.m_ulProductNo)>>::get(r);
        return v;
    }
};

struct KEGS_CHECK_SUM_ACK {
    std::int32_t m_iOK{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
    }

    static KEGS_CHECK_SUM_ACK read_from(Reader& r) {
        KEGS_CHECK_SUM_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        return v;
    }
};

struct KEGS_CHECK_SUM_REQ {
    std::map<std::u16string, std::u16string> m_mapCheckSum{};  // std::map< std::wstring, std::wstring >
    std::uint32_t                            m_ProtectTableCSVal{};  // DWORD

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_mapCheckSum)>>::put(w, m_mapCheckSum);
        Codec<std::remove_cvref_t<decltype(m_ProtectTableCSVal)>>::put(w, m_ProtectTableCSVal);
    }

    static KEGS_CHECK_SUM_REQ read_from(Reader& r) {
        KEGS_CHECK_SUM_REQ v;
        v.m_mapCheckSum = Codec<std::remove_cvref_t<decltype(v.m_mapCheckSum)>>::get(r);
        v.m_ProtectTableCSVal = Codec<std::remove_cvref_t<decltype(v.m_ProtectTableCSVal)>>::get(r);
        return v;
    }
};

struct KEGS_CLOSE_PARTY_NOT {
    std::int64_t m_iPartyUID{};  // UidType
    std::int32_t m_iReason{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iPartyUID)>>::put(w, m_iPartyUID);
        Codec<std::remove_cvref_t<decltype(m_iReason)>>::put(w, m_iReason);
    }

    static KEGS_CLOSE_PARTY_NOT read_from(Reader& r) {
        KEGS_CLOSE_PARTY_NOT v;
        v.m_iPartyUID = Codec<std::remove_cvref_t<decltype(v.m_iPartyUID)>>::get(r);
        v.m_iReason = Codec<std::remove_cvref_t<decltype(v.m_iReason)>>::get(r);
        return v;
    }
};

struct KEGS_COME_BACK_BUFF_NOT {
    bool         m_bIsFinish{};
    std::int64_t m_iComeBackBuffEnd{};  // __int64

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bIsFinish)>>::put(w, m_bIsFinish);
        Codec<std::remove_cvref_t<decltype(m_iComeBackBuffEnd)>>::put(w, m_iComeBackBuffEnd);
    }

    static KEGS_COME_BACK_BUFF_NOT read_from(Reader& r) {
        KEGS_COME_BACK_BUFF_NOT v;
        v.m_bIsFinish = Codec<std::remove_cvref_t<decltype(v.m_bIsFinish)>>::get(r);
        v.m_iComeBackBuffEnd = Codec<std::remove_cvref_t<decltype(v.m_iComeBackBuffEnd)>>::get(r);
        return v;
    }
};

struct KEGS_COMMANDS_FOR_PETS_REQ {
    char m_cActionType{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cActionType)>>::put(w, m_cActionType);
    }

    static KEGS_COMMANDS_FOR_PETS_REQ read_from(Reader& r) {
        KEGS_COMMANDS_FOR_PETS_REQ v;
        v.m_cActionType = Codec<std::remove_cvref_t<decltype(v.m_cActionType)>>::get(r);
        return v;
    }
};

struct KEGS_COMMUNITY_USER_LIST_ACK {
    std::int32_t                    m_iOK{};  // int
    std::uint32_t                   m_uiTotalPage{};  // UINT
    std::uint32_t                   m_uiViewPage{};  // UINT
    std::vector<KCommunityUserInfo> m_vecUserList{};  // std::vector< KCommunityUserInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_uiTotalPage)>>::put(w, m_uiTotalPage);
        Codec<std::remove_cvref_t<decltype(m_uiViewPage)>>::put(w, m_uiViewPage);
        Codec<std::remove_cvref_t<decltype(m_vecUserList)>>::put(w, m_vecUserList);
    }

    static KEGS_COMMUNITY_USER_LIST_ACK read_from(Reader& r) {
        KEGS_COMMUNITY_USER_LIST_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_uiTotalPage = Codec<std::remove_cvref_t<decltype(v.m_uiTotalPage)>>::get(r);
        v.m_uiViewPage = Codec<std::remove_cvref_t<decltype(v.m_uiViewPage)>>::get(r);
        v.m_vecUserList = Codec<std::remove_cvref_t<decltype(v.m_vecUserList)>>::get(r);
        return v;
    }
};

struct KEGS_COMMUNITY_USER_LIST_MONITORING_MODE_REQ {
    std::uint32_t             m_uiViewPage{};  // UINT
    std::vector<std::int64_t> m_vecUnitUIDList{};  // std::vector< UidType >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_uiViewPage)>>::put(w, m_uiViewPage);
        Codec<std::remove_cvref_t<decltype(m_vecUnitUIDList)>>::put(w, m_vecUnitUIDList);
    }

    static KEGS_COMMUNITY_USER_LIST_MONITORING_MODE_REQ read_from(Reader& r) {
        KEGS_COMMUNITY_USER_LIST_MONITORING_MODE_REQ v;
        v.m_uiViewPage = Codec<std::remove_cvref_t<decltype(v.m_uiViewPage)>>::get(r);
        v.m_vecUnitUIDList = Codec<std::remove_cvref_t<decltype(v.m_vecUnitUIDList)>>::get(r);
        return v;
    }
};

struct KEGS_COMMUNITY_USER_LIST_REQ {
    std::uint32_t m_uiViewPage{};  // UINT
    bool          m_bMode{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_uiViewPage)>>::put(w, m_uiViewPage);
        Codec<std::remove_cvref_t<decltype(m_bMode)>>::put(w, m_bMode);
    }

    static KEGS_COMMUNITY_USER_LIST_REQ read_from(Reader& r) {
        KEGS_COMMUNITY_USER_LIST_REQ v;
        v.m_uiViewPage = Codec<std::remove_cvref_t<decltype(v.m_uiViewPage)>>::get(r);
        v.m_bMode = Codec<std::remove_cvref_t<decltype(v.m_bMode)>>::get(r);
        return v;
    }
};

struct KEGS_CONNECT_ACK {
    std::int32_t m_iOK{};  // int
    std::int32_t m_UDPPort{};  // int
    std::int32_t m_iChannelID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_UDPPort)>>::put(w, m_UDPPort);
        Codec<std::remove_cvref_t<decltype(m_iChannelID)>>::put(w, m_iChannelID);
    }

    static KEGS_CONNECT_ACK read_from(Reader& r) {
        KEGS_CONNECT_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_UDPPort = Codec<std::remove_cvref_t<decltype(v.m_UDPPort)>>::get(r);
        v.m_iChannelID = Codec<std::remove_cvref_t<decltype(v.m_iChannelID)>>::get(r);
        return v;
    }
};

struct KEGS_CONNECT_REQ {
    std::u16string m_wstrVersion{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrVersion)>>::put(w, m_wstrVersion);
    }

    static KEGS_CONNECT_REQ read_from(Reader& r) {
        KEGS_CONNECT_REQ v;
        v.m_wstrVersion = Codec<std::remove_cvref_t<decltype(v.m_wstrVersion)>>::get(r);
        return v;
    }
};

struct KEGS_COUPLE_MAKING_SUCCESS_NOT {
    std::u16string m_wstrRequestUnitName{};  // std::wstring
    std::u16string m_wstrAcceptUnitName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrRequestUnitName)>>::put(w, m_wstrRequestUnitName);
        Codec<std::remove_cvref_t<decltype(m_wstrAcceptUnitName)>>::put(w, m_wstrAcceptUnitName);
    }

    static KEGS_COUPLE_MAKING_SUCCESS_NOT read_from(Reader& r) {
        KEGS_COUPLE_MAKING_SUCCESS_NOT v;
        v.m_wstrRequestUnitName = Codec<std::remove_cvref_t<decltype(v.m_wstrRequestUnitName)>>::get(r);
        v.m_wstrAcceptUnitName = Codec<std::remove_cvref_t<decltype(v.m_wstrAcceptUnitName)>>::get(r);
        return v;
    }
};

struct KEGS_COUPLE_PROPOSE_ACK {
    std::int32_t   m_iOK{};  // int
    std::u16string m_wstrNickName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
    }

    static KEGS_COUPLE_PROPOSE_ACK read_from(Reader& r) {
        KEGS_COUPLE_PROPOSE_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        return v;
    }
};

struct KEGS_COUPLE_PROPOSE_AGREE_NOT {
    char           m_cAnswer{};
    std::int64_t   m_iRequestUnitUID{};  // UidType
    std::u16string m_wstrRequestUnitName{};  // std::wstring
    std::int64_t   m_iAcceptUserUID{};  // UidType
    std::uint8_t   m_ucAcceptUnitLevel{};  // u_char
    char           m_cAcceptUnitClass{};
    std::int64_t   m_iAcceptUnitUID{};  // UidType
    std::u16string m_wstrAcceptUnitName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cAnswer)>>::put(w, m_cAnswer);
        Codec<std::remove_cvref_t<decltype(m_iRequestUnitUID)>>::put(w, m_iRequestUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrRequestUnitName)>>::put(w, m_wstrRequestUnitName);
        Codec<std::remove_cvref_t<decltype(m_iAcceptUserUID)>>::put(w, m_iAcceptUserUID);
        Codec<std::remove_cvref_t<decltype(m_ucAcceptUnitLevel)>>::put(w, m_ucAcceptUnitLevel);
        Codec<std::remove_cvref_t<decltype(m_cAcceptUnitClass)>>::put(w, m_cAcceptUnitClass);
        Codec<std::remove_cvref_t<decltype(m_iAcceptUnitUID)>>::put(w, m_iAcceptUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrAcceptUnitName)>>::put(w, m_wstrAcceptUnitName);
    }

    static KEGS_COUPLE_PROPOSE_AGREE_NOT read_from(Reader& r) {
        KEGS_COUPLE_PROPOSE_AGREE_NOT v;
        v.m_cAnswer = Codec<std::remove_cvref_t<decltype(v.m_cAnswer)>>::get(r);
        v.m_iRequestUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iRequestUnitUID)>>::get(r);
        v.m_wstrRequestUnitName = Codec<std::remove_cvref_t<decltype(v.m_wstrRequestUnitName)>>::get(r);
        v.m_iAcceptUserUID = Codec<std::remove_cvref_t<decltype(v.m_iAcceptUserUID)>>::get(r);
        v.m_ucAcceptUnitLevel = Codec<std::remove_cvref_t<decltype(v.m_ucAcceptUnitLevel)>>::get(r);
        v.m_cAcceptUnitClass = Codec<std::remove_cvref_t<decltype(v.m_cAcceptUnitClass)>>::get(r);
        v.m_iAcceptUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iAcceptUnitUID)>>::get(r);
        v.m_wstrAcceptUnitName = Codec<std::remove_cvref_t<decltype(v.m_wstrAcceptUnitName)>>::get(r);
        return v;
    }
};

struct KEGS_COUPLE_PROPOSE_NOT {
    std::u16string m_wstrRequestUnitName{};  // std::wstring
    std::int64_t   m_iRequestUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrRequestUnitName)>>::put(w, m_wstrRequestUnitName);
        Codec<std::remove_cvref_t<decltype(m_iRequestUnitUID)>>::put(w, m_iRequestUnitUID);
    }

    static KEGS_COUPLE_PROPOSE_NOT read_from(Reader& r) {
        KEGS_COUPLE_PROPOSE_NOT v;
        v.m_wstrRequestUnitName = Codec<std::remove_cvref_t<decltype(v.m_wstrRequestUnitName)>>::get(r);
        v.m_iRequestUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iRequestUnitUID)>>::get(r);
        return v;
    }
};

struct KEGS_COUPLE_PROPOSE_REQ {
    std::u16string m_wstrUnitName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrUnitName)>>::put(w, m_wstrUnitName);
    }

    static KEGS_COUPLE_PROPOSE_REQ read_from(Reader& r) {
        KEGS_COUPLE_PROPOSE_REQ v;
        v.m_wstrUnitName = Codec<std::remove_cvref_t<decltype(v.m_wstrUnitName)>>::get(r);
        return v;
    }
};

struct KEGS_CREATE_GUILD_REQ {
    std::int64_t   m_iItemUID{};  // UidType
    std::u16string m_wstrGuildName{};  // std::wstring
    std::u16string m_wstrGuildMessage{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_wstrGuildName)>>::put(w, m_wstrGuildName);
        Codec<std::remove_cvref_t<decltype(m_wstrGuildMessage)>>::put(w, m_wstrGuildMessage);
    }

    static KEGS_CREATE_GUILD_REQ read_from(Reader& r) {
        KEGS_CREATE_GUILD_REQ v;
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_wstrGuildName = Codec<std::remove_cvref_t<decltype(v.m_wstrGuildName)>>::get(r);
        v.m_wstrGuildMessage = Codec<std::remove_cvref_t<decltype(v.m_wstrGuildMessage)>>::get(r);
        return v;
    }
};

struct KEGIANT_ROLEREG_CREATE_GUILD_ACK {
    std::int32_t          m_iOK{};  // int
    KEGS_CREATE_GUILD_REQ m_CreateGuildReqPacket{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_CreateGuildReqPacket)>>::put(w, m_CreateGuildReqPacket);
    }

    static KEGIANT_ROLEREG_CREATE_GUILD_ACK read_from(Reader& r) {
        KEGIANT_ROLEREG_CREATE_GUILD_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_CreateGuildReqPacket = Codec<std::remove_cvref_t<decltype(v.m_CreateGuildReqPacket)>>::get(r);
        return v;
    }
};

struct KEGIANT_ROLEREG_CREATE_GUILD_REQ {
    std::int64_t          m_iActorUID{};  // UidType
    std::uint32_t         m_uiGiantUID{};  // unsigned int
    KEGS_CREATE_GUILD_REQ m_CreateGuildReqPacket{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iActorUID)>>::put(w, m_iActorUID);
        Codec<std::remove_cvref_t<decltype(m_uiGiantUID)>>::put(w, m_uiGiantUID);
        Codec<std::remove_cvref_t<decltype(m_CreateGuildReqPacket)>>::put(w, m_CreateGuildReqPacket);
    }

    static KEGIANT_ROLEREG_CREATE_GUILD_REQ read_from(Reader& r) {
        KEGIANT_ROLEREG_CREATE_GUILD_REQ v;
        v.m_iActorUID = Codec<std::remove_cvref_t<decltype(v.m_iActorUID)>>::get(r);
        v.m_uiGiantUID = Codec<std::remove_cvref_t<decltype(v.m_uiGiantUID)>>::get(r);
        v.m_CreateGuildReqPacket = Codec<std::remove_cvref_t<decltype(v.m_CreateGuildReqPacket)>>::get(r);
        return v;
    }
};

struct KEGS_CREATE_NEW_UNIT_REQ {
    std::u16string m_wstrNickName{};  // std::wstring
    std::int32_t   m_iClass{};  // int
    std::int32_t   m_iServerGroup{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_iClass)>>::put(w, m_iClass);
        Codec<std::remove_cvref_t<decltype(m_iServerGroup)>>::put(w, m_iServerGroup);
    }

    static KEGS_CREATE_NEW_UNIT_REQ read_from(Reader& r) {
        KEGS_CREATE_NEW_UNIT_REQ v;
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_iClass = Codec<std::remove_cvref_t<decltype(v.m_iClass)>>::get(r);
        v.m_iServerGroup = Codec<std::remove_cvref_t<decltype(v.m_iServerGroup)>>::get(r);
        return v;
    }
};

struct KEGS_CREATE_PERSONAL_SHOP_ACK {
    std::int32_t m_iOK{};  // int
    std::int64_t m_iPersonalShopUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iPersonalShopUID)>>::put(w, m_iPersonalShopUID);
    }

    static KEGS_CREATE_PERSONAL_SHOP_ACK read_from(Reader& r) {
        KEGS_CREATE_PERSONAL_SHOP_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iPersonalShopUID = Codec<std::remove_cvref_t<decltype(v.m_iPersonalShopUID)>>::get(r);
        return v;
    }
};

struct KEGS_CREATE_PET_REQ {
    std::int64_t   m_iItemUID{};  // UidType
    std::u16string m_wstrPetName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_wstrPetName)>>::put(w, m_wstrPetName);
    }

    static KEGS_CREATE_PET_REQ read_from(Reader& r) {
        KEGS_CREATE_PET_REQ v;
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_wstrPetName = Codec<std::remove_cvref_t<decltype(v.m_wstrPetName)>>::get(r);
        return v;
    }
};

struct KEGS_CREATE_PVP_ITEM_NOT {
    std::int32_t  m_iPositionIndex{};  // int
    KDropItemData m_kDropItemData{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iPositionIndex)>>::put(w, m_iPositionIndex);
        Codec<std::remove_cvref_t<decltype(m_kDropItemData)>>::put(w, m_kDropItemData);
    }

    static KEGS_CREATE_PVP_ITEM_NOT read_from(Reader& r) {
        KEGS_CREATE_PVP_ITEM_NOT v;
        v.m_iPositionIndex = Codec<std::remove_cvref_t<decltype(v.m_iPositionIndex)>>::get(r);
        v.m_kDropItemData = Codec<std::remove_cvref_t<decltype(v.m_kDropItemData)>>::get(r);
        return v;
    }
};

struct KEGS_CREATE_PVP_ITEM_REQ {
    std::int32_t m_iPositionIndex{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iPositionIndex)>>::put(w, m_iPositionIndex);
    }

    static KEGS_CREATE_PVP_ITEM_REQ read_from(Reader& r) {
        KEGS_CREATE_PVP_ITEM_REQ v;
        v.m_iPositionIndex = Codec<std::remove_cvref_t<decltype(v.m_iPositionIndex)>>::get(r);
        return v;
    }
};

struct KEGS_CREATE_RIDING_PET_REQ {
    std::int64_t m_iItemUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
    }

    static KEGS_CREATE_RIDING_PET_REQ read_from(Reader& r) {
        KEGS_CREATE_RIDING_PET_REQ v;
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        return v;
    }
};

struct KEGS_CREATE_SECOND_SECURITY_REQ {
    std::u16string m_wstrSecondPW{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrSecondPW)>>::put(w, m_wstrSecondPW);
    }

    static KEGS_CREATE_SECOND_SECURITY_REQ read_from(Reader& r) {
        KEGS_CREATE_SECOND_SECURITY_REQ v;
        v.m_wstrSecondPW = Codec<std::remove_cvref_t<decltype(v.m_wstrSecondPW)>>::get(r);
        return v;
    }
};

struct KEGS_CREATE_TC_ROOM_REQ {
    std::int32_t m_iTCID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iTCID)>>::put(w, m_iTCID);
    }

    static KEGS_CREATE_TC_ROOM_REQ read_from(Reader& r) {
        KEGS_CREATE_TC_ROOM_REQ v;
        v.m_iTCID = Codec<std::remove_cvref_t<decltype(v.m_iTCID)>>::get(r);
        return v;
    }
};

struct KEGS_CREATE_TRADE_NOT {
    std::int64_t m_iTradeUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iTradeUID)>>::put(w, m_iTradeUID);
    }

    static KEGS_CREATE_TRADE_NOT read_from(Reader& r) {
        KEGS_CREATE_TRADE_NOT v;
        v.m_iTradeUID = Codec<std::remove_cvref_t<decltype(v.m_iTradeUID)>>::get(r);
        return v;
    }
};

struct KEGS_CREATE_UNIT_REQ {
    std::u16string m_wstrNickName{};  // std::wstring
    std::int32_t   m_iClass{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_iClass)>>::put(w, m_iClass);
    }

    static KEGS_CREATE_UNIT_REQ read_from(Reader& r) {
        KEGS_CREATE_UNIT_REQ v;
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_iClass = Codec<std::remove_cvref_t<decltype(v.m_iClass)>>::get(r);
        return v;
    }
};

struct KEGIANT_ROLEREG_CREATE_UNIT_ACK {
    std::int32_t         m_iOK{};  // int
    KEGS_CREATE_UNIT_REQ m_CreateUnitReqPacket{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_CreateUnitReqPacket)>>::put(w, m_CreateUnitReqPacket);
    }

    static KEGIANT_ROLEREG_CREATE_UNIT_ACK read_from(Reader& r) {
        KEGIANT_ROLEREG_CREATE_UNIT_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_CreateUnitReqPacket = Codec<std::remove_cvref_t<decltype(v.m_CreateUnitReqPacket)>>::get(r);
        return v;
    }
};

struct KEGIANT_ROLEREG_CREATE_UNIT_REQ {
    std::int64_t         m_iActorUID{};  // UidType
    std::uint32_t        m_uiGiantUID{};  // unsigned int
    KEGS_CREATE_UNIT_REQ m_CreateUnitReqPacket{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iActorUID)>>::put(w, m_iActorUID);
        Codec<std::remove_cvref_t<decltype(m_uiGiantUID)>>::put(w, m_uiGiantUID);
        Codec<std::remove_cvref_t<decltype(m_CreateUnitReqPacket)>>::put(w, m_CreateUnitReqPacket);
    }

    static KEGIANT_ROLEREG_CREATE_UNIT_REQ read_from(Reader& r) {
        KEGIANT_ROLEREG_CREATE_UNIT_REQ v;
        v.m_iActorUID = Codec<std::remove_cvref_t<decltype(v.m_iActorUID)>>::get(r);
        v.m_uiGiantUID = Codec<std::remove_cvref_t<decltype(v.m_uiGiantUID)>>::get(r);
        v.m_CreateUnitReqPacket = Codec<std::remove_cvref_t<decltype(v.m_CreateUnitReqPacket)>>::get(r);
        return v;
    }
};

struct KEGS_CURRENT_KILL_SCORE_NOT {
    std::map<std::int64_t, std::pair<std::int32_t, std::int32_t>> m_mapKillScore{};  // std::map< UidType, std::pair< int, int > >
    std::map<std::int64_t, std::int32_t>                          m_mapMDKillScore{};  // std::map< UidType, int >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_mapKillScore)>>::put(w, m_mapKillScore);
        Codec<std::remove_cvref_t<decltype(m_mapMDKillScore)>>::put(w, m_mapMDKillScore);
    }

    static KEGS_CURRENT_KILL_SCORE_NOT read_from(Reader& r) {
        KEGS_CURRENT_KILL_SCORE_NOT v;
        v.m_mapKillScore = Codec<std::remove_cvref_t<decltype(v.m_mapKillScore)>>::get(r);
        v.m_mapMDKillScore = Codec<std::remove_cvref_t<decltype(v.m_mapMDKillScore)>>::get(r);
        return v;
    }
};

struct KEGS_CURRENT_TEAM_KILL_SCORE_NOT {
    char m_RedTeamKill{};
    char m_BlueTeamKill{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_RedTeamKill)>>::put(w, m_RedTeamKill);
        Codec<std::remove_cvref_t<decltype(m_BlueTeamKill)>>::put(w, m_BlueTeamKill);
    }

    static KEGS_CURRENT_TEAM_KILL_SCORE_NOT read_from(Reader& r) {
        KEGS_CURRENT_TEAM_KILL_SCORE_NOT v;
        v.m_RedTeamKill = Codec<std::remove_cvref_t<decltype(v.m_RedTeamKill)>>::get(r);
        v.m_BlueTeamKill = Codec<std::remove_cvref_t<decltype(v.m_BlueTeamKill)>>::get(r);
        return v;
    }
};

struct KEGS_CURRENT_TIME_ACK {
    std::u16string m_wstrCurrentTime{};  // std::wstring
    std::int64_t   m_tCurrentTime{};  // __int64

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrCurrentTime)>>::put(w, m_wstrCurrentTime);
        Codec<std::remove_cvref_t<decltype(m_tCurrentTime)>>::put(w, m_tCurrentTime);
    }

    static KEGS_CURRENT_TIME_ACK read_from(Reader& r) {
        KEGS_CURRENT_TIME_ACK v;
        v.m_wstrCurrentTime = Codec<std::remove_cvref_t<decltype(v.m_wstrCurrentTime)>>::get(r);
        v.m_tCurrentTime = Codec<std::remove_cvref_t<decltype(v.m_tCurrentTime)>>::get(r);
        return v;
    }
};

struct KEGS_DECIDE_TO_USE_THIS_SKILL_PAGE_REQ {
    std::int32_t m_iSkillPagesNumberDecidedToUse{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iSkillPagesNumberDecidedToUse)>>::put(w, m_iSkillPagesNumberDecidedToUse);
    }

    static KEGS_DECIDE_TO_USE_THIS_SKILL_PAGE_REQ read_from(Reader& r) {
        KEGS_DECIDE_TO_USE_THIS_SKILL_PAGE_REQ v;
        v.m_iSkillPagesNumberDecidedToUse = Codec<std::remove_cvref_t<decltype(v.m_iSkillPagesNumberDecidedToUse)>>::get(r);
        return v;
    }
};

struct KEGS_DECREASE_ENDURANCE_NOT {
    std::vector<KDecreaseEnduranceInfo> m_vecEnduranceUpdate{};  // std::vector< KDecreaseEnduranceInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecEnduranceUpdate)>>::put(w, m_vecEnduranceUpdate);
    }

    static KEGS_DECREASE_ENDURANCE_NOT read_from(Reader& r) {
        KEGS_DECREASE_ENDURANCE_NOT v;
        v.m_vecEnduranceUpdate = Codec<std::remove_cvref_t<decltype(v.m_vecEnduranceUpdate)>>::get(r);
        return v;
    }
};

struct KEGS_DECREASE_SPIRIT_NOT {
    std::int32_t m_iSpirit{};  // int
    std::int32_t m_iHonorPoint{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iSpirit)>>::put(w, m_iSpirit);
        Codec<std::remove_cvref_t<decltype(m_iHonorPoint)>>::put(w, m_iHonorPoint);
    }

    static KEGS_DECREASE_SPIRIT_NOT read_from(Reader& r) {
        KEGS_DECREASE_SPIRIT_NOT v;
        v.m_iSpirit = Codec<std::remove_cvref_t<decltype(v.m_iSpirit)>>::get(r);
        v.m_iHonorPoint = Codec<std::remove_cvref_t<decltype(v.m_iHonorPoint)>>::get(r);
        return v;
    }
};

struct KEGS_DEFENCE_DUNGEON_WAVE_ID_NOT {
    std::int32_t m_iWaveID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iWaveID)>>::put(w, m_iWaveID);
    }

    static KEGS_DEFENCE_DUNGEON_WAVE_ID_NOT read_from(Reader& r) {
        KEGS_DEFENCE_DUNGEON_WAVE_ID_NOT v;
        v.m_iWaveID = Codec<std::remove_cvref_t<decltype(v.m_iWaveID)>>::get(r);
        return v;
    }
};

struct KEGS_DELETE_APPLY_JOIN_GUILD_REQ {
    std::int64_t m_iUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
    }

    static KEGS_DELETE_APPLY_JOIN_GUILD_REQ read_from(Reader& r) {
        KEGS_DELETE_APPLY_JOIN_GUILD_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        return v;
    }
};

struct KEGS_DELETE_FRIEND_ACK {
    std::int32_t m_iOK{};  // int
    std::int64_t m_iUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
    }

    static KEGS_DELETE_FRIEND_ACK read_from(Reader& r) {
        KEGS_DELETE_FRIEND_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        return v;
    }
};

struct KEGS_DELETE_FRIEND_GROUP_ACK {
    std::int32_t m_iOK{};  // int
    char         m_cGroupID{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_cGroupID)>>::put(w, m_cGroupID);
    }

    static KEGS_DELETE_FRIEND_GROUP_ACK read_from(Reader& r) {
        KEGS_DELETE_FRIEND_GROUP_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_cGroupID = Codec<std::remove_cvref_t<decltype(v.m_cGroupID)>>::get(r);
        return v;
    }
};

struct KEGS_DELETE_FRIEND_GROUP_REQ {
    char m_cGroupID{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cGroupID)>>::put(w, m_cGroupID);
    }

    static KEGS_DELETE_FRIEND_GROUP_REQ read_from(Reader& r) {
        KEGS_DELETE_FRIEND_GROUP_REQ v;
        v.m_cGroupID = Codec<std::remove_cvref_t<decltype(v.m_cGroupID)>>::get(r);
        return v;
    }
};

struct KEGS_DELETE_FRIEND_NOT {
    std::int64_t m_iUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
    }

    static KEGS_DELETE_FRIEND_NOT read_from(Reader& r) {
        KEGS_DELETE_FRIEND_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        return v;
    }
};

struct KEGS_DELETE_ITEM_REQ {
    std::int64_t m_iItemUID{};  // UidType
    std::int32_t m_iQuantity{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_iQuantity)>>::put(w, m_iQuantity);
    }

    static KEGS_DELETE_ITEM_REQ read_from(Reader& r) {
        KEGS_DELETE_ITEM_REQ v;
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_iQuantity = Codec<std::remove_cvref_t<decltype(v.m_iQuantity)>>::get(r);
        return v;
    }
};

struct KEGS_DELETE_LETTER_ACK {
    std::int32_t              m_iOK{};  // int
    std::vector<std::int64_t> m_vecPostNo{};  // std::vector< UidType >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_vecPostNo)>>::put(w, m_vecPostNo);
    }

    static KEGS_DELETE_LETTER_ACK read_from(Reader& r) {
        KEGS_DELETE_LETTER_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_vecPostNo = Codec<std::remove_cvref_t<decltype(v.m_vecPostNo)>>::get(r);
        return v;
    }
};

struct KEGS_DELETE_LETTER_REQ {
    std::vector<std::int64_t> m_vecPostNo{};  // std::vector< UidType >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecPostNo)>>::put(w, m_vecPostNo);
    }

    static KEGS_DELETE_LETTER_REQ read_from(Reader& r) {
        KEGS_DELETE_LETTER_REQ v;
        v.m_vecPostNo = Codec<std::remove_cvref_t<decltype(v.m_vecPostNo)>>::get(r);
        return v;
    }
};

struct KEGS_DELETE_SECOND_SECURITY_VERIFY_REQ {
    std::u16string m_wstrSecondPW{};  // std::wstring
    bool           m_bDelete{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrSecondPW)>>::put(w, m_wstrSecondPW);
        Codec<std::remove_cvref_t<decltype(m_bDelete)>>::put(w, m_bDelete);
    }

    static KEGS_DELETE_SECOND_SECURITY_VERIFY_REQ read_from(Reader& r) {
        KEGS_DELETE_SECOND_SECURITY_VERIFY_REQ v;
        v.m_wstrSecondPW = Codec<std::remove_cvref_t<decltype(v.m_wstrSecondPW)>>::get(r);
        v.m_bDelete = Codec<std::remove_cvref_t<decltype(v.m_bDelete)>>::get(r);
        return v;
    }
};

struct KEGS_DELETE_TEMP_ITEM_ACK {
    std::int32_t m_iOK{};  // int
    std::int32_t m_iTempItemUID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iTempItemUID)>>::put(w, m_iTempItemUID);
    }

    static KEGS_DELETE_TEMP_ITEM_ACK read_from(Reader& r) {
        KEGS_DELETE_TEMP_ITEM_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iTempItemUID = Codec<std::remove_cvref_t<decltype(v.m_iTempItemUID)>>::get(r);
        return v;
    }
};

struct KEGS_DELETE_UNIT_ACK {
    std::int32_t   m_iOK{};  // int
    std::int64_t   m_iUnitUID{};  // UidType
    std::uint32_t  m_uiKNMSerialNum{};  // u_int
    std::int32_t   m_iGuildUID{};  // int
    std::u16string m_wstrNickName{};  // std::wstring
    std::uint8_t   m_ucLevel{};  // u_char
    char           m_cUnitClass{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_uiKNMSerialNum)>>::put(w, m_uiKNMSerialNum);
        Codec<std::remove_cvref_t<decltype(m_iGuildUID)>>::put(w, m_iGuildUID);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_ucLevel)>>::put(w, m_ucLevel);
        Codec<std::remove_cvref_t<decltype(m_cUnitClass)>>::put(w, m_cUnitClass);
    }

    static KEGS_DELETE_UNIT_ACK read_from(Reader& r) {
        KEGS_DELETE_UNIT_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_uiKNMSerialNum = Codec<std::remove_cvref_t<decltype(v.m_uiKNMSerialNum)>>::get(r);
        v.m_iGuildUID = Codec<std::remove_cvref_t<decltype(v.m_iGuildUID)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_ucLevel = Codec<std::remove_cvref_t<decltype(v.m_ucLevel)>>::get(r);
        v.m_cUnitClass = Codec<std::remove_cvref_t<decltype(v.m_cUnitClass)>>::get(r);
        return v;
    }
};

struct KEGS_DELETE_UNIT_REQ {
    std::int64_t m_iUnitUID{};  // UidType
    std::int64_t m_iUserUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
    }

    static KEGS_DELETE_UNIT_REQ read_from(Reader& r) {
        KEGS_DELETE_UNIT_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        return v;
    }
};

struct KEGS_DEL_BLACKLIST_USER_REQ {
    std::int64_t m_iUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
    }

    static KEGS_DEL_BLACKLIST_USER_REQ read_from(Reader& r) {
        KEGS_DEL_BLACKLIST_USER_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        return v;
    }
};

struct KEGS_DEL_COMMON_FLAG_NOT {
    std::uint32_t dwFlag{};  // DWORD

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(dwFlag)>>::put(w, dwFlag);
    }

    static KEGS_DEL_COMMON_FLAG_NOT read_from(Reader& r) {
        KEGS_DEL_COMMON_FLAG_NOT v;
        v.dwFlag = Codec<std::remove_cvref_t<decltype(v.dwFlag)>>::get(r);
        return v;
    }
};

struct KEGS_DEL_TUTORIAL_NOT {
    std::int64_t   m_iUnitUID{};  // UidType
    std::u16string m_wstrReason{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrReason)>>::put(w, m_wstrReason);
    }

    static KEGS_DEL_TUTORIAL_NOT read_from(Reader& r) {
        KEGS_DEL_TUTORIAL_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_wstrReason = Codec<std::remove_cvref_t<decltype(v.m_wstrReason)>>::get(r);
        return v;
    }
};

struct KEGS_DEL_TUTORIAL_REQ {
    std::int64_t m_iUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
    }

    static KEGS_DEL_TUTORIAL_REQ read_from(Reader& r) {
        KEGS_DEL_TUTORIAL_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        return v;
    }
};

struct KEGS_DENY_FRIEND_ACK {
    std::int32_t m_iOK{};  // int
    std::int64_t m_iUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
    }

    static KEGS_DENY_FRIEND_ACK read_from(Reader& r) {
        KEGS_DENY_FRIEND_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        return v;
    }
};

struct KEGS_DENY_FRIEND_NOT {
    std::int64_t   m_iUnitUID{};  // UidType
    std::u16string m_wstrNickName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
    }

    static KEGS_DENY_FRIEND_NOT read_from(Reader& r) {
        KEGS_DENY_FRIEND_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        return v;
    }
};

struct KEGS_DUNGEON_EFFECT_REMAINING_TIME_NOT {
    float m_fRemainingTime{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_fRemainingTime)>>::put(w, m_fRemainingTime);
    }

    static KEGS_DUNGEON_EFFECT_REMAINING_TIME_NOT read_from(Reader& r) {
        KEGS_DUNGEON_EFFECT_REMAINING_TIME_NOT v;
        v.m_fRemainingTime = Codec<std::remove_cvref_t<decltype(v.m_fRemainingTime)>>::get(r);
        return v;
    }
};

struct KEGS_DUNGEON_EFFECT_TIME_OUT_NOT {
    char m_cType{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cType)>>::put(w, m_cType);
    }

    static KEGS_DUNGEON_EFFECT_TIME_OUT_NOT read_from(Reader& r) {
        KEGS_DUNGEON_EFFECT_TIME_OUT_NOT v;
        v.m_cType = Codec<std::remove_cvref_t<decltype(v.m_cType)>>::get(r);
        return v;
    }
};

struct KEGS_DUNGEON_KILLALLNPC_CHECK_ACK {
    bool m_bResult{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bResult)>>::put(w, m_bResult);
    }

    static KEGS_DUNGEON_KILLALLNPC_CHECK_ACK read_from(Reader& r) {
        KEGS_DUNGEON_KILLALLNPC_CHECK_ACK v;
        v.m_bResult = Codec<std::remove_cvref_t<decltype(v.m_bResult)>>::get(r);
        return v;
    }
};

struct KEGS_DUNGEON_PLAY_INFO_TO_SERVER_NOT {
    std::int32_t m_ComboScore{};  // int
    std::int32_t m_TechScore{};  // int
    std::int32_t m_nDamageCount{};  // int
    std::int32_t m_iTotalAttackedDamage{};  // int
    std::int32_t m_iTotalRecoveryHP{};  // int
    std::int32_t m_iUsingKindOfSkill{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_ComboScore)>>::put(w, m_ComboScore);
        Codec<std::remove_cvref_t<decltype(m_TechScore)>>::put(w, m_TechScore);
        Codec<std::remove_cvref_t<decltype(m_nDamageCount)>>::put(w, m_nDamageCount);
        Codec<std::remove_cvref_t<decltype(m_iTotalAttackedDamage)>>::put(w, m_iTotalAttackedDamage);
        Codec<std::remove_cvref_t<decltype(m_iTotalRecoveryHP)>>::put(w, m_iTotalRecoveryHP);
        Codec<std::remove_cvref_t<decltype(m_iUsingKindOfSkill)>>::put(w, m_iUsingKindOfSkill);
    }

    static KEGS_DUNGEON_PLAY_INFO_TO_SERVER_NOT read_from(Reader& r) {
        KEGS_DUNGEON_PLAY_INFO_TO_SERVER_NOT v;
        v.m_ComboScore = Codec<std::remove_cvref_t<decltype(v.m_ComboScore)>>::get(r);
        v.m_TechScore = Codec<std::remove_cvref_t<decltype(v.m_TechScore)>>::get(r);
        v.m_nDamageCount = Codec<std::remove_cvref_t<decltype(v.m_nDamageCount)>>::get(r);
        v.m_iTotalAttackedDamage = Codec<std::remove_cvref_t<decltype(v.m_iTotalAttackedDamage)>>::get(r);
        v.m_iTotalRecoveryHP = Codec<std::remove_cvref_t<decltype(v.m_iTotalRecoveryHP)>>::get(r);
        v.m_iUsingKindOfSkill = Codec<std::remove_cvref_t<decltype(v.m_iUsingKindOfSkill)>>::get(r);
        return v;
    }
};

struct KEGS_DUNGEON_SECRET_STAGE_ENTER_CHECK_ACK {
    std::int32_t m_iOK{};  // int
    bool         m_bCanEnterSecretStage{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_bCanEnterSecretStage)>>::put(w, m_bCanEnterSecretStage);
    }

    static KEGS_DUNGEON_SECRET_STAGE_ENTER_CHECK_ACK read_from(Reader& r) {
        KEGS_DUNGEON_SECRET_STAGE_ENTER_CHECK_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_bCanEnterSecretStage = Codec<std::remove_cvref_t<decltype(v.m_bCanEnterSecretStage)>>::get(r);
        return v;
    }
};

struct KEGS_DUNGEON_SECRET_STAGE_ENTER_CHECK_NOT {
    std::int32_t m_iStartSecretStageEnteringEvent{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iStartSecretStageEnteringEvent)>>::put(w, m_iStartSecretStageEnteringEvent);
    }

    static KEGS_DUNGEON_SECRET_STAGE_ENTER_CHECK_NOT read_from(Reader& r) {
        KEGS_DUNGEON_SECRET_STAGE_ENTER_CHECK_NOT v;
        v.m_iStartSecretStageEnteringEvent = Codec<std::remove_cvref_t<decltype(v.m_iStartSecretStageEnteringEvent)>>::get(r);
        return v;
    }
};

struct KEGS_DUNGEON_SECRET_STAGE_ENTER_RESULT_NOT {
    bool m_bProceedToEnterSecretStage{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bProceedToEnterSecretStage)>>::put(w, m_bProceedToEnterSecretStage);
    }

    static KEGS_DUNGEON_SECRET_STAGE_ENTER_RESULT_NOT read_from(Reader& r) {
        KEGS_DUNGEON_SECRET_STAGE_ENTER_RESULT_NOT v;
        v.m_bProceedToEnterSecretStage = Codec<std::remove_cvref_t<decltype(v.m_bProceedToEnterSecretStage)>>::get(r);
        return v;
    }
};

struct KEGS_DUNGEON_SECRET_STAGE_ENTER_SELECT_NOT {
    bool m_bWantToEnterSecretStage{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bWantToEnterSecretStage)>>::put(w, m_bWantToEnterSecretStage);
    }

    static KEGS_DUNGEON_SECRET_STAGE_ENTER_SELECT_NOT read_from(Reader& r) {
        KEGS_DUNGEON_SECRET_STAGE_ENTER_SELECT_NOT v;
        v.m_bWantToEnterSecretStage = Codec<std::remove_cvref_t<decltype(v.m_bWantToEnterSecretStage)>>::get(r);
        return v;
    }
};

struct KEGS_DUNGEON_STAGE_LOAD_COMPLETE_NOT {
    std::int32_t m_iSlotID{};  // int
    std::int64_t m_iUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iSlotID)>>::put(w, m_iSlotID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
    }

    static KEGS_DUNGEON_STAGE_LOAD_COMPLETE_NOT read_from(Reader& r) {
        KEGS_DUNGEON_STAGE_LOAD_COMPLETE_NOT v;
        v.m_iSlotID = Codec<std::remove_cvref_t<decltype(v.m_iSlotID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        return v;
    }
};

struct KEGS_DUNGEON_STAGE_LOAD_COMPLETE_REQ {
    bool m_bIsSecretStage{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bIsSecretStage)>>::put(w, m_bIsSecretStage);
    }

    static KEGS_DUNGEON_STAGE_LOAD_COMPLETE_REQ read_from(Reader& r) {
        KEGS_DUNGEON_STAGE_LOAD_COMPLETE_REQ v;
        v.m_bIsSecretStage = Codec<std::remove_cvref_t<decltype(v.m_bIsSecretStage)>>::get(r);
        return v;
    }
};

struct KEGS_DUNGEON_STAGE_LOAD_REQ {
    std::int32_t m_iStageID{};  // int
    std::int16_t m_sPartyFever{};  // short

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iStageID)>>::put(w, m_iStageID);
        Codec<std::remove_cvref_t<decltype(m_sPartyFever)>>::put(w, m_sPartyFever);
    }

    static KEGS_DUNGEON_STAGE_LOAD_REQ read_from(Reader& r) {
        KEGS_DUNGEON_STAGE_LOAD_REQ v;
        v.m_iStageID = Codec<std::remove_cvref_t<decltype(v.m_iStageID)>>::get(r);
        v.m_sPartyFever = Codec<std::remove_cvref_t<decltype(v.m_sPartyFever)>>::get(r);
        return v;
    }
};

struct KEGS_DUNGEON_STAGE_START_NOT {
    std::int32_t m_iStageID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iStageID)>>::put(w, m_iStageID);
    }

    static KEGS_DUNGEON_STAGE_START_NOT read_from(Reader& r) {
        KEGS_DUNGEON_STAGE_START_NOT v;
        v.m_iStageID = Codec<std::remove_cvref_t<decltype(v.m_iStageID)>>::get(r);
        return v;
    }
};

struct KEGS_DUNGEON_SUB_STAGE_CLEAR_ACK {
    std::int32_t m_iOK{};  // int
    std::int32_t m_iClearType{};  // int
    std::int32_t m_iStageIndex{};  // int
    std::int32_t m_iSubStageIndex{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iClearType)>>::put(w, m_iClearType);
        Codec<std::remove_cvref_t<decltype(m_iStageIndex)>>::put(w, m_iStageIndex);
        Codec<std::remove_cvref_t<decltype(m_iSubStageIndex)>>::put(w, m_iSubStageIndex);
    }

    static KEGS_DUNGEON_SUB_STAGE_CLEAR_ACK read_from(Reader& r) {
        KEGS_DUNGEON_SUB_STAGE_CLEAR_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iClearType = Codec<std::remove_cvref_t<decltype(v.m_iClearType)>>::get(r);
        v.m_iStageIndex = Codec<std::remove_cvref_t<decltype(v.m_iStageIndex)>>::get(r);
        v.m_iSubStageIndex = Codec<std::remove_cvref_t<decltype(v.m_iSubStageIndex)>>::get(r);
        return v;
    }
};

struct KEGS_DUNGEON_SUB_STAGE_CLEAR_REQ {
    std::int32_t m_iClearConditionIndex{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iClearConditionIndex)>>::put(w, m_iClearConditionIndex);
    }

    static KEGS_DUNGEON_SUB_STAGE_CLEAR_REQ read_from(Reader& r) {
        KEGS_DUNGEON_SUB_STAGE_CLEAR_REQ v;
        v.m_iClearConditionIndex = Codec<std::remove_cvref_t<decltype(v.m_iClearConditionIndex)>>::get(r);
        return v;
    }
};

struct KEGS_DUNGEON_SUB_STAGE_GO_NEXT_ALL_NOT {
    char m_iBeforeStageID{};
    char m_iNextStageID{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iBeforeStageID)>>::put(w, m_iBeforeStageID);
        Codec<std::remove_cvref_t<decltype(m_iNextStageID)>>::put(w, m_iNextStageID);
    }

    static KEGS_DUNGEON_SUB_STAGE_GO_NEXT_ALL_NOT read_from(Reader& r) {
        KEGS_DUNGEON_SUB_STAGE_GO_NEXT_ALL_NOT v;
        v.m_iBeforeStageID = Codec<std::remove_cvref_t<decltype(v.m_iBeforeStageID)>>::get(r);
        v.m_iNextStageID = Codec<std::remove_cvref_t<decltype(v.m_iNextStageID)>>::get(r);
        return v;
    }
};

struct KEGS_DUNGEON_SUB_STAGE_GO_NEXT_REQ {
    char m_iBeforeStageID{};
    char m_iNextStageID{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iBeforeStageID)>>::put(w, m_iBeforeStageID);
        Codec<std::remove_cvref_t<decltype(m_iNextStageID)>>::put(w, m_iNextStageID);
    }

    static KEGS_DUNGEON_SUB_STAGE_GO_NEXT_REQ read_from(Reader& r) {
        KEGS_DUNGEON_SUB_STAGE_GO_NEXT_REQ v;
        v.m_iBeforeStageID = Codec<std::remove_cvref_t<decltype(v.m_iBeforeStageID)>>::get(r);
        v.m_iNextStageID = Codec<std::remove_cvref_t<decltype(v.m_iNextStageID)>>::get(r);
        return v;
    }
};

struct KEGS_DUNGEON_SUB_STAGE_LOAD_COMPLETE_REQ {
    char m_iSubStageID{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iSubStageID)>>::put(w, m_iSubStageID);
    }

    static KEGS_DUNGEON_SUB_STAGE_LOAD_COMPLETE_REQ read_from(Reader& r) {
        KEGS_DUNGEON_SUB_STAGE_LOAD_COMPLETE_REQ v;
        v.m_iSubStageID = Codec<std::remove_cvref_t<decltype(v.m_iSubStageID)>>::get(r);
        return v;
    }
};

struct KEGS_DUNGEON_SUB_STAGE_OPEN_NOT {
    char m_iSubStageID{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iSubStageID)>>::put(w, m_iSubStageID);
    }

    static KEGS_DUNGEON_SUB_STAGE_OPEN_NOT read_from(Reader& r) {
        KEGS_DUNGEON_SUB_STAGE_OPEN_NOT v;
        v.m_iSubStageID = Codec<std::remove_cvref_t<decltype(v.m_iSubStageID)>>::get(r);
        return v;
    }
};

struct KEGS_DUNGEON_SUB_STAGE_OPEN_REQ {
    char m_iSubStageID{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iSubStageID)>>::put(w, m_iSubStageID);
    }

    static KEGS_DUNGEON_SUB_STAGE_OPEN_REQ read_from(Reader& r) {
        KEGS_DUNGEON_SUB_STAGE_OPEN_REQ v;
        v.m_iSubStageID = Codec<std::remove_cvref_t<decltype(v.m_iSubStageID)>>::get(r);
        return v;
    }
};

struct KEGS_DUNGEON_SUB_STAGE_START_NOT {
    char m_iSubStageID{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iSubStageID)>>::put(w, m_iSubStageID);
    }

    static KEGS_DUNGEON_SUB_STAGE_START_NOT read_from(Reader& r) {
        KEGS_DUNGEON_SUB_STAGE_START_NOT v;
        v.m_iSubStageID = Codec<std::remove_cvref_t<decltype(v.m_iSubStageID)>>::get(r);
        return v;
    }
};

struct KEGS_EAT_SPECIAL_FEED_NOT {
    std::int64_t m_iUnitUID{};  // UidType
    std::int64_t m_iPetUID{};  // UidType
    std::int16_t m_sPercent{};  // short
    std::int32_t m_iCurrentIntimacy{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iPetUID)>>::put(w, m_iPetUID);
        Codec<std::remove_cvref_t<decltype(m_sPercent)>>::put(w, m_sPercent);
        Codec<std::remove_cvref_t<decltype(m_iCurrentIntimacy)>>::put(w, m_iCurrentIntimacy);
    }

    static KEGS_EAT_SPECIAL_FEED_NOT read_from(Reader& r) {
        KEGS_EAT_SPECIAL_FEED_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iPetUID = Codec<std::remove_cvref_t<decltype(v.m_iPetUID)>>::get(r);
        v.m_sPercent = Codec<std::remove_cvref_t<decltype(v.m_sPercent)>>::get(r);
        v.m_iCurrentIntimacy = Codec<std::remove_cvref_t<decltype(v.m_iCurrentIntimacy)>>::get(r);
        return v;
    }
};

struct KEGS_ED_MONITORING_BLOCK_ACK {
    std::int32_t m_iOK{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
    }

    static KEGS_ED_MONITORING_BLOCK_ACK read_from(Reader& r) {
        KEGS_ED_MONITORING_BLOCK_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        return v;
    }
};

struct KEGS_ED_MONITORING_BLOCK_REQ {
    std::int64_t m_iUnitUID{};  // UidType
    std::uint8_t m_ucType{};  // unsigned char

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_ucType)>>::put(w, m_ucType);
    }

    static KEGS_ED_MONITORING_BLOCK_REQ read_from(Reader& r) {
        KEGS_ED_MONITORING_BLOCK_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_ucType = Codec<std::remove_cvref_t<decltype(v.m_ucType)>>::get(r);
        return v;
    }
};

struct KEGS_ENCHANT_ATTACH_ITEM_REQ {
    std::int64_t m_iDestItemUID{};  // UidType
    std::int64_t m_iAttachItemUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iDestItemUID)>>::put(w, m_iDestItemUID);
        Codec<std::remove_cvref_t<decltype(m_iAttachItemUID)>>::put(w, m_iAttachItemUID);
    }

    static KEGS_ENCHANT_ATTACH_ITEM_REQ read_from(Reader& r) {
        KEGS_ENCHANT_ATTACH_ITEM_REQ v;
        v.m_iDestItemUID = Codec<std::remove_cvref_t<decltype(v.m_iDestItemUID)>>::get(r);
        v.m_iAttachItemUID = Codec<std::remove_cvref_t<decltype(v.m_iAttachItemUID)>>::get(r);
        return v;
    }
};

struct KEGS_ENCHANT_ITEM_REQ {
    std::int64_t m_iItemUID{};  // UidType
    bool         m_bIsRareEnchantStone{};
    bool         m_bIsNewEnchant{};
    bool         m_bIsSupportMaterial{};
    bool         m_bIsEnchantPlus{};
    bool         m_bIsDestroyGuard{};
    bool         m_bDebug{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_bIsRareEnchantStone)>>::put(w, m_bIsRareEnchantStone);
        Codec<std::remove_cvref_t<decltype(m_bIsNewEnchant)>>::put(w, m_bIsNewEnchant);
        Codec<std::remove_cvref_t<decltype(m_bIsSupportMaterial)>>::put(w, m_bIsSupportMaterial);
        Codec<std::remove_cvref_t<decltype(m_bIsEnchantPlus)>>::put(w, m_bIsEnchantPlus);
        Codec<std::remove_cvref_t<decltype(m_bIsDestroyGuard)>>::put(w, m_bIsDestroyGuard);
        Codec<std::remove_cvref_t<decltype(m_bDebug)>>::put(w, m_bDebug);
    }

    static KEGS_ENCHANT_ITEM_REQ read_from(Reader& r) {
        KEGS_ENCHANT_ITEM_REQ v;
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_bIsRareEnchantStone = Codec<std::remove_cvref_t<decltype(v.m_bIsRareEnchantStone)>>::get(r);
        v.m_bIsNewEnchant = Codec<std::remove_cvref_t<decltype(v.m_bIsNewEnchant)>>::get(r);
        v.m_bIsSupportMaterial = Codec<std::remove_cvref_t<decltype(v.m_bIsSupportMaterial)>>::get(r);
        v.m_bIsEnchantPlus = Codec<std::remove_cvref_t<decltype(v.m_bIsEnchantPlus)>>::get(r);
        v.m_bIsDestroyGuard = Codec<std::remove_cvref_t<decltype(v.m_bIsDestroyGuard)>>::get(r);
        v.m_bDebug = Codec<std::remove_cvref_t<decltype(v.m_bDebug)>>::get(r);
        return v;
    }
};

struct KEGS_END_GAME_REQ {
    std::int32_t m_iWinTeam{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iWinTeam)>>::put(w, m_iWinTeam);
    }

    static KEGS_END_GAME_REQ read_from(Reader& r) {
        KEGS_END_GAME_REQ v;
        v.m_iWinTeam = Codec<std::remove_cvref_t<decltype(v.m_iWinTeam)>>::get(r);
        return v;
    }
};

struct KEGS_END_TC_GAME_REQ {
    bool m_bIsSuccess{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bIsSuccess)>>::put(w, m_bIsSuccess);
    }

    static KEGS_END_TC_GAME_REQ read_from(Reader& r) {
        KEGS_END_TC_GAME_REQ v;
        v.m_bIsSuccess = Codec<std::remove_cvref_t<decltype(v.m_bIsSuccess)>>::get(r);
        return v;
    }
};

struct KEGS_ENTER_DEFENCE_DUNGEON_POPUP_REPLY_NOT {
    bool m_bAgree{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bAgree)>>::put(w, m_bAgree);
    }

    static KEGS_ENTER_DEFENCE_DUNGEON_POPUP_REPLY_NOT read_from(Reader& r) {
        KEGS_ENTER_DEFENCE_DUNGEON_POPUP_REPLY_NOT v;
        v.m_bAgree = Codec<std::remove_cvref_t<decltype(v.m_bAgree)>>::get(r);
        return v;
    }
};

struct KEGS_ENTER_NPC_HOUSE_REQ {
    std::int32_t m_iNPCID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iNPCID)>>::put(w, m_iNPCID);
    }

    static KEGS_ENTER_NPC_HOUSE_REQ read_from(Reader& r) {
        KEGS_ENTER_NPC_HOUSE_REQ v;
        v.m_iNPCID = Codec<std::remove_cvref_t<decltype(v.m_iNPCID)>>::get(r);
        return v;
    }
};

struct KEGS_ENTRY_POINT_CHANGE_NICK_NAME_REQ {
    std::int32_t   m_iServerGroup{};  // int
    std::int64_t   m_iUnitUID{};  // UidType
    std::u16string m_wstrNickName{};  // std::wstring
    bool           m_bCheckOnly{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iServerGroup)>>::put(w, m_iServerGroup);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_bCheckOnly)>>::put(w, m_bCheckOnly);
    }

    static KEGS_ENTRY_POINT_CHANGE_NICK_NAME_REQ read_from(Reader& r) {
        KEGS_ENTRY_POINT_CHANGE_NICK_NAME_REQ v;
        v.m_iServerGroup = Codec<std::remove_cvref_t<decltype(v.m_iServerGroup)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_bCheckOnly = Codec<std::remove_cvref_t<decltype(v.m_bCheckOnly)>>::get(r);
        return v;
    }
};

struct KEGS_ENTRY_POINT_CHECK_NICK_NAME_REQ {
    std::int32_t   m_iServerGroup{};  // int
    std::u16string m_wstrNickName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iServerGroup)>>::put(w, m_iServerGroup);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
    }

    static KEGS_ENTRY_POINT_CHECK_NICK_NAME_REQ read_from(Reader& r) {
        KEGS_ENTRY_POINT_CHECK_NICK_NAME_REQ v;
        v.m_iServerGroup = Codec<std::remove_cvref_t<decltype(v.m_iServerGroup)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        return v;
    }
};

struct KEGS_ENTRY_POINT_DELETE_UNIT_REQ {
    std::int64_t m_iUnitUID{};  // UidType
    std::int64_t m_iUserUID{};  // UidType
    std::int32_t m_iServerGroup{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
        Codec<std::remove_cvref_t<decltype(m_iServerGroup)>>::put(w, m_iServerGroup);
    }

    static KEGS_ENTRY_POINT_DELETE_UNIT_REQ read_from(Reader& r) {
        KEGS_ENTRY_POINT_DELETE_UNIT_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        v.m_iServerGroup = Codec<std::remove_cvref_t<decltype(v.m_iServerGroup)>>::get(r);
        return v;
    }
};

struct KEGS_ENTRY_POINT_GET_CHANNEL_LIST_ACK {
    std::map<std::int32_t, KChannelInfo>      m_mapSolesChannelList{};  // std::map< int, KChannelInfo >
    std::map<std::int32_t, KChannelInfo>      m_mapGaiaChannelList{};  // std::map< int, KChannelInfo >
    std::map<std::int32_t, KChannelBonusInfo> m_mapSolesChannelBonusList{};  // std::map< int, KChannelBonusInfo >
    std::map<std::int32_t, KChannelBonusInfo> m_mapGaiaChannelBonusList{};  // std::map< int, KChannelBonusInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_mapSolesChannelList)>>::put(w, m_mapSolesChannelList);
        Codec<std::remove_cvref_t<decltype(m_mapGaiaChannelList)>>::put(w, m_mapGaiaChannelList);
        Codec<std::remove_cvref_t<decltype(m_mapSolesChannelBonusList)>>::put(w, m_mapSolesChannelBonusList);
        Codec<std::remove_cvref_t<decltype(m_mapGaiaChannelBonusList)>>::put(w, m_mapGaiaChannelBonusList);
    }

    static KEGS_ENTRY_POINT_GET_CHANNEL_LIST_ACK read_from(Reader& r) {
        KEGS_ENTRY_POINT_GET_CHANNEL_LIST_ACK v;
        v.m_mapSolesChannelList = Codec<std::remove_cvref_t<decltype(v.m_mapSolesChannelList)>>::get(r);
        v.m_mapGaiaChannelList = Codec<std::remove_cvref_t<decltype(v.m_mapGaiaChannelList)>>::get(r);
        v.m_mapSolesChannelBonusList = Codec<std::remove_cvref_t<decltype(v.m_mapSolesChannelBonusList)>>::get(r);
        v.m_mapGaiaChannelBonusList = Codec<std::remove_cvref_t<decltype(v.m_mapGaiaChannelBonusList)>>::get(r);
        return v;
    }
};

struct KEGS_EQUIP_TITLE_ACK {
    std::int32_t m_iOK{};  // int
    std::int32_t m_iTitleID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iTitleID)>>::put(w, m_iTitleID);
    }

    static KEGS_EQUIP_TITLE_ACK read_from(Reader& r) {
        KEGS_EQUIP_TITLE_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iTitleID = Codec<std::remove_cvref_t<decltype(v.m_iTitleID)>>::get(r);
        return v;
    }
};

struct KEGS_EQUIP_TITLE_REQ {
    std::int32_t m_iTitleID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iTitleID)>>::put(w, m_iTitleID);
    }

    static KEGS_EQUIP_TITLE_REQ read_from(Reader& r) {
        KEGS_EQUIP_TITLE_REQ v;
        v.m_iTitleID = Codec<std::remove_cvref_t<decltype(v.m_iTitleID)>>::get(r);
        return v;
    }
};

struct KEGS_ERROR_POPUP_NOT {
    std::int32_t m_iOK{};  // int
    std::int32_t m_iErrorData1{};  // int
    std::int32_t m_iErrorData2{};  // int
    bool         m_bPartyMember{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iErrorData1)>>::put(w, m_iErrorData1);
        Codec<std::remove_cvref_t<decltype(m_iErrorData2)>>::put(w, m_iErrorData2);
        Codec<std::remove_cvref_t<decltype(m_bPartyMember)>>::put(w, m_bPartyMember);
    }

    static KEGS_ERROR_POPUP_NOT read_from(Reader& r) {
        KEGS_ERROR_POPUP_NOT v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iErrorData1 = Codec<std::remove_cvref_t<decltype(v.m_iErrorData1)>>::get(r);
        v.m_iErrorData2 = Codec<std::remove_cvref_t<decltype(v.m_iErrorData2)>>::get(r);
        v.m_bPartyMember = Codec<std::remove_cvref_t<decltype(v.m_bPartyMember)>>::get(r);
        return v;
    }
};

struct KEGS_EVENT_QUEST_CHECK_FOR_ADMIN_NOT {
    std::vector<std::int32_t> m_vecWarningQuestID{};  // std::vector<int>

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecWarningQuestID)>>::put(w, m_vecWarningQuestID);
    }

    static KEGS_EVENT_QUEST_CHECK_FOR_ADMIN_NOT read_from(Reader& r) {
        KEGS_EVENT_QUEST_CHECK_FOR_ADMIN_NOT v;
        v.m_vecWarningQuestID = Codec<std::remove_cvref_t<decltype(v.m_vecWarningQuestID)>>::get(r);
        return v;
    }
};

struct KEGS_EVENT_QUEST_INFO_NOT {
    char                      m_cType{};
    std::vector<std::int32_t> m_vecQuestID{};  // std::vector< int >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cType)>>::put(w, m_cType);
        Codec<std::remove_cvref_t<decltype(m_vecQuestID)>>::put(w, m_vecQuestID);
    }

    static KEGS_EVENT_QUEST_INFO_NOT read_from(Reader& r) {
        KEGS_EVENT_QUEST_INFO_NOT v;
        v.m_cType = Codec<std::remove_cvref_t<decltype(v.m_cType)>>::get(r);
        v.m_vecQuestID = Codec<std::remove_cvref_t<decltype(v.m_vecQuestID)>>::get(r);
        return v;
    }
};

struct KEGS_EXCHANGE_CASH_REQ {
    std::int32_t m_uiExchangeValue{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_uiExchangeValue)>>::put(w, m_uiExchangeValue);
    }

    static KEGS_EXCHANGE_CASH_REQ read_from(Reader& r) {
        KEGS_EXCHANGE_CASH_REQ v;
        v.m_uiExchangeValue = Codec<std::remove_cvref_t<decltype(v.m_uiExchangeValue)>>::get(r);
        return v;
    }
};

struct KEGS_EXPAND_BANK_SLOT_ED_ACK {
    std::int32_t                         m_iOK{};  // int
    std::int32_t                         m_iED{};  // int
    std::map<std::int32_t, std::int32_t> m_mapExpandedCategorySlot{};  // std::map< int, int >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iED)>>::put(w, m_iED);
        Codec<std::remove_cvref_t<decltype(m_mapExpandedCategorySlot)>>::put(w, m_mapExpandedCategorySlot);
    }

    static KEGS_EXPAND_BANK_SLOT_ED_ACK read_from(Reader& r) {
        KEGS_EXPAND_BANK_SLOT_ED_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iED = Codec<std::remove_cvref_t<decltype(v.m_iED)>>::get(r);
        v.m_mapExpandedCategorySlot = Codec<std::remove_cvref_t<decltype(v.m_mapExpandedCategorySlot)>>::get(r);
        return v;
    }
};

struct KEGS_EXPAND_BANK_SLOT_NOT {
    std::map<std::int32_t, std::int32_t> m_mapExpandedCategorySlot{};  // std::map< int, int >
    std::int32_t                         m_iOK{};  // int
    std::int32_t                         m_iED{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_mapExpandedCategorySlot)>>::put(w, m_mapExpandedCategorySlot);
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iED)>>::put(w, m_iED);
    }

    static KEGS_EXPAND_BANK_SLOT_NOT read_from(Reader& r) {
        KEGS_EXPAND_BANK_SLOT_NOT v;
        v.m_mapExpandedCategorySlot = Codec<std::remove_cvref_t<decltype(v.m_mapExpandedCategorySlot)>>::get(r);
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iED = Codec<std::remove_cvref_t<decltype(v.m_iED)>>::get(r);
        return v;
    }
};

struct KEGS_EXPAND_CHAR_SLOT_NOT {
    std::int32_t m_iCharSlotSize{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iCharSlotSize)>>::put(w, m_iCharSlotSize);
    }

    static KEGS_EXPAND_CHAR_SLOT_NOT read_from(Reader& r) {
        KEGS_EXPAND_CHAR_SLOT_NOT v;
        v.m_iCharSlotSize = Codec<std::remove_cvref_t<decltype(v.m_iCharSlotSize)>>::get(r);
        return v;
    }
};

struct KEGS_EXPAND_SKILL_NOTE_PAGE_NUM_NOT {
    char m_cExpandedPageNum{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cExpandedPageNum)>>::put(w, m_cExpandedPageNum);
    }

    static KEGS_EXPAND_SKILL_NOTE_PAGE_NUM_NOT read_from(Reader& r) {
        KEGS_EXPAND_SKILL_NOTE_PAGE_NUM_NOT v;
        v.m_cExpandedPageNum = Codec<std::remove_cvref_t<decltype(v.m_cExpandedPageNum)>>::get(r);
        return v;
    }
};

struct KEGS_EXPAND_SKILL_PAGE_ACK {
    std::int32_t              m_iOK{};  // int
    std::int32_t              m_iED{};  // int
    std::int32_t              m_iTheNumberOfSkillPagesAvailable{};  // int
    std::int32_t              m_iSPointAvailable{};  // int
    std::int32_t              m_iCSPointAvailable{};  // int
    std::vector<std::int32_t> m_vecSkillListLearned{};  // std::vector<int>
    std::vector<std::int16_t> m_vecUnsealedSkillID{};  // std::vector< short >
    bool                      m_bPayWithED{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iED)>>::put(w, m_iED);
        Codec<std::remove_cvref_t<decltype(m_iTheNumberOfSkillPagesAvailable)>>::put(w, m_iTheNumberOfSkillPagesAvailable);
        Codec<std::remove_cvref_t<decltype(m_iSPointAvailable)>>::put(w, m_iSPointAvailable);
        Codec<std::remove_cvref_t<decltype(m_iCSPointAvailable)>>::put(w, m_iCSPointAvailable);
        Codec<std::remove_cvref_t<decltype(m_vecSkillListLearned)>>::put(w, m_vecSkillListLearned);
        Codec<std::remove_cvref_t<decltype(m_vecUnsealedSkillID)>>::put(w, m_vecUnsealedSkillID);
        Codec<std::remove_cvref_t<decltype(m_bPayWithED)>>::put(w, m_bPayWithED);
    }

    static KEGS_EXPAND_SKILL_PAGE_ACK read_from(Reader& r) {
        KEGS_EXPAND_SKILL_PAGE_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iED = Codec<std::remove_cvref_t<decltype(v.m_iED)>>::get(r);
        v.m_iTheNumberOfSkillPagesAvailable = Codec<std::remove_cvref_t<decltype(v.m_iTheNumberOfSkillPagesAvailable)>>::get(r);
        v.m_iSPointAvailable = Codec<std::remove_cvref_t<decltype(v.m_iSPointAvailable)>>::get(r);
        v.m_iCSPointAvailable = Codec<std::remove_cvref_t<decltype(v.m_iCSPointAvailable)>>::get(r);
        v.m_vecSkillListLearned = Codec<std::remove_cvref_t<decltype(v.m_vecSkillListLearned)>>::get(r);
        v.m_vecUnsealedSkillID = Codec<std::remove_cvref_t<decltype(v.m_vecUnsealedSkillID)>>::get(r);
        v.m_bPayWithED = Codec<std::remove_cvref_t<decltype(v.m_bPayWithED)>>::get(r);
        return v;
    }
};

struct KEGS_EXPAND_SKILL_SLOT_NOT {
    std::int64_t   m_iUnitUID{};  // UidType
    char           m_cSkillSlotBExpirationState{};
    std::u16string m_wstrSkillSlotBEndDate{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_cSkillSlotBExpirationState)>>::put(w, m_cSkillSlotBExpirationState);
        Codec<std::remove_cvref_t<decltype(m_wstrSkillSlotBEndDate)>>::put(w, m_wstrSkillSlotBEndDate);
    }

    static KEGS_EXPAND_SKILL_SLOT_NOT read_from(Reader& r) {
        KEGS_EXPAND_SKILL_SLOT_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_cSkillSlotBExpirationState = Codec<std::remove_cvref_t<decltype(v.m_cSkillSlotBExpirationState)>>::get(r);
        v.m_wstrSkillSlotBEndDate = Codec<std::remove_cvref_t<decltype(v.m_wstrSkillSlotBEndDate)>>::get(r);
        return v;
    }
};

struct KEGS_EXPIRE_CASH_SKILL_POINT_NOT {
    std::int64_t                                      m_iUnitUID{};  // UidType
    std::vector<std::int32_t>                         m_vecRetrievedSPoint{};  // std::vector<int>
    std::vector<std::map<std::int32_t, std::int32_t>> m_vecMapHaveSKill{};  // std::vector< std::map<int,int> >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_vecRetrievedSPoint)>>::put(w, m_vecRetrievedSPoint);
        Codec<std::remove_cvref_t<decltype(m_vecMapHaveSKill)>>::put(w, m_vecMapHaveSKill);
    }

    static KEGS_EXPIRE_CASH_SKILL_POINT_NOT read_from(Reader& r) {
        KEGS_EXPIRE_CASH_SKILL_POINT_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_vecRetrievedSPoint = Codec<std::remove_cvref_t<decltype(v.m_vecRetrievedSPoint)>>::get(r);
        v.m_vecMapHaveSKill = Codec<std::remove_cvref_t<decltype(v.m_vecMapHaveSKill)>>::get(r);
        return v;
    }
};

struct KEGS_FEED_PETS_REQ {
    std::int64_t m_iItemUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
    }

    static KEGS_FEED_PETS_REQ read_from(Reader& r) {
        KEGS_FEED_PETS_REQ v;
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        return v;
    }
};

struct KEGS_FIRST_FEED_PETS_NOT {
    bool m_bAutoFeed{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bAutoFeed)>>::put(w, m_bAutoFeed);
    }

    static KEGS_FIRST_FEED_PETS_NOT read_from(Reader& r) {
        KEGS_FIRST_FEED_PETS_NOT v;
        v.m_bAutoFeed = Codec<std::remove_cvref_t<decltype(v.m_bAutoFeed)>>::get(r);
        return v;
    }
};

struct KEGS_FORCED_EXIT_VOTE_ACK {
    std::int64_t m_iUserUID{};  // UidType
    std::int32_t m_iOK{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
    }

    static KEGS_FORCED_EXIT_VOTE_ACK read_from(Reader& r) {
        KEGS_FORCED_EXIT_VOTE_ACK v;
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        return v;
    }
};

struct KEGS_FORCED_EXIT_VOTE_REQ {
    std::int64_t m_iUserUID{};  // UidType
    bool         m_bWantForceExit{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
        Codec<std::remove_cvref_t<decltype(m_bWantForceExit)>>::put(w, m_bWantForceExit);
    }

    static KEGS_FORCED_EXIT_VOTE_REQ read_from(Reader& r) {
        KEGS_FORCED_EXIT_VOTE_REQ v;
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        v.m_bWantForceExit = Codec<std::remove_cvref_t<decltype(v.m_bWantForceExit)>>::get(r);
        return v;
    }
};

struct KEGS_FORCE_RELAY_NOT {
    std::int64_t m_iTargetUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iTargetUnitUID)>>::put(w, m_iTargetUnitUID);
    }

    static KEGS_FORCE_RELAY_NOT read_from(Reader& r) {
        KEGS_FORCE_RELAY_NOT v;
        v.m_iTargetUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iTargetUnitUID)>>::get(r);
        return v;
    }
};

struct KEGS_FRAME_AVERAGE_REQ {
    char         m_cGameType{};
    std::int32_t m_iTotalFrame{};  // int
    std::int32_t m_iFrameAverage{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cGameType)>>::put(w, m_cGameType);
        Codec<std::remove_cvref_t<decltype(m_iTotalFrame)>>::put(w, m_iTotalFrame);
        Codec<std::remove_cvref_t<decltype(m_iFrameAverage)>>::put(w, m_iFrameAverage);
    }

    static KEGS_FRAME_AVERAGE_REQ read_from(Reader& r) {
        KEGS_FRAME_AVERAGE_REQ v;
        v.m_cGameType = Codec<std::remove_cvref_t<decltype(v.m_cGameType)>>::get(r);
        v.m_iTotalFrame = Codec<std::remove_cvref_t<decltype(v.m_iTotalFrame)>>::get(r);
        v.m_iFrameAverage = Codec<std::remove_cvref_t<decltype(v.m_iFrameAverage)>>::get(r);
        return v;
    }
};

struct KEGS_FRIEND_POSITION_UPDATE_NOT {
    std::map<std::int64_t, char> m_mapFriendPosition{};  // std::map< UidType, char >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_mapFriendPosition)>>::put(w, m_mapFriendPosition);
    }

    static KEGS_FRIEND_POSITION_UPDATE_NOT read_from(Reader& r) {
        KEGS_FRIEND_POSITION_UPDATE_NOT v;
        v.m_mapFriendPosition = Codec<std::remove_cvref_t<decltype(v.m_mapFriendPosition)>>::get(r);
        return v;
    }
};

struct KEGS_GAME_LOADING_NOT {
    std::int64_t m_iUnitUID{};  // UidType
    std::int32_t m_iLoadingProgress{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iLoadingProgress)>>::put(w, m_iLoadingProgress);
    }

    static KEGS_GAME_LOADING_NOT read_from(Reader& r) {
        KEGS_GAME_LOADING_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iLoadingProgress = Codec<std::remove_cvref_t<decltype(v.m_iLoadingProgress)>>::get(r);
        return v;
    }
};

struct KEGS_GAME_LOADING_REQ {
    std::int32_t m_iLoadingProgress{};  // int
    std::int32_t m_iUnitLevel{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iLoadingProgress)>>::put(w, m_iLoadingProgress);
        Codec<std::remove_cvref_t<decltype(m_iUnitLevel)>>::put(w, m_iUnitLevel);
    }

    static KEGS_GAME_LOADING_REQ read_from(Reader& r) {
        KEGS_GAME_LOADING_REQ v;
        v.m_iLoadingProgress = Codec<std::remove_cvref_t<decltype(v.m_iLoadingProgress)>>::get(r);
        v.m_iUnitLevel = Codec<std::remove_cvref_t<decltype(v.m_iUnitLevel)>>::get(r);
        return v;
    }
};

struct KEGS_GAME_P2P_CONNECT_RESULT_ACK {
    std::int32_t m_iOK{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
    }

    static KEGS_GAME_P2P_CONNECT_RESULT_ACK read_from(Reader& r) {
        KEGS_GAME_P2P_CONNECT_RESULT_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        return v;
    }
};

struct KEGS_GAME_P2P_CONNECT_RESULT_REQ {
    bool         m_bConnectSuccess{};
    std::int64_t m_UnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bConnectSuccess)>>::put(w, m_bConnectSuccess);
        Codec<std::remove_cvref_t<decltype(m_UnitUID)>>::put(w, m_UnitUID);
    }

    static KEGS_GAME_P2P_CONNECT_RESULT_REQ read_from(Reader& r) {
        KEGS_GAME_P2P_CONNECT_RESULT_REQ v;
        v.m_bConnectSuccess = Codec<std::remove_cvref_t<decltype(v.m_bConnectSuccess)>>::get(r);
        v.m_UnitUID = Codec<std::remove_cvref_t<decltype(v.m_UnitUID)>>::get(r);
        return v;
    }
};

struct KEGS_GAME_START_TIME_COUNT_NOT {
    std::int64_t m_iRoomUID{};  // UidType
    char         m_cSecondCount{};
    bool         m_bIsDungeonRoom{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iRoomUID)>>::put(w, m_iRoomUID);
        Codec<std::remove_cvref_t<decltype(m_cSecondCount)>>::put(w, m_cSecondCount);
        Codec<std::remove_cvref_t<decltype(m_bIsDungeonRoom)>>::put(w, m_bIsDungeonRoom);
    }

    static KEGS_GAME_START_TIME_COUNT_NOT read_from(Reader& r) {
        KEGS_GAME_START_TIME_COUNT_NOT v;
        v.m_iRoomUID = Codec<std::remove_cvref_t<decltype(v.m_iRoomUID)>>::get(r);
        v.m_cSecondCount = Codec<std::remove_cvref_t<decltype(v.m_cSecondCount)>>::get(r);
        v.m_bIsDungeonRoom = Codec<std::remove_cvref_t<decltype(v.m_bIsDungeonRoom)>>::get(r);
        return v;
    }
};

struct KEGS_GATHER_GIVE_UP_QUEST_REQ {
    std::vector<std::int32_t> m_vecGatherQuestID{};  // std::vector< int >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecGatherQuestID)>>::put(w, m_vecGatherQuestID);
    }

    static KEGS_GATHER_GIVE_UP_QUEST_REQ read_from(Reader& r) {
        KEGS_GATHER_GIVE_UP_QUEST_REQ v;
        v.m_vecGatherQuestID = Codec<std::remove_cvref_t<decltype(v.m_vecGatherQuestID)>>::get(r);
        return v;
    }
};

struct KEGS_GET_ADVERTISEMENT_EVENT_ACK {
    std::vector<std::u16string> m_vecAdvertisementURL{};  // std::vector< std::wstring >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecAdvertisementURL)>>::put(w, m_vecAdvertisementURL);
    }

    static KEGS_GET_ADVERTISEMENT_EVENT_ACK read_from(Reader& r) {
        KEGS_GET_ADVERTISEMENT_EVENT_ACK v;
        v.m_vecAdvertisementURL = Codec<std::remove_cvref_t<decltype(v.m_vecAdvertisementURL)>>::get(r);
        return v;
    }
};

struct KEGS_GET_APPLY_JOIN_GUILD_LIST_ACK {
    std::int32_t                     m_iOK{};  // int
    std::u16string                   m_wstrGuildName{};  // std::wstring
    std::uint32_t                    m_uiTotalPage{};  // u_int
    std::uint32_t                    m_uiViewPage{};  // u_int
    std::vector<KApplyJoinGuildInfo> m_vecApplyJoinGuildList{};  // std::vector< KApplyJoinGuildInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_wstrGuildName)>>::put(w, m_wstrGuildName);
        Codec<std::remove_cvref_t<decltype(m_uiTotalPage)>>::put(w, m_uiTotalPage);
        Codec<std::remove_cvref_t<decltype(m_uiViewPage)>>::put(w, m_uiViewPage);
        Codec<std::remove_cvref_t<decltype(m_vecApplyJoinGuildList)>>::put(w, m_vecApplyJoinGuildList);
    }

    static KEGS_GET_APPLY_JOIN_GUILD_LIST_ACK read_from(Reader& r) {
        KEGS_GET_APPLY_JOIN_GUILD_LIST_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_wstrGuildName = Codec<std::remove_cvref_t<decltype(v.m_wstrGuildName)>>::get(r);
        v.m_uiTotalPage = Codec<std::remove_cvref_t<decltype(v.m_uiTotalPage)>>::get(r);
        v.m_uiViewPage = Codec<std::remove_cvref_t<decltype(v.m_uiViewPage)>>::get(r);
        v.m_vecApplyJoinGuildList = Codec<std::remove_cvref_t<decltype(v.m_vecApplyJoinGuildList)>>::get(r);
        return v;
    }
};

struct KEGS_GET_APPLY_JOIN_GUILD_LIST_REQ {
    std::uint32_t m_uiViewPage{};  // u_int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_uiViewPage)>>::put(w, m_uiViewPage);
    }

    static KEGS_GET_APPLY_JOIN_GUILD_LIST_REQ read_from(Reader& r) {
        KEGS_GET_APPLY_JOIN_GUILD_LIST_REQ v;
        v.m_uiViewPage = Codec<std::remove_cvref_t<decltype(v.m_uiViewPage)>>::get(r);
        return v;
    }
};

struct KEGS_GET_CHANNEL_LIST_ACK {
    std::int32_t                              m_iServerGroupID{};  // int
    std::vector<KChannelInfo>                 m_vecChannelInfo{};  // std::vector< KChannelInfo >
    std::map<std::int32_t, KChannelBonusInfo> m_mapChannelBonusInfo{};  // std::map< int, KChannelBonusInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iServerGroupID)>>::put(w, m_iServerGroupID);
        Codec<std::remove_cvref_t<decltype(m_vecChannelInfo)>>::put(w, m_vecChannelInfo);
        Codec<std::remove_cvref_t<decltype(m_mapChannelBonusInfo)>>::put(w, m_mapChannelBonusInfo);
    }

    static KEGS_GET_CHANNEL_LIST_ACK read_from(Reader& r) {
        KEGS_GET_CHANNEL_LIST_ACK v;
        v.m_iServerGroupID = Codec<std::remove_cvref_t<decltype(v.m_iServerGroupID)>>::get(r);
        v.m_vecChannelInfo = Codec<std::remove_cvref_t<decltype(v.m_vecChannelInfo)>>::get(r);
        v.m_mapChannelBonusInfo = Codec<std::remove_cvref_t<decltype(v.m_mapChannelBonusInfo)>>::get(r);
        return v;
    }
};

struct KEGS_GET_CREATE_UNIT_TODAY_COUNT_ACK {
    std::int64_t                         m_iUserUID{};  // UidType
    std::map<std::int32_t, std::int32_t> m_mapCreateCharCountToday{};  // std::map< int, int >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
        Codec<std::remove_cvref_t<decltype(m_mapCreateCharCountToday)>>::put(w, m_mapCreateCharCountToday);
    }

    static KEGS_GET_CREATE_UNIT_TODAY_COUNT_ACK read_from(Reader& r) {
        KEGS_GET_CREATE_UNIT_TODAY_COUNT_ACK v;
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        v.m_mapCreateCharCountToday = Codec<std::remove_cvref_t<decltype(v.m_mapCreateCharCountToday)>>::get(r);
        return v;
    }
};

struct KEGS_GET_GUILD_AD_LIST_REQ {
    char          m_cSortType{};
    std::uint32_t m_uiViewPage{};  // u_int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cSortType)>>::put(w, m_cSortType);
        Codec<std::remove_cvref_t<decltype(m_uiViewPage)>>::put(w, m_uiViewPage);
    }

    static KEGS_GET_GUILD_AD_LIST_REQ read_from(Reader& r) {
        KEGS_GET_GUILD_AD_LIST_REQ v;
        v.m_cSortType = Codec<std::remove_cvref_t<decltype(v.m_cSortType)>>::get(r);
        v.m_uiViewPage = Codec<std::remove_cvref_t<decltype(v.m_uiViewPage)>>::get(r);
        return v;
    }
};

struct KEGS_GET_GUILD_SKILL_IN_BOARD_REQ {
    std::int32_t m_iGuildUID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iGuildUID)>>::put(w, m_iGuildUID);
    }

    static KEGS_GET_GUILD_SKILL_IN_BOARD_REQ read_from(Reader& r) {
        KEGS_GET_GUILD_SKILL_IN_BOARD_REQ v;
        v.m_iGuildUID = Codec<std::remove_cvref_t<decltype(v.m_iGuildUID)>>::get(r);
        return v;
    }
};

struct KEGS_GET_GUILD_SKILL_NOT {
    std::int32_t m_iGuildSkillID{};  // int
    std::int32_t m_iGuildSkillLv{};  // int
    std::int32_t m_iGuildSkillCSPoint{};  // int
    std::int32_t m_iGuildSPoint{};  // int
    std::int32_t m_iGuildCSPoint{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iGuildSkillID)>>::put(w, m_iGuildSkillID);
        Codec<std::remove_cvref_t<decltype(m_iGuildSkillLv)>>::put(w, m_iGuildSkillLv);
        Codec<std::remove_cvref_t<decltype(m_iGuildSkillCSPoint)>>::put(w, m_iGuildSkillCSPoint);
        Codec<std::remove_cvref_t<decltype(m_iGuildSPoint)>>::put(w, m_iGuildSPoint);
        Codec<std::remove_cvref_t<decltype(m_iGuildCSPoint)>>::put(w, m_iGuildCSPoint);
    }

    static KEGS_GET_GUILD_SKILL_NOT read_from(Reader& r) {
        KEGS_GET_GUILD_SKILL_NOT v;
        v.m_iGuildSkillID = Codec<std::remove_cvref_t<decltype(v.m_iGuildSkillID)>>::get(r);
        v.m_iGuildSkillLv = Codec<std::remove_cvref_t<decltype(v.m_iGuildSkillLv)>>::get(r);
        v.m_iGuildSkillCSPoint = Codec<std::remove_cvref_t<decltype(v.m_iGuildSkillCSPoint)>>::get(r);
        v.m_iGuildSPoint = Codec<std::remove_cvref_t<decltype(v.m_iGuildSPoint)>>::get(r);
        v.m_iGuildCSPoint = Codec<std::remove_cvref_t<decltype(v.m_iGuildCSPoint)>>::get(r);
        return v;
    }
};

struct KEGS_GET_GUILD_SKILL_REQ {
    std::int32_t m_iGuildSkillID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iGuildSkillID)>>::put(w, m_iGuildSkillID);
    }

    static KEGS_GET_GUILD_SKILL_REQ read_from(Reader& r) {
        KEGS_GET_GUILD_SKILL_REQ v;
        v.m_iGuildSkillID = Codec<std::remove_cvref_t<decltype(v.m_iGuildSkillID)>>::get(r);
        return v;
    }
};

struct KEGS_GET_GUILD_USER_LIST_REQ {
    std::uint32_t m_uiViewPage{};  // UINT
    char          m_cGuildUserListType{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_uiViewPage)>>::put(w, m_uiViewPage);
        Codec<std::remove_cvref_t<decltype(m_cGuildUserListType)>>::put(w, m_cGuildUserListType);
    }

    static KEGS_GET_GUILD_USER_LIST_REQ read_from(Reader& r) {
        KEGS_GET_GUILD_USER_LIST_REQ v;
        v.m_uiViewPage = Codec<std::remove_cvref_t<decltype(v.m_uiViewPage)>>::get(r);
        v.m_cGuildUserListType = Codec<std::remove_cvref_t<decltype(v.m_cGuildUserListType)>>::get(r);
        return v;
    }
};

struct KEGS_GET_ITEM_NOT {
    std::int64_t                             m_GetUnitUID{};  // UidType
    std::int32_t                             m_iItemID{};  // int
    std::int32_t                             m_iDropItemUID{};  // int
    char                                     m_cGetItemType{};
    bool                                     m_bIsItemSuccess{};
    std::int32_t                             m_iTotalED{};  // int
    std::map<std::int64_t, KDungeonRewardED> m_mapGetED{};  // std::map< UidType, KDungeonRewardED >
    char                                     m_cEnchantLevel{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_GetUnitUID)>>::put(w, m_GetUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iItemID)>>::put(w, m_iItemID);
        Codec<std::remove_cvref_t<decltype(m_iDropItemUID)>>::put(w, m_iDropItemUID);
        Codec<std::remove_cvref_t<decltype(m_cGetItemType)>>::put(w, m_cGetItemType);
        Codec<std::remove_cvref_t<decltype(m_bIsItemSuccess)>>::put(w, m_bIsItemSuccess);
        Codec<std::remove_cvref_t<decltype(m_iTotalED)>>::put(w, m_iTotalED);
        Codec<std::remove_cvref_t<decltype(m_mapGetED)>>::put(w, m_mapGetED);
        Codec<std::remove_cvref_t<decltype(m_cEnchantLevel)>>::put(w, m_cEnchantLevel);
    }

    static KEGS_GET_ITEM_NOT read_from(Reader& r) {
        KEGS_GET_ITEM_NOT v;
        v.m_GetUnitUID = Codec<std::remove_cvref_t<decltype(v.m_GetUnitUID)>>::get(r);
        v.m_iItemID = Codec<std::remove_cvref_t<decltype(v.m_iItemID)>>::get(r);
        v.m_iDropItemUID = Codec<std::remove_cvref_t<decltype(v.m_iDropItemUID)>>::get(r);
        v.m_cGetItemType = Codec<std::remove_cvref_t<decltype(v.m_cGetItemType)>>::get(r);
        v.m_bIsItemSuccess = Codec<std::remove_cvref_t<decltype(v.m_bIsItemSuccess)>>::get(r);
        v.m_iTotalED = Codec<std::remove_cvref_t<decltype(v.m_iTotalED)>>::get(r);
        v.m_mapGetED = Codec<std::remove_cvref_t<decltype(v.m_mapGetED)>>::get(r);
        v.m_cEnchantLevel = Codec<std::remove_cvref_t<decltype(v.m_cEnchantLevel)>>::get(r);
        return v;
    }
};

struct KEGS_GET_MY_INVENTORY_ACK {
    std::int32_t m_iOK{};  // int
    bool         m_bIsRecommend{};
    bool         m_bSharingBank{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_bIsRecommend)>>::put(w, m_bIsRecommend);
        Codec<std::remove_cvref_t<decltype(m_bSharingBank)>>::put(w, m_bSharingBank);
    }

    static KEGS_GET_MY_INVENTORY_ACK read_from(Reader& r) {
        KEGS_GET_MY_INVENTORY_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_bIsRecommend = Codec<std::remove_cvref_t<decltype(v.m_bIsRecommend)>>::get(r);
        v.m_bSharingBank = Codec<std::remove_cvref_t<decltype(v.m_bSharingBank)>>::get(r);
        return v;
    }
};

struct KEGS_GET_MY_MESSENGER_SN_ACK {
    std::int32_t  m_iOK{};  // int
    std::uint32_t m_uiKNMSerialNum{};  // u_int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_uiKNMSerialNum)>>::put(w, m_uiKNMSerialNum);
    }

    static KEGS_GET_MY_MESSENGER_SN_ACK read_from(Reader& r) {
        KEGS_GET_MY_MESSENGER_SN_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_uiKNMSerialNum = Codec<std::remove_cvref_t<decltype(v.m_uiKNMSerialNum)>>::get(r);
        return v;
    }
};

struct KEGS_GET_NEXT_BANK_ED_ACK {
    std::int32_t m_iOK{};  // int
    std::int32_t m_iED{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iED)>>::put(w, m_iED);
    }

    static KEGS_GET_NEXT_BANK_ED_ACK read_from(Reader& r) {
        KEGS_GET_NEXT_BANK_ED_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iED = Codec<std::remove_cvref_t<decltype(v.m_iED)>>::get(r);
        return v;
    }
};

struct KEGS_GET_PET_LIST_REQ {
    std::uint32_t m_nViewPage{};  // UINT

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_nViewPage)>>::put(w, m_nViewPage);
    }

    static KEGS_GET_PET_LIST_REQ read_from(Reader& r) {
        KEGS_GET_PET_LIST_REQ v;
        v.m_nViewPage = Codec<std::remove_cvref_t<decltype(v.m_nViewPage)>>::get(r);
        return v;
    }
};

struct KEGS_GET_POST_BLACK_LIST_ACK {
    std::vector<std::u16string> m_vecBlackList{};  // std::vector< std::wstring >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecBlackList)>>::put(w, m_vecBlackList);
    }

    static KEGS_GET_POST_BLACK_LIST_ACK read_from(Reader& r) {
        KEGS_GET_POST_BLACK_LIST_ACK v;
        v.m_vecBlackList = Codec<std::remove_cvref_t<decltype(v.m_vecBlackList)>>::get(r);
        return v;
    }
};

struct KEGS_GET_POST_LETTER_LIST_REQ {
    char         m_cLetterListType{};
    std::int32_t m_iPostPageNo{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cLetterListType)>>::put(w, m_cLetterListType);
        Codec<std::remove_cvref_t<decltype(m_iPostPageNo)>>::put(w, m_iPostPageNo);
    }

    static KEGS_GET_POST_LETTER_LIST_REQ read_from(Reader& r) {
        KEGS_GET_POST_LETTER_LIST_REQ v;
        v.m_cLetterListType = Codec<std::remove_cvref_t<decltype(v.m_cLetterListType)>>::get(r);
        v.m_iPostPageNo = Codec<std::remove_cvref_t<decltype(v.m_iPostPageNo)>>::get(r);
        return v;
    }
};

struct KEGS_GET_PURCHASED_CASH_ITEM_REQ {
    std::uint32_t                        m_ulOrderNo{};  // unsigned long
    std::uint32_t                        m_ulProductNo{};  // unsigned long
    std::u16string                       m_wstrProductID{};  // std::wstring
    std::uint16_t                        m_usProductExpire{};  // unsigned short
    std::uint16_t                        m_usProductPieces{};  // unsigned short
    std::uint8_t                         m_byteProductKind{};  // BYTE
    std::int32_t                         m_iQuantity{};  // int
    std::map<std::int32_t, std::int32_t> m_mapSocketForCash{};  // std::map< int, int >
    std::map<std::int32_t, std::int32_t> m_mapSocketGroupIDForCash{};  // std::map< int, int >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_ulOrderNo)>>::put(w, m_ulOrderNo);
        Codec<std::remove_cvref_t<decltype(m_ulProductNo)>>::put(w, m_ulProductNo);
        Codec<std::remove_cvref_t<decltype(m_wstrProductID)>>::put(w, m_wstrProductID);
        Codec<std::remove_cvref_t<decltype(m_usProductExpire)>>::put(w, m_usProductExpire);
        Codec<std::remove_cvref_t<decltype(m_usProductPieces)>>::put(w, m_usProductPieces);
        Codec<std::remove_cvref_t<decltype(m_byteProductKind)>>::put(w, m_byteProductKind);
        Codec<std::remove_cvref_t<decltype(m_iQuantity)>>::put(w, m_iQuantity);
        Codec<std::remove_cvref_t<decltype(m_mapSocketForCash)>>::put(w, m_mapSocketForCash);
        Codec<std::remove_cvref_t<decltype(m_mapSocketGroupIDForCash)>>::put(w, m_mapSocketGroupIDForCash);
    }

    static KEGS_GET_PURCHASED_CASH_ITEM_REQ read_from(Reader& r) {
        KEGS_GET_PURCHASED_CASH_ITEM_REQ v;
        v.m_ulOrderNo = Codec<std::remove_cvref_t<decltype(v.m_ulOrderNo)>>::get(r);
        v.m_ulProductNo = Codec<std::remove_cvref_t<decltype(v.m_ulProductNo)>>::get(r);
        v.m_wstrProductID = Codec<std::remove_cvref_t<decltype(v.m_wstrProductID)>>::get(r);
        v.m_usProductExpire = Codec<std::remove_cvref_t<decltype(v.m_usProductExpire)>>::get(r);
        v.m_usProductPieces = Codec<std::remove_cvref_t<decltype(v.m_usProductPieces)>>::get(r);
        v.m_byteProductKind = Codec<std::remove_cvref_t<decltype(v.m_byteProductKind)>>::get(r);
        v.m_iQuantity = Codec<std::remove_cvref_t<decltype(v.m_iQuantity)>>::get(r);
        v.m_mapSocketForCash = Codec<std::remove_cvref_t<decltype(v.m_mapSocketForCash)>>::get(r);
        v.m_mapSocketGroupIDForCash = Codec<std::remove_cvref_t<decltype(v.m_mapSocketGroupIDForCash)>>::get(r);
        return v;
    }
};

struct KEGS_GET_RANKING_INFO_REQ {
    std::int32_t  m_iRankingType{};  // int
    std::uint32_t m_uiViewPage{};  // UINT

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iRankingType)>>::put(w, m_iRankingType);
        Codec<std::remove_cvref_t<decltype(m_uiViewPage)>>::put(w, m_uiViewPage);
    }

    static KEGS_GET_RANKING_INFO_REQ read_from(Reader& r) {
        KEGS_GET_RANKING_INFO_REQ v;
        v.m_iRankingType = Codec<std::remove_cvref_t<decltype(v.m_iRankingType)>>::get(r);
        v.m_uiViewPage = Codec<std::remove_cvref_t<decltype(v.m_uiViewPage)>>::get(r);
        return v;
    }
};

struct KEGS_GET_RECOMMEND_USER_LIST_REQ {
    std::uint32_t m_uiViewPage{};  // u_int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_uiViewPage)>>::put(w, m_uiViewPage);
    }

    static KEGS_GET_RECOMMEND_USER_LIST_REQ read_from(Reader& r) {
        KEGS_GET_RECOMMEND_USER_LIST_REQ v;
        v.m_uiViewPage = Codec<std::remove_cvref_t<decltype(v.m_uiViewPage)>>::get(r);
        return v;
    }
};

struct KEGS_GET_RIDING_PET_LIST_REQ {
    std::uint8_t m_byteViewPage{};  // byte

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_byteViewPage)>>::put(w, m_byteViewPage);
    }

    static KEGS_GET_RIDING_PET_LIST_REQ read_from(Reader& r) {
        KEGS_GET_RIDING_PET_LIST_REQ v;
        v.m_byteViewPage = Codec<std::remove_cvref_t<decltype(v.m_byteViewPage)>>::get(r);
        return v;
    }
};

struct KEGS_GET_SHARE_BANK_REQ {
    std::u16string m_wstrNickName{};  // std::wstring
    std::int32_t   m_iNewUnitTradeBlockDay{};  // int
    std::int32_t   m_iNewUnitTradeBlockUnitClass{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_iNewUnitTradeBlockDay)>>::put(w, m_iNewUnitTradeBlockDay);
        Codec<std::remove_cvref_t<decltype(m_iNewUnitTradeBlockUnitClass)>>::put(w, m_iNewUnitTradeBlockUnitClass);
    }

    static KEGS_GET_SHARE_BANK_REQ read_from(Reader& r) {
        KEGS_GET_SHARE_BANK_REQ v;
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_iNewUnitTradeBlockDay = Codec<std::remove_cvref_t<decltype(v.m_iNewUnitTradeBlockDay)>>::get(r);
        v.m_iNewUnitTradeBlockUnitClass = Codec<std::remove_cvref_t<decltype(v.m_iNewUnitTradeBlockUnitClass)>>::get(r);
        return v;
    }
};

struct KEGS_GET_TEMP_ITEM_REQ {
    std::int32_t m_iTempItemUID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iTempItemUID)>>::put(w, m_iTempItemUID);
    }

    static KEGS_GET_TEMP_ITEM_REQ read_from(Reader& r) {
        KEGS_GET_TEMP_ITEM_REQ v;
        v.m_iTempItemUID = Codec<std::remove_cvref_t<decltype(v.m_iTempItemUID)>>::get(r);
        return v;
    }
};

struct KEGS_GET_WISH_LIST_ACK {
    std::map<std::int32_t, std::int32_t> m_mapWishList{};  // std::map< int, int >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_mapWishList)>>::put(w, m_mapWishList);
    }

    static KEGS_GET_WISH_LIST_ACK read_from(Reader& r) {
        KEGS_GET_WISH_LIST_ACK v;
        v.m_mapWishList = Codec<std::remove_cvref_t<decltype(v.m_mapWishList)>>::get(r);
        return v;
    }
};

struct KEGS_GIVE_UP_QUEST_REQ {
    std::int32_t m_iQuestID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iQuestID)>>::put(w, m_iQuestID);
    }

    static KEGS_GIVE_UP_QUEST_REQ read_from(Reader& r) {
        KEGS_GIVE_UP_QUEST_REQ v;
        v.m_iQuestID = Codec<std::remove_cvref_t<decltype(v.m_iQuestID)>>::get(r);
        return v;
    }
};

struct KEGS_GUILD_MESSAGE_NOT {
    char           m_cMessageType{};
    std::int64_t   m_iUnitUID{};  // UidType
    std::u16string m_wstrNickName{};  // std::wstring
    std::u16string m_wstrMessage{};  // std::wstring
    std::int32_t   m_iMessageData{};  // int
    std::int32_t   m_iMessageData2{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cMessageType)>>::put(w, m_cMessageType);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_wstrMessage)>>::put(w, m_wstrMessage);
        Codec<std::remove_cvref_t<decltype(m_iMessageData)>>::put(w, m_iMessageData);
        Codec<std::remove_cvref_t<decltype(m_iMessageData2)>>::put(w, m_iMessageData2);
    }

    static KEGS_GUILD_MESSAGE_NOT read_from(Reader& r) {
        KEGS_GUILD_MESSAGE_NOT v;
        v.m_cMessageType = Codec<std::remove_cvref_t<decltype(v.m_cMessageType)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_wstrMessage = Codec<std::remove_cvref_t<decltype(v.m_wstrMessage)>>::get(r);
        v.m_iMessageData = Codec<std::remove_cvref_t<decltype(v.m_iMessageData)>>::get(r);
        v.m_iMessageData2 = Codec<std::remove_cvref_t<decltype(v.m_iMessageData2)>>::get(r);
        return v;
    }
};

struct KEGS_HENIR_REWARD_COUNT_NOT {
    bool         m_bUnLimited{};
    std::int32_t m_iNormal{};  // int
    std::int32_t m_iPremium{};  // int
    std::int32_t m_iEvent{};  // int
    std::int32_t m_iPremiumMAX{};  // int
    std::int32_t m_iEventMAX{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bUnLimited)>>::put(w, m_bUnLimited);
        Codec<std::remove_cvref_t<decltype(m_iNormal)>>::put(w, m_iNormal);
        Codec<std::remove_cvref_t<decltype(m_iPremium)>>::put(w, m_iPremium);
        Codec<std::remove_cvref_t<decltype(m_iEvent)>>::put(w, m_iEvent);
        Codec<std::remove_cvref_t<decltype(m_iPremiumMAX)>>::put(w, m_iPremiumMAX);
        Codec<std::remove_cvref_t<decltype(m_iEventMAX)>>::put(w, m_iEventMAX);
    }

    static KEGS_HENIR_REWARD_COUNT_NOT read_from(Reader& r) {
        KEGS_HENIR_REWARD_COUNT_NOT v;
        v.m_bUnLimited = Codec<std::remove_cvref_t<decltype(v.m_bUnLimited)>>::get(r);
        v.m_iNormal = Codec<std::remove_cvref_t<decltype(v.m_iNormal)>>::get(r);
        v.m_iPremium = Codec<std::remove_cvref_t<decltype(v.m_iPremium)>>::get(r);
        v.m_iEvent = Codec<std::remove_cvref_t<decltype(v.m_iEvent)>>::get(r);
        v.m_iPremiumMAX = Codec<std::remove_cvref_t<decltype(v.m_iPremiumMAX)>>::get(r);
        v.m_iEventMAX = Codec<std::remove_cvref_t<decltype(v.m_iEventMAX)>>::get(r);
        return v;
    }
};

struct KEGS_IDENTIFY_ITEM_REQ {
    std::int64_t m_iItemUID{};  // UidType
    std::int32_t m_iQuantity{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_iQuantity)>>::put(w, m_iQuantity);
    }

    static KEGS_IDENTIFY_ITEM_REQ read_from(Reader& r) {
        KEGS_IDENTIFY_ITEM_REQ v;
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_iQuantity = Codec<std::remove_cvref_t<decltype(v.m_iQuantity)>>::get(r);
        return v;
    }
};

struct KEGS_INIT_GUILD_SKILL_NOT {
    std::int32_t m_iGuildSPoint{};  // int
    std::int32_t m_iGuildCSPoint{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iGuildSPoint)>>::put(w, m_iGuildSPoint);
        Codec<std::remove_cvref_t<decltype(m_iGuildCSPoint)>>::put(w, m_iGuildCSPoint);
    }

    static KEGS_INIT_GUILD_SKILL_NOT read_from(Reader& r) {
        KEGS_INIT_GUILD_SKILL_NOT v;
        v.m_iGuildSPoint = Codec<std::remove_cvref_t<decltype(v.m_iGuildSPoint)>>::get(r);
        v.m_iGuildCSPoint = Codec<std::remove_cvref_t<decltype(v.m_iGuildCSPoint)>>::get(r);
        return v;
    }
};

struct KEGS_INIT_GUILD_SKILL_REQ {
    std::int64_t m_iItemUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
    }

    static KEGS_INIT_GUILD_SKILL_REQ read_from(Reader& r) {
        KEGS_INIT_GUILD_SKILL_REQ v;
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        return v;
    }
};

struct KEGS_INIT_SKILL_TREE_REQ {
    std::int64_t m_iItemUID{};  // UidType
    std::int32_t m_iActiveSkillPageNumber{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_iActiveSkillPageNumber)>>::put(w, m_iActiveSkillPageNumber);
    }

    static KEGS_INIT_SKILL_TREE_REQ read_from(Reader& r) {
        KEGS_INIT_SKILL_TREE_REQ v;
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_iActiveSkillPageNumber = Codec<std::remove_cvref_t<decltype(v.m_iActiveSkillPageNumber)>>::get(r);
        return v;
    }
};

struct KEGS_INTRUDE_START_ACK {
    std::int32_t m_iOK{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
    }

    static KEGS_INTRUDE_START_ACK read_from(Reader& r) {
        KEGS_INTRUDE_START_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        return v;
    }
};

struct KEGS_INTRUDE_START_REQ {
    std::int32_t m_StartPosIndex{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_StartPosIndex)>>::put(w, m_StartPosIndex);
    }

    static KEGS_INTRUDE_START_REQ read_from(Reader& r) {
        KEGS_INTRUDE_START_REQ v;
        v.m_StartPosIndex = Codec<std::remove_cvref_t<decltype(v.m_StartPosIndex)>>::get(r);
        return v;
    }
};

struct KEGS_INVITE_GUILD_ACK {
    std::int32_t   m_iOK{};  // int
    std::u16string m_wstrReceiverNickName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_wstrReceiverNickName)>>::put(w, m_wstrReceiverNickName);
    }

    static KEGS_INVITE_GUILD_ACK read_from(Reader& r) {
        KEGS_INVITE_GUILD_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_wstrReceiverNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrReceiverNickName)>>::get(r);
        return v;
    }
};

struct KEGS_INVITE_GUILD_REPLY_DENY_NOT {
    std::u16string m_wstrNickName{};  // std::wstring
    std::int32_t   m_iReason{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_iReason)>>::put(w, m_iReason);
    }

    static KEGS_INVITE_GUILD_REPLY_DENY_NOT read_from(Reader& r) {
        KEGS_INVITE_GUILD_REPLY_DENY_NOT v;
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_iReason = Codec<std::remove_cvref_t<decltype(v.m_iReason)>>::get(r);
        return v;
    }
};

struct KEGS_INVITE_GUILD_REPLY_REQ {
    std::int32_t   m_iGuildUID{};  // int
    std::u16string m_wstrSenderNickName{};  // std::wstring
    std::int32_t   m_iReason{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iGuildUID)>>::put(w, m_iGuildUID);
        Codec<std::remove_cvref_t<decltype(m_wstrSenderNickName)>>::put(w, m_wstrSenderNickName);
        Codec<std::remove_cvref_t<decltype(m_iReason)>>::put(w, m_iReason);
    }

    static KEGS_INVITE_GUILD_REPLY_REQ read_from(Reader& r) {
        KEGS_INVITE_GUILD_REPLY_REQ v;
        v.m_iGuildUID = Codec<std::remove_cvref_t<decltype(v.m_iGuildUID)>>::get(r);
        v.m_wstrSenderNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrSenderNickName)>>::get(r);
        v.m_iReason = Codec<std::remove_cvref_t<decltype(v.m_iReason)>>::get(r);
        return v;
    }
};

struct KEGS_INVITE_GUILD_REQ {
    std::u16string m_wstrNickName{};  // std::wstring
    std::int64_t   m_iUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
    }

    static KEGS_INVITE_GUILD_REQ read_from(Reader& r) {
        KEGS_INVITE_GUILD_REQ v;
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        return v;
    }
};

struct KEGS_INVITE_PARTY_ACK {
    std::int32_t   m_iOK{};  // int
    std::int64_t   m_iReceiverUID{};  // UidType
    std::u16string m_wstrUDPRelayIP{};  // std::wstring
    std::uint16_t  m_usUDPRelayPort{};  // USHORT

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iReceiverUID)>>::put(w, m_iReceiverUID);
        Codec<std::remove_cvref_t<decltype(m_wstrUDPRelayIP)>>::put(w, m_wstrUDPRelayIP);
        Codec<std::remove_cvref_t<decltype(m_usUDPRelayPort)>>::put(w, m_usUDPRelayPort);
    }

    static KEGS_INVITE_PARTY_ACK read_from(Reader& r) {
        KEGS_INVITE_PARTY_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iReceiverUID = Codec<std::remove_cvref_t<decltype(v.m_iReceiverUID)>>::get(r);
        v.m_wstrUDPRelayIP = Codec<std::remove_cvref_t<decltype(v.m_wstrUDPRelayIP)>>::get(r);
        v.m_usUDPRelayPort = Codec<std::remove_cvref_t<decltype(v.m_usUDPRelayPort)>>::get(r);
        return v;
    }
};

struct KEGS_INVITE_PARTY_DENY_NOT {
    std::int32_t m_iDenyReasonNetError{};  // int
    std::int64_t m_iUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iDenyReasonNetError)>>::put(w, m_iDenyReasonNetError);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
    }

    static KEGS_INVITE_PARTY_DENY_NOT read_from(Reader& r) {
        KEGS_INVITE_PARTY_DENY_NOT v;
        v.m_iDenyReasonNetError = Codec<std::remove_cvref_t<decltype(v.m_iDenyReasonNetError)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        return v;
    }
};

struct KEGS_INVITE_PARTY_NOT {
    std::int64_t   m_iPartyUID{};  // UidType
    std::int64_t   m_iHostUnitUID{};  // UidType
    std::u16string m_wstrHostNickName{};  // std::wstring
    char           m_cPartyType{};
    std::u16string m_wstrUDPRelayIP{};  // std::wstring
    std::uint16_t  m_usUDPRelayPort{};  // USHORT

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iPartyUID)>>::put(w, m_iPartyUID);
        Codec<std::remove_cvref_t<decltype(m_iHostUnitUID)>>::put(w, m_iHostUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrHostNickName)>>::put(w, m_wstrHostNickName);
        Codec<std::remove_cvref_t<decltype(m_cPartyType)>>::put(w, m_cPartyType);
        Codec<std::remove_cvref_t<decltype(m_wstrUDPRelayIP)>>::put(w, m_wstrUDPRelayIP);
        Codec<std::remove_cvref_t<decltype(m_usUDPRelayPort)>>::put(w, m_usUDPRelayPort);
    }

    static KEGS_INVITE_PARTY_NOT read_from(Reader& r) {
        KEGS_INVITE_PARTY_NOT v;
        v.m_iPartyUID = Codec<std::remove_cvref_t<decltype(v.m_iPartyUID)>>::get(r);
        v.m_iHostUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iHostUnitUID)>>::get(r);
        v.m_wstrHostNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrHostNickName)>>::get(r);
        v.m_cPartyType = Codec<std::remove_cvref_t<decltype(v.m_cPartyType)>>::get(r);
        v.m_wstrUDPRelayIP = Codec<std::remove_cvref_t<decltype(v.m_wstrUDPRelayIP)>>::get(r);
        v.m_usUDPRelayPort = Codec<std::remove_cvref_t<decltype(v.m_usUDPRelayPort)>>::get(r);
        return v;
    }
};

struct KEGS_INVITE_PARTY_REPLY_NOT {
    std::int64_t m_iPartyUID{};  // UidType
    std::int64_t m_iHostUnitUID{};  // UidType
    char         m_cReplyReason{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iPartyUID)>>::put(w, m_iPartyUID);
        Codec<std::remove_cvref_t<decltype(m_iHostUnitUID)>>::put(w, m_iHostUnitUID);
        Codec<std::remove_cvref_t<decltype(m_cReplyReason)>>::put(w, m_cReplyReason);
    }

    static KEGS_INVITE_PARTY_REPLY_NOT read_from(Reader& r) {
        KEGS_INVITE_PARTY_REPLY_NOT v;
        v.m_iPartyUID = Codec<std::remove_cvref_t<decltype(v.m_iPartyUID)>>::get(r);
        v.m_iHostUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iHostUnitUID)>>::get(r);
        v.m_cReplyReason = Codec<std::remove_cvref_t<decltype(v.m_cReplyReason)>>::get(r);
        return v;
    }
};

struct KEGS_INVITE_PARTY_REQ {
    std::int64_t     m_iReceiverUID{};  // UidType
    std::u16string   m_wstrNickName{};  // std::wstring
    std::int32_t     m_iServerGroupID{};  // int
    KCreatePartyInfo m_kCreatePartyInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iReceiverUID)>>::put(w, m_iReceiverUID);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_iServerGroupID)>>::put(w, m_iServerGroupID);
        Codec<std::remove_cvref_t<decltype(m_kCreatePartyInfo)>>::put(w, m_kCreatePartyInfo);
    }

    static KEGS_INVITE_PARTY_REQ read_from(Reader& r) {
        KEGS_INVITE_PARTY_REQ v;
        v.m_iReceiverUID = Codec<std::remove_cvref_t<decltype(v.m_iReceiverUID)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_iServerGroupID = Codec<std::remove_cvref_t<decltype(v.m_iServerGroupID)>>::get(r);
        v.m_kCreatePartyInfo = Codec<std::remove_cvref_t<decltype(v.m_kCreatePartyInfo)>>::get(r);
        return v;
    }
};

struct KEGS_INVITE_PVP_ROOM_ACK {
    std::int32_t   m_iOK{};  // int
    std::u16string m_wstrNickName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
    }

    static KEGS_INVITE_PVP_ROOM_ACK read_from(Reader& r) {
        KEGS_INVITE_PVP_ROOM_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        return v;
    }
};

struct KEGS_INVITE_PVP_ROOM_REQ {
    std::int64_t m_iUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
    }

    static KEGS_INVITE_PVP_ROOM_REQ read_from(Reader& r) {
        KEGS_INVITE_PVP_ROOM_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        return v;
    }
};

struct KEGS_ITEM_CONVERT_REQ {
    std::int64_t m_iItemUID{};  // UidType
    std::int32_t m_iQuantity{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_iQuantity)>>::put(w, m_iQuantity);
    }

    static KEGS_ITEM_CONVERT_REQ read_from(Reader& r) {
        KEGS_ITEM_CONVERT_REQ v;
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_iQuantity = Codec<std::remove_cvref_t<decltype(v.m_iQuantity)>>::get(r);
        return v;
    }
};

struct KEGS_ITEM_EVALUATE_REQ {
    std::int64_t m_iItemUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
    }

    static KEGS_ITEM_EVALUATE_REQ read_from(Reader& r) {
        KEGS_ITEM_EVALUATE_REQ v;
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        return v;
    }
};

struct KEGS_ITEM_EXCHANGE_REQ {
    std::int32_t m_iHouseID{};  // int
    std::int64_t m_iSourceItemUID{};  // UidType
    std::int32_t m_iSourceItemID{};  // int
    std::int32_t m_iSourceQuantity{};  // int
    std::int32_t m_iDestItemID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iHouseID)>>::put(w, m_iHouseID);
        Codec<std::remove_cvref_t<decltype(m_iSourceItemUID)>>::put(w, m_iSourceItemUID);
        Codec<std::remove_cvref_t<decltype(m_iSourceItemID)>>::put(w, m_iSourceItemID);
        Codec<std::remove_cvref_t<decltype(m_iSourceQuantity)>>::put(w, m_iSourceQuantity);
        Codec<std::remove_cvref_t<decltype(m_iDestItemID)>>::put(w, m_iDestItemID);
    }

    static KEGS_ITEM_EXCHANGE_REQ read_from(Reader& r) {
        KEGS_ITEM_EXCHANGE_REQ v;
        v.m_iHouseID = Codec<std::remove_cvref_t<decltype(v.m_iHouseID)>>::get(r);
        v.m_iSourceItemUID = Codec<std::remove_cvref_t<decltype(v.m_iSourceItemUID)>>::get(r);
        v.m_iSourceItemID = Codec<std::remove_cvref_t<decltype(v.m_iSourceItemID)>>::get(r);
        v.m_iSourceQuantity = Codec<std::remove_cvref_t<decltype(v.m_iSourceQuantity)>>::get(r);
        v.m_iDestItemID = Codec<std::remove_cvref_t<decltype(v.m_iDestItemID)>>::get(r);
        return v;
    }
};

struct KEGS_ITEM_EXPIRATION_NOT {
    std::vector<std::int64_t> m_vecItemUID{};  // std::vector< UidType >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecItemUID)>>::put(w, m_vecItemUID);
    }

    static KEGS_ITEM_EXPIRATION_NOT read_from(Reader& r) {
        KEGS_ITEM_EXPIRATION_NOT v;
        v.m_vecItemUID = Codec<std::remove_cvref_t<decltype(v.m_vecItemUID)>>::get(r);
        return v;
    }
};

struct KEGS_ITEM_EXTRACT_REQ {
    std::int64_t m_iItemUID{};  // UidType
    std::int32_t m_iQuantity{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_iQuantity)>>::put(w, m_iQuantity);
    }

    static KEGS_ITEM_EXTRACT_REQ read_from(Reader& r) {
        KEGS_ITEM_EXTRACT_REQ v;
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_iQuantity = Codec<std::remove_cvref_t<decltype(v.m_iQuantity)>>::get(r);
        return v;
    }
};

struct KEGS_ITEM_MANUFACTURE_REQ {
    std::int32_t m_iManufactureID{};  // int
    std::int32_t m_iQuantity{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iManufactureID)>>::put(w, m_iManufactureID);
        Codec<std::remove_cvref_t<decltype(m_iQuantity)>>::put(w, m_iQuantity);
    }

    static KEGS_ITEM_MANUFACTURE_REQ read_from(Reader& r) {
        KEGS_ITEM_MANUFACTURE_REQ v;
        v.m_iManufactureID = Codec<std::remove_cvref_t<decltype(v.m_iManufactureID)>>::get(r);
        v.m_iQuantity = Codec<std::remove_cvref_t<decltype(v.m_iQuantity)>>::get(r);
        return v;
    }
};

struct KEGS_JOIN_BATTLE_FIELD_REQ {
    KBattleFieldJoinInfo m_kBattleFieldJoinInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kBattleFieldJoinInfo)>>::put(w, m_kBattleFieldJoinInfo);
    }

    static KEGS_JOIN_BATTLE_FIELD_REQ read_from(Reader& r) {
        KEGS_JOIN_BATTLE_FIELD_REQ v;
        v.m_kBattleFieldJoinInfo = Codec<std::remove_cvref_t<decltype(v.m_kBattleFieldJoinInfo)>>::get(r);
        return v;
    }
};

struct KEGS_JOIN_MY_PSHOP_AGENCY_REQ {
    char m_cJoinType{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cJoinType)>>::put(w, m_cJoinType);
    }

    static KEGS_JOIN_MY_PSHOP_AGENCY_REQ read_from(Reader& r) {
        KEGS_JOIN_MY_PSHOP_AGENCY_REQ v;
        v.m_cJoinType = Codec<std::remove_cvref_t<decltype(v.m_cJoinType)>>::get(r);
        return v;
    }
};

struct KEGS_JOIN_PERSONAL_SHOP_REQ {
    std::int64_t m_iUnitUID{};  // UidType
    char         m_cStageKey{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_cStageKey)>>::put(w, m_cStageKey);
    }

    static KEGS_JOIN_PERSONAL_SHOP_REQ read_from(Reader& r) {
        KEGS_JOIN_PERSONAL_SHOP_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_cStageKey = Codec<std::remove_cvref_t<decltype(v.m_cStageKey)>>::get(r);
        return v;
    }
};

struct KEGS_JOIN_ROOM_REQ {
    std::int64_t   m_iRoomUID{};  // UidType
    std::u16string m_wstrPassword{};  // std::wstring
    bool           m_bIsObserver{};
    bool           m_bInvitation{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iRoomUID)>>::put(w, m_iRoomUID);
        Codec<std::remove_cvref_t<decltype(m_wstrPassword)>>::put(w, m_wstrPassword);
        Codec<std::remove_cvref_t<decltype(m_bIsObserver)>>::put(w, m_bIsObserver);
        Codec<std::remove_cvref_t<decltype(m_bInvitation)>>::put(w, m_bInvitation);
    }

    static KEGS_JOIN_ROOM_REQ read_from(Reader& r) {
        KEGS_JOIN_ROOM_REQ v;
        v.m_iRoomUID = Codec<std::remove_cvref_t<decltype(v.m_iRoomUID)>>::get(r);
        v.m_wstrPassword = Codec<std::remove_cvref_t<decltype(v.m_wstrPassword)>>::get(r);
        v.m_bIsObserver = Codec<std::remove_cvref_t<decltype(v.m_bIsObserver)>>::get(r);
        v.m_bInvitation = Codec<std::remove_cvref_t<decltype(v.m_bInvitation)>>::get(r);
        return v;
    }
};

struct KEGS_JOIN_SQUARE_REQ {
    std::int64_t m_iSquareUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iSquareUID)>>::put(w, m_iSquareUID);
    }

    static KEGS_JOIN_SQUARE_REQ read_from(Reader& r) {
        KEGS_JOIN_SQUARE_REQ v;
        v.m_iSquareUID = Codec<std::remove_cvref_t<decltype(v.m_iSquareUID)>>::get(r);
        return v;
    }
};

struct KEGS_JOIN_WEDDING_HALL_REQ {
    std::int64_t m_iItemUID{};  // UidType
    std::int32_t m_iWeddingUID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_iWeddingUID)>>::put(w, m_iWeddingUID);
    }

    static KEGS_JOIN_WEDDING_HALL_REQ read_from(Reader& r) {
        KEGS_JOIN_WEDDING_HALL_REQ v;
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_iWeddingUID = Codec<std::remove_cvref_t<decltype(v.m_iWeddingUID)>>::get(r);
        return v;
    }
};

struct KEGS_KICK_GUILD_MEMBER_ACK {
    std::int32_t m_iOK{};  // int
    bool         m_bKickBySysop{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_bKickBySysop)>>::put(w, m_bKickBySysop);
    }

    static KEGS_KICK_GUILD_MEMBER_ACK read_from(Reader& r) {
        KEGS_KICK_GUILD_MEMBER_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_bKickBySysop = Codec<std::remove_cvref_t<decltype(v.m_bKickBySysop)>>::get(r);
        return v;
    }
};

struct KEGS_KICK_GUILD_MEMBER_REQ {
    std::int64_t m_iUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
    }

    static KEGS_KICK_GUILD_MEMBER_REQ read_from(Reader& r) {
        KEGS_KICK_GUILD_MEMBER_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        return v;
    }
};

struct KEGS_KNM_REQUEST_NEW_FRIEND_INFO_ACK {
    std::int32_t   m_iOK{};  // int
    std::uint32_t  m_uiKNMSerialNum{};  // u_int
    std::u16string m_wstrUnitNickName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_uiKNMSerialNum)>>::put(w, m_uiKNMSerialNum);
        Codec<std::remove_cvref_t<decltype(m_wstrUnitNickName)>>::put(w, m_wstrUnitNickName);
    }

    static KEGS_KNM_REQUEST_NEW_FRIEND_INFO_ACK read_from(Reader& r) {
        KEGS_KNM_REQUEST_NEW_FRIEND_INFO_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_uiKNMSerialNum = Codec<std::remove_cvref_t<decltype(v.m_uiKNMSerialNum)>>::get(r);
        v.m_wstrUnitNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrUnitNickName)>>::get(r);
        return v;
    }
};

struct KEGS_KNM_REQUEST_NEW_FRIEND_INFO_REQ {
    std::u16string m_wstrUnitNickName{};  // std::wstring
    std::int64_t   m_iUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrUnitNickName)>>::put(w, m_wstrUnitNickName);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
    }

    static KEGS_KNM_REQUEST_NEW_FRIEND_INFO_REQ read_from(Reader& r) {
        KEGS_KNM_REQUEST_NEW_FRIEND_INFO_REQ v;
        v.m_wstrUnitNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrUnitNickName)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        return v;
    }
};

struct KEGS_KNPROTECT_AUTH_CHECK_NOT {
    std::uint32_t dwIndex{};  // unsigned long
    std::uint32_t dwValue1{};  // unsigned long
    std::uint32_t dwValue2{};  // unsigned long
    std::uint32_t dwValue3{};  // unsigned long

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(dwIndex)>>::put(w, dwIndex);
        Codec<std::remove_cvref_t<decltype(dwValue1)>>::put(w, dwValue1);
        Codec<std::remove_cvref_t<decltype(dwValue2)>>::put(w, dwValue2);
        Codec<std::remove_cvref_t<decltype(dwValue3)>>::put(w, dwValue3);
    }

    static KEGS_KNPROTECT_AUTH_CHECK_NOT read_from(Reader& r) {
        KEGS_KNPROTECT_AUTH_CHECK_NOT v;
        v.dwIndex = Codec<std::remove_cvref_t<decltype(v.dwIndex)>>::get(r);
        v.dwValue1 = Codec<std::remove_cvref_t<decltype(v.dwValue1)>>::get(r);
        v.dwValue2 = Codec<std::remove_cvref_t<decltype(v.dwValue2)>>::get(r);
        v.dwValue3 = Codec<std::remove_cvref_t<decltype(v.dwValue3)>>::get(r);
        return v;
    }
};

struct KEGS_KOM_FILE_CHECK_LOG_ACK {
    std::u16string m_wstrInvalidKomName{};  // std::wstring
    std::int32_t   m_iOK{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrInvalidKomName)>>::put(w, m_wstrInvalidKomName);
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
    }

    static KEGS_KOM_FILE_CHECK_LOG_ACK read_from(Reader& r) {
        KEGS_KOM_FILE_CHECK_LOG_ACK v;
        v.m_wstrInvalidKomName = Codec<std::remove_cvref_t<decltype(v.m_wstrInvalidKomName)>>::get(r);
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        return v;
    }
};

struct KEGS_KOM_FILE_CHECK_LOG_REQ {
    std::u16string m_wstrInvalidKomName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrInvalidKomName)>>::put(w, m_wstrInvalidKomName);
    }

    static KEGS_KOM_FILE_CHECK_LOG_REQ read_from(Reader& r) {
        KEGS_KOM_FILE_CHECK_LOG_REQ v;
        v.m_wstrInvalidKomName = Codec<std::remove_cvref_t<decltype(v.m_wstrInvalidKomName)>>::get(r);
        return v;
    }
};

struct KEGS_LEAVE_PARTY_NOT {
    std::int64_t              m_iLeaveUnitUID{};  // UidType
    std::int64_t              m_iNewHostUID{};  // UidType
    std::int64_t              m_iPartyUID{};  // UidType
    std::vector<std::int64_t> m_vecPartyUnitUIDList{};  // std::vector< UidType >
    bool                      m_bExistComeBackUser{};
    std::int32_t              m_iReason{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iLeaveUnitUID)>>::put(w, m_iLeaveUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iNewHostUID)>>::put(w, m_iNewHostUID);
        Codec<std::remove_cvref_t<decltype(m_iPartyUID)>>::put(w, m_iPartyUID);
        Codec<std::remove_cvref_t<decltype(m_vecPartyUnitUIDList)>>::put(w, m_vecPartyUnitUIDList);
        Codec<std::remove_cvref_t<decltype(m_bExistComeBackUser)>>::put(w, m_bExistComeBackUser);
        Codec<std::remove_cvref_t<decltype(m_iReason)>>::put(w, m_iReason);
    }

    static KEGS_LEAVE_PARTY_NOT read_from(Reader& r) {
        KEGS_LEAVE_PARTY_NOT v;
        v.m_iLeaveUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iLeaveUnitUID)>>::get(r);
        v.m_iNewHostUID = Codec<std::remove_cvref_t<decltype(v.m_iNewHostUID)>>::get(r);
        v.m_iPartyUID = Codec<std::remove_cvref_t<decltype(v.m_iPartyUID)>>::get(r);
        v.m_vecPartyUnitUIDList = Codec<std::remove_cvref_t<decltype(v.m_vecPartyUnitUIDList)>>::get(r);
        v.m_bExistComeBackUser = Codec<std::remove_cvref_t<decltype(v.m_bExistComeBackUser)>>::get(r);
        v.m_iReason = Codec<std::remove_cvref_t<decltype(v.m_iReason)>>::get(r);
        return v;
    }
};

struct KEGS_LEAVE_PARTY_REQ {
    std::int32_t m_iReason{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iReason)>>::put(w, m_iReason);
    }

    static KEGS_LEAVE_PARTY_REQ read_from(Reader& r) {
        KEGS_LEAVE_PARTY_REQ v;
        v.m_iReason = Codec<std::remove_cvref_t<decltype(v.m_iReason)>>::get(r);
        return v;
    }
};

struct KEGS_LEAVE_ROOM_ACK {
    std::int32_t         m_iOK{};  // int
    std::int32_t         m_iReason{};  // int
    std::int32_t         m_iItemID{};  // int
    bool                 m_bNotLeaveParty{};
    KBattleFieldJoinInfo m_kBattleFieldJoinInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iReason)>>::put(w, m_iReason);
        Codec<std::remove_cvref_t<decltype(m_iItemID)>>::put(w, m_iItemID);
        Codec<std::remove_cvref_t<decltype(m_bNotLeaveParty)>>::put(w, m_bNotLeaveParty);
        Codec<std::remove_cvref_t<decltype(m_kBattleFieldJoinInfo)>>::put(w, m_kBattleFieldJoinInfo);
    }

    static KEGS_LEAVE_ROOM_ACK read_from(Reader& r) {
        KEGS_LEAVE_ROOM_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iReason = Codec<std::remove_cvref_t<decltype(v.m_iReason)>>::get(r);
        v.m_iItemID = Codec<std::remove_cvref_t<decltype(v.m_iItemID)>>::get(r);
        v.m_bNotLeaveParty = Codec<std::remove_cvref_t<decltype(v.m_bNotLeaveParty)>>::get(r);
        v.m_kBattleFieldJoinInfo = Codec<std::remove_cvref_t<decltype(v.m_kBattleFieldJoinInfo)>>::get(r);
        return v;
    }
};

struct KEGS_LEAVE_SQUARE_NOT {
    std::int64_t m_iUnitUID{};  // UidType
    std::int32_t m_iReason{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iReason)>>::put(w, m_iReason);
    }

    static KEGS_LEAVE_SQUARE_NOT read_from(Reader& r) {
        KEGS_LEAVE_SQUARE_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iReason = Codec<std::remove_cvref_t<decltype(v.m_iReason)>>::get(r);
        return v;
    }
};

struct KEGS_MAKE_FRIEND_GROUP_ACK {
    std::int32_t   m_iOK{};  // int
    char           m_cGroupID{};
    std::u16string m_wstrGroupName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_cGroupID)>>::put(w, m_cGroupID);
        Codec<std::remove_cvref_t<decltype(m_wstrGroupName)>>::put(w, m_wstrGroupName);
    }

    static KEGS_MAKE_FRIEND_GROUP_ACK read_from(Reader& r) {
        KEGS_MAKE_FRIEND_GROUP_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_cGroupID = Codec<std::remove_cvref_t<decltype(v.m_cGroupID)>>::get(r);
        v.m_wstrGroupName = Codec<std::remove_cvref_t<decltype(v.m_wstrGroupName)>>::get(r);
        return v;
    }
};

struct KEGS_MAKE_FRIEND_GROUP_REQ {
    std::u16string m_wstrGroupName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrGroupName)>>::put(w, m_wstrGroupName);
    }

    static KEGS_MAKE_FRIEND_GROUP_REQ read_from(Reader& r) {
        KEGS_MAKE_FRIEND_GROUP_REQ v;
        v.m_wstrGroupName = Codec<std::remove_cvref_t<decltype(v.m_wstrGroupName)>>::get(r);
        return v;
    }
};

struct KEGS_MATCH_CLOSE_NOT {
    std::int32_t m_iReason{};  // int
    std::int64_t m_iCancelUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iReason)>>::put(w, m_iReason);
        Codec<std::remove_cvref_t<decltype(m_iCancelUnitUID)>>::put(w, m_iCancelUnitUID);
    }

    static KEGS_MATCH_CLOSE_NOT read_from(Reader& r) {
        KEGS_MATCH_CLOSE_NOT v;
        v.m_iReason = Codec<std::remove_cvref_t<decltype(v.m_iReason)>>::get(r);
        v.m_iCancelUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iCancelUnitUID)>>::get(r);
        return v;
    }
};

struct KEGS_MATCH_MAKING_SUCCESS_NOT {
    std::int64_t m_iMatchUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iMatchUID)>>::put(w, m_iMatchUID);
    }

    static KEGS_MATCH_MAKING_SUCCESS_NOT read_from(Reader& r) {
        KEGS_MATCH_MAKING_SUCCESS_NOT v;
        v.m_iMatchUID = Codec<std::remove_cvref_t<decltype(v.m_iMatchUID)>>::get(r);
        return v;
    }
};

struct KEGS_MATCH_MAKING_SUCCESS_REPLY_NOT {
    bool                   m_bGameStartAgree{};
    bool                   m_bPvpPowerMode{};
    bool                   m_bPvpItemMode{};
    std::set<std::int16_t> m_setSelectedPvpMap{};  // std::set< short >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bGameStartAgree)>>::put(w, m_bGameStartAgree);
        Codec<std::remove_cvref_t<decltype(m_bPvpPowerMode)>>::put(w, m_bPvpPowerMode);
        Codec<std::remove_cvref_t<decltype(m_bPvpItemMode)>>::put(w, m_bPvpItemMode);
        Codec<std::remove_cvref_t<decltype(m_setSelectedPvpMap)>>::put(w, m_setSelectedPvpMap);
    }

    static KEGS_MATCH_MAKING_SUCCESS_REPLY_NOT read_from(Reader& r) {
        KEGS_MATCH_MAKING_SUCCESS_REPLY_NOT v;
        v.m_bGameStartAgree = Codec<std::remove_cvref_t<decltype(v.m_bGameStartAgree)>>::get(r);
        v.m_bPvpPowerMode = Codec<std::remove_cvref_t<decltype(v.m_bPvpPowerMode)>>::get(r);
        v.m_bPvpItemMode = Codec<std::remove_cvref_t<decltype(v.m_bPvpItemMode)>>::get(r);
        v.m_setSelectedPvpMap = Codec<std::remove_cvref_t<decltype(v.m_setSelectedPvpMap)>>::get(r);
        return v;
    }
};

struct KEGS_MEGAPHONE_MESSAGE_NOT {
    std::u16string m_wstrSenderNickName{};  // std::wstring
    std::int32_t   m_iSenderChannelID{};  // int
    std::int32_t   m_iSenderLocation{};  // int
    std::uint8_t   m_cMessageColor{};  // u_char
    std::u16string m_wstrMessage{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrSenderNickName)>>::put(w, m_wstrSenderNickName);
        Codec<std::remove_cvref_t<decltype(m_iSenderChannelID)>>::put(w, m_iSenderChannelID);
        Codec<std::remove_cvref_t<decltype(m_iSenderLocation)>>::put(w, m_iSenderLocation);
        Codec<std::remove_cvref_t<decltype(m_cMessageColor)>>::put(w, m_cMessageColor);
        Codec<std::remove_cvref_t<decltype(m_wstrMessage)>>::put(w, m_wstrMessage);
    }

    static KEGS_MEGAPHONE_MESSAGE_NOT read_from(Reader& r) {
        KEGS_MEGAPHONE_MESSAGE_NOT v;
        v.m_wstrSenderNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrSenderNickName)>>::get(r);
        v.m_iSenderChannelID = Codec<std::remove_cvref_t<decltype(v.m_iSenderChannelID)>>::get(r);
        v.m_iSenderLocation = Codec<std::remove_cvref_t<decltype(v.m_iSenderLocation)>>::get(r);
        v.m_cMessageColor = Codec<std::remove_cvref_t<decltype(v.m_cMessageColor)>>::get(r);
        v.m_wstrMessage = Codec<std::remove_cvref_t<decltype(v.m_wstrMessage)>>::get(r);
        return v;
    }
};

struct KEGS_MODIFY_WISH_LIST_ACK {
    std::int32_t              m_iOK{};  // int
    std::vector<std::int32_t> m_vecWishListItemID{};  // std::vector< int >
    bool                      m_bIsDelete{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_vecWishListItemID)>>::put(w, m_vecWishListItemID);
        Codec<std::remove_cvref_t<decltype(m_bIsDelete)>>::put(w, m_bIsDelete);
    }

    static KEGS_MODIFY_WISH_LIST_ACK read_from(Reader& r) {
        KEGS_MODIFY_WISH_LIST_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_vecWishListItemID = Codec<std::remove_cvref_t<decltype(v.m_vecWishListItemID)>>::get(r);
        v.m_bIsDelete = Codec<std::remove_cvref_t<decltype(v.m_bIsDelete)>>::get(r);
        return v;
    }
};

struct KEGS_MODIFY_WISH_LIST_REQ {
    std::vector<std::int32_t> m_vecWishListItemID{};  // std::vector< int >
    bool                      m_bIsDelete{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecWishListItemID)>>::put(w, m_vecWishListItemID);
        Codec<std::remove_cvref_t<decltype(m_bIsDelete)>>::put(w, m_bIsDelete);
    }

    static KEGS_MODIFY_WISH_LIST_REQ read_from(Reader& r) {
        KEGS_MODIFY_WISH_LIST_REQ v;
        v.m_vecWishListItemID = Codec<std::remove_cvref_t<decltype(v.m_vecWishListItemID)>>::get(r);
        v.m_bIsDelete = Codec<std::remove_cvref_t<decltype(v.m_bIsDelete)>>::get(r);
        return v;
    }
};

struct KEGS_MODULE_INFO_UPDATE_NOT {
    std::vector<std::u16string> m_vecModuleName{};  // std::vector< std::wstring >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecModuleName)>>::put(w, m_vecModuleName);
    }

    static KEGS_MODULE_INFO_UPDATE_NOT read_from(Reader& r) {
        KEGS_MODULE_INFO_UPDATE_NOT v;
        v.m_vecModuleName = Codec<std::remove_cvref_t<decltype(v.m_vecModuleName)>>::get(r);
        return v;
    }
};

struct KEGS_MOVE_FRIEND_ACK {
    std::int32_t m_iOK{};  // int
    std::int64_t m_iUnitUID{};  // UidType
    char         m_cTargetGroupID{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_cTargetGroupID)>>::put(w, m_cTargetGroupID);
    }

    static KEGS_MOVE_FRIEND_ACK read_from(Reader& r) {
        KEGS_MOVE_FRIEND_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_cTargetGroupID = Codec<std::remove_cvref_t<decltype(v.m_cTargetGroupID)>>::get(r);
        return v;
    }
};

struct KEGS_MOVE_FRIEND_REQ {
    std::int64_t m_iUnitUID{};  // UidType
    char         m_cTargetGroupID{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_cTargetGroupID)>>::put(w, m_cTargetGroupID);
    }

    static KEGS_MOVE_FRIEND_REQ read_from(Reader& r) {
        KEGS_MOVE_FRIEND_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_cTargetGroupID = Codec<std::remove_cvref_t<decltype(v.m_cTargetGroupID)>>::get(r);
        return v;
    }
};

struct KEGS_MY_USER_UNIT_INFO_TO_SERVER_REQ {
    KDungeonPlayResultInfo m_kMyPlayResult{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kMyPlayResult)>>::put(w, m_kMyPlayResult);
    }

    static KEGS_MY_USER_UNIT_INFO_TO_SERVER_REQ read_from(Reader& r) {
        KEGS_MY_USER_UNIT_INFO_TO_SERVER_REQ v;
        v.m_kMyPlayResult = Codec<std::remove_cvref_t<decltype(v.m_kMyPlayResult)>>::get(r);
        return v;
    }
};

struct KEGS_NEW_BLACKLIST_USER_ACK {
    std::int32_t       m_iOK{};  // int
    KChatBlackListUnit m_kChatBlackListUnit{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_kChatBlackListUnit)>>::put(w, m_kChatBlackListUnit);
    }

    static KEGS_NEW_BLACKLIST_USER_ACK read_from(Reader& r) {
        KEGS_NEW_BLACKLIST_USER_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_kChatBlackListUnit = Codec<std::remove_cvref_t<decltype(v.m_kChatBlackListUnit)>>::get(r);
        return v;
    }
};

struct KEGS_NEW_BLACKLIST_USER_REQ {
    std::u16string m_wstrNickName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
    }

    static KEGS_NEW_BLACKLIST_USER_REQ read_from(Reader& r) {
        KEGS_NEW_BLACKLIST_USER_REQ v;
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        return v;
    }
};

struct KEGS_NEW_CHARACTER_TRADE_BLOCK_MSG_NOT {
    std::u16string m_wstrNewCharTBMsg{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrNewCharTBMsg)>>::put(w, m_wstrNewCharTBMsg);
    }

    static KEGS_NEW_CHARACTER_TRADE_BLOCK_MSG_NOT read_from(Reader& r) {
        KEGS_NEW_CHARACTER_TRADE_BLOCK_MSG_NOT v;
        v.m_wstrNewCharTBMsg = Codec<std::remove_cvref_t<decltype(v.m_wstrNewCharTBMsg)>>::get(r);
        return v;
    }
};

struct KEGS_NEW_POST_BLACK_LIST_ACK {
    std::int32_t   m_iOK{};  // int
    std::u16string m_wstrNickName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
    }

    static KEGS_NEW_POST_BLACK_LIST_ACK read_from(Reader& r) {
        KEGS_NEW_POST_BLACK_LIST_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        return v;
    }
};

struct KEGS_NEW_POST_BLACK_LIST_REQ {
    std::u16string m_wstrNickName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
    }

    static KEGS_NEW_POST_BLACK_LIST_REQ read_from(Reader& r) {
        KEGS_NEW_POST_BLACK_LIST_REQ v;
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        return v;
    }
};

struct KEGS_NEW_PRESENT_CASH_INVENTORY_NOT {
    std::int32_t m_iPresentCount{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iPresentCount)>>::put(w, m_iPresentCount);
    }

    static KEGS_NEW_PRESENT_CASH_INVENTORY_NOT read_from(Reader& r) {
        KEGS_NEW_PRESENT_CASH_INVENTORY_NOT v;
        v.m_iPresentCount = Codec<std::remove_cvref_t<decltype(v.m_iPresentCount)>>::get(r);
        return v;
    }
};

struct KEGS_NEW_QUEST_REQ {
    std::int32_t m_iQuestID{};  // int
    std::int32_t m_iTalkNPCID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iQuestID)>>::put(w, m_iQuestID);
        Codec<std::remove_cvref_t<decltype(m_iTalkNPCID)>>::put(w, m_iTalkNPCID);
    }

    static KEGS_NEW_QUEST_REQ read_from(Reader& r) {
        KEGS_NEW_QUEST_REQ v;
        v.m_iQuestID = Codec<std::remove_cvref_t<decltype(v.m_iQuestID)>>::get(r);
        v.m_iTalkNPCID = Codec<std::remove_cvref_t<decltype(v.m_iTalkNPCID)>>::get(r);
        return v;
    }
};

struct KEGS_NEW_USER_JOIN_ACK {
    std::int32_t m_iOK{};  // int
    std::int64_t m_iUserUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
    }

    static KEGS_NEW_USER_JOIN_ACK read_from(Reader& r) {
        KEGS_NEW_USER_JOIN_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        return v;
    }
};

