// kdb/Repositories.h - typed data access that replaces the *DBThread.cpp ODBC callers.
//
// The 2014 server sent DBE_* events to a pool of DB threads which each executed a stored procedure
// over ODBC. In the single process rewrite a repository is a plain object that runs the equivalent
// SQLite statement directly. Callers may still dispatch asynchronously (see kdb/DbWorker) when a
// query must not block the network thread.
#pragma once

#include <array>
#include <cstdint>
#include <memory>
#include <optional>
#include <string>
#include <vector>

#include "kcore/Types.h"
#include "kdb/Database.h"

namespace knc::db {

// ---------------------------------------------------------------------------
// Account
enum class AuthOutcome {
    kOk = 0,
    kNoSuchAccount,
    kBadPassword,
    kBlocked,
    kDeleted,
    kDatabaseError,
};

struct AccountBlockInfoMirror {
    WideString endTime;
    WideString blockReason;
    byte blockType = 0;
    WideString blockReason2;
    WideString blockEndDate;
};

// Keys of dbo.AccountOptions (AccountUID, OptKey, OptValue). The 2014 server kept the first two in
// dbo.Account columns; the rewrite keeps them as options so the schema does not grow a column per
// flag.
inline constexpr const char* kOptIsRecommend = "IsRecommend";  // KAccountInfo.m_bIsRecommend
inline constexpr const char* kOptShareBank = "ShareBank";      // P_GBankShare_SEL

struct AccountRecord {
    UidType uid = 0;  // AccountUID (the 2014 code used a 64 bit UidType for accounts too)
    std::string loginId;
    std::string displayName;
    int authLevel = 0;
    bool internalUser = false;
    bool guestUser = false;
    int age = 0;
    int sex = 0;
    std::uint32_t nexonSn = 0;
    AccountBlockInfoMirror blockInfo;
    bool blocked = false;
    std::int64_t lastLoginAt = 0;
    std::string lastLoginIp;
    std::int64_t createdAt = 0;
};

class AccountRepository {
public:
    explicit AccountRepository(Database& db) : db_(db) {}

    AuthOutcome authenticate(const std::string& loginId, const std::string& password, const std::string& ip,
                             AccountRecord* out, std::string* error = nullptr);
    AuthOutcome authenticateGuest(const std::string& machineId, const std::string& ip, AccountRecord* out,
                                  std::string* error = nullptr);
    bool create(const std::string& loginId, const std::string& password, const std::string& displayName,
                bool guest, std::int64_t* uidOut, std::string* error = nullptr);
    bool load(std::int64_t accountUid, AccountRecord* out, std::string* error = nullptr);
    bool applyBlock(std::int64_t accountUid, const std::string& endTime, const std::string& reason, int blockType,
                    const std::string& reason2, const std::string& endDate);
    bool clearBlock(std::int64_t accountUid);
    bool setOption(std::int64_t accountUid, const std::string& key, std::int64_t value);
    std::int64_t getOption(std::int64_t accountUid, const std::string& key, std::int64_t def = 0);
    bool recordLogin(std::int64_t accountUid, const std::string& loginId, const std::string& ip,
                     const std::string& machineId, const std::string& server, int result);
    std::int64_t count();

private:
    Database& db_;
};

// ---------------------------------------------------------------------------
// Units (characters)
enum class CreateUnitReason { kOk = 0, kNameTaken, kNoSlot, kInvalidClass, kDatabaseError };

struct CreateUnitResult {
    bool ok = false;
    CreateUnitReason reason = CreateUnitReason::kDatabaseError;
    UidType unitUid = 0;
    std::string message;
};

struct UnitSummary {
    UidType unitUid = 0;
    std::string nickName;
    int slot = 0;
    int unitClass = 0;
    int level = 0;
    std::int64_t ed = 0;
};

struct UnitRecord {
    UidType unitUid = 0;
    std::int64_t accountUid = 0;
    int slot = 0;
    std::string nickName;
    int unitClass = 0;
    int level = 1;
    std::int64_t exp = 0;
    std::int64_t ed = 0;
    int authLevel = 0;
    int knmSerial = 0;
    int sp = 0;
    int csp = 0;
    int maxCsp = 0;
    std::string cspEndDate;
    int straightWins = 0;
    int officialMatch = 0;
    int rating = 0;
    int maxRating = 0;
    int rPoint = 0;
    int aPoint = 0;
    int rank = 0;
    double kFactor = 0.0;
    int mapId = 0;
    int lastFieldId = 0;
    double lastPosX = 0, lastPosY = 0, lastPosZ = 0;
    std::string lastTown;
    std::int64_t createdAt = 0;
    std::int64_t lastPlayedAt = 0;
};

class UnitRepository {
public:
    static constexpr int kMaxUnitsPerAccount = 12;

