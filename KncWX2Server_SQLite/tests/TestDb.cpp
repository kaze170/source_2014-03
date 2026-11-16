// tests/TestDb.cpp - the single SQLite database file: WAL, schema, seeds, repositories.
#include <cstdio>
#include <filesystem>
#include <string>

#include "TestFramework.h"
#include "kdb/Database.h"
#include "kdb/Repositories.h"
#include "kdb/Schema.h"

using namespace knc;
using namespace knc::db;

namespace {

struct TempDb {
    Database db;
    std::unique_ptr<Repositories> repos;
    Repositories& r() { return *repos; }
    std::string path;
    TempDb() {
        path = (std::filesystem::temp_directory_path() / "knc_test.db").string();
        std::error_code ec;
        std::filesystem::remove(path, ec);
        std::filesystem::remove(path + "-wal", ec);
        std::filesystem::remove(path + "-shm", ec);
        Database::Config cfg;
        cfg.path = path;
        std::string err;
        KCHECK(db.open(cfg, &err));
        const auto migration = applySchema(db);
        KCHECK(migration.ok);
        repos = makeRepositories(db);
    }
    ~TempDb() { db.close(); }
};

}  // namespace

KTEST(Db, walModeIsActive) {
    TempDb temp;
    const Rows rows = temp.db.query("PRAGMA journal_mode;", {});
    KCHECK(!rows.empty());
    KCHECK_EQ(rows.front().begin()->second.asText(), std::string("wal"));
    KCHECK(std::filesystem::exists(temp.path));
}

KTEST(Db, schemaIsCreatedAndVersioned) {
    TempDb temp;
    const auto stats = inspectSchema(temp.db);
    KCHECK(stats.tables >= 30);
    KCHECK(stats.indexes >= 10);
    KCHECK_EQ(stats.userVersion, kSchemaVersion);

    const auto migration = applySchema(temp.db);  // idempotent re-run
    KCHECK(migration.ok);
    KCHECK_EQ(inspectSchema(temp.db).tables, stats.tables);
    std::string report;
    KCHECK(temp.db.integrityCheck(&report));
}

KTEST(Db, seedAccountsAndTopology) {
    TempDb temp;
    std::string err;
    KCHECK(seedDefaultAccounts(temp.db, &err));
    KCHECK(seedDefaultTopology(temp.db, "Game01", "127.0.0.1", 9400, 9410, 9500, 3000, &err));

    const Value accounts = temp.db.queryValue("SELECT COUNT(*) FROM Accounts;");
    KCHECK_EQ(accounts.asInt(), std::int64_t(2));
    const Value channel = temp.db.queryValue("SELECT MasterPort FROM ChannelList WHERE ChannelID = 1;");
    KCHECK_EQ(channel.asInt(), std::int64_t(9400));
    const Value server = temp.db.queryValue("SELECT Port FROM ServerList WHERE ServerUID = 1;");
    KCHECK_EQ(server.asInt(), std::int64_t(9410));
}

KTEST(Db, accountAuthentication) {
    TempDb temp;
    KCHECK(seedDefaultAccounts(temp.db));
    auto& accounts = temp.r().accounts;

    AccountRecord out;
    AuthOutcome outcome = accounts.authenticate("test", "test", "127.0.0.1", &out);
    KCHECK(outcome == AuthOutcome::kOk);
    KCHECK_EQ(out.loginId, std::string("test"));
    KCHECK(out.uid > 0);

    outcome = accounts.authenticate("test", "wrong", "127.0.0.1", &out);
    KCHECK(outcome == AuthOutcome::kBadPassword);

    outcome = accounts.authenticate("nobody", "x", "127.0.0.1", &out);
    KCHECK(outcome == AuthOutcome::kNoSuchAccount);

    // A block ends authentication with a dedicated code that carries the reason.
    accounts.applyBlock(out.uid, "2099-01-01 00:00:00", "hacking", 1, "cheat", "2099-01-01");
    outcome = accounts.authenticate("test", "test", "127.0.0.1", &out);
    KCHECK(outcome == AuthOutcome::kBlocked);
    KCHECK(out.blockInfo.blockReason == u"hacking");
}

