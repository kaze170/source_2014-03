-- 003_social.sql - friends, guilds, mail, personal shops and trade audit.
-- Derived from dbo.GFriendList, dbo.GPost*, dbo.GBlacklist, dbo.GTutor, dbo.DelGuildInfo.

CREATE TABLE IF NOT EXISTS Friends (
    UnitUID        INTEGER NOT NULL,
    FriendUnitUID  INTEGER NOT NULL,
    GroupName      TEXT    NOT NULL DEFAULT '',
    Memo           TEXT    NOT NULL DEFAULT '',
    CreatedAt      INTEGER NOT NULL DEFAULT 0,
    PRIMARY KEY (UnitUID, FriendUnitUID)
);

CREATE TABLE IF NOT EXISTS Blacklist (
    UnitUID        INTEGER NOT NULL,
    BlockedUnitUID INTEGER NOT NULL,
    ReasonCode     INTEGER NOT NULL DEFAULT 0,
    CreatedAt      INTEGER NOT NULL DEFAULT 0,
    PRIMARY KEY (UnitUID, BlockedUnitUID)
);

CREATE TABLE IF NOT EXISTS Guilds (
    GuildUID       INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    Name           TEXT    NOT NULL UNIQUE,
    MasterUnitUID  INTEGER NOT NULL DEFAULT 0,
    Level          INTEGER NOT NULL DEFAULT 1,
    Exp            INTEGER NOT NULL DEFAULT 0,
    Point          INTEGER NOT NULL DEFAULT 0,
    Notice         TEXT    NOT NULL DEFAULT '',
    CreatedAt      INTEGER NOT NULL DEFAULT 0,
    IsDeleted      INTEGER NOT NULL DEFAULT 0
);

CREATE TABLE IF NOT EXISTS GuildMembers (
    GuildUID       INTEGER NOT NULL REFERENCES Guilds(GuildUID) ON DELETE CASCADE,
    UnitUID        INTEGER NOT NULL,
    Rank           INTEGER NOT NULL DEFAULT 0,
    JoinedAt       INTEGER NOT NULL DEFAULT 0,
    Contribution   INTEGER NOT NULL DEFAULT 0,
    PRIMARY KEY (GuildUID, UnitUID)
);

-- dbo.GPost / GPost_ED / GPost_Attribute / GPost_Enchant / GPost_Socket
CREATE TABLE IF NOT EXISTS Posts (
    PostUID        INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    SenderUnitUID  INTEGER NOT NULL DEFAULT 0,
    SenderName     TEXT    NOT NULL DEFAULT '',
    ReceiverUnitUID INTEGER NOT NULL DEFAULT 0,
    ReceiverName   TEXT    NOT NULL DEFAULT '',
    Subject        TEXT    NOT NULL DEFAULT '',
    Body           TEXT    NOT NULL DEFAULT '',
    Ed             INTEGER NOT NULL DEFAULT 0,
    SentAt         INTEGER NOT NULL DEFAULT 0,
    ReadAt         INTEGER NOT NULL DEFAULT 0,
    ExpiresAt      INTEGER NOT NULL DEFAULT 0,
    IsDeleted      INTEGER NOT NULL DEFAULT 0
);
CREATE INDEX IF NOT EXISTS IX_Posts_Receiver ON Posts(ReceiverUnitUID, IsDeleted);

CREATE TABLE IF NOT EXISTS PostItems (
    PostItemUID    INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    PostUID        INTEGER NOT NULL REFERENCES Posts(PostUID) ON DELETE CASCADE,
    ItemCode       INTEGER NOT NULL,
    Quantity       INTEGER NOT NULL DEFAULT 1,
    EnchantLevel   INTEGER NOT NULL DEFAULT 0,
    Attributes     BLOB,
    Sockets        BLOB
);

-- dbo.GRecommend (recommendation / "friend invite" rewards)
CREATE TABLE IF NOT EXISTS Recommendations (
    UnitUID        INTEGER NOT NULL,
    TargetUnitUID  INTEGER NOT NULL,
    At             INTEGER NOT NULL DEFAULT 0,
    PRIMARY KEY (UnitUID, TargetUnitUID)
);
