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
and conditionality may not. It currently reports `42 packet layout(s) match the 2014 serializers`, and
it is how the missing `guildUid` in `EGS_DELETE_UNIT_ACK`, the wrong `channelRandomKey` in the login
request and the unsigned `sealData` were found. It also fails when a type it checks tests a flag the
generated table does not define, which is how those 61 flags were found in the first place.

`tools/dump_original_layouts.py <Type>...` prints the resolved declaration of any original type (and
`--sizes` prints the fixed byte count of each), which is the fastest way to answer "what does the 2014
struct actually contain".

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
make test                                  # 97 tests, ~2 s
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

`Login` and `Channel` need no sockets at all: `LoginService`/`ChannelService` take a
`LoginResponder`, so a test drives packets straight into the service and decodes the replies.
