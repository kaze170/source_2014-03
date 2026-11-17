# Porting notes: 2014 -> this rewrite

A map from the original files to their replacements, the deliberate differences, and the list of
what is still missing.

## File map

| 2014 | here | notes |
| ---- | ---- | ----- |
| `KNCSDK/Include/Serializer/Serializer.*` | `kcore/Serialize.h`, `src/kcore/Serialize.cpp` | untagged, big endian, `wstring` as UTF-16LE |
| `KNCSDK/Include/Serializer/SerBuffer.*` | `kcore/SerBuffer.h/.cpp` | `[u32 len][bool][zlib]` |
| `KNCSDK/Include/KncSecurity/*` | `ksec/{Des,Md5,SecureBuffer}` | DES-CBC + HMAC-MD5-10 + replay window |
| `Common/Event.*` | `kproto/Event.h`, `src/kproto/Event.cpp` | KEvent envelope, `pushTrace`/`popTrace` |
| `Common/EventID_*.h` | `include/kproto/EventId.gen.h` | 2,909 ids, generated |
| `Common/NetLayer.*`, `SimLayer.*` | `knet/{Socket,Reactor,Session,Listener}` | poll(2) reactor, non blocking sockets |
| `Common/BaseServer.*` | `kserver/Config`, `kserver/Server` | ini file, version strings, listener setup |
| `Common/ServerDefine.h` | `include/kproto/FeatureFlags.gen.h` | 568 on / 133 off, generated |
| `LoginServer/*` | `kserver/LoginService` | connect + verify + new user |
| `ChannelServer/*` | `kserver/ChannelService` | server set, channel list, character flow |
| `Common/ExpTable.*` + `ExpTable.lua` | `kgame/ExpTable`, `ExpTableData.gen.cpp` | generated from `ScriptData/ExpTable.xls` |
| `Common/StatTable` + `ServerResource/StatTable.lua` | `kgame/StatTable`, `StatTableData.gen.cpp` | 4,198 rows, 52 classes |
| `Common/ODBC/*`, `DBThreadManager`, `DBLayer` | `kdb/*` | one SQLite file, typed repositories |
| `DataBase/Database1`, `Database2`, `DB_Backup/*` | `sql/001..005` | normalized, same entities |
| `Query/InsertServerList.sql` | `db::seedDefaultTopology()` | seeded on every start |

## Wire compatibility

The bytes are the contract. `tests/TestWire.cpp` pins the serializer (both directions, every
primitive, containers, `SerBuffer` compression), the event envelope offsets, and the crypto known
answer tests. `tests/TestNet.cpp` performs a real handshake over a socket and checks the SPI-0
`E_ACCEPT_CONNECTION_NOT`, sealed frames, ICV failures and partial frames.

Keeping that contract is why several "obvious" cleanups were rejected:

