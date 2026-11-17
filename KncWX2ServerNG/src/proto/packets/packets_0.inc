// ---------------------------------------------------------------------------
// packets_0.inc - GENERATED FILE, DO NOT EDIT BY HAND.
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

struct KAbuserEventLog {
    std::int64_t  m_iUserUID{};  // UidType
    std::int64_t  m_iUnitUID{};  // UidType
    std::int32_t  m_iED{};  // int
    std::int32_t  m_iGetItemCount{};  // int
    std::int32_t  m_iSpirit{};  // int
    std::int64_t  m_iRegDate{};  // __int64
    std::uint16_t m_usEventID{};  // unsigned short

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iED)>>::put(w, m_iED);
        Codec<std::remove_cvref_t<decltype(m_iGetItemCount)>>::put(w, m_iGetItemCount);
        Codec<std::remove_cvref_t<decltype(m_iSpirit)>>::put(w, m_iSpirit);
        Codec<std::remove_cvref_t<decltype(m_iRegDate)>>::put(w, m_iRegDate);
        Codec<std::remove_cvref_t<decltype(m_usEventID)>>::put(w, m_usEventID);
    }

    static KAbuserEventLog read_from(Reader& r) {
        KAbuserEventLog v;
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iED = Codec<std::remove_cvref_t<decltype(v.m_iED)>>::get(r);
        v.m_iGetItemCount = Codec<std::remove_cvref_t<decltype(v.m_iGetItemCount)>>::get(r);
        v.m_iSpirit = Codec<std::remove_cvref_t<decltype(v.m_iSpirit)>>::get(r);
        v.m_iRegDate = Codec<std::remove_cvref_t<decltype(v.m_iRegDate)>>::get(r);
        v.m_usEventID = Codec<std::remove_cvref_t<decltype(v.m_usEventID)>>::get(r);
        return v;
    }
};

struct KAbuserList {
    std::int32_t              m_iReleaseTick{};  // int
    std::vector<std::int64_t> m_vecAbuserList{};  // std::vector< UidType >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iReleaseTick)>>::put(w, m_iReleaseTick);
        Codec<std::remove_cvref_t<decltype(m_vecAbuserList)>>::put(w, m_vecAbuserList);
    }

    static KAbuserList read_from(Reader& r) {
        KAbuserList v;
        v.m_iReleaseTick = Codec<std::remove_cvref_t<decltype(v.m_iReleaseTick)>>::get(r);
        v.m_vecAbuserList = Codec<std::remove_cvref_t<decltype(v.m_vecAbuserList)>>::get(r);
        return v;
    }
};

struct KAccountBlockInfo {
    std::u16string m_wstrEndTime{};  // std::wstring
    std::u16string m_wstrBlockReason{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrEndTime)>>::put(w, m_wstrEndTime);
        Codec<std::remove_cvref_t<decltype(m_wstrBlockReason)>>::put(w, m_wstrBlockReason);
    }

    static KAccountBlockInfo read_from(Reader& r) {
        KAccountBlockInfo v;
        v.m_wstrEndTime = Codec<std::remove_cvref_t<decltype(v.m_wstrEndTime)>>::get(r);
        v.m_wstrBlockReason = Codec<std::remove_cvref_t<decltype(v.m_wstrBlockReason)>>::get(r);
        return v;
    }
};

struct KAccountOption {
    bool m_bPlayGuide{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bPlayGuide)>>::put(w, m_bPlayGuide);
    }

    static KAccountOption read_from(Reader& r) {
        KAccountOption v;
        v.m_bPlayGuide = Codec<std::remove_cvref_t<decltype(v.m_bPlayGuide)>>::get(r);
        return v;
    }
};

struct KAccountInfo {
    std::int64_t      m_nUserUID{};  // UidType
    std::u16string    m_wstrID{};  // std::wstring
    std::u16string    m_wstrName{};  // std::wstring
    std::int32_t      m_iAuthLevel{};  // int
    bool              m_bInternalUser{};
    KAccountOption    m_kAccountOption{};
    KAccountBlockInfo m_kAccountBlockInfo{};
    bool              m_bIsRecommend{};
    bool              m_bIsGuestUser{};
    std::u16string    m_wstrOTP{};  // std::wstring
    std::u16string    m_wstrRegDate{};  // std::wstring
    std::u16string    m_wstrLastLogin{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_nUserUID)>>::put(w, m_nUserUID);
        Codec<std::remove_cvref_t<decltype(m_wstrID)>>::put(w, m_wstrID);
        Codec<std::remove_cvref_t<decltype(m_wstrName)>>::put(w, m_wstrName);
        Codec<std::remove_cvref_t<decltype(m_iAuthLevel)>>::put(w, m_iAuthLevel);
        Codec<std::remove_cvref_t<decltype(m_bInternalUser)>>::put(w, m_bInternalUser);
        Codec<std::remove_cvref_t<decltype(m_kAccountOption)>>::put(w, m_kAccountOption);
        Codec<std::remove_cvref_t<decltype(m_kAccountBlockInfo)>>::put(w, m_kAccountBlockInfo);
        Codec<std::remove_cvref_t<decltype(m_bIsRecommend)>>::put(w, m_bIsRecommend);
        Codec<std::remove_cvref_t<decltype(m_bIsGuestUser)>>::put(w, m_bIsGuestUser);
        Codec<std::remove_cvref_t<decltype(m_wstrOTP)>>::put(w, m_wstrOTP);
        Codec<std::remove_cvref_t<decltype(m_wstrRegDate)>>::put(w, m_wstrRegDate);
        Codec<std::remove_cvref_t<decltype(m_wstrLastLogin)>>::put(w, m_wstrLastLogin);
    }

    static KAccountInfo read_from(Reader& r) {
        KAccountInfo v;
        v.m_nUserUID = Codec<std::remove_cvref_t<decltype(v.m_nUserUID)>>::get(r);
        v.m_wstrID = Codec<std::remove_cvref_t<decltype(v.m_wstrID)>>::get(r);
        v.m_wstrName = Codec<std::remove_cvref_t<decltype(v.m_wstrName)>>::get(r);
        v.m_iAuthLevel = Codec<std::remove_cvref_t<decltype(v.m_iAuthLevel)>>::get(r);
        v.m_bInternalUser = Codec<std::remove_cvref_t<decltype(v.m_bInternalUser)>>::get(r);
        v.m_kAccountOption = Codec<std::remove_cvref_t<decltype(v.m_kAccountOption)>>::get(r);
        v.m_kAccountBlockInfo = Codec<std::remove_cvref_t<decltype(v.m_kAccountBlockInfo)>>::get(r);
        v.m_bIsRecommend = Codec<std::remove_cvref_t<decltype(v.m_bIsRecommend)>>::get(r);
        v.m_bIsGuestUser = Codec<std::remove_cvref_t<decltype(v.m_bIsGuestUser)>>::get(r);
        v.m_wstrOTP = Codec<std::remove_cvref_t<decltype(v.m_wstrOTP)>>::get(r);
        v.m_wstrRegDate = Codec<std::remove_cvref_t<decltype(v.m_wstrRegDate)>>::get(r);
        v.m_wstrLastLogin = Codec<std::remove_cvref_t<decltype(v.m_wstrLastLogin)>>::get(r);
        return v;
    }
};

struct KAdminCheatSkill {
    std::int32_t m_iSkillLevel{};  // int
    std::int32_t m_iSkillCSPoint{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iSkillLevel)>>::put(w, m_iSkillLevel);
        Codec<std::remove_cvref_t<decltype(m_iSkillCSPoint)>>::put(w, m_iSkillCSPoint);
    }

    static KAdminCheatSkill read_from(Reader& r) {
        KAdminCheatSkill v;
        v.m_iSkillLevel = Codec<std::remove_cvref_t<decltype(v.m_iSkillLevel)>>::get(r);
        v.m_iSkillCSPoint = Codec<std::remove_cvref_t<decltype(v.m_iSkillCSPoint)>>::get(r);
        return v;
    }
};

struct KApplyDeleteInfo {
    std::int32_t m_iGuildUID{};  // int
    std::int64_t m_iUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iGuildUID)>>::put(w, m_iGuildUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
    }

    static KApplyDeleteInfo read_from(Reader& r) {
        KApplyDeleteInfo v;
        v.m_iGuildUID = Codec<std::remove_cvref_t<decltype(v.m_iGuildUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        return v;
    }
};

struct KApplyJoinGuildInfo {
    std::int32_t   m_iApplyJoinGuildUID{};  // int
    std::int64_t   m_iUnitUID{};  // UidType
    std::u16string m_wstrNickName{};  // std::wstring
    char           m_cUnitClass{};
    std::uint8_t   m_ucLevel{};  // u_char
    std::u16string m_wstrMessage{};  // std::wstring
    std::u16string m_wstrRegDate{};  // std::wstring
    std::int64_t   m_tRegDate{};  // __int64

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iApplyJoinGuildUID)>>::put(w, m_iApplyJoinGuildUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_cUnitClass)>>::put(w, m_cUnitClass);
        Codec<std::remove_cvref_t<decltype(m_ucLevel)>>::put(w, m_ucLevel);
        Codec<std::remove_cvref_t<decltype(m_wstrMessage)>>::put(w, m_wstrMessage);
        Codec<std::remove_cvref_t<decltype(m_wstrRegDate)>>::put(w, m_wstrRegDate);
        Codec<std::remove_cvref_t<decltype(m_tRegDate)>>::put(w, m_tRegDate);
    }

    static KApplyJoinGuildInfo read_from(Reader& r) {
        KApplyJoinGuildInfo v;
        v.m_iApplyJoinGuildUID = Codec<std::remove_cvref_t<decltype(v.m_iApplyJoinGuildUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_cUnitClass = Codec<std::remove_cvref_t<decltype(v.m_cUnitClass)>>::get(r);
        v.m_ucLevel = Codec<std::remove_cvref_t<decltype(v.m_ucLevel)>>::get(r);
        v.m_wstrMessage = Codec<std::remove_cvref_t<decltype(v.m_wstrMessage)>>::get(r);
        v.m_wstrRegDate = Codec<std::remove_cvref_t<decltype(v.m_wstrRegDate)>>::get(r);
        v.m_tRegDate = Codec<std::remove_cvref_t<decltype(v.m_tRegDate)>>::get(r);
        return v;
    }
};

struct KAttribEnchantNpcInfo {
    char m_cAttribEnchant1{};
    char m_cAttribEnchant2{};
    char m_cExtraSpecialAbility1{};
    char m_cExtraSpecialAbility2{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cAttribEnchant1)>>::put(w, m_cAttribEnchant1);
        Codec<std::remove_cvref_t<decltype(m_cAttribEnchant2)>>::put(w, m_cAttribEnchant2);
        Codec<std::remove_cvref_t<decltype(m_cExtraSpecialAbility1)>>::put(w, m_cExtraSpecialAbility1);
        Codec<std::remove_cvref_t<decltype(m_cExtraSpecialAbility2)>>::put(w, m_cExtraSpecialAbility2);
    }

    static KAttribEnchantNpcInfo read_from(Reader& r) {
        KAttribEnchantNpcInfo v;
        v.m_cAttribEnchant1 = Codec<std::remove_cvref_t<decltype(v.m_cAttribEnchant1)>>::get(r);
        v.m_cAttribEnchant2 = Codec<std::remove_cvref_t<decltype(v.m_cAttribEnchant2)>>::get(r);
        v.m_cExtraSpecialAbility1 = Codec<std::remove_cvref_t<decltype(v.m_cExtraSpecialAbility1)>>::get(r);
        v.m_cExtraSpecialAbility2 = Codec<std::remove_cvref_t<decltype(v.m_cExtraSpecialAbility2)>>::get(r);
        return v;
    }
};

struct KAutoPartyBonusInfo {
    char         m_cDungeonMode{};
    std::int32_t m_iDungeonIDWithDif{};  // int
    bool         m_bActivatedBonus{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cDungeonMode)>>::put(w, m_cDungeonMode);
        Codec<std::remove_cvref_t<decltype(m_iDungeonIDWithDif)>>::put(w, m_iDungeonIDWithDif);
        Codec<std::remove_cvref_t<decltype(m_bActivatedBonus)>>::put(w, m_bActivatedBonus);
    }

    static KAutoPartyBonusInfo read_from(Reader& r) {
        KAutoPartyBonusInfo v;
        v.m_cDungeonMode = Codec<std::remove_cvref_t<decltype(v.m_cDungeonMode)>>::get(r);
        v.m_iDungeonIDWithDif = Codec<std::remove_cvref_t<decltype(v.m_iDungeonIDWithDif)>>::get(r);
        v.m_bActivatedBonus = Codec<std::remove_cvref_t<decltype(v.m_bActivatedBonus)>>::get(r);
        return v;
    }
};

struct KAutoPartyConditionInfo {
    std::int32_t m_iDungeonID{};  // int
    char         m_cDifficultyLevel{};
    char         m_cGetItemType{};
    char         m_cDungeonMode{};
    char         m_cLimitPlayerCount{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iDungeonID)>>::put(w, m_iDungeonID);
        Codec<std::remove_cvref_t<decltype(m_cDifficultyLevel)>>::put(w, m_cDifficultyLevel);
        Codec<std::remove_cvref_t<decltype(m_cGetItemType)>>::put(w, m_cGetItemType);
        Codec<std::remove_cvref_t<decltype(m_cDungeonMode)>>::put(w, m_cDungeonMode);
        Codec<std::remove_cvref_t<decltype(m_cLimitPlayerCount)>>::put(w, m_cLimitPlayerCount);
    }

    static KAutoPartyConditionInfo read_from(Reader& r) {
        KAutoPartyConditionInfo v;
        v.m_iDungeonID = Codec<std::remove_cvref_t<decltype(v.m_iDungeonID)>>::get(r);
        v.m_cDifficultyLevel = Codec<std::remove_cvref_t<decltype(v.m_cDifficultyLevel)>>::get(r);
        v.m_cGetItemType = Codec<std::remove_cvref_t<decltype(v.m_cGetItemType)>>::get(r);
        v.m_cDungeonMode = Codec<std::remove_cvref_t<decltype(v.m_cDungeonMode)>>::get(r);
        v.m_cLimitPlayerCount = Codec<std::remove_cvref_t<decltype(v.m_cLimitPlayerCount)>>::get(r);
        return v;
    }
};

struct KAutoPartyDungeonBonusInfo {
    std::int32_t m_iDungeonID{};  // int
    char         m_cDungeonMode{};
    bool         m_bActivatedBonus{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iDungeonID)>>::put(w, m_iDungeonID);
        Codec<std::remove_cvref_t<decltype(m_cDungeonMode)>>::put(w, m_cDungeonMode);
        Codec<std::remove_cvref_t<decltype(m_bActivatedBonus)>>::put(w, m_bActivatedBonus);
    }

    static KAutoPartyDungeonBonusInfo read_from(Reader& r) {
        KAutoPartyDungeonBonusInfo v;
        v.m_iDungeonID = Codec<std::remove_cvref_t<decltype(v.m_iDungeonID)>>::get(r);
        v.m_cDungeonMode = Codec<std::remove_cvref_t<decltype(v.m_cDungeonMode)>>::get(r);
        v.m_bActivatedBonus = Codec<std::remove_cvref_t<decltype(v.m_bActivatedBonus)>>::get(r);
        return v;
    }
};

struct KAutoPartyMakingProcessCondition {
    std::int32_t m_iDungeonMode{};  // int
    std::int32_t m_iDungeonIDWithDif{};  // int
    std::int32_t m_iBeginLevel{};  // int
    std::int32_t m_iEndLevel{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iDungeonMode)>>::put(w, m_iDungeonMode);
        Codec<std::remove_cvref_t<decltype(m_iDungeonIDWithDif)>>::put(w, m_iDungeonIDWithDif);
        Codec<std::remove_cvref_t<decltype(m_iBeginLevel)>>::put(w, m_iBeginLevel);
        Codec<std::remove_cvref_t<decltype(m_iEndLevel)>>::put(w, m_iEndLevel);
    }

    static KAutoPartyMakingProcessCondition read_from(Reader& r) {
        KAutoPartyMakingProcessCondition v;
        v.m_iDungeonMode = Codec<std::remove_cvref_t<decltype(v.m_iDungeonMode)>>::get(r);
        v.m_iDungeonIDWithDif = Codec<std::remove_cvref_t<decltype(v.m_iDungeonIDWithDif)>>::get(r);
        v.m_iBeginLevel = Codec<std::remove_cvref_t<decltype(v.m_iBeginLevel)>>::get(r);
        v.m_iEndLevel = Codec<std::remove_cvref_t<decltype(v.m_iEndLevel)>>::get(r);
        return v;
    }
};

struct KBattleFieldJoinInfo {
    std::int32_t              m_iBattleFieldID{};  // int
    std::int32_t              m_iStartPosIndex{};  // int
    std::vector<std::int64_t> m_vecPartyMemberList{};  // std::vector< UidType >
    bool                      m_bMoveForMyParty{};
    bool                      m_bNowBattleFieldPositionInfoStartPosition{};
    std::uint16_t             m_usBattleFieldPositionValue{};  // unsigned short

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iBattleFieldID)>>::put(w, m_iBattleFieldID);
        Codec<std::remove_cvref_t<decltype(m_iStartPosIndex)>>::put(w, m_iStartPosIndex);
        Codec<std::remove_cvref_t<decltype(m_vecPartyMemberList)>>::put(w, m_vecPartyMemberList);
        Codec<std::remove_cvref_t<decltype(m_bMoveForMyParty)>>::put(w, m_bMoveForMyParty);
        Codec<std::remove_cvref_t<decltype(m_bNowBattleFieldPositionInfoStartPosition)>>::put(w, m_bNowBattleFieldPositionInfoStartPosition);
        Codec<std::remove_cvref_t<decltype(m_usBattleFieldPositionValue)>>::put(w, m_usBattleFieldPositionValue);
    }

    static KBattleFieldJoinInfo read_from(Reader& r) {
        KBattleFieldJoinInfo v;
        v.m_iBattleFieldID = Codec<std::remove_cvref_t<decltype(v.m_iBattleFieldID)>>::get(r);
        v.m_iStartPosIndex = Codec<std::remove_cvref_t<decltype(v.m_iStartPosIndex)>>::get(r);
        v.m_vecPartyMemberList = Codec<std::remove_cvref_t<decltype(v.m_vecPartyMemberList)>>::get(r);
        v.m_bMoveForMyParty = Codec<std::remove_cvref_t<decltype(v.m_bMoveForMyParty)>>::get(r);
        v.m_bNowBattleFieldPositionInfoStartPosition = Codec<std::remove_cvref_t<decltype(v.m_bNowBattleFieldPositionInfoStartPosition)>>::get(r);
        v.m_usBattleFieldPositionValue = Codec<std::remove_cvref_t<decltype(v.m_usBattleFieldPositionValue)>>::get(r);
        return v;
    }
};

struct KBattleFieldRoomInfo {
    std::int32_t                                   m_iBattleFieldID{};  // int
    std::int64_t                                   m_RoomUID{};  // UidType
    char                                           m_MaxSlot{};
    std::map<std::int64_t, std::int64_t>           m_mapUnitUIDPartyUID{};  // std::map< UidType, UidType >
    std::map<std::int64_t, std::set<std::int64_t>> m_mapPartyList{};  // std::map< UidType, std::set< UidType > >
    std::uint8_t                                   m_byteBossFieldState{};  // byte

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iBattleFieldID)>>::put(w, m_iBattleFieldID);
        Codec<std::remove_cvref_t<decltype(m_RoomUID)>>::put(w, m_RoomUID);
        Codec<std::remove_cvref_t<decltype(m_MaxSlot)>>::put(w, m_MaxSlot);
        Codec<std::remove_cvref_t<decltype(m_mapUnitUIDPartyUID)>>::put(w, m_mapUnitUIDPartyUID);
        Codec<std::remove_cvref_t<decltype(m_mapPartyList)>>::put(w, m_mapPartyList);
        Codec<std::remove_cvref_t<decltype(m_byteBossFieldState)>>::put(w, m_byteBossFieldState);
    }

    static KBattleFieldRoomInfo read_from(Reader& r) {
        KBattleFieldRoomInfo v;
        v.m_iBattleFieldID = Codec<std::remove_cvref_t<decltype(v.m_iBattleFieldID)>>::get(r);
        v.m_RoomUID = Codec<std::remove_cvref_t<decltype(v.m_RoomUID)>>::get(r);
        v.m_MaxSlot = Codec<std::remove_cvref_t<decltype(v.m_MaxSlot)>>::get(r);
        v.m_mapUnitUIDPartyUID = Codec<std::remove_cvref_t<decltype(v.m_mapUnitUIDPartyUID)>>::get(r);
        v.m_mapPartyList = Codec<std::remove_cvref_t<decltype(v.m_mapPartyList)>>::get(r);
        v.m_byteBossFieldState = Codec<std::remove_cvref_t<decltype(v.m_byteBossFieldState)>>::get(r);
        return v;
    }
};

struct KBillBuyInfo {
    std::int32_t   m_iProductNo{};  // int
    std::int32_t   m_iPoint{};  // int
    std::int32_t   m_iProductID{};  // int
    std::uint16_t  m_usOrderQuantity{};  // unsigned short
    std::int32_t   m_iCategory{};  // int
    std::u16string m_wstrMemo{};  // std::wstring
    std::u16string m_wstrOrderID{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iProductNo)>>::put(w, m_iProductNo);
        Codec<std::remove_cvref_t<decltype(m_iPoint)>>::put(w, m_iPoint);
        Codec<std::remove_cvref_t<decltype(m_iProductID)>>::put(w, m_iProductID);
        Codec<std::remove_cvref_t<decltype(m_usOrderQuantity)>>::put(w, m_usOrderQuantity);
        Codec<std::remove_cvref_t<decltype(m_iCategory)>>::put(w, m_iCategory);
        Codec<std::remove_cvref_t<decltype(m_wstrMemo)>>::put(w, m_wstrMemo);
        Codec<std::remove_cvref_t<decltype(m_wstrOrderID)>>::put(w, m_wstrOrderID);
    }

    static KBillBuyInfo read_from(Reader& r) {
        KBillBuyInfo v;
        v.m_iProductNo = Codec<std::remove_cvref_t<decltype(v.m_iProductNo)>>::get(r);
        v.m_iPoint = Codec<std::remove_cvref_t<decltype(v.m_iPoint)>>::get(r);
        v.m_iProductID = Codec<std::remove_cvref_t<decltype(v.m_iProductID)>>::get(r);
        v.m_usOrderQuantity = Codec<std::remove_cvref_t<decltype(v.m_usOrderQuantity)>>::get(r);
        v.m_iCategory = Codec<std::remove_cvref_t<decltype(v.m_iCategory)>>::get(r);
        v.m_wstrMemo = Codec<std::remove_cvref_t<decltype(v.m_wstrMemo)>>::get(r);
        v.m_wstrOrderID = Codec<std::remove_cvref_t<decltype(v.m_wstrOrderID)>>::get(r);
        return v;
    }
};

struct KBillCouponItemInfo {
    std::int32_t m_iProductNO{};  // int
    std::int32_t m_iCount{};  // int
    std::int64_t m_iTranNo{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iProductNO)>>::put(w, m_iProductNO);
        Codec<std::remove_cvref_t<decltype(m_iCount)>>::put(w, m_iCount);
        Codec<std::remove_cvref_t<decltype(m_iTranNo)>>::put(w, m_iTranNo);
    }

    static KBillCouponItemInfo read_from(Reader& r) {
        KBillCouponItemInfo v;
        v.m_iProductNO = Codec<std::remove_cvref_t<decltype(v.m_iProductNO)>>::get(r);
        v.m_iCount = Codec<std::remove_cvref_t<decltype(v.m_iCount)>>::get(r);
        v.m_iTranNo = Codec<std::remove_cvref_t<decltype(v.m_iTranNo)>>::get(r);
        return v;
    }
};

struct KBillPackageInfo {
    std::int32_t              m_iPackageNo{};  // int
    std::vector<std::int32_t> m_vecProductNo{};  // std::vector< int >
    std::uint32_t             m_ulProductNo{};  // unsigned long
    std::int32_t              m_iProductID{};  // int
    std::u16string            m_wstrProductID{};  // std::wstring
    std::int16_t              m_sProductExpire{};  // short
    std::uint16_t             m_usProductPieces{};  // unsigned short
    std::vector<std::int32_t> m_vecSocketOption{};  // std::vector< int >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iPackageNo)>>::put(w, m_iPackageNo);
        Codec<std::remove_cvref_t<decltype(m_vecProductNo)>>::put(w, m_vecProductNo);
        Codec<std::remove_cvref_t<decltype(m_ulProductNo)>>::put(w, m_ulProductNo);
        Codec<std::remove_cvref_t<decltype(m_iProductID)>>::put(w, m_iProductID);
        Codec<std::remove_cvref_t<decltype(m_wstrProductID)>>::put(w, m_wstrProductID);
        Codec<std::remove_cvref_t<decltype(m_sProductExpire)>>::put(w, m_sProductExpire);
        Codec<std::remove_cvref_t<decltype(m_usProductPieces)>>::put(w, m_usProductPieces);
        Codec<std::remove_cvref_t<decltype(m_vecSocketOption)>>::put(w, m_vecSocketOption);
    }

    static KBillPackageInfo read_from(Reader& r) {
        KBillPackageInfo v;
        v.m_iPackageNo = Codec<std::remove_cvref_t<decltype(v.m_iPackageNo)>>::get(r);
        v.m_vecProductNo = Codec<std::remove_cvref_t<decltype(v.m_vecProductNo)>>::get(r);
        v.m_ulProductNo = Codec<std::remove_cvref_t<decltype(v.m_ulProductNo)>>::get(r);
        v.m_iProductID = Codec<std::remove_cvref_t<decltype(v.m_iProductID)>>::get(r);
        v.m_wstrProductID = Codec<std::remove_cvref_t<decltype(v.m_wstrProductID)>>::get(r);
        v.m_sProductExpire = Codec<std::remove_cvref_t<decltype(v.m_sProductExpire)>>::get(r);
        v.m_usProductPieces = Codec<std::remove_cvref_t<decltype(v.m_usProductPieces)>>::get(r);
        v.m_vecSocketOption = Codec<std::remove_cvref_t<decltype(v.m_vecSocketOption)>>::get(r);
        return v;
    }
};

struct KBillProductAttribute {
    std::int32_t              m_iProductNo{};  // int
    std::vector<std::int32_t> m_vecAttribute{};  // std::vector< int >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iProductNo)>>::put(w, m_iProductNo);
        Codec<std::remove_cvref_t<decltype(m_vecAttribute)>>::put(w, m_vecAttribute);
    }

    static KBillProductAttribute read_from(Reader& r) {
        KBillProductAttribute v;
        v.m_iProductNo = Codec<std::remove_cvref_t<decltype(v.m_iProductNo)>>::get(r);
        v.m_vecAttribute = Codec<std::remove_cvref_t<decltype(v.m_vecAttribute)>>::get(r);
        return v;
    }
};

struct KBillProductCategory {
    char           m_cCategoryNo{};
    std::u16string m_wstrCategoryName{};  // std::wstring
    char           m_cParentCategoryNo{};
    char           m_cDisplayNo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cCategoryNo)>>::put(w, m_cCategoryNo);
        Codec<std::remove_cvref_t<decltype(m_wstrCategoryName)>>::put(w, m_wstrCategoryName);
        Codec<std::remove_cvref_t<decltype(m_cParentCategoryNo)>>::put(w, m_cParentCategoryNo);
        Codec<std::remove_cvref_t<decltype(m_cDisplayNo)>>::put(w, m_cDisplayNo);
    }

    static KBillProductCategory read_from(Reader& r) {
        KBillProductCategory v;
        v.m_cCategoryNo = Codec<std::remove_cvref_t<decltype(v.m_cCategoryNo)>>::get(r);
        v.m_wstrCategoryName = Codec<std::remove_cvref_t<decltype(v.m_wstrCategoryName)>>::get(r);
        v.m_cParentCategoryNo = Codec<std::remove_cvref_t<decltype(v.m_cParentCategoryNo)>>::get(r);
        v.m_cDisplayNo = Codec<std::remove_cvref_t<decltype(v.m_cDisplayNo)>>::get(r);
        return v;
    }
};

struct KBillProductInfo {
    std::int32_t   m_iProductNo{};  // int
    std::int32_t   m_iProductID{};  // int
    bool           m_bSale{};
    bool           m_bEvent{};
    bool           m_bShow{};
    bool           m_bIsCashInvenSkip{};
    std::u16string m_wstrProductName{};  // std::wstring
    char           m_cCategoryNo{};
    std::int32_t   m_iSalePrice{};  // int
    std::int32_t   m_iRealPrice{};  // int
    char           m_cPeriod{};
    char           m_cQuantity{};
    bool           m_bEnableGift{};
    char           m_cLimitLevel{};
    std::u16string m_wstrEndDate{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iProductNo)>>::put(w, m_iProductNo);
        Codec<std::remove_cvref_t<decltype(m_iProductID)>>::put(w, m_iProductID);
        Codec<std::remove_cvref_t<decltype(m_bSale)>>::put(w, m_bSale);
        Codec<std::remove_cvref_t<decltype(m_bEvent)>>::put(w, m_bEvent);
        Codec<std::remove_cvref_t<decltype(m_bShow)>>::put(w, m_bShow);
        Codec<std::remove_cvref_t<decltype(m_bIsCashInvenSkip)>>::put(w, m_bIsCashInvenSkip);
        Codec<std::remove_cvref_t<decltype(m_wstrProductName)>>::put(w, m_wstrProductName);
        Codec<std::remove_cvref_t<decltype(m_cCategoryNo)>>::put(w, m_cCategoryNo);
        Codec<std::remove_cvref_t<decltype(m_iSalePrice)>>::put(w, m_iSalePrice);
        Codec<std::remove_cvref_t<decltype(m_iRealPrice)>>::put(w, m_iRealPrice);
        Codec<std::remove_cvref_t<decltype(m_cPeriod)>>::put(w, m_cPeriod);
        Codec<std::remove_cvref_t<decltype(m_cQuantity)>>::put(w, m_cQuantity);
        Codec<std::remove_cvref_t<decltype(m_bEnableGift)>>::put(w, m_bEnableGift);
        Codec<std::remove_cvref_t<decltype(m_cLimitLevel)>>::put(w, m_cLimitLevel);
        Codec<std::remove_cvref_t<decltype(m_wstrEndDate)>>::put(w, m_wstrEndDate);
    }

    static KBillProductInfo read_from(Reader& r) {
        KBillProductInfo v;
        v.m_iProductNo = Codec<std::remove_cvref_t<decltype(v.m_iProductNo)>>::get(r);
        v.m_iProductID = Codec<std::remove_cvref_t<decltype(v.m_iProductID)>>::get(r);
        v.m_bSale = Codec<std::remove_cvref_t<decltype(v.m_bSale)>>::get(r);
        v.m_bEvent = Codec<std::remove_cvref_t<decltype(v.m_bEvent)>>::get(r);
        v.m_bShow = Codec<std::remove_cvref_t<decltype(v.m_bShow)>>::get(r);
        v.m_bIsCashInvenSkip = Codec<std::remove_cvref_t<decltype(v.m_bIsCashInvenSkip)>>::get(r);
        v.m_wstrProductName = Codec<std::remove_cvref_t<decltype(v.m_wstrProductName)>>::get(r);
        v.m_cCategoryNo = Codec<std::remove_cvref_t<decltype(v.m_cCategoryNo)>>::get(r);
        v.m_iSalePrice = Codec<std::remove_cvref_t<decltype(v.m_iSalePrice)>>::get(r);
        v.m_iRealPrice = Codec<std::remove_cvref_t<decltype(v.m_iRealPrice)>>::get(r);
        v.m_cPeriod = Codec<std::remove_cvref_t<decltype(v.m_cPeriod)>>::get(r);
        v.m_cQuantity = Codec<std::remove_cvref_t<decltype(v.m_cQuantity)>>::get(r);
        v.m_bEnableGift = Codec<std::remove_cvref_t<decltype(v.m_bEnableGift)>>::get(r);
        v.m_cLimitLevel = Codec<std::remove_cvref_t<decltype(v.m_cLimitLevel)>>::get(r);
        v.m_wstrEndDate = Codec<std::remove_cvref_t<decltype(v.m_wstrEndDate)>>::get(r);
        return v;
    }
};

struct KBillOrderInfo {
    std::int64_t              m_iTransNo{};  // UidType
    std::u16string            m_wstrOrderID{};  // std::wstring
    KBillProductInfo          m_kBillProductInfo{};
    std::vector<std::int32_t> m_vecSocketOption{};  // std::vector< int >
    std::uint8_t              m_byteType{};  // byte
    std::u16string            m_wstrSenderNickName{};  // std::wstring
    std::u16string            m_wstrSenderPresentMessage{};  // std::wstring
    std::int64_t              m_iFromUnitUID{};  // UidType
    std::uint8_t              m_byteProductKind{};  // BYTE

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iTransNo)>>::put(w, m_iTransNo);
        Codec<std::remove_cvref_t<decltype(m_wstrOrderID)>>::put(w, m_wstrOrderID);
        Codec<std::remove_cvref_t<decltype(m_kBillProductInfo)>>::put(w, m_kBillProductInfo);
        Codec<std::remove_cvref_t<decltype(m_vecSocketOption)>>::put(w, m_vecSocketOption);
        Codec<std::remove_cvref_t<decltype(m_byteType)>>::put(w, m_byteType);
        Codec<std::remove_cvref_t<decltype(m_wstrSenderNickName)>>::put(w, m_wstrSenderNickName);
        Codec<std::remove_cvref_t<decltype(m_wstrSenderPresentMessage)>>::put(w, m_wstrSenderPresentMessage);
        Codec<std::remove_cvref_t<decltype(m_iFromUnitUID)>>::put(w, m_iFromUnitUID);
        Codec<std::remove_cvref_t<decltype(m_byteProductKind)>>::put(w, m_byteProductKind);
    }

    static KBillOrderInfo read_from(Reader& r) {
        KBillOrderInfo v;
        v.m_iTransNo = Codec<std::remove_cvref_t<decltype(v.m_iTransNo)>>::get(r);
        v.m_wstrOrderID = Codec<std::remove_cvref_t<decltype(v.m_wstrOrderID)>>::get(r);
        v.m_kBillProductInfo = Codec<std::remove_cvref_t<decltype(v.m_kBillProductInfo)>>::get(r);
        v.m_vecSocketOption = Codec<std::remove_cvref_t<decltype(v.m_vecSocketOption)>>::get(r);
        v.m_byteType = Codec<std::remove_cvref_t<decltype(v.m_byteType)>>::get(r);
        v.m_wstrSenderNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrSenderNickName)>>::get(r);
        v.m_wstrSenderPresentMessage = Codec<std::remove_cvref_t<decltype(v.m_wstrSenderPresentMessage)>>::get(r);
        v.m_iFromUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iFromUnitUID)>>::get(r);
        v.m_byteProductKind = Codec<std::remove_cvref_t<decltype(v.m_byteProductKind)>>::get(r);
        return v;
    }
};

struct KBlockInfo {
    std::int64_t   m_iBlockUID{};  // UidType
    char           m_cBlockType{};
    std::int64_t   m_iBlockTarget{};  // UidType
    std::u16string m_wstrBlockTarget{};  // std::wstring
    std::u16string m_wstrStartDate{};  // std::wstring
    std::u16string m_wstrEndDate{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iBlockUID)>>::put(w, m_iBlockUID);
        Codec<std::remove_cvref_t<decltype(m_cBlockType)>>::put(w, m_cBlockType);
        Codec<std::remove_cvref_t<decltype(m_iBlockTarget)>>::put(w, m_iBlockTarget);
        Codec<std::remove_cvref_t<decltype(m_wstrBlockTarget)>>::put(w, m_wstrBlockTarget);
        Codec<std::remove_cvref_t<decltype(m_wstrStartDate)>>::put(w, m_wstrStartDate);
        Codec<std::remove_cvref_t<decltype(m_wstrEndDate)>>::put(w, m_wstrEndDate);
    }

    static KBlockInfo read_from(Reader& r) {
        KBlockInfo v;
        v.m_iBlockUID = Codec<std::remove_cvref_t<decltype(v.m_iBlockUID)>>::get(r);
        v.m_cBlockType = Codec<std::remove_cvref_t<decltype(v.m_cBlockType)>>::get(r);
        v.m_iBlockTarget = Codec<std::remove_cvref_t<decltype(v.m_iBlockTarget)>>::get(r);
        v.m_wstrBlockTarget = Codec<std::remove_cvref_t<decltype(v.m_wstrBlockTarget)>>::get(r);
        v.m_wstrStartDate = Codec<std::remove_cvref_t<decltype(v.m_wstrStartDate)>>::get(r);
        v.m_wstrEndDate = Codec<std::remove_cvref_t<decltype(v.m_wstrEndDate)>>::get(r);
        return v;
    }
};

