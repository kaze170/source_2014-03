# Wire protocol

Everything in this file was read out of `../KncWX2Server/Common` and `../KNCSDK/Include/Serializer`,
and is reproduced bit for bit by `kcore`, `ksec` and `kproto`. Where the original used a
feature-flag `#ifdef`, the field is marked with the flag so the layout can be traced back.

## 1. TCP framing

```
[u16 LE totalLen] [KSecureBuffer(totalLen - 2)]
```

`totalLen` includes its own two bytes. The reader (`Session::processOneFrame`) rejects a length below
2 and waits for the rest of a partial frame, which is what the 2014 `KSession::OnRecvCompleted` did.

## 2. KSecureBuffer

```
[SPI  u16 LE]            0 until the handshake has completed
[Seq  u32 LE]            1, 2, 3 ... (only advances while SPI != 0)
[IV   8 bytes]           'A' + rand() % 40, repeated
[payload + padding]      DES-CBC
[ICV  10 bytes]          HMAC-MD5 truncated to 10
```

* Cipher: DES-CBC, 8 byte key, IV as above, plaintext is `payload || 0x00 padding || padLen`.
* ICV: `HMAC-MD5(authKey, SPI || Seq || IV || ciphertext)` truncated to the first 10 bytes.
* Replay window: `lastSequenceNum`; `isAuthentic` rejects a sequence number that was already used.
* SPI 0 keys (the pre-handshake pair, CP949 bytes):
  * auth `c0 d3 bd c3 b7 ce b8 b8`
  * crypto `c7 d8 c4 bf b5 e9 c0 fd`

### Handshake

On accept the server calls `SecurityDatabase::createNewSA()` (random keys in 1..255, seq 1) and
sends `E_ACCEPT_CONNECTION_NOT` (event id **1**) sealed with SPI 0:

```
payload = [u16 spi] [SecurityAssociation]
SecurityAssociation = [8B auth key] [8B crypto key] [u32 seq] [u32 lastSeq]
```

38 bytes in total; the session switches to the new SPI for everything after that packet.

## 3. KEvent envelope

```
[u32    performerID]
[u32    uidCount] [i64 uid ...]      ascending set, MAX_UID_NUM = 2000
[i64    trace[0]]
[i64    trace[1]]
[u16    eventID]
[KSerBuffer payload]
```

Offsets: performer 0-3, uid count 4-7, trace 8-23, event id 24-25, payload from 26. `trace` starts
empty (`-1`). Envelope bytes handled in `kproto/Event.cpp`.

`KSerBuffer`:

```
[u32 length]
if length > 0: [bool compressed] [bytes]     compressed = [u32_le originalSize][zlib level 1]
```

## 4. Serialization rules (KSerializer)

* No type tags, every fixed width number big endian.
* `bool`/`char` are one byte (1/0 for `bool`).
* `std::string`  -> `[u32 byteLen][bytes]`
* `std::wstring` -> `[u32 byteLen][UTF-16LE code units]` (`byteLen == units * 2`). This is little
  endian even though numbers are big endian: the 2014 client memcpy'd `wchar_t` buffers.
* `vector`/`list`/`deque`/`set` -> `[u32 count][elements]`
* `map` -> `[u32 count][key, value ...]` (sorted)
* `pair` -> `first, second`

## 5. Identifiers

Performer ids combine a server class and an object class (`Common/KncSend.h`):

| id | value | meaning |
| -- | ----- | ------- |
| `PI_GS_PROXY` | `0x101` | client proxy on the game port |
| `PI_GS_USER` | `0x202` | a user actor |
| `PI_GS_CHARACTER` | `0x203` | a character actor |
| `PI_CN_USER` | `0x302` | center user |
| `PI_CHANNEL_USER` | `0x402` | channel user |
| `PI_LOGIN_USER` | `0x502` | login user |
| `PI_GLOBAL_USER` | `0x702` | global user |

