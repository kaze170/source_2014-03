#include <chrono>

#include "kcore/Log.h"
#include "kdb/Repositories.h"

namespace knc::db {
namespace {

std::int64_t nowMs() {
    return std::chrono::duration_cast<std::chrono::milliseconds>(
               std::chrono::system_clock::now().time_since_epoch())
        .count();
}

UnitRecord readUnit(const Row& row) {
    UnitRecord u;
    u.unitUid = row.at("UnitUID").asInt();
    u.accountUid = row.at("AccountUID").asInt();
    u.slot = static_cast<int>(row.at("Slot").asInt());
    u.nickName = row.at("NickName").asText();
    u.unitClass = static_cast<int>(row.at("UnitClass").asInt());
    u.level = static_cast<int>(row.at("Level").asInt());
    u.exp = row.at("Exp").asInt();
    u.ed = row.at("Ed").asInt();
    u.authLevel = static_cast<int>(row.at("AuthLevel").asInt());
    u.knmSerial = static_cast<int>(row.at("KnmSerial").asInt());
    u.sp = static_cast<int>(row.at("SP").asInt());
    u.csp = static_cast<int>(row.at("CSP").asInt());
    u.maxCsp = static_cast<int>(row.at("MaxCSP").asInt());
    u.cspEndDate = row.at("CspEndDate").asText();
    u.straightWins = static_cast<int>(row.at("StraightWins").asInt());
    u.officialMatch = static_cast<int>(row.at("OfficialMatch").asInt());
    u.rating = static_cast<int>(row.at("Rating").asInt());
    u.maxRating = static_cast<int>(row.at("MaxRating").asInt());
    u.rPoint = static_cast<int>(row.at("RPoint").asInt());
    u.aPoint = static_cast<int>(row.at("APoint").asInt());
    u.rank = static_cast<int>(row.at("Rank").asInt());
    u.kFactor = row.at("KFactor").asReal();
    u.mapId = static_cast<int>(row.at("MapId").asInt());
    u.lastFieldId = static_cast<int>(row.at("LastFieldId").asInt());
    u.lastPosX = row.at("LastPosX").asReal();
    u.lastPosY = row.at("LastPosY").asReal();
    u.lastPosZ = row.at("LastPosZ").asReal();
    u.lastTown = row.at("LastTown").asText();
    u.createdAt = row.at("CreatedAt").asInt();
    u.lastPlayedAt = row.at("LastPlayedAt").asInt();
    return u;
}

}  // namespace

bool UnitRepository::listSummaries(std::int64_t accountUid, std::vector<UnitSummary>* out, std::string* error) {
    if (!out) return false;
    out->clear();
    const Rows rows = db_.query(
        "SELECT UnitUID, NickName, Slot, UnitClass, Level, Ed FROM Units WHERE AccountUID = ? AND DeletedAt = 0 "
        "ORDER BY Slot;",
        {Value(accountUid)}, error);
    for (const auto& row : rows) {
        UnitSummary summary;
        summary.unitUid = row.at("UnitUID").asInt();
        summary.nickName = row.at("NickName").asText();
        summary.slot = static_cast<int>(row.at("Slot").asInt());
        summary.unitClass = static_cast<int>(row.at("UnitClass").asInt());
        summary.level = static_cast<int>(row.at("Level").asInt());
        summary.ed = row.at("Ed").asInt();
        out->push_back(std::move(summary));
    }
    return true;
}

CreateUnitResult UnitRepository::create(std::int64_t accountUid, int slot, const std::string& nickName, int unitClass,
                                        int level) {
    CreateUnitResult result;
    if (nickName.empty()) {
        result.reason = CreateUnitReason::kInvalidClass;
        result.message = "empty nickname";
        return result;
    }
    if (!isNickNameAvailable(nickName)) {
        result.reason = CreateUnitReason::kNameTaken;
        result.message = "nickname already in use";
        return result;
    }

    const Value existing = db_.queryValue(
        "SELECT COUNT(*) FROM Units WHERE AccountUID = ? AND Slot = ? AND DeletedAt = 0;",
        {Value(accountUid), Value(slot)});
    if (existing.asInt() != 0) {
        result.reason = CreateUnitReason::kNoSlot;
        result.message = "slot occupied";
        return result;
    }

    std::string error;
    const bool inserted = db_.run(
        "INSERT INTO Units(AccountUID, Slot, NickName, UnitClass, Level, CreatedAt, LastPlayedAt) "
        "VALUES(?,?,?,?,?,?,?);",
        {Value(accountUid), Value(slot), Value(nickName), Value(unitClass), Value(level), Value(nowMs()),
         Value(nowMs())},
        &error);
    if (!inserted) {
        result.reason = CreateUnitReason::kDatabaseError;
        result.message = error;
        return result;
    }

    // Default bags, matching KItemManager's defaults for a fresh character: 80 equipment slots and
    // 60 consumable slots. Later expansion (EDInventoryExpand.lua) raises them through
    // InventoryRepository::setInventorySize.
    const UidType unitUid = db_.lastInsertId();
    for (const auto& [category, size] : {std::pair<int, int>{1, 80}, std::pair<int, int>{2, 60}}) {
        db_.run(
            "INSERT INTO InventorySizes(UnitUID, Category, Size) VALUES(?,?,?) "
            "ON CONFLICT(UnitUID, Category) DO NOTHING;",
            {Value(unitUid), Value(category), Value(size)});
    }

    // dbo.GCreateUnitCnt: keep the per-account daily counter that ERR_CREATE_UNIT_09 enforces.
    const std::int64_t today = nowMs() / 86400000;
    db_.run(
        "INSERT INTO CreateUnitCount(AccountUID, Day, Count) VALUES(?,?,1) "
        "ON CONFLICT(AccountUID) DO UPDATE SET "
        "Count = CASE WHEN Day = excluded.Day THEN Count + 1 ELSE 1 END, Day = excluded.Day;",
        {Value(accountUid), Value(today)});

    result.ok = true;
    result.reason = CreateUnitReason::kOk;
    result.unitUid = unitUid;
    return result;
}

bool UnitRepository::load(UidType unitUid, UnitRecord* out, std::string* error) {
    const Rows rows = db_.query("SELECT * FROM Units WHERE UnitUID = ? AND DeletedAt = 0;", {Value(unitUid)},
                                error);
    if (rows.empty()) return false;
    if (out) *out = readUnit(rows.front());
    return true;
}

bool UnitRepository::save(const UnitRecord& unit, std::string* error) {
    return db_.run(
        "UPDATE Units SET Slot = ?, NickName = ?, UnitClass = ?, Level = ?, Exp = ?, Ed = ?, AuthLevel = ?, "
        "KnmSerial = ?, SP = ?, CSP = ?, MaxCSP = ?, CspEndDate = ?, StraightWins = ?, OfficialMatch = ?, "
        "Rating = ?, MaxRating = ?, RPoint = ?, APoint = ?, Rank = ?, KFactor = ?, MapId = ?, LastFieldId = ?, "
        "LastPosX = ?, LastPosY = ?, LastPosZ = ?, LastTown = ?, LastPlayedAt = ? WHERE UnitUID = ?;",
        {Value(unit.slot), Value(unit.nickName), Value(unit.unitClass), Value(unit.level), Value(unit.exp),
         Value(unit.ed), Value(unit.authLevel), Value(unit.knmSerial), Value(unit.sp), Value(unit.csp),
         Value(unit.maxCsp), Value(unit.cspEndDate), Value(unit.straightWins), Value(unit.officialMatch),
         Value(unit.rating), Value(unit.maxRating), Value(unit.rPoint), Value(unit.aPoint), Value(unit.rank),
         Value(unit.kFactor), Value(unit.mapId), Value(unit.lastFieldId), Value(unit.lastPosX), Value(unit.lastPosY),
         Value(unit.lastPosZ), Value(unit.lastTown), Value(nowMs()), Value(unit.unitUid)},
        error);
}

bool UnitRepository::remove(UidType unitUid, std::int64_t reuseDelayMs) {
    const Rows rows = db_.query("SELECT NickName, AccountUID FROM Units WHERE UnitUID = ?;", {Value(unitUid)});
    if (rows.empty()) return false;
    const std::string nickName = rows.front().at("NickName").asText();
    const std::int64_t accountUid = rows.front().at("AccountUID").asInt();

    return db_.transaction([&] {
        if (!db_.run("DELETE FROM Units WHERE UnitUID = ?;", {Value(unitUid)})) return false;
        if (!db_.run(
                "INSERT OR REPLACE INTO DeletedNickNames(NickName, AccountUID, UnitUID, DeletedAt, ReusableAt) "
                "VALUES(?,?,?,?,?);",
                {Value(nickName), Value(accountUid), Value(unitUid), Value(nowMs()),
                 Value(nowMs() + reuseDelayMs)})) {
            return false;
        }
        return true;
    });
}

bool UnitRepository::isNickNameAvailable(const std::string& nickName) {
    const Value live =
        db_.queryValue("SELECT COUNT(*) FROM Units WHERE NickName = ? AND DeletedAt = 0;", {Value(nickName)});
    if (live.asInt() != 0) return false;
    const Value reserved = db_.queryValue(
        "SELECT COUNT(*) FROM DeletedNickNames WHERE NickName = ? AND ReusableAt > ?;", {Value(nickName), Value(nowMs())});
    return reserved.asInt() == 0;
}

std::int64_t UnitRepository::countForAccount(std::int64_t accountUid) {
    return db_.queryValue("SELECT COUNT(*) FROM Units WHERE AccountUID = ? AND DeletedAt = 0;", {Value(accountUid)})
        .asInt();
}

std::int64_t UnitRepository::countCreatedToday(std::int64_t accountUid) {
    const Rows rows = db_.query("SELECT Day, Count FROM CreateUnitCount WHERE AccountUID = ?;",
                                {Value(accountUid)});
    if (rows.empty()) return 0;
    const std::int64_t day = rows.front().at("Day").asInt();
    const std::int64_t today = nowMs() / 86400000;
    return day == today ? rows.front().at("Count").asInt() : 0;
}

// ---------------------------------------------------------------------------
int InventoryRepository::getInventorySize(UidType unitUid, int category) {
    const Value value = db_.queryValue(
        "SELECT Size FROM InventorySizes WHERE UnitUID = ? AND Category = ?;",
        {Value(unitUid), Value(category)}, Value(0));
    return static_cast<int>(value.asInt());
}

bool InventoryRepository::setInventorySize(UidType unitUid, int category, int size) {
    if (size < 0) return false;
    return db_.run(
        "INSERT INTO InventorySizes(UnitUID, Category, Size) VALUES(?,?,?) "
        "ON CONFLICT(UnitUID, Category) DO UPDATE SET Size = excluded.Size;",
        {Value(unitUid), Value(category), Value(size)});
}

bool InventoryRepository::listForUnit(UidType unitUid, int category, std::vector<ItemRecord>* out,
                                      std::string* error) {
    if (!out) return false;
    out->clear();
    const Rows rows = db_.query("SELECT * FROM Items WHERE UnitUID = ? AND Category = ? ORDER BY SlotIndex;",
                                {Value(unitUid), Value(category)}, error);
    for (const auto& row : rows) {
        ItemRecord item;
        item.itemUid = row.at("ItemUID").asInt();
        item.unitUid = row.at("UnitUID").asInt();
        item.itemCode = static_cast<int>(row.at("ItemCode").asInt());
        item.slotIndex = static_cast<int>(row.at("SlotIndex").asInt());
        item.category = static_cast<int>(row.at("Category").asInt());
        item.quantity = static_cast<int>(row.at("Quantity").asInt());
        item.enchantLevel = static_cast<int>(row.at("EnchantLevel").asInt());
        item.endurance = static_cast<int>(row.at("Endurance").asInt());
        item.maxEndurance = static_cast<int>(row.at("MaxEndurance").asInt());
        item.periodEnd = row.at("PeriodEnd").asText();
        item.equipped = row.at("IsEquipped").asBool();

        const Rows attrs = db_.query(
            "SELECT AttrIndex, AttrType, AttrValue FROM ItemAttributes WHERE ItemUID = ? ORDER BY AttrIndex;",
            {Value(item.itemUid)});
        for (const auto& a : attrs) {
            item.attributes.push_back(ItemAttributeRecord{static_cast<int>(a.at("AttrIndex").asInt()),
                                                          static_cast<int>(a.at("AttrType").asInt()),
                                                          static_cast<int>(a.at("AttrValue").asInt())});
        }
        const Rows sockets = db_.query(
            "SELECT SocketIndex, SocketType, SocketValue FROM ItemSockets WHERE ItemUID = ? ORDER BY SocketIndex;",
            {Value(item.itemUid)});
        for (const auto& s : sockets) {
            item.sockets.push_back(ItemSocketRecord{static_cast<int>(s.at("SocketIndex").asInt()),
                                                    static_cast<int>(s.at("SocketType").asInt()),
                                                    static_cast<int>(s.at("SocketValue").asInt())});
        }
        out->push_back(std::move(item));
    }
    return true;
}

bool InventoryRepository::addItem(UidType unitUid, int itemCode, int category, int slotIndex, int quantity,
                                  std::int64_t* itemUidOut) {
    const bool ok = db_.run(
        "INSERT INTO Items(UnitUID, ItemCode, SlotIndex, Category, Quantity, CreatedAt, UpdatedAt) "
        "VALUES(?,?,?,?,?,?,?);",
        {Value(unitUid), Value(itemCode), Value(slotIndex), Value(category), Value(quantity), Value(nowMs()),
         Value(nowMs())});
    if (ok && itemUidOut) *itemUidOut = db_.lastInsertId();
    return ok;
}

bool InventoryRepository::removeItem(std::int64_t itemUid) {
    return db_.run("DELETE FROM Items WHERE ItemUID = ?;", {Value(itemUid)});
}

bool InventoryRepository::setEnchant(std::int64_t itemUid, int level) {
    return db_.run("UPDATE Items SET EnchantLevel = ?, UpdatedAt = ? WHERE ItemUID = ?;",
                   {Value(level), Value(nowMs()), Value(itemUid)});
}

bool InventoryRepository::setAttribute(std::int64_t itemUid, int index, int type, int value) {
    return db_.run(
        "INSERT OR REPLACE INTO ItemAttributes(ItemUID, AttrIndex, AttrType, AttrValue) VALUES(?,?,?,?);",
        {Value(itemUid), Value(index), Value(type), Value(value)});
}

bool InventoryRepository::setSocket(std::int64_t itemUid, int index, int type, int value) {
    return db_.run(
        "INSERT OR REPLACE INTO ItemSockets(ItemUID, SocketIndex, SocketType, SocketValue) VALUES(?,?,?,?);",
        {Value(itemUid), Value(index), Value(type), Value(value)});
}

bool InventoryRepository::setEquipped(UidType unitUid, std::int64_t itemUid, bool equipped) {
    if (equipped) {
        if (!db_.run("UPDATE Items SET IsEquipped = 0 WHERE UnitUID = ?;", {Value(unitUid)})) return false;
    }
    return db_.run("UPDATE Items SET IsEquipped = ?, UpdatedAt = ? WHERE ItemUID = ?;",
                   {Value(equipped), Value(nowMs()), Value(itemUid)});
}

std::int64_t InventoryRepository::totalEd(UidType unitUid) {
    return db_.queryValue("SELECT Ed FROM Units WHERE UnitUID = ?;", {Value(unitUid)}).asInt();
}

}  // namespace knc::db