struct KBossFieldCreateInfo {
    std::int64_t m_tFieldHoldingTime{};  // __time64_t
    std::int64_t m_tPortalOpenTime{};  // __time64_t
    std::int32_t m_iBossFieldID{};  // int
    std::int32_t m_iPortalMapID{};  // int
    bool         m_bBossField{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_tFieldHoldingTime)>>::put(w, m_tFieldHoldingTime);
        Codec<std::remove_cvref_t<decltype(m_tPortalOpenTime)>>::put(w, m_tPortalOpenTime);
        Codec<std::remove_cvref_t<decltype(m_iBossFieldID)>>::put(w, m_iBossFieldID);
        Codec<std::remove_cvref_t<decltype(m_iPortalMapID)>>::put(w, m_iPortalMapID);
        Codec<std::remove_cvref_t<decltype(m_bBossField)>>::put(w, m_bBossField);
    }

    static KBossFieldCreateInfo read_from(Reader& r) {
        KBossFieldCreateInfo v;
        v.m_tFieldHoldingTime = Codec<std::remove_cvref_t<decltype(v.m_tFieldHoldingTime)>>::get(r);
        v.m_tPortalOpenTime = Codec<std::remove_cvref_t<decltype(v.m_tPortalOpenTime)>>::get(r);
        v.m_iBossFieldID = Codec<std::remove_cvref_t<decltype(v.m_iBossFieldID)>>::get(r);
        v.m_iPortalMapID = Codec<std::remove_cvref_t<decltype(v.m_iPortalMapID)>>::get(r);
        v.m_bBossField = Codec<std::remove_cvref_t<decltype(v.m_bBossField)>>::get(r);
        return v;
    }
};

struct KBossFieldJoinInfo {
    std::int32_t  m_iReturnMapID{};  // int
    std::uint8_t  m_ucLastTouchLineIndex{};  // unsigned char
    std::uint16_t m_usLastPosValue{};  // unsigned short
    bool          m_bIgnoreLastTouch{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iReturnMapID)>>::put(w, m_iReturnMapID);
        Codec<std::remove_cvref_t<decltype(m_ucLastTouchLineIndex)>>::put(w, m_ucLastTouchLineIndex);
        Codec<std::remove_cvref_t<decltype(m_usLastPosValue)>>::put(w, m_usLastPosValue);
        Codec<std::remove_cvref_t<decltype(m_bIgnoreLastTouch)>>::put(w, m_bIgnoreLastTouch);
    }

    static KBossFieldJoinInfo read_from(Reader& r) {
        KBossFieldJoinInfo v;
        v.m_iReturnMapID = Codec<std::remove_cvref_t<decltype(v.m_iReturnMapID)>>::get(r);
        v.m_ucLastTouchLineIndex = Codec<std::remove_cvref_t<decltype(v.m_ucLastTouchLineIndex)>>::get(r);
        v.m_usLastPosValue = Codec<std::remove_cvref_t<decltype(v.m_usLastPosValue)>>::get(r);
        v.m_bIgnoreLastTouch = Codec<std::remove_cvref_t<decltype(v.m_bIgnoreLastTouch)>>::get(r);
        return v;
    }
};

struct KBossFieldLog {
    std::int32_t   m_iBossFieldID{};  // int
    std::int32_t   m_iPortalMapID{};  // int
    std::int32_t   m_iPlayTime{};  // int
    std::uint8_t   m_byteRemainUserCount{};  // byte
    bool           m_bSucceed{};
    std::u16string m_wstrPortalOpenTime{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iBossFieldID)>>::put(w, m_iBossFieldID);
        Codec<std::remove_cvref_t<decltype(m_iPortalMapID)>>::put(w, m_iPortalMapID);
        Codec<std::remove_cvref_t<decltype(m_iPlayTime)>>::put(w, m_iPlayTime);
        Codec<std::remove_cvref_t<decltype(m_byteRemainUserCount)>>::put(w, m_byteRemainUserCount);
        Codec<std::remove_cvref_t<decltype(m_bSucceed)>>::put(w, m_bSucceed);
        Codec<std::remove_cvref_t<decltype(m_wstrPortalOpenTime)>>::put(w, m_wstrPortalOpenTime);
    }

    static KBossFieldLog read_from(Reader& r) {
        KBossFieldLog v;
        v.m_iBossFieldID = Codec<std::remove_cvref_t<decltype(v.m_iBossFieldID)>>::get(r);
        v.m_iPortalMapID = Codec<std::remove_cvref_t<decltype(v.m_iPortalMapID)>>::get(r);
        v.m_iPlayTime = Codec<std::remove_cvref_t<decltype(v.m_iPlayTime)>>::get(r);
        v.m_byteRemainUserCount = Codec<std::remove_cvref_t<decltype(v.m_byteRemainUserCount)>>::get(r);
        v.m_bSucceed = Codec<std::remove_cvref_t<decltype(v.m_bSucceed)>>::get(r);
        v.m_wstrPortalOpenTime = Codec<std::remove_cvref_t<decltype(v.m_wstrPortalOpenTime)>>::get(r);
        return v;
    }
};

struct KBossFieldUserLog {
    std::int64_t   m_iUnitUID{};  // UidType
    std::int64_t   m_iGivenDamage{};  // __int64
    std::int64_t   m_iAttackDamage{};  // __int64
    std::int32_t   m_iEXP{};  // int
    std::int32_t   m_iED{};  // int
    std::int16_t   m_sNumResurrectionStone{};  // short
    std::uint8_t   m_byteLevel{};  // byte
    std::uint8_t   m_byteClass{};  // byte
    std::uint8_t   m_byteCompletionType{};  // byte
    std::uint8_t   m_byteContributionRank{};  // byte
    std::u16string m_wstrNickName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iGivenDamage)>>::put(w, m_iGivenDamage);
        Codec<std::remove_cvref_t<decltype(m_iAttackDamage)>>::put(w, m_iAttackDamage);
        Codec<std::remove_cvref_t<decltype(m_iEXP)>>::put(w, m_iEXP);
        Codec<std::remove_cvref_t<decltype(m_iED)>>::put(w, m_iED);
        Codec<std::remove_cvref_t<decltype(m_sNumResurrectionStone)>>::put(w, m_sNumResurrectionStone);
        Codec<std::remove_cvref_t<decltype(m_byteLevel)>>::put(w, m_byteLevel);
        Codec<std::remove_cvref_t<decltype(m_byteClass)>>::put(w, m_byteClass);
        Codec<std::remove_cvref_t<decltype(m_byteCompletionType)>>::put(w, m_byteCompletionType);
        Codec<std::remove_cvref_t<decltype(m_byteContributionRank)>>::put(w, m_byteContributionRank);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
    }

    static KBossFieldUserLog read_from(Reader& r) {
        KBossFieldUserLog v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iGivenDamage = Codec<std::remove_cvref_t<decltype(v.m_iGivenDamage)>>::get(r);
        v.m_iAttackDamage = Codec<std::remove_cvref_t<decltype(v.m_iAttackDamage)>>::get(r);
        v.m_iEXP = Codec<std::remove_cvref_t<decltype(v.m_iEXP)>>::get(r);
        v.m_iED = Codec<std::remove_cvref_t<decltype(v.m_iED)>>::get(r);
        v.m_sNumResurrectionStone = Codec<std::remove_cvref_t<decltype(v.m_sNumResurrectionStone)>>::get(r);
        v.m_byteLevel = Codec<std::remove_cvref_t<decltype(v.m_byteLevel)>>::get(r);
        v.m_byteClass = Codec<std::remove_cvref_t<decltype(v.m_byteClass)>>::get(r);
        v.m_byteCompletionType = Codec<std::remove_cvref_t<decltype(v.m_byteCompletionType)>>::get(r);
        v.m_byteContributionRank = Codec<std::remove_cvref_t<decltype(v.m_byteContributionRank)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        return v;
    }
};

struct KBuffBehaviorFactor {
    std::uint32_t      m_uiType{};  // UINT
    std::vector<float> m_vecValues{};  // std::vector< float >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_uiType)>>::put(w, m_uiType);
        Codec<std::remove_cvref_t<decltype(m_vecValues)>>::put(w, m_vecValues);
    }

    static KBuffBehaviorFactor read_from(Reader& r) {
        KBuffBehaviorFactor v;
        v.m_uiType = Codec<std::remove_cvref_t<decltype(v.m_uiType)>>::get(r);
        v.m_vecValues = Codec<std::remove_cvref_t<decltype(v.m_vecValues)>>::get(r);
        return v;
    }
};

struct KBuffFinalizerFactor {
    std::uint32_t      m_uiType{};  // UINT
    std::vector<float> m_vecValues{};  // std::vector< float >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_uiType)>>::put(w, m_uiType);
        Codec<std::remove_cvref_t<decltype(m_vecValues)>>::put(w, m_vecValues);
    }

    static KBuffFinalizerFactor read_from(Reader& r) {
        KBuffFinalizerFactor v;
        v.m_uiType = Codec<std::remove_cvref_t<decltype(v.m_uiType)>>::get(r);
        v.m_vecValues = Codec<std::remove_cvref_t<decltype(v.m_vecValues)>>::get(r);
        return v;
    }
};

struct KBuffIdentity {
    std::int32_t  m_eBuffTempletID{};  // int
    std::uint32_t m_uiUniqueNum{};  // UINT

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_eBuffTempletID)>>::put(w, m_eBuffTempletID);
        Codec<std::remove_cvref_t<decltype(m_uiUniqueNum)>>::put(w, m_uiUniqueNum);
    }

    static KBuffIdentity read_from(Reader& r) {
        KBuffIdentity v;
        v.m_eBuffTempletID = Codec<std::remove_cvref_t<decltype(v.m_eBuffTempletID)>>::get(r);
        v.m_uiUniqueNum = Codec<std::remove_cvref_t<decltype(v.m_uiUniqueNum)>>::get(r);
        return v;
    }
};

struct KBuffFactor {
    std::vector<KBuffBehaviorFactor>  m_vecBuffBehaviorFactors{};  // std::vector< KBuffBehaviorFactor >
    std::vector<KBuffFinalizerFactor> m_vecBuffFinalizerFactors{};  // std::vector< KBuffFinalizerFactor >
    KBuffIdentity                     m_BuffIdentity{};
    float                             m_fAccumulationMultiplier{};
    std::uint8_t                      m_ucAccumulationCountNow{};  // UCHAR
    std::int32_t                      m_iFactorID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecBuffBehaviorFactors)>>::put(w, m_vecBuffBehaviorFactors);
        Codec<std::remove_cvref_t<decltype(m_vecBuffFinalizerFactors)>>::put(w, m_vecBuffFinalizerFactors);
        Codec<std::remove_cvref_t<decltype(m_BuffIdentity)>>::put(w, m_BuffIdentity);
        Codec<std::remove_cvref_t<decltype(m_fAccumulationMultiplier)>>::put(w, m_fAccumulationMultiplier);
        Codec<std::remove_cvref_t<decltype(m_ucAccumulationCountNow)>>::put(w, m_ucAccumulationCountNow);
        Codec<std::remove_cvref_t<decltype(m_iFactorID)>>::put(w, m_iFactorID);
    }

    static KBuffFactor read_from(Reader& r) {
        KBuffFactor v;
        v.m_vecBuffBehaviorFactors = Codec<std::remove_cvref_t<decltype(v.m_vecBuffBehaviorFactors)>>::get(r);
        v.m_vecBuffFinalizerFactors = Codec<std::remove_cvref_t<decltype(v.m_vecBuffFinalizerFactors)>>::get(r);
        v.m_BuffIdentity = Codec<std::remove_cvref_t<decltype(v.m_BuffIdentity)>>::get(r);
        v.m_fAccumulationMultiplier = Codec<std::remove_cvref_t<decltype(v.m_fAccumulationMultiplier)>>::get(r);
        v.m_ucAccumulationCountNow = Codec<std::remove_cvref_t<decltype(v.m_ucAccumulationCountNow)>>::get(r);
        v.m_iFactorID = Codec<std::remove_cvref_t<decltype(v.m_iFactorID)>>::get(r);
        return v;
    }
};

struct KBuffInfo {
    KBuffFactor  m_kFactorInfo{};
    std::int64_t m_iBuffStartTime{};  // __int64
    std::int64_t m_iBuffEndTime{};  // __int64

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kFactorInfo)>>::put(w, m_kFactorInfo);
        Codec<std::remove_cvref_t<decltype(m_iBuffStartTime)>>::put(w, m_iBuffStartTime);
        Codec<std::remove_cvref_t<decltype(m_iBuffEndTime)>>::put(w, m_iBuffEndTime);
    }

    static KBuffInfo read_from(Reader& r) {
        KBuffInfo v;
        v.m_kFactorInfo = Codec<std::remove_cvref_t<decltype(v.m_kFactorInfo)>>::get(r);
        v.m_iBuffStartTime = Codec<std::remove_cvref_t<decltype(v.m_iBuffStartTime)>>::get(r);
        v.m_iBuffEndTime = Codec<std::remove_cvref_t<decltype(v.m_iBuffEndTime)>>::get(r);
        return v;
    }
};

struct KBuffSkillInfo {
    std::int64_t m_iCaughtUnitUID{};  // UidType
    std::int32_t m_iBuffEnum{};  // int
    std::int64_t m_iCastingUnitUID{};  // UidType
    bool         m_bIsBuffCasting{};
    char         m_cBuffLevel{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iCaughtUnitUID)>>::put(w, m_iCaughtUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iBuffEnum)>>::put(w, m_iBuffEnum);
        Codec<std::remove_cvref_t<decltype(m_iCastingUnitUID)>>::put(w, m_iCastingUnitUID);
        Codec<std::remove_cvref_t<decltype(m_bIsBuffCasting)>>::put(w, m_bIsBuffCasting);
        Codec<std::remove_cvref_t<decltype(m_cBuffLevel)>>::put(w, m_cBuffLevel);
    }

    static KBuffSkillInfo read_from(Reader& r) {
        KBuffSkillInfo v;
        v.m_iCaughtUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iCaughtUnitUID)>>::get(r);
        v.m_iBuffEnum = Codec<std::remove_cvref_t<decltype(v.m_iBuffEnum)>>::get(r);
        v.m_iCastingUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iCastingUnitUID)>>::get(r);
        v.m_bIsBuffCasting = Codec<std::remove_cvref_t<decltype(v.m_bIsBuffCasting)>>::get(r);
        v.m_cBuffLevel = Codec<std::remove_cvref_t<decltype(v.m_cBuffLevel)>>::get(r);
        return v;
    }
};

struct KBuyCharInfo {
    char         m_cLevel{};
    std::int32_t m_iUnitClass{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cLevel)>>::put(w, m_cLevel);
        Codec<std::remove_cvref_t<decltype(m_iUnitClass)>>::put(w, m_iUnitClass);
    }

    static KBuyCharInfo read_from(Reader& r) {
        KBuyCharInfo v;
        v.m_cLevel = Codec<std::remove_cvref_t<decltype(v.m_cLevel)>>::get(r);
        v.m_iUnitClass = Codec<std::remove_cvref_t<decltype(v.m_iUnitClass)>>::get(r);
        return v;
    }
};

struct KBuyGPItemInfo {
    std::int32_t m_iItemID{};  // int
    std::int32_t m_iPrice{};  // int
    std::int32_t m_iPvPPoint{};  // int
    char         m_cPeriodType{};
    std::int32_t m_iQuantity{};  // int
    std::int16_t m_sEndurance{};  // short
    std::int16_t m_sPeriod{};  // short

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iItemID)>>::put(w, m_iItemID);
        Codec<std::remove_cvref_t<decltype(m_iPrice)>>::put(w, m_iPrice);
        Codec<std::remove_cvref_t<decltype(m_iPvPPoint)>>::put(w, m_iPvPPoint);
        Codec<std::remove_cvref_t<decltype(m_cPeriodType)>>::put(w, m_cPeriodType);
        Codec<std::remove_cvref_t<decltype(m_iQuantity)>>::put(w, m_iQuantity);
        Codec<std::remove_cvref_t<decltype(m_sEndurance)>>::put(w, m_sEndurance);
        Codec<std::remove_cvref_t<decltype(m_sPeriod)>>::put(w, m_sPeriod);
    }

    static KBuyGPItemInfo read_from(Reader& r) {
        KBuyGPItemInfo v;
        v.m_iItemID = Codec<std::remove_cvref_t<decltype(v.m_iItemID)>>::get(r);
        v.m_iPrice = Codec<std::remove_cvref_t<decltype(v.m_iPrice)>>::get(r);
        v.m_iPvPPoint = Codec<std::remove_cvref_t<decltype(v.m_iPvPPoint)>>::get(r);
        v.m_cPeriodType = Codec<std::remove_cvref_t<decltype(v.m_cPeriodType)>>::get(r);
        v.m_iQuantity = Codec<std::remove_cvref_t<decltype(v.m_iQuantity)>>::get(r);
        v.m_sEndurance = Codec<std::remove_cvref_t<decltype(v.m_sEndurance)>>::get(r);
        v.m_sPeriod = Codec<std::remove_cvref_t<decltype(v.m_sPeriod)>>::get(r);
        return v;
    }
};

struct KCCUInfo {
    std::int64_t m_iCCUTime{};  // __int64
    std::int32_t m_iCCUCount{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iCCUTime)>>::put(w, m_iCCUTime);
        Codec<std::remove_cvref_t<decltype(m_iCCUCount)>>::put(w, m_iCCUCount);
    }

    static KCCUInfo read_from(Reader& r) {
        KCCUInfo v;
        v.m_iCCUTime = Codec<std::remove_cvref_t<decltype(v.m_iCCUTime)>>::get(r);
        v.m_iCCUCount = Codec<std::remove_cvref_t<decltype(v.m_iCCUCount)>>::get(r);
        return v;
    }
};

struct KCashItemResolveInfo {
    std::int32_t m_iItemID{};  // int
    std::int32_t m_iSalePrice{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iItemID)>>::put(w, m_iItemID);
        Codec<std::remove_cvref_t<decltype(m_iSalePrice)>>::put(w, m_iSalePrice);
    }

    static KCashItemResolveInfo read_from(Reader& r) {
        KCashItemResolveInfo v;
        v.m_iItemID = Codec<std::remove_cvref_t<decltype(v.m_iItemID)>>::get(r);
        v.m_iSalePrice = Codec<std::remove_cvref_t<decltype(v.m_iSalePrice)>>::get(r);
        return v;
    }
};

struct KCashPruductInfo {
    std::uint32_t m_ulProductNo{};  // unsigned long
    std::int16_t  m_sProductExpire{};  // short
    std::uint16_t m_usProductPieces{};  // u_short
    std::int32_t  m_iItemID{};  // int
    std::uint32_t m_ulSalePrice{};  // unsigned long

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_ulProductNo)>>::put(w, m_ulProductNo);
        Codec<std::remove_cvref_t<decltype(m_sProductExpire)>>::put(w, m_sProductExpire);
        Codec<std::remove_cvref_t<decltype(m_usProductPieces)>>::put(w, m_usProductPieces);
        Codec<std::remove_cvref_t<decltype(m_iItemID)>>::put(w, m_iItemID);
        Codec<std::remove_cvref_t<decltype(m_ulSalePrice)>>::put(w, m_ulSalePrice);
    }

    static KCashPruductInfo read_from(Reader& r) {
        KCashPruductInfo v;
        v.m_ulProductNo = Codec<std::remove_cvref_t<decltype(v.m_ulProductNo)>>::get(r);
        v.m_sProductExpire = Codec<std::remove_cvref_t<decltype(v.m_sProductExpire)>>::get(r);
        v.m_usProductPieces = Codec<std::remove_cvref_t<decltype(v.m_usProductPieces)>>::get(r);
        v.m_iItemID = Codec<std::remove_cvref_t<decltype(v.m_iItemID)>>::get(r);
        v.m_ulSalePrice = Codec<std::remove_cvref_t<decltype(v.m_ulSalePrice)>>::get(r);
        return v;
    }
};

struct KChangeGuildNameCheckREQ {
    std::int64_t   m_iUnitUID{};  // UidType
    std::u16string m_wstrNewGuildName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrNewGuildName)>>::put(w, m_wstrNewGuildName);
    }

    static KChangeGuildNameCheckREQ read_from(Reader& r) {
        KChangeGuildNameCheckREQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_wstrNewGuildName = Codec<std::remove_cvref_t<decltype(v.m_wstrNewGuildName)>>::get(r);
        return v;
    }
};

struct KChangeGuildNameREQ {
    std::int64_t   m_iUnitUID{};  // UidType
    std::int32_t   m_iGuildUID{};  // int
    std::u16string m_wstrOldGuildName{};  // std::wstring
    std::u16string m_wstrNewGuildName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iGuildUID)>>::put(w, m_iGuildUID);
        Codec<std::remove_cvref_t<decltype(m_wstrOldGuildName)>>::put(w, m_wstrOldGuildName);
        Codec<std::remove_cvref_t<decltype(m_wstrNewGuildName)>>::put(w, m_wstrNewGuildName);
    }

    static KChangeGuildNameREQ read_from(Reader& r) {
        KChangeGuildNameREQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iGuildUID = Codec<std::remove_cvref_t<decltype(v.m_iGuildUID)>>::get(r);
        v.m_wstrOldGuildName = Codec<std::remove_cvref_t<decltype(v.m_wstrOldGuildName)>>::get(r);
        v.m_wstrNewGuildName = Codec<std::remove_cvref_t<decltype(v.m_wstrNewGuildName)>>::get(r);
        return v;
    }
};

struct KChannelBonusInfo {
    bool         m_bEnable{};
    std::int32_t m_iBeginLv{};  // int
    std::int32_t m_iEndLv{};  // int
    std::int32_t m_iPerExp{};  // int
    std::int32_t m_iPerED{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bEnable)>>::put(w, m_bEnable);
        Codec<std::remove_cvref_t<decltype(m_iBeginLv)>>::put(w, m_iBeginLv);
        Codec<std::remove_cvref_t<decltype(m_iEndLv)>>::put(w, m_iEndLv);
        Codec<std::remove_cvref_t<decltype(m_iPerExp)>>::put(w, m_iPerExp);
        Codec<std::remove_cvref_t<decltype(m_iPerED)>>::put(w, m_iPerED);
    }

    static KChannelBonusInfo read_from(Reader& r) {
        KChannelBonusInfo v;
        v.m_bEnable = Codec<std::remove_cvref_t<decltype(v.m_bEnable)>>::get(r);
        v.m_iBeginLv = Codec<std::remove_cvref_t<decltype(v.m_iBeginLv)>>::get(r);
        v.m_iEndLv = Codec<std::remove_cvref_t<decltype(v.m_iEndLv)>>::get(r);
        v.m_iPerExp = Codec<std::remove_cvref_t<decltype(v.m_iPerExp)>>::get(r);
        v.m_iPerED = Codec<std::remove_cvref_t<decltype(v.m_iPerED)>>::get(r);
        return v;
    }
};

struct KChannelInfo {
    std::int32_t   m_iServerUID{};  // int
    std::int32_t   m_iChannelID{};  // int
    std::u16string m_wstrChannelName{};  // std::wstring
    std::int32_t   m_iServerGroupID{};  // int
    std::u16string m_wstrIP{};  // std::wstring
    std::uint16_t  m_usMasterPort{};  // u_short
    std::uint16_t  m_usNCUDPPort{};  // u_short
    std::int32_t   m_iMaxUser{};  // int
    std::int32_t   m_iCurrentUser{};  // int
    std::int32_t   m_iCurPartyCount{};  // int
    std::int32_t   m_iPlayGamePartyCount{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iServerUID)>>::put(w, m_iServerUID);
        Codec<std::remove_cvref_t<decltype(m_iChannelID)>>::put(w, m_iChannelID);
        Codec<std::remove_cvref_t<decltype(m_wstrChannelName)>>::put(w, m_wstrChannelName);
        Codec<std::remove_cvref_t<decltype(m_iServerGroupID)>>::put(w, m_iServerGroupID);
        Codec<std::remove_cvref_t<decltype(m_wstrIP)>>::put(w, m_wstrIP);
        Codec<std::remove_cvref_t<decltype(m_usMasterPort)>>::put(w, m_usMasterPort);
        Codec<std::remove_cvref_t<decltype(m_usNCUDPPort)>>::put(w, m_usNCUDPPort);
        Codec<std::remove_cvref_t<decltype(m_iMaxUser)>>::put(w, m_iMaxUser);
        Codec<std::remove_cvref_t<decltype(m_iCurrentUser)>>::put(w, m_iCurrentUser);
        Codec<std::remove_cvref_t<decltype(m_iCurPartyCount)>>::put(w, m_iCurPartyCount);
        Codec<std::remove_cvref_t<decltype(m_iPlayGamePartyCount)>>::put(w, m_iPlayGamePartyCount);
    }

    static KChannelInfo read_from(Reader& r) {
        KChannelInfo v;
        v.m_iServerUID = Codec<std::remove_cvref_t<decltype(v.m_iServerUID)>>::get(r);
        v.m_iChannelID = Codec<std::remove_cvref_t<decltype(v.m_iChannelID)>>::get(r);
        v.m_wstrChannelName = Codec<std::remove_cvref_t<decltype(v.m_wstrChannelName)>>::get(r);
        v.m_iServerGroupID = Codec<std::remove_cvref_t<decltype(v.m_iServerGroupID)>>::get(r);
        v.m_wstrIP = Codec<std::remove_cvref_t<decltype(v.m_wstrIP)>>::get(r);
        v.m_usMasterPort = Codec<std::remove_cvref_t<decltype(v.m_usMasterPort)>>::get(r);
        v.m_usNCUDPPort = Codec<std::remove_cvref_t<decltype(v.m_usNCUDPPort)>>::get(r);
        v.m_iMaxUser = Codec<std::remove_cvref_t<decltype(v.m_iMaxUser)>>::get(r);
        v.m_iCurrentUser = Codec<std::remove_cvref_t<decltype(v.m_iCurrentUser)>>::get(r);
        v.m_iCurPartyCount = Codec<std::remove_cvref_t<decltype(v.m_iCurPartyCount)>>::get(r);
        v.m_iPlayGamePartyCount = Codec<std::remove_cvref_t<decltype(v.m_iPlayGamePartyCount)>>::get(r);
        return v;
    }
};

struct KChannelSimpleInfo {
    std::int32_t m_iChannelID{};  // int
    std::int32_t m_iMaxUser{};  // int
    std::int32_t m_iCurrentUser{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iChannelID)>>::put(w, m_iChannelID);
        Codec<std::remove_cvref_t<decltype(m_iMaxUser)>>::put(w, m_iMaxUser);
        Codec<std::remove_cvref_t<decltype(m_iCurrentUser)>>::put(w, m_iCurrentUser);
    }

    static KChannelSimpleInfo read_from(Reader& r) {
        KChannelSimpleInfo v;
        v.m_iChannelID = Codec<std::remove_cvref_t<decltype(v.m_iChannelID)>>::get(r);
        v.m_iMaxUser = Codec<std::remove_cvref_t<decltype(v.m_iMaxUser)>>::get(r);
        v.m_iCurrentUser = Codec<std::remove_cvref_t<decltype(v.m_iCurrentUser)>>::get(r);
        return v;
    }
};

struct KChatBlackListUnit {
    std::int64_t   m_iUnitUID{};  // UidType
    std::u16string m_wstrNickName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
    }

    static KChatBlackListUnit read_from(Reader& r) {
        KChatBlackListUnit v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        return v;
    }
};

struct KCommunityUserInfo {
    std::int64_t   m_iUnitUID{};  // UidType
    std::u16string m_wstrNickName{};  // std::wstring
    char           m_cUnitClass{};
    std::uint8_t   m_ucLevel{};  // UCHAR
    char           m_cState{};
    std::int64_t   m_iStateCode{};  // UidType
    std::int64_t   m_iRoomUID{};  // UidType
    std::int32_t   m_iED{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_cUnitClass)>>::put(w, m_cUnitClass);
        Codec<std::remove_cvref_t<decltype(m_ucLevel)>>::put(w, m_ucLevel);
        Codec<std::remove_cvref_t<decltype(m_cState)>>::put(w, m_cState);
        Codec<std::remove_cvref_t<decltype(m_iStateCode)>>::put(w, m_iStateCode);
        Codec<std::remove_cvref_t<decltype(m_iRoomUID)>>::put(w, m_iRoomUID);
        Codec<std::remove_cvref_t<decltype(m_iED)>>::put(w, m_iED);
    }

    static KCommunityUserInfo read_from(Reader& r) {
        KCommunityUserInfo v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_cUnitClass = Codec<std::remove_cvref_t<decltype(v.m_cUnitClass)>>::get(r);
        v.m_ucLevel = Codec<std::remove_cvref_t<decltype(v.m_ucLevel)>>::get(r);
        v.m_cState = Codec<std::remove_cvref_t<decltype(v.m_cState)>>::get(r);
        v.m_iStateCode = Codec<std::remove_cvref_t<decltype(v.m_iStateCode)>>::get(r);
        v.m_iRoomUID = Codec<std::remove_cvref_t<decltype(v.m_iRoomUID)>>::get(r);
        v.m_iED = Codec<std::remove_cvref_t<decltype(v.m_iED)>>::get(r);
        return v;
    }
};

struct KCompleteQuestInfo {
    std::int32_t m_iQuestID{};  // int
    std::int32_t m_iCompleteCount{};  // int
    std::int64_t m_tCompleteDate{};  // __int64

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iQuestID)>>::put(w, m_iQuestID);
        Codec<std::remove_cvref_t<decltype(m_iCompleteCount)>>::put(w, m_iCompleteCount);
        Codec<std::remove_cvref_t<decltype(m_tCompleteDate)>>::put(w, m_tCompleteDate);
    }

    static KCompleteQuestInfo read_from(Reader& r) {
        KCompleteQuestInfo v;
        v.m_iQuestID = Codec<std::remove_cvref_t<decltype(v.m_iQuestID)>>::get(r);
        v.m_iCompleteCount = Codec<std::remove_cvref_t<decltype(v.m_iCompleteCount)>>::get(r);
        v.m_tCompleteDate = Codec<std::remove_cvref_t<decltype(v.m_tCompleteDate)>>::get(r);
        return v;
    }
};

struct KConnectTimeEventInfo {
    std::int32_t   m_iEventUID{};  // int
    std::u16string m_wstrEventTime{};  // std::wstring
    bool           m_bAccountEvent{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iEventUID)>>::put(w, m_iEventUID);
        Codec<std::remove_cvref_t<decltype(m_wstrEventTime)>>::put(w, m_wstrEventTime);
        Codec<std::remove_cvref_t<decltype(m_bAccountEvent)>>::put(w, m_bAccountEvent);
    }

    static KConnectTimeEventInfo read_from(Reader& r) {
        KConnectTimeEventInfo v;
        v.m_iEventUID = Codec<std::remove_cvref_t<decltype(v.m_iEventUID)>>::get(r);
        v.m_wstrEventTime = Codec<std::remove_cvref_t<decltype(v.m_wstrEventTime)>>::get(r);
        v.m_bAccountEvent = Codec<std::remove_cvref_t<decltype(v.m_bAccountEvent)>>::get(r);
        return v;
    }
};

struct KConnectionUnitInfo {
    std::int64_t   m_iUnitUID{};  // UidType
    std::int32_t   m_nKNMSN{};  // int
    char           m_cUnitClass{};
    std::uint8_t   m_ucLevel{};  // u_char
    std::u16string m_wstrCharName{};  // std::wstring
    char           m_cRank{};
    std::int32_t   m_iRPoint{};  // int
    std::uint32_t  m_uiPVPRanking{};  // UINT
    std::int32_t   m_iWin{};  // int
    std::int32_t   m_iLose{};  // int
    std::uint32_t  m_uiKillNum{};  // UINT
    std::int32_t   m_iChannelID{};  // int
    char           m_cState{};
    std::int32_t   m_iStateCode{};  // int
    std::int64_t   m_iPartyUID{};  // UidType
    std::int64_t   m_iRoomUID{};  // UidType
    bool           m_bPublic{};
    bool           m_bCanIntrude{};
    std::int32_t   m_iDungeonID{};  // int
    char           m_iRoomListID{};
    std::u16string m_wstrGuildName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_nKNMSN)>>::put(w, m_nKNMSN);
        Codec<std::remove_cvref_t<decltype(m_cUnitClass)>>::put(w, m_cUnitClass);
        Codec<std::remove_cvref_t<decltype(m_ucLevel)>>::put(w, m_ucLevel);
        Codec<std::remove_cvref_t<decltype(m_wstrCharName)>>::put(w, m_wstrCharName);
        Codec<std::remove_cvref_t<decltype(m_cRank)>>::put(w, m_cRank);
        Codec<std::remove_cvref_t<decltype(m_iRPoint)>>::put(w, m_iRPoint);
        Codec<std::remove_cvref_t<decltype(m_uiPVPRanking)>>::put(w, m_uiPVPRanking);
        Codec<std::remove_cvref_t<decltype(m_iWin)>>::put(w, m_iWin);
        Codec<std::remove_cvref_t<decltype(m_iLose)>>::put(w, m_iLose);
        Codec<std::remove_cvref_t<decltype(m_uiKillNum)>>::put(w, m_uiKillNum);
        Codec<std::remove_cvref_t<decltype(m_iChannelID)>>::put(w, m_iChannelID);
        Codec<std::remove_cvref_t<decltype(m_cState)>>::put(w, m_cState);
        Codec<std::remove_cvref_t<decltype(m_iStateCode)>>::put(w, m_iStateCode);
        Codec<std::remove_cvref_t<decltype(m_iPartyUID)>>::put(w, m_iPartyUID);
        Codec<std::remove_cvref_t<decltype(m_iRoomUID)>>::put(w, m_iRoomUID);
        Codec<std::remove_cvref_t<decltype(m_bPublic)>>::put(w, m_bPublic);
        Codec<std::remove_cvref_t<decltype(m_bCanIntrude)>>::put(w, m_bCanIntrude);
        Codec<std::remove_cvref_t<decltype(m_iDungeonID)>>::put(w, m_iDungeonID);
        Codec<std::remove_cvref_t<decltype(m_iRoomListID)>>::put(w, m_iRoomListID);
        Codec<std::remove_cvref_t<decltype(m_wstrGuildName)>>::put(w, m_wstrGuildName);
    }

    static KConnectionUnitInfo read_from(Reader& r) {
        KConnectionUnitInfo v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_nKNMSN = Codec<std::remove_cvref_t<decltype(v.m_nKNMSN)>>::get(r);
        v.m_cUnitClass = Codec<std::remove_cvref_t<decltype(v.m_cUnitClass)>>::get(r);
        v.m_ucLevel = Codec<std::remove_cvref_t<decltype(v.m_ucLevel)>>::get(r);
        v.m_wstrCharName = Codec<std::remove_cvref_t<decltype(v.m_wstrCharName)>>::get(r);
        v.m_cRank = Codec<std::remove_cvref_t<decltype(v.m_cRank)>>::get(r);
        v.m_iRPoint = Codec<std::remove_cvref_t<decltype(v.m_iRPoint)>>::get(r);
        v.m_uiPVPRanking = Codec<std::remove_cvref_t<decltype(v.m_uiPVPRanking)>>::get(r);
        v.m_iWin = Codec<std::remove_cvref_t<decltype(v.m_iWin)>>::get(r);
        v.m_iLose = Codec<std::remove_cvref_t<decltype(v.m_iLose)>>::get(r);
        v.m_uiKillNum = Codec<std::remove_cvref_t<decltype(v.m_uiKillNum)>>::get(r);
        v.m_iChannelID = Codec<std::remove_cvref_t<decltype(v.m_iChannelID)>>::get(r);
        v.m_cState = Codec<std::remove_cvref_t<decltype(v.m_cState)>>::get(r);
        v.m_iStateCode = Codec<std::remove_cvref_t<decltype(v.m_iStateCode)>>::get(r);
        v.m_iPartyUID = Codec<std::remove_cvref_t<decltype(v.m_iPartyUID)>>::get(r);
        v.m_iRoomUID = Codec<std::remove_cvref_t<decltype(v.m_iRoomUID)>>::get(r);
        v.m_bPublic = Codec<std::remove_cvref_t<decltype(v.m_bPublic)>>::get(r);
        v.m_bCanIntrude = Codec<std::remove_cvref_t<decltype(v.m_bCanIntrude)>>::get(r);
        v.m_iDungeonID = Codec<std::remove_cvref_t<decltype(v.m_iDungeonID)>>::get(r);
        v.m_iRoomListID = Codec<std::remove_cvref_t<decltype(v.m_iRoomListID)>>::get(r);
        v.m_wstrGuildName = Codec<std::remove_cvref_t<decltype(v.m_wstrGuildName)>>::get(r);
        return v;
    }
};

