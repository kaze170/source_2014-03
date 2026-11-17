// ---------------------------------------------------------------------------
// packets_6.inc - GENERATED FILE, DO NOT EDIT BY HAND.
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

struct KDBE_TRADE_COMPLETE_REQ {
    std::int64_t                    m_iUnitUID{};  // UidType
    std::int32_t                    m_iED{};  // int
    KItemQuantityUpdate             m_kQuantityUpdate{};
    std::vector<KInventoryItemInfo> m_vecInventorySlotInfo{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iED)>>::put(w, m_iED);
        Codec<std::remove_cvref_t<decltype(m_kQuantityUpdate)>>::put(w, m_kQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecInventorySlotInfo)>>::put(w, m_vecInventorySlotInfo);
    }

    static KDBE_TRADE_COMPLETE_REQ read_from(Reader& r) {
        KDBE_TRADE_COMPLETE_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iED = Codec<std::remove_cvref_t<decltype(v.m_iED)>>::get(r);
        v.m_kQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kQuantityUpdate)>>::get(r);
        v.m_vecInventorySlotInfo = Codec<std::remove_cvref_t<decltype(v.m_vecInventorySlotInfo)>>::get(r);
        return v;
    }
};

struct KDBE_USE_FINALITY_SKILL_ACK {
    std::int32_t                    m_iOK{};  // int
    KItemQuantityUpdate             m_kItemQuantityUpdate{};
    std::vector<KInventoryItemInfo> m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
    }

    static KDBE_USE_FINALITY_SKILL_ACK read_from(Reader& r) {
        KDBE_USE_FINALITY_SKILL_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        return v;
    }
};

struct KDBE_USE_FINALITY_SKILL_REQ {
    std::int64_t                    m_iUnitUID{};  // UidType
    KItemQuantityUpdate             m_kItemQuantityUpdate{};
    std::vector<KInventoryItemInfo> m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >
    std::vector<KItemInfo>          m_vecItemInfo{};  // std::vector< KItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
        Codec<std::remove_cvref_t<decltype(m_vecItemInfo)>>::put(w, m_vecItemInfo);
    }

    static KDBE_USE_FINALITY_SKILL_REQ read_from(Reader& r) {
        KDBE_USE_FINALITY_SKILL_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        v.m_vecItemInfo = Codec<std::remove_cvref_t<decltype(v.m_vecItemInfo)>>::get(r);
        return v;
    }
};

struct KDBE_USE_ITEM_IN_INVENTORY_ACK {
    std::int32_t                    m_iOK{};  // int
    std::int32_t                    m_iUsedItemID{};  // int
    std::int64_t                    m_iItemUID{};  // UidType
    std::int32_t                    m_iWarpPointMapID{};  // int
    std::int32_t                    m_iED{};  // int
    KItemQuantityUpdate             m_kItemQuantityUpdate{};
    std::vector<KInventoryItemInfo> m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >
    std::int64_t                    m_iTempCode{};  // __int64

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iUsedItemID)>>::put(w, m_iUsedItemID);
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_iWarpPointMapID)>>::put(w, m_iWarpPointMapID);
        Codec<std::remove_cvref_t<decltype(m_iED)>>::put(w, m_iED);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
        Codec<std::remove_cvref_t<decltype(m_iTempCode)>>::put(w, m_iTempCode);
    }

    static KDBE_USE_ITEM_IN_INVENTORY_ACK read_from(Reader& r) {
        KDBE_USE_ITEM_IN_INVENTORY_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iUsedItemID = Codec<std::remove_cvref_t<decltype(v.m_iUsedItemID)>>::get(r);
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_iWarpPointMapID = Codec<std::remove_cvref_t<decltype(v.m_iWarpPointMapID)>>::get(r);
        v.m_iED = Codec<std::remove_cvref_t<decltype(v.m_iED)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        v.m_iTempCode = Codec<std::remove_cvref_t<decltype(v.m_iTempCode)>>::get(r);
        return v;
    }
};

struct KDBE_USE_ITEM_IN_INVENTORY_REQ {
    std::int64_t                    m_iUnitUID{};  // UidType
    std::int32_t                    m_iUsedItemID{};  // int
    std::int64_t                    m_iItemUID{};  // UidType
    std::int32_t                    m_iWarpPointMapID{};  // int
    std::int32_t                    m_iED{};  // int
    KItemQuantityUpdate             m_kItemQuantityUpdate{};
    std::vector<KInventoryItemInfo> m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >
    std::int64_t                    m_iTempCode{};  // __int64

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iUsedItemID)>>::put(w, m_iUsedItemID);
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_iWarpPointMapID)>>::put(w, m_iWarpPointMapID);
        Codec<std::remove_cvref_t<decltype(m_iED)>>::put(w, m_iED);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
        Codec<std::remove_cvref_t<decltype(m_iTempCode)>>::put(w, m_iTempCode);
    }

    static KDBE_USE_ITEM_IN_INVENTORY_REQ read_from(Reader& r) {
        KDBE_USE_ITEM_IN_INVENTORY_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iUsedItemID = Codec<std::remove_cvref_t<decltype(v.m_iUsedItemID)>>::get(r);
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_iWarpPointMapID = Codec<std::remove_cvref_t<decltype(v.m_iWarpPointMapID)>>::get(r);
        v.m_iED = Codec<std::remove_cvref_t<decltype(v.m_iED)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        v.m_iTempCode = Codec<std::remove_cvref_t<decltype(v.m_iTempCode)>>::get(r);
        return v;
    }
};

struct KDBE_WEDDING_PROPOSE_ACK {
    std::int32_t                    m_iOK{};  // int
    std::int32_t                    m_iWeddingUID{};  // int
    std::int64_t                    m_iManUID{};  // UidType
    std::int64_t                    m_iGirlUID{};  // UidType
    char                            m_cWeddingHallType{};
    char                            m_cOfficiantNPC{};
    std::u16string                  m_wstrWeddingDate{};  // std::wstring
    std::u16string                  m_wstrWeddingMsg{};  // std::wstring
    KItemQuantityUpdate             m_kItemQuantityUpdate{};
    std::vector<KInventoryItemInfo> m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iWeddingUID)>>::put(w, m_iWeddingUID);
        Codec<std::remove_cvref_t<decltype(m_iManUID)>>::put(w, m_iManUID);
        Codec<std::remove_cvref_t<decltype(m_iGirlUID)>>::put(w, m_iGirlUID);
        Codec<std::remove_cvref_t<decltype(m_cWeddingHallType)>>::put(w, m_cWeddingHallType);
        Codec<std::remove_cvref_t<decltype(m_cOfficiantNPC)>>::put(w, m_cOfficiantNPC);
        Codec<std::remove_cvref_t<decltype(m_wstrWeddingDate)>>::put(w, m_wstrWeddingDate);
        Codec<std::remove_cvref_t<decltype(m_wstrWeddingMsg)>>::put(w, m_wstrWeddingMsg);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
    }

    static KDBE_WEDDING_PROPOSE_ACK read_from(Reader& r) {
        KDBE_WEDDING_PROPOSE_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iWeddingUID = Codec<std::remove_cvref_t<decltype(v.m_iWeddingUID)>>::get(r);
        v.m_iManUID = Codec<std::remove_cvref_t<decltype(v.m_iManUID)>>::get(r);
        v.m_iGirlUID = Codec<std::remove_cvref_t<decltype(v.m_iGirlUID)>>::get(r);
        v.m_cWeddingHallType = Codec<std::remove_cvref_t<decltype(v.m_cWeddingHallType)>>::get(r);
        v.m_cOfficiantNPC = Codec<std::remove_cvref_t<decltype(v.m_cOfficiantNPC)>>::get(r);
        v.m_wstrWeddingDate = Codec<std::remove_cvref_t<decltype(v.m_wstrWeddingDate)>>::get(r);
        v.m_wstrWeddingMsg = Codec<std::remove_cvref_t<decltype(v.m_wstrWeddingMsg)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        return v;
    }
};

struct KDBE_WEDDING_PROPOSE_REQ {
    std::int64_t                    m_iUnitUID{};  // UidType
    std::int64_t                    m_iManUID{};  // UidType
    std::int64_t                    m_iGirlUID{};  // UidType
    char                            m_cWeddingHallType{};
    char                            m_cOfficiantNPC{};
    std::u16string                  m_wstrWeddingDate{};  // std::wstring
    std::u16string                  m_wstrWeddingMsg{};  // std::wstring
    std::u16string                  m_wstrWeddingNickName{};  // std::wstring
    std::vector<std::int64_t>       m_vecInviteUnitList{};  // std::vector<UidType>
    KItemQuantityUpdate             m_kItemQuantityUpdate{};
    std::vector<KInventoryItemInfo> m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iManUID)>>::put(w, m_iManUID);
        Codec<std::remove_cvref_t<decltype(m_iGirlUID)>>::put(w, m_iGirlUID);
        Codec<std::remove_cvref_t<decltype(m_cWeddingHallType)>>::put(w, m_cWeddingHallType);
        Codec<std::remove_cvref_t<decltype(m_cOfficiantNPC)>>::put(w, m_cOfficiantNPC);
        Codec<std::remove_cvref_t<decltype(m_wstrWeddingDate)>>::put(w, m_wstrWeddingDate);
        Codec<std::remove_cvref_t<decltype(m_wstrWeddingMsg)>>::put(w, m_wstrWeddingMsg);
        Codec<std::remove_cvref_t<decltype(m_wstrWeddingNickName)>>::put(w, m_wstrWeddingNickName);
        Codec<std::remove_cvref_t<decltype(m_vecInviteUnitList)>>::put(w, m_vecInviteUnitList);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
    }

    static KDBE_WEDDING_PROPOSE_REQ read_from(Reader& r) {
        KDBE_WEDDING_PROPOSE_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iManUID = Codec<std::remove_cvref_t<decltype(v.m_iManUID)>>::get(r);
        v.m_iGirlUID = Codec<std::remove_cvref_t<decltype(v.m_iGirlUID)>>::get(r);
        v.m_cWeddingHallType = Codec<std::remove_cvref_t<decltype(v.m_cWeddingHallType)>>::get(r);
        v.m_cOfficiantNPC = Codec<std::remove_cvref_t<decltype(v.m_cOfficiantNPC)>>::get(r);
        v.m_wstrWeddingDate = Codec<std::remove_cvref_t<decltype(v.m_wstrWeddingDate)>>::get(r);
        v.m_wstrWeddingMsg = Codec<std::remove_cvref_t<decltype(v.m_wstrWeddingMsg)>>::get(r);
        v.m_wstrWeddingNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrWeddingNickName)>>::get(r);
        v.m_vecInviteUnitList = Codec<std::remove_cvref_t<decltype(v.m_vecInviteUnitList)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        return v;
    }
};

struct KERM_BUY_PERSONAL_SHOP_ITEM_COMPLETE_NOT {
    std::int64_t           m_iSellUnitUID{};  // UidType
    std::int32_t           m_iSellUnitEDIN{};  // int
    std::int32_t           m_iSellCommissionED{};  // int
    KInventoryItemInfo     m_kSellIntemInfoOUT{};
    std::int64_t           m_iPriceUnitUID{};  // UidType
    std::int32_t           m_iPriceUnitEDOUT{};  // int
    std::int32_t           m_iPriceCommissionED{};  // int
    KInventoryItemInfo     m_kPriceIntemInfoIN{};
    bool                   m_bIsSquare{};
    KItemQuantityUpdate    m_kItemQuantityUpdateTemp{};
    std::vector<KItemInfo> m_vecItemInfoTemp{};  // std::vector< KItemInfo >
    std::int32_t           m_iEDTemp{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iSellUnitUID)>>::put(w, m_iSellUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iSellUnitEDIN)>>::put(w, m_iSellUnitEDIN);
        Codec<std::remove_cvref_t<decltype(m_iSellCommissionED)>>::put(w, m_iSellCommissionED);
        Codec<std::remove_cvref_t<decltype(m_kSellIntemInfoOUT)>>::put(w, m_kSellIntemInfoOUT);
        Codec<std::remove_cvref_t<decltype(m_iPriceUnitUID)>>::put(w, m_iPriceUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iPriceUnitEDOUT)>>::put(w, m_iPriceUnitEDOUT);
        Codec<std::remove_cvref_t<decltype(m_iPriceCommissionED)>>::put(w, m_iPriceCommissionED);
        Codec<std::remove_cvref_t<decltype(m_kPriceIntemInfoIN)>>::put(w, m_kPriceIntemInfoIN);
        Codec<std::remove_cvref_t<decltype(m_bIsSquare)>>::put(w, m_bIsSquare);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdateTemp)>>::put(w, m_kItemQuantityUpdateTemp);
        Codec<std::remove_cvref_t<decltype(m_vecItemInfoTemp)>>::put(w, m_vecItemInfoTemp);
        Codec<std::remove_cvref_t<decltype(m_iEDTemp)>>::put(w, m_iEDTemp);
    }

    static KERM_BUY_PERSONAL_SHOP_ITEM_COMPLETE_NOT read_from(Reader& r) {
        KERM_BUY_PERSONAL_SHOP_ITEM_COMPLETE_NOT v;
        v.m_iSellUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iSellUnitUID)>>::get(r);
        v.m_iSellUnitEDIN = Codec<std::remove_cvref_t<decltype(v.m_iSellUnitEDIN)>>::get(r);
        v.m_iSellCommissionED = Codec<std::remove_cvref_t<decltype(v.m_iSellCommissionED)>>::get(r);
        v.m_kSellIntemInfoOUT = Codec<std::remove_cvref_t<decltype(v.m_kSellIntemInfoOUT)>>::get(r);
        v.m_iPriceUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iPriceUnitUID)>>::get(r);
        v.m_iPriceUnitEDOUT = Codec<std::remove_cvref_t<decltype(v.m_iPriceUnitEDOUT)>>::get(r);
        v.m_iPriceCommissionED = Codec<std::remove_cvref_t<decltype(v.m_iPriceCommissionED)>>::get(r);
        v.m_kPriceIntemInfoIN = Codec<std::remove_cvref_t<decltype(v.m_kPriceIntemInfoIN)>>::get(r);
        v.m_bIsSquare = Codec<std::remove_cvref_t<decltype(v.m_bIsSquare)>>::get(r);
        v.m_kItemQuantityUpdateTemp = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdateTemp)>>::get(r);
        v.m_vecItemInfoTemp = Codec<std::remove_cvref_t<decltype(v.m_vecItemInfoTemp)>>::get(r);
        v.m_iEDTemp = Codec<std::remove_cvref_t<decltype(v.m_iEDTemp)>>::get(r);
        return v;
    }
};

struct KERM_BUY_PERSONAL_SHOP_ITEM_REQ {
    std::int64_t           m_iBuyItemUID{};  // UidType
    std::int32_t           m_iBuyItemID{};  // int
    std::int32_t           m_iBuyQuantity{};  // int
    std::int32_t           m_iBuyUnitNowED{};  // int
    KItemQuantityUpdate    m_kItemQuantityUpdateTemp{};
    std::vector<KItemInfo> m_vecItemInfoTemp{};  // std::vector< KItemInfo >
    std::int32_t           m_iEDTemp{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iBuyItemUID)>>::put(w, m_iBuyItemUID);
        Codec<std::remove_cvref_t<decltype(m_iBuyItemID)>>::put(w, m_iBuyItemID);
        Codec<std::remove_cvref_t<decltype(m_iBuyQuantity)>>::put(w, m_iBuyQuantity);
        Codec<std::remove_cvref_t<decltype(m_iBuyUnitNowED)>>::put(w, m_iBuyUnitNowED);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdateTemp)>>::put(w, m_kItemQuantityUpdateTemp);
        Codec<std::remove_cvref_t<decltype(m_vecItemInfoTemp)>>::put(w, m_vecItemInfoTemp);
        Codec<std::remove_cvref_t<decltype(m_iEDTemp)>>::put(w, m_iEDTemp);
    }

    static KERM_BUY_PERSONAL_SHOP_ITEM_REQ read_from(Reader& r) {
        KERM_BUY_PERSONAL_SHOP_ITEM_REQ v;
        v.m_iBuyItemUID = Codec<std::remove_cvref_t<decltype(v.m_iBuyItemUID)>>::get(r);
        v.m_iBuyItemID = Codec<std::remove_cvref_t<decltype(v.m_iBuyItemID)>>::get(r);
        v.m_iBuyQuantity = Codec<std::remove_cvref_t<decltype(v.m_iBuyQuantity)>>::get(r);
        v.m_iBuyUnitNowED = Codec<std::remove_cvref_t<decltype(v.m_iBuyUnitNowED)>>::get(r);
        v.m_kItemQuantityUpdateTemp = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdateTemp)>>::get(r);
        v.m_vecItemInfoTemp = Codec<std::remove_cvref_t<decltype(v.m_vecItemInfoTemp)>>::get(r);
        v.m_iEDTemp = Codec<std::remove_cvref_t<decltype(v.m_iEDTemp)>>::get(r);
        return v;
    }
};

struct KERM_CONFIRM_BUY_PERSONAL_SHOP_ITEM_ACK {
    std::int32_t           m_iOK{};  // int
    std::int64_t           m_iBuyUnitUID{};  // UidType
    std::int64_t           m_iBuyItemUID{};  // UidType
    std::int32_t           m_iBuyQuantity{};  // int
    KItemQuantityUpdate    m_kItemQuantityUpdateTemp{};
    std::vector<KItemInfo> m_vecItemInfoTemp{};  // std::vector< KItemInfo >
    std::int32_t           m_iEDTemp{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iBuyUnitUID)>>::put(w, m_iBuyUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iBuyItemUID)>>::put(w, m_iBuyItemUID);
        Codec<std::remove_cvref_t<decltype(m_iBuyQuantity)>>::put(w, m_iBuyQuantity);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdateTemp)>>::put(w, m_kItemQuantityUpdateTemp);
        Codec<std::remove_cvref_t<decltype(m_vecItemInfoTemp)>>::put(w, m_vecItemInfoTemp);
        Codec<std::remove_cvref_t<decltype(m_iEDTemp)>>::put(w, m_iEDTemp);
    }

    static KERM_CONFIRM_BUY_PERSONAL_SHOP_ITEM_ACK read_from(Reader& r) {
        KERM_CONFIRM_BUY_PERSONAL_SHOP_ITEM_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iBuyUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iBuyUnitUID)>>::get(r);
        v.m_iBuyItemUID = Codec<std::remove_cvref_t<decltype(v.m_iBuyItemUID)>>::get(r);
        v.m_iBuyQuantity = Codec<std::remove_cvref_t<decltype(v.m_iBuyQuantity)>>::get(r);
        v.m_kItemQuantityUpdateTemp = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdateTemp)>>::get(r);
        v.m_vecItemInfoTemp = Codec<std::remove_cvref_t<decltype(v.m_vecItemInfoTemp)>>::get(r);
        v.m_iEDTemp = Codec<std::remove_cvref_t<decltype(v.m_iEDTemp)>>::get(r);
        return v;
    }
};

struct KERM_CONFIRM_TRADE_ACK {
    std::int32_t           m_iOK{};  // int
    KItemQuantityUpdate    m_kItemQuantityUpdateTemp{};
    std::vector<KItemInfo> m_vecItemInfoTemp{};  // std::vector< KItemInfo >
    std::int32_t           m_iEDTemp{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdateTemp)>>::put(w, m_kItemQuantityUpdateTemp);
        Codec<std::remove_cvref_t<decltype(m_vecItemInfoTemp)>>::put(w, m_vecItemInfoTemp);
        Codec<std::remove_cvref_t<decltype(m_iEDTemp)>>::put(w, m_iEDTemp);
    }

    static KERM_CONFIRM_TRADE_ACK read_from(Reader& r) {
        KERM_CONFIRM_TRADE_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_kItemQuantityUpdateTemp = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdateTemp)>>::get(r);
        v.m_vecItemInfoTemp = Codec<std::remove_cvref_t<decltype(v.m_vecItemInfoTemp)>>::get(r);
        v.m_iEDTemp = Codec<std::remove_cvref_t<decltype(v.m_iEDTemp)>>::get(r);
        return v;
    }
};

struct KERM_CONFIRM_TRADE_NOT {
    std::int64_t                    m_iTradeUID{};  // UidType
    std::vector<KInventoryItemInfo> m_vecItemIn{};  // std::vector< KInventoryItemInfo >
    std::vector<KInventoryItemInfo> m_vecItemOut{};  // std::vector< KInventoryItemInfo >
    std::int32_t                    m_iEDIn{};  // int
    std::int32_t                    m_iEDOut{};  // int
    KItemQuantityUpdate             m_kItemQuantityUpdateTemp{};
    std::vector<KItemInfo>          m_vecItemInfoTemp{};  // std::vector< KItemInfo >
    std::int32_t                    m_iEDTemp{};  // int
    std::int64_t                    m_iOtherCID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iTradeUID)>>::put(w, m_iTradeUID);
        Codec<std::remove_cvref_t<decltype(m_vecItemIn)>>::put(w, m_vecItemIn);
        Codec<std::remove_cvref_t<decltype(m_vecItemOut)>>::put(w, m_vecItemOut);
        Codec<std::remove_cvref_t<decltype(m_iEDIn)>>::put(w, m_iEDIn);
        Codec<std::remove_cvref_t<decltype(m_iEDOut)>>::put(w, m_iEDOut);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdateTemp)>>::put(w, m_kItemQuantityUpdateTemp);
        Codec<std::remove_cvref_t<decltype(m_vecItemInfoTemp)>>::put(w, m_vecItemInfoTemp);
        Codec<std::remove_cvref_t<decltype(m_iEDTemp)>>::put(w, m_iEDTemp);
        Codec<std::remove_cvref_t<decltype(m_iOtherCID)>>::put(w, m_iOtherCID);
    }

    static KERM_CONFIRM_TRADE_NOT read_from(Reader& r) {
        KERM_CONFIRM_TRADE_NOT v;
        v.m_iTradeUID = Codec<std::remove_cvref_t<decltype(v.m_iTradeUID)>>::get(r);
        v.m_vecItemIn = Codec<std::remove_cvref_t<decltype(v.m_vecItemIn)>>::get(r);
        v.m_vecItemOut = Codec<std::remove_cvref_t<decltype(v.m_vecItemOut)>>::get(r);
        v.m_iEDIn = Codec<std::remove_cvref_t<decltype(v.m_iEDIn)>>::get(r);
        v.m_iEDOut = Codec<std::remove_cvref_t<decltype(v.m_iEDOut)>>::get(r);
        v.m_kItemQuantityUpdateTemp = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdateTemp)>>::get(r);
        v.m_vecItemInfoTemp = Codec<std::remove_cvref_t<decltype(v.m_vecItemInfoTemp)>>::get(r);
        v.m_iEDTemp = Codec<std::remove_cvref_t<decltype(v.m_iEDTemp)>>::get(r);
        v.m_iOtherCID = Codec<std::remove_cvref_t<decltype(v.m_iOtherCID)>>::get(r);
        return v;
    }
};

struct KERM_DB_UPDATE_TRADE_RESULT_NOT {
    std::int64_t                    m_iUnitUID{};  // UidType
    std::int32_t                    m_iChangedED{};  // int
    std::vector<KInventoryItemInfo> m_vecUpdated{};  // std::vector< KInventoryItemInfo >
    std::vector<KInventoryItemInfo> m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >
    KItemQuantityUpdate             m_kItemQuantityUpdate_OUT{};
    std::vector<KInventoryItemInfo> m_vecInsertDBItem{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iChangedED)>>::put(w, m_iChangedED);
        Codec<std::remove_cvref_t<decltype(m_vecUpdated)>>::put(w, m_vecUpdated);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate_OUT)>>::put(w, m_kItemQuantityUpdate_OUT);
        Codec<std::remove_cvref_t<decltype(m_vecInsertDBItem)>>::put(w, m_vecInsertDBItem);
    }

    static KERM_DB_UPDATE_TRADE_RESULT_NOT read_from(Reader& r) {
        KERM_DB_UPDATE_TRADE_RESULT_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iChangedED = Codec<std::remove_cvref_t<decltype(v.m_iChangedED)>>::get(r);
        v.m_vecUpdated = Codec<std::remove_cvref_t<decltype(v.m_vecUpdated)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        v.m_kItemQuantityUpdate_OUT = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate_OUT)>>::get(r);
        v.m_vecInsertDBItem = Codec<std::remove_cvref_t<decltype(v.m_vecInsertDBItem)>>::get(r);
        return v;
    }
};

struct KDBE_DB_UPDATE_TRADE_RESULT_ACK {
    std::int32_t                                 m_iOK{};  // int
    std::vector<KERM_DB_UPDATE_TRADE_RESULT_NOT> m_vecDBUpdate{};  // std::vector< KERM_DB_UPDATE_TRADE_RESULT_NOT >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_vecDBUpdate)>>::put(w, m_vecDBUpdate);
    }

    static KDBE_DB_UPDATE_TRADE_RESULT_ACK read_from(Reader& r) {
        KDBE_DB_UPDATE_TRADE_RESULT_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_vecDBUpdate = Codec<std::remove_cvref_t<decltype(v.m_vecDBUpdate)>>::get(r);
        return v;
    }
};

struct KDBE_DB_UPDATE_TRADE_RESULT_REQ {
    std::vector<KERM_DB_UPDATE_TRADE_RESULT_NOT> m_vecDBUpdate{};  // std::vector< KERM_DB_UPDATE_TRADE_RESULT_NOT >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecDBUpdate)>>::put(w, m_vecDBUpdate);
    }

    static KDBE_DB_UPDATE_TRADE_RESULT_REQ read_from(Reader& r) {
        KDBE_DB_UPDATE_TRADE_RESULT_REQ v;
        v.m_vecDBUpdate = Codec<std::remove_cvref_t<decltype(v.m_vecDBUpdate)>>::get(r);
        return v;
    }
};

struct KJPEventRewardInfo {
    std::int64_t   m_iRID{};  // __int64
    std::u16string m_wstrUserID{};  // std::wstring
    std::int64_t   m_iUserUID{};  // UidType
    std::int64_t   m_iUnitUID{};  // UidType
    std::int32_t   m_iRewardID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iRID)>>::put(w, m_iRID);
        Codec<std::remove_cvref_t<decltype(m_wstrUserID)>>::put(w, m_wstrUserID);
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iRewardID)>>::put(w, m_iRewardID);
    }

    static KJPEventRewardInfo read_from(Reader& r) {
        KJPEventRewardInfo v;
        v.m_iRID = Codec<std::remove_cvref_t<decltype(v.m_iRID)>>::get(r);
        v.m_wstrUserID = Codec<std::remove_cvref_t<decltype(v.m_wstrUserID)>>::get(r);
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iRewardID = Codec<std::remove_cvref_t<decltype(v.m_iRewardID)>>::get(r);
        return v;
    }
};

struct KDBE_CHECK_REWARD_FROM_RELAY_DB_JP_EVENT_ACK {
    std::vector<KJPEventRewardInfo> m_vecRewardInfo{};  // std::vector< KJPEventRewardInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecRewardInfo)>>::put(w, m_vecRewardInfo);
    }

    static KDBE_CHECK_REWARD_FROM_RELAY_DB_JP_EVENT_ACK read_from(Reader& r) {
        KDBE_CHECK_REWARD_FROM_RELAY_DB_JP_EVENT_ACK v;
        v.m_vecRewardInfo = Codec<std::remove_cvref_t<decltype(v.m_vecRewardInfo)>>::get(r);
        return v;
    }
};

struct KEBILL_REWARD_COUPON_JP_EVENT_ACK {
    KJPEventRewardInfo    m_kRewardInfo{};
    KEBILL_USE_COUPON_ACK m_kUseCouponPacket{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kRewardInfo)>>::put(w, m_kRewardInfo);
        Codec<std::remove_cvref_t<decltype(m_kUseCouponPacket)>>::put(w, m_kUseCouponPacket);
    }

    static KEBILL_REWARD_COUPON_JP_EVENT_ACK read_from(Reader& r) {
        KEBILL_REWARD_COUPON_JP_EVENT_ACK v;
        v.m_kRewardInfo = Codec<std::remove_cvref_t<decltype(v.m_kRewardInfo)>>::get(r);
        v.m_kUseCouponPacket = Codec<std::remove_cvref_t<decltype(v.m_kUseCouponPacket)>>::get(r);
        return v;
    }
};

struct KKeyboardMappingInfo {
    std::map<std::int16_t, std::int16_t> m_mapKeyboardMappingInfo{};  // std::map<short, short>

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_mapKeyboardMappingInfo)>>::put(w, m_mapKeyboardMappingInfo);
    }

    static KKeyboardMappingInfo read_from(Reader& r) {
        KKeyboardMappingInfo v;
        v.m_mapKeyboardMappingInfo = Codec<std::remove_cvref_t<decltype(v.m_mapKeyboardMappingInfo)>>::get(r);
        return v;
    }
};

struct KDBE_KEYBOARD_MAPPING_INFO_WRITE_REQ {
    std::int64_t         m_iUserUID{};  // UidType
    KKeyboardMappingInfo m_kKeyboardMappingInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
        Codec<std::remove_cvref_t<decltype(m_kKeyboardMappingInfo)>>::put(w, m_kKeyboardMappingInfo);
    }

    static KDBE_KEYBOARD_MAPPING_INFO_WRITE_REQ read_from(Reader& r) {
        KDBE_KEYBOARD_MAPPING_INFO_WRITE_REQ v;
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        v.m_kKeyboardMappingInfo = Codec<std::remove_cvref_t<decltype(v.m_kKeyboardMappingInfo)>>::get(r);
        return v;
    }
};

struct KEGS_KEYBOARD_MAPPING_INFO_NOT {
    std::int32_t         m_iOK{};  // int
    KKeyboardMappingInfo m_kKeyboardMappingInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_kKeyboardMappingInfo)>>::put(w, m_kKeyboardMappingInfo);
    }

    static KEGS_KEYBOARD_MAPPING_INFO_NOT read_from(Reader& r) {
        KEGS_KEYBOARD_MAPPING_INFO_NOT v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_kKeyboardMappingInfo = Codec<std::remove_cvref_t<decltype(v.m_kKeyboardMappingInfo)>>::get(r);
        return v;
    }
};

struct KLastPositionInfo {
    std::int32_t  m_iMapID{};  // int
    std::uint8_t  m_ucLastTouchLineIndex{};  // unsigned char
    std::uint16_t m_usLastPosValue{};  // unsigned short
    bool          m_bIgnoreLastTouch{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iMapID)>>::put(w, m_iMapID);
        Codec<std::remove_cvref_t<decltype(m_ucLastTouchLineIndex)>>::put(w, m_ucLastTouchLineIndex);
        Codec<std::remove_cvref_t<decltype(m_usLastPosValue)>>::put(w, m_usLastPosValue);
        Codec<std::remove_cvref_t<decltype(m_bIgnoreLastTouch)>>::put(w, m_bIgnoreLastTouch);
    }

    static KLastPositionInfo read_from(Reader& r) {
        KLastPositionInfo v;
        v.m_iMapID = Codec<std::remove_cvref_t<decltype(v.m_iMapID)>>::get(r);
        v.m_ucLastTouchLineIndex = Codec<std::remove_cvref_t<decltype(v.m_ucLastTouchLineIndex)>>::get(r);
        v.m_usLastPosValue = Codec<std::remove_cvref_t<decltype(v.m_usLastPosValue)>>::get(r);
        v.m_bIgnoreLastTouch = Codec<std::remove_cvref_t<decltype(v.m_bIgnoreLastTouch)>>::get(r);
        return v;
    }
};

struct KEGS_UPDATE_BATTLE_FIELD_USER_POS_NOT {
    KLastPositionInfo m_kLastPos{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kLastPos)>>::put(w, m_kLastPos);
    }

    static KEGS_UPDATE_BATTLE_FIELD_USER_POS_NOT read_from(Reader& r) {
        KEGS_UPDATE_BATTLE_FIELD_USER_POS_NOT v;
        v.m_kLastPos = Codec<std::remove_cvref_t<decltype(v.m_kLastPos)>>::get(r);
        return v;
    }
};

struct KLogData {
    std::uint8_t                m_byteLogLevel{};  // BYTE
    std::u16string              m_wstrRegDate{};  // std::wstring
    std::u16string              m_wstrFunctionName{};  // std::wstring
    std::vector<std::u16string> m_vecContent{};  // std::vector< std::wstring >
    std::u16string              m_wstrFileName{};  // std::wstring
    std::int32_t                m_iFileLine{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_byteLogLevel)>>::put(w, m_byteLogLevel);
        Codec<std::remove_cvref_t<decltype(m_wstrRegDate)>>::put(w, m_wstrRegDate);
        Codec<std::remove_cvref_t<decltype(m_wstrFunctionName)>>::put(w, m_wstrFunctionName);
        Codec<std::remove_cvref_t<decltype(m_vecContent)>>::put(w, m_vecContent);
        Codec<std::remove_cvref_t<decltype(m_wstrFileName)>>::put(w, m_wstrFileName);
        Codec<std::remove_cvref_t<decltype(m_iFileLine)>>::put(w, m_iFileLine);
    }

    static KLogData read_from(Reader& r) {
        KLogData v;
        v.m_byteLogLevel = Codec<std::remove_cvref_t<decltype(v.m_byteLogLevel)>>::get(r);
        v.m_wstrRegDate = Codec<std::remove_cvref_t<decltype(v.m_wstrRegDate)>>::get(r);
        v.m_wstrFunctionName = Codec<std::remove_cvref_t<decltype(v.m_wstrFunctionName)>>::get(r);
        v.m_vecContent = Codec<std::remove_cvref_t<decltype(v.m_vecContent)>>::get(r);
        v.m_wstrFileName = Codec<std::remove_cvref_t<decltype(v.m_wstrFileName)>>::get(r);
        v.m_iFileLine = Codec<std::remove_cvref_t<decltype(v.m_iFileLine)>>::get(r);
        return v;
    }
};

struct KLogStatInfo {
    std::u16string m_wstrFunctionName{};  // std::wstring
    std::u16string m_wstrFileName{};  // std::wstring
    std::u16string m_wstrLineNum{};  // std::wstring
    std::int32_t   m_iCount{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrFunctionName)>>::put(w, m_wstrFunctionName);
        Codec<std::remove_cvref_t<decltype(m_wstrFileName)>>::put(w, m_wstrFileName);
        Codec<std::remove_cvref_t<decltype(m_wstrLineNum)>>::put(w, m_wstrLineNum);
        Codec<std::remove_cvref_t<decltype(m_iCount)>>::put(w, m_iCount);
    }

    static KLogStatInfo read_from(Reader& r) {
        KLogStatInfo v;
        v.m_wstrFunctionName = Codec<std::remove_cvref_t<decltype(v.m_wstrFunctionName)>>::get(r);
        v.m_wstrFileName = Codec<std::remove_cvref_t<decltype(v.m_wstrFileName)>>::get(r);
        v.m_wstrLineNum = Codec<std::remove_cvref_t<decltype(v.m_wstrLineNum)>>::get(r);
        v.m_iCount = Codec<std::remove_cvref_t<decltype(v.m_iCount)>>::get(r);
        return v;
    }
};

struct KDBE_LOG_STATISTICS_INFO_NOT {
    std::vector<KLogStatInfo> m_vecLogStat{};  // std::vector< KLogStatInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecLogStat)>>::put(w, m_vecLogStat);
    }

    static KDBE_LOG_STATISTICS_INFO_NOT read_from(Reader& r) {
        KDBE_LOG_STATISTICS_INFO_NOT v;
        v.m_vecLogStat = Codec<std::remove_cvref_t<decltype(v.m_vecLogStat)>>::get(r);
        return v;
    }
};

struct KLoginResultInfo {
    std::int32_t              m_iServerGroupID{};  // int
    std::int64_t              m_uiServerUID{};  // UidType
    std::vector<std::int32_t> m_vecResultCount{};  // std::vector<int>

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iServerGroupID)>>::put(w, m_iServerGroupID);
        Codec<std::remove_cvref_t<decltype(m_uiServerUID)>>::put(w, m_uiServerUID);
        Codec<std::remove_cvref_t<decltype(m_vecResultCount)>>::put(w, m_vecResultCount);
    }

    static KLoginResultInfo read_from(Reader& r) {
        KLoginResultInfo v;
        v.m_iServerGroupID = Codec<std::remove_cvref_t<decltype(v.m_iServerGroupID)>>::get(r);
        v.m_uiServerUID = Codec<std::remove_cvref_t<decltype(v.m_uiServerUID)>>::get(r);
        v.m_vecResultCount = Codec<std::remove_cvref_t<decltype(v.m_vecResultCount)>>::get(r);
        return v;
    }
};

struct KMapInfo {
    std::int16_t m_MinX{};  // short
    std::int16_t m_MinY{};  // short
    std::int16_t m_MinZ{};  // short
    std::int16_t m_MaxX{};  // short
    std::int16_t m_MaxY{};  // short
    std::int16_t m_MaxZ{};  // short

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_MinX)>>::put(w, m_MinX);
        Codec<std::remove_cvref_t<decltype(m_MinY)>>::put(w, m_MinY);
        Codec<std::remove_cvref_t<decltype(m_MinZ)>>::put(w, m_MinZ);
        Codec<std::remove_cvref_t<decltype(m_MaxX)>>::put(w, m_MaxX);
        Codec<std::remove_cvref_t<decltype(m_MaxY)>>::put(w, m_MaxY);
        Codec<std::remove_cvref_t<decltype(m_MaxZ)>>::put(w, m_MaxZ);
    }

    static KMapInfo read_from(Reader& r) {
        KMapInfo v;
        v.m_MinX = Codec<std::remove_cvref_t<decltype(v.m_MinX)>>::get(r);
        v.m_MinY = Codec<std::remove_cvref_t<decltype(v.m_MinY)>>::get(r);
        v.m_MinZ = Codec<std::remove_cvref_t<decltype(v.m_MinZ)>>::get(r);
        v.m_MaxX = Codec<std::remove_cvref_t<decltype(v.m_MaxX)>>::get(r);
        v.m_MaxY = Codec<std::remove_cvref_t<decltype(v.m_MaxY)>>::get(r);
        v.m_MaxZ = Codec<std::remove_cvref_t<decltype(v.m_MaxZ)>>::get(r);
        return v;
    }
};

struct KMatchUserInfo {
    std::int64_t   m_iGSUID{};  // UidType
    std::int64_t   m_iUnitUID{};  // UidType
    std::int64_t   m_iPartyUID{};  // UidType
    std::u16string m_wstrNickName{};  // std::wstring
    char           m_cUnitClass{};
    std::uint8_t   m_ucLevel{};  // UCHAR
    char           m_cCurrentRank{};
    std::int32_t   m_iOfficialMatchCnt{};  // int
    std::int32_t   m_iRating{};  // int
    std::int32_t   m_iRPoint{};  // int
    std::int32_t   m_iMatchWaitTime{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iGSUID)>>::put(w, m_iGSUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iPartyUID)>>::put(w, m_iPartyUID);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_cUnitClass)>>::put(w, m_cUnitClass);
        Codec<std::remove_cvref_t<decltype(m_ucLevel)>>::put(w, m_ucLevel);
        Codec<std::remove_cvref_t<decltype(m_cCurrentRank)>>::put(w, m_cCurrentRank);
        Codec<std::remove_cvref_t<decltype(m_iOfficialMatchCnt)>>::put(w, m_iOfficialMatchCnt);
        Codec<std::remove_cvref_t<decltype(m_iRating)>>::put(w, m_iRating);
        Codec<std::remove_cvref_t<decltype(m_iRPoint)>>::put(w, m_iRPoint);
        Codec<std::remove_cvref_t<decltype(m_iMatchWaitTime)>>::put(w, m_iMatchWaitTime);
    }

    static KMatchUserInfo read_from(Reader& r) {
        KMatchUserInfo v;
        v.m_iGSUID = Codec<std::remove_cvref_t<decltype(v.m_iGSUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iPartyUID = Codec<std::remove_cvref_t<decltype(v.m_iPartyUID)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_cUnitClass = Codec<std::remove_cvref_t<decltype(v.m_cUnitClass)>>::get(r);
        v.m_ucLevel = Codec<std::remove_cvref_t<decltype(v.m_ucLevel)>>::get(r);
        v.m_cCurrentRank = Codec<std::remove_cvref_t<decltype(v.m_cCurrentRank)>>::get(r);
        v.m_iOfficialMatchCnt = Codec<std::remove_cvref_t<decltype(v.m_iOfficialMatchCnt)>>::get(r);
        v.m_iRating = Codec<std::remove_cvref_t<decltype(v.m_iRating)>>::get(r);
        v.m_iRPoint = Codec<std::remove_cvref_t<decltype(v.m_iRPoint)>>::get(r);
        v.m_iMatchWaitTime = Codec<std::remove_cvref_t<decltype(v.m_iMatchWaitTime)>>::get(r);
        return v;
    }
};

struct KEGB_REQUEST_MATCH_MAKING_REQ {
    bool                        m_bRegFrontOfList{};
    char                        m_cPvpPlayerCount{};
    char                        m_cPvpGameType{};
    std::vector<KMatchUserInfo> m_vecMatchUserInfo{};  // std::vector< KMatchUserInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bRegFrontOfList)>>::put(w, m_bRegFrontOfList);
        Codec<std::remove_cvref_t<decltype(m_cPvpPlayerCount)>>::put(w, m_cPvpPlayerCount);
        Codec<std::remove_cvref_t<decltype(m_cPvpGameType)>>::put(w, m_cPvpGameType);
        Codec<std::remove_cvref_t<decltype(m_vecMatchUserInfo)>>::put(w, m_vecMatchUserInfo);
    }

    static KEGB_REQUEST_MATCH_MAKING_REQ read_from(Reader& r) {
        KEGB_REQUEST_MATCH_MAKING_REQ v;
        v.m_bRegFrontOfList = Codec<std::remove_cvref_t<decltype(v.m_bRegFrontOfList)>>::get(r);
        v.m_cPvpPlayerCount = Codec<std::remove_cvref_t<decltype(v.m_cPvpPlayerCount)>>::get(r);
        v.m_cPvpGameType = Codec<std::remove_cvref_t<decltype(v.m_cPvpGameType)>>::get(r);
        v.m_vecMatchUserInfo = Codec<std::remove_cvref_t<decltype(v.m_vecMatchUserInfo)>>::get(r);
        return v;
    }
};

struct KEPM_GET_PARTY_INFO_FOR_MATCH_MAKING_ACK {
    std::int32_t                m_iOK{};  // int
    bool                        m_bRegFrontOfList{};
    char                        m_cPvpPlayerCount{};
    char                        m_cPvpGameType{};
    std::vector<KMatchUserInfo> m_vecMatchUserInfo{};  // std::vector< KMatchUserInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_bRegFrontOfList)>>::put(w, m_bRegFrontOfList);
        Codec<std::remove_cvref_t<decltype(m_cPvpPlayerCount)>>::put(w, m_cPvpPlayerCount);
        Codec<std::remove_cvref_t<decltype(m_cPvpGameType)>>::put(w, m_cPvpGameType);
        Codec<std::remove_cvref_t<decltype(m_vecMatchUserInfo)>>::put(w, m_vecMatchUserInfo);
    }

    static KEPM_GET_PARTY_INFO_FOR_MATCH_MAKING_ACK read_from(Reader& r) {
        KEPM_GET_PARTY_INFO_FOR_MATCH_MAKING_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_bRegFrontOfList = Codec<std::remove_cvref_t<decltype(v.m_bRegFrontOfList)>>::get(r);
        v.m_cPvpPlayerCount = Codec<std::remove_cvref_t<decltype(v.m_cPvpPlayerCount)>>::get(r);
        v.m_cPvpGameType = Codec<std::remove_cvref_t<decltype(v.m_cPvpGameType)>>::get(r);
        v.m_vecMatchUserInfo = Codec<std::remove_cvref_t<decltype(v.m_vecMatchUserInfo)>>::get(r);
        return v;
    }
};

struct KMessengerInfo {
    std::map<std::int64_t, KFriendInfo> m_mapFriendInfo{};  // std::map< UidType, KFriendInfo >
    std::map<char, std::u16string>      m_mapGroup{};  // std::map< char, std::wstring >
    std::vector<KFriendMessageInfo>     m_vecFriendMessageInfo{};  // std::vector< KFriendMessageInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_mapFriendInfo)>>::put(w, m_mapFriendInfo);
        Codec<std::remove_cvref_t<decltype(m_mapGroup)>>::put(w, m_mapGroup);
        Codec<std::remove_cvref_t<decltype(m_vecFriendMessageInfo)>>::put(w, m_vecFriendMessageInfo);
    }

    static KMessengerInfo read_from(Reader& r) {
        KMessengerInfo v;
        v.m_mapFriendInfo = Codec<std::remove_cvref_t<decltype(v.m_mapFriendInfo)>>::get(r);
        v.m_mapGroup = Codec<std::remove_cvref_t<decltype(v.m_mapGroup)>>::get(r);
        v.m_vecFriendMessageInfo = Codec<std::remove_cvref_t<decltype(v.m_vecFriendMessageInfo)>>::get(r);
        return v;
    }
};

struct KMissionUpdate {
    std::int32_t              m_iTitleID{};  // int
    std::vector<std::int16_t> m_vecClearData{};  // std::vector< short >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iTitleID)>>::put(w, m_iTitleID);
        Codec<std::remove_cvref_t<decltype(m_vecClearData)>>::put(w, m_vecClearData);
    }

    static KMissionUpdate read_from(Reader& r) {
        KMissionUpdate v;
        v.m_iTitleID = Codec<std::remove_cvref_t<decltype(v.m_iTitleID)>>::get(r);
        v.m_vecClearData = Codec<std::remove_cvref_t<decltype(v.m_vecClearData)>>::get(r);
        return v;
    }
};

struct KModuleInfo {
    std::int32_t   m_iModuleUID{};  // int
    std::u16string m_wstrModuleName{};  // std::wstring
    std::int64_t   m_iCount{};  // __int64
    std::int32_t   m_iFlag{};  // int
    bool           m_bAllow{};
    bool           m_bReserveDBUpdate{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iModuleUID)>>::put(w, m_iModuleUID);
        Codec<std::remove_cvref_t<decltype(m_wstrModuleName)>>::put(w, m_wstrModuleName);
        Codec<std::remove_cvref_t<decltype(m_iCount)>>::put(w, m_iCount);
        Codec<std::remove_cvref_t<decltype(m_iFlag)>>::put(w, m_iFlag);
        Codec<std::remove_cvref_t<decltype(m_bAllow)>>::put(w, m_bAllow);
        Codec<std::remove_cvref_t<decltype(m_bReserveDBUpdate)>>::put(w, m_bReserveDBUpdate);
    }

    static KModuleInfo read_from(Reader& r) {
        KModuleInfo v;
        v.m_iModuleUID = Codec<std::remove_cvref_t<decltype(v.m_iModuleUID)>>::get(r);
        v.m_wstrModuleName = Codec<std::remove_cvref_t<decltype(v.m_wstrModuleName)>>::get(r);
        v.m_iCount = Codec<std::remove_cvref_t<decltype(v.m_iCount)>>::get(r);
        v.m_iFlag = Codec<std::remove_cvref_t<decltype(v.m_iFlag)>>::get(r);
        v.m_bAllow = Codec<std::remove_cvref_t<decltype(v.m_bAllow)>>::get(r);
        v.m_bReserveDBUpdate = Codec<std::remove_cvref_t<decltype(v.m_bReserveDBUpdate)>>::get(r);
        return v;
    }
};

struct KDBE_GET_MODULE_INFO_LIST_ACK {
    std::vector<KModuleInfo> m_vecModuleList{};  // std::vector< KModuleInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecModuleList)>>::put(w, m_vecModuleList);
    }

    static KDBE_GET_MODULE_INFO_LIST_ACK read_from(Reader& r) {
        KDBE_GET_MODULE_INFO_LIST_ACK v;
        v.m_vecModuleList = Codec<std::remove_cvref_t<decltype(v.m_vecModuleList)>>::get(r);
        return v;
    }
};

struct KNMUnitInfo {
    std::int64_t m_iUnitUID{};  // UidType
    std::int32_t m_iState{};  // int
    std::int32_t m_iDungeonID{};  // int
    std::int64_t m_iRoomUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iState)>>::put(w, m_iState);
        Codec<std::remove_cvref_t<decltype(m_iDungeonID)>>::put(w, m_iDungeonID);
        Codec<std::remove_cvref_t<decltype(m_iRoomUID)>>::put(w, m_iRoomUID);
    }

    static KNMUnitInfo read_from(Reader& r) {
        KNMUnitInfo v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_iState = Codec<std::remove_cvref_t<decltype(v.m_iState)>>::get(r);
        v.m_iDungeonID = Codec<std::remove_cvref_t<decltype(v.m_iDungeonID)>>::get(r);
        v.m_iRoomUID = Codec<std::remove_cvref_t<decltype(v.m_iRoomUID)>>::get(r);
        return v;
    }
};

struct KEGS_KNM_INVITE_NOT {
    std::u16string m_wstrNickName{};  // std::wstring
    KNMUnitInfo    m_kUnitInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_kUnitInfo)>>::put(w, m_kUnitInfo);
    }

    static KEGS_KNM_INVITE_NOT read_from(Reader& r) {
        KEGS_KNM_INVITE_NOT v;
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_kUnitInfo = Codec<std::remove_cvref_t<decltype(v.m_kUnitInfo)>>::get(r);
        return v;
    }
};

struct KEGS_KNM_TOGETHER_ACK {
    std::int32_t m_iOK{};  // int
    KNMUnitInfo  m_kUnitInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_kUnitInfo)>>::put(w, m_kUnitInfo);
    }

    static KEGS_KNM_TOGETHER_ACK read_from(Reader& r) {
        KEGS_KNM_TOGETHER_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_kUnitInfo = Codec<std::remove_cvref_t<decltype(v.m_kUnitInfo)>>::get(r);
        return v;
    }
};

struct KELG_KNM_INVITE_REQ {
    std::u16string m_wstrInviteUnitNickName{};  // std::wstring
    std::u16string m_wstrMyNickName{};  // std::wstring
    KNMUnitInfo    m_kMyUnitInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrInviteUnitNickName)>>::put(w, m_wstrInviteUnitNickName);
        Codec<std::remove_cvref_t<decltype(m_wstrMyNickName)>>::put(w, m_wstrMyNickName);
        Codec<std::remove_cvref_t<decltype(m_kMyUnitInfo)>>::put(w, m_kMyUnitInfo);
    }

    static KELG_KNM_INVITE_REQ read_from(Reader& r) {
        KELG_KNM_INVITE_REQ v;
        v.m_wstrInviteUnitNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrInviteUnitNickName)>>::get(r);
        v.m_wstrMyNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrMyNickName)>>::get(r);
        v.m_kMyUnitInfo = Codec<std::remove_cvref_t<decltype(v.m_kMyUnitInfo)>>::get(r);
        return v;
    }
};

struct KELG_KNM_TOGETHER_BY_UNIT_INFO_ACK {
    std::int32_t m_iOK{};  // int
    std::int64_t m_iDemandGSUID{};  // UidType
    std::int64_t m_iDemandUserUID{};  // UidType
    KNMUnitInfo  m_kUnitInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iDemandGSUID)>>::put(w, m_iDemandGSUID);
        Codec<std::remove_cvref_t<decltype(m_iDemandUserUID)>>::put(w, m_iDemandUserUID);
        Codec<std::remove_cvref_t<decltype(m_kUnitInfo)>>::put(w, m_kUnitInfo);
    }

    static KELG_KNM_TOGETHER_BY_UNIT_INFO_ACK read_from(Reader& r) {
        KELG_KNM_TOGETHER_BY_UNIT_INFO_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iDemandGSUID = Codec<std::remove_cvref_t<decltype(v.m_iDemandGSUID)>>::get(r);
        v.m_iDemandUserUID = Codec<std::remove_cvref_t<decltype(v.m_iDemandUserUID)>>::get(r);
        v.m_kUnitInfo = Codec<std::remove_cvref_t<decltype(v.m_kUnitInfo)>>::get(r);
        return v;
    }
};

struct KNXBTBonusProductInfo {
    std::uint32_t  m_ulProductNo{};  // unsigned long
    std::u16string m_wstrExtend{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_ulProductNo)>>::put(w, m_ulProductNo);
        Codec<std::remove_cvref_t<decltype(m_wstrExtend)>>::put(w, m_wstrExtend);
    }

    static KNXBTBonusProductInfo read_from(Reader& r) {
        KNXBTBonusProductInfo v;
        v.m_ulProductNo = Codec<std::remove_cvref_t<decltype(v.m_ulProductNo)>>::get(r);
        v.m_wstrExtend = Codec<std::remove_cvref_t<decltype(v.m_wstrExtend)>>::get(r);
        return v;
    }
};

struct KNXBTBonusProductInfoPickUp {
    std::uint32_t m_ulBonusProduct{};  // unsigned long
    std::uint32_t m_ulBonusAmount{};  // unsigned long

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_ulBonusProduct)>>::put(w, m_ulBonusProduct);
        Codec<std::remove_cvref_t<decltype(m_ulBonusAmount)>>::put(w, m_ulBonusAmount);
    }

    static KNXBTBonusProductInfoPickUp read_from(Reader& r) {
        KNXBTBonusProductInfoPickUp v;
        v.m_ulBonusProduct = Codec<std::remove_cvref_t<decltype(v.m_ulBonusProduct)>>::get(r);
        v.m_ulBonusAmount = Codec<std::remove_cvref_t<decltype(v.m_ulBonusAmount)>>::get(r);
        return v;
    }
};

struct KNXBTCategoryInfo {
    std::uint32_t  m_ulCategoryNo{};  // unsigned long
    std::u16string m_wstrCategoryName{};  // std::wstring
    std::uint32_t  m_ulParentCategoryNo{};  // unsigned long
    std::uint32_t  m_ulDisplayNo{};  // unsigned long

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_ulCategoryNo)>>::put(w, m_ulCategoryNo);
        Codec<std::remove_cvref_t<decltype(m_wstrCategoryName)>>::put(w, m_wstrCategoryName);
        Codec<std::remove_cvref_t<decltype(m_ulParentCategoryNo)>>::put(w, m_ulParentCategoryNo);
        Codec<std::remove_cvref_t<decltype(m_ulDisplayNo)>>::put(w, m_ulDisplayNo);
    }

    static KNXBTCategoryInfo read_from(Reader& r) {
        KNXBTCategoryInfo v;
        v.m_ulCategoryNo = Codec<std::remove_cvref_t<decltype(v.m_ulCategoryNo)>>::get(r);
        v.m_wstrCategoryName = Codec<std::remove_cvref_t<decltype(v.m_wstrCategoryName)>>::get(r);
        v.m_ulParentCategoryNo = Codec<std::remove_cvref_t<decltype(v.m_ulParentCategoryNo)>>::get(r);
        v.m_ulDisplayNo = Codec<std::remove_cvref_t<decltype(v.m_ulDisplayNo)>>::get(r);
        return v;
    }
};

struct KENX_BT_CATEGORY_INQUIRY_ACK {
    std::uint32_t                  m_ulPacketNo{};  // unsigned long
    std::uint8_t                   m_bytePacketType{};  // BYTE
    std::uint32_t                  m_ulResult{};  // unsigned long
    std::vector<KNXBTCategoryInfo> m_vecNXBTCategoryInfo{};  // std::vector< KNXBTCategoryInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_ulPacketNo)>>::put(w, m_ulPacketNo);
        Codec<std::remove_cvref_t<decltype(m_bytePacketType)>>::put(w, m_bytePacketType);
        Codec<std::remove_cvref_t<decltype(m_ulResult)>>::put(w, m_ulResult);
        Codec<std::remove_cvref_t<decltype(m_vecNXBTCategoryInfo)>>::put(w, m_vecNXBTCategoryInfo);
    }

    static KENX_BT_CATEGORY_INQUIRY_ACK read_from(Reader& r) {
        KENX_BT_CATEGORY_INQUIRY_ACK v;
        v.m_ulPacketNo = Codec<std::remove_cvref_t<decltype(v.m_ulPacketNo)>>::get(r);
        v.m_bytePacketType = Codec<std::remove_cvref_t<decltype(v.m_bytePacketType)>>::get(r);
        v.m_ulResult = Codec<std::remove_cvref_t<decltype(v.m_ulResult)>>::get(r);
        v.m_vecNXBTCategoryInfo = Codec<std::remove_cvref_t<decltype(v.m_vecNXBTCategoryInfo)>>::get(r);
        return v;
    }
};

struct KNXBTCouponProductInfo {
    std::uint32_t  m_ulProductNo{};  // unsigned long
    std::u16string m_wstrExtendValue{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_ulProductNo)>>::put(w, m_ulProductNo);
        Codec<std::remove_cvref_t<decltype(m_wstrExtendValue)>>::put(w, m_wstrExtendValue);
    }

    static KNXBTCouponProductInfo read_from(Reader& r) {
        KNXBTCouponProductInfo v;
        v.m_ulProductNo = Codec<std::remove_cvref_t<decltype(v.m_ulProductNo)>>::get(r);
        v.m_wstrExtendValue = Codec<std::remove_cvref_t<decltype(v.m_wstrExtendValue)>>::get(r);
        return v;
    }
};

struct KENX_BT_COUPON_INQUIRY_ACK {
    std::uint32_t                       m_ulPacketNo{};  // unsigned long
    std::uint8_t                        m_bytePacketType{};  // BYTE
    std::uint32_t                       m_ulResult{};  // unsigned long
    std::vector<KNXBTCouponProductInfo> m_vecProductInfo{};  // std::vector< KNXBTCouponProductInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_ulPacketNo)>>::put(w, m_ulPacketNo);
        Codec<std::remove_cvref_t<decltype(m_bytePacketType)>>::put(w, m_bytePacketType);
        Codec<std::remove_cvref_t<decltype(m_ulResult)>>::put(w, m_ulResult);
        Codec<std::remove_cvref_t<decltype(m_vecProductInfo)>>::put(w, m_vecProductInfo);
    }

    static KENX_BT_COUPON_INQUIRY_ACK read_from(Reader& r) {
        KENX_BT_COUPON_INQUIRY_ACK v;
        v.m_ulPacketNo = Codec<std::remove_cvref_t<decltype(v.m_ulPacketNo)>>::get(r);
        v.m_bytePacketType = Codec<std::remove_cvref_t<decltype(v.m_bytePacketType)>>::get(r);
        v.m_ulResult = Codec<std::remove_cvref_t<decltype(v.m_ulResult)>>::get(r);
        v.m_vecProductInfo = Codec<std::remove_cvref_t<decltype(v.m_vecProductInfo)>>::get(r);
        return v;
    }
};

struct KENX_BT_COUPON_INQUIRY_NEW_ACK {
    std::uint32_t                       m_ulPacketNo{};  // unsigned long
    std::uint8_t                        m_bytePacketType{};  // BYTE
    std::uint32_t                       m_ulResult{};  // unsigned long
    std::vector<KNXBTCouponProductInfo> m_vecProductInfo{};  // std::vector< KNXBTCouponProductInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_ulPacketNo)>>::put(w, m_ulPacketNo);
        Codec<std::remove_cvref_t<decltype(m_bytePacketType)>>::put(w, m_bytePacketType);
        Codec<std::remove_cvref_t<decltype(m_ulResult)>>::put(w, m_ulResult);
        Codec<std::remove_cvref_t<decltype(m_vecProductInfo)>>::put(w, m_vecProductInfo);
    }

    static KENX_BT_COUPON_INQUIRY_NEW_ACK read_from(Reader& r) {
        KENX_BT_COUPON_INQUIRY_NEW_ACK v;
        v.m_ulPacketNo = Codec<std::remove_cvref_t<decltype(v.m_ulPacketNo)>>::get(r);
        v.m_bytePacketType = Codec<std::remove_cvref_t<decltype(v.m_bytePacketType)>>::get(r);
        v.m_ulResult = Codec<std::remove_cvref_t<decltype(v.m_ulResult)>>::get(r);
        v.m_vecProductInfo = Codec<std::remove_cvref_t<decltype(v.m_vecProductInfo)>>::get(r);
        return v;
    }
};

struct KENX_BT_COUPON_USING_ACK {
    std::uint32_t                       m_ulPacketNo{};  // unsigned long
    std::uint8_t                        m_bytePacketType{};  // BYTE
    std::u16string                      m_wstrCouponTXID{};  // std::wstring
    std::uint32_t                       m_ulResult{};  // unsigned long
    std::uint32_t                       m_ulOrderNo{};  // unsigned long
    std::vector<KNXBTCouponProductInfo> m_vecCouponProductInfo{};  // std::vector< KNXBTCouponProductInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_ulPacketNo)>>::put(w, m_ulPacketNo);
        Codec<std::remove_cvref_t<decltype(m_bytePacketType)>>::put(w, m_bytePacketType);
        Codec<std::remove_cvref_t<decltype(m_wstrCouponTXID)>>::put(w, m_wstrCouponTXID);
        Codec<std::remove_cvref_t<decltype(m_ulResult)>>::put(w, m_ulResult);
        Codec<std::remove_cvref_t<decltype(m_ulOrderNo)>>::put(w, m_ulOrderNo);
        Codec<std::remove_cvref_t<decltype(m_vecCouponProductInfo)>>::put(w, m_vecCouponProductInfo);
    }

    static KENX_BT_COUPON_USING_ACK read_from(Reader& r) {
        KENX_BT_COUPON_USING_ACK v;
        v.m_ulPacketNo = Codec<std::remove_cvref_t<decltype(v.m_ulPacketNo)>>::get(r);
        v.m_bytePacketType = Codec<std::remove_cvref_t<decltype(v.m_bytePacketType)>>::get(r);
        v.m_wstrCouponTXID = Codec<std::remove_cvref_t<decltype(v.m_wstrCouponTXID)>>::get(r);
        v.m_ulResult = Codec<std::remove_cvref_t<decltype(v.m_ulResult)>>::get(r);
        v.m_ulOrderNo = Codec<std::remove_cvref_t<decltype(v.m_ulOrderNo)>>::get(r);
        v.m_vecCouponProductInfo = Codec<std::remove_cvref_t<decltype(v.m_vecCouponProductInfo)>>::get(r);
        return v;
    }
};

struct KNXBTLotteryInfo {
    std::uint32_t  m_ulOrderNo{};  // unsigned long
    std::uint32_t  m_ulLotteryProductNo{};  // unsigned long
    std::uint32_t  m_ulProductNo{};  // unsigned long
    std::u16string m_wstrProductName{};  // std::wstring
    std::u16string m_wstrProductID{};  // std::wstring
    std::int16_t   m_sProductExpire{};  // short
    std::uint16_t  m_usProductPieces{};  // unsigned short
    std::u16string m_wstrProductAttribute0{};  // std::wstring
    std::u16string m_wstrProductAttribute1{};  // std::wstring
    std::u16string m_wstrProductAttribute2{};  // std::wstring
    std::u16string m_wstrProductAttribute3{};  // std::wstring
    std::u16string m_wstrProductAttribute4{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_ulOrderNo)>>::put(w, m_ulOrderNo);
        Codec<std::remove_cvref_t<decltype(m_ulLotteryProductNo)>>::put(w, m_ulLotteryProductNo);
        Codec<std::remove_cvref_t<decltype(m_ulProductNo)>>::put(w, m_ulProductNo);
        Codec<std::remove_cvref_t<decltype(m_wstrProductName)>>::put(w, m_wstrProductName);
        Codec<std::remove_cvref_t<decltype(m_wstrProductID)>>::put(w, m_wstrProductID);
        Codec<std::remove_cvref_t<decltype(m_sProductExpire)>>::put(w, m_sProductExpire);
        Codec<std::remove_cvref_t<decltype(m_usProductPieces)>>::put(w, m_usProductPieces);
        Codec<std::remove_cvref_t<decltype(m_wstrProductAttribute0)>>::put(w, m_wstrProductAttribute0);
        Codec<std::remove_cvref_t<decltype(m_wstrProductAttribute1)>>::put(w, m_wstrProductAttribute1);
        Codec<std::remove_cvref_t<decltype(m_wstrProductAttribute2)>>::put(w, m_wstrProductAttribute2);
        Codec<std::remove_cvref_t<decltype(m_wstrProductAttribute3)>>::put(w, m_wstrProductAttribute3);
        Codec<std::remove_cvref_t<decltype(m_wstrProductAttribute4)>>::put(w, m_wstrProductAttribute4);
    }

    static KNXBTLotteryInfo read_from(Reader& r) {
        KNXBTLotteryInfo v;
        v.m_ulOrderNo = Codec<std::remove_cvref_t<decltype(v.m_ulOrderNo)>>::get(r);
        v.m_ulLotteryProductNo = Codec<std::remove_cvref_t<decltype(v.m_ulLotteryProductNo)>>::get(r);
        v.m_ulProductNo = Codec<std::remove_cvref_t<decltype(v.m_ulProductNo)>>::get(r);
        v.m_wstrProductName = Codec<std::remove_cvref_t<decltype(v.m_wstrProductName)>>::get(r);
        v.m_wstrProductID = Codec<std::remove_cvref_t<decltype(v.m_wstrProductID)>>::get(r);
        v.m_sProductExpire = Codec<std::remove_cvref_t<decltype(v.m_sProductExpire)>>::get(r);
        v.m_usProductPieces = Codec<std::remove_cvref_t<decltype(v.m_usProductPieces)>>::get(r);
        v.m_wstrProductAttribute0 = Codec<std::remove_cvref_t<decltype(v.m_wstrProductAttribute0)>>::get(r);
        v.m_wstrProductAttribute1 = Codec<std::remove_cvref_t<decltype(v.m_wstrProductAttribute1)>>::get(r);
        v.m_wstrProductAttribute2 = Codec<std::remove_cvref_t<decltype(v.m_wstrProductAttribute2)>>::get(r);
        v.m_wstrProductAttribute3 = Codec<std::remove_cvref_t<decltype(v.m_wstrProductAttribute3)>>::get(r);
        v.m_wstrProductAttribute4 = Codec<std::remove_cvref_t<decltype(v.m_wstrProductAttribute4)>>::get(r);
        return v;
    }
};

struct KNXBTOrderInfo {
    std::uint32_t  m_ulOrderNo{};  // unsigned long
    std::uint32_t  m_ulProductNo{};  // unsigned long
    std::uint8_t   m_byteProductKind{};  // BYTE
    std::u16string m_wstrProductName{};  // std::wstring
    std::u16string m_wstrProductID{};  // std::wstring
    std::int16_t   m_sProductExpire{};  // short
    std::uint16_t  m_usProductPieces{};  // unsigned short
    std::uint16_t  m_usOrderQuantity{};  // unsigned short
    std::uint16_t  m_usRemainOrderQuantity{};  // unsigned short
    std::uint8_t   m_byteIsPresent{};  // BYTE
    std::uint8_t   m_byteIsRead{};  // BYTE
    std::uint8_t   m_byteSenderServerNo{};  // BYTE
    std::u16string m_wstrSenderGameID{};  // std::wstring
    std::u16string m_wstrSenderPresentMessage{};  // std::wstring
    std::u16string m_wstrProductAttribute0{};  // std::wstring
    std::u16string m_wstrProductAttribute1{};  // std::wstring
    std::u16string m_wstrProductAttribute2{};  // std::wstring
    std::u16string m_wstrProductAttribute3{};  // std::wstring
    std::u16string m_wstrProductAttribute4{};  // std::wstring
    std::u16string m_wstrExtendValue{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_ulOrderNo)>>::put(w, m_ulOrderNo);
        Codec<std::remove_cvref_t<decltype(m_ulProductNo)>>::put(w, m_ulProductNo);
        Codec<std::remove_cvref_t<decltype(m_byteProductKind)>>::put(w, m_byteProductKind);
        Codec<std::remove_cvref_t<decltype(m_wstrProductName)>>::put(w, m_wstrProductName);
        Codec<std::remove_cvref_t<decltype(m_wstrProductID)>>::put(w, m_wstrProductID);
        Codec<std::remove_cvref_t<decltype(m_sProductExpire)>>::put(w, m_sProductExpire);
        Codec<std::remove_cvref_t<decltype(m_usProductPieces)>>::put(w, m_usProductPieces);
        Codec<std::remove_cvref_t<decltype(m_usOrderQuantity)>>::put(w, m_usOrderQuantity);
        Codec<std::remove_cvref_t<decltype(m_usRemainOrderQuantity)>>::put(w, m_usRemainOrderQuantity);
        Codec<std::remove_cvref_t<decltype(m_byteIsPresent)>>::put(w, m_byteIsPresent);
        Codec<std::remove_cvref_t<decltype(m_byteIsRead)>>::put(w, m_byteIsRead);
        Codec<std::remove_cvref_t<decltype(m_byteSenderServerNo)>>::put(w, m_byteSenderServerNo);
        Codec<std::remove_cvref_t<decltype(m_wstrSenderGameID)>>::put(w, m_wstrSenderGameID);
        Codec<std::remove_cvref_t<decltype(m_wstrSenderPresentMessage)>>::put(w, m_wstrSenderPresentMessage);
        Codec<std::remove_cvref_t<decltype(m_wstrProductAttribute0)>>::put(w, m_wstrProductAttribute0);
        Codec<std::remove_cvref_t<decltype(m_wstrProductAttribute1)>>::put(w, m_wstrProductAttribute1);
        Codec<std::remove_cvref_t<decltype(m_wstrProductAttribute2)>>::put(w, m_wstrProductAttribute2);
        Codec<std::remove_cvref_t<decltype(m_wstrProductAttribute3)>>::put(w, m_wstrProductAttribute3);
        Codec<std::remove_cvref_t<decltype(m_wstrProductAttribute4)>>::put(w, m_wstrProductAttribute4);
        Codec<std::remove_cvref_t<decltype(m_wstrExtendValue)>>::put(w, m_wstrExtendValue);
    }

    static KNXBTOrderInfo read_from(Reader& r) {
        KNXBTOrderInfo v;
        v.m_ulOrderNo = Codec<std::remove_cvref_t<decltype(v.m_ulOrderNo)>>::get(r);
        v.m_ulProductNo = Codec<std::remove_cvref_t<decltype(v.m_ulProductNo)>>::get(r);
        v.m_byteProductKind = Codec<std::remove_cvref_t<decltype(v.m_byteProductKind)>>::get(r);
        v.m_wstrProductName = Codec<std::remove_cvref_t<decltype(v.m_wstrProductName)>>::get(r);
        v.m_wstrProductID = Codec<std::remove_cvref_t<decltype(v.m_wstrProductID)>>::get(r);
        v.m_sProductExpire = Codec<std::remove_cvref_t<decltype(v.m_sProductExpire)>>::get(r);
        v.m_usProductPieces = Codec<std::remove_cvref_t<decltype(v.m_usProductPieces)>>::get(r);
        v.m_usOrderQuantity = Codec<std::remove_cvref_t<decltype(v.m_usOrderQuantity)>>::get(r);
        v.m_usRemainOrderQuantity = Codec<std::remove_cvref_t<decltype(v.m_usRemainOrderQuantity)>>::get(r);
        v.m_byteIsPresent = Codec<std::remove_cvref_t<decltype(v.m_byteIsPresent)>>::get(r);
        v.m_byteIsRead = Codec<std::remove_cvref_t<decltype(v.m_byteIsRead)>>::get(r);
        v.m_byteSenderServerNo = Codec<std::remove_cvref_t<decltype(v.m_byteSenderServerNo)>>::get(r);
        v.m_wstrSenderGameID = Codec<std::remove_cvref_t<decltype(v.m_wstrSenderGameID)>>::get(r);
        v.m_wstrSenderPresentMessage = Codec<std::remove_cvref_t<decltype(v.m_wstrSenderPresentMessage)>>::get(r);
        v.m_wstrProductAttribute0 = Codec<std::remove_cvref_t<decltype(v.m_wstrProductAttribute0)>>::get(r);
        v.m_wstrProductAttribute1 = Codec<std::remove_cvref_t<decltype(v.m_wstrProductAttribute1)>>::get(r);
        v.m_wstrProductAttribute2 = Codec<std::remove_cvref_t<decltype(v.m_wstrProductAttribute2)>>::get(r);
        v.m_wstrProductAttribute3 = Codec<std::remove_cvref_t<decltype(v.m_wstrProductAttribute3)>>::get(r);
        v.m_wstrProductAttribute4 = Codec<std::remove_cvref_t<decltype(v.m_wstrProductAttribute4)>>::get(r);
        v.m_wstrExtendValue = Codec<std::remove_cvref_t<decltype(v.m_wstrExtendValue)>>::get(r);
        return v;
    }
};

struct KNXBTPackageInfo {
    std::uint32_t  m_ulOrderNo{};  // unsigned long
    std::uint32_t  m_ulPackageProductNo{};  // unsigned long
    std::uint32_t  m_ulProductNo{};  // unsigned long
    std::u16string m_wstrProductName{};  // std::wstring
    std::u16string m_wstrProductID{};  // std::wstring
    std::int16_t   m_sProductExpire{};  // short
    std::uint16_t  m_usProductPieces{};  // unsigned short
    std::u16string m_wstrProductAttribute0{};  // std::wstring
    std::u16string m_wstrProductAttribute1{};  // std::wstring
    std::u16string m_wstrProductAttribute2{};  // std::wstring
    std::u16string m_wstrProductAttribute3{};  // std::wstring
    std::u16string m_wstrProductAttribute4{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_ulOrderNo)>>::put(w, m_ulOrderNo);
        Codec<std::remove_cvref_t<decltype(m_ulPackageProductNo)>>::put(w, m_ulPackageProductNo);
        Codec<std::remove_cvref_t<decltype(m_ulProductNo)>>::put(w, m_ulProductNo);
        Codec<std::remove_cvref_t<decltype(m_wstrProductName)>>::put(w, m_wstrProductName);
        Codec<std::remove_cvref_t<decltype(m_wstrProductID)>>::put(w, m_wstrProductID);
        Codec<std::remove_cvref_t<decltype(m_sProductExpire)>>::put(w, m_sProductExpire);
        Codec<std::remove_cvref_t<decltype(m_usProductPieces)>>::put(w, m_usProductPieces);
        Codec<std::remove_cvref_t<decltype(m_wstrProductAttribute0)>>::put(w, m_wstrProductAttribute0);
        Codec<std::remove_cvref_t<decltype(m_wstrProductAttribute1)>>::put(w, m_wstrProductAttribute1);
        Codec<std::remove_cvref_t<decltype(m_wstrProductAttribute2)>>::put(w, m_wstrProductAttribute2);
        Codec<std::remove_cvref_t<decltype(m_wstrProductAttribute3)>>::put(w, m_wstrProductAttribute3);
        Codec<std::remove_cvref_t<decltype(m_wstrProductAttribute4)>>::put(w, m_wstrProductAttribute4);
    }

    static KNXBTPackageInfo read_from(Reader& r) {
        KNXBTPackageInfo v;
        v.m_ulOrderNo = Codec<std::remove_cvref_t<decltype(v.m_ulOrderNo)>>::get(r);
        v.m_ulPackageProductNo = Codec<std::remove_cvref_t<decltype(v.m_ulPackageProductNo)>>::get(r);
        v.m_ulProductNo = Codec<std::remove_cvref_t<decltype(v.m_ulProductNo)>>::get(r);
        v.m_wstrProductName = Codec<std::remove_cvref_t<decltype(v.m_wstrProductName)>>::get(r);
        v.m_wstrProductID = Codec<std::remove_cvref_t<decltype(v.m_wstrProductID)>>::get(r);
        v.m_sProductExpire = Codec<std::remove_cvref_t<decltype(v.m_sProductExpire)>>::get(r);
        v.m_usProductPieces = Codec<std::remove_cvref_t<decltype(v.m_usProductPieces)>>::get(r);
        v.m_wstrProductAttribute0 = Codec<std::remove_cvref_t<decltype(v.m_wstrProductAttribute0)>>::get(r);
        v.m_wstrProductAttribute1 = Codec<std::remove_cvref_t<decltype(v.m_wstrProductAttribute1)>>::get(r);
        v.m_wstrProductAttribute2 = Codec<std::remove_cvref_t<decltype(v.m_wstrProductAttribute2)>>::get(r);
        v.m_wstrProductAttribute3 = Codec<std::remove_cvref_t<decltype(v.m_wstrProductAttribute3)>>::get(r);
        v.m_wstrProductAttribute4 = Codec<std::remove_cvref_t<decltype(v.m_wstrProductAttribute4)>>::get(r);
        return v;
    }
};

struct KEGS_PURCHASED_CASH_ITEM_LIST_ACK {
    std::int32_t                  m_iOK{};  // int
    std::vector<KNXBTOrderInfo>   m_vecOrderInfo{};  // std::vector< KNXBTOrderInfo >
    std::vector<KNXBTPackageInfo> m_vecPackageInfo{};  // std::vector< KNXBTPackageInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_vecOrderInfo)>>::put(w, m_vecOrderInfo);
        Codec<std::remove_cvref_t<decltype(m_vecPackageInfo)>>::put(w, m_vecPackageInfo);
    }

    static KEGS_PURCHASED_CASH_ITEM_LIST_ACK read_from(Reader& r) {
        KEGS_PURCHASED_CASH_ITEM_LIST_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_vecOrderInfo = Codec<std::remove_cvref_t<decltype(v.m_vecOrderInfo)>>::get(r);
        v.m_vecPackageInfo = Codec<std::remove_cvref_t<decltype(v.m_vecPackageInfo)>>::get(r);
        return v;
    }
};

struct KENX_BT_NISMS_INVENTORY_INQUIRY_ACK {
    std::uint32_t                 m_ulPacketNo{};  // unsigned long
    std::uint8_t                  m_bytePacketType{};  // BYTE
    std::uint32_t                 m_ulResult{};  // unsigned long
    std::uint32_t                 m_ulTotalCount{};  // unsigned long
    std::vector<KNXBTOrderInfo>   m_vecNXBTOrderInfo{};  // std::vector< KNXBTOrderInfo >
    std::vector<KNXBTPackageInfo> m_vecNXBTPackageInfo{};  // std::vector< KNXBTPackageInfo >
    std::vector<KNXBTLotteryInfo> m_vecNXBTLotteryInfo{};  // std::vector< KNXBTLotteryInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_ulPacketNo)>>::put(w, m_ulPacketNo);
        Codec<std::remove_cvref_t<decltype(m_bytePacketType)>>::put(w, m_bytePacketType);
        Codec<std::remove_cvref_t<decltype(m_ulResult)>>::put(w, m_ulResult);
        Codec<std::remove_cvref_t<decltype(m_ulTotalCount)>>::put(w, m_ulTotalCount);
        Codec<std::remove_cvref_t<decltype(m_vecNXBTOrderInfo)>>::put(w, m_vecNXBTOrderInfo);
        Codec<std::remove_cvref_t<decltype(m_vecNXBTPackageInfo)>>::put(w, m_vecNXBTPackageInfo);
        Codec<std::remove_cvref_t<decltype(m_vecNXBTLotteryInfo)>>::put(w, m_vecNXBTLotteryInfo);
    }

    static KENX_BT_NISMS_INVENTORY_INQUIRY_ACK read_from(Reader& r) {
        KENX_BT_NISMS_INVENTORY_INQUIRY_ACK v;
        v.m_ulPacketNo = Codec<std::remove_cvref_t<decltype(v.m_ulPacketNo)>>::get(r);
        v.m_bytePacketType = Codec<std::remove_cvref_t<decltype(v.m_bytePacketType)>>::get(r);
        v.m_ulResult = Codec<std::remove_cvref_t<decltype(v.m_ulResult)>>::get(r);
        v.m_ulTotalCount = Codec<std::remove_cvref_t<decltype(v.m_ulTotalCount)>>::get(r);
        v.m_vecNXBTOrderInfo = Codec<std::remove_cvref_t<decltype(v.m_vecNXBTOrderInfo)>>::get(r);
        v.m_vecNXBTPackageInfo = Codec<std::remove_cvref_t<decltype(v.m_vecNXBTPackageInfo)>>::get(r);
        v.m_vecNXBTLotteryInfo = Codec<std::remove_cvref_t<decltype(v.m_vecNXBTLotteryInfo)>>::get(r);
        return v;
    }
};

struct KNXBTPickUpPackageInfo {
    std::uint32_t  m_ulProductNo{};  // unsigned long
    std::u16string m_wstrProductID{};  // std::wstring
    std::int16_t   m_sProductExpire{};  // short
    std::uint16_t  m_usProductPieces{};  // unsigned short
    std::int32_t   m_iSubProductNo{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_ulProductNo)>>::put(w, m_ulProductNo);
        Codec<std::remove_cvref_t<decltype(m_wstrProductID)>>::put(w, m_wstrProductID);
        Codec<std::remove_cvref_t<decltype(m_sProductExpire)>>::put(w, m_sProductExpire);
        Codec<std::remove_cvref_t<decltype(m_usProductPieces)>>::put(w, m_usProductPieces);
        Codec<std::remove_cvref_t<decltype(m_iSubProductNo)>>::put(w, m_iSubProductNo);
    }

    static KNXBTPickUpPackageInfo read_from(Reader& r) {
        KNXBTPickUpPackageInfo v;
        v.m_ulProductNo = Codec<std::remove_cvref_t<decltype(v.m_ulProductNo)>>::get(r);
        v.m_wstrProductID = Codec<std::remove_cvref_t<decltype(v.m_wstrProductID)>>::get(r);
        v.m_sProductExpire = Codec<std::remove_cvref_t<decltype(v.m_sProductExpire)>>::get(r);
        v.m_usProductPieces = Codec<std::remove_cvref_t<decltype(v.m_usProductPieces)>>::get(r);
        v.m_iSubProductNo = Codec<std::remove_cvref_t<decltype(v.m_iSubProductNo)>>::get(r);
        return v;
    }
};

struct KEBILL_PICK_UP_REQ {
    std::int64_t                         m_iTransNo{};  // UidType
    std::int32_t                         m_iProductNo{};  // int
    std::int64_t                         m_iUnitUID{};  // UidType
    std::map<std::int32_t, std::int32_t> m_mapSocketForCash{};  // std::map< int, int >
    std::uint8_t                         m_byteProductKind{};  // BYTE
    std::vector<KNXBTPickUpPackageInfo>  m_vecPickUpPackageInfo{};  // std::vector< KNXBTPickUpPackageInfo >
    std::int32_t                         m_iSubProductNo{};  // int
    std::int32_t                         m_iSubProductID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iTransNo)>>::put(w, m_iTransNo);
        Codec<std::remove_cvref_t<decltype(m_iProductNo)>>::put(w, m_iProductNo);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_mapSocketForCash)>>::put(w, m_mapSocketForCash);
        Codec<std::remove_cvref_t<decltype(m_byteProductKind)>>::put(w, m_byteProductKind);
        Codec<std::remove_cvref_t<decltype(m_vecPickUpPackageInfo)>>::put(w, m_vecPickUpPackageInfo);
        Codec<std::remove_cvref_t<decltype(m_iSubProductNo)>>::put(w, m_iSubProductNo);
        Codec<std::remove_cvref_t<decltype(m_iSubProductID)>>::put(w, m_iSubProductID);
    }

    static KEBILL_PICK_UP_REQ read_from(Reader& r) {
        KEBILL_PICK_UP_REQ v;
        v.m_iTransNo = Codec<std::remove_cvref_t<decltype(v.m_iTransNo)>>::get(r);
        v.m_iProductNo = Codec<std::remove_cvref_t<decltype(v.m_iProductNo)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_mapSocketForCash = Codec<std::remove_cvref_t<decltype(v.m_mapSocketForCash)>>::get(r);
        v.m_byteProductKind = Codec<std::remove_cvref_t<decltype(v.m_byteProductKind)>>::get(r);
        v.m_vecPickUpPackageInfo = Codec<std::remove_cvref_t<decltype(v.m_vecPickUpPackageInfo)>>::get(r);
        v.m_iSubProductNo = Codec<std::remove_cvref_t<decltype(v.m_iSubProductNo)>>::get(r);
        v.m_iSubProductID = Codec<std::remove_cvref_t<decltype(v.m_iSubProductID)>>::get(r);
        return v;
    }
};

struct KEGS_GET_PURCHASED_PACKAGE_CASH_ITEM_REQ {
    std::uint32_t                        m_ulOrderNo{};  // unsigned long
    std::uint32_t                        m_ulPackageProductNo{};  // unsigned long
    std::u16string                       m_wstrPackageProductID{};  // std::wstring
    std::uint8_t                         m_byteProductKind{};  // BYTE
    std::vector<KNXBTPickUpPackageInfo>  m_vecPickUpPackageInfo{};  // std::vector< KNXBTPickUpPackageInfo >
    std::map<std::int32_t, std::int32_t> m_mapSocketForCash{};  // std::map< int, int >
    std::map<std::int32_t, std::int32_t> m_mapSocketGroupIDForCash{};  // std::map< int, int >
    std::int32_t                         m_iProductID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_ulOrderNo)>>::put(w, m_ulOrderNo);
        Codec<std::remove_cvref_t<decltype(m_ulPackageProductNo)>>::put(w, m_ulPackageProductNo);
        Codec<std::remove_cvref_t<decltype(m_wstrPackageProductID)>>::put(w, m_wstrPackageProductID);
        Codec<std::remove_cvref_t<decltype(m_byteProductKind)>>::put(w, m_byteProductKind);
        Codec<std::remove_cvref_t<decltype(m_vecPickUpPackageInfo)>>::put(w, m_vecPickUpPackageInfo);
        Codec<std::remove_cvref_t<decltype(m_mapSocketForCash)>>::put(w, m_mapSocketForCash);
        Codec<std::remove_cvref_t<decltype(m_mapSocketGroupIDForCash)>>::put(w, m_mapSocketGroupIDForCash);
        Codec<std::remove_cvref_t<decltype(m_iProductID)>>::put(w, m_iProductID);
    }

    static KEGS_GET_PURCHASED_PACKAGE_CASH_ITEM_REQ read_from(Reader& r) {
        KEGS_GET_PURCHASED_PACKAGE_CASH_ITEM_REQ v;
        v.m_ulOrderNo = Codec<std::remove_cvref_t<decltype(v.m_ulOrderNo)>>::get(r);
        v.m_ulPackageProductNo = Codec<std::remove_cvref_t<decltype(v.m_ulPackageProductNo)>>::get(r);
        v.m_wstrPackageProductID = Codec<std::remove_cvref_t<decltype(v.m_wstrPackageProductID)>>::get(r);
        v.m_byteProductKind = Codec<std::remove_cvref_t<decltype(v.m_byteProductKind)>>::get(r);
        v.m_vecPickUpPackageInfo = Codec<std::remove_cvref_t<decltype(v.m_vecPickUpPackageInfo)>>::get(r);
        v.m_mapSocketForCash = Codec<std::remove_cvref_t<decltype(v.m_mapSocketForCash)>>::get(r);
        v.m_mapSocketGroupIDForCash = Codec<std::remove_cvref_t<decltype(v.m_mapSocketGroupIDForCash)>>::get(r);
        v.m_iProductID = Codec<std::remove_cvref_t<decltype(v.m_iProductID)>>::get(r);
        return v;
    }
};

struct KNXBTProductInfo {
    std::uint32_t                      m_ulProductNo{};  // unsigned long
    std::uint32_t                      m_ulRelationProductNo{};  // unsigned long
    std::int16_t                       m_sProductExpire{};  // short
    std::uint16_t                      m_usProductPieces{};  // unsigned short
    std::u16string                     m_wstrProductID{};  // std::wstring
    std::u16string                     m_wstrProductGUID{};  // std::wstring
    std::uint32_t                      m_ulPaymentType{};  // unsigned long
    std::u16string                     m_wstrProductType{};  // std::wstring
    std::uint32_t                      m_ulSalePrice{};  // unsigned long
    std::uint32_t                      m_ulCategoryNo{};  // unsigned long
    std::vector<KNXBTBonusProductInfo> m_vecBonusProductInfo{};  // std::vector< KNXBTBonusProductInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_ulProductNo)>>::put(w, m_ulProductNo);
        Codec<std::remove_cvref_t<decltype(m_ulRelationProductNo)>>::put(w, m_ulRelationProductNo);
        Codec<std::remove_cvref_t<decltype(m_sProductExpire)>>::put(w, m_sProductExpire);
        Codec<std::remove_cvref_t<decltype(m_usProductPieces)>>::put(w, m_usProductPieces);
        Codec<std::remove_cvref_t<decltype(m_wstrProductID)>>::put(w, m_wstrProductID);
        Codec<std::remove_cvref_t<decltype(m_wstrProductGUID)>>::put(w, m_wstrProductGUID);
        Codec<std::remove_cvref_t<decltype(m_ulPaymentType)>>::put(w, m_ulPaymentType);
        Codec<std::remove_cvref_t<decltype(m_wstrProductType)>>::put(w, m_wstrProductType);
        Codec<std::remove_cvref_t<decltype(m_ulSalePrice)>>::put(w, m_ulSalePrice);
        Codec<std::remove_cvref_t<decltype(m_ulCategoryNo)>>::put(w, m_ulCategoryNo);
        Codec<std::remove_cvref_t<decltype(m_vecBonusProductInfo)>>::put(w, m_vecBonusProductInfo);
    }

    static KNXBTProductInfo read_from(Reader& r) {
        KNXBTProductInfo v;
        v.m_ulProductNo = Codec<std::remove_cvref_t<decltype(v.m_ulProductNo)>>::get(r);
        v.m_ulRelationProductNo = Codec<std::remove_cvref_t<decltype(v.m_ulRelationProductNo)>>::get(r);
        v.m_sProductExpire = Codec<std::remove_cvref_t<decltype(v.m_sProductExpire)>>::get(r);
        v.m_usProductPieces = Codec<std::remove_cvref_t<decltype(v.m_usProductPieces)>>::get(r);
        v.m_wstrProductID = Codec<std::remove_cvref_t<decltype(v.m_wstrProductID)>>::get(r);
        v.m_wstrProductGUID = Codec<std::remove_cvref_t<decltype(v.m_wstrProductGUID)>>::get(r);
        v.m_ulPaymentType = Codec<std::remove_cvref_t<decltype(v.m_ulPaymentType)>>::get(r);
        v.m_wstrProductType = Codec<std::remove_cvref_t<decltype(v.m_wstrProductType)>>::get(r);
        v.m_ulSalePrice = Codec<std::remove_cvref_t<decltype(v.m_ulSalePrice)>>::get(r);
        v.m_ulCategoryNo = Codec<std::remove_cvref_t<decltype(v.m_ulCategoryNo)>>::get(r);
        v.m_vecBonusProductInfo = Codec<std::remove_cvref_t<decltype(v.m_vecBonusProductInfo)>>::get(r);
        return v;
    }
};

struct KEGS_CASH_PRODUCT_INFO_ACK {
    bool                          m_bFinal{};
    std::vector<KNXBTProductInfo> m_vecProductInfo{};  // std::vector< KNXBTProductInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bFinal)>>::put(w, m_bFinal);
        Codec<std::remove_cvref_t<decltype(m_vecProductInfo)>>::put(w, m_vecProductInfo);
    }

    static KEGS_CASH_PRODUCT_INFO_ACK read_from(Reader& r) {
        KEGS_CASH_PRODUCT_INFO_ACK v;
        v.m_bFinal = Codec<std::remove_cvref_t<decltype(v.m_bFinal)>>::get(r);
        v.m_vecProductInfo = Codec<std::remove_cvref_t<decltype(v.m_vecProductInfo)>>::get(r);
        return v;
    }
};

struct KENX_BT_PRODUCT_INQUIRY_ACK {
    std::uint32_t                 m_ulPacketNo{};  // unsigned long
    std::uint8_t                  m_bytePacketType{};  // BYTE
    std::uint32_t                 m_ulResult{};  // unsigned long
    std::int64_t                  m_iReleaseTick{};  // __int64
    std::uint32_t                 m_ulTotalProductCount{};  // unsigned long
    std::vector<KNXBTProductInfo> m_vecNXBTProductInfo{};  // std::vector< KNXBTProductInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_ulPacketNo)>>::put(w, m_ulPacketNo);
        Codec<std::remove_cvref_t<decltype(m_bytePacketType)>>::put(w, m_bytePacketType);
        Codec<std::remove_cvref_t<decltype(m_ulResult)>>::put(w, m_ulResult);
        Codec<std::remove_cvref_t<decltype(m_iReleaseTick)>>::put(w, m_iReleaseTick);
        Codec<std::remove_cvref_t<decltype(m_ulTotalProductCount)>>::put(w, m_ulTotalProductCount);
        Codec<std::remove_cvref_t<decltype(m_vecNXBTProductInfo)>>::put(w, m_vecNXBTProductInfo);
    }

    static KENX_BT_PRODUCT_INQUIRY_ACK read_from(Reader& r) {
        KENX_BT_PRODUCT_INQUIRY_ACK v;
        v.m_ulPacketNo = Codec<std::remove_cvref_t<decltype(v.m_ulPacketNo)>>::get(r);
        v.m_bytePacketType = Codec<std::remove_cvref_t<decltype(v.m_bytePacketType)>>::get(r);
        v.m_ulResult = Codec<std::remove_cvref_t<decltype(v.m_ulResult)>>::get(r);
        v.m_iReleaseTick = Codec<std::remove_cvref_t<decltype(v.m_iReleaseTick)>>::get(r);
        v.m_ulTotalProductCount = Codec<std::remove_cvref_t<decltype(v.m_ulTotalProductCount)>>::get(r);
        v.m_vecNXBTProductInfo = Codec<std::remove_cvref_t<decltype(v.m_vecNXBTProductInfo)>>::get(r);
        return v;
    }
};

struct KNXBTPurchaseAckInfo {
    std::uint32_t  m_ulProductNo{};  // unsigned long
    std::uint16_t  m_usOrderQuantity{};  // unsigned short
    std::u16string m_wstrExtendValue{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_ulProductNo)>>::put(w, m_ulProductNo);
        Codec<std::remove_cvref_t<decltype(m_usOrderQuantity)>>::put(w, m_usOrderQuantity);
        Codec<std::remove_cvref_t<decltype(m_wstrExtendValue)>>::put(w, m_wstrExtendValue);
    }

    static KNXBTPurchaseAckInfo read_from(Reader& r) {
        KNXBTPurchaseAckInfo v;
        v.m_ulProductNo = Codec<std::remove_cvref_t<decltype(v.m_ulProductNo)>>::get(r);
        v.m_usOrderQuantity = Codec<std::remove_cvref_t<decltype(v.m_usOrderQuantity)>>::get(r);
        v.m_wstrExtendValue = Codec<std::remove_cvref_t<decltype(v.m_wstrExtendValue)>>::get(r);
        return v;
    }
};

struct KENX_BT_PURCHASE_GIFT_ACK {
    std::uint32_t                     m_ulPacketNo{};  // unsigned long
    std::uint8_t                      m_bytePacketType{};  // BYTE
    std::u16string                    m_wstrOrderID{};  // std::wstring
    std::uint32_t                     m_ulResult{};  // unsigned long
    std::uint32_t                     m_ulOrderNo{};  // unsigned long
    std::vector<KNXBTPurchaseAckInfo> m_vecNXBTPurchaseInfo{};  // std::vector< KNXBTPurchaseAckInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_ulPacketNo)>>::put(w, m_ulPacketNo);
        Codec<std::remove_cvref_t<decltype(m_bytePacketType)>>::put(w, m_bytePacketType);
        Codec<std::remove_cvref_t<decltype(m_wstrOrderID)>>::put(w, m_wstrOrderID);
        Codec<std::remove_cvref_t<decltype(m_ulResult)>>::put(w, m_ulResult);
        Codec<std::remove_cvref_t<decltype(m_ulOrderNo)>>::put(w, m_ulOrderNo);
        Codec<std::remove_cvref_t<decltype(m_vecNXBTPurchaseInfo)>>::put(w, m_vecNXBTPurchaseInfo);
    }

    static KENX_BT_PURCHASE_GIFT_ACK read_from(Reader& r) {
        KENX_BT_PURCHASE_GIFT_ACK v;
        v.m_ulPacketNo = Codec<std::remove_cvref_t<decltype(v.m_ulPacketNo)>>::get(r);
        v.m_bytePacketType = Codec<std::remove_cvref_t<decltype(v.m_bytePacketType)>>::get(r);
        v.m_wstrOrderID = Codec<std::remove_cvref_t<decltype(v.m_wstrOrderID)>>::get(r);
        v.m_ulResult = Codec<std::remove_cvref_t<decltype(v.m_ulResult)>>::get(r);
        v.m_ulOrderNo = Codec<std::remove_cvref_t<decltype(v.m_ulOrderNo)>>::get(r);
        v.m_vecNXBTPurchaseInfo = Codec<std::remove_cvref_t<decltype(v.m_vecNXBTPurchaseInfo)>>::get(r);
        return v;
    }
};

struct KENX_BT_PURCHASE_ITEM_ACK {
    std::uint32_t                     m_ulPacketNo{};  // unsigned long
    std::uint8_t                      m_bytePacketType{};  // BYTE
    std::u16string                    m_wstrOrderID{};  // std::wstring
    std::uint32_t                     m_ulResult{};  // unsigned long
    std::uint32_t                     m_ulOrderNo{};  // unsigned long
    std::vector<KNXBTPurchaseAckInfo> m_vecNXBTPurchaseInfo{};  // std::vector< KNXBTPurchaseAckInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_ulPacketNo)>>::put(w, m_ulPacketNo);
        Codec<std::remove_cvref_t<decltype(m_bytePacketType)>>::put(w, m_bytePacketType);
        Codec<std::remove_cvref_t<decltype(m_wstrOrderID)>>::put(w, m_wstrOrderID);
        Codec<std::remove_cvref_t<decltype(m_ulResult)>>::put(w, m_ulResult);
        Codec<std::remove_cvref_t<decltype(m_ulOrderNo)>>::put(w, m_ulOrderNo);
        Codec<std::remove_cvref_t<decltype(m_vecNXBTPurchaseInfo)>>::put(w, m_vecNXBTPurchaseInfo);
    }

    static KENX_BT_PURCHASE_ITEM_ACK read_from(Reader& r) {
        KENX_BT_PURCHASE_ITEM_ACK v;
        v.m_ulPacketNo = Codec<std::remove_cvref_t<decltype(v.m_ulPacketNo)>>::get(r);
        v.m_bytePacketType = Codec<std::remove_cvref_t<decltype(v.m_bytePacketType)>>::get(r);
        v.m_wstrOrderID = Codec<std::remove_cvref_t<decltype(v.m_wstrOrderID)>>::get(r);
        v.m_ulResult = Codec<std::remove_cvref_t<decltype(v.m_ulResult)>>::get(r);
        v.m_ulOrderNo = Codec<std::remove_cvref_t<decltype(v.m_ulOrderNo)>>::get(r);
        v.m_vecNXBTPurchaseInfo = Codec<std::remove_cvref_t<decltype(v.m_vecNXBTPurchaseInfo)>>::get(r);
        return v;
    }
};

struct KNXBTPurchaseReqInfo {
    std::uint32_t m_ulProductNo{};  // unsigned long
    std::uint16_t m_usOrderQuantity{};  // unsigned short

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_ulProductNo)>>::put(w, m_ulProductNo);
        Codec<std::remove_cvref_t<decltype(m_usOrderQuantity)>>::put(w, m_usOrderQuantity);
    }

    static KNXBTPurchaseReqInfo read_from(Reader& r) {
        KNXBTPurchaseReqInfo v;
        v.m_ulProductNo = Codec<std::remove_cvref_t<decltype(v.m_ulProductNo)>>::get(r);
        v.m_usOrderQuantity = Codec<std::remove_cvref_t<decltype(v.m_usOrderQuantity)>>::get(r);
        return v;
    }
};

struct KDBE_PRESENT_CASH_ITEM_CHECK_NICKNAME_ACK {
    std::int32_t                      m_iOK{};  // int
    std::u16string                    m_wstrUserID{};  // std::wstring
    std::u16string                    m_wstrMessage{};  // std::wstring
    std::vector<KNXBTPurchaseReqInfo> m_vecPurchaseReqInfo{};  // std::vector< KNXBTPurchaseReqInfo >
    std::int64_t                      m_iReceiverUserUID{};  // UidType
    std::int64_t                      m_iReceiverUnitUID{};  // UidType
    std::u16string                    m_wstrReceiverNickName{};  // std::wstring
    char                              m_cReceiverLevel{};
    std::int32_t                      m_iReceiverUnitClass{};  // int
    std::int32_t                      m_iUseCashType{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_wstrUserID)>>::put(w, m_wstrUserID);
        Codec<std::remove_cvref_t<decltype(m_wstrMessage)>>::put(w, m_wstrMessage);
        Codec<std::remove_cvref_t<decltype(m_vecPurchaseReqInfo)>>::put(w, m_vecPurchaseReqInfo);
        Codec<std::remove_cvref_t<decltype(m_iReceiverUserUID)>>::put(w, m_iReceiverUserUID);
        Codec<std::remove_cvref_t<decltype(m_iReceiverUnitUID)>>::put(w, m_iReceiverUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrReceiverNickName)>>::put(w, m_wstrReceiverNickName);
        Codec<std::remove_cvref_t<decltype(m_cReceiverLevel)>>::put(w, m_cReceiverLevel);
        Codec<std::remove_cvref_t<decltype(m_iReceiverUnitClass)>>::put(w, m_iReceiverUnitClass);
        Codec<std::remove_cvref_t<decltype(m_iUseCashType)>>::put(w, m_iUseCashType);
    }

    static KDBE_PRESENT_CASH_ITEM_CHECK_NICKNAME_ACK read_from(Reader& r) {
        KDBE_PRESENT_CASH_ITEM_CHECK_NICKNAME_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_wstrUserID = Codec<std::remove_cvref_t<decltype(v.m_wstrUserID)>>::get(r);
        v.m_wstrMessage = Codec<std::remove_cvref_t<decltype(v.m_wstrMessage)>>::get(r);
        v.m_vecPurchaseReqInfo = Codec<std::remove_cvref_t<decltype(v.m_vecPurchaseReqInfo)>>::get(r);
        v.m_iReceiverUserUID = Codec<std::remove_cvref_t<decltype(v.m_iReceiverUserUID)>>::get(r);
        v.m_iReceiverUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iReceiverUnitUID)>>::get(r);
        v.m_wstrReceiverNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrReceiverNickName)>>::get(r);
        v.m_cReceiverLevel = Codec<std::remove_cvref_t<decltype(v.m_cReceiverLevel)>>::get(r);
        v.m_iReceiverUnitClass = Codec<std::remove_cvref_t<decltype(v.m_iReceiverUnitClass)>>::get(r);
        v.m_iUseCashType = Codec<std::remove_cvref_t<decltype(v.m_iUseCashType)>>::get(r);
        return v;
    }
};

struct KDBE_PRESENT_CASH_ITEM_CHECK_NICKNAME_REQ {
    std::u16string                    m_wstrReceiverNickName{};  // std::wstring
    std::u16string                    m_wstrMessage{};  // std::wstring
    std::vector<KNXBTPurchaseReqInfo> m_vecPurchaseReqInfo{};  // std::vector< KNXBTPurchaseReqInfo >
    std::int32_t                      m_iUseCashType{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrReceiverNickName)>>::put(w, m_wstrReceiverNickName);
        Codec<std::remove_cvref_t<decltype(m_wstrMessage)>>::put(w, m_wstrMessage);
        Codec<std::remove_cvref_t<decltype(m_vecPurchaseReqInfo)>>::put(w, m_vecPurchaseReqInfo);
        Codec<std::remove_cvref_t<decltype(m_iUseCashType)>>::put(w, m_iUseCashType);
    }

    static KDBE_PRESENT_CASH_ITEM_CHECK_NICKNAME_REQ read_from(Reader& r) {
        KDBE_PRESENT_CASH_ITEM_CHECK_NICKNAME_REQ v;
        v.m_wstrReceiverNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrReceiverNickName)>>::get(r);
        v.m_wstrMessage = Codec<std::remove_cvref_t<decltype(v.m_wstrMessage)>>::get(r);
        v.m_vecPurchaseReqInfo = Codec<std::remove_cvref_t<decltype(v.m_vecPurchaseReqInfo)>>::get(r);
        v.m_iUseCashType = Codec<std::remove_cvref_t<decltype(v.m_iUseCashType)>>::get(r);
        return v;
    }
};

struct KEGS_BUY_CASH_ITEM_ACK {
    std::int32_t                      m_iOK{};  // int
    std::vector<KNXBTPurchaseReqInfo> m_vecPurchaseReqInfo{};  // std::vector< KNXBTPurchaseReqInfo >
    bool                              m_bIsWishList{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_vecPurchaseReqInfo)>>::put(w, m_vecPurchaseReqInfo);
        Codec<std::remove_cvref_t<decltype(m_bIsWishList)>>::put(w, m_bIsWishList);
    }

    static KEGS_BUY_CASH_ITEM_ACK read_from(Reader& r) {
        KEGS_BUY_CASH_ITEM_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_vecPurchaseReqInfo = Codec<std::remove_cvref_t<decltype(v.m_vecPurchaseReqInfo)>>::get(r);
        v.m_bIsWishList = Codec<std::remove_cvref_t<decltype(v.m_bIsWishList)>>::get(r);
        return v;
    }
};

struct KEGS_BUY_CASH_ITEM_REQ {
    std::vector<KNXBTPurchaseReqInfo> m_vecPurchaseReqInfo{};  // std::vector< KNXBTPurchaseReqInfo >
    bool                              m_bIsWishList{};
    std::int32_t                      m_iUseCashType{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecPurchaseReqInfo)>>::put(w, m_vecPurchaseReqInfo);
        Codec<std::remove_cvref_t<decltype(m_bIsWishList)>>::put(w, m_bIsWishList);
        Codec<std::remove_cvref_t<decltype(m_iUseCashType)>>::put(w, m_iUseCashType);
    }

    static KEGS_BUY_CASH_ITEM_REQ read_from(Reader& r) {
        KEGS_BUY_CASH_ITEM_REQ v;
        v.m_vecPurchaseReqInfo = Codec<std::remove_cvref_t<decltype(v.m_vecPurchaseReqInfo)>>::get(r);
        v.m_bIsWishList = Codec<std::remove_cvref_t<decltype(v.m_bIsWishList)>>::get(r);
        v.m_iUseCashType = Codec<std::remove_cvref_t<decltype(v.m_iUseCashType)>>::get(r);
        return v;
    }
};

struct KEGS_PRESENT_CASH_ITEM_REQ {
    std::u16string                    m_wstrPassword{};  // std::wstring
    std::u16string                    m_wstrReceiverNickName{};  // std::wstring
    std::u16string                    m_wstrMessage{};  // std::wstring
    std::vector<KNXBTPurchaseReqInfo> m_vecPurchaseReqInfo{};  // std::vector< KNXBTPurchaseReqInfo >
    bool                              m_bIsWishList{};
    std::int32_t                      m_iUseCashType{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrPassword)>>::put(w, m_wstrPassword);
        Codec<std::remove_cvref_t<decltype(m_wstrReceiverNickName)>>::put(w, m_wstrReceiverNickName);
        Codec<std::remove_cvref_t<decltype(m_wstrMessage)>>::put(w, m_wstrMessage);
        Codec<std::remove_cvref_t<decltype(m_vecPurchaseReqInfo)>>::put(w, m_vecPurchaseReqInfo);
        Codec<std::remove_cvref_t<decltype(m_bIsWishList)>>::put(w, m_bIsWishList);
        Codec<std::remove_cvref_t<decltype(m_iUseCashType)>>::put(w, m_iUseCashType);
    }

    static KEGS_PRESENT_CASH_ITEM_REQ read_from(Reader& r) {
        KEGS_PRESENT_CASH_ITEM_REQ v;
        v.m_wstrPassword = Codec<std::remove_cvref_t<decltype(v.m_wstrPassword)>>::get(r);
        v.m_wstrReceiverNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrReceiverNickName)>>::get(r);
        v.m_wstrMessage = Codec<std::remove_cvref_t<decltype(v.m_wstrMessage)>>::get(r);
        v.m_vecPurchaseReqInfo = Codec<std::remove_cvref_t<decltype(v.m_vecPurchaseReqInfo)>>::get(r);
        v.m_bIsWishList = Codec<std::remove_cvref_t<decltype(v.m_bIsWishList)>>::get(r);
        v.m_iUseCashType = Codec<std::remove_cvref_t<decltype(v.m_iUseCashType)>>::get(r);
        return v;
    }
};

struct KELG_PRESENT_CASH_ITEM_ACK {
    std::int32_t                      m_iOK{};  // int
    std::u16string                    m_wstrReceiverNickName{};  // std::wstring
    std::u16string                    m_wstrMessage{};  // std::wstring
    std::vector<KNXBTPurchaseReqInfo> m_vecPurchaseReqInfo{};  // std::vector< KNXBTPurchaseReqInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_wstrReceiverNickName)>>::put(w, m_wstrReceiverNickName);
        Codec<std::remove_cvref_t<decltype(m_wstrMessage)>>::put(w, m_wstrMessage);
        Codec<std::remove_cvref_t<decltype(m_vecPurchaseReqInfo)>>::put(w, m_vecPurchaseReqInfo);
    }

    static KELG_PRESENT_CASH_ITEM_ACK read_from(Reader& r) {
        KELG_PRESENT_CASH_ITEM_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_wstrReceiverNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrReceiverNickName)>>::get(r);
        v.m_wstrMessage = Codec<std::remove_cvref_t<decltype(v.m_wstrMessage)>>::get(r);
        v.m_vecPurchaseReqInfo = Codec<std::remove_cvref_t<decltype(v.m_vecPurchaseReqInfo)>>::get(r);
        return v;
    }
};

struct KELG_PRESENT_CASH_ITEM_REQ {
    std::uint32_t                     m_uiPwdHash{};  // UINT32
    std::u16string                    m_wstrPassword{};  // std::wstring
    std::u16string                    m_wstrReceiverNickName{};  // std::wstring
    std::u16string                    m_wstrMessage{};  // std::wstring
    std::vector<KNXBTPurchaseReqInfo> m_vecPurchaseReqInfo{};  // std::vector< KNXBTPurchaseReqInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_uiPwdHash)>>::put(w, m_uiPwdHash);
        Codec<std::remove_cvref_t<decltype(m_wstrPassword)>>::put(w, m_wstrPassword);
        Codec<std::remove_cvref_t<decltype(m_wstrReceiverNickName)>>::put(w, m_wstrReceiverNickName);
        Codec<std::remove_cvref_t<decltype(m_wstrMessage)>>::put(w, m_wstrMessage);
        Codec<std::remove_cvref_t<decltype(m_vecPurchaseReqInfo)>>::put(w, m_vecPurchaseReqInfo);
    }

    static KELG_PRESENT_CASH_ITEM_REQ read_from(Reader& r) {
        KELG_PRESENT_CASH_ITEM_REQ v;
        v.m_uiPwdHash = Codec<std::remove_cvref_t<decltype(v.m_uiPwdHash)>>::get(r);
        v.m_wstrPassword = Codec<std::remove_cvref_t<decltype(v.m_wstrPassword)>>::get(r);
        v.m_wstrReceiverNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrReceiverNickName)>>::get(r);
        v.m_wstrMessage = Codec<std::remove_cvref_t<decltype(v.m_wstrMessage)>>::get(r);
        v.m_vecPurchaseReqInfo = Codec<std::remove_cvref_t<decltype(v.m_vecPurchaseReqInfo)>>::get(r);
        return v;
    }
};

struct KENX_BT_PURCHASE_GIFT_REQ {
    std::uint32_t                     m_ulPacketNo{};  // unsigned long
    std::uint8_t                      m_bytePacketType{};  // BYTE
    std::uint32_t                     m_ulRemoteIP{};  // unsigned long
    std::uint8_t                      m_byteReason{};  // BYTE
    std::u16string                    m_wstrSenderGameID{};  // std::wstring
    std::u16string                    m_wstrSenderUserID{};  // std::wstring
    std::uint32_t                     m_ulSenderUserOID{};  // unsigned long
    std::u16string                    m_wstrSenderUserName{};  // std::wstring
    std::uint8_t                      m_byteSenderUserAge{};  // BYTE
    std::uint8_t                      m_byteReceiverServerNo{};  // BYTE
    std::u16string                    m_wstrReceiverGameID{};  // std::wstring
    std::u16string                    m_wstrMessage{};  // std::wstring
    std::u16string                    m_wstrOrderID{};  // std::wstring
    std::uint32_t                     m_ulPaymentType{};  // unsigned long
    std::uint32_t                     m_ulTotalAmount{};  // unsigned long
    std::vector<KNXBTPurchaseReqInfo> m_vecNXBTPurchaseInfo{};  // std::vector< KNXBTPurchaseReqInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_ulPacketNo)>>::put(w, m_ulPacketNo);
        Codec<std::remove_cvref_t<decltype(m_bytePacketType)>>::put(w, m_bytePacketType);
        Codec<std::remove_cvref_t<decltype(m_ulRemoteIP)>>::put(w, m_ulRemoteIP);
        Codec<std::remove_cvref_t<decltype(m_byteReason)>>::put(w, m_byteReason);
        Codec<std::remove_cvref_t<decltype(m_wstrSenderGameID)>>::put(w, m_wstrSenderGameID);
        Codec<std::remove_cvref_t<decltype(m_wstrSenderUserID)>>::put(w, m_wstrSenderUserID);
        Codec<std::remove_cvref_t<decltype(m_ulSenderUserOID)>>::put(w, m_ulSenderUserOID);
        Codec<std::remove_cvref_t<decltype(m_wstrSenderUserName)>>::put(w, m_wstrSenderUserName);
        Codec<std::remove_cvref_t<decltype(m_byteSenderUserAge)>>::put(w, m_byteSenderUserAge);
        Codec<std::remove_cvref_t<decltype(m_byteReceiverServerNo)>>::put(w, m_byteReceiverServerNo);
        Codec<std::remove_cvref_t<decltype(m_wstrReceiverGameID)>>::put(w, m_wstrReceiverGameID);
        Codec<std::remove_cvref_t<decltype(m_wstrMessage)>>::put(w, m_wstrMessage);
        Codec<std::remove_cvref_t<decltype(m_wstrOrderID)>>::put(w, m_wstrOrderID);
        Codec<std::remove_cvref_t<decltype(m_ulPaymentType)>>::put(w, m_ulPaymentType);
        Codec<std::remove_cvref_t<decltype(m_ulTotalAmount)>>::put(w, m_ulTotalAmount);
        Codec<std::remove_cvref_t<decltype(m_vecNXBTPurchaseInfo)>>::put(w, m_vecNXBTPurchaseInfo);
    }

    static KENX_BT_PURCHASE_GIFT_REQ read_from(Reader& r) {
        KENX_BT_PURCHASE_GIFT_REQ v;
        v.m_ulPacketNo = Codec<std::remove_cvref_t<decltype(v.m_ulPacketNo)>>::get(r);
        v.m_bytePacketType = Codec<std::remove_cvref_t<decltype(v.m_bytePacketType)>>::get(r);
        v.m_ulRemoteIP = Codec<std::remove_cvref_t<decltype(v.m_ulRemoteIP)>>::get(r);
        v.m_byteReason = Codec<std::remove_cvref_t<decltype(v.m_byteReason)>>::get(r);
        v.m_wstrSenderGameID = Codec<std::remove_cvref_t<decltype(v.m_wstrSenderGameID)>>::get(r);
        v.m_wstrSenderUserID = Codec<std::remove_cvref_t<decltype(v.m_wstrSenderUserID)>>::get(r);
        v.m_ulSenderUserOID = Codec<std::remove_cvref_t<decltype(v.m_ulSenderUserOID)>>::get(r);
        v.m_wstrSenderUserName = Codec<std::remove_cvref_t<decltype(v.m_wstrSenderUserName)>>::get(r);
        v.m_byteSenderUserAge = Codec<std::remove_cvref_t<decltype(v.m_byteSenderUserAge)>>::get(r);
        v.m_byteReceiverServerNo = Codec<std::remove_cvref_t<decltype(v.m_byteReceiverServerNo)>>::get(r);
        v.m_wstrReceiverGameID = Codec<std::remove_cvref_t<decltype(v.m_wstrReceiverGameID)>>::get(r);
        v.m_wstrMessage = Codec<std::remove_cvref_t<decltype(v.m_wstrMessage)>>::get(r);
        v.m_wstrOrderID = Codec<std::remove_cvref_t<decltype(v.m_wstrOrderID)>>::get(r);
        v.m_ulPaymentType = Codec<std::remove_cvref_t<decltype(v.m_ulPaymentType)>>::get(r);
        v.m_ulTotalAmount = Codec<std::remove_cvref_t<decltype(v.m_ulTotalAmount)>>::get(r);
        v.m_vecNXBTPurchaseInfo = Codec<std::remove_cvref_t<decltype(v.m_vecNXBTPurchaseInfo)>>::get(r);
        return v;
    }
};

struct KENX_BT_PURCHASE_ITEM_REQ {
    std::uint32_t                     m_ulPacketNo{};  // unsigned long
    std::uint8_t                      m_bytePacketType{};  // BYTE
    std::uint32_t                     m_ulRemoteIP{};  // unsigned long
    std::uint8_t                      m_byteReason{};  // BYTE
    std::u16string                    m_wstrGameID{};  // std::wstring
    std::u16string                    m_wstrUserID{};  // std::wstring
    std::uint32_t                     m_ulUserOID{};  // unsigned long
    std::u16string                    m_wstrUserName{};  // std::wstring
    std::uint8_t                      m_byteUserAge{};  // BYTE
    std::u16string                    m_wstrOrderID{};  // std::wstring
    std::uint32_t                     m_ulPaymentType{};  // unsigned long
    std::uint32_t                     m_ulTotalAmount{};  // unsigned long
    std::vector<KNXBTPurchaseReqInfo> m_vecNXBTPurchaseInfo{};  // std::vector< KNXBTPurchaseReqInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_ulPacketNo)>>::put(w, m_ulPacketNo);
        Codec<std::remove_cvref_t<decltype(m_bytePacketType)>>::put(w, m_bytePacketType);
        Codec<std::remove_cvref_t<decltype(m_ulRemoteIP)>>::put(w, m_ulRemoteIP);
        Codec<std::remove_cvref_t<decltype(m_byteReason)>>::put(w, m_byteReason);
        Codec<std::remove_cvref_t<decltype(m_wstrGameID)>>::put(w, m_wstrGameID);
        Codec<std::remove_cvref_t<decltype(m_wstrUserID)>>::put(w, m_wstrUserID);
        Codec<std::remove_cvref_t<decltype(m_ulUserOID)>>::put(w, m_ulUserOID);
        Codec<std::remove_cvref_t<decltype(m_wstrUserName)>>::put(w, m_wstrUserName);
        Codec<std::remove_cvref_t<decltype(m_byteUserAge)>>::put(w, m_byteUserAge);
        Codec<std::remove_cvref_t<decltype(m_wstrOrderID)>>::put(w, m_wstrOrderID);
        Codec<std::remove_cvref_t<decltype(m_ulPaymentType)>>::put(w, m_ulPaymentType);
        Codec<std::remove_cvref_t<decltype(m_ulTotalAmount)>>::put(w, m_ulTotalAmount);
        Codec<std::remove_cvref_t<decltype(m_vecNXBTPurchaseInfo)>>::put(w, m_vecNXBTPurchaseInfo);
    }

    static KENX_BT_PURCHASE_ITEM_REQ read_from(Reader& r) {
        KENX_BT_PURCHASE_ITEM_REQ v;
        v.m_ulPacketNo = Codec<std::remove_cvref_t<decltype(v.m_ulPacketNo)>>::get(r);
        v.m_bytePacketType = Codec<std::remove_cvref_t<decltype(v.m_bytePacketType)>>::get(r);
        v.m_ulRemoteIP = Codec<std::remove_cvref_t<decltype(v.m_ulRemoteIP)>>::get(r);
        v.m_byteReason = Codec<std::remove_cvref_t<decltype(v.m_byteReason)>>::get(r);
        v.m_wstrGameID = Codec<std::remove_cvref_t<decltype(v.m_wstrGameID)>>::get(r);
        v.m_wstrUserID = Codec<std::remove_cvref_t<decltype(v.m_wstrUserID)>>::get(r);
        v.m_ulUserOID = Codec<std::remove_cvref_t<decltype(v.m_ulUserOID)>>::get(r);
        v.m_wstrUserName = Codec<std::remove_cvref_t<decltype(v.m_wstrUserName)>>::get(r);
        v.m_byteUserAge = Codec<std::remove_cvref_t<decltype(v.m_byteUserAge)>>::get(r);
        v.m_wstrOrderID = Codec<std::remove_cvref_t<decltype(v.m_wstrOrderID)>>::get(r);
        v.m_ulPaymentType = Codec<std::remove_cvref_t<decltype(v.m_ulPaymentType)>>::get(r);
        v.m_ulTotalAmount = Codec<std::remove_cvref_t<decltype(v.m_ulTotalAmount)>>::get(r);
        v.m_vecNXBTPurchaseInfo = Codec<std::remove_cvref_t<decltype(v.m_vecNXBTPurchaseInfo)>>::get(r);
        return v;
    }
};

struct KNXBTSubProductInfo {
    std::uint32_t  m_ulProductNo{};  // unsigned long
    std::u16string m_wstrProductName{};  // std::wstring
    std::u16string m_wstrProductID{};  // std::wstring
    std::int16_t   m_sProductExpire{};  // short
    std::uint16_t  m_usProductPieces{};  // unsigned short
    std::u16string m_wstrProductAttribute0{};  // std::wstring
    std::u16string m_wstrProductAttribute1{};  // std::wstring
    std::u16string m_wstrProductAttribute2{};  // std::wstring
    std::u16string m_wstrProductAttribute3{};  // std::wstring
    std::u16string m_wstrProductAttribute4{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_ulProductNo)>>::put(w, m_ulProductNo);
        Codec<std::remove_cvref_t<decltype(m_wstrProductName)>>::put(w, m_wstrProductName);
        Codec<std::remove_cvref_t<decltype(m_wstrProductID)>>::put(w, m_wstrProductID);
        Codec<std::remove_cvref_t<decltype(m_sProductExpire)>>::put(w, m_sProductExpire);
        Codec<std::remove_cvref_t<decltype(m_usProductPieces)>>::put(w, m_usProductPieces);
        Codec<std::remove_cvref_t<decltype(m_wstrProductAttribute0)>>::put(w, m_wstrProductAttribute0);
        Codec<std::remove_cvref_t<decltype(m_wstrProductAttribute1)>>::put(w, m_wstrProductAttribute1);
        Codec<std::remove_cvref_t<decltype(m_wstrProductAttribute2)>>::put(w, m_wstrProductAttribute2);
        Codec<std::remove_cvref_t<decltype(m_wstrProductAttribute3)>>::put(w, m_wstrProductAttribute3);
        Codec<std::remove_cvref_t<decltype(m_wstrProductAttribute4)>>::put(w, m_wstrProductAttribute4);
    }

    static KNXBTSubProductInfo read_from(Reader& r) {
        KNXBTSubProductInfo v;
        v.m_ulProductNo = Codec<std::remove_cvref_t<decltype(v.m_ulProductNo)>>::get(r);
        v.m_wstrProductName = Codec<std::remove_cvref_t<decltype(v.m_wstrProductName)>>::get(r);
        v.m_wstrProductID = Codec<std::remove_cvref_t<decltype(v.m_wstrProductID)>>::get(r);
        v.m_sProductExpire = Codec<std::remove_cvref_t<decltype(v.m_sProductExpire)>>::get(r);
        v.m_usProductPieces = Codec<std::remove_cvref_t<decltype(v.m_usProductPieces)>>::get(r);
        v.m_wstrProductAttribute0 = Codec<std::remove_cvref_t<decltype(v.m_wstrProductAttribute0)>>::get(r);
        v.m_wstrProductAttribute1 = Codec<std::remove_cvref_t<decltype(v.m_wstrProductAttribute1)>>::get(r);
        v.m_wstrProductAttribute2 = Codec<std::remove_cvref_t<decltype(v.m_wstrProductAttribute2)>>::get(r);
        v.m_wstrProductAttribute3 = Codec<std::remove_cvref_t<decltype(v.m_wstrProductAttribute3)>>::get(r);
        v.m_wstrProductAttribute4 = Codec<std::remove_cvref_t<decltype(v.m_wstrProductAttribute4)>>::get(r);
        return v;
    }
};

struct KENX_BT_NISMS_INVENTORY_PICK_UP_ONCE_ACK {
    std::uint32_t                            m_ulPacketNo{};  // unsigned long
    std::uint8_t                             m_bytePacketType{};  // BYTE
    std::uint32_t                            m_ulResult{};  // unsigned long
    std::uint32_t                            m_ulOrderNo{};  // unsigned long
    std::uint32_t                            m_ulProductNo{};  // unsigned long
    std::uint8_t                             m_byteProductKind{};  // BYTE
    std::u16string                           m_wstrProductName{};  // std::wstring
    std::u16string                           m_wstrProductID{};  // std::wstring
    std::uint16_t                            m_usProductExpire{};  // unsigned short
    std::uint16_t                            m_usProductPieces{};  // unsigned short
    std::uint16_t                            m_usOrderQuantity{};  // unsigned short
    std::u16string                           m_wstrProductAttribute0{};  // std::wstring
    std::u16string                           m_wstrProductAttribute1{};  // std::wstring
    std::u16string                           m_wstrProductAttribute2{};  // std::wstring
    std::u16string                           m_wstrProductAttribute3{};  // std::wstring
    std::u16string                           m_wstrProductAttribute4{};  // std::wstring
    std::u16string                           m_wstrExtendValue{};  // std::wstring
    std::vector<KNXBTSubProductInfo>         m_vecNXBTSubProductInfo{};  // std::vector< KNXBTSubProductInfo >
    std::vector<KNXBTBonusProductInfoPickUp> m_vecKNXBTBonusProductInfo{};  // std::vector< KNXBTBonusProductInfoPickUp >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_ulPacketNo)>>::put(w, m_ulPacketNo);
        Codec<std::remove_cvref_t<decltype(m_bytePacketType)>>::put(w, m_bytePacketType);
        Codec<std::remove_cvref_t<decltype(m_ulResult)>>::put(w, m_ulResult);
        Codec<std::remove_cvref_t<decltype(m_ulOrderNo)>>::put(w, m_ulOrderNo);
        Codec<std::remove_cvref_t<decltype(m_ulProductNo)>>::put(w, m_ulProductNo);
        Codec<std::remove_cvref_t<decltype(m_byteProductKind)>>::put(w, m_byteProductKind);
        Codec<std::remove_cvref_t<decltype(m_wstrProductName)>>::put(w, m_wstrProductName);
        Codec<std::remove_cvref_t<decltype(m_wstrProductID)>>::put(w, m_wstrProductID);
        Codec<std::remove_cvref_t<decltype(m_usProductExpire)>>::put(w, m_usProductExpire);
        Codec<std::remove_cvref_t<decltype(m_usProductPieces)>>::put(w, m_usProductPieces);
        Codec<std::remove_cvref_t<decltype(m_usOrderQuantity)>>::put(w, m_usOrderQuantity);
        Codec<std::remove_cvref_t<decltype(m_wstrProductAttribute0)>>::put(w, m_wstrProductAttribute0);
        Codec<std::remove_cvref_t<decltype(m_wstrProductAttribute1)>>::put(w, m_wstrProductAttribute1);
        Codec<std::remove_cvref_t<decltype(m_wstrProductAttribute2)>>::put(w, m_wstrProductAttribute2);
        Codec<std::remove_cvref_t<decltype(m_wstrProductAttribute3)>>::put(w, m_wstrProductAttribute3);
        Codec<std::remove_cvref_t<decltype(m_wstrProductAttribute4)>>::put(w, m_wstrProductAttribute4);
        Codec<std::remove_cvref_t<decltype(m_wstrExtendValue)>>::put(w, m_wstrExtendValue);
        Codec<std::remove_cvref_t<decltype(m_vecNXBTSubProductInfo)>>::put(w, m_vecNXBTSubProductInfo);
        Codec<std::remove_cvref_t<decltype(m_vecKNXBTBonusProductInfo)>>::put(w, m_vecKNXBTBonusProductInfo);
    }

    static KENX_BT_NISMS_INVENTORY_PICK_UP_ONCE_ACK read_from(Reader& r) {
        KENX_BT_NISMS_INVENTORY_PICK_UP_ONCE_ACK v;
        v.m_ulPacketNo = Codec<std::remove_cvref_t<decltype(v.m_ulPacketNo)>>::get(r);
        v.m_bytePacketType = Codec<std::remove_cvref_t<decltype(v.m_bytePacketType)>>::get(r);
        v.m_ulResult = Codec<std::remove_cvref_t<decltype(v.m_ulResult)>>::get(r);
        v.m_ulOrderNo = Codec<std::remove_cvref_t<decltype(v.m_ulOrderNo)>>::get(r);
        v.m_ulProductNo = Codec<std::remove_cvref_t<decltype(v.m_ulProductNo)>>::get(r);
        v.m_byteProductKind = Codec<std::remove_cvref_t<decltype(v.m_byteProductKind)>>::get(r);
        v.m_wstrProductName = Codec<std::remove_cvref_t<decltype(v.m_wstrProductName)>>::get(r);
        v.m_wstrProductID = Codec<std::remove_cvref_t<decltype(v.m_wstrProductID)>>::get(r);
        v.m_usProductExpire = Codec<std::remove_cvref_t<decltype(v.m_usProductExpire)>>::get(r);
        v.m_usProductPieces = Codec<std::remove_cvref_t<decltype(v.m_usProductPieces)>>::get(r);
        v.m_usOrderQuantity = Codec<std::remove_cvref_t<decltype(v.m_usOrderQuantity)>>::get(r);
        v.m_wstrProductAttribute0 = Codec<std::remove_cvref_t<decltype(v.m_wstrProductAttribute0)>>::get(r);
        v.m_wstrProductAttribute1 = Codec<std::remove_cvref_t<decltype(v.m_wstrProductAttribute1)>>::get(r);
        v.m_wstrProductAttribute2 = Codec<std::remove_cvref_t<decltype(v.m_wstrProductAttribute2)>>::get(r);
        v.m_wstrProductAttribute3 = Codec<std::remove_cvref_t<decltype(v.m_wstrProductAttribute3)>>::get(r);
        v.m_wstrProductAttribute4 = Codec<std::remove_cvref_t<decltype(v.m_wstrProductAttribute4)>>::get(r);
        v.m_wstrExtendValue = Codec<std::remove_cvref_t<decltype(v.m_wstrExtendValue)>>::get(r);
        v.m_vecNXBTSubProductInfo = Codec<std::remove_cvref_t<decltype(v.m_vecNXBTSubProductInfo)>>::get(r);
        v.m_vecKNXBTBonusProductInfo = Codec<std::remove_cvref_t<decltype(v.m_vecKNXBTBonusProductInfo)>>::get(r);
        return v;
    }
};

struct KNetAddress {
    std::u16string m_wstrIP{};  // std::wstring
    std::uint16_t  m_usPort{};  // unsigned short

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrIP)>>::put(w, m_wstrIP);
        Codec<std::remove_cvref_t<decltype(m_usPort)>>::put(w, m_usPort);
    }

    static KNetAddress read_from(Reader& r) {
        KNetAddress v;
        v.m_wstrIP = Codec<std::remove_cvref_t<decltype(v.m_wstrIP)>>::get(r);
        v.m_usPort = Codec<std::remove_cvref_t<decltype(v.m_usPort)>>::get(r);
        return v;
    }
};

struct KECN_VERIFY_SERVER_CONNECT_ACK {
    std::int32_t   m_nOK{};  // int
    std::int64_t   m_nUID{};  // UidType
    std::int32_t   m_iDBRegServerGroupID{};  // int
    std::int32_t   m_iLocalServerGroupID{};  // int
    std::int64_t   m_iServerUID{};  // UidType
    std::u16string m_strName{};  // std::wstring
    std::int16_t   m_sMaxNum{};  // short
    KNetAddress    m_kAddr{};
    std::int32_t   m_iVersion{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_nOK)>>::put(w, m_nOK);
        Codec<std::remove_cvref_t<decltype(m_nUID)>>::put(w, m_nUID);
        Codec<std::remove_cvref_t<decltype(m_iDBRegServerGroupID)>>::put(w, m_iDBRegServerGroupID);
        Codec<std::remove_cvref_t<decltype(m_iLocalServerGroupID)>>::put(w, m_iLocalServerGroupID);
        Codec<std::remove_cvref_t<decltype(m_iServerUID)>>::put(w, m_iServerUID);
        Codec<std::remove_cvref_t<decltype(m_strName)>>::put(w, m_strName);
        Codec<std::remove_cvref_t<decltype(m_sMaxNum)>>::put(w, m_sMaxNum);
        Codec<std::remove_cvref_t<decltype(m_kAddr)>>::put(w, m_kAddr);
        Codec<std::remove_cvref_t<decltype(m_iVersion)>>::put(w, m_iVersion);
    }

    static KECN_VERIFY_SERVER_CONNECT_ACK read_from(Reader& r) {
        KECN_VERIFY_SERVER_CONNECT_ACK v;
        v.m_nOK = Codec<std::remove_cvref_t<decltype(v.m_nOK)>>::get(r);
        v.m_nUID = Codec<std::remove_cvref_t<decltype(v.m_nUID)>>::get(r);
        v.m_iDBRegServerGroupID = Codec<std::remove_cvref_t<decltype(v.m_iDBRegServerGroupID)>>::get(r);
        v.m_iLocalServerGroupID = Codec<std::remove_cvref_t<decltype(v.m_iLocalServerGroupID)>>::get(r);
        v.m_iServerUID = Codec<std::remove_cvref_t<decltype(v.m_iServerUID)>>::get(r);
        v.m_strName = Codec<std::remove_cvref_t<decltype(v.m_strName)>>::get(r);
        v.m_sMaxNum = Codec<std::remove_cvref_t<decltype(v.m_sMaxNum)>>::get(r);
        v.m_kAddr = Codec<std::remove_cvref_t<decltype(v.m_kAddr)>>::get(r);
        v.m_iVersion = Codec<std::remove_cvref_t<decltype(v.m_iVersion)>>::get(r);
        return v;
    }
};

struct KECN_VERIFY_SERVER_CONNECT_REQ {
    KNetAddress  m_kNetAddress{};
    std::int32_t m_iServerGroupID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kNetAddress)>>::put(w, m_kNetAddress);
        Codec<std::remove_cvref_t<decltype(m_iServerGroupID)>>::put(w, m_iServerGroupID);
    }

    static KECN_VERIFY_SERVER_CONNECT_REQ read_from(Reader& r) {
        KECN_VERIFY_SERVER_CONNECT_REQ v;
        v.m_kNetAddress = Codec<std::remove_cvref_t<decltype(v.m_kNetAddress)>>::get(r);
        v.m_iServerGroupID = Codec<std::remove_cvref_t<decltype(v.m_iServerGroupID)>>::get(r);
        return v;
    }
};

struct KNexonAccountInfo {
    std::u16string m_wstrID{};  // std::wstring
    std::uint32_t  m_uiNexonSN{};  // UINT32
    std::u16string m_wstrLoginIP{};  // std::wstring
    bool           m_bSex{};
    std::uint32_t  m_uiAge{};  // UINT32
    std::uint32_t  m_uiPwdHash{};  // UINT32
    std::uint32_t  m_uiSsnHash{};  // UINT32
    std::uint32_t  m_uiFlag0{};  // UINT32
    std::uint32_t  m_uiFlag1{};  // UINT32
    bool           m_bNewMembership{};
    std::uint8_t   m_uMainAuthLevel{};  // UINT8
    std::uint8_t   m_uSubAuthLevel{};  // UINT8
    std::u16string m_wstrNationCode{};  // std::wstring
    std::u16string m_wstrMeta{};  // std::wstring
    std::uint8_t   m_uSecureCode{};  // UINT8
    std::uint8_t   m_uChannelCode{};  // UINT8
    std::u16string m_wstrChannelUID{};  // std::wstring
    std::uint8_t   m_byteGuestUser{};  // byte

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrID)>>::put(w, m_wstrID);
        Codec<std::remove_cvref_t<decltype(m_uiNexonSN)>>::put(w, m_uiNexonSN);
        Codec<std::remove_cvref_t<decltype(m_wstrLoginIP)>>::put(w, m_wstrLoginIP);
        Codec<std::remove_cvref_t<decltype(m_bSex)>>::put(w, m_bSex);
        Codec<std::remove_cvref_t<decltype(m_uiAge)>>::put(w, m_uiAge);
        Codec<std::remove_cvref_t<decltype(m_uiPwdHash)>>::put(w, m_uiPwdHash);
        Codec<std::remove_cvref_t<decltype(m_uiSsnHash)>>::put(w, m_uiSsnHash);
        Codec<std::remove_cvref_t<decltype(m_uiFlag0)>>::put(w, m_uiFlag0);
        Codec<std::remove_cvref_t<decltype(m_uiFlag1)>>::put(w, m_uiFlag1);
        Codec<std::remove_cvref_t<decltype(m_bNewMembership)>>::put(w, m_bNewMembership);
        Codec<std::remove_cvref_t<decltype(m_uMainAuthLevel)>>::put(w, m_uMainAuthLevel);
        Codec<std::remove_cvref_t<decltype(m_uSubAuthLevel)>>::put(w, m_uSubAuthLevel);
        Codec<std::remove_cvref_t<decltype(m_wstrNationCode)>>::put(w, m_wstrNationCode);
        Codec<std::remove_cvref_t<decltype(m_wstrMeta)>>::put(w, m_wstrMeta);
        Codec<std::remove_cvref_t<decltype(m_uSecureCode)>>::put(w, m_uSecureCode);
        Codec<std::remove_cvref_t<decltype(m_uChannelCode)>>::put(w, m_uChannelCode);
        Codec<std::remove_cvref_t<decltype(m_wstrChannelUID)>>::put(w, m_wstrChannelUID);
        Codec<std::remove_cvref_t<decltype(m_byteGuestUser)>>::put(w, m_byteGuestUser);
    }

    static KNexonAccountInfo read_from(Reader& r) {
        KNexonAccountInfo v;
        v.m_wstrID = Codec<std::remove_cvref_t<decltype(v.m_wstrID)>>::get(r);
        v.m_uiNexonSN = Codec<std::remove_cvref_t<decltype(v.m_uiNexonSN)>>::get(r);
        v.m_wstrLoginIP = Codec<std::remove_cvref_t<decltype(v.m_wstrLoginIP)>>::get(r);
        v.m_bSex = Codec<std::remove_cvref_t<decltype(v.m_bSex)>>::get(r);
        v.m_uiAge = Codec<std::remove_cvref_t<decltype(v.m_uiAge)>>::get(r);
        v.m_uiPwdHash = Codec<std::remove_cvref_t<decltype(v.m_uiPwdHash)>>::get(r);
        v.m_uiSsnHash = Codec<std::remove_cvref_t<decltype(v.m_uiSsnHash)>>::get(r);
        v.m_uiFlag0 = Codec<std::remove_cvref_t<decltype(v.m_uiFlag0)>>::get(r);
        v.m_uiFlag1 = Codec<std::remove_cvref_t<decltype(v.m_uiFlag1)>>::get(r);
        v.m_bNewMembership = Codec<std::remove_cvref_t<decltype(v.m_bNewMembership)>>::get(r);
        v.m_uMainAuthLevel = Codec<std::remove_cvref_t<decltype(v.m_uMainAuthLevel)>>::get(r);
        v.m_uSubAuthLevel = Codec<std::remove_cvref_t<decltype(v.m_uSubAuthLevel)>>::get(r);
        v.m_wstrNationCode = Codec<std::remove_cvref_t<decltype(v.m_wstrNationCode)>>::get(r);
        v.m_wstrMeta = Codec<std::remove_cvref_t<decltype(v.m_wstrMeta)>>::get(r);
        v.m_uSecureCode = Codec<std::remove_cvref_t<decltype(v.m_uSecureCode)>>::get(r);
        v.m_uChannelCode = Codec<std::remove_cvref_t<decltype(v.m_uChannelCode)>>::get(r);
        v.m_wstrChannelUID = Codec<std::remove_cvref_t<decltype(v.m_wstrChannelUID)>>::get(r);
        v.m_byteGuestUser = Codec<std::remove_cvref_t<decltype(v.m_byteGuestUser)>>::get(r);
        return v;
    }
};

struct KELG_GAME_USER_AUTHENTICATE_ACK {
    std::int32_t                         m_iOK{};  // int
    KAccountInfo                         m_kAccountInfo{};
    KNexonAccountInfo                    m_kNexonAccountInfo{};
    char                                 m_cHackingUserType{};
    std::uint32_t                        m_uiGiantAccountType{};  // unsigned int
    std::map<std::int32_t, std::int32_t> m_mapAccCountInfo{};  // std::map< int, int >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_kAccountInfo)>>::put(w, m_kAccountInfo);
        Codec<std::remove_cvref_t<decltype(m_kNexonAccountInfo)>>::put(w, m_kNexonAccountInfo);
        Codec<std::remove_cvref_t<decltype(m_cHackingUserType)>>::put(w, m_cHackingUserType);
        Codec<std::remove_cvref_t<decltype(m_uiGiantAccountType)>>::put(w, m_uiGiantAccountType);
        Codec<std::remove_cvref_t<decltype(m_mapAccCountInfo)>>::put(w, m_mapAccCountInfo);
    }

    static KELG_GAME_USER_AUTHENTICATE_ACK read_from(Reader& r) {
        KELG_GAME_USER_AUTHENTICATE_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_kAccountInfo = Codec<std::remove_cvref_t<decltype(v.m_kAccountInfo)>>::get(r);
        v.m_kNexonAccountInfo = Codec<std::remove_cvref_t<decltype(v.m_kNexonAccountInfo)>>::get(r);
        v.m_cHackingUserType = Codec<std::remove_cvref_t<decltype(v.m_cHackingUserType)>>::get(r);
        v.m_uiGiantAccountType = Codec<std::remove_cvref_t<decltype(v.m_uiGiantAccountType)>>::get(r);
        v.m_mapAccCountInfo = Codec<std::remove_cvref_t<decltype(v.m_mapAccCountInfo)>>::get(r);
        return v;
    }
};

struct KELG_NEXON_USER_AUTHENTICATE_ACK {
    std::int32_t                         m_iOK{};  // int
    KAccountInfo                         m_kAccountInfo{};
    KNexonAccountInfo                    m_kNexonAccountInfo{};
    char                                 m_cHackingUserType{};
    bool                                 m_bJoinAccount{};
    std::map<std::int32_t, std::int32_t> m_mapAccCountInfo{};  // std::map< int, int >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_kAccountInfo)>>::put(w, m_kAccountInfo);
        Codec<std::remove_cvref_t<decltype(m_kNexonAccountInfo)>>::put(w, m_kNexonAccountInfo);
        Codec<std::remove_cvref_t<decltype(m_cHackingUserType)>>::put(w, m_cHackingUserType);
        Codec<std::remove_cvref_t<decltype(m_bJoinAccount)>>::put(w, m_bJoinAccount);
        Codec<std::remove_cvref_t<decltype(m_mapAccCountInfo)>>::put(w, m_mapAccCountInfo);
    }

    static KELG_NEXON_USER_AUTHENTICATE_ACK read_from(Reader& r) {
        KELG_NEXON_USER_AUTHENTICATE_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_kAccountInfo = Codec<std::remove_cvref_t<decltype(v.m_kAccountInfo)>>::get(r);
        v.m_kNexonAccountInfo = Codec<std::remove_cvref_t<decltype(v.m_kNexonAccountInfo)>>::get(r);
        v.m_cHackingUserType = Codec<std::remove_cvref_t<decltype(v.m_cHackingUserType)>>::get(r);
        v.m_bJoinAccount = Codec<std::remove_cvref_t<decltype(v.m_bJoinAccount)>>::get(r);
        v.m_mapAccCountInfo = Codec<std::remove_cvref_t<decltype(v.m_mapAccCountInfo)>>::get(r);
        return v;
    }
};

struct KNpcUnitBuffInfo {
    std::int32_t             m_iNpcUID{};  // int
    std::vector<KBuffFactor> m_vecBuffFactor{};  // std::vector< KBuffFactor >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iNpcUID)>>::put(w, m_iNpcUID);
        Codec<std::remove_cvref_t<decltype(m_vecBuffFactor)>>::put(w, m_vecBuffFactor);
    }

    static KNpcUnitBuffInfo read_from(Reader& r) {
        KNpcUnitBuffInfo v;
        v.m_iNpcUID = Codec<std::remove_cvref_t<decltype(v.m_iNpcUID)>>::get(r);
        v.m_vecBuffFactor = Codec<std::remove_cvref_t<decltype(v.m_vecBuffFactor)>>::get(r);
        return v;
    }
};

struct KEGS_UPDATE_NPC_UNIT_BUFF_INFO_BROAD_NOT {
    std::vector<KNpcUnitBuffInfo> m_vecNpcUnitBuff{};  // std::vector< KNpcUnitBuffInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecNpcUnitBuff)>>::put(w, m_vecNpcUnitBuff);
    }

    static KEGS_UPDATE_NPC_UNIT_BUFF_INFO_BROAD_NOT read_from(Reader& r) {
        KEGS_UPDATE_NPC_UNIT_BUFF_INFO_BROAD_NOT v;
        v.m_vecNpcUnitBuff = Codec<std::remove_cvref_t<decltype(v.m_vecNpcUnitBuff)>>::get(r);
        return v;
    }
};

struct KEGS_UPDATE_NPC_UNIT_BUFF_INFO_NOT {
    std::vector<std::int64_t>     m_vecToUnitUID{};  // std::vector<UidType>
    std::vector<KNpcUnitBuffInfo> m_vecNpcUnitBuff{};  // std::vector< KNpcUnitBuffInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecToUnitUID)>>::put(w, m_vecToUnitUID);
        Codec<std::remove_cvref_t<decltype(m_vecNpcUnitBuff)>>::put(w, m_vecNpcUnitBuff);
    }

    static KEGS_UPDATE_NPC_UNIT_BUFF_INFO_NOT read_from(Reader& r) {
        KEGS_UPDATE_NPC_UNIT_BUFF_INFO_NOT v;
        v.m_vecToUnitUID = Codec<std::remove_cvref_t<decltype(v.m_vecToUnitUID)>>::get(r);
        v.m_vecNpcUnitBuff = Codec<std::remove_cvref_t<decltype(v.m_vecNpcUnitBuff)>>::get(r);
        return v;
    }
};

struct KOldPartyInfo {
    std::int64_t           m_iOldPartyUID{};  // UidType
    std::int64_t           m_iOldHostUnitUID{};  // UidType
    std::set<std::int64_t> m_setOldPartyMember{};  // std::set< UidType >
    std::int16_t           m_sOldPartyFever{};  // short

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOldPartyUID)>>::put(w, m_iOldPartyUID);
        Codec<std::remove_cvref_t<decltype(m_iOldHostUnitUID)>>::put(w, m_iOldHostUnitUID);
        Codec<std::remove_cvref_t<decltype(m_setOldPartyMember)>>::put(w, m_setOldPartyMember);
        Codec<std::remove_cvref_t<decltype(m_sOldPartyFever)>>::put(w, m_sOldPartyFever);
    }

    static KOldPartyInfo read_from(Reader& r) {
        KOldPartyInfo v;
        v.m_iOldPartyUID = Codec<std::remove_cvref_t<decltype(v.m_iOldPartyUID)>>::get(r);
        v.m_iOldHostUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iOldHostUnitUID)>>::get(r);
        v.m_setOldPartyMember = Codec<std::remove_cvref_t<decltype(v.m_setOldPartyMember)>>::get(r);
        v.m_sOldPartyFever = Codec<std::remove_cvref_t<decltype(v.m_sOldPartyFever)>>::get(r);
        return v;
    }
};

struct KAutoPartyUserInfo {
    std::int64_t   m_iGSUID{};  // UidType
    std::int64_t   m_iUnitUID{};  // UidType
    KOldPartyInfo  m_kOldPartyInfo{};
    std::u16string m_wstrNickName{};  // std::wstring
    char           m_cUnitClass{};
    std::uint8_t   m_ucLevel{};  // UCHAR
    std::int32_t   m_iAutoPartyWaitTime{};  // int
    bool           m_bAutoPartyBonus{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iGSUID)>>::put(w, m_iGSUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_kOldPartyInfo)>>::put(w, m_kOldPartyInfo);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_cUnitClass)>>::put(w, m_cUnitClass);
        Codec<std::remove_cvref_t<decltype(m_ucLevel)>>::put(w, m_ucLevel);
        Codec<std::remove_cvref_t<decltype(m_iAutoPartyWaitTime)>>::put(w, m_iAutoPartyWaitTime);
        Codec<std::remove_cvref_t<decltype(m_bAutoPartyBonus)>>::put(w, m_bAutoPartyBonus);
    }

    static KAutoPartyUserInfo read_from(Reader& r) {
        KAutoPartyUserInfo v;
        v.m_iGSUID = Codec<std::remove_cvref_t<decltype(v.m_iGSUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_kOldPartyInfo = Codec<std::remove_cvref_t<decltype(v.m_kOldPartyInfo)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_cUnitClass = Codec<std::remove_cvref_t<decltype(v.m_cUnitClass)>>::get(r);
        v.m_ucLevel = Codec<std::remove_cvref_t<decltype(v.m_ucLevel)>>::get(r);
        v.m_iAutoPartyWaitTime = Codec<std::remove_cvref_t<decltype(v.m_iAutoPartyWaitTime)>>::get(r);
        v.m_bAutoPartyBonus = Codec<std::remove_cvref_t<decltype(v.m_bAutoPartyBonus)>>::get(r);
        return v;
    }
};

struct KEGB_REQUEST_AUTO_PARTY_MAKING_REQ {
    bool                            m_bRemakingAutoParty{};
    KAutoPartyConditionInfo         m_kAutoPartyCondition{};
    std::vector<KAutoPartyUserInfo> m_vecAutoPartyUser{};  // std::vector< KAutoPartyUserInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bRemakingAutoParty)>>::put(w, m_bRemakingAutoParty);
        Codec<std::remove_cvref_t<decltype(m_kAutoPartyCondition)>>::put(w, m_kAutoPartyCondition);
        Codec<std::remove_cvref_t<decltype(m_vecAutoPartyUser)>>::put(w, m_vecAutoPartyUser);
    }

    static KEGB_REQUEST_AUTO_PARTY_MAKING_REQ read_from(Reader& r) {
        KEGB_REQUEST_AUTO_PARTY_MAKING_REQ v;
        v.m_bRemakingAutoParty = Codec<std::remove_cvref_t<decltype(v.m_bRemakingAutoParty)>>::get(r);
        v.m_kAutoPartyCondition = Codec<std::remove_cvref_t<decltype(v.m_kAutoPartyCondition)>>::get(r);
        v.m_vecAutoPartyUser = Codec<std::remove_cvref_t<decltype(v.m_vecAutoPartyUser)>>::get(r);
        return v;
    }
};

struct KEPM_GET_PARTY_INFO_FOR_AUTO_PARTY_MAKING_ACK {
    std::int32_t                    m_iOK{};  // int
    std::u16string                  m_wstrFailUserNickName{};  // std::wstring
    bool                            m_bRemakingAutoParty{};
    KAutoPartyConditionInfo         m_kAutoPartyCondition{};
    std::vector<KAutoPartyUserInfo> m_vecAutoPartyUser{};  // std::vector< KAutoPartyUserInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_wstrFailUserNickName)>>::put(w, m_wstrFailUserNickName);
        Codec<std::remove_cvref_t<decltype(m_bRemakingAutoParty)>>::put(w, m_bRemakingAutoParty);
        Codec<std::remove_cvref_t<decltype(m_kAutoPartyCondition)>>::put(w, m_kAutoPartyCondition);
        Codec<std::remove_cvref_t<decltype(m_vecAutoPartyUser)>>::put(w, m_vecAutoPartyUser);
    }

    static KEPM_GET_PARTY_INFO_FOR_AUTO_PARTY_MAKING_ACK read_from(Reader& r) {
        KEPM_GET_PARTY_INFO_FOR_AUTO_PARTY_MAKING_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_wstrFailUserNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrFailUserNickName)>>::get(r);
        v.m_bRemakingAutoParty = Codec<std::remove_cvref_t<decltype(v.m_bRemakingAutoParty)>>::get(r);
        v.m_kAutoPartyCondition = Codec<std::remove_cvref_t<decltype(v.m_kAutoPartyCondition)>>::get(r);
        v.m_vecAutoPartyUser = Codec<std::remove_cvref_t<decltype(v.m_vecAutoPartyUser)>>::get(r);
        return v;
    }
};

struct KOpenRandomItemStorage {
    std::int32_t                         m_iRessurectionCount{};  // int
    std::int32_t                         m_iRestoreSpirit{};  // int
    std::int32_t                         m_iRandomItemID{};  // int
    bool                                 m_bIsCharmItem{};
    std::int32_t                         m_iSpendED{};  // int
    std::map<std::int32_t, std::int32_t> m_mapInsertedItem{};  // std::map< int, int >
    std::vector<KInventoryItemInfo>      m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >
    std::vector<KItemInfo>               m_vecItemInfo{};  // std::vector< KItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iRessurectionCount)>>::put(w, m_iRessurectionCount);
        Codec<std::remove_cvref_t<decltype(m_iRestoreSpirit)>>::put(w, m_iRestoreSpirit);
        Codec<std::remove_cvref_t<decltype(m_iRandomItemID)>>::put(w, m_iRandomItemID);
        Codec<std::remove_cvref_t<decltype(m_bIsCharmItem)>>::put(w, m_bIsCharmItem);
        Codec<std::remove_cvref_t<decltype(m_iSpendED)>>::put(w, m_iSpendED);
        Codec<std::remove_cvref_t<decltype(m_mapInsertedItem)>>::put(w, m_mapInsertedItem);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
        Codec<std::remove_cvref_t<decltype(m_vecItemInfo)>>::put(w, m_vecItemInfo);
    }

    static KOpenRandomItemStorage read_from(Reader& r) {
        KOpenRandomItemStorage v;
        v.m_iRessurectionCount = Codec<std::remove_cvref_t<decltype(v.m_iRessurectionCount)>>::get(r);
        v.m_iRestoreSpirit = Codec<std::remove_cvref_t<decltype(v.m_iRestoreSpirit)>>::get(r);
        v.m_iRandomItemID = Codec<std::remove_cvref_t<decltype(v.m_iRandomItemID)>>::get(r);
        v.m_bIsCharmItem = Codec<std::remove_cvref_t<decltype(v.m_bIsCharmItem)>>::get(r);
        v.m_iSpendED = Codec<std::remove_cvref_t<decltype(v.m_iSpendED)>>::get(r);
        v.m_mapInsertedItem = Codec<std::remove_cvref_t<decltype(v.m_mapInsertedItem)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        v.m_vecItemInfo = Codec<std::remove_cvref_t<decltype(v.m_vecItemInfo)>>::get(r);
        return v;
    }
};

struct KPVPChannelInfo {
    std::int32_t  m_iMinRating{};  // int
    std::int32_t  m_iMaxRating{};  // int
    std::uint32_t m_iChannelID{};  // UINT
    char          m_cPVPChannelClass{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iMinRating)>>::put(w, m_iMinRating);
        Codec<std::remove_cvref_t<decltype(m_iMaxRating)>>::put(w, m_iMaxRating);
        Codec<std::remove_cvref_t<decltype(m_iChannelID)>>::put(w, m_iChannelID);
        Codec<std::remove_cvref_t<decltype(m_cPVPChannelClass)>>::put(w, m_cPVPChannelClass);
    }

    static KPVPChannelInfo read_from(Reader& r) {
        KPVPChannelInfo v;
        v.m_iMinRating = Codec<std::remove_cvref_t<decltype(v.m_iMinRating)>>::get(r);
        v.m_iMaxRating = Codec<std::remove_cvref_t<decltype(v.m_iMaxRating)>>::get(r);
        v.m_iChannelID = Codec<std::remove_cvref_t<decltype(v.m_iChannelID)>>::get(r);
        v.m_cPVPChannelClass = Codec<std::remove_cvref_t<decltype(v.m_cPVPChannelClass)>>::get(r);
        return v;
    }
};

struct KEGS_OPEN_PVP_ROOM_LIST_ACK {
    std::int32_t                 m_iOK{};  // int
    std::uint32_t                m_iConnectChannelID{};  // UINT
    std::vector<KPVPChannelInfo> m_vecChannel{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iConnectChannelID)>>::put(w, m_iConnectChannelID);
        Codec<std::remove_cvref_t<decltype(m_vecChannel)>>::put(w, m_vecChannel);
    }

    static KEGS_OPEN_PVP_ROOM_LIST_ACK read_from(Reader& r) {
        KEGS_OPEN_PVP_ROOM_LIST_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iConnectChannelID = Codec<std::remove_cvref_t<decltype(v.m_iConnectChannelID)>>::get(r);
        v.m_vecChannel = Codec<std::remove_cvref_t<decltype(v.m_vecChannel)>>::get(r);
        return v;
    }
};

struct KPVPUnitResultInfo {
    std::int64_t m_UnitUID{};  // UidType
    std::int32_t m_nRanking{};  // int
    std::int32_t m_nKillNum{};  // int
    std::int32_t m_nMDKillNum{};  // int
    std::int32_t m_nDeathNum{};  // int
    std::int32_t m_iRating{};  // int
    std::int32_t m_iRPoint{};  // int
    std::int32_t m_iAPoint{};  // int
    float        m_fKFactorIncrement{};
    std::int32_t m_nEXP{};  // int
    bool         m_bIsMVP{};
    bool         m_bWin{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_UnitUID)>>::put(w, m_UnitUID);
        Codec<std::remove_cvref_t<decltype(m_nRanking)>>::put(w, m_nRanking);
        Codec<std::remove_cvref_t<decltype(m_nKillNum)>>::put(w, m_nKillNum);
        Codec<std::remove_cvref_t<decltype(m_nMDKillNum)>>::put(w, m_nMDKillNum);
        Codec<std::remove_cvref_t<decltype(m_nDeathNum)>>::put(w, m_nDeathNum);
        Codec<std::remove_cvref_t<decltype(m_iRating)>>::put(w, m_iRating);
        Codec<std::remove_cvref_t<decltype(m_iRPoint)>>::put(w, m_iRPoint);
        Codec<std::remove_cvref_t<decltype(m_iAPoint)>>::put(w, m_iAPoint);
        Codec<std::remove_cvref_t<decltype(m_fKFactorIncrement)>>::put(w, m_fKFactorIncrement);
        Codec<std::remove_cvref_t<decltype(m_nEXP)>>::put(w, m_nEXP);
        Codec<std::remove_cvref_t<decltype(m_bIsMVP)>>::put(w, m_bIsMVP);
        Codec<std::remove_cvref_t<decltype(m_bWin)>>::put(w, m_bWin);
    }

    static KPVPUnitResultInfo read_from(Reader& r) {
        KPVPUnitResultInfo v;
        v.m_UnitUID = Codec<std::remove_cvref_t<decltype(v.m_UnitUID)>>::get(r);
        v.m_nRanking = Codec<std::remove_cvref_t<decltype(v.m_nRanking)>>::get(r);
        v.m_nKillNum = Codec<std::remove_cvref_t<decltype(v.m_nKillNum)>>::get(r);
        v.m_nMDKillNum = Codec<std::remove_cvref_t<decltype(v.m_nMDKillNum)>>::get(r);
        v.m_nDeathNum = Codec<std::remove_cvref_t<decltype(v.m_nDeathNum)>>::get(r);
        v.m_iRating = Codec<std::remove_cvref_t<decltype(v.m_iRating)>>::get(r);
        v.m_iRPoint = Codec<std::remove_cvref_t<decltype(v.m_iRPoint)>>::get(r);
        v.m_iAPoint = Codec<std::remove_cvref_t<decltype(v.m_iAPoint)>>::get(r);
        v.m_fKFactorIncrement = Codec<std::remove_cvref_t<decltype(v.m_fKFactorIncrement)>>::get(r);
        v.m_nEXP = Codec<std::remove_cvref_t<decltype(v.m_nEXP)>>::get(r);
        v.m_bIsMVP = Codec<std::remove_cvref_t<decltype(v.m_bIsMVP)>>::get(r);
        v.m_bWin = Codec<std::remove_cvref_t<decltype(v.m_bWin)>>::get(r);
        return v;
    }
};

struct KEGS_END_GAME_PVP_RESULT_DATA_NOT {
    char                            m_cWinTeam{};
    bool                            m_bIsDrawn{};
    bool                            m_bIsAbnormalEndPlay{};
    std::int32_t                    m_iPlayTime{};  // int
    std::int32_t                    m_iGameType{};  // int
    std::int32_t                    m_iSpirit{};  // int
    std::uint32_t                   m_uiChannelID{};  // UINT
    std::vector<KPVPUnitResultInfo> m_vecPVPUnitInfo{};  // std::vector< KPVPUnitResultInfo >
    bool                            m_bOfficialMatch{};
    bool                            m_bIsHeroNPC{};
    std::int32_t                    m_iRematchCount{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cWinTeam)>>::put(w, m_cWinTeam);
        Codec<std::remove_cvref_t<decltype(m_bIsDrawn)>>::put(w, m_bIsDrawn);
        Codec<std::remove_cvref_t<decltype(m_bIsAbnormalEndPlay)>>::put(w, m_bIsAbnormalEndPlay);
        Codec<std::remove_cvref_t<decltype(m_iPlayTime)>>::put(w, m_iPlayTime);
        Codec<std::remove_cvref_t<decltype(m_iGameType)>>::put(w, m_iGameType);
        Codec<std::remove_cvref_t<decltype(m_iSpirit)>>::put(w, m_iSpirit);
        Codec<std::remove_cvref_t<decltype(m_uiChannelID)>>::put(w, m_uiChannelID);
        Codec<std::remove_cvref_t<decltype(m_vecPVPUnitInfo)>>::put(w, m_vecPVPUnitInfo);
        Codec<std::remove_cvref_t<decltype(m_bOfficialMatch)>>::put(w, m_bOfficialMatch);
        Codec<std::remove_cvref_t<decltype(m_bIsHeroNPC)>>::put(w, m_bIsHeroNPC);
        Codec<std::remove_cvref_t<decltype(m_iRematchCount)>>::put(w, m_iRematchCount);
    }

    static KEGS_END_GAME_PVP_RESULT_DATA_NOT read_from(Reader& r) {
        KEGS_END_GAME_PVP_RESULT_DATA_NOT v;
        v.m_cWinTeam = Codec<std::remove_cvref_t<decltype(v.m_cWinTeam)>>::get(r);
        v.m_bIsDrawn = Codec<std::remove_cvref_t<decltype(v.m_bIsDrawn)>>::get(r);
        v.m_bIsAbnormalEndPlay = Codec<std::remove_cvref_t<decltype(v.m_bIsAbnormalEndPlay)>>::get(r);
        v.m_iPlayTime = Codec<std::remove_cvref_t<decltype(v.m_iPlayTime)>>::get(r);
        v.m_iGameType = Codec<std::remove_cvref_t<decltype(v.m_iGameType)>>::get(r);
        v.m_iSpirit = Codec<std::remove_cvref_t<decltype(v.m_iSpirit)>>::get(r);
        v.m_uiChannelID = Codec<std::remove_cvref_t<decltype(v.m_uiChannelID)>>::get(r);
        v.m_vecPVPUnitInfo = Codec<std::remove_cvref_t<decltype(v.m_vecPVPUnitInfo)>>::get(r);
        v.m_bOfficialMatch = Codec<std::remove_cvref_t<decltype(v.m_bOfficialMatch)>>::get(r);
        v.m_bIsHeroNPC = Codec<std::remove_cvref_t<decltype(v.m_bIsHeroNPC)>>::get(r);
        v.m_iRematchCount = Codec<std::remove_cvref_t<decltype(v.m_iRematchCount)>>::get(r);
        return v;
    }
};

struct KPVPUserSimpleInfo {
    std::int64_t m_iUnitUID{};  // UidType
    char         m_cState{};
    std::int32_t m_iStateCode{};  // int
    std::int64_t m_iRoomUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_cState)>>::put(w, m_cState);
        Codec<std::remove_cvref_t<decltype(m_iStateCode)>>::put(w, m_iStateCode);
        Codec<std::remove_cvref_t<decltype(m_iRoomUID)>>::put(w, m_iRoomUID);
    }

    static KPVPUserSimpleInfo read_from(Reader& r) {
        KPVPUserSimpleInfo v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_cState = Codec<std::remove_cvref_t<decltype(v.m_cState)>>::get(r);
        v.m_iStateCode = Codec<std::remove_cvref_t<decltype(v.m_iStateCode)>>::get(r);
        v.m_iRoomUID = Codec<std::remove_cvref_t<decltype(v.m_iRoomUID)>>::get(r);
        return v;
    }
};

struct KECN_UPDATE_PVP_USER_INFO_NOT {
    std::vector<KPVPUserSimpleInfo> m_vecUserInfo{};  // std::vector< KPVPUserSimpleInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecUserInfo)>>::put(w, m_vecUserInfo);
    }

    static KECN_UPDATE_PVP_USER_INFO_NOT read_from(Reader& r) {
        KECN_UPDATE_PVP_USER_INFO_NOT v;
        v.m_vecUserInfo = Codec<std::remove_cvref_t<decltype(v.m_vecUserInfo)>>::get(r);
        return v;
    }
};

struct KPacketOK {
    std::int32_t m_iOK{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
    }

    static KPacketOK read_from(Reader& r) {
        KPacketOK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        return v;
    }
};

struct KPacketProcessingCountInfo {
    std::uint16_t m_usEventID{};  // u_short
    std::uint32_t m_dwCount{};  // DWORD

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_usEventID)>>::put(w, m_usEventID);
        Codec<std::remove_cvref_t<decltype(m_dwCount)>>::put(w, m_dwCount);
    }

    static KPacketProcessingCountInfo read_from(Reader& r) {
        KPacketProcessingCountInfo v;
        v.m_usEventID = Codec<std::remove_cvref_t<decltype(v.m_usEventID)>>::get(r);
        v.m_dwCount = Codec<std::remove_cvref_t<decltype(v.m_dwCount)>>::get(r);
        return v;
    }
};

struct KPacketProcessingLatencyInfo {
    std::uint16_t  m_usEventID{};  // u_short
    std::int64_t   m_iUnitUID{};  // UidType
    std::uint32_t  m_dwMaxLatency{};  // DWORD
    std::u16string m_wstrMaxLatencyTime{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_usEventID)>>::put(w, m_usEventID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_dwMaxLatency)>>::put(w, m_dwMaxLatency);
        Codec<std::remove_cvref_t<decltype(m_wstrMaxLatencyTime)>>::put(w, m_wstrMaxLatencyTime);
    }

    static KPacketProcessingLatencyInfo read_from(Reader& r) {
        KPacketProcessingLatencyInfo v;
        v.m_usEventID = Codec<std::remove_cvref_t<decltype(v.m_usEventID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_dwMaxLatency = Codec<std::remove_cvref_t<decltype(v.m_dwMaxLatency)>>::get(r);
        v.m_wstrMaxLatencyTime = Codec<std::remove_cvref_t<decltype(v.m_wstrMaxLatencyTime)>>::get(r);
        return v;
    }
};

struct KDBE_UPDATE_PAKCET_MORNITORING_LOG_NOT {
    std::vector<KPacketProcessingLatencyInfo> m_vecGSUserPacketLatencyInfo{};  // std::vector< KPacketProcessingLatencyInfo >
    std::vector<KPacketProcessingCountInfo>   m_vecLoginProxyPacketCountInfo{};  // std::vector< KPacketProcessingCountInfo >
    std::vector<KPacketProcessingCountInfo>   m_vecCnProxyPacketCountInfo{};  // std::vector< KPacketProcessingCountInfo >
    std::vector<KPacketProcessingCountInfo>   m_vecGlobalProxyPacketCountInfo{};  // std::vector< KPacketProcessingCountInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecGSUserPacketLatencyInfo)>>::put(w, m_vecGSUserPacketLatencyInfo);
        Codec<std::remove_cvref_t<decltype(m_vecLoginProxyPacketCountInfo)>>::put(w, m_vecLoginProxyPacketCountInfo);
        Codec<std::remove_cvref_t<decltype(m_vecCnProxyPacketCountInfo)>>::put(w, m_vecCnProxyPacketCountInfo);
        Codec<std::remove_cvref_t<decltype(m_vecGlobalProxyPacketCountInfo)>>::put(w, m_vecGlobalProxyPacketCountInfo);
    }

    static KDBE_UPDATE_PAKCET_MORNITORING_LOG_NOT read_from(Reader& r) {
        KDBE_UPDATE_PAKCET_MORNITORING_LOG_NOT v;
        v.m_vecGSUserPacketLatencyInfo = Codec<std::remove_cvref_t<decltype(v.m_vecGSUserPacketLatencyInfo)>>::get(r);
        v.m_vecLoginProxyPacketCountInfo = Codec<std::remove_cvref_t<decltype(v.m_vecLoginProxyPacketCountInfo)>>::get(r);
        v.m_vecCnProxyPacketCountInfo = Codec<std::remove_cvref_t<decltype(v.m_vecCnProxyPacketCountInfo)>>::get(r);
        v.m_vecGlobalProxyPacketCountInfo = Codec<std::remove_cvref_t<decltype(v.m_vecGlobalProxyPacketCountInfo)>>::get(r);
        return v;
    }
};

struct KPacketReason {
    std::int32_t m_iReason{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iReason)>>::put(w, m_iReason);
    }

    static KPacketReason read_from(Reader& r) {
        KPacketReason v;
        v.m_iReason = Codec<std::remove_cvref_t<decltype(v.m_iReason)>>::get(r);
        return v;
    }
};

struct KPartyInfo {
    std::int64_t   m_iPartyUID{};  // UidType
    char           m_cPartyType{};
    std::u16string m_wstrPartyName{};  // std::wstring
    char           m_JoinSlot{};
    char           m_MaxSlot{};
    bool           m_bPublic{};
    std::int32_t   m_iDungeonID{};  // int
    char           m_DifficultyLevel{};
    char           m_cGetItemType{};
    char           m_cDungeonMode{};
    std::int16_t   m_sPartyFever{};  // short
    bool           m_bCheckLowLevel{};
    char           m_cPVPGameType{};
    bool           m_bComeBackBuff{};
    std::u16string m_wsrtComeBackBuffEnd{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iPartyUID)>>::put(w, m_iPartyUID);
        Codec<std::remove_cvref_t<decltype(m_cPartyType)>>::put(w, m_cPartyType);
        Codec<std::remove_cvref_t<decltype(m_wstrPartyName)>>::put(w, m_wstrPartyName);
        Codec<std::remove_cvref_t<decltype(m_JoinSlot)>>::put(w, m_JoinSlot);
        Codec<std::remove_cvref_t<decltype(m_MaxSlot)>>::put(w, m_MaxSlot);
        Codec<std::remove_cvref_t<decltype(m_bPublic)>>::put(w, m_bPublic);
        Codec<std::remove_cvref_t<decltype(m_iDungeonID)>>::put(w, m_iDungeonID);
        Codec<std::remove_cvref_t<decltype(m_DifficultyLevel)>>::put(w, m_DifficultyLevel);
        Codec<std::remove_cvref_t<decltype(m_cGetItemType)>>::put(w, m_cGetItemType);
        Codec<std::remove_cvref_t<decltype(m_cDungeonMode)>>::put(w, m_cDungeonMode);
        Codec<std::remove_cvref_t<decltype(m_sPartyFever)>>::put(w, m_sPartyFever);
        Codec<std::remove_cvref_t<decltype(m_bCheckLowLevel)>>::put(w, m_bCheckLowLevel);
        Codec<std::remove_cvref_t<decltype(m_cPVPGameType)>>::put(w, m_cPVPGameType);
        Codec<std::remove_cvref_t<decltype(m_bComeBackBuff)>>::put(w, m_bComeBackBuff);
        Codec<std::remove_cvref_t<decltype(m_wsrtComeBackBuffEnd)>>::put(w, m_wsrtComeBackBuffEnd);
    }

    static KPartyInfo read_from(Reader& r) {
        KPartyInfo v;
        v.m_iPartyUID = Codec<std::remove_cvref_t<decltype(v.m_iPartyUID)>>::get(r);
        v.m_cPartyType = Codec<std::remove_cvref_t<decltype(v.m_cPartyType)>>::get(r);
        v.m_wstrPartyName = Codec<std::remove_cvref_t<decltype(v.m_wstrPartyName)>>::get(r);
        v.m_JoinSlot = Codec<std::remove_cvref_t<decltype(v.m_JoinSlot)>>::get(r);
        v.m_MaxSlot = Codec<std::remove_cvref_t<decltype(v.m_MaxSlot)>>::get(r);
        v.m_bPublic = Codec<std::remove_cvref_t<decltype(v.m_bPublic)>>::get(r);
        v.m_iDungeonID = Codec<std::remove_cvref_t<decltype(v.m_iDungeonID)>>::get(r);
        v.m_DifficultyLevel = Codec<std::remove_cvref_t<decltype(v.m_DifficultyLevel)>>::get(r);
        v.m_cGetItemType = Codec<std::remove_cvref_t<decltype(v.m_cGetItemType)>>::get(r);
        v.m_cDungeonMode = Codec<std::remove_cvref_t<decltype(v.m_cDungeonMode)>>::get(r);
        v.m_sPartyFever = Codec<std::remove_cvref_t<decltype(v.m_sPartyFever)>>::get(r);
        v.m_bCheckLowLevel = Codec<std::remove_cvref_t<decltype(v.m_bCheckLowLevel)>>::get(r);
        v.m_cPVPGameType = Codec<std::remove_cvref_t<decltype(v.m_cPVPGameType)>>::get(r);
        v.m_bComeBackBuff = Codec<std::remove_cvref_t<decltype(v.m_bComeBackBuff)>>::get(r);
        v.m_wsrtComeBackBuffEnd = Codec<std::remove_cvref_t<decltype(v.m_wsrtComeBackBuffEnd)>>::get(r);
        return v;
    }
};

struct KEGS_CHANGE_PARTY_TYPE_NOT {
    KPartyInfo m_kPartyInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kPartyInfo)>>::put(w, m_kPartyInfo);
    }

    static KEGS_CHANGE_PARTY_TYPE_NOT read_from(Reader& r) {
        KEGS_CHANGE_PARTY_TYPE_NOT v;
        v.m_kPartyInfo = Codec<std::remove_cvref_t<decltype(v.m_kPartyInfo)>>::get(r);
        return v;
    }
};

struct KEPM_UPDATE_PARTY_IN_PARTY_LIST_NOT {
    KPartyInfo m_kPartyInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kPartyInfo)>>::put(w, m_kPartyInfo);
    }

    static KEPM_UPDATE_PARTY_IN_PARTY_LIST_NOT read_from(Reader& r) {
        KEPM_UPDATE_PARTY_IN_PARTY_LIST_NOT v;
        v.m_kPartyInfo = Codec<std::remove_cvref_t<decltype(v.m_kPartyInfo)>>::get(r);
        return v;
    }
};

struct KPartyListUserInfo {
    std::int32_t   m_iChannelID{};  // int
    std::int64_t   m_iUnitUID{};  // UidType
    std::u16string m_wstrNickName{};  // std::wstring
    char           m_cUnitClass{};
    std::uint8_t   m_ucLevel{};  // UCHAR
    std::uint8_t   m_cState{};  // UCHAR
    std::int32_t   m_iStateCode{};  // int
    bool           m_bHost{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iChannelID)>>::put(w, m_iChannelID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_cUnitClass)>>::put(w, m_cUnitClass);
        Codec<std::remove_cvref_t<decltype(m_ucLevel)>>::put(w, m_ucLevel);
        Codec<std::remove_cvref_t<decltype(m_cState)>>::put(w, m_cState);
        Codec<std::remove_cvref_t<decltype(m_iStateCode)>>::put(w, m_iStateCode);
        Codec<std::remove_cvref_t<decltype(m_bHost)>>::put(w, m_bHost);
    }

    static KPartyListUserInfo read_from(Reader& r) {
        KPartyListUserInfo v;
        v.m_iChannelID = Codec<std::remove_cvref_t<decltype(v.m_iChannelID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_cUnitClass = Codec<std::remove_cvref_t<decltype(v.m_cUnitClass)>>::get(r);
        v.m_ucLevel = Codec<std::remove_cvref_t<decltype(v.m_ucLevel)>>::get(r);
        v.m_cState = Codec<std::remove_cvref_t<decltype(v.m_cState)>>::get(r);
        v.m_iStateCode = Codec<std::remove_cvref_t<decltype(v.m_iStateCode)>>::get(r);
        v.m_bHost = Codec<std::remove_cvref_t<decltype(v.m_bHost)>>::get(r);
        return v;
    }
};

struct KPartyListInfo {
    KPartyInfo                      m_kPartyInfo{};
    std::vector<KPartyListUserInfo> m_vecPartyListUserInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kPartyInfo)>>::put(w, m_kPartyInfo);
        Codec<std::remove_cvref_t<decltype(m_vecPartyListUserInfo)>>::put(w, m_vecPartyListUserInfo);
    }

    static KPartyListInfo read_from(Reader& r) {
        KPartyListInfo v;
        v.m_kPartyInfo = Codec<std::remove_cvref_t<decltype(v.m_kPartyInfo)>>::get(r);
        v.m_vecPartyListUserInfo = Codec<std::remove_cvref_t<decltype(v.m_vecPartyListUserInfo)>>::get(r);
        return v;
    }
};

struct KPartyMemberStatus {
    float m_fHpPercent{};
    float m_fMpPercent{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_fHpPercent)>>::put(w, m_fHpPercent);
        Codec<std::remove_cvref_t<decltype(m_fMpPercent)>>::put(w, m_fMpPercent);
    }

    static KPartyMemberStatus read_from(Reader& r) {
        KPartyMemberStatus v;
        v.m_fHpPercent = Codec<std::remove_cvref_t<decltype(v.m_fHpPercent)>>::get(r);
        v.m_fMpPercent = Codec<std::remove_cvref_t<decltype(v.m_fMpPercent)>>::get(r);
        return v;
    }
};

struct KEGS_UPDATE_PARTY_MEMBER_STATUS_NOT {
    std::int64_t       m_iUnitUID{};  // UidType
    KPartyMemberStatus m_kStatus{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_kStatus)>>::put(w, m_kStatus);
    }

    static KEGS_UPDATE_PARTY_MEMBER_STATUS_NOT read_from(Reader& r) {
        KEGS_UPDATE_PARTY_MEMBER_STATUS_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_kStatus = Codec<std::remove_cvref_t<decltype(v.m_kStatus)>>::get(r);
        return v;
    }
};

struct KPartyUserInfo {
    std::int64_t       m_iGSUID{};  // UidType
    std::int32_t       m_iChannelID{};  // int
    std::int64_t       m_iFieldUID{};  // UidType
    std::int64_t       m_iUnitUID{};  // UidType
    char               m_cUnitClass{};
    std::u16string     m_wstrNickName{};  // std::wstring
    std::uint8_t       m_ucLevel{};  // UCHAR
    bool               m_bIsGuestUser{};
    std::uint8_t       m_cState{};  // UCHAR
    std::int32_t       m_iStateCode{};  // int
    bool               m_bReady{};
    bool               m_bHost{};
    bool               m_bComeBack{};
    std::u16string     m_wsrtComeBackEnd{};  // std::wstring
    char               m_cRank{};
    std::int32_t       m_iOfficialMatchCnt{};  // int
    std::int32_t       m_iRating{};  // int
    std::int32_t       m_iRPoint{};  // int
    KPartyMemberStatus m_kGamePlayStatus{};
    std::u16string     m_wstrIP{};  // std::wstring
    std::uint16_t      m_usPort{};  // USHORT
    std::uint32_t      m_dwPingScore{};  // DWORD
    std::u16string     m_wstrInternalIP{};  // std::wstring
    std::uint16_t      m_usInternalPort{};  // USHORT

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iGSUID)>>::put(w, m_iGSUID);
        Codec<std::remove_cvref_t<decltype(m_iChannelID)>>::put(w, m_iChannelID);
        Codec<std::remove_cvref_t<decltype(m_iFieldUID)>>::put(w, m_iFieldUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_cUnitClass)>>::put(w, m_cUnitClass);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_ucLevel)>>::put(w, m_ucLevel);
        Codec<std::remove_cvref_t<decltype(m_bIsGuestUser)>>::put(w, m_bIsGuestUser);
        Codec<std::remove_cvref_t<decltype(m_cState)>>::put(w, m_cState);
        Codec<std::remove_cvref_t<decltype(m_iStateCode)>>::put(w, m_iStateCode);
        Codec<std::remove_cvref_t<decltype(m_bReady)>>::put(w, m_bReady);
        Codec<std::remove_cvref_t<decltype(m_bHost)>>::put(w, m_bHost);
        Codec<std::remove_cvref_t<decltype(m_bComeBack)>>::put(w, m_bComeBack);
        Codec<std::remove_cvref_t<decltype(m_wsrtComeBackEnd)>>::put(w, m_wsrtComeBackEnd);
        Codec<std::remove_cvref_t<decltype(m_cRank)>>::put(w, m_cRank);
        Codec<std::remove_cvref_t<decltype(m_iOfficialMatchCnt)>>::put(w, m_iOfficialMatchCnt);
        Codec<std::remove_cvref_t<decltype(m_iRating)>>::put(w, m_iRating);
        Codec<std::remove_cvref_t<decltype(m_iRPoint)>>::put(w, m_iRPoint);
        Codec<std::remove_cvref_t<decltype(m_kGamePlayStatus)>>::put(w, m_kGamePlayStatus);
        Codec<std::remove_cvref_t<decltype(m_wstrIP)>>::put(w, m_wstrIP);
        Codec<std::remove_cvref_t<decltype(m_usPort)>>::put(w, m_usPort);
        Codec<std::remove_cvref_t<decltype(m_dwPingScore)>>::put(w, m_dwPingScore);
        Codec<std::remove_cvref_t<decltype(m_wstrInternalIP)>>::put(w, m_wstrInternalIP);
        Codec<std::remove_cvref_t<decltype(m_usInternalPort)>>::put(w, m_usInternalPort);
    }

    static KPartyUserInfo read_from(Reader& r) {
        KPartyUserInfo v;
        v.m_iGSUID = Codec<std::remove_cvref_t<decltype(v.m_iGSUID)>>::get(r);
        v.m_iChannelID = Codec<std::remove_cvref_t<decltype(v.m_iChannelID)>>::get(r);
        v.m_iFieldUID = Codec<std::remove_cvref_t<decltype(v.m_iFieldUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_cUnitClass = Codec<std::remove_cvref_t<decltype(v.m_cUnitClass)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_ucLevel = Codec<std::remove_cvref_t<decltype(v.m_ucLevel)>>::get(r);
        v.m_bIsGuestUser = Codec<std::remove_cvref_t<decltype(v.m_bIsGuestUser)>>::get(r);
        v.m_cState = Codec<std::remove_cvref_t<decltype(v.m_cState)>>::get(r);
        v.m_iStateCode = Codec<std::remove_cvref_t<decltype(v.m_iStateCode)>>::get(r);
        v.m_bReady = Codec<std::remove_cvref_t<decltype(v.m_bReady)>>::get(r);
        v.m_bHost = Codec<std::remove_cvref_t<decltype(v.m_bHost)>>::get(r);
        v.m_bComeBack = Codec<std::remove_cvref_t<decltype(v.m_bComeBack)>>::get(r);
        v.m_wsrtComeBackEnd = Codec<std::remove_cvref_t<decltype(v.m_wsrtComeBackEnd)>>::get(r);
        v.m_cRank = Codec<std::remove_cvref_t<decltype(v.m_cRank)>>::get(r);
        v.m_iOfficialMatchCnt = Codec<std::remove_cvref_t<decltype(v.m_iOfficialMatchCnt)>>::get(r);
        v.m_iRating = Codec<std::remove_cvref_t<decltype(v.m_iRating)>>::get(r);
        v.m_iRPoint = Codec<std::remove_cvref_t<decltype(v.m_iRPoint)>>::get(r);
        v.m_kGamePlayStatus = Codec<std::remove_cvref_t<decltype(v.m_kGamePlayStatus)>>::get(r);
        v.m_wstrIP = Codec<std::remove_cvref_t<decltype(v.m_wstrIP)>>::get(r);
        v.m_usPort = Codec<std::remove_cvref_t<decltype(v.m_usPort)>>::get(r);
        v.m_dwPingScore = Codec<std::remove_cvref_t<decltype(v.m_dwPingScore)>>::get(r);
        v.m_wstrInternalIP = Codec<std::remove_cvref_t<decltype(v.m_wstrInternalIP)>>::get(r);
        v.m_usInternalPort = Codec<std::remove_cvref_t<decltype(v.m_usInternalPort)>>::get(r);
        return v;
    }
};

struct KEGS_JOIN_PARTY_NOT {
    std::int32_t                m_iOK{};  // int
    std::vector<KPartyInfo>     m_vecPartyInfo{};  // std::vector< KPartyInfo >
    std::vector<KPartyUserInfo> m_vecPartyUserInfo{};  // std::vector< KPartyUserInfo >
    std::vector<std::int64_t>   m_vecCurPartyUnitUIDList{};  // std::vector< UidType >
    std::u16string              m_wstrUDPRelayIP{};  // std::wstring
    std::uint16_t               m_usUDPRelayPort{};  // USHORT

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_vecPartyInfo)>>::put(w, m_vecPartyInfo);
        Codec<std::remove_cvref_t<decltype(m_vecPartyUserInfo)>>::put(w, m_vecPartyUserInfo);
        Codec<std::remove_cvref_t<decltype(m_vecCurPartyUnitUIDList)>>::put(w, m_vecCurPartyUnitUIDList);
        Codec<std::remove_cvref_t<decltype(m_wstrUDPRelayIP)>>::put(w, m_wstrUDPRelayIP);
        Codec<std::remove_cvref_t<decltype(m_usUDPRelayPort)>>::put(w, m_usUDPRelayPort);
    }

    static KEGS_JOIN_PARTY_NOT read_from(Reader& r) {
        KEGS_JOIN_PARTY_NOT v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_vecPartyInfo = Codec<std::remove_cvref_t<decltype(v.m_vecPartyInfo)>>::get(r);
        v.m_vecPartyUserInfo = Codec<std::remove_cvref_t<decltype(v.m_vecPartyUserInfo)>>::get(r);
        v.m_vecCurPartyUnitUIDList = Codec<std::remove_cvref_t<decltype(v.m_vecCurPartyUnitUIDList)>>::get(r);
        v.m_wstrUDPRelayIP = Codec<std::remove_cvref_t<decltype(v.m_wstrUDPRelayIP)>>::get(r);
        v.m_usUDPRelayPort = Codec<std::remove_cvref_t<decltype(v.m_usUDPRelayPort)>>::get(r);
        return v;
    }
};

struct KEGS_REGROUP_PARTY_NOT {
    KPartyInfo                  m_kPartyInfo{};
    std::vector<KPartyUserInfo> m_vecPartyUserInfo{};  // std::vector< KPartyUserInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kPartyInfo)>>::put(w, m_kPartyInfo);
        Codec<std::remove_cvref_t<decltype(m_vecPartyUserInfo)>>::put(w, m_vecPartyUserInfo);
    }

    static KEGS_REGROUP_PARTY_NOT read_from(Reader& r) {
        KEGS_REGROUP_PARTY_NOT v;
        v.m_kPartyInfo = Codec<std::remove_cvref_t<decltype(v.m_kPartyInfo)>>::get(r);
        v.m_vecPartyUserInfo = Codec<std::remove_cvref_t<decltype(v.m_vecPartyUserInfo)>>::get(r);
        return v;
    }
};

struct KELG_INVITE_PARTY_FIND_RECEIVER_ACK {
    std::int32_t                m_iOK{};  // int
    std::int64_t                m_iHostUnitUID{};  // UidType
    std::vector<KPartyUserInfo> m_vecReceiverPartyUserInfo{};  // std::vector< KPartyUserInfo >
    KCreatePartyInfo            m_kCreatePartyInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iHostUnitUID)>>::put(w, m_iHostUnitUID);
        Codec<std::remove_cvref_t<decltype(m_vecReceiverPartyUserInfo)>>::put(w, m_vecReceiverPartyUserInfo);
        Codec<std::remove_cvref_t<decltype(m_kCreatePartyInfo)>>::put(w, m_kCreatePartyInfo);
    }

    static KELG_INVITE_PARTY_FIND_RECEIVER_ACK read_from(Reader& r) {
        KELG_INVITE_PARTY_FIND_RECEIVER_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iHostUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iHostUnitUID)>>::get(r);
        v.m_vecReceiverPartyUserInfo = Codec<std::remove_cvref_t<decltype(v.m_vecReceiverPartyUserInfo)>>::get(r);
        v.m_kCreatePartyInfo = Codec<std::remove_cvref_t<decltype(v.m_kCreatePartyInfo)>>::get(r);
        return v;
    }
};

struct KELG_INVITE_PARTY_REPLY_TO_HOST_NOT {
    std::int64_t   m_iHostUnitUID{};  // UidType
    char           m_cReplyReason{};
    KPartyUserInfo m_kPartyUserInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iHostUnitUID)>>::put(w, m_iHostUnitUID);
        Codec<std::remove_cvref_t<decltype(m_cReplyReason)>>::put(w, m_cReplyReason);
        Codec<std::remove_cvref_t<decltype(m_kPartyUserInfo)>>::put(w, m_kPartyUserInfo);
    }

    static KELG_INVITE_PARTY_REPLY_TO_HOST_NOT read_from(Reader& r) {
        KELG_INVITE_PARTY_REPLY_TO_HOST_NOT v;
        v.m_iHostUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iHostUnitUID)>>::get(r);
        v.m_cReplyReason = Codec<std::remove_cvref_t<decltype(v.m_cReplyReason)>>::get(r);
        v.m_kPartyUserInfo = Codec<std::remove_cvref_t<decltype(v.m_kPartyUserInfo)>>::get(r);
        return v;
    }
};

struct KEPM_ADD_PARTY_MEMBER_IN_PARTY_LIST_NOT {
    std::int64_t   m_iPartyUID{};  // UidType
    KPartyUserInfo m_kPartyUserInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iPartyUID)>>::put(w, m_iPartyUID);
        Codec<std::remove_cvref_t<decltype(m_kPartyUserInfo)>>::put(w, m_kPartyUserInfo);
    }

    static KEPM_ADD_PARTY_MEMBER_IN_PARTY_LIST_NOT read_from(Reader& r) {
        KEPM_ADD_PARTY_MEMBER_IN_PARTY_LIST_NOT v;
        v.m_iPartyUID = Codec<std::remove_cvref_t<decltype(v.m_iPartyUID)>>::get(r);
        v.m_kPartyUserInfo = Codec<std::remove_cvref_t<decltype(v.m_kPartyUserInfo)>>::get(r);
        return v;
    }
};

struct KEPM_CREATE_PARTY_BY_INVITE_ACK {
    std::int32_t   m_iOK{};  // int
    KPartyInfo     m_kPartyInfo{};
    KPartyUserInfo m_kHostPartyUserInfo{};
    KPartyUserInfo m_kReceiverPartyUserInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_kPartyInfo)>>::put(w, m_kPartyInfo);
        Codec<std::remove_cvref_t<decltype(m_kHostPartyUserInfo)>>::put(w, m_kHostPartyUserInfo);
        Codec<std::remove_cvref_t<decltype(m_kReceiverPartyUserInfo)>>::put(w, m_kReceiverPartyUserInfo);
    }

    static KEPM_CREATE_PARTY_BY_INVITE_ACK read_from(Reader& r) {
        KEPM_CREATE_PARTY_BY_INVITE_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_kPartyInfo = Codec<std::remove_cvref_t<decltype(v.m_kPartyInfo)>>::get(r);
        v.m_kHostPartyUserInfo = Codec<std::remove_cvref_t<decltype(v.m_kHostPartyUserInfo)>>::get(r);
        v.m_kReceiverPartyUserInfo = Codec<std::remove_cvref_t<decltype(v.m_kReceiverPartyUserInfo)>>::get(r);
        return v;
    }
};

struct KEPM_CREATE_PARTY_BY_INVITE_REQ {
    char             m_cAuthLevel{};
    KCreatePartyInfo m_kCreatePartyInfo{};
    KPartyUserInfo   m_kHostPartyUserInfo{};
    KPartyUserInfo   m_kReceiverPartyUserInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cAuthLevel)>>::put(w, m_cAuthLevel);
        Codec<std::remove_cvref_t<decltype(m_kCreatePartyInfo)>>::put(w, m_kCreatePartyInfo);
        Codec<std::remove_cvref_t<decltype(m_kHostPartyUserInfo)>>::put(w, m_kHostPartyUserInfo);
        Codec<std::remove_cvref_t<decltype(m_kReceiverPartyUserInfo)>>::put(w, m_kReceiverPartyUserInfo);
    }

    static KEPM_CREATE_PARTY_BY_INVITE_REQ read_from(Reader& r) {
        KEPM_CREATE_PARTY_BY_INVITE_REQ v;
        v.m_cAuthLevel = Codec<std::remove_cvref_t<decltype(v.m_cAuthLevel)>>::get(r);
        v.m_kCreatePartyInfo = Codec<std::remove_cvref_t<decltype(v.m_kCreatePartyInfo)>>::get(r);
        v.m_kHostPartyUserInfo = Codec<std::remove_cvref_t<decltype(v.m_kHostPartyUserInfo)>>::get(r);
        v.m_kReceiverPartyUserInfo = Codec<std::remove_cvref_t<decltype(v.m_kReceiverPartyUserInfo)>>::get(r);
        return v;
    }
};

struct KEPM_INVITE_PARTY_REPLY_NOT {
    std::int64_t   m_iPartyUID{};  // UidType
    char           m_cReplyReason{};
    KPartyUserInfo m_kPartyUserInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iPartyUID)>>::put(w, m_iPartyUID);
        Codec<std::remove_cvref_t<decltype(m_cReplyReason)>>::put(w, m_cReplyReason);
        Codec<std::remove_cvref_t<decltype(m_kPartyUserInfo)>>::put(w, m_kPartyUserInfo);
    }

    static KEPM_INVITE_PARTY_REPLY_NOT read_from(Reader& r) {
        KEPM_INVITE_PARTY_REPLY_NOT v;
        v.m_iPartyUID = Codec<std::remove_cvref_t<decltype(v.m_iPartyUID)>>::get(r);
        v.m_cReplyReason = Codec<std::remove_cvref_t<decltype(v.m_cReplyReason)>>::get(r);
        v.m_kPartyUserInfo = Codec<std::remove_cvref_t<decltype(v.m_kPartyUserInfo)>>::get(r);
        return v;
    }
};

struct KEPM_INVITE_PARTY_REQ {
    std::int64_t   m_iPartyUID{};  // UidType
    KPartyUserInfo m_kPartyUserInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iPartyUID)>>::put(w, m_iPartyUID);
        Codec<std::remove_cvref_t<decltype(m_kPartyUserInfo)>>::put(w, m_kPartyUserInfo);
    }

    static KEPM_INVITE_PARTY_REQ read_from(Reader& r) {
        KEPM_INVITE_PARTY_REQ v;
        v.m_iPartyUID = Codec<std::remove_cvref_t<decltype(v.m_iPartyUID)>>::get(r);
        v.m_kPartyUserInfo = Codec<std::remove_cvref_t<decltype(v.m_kPartyUserInfo)>>::get(r);
        return v;
    }
};

struct KEPM_OPEN_PARTY_LIST_NOT {
    KPartyInfo                  m_kPartyInfo{};
    std::vector<KPartyUserInfo> m_vecUserInfo{};  // std::vector< KPartyUserInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kPartyInfo)>>::put(w, m_kPartyInfo);
        Codec<std::remove_cvref_t<decltype(m_vecUserInfo)>>::put(w, m_vecUserInfo);
    }

    static KEPM_OPEN_PARTY_LIST_NOT read_from(Reader& r) {
        KEPM_OPEN_PARTY_LIST_NOT v;
        v.m_kPartyInfo = Codec<std::remove_cvref_t<decltype(v.m_kPartyInfo)>>::get(r);
        v.m_vecUserInfo = Codec<std::remove_cvref_t<decltype(v.m_vecUserInfo)>>::get(r);
        return v;
    }
};

struct KEPM_UPDATE_PARTY_MEMBER_IN_PARTY_LIST_NOT {
    std::int64_t                m_iPartyUID{};  // UidType
    std::vector<KPartyUserInfo> m_vecUpdatePartyUser{};  // std::vector< KPartyUserInfo >
    std::vector<std::int64_t>   m_vecDeletePartyUser{};  // std::vector< UidType >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iPartyUID)>>::put(w, m_iPartyUID);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatePartyUser)>>::put(w, m_vecUpdatePartyUser);
        Codec<std::remove_cvref_t<decltype(m_vecDeletePartyUser)>>::put(w, m_vecDeletePartyUser);
    }

    static KEPM_UPDATE_PARTY_MEMBER_IN_PARTY_LIST_NOT read_from(Reader& r) {
        KEPM_UPDATE_PARTY_MEMBER_IN_PARTY_LIST_NOT v;
        v.m_iPartyUID = Codec<std::remove_cvref_t<decltype(v.m_iPartyUID)>>::get(r);
        v.m_vecUpdatePartyUser = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatePartyUser)>>::get(r);
        v.m_vecDeletePartyUser = Codec<std::remove_cvref_t<decltype(v.m_vecDeletePartyUser)>>::get(r);
        return v;
    }
};

struct KPartyUserJoinRoomInfo {
    std::int64_t m_iRoomUID{};  // UidType
    std::int32_t m_iBattleFieldID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iRoomUID)>>::put(w, m_iRoomUID);
        Codec<std::remove_cvref_t<decltype(m_iBattleFieldID)>>::put(w, m_iBattleFieldID);
    }

    static KPartyUserJoinRoomInfo read_from(Reader& r) {
        KPartyUserJoinRoomInfo v;
        v.m_iRoomUID = Codec<std::remove_cvref_t<decltype(v.m_iRoomUID)>>::get(r);
        v.m_iBattleFieldID = Codec<std::remove_cvref_t<decltype(v.m_iBattleFieldID)>>::get(r);
        return v;
    }
};

struct KEPM_UPDATE_PARTY_USER_JOIN_ROOM_INFO_NOT {
    KPartyUserJoinRoomInfo m_kJoinRoomInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kJoinRoomInfo)>>::put(w, m_kJoinRoomInfo);
    }

    static KEPM_UPDATE_PARTY_USER_JOIN_ROOM_INFO_NOT read_from(Reader& r) {
        KEPM_UPDATE_PARTY_USER_JOIN_ROOM_INFO_NOT v;
        v.m_kJoinRoomInfo = Codec<std::remove_cvref_t<decltype(v.m_kJoinRoomInfo)>>::get(r);
        return v;
    }
};

struct KPcBangReward {
    std::int32_t m_iPcBangType{};  // int
    float        m_fEXPRate{};
    float        m_fEDRate{};
    float        m_fVSPointRate{};
    bool         m_bSpiritFree{};
    float        m_fPetEXPRate{};
    std::int32_t m_iAdditionalHenirReward{};  // int
    bool         m_bSecretDungeonFree{};
    bool         m_bAvatar{};
    bool         m_bTitle{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iPcBangType)>>::put(w, m_iPcBangType);
        Codec<std::remove_cvref_t<decltype(m_fEXPRate)>>::put(w, m_fEXPRate);
        Codec<std::remove_cvref_t<decltype(m_fEDRate)>>::put(w, m_fEDRate);
        Codec<std::remove_cvref_t<decltype(m_fVSPointRate)>>::put(w, m_fVSPointRate);
        Codec<std::remove_cvref_t<decltype(m_bSpiritFree)>>::put(w, m_bSpiritFree);
        Codec<std::remove_cvref_t<decltype(m_fPetEXPRate)>>::put(w, m_fPetEXPRate);
        Codec<std::remove_cvref_t<decltype(m_iAdditionalHenirReward)>>::put(w, m_iAdditionalHenirReward);
        Codec<std::remove_cvref_t<decltype(m_bSecretDungeonFree)>>::put(w, m_bSecretDungeonFree);
        Codec<std::remove_cvref_t<decltype(m_bAvatar)>>::put(w, m_bAvatar);
        Codec<std::remove_cvref_t<decltype(m_bTitle)>>::put(w, m_bTitle);
    }

    static KPcBangReward read_from(Reader& r) {
        KPcBangReward v;
        v.m_iPcBangType = Codec<std::remove_cvref_t<decltype(v.m_iPcBangType)>>::get(r);
        v.m_fEXPRate = Codec<std::remove_cvref_t<decltype(v.m_fEXPRate)>>::get(r);
        v.m_fEDRate = Codec<std::remove_cvref_t<decltype(v.m_fEDRate)>>::get(r);
        v.m_fVSPointRate = Codec<std::remove_cvref_t<decltype(v.m_fVSPointRate)>>::get(r);
        v.m_bSpiritFree = Codec<std::remove_cvref_t<decltype(v.m_bSpiritFree)>>::get(r);
        v.m_fPetEXPRate = Codec<std::remove_cvref_t<decltype(v.m_fPetEXPRate)>>::get(r);
        v.m_iAdditionalHenirReward = Codec<std::remove_cvref_t<decltype(v.m_iAdditionalHenirReward)>>::get(r);
        v.m_bSecretDungeonFree = Codec<std::remove_cvref_t<decltype(v.m_bSecretDungeonFree)>>::get(r);
        v.m_bAvatar = Codec<std::remove_cvref_t<decltype(v.m_bAvatar)>>::get(r);
        v.m_bTitle = Codec<std::remove_cvref_t<decltype(v.m_bTitle)>>::get(r);
        return v;
    }
};

struct KEGS_ADMIN_SET_PC_BANG_ACK {
    std::int32_t  m_iOK{};  // int
    bool          m_bOn{};
    std::int32_t  m_iPcBangType{};  // int
    KPcBangReward m_kPcBangReward{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_bOn)>>::put(w, m_bOn);
        Codec<std::remove_cvref_t<decltype(m_iPcBangType)>>::put(w, m_iPcBangType);
        Codec<std::remove_cvref_t<decltype(m_kPcBangReward)>>::put(w, m_kPcBangReward);
    }

    static KEGS_ADMIN_SET_PC_BANG_ACK read_from(Reader& r) {
        KEGS_ADMIN_SET_PC_BANG_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_bOn = Codec<std::remove_cvref_t<decltype(v.m_bOn)>>::get(r);
        v.m_iPcBangType = Codec<std::remove_cvref_t<decltype(v.m_iPcBangType)>>::get(r);
        v.m_kPcBangReward = Codec<std::remove_cvref_t<decltype(v.m_kPcBangReward)>>::get(r);
        return v;
    }
};

struct KENX_USER_LOGIN_NOT {
    bool          m_bIsGameBang{};
    KPcBangReward m_kPcBangReward{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bIsGameBang)>>::put(w, m_bIsGameBang);
        Codec<std::remove_cvref_t<decltype(m_kPcBangReward)>>::put(w, m_kPcBangReward);
    }

    static KENX_USER_LOGIN_NOT read_from(Reader& r) {
        KENX_USER_LOGIN_NOT v;
        v.m_bIsGameBang = Codec<std::remove_cvref_t<decltype(v.m_bIsGameBang)>>::get(r);
        v.m_kPcBangReward = Codec<std::remove_cvref_t<decltype(v.m_kPcBangReward)>>::get(r);
        return v;
    }
};

struct KPersonalShopItemInfo {
    std::int64_t m_iItemUID{};  // UidType
    std::int32_t m_iQuantity{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_iQuantity)>>::put(w, m_iQuantity);
    }

    static KPersonalShopItemInfo read_from(Reader& r) {
        KPersonalShopItemInfo v;
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_iQuantity = Codec<std::remove_cvref_t<decltype(v.m_iQuantity)>>::get(r);
        return v;
    }
};

struct KPersonalShopUserInfo {
    std::int64_t   m_iGSUID{};  // UidType
    std::int64_t   m_iUserUID{};  // UidType
    std::int64_t   m_iUnitUID{};  // UidType
    std::u16string m_wstrNickName{};  // std::wstring
    bool           m_bIsSquare{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iGSUID)>>::put(w, m_iGSUID);
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_bIsSquare)>>::put(w, m_bIsSquare);
    }

    static KPersonalShopUserInfo read_from(Reader& r) {
        KPersonalShopUserInfo v;
        v.m_iGSUID = Codec<std::remove_cvref_t<decltype(v.m_iGSUID)>>::get(r);
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_bIsSquare = Codec<std::remove_cvref_t<decltype(v.m_bIsSquare)>>::get(r);
        return v;
    }
};

struct KPetInfo {
    std::int64_t   m_iPetUID{};  // UidType
    std::int32_t   m_iPetID{};  // int
    std::u16string m_wstrPetName{};  // std::wstring
    char           m_cEvolutionStep{};
    std::int16_t   m_sSatiety{};  // short
    std::int32_t   m_iIntimacy{};  // int
    std::int16_t   m_sExtroversion{};  // short
    std::int16_t   m_sEmotion{};  // short
    bool           m_bAutoFeed{};
    std::u16string m_wstrLastFeedDate{};  // std::wstring
    std::u16string m_wstrLastSummonDate{};  // std::wstring
    std::u16string m_wstrRegDate{};  // std::wstring
    bool           m_bAutoLooting{};
    bool           m_bFreeAutoLooting{};
    std::u16string m_wstrDestroyDate{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iPetUID)>>::put(w, m_iPetUID);
        Codec<std::remove_cvref_t<decltype(m_iPetID)>>::put(w, m_iPetID);
        Codec<std::remove_cvref_t<decltype(m_wstrPetName)>>::put(w, m_wstrPetName);
        Codec<std::remove_cvref_t<decltype(m_cEvolutionStep)>>::put(w, m_cEvolutionStep);
        Codec<std::remove_cvref_t<decltype(m_sSatiety)>>::put(w, m_sSatiety);
        Codec<std::remove_cvref_t<decltype(m_iIntimacy)>>::put(w, m_iIntimacy);
        Codec<std::remove_cvref_t<decltype(m_sExtroversion)>>::put(w, m_sExtroversion);
        Codec<std::remove_cvref_t<decltype(m_sEmotion)>>::put(w, m_sEmotion);
        Codec<std::remove_cvref_t<decltype(m_bAutoFeed)>>::put(w, m_bAutoFeed);
        Codec<std::remove_cvref_t<decltype(m_wstrLastFeedDate)>>::put(w, m_wstrLastFeedDate);
        Codec<std::remove_cvref_t<decltype(m_wstrLastSummonDate)>>::put(w, m_wstrLastSummonDate);
        Codec<std::remove_cvref_t<decltype(m_wstrRegDate)>>::put(w, m_wstrRegDate);
        Codec<std::remove_cvref_t<decltype(m_bAutoLooting)>>::put(w, m_bAutoLooting);
        Codec<std::remove_cvref_t<decltype(m_bFreeAutoLooting)>>::put(w, m_bFreeAutoLooting);
        Codec<std::remove_cvref_t<decltype(m_wstrDestroyDate)>>::put(w, m_wstrDestroyDate);
    }

    static KPetInfo read_from(Reader& r) {
        KPetInfo v;
        v.m_iPetUID = Codec<std::remove_cvref_t<decltype(v.m_iPetUID)>>::get(r);
        v.m_iPetID = Codec<std::remove_cvref_t<decltype(v.m_iPetID)>>::get(r);
        v.m_wstrPetName = Codec<std::remove_cvref_t<decltype(v.m_wstrPetName)>>::get(r);
        v.m_cEvolutionStep = Codec<std::remove_cvref_t<decltype(v.m_cEvolutionStep)>>::get(r);
        v.m_sSatiety = Codec<std::remove_cvref_t<decltype(v.m_sSatiety)>>::get(r);
        v.m_iIntimacy = Codec<std::remove_cvref_t<decltype(v.m_iIntimacy)>>::get(r);
        v.m_sExtroversion = Codec<std::remove_cvref_t<decltype(v.m_sExtroversion)>>::get(r);
        v.m_sEmotion = Codec<std::remove_cvref_t<decltype(v.m_sEmotion)>>::get(r);
        v.m_bAutoFeed = Codec<std::remove_cvref_t<decltype(v.m_bAutoFeed)>>::get(r);
        v.m_wstrLastFeedDate = Codec<std::remove_cvref_t<decltype(v.m_wstrLastFeedDate)>>::get(r);
        v.m_wstrLastSummonDate = Codec<std::remove_cvref_t<decltype(v.m_wstrLastSummonDate)>>::get(r);
        v.m_wstrRegDate = Codec<std::remove_cvref_t<decltype(v.m_wstrRegDate)>>::get(r);
        v.m_bAutoLooting = Codec<std::remove_cvref_t<decltype(v.m_bAutoLooting)>>::get(r);
        v.m_bFreeAutoLooting = Codec<std::remove_cvref_t<decltype(v.m_bFreeAutoLooting)>>::get(r);
        v.m_wstrDestroyDate = Codec<std::remove_cvref_t<decltype(v.m_wstrDestroyDate)>>::get(r);
        return v;
    }
};

struct KDBE_CHANGE_PET_NAME_ACK {
    std::int64_t                    m_iItemUID{};  // UidType
    std::int32_t                    m_iOK{};  // int
    KPetInfo                        m_kPetInfo{};
    KItemQuantityUpdate             m_kItemQuantityUpdate{};
    std::vector<KInventoryItemInfo> m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_kPetInfo)>>::put(w, m_kPetInfo);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
    }

    static KDBE_CHANGE_PET_NAME_ACK read_from(Reader& r) {
        KDBE_CHANGE_PET_NAME_ACK v;
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_kPetInfo = Codec<std::remove_cvref_t<decltype(v.m_kPetInfo)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        return v;
    }
};

struct KDBE_CHANGE_PET_NAME_REQ {
    std::int64_t                    m_iItemUID{};  // UidType
    std::int64_t                    m_iUnitUID{};  // UidType
    KPetInfo                        m_kPetInfo{};
    KItemQuantityUpdate             m_kItemQuantityUpdate{};
    std::vector<KInventoryItemInfo> m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_kPetInfo)>>::put(w, m_kPetInfo);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
    }

    static KDBE_CHANGE_PET_NAME_REQ read_from(Reader& r) {
        KDBE_CHANGE_PET_NAME_REQ v;
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_kPetInfo = Codec<std::remove_cvref_t<decltype(v.m_kPetInfo)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        return v;
    }
};

struct KDBE_CREATE_PET_ACK {
    std::int32_t                    m_iOK{};  // int
    KPetInfo                        m_kPetInfo{};
    KItemQuantityUpdate             m_kItemQuantityUpdate{};
    std::vector<KInventoryItemInfo> m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_kPetInfo)>>::put(w, m_kPetInfo);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
    }

    static KDBE_CREATE_PET_ACK read_from(Reader& r) {
        KDBE_CREATE_PET_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_kPetInfo = Codec<std::remove_cvref_t<decltype(v.m_kPetInfo)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        return v;
    }
};

struct KDBE_CREATE_PET_REQ {
    std::int64_t                    m_iUnitUID{};  // UidType
    KPetInfo                        m_kPetInfo{};
    std::int16_t                    m_sPeriod{};  // short
    KItemQuantityUpdate             m_kItemQuantityUpdate{};
    std::vector<KInventoryItemInfo> m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_kPetInfo)>>::put(w, m_kPetInfo);
        Codec<std::remove_cvref_t<decltype(m_sPeriod)>>::put(w, m_sPeriod);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
    }

    static KDBE_CREATE_PET_REQ read_from(Reader& r) {
        KDBE_CREATE_PET_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_kPetInfo = Codec<std::remove_cvref_t<decltype(v.m_kPetInfo)>>::get(r);
        v.m_sPeriod = Codec<std::remove_cvref_t<decltype(v.m_sPeriod)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        return v;
    }
};

struct KDBE_UPDATE_UNIT_INFO_ACK {
    std::int32_t                          m_iOK{};  // int
    std::int32_t                          m_iEventID{};  // int
    std::int32_t                          m_iEXP{};  // int
    std::int32_t                          m_iED{};  // int
    std::int32_t                          m_iOfficialMatchCnt{};  // int
    std::int32_t                          m_iRating{};  // int
    std::int32_t                          m_iMaxRating{};  // int
    std::int32_t                          m_iRPoint{};  // int
    std::int32_t                          m_iAPoint{};  // int
    std::int32_t                          m_iWin{};  // int
    std::int32_t                          m_iLose{};  // int
    float                                 m_fKFactor{};
    std::vector<std::int32_t>             m_vecDungeonClear{};  // std::vector< int >
    std::vector<std::int32_t>             m_vecTCClear{};  // std::vector< int >
    KItemQuantityUpdate                   m_kItemQuantityUpdate{};
    std::map<std::int64_t, std::int32_t>  m_kItemEnduranceUpdate{};  // KItemEnduranceUpdate
    std::map<std::int64_t, KItemPosition> m_kItemPositionUpdate{};  // KItemPositionUpdate
    std::int32_t                          m_iNumResurrectionStone{};  // int
    std::int32_t                          m_iNumAutoPaymentResStone{};  // int
    std::vector<KPetInfo>                 m_vecPet{};  // std::vector< KPetInfo >
    std::vector<std::int64_t>             m_vecRidingPetUID{};  // std::vector<UidType>

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iEventID)>>::put(w, m_iEventID);
        Codec<std::remove_cvref_t<decltype(m_iEXP)>>::put(w, m_iEXP);
        Codec<std::remove_cvref_t<decltype(m_iED)>>::put(w, m_iED);
        Codec<std::remove_cvref_t<decltype(m_iOfficialMatchCnt)>>::put(w, m_iOfficialMatchCnt);
        Codec<std::remove_cvref_t<decltype(m_iRating)>>::put(w, m_iRating);
        Codec<std::remove_cvref_t<decltype(m_iMaxRating)>>::put(w, m_iMaxRating);
        Codec<std::remove_cvref_t<decltype(m_iRPoint)>>::put(w, m_iRPoint);
        Codec<std::remove_cvref_t<decltype(m_iAPoint)>>::put(w, m_iAPoint);
        Codec<std::remove_cvref_t<decltype(m_iWin)>>::put(w, m_iWin);
        Codec<std::remove_cvref_t<decltype(m_iLose)>>::put(w, m_iLose);
        Codec<std::remove_cvref_t<decltype(m_fKFactor)>>::put(w, m_fKFactor);
        Codec<std::remove_cvref_t<decltype(m_vecDungeonClear)>>::put(w, m_vecDungeonClear);
        Codec<std::remove_cvref_t<decltype(m_vecTCClear)>>::put(w, m_vecTCClear);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_kItemEnduranceUpdate)>>::put(w, m_kItemEnduranceUpdate);
        Codec<std::remove_cvref_t<decltype(m_kItemPositionUpdate)>>::put(w, m_kItemPositionUpdate);
        Codec<std::remove_cvref_t<decltype(m_iNumResurrectionStone)>>::put(w, m_iNumResurrectionStone);
        Codec<std::remove_cvref_t<decltype(m_iNumAutoPaymentResStone)>>::put(w, m_iNumAutoPaymentResStone);
        Codec<std::remove_cvref_t<decltype(m_vecPet)>>::put(w, m_vecPet);
        Codec<std::remove_cvref_t<decltype(m_vecRidingPetUID)>>::put(w, m_vecRidingPetUID);
    }

    static KDBE_UPDATE_UNIT_INFO_ACK read_from(Reader& r) {
        KDBE_UPDATE_UNIT_INFO_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iEventID = Codec<std::remove_cvref_t<decltype(v.m_iEventID)>>::get(r);
        v.m_iEXP = Codec<std::remove_cvref_t<decltype(v.m_iEXP)>>::get(r);
        v.m_iED = Codec<std::remove_cvref_t<decltype(v.m_iED)>>::get(r);
        v.m_iOfficialMatchCnt = Codec<std::remove_cvref_t<decltype(v.m_iOfficialMatchCnt)>>::get(r);
        v.m_iRating = Codec<std::remove_cvref_t<decltype(v.m_iRating)>>::get(r);
        v.m_iMaxRating = Codec<std::remove_cvref_t<decltype(v.m_iMaxRating)>>::get(r);
        v.m_iRPoint = Codec<std::remove_cvref_t<decltype(v.m_iRPoint)>>::get(r);
        v.m_iAPoint = Codec<std::remove_cvref_t<decltype(v.m_iAPoint)>>::get(r);
        v.m_iWin = Codec<std::remove_cvref_t<decltype(v.m_iWin)>>::get(r);
        v.m_iLose = Codec<std::remove_cvref_t<decltype(v.m_iLose)>>::get(r);
        v.m_fKFactor = Codec<std::remove_cvref_t<decltype(v.m_fKFactor)>>::get(r);
        v.m_vecDungeonClear = Codec<std::remove_cvref_t<decltype(v.m_vecDungeonClear)>>::get(r);
        v.m_vecTCClear = Codec<std::remove_cvref_t<decltype(v.m_vecTCClear)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_kItemEnduranceUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemEnduranceUpdate)>>::get(r);
        v.m_kItemPositionUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemPositionUpdate)>>::get(r);
        v.m_iNumResurrectionStone = Codec<std::remove_cvref_t<decltype(v.m_iNumResurrectionStone)>>::get(r);
        v.m_iNumAutoPaymentResStone = Codec<std::remove_cvref_t<decltype(v.m_iNumAutoPaymentResStone)>>::get(r);
        v.m_vecPet = Codec<std::remove_cvref_t<decltype(v.m_vecPet)>>::get(r);
        v.m_vecRidingPetUID = Codec<std::remove_cvref_t<decltype(v.m_vecRidingPetUID)>>::get(r);
        return v;
    }
};

struct KEGS_CHANGE_PET_NAME_ACK {
    std::int32_t                    m_iOK{};  // int
    KPetInfo                        m_kChangedPetInfo{};
    std::vector<KInventoryItemInfo> m_vecKInventorySlotInfo{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_kChangedPetInfo)>>::put(w, m_kChangedPetInfo);
        Codec<std::remove_cvref_t<decltype(m_vecKInventorySlotInfo)>>::put(w, m_vecKInventorySlotInfo);
    }

    static KEGS_CHANGE_PET_NAME_ACK read_from(Reader& r) {
        KEGS_CHANGE_PET_NAME_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_kChangedPetInfo = Codec<std::remove_cvref_t<decltype(v.m_kChangedPetInfo)>>::get(r);
        v.m_vecKInventorySlotInfo = Codec<std::remove_cvref_t<decltype(v.m_vecKInventorySlotInfo)>>::get(r);
        return v;
    }
};

struct KEGS_CREATE_PET_ACK {
    std::int32_t                    m_iOK{};  // int
    KPetInfo                        m_kCreatedPetInfo{};
    std::vector<KInventoryItemInfo> m_vecKInventorySlotInfo{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_kCreatedPetInfo)>>::put(w, m_kCreatedPetInfo);
        Codec<std::remove_cvref_t<decltype(m_vecKInventorySlotInfo)>>::put(w, m_vecKInventorySlotInfo);
    }

    static KEGS_CREATE_PET_ACK read_from(Reader& r) {
        KEGS_CREATE_PET_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_kCreatedPetInfo = Codec<std::remove_cvref_t<decltype(v.m_kCreatedPetInfo)>>::get(r);
        v.m_vecKInventorySlotInfo = Codec<std::remove_cvref_t<decltype(v.m_vecKInventorySlotInfo)>>::get(r);
        return v;
    }
};

struct KEGS_GET_PET_LIST_ACK {
    std::int32_t          m_iOK{};  // int
    std::uint32_t         m_nTotalPage{};  // UINT
    std::uint32_t         m_nViewPage{};  // UINT
    std::vector<KPetInfo> m_vecPetList{};  // std::vector< KPetInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_nTotalPage)>>::put(w, m_nTotalPage);
        Codec<std::remove_cvref_t<decltype(m_nViewPage)>>::put(w, m_nViewPage);
        Codec<std::remove_cvref_t<decltype(m_vecPetList)>>::put(w, m_vecPetList);
    }

    static KEGS_GET_PET_LIST_ACK read_from(Reader& r) {
        KEGS_GET_PET_LIST_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_nTotalPage = Codec<std::remove_cvref_t<decltype(v.m_nTotalPage)>>::get(r);
        v.m_nViewPage = Codec<std::remove_cvref_t<decltype(v.m_nViewPage)>>::get(r);
        v.m_vecPetList = Codec<std::remove_cvref_t<decltype(v.m_vecPetList)>>::get(r);
        return v;
    }
};

struct KEGS_PET_EVOLUTION_ACK {
    std::int32_t m_iOK{};  // int
    KPetInfo     m_kEvolutionResultPetInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_kEvolutionResultPetInfo)>>::put(w, m_kEvolutionResultPetInfo);
    }

    static KEGS_PET_EVOLUTION_ACK read_from(Reader& r) {
        KEGS_PET_EVOLUTION_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_kEvolutionResultPetInfo = Codec<std::remove_cvref_t<decltype(v.m_kEvolutionResultPetInfo)>>::get(r);
        return v;
    }
};

struct KEGS_PET_EVOLUTION_NOT {
    std::int64_t m_iUnitUID{};  // UidType
    KPetInfo     m_kPetInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_kPetInfo)>>::put(w, m_kPetInfo);
    }

    static KEGS_PET_EVOLUTION_NOT read_from(Reader& r) {
        KEGS_PET_EVOLUTION_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_kPetInfo = Codec<std::remove_cvref_t<decltype(v.m_kPetInfo)>>::get(r);
        return v;
    }
};

struct KEGS_SUMMON_PET_ACK {
    std::int32_t                    m_iOK{};  // int
    KPetInfo                        m_kSummonedPetInfo{};
    std::vector<KInventoryItemInfo> m_vecPetInventorySlotInfo{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_kSummonedPetInfo)>>::put(w, m_kSummonedPetInfo);
        Codec<std::remove_cvref_t<decltype(m_vecPetInventorySlotInfo)>>::put(w, m_vecPetInventorySlotInfo);
    }

    static KEGS_SUMMON_PET_ACK read_from(Reader& r) {
        KEGS_SUMMON_PET_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_kSummonedPetInfo = Codec<std::remove_cvref_t<decltype(v.m_kSummonedPetInfo)>>::get(r);
        v.m_vecPetInventorySlotInfo = Codec<std::remove_cvref_t<decltype(v.m_vecPetInventorySlotInfo)>>::get(r);
        return v;
    }
};

struct KEGS_SUMMON_PET_NOT {
    std::int64_t          m_iUnitUID{};  // UidType
    std::vector<KPetInfo> m_vecPetInfo{};  // std::vector< KPetInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_vecPetInfo)>>::put(w, m_vecPetInfo);
    }

    static KEGS_SUMMON_PET_NOT read_from(Reader& r) {
        KEGS_SUMMON_PET_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_vecPetInfo = Codec<std::remove_cvref_t<decltype(v.m_vecPetInfo)>>::get(r);
        return v;
    }
};

struct KERM_PET_EVOLUTION_NOT {
    std::int64_t m_iUnitUID{};  // UidType
    KPetInfo     m_kEvolutionPetInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_kEvolutionPetInfo)>>::put(w, m_kEvolutionPetInfo);
    }

    static KERM_PET_EVOLUTION_NOT read_from(Reader& r) {
        KERM_PET_EVOLUTION_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_kEvolutionPetInfo = Codec<std::remove_cvref_t<decltype(v.m_kEvolutionPetInfo)>>::get(r);
        return v;
    }
};

struct KERM_SUMMON_PET_NOT {
    std::int64_t          m_iUnitUID{};  // UidType
    std::vector<KPetInfo> m_vecPetInfo{};  // std::vector< KPetInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_vecPetInfo)>>::put(w, m_vecPetInfo);
    }

    static KERM_SUMMON_PET_NOT read_from(Reader& r) {
        KERM_SUMMON_PET_NOT v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_vecPetInfo = Codec<std::remove_cvref_t<decltype(v.m_vecPetInfo)>>::get(r);
        return v;
    }
};

struct KETR_PET_EVOLUTION_NOT {
    std::vector<std::int64_t> m_vecUID{};  // std::vector< UidType >
    std::int64_t              m_iUnitUID{};  // UidType
    KPetInfo                  m_kEvolutionPetInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecUID)>>::put(w, m_vecUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_kEvolutionPetInfo)>>::put(w, m_kEvolutionPetInfo);
    }

    static KETR_PET_EVOLUTION_NOT read_from(Reader& r) {
        KETR_PET_EVOLUTION_NOT v;
        v.m_vecUID = Codec<std::remove_cvref_t<decltype(v.m_vecUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_kEvolutionPetInfo = Codec<std::remove_cvref_t<decltype(v.m_kEvolutionPetInfo)>>::get(r);
        return v;
    }
};

struct KETR_SUMMON_PET_NOT {
    std::vector<std::int64_t> m_vecUID{};  // std::vector< UidType >
    std::int64_t              m_iUnitUID{};  // UidType
    std::vector<KPetInfo>     m_vecPetInfo{};  // std::vector< KPetInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecUID)>>::put(w, m_vecUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_vecPetInfo)>>::put(w, m_vecPetInfo);
    }

    static KETR_SUMMON_PET_NOT read_from(Reader& r) {
        KETR_SUMMON_PET_NOT v;
        v.m_vecUID = Codec<std::remove_cvref_t<decltype(v.m_vecUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_vecPetInfo = Codec<std::remove_cvref_t<decltype(v.m_vecPetInfo)>>::get(r);
        return v;
    }
};

struct KPostItemInfo {
    std::int64_t              m_iPostNo{};  // UidType
    std::int64_t              m_iFromUnitUID{};  // UidType
    std::int64_t              m_iToUnitUID{};  // UidType
    char                      m_cScriptType{};
    std::int32_t              m_iScriptIndex{};  // int
    std::int32_t              m_iQuantity{};  // int
    char                      m_cEnchantLevel{};
    KItemAttributeEnchantInfo m_kAttribEnchantInfo{};
    std::vector<std::int32_t> m_vecItemSocket{};  // std::vector< int >
    std::vector<std::int32_t> m_vecRandomSocket{};  // std::vector< int >
    char                      m_cItemState{};
    std::uint8_t              m_ucSealData{};  // unsigned char
    std::u16string            m_wstrRegDate{};  // std::wstring
    std::u16string            m_wstrFromNickName{};  // std::wstring
    std::u16string            m_wstrTitle{};  // std::wstring
    std::u16string            m_wstrMessage{};  // std::wstring
    bool                      m_bRead{};
    std::uint8_t              m_byteExpandedSocketNum{};  // byte

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iPostNo)>>::put(w, m_iPostNo);
        Codec<std::remove_cvref_t<decltype(m_iFromUnitUID)>>::put(w, m_iFromUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iToUnitUID)>>::put(w, m_iToUnitUID);
        Codec<std::remove_cvref_t<decltype(m_cScriptType)>>::put(w, m_cScriptType);
        Codec<std::remove_cvref_t<decltype(m_iScriptIndex)>>::put(w, m_iScriptIndex);
        Codec<std::remove_cvref_t<decltype(m_iQuantity)>>::put(w, m_iQuantity);
        Codec<std::remove_cvref_t<decltype(m_cEnchantLevel)>>::put(w, m_cEnchantLevel);
        Codec<std::remove_cvref_t<decltype(m_kAttribEnchantInfo)>>::put(w, m_kAttribEnchantInfo);
        Codec<std::remove_cvref_t<decltype(m_vecItemSocket)>>::put(w, m_vecItemSocket);
        Codec<std::remove_cvref_t<decltype(m_vecRandomSocket)>>::put(w, m_vecRandomSocket);
        Codec<std::remove_cvref_t<decltype(m_cItemState)>>::put(w, m_cItemState);
        Codec<std::remove_cvref_t<decltype(m_ucSealData)>>::put(w, m_ucSealData);
        Codec<std::remove_cvref_t<decltype(m_wstrRegDate)>>::put(w, m_wstrRegDate);
        Codec<std::remove_cvref_t<decltype(m_wstrFromNickName)>>::put(w, m_wstrFromNickName);
        Codec<std::remove_cvref_t<decltype(m_wstrTitle)>>::put(w, m_wstrTitle);
        Codec<std::remove_cvref_t<decltype(m_wstrMessage)>>::put(w, m_wstrMessage);
        Codec<std::remove_cvref_t<decltype(m_bRead)>>::put(w, m_bRead);
        Codec<std::remove_cvref_t<decltype(m_byteExpandedSocketNum)>>::put(w, m_byteExpandedSocketNum);
    }

    static KPostItemInfo read_from(Reader& r) {
        KPostItemInfo v;
        v.m_iPostNo = Codec<std::remove_cvref_t<decltype(v.m_iPostNo)>>::get(r);
        v.m_iFromUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iFromUnitUID)>>::get(r);
        v.m_iToUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iToUnitUID)>>::get(r);
        v.m_cScriptType = Codec<std::remove_cvref_t<decltype(v.m_cScriptType)>>::get(r);
        v.m_iScriptIndex = Codec<std::remove_cvref_t<decltype(v.m_iScriptIndex)>>::get(r);
        v.m_iQuantity = Codec<std::remove_cvref_t<decltype(v.m_iQuantity)>>::get(r);
        v.m_cEnchantLevel = Codec<std::remove_cvref_t<decltype(v.m_cEnchantLevel)>>::get(r);
        v.m_kAttribEnchantInfo = Codec<std::remove_cvref_t<decltype(v.m_kAttribEnchantInfo)>>::get(r);
        v.m_vecItemSocket = Codec<std::remove_cvref_t<decltype(v.m_vecItemSocket)>>::get(r);
        v.m_vecRandomSocket = Codec<std::remove_cvref_t<decltype(v.m_vecRandomSocket)>>::get(r);
        v.m_cItemState = Codec<std::remove_cvref_t<decltype(v.m_cItemState)>>::get(r);
        v.m_ucSealData = Codec<std::remove_cvref_t<decltype(v.m_ucSealData)>>::get(r);
        v.m_wstrRegDate = Codec<std::remove_cvref_t<decltype(v.m_wstrRegDate)>>::get(r);
        v.m_wstrFromNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrFromNickName)>>::get(r);
        v.m_wstrTitle = Codec<std::remove_cvref_t<decltype(v.m_wstrTitle)>>::get(r);
        v.m_wstrMessage = Codec<std::remove_cvref_t<decltype(v.m_wstrMessage)>>::get(r);
        v.m_bRead = Codec<std::remove_cvref_t<decltype(v.m_bRead)>>::get(r);
        v.m_byteExpandedSocketNum = Codec<std::remove_cvref_t<decltype(v.m_byteExpandedSocketNum)>>::get(r);
        return v;
    }
};

struct KDBE_GET_POST_LETTER_LIST_ACK {
    std::int32_t                m_iOK{};  // int
    std::vector<KPostItemInfo>  m_vecPostItem{};  // std::vector< KPostItemInfo >
    std::vector<std::u16string> m_vecBlackList{};  // std::vector< std::wstring >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_vecPostItem)>>::put(w, m_vecPostItem);
        Codec<std::remove_cvref_t<decltype(m_vecBlackList)>>::put(w, m_vecBlackList);
    }

    static KDBE_GET_POST_LETTER_LIST_ACK read_from(Reader& r) {
        KDBE_GET_POST_LETTER_LIST_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_vecPostItem = Codec<std::remove_cvref_t<decltype(v.m_vecPostItem)>>::get(r);
        v.m_vecBlackList = Codec<std::remove_cvref_t<decltype(v.m_vecBlackList)>>::get(r);
        return v;
    }
};

struct KDBE_INSERT_LETTER_TO_POST_ACK {
    std::int32_t                    m_iOK{};  // int
    std::u16string                  m_wstrToNickName{};  // std::wstring
    KPostItemInfo                   m_kSendLetter{};
    KItemQuantityUpdate             m_kItemQuantityUpdate{};
    std::vector<KInventoryItemInfo> m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >
    std::int32_t                    m_iSendLetterCost{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_wstrToNickName)>>::put(w, m_wstrToNickName);
        Codec<std::remove_cvref_t<decltype(m_kSendLetter)>>::put(w, m_kSendLetter);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
        Codec<std::remove_cvref_t<decltype(m_iSendLetterCost)>>::put(w, m_iSendLetterCost);
    }

    static KDBE_INSERT_LETTER_TO_POST_ACK read_from(Reader& r) {
        KDBE_INSERT_LETTER_TO_POST_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_wstrToNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrToNickName)>>::get(r);
        v.m_kSendLetter = Codec<std::remove_cvref_t<decltype(v.m_kSendLetter)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        v.m_iSendLetterCost = Codec<std::remove_cvref_t<decltype(v.m_iSendLetterCost)>>::get(r);
        return v;
    }
};

struct KDBE_INSERT_LETTER_TO_POST_REQ {
    std::int64_t                    m_iFromUnitUID{};  // UidType
    std::u16string                  m_wstrToNickName{};  // std::wstring
    KPostItemInfo                   m_kSendLetter{};
    KItemQuantityUpdate             m_kItemQuantityUpdate{};
    std::vector<KInventoryItemInfo> m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >
    std::int32_t                    m_iSendLetterCost{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iFromUnitUID)>>::put(w, m_iFromUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrToNickName)>>::put(w, m_wstrToNickName);
        Codec<std::remove_cvref_t<decltype(m_kSendLetter)>>::put(w, m_kSendLetter);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
        Codec<std::remove_cvref_t<decltype(m_iSendLetterCost)>>::put(w, m_iSendLetterCost);
    }

    static KDBE_INSERT_LETTER_TO_POST_REQ read_from(Reader& r) {
        KDBE_INSERT_LETTER_TO_POST_REQ v;
        v.m_iFromUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iFromUnitUID)>>::get(r);
        v.m_wstrToNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrToNickName)>>::get(r);
        v.m_kSendLetter = Codec<std::remove_cvref_t<decltype(v.m_kSendLetter)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        v.m_iSendLetterCost = Codec<std::remove_cvref_t<decltype(v.m_iSendLetterCost)>>::get(r);
        return v;
    }
};

struct KDBE_INSERT_POST_ITEM_TO_INVENTORY_ACK {
    std::int32_t                         m_iOK{};  // int
    std::map<std::int32_t, std::int32_t> m_mapInsertedItem{};  // std::map< int, int >
    KItemQuantityUpdate                  m_kItemQuantityUpdate{};
    std::vector<KInventoryItemInfo>      m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >
    std::map<std::int64_t, KItemInfo>    m_mapItemInfo{};  // std::map< UidType, KItemInfo >
    std::vector<KPostItemInfo>           m_vecPostItemInfo{};  // std::vector< KPostItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_mapInsertedItem)>>::put(w, m_mapInsertedItem);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
        Codec<std::remove_cvref_t<decltype(m_mapItemInfo)>>::put(w, m_mapItemInfo);
        Codec<std::remove_cvref_t<decltype(m_vecPostItemInfo)>>::put(w, m_vecPostItemInfo);
    }

    static KDBE_INSERT_POST_ITEM_TO_INVENTORY_ACK read_from(Reader& r) {
        KDBE_INSERT_POST_ITEM_TO_INVENTORY_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_mapInsertedItem = Codec<std::remove_cvref_t<decltype(v.m_mapInsertedItem)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        v.m_mapItemInfo = Codec<std::remove_cvref_t<decltype(v.m_mapItemInfo)>>::get(r);
        v.m_vecPostItemInfo = Codec<std::remove_cvref_t<decltype(v.m_vecPostItemInfo)>>::get(r);
        return v;
    }
};

struct KDBE_INSERT_POST_ITEM_TO_INVENTORY_REQ {
    std::int64_t                         m_iUnitUID{};  // UidType
    std::map<std::int32_t, std::int32_t> m_mapInsertedItem{};  // std::map< int, int >
    KItemQuantityUpdate                  m_kItemQuantityUpdate{};
    std::vector<KInventoryItemInfo>      m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >
    std::vector<KItemInfo>               m_vecItemInfo{};  // std::vector< KItemInfo >
    std::vector<KPostItemInfo>           m_vecPostItemInfo{};  // std::vector< KPostItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_mapInsertedItem)>>::put(w, m_mapInsertedItem);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
        Codec<std::remove_cvref_t<decltype(m_vecItemInfo)>>::put(w, m_vecItemInfo);
        Codec<std::remove_cvref_t<decltype(m_vecPostItemInfo)>>::put(w, m_vecPostItemInfo);
    }

    static KDBE_INSERT_POST_ITEM_TO_INVENTORY_REQ read_from(Reader& r) {
        KDBE_INSERT_POST_ITEM_TO_INVENTORY_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_mapInsertedItem = Codec<std::remove_cvref_t<decltype(v.m_mapInsertedItem)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        v.m_vecItemInfo = Codec<std::remove_cvref_t<decltype(v.m_vecItemInfo)>>::get(r);
        v.m_vecPostItemInfo = Codec<std::remove_cvref_t<decltype(v.m_vecPostItemInfo)>>::get(r);
        return v;
    }
};

struct KDBE_INSERT_REWARD_TO_POST_ACK {
    std::int32_t  m_iOK{};  // int
    KPostItemInfo m_iRewardLetter{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iRewardLetter)>>::put(w, m_iRewardLetter);
    }

    static KDBE_INSERT_REWARD_TO_POST_ACK read_from(Reader& r) {
        KDBE_INSERT_REWARD_TO_POST_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iRewardLetter = Codec<std::remove_cvref_t<decltype(v.m_iRewardLetter)>>::get(r);
        return v;
    }
};

struct KDBE_PREPARE_INSERT_LETTER_TO_POST_ACK {
    std::int32_t   m_iOK{};  // int
    std::int64_t   m_iItemUID{};  // UidType
    std::u16string m_wstrToNickName{};  // std::wstring
    KPostItemInfo  m_kSendLetter{};
    std::int32_t   m_iSendLetterCost{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_wstrToNickName)>>::put(w, m_wstrToNickName);
        Codec<std::remove_cvref_t<decltype(m_kSendLetter)>>::put(w, m_kSendLetter);
        Codec<std::remove_cvref_t<decltype(m_iSendLetterCost)>>::put(w, m_iSendLetterCost);
    }

    static KDBE_PREPARE_INSERT_LETTER_TO_POST_ACK read_from(Reader& r) {
        KDBE_PREPARE_INSERT_LETTER_TO_POST_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_wstrToNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrToNickName)>>::get(r);
        v.m_kSendLetter = Codec<std::remove_cvref_t<decltype(v.m_kSendLetter)>>::get(r);
        v.m_iSendLetterCost = Codec<std::remove_cvref_t<decltype(v.m_iSendLetterCost)>>::get(r);
        return v;
    }
};

struct KDBE_PREPARE_INSERT_LETTER_TO_POST_REQ {
    std::int64_t   m_iFromUnitUID{};  // UidType
    std::int64_t   m_iItemUID{};  // UidType
    std::u16string m_wstrToNickName{};  // std::wstring
    KPostItemInfo  m_kSendLetter{};
    std::int32_t   m_iSendLetterCost{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iFromUnitUID)>>::put(w, m_iFromUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_wstrToNickName)>>::put(w, m_wstrToNickName);
        Codec<std::remove_cvref_t<decltype(m_kSendLetter)>>::put(w, m_kSendLetter);
        Codec<std::remove_cvref_t<decltype(m_iSendLetterCost)>>::put(w, m_iSendLetterCost);
    }

    static KDBE_PREPARE_INSERT_LETTER_TO_POST_REQ read_from(Reader& r) {
        KDBE_PREPARE_INSERT_LETTER_TO_POST_REQ v;
        v.m_iFromUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iFromUnitUID)>>::get(r);
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_wstrToNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrToNickName)>>::get(r);
        v.m_kSendLetter = Codec<std::remove_cvref_t<decltype(v.m_kSendLetter)>>::get(r);
        v.m_iSendLetterCost = Codec<std::remove_cvref_t<decltype(v.m_iSendLetterCost)>>::get(r);
        return v;
    }
};

struct KEGS_READ_LETTER_ACK {
    std::int32_t  m_iOK{};  // int
    KPostItemInfo m_kLetter{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_kLetter)>>::put(w, m_kLetter);
    }

    static KEGS_READ_LETTER_ACK read_from(Reader& r) {
        KEGS_READ_LETTER_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_kLetter = Codec<std::remove_cvref_t<decltype(v.m_kLetter)>>::get(r);
        return v;
    }
};

struct KELG_FIND_USER_SEND_LETTER_NOT {
    KPostItemInfo m_kLetter{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kLetter)>>::put(w, m_kLetter);
    }

    static KELG_FIND_USER_SEND_LETTER_NOT read_from(Reader& r) {
        KELG_FIND_USER_SEND_LETTER_NOT v;
        v.m_kLetter = Codec<std::remove_cvref_t<decltype(v.m_kLetter)>>::get(r);
        return v;
    }
};

struct KPostItemTitleInfo {
    std::int64_t   m_iPostNo{};  // UidType
    char           m_cScriptType{};
    std::int32_t   m_iScriptIndex{};  // int
    char           m_cEnchantLevel{};
    std::u16string m_wstrRegDate{};  // std::wstring
    std::u16string m_wstrFromNickName{};  // std::wstring
    std::u16string m_wstrTitle{};  // std::wstring
    bool           m_bRead{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iPostNo)>>::put(w, m_iPostNo);
        Codec<std::remove_cvref_t<decltype(m_cScriptType)>>::put(w, m_cScriptType);
        Codec<std::remove_cvref_t<decltype(m_iScriptIndex)>>::put(w, m_iScriptIndex);
        Codec<std::remove_cvref_t<decltype(m_cEnchantLevel)>>::put(w, m_cEnchantLevel);
        Codec<std::remove_cvref_t<decltype(m_wstrRegDate)>>::put(w, m_wstrRegDate);
        Codec<std::remove_cvref_t<decltype(m_wstrFromNickName)>>::put(w, m_wstrFromNickName);
        Codec<std::remove_cvref_t<decltype(m_wstrTitle)>>::put(w, m_wstrTitle);
        Codec<std::remove_cvref_t<decltype(m_bRead)>>::put(w, m_bRead);
    }

    static KPostItemTitleInfo read_from(Reader& r) {
        KPostItemTitleInfo v;
        v.m_iPostNo = Codec<std::remove_cvref_t<decltype(v.m_iPostNo)>>::get(r);
        v.m_cScriptType = Codec<std::remove_cvref_t<decltype(v.m_cScriptType)>>::get(r);
        v.m_iScriptIndex = Codec<std::remove_cvref_t<decltype(v.m_iScriptIndex)>>::get(r);
        v.m_cEnchantLevel = Codec<std::remove_cvref_t<decltype(v.m_cEnchantLevel)>>::get(r);
        v.m_wstrRegDate = Codec<std::remove_cvref_t<decltype(v.m_wstrRegDate)>>::get(r);
        v.m_wstrFromNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrFromNickName)>>::get(r);
        v.m_wstrTitle = Codec<std::remove_cvref_t<decltype(v.m_wstrTitle)>>::get(r);
        v.m_bRead = Codec<std::remove_cvref_t<decltype(v.m_bRead)>>::get(r);
        return v;
    }
};

struct KEGS_GET_POST_LETTER_LIST_ACK {
    std::int32_t                    m_iOK{};  // int
    std::int32_t                    m_iTotalLetterCount{};  // int
    std::vector<KPostItemTitleInfo> m_vecPostItemTitle{};  // std::vector< KPostItemTitleInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iTotalLetterCount)>>::put(w, m_iTotalLetterCount);
        Codec<std::remove_cvref_t<decltype(m_vecPostItemTitle)>>::put(w, m_vecPostItemTitle);
    }

    static KEGS_GET_POST_LETTER_LIST_ACK read_from(Reader& r) {
        KEGS_GET_POST_LETTER_LIST_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iTotalLetterCount = Codec<std::remove_cvref_t<decltype(v.m_iTotalLetterCount)>>::get(r);
        v.m_vecPostItemTitle = Codec<std::remove_cvref_t<decltype(v.m_vecPostItemTitle)>>::get(r);
        return v;
    }
};

struct KPostSmallTitleInfo {
    char           m_cScriptType{};
    std::u16string m_wstrLetterTitle{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cScriptType)>>::put(w, m_cScriptType);
        Codec<std::remove_cvref_t<decltype(m_wstrLetterTitle)>>::put(w, m_wstrLetterTitle);
    }

    static KPostSmallTitleInfo read_from(Reader& r) {
        KPostSmallTitleInfo v;
        v.m_cScriptType = Codec<std::remove_cvref_t<decltype(v.m_cScriptType)>>::get(r);
        v.m_wstrLetterTitle = Codec<std::remove_cvref_t<decltype(v.m_wstrLetterTitle)>>::get(r);
        return v;
    }
};

struct KEGS_NEW_POST_LETTER_INFO_NOT {
    std::int32_t                     m_iNewUserLetterCount{};  // int
    std::int32_t                     m_iNewSystemLetterCount{};  // int
    std::int32_t                     m_iNewRelationshipLetterCount{};  // int
    std::int32_t                     m_iTotalLetterCount{};  // int
    std::vector<KPostSmallTitleInfo> m_vecNewLetterTitle{};  // std::vector< KPostSmallTitleInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iNewUserLetterCount)>>::put(w, m_iNewUserLetterCount);
        Codec<std::remove_cvref_t<decltype(m_iNewSystemLetterCount)>>::put(w, m_iNewSystemLetterCount);
        Codec<std::remove_cvref_t<decltype(m_iNewRelationshipLetterCount)>>::put(w, m_iNewRelationshipLetterCount);
        Codec<std::remove_cvref_t<decltype(m_iTotalLetterCount)>>::put(w, m_iTotalLetterCount);
        Codec<std::remove_cvref_t<decltype(m_vecNewLetterTitle)>>::put(w, m_vecNewLetterTitle);
    }

    static KEGS_NEW_POST_LETTER_INFO_NOT read_from(Reader& r) {
        KEGS_NEW_POST_LETTER_INFO_NOT v;
        v.m_iNewUserLetterCount = Codec<std::remove_cvref_t<decltype(v.m_iNewUserLetterCount)>>::get(r);
        v.m_iNewSystemLetterCount = Codec<std::remove_cvref_t<decltype(v.m_iNewSystemLetterCount)>>::get(r);
        v.m_iNewRelationshipLetterCount = Codec<std::remove_cvref_t<decltype(v.m_iNewRelationshipLetterCount)>>::get(r);
        v.m_iTotalLetterCount = Codec<std::remove_cvref_t<decltype(v.m_iTotalLetterCount)>>::get(r);
        v.m_vecNewLetterTitle = Codec<std::remove_cvref_t<decltype(v.m_vecNewLetterTitle)>>::get(r);
        return v;
    }
};

struct KPropertyValue {
    std::uint8_t               m_bytePropertyValue{};  // BYTE
    std::uint32_t              m_uintPropertyValue{};  // unsigned int
    std::uint32_t              m_ulongPropertyValue{};  // unsigned long
    std::u16string             m_wstrPropertyValue{};  // std::wstring
    std::vector<std::uint32_t> m_vecPropertyValue{};  // std::vector< unsigned long >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bytePropertyValue)>>::put(w, m_bytePropertyValue);
        Codec<std::remove_cvref_t<decltype(m_uintPropertyValue)>>::put(w, m_uintPropertyValue);
        Codec<std::remove_cvref_t<decltype(m_ulongPropertyValue)>>::put(w, m_ulongPropertyValue);
        Codec<std::remove_cvref_t<decltype(m_wstrPropertyValue)>>::put(w, m_wstrPropertyValue);
        Codec<std::remove_cvref_t<decltype(m_vecPropertyValue)>>::put(w, m_vecPropertyValue);
    }

    static KPropertyValue read_from(Reader& r) {
        KPropertyValue v;
        v.m_bytePropertyValue = Codec<std::remove_cvref_t<decltype(v.m_bytePropertyValue)>>::get(r);
        v.m_uintPropertyValue = Codec<std::remove_cvref_t<decltype(v.m_uintPropertyValue)>>::get(r);
        v.m_ulongPropertyValue = Codec<std::remove_cvref_t<decltype(v.m_ulongPropertyValue)>>::get(r);
        v.m_wstrPropertyValue = Codec<std::remove_cvref_t<decltype(v.m_wstrPropertyValue)>>::get(r);
        v.m_vecPropertyValue = Codec<std::remove_cvref_t<decltype(v.m_vecPropertyValue)>>::get(r);
        return v;
    }
};

struct KENX_AUTH_LOGIN_ACK {
    std::uint8_t                            m_bytePacketType{};  // BYTE
    std::uint8_t                            m_byteLoginType{};  // BYTE
    std::int64_t                            m_iSessionNo{};  // __int64
    std::u16string                          m_wstrUserID{};  // std::wstring
    std::uint8_t                            m_byteAuthorizeResult{};  // BYTE
    std::uint8_t                            m_byteAuthorizeType{};  // BYTE
    std::uint8_t                            m_byteChargeType{};  // BYTE
    std::uint8_t                            m_byteOption{};  // BYTE
    std::uint32_t                           m_ulArgument{};  // unsigned long
    std::uint8_t                            m_byteIsCharged{};  // BYTE
    std::map<std::uint16_t, KPropertyValue> m_mapProperty{};  // std::map< unsigned short, KPropertyValue >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bytePacketType)>>::put(w, m_bytePacketType);
        Codec<std::remove_cvref_t<decltype(m_byteLoginType)>>::put(w, m_byteLoginType);
        Codec<std::remove_cvref_t<decltype(m_iSessionNo)>>::put(w, m_iSessionNo);
        Codec<std::remove_cvref_t<decltype(m_wstrUserID)>>::put(w, m_wstrUserID);
        Codec<std::remove_cvref_t<decltype(m_byteAuthorizeResult)>>::put(w, m_byteAuthorizeResult);
        Codec<std::remove_cvref_t<decltype(m_byteAuthorizeType)>>::put(w, m_byteAuthorizeType);
        Codec<std::remove_cvref_t<decltype(m_byteChargeType)>>::put(w, m_byteChargeType);
        Codec<std::remove_cvref_t<decltype(m_byteOption)>>::put(w, m_byteOption);
        Codec<std::remove_cvref_t<decltype(m_ulArgument)>>::put(w, m_ulArgument);
        Codec<std::remove_cvref_t<decltype(m_byteIsCharged)>>::put(w, m_byteIsCharged);
        Codec<std::remove_cvref_t<decltype(m_mapProperty)>>::put(w, m_mapProperty);
    }

    static KENX_AUTH_LOGIN_ACK read_from(Reader& r) {
        KENX_AUTH_LOGIN_ACK v;
        v.m_bytePacketType = Codec<std::remove_cvref_t<decltype(v.m_bytePacketType)>>::get(r);
        v.m_byteLoginType = Codec<std::remove_cvref_t<decltype(v.m_byteLoginType)>>::get(r);
        v.m_iSessionNo = Codec<std::remove_cvref_t<decltype(v.m_iSessionNo)>>::get(r);
        v.m_wstrUserID = Codec<std::remove_cvref_t<decltype(v.m_wstrUserID)>>::get(r);
        v.m_byteAuthorizeResult = Codec<std::remove_cvref_t<decltype(v.m_byteAuthorizeResult)>>::get(r);
        v.m_byteAuthorizeType = Codec<std::remove_cvref_t<decltype(v.m_byteAuthorizeType)>>::get(r);
        v.m_byteChargeType = Codec<std::remove_cvref_t<decltype(v.m_byteChargeType)>>::get(r);
        v.m_byteOption = Codec<std::remove_cvref_t<decltype(v.m_byteOption)>>::get(r);
        v.m_ulArgument = Codec<std::remove_cvref_t<decltype(v.m_ulArgument)>>::get(r);
        v.m_byteIsCharged = Codec<std::remove_cvref_t<decltype(v.m_byteIsCharged)>>::get(r);
        v.m_mapProperty = Codec<std::remove_cvref_t<decltype(v.m_mapProperty)>>::get(r);
        return v;
    }
};

struct KENX_AUTH_LOGIN_REQ {
    std::uint8_t                            m_bytePacketType{};  // BYTE
    std::uint8_t                            m_byteLoginType{};  // BYTE
    std::u16string                          m_wstrUserID{};  // std::wstring
    std::uint32_t                           m_uiIP{};  // unsigned int
    std::map<std::uint16_t, KPropertyValue> m_mapProperty{};  // std::map< unsigned short, KPropertyValue >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bytePacketType)>>::put(w, m_bytePacketType);
        Codec<std::remove_cvref_t<decltype(m_byteLoginType)>>::put(w, m_byteLoginType);
        Codec<std::remove_cvref_t<decltype(m_wstrUserID)>>::put(w, m_wstrUserID);
        Codec<std::remove_cvref_t<decltype(m_uiIP)>>::put(w, m_uiIP);
        Codec<std::remove_cvref_t<decltype(m_mapProperty)>>::put(w, m_mapProperty);
    }

    static KENX_AUTH_LOGIN_REQ read_from(Reader& r) {
        KENX_AUTH_LOGIN_REQ v;
        v.m_bytePacketType = Codec<std::remove_cvref_t<decltype(v.m_bytePacketType)>>::get(r);
        v.m_byteLoginType = Codec<std::remove_cvref_t<decltype(v.m_byteLoginType)>>::get(r);
        v.m_wstrUserID = Codec<std::remove_cvref_t<decltype(v.m_wstrUserID)>>::get(r);
        v.m_uiIP = Codec<std::remove_cvref_t<decltype(v.m_uiIP)>>::get(r);
        v.m_mapProperty = Codec<std::remove_cvref_t<decltype(v.m_mapProperty)>>::get(r);
        return v;
    }
};

struct KPurchaseCoupon {
    std::uint32_t  m_uiNexonSN{};  // UINT32
    std::u16string m_wstrUserID{};  // std::wstring
    std::u16string m_wstrUserName{};  // std::wstring
    std::u16string m_wstrCouponSerial{};  // std::wstring
    std::int32_t   m_iGameType{};  // int
    std::int32_t   m_iServerDomain{};  // int
    std::u16string m_wstrUnitNickName{};  // std::wstring
    std::int32_t   m_iReason{};  // int
    std::u16string m_wstrCafeNo{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_uiNexonSN)>>::put(w, m_uiNexonSN);
        Codec<std::remove_cvref_t<decltype(m_wstrUserID)>>::put(w, m_wstrUserID);
        Codec<std::remove_cvref_t<decltype(m_wstrUserName)>>::put(w, m_wstrUserName);
        Codec<std::remove_cvref_t<decltype(m_wstrCouponSerial)>>::put(w, m_wstrCouponSerial);
        Codec<std::remove_cvref_t<decltype(m_iGameType)>>::put(w, m_iGameType);
        Codec<std::remove_cvref_t<decltype(m_iServerDomain)>>::put(w, m_iServerDomain);
        Codec<std::remove_cvref_t<decltype(m_wstrUnitNickName)>>::put(w, m_wstrUnitNickName);
        Codec<std::remove_cvref_t<decltype(m_iReason)>>::put(w, m_iReason);
        Codec<std::remove_cvref_t<decltype(m_wstrCafeNo)>>::put(w, m_wstrCafeNo);
    }

    static KPurchaseCoupon read_from(Reader& r) {
        KPurchaseCoupon v;
        v.m_uiNexonSN = Codec<std::remove_cvref_t<decltype(v.m_uiNexonSN)>>::get(r);
        v.m_wstrUserID = Codec<std::remove_cvref_t<decltype(v.m_wstrUserID)>>::get(r);
        v.m_wstrUserName = Codec<std::remove_cvref_t<decltype(v.m_wstrUserName)>>::get(r);
        v.m_wstrCouponSerial = Codec<std::remove_cvref_t<decltype(v.m_wstrCouponSerial)>>::get(r);
        v.m_iGameType = Codec<std::remove_cvref_t<decltype(v.m_iGameType)>>::get(r);
        v.m_iServerDomain = Codec<std::remove_cvref_t<decltype(v.m_iServerDomain)>>::get(r);
        v.m_wstrUnitNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrUnitNickName)>>::get(r);
        v.m_iReason = Codec<std::remove_cvref_t<decltype(v.m_iReason)>>::get(r);
        v.m_wstrCafeNo = Codec<std::remove_cvref_t<decltype(v.m_wstrCafeNo)>>::get(r);
        return v;
    }
};

struct KPurchaserInfo {
    std::int32_t   m_iServerGroupID{};  // int
    std::int64_t   m_iUserUID{};  // UidType
    std::u16string m_wstrUserName{};  // std::wstring
    std::int64_t   m_iUnitUID{};  // UidType
    std::u16string m_wstrUnitNickName{};  // std::wstring
    std::uint8_t   m_ucLevel{};  // UCHAR
    std::u16string m_wstrIP{};  // std::wstring
    std::uint32_t  m_uiPublisherUID{};  // unsigned int
    std::int32_t   m_iChannelingCode{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iServerGroupID)>>::put(w, m_iServerGroupID);
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
        Codec<std::remove_cvref_t<decltype(m_wstrUserName)>>::put(w, m_wstrUserName);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrUnitNickName)>>::put(w, m_wstrUnitNickName);
        Codec<std::remove_cvref_t<decltype(m_ucLevel)>>::put(w, m_ucLevel);
        Codec<std::remove_cvref_t<decltype(m_wstrIP)>>::put(w, m_wstrIP);
        Codec<std::remove_cvref_t<decltype(m_uiPublisherUID)>>::put(w, m_uiPublisherUID);
        Codec<std::remove_cvref_t<decltype(m_iChannelingCode)>>::put(w, m_iChannelingCode);
    }

    static KPurchaserInfo read_from(Reader& r) {
        KPurchaserInfo v;
        v.m_iServerGroupID = Codec<std::remove_cvref_t<decltype(v.m_iServerGroupID)>>::get(r);
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        v.m_wstrUserName = Codec<std::remove_cvref_t<decltype(v.m_wstrUserName)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_wstrUnitNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrUnitNickName)>>::get(r);
        v.m_ucLevel = Codec<std::remove_cvref_t<decltype(v.m_ucLevel)>>::get(r);
        v.m_wstrIP = Codec<std::remove_cvref_t<decltype(v.m_wstrIP)>>::get(r);
        v.m_uiPublisherUID = Codec<std::remove_cvref_t<decltype(v.m_uiPublisherUID)>>::get(r);
        v.m_iChannelingCode = Codec<std::remove_cvref_t<decltype(v.m_iChannelingCode)>>::get(r);
        return v;
    }
};

struct KEBILL_EXCHANGE_CASH_RESERVE_REQ {
    KPurchaserInfo m_PurchaserInfo{};
    std::int32_t   m_iPoint{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_PurchaserInfo)>>::put(w, m_PurchaserInfo);
        Codec<std::remove_cvref_t<decltype(m_iPoint)>>::put(w, m_iPoint);
    }

    static KEBILL_EXCHANGE_CASH_RESERVE_REQ read_from(Reader& r) {
        KEBILL_EXCHANGE_CASH_RESERVE_REQ v;
        v.m_PurchaserInfo = Codec<std::remove_cvref_t<decltype(v.m_PurchaserInfo)>>::get(r);
        v.m_iPoint = Codec<std::remove_cvref_t<decltype(v.m_iPoint)>>::get(r);
        return v;
    }
};

struct KEBILL_EXCHANGE_CASH_RESERVE_ACK {
    std::int32_t                     m_iOK{};  // int
    std::u16string                   m_wstrTransaction{};  // std::wstring
    KEBILL_EXCHANGE_CASH_RESERVE_REQ m_kPacketReq{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_wstrTransaction)>>::put(w, m_wstrTransaction);
        Codec<std::remove_cvref_t<decltype(m_kPacketReq)>>::put(w, m_kPacketReq);
    }

    static KEBILL_EXCHANGE_CASH_RESERVE_ACK read_from(Reader& r) {
        KEBILL_EXCHANGE_CASH_RESERVE_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_wstrTransaction = Codec<std::remove_cvref_t<decltype(v.m_wstrTransaction)>>::get(r);
        v.m_kPacketReq = Codec<std::remove_cvref_t<decltype(v.m_kPacketReq)>>::get(r);
        return v;
    }
};

struct KEBILL_USE_COUPON_REQ {
    KPurchaserInfo m_PurchaserInfo{};
    std::int32_t   m_iCouponType{};  // int
    std::u16string m_wstrSerialCode{};  // std::wstring
    std::u16string m_wstrTransaction{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_PurchaserInfo)>>::put(w, m_PurchaserInfo);
        Codec<std::remove_cvref_t<decltype(m_iCouponType)>>::put(w, m_iCouponType);
        Codec<std::remove_cvref_t<decltype(m_wstrSerialCode)>>::put(w, m_wstrSerialCode);
        Codec<std::remove_cvref_t<decltype(m_wstrTransaction)>>::put(w, m_wstrTransaction);
    }

    static KEBILL_USE_COUPON_REQ read_from(Reader& r) {
        KEBILL_USE_COUPON_REQ v;
        v.m_PurchaserInfo = Codec<std::remove_cvref_t<decltype(v.m_PurchaserInfo)>>::get(r);
        v.m_iCouponType = Codec<std::remove_cvref_t<decltype(v.m_iCouponType)>>::get(r);
        v.m_wstrSerialCode = Codec<std::remove_cvref_t<decltype(v.m_wstrSerialCode)>>::get(r);
        v.m_wstrTransaction = Codec<std::remove_cvref_t<decltype(v.m_wstrTransaction)>>::get(r);
        return v;
    }
};

struct KEBILL_REWARD_COUPON_JP_EVENT_REQ {
    KJPEventRewardInfo    m_kRewardInfo{};
    KEBILL_USE_COUPON_REQ m_kUseCouponPacket{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kRewardInfo)>>::put(w, m_kRewardInfo);
        Codec<std::remove_cvref_t<decltype(m_kUseCouponPacket)>>::put(w, m_kUseCouponPacket);
    }

    static KEBILL_REWARD_COUPON_JP_EVENT_REQ read_from(Reader& r) {
        KEBILL_REWARD_COUPON_JP_EVENT_REQ v;
        v.m_kRewardInfo = Codec<std::remove_cvref_t<decltype(v.m_kRewardInfo)>>::get(r);
        v.m_kUseCouponPacket = Codec<std::remove_cvref_t<decltype(v.m_kUseCouponPacket)>>::get(r);
        return v;
    }
};

struct KEBILL_USE_COUPON_RESERVE_ACK {
    std::int32_t          m_iOK{};  // int
    KEBILL_USE_COUPON_REQ m_kPacketReq{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_kPacketReq)>>::put(w, m_kPacketReq);
    }

    static KEBILL_USE_COUPON_RESERVE_ACK read_from(Reader& r) {
        KEBILL_USE_COUPON_RESERVE_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_kPacketReq = Codec<std::remove_cvref_t<decltype(v.m_kPacketReq)>>::get(r);
        return v;
    }
};

struct KELG_EXCHANGE_CASH_REQ {
    KPurchaserInfo m_PurchaserInfo{};
    std::int32_t   m_iPoint{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_PurchaserInfo)>>::put(w, m_PurchaserInfo);
        Codec<std::remove_cvref_t<decltype(m_iPoint)>>::put(w, m_iPoint);
    }

    static KELG_EXCHANGE_CASH_REQ read_from(Reader& r) {
        KELG_EXCHANGE_CASH_REQ v;
        v.m_PurchaserInfo = Codec<std::remove_cvref_t<decltype(v.m_PurchaserInfo)>>::get(r);
        v.m_iPoint = Codec<std::remove_cvref_t<decltype(v.m_iPoint)>>::get(r);
        return v;
    }
};

struct KPvpMatchUserResultLog {
    std::int64_t   m_iUnitUID{};  // UidType
    std::u16string m_wstrNickName{};  // std::wstring
    std::uint8_t   m_ucLevel{};  // u_char
    char           m_cUnitClass{};
    std::int32_t   m_iRating{};  // int
    char           m_cPvpRank{};
    std::int32_t   m_iRPoint{};  // int
    std::int32_t   m_iAPoint{};  // int
    std::int32_t   m_iOfficialMatchCnt{};  // int
    char           m_cTeam{};
    bool           m_bPartyApp{};
    std::int32_t   m_iWaitTime{};  // int
    std::int32_t   m_iWin{};  // int
    std::int32_t   m_iKill{};  // int
    std::int32_t   m_iAssist{};  // int
    std::int32_t   m_iDeath{};  // int
    std::int32_t   m_iNetRatingInc{};  // int
    std::int32_t   m_iNetRPInc{};  // int
    std::int32_t   m_iNetAPInc{};  // int
    std::int32_t   m_iEXPearned{};  // int
    char           m_cEndType{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_ucLevel)>>::put(w, m_ucLevel);
        Codec<std::remove_cvref_t<decltype(m_cUnitClass)>>::put(w, m_cUnitClass);
        Codec<std::remove_cvref_t<decltype(m_iRating)>>::put(w, m_iRating);
        Codec<std::remove_cvref_t<decltype(m_cPvpRank)>>::put(w, m_cPvpRank);
        Codec<std::remove_cvref_t<decltype(m_iRPoint)>>::put(w, m_iRPoint);
        Codec<std::remove_cvref_t<decltype(m_iAPoint)>>::put(w, m_iAPoint);
        Codec<std::remove_cvref_t<decltype(m_iOfficialMatchCnt)>>::put(w, m_iOfficialMatchCnt);
        Codec<std::remove_cvref_t<decltype(m_cTeam)>>::put(w, m_cTeam);
        Codec<std::remove_cvref_t<decltype(m_bPartyApp)>>::put(w, m_bPartyApp);
        Codec<std::remove_cvref_t<decltype(m_iWaitTime)>>::put(w, m_iWaitTime);
        Codec<std::remove_cvref_t<decltype(m_iWin)>>::put(w, m_iWin);
        Codec<std::remove_cvref_t<decltype(m_iKill)>>::put(w, m_iKill);
        Codec<std::remove_cvref_t<decltype(m_iAssist)>>::put(w, m_iAssist);
        Codec<std::remove_cvref_t<decltype(m_iDeath)>>::put(w, m_iDeath);
        Codec<std::remove_cvref_t<decltype(m_iNetRatingInc)>>::put(w, m_iNetRatingInc);
        Codec<std::remove_cvref_t<decltype(m_iNetRPInc)>>::put(w, m_iNetRPInc);
        Codec<std::remove_cvref_t<decltype(m_iNetAPInc)>>::put(w, m_iNetAPInc);
        Codec<std::remove_cvref_t<decltype(m_iEXPearned)>>::put(w, m_iEXPearned);
        Codec<std::remove_cvref_t<decltype(m_cEndType)>>::put(w, m_cEndType);
    }

    static KPvpMatchUserResultLog read_from(Reader& r) {
        KPvpMatchUserResultLog v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_ucLevel = Codec<std::remove_cvref_t<decltype(v.m_ucLevel)>>::get(r);
        v.m_cUnitClass = Codec<std::remove_cvref_t<decltype(v.m_cUnitClass)>>::get(r);
        v.m_iRating = Codec<std::remove_cvref_t<decltype(v.m_iRating)>>::get(r);
        v.m_cPvpRank = Codec<std::remove_cvref_t<decltype(v.m_cPvpRank)>>::get(r);
        v.m_iRPoint = Codec<std::remove_cvref_t<decltype(v.m_iRPoint)>>::get(r);
        v.m_iAPoint = Codec<std::remove_cvref_t<decltype(v.m_iAPoint)>>::get(r);
        v.m_iOfficialMatchCnt = Codec<std::remove_cvref_t<decltype(v.m_iOfficialMatchCnt)>>::get(r);
        v.m_cTeam = Codec<std::remove_cvref_t<decltype(v.m_cTeam)>>::get(r);
        v.m_bPartyApp = Codec<std::remove_cvref_t<decltype(v.m_bPartyApp)>>::get(r);
        v.m_iWaitTime = Codec<std::remove_cvref_t<decltype(v.m_iWaitTime)>>::get(r);
        v.m_iWin = Codec<std::remove_cvref_t<decltype(v.m_iWin)>>::get(r);
        v.m_iKill = Codec<std::remove_cvref_t<decltype(v.m_iKill)>>::get(r);
        v.m_iAssist = Codec<std::remove_cvref_t<decltype(v.m_iAssist)>>::get(r);
        v.m_iDeath = Codec<std::remove_cvref_t<decltype(v.m_iDeath)>>::get(r);
        v.m_iNetRatingInc = Codec<std::remove_cvref_t<decltype(v.m_iNetRatingInc)>>::get(r);
        v.m_iNetRPInc = Codec<std::remove_cvref_t<decltype(v.m_iNetRPInc)>>::get(r);
        v.m_iNetAPInc = Codec<std::remove_cvref_t<decltype(v.m_iNetAPInc)>>::get(r);
        v.m_iEXPearned = Codec<std::remove_cvref_t<decltype(v.m_iEXPearned)>>::get(r);
        v.m_cEndType = Codec<std::remove_cvref_t<decltype(v.m_cEndType)>>::get(r);
        return v;
    }
};

struct KDBE_PVP_MATCH_RESULT_LOG_NOT {
    char                                m_cMatchType{};
    char                                m_cNpcPvpType{};
    std::u16string                      m_wstrRegDate{};  // std::wstring
    std::int32_t                        m_iPlayTime{};  // int
    std::vector<KPvpMatchUserResultLog> m_vecPvpMatchUserLog{};  // std::vector< KPvpMatchUserResultLog >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cMatchType)>>::put(w, m_cMatchType);
        Codec<std::remove_cvref_t<decltype(m_cNpcPvpType)>>::put(w, m_cNpcPvpType);
        Codec<std::remove_cvref_t<decltype(m_wstrRegDate)>>::put(w, m_wstrRegDate);
        Codec<std::remove_cvref_t<decltype(m_iPlayTime)>>::put(w, m_iPlayTime);
        Codec<std::remove_cvref_t<decltype(m_vecPvpMatchUserLog)>>::put(w, m_vecPvpMatchUserLog);
    }

    static KDBE_PVP_MATCH_RESULT_LOG_NOT read_from(Reader& r) {
        KDBE_PVP_MATCH_RESULT_LOG_NOT v;
        v.m_cMatchType = Codec<std::remove_cvref_t<decltype(v.m_cMatchType)>>::get(r);
        v.m_cNpcPvpType = Codec<std::remove_cvref_t<decltype(v.m_cNpcPvpType)>>::get(r);
        v.m_wstrRegDate = Codec<std::remove_cvref_t<decltype(v.m_wstrRegDate)>>::get(r);
        v.m_iPlayTime = Codec<std::remove_cvref_t<decltype(v.m_iPlayTime)>>::get(r);
        v.m_vecPvpMatchUserLog = Codec<std::remove_cvref_t<decltype(v.m_vecPvpMatchUserLog)>>::get(r);
        return v;
    }
};

struct KPvpNpcInfo {
    std::int32_t m_iNpcID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iNpcID)>>::put(w, m_iNpcID);
    }

    static KPvpNpcInfo read_from(Reader& r) {
        KPvpNpcInfo v;
        v.m_iNpcID = Codec<std::remove_cvref_t<decltype(v.m_iNpcID)>>::get(r);
        return v;
    }
};

struct KPvpPlayResultInfo {
    std::int32_t m_iRPoint{};  // int
    std::int32_t m_iAPoint{};  // int
    std::int32_t m_iWinCount{};  // int
    bool         m_bIsPlayWithParty{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iRPoint)>>::put(w, m_iRPoint);
        Codec<std::remove_cvref_t<decltype(m_iAPoint)>>::put(w, m_iAPoint);
        Codec<std::remove_cvref_t<decltype(m_iWinCount)>>::put(w, m_iWinCount);
        Codec<std::remove_cvref_t<decltype(m_bIsPlayWithParty)>>::put(w, m_bIsPlayWithParty);
    }

    static KPvpPlayResultInfo read_from(Reader& r) {
        KPvpPlayResultInfo v;
        v.m_iRPoint = Codec<std::remove_cvref_t<decltype(v.m_iRPoint)>>::get(r);
        v.m_iAPoint = Codec<std::remove_cvref_t<decltype(v.m_iAPoint)>>::get(r);
        v.m_iWinCount = Codec<std::remove_cvref_t<decltype(v.m_iWinCount)>>::get(r);
        v.m_bIsPlayWithParty = Codec<std::remove_cvref_t<decltype(v.m_bIsPlayWithParty)>>::get(r);
        return v;
    }
};

struct KERM_UPDATE_PVP_PLAY_UNIT_INFO_NOT {
    KPvpPlayResultInfo m_kPvpUnitInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kPvpUnitInfo)>>::put(w, m_kPvpUnitInfo);
    }

    static KERM_UPDATE_PVP_PLAY_UNIT_INFO_NOT read_from(Reader& r) {
        KERM_UPDATE_PVP_PLAY_UNIT_INFO_NOT v;
        v.m_kPvpUnitInfo = Codec<std::remove_cvref_t<decltype(v.m_kPvpUnitInfo)>>::get(r);
        return v;
    }
};

struct KPvpRankingInfo {
    std::int32_t   m_iRank{};  // int
    std::int64_t   m_iUnitUID{};  // UidType
    std::u16string m_wstrNickName{};  // std::wstring
    char           m_cUnitClass{};
    std::uint8_t   m_ucLevel{};  // UCHAR
    char           m_cRank{};
    std::int32_t   m_iRPoint{};  // int
    std::int32_t   m_iWin{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iRank)>>::put(w, m_iRank);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_cUnitClass)>>::put(w, m_cUnitClass);
        Codec<std::remove_cvref_t<decltype(m_ucLevel)>>::put(w, m_ucLevel);
        Codec<std::remove_cvref_t<decltype(m_cRank)>>::put(w, m_cRank);
        Codec<std::remove_cvref_t<decltype(m_iRPoint)>>::put(w, m_iRPoint);
        Codec<std::remove_cvref_t<decltype(m_iWin)>>::put(w, m_iWin);
    }

    static KPvpRankingInfo read_from(Reader& r) {
        KPvpRankingInfo v;
        v.m_iRank = Codec<std::remove_cvref_t<decltype(v.m_iRank)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_cUnitClass = Codec<std::remove_cvref_t<decltype(v.m_cUnitClass)>>::get(r);
        v.m_ucLevel = Codec<std::remove_cvref_t<decltype(v.m_ucLevel)>>::get(r);
        v.m_cRank = Codec<std::remove_cvref_t<decltype(v.m_cRank)>>::get(r);
        v.m_iRPoint = Codec<std::remove_cvref_t<decltype(v.m_iRPoint)>>::get(r);
        v.m_iWin = Codec<std::remove_cvref_t<decltype(v.m_iWin)>>::get(r);
        return v;
    }
};

struct KDBE_GET_WEB_RANKING_INFO_ACK {
    std::vector<KDungeonRankingInfo> m_vecDungeonRanking{};  // std::vector< KDungeonRankingInfo >
    std::vector<KPvpRankingInfo>     m_vecPvpRanking{};  // std::vector< KPvpRankingInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecDungeonRanking)>>::put(w, m_vecDungeonRanking);
        Codec<std::remove_cvref_t<decltype(m_vecPvpRanking)>>::put(w, m_vecPvpRanking);
    }

    static KDBE_GET_WEB_RANKING_INFO_ACK read_from(Reader& r) {
        KDBE_GET_WEB_RANKING_INFO_ACK v;
        v.m_vecDungeonRanking = Codec<std::remove_cvref_t<decltype(v.m_vecDungeonRanking)>>::get(r);
        v.m_vecPvpRanking = Codec<std::remove_cvref_t<decltype(v.m_vecPvpRanking)>>::get(r);
        return v;
    }
};

struct KEGS_GET_RANKING_INFO_ACK {
    std::int32_t                     m_iRankingType{};  // int
    std::uint32_t                    m_uiViewPage{};  // UINT
    std::uint32_t                    m_uiTotalPage{};  // UINT
    std::vector<KHenirRankingInfo>   m_vecHenirRankingPage{};  // std::vector< KHenirRankingInfo >
    std::vector<KDungeonRankingInfo> m_vecDungeonRankingPage{};  // std::vector< KDungeonRankingInfo >
    std::vector<KPvpRankingInfo>     m_vecPvpRankingPage{};  // std::vector< KPvpRankingInfo >
    std::vector<KHenirRankingInfo>   m_vecMyHenirRanking{};  // std::vector< KHenirRankingInfo >
    std::vector<KDungeonRankingInfo> m_vecMyDungeonRanking{};  // std::vector< KDungeonRankingInfo >
    std::vector<KPvpRankingInfo>     m_vecMyPvpRanking{};  // std::vector< KPvpRankingInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iRankingType)>>::put(w, m_iRankingType);
        Codec<std::remove_cvref_t<decltype(m_uiViewPage)>>::put(w, m_uiViewPage);
        Codec<std::remove_cvref_t<decltype(m_uiTotalPage)>>::put(w, m_uiTotalPage);
        Codec<std::remove_cvref_t<decltype(m_vecHenirRankingPage)>>::put(w, m_vecHenirRankingPage);
        Codec<std::remove_cvref_t<decltype(m_vecDungeonRankingPage)>>::put(w, m_vecDungeonRankingPage);
        Codec<std::remove_cvref_t<decltype(m_vecPvpRankingPage)>>::put(w, m_vecPvpRankingPage);
        Codec<std::remove_cvref_t<decltype(m_vecMyHenirRanking)>>::put(w, m_vecMyHenirRanking);
        Codec<std::remove_cvref_t<decltype(m_vecMyDungeonRanking)>>::put(w, m_vecMyDungeonRanking);
        Codec<std::remove_cvref_t<decltype(m_vecMyPvpRanking)>>::put(w, m_vecMyPvpRanking);
    }

    static KEGS_GET_RANKING_INFO_ACK read_from(Reader& r) {
        KEGS_GET_RANKING_INFO_ACK v;
        v.m_iRankingType = Codec<std::remove_cvref_t<decltype(v.m_iRankingType)>>::get(r);
        v.m_uiViewPage = Codec<std::remove_cvref_t<decltype(v.m_uiViewPage)>>::get(r);
        v.m_uiTotalPage = Codec<std::remove_cvref_t<decltype(v.m_uiTotalPage)>>::get(r);
        v.m_vecHenirRankingPage = Codec<std::remove_cvref_t<decltype(v.m_vecHenirRankingPage)>>::get(r);
        v.m_vecDungeonRankingPage = Codec<std::remove_cvref_t<decltype(v.m_vecDungeonRankingPage)>>::get(r);
        v.m_vecPvpRankingPage = Codec<std::remove_cvref_t<decltype(v.m_vecPvpRankingPage)>>::get(r);
        v.m_vecMyHenirRanking = Codec<std::remove_cvref_t<decltype(v.m_vecMyHenirRanking)>>::get(r);
        v.m_vecMyDungeonRanking = Codec<std::remove_cvref_t<decltype(v.m_vecMyDungeonRanking)>>::get(r);
        v.m_vecMyPvpRanking = Codec<std::remove_cvref_t<decltype(v.m_vecMyPvpRanking)>>::get(r);
        return v;
    }
};

struct KELG_WEB_RANKING_REFRESH_NOT {
    bool                                        m_bInit{};
    char                                        m_cRankingRefreshType{};
    std::uint32_t                               m_uiTotalSize{};  // u_int
    std::vector<KDungeonRankingInfo>            m_vecDungeonRanking{};  // std::vector< KDungeonRankingInfo >
    std::map<std::int64_t, KDungeonRankingInfo> m_mapDungeonRanking{};  // std::map< UidType, KDungeonRankingInfo >
    std::vector<KPvpRankingInfo>                m_vecPvpRanking{};  // std::vector< KPvpRankingInfo >
    std::map<std::int64_t, KPvpRankingInfo>     m_mapPvpRanking{};  // std::map< UidType, KPvpRankingInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bInit)>>::put(w, m_bInit);
        Codec<std::remove_cvref_t<decltype(m_cRankingRefreshType)>>::put(w, m_cRankingRefreshType);
        Codec<std::remove_cvref_t<decltype(m_uiTotalSize)>>::put(w, m_uiTotalSize);
        Codec<std::remove_cvref_t<decltype(m_vecDungeonRanking)>>::put(w, m_vecDungeonRanking);
        Codec<std::remove_cvref_t<decltype(m_mapDungeonRanking)>>::put(w, m_mapDungeonRanking);
        Codec<std::remove_cvref_t<decltype(m_vecPvpRanking)>>::put(w, m_vecPvpRanking);
        Codec<std::remove_cvref_t<decltype(m_mapPvpRanking)>>::put(w, m_mapPvpRanking);
    }

    static KELG_WEB_RANKING_REFRESH_NOT read_from(Reader& r) {
        KELG_WEB_RANKING_REFRESH_NOT v;
        v.m_bInit = Codec<std::remove_cvref_t<decltype(v.m_bInit)>>::get(r);
        v.m_cRankingRefreshType = Codec<std::remove_cvref_t<decltype(v.m_cRankingRefreshType)>>::get(r);
        v.m_uiTotalSize = Codec<std::remove_cvref_t<decltype(v.m_uiTotalSize)>>::get(r);
        v.m_vecDungeonRanking = Codec<std::remove_cvref_t<decltype(v.m_vecDungeonRanking)>>::get(r);
        v.m_mapDungeonRanking = Codec<std::remove_cvref_t<decltype(v.m_mapDungeonRanking)>>::get(r);
        v.m_vecPvpRanking = Codec<std::remove_cvref_t<decltype(v.m_vecPvpRanking)>>::get(r);
        v.m_mapPvpRanking = Codec<std::remove_cvref_t<decltype(v.m_mapPvpRanking)>>::get(r);
        return v;
    }
};

struct KQuestAck {
    std::int32_t                         m_iOK{};  // int
    std::int32_t                         m_iQuestID{};  // int
    std::map<std::int32_t, std::int32_t> m_mapInsertedItem{};  // std::map< int, int >
    KItemQuantityUpdate                  m_kItemQuantityUpdate{};
    std::vector<KInventoryItemInfo>      m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >
    std::map<std::int64_t, KItemInfo>    m_mapItemInfo{};  // std::map< UidType, KItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iQuestID)>>::put(w, m_iQuestID);
        Codec<std::remove_cvref_t<decltype(m_mapInsertedItem)>>::put(w, m_mapInsertedItem);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
        Codec<std::remove_cvref_t<decltype(m_mapItemInfo)>>::put(w, m_mapItemInfo);
    }

    static KQuestAck read_from(Reader& r) {
        KQuestAck v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iQuestID = Codec<std::remove_cvref_t<decltype(v.m_iQuestID)>>::get(r);
        v.m_mapInsertedItem = Codec<std::remove_cvref_t<decltype(v.m_mapInsertedItem)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        v.m_mapItemInfo = Codec<std::remove_cvref_t<decltype(v.m_mapItemInfo)>>::get(r);
        return v;
    }
};

struct KQuestReq {
    std::int64_t m_UnitUID{};  // UidType
    std::int32_t m_iQuestID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_UnitUID)>>::put(w, m_UnitUID);
        Codec<std::remove_cvref_t<decltype(m_iQuestID)>>::put(w, m_iQuestID);
    }

    static KQuestReq read_from(Reader& r) {
        KQuestReq v;
        v.m_UnitUID = Codec<std::remove_cvref_t<decltype(v.m_UnitUID)>>::get(r);
        v.m_iQuestID = Codec<std::remove_cvref_t<decltype(v.m_iQuestID)>>::get(r);
        return v;
    }
};

struct KQuestUpdate {
    std::int32_t              m_iQuestID{};  // int
    std::vector<std::uint8_t> m_vecClearData{};  // std::vector<UCHAR>

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iQuestID)>>::put(w, m_iQuestID);
        Codec<std::remove_cvref_t<decltype(m_vecClearData)>>::put(w, m_vecClearData);
    }

    static KQuestUpdate read_from(Reader& r) {
        KQuestUpdate v;
        v.m_iQuestID = Codec<std::remove_cvref_t<decltype(v.m_iQuestID)>>::get(r);
        v.m_vecClearData = Codec<std::remove_cvref_t<decltype(v.m_vecClearData)>>::get(r);
        return v;
    }
};

struct KDBE_ACCOUNT_QUEST_UPDATE_NOT {
    std::int64_t              m_iUserUID{};  // UidType
    std::vector<KQuestUpdate> m_vecAccountQuesting{};  // std::vector< KQuestUpdate >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
        Codec<std::remove_cvref_t<decltype(m_vecAccountQuesting)>>::put(w, m_vecAccountQuesting);
    }

    static KDBE_ACCOUNT_QUEST_UPDATE_NOT read_from(Reader& r) {
        KDBE_ACCOUNT_QUEST_UPDATE_NOT v;
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        v.m_vecAccountQuesting = Codec<std::remove_cvref_t<decltype(v.m_vecAccountQuesting)>>::get(r);
        return v;
    }
};

struct KRankerInfo {
    std::int64_t   m_iUnitUID{};  // UidType
    std::u16string m_wstrNickName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
    }

    static KRankerInfo read_from(Reader& r) {
        KRankerInfo v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        return v;
    }
};

struct KRecommendUserInfo {
    std::int64_t   m_iUnitUID{};  // UidType
    std::u16string m_wstrNickName{};  // std::wstring
    std::uint8_t   m_ucLevel{};  // u_char
    std::u16string m_wstrRecommendDate{};  // std::wstring
    bool           m_bIsOnline{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_ucLevel)>>::put(w, m_ucLevel);
        Codec<std::remove_cvref_t<decltype(m_wstrRecommendDate)>>::put(w, m_wstrRecommendDate);
        Codec<std::remove_cvref_t<decltype(m_bIsOnline)>>::put(w, m_bIsOnline);
    }

    static KRecommendUserInfo read_from(Reader& r) {
        KRecommendUserInfo v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_ucLevel = Codec<std::remove_cvref_t<decltype(v.m_ucLevel)>>::get(r);
        v.m_wstrRecommendDate = Codec<std::remove_cvref_t<decltype(v.m_wstrRecommendDate)>>::get(r);
        v.m_bIsOnline = Codec<std::remove_cvref_t<decltype(v.m_bIsOnline)>>::get(r);
        return v;
    }
};

struct KDBE_GET_RECOMMEND_USER_LIST_ACK {
    std::int32_t                    m_iOK{};  // int
    std::vector<KRecommendUserInfo> m_vecRecommendUserList{};  // std::vector< KRecommendUserInfo >
    std::uint32_t                   m_uiViewPage{};  // u_int
    std::uint32_t                   m_uiTotalPage{};  // u_int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_vecRecommendUserList)>>::put(w, m_vecRecommendUserList);
        Codec<std::remove_cvref_t<decltype(m_uiViewPage)>>::put(w, m_uiViewPage);
        Codec<std::remove_cvref_t<decltype(m_uiTotalPage)>>::put(w, m_uiTotalPage);
    }

    static KDBE_GET_RECOMMEND_USER_LIST_ACK read_from(Reader& r) {
        KDBE_GET_RECOMMEND_USER_LIST_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_vecRecommendUserList = Codec<std::remove_cvref_t<decltype(v.m_vecRecommendUserList)>>::get(r);
        v.m_uiViewPage = Codec<std::remove_cvref_t<decltype(v.m_uiViewPage)>>::get(r);
        v.m_uiTotalPage = Codec<std::remove_cvref_t<decltype(v.m_uiTotalPage)>>::get(r);
        return v;
    }
};

struct KEGS_GET_RECOMMEND_USER_LIST_ACK {
    std::int32_t                    m_iOK{};  // int
    std::uint32_t                   m_uiTotalPage{};  // u_int
    std::uint32_t                   m_uiViewPage{};  // u_int
    std::vector<KRecommendUserInfo> m_vecRecommendUserList{};  // std::vector< KRecommendUserInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_uiTotalPage)>>::put(w, m_uiTotalPage);
        Codec<std::remove_cvref_t<decltype(m_uiViewPage)>>::put(w, m_uiViewPage);
        Codec<std::remove_cvref_t<decltype(m_vecRecommendUserList)>>::put(w, m_vecRecommendUserList);
    }

    static KEGS_GET_RECOMMEND_USER_LIST_ACK read_from(Reader& r) {
        KEGS_GET_RECOMMEND_USER_LIST_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_uiTotalPage = Codec<std::remove_cvref_t<decltype(v.m_uiTotalPage)>>::get(r);
        v.m_uiViewPage = Codec<std::remove_cvref_t<decltype(v.m_uiViewPage)>>::get(r);
        v.m_vecRecommendUserList = Codec<std::remove_cvref_t<decltype(v.m_vecRecommendUserList)>>::get(r);
        return v;
    }
};

struct KRecordBuffInfo {
    std::int32_t   m_iBuffID{};  // int
    std::u16string m_wstrStartTime{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iBuffID)>>::put(w, m_iBuffID);
        Codec<std::remove_cvref_t<decltype(m_wstrStartTime)>>::put(w, m_wstrStartTime);
    }

    static KRecordBuffInfo read_from(Reader& r) {
        KRecordBuffInfo v;
        v.m_iBuffID = Codec<std::remove_cvref_t<decltype(v.m_iBuffID)>>::get(r);
        v.m_wstrStartTime = Codec<std::remove_cvref_t<decltype(v.m_wstrStartTime)>>::get(r);
        return v;
    }
};

struct KRegPersonalShopItemInfo {
    std::int64_t m_iItemUID{};  // UidType
    std::int32_t m_iQuantity{};  // int
    std::int32_t m_iPricePerOne{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_iQuantity)>>::put(w, m_iQuantity);
        Codec<std::remove_cvref_t<decltype(m_iPricePerOne)>>::put(w, m_iPricePerOne);
    }

    static KRegPersonalShopItemInfo read_from(Reader& r) {
        KRegPersonalShopItemInfo v;
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_iQuantity = Codec<std::remove_cvref_t<decltype(v.m_iQuantity)>>::get(r);
        v.m_iPricePerOne = Codec<std::remove_cvref_t<decltype(v.m_iPricePerOne)>>::get(r);
        return v;
    }
};

struct KEGS_REG_PERSONAL_SHOP_ITEM_REQ {
    std::u16string                        m_wstrPersonalShopName{};  // std::wstring
    std::vector<KRegPersonalShopItemInfo> m_vecRegItem{};  // std::vector< KRegPersonalShopItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrPersonalShopName)>>::put(w, m_wstrPersonalShopName);
        Codec<std::remove_cvref_t<decltype(m_vecRegItem)>>::put(w, m_vecRegItem);
    }

    static KEGS_REG_PERSONAL_SHOP_ITEM_REQ read_from(Reader& r) {
        KEGS_REG_PERSONAL_SHOP_ITEM_REQ v;
        v.m_wstrPersonalShopName = Codec<std::remove_cvref_t<decltype(v.m_wstrPersonalShopName)>>::get(r);
        v.m_vecRegItem = Codec<std::remove_cvref_t<decltype(v.m_vecRegItem)>>::get(r);
        return v;
    }
};

struct KRegUnitInfo {
    std::int64_t   m_iUserUID{};  // UidType
    std::int64_t   m_iUnitUID{};  // UidType
    std::u16string m_wstrUnitNickName{};  // std::wstring
    std::int64_t   m_iGSUID{};  // UidType
    std::int32_t   m_iChannelID{};  // int
    std::u16string m_wstrIP{};  // std::wstring
    char           m_cAuthLevel{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrUnitNickName)>>::put(w, m_wstrUnitNickName);
        Codec<std::remove_cvref_t<decltype(m_iGSUID)>>::put(w, m_iGSUID);
        Codec<std::remove_cvref_t<decltype(m_iChannelID)>>::put(w, m_iChannelID);
        Codec<std::remove_cvref_t<decltype(m_wstrIP)>>::put(w, m_wstrIP);
        Codec<std::remove_cvref_t<decltype(m_cAuthLevel)>>::put(w, m_cAuthLevel);
    }

    static KRegUnitInfo read_from(Reader& r) {
        KRegUnitInfo v;
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_wstrUnitNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrUnitNickName)>>::get(r);
        v.m_iGSUID = Codec<std::remove_cvref_t<decltype(v.m_iGSUID)>>::get(r);
        v.m_iChannelID = Codec<std::remove_cvref_t<decltype(v.m_iChannelID)>>::get(r);
        v.m_wstrIP = Codec<std::remove_cvref_t<decltype(v.m_wstrIP)>>::get(r);
        v.m_cAuthLevel = Codec<std::remove_cvref_t<decltype(v.m_cAuthLevel)>>::get(r);
        return v;
    }
};

struct KEGB_REGISTER_UNIT_NOT {
    KRegUnitInfo m_kUnitInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kUnitInfo)>>::put(w, m_kUnitInfo);
    }

    static KEGB_REGISTER_UNIT_NOT read_from(Reader& r) {
        KEGB_REGISTER_UNIT_NOT v;
        v.m_kUnitInfo = Codec<std::remove_cvref_t<decltype(v.m_kUnitInfo)>>::get(r);
        return v;
    }
};

struct KRelationshipInfo {
    char           m_cRelationshipType{};
    std::int64_t   m_iOtherUnitUID{};  // UidType
    std::u16string m_wstrOtherNickName{};  // std::wstring
    std::u16string m_wstrLoveWord{};  // std::wstring
    std::int64_t   m_tDate{};  // __int64
    std::int64_t   m_tLastReward{};  // __int64
    char           m_cRewardTitleType{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cRelationshipType)>>::put(w, m_cRelationshipType);
        Codec<std::remove_cvref_t<decltype(m_iOtherUnitUID)>>::put(w, m_iOtherUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrOtherNickName)>>::put(w, m_wstrOtherNickName);
        Codec<std::remove_cvref_t<decltype(m_wstrLoveWord)>>::put(w, m_wstrLoveWord);
        Codec<std::remove_cvref_t<decltype(m_tDate)>>::put(w, m_tDate);
        Codec<std::remove_cvref_t<decltype(m_tLastReward)>>::put(w, m_tLastReward);
        Codec<std::remove_cvref_t<decltype(m_cRewardTitleType)>>::put(w, m_cRewardTitleType);
    }

    static KRelationshipInfo read_from(Reader& r) {
        KRelationshipInfo v;
        v.m_cRelationshipType = Codec<std::remove_cvref_t<decltype(v.m_cRelationshipType)>>::get(r);
        v.m_iOtherUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iOtherUnitUID)>>::get(r);
        v.m_wstrOtherNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrOtherNickName)>>::get(r);
        v.m_wstrLoveWord = Codec<std::remove_cvref_t<decltype(v.m_wstrLoveWord)>>::get(r);
        v.m_tDate = Codec<std::remove_cvref_t<decltype(v.m_tDate)>>::get(r);
        v.m_tLastReward = Codec<std::remove_cvref_t<decltype(v.m_tLastReward)>>::get(r);
        v.m_cRewardTitleType = Codec<std::remove_cvref_t<decltype(v.m_cRewardTitleType)>>::get(r);
        return v;
    }
};

struct KReturnToFieldInfo {
    std::int64_t           m_iReturnToFieldUnitUID{};  // UidType
    std::int64_t           m_iRoomUID{};  // UidType
    std::set<std::int64_t> m_setRemainUnitUID{};  // std::set< UidType >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iReturnToFieldUnitUID)>>::put(w, m_iReturnToFieldUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iRoomUID)>>::put(w, m_iRoomUID);
        Codec<std::remove_cvref_t<decltype(m_setRemainUnitUID)>>::put(w, m_setRemainUnitUID);
    }

    static KReturnToFieldInfo read_from(Reader& r) {
        KReturnToFieldInfo v;
        v.m_iReturnToFieldUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iReturnToFieldUnitUID)>>::get(r);
        v.m_iRoomUID = Codec<std::remove_cvref_t<decltype(v.m_iRoomUID)>>::get(r);
        v.m_setRemainUnitUID = Codec<std::remove_cvref_t<decltype(v.m_setRemainUnitUID)>>::get(r);
        return v;
    }
};

struct KERM_COMPLETE_RETURN_TO_FIELD_NOT {
    KReturnToFieldInfo m_kReturnToFieldInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kReturnToFieldInfo)>>::put(w, m_kReturnToFieldInfo);
    }

    static KERM_COMPLETE_RETURN_TO_FIELD_NOT read_from(Reader& r) {
        KERM_COMPLETE_RETURN_TO_FIELD_NOT v;
        v.m_kReturnToFieldInfo = Codec<std::remove_cvref_t<decltype(v.m_kReturnToFieldInfo)>>::get(r);
        return v;
    }
};

struct KRidingPetInfo {
    std::int64_t   m_iRidingPetUID{};  // UidType
    std::uint16_t  m_usRindingPetID{};  // USHORT
    float          m_fStamina{};
    std::u16string m_wstrDestroyDate{};  // std::wstring
    std::int64_t   m_iLastUnSummonDate{};  // __int64

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iRidingPetUID)>>::put(w, m_iRidingPetUID);
        Codec<std::remove_cvref_t<decltype(m_usRindingPetID)>>::put(w, m_usRindingPetID);
        Codec<std::remove_cvref_t<decltype(m_fStamina)>>::put(w, m_fStamina);
        Codec<std::remove_cvref_t<decltype(m_wstrDestroyDate)>>::put(w, m_wstrDestroyDate);
        Codec<std::remove_cvref_t<decltype(m_iLastUnSummonDate)>>::put(w, m_iLastUnSummonDate);
    }

    static KRidingPetInfo read_from(Reader& r) {
        KRidingPetInfo v;
        v.m_iRidingPetUID = Codec<std::remove_cvref_t<decltype(v.m_iRidingPetUID)>>::get(r);
        v.m_usRindingPetID = Codec<std::remove_cvref_t<decltype(v.m_usRindingPetID)>>::get(r);
        v.m_fStamina = Codec<std::remove_cvref_t<decltype(v.m_fStamina)>>::get(r);
        v.m_wstrDestroyDate = Codec<std::remove_cvref_t<decltype(v.m_wstrDestroyDate)>>::get(r);
        v.m_iLastUnSummonDate = Codec<std::remove_cvref_t<decltype(v.m_iLastUnSummonDate)>>::get(r);
        return v;
    }
};

struct KDBE_CREATE_RIDING_PET_ACK {
    std::int32_t                    m_iOK{};  // int
    KRidingPetInfo                  m_kCreatedPetInfo{};
    KItemQuantityUpdate             m_kItemQuantityUpdate{};
    std::vector<KInventoryItemInfo> m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_kCreatedPetInfo)>>::put(w, m_kCreatedPetInfo);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
    }

    static KDBE_CREATE_RIDING_PET_ACK read_from(Reader& r) {
        KDBE_CREATE_RIDING_PET_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_kCreatedPetInfo = Codec<std::remove_cvref_t<decltype(v.m_kCreatedPetInfo)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        return v;
    }
};

struct KDBE_GET_RIDING_PET_LIST_ACK {
    std::int32_t                m_iOK{};  // int
    std::int32_t                m_iAfterWorkStorageKey{};  // int
    std::vector<KRidingPetInfo> m_vecRidingPetList{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iAfterWorkStorageKey)>>::put(w, m_iAfterWorkStorageKey);
        Codec<std::remove_cvref_t<decltype(m_vecRidingPetList)>>::put(w, m_vecRidingPetList);
    }

    static KDBE_GET_RIDING_PET_LIST_ACK read_from(Reader& r) {
        KDBE_GET_RIDING_PET_LIST_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iAfterWorkStorageKey = Codec<std::remove_cvref_t<decltype(v.m_iAfterWorkStorageKey)>>::get(r);
        v.m_vecRidingPetList = Codec<std::remove_cvref_t<decltype(v.m_vecRidingPetList)>>::get(r);
        return v;
    }
};

struct KEGS_CREATE_RIDING_PET_ACK {
    std::int32_t                    m_iOK{};  // int
    KRidingPetInfo                  m_kCreatedPetInfo{};
    std::vector<KInventoryItemInfo> m_vecKInventorySlotInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_kCreatedPetInfo)>>::put(w, m_kCreatedPetInfo);
        Codec<std::remove_cvref_t<decltype(m_vecKInventorySlotInfo)>>::put(w, m_vecKInventorySlotInfo);
    }

    static KEGS_CREATE_RIDING_PET_ACK read_from(Reader& r) {
        KEGS_CREATE_RIDING_PET_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_kCreatedPetInfo = Codec<std::remove_cvref_t<decltype(v.m_kCreatedPetInfo)>>::get(r);
        v.m_vecKInventorySlotInfo = Codec<std::remove_cvref_t<decltype(v.m_vecKInventorySlotInfo)>>::get(r);
        return v;
    }
};

struct KEGS_GET_RIDING_PET_LIST_ACK {
    std::int32_t                m_iOK{};  // int
    std::vector<KRidingPetInfo> m_vecRidingPetList{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_vecRidingPetList)>>::put(w, m_vecRidingPetList);
    }

    static KEGS_GET_RIDING_PET_LIST_ACK read_from(Reader& r) {
        KEGS_GET_RIDING_PET_LIST_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_vecRidingPetList = Codec<std::remove_cvref_t<decltype(v.m_vecRidingPetList)>>::get(r);
        return v;
    }
};

struct KEGS_SUMMON_RIDING_PET_ACK {
    std::int32_t   m_iOK{};  // int
    KRidingPetInfo m_kRidingPetInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_kRidingPetInfo)>>::put(w, m_kRidingPetInfo);
    }

    static KEGS_SUMMON_RIDING_PET_ACK read_from(Reader& r) {
        KEGS_SUMMON_RIDING_PET_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_kRidingPetInfo = Codec<std::remove_cvref_t<decltype(v.m_kRidingPetInfo)>>::get(r);
        return v;
    }
};

struct KRoomInfo {
    char                                 m_RoomType{};
    std::int64_t                         m_RoomUID{};  // UidType
    std::uint32_t                        m_uiRoomListID{};  // UINT
    std::u16string                       m_RoomName{};  // std::wstring
    char                                 m_RoomState{};
    bool                                 m_bPublic{};
    bool                                 m_bTeamBalance{};
    std::u16string                       m_wstrPassword{};  // std::wstring
    char                                 m_MaxSlot{};
    char                                 m_JoinSlot{};
    std::u16string                       m_wstrUDPRelayIP{};  // std::wstring
    std::uint16_t                        m_usUDPRelayPort{};  // USHORT
    bool                                 m_bStartedByAutoParty{};
    char                                 m_PVPGameType{};
    bool                                 m_bIsItemMode{};
    std::int32_t                         m_iPVPChannelClass{};  // int
    char                                 m_WinKillNum{};
    bool                                 m_bCanIntrude{};
    float                                m_fPlayTime{};
    std::int16_t                         m_WorldID{};  // short
    std::int16_t                         m_ShowPvpMapWorldID{};  // short
    char                                 m_DifficultyLevel{};
    std::int32_t                         m_iDungeonID{};  // int
    char                                 m_cGetItemType{};
    char                                 m_cDungeonMode{};
    std::int64_t                         m_iPartyUID{};  // UidType
    std::int32_t                         m_iBattleFieldID{};  // int
    std::map<std::int16_t, std::int32_t> m_mapAllPlayersSelectedMap{};  // std::map< short, int >
    bool                                 m_bDefenceDungeonOpen{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_RoomType)>>::put(w, m_RoomType);
        Codec<std::remove_cvref_t<decltype(m_RoomUID)>>::put(w, m_RoomUID);
        Codec<std::remove_cvref_t<decltype(m_uiRoomListID)>>::put(w, m_uiRoomListID);
        Codec<std::remove_cvref_t<decltype(m_RoomName)>>::put(w, m_RoomName);
        Codec<std::remove_cvref_t<decltype(m_RoomState)>>::put(w, m_RoomState);
        Codec<std::remove_cvref_t<decltype(m_bPublic)>>::put(w, m_bPublic);
        Codec<std::remove_cvref_t<decltype(m_bTeamBalance)>>::put(w, m_bTeamBalance);
        Codec<std::remove_cvref_t<decltype(m_wstrPassword)>>::put(w, m_wstrPassword);
        Codec<std::remove_cvref_t<decltype(m_MaxSlot)>>::put(w, m_MaxSlot);
        Codec<std::remove_cvref_t<decltype(m_JoinSlot)>>::put(w, m_JoinSlot);
        Codec<std::remove_cvref_t<decltype(m_wstrUDPRelayIP)>>::put(w, m_wstrUDPRelayIP);
        Codec<std::remove_cvref_t<decltype(m_usUDPRelayPort)>>::put(w, m_usUDPRelayPort);
        Codec<std::remove_cvref_t<decltype(m_bStartedByAutoParty)>>::put(w, m_bStartedByAutoParty);
        Codec<std::remove_cvref_t<decltype(m_PVPGameType)>>::put(w, m_PVPGameType);
        Codec<std::remove_cvref_t<decltype(m_bIsItemMode)>>::put(w, m_bIsItemMode);
        Codec<std::remove_cvref_t<decltype(m_iPVPChannelClass)>>::put(w, m_iPVPChannelClass);
        Codec<std::remove_cvref_t<decltype(m_WinKillNum)>>::put(w, m_WinKillNum);
        Codec<std::remove_cvref_t<decltype(m_bCanIntrude)>>::put(w, m_bCanIntrude);
        Codec<std::remove_cvref_t<decltype(m_fPlayTime)>>::put(w, m_fPlayTime);
        Codec<std::remove_cvref_t<decltype(m_WorldID)>>::put(w, m_WorldID);
        Codec<std::remove_cvref_t<decltype(m_ShowPvpMapWorldID)>>::put(w, m_ShowPvpMapWorldID);
        Codec<std::remove_cvref_t<decltype(m_DifficultyLevel)>>::put(w, m_DifficultyLevel);
        Codec<std::remove_cvref_t<decltype(m_iDungeonID)>>::put(w, m_iDungeonID);
        Codec<std::remove_cvref_t<decltype(m_cGetItemType)>>::put(w, m_cGetItemType);
        Codec<std::remove_cvref_t<decltype(m_cDungeonMode)>>::put(w, m_cDungeonMode);
        Codec<std::remove_cvref_t<decltype(m_iPartyUID)>>::put(w, m_iPartyUID);
        Codec<std::remove_cvref_t<decltype(m_iBattleFieldID)>>::put(w, m_iBattleFieldID);
        Codec<std::remove_cvref_t<decltype(m_mapAllPlayersSelectedMap)>>::put(w, m_mapAllPlayersSelectedMap);
        Codec<std::remove_cvref_t<decltype(m_bDefenceDungeonOpen)>>::put(w, m_bDefenceDungeonOpen);
    }

    static KRoomInfo read_from(Reader& r) {
        KRoomInfo v;
        v.m_RoomType = Codec<std::remove_cvref_t<decltype(v.m_RoomType)>>::get(r);
        v.m_RoomUID = Codec<std::remove_cvref_t<decltype(v.m_RoomUID)>>::get(r);
        v.m_uiRoomListID = Codec<std::remove_cvref_t<decltype(v.m_uiRoomListID)>>::get(r);
        v.m_RoomName = Codec<std::remove_cvref_t<decltype(v.m_RoomName)>>::get(r);
        v.m_RoomState = Codec<std::remove_cvref_t<decltype(v.m_RoomState)>>::get(r);
        v.m_bPublic = Codec<std::remove_cvref_t<decltype(v.m_bPublic)>>::get(r);
        v.m_bTeamBalance = Codec<std::remove_cvref_t<decltype(v.m_bTeamBalance)>>::get(r);
        v.m_wstrPassword = Codec<std::remove_cvref_t<decltype(v.m_wstrPassword)>>::get(r);
        v.m_MaxSlot = Codec<std::remove_cvref_t<decltype(v.m_MaxSlot)>>::get(r);
        v.m_JoinSlot = Codec<std::remove_cvref_t<decltype(v.m_JoinSlot)>>::get(r);
        v.m_wstrUDPRelayIP = Codec<std::remove_cvref_t<decltype(v.m_wstrUDPRelayIP)>>::get(r);
        v.m_usUDPRelayPort = Codec<std::remove_cvref_t<decltype(v.m_usUDPRelayPort)>>::get(r);
        v.m_bStartedByAutoParty = Codec<std::remove_cvref_t<decltype(v.m_bStartedByAutoParty)>>::get(r);
        v.m_PVPGameType = Codec<std::remove_cvref_t<decltype(v.m_PVPGameType)>>::get(r);
        v.m_bIsItemMode = Codec<std::remove_cvref_t<decltype(v.m_bIsItemMode)>>::get(r);
        v.m_iPVPChannelClass = Codec<std::remove_cvref_t<decltype(v.m_iPVPChannelClass)>>::get(r);
        v.m_WinKillNum = Codec<std::remove_cvref_t<decltype(v.m_WinKillNum)>>::get(r);
        v.m_bCanIntrude = Codec<std::remove_cvref_t<decltype(v.m_bCanIntrude)>>::get(r);
        v.m_fPlayTime = Codec<std::remove_cvref_t<decltype(v.m_fPlayTime)>>::get(r);
        v.m_WorldID = Codec<std::remove_cvref_t<decltype(v.m_WorldID)>>::get(r);
        v.m_ShowPvpMapWorldID = Codec<std::remove_cvref_t<decltype(v.m_ShowPvpMapWorldID)>>::get(r);
        v.m_DifficultyLevel = Codec<std::remove_cvref_t<decltype(v.m_DifficultyLevel)>>::get(r);
        v.m_iDungeonID = Codec<std::remove_cvref_t<decltype(v.m_iDungeonID)>>::get(r);
        v.m_cGetItemType = Codec<std::remove_cvref_t<decltype(v.m_cGetItemType)>>::get(r);
        v.m_cDungeonMode = Codec<std::remove_cvref_t<decltype(v.m_cDungeonMode)>>::get(r);
        v.m_iPartyUID = Codec<std::remove_cvref_t<decltype(v.m_iPartyUID)>>::get(r);
        v.m_iBattleFieldID = Codec<std::remove_cvref_t<decltype(v.m_iBattleFieldID)>>::get(r);
        v.m_mapAllPlayersSelectedMap = Codec<std::remove_cvref_t<decltype(v.m_mapAllPlayersSelectedMap)>>::get(r);
        v.m_bDefenceDungeonOpen = Codec<std::remove_cvref_t<decltype(v.m_bDefenceDungeonOpen)>>::get(r);
        return v;
    }
};

struct KEGS_CREATE_ROOM_REQ {
    bool      m_bObserver{};
    KRoomInfo m_RoomInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_bObserver)>>::put(w, m_bObserver);
        Codec<std::remove_cvref_t<decltype(m_RoomInfo)>>::put(w, m_RoomInfo);
    }

    static KEGS_CREATE_ROOM_REQ read_from(Reader& r) {
        KEGS_CREATE_ROOM_REQ v;
        v.m_bObserver = Codec<std::remove_cvref_t<decltype(v.m_bObserver)>>::get(r);
        v.m_RoomInfo = Codec<std::remove_cvref_t<decltype(v.m_RoomInfo)>>::get(r);
        return v;
    }
};

struct KEGS_CREATE_TUTORIAL_ROOM_REQ {
    KRoomInfo m_RoomInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_RoomInfo)>>::put(w, m_RoomInfo);
    }

    static KEGS_CREATE_TUTORIAL_ROOM_REQ read_from(Reader& r) {
        KEGS_CREATE_TUTORIAL_ROOM_REQ v;
        v.m_RoomInfo = Codec<std::remove_cvref_t<decltype(v.m_RoomInfo)>>::get(r);
        return v;
    }
};

struct KEGS_INVITE_PVP_ROOM_NOT {
    std::u16string m_wstrNickName{};  // std::wstring
    KRoomInfo      m_kRoomInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_kRoomInfo)>>::put(w, m_kRoomInfo);
    }

    static KEGS_INVITE_PVP_ROOM_NOT read_from(Reader& r) {
        KEGS_INVITE_PVP_ROOM_NOT v;
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_kRoomInfo = Codec<std::remove_cvref_t<decltype(v.m_kRoomInfo)>>::get(r);
        return v;
    }
};

struct KEGS_ROOM_LIST_ACK {
    std::int32_t           m_iOK{};  // int
    std::uint32_t          m_nTotalPage{};  // UINT
    std::uint32_t          m_nViewPage{};  // UINT
    std::vector<KRoomInfo> m_vRoomInfo{};  // std::vector< KRoomInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_nTotalPage)>>::put(w, m_nTotalPage);
        Codec<std::remove_cvref_t<decltype(m_nViewPage)>>::put(w, m_nViewPage);
        Codec<std::remove_cvref_t<decltype(m_vRoomInfo)>>::put(w, m_vRoomInfo);
    }

    static KEGS_ROOM_LIST_ACK read_from(Reader& r) {
        KEGS_ROOM_LIST_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_nTotalPage = Codec<std::remove_cvref_t<decltype(v.m_nTotalPage)>>::get(r);
        v.m_nViewPage = Codec<std::remove_cvref_t<decltype(v.m_nViewPage)>>::get(r);
        v.m_vRoomInfo = Codec<std::remove_cvref_t<decltype(v.m_vRoomInfo)>>::get(r);
        return v;
    }
};

struct KELG_INVITE_PVP_ROOM_REQ {
    std::u16string m_wstrFromNickName{};  // std::wstring
    std::int64_t   m_iToUnitUID{};  // UidType
    KRoomInfo      m_kRoomInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrFromNickName)>>::put(w, m_wstrFromNickName);
        Codec<std::remove_cvref_t<decltype(m_iToUnitUID)>>::put(w, m_iToUnitUID);
        Codec<std::remove_cvref_t<decltype(m_kRoomInfo)>>::put(w, m_kRoomInfo);
    }

    static KELG_INVITE_PVP_ROOM_REQ read_from(Reader& r) {
        KELG_INVITE_PVP_ROOM_REQ v;
        v.m_wstrFromNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrFromNickName)>>::get(r);
        v.m_iToUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iToUnitUID)>>::get(r);
        v.m_kRoomInfo = Codec<std::remove_cvref_t<decltype(v.m_kRoomInfo)>>::get(r);
        return v;
    }
};

struct KERM_ROOM_LIST_INFO_NOT {
    std::int32_t m_iCode{};  // int
    KRoomInfo    m_kRoomInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iCode)>>::put(w, m_iCode);
        Codec<std::remove_cvref_t<decltype(m_kRoomInfo)>>::put(w, m_kRoomInfo);
    }

    static KERM_ROOM_LIST_INFO_NOT read_from(Reader& r) {
        KERM_ROOM_LIST_INFO_NOT v;
        v.m_iCode = Codec<std::remove_cvref_t<decltype(v.m_iCode)>>::get(r);
        v.m_kRoomInfo = Codec<std::remove_cvref_t<decltype(v.m_kRoomInfo)>>::get(r);
        return v;
    }
};

struct KSellPShopItemDBUpdateInfo {
    std::int64_t m_iItemUID{};  // UidType
    std::int32_t m_iQuantity{};  // int
    std::int32_t m_iTotalSellEDIn{};  // int
    std::int32_t m_iTotalSoldItemQuantity{};  // int
    std::int32_t m_iTotalSellCommissionED{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iItemUID)>>::put(w, m_iItemUID);
        Codec<std::remove_cvref_t<decltype(m_iQuantity)>>::put(w, m_iQuantity);
        Codec<std::remove_cvref_t<decltype(m_iTotalSellEDIn)>>::put(w, m_iTotalSellEDIn);
        Codec<std::remove_cvref_t<decltype(m_iTotalSoldItemQuantity)>>::put(w, m_iTotalSoldItemQuantity);
        Codec<std::remove_cvref_t<decltype(m_iTotalSellCommissionED)>>::put(w, m_iTotalSellCommissionED);
    }

    static KSellPShopItemDBUpdateInfo read_from(Reader& r) {
        KSellPShopItemDBUpdateInfo v;
        v.m_iItemUID = Codec<std::remove_cvref_t<decltype(v.m_iItemUID)>>::get(r);
        v.m_iQuantity = Codec<std::remove_cvref_t<decltype(v.m_iQuantity)>>::get(r);
        v.m_iTotalSellEDIn = Codec<std::remove_cvref_t<decltype(v.m_iTotalSellEDIn)>>::get(r);
        v.m_iTotalSoldItemQuantity = Codec<std::remove_cvref_t<decltype(v.m_iTotalSoldItemQuantity)>>::get(r);
        v.m_iTotalSellCommissionED = Codec<std::remove_cvref_t<decltype(v.m_iTotalSellCommissionED)>>::get(r);
        return v;
    }
};

struct KSellPersonalShopItemInfo {
    KInventoryItemInfo m_kInventoryItemInfo{};
    std::int32_t       m_iPricePerOne{};  // int
    std::int32_t       m_iFeePerOne{};  // int
    char               m_cPShopItemType{};
    std::int32_t       m_iTotalSellEDIn{};  // int
    std::int32_t       m_iTotalSoldItemQuantity{};  // int
    std::int64_t       m_iInventoryItemUID{};  // UidType

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kInventoryItemInfo)>>::put(w, m_kInventoryItemInfo);
        Codec<std::remove_cvref_t<decltype(m_iPricePerOne)>>::put(w, m_iPricePerOne);
        Codec<std::remove_cvref_t<decltype(m_iFeePerOne)>>::put(w, m_iFeePerOne);
        Codec<std::remove_cvref_t<decltype(m_cPShopItemType)>>::put(w, m_cPShopItemType);
        Codec<std::remove_cvref_t<decltype(m_iTotalSellEDIn)>>::put(w, m_iTotalSellEDIn);
        Codec<std::remove_cvref_t<decltype(m_iTotalSoldItemQuantity)>>::put(w, m_iTotalSoldItemQuantity);
        Codec<std::remove_cvref_t<decltype(m_iInventoryItemUID)>>::put(w, m_iInventoryItemUID);
    }

    static KSellPersonalShopItemInfo read_from(Reader& r) {
        KSellPersonalShopItemInfo v;
        v.m_kInventoryItemInfo = Codec<std::remove_cvref_t<decltype(v.m_kInventoryItemInfo)>>::get(r);
        v.m_iPricePerOne = Codec<std::remove_cvref_t<decltype(v.m_iPricePerOne)>>::get(r);
        v.m_iFeePerOne = Codec<std::remove_cvref_t<decltype(v.m_iFeePerOne)>>::get(r);
        v.m_cPShopItemType = Codec<std::remove_cvref_t<decltype(v.m_cPShopItemType)>>::get(r);
        v.m_iTotalSellEDIn = Codec<std::remove_cvref_t<decltype(v.m_iTotalSellEDIn)>>::get(r);
        v.m_iTotalSoldItemQuantity = Codec<std::remove_cvref_t<decltype(v.m_iTotalSoldItemQuantity)>>::get(r);
        v.m_iInventoryItemUID = Codec<std::remove_cvref_t<decltype(v.m_iInventoryItemUID)>>::get(r);
        return v;
    }
};

struct KDBE_INSERT_TO_INVENTORY_PICK_UP_FROM_PSHOP_AGENCY_ACK {
    std::int32_t                           m_iOK{};  // int
    std::map<std::int32_t, std::int32_t>   m_mapInsertItem{};  // std::map< int, int >
    KItemQuantityUpdate                    m_kItemQuantityUpdate{};
    std::vector<KInventoryItemInfo>        m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >
    std::map<std::int64_t, KItemInfo>      m_mapItemInfo{};  // std::map< UidType, KItemInfo >
    std::vector<KSellPersonalShopItemInfo> m_vecSellItemInfo{};  // std::vector< KSellPersonalShopItemInfo >
    bool                                   m_bRemainSellItem{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_mapInsertItem)>>::put(w, m_mapInsertItem);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
        Codec<std::remove_cvref_t<decltype(m_mapItemInfo)>>::put(w, m_mapItemInfo);
        Codec<std::remove_cvref_t<decltype(m_vecSellItemInfo)>>::put(w, m_vecSellItemInfo);
        Codec<std::remove_cvref_t<decltype(m_bRemainSellItem)>>::put(w, m_bRemainSellItem);
    }

    static KDBE_INSERT_TO_INVENTORY_PICK_UP_FROM_PSHOP_AGENCY_ACK read_from(Reader& r) {
        KDBE_INSERT_TO_INVENTORY_PICK_UP_FROM_PSHOP_AGENCY_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_mapInsertItem = Codec<std::remove_cvref_t<decltype(v.m_mapInsertItem)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        v.m_mapItemInfo = Codec<std::remove_cvref_t<decltype(v.m_mapItemInfo)>>::get(r);
        v.m_vecSellItemInfo = Codec<std::remove_cvref_t<decltype(v.m_vecSellItemInfo)>>::get(r);
        v.m_bRemainSellItem = Codec<std::remove_cvref_t<decltype(v.m_bRemainSellItem)>>::get(r);
        return v;
    }
};

struct KDBE_INSERT_TO_INVENTORY_PICK_UP_FROM_PSHOP_AGENCY_REQ {
    std::int64_t                           m_iUnitUID{};  // UidType
    std::map<std::int32_t, std::int32_t>   m_mapInsertItem{};  // std::map< int, int >
    KItemQuantityUpdate                    m_kItemQuantityUpdate{};
    std::vector<KInventoryItemInfo>        m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >
    std::vector<KItemInfo>                 m_vecItemInfo{};  // std::vector< KItemInfo >
    std::vector<KSellPersonalShopItemInfo> m_vecSellItemInfo{};  // std::vector< KSellPersonalShopItemInfo >
    bool                                   m_bRemainSellItem{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_mapInsertItem)>>::put(w, m_mapInsertItem);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
        Codec<std::remove_cvref_t<decltype(m_vecItemInfo)>>::put(w, m_vecItemInfo);
        Codec<std::remove_cvref_t<decltype(m_vecSellItemInfo)>>::put(w, m_vecSellItemInfo);
        Codec<std::remove_cvref_t<decltype(m_bRemainSellItem)>>::put(w, m_bRemainSellItem);
    }

    static KDBE_INSERT_TO_INVENTORY_PICK_UP_FROM_PSHOP_AGENCY_REQ read_from(Reader& r) {
        KDBE_INSERT_TO_INVENTORY_PICK_UP_FROM_PSHOP_AGENCY_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_mapInsertItem = Codec<std::remove_cvref_t<decltype(v.m_mapInsertItem)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        v.m_vecItemInfo = Codec<std::remove_cvref_t<decltype(v.m_vecItemInfo)>>::get(r);
        v.m_vecSellItemInfo = Codec<std::remove_cvref_t<decltype(v.m_vecSellItemInfo)>>::get(r);
        v.m_bRemainSellItem = Codec<std::remove_cvref_t<decltype(v.m_bRemainSellItem)>>::get(r);
        return v;
    }
};

struct KDBE_PICK_UP_FROM_PSHOP_AGENCY_ACK {
    std::int32_t                           m_iOK{};  // int
    std::int64_t                           m_iGSUID{};  // UidType
    std::int64_t                           m_iUnitUID{};  // UidType
    std::vector<KSellPersonalShopItemInfo> m_vecPickUpItemList{};  // std::vector< KSellPersonalShopItemInfo >
    std::int32_t                           m_iSellItemTotalED{};  // int
    std::vector<std::int64_t>              m_vecPickUpFailList{};  // std::vector< UidType >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iGSUID)>>::put(w, m_iGSUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_vecPickUpItemList)>>::put(w, m_vecPickUpItemList);
        Codec<std::remove_cvref_t<decltype(m_iSellItemTotalED)>>::put(w, m_iSellItemTotalED);
        Codec<std::remove_cvref_t<decltype(m_vecPickUpFailList)>>::put(w, m_vecPickUpFailList);
    }

    static KDBE_PICK_UP_FROM_PSHOP_AGENCY_ACK read_from(Reader& r) {
        KDBE_PICK_UP_FROM_PSHOP_AGENCY_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iGSUID = Codec<std::remove_cvref_t<decltype(v.m_iGSUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_vecPickUpItemList = Codec<std::remove_cvref_t<decltype(v.m_vecPickUpItemList)>>::get(r);
        v.m_iSellItemTotalED = Codec<std::remove_cvref_t<decltype(v.m_iSellItemTotalED)>>::get(r);
        v.m_vecPickUpFailList = Codec<std::remove_cvref_t<decltype(v.m_vecPickUpFailList)>>::get(r);
        return v;
    }
};

struct KDBE_PICK_UP_FROM_PSHOP_AGENCY_REQ {
    std::int64_t                           m_iGSUID{};  // UidType
    std::int64_t                           m_iUnitUID{};  // UidType
    std::vector<KSellPersonalShopItemInfo> m_vecPickUpItemList{};  // std::vector< KSellPersonalShopItemInfo >
    std::int32_t                           m_iSellItemTotalED{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iGSUID)>>::put(w, m_iGSUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_vecPickUpItemList)>>::put(w, m_vecPickUpItemList);
        Codec<std::remove_cvref_t<decltype(m_iSellItemTotalED)>>::put(w, m_iSellItemTotalED);
    }

    static KDBE_PICK_UP_FROM_PSHOP_AGENCY_REQ read_from(Reader& r) {
        KDBE_PICK_UP_FROM_PSHOP_AGENCY_REQ v;
        v.m_iGSUID = Codec<std::remove_cvref_t<decltype(v.m_iGSUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_vecPickUpItemList = Codec<std::remove_cvref_t<decltype(v.m_vecPickUpItemList)>>::get(r);
        v.m_iSellItemTotalED = Codec<std::remove_cvref_t<decltype(v.m_iSellItemTotalED)>>::get(r);
        return v;
    }
};

struct KDBE_PREPARE_REG_PSHOP_AGENCY_ITEM_ACK {
    std::int32_t                           m_iOK{};  // int
    KItemQuantityUpdate                    m_kItemQuantityUpdate{};
    std::vector<KInventoryItemInfo>        m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >
    std::u16string                         m_wstrPersonalShopName{};  // std::wstring
    std::vector<KSellPersonalShopItemInfo> m_vecSellItemInfo{};  // std::vector< KSellPersonalShopItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
        Codec<std::remove_cvref_t<decltype(m_wstrPersonalShopName)>>::put(w, m_wstrPersonalShopName);
        Codec<std::remove_cvref_t<decltype(m_vecSellItemInfo)>>::put(w, m_vecSellItemInfo);
    }

    static KDBE_PREPARE_REG_PSHOP_AGENCY_ITEM_ACK read_from(Reader& r) {
        KDBE_PREPARE_REG_PSHOP_AGENCY_ITEM_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        v.m_wstrPersonalShopName = Codec<std::remove_cvref_t<decltype(v.m_wstrPersonalShopName)>>::get(r);
        v.m_vecSellItemInfo = Codec<std::remove_cvref_t<decltype(v.m_vecSellItemInfo)>>::get(r);
        return v;
    }
};

struct KDBE_PREPARE_REG_PSHOP_AGENCY_ITEM_REQ {
    std::int64_t                           m_iUnitUID{};  // UidType
    KItemQuantityUpdate                    m_kItemQuantityUpdate{};
    std::vector<KInventoryItemInfo>        m_vecUpdatedInventorySlot{};  // std::vector< KInventoryItemInfo >
    std::u16string                         m_wstrPersonalShopName{};  // std::wstring
    std::vector<KSellPersonalShopItemInfo> m_vecSellItemInfo{};  // std::vector< KSellPersonalShopItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_kItemQuantityUpdate)>>::put(w, m_kItemQuantityUpdate);
        Codec<std::remove_cvref_t<decltype(m_vecUpdatedInventorySlot)>>::put(w, m_vecUpdatedInventorySlot);
        Codec<std::remove_cvref_t<decltype(m_wstrPersonalShopName)>>::put(w, m_wstrPersonalShopName);
        Codec<std::remove_cvref_t<decltype(m_vecSellItemInfo)>>::put(w, m_vecSellItemInfo);
    }

    static KDBE_PREPARE_REG_PSHOP_AGENCY_ITEM_REQ read_from(Reader& r) {
        KDBE_PREPARE_REG_PSHOP_AGENCY_ITEM_REQ v;
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_kItemQuantityUpdate = Codec<std::remove_cvref_t<decltype(v.m_kItemQuantityUpdate)>>::get(r);
        v.m_vecUpdatedInventorySlot = Codec<std::remove_cvref_t<decltype(v.m_vecUpdatedInventorySlot)>>::get(r);
        v.m_wstrPersonalShopName = Codec<std::remove_cvref_t<decltype(v.m_wstrPersonalShopName)>>::get(r);
        v.m_vecSellItemInfo = Codec<std::remove_cvref_t<decltype(v.m_vecSellItemInfo)>>::get(r);
        return v;
    }
};

struct KEGS_JOIN_MY_PSHOP_AGENCY_ACK {
    std::int32_t                           m_iOK{};  // int
    char                                   m_cJoinType{};
    std::u16string                         m_wstrPersonalShopName{};  // std::wstring
    std::u16string                         m_wstrNickName{};  // std::wstring
    std::vector<KSellPersonalShopItemInfo> m_vecSellItemInfo{};  // std::vector< KSellPersonalShopItemInfo >
    std::int32_t                           m_iUsedItemID{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_cJoinType)>>::put(w, m_cJoinType);
        Codec<std::remove_cvref_t<decltype(m_wstrPersonalShopName)>>::put(w, m_wstrPersonalShopName);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_vecSellItemInfo)>>::put(w, m_vecSellItemInfo);
        Codec<std::remove_cvref_t<decltype(m_iUsedItemID)>>::put(w, m_iUsedItemID);
    }

    static KEGS_JOIN_MY_PSHOP_AGENCY_ACK read_from(Reader& r) {
        KEGS_JOIN_MY_PSHOP_AGENCY_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_cJoinType = Codec<std::remove_cvref_t<decltype(v.m_cJoinType)>>::get(r);
        v.m_wstrPersonalShopName = Codec<std::remove_cvref_t<decltype(v.m_wstrPersonalShopName)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_vecSellItemInfo = Codec<std::remove_cvref_t<decltype(v.m_vecSellItemInfo)>>::get(r);
        v.m_iUsedItemID = Codec<std::remove_cvref_t<decltype(v.m_iUsedItemID)>>::get(r);
        return v;
    }
};

struct KEGS_JOIN_PERSONAL_SHOP_ACK {
    std::int32_t                           m_iOK{};  // int
    std::u16string                         m_wstrPersonalShopName{};  // std::wstring
    std::u16string                         m_wstrNickName{};  // std::wstring
    std::vector<KSellPersonalShopItemInfo> m_vecSellItemInfo{};  // std::vector< KSellPersonalShopItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_wstrPersonalShopName)>>::put(w, m_wstrPersonalShopName);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_vecSellItemInfo)>>::put(w, m_vecSellItemInfo);
    }

    static KEGS_JOIN_PERSONAL_SHOP_ACK read_from(Reader& r) {
        KEGS_JOIN_PERSONAL_SHOP_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_wstrPersonalShopName = Codec<std::remove_cvref_t<decltype(v.m_wstrPersonalShopName)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_vecSellItemInfo = Codec<std::remove_cvref_t<decltype(v.m_vecSellItemInfo)>>::get(r);
        return v;
    }
};

struct KEGS_PICK_UP_FROM_PSHOP_AGENCY_ACK {
    std::int32_t                           m_iOK{};  // int
    std::int32_t                           m_iED{};  // int
    std::vector<KSellPersonalShopItemInfo> m_vecSellItemInfo{};  // std::vector< KSellPersonalShopItemInfo >
    std::vector<KInventoryItemInfo>        m_vecInventorySlotInfo{};  // std::vector< KInventoryItemInfo >
    bool                                   m_bRemainSellItem{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iED)>>::put(w, m_iED);
        Codec<std::remove_cvref_t<decltype(m_vecSellItemInfo)>>::put(w, m_vecSellItemInfo);
        Codec<std::remove_cvref_t<decltype(m_vecInventorySlotInfo)>>::put(w, m_vecInventorySlotInfo);
        Codec<std::remove_cvref_t<decltype(m_bRemainSellItem)>>::put(w, m_bRemainSellItem);
    }

    static KEGS_PICK_UP_FROM_PSHOP_AGENCY_ACK read_from(Reader& r) {
        KEGS_PICK_UP_FROM_PSHOP_AGENCY_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iED = Codec<std::remove_cvref_t<decltype(v.m_iED)>>::get(r);
        v.m_vecSellItemInfo = Codec<std::remove_cvref_t<decltype(v.m_vecSellItemInfo)>>::get(r);
        v.m_vecInventorySlotInfo = Codec<std::remove_cvref_t<decltype(v.m_vecInventorySlotInfo)>>::get(r);
        v.m_bRemainSellItem = Codec<std::remove_cvref_t<decltype(v.m_bRemainSellItem)>>::get(r);
        return v;
    }
};

struct KEGS_REG_PERSONAL_SHOP_ITEM_ACK {
    std::int32_t                           m_iOK{};  // int
    std::int32_t                           m_iED{};  // int
    std::u16string                         m_wstrPersonalShopName{};  // std::wstring
    std::vector<KSellPersonalShopItemInfo> m_vecSellItemInfo{};  // std::vector< KSellPersonalShopItemInfo >
    std::vector<KInventoryItemInfo>        m_vecInventorySlotInfo{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iED)>>::put(w, m_iED);
        Codec<std::remove_cvref_t<decltype(m_wstrPersonalShopName)>>::put(w, m_wstrPersonalShopName);
        Codec<std::remove_cvref_t<decltype(m_vecSellItemInfo)>>::put(w, m_vecSellItemInfo);
        Codec<std::remove_cvref_t<decltype(m_vecInventorySlotInfo)>>::put(w, m_vecInventorySlotInfo);
    }

    static KEGS_REG_PERSONAL_SHOP_ITEM_ACK read_from(Reader& r) {
        KEGS_REG_PERSONAL_SHOP_ITEM_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iED = Codec<std::remove_cvref_t<decltype(v.m_iED)>>::get(r);
        v.m_wstrPersonalShopName = Codec<std::remove_cvref_t<decltype(v.m_wstrPersonalShopName)>>::get(r);
        v.m_vecSellItemInfo = Codec<std::remove_cvref_t<decltype(v.m_vecSellItemInfo)>>::get(r);
        v.m_vecInventorySlotInfo = Codec<std::remove_cvref_t<decltype(v.m_vecInventorySlotInfo)>>::get(r);
        return v;
    }
};

struct KERM_JOIN_PERSONAL_SHOP_ACK {
    std::int32_t                           m_iOK{};  // int
    std::int64_t                           m_iPersonalShopUID{};  // UidType
    std::u16string                         m_wstrPersonalShopName{};  // std::wstring
    std::u16string                         m_wstrNickName{};  // std::wstring
    std::vector<KSellPersonalShopItemInfo> m_vecSellItemInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iPersonalShopUID)>>::put(w, m_iPersonalShopUID);
        Codec<std::remove_cvref_t<decltype(m_wstrPersonalShopName)>>::put(w, m_wstrPersonalShopName);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_vecSellItemInfo)>>::put(w, m_vecSellItemInfo);
    }

    static KERM_JOIN_PERSONAL_SHOP_ACK read_from(Reader& r) {
        KERM_JOIN_PERSONAL_SHOP_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iPersonalShopUID = Codec<std::remove_cvref_t<decltype(v.m_iPersonalShopUID)>>::get(r);
        v.m_wstrPersonalShopName = Codec<std::remove_cvref_t<decltype(v.m_wstrPersonalShopName)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_vecSellItemInfo = Codec<std::remove_cvref_t<decltype(v.m_vecSellItemInfo)>>::get(r);
        return v;
    }
};

struct KERM_PERSONAL_SHOP_INFO_NOT {
    std::int32_t                           m_iCode{};  // int
    std::int64_t                           m_iPSUID{};  // UidType
    std::u16string                         m_wstrSellerNickName{};  // std::wstring
    std::int64_t                           m_iHostUnitUID{};  // UidType
    std::vector<KSellPersonalShopItemInfo> m_vecItemInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iCode)>>::put(w, m_iCode);
        Codec<std::remove_cvref_t<decltype(m_iPSUID)>>::put(w, m_iPSUID);
        Codec<std::remove_cvref_t<decltype(m_wstrSellerNickName)>>::put(w, m_wstrSellerNickName);
        Codec<std::remove_cvref_t<decltype(m_iHostUnitUID)>>::put(w, m_iHostUnitUID);
        Codec<std::remove_cvref_t<decltype(m_vecItemInfo)>>::put(w, m_vecItemInfo);
    }

    static KERM_PERSONAL_SHOP_INFO_NOT read_from(Reader& r) {
        KERM_PERSONAL_SHOP_INFO_NOT v;
        v.m_iCode = Codec<std::remove_cvref_t<decltype(v.m_iCode)>>::get(r);
        v.m_iPSUID = Codec<std::remove_cvref_t<decltype(v.m_iPSUID)>>::get(r);
        v.m_wstrSellerNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrSellerNickName)>>::get(r);
        v.m_iHostUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iHostUnitUID)>>::get(r);
        v.m_vecItemInfo = Codec<std::remove_cvref_t<decltype(v.m_vecItemInfo)>>::get(r);
        return v;
    }
};

struct KERM_PICK_UP_FROM_PSHOP_AGENCY_ACK {
    std::int32_t                           m_iOK{};  // int
    std::int32_t                           m_iTotalSellEDIn{};  // int
    std::vector<KSellPersonalShopItemInfo> m_vecPickUpSuccessItemList{};  // std::vector< KSellPersonalShopItemInfo >
    std::vector<KSellPersonalShopItemInfo> m_vecSellItemInfo{};  // std::vector< KSellPersonalShopItemInfo >
    bool                                   m_bRemainSellItem{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iTotalSellEDIn)>>::put(w, m_iTotalSellEDIn);
        Codec<std::remove_cvref_t<decltype(m_vecPickUpSuccessItemList)>>::put(w, m_vecPickUpSuccessItemList);
        Codec<std::remove_cvref_t<decltype(m_vecSellItemInfo)>>::put(w, m_vecSellItemInfo);
        Codec<std::remove_cvref_t<decltype(m_bRemainSellItem)>>::put(w, m_bRemainSellItem);
    }

    static KERM_PICK_UP_FROM_PSHOP_AGENCY_ACK read_from(Reader& r) {
        KERM_PICK_UP_FROM_PSHOP_AGENCY_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iTotalSellEDIn = Codec<std::remove_cvref_t<decltype(v.m_iTotalSellEDIn)>>::get(r);
        v.m_vecPickUpSuccessItemList = Codec<std::remove_cvref_t<decltype(v.m_vecPickUpSuccessItemList)>>::get(r);
        v.m_vecSellItemInfo = Codec<std::remove_cvref_t<decltype(v.m_vecSellItemInfo)>>::get(r);
        v.m_bRemainSellItem = Codec<std::remove_cvref_t<decltype(v.m_bRemainSellItem)>>::get(r);
        return v;
    }
};

struct KERM_REG_PERSONAL_SHOP_ITEM_ACK {
    std::int32_t                           m_iOK{};  // int
    std::int64_t                           m_iGSUID{};  // UidType
    std::int64_t                           m_iUnitUID{};  // UidType
    std::vector<KInventoryItemInfo>        m_vecInventorySlotInfo{};  // std::vector< KInventoryItemInfo >
    std::int32_t                           m_iOpeningED{};  // int
    std::u16string                         m_wstrPersonalShopName{};  // std::wstring
    std::vector<KSellPersonalShopItemInfo> m_vecSellItemInfo{};  // std::vector< KSellPersonalShopItemInfo >
    std::vector<KSellPersonalShopItemInfo> m_vecAddCompleteItemInfo{};  // std::vector< KSellPersonalShopItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iGSUID)>>::put(w, m_iGSUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_vecInventorySlotInfo)>>::put(w, m_vecInventorySlotInfo);
        Codec<std::remove_cvref_t<decltype(m_iOpeningED)>>::put(w, m_iOpeningED);
        Codec<std::remove_cvref_t<decltype(m_wstrPersonalShopName)>>::put(w, m_wstrPersonalShopName);
        Codec<std::remove_cvref_t<decltype(m_vecSellItemInfo)>>::put(w, m_vecSellItemInfo);
        Codec<std::remove_cvref_t<decltype(m_vecAddCompleteItemInfo)>>::put(w, m_vecAddCompleteItemInfo);
    }

    static KERM_REG_PERSONAL_SHOP_ITEM_ACK read_from(Reader& r) {
        KERM_REG_PERSONAL_SHOP_ITEM_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iGSUID = Codec<std::remove_cvref_t<decltype(v.m_iGSUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_vecInventorySlotInfo = Codec<std::remove_cvref_t<decltype(v.m_vecInventorySlotInfo)>>::get(r);
        v.m_iOpeningED = Codec<std::remove_cvref_t<decltype(v.m_iOpeningED)>>::get(r);
        v.m_wstrPersonalShopName = Codec<std::remove_cvref_t<decltype(v.m_wstrPersonalShopName)>>::get(r);
        v.m_vecSellItemInfo = Codec<std::remove_cvref_t<decltype(v.m_vecSellItemInfo)>>::get(r);
        v.m_vecAddCompleteItemInfo = Codec<std::remove_cvref_t<decltype(v.m_vecAddCompleteItemInfo)>>::get(r);
        return v;
    }
};

struct KERM_REG_PERSONAL_SHOP_ITEM_REQ {
    std::u16string                         m_wstrPersonalShopName{};  // std::wstring
    std::vector<KSellPersonalShopItemInfo> m_vecSellItemInfo{};  // std::vector< KSellPersonalShopItemInfo >
    std::vector<KInventoryItemInfo>        m_vecInventorySlotInfo{};  // std::vector< KInventoryItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_wstrPersonalShopName)>>::put(w, m_wstrPersonalShopName);
        Codec<std::remove_cvref_t<decltype(m_vecSellItemInfo)>>::put(w, m_vecSellItemInfo);
        Codec<std::remove_cvref_t<decltype(m_vecInventorySlotInfo)>>::put(w, m_vecInventorySlotInfo);
    }

    static KERM_REG_PERSONAL_SHOP_ITEM_REQ read_from(Reader& r) {
        KERM_REG_PERSONAL_SHOP_ITEM_REQ v;
        v.m_wstrPersonalShopName = Codec<std::remove_cvref_t<decltype(v.m_wstrPersonalShopName)>>::get(r);
        v.m_vecSellItemInfo = Codec<std::remove_cvref_t<decltype(v.m_vecSellItemInfo)>>::get(r);
        v.m_vecInventorySlotInfo = Codec<std::remove_cvref_t<decltype(v.m_vecInventorySlotInfo)>>::get(r);
        return v;
    }
};

struct KPersonalShopAgencyInfo {
    std::int64_t                           m_iHostUnitUID{};  // UidType
    std::int64_t                           m_iPShopAgencyUID{};  // UidType
    std::u16string                         m_wstrAgencyOpenDate{};  // std::wstring
    std::u16string                         m_wstrAgencyExpirationDate{};  // std::wstring
    bool                                   m_bOnSale{};
    std::vector<KSellPersonalShopItemInfo> m_vecSellPShopAgencyItem{};  // std::vector< KSellPersonalShopItemInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iHostUnitUID)>>::put(w, m_iHostUnitUID);
        Codec<std::remove_cvref_t<decltype(m_iPShopAgencyUID)>>::put(w, m_iPShopAgencyUID);
        Codec<std::remove_cvref_t<decltype(m_wstrAgencyOpenDate)>>::put(w, m_wstrAgencyOpenDate);
        Codec<std::remove_cvref_t<decltype(m_wstrAgencyExpirationDate)>>::put(w, m_wstrAgencyExpirationDate);
        Codec<std::remove_cvref_t<decltype(m_bOnSale)>>::put(w, m_bOnSale);
        Codec<std::remove_cvref_t<decltype(m_vecSellPShopAgencyItem)>>::put(w, m_vecSellPShopAgencyItem);
    }

    static KPersonalShopAgencyInfo read_from(Reader& r) {
        KPersonalShopAgencyInfo v;
        v.m_iHostUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iHostUnitUID)>>::get(r);
        v.m_iPShopAgencyUID = Codec<std::remove_cvref_t<decltype(v.m_iPShopAgencyUID)>>::get(r);
        v.m_wstrAgencyOpenDate = Codec<std::remove_cvref_t<decltype(v.m_wstrAgencyOpenDate)>>::get(r);
        v.m_wstrAgencyExpirationDate = Codec<std::remove_cvref_t<decltype(v.m_wstrAgencyExpirationDate)>>::get(r);
        v.m_bOnSale = Codec<std::remove_cvref_t<decltype(v.m_bOnSale)>>::get(r);
        v.m_vecSellPShopAgencyItem = Codec<std::remove_cvref_t<decltype(v.m_vecSellPShopAgencyItem)>>::get(r);
        return v;
    }
};

struct KSellPShopItemBackupData {
    KSellPersonalShopItemInfo m_kSellPShopItemInfo{};
    std::int32_t              m_iTotalSoldItemQuantity{};  // int
    std::int32_t              m_iTotalSellCommissionED{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_kSellPShopItemInfo)>>::put(w, m_kSellPShopItemInfo);
        Codec<std::remove_cvref_t<decltype(m_iTotalSoldItemQuantity)>>::put(w, m_iTotalSoldItemQuantity);
        Codec<std::remove_cvref_t<decltype(m_iTotalSellCommissionED)>>::put(w, m_iTotalSellCommissionED);
    }

    static KSellPShopItemBackupData read_from(Reader& r) {
        KSellPShopItemBackupData v;
        v.m_kSellPShopItemInfo = Codec<std::remove_cvref_t<decltype(v.m_kSellPShopItemInfo)>>::get(r);
        v.m_iTotalSoldItemQuantity = Codec<std::remove_cvref_t<decltype(v.m_iTotalSoldItemQuantity)>>::get(r);
        v.m_iTotalSellCommissionED = Codec<std::remove_cvref_t<decltype(v.m_iTotalSellCommissionED)>>::get(r);
        return v;
    }
};

struct KERM_CHECK_MY_PSHOP_AGENCY_INFO_REQ {
    std::int64_t                          m_iUserUID{};  // UidType
    std::int64_t                          m_iUnitUID{};  // UidType
    std::u16string                        m_wstrNickName{};  // std::wstring
    char                                  m_cPersonalShopType{};
    std::u16string                        m_wstrAgencyExpirationDate{};  // std::wstring
    std::u16string                        m_wstrAgencyOpenDate{};  // std::wstring
    std::u16string                        m_wstrPersonalShopName{};  // std::wstring
    std::vector<KSellPShopItemBackupData> m_vecSellItemInfo{};  // std::vector< KSellPShopItemBackupData >
    bool                                  m_bOnSale{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_cPersonalShopType)>>::put(w, m_cPersonalShopType);
        Codec<std::remove_cvref_t<decltype(m_wstrAgencyExpirationDate)>>::put(w, m_wstrAgencyExpirationDate);
        Codec<std::remove_cvref_t<decltype(m_wstrAgencyOpenDate)>>::put(w, m_wstrAgencyOpenDate);
        Codec<std::remove_cvref_t<decltype(m_wstrPersonalShopName)>>::put(w, m_wstrPersonalShopName);
        Codec<std::remove_cvref_t<decltype(m_vecSellItemInfo)>>::put(w, m_vecSellItemInfo);
        Codec<std::remove_cvref_t<decltype(m_bOnSale)>>::put(w, m_bOnSale);
    }

    static KERM_CHECK_MY_PSHOP_AGENCY_INFO_REQ read_from(Reader& r) {
        KERM_CHECK_MY_PSHOP_AGENCY_INFO_REQ v;
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_cPersonalShopType = Codec<std::remove_cvref_t<decltype(v.m_cPersonalShopType)>>::get(r);
        v.m_wstrAgencyExpirationDate = Codec<std::remove_cvref_t<decltype(v.m_wstrAgencyExpirationDate)>>::get(r);
        v.m_wstrAgencyOpenDate = Codec<std::remove_cvref_t<decltype(v.m_wstrAgencyOpenDate)>>::get(r);
        v.m_wstrPersonalShopName = Codec<std::remove_cvref_t<decltype(v.m_wstrPersonalShopName)>>::get(r);
        v.m_vecSellItemInfo = Codec<std::remove_cvref_t<decltype(v.m_vecSellItemInfo)>>::get(r);
        v.m_bOnSale = Codec<std::remove_cvref_t<decltype(v.m_bOnSale)>>::get(r);
        return v;
    }
};

struct KERM_OPEN_PSHOP_AGENCY_BY_SERVER_NOT {
    std::int64_t                          m_iUserUID{};  // UidType
    std::int64_t                          m_iUnitUID{};  // UidType
    std::u16string                        m_wstrNickName{};  // std::wstring
    char                                  m_cPersonalShopType{};
    std::u16string                        m_wstrAgencyExpirationDate{};  // std::wstring
    std::u16string                        m_wstrAgencyOpenDate{};  // std::wstring
    std::u16string                        m_wstrPersonalShopName{};  // std::wstring
    std::vector<KSellPShopItemBackupData> m_vecSellItemInfo{};  // std::vector< KSellPShopItemBackupData >
    bool                                  m_bOnSale{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUserUID)>>::put(w, m_iUserUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_cPersonalShopType)>>::put(w, m_cPersonalShopType);
        Codec<std::remove_cvref_t<decltype(m_wstrAgencyExpirationDate)>>::put(w, m_wstrAgencyExpirationDate);
        Codec<std::remove_cvref_t<decltype(m_wstrAgencyOpenDate)>>::put(w, m_wstrAgencyOpenDate);
        Codec<std::remove_cvref_t<decltype(m_wstrPersonalShopName)>>::put(w, m_wstrPersonalShopName);
        Codec<std::remove_cvref_t<decltype(m_vecSellItemInfo)>>::put(w, m_vecSellItemInfo);
        Codec<std::remove_cvref_t<decltype(m_bOnSale)>>::put(w, m_bOnSale);
    }

    static KERM_OPEN_PSHOP_AGENCY_BY_SERVER_NOT read_from(Reader& r) {
        KERM_OPEN_PSHOP_AGENCY_BY_SERVER_NOT v;
        v.m_iUserUID = Codec<std::remove_cvref_t<decltype(v.m_iUserUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_cPersonalShopType = Codec<std::remove_cvref_t<decltype(v.m_cPersonalShopType)>>::get(r);
        v.m_wstrAgencyExpirationDate = Codec<std::remove_cvref_t<decltype(v.m_wstrAgencyExpirationDate)>>::get(r);
        v.m_wstrAgencyOpenDate = Codec<std::remove_cvref_t<decltype(v.m_wstrAgencyOpenDate)>>::get(r);
        v.m_wstrPersonalShopName = Codec<std::remove_cvref_t<decltype(v.m_wstrPersonalShopName)>>::get(r);
        v.m_vecSellItemInfo = Codec<std::remove_cvref_t<decltype(v.m_vecSellItemInfo)>>::get(r);
        v.m_bOnSale = Codec<std::remove_cvref_t<decltype(v.m_bOnSale)>>::get(r);
        return v;
    }
};

struct KDBE_LOAD_PSHOP_AGENCY_ACK {
    std::vector<KERM_OPEN_PSHOP_AGENCY_BY_SERVER_NOT> m_vecOpenPShopAgency{};  // std::vector< KERM_OPEN_PSHOP_AGENCY_BY_SERVER_NOT >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_vecOpenPShopAgency)>>::put(w, m_vecOpenPShopAgency);
    }

    static KDBE_LOAD_PSHOP_AGENCY_ACK read_from(Reader& r) {
        KDBE_LOAD_PSHOP_AGENCY_ACK v;
        v.m_vecOpenPShopAgency = Codec<std::remove_cvref_t<decltype(v.m_vecOpenPShopAgency)>>::get(r);
        return v;
    }
};

struct KServerGroupInfo {
    std::int32_t   m_iOrder{};  // int
    std::int32_t   m_iServerGroupUID{};  // int
    std::u16string m_wstrServerGroupName{};  // std::wstring
    std::u16string m_kServerIP{};  // std::wstring
    std::uint16_t  m_usMasterPort{};  // u_short
    std::uint16_t  m_usNCUDPPort{};  // u_short
    bool           m_bEnable{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOrder)>>::put(w, m_iOrder);
        Codec<std::remove_cvref_t<decltype(m_iServerGroupUID)>>::put(w, m_iServerGroupUID);
        Codec<std::remove_cvref_t<decltype(m_wstrServerGroupName)>>::put(w, m_wstrServerGroupName);
        Codec<std::remove_cvref_t<decltype(m_kServerIP)>>::put(w, m_kServerIP);
        Codec<std::remove_cvref_t<decltype(m_usMasterPort)>>::put(w, m_usMasterPort);
        Codec<std::remove_cvref_t<decltype(m_usNCUDPPort)>>::put(w, m_usNCUDPPort);
        Codec<std::remove_cvref_t<decltype(m_bEnable)>>::put(w, m_bEnable);
    }

    static KServerGroupInfo read_from(Reader& r) {
        KServerGroupInfo v;
        v.m_iOrder = Codec<std::remove_cvref_t<decltype(v.m_iOrder)>>::get(r);
        v.m_iServerGroupUID = Codec<std::remove_cvref_t<decltype(v.m_iServerGroupUID)>>::get(r);
        v.m_wstrServerGroupName = Codec<std::remove_cvref_t<decltype(v.m_wstrServerGroupName)>>::get(r);
        v.m_kServerIP = Codec<std::remove_cvref_t<decltype(v.m_kServerIP)>>::get(r);
        v.m_usMasterPort = Codec<std::remove_cvref_t<decltype(v.m_usMasterPort)>>::get(r);
        v.m_usNCUDPPort = Codec<std::remove_cvref_t<decltype(v.m_usNCUDPPort)>>::get(r);
        v.m_bEnable = Codec<std::remove_cvref_t<decltype(v.m_bEnable)>>::get(r);
        return v;
    }
};

struct KDBE_SERVERGROUP_LIST_ACK {
    std::map<std::int32_t, KServerGroupInfo> m_mapServerGroupList{};  // std::map< int, KServerGroupInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_mapServerGroupList)>>::put(w, m_mapServerGroupList);
    }

    static KDBE_SERVERGROUP_LIST_ACK read_from(Reader& r) {
        KDBE_SERVERGROUP_LIST_ACK v;
        v.m_mapServerGroupList = Codec<std::remove_cvref_t<decltype(v.m_mapServerGroupList)>>::get(r);
        return v;
    }
};

struct KECH_GET_SERVERGROUP_LIST_NOT {
    std::map<std::int32_t, KServerGroupInfo> m_mapServerGroupList{};  // std::map< int, KServerGroupInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_mapServerGroupList)>>::put(w, m_mapServerGroupList);
    }

    static KECH_GET_SERVERGROUP_LIST_NOT read_from(Reader& r) {
        KECH_GET_SERVERGROUP_LIST_NOT v;
        v.m_mapServerGroupList = Codec<std::remove_cvref_t<decltype(v.m_mapServerGroupList)>>::get(r);
        return v;
    }
};

struct KServerInfo {
    std::int32_t   m_iUID{};  // int
    std::u16string m_wstrName{};  // std::wstring
    std::int32_t   m_iServerGroupID{};  // int
    std::int32_t   m_iServerClass{};  // int
    std::u16string m_wstrPublicIP{};  // std::wstring
    std::u16string m_wstrPrivateIP{};  // std::wstring
    std::uint16_t  m_usMasterPort{};  // u_short
    std::uint16_t  m_usNCUDPPort{};  // u_short
    std::int32_t   m_nMaxUser{};  // int
    std::int32_t   m_nCurrentUser{};  // int
    bool           m_bOn{};
    char           m_cServerRollType{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iUID)>>::put(w, m_iUID);
        Codec<std::remove_cvref_t<decltype(m_wstrName)>>::put(w, m_wstrName);
        Codec<std::remove_cvref_t<decltype(m_iServerGroupID)>>::put(w, m_iServerGroupID);
        Codec<std::remove_cvref_t<decltype(m_iServerClass)>>::put(w, m_iServerClass);
        Codec<std::remove_cvref_t<decltype(m_wstrPublicIP)>>::put(w, m_wstrPublicIP);
        Codec<std::remove_cvref_t<decltype(m_wstrPrivateIP)>>::put(w, m_wstrPrivateIP);
        Codec<std::remove_cvref_t<decltype(m_usMasterPort)>>::put(w, m_usMasterPort);
        Codec<std::remove_cvref_t<decltype(m_usNCUDPPort)>>::put(w, m_usNCUDPPort);
        Codec<std::remove_cvref_t<decltype(m_nMaxUser)>>::put(w, m_nMaxUser);
        Codec<std::remove_cvref_t<decltype(m_nCurrentUser)>>::put(w, m_nCurrentUser);
        Codec<std::remove_cvref_t<decltype(m_bOn)>>::put(w, m_bOn);
        Codec<std::remove_cvref_t<decltype(m_cServerRollType)>>::put(w, m_cServerRollType);
    }

    static KServerInfo read_from(Reader& r) {
        KServerInfo v;
        v.m_iUID = Codec<std::remove_cvref_t<decltype(v.m_iUID)>>::get(r);
        v.m_wstrName = Codec<std::remove_cvref_t<decltype(v.m_wstrName)>>::get(r);
        v.m_iServerGroupID = Codec<std::remove_cvref_t<decltype(v.m_iServerGroupID)>>::get(r);
        v.m_iServerClass = Codec<std::remove_cvref_t<decltype(v.m_iServerClass)>>::get(r);
        v.m_wstrPublicIP = Codec<std::remove_cvref_t<decltype(v.m_wstrPublicIP)>>::get(r);
        v.m_wstrPrivateIP = Codec<std::remove_cvref_t<decltype(v.m_wstrPrivateIP)>>::get(r);
        v.m_usMasterPort = Codec<std::remove_cvref_t<decltype(v.m_usMasterPort)>>::get(r);
        v.m_usNCUDPPort = Codec<std::remove_cvref_t<decltype(v.m_usNCUDPPort)>>::get(r);
        v.m_nMaxUser = Codec<std::remove_cvref_t<decltype(v.m_nMaxUser)>>::get(r);
        v.m_nCurrentUser = Codec<std::remove_cvref_t<decltype(v.m_nCurrentUser)>>::get(r);
        v.m_bOn = Codec<std::remove_cvref_t<decltype(v.m_bOn)>>::get(r);
        v.m_cServerRollType = Codec<std::remove_cvref_t<decltype(v.m_cServerRollType)>>::get(r);
        return v;
    }
};

struct KDBE_GAME_SERVER_ON_ACK {
    std::int32_t                        m_iOK{};  // int
    KServerInfo                         m_kServerInfo{};
    std::map<std::int32_t, KNetAddress> m_mapCenterList{};  // std::map< int, KNetAddress >
    std::map<std::int32_t, KNetAddress> m_mapLoginList{};  // std::map< int, KNetAddress >
    std::int32_t                        m_iChannelID{};  // int
    KChannelBonusInfo                   m_kBonusInfo{};
    std::map<std::int32_t, KNetAddress> m_mapGlobalList{};  // std::map< int, KNetAddress >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_kServerInfo)>>::put(w, m_kServerInfo);
        Codec<std::remove_cvref_t<decltype(m_mapCenterList)>>::put(w, m_mapCenterList);
        Codec<std::remove_cvref_t<decltype(m_mapLoginList)>>::put(w, m_mapLoginList);
        Codec<std::remove_cvref_t<decltype(m_iChannelID)>>::put(w, m_iChannelID);
        Codec<std::remove_cvref_t<decltype(m_kBonusInfo)>>::put(w, m_kBonusInfo);
        Codec<std::remove_cvref_t<decltype(m_mapGlobalList)>>::put(w, m_mapGlobalList);
    }

    static KDBE_GAME_SERVER_ON_ACK read_from(Reader& r) {
        KDBE_GAME_SERVER_ON_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_kServerInfo = Codec<std::remove_cvref_t<decltype(v.m_kServerInfo)>>::get(r);
        v.m_mapCenterList = Codec<std::remove_cvref_t<decltype(v.m_mapCenterList)>>::get(r);
        v.m_mapLoginList = Codec<std::remove_cvref_t<decltype(v.m_mapLoginList)>>::get(r);
        v.m_iChannelID = Codec<std::remove_cvref_t<decltype(v.m_iChannelID)>>::get(r);
        v.m_kBonusInfo = Codec<std::remove_cvref_t<decltype(v.m_kBonusInfo)>>::get(r);
        v.m_mapGlobalList = Codec<std::remove_cvref_t<decltype(v.m_mapGlobalList)>>::get(r);
        return v;
    }
};

struct KDBE_LOGIN_SERVER_ON_ACK {
    std::int32_t m_iOK{};  // int
    std::int32_t m_iProxyID{};  // int
    KServerInfo  m_kServerInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_iProxyID)>>::put(w, m_iProxyID);
        Codec<std::remove_cvref_t<decltype(m_kServerInfo)>>::put(w, m_kServerInfo);
    }

    static KDBE_LOGIN_SERVER_ON_ACK read_from(Reader& r) {
        KDBE_LOGIN_SERVER_ON_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_iProxyID = Codec<std::remove_cvref_t<decltype(v.m_iProxyID)>>::get(r);
        v.m_kServerInfo = Codec<std::remove_cvref_t<decltype(v.m_kServerInfo)>>::get(r);
        return v;
    }
};

struct KServerSetData {
    std::int32_t   serverSetID{};  // int
    std::u16string serverSetName{};  // std::wstring
    char           userCountLevel{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(serverSetID)>>::put(w, serverSetID);
        Codec<std::remove_cvref_t<decltype(serverSetName)>>::put(w, serverSetName);
        Codec<std::remove_cvref_t<decltype(userCountLevel)>>::put(w, userCountLevel);
    }

    static KServerSetData read_from(Reader& r) {
        KServerSetData v;
        v.serverSetID = Codec<std::remove_cvref_t<decltype(v.serverSetID)>>::get(r);
        v.serverSetName = Codec<std::remove_cvref_t<decltype(v.serverSetName)>>::get(r);
        v.userCountLevel = Codec<std::remove_cvref_t<decltype(v.userCountLevel)>>::get(r);
        return v;
    }
};

struct KEGS_GET_SERVER_SET_DATA_ACK {
    std::int32_t                m_iOK{};  // int
    std::vector<KServerSetData> m_ServerSetList{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_ServerSetList)>>::put(w, m_ServerSetList);
    }

    static KEGS_GET_SERVER_SET_DATA_ACK read_from(Reader& r) {
        KEGS_GET_SERVER_SET_DATA_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_ServerSetList = Codec<std::remove_cvref_t<decltype(v.m_ServerSetList)>>::get(r);
        return v;
    }
};

struct KSkillData {
    std::int16_t m_iSkillID{};  // short
    std::uint8_t m_cSkillLevel{};  // UCHAR

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iSkillID)>>::put(w, m_iSkillID);
        Codec<std::remove_cvref_t<decltype(m_cSkillLevel)>>::put(w, m_cSkillLevel);
    }

    static KSkillData read_from(Reader& r) {
        KSkillData v;
        v.m_iSkillID = Codec<std::remove_cvref_t<decltype(v.m_iSkillID)>>::get(r);
        v.m_cSkillLevel = Codec<std::remove_cvref_t<decltype(v.m_cSkillLevel)>>::get(r);
        return v;
    }
};

struct KSquareCreateInfo {
    std::int32_t   m_iSquareType{};  // int
    std::u16string m_wstrSquareName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iSquareType)>>::put(w, m_iSquareType);
        Codec<std::remove_cvref_t<decltype(m_wstrSquareName)>>::put(w, m_wstrSquareName);
    }

    static KSquareCreateInfo read_from(Reader& r) {
        KSquareCreateInfo v;
        v.m_iSquareType = Codec<std::remove_cvref_t<decltype(v.m_iSquareType)>>::get(r);
        v.m_wstrSquareName = Codec<std::remove_cvref_t<decltype(v.m_wstrSquareName)>>::get(r);
        return v;
    }
};

struct KDBE_CENTER_SERVER_ON_ACK {
    std::int32_t                   m_iOK{};  // int
    KServerInfo                    m_kServerInfo{};
    std::int32_t                   m_iProxyID{};  // int
    std::vector<KSquareCreateInfo> m_vecSquareList{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_kServerInfo)>>::put(w, m_kServerInfo);
        Codec<std::remove_cvref_t<decltype(m_iProxyID)>>::put(w, m_iProxyID);
        Codec<std::remove_cvref_t<decltype(m_vecSquareList)>>::put(w, m_vecSquareList);
    }

    static KDBE_CENTER_SERVER_ON_ACK read_from(Reader& r) {
        KDBE_CENTER_SERVER_ON_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_kServerInfo = Codec<std::remove_cvref_t<decltype(v.m_kServerInfo)>>::get(r);
        v.m_iProxyID = Codec<std::remove_cvref_t<decltype(v.m_iProxyID)>>::get(r);
        v.m_vecSquareList = Codec<std::remove_cvref_t<decltype(v.m_vecSquareList)>>::get(r);
        return v;
    }
};

struct KSquareInfo {
    char           m_cSquareType{};
    std::int64_t   m_iSquareUID{};  // UidType
    std::u16string m_wstrRoomName{};  // std::wstring
    char           m_MaxSlot{};
    char           m_JoinSlot{};
    std::u16string m_RelayIP{};  // std::wstring
    std::int32_t   m_Port{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cSquareType)>>::put(w, m_cSquareType);
        Codec<std::remove_cvref_t<decltype(m_iSquareUID)>>::put(w, m_iSquareUID);
        Codec<std::remove_cvref_t<decltype(m_wstrRoomName)>>::put(w, m_wstrRoomName);
        Codec<std::remove_cvref_t<decltype(m_MaxSlot)>>::put(w, m_MaxSlot);
        Codec<std::remove_cvref_t<decltype(m_JoinSlot)>>::put(w, m_JoinSlot);
        Codec<std::remove_cvref_t<decltype(m_RelayIP)>>::put(w, m_RelayIP);
        Codec<std::remove_cvref_t<decltype(m_Port)>>::put(w, m_Port);
    }

    static KSquareInfo read_from(Reader& r) {
        KSquareInfo v;
        v.m_cSquareType = Codec<std::remove_cvref_t<decltype(v.m_cSquareType)>>::get(r);
        v.m_iSquareUID = Codec<std::remove_cvref_t<decltype(v.m_iSquareUID)>>::get(r);
        v.m_wstrRoomName = Codec<std::remove_cvref_t<decltype(v.m_wstrRoomName)>>::get(r);
        v.m_MaxSlot = Codec<std::remove_cvref_t<decltype(v.m_MaxSlot)>>::get(r);
        v.m_JoinSlot = Codec<std::remove_cvref_t<decltype(v.m_JoinSlot)>>::get(r);
        v.m_RelayIP = Codec<std::remove_cvref_t<decltype(v.m_RelayIP)>>::get(r);
        v.m_Port = Codec<std::remove_cvref_t<decltype(v.m_Port)>>::get(r);
        return v;
    }
};

struct KEGS_SQUARE_LIST_ACK {
    std::int32_t             m_iOK{};  // int
    std::uint32_t            m_nTotalPage{};  // UINT
    std::uint32_t            m_nViewPage{};  // UINT
    std::vector<KSquareInfo> m_vecSquareInfo{};  // std::vector< KSquareInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_nTotalPage)>>::put(w, m_nTotalPage);
        Codec<std::remove_cvref_t<decltype(m_nViewPage)>>::put(w, m_nViewPage);
        Codec<std::remove_cvref_t<decltype(m_vecSquareInfo)>>::put(w, m_vecSquareInfo);
    }

    static KEGS_SQUARE_LIST_ACK read_from(Reader& r) {
        KEGS_SQUARE_LIST_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_nTotalPage = Codec<std::remove_cvref_t<decltype(v.m_nTotalPage)>>::get(r);
        v.m_nViewPage = Codec<std::remove_cvref_t<decltype(v.m_nViewPage)>>::get(r);
        v.m_vecSquareInfo = Codec<std::remove_cvref_t<decltype(v.m_vecSquareInfo)>>::get(r);
        return v;
    }
};

struct KERM_SQUARE_LIST_INFO_NOT {
    std::int32_t m_iCode{};  // int
    KSquareInfo  m_kSquareInfo{};

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iCode)>>::put(w, m_iCode);
        Codec<std::remove_cvref_t<decltype(m_kSquareInfo)>>::put(w, m_kSquareInfo);
    }

    static KERM_SQUARE_LIST_INFO_NOT read_from(Reader& r) {
        KERM_SQUARE_LIST_INFO_NOT v;
        v.m_iCode = Codec<std::remove_cvref_t<decltype(v.m_iCode)>>::get(r);
        v.m_kSquareInfo = Codec<std::remove_cvref_t<decltype(v.m_kSquareInfo)>>::get(r);
        return v;
    }
};

struct KSquarePersonalShopInfo {
    char           m_cUpdateType{};
    char           m_cPersonalShopState{};
    char           m_cPersonalShopType{};
    std::int64_t   m_iPersonalShopUID{};  // UidType
    std::u16string m_wstrPersonalShopName{};  // std::wstring

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_cUpdateType)>>::put(w, m_cUpdateType);
        Codec<std::remove_cvref_t<decltype(m_cPersonalShopState)>>::put(w, m_cPersonalShopState);
        Codec<std::remove_cvref_t<decltype(m_cPersonalShopType)>>::put(w, m_cPersonalShopType);
        Codec<std::remove_cvref_t<decltype(m_iPersonalShopUID)>>::put(w, m_iPersonalShopUID);
        Codec<std::remove_cvref_t<decltype(m_wstrPersonalShopName)>>::put(w, m_wstrPersonalShopName);
    }

    static KSquarePersonalShopInfo read_from(Reader& r) {
        KSquarePersonalShopInfo v;
        v.m_cUpdateType = Codec<std::remove_cvref_t<decltype(v.m_cUpdateType)>>::get(r);
        v.m_cPersonalShopState = Codec<std::remove_cvref_t<decltype(v.m_cPersonalShopState)>>::get(r);
        v.m_cPersonalShopType = Codec<std::remove_cvref_t<decltype(v.m_cPersonalShopType)>>::get(r);
        v.m_iPersonalShopUID = Codec<std::remove_cvref_t<decltype(v.m_iPersonalShopUID)>>::get(r);
        v.m_wstrPersonalShopName = Codec<std::remove_cvref_t<decltype(v.m_wstrPersonalShopName)>>::get(r);
        return v;
    }
};

struct KSquareUserInfo {
    std::int64_t                          m_iGSUID{};  // UidType
    std::int64_t                          m_iUnitUID{};  // UidType
    std::u16string                        m_wstrNickName{};  // std::wstring
    std::u16string                        m_wstrIP{};  // std::wstring
    std::uint16_t                         m_usPort{};  // unsigned short
    char                                  m_cUnitClass{};
    std::uint8_t                          m_ucLevel{};  // UCHAR
    char                                  m_cRank{};
    bool                                  m_bIsParty{};
    char                                  m_cPersonalShopState{};
    char                                  m_cPersonalShopType{};
    std::u16string                        m_wstrPersonalShopName{};  // std::wstring
    std::vector<KInventoryItemSimpleInfo> m_vecEquippedItem{};  // std::vector< KInventoryItemSimpleInfo >
    std::int32_t                          m_iTitleID{};  // int
    std::u16string                        m_wstrGuildName{};  // std::wstring
    std::int32_t                          m_iED{};  // int
    std::int32_t                          m_iEventQuestClearCount{};  // int
    std::int32_t                          m_iExchangeCount{};  // int

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iGSUID)>>::put(w, m_iGSUID);
        Codec<std::remove_cvref_t<decltype(m_iUnitUID)>>::put(w, m_iUnitUID);
        Codec<std::remove_cvref_t<decltype(m_wstrNickName)>>::put(w, m_wstrNickName);
        Codec<std::remove_cvref_t<decltype(m_wstrIP)>>::put(w, m_wstrIP);
        Codec<std::remove_cvref_t<decltype(m_usPort)>>::put(w, m_usPort);
        Codec<std::remove_cvref_t<decltype(m_cUnitClass)>>::put(w, m_cUnitClass);
        Codec<std::remove_cvref_t<decltype(m_ucLevel)>>::put(w, m_ucLevel);
        Codec<std::remove_cvref_t<decltype(m_cRank)>>::put(w, m_cRank);
        Codec<std::remove_cvref_t<decltype(m_bIsParty)>>::put(w, m_bIsParty);
        Codec<std::remove_cvref_t<decltype(m_cPersonalShopState)>>::put(w, m_cPersonalShopState);
        Codec<std::remove_cvref_t<decltype(m_cPersonalShopType)>>::put(w, m_cPersonalShopType);
        Codec<std::remove_cvref_t<decltype(m_wstrPersonalShopName)>>::put(w, m_wstrPersonalShopName);
        Codec<std::remove_cvref_t<decltype(m_vecEquippedItem)>>::put(w, m_vecEquippedItem);
        Codec<std::remove_cvref_t<decltype(m_iTitleID)>>::put(w, m_iTitleID);
        Codec<std::remove_cvref_t<decltype(m_wstrGuildName)>>::put(w, m_wstrGuildName);
        Codec<std::remove_cvref_t<decltype(m_iED)>>::put(w, m_iED);
        Codec<std::remove_cvref_t<decltype(m_iEventQuestClearCount)>>::put(w, m_iEventQuestClearCount);
        Codec<std::remove_cvref_t<decltype(m_iExchangeCount)>>::put(w, m_iExchangeCount);
    }

    static KSquareUserInfo read_from(Reader& r) {
        KSquareUserInfo v;
        v.m_iGSUID = Codec<std::remove_cvref_t<decltype(v.m_iGSUID)>>::get(r);
        v.m_iUnitUID = Codec<std::remove_cvref_t<decltype(v.m_iUnitUID)>>::get(r);
        v.m_wstrNickName = Codec<std::remove_cvref_t<decltype(v.m_wstrNickName)>>::get(r);
        v.m_wstrIP = Codec<std::remove_cvref_t<decltype(v.m_wstrIP)>>::get(r);
        v.m_usPort = Codec<std::remove_cvref_t<decltype(v.m_usPort)>>::get(r);
        v.m_cUnitClass = Codec<std::remove_cvref_t<decltype(v.m_cUnitClass)>>::get(r);
        v.m_ucLevel = Codec<std::remove_cvref_t<decltype(v.m_ucLevel)>>::get(r);
        v.m_cRank = Codec<std::remove_cvref_t<decltype(v.m_cRank)>>::get(r);
        v.m_bIsParty = Codec<std::remove_cvref_t<decltype(v.m_bIsParty)>>::get(r);
        v.m_cPersonalShopState = Codec<std::remove_cvref_t<decltype(v.m_cPersonalShopState)>>::get(r);
        v.m_cPersonalShopType = Codec<std::remove_cvref_t<decltype(v.m_cPersonalShopType)>>::get(r);
        v.m_wstrPersonalShopName = Codec<std::remove_cvref_t<decltype(v.m_wstrPersonalShopName)>>::get(r);
        v.m_vecEquippedItem = Codec<std::remove_cvref_t<decltype(v.m_vecEquippedItem)>>::get(r);
        v.m_iTitleID = Codec<std::remove_cvref_t<decltype(v.m_iTitleID)>>::get(r);
        v.m_wstrGuildName = Codec<std::remove_cvref_t<decltype(v.m_wstrGuildName)>>::get(r);
        v.m_iED = Codec<std::remove_cvref_t<decltype(v.m_iED)>>::get(r);
        v.m_iEventQuestClearCount = Codec<std::remove_cvref_t<decltype(v.m_iEventQuestClearCount)>>::get(r);
        v.m_iExchangeCount = Codec<std::remove_cvref_t<decltype(v.m_iExchangeCount)>>::get(r);
        return v;
    }
};

struct KEGS_JOIN_SQUARE_ACK {
    std::int32_t                 m_iOK{};  // int
    KSquareInfo                  m_kSquareInfo{};
    std::vector<KSquareUserInfo> m_vecUserInfo{};  // std::vector< KSquareUserInfo >

    void write_to(Writer& w) const {
        Codec<std::remove_cvref_t<decltype(m_iOK)>>::put(w, m_iOK);
        Codec<std::remove_cvref_t<decltype(m_kSquareInfo)>>::put(w, m_kSquareInfo);
        Codec<std::remove_cvref_t<decltype(m_vecUserInfo)>>::put(w, m_vecUserInfo);
    }

    static KEGS_JOIN_SQUARE_ACK read_from(Reader& r) {
        KEGS_JOIN_SQUARE_ACK v;
        v.m_iOK = Codec<std::remove_cvref_t<decltype(v.m_iOK)>>::get(r);
        v.m_kSquareInfo = Codec<std::remove_cvref_t<decltype(v.m_kSquareInfo)>>::get(r);
        v.m_vecUserInfo = Codec<std::remove_cvref_t<decltype(v.m_vecUserInfo)>>::get(r);
        return v;
    }
};

