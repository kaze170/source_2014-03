#include <chrono>

#include "kcore/Log.h"
#include "kdb/Repositories.h"
#include "kdb/Schema.h"

namespace knc::db {
namespace {

std::int64_t nowMs() {
    return std::chrono::duration_cast<std::chrono::milliseconds>(
               std::chrono::system_clock::now().time_since_epoch())
        .count();
}

AccountRecord readAccount(const Row& row) {
    AccountRecord record;
    record.uid = row.at("AccountUID").asInt();
    record.loginId = row.at("LoginID").asText();
    record.displayName = row.at("DisplayName").asText();
    record.authLevel = static_cast<int>(row.at("AuthLevel").asInt());
    record.internalUser = row.at("InternalUser").asBool();
    record.guestUser = row.at("GuestUser").asBool();
    record.age = static_cast<int>(row.at("Age").asInt());
    record.sex = static_cast<int>(row.at("Sex").asInt());
    record.nexonSn = static_cast<std::uint32_t>(row.at("NexonSN").asInt());
    record.blockInfo.endTime = toWide(row.at("BlockEndTime").asText());
    record.blockInfo.blockReason = toWide(row.at("BlockReason").asText());
    record.blockInfo.blockType = static_cast<byte>(row.at("BlockType").asInt());
    record.blockInfo.blockReason2 = toWide(row.at("BlockReason2").asText());
    record.blockInfo.blockEndDate = toWide(row.at("BlockEndDate").asText());
    record.blocked = !record.blockInfo.endTime.empty();
    record.lastLoginAt = row.at("LastLoginAt").asInt();
    record.lastLoginIp = row.at("LastLoginIp").asText();
    record.createdAt = row.at("CreatedAt").asInt();
    return record;
}

}  // namespace

AuthOutcome AccountRepository::authenticate(const std::string& loginId, const std::string& password,
                                            const std::string& ip, AccountRecord* out, std::string* error) {
    const Rows rows = db_.query(
        "SELECT * FROM Accounts WHERE LoginID = ? AND IsDeleted = 0;", {Value(loginId)}, error);
    if (rows.empty()) return AuthOutcome::kNoSuchAccount;

    const Row& row = rows.front();
    if (row.at("PasswordHash").asText() != hashAccountPassword(loginId, password)) return AuthOutcome::kBadPassword;

    AccountRecord record = readAccount(row);
    if (record.blocked) {
        if (out) *out = record;
        return AuthOutcome::kBlocked;
    }

    // Successful login: stamp the account and append to the audit trail.
    db_.run("UPDATE Accounts SET LastLoginAt = ?, LastLoginIp = ? WHERE AccountUID = ?;",
            {Value(nowMs()), Value(ip), Value(record.uid)});
    record.lastLoginAt = nowMs();
    record.lastLoginIp = ip;
    if (out) *out = record;
    return AuthOutcome::kOk;
}

AuthOutcome AccountRepository::authenticateGuest(const std::string& machineId, const std::string& ip,
                                                 AccountRecord* out, std::string* error) {
    // Guest accounts are keyed by the machine id, matching the 2014 "guest user" flag.
    const std::string loginId = "guest_" + machineId.substr(0, 16);
    const Rows rows = db_.query("SELECT * FROM Accounts WHERE LoginID = ? AND IsDeleted = 0;", {Value(loginId)},
                                error);
    if (!rows.empty()) {
        AccountRecord record = readAccount(rows.front());
        if (record.blocked) {
            if (out) *out = record;
            return AuthOutcome::kBlocked;
        }
        db_.run("UPDATE Accounts SET LastLoginAt = ?, LastLoginIp = ? WHERE AccountUID = ?;",
                {Value(nowMs()), Value(ip), Value(record.uid)});
        record.lastLoginAt = nowMs();
        record.lastLoginIp = ip;
        if (out) *out = record;
        return AuthOutcome::kOk;
    }

    std::int64_t uid = 0;
    if (!create(loginId, machineId, "Guest", true, &uid, error)) return AuthOutcome::kDatabaseError;
    return load(uid, out, error) ? AuthOutcome::kOk : AuthOutcome::kDatabaseError;
}

bool AccountRepository::create(const std::string& loginId, const std::string& password,
                               const std::string& displayName, bool guest, std::int64_t* uidOut,
                               std::string* error) {
    std::string err;
    const bool ok = db_.run(
        "INSERT INTO Accounts(LoginID, PasswordHash, DisplayName, GuestUser, CreatedAt) VALUES(?,?,?,?,?);",
        {Value(loginId), Value(hashAccountPassword(loginId, password)), Value(displayName), Value(guest), Value(nowMs())},
        &err);
    if (!ok) {
        if (error) *error = err;
        return false;
    }
    if (uidOut) *uidOut = db_.lastInsertId();
    return true;
}

bool AccountRepository::load(std::int64_t accountUid, AccountRecord* out, std::string* error) {
    const Rows rows = db_.query("SELECT * FROM Accounts WHERE AccountUID = ?;", {Value(accountUid)}, error);
    if (rows.empty()) return false;
    if (out) *out = readAccount(rows.front());
    return true;
}

bool AccountRepository::applyBlock(std::int64_t accountUid, const std::string& endTime, const std::string& reason,
                                   int blockType, const std::string& reason2, const std::string& endDate) {
    return db_.run(
        "UPDATE Accounts SET BlockEndTime = ?, BlockReason = ?, BlockType = ?, BlockReason2 = ?, BlockEndDate = ? "
        "WHERE AccountUID = ?;",
        {Value(endTime), Value(reason), Value(blockType), Value(reason2), Value(endDate), Value(accountUid)});
}

bool AccountRepository::clearBlock(std::int64_t accountUid) {
    return db_.run(
        "UPDATE Accounts SET BlockEndTime = '', BlockReason = '', BlockType = 0, BlockReason2 = '', "
        "BlockEndDate = '' WHERE AccountUID = ?;",
        {Value(accountUid)});
}

bool AccountRepository::setOption(std::int64_t accountUid, const std::string& key, std::int64_t value) {
    return db_.run("INSERT OR REPLACE INTO AccountOptions(AccountUID, OptKey, OptValue) VALUES(?,?,?);",
                   {Value(accountUid), Value(key), Value(value)});
}

std::int64_t AccountRepository::getOption(std::int64_t accountUid, const std::string& key, std::int64_t def) {
    const Value value = db_.queryValue("SELECT OptValue FROM AccountOptions WHERE AccountUID = ? AND OptKey = ?;",
                                       {Value(accountUid), Value(key)}, Value(def));
    return value.asInt(def);
}

bool AccountRepository::recordLogin(std::int64_t accountUid, const std::string& loginId, const std::string& ip,
                                    const std::string& machineId, const std::string& server, int result) {
    return db_.run(
        "INSERT INTO LoginHistory(AccountUID, LoginID, Ip, MachineId, ServerName, ResultCode, At) "
        "VALUES(?,?,?,?,?,?,?);",
        {Value(accountUid), Value(loginId), Value(ip), Value(machineId), Value(server), Value(result),
         Value(nowMs())});
}

std::int64_t AccountRepository::count() {
    return db_.queryValue("SELECT COUNT(*) FROM Accounts;").asInt();
}

}  // namespace knc::db