struct KCreateGuildREQ {
    std::int64_t   m_iUnitUID{};  // UidType
    std::int64_t   m_iItemUID{};  // UidType
    std::u16string m_wstrGuildName{};  // std::wstring
    std::u16string m_wstrGuildMessage{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_wstrGuildName)>>::put(w, m_wstrGuildName);
        Codec<std::remove_cvref_t<decltype(m_wstrGuildMessage)>>::put(w, m_wstrGuildMessage);
    }

    static KCreateGuildREQ read_from(Reader& r) {
        KCreateGuildREQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_wstrGuildName = Codec<std::remove_cvref_t<decltype(v.m_wstrGuildName)>>::get(r);
        v.m_wstrGuildMessage = Codec<std::remove_cvref_t<decltype(v.m_wstrGuildMessage)>>::get(r);
        return v;
    }
};

struct KCreatePartyInfo {
    char         m_cPartyType{};
    std::int32_t m_iDungeonID{};  // int
    char         m_DifficultyLevel{};
    char         m_cGetItemType{};
    char         m_cDungeonMode{};
    bool         m_bCheckLowLevel{};
    char         m_cPVPGameType{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cPartyType)>>::put(w, m_cPartyType);
        Codec<std::remove_cvref_t<decltype(m_iDungeonID)>>::put(w, m_iDungeonID);
        Codec<std::remove_cvref_t<decltype(m_DifficultyLevel)>>::put(w, m_DifficultyLevel);
        Codec<std::remove_cvref_t<decltype(m_cGetItemType)>>::put(w, m_cGetItemType);
        Codec<std::remove_cvref_t<decltype(m_cDungeonMode)>>::put(w, m_cDungeonMode);
        Codec<std::remove_cvref_t<decltype(m_bCheckLowLevel)>>::put(w, m_bCheckLowLevel);
        Codec<std::remove_cvref_t<decltype(m_cPVPGameType)>>::put(w, m_cPVPGameType);
    }

    static KCreatePartyInfo read_from(Reader& r) {
        KCreatePartyInfo v;
        v.m_cPartyType = Codec<std::remove_cvref_t<decltype(v.m_cPartyType)>>::get(r);
        v.m_iDungeonID = Codec<std::remove_cvref_t<decltype(v.m_iDungeonID)>>::get(r);
        v.m_DifficultyLevel = Codec<std::remove_cvref_t<decltype(v.m_DifficultyLevel)>>::get(r);
        v.m_cGetItemType = Codec<std::remove_cvref_t<decltype(v.m_cGetItemType)>>::get(r);
        v.m_cDungeonMode = Codec<std::remove_cvref_t<decltype(v.m_cDungeonMode)>>::get(r);
        v.m_bCheckLowLevel = Codec<std::remove_cvref_t<decltype(v.m_bCheckLowLevel)>>::get(r);
        v.m_cPVPGameType = Codec<std::remove_cvref_t<decltype(v.m_cPVPGameType)>>::get(r);
        return v;
    }
};

struct KCumulativeTimeEventInfo {
    std::int32_t m_iEventUID{};  // int
    std::int32_t m_iCumulativeTime{};  // int
    bool         m_bAccountEvent{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iEventUID)>>::put(w, m_iEventUID);
        Codec<std::remove_cvref_t<decltype(m_iCumulativeTime)>>::put(w, m_iCumulativeTime);
        Codec<std::remove_cvref_t<decltype(m_bAccountEvent)>>::put(w, m_bAccountEvent);
    }

    static KCumulativeTimeEventInfo read_from(Reader& r) {
        KCumulativeTimeEventInfo v;
        v.m_iEventUID = Codec<std::remove_cvref_t<decltype(v.m_iEventUID)>>::get(r);
        v.m_iCumulativeTime = Codec<std::remove_cvref_t<decltype(v.m_iCumulativeTime)>>::get(r);
        v.m_bAccountEvent = Codec<std::remove_cvref_t<decltype(v.m_bAccountEvent)>>::get(r);
        return v;
    }
};

struct KDBConnectionInfo {
    std::int32_t   m_iDBType{};  // int
    std::u16string m_wstrConnectionString{};  // std::wstring
    std::int32_t   m_iThreadCount{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iDBType)>>::put(w, m_iDBType);
        Codec<std::remove_cvref_t<decltype(m_wstrConnectionString)>>::put(w, m_wstrConnectionString);
        Codec<std::remove_cvref_t<decltype(m_iThreadCount)>>::put(w, m_iThreadCount);
    }

    static KDBConnectionInfo read_from(Reader& r) {
        KDBConnectionInfo v;
        v.m_iDBType = Codec<std::remove_cvref_t<decltype(v.m_iDBType)>>::get(r);
        v.m_wstrConnectionString = Codec<std::remove_cvref_t<decltype(v.m_wstrConnectionString)>>::get(r);
        v.m_iThreadCount = Codec<std::remove_cvref_t<decltype(v.m_iThreadCount)>>::get(r);
        return v;
    }
};

struct KDBE_2013_EVENT_MISSION_COMPLETE_ACK {
    std::int32_t m_iOK{};  // int
    std::uint8_t m_ucLevel{};  // u_char

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_ucLevel)>>::put(w, m_ucLevel);
    }

    static KDBE_2013_EVENT_MISSION_COMPLETE_ACK read_from(Reader& r) {
        KDBE_2013_EVENT_MISSION_COMPLETE_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_ucLevel = Codec<std::remove_cvref_t<decltype(v.m_ucLevel)>>::get(r);
        return v;
    }
};

struct KDBE_2013_EVENT_MISSION_COMPLETE_REQ {
    std::int64_t m_iUnitUID{};  // UidType
    std::uint8_t m_ucLevel{};  // u_char

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_ucLevel)>>::put(w, m_ucLevel);
    }

    static KDBE_2013_EVENT_MISSION_COMPLETE_REQ read_from(Reader& r) {
        KDBE_2013_EVENT_MISSION_COMPLETE_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_ucLevel = Codec<std::remove_cvref_t<decltype(v.m_ucLevel)>>::get(r);
        return v;
    }
};

struct KDBE_2014_EVENT_MISSION_COMPLETE_REQ {
    std::int64_t m_iUserUID{};  // UidType
    std::int64_t m_iUnitUID{};  // UidType
    std::int32_t m_iNewYearMissionStepID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iNewYearMissionStepID)>>::put(w, m_iNewYearMissionStepID);
    }

    static KDBE_2014_EVENT_MISSION_COMPLETE_REQ read_from(Reader& r) {
        KDBE_2014_EVENT_MISSION_COMPLETE_REQ v;
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iNewYearMissionStepID = Codec<std::remove_cvref_t<decltype(v.m_iNewYearMissionStepID)>>::get(r);
        return v;
    }
};

struct KDBE_ABUSER_LIST_ACK {
    std::map<std::int32_t, KAbuserList> m_mapAbuserList{};  // std::map< int, KAbuserList >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_mapAbuserList)>>::put(w, m_mapAbuserList);
    }

    static KDBE_ABUSER_LIST_ACK read_from(Reader& r) {
        KDBE_ABUSER_LIST_ACK v;
        v.m_mapAbuserList = Codec<std::remove_cvref_t<decltype(v.m_mapAbuserList)>>::get(r);
        return v;
    }
};

struct KDBE_ABUSER_LIST_REQ {
    std::map<std::int32_t, std::int32_t> m_mapReleaseTick{};  // std::map< int, int >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_mapReleaseTick)>>::put(w, m_mapReleaseTick);
    }

    static KDBE_ABUSER_LIST_REQ read_from(Reader& r) {
        KDBE_ABUSER_LIST_REQ v;
        v.m_mapReleaseTick = Codec<std::remove_cvref_t<decltype(v.m_mapReleaseTick)>>::get(r);
        return v;
    }
};

struct KDBE_ACCEPT_FRIEND_REQ {
    std::int64_t m_iUnitUID{};  // UidType
    std::int64_t m_iFriendUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iFriendUnitUID)>>::put(w, m_iFriendUnitUID);
    }

    static KDBE_ACCEPT_FRIEND_REQ read_from(Reader& r) {
        KDBE_ACCEPT_FRIEND_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iFriendUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iFriendUnitUID)>>::get(r);
        return v;
    }
};

struct KDBE_ADMIN_AUTO_GET_ALL_SKILL_ACK {
    std::int32_t                         m_iOK{};  // int
    std::int64_t                         m_iUnitUID{};  // UidType
    std::int32_t                         m_iActiveSkillPageNumber{};  // int
    char                                 m_cUnitClass{};
    std::int32_t                         m_iSPoint{};  // int
    std::int32_t                         m_iCSPoint{};  // int
    std::map<std::int32_t, std::int32_t> m_mapGetSkillList{};  // std::map<int, int>
    std::vector<std::int16_t>            m_vecUnsealedSkillID{};  // std::vector< short >
    std::int32_t                         m_iNewDefaultSkill1{};  // int
    std::int32_t                         m_iNewDefaultSkill2{};  // int
    std::int32_t                         m_iNewDefaultSkill3{};  // int
    std::int32_t                         m_iNewDefaultSkill4{};  // int
    std::int32_t                         m_iNewDefaultSkill5{};  // int
    std::int32_t                         m_iNewDefaultSkill6{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iActiveSkillPageNumber)>>::put(w, m_iActiveSkillPageNumber);
        Codec<std::remove_cvref_t<decltype(m_cUnitClass)>>::put(w, m_cUnitClass);
        Codec<std::remove_cvref_t<decltype(m_iSPoint)>>::put(w, m_iSPoint);
        Codec<std::remove_cvref_t<decltype(m_iCSPoint)>>::put(w, m_iCSPoint);
        Codec<std::remove_cvref_t<decltype(m_mapGetSkillList)>>::put(w, m_mapGetSkillList);
        Codec<std::remove_cvref_t<decltype(m_vecUnsealedSkillID)>>::put(w, m_vecUnsealedSkillID);
        Codec<std::remove_cvref_t<decltype(m_iNewDefaultSkill1)>>::put(w, m_iNewDefaultSkill1);
        Codec<std::remove_cvref_t<decltype(m_iNewDefaultSkill2)>>::put(w, m_iNewDefaultSkill2);
        Codec<std::remove_cvref_t<decltype(m_iNewDefaultSkill3)>>::put(w, m_iNewDefaultSkill3);
        Codec<std::remove_cvref_t<decltype(m_iNewDefaultSkill4)>>::put(w, m_iNewDefaultSkill4);
        Codec<std::remove_cvref_t<decltype(m_iNewDefaultSkill5)>>::put(w, m_iNewDefaultSkill5);
        Codec<std::remove_cvref_t<decltype(m_iNewDefaultSkill6)>>::put(w, m_iNewDefaultSkill6);
    }

    static KDBE_ADMIN_AUTO_GET_ALL_SKILL_ACK read_from(Reader& r) {
        KDBE_ADMIN_AUTO_GET_ALL_SKILL_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iActiveSkillPageNumber = Codec<std::remove_cvref_t<decltype(v.m_iActiveSkillPageNumber)>>::get(r);
        v.m_cUnitClass = Codec<std::remove_cvref_t<decltype(v.m_cUnitClass)>>::get(r);
        v.m_iSPoint = Codec<std::remove_cvref_t<decltype(v.m_iSPoint)>>::get(r);
        v.m_iCSPoint = Codec<std::remove_cvref_t<decltype(v.m_iCSPoint)>>::get(r);
        v.m_mapGetSkillList = Codec<std::remove_cvref_t<decltype(v.m_mapGetSkillList)>>::get(r);
        v.m_vecUnsealedSkillID = Codec<std::remove_cvref_t<decltype(v.m_vecUnsealedSkillID)>>::get(r);
        v.m_iNewDefaultSkill1 = Codec<std::remove_cvref_t<decltype(v.m_iNewDefaultSkill1)>>::get(r);
        v.m_iNewDefaultSkill2 = Codec<std::remove_cvref_t<decltype(v.m_iNewDefaultSkill2)>>::get(r);
        v.m_iNewDefaultSkill3 = Codec<std::remove_cvref_t<decltype(v.m_iNewDefaultSkill3)>>::get(r);
        v.m_iNewDefaultSkill4 = Codec<std::remove_cvref_t<decltype(v.m_iNewDefaultSkill4)>>::get(r);
        v.m_iNewDefaultSkill5 = Codec<std::remove_cvref_t<decltype(v.m_iNewDefaultSkill5)>>::get(r);
        v.m_iNewDefaultSkill6 = Codec<std::remove_cvref_t<decltype(v.m_iNewDefaultSkill6)>>::get(r);
        return v;
    }
};

struct KDBE_ADMIN_AUTO_GET_ALL_SKILL_REQ {
    std::int64_t                         m_iUnitUID{};  // UidType
    std::int32_t                         m_iActiveSkillPageNumber{};  // int
    char                                 m_cUnitClass{};
    std::int32_t                         m_iSPoint{};  // int
    std::int32_t                         m_iCSPoint{};  // int
    std::map<std::int32_t, std::int32_t> m_mapGetSkillList{};  // std::map<int, int>
    std::vector<std::int16_t>            m_vecUnsealedSkillID{};  // std::vector< short >
    std::int32_t                         m_iNewDefaultSkill1{};  // int
    std::int32_t                         m_iNewDefaultSkill2{};  // int
    std::int32_t                         m_iNewDefaultSkill3{};  // int
    std::int32_t                         m_iNewDefaultSkill4{};  // int
    std::int32_t                         m_iNewDefaultSkill5{};  // int
    std::int32_t                         m_iNewDefaultSkill6{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iActiveSkillPageNumber)>>::put(w, m_iActiveSkillPageNumber);
        Codec<std::remove_cvref_t<decltype(m_cUnitClass)>>::put(w, m_cUnitClass);
        Codec<std::remove_cvref_t<decltype(m_iSPoint)>>::put(w, m_iSPoint);
        Codec<std::remove_cvref_t<decltype(m_iCSPoint)>>::put(w, m_iCSPoint);
        Codec<std::remove_cvref_t<decltype(m_mapGetSkillList)>>::put(w, m_mapGetSkillList);
        Codec<std::remove_cvref_t<decltype(m_vecUnsealedSkillID)>>::put(w, m_vecUnsealedSkillID);
        Codec<std::remove_cvref_t<decltype(m_iNewDefaultSkill1)>>::put(w, m_iNewDefaultSkill1);
        Codec<std::remove_cvref_t<decltype(m_iNewDefaultSkill2)>>::put(w, m_iNewDefaultSkill2);
        Codec<std::remove_cvref_t<decltype(m_iNewDefaultSkill3)>>::put(w, m_iNewDefaultSkill3);
        Codec<std::remove_cvref_t<decltype(m_iNewDefaultSkill4)>>::put(w, m_iNewDefaultSkill4);
        Codec<std::remove_cvref_t<decltype(m_iNewDefaultSkill5)>>::put(w, m_iNewDefaultSkill5);
        Codec<std::remove_cvref_t<decltype(m_iNewDefaultSkill6)>>::put(w, m_iNewDefaultSkill6);
    }

    static KDBE_ADMIN_AUTO_GET_ALL_SKILL_REQ read_from(Reader& r) {
        KDBE_ADMIN_AUTO_GET_ALL_SKILL_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iActiveSkillPageNumber = Codec<std::remove_cvref_t<decltype(v.m_iActiveSkillPageNumber)>>::get(r);
        v.m_cUnitClass = Codec<std::remove_cvref_t<decltype(v.m_cUnitClass)>>::get(r);
        v.m_iSPoint = Codec<std::remove_cvref_t<decltype(v.m_iSPoint)>>::get(r);
        v.m_iCSPoint = Codec<std::remove_cvref_t<decltype(v.m_iCSPoint)>>::get(r);
        v.m_mapGetSkillList = Codec<std::remove_cvref_t<decltype(v.m_mapGetSkillList)>>::get(r);
        v.m_vecUnsealedSkillID = Codec<std::remove_cvref_t<decltype(v.m_vecUnsealedSkillID)>>::get(r);
        v.m_iNewDefaultSkill1 = Codec<std::remove_cvref_t<decltype(v.m_iNewDefaultSkill1)>>::get(r);
        v.m_iNewDefaultSkill2 = Codec<std::remove_cvref_t<decltype(v.m_iNewDefaultSkill2)>>::get(r);
        v.m_iNewDefaultSkill3 = Codec<std::remove_cvref_t<decltype(v.m_iNewDefaultSkill3)>>::get(r);
        v.m_iNewDefaultSkill4 = Codec<std::remove_cvref_t<decltype(v.m_iNewDefaultSkill4)>>::get(r);
        v.m_iNewDefaultSkill5 = Codec<std::remove_cvref_t<decltype(v.m_iNewDefaultSkill5)>>::get(r);
        v.m_iNewDefaultSkill6 = Codec<std::remove_cvref_t<decltype(v.m_iNewDefaultSkill6)>>::get(r);
        return v;
    }
};

struct KDBE_ADMIN_CASH_SKILL_POINT_DATE_CHANGE_REQ {
    std::int64_t   m_iUnitUID{};  // UidType
    std::u16string m_wstrModifyDate{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrModifyDate)>>::put(w, m_wstrModifyDate);
    }

    static KDBE_ADMIN_CASH_SKILL_POINT_DATE_CHANGE_REQ read_from(Reader& r) {
        KDBE_ADMIN_CASH_SKILL_POINT_DATE_CHANGE_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_wstrModifyDate = Codec<std::remove_cvref_t<decltype(v.m_wstrModifyDate)>>::get(r);
        return v;
    }
};

struct KDBE_ADMIN_CHANGE_COUPLE_DATE_REQ {
    float        m_fAddRate{};
    std::int64_t m_iUnitUID{};  // UidType
    std::int64_t m_iLoverUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_fAddRate)>>::put(w, m_fAddRate);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iLoverUnitUID)>>::put(w, m_iLoverUnitUID);
    }

    static KDBE_ADMIN_CHANGE_COUPLE_DATE_REQ read_from(Reader& r) {
        KDBE_ADMIN_CHANGE_COUPLE_DATE_REQ v;
        v.m_fAddRate = Codec<std::remove_cvref_t<decltype(v.m_fAddRate)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iLoverUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iLoverUnitUID)>>::get(r);
        return v;
    }
};

struct KDBE_ADMIN_CHANGE_UNIT_CLASS_ACK {
    std::int32_t m_iOK{};  // int
    std::int64_t m_iUnitUID{};  // UidType
    char         m_cUnitClass{};
    std::int32_t m_iNewDefaultSkill1{};  // int
    std::int32_t m_iNewDefaultSkill2{};  // int
    std::int32_t m_iNewDefaultSkill3{};  // int
    std::int32_t m_iNewDefaultSkill4{};  // int
    std::int32_t m_iNewDefaultSkill5{};  // int
    std::int32_t m_iNewDefaultSkill6{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_cUnitClass)>>::put(w, m_cUnitClass);
        Codec<std::remove_cvref_t<decltype(m_iNewDefaultSkill1)>>::put(w, m_iNewDefaultSkill1);
        Codec<std::remove_cvref_t<decltype(m_iNewDefaultSkill2)>>::put(w, m_iNewDefaultSkill2);
        Codec<std::remove_cvref_t<decltype(m_iNewDefaultSkill3)>>::put(w, m_iNewDefaultSkill3);
        Codec<std::remove_cvref_t<decltype(m_iNewDefaultSkill4)>>::put(w, m_iNewDefaultSkill4);
        Codec<std::remove_cvref_t<decltype(m_iNewDefaultSkill5)>>::put(w, m_iNewDefaultSkill5);
        Codec<std::remove_cvref_t<decltype(m_iNewDefaultSkill6)>>::put(w, m_iNewDefaultSkill6);
    }

    static KDBE_ADMIN_CHANGE_UNIT_CLASS_ACK read_from(Reader& r) {
        KDBE_ADMIN_CHANGE_UNIT_CLASS_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_cUnitClass = Codec<std::remove_cvref_t<decltype(v.m_cUnitClass)>>::get(r);
        v.m_iNewDefaultSkill1 = Codec<std::remove_cvref_t<decltype(v.m_iNewDefaultSkill1)>>::get(r);
        v.m_iNewDefaultSkill2 = Codec<std::remove_cvref_t<decltype(v.m_iNewDefaultSkill2)>>::get(r);
        v.m_iNewDefaultSkill3 = Codec<std::remove_cvref_t<decltype(v.m_iNewDefaultSkill3)>>::get(r);
        v.m_iNewDefaultSkill4 = Codec<std::remove_cvref_t<decltype(v.m_iNewDefaultSkill4)>>::get(r);
        v.m_iNewDefaultSkill5 = Codec<std::remove_cvref_t<decltype(v.m_iNewDefaultSkill5)>>::get(r);
        v.m_iNewDefaultSkill6 = Codec<std::remove_cvref_t<decltype(v.m_iNewDefaultSkill6)>>::get(r);
        return v;
    }
};

struct KDBE_ADMIN_CHANGE_UNIT_CLASS_REQ {
    std::int64_t m_iUnitUID{};  // UidType
    char         m_cUnitClass{};
    std::int32_t m_iNewDefaultSkill1{};  // int
    std::int32_t m_iNewDefaultSkill2{};  // int
    std::int32_t m_iNewDefaultSkill3{};  // int
    std::int32_t m_iNewDefaultSkill4{};  // int
    std::int32_t m_iNewDefaultSkill5{};  // int
    std::int32_t m_iNewDefaultSkill6{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_cUnitClass)>>::put(w, m_cUnitClass);
        Codec<std::remove_cvref_t<decltype(m_iNewDefaultSkill1)>>::put(w, m_iNewDefaultSkill1);
        Codec<std::remove_cvref_t<decltype(m_iNewDefaultSkill2)>>::put(w, m_iNewDefaultSkill2);
        Codec<std::remove_cvref_t<decltype(m_iNewDefaultSkill3)>>::put(w, m_iNewDefaultSkill3);
        Codec<std::remove_cvref_t<decltype(m_iNewDefaultSkill4)>>::put(w, m_iNewDefaultSkill4);
        Codec<std::remove_cvref_t<decltype(m_iNewDefaultSkill5)>>::put(w, m_iNewDefaultSkill5);
        Codec<std::remove_cvref_t<decltype(m_iNewDefaultSkill6)>>::put(w, m_iNewDefaultSkill6);
    }

    static KDBE_ADMIN_CHANGE_UNIT_CLASS_REQ read_from(Reader& r) {
        KDBE_ADMIN_CHANGE_UNIT_CLASS_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_cUnitClass = Codec<std::remove_cvref_t<decltype(v.m_cUnitClass)>>::get(r);
        v.m_iNewDefaultSkill1 = Codec<std::remove_cvref_t<decltype(v.m_iNewDefaultSkill1)>>::get(r);
        v.m_iNewDefaultSkill2 = Codec<std::remove_cvref_t<decltype(v.m_iNewDefaultSkill2)>>::get(r);
        v.m_iNewDefaultSkill3 = Codec<std::remove_cvref_t<decltype(v.m_iNewDefaultSkill3)>>::get(r);
        v.m_iNewDefaultSkill4 = Codec<std::remove_cvref_t<decltype(v.m_iNewDefaultSkill4)>>::get(r);
        v.m_iNewDefaultSkill5 = Codec<std::remove_cvref_t<decltype(v.m_iNewDefaultSkill5)>>::get(r);
        v.m_iNewDefaultSkill6 = Codec<std::remove_cvref_t<decltype(v.m_iNewDefaultSkill6)>>::get(r);
        return v;
    }
};

struct KDBE_ADMIN_CHEAT_GET_ALL_SKILL_REQ {
    std::int64_t                             m_iUnitUID{};  // UidType
    std::int32_t                             m_iActiveSkillPageNumber{};  // int
    std::vector<std::int16_t>                m_vecUnSealedSkill{};  // std::vector<short>
    std::map<std::int32_t, KAdminCheatSkill> m_mapSkillInfo{};  // std::map<int, KAdminCheatSkill>

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iActiveSkillPageNumber)>>::put(w, m_iActiveSkillPageNumber);
        Codec<std::remove_cvref_t<decltype(m_vecUnSealedSkill)>>::put(w, m_vecUnSealedSkill);
        Codec<std::remove_cvref_t<decltype(m_mapSkillInfo)>>::put(w, m_mapSkillInfo);
    }

    static KDBE_ADMIN_CHEAT_GET_ALL_SKILL_REQ read_from(Reader& r) {
        KDBE_ADMIN_CHEAT_GET_ALL_SKILL_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iActiveSkillPageNumber = Codec<std::remove_cvref_t<decltype(v.m_iActiveSkillPageNumber)>>::get(r);
        v.m_vecUnSealedSkill = Codec<std::remove_cvref_t<decltype(v.m_vecUnSealedSkill)>>::get(r);
        v.m_mapSkillInfo = Codec<std::remove_cvref_t<decltype(v.m_mapSkillInfo)>>::get(r);
        return v;
    }
};

struct KDBE_ADMIN_GET_ITEM_SET_NOT {
    std::u16string m_wstrNickName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
    }

    static KDBE_ADMIN_GET_ITEM_SET_NOT read_from(Reader& r) {
        KDBE_ADMIN_GET_ITEM_SET_NOT v;
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        return v;
    }
};

struct KDBE_ADMIN_GET_SKILL_ACK {
    std::int32_t m_iOK{};  // int
    std::int32_t m_iActiveSkillPageNumber{};  // int
    std::int32_t m_iSkillID{};  // int
    std::int32_t m_iSkillLevel{};  // int
    std::int32_t m_iCSPoint{};  // int
    bool         m_bUnsealed{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iActiveSkillPageNumber)>>::put(w, m_iActiveSkillPageNumber);
        Codec<std::remove_cvref_t<decltype(m_iSkillID)>>::put(w, m_iSkillID);
        Codec<std::remove_cvref_t<decltype(m_iSkillLevel)>>::put(w, m_iSkillLevel);
        Codec<std::remove_cvref_t<decltype(m_iCSPoint)>>::put(w, m_iCSPoint);
        Codec<std::remove_cvref_t<decltype(m_bUnsealed)>>::put(w, m_bUnsealed);
    }

    static KDBE_ADMIN_GET_SKILL_ACK read_from(Reader& r) {
        KDBE_ADMIN_GET_SKILL_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iActiveSkillPageNumber = Codec<std::remove_cvref_t<decltype(v.m_iActiveSkillPageNumber)>>::get(r);
        v.m_iSkillID = Codec<std::remove_cvref_t<decltype(v.m_iSkillID)>>::get(r);
        v.m_iSkillLevel = Codec<std::remove_cvref_t<decltype(v.m_iSkillLevel)>>::get(r);
        v.m_iCSPoint = Codec<std::remove_cvref_t<decltype(v.m_iCSPoint)>>::get(r);
        v.m_bUnsealed = Codec<std::remove_cvref_t<decltype(v.m_bUnsealed)>>::get(r);
        return v;
    }
};

struct KDBE_ADMIN_GET_SKILL_REQ {
    std::int64_t m_iUnitUID{};  // UidType
    std::int32_t m_iActiveSkillPageNumber{};  // int
    std::int32_t m_iSkillID{};  // int
    std::int32_t m_iSkillLevel{};  // int
    std::int32_t m_iCSPoint{};  // int
    bool         m_bUnsealed{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iActiveSkillPageNumber)>>::put(w, m_iActiveSkillPageNumber);
        Codec<std::remove_cvref_t<decltype(m_iSkillID)>>::put(w, m_iSkillID);
        Codec<std::remove_cvref_t<decltype(m_iSkillLevel)>>::put(w, m_iSkillLevel);
        Codec<std::remove_cvref_t<decltype(m_iCSPoint)>>::put(w, m_iCSPoint);
        Codec<std::remove_cvref_t<decltype(m_bUnsealed)>>::put(w, m_bUnsealed);
    }

    static KDBE_ADMIN_GET_SKILL_REQ read_from(Reader& r) {
        KDBE_ADMIN_GET_SKILL_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iActiveSkillPageNumber = Codec<std::remove_cvref_t<decltype(v.m_iActiveSkillPageNumber)>>::get(r);
        v.m_iSkillID = Codec<std::remove_cvref_t<decltype(v.m_iSkillID)>>::get(r);
        v.m_iSkillLevel = Codec<std::remove_cvref_t<decltype(v.m_iSkillLevel)>>::get(r);
        v.m_iCSPoint = Codec<std::remove_cvref_t<decltype(v.m_iCSPoint)>>::get(r);
        v.m_bUnsealed = Codec<std::remove_cvref_t<decltype(v.m_bUnsealed)>>::get(r);
        return v;
    }
};

struct KDBE_APPLY_JOIN_GUILD_ACK {
    std::int32_t        m_iOK{};  // int
    KApplyJoinGuildInfo m_kApplyJoinGuildInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_kApplyJoinGuildInfo)>>::put(w, m_kApplyJoinGuildInfo);
    }

    static KDBE_APPLY_JOIN_GUILD_ACK read_from(Reader& r) {
        KDBE_APPLY_JOIN_GUILD_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_kApplyJoinGuildInfo = Codec<std::remove_cvref_t<decltype(v.m_kApplyJoinGuildInfo)>>::get(r);
        return v;
    }
};

struct KDBE_APPLY_JOIN_GUILD_REQ {
    KApplyJoinGuildInfo           m_kApplyJoinGuildInfo{};
    bool                          m_bOldApplyDelete{};
    std::vector<KApplyDeleteInfo> m_vecDeleteApply{};  // std::vector< KApplyDeleteInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kApplyJoinGuildInfo)>>::put(w, m_kApplyJoinGuildInfo);
        Codec<std::remove_cvref_t<decltype(m_bOldApplyDelete)>>::put(w, m_bOldApplyDelete);
        Codec<std::remove_cvref_t<decltype(m_vecDeleteApply)>>::put(w, m_vecDeleteApply);
    }

    static KDBE_APPLY_JOIN_GUILD_REQ read_from(Reader& r) {
        KDBE_APPLY_JOIN_GUILD_REQ v;
        v.m_kApplyJoinGuildInfo = Codec<std::remove_cvref_t<decltype(v.m_kApplyJoinGuildInfo)>>::get(r);
        v.m_bOldApplyDelete = Codec<std::remove_cvref_t<decltype(v.m_bOldApplyDelete)>>::get(r);
        v.m_vecDeleteApply = Codec<std::remove_cvref_t<decltype(v.m_vecDeleteApply)>>::get(r);
        return v;
    }
};

struct KDBE_BLOCK_FRIEND_REQ {
    std::int64_t m_iUnitUID{};  // UidType
    std::int64_t m_iFriendUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iFriendUnitUID)>>::put(w, m_iFriendUnitUID);
    }

    static KDBE_BLOCK_FRIEND_REQ read_from(Reader& r) {
        KDBE_BLOCK_FRIEND_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iFriendUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iFriendUnitUID)>>::get(r);
        return v;
    }
};

struct KDBE_BOSS_FIELD_LOG_NOT {
    KBossFieldLog                                          m_kFieldLog{};
    std::map<std::int64_t, std::vector<KBossFieldUserLog>> m_mapUserLog{};  // TYPE_BOSS_FIELD_USER_LOG

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kFieldLog)>>::put(w, m_kFieldLog);
        Codec<std::remove_cvref_t<decltype(m_mapUserLog)>>::put(w, m_mapUserLog);
    }

    static KDBE_BOSS_FIELD_LOG_NOT read_from(Reader& r) {
        KDBE_BOSS_FIELD_LOG_NOT v;
        v.m_kFieldLog = Codec<std::remove_cvref_t<decltype(v.m_kFieldLog)>>::get(r);
        v.m_mapUserLog = Codec<std::remove_cvref_t<decltype(v.m_mapUserLog)>>::get(r);
        return v;
    }
};

struct KDBE_BREAK_UP_ACK {
    std::int32_t m_iOK{};  // int
    std::int64_t m_iMyUnitUID{};  // UidType
    std::int64_t m_iLoverUnitUID{};  // UidType
    std::int32_t m_iCost{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iMyUnitUID)>>::put(w, m_iMyUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iLoverUnitUID)>>::put(w, m_iLoverUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iCost)>>::put(w, m_iCost);
    }

    static KDBE_BREAK_UP_ACK read_from(Reader& r) {
        KDBE_BREAK_UP_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iMyUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iMyUnitUID)>>::get(r);
        v.m_iLoverUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iLoverUnitUID)>>::get(r);
        v.m_iCost = Codec<std::remove_cvref_t<decltype(v.m_iCost)>>::get(r);
        return v;
    }
};

struct KDBE_BREAK_UP_REQ {
    std::int64_t m_iMyUnitUID{};  // UidType
    std::int64_t m_iLoverUnitUID{};  // UidType
    std::int32_t m_iCost{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iMyUnitUID)>>::put(w, m_iMyUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iLoverUnitUID)>>::put(w, m_iLoverUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iCost)>>::put(w, m_iCost);
    }

    static KDBE_BREAK_UP_REQ read_from(Reader& r) {
        KDBE_BREAK_UP_REQ v;
        v.m_iMyUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iMyUnitUID)>>::get(r);
        v.m_iLoverUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iLoverUnitUID)>>::get(r);
        v.m_iCost = Codec<std::remove_cvref_t<decltype(v.m_iCost)>>::get(r);
        return v;
    }
};

struct KDBE_BUY_AP_ITEM_LOG_NOT {
    std::int64_t   m_iUnitUID{};  // UidType
    std::int32_t   m_iItemID{};  // int
    std::int32_t   m_iQuantity{};  // int
    std::int32_t   m_iBeforAP{};  // int
    std::int32_t   m_iAfterAP{};  // int
    std::u16string m_wstrBuyDate{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iItemID)>>::put(w, m_iItemID);
        Codec<std::remove_cvref_t<decltype(m_iQuantity)>>::put(w, m_iQuantity);
        Codec<std::remove_cvref_t<decltype(m_iBeforAP)>>::put(w, m_iBeforAP);
        Codec<std::remove_cvref_t<decltype(m_iAfterAP)>>::put(w, m_iAfterAP);
        Codec<std::remove_cvref_t<decltype(m_wstrBuyDate)>>::put(w, m_wstrBuyDate);
    }

    static KDBE_BUY_AP_ITEM_LOG_NOT read_from(Reader& r) {
        KDBE_BUY_AP_ITEM_LOG_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iItemID = Codec<std::remove_cvref_t<decltype(v.m_iItemID)>>::get(r);
        v.m_iQuantity = Codec<std::remove_cvref_t<decltype(v.m_iQuantity)>>::get(r);
        v.m_iBeforAP = Codec<std::remove_cvref_t<decltype(v.m_iBeforAP)>>::get(r);
        v.m_iAfterAP = Codec<std::remove_cvref_t<decltype(v.m_iAfterAP)>>::get(r);
        v.m_wstrBuyDate = Codec<std::remove_cvref_t<decltype(v.m_wstrBuyDate)>>::get(r);
        return v;
    }
};

