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
make test                                  # 105 tests, ~2 s
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
