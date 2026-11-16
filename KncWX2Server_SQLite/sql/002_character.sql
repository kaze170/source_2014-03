-- 002_character.sql - characters ("units"), stats, skills, inventory and quests.
-- Derived from DataBase/Database2/Database2 dbo.GUser / dbo.GUnit / dbo.GItem* / dbo.GSkill*.

CREATE TABLE IF NOT EXISTS Units (
    UnitUID        INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    AccountUID     INTEGER NOT NULL REFERENCES Accounts(AccountUID) ON DELETE CASCADE,
    Slot           INTEGER NOT NULL DEFAULT 0,
    NickName       TEXT    NOT NULL,               -- dbo.GUnitNickName
    UnitClass      INTEGER NOT NULL DEFAULT 0,     -- 0 Elsword, 1 Aisha, 2 Rena, ...
    Level          INTEGER NOT NULL DEFAULT 1,
    Exp            INTEGER NOT NULL DEFAULT 0,
    Ed             INTEGER NOT NULL DEFAULT 0,     -- dbo.GUser.ED
    AuthLevel      INTEGER NOT NULL DEFAULT 0,
    KnmSerial      INTEGER NOT NULL DEFAULT 0,
    SP             INTEGER NOT NULL DEFAULT 0,
    CSP            INTEGER NOT NULL DEFAULT 0,
    MaxCSP         INTEGER NOT NULL DEFAULT 0,
    CspEndDate     TEXT    NOT NULL DEFAULT '',
    StraightWins   INTEGER NOT NULL DEFAULT 0,
    OfficialMatch  INTEGER NOT NULL DEFAULT 0,
    Rating         INTEGER NOT NULL DEFAULT 0,
    MaxRating      INTEGER NOT NULL DEFAULT 0,
    RPoint         INTEGER NOT NULL DEFAULT 0,
    APoint         INTEGER NOT NULL DEFAULT 0,
    Rank           INTEGER NOT NULL DEFAULT 0,
    KFactor        REAL    NOT NULL DEFAULT 0,
    MapId          INTEGER NOT NULL DEFAULT 0,
    LastFieldId    INTEGER NOT NULL DEFAULT 0,
    LastPosX       REAL    NOT NULL DEFAULT 0,
    LastPosY       REAL    NOT NULL DEFAULT 0,
    LastPosZ       REAL    NOT NULL DEFAULT 0,
    LastTown       TEXT    NOT NULL DEFAULT '',
    CreatedAt      INTEGER NOT NULL DEFAULT 0,
    LastPlayedAt   INTEGER NOT NULL DEFAULT 0,
    DeletedAt      INTEGER NOT NULL DEFAULT 0
);
CREATE UNIQUE INDEX IF NOT EXISTS UX_Units_NickName ON Units(NickName);
CREATE INDEX IF NOT EXISTS IX_Units_Account ON Units(AccountUID, Slot);

-- dbo.GDeletedNickName / dbo.GDeletedNickNameHistory
CREATE TABLE IF NOT EXISTS DeletedNickNames (
    NickName       TEXT    NOT NULL PRIMARY KEY,
    AccountUID     INTEGER NOT NULL DEFAULT 0,
    UnitUID        INTEGER NOT NULL DEFAULT 0,
    DeletedAt      INTEGER NOT NULL DEFAULT 0,
    ReusableAt     INTEGER NOT NULL DEFAULT 0
);

-- dbo.GCreateUnitCnt
CREATE TABLE IF NOT EXISTS CreateUnitCount (
    AccountUID     INTEGER NOT NULL PRIMARY KEY,
    Day            INTEGER NOT NULL DEFAULT 0,
    Count          INTEGER NOT NULL DEFAULT 0
);

CREATE TABLE IF NOT EXISTS UnitStats (
    UnitUID        INTEGER NOT NULL REFERENCES Units(UnitUID) ON DELETE CASCADE,
    StatKey        INTEGER NOT NULL,
    BaseValue      REAL    NOT NULL DEFAULT 0,
    GameValue      REAL    NOT NULL DEFAULT 0,
    PRIMARY KEY (UnitUID, StatKey)
);

-- dbo.GSkill / dbo.GSkillSlot / dbo.GSkillSlot2
CREATE TABLE IF NOT EXISTS Skills (
    UnitUID        INTEGER NOT NULL REFERENCES Units(UnitUID) ON DELETE CASCADE,
    SkillId        INTEGER NOT NULL,
    SkillLevel     INTEGER NOT NULL DEFAULT 1,
    PRIMARY KEY (UnitUID, SkillId)
);

CREATE TABLE IF NOT EXISTS SkillSlots (
    UnitUID        INTEGER NOT NULL REFERENCES Units(UnitUID) ON DELETE CASCADE,
    Page           INTEGER NOT NULL DEFAULT 0,
    Slot           INTEGER NOT NULL,
    SkillId        INTEGER NOT NULL DEFAULT 0,
    PRIMARY KEY (UnitUID, Page, Slot)
);

