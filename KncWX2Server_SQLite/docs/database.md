# Database

One SQLite file (`data/knc.db` by default) replaces six ODBC DSNs (`DC_ACCOUNT`, `DC_GAME`,
`DC_LOG`, `DC_SMS`, ...), the `DBThreadManager` pool and ~200 stored procedures. A repository is a
plain object that runs one statement; the server calls it directly on the network thread (or from a
worker, when a query must not block).

## Conventions

* Timestamps: unix **milliseconds** in `INTEGER` columns. `0` means "never".
* Text: UTF-8 in the database; CP949 is only used for the 2014 `.ini` files and legacy resources.
* Accounts and units are never hard deleted: `Units.DeletedAt` marks them, `DeletedNickNames` keeps
  the name reserved for a reuse delay.
* Deletion cascades through foreign keys (`PRAGMA foreign_keys = ON`).
* All statements are `CREATE ... IF NOT EXISTS`, so `applySchema()` is idempotent and runs on every
  start; the schema version lives in `PRAGMA user_version` and in `SchemaInfo`.

Pragmas applied on open (`kdb/Database.cpp`):

```
journal_mode=WAL   synchronous=NORMAL   foreign_keys=ON   cache_size=-65536 (64 MiB)
wal_autocheckpoint=2000   mmap_size=268435456 (256 MiB)   journal_size_limit=67108864
```

On `close()` the database checkpoints with `TRUNCATE`, so a clean shutdown leaves no `-wal` file.

## Migrations

| script | contents |
| ------ | -------- |
| `001_core.sql` | `Accounts`, `AccountOptions`, `LoginHistory`, `ServerSet`, `ServerList`, `ChannelList`, `ConnectStatus`, `SchemaInfo` |
| `002_character.sql` | `Units`, `UnitStats`, `Skills`, `SkillSlots`, `Items`, `ItemAttributes`, `ItemSockets`, `Quests`, `QuestClears`, `DungeonClears`, `Titles`, `InventorySizes`, `DeletedNickNames`, `CreateUnitCount` |
| `003_social.sql` | `Friends`, `Blacklist`, `Guilds`, `GuildMembers`, `Posts`, `PostItems`, `Recommendations` |
| `004_economy.sql` | `CashItems`, `CashItemDetails`, `CashInventory`, `BillingAccounts`, `BillingTransactions`, `TradeLog` |
| `005_progress.sql` | `Rankings`, `EventRewards`, `WorldMissions`, `WorldMissionProgress`, `FieldBossState`, `MatchMakingQueue`, `Statistics`, `Scripts`, `LogEntries`, `SmsQueue` |

`tools/gen_schema.py` embeds every script into `src/kdb/SchemaSql.gen.cpp`, so the binary is
self-contained - run `make gen` after editing any `.sql` file.

## Key tables

### Accounts

```sql
Accounts(
    AccountUID INTEGER PRIMARY KEY AUTOINCREMENT,
    LoginID    TEXT NOT NULL UNIQUE,
    PasswordHash TEXT NOT NULL,      -- salted MD5 placeholder, see below
    DisplayName TEXT NOT NULL,
    AuthLevel  INTEGER,              -- 0 player, 9 operator (KAccountInfo::m_iAuthLevel)
    InternalUser INTEGER, GuestUser INTEGER, IsDeleted INTEGER,
    BlockEndTime TEXT, BlockReason TEXT, BlockType INTEGER, BlockReason2 TEXT, BlockEndDate TEXT,
    Age INTEGER, Sex INTEGER, NexonSN INTEGER,
    CreatedAt INTEGER, LastLoginAt INTEGER, LastLoginIp TEXT
)
```

A block is **active while `BlockEndTime` is non-empty** - that is how `KAccountBlockInfo` behaves on
the wire (an empty end time means "not blocked"), and `knc_dbtool block` writes the far future for a
permanent punishment.