Event ids are the enumeration order of `EventID_System.h` -> `EventID_Client.h` ->
`EventID_Server.h`; 2,909 ids, generated into `include/kproto/EventId.gen.h` by
`tools/gen_eventids.py`. `E_HEART_BEAT = 0`, `E_ACCEPT_CONNECTION_NOT = 1`,
`E_CONNECTION_LOST_NOT = 2`, `E_UDP_PORT_NOT = 3`.

## 6. Packets implemented today

Field lists are wire order; `<-` marks a flag-gated field that this build carries.

### 6.1 Login port

`EGS_CONNECT_REQ` (from `KEGS_CONNECT_REQ`)

```
wstring version
```

`EGS_CONNECT_ACK`

```
int  ok            NET_OK / ERR_CONNECT_00 (3) / ERR_CONNECT_04 (4) / ERR_CONNECT_05 (5)
int  udpPort       the NC UDP port of the channel (9151 by default)
int  channelId
```

`udpKickOff` (`UDP_CAN_NOT_SEND_USER_KICK`) is not part of this build, so the ack is a flat 12 bytes.

`EGS_VERIFY_ACCOUNT_REQ` (`KUserAuthenticateReq`)

```
bool     debugAuth
wstring  userId
wstring  password
wstring  hwid                 <- SERV_OTP_AUTH (on)
string   machineId            <- SERV_MACHINE_ID_DUPLICATE_CHECK (on)
int      channelingCode
bool     manualLogin          <- SERV_KOG_OTP_VERIFY (on)
```

Six fixed bytes plus the four string prefixes; an empty request is 22 bytes. Each of the following is
resolved *off* in this build, so it is **not** on the wire and must not be read from it:

```
int      channelRandomKey     <- SERV_DLL_LIST_CHECK_BEFOR_LOADING   (off)
byte[12] charServerSn         <- SERV_SERIAL_NUMBER_AVAILABILITY_CHECK_IN_GAMESERVER (off, SERVER_SN bytes)
wstring  clientTime           <- SERV_CHECK_MACHINE_LOCALTIME        (off)
wstring  socketId             <- SERV_COUNTRY_TH                     (off)
bool     steamClient          <- SERV_STEAM                          (off)
bool     aeriaClient          <- SERV_CHANNELING_AERIA               (off)
```

`EGS_VERIFY_ACCOUNT_ACK`

```
int          ok
KAccountInfo accountInfo
char         hackingUserType
wstring      adUrl
bool         sex
u32          age
u32          nexonSn
wstring      clientIp         <- SERV_CLIENT_PUBLIC_IP
u8           channelCode
wstring      channelUserId
```

`eventMoney` (`SERV_EVENT_MONEY`) is resolved off in this build, so the ack ends right there: 71
bytes with all ten string length prefixes zero.

`KAccountInfo`

```
i64      userUid
wstring  id
wstring  name
int      authLevel
bool     internalUser
KAccountOption          { bool playGuide }
KAccountBlockInfo       { wstring endTime; wstring reason }    // SERV_HACKING_USER_CHECK_COUNT off
bool     isRecommend
bool     isGuestUser
wstring  otp
wstring  regDate        <- SERV_CASH_ITEM_LIST
wstring  lastLogin      <- SERV_SECOND_SECURITY
```

44 bytes with every string empty. `channelRandomKey` (`SERV_DLL_LIST_CHECK_BEFOR_LOADING`) is not
part of this build, and the block info stops after `reason`: its `byte type`, `wstring reason2` and
`wstring endDate` only travel with `SERV_HACKING_USER_CHECK_COUNT`.

### 6.2 Channel port

`EGS_GET_SERVER_SET_DATA_ACK`: `int ok`, `vector<KServerSetData>`
(`int serverSetId`, `wstring name`, `char userCountLevel` where 1 = free, 2 = normal, 3 = busy,
4 = full).

