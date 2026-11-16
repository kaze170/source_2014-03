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
`SERV_*` macro: 568 enabled, 133 disabled. That single set is what every packet layout in this
rewrite was resolved with. If a flag is ever flipped, the `static_assert`s in
`include/kproto/UnitPackets.h` fail the build instead of silently changing the wire format.

## Known gaps

* Rooms, dungeons, PvP matches, parties, guilds, the cash shop transaction path, the UDP relay
  (`UDP_RELAY`, `SP_*` ids, `MTU_MAX = 508`), the game port simulation and the global/monitoring
  tooling are **not** implemented. The game port accepts connections and answers `EGS_CONNECT_REQ`,
  `EGS_VERIFY_ACCOUNT_REQ` and the channel/character packets; everything past unit selection is
  missing.
* `SocialRepository` / `BillingRepository` / `ProgressRepository` are complete against the schema
  but no packet handler calls them yet (friends, mail, guilds, cash items, rankings).
* Item templates (`ItemTemplet.mdb`, 8 MB of stats) are not imported; `kgame::Character` reads the
  per-item attributes and sockets that are stored in the database, not the template table.
* `Scripts` / `ScriptDBManager` (the `.mdb` script tables) are not read at runtime.
* No reconnection grace, no double-login detection beyond `Accounts.LastLoginAt`, no per-IP limits
  (`CompanyIPTable.lua`), no hud/anti-cheat (`HackingProtectionManager`).

## Testing

```sh
make test                                  # 78 tests, ~2 s
build/linux/bin/knc_tests --filter Db      # one suite
```

| suite | covers |
| ----- | ------ |
| `Wire` | serializer both directions, every primitive, containers, `SerBuffer` compression, envelope offsets, crypto known answers |
| `Net` | framing, SPI-0 handshake over a real socket, sealed frames, ICV mismatch, partial frames, dispatch |
| `Db` | schema apply + idempotency, accounts, units, inventory, transactions, foreign keys |
| `Game` | level curve, stat table, character load/save, inventory categories, nickname/enchant/drop rules |
| `Login` | every `LoginService` reply: version, capacity, unknown account, bad password, block details, id length, new user, state machine |
| `Channel` | server set list/select, channel list, unit list, create/validate/daily limit, select, delete + name reservation, inventory bags, access control |
| `Server` | ini parsing, and the same flow end to end over sockets on ephemeral ports with a temporary database |

`Login` and `Channel` need no sockets at all: `LoginService`/`ChannelService` take a
`LoginResponder`, so a test drives packets straight into the service and decodes the replies.
