-- 005_progress.sql - ranking, events, world missions, battle field matchmaking and stats.

-- dbo.Rank_* / dbo.GRank_* collapsed into one parameterised table (rankType selects the board).
CREATE TABLE IF NOT EXISTS Rankings (
    RankType       INTEGER NOT NULL,   -- 0 pvp, 1 elsword, 2 aisha, 3 rena, 4 arcade, ...
    UnitUID        INTEGER NOT NULL,
    NickName       TEXT    NOT NULL DEFAULT '',
    Value          INTEGER NOT NULL DEFAULT 0,
    Extra          INTEGER NOT NULL DEFAULT 0,
    ServerSetID    INTEGER NOT NULL DEFAULT 0,
    UpdatedAt      INTEGER NOT NULL DEFAULT 0,
    PRIMARY KEY (RankType, UnitUID)
);
CREATE INDEX IF NOT EXISTS IX_Rankings_Order ON Rankings(RankType, Value DESC);

CREATE TABLE IF NOT EXISTS EventRewards (
    EventId        INTEGER NOT NULL,
    AccountUID     INTEGER NOT NULL,
    UnitUID        INTEGER NOT NULL DEFAULT 0,
    RewardCode     INTEGER NOT NULL,
    ClaimedAt      INTEGER NOT NULL DEFAULT 0,
    PRIMARY KEY (EventId, AccountUID, RewardCode)
);

CREATE TABLE IF NOT EXISTS WorldMissions (
    MissionId      INTEGER NOT NULL PRIMARY KEY,
    Name           TEXT    NOT NULL DEFAULT '',
    TargetCount    INTEGER NOT NULL DEFAULT 1,
    RewardItemCode INTEGER NOT NULL DEFAULT 0,
    RewardQuantity INTEGER NOT NULL DEFAULT 1,
    StartAt        INTEGER NOT NULL DEFAULT 0,
    EndAt          INTEGER NOT NULL DEFAULT 0,
    IsActive       INTEGER NOT NULL DEFAULT 1
);

CREATE TABLE IF NOT EXISTS WorldMissionProgress (
    MissionId      INTEGER NOT NULL REFERENCES WorldMissions(MissionId) ON DELETE CASCADE,
    Scope          INTEGER NOT NULL DEFAULT 0,   -- world wide counter row when 0
    Counter        INTEGER NOT NULL DEFAULT 0,
    UpdatedAt      INTEGER NOT NULL DEFAULT 0,
    PRIMARY KEY (MissionId, Scope)
);

CREATE TABLE IF NOT EXISTS FieldBossState (
    BossId         INTEGER NOT NULL PRIMARY KEY,
    FieldId        INTEGER NOT NULL DEFAULT 0,
    NextSpawnAt    INTEGER NOT NULL DEFAULT 0,
    LastKillerUnit INTEGER NOT NULL DEFAULT 0,
    KillCount      INTEGER NOT NULL DEFAULT 0
);

CREATE TABLE IF NOT EXISTS MatchMakingQueue (
    EntryUID       INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    UnitUID        INTEGER NOT NULL,
    Mode           INTEGER NOT NULL DEFAULT 0,
    Rating         INTEGER NOT NULL DEFAULT 0,
    QueuedAt       INTEGER NOT NULL DEFAULT 0,
    MatchId        INTEGER NOT NULL DEFAULT 0
);

CREATE TABLE IF NOT EXISTS Statistics (
    StatId         INTEGER NOT NULL,
    Bucket         INTEGER NOT NULL,    -- unix minute/hour bucket
    ServerName     TEXT    NOT NULL DEFAULT '',
    Counter0       INTEGER NOT NULL DEFAULT 0,
    Counter1       INTEGER NOT NULL DEFAULT 0,
    Counter2       INTEGER NOT NULL DEFAULT 0,
    PRIMARY KEY (StatId, Bucket, ServerName)
);

CREATE TABLE IF NOT EXISTS Scripts (
    ScriptName     TEXT    NOT NULL PRIMARY KEY,
    Version        INTEGER NOT NULL DEFAULT 1,
    Content        BLOB    NOT NULL,
    UpdatedAt      INTEGER NOT NULL DEFAULT 0
);

CREATE TABLE IF NOT EXISTS LogEntries (
    Seq            INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    Category       INTEGER NOT NULL DEFAULT 0,
    Level          INTEGER NOT NULL DEFAULT 2,
    ServerName     TEXT    NOT NULL DEFAULT '',
    AccountUID     INTEGER NOT NULL DEFAULT 0,
    UnitUID        INTEGER NOT NULL DEFAULT 0,
    Message        TEXT    NOT NULL DEFAULT '',
    At             INTEGER NOT NULL DEFAULT 0
);
CREATE INDEX IF NOT EXISTS IX_LogEntries_At ON LogEntries(At, Category);

CREATE TABLE IF NOT EXISTS SmsQueue (
    SmsUID         INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    PhoneNumber    TEXT    NOT NULL DEFAULT '',
    Message        TEXT    NOT NULL DEFAULT '',
    QueuedAt       INTEGER NOT NULL DEFAULT 0,
    SentAt         INTEGER NOT NULL DEFAULT 0,
    Success        INTEGER NOT NULL DEFAULT 0
);