`EGS_SELECT_SERVER_SET_REQ`: `int serverSetId`.
`EGS_SELECT_SERVER_SET_ACK`: `int ok`, `wstring ip`, `u16 port`, `KAccountBlockInfo`.

`EGS_GET_CHANNEL_LIST_ACK` (`SERV_CHANNEL_LIST_RENEWAL`): `int serverGroupId`,
`vector<KChannelInfo>`, `map<int, KChannelBonusInfo>`; bonus info is
`{ bool enable; int beginLevel; int endLevel; int perExp; int perEd }` (17 bytes, and the level
range is inclusive).

`KChannelInfo`

```
int      serverUid
int      channelId
wstring  channelName
int      serverGroupId
wstring  ip
u16      masterPort
u16      ncUdpPort
int      maxUser
int      currentUser
int      curPartyCount
int      playPartyCount
```

The channel carries no `bonusInfo` member: `SERV_INTEGRATE_THE_CHANNEL_INFO` is resolved off, so the
bonus table only travels in the `EGS_GET_CHANNEL_LIST_ACK` map above. 40 bytes with both strings
empty.

`EGS_MY_UNIT_AND_INVENTORY_INFO_LIST_ACK`: `int ok`, `int unitSlot`, `vector<KUnitInfo>`,
`bool sharingBank <- SERV_SHARING_BANK_QUEST_CASH` (on in this build; it is *last*, not next to
`ok`).

`EGS_CREATE_UNIT_REQ`: `wstring nickName`, `int unitClass`.
`EGS_CREATE_UNIT_ACK`: `int ok`, `KUnitInfo`, `vector<int> skillSlot`, `wstring enableDate`
(282 bytes with an empty character).

`EGS_DELETE_UNIT_REQ`: `i64 unitUid`, `i64 userUid <- SERV_UNIT_DELETE_EXCEPTION_MODIFY` (on, 16 bytes).
`EGS_DELETE_UNIT_ACK`: `int ok`, `i64 unitUid`, `u32 knmSerialNum`, `int guildUid <- GUILD_TEST` (on),
`wstring nickName`, `u8 level`, `char unitClass` (`SERV_CHAR_LOG`, on). `SERV_UNIT_WAIT_DELETE` is
off, so neither delete date is sent. 26 bytes with an empty nick name.

`EGS_SELECT_UNIT_REQ`: `i64 unitUid`, `set<int> codeEventScriptId <- SERV_CODE_EVENT` (on; the
`map<int, ...> globalEventData` of `SERV_GLOBAL_EVENT_TABLE` is off, so it is absent).
`EGS_SELECT_UNIT_ACK`: `int ok`.

`EGS_GET_MY_INVENTORY_ACK`: `int ok`, `bool isRecommend`, `bool sharingBank`.
`EGS_SELECT_UNIT_INVENTORY_INFO_NOT`: `int slotCategory`, `map<i64, KInventoryItemInfo>`.

### 6.3 KUnitInfo

The full struct is in `include/kproto/UnitPackets.h`; `kproto/UnitPackets.h` also `static_assert`s
the feature flags it was resolved with, so a flag change cannot silently shift the layout.