    explicit UnitRepository(Database& db) : db_(db) {}

    bool listSummaries(std::int64_t accountUid, std::vector<UnitSummary>* out, std::string* error = nullptr);
    CreateUnitResult create(std::int64_t accountUid, int slot, const std::string& nickName, int unitClass,
                            int level = 1);
    bool load(UidType unitUid, UnitRecord* out, std::string* error = nullptr);
    bool save(const UnitRecord& unit, std::string* error = nullptr);
    bool remove(UidType unitUid, std::int64_t reuseDelayMs = 0);
    bool isNickNameAvailable(const std::string& nickName);
    std::int64_t countForAccount(std::int64_t accountUid);
    std::int64_t countCreatedToday(std::int64_t accountUid);

private:
    Database& db_;
};

// ---------------------------------------------------------------------------
// Inventory
struct ItemAttributeRecord {
    int index = 0;
    int type = 0;
    int value = 0;
};
struct ItemSocketRecord {
    int index = 0;
    int type = 0;
    int value = 0;
};
struct ItemRecord {
    std::int64_t itemUid = 0;
    UidType unitUid = 0;
    int itemCode = 0;
    int slotIndex = 0;
    int category = 0;
    int quantity = 1;
    int enchantLevel = 0;
    int endurance = 0;
    int maxEndurance = 0;
    std::string periodEnd;
    bool equipped = false;
    std::vector<ItemAttributeRecord> attributes;
    std::vector<ItemSocketRecord> sockets;
    // Independent original option-ID arrays; absent slots are zero.
    std::array<int, 5> itemSocketOptions{};
    std::array<int, 5> randomSocketOptions{};
};

class InventoryRepository {
public:
    explicit InventoryRepository(Database& db) : db_(db) {}

    bool listForUnit(UidType unitUid, int category, std::vector<ItemRecord>* out, std::string* error = nullptr);
    // Slot count of one bag (dbo.GInventorySize / KItemManager::GetInventorySize).
    int getInventorySize(UidType unitUid, int category);
    bool setInventorySize(UidType unitUid, int category, int size);
    bool addItem(UidType unitUid, int itemCode, int category, int slotIndex, int quantity,
                 std::int64_t* itemUidOut = nullptr);
    bool removeItem(std::int64_t itemUid);
    bool setEnchant(std::int64_t itemUid, int level);
    bool setAttribute(std::int64_t itemUid, int index, int type, int value);
    bool setSocket(std::int64_t itemUid, int index, int type, int value);
    // random=false selects the ordinary socket array; true selects the random array.
    bool setSocketOption(std::int64_t itemUid, bool random, int index, int optionId);
    bool setEquipped(UidType unitUid, std::int64_t itemUid, bool equipped);
    std::int64_t totalEd(UidType unitUid);

private:
    Database& db_;
};

// ---------------------------------------------------------------------------
// Social: friends, guilds, mail
struct FriendRecord {
    UidType unitUid = 0;
    std::string nickName;
    std::string groupName;
    std::string memo;
    bool online = false;
};
struct GuildRecord {
    std::int64_t guildUid = 0;
    std::string name;
    UidType masterUnitUid = 0;
    int level = 1;
    std::int64_t exp = 0;
    std::int64_t point = 0;
    std::string notice;
};
struct MailItemRecord {
    int itemCode = 0;
    int quantity = 1;
    int enchantLevel = 0;
};
struct MailRecord {
    std::int64_t postUid = 0;
    UidType senderUnitUid = 0;
    std::string senderName;
    std::string subject;
    std::string body;
    std::int64_t ed = 0;
    std::int64_t sentAt = 0;
    bool read = false;
    std::vector<MailItemRecord> items;
};

class SocialRepository {
public:
    explicit SocialRepository(Database& db) : db_(db) {}

    bool addFriend(UidType unitUid, UidType friendUnitUid, const std::string& group, const std::string& memo);
    bool removeFriend(UidType unitUid, UidType friendUnitUid);
    bool listFriends(UidType unitUid, std::vector<FriendRecord>* out);

    bool createGuild(const std::string& name, UidType masterUnitUid, std::int64_t* guildUidOut);
    bool joinGuild(std::int64_t guildUid, UidType unitUid, int rank = 0);
    bool loadGuildOf(UidType unitUid, GuildRecord* out);
    bool listGuildMembers(std::int64_t guildUid, std::vector<UnitSummary>* out);

