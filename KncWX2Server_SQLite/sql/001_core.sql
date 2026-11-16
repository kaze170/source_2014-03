-- 001_core.sql - account, server topology, options and the schema bookkeeping tables.
--
-- Derived from DataBase/Database2/Database1 (Account/Game01) and the MServerList/ConnectStatusDB
-- tables the 2014 servers register themselves in. MSSQL types map as:
--   INT/INTEGER -> INTEGER, BIGINT -> INTEGER, NVARCHAR(n) -> TEXT, DATETIME -> INTEGER (unix ms)
--   TINYINT/BIT -> INTEGER, BINARY/VARBINARY -> BLOB.

PRAGMA foreign_keys = ON;

-- ---------------------------------------------------------------- meta ----
CREATE TABLE IF NOT EXISTS SchemaInfo (
    Version        INTEGER NOT NULL PRIMARY KEY,
    AppliedAt      INTEGER NOT NULL,
    Description    TEXT    NOT NULL
);

-- ------------------------------------------------------------- account ----
CREATE TABLE IF NOT EXISTS Accounts (
    AccountUID     INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    LoginID        TEXT    NOT NULL UNIQUE,          -- was NVARCHAR(20) in Account.dbo.Accounts
    PasswordHash   TEXT    NOT NULL,                 -- sha256(LoginID || ':' || Password)
    PasswordSalt   TEXT    NOT NULL DEFAULT '',
    DisplayName    TEXT    NOT NULL DEFAULT '',
    AuthLevel      INTEGER NOT NULL DEFAULT 0,       -- 0 = player, >= 1 = gm
    InternalUser   INTEGER NOT NULL DEFAULT 0,
    GuestUser      INTEGER NOT NULL DEFAULT 0,
    Age            INTEGER NOT NULL DEFAULT 0,
    Sex            INTEGER NOT NULL DEFAULT 0,       -- 0 female, 1 male
    NexonSN        INTEGER NOT NULL DEFAULT 0,
    BlockEndTime   TEXT    NOT NULL DEFAULT '',
    BlockReason    TEXT    NOT NULL DEFAULT '',
    BlockType      INTEGER NOT NULL DEFAULT 0,
    BlockReason2   TEXT    NOT NULL DEFAULT '',
    BlockEndDate   TEXT    NOT NULL DEFAULT '',
    LastLoginAt    INTEGER NOT NULL DEFAULT 0,
    LastLoginIp    TEXT    NOT NULL DEFAULT '',
    CreatedAt      INTEGER NOT NULL DEFAULT 0,
    IsDeleted      INTEGER NOT NULL DEFAULT 0
);
CREATE INDEX IF NOT EXISTS IX_Accounts_LoginID ON Accounts(LoginID);

-- One row per (account, key). The primary key used to be AccountUID alone, which made the table
-- hold a single option per account: writing a second key silently replaced the first.
CREATE TABLE IF NOT EXISTS AccountOptions (
    AccountUID     INTEGER NOT NULL REFERENCES Accounts(AccountUID) ON DELETE CASCADE,
    OptKey         TEXT    NOT NULL,
    OptValue       INTEGER NOT NULL DEFAULT 0,
    PRIMARY KEY (AccountUID, OptKey)
);

CREATE TABLE IF NOT EXISTS LoginHistory (
    Seq            INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    AccountUID     INTEGER NOT NULL DEFAULT 0,
    LoginID        TEXT    NOT NULL DEFAULT '',
    Ip             TEXT    NOT NULL DEFAULT '',
    MachineId      TEXT    NOT NULL DEFAULT '',
    ServerName     TEXT    NOT NULL DEFAULT '',
    ResultCode     INTEGER NOT NULL DEFAULT 0,
    At             INTEGER NOT NULL DEFAULT 0
);
CREATE INDEX IF NOT EXISTS IX_LoginHistory_Account ON LoginHistory(AccountUID, At);

-- ------------------------------------------------- server topology --------
-- Replaces dbo.MServerList / dbo.ConnectStatusDB (see KncWX2Server/Query/InsertServerList.sql).
CREATE TABLE IF NOT EXISTS ServerSet (
    ServerSetID    INTEGER NOT NULL PRIMARY KEY,
    Name           TEXT    NOT NULL,
    DisplayOrder   INTEGER NOT NULL DEFAULT 0,
    IsOpen         INTEGER NOT NULL DEFAULT 1
);

CREATE TABLE IF NOT EXISTS ServerList (
    ServerUID      INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    Name           TEXT    NOT NULL,
    ServerSetID    INTEGER NOT NULL DEFAULT 0,
    ServerType     INTEGER NOT NULL DEFAULT 1,   -- 0 login, 1 game, 2 channel, 3 center, 4 global
    Ip             TEXT    NOT NULL DEFAULT '127.0.0.1',
    Port           INTEGER NOT NULL DEFAULT 0,
    NcUdpPort      INTEGER NOT NULL DEFAULT 0,
    MaxUser        INTEGER NOT NULL DEFAULT 3000,
    CurrentUser    INTEGER NOT NULL DEFAULT 0,
    DisplayOrder   INTEGER NOT NULL DEFAULT 1,
    IsOpen         INTEGER NOT NULL DEFAULT 1,
    ServerClass    INTEGER NOT NULL DEFAULT 0
);

CREATE TABLE IF NOT EXISTS ChannelList (
    ChannelUID     INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    ServerSetID    INTEGER NOT NULL DEFAULT 0,
    ChannelID      INTEGER NOT NULL,
    Name           TEXT    NOT NULL,
    Ip             TEXT    NOT NULL DEFAULT '127.0.0.1',
    MasterPort     INTEGER NOT NULL DEFAULT 0,
    NcUdpPort      INTEGER NOT NULL DEFAULT 0,
    MaxUser        INTEGER NOT NULL DEFAULT 3000,
    CurrentUser    INTEGER NOT NULL DEFAULT 0,
    PartyCount     INTEGER NOT NULL DEFAULT 0,
    PlayingParty   INTEGER NOT NULL DEFAULT 0,
    IsOpen         INTEGER NOT NULL DEFAULT 1,
    BonusExp       INTEGER NOT NULL DEFAULT 0,
    BonusDrop      INTEGER NOT NULL DEFAULT 0
);
CREATE UNIQUE INDEX IF NOT EXISTS UX_ChannelList_Set_Channel ON ChannelList(ServerSetID, ChannelID);

-- Channel membership runtime table (was ConnectStatusDB): who is on which channel.
CREATE TABLE IF NOT EXISTS ConnectStatus (
    AccountUID     INTEGER NOT NULL PRIMARY KEY,
    UnitUID        INTEGER NOT NULL DEFAULT 0,
    ChannelID      INTEGER NOT NULL DEFAULT 0,
    ServerName     TEXT    NOT NULL DEFAULT '',
    ConnectedAt    INTEGER NOT NULL DEFAULT 0,
    UpdatedAt      INTEGER NOT NULL DEFAULT 0
);