```
i64   ownerUserUid, char authLevel, i64 unitUid, u32 knmSerialNum, char unitClass, wstring nickName
wstring ip, u16 port
int   ed, u8 level, int exp
int   officialMatchCnt, rating, maxRating, rPoint, aPoint, bool isWinBeforeMatch
char  rank, float kFactor, bool isRedistributionUser, int pastSeasonWin
int   sPoint, csPoint, maxCsPoint, wstring csPointEndDate
int   nowBaseLevelExp, nextBaseLevelExp, straightVictories
KStat stat, KStat gameStat            { 5 x int: HP, atkPhys, atkMag, defPhys, defMag }
KLastPositionInfo lastPos             { int mapId; u8 touchLine; u16 posValue; bool ignore <- SERV_BATTLE_FIELD_BOSS }
KBuffInfo buffInfo                    { KBuffFactor factor; i64 startTime; i64 endTime }
int   win, lose
map<int, KDungeonClearInfo>           { int id; int maxScore; char maxRank; wstring clearTime; bool isNew }
map<int, KTCClearInfo>                { int id; wstring clearTime; bool isNew }
map<int, KDungeonPlayInfo>            { int id; int playTimes; int clearTimes; bool isNew }
map<int, KInventoryItemInfo>
KUnitSkillData
bool  isParty, int spiritMax, int spirit, bool isGameBang
char  pcBangType                      <- SERV_PC_BANG_TYPE
int   titleId                         <- SERV_TITLE_DATA_SIZE (int, not short)
KUserGuildInfo                        { int uid; wstring name; u8 grade; int honor }
wstring lastLoginTime
i64   warpVipEndDate                  <- SERV_ADD_WARP_BUTTON
int   eventQuestClearCount
int   exchangeCount
```

`KBuffFactor` = `{ vector<KBuffBehaviorFactor>; vector<KBuffFinalizerFactor>; KBuffIdentity;
float accumulationMultiplier; u8 accumulationCountNow; int factorId }`, where each
`KBuffBehaviorFactor`/`KBuffFinalizerFactor` is `{ u32 type; vector<float> values }` and
`KBuffIdentity` is `{ u32 templedId; u32 uniqueNum }`.

`KUnitInfo` is 270 bytes with every string and container empty (226 bytes before the buff block,
dungeon play map and VIP tail were added), which `tests/TestPacketLayout.cpp` pins down.

`KInventoryItemInfo` = `{ i64 itemUid; char slotCategory; short slotId; KItemInfo }` (43 bytes empty).
`KItemInfo` (2013-05 item rework enabled) = `{ int itemId; char usageType; int quantity;
short endurance; unsigned char sealData; char enchantLevel; KItemAttributeEnchantInfo (3 chars);
vector<int> itemSocket; vector<int> randomSocket; char itemState; short period;
wstring expirationDate; byte expandedSocketNum }` (32 bytes empty).

`sealData` is the one field whose *signedness* matters even though the size does not: the 2014 header
declares `unsigned char`, and a sealed item stores `>= 100`, so declaring it `char` would let a value
read back negative. `m_iGoldTicketKeyUID` (`SERV_GOLD_TICKET`) is resolved off and is not sent.

## 7. Reply codes

From `Common/NetError_def.h` (an enum starting at `NET_OK = 0`, so the numbers below are its exact
ordinals):

| code | name | used for |
| ---- | ---- | -------- |
| 0 | `NET_OK` | success |
| 3 | `ERR_CONNECT_00` | version mismatch |
| 4 | `ERR_CONNECT_04` | server full |
| 5 | `ERR_CONNECT_05` | maintenance |
| 10 | `ERR_VERIFY_04` | no such account |
| 11 | `ERR_VERIFY_05` | deleted account |
| 12 | `ERR_VERIFY_06` | wrong password |
| 15 | `ERR_VERIFY_11` | blocked account |
| 19 | `ERR_VERIFY_15` | unknown channel / server set |
| 20 | `ERR_VERIFY_16` | malformed id |
| 22/23 | `ERR_NEW_ACCOUNT_03/04` | id / password length |
| 25 | `ERR_CREATE_UNIT_00` | blocked account tried to create |
| 26 | `ERR_CREATE_UNIT_01` | nickname taken |
| 29 | `ERR_CREATE_UNIT_04` | nickname length |
| 30 | `ERR_CREATE_UNIT_05` | no free character slot |
| 33 | `ERR_CREATE_UNIT_08` | nickname not allowed |
| 34 | `ERR_CREATE_UNIT_09` | 4 characters per day |
| 35 | `ERR_SELECT_UNIT_00` | character not found / not yours |
| 36 | `ERR_SELECT_UNIT_01` | bad class |