KTEST(Db, createAccountAndDuplicateDetection) {
    TempDb temp;
    auto& accounts = temp.r().accounts;
    std::int64_t uid = 0;
    KCHECK(accounts.create("newbie", "pw", "Newbie", false, &uid));
    KCHECK(uid > 0);
    std::int64_t again = 0;
    KCHECK(!accounts.create("newbie", "pw", "Dup", false, &again));  // UNIQUE(LoginID)
    KCHECK_EQ(accounts.count(), std::int64_t(1));
}

KTEST(Db, unitCreationAndLookup) {
    TempDb temp;
    KCHECK(seedDefaultAccounts(temp.db));
    auto& accounts = temp.r().accounts;
    auto& units = temp.r().units;

    AccountRecord account;
    KCHECK(accounts.authenticate("test", "test", "127.0.0.1", &account) == AuthOutcome::kOk);

    CreateUnitResult created = units.create(account.uid, 0, "Elsword", 0, 1);
    KCHECK(created.ok);
    KCHECK(created.unitUid > 0);

    CreateUnitResult duplicateName = units.create(account.uid, 1, "Elsword", 3, 1);
    KCHECK(!duplicateName.ok);  // UX_Units_NickName
    KCHECK(duplicateName.reason == CreateUnitReason::kNameTaken);

    std::vector<UnitSummary> list;
    KCHECK(units.listSummaries(account.uid, &list));
    KCHECK_EQ(list.size(), std::size_t(1));
    KCHECK_EQ(list[0].nickName, std::string("Elsword"));
    KCHECK_EQ(list[0].slot, 0);

    UnitRecord unit;
    KCHECK(units.load(created.unitUid, &unit));
    KCHECK_EQ(unit.level, 1);
    unit.level = 42;
    unit.ed = 123456;
    KCHECK(units.save(unit));

    UnitRecord reloaded;
    KCHECK(units.load(created.unitUid, &reloaded));
    KCHECK_EQ(reloaded.level, 42);
    KCHECK_EQ(reloaded.ed, std::int64_t(123456));

    // Deleting the unit frees the nickname but keeps it reserved until ReusableAt.
    KCHECK(units.remove(created.unitUid));
    KCHECK_EQ(units.countForAccount(account.uid), std::int64_t(0));
    const Value reserved = temp.db.queryValue("SELECT COUNT(*) FROM DeletedNickNames WHERE NickName = 'Elsword';");
    KCHECK_EQ(reserved.asInt(), std::int64_t(1));
}

KTEST(Db, inventoryRoundTrip) {
    TempDb temp;
    KCHECK(seedDefaultAccounts(temp.db));
    AccountRecord account;
    KCHECK(temp.r().accounts.authenticate("test", "test", "127.0.0.1", &account) == AuthOutcome::kOk);
    const CreateUnitResult created = temp.r().units.create(account.uid, 0, "Rena", 2, 1);
    KCHECK(created.ok);

    auto& inventory = temp.r().inventory;
    std::int64_t itemUid = 0;
    KCHECK(inventory.addItem(created.unitUid, 1001, 1, 0, 5, &itemUid));
    KCHECK(itemUid > 0);
    KCHECK(inventory.addItem(created.unitUid, 1002, 1, 0, 1));
    KCHECK(inventory.setEnchant(itemUid, 7));
    KCHECK(inventory.setAttribute(itemUid, 0, 3, 120));
    KCHECK(inventory.setSocket(itemUid, 0, 2, 55));

    std::vector<ItemRecord> items;
    KCHECK(inventory.listForUnit(created.unitUid, 1, &items));
    KCHECK_EQ(items.size(), std::size_t(2));
    KCHECK_EQ(items[0].enchantLevel, 7);
    KCHECK_EQ(items[0].quantity, 5);
    KCHECK_EQ(items[0].attributes.size(), std::size_t(1));
    KCHECK_EQ(items[0].sockets.size(), std::size_t(1));
    KCHECK_EQ(items[0].sockets[0].value, 55);

    KCHECK(inventory.removeItem(itemUid));
    KCHECK(inventory.listForUnit(created.unitUid, 1, &items));
    KCHECK_EQ(items.size(), std::size_t(1));
}

