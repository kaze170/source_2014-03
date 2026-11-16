// tools_src/dbtool_main.cpp - knc_dbtool: create, migrate, seed, inspect and repair data/knc.db.
//
// Replaces the fifteen-ODBC-DSN + stored-procedure maintenance chores of the 2014 deployment (and
// the InsertServerList.sql / DB_Backup scripts) with one small CLI over the same schema the server
// uses:
//
//   knc_dbtool --db data/knc.db init            create/upgrade the schema and seed the demo data
//   knc_dbtool --db data/knc.db info            schema version, table/index counts, file size
//   knc_dbtool --db data/knc.db check           PRAGMA integrity_check + foreign_key_check
//   knc_dbtool --db data/knc.db accounts        list accounts
//   knc_dbtool --db data/knc.db units [--account <id>]
//   knc_dbtool --db data/knc.db create-account <login> <password> [--name <display>]
//   knc_dbtool --db data/knc.db create-unit <login> <nickname> <class>
//   knc_dbtool --db data/knc.db block <login> <reason> | unblock <login>
//   knc_dbtool --db data/knc.db topologies        list channels and server sets
//   knc_dbtool --db data/knc.db vacuum            checkpoint + VACUUM (WAL housekeeping)
#include <cstdio>
#include <cstring>
#include <string>
#include <vector>

#include "kcore/Types.h"
#include "kdb/Database.h"
#include "kdb/Repositories.h"
#include "kdb/Schema.h"
#include "kgame/Character.h"
#include "kgame/ExpTable.h"
#include "kgame/StatTable.h"

