#include "kdb/Schema.h"

#include <chrono>

#include "kcore/Log.h"
#include "ksec/Md5.h"

namespace knc::db {
namespace {

std::int64_t nowMs() {
    return std::chrono::duration_cast<std::chrono::milliseconds>(
               std::chrono::system_clock::now().time_since_epoch())
        .count();
}

}  // namespace

std::string hashAccountPassword(const std::string& loginId, const std::string& password) {
    // Placeholder for the production password hash. The 2014 build stored a plain password column
    // and compared it in mup_auth_user; the rewrite keeps credentials out of the clear but does not
    // pretend to implement a full KDF.
    sec::Md5 md5;
    // The separator is 0x1f so a login id can never be confused with a password prefix.
    const std::string salted = loginId + "\x1f" + password;
    md5.update(salted.data(), salted.size());
    const auto digest = md5.digest();
    static const char* kHex = "0123456789abcdef";
    std::string out;
    out.reserve(32);
    for (auto b : digest) {
        out.push_back(kHex[b >> 4]);
        out.push_back(kHex[b & 0x0F]);
    }
    return out;
}

MigrationResult applySchema(Database& db) {
    MigrationResult result;
    result.fromVersion = db.userVersion();

    for (const auto& [name, sql] : embeddedScripts()) {
        std::string error;
        if (!db.exec(sql, &error)) {
            result.error = name + ": " + error;
            KLOG(LogLevel::kError, "db") << "schema script failed: " << result.error;
            return result;
        }
        result.appliedScripts.push_back(name);
    }

    if (result.fromVersion != kSchemaVersion) {
        std::string error;
        db.run("INSERT OR REPLACE INTO SchemaInfo(Version, AppliedAt, Description) VALUES(?,?,?);",
               {Value(kSchemaVersion), Value(nowMs()), Value("schema " + std::to_string(kSchemaVersion))},
               &error);
        db.setUserVersion(kSchemaVersion);
    }

    result.toVersion = db.userVersion();
    result.ok = true;
    KLOG(LogLevel::kInfo, "db") << "schema at version " << result.toVersion << " (" << result.appliedScripts.size()
                                << " scripts)";
    return result;
}

bool seedDefaultTopology(Database& db, const std::string& serverName, const std::string& ip,
                         unsigned short channelPort, unsigned short gamePort, unsigned short udpPort, int maxUser,
                         std::string* error) {
    const bool ok = db.transaction([&] {
        db.run("INSERT OR IGNORE INTO ServerSet(ServerSetID, Name, DisplayOrder, IsOpen) VALUES(0, ?, 0, 1);",
               {Value("SOLES")});
        db.run("INSERT OR IGNORE INTO ServerSet(ServerSetID, Name, DisplayOrder, IsOpen) VALUES(1, ?, 1, 1);",
               {Value("GAIA")});

        // Server type 1 == game (see InsertServerList.sql).
        db.run(
            "INSERT OR REPLACE INTO ServerList(ServerUID, Name, ServerSetID, ServerType, Ip, Port, NcUdpPort, "
            "MaxUser, CurrentUser, DisplayOrder, IsOpen) VALUES(1, ?, 0, 1, ?, ?, ?, ?, 0, 1, 1);",
            {Value(serverName), Value(ip), Value(static_cast<std::int64_t>(gamePort)),
             Value(static_cast<std::int64_t>(udpPort)), Value(maxUser)});

        db.run(
            "INSERT OR REPLACE INTO ChannelList(ChannelUID, ServerSetID, ChannelID, Name, Ip, MasterPort, "
            "NcUdpPort, MaxUser, CurrentUser, PartyCount, PlayingParty, IsOpen) "
            "VALUES(1, 0, 1, ?, ?, ?, ?, ?, 0, 0, 0, 1);",
            {Value("Channel 1"), Value(ip), Value(static_cast<std::int64_t>(channelPort)),
             Value(static_cast<std::int64_t>(udpPort)), Value(maxUser)});
        return true;
    });
    if (!ok && error) *error = db.lastError();
    return ok;
}

bool seedDefaultAccounts(Database& db, std::string* error) {
    const Value count = db.queryValue("SELECT COUNT(*) FROM Accounts;");
    if (count.asInt() > 0) return true;

    const bool ok = db.transaction([&] {
        struct Seed {
            const char* id;
            const char* password;
            const char* name;
            int authLevel;
            int internalUser;
        };
        const Seed seeds[] = {{"test", "test", "TestPlayer", 0, 1}, {"gm", "gm", "GameMaster", 9, 1}};
        for (const auto& seed : seeds) {
            const std::string hash = hashAccountPassword(seed.id, seed.password);
            db.run(
                "INSERT INTO Accounts(LoginID, PasswordHash, DisplayName, AuthLevel, InternalUser, CreatedAt) "
                "VALUES(?,?,?,?,?,?);",
                {Value(seed.id), Value(hash), Value(seed.name), Value(seed.authLevel), Value(seed.internalUser),
                 Value(nowMs())});
        }
        return true;
    });
    if (!ok && error) *error = db.lastError();
    return ok;
}

SchemaStats inspectSchema(Database& db) {
    SchemaStats stats;
    stats.userVersion = db.userVersion();
    const Rows tables = db.query("SELECT COUNT(*) AS c FROM sqlite_master WHERE type='table';", {});
    const Rows indexes = db.query("SELECT COUNT(*) AS c FROM sqlite_master WHERE type='index';", {});
    const Rows triggers = db.query("SELECT COUNT(*) AS c FROM sqlite_master WHERE type='trigger';", {});
    if (!tables.empty()) stats.tables = static_cast<int>(tables[0].at("c").asInt());
    if (!indexes.empty()) stats.indexes = static_cast<int>(indexes[0].at("c").asInt());
    if (!triggers.empty()) stats.triggers = static_cast<int>(triggers[0].at("c").asInt());
    return stats;
}

}  // namespace knc::db