-- dbo.GItem and its satellite tables (attribute / enchant / socket / endurance / period / quantity)
CREATE TABLE IF NOT EXISTS Items (
    ItemUID        INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    UnitUID        INTEGER NOT NULL REFERENCES Units(UnitUID) ON DELETE CASCADE,
    ItemCode       INTEGER NOT NULL,
    SlotIndex      INTEGER NOT NULL DEFAULT 0,
    Category       INTEGER NOT NULL DEFAULT 0,      -- 0 inventory, 1 equip, 2 cash, 3 warehouse
    Quantity       INTEGER NOT NULL DEFAULT 1,
    EnchantLevel   INTEGER NOT NULL DEFAULT 0,
    Endurance      INTEGER NOT NULL DEFAULT 0,
    MaxEndurance   INTEGER NOT NULL DEFAULT 0,
    PeriodEnd      TEXT    NOT NULL DEFAULT '',
    IsEquipped     INTEGER NOT NULL DEFAULT 0,
    CreatedAt      INTEGER NOT NULL DEFAULT 0,
    UpdatedAt      INTEGER NOT NULL DEFAULT 0
);
CREATE INDEX IF NOT EXISTS IX_Items_Unit ON Items(UnitUID, Category);

CREATE TABLE IF NOT EXISTS ItemAttributes (
    ItemUID        INTEGER NOT NULL REFERENCES Items(ItemUID) ON DELETE CASCADE,
    AttrIndex      INTEGER NOT NULL,
    AttrType       INTEGER NOT NULL DEFAULT 0,
    AttrValue      INTEGER NOT NULL DEFAULT 0,
    PRIMARY KEY (ItemUID, AttrIndex)
);

CREATE TABLE IF NOT EXISTS ItemSockets (
    ItemUID        INTEGER NOT NULL REFERENCES Items(ItemUID) ON DELETE CASCADE,
    SocketIndex    INTEGER NOT NULL,
    SocketType     INTEGER NOT NULL DEFAULT 0,
    SocketValue    INTEGER NOT NULL DEFAULT 0,
    PRIMARY KEY (ItemUID, SocketIndex)
);

-- dbo.GQuests / GQuests_Complete / GQuests_Repeat, dbo.GQuestCode
CREATE TABLE IF NOT EXISTS Quests (
    UnitUID        INTEGER NOT NULL REFERENCES Units(UnitUID) ON DELETE CASCADE,
    QuestId        INTEGER NOT NULL,
    Progress       INTEGER NOT NULL DEFAULT 0,
    State          INTEGER NOT NULL DEFAULT 0,   -- 0 active, 1 complete
    AcceptedAt     INTEGER NOT NULL DEFAULT 0,
    PRIMARY KEY (UnitUID, QuestId)
);

CREATE TABLE IF NOT EXISTS QuestClears (
    UnitUID        INTEGER NOT NULL REFERENCES Units(UnitUID) ON DELETE CASCADE,
    QuestId        INTEGER NOT NULL,
    ClearedAt      INTEGER NOT NULL DEFAULT 0,
    RepeatCount    INTEGER NOT NULL DEFAULT 0,
    PRIMARY KEY (UnitUID, QuestId)
);

-- dbo.GDungeonClear / dbo.GTrainingCenter / dbo.GTitle_*
CREATE TABLE IF NOT EXISTS DungeonClears (
    UnitUID        INTEGER NOT NULL REFERENCES Units(UnitUID) ON DELETE CASCADE,
    DungeonId      INTEGER NOT NULL,
    Difficulty     INTEGER NOT NULL DEFAULT 0,
    ClearCount     INTEGER NOT NULL DEFAULT 0,
    BestTimeMs     INTEGER NOT NULL DEFAULT 0,
    LastClearedAt  INTEGER NOT NULL DEFAULT 0,
    PRIMARY KEY (UnitUID, DungeonId, Difficulty)
);

CREATE TABLE IF NOT EXISTS Titles (
    UnitUID        INTEGER NOT NULL REFERENCES Units(UnitUID) ON DELETE CASCADE,
    TitleId        INTEGER NOT NULL,
    IsComplete     INTEGER NOT NULL DEFAULT 0,
    IsEquipped     INTEGER NOT NULL DEFAULT 0,
    MissionCount   INTEGER NOT NULL DEFAULT 0,
    PRIMARY KEY (UnitUID, TitleId)
);

CREATE TABLE IF NOT EXISTS InventorySizes (
    UnitUID        INTEGER NOT NULL REFERENCES Units(UnitUID) ON DELETE CASCADE,
    Category       INTEGER NOT NULL,
    Size           INTEGER NOT NULL DEFAULT 0,
    PRIMARY KEY (UnitUID, Category)
);