    bool sendMail(UidType senderUnitUid, UidType receiverUnitUid, const std::string& subject,
                  const std::string& body, std::int64_t ed, int itemCode, int quantity,
                  std::int64_t* postUidOut = nullptr);
    bool listMail(UidType receiverUnitUid, std::vector<MailRecord>* out);
    bool markMailRead(std::int64_t postUid);
    bool deleteMail(std::int64_t postUid);

private:
    Database& db_;
};

// ---------------------------------------------------------------------------
// Server topology (channel list / server list)
struct ChannelRecord {
    std::int64_t channelUid = 0;
    int serverSetId = 0;
    int channelId = 0;
    std::string name;
    std::string ip;
    unsigned short masterPort = 0;
    unsigned short ncUdpPort = 0;
    int maxUser = 0;
    int currentUser = 0;
    int partyCount = 0;
    int playingParty = 0;
    bool open = true;
    int bonusExp = 0;
    int bonusDrop = 0;
};

struct ServerRecord {
    std::int64_t serverUid = 0;
    std::string name;
    int serverSetId = 0;
    int serverType = 0;
    std::string ip;
    unsigned short port = 0;
    unsigned short ncUdpPort = 0;
    int maxUser = 0;
    int currentUser = 0;
    bool open = true;
};

class TopologyRepository {
public:
    explicit TopologyRepository(Database& db) : db_(db) {}

    bool listChannels(int serverSetId, std::vector<ChannelRecord>* out);
    bool listServers(int serverSetId, std::vector<ServerRecord>* out);
    bool setCurrentUsers(int serverSetId, int channelId, int currentUser);
    bool setChannelPartyCounts(int serverSetId, int channelId, int parties, int playing);
    bool setChannelOpen(int serverSetId, int channelId, bool open);
    bool upsertConnectStatus(std::int64_t accountUid, UidType unitUid, int channelId, const std::string& server);
    bool removeConnectStatus(std::int64_t accountUid);
    int connectedCount(int channelId);

private:
    Database& db_;
};

// ---------------------------------------------------------------------------
// Cash shop / billing
struct CashItemRecord {
    std::int64_t cashItemId = 0;
    int itemCode = 0;
    std::string name;
    std::int64_t price = 0;
    int cashType = 0;
    int durationDays = 0;
    bool package = false;
    bool selling = true;
};

class BillingRepository {
public:
    explicit BillingRepository(Database& db) : db_(db) {}

    bool listCashItems(std::vector<CashItemRecord>* out, bool onlySelling = true);
    bool addCash(std::int64_t accountUid, std::int64_t amount, const std::string& reference,
                 std::int64_t* balanceOut = nullptr);
    bool purchase(std::int64_t accountUid, std::int64_t cashItemId, std::int64_t* newBalance = nullptr,
                  std::int64_t* cashInvUid = nullptr);
    std::int64_t balance(std::int64_t accountUid);
    bool seedSampleItems();

private:
    Database& db_;
};

// ---------------------------------------------------------------------------
// Rankings / events / world missions
class ProgressRepository {
public:
    explicit ProgressRepository(Database& db) : db_(db) {}

    bool updateRanking(int rankType, UidType unitUid, const std::string& nickName, std::int64_t value,
                       int serverSetId = 0);
    bool topRanking(int rankType, int limit, std::vector<std::pair<UidType, std::int64_t>>* out);
    bool claimEventReward(std::int64_t eventId, std::int64_t accountUid, UidType unitUid, int rewardCode);
    bool hasClaimedEventReward(std::int64_t eventId, std::int64_t accountUid, int rewardCode);
    bool incrementWorldMission(std::int64_t missionId, int scope, std::int64_t delta, std::int64_t* counterOut);
    bool recordDungeonClear(UidType unitUid, int dungeonId, int difficulty, std::int64_t clearTimeMs);
    bool log(int category, int level, const std::string& server, std::int64_t accountUid, UidType unitUid,
             const std::string& message);

private:
    Database& db_;
};

// Global accessor over a shared Database instance (the singleton by default).
struct Repositories {
    explicit Repositories(Database& db)
        : accounts(db), units(db), inventory(db), social(db), topology(db), billing(db), progress(db) {}

    AccountRepository accounts;
    UnitRepository units;
    InventoryRepository inventory;
    SocialRepository social;
    TopologyRepository topology;
    BillingRepository billing;
    ProgressRepository progress;
};

// Returns the process wide repositories bound to Database::instance().
Repositories& repositories();
// Builds a fresh set of repositories over an explicit database (used by the unit tests).
std::unique_ptr<Repositories> makeRepositories(Database& db);

}  // namespace knc::db
