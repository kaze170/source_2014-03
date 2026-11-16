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

bool SocialRepository::addFriend(UidType unitUid, UidType friendUnitUid, const std::string& group,
                                 const std::string& memo) {
    if (unitUid == friendUnitUid) return false;
    return db_.run("INSERT OR IGNORE INTO Friends(UnitUID, FriendUnitUID, GroupName, Memo, CreatedAt) "
                   "VALUES(?,?,?,?,?);",
                   {Value(unitUid), Value(friendUnitUid), Value(group), Value(memo), Value(nowMs())}) &&
           db_.changes() > 0;
}

bool SocialRepository::removeFriend(UidType unitUid, UidType friendUnitUid) {
    return db_.run("DELETE FROM Friends WHERE UnitUID = ? AND FriendUnitUID = ?;",
                   {Value(unitUid), Value(friendUnitUid)});
}

bool SocialRepository::listFriends(UidType unitUid, std::vector<FriendRecord>* out) {
    if (!out) return false;
    out->clear();
    const Rows rows = db_.query(
        "SELECT f.FriendUnitUID, f.GroupName, f.Memo, COALESCE(u.NickName, '') AS NickName "
        "FROM Friends f LEFT JOIN Units u ON u.UnitUID = f.FriendUnitUID WHERE f.UnitUID = ?;",
        {Value(unitUid)});
    for (const auto& row : rows) {
        FriendRecord record;
        record.unitUid = row.at("FriendUnitUID").asInt();
        record.groupName = row.at("GroupName").asText();
        record.memo = row.at("Memo").asText();
        record.nickName = row.at("NickName").asText();
        out->push_back(std::move(record));
    }
    return true;
}

bool SocialRepository::createGuild(const std::string& name, UidType masterUnitUid, std::int64_t* guildUidOut) {
    const bool ok = db_.transaction([&] {
        if (!db_.run("INSERT INTO Guilds(Name, MasterUnitUid, CreatedAt) VALUES(?,?,?);",
                     {Value(name), Value(masterUnitUid), Value(nowMs())})) {
            return false;
        }
        const std::int64_t guildUid = db_.lastInsertId();
        if (guildUidOut) *guildUidOut = guildUid;
        return db_.run("INSERT INTO GuildMembers(GuildUID, UnitUID, Rank, JoinedAt) VALUES(?,?,0,?);",
                       {Value(guildUid), Value(masterUnitUid), Value(nowMs())});
    });
    return ok;
}

bool SocialRepository::joinGuild(std::int64_t guildUid, UidType unitUid, int rank) {
    return db_.run("INSERT OR REPLACE INTO GuildMembers(GuildUID, UnitUID, Rank, JoinedAt) VALUES(?,?,?,?);",
                   {Value(guildUid), Value(unitUid), Value(rank), Value(nowMs())});
}

bool SocialRepository::loadGuildOf(UidType unitUid, GuildRecord* out) {
    const Rows rows = db_.query(
        "SELECT g.* FROM Guilds g JOIN GuildMembers m ON m.GuildUID = g.GuildUID "
        "WHERE m.UnitUID = ? AND g.IsDeleted = 0;",
        {Value(unitUid)});
    if (rows.empty()) return false;
    if (out) {
        const Row& row = rows.front();
        out->guildUid = row.at("GuildUID").asInt();
        out->name = row.at("Name").asText();
        out->masterUnitUid = row.at("MasterUnitUid").asInt();
        out->level = static_cast<int>(row.at("Level").asInt());
        out->exp = row.at("Exp").asInt();
        out->point = row.at("Point").asInt();
        out->notice = row.at("Notice").asText();
    }
    return true;
}

bool SocialRepository::listGuildMembers(std::int64_t guildUid, std::vector<UnitSummary>* out) {
    if (!out) return false;
    out->clear();
    const Rows rows = db_.query(
        "SELECT u.UnitUID, u.NickName, u.Slot, u.UnitClass, u.Level, u.Ed FROM GuildMembers m "
        "JOIN Units u ON u.UnitUID = m.UnitUID WHERE m.GuildUID = ? ORDER BY m.Rank;",
        {Value(guildUid)});
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

bool SocialRepository::sendMail(UidType senderUnitUid, UidType receiverUnitUid, const std::string& subject,
                                const std::string& body, std::int64_t ed, int itemCode, int quantity,
                                std::int64_t* postUidOut) {
    return db_.transaction([&] {
        const Value senderName = db_.queryValue("SELECT NickName FROM Units WHERE UnitUID = ?;",
                                                {Value(senderUnitUid)});
        const Value receiverName = db_.queryValue("SELECT NickName FROM Units WHERE UnitUID = ?;",
                                                  {Value(receiverUnitUid)});
        if (!db_.run(
                "INSERT INTO Posts(SenderUnitUID, SenderName, ReceiverUnitUID, ReceiverName, Subject, Body, Ed, "
                "SentAt, ExpiresAt) VALUES(?,?,?,?,?,?,?,?,?);",
                {Value(senderUnitUid), Value(senderName.asText()), Value(receiverUnitUid),
                 Value(receiverName.asText()), Value(subject), Value(body), Value(ed), Value(nowMs()),
                 Value(nowMs() + 30LL * 24 * 3600 * 1000)})) {
            return false;
        }
        const std::int64_t postUid = db_.lastInsertId();
        if (postUidOut) *postUidOut = postUid;
        if (itemCode > 0) {
            if (!db_.run("INSERT INTO PostItems(PostUID, ItemCode, Quantity) VALUES(?,?,?);",
                         {Value(postUid), Value(itemCode), Value(quantity)})) {
                return false;
            }
        }
        return true;
    });
}

bool SocialRepository::listMail(UidType receiverUnitUid, std::vector<MailRecord>* out) {
    if (!out) return false;
    out->clear();
    const Rows rows = db_.query(
        "SELECT * FROM Posts WHERE ReceiverUnitUID = ? AND IsDeleted = 0 ORDER BY SentAt DESC;",
        {Value(receiverUnitUid)});
    for (const auto& row : rows) {
        MailRecord mail;
        mail.postUid = row.at("PostUID").asInt();
        mail.senderUnitUid = row.at("SenderUnitUID").asInt();
        mail.senderName = row.at("SenderName").asText();
        mail.subject = row.at("Subject").asText();
        mail.body = row.at("Body").asText();
        mail.ed = row.at("Ed").asInt();
        mail.sentAt = row.at("SentAt").asInt();
        mail.read = row.at("ReadAt").asInt() != 0;
        const Rows items = db_.query(
            "SELECT ItemCode, Quantity, EnchantLevel FROM PostItems WHERE PostUID = ?;", {Value(mail.postUid)});
        for (const auto& item : items) {
            mail.items.push_back(MailItemRecord{static_cast<int>(item.at("ItemCode").asInt()),
                                                static_cast<int>(item.at("Quantity").asInt()),
                                                static_cast<int>(item.at("EnchantLevel").asInt())});
        }
        out->push_back(std::move(mail));
    }
    return true;
}

bool SocialRepository::markMailRead(std::int64_t postUid) {
    return db_.run("UPDATE Posts SET ReadAt = ? WHERE PostUID = ? AND ReadAt = 0;",
                   {Value(nowMs()), Value(postUid)});
}

bool SocialRepository::deleteMail(std::int64_t postUid) {
    return db_.run("UPDATE Posts SET IsDeleted = 1 WHERE PostUID = ?;", {Value(postUid)});
}

}  // namespace knc::db