KTEST(Db, friendsAndMail) {
    TempDb temp;
    KCHECK(seedDefaultAccounts(temp.db));
    auto& accounts = temp.r().accounts;
    AccountRecord a;
    AccountRecord b{};
    KCHECK(accounts.authenticate("test", "test", "127.0.0.1", &a) == AuthOutcome::kOk);
    std::int64_t gmUid = 0;
    KCHECK(accounts.create("friend", "pw", "Friend", false, &gmUid));

    const CreateUnitResult ua = temp.r().units.create(a.uid, 0, "Aisha", 1, 1);
    const CreateUnitResult ub = temp.r().units.create(gmUid, 0, "FriendUnit", 1, 1);
    KCHECK(ua.ok && ub.ok);

    auto& social = temp.r().social;
    KCHECK(social.addFriend(ua.unitUid, ub.unitUid, "party", "hi"));
    std::vector<FriendRecord> friends;
    KCHECK(social.listFriends(ua.unitUid, &friends));
    KCHECK_EQ(friends.size(), std::size_t(1));
    KCHECK_EQ(friends[0].memo, std::string("hi"));
    KCHECK(!social.addFriend(ua.unitUid, ub.unitUid, "party", "dup"));  // primary key

    std::int64_t postUid = 0;
    KCHECK(social.sendMail(ub.unitUid, ua.unitUid, "Subject", "Body", 500, 1002, 1, &postUid));
    std::vector<MailRecord> mails;
    KCHECK(social.listMail(ua.unitUid, &mails));
    KCHECK_EQ(mails.size(), std::size_t(1));
    KCHECK_EQ(mails[0].subject, std::string("Subject"));
    KCHECK_EQ(mails[0].ed, std::int64_t(500));
    KCHECK_EQ(mails[0].items.size(), std::size_t(1));
    KCHECK(social.markMailRead(postUid));
    KCHECK(social.deleteMail(postUid));
    KCHECK(social.listMail(ua.unitUid, &mails));
    KCHECK_EQ(mails.size(), std::size_t(0));
}

KTEST(Db, channelsAndServerList) {
    TempDb temp;
    std::string err;
    KCHECK(seedDefaultTopology(temp.db, "Game01", "127.0.0.1", 9400, 9410, 9500, 3000, &err));
    auto& topology = temp.r().topology;
    std::vector<ChannelRecord> channels;
    KCHECK(topology.listChannels(0, &channels));
    KCHECK_EQ(channels.size(), std::size_t(1));
    KCHECK_EQ(channels[0].masterPort, 9400);
    KCHECK_EQ(channels[0].name, std::string("Channel 1"));

    KCHECK(topology.setCurrentUsers(0, 1, 137));
    KCHECK(topology.listChannels(0, &channels));
    KCHECK_EQ(channels[0].currentUser, 137);
}

KTEST(Db, transactionsRollBack) {
    TempDb temp;
    KCHECK(seedDefaultAccounts(temp.db));
    const bool ok = temp.db.transaction([] { return false; });  // abort
    KCHECK(!ok);
    const Value count = temp.db.queryValue("SELECT COUNT(*) FROM Accounts;");
    KCHECK_EQ(count.asInt(), std::int64_t(2));

    const bool ok2 = temp.db.transaction([] { return true; });
    KCHECK(ok2);
}