* `wstring` stays UTF-16LE instead of big endian (the client memcpy'd `wchar_t` buffers).
* Packet structs keep their 2014 field order and keep flag-gated fields instead of being reordered
  into "logical" groups; `include/kproto/UnitPackets.h` `static_assert`s the flags the layout
  assumes.
* `char` (not `bool`) is used wherever the 2014 struct used `char`, because both are one byte but a
  future `bool` refactor could change packing assumptions in the MSVC build.

## Deliberate differences

* **One process.** The five executables became one; inter-server events (`E_CN_*`, `E_GS_*`,
  `E_LG_*` between servers) are plain function calls. `PortRole` keeps the port split so a client
  still connects to the login port and is redirected to the channel port.
* **SQLite instead of SQL Server.** No stored procedures; a repository call is one prepared
  statement. WAL gives concurrent readers for `knc_dbtool` while the server runs.
* **No ODBC thread pool.** Queries are synchronous. A long query blocks the reactor, which is
  acceptable at this scale (a character load is a handful of indexed lookups); `Reactor::post()`
  exists for when it stops being acceptable.
* **Password storage.** The 2014 database compared a plain password column in `mup_auth_user`. This
  build stores `hex(md5(login + 0x1f + password))` - still not a KDF, but no clear text.
* **Compiled in data.** `ExpTable.lua` / `StatTable.lua` are generated into `.gen.cpp` files so the
  server runs without a resource directory. `make gen` rebuilds them from the original files.
* **Config file.** One `config/knc.ini` with a section per former server, instead of five files that
  only held a version string and a message table (the ports were compiled in). The `[Version]`
  keys - `Internal Version`, `OpenTest Version`, `Service Version` - are read exactly as
  `KBaseServer::LoadINIFile` did, because `KGSUser::OnConnectReq` compares the client's
  `m_wstrVersion` against them.

## Feature flags

`tools/gen_featureflags.py` reads `Common/ServerDefine.h` and emits one `constexpr bool` per
macro: 691 enabled, 185 disabled, plus 61 that the packet headers test but the define tree never
mentions (region `NONE`). That single set is what every packet layout in
this rewrite was resolved with. If a flag is ever flipped, the `static_assert`s in
`include/kproto/UnitPackets.h` fail the build instead of silently changing the wire format.

The generator is a small preprocessor, not a grep: it walks the original include chain in file order
and honours `#undef`, so the 351 macros that `ServerDefine.h` only *mentions* (they come from
`OnlyGlobal/ServerDefine/ServerDefine_Global.h`, through the region ladder) are counted, and the ones
that are `#undef`ed later - or defined only inside a block that is itself off - are not.

The 61 extra entries come from scanning `CommonPacket.h`, `ClientPacket.h`, `ServerPacket.h` and the
two matching `.cpp` files for `#ifdef`/`#if defined(...)`. A macro that guards a *field* but is never
defined - `SERV_GOLD_TICKET` around `m_iGoldTicketKeyUID`, `SERV_STEAM` around `m_bSteamClient` - is
off, and keeping it in the table means "off" and "unknown" can no longer be confused.

### Checking a packet layout

`tools/verify_packets.py` is the guard rail. For every type in its table it resolves the original
`SERIALIZE_DEFINE_PUT` body with the generated flags, reduces each field to a canonical token
(`i8/u8/i16/.../f32/wstr/str`, `vec<>`, `map<>`, nested structs expanded recursively) and compares
that ordered token stream with our `serialize()`. Field *names* may differ; order, width, signedness
and conditionality may not. It currently reports `46 packet layout(s) match the 2014 serializers` -
every struct in `kproto` - and it is how the missing `guildUid` in `EGS_DELETE_UNIT_ACK`, the wrong
`channelRandomKey` in the login request and the unsigned `sealData` were found. It also fails when a
type it checks tests a flag the generated table does not define, which is how those 61 flags were
found in the first place.

A comparator is only worth having if it actually rejects things, so `--self-test` breaks three
layouts on purpose - a swapped pair of fields, a flipped signedness, and a dropped field in the middle
of `KUnitInfo` - and fails unless all three are reported. `make check` runs it before the real
comparison, so the guard rail cannot rot into a script that always prints "matches".

`tools/dump_original_layouts.py <Type>...` prints the resolved declaration of any original type (and
`--sizes` prints the fixed byte count of each), which is the fastest way to answer "what does the 2014
struct actually contain".

### The level curve has a ceiling that is not the table length

`ExpTable.xls` has 80 rows, but the shipped `GameSysValTable.lua` says
`GameSysVal:AddLimitsMaxLevel( 65 )`, and `KExpTable::CheckLevelUp` ends with

    if ( static_cast<u_char>( SiKGameSysVal()->GetLimitsLevel() ) < nResultLevel )
        nResultLevel = static_cast<u_char>( SiKGameSysVal()->GetLimitsLevel() );

so a unit holding the exp for row 80 is level 65, not 80. `ExpTable::levelForExp` therefore takes
the limit as an argument (`ExpRules::kMaxLevel` by default, and `AddLimitsMaxLevel_LUA` accepts
1..80 so another deployment can pass its own). The regional `GameSysValTable.lua` files use 67 (EU)
and 70 (CN/JP/TW), which is why the limit is a parameter rather than a constant inside the table.

`KUnitInfo` is built the same way the original builds it: `GSUserFunction.cpp:4737` recomputes the
level from the exp total with `CheckLevelUp( 1, m_iEXP )` instead of trusting the stored column, and
:4738-4739 fill `m_nNowBaseLevelEXP` / `m_nNextBaseLevelEXP` with `GetRequireTotalExpbyLevel` -
cumulative totals, not the window inside the level. Sending the stored level, or the in-level
window, both produce a client that disagrees with the server about when the next level starts.

### The inventory answer: order, empty bags, and "only once"

`EGS_GET_MY_INVENTORY_REQ` looks like a simple list reply but the 2014 handler has three properties
that a naive implementation gets wrong, and each of them is visible to the client:

* **The ack comes last.** `GSUserGameCommon.cpp:1158` sends one
  `EGS_SELECT_UNIT_INVENTORY_INFO_NOT` per bag and only then `EGS_GET_MY_INVENTORY_ACK`. A client
  that waits for the ack before building the window sees an empty inventory if the order is
  reversed.
* **Empty bags are not sent at all.** `KInventory::GetSelectUnitInventoryInfo` skips a category when
  it holds no items, so a fresh character receives exactly one frame: the ack.
* **Bank and worn equipment are excluded.** The loop runs `ST_EQUIP` (1) .. `ST_END` (16) but skips
  `ST_BANK` (8) and `ST_E_EQUIP` (9).

A fourth one is easy to miss: `GetInventoryInfo` records the category in `m_setGetMyInventory`
*before* looking at the items, with the comment "인벤정보는 한번만 준다!" - so a second request for
the same unit answers with the ack alone, and the marks are only cleared by `KInventory::Clear`,
which runs when the unit is loaded. `AccountSession::reportedInventoryCategories` mirrors that.

The two flags on the ack (`m_bIsRecommend`, `m_bSharingBank`) come from the account, not from the
constants the rewrite started with, so they live in `AccountOptions`.

### Checking the error codes

Every reply carries a bare integer, so "wrong password" is only meaningful because
`ERR_VERIFY_06` happens to sit at index 12 of a flat C enumeration. Nothing in the rewrite expresses
that arithmetic, so `tools/verify_neterrors.py` re-derives the whole `NetErrorEnum` from
`Common/NetError_def.h` and checks each `kErrX = N, // ERR_Y` in `include/kserver/LoginService.h`
against it. It also refuses a comment that names an error the 2014 header does not define, and it
re-checks the codes against all ten regional enumerations under `Common/Neterror/` - those renumber
the enum for other builds (`SERV_SEPARATE_NET_ERROR`), and a code that drifted would mean one thing
here and something else there.

Current output: `28 error code(s) match the 2014 NetErrorEnum (1367 names in the enumeration, and
the same values in all 10 regional enumerations)`. Which variant the shipped build actually used was
settled by the sources: `SERV_SEPARATE_NET_ERROR` appears only in the per-region `ServerDefine_*.h`
files, never in the shared configuration, so `Common/NetError_def.h` is the one that applied.

### Checking the event ids

A correctly encoded payload is useless if the event id in front of it is wrong - the client learns what
a message *is* from the id alone. `tools/verify_eventids.py` rebuilds the id table from
`EventID_System.h`, `EventID_Client.h` and `EventID_Server.h`, then re-evaluates `EventId.gen.h` by
following its `X = Y + 1` chain (so a reordered or truncated generated block is caught, not trusted).
It cross checks the 29 ids the rewrite spells by name, and asserts that each of the 12 REQ/ACK pairs
the services answer is adjacent, which is how the 2014 headers are written.

Current output: `2909 event id(s) verified against the 2014 headers (2909 total), 29 checked by name,
12 REQ/ACK pairs adjacent`. `TestWire::EventId.idsUsedByTheServicesAreThe2014Numbers` pins the same
absolute numbers inside the test suite, so a regenerated header fails a test even without the script.

## Known gaps

* Rooms, dungeons, PvP matches, parties, guilds, the cash shop transaction path, the UDP relay
  (`UDP_RELAY`, `SP_*` ids, `MTU_MAX = 508`), the game port simulation and the global/monitoring
  tooling are **not** implemented. The game port accepts connections and answers `EGS_CONNECT_REQ`,
  `EGS_VERIFY_ACCOUNT_REQ` and the channel/character packets; everything past unit selection is
  missing.
* `SocialRepository` / `BillingRepository` / `ProgressRepository` are complete against the schema
  but almost nothing calls them yet. The exception is guild membership: `ChannelService::toUnitInfo`
  and `EGS_DELETE_UNIT_ACK` read it so the `KUserGuildInfo` block and the ack's `guildUid` carry the
  character's real guild instead of zeros. Friends, mail, cash items and rankings have no handler.
* Item templates (`ItemTemplet.mdb`, 8 MB of stats) are not imported; `kgame::Character` reads the
  per-item attributes and sockets that are stored in the database, not the template table.
* `Scripts` / `ScriptDBManager` (the `.mdb` script tables) are not read at runtime.
* No reconnection grace, no double-login detection beyond `Accounts.LastLoginAt`, no per-IP limits
  (`CompanyIPTable.lua`), no hud/anti-cheat (`HackingProtectionManager`).

## Testing

```sh
make test                                  # 117 tests, ~2 s
build/linux/bin/knc_tests --filter Db      # one suite
```

| suite | covers |
| ----- | ------ |
| `Wire` | serializer both directions, every primitive, containers, `SerBuffer` compression, envelope offsets, crypto known answers |
| `PacketLayout` | the wire size and field order of every client facing packet, straight off the 2014 `SERIALIZE_DEFINE_PUT` bodies (empty-string baselines, the delete-ack guild uid, the unsigned seal byte) |
| `PacketFlags` | the feature flags that decide *which fields exist*, so a regenerated flag table cannot move bytes without failing a test |
| `PacketBytes` | the bytes the services really emit, walked with a bare `Serializer` in the 2014 field order instead of round-tripped through our own serializer |
| `Net` | framing, SPI-0 handshake over a real socket, sealed frames, ICV mismatch, partial frames, dispatch |
| `Db` | schema apply + idempotency, accounts, units, inventory, transactions, foreign keys |
| `Game` | level curve, stat table, character load/save, inventory categories, nickname/enchant/drop rules |
| `Login` | every `LoginService` reply: version, capacity, unknown account, bad password, block details, id length, new user, state machine |
| `Channel` | server set list/select, channel list, unit list, create/validate/daily limit, select, delete + name reservation, inventory bags, access control |
| `Server` | ini parsing, and the same flow end to end over sockets on ephemeral ports with a temporary database |

`Server` also covers port routing: a login id is accepted from *any* port (the 2014 client repeated
the handshake after being redirected), a channel id is refused until the session has authenticated, and
is answered afterwards - one process, one session, so the handshake that just succeeded is what
unlocks the channel service on that port.

### KUnitInfo carries the table row, and `fromRecord` hides the difference

`OnAccountSelectUnitAck` (`GSUserFunction.cpp:4741-4756`) does not compute the select payload from
scratch - it receives a packet the DB layer already filled and adds to it. All it adds to the stat
block is the raw table row:

```cpp
GetUnitStat( class, level, sUnitStat );   // StatTable, base row only
m_kStat.baseHp   += sUnitStat.baseHp;     // ...and the same for the other four
```

So `KUnitInfo::m_kStat` is the level/class table row and nothing else. Equipment is absent, buffs are
absent, and the numbers move only when the class or the level moves. `ChannelService::toUnitInfo`
therefore reads `Character::baseStat()`, not `totalStat()`.

The rewrite had to spell that out because the two are equal on this path for an unrelated reason:
`Character::fromRecord()` (`src/kgame/Character.cpp:35`) never loads `items_`, so `equipmentStat()`
contributes nothing and `totalStat_ == baseStat_` even for a character wearing a +5000 HP item. A
test that merely compares the payload before and after equipping something passes with either
accessor, i.e. it proves nothing. The test that is actually falsifiable asserts the five fields
against `defaultStatTable().getUnitStat(class, level)`.

`m_kGameStat` is a different field and stays zero here. It is the *in-battle* block: `GetUnitInfo`
(`:4041`) initialises it, the room code overwrites it on entry (`:11828-12054`), and the select path
never touches it. Copying `m_kStat` into it would tell a freshly selected character it is already in
a fight.

### Two KUnitInfo fields the client is told to ignore

`m_nStraightVictories` is zeroed by the server on both paths that hand a `KUnitInfo` to the client -
the unit list (`GSUserGameCommon.cpp:226`) and the select ack (`GSUserFunction.cpp:4715`) - right
after the row arrives from the DB. The column still exists, but echoing it changes what the client
shows, so `toUnitInfo` writes 0.

`m_iPcBangType` is only assigned on the `GetUnitInfo` path (`:4004`); the list and the select ack
leave the value `KUnitInfo::Init()` gave it, which is **-1**, not 0. A character outside a PC bang
therefore reports -1. The same holds for `m_bIsGameBang` (false) and for the P2P `m_wstrIP`/
`m_usPort`, which the original fills from the UDP echo listener via `SetP2PIP` - the rewrite has no
such listener, so they stay empty and the field is documented rather than faked.

### Known deviation: channel and server-set user counts

The original channel list reads each game server's user count from the database
(`GameServer.cpp:1148`, `ChannelLogDBThread.cpp:339`); the server-set list sums
those game-server counts per set (`GSUserSession.cpp`, `EGS_GET_SERVER_SET_DATA_REQ`).
The rewrite has no per-game-server registry: its channel list currently uses the
process-wide connection count for every configured channel, including connections
to other listener roles. This is only an approximation, not per-channel CCU. The
server-set occupancy level still assumes zero users rather than attributing that
same global count to every set. Accurate counts require tracking game-server users
and their channel/set association.

## Known deviation: item socket ids on the wire

`KItemInfo` carries two socket vectors, `m_vecItemSocket` and `m_vecRandomSocket`. Both are lists of
**option ids**, not (type, value) pairs: the loader truncates trailing zeroes from two separate
5-slot `int` arrays (`GSGameDBThread.cpp:9258-9265` under `SERV_BATTLE_FIELD_BOSS`, `:9189-9197` for
the random ones) and `XSLItemManager.cpp:1890-1892` feeds both vectors to the same
`GetAndAddStatRelationLevel()`. Each vector has its own length, and either can be empty.

The rewrite's `ItemSockets` table models a socket as `(SocketType, SocketValue)` - a stat kind and
its amount - because that is what `Character::equipmentStat()` needs to add the bonus
(`Character.cpp:192-203`). It is therefore **not** the 2014 encoding, and `toUnitInfo`'s inventory
notice maps the two columns straight onto the two wire vectors, which is wrong for a client that
reads them as option ids. Fixing it properly means storing the option id alongside the stat pair in
the schema and in `ItemRecord`; that is a schema change, not a mapping tweak, so it is recorded here
instead of being guessed at.

### Known deviation: spirit maximum

The select ack sets `m_iSpiritMax = SiKSpiritTable()->GetMaxSpirit()` (`GSUserFunction.cpp:4750`),
which reads `MAX_SPIRIT` out of `SpiritTable.lua`. That script is not in this tree - only the
`GameServer.exe` builds are - so the rewrite reports 0 for `spiritMax`/`spirit` rather than
inventing a number. Note the open question this leaves: when `SERV_BATTLE_FIELD_SYSTEM` is defined
(as it is, `ServerDefine.h:2768`), the same function keeps spirit in `KUserSpiritManager`
(`:4102 m_kUserSpiritManager.Clear()`, `:3990 GetSpiritMax()`), so the level-1 line above may not be
what the shipping build actually sent. Resolving it needs either the missing script or a decision
about which of the two paths the 2014 deployment used.

### Two things the 2014 server set handlers do that look like bugs

`EGS_GET_SERVER_SET_DATA_REQ` (`GSUserSession.cpp:2086`) classifies the load with the absolute
constants 1500 and 3000 - `iCCU < 1500` is FREE, `< 3000` NORMAL, `< iMaxCCU` BUSY, otherwise FULL.
The percentage thresholds written in the `USER_COUNT_LEVEL` comments only exist in the original as
commented-out code, so the rewrite uses the constants, not a ratio of the capacity.

`EGS_SELECT_SERVER_SET_REQ` (`GSUserSession.cpp:2166`) never reads `m_ServerSetID`; the comment says
the deployment had a single server set. An id that does not exist is therefore not an error - the
answer is always this server's address and master port. The same handler is where a punished account
is stopped: any auth level from -2 to -9 gets `ERR_VERIFY_11` (15, because
`SERV_ACCOUNT_BLOCK_MESSAGE_RENEWAL` is undefined for this build), the block reason, and a
disconnect. -1 is a warning level and still plays.

`Login` and `Channel` need no sockets at all: `LoginService`/`ChannelService` take a
`LoginResponder`, so a test drives packets straight into the service and decodes the replies.
