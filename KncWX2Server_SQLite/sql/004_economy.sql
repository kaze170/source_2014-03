-- 004_economy.sql - cash shop, billing and ED trading.
-- Derived from dbo.CashItemList / CashItemDisplayList / CashPackageInfoList / EPEventPerfectItem.

CREATE TABLE IF NOT EXISTS CashItems (
    CashItemId     INTEGER NOT NULL PRIMARY KEY,
    ItemCode       INTEGER NOT NULL,
    Name           TEXT    NOT NULL DEFAULT '',
    Price          INTEGER NOT NULL DEFAULT 0,      -- in cash points
    CashType       INTEGER NOT NULL DEFAULT 0,      -- 0 nexon, 1 giant, ...
    DurationDays   INTEGER NOT NULL DEFAULT 0,
    IsPackage      INTEGER NOT NULL DEFAULT 0,
    IsSelling      INTEGER NOT NULL DEFAULT 1,
    DisplayOrder   INTEGER NOT NULL DEFAULT 0,
    StartAt        INTEGER NOT NULL DEFAULT 0,
    EndAt          INTEGER NOT NULL DEFAULT 0
);

CREATE TABLE IF NOT EXISTS CashItemDetails (
    CashItemId     INTEGER NOT NULL REFERENCES CashItems(CashItemId) ON DELETE CASCADE,
    ItemCode       INTEGER NOT NULL,
    Quantity       INTEGER NOT NULL DEFAULT 1,
    PRIMARY KEY (CashItemId, ItemCode)
);

CREATE TABLE IF NOT EXISTS CashInventory (
    CashInvUID     INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    AccountUID     INTEGER NOT NULL,
    CashItemId     INTEGER NOT NULL,
    ItemCode       INTEGER NOT NULL,
    PurchasedAt    INTEGER NOT NULL DEFAULT 0,
    ExpiresAt      INTEGER NOT NULL DEFAULT 0,
    IsConsumed     INTEGER NOT NULL DEFAULT 0,
    IsGift         INTEGER NOT NULL DEFAULT 0
);
CREATE INDEX IF NOT EXISTS IX_CashInventory_Account ON CashInventory(AccountUID);

CREATE TABLE IF NOT EXISTS BillingAccounts (
    AccountUID     INTEGER NOT NULL PRIMARY KEY,
    CashBalance    INTEGER NOT NULL DEFAULT 0,
    BonusCash      INTEGER NOT NULL DEFAULT 0,
    UpdatedAt      INTEGER NOT NULL DEFAULT 0
);

CREATE TABLE IF NOT EXISTS BillingTransactions (
    TxUID          INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    AccountUID     INTEGER NOT NULL DEFAULT 0,
    TxType         INTEGER NOT NULL DEFAULT 0,   -- 0 charge, 1 purchase, 2 refund, 3 bonus
    Amount         INTEGER NOT NULL DEFAULT 0,
    BalanceAfter   INTEGER NOT NULL DEFAULT 0,
    Reference      TEXT    NOT NULL DEFAULT '',
    At             INTEGER NOT NULL DEFAULT 0,
    Success        INTEGER NOT NULL DEFAULT 1
);
CREATE INDEX IF NOT EXISTS IX_BillingTx_Account ON BillingTransactions(AccountUID, At);

CREATE TABLE IF NOT EXISTS TradeLog (
    TradeUID       INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    SenderUnitUID  INTEGER NOT NULL DEFAULT 0,
    ReceiverUnitUID INTEGER NOT NULL DEFAULT 0,
    EdAmount       INTEGER NOT NULL DEFAULT 0,
    ItemSummary    TEXT    NOT NULL DEFAULT '',
    At             INTEGER NOT NULL DEFAULT 0
);