struct KDBE_BUY_UNIT_CLASS_CHANGE_REQ {
    std::int32_t                         m_iOK{};  // int
    std::int64_t                         m_iUnitUID{};  // UidType
    std::int32_t                         m_iNewUnitClass{};  // int
    std::int32_t                         m_iSPoint{};  // int
    std::int32_t                         m_iCSPoint{};  // int
    std::int32_t                         m_iTheNumberOfSkillPagesAvailable{};  // int
    std::int32_t                         m_iDefaultSkillID1{};  // int
    std::int32_t                         m_iDefaultSkillID2{};  // int
    std::int32_t                         m_iDefaultSkillID3{};  // int
    std::int32_t                         m_iDefaultSkillID4{};  // int
    std::int32_t                         m_iDefaultSkillID5{};  // int
    std::int32_t                         m_iDefaultSkillID6{};  // int
    std::map<std::int32_t, std::int32_t> m_mapChangeSkill{};  // std::map< int, int >
    std::map<std::int32_t, std::int32_t> m_mapChangeMemo{};  // std::map< int, int >
    std::map<std::int64_t, std::int32_t> m_mapChangeItem{};  // std::map< UidType, int >
    std::map<std::int32_t, std::int32_t> m_mapChangeCompleteQuest{};  // std::map< int, int >
    std::map<std::int32_t, std::int32_t> m_mapChangeInProgressQuest{};  // std::map< int, int >
    std::vector<std::int32_t>            m_vecDeleteCompleteQuest{};  // std::vector< int >
    std::vector<std::int32_t>            m_vecDeleteInProgressQuest{};  // std::vector< int >
    bool                                 m_bUnlimitedSecondJobItem{};
    std::int32_t                         m_iOldUnitClass{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iNewUnitClass)>>::put(w, m_iNewUnitClass);
        Codec<std::remove_cvref_t<decltype(m_iSPoint)>>::put(w, m_iSPoint);
        Codec<std::remove_cvref_t<decltype(m_iCSPoint)>>::put(w, m_iCSPoint);
        Codec<std::remove_cvref_t<decltype(m_iTheNumberOfSkillPagesAvailable)>>::put(w, m_iTheNumberOfSkillPagesAvailable);
        Codec<std::remove_cvref_t<decltype(m_iDefaultSkillID1)>>::put(w, m_iDefaultSkillID1);
        Codec<std::remove_cvref_t<decltype(m_iDefaultSkillID2)>>::put(w, m_iDefaultSkillID2);
        Codec<std::remove_cvref_t<decltype(m_iDefaultSkillID3)>>::put(w, m_iDefaultSkillID3);
        Codec<std::remove_cvref_t<decltype(m_iDefaultSkillID4)>>::put(w, m_iDefaultSkillID4);
        Codec<std::remove_cvref_t<decltype(m_iDefaultSkillID5)>>::put(w, m_iDefaultSkillID5);
        Codec<std::remove_cvref_t<decltype(m_iDefaultSkillID6)>>::put(w, m_iDefaultSkillID6);
        Codec<std::remove_cvref_t<decltype(m_mapChangeSkill)>>::put(w, m_mapChangeSkill);
        Codec<std::remove_cvref_t<decltype(m_mapChangeMemo)>>::put(w, m_mapChangeMemo);
        Codec<std::remove_cvref_t<decltype(m_mapChangeItem)>>::put(w, m_mapChangeItem);
        Codec<std::remove_cvref_t<decltype(m_mapChangeCompleteQuest)>>::put(w, m_mapChangeCompleteQuest);
        Codec<std::remove_cvref_t<decltype(m_mapChangeInProgressQuest)>>::put(w, m_mapChangeInProgressQuest);
        Codec<std::remove_cvref_t<decltype(m_vecDeleteCompleteQuest)>>::put(w, m_vecDeleteCompleteQuest);
        Codec<std::remove_cvref_t<decltype(m_vecDeleteInProgressQuest)>>::put(w, m_vecDeleteInProgressQuest);
        Codec<std::remove_cvref_t<decltype(m_bUnlimitedSecondJobItem)>>::put(w, m_bUnlimitedSecondJobItem);
        Codec<std::remove_cvref_t<decltype(m_iOldUnitClass)>>::put(w, m_iOldUnitClass);
    }

    static KDBE_BUY_UNIT_CLASS_CHANGE_REQ read_from(Reader& r) {
        KDBE_BUY_UNIT_CLASS_CHANGE_REQ v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iNewUnitClass = Codec<std::remove_cvref_t<decltype(v.m_iNewUnitClass)>>::get(r);
        v.m_iSPoint = Codec<std::remove_cvref_t<decltype(v.m_iSPoint)>>::get(r);
        v.m_iCSPoint = Codec<std::remove_cvref_t<decltype(v.m_iCSPoint)>>::get(r);
        v.m_iTheNumberOfSkillPagesAvailable = Codec<std::remove_cvref_t<decltype(v.m_iTheNumberOfSkillPagesAvailable)>>::get(r);
        v.m_iDefaultSkillID1 = Codec<std::remove_cvref_t<decltype(v.m_iDefaultSkillID1)>>::get(r);
        v.m_iDefaultSkillID2 = Codec<std::remove_cvref_t<decltype(v.m_iDefaultSkillID2)>>::get(r);
        v.m_iDefaultSkillID3 = Codec<std::remove_cvref_t<decltype(v.m_iDefaultSkillID3)>>::get(r);
        v.m_iDefaultSkillID4 = Codec<std::remove_cvref_t<decltype(v.m_iDefaultSkillID4)>>::get(r);
        v.m_iDefaultSkillID5 = Codec<std::remove_cvref_t<decltype(v.m_iDefaultSkillID5)>>::get(r);
        v.m_iDefaultSkillID6 = Codec<std::remove_cvref_t<decltype(v.m_iDefaultSkillID6)>>::get(r);
        v.m_mapChangeSkill = Codec<std::remove_cvref_t<decltype(v.m_mapChangeSkill)>>::get(r);
        v.m_mapChangeMemo = Codec<std::remove_cvref_t<decltype(v.m_mapChangeMemo)>>::get(r);
        v.m_mapChangeItem = Codec<std::remove_cvref_t<decltype(v.m_mapChangeItem)>>::get(r);
        v.m_mapChangeCompleteQuest = Codec<std::remove_cvref_t<decltype(v.m_mapChangeCompleteQuest)>>::get(r);
        v.m_mapChangeInProgressQuest = Codec<std::remove_cvref_t<decltype(v.m_mapChangeInProgressQuest)>>::get(r);
        v.m_vecDeleteCompleteQuest = Codec<std::remove_cvref_t<decltype(v.m_vecDeleteCompleteQuest)>>::get(r);
        v.m_vecDeleteInProgressQuest = Codec<std::remove_cvref_t<decltype(v.m_vecDeleteInProgressQuest)>>::get(r);
        v.m_bUnlimitedSecondJobItem = Codec<std::remove_cvref_t<decltype(v.m_bUnlimitedSecondJobItem)>>::get(r);
        v.m_iOldUnitClass = Codec<std::remove_cvref_t<decltype(v.m_iOldUnitClass)>>::get(r);
        return v;
    }
};

struct KDBE_CHANGE_GUILD_MEMBER_GRADE_ACK {
    std::int32_t   m_iOK{};  // int
    std::int32_t   m_iGuildUID{};  // int
    std::int64_t   m_iTargetUnitUID{};  // UidType
    std::u16string m_wstrTargetNickName{};  // std::wstring
    std::uint8_t   m_ucMemberShipGrade{};  // u_char
    bool           m_bChangeGuildMaster{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iGuildUID)>>::put(w, m_iGuildUID);
        Codec<std::remove_cvref_t<decltype(m_iTargetUnitUID)>>::put(w, m_iTargetUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrTargetNickName)>>::put(w, m_wstrTargetNickName);
        Codec<std::remove_cvref_t<decltype(m_ucMemberShipGrade)>>::put(w, m_ucMemberShipGrade);
        Codec<std::remove_cvref_t<decltype(m_bChangeGuildMaster)>>::put(w, m_bChangeGuildMaster);
    }

    static KDBE_CHANGE_GUILD_MEMBER_GRADE_ACK read_from(Reader& r) {
        KDBE_CHANGE_GUILD_MEMBER_GRADE_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iGuildUID = Codec<std::remove_cvref_t<decltype(v.m_iGuildUID)>>::get(r);
        v.m_iTargetUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iTargetUnitUID)>>::get(r);
        v.m_wstrTargetNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrTargetNickName)>>::get(r);
        v.m_ucMemberShipGrade = Codec<std::remove_cvref_t<decltype(v.m_ucMemberShipGrade)>>::get(r);
        v.m_bChangeGuildMaster = Codec<std::remove_cvref_t<decltype(v.m_bChangeGuildMaster)>>::get(r);
        return v;
    }
};

struct KDBE_CHANGE_GUILD_MEMBER_GRADE_REQ {
    std::int64_t   m_iUnitUID{};  // UidType
    std::int32_t   m_iGuildUID{};  // int
    std::int64_t   m_iTargetUnitUID{};  // UidType
    std::u16string m_wstrTargetNickName{};  // std::wstring
    std::uint8_t   m_ucMemberShipGrade{};  // u_char
    bool           m_bChangeGuildMaster{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iGuildUID)>>::put(w, m_iGuildUID);
        Codec<std::remove_cvref_t<decltype(m_iTargetUnitUID)>>::put(w, m_iTargetUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrTargetNickName)>>::put(w, m_wstrTargetNickName);
        Codec<std::remove_cvref_t<decltype(m_ucMemberShipGrade)>>::put(w, m_ucMemberShipGrade);
        Codec<std::remove_cvref_t<decltype(m_bChangeGuildMaster)>>::put(w, m_bChangeGuildMaster);
    }

    static KDBE_CHANGE_GUILD_MEMBER_GRADE_REQ read_from(Reader& r) {
        KDBE_CHANGE_GUILD_MEMBER_GRADE_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iGuildUID = Codec<std::remove_cvref_t<decltype(v.m_iGuildUID)>>::get(r);
        v.m_iTargetUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iTargetUnitUID)>>::get(r);
        v.m_wstrTargetNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrTargetNickName)>>::get(r);
        v.m_ucMemberShipGrade = Codec<std::remove_cvref_t<decltype(v.m_ucMemberShipGrade)>>::get(r);
        v.m_bChangeGuildMaster = Codec<std::remove_cvref_t<decltype(v.m_bChangeGuildMaster)>>::get(r);
        return v;
    }
};

struct KDBE_CHANGE_GUILD_MEMBER_MESSAGE_ACK {
    std::int32_t   m_iOK{};  // int
    std::int64_t   m_iUnitUID{};  // UidType
    std::int64_t   m_iTargetUnitUID{};  // UidType
    std::int32_t   m_iGuildUID{};  // int
    std::u16string m_wstrMessage{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iTargetUnitUID)>>::put(w, m_iTargetUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iGuildUID)>>::put(w, m_iGuildUID);
        Codec<std::remove_cvref_t<decltype(m_wstrMessage)>>::put(w, m_wstrMessage);
    }

    static KDBE_CHANGE_GUILD_MEMBER_MESSAGE_ACK read_from(Reader& r) {
        KDBE_CHANGE_GUILD_MEMBER_MESSAGE_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iTargetUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iTargetUnitUID)>>::get(r);
        v.m_iGuildUID = Codec<std::remove_cvref_t<decltype(v.m_iGuildUID)>>::get(r);
        v.m_wstrMessage = Codec<std::remove_cvref_t<decltype(v.m_wstrMessage)>>::get(r);
        return v;
    }
};

struct KDBE_CHANGE_GUILD_MEMBER_MESSAGE_REQ {
    std::int64_t   m_iUnitUID{};  // UidType
    std::int64_t   m_iTargetUnitUID{};  // UidType
    std::int32_t   m_iGuildUID{};  // int
    std::u16string m_wstrMessage{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iTargetUnitUID)>>::put(w, m_iTargetUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iGuildUID)>>::put(w, m_iGuildUID);
        Codec<std::remove_cvref_t<decltype(m_wstrMessage)>>::put(w, m_wstrMessage);
    }

    static KDBE_CHANGE_GUILD_MEMBER_MESSAGE_REQ read_from(Reader& r) {
        KDBE_CHANGE_GUILD_MEMBER_MESSAGE_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iTargetUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iTargetUnitUID)>>::get(r);
        v.m_iGuildUID = Codec<std::remove_cvref_t<decltype(v.m_iGuildUID)>>::get(r);
        v.m_wstrMessage = Codec<std::remove_cvref_t<decltype(v.m_wstrMessage)>>::get(r);
        return v;
    }
};

struct KDBE_CHANGE_GUILD_MESSAGE_ACK {
    std::int32_t   m_iOK{};  // int
    std::int32_t   m_iGuildUID{};  // int
    std::u16string m_wstrMessage{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iGuildUID)>>::put(w, m_iGuildUID);
        Codec<std::remove_cvref_t<decltype(m_wstrMessage)>>::put(w, m_wstrMessage);
    }

    static KDBE_CHANGE_GUILD_MESSAGE_ACK read_from(Reader& r) {
        KDBE_CHANGE_GUILD_MESSAGE_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iGuildUID = Codec<std::remove_cvref_t<decltype(v.m_iGuildUID)>>::get(r);
        v.m_wstrMessage = Codec<std::remove_cvref_t<decltype(v.m_wstrMessage)>>::get(r);
        return v;
    }
};

struct KDBE_CHANGE_GUILD_MESSAGE_REQ {
    std::int64_t   m_iUnitUID{};  // UidType
    std::int32_t   m_iGuildUID{};  // int
    std::u16string m_wstrMessage{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iGuildUID)>>::put(w, m_iGuildUID);
        Codec<std::remove_cvref_t<decltype(m_wstrMessage)>>::put(w, m_wstrMessage);
    }

    static KDBE_CHANGE_GUILD_MESSAGE_REQ read_from(Reader& r) {
        KDBE_CHANGE_GUILD_MESSAGE_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iGuildUID = Codec<std::remove_cvref_t<decltype(v.m_iGuildUID)>>::get(r);
        v.m_wstrMessage = Codec<std::remove_cvref_t<decltype(v.m_wstrMessage)>>::get(r);
        return v;
    }
};

struct KDBE_CHANGE_GUILD_NAME_CHECK_ACK {
    std::int32_t m_iOK{};  // int
    std::int64_t m_iUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
    }

    static KDBE_CHANGE_GUILD_NAME_CHECK_ACK read_from(Reader& r) {
        KDBE_CHANGE_GUILD_NAME_CHECK_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        return v;
    }
};

struct KDBE_CHANGE_PET_ID_ACK {
    std::int32_t m_iOK{};  // int
    std::int64_t m_iPetUID{};  // UidType
    std::int32_t m_iBeforePetID{};  // int
    std::int32_t m_iAfterPetID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iPetUID)>>::put(w, m_iPetUID);
        Codec<std::remove_cvref_t<decltype(m_iBeforePetID)>>::put(w, m_iBeforePetID);
        Codec<std::remove_cvref_t<decltype(m_iAfterPetID)>>::put(w, m_iAfterPetID);
    }

    static KDBE_CHANGE_PET_ID_ACK read_from(Reader& r) {
        KDBE_CHANGE_PET_ID_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iPetUID = Codec<std::remove_cvref_t<decltype(v.m_iPetUID)>>::get(r);
        v.m_iBeforePetID = Codec<std::remove_cvref_t<decltype(v.m_iBeforePetID)>>::get(r);
        v.m_iAfterPetID = Codec<std::remove_cvref_t<decltype(v.m_iAfterPetID)>>::get(r);
        return v;
    }
};

struct KDBE_CHANGE_PET_ID_LOG_NOT {
    std::int64_t m_iUnitUID{};  // UidType
    std::int64_t m_iPetUID{};  // UidType
    std::int32_t m_iBeforePetID{};  // int
    std::int32_t m_iAfterPetID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iPetUID)>>::put(w, m_iPetUID);
        Codec<std::remove_cvref_t<decltype(m_iBeforePetID)>>::put(w, m_iBeforePetID);
        Codec<std::remove_cvref_t<decltype(m_iAfterPetID)>>::put(w, m_iAfterPetID);
    }

    static KDBE_CHANGE_PET_ID_LOG_NOT read_from(Reader& r) {
        KDBE_CHANGE_PET_ID_LOG_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iPetUID = Codec<std::remove_cvref_t<decltype(v.m_iPetUID)>>::get(r);
        v.m_iBeforePetID = Codec<std::remove_cvref_t<decltype(v.m_iBeforePetID)>>::get(r);
        v.m_iAfterPetID = Codec<std::remove_cvref_t<decltype(v.m_iAfterPetID)>>::get(r);
        return v;
    }
};

struct KDBE_CHANGE_PET_ID_REQ {
    std::int64_t m_iPetUID{};  // UidType
    std::int32_t m_iBeforePetID{};  // int
    std::int32_t m_iAfterPetID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iPetUID)>>::put(w, m_iPetUID);
        Codec<std::remove_cvref_t<decltype(m_iBeforePetID)>>::put(w, m_iBeforePetID);
        Codec<std::remove_cvref_t<decltype(m_iAfterPetID)>>::put(w, m_iAfterPetID);
    }

    static KDBE_CHANGE_PET_ID_REQ read_from(Reader& r) {
        KDBE_CHANGE_PET_ID_REQ v;
        v.m_iPetUID = Codec<std::remove_cvref_t<decltype(v.m_iPetUID)>>::get(r);
        v.m_iBeforePetID = Codec<std::remove_cvref_t<decltype(v.m_iBeforePetID)>>::get(r);
        v.m_iAfterPetID = Codec<std::remove_cvref_t<decltype(v.m_iAfterPetID)>>::get(r);
        return v;
    }
};

struct KDBE_CHANGE_SECOND_SECURITY_PW_REQ {
    std::int64_t   m_iUserID{};  // UidType
    std::u16string m_wstrOldSecondPW{};  // std::wstring
    std::u16string m_wstrNewSecondPW{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUserID)>>::put(w, m_iUserID);
        Codec<std::remove_cvref_t<decltype(m_wstrOldSecondPW)>>::put(w, m_wstrOldSecondPW);
        Codec<std::remove_cvref_t<decltype(m_wstrNewSecondPW)>>::put(w, m_wstrNewSecondPW);
    }

    static KDBE_CHANGE_SECOND_SECURITY_PW_REQ read_from(Reader& r) {
        KDBE_CHANGE_SECOND_SECURITY_PW_REQ v;
        v.m_iUserID = Codec<std::remove_cvref_t<decltype(v.m_iUserID)>>::get(r);
        v.m_wstrOldSecondPW = Codec<std::remove_cvref_t<decltype(v.m_wstrOldSecondPW)>>::get(r);
        v.m_wstrNewSecondPW = Codec<std::remove_cvref_t<decltype(v.m_wstrNewSecondPW)>>::get(r);
        return v;
    }
};

struct KDBE_CHANGE_UNIT_CLASS_ACK {
    std::int32_t  m_iOK{};  // int
    std::uint16_t m_usEventID{};  // u_short
    char          m_cUnitClass{};
    std::int32_t  m_iNewDefaultSkill1{};  // int
    std::int32_t  m_iNewDefaultSkill2{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_usEventID)>>::put(w, m_usEventID);
        Codec<std::remove_cvref_t<decltype(m_cUnitClass)>>::put(w, m_cUnitClass);
        Codec<std::remove_cvref_t<decltype(m_iNewDefaultSkill1)>>::put(w, m_iNewDefaultSkill1);
        Codec<std::remove_cvref_t<decltype(m_iNewDefaultSkill2)>>::put(w, m_iNewDefaultSkill2);
    }

    static KDBE_CHANGE_UNIT_CLASS_ACK read_from(Reader& r) {
        KDBE_CHANGE_UNIT_CLASS_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_usEventID = Codec<std::remove_cvref_t<decltype(v.m_usEventID)>>::get(r);
        v.m_cUnitClass = Codec<std::remove_cvref_t<decltype(v.m_cUnitClass)>>::get(r);
        v.m_iNewDefaultSkill1 = Codec<std::remove_cvref_t<decltype(v.m_iNewDefaultSkill1)>>::get(r);
        v.m_iNewDefaultSkill2 = Codec<std::remove_cvref_t<decltype(v.m_iNewDefaultSkill2)>>::get(r);
        return v;
    }
};

struct KDBE_CHANGE_UNIT_CLASS_REQ {
    std::int64_t  m_iUnitUID{};  // UidType
    std::uint16_t m_usEventID{};  // u_short
    char          m_cUnitClass{};
    std::int32_t  m_iNewDefaultSkill1{};  // int
    std::int32_t  m_iNewDefaultSkill2{};  // int
    std::int32_t  m_iTheNumberOfSkillPagesAvailable{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_usEventID)>>::put(w, m_usEventID);
        Codec<std::remove_cvref_t<decltype(m_cUnitClass)>>::put(w, m_cUnitClass);
        Codec<std::remove_cvref_t<decltype(m_iNewDefaultSkill1)>>::put(w, m_iNewDefaultSkill1);
        Codec<std::remove_cvref_t<decltype(m_iNewDefaultSkill2)>>::put(w, m_iNewDefaultSkill2);
        Codec<std::remove_cvref_t<decltype(m_iTheNumberOfSkillPagesAvailable)>>::put(w, m_iTheNumberOfSkillPagesAvailable);
    }

    static KDBE_CHANGE_UNIT_CLASS_REQ read_from(Reader& r) {
        KDBE_CHANGE_UNIT_CLASS_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_usEventID = Codec<std::remove_cvref_t<decltype(v.m_usEventID)>>::get(r);
        v.m_cUnitClass = Codec<std::remove_cvref_t<decltype(v.m_cUnitClass)>>::get(r);
        v.m_iNewDefaultSkill1 = Codec<std::remove_cvref_t<decltype(v.m_iNewDefaultSkill1)>>::get(r);
        v.m_iNewDefaultSkill2 = Codec<std::remove_cvref_t<decltype(v.m_iNewDefaultSkill2)>>::get(r);
        v.m_iTheNumberOfSkillPagesAvailable = Codec<std::remove_cvref_t<decltype(v.m_iTheNumberOfSkillPagesAvailable)>>::get(r);
        return v;
    }
};

struct KDBE_CHANNEL_LIST_ACK {
    std::map<std::int32_t, KChannelInfo>      m_mapChannelList{};  // std::map< int, KChannelInfo >
    std::map<std::int32_t, KChannelBonusInfo> m_mapChannelBonusList{};  // std::map< int, KChannelBonusInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_mapChannelList)>>::put(w, m_mapChannelList);
        Codec<std::remove_cvref_t<decltype(m_mapChannelBonusList)>>::put(w, m_mapChannelBonusList);
    }

    static KDBE_CHANNEL_LIST_ACK read_from(Reader& r) {
        KDBE_CHANNEL_LIST_ACK v;
        v.m_mapChannelList = Codec<std::remove_cvref_t<decltype(v.m_mapChannelList)>>::get(r);
        v.m_mapChannelBonusList = Codec<std::remove_cvref_t<decltype(v.m_mapChannelBonusList)>>::get(r);
        return v;
    }
};

struct KDBE_CHANNEL_LIST_REQ {
    std::int32_t m_iServerGroupID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iServerGroupID)>>::put(w, m_iServerGroupID);
    }

    static KDBE_CHANNEL_LIST_REQ read_from(Reader& r) {
        KDBE_CHANNEL_LIST_REQ v;
        v.m_iServerGroupID = Codec<std::remove_cvref_t<decltype(v.m_iServerGroupID)>>::get(r);
        return v;
    }
};

struct KDBE_CHARACTER_LEVELUP_NOT {
    std::int64_t m_nCharUID{};  // UidType
    std::uint8_t m_ucLevelBefore{};  // unsigned char
    std::uint8_t m_ucLevelAfter{};  // unsigned char

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_nCharUID)>>::put(w, m_nCharUID);
        Codec<std::remove_cvref_t<decltype(m_ucLevelBefore)>>::put(w, m_ucLevelBefore);
        Codec<std::remove_cvref_t<decltype(m_ucLevelAfter)>>::put(w, m_ucLevelAfter);
    }

    static KDBE_CHARACTER_LEVELUP_NOT read_from(Reader& r) {
        KDBE_CHARACTER_LEVELUP_NOT v;
        v.m_nCharUID = Codec<std::remove_cvref_t<decltype(v.m_nCharUID)>>::get(r);
        v.m_ucLevelBefore = Codec<std::remove_cvref_t<decltype(v.m_ucLevelBefore)>>::get(r);
        v.m_ucLevelAfter = Codec<std::remove_cvref_t<decltype(v.m_ucLevelAfter)>>::get(r);
        return v;
    }
};

struct KDBE_CHAT_OPTION_INFO_WRITE_REQ {
    std::int64_t m_iUserUID{};  // UidType
    std::uint8_t m_cIndex{};  // byte
    bool         m_bValue{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
        Codec<std::remove_cvref_t<decltype(m_cIndex)>>::put(w, m_cIndex);
        Codec<std::remove_cvref_t<decltype(m_bValue)>>::put(w, m_bValue);
    }

    static KDBE_CHAT_OPTION_INFO_WRITE_REQ read_from(Reader& r) {
        KDBE_CHAT_OPTION_INFO_WRITE_REQ v;
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        v.m_cIndex = Codec<std::remove_cvref_t<decltype(v.m_cIndex)>>::get(r);
        v.m_bValue = Codec<std::remove_cvref_t<decltype(v.m_bValue)>>::get(r);
        return v;
    }
};

struct KDBE_CHECK_ACCOUNT_BLOCK_LIST_ACK {
    std::map<std::int32_t, std::int32_t>           m_mapReleaseTick{};  // std::map< int, int >
    std::map<std::int32_t, std::set<std::int64_t>> m_mapAccountBlockList{};  // std::map< int, std::set< UidType > >
    std::vector<std::string>                       m_vecMachineIDBlockList{};  // std::vector< std::string >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_mapReleaseTick)>>::put(w, m_mapReleaseTick);
        Codec<std::remove_cvref_t<decltype(m_mapAccountBlockList)>>::put(w, m_mapAccountBlockList);
        Codec<std::remove_cvref_t<decltype(m_vecMachineIDBlockList)>>::put(w, m_vecMachineIDBlockList);
    }

    static KDBE_CHECK_ACCOUNT_BLOCK_LIST_ACK read_from(Reader& r) {
        KDBE_CHECK_ACCOUNT_BLOCK_LIST_ACK v;
        v.m_mapReleaseTick = Codec<std::remove_cvref_t<decltype(v.m_mapReleaseTick)>>::get(r);
        v.m_mapAccountBlockList = Codec<std::remove_cvref_t<decltype(v.m_mapAccountBlockList)>>::get(r);
        v.m_vecMachineIDBlockList = Codec<std::remove_cvref_t<decltype(v.m_vecMachineIDBlockList)>>::get(r);
        return v;
    }
};

struct KDBE_CHECK_ACCOUNT_BLOCK_LIST_REQ {
    std::map<std::int32_t, std::int32_t> m_mapReleaseTick{};  // std::map< int, int >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_mapReleaseTick)>>::put(w, m_mapReleaseTick);
    }

    static KDBE_CHECK_ACCOUNT_BLOCK_LIST_REQ read_from(Reader& r) {
        KDBE_CHECK_ACCOUNT_BLOCK_LIST_REQ v;
        v.m_mapReleaseTick = Codec<std::remove_cvref_t<decltype(v.m_mapReleaseTick)>>::get(r);
        return v;
    }
};

struct KDBE_CHECK_BLOCK_LIST_ACK {
    std::int32_t                                    m_iReleaseTick{};  // int
    std::map<std::int32_t, std::vector<KBlockInfo>> m_mapBlockInfo{};  // std::map< int, std::vector< KBlockInfo > >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iReleaseTick)>>::put(w, m_iReleaseTick);
        Codec<std::remove_cvref_t<decltype(m_mapBlockInfo)>>::put(w, m_mapBlockInfo);
    }

    static KDBE_CHECK_BLOCK_LIST_ACK read_from(Reader& r) {
        KDBE_CHECK_BLOCK_LIST_ACK v;
        v.m_iReleaseTick = Codec<std::remove_cvref_t<decltype(v.m_iReleaseTick)>>::get(r);
        v.m_mapBlockInfo = Codec<std::remove_cvref_t<decltype(v.m_mapBlockInfo)>>::get(r);
        return v;
    }
};

struct KDBE_CHECK_BLOCK_LIST_REQ {
    std::int32_t m_iReleaseTick{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iReleaseTick)>>::put(w, m_iReleaseTick);
    }

    static KDBE_CHECK_BLOCK_LIST_REQ read_from(Reader& r) {
        KDBE_CHECK_BLOCK_LIST_REQ v;
        v.m_iReleaseTick = Codec<std::remove_cvref_t<decltype(v.m_iReleaseTick)>>::get(r);
        return v;
    }
};

struct KDBE_CHECK_EVENT_UPDATE_ACK {
    std::map<std::int32_t, std::int32_t> m_mapReleaseTick{};  // std::map< int, int >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_mapReleaseTick)>>::put(w, m_mapReleaseTick);
    }

    static KDBE_CHECK_EVENT_UPDATE_ACK read_from(Reader& r) {
        KDBE_CHECK_EVENT_UPDATE_ACK v;
        v.m_mapReleaseTick = Codec<std::remove_cvref_t<decltype(v.m_mapReleaseTick)>>::get(r);
        return v;
    }
};

struct KDBE_CHECK_HACKING_TOOL_LIST_REQ {
    std::int32_t m_iReleaseTick{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iReleaseTick)>>::put(w, m_iReleaseTick);
    }

    static KDBE_CHECK_HACKING_TOOL_LIST_REQ read_from(Reader& r) {
        KDBE_CHECK_HACKING_TOOL_LIST_REQ v;
        v.m_iReleaseTick = Codec<std::remove_cvref_t<decltype(v.m_iReleaseTick)>>::get(r);
        return v;
    }
};

struct KDBE_CHECK_REWARD_FROM_RELAY_DB_JP_EVENT_REQ {
    std::int64_t m_iUserUID{};  // UidType
    std::int64_t m_iUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
    }

    static KDBE_CHECK_REWARD_FROM_RELAY_DB_JP_EVENT_REQ read_from(Reader& r) {
        KDBE_CHECK_REWARD_FROM_RELAY_DB_JP_EVENT_REQ v;
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        return v;
    }
};

struct KDBE_CHECK_TIME_DROP_RESULT_ACK {
    std::int32_t   m_iOK{};  // int
    std::int64_t   m_iGSUID{};  // UidType
    std::int64_t   m_iUserUID{};  // UidType
    std::int64_t   m_iUnitUID{};  // UidType
    std::u16string m_wstrNickName{};  // std::wstring
    std::int32_t   m_iTimeDropID{};  // int
    std::int32_t   m_iRewardItemID{};  // int
    std::u16string m_wstrRewardItemName{};  // std::wstring
    std::int32_t   m_iDropItemUID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iGSUID)>>::put(w, m_iGSUID);
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_iTimeDropID)>>::put(w, m_iTimeDropID);
        Codec<std::remove_cvref_t<decltype(m_iRewardItemID)>>::put(w, m_iRewardItemID);
        Codec<std::remove_cvref_t<decltype(m_wstrRewardItemName)>>::put(w, m_wstrRewardItemName);
        Codec<std::remove_cvref_t<decltype(m_iDropItemUID)>>::put(w, m_iDropItemUID);
    }

    static KDBE_CHECK_TIME_DROP_RESULT_ACK read_from(Reader& r) {
        KDBE_CHECK_TIME_DROP_RESULT_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iGSUID = Codec<std::remove_cvref_t<decltype(v.m_iGSUID)>>::get(r);
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_iTimeDropID = Codec<std::remove_cvref_t<decltype(v.m_iTimeDropID)>>::get(r);
        v.m_iRewardItemID = Codec<std::remove_cvref_t<decltype(v.m_iRewardItemID)>>::get(r);
        v.m_wstrRewardItemName = Codec<std::remove_cvref_t<decltype(v.m_wstrRewardItemName)>>::get(r);
        v.m_iDropItemUID = Codec<std::remove_cvref_t<decltype(v.m_iDropItemUID)>>::get(r);
        return v;
    }
};

struct KDBE_CHECK_TIME_DROP_RESULT_REQ {
    std::int64_t   m_iGSUID{};  // UidType
    std::int64_t   m_iUserUID{};  // UidType
    std::int64_t   m_iUnitUID{};  // UidType
    std::u16string m_wstrNickName{};  // std::wstring
    std::int32_t   m_iTimeDropID{};  // int
    std::int32_t   m_iRewardItemID{};  // int
    std::u16string m_wstrRewardItemName{};  // std::wstring
    std::int32_t   m_iDropItemUID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iGSUID)>>::put(w, m_iGSUID);
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_iTimeDropID)>>::put(w, m_iTimeDropID);
        Codec<std::remove_cvref_t<decltype(m_iRewardItemID)>>::put(w, m_iRewardItemID);
        Codec<std::remove_cvref_t<decltype(m_wstrRewardItemName)>>::put(w, m_wstrRewardItemName);
        Codec<std::remove_cvref_t<decltype(m_iDropItemUID)>>::put(w, m_iDropItemUID);
    }

    static KDBE_CHECK_TIME_DROP_RESULT_REQ read_from(Reader& r) {
        KDBE_CHECK_TIME_DROP_RESULT_REQ v;
        v.m_iGSUID = Codec<std::remove_cvref_t<decltype(v.m_iGSUID)>>::get(r);
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_iTimeDropID = Codec<std::remove_cvref_t<decltype(v.m_iTimeDropID)>>::get(r);
        v.m_iRewardItemID = Codec<std::remove_cvref_t<decltype(v.m_iRewardItemID)>>::get(r);
        v.m_wstrRewardItemName = Codec<std::remove_cvref_t<decltype(v.m_wstrRewardItemName)>>::get(r);
        v.m_iDropItemUID = Codec<std::remove_cvref_t<decltype(v.m_iDropItemUID)>>::get(r);
        return v;
    }
};

struct KDBE_CH_USER_GENERAL_LOGIN_ACK {
    std::int32_t   m_iOK{};  // int
    std::u16string m_wstrUserID{};  // std::wstring
    std::int64_t   m_iUserUID{};  // UidType
    std::u16string m_wstrOTP{};  // std::wstring
    std::uint32_t  m_uiPublisherUID{};  // unsigned int
    std::int32_t   m_iChannelingCode{};  // int
    std::u16string m_wstrPurchaseTok{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_wstrUserID)>>::put(w, m_wstrUserID);
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
        Codec<std::remove_cvref_t<decltype(m_wstrOTP)>>::put(w, m_wstrOTP);
        Codec<std::remove_cvref_t<decltype(m_uiPublisherUID)>>::put(w, m_uiPublisherUID);
        Codec<std::remove_cvref_t<decltype(m_iChannelingCode)>>::put(w, m_iChannelingCode);
        Codec<std::remove_cvref_t<decltype(m_wstrPurchaseTok)>>::put(w, m_wstrPurchaseTok);
    }

    static KDBE_CH_USER_GENERAL_LOGIN_ACK read_from(Reader& r) {
        KDBE_CH_USER_GENERAL_LOGIN_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_wstrUserID = Codec<std::remove_cvref_t<decltype(v.m_wstrUserID)>>::get(r);
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        v.m_wstrOTP = Codec<std::remove_cvref_t<decltype(v.m_wstrOTP)>>::get(r);
        v.m_uiPublisherUID = Codec<std::remove_cvref_t<decltype(v.m_uiPublisherUID)>>::get(r);
        v.m_iChannelingCode = Codec<std::remove_cvref_t<decltype(v.m_iChannelingCode)>>::get(r);
        v.m_wstrPurchaseTok = Codec<std::remove_cvref_t<decltype(v.m_wstrPurchaseTok)>>::get(r);
        return v;
    }
};

struct KDBE_CH_USER_GENERAL_LOGIN_REQ {
    std::u16string m_wstrServiceAccountID{};  // std::wstring
    std::u16string m_wstrIP{};  // std::wstring
    std::uint32_t  m_uiPublisherUID{};  // unsigned int
    std::int32_t   m_iChannelingCode{};  // int
    std::u16string m_wstrPurchaseTok{};  // std::wstring
    bool           m_bServerUseKogOTP{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrServiceAccountID)>>::put(w, m_wstrServiceAccountID);
        Codec<std::remove_cvref_t<decltype(m_wstrIP)>>::put(w, m_wstrIP);
        Codec<std::remove_cvref_t<decltype(m_uiPublisherUID)>>::put(w, m_uiPublisherUID);
        Codec<std::remove_cvref_t<decltype(m_iChannelingCode)>>::put(w, m_iChannelingCode);
        Codec<std::remove_cvref_t<decltype(m_wstrPurchaseTok)>>::put(w, m_wstrPurchaseTok);
        Codec<std::remove_cvref_t<decltype(m_bServerUseKogOTP)>>::put(w, m_bServerUseKogOTP);
    }

    static KDBE_CH_USER_GENERAL_LOGIN_REQ read_from(Reader& r) {
        KDBE_CH_USER_GENERAL_LOGIN_REQ v;
        v.m_wstrServiceAccountID = Codec<std::remove_cvref_t<decltype(v.m_wstrServiceAccountID)>>::get(r);
        v.m_wstrIP = Codec<std::remove_cvref_t<decltype(v.m_wstrIP)>>::get(r);
        v.m_uiPublisherUID = Codec<std::remove_cvref_t<decltype(v.m_uiPublisherUID)>>::get(r);
        v.m_iChannelingCode = Codec<std::remove_cvref_t<decltype(v.m_iChannelingCode)>>::get(r);
        v.m_wstrPurchaseTok = Codec<std::remove_cvref_t<decltype(v.m_wstrPurchaseTok)>>::get(r);
        v.m_bServerUseKogOTP = Codec<std::remove_cvref_t<decltype(v.m_bServerUseKogOTP)>>::get(r);
        return v;
    }
};

