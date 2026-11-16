#include <chrono>

#include "kdb/Repositories.h"

namespace knc::db {
namespace {

std::int64_t nowMs() {
    return std::chrono::duration_cast<std::chrono::milliseconds>(
               std::chrono::system_clock::now().time_since_epoch())
        .count();
}

}  // namespace

bool TopologyRepository::listChannels(int serverSetId, std::vector<ChannelRecord>* out) {
    if (!out) return false;
    out->clear();
    const Rows rows = db_.query(
        "SELECT * FROM ChannelList WHERE ServerSetID = ? AND IsOpen = 1 ORDER BY ChannelID;", {Value(serverSetId)});
    for (const auto& row : rows) {
        ChannelRecord record;
        record.channelUid = row.at("ChannelUID").asInt();
        record.serverSetId = static_cast<int>(row.at("ServerSetID").asInt());
        record.channelId = static_cast<int>(row.at("ChannelID").asInt());
        record.name = row.at("Name").asText();
        record.ip = row.at("Ip").asText();
        record.masterPort = static_cast<unsigned short>(row.at("MasterPort").asInt());
        record.ncUdpPort = static_cast<unsigned short>(row.at("NcUdpPort").asInt());
        record.maxUser = static_cast<int>(row.at("MaxUser").asInt());
        record.currentUser = static_cast<int>(row.at("CurrentUser").asInt());
        record.partyCount = static_cast<int>(row.at("PartyCount").asInt());
        record.playingParty = static_cast<int>(row.at("PlayingParty").asInt());
        record.open = row.at("IsOpen").asBool();
        record.bonusExp = static_cast<int>(row.at("BonusExp").asInt());
        record.bonusDrop = static_cast<int>(row.at("BonusDrop").asInt());
        out->push_back(std::move(record));
    }
    return true;
}

bool TopologyRepository::listServers(int serverSetId, std::vector<ServerRecord>* out) {
    if (!out) return false;
    out->clear();
    const Rows rows = db_.query("SELECT * FROM ServerList WHERE ServerSetID = ? ORDER BY DisplayOrder;",
                                {Value(serverSetId)});
    for (const auto& row : rows) {
        ServerRecord record;
        record.serverUid = row.at("ServerUID").asInt();
        record.name = row.at("Name").asText();
        record.serverSetId = static_cast<int>(row.at("ServerSetID").asInt());
        record.serverType = static_cast<int>(row.at("ServerType").asInt());
        record.ip = row.at("Ip").asText();
        record.port = static_cast<unsigned short>(row.at("Port").asInt());
        record.ncUdpPort = static_cast<unsigned short>(row.at("NcUdpPort").asInt());
        record.maxUser = static_cast<int>(row.at("MaxUser").asInt());
        record.currentUser = static_cast<int>(row.at("CurrentUser").asInt());
        record.open = row.at("IsOpen").asBool();
        out->push_back(std::move(record));
    }
    return true;
}

bool TopologyRepository::setCurrentUsers(int serverSetId, int channelId, int currentUser) {
    return db_.run("UPDATE ChannelList SET CurrentUser = ? WHERE ServerSetID = ? AND ChannelID = ?;",
                   {Value(currentUser), Value(serverSetId), Value(channelId)});
}

bool TopologyRepository::setChannelPartyCounts(int serverSetId, int channelId, int parties, int playing) {
    return db_.run("UPDATE ChannelList SET PartyCount = ?, PlayingParty = ? WHERE ServerSetID = ? AND ChannelID = ?;",
                   {Value(parties), Value(playing), Value(serverSetId), Value(channelId)});
}

bool TopologyRepository::setChannelOpen(int serverSetId, int channelId, bool open) {
    return db_.run("UPDATE ChannelList SET IsOpen = ? WHERE ServerSetID = ? AND ChannelID = ?;",
                   {Value(open), Value(serverSetId), Value(channelId)});
}

bool TopologyRepository::upsertConnectStatus(std::int64_t accountUid, UidType unitUid, int channelId,
                                             const std::string& server) {
    return db_.run(
        "INSERT INTO ConnectStatus(AccountUID, UnitUID, ChannelID, ServerName, ConnectedAt, UpdatedAt) "
        "VALUES(?,?,?,?,?,?) ON CONFLICT(AccountUID) DO UPDATE SET UnitUID = excluded.UnitUID, "
        "ChannelID = excluded.ChannelID, ServerName = excluded.ServerName, UpdatedAt = excluded.UpdatedAt;",
        {Value(accountUid), Value(unitUid), Value(channelId), Value(server), Value(nowMs()), Value(nowMs())});
}

bool TopologyRepository::removeConnectStatus(std::int64_t accountUid) {
    return db_.run("DELETE FROM ConnectStatus WHERE AccountUID = ?;", {Value(accountUid)});
}

int TopologyRepository::connectedCount(int channelId) {
    return static_cast<int>(
        db_.queryValue("SELECT COUNT(*) FROM ConnectStatus WHERE ChannelID = ?;", {Value(channelId)}).asInt());
}

// ---------------------------------------------------------------------------
bool BillingRepository::listCashItems(std::vector<CashItemRecord>* out, bool onlySelling) {
    if (!out) return false;
    out->clear();
    const Rows rows = db_.query(
        onlySelling ? "SELECT * FROM CashItems WHERE IsSelling = 1 ORDER BY DisplayOrder;"
                    : "SELECT * FROM CashItems ORDER BY DisplayOrder;",
        {});
    for (const auto& row : rows) {
        CashItemRecord record;
        record.cashItemId = row.at("CashItemId").asInt();
        record.itemCode = static_cast<int>(row.at("ItemCode").asInt());
        record.name = row.at("Name").asText();
        record.price = row.at("Price").asInt();
        record.cashType = static_cast<int>(row.at("CashType").asInt());
        record.durationDays = static_cast<int>(row.at("DurationDays").asInt());
        record.package = row.at("IsPackage").asBool();
        record.selling = row.at("IsSelling").asBool();
        out->push_back(std::move(record));
    }
    return true;
}

bool BillingRepository::addCash(std::int64_t accountUid, std::int64_t amount, const std::string& reference,
                                std::int64_t* balanceOut) {
    return db_.transaction([&] {
        if (!db_.run("INSERT OR IGNORE INTO BillingAccounts(AccountUID, CashBalance, UpdatedAt) VALUES(?,0,?);",
                     {Value(accountUid), Value(nowMs())})) {
            return false;
        }
        if (!db_.run("UPDATE BillingAccounts SET CashBalance = CashBalance + ?, UpdatedAt = ? WHERE AccountUID = ?;",
                     {Value(amount), Value(nowMs()), Value(accountUid)})) {
            return false;
        }
        const std::int64_t balance =
            db_.queryValue("SELECT CashBalance FROM BillingAccounts WHERE AccountUID = ?;", {Value(accountUid)}).asInt();
        if (!db_.run(
                "INSERT INTO BillingTransactions(AccountUID, TxType, Amount, BalanceAfter, Reference, At, Success) "
                "VALUES(?,?,?,?,?,?,1);",
                {Value(accountUid), Value(amount >= 0 ? 0 : 2), Value(amount), Value(balance), Value(reference),
                 Value(nowMs())})) {
            return false;
        }
        if (balanceOut) *balanceOut = balance;
        return true;
    });
}

bool BillingRepository::purchase(std::int64_t accountUid, std::int64_t cashItemId, std::int64_t* newBalance,
                                 std::int64_t* cashInvUid) {
    return db_.transaction([&] {
        const Rows items = db_.query("SELECT ItemCode, Price, DurationDays FROM CashItems WHERE CashItemId = ?;",
                                     {Value(cashItemId)});
        if (items.empty()) return false;
        const Row& item = items.front();
        const std::int64_t price = item.at("Price").asInt();
        const std::int64_t balance =
            db_.queryValue("SELECT CashBalance FROM BillingAccounts WHERE AccountUID = ?;", {Value(accountUid)}).asInt();
        if (balance < price) return false;

        if (!db_.run("UPDATE BillingAccounts SET CashBalance = CashBalance - ?, UpdatedAt = ? WHERE AccountUID = ?;",
                     {Value(price), Value(nowMs()), Value(accountUid)})) {
            return false;
        }
        const std::int64_t durationMs = item.at("DurationDays").asInt() * 24LL * 3600 * 1000;
        if (!db_.run(
                "INSERT INTO CashInventory(AccountUID, CashItemId, ItemCode, PurchasedAt, ExpiresAt) VALUES(?,?,?,?,?);",
                {Value(accountUid), Value(cashItemId), item.at("ItemCode").asInt(), Value(nowMs()),
                 Value(durationMs > 0 ? nowMs() + durationMs : 0)})) {
            return false;
        }
        const std::int64_t invUid = db_.lastInsertId();
        const std::int64_t after = balance - price;
        if (!db_.run(
                "INSERT INTO BillingTransactions(AccountUID, TxType, Amount, BalanceAfter, Reference, At, Success) "
                "VALUES(?,1,?,?,?,?,1);",
                {Value(accountUid), Value(-price), Value(after), Value("cashitem:" + std::to_string(cashItemId)),
                 Value(nowMs())})) {
            return false;
        }
        if (newBalance) *newBalance = after;
        if (cashInvUid) *cashInvUid = invUid;
        return true;
    });
}

std::int64_t BillingRepository::balance(std::int64_t accountUid) {
    return db_.queryValue("SELECT CashBalance FROM BillingAccounts WHERE AccountUID = ?;", {Value(accountUid)}).asInt();
}

bool BillingRepository::seedSampleItems() {
    return db_.transaction([&] {
        const struct {
            std::int64_t id;
            int itemCode;
            const char* name;
            std::int64_t price;
            int order;
        } samples[] = {
            {1, 1001, "El Resonance Potion", 100, 1},
            {2, 1002, "Resurrection Stone x5", 250, 2},
            {3, 1003, "Inventory Expansion (8)", 400, 3},
        };
        for (const auto& sample : samples) {
            if (!db_.run(
                    "INSERT OR REPLACE INTO CashItems(CashItemId, ItemCode, Name, Price, CashType, DurationDays, "
                    "IsPackage, IsSelling, DisplayOrder) VALUES(?,?,?,?,0,0,0,1,?);",
                    {Value(sample.id), Value(sample.itemCode), Value(sample.name), Value(sample.price),
                     Value(sample.order)})) {
                return false;
            }
        }
        return true;
    });
}

// ---------------------------------------------------------------------------
bool ProgressRepository::updateRanking(int rankType, UidType unitUid, const std::string& nickName,
                                       std::int64_t value, int serverSetId) {
    return db_.run(
        "INSERT INTO Rankings(RankType, UnitUID, NickName, Value, ServerSetID, UpdatedAt) VALUES(?,?,?,?,?,?) "
        "ON CONFLICT(RankType, UnitUID) DO UPDATE SET NickName = excluded.NickName, Value = excluded.Value, "
        "UpdatedAt = excluded.UpdatedAt;",
        {Value(rankType), Value(unitUid), Value(nickName), Value(value), Value(serverSetId), Value(nowMs())});
}

bool ProgressRepository::topRanking(int rankType, int limit, std::vector<std::pair<UidType, std::int64_t>>* out) {
    if (!out) return false;
    out->clear();
    const Rows rows = db_.query(
        "SELECT UnitUID, Value FROM Rankings WHERE RankType = ? ORDER BY Value DESC, UnitUID LIMIT ?;",
        {Value(rankType), Value(limit)});
    for (const auto& row : rows) out->emplace_back(row.at("UnitUID").asInt(), row.at("Value").asInt());
    return true;
}

bool ProgressRepository::claimEventReward(std::int64_t eventId, std::int64_t accountUid, UidType unitUid,
                                          int rewardCode) {
    return db_.run(
        "INSERT OR IGNORE INTO EventRewards(EventId, AccountUID, UnitUID, RewardCode, ClaimedAt) VALUES(?,?,?,?,?);",
        {Value(eventId), Value(accountUid), Value(unitUid), Value(rewardCode), Value(nowMs())}) &&
           db_.changes() > 0;
}

bool ProgressRepository::hasClaimedEventReward(std::int64_t eventId, std::int64_t accountUid, int rewardCode) {
    return db_.queryValue("SELECT COUNT(*) FROM EventRewards WHERE EventId = ? AND AccountUID = ? AND RewardCode = ?;",
                          {Value(eventId), Value(accountUid), Value(rewardCode)})
               .asInt() > 0;
}

bool ProgressRepository::incrementWorldMission(std::int64_t missionId, int scope, std::int64_t delta,
                                              std::int64_t* counterOut) {
    return db_.transaction([&] {
        if (!db_.run(
                "INSERT INTO WorldMissionProgress(MissionId, Scope, Counter, UpdatedAt) VALUES(?,?,0,?) "
                "ON CONFLICT(MissionId, Scope) DO NOTHING;",
                {Value(missionId), Value(scope), Value(nowMs())})) {
            return false;
        }
        if (!db_.run("UPDATE WorldMissionProgress SET Counter = Counter + ?, UpdatedAt = ? "
                     "WHERE MissionId = ? AND Scope = ?;",
                     {Value(delta), Value(nowMs()), Value(missionId), Value(scope)})) {
            return false;
        }
        const std::int64_t counter = db_.queryValue(
            "SELECT Counter FROM WorldMissionProgress WHERE MissionId = ? AND Scope = ?;",
            {Value(missionId), Value(scope)}).asInt();
        if (counterOut) *counterOut = counter;
        return true;
    });
}

bool ProgressRepository::recordDungeonClear(UidType unitUid, int dungeonId, int difficulty,
                                            std::int64_t clearTimeMs) {
    return db_.run(
        "INSERT INTO DungeonClears(UnitUID, DungeonId, Difficulty, ClearCount, BestTimeMs, LastClearedAt) "
        "VALUES(?,?,?,1,?,?) ON CONFLICT(UnitUID, DungeonId, Difficulty) DO UPDATE SET "
        "ClearCount = ClearCount + 1, "
        "BestTimeMs = CASE WHEN BestTimeMs = 0 OR excluded.BestTimeMs < BestTimeMs THEN excluded.BestTimeMs "
        "ELSE BestTimeMs END, LastClearedAt = excluded.LastClearedAt;",
        {Value(unitUid), Value(dungeonId), Value(difficulty), Value(clearTimeMs), Value(nowMs())});
}

bool ProgressRepository::log(int category, int level, const std::string& server, std::int64_t accountUid,
                            UidType unitUid, const std::string& message) {
    return db_.run(
        "INSERT INTO LogEntries(Category, Level, ServerName, AccountUID, UnitUID, Message, At) VALUES(?,?,?,?,?,?,?);",
        {Value(category), Value(level), Value(server), Value(accountUid), Value(unitUid), Value(message),
         Value(nowMs())});
}

// ---------------------------------------------------------------------------
Repositories& repositories() {
    static Repositories instance(Database::instance());
    return instance;
}

std::unique_ptr<Repositories> makeRepositories(Database& db) { return std::make_unique<Repositories>(db); }

}  // namespace knc::db
