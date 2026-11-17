// ---------------------------------------------------------------------------
// packets_7.inc - GENERATED FILE, DO NOT EDIT BY HAND.
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

struct KEGS_JOIN_SQUARE_NOT {
    KSquareUserInfo m_kUserInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kUserInfo)>>::put(w, m_kUserInfo);
    }

    static KEGS_JOIN_SQUARE_NOT read_from(Reader& r) {
        KEGS_JOIN_SQUARE_NOT v;
        v.m_kUserInfo = Codec<std::remove_cvref_t<decltype(v.m_kUserInfo)>>::get(r);
        return v;
    }
};

struct KStat {
    std::int32_t m_iBaseHP{};  // int
    std::int32_t m_iAtkPhysic{};  // int
    std::int32_t m_iAtkMagic{};  // int
    std::int32_t m_iDefPhysic{};  // int
    std::int32_t m_iDefMagic{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iBaseHP)>>::put(w, m_iBaseHP);
        Codec<std::remove_cvref_t<decltype(m_iAtkPhysic)>>::put(w, m_iAtkPhysic);
        Codec<std::remove_cvref_t<decltype(m_iAtkMagic)>>::put(w, m_iAtkMagic);
        Codec<std::remove_cvref_t<decltype(m_iDefPhysic)>>::put(w, m_iDefPhysic);
        Codec<std::remove_cvref_t<decltype(m_iDefMagic)>>::put(w, m_iDefMagic);
    }

    static KStat read_from(Reader& r) {
        KStat v;
        v.m_iBaseHP = Codec<std::remove_cvref_t<decltype(v.m_iBaseHP)>>::get(r);
        v.m_iAtkPhysic = Codec<std::remove_cvref_t<decltype(v.m_iAtkPhysic)>>::get(r);
        v.m_iAtkMagic = Codec<std::remove_cvref_t<decltype(v.m_iAtkMagic)>>::get(r);
        v.m_iDefPhysic = Codec<std::remove_cvref_t<decltype(v.m_iDefPhysic)>>::get(r);
        v.m_iDefMagic = Codec<std::remove_cvref_t<decltype(v.m_iDefMagic)>>::get(r);
        return v;
    }
};

struct KEGS_CHANGE_EQUIPPED_ITEM_IN_ROOM_NOT {
    char                            m_cChangeEquippedPacketType{};
    std::int64_t                    m_UnitUID{};  // UidType
    std::vector<KInventoryItemInfo> m_vecInventorySlotInfo{};  // std::vector< KInventoryItemInfo >
    KStat                           m_kGameStat{};
    std::int32_t                    m_iEquippedTitleID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cChangeEquippedPacketType)>>::put(w, m_cChangeEquippedPacketType);
        Codec<std::remove_cvref_t<decltype(m_UnitUID)>>::put(w, m_UnitUID);
        Codec<std::remove_cvref_t<decltype(m_vecInventorySlotInfo)>>::put(w, m_vecInventorySlotInfo);
        Codec<std::remove_cvref_t<decltype(m_kGameStat)>>::put(w, m_kGameStat);
        Codec<std::remove_cvref_t<decltype(m_iEquippedTitleID)>>::put(w, m_iEquippedTitleID);
    }

    static KEGS_CHANGE_EQUIPPED_ITEM_IN_ROOM_NOT read_from(Reader& r) {
        KEGS_CHANGE_EQUIPPED_ITEM_IN_ROOM_NOT v;
        v.m_cChangeEquippedPacketType = Codec<std::remove_cvref_t<decltype(v.m_cChangeEquippedPacketType)>>::get(r);
        v.m_UnitUID = Codec<std::remove_cvref_t<decltype(v.m_UnitUID)>>::get(r);
        v.m_vecInventorySlotInfo = Codec<std::remove_cvref_t<decltype(v.m_vecInventorySlotInfo)>>::get(r);
        v.m_kGameStat = Codec<std::remove_cvref_t<decltype(v.m_kGameStat)>>::get(r);
        v.m_iEquippedTitleID = Codec<std::remove_cvref_t<decltype(v.m_iEquippedTitleID)>>::get(r);
        return v;
    }
};

struct KEGS_CHANGE_GAME_STAT_INFO_IN_ROOM_NOT {
    std::int64_t m_iUnitUID{};  // UidType
    KStat        m_kGameStat{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_kGameStat)>>::put(w, m_kGameStat);
    }

    static KEGS_CHANGE_GAME_STAT_INFO_IN_ROOM_NOT read_from(Reader& r) {
        KEGS_CHANGE_GAME_STAT_INFO_IN_ROOM_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_kGameStat = Codec<std::remove_cvref_t<decltype(v.m_kGameStat)>>::get(r);
        return v;
    }
};

struct KEGS_CHAR_LEVEL_UP_NOT {
    std::int64_t m_iUnitUID{};  // UidType
    std::uint8_t m_ucLevel{};  // UCHAR
    KStat        m_kBaseStat{};
    KStat        m_kGameStat{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_ucLevel)>>::put(w, m_ucLevel);
        Codec<std::remove_cvref_t<decltype(m_kBaseStat)>>::put(w, m_kBaseStat);
        Codec<std::remove_cvref_t<decltype(m_kGameStat)>>::put(w, m_kGameStat);
    }

    static KEGS_CHAR_LEVEL_UP_NOT read_from(Reader& r) {
        KEGS_CHAR_LEVEL_UP_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_ucLevel = Codec<std::remove_cvref_t<decltype(v.m_ucLevel)>>::get(r);
        v.m_kBaseStat = Codec<std::remove_cvref_t<decltype(v.m_kBaseStat)>>::get(r);
        v.m_kGameStat = Codec<std::remove_cvref_t<decltype(v.m_kGameStat)>>::get(r);
        return v;
    }
};

struct KEGS_UPDATE_BUFF_INFO_IN_ROOM_NOT {
    std::int64_t           m_iUnitUID{};  // UidType
    std::vector<KBuffInfo> m_vecActivateBuffList{};  // std::vector< KBuffInfo >
    KStat                  m_kGameStat{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_vecActivateBuffList)>>::put(w, m_vecActivateBuffList);
        Codec<std::remove_cvref_t<decltype(m_kGameStat)>>::put(w, m_kGameStat);
    }

    static KEGS_UPDATE_BUFF_INFO_IN_ROOM_NOT read_from(Reader& r) {
        KEGS_UPDATE_BUFF_INFO_IN_ROOM_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_vecActivateBuffList = Codec<std::remove_cvref_t<decltype(v.m_vecActivateBuffList)>>::get(r);
        v.m_kGameStat = Codec<std::remove_cvref_t<decltype(v.m_kGameStat)>>::get(r);
        return v;
    }
};

struct KEGS_ZERO_ENDURANCE_ITEM_IN_ROOM_NOT {
    std::int64_t                    m_UnitUID{};  // UidType
    std::vector<KInventoryItemInfo> m_vecInventorySlotInfo{};  // std::vector< KInventoryItemInfo >
    KStat                           m_kGameStat{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_UnitUID)>>::put(w, m_UnitUID);
        Codec<std::remove_cvref_t<decltype(m_vecInventorySlotInfo)>>::put(w, m_vecInventorySlotInfo);
        Codec<std::remove_cvref_t<decltype(m_kGameStat)>>::put(w, m_kGameStat);
    }

    static KEGS_ZERO_ENDURANCE_ITEM_IN_ROOM_NOT read_from(Reader& r) {
        KEGS_ZERO_ENDURANCE_ITEM_IN_ROOM_NOT v;
        v.m_UnitUID = Codec<std::remove_cvref_t<decltype(v.m_UnitUID)>>::get(r);
        v.m_vecInventorySlotInfo = Codec<std::remove_cvref_t<decltype(v.m_vecInventorySlotInfo)>>::get(r);
        v.m_kGameStat = Codec<std::remove_cvref_t<decltype(v.m_kGameStat)>>::get(r);
        return v;
    }
};

struct KStatIncreaseRate {
    float m_fIncreaseAtkPhysicRate{};
    float m_fIncreaseAtkMagicRate{};
    float m_fIncreaseDefPhysicRate{};
    float m_fIncreaseDefMagicRate{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_fIncreaseAtkPhysicRate)>>::put(w, m_fIncreaseAtkPhysicRate);
        Codec<std::remove_cvref_t<decltype(m_fIncreaseAtkMagicRate)>>::put(w, m_fIncreaseAtkMagicRate);
        Codec<std::remove_cvref_t<decltype(m_fIncreaseDefPhysicRate)>>::put(w, m_fIncreaseDefPhysicRate);
        Codec<std::remove_cvref_t<decltype(m_fIncreaseDefMagicRate)>>::put(w, m_fIncreaseDefMagicRate);
    }

    static KStatIncreaseRate read_from(Reader& r) {
        KStatIncreaseRate v;
        v.m_fIncreaseAtkPhysicRate = Codec<std::remove_cvref_t<decltype(v.m_fIncreaseAtkPhysicRate)>>::get(r);
        v.m_fIncreaseAtkMagicRate = Codec<std::remove_cvref_t<decltype(v.m_fIncreaseAtkMagicRate)>>::get(r);
        v.m_fIncreaseDefPhysicRate = Codec<std::remove_cvref_t<decltype(v.m_fIncreaseDefPhysicRate)>>::get(r);
        v.m_fIncreaseDefMagicRate = Codec<std::remove_cvref_t<decltype(v.m_fIncreaseDefMagicRate)>>::get(r);
        return v;
    }
};

struct KStatRelationLevel {
    float m_fBaseHPRelLV{};
    float m_fAtkPhysicRelLV{};
    float m_fAtkMagicRelLV{};
    float m_fDefPhysicRelLV{};
    float m_fDefMagicRelLV{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_fBaseHPRelLV)>>::put(w, m_fBaseHPRelLV);
        Codec<std::remove_cvref_t<decltype(m_fAtkPhysicRelLV)>>::put(w, m_fAtkPhysicRelLV);
        Codec<std::remove_cvref_t<decltype(m_fAtkMagicRelLV)>>::put(w, m_fAtkMagicRelLV);
        Codec<std::remove_cvref_t<decltype(m_fDefPhysicRelLV)>>::put(w, m_fDefPhysicRelLV);
        Codec<std::remove_cvref_t<decltype(m_fDefMagicRelLV)>>::put(w, m_fDefMagicRelLV);
    }

    static KStatRelationLevel read_from(Reader& r) {
        KStatRelationLevel v;
        v.m_fBaseHPRelLV = Codec<std::remove_cvref_t<decltype(v.m_fBaseHPRelLV)>>::get(r);
        v.m_fAtkPhysicRelLV = Codec<std::remove_cvref_t<decltype(v.m_fAtkPhysicRelLV)>>::get(r);
        v.m_fAtkMagicRelLV = Codec<std::remove_cvref_t<decltype(v.m_fAtkMagicRelLV)>>::get(r);
        v.m_fDefPhysicRelLV = Codec<std::remove_cvref_t<decltype(v.m_fDefPhysicRelLV)>>::get(r);
        v.m_fDefMagicRelLV = Codec<std::remove_cvref_t<decltype(v.m_fDefMagicRelLV)>>::get(r);
        return v;
    }
};

struct KStateChangeAck {
    std::int32_t  m_iOK{};  // int
    std::uint32_t m_nMapID{};  // UINT

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_nMapID)>>::put(w, m_nMapID);
    }

    static KStateChangeAck read_from(Reader& r) {
        KStateChangeAck v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_nMapID = Codec<std::remove_cvref_t<decltype(v.m_nMapID)>>::get(r);
        return v;
    }
};

struct KStateChangeReq {
    std::uint32_t m_nMapID{};  // UINT

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_nMapID)>>::put(w, m_nMapID);
    }

    static KStateChangeReq read_from(Reader& r) {
        KStateChangeReq v;
        v.m_nMapID = Codec<std::remove_cvref_t<decltype(v.m_nMapID)>>::get(r);
        return v;
    }
};

struct KStatisticsDetailData {
    std::int32_t   m_iNum{};  // int
    std::u16string m_wszString{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iNum)>>::put(w, m_iNum);
        Codec<std::remove_cvref_t<decltype(m_wszString)>>::put(w, m_wszString);
    }

    static KStatisticsDetailData read_from(Reader& r) {
        KStatisticsDetailData v;
        v.m_iNum = Codec<std::remove_cvref_t<decltype(v.m_iNum)>>::get(r);
        v.m_wszString = Codec<std::remove_cvref_t<decltype(v.m_wszString)>>::get(r);
        return v;
    }
};

struct KStatisticsKey {
    std::vector<std::int32_t>   m_vecIntKey{};  // std::vector< int >
    std::vector<std::u16string> m_vecStringKey{};  // std::vector< std::wstring >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecIntKey)>>::put(w, m_vecIntKey);
        Codec<std::remove_cvref_t<decltype(m_vecStringKey)>>::put(w, m_vecStringKey);
    }

    static KStatisticsKey read_from(Reader& r) {
        KStatisticsKey v;
        v.m_vecIntKey = Codec<std::remove_cvref_t<decltype(v.m_vecIntKey)>>::get(r);
        v.m_vecStringKey = Codec<std::remove_cvref_t<decltype(v.m_vecStringKey)>>::get(r);
        return v;
    }
};

struct KStatisticsInfo {
    std::int32_t   m_iStatisticsID{};  // int
    KStatisticsKey m_kKey{};
    std::int32_t   m_iIndex{};  // int
    std::int32_t   m_iCount{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iStatisticsID)>>::put(w, m_iStatisticsID);
        Codec<std::remove_cvref_t<decltype(m_kKey)>>::put(w, m_kKey);
        Codec<std::remove_cvref_t<decltype(m_iIndex)>>::put(w, m_iIndex);
        Codec<std::remove_cvref_t<decltype(m_iCount)>>::put(w, m_iCount);
    }

    static KStatisticsInfo read_from(Reader& r) {
        KStatisticsInfo v;
        v.m_iStatisticsID = Codec<std::remove_cvref_t<decltype(v.m_iStatisticsID)>>::get(r);
        v.m_kKey = Codec<std::remove_cvref_t<decltype(v.m_kKey)>>::get(r);
        v.m_iIndex = Codec<std::remove_cvref_t<decltype(v.m_iIndex)>>::get(r);
        v.m_iCount = Codec<std::remove_cvref_t<decltype(v.m_iCount)>>::get(r);
        return v;
    }
};

struct KE_STATISTICS_INFO_NOT {
    std::vector<KStatisticsInfo> m_vecStatisticsInfo{};  // std::vector< KStatisticsInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecStatisticsInfo)>>::put(w, m_vecStatisticsInfo);
    }

    static KE_STATISTICS_INFO_NOT read_from(Reader& r) {
        KE_STATISTICS_INFO_NOT v;
        v.m_vecStatisticsInfo = Codec<std::remove_cvref_t<decltype(v.m_vecStatisticsInfo)>>::get(r);
        return v;
    }
};

struct KSubMissionInstance {
    std::int32_t m_iID{};  // int
    std::int16_t m_sClearData{};  // short
    bool         m_bIsSuccess{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iID)>>::put(w, m_iID);
        Codec<std::remove_cvref_t<decltype(m_sClearData)>>::put(w, m_sClearData);
        Codec<std::remove_cvref_t<decltype(m_bIsSuccess)>>::put(w, m_bIsSuccess);
    }

    static KSubMissionInstance read_from(Reader& r) {
        KSubMissionInstance v;
        v.m_iID = Codec<std::remove_cvref_t<decltype(v.m_iID)>>::get(r);
        v.m_sClearData = Codec<std::remove_cvref_t<decltype(v.m_sClearData)>>::get(r);
        v.m_bIsSuccess = Codec<std::remove_cvref_t<decltype(v.m_bIsSuccess)>>::get(r);
        return v;
    }
};

struct KMissionInstance {
    std::int32_t                     m_iID{};  // int
    std::vector<KSubMissionInstance> m_vecSubMissionInstance{};  // std::vector< KSubMissionInstance >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iID)>>::put(w, m_iID);
        Codec<std::remove_cvref_t<decltype(m_vecSubMissionInstance)>>::put(w, m_vecSubMissionInstance);
    }

    static KMissionInstance read_from(Reader& r) {
        KMissionInstance v;
        v.m_iID = Codec<std::remove_cvref_t<decltype(v.m_iID)>>::get(r);
        v.m_vecSubMissionInstance = Codec<std::remove_cvref_t<decltype(v.m_vecSubMissionInstance)>>::get(r);
        return v;
    }
};

struct KEGS_NEW_MISSION_NOT {
    bool                          m_bFirst{};
    std::vector<KMissionInstance> m_vecNewMission{};  // std::vector< KMissionInstance >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bFirst)>>::put(w, m_bFirst);
        Codec<std::remove_cvref_t<decltype(m_vecNewMission)>>::put(w, m_vecNewMission);
    }

    static KEGS_NEW_MISSION_NOT read_from(Reader& r) {
        KEGS_NEW_MISSION_NOT v;
        v.m_bFirst = Codec<std::remove_cvref_t<decltype(v.m_bFirst)>>::get(r);
        v.m_vecNewMission = Codec<std::remove_cvref_t<decltype(v.m_vecNewMission)>>::get(r);
        return v;
    }
};

struct KEGS_UPDATE_MISSION_NOT {
    std::vector<KMissionInstance> m_vecMissionInst{};  // std::vector< KMissionInstance >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecMissionInst)>>::put(w, m_vecMissionInst);
    }

    static KEGS_UPDATE_MISSION_NOT read_from(Reader& r) {
        KEGS_UPDATE_MISSION_NOT v;
        v.m_vecMissionInst = Codec<std::remove_cvref_t<decltype(v.m_vecMissionInst)>>::get(r);
        return v;
    }
};

struct KSubQuestData {
    std::int32_t m_iInInventoryItemCount{};  // int
    bool         m_bSussece{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iInInventoryItemCount)>>::put(w, m_iInInventoryItemCount);
        Codec<std::remove_cvref_t<decltype(m_bSussece)>>::put(w, m_bSussece);
    }

    static KSubQuestData read_from(Reader& r) {
        KSubQuestData v;
        v.m_iInInventoryItemCount = Codec<std::remove_cvref_t<decltype(v.m_iInInventoryItemCount)>>::get(r);
        v.m_bSussece = Codec<std::remove_cvref_t<decltype(v.m_bSussece)>>::get(r);
        return v;
    }
};

struct KSubQuestInfo {
    std::map<std::int32_t, KSubQuestData> m_mapSubQuestInfo{};  // std::map<int,KSubQuestData>

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_mapSubQuestInfo)>>::put(w, m_mapSubQuestInfo);
    }

    static KSubQuestInfo read_from(Reader& r) {
        KSubQuestInfo v;
        v.m_mapSubQuestInfo = Codec<std::remove_cvref_t<decltype(v.m_mapSubQuestInfo)>>::get(r);
        return v;
    }
};

struct KEGS_END_GAME_DUNGEON_RESULT_DATA_NOT {
    std::int32_t                                                  m_nSec{};  // int
    bool                                                          m_bIsWin{};
    std::int32_t                                                  m_iDungeonID{};  // int
    char                                                          m_cDifficulty{};
    char                                                          m_cDungeonMode{};
    std::vector<KDungeonUnitResultInfo>                           m_vecDungeonUnitInfo{};  // std::vector< KDungeonUnitResultInfo >
    std::map<std::int64_t, std::map<std::int32_t, KSubQuestInfo>> m_mapOngoingQuest{};  // std::map< UidType, std::map< int, KSubQuestInfo > >
    bool                                                          m_bIsHenirDungeon{};
    std::int32_t                                                  m_iDungeonEventPoint{};  // int
    std::int32_t                                                  m_iHenirEventPoint{};  // int
    std::map<std::int64_t, bool>                                  m_mapHaveExpInDungeon{};  // std::map< UidType, bool >
    bool                                                          m_bIsClearDefenceDungeon{};
    std::int32_t                                                  m_iProtectedSmallCrystal{};  // int
    bool                                                          m_bComeBackUserInParty{};
    std::map<std::int64_t, bool>                                  m_mapSuitableLevelInfo{};  // std::map< UidType, bool >
    bool                                                          m_bSpecChar1{};
    bool                                                          m_bSpecChar2{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_nSec)>>::put(w, m_nSec);
        Codec<std::remove_cvref_t<decltype(m_bIsWin)>>::put(w, m_bIsWin);
        Codec<std::remove_cvref_t<decltype(m_iDungeonID)>>::put(w, m_iDungeonID);
        Codec<std::remove_cvref_t<decltype(m_cDifficulty)>>::put(w, m_cDifficulty);
        Codec<std::remove_cvref_t<decltype(m_cDungeonMode)>>::put(w, m_cDungeonMode);
        Codec<std::remove_cvref_t<decltype(m_vecDungeonUnitInfo)>>::put(w, m_vecDungeonUnitInfo);
        Codec<std::remove_cvref_t<decltype(m_mapOngoingQuest)>>::put(w, m_mapOngoingQuest);
        Codec<std::remove_cvref_t<decltype(m_bIsHenirDungeon)>>::put(w, m_bIsHenirDungeon);
        Codec<std::remove_cvref_t<decltype(m_iDungeonEventPoint)>>::put(w, m_iDungeonEventPoint);
        Codec<std::remove_cvref_t<decltype(m_iHenirEventPoint)>>::put(w, m_iHenirEventPoint);
        Codec<std::remove_cvref_t<decltype(m_mapHaveExpInDungeon)>>::put(w, m_mapHaveExpInDungeon);
        Codec<std::remove_cvref_t<decltype(m_bIsClearDefenceDungeon)>>::put(w, m_bIsClearDefenceDungeon);
        Codec<std::remove_cvref_t<decltype(m_iProtectedSmallCrystal)>>::put(w, m_iProtectedSmallCrystal);
        Codec<std::remove_cvref_t<decltype(m_bComeBackUserInParty)>>::put(w, m_bComeBackUserInParty);
        Codec<std::remove_cvref_t<decltype(m_mapSuitableLevelInfo)>>::put(w, m_mapSuitableLevelInfo);
        Codec<std::remove_cvref_t<decltype(m_bSpecChar1)>>::put(w, m_bSpecChar1);
        Codec<std::remove_cvref_t<decltype(m_bSpecChar2)>>::put(w, m_bSpecChar2);
    }

    static KEGS_END_GAME_DUNGEON_RESULT_DATA_NOT read_from(Reader& r) {
        KEGS_END_GAME_DUNGEON_RESULT_DATA_NOT v;
        v.m_nSec = Codec<std::remove_cvref_t<decltype(v.m_nSec)>>::get(r);
        v.m_bIsWin = Codec<std::remove_cvref_t<decltype(v.m_bIsWin)>>::get(r);
        v.m_iDungeonID = Codec<std::remove_cvref_t<decltype(v.m_iDungeonID)>>::get(r);
        v.m_cDifficulty = Codec<std::remove_cvref_t<decltype(v.m_cDifficulty)>>::get(r);
        v.m_cDungeonMode = Codec<std::remove_cvref_t<decltype(v.m_cDungeonMode)>>::get(r);
        v.m_vecDungeonUnitInfo = Codec<std::remove_cvref_t<decltype(v.m_vecDungeonUnitInfo)>>::get(r);
        v.m_mapOngoingQuest = Codec<std::remove_cvref_t<decltype(v.m_mapOngoingQuest)>>::get(r);
        v.m_bIsHenirDungeon = Codec<std::remove_cvref_t<decltype(v.m_bIsHenirDungeon)>>::get(r);
        v.m_iDungeonEventPoint = Codec<std::remove_cvref_t<decltype(v.m_iDungeonEventPoint)>>::get(r);
        v.m_iHenirEventPoint = Codec<std::remove_cvref_t<decltype(v.m_iHenirEventPoint)>>::get(r);
        v.m_mapHaveExpInDungeon = Codec<std::remove_cvref_t<decltype(v.m_mapHaveExpInDungeon)>>::get(r);
        v.m_bIsClearDefenceDungeon = Codec<std::remove_cvref_t<decltype(v.m_bIsClearDefenceDungeon)>>::get(r);
        v.m_iProtectedSmallCrystal = Codec<std::remove_cvref_t<decltype(v.m_iProtectedSmallCrystal)>>::get(r);
        v.m_bComeBackUserInParty = Codec<std::remove_cvref_t<decltype(v.m_bComeBackUserInParty)>>::get(r);
        v.m_mapSuitableLevelInfo = Codec<std::remove_cvref_t<decltype(v.m_mapSuitableLevelInfo)>>::get(r);
        v.m_bSpecChar1 = Codec<std::remove_cvref_t<decltype(v.m_bSpecChar1)>>::get(r);
        v.m_bSpecChar2 = Codec<std::remove_cvref_t<decltype(v.m_bSpecChar2)>>::get(r);
        return v;
    }
};

struct KERM_SET_QUEST_ITEM_INFO_NOT {
    std::map<std::int32_t, KSubQuestInfo> m_mapDropQuestItembyIngQuest{};  // std::map<int, KSubQuestInfo>
    std::set<std::int32_t>                m_setQuestInfo{};  // std::set< int >
    std::set<std::int32_t>                m_setGoingQuestInfo{};  // std::set< int >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_mapDropQuestItembyIngQuest)>>::put(w, m_mapDropQuestItembyIngQuest);
        Codec<std::remove_cvref_t<decltype(m_setQuestInfo)>>::put(w, m_setQuestInfo);
        Codec<std::remove_cvref_t<decltype(m_setGoingQuestInfo)>>::put(w, m_setGoingQuestInfo);
    }

    static KERM_SET_QUEST_ITEM_INFO_NOT read_from(Reader& r) {
        KERM_SET_QUEST_ITEM_INFO_NOT v;
        v.m_mapDropQuestItembyIngQuest = Codec<std::remove_cvref_t<decltype(v.m_mapDropQuestItembyIngQuest)>>::get(r);
        v.m_setQuestInfo = Codec<std::remove_cvref_t<decltype(v.m_setQuestInfo)>>::get(r);
        v.m_setGoingQuestInfo = Codec<std::remove_cvref_t<decltype(v.m_setGoingQuestInfo)>>::get(r);
        return v;
    }
};

struct KSubQuestInstance {
    std::int32_t m_iID{};  // int
    std::uint8_t m_ucClearData{};  // UCHAR
    bool         m_bIsSuccess{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iID)>>::put(w, m_iID);
        Codec<std::remove_cvref_t<decltype(m_ucClearData)>>::put(w, m_ucClearData);
        Codec<std::remove_cvref_t<decltype(m_bIsSuccess)>>::put(w, m_bIsSuccess);
    }

    static KSubQuestInstance read_from(Reader& r) {
        KSubQuestInstance v;
        v.m_iID = Codec<std::remove_cvref_t<decltype(v.m_iID)>>::get(r);
        v.m_ucClearData = Codec<std::remove_cvref_t<decltype(v.m_ucClearData)>>::get(r);
        v.m_bIsSuccess = Codec<std::remove_cvref_t<decltype(v.m_bIsSuccess)>>::get(r);
        return v;
    }
};

struct KQuestInstance {
    std::int32_t                   m_iID{};  // int
    std::int64_t                   m_OwnorUnitUID{};  // UidType
    std::vector<KSubQuestInstance> m_vecSubQuestInstance{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iID)>>::put(w, m_iID);
        Codec<std::remove_cvref_t<decltype(m_OwnorUnitUID)>>::put(w, m_OwnorUnitUID);
        Codec<std::remove_cvref_t<decltype(m_vecSubQuestInstance)>>::put(w, m_vecSubQuestInstance);
    }

    static KQuestInstance read_from(Reader& r) {
        KQuestInstance v;
        v.m_iID = Codec<std::remove_cvref_t<decltype(v.m_iID)>>::get(r);
        v.m_OwnorUnitUID = Codec<std::remove_cvref_t<decltype(v.m_OwnorUnitUID)>>::get(r);
        v.m_vecSubQuestInstance = Codec<std::remove_cvref_t<decltype(v.m_vecSubQuestInstance)>>::get(r);
        return v;
    }
};

struct KEGS_NEW_QUEST_NOT {
    KQuestInstance                  m_QuestInst{};
    std::vector<KInventoryItemInfo> m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_QuestInst)>>::put(w, m_QuestInst);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
    }

    static KEGS_NEW_QUEST_NOT read_from(Reader& r) {
        KEGS_NEW_QUEST_NOT v;
        v.m_QuestInst = Codec<std::remove_cvref_t<decltype(v.m_QuestInst)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        return v;
    }
};

struct KEGS_SELECT_UNIT_2_NOT {
    std::int32_t                         m_iOK{};  // int
    std::vector<KQuestInstance>          m_vecQuest{};  // std::vector< KQuestInstance >
    std::vector<KCompleteQuestInfo>      m_vecCompletQuest{};  // std::vector< KCompleteQuestInfo >
    std::map<std::int32_t, std::int32_t> m_mapRandomQuestList{};  // std::map< int, int >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_vecQuest)>>::put(w, m_vecQuest);
        Codec<std::remove_cvref_t<decltype(m_vecCompletQuest)>>::put(w, m_vecCompletQuest);
        Codec<std::remove_cvref_t<decltype(m_mapRandomQuestList)>>::put(w, m_mapRandomQuestList);
    }

    static KEGS_SELECT_UNIT_2_NOT read_from(Reader& r) {
        KEGS_SELECT_UNIT_2_NOT v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_vecQuest = Codec<std::remove_cvref_t<decltype(v.m_vecQuest)>>::get(r);
        v.m_vecCompletQuest = Codec<std::remove_cvref_t<decltype(v.m_vecCompletQuest)>>::get(r);
        v.m_mapRandomQuestList = Codec<std::remove_cvref_t<decltype(v.m_mapRandomQuestList)>>::get(r);
        return v;
    }
};

struct KEGS_UPDATE_QUEST_NOT {
    std::vector<KQuestInstance> m_vecQuestInst{};  // std::vector< KQuestInstance >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecQuestInst)>>::put(w, m_vecQuestInst);
    }

    static KEGS_UPDATE_QUEST_NOT read_from(Reader& r) {
        KEGS_UPDATE_QUEST_NOT v;
        v.m_vecQuestInst = Codec<std::remove_cvref_t<decltype(v.m_vecQuestInst)>>::get(r);
        return v;
    }
};

struct KEGS_UPDATE_QUEST_REQ {
    KQuestInstance m_QuestInst{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_QuestInst)>>::put(w, m_QuestInst);
    }

    static KEGS_UPDATE_QUEST_REQ read_from(Reader& r) {
        KEGS_UPDATE_QUEST_REQ v;
        v.m_QuestInst = Codec<std::remove_cvref_t<decltype(v.m_QuestInst)>>::get(r);
        return v;
    }
};

struct KSuccessTradeData {
    bool                            m_bIsSuccess{};
    std::vector<KInventoryItemInfo> m_vecInventorySlotInfo{};  // std::vector< KInventoryItemInfo >
    KERM_CONFIRM_TRADE_NOT          m_kConfirmTradeNot{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bIsSuccess)>>::put(w, m_bIsSuccess);
        Codec<std::remove_cvref_t<decltype(m_vecInventorySlotInfo)>>::put(w, m_vecInventorySlotInfo);
        Codec<std::remove_cvref_t<decltype(m_kConfirmTradeNot)>>::put(w, m_kConfirmTradeNot);
    }

    static KSuccessTradeData read_from(Reader& r) {
        KSuccessTradeData v;
        v.m_bIsSuccess = Codec<std::remove_cvref_t<decltype(v.m_bIsSuccess)>>::get(r);
        v.m_vecInventorySlotInfo = Codec<std::remove_cvref_t<decltype(v.m_vecInventorySlotInfo)>>::get(r);
        v.m_kConfirmTradeNot = Codec<std::remove_cvref_t<decltype(v.m_kConfirmTradeNot)>>::get(r);
        return v;
    }
};

struct KTCClearInfo {
    std::int32_t   m_iTCID{};  // int
    std::u16string m_wstrClearTime{};  // std::wstring
    bool           m_bNew{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iTCID)>>::put(w, m_iTCID);
        Codec<std::remove_cvref_t<decltype(m_wstrClearTime)>>::put(w, m_wstrClearTime);
        Codec<std::remove_cvref_t<decltype(m_bNew)>>::put(w, m_bNew);
    }

    static KTCClearInfo read_from(Reader& r) {
        KTCClearInfo v;
        v.m_iTCID = Codec<std::remove_cvref_t<decltype(v.m_iTCID)>>::get(r);
        v.m_wstrClearTime = Codec<std::remove_cvref_t<decltype(v.m_wstrClearTime)>>::get(r);
        v.m_bNew = Codec<std::remove_cvref_t<decltype(v.m_bNew)>>::get(r);
        return v;
    }
};

struct KDBE_UPDATE_UNIT_INFO_REQ {
    bool                                              m_bFinal{};
    std::int32_t                                      m_iEventID{};  // int
    std::int64_t                                      m_iUnitUID{};  // UidType
    std::int32_t                                      m_iEXP{};  // int
    std::int32_t                                      m_iLevel{};  // int
    std::int32_t                                      m_iED{};  // int
    std::int32_t                                      m_iRealDataED{};  // int
    std::int32_t                                      m_iOfficialMatchCnt{};  // int
    std::int32_t                                      m_iRating{};  // int
    std::int32_t                                      m_iMaxRating{};  // int
    std::int32_t                                      m_iRPoint{};  // int
    std::int32_t                                      m_iAPoint{};  // int
    bool                                              m_bIsWinBeforeMatch{};
    char                                              m_cEmblemEnum{};
    float                                             m_fKFactor{};
    std::int32_t                                      m_iWin{};  // int
    std::int32_t                                      m_iLose{};  // int
    KLastPositionInfo                                 m_kLastPos{};
    std::map<std::int32_t, KDungeonClearInfo>         m_mapDungeonClear{};  // std::map< int, KDungeonClearInfo >
    std::map<std::int32_t, KTCClearInfo>              m_mapTCClear{};  // std::map< int, KTCClearInfo >
    std::map<std::int32_t, KDungeonPlayInfo>          m_mapDungeonPlay{};  // std::map< int, KDungeonPlayInfo >
    KItemQuantityUpdate                               m_kItemQuantityUpdate{};
    std::map<std::int64_t, std::int32_t>              m_kItemEnduranceUpdate{};  // KItemEnduranceUpdate
    std::map<std::int64_t, KItemPosition>             m_kItemPositionUpdate{};  // KItemPositionUpdate
    std::vector<KQuestUpdate>                         m_vecQuestData{};  // std::vector< KQuestUpdate >
    std::vector<KMissionUpdate>                       m_vecMissionData{};  // std::vector< KMissionUpdate >
    std::int32_t                                      m_iInitTitleID{};  // int
    std::int32_t                                      m_iEquippedTitleID{};  // int
    std::map<std::int32_t, std::vector<std::int32_t>> m_mapSkillSlotVector{};  // std::map< int, std::vector<int> >
    std::u16string                                    m_wstrSkillSlotBEndDate{};  // std::wstring
    KDenyOptions                                      m_kDenyOptions{};
    std::int32_t                                      m_iSpirit{};  // int
    bool                                              m_bIsSpiritUpdated{};
    std::int32_t                                      m_iNumResurrectionStone{};  // int
    std::int32_t                                      m_iNumAutoPaymentResStone{};  // int
    std::map<std::int32_t, std::int32_t>              m_mapWishList{};  // std::map< int, int >
    std::vector<KHenirRankingInfo>                    m_vecHenirRanking{};  // std::vector< KHenirRankingInfo >
    std::int32_t                                      m_iGuildUID{};  // int
    std::int32_t                                      m_iHonorPoint{};  // int
    std::vector<KCumulativeTimeEventInfo>             m_vecUpdateEventTime{};  // std::vector< KCumulativeTimeEventInfo >
    std::vector<std::int32_t>                         m_vecGiveUpQuestList{};  // std::vector< int >
    std::vector<KPetInfo>                             m_vecPet{};  // std::vector< KPetInfo >
    std::map<std::int32_t, std::int32_t>              m_mapCharGameCount{};  // std::map< int, int >
    KEGS_HENIR_REWARD_COUNT_NOT                       m_kHenirRewardCnt{};
    KGamePlayStatusContainer                          m_kGamePlayStatus{};
    std::vector<KRecordBuffInfo>                      m_vecRecordBuffInfo{};  // std::vector< KRecordBuffInfo >
    std::vector<KRidingPetInfo>                       m_vecRidingPetList{};
    char                                              m_cUnitRelationshipType{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bFinal)>>::put(w, m_bFinal);
        Codec<std::remove_cvref_t<decltype(m_iEventID)>>::put(w, m_iEventID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iEXP)>>::put(w, m_iEXP);
        Codec<std::remove_cvref_t<decltype(m_iLevel)>>::put(w, m_iLevel);
        Codec<std::remove_cvref_t<decltype(m_iED)>>::put(w, m_iED);
        Codec<std::remove_cvref_t<decltype(m_iRealDataED)>>::put(w, m_iRealDataED);
        Codec<std::remove_cvref_t<decltype(m_iOfficialMatchCnt)>>::put(w, m_iOfficialMatchCnt);
        Codec<std::remove_cvref_t<decltype(m_iRating)>>::put(w, m_iRating);
        Codec<std::remove_cvref_t<decltype(m_iMaxRating)>>::put(w, m_iMaxRating);
        Codec<std::remove_cvref_t<decltype(m_iRPoint)>>::put(w, m_iRPoint);
        Codec<std::remove_cvref_t<decltype(m_iAPoint)>>::put(w, m_iAPoint);
        Codec<std::remove_cvref_t<decltype(m_bIsWinBeforeMatch)>>::put(w, m_bIsWinBeforeMatch);
        Codec<std::remove_cvref_t<decltype(m_cEmblemEnum)>>::put(w, m_cEmblemEnum);
        Codec<std::remove_cvref_t<decltype(m_fKFactor)>>::put(w, m_fKFactor);
        Codec<std::remove_cvref_t<decltype(m_iWin)>>::put(w, m_iWin);
        Codec<std::remove_cvref_t<decltype(m_iLose)>>::put(w, m_iLose);
        Codec<std::remove_cvref_t<decltype(m_kLastPos)>>::put(w, m_kLastPos);
        Codec<std::remove_cvref_t<decltype(m_mapDungeonClear)>>::put(w, m_mapDungeonClear);
        Codec<std::remove_cvref_t<decltype(m_mapTCClear)>>::put(w, m_mapTCClear);
        Codec<std::remove_cvref_t<decltype(m_mapDungeonPlay)>>::put(w, m_mapDungeonPlay);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_kItemEnduranceUpdate)>>::put(w, m_kItemEnduranceUpdate);
        Codec<std::remove_cvref_t<decltype(m_kItemPositionUpdate)>>::put(w, m_kItemPositionUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecQuestData)>>::put(w, m_vecQuestData);
        Codec<std::remove_cvref_t<decltype(m_vecMissionData)>>::put(w, m_vecMissionData);
        Codec<std::remove_cvref_t<decltype(m_iInitTitleID)>>::put(w, m_iInitTitleID);
        Codec<std::remove_cvref_t<decltype(m_iEquippedTitleID)>>::put(w, m_iEquippedTitleID);
        Codec<std::remove_cvref_t<decltype(m_mapSkillSlotVector)>>::put(w, m_mapSkillSlotVector);
        Codec<std::remove_cvref_t<decltype(m_wstrSkillSlotBEndDate)>>::put(w, m_wstrSkillSlotBEndDate);
        Codec<std::remove_cvref_t<decltype(m_kDenyOptions)>>::put(w, m_kDenyOptions);
        Codec<std::remove_cvref_t<decltype(m_iSpirit)>>::put(w, m_iSpirit);
        Codec<std::remove_cvref_t<decltype(m_bIsSpiritUpdated)>>::put(w, m_bIsSpiritUpdated);
        Codec<std::remove_cvref_t<decltype(m_iNumResurrectionStone)>>::put(w, m_iNumResurrectionStone);
        Codec<std::remove_cvref_t<decltype(m_iNumAutoPaymentResStone)>>::put(w, m_iNumAutoPaymentResStone);
        Codec<std::remove_cvref_t<decltype(m_mapWishList)>>::put(w, m_mapWishList);
        Codec<std::remove_cvref_t<decltype(m_vecHenirRanking)>>::put(w, m_vecHenirRanking);
        Codec<std::remove_cvref_t<decltype(m_iGuildUID)>>::put(w, m_iGuildUID);
        Codec<std::remove_cvref_t<decltype(m_iHonorPoint)>>::put(w, m_iHonorPoint);
        Codec<std::remove_cvref_t<decltype(m_vecUpdateEventTime)>>::put(w, m_vecUpdateEventTime);
        Codec<std::remove_cvref_t<decltype(m_vecGiveUpQuestList)>>::put(w, m_vecGiveUpQuestList);
        Codec<std::remove_cvref_t<decltype(m_vecPet)>>::put(w, m_vecPet);
        Codec<std::remove_cvref_t<decltype(m_mapCharGameCount)>>::put(w, m_mapCharGameCount);
        Codec<std::remove_cvref_t<decltype(m_kHenirRewardCnt)>>::put(w, m_kHenirRewardCnt);
        Codec<std::remove_cvref_t<decltype(m_kGamePlayStatus)>>::put(w, m_kGamePlayStatus);
        Codec<std::remove_cvref_t<decltype(m_vecRecordBuffInfo)>>::put(w, m_vecRecordBuffInfo);
        Codec<std::remove_cvref_t<decltype(m_vecRidingPetList)>>::put(w, m_vecRidingPetList);
        Codec<std::remove_cvref_t<decltype(m_cUnitRelationshipType)>>::put(w, m_cUnitRelationshipType);
    }

    static KDBE_UPDATE_UNIT_INFO_REQ read_from(Reader& r) {
        KDBE_UPDATE_UNIT_INFO_REQ v;
        v.m_bFinal = Codec<std::remove_cvref_t<decltype(v.m_bFinal)>>::get(r);
        v.m_iEventID = Codec<std::remove_cvref_t<decltype(v.m_iEventID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iEXP = Codec<std::remove_cvref_t<decltype(v.m_iEXP)>>::get(r);
        v.m_iLevel = Codec<std::remove_cvref_t<decltype(v.m_iLevel)>>::get(r);
        v.m_iED = Codec<std::remove_cvref_t<decltype(v.m_iED)>>::get(r);
        v.m_iRealDataED = Codec<std::remove_cvref_t<decltype(v.m_iRealDataED)>>::get(r);
        v.m_iOfficialMatchCnt = Codec<std::remove_cvref_t<decltype(v.m_iOfficialMatchCnt)>>::get(r);
        v.m_iRating = Codec<std::remove_cvref_t<decltype(v.m_iRating)>>::get(r);
        v.m_iMaxRating = Codec<std::remove_cvref_t<decltype(v.m_iMaxRating)>>::get(r);
        v.m_iRPoint = Codec<std::remove_cvref_t<decltype(v.m_iRPoint)>>::get(r);
        v.m_iAPoint = Codec<std::remove_cvref_t<decltype(v.m_iAPoint)>>::get(r);
        v.m_bIsWinBeforeMatch = Codec<std::remove_cvref_t<decltype(v.m_bIsWinBeforeMatch)>>::get(r);
        v.m_cEmblemEnum = Codec<std::remove_cvref_t<decltype(v.m_cEmblemEnum)>>::get(r);
        v.m_fKFactor = Codec<std::remove_cvref_t<decltype(v.m_fKFactor)>>::get(r);
        v.m_iWin = Codec<std::remove_cvref_t<decltype(v.m_iWin)>>::get(r);
        v.m_iLose = Codec<std::remove_cvref_t<decltype(v.m_iLose)>>::get(r);
        v.m_kLastPos = Codec<std::remove_cvref_t<decltype(v.m_kLastPos)>>::get(r);
        v.m_mapDungeonClear = Codec<std::remove_cvref_t<decltype(v.m_mapDungeonClear)>>::get(r);
        v.m_mapTCClear = Codec<std::remove_cvref_t<decltype(v.m_mapTCClear)>>::get(r);
        v.m_mapDungeonPlay = Codec<std::remove_cvref_t<decltype(v.m_mapDungeonPlay)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_kItemEnduranceUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemEnduranceUpdate)>>::get(r);
        v.m_kItemPositionUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemPositionUpdate)>>::get(r);
        v.m_vecQuestData = Codec<std::remove_cvref_t<decltype(v.m_vecQuestData)>>::get(r);
        v.m_vecMissionData = Codec<std::remove_cvref_t<decltype(v.m_vecMissionData)>>::get(r);
        v.m_iInitTitleID = Codec<std::remove_cvref_t<decltype(v.m_iInitTitleID)>>::get(r);
        v.m_iEquippedTitleID = Codec<std::remove_cvref_t<decltype(v.m_iEquippedTitleID)>>::get(r);
        v.m_mapSkillSlotVector = Codec<std::remove_cvref_t<decltype(v.m_mapSkillSlotVector)>>::get(r);
        v.m_wstrSkillSlotBEndDate = Codec<std::remove_cvref_t<decltype(v.m_wstrSkillSlotBEndDate)>>::get(r);
        v.m_kDenyOptions = Codec<std::remove_cvref_t<decltype(v.m_kDenyOptions)>>::get(r);
        v.m_iSpirit = Codec<std::remove_cvref_t<decltype(v.m_iSpirit)>>::get(r);
        v.m_bIsSpiritUpdated = Codec<std::remove_cvref_t<decltype(v.m_bIsSpiritUpdated)>>::get(r);
        v.m_iNumResurrectionStone = Codec<std::remove_cvref_t<decltype(v.m_iNumResurrectionStone)>>::get(r);
        v.m_iNumAutoPaymentResStone = Codec<std::remove_cvref_t<decltype(v.m_iNumAutoPaymentResStone)>>::get(r);
        v.m_mapWishList = Codec<std::remove_cvref_t<decltype(v.m_mapWishList)>>::get(r);
        v.m_vecHenirRanking = Codec<std::remove_cvref_t<decltype(v.m_vecHenirRanking)>>::get(r);
        v.m_iGuildUID = Codec<std::remove_cvref_t<decltype(v.m_iGuildUID)>>::get(r);
        v.m_iHonorPoint = Codec<std::remove_cvref_t<decltype(v.m_iHonorPoint)>>::get(r);
        v.m_vecUpdateEventTime = Codec<std::remove_cvref_t<decltype(v.m_vecUpdateEventTime)>>::get(r);
        v.m_vecGiveUpQuestList = Codec<std::remove_cvref_t<decltype(v.m_vecGiveUpQuestList)>>::get(r);
        v.m_vecPet = Codec<std::remove_cvref_t<decltype(v.m_vecPet)>>::get(r);
        v.m_mapCharGameCount = Codec<std::remove_cvref_t<decltype(v.m_mapCharGameCount)>>::get(r);
        v.m_kHenirRewardCnt = Codec<std::remove_cvref_t<decltype(v.m_kHenirRewardCnt)>>::get(r);
        v.m_kGamePlayStatus = Codec<std::remove_cvref_t<decltype(v.m_kGamePlayStatus)>>::get(r);
        v.m_vecRecordBuffInfo = Codec<std::remove_cvref_t<decltype(v.m_vecRecordBuffInfo)>>::get(r);
        v.m_vecRidingPetList = Codec<std::remove_cvref_t<decltype(v.m_vecRidingPetList)>>::get(r);
        v.m_cUnitRelationshipType = Codec<std::remove_cvref_t<decltype(v.m_cUnitRelationshipType)>>::get(r);
        return v;
    }
};

struct KTCInfo {
    std::int32_t   m_iTCID{};  // int
    std::int32_t   m_iDungeonID{};  // int
    std::int64_t   m_RoomUID{};  // UidType
    char           m_cRoomType{};
    std::u16string m_wstrUDPRelayIP{};  // std::wstring
    std::uint16_t  m_usUDPRelayPort{};  // USHORT
    float          m_fPlayTime{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iTCID)>>::put(w, m_iTCID);
        Codec<std::remove_cvref_t<decltype(m_iDungeonID)>>::put(w, m_iDungeonID);
        Codec<std::remove_cvref_t<decltype(m_RoomUID)>>::put(w, m_RoomUID);
        Codec<std::remove_cvref_t<decltype(m_cRoomType)>>::put(w, m_cRoomType);
        Codec<std::remove_cvref_t<decltype(m_wstrUDPRelayIP)>>::put(w, m_wstrUDPRelayIP);
        Codec<std::remove_cvref_t<decltype(m_usUDPRelayPort)>>::put(w, m_usUDPRelayPort);
        Codec<std::remove_cvref_t<decltype(m_fPlayTime)>>::put(w, m_fPlayTime);
    }

    static KTCInfo read_from(Reader& r) {
        KTCInfo v;
        v.m_iTCID = Codec<std::remove_cvref_t<decltype(v.m_iTCID)>>::get(r);
        v.m_iDungeonID = Codec<std::remove_cvref_t<decltype(v.m_iDungeonID)>>::get(r);
        v.m_RoomUID = Codec<std::remove_cvref_t<decltype(v.m_RoomUID)>>::get(r);
        v.m_cRoomType = Codec<std::remove_cvref_t<decltype(v.m_cRoomType)>>::get(r);
        v.m_wstrUDPRelayIP = Codec<std::remove_cvref_t<decltype(v.m_wstrUDPRelayIP)>>::get(r);
        v.m_usUDPRelayPort = Codec<std::remove_cvref_t<decltype(v.m_usUDPRelayPort)>>::get(r);
        v.m_fPlayTime = Codec<std::remove_cvref_t<decltype(v.m_fPlayTime)>>::get(r);
        return v;
    }
};

struct KTempInventoryItemInfo {
    std::int32_t m_iTempItemUID{};  // int
    std::int32_t m_iItemID{};  // int
    std::int32_t m_iQuantity{};  // int
    std::uint8_t m_ucSealData{};  // unsigned char
    char         m_cEnchantLevel{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iTempItemUID)>>::put(w, m_iTempItemUID);
        Codec<std::remove_cvref_t<decltype(m_iItemID)>>::put(w, m_iItemID);
        Codec<std::remove_cvref_t<decltype(m_iQuantity)>>::put(w, m_iQuantity);
        Codec<std::remove_cvref_t<decltype(m_ucSealData)>>::put(w, m_ucSealData);
        Codec<std::remove_cvref_t<decltype(m_cEnchantLevel)>>::put(w, m_cEnchantLevel);
    }

    static KTempInventoryItemInfo read_from(Reader& r) {
        KTempInventoryItemInfo v;
        v.m_iTempItemUID = Codec<std::remove_cvref_t<decltype(v.m_iTempItemUID)>>::get(r);
        v.m_iItemID = Codec<std::remove_cvref_t<decltype(v.m_iItemID)>>::get(r);
        v.m_iQuantity = Codec<std::remove_cvref_t<decltype(v.m_iQuantity)>>::get(r);
        v.m_ucSealData = Codec<std::remove_cvref_t<decltype(v.m_ucSealData)>>::get(r);
        v.m_cEnchantLevel = Codec<std::remove_cvref_t<decltype(v.m_cEnchantLevel)>>::get(r);
        return v;
    }
};

struct KEGS_GET_ITEM_TEMP_INVENTORY_NOT {
    KTempInventoryItemInfo m_kTempInvenItem{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kTempInvenItem)>>::put(w, m_kTempInvenItem);
    }

    static KEGS_GET_ITEM_TEMP_INVENTORY_NOT read_from(Reader& r) {
        KEGS_GET_ITEM_TEMP_INVENTORY_NOT v;
        v.m_kTempInvenItem = Codec<std::remove_cvref_t<decltype(v.m_kTempInvenItem)>>::get(r);
        return v;
    }
};

struct KTileInfo {
    std::int32_t m_NumX{};  // int
    std::int32_t m_NumY{};  // int
    std::int32_t m_NumZ{};  // int
    std::int16_t m_SizeX{};  // short
    std::int16_t m_SizeY{};  // short
    std::int16_t m_SizeZ{};  // short

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_NumX)>>::put(w, m_NumX);
        Codec<std::remove_cvref_t<decltype(m_NumY)>>::put(w, m_NumY);
        Codec<std::remove_cvref_t<decltype(m_NumZ)>>::put(w, m_NumZ);
        Codec<std::remove_cvref_t<decltype(m_SizeX)>>::put(w, m_SizeX);
        Codec<std::remove_cvref_t<decltype(m_SizeY)>>::put(w, m_SizeY);
        Codec<std::remove_cvref_t<decltype(m_SizeZ)>>::put(w, m_SizeZ);
    }

    static KTileInfo read_from(Reader& r) {
        KTileInfo v;
        v.m_NumX = Codec<std::remove_cvref_t<decltype(v.m_NumX)>>::get(r);
        v.m_NumY = Codec<std::remove_cvref_t<decltype(v.m_NumY)>>::get(r);
        v.m_NumZ = Codec<std::remove_cvref_t<decltype(v.m_NumZ)>>::get(r);
        v.m_SizeX = Codec<std::remove_cvref_t<decltype(v.m_SizeX)>>::get(r);
        v.m_SizeY = Codec<std::remove_cvref_t<decltype(v.m_SizeY)>>::get(r);
        v.m_SizeZ = Codec<std::remove_cvref_t<decltype(v.m_SizeZ)>>::get(r);
        return v;
    }
};

struct KTimeDropInfo {
    std::int32_t   m_iTimeDropID{};  // int
    std::int32_t   m_iDropItemID{};  // int
    std::u16string m_wstrDropTime{};  // std::wstring
    std::u16string m_wstrServerIP{};  // std::wstring
    bool           m_bNoticeMsg{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iTimeDropID)>>::put(w, m_iTimeDropID);
        Codec<std::remove_cvref_t<decltype(m_iDropItemID)>>::put(w, m_iDropItemID);
        Codec<std::remove_cvref_t<decltype(m_wstrDropTime)>>::put(w, m_wstrDropTime);
        Codec<std::remove_cvref_t<decltype(m_wstrServerIP)>>::put(w, m_wstrServerIP);
        Codec<std::remove_cvref_t<decltype(m_bNoticeMsg)>>::put(w, m_bNoticeMsg);
    }

    static KTimeDropInfo read_from(Reader& r) {
        KTimeDropInfo v;
        v.m_iTimeDropID = Codec<std::remove_cvref_t<decltype(v.m_iTimeDropID)>>::get(r);
        v.m_iDropItemID = Codec<std::remove_cvref_t<decltype(v.m_iDropItemID)>>::get(r);
        v.m_wstrDropTime = Codec<std::remove_cvref_t<decltype(v.m_wstrDropTime)>>::get(r);
        v.m_wstrServerIP = Codec<std::remove_cvref_t<decltype(v.m_wstrServerIP)>>::get(r);
        v.m_bNoticeMsg = Codec<std::remove_cvref_t<decltype(v.m_bNoticeMsg)>>::get(r);
        return v;
    }
};

struct KDBE_GET_TIME_DROP_INFO_ACK {
    std::vector<KTimeDropInfo> m_vecTimeDropInfo{};  // std::vector< KTimeDropInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecTimeDropInfo)>>::put(w, m_vecTimeDropInfo);
    }

    static KDBE_GET_TIME_DROP_INFO_ACK read_from(Reader& r) {
        KDBE_GET_TIME_DROP_INFO_ACK v;
        v.m_vecTimeDropInfo = Codec<std::remove_cvref_t<decltype(v.m_vecTimeDropInfo)>>::get(r);
        return v;
    }
};

struct KTimeEnchantEventInfo {
    std::int32_t m_iEventUID{};  // int
    std::int64_t m_tBeginDate{};  // __time64_t
    std::int64_t m_tEndDate{};  // __time64_t
    char         m_cEnchantLevel{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iEventUID)>>::put(w, m_iEventUID);
        Codec<std::remove_cvref_t<decltype(m_tBeginDate)>>::put(w, m_tBeginDate);
        Codec<std::remove_cvref_t<decltype(m_tEndDate)>>::put(w, m_tEndDate);
        Codec<std::remove_cvref_t<decltype(m_cEnchantLevel)>>::put(w, m_cEnchantLevel);
    }

    static KTimeEnchantEventInfo read_from(Reader& r) {
        KTimeEnchantEventInfo v;
        v.m_iEventUID = Codec<std::remove_cvref_t<decltype(v.m_iEventUID)>>::get(r);
        v.m_tBeginDate = Codec<std::remove_cvref_t<decltype(v.m_tBeginDate)>>::get(r);
        v.m_tEndDate = Codec<std::remove_cvref_t<decltype(v.m_tEndDate)>>::get(r);
        v.m_cEnchantLevel = Codec<std::remove_cvref_t<decltype(v.m_cEnchantLevel)>>::get(r);
        return v;
    }
};

struct KDBE_TIME_ENCHANT_EVENT_INFO_ACK {
    std::vector<KTimeEnchantEventInfo> m_vecTimeEnchantEventInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecTimeEnchantEventInfo)>>::put(w, m_vecTimeEnchantEventInfo);
    }

    static KDBE_TIME_ENCHANT_EVENT_INFO_ACK read_from(Reader& r) {
        KDBE_TIME_ENCHANT_EVENT_INFO_ACK v;
        v.m_vecTimeEnchantEventInfo = Codec<std::remove_cvref_t<decltype(v.m_vecTimeEnchantEventInfo)>>::get(r);
        return v;
    }
};

struct KDBE_TIME_ENCHANT_EVENT_INFO_NOT {
    std::int32_t          m_iTimeEnchantEventUID{};  // int
    KTimeEnchantEventInfo m_kTimeEnchantEventInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iTimeEnchantEventUID)>>::put(w, m_iTimeEnchantEventUID);
        Codec<std::remove_cvref_t<decltype(m_kTimeEnchantEventInfo)>>::put(w, m_kTimeEnchantEventInfo);
    }

    static KDBE_TIME_ENCHANT_EVENT_INFO_NOT read_from(Reader& r) {
        KDBE_TIME_ENCHANT_EVENT_INFO_NOT v;
        v.m_iTimeEnchantEventUID = Codec<std::remove_cvref_t<decltype(v.m_iTimeEnchantEventUID)>>::get(r);
        v.m_kTimeEnchantEventInfo = Codec<std::remove_cvref_t<decltype(v.m_kTimeEnchantEventInfo)>>::get(r);
        return v;
    }
};

struct KDBE_TIME_ENCHANT_EVENT_INFO_UPDATE_REQ {
    std::int64_t          m_iGSUID{};  // UidType
    std::int64_t          m_iUnitUID{};  // UidType
    std::int32_t          m_iTimeEnchantEventUID{};  // int
    KTimeEnchantEventInfo m_kTimeEnchantEventInfo{};
    std::int32_t          m_iAfterWorkStorageKey{};  // int
    std::uint16_t         m_usEventID{};  // unsigned short

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iGSUID)>>::put(w, m_iGSUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iTimeEnchantEventUID)>>::put(w, m_iTimeEnchantEventUID);
        Codec<std::remove_cvref_t<decltype(m_kTimeEnchantEventInfo)>>::put(w, m_kTimeEnchantEventInfo);
        Codec<std::remove_cvref_t<decltype(m_iAfterWorkStorageKey)>>::put(w, m_iAfterWorkStorageKey);
        Codec<std::remove_cvref_t<decltype(m_usEventID)>>::put(w, m_usEventID);
    }

    static KDBE_TIME_ENCHANT_EVENT_INFO_UPDATE_REQ read_from(Reader& r) {
        KDBE_TIME_ENCHANT_EVENT_INFO_UPDATE_REQ v;
        v.m_iGSUID = Codec<std::remove_cvref_t<decltype(v.m_iGSUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iTimeEnchantEventUID = Codec<std::remove_cvref_t<decltype(v.m_iTimeEnchantEventUID)>>::get(r);
        v.m_kTimeEnchantEventInfo = Codec<std::remove_cvref_t<decltype(v.m_kTimeEnchantEventInfo)>>::get(r);
        v.m_iAfterWorkStorageKey = Codec<std::remove_cvref_t<decltype(v.m_iAfterWorkStorageKey)>>::get(r);
        v.m_usEventID = Codec<std::remove_cvref_t<decltype(v.m_usEventID)>>::get(r);
        return v;
    }
};

struct KTimeEnchantEventResult {
    std::int32_t m_iAfterWorkStorageKey{};  // int
    char         m_cTimeEnchantEventLevel{};
    bool         m_bNotifyGetItem{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iAfterWorkStorageKey)>>::put(w, m_iAfterWorkStorageKey);
        Codec<std::remove_cvref_t<decltype(m_cTimeEnchantEventLevel)>>::put(w, m_cTimeEnchantEventLevel);
        Codec<std::remove_cvref_t<decltype(m_bNotifyGetItem)>>::put(w, m_bNotifyGetItem);
    }

    static KTimeEnchantEventResult read_from(Reader& r) {
        KTimeEnchantEventResult v;
        v.m_iAfterWorkStorageKey = Codec<std::remove_cvref_t<decltype(v.m_iAfterWorkStorageKey)>>::get(r);
        v.m_cTimeEnchantEventLevel = Codec<std::remove_cvref_t<decltype(v.m_cTimeEnchantEventLevel)>>::get(r);
        v.m_bNotifyGetItem = Codec<std::remove_cvref_t<decltype(v.m_bNotifyGetItem)>>::get(r);
        return v;
    }
};

struct KTimeEventSimpleInfo {
    std::int32_t   m_iEventUID{};  // int
    std::int32_t   m_iEventID{};  // int
    std::u16string m_wstrEventName{};  // std::wstring
    float          m_fEventTime{};
    std::u16string m_wstrAdvertisementURL{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iEventUID)>>::put(w, m_iEventUID);
        Codec<std::remove_cvref_t<decltype(m_iEventID)>>::put(w, m_iEventID);
        Codec<std::remove_cvref_t<decltype(m_wstrEventName)>>::put(w, m_wstrEventName);
        Codec<std::remove_cvref_t<decltype(m_fEventTime)>>::put(w, m_fEventTime);
        Codec<std::remove_cvref_t<decltype(m_wstrAdvertisementURL)>>::put(w, m_wstrAdvertisementURL);
    }

    static KTimeEventSimpleInfo read_from(Reader& r) {
        KTimeEventSimpleInfo v;
        v.m_iEventUID = Codec<std::remove_cvref_t<decltype(v.m_iEventUID)>>::get(r);
        v.m_iEventID = Codec<std::remove_cvref_t<decltype(v.m_iEventID)>>::get(r);
        v.m_wstrEventName = Codec<std::remove_cvref_t<decltype(v.m_wstrEventName)>>::get(r);
        v.m_fEventTime = Codec<std::remove_cvref_t<decltype(v.m_fEventTime)>>::get(r);
        v.m_wstrAdvertisementURL = Codec<std::remove_cvref_t<decltype(v.m_wstrAdvertisementURL)>>::get(r);
        return v;
    }
};

struct KEGS_TIME_EVENT_INFO_NOT {
    char                              m_cNotifyType{};
    std::vector<KTimeEventSimpleInfo> m_vecTimeEvent{};  // std::vector< KTimeEventSimpleInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cNotifyType)>>::put(w, m_cNotifyType);
        Codec<std::remove_cvref_t<decltype(m_vecTimeEvent)>>::put(w, m_vecTimeEvent);
    }

    static KEGS_TIME_EVENT_INFO_NOT read_from(Reader& r) {
        KEGS_TIME_EVENT_INFO_NOT v;
        v.m_cNotifyType = Codec<std::remove_cvref_t<decltype(v.m_cNotifyType)>>::get(r);
        v.m_vecTimeEvent = Codec<std::remove_cvref_t<decltype(v.m_vecTimeEvent)>>::get(r);
        return v;
    }
};

struct KTimeOpenRandomItemEventDBUPdate {
    std::int32_t m_iID{};  // int
    std::int64_t m_tUpdateTime{};  // __int64
    std::int64_t m_iGetUserUID{};  // UidType
    std::int64_t m_iGetUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iID)>>::put(w, m_iID);
        Codec<std::remove_cvref_t<decltype(m_tUpdateTime)>>::put(w, m_tUpdateTime);
        Codec<std::remove_cvref_t<decltype(m_iGetUserUID)>>::put(w, m_iGetUserUID);
        Codec<std::remove_cvref_t<decltype(m_iGetUnitUID)>>::put(w, m_iGetUnitUID);
    }

    static KTimeOpenRandomItemEventDBUPdate read_from(Reader& r) {
        KTimeOpenRandomItemEventDBUPdate v;
        v.m_iID = Codec<std::remove_cvref_t<decltype(v.m_iID)>>::get(r);
        v.m_tUpdateTime = Codec<std::remove_cvref_t<decltype(v.m_tUpdateTime)>>::get(r);
        v.m_iGetUserUID = Codec<std::remove_cvref_t<decltype(v.m_iGetUserUID)>>::get(r);
        v.m_iGetUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iGetUnitUID)>>::get(r);
        return v;
    }
};

struct KDBE_UPDATE_TIME_RANDOM_ITEM_EVENT_RESULT_NOT {
    KTimeOpenRandomItemEventDBUPdate m_kDBUpdateInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kDBUpdateInfo)>>::put(w, m_kDBUpdateInfo);
    }

    static KDBE_UPDATE_TIME_RANDOM_ITEM_EVENT_RESULT_NOT read_from(Reader& r) {
        KDBE_UPDATE_TIME_RANDOM_ITEM_EVENT_RESULT_NOT v;
        v.m_kDBUpdateInfo = Codec<std::remove_cvref_t<decltype(v.m_kDBUpdateInfo)>>::get(r);
        return v;
    }
};

struct KTimeOpenRandomItemEventInfo {
    std::int32_t   m_iID{};  // int
    std::int32_t   m_iItemID{};  // int
    std::u16string m_wstrDropTime{};  // std::wstring
    std::u16string m_wstrDropEndTime{};  // std::wstring
    std::int32_t   m_iOpenItemID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iID)>>::put(w, m_iID);
        Codec<std::remove_cvref_t<decltype(m_iItemID)>>::put(w, m_iItemID);
        Codec<std::remove_cvref_t<decltype(m_wstrDropTime)>>::put(w, m_wstrDropTime);
        Codec<std::remove_cvref_t<decltype(m_wstrDropEndTime)>>::put(w, m_wstrDropEndTime);
        Codec<std::remove_cvref_t<decltype(m_iOpenItemID)>>::put(w, m_iOpenItemID);
    }

    static KTimeOpenRandomItemEventInfo read_from(Reader& r) {
        KTimeOpenRandomItemEventInfo v;
        v.m_iID = Codec<std::remove_cvref_t<decltype(v.m_iID)>>::get(r);
        v.m_iItemID = Codec<std::remove_cvref_t<decltype(v.m_iItemID)>>::get(r);
        v.m_wstrDropTime = Codec<std::remove_cvref_t<decltype(v.m_wstrDropTime)>>::get(r);
        v.m_wstrDropEndTime = Codec<std::remove_cvref_t<decltype(v.m_wstrDropEndTime)>>::get(r);
        v.m_iOpenItemID = Codec<std::remove_cvref_t<decltype(v.m_iOpenItemID)>>::get(r);
        return v;
    }
};

struct KDBE_GET_TIME_RANDOM_ITEM_EVENT_INFO_ACK {
    std::vector<KTimeOpenRandomItemEventInfo> m_vecTimeOpenRandomItemInfo{};  // std::vector< KTimeOpenRandomItemEventInfo >
    std::set<std::int64_t>                    m_setGetRewardUserUIDList{};  // std::set< UidType >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecTimeOpenRandomItemInfo)>>::put(w, m_vecTimeOpenRandomItemInfo);
        Codec<std::remove_cvref_t<decltype(m_setGetRewardUserUIDList)>>::put(w, m_setGetRewardUserUIDList);
    }

    static KDBE_GET_TIME_RANDOM_ITEM_EVENT_INFO_ACK read_from(Reader& r) {
        KDBE_GET_TIME_RANDOM_ITEM_EVENT_INFO_ACK v;
        v.m_vecTimeOpenRandomItemInfo = Codec<std::remove_cvref_t<decltype(v.m_vecTimeOpenRandomItemInfo)>>::get(r);
        v.m_setGetRewardUserUIDList = Codec<std::remove_cvref_t<decltype(v.m_setGetRewardUserUIDList)>>::get(r);
        return v;
    }
};

struct KTitleInfo {
    std::int32_t   m_iTitleID{};  // int
    std::u16string m_wstrEndDate{};  // std::wstring
    std::int64_t   m_kCTime{};  // __int64
    bool           m_bInfinity{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iTitleID)>>::put(w, m_iTitleID);
        Codec<std::remove_cvref_t<decltype(m_wstrEndDate)>>::put(w, m_wstrEndDate);
        Codec<std::remove_cvref_t<decltype(m_kCTime)>>::put(w, m_kCTime);
        Codec<std::remove_cvref_t<decltype(m_bInfinity)>>::put(w, m_bInfinity);
    }

    static KTitleInfo read_from(Reader& r) {
        KTitleInfo v;
        v.m_iTitleID = Codec<std::remove_cvref_t<decltype(v.m_iTitleID)>>::get(r);
        v.m_wstrEndDate = Codec<std::remove_cvref_t<decltype(v.m_wstrEndDate)>>::get(r);
        v.m_kCTime = Codec<std::remove_cvref_t<decltype(v.m_kCTime)>>::get(r);
        v.m_bInfinity = Codec<std::remove_cvref_t<decltype(v.m_bInfinity)>>::get(r);
        return v;
    }
};

struct KEGS_REWARD_TITLE_NOT {
    KTitleInfo m_kTitle{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kTitle)>>::put(w, m_kTitle);
    }

    static KEGS_REWARD_TITLE_NOT read_from(Reader& r) {
        KEGS_REWARD_TITLE_NOT v;
        v.m_kTitle = Codec<std::remove_cvref_t<decltype(v.m_kTitle)>>::get(r);
        return v;
    }
};

struct KEGS_SELECT_UNIT_3_NOT {
    std::int32_t                                                       m_iOK{};  // int
    std::vector<KMissionInstance>                                      m_vecMission{};  // std::vector< KMissionInstance >
    std::vector<KTitleInfo>                                            m_vecTitle{};  // std::vector< KTitleInfo >
    std::int64_t                                                       m_iSummonedPetUID{};  // UidType
    std::vector<KPetInfo>                                              m_vecPetList{};  // std::vector< KPetInfo >
    std::vector<std::int64_t>                                          m_vecCanEvolutionPetList{};  // std::vector< UidType >
    std::vector<std::int64_t>                                          m_vecNeverSummonPetList{};  // std::vector< UidType >
    std::map<std::int64_t, std::map<std::int64_t, KInventoryItemInfo>> m_mapPetItem{};  // std::map< UidType, std::map< UidType, KInventoryItemInfo > >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_vecMission)>>::put(w, m_vecMission);
        Codec<std::remove_cvref_t<decltype(m_vecTitle)>>::put(w, m_vecTitle);
        Codec<std::remove_cvref_t<decltype(m_iSummonedPetUID)>>::put(w, m_iSummonedPetUID);
        Codec<std::remove_cvref_t<decltype(m_vecPetList)>>::put(w, m_vecPetList);
        Codec<std::remove_cvref_t<decltype(m_vecCanEvolutionPetList)>>::put(w, m_vecCanEvolutionPetList);
        Codec<std::remove_cvref_t<decltype(m_vecNeverSummonPetList)>>::put(w, m_vecNeverSummonPetList);
        Codec<std::remove_cvref_t<decltype(m_mapPetItem)>>::put(w, m_mapPetItem);
    }

    static KEGS_SELECT_UNIT_3_NOT read_from(Reader& r) {
        KEGS_SELECT_UNIT_3_NOT v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_vecMission = Codec<std::remove_cvref_t<decltype(v.m_vecMission)>>::get(r);
        v.m_vecTitle = Codec<std::remove_cvref_t<decltype(v.m_vecTitle)>>::get(r);
        v.m_iSummonedPetUID = Codec<std::remove_cvref_t<decltype(v.m_iSummonedPetUID)>>::get(r);
        v.m_vecPetList = Codec<std::remove_cvref_t<decltype(v.m_vecPetList)>>::get(r);
        v.m_vecCanEvolutionPetList = Codec<std::remove_cvref_t<decltype(v.m_vecCanEvolutionPetList)>>::get(r);
        v.m_vecNeverSummonPetList = Codec<std::remove_cvref_t<decltype(v.m_vecNeverSummonPetList)>>::get(r);
        v.m_mapPetItem = Codec<std::remove_cvref_t<decltype(v.m_mapPetItem)>>::get(r);
        return v;
    }
};

struct KTitleReward {
    std::int32_t m_iTitleID{};  // int
    std::int16_t m_sPeriod{};  // short

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iTitleID)>>::put(w, m_iTitleID);
        Codec<std::remove_cvref_t<decltype(m_sPeriod)>>::put(w, m_sPeriod);
    }

    static KTitleReward read_from(Reader& r) {
        KTitleReward v;
        v.m_iTitleID = Codec<std::remove_cvref_t<decltype(v.m_iTitleID)>>::get(r);
        v.m_sPeriod = Codec<std::remove_cvref_t<decltype(v.m_sPeriod)>>::get(r);
        return v;
    }
};

struct KRankingTitleReward {
    std::int64_t              m_iUnitUID{};  // UidType
    std::vector<KTitleReward> m_vecReward{};  // std::vector< KTitleReward >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_vecReward)>>::put(w, m_vecReward);
    }

    static KRankingTitleReward read_from(Reader& r) {
        KRankingTitleReward v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_vecReward = Codec<std::remove_cvref_t<decltype(v.m_vecReward)>>::get(r);
        return v;
    }
};

struct KELG_RANKING_TITLE_REWARD_NOT {
    std::map<std::int64_t, KRankingTitleReward> m_mapTitleReward{};  // std::map< UidType, KRankingTitleReward >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_mapTitleReward)>>::put(w, m_mapTitleReward);
    }

    static KELG_RANKING_TITLE_REWARD_NOT read_from(Reader& r) {
        KELG_RANKING_TITLE_REWARD_NOT v;
        v.m_mapTitleReward = Codec<std::remove_cvref_t<decltype(v.m_mapTitleReward)>>::get(r);
        return v;
    }
};

struct KToolServerList {
    std::int32_t   m_iServerUID{};  // int
    std::int32_t   m_iServerSetID{};  // int
    std::int32_t   m_iServerType{};  // int
    std::int32_t   m_iChannelID{};  // int
    std::u16string m_wstrChannelName{};  // std::wstring
    std::u16string m_wstrIP{};  // std::wstring
    std::int32_t   m_iPort{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iServerUID)>>::put(w, m_iServerUID);
        Codec<std::remove_cvref_t<decltype(m_iServerSetID)>>::put(w, m_iServerSetID);
        Codec<std::remove_cvref_t<decltype(m_iServerType)>>::put(w, m_iServerType);
        Codec<std::remove_cvref_t<decltype(m_iChannelID)>>::put(w, m_iChannelID);
        Codec<std::remove_cvref_t<decltype(m_wstrChannelName)>>::put(w, m_wstrChannelName);
        Codec<std::remove_cvref_t<decltype(m_wstrIP)>>::put(w, m_wstrIP);
        Codec<std::remove_cvref_t<decltype(m_iPort)>>::put(w, m_iPort);
    }

    static KToolServerList read_from(Reader& r) {
        KToolServerList v;
        v.m_iServerUID = Codec<std::remove_cvref_t<decltype(v.m_iServerUID)>>::get(r);
        v.m_iServerSetID = Codec<std::remove_cvref_t<decltype(v.m_iServerSetID)>>::get(r);
        v.m_iServerType = Codec<std::remove_cvref_t<decltype(v.m_iServerType)>>::get(r);
        v.m_iChannelID = Codec<std::remove_cvref_t<decltype(v.m_iChannelID)>>::get(r);
        v.m_wstrChannelName = Codec<std::remove_cvref_t<decltype(v.m_wstrChannelName)>>::get(r);
        v.m_wstrIP = Codec<std::remove_cvref_t<decltype(v.m_wstrIP)>>::get(r);
        v.m_iPort = Codec<std::remove_cvref_t<decltype(v.m_iPort)>>::get(r);
        return v;
    }
};

struct KDBE_GET_GAME_SERVER_LIST_ACK {
    std::int32_t                 m_iServerCount{};  // int
    std::vector<KToolServerList> m_vecServerList{};  // std::vector< KToolServerList >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iServerCount)>>::put(w, m_iServerCount);
        Codec<std::remove_cvref_t<decltype(m_vecServerList)>>::put(w, m_vecServerList);
    }

    static KDBE_GET_GAME_SERVER_LIST_ACK read_from(Reader& r) {
        KDBE_GET_GAME_SERVER_LIST_ACK v;
        v.m_iServerCount = Codec<std::remove_cvref_t<decltype(v.m_iServerCount)>>::get(r);
        v.m_vecServerList = Codec<std::remove_cvref_t<decltype(v.m_vecServerList)>>::get(r);
        return v;
    }
};

struct KE_TOOL_SERVER_LIST_ACK {
    std::int32_t                 m_iServerCount{};  // int
    std::vector<KToolServerList> m_vecServerList{};  // std::vector< KToolServerList >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iServerCount)>>::put(w, m_iServerCount);
        Codec<std::remove_cvref_t<decltype(m_vecServerList)>>::put(w, m_vecServerList);
    }

    static KE_TOOL_SERVER_LIST_ACK read_from(Reader& r) {
        KE_TOOL_SERVER_LIST_ACK v;
        v.m_iServerCount = Codec<std::remove_cvref_t<decltype(v.m_iServerCount)>>::get(r);
        v.m_vecServerList = Codec<std::remove_cvref_t<decltype(v.m_vecServerList)>>::get(r);
        return v;
    }
};

struct KTradeBoardItemInfo {
    std::int64_t              m_iPersonalShopUID{};  // UidType
    std::int64_t              m_iHostUnitUID{};  // UidType
    std::u16string            m_wstrSellerNickName{};  // std::wstring
    KSellPersonalShopItemInfo m_kSellItemInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iPersonalShopUID)>>::put(w, m_iPersonalShopUID);
        Codec<std::remove_cvref_t<decltype(m_iHostUnitUID)>>::put(w, m_iHostUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrSellerNickName)>>::put(w, m_wstrSellerNickName);
        Codec<std::remove_cvref_t<decltype(m_kSellItemInfo)>>::put(w, m_kSellItemInfo);
    }

    static KTradeBoardItemInfo read_from(Reader& r) {
        KTradeBoardItemInfo v;
        v.m_iPersonalShopUID = Codec<std::remove_cvref_t<decltype(v.m_iPersonalShopUID)>>::get(r);
        v.m_iHostUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iHostUnitUID)>>::get(r);
        v.m_wstrSellerNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrSellerNickName)>>::get(r);
        v.m_kSellItemInfo = Codec<std::remove_cvref_t<decltype(v.m_kSellItemInfo)>>::get(r);
        return v;
    }
};

struct KEGS_SEARCH_TRADE_BOARD_ACK {
    std::int32_t                     m_iOK{};  // int
    std::vector<KTradeBoardItemInfo> m_vecItemInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_vecItemInfo)>>::put(w, m_vecItemInfo);
    }

    static KEGS_SEARCH_TRADE_BOARD_ACK read_from(Reader& r) {
        KEGS_SEARCH_TRADE_BOARD_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_vecItemInfo = Codec<std::remove_cvref_t<decltype(v.m_vecItemInfo)>>::get(r);
        return v;
    }
};

struct KTradeShareItemInfo {
    std::int64_t m_iItemUID{};  // UidType
    std::int64_t m_iFromUnitUID{};  // UidType
    std::int64_t m_iToUnitUID{};  // UidType
    char         m_cInventoryCategory{};
    std::int16_t m_sSlotID{};  // short
    std::uint8_t m_ucSealCnt{};  // u_char
    std::uint8_t m_ucDeleteReason{};  // u_char
    std::int32_t m_iItemID{};  // int
    std::int64_t m_iNewItemUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_iFromUnitUID)>>::put(w, m_iFromUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iToUnitUID)>>::put(w, m_iToUnitUID);
        Codec<std::remove_cvref_t<decltype(m_cInventoryCategory)>>::put(w, m_cInventoryCategory);
        Codec<std::remove_cvref_t<decltype(m_sSlotID)>>::put(w, m_sSlotID);
        Codec<std::remove_cvref_t<decltype(m_ucSealCnt)>>::put(w, m_ucSealCnt);
        Codec<std::remove_cvref_t<decltype(m_ucDeleteReason)>>::put(w, m_ucDeleteReason);
        Codec<std::remove_cvref_t<decltype(m_iItemID)>>::put(w, m_iItemID);
        Codec<std::remove_cvref_t<decltype(m_iNewItemUID)>>::put(w, m_iNewItemUID);
    }

    static KTradeShareItemInfo read_from(Reader& r) {
        KTradeShareItemInfo v;
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_iFromUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iFromUnitUID)>>::get(r);
        v.m_iToUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iToUnitUID)>>::get(r);
        v.m_cInventoryCategory = Codec<std::remove_cvref_t<decltype(v.m_cInventoryCategory)>>::get(r);
        v.m_sSlotID = Codec<std::remove_cvref_t<decltype(v.m_sSlotID)>>::get(r);
        v.m_ucSealCnt = Codec<std::remove_cvref_t<decltype(v.m_ucSealCnt)>>::get(r);
        v.m_ucDeleteReason = Codec<std::remove_cvref_t<decltype(v.m_ucDeleteReason)>>::get(r);
        v.m_iItemID = Codec<std::remove_cvref_t<decltype(v.m_iItemID)>>::get(r);
        v.m_iNewItemUID = Codec<std::remove_cvref_t<decltype(v.m_iNewItemUID)>>::get(r);
        return v;
    }
};

struct KDBE_UPDATE_SHARE_ITEM_ACK {
    std::int32_t                     m_iOK{};  // int
    bool                             m_bReload{};
    std::u16string                   m_wstrReloadNickname{};  // std::wstring
    std::vector<KTradeShareItemInfo> m_vecTradeShareItemResult{};  // std::vector< KTradeShareItemInfo >
    std::vector<KInventoryItemInfo>  m_vecInventorySlotItem{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_bReload)>>::put(w, m_bReload);
        Codec<std::remove_cvref_t<decltype(m_wstrReloadNickname)>>::put(w, m_wstrReloadNickname);
        Codec<std::remove_cvref_t<decltype(m_vecTradeShareItemResult)>>::put(w, m_vecTradeShareItemResult);
        Codec<std::remove_cvref_t<decltype(m_vecInventorySlotItem)>>::put(w, m_vecInventorySlotItem);
    }

    static KDBE_UPDATE_SHARE_ITEM_ACK read_from(Reader& r) {
        KDBE_UPDATE_SHARE_ITEM_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_bReload = Codec<std::remove_cvref_t<decltype(v.m_bReload)>>::get(r);
        v.m_wstrReloadNickname = Codec<std::remove_cvref_t<decltype(v.m_wstrReloadNickname)>>::get(r);
        v.m_vecTradeShareItemResult = Codec<std::remove_cvref_t<decltype(v.m_vecTradeShareItemResult)>>::get(r);
        v.m_vecInventorySlotItem = Codec<std::remove_cvref_t<decltype(v.m_vecInventorySlotItem)>>::get(r);
        return v;
    }
};

struct KTradeShareItemResult {
    std::int64_t   m_iBeforeItemUID{};  // UidType
    std::int64_t   m_iNewItemUID{};  // UidType
    std::u16string m_wstrRegDate{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iBeforeItemUID)>>::put(w, m_iBeforeItemUID);
        Codec<std::remove_cvref_t<decltype(m_iNewItemUID)>>::put(w, m_iNewItemUID);
        Codec<std::remove_cvref_t<decltype(m_wstrRegDate)>>::put(w, m_wstrRegDate);
    }

    static KTradeShareItemResult read_from(Reader& r) {
        KTradeShareItemResult v;
        v.m_iBeforeItemUID = Codec<std::remove_cvref_t<decltype(v.m_iBeforeItemUID)>>::get(r);
        v.m_iNewItemUID = Codec<std::remove_cvref_t<decltype(v.m_iNewItemUID)>>::get(r);
        v.m_wstrRegDate = Codec<std::remove_cvref_t<decltype(v.m_wstrRegDate)>>::get(r);
        return v;
    }
};

struct KEGS_UPDATE_ITEM_POSITION_NOT {
    std::vector<KTradeShareItemResult> m_vecShareItemResult{};  // std::vector< KTradeShareItemResult >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecShareItemResult)>>::put(w, m_vecShareItemResult);
    }

    static KEGS_UPDATE_ITEM_POSITION_NOT read_from(Reader& r) {
        KEGS_UPDATE_ITEM_POSITION_NOT v;
        v.m_vecShareItemResult = Codec<std::remove_cvref_t<decltype(v.m_vecShareItemResult)>>::get(r);
        return v;
    }
};

struct KTradeUserInfo {
    std::int64_t   m_iGSUID{};  // UidType
    std::int64_t   m_iUserUID{};  // UidType
    std::int64_t   m_iUnitUID{};  // UidType
    std::u16string m_wstrNickName{};  // std::wstring
    std::int32_t   m_iED{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iGSUID)>>::put(w, m_iGSUID);
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_iED)>>::put(w, m_iED);
    }

    static KTradeUserInfo read_from(Reader& r) {
        KTradeUserInfo v;
        v.m_iGSUID = Codec<std::remove_cvref_t<decltype(v.m_iGSUID)>>::get(r);
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_iED = Codec<std::remove_cvref_t<decltype(v.m_iED)>>::get(r);
        return v;
    }
};

struct KEGS_JOIN_TRADE_NOT {
    KTradeUserInfo m_kTradeUserInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kTradeUserInfo)>>::put(w, m_kTradeUserInfo);
    }

    static KEGS_JOIN_TRADE_NOT read_from(Reader& r) {
        KEGS_JOIN_TRADE_NOT v;
        v.m_kTradeUserInfo = Codec<std::remove_cvref_t<decltype(v.m_kTradeUserInfo)>>::get(r);
        return v;
    }
};

struct KEGS_TRADE_USER_INFO_NOT {
    std::vector<KTradeUserInfo> m_vecTradeUserInfo{};  // std::vector< KTradeUserInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecTradeUserInfo)>>::put(w, m_vecTradeUserInfo);
    }

    static KEGS_TRADE_USER_INFO_NOT read_from(Reader& r) {
        KEGS_TRADE_USER_INFO_NOT v;
        v.m_vecTradeUserInfo = Codec<std::remove_cvref_t<decltype(v.m_vecTradeUserInfo)>>::get(r);
        return v;
    }
};

struct KERM_JOIN_TRADE_ACK {
    std::int32_t                m_iOK{};  // int
    std::int64_t                m_iTradeUID{};  // UidType
    std::vector<KTradeUserInfo> m_vecTradeUserInfo{};  // std::vector< KTradeUserInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iTradeUID)>>::put(w, m_iTradeUID);
        Codec<std::remove_cvref_t<decltype(m_vecTradeUserInfo)>>::put(w, m_vecTradeUserInfo);
    }

    static KERM_JOIN_TRADE_ACK read_from(Reader& r) {
        KERM_JOIN_TRADE_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iTradeUID = Codec<std::remove_cvref_t<decltype(v.m_iTradeUID)>>::get(r);
        v.m_vecTradeUserInfo = Codec<std::remove_cvref_t<decltype(v.m_vecTradeUserInfo)>>::get(r);
        return v;
    }
};

struct KERM_JOIN_TRADE_REQ {
    KTradeUserInfo m_kMemberInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kMemberInfo)>>::put(w, m_kMemberInfo);
    }

    static KERM_JOIN_TRADE_REQ read_from(Reader& r) {
        KERM_JOIN_TRADE_REQ v;
        v.m_kMemberInfo = Codec<std::remove_cvref_t<decltype(v.m_kMemberInfo)>>::get(r);
        return v;
    }
};

struct KTutorialUnitInfo {
    std::int64_t   m_iUnitUID{};  // UidType
    std::uint8_t   m_ucLevel{};  // u_char
    std::u16string m_wstrNickName{};  // std::wstring
    bool           m_bIsOnline{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_ucLevel)>>::put(w, m_ucLevel);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_bIsOnline)>>::put(w, m_bIsOnline);
    }

    static KTutorialUnitInfo read_from(Reader& r) {
        KTutorialUnitInfo v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_ucLevel = Codec<std::remove_cvref_t<decltype(v.m_ucLevel)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_bIsOnline = Codec<std::remove_cvref_t<decltype(v.m_bIsOnline)>>::get(r);
        return v;
    }
};

struct KDBE_INSERT_TUTORIAL_ACK {
    std::int32_t      m_iOK{};  // int
    std::int64_t      m_iTeacherUID{};  // UidType
    KTutorialUnitInfo m_kStudentUnitInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iTeacherUID)>>::put(w, m_iTeacherUID);
        Codec<std::remove_cvref_t<decltype(m_kStudentUnitInfo)>>::put(w, m_kStudentUnitInfo);
    }

    static KDBE_INSERT_TUTORIAL_ACK read_from(Reader& r) {
        KDBE_INSERT_TUTORIAL_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iTeacherUID = Codec<std::remove_cvref_t<decltype(v.m_iTeacherUID)>>::get(r);
        v.m_kStudentUnitInfo = Codec<std::remove_cvref_t<decltype(v.m_kStudentUnitInfo)>>::get(r);
        return v;
    }
};

struct KDBE_INSERT_TUTORIAL_REQ {
    std::int64_t      m_iTeacherUID{};  // UidType
    KTutorialUnitInfo m_kStudentUnitInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iTeacherUID)>>::put(w, m_iTeacherUID);
        Codec<std::remove_cvref_t<decltype(m_kStudentUnitInfo)>>::put(w, m_kStudentUnitInfo);
    }

    static KDBE_INSERT_TUTORIAL_REQ read_from(Reader& r) {
        KDBE_INSERT_TUTORIAL_REQ v;
        v.m_iTeacherUID = Codec<std::remove_cvref_t<decltype(v.m_iTeacherUID)>>::get(r);
        v.m_kStudentUnitInfo = Codec<std::remove_cvref_t<decltype(v.m_kStudentUnitInfo)>>::get(r);
        return v;
    }
};

struct KEGS_REQUEST_TUTORIAL_REPLY_NOT {
    std::int64_t      m_iReceiverUnitUID{};  // UidType
    KTutorialUnitInfo m_kTutorialUnitInfo{};
    bool              m_bAccept{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iReceiverUnitUID)>>::put(w, m_iReceiverUnitUID);
        Codec<std::remove_cvref_t<decltype(m_kTutorialUnitInfo)>>::put(w, m_kTutorialUnitInfo);
        Codec<std::remove_cvref_t<decltype(m_bAccept)>>::put(w, m_bAccept);
    }

    static KEGS_REQUEST_TUTORIAL_REPLY_NOT read_from(Reader& r) {
        KEGS_REQUEST_TUTORIAL_REPLY_NOT v;
        v.m_iReceiverUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iReceiverUnitUID)>>::get(r);
        v.m_kTutorialUnitInfo = Codec<std::remove_cvref_t<decltype(v.m_kTutorialUnitInfo)>>::get(r);
        v.m_bAccept = Codec<std::remove_cvref_t<decltype(v.m_bAccept)>>::get(r);
        return v;
    }
};

struct KEGS_TUTORIAL_STUDENT_LIST_ACK {
    std::int32_t                   m_iOK{};  // int
    std::int32_t                   m_iTotalPage{};  // int
    std::int32_t                   m_iViewPage{};  // int
    std::vector<KTutorialUnitInfo> m_vecTutorialStudentList{};  // std::vector< KTutorialUnitInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iTotalPage)>>::put(w, m_iTotalPage);
        Codec<std::remove_cvref_t<decltype(m_iViewPage)>>::put(w, m_iViewPage);
        Codec<std::remove_cvref_t<decltype(m_vecTutorialStudentList)>>::put(w, m_vecTutorialStudentList);
    }

    static KEGS_TUTORIAL_STUDENT_LIST_ACK read_from(Reader& r) {
        KEGS_TUTORIAL_STUDENT_LIST_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iTotalPage = Codec<std::remove_cvref_t<decltype(v.m_iTotalPage)>>::get(r);
        v.m_iViewPage = Codec<std::remove_cvref_t<decltype(v.m_iViewPage)>>::get(r);
        v.m_vecTutorialStudentList = Codec<std::remove_cvref_t<decltype(v.m_vecTutorialStudentList)>>::get(r);
        return v;
    }
};

struct KEGS_TUTORIAL_UPDATE_UNIT_INFO_NOT {
    bool              m_bIsReply{};
    std::int64_t      m_iReceiverUID{};  // UidType
    KTutorialUnitInfo m_kTutorialUnitInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bIsReply)>>::put(w, m_bIsReply);
        Codec<std::remove_cvref_t<decltype(m_iReceiverUID)>>::put(w, m_iReceiverUID);
        Codec<std::remove_cvref_t<decltype(m_kTutorialUnitInfo)>>::put(w, m_kTutorialUnitInfo);
    }

    static KEGS_TUTORIAL_UPDATE_UNIT_INFO_NOT read_from(Reader& r) {
        KEGS_TUTORIAL_UPDATE_UNIT_INFO_NOT v;
        v.m_bIsReply = Codec<std::remove_cvref_t<decltype(v.m_bIsReply)>>::get(r);
        v.m_iReceiverUID = Codec<std::remove_cvref_t<decltype(v.m_iReceiverUID)>>::get(r);
        v.m_kTutorialUnitInfo = Codec<std::remove_cvref_t<decltype(v.m_kTutorialUnitInfo)>>::get(r);
        return v;
    }
};

struct KELG_REQUEST_TUTORIAL_NOT {
    std::int64_t      m_iReceiverUnitUID{};  // UidType
    KTutorialUnitInfo m_kTutorialUnitInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iReceiverUnitUID)>>::put(w, m_iReceiverUnitUID);
        Codec<std::remove_cvref_t<decltype(m_kTutorialUnitInfo)>>::put(w, m_kTutorialUnitInfo);
    }

    static KELG_REQUEST_TUTORIAL_NOT read_from(Reader& r) {
        KELG_REQUEST_TUTORIAL_NOT v;
        v.m_iReceiverUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iReceiverUnitUID)>>::get(r);
        v.m_kTutorialUnitInfo = Codec<std::remove_cvref_t<decltype(v.m_kTutorialUnitInfo)>>::get(r);
        return v;
    }
};

struct KTutorialDBUnitInfo {
    KTutorialUnitInfo m_kTutorialUnitInfo{};
    std::u16string    m_wstrLastDate{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kTutorialUnitInfo)>>::put(w, m_kTutorialUnitInfo);
        Codec<std::remove_cvref_t<decltype(m_wstrLastDate)>>::put(w, m_wstrLastDate);
    }

    static KTutorialDBUnitInfo read_from(Reader& r) {
        KTutorialDBUnitInfo v;
        v.m_kTutorialUnitInfo = Codec<std::remove_cvref_t<decltype(v.m_kTutorialUnitInfo)>>::get(r);
        v.m_wstrLastDate = Codec<std::remove_cvref_t<decltype(v.m_wstrLastDate)>>::get(r);
        return v;
    }
};

struct KEGS_SELECT_UNIT_4_NOT {
    std::int32_t                          m_iOK{};  // int
    std::vector<KChatBlackListUnit>       m_vecChatBlackList{};  // std::vector< KChatBlackListUnit >
    KDenyOptions                          m_kDenyOptions{};
    bool                                  m_bIsRecommend{};
    std::int64_t                          m_iRecommendUnitUID{};  // UidType
    std::vector<KTutorialDBUnitInfo>      m_vecTutorialDBUnitInfo{};  // std::vector< KTutorialDBUnitInfo >
    std::vector<KConnectTimeEventInfo>    m_vecConnectTimeEvent{};  // std::vector< KConnectTimeEventInfo >
    std::vector<KCumulativeTimeEventInfo> m_vecCumulativeTimeEvent{};  // std::vector< KCumulativeTimeEventInfo >
    std::vector<KHenirRankingInfo>        m_vecHenirRanking{};  // std::vector< KHenirRankingInfo >
    KMessengerInfo                        m_kMessengerInfo{};
    KGamePlayStatusContainer              m_kGamePlayStatus{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_vecChatBlackList)>>::put(w, m_vecChatBlackList);
        Codec<std::remove_cvref_t<decltype(m_kDenyOptions)>>::put(w, m_kDenyOptions);
        Codec<std::remove_cvref_t<decltype(m_bIsRecommend)>>::put(w, m_bIsRecommend);
        Codec<std::remove_cvref_t<decltype(m_iRecommendUnitUID)>>::put(w, m_iRecommendUnitUID);
        Codec<std::remove_cvref_t<decltype(m_vecTutorialDBUnitInfo)>>::put(w, m_vecTutorialDBUnitInfo);
        Codec<std::remove_cvref_t<decltype(m_vecConnectTimeEvent)>>::put(w, m_vecConnectTimeEvent);
        Codec<std::remove_cvref_t<decltype(m_vecCumulativeTimeEvent)>>::put(w, m_vecCumulativeTimeEvent);
        Codec<std::remove_cvref_t<decltype(m_vecHenirRanking)>>::put(w, m_vecHenirRanking);
        Codec<std::remove_cvref_t<decltype(m_kMessengerInfo)>>::put(w, m_kMessengerInfo);
        Codec<std::remove_cvref_t<decltype(m_kGamePlayStatus)>>::put(w, m_kGamePlayStatus);
    }

    static KEGS_SELECT_UNIT_4_NOT read_from(Reader& r) {
        KEGS_SELECT_UNIT_4_NOT v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_vecChatBlackList = Codec<std::remove_cvref_t<decltype(v.m_vecChatBlackList)>>::get(r);
        v.m_kDenyOptions = Codec<std::remove_cvref_t<decltype(v.m_kDenyOptions)>>::get(r);
        v.m_bIsRecommend = Codec<std::remove_cvref_t<decltype(v.m_bIsRecommend)>>::get(r);
        v.m_iRecommendUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iRecommendUnitUID)>>::get(r);
        v.m_vecTutorialDBUnitInfo = Codec<std::remove_cvref_t<decltype(v.m_vecTutorialDBUnitInfo)>>::get(r);
        v.m_vecConnectTimeEvent = Codec<std::remove_cvref_t<decltype(v.m_vecConnectTimeEvent)>>::get(r);
        v.m_vecCumulativeTimeEvent = Codec<std::remove_cvref_t<decltype(v.m_vecCumulativeTimeEvent)>>::get(r);
        v.m_vecHenirRanking = Codec<std::remove_cvref_t<decltype(v.m_vecHenirRanking)>>::get(r);
        v.m_kMessengerInfo = Codec<std::remove_cvref_t<decltype(v.m_kMessengerInfo)>>::get(r);
        v.m_kGamePlayStatus = Codec<std::remove_cvref_t<decltype(v.m_kGamePlayStatus)>>::get(r);
        return v;
    }
};

struct KUnitBuffInfo {
    std::int64_t                      m_iInsertTime{};  // __int64
    std::map<std::int32_t, KBuffInfo> m_mapBuffInfo{};  // std::map< int, KBuffInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iInsertTime)>>::put(w, m_iInsertTime);
        Codec<std::remove_cvref_t<decltype(m_mapBuffInfo)>>::put(w, m_mapBuffInfo);
    }

    static KUnitBuffInfo read_from(Reader& r) {
        KUnitBuffInfo v;
        v.m_iInsertTime = Codec<std::remove_cvref_t<decltype(v.m_iInsertTime)>>::get(r);
        v.m_mapBuffInfo = Codec<std::remove_cvref_t<decltype(v.m_mapBuffInfo)>>::get(r);
        return v;
    }
};

struct KELG_UPDATE_USER_UNIT_BUFF_INFO_ACK {
    KUnitBuffInfo m_kUnitBuffInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kUnitBuffInfo)>>::put(w, m_kUnitBuffInfo);
    }

    static KELG_UPDATE_USER_UNIT_BUFF_INFO_ACK read_from(Reader& r) {
        KELG_UPDATE_USER_UNIT_BUFF_INFO_ACK v;
        v.m_kUnitBuffInfo = Codec<std::remove_cvref_t<decltype(v.m_kUnitBuffInfo)>>::get(r);
        return v;
    }
};

struct KUnitSkillData {
    std::array<KSkillData, 4> m_aEquippedSkill{};  // KSkillData[EQUIPPED_SKILL_SLOT_COUNT]
    std::array<KSkillData, 4> m_aEquippedSkillSlotB{};  // KSkillData[EQUIPPED_SKILL_SLOT_COUNT]
    std::u16string            m_wstrSkillSlotBEndDate{};  // std::wstring
    char                      m_cSkillSlotBExpirationState{};
    std::vector<KSkillData>   m_vecPassiveSkill{};  // std::vector< KSkillData >
    std::vector<KSkillData>   m_vecGuildPassiveSkill{};  // std::vector< KSkillData >
    std::vector<std::int32_t> m_vecSkillNote{};  // std::vector< int >
    std::uint8_t              m_nActiveSkillPagesNumber{};  // unsigned char
    std::uint8_t              m_nTheNumberOfSkillPagesAvailable{};  // unsigned char

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_aEquippedSkill[0])>>::put(w, m_aEquippedSkill[0]);
        Codec<std::remove_cvref_t<decltype(m_aEquippedSkill[1])>>::put(w, m_aEquippedSkill[1]);
        Codec<std::remove_cvref_t<decltype(m_aEquippedSkill[2])>>::put(w, m_aEquippedSkill[2]);
        Codec<std::remove_cvref_t<decltype(m_aEquippedSkill[3])>>::put(w, m_aEquippedSkill[3]);
        Codec<std::remove_cvref_t<decltype(m_aEquippedSkillSlotB[0])>>::put(w, m_aEquippedSkillSlotB[0]);
        Codec<std::remove_cvref_t<decltype(m_aEquippedSkillSlotB[1])>>::put(w, m_aEquippedSkillSlotB[1]);
        Codec<std::remove_cvref_t<decltype(m_aEquippedSkillSlotB[2])>>::put(w, m_aEquippedSkillSlotB[2]);
        Codec<std::remove_cvref_t<decltype(m_aEquippedSkillSlotB[3])>>::put(w, m_aEquippedSkillSlotB[3]);
        Codec<std::remove_cvref_t<decltype(m_wstrSkillSlotBEndDate)>>::put(w, m_wstrSkillSlotBEndDate);
        Codec<std::remove_cvref_t<decltype(m_cSkillSlotBExpirationState)>>::put(w, m_cSkillSlotBExpirationState);
        Codec<std::remove_cvref_t<decltype(m_vecPassiveSkill)>>::put(w, m_vecPassiveSkill);
        Codec<std::remove_cvref_t<decltype(m_vecGuildPassiveSkill)>>::put(w, m_vecGuildPassiveSkill);
        Codec<std::remove_cvref_t<decltype(m_vecSkillNote)>>::put(w, m_vecSkillNote);
        Codec<std::remove_cvref_t<decltype(m_nActiveSkillPagesNumber)>>::put(w, m_nActiveSkillPagesNumber);
        Codec<std::remove_cvref_t<decltype(m_nTheNumberOfSkillPagesAvailable)>>::put(w, m_nTheNumberOfSkillPagesAvailable);
    }

    static KUnitSkillData read_from(Reader& r) {
        KUnitSkillData v;
        v.m_aEquippedSkill[0] = Codec<std::remove_cvref_t<decltype(v.m_aEquippedSkill[0])>>::get(r);
        v.m_aEquippedSkill[1] = Codec<std::remove_cvref_t<decltype(v.m_aEquippedSkill[1])>>::get(r);
        v.m_aEquippedSkill[2] = Codec<std::remove_cvref_t<decltype(v.m_aEquippedSkill[2])>>::get(r);
        v.m_aEquippedSkill[3] = Codec<std::remove_cvref_t<decltype(v.m_aEquippedSkill[3])>>::get(r);
        v.m_aEquippedSkillSlotB[0] = Codec<std::remove_cvref_t<decltype(v.m_aEquippedSkillSlotB[0])>>::get(r);
        v.m_aEquippedSkillSlotB[1] = Codec<std::remove_cvref_t<decltype(v.m_aEquippedSkillSlotB[1])>>::get(r);
        v.m_aEquippedSkillSlotB[2] = Codec<std::remove_cvref_t<decltype(v.m_aEquippedSkillSlotB[2])>>::get(r);
        v.m_aEquippedSkillSlotB[3] = Codec<std::remove_cvref_t<decltype(v.m_aEquippedSkillSlotB[3])>>::get(r);
        v.m_wstrSkillSlotBEndDate = Codec<std::remove_cvref_t<decltype(v.m_wstrSkillSlotBEndDate)>>::get(r);
        v.m_cSkillSlotBExpirationState = Codec<std::remove_cvref_t<decltype(v.m_cSkillSlotBExpirationState)>>::get(r);
        v.m_vecPassiveSkill = Codec<std::remove_cvref_t<decltype(v.m_vecPassiveSkill)>>::get(r);
        v.m_vecGuildPassiveSkill = Codec<std::remove_cvref_t<decltype(v.m_vecGuildPassiveSkill)>>::get(r);
        v.m_vecSkillNote = Codec<std::remove_cvref_t<decltype(v.m_vecSkillNote)>>::get(r);
        v.m_nActiveSkillPagesNumber = Codec<std::remove_cvref_t<decltype(v.m_nActiveSkillPagesNumber)>>::get(r);
        v.m_nTheNumberOfSkillPagesAvailable = Codec<std::remove_cvref_t<decltype(v.m_nTheNumberOfSkillPagesAvailable)>>::get(r);
        return v;
    }
};

struct KEGS_CHANGE_SKILL_INFO_IN_ROOM_NOT {
    std::int64_t   m_iUnitUID{};  // UidType
    std::uint8_t   m_ucMemberShipGrade{};  // u_char
    KStat          m_kGameStat{};
    KUnitSkillData m_UnitSkillData{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_ucMemberShipGrade)>>::put(w, m_ucMemberShipGrade);
        Codec<std::remove_cvref_t<decltype(m_kGameStat)>>::put(w, m_kGameStat);
        Codec<std::remove_cvref_t<decltype(m_UnitSkillData)>>::put(w, m_UnitSkillData);
    }

    static KEGS_CHANGE_SKILL_INFO_IN_ROOM_NOT read_from(Reader& r) {
        KEGS_CHANGE_SKILL_INFO_IN_ROOM_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_ucMemberShipGrade = Codec<std::remove_cvref_t<decltype(v.m_ucMemberShipGrade)>>::get(r);
        v.m_kGameStat = Codec<std::remove_cvref_t<decltype(v.m_kGameStat)>>::get(r);
        v.m_UnitSkillData = Codec<std::remove_cvref_t<decltype(v.m_UnitSkillData)>>::get(r);
        return v;
    }
};

struct KRoomUserInfo {
    std::int64_t                               m_iGSUID{};  // UidType
    std::int64_t                               m_iOwnerUserUID{};  // UidType
    std::int16_t                               m_sServerGroupID{};  // short
    char                                       m_cAuthLevel{};
    bool                                       m_bMale{};
    std::uint8_t                               m_ucAge{};  // UCHAR
    std::int64_t                               m_nUnitUID{};  // UidType
    std::uint32_t                              m_uiKNMSerialNum{};  // u_int
    char                                       m_cUnitClass{};
    std::u16string                             m_wstrNickName{};  // std::wstring
    std::int32_t                               m_iNumResurrectionStone{};  // int
    std::u16string                             m_wstrIP{};  // std::wstring
    std::uint16_t                              m_usPort{};  // USHORT
    std::u16string                             m_wstrInternalIP{};  // std::wstring
    std::uint16_t                              m_usInternalPort{};  // USHORT
    std::int64_t                               m_iPartyUID{};  // UidType
    std::vector<KGamePlayStatus>               m_vecGamePlayStatus{};  // std::vector< KGamePlayStatus >
    std::vector<KBuffInfo>                     m_vecBuffInfo{};  // std::vector< KBuffInfo >
    std::uint8_t                               m_ucLevel{};  // UCHAR
    KStat                                      m_kGameStat{};
    std::map<std::int32_t, KInventoryItemInfo> m_mapEquippedItem{};  // std::map< int, KInventoryItemInfo >
    std::map<std::int32_t, KInventoryItemInfo> m_mapSpecialItem{};  // std::map< int, KInventoryItemInfo >
    KUnitSkillData                             m_UnitSkillData{};
    bool                                       m_bIsPvpNpc{};
    std::int32_t                               m_iOfficialMatchCnt{};  // int
    std::int32_t                               m_iRating{};  // int
    std::int32_t                               m_iMaxRating{};  // int
    bool                                       m_bIsWinBeforeMatch{};
    char                                       m_cRank{};
    char                                       m_cRankForServer{};
    float                                      m_fKFactor{};
    bool                                       m_bIsRedistributionUser{};
    bool                                       m_bIsGameBang{};
    std::int32_t                               m_iPcBangType{};  // int
    bool                                       m_bIsObserver{};
    std::map<char, float>                      m_mapBonusRate{};  // std::map< char, float >
    bool                                       m_bIsRingofpvprebirth{};
    bool                                       m_bIsGuestUser{};
    std::map<std::int32_t, KSubQuestInfo>      m_mapOngoingQuest{};  // std::map< int, KSubQuestInfo >
    std::int32_t                               m_iTitleID{};  // int
    std::u16string                             m_wstrGuildName{};  // std::wstring
    std::int32_t                               m_iGuildUID{};  // int
    std::uint8_t                               m_ucMemberShipGrade{};  // u_char
    std::vector<KPetInfo>                      m_vecPet{};  // std::vector< KPetInfo >
    std::set<std::int32_t>                     m_setQuestInfo{};  // std::set< int >
    bool                                       m_bUseItem{};
    std::set<std::int32_t>                     m_setGoingQuestInfo{};  // std::set< int >
    bool                                       m_bComeBackUser{};
    bool                                       m_bHenirReward{};
    std::set<std::int32_t>                     m_setUseSkillBuffInPlay{};  // std::set< int >
    bool                                       m_bEnterCashShop{};
    std::int64_t                               m_iRidingPetUID{};  // UidType
    std::uint16_t                              m_usRidingPetID{};  // USHORT
    char                                       m_cWeddingStatus{};
    std::int64_t                               m_iLoverUnitUID{};  // UidType
    std::int32_t                               m_iEventQuestClearCount{};  // int
    std::int32_t                               m_iExchangeCount{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iGSUID)>>::put(w, m_iGSUID);
        Codec<std::remove_cvref_t<decltype(m_iOwnerUserUID)>>::put(w, m_iOwnerUserUID);
        Codec<std::remove_cvref_t<decltype(m_sServerGroupID)>>::put(w, m_sServerGroupID);
        Codec<std::remove_cvref_t<decltype(m_cAuthLevel)>>::put(w, m_cAuthLevel);
        Codec<std::remove_cvref_t<decltype(m_bMale)>>::put(w, m_bMale);
        Codec<std::remove_cvref_t<decltype(m_ucAge)>>::put(w, m_ucAge);
        Codec<std::remove_cvref_t<decltype(m_nUnitUID)>>::put(w, m_nUnitUID);
        Codec<std::remove_cvref_t<decltype(m_uiKNMSerialNum)>>::put(w, m_uiKNMSerialNum);
        Codec<std::remove_cvref_t<decltype(m_cUnitClass)>>::put(w, m_cUnitClass);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_iNumResurrectionStone)>>::put(w, m_iNumResurrectionStone);
        Codec<std::remove_cvref_t<decltype(m_wstrIP)>>::put(w, m_wstrIP);
        Codec<std::remove_cvref_t<decltype(m_usPort)>>::put(w, m_usPort);
        Codec<std::remove_cvref_t<decltype(m_wstrInternalIP)>>::put(w, m_wstrInternalIP);
        Codec<std::remove_cvref_t<decltype(m_usInternalPort)>>::put(w, m_usInternalPort);
        Codec<std::remove_cvref_t<decltype(m_iPartyUID)>>::put(w, m_iPartyUID);
        Codec<std::remove_cvref_t<decltype(m_vecGamePlayStatus)>>::put(w, m_vecGamePlayStatus);
        Codec<std::remove_cvref_t<decltype(m_vecBuffInfo)>>::put(w, m_vecBuffInfo);
        Codec<std::remove_cvref_t<decltype(m_ucLevel)>>::put(w, m_ucLevel);
        Codec<std::remove_cvref_t<decltype(m_kGameStat)>>::put(w, m_kGameStat);
        Codec<std::remove_cvref_t<decltype(m_mapEquippedItem)>>::put(w, m_mapEquippedItem);
        Codec<std::remove_cvref_t<decltype(m_mapSpecialItem)>>::put(w, m_mapSpecialItem);
        Codec<std::remove_cvref_t<decltype(m_UnitSkillData)>>::put(w, m_UnitSkillData);
        Codec<std::remove_cvref_t<decltype(m_bIsPvpNpc)>>::put(w, m_bIsPvpNpc);
        Codec<std::remove_cvref_t<decltype(m_iOfficialMatchCnt)>>::put(w, m_iOfficialMatchCnt);
        Codec<std::remove_cvref_t<decltype(m_iRating)>>::put(w, m_iRating);
        Codec<std::remove_cvref_t<decltype(m_iMaxRating)>>::put(w, m_iMaxRating);
        Codec<std::remove_cvref_t<decltype(m_bIsWinBeforeMatch)>>::put(w, m_bIsWinBeforeMatch);
        Codec<std::remove_cvref_t<decltype(m_cRank)>>::put(w, m_cRank);
        Codec<std::remove_cvref_t<decltype(m_cRankForServer)>>::put(w, m_cRankForServer);
        Codec<std::remove_cvref_t<decltype(m_fKFactor)>>::put(w, m_fKFactor);
        Codec<std::remove_cvref_t<decltype(m_bIsRedistributionUser)>>::put(w, m_bIsRedistributionUser);
        Codec<std::remove_cvref_t<decltype(m_bIsGameBang)>>::put(w, m_bIsGameBang);
        Codec<std::remove_cvref_t<decltype(m_iPcBangType)>>::put(w, m_iPcBangType);
        Codec<std::remove_cvref_t<decltype(m_bIsObserver)>>::put(w, m_bIsObserver);
        Codec<std::remove_cvref_t<decltype(m_mapBonusRate)>>::put(w, m_mapBonusRate);
        Codec<std::remove_cvref_t<decltype(m_bIsRingofpvprebirth)>>::put(w, m_bIsRingofpvprebirth);
        Codec<std::remove_cvref_t<decltype(m_bIsGuestUser)>>::put(w, m_bIsGuestUser);
        Codec<std::remove_cvref_t<decltype(m_mapOngoingQuest)>>::put(w, m_mapOngoingQuest);
        Codec<std::remove_cvref_t<decltype(m_iTitleID)>>::put(w, m_iTitleID);
        Codec<std::remove_cvref_t<decltype(m_wstrGuildName)>>::put(w, m_wstrGuildName);
        Codec<std::remove_cvref_t<decltype(m_iGuildUID)>>::put(w, m_iGuildUID);
        Codec<std::remove_cvref_t<decltype(m_ucMemberShipGrade)>>::put(w, m_ucMemberShipGrade);
        Codec<std::remove_cvref_t<decltype(m_vecPet)>>::put(w, m_vecPet);
        Codec<std::remove_cvref_t<decltype(m_setQuestInfo)>>::put(w, m_setQuestInfo);
        Codec<std::remove_cvref_t<decltype(m_bUseItem)>>::put(w, m_bUseItem);
        Codec<std::remove_cvref_t<decltype(m_setGoingQuestInfo)>>::put(w, m_setGoingQuestInfo);
        Codec<std::remove_cvref_t<decltype(m_bComeBackUser)>>::put(w, m_bComeBackUser);
        Codec<std::remove_cvref_t<decltype(m_bHenirReward)>>::put(w, m_bHenirReward);
        Codec<std::remove_cvref_t<decltype(m_setUseSkillBuffInPlay)>>::put(w, m_setUseSkillBuffInPlay);
        Codec<std::remove_cvref_t<decltype(m_bEnterCashShop)>>::put(w, m_bEnterCashShop);
        Codec<std::remove_cvref_t<decltype(m_iRidingPetUID)>>::put(w, m_iRidingPetUID);
        Codec<std::remove_cvref_t<decltype(m_usRidingPetID)>>::put(w, m_usRidingPetID);
        Codec<std::remove_cvref_t<decltype(m_cWeddingStatus)>>::put(w, m_cWeddingStatus);
        Codec<std::remove_cvref_t<decltype(m_iLoverUnitUID)>>::put(w, m_iLoverUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iEventQuestClearCount)>>::put(w, m_iEventQuestClearCount);
        Codec<std::remove_cvref_t<decltype(m_iExchangeCount)>>::put(w, m_iExchangeCount);
    }

    static KRoomUserInfo read_from(Reader& r) {
        KRoomUserInfo v;
        v.m_iGSUID = Codec<std::remove_cvref_t<decltype(v.m_iGSUID)>>::get(r);
        v.m_iOwnerUserUID = Codec<std::remove_cvref_t<decltype(v.m_iOwnerUserUID)>>::get(r);
        v.m_sServerGroupID = Codec<std::remove_cvref_t<decltype(v.m_sServerGroupID)>>::get(r);
        v.m_cAuthLevel = Codec<std::remove_cvref_t<decltype(v.m_cAuthLevel)>>::get(r);
        v.m_bMale = Codec<std::remove_cvref_t<decltype(v.m_bMale)>>::get(r);
        v.m_ucAge = Codec<std::remove_cvref_t<decltype(v.m_ucAge)>>::get(r);
        v.m_nUnitUID = Codec<std::remove_cvref_t<decltype(v.m_nUnitUID)>>::get(r);
        v.m_uiKNMSerialNum = Codec<std::remove_cvref_t<decltype(v.m_uiKNMSerialNum)>>::get(r);
        v.m_cUnitClass = Codec<std::remove_cvref_t<decltype(v.m_cUnitClass)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_iNumResurrectionStone = Codec<std::remove_cvref_t<decltype(v.m_iNumResurrectionStone)>>::get(r);
        v.m_wstrIP = Codec<std::remove_cvref_t<decltype(v.m_wstrIP)>>::get(r);
        v.m_usPort = Codec<std::remove_cvref_t<decltype(v.m_usPort)>>::get(r);
        v.m_wstrInternalIP = Codec<std::remove_cvref_t<decltype(v.m_wstrInternalIP)>>::get(r);
        v.m_usInternalPort = Codec<std::remove_cvref_t<decltype(v.m_usInternalPort)>>::get(r);
        v.m_iPartyUID = Codec<std::remove_cvref_t<decltype(v.m_iPartyUID)>>::get(r);
        v.m_vecGamePlayStatus = Codec<std::remove_cvref_t<decltype(v.m_vecGamePlayStatus)>>::get(r);
        v.m_vecBuffInfo = Codec<std::remove_cvref_t<decltype(v.m_vecBuffInfo)>>::get(r);
        v.m_ucLevel = Codec<std::remove_cvref_t<decltype(v.m_ucLevel)>>::get(r);
        v.m_kGameStat = Codec<std::remove_cvref_t<decltype(v.m_kGameStat)>>::get(r);
        v.m_mapEquippedItem = Codec<std::remove_cvref_t<decltype(v.m_mapEquippedItem)>>::get(r);
        v.m_mapSpecialItem = Codec<std::remove_cvref_t<decltype(v.m_mapSpecialItem)>>::get(r);
        v.m_UnitSkillData = Codec<std::remove_cvref_t<decltype(v.m_UnitSkillData)>>::get(r);
        v.m_bIsPvpNpc = Codec<std::remove_cvref_t<decltype(v.m_bIsPvpNpc)>>::get(r);
        v.m_iOfficialMatchCnt = Codec<std::remove_cvref_t<decltype(v.m_iOfficialMatchCnt)>>::get(r);
        v.m_iRating = Codec<std::remove_cvref_t<decltype(v.m_iRating)>>::get(r);
        v.m_iMaxRating = Codec<std::remove_cvref_t<decltype(v.m_iMaxRating)>>::get(r);
        v.m_bIsWinBeforeMatch = Codec<std::remove_cvref_t<decltype(v.m_bIsWinBeforeMatch)>>::get(r);
        v.m_cRank = Codec<std::remove_cvref_t<decltype(v.m_cRank)>>::get(r);
        v.m_cRankForServer = Codec<std::remove_cvref_t<decltype(v.m_cRankForServer)>>::get(r);
        v.m_fKFactor = Codec<std::remove_cvref_t<decltype(v.m_fKFactor)>>::get(r);
        v.m_bIsRedistributionUser = Codec<std::remove_cvref_t<decltype(v.m_bIsRedistributionUser)>>::get(r);
        v.m_bIsGameBang = Codec<std::remove_cvref_t<decltype(v.m_bIsGameBang)>>::get(r);
        v.m_iPcBangType = Codec<std::remove_cvref_t<decltype(v.m_iPcBangType)>>::get(r);
        v.m_bIsObserver = Codec<std::remove_cvref_t<decltype(v.m_bIsObserver)>>::get(r);
        v.m_mapBonusRate = Codec<std::remove_cvref_t<decltype(v.m_mapBonusRate)>>::get(r);
        v.m_bIsRingofpvprebirth = Codec<std::remove_cvref_t<decltype(v.m_bIsRingofpvprebirth)>>::get(r);
        v.m_bIsGuestUser = Codec<std::remove_cvref_t<decltype(v.m_bIsGuestUser)>>::get(r);
        v.m_mapOngoingQuest = Codec<std::remove_cvref_t<decltype(v.m_mapOngoingQuest)>>::get(r);
        v.m_iTitleID = Codec<std::remove_cvref_t<decltype(v.m_iTitleID)>>::get(r);
        v.m_wstrGuildName = Codec<std::remove_cvref_t<decltype(v.m_wstrGuildName)>>::get(r);
        v.m_iGuildUID = Codec<std::remove_cvref_t<decltype(v.m_iGuildUID)>>::get(r);
        v.m_ucMemberShipGrade = Codec<std::remove_cvref_t<decltype(v.m_ucMemberShipGrade)>>::get(r);
        v.m_vecPet = Codec<std::remove_cvref_t<decltype(v.m_vecPet)>>::get(r);
        v.m_setQuestInfo = Codec<std::remove_cvref_t<decltype(v.m_setQuestInfo)>>::get(r);
        v.m_bUseItem = Codec<std::remove_cvref_t<decltype(v.m_bUseItem)>>::get(r);
        v.m_setGoingQuestInfo = Codec<std::remove_cvref_t<decltype(v.m_setGoingQuestInfo)>>::get(r);
        v.m_bComeBackUser = Codec<std::remove_cvref_t<decltype(v.m_bComeBackUser)>>::get(r);
        v.m_bHenirReward = Codec<std::remove_cvref_t<decltype(v.m_bHenirReward)>>::get(r);
        v.m_setUseSkillBuffInPlay = Codec<std::remove_cvref_t<decltype(v.m_setUseSkillBuffInPlay)>>::get(r);
        v.m_bEnterCashShop = Codec<std::remove_cvref_t<decltype(v.m_bEnterCashShop)>>::get(r);
        v.m_iRidingPetUID = Codec<std::remove_cvref_t<decltype(v.m_iRidingPetUID)>>::get(r);
        v.m_usRidingPetID = Codec<std::remove_cvref_t<decltype(v.m_usRidingPetID)>>::get(r);
        v.m_cWeddingStatus = Codec<std::remove_cvref_t<decltype(v.m_cWeddingStatus)>>::get(r);
        v.m_iLoverUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iLoverUnitUID)>>::get(r);
        v.m_iEventQuestClearCount = Codec<std::remove_cvref_t<decltype(v.m_iEventQuestClearCount)>>::get(r);
        v.m_iExchangeCount = Codec<std::remove_cvref_t<decltype(v.m_iExchangeCount)>>::get(r);
        return v;
    }
};

struct KEGB_GET_ROOM_USER_INFO_FOR_MATCH_ACK {
    KRoomUserInfo m_kRoomUserInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kRoomUserInfo)>>::put(w, m_kRoomUserInfo);
    }

    static KEGB_GET_ROOM_USER_INFO_FOR_MATCH_ACK read_from(Reader& r) {
        KEGB_GET_ROOM_USER_INFO_FOR_MATCH_ACK v;
        v.m_kRoomUserInfo = Codec<std::remove_cvref_t<decltype(v.m_kRoomUserInfo)>>::get(r);
        return v;
    }
};

struct KEGB_OPEN_PVP_ROOM_FOR_MATCH_REQ {
    std::int64_t                         m_iMatchUID{};  // UidType
    char                                 m_cMatchType{};
    char                                 m_cPvpNpcType{};
    KRoomInfo                            m_kRoomInfo{};
    std::vector<KRoomUserInfo>           m_vecRedTeam{};  // std::vector< KRoomUserInfo >
    std::vector<KRoomUserInfo>           m_vecBlueTeam{};  // std::vector< KRoomUserInfo >
    std::map<std::int64_t, std::int32_t> m_mapMatchWaitTime{};  // std::map< UidType, int >
    std::map<std::int64_t, KPvpNpcInfo>  m_mapPvpNpcInfo{};  // std::map< UidType, KPvpNpcInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iMatchUID)>>::put(w, m_iMatchUID);
        Codec<std::remove_cvref_t<decltype(m_cMatchType)>>::put(w, m_cMatchType);
        Codec<std::remove_cvref_t<decltype(m_cPvpNpcType)>>::put(w, m_cPvpNpcType);
        Codec<std::remove_cvref_t<decltype(m_kRoomInfo)>>::put(w, m_kRoomInfo);
        Codec<std::remove_cvref_t<decltype(m_vecRedTeam)>>::put(w, m_vecRedTeam);
        Codec<std::remove_cvref_t<decltype(m_vecBlueTeam)>>::put(w, m_vecBlueTeam);
        Codec<std::remove_cvref_t<decltype(m_mapMatchWaitTime)>>::put(w, m_mapMatchWaitTime);
        Codec<std::remove_cvref_t<decltype(m_mapPvpNpcInfo)>>::put(w, m_mapPvpNpcInfo);
    }

    static KEGB_OPEN_PVP_ROOM_FOR_MATCH_REQ read_from(Reader& r) {
        KEGB_OPEN_PVP_ROOM_FOR_MATCH_REQ v;
        v.m_iMatchUID = Codec<std::remove_cvref_t<decltype(v.m_iMatchUID)>>::get(r);
        v.m_cMatchType = Codec<std::remove_cvref_t<decltype(v.m_cMatchType)>>::get(r);
        v.m_cPvpNpcType = Codec<std::remove_cvref_t<decltype(v.m_cPvpNpcType)>>::get(r);
        v.m_kRoomInfo = Codec<std::remove_cvref_t<decltype(v.m_kRoomInfo)>>::get(r);
        v.m_vecRedTeam = Codec<std::remove_cvref_t<decltype(v.m_vecRedTeam)>>::get(r);
        v.m_vecBlueTeam = Codec<std::remove_cvref_t<decltype(v.m_vecBlueTeam)>>::get(r);
        v.m_mapMatchWaitTime = Codec<std::remove_cvref_t<decltype(v.m_mapMatchWaitTime)>>::get(r);
        v.m_mapPvpNpcInfo = Codec<std::remove_cvref_t<decltype(v.m_mapPvpNpcInfo)>>::get(r);
        return v;
    }
};

struct KEGS_CREATE_TC_ROOM_ACK {
    std::int32_t  m_iOK{};  // int
    KTCInfo       m_kTCInfo{};
    KRoomUserInfo m_kRoomUserInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_kTCInfo)>>::put(w, m_kTCInfo);
        Codec<std::remove_cvref_t<decltype(m_kRoomUserInfo)>>::put(w, m_kRoomUserInfo);
    }

    static KEGS_CREATE_TC_ROOM_ACK read_from(Reader& r) {
        KEGS_CREATE_TC_ROOM_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_kTCInfo = Codec<std::remove_cvref_t<decltype(v.m_kTCInfo)>>::get(r);
        v.m_kRoomUserInfo = Codec<std::remove_cvref_t<decltype(v.m_kRoomUserInfo)>>::get(r);
        return v;
    }
};

struct KERM_CHANGE_EQUIPPED_ITEM_IN_ROOM_NOT {
    char                            m_cChangeEquippedPacketType{};
    std::int64_t                    m_UnitUID{};  // UidType
    std::vector<KInventoryItemInfo> m_vecInventorySlotInfo{};  // std::vector< KInventoryItemInfo>
    KRoomUserInfo                   m_kRoomUserInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cChangeEquippedPacketType)>>::put(w, m_cChangeEquippedPacketType);
        Codec<std::remove_cvref_t<decltype(m_UnitUID)>>::put(w, m_UnitUID);
        Codec<std::remove_cvref_t<decltype(m_vecInventorySlotInfo)>>::put(w, m_vecInventorySlotInfo);
        Codec<std::remove_cvref_t<decltype(m_kRoomUserInfo)>>::put(w, m_kRoomUserInfo);
    }

    static KERM_CHANGE_EQUIPPED_ITEM_IN_ROOM_NOT read_from(Reader& r) {
        KERM_CHANGE_EQUIPPED_ITEM_IN_ROOM_NOT v;
        v.m_cChangeEquippedPacketType = Codec<std::remove_cvref_t<decltype(v.m_cChangeEquippedPacketType)>>::get(r);
        v.m_UnitUID = Codec<std::remove_cvref_t<decltype(v.m_UnitUID)>>::get(r);
        v.m_vecInventorySlotInfo = Codec<std::remove_cvref_t<decltype(v.m_vecInventorySlotInfo)>>::get(r);
        v.m_kRoomUserInfo = Codec<std::remove_cvref_t<decltype(v.m_kRoomUserInfo)>>::get(r);
        return v;
    }
};

struct KERM_CHAR_LEVEL_UP_NOT {
    KStat         m_kBaseStat{};
    KRoomUserInfo m_kRoomUserInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kBaseStat)>>::put(w, m_kBaseStat);
        Codec<std::remove_cvref_t<decltype(m_kRoomUserInfo)>>::put(w, m_kRoomUserInfo);
    }

    static KERM_CHAR_LEVEL_UP_NOT read_from(Reader& r) {
        KERM_CHAR_LEVEL_UP_NOT v;
        v.m_kBaseStat = Codec<std::remove_cvref_t<decltype(v.m_kBaseStat)>>::get(r);
        v.m_kRoomUserInfo = Codec<std::remove_cvref_t<decltype(v.m_kRoomUserInfo)>>::get(r);
        return v;
    }
};

struct KERM_JOIN_BATTLE_FIELD_REQ {
    KRoomUserInfo             m_kRoomUserInfo{};
    std::vector<std::int64_t> m_vecStudentUnitUID{};  // std::vector< UidType >
    KBattleFieldJoinInfo      m_kBattleFieldJoinInfo{};
    KReturnToFieldInfo        m_kReturnToFieldInfo{};
    KBossFieldJoinInfo        m_kBossFieldJoinInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kRoomUserInfo)>>::put(w, m_kRoomUserInfo);
        Codec<std::remove_cvref_t<decltype(m_vecStudentUnitUID)>>::put(w, m_vecStudentUnitUID);
        Codec<std::remove_cvref_t<decltype(m_kBattleFieldJoinInfo)>>::put(w, m_kBattleFieldJoinInfo);
        Codec<std::remove_cvref_t<decltype(m_kReturnToFieldInfo)>>::put(w, m_kReturnToFieldInfo);
        Codec<std::remove_cvref_t<decltype(m_kBossFieldJoinInfo)>>::put(w, m_kBossFieldJoinInfo);
    }

    static KERM_JOIN_BATTLE_FIELD_REQ read_from(Reader& r) {
        KERM_JOIN_BATTLE_FIELD_REQ v;
        v.m_kRoomUserInfo = Codec<std::remove_cvref_t<decltype(v.m_kRoomUserInfo)>>::get(r);
        v.m_vecStudentUnitUID = Codec<std::remove_cvref_t<decltype(v.m_vecStudentUnitUID)>>::get(r);
        v.m_kBattleFieldJoinInfo = Codec<std::remove_cvref_t<decltype(v.m_kBattleFieldJoinInfo)>>::get(r);
        v.m_kReturnToFieldInfo = Codec<std::remove_cvref_t<decltype(v.m_kReturnToFieldInfo)>>::get(r);
        v.m_kBossFieldJoinInfo = Codec<std::remove_cvref_t<decltype(v.m_kBossFieldJoinInfo)>>::get(r);
        return v;
    }
};

struct KERM_JOIN_ROOM_REQ {
    bool                      m_bQuickJoin{};
    std::u16string            m_wstrPassword{};  // std::wstring
    KRoomUserInfo             m_kRoomUserInfo{};
    std::vector<std::int64_t> m_vecStudentUnitUID{};  // std::vector< UidType >
    bool                      m_bInvitation{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bQuickJoin)>>::put(w, m_bQuickJoin);
        Codec<std::remove_cvref_t<decltype(m_wstrPassword)>>::put(w, m_wstrPassword);
        Codec<std::remove_cvref_t<decltype(m_kRoomUserInfo)>>::put(w, m_kRoomUserInfo);
        Codec<std::remove_cvref_t<decltype(m_vecStudentUnitUID)>>::put(w, m_vecStudentUnitUID);
        Codec<std::remove_cvref_t<decltype(m_bInvitation)>>::put(w, m_bInvitation);
    }

    static KERM_JOIN_ROOM_REQ read_from(Reader& r) {
        KERM_JOIN_ROOM_REQ v;
        v.m_bQuickJoin = Codec<std::remove_cvref_t<decltype(v.m_bQuickJoin)>>::get(r);
        v.m_wstrPassword = Codec<std::remove_cvref_t<decltype(v.m_wstrPassword)>>::get(r);
        v.m_kRoomUserInfo = Codec<std::remove_cvref_t<decltype(v.m_kRoomUserInfo)>>::get(r);
        v.m_vecStudentUnitUID = Codec<std::remove_cvref_t<decltype(v.m_vecStudentUnitUID)>>::get(r);
        v.m_bInvitation = Codec<std::remove_cvref_t<decltype(v.m_bInvitation)>>::get(r);
        return v;
    }
};

struct KERM_MY_USER_UNIT_INFO_TO_SERVER_REQ {
    KRoomUserInfo          m_kRoomUserInfo{};
    KDungeonPlayResultInfo m_kDungeonUnitInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kRoomUserInfo)>>::put(w, m_kRoomUserInfo);
        Codec<std::remove_cvref_t<decltype(m_kDungeonUnitInfo)>>::put(w, m_kDungeonUnitInfo);
    }

    static KERM_MY_USER_UNIT_INFO_TO_SERVER_REQ read_from(Reader& r) {
        KERM_MY_USER_UNIT_INFO_TO_SERVER_REQ v;
        v.m_kRoomUserInfo = Codec<std::remove_cvref_t<decltype(v.m_kRoomUserInfo)>>::get(r);
        v.m_kDungeonUnitInfo = Codec<std::remove_cvref_t<decltype(v.m_kDungeonUnitInfo)>>::get(r);
        return v;
    }
};

struct KERM_OPEN_BATTLE_FIELD_REQ {
    KRoomInfo                 m_kRoomInfo{};
    KRoomUserInfo             m_kRoomUserInfo{};
    std::vector<std::int64_t> m_vecStudentUnitUID{};  // std::vector< UidType >
    KBattleFieldJoinInfo      m_kBattleFieldJoinInfo{};
    KReturnToFieldInfo        m_kReturnToFieldInfo{};
    KBossFieldCreateInfo      m_KBossFieldCreateInfo{};
    KBossFieldJoinInfo        m_kBossFieldJoinInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kRoomInfo)>>::put(w, m_kRoomInfo);
        Codec<std::remove_cvref_t<decltype(m_kRoomUserInfo)>>::put(w, m_kRoomUserInfo);
        Codec<std::remove_cvref_t<decltype(m_vecStudentUnitUID)>>::put(w, m_vecStudentUnitUID);
        Codec<std::remove_cvref_t<decltype(m_kBattleFieldJoinInfo)>>::put(w, m_kBattleFieldJoinInfo);
        Codec<std::remove_cvref_t<decltype(m_kReturnToFieldInfo)>>::put(w, m_kReturnToFieldInfo);
        Codec<std::remove_cvref_t<decltype(m_KBossFieldCreateInfo)>>::put(w, m_KBossFieldCreateInfo);
        Codec<std::remove_cvref_t<decltype(m_kBossFieldJoinInfo)>>::put(w, m_kBossFieldJoinInfo);
    }

    static KERM_OPEN_BATTLE_FIELD_REQ read_from(Reader& r) {
        KERM_OPEN_BATTLE_FIELD_REQ v;
        v.m_kRoomInfo = Codec<std::remove_cvref_t<decltype(v.m_kRoomInfo)>>::get(r);
        v.m_kRoomUserInfo = Codec<std::remove_cvref_t<decltype(v.m_kRoomUserInfo)>>::get(r);
        v.m_vecStudentUnitUID = Codec<std::remove_cvref_t<decltype(v.m_vecStudentUnitUID)>>::get(r);
        v.m_kBattleFieldJoinInfo = Codec<std::remove_cvref_t<decltype(v.m_kBattleFieldJoinInfo)>>::get(r);
        v.m_kReturnToFieldInfo = Codec<std::remove_cvref_t<decltype(v.m_kReturnToFieldInfo)>>::get(r);
        v.m_KBossFieldCreateInfo = Codec<std::remove_cvref_t<decltype(v.m_KBossFieldCreateInfo)>>::get(r);
        v.m_kBossFieldJoinInfo = Codec<std::remove_cvref_t<decltype(v.m_kBossFieldJoinInfo)>>::get(r);
        return v;
    }
};

struct KERM_OPEN_ROOM_REQ {
    bool                      m_bQuickJoin{};
    KRoomInfo                 m_kRoomInfo{};
    KRoomUserInfo             m_kRoomUserInfo{};
    std::vector<std::int64_t> m_vecStudentUnitUID{};  // std::vector< UidType >
    std::u16string            m_wstrChannelIP{};  // std::wstring
    std::int32_t              m_iCurEXP{};  // int
    std::int32_t              m_iCurED{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bQuickJoin)>>::put(w, m_bQuickJoin);
        Codec<std::remove_cvref_t<decltype(m_kRoomInfo)>>::put(w, m_kRoomInfo);
        Codec<std::remove_cvref_t<decltype(m_kRoomUserInfo)>>::put(w, m_kRoomUserInfo);
        Codec<std::remove_cvref_t<decltype(m_vecStudentUnitUID)>>::put(w, m_vecStudentUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrChannelIP)>>::put(w, m_wstrChannelIP);
        Codec<std::remove_cvref_t<decltype(m_iCurEXP)>>::put(w, m_iCurEXP);
        Codec<std::remove_cvref_t<decltype(m_iCurED)>>::put(w, m_iCurED);
    }

    static KERM_OPEN_ROOM_REQ read_from(Reader& r) {
        KERM_OPEN_ROOM_REQ v;
        v.m_bQuickJoin = Codec<std::remove_cvref_t<decltype(v.m_bQuickJoin)>>::get(r);
        v.m_kRoomInfo = Codec<std::remove_cvref_t<decltype(v.m_kRoomInfo)>>::get(r);
        v.m_kRoomUserInfo = Codec<std::remove_cvref_t<decltype(v.m_kRoomUserInfo)>>::get(r);
        v.m_vecStudentUnitUID = Codec<std::remove_cvref_t<decltype(v.m_vecStudentUnitUID)>>::get(r);
        v.m_wstrChannelIP = Codec<std::remove_cvref_t<decltype(v.m_wstrChannelIP)>>::get(r);
        v.m_iCurEXP = Codec<std::remove_cvref_t<decltype(v.m_iCurEXP)>>::get(r);
        v.m_iCurED = Codec<std::remove_cvref_t<decltype(v.m_iCurED)>>::get(r);
        return v;
    }
};

struct KERM_OPEN_TC_ROOM_REQ {
    KTCInfo       m_kTCInfo{};
    KRoomUserInfo m_kRoomUserInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kTCInfo)>>::put(w, m_kTCInfo);
        Codec<std::remove_cvref_t<decltype(m_kRoomUserInfo)>>::put(w, m_kRoomUserInfo);
    }

    static KERM_OPEN_TC_ROOM_REQ read_from(Reader& r) {
        KERM_OPEN_TC_ROOM_REQ v;
        v.m_kTCInfo = Codec<std::remove_cvref_t<decltype(v.m_kTCInfo)>>::get(r);
        v.m_kRoomUserInfo = Codec<std::remove_cvref_t<decltype(v.m_kRoomUserInfo)>>::get(r);
        return v;
    }
};

struct KERM_PREPARE_FOR_DEFENCE_DUNGEON_ACK {
    KRoomUserInfo m_kRoomUserInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kRoomUserInfo)>>::put(w, m_kRoomUserInfo);
    }

    static KERM_PREPARE_FOR_DEFENCE_DUNGEON_ACK read_from(Reader& r) {
        KERM_PREPARE_FOR_DEFENCE_DUNGEON_ACK v;
        v.m_kRoomUserInfo = Codec<std::remove_cvref_t<decltype(v.m_kRoomUserInfo)>>::get(r);
        return v;
    }
};

struct KERM_ZERO_ENDURANCE_ITEM_IN_ROOM_NOT {
    std::int64_t                    m_UnitUID{};  // UidType
    std::vector<KInventoryItemInfo> m_vecInventorySlotInfo{};  // std::vector< KInventoryItemInfo >
    KRoomUserInfo                   m_kRoomUserInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_UnitUID)>>::put(w, m_UnitUID);
        Codec<std::remove_cvref_t<decltype(m_vecInventorySlotInfo)>>::put(w, m_vecInventorySlotInfo);
        Codec<std::remove_cvref_t<decltype(m_kRoomUserInfo)>>::put(w, m_kRoomUserInfo);
    }

    static KERM_ZERO_ENDURANCE_ITEM_IN_ROOM_NOT read_from(Reader& r) {
        KERM_ZERO_ENDURANCE_ITEM_IN_ROOM_NOT v;
        v.m_UnitUID = Codec<std::remove_cvref_t<decltype(v.m_UnitUID)>>::get(r);
        v.m_vecInventorySlotInfo = Codec<std::remove_cvref_t<decltype(v.m_vecInventorySlotInfo)>>::get(r);
        v.m_kRoomUserInfo = Codec<std::remove_cvref_t<decltype(v.m_kRoomUserInfo)>>::get(r);
        return v;
    }
};

struct KPartyRoomUserInfo {
    KRoomUserInfo             m_kRoomUserInfo{};
    std::vector<std::int64_t> m_vecStudentUnitUID{};  // std::vector< UidType >
    KOldPartyInfo             m_kOldPartyInfo{};
    bool                      m_bAutoPartyBonus{};
    std::u16string            m_wstrChannelIP{};  // std::wstring
    std::int32_t              m_iCurEXP{};  // int
    std::int32_t              m_iCurED{};  // int
    std::int32_t              m_iAutoPartyWaitTime{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kRoomUserInfo)>>::put(w, m_kRoomUserInfo);
        Codec<std::remove_cvref_t<decltype(m_vecStudentUnitUID)>>::put(w, m_vecStudentUnitUID);
        Codec<std::remove_cvref_t<decltype(m_kOldPartyInfo)>>::put(w, m_kOldPartyInfo);
        Codec<std::remove_cvref_t<decltype(m_bAutoPartyBonus)>>::put(w, m_bAutoPartyBonus);
        Codec<std::remove_cvref_t<decltype(m_wstrChannelIP)>>::put(w, m_wstrChannelIP);
        Codec<std::remove_cvref_t<decltype(m_iCurEXP)>>::put(w, m_iCurEXP);
        Codec<std::remove_cvref_t<decltype(m_iCurED)>>::put(w, m_iCurED);
        Codec<std::remove_cvref_t<decltype(m_iAutoPartyWaitTime)>>::put(w, m_iAutoPartyWaitTime);
    }

    static KPartyRoomUserInfo read_from(Reader& r) {
        KPartyRoomUserInfo v;
        v.m_kRoomUserInfo = Codec<std::remove_cvref_t<decltype(v.m_kRoomUserInfo)>>::get(r);
        v.m_vecStudentUnitUID = Codec<std::remove_cvref_t<decltype(v.m_vecStudentUnitUID)>>::get(r);
        v.m_kOldPartyInfo = Codec<std::remove_cvref_t<decltype(v.m_kOldPartyInfo)>>::get(r);
        v.m_bAutoPartyBonus = Codec<std::remove_cvref_t<decltype(v.m_bAutoPartyBonus)>>::get(r);
        v.m_wstrChannelIP = Codec<std::remove_cvref_t<decltype(v.m_wstrChannelIP)>>::get(r);
        v.m_iCurEXP = Codec<std::remove_cvref_t<decltype(v.m_iCurEXP)>>::get(r);
        v.m_iCurED = Codec<std::remove_cvref_t<decltype(v.m_iCurED)>>::get(r);
        v.m_iAutoPartyWaitTime = Codec<std::remove_cvref_t<decltype(v.m_iAutoPartyWaitTime)>>::get(r);
        return v;
    }
};

struct KEGB_GET_REGROUP_PARTY_INFO_ACK {
    KPartyUserInfo     m_kPartyUserInfo{};
    KPartyRoomUserInfo m_kPartyRoomUserInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kPartyUserInfo)>>::put(w, m_kPartyUserInfo);
        Codec<std::remove_cvref_t<decltype(m_kPartyRoomUserInfo)>>::put(w, m_kPartyRoomUserInfo);
    }

    static KEGB_GET_REGROUP_PARTY_INFO_ACK read_from(Reader& r) {
        KEGB_GET_REGROUP_PARTY_INFO_ACK v;
        v.m_kPartyUserInfo = Codec<std::remove_cvref_t<decltype(v.m_kPartyUserInfo)>>::get(r);
        v.m_kPartyRoomUserInfo = Codec<std::remove_cvref_t<decltype(v.m_kPartyRoomUserInfo)>>::get(r);
        return v;
    }
};

struct KEPM_CHECK_FOR_PARTY_GAME_START_ACK {
    std::int32_t                    m_iOK{};  // int
    std::int64_t                    m_iPartyUID{};  // UidType
    std::uint16_t                   m_usEventID{};  // unsigned short
    std::int16_t                    m_sWorldID{};  // short
    std::vector<KPartyRoomUserInfo> m_vecUserInfo{};  // std::vector< KPartyRoomUserInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iPartyUID)>>::put(w, m_iPartyUID);
        Codec<std::remove_cvref_t<decltype(m_usEventID)>>::put(w, m_usEventID);
        Codec<std::remove_cvref_t<decltype(m_sWorldID)>>::put(w, m_sWorldID);
        Codec<std::remove_cvref_t<decltype(m_vecUserInfo)>>::put(w, m_vecUserInfo);
    }

    static KEPM_CHECK_FOR_PARTY_GAME_START_ACK read_from(Reader& r) {
        KEPM_CHECK_FOR_PARTY_GAME_START_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iPartyUID = Codec<std::remove_cvref_t<decltype(v.m_iPartyUID)>>::get(r);
        v.m_usEventID = Codec<std::remove_cvref_t<decltype(v.m_usEventID)>>::get(r);
        v.m_sWorldID = Codec<std::remove_cvref_t<decltype(v.m_sWorldID)>>::get(r);
        v.m_vecUserInfo = Codec<std::remove_cvref_t<decltype(v.m_vecUserInfo)>>::get(r);
        return v;
    }
};

struct KERM_PARTY_OPEN_DUNGEON_ROOM_REQ {
    KRoomInfo                       m_kRoomInfo{};
    std::int16_t                    m_sWorldID{};  // short
    std::vector<KPartyRoomUserInfo> m_vecUserInfo{};  // std::vector< KPartyRoomUserInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kRoomInfo)>>::put(w, m_kRoomInfo);
        Codec<std::remove_cvref_t<decltype(m_sWorldID)>>::put(w, m_sWorldID);
        Codec<std::remove_cvref_t<decltype(m_vecUserInfo)>>::put(w, m_vecUserInfo);
    }

    static KERM_PARTY_OPEN_DUNGEON_ROOM_REQ read_from(Reader& r) {
        KERM_PARTY_OPEN_DUNGEON_ROOM_REQ v;
        v.m_kRoomInfo = Codec<std::remove_cvref_t<decltype(v.m_kRoomInfo)>>::get(r);
        v.m_sWorldID = Codec<std::remove_cvref_t<decltype(v.m_sWorldID)>>::get(r);
        v.m_vecUserInfo = Codec<std::remove_cvref_t<decltype(v.m_vecUserInfo)>>::get(r);
        return v;
    }
};

struct KRegroupPartyUserInfo {
    KPartyUserInfo     m_kPartyUserInfo{};
    KPartyRoomUserInfo m_kPartyRoomUserInfo{};
    KOldPartyInfo      m_kOldPartyInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kPartyUserInfo)>>::put(w, m_kPartyUserInfo);
        Codec<std::remove_cvref_t<decltype(m_kPartyRoomUserInfo)>>::put(w, m_kPartyRoomUserInfo);
        Codec<std::remove_cvref_t<decltype(m_kOldPartyInfo)>>::put(w, m_kOldPartyInfo);
    }

    static KRegroupPartyUserInfo read_from(Reader& r) {
        KRegroupPartyUserInfo v;
        v.m_kPartyUserInfo = Codec<std::remove_cvref_t<decltype(v.m_kPartyUserInfo)>>::get(r);
        v.m_kPartyRoomUserInfo = Codec<std::remove_cvref_t<decltype(v.m_kPartyRoomUserInfo)>>::get(r);
        v.m_kOldPartyInfo = Codec<std::remove_cvref_t<decltype(v.m_kOldPartyInfo)>>::get(r);
        return v;
    }
};

struct KEPM_REGROUP_PARTY_FOR_AUTO_PARTY_DUNGEON_GAME_REQ {
    std::int64_t                       m_iAutoPartyUID{};  // UidType
    std::int64_t                       m_iAutoPartyHostUnitUID{};  // UidType
    KCreatePartyInfo                   m_kCreatePartyInfo{};
    std::vector<KRegroupPartyUserInfo> m_vecRegroupPartyUserInfo{};  // std::vector< KRegroupPartyUserInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iAutoPartyUID)>>::put(w, m_iAutoPartyUID);
        Codec<std::remove_cvref_t<decltype(m_iAutoPartyHostUnitUID)>>::put(w, m_iAutoPartyHostUnitUID);
        Codec<std::remove_cvref_t<decltype(m_kCreatePartyInfo)>>::put(w, m_kCreatePartyInfo);
        Codec<std::remove_cvref_t<decltype(m_vecRegroupPartyUserInfo)>>::put(w, m_vecRegroupPartyUserInfo);
    }

    static KEPM_REGROUP_PARTY_FOR_AUTO_PARTY_DUNGEON_GAME_REQ read_from(Reader& r) {
        KEPM_REGROUP_PARTY_FOR_AUTO_PARTY_DUNGEON_GAME_REQ v;
        v.m_iAutoPartyUID = Codec<std::remove_cvref_t<decltype(v.m_iAutoPartyUID)>>::get(r);
        v.m_iAutoPartyHostUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iAutoPartyHostUnitUID)>>::get(r);
        v.m_kCreatePartyInfo = Codec<std::remove_cvref_t<decltype(v.m_kCreatePartyInfo)>>::get(r);
        v.m_vecRegroupPartyUserInfo = Codec<std::remove_cvref_t<decltype(v.m_vecRegroupPartyUserInfo)>>::get(r);
        return v;
    }
};

struct KEPM_REQUEST_REGROUP_PARTY_NOT {
    KCreatePartyInfo                   m_kCreatePartyInfo{};
    std::int64_t                       m_iOldHostUnitUID{};  // UidType
    std::int16_t                       m_sPartyFever{};  // short
    std::vector<KRegroupPartyUserInfo> m_vecRegroupPartyUserInfo{};  // std::vector< KRegroupPartyUserInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kCreatePartyInfo)>>::put(w, m_kCreatePartyInfo);
        Codec<std::remove_cvref_t<decltype(m_iOldHostUnitUID)>>::put(w, m_iOldHostUnitUID);
        Codec<std::remove_cvref_t<decltype(m_sPartyFever)>>::put(w, m_sPartyFever);
        Codec<std::remove_cvref_t<decltype(m_vecRegroupPartyUserInfo)>>::put(w, m_vecRegroupPartyUserInfo);
    }

    static KEPM_REQUEST_REGROUP_PARTY_NOT read_from(Reader& r) {
        KEPM_REQUEST_REGROUP_PARTY_NOT v;
        v.m_kCreatePartyInfo = Codec<std::remove_cvref_t<decltype(v.m_kCreatePartyInfo)>>::get(r);
        v.m_iOldHostUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iOldHostUnitUID)>>::get(r);
        v.m_sPartyFever = Codec<std::remove_cvref_t<decltype(v.m_sPartyFever)>>::get(r);
        v.m_vecRegroupPartyUserInfo = Codec<std::remove_cvref_t<decltype(v.m_vecRegroupPartyUserInfo)>>::get(r);
        return v;
    }
};

struct KRoomSlotInfo {
    char          m_Index{};
    char          m_SlotState{};
    bool          m_bHost{};
    bool          m_bReady{};
    bool          m_bPitIn{};
    bool          m_bTrade{};
    std::int32_t  m_TeamNum{};  // int
    KRoomUserInfo m_kRoomUserInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_Index)>>::put(w, m_Index);
        Codec<std::remove_cvref_t<decltype(m_SlotState)>>::put(w, m_SlotState);
        Codec<std::remove_cvref_t<decltype(m_bHost)>>::put(w, m_bHost);
        Codec<std::remove_cvref_t<decltype(m_bReady)>>::put(w, m_bReady);
        Codec<std::remove_cvref_t<decltype(m_bPitIn)>>::put(w, m_bPitIn);
        Codec<std::remove_cvref_t<decltype(m_bTrade)>>::put(w, m_bTrade);
        Codec<std::remove_cvref_t<decltype(m_TeamNum)>>::put(w, m_TeamNum);
        Codec<std::remove_cvref_t<decltype(m_kRoomUserInfo)>>::put(w, m_kRoomUserInfo);
    }

    static KRoomSlotInfo read_from(Reader& r) {
        KRoomSlotInfo v;
        v.m_Index = Codec<std::remove_cvref_t<decltype(v.m_Index)>>::get(r);
        v.m_SlotState = Codec<std::remove_cvref_t<decltype(v.m_SlotState)>>::get(r);
        v.m_bHost = Codec<std::remove_cvref_t<decltype(v.m_bHost)>>::get(r);
        v.m_bReady = Codec<std::remove_cvref_t<decltype(v.m_bReady)>>::get(r);
        v.m_bPitIn = Codec<std::remove_cvref_t<decltype(v.m_bPitIn)>>::get(r);
        v.m_bTrade = Codec<std::remove_cvref_t<decltype(v.m_bTrade)>>::get(r);
        v.m_TeamNum = Codec<std::remove_cvref_t<decltype(v.m_TeamNum)>>::get(r);
        v.m_kRoomUserInfo = Codec<std::remove_cvref_t<decltype(v.m_kRoomUserInfo)>>::get(r);
        return v;
    }
};

struct KEGS_CREATE_ROOM_ACK {
    std::int32_t               m_iOK{};  // int
    KRoomInfo                  m_RoomInfo{};
    std::vector<KRoomSlotInfo> m_vecSlot{};  // std::vector< KRoomSlotInfo >
    std::vector<KRoomSlotInfo> m_vecObserverSlot{};  // std::vector< KRoomSlotInfo >
    std::u16string             m_wstrCNIP{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_RoomInfo)>>::put(w, m_RoomInfo);
        Codec<std::remove_cvref_t<decltype(m_vecSlot)>>::put(w, m_vecSlot);
        Codec<std::remove_cvref_t<decltype(m_vecObserverSlot)>>::put(w, m_vecObserverSlot);
        Codec<std::remove_cvref_t<decltype(m_wstrCNIP)>>::put(w, m_wstrCNIP);
    }

    static KEGS_CREATE_ROOM_ACK read_from(Reader& r) {
        KEGS_CREATE_ROOM_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_RoomInfo = Codec<std::remove_cvref_t<decltype(v.m_RoomInfo)>>::get(r);
        v.m_vecSlot = Codec<std::remove_cvref_t<decltype(v.m_vecSlot)>>::get(r);
        v.m_vecObserverSlot = Codec<std::remove_cvref_t<decltype(v.m_vecObserverSlot)>>::get(r);
        v.m_wstrCNIP = Codec<std::remove_cvref_t<decltype(v.m_wstrCNIP)>>::get(r);
        return v;
    }
};

struct KEGS_CREATE_TUTORIAL_ROOM_ACK {
    std::int32_t               m_iOK{};  // int
    KRoomInfo                  m_RoomInfo{};
    std::vector<KRoomSlotInfo> m_vecSlot{};  // std::vector< KRoomSlotInfo >
    std::u16string             m_wstrCNIP{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_RoomInfo)>>::put(w, m_RoomInfo);
        Codec<std::remove_cvref_t<decltype(m_vecSlot)>>::put(w, m_vecSlot);
        Codec<std::remove_cvref_t<decltype(m_wstrCNIP)>>::put(w, m_wstrCNIP);
    }

    static KEGS_CREATE_TUTORIAL_ROOM_ACK read_from(Reader& r) {
        KEGS_CREATE_TUTORIAL_ROOM_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_RoomInfo = Codec<std::remove_cvref_t<decltype(v.m_RoomInfo)>>::get(r);
        v.m_vecSlot = Codec<std::remove_cvref_t<decltype(v.m_vecSlot)>>::get(r);
        v.m_wstrCNIP = Codec<std::remove_cvref_t<decltype(v.m_wstrCNIP)>>::get(r);
        return v;
    }
};

struct KEGS_DEFENCE_DUNGEON_GAME_START_NOT {
    std::int16_t                    m_sWorldID{};  // short
    KRoomInfo                       m_RoomInfo{};
    std::vector<KRoomSlotInfo>      m_vecSlot{};  // std::vector< KRoomSlotInfo >
    std::vector<KRoomSlotInfo>      m_vecObserverSlot{};  // std::vector< KRoomSlotInfo >
    std::u16string                  m_wstrCNIP{};  // std::wstring
    std::vector<KInventoryItemInfo> m_vecInventorySlotInfo{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_sWorldID)>>::put(w, m_sWorldID);
        Codec<std::remove_cvref_t<decltype(m_RoomInfo)>>::put(w, m_RoomInfo);
        Codec<std::remove_cvref_t<decltype(m_vecSlot)>>::put(w, m_vecSlot);
        Codec<std::remove_cvref_t<decltype(m_vecObserverSlot)>>::put(w, m_vecObserverSlot);
        Codec<std::remove_cvref_t<decltype(m_wstrCNIP)>>::put(w, m_wstrCNIP);
        Codec<std::remove_cvref_t<decltype(m_vecInventorySlotInfo)>>::put(w, m_vecInventorySlotInfo);
    }

    static KEGS_DEFENCE_DUNGEON_GAME_START_NOT read_from(Reader& r) {
        KEGS_DEFENCE_DUNGEON_GAME_START_NOT v;
        v.m_sWorldID = Codec<std::remove_cvref_t<decltype(v.m_sWorldID)>>::get(r);
        v.m_RoomInfo = Codec<std::remove_cvref_t<decltype(v.m_RoomInfo)>>::get(r);
        v.m_vecSlot = Codec<std::remove_cvref_t<decltype(v.m_vecSlot)>>::get(r);
        v.m_vecObserverSlot = Codec<std::remove_cvref_t<decltype(v.m_vecObserverSlot)>>::get(r);
        v.m_wstrCNIP = Codec<std::remove_cvref_t<decltype(v.m_wstrCNIP)>>::get(r);
        v.m_vecInventorySlotInfo = Codec<std::remove_cvref_t<decltype(v.m_vecInventorySlotInfo)>>::get(r);
        return v;
    }
};

struct KEGS_END_GAME_NOT {
    char                       m_cRoomState{};
    std::int32_t               m_iWinTeam{};  // int
    std::vector<KRoomSlotInfo> m_vecSlot{};  // std::vector< KRoomSlotInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cRoomState)>>::put(w, m_cRoomState);
        Codec<std::remove_cvref_t<decltype(m_iWinTeam)>>::put(w, m_iWinTeam);
        Codec<std::remove_cvref_t<decltype(m_vecSlot)>>::put(w, m_vecSlot);
    }

    static KEGS_END_GAME_NOT read_from(Reader& r) {
        KEGS_END_GAME_NOT v;
        v.m_cRoomState = Codec<std::remove_cvref_t<decltype(v.m_cRoomState)>>::get(r);
        v.m_iWinTeam = Codec<std::remove_cvref_t<decltype(v.m_iWinTeam)>>::get(r);
        v.m_vecSlot = Codec<std::remove_cvref_t<decltype(v.m_vecSlot)>>::get(r);
        return v;
    }
};

struct KEGS_GAME_START_PVP_MATCH_NOT {
    KRoomInfo                           m_RoomInfo{};
    std::vector<KRoomSlotInfo>          m_vecSlot{};  // std::vector< KRoomSlotInfo >
    std::vector<KRoomSlotInfo>          m_vecObserverSlot{};  // std::vector< KRoomSlotInfo >
    std::map<std::int64_t, KPvpNpcInfo> m_mapPvpNpcInfo{};  // std::map< UidType, KPvpNpcInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_RoomInfo)>>::put(w, m_RoomInfo);
        Codec<std::remove_cvref_t<decltype(m_vecSlot)>>::put(w, m_vecSlot);
        Codec<std::remove_cvref_t<decltype(m_vecObserverSlot)>>::put(w, m_vecObserverSlot);
        Codec<std::remove_cvref_t<decltype(m_mapPvpNpcInfo)>>::put(w, m_mapPvpNpcInfo);
    }

    static KEGS_GAME_START_PVP_MATCH_NOT read_from(Reader& r) {
        KEGS_GAME_START_PVP_MATCH_NOT v;
        v.m_RoomInfo = Codec<std::remove_cvref_t<decltype(v.m_RoomInfo)>>::get(r);
        v.m_vecSlot = Codec<std::remove_cvref_t<decltype(v.m_vecSlot)>>::get(r);
        v.m_vecObserverSlot = Codec<std::remove_cvref_t<decltype(v.m_vecObserverSlot)>>::get(r);
        v.m_mapPvpNpcInfo = Codec<std::remove_cvref_t<decltype(v.m_mapPvpNpcInfo)>>::get(r);
        return v;
    }
};

struct KEGS_INTRUDE_START_NOT {
    char          m_RoomState{};
    std::int32_t  m_StartPosIndex{};  // int
    KRoomSlotInfo m_IntruderSlot{};
    bool          m_bObserver{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_RoomState)>>::put(w, m_RoomState);
        Codec<std::remove_cvref_t<decltype(m_StartPosIndex)>>::put(w, m_StartPosIndex);
        Codec<std::remove_cvref_t<decltype(m_IntruderSlot)>>::put(w, m_IntruderSlot);
        Codec<std::remove_cvref_t<decltype(m_bObserver)>>::put(w, m_bObserver);
    }

    static KEGS_INTRUDE_START_NOT read_from(Reader& r) {
        KEGS_INTRUDE_START_NOT v;
        v.m_RoomState = Codec<std::remove_cvref_t<decltype(v.m_RoomState)>>::get(r);
        v.m_StartPosIndex = Codec<std::remove_cvref_t<decltype(v.m_StartPosIndex)>>::get(r);
        v.m_IntruderSlot = Codec<std::remove_cvref_t<decltype(v.m_IntruderSlot)>>::get(r);
        v.m_bObserver = Codec<std::remove_cvref_t<decltype(v.m_bObserver)>>::get(r);
        return v;
    }
};

struct KEGS_JOIN_BATTLE_FIELD_ACK {
    std::int32_t               m_iOK{};  // int
    KBattleFieldJoinInfo       m_kBattleFieldJoinInfo{};
    KRoomInfo                  m_RoomInfo{};
    std::vector<KRoomSlotInfo> m_vecSlot{};  // std::vector< KRoomSlotInfo >
    std::u16string             m_wstrCNIP{};  // std::wstring
    std::int32_t               m_iRequireLevel{};  // int
    std::int32_t               m_iRequireDungeonID{};  // int
    std::u16string             m_wstrUDPRelayIP{};  // std::wstring
    std::uint16_t              m_usUDPRelayPort{};  // USHORT
    std::vector<std::int64_t>  m_vecEnterCashShopUser{};  // std::vector< UidType >
    std::int64_t               m_tRemainFieldHoldingTime{};  // __time64_t

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_kBattleFieldJoinInfo)>>::put(w, m_kBattleFieldJoinInfo);
        Codec<std::remove_cvref_t<decltype(m_RoomInfo)>>::put(w, m_RoomInfo);
        Codec<std::remove_cvref_t<decltype(m_vecSlot)>>::put(w, m_vecSlot);
        Codec<std::remove_cvref_t<decltype(m_wstrCNIP)>>::put(w, m_wstrCNIP);
        Codec<std::remove_cvref_t<decltype(m_iRequireLevel)>>::put(w, m_iRequireLevel);
        Codec<std::remove_cvref_t<decltype(m_iRequireDungeonID)>>::put(w, m_iRequireDungeonID);
        Codec<std::remove_cvref_t<decltype(m_wstrUDPRelayIP)>>::put(w, m_wstrUDPRelayIP);
        Codec<std::remove_cvref_t<decltype(m_usUDPRelayPort)>>::put(w, m_usUDPRelayPort);
        Codec<std::remove_cvref_t<decltype(m_vecEnterCashShopUser)>>::put(w, m_vecEnterCashShopUser);
        Codec<std::remove_cvref_t<decltype(m_tRemainFieldHoldingTime)>>::put(w, m_tRemainFieldHoldingTime);
    }

    static KEGS_JOIN_BATTLE_FIELD_ACK read_from(Reader& r) {
        KEGS_JOIN_BATTLE_FIELD_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_kBattleFieldJoinInfo = Codec<std::remove_cvref_t<decltype(v.m_kBattleFieldJoinInfo)>>::get(r);
        v.m_RoomInfo = Codec<std::remove_cvref_t<decltype(v.m_RoomInfo)>>::get(r);
        v.m_vecSlot = Codec<std::remove_cvref_t<decltype(v.m_vecSlot)>>::get(r);
        v.m_wstrCNIP = Codec<std::remove_cvref_t<decltype(v.m_wstrCNIP)>>::get(r);
        v.m_iRequireLevel = Codec<std::remove_cvref_t<decltype(v.m_iRequireLevel)>>::get(r);
        v.m_iRequireDungeonID = Codec<std::remove_cvref_t<decltype(v.m_iRequireDungeonID)>>::get(r);
        v.m_wstrUDPRelayIP = Codec<std::remove_cvref_t<decltype(v.m_wstrUDPRelayIP)>>::get(r);
        v.m_usUDPRelayPort = Codec<std::remove_cvref_t<decltype(v.m_usUDPRelayPort)>>::get(r);
        v.m_vecEnterCashShopUser = Codec<std::remove_cvref_t<decltype(v.m_vecEnterCashShopUser)>>::get(r);
        v.m_tRemainFieldHoldingTime = Codec<std::remove_cvref_t<decltype(v.m_tRemainFieldHoldingTime)>>::get(r);
        return v;
    }
};

struct KEGS_JOIN_ROOM_ACK {
    std::int32_t               m_iOK{};  // int
    KRoomInfo                  m_RoomInfo{};
    std::vector<KRoomSlotInfo> m_vecSlot{};  // std::vector< KRoomSlotInfo >
    std::vector<KRoomSlotInfo> m_vecObserverSlot{};  // std::vector< KRoomSlotInfo >
    std::u16string             m_wstrCNIP{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_RoomInfo)>>::put(w, m_RoomInfo);
        Codec<std::remove_cvref_t<decltype(m_vecSlot)>>::put(w, m_vecSlot);
        Codec<std::remove_cvref_t<decltype(m_vecObserverSlot)>>::put(w, m_vecObserverSlot);
        Codec<std::remove_cvref_t<decltype(m_wstrCNIP)>>::put(w, m_wstrCNIP);
    }

    static KEGS_JOIN_ROOM_ACK read_from(Reader& r) {
        KEGS_JOIN_ROOM_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_RoomInfo = Codec<std::remove_cvref_t<decltype(v.m_RoomInfo)>>::get(r);
        v.m_vecSlot = Codec<std::remove_cvref_t<decltype(v.m_vecSlot)>>::get(r);
        v.m_vecObserverSlot = Codec<std::remove_cvref_t<decltype(v.m_vecObserverSlot)>>::get(r);
        v.m_wstrCNIP = Codec<std::remove_cvref_t<decltype(v.m_wstrCNIP)>>::get(r);
        return v;
    }
};

struct KEGS_JOIN_ROOM_NOT {
    KRoomSlotInfo m_JoinSlot{};
    std::int32_t  m_StartPosIndex{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_JoinSlot)>>::put(w, m_JoinSlot);
        Codec<std::remove_cvref_t<decltype(m_StartPosIndex)>>::put(w, m_StartPosIndex);
    }

    static KEGS_JOIN_ROOM_NOT read_from(Reader& r) {
        KEGS_JOIN_ROOM_NOT v;
        v.m_JoinSlot = Codec<std::remove_cvref_t<decltype(v.m_JoinSlot)>>::get(r);
        v.m_StartPosIndex = Codec<std::remove_cvref_t<decltype(v.m_StartPosIndex)>>::get(r);
        return v;
    }
};

struct KEGS_LEAVE_BATTLE_FIELD_NOT {
    char                       m_cRoomState{};
    std::int64_t               m_iUnitUID{};  // UidType
    std::int64_t               m_iPartyUID{};  // UidType
    std::int32_t               m_iReason{};  // int
    std::vector<KRoomSlotInfo> m_vecSlot{};  // std::vector< KRoomSlotInfo >
    std::vector<KRoomSlotInfo> m_vecObserverSlot{};  // std::vector< KRoomSlotInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cRoomState)>>::put(w, m_cRoomState);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iPartyUID)>>::put(w, m_iPartyUID);
        Codec<std::remove_cvref_t<decltype(m_iReason)>>::put(w, m_iReason);
        Codec<std::remove_cvref_t<decltype(m_vecSlot)>>::put(w, m_vecSlot);
        Codec<std::remove_cvref_t<decltype(m_vecObserverSlot)>>::put(w, m_vecObserverSlot);
    }

    static KEGS_LEAVE_BATTLE_FIELD_NOT read_from(Reader& r) {
        KEGS_LEAVE_BATTLE_FIELD_NOT v;
        v.m_cRoomState = Codec<std::remove_cvref_t<decltype(v.m_cRoomState)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iPartyUID = Codec<std::remove_cvref_t<decltype(v.m_iPartyUID)>>::get(r);
        v.m_iReason = Codec<std::remove_cvref_t<decltype(v.m_iReason)>>::get(r);
        v.m_vecSlot = Codec<std::remove_cvref_t<decltype(v.m_vecSlot)>>::get(r);
        v.m_vecObserverSlot = Codec<std::remove_cvref_t<decltype(v.m_vecObserverSlot)>>::get(r);
        return v;
    }
};

struct KEGS_LEAVE_GAME_NOT {
    char                       m_cRoomState{};
    std::int64_t               m_iUnitUID{};  // UidType
    std::vector<KRoomSlotInfo> m_vecSlot{};  // std::vector< KRoomSlotInfo >
    std::vector<KRoomSlotInfo> m_vecObserverSlot{};  // std::vector< KRoomSlotInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cRoomState)>>::put(w, m_cRoomState);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_vecSlot)>>::put(w, m_vecSlot);
        Codec<std::remove_cvref_t<decltype(m_vecObserverSlot)>>::put(w, m_vecObserverSlot);
    }

    static KEGS_LEAVE_GAME_NOT read_from(Reader& r) {
        KEGS_LEAVE_GAME_NOT v;
        v.m_cRoomState = Codec<std::remove_cvref_t<decltype(v.m_cRoomState)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_vecSlot = Codec<std::remove_cvref_t<decltype(v.m_vecSlot)>>::get(r);
        v.m_vecObserverSlot = Codec<std::remove_cvref_t<decltype(v.m_vecObserverSlot)>>::get(r);
        return v;
    }
};

struct KEGS_LEAVE_ROOM_NOT {
    char                       m_cRoomState{};
    std::int64_t               m_iUnitUID{};  // UidType
    std::int32_t               m_iReason{};  // int
    std::vector<KRoomSlotInfo> m_vecSlot{};  // std::vector< KRoomSlotInfo >
    std::vector<KRoomSlotInfo> m_vecObserverSlot{};  // std::vector< KRoomSlotInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cRoomState)>>::put(w, m_cRoomState);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iReason)>>::put(w, m_iReason);
        Codec<std::remove_cvref_t<decltype(m_vecSlot)>>::put(w, m_vecSlot);
        Codec<std::remove_cvref_t<decltype(m_vecObserverSlot)>>::put(w, m_vecObserverSlot);
    }

    static KEGS_LEAVE_ROOM_NOT read_from(Reader& r) {
        KEGS_LEAVE_ROOM_NOT v;
        v.m_cRoomState = Codec<std::remove_cvref_t<decltype(v.m_cRoomState)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iReason = Codec<std::remove_cvref_t<decltype(v.m_iReason)>>::get(r);
        v.m_vecSlot = Codec<std::remove_cvref_t<decltype(v.m_vecSlot)>>::get(r);
        v.m_vecObserverSlot = Codec<std::remove_cvref_t<decltype(v.m_vecObserverSlot)>>::get(r);
        return v;
    }
};

struct KEGS_LEAVE_WEDDING_HALL_NOT {
    std::int32_t               m_iReason{};  // int
    char                       m_cRoomState{};
    std::int64_t               m_iUnitUID{};  // UidType
    std::vector<KRoomSlotInfo> m_vecSlot{};  // std::vector< KRoomSlotInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iReason)>>::put(w, m_iReason);
        Codec<std::remove_cvref_t<decltype(m_cRoomState)>>::put(w, m_cRoomState);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_vecSlot)>>::put(w, m_vecSlot);
    }

    static KEGS_LEAVE_WEDDING_HALL_NOT read_from(Reader& r) {
        KEGS_LEAVE_WEDDING_HALL_NOT v;
        v.m_iReason = Codec<std::remove_cvref_t<decltype(v.m_iReason)>>::get(r);
        v.m_cRoomState = Codec<std::remove_cvref_t<decltype(v.m_cRoomState)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_vecSlot = Codec<std::remove_cvref_t<decltype(v.m_vecSlot)>>::get(r);
        return v;
    }
};

struct KEGS_PARTY_GAME_START_NOT {
    std::int16_t                    m_sWorldID{};  // short
    KRoomInfo                       m_RoomInfo{};
    std::vector<KRoomSlotInfo>      m_vecSlot{};  // std::vector< KRoomSlotInfo >
    std::vector<KRoomSlotInfo>      m_vecObserverSlot{};  // std::vector< KRoomSlotInfo >
    std::u16string                  m_wstrCNIP{};  // std::wstring
    std::vector<KInventoryItemInfo> m_vecInventorySlotInfo{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_sWorldID)>>::put(w, m_sWorldID);
        Codec<std::remove_cvref_t<decltype(m_RoomInfo)>>::put(w, m_RoomInfo);
        Codec<std::remove_cvref_t<decltype(m_vecSlot)>>::put(w, m_vecSlot);
        Codec<std::remove_cvref_t<decltype(m_vecObserverSlot)>>::put(w, m_vecObserverSlot);
        Codec<std::remove_cvref_t<decltype(m_wstrCNIP)>>::put(w, m_wstrCNIP);
        Codec<std::remove_cvref_t<decltype(m_vecInventorySlotInfo)>>::put(w, m_vecInventorySlotInfo);
    }

    static KEGS_PARTY_GAME_START_NOT read_from(Reader& r) {
        KEGS_PARTY_GAME_START_NOT v;
        v.m_sWorldID = Codec<std::remove_cvref_t<decltype(v.m_sWorldID)>>::get(r);
        v.m_RoomInfo = Codec<std::remove_cvref_t<decltype(v.m_RoomInfo)>>::get(r);
        v.m_vecSlot = Codec<std::remove_cvref_t<decltype(v.m_vecSlot)>>::get(r);
        v.m_vecObserverSlot = Codec<std::remove_cvref_t<decltype(v.m_vecObserverSlot)>>::get(r);
        v.m_wstrCNIP = Codec<std::remove_cvref_t<decltype(v.m_wstrCNIP)>>::get(r);
        v.m_vecInventorySlotInfo = Codec<std::remove_cvref_t<decltype(v.m_vecInventorySlotInfo)>>::get(r);
        return v;
    }
};

struct KEGS_PLAY_START_NOT {
    char                       m_RoomState{};
    std::vector<KRoomSlotInfo> m_vecSlot{};  // std::vector< KRoomSlotInfo >
    std::int32_t               m_iDungeonID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_RoomState)>>::put(w, m_RoomState);
        Codec<std::remove_cvref_t<decltype(m_vecSlot)>>::put(w, m_vecSlot);
        Codec<std::remove_cvref_t<decltype(m_iDungeonID)>>::put(w, m_iDungeonID);
    }

    static KEGS_PLAY_START_NOT read_from(Reader& r) {
        KEGS_PLAY_START_NOT v;
        v.m_RoomState = Codec<std::remove_cvref_t<decltype(v.m_RoomState)>>::get(r);
        v.m_vecSlot = Codec<std::remove_cvref_t<decltype(v.m_vecSlot)>>::get(r);
        v.m_iDungeonID = Codec<std::remove_cvref_t<decltype(v.m_iDungeonID)>>::get(r);
        return v;
    }
};

struct KEGS_QUICK_JOIN_ACK {
    std::int32_t               m_iOK{};  // int
    KRoomInfo                  m_RoomInfo{};
    std::vector<KRoomSlotInfo> m_vecSlot{};  // std::vector< KRoomSlotInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_RoomInfo)>>::put(w, m_RoomInfo);
        Codec<std::remove_cvref_t<decltype(m_vecSlot)>>::put(w, m_vecSlot);
    }

    static KEGS_QUICK_JOIN_ACK read_from(Reader& r) {
        KEGS_QUICK_JOIN_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_RoomInfo = Codec<std::remove_cvref_t<decltype(v.m_RoomInfo)>>::get(r);
        v.m_vecSlot = Codec<std::remove_cvref_t<decltype(v.m_vecSlot)>>::get(r);
        return v;
    }
};

struct KEGS_REFRESH_ROOM_ACK {
    std::int32_t               m_iOK{};  // int
    KRoomInfo                  m_RoomInfo{};
    std::vector<KRoomSlotInfo> m_vecSlot{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_RoomInfo)>>::put(w, m_RoomInfo);
        Codec<std::remove_cvref_t<decltype(m_vecSlot)>>::put(w, m_vecSlot);
    }

    static KEGS_REFRESH_ROOM_ACK read_from(Reader& r) {
        KEGS_REFRESH_ROOM_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_RoomInfo = Codec<std::remove_cvref_t<decltype(v.m_RoomInfo)>>::get(r);
        v.m_vecSlot = Codec<std::remove_cvref_t<decltype(v.m_vecSlot)>>::get(r);
        return v;
    }
};

struct KEGS_RESULT_ALL_UNIT_SUCCESS_NOT {
    KRoomInfo                  m_RoomInfo{};
    std::vector<KRoomSlotInfo> m_vecSlot{};  // std::vector< KRoomSlotInfo >
    KReturnToFieldInfo         m_kReturnToFieldInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_RoomInfo)>>::put(w, m_RoomInfo);
        Codec<std::remove_cvref_t<decltype(m_vecSlot)>>::put(w, m_vecSlot);
        Codec<std::remove_cvref_t<decltype(m_kReturnToFieldInfo)>>::put(w, m_kReturnToFieldInfo);
    }

    static KEGS_RESULT_ALL_UNIT_SUCCESS_NOT read_from(Reader& r) {
        KEGS_RESULT_ALL_UNIT_SUCCESS_NOT v;
        v.m_RoomInfo = Codec<std::remove_cvref_t<decltype(v.m_RoomInfo)>>::get(r);
        v.m_vecSlot = Codec<std::remove_cvref_t<decltype(v.m_vecSlot)>>::get(r);
        v.m_kReturnToFieldInfo = Codec<std::remove_cvref_t<decltype(v.m_kReturnToFieldInfo)>>::get(r);
        return v;
    }
};

struct KEGS_STATE_CHANGE_GAME_INTRUDE_NOT {
    char          m_RoomState{};
    KRoomSlotInfo m_IntruderSlot{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_RoomState)>>::put(w, m_RoomState);
        Codec<std::remove_cvref_t<decltype(m_IntruderSlot)>>::put(w, m_IntruderSlot);
    }

    static KEGS_STATE_CHANGE_GAME_INTRUDE_NOT read_from(Reader& r) {
        KEGS_STATE_CHANGE_GAME_INTRUDE_NOT v;
        v.m_RoomState = Codec<std::remove_cvref_t<decltype(v.m_RoomState)>>::get(r);
        v.m_IntruderSlot = Codec<std::remove_cvref_t<decltype(v.m_IntruderSlot)>>::get(r);
        return v;
    }
};

struct KEGS_STATE_CHANGE_GAME_START_NOT {
    char                            m_cRoomState{};
    std::int32_t                    m_iGameType{};  // int
    std::int32_t                    m_iSpirit{};  // int
    std::int16_t                    m_sWorldID{};  // short
    std::vector<KRoomSlotInfo>      m_vecSlot{};  // std::vector< KRoomSlotInfo >
    std::vector<KInventoryItemInfo> m_vecInventorySlotInfo{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cRoomState)>>::put(w, m_cRoomState);
        Codec<std::remove_cvref_t<decltype(m_iGameType)>>::put(w, m_iGameType);
        Codec<std::remove_cvref_t<decltype(m_iSpirit)>>::put(w, m_iSpirit);
        Codec<std::remove_cvref_t<decltype(m_sWorldID)>>::put(w, m_sWorldID);
        Codec<std::remove_cvref_t<decltype(m_vecSlot)>>::put(w, m_vecSlot);
        Codec<std::remove_cvref_t<decltype(m_vecInventorySlotInfo)>>::put(w, m_vecInventorySlotInfo);
    }

    static KEGS_STATE_CHANGE_GAME_START_NOT read_from(Reader& r) {
        KEGS_STATE_CHANGE_GAME_START_NOT v;
        v.m_cRoomState = Codec<std::remove_cvref_t<decltype(v.m_cRoomState)>>::get(r);
        v.m_iGameType = Codec<std::remove_cvref_t<decltype(v.m_iGameType)>>::get(r);
        v.m_iSpirit = Codec<std::remove_cvref_t<decltype(v.m_iSpirit)>>::get(r);
        v.m_sWorldID = Codec<std::remove_cvref_t<decltype(v.m_sWorldID)>>::get(r);
        v.m_vecSlot = Codec<std::remove_cvref_t<decltype(v.m_vecSlot)>>::get(r);
        v.m_vecInventorySlotInfo = Codec<std::remove_cvref_t<decltype(v.m_vecInventorySlotInfo)>>::get(r);
        return v;
    }
};

struct KEGS_STATE_CHANGE_RESULT_NOT {
    char                       m_cRoomState{};
    std::vector<KRoomSlotInfo> m_vecSlot{};  // std::vector< KRoomSlotInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cRoomState)>>::put(w, m_cRoomState);
        Codec<std::remove_cvref_t<decltype(m_vecSlot)>>::put(w, m_vecSlot);
    }

    static KEGS_STATE_CHANGE_RESULT_NOT read_from(Reader& r) {
        KEGS_STATE_CHANGE_RESULT_NOT v;
        v.m_cRoomState = Codec<std::remove_cvref_t<decltype(v.m_cRoomState)>>::get(r);
        v.m_vecSlot = Codec<std::remove_cvref_t<decltype(v.m_vecSlot)>>::get(r);
        return v;
    }
};

struct KERM_GAME_START_NOT {
    char                       m_cRoomState{};
    std::int32_t               m_iGameType{};  // int
    std::vector<KRoomSlotInfo> m_vecSlot{};  // std::vector< KRoomSlotInfo >
    std::int32_t               m_iDungeonID{};  // int
    std::int16_t               m_sWorldID{};  // short

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cRoomState)>>::put(w, m_cRoomState);
        Codec<std::remove_cvref_t<decltype(m_iGameType)>>::put(w, m_iGameType);
        Codec<std::remove_cvref_t<decltype(m_vecSlot)>>::put(w, m_vecSlot);
        Codec<std::remove_cvref_t<decltype(m_iDungeonID)>>::put(w, m_iDungeonID);
        Codec<std::remove_cvref_t<decltype(m_sWorldID)>>::put(w, m_sWorldID);
    }

    static KERM_GAME_START_NOT read_from(Reader& r) {
        KERM_GAME_START_NOT v;
        v.m_cRoomState = Codec<std::remove_cvref_t<decltype(v.m_cRoomState)>>::get(r);
        v.m_iGameType = Codec<std::remove_cvref_t<decltype(v.m_iGameType)>>::get(r);
        v.m_vecSlot = Codec<std::remove_cvref_t<decltype(v.m_vecSlot)>>::get(r);
        v.m_iDungeonID = Codec<std::remove_cvref_t<decltype(v.m_iDungeonID)>>::get(r);
        v.m_sWorldID = Codec<std::remove_cvref_t<decltype(v.m_sWorldID)>>::get(r);
        return v;
    }
};

struct KERM_JOIN_BATTLE_FIELD_ACK {
    std::int32_t               m_iOK{};  // int
    KBattleFieldJoinInfo       m_kBattleFieldJoinInfo{};
    KRoomInfo                  m_RoomInfo{};
    std::vector<KRoomSlotInfo> m_vecSlot{};  // std::vector< KRoomSlotInfo >
    std::u16string             m_wstrCNIP{};  // std::wstring
    std::int32_t               m_iRequireLevel{};  // int
    std::int32_t               m_iRequireDungeonID{};  // int
    std::u16string             m_wstrUDPRelayIP{};  // std::wstring
    std::uint16_t              m_usUDPRelayPort{};  // USHORT
    std::vector<std::int64_t>  m_vecEnterCashShopUser{};  // std::vector< UidType >
    std::int64_t               m_tRemainFieldHoldingTime{};  // __time64_t
    KBossFieldJoinInfo         m_kBossFieldJoinInfo{};
    bool                       m_bBossField{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_kBattleFieldJoinInfo)>>::put(w, m_kBattleFieldJoinInfo);
        Codec<std::remove_cvref_t<decltype(m_RoomInfo)>>::put(w, m_RoomInfo);
        Codec<std::remove_cvref_t<decltype(m_vecSlot)>>::put(w, m_vecSlot);
        Codec<std::remove_cvref_t<decltype(m_wstrCNIP)>>::put(w, m_wstrCNIP);
        Codec<std::remove_cvref_t<decltype(m_iRequireLevel)>>::put(w, m_iRequireLevel);
        Codec<std::remove_cvref_t<decltype(m_iRequireDungeonID)>>::put(w, m_iRequireDungeonID);
        Codec<std::remove_cvref_t<decltype(m_wstrUDPRelayIP)>>::put(w, m_wstrUDPRelayIP);
        Codec<std::remove_cvref_t<decltype(m_usUDPRelayPort)>>::put(w, m_usUDPRelayPort);
        Codec<std::remove_cvref_t<decltype(m_vecEnterCashShopUser)>>::put(w, m_vecEnterCashShopUser);
        Codec<std::remove_cvref_t<decltype(m_tRemainFieldHoldingTime)>>::put(w, m_tRemainFieldHoldingTime);
        Codec<std::remove_cvref_t<decltype(m_kBossFieldJoinInfo)>>::put(w, m_kBossFieldJoinInfo);
        Codec<std::remove_cvref_t<decltype(m_bBossField)>>::put(w, m_bBossField);
    }

    static KERM_JOIN_BATTLE_FIELD_ACK read_from(Reader& r) {
        KERM_JOIN_BATTLE_FIELD_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_kBattleFieldJoinInfo = Codec<std::remove_cvref_t<decltype(v.m_kBattleFieldJoinInfo)>>::get(r);
        v.m_RoomInfo = Codec<std::remove_cvref_t<decltype(v.m_RoomInfo)>>::get(r);
        v.m_vecSlot = Codec<std::remove_cvref_t<decltype(v.m_vecSlot)>>::get(r);
        v.m_wstrCNIP = Codec<std::remove_cvref_t<decltype(v.m_wstrCNIP)>>::get(r);
        v.m_iRequireLevel = Codec<std::remove_cvref_t<decltype(v.m_iRequireLevel)>>::get(r);
        v.m_iRequireDungeonID = Codec<std::remove_cvref_t<decltype(v.m_iRequireDungeonID)>>::get(r);
        v.m_wstrUDPRelayIP = Codec<std::remove_cvref_t<decltype(v.m_wstrUDPRelayIP)>>::get(r);
        v.m_usUDPRelayPort = Codec<std::remove_cvref_t<decltype(v.m_usUDPRelayPort)>>::get(r);
        v.m_vecEnterCashShopUser = Codec<std::remove_cvref_t<decltype(v.m_vecEnterCashShopUser)>>::get(r);
        v.m_tRemainFieldHoldingTime = Codec<std::remove_cvref_t<decltype(v.m_tRemainFieldHoldingTime)>>::get(r);
        v.m_kBossFieldJoinInfo = Codec<std::remove_cvref_t<decltype(v.m_kBossFieldJoinInfo)>>::get(r);
        v.m_bBossField = Codec<std::remove_cvref_t<decltype(v.m_bBossField)>>::get(r);
        return v;
    }
};

struct KERM_OPEN_BATTLE_FIELD_ACK {
    std::int32_t               m_iOK{};  // int
    KRoomInfo                  m_RoomInfo{};
    std::vector<KRoomSlotInfo> m_vecSlot{};  // std::vector< KRoomSlotInfo >
    std::u16string             m_wstrCNIP{};  // std::wstring
    KBattleFieldJoinInfo       m_kBattleFieldJoinInfo{};
    std::u16string             m_wstrUDPRelayIP{};  // std::wstring
    std::uint16_t              m_usUDPRelayPort{};  // USHORT
    std::int64_t               m_tRemainFieldHoldingTime{};  // __time64_t
    KBossFieldJoinInfo         m_kBossFieldJoinInfo{};
    bool                       m_bBossField{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_RoomInfo)>>::put(w, m_RoomInfo);
        Codec<std::remove_cvref_t<decltype(m_vecSlot)>>::put(w, m_vecSlot);
        Codec<std::remove_cvref_t<decltype(m_wstrCNIP)>>::put(w, m_wstrCNIP);
        Codec<std::remove_cvref_t<decltype(m_kBattleFieldJoinInfo)>>::put(w, m_kBattleFieldJoinInfo);
        Codec<std::remove_cvref_t<decltype(m_wstrUDPRelayIP)>>::put(w, m_wstrUDPRelayIP);
        Codec<std::remove_cvref_t<decltype(m_usUDPRelayPort)>>::put(w, m_usUDPRelayPort);
        Codec<std::remove_cvref_t<decltype(m_tRemainFieldHoldingTime)>>::put(w, m_tRemainFieldHoldingTime);
        Codec<std::remove_cvref_t<decltype(m_kBossFieldJoinInfo)>>::put(w, m_kBossFieldJoinInfo);
        Codec<std::remove_cvref_t<decltype(m_bBossField)>>::put(w, m_bBossField);
    }

    static KERM_OPEN_BATTLE_FIELD_ACK read_from(Reader& r) {
        KERM_OPEN_BATTLE_FIELD_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_RoomInfo = Codec<std::remove_cvref_t<decltype(v.m_RoomInfo)>>::get(r);
        v.m_vecSlot = Codec<std::remove_cvref_t<decltype(v.m_vecSlot)>>::get(r);
        v.m_wstrCNIP = Codec<std::remove_cvref_t<decltype(v.m_wstrCNIP)>>::get(r);
        v.m_kBattleFieldJoinInfo = Codec<std::remove_cvref_t<decltype(v.m_kBattleFieldJoinInfo)>>::get(r);
        v.m_wstrUDPRelayIP = Codec<std::remove_cvref_t<decltype(v.m_wstrUDPRelayIP)>>::get(r);
        v.m_usUDPRelayPort = Codec<std::remove_cvref_t<decltype(v.m_usUDPRelayPort)>>::get(r);
        v.m_tRemainFieldHoldingTime = Codec<std::remove_cvref_t<decltype(v.m_tRemainFieldHoldingTime)>>::get(r);
        v.m_kBossFieldJoinInfo = Codec<std::remove_cvref_t<decltype(v.m_kBossFieldJoinInfo)>>::get(r);
        v.m_bBossField = Codec<std::remove_cvref_t<decltype(v.m_bBossField)>>::get(r);
        return v;
    }
};

struct KERM_PARTY_OPEN_DUNGEON_ROOM_NOT {
    std::int16_t                                   m_sWorldID{};  // short
    KRoomInfo                                      m_RoomInfo{};
    std::vector<KRoomSlotInfo>                     m_vecSlot{};  // std::vector< KRoomSlotInfo >
    std::vector<KRoomSlotInfo>                     m_vecObserverSlot{};  // std::vector< KRoomSlotInfo >
    std::u16string                                 m_wstrCNIP{};  // std::wstring
    std::vector<KInventoryItemInfo>                m_vecInventorySlotInfo{};  // std::vector< KInventoryItemInfo >
    std::map<std::int64_t, std::vector<KBuffInfo>> m_mapActivateBuff{};  // std::map< UidType, std::vector< KBuffInfo > >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_sWorldID)>>::put(w, m_sWorldID);
        Codec<std::remove_cvref_t<decltype(m_RoomInfo)>>::put(w, m_RoomInfo);
        Codec<std::remove_cvref_t<decltype(m_vecSlot)>>::put(w, m_vecSlot);
        Codec<std::remove_cvref_t<decltype(m_vecObserverSlot)>>::put(w, m_vecObserverSlot);
        Codec<std::remove_cvref_t<decltype(m_wstrCNIP)>>::put(w, m_wstrCNIP);
        Codec<std::remove_cvref_t<decltype(m_vecInventorySlotInfo)>>::put(w, m_vecInventorySlotInfo);
        Codec<std::remove_cvref_t<decltype(m_mapActivateBuff)>>::put(w, m_mapActivateBuff);
    }

    static KERM_PARTY_OPEN_DUNGEON_ROOM_NOT read_from(Reader& r) {
        KERM_PARTY_OPEN_DUNGEON_ROOM_NOT v;
        v.m_sWorldID = Codec<std::remove_cvref_t<decltype(v.m_sWorldID)>>::get(r);
        v.m_RoomInfo = Codec<std::remove_cvref_t<decltype(v.m_RoomInfo)>>::get(r);
        v.m_vecSlot = Codec<std::remove_cvref_t<decltype(v.m_vecSlot)>>::get(r);
        v.m_vecObserverSlot = Codec<std::remove_cvref_t<decltype(v.m_vecObserverSlot)>>::get(r);
        v.m_wstrCNIP = Codec<std::remove_cvref_t<decltype(v.m_wstrCNIP)>>::get(r);
        v.m_vecInventorySlotInfo = Codec<std::remove_cvref_t<decltype(v.m_vecInventorySlotInfo)>>::get(r);
        v.m_mapActivateBuff = Codec<std::remove_cvref_t<decltype(v.m_mapActivateBuff)>>::get(r);
        return v;
    }
};

struct KUpdateCCUInfo {
    std::int64_t m_iServerUID{};  // UidType
    std::int32_t m_iConcurrentUser{};  // int
    std::int32_t m_iCurPartyCount{};  // int
    std::int32_t m_iPlayGamePartyCount{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iServerUID)>>::put(w, m_iServerUID);
        Codec<std::remove_cvref_t<decltype(m_iConcurrentUser)>>::put(w, m_iConcurrentUser);
        Codec<std::remove_cvref_t<decltype(m_iCurPartyCount)>>::put(w, m_iCurPartyCount);
        Codec<std::remove_cvref_t<decltype(m_iPlayGamePartyCount)>>::put(w, m_iPlayGamePartyCount);
    }

    static KUpdateCCUInfo read_from(Reader& r) {
        KUpdateCCUInfo v;
        v.m_iServerUID = Codec<std::remove_cvref_t<decltype(v.m_iServerUID)>>::get(r);
        v.m_iConcurrentUser = Codec<std::remove_cvref_t<decltype(v.m_iConcurrentUser)>>::get(r);
        v.m_iCurPartyCount = Codec<std::remove_cvref_t<decltype(v.m_iCurPartyCount)>>::get(r);
        v.m_iPlayGamePartyCount = Codec<std::remove_cvref_t<decltype(v.m_iPlayGamePartyCount)>>::get(r);
        return v;
    }
};

struct KDBE_UPDATE_CCU_INFO_NOT {
    std::u16string              m_wstrRegDate{};  // std::wstring
    std::vector<KUpdateCCUInfo> m_vecCCUInfo{};  // std::vector< KUpdateCCUInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrRegDate)>>::put(w, m_wstrRegDate);
        Codec<std::remove_cvref_t<decltype(m_vecCCUInfo)>>::put(w, m_vecCCUInfo);
    }

    static KDBE_UPDATE_CCU_INFO_NOT read_from(Reader& r) {
        KDBE_UPDATE_CCU_INFO_NOT v;
        v.m_wstrRegDate = Codec<std::remove_cvref_t<decltype(v.m_wstrRegDate)>>::get(r);
        v.m_vecCCUInfo = Codec<std::remove_cvref_t<decltype(v.m_vecCCUInfo)>>::get(r);
        return v;
    }
};

struct KELG_UPDATE_CCU_INFO_NOT {
    KUpdateCCUInfo m_kCCUInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kCCUInfo)>>::put(w, m_kCCUInfo);
    }

    static KELG_UPDATE_CCU_INFO_NOT read_from(Reader& r) {
        KELG_UPDATE_CCU_INFO_NOT v;
        v.m_kCCUInfo = Codec<std::remove_cvref_t<decltype(v.m_kCCUInfo)>>::get(r);
        return v;
    }
};

struct KUpdateShareItemInfo {
    std::int64_t m_iItemUID{};  // UidType
    std::int64_t m_iUnitUID{};  // UidType
    std::int32_t m_iUsageType{};  // int
    std::int32_t m_iValue{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iUsageType)>>::put(w, m_iUsageType);
        Codec<std::remove_cvref_t<decltype(m_iValue)>>::put(w, m_iValue);
    }

    static KUpdateShareItemInfo read_from(Reader& r) {
        KUpdateShareItemInfo v;
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iUsageType = Codec<std::remove_cvref_t<decltype(v.m_iUsageType)>>::get(r);
        v.m_iValue = Codec<std::remove_cvref_t<decltype(v.m_iValue)>>::get(r);
        return v;
    }
};

struct KDBE_UPDATE_SHARE_ITEM_REQ {
    std::int64_t                                 m_iUnitUID{};  // UidType
    std::map<std::int64_t, KItemPosition>        m_mapUpdatePosition{};  // KItemPositionUpdate
    std::map<std::int64_t, KTradeShareItemInfo>  m_mapInItem{};  // std::map< UidType, KTradeShareItemInfo >
    std::map<std::int64_t, KTradeShareItemInfo>  m_mapOutItem{};  // std::map< UidType, KTradeShareItemInfo >
    std::map<std::int64_t, KUpdateShareItemInfo> m_mapUpdateEndurance{};  // std::map< UidType, KUpdateShareItemInfo >
    std::map<std::int64_t, KUpdateShareItemInfo> m_mapUpdateQuantity{};  // std::map< UidType, KUpdateShareItemInfo >
    std::vector<std::int64_t>                    m_vecDeletedItem{};  // std::vector< UidType >
    bool                                         m_bReload{};
    std::u16string                               m_wstrReloadNickname{};  // std::wstring
    std::vector<KInventoryItemInfo>              m_vecInventorySlotItem{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_mapUpdatePosition)>>::put(w, m_mapUpdatePosition);
        Codec<std::remove_cvref_t<decltype(m_mapInItem)>>::put(w, m_mapInItem);
        Codec<std::remove_cvref_t<decltype(m_mapOutItem)>>::put(w, m_mapOutItem);
        Codec<std::remove_cvref_t<decltype(m_mapUpdateEndurance)>>::put(w, m_mapUpdateEndurance);
        Codec<std::remove_cvref_t<decltype(m_mapUpdateQuantity)>>::put(w, m_mapUpdateQuantity);
        Codec<std::remove_cvref_t<decltype(m_vecDeletedItem)>>::put(w, m_vecDeletedItem);
        Codec<std::remove_cvref_t<decltype(m_bReload)>>::put(w, m_bReload);
        Codec<std::remove_cvref_t<decltype(m_wstrReloadNickname)>>::put(w, m_wstrReloadNickname);
        Codec<std::remove_cvref_t<decltype(m_vecInventorySlotItem)>>::put(w, m_vecInventorySlotItem);
    }

    static KDBE_UPDATE_SHARE_ITEM_REQ read_from(Reader& r) {
        KDBE_UPDATE_SHARE_ITEM_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_mapUpdatePosition = Codec<std::remove_cvref_t<decltype(v.m_mapUpdatePosition)>>::get(r);
        v.m_mapInItem = Codec<std::remove_cvref_t<decltype(v.m_mapInItem)>>::get(r);
        v.m_mapOutItem = Codec<std::remove_cvref_t<decltype(v.m_mapOutItem)>>::get(r);
        v.m_mapUpdateEndurance = Codec<std::remove_cvref_t<decltype(v.m_mapUpdateEndurance)>>::get(r);
        v.m_mapUpdateQuantity = Codec<std::remove_cvref_t<decltype(v.m_mapUpdateQuantity)>>::get(r);
        v.m_vecDeletedItem = Codec<std::remove_cvref_t<decltype(v.m_vecDeletedItem)>>::get(r);
        v.m_bReload = Codec<std::remove_cvref_t<decltype(v.m_bReload)>>::get(r);
        v.m_wstrReloadNickname = Codec<std::remove_cvref_t<decltype(v.m_wstrReloadNickname)>>::get(r);
        v.m_vecInventorySlotItem = Codec<std::remove_cvref_t<decltype(v.m_vecInventorySlotItem)>>::get(r);
        return v;
    }
};

struct KUserAuthenticateReq {
    bool           m_bDebugAuth{};
    std::u16string m_wstrUserID{};  // std::wstring
    std::u16string m_wstrPasswd{};  // std::wstring
    std::u16string m_wstrHWID{};  // std::wstring
    std::string    m_strMachineID{};
    std::int32_t   m_iChannelingCode{};  // int
    bool           m_bManualLogin{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bDebugAuth)>>::put(w, m_bDebugAuth);
        Codec<std::remove_cvref_t<decltype(m_wstrUserID)>>::put(w, m_wstrUserID);
        Codec<std::remove_cvref_t<decltype(m_wstrPasswd)>>::put(w, m_wstrPasswd);
        Codec<std::remove_cvref_t<decltype(m_wstrHWID)>>::put(w, m_wstrHWID);
        Codec<std::remove_cvref_t<decltype(m_strMachineID)>>::put(w, m_strMachineID);
        Codec<std::remove_cvref_t<decltype(m_iChannelingCode)>>::put(w, m_iChannelingCode);
        Codec<std::remove_cvref_t<decltype(m_bManualLogin)>>::put(w, m_bManualLogin);
    }

    static KUserAuthenticateReq read_from(Reader& r) {
        KUserAuthenticateReq v;
        v.m_bDebugAuth = Codec<std::remove_cvref_t<decltype(v.m_bDebugAuth)>>::get(r);
        v.m_wstrUserID = Codec<std::remove_cvref_t<decltype(v.m_wstrUserID)>>::get(r);
        v.m_wstrPasswd = Codec<std::remove_cvref_t<decltype(v.m_wstrPasswd)>>::get(r);
        v.m_wstrHWID = Codec<std::remove_cvref_t<decltype(v.m_wstrHWID)>>::get(r);
        v.m_strMachineID = Codec<std::remove_cvref_t<decltype(v.m_strMachineID)>>::get(r);
        v.m_iChannelingCode = Codec<std::remove_cvref_t<decltype(v.m_iChannelingCode)>>::get(r);
        v.m_bManualLogin = Codec<std::remove_cvref_t<decltype(v.m_bManualLogin)>>::get(r);
        return v;
    }
};

struct KEGS_CONNECT_CHANNEL_CHANGE_REQ {
    KEGS_CONNECT_REQ     m_kConnectReq{};
    KUserAuthenticateReq m_kVerifyAccountReq{};  // KEGS_VERIFY_ACCOUNT_REQ
    KEGS_SELECT_UNIT_REQ m_kSelectUnitReq{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kConnectReq)>>::put(w, m_kConnectReq);
        Codec<std::remove_cvref_t<decltype(m_kVerifyAccountReq)>>::put(w, m_kVerifyAccountReq);
        Codec<std::remove_cvref_t<decltype(m_kSelectUnitReq)>>::put(w, m_kSelectUnitReq);
    }

    static KEGS_CONNECT_CHANNEL_CHANGE_REQ read_from(Reader& r) {
        KEGS_CONNECT_CHANNEL_CHANGE_REQ v;
        v.m_kConnectReq = Codec<std::remove_cvref_t<decltype(v.m_kConnectReq)>>::get(r);
        v.m_kVerifyAccountReq = Codec<std::remove_cvref_t<decltype(v.m_kVerifyAccountReq)>>::get(r);
        v.m_kSelectUnitReq = Codec<std::remove_cvref_t<decltype(v.m_kSelectUnitReq)>>::get(r);
        return v;
    }
};

struct KELG_GAME_USER_AUTHENTICATE_REQ {
    KUserAuthenticateReq m_kAuthenticateReq{};
    KNexonAccountInfo    m_kNexonAccountInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kAuthenticateReq)>>::put(w, m_kAuthenticateReq);
        Codec<std::remove_cvref_t<decltype(m_kNexonAccountInfo)>>::put(w, m_kNexonAccountInfo);
    }

    static KELG_GAME_USER_AUTHENTICATE_REQ read_from(Reader& r) {
        KELG_GAME_USER_AUTHENTICATE_REQ v;
        v.m_kAuthenticateReq = Codec<std::remove_cvref_t<decltype(v.m_kAuthenticateReq)>>::get(r);
        v.m_kNexonAccountInfo = Codec<std::remove_cvref_t<decltype(v.m_kNexonAccountInfo)>>::get(r);
        return v;
    }
};

struct KELG_NEXON_USER_AUTHENTICATE_REQ {
    KUserAuthenticateReq m_kAuthenticateReq{};
    KNexonAccountInfo    m_kNexonAccountInfo{};
    bool                 m_bServerUseKogOTP{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kAuthenticateReq)>>::put(w, m_kAuthenticateReq);
        Codec<std::remove_cvref_t<decltype(m_kNexonAccountInfo)>>::put(w, m_kNexonAccountInfo);
        Codec<std::remove_cvref_t<decltype(m_bServerUseKogOTP)>>::put(w, m_bServerUseKogOTP);
    }

    static KELG_NEXON_USER_AUTHENTICATE_REQ read_from(Reader& r) {
        KELG_NEXON_USER_AUTHENTICATE_REQ v;
        v.m_kAuthenticateReq = Codec<std::remove_cvref_t<decltype(v.m_kAuthenticateReq)>>::get(r);
        v.m_kNexonAccountInfo = Codec<std::remove_cvref_t<decltype(v.m_kNexonAccountInfo)>>::get(r);
        v.m_bServerUseKogOTP = Codec<std::remove_cvref_t<decltype(v.m_bServerUseKogOTP)>>::get(r);
        return v;
    }
};

struct KUserConnectTimeEventInfo {
    std::int32_t   m_iEventID{};  // int
    float          m_fEventTime{};
    std::int64_t   m_kCTime{};  // __int64
    std::u16string m_wstrEventName{};  // std::wstring
    bool           m_bInit{};
    bool           m_bGiveOnce{};
    bool           m_bAccountEvent{};
    double         m_fBackUpTime{};
    bool           m_bRepeatRewardEvent{};
    std::int32_t   m_iBeforeEventID{};  // int
    std::int32_t   m_iEventItemID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iEventID)>>::put(w, m_iEventID);
        Codec<std::remove_cvref_t<decltype(m_fEventTime)>>::put(w, m_fEventTime);
        Codec<std::remove_cvref_t<decltype(m_kCTime)>>::put(w, m_kCTime);
        Codec<std::remove_cvref_t<decltype(m_wstrEventName)>>::put(w, m_wstrEventName);
        Codec<std::remove_cvref_t<decltype(m_bInit)>>::put(w, m_bInit);
        Codec<std::remove_cvref_t<decltype(m_bGiveOnce)>>::put(w, m_bGiveOnce);
        Codec<std::remove_cvref_t<decltype(m_bAccountEvent)>>::put(w, m_bAccountEvent);
        Codec<std::remove_cvref_t<decltype(m_fBackUpTime)>>::put(w, m_fBackUpTime);
        Codec<std::remove_cvref_t<decltype(m_bRepeatRewardEvent)>>::put(w, m_bRepeatRewardEvent);
        Codec<std::remove_cvref_t<decltype(m_iBeforeEventID)>>::put(w, m_iBeforeEventID);
        Codec<std::remove_cvref_t<decltype(m_iEventItemID)>>::put(w, m_iEventItemID);
    }

    static KUserConnectTimeEventInfo read_from(Reader& r) {
        KUserConnectTimeEventInfo v;
        v.m_iEventID = Codec<std::remove_cvref_t<decltype(v.m_iEventID)>>::get(r);
        v.m_fEventTime = Codec<std::remove_cvref_t<decltype(v.m_fEventTime)>>::get(r);
        v.m_kCTime = Codec<std::remove_cvref_t<decltype(v.m_kCTime)>>::get(r);
        v.m_wstrEventName = Codec<std::remove_cvref_t<decltype(v.m_wstrEventName)>>::get(r);
        v.m_bInit = Codec<std::remove_cvref_t<decltype(v.m_bInit)>>::get(r);
        v.m_bGiveOnce = Codec<std::remove_cvref_t<decltype(v.m_bGiveOnce)>>::get(r);
        v.m_bAccountEvent = Codec<std::remove_cvref_t<decltype(v.m_bAccountEvent)>>::get(r);
        v.m_fBackUpTime = Codec<std::remove_cvref_t<decltype(v.m_fBackUpTime)>>::get(r);
        v.m_bRepeatRewardEvent = Codec<std::remove_cvref_t<decltype(v.m_bRepeatRewardEvent)>>::get(r);
        v.m_iBeforeEventID = Codec<std::remove_cvref_t<decltype(v.m_iBeforeEventID)>>::get(r);
        v.m_iEventItemID = Codec<std::remove_cvref_t<decltype(v.m_iEventItemID)>>::get(r);
        return v;
    }
};

struct KChannelChangeInfo {
    std::map<std::int32_t, KUserConnectTimeEventInfo> m_mapEventDurationTime{};  // std::map< int, KUserConnectTimeEventInfo >
    std::int32_t                                      m_iEquippedTitleID{};  // int
    bool                                              m_bIsPcBang{};
    std::int32_t                                      m_iPcBangType{};  // int
    std::vector<KInventoryItemInfo>                   m_vecPcBangItemInfo{};  // std::vector< KInventoryItemInfo >
    std::int32_t                                      m_nUnitSlot{};  // int
    std::int64_t                                      m_iSummonedRidingPetUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_mapEventDurationTime)>>::put(w, m_mapEventDurationTime);
        Codec<std::remove_cvref_t<decltype(m_iEquippedTitleID)>>::put(w, m_iEquippedTitleID);
        Codec<std::remove_cvref_t<decltype(m_bIsPcBang)>>::put(w, m_bIsPcBang);
        Codec<std::remove_cvref_t<decltype(m_iPcBangType)>>::put(w, m_iPcBangType);
        Codec<std::remove_cvref_t<decltype(m_vecPcBangItemInfo)>>::put(w, m_vecPcBangItemInfo);
        Codec<std::remove_cvref_t<decltype(m_nUnitSlot)>>::put(w, m_nUnitSlot);
        Codec<std::remove_cvref_t<decltype(m_iSummonedRidingPetUID)>>::put(w, m_iSummonedRidingPetUID);
    }

    static KChannelChangeInfo read_from(Reader& r) {
        KChannelChangeInfo v;
        v.m_mapEventDurationTime = Codec<std::remove_cvref_t<decltype(v.m_mapEventDurationTime)>>::get(r);
        v.m_iEquippedTitleID = Codec<std::remove_cvref_t<decltype(v.m_iEquippedTitleID)>>::get(r);
        v.m_bIsPcBang = Codec<std::remove_cvref_t<decltype(v.m_bIsPcBang)>>::get(r);
        v.m_iPcBangType = Codec<std::remove_cvref_t<decltype(v.m_iPcBangType)>>::get(r);
        v.m_vecPcBangItemInfo = Codec<std::remove_cvref_t<decltype(v.m_vecPcBangItemInfo)>>::get(r);
        v.m_nUnitSlot = Codec<std::remove_cvref_t<decltype(v.m_nUnitSlot)>>::get(r);
        v.m_iSummonedRidingPetUID = Codec<std::remove_cvref_t<decltype(v.m_iSummonedRidingPetUID)>>::get(r);
        return v;
    }
};

struct KDBE_CHANNEL_CHANGE_GAME_SELECT_UNIT_REQ {
    KEGS_CONNECT_ACK        m_kConnectAck{};
    KEGS_VERIFY_ACCOUNT_ACK m_kVerifyAccountAck{};
    KEGS_SELECT_UNIT_REQ    m_kSelectUnitReq{};
    KChannelChangeInfo      m_kChangeUserInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kConnectAck)>>::put(w, m_kConnectAck);
        Codec<std::remove_cvref_t<decltype(m_kVerifyAccountAck)>>::put(w, m_kVerifyAccountAck);
        Codec<std::remove_cvref_t<decltype(m_kSelectUnitReq)>>::put(w, m_kSelectUnitReq);
        Codec<std::remove_cvref_t<decltype(m_kChangeUserInfo)>>::put(w, m_kChangeUserInfo);
    }

    static KDBE_CHANNEL_CHANGE_GAME_SELECT_UNIT_REQ read_from(Reader& r) {
        KDBE_CHANNEL_CHANGE_GAME_SELECT_UNIT_REQ v;
        v.m_kConnectAck = Codec<std::remove_cvref_t<decltype(v.m_kConnectAck)>>::get(r);
        v.m_kVerifyAccountAck = Codec<std::remove_cvref_t<decltype(v.m_kVerifyAccountAck)>>::get(r);
        v.m_kSelectUnitReq = Codec<std::remove_cvref_t<decltype(v.m_kSelectUnitReq)>>::get(r);
        v.m_kChangeUserInfo = Codec<std::remove_cvref_t<decltype(v.m_kChangeUserInfo)>>::get(r);
        return v;
    }
};

struct KECN_CHECK_CHANNEL_CHANGE_REQ {
    std::int64_t       m_iSourGSUID{};  // UidType
    std::int64_t       m_iDestGSUID{};  // UidType
    std::int64_t       m_iUserUID{};  // UidType
    std::int64_t       m_iCharUID{};  // UidType
    KChannelChangeInfo m_kChangeUserInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iSourGSUID)>>::put(w, m_iSourGSUID);
        Codec<std::remove_cvref_t<decltype(m_iDestGSUID)>>::put(w, m_iDestGSUID);
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
        Codec<std::remove_cvref_t<decltype(m_iCharUID)>>::put(w, m_iCharUID);
        Codec<std::remove_cvref_t<decltype(m_kChangeUserInfo)>>::put(w, m_kChangeUserInfo);
    }

    static KECN_CHECK_CHANNEL_CHANGE_REQ read_from(Reader& r) {
        KECN_CHECK_CHANNEL_CHANGE_REQ v;
        v.m_iSourGSUID = Codec<std::remove_cvref_t<decltype(v.m_iSourGSUID)>>::get(r);
        v.m_iDestGSUID = Codec<std::remove_cvref_t<decltype(v.m_iDestGSUID)>>::get(r);
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        v.m_iCharUID = Codec<std::remove_cvref_t<decltype(v.m_iCharUID)>>::get(r);
        v.m_kChangeUserInfo = Codec<std::remove_cvref_t<decltype(v.m_kChangeUserInfo)>>::get(r);
        return v;
    }
};

struct KEGS_CONNECT_CHANNEL_CHANGE_ACK {
    std::int32_t            m_iOK{};  // int
    KEGS_CONNECT_ACK        m_kConnectAck{};
    KEGS_VERIFY_ACCOUNT_ACK m_kVerifyAccountAck{};
    KChannelChangeInfo      m_kChangeUserInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_kConnectAck)>>::put(w, m_kConnectAck);
        Codec<std::remove_cvref_t<decltype(m_kVerifyAccountAck)>>::put(w, m_kVerifyAccountAck);
        Codec<std::remove_cvref_t<decltype(m_kChangeUserInfo)>>::put(w, m_kChangeUserInfo);
    }

    static KEGS_CONNECT_CHANNEL_CHANGE_ACK read_from(Reader& r) {
        KEGS_CONNECT_CHANNEL_CHANGE_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_kConnectAck = Codec<std::remove_cvref_t<decltype(v.m_kConnectAck)>>::get(r);
        v.m_kVerifyAccountAck = Codec<std::remove_cvref_t<decltype(v.m_kVerifyAccountAck)>>::get(r);
        v.m_kChangeUserInfo = Codec<std::remove_cvref_t<decltype(v.m_kChangeUserInfo)>>::get(r);
        return v;
    }
};

struct KELG_CHANNEL_CHANGE_NEXON_USER_AUTHENTICATE_ACK {
    KEGS_CONNECT_ACK                 m_kConnectAck{};
    KELG_NEXON_USER_AUTHENTICATE_ACK m_kUserAuthAck{};
    KEGS_SELECT_UNIT_REQ             m_kSelectUnitReq{};
    KChannelChangeInfo               m_kChangeUserInfo{};
    bool                             m_bUseSecondPW{};
    std::u16string                   m_wstrSecondPW{};  // std::wstring
    std::int32_t                     m_iFailedCount{};  // int
    std::u16string                   m_wstrLastAuthDate{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kConnectAck)>>::put(w, m_kConnectAck);
        Codec<std::remove_cvref_t<decltype(m_kUserAuthAck)>>::put(w, m_kUserAuthAck);
        Codec<std::remove_cvref_t<decltype(m_kSelectUnitReq)>>::put(w, m_kSelectUnitReq);
        Codec<std::remove_cvref_t<decltype(m_kChangeUserInfo)>>::put(w, m_kChangeUserInfo);
        Codec<std::remove_cvref_t<decltype(m_bUseSecondPW)>>::put(w, m_bUseSecondPW);
        Codec<std::remove_cvref_t<decltype(m_wstrSecondPW)>>::put(w, m_wstrSecondPW);
        Codec<std::remove_cvref_t<decltype(m_iFailedCount)>>::put(w, m_iFailedCount);
        Codec<std::remove_cvref_t<decltype(m_wstrLastAuthDate)>>::put(w, m_wstrLastAuthDate);
    }

    static KELG_CHANNEL_CHANGE_NEXON_USER_AUTHENTICATE_ACK read_from(Reader& r) {
        KELG_CHANNEL_CHANGE_NEXON_USER_AUTHENTICATE_ACK v;
        v.m_kConnectAck = Codec<std::remove_cvref_t<decltype(v.m_kConnectAck)>>::get(r);
        v.m_kUserAuthAck = Codec<std::remove_cvref_t<decltype(v.m_kUserAuthAck)>>::get(r);
        v.m_kSelectUnitReq = Codec<std::remove_cvref_t<decltype(v.m_kSelectUnitReq)>>::get(r);
        v.m_kChangeUserInfo = Codec<std::remove_cvref_t<decltype(v.m_kChangeUserInfo)>>::get(r);
        v.m_bUseSecondPW = Codec<std::remove_cvref_t<decltype(v.m_bUseSecondPW)>>::get(r);
        v.m_wstrSecondPW = Codec<std::remove_cvref_t<decltype(v.m_wstrSecondPW)>>::get(r);
        v.m_iFailedCount = Codec<std::remove_cvref_t<decltype(v.m_iFailedCount)>>::get(r);
        v.m_wstrLastAuthDate = Codec<std::remove_cvref_t<decltype(v.m_wstrLastAuthDate)>>::get(r);
        return v;
    }
};

struct KELG_CHANNEL_CHANGE_NEXON_USER_AUTHENTICATE_REQ {
    KEGS_CONNECT_ACK                 m_kConnectAck{};
    KELG_NEXON_USER_AUTHENTICATE_REQ m_kUserAuthReq{};
    KEGS_SELECT_UNIT_REQ             m_kSelectUnitReq{};
    KChannelChangeInfo               m_kChangeUserInfo{};
    bool                             m_bServerUseKogOTP{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kConnectAck)>>::put(w, m_kConnectAck);
        Codec<std::remove_cvref_t<decltype(m_kUserAuthReq)>>::put(w, m_kUserAuthReq);
        Codec<std::remove_cvref_t<decltype(m_kSelectUnitReq)>>::put(w, m_kSelectUnitReq);
        Codec<std::remove_cvref_t<decltype(m_kChangeUserInfo)>>::put(w, m_kChangeUserInfo);
        Codec<std::remove_cvref_t<decltype(m_bServerUseKogOTP)>>::put(w, m_bServerUseKogOTP);
    }

    static KELG_CHANNEL_CHANGE_NEXON_USER_AUTHENTICATE_REQ read_from(Reader& r) {
        KELG_CHANNEL_CHANGE_NEXON_USER_AUTHENTICATE_REQ v;
        v.m_kConnectAck = Codec<std::remove_cvref_t<decltype(v.m_kConnectAck)>>::get(r);
        v.m_kUserAuthReq = Codec<std::remove_cvref_t<decltype(v.m_kUserAuthReq)>>::get(r);
        v.m_kSelectUnitReq = Codec<std::remove_cvref_t<decltype(v.m_kSelectUnitReq)>>::get(r);
        v.m_kChangeUserInfo = Codec<std::remove_cvref_t<decltype(v.m_kChangeUserInfo)>>::get(r);
        v.m_bServerUseKogOTP = Codec<std::remove_cvref_t<decltype(v.m_bServerUseKogOTP)>>::get(r);
        return v;
    }
};

struct KELG_CHANNEL_CHANGE_REGISTER_USER_ACK {
    std::int32_t            m_iOK{};  // int
    KEGS_CONNECT_ACK        m_kConnectAck{};
    KEGS_VERIFY_ACCOUNT_ACK m_kVerifyAccountAck{};
    KEGS_SELECT_UNIT_REQ    m_kSelectUnitReq{};
    KChannelChangeInfo      m_kChangeUserInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_kConnectAck)>>::put(w, m_kConnectAck);
        Codec<std::remove_cvref_t<decltype(m_kVerifyAccountAck)>>::put(w, m_kVerifyAccountAck);
        Codec<std::remove_cvref_t<decltype(m_kSelectUnitReq)>>::put(w, m_kSelectUnitReq);
        Codec<std::remove_cvref_t<decltype(m_kChangeUserInfo)>>::put(w, m_kChangeUserInfo);
    }

    static KELG_CHANNEL_CHANGE_REGISTER_USER_ACK read_from(Reader& r) {
        KELG_CHANNEL_CHANGE_REGISTER_USER_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_kConnectAck = Codec<std::remove_cvref_t<decltype(v.m_kConnectAck)>>::get(r);
        v.m_kVerifyAccountAck = Codec<std::remove_cvref_t<decltype(v.m_kVerifyAccountAck)>>::get(r);
        v.m_kSelectUnitReq = Codec<std::remove_cvref_t<decltype(v.m_kSelectUnitReq)>>::get(r);
        v.m_kChangeUserInfo = Codec<std::remove_cvref_t<decltype(v.m_kChangeUserInfo)>>::get(r);
        return v;
    }
};

struct KELG_CHANNEL_CHANGE_REGISTER_USER_REQ {
    KEGS_CONNECT_ACK        m_kConnectAck{};
    KEGS_VERIFY_ACCOUNT_ACK m_kVerifyAccountAck{};
    KEGS_SELECT_UNIT_REQ    m_kSelectUnitReq{};
    KChannelChangeInfo      m_kChangeUserInfo{};
    KELG_REGISTER_USER_REQ  m_kRegUserReq{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kConnectAck)>>::put(w, m_kConnectAck);
        Codec<std::remove_cvref_t<decltype(m_kVerifyAccountAck)>>::put(w, m_kVerifyAccountAck);
        Codec<std::remove_cvref_t<decltype(m_kSelectUnitReq)>>::put(w, m_kSelectUnitReq);
        Codec<std::remove_cvref_t<decltype(m_kChangeUserInfo)>>::put(w, m_kChangeUserInfo);
        Codec<std::remove_cvref_t<decltype(m_kRegUserReq)>>::put(w, m_kRegUserReq);
    }

    static KELG_CHANNEL_CHANGE_REGISTER_USER_REQ read_from(Reader& r) {
        KELG_CHANNEL_CHANGE_REGISTER_USER_REQ v;
        v.m_kConnectAck = Codec<std::remove_cvref_t<decltype(v.m_kConnectAck)>>::get(r);
        v.m_kVerifyAccountAck = Codec<std::remove_cvref_t<decltype(v.m_kVerifyAccountAck)>>::get(r);
        v.m_kSelectUnitReq = Codec<std::remove_cvref_t<decltype(v.m_kSelectUnitReq)>>::get(r);
        v.m_kChangeUserInfo = Codec<std::remove_cvref_t<decltype(v.m_kChangeUserInfo)>>::get(r);
        v.m_kRegUserReq = Codec<std::remove_cvref_t<decltype(v.m_kRegUserReq)>>::get(r);
        return v;
    }
};

struct KUserCumulativeTimeEventInfo {
    std::int32_t   m_iEventUID{};  // int
    std::int32_t   m_iEventID{};  // int
    bool           m_bInit{};
    std::u16string m_wstrEventName{};  // std::wstring
    float          m_fEventTime{};
    bool           m_bDirectReward{};
    bool           m_bAccountEvent{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iEventUID)>>::put(w, m_iEventUID);
        Codec<std::remove_cvref_t<decltype(m_iEventID)>>::put(w, m_iEventID);
        Codec<std::remove_cvref_t<decltype(m_bInit)>>::put(w, m_bInit);
        Codec<std::remove_cvref_t<decltype(m_wstrEventName)>>::put(w, m_wstrEventName);
        Codec<std::remove_cvref_t<decltype(m_fEventTime)>>::put(w, m_fEventTime);
        Codec<std::remove_cvref_t<decltype(m_bDirectReward)>>::put(w, m_bDirectReward);
        Codec<std::remove_cvref_t<decltype(m_bAccountEvent)>>::put(w, m_bAccountEvent);
    }

    static KUserCumulativeTimeEventInfo read_from(Reader& r) {
        KUserCumulativeTimeEventInfo v;
        v.m_iEventUID = Codec<std::remove_cvref_t<decltype(v.m_iEventUID)>>::get(r);
        v.m_iEventID = Codec<std::remove_cvref_t<decltype(v.m_iEventID)>>::get(r);
        v.m_bInit = Codec<std::remove_cvref_t<decltype(v.m_bInit)>>::get(r);
        v.m_wstrEventName = Codec<std::remove_cvref_t<decltype(v.m_wstrEventName)>>::get(r);
        v.m_fEventTime = Codec<std::remove_cvref_t<decltype(v.m_fEventTime)>>::get(r);
        v.m_bDirectReward = Codec<std::remove_cvref_t<decltype(v.m_bDirectReward)>>::get(r);
        v.m_bAccountEvent = Codec<std::remove_cvref_t<decltype(v.m_bAccountEvent)>>::get(r);
        return v;
    }
};

struct KUserGuildInfo {
    std::int32_t   m_iGuildUID{};  // int
    std::u16string m_wstrGuildName{};  // std::wstring
    std::uint8_t   m_ucMemberShipGrade{};  // u_char
    std::int32_t   m_iHonorPoint{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iGuildUID)>>::put(w, m_iGuildUID);
        Codec<std::remove_cvref_t<decltype(m_wstrGuildName)>>::put(w, m_wstrGuildName);
        Codec<std::remove_cvref_t<decltype(m_ucMemberShipGrade)>>::put(w, m_ucMemberShipGrade);
        Codec<std::remove_cvref_t<decltype(m_iHonorPoint)>>::put(w, m_iHonorPoint);
    }

    static KUserGuildInfo read_from(Reader& r) {
        KUserGuildInfo v;
        v.m_iGuildUID = Codec<std::remove_cvref_t<decltype(v.m_iGuildUID)>>::get(r);
        v.m_wstrGuildName = Codec<std::remove_cvref_t<decltype(v.m_wstrGuildName)>>::get(r);
        v.m_ucMemberShipGrade = Codec<std::remove_cvref_t<decltype(v.m_ucMemberShipGrade)>>::get(r);
        v.m_iHonorPoint = Codec<std::remove_cvref_t<decltype(v.m_iHonorPoint)>>::get(r);
        return v;
    }
};

struct KUnitInfo {
    std::int64_t                               m_iOwnerUserUID{};  // UidType
    char                                       m_cAuthLevel{};
    std::int64_t                               m_nUnitUID{};  // UidType
    std::uint32_t                              m_uiKNMSerialNum{};  // u_int
    char                                       m_cUnitClass{};
    std::u16string                             m_wstrNickName{};  // std::wstring
    std::u16string                             m_wstrIP{};  // std::wstring
    std::uint16_t                              m_usPort{};  // USHORT
    std::int32_t                               m_iED{};  // int
    std::uint8_t                               m_ucLevel{};  // UCHAR
    std::int32_t                               m_iEXP{};  // int
    std::int32_t                               m_iOfficialMatchCnt{};  // int
    std::int32_t                               m_iRating{};  // int
    std::int32_t                               m_iMaxRating{};  // int
    std::int32_t                               m_iRPoint{};  // int
    std::int32_t                               m_iAPoint{};  // int
    bool                                       m_bIsWinBeforeMatch{};
    char                                       m_cRank{};
    float                                      m_fKFactor{};
    bool                                       m_bIsRedistributionUser{};
    std::int32_t                               m_iPastSeasonWin{};  // int
    std::int32_t                               m_iSPoint{};  // int
    std::int32_t                               m_iCSPoint{};  // int
    std::int32_t                               m_iMaxCSPoint{};  // int
    std::u16string                             m_wstrCSPointEndDate{};  // std::wstring
    std::int32_t                               m_nNowBaseLevelEXP{};  // int
    std::int32_t                               m_nNextBaseLevelEXP{};  // int
    std::int32_t                               m_nStraightVictories{};  // int
    KStat                                      m_kStat{};
    KStat                                      m_kGameStat{};
    KLastPositionInfo                          m_kLastPos{};
    std::vector<KBuffInfo>                     m_vecBuffInfo{};  // std::vector< KBuffInfo >
    std::int32_t                               m_iWin{};  // int
    std::int32_t                               m_iLose{};  // int
    std::map<std::int32_t, KDungeonClearInfo>  m_mapDungeonClear{};  // std::map< int, KDungeonClearInfo >
    std::map<std::int32_t, KTCClearInfo>       m_mapTCClear{};  // std::map< int, KTCClearInfo >
    std::map<std::int32_t, KDungeonPlayInfo>   m_mapDungeonPlay{};  // std::map< int, KDungeonPlayInfo >
    std::map<std::int32_t, KInventoryItemInfo> m_mapEquippedItem{};  // std::map< int, KInventoryItemInfo >
    KUnitSkillData                             m_UnitSkillData{};
    bool                                       m_bIsParty{};
    std::int32_t                               m_iSpiritMax{};  // int
    std::int32_t                               m_iSpirit{};  // int
    bool                                       m_bIsGameBang{};
    std::int32_t                               m_iPcBangType{};  // int
    std::int32_t                               m_iTitleID{};  // int
    KUserGuildInfo                             m_kUserGuildInfo{};
    std::u16string                             m_wstrLastLoginTime{};  // std::wstring
    std::int64_t                               m_trWarpVipEndData{};  // __int64
    std::int32_t                               m_iEventQuestClearCount{};  // int
    std::int32_t                               m_iExchangeCount{};  // int
    std::uint8_t                               m_ucOldYearMissionRewardedLevel{};  // u_char
    std::int32_t                               m_iNewYearMissionStepID{};  // int
    std::uint8_t                               m_ucCheckPowerCount{};  // unsigned char
    std::int64_t                               m_iCheckPowerTime{};  // __int64
    bool                                       m_bCheckPowerShowPopUp{};
    std::uint8_t                               m_ucCheckPowerScore{};  // unsigned char

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOwnerUserUID)>>::put(w, m_iOwnerUserUID);
        Codec<std::remove_cvref_t<decltype(m_cAuthLevel)>>::put(w, m_cAuthLevel);
        Codec<std::remove_cvref_t<decltype(m_nUnitUID)>>::put(w, m_nUnitUID);
        Codec<std::remove_cvref_t<decltype(m_uiKNMSerialNum)>>::put(w, m_uiKNMSerialNum);
        Codec<std::remove_cvref_t<decltype(m_cUnitClass)>>::put(w, m_cUnitClass);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_wstrIP)>>::put(w, m_wstrIP);
        Codec<std::remove_cvref_t<decltype(m_usPort)>>::put(w, m_usPort);
        Codec<std::remove_cvref_t<decltype(m_iED)>>::put(w, m_iED);
        Codec<std::remove_cvref_t<decltype(m_ucLevel)>>::put(w, m_ucLevel);
        Codec<std::remove_cvref_t<decltype(m_iEXP)>>::put(w, m_iEXP);
        Codec<std::remove_cvref_t<decltype(m_iOfficialMatchCnt)>>::put(w, m_iOfficialMatchCnt);
        Codec<std::remove_cvref_t<decltype(m_iRating)>>::put(w, m_iRating);
        Codec<std::remove_cvref_t<decltype(m_iMaxRating)>>::put(w, m_iMaxRating);
        Codec<std::remove_cvref_t<decltype(m_iRPoint)>>::put(w, m_iRPoint);
        Codec<std::remove_cvref_t<decltype(m_iAPoint)>>::put(w, m_iAPoint);
        Codec<std::remove_cvref_t<decltype(m_bIsWinBeforeMatch)>>::put(w, m_bIsWinBeforeMatch);
        Codec<std::remove_cvref_t<decltype(m_cRank)>>::put(w, m_cRank);
        Codec<std::remove_cvref_t<decltype(m_fKFactor)>>::put(w, m_fKFactor);
        Codec<std::remove_cvref_t<decltype(m_bIsRedistributionUser)>>::put(w, m_bIsRedistributionUser);
        Codec<std::remove_cvref_t<decltype(m_iPastSeasonWin)>>::put(w, m_iPastSeasonWin);
        Codec<std::remove_cvref_t<decltype(m_iSPoint)>>::put(w, m_iSPoint);
        Codec<std::remove_cvref_t<decltype(m_iCSPoint)>>::put(w, m_iCSPoint);
        Codec<std::remove_cvref_t<decltype(m_iMaxCSPoint)>>::put(w, m_iMaxCSPoint);
        Codec<std::remove_cvref_t<decltype(m_wstrCSPointEndDate)>>::put(w, m_wstrCSPointEndDate);
        Codec<std::remove_cvref_t<decltype(m_nNowBaseLevelEXP)>>::put(w, m_nNowBaseLevelEXP);
        Codec<std::remove_cvref_t<decltype(m_nNextBaseLevelEXP)>>::put(w, m_nNextBaseLevelEXP);
        Codec<std::remove_cvref_t<decltype(m_nStraightVictories)>>::put(w, m_nStraightVictories);
        Codec<std::remove_cvref_t<decltype(m_kStat)>>::put(w, m_kStat);
        Codec<std::remove_cvref_t<decltype(m_kGameStat)>>::put(w, m_kGameStat);
        Codec<std::remove_cvref_t<decltype(m_kLastPos)>>::put(w, m_kLastPos);
        Codec<std::remove_cvref_t<decltype(m_vecBuffInfo)>>::put(w, m_vecBuffInfo);
        Codec<std::remove_cvref_t<decltype(m_iWin)>>::put(w, m_iWin);
        Codec<std::remove_cvref_t<decltype(m_iLose)>>::put(w, m_iLose);
        Codec<std::remove_cvref_t<decltype(m_mapDungeonClear)>>::put(w, m_mapDungeonClear);
        Codec<std::remove_cvref_t<decltype(m_mapTCClear)>>::put(w, m_mapTCClear);
        Codec<std::remove_cvref_t<decltype(m_mapDungeonPlay)>>::put(w, m_mapDungeonPlay);
        Codec<std::remove_cvref_t<decltype(m_mapEquippedItem)>>::put(w, m_mapEquippedItem);
        Codec<std::remove_cvref_t<decltype(m_UnitSkillData)>>::put(w, m_UnitSkillData);
        Codec<std::remove_cvref_t<decltype(m_bIsParty)>>::put(w, m_bIsParty);
        Codec<std::remove_cvref_t<decltype(m_iSpiritMax)>>::put(w, m_iSpiritMax);
        Codec<std::remove_cvref_t<decltype(m_iSpirit)>>::put(w, m_iSpirit);
        Codec<std::remove_cvref_t<decltype(m_bIsGameBang)>>::put(w, m_bIsGameBang);
        Codec<std::remove_cvref_t<decltype(m_iPcBangType)>>::put(w, m_iPcBangType);
        Codec<std::remove_cvref_t<decltype(m_iTitleID)>>::put(w, m_iTitleID);
        Codec<std::remove_cvref_t<decltype(m_kUserGuildInfo)>>::put(w, m_kUserGuildInfo);
        Codec<std::remove_cvref_t<decltype(m_wstrLastLoginTime)>>::put(w, m_wstrLastLoginTime);
        Codec<std::remove_cvref_t<decltype(m_trWarpVipEndData)>>::put(w, m_trWarpVipEndData);
        Codec<std::remove_cvref_t<decltype(m_iEventQuestClearCount)>>::put(w, m_iEventQuestClearCount);
        Codec<std::remove_cvref_t<decltype(m_iExchangeCount)>>::put(w, m_iExchangeCount);
        Codec<std::remove_cvref_t<decltype(m_ucOldYearMissionRewardedLevel)>>::put(w, m_ucOldYearMissionRewardedLevel);
        Codec<std::remove_cvref_t<decltype(m_iNewYearMissionStepID)>>::put(w, m_iNewYearMissionStepID);
        Codec<std::remove_cvref_t<decltype(m_ucCheckPowerCount)>>::put(w, m_ucCheckPowerCount);
        Codec<std::remove_cvref_t<decltype(m_iCheckPowerTime)>>::put(w, m_iCheckPowerTime);
        Codec<std::remove_cvref_t<decltype(m_bCheckPowerShowPopUp)>>::put(w, m_bCheckPowerShowPopUp);
        Codec<std::remove_cvref_t<decltype(m_ucCheckPowerScore)>>::put(w, m_ucCheckPowerScore);
    }

    static KUnitInfo read_from(Reader& r) {
        KUnitInfo v;
        v.m_iOwnerUserUID = Codec<std::remove_cvref_t<decltype(v.m_iOwnerUserUID)>>::get(r);
        v.m_cAuthLevel = Codec<std::remove_cvref_t<decltype(v.m_cAuthLevel)>>::get(r);
        v.m_nUnitUID = Codec<std::remove_cvref_t<decltype(v.m_nUnitUID)>>::get(r);
        v.m_uiKNMSerialNum = Codec<std::remove_cvref_t<decltype(v.m_uiKNMSerialNum)>>::get(r);
        v.m_cUnitClass = Codec<std::remove_cvref_t<decltype(v.m_cUnitClass)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_wstrIP = Codec<std::remove_cvref_t<decltype(v.m_wstrIP)>>::get(r);
        v.m_usPort = Codec<std::remove_cvref_t<decltype(v.m_usPort)>>::get(r);
        v.m_iED = Codec<std::remove_cvref_t<decltype(v.m_iED)>>::get(r);
        v.m_ucLevel = Codec<std::remove_cvref_t<decltype(v.m_ucLevel)>>::get(r);
        v.m_iEXP = Codec<std::remove_cvref_t<decltype(v.m_iEXP)>>::get(r);
        v.m_iOfficialMatchCnt = Codec<std::remove_cvref_t<decltype(v.m_iOfficialMatchCnt)>>::get(r);
        v.m_iRating = Codec<std::remove_cvref_t<decltype(v.m_iRating)>>::get(r);
        v.m_iMaxRating = Codec<std::remove_cvref_t<decltype(v.m_iMaxRating)>>::get(r);
        v.m_iRPoint = Codec<std::remove_cvref_t<decltype(v.m_iRPoint)>>::get(r);
        v.m_iAPoint = Codec<std::remove_cvref_t<decltype(v.m_iAPoint)>>::get(r);
        v.m_bIsWinBeforeMatch = Codec<std::remove_cvref_t<decltype(v.m_bIsWinBeforeMatch)>>::get(r);
        v.m_cRank = Codec<std::remove_cvref_t<decltype(v.m_cRank)>>::get(r);
        v.m_fKFactor = Codec<std::remove_cvref_t<decltype(v.m_fKFactor)>>::get(r);
        v.m_bIsRedistributionUser = Codec<std::remove_cvref_t<decltype(v.m_bIsRedistributionUser)>>::get(r);
        v.m_iPastSeasonWin = Codec<std::remove_cvref_t<decltype(v.m_iPastSeasonWin)>>::get(r);
        v.m_iSPoint = Codec<std::remove_cvref_t<decltype(v.m_iSPoint)>>::get(r);
        v.m_iCSPoint = Codec<std::remove_cvref_t<decltype(v.m_iCSPoint)>>::get(r);
        v.m_iMaxCSPoint = Codec<std::remove_cvref_t<decltype(v.m_iMaxCSPoint)>>::get(r);
        v.m_wstrCSPointEndDate = Codec<std::remove_cvref_t<decltype(v.m_wstrCSPointEndDate)>>::get(r);
        v.m_nNowBaseLevelEXP = Codec<std::remove_cvref_t<decltype(v.m_nNowBaseLevelEXP)>>::get(r);
        v.m_nNextBaseLevelEXP = Codec<std::remove_cvref_t<decltype(v.m_nNextBaseLevelEXP)>>::get(r);
        v.m_nStraightVictories = Codec<std::remove_cvref_t<decltype(v.m_nStraightVictories)>>::get(r);
        v.m_kStat = Codec<std::remove_cvref_t<decltype(v.m_kStat)>>::get(r);
        v.m_kGameStat = Codec<std::remove_cvref_t<decltype(v.m_kGameStat)>>::get(r);
        v.m_kLastPos = Codec<std::remove_cvref_t<decltype(v.m_kLastPos)>>::get(r);
        v.m_vecBuffInfo = Codec<std::remove_cvref_t<decltype(v.m_vecBuffInfo)>>::get(r);
        v.m_iWin = Codec<std::remove_cvref_t<decltype(v.m_iWin)>>::get(r);
        v.m_iLose = Codec<std::remove_cvref_t<decltype(v.m_iLose)>>::get(r);
        v.m_mapDungeonClear = Codec<std::remove_cvref_t<decltype(v.m_mapDungeonClear)>>::get(r);
        v.m_mapTCClear = Codec<std::remove_cvref_t<decltype(v.m_mapTCClear)>>::get(r);
        v.m_mapDungeonPlay = Codec<std::remove_cvref_t<decltype(v.m_mapDungeonPlay)>>::get(r);
        v.m_mapEquippedItem = Codec<std::remove_cvref_t<decltype(v.m_mapEquippedItem)>>::get(r);
        v.m_UnitSkillData = Codec<std::remove_cvref_t<decltype(v.m_UnitSkillData)>>::get(r);
        v.m_bIsParty = Codec<std::remove_cvref_t<decltype(v.m_bIsParty)>>::get(r);
        v.m_iSpiritMax = Codec<std::remove_cvref_t<decltype(v.m_iSpiritMax)>>::get(r);
        v.m_iSpirit = Codec<std::remove_cvref_t<decltype(v.m_iSpirit)>>::get(r);
        v.m_bIsGameBang = Codec<std::remove_cvref_t<decltype(v.m_bIsGameBang)>>::get(r);
        v.m_iPcBangType = Codec<std::remove_cvref_t<decltype(v.m_iPcBangType)>>::get(r);
        v.m_iTitleID = Codec<std::remove_cvref_t<decltype(v.m_iTitleID)>>::get(r);
        v.m_kUserGuildInfo = Codec<std::remove_cvref_t<decltype(v.m_kUserGuildInfo)>>::get(r);
        v.m_wstrLastLoginTime = Codec<std::remove_cvref_t<decltype(v.m_wstrLastLoginTime)>>::get(r);
        v.m_trWarpVipEndData = Codec<std::remove_cvref_t<decltype(v.m_trWarpVipEndData)>>::get(r);
        v.m_iEventQuestClearCount = Codec<std::remove_cvref_t<decltype(v.m_iEventQuestClearCount)>>::get(r);
        v.m_iExchangeCount = Codec<std::remove_cvref_t<decltype(v.m_iExchangeCount)>>::get(r);
        v.m_ucOldYearMissionRewardedLevel = Codec<std::remove_cvref_t<decltype(v.m_ucOldYearMissionRewardedLevel)>>::get(r);
        v.m_iNewYearMissionStepID = Codec<std::remove_cvref_t<decltype(v.m_iNewYearMissionStepID)>>::get(r);
        v.m_ucCheckPowerCount = Codec<std::remove_cvref_t<decltype(v.m_ucCheckPowerCount)>>::get(r);
        v.m_iCheckPowerTime = Codec<std::remove_cvref_t<decltype(v.m_iCheckPowerTime)>>::get(r);
        v.m_bCheckPowerShowPopUp = Codec<std::remove_cvref_t<decltype(v.m_bCheckPowerShowPopUp)>>::get(r);
        v.m_ucCheckPowerScore = Codec<std::remove_cvref_t<decltype(v.m_ucCheckPowerScore)>>::get(r);
        return v;
    }
};

struct KEGS_ADMIN_MODIFY_UNIT_LEVEL_ACK {
    std::int32_t m_iOK{};  // int
    KUnitInfo    m_kUnitInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_kUnitInfo)>>::put(w, m_kUnitInfo);
    }

    static KEGS_ADMIN_MODIFY_UNIT_LEVEL_ACK read_from(Reader& r) {
        KEGS_ADMIN_MODIFY_UNIT_LEVEL_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_kUnitInfo = Codec<std::remove_cvref_t<decltype(v.m_kUnitInfo)>>::get(r);
        return v;
    }
};

struct KEGS_BUY_UNIT_CLASS_CHANGE_NOT {
    std::int64_t                    m_iUnitUID{};  // UidType
    std::int32_t                    m_iNewUnitClass{};  // int
    std::vector<std::int16_t>       m_vecSkillUnsealed{};  // std::vector< short >
    std::map<char, std::int32_t>    m_mapChangeMemo{};  // std::map< char, int >
    std::vector<KInventoryItemInfo> m_vecChangeItem{};  // std::vector< KInventoryItemInfo >
    std::vector<KCompleteQuestInfo> m_vecChangeCompleteQuest{};  // std::vector< KCompleteQuestInfo >
    std::vector<KQuestInstance>     m_vecChangeInProgressQuest{};  // std::vector< KQuestInstance >
    KUnitInfo                       m_kUnitInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iNewUnitClass)>>::put(w, m_iNewUnitClass);
        Codec<std::remove_cvref_t<decltype(m_vecSkillUnsealed)>>::put(w, m_vecSkillUnsealed);
        Codec<std::remove_cvref_t<decltype(m_mapChangeMemo)>>::put(w, m_mapChangeMemo);
        Codec<std::remove_cvref_t<decltype(m_vecChangeItem)>>::put(w, m_vecChangeItem);
        Codec<std::remove_cvref_t<decltype(m_vecChangeCompleteQuest)>>::put(w, m_vecChangeCompleteQuest);
        Codec<std::remove_cvref_t<decltype(m_vecChangeInProgressQuest)>>::put(w, m_vecChangeInProgressQuest);
        Codec<std::remove_cvref_t<decltype(m_kUnitInfo)>>::put(w, m_kUnitInfo);
    }

    static KEGS_BUY_UNIT_CLASS_CHANGE_NOT read_from(Reader& r) {
        KEGS_BUY_UNIT_CLASS_CHANGE_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iNewUnitClass = Codec<std::remove_cvref_t<decltype(v.m_iNewUnitClass)>>::get(r);
        v.m_vecSkillUnsealed = Codec<std::remove_cvref_t<decltype(v.m_vecSkillUnsealed)>>::get(r);
        v.m_mapChangeMemo = Codec<std::remove_cvref_t<decltype(v.m_mapChangeMemo)>>::get(r);
        v.m_vecChangeItem = Codec<std::remove_cvref_t<decltype(v.m_vecChangeItem)>>::get(r);
        v.m_vecChangeCompleteQuest = Codec<std::remove_cvref_t<decltype(v.m_vecChangeCompleteQuest)>>::get(r);
        v.m_vecChangeInProgressQuest = Codec<std::remove_cvref_t<decltype(v.m_vecChangeInProgressQuest)>>::get(r);
        v.m_kUnitInfo = Codec<std::remove_cvref_t<decltype(v.m_kUnitInfo)>>::get(r);
        return v;
    }
};

struct KEGS_CHANGE_JOB_CASH_ITEM_NOT {
    KUnitInfo m_kUnitInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kUnitInfo)>>::put(w, m_kUnitInfo);
    }

    static KEGS_CHANGE_JOB_CASH_ITEM_NOT read_from(Reader& r) {
        KEGS_CHANGE_JOB_CASH_ITEM_NOT v;
        v.m_kUnitInfo = Codec<std::remove_cvref_t<decltype(v.m_kUnitInfo)>>::get(r);
        return v;
    }
};

struct KEGS_CHARACTER_LIST_ACK {
    std::int32_t                                   m_iOK{};  // int
    std::map<std::int32_t, std::int32_t>           m_mapServerGroupUnitSlot{};  // std::map< int, int >
    std::map<std::int32_t, std::vector<KUnitInfo>> m_mapServerGroupUnitInfo{};  // std::map< int, std::vector<KUnitInfo> >
    std::u16string                                 m_strUserID{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_mapServerGroupUnitSlot)>>::put(w, m_mapServerGroupUnitSlot);
        Codec<std::remove_cvref_t<decltype(m_mapServerGroupUnitInfo)>>::put(w, m_mapServerGroupUnitInfo);
        Codec<std::remove_cvref_t<decltype(m_strUserID)>>::put(w, m_strUserID);
    }

    static KEGS_CHARACTER_LIST_ACK read_from(Reader& r) {
        KEGS_CHARACTER_LIST_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_mapServerGroupUnitSlot = Codec<std::remove_cvref_t<decltype(v.m_mapServerGroupUnitSlot)>>::get(r);
        v.m_mapServerGroupUnitInfo = Codec<std::remove_cvref_t<decltype(v.m_mapServerGroupUnitInfo)>>::get(r);
        v.m_strUserID = Codec<std::remove_cvref_t<decltype(v.m_strUserID)>>::get(r);
        return v;
    }
};

struct KEGS_CREATE_UNIT_ACK {
    std::int32_t              m_iOK{};  // int
    KUnitInfo                 m_kUnitInfo{};
    std::vector<std::int32_t> m_vecSkillSlot{};  // std::vector<int>
    std::u16string            m_wstrEnableDate{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_kUnitInfo)>>::put(w, m_kUnitInfo);
        Codec<std::remove_cvref_t<decltype(m_vecSkillSlot)>>::put(w, m_vecSkillSlot);
        Codec<std::remove_cvref_t<decltype(m_wstrEnableDate)>>::put(w, m_wstrEnableDate);
    }

    static KEGS_CREATE_UNIT_ACK read_from(Reader& r) {
        KEGS_CREATE_UNIT_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_kUnitInfo = Codec<std::remove_cvref_t<decltype(v.m_kUnitInfo)>>::get(r);
        v.m_vecSkillSlot = Codec<std::remove_cvref_t<decltype(v.m_vecSkillSlot)>>::get(r);
        v.m_wstrEnableDate = Codec<std::remove_cvref_t<decltype(v.m_wstrEnableDate)>>::get(r);
        return v;
    }
};

struct KEGS_END_TC_GAME_ACK {
    std::int32_t m_iOK{};  // int
    bool         m_bIsSuccess{};
    KUnitInfo    m_kUnitInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_bIsSuccess)>>::put(w, m_bIsSuccess);
        Codec<std::remove_cvref_t<decltype(m_kUnitInfo)>>::put(w, m_kUnitInfo);
    }

    static KEGS_END_TC_GAME_ACK read_from(Reader& r) {
        KEGS_END_TC_GAME_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_bIsSuccess = Codec<std::remove_cvref_t<decltype(v.m_bIsSuccess)>>::get(r);
        v.m_kUnitInfo = Codec<std::remove_cvref_t<decltype(v.m_kUnitInfo)>>::get(r);
        return v;
    }
};

struct KEGS_MY_UNIT_AND_INVENTORY_INFO_LIST_ACK {
    std::int32_t           m_iOK{};  // int
    std::int32_t           m_nUnitSlot{};  // int
    std::vector<KUnitInfo> m_vecUnitInfo{};
    bool                   m_bSharingBank{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_nUnitSlot)>>::put(w, m_nUnitSlot);
        Codec<std::remove_cvref_t<decltype(m_vecUnitInfo)>>::put(w, m_vecUnitInfo);
        Codec<std::remove_cvref_t<decltype(m_bSharingBank)>>::put(w, m_bSharingBank);
    }

    static KEGS_MY_UNIT_AND_INVENTORY_INFO_LIST_ACK read_from(Reader& r) {
        KEGS_MY_UNIT_AND_INVENTORY_INFO_LIST_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_nUnitSlot = Codec<std::remove_cvref_t<decltype(v.m_nUnitSlot)>>::get(r);
        v.m_vecUnitInfo = Codec<std::remove_cvref_t<decltype(v.m_vecUnitInfo)>>::get(r);
        v.m_bSharingBank = Codec<std::remove_cvref_t<decltype(v.m_bSharingBank)>>::get(r);
        return v;
    }
};

struct KEGS_UPDATE_UNIT_INFO_NOT {
    KUnitInfo                            m_kUnitInfo{};
    std::vector<KInventoryItemInfo>      m_vecKInventorySlotInfo{};  // std::vector< KInventoryItemInfo >
    std::map<std::int32_t, std::int32_t> m_mapItemObtained{};  // std::map< int, int >
    bool                                 m_bGameEnd{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kUnitInfo)>>::put(w, m_kUnitInfo);
        Codec<std::remove_cvref_t<decltype(m_vecKInventorySlotInfo)>>::put(w, m_vecKInventorySlotInfo);
        Codec<std::remove_cvref_t<decltype(m_mapItemObtained)>>::put(w, m_mapItemObtained);
        Codec<std::remove_cvref_t<decltype(m_bGameEnd)>>::put(w, m_bGameEnd);
    }

    static KEGS_UPDATE_UNIT_INFO_NOT read_from(Reader& r) {
        KEGS_UPDATE_UNIT_INFO_NOT v;
        v.m_kUnitInfo = Codec<std::remove_cvref_t<decltype(v.m_kUnitInfo)>>::get(r);
        v.m_vecKInventorySlotInfo = Codec<std::remove_cvref_t<decltype(v.m_vecKInventorySlotInfo)>>::get(r);
        v.m_mapItemObtained = Codec<std::remove_cvref_t<decltype(v.m_mapItemObtained)>>::get(r);
        v.m_bGameEnd = Codec<std::remove_cvref_t<decltype(v.m_bGameEnd)>>::get(r);
        return v;
    }
};

struct KEGS_QUEST_COMPLETE_ACK {
    std::int32_t              m_iOK{};  // int
    KCompleteQuestInfo        m_kCompleteQuestInfo{};
    KEGS_UPDATE_UNIT_INFO_NOT m_kUpdateUnitInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_kCompleteQuestInfo)>>::put(w, m_kCompleteQuestInfo);
        Codec<std::remove_cvref_t<decltype(m_kUpdateUnitInfo)>>::put(w, m_kUpdateUnitInfo);
    }

    static KEGS_QUEST_COMPLETE_ACK read_from(Reader& r) {
        KEGS_QUEST_COMPLETE_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_kCompleteQuestInfo = Codec<std::remove_cvref_t<decltype(v.m_kCompleteQuestInfo)>>::get(r);
        v.m_kUpdateUnitInfo = Codec<std::remove_cvref_t<decltype(v.m_kUpdateUnitInfo)>>::get(r);
        return v;
    }
};

struct KEGS_ALL_COMPLETED_QUEST_COMPLETE_ACK {
    std::int32_t                         m_iOK{};  // int
    std::vector<KEGS_QUEST_COMPLETE_ACK> m_vecQuestCompleteAck{};  // std::vector< KEGS_QUEST_COMPLETE_ACK >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_vecQuestCompleteAck)>>::put(w, m_vecQuestCompleteAck);
    }

    static KEGS_ALL_COMPLETED_QUEST_COMPLETE_ACK read_from(Reader& r) {
        KEGS_ALL_COMPLETED_QUEST_COMPLETE_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_vecQuestCompleteAck = Codec<std::remove_cvref_t<decltype(v.m_vecQuestCompleteAck)>>::get(r);
        return v;
    }
};

struct KUserPShopAgencyInfo {
    bool                                  m_bIsPShopOpen{};
    std::u16string                        m_wstrAgencyExpirationDate{};  // std::wstring
    char                                  m_cShopType{};
    std::vector<KSellPShopItemBackupData> m_vecSellItemInfo{};  // std::vector< KSellPShopItemBackupData >
    std::int64_t                          m_iUserUID{};  // UidType
    std::int64_t                          m_iUnitUID{};  // UidType
    std::u16string                        m_wstrNickName{};  // std::wstring
    std::u16string                        m_wstrAgencyOpenDate{};  // std::wstring
    std::u16string                        m_wstrPersonalShopName{};  // std::wstring
    bool                                  m_bOnSale{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bIsPShopOpen)>>::put(w, m_bIsPShopOpen);
        Codec<std::remove_cvref_t<decltype(m_wstrAgencyExpirationDate)>>::put(w, m_wstrAgencyExpirationDate);
        Codec<std::remove_cvref_t<decltype(m_cShopType)>>::put(w, m_cShopType);
        Codec<std::remove_cvref_t<decltype(m_vecSellItemInfo)>>::put(w, m_vecSellItemInfo);
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_wstrAgencyOpenDate)>>::put(w, m_wstrAgencyOpenDate);
        Codec<std::remove_cvref_t<decltype(m_wstrPersonalShopName)>>::put(w, m_wstrPersonalShopName);
        Codec<std::remove_cvref_t<decltype(m_bOnSale)>>::put(w, m_bOnSale);
    }

    static KUserPShopAgencyInfo read_from(Reader& r) {
        KUserPShopAgencyInfo v;
        v.m_bIsPShopOpen = Codec<std::remove_cvref_t<decltype(v.m_bIsPShopOpen)>>::get(r);
        v.m_wstrAgencyExpirationDate = Codec<std::remove_cvref_t<decltype(v.m_wstrAgencyExpirationDate)>>::get(r);
        v.m_cShopType = Codec<std::remove_cvref_t<decltype(v.m_cShopType)>>::get(r);
        v.m_vecSellItemInfo = Codec<std::remove_cvref_t<decltype(v.m_vecSellItemInfo)>>::get(r);
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_wstrAgencyOpenDate = Codec<std::remove_cvref_t<decltype(v.m_wstrAgencyOpenDate)>>::get(r);
        v.m_wstrPersonalShopName = Codec<std::remove_cvref_t<decltype(v.m_wstrPersonalShopName)>>::get(r);
        v.m_bOnSale = Codec<std::remove_cvref_t<decltype(v.m_bOnSale)>>::get(r);
        return v;
    }
};

struct KEGS_SELECT_UNIT_5_NOT {
    std::int32_t                         m_iOK{};  // int
    bool                                 m_bIsSpiritUpdated{};
    std::map<std::int32_t, std::int32_t> m_mapCharGameCount{};  // std::map< int, int >
    KUserPShopAgencyInfo                 m_kPShopAgencyInfo{};
    KEGS_HENIR_REWARD_COUNT_NOT          m_PacketHenirRewardCount{};
    bool                                 m_bCashShopOpen{};
    bool                                 m_bFirstSelect{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_bIsSpiritUpdated)>>::put(w, m_bIsSpiritUpdated);
        Codec<std::remove_cvref_t<decltype(m_mapCharGameCount)>>::put(w, m_mapCharGameCount);
        Codec<std::remove_cvref_t<decltype(m_kPShopAgencyInfo)>>::put(w, m_kPShopAgencyInfo);
        Codec<std::remove_cvref_t<decltype(m_PacketHenirRewardCount)>>::put(w, m_PacketHenirRewardCount);
        Codec<std::remove_cvref_t<decltype(m_bCashShopOpen)>>::put(w, m_bCashShopOpen);
        Codec<std::remove_cvref_t<decltype(m_bFirstSelect)>>::put(w, m_bFirstSelect);
    }

    static KEGS_SELECT_UNIT_5_NOT read_from(Reader& r) {
        KEGS_SELECT_UNIT_5_NOT v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_bIsSpiritUpdated = Codec<std::remove_cvref_t<decltype(v.m_bIsSpiritUpdated)>>::get(r);
        v.m_mapCharGameCount = Codec<std::remove_cvref_t<decltype(v.m_mapCharGameCount)>>::get(r);
        v.m_kPShopAgencyInfo = Codec<std::remove_cvref_t<decltype(v.m_kPShopAgencyInfo)>>::get(r);
        v.m_PacketHenirRewardCount = Codec<std::remove_cvref_t<decltype(v.m_PacketHenirRewardCount)>>::get(r);
        v.m_bCashShopOpen = Codec<std::remove_cvref_t<decltype(v.m_bCashShopOpen)>>::get(r);
        v.m_bFirstSelect = Codec<std::remove_cvref_t<decltype(v.m_bFirstSelect)>>::get(r);
        return v;
    }
};

struct KUserSkillData {
    std::int16_t m_iSkillID{};  // short
    std::uint8_t m_cSkillLevel{};  // UCHAR
    std::uint8_t m_cSkillCSPoint{};  // UCHAR

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iSkillID)>>::put(w, m_iSkillID);
        Codec<std::remove_cvref_t<decltype(m_cSkillLevel)>>::put(w, m_cSkillLevel);
        Codec<std::remove_cvref_t<decltype(m_cSkillCSPoint)>>::put(w, m_cSkillCSPoint);
    }

    static KUserSkillData read_from(Reader& r) {
        KUserSkillData v;
        v.m_iSkillID = Codec<std::remove_cvref_t<decltype(v.m_iSkillID)>>::get(r);
        v.m_cSkillLevel = Codec<std::remove_cvref_t<decltype(v.m_cSkillLevel)>>::get(r);
        v.m_cSkillCSPoint = Codec<std::remove_cvref_t<decltype(v.m_cSkillCSPoint)>>::get(r);
        return v;
    }
};

struct KRetrievedSkillPageData {
    std::int32_t                m_iRetrievedSPoint{};  // int
    std::vector<KUserSkillData> m_vecUserSkillData{};  // std::vector< KUserSkillData >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iRetrievedSPoint)>>::put(w, m_iRetrievedSPoint);
        Codec<std::remove_cvref_t<decltype(m_vecUserSkillData)>>::put(w, m_vecUserSkillData);
    }

    static KRetrievedSkillPageData read_from(Reader& r) {
        KRetrievedSkillPageData v;
        v.m_iRetrievedSPoint = Codec<std::remove_cvref_t<decltype(v.m_iRetrievedSPoint)>>::get(r);
        v.m_vecUserSkillData = Codec<std::remove_cvref_t<decltype(v.m_vecUserSkillData)>>::get(r);
        return v;
    }
};

struct KDBE_EXPIRE_CASH_SKILL_POINT_REQ {
    std::int64_t                         m_iUnitUID{};  // UidType
    std::vector<KRetrievedSkillPageData> m_vecRetrievedSkillPageData{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_vecRetrievedSkillPageData)>>::put(w, m_vecRetrievedSkillPageData);
    }

    static KDBE_EXPIRE_CASH_SKILL_POINT_REQ read_from(Reader& r) {
        KDBE_EXPIRE_CASH_SKILL_POINT_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_vecRetrievedSkillPageData = Codec<std::remove_cvref_t<decltype(v.m_vecRetrievedSkillPageData)>>::get(r);
        return v;
    }
};

struct KUserSkillPageData {
    std::array<KSkillData, 4>   m_aEquippedSkill{};  // KSkillData[4]
    std::array<KSkillData, 4>   m_aEquippedSkillSlotB{};  // KSkillData[4]
    std::uint16_t               m_usSkillPoint{};  // USHORT
    std::uint16_t               m_usCashSkillPoint{};  // USHORT
    std::vector<KUserSkillData> m_vecUserSkillData{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_aEquippedSkill[0])>>::put(w, m_aEquippedSkill[0]);
        Codec<std::remove_cvref_t<decltype(m_aEquippedSkill[1])>>::put(w, m_aEquippedSkill[1]);
        Codec<std::remove_cvref_t<decltype(m_aEquippedSkill[2])>>::put(w, m_aEquippedSkill[2]);
        Codec<std::remove_cvref_t<decltype(m_aEquippedSkill[3])>>::put(w, m_aEquippedSkill[3]);
        Codec<std::remove_cvref_t<decltype(m_aEquippedSkillSlotB[0])>>::put(w, m_aEquippedSkillSlotB[0]);
        Codec<std::remove_cvref_t<decltype(m_aEquippedSkillSlotB[1])>>::put(w, m_aEquippedSkillSlotB[1]);
        Codec<std::remove_cvref_t<decltype(m_aEquippedSkillSlotB[2])>>::put(w, m_aEquippedSkillSlotB[2]);
        Codec<std::remove_cvref_t<decltype(m_aEquippedSkillSlotB[3])>>::put(w, m_aEquippedSkillSlotB[3]);
        Codec<std::remove_cvref_t<decltype(m_usSkillPoint)>>::put(w, m_usSkillPoint);
        Codec<std::remove_cvref_t<decltype(m_usCashSkillPoint)>>::put(w, m_usCashSkillPoint);
        Codec<std::remove_cvref_t<decltype(m_vecUserSkillData)>>::put(w, m_vecUserSkillData);
    }

    static KUserSkillPageData read_from(Reader& r) {
        KUserSkillPageData v;
        v.m_aEquippedSkill[0] = Codec<std::remove_cvref_t<decltype(v.m_aEquippedSkill[0])>>::get(r);
        v.m_aEquippedSkill[1] = Codec<std::remove_cvref_t<decltype(v.m_aEquippedSkill[1])>>::get(r);
        v.m_aEquippedSkill[2] = Codec<std::remove_cvref_t<decltype(v.m_aEquippedSkill[2])>>::get(r);
        v.m_aEquippedSkill[3] = Codec<std::remove_cvref_t<decltype(v.m_aEquippedSkill[3])>>::get(r);
        v.m_aEquippedSkillSlotB[0] = Codec<std::remove_cvref_t<decltype(v.m_aEquippedSkillSlotB[0])>>::get(r);
        v.m_aEquippedSkillSlotB[1] = Codec<std::remove_cvref_t<decltype(v.m_aEquippedSkillSlotB[1])>>::get(r);
        v.m_aEquippedSkillSlotB[2] = Codec<std::remove_cvref_t<decltype(v.m_aEquippedSkillSlotB[2])>>::get(r);
        v.m_aEquippedSkillSlotB[3] = Codec<std::remove_cvref_t<decltype(v.m_aEquippedSkillSlotB[3])>>::get(r);
        v.m_usSkillPoint = Codec<std::remove_cvref_t<decltype(v.m_usSkillPoint)>>::get(r);
        v.m_usCashSkillPoint = Codec<std::remove_cvref_t<decltype(v.m_usCashSkillPoint)>>::get(r);
        v.m_vecUserSkillData = Codec<std::remove_cvref_t<decltype(v.m_vecUserSkillData)>>::get(r);
        return v;
    }
};

struct KDBE_SELECT_UNIT_ACK {
    std::int32_t                                                       m_iOK{};  // int
    KUnitInfo                                                          m_kUnitInfo{};
    std::u16string                                                     m_wstrUnitCreateDate{};  // std::wstring
    std::int32_t                                                       m_iRealDataED{};  // int
    std::u16string                                                     m_wstrUnitLastLoginDate{};  // std::wstring
    std::map<std::int32_t, std::int32_t>                               m_mapInventorySlotSize{};  // std::map< int, int >
    std::map<std::int64_t, KInventoryItemInfo>                         m_mapItem{};  // std::map< UidType, KInventoryItemInfo >
    std::map<std::int64_t, std::map<std::int64_t, KInventoryItemInfo>> m_mapPetItem{};  // std::map< UidType, std::map< UidType, KInventoryItemInfo > >
    std::int32_t                                                       m_iNumResurrectionStone{};  // int
    std::int32_t                                                       m_iNumAutoPaymentResStone{};  // int
    std::u16string                                                     m_wstrAutoPayResStoneLastDate{};  // std::wstring
    std::vector<KQuestInstance>                                        m_vecQuest{};  // std::vector< KQuestInstance >
    std::vector<KCompleteQuestInfo>                                    m_vecCompletQuest{};  // std::vector< KCompleteQuestInfo >
    std::vector<KMissionInstance>                                      m_vecMission{};  // std::vector< KMissionInstance >
    std::vector<KTitleInfo>                                            m_vecTitle{};  // std::vector< KTitleInfo >
    std::vector<KUserSkillPageData>                                    m_vecUserSkillPageData{};
    std::vector<std::int16_t>                                          m_vecSkillUnsealed{};  // std::vector< short >
    char                                                               m_cSkillNoteMaxPageNum{};
    std::map<char, std::int32_t>                                       m_mapSkillNote{};  // std::map< char, int >
    std::vector<KChatBlackListUnit>                                    m_vecChatBlackList{};  // std::vector< KChatBlackListUnit >
    KDenyOptions                                                       m_kDenyOptions{};
    bool                                                               m_bIsRecommend{};
    std::int64_t                                                       m_iRecommendUnitUID{};  // UidType
    std::vector<KTutorialDBUnitInfo>                                   m_vecTutorialDBUnitInfo{};  // std::vector< KTutorialDBUnitInfo >
    bool                                                               m_bIsSpiritUpdated{};
    std::vector<KConnectTimeEventInfo>                                 m_vecConnectTimeEvent{};  // std::vector< KConnectTimeEventInfo >
    std::vector<KCumulativeTimeEventInfo>                              m_vecCumulativeTimeEvent{};  // std::vector< KCumulativeTimeEventInfo >
    std::vector<KHenirRankingInfo>                                     m_vecHenirRanking{};  // std::vector< KHenirRankingInfo >
    KMessengerInfo                                                     m_kMessengerInfo{};
    std::int64_t                                                       m_iSummonedPetUID{};  // UidType
    std::vector<KPetInfo>                                              m_vecPetList{};  // std::vector< KPetInfo >
    std::vector<std::int64_t>                                          m_vecCanEvolutionPetList{};  // std::vector< UidType >
    std::vector<std::int64_t>                                          m_vecNeverSummonPetList{};  // std::vector< UidType >
    std::map<std::int32_t, std::int32_t>                               m_mapCharGameCount{};  // std::map< int, int >
    KUserPShopAgencyInfo                                               m_kPShopAgencyInfo{};
    KEGS_HENIR_REWARD_COUNT_NOT                                        m_PacketHenirRewardCount{};
    std::map<std::int32_t, std::int32_t>                               m_mapRandomQuestList{};  // std::map< int, int >
    bool                                                               m_bCashShopOpen{};
    KGamePlayStatusContainer                                           m_kGamePlayStatus{};
    std::vector<KQuestInstance>                                        m_vecAccountQuesting{};  // std::vector< KQuestInstance >
    std::vector<KCompleteQuestInfo>                                    m_vecAccountCompleteQuest{};  // std::vector< KCompleteQuestInfo >
    bool                                                               m_bFirstSelect{};
    std::vector<std::int32_t>                                          m_vecAdvertisementEvent{};  // std::vector< int >
    KRelationshipInfo                                                  m_kRelationshipInfo{};
    std::int64_t                                                       m_nWrongUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_kUnitInfo)>>::put(w, m_kUnitInfo);
        Codec<std::remove_cvref_t<decltype(m_wstrUnitCreateDate)>>::put(w, m_wstrUnitCreateDate);
        Codec<std::remove_cvref_t<decltype(m_iRealDataED)>>::put(w, m_iRealDataED);
        Codec<std::remove_cvref_t<decltype(m_wstrUnitLastLoginDate)>>::put(w, m_wstrUnitLastLoginDate);
        Codec<std::remove_cvref_t<decltype(m_mapInventorySlotSize)>>::put(w, m_mapInventorySlotSize);
        Codec<std::remove_cvref_t<decltype(m_mapItem)>>::put(w, m_mapItem);
        Codec<std::remove_cvref_t<decltype(m_mapPetItem)>>::put(w, m_mapPetItem);
        Codec<std::remove_cvref_t<decltype(m_iNumResurrectionStone)>>::put(w, m_iNumResurrectionStone);
        Codec<std::remove_cvref_t<decltype(m_iNumAutoPaymentResStone)>>::put(w, m_iNumAutoPaymentResStone);
        Codec<std::remove_cvref_t<decltype(m_wstrAutoPayResStoneLastDate)>>::put(w, m_wstrAutoPayResStoneLastDate);
        Codec<std::remove_cvref_t<decltype(m_vecQuest)>>::put(w, m_vecQuest);
        Codec<std::remove_cvref_t<decltype(m_vecCompletQuest)>>::put(w, m_vecCompletQuest);
        Codec<std::remove_cvref_t<decltype(m_vecMission)>>::put(w, m_vecMission);
        Codec<std::remove_cvref_t<decltype(m_vecTitle)>>::put(w, m_vecTitle);
        Codec<std::remove_cvref_t<decltype(m_vecUserSkillPageData)>>::put(w, m_vecUserSkillPageData);
        Codec<std::remove_cvref_t<decltype(m_vecSkillUnsealed)>>::put(w, m_vecSkillUnsealed);
        Codec<std::remove_cvref_t<decltype(m_cSkillNoteMaxPageNum)>>::put(w, m_cSkillNoteMaxPageNum);
        Codec<std::remove_cvref_t<decltype(m_mapSkillNote)>>::put(w, m_mapSkillNote);
        Codec<std::remove_cvref_t<decltype(m_vecChatBlackList)>>::put(w, m_vecChatBlackList);
        Codec<std::remove_cvref_t<decltype(m_kDenyOptions)>>::put(w, m_kDenyOptions);
        Codec<std::remove_cvref_t<decltype(m_bIsRecommend)>>::put(w, m_bIsRecommend);
        Codec<std::remove_cvref_t<decltype(m_iRecommendUnitUID)>>::put(w, m_iRecommendUnitUID);
        Codec<std::remove_cvref_t<decltype(m_vecTutorialDBUnitInfo)>>::put(w, m_vecTutorialDBUnitInfo);
        Codec<std::remove_cvref_t<decltype(m_bIsSpiritUpdated)>>::put(w, m_bIsSpiritUpdated);
        Codec<std::remove_cvref_t<decltype(m_vecConnectTimeEvent)>>::put(w, m_vecConnectTimeEvent);
        Codec<std::remove_cvref_t<decltype(m_vecCumulativeTimeEvent)>>::put(w, m_vecCumulativeTimeEvent);
        Codec<std::remove_cvref_t<decltype(m_vecHenirRanking)>>::put(w, m_vecHenirRanking);
        Codec<std::remove_cvref_t<decltype(m_kMessengerInfo)>>::put(w, m_kMessengerInfo);
        Codec<std::remove_cvref_t<decltype(m_iSummonedPetUID)>>::put(w, m_iSummonedPetUID);
        Codec<std::remove_cvref_t<decltype(m_vecPetList)>>::put(w, m_vecPetList);
        Codec<std::remove_cvref_t<decltype(m_vecCanEvolutionPetList)>>::put(w, m_vecCanEvolutionPetList);
        Codec<std::remove_cvref_t<decltype(m_vecNeverSummonPetList)>>::put(w, m_vecNeverSummonPetList);
        Codec<std::remove_cvref_t<decltype(m_mapCharGameCount)>>::put(w, m_mapCharGameCount);
        Codec<std::remove_cvref_t<decltype(m_kPShopAgencyInfo)>>::put(w, m_kPShopAgencyInfo);
        Codec<std::remove_cvref_t<decltype(m_PacketHenirRewardCount)>>::put(w, m_PacketHenirRewardCount);
        Codec<std::remove_cvref_t<decltype(m_mapRandomQuestList)>>::put(w, m_mapRandomQuestList);
        Codec<std::remove_cvref_t<decltype(m_bCashShopOpen)>>::put(w, m_bCashShopOpen);
        Codec<std::remove_cvref_t<decltype(m_kGamePlayStatus)>>::put(w, m_kGamePlayStatus);
        Codec<std::remove_cvref_t<decltype(m_vecAccountQuesting)>>::put(w, m_vecAccountQuesting);
        Codec<std::remove_cvref_t<decltype(m_vecAccountCompleteQuest)>>::put(w, m_vecAccountCompleteQuest);
        Codec<std::remove_cvref_t<decltype(m_bFirstSelect)>>::put(w, m_bFirstSelect);
        Codec<std::remove_cvref_t<decltype(m_vecAdvertisementEvent)>>::put(w, m_vecAdvertisementEvent);
        Codec<std::remove_cvref_t<decltype(m_kRelationshipInfo)>>::put(w, m_kRelationshipInfo);
        Codec<std::remove_cvref_t<decltype(m_nWrongUnitUID)>>::put(w, m_nWrongUnitUID);
    }

    static KDBE_SELECT_UNIT_ACK read_from(Reader& r) {
        KDBE_SELECT_UNIT_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_kUnitInfo = Codec<std::remove_cvref_t<decltype(v.m_kUnitInfo)>>::get(r);
        v.m_wstrUnitCreateDate = Codec<std::remove_cvref_t<decltype(v.m_wstrUnitCreateDate)>>::get(r);
        v.m_iRealDataED = Codec<std::remove_cvref_t<decltype(v.m_iRealDataED)>>::get(r);
        v.m_wstrUnitLastLoginDate = Codec<std::remove_cvref_t<decltype(v.m_wstrUnitLastLoginDate)>>::get(r);
        v.m_mapInventorySlotSize = Codec<std::remove_cvref_t<decltype(v.m_mapInventorySlotSize)>>::get(r);
        v.m_mapItem = Codec<std::remove_cvref_t<decltype(v.m_mapItem)>>::get(r);
        v.m_mapPetItem = Codec<std::remove_cvref_t<decltype(v.m_mapPetItem)>>::get(r);
        v.m_iNumResurrectionStone = Codec<std::remove_cvref_t<decltype(v.m_iNumResurrectionStone)>>::get(r);
        v.m_iNumAutoPaymentResStone = Codec<std::remove_cvref_t<decltype(v.m_iNumAutoPaymentResStone)>>::get(r);
        v.m_wstrAutoPayResStoneLastDate = Codec<std::remove_cvref_t<decltype(v.m_wstrAutoPayResStoneLastDate)>>::get(r);
        v.m_vecQuest = Codec<std::remove_cvref_t<decltype(v.m_vecQuest)>>::get(r);
        v.m_vecCompletQuest = Codec<std::remove_cvref_t<decltype(v.m_vecCompletQuest)>>::get(r);
        v.m_vecMission = Codec<std::remove_cvref_t<decltype(v.m_vecMission)>>::get(r);
        v.m_vecTitle = Codec<std::remove_cvref_t<decltype(v.m_vecTitle)>>::get(r);
        v.m_vecUserSkillPageData = Codec<std::remove_cvref_t<decltype(v.m_vecUserSkillPageData)>>::get(r);
        v.m_vecSkillUnsealed = Codec<std::remove_cvref_t<decltype(v.m_vecSkillUnsealed)>>::get(r);
        v.m_cSkillNoteMaxPageNum = Codec<std::remove_cvref_t<decltype(v.m_cSkillNoteMaxPageNum)>>::get(r);
        v.m_mapSkillNote = Codec<std::remove_cvref_t<decltype(v.m_mapSkillNote)>>::get(r);
        v.m_vecChatBlackList = Codec<std::remove_cvref_t<decltype(v.m_vecChatBlackList)>>::get(r);
        v.m_kDenyOptions = Codec<std::remove_cvref_t<decltype(v.m_kDenyOptions)>>::get(r);
        v.m_bIsRecommend = Codec<std::remove_cvref_t<decltype(v.m_bIsRecommend)>>::get(r);
        v.m_iRecommendUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iRecommendUnitUID)>>::get(r);
        v.m_vecTutorialDBUnitInfo = Codec<std::remove_cvref_t<decltype(v.m_vecTutorialDBUnitInfo)>>::get(r);
        v.m_bIsSpiritUpdated = Codec<std::remove_cvref_t<decltype(v.m_bIsSpiritUpdated)>>::get(r);
        v.m_vecConnectTimeEvent = Codec<std::remove_cvref_t<decltype(v.m_vecConnectTimeEvent)>>::get(r);
        v.m_vecCumulativeTimeEvent = Codec<std::remove_cvref_t<decltype(v.m_vecCumulativeTimeEvent)>>::get(r);
        v.m_vecHenirRanking = Codec<std::remove_cvref_t<decltype(v.m_vecHenirRanking)>>::get(r);
        v.m_kMessengerInfo = Codec<std::remove_cvref_t<decltype(v.m_kMessengerInfo)>>::get(r);
        v.m_iSummonedPetUID = Codec<std::remove_cvref_t<decltype(v.m_iSummonedPetUID)>>::get(r);
        v.m_vecPetList = Codec<std::remove_cvref_t<decltype(v.m_vecPetList)>>::get(r);
        v.m_vecCanEvolutionPetList = Codec<std::remove_cvref_t<decltype(v.m_vecCanEvolutionPetList)>>::get(r);
        v.m_vecNeverSummonPetList = Codec<std::remove_cvref_t<decltype(v.m_vecNeverSummonPetList)>>::get(r);
        v.m_mapCharGameCount = Codec<std::remove_cvref_t<decltype(v.m_mapCharGameCount)>>::get(r);
        v.m_kPShopAgencyInfo = Codec<std::remove_cvref_t<decltype(v.m_kPShopAgencyInfo)>>::get(r);
        v.m_PacketHenirRewardCount = Codec<std::remove_cvref_t<decltype(v.m_PacketHenirRewardCount)>>::get(r);
        v.m_mapRandomQuestList = Codec<std::remove_cvref_t<decltype(v.m_mapRandomQuestList)>>::get(r);
        v.m_bCashShopOpen = Codec<std::remove_cvref_t<decltype(v.m_bCashShopOpen)>>::get(r);
        v.m_kGamePlayStatus = Codec<std::remove_cvref_t<decltype(v.m_kGamePlayStatus)>>::get(r);
        v.m_vecAccountQuesting = Codec<std::remove_cvref_t<decltype(v.m_vecAccountQuesting)>>::get(r);
        v.m_vecAccountCompleteQuest = Codec<std::remove_cvref_t<decltype(v.m_vecAccountCompleteQuest)>>::get(r);
        v.m_bFirstSelect = Codec<std::remove_cvref_t<decltype(v.m_bFirstSelect)>>::get(r);
        v.m_vecAdvertisementEvent = Codec<std::remove_cvref_t<decltype(v.m_vecAdvertisementEvent)>>::get(r);
        v.m_kRelationshipInfo = Codec<std::remove_cvref_t<decltype(v.m_kRelationshipInfo)>>::get(r);
        v.m_nWrongUnitUID = Codec<std::remove_cvref_t<decltype(v.m_nWrongUnitUID)>>::get(r);
        return v;
    }
};

struct KDBE_CHANNEL_CHANGE_GAME_SELECT_UNIT_ACK {
    KEGS_CONNECT_ACK        m_kConnectAck{};
    KEGS_VERIFY_ACCOUNT_ACK m_kVerifyAccountAck{};
    KDBE_SELECT_UNIT_ACK    m_kSelectUnitAck{};
    KChannelChangeInfo      m_kChangeUserInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kConnectAck)>>::put(w, m_kConnectAck);
        Codec<std::remove_cvref_t<decltype(m_kVerifyAccountAck)>>::put(w, m_kVerifyAccountAck);
        Codec<std::remove_cvref_t<decltype(m_kSelectUnitAck)>>::put(w, m_kSelectUnitAck);
        Codec<std::remove_cvref_t<decltype(m_kChangeUserInfo)>>::put(w, m_kChangeUserInfo);
    }

    static KDBE_CHANNEL_CHANGE_GAME_SELECT_UNIT_ACK read_from(Reader& r) {
        KDBE_CHANNEL_CHANGE_GAME_SELECT_UNIT_ACK v;
        v.m_kConnectAck = Codec<std::remove_cvref_t<decltype(v.m_kConnectAck)>>::get(r);
        v.m_kVerifyAccountAck = Codec<std::remove_cvref_t<decltype(v.m_kVerifyAccountAck)>>::get(r);
        v.m_kSelectUnitAck = Codec<std::remove_cvref_t<decltype(v.m_kSelectUnitAck)>>::get(r);
        v.m_kChangeUserInfo = Codec<std::remove_cvref_t<decltype(v.m_kChangeUserInfo)>>::get(r);
        return v;
    }
};

struct KEGS_DECIDE_TO_USE_THIS_SKILL_PAGE_ACK {
    std::int32_t       m_iOK{};  // int
    std::int32_t       m_iSkillPagesNumberDecidedToUse{};  // int
    std::int32_t       m_iSPointAvailable{};  // int
    std::int32_t       m_iCSPointAvailable{};  // int
    KUserSkillPageData m_kUserSkillPageData{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iSkillPagesNumberDecidedToUse)>>::put(w, m_iSkillPagesNumberDecidedToUse);
        Codec<std::remove_cvref_t<decltype(m_iSPointAvailable)>>::put(w, m_iSPointAvailable);
        Codec<std::remove_cvref_t<decltype(m_iCSPointAvailable)>>::put(w, m_iCSPointAvailable);
        Codec<std::remove_cvref_t<decltype(m_kUserSkillPageData)>>::put(w, m_kUserSkillPageData);
    }

    static KEGS_DECIDE_TO_USE_THIS_SKILL_PAGE_ACK read_from(Reader& r) {
        KEGS_DECIDE_TO_USE_THIS_SKILL_PAGE_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iSkillPagesNumberDecidedToUse = Codec<std::remove_cvref_t<decltype(v.m_iSkillPagesNumberDecidedToUse)>>::get(r);
        v.m_iSPointAvailable = Codec<std::remove_cvref_t<decltype(v.m_iSPointAvailable)>>::get(r);
        v.m_iCSPointAvailable = Codec<std::remove_cvref_t<decltype(v.m_iCSPointAvailable)>>::get(r);
        v.m_kUserSkillPageData = Codec<std::remove_cvref_t<decltype(v.m_kUserSkillPageData)>>::get(r);
        return v;
    }
};

struct KEGS_SELECT_UNIT_1_NOT {
    std::int32_t                               m_iOK{};  // int
    KUnitInfo                                  m_kUnitInfo{};
    std::u16string                             m_wstrUnitCreateDate{};  // std::wstring
    std::int32_t                               m_iRealDataED{};  // int
    std::u16string                             m_wstrUnitLastLoginDate{};  // std::wstring
    std::map<std::int32_t, std::int32_t>       m_mapInventorySlotSize{};  // std::map< int, int >
    std::map<std::int64_t, KInventoryItemInfo> m_mapItem{};  // std::map< UidType, KInventoryItemInfo >
    std::int32_t                               m_iNumResurrectionStone{};  // int
    std::int32_t                               m_iNumAutoPaymentResStone{};  // int
    std::u16string                             m_wstrAutoPayResStoneLastDate{};  // std::wstring
    std::vector<KUserSkillPageData>            m_vecUserSkillPageData{};  // std::vector< KUserSkillPageData >
    std::vector<std::int16_t>                  m_vecSkillUnsealed{};  // std::vector< short >
    char                                       m_cSkillNoteMaxPageNum{};
    std::map<char, std::int32_t>               m_mapSkillNote{};  // std::map< char, int >
    bool                                       m_bIsNewUnitTradeBlock{};
    std::int32_t                               m_iNewUnitTradeBlockDay{};  // int
    std::int32_t                               m_iNewUnitTradeBlockUnitClass{};  // int
    KRelationshipInfo                          m_kRelationshipInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_kUnitInfo)>>::put(w, m_kUnitInfo);
        Codec<std::remove_cvref_t<decltype(m_wstrUnitCreateDate)>>::put(w, m_wstrUnitCreateDate);
        Codec<std::remove_cvref_t<decltype(m_iRealDataED)>>::put(w, m_iRealDataED);
        Codec<std::remove_cvref_t<decltype(m_wstrUnitLastLoginDate)>>::put(w, m_wstrUnitLastLoginDate);
        Codec<std::remove_cvref_t<decltype(m_mapInventorySlotSize)>>::put(w, m_mapInventorySlotSize);
        Codec<std::remove_cvref_t<decltype(m_mapItem)>>::put(w, m_mapItem);
        Codec<std::remove_cvref_t<decltype(m_iNumResurrectionStone)>>::put(w, m_iNumResurrectionStone);
        Codec<std::remove_cvref_t<decltype(m_iNumAutoPaymentResStone)>>::put(w, m_iNumAutoPaymentResStone);
        Codec<std::remove_cvref_t<decltype(m_wstrAutoPayResStoneLastDate)>>::put(w, m_wstrAutoPayResStoneLastDate);
        Codec<std::remove_cvref_t<decltype(m_vecUserSkillPageData)>>::put(w, m_vecUserSkillPageData);
        Codec<std::remove_cvref_t<decltype(m_vecSkillUnsealed)>>::put(w, m_vecSkillUnsealed);
        Codec<std::remove_cvref_t<decltype(m_cSkillNoteMaxPageNum)>>::put(w, m_cSkillNoteMaxPageNum);
        Codec<std::remove_cvref_t<decltype(m_mapSkillNote)>>::put(w, m_mapSkillNote);
        Codec<std::remove_cvref_t<decltype(m_bIsNewUnitTradeBlock)>>::put(w, m_bIsNewUnitTradeBlock);
        Codec<std::remove_cvref_t<decltype(m_iNewUnitTradeBlockDay)>>::put(w, m_iNewUnitTradeBlockDay);
        Codec<std::remove_cvref_t<decltype(m_iNewUnitTradeBlockUnitClass)>>::put(w, m_iNewUnitTradeBlockUnitClass);
        Codec<std::remove_cvref_t<decltype(m_kRelationshipInfo)>>::put(w, m_kRelationshipInfo);
    }

    static KEGS_SELECT_UNIT_1_NOT read_from(Reader& r) {
        KEGS_SELECT_UNIT_1_NOT v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_kUnitInfo = Codec<std::remove_cvref_t<decltype(v.m_kUnitInfo)>>::get(r);
        v.m_wstrUnitCreateDate = Codec<std::remove_cvref_t<decltype(v.m_wstrUnitCreateDate)>>::get(r);
        v.m_iRealDataED = Codec<std::remove_cvref_t<decltype(v.m_iRealDataED)>>::get(r);
        v.m_wstrUnitLastLoginDate = Codec<std::remove_cvref_t<decltype(v.m_wstrUnitLastLoginDate)>>::get(r);
        v.m_mapInventorySlotSize = Codec<std::remove_cvref_t<decltype(v.m_mapInventorySlotSize)>>::get(r);
        v.m_mapItem = Codec<std::remove_cvref_t<decltype(v.m_mapItem)>>::get(r);
        v.m_iNumResurrectionStone = Codec<std::remove_cvref_t<decltype(v.m_iNumResurrectionStone)>>::get(r);
        v.m_iNumAutoPaymentResStone = Codec<std::remove_cvref_t<decltype(v.m_iNumAutoPaymentResStone)>>::get(r);
        v.m_wstrAutoPayResStoneLastDate = Codec<std::remove_cvref_t<decltype(v.m_wstrAutoPayResStoneLastDate)>>::get(r);
        v.m_vecUserSkillPageData = Codec<std::remove_cvref_t<decltype(v.m_vecUserSkillPageData)>>::get(r);
        v.m_vecSkillUnsealed = Codec<std::remove_cvref_t<decltype(v.m_vecSkillUnsealed)>>::get(r);
        v.m_cSkillNoteMaxPageNum = Codec<std::remove_cvref_t<decltype(v.m_cSkillNoteMaxPageNum)>>::get(r);
        v.m_mapSkillNote = Codec<std::remove_cvref_t<decltype(v.m_mapSkillNote)>>::get(r);
        v.m_bIsNewUnitTradeBlock = Codec<std::remove_cvref_t<decltype(v.m_bIsNewUnitTradeBlock)>>::get(r);
        v.m_iNewUnitTradeBlockDay = Codec<std::remove_cvref_t<decltype(v.m_iNewUnitTradeBlockDay)>>::get(r);
        v.m_iNewUnitTradeBlockUnitClass = Codec<std::remove_cvref_t<decltype(v.m_iNewUnitTradeBlockUnitClass)>>::get(r);
        v.m_kRelationshipInfo = Codec<std::remove_cvref_t<decltype(v.m_kRelationshipInfo)>>::get(r);
        return v;
    }
};

struct KWeddingHallInfo {
    std::int32_t   m_iWeddingUID{};  // int
    char           m_cWeddingHallType{};
    char           m_cOfficiantNPC{};
    std::int64_t   m_iGroom{};  // UidType
    std::int64_t   m_iBride{};  // UidType
    std::u16string m_wstrWeddingDate{};  // std::wstring
    std::int64_t   m_tWeddingDate{};  // __int64
    std::u16string m_wstrWeddingMsg{};  // std::wstring
    std::int64_t   m_iRoomUID{};  // UidType
    bool           m_bSuccess{};
    bool           m_bDelete{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iWeddingUID)>>::put(w, m_iWeddingUID);
        Codec<std::remove_cvref_t<decltype(m_cWeddingHallType)>>::put(w, m_cWeddingHallType);
        Codec<std::remove_cvref_t<decltype(m_cOfficiantNPC)>>::put(w, m_cOfficiantNPC);
        Codec<std::remove_cvref_t<decltype(m_iGroom)>>::put(w, m_iGroom);
        Codec<std::remove_cvref_t<decltype(m_iBride)>>::put(w, m_iBride);
        Codec<std::remove_cvref_t<decltype(m_wstrWeddingDate)>>::put(w, m_wstrWeddingDate);
        Codec<std::remove_cvref_t<decltype(m_tWeddingDate)>>::put(w, m_tWeddingDate);
        Codec<std::remove_cvref_t<decltype(m_wstrWeddingMsg)>>::put(w, m_wstrWeddingMsg);
        Codec<std::remove_cvref_t<decltype(m_iRoomUID)>>::put(w, m_iRoomUID);
        Codec<std::remove_cvref_t<decltype(m_bSuccess)>>::put(w, m_bSuccess);
        Codec<std::remove_cvref_t<decltype(m_bDelete)>>::put(w, m_bDelete);
    }

    static KWeddingHallInfo read_from(Reader& r) {
        KWeddingHallInfo v;
        v.m_iWeddingUID = Codec<std::remove_cvref_t<decltype(v.m_iWeddingUID)>>::get(r);
        v.m_cWeddingHallType = Codec<std::remove_cvref_t<decltype(v.m_cWeddingHallType)>>::get(r);
        v.m_cOfficiantNPC = Codec<std::remove_cvref_t<decltype(v.m_cOfficiantNPC)>>::get(r);
        v.m_iGroom = Codec<std::remove_cvref_t<decltype(v.m_iGroom)>>::get(r);
        v.m_iBride = Codec<std::remove_cvref_t<decltype(v.m_iBride)>>::get(r);
        v.m_wstrWeddingDate = Codec<std::remove_cvref_t<decltype(v.m_wstrWeddingDate)>>::get(r);
        v.m_tWeddingDate = Codec<std::remove_cvref_t<decltype(v.m_tWeddingDate)>>::get(r);
        v.m_wstrWeddingMsg = Codec<std::remove_cvref_t<decltype(v.m_wstrWeddingMsg)>>::get(r);
        v.m_iRoomUID = Codec<std::remove_cvref_t<decltype(v.m_iRoomUID)>>::get(r);
        v.m_bSuccess = Codec<std::remove_cvref_t<decltype(v.m_bSuccess)>>::get(r);
        v.m_bDelete = Codec<std::remove_cvref_t<decltype(v.m_bDelete)>>::get(r);
        return v;
    }
};

struct KDBE_LOAD_WEDDING_HALL_INFO_ACK {
    std::int32_t                             m_iOK{};  // int
    std::map<std::int32_t, KWeddingHallInfo> m_mapWeddingInfo{};  // std::map<int, KWeddingHallInfo>

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_mapWeddingInfo)>>::put(w, m_mapWeddingInfo);
    }

    static KDBE_LOAD_WEDDING_HALL_INFO_ACK read_from(Reader& r) {
        KDBE_LOAD_WEDDING_HALL_INFO_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_mapWeddingInfo = Codec<std::remove_cvref_t<decltype(v.m_mapWeddingInfo)>>::get(r);
        return v;
    }
};

struct KDBE_WEDDING_HALL_INFO_UPDATE_NOT {
    std::map<std::int32_t, KWeddingHallInfo> m_mapWeddingInfo{};  // std::map<int, KWeddingHallInfo>

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_mapWeddingInfo)>>::put(w, m_mapWeddingInfo);
    }

    static KDBE_WEDDING_HALL_INFO_UPDATE_NOT read_from(Reader& r) {
        KDBE_WEDDING_HALL_INFO_UPDATE_NOT v;
        v.m_mapWeddingInfo = Codec<std::remove_cvref_t<decltype(v.m_mapWeddingInfo)>>::get(r);
        return v;
    }
};

struct KEGS_JOIN_WEDDING_HALL_ACK {
    std::int32_t               m_iOK{};  // int
    KWeddingHallInfo           m_kWeddingInfo{};
    KRoomInfo                  m_RoomInfo{};
    std::vector<KRoomSlotInfo> m_vecSlot{};  // std::vector< KRoomSlotInfo >
    std::u16string             m_wstrCNIP{};  // std::wstring
    std::u16string             m_wstrUDPRelayIP{};  // std::wstring
    std::uint16_t              m_usUDPRelayPort{};  // USHORT

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_kWeddingInfo)>>::put(w, m_kWeddingInfo);
        Codec<std::remove_cvref_t<decltype(m_RoomInfo)>>::put(w, m_RoomInfo);
        Codec<std::remove_cvref_t<decltype(m_vecSlot)>>::put(w, m_vecSlot);
        Codec<std::remove_cvref_t<decltype(m_wstrCNIP)>>::put(w, m_wstrCNIP);
        Codec<std::remove_cvref_t<decltype(m_wstrUDPRelayIP)>>::put(w, m_wstrUDPRelayIP);
        Codec<std::remove_cvref_t<decltype(m_usUDPRelayPort)>>::put(w, m_usUDPRelayPort);
    }

    static KEGS_JOIN_WEDDING_HALL_ACK read_from(Reader& r) {
        KEGS_JOIN_WEDDING_HALL_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_kWeddingInfo = Codec<std::remove_cvref_t<decltype(v.m_kWeddingInfo)>>::get(r);
        v.m_RoomInfo = Codec<std::remove_cvref_t<decltype(v.m_RoomInfo)>>::get(r);
        v.m_vecSlot = Codec<std::remove_cvref_t<decltype(v.m_vecSlot)>>::get(r);
        v.m_wstrCNIP = Codec<std::remove_cvref_t<decltype(v.m_wstrCNIP)>>::get(r);
        v.m_wstrUDPRelayIP = Codec<std::remove_cvref_t<decltype(v.m_wstrUDPRelayIP)>>::get(r);
        v.m_usUDPRelayPort = Codec<std::remove_cvref_t<decltype(v.m_usUDPRelayPort)>>::get(r);
        return v;
    }
};

struct KELG_ADD_WEDDING_HALL_INFO_NOT {
    KWeddingHallInfo m_kWeddingHallInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kWeddingHallInfo)>>::put(w, m_kWeddingHallInfo);
    }

    static KELG_ADD_WEDDING_HALL_INFO_NOT read_from(Reader& r) {
        KELG_ADD_WEDDING_HALL_INFO_NOT v;
        v.m_kWeddingHallInfo = Codec<std::remove_cvref_t<decltype(v.m_kWeddingHallInfo)>>::get(r);
        return v;
    }
};

struct KELG_CHECK_WEDDING_HALL_INFO_ACK {
    std::int32_t     m_iOK{};  // int
    KWeddingHallInfo m_kWeddingInfo{};
    char             m_cUnitWeddingStatus{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_kWeddingInfo)>>::put(w, m_kWeddingInfo);
        Codec<std::remove_cvref_t<decltype(m_cUnitWeddingStatus)>>::put(w, m_cUnitWeddingStatus);
    }

    static KELG_CHECK_WEDDING_HALL_INFO_ACK read_from(Reader& r) {
        KELG_CHECK_WEDDING_HALL_INFO_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_kWeddingInfo = Codec<std::remove_cvref_t<decltype(v.m_kWeddingInfo)>>::get(r);
        v.m_cUnitWeddingStatus = Codec<std::remove_cvref_t<decltype(v.m_cUnitWeddingStatus)>>::get(r);
        return v;
    }
};

struct KERM_LEAVE_BATTLE_FIELD_FOR_JOIN_WEDDING_HALL_REQ {
    KWeddingHallInfo m_kWeddingInfo{};
    char             m_cUnitWeddingStatus{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kWeddingInfo)>>::put(w, m_kWeddingInfo);
        Codec<std::remove_cvref_t<decltype(m_cUnitWeddingStatus)>>::put(w, m_cUnitWeddingStatus);
    }

    static KERM_LEAVE_BATTLE_FIELD_FOR_JOIN_WEDDING_HALL_REQ read_from(Reader& r) {
        KERM_LEAVE_BATTLE_FIELD_FOR_JOIN_WEDDING_HALL_REQ v;
        v.m_kWeddingInfo = Codec<std::remove_cvref_t<decltype(v.m_kWeddingInfo)>>::get(r);
        v.m_cUnitWeddingStatus = Codec<std::remove_cvref_t<decltype(v.m_cUnitWeddingStatus)>>::get(r);
        return v;
    }
};

struct KERM_OPEN_WEDDING_HALL_ACK {
    std::int32_t               m_iOK{};  // int
    KWeddingHallInfo           m_kWeddingInfo{};
    KRoomInfo                  m_RoomInfo{};
    std::vector<KRoomSlotInfo> m_vecSlot{};  // std::vector< KRoomSlotInfo >
    std::u16string             m_wstrCNIP{};  // std::wstring
    std::u16string             m_wstrUDPRelayIP{};  // std::wstring
    std::uint16_t              m_usUDPRelayPort{};  // USHORT

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_kWeddingInfo)>>::put(w, m_kWeddingInfo);
        Codec<std::remove_cvref_t<decltype(m_RoomInfo)>>::put(w, m_RoomInfo);
        Codec<std::remove_cvref_t<decltype(m_vecSlot)>>::put(w, m_vecSlot);
        Codec<std::remove_cvref_t<decltype(m_wstrCNIP)>>::put(w, m_wstrCNIP);
        Codec<std::remove_cvref_t<decltype(m_wstrUDPRelayIP)>>::put(w, m_wstrUDPRelayIP);
        Codec<std::remove_cvref_t<decltype(m_usUDPRelayPort)>>::put(w, m_usUDPRelayPort);
    }

    static KERM_OPEN_WEDDING_HALL_ACK read_from(Reader& r) {
        KERM_OPEN_WEDDING_HALL_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_kWeddingInfo = Codec<std::remove_cvref_t<decltype(v.m_kWeddingInfo)>>::get(r);
        v.m_RoomInfo = Codec<std::remove_cvref_t<decltype(v.m_RoomInfo)>>::get(r);
        v.m_vecSlot = Codec<std::remove_cvref_t<decltype(v.m_vecSlot)>>::get(r);
        v.m_wstrCNIP = Codec<std::remove_cvref_t<decltype(v.m_wstrCNIP)>>::get(r);
        v.m_wstrUDPRelayIP = Codec<std::remove_cvref_t<decltype(v.m_wstrUDPRelayIP)>>::get(r);
        v.m_usUDPRelayPort = Codec<std::remove_cvref_t<decltype(v.m_usUDPRelayPort)>>::get(r);
        return v;
    }
};

struct KERM_OPEN_WEDDING_HALL_REQ {
    KWeddingHallInfo   m_kWeddingInfo{};
    KRoomUserInfo      m_kRoomUserInfo{};
    KReturnToFieldInfo m_kReturnToFieldInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kWeddingInfo)>>::put(w, m_kWeddingInfo);
        Codec<std::remove_cvref_t<decltype(m_kRoomUserInfo)>>::put(w, m_kRoomUserInfo);
        Codec<std::remove_cvref_t<decltype(m_kReturnToFieldInfo)>>::put(w, m_kReturnToFieldInfo);
    }

    static KERM_OPEN_WEDDING_HALL_REQ read_from(Reader& r) {
        KERM_OPEN_WEDDING_HALL_REQ v;
        v.m_kWeddingInfo = Codec<std::remove_cvref_t<decltype(v.m_kWeddingInfo)>>::get(r);
        v.m_kRoomUserInfo = Codec<std::remove_cvref_t<decltype(v.m_kRoomUserInfo)>>::get(r);
        v.m_kReturnToFieldInfo = Codec<std::remove_cvref_t<decltype(v.m_kReturnToFieldInfo)>>::get(r);
        return v;
    }
};

struct KERM_WEDDING_HALL_INFO_NOT {
    char                      m_cType{};
    KWeddingHallInfo          m_kHallInfo{};
    std::vector<std::int64_t> m_vecJoinUnitList{};  // std::vector< UidType >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cType)>>::put(w, m_cType);
        Codec<std::remove_cvref_t<decltype(m_kHallInfo)>>::put(w, m_kHallInfo);
        Codec<std::remove_cvref_t<decltype(m_vecJoinUnitList)>>::put(w, m_vecJoinUnitList);
    }

    static KERM_WEDDING_HALL_INFO_NOT read_from(Reader& r) {
        KERM_WEDDING_HALL_INFO_NOT v;
        v.m_cType = Codec<std::remove_cvref_t<decltype(v.m_cType)>>::get(r);
        v.m_kHallInfo = Codec<std::remove_cvref_t<decltype(v.m_kHallInfo)>>::get(r);
        v.m_vecJoinUnitList = Codec<std::remove_cvref_t<decltype(v.m_vecJoinUnitList)>>::get(r);
        return v;
    }
};

struct KWeddingItemInfo {
    std::int64_t   m_iItemUID{};  // UidType
    std::int32_t   m_iWeddingUID{};  // int
    char           m_cWeddingHallType{};
    char           m_cOfficiantNPC{};
    std::int64_t   m_iGroom{};  // UidType
    std::int64_t   m_iBride{};  // UidType
    std::u16string m_wstrGroom{};  // std::wstring
    std::u16string m_wstrBride{};  // std::wstring
    std::u16string m_wstrWeddingDate{};  // std::wstring
    std::u16string m_wstrWeddingMsg{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_iWeddingUID)>>::put(w, m_iWeddingUID);
        Codec<std::remove_cvref_t<decltype(m_cWeddingHallType)>>::put(w, m_cWeddingHallType);
        Codec<std::remove_cvref_t<decltype(m_cOfficiantNPC)>>::put(w, m_cOfficiantNPC);
        Codec<std::remove_cvref_t<decltype(m_iGroom)>>::put(w, m_iGroom);
        Codec<std::remove_cvref_t<decltype(m_iBride)>>::put(w, m_iBride);
        Codec<std::remove_cvref_t<decltype(m_wstrGroom)>>::put(w, m_wstrGroom);
        Codec<std::remove_cvref_t<decltype(m_wstrBride)>>::put(w, m_wstrBride);
        Codec<std::remove_cvref_t<decltype(m_wstrWeddingDate)>>::put(w, m_wstrWeddingDate);
        Codec<std::remove_cvref_t<decltype(m_wstrWeddingMsg)>>::put(w, m_wstrWeddingMsg);
    }

    static KWeddingItemInfo read_from(Reader& r) {
        KWeddingItemInfo v;
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_iWeddingUID = Codec<std::remove_cvref_t<decltype(v.m_iWeddingUID)>>::get(r);
        v.m_cWeddingHallType = Codec<std::remove_cvref_t<decltype(v.m_cWeddingHallType)>>::get(r);
        v.m_cOfficiantNPC = Codec<std::remove_cvref_t<decltype(v.m_cOfficiantNPC)>>::get(r);
        v.m_iGroom = Codec<std::remove_cvref_t<decltype(v.m_iGroom)>>::get(r);
        v.m_iBride = Codec<std::remove_cvref_t<decltype(v.m_iBride)>>::get(r);
        v.m_wstrGroom = Codec<std::remove_cvref_t<decltype(v.m_wstrGroom)>>::get(r);
        v.m_wstrBride = Codec<std::remove_cvref_t<decltype(v.m_wstrBride)>>::get(r);
        v.m_wstrWeddingDate = Codec<std::remove_cvref_t<decltype(v.m_wstrWeddingDate)>>::get(r);
        v.m_wstrWeddingMsg = Codec<std::remove_cvref_t<decltype(v.m_wstrWeddingMsg)>>::get(r);
        return v;
    }
};

struct KDBE_WEDDING_ITEM_DELETE_ACK {
    std::int32_t                             m_iOK{};  // int
    std::map<std::int64_t, KWeddingItemInfo> m_mapWeddingItemInfo{};  // std::map< UidType, KWeddingItemInfo >
    std::vector<KInventoryItemInfo>          m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >
    KItemQuantityUpdate                      m_kItemQuantityUpdate{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_mapWeddingItemInfo)>>::put(w, m_mapWeddingItemInfo);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
    }

    static KDBE_WEDDING_ITEM_DELETE_ACK read_from(Reader& r) {
        KDBE_WEDDING_ITEM_DELETE_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_mapWeddingItemInfo = Codec<std::remove_cvref_t<decltype(v.m_mapWeddingItemInfo)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        return v;
    }
};

struct KDBE_WEDDING_ITEM_DELETE_REQ {
    std::int64_t                             m_iUnitUID{};  // UidType
    std::map<std::int64_t, KWeddingItemInfo> m_mapWeddingItemInfo{};  // std::map< UidType, KWeddingItemInfo >
    std::vector<KInventoryItemInfo>          m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >
    std::vector<KDeletedItemInfo>            m_vecDeleted{};  // std::vector< KDeletedItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_mapWeddingItemInfo)>>::put(w, m_mapWeddingItemInfo);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
        Codec<std::remove_cvref_t<decltype(m_vecDeleted)>>::put(w, m_vecDeleted);
    }

    static KDBE_WEDDING_ITEM_DELETE_REQ read_from(Reader& r) {
        KDBE_WEDDING_ITEM_DELETE_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_mapWeddingItemInfo = Codec<std::remove_cvref_t<decltype(v.m_mapWeddingItemInfo)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        v.m_vecDeleted = Codec<std::remove_cvref_t<decltype(v.m_vecDeleted)>>::get(r);
        return v;
    }
};

struct KEGS_WEDDING_ITEM_INFO_ACK {
    std::int32_t                             m_iOK{};  // int
    std::map<std::int64_t, KWeddingItemInfo> m_mapWeddingItemInfo{};  // std::map< UidType, KWeddingItemInfo >
    std::vector<KInventoryItemInfo>          m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_mapWeddingItemInfo)>>::put(w, m_mapWeddingItemInfo);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
    }

    static KEGS_WEDDING_ITEM_INFO_ACK read_from(Reader& r) {
        KEGS_WEDDING_ITEM_INFO_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_mapWeddingItemInfo = Codec<std::remove_cvref_t<decltype(v.m_mapWeddingItemInfo)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        return v;
    }
};

struct KELG_WEDDING_ITEM_FIND_INFO_REQ {
    std::int64_t                             m_iUnitUID{};  // UidType
    std::int32_t                             m_iOK{};  // int
    std::map<std::int64_t, KWeddingItemInfo> m_mapWeddingItemInfo{};  // std::map< UidType, KWeddingItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_mapWeddingItemInfo)>>::put(w, m_mapWeddingItemInfo);
    }

    static KELG_WEDDING_ITEM_FIND_INFO_REQ read_from(Reader& r) {
        KELG_WEDDING_ITEM_FIND_INFO_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_mapWeddingItemInfo = Codec<std::remove_cvref_t<decltype(v.m_mapWeddingItemInfo)>>::get(r);
        return v;
    }
};

struct VECTOR3 {
    float x{};
    float y{};
    float z{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(x)>>::put(w, x);
        Codec<std::remove_cvref_t<decltype(y)>>::put(w, y);
        Codec<std::remove_cvref_t<decltype(z)>>::put(w, z);
    }

    static VECTOR3 read_from(Reader& r) {
        VECTOR3 v;
        v.x = Codec<std::remove_cvref_t<decltype(v.x)>>::get(r);
        v.y = Codec<std::remove_cvref_t<decltype(v.y)>>::get(r);
        v.z = Codec<std::remove_cvref_t<decltype(v.z)>>::get(r);
        return v;
    }
};

struct KEGS_CALL_MY_LOVER_AGREE_ROOM_NOT {
    bool         m_bAgree{};
    std::int32_t m_iBattleFieldID{};  // int
    std::int64_t m_RoomUID{};  // UidType
    VECTOR3      m_vPos{};
    std::uint8_t m_LastTouchLineIndex{};  // unsigned char
    std::int64_t m_iCalledUserUID{};  // UidType
    std::int64_t m_iCalledGSUID{};  // UidType
    char         m_cCallType{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bAgree)>>::put(w, m_bAgree);
        Codec<std::remove_cvref_t<decltype(m_iBattleFieldID)>>::put(w, m_iBattleFieldID);
        Codec<std::remove_cvref_t<decltype(m_RoomUID)>>::put(w, m_RoomUID);
        Codec<std::remove_cvref_t<decltype(m_vPos)>>::put(w, m_vPos);
        Codec<std::remove_cvref_t<decltype(m_LastTouchLineIndex)>>::put(w, m_LastTouchLineIndex);
        Codec<std::remove_cvref_t<decltype(m_iCalledUserUID)>>::put(w, m_iCalledUserUID);
        Codec<std::remove_cvref_t<decltype(m_iCalledGSUID)>>::put(w, m_iCalledGSUID);
        Codec<std::remove_cvref_t<decltype(m_cCallType)>>::put(w, m_cCallType);
    }

    static KEGS_CALL_MY_LOVER_AGREE_ROOM_NOT read_from(Reader& r) {
        KEGS_CALL_MY_LOVER_AGREE_ROOM_NOT v;
        v.m_bAgree = Codec<std::remove_cvref_t<decltype(v.m_bAgree)>>::get(r);
        v.m_iBattleFieldID = Codec<std::remove_cvref_t<decltype(v.m_iBattleFieldID)>>::get(r);
        v.m_RoomUID = Codec<std::remove_cvref_t<decltype(v.m_RoomUID)>>::get(r);
        v.m_vPos = Codec<std::remove_cvref_t<decltype(v.m_vPos)>>::get(r);
        v.m_LastTouchLineIndex = Codec<std::remove_cvref_t<decltype(v.m_LastTouchLineIndex)>>::get(r);
        v.m_iCalledUserUID = Codec<std::remove_cvref_t<decltype(v.m_iCalledUserUID)>>::get(r);
        v.m_iCalledGSUID = Codec<std::remove_cvref_t<decltype(v.m_iCalledGSUID)>>::get(r);
        v.m_cCallType = Codec<std::remove_cvref_t<decltype(v.m_cCallType)>>::get(r);
        return v;
    }
};

struct KEGS_CALL_MY_LOVER_AGREE_VILLAGE_NOT {
    bool         m_bAgree{};
    std::int32_t m_iMapID{};  // int
    VECTOR3      m_vPos{};
    std::uint8_t m_LastTouchLineIndex{};  // unsigned char
    std::int64_t m_iCalledUserUID{};  // UidType
    std::int64_t m_iCalledGSUID{};  // UidType
    char         m_cCallType{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bAgree)>>::put(w, m_bAgree);
        Codec<std::remove_cvref_t<decltype(m_iMapID)>>::put(w, m_iMapID);
        Codec<std::remove_cvref_t<decltype(m_vPos)>>::put(w, m_vPos);
        Codec<std::remove_cvref_t<decltype(m_LastTouchLineIndex)>>::put(w, m_LastTouchLineIndex);
        Codec<std::remove_cvref_t<decltype(m_iCalledUserUID)>>::put(w, m_iCalledUserUID);
        Codec<std::remove_cvref_t<decltype(m_iCalledGSUID)>>::put(w, m_iCalledGSUID);
        Codec<std::remove_cvref_t<decltype(m_cCallType)>>::put(w, m_cCallType);
    }

    static KEGS_CALL_MY_LOVER_AGREE_VILLAGE_NOT read_from(Reader& r) {
        KEGS_CALL_MY_LOVER_AGREE_VILLAGE_NOT v;
        v.m_bAgree = Codec<std::remove_cvref_t<decltype(v.m_bAgree)>>::get(r);
        v.m_iMapID = Codec<std::remove_cvref_t<decltype(v.m_iMapID)>>::get(r);
        v.m_vPos = Codec<std::remove_cvref_t<decltype(v.m_vPos)>>::get(r);
        v.m_LastTouchLineIndex = Codec<std::remove_cvref_t<decltype(v.m_LastTouchLineIndex)>>::get(r);
        v.m_iCalledUserUID = Codec<std::remove_cvref_t<decltype(v.m_iCalledUserUID)>>::get(r);
        v.m_iCalledGSUID = Codec<std::remove_cvref_t<decltype(v.m_iCalledGSUID)>>::get(r);
        v.m_cCallType = Codec<std::remove_cvref_t<decltype(v.m_cCallType)>>::get(r);
        return v;
    }
};

struct KEGS_CALL_MY_LOVER_JOIN_BATTLE_FIELD_NOT {
    std::int32_t               m_iOK{};  // int
    KBattleFieldJoinInfo       m_kBattleFieldJoinInfo{};
    KRoomInfo                  m_RoomInfo{};
    std::vector<KRoomSlotInfo> m_vecSlot{};  // std::vector< KRoomSlotInfo >
    std::u16string             m_wstrCNIP{};  // std::wstring
    VECTOR3                    m_vPos{};
    std::int32_t               m_iRequireLevel{};  // int
    std::int32_t               m_iRequireDungeonID{};  // int
    std::u16string             m_wstrUDPRelayIP{};  // std::wstring
    std::uint16_t              m_usUDPRelayPort{};  // USHORT
    std::vector<std::int64_t>  m_vecEnterCashShopUser{};  // std::vector< UidType >
    std::int64_t               m_iCalledUserUID{};  // UidType
    std::int64_t               m_iCalledGSUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_kBattleFieldJoinInfo)>>::put(w, m_kBattleFieldJoinInfo);
        Codec<std::remove_cvref_t<decltype(m_RoomInfo)>>::put(w, m_RoomInfo);
        Codec<std::remove_cvref_t<decltype(m_vecSlot)>>::put(w, m_vecSlot);
        Codec<std::remove_cvref_t<decltype(m_wstrCNIP)>>::put(w, m_wstrCNIP);
        Codec<std::remove_cvref_t<decltype(m_vPos)>>::put(w, m_vPos);
        Codec<std::remove_cvref_t<decltype(m_iRequireLevel)>>::put(w, m_iRequireLevel);
        Codec<std::remove_cvref_t<decltype(m_iRequireDungeonID)>>::put(w, m_iRequireDungeonID);
        Codec<std::remove_cvref_t<decltype(m_wstrUDPRelayIP)>>::put(w, m_wstrUDPRelayIP);
        Codec<std::remove_cvref_t<decltype(m_usUDPRelayPort)>>::put(w, m_usUDPRelayPort);
        Codec<std::remove_cvref_t<decltype(m_vecEnterCashShopUser)>>::put(w, m_vecEnterCashShopUser);
        Codec<std::remove_cvref_t<decltype(m_iCalledUserUID)>>::put(w, m_iCalledUserUID);
        Codec<std::remove_cvref_t<decltype(m_iCalledGSUID)>>::put(w, m_iCalledGSUID);
    }

    static KEGS_CALL_MY_LOVER_JOIN_BATTLE_FIELD_NOT read_from(Reader& r) {
        KEGS_CALL_MY_LOVER_JOIN_BATTLE_FIELD_NOT v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_kBattleFieldJoinInfo = Codec<std::remove_cvref_t<decltype(v.m_kBattleFieldJoinInfo)>>::get(r);
        v.m_RoomInfo = Codec<std::remove_cvref_t<decltype(v.m_RoomInfo)>>::get(r);
        v.m_vecSlot = Codec<std::remove_cvref_t<decltype(v.m_vecSlot)>>::get(r);
        v.m_wstrCNIP = Codec<std::remove_cvref_t<decltype(v.m_wstrCNIP)>>::get(r);
        v.m_vPos = Codec<std::remove_cvref_t<decltype(v.m_vPos)>>::get(r);
        v.m_iRequireLevel = Codec<std::remove_cvref_t<decltype(v.m_iRequireLevel)>>::get(r);
        v.m_iRequireDungeonID = Codec<std::remove_cvref_t<decltype(v.m_iRequireDungeonID)>>::get(r);
        v.m_wstrUDPRelayIP = Codec<std::remove_cvref_t<decltype(v.m_wstrUDPRelayIP)>>::get(r);
        v.m_usUDPRelayPort = Codec<std::remove_cvref_t<decltype(v.m_usUDPRelayPort)>>::get(r);
        v.m_vecEnterCashShopUser = Codec<std::remove_cvref_t<decltype(v.m_vecEnterCashShopUser)>>::get(r);
        v.m_iCalledUserUID = Codec<std::remove_cvref_t<decltype(v.m_iCalledUserUID)>>::get(r);
        v.m_iCalledGSUID = Codec<std::remove_cvref_t<decltype(v.m_iCalledGSUID)>>::get(r);
        return v;
    }
};

struct KEGS_CALL_MY_LOVER_REQ {
    std::int32_t m_iMapID{};  // int
    VECTOR3      m_vPos{};
    std::uint8_t m_LastTouchLineIndex{};  // unsigned char

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iMapID)>>::put(w, m_iMapID);
        Codec<std::remove_cvref_t<decltype(m_vPos)>>::put(w, m_vPos);
        Codec<std::remove_cvref_t<decltype(m_LastTouchLineIndex)>>::put(w, m_LastTouchLineIndex);
    }

    static KEGS_CALL_MY_LOVER_REQ read_from(Reader& r) {
        KEGS_CALL_MY_LOVER_REQ v;
        v.m_iMapID = Codec<std::remove_cvref_t<decltype(v.m_iMapID)>>::get(r);
        v.m_vPos = Codec<std::remove_cvref_t<decltype(v.m_vPos)>>::get(r);
        v.m_LastTouchLineIndex = Codec<std::remove_cvref_t<decltype(v.m_LastTouchLineIndex)>>::get(r);
        return v;
    }
};

struct KEGS_CALL_MY_LOVER_ROOM_NOT {
    std::int32_t m_iBattleFieldID{};  // int
    std::int64_t m_RoomUID{};  // UidType
    VECTOR3      m_vPos{};
    std::uint8_t m_LastTouchLineIndex{};  // unsigned char
    std::int64_t m_iCalledUserUID{};  // UidType
    std::int64_t m_iCalledGSUID{};  // UidType
    char         m_cCallType{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iBattleFieldID)>>::put(w, m_iBattleFieldID);
        Codec<std::remove_cvref_t<decltype(m_RoomUID)>>::put(w, m_RoomUID);
        Codec<std::remove_cvref_t<decltype(m_vPos)>>::put(w, m_vPos);
        Codec<std::remove_cvref_t<decltype(m_LastTouchLineIndex)>>::put(w, m_LastTouchLineIndex);
        Codec<std::remove_cvref_t<decltype(m_iCalledUserUID)>>::put(w, m_iCalledUserUID);
        Codec<std::remove_cvref_t<decltype(m_iCalledGSUID)>>::put(w, m_iCalledGSUID);
        Codec<std::remove_cvref_t<decltype(m_cCallType)>>::put(w, m_cCallType);
    }

    static KEGS_CALL_MY_LOVER_ROOM_NOT read_from(Reader& r) {
        KEGS_CALL_MY_LOVER_ROOM_NOT v;
        v.m_iBattleFieldID = Codec<std::remove_cvref_t<decltype(v.m_iBattleFieldID)>>::get(r);
        v.m_RoomUID = Codec<std::remove_cvref_t<decltype(v.m_RoomUID)>>::get(r);
        v.m_vPos = Codec<std::remove_cvref_t<decltype(v.m_vPos)>>::get(r);
        v.m_LastTouchLineIndex = Codec<std::remove_cvref_t<decltype(v.m_LastTouchLineIndex)>>::get(r);
        v.m_iCalledUserUID = Codec<std::remove_cvref_t<decltype(v.m_iCalledUserUID)>>::get(r);
        v.m_iCalledGSUID = Codec<std::remove_cvref_t<decltype(v.m_iCalledGSUID)>>::get(r);
        v.m_cCallType = Codec<std::remove_cvref_t<decltype(v.m_cCallType)>>::get(r);
        return v;
    }
};

struct KEGS_CALL_MY_LOVER_STATE_CHANGE_FIELD_NOT {
    std::int32_t m_iOK{};  // int
    std::int32_t m_iMapID{};  // int
    VECTOR3      m_vPos{};
    std::uint8_t m_LastTouchLineIndex{};  // unsigned char
    std::int32_t m_iLimitLevel{};  // int
    std::int32_t m_iRequireDungeonID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iMapID)>>::put(w, m_iMapID);
        Codec<std::remove_cvref_t<decltype(m_vPos)>>::put(w, m_vPos);
        Codec<std::remove_cvref_t<decltype(m_LastTouchLineIndex)>>::put(w, m_LastTouchLineIndex);
        Codec<std::remove_cvref_t<decltype(m_iLimitLevel)>>::put(w, m_iLimitLevel);
        Codec<std::remove_cvref_t<decltype(m_iRequireDungeonID)>>::put(w, m_iRequireDungeonID);
    }

    static KEGS_CALL_MY_LOVER_STATE_CHANGE_FIELD_NOT read_from(Reader& r) {
        KEGS_CALL_MY_LOVER_STATE_CHANGE_FIELD_NOT v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iMapID = Codec<std::remove_cvref_t<decltype(v.m_iMapID)>>::get(r);
        v.m_vPos = Codec<std::remove_cvref_t<decltype(v.m_vPos)>>::get(r);
        v.m_LastTouchLineIndex = Codec<std::remove_cvref_t<decltype(v.m_LastTouchLineIndex)>>::get(r);
        v.m_iLimitLevel = Codec<std::remove_cvref_t<decltype(v.m_iLimitLevel)>>::get(r);
        v.m_iRequireDungeonID = Codec<std::remove_cvref_t<decltype(v.m_iRequireDungeonID)>>::get(r);
        return v;
    }
};

struct KEGS_CALL_MY_LOVER_VILLAGE_NOT {
    std::int32_t m_iMapID{};  // int
    VECTOR3      m_vPos{};
    std::uint8_t m_LastTouchLineIndex{};  // unsigned char
    std::int64_t m_iCalledUserUID{};  // UidType
    std::int64_t m_iCalledGSUID{};  // UidType
    char         m_cCallType{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iMapID)>>::put(w, m_iMapID);
        Codec<std::remove_cvref_t<decltype(m_vPos)>>::put(w, m_vPos);
        Codec<std::remove_cvref_t<decltype(m_LastTouchLineIndex)>>::put(w, m_LastTouchLineIndex);
        Codec<std::remove_cvref_t<decltype(m_iCalledUserUID)>>::put(w, m_iCalledUserUID);
        Codec<std::remove_cvref_t<decltype(m_iCalledGSUID)>>::put(w, m_iCalledGSUID);
        Codec<std::remove_cvref_t<decltype(m_cCallType)>>::put(w, m_cCallType);
    }

    static KEGS_CALL_MY_LOVER_VILLAGE_NOT read_from(Reader& r) {
        KEGS_CALL_MY_LOVER_VILLAGE_NOT v;
        v.m_iMapID = Codec<std::remove_cvref_t<decltype(v.m_iMapID)>>::get(r);
        v.m_vPos = Codec<std::remove_cvref_t<decltype(v.m_vPos)>>::get(r);
        v.m_LastTouchLineIndex = Codec<std::remove_cvref_t<decltype(v.m_LastTouchLineIndex)>>::get(r);
        v.m_iCalledUserUID = Codec<std::remove_cvref_t<decltype(v.m_iCalledUserUID)>>::get(r);
        v.m_iCalledGSUID = Codec<std::remove_cvref_t<decltype(v.m_iCalledGSUID)>>::get(r);
        v.m_cCallType = Codec<std::remove_cvref_t<decltype(v.m_cCallType)>>::get(r);
        return v;
    }
};

struct KEGS_DECISION_ENTER_DEFENCE_DUNGEON_NOT {
    VECTOR3 m_DieBossPos{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_DieBossPos)>>::put(w, m_DieBossPos);
    }

    static KEGS_DECISION_ENTER_DEFENCE_DUNGEON_NOT read_from(Reader& r) {
        KEGS_DECISION_ENTER_DEFENCE_DUNGEON_NOT v;
        v.m_DieBossPos = Codec<std::remove_cvref_t<decltype(v.m_DieBossPos)>>::get(r);
        return v;
    }
};

struct KEGS_DROP_ITEM_NOT {
    std::vector<KDropItemData> m_DropItemDataList{};  // std::vector< KDropItemData >
    VECTOR3                    m_CreatePos{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_DropItemDataList)>>::put(w, m_DropItemDataList);
        Codec<std::remove_cvref_t<decltype(m_CreatePos)>>::put(w, m_CreatePos);
    }

    static KEGS_DROP_ITEM_NOT read_from(Reader& r) {
        KEGS_DROP_ITEM_NOT v;
        v.m_DropItemDataList = Codec<std::remove_cvref_t<decltype(v.m_DropItemDataList)>>::get(r);
        v.m_CreatePos = Codec<std::remove_cvref_t<decltype(v.m_CreatePos)>>::get(r);
        return v;
    }
};

struct KEGS_FIELD_LOADING_COMPLETE_REQ {
    VECTOR3       m_vPos{};
    std::uint8_t  m_ucLastTouchLineIndex{};  // unsigned char
    std::uint16_t m_usLastPosValue{};  // unsigned short

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vPos)>>::put(w, m_vPos);
        Codec<std::remove_cvref_t<decltype(m_ucLastTouchLineIndex)>>::put(w, m_ucLastTouchLineIndex);
        Codec<std::remove_cvref_t<decltype(m_usLastPosValue)>>::put(w, m_usLastPosValue);
    }

    static KEGS_FIELD_LOADING_COMPLETE_REQ read_from(Reader& r) {
        KEGS_FIELD_LOADING_COMPLETE_REQ v;
        v.m_vPos = Codec<std::remove_cvref_t<decltype(v.m_vPos)>>::get(r);
        v.m_ucLastTouchLineIndex = Codec<std::remove_cvref_t<decltype(v.m_ucLastTouchLineIndex)>>::get(r);
        v.m_usLastPosValue = Codec<std::remove_cvref_t<decltype(v.m_usLastPosValue)>>::get(r);
        return v;
    }
};

struct KEGS_FIELD_UNIT_SYNC_DATA_BROAD_NOT {
    std::int64_t                 m_UnitUID{};  // UidType
    VECTOR3                      m_vPos{};
    std::vector<KFieldFrameInfo> m_vecFieldFrame{};  // std::vector< KFieldFrameInfo >
    bool                         m_bIsRight{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_UnitUID)>>::put(w, m_UnitUID);
        Codec<std::remove_cvref_t<decltype(m_vPos)>>::put(w, m_vPos);
        Codec<std::remove_cvref_t<decltype(m_vecFieldFrame)>>::put(w, m_vecFieldFrame);
        Codec<std::remove_cvref_t<decltype(m_bIsRight)>>::put(w, m_bIsRight);
    }

    static KEGS_FIELD_UNIT_SYNC_DATA_BROAD_NOT read_from(Reader& r) {
        KEGS_FIELD_UNIT_SYNC_DATA_BROAD_NOT v;
        v.m_UnitUID = Codec<std::remove_cvref_t<decltype(v.m_UnitUID)>>::get(r);
        v.m_vPos = Codec<std::remove_cvref_t<decltype(v.m_vPos)>>::get(r);
        v.m_vecFieldFrame = Codec<std::remove_cvref_t<decltype(v.m_vecFieldFrame)>>::get(r);
        v.m_bIsRight = Codec<std::remove_cvref_t<decltype(v.m_bIsRight)>>::get(r);
        return v;
    }
};

struct KEGS_FIELD_UNIT_SYNC_DATA_NOT {
    VECTOR3                      m_vPos{};
    std::vector<KFieldFrameInfo> m_vecFieldFrame{};  // std::vector< KFieldFrameInfo >
    bool                         m_bIsRight{};
    std::uint8_t                 m_ucLastTouchLineIndex{};  // unsigned char
    std::uint16_t                m_usLastPosValue{};  // unsigned short

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vPos)>>::put(w, m_vPos);
        Codec<std::remove_cvref_t<decltype(m_vecFieldFrame)>>::put(w, m_vecFieldFrame);
        Codec<std::remove_cvref_t<decltype(m_bIsRight)>>::put(w, m_bIsRight);
        Codec<std::remove_cvref_t<decltype(m_ucLastTouchLineIndex)>>::put(w, m_ucLastTouchLineIndex);
        Codec<std::remove_cvref_t<decltype(m_usLastPosValue)>>::put(w, m_usLastPosValue);
    }

    static KEGS_FIELD_UNIT_SYNC_DATA_NOT read_from(Reader& r) {
        KEGS_FIELD_UNIT_SYNC_DATA_NOT v;
        v.m_vPos = Codec<std::remove_cvref_t<decltype(v.m_vPos)>>::get(r);
        v.m_vecFieldFrame = Codec<std::remove_cvref_t<decltype(v.m_vecFieldFrame)>>::get(r);
        v.m_bIsRight = Codec<std::remove_cvref_t<decltype(v.m_bIsRight)>>::get(r);
        v.m_ucLastTouchLineIndex = Codec<std::remove_cvref_t<decltype(v.m_ucLastTouchLineIndex)>>::get(r);
        v.m_usLastPosValue = Codec<std::remove_cvref_t<decltype(v.m_usLastPosValue)>>::get(r);
        return v;
    }
};

struct KEGS_JOIN_FIELD_CHANNEL_CHANGE_REQ {
    std::int32_t m_iMapID{};  // int
    VECTOR3      m_vPos{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iMapID)>>::put(w, m_iMapID);
        Codec<std::remove_cvref_t<decltype(m_vPos)>>::put(w, m_vPos);
    }

    static KEGS_JOIN_FIELD_CHANNEL_CHANGE_REQ read_from(Reader& r) {
        KEGS_JOIN_FIELD_CHANNEL_CHANGE_REQ v;
        v.m_iMapID = Codec<std::remove_cvref_t<decltype(v.m_iMapID)>>::get(r);
        v.m_vPos = Codec<std::remove_cvref_t<decltype(v.m_vPos)>>::get(r);
        return v;
    }
};

struct KEGS_NPC_UNIT_DIE_REQ {
    std::int32_t                  m_nDieNPCUID{};  // int
    char                          m_cAttUnitType{};
    std::int64_t                  m_uiAttUnit{};  // UidType
    std::int64_t                  m_uiMDAttUnit{};  // UidType
    VECTOR3                       m_DiePos{};
    char                          m_cNpcDieState{};
    std::int32_t                  m_iNpcMaxHP{};  // int
    std::map<std::int64_t, float> m_mapDamageByUser{};  // std::map< UidType, float >
    std::int64_t                  m_iFinisherUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_nDieNPCUID)>>::put(w, m_nDieNPCUID);
        Codec<std::remove_cvref_t<decltype(m_cAttUnitType)>>::put(w, m_cAttUnitType);
        Codec<std::remove_cvref_t<decltype(m_uiAttUnit)>>::put(w, m_uiAttUnit);
        Codec<std::remove_cvref_t<decltype(m_uiMDAttUnit)>>::put(w, m_uiMDAttUnit);
        Codec<std::remove_cvref_t<decltype(m_DiePos)>>::put(w, m_DiePos);
        Codec<std::remove_cvref_t<decltype(m_cNpcDieState)>>::put(w, m_cNpcDieState);
        Codec<std::remove_cvref_t<decltype(m_iNpcMaxHP)>>::put(w, m_iNpcMaxHP);
        Codec<std::remove_cvref_t<decltype(m_mapDamageByUser)>>::put(w, m_mapDamageByUser);
        Codec<std::remove_cvref_t<decltype(m_iFinisherUID)>>::put(w, m_iFinisherUID);
    }

    static KEGS_NPC_UNIT_DIE_REQ read_from(Reader& r) {
        KEGS_NPC_UNIT_DIE_REQ v;
        v.m_nDieNPCUID = Codec<std::remove_cvref_t<decltype(v.m_nDieNPCUID)>>::get(r);
        v.m_cAttUnitType = Codec<std::remove_cvref_t<decltype(v.m_cAttUnitType)>>::get(r);
        v.m_uiAttUnit = Codec<std::remove_cvref_t<decltype(v.m_uiAttUnit)>>::get(r);
        v.m_uiMDAttUnit = Codec<std::remove_cvref_t<decltype(v.m_uiMDAttUnit)>>::get(r);
        v.m_DiePos = Codec<std::remove_cvref_t<decltype(v.m_DiePos)>>::get(r);
        v.m_cNpcDieState = Codec<std::remove_cvref_t<decltype(v.m_cNpcDieState)>>::get(r);
        v.m_iNpcMaxHP = Codec<std::remove_cvref_t<decltype(v.m_iNpcMaxHP)>>::get(r);
        v.m_mapDamageByUser = Codec<std::remove_cvref_t<decltype(v.m_mapDamageByUser)>>::get(r);
        v.m_iFinisherUID = Codec<std::remove_cvref_t<decltype(v.m_iFinisherUID)>>::get(r);
        return v;
    }
};

struct KEGS_SQUARE_UNIT_SYNC_DATA_NOT {
    std::int64_t m_UnitUID{};  // UidType
    char         m_StateID{};
    char         m_StateChangeNum{};
    VECTOR3      m_vPos{};
    std::uint8_t m_LastTouchLineIndex{};  // unsigned char
    bool         m_bIsRight{};
    std::int32_t m_iED{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_UnitUID)>>::put(w, m_UnitUID);
        Codec<std::remove_cvref_t<decltype(m_StateID)>>::put(w, m_StateID);
        Codec<std::remove_cvref_t<decltype(m_StateChangeNum)>>::put(w, m_StateChangeNum);
        Codec<std::remove_cvref_t<decltype(m_vPos)>>::put(w, m_vPos);
        Codec<std::remove_cvref_t<decltype(m_LastTouchLineIndex)>>::put(w, m_LastTouchLineIndex);
        Codec<std::remove_cvref_t<decltype(m_bIsRight)>>::put(w, m_bIsRight);
        Codec<std::remove_cvref_t<decltype(m_iED)>>::put(w, m_iED);
    }

    static KEGS_SQUARE_UNIT_SYNC_DATA_NOT read_from(Reader& r) {
        KEGS_SQUARE_UNIT_SYNC_DATA_NOT v;
        v.m_UnitUID = Codec<std::remove_cvref_t<decltype(v.m_UnitUID)>>::get(r);
        v.m_StateID = Codec<std::remove_cvref_t<decltype(v.m_StateID)>>::get(r);
        v.m_StateChangeNum = Codec<std::remove_cvref_t<decltype(v.m_StateChangeNum)>>::get(r);
        v.m_vPos = Codec<std::remove_cvref_t<decltype(v.m_vPos)>>::get(r);
        v.m_LastTouchLineIndex = Codec<std::remove_cvref_t<decltype(v.m_LastTouchLineIndex)>>::get(r);
        v.m_bIsRight = Codec<std::remove_cvref_t<decltype(v.m_bIsRight)>>::get(r);
        v.m_iED = Codec<std::remove_cvref_t<decltype(v.m_iED)>>::get(r);
        return v;
    }
};

struct KEGS_SQUARE_UNIT_SYNC_DATA_REQ {
    char         m_StateID{};
    char         m_StateChangeNum{};
    VECTOR3      m_vPos{};
    std::uint8_t m_LastTouchLineIndex{};  // unsigned char
    bool         m_bIsRight{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_StateID)>>::put(w, m_StateID);
        Codec<std::remove_cvref_t<decltype(m_StateChangeNum)>>::put(w, m_StateChangeNum);
        Codec<std::remove_cvref_t<decltype(m_vPos)>>::put(w, m_vPos);
        Codec<std::remove_cvref_t<decltype(m_LastTouchLineIndex)>>::put(w, m_LastTouchLineIndex);
        Codec<std::remove_cvref_t<decltype(m_bIsRight)>>::put(w, m_bIsRight);
    }

    static KEGS_SQUARE_UNIT_SYNC_DATA_REQ read_from(Reader& r) {
        KEGS_SQUARE_UNIT_SYNC_DATA_REQ v;
        v.m_StateID = Codec<std::remove_cvref_t<decltype(v.m_StateID)>>::get(r);
        v.m_StateChangeNum = Codec<std::remove_cvref_t<decltype(v.m_StateChangeNum)>>::get(r);
        v.m_vPos = Codec<std::remove_cvref_t<decltype(v.m_vPos)>>::get(r);
        v.m_LastTouchLineIndex = Codec<std::remove_cvref_t<decltype(v.m_LastTouchLineIndex)>>::get(r);
        v.m_bIsRight = Codec<std::remove_cvref_t<decltype(v.m_bIsRight)>>::get(r);
        return v;
    }
};

struct KELG_CALL_MY_LOVER_AGREE_RESULT_ROOM_NOT {
    bool         m_bAgree{};
    std::int64_t m_iCalledUserUID{};  // UidType
    std::int64_t m_iCalledGSUID{};  // UidType
    std::int64_t m_iMyUserUID{};  // UidType
    std::int64_t m_iMyGSUID{};  // UidType
    std::int32_t m_iBattleFieldID{};  // int
    std::int64_t m_RoomUID{};  // UidType
    VECTOR3      m_vPos{};
    std::uint8_t m_LastTouchLineIndex{};  // unsigned char

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bAgree)>>::put(w, m_bAgree);
        Codec<std::remove_cvref_t<decltype(m_iCalledUserUID)>>::put(w, m_iCalledUserUID);
        Codec<std::remove_cvref_t<decltype(m_iCalledGSUID)>>::put(w, m_iCalledGSUID);
        Codec<std::remove_cvref_t<decltype(m_iMyUserUID)>>::put(w, m_iMyUserUID);
        Codec<std::remove_cvref_t<decltype(m_iMyGSUID)>>::put(w, m_iMyGSUID);
        Codec<std::remove_cvref_t<decltype(m_iBattleFieldID)>>::put(w, m_iBattleFieldID);
        Codec<std::remove_cvref_t<decltype(m_RoomUID)>>::put(w, m_RoomUID);
        Codec<std::remove_cvref_t<decltype(m_vPos)>>::put(w, m_vPos);
        Codec<std::remove_cvref_t<decltype(m_LastTouchLineIndex)>>::put(w, m_LastTouchLineIndex);
    }

    static KELG_CALL_MY_LOVER_AGREE_RESULT_ROOM_NOT read_from(Reader& r) {
        KELG_CALL_MY_LOVER_AGREE_RESULT_ROOM_NOT v;
        v.m_bAgree = Codec<std::remove_cvref_t<decltype(v.m_bAgree)>>::get(r);
        v.m_iCalledUserUID = Codec<std::remove_cvref_t<decltype(v.m_iCalledUserUID)>>::get(r);
        v.m_iCalledGSUID = Codec<std::remove_cvref_t<decltype(v.m_iCalledGSUID)>>::get(r);
        v.m_iMyUserUID = Codec<std::remove_cvref_t<decltype(v.m_iMyUserUID)>>::get(r);
        v.m_iMyGSUID = Codec<std::remove_cvref_t<decltype(v.m_iMyGSUID)>>::get(r);
        v.m_iBattleFieldID = Codec<std::remove_cvref_t<decltype(v.m_iBattleFieldID)>>::get(r);
        v.m_RoomUID = Codec<std::remove_cvref_t<decltype(v.m_RoomUID)>>::get(r);
        v.m_vPos = Codec<std::remove_cvref_t<decltype(v.m_vPos)>>::get(r);
        v.m_LastTouchLineIndex = Codec<std::remove_cvref_t<decltype(v.m_LastTouchLineIndex)>>::get(r);
        return v;
    }
};

struct KELG_CALL_MY_LOVER_AGREE_RESULT_VILLAGE_NOT {
    bool         m_bAgree{};
    std::int32_t m_iMapID{};  // int
    VECTOR3      m_vPos{};
    std::uint8_t m_LastTouchLineIndex{};  // unsigned char
    std::int64_t m_iCalledUserUID{};  // UidType
    std::int64_t m_iCalledGSUID{};  // UidType
    std::int64_t m_iMyUserUID{};  // UidType
    std::int64_t m_iMyGSUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bAgree)>>::put(w, m_bAgree);
        Codec<std::remove_cvref_t<decltype(m_iMapID)>>::put(w, m_iMapID);
        Codec<std::remove_cvref_t<decltype(m_vPos)>>::put(w, m_vPos);
        Codec<std::remove_cvref_t<decltype(m_LastTouchLineIndex)>>::put(w, m_LastTouchLineIndex);
        Codec<std::remove_cvref_t<decltype(m_iCalledUserUID)>>::put(w, m_iCalledUserUID);
        Codec<std::remove_cvref_t<decltype(m_iCalledGSUID)>>::put(w, m_iCalledGSUID);
        Codec<std::remove_cvref_t<decltype(m_iMyUserUID)>>::put(w, m_iMyUserUID);
        Codec<std::remove_cvref_t<decltype(m_iMyGSUID)>>::put(w, m_iMyGSUID);
    }

    static KELG_CALL_MY_LOVER_AGREE_RESULT_VILLAGE_NOT read_from(Reader& r) {
        KELG_CALL_MY_LOVER_AGREE_RESULT_VILLAGE_NOT v;
        v.m_bAgree = Codec<std::remove_cvref_t<decltype(v.m_bAgree)>>::get(r);
        v.m_iMapID = Codec<std::remove_cvref_t<decltype(v.m_iMapID)>>::get(r);
        v.m_vPos = Codec<std::remove_cvref_t<decltype(v.m_vPos)>>::get(r);
        v.m_LastTouchLineIndex = Codec<std::remove_cvref_t<decltype(v.m_LastTouchLineIndex)>>::get(r);
        v.m_iCalledUserUID = Codec<std::remove_cvref_t<decltype(v.m_iCalledUserUID)>>::get(r);
        v.m_iCalledGSUID = Codec<std::remove_cvref_t<decltype(v.m_iCalledGSUID)>>::get(r);
        v.m_iMyUserUID = Codec<std::remove_cvref_t<decltype(v.m_iMyUserUID)>>::get(r);
        v.m_iMyGSUID = Codec<std::remove_cvref_t<decltype(v.m_iMyGSUID)>>::get(r);
        return v;
    }
};

struct KELG_CALL_MY_LOVER_FIND_USER_ROOM_REQ {
    std::int32_t         m_iOK{};  // int
    KBattleFieldRoomInfo m_kBTInfo{};
    VECTOR3              m_vPos{};
    std::uint8_t         m_LastTouchLineIndex{};  // unsigned char
    std::int64_t         m_iLoverUnitUID{};  // UidType
    char                 m_cCallType{};
    std::int32_t         m_iChannelID{};  // int
    std::int64_t         m_iCalledUserUID{};  // UidType
    std::int64_t         m_iCalledGSUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_kBTInfo)>>::put(w, m_kBTInfo);
        Codec<std::remove_cvref_t<decltype(m_vPos)>>::put(w, m_vPos);
        Codec<std::remove_cvref_t<decltype(m_LastTouchLineIndex)>>::put(w, m_LastTouchLineIndex);
        Codec<std::remove_cvref_t<decltype(m_iLoverUnitUID)>>::put(w, m_iLoverUnitUID);
        Codec<std::remove_cvref_t<decltype(m_cCallType)>>::put(w, m_cCallType);
        Codec<std::remove_cvref_t<decltype(m_iChannelID)>>::put(w, m_iChannelID);
        Codec<std::remove_cvref_t<decltype(m_iCalledUserUID)>>::put(w, m_iCalledUserUID);
        Codec<std::remove_cvref_t<decltype(m_iCalledGSUID)>>::put(w, m_iCalledGSUID);
    }

    static KELG_CALL_MY_LOVER_FIND_USER_ROOM_REQ read_from(Reader& r) {
        KELG_CALL_MY_LOVER_FIND_USER_ROOM_REQ v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_kBTInfo = Codec<std::remove_cvref_t<decltype(v.m_kBTInfo)>>::get(r);
        v.m_vPos = Codec<std::remove_cvref_t<decltype(v.m_vPos)>>::get(r);
        v.m_LastTouchLineIndex = Codec<std::remove_cvref_t<decltype(v.m_LastTouchLineIndex)>>::get(r);
        v.m_iLoverUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iLoverUnitUID)>>::get(r);
        v.m_cCallType = Codec<std::remove_cvref_t<decltype(v.m_cCallType)>>::get(r);
        v.m_iChannelID = Codec<std::remove_cvref_t<decltype(v.m_iChannelID)>>::get(r);
        v.m_iCalledUserUID = Codec<std::remove_cvref_t<decltype(v.m_iCalledUserUID)>>::get(r);
        v.m_iCalledGSUID = Codec<std::remove_cvref_t<decltype(v.m_iCalledGSUID)>>::get(r);
        return v;
    }
};

struct KELG_CALL_MY_LOVER_FIND_USER_VILLAGE_REQ {
    std::int32_t m_iMapID{};  // int
    VECTOR3      m_vPos{};
    std::uint8_t m_LastTouchLineIndex{};  // unsigned char
    std::int64_t m_iCalledUserUID{};  // UidType
    std::int64_t m_iCalledGSUID{};  // UidType
    std::int32_t m_iChannelID{};  // int
    std::int64_t m_iLoverUnitUID{};  // UidType
    char         m_cCallType{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iMapID)>>::put(w, m_iMapID);
        Codec<std::remove_cvref_t<decltype(m_vPos)>>::put(w, m_vPos);
        Codec<std::remove_cvref_t<decltype(m_LastTouchLineIndex)>>::put(w, m_LastTouchLineIndex);
        Codec<std::remove_cvref_t<decltype(m_iCalledUserUID)>>::put(w, m_iCalledUserUID);
        Codec<std::remove_cvref_t<decltype(m_iCalledGSUID)>>::put(w, m_iCalledGSUID);
        Codec<std::remove_cvref_t<decltype(m_iChannelID)>>::put(w, m_iChannelID);
        Codec<std::remove_cvref_t<decltype(m_iLoverUnitUID)>>::put(w, m_iLoverUnitUID);
        Codec<std::remove_cvref_t<decltype(m_cCallType)>>::put(w, m_cCallType);
    }

    static KELG_CALL_MY_LOVER_FIND_USER_VILLAGE_REQ read_from(Reader& r) {
        KELG_CALL_MY_LOVER_FIND_USER_VILLAGE_REQ v;
        v.m_iMapID = Codec<std::remove_cvref_t<decltype(v.m_iMapID)>>::get(r);
        v.m_vPos = Codec<std::remove_cvref_t<decltype(v.m_vPos)>>::get(r);
        v.m_LastTouchLineIndex = Codec<std::remove_cvref_t<decltype(v.m_LastTouchLineIndex)>>::get(r);
        v.m_iCalledUserUID = Codec<std::remove_cvref_t<decltype(v.m_iCalledUserUID)>>::get(r);
        v.m_iCalledGSUID = Codec<std::remove_cvref_t<decltype(v.m_iCalledGSUID)>>::get(r);
        v.m_iChannelID = Codec<std::remove_cvref_t<decltype(v.m_iChannelID)>>::get(r);
        v.m_iLoverUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iLoverUnitUID)>>::get(r);
        v.m_cCallType = Codec<std::remove_cvref_t<decltype(v.m_cCallType)>>::get(r);
        return v;
    }
};

struct KELG_CALL_MY_LOVER_JOIN_BATTLE_FIELD_NOT {
    std::int64_t m_iCalledUserUID{};  // UidType
    std::int64_t m_iCalledUnitUID{};  // UidType
    std::int64_t m_iCalledGSUID{};  // UidType
    std::int32_t m_iBattleFieldID{};  // int
    std::int64_t m_RoomUID{};  // UidType
    VECTOR3      m_vPos{};
    std::uint8_t m_LastTouchLineIndex{};  // unsigned char

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iCalledUserUID)>>::put(w, m_iCalledUserUID);
        Codec<std::remove_cvref_t<decltype(m_iCalledUnitUID)>>::put(w, m_iCalledUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iCalledGSUID)>>::put(w, m_iCalledGSUID);
        Codec<std::remove_cvref_t<decltype(m_iBattleFieldID)>>::put(w, m_iBattleFieldID);
        Codec<std::remove_cvref_t<decltype(m_RoomUID)>>::put(w, m_RoomUID);
        Codec<std::remove_cvref_t<decltype(m_vPos)>>::put(w, m_vPos);
        Codec<std::remove_cvref_t<decltype(m_LastTouchLineIndex)>>::put(w, m_LastTouchLineIndex);
    }

    static KELG_CALL_MY_LOVER_JOIN_BATTLE_FIELD_NOT read_from(Reader& r) {
        KELG_CALL_MY_LOVER_JOIN_BATTLE_FIELD_NOT v;
        v.m_iCalledUserUID = Codec<std::remove_cvref_t<decltype(v.m_iCalledUserUID)>>::get(r);
        v.m_iCalledUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iCalledUnitUID)>>::get(r);
        v.m_iCalledGSUID = Codec<std::remove_cvref_t<decltype(v.m_iCalledGSUID)>>::get(r);
        v.m_iBattleFieldID = Codec<std::remove_cvref_t<decltype(v.m_iBattleFieldID)>>::get(r);
        v.m_RoomUID = Codec<std::remove_cvref_t<decltype(v.m_RoomUID)>>::get(r);
        v.m_vPos = Codec<std::remove_cvref_t<decltype(v.m_vPos)>>::get(r);
        v.m_LastTouchLineIndex = Codec<std::remove_cvref_t<decltype(v.m_LastTouchLineIndex)>>::get(r);
        return v;
    }
};

struct KELG_CALL_MY_LOVER_STATE_CHANGE_FIELD_NOT {
    bool         m_bUseWarpItem{};
    std::int32_t m_iMapID{};  // int
    VECTOR3      m_vPos{};
    std::uint8_t m_LastTouchLineIndex{};  // unsigned char

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bUseWarpItem)>>::put(w, m_bUseWarpItem);
        Codec<std::remove_cvref_t<decltype(m_iMapID)>>::put(w, m_iMapID);
        Codec<std::remove_cvref_t<decltype(m_vPos)>>::put(w, m_vPos);
        Codec<std::remove_cvref_t<decltype(m_LastTouchLineIndex)>>::put(w, m_LastTouchLineIndex);
    }

    static KELG_CALL_MY_LOVER_STATE_CHANGE_FIELD_NOT read_from(Reader& r) {
        KELG_CALL_MY_LOVER_STATE_CHANGE_FIELD_NOT v;
        v.m_bUseWarpItem = Codec<std::remove_cvref_t<decltype(v.m_bUseWarpItem)>>::get(r);
        v.m_iMapID = Codec<std::remove_cvref_t<decltype(v.m_iMapID)>>::get(r);
        v.m_vPos = Codec<std::remove_cvref_t<decltype(v.m_vPos)>>::get(r);
        v.m_LastTouchLineIndex = Codec<std::remove_cvref_t<decltype(v.m_LastTouchLineIndex)>>::get(r);
        return v;
    }
};

struct KERM_CALL_MY_LOVER_CHECK_ROOM_ACK {
    std::int32_t         m_iOK{};  // int
    KBattleFieldRoomInfo m_kBTInfo{};
    VECTOR3              m_vPos{};
    std::uint8_t         m_LastTouchLineIndex{};  // unsigned char
    std::int64_t         m_iLoverUnitUID{};  // UidType
    char                 m_cCallType{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_kBTInfo)>>::put(w, m_kBTInfo);
        Codec<std::remove_cvref_t<decltype(m_vPos)>>::put(w, m_vPos);
        Codec<std::remove_cvref_t<decltype(m_LastTouchLineIndex)>>::put(w, m_LastTouchLineIndex);
        Codec<std::remove_cvref_t<decltype(m_iLoverUnitUID)>>::put(w, m_iLoverUnitUID);
        Codec<std::remove_cvref_t<decltype(m_cCallType)>>::put(w, m_cCallType);
    }

    static KERM_CALL_MY_LOVER_CHECK_ROOM_ACK read_from(Reader& r) {
        KERM_CALL_MY_LOVER_CHECK_ROOM_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_kBTInfo = Codec<std::remove_cvref_t<decltype(v.m_kBTInfo)>>::get(r);
        v.m_vPos = Codec<std::remove_cvref_t<decltype(v.m_vPos)>>::get(r);
        v.m_LastTouchLineIndex = Codec<std::remove_cvref_t<decltype(v.m_LastTouchLineIndex)>>::get(r);
        v.m_iLoverUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iLoverUnitUID)>>::get(r);
        v.m_cCallType = Codec<std::remove_cvref_t<decltype(v.m_cCallType)>>::get(r);
        return v;
    }
};

struct KERM_CALL_MY_LOVER_CHECK_ROOM_REQ {
    VECTOR3      m_vPos{};
    std::uint8_t m_LastTouchLineIndex{};  // unsigned char
    std::int64_t m_iLoverUnitUID{};  // UidType
    char         m_cCallType{};
    std::int32_t m_iChannelID{};  // int
    std::int64_t m_iCalledUserUID{};  // UidType
    std::int64_t m_iCalledGSUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vPos)>>::put(w, m_vPos);
        Codec<std::remove_cvref_t<decltype(m_LastTouchLineIndex)>>::put(w, m_LastTouchLineIndex);
        Codec<std::remove_cvref_t<decltype(m_iLoverUnitUID)>>::put(w, m_iLoverUnitUID);
        Codec<std::remove_cvref_t<decltype(m_cCallType)>>::put(w, m_cCallType);
        Codec<std::remove_cvref_t<decltype(m_iChannelID)>>::put(w, m_iChannelID);
        Codec<std::remove_cvref_t<decltype(m_iCalledUserUID)>>::put(w, m_iCalledUserUID);
        Codec<std::remove_cvref_t<decltype(m_iCalledGSUID)>>::put(w, m_iCalledGSUID);
    }

    static KERM_CALL_MY_LOVER_CHECK_ROOM_REQ read_from(Reader& r) {
        KERM_CALL_MY_LOVER_CHECK_ROOM_REQ v;
        v.m_vPos = Codec<std::remove_cvref_t<decltype(v.m_vPos)>>::get(r);
        v.m_LastTouchLineIndex = Codec<std::remove_cvref_t<decltype(v.m_LastTouchLineIndex)>>::get(r);
        v.m_iLoverUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iLoverUnitUID)>>::get(r);
        v.m_cCallType = Codec<std::remove_cvref_t<decltype(v.m_cCallType)>>::get(r);
        v.m_iChannelID = Codec<std::remove_cvref_t<decltype(v.m_iChannelID)>>::get(r);
        v.m_iCalledUserUID = Codec<std::remove_cvref_t<decltype(v.m_iCalledUserUID)>>::get(r);
        v.m_iCalledGSUID = Codec<std::remove_cvref_t<decltype(v.m_iCalledGSUID)>>::get(r);
        return v;
    }
};

struct KERM_CALL_MY_LOVER_JOIN_BATTLE_FIELD_REQ {
    std::int64_t              m_iCalledUserUID{};  // UidType
    std::int64_t              m_iCalledUnitUID{};  // UidType
    std::int64_t              m_iCalledGSUID{};  // UidType
    std::int64_t              m_RoomUID{};  // UidType
    VECTOR3                   m_vPos{};
    KRoomUserInfo             m_kRoomUserInfo{};
    std::vector<std::int64_t> m_vecStudentUnitUID{};  // std::vector< UidType >
    KBattleFieldJoinInfo      m_kBattleFieldJoinInfo{};
    KReturnToFieldInfo        m_kReturnToFieldInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iCalledUserUID)>>::put(w, m_iCalledUserUID);
        Codec<std::remove_cvref_t<decltype(m_iCalledUnitUID)>>::put(w, m_iCalledUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iCalledGSUID)>>::put(w, m_iCalledGSUID);
        Codec<std::remove_cvref_t<decltype(m_RoomUID)>>::put(w, m_RoomUID);
        Codec<std::remove_cvref_t<decltype(m_vPos)>>::put(w, m_vPos);
        Codec<std::remove_cvref_t<decltype(m_kRoomUserInfo)>>::put(w, m_kRoomUserInfo);
        Codec<std::remove_cvref_t<decltype(m_vecStudentUnitUID)>>::put(w, m_vecStudentUnitUID);
        Codec<std::remove_cvref_t<decltype(m_kBattleFieldJoinInfo)>>::put(w, m_kBattleFieldJoinInfo);
        Codec<std::remove_cvref_t<decltype(m_kReturnToFieldInfo)>>::put(w, m_kReturnToFieldInfo);
    }

    static KERM_CALL_MY_LOVER_JOIN_BATTLE_FIELD_REQ read_from(Reader& r) {
        KERM_CALL_MY_LOVER_JOIN_BATTLE_FIELD_REQ v;
        v.m_iCalledUserUID = Codec<std::remove_cvref_t<decltype(v.m_iCalledUserUID)>>::get(r);
        v.m_iCalledUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iCalledUnitUID)>>::get(r);
        v.m_iCalledGSUID = Codec<std::remove_cvref_t<decltype(v.m_iCalledGSUID)>>::get(r);
        v.m_RoomUID = Codec<std::remove_cvref_t<decltype(v.m_RoomUID)>>::get(r);
        v.m_vPos = Codec<std::remove_cvref_t<decltype(v.m_vPos)>>::get(r);
        v.m_kRoomUserInfo = Codec<std::remove_cvref_t<decltype(v.m_kRoomUserInfo)>>::get(r);
        v.m_vecStudentUnitUID = Codec<std::remove_cvref_t<decltype(v.m_vecStudentUnitUID)>>::get(r);
        v.m_kBattleFieldJoinInfo = Codec<std::remove_cvref_t<decltype(v.m_kBattleFieldJoinInfo)>>::get(r);
        v.m_kReturnToFieldInfo = Codec<std::remove_cvref_t<decltype(v.m_kReturnToFieldInfo)>>::get(r);
        return v;
    }
};

struct KERM_CALL_MY_LOVER_LEAVE_FIELD_ACK {
    std::int32_t m_iOK{};  // int
    std::int32_t m_iReason{};  // int
    std::int32_t m_iMapID{};  // int
    VECTOR3      m_vPos{};
    std::uint8_t m_LastTouchLineIndex{};  // unsigned char
    std::int32_t m_iLimitLevel{};  // int
    std::int32_t m_iRequireDungeonID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iReason)>>::put(w, m_iReason);
        Codec<std::remove_cvref_t<decltype(m_iMapID)>>::put(w, m_iMapID);
        Codec<std::remove_cvref_t<decltype(m_vPos)>>::put(w, m_vPos);
        Codec<std::remove_cvref_t<decltype(m_LastTouchLineIndex)>>::put(w, m_LastTouchLineIndex);
        Codec<std::remove_cvref_t<decltype(m_iLimitLevel)>>::put(w, m_iLimitLevel);
        Codec<std::remove_cvref_t<decltype(m_iRequireDungeonID)>>::put(w, m_iRequireDungeonID);
    }

    static KERM_CALL_MY_LOVER_LEAVE_FIELD_ACK read_from(Reader& r) {
        KERM_CALL_MY_LOVER_LEAVE_FIELD_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iReason = Codec<std::remove_cvref_t<decltype(v.m_iReason)>>::get(r);
        v.m_iMapID = Codec<std::remove_cvref_t<decltype(v.m_iMapID)>>::get(r);
        v.m_vPos = Codec<std::remove_cvref_t<decltype(v.m_vPos)>>::get(r);
        v.m_LastTouchLineIndex = Codec<std::remove_cvref_t<decltype(v.m_LastTouchLineIndex)>>::get(r);
        v.m_iLimitLevel = Codec<std::remove_cvref_t<decltype(v.m_iLimitLevel)>>::get(r);
        v.m_iRequireDungeonID = Codec<std::remove_cvref_t<decltype(v.m_iRequireDungeonID)>>::get(r);
        return v;
    }
};

struct KERM_CALL_MY_LOVER_LEAVE_FIELD_REQ {
    std::int32_t m_iReason{};  // int
    std::int32_t m_iMapID{};  // int
    VECTOR3      m_vPos{};
    std::uint8_t m_LastTouchLineIndex{};  // unsigned char
    std::int32_t m_iLimitLevel{};  // int
    std::int32_t m_iRequireDungeonID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iReason)>>::put(w, m_iReason);
        Codec<std::remove_cvref_t<decltype(m_iMapID)>>::put(w, m_iMapID);
        Codec<std::remove_cvref_t<decltype(m_vPos)>>::put(w, m_vPos);
        Codec<std::remove_cvref_t<decltype(m_LastTouchLineIndex)>>::put(w, m_LastTouchLineIndex);
        Codec<std::remove_cvref_t<decltype(m_iLimitLevel)>>::put(w, m_iLimitLevel);
        Codec<std::remove_cvref_t<decltype(m_iRequireDungeonID)>>::put(w, m_iRequireDungeonID);
    }

    static KERM_CALL_MY_LOVER_LEAVE_FIELD_REQ read_from(Reader& r) {
        KERM_CALL_MY_LOVER_LEAVE_FIELD_REQ v;
        v.m_iReason = Codec<std::remove_cvref_t<decltype(v.m_iReason)>>::get(r);
        v.m_iMapID = Codec<std::remove_cvref_t<decltype(v.m_iMapID)>>::get(r);
        v.m_vPos = Codec<std::remove_cvref_t<decltype(v.m_vPos)>>::get(r);
        v.m_LastTouchLineIndex = Codec<std::remove_cvref_t<decltype(v.m_LastTouchLineIndex)>>::get(r);
        v.m_iLimitLevel = Codec<std::remove_cvref_t<decltype(v.m_iLimitLevel)>>::get(r);
        v.m_iRequireDungeonID = Codec<std::remove_cvref_t<decltype(v.m_iRequireDungeonID)>>::get(r);
        return v;
    }
};

struct KETR_FIELD_UNIT_SYNC_DATA_BROAD_NOT {
    std::vector<std::int64_t>    m_vecUID{};  // std::vector< UidType >
    std::int64_t                 m_UnitUID{};  // UidType
    VECTOR3                      m_vPos{};
    std::vector<KFieldFrameInfo> m_vecFieldFrame{};  // std::vector< KFieldFrameInfo >
    bool                         m_bIsRight{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecUID)>>::put(w, m_vecUID);
        Codec<std::remove_cvref_t<decltype(m_UnitUID)>>::put(w, m_UnitUID);
        Codec<std::remove_cvref_t<decltype(m_vPos)>>::put(w, m_vPos);
        Codec<std::remove_cvref_t<decltype(m_vecFieldFrame)>>::put(w, m_vecFieldFrame);
        Codec<std::remove_cvref_t<decltype(m_bIsRight)>>::put(w, m_bIsRight);
    }

    static KETR_FIELD_UNIT_SYNC_DATA_BROAD_NOT read_from(Reader& r) {
        KETR_FIELD_UNIT_SYNC_DATA_BROAD_NOT v;
        v.m_vecUID = Codec<std::remove_cvref_t<decltype(v.m_vecUID)>>::get(r);
        v.m_UnitUID = Codec<std::remove_cvref_t<decltype(v.m_UnitUID)>>::get(r);
        v.m_vPos = Codec<std::remove_cvref_t<decltype(v.m_vPos)>>::get(r);
        v.m_vecFieldFrame = Codec<std::remove_cvref_t<decltype(v.m_vecFieldFrame)>>::get(r);
        v.m_bIsRight = Codec<std::remove_cvref_t<decltype(v.m_bIsRight)>>::get(r);
        return v;
    }
};

struct KFieldUserInfo {
    std::int64_t                          m_iUnitUID{};  // UidType
    std::u16string                        m_wstrNickName{};  // std::wstring
    char                                  m_cUnitClass{};
    std::uint8_t                          m_ucLevel{};  // UCHAR
    char                                  m_cRank{};
    std::vector<KInventoryItemSimpleInfo> m_vecEquippedItem{};  // std::vector< KInventoryItemSimpleInfo >
    std::int32_t                          m_iTitleID{};  // int
    VECTOR3                               m_vPos{};
    std::int64_t                          m_iPartyUID{};  // UidType
    std::u16string                        m_wstrGuildName{};  // std::wstring
    std::vector<KFieldPetInfo>            m_vecPet{};  // std::vector< KFieldPetInfo >
    std::int32_t                          m_iED{};  // int
    bool                                  m_bInvisible{};
    std::int64_t                          m_iRidingPetUID{};  // UidType
    std::uint16_t                         m_usRidingPetID{};  // USHORT
    std::int64_t                          m_iLoverUnitUID{};  // UidType
    std::int32_t                          m_iEventQuestClearCount{};  // int
    std::int32_t                          m_iExchangeCount{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_cUnitClass)>>::put(w, m_cUnitClass);
        Codec<std::remove_cvref_t<decltype(m_ucLevel)>>::put(w, m_ucLevel);
        Codec<std::remove_cvref_t<decltype(m_cRank)>>::put(w, m_cRank);
        Codec<std::remove_cvref_t<decltype(m_vecEquippedItem)>>::put(w, m_vecEquippedItem);
        Codec<std::remove_cvref_t<decltype(m_iTitleID)>>::put(w, m_iTitleID);
        Codec<std::remove_cvref_t<decltype(m_vPos)>>::put(w, m_vPos);
        Codec<std::remove_cvref_t<decltype(m_iPartyUID)>>::put(w, m_iPartyUID);
        Codec<std::remove_cvref_t<decltype(m_wstrGuildName)>>::put(w, m_wstrGuildName);
        Codec<std::remove_cvref_t<decltype(m_vecPet)>>::put(w, m_vecPet);
        Codec<std::remove_cvref_t<decltype(m_iED)>>::put(w, m_iED);
        Codec<std::remove_cvref_t<decltype(m_bInvisible)>>::put(w, m_bInvisible);
        Codec<std::remove_cvref_t<decltype(m_iRidingPetUID)>>::put(w, m_iRidingPetUID);
        Codec<std::remove_cvref_t<decltype(m_usRidingPetID)>>::put(w, m_usRidingPetID);
        Codec<std::remove_cvref_t<decltype(m_iLoverUnitUID)>>::put(w, m_iLoverUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iEventQuestClearCount)>>::put(w, m_iEventQuestClearCount);
        Codec<std::remove_cvref_t<decltype(m_iExchangeCount)>>::put(w, m_iExchangeCount);
    }

    static KFieldUserInfo read_from(Reader& r) {
        KFieldUserInfo v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_cUnitClass = Codec<std::remove_cvref_t<decltype(v.m_cUnitClass)>>::get(r);
        v.m_ucLevel = Codec<std::remove_cvref_t<decltype(v.m_ucLevel)>>::get(r);
        v.m_cRank = Codec<std::remove_cvref_t<decltype(v.m_cRank)>>::get(r);
        v.m_vecEquippedItem = Codec<std::remove_cvref_t<decltype(v.m_vecEquippedItem)>>::get(r);
        v.m_iTitleID = Codec<std::remove_cvref_t<decltype(v.m_iTitleID)>>::get(r);
        v.m_vPos = Codec<std::remove_cvref_t<decltype(v.m_vPos)>>::get(r);
        v.m_iPartyUID = Codec<std::remove_cvref_t<decltype(v.m_iPartyUID)>>::get(r);
        v.m_wstrGuildName = Codec<std::remove_cvref_t<decltype(v.m_wstrGuildName)>>::get(r);
        v.m_vecPet = Codec<std::remove_cvref_t<decltype(v.m_vecPet)>>::get(r);
        v.m_iED = Codec<std::remove_cvref_t<decltype(v.m_iED)>>::get(r);
        v.m_bInvisible = Codec<std::remove_cvref_t<decltype(v.m_bInvisible)>>::get(r);
        v.m_iRidingPetUID = Codec<std::remove_cvref_t<decltype(v.m_iRidingPetUID)>>::get(r);
        v.m_usRidingPetID = Codec<std::remove_cvref_t<decltype(v.m_usRidingPetID)>>::get(r);
        v.m_iLoverUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iLoverUnitUID)>>::get(r);
        v.m_iEventQuestClearCount = Codec<std::remove_cvref_t<decltype(v.m_iEventQuestClearCount)>>::get(r);
        v.m_iExchangeCount = Codec<std::remove_cvref_t<decltype(v.m_iExchangeCount)>>::get(r);
        return v;
    }
};

struct KEGS_CHANGE_USER_LIST_IN_SECTOR_NOT {
    std::vector<KFieldUserInfo> m_vecNewUserList{};  // std::vector< KFieldUserInfo >
    std::vector<std::int64_t>   m_vecDelUserList{};  // std::vector< UidType >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecNewUserList)>>::put(w, m_vecNewUserList);
        Codec<std::remove_cvref_t<decltype(m_vecDelUserList)>>::put(w, m_vecDelUserList);
    }

    static KEGS_CHANGE_USER_LIST_IN_SECTOR_NOT read_from(Reader& r) {
        KEGS_CHANGE_USER_LIST_IN_SECTOR_NOT v;
        v.m_vecNewUserList = Codec<std::remove_cvref_t<decltype(v.m_vecNewUserList)>>::get(r);
        v.m_vecDelUserList = Codec<std::remove_cvref_t<decltype(v.m_vecDelUserList)>>::get(r);
        return v;
    }
};

struct KEGS_TOGGLE_INVISIBLE_NOT {
    std::int64_t   m_iUnitUID{};  // UidType
    KFieldUserInfo m_kFieldUserInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_kFieldUserInfo)>>::put(w, m_kFieldUserInfo);
    }

    static KEGS_TOGGLE_INVISIBLE_NOT read_from(Reader& r) {
        KEGS_TOGGLE_INVISIBLE_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_kFieldUserInfo = Codec<std::remove_cvref_t<decltype(v.m_kFieldUserInfo)>>::get(r);
        return v;
    }
};

struct KERM_CHANGE_FIELD_USER_INFO_NOT {
    KFieldUserInfo m_kInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kInfo)>>::put(w, m_kInfo);
    }

    static KERM_CHANGE_FIELD_USER_INFO_NOT read_from(Reader& r) {
        KERM_CHANGE_FIELD_USER_INFO_NOT v;
        v.m_kInfo = Codec<std::remove_cvref_t<decltype(v.m_kInfo)>>::get(r);
        return v;
    }
};

struct KERM_JOIN_FIELD_REQ {
    std::int64_t           m_iFieldUID{};  // UidType
    KFieldUserInfo         m_kFieldUserInfo{};
    std::set<std::int64_t> m_setPartyUIDList{};  // std::set< UidType >
    std::set<std::int64_t> m_setFriendUIDList{};  // std::set< UidType >
    KFieldOption           m_kFieldOption{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iFieldUID)>>::put(w, m_iFieldUID);
        Codec<std::remove_cvref_t<decltype(m_kFieldUserInfo)>>::put(w, m_kFieldUserInfo);
        Codec<std::remove_cvref_t<decltype(m_setPartyUIDList)>>::put(w, m_setPartyUIDList);
        Codec<std::remove_cvref_t<decltype(m_setFriendUIDList)>>::put(w, m_setFriendUIDList);
        Codec<std::remove_cvref_t<decltype(m_kFieldOption)>>::put(w, m_kFieldOption);
    }

    static KERM_JOIN_FIELD_REQ read_from(Reader& r) {
        KERM_JOIN_FIELD_REQ v;
        v.m_iFieldUID = Codec<std::remove_cvref_t<decltype(v.m_iFieldUID)>>::get(r);
        v.m_kFieldUserInfo = Codec<std::remove_cvref_t<decltype(v.m_kFieldUserInfo)>>::get(r);
        v.m_setPartyUIDList = Codec<std::remove_cvref_t<decltype(v.m_setPartyUIDList)>>::get(r);
        v.m_setFriendUIDList = Codec<std::remove_cvref_t<decltype(v.m_setFriendUIDList)>>::get(r);
        v.m_kFieldOption = Codec<std::remove_cvref_t<decltype(v.m_kFieldOption)>>::get(r);
        return v;
    }
};

struct KERM_TOGGLE_INVISIBLE_NOT {
    std::int64_t   m_iUnitUID{};  // UidType
    KFieldUserInfo m_kFieldUserInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_kFieldUserInfo)>>::put(w, m_kFieldUserInfo);
    }

    static KERM_TOGGLE_INVISIBLE_NOT read_from(Reader& r) {
        KERM_TOGGLE_INVISIBLE_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_kFieldUserInfo = Codec<std::remove_cvref_t<decltype(v.m_kFieldUserInfo)>>::get(r);
        return v;
    }
};

struct KETR_CHANGE_USER_LIST_IN_SECTOR_NOT {
    std::vector<std::int64_t>   m_vecUID{};  // std::vector< UidType >
    std::vector<KFieldUserInfo> m_vecNewUserList{};  // std::vector< KFieldUserInfo >
    std::vector<std::int64_t>   m_vecDelUserList{};  // std::vector< UidType >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecUID)>>::put(w, m_vecUID);
        Codec<std::remove_cvref_t<decltype(m_vecNewUserList)>>::put(w, m_vecNewUserList);
        Codec<std::remove_cvref_t<decltype(m_vecDelUserList)>>::put(w, m_vecDelUserList);
    }

    static KETR_CHANGE_USER_LIST_IN_SECTOR_NOT read_from(Reader& r) {
        KETR_CHANGE_USER_LIST_IN_SECTOR_NOT v;
        v.m_vecUID = Codec<std::remove_cvref_t<decltype(v.m_vecUID)>>::get(r);
        v.m_vecNewUserList = Codec<std::remove_cvref_t<decltype(v.m_vecNewUserList)>>::get(r);
        v.m_vecDelUserList = Codec<std::remove_cvref_t<decltype(v.m_vecDelUserList)>>::get(r);
        return v;
    }
};

struct KETR_TOGGLE_INVISIBLE_NOT {
    std::vector<std::int64_t> m_vecUID{};  // std::vector< UidType >
    std::int64_t              m_iUnitUID{};  // UidType
    KFieldUserInfo            m_kFieldUserInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecUID)>>::put(w, m_vecUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_kFieldUserInfo)>>::put(w, m_kFieldUserInfo);
    }

    static KETR_TOGGLE_INVISIBLE_NOT read_from(Reader& r) {
        KETR_TOGGLE_INVISIBLE_NOT v;
        v.m_vecUID = Codec<std::remove_cvref_t<decltype(v.m_vecUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_kFieldUserInfo = Codec<std::remove_cvref_t<decltype(v.m_kFieldUserInfo)>>::get(r);
        return v;
    }
};

struct KNPCUnitReq {
    std::int32_t              m_UID{};  // int
    std::int32_t              m_NPCID{};  // int
    char                      m_Level{};
    VECTOR3                   m_vPos{};
    std::int32_t              m_nStartPos{};  // int
    std::vector<std::int32_t> m_vecPetrolLineIndex{};  // std::vector< int >
    std::vector<std::int32_t> m_vecPlayLineIndex{};  // std::vector< int >
    std::int32_t              m_iGroupID{};  // int
    bool                      m_bAggressive{};
    bool                      m_bActive{};
    float                     m_fDelayTime{};
    char                      m_KeyCode{};
    bool                      m_bFocusCamera{};
    bool                      m_bShowGage{};
    VECTOR3                   m_AddPos{};
    bool                      m_bHasBossGage{};
    bool                      m_bShowBossName{};
    bool                      m_bShowSubBossName{};
    bool                      m_bSiegeMode{};
    bool                      m_bIsRight{};
    bool                      m_bNoDrop{};
    float                     m_fUnitScale{};
    char                      m_cTeamNum{};
    char                      m_cAIType{};
    std::int64_t              m_iAllyUID{};  // UidType
    char                      m_cAllyTeam{};
    char                      m_cMonsterGrade{};
    std::uint16_t             m_usBossGaugeHPLines{};  // u_short
    bool                      m_bAllyNpc{};
    bool                      m_bAttributeNpc{};
    char                      m_cCreateMonsterType{};
    float                     m_fShowBossNameDelayTime{};
    bool                      m_bStopAtStartState{};
    std::int32_t              m_iBossGroupID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_UID)>>::put(w, m_UID);
        Codec<std::remove_cvref_t<decltype(m_NPCID)>>::put(w, m_NPCID);
        Codec<std::remove_cvref_t<decltype(m_Level)>>::put(w, m_Level);
        Codec<std::remove_cvref_t<decltype(m_vPos)>>::put(w, m_vPos);
        Codec<std::remove_cvref_t<decltype(m_nStartPos)>>::put(w, m_nStartPos);
        Codec<std::remove_cvref_t<decltype(m_vecPetrolLineIndex)>>::put(w, m_vecPetrolLineIndex);
        Codec<std::remove_cvref_t<decltype(m_vecPlayLineIndex)>>::put(w, m_vecPlayLineIndex);
        Codec<std::remove_cvref_t<decltype(m_iGroupID)>>::put(w, m_iGroupID);
        Codec<std::remove_cvref_t<decltype(m_bAggressive)>>::put(w, m_bAggressive);
        Codec<std::remove_cvref_t<decltype(m_bActive)>>::put(w, m_bActive);
        Codec<std::remove_cvref_t<decltype(m_fDelayTime)>>::put(w, m_fDelayTime);
        Codec<std::remove_cvref_t<decltype(m_KeyCode)>>::put(w, m_KeyCode);
        Codec<std::remove_cvref_t<decltype(m_bFocusCamera)>>::put(w, m_bFocusCamera);
        Codec<std::remove_cvref_t<decltype(m_bShowGage)>>::put(w, m_bShowGage);
        Codec<std::remove_cvref_t<decltype(m_AddPos)>>::put(w, m_AddPos);
        Codec<std::remove_cvref_t<decltype(m_bHasBossGage)>>::put(w, m_bHasBossGage);
        Codec<std::remove_cvref_t<decltype(m_bShowBossName)>>::put(w, m_bShowBossName);
        Codec<std::remove_cvref_t<decltype(m_bShowSubBossName)>>::put(w, m_bShowSubBossName);
        Codec<std::remove_cvref_t<decltype(m_bSiegeMode)>>::put(w, m_bSiegeMode);
        Codec<std::remove_cvref_t<decltype(m_bIsRight)>>::put(w, m_bIsRight);
        Codec<std::remove_cvref_t<decltype(m_bNoDrop)>>::put(w, m_bNoDrop);
        Codec<std::remove_cvref_t<decltype(m_fUnitScale)>>::put(w, m_fUnitScale);
        Codec<std::remove_cvref_t<decltype(m_cTeamNum)>>::put(w, m_cTeamNum);
        Codec<std::remove_cvref_t<decltype(m_cAIType)>>::put(w, m_cAIType);
        Codec<std::remove_cvref_t<decltype(m_iAllyUID)>>::put(w, m_iAllyUID);
        Codec<std::remove_cvref_t<decltype(m_cAllyTeam)>>::put(w, m_cAllyTeam);
        Codec<std::remove_cvref_t<decltype(m_cMonsterGrade)>>::put(w, m_cMonsterGrade);
        Codec<std::remove_cvref_t<decltype(m_usBossGaugeHPLines)>>::put(w, m_usBossGaugeHPLines);
        Codec<std::remove_cvref_t<decltype(m_bAllyNpc)>>::put(w, m_bAllyNpc);
        Codec<std::remove_cvref_t<decltype(m_bAttributeNpc)>>::put(w, m_bAttributeNpc);
        Codec<std::remove_cvref_t<decltype(m_cCreateMonsterType)>>::put(w, m_cCreateMonsterType);
        Codec<std::remove_cvref_t<decltype(m_fShowBossNameDelayTime)>>::put(w, m_fShowBossNameDelayTime);
        Codec<std::remove_cvref_t<decltype(m_bStopAtStartState)>>::put(w, m_bStopAtStartState);
        Codec<std::remove_cvref_t<decltype(m_iBossGroupID)>>::put(w, m_iBossGroupID);
    }

    static KNPCUnitReq read_from(Reader& r) {
        KNPCUnitReq v;
        v.m_UID = Codec<std::remove_cvref_t<decltype(v.m_UID)>>::get(r);
        v.m_NPCID = Codec<std::remove_cvref_t<decltype(v.m_NPCID)>>::get(r);
        v.m_Level = Codec<std::remove_cvref_t<decltype(v.m_Level)>>::get(r);
        v.m_vPos = Codec<std::remove_cvref_t<decltype(v.m_vPos)>>::get(r);
        v.m_nStartPos = Codec<std::remove_cvref_t<decltype(v.m_nStartPos)>>::get(r);
        v.m_vecPetrolLineIndex = Codec<std::remove_cvref_t<decltype(v.m_vecPetrolLineIndex)>>::get(r);
        v.m_vecPlayLineIndex = Codec<std::remove_cvref_t<decltype(v.m_vecPlayLineIndex)>>::get(r);
        v.m_iGroupID = Codec<std::remove_cvref_t<decltype(v.m_iGroupID)>>::get(r);
        v.m_bAggressive = Codec<std::remove_cvref_t<decltype(v.m_bAggressive)>>::get(r);
        v.m_bActive = Codec<std::remove_cvref_t<decltype(v.m_bActive)>>::get(r);
        v.m_fDelayTime = Codec<std::remove_cvref_t<decltype(v.m_fDelayTime)>>::get(r);
        v.m_KeyCode = Codec<std::remove_cvref_t<decltype(v.m_KeyCode)>>::get(r);
        v.m_bFocusCamera = Codec<std::remove_cvref_t<decltype(v.m_bFocusCamera)>>::get(r);
        v.m_bShowGage = Codec<std::remove_cvref_t<decltype(v.m_bShowGage)>>::get(r);
        v.m_AddPos = Codec<std::remove_cvref_t<decltype(v.m_AddPos)>>::get(r);
        v.m_bHasBossGage = Codec<std::remove_cvref_t<decltype(v.m_bHasBossGage)>>::get(r);
        v.m_bShowBossName = Codec<std::remove_cvref_t<decltype(v.m_bShowBossName)>>::get(r);
        v.m_bShowSubBossName = Codec<std::remove_cvref_t<decltype(v.m_bShowSubBossName)>>::get(r);
        v.m_bSiegeMode = Codec<std::remove_cvref_t<decltype(v.m_bSiegeMode)>>::get(r);
        v.m_bIsRight = Codec<std::remove_cvref_t<decltype(v.m_bIsRight)>>::get(r);
        v.m_bNoDrop = Codec<std::remove_cvref_t<decltype(v.m_bNoDrop)>>::get(r);
        v.m_fUnitScale = Codec<std::remove_cvref_t<decltype(v.m_fUnitScale)>>::get(r);
        v.m_cTeamNum = Codec<std::remove_cvref_t<decltype(v.m_cTeamNum)>>::get(r);
        v.m_cAIType = Codec<std::remove_cvref_t<decltype(v.m_cAIType)>>::get(r);
        v.m_iAllyUID = Codec<std::remove_cvref_t<decltype(v.m_iAllyUID)>>::get(r);
        v.m_cAllyTeam = Codec<std::remove_cvref_t<decltype(v.m_cAllyTeam)>>::get(r);
        v.m_cMonsterGrade = Codec<std::remove_cvref_t<decltype(v.m_cMonsterGrade)>>::get(r);
        v.m_usBossGaugeHPLines = Codec<std::remove_cvref_t<decltype(v.m_usBossGaugeHPLines)>>::get(r);
        v.m_bAllyNpc = Codec<std::remove_cvref_t<decltype(v.m_bAllyNpc)>>::get(r);
        v.m_bAttributeNpc = Codec<std::remove_cvref_t<decltype(v.m_bAttributeNpc)>>::get(r);
        v.m_cCreateMonsterType = Codec<std::remove_cvref_t<decltype(v.m_cCreateMonsterType)>>::get(r);
        v.m_fShowBossNameDelayTime = Codec<std::remove_cvref_t<decltype(v.m_fShowBossNameDelayTime)>>::get(r);
        v.m_bStopAtStartState = Codec<std::remove_cvref_t<decltype(v.m_bStopAtStartState)>>::get(r);
        v.m_iBossGroupID = Codec<std::remove_cvref_t<decltype(v.m_iBossGroupID)>>::get(r);
        return v;
    }
};

struct KAttribNpcUnitInfo {
    KNPCUnitReq           m_sNPCUnitReq{};
    KAttribEnchantNpcInfo m_sAttribEnchantNpcInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_sNPCUnitReq)>>::put(w, m_sNPCUnitReq);
        Codec<std::remove_cvref_t<decltype(m_sAttribEnchantNpcInfo)>>::put(w, m_sAttribEnchantNpcInfo);
    }

    static KAttribNpcUnitInfo read_from(Reader& r) {
        KAttribNpcUnitInfo v;
        v.m_sNPCUnitReq = Codec<std::remove_cvref_t<decltype(v.m_sNPCUnitReq)>>::get(r);
        v.m_sAttribEnchantNpcInfo = Codec<std::remove_cvref_t<decltype(v.m_sAttribEnchantNpcInfo)>>::get(r);
        return v;
    }
};

struct KEGS_ADMIN_NPC_UNIT_CREATE_NOT {
    KNPCUnitReq m_kNPCUnitNot{};
    char        m_cAttrib1{};
    char        m_cAttrib2{};
    char        m_cSA1{};
    char        m_cSA2{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kNPCUnitNot)>>::put(w, m_kNPCUnitNot);
        Codec<std::remove_cvref_t<decltype(m_cAttrib1)>>::put(w, m_cAttrib1);
        Codec<std::remove_cvref_t<decltype(m_cAttrib2)>>::put(w, m_cAttrib2);
        Codec<std::remove_cvref_t<decltype(m_cSA1)>>::put(w, m_cSA1);
        Codec<std::remove_cvref_t<decltype(m_cSA2)>>::put(w, m_cSA2);
    }

    static KEGS_ADMIN_NPC_UNIT_CREATE_NOT read_from(Reader& r) {
        KEGS_ADMIN_NPC_UNIT_CREATE_NOT v;
        v.m_kNPCUnitNot = Codec<std::remove_cvref_t<decltype(v.m_kNPCUnitNot)>>::get(r);
        v.m_cAttrib1 = Codec<std::remove_cvref_t<decltype(v.m_cAttrib1)>>::get(r);
        v.m_cAttrib2 = Codec<std::remove_cvref_t<decltype(v.m_cAttrib2)>>::get(r);
        v.m_cSA1 = Codec<std::remove_cvref_t<decltype(v.m_cSA1)>>::get(r);
        v.m_cSA2 = Codec<std::remove_cvref_t<decltype(v.m_cSA2)>>::get(r);
        return v;
    }
};

struct KEGS_ADMIN_NPC_UNIT_CREATE_REQ {
    KNPCUnitReq m_kNPCUnitReq{};
    char        m_cAttrib1{};
    char        m_cAttrib2{};
    char        m_cSA1{};
    char        m_cSA2{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kNPCUnitReq)>>::put(w, m_kNPCUnitReq);
        Codec<std::remove_cvref_t<decltype(m_cAttrib1)>>::put(w, m_cAttrib1);
        Codec<std::remove_cvref_t<decltype(m_cAttrib2)>>::put(w, m_cAttrib2);
        Codec<std::remove_cvref_t<decltype(m_cSA1)>>::put(w, m_cSA1);
        Codec<std::remove_cvref_t<decltype(m_cSA2)>>::put(w, m_cSA2);
    }

    static KEGS_ADMIN_NPC_UNIT_CREATE_REQ read_from(Reader& r) {
        KEGS_ADMIN_NPC_UNIT_CREATE_REQ v;
        v.m_kNPCUnitReq = Codec<std::remove_cvref_t<decltype(v.m_kNPCUnitReq)>>::get(r);
        v.m_cAttrib1 = Codec<std::remove_cvref_t<decltype(v.m_cAttrib1)>>::get(r);
        v.m_cAttrib2 = Codec<std::remove_cvref_t<decltype(v.m_cAttrib2)>>::get(r);
        v.m_cSA1 = Codec<std::remove_cvref_t<decltype(v.m_cSA1)>>::get(r);
        v.m_cSA2 = Codec<std::remove_cvref_t<decltype(v.m_cSA2)>>::get(r);
        return v;
    }
};

struct KEGS_BATTLE_FIELD_NPC_LOAD_NOT {
    std::vector<KNPCUnitReq>                      m_vecNPCData{};  // std::vector< KNPCUnitReq >
    std::map<std::int32_t, KAttribEnchantNpcInfo> m_mapAttribNpcInfo{};  // std::map< int, KAttribEnchantNpcInfo >
    std::int32_t                                  m_iBattleFieldID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecNPCData)>>::put(w, m_vecNPCData);
        Codec<std::remove_cvref_t<decltype(m_mapAttribNpcInfo)>>::put(w, m_mapAttribNpcInfo);
        Codec<std::remove_cvref_t<decltype(m_iBattleFieldID)>>::put(w, m_iBattleFieldID);
    }

    static KEGS_BATTLE_FIELD_NPC_LOAD_NOT read_from(Reader& r) {
        KEGS_BATTLE_FIELD_NPC_LOAD_NOT v;
        v.m_vecNPCData = Codec<std::remove_cvref_t<decltype(v.m_vecNPCData)>>::get(r);
        v.m_mapAttribNpcInfo = Codec<std::remove_cvref_t<decltype(v.m_mapAttribNpcInfo)>>::get(r);
        v.m_iBattleFieldID = Codec<std::remove_cvref_t<decltype(v.m_iBattleFieldID)>>::get(r);
        return v;
    }
};

struct KEGS_CREATE_ATTRIB_NPC_NOT {
    std::vector<KAttribNpcUnitInfo> m_vecAttribNPCUnit{};  // std::vector< KAttribNpcUnitInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecAttribNPCUnit)>>::put(w, m_vecAttribNPCUnit);
    }

    static KEGS_CREATE_ATTRIB_NPC_NOT read_from(Reader& r) {
        KEGS_CREATE_ATTRIB_NPC_NOT v;
        v.m_vecAttribNPCUnit = Codec<std::remove_cvref_t<decltype(v.m_vecAttribNPCUnit)>>::get(r);
        return v;
    }
};

struct KEGS_NPC_UNIT_CREATE_REQ {
    std::vector<KNPCUnitReq> m_vecNPCUnitReq{};  // std::vector< KNPCUnitReq >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecNPCUnitReq)>>::put(w, m_vecNPCUnitReq);
    }

    static KEGS_NPC_UNIT_CREATE_REQ read_from(Reader& r) {
        KEGS_NPC_UNIT_CREATE_REQ v;
        v.m_vecNPCUnitReq = Codec<std::remove_cvref_t<decltype(v.m_vecNPCUnitReq)>>::get(r);
        return v;
    }
};

struct KNPCList {
    std::vector<KNPCUnitReq> m_NPCList{};  // std::vector< KNPCUnitReq >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_NPCList)>>::put(w, m_NPCList);
    }

    static KNPCList read_from(Reader& r) {
        KNPCList v;
        v.m_NPCList = Codec<std::remove_cvref_t<decltype(v.m_NPCList)>>::get(r);
        return v;
    }
};

struct KEGS_DUNGEON_STAGE_LOAD_NOT {
    std::int32_t                                  m_iStageID{};  // int
    std::int32_t                                  m_iNumMember{};  // int
    std::map<std::int32_t, KNPCList>              m_mapNPCData{};  // std::map< int, KNPCList >
    std::map<std::int32_t, KAttribEnchantNpcInfo> m_mapAttribNpcInfo{};  // std::map< int, KAttribEnchantNpcInfo >
    float                                         m_fItemDropEventProbRate{};
    bool                                          m_bWithPlayPcBangEvent{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iStageID)>>::put(w, m_iStageID);
        Codec<std::remove_cvref_t<decltype(m_iNumMember)>>::put(w, m_iNumMember);
        Codec<std::remove_cvref_t<decltype(m_mapNPCData)>>::put(w, m_mapNPCData);
        Codec<std::remove_cvref_t<decltype(m_mapAttribNpcInfo)>>::put(w, m_mapAttribNpcInfo);
        Codec<std::remove_cvref_t<decltype(m_fItemDropEventProbRate)>>::put(w, m_fItemDropEventProbRate);
        Codec<std::remove_cvref_t<decltype(m_bWithPlayPcBangEvent)>>::put(w, m_bWithPlayPcBangEvent);
    }

    static KEGS_DUNGEON_STAGE_LOAD_NOT read_from(Reader& r) {
        KEGS_DUNGEON_STAGE_LOAD_NOT v;
        v.m_iStageID = Codec<std::remove_cvref_t<decltype(v.m_iStageID)>>::get(r);
        v.m_iNumMember = Codec<std::remove_cvref_t<decltype(v.m_iNumMember)>>::get(r);
        v.m_mapNPCData = Codec<std::remove_cvref_t<decltype(v.m_mapNPCData)>>::get(r);
        v.m_mapAttribNpcInfo = Codec<std::remove_cvref_t<decltype(v.m_mapAttribNpcInfo)>>::get(r);
        v.m_fItemDropEventProbRate = Codec<std::remove_cvref_t<decltype(v.m_fItemDropEventProbRate)>>::get(r);
        v.m_bWithPlayPcBangEvent = Codec<std::remove_cvref_t<decltype(v.m_bWithPlayPcBangEvent)>>::get(r);
        return v;
    }
};

struct KNPCUnitNot {
    KNPCUnitReq m_kNPCUnitReq{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kNPCUnitReq)>>::put(w, m_kNPCUnitReq);
    }

    static KNPCUnitNot read_from(Reader& r) {
        KNPCUnitNot v;
        v.m_kNPCUnitReq = Codec<std::remove_cvref_t<decltype(v.m_kNPCUnitReq)>>::get(r);
        return v;
    }
};

struct KEGS_NPC_UNIT_CREATE_NOT {
    std::vector<KNPCUnitNot>                      m_vecNPCUnitAck{};  // std::vector< KNPCUnitNot >
    std::map<std::int32_t, KAttribEnchantNpcInfo> m_mapAttribNpcInfo{};  // std::map< int, KAttribEnchantNpcInfo >
    std::int32_t                                  m_iBattleFieldID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecNPCUnitAck)>>::put(w, m_vecNPCUnitAck);
        Codec<std::remove_cvref_t<decltype(m_mapAttribNpcInfo)>>::put(w, m_mapAttribNpcInfo);
        Codec<std::remove_cvref_t<decltype(m_iBattleFieldID)>>::put(w, m_iBattleFieldID);
    }

    static KEGS_NPC_UNIT_CREATE_NOT read_from(Reader& r) {
        KEGS_NPC_UNIT_CREATE_NOT v;
        v.m_vecNPCUnitAck = Codec<std::remove_cvref_t<decltype(v.m_vecNPCUnitAck)>>::get(r);
        v.m_mapAttribNpcInfo = Codec<std::remove_cvref_t<decltype(v.m_mapAttribNpcInfo)>>::get(r);
        v.m_iBattleFieldID = Codec<std::remove_cvref_t<decltype(v.m_iBattleFieldID)>>::get(r);
        return v;
    }
};

struct KEGS_NPC_UNIT_CREATE_MIDDLE_BOSS_NOT {
    KEGS_NPC_UNIT_CREATE_NOT m_kCreatePacket{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kCreatePacket)>>::put(w, m_kCreatePacket);
    }

    static KEGS_NPC_UNIT_CREATE_MIDDLE_BOSS_NOT read_from(Reader& r) {
        KEGS_NPC_UNIT_CREATE_MIDDLE_BOSS_NOT v;
        v.m_kCreatePacket = Codec<std::remove_cvref_t<decltype(v.m_kCreatePacket)>>::get(r);
        return v;
    }
};

struct kDisCountItemInfo {
    std::int32_t              m_DisCountProNo{};  // int
    std::int32_t              m_BasicProNo{};  // int
    std::vector<std::int32_t> m_DisCountKeepItem{};  // std::vector< int >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_DisCountProNo)>>::put(w, m_DisCountProNo);
        Codec<std::remove_cvref_t<decltype(m_BasicProNo)>>::put(w, m_BasicProNo);
        Codec<std::remove_cvref_t<decltype(m_DisCountKeepItem)>>::put(w, m_DisCountKeepItem);
    }

    static kDisCountItemInfo read_from(Reader& r) {
        kDisCountItemInfo v;
        v.m_DisCountProNo = Codec<std::remove_cvref_t<decltype(v.m_DisCountProNo)>>::get(r);
        v.m_BasicProNo = Codec<std::remove_cvref_t<decltype(v.m_BasicProNo)>>::get(r);
        v.m_DisCountKeepItem = Codec<std::remove_cvref_t<decltype(v.m_DisCountKeepItem)>>::get(r);
        return v;
    }
};

struct KEGS_BILL_PRODUCT_INFO_ACK {
    bool                                              m_bFinal{};
    std::vector<KBillProductInfo>                     m_vecProductInfo{};  // std::vector< KBillProductInfo >
    std::map<std::int32_t, std::vector<std::int32_t>> m_mapKeepShowItem{};  // std::map< int,std::vector< int > >
    std::map<std::int32_t, kDisCountItemInfo>         m_DisCountInfoMap{};  // std::map< int, kDisCountItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bFinal)>>::put(w, m_bFinal);
        Codec<std::remove_cvref_t<decltype(m_vecProductInfo)>>::put(w, m_vecProductInfo);
        Codec<std::remove_cvref_t<decltype(m_mapKeepShowItem)>>::put(w, m_mapKeepShowItem);
        Codec<std::remove_cvref_t<decltype(m_DisCountInfoMap)>>::put(w, m_DisCountInfoMap);
    }

    static KEGS_BILL_PRODUCT_INFO_ACK read_from(Reader& r) {
        KEGS_BILL_PRODUCT_INFO_ACK v;
        v.m_bFinal = Codec<std::remove_cvref_t<decltype(v.m_bFinal)>>::get(r);
        v.m_vecProductInfo = Codec<std::remove_cvref_t<decltype(v.m_vecProductInfo)>>::get(r);
        v.m_mapKeepShowItem = Codec<std::remove_cvref_t<decltype(v.m_mapKeepShowItem)>>::get(r);
        v.m_DisCountInfoMap = Codec<std::remove_cvref_t<decltype(v.m_DisCountInfoMap)>>::get(r);
        return v;
    }
};

struct kRandomItemMappingToInfoClient {
    std::int32_t   m_iBeforeGroupID{};  // int
    std::int32_t   m_iAfterGroupID{};  // int
    bool           m_bCheckMapping{};
    std::u16string m_tStartTime{};  // std::wstring
    std::u16string m_tEndTime{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iBeforeGroupID)>>::put(w, m_iBeforeGroupID);
        Codec<std::remove_cvref_t<decltype(m_iAfterGroupID)>>::put(w, m_iAfterGroupID);
        Codec<std::remove_cvref_t<decltype(m_bCheckMapping)>>::put(w, m_bCheckMapping);
        Codec<std::remove_cvref_t<decltype(m_tStartTime)>>::put(w, m_tStartTime);
        Codec<std::remove_cvref_t<decltype(m_tEndTime)>>::put(w, m_tEndTime);
    }

    static kRandomItemMappingToInfoClient read_from(Reader& r) {
        kRandomItemMappingToInfoClient v;
        v.m_iBeforeGroupID = Codec<std::remove_cvref_t<decltype(v.m_iBeforeGroupID)>>::get(r);
        v.m_iAfterGroupID = Codec<std::remove_cvref_t<decltype(v.m_iAfterGroupID)>>::get(r);
        v.m_bCheckMapping = Codec<std::remove_cvref_t<decltype(v.m_bCheckMapping)>>::get(r);
        v.m_tStartTime = Codec<std::remove_cvref_t<decltype(v.m_tStartTime)>>::get(r);
        v.m_tEndTime = Codec<std::remove_cvref_t<decltype(v.m_tEndTime)>>::get(r);
        return v;
    }
};

struct KEGS_CUBE_IN_ITEM_MAPPING_NOT {
    std::map<std::int32_t, std::map<std::int32_t, kRandomItemMappingToInfoClient>> m_mapCubeInItemMapping{};  // std::map< int, std::map<int, kRandomItemMappingToInfoClient> >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_mapCubeInItemMapping)>>::put(w, m_mapCubeInItemMapping);
    }

    static KEGS_CUBE_IN_ITEM_MAPPING_NOT read_from(Reader& r) {
        KEGS_CUBE_IN_ITEM_MAPPING_NOT v;
        v.m_mapCubeInItemMapping = Codec<std::remove_cvref_t<decltype(v.m_mapCubeInItemMapping)>>::get(r);
        return v;
    }
};

