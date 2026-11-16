# KncWX2Server_SQLite

A single process rewrite of the 2014 Elsword X2 server (`../KncWX2Server`), on SQLite instead of
fifteen ODBC DSNs, built from the original wire format so a 2014 client can still talk to it.

```
kncserver    one process replacing LoginServer + ChannelServer + GameServer (+ Center/Global)
knc_dbtool   create / migrate / inspect the database
knc_tests    the test runner (105 tests, no external dependencies)
```

## Why this exists

The original deployment was five executables (`LoginServer.exe`, `ChannelServer.exe`,
`GameServer.exe`, `CenterServer.exe`, `GlobalServer.exe`) that talked to each other over the same
KEvent protocol they used for clients, and to Microsoft SQL Server through six ODBC DSNs and a
thread pool of stored procedure callers (`Common/DBThreadManager.cpp`, `Common/ODBC`). Nothing in it
can run on a modern machine without SQL Server, the Win32 build environment of 2010 and the KDNU
dependency tree.

This rewrite keeps the part that defines the game - the encrypted X2 protocol, the event envelope,
the packet layouts and the 2014 data tables - and replaces the transport plumbing:

| 2014                                                | here                                       |
| --------------------------------------------------- | ------------------------------------------ |
| 5 processes, inter-server events over TCP           | 1 process, 1 `Reactor`, direct calls       |
| 6 ODBC DSNs + `DBThreadManager` + stored procedures | one SQLite file + typed repositories       |
| `KSecureBuffer` (DES-CBC + HMAC-MD5-10)             | reimplemented bit exactly (`ksec`)         |
| `KSerializer` (untagged, big endian)                | reimplemented bit exactly (`kcore`)        |
| `ExpTable.lua`, `StatTable.lua`, `ServerDefine.h`   | generated into `.gen.cpp/.h` at build time |

## Layout

```
include/kcore   Types, ByteOrder, Serializer, SerBuffer, Text (UTF-8/16/UHC), Log, IniFile
include/ksec    DES, MD5, KSecureBuffer (handshake, ICV, replay window)
include/kproto  Event (KEvent envelope), EventId.gen.h, FeatureFlags.gen.h, packets
include/knet    Socket, Reactor (poll), Session (framing + seal), Listener
include/kdb     Database (SQLite), Schema, Repositories (accounts/units/inventory/social/...)
include/kgame   ExpTable, StatTable, Character, Rules (nicknames, enchant, drops, exp bonus)
include/kserver Config (ini), LoginService, ChannelService, ServerSession, Server
sql/            001_core .. 005_progress, embedded into the binary by tools/gen_schema.py
config/         knc.ini (one section per former server)
tools/          generators (run with `make gen`), plus dump_original_layouts.py (reads the
                2014 headers) and verify_packets.py (compares every packet layout with them)
tests/          TestWire, TestPacketLayout, TestPacketBytes, TestNet, TestDb,
                TestGame, TestLoginService, TestChannelService, TestServer
third_party/    SQLite 3.54.1 amalgamation, zlib 1.2.3
```

## Build

Linux / MinGW (what CI and this sandbox use):

```sh
make -j2          # server, tools, tests -> build/linux/bin
make test         # runs the 105 tests
make check        # packet layouts vs the 2014 sources, then knc_dbtool init + integrity check
make gen          # regenerate the .gen.* sources from ../KncWX2Server
```

Windows: `build/vcxproj/KncWX2Server_SQLite.sln` (MSVC 2026, `/std:c++latest`, x64). The project files
list the same sources; `third_party/sqlite/sqlite3.c` is compiled as C.

## Run

```sh
./build/linux/bin/kncserver --config config/knc.ini
```

```
[INFO][listen] login listening on tcp/9000
[INFO][listen] channel listening on tcp/9100
[INFO][listen] game listening on tcp/9200
[INFO][server] KncWX2Server started, version 2014031300, db data/knc.db
```

The first start creates `data/knc.db`, applies the schema and seeds the demo accounts `test/test`
and `gm/gm`. Useful flags: `--db <path>`, `--port login=19000,channel=19100`, `--version <tag>`,
`--seconds N`, `--print-config`, `--check-config`, `--log-level debug`, `--help`.

The database tool:

```sh
knc_dbtool --db data/knc.db init          # schema + demo data
knc_dbtool --db data/knc.db info          # version, object counts, account/character counts
knc_dbtool --db data/knc.db check         # integrity_check + foreign_key_check
knc_dbtool --db data/knc.db accounts
knc_dbtool --db data/knc.db create-unit test Rena 2
```

## What a client sees

The 2014 flow, unchanged on the wire:

```
login port (9000)                        channel port (9100)
  E_ACCEPT_CONNECTION_NOT  (SPI 0)         E_ACCEPT_CONNECTION_NOT  (SPI 0)
  EGS_CONNECT_REQ   -> EGS_CONNECT_ACK     EGS_CONNECT_REQ      -> EGS_CONNECT_ACK
  EGS_VERIFY_ACCOUNT_REQ                   EGS_VERIFY_ACCOUNT_REQ
                    -> EGS_VERIFY_ACCOUNT_ACK
                                           EGS_GET_SERVER_SET_DATA_REQ  -> ..._ACK
                                           EGS_SELECT_SERVER_SET_REQ    -> ..._ACK (ip, port)
                                           EGS_GET_CHANNEL_LIST_REQ     -> ..._ACK
                                           EGS_MY_UNIT_AND_INVENTORY_INFO_LIST_REQ -> ..._ACK
                                           EGS_CREATE_UNIT_REQ / EGS_DELETE_UNIT_REQ
                                           EGS_SELECT_UNIT_REQ          -> ..._ACK
                                           EGS_GET_MY_INVENTORY_REQ     -> ..._ACK + 5 notices
```

Reply codes are the 2014 `NetErrorEnum` values (`Common/NetError_def.h`): a version mismatch is
`ERR_CONNECT_00` (3), a full server `ERR_CONNECT_04` (4), a bad password `ERR_VERIFY_06` (12), a
blocked account `ERR_VERIFY_11` (15), a taken nickname `ERR_CREATE_UNIT_01` (26), and so on.

`docs/protocol.md` has the frame layout, the crypto, and the packet-by-packet field lists;
`docs/database.md` has the schema; `docs/porting.md` maps 2014 files to their replacements and lists
what is not implemented yet.

## Status

Implemented and tested: crypto + framing, event envelope, login/verify, server set and channel list,
unit list/create/delete/select, inventory read, account + character persistence, the level curve and
stat tables from the 2014 data, nickname/enchant/drop/exp-bonus rules, the config file, the database
tool.

Not implemented yet: rooms, dungeons, PvP matches, parties, guilds, cash shop transactions, the UDP
relay, the game port's in-field simulation, and the global/server-monitoring tooling. The game port
accepts connections and answers the login packets today; everything past unit selection is a stub.
