// kdb/Schema.h - schema bootstrap/migration for the single database file.
#pragma once

#include <string>
#include <vector>

#include "kdb/Database.h"

namespace knc::db {

// Current schema version. Bump together with a new sql/00N_*.sql migration file.
inline constexpr std::int64_t kSchemaVersion = 2;

struct MigrationResult {
    bool ok = false;
    std::int64_t fromVersion = 0;
    std::int64_t toVersion = 0;
    std::vector<std::string> appliedScripts;
    std::string error;
};

// Applies every embedded schema script (idempotent: all statements are CREATE ... IF NOT EXISTS).
MigrationResult applySchema(Database& db);

// Inserts the demo server set / channel rows the original InsertServerList.sql provided.
bool seedDefaultTopology(Database& db, const std::string& serverName, const std::string& ip,
                         unsigned short channelPort, unsigned short gamePort, unsigned short udpPort,
                         int maxUser, std::string* error = nullptr);

// Password storage. The database keeps a salted MD5 of "loginId\x1fpassword" in Accounts.PasswordHash;
// it exists so credentials are not stored in the clear, but it is NOT a production KDF and must be
// replaced before any real deployment. Both the seeder and AccountRepository use this one function.
std::string hashAccountPassword(const std::string& loginId, const std::string& password);

// Seeds the sample accounts ("test"/"test", "gm"/"gm") when the Accounts table is empty.
bool seedDefaultAccounts(Database& db, std::string* error = nullptr);

// Diagnostics used by knc_dbtool.
struct SchemaStats {
    int tables = 0;
    int indexes = 0;
    int triggers = 0;
    std::int64_t userVersion = 0;
};
SchemaStats inspectSchema(Database& db);

// Exposes the embedded SQL scripts (they are compiled into the binary so an install is one exe).
const std::vector<std::pair<std::string, std::string>>& embeddedScripts();

}  // namespace knc::db