struct KDBE_CH_USER_KOGOTP_LOGIN_ACK {
    std::int32_t   m_iOK{};  // int
    std::u16string m_wstrUserID{};  // std::wstring
    std::int64_t   m_iUserUID{};  // UidType
    std::u16string m_wstrOTP{};  // std::wstring
    std::uint32_t  m_uiPublisherUID{};  // unsigned int
    std::u16string m_wstrPurchaseTok{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_wstrUserID)>>::put(w, m_wstrUserID);
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
        Codec<std::remove_cvref_t<decltype(m_wstrOTP)>>::put(w, m_wstrOTP);
        Codec<std::remove_cvref_t<decltype(m_uiPublisherUID)>>::put(w, m_uiPublisherUID);
        Codec<std::remove_cvref_t<decltype(m_wstrPurchaseTok)>>::put(w, m_wstrPurchaseTok);
    }

    static KDBE_CH_USER_KOGOTP_LOGIN_ACK read_from(Reader& r) {
        KDBE_CH_USER_KOGOTP_LOGIN_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_wstrUserID = Codec<std::remove_cvref_t<decltype(v.m_wstrUserID)>>::get(r);
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        v.m_wstrOTP = Codec<std::remove_cvref_t<decltype(v.m_wstrOTP)>>::get(r);
        v.m_uiPublisherUID = Codec<std::remove_cvref_t<decltype(v.m_uiPublisherUID)>>::get(r);
        v.m_wstrPurchaseTok = Codec<std::remove_cvref_t<decltype(v.m_wstrPurchaseTok)>>::get(r);
        return v;
    }
};

struct KDBE_CH_USER_KOGOTP_LOGIN_REQ {
    std::u16string m_wstrServiceAccountID{};  // std::wstring
    std::uint32_t  m_uiPublisherUID{};  // unsigned int
    std::u16string m_wstrOTP{};  // std::wstring
    std::u16string m_wstrIP{};  // std::wstring
    std::u16string m_wstrPurchaseTok{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrServiceAccountID)>>::put(w, m_wstrServiceAccountID);
        Codec<std::remove_cvref_t<decltype(m_uiPublisherUID)>>::put(w, m_uiPublisherUID);
        Codec<std::remove_cvref_t<decltype(m_wstrOTP)>>::put(w, m_wstrOTP);
        Codec<std::remove_cvref_t<decltype(m_wstrIP)>>::put(w, m_wstrIP);
        Codec<std::remove_cvref_t<decltype(m_wstrPurchaseTok)>>::put(w, m_wstrPurchaseTok);
    }

    static KDBE_CH_USER_KOGOTP_LOGIN_REQ read_from(Reader& r) {
        KDBE_CH_USER_KOGOTP_LOGIN_REQ v;
        v.m_wstrServiceAccountID = Codec<std::remove_cvref_t<decltype(v.m_wstrServiceAccountID)>>::get(r);
        v.m_uiPublisherUID = Codec<std::remove_cvref_t<decltype(v.m_uiPublisherUID)>>::get(r);
        v.m_wstrOTP = Codec<std::remove_cvref_t<decltype(v.m_wstrOTP)>>::get(r);
        v.m_wstrIP = Codec<std::remove_cvref_t<decltype(v.m_wstrIP)>>::get(r);
        v.m_wstrPurchaseTok = Codec<std::remove_cvref_t<decltype(v.m_wstrPurchaseTok)>>::get(r);
        return v;
    }
};

struct KDBE_COUPLE_PROPOSE_USER_FIND_ACK {
    std::int32_t   m_iOK{};  // int
    std::u16string m_wstrOtherNickName{};  // std::wstring
    std::int64_t   m_iOtherUserUID{};  // UidType
    std::int64_t   m_iOtherUnitUID{};  // UidType
    char           m_cUnitClass{};
    char           m_cRelationshipType{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_wstrOtherNickName)>>::put(w, m_wstrOtherNickName);
        Codec<std::remove_cvref_t<decltype(m_iOtherUserUID)>>::put(w, m_iOtherUserUID);
        Codec<std::remove_cvref_t<decltype(m_iOtherUnitUID)>>::put(w, m_iOtherUnitUID);
        Codec<std::remove_cvref_t<decltype(m_cUnitClass)>>::put(w, m_cUnitClass);
        Codec<std::remove_cvref_t<decltype(m_cRelationshipType)>>::put(w, m_cRelationshipType);
    }

    static KDBE_COUPLE_PROPOSE_USER_FIND_ACK read_from(Reader& r) {
        KDBE_COUPLE_PROPOSE_USER_FIND_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_wstrOtherNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrOtherNickName)>>::get(r);
        v.m_iOtherUserUID = Codec<std::remove_cvref_t<decltype(v.m_iOtherUserUID)>>::get(r);
        v.m_iOtherUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iOtherUnitUID)>>::get(r);
        v.m_cUnitClass = Codec<std::remove_cvref_t<decltype(v.m_cUnitClass)>>::get(r);
        v.m_cRelationshipType = Codec<std::remove_cvref_t<decltype(v.m_cRelationshipType)>>::get(r);
        return v;
    }
};

struct KDBE_CREATE_SECOND_SECURITY_ACK {
    std::int32_t   m_iOK{};  // int
    std::u16string m_wstrSecondPW{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_wstrSecondPW)>>::put(w, m_wstrSecondPW);
    }

    static KDBE_CREATE_SECOND_SECURITY_ACK read_from(Reader& r) {
        KDBE_CREATE_SECOND_SECURITY_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_wstrSecondPW = Codec<std::remove_cvref_t<decltype(v.m_wstrSecondPW)>>::get(r);
        return v;
    }
};

struct KDBE_CREATE_SECOND_SECURITY_REQ {
    std::int64_t   m_iUserID{};  // UidType
    std::u16string m_wstrSecondPW{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUserID)>>::put(w, m_iUserID);
        Codec<std::remove_cvref_t<decltype(m_wstrSecondPW)>>::put(w, m_wstrSecondPW);
    }

    static KDBE_CREATE_SECOND_SECURITY_REQ read_from(Reader& r) {
        KDBE_CREATE_SECOND_SECURITY_REQ v;
        v.m_iUserID = Codec<std::remove_cvref_t<decltype(v.m_iUserID)>>::get(r);
        v.m_wstrSecondPW = Codec<std::remove_cvref_t<decltype(v.m_wstrSecondPW)>>::get(r);
        return v;
    }
};

struct KDBE_DECIDE_TO_USE_THIS_SKILL_PAGE_ACK {
    std::int32_t m_iOK{};  // int
    std::int32_t m_iSkillPagesNumberToBeActive{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iSkillPagesNumberToBeActive)>>::put(w, m_iSkillPagesNumberToBeActive);
    }

    static KDBE_DECIDE_TO_USE_THIS_SKILL_PAGE_ACK read_from(Reader& r) {
        KDBE_DECIDE_TO_USE_THIS_SKILL_PAGE_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iSkillPagesNumberToBeActive = Codec<std::remove_cvref_t<decltype(v.m_iSkillPagesNumberToBeActive)>>::get(r);
        return v;
    }
};

struct KDBE_DECIDE_TO_USE_THIS_SKILL_PAGE_REQ {
    std::int64_t m_iUnitUID{};  // UidType
    std::int32_t m_iSkillPagesNumberToBeActive{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iSkillPagesNumberToBeActive)>>::put(w, m_iSkillPagesNumberToBeActive);
    }

    static KDBE_DECIDE_TO_USE_THIS_SKILL_PAGE_REQ read_from(Reader& r) {
        KDBE_DECIDE_TO_USE_THIS_SKILL_PAGE_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iSkillPagesNumberToBeActive = Codec<std::remove_cvref_t<decltype(v.m_iSkillPagesNumberToBeActive)>>::get(r);
        return v;
    }
};

struct KDBE_DELETE_APLLY_FOR_ACCEPT_JOIN_GUILD_ACK {
    std::int32_t m_iOK{};  // int
    std::int32_t m_iGuildUID{};  // int
    std::int64_t m_iUnitUID{};  // UidType
    std::int64_t m_iApplyJoinUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iGuildUID)>>::put(w, m_iGuildUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iApplyJoinUnitUID)>>::put(w, m_iApplyJoinUnitUID);
    }

    static KDBE_DELETE_APLLY_FOR_ACCEPT_JOIN_GUILD_ACK read_from(Reader& r) {
        KDBE_DELETE_APLLY_FOR_ACCEPT_JOIN_GUILD_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iGuildUID = Codec<std::remove_cvref_t<decltype(v.m_iGuildUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iApplyJoinUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iApplyJoinUnitUID)>>::get(r);
        return v;
    }
};

struct KDBE_DELETE_APPLY_JOIN_GUILD_ACK {
    std::int32_t m_iOK{};  // int
    std::int32_t m_iGuildUID{};  // int
    std::int64_t m_iUnitUID{};  // UidType
    std::int64_t m_iDeletedUnitUID{};  // UidType
    bool         m_bDeleteChar{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iGuildUID)>>::put(w, m_iGuildUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iDeletedUnitUID)>>::put(w, m_iDeletedUnitUID);
        Codec<std::remove_cvref_t<decltype(m_bDeleteChar)>>::put(w, m_bDeleteChar);
    }

    static KDBE_DELETE_APPLY_JOIN_GUILD_ACK read_from(Reader& r) {
        KDBE_DELETE_APPLY_JOIN_GUILD_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iGuildUID = Codec<std::remove_cvref_t<decltype(v.m_iGuildUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iDeletedUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iDeletedUnitUID)>>::get(r);
        v.m_bDeleteChar = Codec<std::remove_cvref_t<decltype(v.m_bDeleteChar)>>::get(r);
        return v;
    }
};

struct KDBE_DELETE_FRIEND_GROUP_REQ {
    std::int64_t m_iUnitUID{};  // UidType
    char         m_cGroupID{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_cGroupID)>>::put(w, m_cGroupID);
    }

    static KDBE_DELETE_FRIEND_GROUP_REQ read_from(Reader& r) {
        KDBE_DELETE_FRIEND_GROUP_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_cGroupID = Codec<std::remove_cvref_t<decltype(v.m_cGroupID)>>::get(r);
        return v;
    }
};

struct KDBE_DELETE_FRIEND_REQ {
    std::int64_t m_iUnitUID{};  // UidType
    std::int64_t m_iFriendUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iFriendUnitUID)>>::put(w, m_iFriendUnitUID);
    }

    static KDBE_DELETE_FRIEND_REQ read_from(Reader& r) {
        KDBE_DELETE_FRIEND_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iFriendUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iFriendUnitUID)>>::get(r);
        return v;
    }
};

struct KDBE_DELETE_GUILD_AD_LIST_NOT {
    std::vector<std::int32_t>     m_vecDeleteGuildAd{};  // std::vector< int >
    std::vector<KApplyDeleteInfo> m_vecDeleteApply{};  // std::vector< KApplyDeleteInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecDeleteGuildAd)>>::put(w, m_vecDeleteGuildAd);
        Codec<std::remove_cvref_t<decltype(m_vecDeleteApply)>>::put(w, m_vecDeleteApply);
    }

    static KDBE_DELETE_GUILD_AD_LIST_NOT read_from(Reader& r) {
        KDBE_DELETE_GUILD_AD_LIST_NOT v;
        v.m_vecDeleteGuildAd = Codec<std::remove_cvref_t<decltype(v.m_vecDeleteGuildAd)>>::get(r);
        v.m_vecDeleteApply = Codec<std::remove_cvref_t<decltype(v.m_vecDeleteApply)>>::get(r);
        return v;
    }
};

struct KDBE_DELETE_LETTER_NOT {
    std::int64_t              m_iUnitUID{};  // UidType
    std::vector<std::int64_t> m_vecPostNo{};  // std::vector< UidType >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_vecPostNo)>>::put(w, m_vecPostNo);
    }

    static KDBE_DELETE_LETTER_NOT read_from(Reader& r) {
        KDBE_DELETE_LETTER_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_vecPostNo = Codec<std::remove_cvref_t<decltype(v.m_vecPostNo)>>::get(r);
        return v;
    }
};

struct KDBE_DELETE_NICK_NAME_REQ {
    std::int64_t   m_iUnitUID{};  // UidType
    std::u16string m_wstrNickName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
    }

    static KDBE_DELETE_NICK_NAME_REQ read_from(Reader& r) {
        KDBE_DELETE_NICK_NAME_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        return v;
    }
};

struct KDBE_DELETE_RANKING_INFO_NOT {
    std::int32_t m_iRankingType{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iRankingType)>>::put(w, m_iRankingType);
    }

    static KDBE_DELETE_RANKING_INFO_NOT read_from(Reader& r) {
        KDBE_DELETE_RANKING_INFO_NOT v;
        v.m_iRankingType = Codec<std::remove_cvref_t<decltype(v.m_iRankingType)>>::get(r);
        return v;
    }
};

struct KDBE_DEL_BLACKLIST_USER_ACK {
    std::int32_t m_iOK{};  // int
    std::int64_t m_iBlackListUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iBlackListUnitUID)>>::put(w, m_iBlackListUnitUID);
    }

    static KDBE_DEL_BLACKLIST_USER_ACK read_from(Reader& r) {
        KDBE_DEL_BLACKLIST_USER_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iBlackListUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iBlackListUnitUID)>>::get(r);
        return v;
    }
};

struct KDBE_DEL_BLACKLIST_USER_REQ {
    std::int64_t m_iUnitUID{};  // UidType
    std::int64_t m_iBlackListUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iBlackListUnitUID)>>::put(w, m_iBlackListUnitUID);
    }

    static KDBE_DEL_BLACKLIST_USER_REQ read_from(Reader& r) {
        KDBE_DEL_BLACKLIST_USER_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iBlackListUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iBlackListUnitUID)>>::get(r);
        return v;
    }
};

struct KDBE_DEL_TUTORIAL_ACK {
    std::int32_t m_iOK{};  // int
    std::int64_t m_iTeacherUID{};  // UidType
    std::int64_t m_iStudentUID{};  // UidType
    char         m_cReason{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iTeacherUID)>>::put(w, m_iTeacherUID);
        Codec<std::remove_cvref_t<decltype(m_iStudentUID)>>::put(w, m_iStudentUID);
        Codec<std::remove_cvref_t<decltype(m_cReason)>>::put(w, m_cReason);
    }

    static KDBE_DEL_TUTORIAL_ACK read_from(Reader& r) {
        KDBE_DEL_TUTORIAL_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iTeacherUID = Codec<std::remove_cvref_t<decltype(v.m_iTeacherUID)>>::get(r);
        v.m_iStudentUID = Codec<std::remove_cvref_t<decltype(v.m_iStudentUID)>>::get(r);
        v.m_cReason = Codec<std::remove_cvref_t<decltype(v.m_cReason)>>::get(r);
        return v;
    }
};

struct KDBE_DEL_TUTORIAL_REQ {
    std::int64_t m_iTeacherUID{};  // UidType
    std::int64_t m_iStudentUID{};  // UidType
    char         m_cReason{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iTeacherUID)>>::put(w, m_iTeacherUID);
        Codec<std::remove_cvref_t<decltype(m_iStudentUID)>>::put(w, m_iStudentUID);
        Codec<std::remove_cvref_t<decltype(m_cReason)>>::put(w, m_cReason);
    }

    static KDBE_DEL_TUTORIAL_REQ read_from(Reader& r) {
        KDBE_DEL_TUTORIAL_REQ v;
        v.m_iTeacherUID = Codec<std::remove_cvref_t<decltype(v.m_iTeacherUID)>>::get(r);
        v.m_iStudentUID = Codec<std::remove_cvref_t<decltype(v.m_iStudentUID)>>::get(r);
        v.m_cReason = Codec<std::remove_cvref_t<decltype(v.m_cReason)>>::get(r);
        return v;
    }
};

struct KDBE_DENY_FRIEND_REQ {
    std::int64_t m_iUnitUID{};  // UidType
    std::int64_t m_iFriendUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iFriendUnitUID)>>::put(w, m_iFriendUnitUID);
    }

    static KDBE_DENY_FRIEND_REQ read_from(Reader& r) {
        KDBE_DENY_FRIEND_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iFriendUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iFriendUnitUID)>>::get(r);
        return v;
    }
};

struct KDBE_DISBAND_GUILD_ACK {
    std::int32_t m_iOK{};  // int
    std::int32_t m_iGuildUID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iGuildUID)>>::put(w, m_iGuildUID);
    }

    static KDBE_DISBAND_GUILD_ACK read_from(Reader& r) {
        KDBE_DISBAND_GUILD_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iGuildUID = Codec<std::remove_cvref_t<decltype(v.m_iGuildUID)>>::get(r);
        return v;
    }
};

struct KDBE_DISBAND_GUILD_REQ {
    std::int64_t m_iUnitUID{};  // UidType
    std::int32_t m_iGuildUID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iGuildUID)>>::put(w, m_iGuildUID);
    }

    static KDBE_DISBAND_GUILD_REQ read_from(Reader& r) {
        KDBE_DISBAND_GUILD_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iGuildUID = Codec<std::remove_cvref_t<decltype(v.m_iGuildUID)>>::get(r);
        return v;
    }
};

struct KDBE_DUNGEON_RESULT_LEAVE_LOG_NOT {
    std::int64_t   m_iUnitUID{};  // UidType
    std::u16string m_wstrLeaveReason{};  // std::wstring
    std::u16string m_wstrRoomState{};  // std::wstring
    std::u16string m_wstrRegDate{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrLeaveReason)>>::put(w, m_wstrLeaveReason);
        Codec<std::remove_cvref_t<decltype(m_wstrRoomState)>>::put(w, m_wstrRoomState);
        Codec<std::remove_cvref_t<decltype(m_wstrRegDate)>>::put(w, m_wstrRegDate);
    }

    static KDBE_DUNGEON_RESULT_LEAVE_LOG_NOT read_from(Reader& r) {
        KDBE_DUNGEON_RESULT_LEAVE_LOG_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_wstrLeaveReason = Codec<std::remove_cvref_t<decltype(v.m_wstrLeaveReason)>>::get(r);
        v.m_wstrRoomState = Codec<std::remove_cvref_t<decltype(v.m_wstrRoomState)>>::get(r);
        v.m_wstrRegDate = Codec<std::remove_cvref_t<decltype(v.m_wstrRegDate)>>::get(r);
        return v;
    }
};

struct KDBE_EXCHANGE_LIMIT_INFO_NOT {
    std::int32_t m_iExchangeLimitUID{};  // int
    std::int32_t m_iLimitUID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iExchangeLimitUID)>>::put(w, m_iExchangeLimitUID);
        Codec<std::remove_cvref_t<decltype(m_iLimitUID)>>::put(w, m_iLimitUID);
    }

    static KDBE_EXCHANGE_LIMIT_INFO_NOT read_from(Reader& r) {
        KDBE_EXCHANGE_LIMIT_INFO_NOT v;
        v.m_iExchangeLimitUID = Codec<std::remove_cvref_t<decltype(v.m_iExchangeLimitUID)>>::get(r);
        v.m_iLimitUID = Codec<std::remove_cvref_t<decltype(v.m_iLimitUID)>>::get(r);
        return v;
    }
};

struct KDBE_EXCHANGE_LIMIT_INFO_ROLLBACK_NOT {
    std::int32_t m_iRollBackUID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iRollBackUID)>>::put(w, m_iRollBackUID);
    }

    static KDBE_EXCHANGE_LIMIT_INFO_ROLLBACK_NOT read_from(Reader& r) {
        KDBE_EXCHANGE_LIMIT_INFO_ROLLBACK_NOT v;
        v.m_iRollBackUID = Codec<std::remove_cvref_t<decltype(v.m_iRollBackUID)>>::get(r);
        return v;
    }
};

struct KDBE_EXCHANGE_LIMIT_INFO_UPDATE_ACK {
    std::int32_t m_iOK{};  // int
    std::int32_t m_iExchangeLimitUID{};  // int
    std::int32_t m_iLimitUID{};  // int
    std::int32_t m_iRollBackUID{};  // int
    std::int64_t m_iGSUID{};  // UidType
    std::int64_t m_iUnitUID{};  // UidType
    std::int32_t m_iAfterWorkStorageKey{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iExchangeLimitUID)>>::put(w, m_iExchangeLimitUID);
        Codec<std::remove_cvref_t<decltype(m_iLimitUID)>>::put(w, m_iLimitUID);
        Codec<std::remove_cvref_t<decltype(m_iRollBackUID)>>::put(w, m_iRollBackUID);
        Codec<std::remove_cvref_t<decltype(m_iGSUID)>>::put(w, m_iGSUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iAfterWorkStorageKey)>>::put(w, m_iAfterWorkStorageKey);
    }

    static KDBE_EXCHANGE_LIMIT_INFO_UPDATE_ACK read_from(Reader& r) {
        KDBE_EXCHANGE_LIMIT_INFO_UPDATE_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iExchangeLimitUID = Codec<std::remove_cvref_t<decltype(v.m_iExchangeLimitUID)>>::get(r);
        v.m_iLimitUID = Codec<std::remove_cvref_t<decltype(v.m_iLimitUID)>>::get(r);
        v.m_iRollBackUID = Codec<std::remove_cvref_t<decltype(v.m_iRollBackUID)>>::get(r);
        v.m_iGSUID = Codec<std::remove_cvref_t<decltype(v.m_iGSUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iAfterWorkStorageKey = Codec<std::remove_cvref_t<decltype(v.m_iAfterWorkStorageKey)>>::get(r);
        return v;
    }
};

struct KDBE_EXCHANGE_LIMIT_INFO_UPDATE_REQ {
    std::int32_t   m_iExchangeLimitUID{};  // int
    std::int32_t   m_iLimitUID{};  // int
    std::int32_t   m_iDestItemID{};  // int
    std::u16string m_wstrRegDate{};  // std::wstring
    std::int64_t   m_iGSUID{};  // UidType
    std::int64_t   m_iUnitUID{};  // UidType
    std::int32_t   m_iAfterWorkStorageKey{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iExchangeLimitUID)>>::put(w, m_iExchangeLimitUID);
        Codec<std::remove_cvref_t<decltype(m_iLimitUID)>>::put(w, m_iLimitUID);
        Codec<std::remove_cvref_t<decltype(m_iDestItemID)>>::put(w, m_iDestItemID);
        Codec<std::remove_cvref_t<decltype(m_wstrRegDate)>>::put(w, m_wstrRegDate);
        Codec<std::remove_cvref_t<decltype(m_iGSUID)>>::put(w, m_iGSUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iAfterWorkStorageKey)>>::put(w, m_iAfterWorkStorageKey);
    }

    static KDBE_EXCHANGE_LIMIT_INFO_UPDATE_REQ read_from(Reader& r) {
        KDBE_EXCHANGE_LIMIT_INFO_UPDATE_REQ v;
        v.m_iExchangeLimitUID = Codec<std::remove_cvref_t<decltype(v.m_iExchangeLimitUID)>>::get(r);
        v.m_iLimitUID = Codec<std::remove_cvref_t<decltype(v.m_iLimitUID)>>::get(r);
        v.m_iDestItemID = Codec<std::remove_cvref_t<decltype(v.m_iDestItemID)>>::get(r);
        v.m_wstrRegDate = Codec<std::remove_cvref_t<decltype(v.m_wstrRegDate)>>::get(r);
        v.m_iGSUID = Codec<std::remove_cvref_t<decltype(v.m_iGSUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iAfterWorkStorageKey = Codec<std::remove_cvref_t<decltype(v.m_iAfterWorkStorageKey)>>::get(r);
        return v;
    }
};

struct KDBE_EXPAND_CHAR_SLOT_ACK {
    std::int32_t m_iOK{};  // int
    std::int32_t m_iCharSlotSize{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iCharSlotSize)>>::put(w, m_iCharSlotSize);
    }

    static KDBE_EXPAND_CHAR_SLOT_ACK read_from(Reader& r) {
        KDBE_EXPAND_CHAR_SLOT_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iCharSlotSize = Codec<std::remove_cvref_t<decltype(v.m_iCharSlotSize)>>::get(r);
        return v;
    }
};

struct KDBE_EXPAND_CHAR_SLOT_REQ {
    std::int32_t m_iExpandSlotSize{};  // int
    std::int32_t m_iCharSlotMax{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iExpandSlotSize)>>::put(w, m_iExpandSlotSize);
        Codec<std::remove_cvref_t<decltype(m_iCharSlotMax)>>::put(w, m_iCharSlotMax);
    }

    static KDBE_EXPAND_CHAR_SLOT_REQ read_from(Reader& r) {
        KDBE_EXPAND_CHAR_SLOT_REQ v;
        v.m_iExpandSlotSize = Codec<std::remove_cvref_t<decltype(v.m_iExpandSlotSize)>>::get(r);
        v.m_iCharSlotMax = Codec<std::remove_cvref_t<decltype(v.m_iCharSlotMax)>>::get(r);
        return v;
    }
};

struct KDBE_EXPAND_GUILD_MAX_MEMBER_ACK {
    std::int32_t  m_iOK{};  // int
    std::int32_t  m_iGuildUID{};  // int
    std::uint16_t m_usMaxNumMember{};  // u_short

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iGuildUID)>>::put(w, m_iGuildUID);
        Codec<std::remove_cvref_t<decltype(m_usMaxNumMember)>>::put(w, m_usMaxNumMember);
    }

    static KDBE_EXPAND_GUILD_MAX_MEMBER_ACK read_from(Reader& r) {
        KDBE_EXPAND_GUILD_MAX_MEMBER_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iGuildUID = Codec<std::remove_cvref_t<decltype(v.m_iGuildUID)>>::get(r);
        v.m_usMaxNumMember = Codec<std::remove_cvref_t<decltype(v.m_usMaxNumMember)>>::get(r);
        return v;
    }
};

struct KDBE_EXPAND_GUILD_MAX_MEMBER_REQ {
    std::int64_t m_iUnitUID{};  // UidType
    std::int32_t m_iGuildUID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iGuildUID)>>::put(w, m_iGuildUID);
    }

    static KDBE_EXPAND_GUILD_MAX_MEMBER_REQ read_from(Reader& r) {
        KDBE_EXPAND_GUILD_MAX_MEMBER_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iGuildUID = Codec<std::remove_cvref_t<decltype(v.m_iGuildUID)>>::get(r);
        return v;
    }
};

struct KDBE_EXPAND_INVENTORY_SLOT_ACK {
    std::map<std::int32_t, std::int32_t> m_mapExpandedSlot{};  // std::map< int, int >
    std::uint16_t                        m_usEventID{};  // unsigned short
    std::int32_t                         m_iED{};  // int
    std::int32_t                         m_iOK{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_mapExpandedSlot)>>::put(w, m_mapExpandedSlot);
        Codec<std::remove_cvref_t<decltype(m_usEventID)>>::put(w, m_usEventID);
        Codec<std::remove_cvref_t<decltype(m_iED)>>::put(w, m_iED);
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
    }

    static KDBE_EXPAND_INVENTORY_SLOT_ACK read_from(Reader& r) {
        KDBE_EXPAND_INVENTORY_SLOT_ACK v;
        v.m_mapExpandedSlot = Codec<std::remove_cvref_t<decltype(v.m_mapExpandedSlot)>>::get(r);
        v.m_usEventID = Codec<std::remove_cvref_t<decltype(v.m_usEventID)>>::get(r);
        v.m_iED = Codec<std::remove_cvref_t<decltype(v.m_iED)>>::get(r);
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        return v;
    }
};

struct KDBE_EXPAND_INVENTORY_SLOT_REQ {
    std::int64_t                         m_iUnitUID{};  // UidType
    std::uint16_t                        m_usEventID{};  // unsigned short
    std::map<std::int32_t, std::int32_t> m_mapExpandedSlot{};  // std::map< int, int >
    std::int32_t                         m_iED{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_usEventID)>>::put(w, m_usEventID);
        Codec<std::remove_cvref_t<decltype(m_mapExpandedSlot)>>::put(w, m_mapExpandedSlot);
        Codec<std::remove_cvref_t<decltype(m_iED)>>::put(w, m_iED);
    }

    static KDBE_EXPAND_INVENTORY_SLOT_REQ read_from(Reader& r) {
        KDBE_EXPAND_INVENTORY_SLOT_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_usEventID = Codec<std::remove_cvref_t<decltype(v.m_usEventID)>>::get(r);
        v.m_mapExpandedSlot = Codec<std::remove_cvref_t<decltype(v.m_mapExpandedSlot)>>::get(r);
        v.m_iED = Codec<std::remove_cvref_t<decltype(v.m_iED)>>::get(r);
        return v;
    }
};

struct KDBE_EXPAND_SKILL_NOTE_PAGE_ACK {
    std::int32_t m_iOK{};  // int
    char         m_cExpandedMaxPageNum{};
    char         m_cRollBackMaxPageNum{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_cExpandedMaxPageNum)>>::put(w, m_cExpandedMaxPageNum);
        Codec<std::remove_cvref_t<decltype(m_cRollBackMaxPageNum)>>::put(w, m_cRollBackMaxPageNum);
    }

    static KDBE_EXPAND_SKILL_NOTE_PAGE_ACK read_from(Reader& r) {
        KDBE_EXPAND_SKILL_NOTE_PAGE_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_cExpandedMaxPageNum = Codec<std::remove_cvref_t<decltype(v.m_cExpandedMaxPageNum)>>::get(r);
        v.m_cRollBackMaxPageNum = Codec<std::remove_cvref_t<decltype(v.m_cRollBackMaxPageNum)>>::get(r);
        return v;
    }
};

struct KDBE_EXPAND_SKILL_NOTE_PAGE_REQ {
    std::int64_t m_iUnitUID{};  // UidType
    char         m_cExpandedMaxPageNum{};
    char         m_cRollBackMaxPageNum{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_cExpandedMaxPageNum)>>::put(w, m_cExpandedMaxPageNum);
        Codec<std::remove_cvref_t<decltype(m_cRollBackMaxPageNum)>>::put(w, m_cRollBackMaxPageNum);
    }

    static KDBE_EXPAND_SKILL_NOTE_PAGE_REQ read_from(Reader& r) {
        KDBE_EXPAND_SKILL_NOTE_PAGE_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_cExpandedMaxPageNum = Codec<std::remove_cvref_t<decltype(v.m_cExpandedMaxPageNum)>>::get(r);
        v.m_cRollBackMaxPageNum = Codec<std::remove_cvref_t<decltype(v.m_cRollBackMaxPageNum)>>::get(r);
        return v;
    }
};

struct KDBE_EXPAND_SKILL_PAGE_ACK {
    std::int32_t                m_iOK{};  // int
    std::int64_t                m_iUnitUID{};  // UidType
    std::int32_t                m_iSkillPageNumberToBeAdded{};  // int
    std::int32_t                m_iSPoint{};  // int
    std::int32_t                m_iCSPoint{};  // int
    std::array<std::int32_t, 6> m_iDefaultSkill{};  // int[6]
    char                        m_cUnitClass{};
    std::uint16_t               m_usEventID{};  // unsigned short

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iSkillPageNumberToBeAdded)>>::put(w, m_iSkillPageNumberToBeAdded);
        Codec<std::remove_cvref_t<decltype(m_iSPoint)>>::put(w, m_iSPoint);
        Codec<std::remove_cvref_t<decltype(m_iCSPoint)>>::put(w, m_iCSPoint);
        Codec<std::remove_cvref_t<decltype(m_iDefaultSkill[0])>>::put(w, m_iDefaultSkill[0]);
        Codec<std::remove_cvref_t<decltype(m_iDefaultSkill[1])>>::put(w, m_iDefaultSkill[1]);
        Codec<std::remove_cvref_t<decltype(m_iDefaultSkill[2])>>::put(w, m_iDefaultSkill[2]);
        Codec<std::remove_cvref_t<decltype(m_iDefaultSkill[3])>>::put(w, m_iDefaultSkill[3]);
        Codec<std::remove_cvref_t<decltype(m_iDefaultSkill[4])>>::put(w, m_iDefaultSkill[4]);
        Codec<std::remove_cvref_t<decltype(m_iDefaultSkill[5])>>::put(w, m_iDefaultSkill[5]);
        Codec<std::remove_cvref_t<decltype(m_cUnitClass)>>::put(w, m_cUnitClass);
        Codec<std::remove_cvref_t<decltype(m_usEventID)>>::put(w, m_usEventID);
    }

    static KDBE_EXPAND_SKILL_PAGE_ACK read_from(Reader& r) {
        KDBE_EXPAND_SKILL_PAGE_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iSkillPageNumberToBeAdded = Codec<std::remove_cvref_t<decltype(v.m_iSkillPageNumberToBeAdded)>>::get(r);
        v.m_iSPoint = Codec<std::remove_cvref_t<decltype(v.m_iSPoint)>>::get(r);
        v.m_iCSPoint = Codec<std::remove_cvref_t<decltype(v.m_iCSPoint)>>::get(r);
        v.m_iDefaultSkill[0] = Codec<std::remove_cvref_t<decltype(v.m_iDefaultSkill[0])>>::get(r);
        v.m_iDefaultSkill[1] = Codec<std::remove_cvref_t<decltype(v.m_iDefaultSkill[1])>>::get(r);
        v.m_iDefaultSkill[2] = Codec<std::remove_cvref_t<decltype(v.m_iDefaultSkill[2])>>::get(r);
        v.m_iDefaultSkill[3] = Codec<std::remove_cvref_t<decltype(v.m_iDefaultSkill[3])>>::get(r);
        v.m_iDefaultSkill[4] = Codec<std::remove_cvref_t<decltype(v.m_iDefaultSkill[4])>>::get(r);
        v.m_iDefaultSkill[5] = Codec<std::remove_cvref_t<decltype(v.m_iDefaultSkill[5])>>::get(r);
        v.m_cUnitClass = Codec<std::remove_cvref_t<decltype(v.m_cUnitClass)>>::get(r);
        v.m_usEventID = Codec<std::remove_cvref_t<decltype(v.m_usEventID)>>::get(r);
        return v;
    }
};

struct KDBE_EXPAND_SKILL_PAGE_REQ {
    std::int64_t                m_iUnitUID{};  // UidType
    std::vector<std::int32_t>   m_vecSkillPageNumberToBeAdded{};  // std::vector<int>
    std::int32_t                m_iSPoint{};  // int
    std::int32_t                m_iCSPoint{};  // int
    std::array<std::int32_t, 6> m_iDefaultSkill{};  // int[6]
    char                        m_cUnitClass{};
    std::uint16_t               m_usEventID{};  // unsigned short

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_vecSkillPageNumberToBeAdded)>>::put(w, m_vecSkillPageNumberToBeAdded);
        Codec<std::remove_cvref_t<decltype(m_iSPoint)>>::put(w, m_iSPoint);
        Codec<std::remove_cvref_t<decltype(m_iCSPoint)>>::put(w, m_iCSPoint);
        Codec<std::remove_cvref_t<decltype(m_iDefaultSkill[0])>>::put(w, m_iDefaultSkill[0]);
        Codec<std::remove_cvref_t<decltype(m_iDefaultSkill[1])>>::put(w, m_iDefaultSkill[1]);
        Codec<std::remove_cvref_t<decltype(m_iDefaultSkill[2])>>::put(w, m_iDefaultSkill[2]);
        Codec<std::remove_cvref_t<decltype(m_iDefaultSkill[3])>>::put(w, m_iDefaultSkill[3]);
        Codec<std::remove_cvref_t<decltype(m_iDefaultSkill[4])>>::put(w, m_iDefaultSkill[4]);
        Codec<std::remove_cvref_t<decltype(m_iDefaultSkill[5])>>::put(w, m_iDefaultSkill[5]);
        Codec<std::remove_cvref_t<decltype(m_cUnitClass)>>::put(w, m_cUnitClass);
        Codec<std::remove_cvref_t<decltype(m_usEventID)>>::put(w, m_usEventID);
    }

    static KDBE_EXPAND_SKILL_PAGE_REQ read_from(Reader& r) {
        KDBE_EXPAND_SKILL_PAGE_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_vecSkillPageNumberToBeAdded = Codec<std::remove_cvref_t<decltype(v.m_vecSkillPageNumberToBeAdded)>>::get(r);
        v.m_iSPoint = Codec<std::remove_cvref_t<decltype(v.m_iSPoint)>>::get(r);
        v.m_iCSPoint = Codec<std::remove_cvref_t<decltype(v.m_iCSPoint)>>::get(r);
        v.m_iDefaultSkill[0] = Codec<std::remove_cvref_t<decltype(v.m_iDefaultSkill[0])>>::get(r);
        v.m_iDefaultSkill[1] = Codec<std::remove_cvref_t<decltype(v.m_iDefaultSkill[1])>>::get(r);
        v.m_iDefaultSkill[2] = Codec<std::remove_cvref_t<decltype(v.m_iDefaultSkill[2])>>::get(r);
        v.m_iDefaultSkill[3] = Codec<std::remove_cvref_t<decltype(v.m_iDefaultSkill[3])>>::get(r);
        v.m_iDefaultSkill[4] = Codec<std::remove_cvref_t<decltype(v.m_iDefaultSkill[4])>>::get(r);
        v.m_iDefaultSkill[5] = Codec<std::remove_cvref_t<decltype(v.m_iDefaultSkill[5])>>::get(r);
        v.m_cUnitClass = Codec<std::remove_cvref_t<decltype(v.m_cUnitClass)>>::get(r);
        v.m_usEventID = Codec<std::remove_cvref_t<decltype(v.m_usEventID)>>::get(r);
        return v;
    }
};