namespace {

struct Options {
    std::string path = "data/knc.db";
    std::string command = "info";
    std::vector<std::string> args;
    std::string account;
    std::string displayName;
};

int usage() {
    std::printf(
        "knc_dbtool - inspect and maintain the KncWX2Server SQLite database\n"
        "\n"
        "  --db <path>                 database file (default data/knc.db)\n"
        "  init                        apply the schema, seed demo accounts and topology\n"
        "  info                        schema version and object counts\n"
        "  check                       integrity_check + foreign_key_check\n"
        "  accounts                    list accounts\n"
        "  units [--account <login>]   list characters\n"
        "  create-account <login> <password> [--name <display>]\n"
        "  create-unit <login> <nickname> <class>\n"
        "  block <login> <reason...>   set the account block\n"
        "  unblock <login>\n"
        "  topologies                  list channels and server sets\n"
        "  vacuum                      checkpoint and compact the database\n");
    return 0;
}

bool parse(int argc, char** argv, Options* options) {
    for (int i = 1; i < argc; ++i) {
        const std::string arg = argv[i];
        if (arg == "--help" || arg == "-h") return false;
        if (arg == "--db") {
            if (i + 1 >= argc) return false;
            options->path = argv[++i];
        } else if (arg == "--account") {
            if (i + 1 >= argc) return false;
            options->account = argv[++i];
        } else if (arg == "--name") {
            if (i + 1 >= argc) return false;
            options->displayName = argv[++i];
        } else if (!arg.empty() && arg[0] == '-') {
            std::fprintf(stderr, "unknown option '%s'\n", arg.c_str());
            return false;
        } else if (options->command == "info" && options->args.empty() && options->command == "info") {
            options->command = arg;
        } else {
            options->args.push_back(arg);
        }
    }
    return true;
}

bool openDatabase(const Options& options, knc::db::Database* db) {
    knc::db::Database::Config config;
    config.path = options.path;
    std::string error;
    if (!db->open(config, &error)) {
        std::fprintf(stderr, "cannot open %s: %s\n", options.path.c_str(), error.c_str());
        return false;
    }
    return true;
}

int commandInit(knc::db::Database& db) {
    const knc::db::MigrationResult migration = knc::db::applySchema(db);
    if (!migration.ok) {
        std::fprintf(stderr, "migration failed: %s\n", migration.error.c_str());
        return 1;
    }
    std::string error;
    if (!knc::db::seedDefaultAccounts(db, &error)) {
        std::fprintf(stderr, "seeding accounts failed: %s\n", error.c_str());
        return 1;
    }
    if (!knc::db::seedDefaultTopology(db, "KncWX2Server", "127.0.0.1", 9100, 9200, 9151, 300, &error)) {
        std::fprintf(stderr, "seeding topology failed: %s\n", error.c_str());
        return 1;
    }
    const knc::db::SchemaStats stats = knc::db::inspectSchema(db);
    std::printf("schema version %lld: %d tables, %d indexes, %d triggers\n",
                static_cast<long long>(stats.userVersion), stats.tables, stats.indexes, stats.triggers);
    return 0;
}

int commandInfo(knc::db::Database& db, const Options& options) {
    const knc::db::SchemaStats stats = knc::db::inspectSchema(db);
    std::printf("database      : %s\n", options.path.c_str());
    std::printf("schema version: %lld\n", static_cast<long long>(stats.userVersion));
    std::printf("tables        : %d\n", stats.tables);
    std::printf("indexes       : %d\n", stats.indexes);
    std::printf("triggers      : %d\n", stats.triggers);

    auto& repos = *knc::db::makeRepositories(db);
    const knc::db::Rows users = db.query("SELECT COUNT(*) AS c FROM Accounts;", {});
    const knc::db::Rows units = db.query("SELECT COUNT(*) AS c FROM Units WHERE DeletedAt = 0;", {});
    std::printf("accounts      : %lld\n", static_cast<long long>(users.empty() ? 0 : users[0].at("c").asInt()));
    std::printf("characters    : %lld\n", static_cast<long long>(units.empty() ? 0 : units[0].at("c").asInt()));
    const knc::db::Rows journal = db.query("PRAGMA journal_mode;", {});
    std::printf("journal mode  : %s\n",
                journal.empty() ? "?" : journal.front().begin()->second.asText().c_str());
    (void)repos;
    return 0;
}

int commandCheck(knc::db::Database& db) {
    std::string report;
    if (!db.integrityCheck(&report)) {
        std::printf("integrity_check: FAILED\n%s\n", report.c_str());
        return 1;
    }
    std::printf("integrity_check: ok\n");

    const knc::db::Rows violations = db.query("PRAGMA foreign_key_check;", {});
    if (!violations.empty()) {
        std::printf("foreign_key_check: %zu violation(s)\n", violations.size());
        for (const auto& row : violations) {
            std::printf("  table=%s rowid=%lld parent=%s\n", row.at("table").asText().c_str(),
                        static_cast<long long>(row.at("rowid").asInt()),
                        row.at("parent").asText().c_str());
        }
        return 1;
    }
    std::printf("foreign_key_check: ok\n");
    return 0;
}

int commandAccounts(knc::db::Database& db) {
    const knc::db::Rows rows = db.query(
        "SELECT AccountUID, LoginID, DisplayName, AuthLevel, GuestUser, BlockEndTime, BlockReason,"
        " CreatedAt FROM Accounts ORDER BY AccountUID;",
        {});
    std::printf("%-6s %-16s %-16s %-5s %-5s %s\n", "uid", "login", "name", "auth", "guest", "block");
    for (const auto& row : rows) {
        std::printf("%-6lld %-16s %-16s %-5lld %-5lld %s\n", static_cast<long long>(row.at("AccountUID").asInt()),
                    row.at("LoginID").asText().c_str(), row.at("DisplayName").asText().c_str(),
                    static_cast<long long>(row.at("AuthLevel").asInt()),
                    static_cast<long long>(row.at("GuestUser").asInt()),
                    row.at("BlockEndTime").asText().empty() ? "-" : row.at("BlockReason").asText().c_str());
    }
    return 0;
}

int commandUnits(knc::db::Database& db, const Options& options) {
    std::int64_t accountUid = 0;
    if (!options.account.empty()) {
        const knc::db::Rows rows = db.query("SELECT AccountUID FROM Accounts WHERE LoginID = ?;",
                                            {knc::db::Value(options.account)});
        if (rows.empty()) {
            std::fprintf(stderr, "no such account '%s'\n", options.account.c_str());
            return 1;
        }
        accountUid = rows.front().at("AccountUID").asInt();
    }

    const knc::db::Rows rows =
        accountUid == 0
            ? db.query("SELECT UnitUID, AccountUID, Slot, NickName, UnitClass, Level, Exp, Ed FROM Units "
                       "WHERE DeletedAt = 0 ORDER BY AccountUID, Slot;",
                       {})
            : db.query("SELECT UnitUID, AccountUID, Slot, NickName, UnitClass, Level, Exp, Ed FROM Units "
                       "WHERE DeletedAt = 0 AND AccountUID = ? ORDER BY Slot;",
                       {knc::db::Value(accountUid)});

    std::printf("%-8s %-8s %-4s %-16s %-5s %-5s %-12s %s\n", "uid", "account", "slot", "nickname",
                "class", "level", "exp", "ed");
    for (const auto& row : rows) {
        std::printf("%-8lld %-8lld %-4lld %-16s %-5lld %-5lld %-12lld %lld\n",
                    static_cast<long long>(row.at("UnitUID").asInt()),
                    static_cast<long long>(row.at("AccountUID").asInt()),
                    static_cast<long long>(row.at("Slot").asInt()), row.at("NickName").asText().c_str(),
                    static_cast<long long>(row.at("UnitClass").asInt()),
                    static_cast<long long>(row.at("Level").asInt()),
                    static_cast<long long>(row.at("Exp").asInt()),
                    static_cast<long long>(row.at("Ed").asInt()));
    }
    return 0;
}

int commandCreateAccount(knc::db::Database& db, const Options& options) {
    if (options.args.size() < 2) {
        std::fprintf(stderr, "create-account needs <login> <password>\n");
        return 2;
    }
    auto repos = knc::db::makeRepositories(db);
    std::int64_t uid = 0;
    std::string error;
    const std::string display = options.displayName.empty() ? options.args[0] : options.displayName;
    if (!repos->accounts.create(options.args[0], options.args[1], display, false, &uid, &error)) {
        std::fprintf(stderr, "cannot create account: %s\n", error.c_str());
        return 1;
    }
    std::printf("created account %s (uid %lld)\n", options.args[0].c_str(), static_cast<long long>(uid));
    return 0;
}

int commandCreateUnit(knc::db::Database& db, const Options& options) {
    if (options.args.size() < 3) {
        std::fprintf(stderr, "create-unit needs <login> <nickname> <class>\n");
        return 2;
    }
    auto repos = knc::db::makeRepositories(db);
    const knc::db::Rows rows = db.query("SELECT AccountUID FROM Accounts WHERE LoginID = ?;",
                                        {knc::db::Value(options.args[0])});
    if (rows.empty()) {
        std::fprintf(stderr, "no such account '%s'\n", options.args[0].c_str());
        return 1;
    }
    const std::int64_t accountUid = rows.front().at("AccountUID").asInt();
    const knc::db::CreateUnitResult result =
        repos->units.create(accountUid, 0, options.args[1], std::atoi(options.args[2].c_str()), 1);
    if (!result.ok) {
        std::fprintf(stderr, "cannot create unit: %s\n", result.message.c_str());
        return 1;
    }
    std::printf("created unit %s (uid %lld) for %s\n", options.args[1].c_str(),
                static_cast<long long>(result.unitUid), options.args[0].c_str());
    return 0;
}

int commandBlock(knc::db::Database& db, const Options& options, bool block) {
    if (options.args.empty()) {
        std::fprintf(stderr, "block/unblock needs <login>\n");
        return 2;
    }
    auto repos = knc::db::makeRepositories(db);
    const knc::db::Rows rows = db.query("SELECT AccountUID FROM Accounts WHERE LoginID = ?;",
                                        {knc::db::Value(options.args[0])});
    if (rows.empty()) {
        std::fprintf(stderr, "no such account '%s'\n", options.args[0].c_str());
        return 1;
    }
    const std::int64_t uid = rows.front().at("AccountUID").asInt();
    if (!block) {
        if (!repos->accounts.clearBlock(uid)) {
            std::fprintf(stderr, "cannot clear block\n");
            return 1;
        }
        std::printf("unblocked %s\n", options.args[0].c_str());
        return 0;
    }
    const std::string reason = options.args.size() > 1 ? options.args[1] : "manual block";
    // kdb treats a block as active while BlockEndTime is non-empty, so a permanent block is written
    // as the far future (the 2014 AccountBlockInfo carried the same text to the client).
    if (!repos->accounts.applyBlock(uid, "9999-12-31 23:59:59", reason, 0, "", "")) {
        std::fprintf(stderr, "cannot apply block\n");
        return 1;
    }
    std::printf("blocked %s (%s)\n", options.args[0].c_str(), reason.c_str());
    return 0;
}

int commandTopologies(knc::db::Database& db) {
    auto repos = knc::db::makeRepositories(db);
    std::vector<knc::db::ChannelRecord> channels;
    std::vector<knc::db::ServerRecord> servers;
    repos->topology.listChannels(1, &channels);
    repos->topology.listServers(1, &servers);
    std::printf("server sets / servers:\n");
    for (const auto& server : servers) {
        std::printf("  set %d server %lld %-20s %s:%u max %d %s\n", server.serverSetId,
                    static_cast<long long>(server.serverUid), server.name.c_str(), server.ip.c_str(),
                    server.port, server.maxUser, server.open ? "open" : "closed");
    }
    std::printf("channels:\n");
    for (const auto& channel : channels) {
        std::printf("  set %d channel %d %-16s %s:%u udp %u max %d %s\n", channel.serverSetId,
                    channel.channelId, channel.name.c_str(), channel.ip.c_str(), channel.masterPort,
                    channel.ncUdpPort, channel.maxUser, channel.open ? "open" : "closed");
    }
    return 0;
}

int commandVacuum(knc::db::Database& db) {
    std::string error;
    if (!db.exec("PRAGMA wal_checkpoint(TRUNCATE); VACUUM;", &error)) {
        std::fprintf(stderr, "vacuum failed: %s\n", error.c_str());
        return 1;
    }
    std::printf("database compacted\n");
    return 0;
}

}  // namespace

int main(int argc, char** argv) {
    Options options;
    if (!parse(argc, argv, &options)) return usage();

    knc::db::Database db;
    if (!openDatabase(options, &db)) return 1;

    // The command word is the first free argument, so a leading --db still works.
    const std::string command = options.command;
    if (command == "init") return commandInit(db);
    if (command == "info") return commandInfo(db, options);
    if (command == "check") return commandCheck(db);
    if (command == "accounts") return commandAccounts(db);
    if (command == "units") return commandUnits(db, options);
    if (command == "create-account") return commandCreateAccount(db, options);
    if (command == "create-unit") return commandCreateUnit(db, options);
    if (command == "block") return commandBlock(db, options, true);
    if (command == "unblock") return commandBlock(db, options, false);
    if (command == "topologies") return commandTopologies(db);
    if (command == "vacuum") return commandVacuum(db);

    std::fprintf(stderr, "unknown command '%s' (try --help)\n", command.c_str());
    return 2;
}