`PasswordHash` is `hex(md5(loginId + "\x1f" + password))` (`db::hashAccountPassword`). It keeps
credentials out of the clear, but it is **not** a production KDF and must be replaced before any real
deployment; the salt, the separator and the digest are all in one function on purpose.

### Units

```sql
Units(
    UnitUID INTEGER PRIMARY KEY AUTOINCREMENT, AccountUID INTEGER REFERENCES Accounts,
    Slot INTEGER, NickName TEXT NOT NULL,          -- unique among live units
    UnitClass INTEGER, Level INTEGER, Exp INTEGER, Ed INTEGER,   -- ED lives on the character
    AuthLevel, KnmSerial, SP, CSP, MaxCSP, CspEndDate,
    StraightWins, OfficialMatch, Rating, MaxRating, RPoint, APoint, Rank, KFactor,
    MapId, LastFieldId, LastPosX/Y/Z, LastTown,
    CreatedAt, LastPlayedAt, DeletedAt
)
```

`Exp` is the **cumulative** exp of the character: `ScriptData/ExpTable.xls` publishes the total exp
per level (level 2 at 600, level 80 at 91,056,800), and `kgame::ExpTable::levelForExp` applies the
same comparison the 2014 `KExpTable::CheckLevelUp` did.

### Items

```sql
Items(ItemUID, UnitUID, ItemCode, SlotIndex, Category, Quantity, EnchantLevel, Endurance,
      MaxEndurance, PeriodEnd, IsEquipped, CreatedAt, UpdatedAt)
ItemAttributes(ItemUID, AttrIndex, AttrType, AttrValue)   -- AttrType is a KStat key
ItemSockets(ItemUID, SocketIndex, SocketType, SocketValue)
InventorySizes(UnitUID, Category, Size)                   -- one row per bag
```

`Category` follows the client's bag numbering (1 equipment, 2 consumable, 3 cash, 4 quest,
5 special). A new character gets 80 equipment slots and 60 consumable slots; later expansion
(`EDInventoryExpand.lua`) goes through `InventoryRepository::setInventorySize`.

### Topology

`ServerSet` / `ServerList` / `ChannelList` mirror `Query/InsertServerList.sql`. Each start calls
`seedDefaultTopology()` with the configured name, IP and ports, so the database and `config/knc.ini`
cannot drift apart.

## Repository API

`include/kdb/Repositories.h`, one struct per domain, all bound to one `Database`:

| repository | main calls |
| ---------- | ---------- |
| `AccountRepository` | `authenticate`, `authenticateGuest`, `create`, `load`, `applyBlock`, `clearBlock`, `setOption`/`getOption`, `recordLogin`, `count` |
| `UnitRepository` | `listSummaries`, `create`, `load`, `save`, `remove`, `isNickNameAvailable`, `countForAccount`, `countCreatedToday` |
| `InventoryRepository` | `listForUnit`, `addItem`, `removeItem`, `setEnchant`, `setAttribute`, `setSocket`, `setEquipped`, `getInventorySize`/`setInventorySize`, `totalEd` |
| `SocialRepository` | friends, guilds, mail |
| `TopologyRepository` | `listChannels`, `listServers`, `setCurrentUsers`, `upsertConnectStatus` |
| `BillingRepository` | cash items, `addCash`, `purchase`, `balance` |
| `ProgressRepository` | rankings, event rewards, world missions, dungeon clears, log |

`db::repositories()` is the process wide set; `db::makeRepositories(Database&)` builds one over an
explicit database (the tests use it with `:memory:` databases).

## Tools

```sh
knc_dbtool --db data/knc.db init        # create + seed
knc_dbtool --db data/knc.db check       # PRAGMA integrity_check, foreign_key_check
knc_dbtool --db data/knc.db vacuum      # checkpoint + VACUUM
```

A WAL database can be read while the server runs; open it read-only (`Config::readOnly`).