struct KDBE_EXPAND_SKILL_SLOT_ACK {
    std::int32_t   m_iOK{};  // int
    std::int32_t   m_iPeriodExpire{};  // int
    std::u16string m_wstrSkillSlotBEndDate{};  // std::wstring
    std::uint16_t  m_usEventID{};  // u_short

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iPeriodExpire)>>::put(w, m_iPeriodExpire);
        Codec<std::remove_cvref_t<decltype(m_wstrSkillSlotBEndDate)>>::put(w, m_wstrSkillSlotBEndDate);
        Codec<std::remove_cvref_t<decltype(m_usEventID)>>::put(w, m_usEventID);
    }

    static KDBE_EXPAND_SKILL_SLOT_ACK read_from(Reader& r) {
        KDBE_EXPAND_SKILL_SLOT_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iPeriodExpire = Codec<std::remove_cvref_t<decltype(v.m_iPeriodExpire)>>::get(r);
        v.m_wstrSkillSlotBEndDate = Codec<std::remove_cvref_t<decltype(v.m_wstrSkillSlotBEndDate)>>::get(r);
        v.m_usEventID = Codec<std::remove_cvref_t<decltype(v.m_usEventID)>>::get(r);
        return v;
    }
};

struct KDBE_EXPAND_SKILL_SLOT_REQ {
    std::int64_t  m_iUnitUID{};  // UidType
    std::int32_t  m_iPeriodExpire{};  // int
    std::uint16_t m_usTheNumberOfSkillPagesAvailable{};  // USHORT
    std::uint16_t m_usEventID{};  // u_short

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iPeriodExpire)>>::put(w, m_iPeriodExpire);
        Codec<std::remove_cvref_t<decltype(m_usTheNumberOfSkillPagesAvailable)>>::put(w, m_usTheNumberOfSkillPagesAvailable);
        Codec<std::remove_cvref_t<decltype(m_usEventID)>>::put(w, m_usEventID);
    }

    static KDBE_EXPAND_SKILL_SLOT_REQ read_from(Reader& r) {
        KDBE_EXPAND_SKILL_SLOT_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iPeriodExpire = Codec<std::remove_cvref_t<decltype(v.m_iPeriodExpire)>>::get(r);
        v.m_usTheNumberOfSkillPagesAvailable = Codec<std::remove_cvref_t<decltype(v.m_usTheNumberOfSkillPagesAvailable)>>::get(r);
        v.m_usEventID = Codec<std::remove_cvref_t<decltype(v.m_usEventID)>>::get(r);
        return v;
    }
};

struct KDBE_EXPIRE_CASH_SKILL_POINT_ACK {
    std::int32_t              m_iOK{};  // int
    std::vector<std::int32_t> m_vecRetrievedSPoint{};  // std::vector<int>

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_vecRetrievedSPoint)>>::put(w, m_vecRetrievedSPoint);
    }

    static KDBE_EXPIRE_CASH_SKILL_POINT_ACK read_from(Reader& r) {
        KDBE_EXPIRE_CASH_SKILL_POINT_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_vecRetrievedSPoint = Codec<std::remove_cvref_t<decltype(v.m_vecRetrievedSPoint)>>::get(r);
        return v;
    }
};

struct KDBE_EXPIRE_GUILD_CASH_SKILL_POINT_ACK {
    std::int32_t m_iOK{};  // int
    std::int32_t m_iGuildUID{};  // int
    std::int32_t m_iRetrievedSPoint{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iGuildUID)>>::put(w, m_iGuildUID);
        Codec<std::remove_cvref_t<decltype(m_iRetrievedSPoint)>>::put(w, m_iRetrievedSPoint);
    }

    static KDBE_EXPIRE_GUILD_CASH_SKILL_POINT_ACK read_from(Reader& r) {
        KDBE_EXPIRE_GUILD_CASH_SKILL_POINT_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iGuildUID = Codec<std::remove_cvref_t<decltype(v.m_iGuildUID)>>::get(r);
        v.m_iRetrievedSPoint = Codec<std::remove_cvref_t<decltype(v.m_iRetrievedSPoint)>>::get(r);
        return v;
    }
};

struct KDBE_FIRST_WRITE_COME_BACK_INFO_NOT {
    std::int64_t   m_iUserUID{};  // UidType
    std::int32_t   m_iAge{};  // int
    std::int64_t   m_iUnitUID{};  // UidType
    std::int32_t   m_iUnitClass{};  // int
    std::int32_t   m_iUnitLevel{};  // int
    bool           m_bTodayFirst{};
    std::int32_t   m_iRewardLevel{};  // int
    std::u16string m_wsrtRegDate{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
        Codec<std::remove_cvref_t<decltype(m_iAge)>>::put(w, m_iAge);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitClass)>>::put(w, m_iUnitClass);
        Codec<std::remove_cvref_t<decltype(m_iUnitLevel)>>::put(w, m_iUnitLevel);
        Codec<std::remove_cvref_t<decltype(m_bTodayFirst)>>::put(w, m_bTodayFirst);
        Codec<std::remove_cvref_t<decltype(m_iRewardLevel)>>::put(w, m_iRewardLevel);
        Codec<std::remove_cvref_t<decltype(m_wsrtRegDate)>>::put(w, m_wsrtRegDate);
    }

    static KDBE_FIRST_WRITE_COME_BACK_INFO_NOT read_from(Reader& r) {
        KDBE_FIRST_WRITE_COME_BACK_INFO_NOT v;
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        v.m_iAge = Codec<std::remove_cvref_t<decltype(v.m_iAge)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iUnitClass = Codec<std::remove_cvref_t<decltype(v.m_iUnitClass)>>::get(r);
        v.m_iUnitLevel = Codec<std::remove_cvref_t<decltype(v.m_iUnitLevel)>>::get(r);
        v.m_bTodayFirst = Codec<std::remove_cvref_t<decltype(v.m_bTodayFirst)>>::get(r);
        v.m_iRewardLevel = Codec<std::remove_cvref_t<decltype(v.m_iRewardLevel)>>::get(r);
        v.m_wsrtRegDate = Codec<std::remove_cvref_t<decltype(v.m_wsrtRegDate)>>::get(r);
        return v;
    }
};

struct KDBE_FIRST_WRITE_SECURITY_TYPE_NOT {
    std::int64_t   m_iUserUID{};  // UidType
    std::int32_t   m_iAge{};  // int
    std::int32_t   m_iSecurityType{};  // int
    std::u16string m_wstrCurrTime{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
        Codec<std::remove_cvref_t<decltype(m_iAge)>>::put(w, m_iAge);
        Codec<std::remove_cvref_t<decltype(m_iSecurityType)>>::put(w, m_iSecurityType);
        Codec<std::remove_cvref_t<decltype(m_wstrCurrTime)>>::put(w, m_wstrCurrTime);
    }

    static KDBE_FIRST_WRITE_SECURITY_TYPE_NOT read_from(Reader& r) {
        KDBE_FIRST_WRITE_SECURITY_TYPE_NOT v;
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        v.m_iAge = Codec<std::remove_cvref_t<decltype(v.m_iAge)>>::get(r);
        v.m_iSecurityType = Codec<std::remove_cvref_t<decltype(v.m_iSecurityType)>>::get(r);
        v.m_wstrCurrTime = Codec<std::remove_cvref_t<decltype(v.m_wstrCurrTime)>>::get(r);
        return v;
    }
};

struct KDBE_FRIEND_MESSAGE_NOT {
    std::int64_t   m_iSenderUnitUID{};  // UidType
    std::u16string m_wstrReceiverNickName{};  // std::wstring
    char           m_cMessageType{};
    std::u16string m_wstrMessage{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iSenderUnitUID)>>::put(w, m_iSenderUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrReceiverNickName)>>::put(w, m_wstrReceiverNickName);
        Codec<std::remove_cvref_t<decltype(m_cMessageType)>>::put(w, m_cMessageType);
        Codec<std::remove_cvref_t<decltype(m_wstrMessage)>>::put(w, m_wstrMessage);
    }

    static KDBE_FRIEND_MESSAGE_NOT read_from(Reader& r) {
        KDBE_FRIEND_MESSAGE_NOT v;
        v.m_iSenderUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iSenderUnitUID)>>::get(r);
        v.m_wstrReceiverNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrReceiverNickName)>>::get(r);
        v.m_cMessageType = Codec<std::remove_cvref_t<decltype(v.m_cMessageType)>>::get(r);
        v.m_wstrMessage = Codec<std::remove_cvref_t<decltype(v.m_wstrMessage)>>::get(r);
        return v;
    }
};

struct KDBE_GAME_CREATE_UNIT_REQ {
    std::u16string m_wstrNickName{};  // std::wstring
    std::int32_t   m_iClass{};  // int
    std::u16string m_wstrRegDate{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_iClass)>>::put(w, m_iClass);
        Codec<std::remove_cvref_t<decltype(m_wstrRegDate)>>::put(w, m_wstrRegDate);
    }

    static KDBE_GAME_CREATE_UNIT_REQ read_from(Reader& r) {
        KDBE_GAME_CREATE_UNIT_REQ v;
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_iClass = Codec<std::remove_cvref_t<decltype(v.m_iClass)>>::get(r);
        v.m_wstrRegDate = Codec<std::remove_cvref_t<decltype(v.m_wstrRegDate)>>::get(r);
        return v;
    }
};

struct KDBE_GET_CONCURRENT_USER_INFO_ACK {
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

    static KDBE_GET_CONCURRENT_USER_INFO_ACK read_from(Reader& r) {
        KDBE_GET_CONCURRENT_USER_INFO_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_cCCUType = Codec<std::remove_cvref_t<decltype(v.m_cCCUType)>>::get(r);
        v.m_iServerUID = Codec<std::remove_cvref_t<decltype(v.m_iServerUID)>>::get(r);
        v.m_wstrDate = Codec<std::remove_cvref_t<decltype(v.m_wstrDate)>>::get(r);
        v.m_vecCCUInfo = Codec<std::remove_cvref_t<decltype(v.m_vecCCUInfo)>>::get(r);
        return v;
    }
};

struct KDBE_GET_CONCURRENT_USER_INFO_REQ {
    char           m_cCCUType{};
    std::int64_t   m_iServerUID{};  // UidType
    std::u16string m_wstrDate{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cCCUType)>>::put(w, m_cCCUType);
        Codec<std::remove_cvref_t<decltype(m_iServerUID)>>::put(w, m_iServerUID);
        Codec<std::remove_cvref_t<decltype(m_wstrDate)>>::put(w, m_wstrDate);
    }

    static KDBE_GET_CONCURRENT_USER_INFO_REQ read_from(Reader& r) {
        KDBE_GET_CONCURRENT_USER_INFO_REQ v;
        v.m_cCCUType = Codec<std::remove_cvref_t<decltype(v.m_cCCUType)>>::get(r);
        v.m_iServerUID = Codec<std::remove_cvref_t<decltype(v.m_iServerUID)>>::get(r);
        v.m_wstrDate = Codec<std::remove_cvref_t<decltype(v.m_wstrDate)>>::get(r);
        return v;
    }
};

struct KDBE_GET_CONCURRENT_USER_LOGIN_ACK {
    std::int32_t m_iOK{};  // int
    std::int32_t m_iAuthLevel{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iAuthLevel)>>::put(w, m_iAuthLevel);
    }

    static KDBE_GET_CONCURRENT_USER_LOGIN_ACK read_from(Reader& r) {
        KDBE_GET_CONCURRENT_USER_LOGIN_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iAuthLevel = Codec<std::remove_cvref_t<decltype(v.m_iAuthLevel)>>::get(r);
        return v;
    }
};

struct KDBE_GET_CONCURRENT_USER_LOGIN_REQ {
    std::u16string m_wstrID{};  // std::wstring
    std::u16string m_wstrPW{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrID)>>::put(w, m_wstrID);
        Codec<std::remove_cvref_t<decltype(m_wstrPW)>>::put(w, m_wstrPW);
    }

    static KDBE_GET_CONCURRENT_USER_LOGIN_REQ read_from(Reader& r) {
        KDBE_GET_CONCURRENT_USER_LOGIN_REQ v;
        v.m_wstrID = Codec<std::remove_cvref_t<decltype(v.m_wstrID)>>::get(r);
        v.m_wstrPW = Codec<std::remove_cvref_t<decltype(v.m_wstrPW)>>::get(r);
        return v;
    }
};

struct KDBE_GET_CREATE_UNIT_TODAY_COUNT_REQ {
    std::int64_t m_iUserUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
    }

    static KDBE_GET_CREATE_UNIT_TODAY_COUNT_REQ read_from(Reader& r) {
        KDBE_GET_CREATE_UNIT_TODAY_COUNT_REQ v;
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        return v;
    }
};

struct KDBE_GET_GUILD_SKILL_ACK {
    std::int32_t m_iOK{};  // int
    std::int32_t m_iGuildUID{};  // int
    std::int64_t m_iUnitUID{};  // UidType
    std::int32_t m_iGuildSkillID{};  // int
    std::int32_t m_iGuildSkillLevel{};  // int
    std::int32_t m_iGuildSkillCSPoint{};  // int
    std::int32_t m_iGuildSPoint{};  // int
    std::int32_t m_iGuildCSPoint{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iGuildUID)>>::put(w, m_iGuildUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iGuildSkillID)>>::put(w, m_iGuildSkillID);
        Codec<std::remove_cvref_t<decltype(m_iGuildSkillLevel)>>::put(w, m_iGuildSkillLevel);
        Codec<std::remove_cvref_t<decltype(m_iGuildSkillCSPoint)>>::put(w, m_iGuildSkillCSPoint);
        Codec<std::remove_cvref_t<decltype(m_iGuildSPoint)>>::put(w, m_iGuildSPoint);
        Codec<std::remove_cvref_t<decltype(m_iGuildCSPoint)>>::put(w, m_iGuildCSPoint);
    }

    static KDBE_GET_GUILD_SKILL_ACK read_from(Reader& r) {
        KDBE_GET_GUILD_SKILL_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iGuildUID = Codec<std::remove_cvref_t<decltype(v.m_iGuildUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iGuildSkillID = Codec<std::remove_cvref_t<decltype(v.m_iGuildSkillID)>>::get(r);
        v.m_iGuildSkillLevel = Codec<std::remove_cvref_t<decltype(v.m_iGuildSkillLevel)>>::get(r);
        v.m_iGuildSkillCSPoint = Codec<std::remove_cvref_t<decltype(v.m_iGuildSkillCSPoint)>>::get(r);
        v.m_iGuildSPoint = Codec<std::remove_cvref_t<decltype(v.m_iGuildSPoint)>>::get(r);
        v.m_iGuildCSPoint = Codec<std::remove_cvref_t<decltype(v.m_iGuildCSPoint)>>::get(r);
        return v;
    }
};

struct KDBE_GET_GUILD_SKILL_REQ {
    std::int32_t m_iGuildUID{};  // int
    std::int64_t m_iUnitUID{};  // UidType
    std::int32_t m_iGuildSkillID{};  // int
    std::int32_t m_iGuildSkillLevel{};  // int
    std::int32_t m_iGuildSkillCSPoint{};  // int
    std::int32_t m_iGuildSPoint{};  // int
    std::int32_t m_iGuildCSPoint{};  // int
    std::int32_t m_iMaxGuildCSPoint{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iGuildUID)>>::put(w, m_iGuildUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iGuildSkillID)>>::put(w, m_iGuildSkillID);
        Codec<std::remove_cvref_t<decltype(m_iGuildSkillLevel)>>::put(w, m_iGuildSkillLevel);
        Codec<std::remove_cvref_t<decltype(m_iGuildSkillCSPoint)>>::put(w, m_iGuildSkillCSPoint);
        Codec<std::remove_cvref_t<decltype(m_iGuildSPoint)>>::put(w, m_iGuildSPoint);
        Codec<std::remove_cvref_t<decltype(m_iGuildCSPoint)>>::put(w, m_iGuildCSPoint);
        Codec<std::remove_cvref_t<decltype(m_iMaxGuildCSPoint)>>::put(w, m_iMaxGuildCSPoint);
    }

    static KDBE_GET_GUILD_SKILL_REQ read_from(Reader& r) {
        KDBE_GET_GUILD_SKILL_REQ v;
        v.m_iGuildUID = Codec<std::remove_cvref_t<decltype(v.m_iGuildUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iGuildSkillID = Codec<std::remove_cvref_t<decltype(v.m_iGuildSkillID)>>::get(r);
        v.m_iGuildSkillLevel = Codec<std::remove_cvref_t<decltype(v.m_iGuildSkillLevel)>>::get(r);
        v.m_iGuildSkillCSPoint = Codec<std::remove_cvref_t<decltype(v.m_iGuildSkillCSPoint)>>::get(r);
        v.m_iGuildSPoint = Codec<std::remove_cvref_t<decltype(v.m_iGuildSPoint)>>::get(r);
        v.m_iGuildCSPoint = Codec<std::remove_cvref_t<decltype(v.m_iGuildCSPoint)>>::get(r);
        v.m_iMaxGuildCSPoint = Codec<std::remove_cvref_t<decltype(v.m_iMaxGuildCSPoint)>>::get(r);
        return v;
    }
};

struct KDBE_GET_NICKNAME_BY_UNITUID_ACK {
    std::map<std::int64_t, std::u16string> m_mapNickName{};  // std::map< UidType, std::wstring >
    std::vector<KBillOrderInfo>            m_vecBillOrderInfo{};  // std::vector< KBillOrderInfo >
    std::vector<KBillPackageInfo>          m_vecPackageInfo{};  // std::vector< KBillPackageInfo >
    std::vector<std::int32_t>              m_vecSubPackageTrans{};  // std::vector< int >
    std::vector<std::int32_t>              m_vecSubPackageInfo{};  // std::vector< int >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_mapNickName)>>::put(w, m_mapNickName);
        Codec<std::remove_cvref_t<decltype(m_vecBillOrderInfo)>>::put(w, m_vecBillOrderInfo);
        Codec<std::remove_cvref_t<decltype(m_vecPackageInfo)>>::put(w, m_vecPackageInfo);
        Codec<std::remove_cvref_t<decltype(m_vecSubPackageTrans)>>::put(w, m_vecSubPackageTrans);
        Codec<std::remove_cvref_t<decltype(m_vecSubPackageInfo)>>::put(w, m_vecSubPackageInfo);
    }

    static KDBE_GET_NICKNAME_BY_UNITUID_ACK read_from(Reader& r) {
        KDBE_GET_NICKNAME_BY_UNITUID_ACK v;
        v.m_mapNickName = Codec<std::remove_cvref_t<decltype(v.m_mapNickName)>>::get(r);
        v.m_vecBillOrderInfo = Codec<std::remove_cvref_t<decltype(v.m_vecBillOrderInfo)>>::get(r);
        v.m_vecPackageInfo = Codec<std::remove_cvref_t<decltype(v.m_vecPackageInfo)>>::get(r);
        v.m_vecSubPackageTrans = Codec<std::remove_cvref_t<decltype(v.m_vecSubPackageTrans)>>::get(r);
        v.m_vecSubPackageInfo = Codec<std::remove_cvref_t<decltype(v.m_vecSubPackageInfo)>>::get(r);
        return v;
    }
};

struct KDBE_GET_NICKNAME_BY_UNITUID_REQ {
    std::vector<std::int64_t>     m_vecUID{};  // std::vector< UidType >
    std::vector<KBillOrderInfo>   m_vecBillOrderInfo{};  // std::vector< KBillOrderInfo >
    std::vector<KBillPackageInfo> m_vecPackageInfo{};  // std::vector< KBillPackageInfo >
    std::vector<std::int32_t>     m_vecSubPackageTrans{};  // std::vector< int >
    std::vector<std::int32_t>     m_vecSubPackageInfo{};  // std::vector< int >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecUID)>>::put(w, m_vecUID);
        Codec<std::remove_cvref_t<decltype(m_vecBillOrderInfo)>>::put(w, m_vecBillOrderInfo);
        Codec<std::remove_cvref_t<decltype(m_vecPackageInfo)>>::put(w, m_vecPackageInfo);
        Codec<std::remove_cvref_t<decltype(m_vecSubPackageTrans)>>::put(w, m_vecSubPackageTrans);
        Codec<std::remove_cvref_t<decltype(m_vecSubPackageInfo)>>::put(w, m_vecSubPackageInfo);
    }

    static KDBE_GET_NICKNAME_BY_UNITUID_REQ read_from(Reader& r) {
        KDBE_GET_NICKNAME_BY_UNITUID_REQ v;
        v.m_vecUID = Codec<std::remove_cvref_t<decltype(v.m_vecUID)>>::get(r);
        v.m_vecBillOrderInfo = Codec<std::remove_cvref_t<decltype(v.m_vecBillOrderInfo)>>::get(r);
        v.m_vecPackageInfo = Codec<std::remove_cvref_t<decltype(v.m_vecPackageInfo)>>::get(r);
        v.m_vecSubPackageTrans = Codec<std::remove_cvref_t<decltype(v.m_vecSubPackageTrans)>>::get(r);
        v.m_vecSubPackageInfo = Codec<std::remove_cvref_t<decltype(v.m_vecSubPackageInfo)>>::get(r);
        return v;
    }
};

struct KDBE_GET_POST_LETTER_LIST_REQ {
    std::int64_t m_iUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
    }

    static KDBE_GET_POST_LETTER_LIST_REQ read_from(Reader& r) {
        KDBE_GET_POST_LETTER_LIST_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        return v;
    }
};

struct KDBE_GET_PURCHASED_CASH_ITEM_ACK {
    std::int32_t m_iOK{};  // int
    std::int32_t m_iItemID{};  // int
    std::int32_t m_iQuantity{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iItemID)>>::put(w, m_iItemID);
        Codec<std::remove_cvref_t<decltype(m_iQuantity)>>::put(w, m_iQuantity);
    }

    static KDBE_GET_PURCHASED_CASH_ITEM_ACK read_from(Reader& r) {
        KDBE_GET_PURCHASED_CASH_ITEM_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iItemID = Codec<std::remove_cvref_t<decltype(v.m_iItemID)>>::get(r);
        v.m_iQuantity = Codec<std::remove_cvref_t<decltype(v.m_iQuantity)>>::get(r);
        return v;
    }
};

struct KDBE_GET_PURCHASED_CASH_ITEM_REQ {
    std::int32_t m_iOrderNo{};  // int
    std::int32_t m_iProductNo{};  // int
    std::int32_t m_iPackageProductNo{};  // int
    std::int32_t m_iQuantity{};  // int
    std::int32_t m_iItemID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOrderNo)>>::put(w, m_iOrderNo);
        Codec<std::remove_cvref_t<decltype(m_iProductNo)>>::put(w, m_iProductNo);
        Codec<std::remove_cvref_t<decltype(m_iPackageProductNo)>>::put(w, m_iPackageProductNo);
        Codec<std::remove_cvref_t<decltype(m_iQuantity)>>::put(w, m_iQuantity);
        Codec<std::remove_cvref_t<decltype(m_iItemID)>>::put(w, m_iItemID);
    }

    static KDBE_GET_PURCHASED_CASH_ITEM_REQ read_from(Reader& r) {
        KDBE_GET_PURCHASED_CASH_ITEM_REQ v;
        v.m_iOrderNo = Codec<std::remove_cvref_t<decltype(v.m_iOrderNo)>>::get(r);
        v.m_iProductNo = Codec<std::remove_cvref_t<decltype(v.m_iProductNo)>>::get(r);
        v.m_iPackageProductNo = Codec<std::remove_cvref_t<decltype(v.m_iPackageProductNo)>>::get(r);
        v.m_iQuantity = Codec<std::remove_cvref_t<decltype(v.m_iQuantity)>>::get(r);
        v.m_iItemID = Codec<std::remove_cvref_t<decltype(v.m_iItemID)>>::get(r);
        return v;
    }
};

struct KDBE_GET_RECOMMEND_USER_LIST_REQ {
    std::int64_t m_iUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
    }

    static KDBE_GET_RECOMMEND_USER_LIST_REQ read_from(Reader& r) {
        KDBE_GET_RECOMMEND_USER_LIST_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        return v;
    }
};

struct KDBE_GET_RIDING_PET_LIST_REQ {
    std::int64_t m_iUnitUID{};  // UidType
    std::int32_t m_iAfterWorkStorageKey{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iAfterWorkStorageKey)>>::put(w, m_iAfterWorkStorageKey);
    }

    static KDBE_GET_RIDING_PET_LIST_REQ read_from(Reader& r) {
        KDBE_GET_RIDING_PET_LIST_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iAfterWorkStorageKey = Codec<std::remove_cvref_t<decltype(v.m_iAfterWorkStorageKey)>>::get(r);
        return v;
    }
};

struct KDBE_GET_SECOND_SECURITY_INFO_ACK {
    std::int32_t   m_iOK{};  // int
    bool           m_bUseSecondPW{};
    std::int32_t   m_iFailedCount{};  // int
    std::u16string m_wstrSecondPW{};  // std::wstring
    std::u16string m_wstrLastAuthDate{};  // std::wstring
    bool           m_bIsFirst{};
    bool           m_bIsComeBackUser{};
    std::int32_t   m_iRewardStep{};  // int
    std::u16string m_wstrLastConnectDate{};  // std::wstring
    std::u16string m_wstrComeBackBuffEndDate{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_bUseSecondPW)>>::put(w, m_bUseSecondPW);
        Codec<std::remove_cvref_t<decltype(m_iFailedCount)>>::put(w, m_iFailedCount);
        Codec<std::remove_cvref_t<decltype(m_wstrSecondPW)>>::put(w, m_wstrSecondPW);
        Codec<std::remove_cvref_t<decltype(m_wstrLastAuthDate)>>::put(w, m_wstrLastAuthDate);
        Codec<std::remove_cvref_t<decltype(m_bIsFirst)>>::put(w, m_bIsFirst);
        Codec<std::remove_cvref_t<decltype(m_bIsComeBackUser)>>::put(w, m_bIsComeBackUser);
        Codec<std::remove_cvref_t<decltype(m_iRewardStep)>>::put(w, m_iRewardStep);
        Codec<std::remove_cvref_t<decltype(m_wstrLastConnectDate)>>::put(w, m_wstrLastConnectDate);
        Codec<std::remove_cvref_t<decltype(m_wstrComeBackBuffEndDate)>>::put(w, m_wstrComeBackBuffEndDate);
    }

    static KDBE_GET_SECOND_SECURITY_INFO_ACK read_from(Reader& r) {
        KDBE_GET_SECOND_SECURITY_INFO_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_bUseSecondPW = Codec<std::remove_cvref_t<decltype(v.m_bUseSecondPW)>>::get(r);
        v.m_iFailedCount = Codec<std::remove_cvref_t<decltype(v.m_iFailedCount)>>::get(r);
        v.m_wstrSecondPW = Codec<std::remove_cvref_t<decltype(v.m_wstrSecondPW)>>::get(r);
        v.m_wstrLastAuthDate = Codec<std::remove_cvref_t<decltype(v.m_wstrLastAuthDate)>>::get(r);
        v.m_bIsFirst = Codec<std::remove_cvref_t<decltype(v.m_bIsFirst)>>::get(r);
        v.m_bIsComeBackUser = Codec<std::remove_cvref_t<decltype(v.m_bIsComeBackUser)>>::get(r);
        v.m_iRewardStep = Codec<std::remove_cvref_t<decltype(v.m_iRewardStep)>>::get(r);
        v.m_wstrLastConnectDate = Codec<std::remove_cvref_t<decltype(v.m_wstrLastConnectDate)>>::get(r);
        v.m_wstrComeBackBuffEndDate = Codec<std::remove_cvref_t<decltype(v.m_wstrComeBackBuffEndDate)>>::get(r);
        return v;
    }
};

struct KDBE_GET_SECOND_SECURITY_INFO_REQ {
    std::int64_t                         m_iUserUID{};  // UidType
    std::map<std::int32_t, std::int32_t> m_mapComeBackRewardCondition{};  // std::map< int, int >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
        Codec<std::remove_cvref_t<decltype(m_mapComeBackRewardCondition)>>::put(w, m_mapComeBackRewardCondition);
    }

    static KDBE_GET_SECOND_SECURITY_INFO_REQ read_from(Reader& r) {
        KDBE_GET_SECOND_SECURITY_INFO_REQ v;
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        v.m_mapComeBackRewardCondition = Codec<std::remove_cvref_t<decltype(v.m_mapComeBackRewardCondition)>>::get(r);
        return v;
    }
};

struct KDBE_GET_TOTAL_CCU_FOR_WAIT_QUEUE_ACK {
    std::int32_t m_iTotalCCU{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iTotalCCU)>>::put(w, m_iTotalCCU);
    }

    static KDBE_GET_TOTAL_CCU_FOR_WAIT_QUEUE_ACK read_from(Reader& r) {
        KDBE_GET_TOTAL_CCU_FOR_WAIT_QUEUE_ACK v;
        v.m_iTotalCCU = Codec<std::remove_cvref_t<decltype(v.m_iTotalCCU)>>::get(r);
        return v;
    }
};

struct KDBE_GET_WISH_LIST_ACK {
    std::map<std::int32_t, std::int32_t> m_mapWishList{};  // std::map< int, int >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_mapWishList)>>::put(w, m_mapWishList);
    }

    static KDBE_GET_WISH_LIST_ACK read_from(Reader& r) {
        KDBE_GET_WISH_LIST_ACK v;
        v.m_mapWishList = Codec<std::remove_cvref_t<decltype(v.m_mapWishList)>>::get(r);
        return v;
    }
};

struct KDBE_GET_WISH_LIST_REQ {
    std::int64_t m_iUserUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
    }

    static KDBE_GET_WISH_LIST_REQ read_from(Reader& r) {
        KDBE_GET_WISH_LIST_REQ v;
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        return v;
    }
};

struct KDBE_GIANT_USER_LOGIN_ACK {
    std::int32_t   m_iOK{};  // int
    std::u16string m_wstrUserID{};  // std::wstring
    std::int64_t   m_iUserUID{};  // UidType
    std::u16string m_wstrOTP{};  // std::wstring
    std::uint32_t  m_uiGiantUID{};  // unsigned int
    std::uint32_t  m_uiAccountType{};  // unsigned int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_wstrUserID)>>::put(w, m_wstrUserID);
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
        Codec<std::remove_cvref_t<decltype(m_wstrOTP)>>::put(w, m_wstrOTP);
        Codec<std::remove_cvref_t<decltype(m_uiGiantUID)>>::put(w, m_uiGiantUID);
        Codec<std::remove_cvref_t<decltype(m_uiAccountType)>>::put(w, m_uiAccountType);
    }

    static KDBE_GIANT_USER_LOGIN_ACK read_from(Reader& r) {
        KDBE_GIANT_USER_LOGIN_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_wstrUserID = Codec<std::remove_cvref_t<decltype(v.m_wstrUserID)>>::get(r);
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        v.m_wstrOTP = Codec<std::remove_cvref_t<decltype(v.m_wstrOTP)>>::get(r);
        v.m_uiGiantUID = Codec<std::remove_cvref_t<decltype(v.m_uiGiantUID)>>::get(r);
        v.m_uiAccountType = Codec<std::remove_cvref_t<decltype(v.m_uiAccountType)>>::get(r);
        return v;
    }
};

struct KDBE_GIANT_USER_LOGIN_REQ {
    bool           m_bLogin{};
    std::u16string m_wstrServiceAccountID{};  // std::wstring
    std::uint32_t  m_uiGiantUID{};  // unsigned int
    std::u16string m_wstrOTP{};  // std::wstring
    std::uint32_t  m_uiAccountType{};  // unsigned int
    std::uint32_t  m_uiAccountState{};  // unsigned int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bLogin)>>::put(w, m_bLogin);
        Codec<std::remove_cvref_t<decltype(m_wstrServiceAccountID)>>::put(w, m_wstrServiceAccountID);
        Codec<std::remove_cvref_t<decltype(m_uiGiantUID)>>::put(w, m_uiGiantUID);
        Codec<std::remove_cvref_t<decltype(m_wstrOTP)>>::put(w, m_wstrOTP);
        Codec<std::remove_cvref_t<decltype(m_uiAccountType)>>::put(w, m_uiAccountType);
        Codec<std::remove_cvref_t<decltype(m_uiAccountState)>>::put(w, m_uiAccountState);
    }

    static KDBE_GIANT_USER_LOGIN_REQ read_from(Reader& r) {
        KDBE_GIANT_USER_LOGIN_REQ v;
        v.m_bLogin = Codec<std::remove_cvref_t<decltype(v.m_bLogin)>>::get(r);
        v.m_wstrServiceAccountID = Codec<std::remove_cvref_t<decltype(v.m_wstrServiceAccountID)>>::get(r);
        v.m_uiGiantUID = Codec<std::remove_cvref_t<decltype(v.m_uiGiantUID)>>::get(r);
        v.m_wstrOTP = Codec<std::remove_cvref_t<decltype(v.m_wstrOTP)>>::get(r);
        v.m_uiAccountType = Codec<std::remove_cvref_t<decltype(v.m_uiAccountType)>>::get(r);
        v.m_uiAccountState = Codec<std::remove_cvref_t<decltype(v.m_uiAccountState)>>::get(r);
        return v;
    }
};

struct KDBE_GUILD_LEVEL_UP_ACK {
    std::int32_t   m_iOK{};  // int
    std::int32_t   m_iGuildUID{};  // int
    std::u16string m_wstrGuildName{};  // std::wstring
    std::uint8_t   m_ucGuildLevel{};  // u_char
    std::int32_t   m_iGuildSPoint{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iGuildUID)>>::put(w, m_iGuildUID);
        Codec<std::remove_cvref_t<decltype(m_wstrGuildName)>>::put(w, m_wstrGuildName);
        Codec<std::remove_cvref_t<decltype(m_ucGuildLevel)>>::put(w, m_ucGuildLevel);
        Codec<std::remove_cvref_t<decltype(m_iGuildSPoint)>>::put(w, m_iGuildSPoint);
    }

    static KDBE_GUILD_LEVEL_UP_ACK read_from(Reader& r) {
        KDBE_GUILD_LEVEL_UP_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iGuildUID = Codec<std::remove_cvref_t<decltype(v.m_iGuildUID)>>::get(r);
        v.m_wstrGuildName = Codec<std::remove_cvref_t<decltype(v.m_wstrGuildName)>>::get(r);
        v.m_ucGuildLevel = Codec<std::remove_cvref_t<decltype(v.m_ucGuildLevel)>>::get(r);
        v.m_iGuildSPoint = Codec<std::remove_cvref_t<decltype(v.m_iGuildSPoint)>>::get(r);
        return v;
    }
};

struct KDBE_GUILD_LEVEL_UP_REQ {
    std::int32_t   m_iGuildUID{};  // int
    std::u16string m_wstrGuildName{};  // std::wstring
    std::uint8_t   m_ucGuildLevel{};  // u_char
    std::int32_t   m_iGuildSPoint{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iGuildUID)>>::put(w, m_iGuildUID);
        Codec<std::remove_cvref_t<decltype(m_wstrGuildName)>>::put(w, m_wstrGuildName);
        Codec<std::remove_cvref_t<decltype(m_ucGuildLevel)>>::put(w, m_ucGuildLevel);
        Codec<std::remove_cvref_t<decltype(m_iGuildSPoint)>>::put(w, m_iGuildSPoint);
    }

    static KDBE_GUILD_LEVEL_UP_REQ read_from(Reader& r) {
        KDBE_GUILD_LEVEL_UP_REQ v;
        v.m_iGuildUID = Codec<std::remove_cvref_t<decltype(v.m_iGuildUID)>>::get(r);
        v.m_wstrGuildName = Codec<std::remove_cvref_t<decltype(v.m_wstrGuildName)>>::get(r);
        v.m_ucGuildLevel = Codec<std::remove_cvref_t<decltype(v.m_ucGuildLevel)>>::get(r);
        v.m_iGuildSPoint = Codec<std::remove_cvref_t<decltype(v.m_iGuildSPoint)>>::get(r);
        return v;
    }
};

struct KDBE_INCREASE_DEFENCE_CRYSTAL_REQ {
    std::u16string m_wsrtStartTime{};  // std::wstring
    std::int32_t   m_iProtectedCrystalCount{};  // int
    std::int32_t   m_iTargetCrystal{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wsrtStartTime)>>::put(w, m_wsrtStartTime);
        Codec<std::remove_cvref_t<decltype(m_iProtectedCrystalCount)>>::put(w, m_iProtectedCrystalCount);
        Codec<std::remove_cvref_t<decltype(m_iTargetCrystal)>>::put(w, m_iTargetCrystal);
    }

    static KDBE_INCREASE_DEFENCE_CRYSTAL_REQ read_from(Reader& r) {
        KDBE_INCREASE_DEFENCE_CRYSTAL_REQ v;
        v.m_wsrtStartTime = Codec<std::remove_cvref_t<decltype(v.m_wsrtStartTime)>>::get(r);
        v.m_iProtectedCrystalCount = Codec<std::remove_cvref_t<decltype(v.m_iProtectedCrystalCount)>>::get(r);
        v.m_iTargetCrystal = Codec<std::remove_cvref_t<decltype(v.m_iTargetCrystal)>>::get(r);
        return v;
    }
};

struct KDBE_INIT_SKILL_TREE_ACK {
    std::int32_t m_iOK{};  // int
    std::int32_t m_iActiveSkillPageNumber{};  // int
    std::int64_t m_iItemUID{};  // UidType
    std::int32_t m_iSPoint{};  // int
    std::int32_t m_iCSPoint{};  // int
    std::int32_t m_iDefaultSkillID1{};  // int
    std::int32_t m_iDefaultSkillID2{};  // int
    std::int32_t m_iDefaultSkillID3{};  // int
    std::int32_t m_iDefaultSkillID4{};  // int
    std::int32_t m_iDefaultSkillID5{};  // int
    std::int32_t m_iDefaultSkillID6{};  // int
    std::int32_t m_iBeforeSPoint{};  // int
    std::int32_t m_iBeforeCSPoint{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iActiveSkillPageNumber)>>::put(w, m_iActiveSkillPageNumber);
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_iSPoint)>>::put(w, m_iSPoint);
        Codec<std::remove_cvref_t<decltype(m_iCSPoint)>>::put(w, m_iCSPoint);
        Codec<std::remove_cvref_t<decltype(m_iDefaultSkillID1)>>::put(w, m_iDefaultSkillID1);
        Codec<std::remove_cvref_t<decltype(m_iDefaultSkillID2)>>::put(w, m_iDefaultSkillID2);
        Codec<std::remove_cvref_t<decltype(m_iDefaultSkillID3)>>::put(w, m_iDefaultSkillID3);
        Codec<std::remove_cvref_t<decltype(m_iDefaultSkillID4)>>::put(w, m_iDefaultSkillID4);
        Codec<std::remove_cvref_t<decltype(m_iDefaultSkillID5)>>::put(w, m_iDefaultSkillID5);
        Codec<std::remove_cvref_t<decltype(m_iDefaultSkillID6)>>::put(w, m_iDefaultSkillID6);
        Codec<std::remove_cvref_t<decltype(m_iBeforeSPoint)>>::put(w, m_iBeforeSPoint);
        Codec<std::remove_cvref_t<decltype(m_iBeforeCSPoint)>>::put(w, m_iBeforeCSPoint);
    }

    static KDBE_INIT_SKILL_TREE_ACK read_from(Reader& r) {
        KDBE_INIT_SKILL_TREE_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iActiveSkillPageNumber = Codec<std::remove_cvref_t<decltype(v.m_iActiveSkillPageNumber)>>::get(r);
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_iSPoint = Codec<std::remove_cvref_t<decltype(v.m_iSPoint)>>::get(r);
        v.m_iCSPoint = Codec<std::remove_cvref_t<decltype(v.m_iCSPoint)>>::get(r);
        v.m_iDefaultSkillID1 = Codec<std::remove_cvref_t<decltype(v.m_iDefaultSkillID1)>>::get(r);
        v.m_iDefaultSkillID2 = Codec<std::remove_cvref_t<decltype(v.m_iDefaultSkillID2)>>::get(r);
        v.m_iDefaultSkillID3 = Codec<std::remove_cvref_t<decltype(v.m_iDefaultSkillID3)>>::get(r);
        v.m_iDefaultSkillID4 = Codec<std::remove_cvref_t<decltype(v.m_iDefaultSkillID4)>>::get(r);
        v.m_iDefaultSkillID5 = Codec<std::remove_cvref_t<decltype(v.m_iDefaultSkillID5)>>::get(r);
        v.m_iDefaultSkillID6 = Codec<std::remove_cvref_t<decltype(v.m_iDefaultSkillID6)>>::get(r);
        v.m_iBeforeSPoint = Codec<std::remove_cvref_t<decltype(v.m_iBeforeSPoint)>>::get(r);
        v.m_iBeforeCSPoint = Codec<std::remove_cvref_t<decltype(v.m_iBeforeCSPoint)>>::get(r);
        return v;
    }
};

struct KDBE_INIT_SKILL_TREE_REQ {
    std::int64_t m_iUnitUID{};  // UidType
    std::int32_t m_iActiveSkillPageNumber{};  // int
    std::int64_t m_iItemUID{};  // UidType
    std::int32_t m_iSPoint{};  // int
    std::int32_t m_iCSPoint{};  // int
    std::int32_t m_iDefaultSkillID1{};  // int
    std::int32_t m_iDefaultSkillID2{};  // int
    std::int32_t m_iDefaultSkillID3{};  // int
    std::int32_t m_iDefaultSkillID4{};  // int
    std::int32_t m_iDefaultSkillID5{};  // int
    std::int32_t m_iDefaultSkillID6{};  // int
    std::int32_t m_iBeforeSPoint{};  // int
    std::int32_t m_iBeforeCSPoint{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iActiveSkillPageNumber)>>::put(w, m_iActiveSkillPageNumber);
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_iSPoint)>>::put(w, m_iSPoint);
        Codec<std::remove_cvref_t<decltype(m_iCSPoint)>>::put(w, m_iCSPoint);
        Codec<std::remove_cvref_t<decltype(m_iDefaultSkillID1)>>::put(w, m_iDefaultSkillID1);
        Codec<std::remove_cvref_t<decltype(m_iDefaultSkillID2)>>::put(w, m_iDefaultSkillID2);
        Codec<std::remove_cvref_t<decltype(m_iDefaultSkillID3)>>::put(w, m_iDefaultSkillID3);
        Codec<std::remove_cvref_t<decltype(m_iDefaultSkillID4)>>::put(w, m_iDefaultSkillID4);
        Codec<std::remove_cvref_t<decltype(m_iDefaultSkillID5)>>::put(w, m_iDefaultSkillID5);
        Codec<std::remove_cvref_t<decltype(m_iDefaultSkillID6)>>::put(w, m_iDefaultSkillID6);
        Codec<std::remove_cvref_t<decltype(m_iBeforeSPoint)>>::put(w, m_iBeforeSPoint);
        Codec<std::remove_cvref_t<decltype(m_iBeforeCSPoint)>>::put(w, m_iBeforeCSPoint);
    }

    static KDBE_INIT_SKILL_TREE_REQ read_from(Reader& r) {
        KDBE_INIT_SKILL_TREE_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iActiveSkillPageNumber = Codec<std::remove_cvref_t<decltype(v.m_iActiveSkillPageNumber)>>::get(r);
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_iSPoint = Codec<std::remove_cvref_t<decltype(v.m_iSPoint)>>::get(r);
        v.m_iCSPoint = Codec<std::remove_cvref_t<decltype(v.m_iCSPoint)>>::get(r);
        v.m_iDefaultSkillID1 = Codec<std::remove_cvref_t<decltype(v.m_iDefaultSkillID1)>>::get(r);
        v.m_iDefaultSkillID2 = Codec<std::remove_cvref_t<decltype(v.m_iDefaultSkillID2)>>::get(r);
        v.m_iDefaultSkillID3 = Codec<std::remove_cvref_t<decltype(v.m_iDefaultSkillID3)>>::get(r);
        v.m_iDefaultSkillID4 = Codec<std::remove_cvref_t<decltype(v.m_iDefaultSkillID4)>>::get(r);
        v.m_iDefaultSkillID5 = Codec<std::remove_cvref_t<decltype(v.m_iDefaultSkillID5)>>::get(r);
        v.m_iDefaultSkillID6 = Codec<std::remove_cvref_t<decltype(v.m_iDefaultSkillID6)>>::get(r);
        v.m_iBeforeSPoint = Codec<std::remove_cvref_t<decltype(v.m_iBeforeSPoint)>>::get(r);
        v.m_iBeforeCSPoint = Codec<std::remove_cvref_t<decltype(v.m_iBeforeCSPoint)>>::get(r);
        return v;
    }
};

struct KDBE_INSERT_ADVERTISEMENT_EVENT_INFO_NOT {
    std::int64_t              m_iUserUID{};  // UidType
    std::vector<std::int32_t> m_vecCompletedEvent{};  // std::vector< int >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
        Codec<std::remove_cvref_t<decltype(m_vecCompletedEvent)>>::put(w, m_vecCompletedEvent);
    }

    static KDBE_INSERT_ADVERTISEMENT_EVENT_INFO_NOT read_from(Reader& r) {
        KDBE_INSERT_ADVERTISEMENT_EVENT_INFO_NOT v;
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        v.m_vecCompletedEvent = Codec<std::remove_cvref_t<decltype(v.m_vecCompletedEvent)>>::get(r);
        return v;
    }
};

struct KDBE_INSERT_CASH_DECREASE_ERROR_LOG_NOT {
    std::int64_t m_iUserUID{};  // UidType
    std::int32_t m_iResultCode{};  // int
    std::string  m_strOutString{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
        Codec<std::remove_cvref_t<decltype(m_iResultCode)>>::put(w, m_iResultCode);
        Codec<std::remove_cvref_t<decltype(m_strOutString)>>::put(w, m_strOutString);
    }

    static KDBE_INSERT_CASH_DECREASE_ERROR_LOG_NOT read_from(Reader& r) {
        KDBE_INSERT_CASH_DECREASE_ERROR_LOG_NOT v;
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        v.m_iResultCode = Codec<std::remove_cvref_t<decltype(v.m_iResultCode)>>::get(r);
        v.m_strOutString = Codec<std::remove_cvref_t<decltype(v.m_strOutString)>>::get(r);
        return v;
    }
};

struct KDBE_INSERT_CASH_SKILL_POINT_ACK {
    std::int32_t   m_iOK{};  // int
    std::int32_t   m_iCSPoint{};  // int
    std::int32_t   m_iPeriod{};  // int
    std::u16string m_wstrEndDate{};  // std::wstring
    bool           m_bUpdateEndDateOnly{};
    std::int32_t   m_iSkillPointItemID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iCSPoint)>>::put(w, m_iCSPoint);
        Codec<std::remove_cvref_t<decltype(m_iPeriod)>>::put(w, m_iPeriod);
        Codec<std::remove_cvref_t<decltype(m_wstrEndDate)>>::put(w, m_wstrEndDate);
        Codec<std::remove_cvref_t<decltype(m_bUpdateEndDateOnly)>>::put(w, m_bUpdateEndDateOnly);
        Codec<std::remove_cvref_t<decltype(m_iSkillPointItemID)>>::put(w, m_iSkillPointItemID);
    }

    static KDBE_INSERT_CASH_SKILL_POINT_ACK read_from(Reader& r) {
        KDBE_INSERT_CASH_SKILL_POINT_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iCSPoint = Codec<std::remove_cvref_t<decltype(v.m_iCSPoint)>>::get(r);
        v.m_iPeriod = Codec<std::remove_cvref_t<decltype(v.m_iPeriod)>>::get(r);
        v.m_wstrEndDate = Codec<std::remove_cvref_t<decltype(v.m_wstrEndDate)>>::get(r);
        v.m_bUpdateEndDateOnly = Codec<std::remove_cvref_t<decltype(v.m_bUpdateEndDateOnly)>>::get(r);
        v.m_iSkillPointItemID = Codec<std::remove_cvref_t<decltype(v.m_iSkillPointItemID)>>::get(r);
        return v;
    }
};

struct KDBE_INSERT_CASH_SKILL_POINT_REQ {
    std::int64_t m_iUnitUID{};  // UidType
    std::int32_t m_iCSPoint{};  // int
    std::int32_t m_iPeriod{};  // int
    bool         m_bUpdateEndDateOnly{};
    std::int32_t m_iSkillPointItemID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iCSPoint)>>::put(w, m_iCSPoint);
        Codec<std::remove_cvref_t<decltype(m_iPeriod)>>::put(w, m_iPeriod);
        Codec<std::remove_cvref_t<decltype(m_bUpdateEndDateOnly)>>::put(w, m_bUpdateEndDateOnly);
        Codec<std::remove_cvref_t<decltype(m_iSkillPointItemID)>>::put(w, m_iSkillPointItemID);
    }

    static KDBE_INSERT_CASH_SKILL_POINT_REQ read_from(Reader& r) {
        KDBE_INSERT_CASH_SKILL_POINT_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iCSPoint = Codec<std::remove_cvref_t<decltype(v.m_iCSPoint)>>::get(r);
        v.m_iPeriod = Codec<std::remove_cvref_t<decltype(v.m_iPeriod)>>::get(r);
        v.m_bUpdateEndDateOnly = Codec<std::remove_cvref_t<decltype(v.m_bUpdateEndDateOnly)>>::get(r);
        v.m_iSkillPointItemID = Codec<std::remove_cvref_t<decltype(v.m_iSkillPointItemID)>>::get(r);
        return v;
    }
};

struct KDBE_INSERT_CONNECT_EVENT_REQ {
    std::u16string m_wstrEventName{};  // std::wstring
    std::int32_t   m_iEventType{};  // int
    std::u16string m_wstrNextDayStartTime{};  // std::wstring
    std::u16string m_wstrNextDayEndTime{};  // std::wstring
    std::int32_t   m_iScriptID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrEventName)>>::put(w, m_wstrEventName);
        Codec<std::remove_cvref_t<decltype(m_iEventType)>>::put(w, m_iEventType);
        Codec<std::remove_cvref_t<decltype(m_wstrNextDayStartTime)>>::put(w, m_wstrNextDayStartTime);
        Codec<std::remove_cvref_t<decltype(m_wstrNextDayEndTime)>>::put(w, m_wstrNextDayEndTime);
        Codec<std::remove_cvref_t<decltype(m_iScriptID)>>::put(w, m_iScriptID);
    }

    static KDBE_INSERT_CONNECT_EVENT_REQ read_from(Reader& r) {
        KDBE_INSERT_CONNECT_EVENT_REQ v;
        v.m_wstrEventName = Codec<std::remove_cvref_t<decltype(v.m_wstrEventName)>>::get(r);
        v.m_iEventType = Codec<std::remove_cvref_t<decltype(v.m_iEventType)>>::get(r);
        v.m_wstrNextDayStartTime = Codec<std::remove_cvref_t<decltype(v.m_wstrNextDayStartTime)>>::get(r);
        v.m_wstrNextDayEndTime = Codec<std::remove_cvref_t<decltype(v.m_wstrNextDayEndTime)>>::get(r);
        v.m_iScriptID = Codec<std::remove_cvref_t<decltype(v.m_iScriptID)>>::get(r);
        return v;
    }
};

struct KDBE_INSERT_GUILD_CASH_SKILL_POINT_ACK {
    std::int32_t   m_iOK{};  // int
    std::int32_t   m_iGuildUID{};  // int
    std::int32_t   m_iGuildCSPoint{};  // int
    std::int32_t   m_iPeriod{};  // int
    std::u16string m_wstrEndDate{};  // std::wstring
    bool           m_bUpdateEndDateOnly{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iGuildUID)>>::put(w, m_iGuildUID);
        Codec<std::remove_cvref_t<decltype(m_iGuildCSPoint)>>::put(w, m_iGuildCSPoint);
        Codec<std::remove_cvref_t<decltype(m_iPeriod)>>::put(w, m_iPeriod);
        Codec<std::remove_cvref_t<decltype(m_wstrEndDate)>>::put(w, m_wstrEndDate);
        Codec<std::remove_cvref_t<decltype(m_bUpdateEndDateOnly)>>::put(w, m_bUpdateEndDateOnly);
    }

    static KDBE_INSERT_GUILD_CASH_SKILL_POINT_ACK read_from(Reader& r) {
        KDBE_INSERT_GUILD_CASH_SKILL_POINT_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iGuildUID = Codec<std::remove_cvref_t<decltype(v.m_iGuildUID)>>::get(r);
        v.m_iGuildCSPoint = Codec<std::remove_cvref_t<decltype(v.m_iGuildCSPoint)>>::get(r);
        v.m_iPeriod = Codec<std::remove_cvref_t<decltype(v.m_iPeriod)>>::get(r);
        v.m_wstrEndDate = Codec<std::remove_cvref_t<decltype(v.m_wstrEndDate)>>::get(r);
        v.m_bUpdateEndDateOnly = Codec<std::remove_cvref_t<decltype(v.m_bUpdateEndDateOnly)>>::get(r);
        return v;
    }
};

struct KDBE_INSERT_GUILD_CASH_SKILL_POINT_REQ {
    std::int32_t m_iGuildUID{};  // int
    std::int32_t m_iGuildCSPoint{};  // int
    std::int32_t m_iPeriod{};  // int
    bool         m_bUpdateEndDateOnly{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iGuildUID)>>::put(w, m_iGuildUID);
        Codec<std::remove_cvref_t<decltype(m_iGuildCSPoint)>>::put(w, m_iGuildCSPoint);
        Codec<std::remove_cvref_t<decltype(m_iPeriod)>>::put(w, m_iPeriod);
        Codec<std::remove_cvref_t<decltype(m_bUpdateEndDateOnly)>>::put(w, m_bUpdateEndDateOnly);
    }

    static KDBE_INSERT_GUILD_CASH_SKILL_POINT_REQ read_from(Reader& r) {
        KDBE_INSERT_GUILD_CASH_SKILL_POINT_REQ v;
        v.m_iGuildUID = Codec<std::remove_cvref_t<decltype(v.m_iGuildUID)>>::get(r);
        v.m_iGuildCSPoint = Codec<std::remove_cvref_t<decltype(v.m_iGuildCSPoint)>>::get(r);
        v.m_iPeriod = Codec<std::remove_cvref_t<decltype(v.m_iPeriod)>>::get(r);
        v.m_bUpdateEndDateOnly = Codec<std::remove_cvref_t<decltype(v.m_bUpdateEndDateOnly)>>::get(r);
        return v;
    }
};

struct KDBE_INSERT_PERIOD_PSHOP_AGENCY_ACK {
    std::int32_t   m_iOK{};  // int
    std::u16string m_wstrAgencyExpirationDate{};  // std::wstring
    std::uint16_t  m_usEventID{};  // u_short
    char           m_cShopType{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_wstrAgencyExpirationDate)>>::put(w, m_wstrAgencyExpirationDate);
        Codec<std::remove_cvref_t<decltype(m_usEventID)>>::put(w, m_usEventID);
        Codec<std::remove_cvref_t<decltype(m_cShopType)>>::put(w, m_cShopType);
    }

    static KDBE_INSERT_PERIOD_PSHOP_AGENCY_ACK read_from(Reader& r) {
        KDBE_INSERT_PERIOD_PSHOP_AGENCY_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_wstrAgencyExpirationDate = Codec<std::remove_cvref_t<decltype(v.m_wstrAgencyExpirationDate)>>::get(r);
        v.m_usEventID = Codec<std::remove_cvref_t<decltype(v.m_usEventID)>>::get(r);
        v.m_cShopType = Codec<std::remove_cvref_t<decltype(v.m_cShopType)>>::get(r);
        return v;
    }
};

struct KDBE_INSERT_PERIOD_PSHOP_AGENCY_REQ {
    std::int64_t  m_iUnitUID{};  // UidType
    std::int16_t  m_sAgencyPeriod{};  // short
    std::uint16_t m_usEventID{};  // u_short
    char          m_cShopType{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_sAgencyPeriod)>>::put(w, m_sAgencyPeriod);
        Codec<std::remove_cvref_t<decltype(m_usEventID)>>::put(w, m_usEventID);
        Codec<std::remove_cvref_t<decltype(m_cShopType)>>::put(w, m_cShopType);
    }

    static KDBE_INSERT_PERIOD_PSHOP_AGENCY_REQ read_from(Reader& r) {
        KDBE_INSERT_PERIOD_PSHOP_AGENCY_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_sAgencyPeriod = Codec<std::remove_cvref_t<decltype(v.m_sAgencyPeriod)>>::get(r);
        v.m_usEventID = Codec<std::remove_cvref_t<decltype(v.m_usEventID)>>::get(r);
        v.m_cShopType = Codec<std::remove_cvref_t<decltype(v.m_cShopType)>>::get(r);
        return v;
    }
};

struct KDBE_INSERT_REWARD_TO_POST_REQ {
    std::int64_t   m_iFromUnitUID{};  // UidType
    std::int64_t   m_iToUnitUID{};  // UidType
    std::int32_t   m_iRewardType{};  // int
    std::int32_t   m_iRewardID{};  // int
    std::int16_t   m_sQuantity{};  // short
    std::u16string m_wstrMessage{};  // std::wstring
    bool           m_bGameServerEvent{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iFromUnitUID)>>::put(w, m_iFromUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iToUnitUID)>>::put(w, m_iToUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iRewardType)>>::put(w, m_iRewardType);
        Codec<std::remove_cvref_t<decltype(m_iRewardID)>>::put(w, m_iRewardID);
        Codec<std::remove_cvref_t<decltype(m_sQuantity)>>::put(w, m_sQuantity);
        Codec<std::remove_cvref_t<decltype(m_wstrMessage)>>::put(w, m_wstrMessage);
        Codec<std::remove_cvref_t<decltype(m_bGameServerEvent)>>::put(w, m_bGameServerEvent);
    }

    static KDBE_INSERT_REWARD_TO_POST_REQ read_from(Reader& r) {
        KDBE_INSERT_REWARD_TO_POST_REQ v;
        v.m_iFromUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iFromUnitUID)>>::get(r);
        v.m_iToUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iToUnitUID)>>::get(r);
        v.m_iRewardType = Codec<std::remove_cvref_t<decltype(v.m_iRewardType)>>::get(r);
        v.m_iRewardID = Codec<std::remove_cvref_t<decltype(v.m_iRewardID)>>::get(r);
        v.m_sQuantity = Codec<std::remove_cvref_t<decltype(v.m_sQuantity)>>::get(r);
        v.m_wstrMessage = Codec<std::remove_cvref_t<decltype(v.m_wstrMessage)>>::get(r);
        v.m_bGameServerEvent = Codec<std::remove_cvref_t<decltype(v.m_bGameServerEvent)>>::get(r);
        return v;
    }
};

struct KDBE_INSERT_TITLE_ACK {
    std::int32_t   m_iOK{};  // int
    std::int64_t   m_iUnitUID{};  // UidType
    std::int32_t   m_iTitleID{};  // int
    std::u16string m_wstrEndDate{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iTitleID)>>::put(w, m_iTitleID);
        Codec<std::remove_cvref_t<decltype(m_wstrEndDate)>>::put(w, m_wstrEndDate);
    }

    static KDBE_INSERT_TITLE_ACK read_from(Reader& r) {
        KDBE_INSERT_TITLE_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iTitleID = Codec<std::remove_cvref_t<decltype(v.m_iTitleID)>>::get(r);
        v.m_wstrEndDate = Codec<std::remove_cvref_t<decltype(v.m_wstrEndDate)>>::get(r);
        return v;
    }
};

struct KDBE_INSERT_TITLE_REQ {
    std::int64_t m_iUnitUID{};  // UidType
    std::int32_t m_iTitleID{};  // int
    std::int16_t m_sPeriod{};  // short
    bool         m_bGameServerEvent{};
    bool         m_bExpandPeriod{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iTitleID)>>::put(w, m_iTitleID);
        Codec<std::remove_cvref_t<decltype(m_sPeriod)>>::put(w, m_sPeriod);
        Codec<std::remove_cvref_t<decltype(m_bGameServerEvent)>>::put(w, m_bGameServerEvent);
        Codec<std::remove_cvref_t<decltype(m_bExpandPeriod)>>::put(w, m_bExpandPeriod);
    }

    static KDBE_INSERT_TITLE_REQ read_from(Reader& r) {
        KDBE_INSERT_TITLE_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iTitleID = Codec<std::remove_cvref_t<decltype(v.m_iTitleID)>>::get(r);
        v.m_sPeriod = Codec<std::remove_cvref_t<decltype(v.m_sPeriod)>>::get(r);
        v.m_bGameServerEvent = Codec<std::remove_cvref_t<decltype(v.m_bGameServerEvent)>>::get(r);
        v.m_bExpandPeriod = Codec<std::remove_cvref_t<decltype(v.m_bExpandPeriod)>>::get(r);
        return v;
    }
};

struct KDBE_INSERT_WARP_VIP_ACK {
    std::int32_t   m_iOK{};  // int
    std::u16string m_wstrEndDate{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_wstrEndDate)>>::put(w, m_wstrEndDate);
    }

    static KDBE_INSERT_WARP_VIP_ACK read_from(Reader& r) {
        KDBE_INSERT_WARP_VIP_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_wstrEndDate = Codec<std::remove_cvref_t<decltype(v.m_wstrEndDate)>>::get(r);
        return v;
    }
};

struct KDBE_INSERT_WARP_VIP_REQ {
    std::int64_t m_iUnitUID{};  // UidType
    std::int32_t m_iPeriod{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iPeriod)>>::put(w, m_iPeriod);
    }

    static KDBE_INSERT_WARP_VIP_REQ read_from(Reader& r) {
        KDBE_INSERT_WARP_VIP_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iPeriod = Codec<std::remove_cvref_t<decltype(v.m_iPeriod)>>::get(r);
        return v;
    }
};

struct KDBE_INSERT_WEDDING_REWARD_ACK {
    std::int32_t m_iOK{};  // int
    bool         m_bTitleReward{};
    std::int32_t m_iTitleRewardStep{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_bTitleReward)>>::put(w, m_bTitleReward);
        Codec<std::remove_cvref_t<decltype(m_iTitleRewardStep)>>::put(w, m_iTitleRewardStep);
    }

    static KDBE_INSERT_WEDDING_REWARD_ACK read_from(Reader& r) {
        KDBE_INSERT_WEDDING_REWARD_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_bTitleReward = Codec<std::remove_cvref_t<decltype(v.m_bTitleReward)>>::get(r);
        v.m_iTitleRewardStep = Codec<std::remove_cvref_t<decltype(v.m_iTitleRewardStep)>>::get(r);
        return v;
    }
};

struct KDBE_INSERT_WEDDING_REWARD_REQ {
    std::int64_t                   m_iUnitUID{};  // UidType
    std::int64_t                   m_iLoverUnitUID{};  // UidType
    bool                           m_bTitleReward{};
    std::int32_t                   m_iTitleRewardStep{};  // int
    KDBE_INSERT_REWARD_TO_POST_REQ kPostReq{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iLoverUnitUID)>>::put(w, m_iLoverUnitUID);
        Codec<std::remove_cvref_t<decltype(m_bTitleReward)>>::put(w, m_bTitleReward);
        Codec<std::remove_cvref_t<decltype(m_iTitleRewardStep)>>::put(w, m_iTitleRewardStep);
        Codec<std::remove_cvref_t<decltype(kPostReq)>>::put(w, kPostReq);
    }

    static KDBE_INSERT_WEDDING_REWARD_REQ read_from(Reader& r) {
        KDBE_INSERT_WEDDING_REWARD_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iLoverUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iLoverUnitUID)>>::get(r);
        v.m_bTitleReward = Codec<std::remove_cvref_t<decltype(v.m_bTitleReward)>>::get(r);
        v.m_iTitleRewardStep = Codec<std::remove_cvref_t<decltype(v.m_iTitleRewardStep)>>::get(r);
        v.kPostReq = Codec<std::remove_cvref_t<decltype(v.kPostReq)>>::get(r);
        return v;
    }
};

struct KDBE_ITEM_EVALUATE_ACK {
    std::int32_t              m_iOK{};  // int
    std::int64_t              m_iItemUID{};  // UidType
    std::int32_t              m_iCost{};  // int
    std::vector<std::int32_t> m_vecRandomSocket{};  // std::vector< int >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_iCost)>>::put(w, m_iCost);
        Codec<std::remove_cvref_t<decltype(m_vecRandomSocket)>>::put(w, m_vecRandomSocket);
    }

    static KDBE_ITEM_EVALUATE_ACK read_from(Reader& r) {
        KDBE_ITEM_EVALUATE_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_iCost = Codec<std::remove_cvref_t<decltype(v.m_iCost)>>::get(r);
        v.m_vecRandomSocket = Codec<std::remove_cvref_t<decltype(v.m_vecRandomSocket)>>::get(r);
        return v;
    }
};

struct KDBE_ITEM_EVALUATE_REQ {
    std::int64_t              m_iUnitUID{};  // UidType
    std::int64_t              m_iItemUID{};  // UidType
    std::int32_t              m_iCost{};  // int
    std::vector<std::int32_t> m_vecRandomSocket{};  // std::vector< int >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_iCost)>>::put(w, m_iCost);
        Codec<std::remove_cvref_t<decltype(m_vecRandomSocket)>>::put(w, m_vecRandomSocket);
    }

    static KDBE_ITEM_EVALUATE_REQ read_from(Reader& r) {
        KDBE_ITEM_EVALUATE_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_iCost = Codec<std::remove_cvref_t<decltype(v.m_iCost)>>::get(r);
        v.m_vecRandomSocket = Codec<std::remove_cvref_t<decltype(v.m_vecRandomSocket)>>::get(r);
        return v;
    }
};

struct KDBE_ITEM_EXCHANGE_LOG_NOT {
    std::int64_t                         m_iUserUID{};  // UidType
    std::int64_t                         m_iUnitUID{};  // UidType
    char                                 m_cExchangeType{};
    std::int32_t                         m_iSourceItemID{};  // int
    std::int32_t                         m_iSourceItemQuantity{};  // int
    std::map<std::int32_t, std::int32_t> m_mapResultItem{};  // std::map< int, int >
    std::u16string                       m_wstrRegDate{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_cExchangeType)>>::put(w, m_cExchangeType);
        Codec<std::remove_cvref_t<decltype(m_iSourceItemID)>>::put(w, m_iSourceItemID);
        Codec<std::remove_cvref_t<decltype(m_iSourceItemQuantity)>>::put(w, m_iSourceItemQuantity);
        Codec<std::remove_cvref_t<decltype(m_mapResultItem)>>::put(w, m_mapResultItem);
        Codec<std::remove_cvref_t<decltype(m_wstrRegDate)>>::put(w, m_wstrRegDate);
    }

    static KDBE_ITEM_EXCHANGE_LOG_NOT read_from(Reader& r) {
        KDBE_ITEM_EXCHANGE_LOG_NOT v;
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_cExchangeType = Codec<std::remove_cvref_t<decltype(v.m_cExchangeType)>>::get(r);
        v.m_iSourceItemID = Codec<std::remove_cvref_t<decltype(v.m_iSourceItemID)>>::get(r);
        v.m_iSourceItemQuantity = Codec<std::remove_cvref_t<decltype(v.m_iSourceItemQuantity)>>::get(r);
        v.m_mapResultItem = Codec<std::remove_cvref_t<decltype(v.m_mapResultItem)>>::get(r);
        v.m_wstrRegDate = Codec<std::remove_cvref_t<decltype(v.m_wstrRegDate)>>::get(r);
        return v;
    }
};

struct KDBE_JOIN_ACCOUNT_CHECK_IP_LOG_NOT {
    std::int64_t   m_iUserUID{};  // UidType
    std::u16string m_wstrUserID{};  // std::wstring
    std::u16string m_wstrLoginIP{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
        Codec<std::remove_cvref_t<decltype(m_wstrUserID)>>::put(w, m_wstrUserID);
        Codec<std::remove_cvref_t<decltype(m_wstrLoginIP)>>::put(w, m_wstrLoginIP);
    }

    static KDBE_JOIN_ACCOUNT_CHECK_IP_LOG_NOT read_from(Reader& r) {
        KDBE_JOIN_ACCOUNT_CHECK_IP_LOG_NOT v;
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        v.m_wstrUserID = Codec<std::remove_cvref_t<decltype(v.m_wstrUserID)>>::get(r);
        v.m_wstrLoginIP = Codec<std::remove_cvref_t<decltype(v.m_wstrLoginIP)>>::get(r);
        return v;
    }
};

struct KDBE_KICK_GUILD_MEMBER_ACK {
    std::int32_t m_iOK{};  // int
    std::int64_t m_iUnitUID{};  // UidType
    std::int32_t m_iGuildUID{};  // int
    std::int64_t m_iTargetUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iGuildUID)>>::put(w, m_iGuildUID);
        Codec<std::remove_cvref_t<decltype(m_iTargetUnitUID)>>::put(w, m_iTargetUnitUID);
    }

    static KDBE_KICK_GUILD_MEMBER_ACK read_from(Reader& r) {
        KDBE_KICK_GUILD_MEMBER_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iGuildUID = Codec<std::remove_cvref_t<decltype(v.m_iGuildUID)>>::get(r);
        v.m_iTargetUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iTargetUnitUID)>>::get(r);
        return v;
    }
};

struct KDBE_KICK_GUILD_MEMBER_REQ {
    std::int64_t m_iUnitUID{};  // UidType
    std::int32_t m_iGuildUID{};  // int
    std::int64_t m_iTargetUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iGuildUID)>>::put(w, m_iGuildUID);
        Codec<std::remove_cvref_t<decltype(m_iTargetUnitUID)>>::put(w, m_iTargetUnitUID);
    }

    static KDBE_KICK_GUILD_MEMBER_REQ read_from(Reader& r) {
        KDBE_KICK_GUILD_MEMBER_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iGuildUID = Codec<std::remove_cvref_t<decltype(v.m_iGuildUID)>>::get(r);
        v.m_iTargetUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iTargetUnitUID)>>::get(r);
        return v;
    }
};

struct KDBE_MAKE_FRIEND_GROUP_REQ {
    std::int64_t   m_iUnitUID{};  // UidType
    char           m_cGroupID{};
    std::u16string m_wstrGroupName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_cGroupID)>>::put(w, m_cGroupID);
        Codec<std::remove_cvref_t<decltype(m_wstrGroupName)>>::put(w, m_wstrGroupName);
    }

    static KDBE_MAKE_FRIEND_GROUP_REQ read_from(Reader& r) {
        KDBE_MAKE_FRIEND_GROUP_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_cGroupID = Codec<std::remove_cvref_t<decltype(v.m_cGroupID)>>::get(r);
        v.m_wstrGroupName = Codec<std::remove_cvref_t<decltype(v.m_wstrGroupName)>>::get(r);
        return v;
    }
};

struct KDBE_MOVE_FRIEND_REQ {
    std::int64_t m_iUnitUID{};  // UidType
    std::int64_t m_iFriendUnitUID{};  // UidType
    char         m_cTargetGroupID{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iFriendUnitUID)>>::put(w, m_iFriendUnitUID);
        Codec<std::remove_cvref_t<decltype(m_cTargetGroupID)>>::put(w, m_cTargetGroupID);
    }

    static KDBE_MOVE_FRIEND_REQ read_from(Reader& r) {
        KDBE_MOVE_FRIEND_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iFriendUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iFriendUnitUID)>>::get(r);
        v.m_cTargetGroupID = Codec<std::remove_cvref_t<decltype(v.m_cTargetGroupID)>>::get(r);
        return v;
    }
};

struct KDBE_NEW_BLACKLIST_USER_ACK {
    std::int32_t       m_iOK{};  // int
    KChatBlackListUnit m_kChatBlackListUnit{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_kChatBlackListUnit)>>::put(w, m_kChatBlackListUnit);
    }

    static KDBE_NEW_BLACKLIST_USER_ACK read_from(Reader& r) {
        KDBE_NEW_BLACKLIST_USER_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_kChatBlackListUnit = Codec<std::remove_cvref_t<decltype(v.m_kChatBlackListUnit)>>::get(r);
        return v;
    }
};

struct KDBE_NEW_BLACKLIST_USER_REQ {
    std::int64_t m_iUnitUID{};  // UidType
    std::int64_t m_iBlackListUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iBlackListUnitUID)>>::put(w, m_iBlackListUnitUID);
    }

    static KDBE_NEW_BLACKLIST_USER_REQ read_from(Reader& r) {
        KDBE_NEW_BLACKLIST_USER_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iBlackListUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iBlackListUnitUID)>>::get(r);
        return v;
    }
};

struct KDBE_NEW_POST_BLACK_LIST_ACK {
    std::int32_t   m_iOK{};  // int
    std::u16string m_wstrNickName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
    }

    static KDBE_NEW_POST_BLACK_LIST_ACK read_from(Reader& r) {
        KDBE_NEW_POST_BLACK_LIST_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        return v;
    }
};

struct KDBE_NEW_POST_BLACK_LIST_REQ {
    std::int64_t   m_iUnitUID{};  // UidType
    std::u16string m_wstrNickName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
    }

    static KDBE_NEW_POST_BLACK_LIST_REQ read_from(Reader& r) {
        KDBE_NEW_POST_BLACK_LIST_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        return v;
    }
};

struct KDBE_PET_AUTO_LOOTING_NOT {
    std::int64_t m_iPetUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iPetUID)>>::put(w, m_iPetUID);
    }

    static KDBE_PET_AUTO_LOOTING_NOT read_from(Reader& r) {
        KDBE_PET_AUTO_LOOTING_NOT v;
        v.m_iPetUID = Codec<std::remove_cvref_t<decltype(v.m_iPetUID)>>::get(r);
        return v;
    }
};

struct KDBE_PURCHASED_CASH_ITEM_LIST_REQ {
    std::int32_t   m_nItemPerPage{};  // int
    std::int32_t   m_iCurrentPage{};  // int
    std::u16string m_wstrUnitNickName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_nItemPerPage)>>::put(w, m_nItemPerPage);
        Codec<std::remove_cvref_t<decltype(m_iCurrentPage)>>::put(w, m_iCurrentPage);
        Codec<std::remove_cvref_t<decltype(m_wstrUnitNickName)>>::put(w, m_wstrUnitNickName);
    }

    static KDBE_PURCHASED_CASH_ITEM_LIST_REQ read_from(Reader& r) {
        KDBE_PURCHASED_CASH_ITEM_LIST_REQ v;
        v.m_nItemPerPage = Codec<std::remove_cvref_t<decltype(v.m_nItemPerPage)>>::get(r);
        v.m_iCurrentPage = Codec<std::remove_cvref_t<decltype(v.m_iCurrentPage)>>::get(r);
        v.m_wstrUnitNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrUnitNickName)>>::get(r);
        return v;
    }
};

struct KDBE_QUIT_USER_PVP_RESULT_UPDATE_NOT {
    std::int64_t m_iUnitUID{};  // UidType
    std::int32_t m_iOfficialMatchCnt{};  // int
    std::int32_t m_iRating{};  // int
    std::int32_t m_iMaxRating{};  // int
    std::int32_t m_iRPoint{};  // int
    std::int32_t m_iAPoint{};  // int
    bool         m_bIsWinBeforeMatch{};
    std::int32_t m_cEmblemEnum{};  // int
    double       m_fKFactor{};
    std::int32_t m_iWin{};  // int
    std::int32_t m_iLose{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
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
    }

    static KDBE_QUIT_USER_PVP_RESULT_UPDATE_NOT read_from(Reader& r) {
        KDBE_QUIT_USER_PVP_RESULT_UPDATE_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
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
        return v;
    }
};

struct KDBE_READ_LETTER_NOT {
    std::int64_t m_iUnitUID{};  // UidType
    std::int64_t m_iPostNo{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iPostNo)>>::put(w, m_iPostNo);
    }

    static KDBE_READ_LETTER_NOT read_from(Reader& r) {
        KDBE_READ_LETTER_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iPostNo = Codec<std::remove_cvref_t<decltype(v.m_iPostNo)>>::get(r);
        return v;
    }
};

struct KDBE_RECOMMEND_USER_ACK {
    std::int32_t m_iOK{};  // int
    std::int64_t m_iRecommendedUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iRecommendedUnitUID)>>::put(w, m_iRecommendedUnitUID);
    }

    static KDBE_RECOMMEND_USER_ACK read_from(Reader& r) {
        KDBE_RECOMMEND_USER_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iRecommendedUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iRecommendedUnitUID)>>::get(r);
        return v;
    }
};

struct KDBE_RECOMMEND_USER_REQ {
    std::int64_t   m_iUnitUID{};  // UidType
    std::u16string m_wstrNickName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
    }

    static KDBE_RECOMMEND_USER_REQ read_from(Reader& r) {
        KDBE_RECOMMEND_USER_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        return v;
    }
};

struct KDBE_REGISTRATION_GUILD_AD_ACK {
    std::int32_t   m_iOK{};  // int
    std::int64_t   m_iUnitUID{};  // UidType
    std::int32_t   m_iGuildUID{};  // int
    std::int16_t   m_sPeriod{};  // short
    std::u16string m_wstrAdRegDate{};  // std::wstring
    std::u16string m_wstrAdEndDate{};  // std::wstring
    std::u16string m_wstrAdMessage{};  // std::wstring
    std::int32_t   m_iCost{};  // int
    bool           m_bExistExpiredAd{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iGuildUID)>>::put(w, m_iGuildUID);
        Codec<std::remove_cvref_t<decltype(m_sPeriod)>>::put(w, m_sPeriod);
        Codec<std::remove_cvref_t<decltype(m_wstrAdRegDate)>>::put(w, m_wstrAdRegDate);
        Codec<std::remove_cvref_t<decltype(m_wstrAdEndDate)>>::put(w, m_wstrAdEndDate);
        Codec<std::remove_cvref_t<decltype(m_wstrAdMessage)>>::put(w, m_wstrAdMessage);
        Codec<std::remove_cvref_t<decltype(m_iCost)>>::put(w, m_iCost);
        Codec<std::remove_cvref_t<decltype(m_bExistExpiredAd)>>::put(w, m_bExistExpiredAd);
    }

    static KDBE_REGISTRATION_GUILD_AD_ACK read_from(Reader& r) {
        KDBE_REGISTRATION_GUILD_AD_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iGuildUID = Codec<std::remove_cvref_t<decltype(v.m_iGuildUID)>>::get(r);
        v.m_sPeriod = Codec<std::remove_cvref_t<decltype(v.m_sPeriod)>>::get(r);
        v.m_wstrAdRegDate = Codec<std::remove_cvref_t<decltype(v.m_wstrAdRegDate)>>::get(r);
        v.m_wstrAdEndDate = Codec<std::remove_cvref_t<decltype(v.m_wstrAdEndDate)>>::get(r);
        v.m_wstrAdMessage = Codec<std::remove_cvref_t<decltype(v.m_wstrAdMessage)>>::get(r);
        v.m_iCost = Codec<std::remove_cvref_t<decltype(v.m_iCost)>>::get(r);
        v.m_bExistExpiredAd = Codec<std::remove_cvref_t<decltype(v.m_bExistExpiredAd)>>::get(r);
        return v;
    }
};

struct KDBE_REGISTRATION_GUILD_AD_REQ {
    std::int64_t   m_iUnitUID{};  // UidType
    std::int32_t   m_iGuildUID{};  // int
    std::int16_t   m_sPeriod{};  // short
    std::u16string m_wstrAdMessage{};  // std::wstring
    std::int32_t   m_iCost{};  // int
    bool           m_bExistExpiredAd{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iGuildUID)>>::put(w, m_iGuildUID);
        Codec<std::remove_cvref_t<decltype(m_sPeriod)>>::put(w, m_sPeriod);
        Codec<std::remove_cvref_t<decltype(m_wstrAdMessage)>>::put(w, m_wstrAdMessage);
        Codec<std::remove_cvref_t<decltype(m_iCost)>>::put(w, m_iCost);
        Codec<std::remove_cvref_t<decltype(m_bExistExpiredAd)>>::put(w, m_bExistExpiredAd);
    }

    static KDBE_REGISTRATION_GUILD_AD_REQ read_from(Reader& r) {
        KDBE_REGISTRATION_GUILD_AD_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iGuildUID = Codec<std::remove_cvref_t<decltype(v.m_iGuildUID)>>::get(r);
        v.m_sPeriod = Codec<std::remove_cvref_t<decltype(v.m_sPeriod)>>::get(r);
        v.m_wstrAdMessage = Codec<std::remove_cvref_t<decltype(v.m_wstrAdMessage)>>::get(r);
        v.m_iCost = Codec<std::remove_cvref_t<decltype(v.m_iCost)>>::get(r);
        v.m_bExistExpiredAd = Codec<std::remove_cvref_t<decltype(v.m_bExistExpiredAd)>>::get(r);
        return v;
    }
};

struct KDBE_REGIST_GLOBAL_MISSION_CLEAR_NOT {
    std::u16string m_wstrGlobalMissionDay{};  // std::wstring
    std::int64_t   m_iUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrGlobalMissionDay)>>::put(w, m_wstrGlobalMissionDay);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
    }

    static KDBE_REGIST_GLOBAL_MISSION_CLEAR_NOT read_from(Reader& r) {
        KDBE_REGIST_GLOBAL_MISSION_CLEAR_NOT v;
        v.m_wstrGlobalMissionDay = Codec<std::remove_cvref_t<decltype(v.m_wstrGlobalMissionDay)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        return v;
    }
};

struct KDBE_REG_ABUSER_AUTOMATIC_NOT {
    std::map<std::int32_t, std::int64_t> m_mapRegAbuserInfo{};  // std::map< int, UidType >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_mapRegAbuserInfo)>>::put(w, m_mapRegAbuserInfo);
    }

    static KDBE_REG_ABUSER_AUTOMATIC_NOT read_from(Reader& r) {
        KDBE_REG_ABUSER_AUTOMATIC_NOT v;
        v.m_mapRegAbuserInfo = Codec<std::remove_cvref_t<decltype(v.m_mapRegAbuserInfo)>>::get(r);
        return v;
    }
};

struct KDBE_REG_DUNGEON_CLEAR_TIME_HACK_USER_NOT {
    std::int64_t   m_iUserUID{};  // UidType
    std::u16string m_wstrRegDate{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
        Codec<std::remove_cvref_t<decltype(m_wstrRegDate)>>::put(w, m_wstrRegDate);
    }

    static KDBE_REG_DUNGEON_CLEAR_TIME_HACK_USER_NOT read_from(Reader& r) {
        KDBE_REG_DUNGEON_CLEAR_TIME_HACK_USER_NOT v;
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        v.m_wstrRegDate = Codec<std::remove_cvref_t<decltype(v.m_wstrRegDate)>>::get(r);
        return v;
    }
};

struct KDBE_RELEASE_PET_REQ {
    std::int64_t m_iPetUID{};  // UidType
    std::int64_t m_iUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iPetUID)>>::put(w, m_iPetUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
    }

    static KDBE_RELEASE_PET_REQ read_from(Reader& r) {
        KDBE_RELEASE_PET_REQ v;
        v.m_iPetUID = Codec<std::remove_cvref_t<decltype(v.m_iPetUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        return v;
    }
};

struct KDBE_RELEASE_RIDING_PET_REQ {
    std::int64_t m_iRidingPetUID{};  // UidType
    std::int64_t m_iUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iRidingPetUID)>>::put(w, m_iRidingPetUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
    }

    static KDBE_RELEASE_RIDING_PET_REQ read_from(Reader& r) {
        KDBE_RELEASE_RIDING_PET_REQ v;
        v.m_iRidingPetUID = Codec<std::remove_cvref_t<decltype(v.m_iRidingPetUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        return v;
    }
};

struct KDBE_RENAME_FRIEND_GROUP_REQ {
    std::int64_t   m_iUnitUID{};  // UidType
    char           m_cGroupID{};
    std::u16string m_wstrGroupName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_cGroupID)>>::put(w, m_cGroupID);
        Codec<std::remove_cvref_t<decltype(m_wstrGroupName)>>::put(w, m_wstrGroupName);
    }

    static KDBE_RENAME_FRIEND_GROUP_REQ read_from(Reader& r) {
        KDBE_RENAME_FRIEND_GROUP_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_cGroupID = Codec<std::remove_cvref_t<decltype(v.m_cGroupID)>>::get(r);
        v.m_wstrGroupName = Codec<std::remove_cvref_t<decltype(v.m_wstrGroupName)>>::get(r);
        return v;
    }
};

struct KDBE_REQUEST_FRIEND_ACK {
    std::int32_t   m_iOK{};  // int
    std::int64_t   m_iUnitUID{};  // UidType
    std::u16string m_wstrNickName{};  // std::wstring
    std::u16string m_wstrMessage{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_wstrMessage)>>::put(w, m_wstrMessage);
    }

    static KDBE_REQUEST_FRIEND_ACK read_from(Reader& r) {
        KDBE_REQUEST_FRIEND_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_wstrMessage = Codec<std::remove_cvref_t<decltype(v.m_wstrMessage)>>::get(r);
        return v;
    }
};

struct KDBE_REQUEST_FRIEND_REQ {
    std::int64_t   m_iUnitUID{};  // UidType
    std::u16string m_wstrNickName{};  // std::wstring
    std::u16string m_wstrMessage{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_wstrMessage)>>::put(w, m_wstrMessage);
    }

    static KDBE_REQUEST_FRIEND_REQ read_from(Reader& r) {
        KDBE_REQUEST_FRIEND_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_wstrMessage = Codec<std::remove_cvref_t<decltype(v.m_wstrMessage)>>::get(r);
        return v;
    }
};

struct KDBE_RESET_PERIOD_ITEM_REQ {
    std::int64_t                         m_iUnitUID{};  // UidType
    std::map<std::int64_t, std::int16_t> m_mapExpandPeriodItemList{};  // std::map< UidType, short >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_mapExpandPeriodItemList)>>::put(w, m_mapExpandPeriodItemList);
    }

    static KDBE_RESET_PERIOD_ITEM_REQ read_from(Reader& r) {
        KDBE_RESET_PERIOD_ITEM_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_mapExpandPeriodItemList = Codec<std::remove_cvref_t<decltype(v.m_mapExpandPeriodItemList)>>::get(r);
        return v;
    }
};

struct KDBE_ROLLBACK_PURCHASED_CASH_ITEM_NOT {
    std::int32_t   m_iOrderNo{};  // int
    std::int32_t   m_iProductNo{};  // int
    std::u16string m_wstrMsg{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOrderNo)>>::put(w, m_iOrderNo);
        Codec<std::remove_cvref_t<decltype(m_iProductNo)>>::put(w, m_iProductNo);
        Codec<std::remove_cvref_t<decltype(m_wstrMsg)>>::put(w, m_wstrMsg);
    }

    static KDBE_ROLLBACK_PURCHASED_CASH_ITEM_NOT read_from(Reader& r) {
        KDBE_ROLLBACK_PURCHASED_CASH_ITEM_NOT v;
        v.m_iOrderNo = Codec<std::remove_cvref_t<decltype(v.m_iOrderNo)>>::get(r);
        v.m_iProductNo = Codec<std::remove_cvref_t<decltype(v.m_iProductNo)>>::get(r);
        v.m_wstrMsg = Codec<std::remove_cvref_t<decltype(v.m_wstrMsg)>>::get(r);
        return v;
    }
};

struct KDBE_SEND_PHONE_MSG_NOT {
    std::vector<std::u16string> m_vecPhoneNum{};  // std::vector< std::wstring >
    std::u16string              m_wstrSMSMessage{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecPhoneNum)>>::put(w, m_vecPhoneNum);
        Codec<std::remove_cvref_t<decltype(m_wstrSMSMessage)>>::put(w, m_wstrSMSMessage);
    }

    static KDBE_SEND_PHONE_MSG_NOT read_from(Reader& r) {
        KDBE_SEND_PHONE_MSG_NOT v;
        v.m_vecPhoneNum = Codec<std::remove_cvref_t<decltype(v.m_vecPhoneNum)>>::get(r);
        v.m_wstrSMSMessage = Codec<std::remove_cvref_t<decltype(v.m_wstrSMSMessage)>>::get(r);
        return v;
    }
};

struct KDBE_SEUCCESS_SECOND_SECURITY_ACK {
    std::int32_t   m_iOK{};  // int
    std::u16string m_wstrCurrTime{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_wstrCurrTime)>>::put(w, m_wstrCurrTime);
    }

    static KDBE_SEUCCESS_SECOND_SECURITY_ACK read_from(Reader& r) {
        KDBE_SEUCCESS_SECOND_SECURITY_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_wstrCurrTime = Codec<std::remove_cvref_t<decltype(v.m_wstrCurrTime)>>::get(r);
        return v;
    }
};

struct KDBE_SEUCCESS_SECOND_SECURITY_REQ {
    std::int64_t   m_iUserID{};  // UidType
    std::u16string m_wstrCurrTime{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUserID)>>::put(w, m_iUserID);
        Codec<std::remove_cvref_t<decltype(m_wstrCurrTime)>>::put(w, m_wstrCurrTime);
    }

    static KDBE_SEUCCESS_SECOND_SECURITY_REQ read_from(Reader& r) {
        KDBE_SEUCCESS_SECOND_SECURITY_REQ v;
        v.m_iUserID = Codec<std::remove_cvref_t<decltype(v.m_iUserID)>>::get(r);
        v.m_wstrCurrTime = Codec<std::remove_cvref_t<decltype(v.m_wstrCurrTime)>>::get(r);
        return v;
    }
};

struct KDBE_SHARING_BACK_OPEN_ACK {
    std::int32_t m_iOK{};  // int
    std::int32_t m_iOpenType{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iOpenType)>>::put(w, m_iOpenType);
    }

    static KDBE_SHARING_BACK_OPEN_ACK read_from(Reader& r) {
        KDBE_SHARING_BACK_OPEN_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iOpenType = Codec<std::remove_cvref_t<decltype(v.m_iOpenType)>>::get(r);
        return v;
    }
};

struct KDBE_SHARING_BACK_OPEN_REQ {
    std::int64_t m_iUserUID{};  // UidType
    std::int64_t m_iUnitUID{};  // UidType
    std::int32_t m_iOpenType{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iOpenType)>>::put(w, m_iOpenType);
    }

    static KDBE_SHARING_BACK_OPEN_REQ read_from(Reader& r) {
        KDBE_SHARING_BACK_OPEN_REQ v;
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iOpenType = Codec<std::remove_cvref_t<decltype(v.m_iOpenType)>>::get(r);
        return v;
    }
};

struct KDBE_SPIRIT_STATISTICS_NOT {
    std::int64_t m_iUnitUID{};  // UidType
    std::int32_t m_iSpiritPlayTime{};  // int
    std::int32_t m_iDurationTime{};  // int
    std::int32_t m_iUsedSpirit{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iSpiritPlayTime)>>::put(w, m_iSpiritPlayTime);
        Codec<std::remove_cvref_t<decltype(m_iDurationTime)>>::put(w, m_iDurationTime);
        Codec<std::remove_cvref_t<decltype(m_iUsedSpirit)>>::put(w, m_iUsedSpirit);
    }

    static KDBE_SPIRIT_STATISTICS_NOT read_from(Reader& r) {
        KDBE_SPIRIT_STATISTICS_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iSpiritPlayTime = Codec<std::remove_cvref_t<decltype(v.m_iSpiritPlayTime)>>::get(r);
        v.m_iDurationTime = Codec<std::remove_cvref_t<decltype(v.m_iDurationTime)>>::get(r);
        v.m_iUsedSpirit = Codec<std::remove_cvref_t<decltype(v.m_iUsedSpirit)>>::get(r);
        return v;
    }
};

struct KDBE_START_CHECK_POWER_REQ {
    std::int64_t m_iUnitUID{};  // UidType
    std::uint8_t m_ucCheckPowerCount{};  // unsigned char
    std::int64_t m_iCheckPowerTime{};  // __int64
    std::uint8_t m_ucCheckPowerScore{};  // unsigned char

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_ucCheckPowerCount)>>::put(w, m_ucCheckPowerCount);
        Codec<std::remove_cvref_t<decltype(m_iCheckPowerTime)>>::put(w, m_iCheckPowerTime);
        Codec<std::remove_cvref_t<decltype(m_ucCheckPowerScore)>>::put(w, m_ucCheckPowerScore);
    }

    static KDBE_START_CHECK_POWER_REQ read_from(Reader& r) {
        KDBE_START_CHECK_POWER_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_ucCheckPowerCount = Codec<std::remove_cvref_t<decltype(v.m_ucCheckPowerCount)>>::get(r);
        v.m_iCheckPowerTime = Codec<std::remove_cvref_t<decltype(v.m_iCheckPowerTime)>>::get(r);
        v.m_ucCheckPowerScore = Codec<std::remove_cvref_t<decltype(v.m_ucCheckPowerScore)>>::get(r);
        return v;
    }
};

struct KDBE_STOP_SALE_PSHOP_AGENCY_ACK {
    std::int32_t m_iOK{};  // int
    std::int64_t m_iGSUID{};  // UidType
    std::int64_t m_iUnitUID{};  // UidType
    std::int32_t m_iReason{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iGSUID)>>::put(w, m_iGSUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iReason)>>::put(w, m_iReason);
    }

    static KDBE_STOP_SALE_PSHOP_AGENCY_ACK read_from(Reader& r) {
        KDBE_STOP_SALE_PSHOP_AGENCY_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iGSUID = Codec<std::remove_cvref_t<decltype(v.m_iGSUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iReason = Codec<std::remove_cvref_t<decltype(v.m_iReason)>>::get(r);
        return v;
    }
};

struct KDBE_STOP_SALE_PSHOP_AGENCY_REQ {
    std::int64_t m_iGSUID{};  // UidType
    std::int64_t m_iUnitUID{};  // UidType
    std::int32_t m_iReason{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iGSUID)>>::put(w, m_iGSUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iReason)>>::put(w, m_iReason);
    }

    static KDBE_STOP_SALE_PSHOP_AGENCY_REQ read_from(Reader& r) {
        KDBE_STOP_SALE_PSHOP_AGENCY_REQ v;
        v.m_iGSUID = Codec<std::remove_cvref_t<decltype(v.m_iGSUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iReason = Codec<std::remove_cvref_t<decltype(v.m_iReason)>>::get(r);
        return v;
    }
};

struct KDBE_SYNC_ED_REQ {
    std::int64_t   m_iFromUnitUID{};  // UidType
    bool           m_bIsGmMessage{};
    std::u16string m_wstrToNickName{};  // std::wstring
    std::u16string m_wstrTitle{};  // std::wstring
    std::u16string m_wstrMessage{};  // std::wstring
    std::int32_t   m_iED{};  // int
    std::int64_t   m_iItemUID{};  // UidType
    std::int32_t   m_iQuantity{};  // int
    std::int32_t   m_iChangeED{};  // int
    std::int32_t   m_iSendLetterCost{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iFromUnitUID)>>::put(w, m_iFromUnitUID);
        Codec<std::remove_cvref_t<decltype(m_bIsGmMessage)>>::put(w, m_bIsGmMessage);
        Codec<std::remove_cvref_t<decltype(m_wstrToNickName)>>::put(w, m_wstrToNickName);
        Codec<std::remove_cvref_t<decltype(m_wstrTitle)>>::put(w, m_wstrTitle);
        Codec<std::remove_cvref_t<decltype(m_wstrMessage)>>::put(w, m_wstrMessage);
        Codec<std::remove_cvref_t<decltype(m_iED)>>::put(w, m_iED);
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_iQuantity)>>::put(w, m_iQuantity);
        Codec<std::remove_cvref_t<decltype(m_iChangeED)>>::put(w, m_iChangeED);
        Codec<std::remove_cvref_t<decltype(m_iSendLetterCost)>>::put(w, m_iSendLetterCost);
    }

    static KDBE_SYNC_ED_REQ read_from(Reader& r) {
        KDBE_SYNC_ED_REQ v;
        v.m_iFromUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iFromUnitUID)>>::get(r);
        v.m_bIsGmMessage = Codec<std::remove_cvref_t<decltype(v.m_bIsGmMessage)>>::get(r);
        v.m_wstrToNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrToNickName)>>::get(r);
        v.m_wstrTitle = Codec<std::remove_cvref_t<decltype(v.m_wstrTitle)>>::get(r);
        v.m_wstrMessage = Codec<std::remove_cvref_t<decltype(v.m_wstrMessage)>>::get(r);
        v.m_iED = Codec<std::remove_cvref_t<decltype(v.m_iED)>>::get(r);
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_iQuantity = Codec<std::remove_cvref_t<decltype(v.m_iQuantity)>>::get(r);
        v.m_iChangeED = Codec<std::remove_cvref_t<decltype(v.m_iChangeED)>>::get(r);
        v.m_iSendLetterCost = Codec<std::remove_cvref_t<decltype(v.m_iSendLetterCost)>>::get(r);
        return v;
    }
};

struct KDBE_SYNC_ED_ACK {
    std::int32_t     m_iOK{};  // int
    KDBE_SYNC_ED_REQ m_kInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_kInfo)>>::put(w, m_kInfo);
    }

    static KDBE_SYNC_ED_ACK read_from(Reader& r) {
        KDBE_SYNC_ED_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_kInfo = Codec<std::remove_cvref_t<decltype(v.m_kInfo)>>::get(r);
        return v;
    }
};

struct KDBE_TICK_PERFORMANCE_LOG_NOT {
    std::int64_t   m_iServerUID{};  // UidType
    std::int32_t   m_iServerType{};  // int
    std::uint32_t  m_dwMaxTickCount{};  // DWORD
    std::u16string m_wstrMaxTickRegDate{};  // std::wstring
    std::u16string m_wstrRegDate{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iServerUID)>>::put(w, m_iServerUID);
        Codec<std::remove_cvref_t<decltype(m_iServerType)>>::put(w, m_iServerType);
        Codec<std::remove_cvref_t<decltype(m_dwMaxTickCount)>>::put(w, m_dwMaxTickCount);
        Codec<std::remove_cvref_t<decltype(m_wstrMaxTickRegDate)>>::put(w, m_wstrMaxTickRegDate);
        Codec<std::remove_cvref_t<decltype(m_wstrRegDate)>>::put(w, m_wstrRegDate);
    }

    static KDBE_TICK_PERFORMANCE_LOG_NOT read_from(Reader& r) {
        KDBE_TICK_PERFORMANCE_LOG_NOT v;
        v.m_iServerUID = Codec<std::remove_cvref_t<decltype(v.m_iServerUID)>>::get(r);
        v.m_iServerType = Codec<std::remove_cvref_t<decltype(v.m_iServerType)>>::get(r);
        v.m_dwMaxTickCount = Codec<std::remove_cvref_t<decltype(v.m_dwMaxTickCount)>>::get(r);
        v.m_wstrMaxTickRegDate = Codec<std::remove_cvref_t<decltype(v.m_wstrMaxTickRegDate)>>::get(r);
        v.m_wstrRegDate = Codec<std::remove_cvref_t<decltype(v.m_wstrRegDate)>>::get(r);
        return v;
    }
};

struct KDBE_TIME_ENCHANT_EVENT_INFO_UPDATE_ACK {
    std::int64_t  m_iGSUID{};  // UidType
    std::int64_t  m_iUnitUID{};  // UidType
    std::int32_t  m_iAfterWorkStorageKey{};  // int
    std::uint16_t m_usEventID{};  // unsigned short
    char          m_cEnchantLevel{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iGSUID)>>::put(w, m_iGSUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iAfterWorkStorageKey)>>::put(w, m_iAfterWorkStorageKey);
        Codec<std::remove_cvref_t<decltype(m_usEventID)>>::put(w, m_usEventID);
        Codec<std::remove_cvref_t<decltype(m_cEnchantLevel)>>::put(w, m_cEnchantLevel);
    }

    static KDBE_TIME_ENCHANT_EVENT_INFO_UPDATE_ACK read_from(Reader& r) {
        KDBE_TIME_ENCHANT_EVENT_INFO_UPDATE_ACK v;
        v.m_iGSUID = Codec<std::remove_cvref_t<decltype(v.m_iGSUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iAfterWorkStorageKey = Codec<std::remove_cvref_t<decltype(v.m_iAfterWorkStorageKey)>>::get(r);
        v.m_usEventID = Codec<std::remove_cvref_t<decltype(v.m_usEventID)>>::get(r);
        v.m_cEnchantLevel = Codec<std::remove_cvref_t<decltype(v.m_cEnchantLevel)>>::get(r);
        return v;
    }
};

struct KDBE_UDP_CHECK_KICK_USER_NOT {
    std::int64_t m_iRoomUID{};  // UidType
    std::int32_t m_iPVPChannelClass{};  // int
    std::int32_t m_iMatchType{};  // int
    std::int64_t m_iUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iRoomUID)>>::put(w, m_iRoomUID);
        Codec<std::remove_cvref_t<decltype(m_iPVPChannelClass)>>::put(w, m_iPVPChannelClass);
        Codec<std::remove_cvref_t<decltype(m_iMatchType)>>::put(w, m_iMatchType);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
    }

    static KDBE_UDP_CHECK_KICK_USER_NOT read_from(Reader& r) {
        KDBE_UDP_CHECK_KICK_USER_NOT v;
        v.m_iRoomUID = Codec<std::remove_cvref_t<decltype(v.m_iRoomUID)>>::get(r);
        v.m_iPVPChannelClass = Codec<std::remove_cvref_t<decltype(v.m_iPVPChannelClass)>>::get(r);
        v.m_iMatchType = Codec<std::remove_cvref_t<decltype(v.m_iMatchType)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        return v;
    }
};

struct KDBE_UNBLOCK_FRIEND_REQ {
    std::int64_t m_iUnitUID{};  // UidType
    std::int64_t m_iFriendUnitUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iFriendUnitUID)>>::put(w, m_iFriendUnitUID);
    }

    static KDBE_UNBLOCK_FRIEND_REQ read_from(Reader& r) {
        KDBE_UNBLOCK_FRIEND_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iFriendUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iFriendUnitUID)>>::get(r);
        return v;
    }
};

struct KDBE_UNLIMITED_SECOND_CHANGE_JOB_NOT {
    std::int32_t m_iUnitUID{};  // int
    std::int32_t m_iOldUnitClass{};  // int
    std::int32_t m_iNewUnitClass{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iOldUnitClass)>>::put(w, m_iOldUnitClass);
        Codec<std::remove_cvref_t<decltype(m_iNewUnitClass)>>::put(w, m_iNewUnitClass);
    }

    static KDBE_UNLIMITED_SECOND_CHANGE_JOB_NOT read_from(Reader& r) {
        KDBE_UNLIMITED_SECOND_CHANGE_JOB_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iOldUnitClass = Codec<std::remove_cvref_t<decltype(v.m_iOldUnitClass)>>::get(r);
        v.m_iNewUnitClass = Codec<std::remove_cvref_t<decltype(v.m_iNewUnitClass)>>::get(r);
        return v;
    }
};

struct KDBE_UNSEAL_SKILL_ACK {
    std::int32_t m_iOK{};  // int
    std::int32_t m_iSkillID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iSkillID)>>::put(w, m_iSkillID);
    }

    static KDBE_UNSEAL_SKILL_ACK read_from(Reader& r) {
        KDBE_UNSEAL_SKILL_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iSkillID = Codec<std::remove_cvref_t<decltype(v.m_iSkillID)>>::get(r);
        return v;
    }
};

struct KDBE_UNSEAL_SKILL_REQ {
    std::int64_t m_iUnitUID{};  // UidType
    std::int32_t m_iSkillID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iSkillID)>>::put(w, m_iSkillID);
    }

    static KDBE_UNSEAL_SKILL_REQ read_from(Reader& r) {
        KDBE_UNSEAL_SKILL_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iSkillID = Codec<std::remove_cvref_t<decltype(v.m_iSkillID)>>::get(r);
        return v;
    }
};

struct KDBE_UPDATE_EVENT_MONSTER_KILL_INFO_NOT {
    std::int32_t m_iEventUID{};  // int
    std::int64_t m_iKillUnitUID{};  // UidType
    std::int64_t m_tNpcDieDate{};  // __int64

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iEventUID)>>::put(w, m_iEventUID);
        Codec<std::remove_cvref_t<decltype(m_iKillUnitUID)>>::put(w, m_iKillUnitUID);
        Codec<std::remove_cvref_t<decltype(m_tNpcDieDate)>>::put(w, m_tNpcDieDate);
    }

    static KDBE_UPDATE_EVENT_MONSTER_KILL_INFO_NOT read_from(Reader& r) {
        KDBE_UPDATE_EVENT_MONSTER_KILL_INFO_NOT v;
        v.m_iEventUID = Codec<std::remove_cvref_t<decltype(v.m_iEventUID)>>::get(r);
        v.m_iKillUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iKillUnitUID)>>::get(r);
        v.m_tNpcDieDate = Codec<std::remove_cvref_t<decltype(v.m_tNpcDieDate)>>::get(r);
        return v;
    }
};

struct KDBE_UPDATE_EVENT_TIME_ACK {
    std::int32_t           m_iOK{};  // int
    std::set<std::int32_t> m_setConnectTimeEvent{};  // std::set< int >
    std::set<std::int32_t> m_setCumulativeTimeEvent{};  // std::set< int >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_setConnectTimeEvent)>>::put(w, m_setConnectTimeEvent);
        Codec<std::remove_cvref_t<decltype(m_setCumulativeTimeEvent)>>::put(w, m_setCumulativeTimeEvent);
    }

    static KDBE_UPDATE_EVENT_TIME_ACK read_from(Reader& r) {
        KDBE_UPDATE_EVENT_TIME_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_setConnectTimeEvent = Codec<std::remove_cvref_t<decltype(v.m_setConnectTimeEvent)>>::get(r);
        v.m_setCumulativeTimeEvent = Codec<std::remove_cvref_t<decltype(v.m_setCumulativeTimeEvent)>>::get(r);
        return v;
    }
};

struct KDBE_UPDATE_EVENT_TIME_REQ {
    std::int64_t                          m_iUnitUID{};  // UidType
    std::vector<KConnectTimeEventInfo>    m_vecConnectTimeEvent{};  // std::vector< KConnectTimeEventInfo >
    std::vector<KCumulativeTimeEventInfo> m_vecCumulativeTimeEvent{};  // std::vector< KCumulativeTimeEventInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_vecConnectTimeEvent)>>::put(w, m_vecConnectTimeEvent);
        Codec<std::remove_cvref_t<decltype(m_vecCumulativeTimeEvent)>>::put(w, m_vecCumulativeTimeEvent);
    }

    static KDBE_UPDATE_EVENT_TIME_REQ read_from(Reader& r) {
        KDBE_UPDATE_EVENT_TIME_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_vecConnectTimeEvent = Codec<std::remove_cvref_t<decltype(v.m_vecConnectTimeEvent)>>::get(r);
        v.m_vecCumulativeTimeEvent = Codec<std::remove_cvref_t<decltype(v.m_vecCumulativeTimeEvent)>>::get(r);
        return v;
    }
};

struct KDBE_UPDATE_GLOBAL_MISSION_INFO_REQ {
    std::u16string m_wstrGlobalMissionDay{};  // std::wstring
    std::int32_t   m_iNowCount{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrGlobalMissionDay)>>::put(w, m_wstrGlobalMissionDay);
        Codec<std::remove_cvref_t<decltype(m_iNowCount)>>::put(w, m_iNowCount);
    }

    static KDBE_UPDATE_GLOBAL_MISSION_INFO_REQ read_from(Reader& r) {
        KDBE_UPDATE_GLOBAL_MISSION_INFO_REQ v;
        v.m_wstrGlobalMissionDay = Codec<std::remove_cvref_t<decltype(v.m_wstrGlobalMissionDay)>>::get(r);
        v.m_iNowCount = Codec<std::remove_cvref_t<decltype(v.m_iNowCount)>>::get(r);
        return v;
    }
};

struct KDBE_UPDATE_GUILD_EXP_ACK {
    std::int32_t m_iOK{};  // int
    std::int64_t m_iUnitUID{};  // UidType
    std::int32_t m_iGuildUID{};  // int
    std::int32_t m_iGuildEXP{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iGuildUID)>>::put(w, m_iGuildUID);
        Codec<std::remove_cvref_t<decltype(m_iGuildEXP)>>::put(w, m_iGuildEXP);
    }

    static KDBE_UPDATE_GUILD_EXP_ACK read_from(Reader& r) {
        KDBE_UPDATE_GUILD_EXP_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iGuildUID = Codec<std::remove_cvref_t<decltype(v.m_iGuildUID)>>::get(r);
        v.m_iGuildEXP = Codec<std::remove_cvref_t<decltype(v.m_iGuildEXP)>>::get(r);
        return v;
    }
};

struct KDBE_UPDATE_GUILD_EXP_REQ {
    std::int64_t m_iUnitUID{};  // UidType
    std::int32_t m_iGuildUID{};  // int
    std::int32_t m_iHonorPoint{};  // int
    std::int32_t m_iGuildEXP{};  // int
    std::int32_t m_iGuildLimitEXP{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iGuildUID)>>::put(w, m_iGuildUID);
        Codec<std::remove_cvref_t<decltype(m_iHonorPoint)>>::put(w, m_iHonorPoint);
        Codec<std::remove_cvref_t<decltype(m_iGuildEXP)>>::put(w, m_iGuildEXP);
        Codec<std::remove_cvref_t<decltype(m_iGuildLimitEXP)>>::put(w, m_iGuildLimitEXP);
    }

    static KDBE_UPDATE_GUILD_EXP_REQ read_from(Reader& r) {
        KDBE_UPDATE_GUILD_EXP_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iGuildUID = Codec<std::remove_cvref_t<decltype(v.m_iGuildUID)>>::get(r);
        v.m_iHonorPoint = Codec<std::remove_cvref_t<decltype(v.m_iHonorPoint)>>::get(r);
        v.m_iGuildEXP = Codec<std::remove_cvref_t<decltype(v.m_iGuildEXP)>>::get(r);
        v.m_iGuildLimitEXP = Codec<std::remove_cvref_t<decltype(v.m_iGuildLimitEXP)>>::get(r);
        return v;
    }
};

