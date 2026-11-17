# The KncWX2Server wire protocol

This is the hard contract. The 2014 client is a fixed, unmodifiable binary, so
every byte described here is frozen. Anything in the rewrite may change *except*
what is on this page.

Everything below was read out of the original sources in this repository and
then pinned by executable tests. The "verified by" line on each section names
the test that would fail if the behaviour drifted.

---

## 0. Layer cake

```
   KEvent  (knc.proto.event)                 application payload
     |  KSerializer, big-endian
     v
   KSerBuffer  (knc.proto.serbuffer)         optional zlib compression
     |
     v
   KSecureBuffer  (knc.proto.security)       SPI | Seq | IV | DES-CBC | ICV
     |
     v
   length prefix  (knc.proto.framing)        USHORT, little-endian
     |
     v
   TCP
```

Two different byte orders coexist, deliberately:

| layer | byte order |
|---|---|
| frame length prefix | **little-endian** (raw `memcpy` of a `USHORT`) |
| SPI / sequence number in the ESP header | **little-endian** (`KByteStream::Append`) |
| everything written by `KSerializer` | **big-endian** |
| `wchar_t` inside `KSerializer` | **little-endian** (the one exception) |
| `originalSize` inside a compressed `KSerBuffer` | **little-endian** |

Getting any of these backwards produces a server that completes the handshake
and then silently corrupts every string. The asymmetries are tested explicitly.

---

## 1. TCP framing

```
+------------------+-------------------------------------+
| USHORT len  (LE) | KSecureBuffer frame  (len - 2 bytes)|
+------------------+-------------------------------------+
```

`len` is the **total** size *including its own two bytes*.
`MAX_PACKET_SIZE` is `32768` (`Common/Socket/Overlapped.h`).

*Verified by:* `tests/test_proto.cpp`, "the length prefix is total size,
little-endian" and the dribble/multi-frame cases.

### Defects fixed here

The legacy receive loop (`KSkSession::OnRecvCompleted`) has two remotely
triggerable bugs, both reproduced as tests before being fixed:

1. **`len < 2` is accepted.** `usPacketLen - sizeof(usPacketLen)` is `size_t`
   arithmetic, so `len == 0` becomes `SIZE_MAX - 1` and is handed straight to
   `KByteStream::assign(ptr, nBytes)`. And because `m_dwLeft -= 0` leaves the
   loop state untouched, the thread spins forever on the same two bytes. Two
   zero bytes from any peer are enough to pin a core.
2. **The oversize check sits in the wrong branch.** It only runs when the frame
   has *not* fully arrived, so an oversize-but-buffered frame is processed
   rather than rejected.

`FrameReader` validates the declared length before using it for anything and
never advances by zero.

---

## 2. The security envelope (`KSecureBuffer`)

A cut-down IPsec ESP.

```
+--------+-----------+--------+-----------------------------+---------+
| SPI(2) | SeqNum(4) | IV(8)  | DES-CBC( payload|pad|padLen)| ICV(10) |
|   LE   |    LE     |        |                             |         |
+--------+-----------+--------+-----------------------------+---------+
 \_________________ ICV input (HMAC-MD5, truncated to 10) __/
```

| parameter | value | source |
|---|---|---|
| cipher | DES-CBC, no padding mode | `CryptoPP::CBC_Mode<DES>::ProcessData` |
| key size | 8 bytes | `CRYPTO_KEY_SIZE` |
| IV size | 8 bytes | `GetIVSize()` |
| block size | 8 bytes | `GetBlockSize()` |
| MAC | HMAC-MD5 truncated to **10 bytes** | `GetICVSize()`, `TruncatedFinal` |
| auth key size | 8 bytes | `AUTH_KEY_SIZE` |
| minimum frame | **25 bytes** | `IsValidSize()` |

### Padding (RFC 2406 style)

```
padLen = 8 - ((payloadLen + 1) % 8)       -> always 1..8, never 0
pad    = 01 02 03 ... padLen
trailer= padLen                            (one byte)
```

`GetMaxExtraPadBlocks()` returns 1, but the random source is hard-coded off
(`unsigned char nRand = 0;`), so `nRandBlocks` is always 0 and extra blocks are
never emitted. Our reader validates the pad *pattern* rather than assuming a
length, so it would still accept them.

*Verified by:* "padding is exactly 1..padLen followed by padLen", "a payload
that is already 7 mod 8 gets a single pad byte", and a round-trip across every
payload length 0..24.

### Verification order

Legacy `IsAuthentic_notRWM` checks SPI → size → **ICV** → sequence, and only
then decrypts. We keep that order: the ICV is checked before the ciphertext is
touched, so a forged frame never reaches the DES code or the padding parser.
Comparison is constant-time.

*Verified by:* "a wrong key fails the ICV, never the padding", and a test that
flips every single bit of a sealed frame and requires all of them to be
rejected.

### Sequence numbers

`IsValidSequenceNum_notRWM` (the TCP variant — no replay-window mask):

* `seq == 0` → reject.
* `seq > last` → accept, record.
* `seq == last` → reject.
* `seq < last` → reject, **unless** `last == 0xFFFFFFFF`, which admits one wrap.

The sender increments **only when SPI != 0** (`if( !IsDefaultSPIndex() )`), so
the bootstrap SA keeps sequence 1 forever and the handshake frame is
deterministic. The receiver likewise skips the check while on SPI 0.
`KSession` leaves `m_bCheckSequenceNum` **false** by default.

*Verified by:* "sequence numbers advance only when SPI != 0", "sequence
checking rejects a replayed frame", "sequence checking stays off while on the
bootstrap SA".

### The SPI field is informational

```cpp
// 060302. florist.  (the SPIndex recorded by client and server differ,
//                    so make it work anyway)
//return GetSADB().Find( nSPIndex );   // <- the packet's SPI, commented out
return GetSADB().Find( m_nSPIndex );   // <- the locally stored SPI
```

Each side decrypts with *its own* session SA and ignores the SPI on the wire.
Enforcing it would lock out every shipped client. `peek_spi()` exists for
logging only.

*Verified by:* "the SPI on the wire is read but never enforced".

---

## 3. Key exchange

```
client connects
   |
   |  server: CreateNewSA() -> (spiNew, saNew)
   |<------- E_ACCEPT_CONNECTION_NOT, sealed under SPI 0 ------------
   |         body: USHORT spiNew (BIG-endian)  ||  KSecurityAssociation
   |
   |  server then switches to spiNew/saNew
   |  client inserts saNew under spiNew and sets m_bAuthKeyRecved
   |
   |-------- everything afterwards sealed under spiNew --------------
```

Note the SPI appears **twice with different byte orders** in the same
handshake: big-endian inside the serialized body, little-endian in the ESP
header. That is not a transcription error.

`KSecurityAssociation` serializes as:

```
DWORD authKeyLen (BE) | authKey bytes
DWORD cryptoKeyLen(BE)| cryptoKey bytes
DWORD sequenceNum (BE)
DWORD lastSequenceNum (BE)
DWORD replayWindowMask (BE)
```

The client polls `m_bAuthKeyRecved` every 20 ms for up to
`SECURE_KEY_WAITING_TIME = 5000 ms`, then calls `ReserveDestroy()`.

*Verified by:* "full handshake: SPI 0 carries the session SA", "the SPI appears
big-endian in the body, little-endian in the header".

### The bootstrap SA (SPI 0)

Both sides have these eight-byte keys compiled in. They are the first 8 bytes of
CP949-encoded Korean string literals in
`KNCSDK/Include/KncSecurity/SecurityAssociation.cpp`:

| key | bytes | literal |
|---|---|---|
| auth | `C0 D3 BD C3 B7 CE B8 B8` | `"임시로만드는키"` |
| crypto | `C7 D8 C4 BF B5 E9 C0 FD` | `"해커들절대모를키"` |

They are hard-coded as **hex** in `security.cppm`, never as string literals: a
UTF-8 build of the original line would silently produce different bytes and
lock out every client.

*Verified by:* "bootstrap SA keys are the exact CP949 bytes".

> These keys are public — they ship inside the client. SPI 0 provides no
> confidentiality against anyone who has looked at the binary. It protects
> exactly one message, and only against passive observers who have not. That is
> the original design and it cannot be changed without changing the client.

---

## 4. `KSerializer` encoding

Tagless. `BeginWriting(&buf)` defaults `tagging` to false and every call site on
the wire path uses the default, so the tagged variant is dead code and is
deliberately not implemented.

| type | encoding |
|---|---|
| `char`, `UCHAR` | 1 raw byte |
| `bool` | 1 byte; writes 1/0, **reads as `val == 1`** |
| `wchar_t` | 2 bytes, **native little-endian, not swapped** |
| `short`, `USHORT` | 2 bytes big-endian |
| `int`, `DWORD`, `UINT` | 4 bytes big-endian |
| `INT64`, `UINT64` | 8 bytes big-endian |
| `float`, `double` | IEEE-754 with the bytes reversed |
| `std::string` | `DWORD` byte count (BE) + raw bytes |
| `std::wstring` | `DWORD` **byte** count (BE, = chars × 2) + UTF-16LE units |
| `PutRaw` | bytes only, **no length prefix** |
| vector / list / deque / set / multiset | `DWORD` count (BE) + elements |
| map / multimap | `DWORD` count (BE) + pairs |
| `pair` | first then second, **no count** |

Two traps:

* **`wchar_t` is not byte-swapped.** `Put(wchar_t)` calls `WriteNumber()`
  without the `HTON_` conversion that `Put(USHORT)` applies. Same width,
  opposite order. Every character name and chat line depends on it.
* **`bool` reads as `val == 1`, not `val != 0`.** A byte of `2` deserializes to
  `false`. Some legacy senders really do emit values outside {0,1}.

*Verified by:* `tests/test_serializer.cpp` — 51 checks including
"wchar_t and ushort disagree for the same value" and
"bool reads as (byte == 1), not (byte != 0)".

### Hardening

The legacy `Get()` returns `false` on underflow and most callers ignore the
result; container reads trusted a 32-bit count and looped. We throw
`ProtocolError` instead, and cap:

| cap | value | why |
|---|---|---|
| `kMaxContainerElements` | 2²⁰ | the largest legitimate container is `MAX_UID_NUM = 2000` |
| `kMaxBlobBytes` | 32 MiB | above any real string |
| element count vs. bytes remaining | structural | every element costs ≥ 1 byte, so a larger count is provably a lie |
| `kMaxDecompressedBytes` | 64 MiB | the legacy `UnCompress()` resized to an attacker-supplied `DWORD` |

---

## 5. `KSerBuffer`

```
DWORD length (BE)
if length > 0:
    bool  compressed   (1 byte)
    byte  data[length]
```

When `compressed` is set, `data` is **not** a bare zlib stream:

```
DWORD originalSize   (NATIVE little-endian - raw Write(), not byte-swapped)
byte  zlibStream[]   (compress2(..., level 1): 0x78 header, Adler-32 trailer)
```

Mixed endianness inside one structure. It is real, and it is tested.

*Verified by:* "compressed body starts with a NATIVE-endian original size".

---

## 6. `KEvent`

```
KPerformerInfo:
    DWORD           performerId       (BE)
    DWORD           uidCount          (BE)
    INT64[uidCount] uids, ascending   (BE, std::set order)
INT64   trace[0]                      (BE)
INT64   trace[1]                      (BE)
USHORT  eventId                       (BE)
KSerBuffer payload
```

* An empty trace is `{-1, -1}`. `GetLastSenderUID()` returns `trace[1]` unless
  it is `-1`, in which case `trace[0]`.
* `KPerformerInfo::MAX_UID_NUM == 2000`.
* `m_usFromType` (under `SERV_KEVENT_FROM`) is a local annotation and is **not**
  serialized.
* The receiver calls `m_kbuff.UnCompress()` unconditionally after reading.

An event with no payload is exactly 26 bytes.

*Verified by:* "an empty event has the documented 26-byte layout",
"uids serialize ascending after a BE count", "trace push/pop follows
GetLastSenderUID".

### Heartbeat

Outbound/proxy sessions send `E_HEART_BEAT` (id 0) every >15 s once
`m_bAuthKeyRecved`. An inbound session silent for >60 s is dropped with
`eSIColDR_Heart_Beat`.

---

## 7. Event ids

One flat `unsigned short` space, 0..3385, built by including three X-macro
headers in order:

| band | range | header |
|---|---|---|
| system | 0 – 20 | `EventID_System.h` |
| client | 21 – 1324 | `EventID_Client.h` |
| server | 1325 – 3385 | `EventID_Server.h` |

There are **no `#ifdef`s inside the enum bodies**, so the numbering is
region-invariant — a regional build changes packet *fields*, never ids. The
`*_BEGIN` enumerators live in `Event.h` rather than the headers, so they consume
no slot, which is why `index == value` holds for all 3386 rows and why
`ms_szEventID[]` can be indexed directly.

`EVENT_EVENTID_SENTINEL` (3385) both marks "unknown" and sizes the arrays in
`Common/PacketMornitoring/PacketMornitoring.h`, so the space must stay
contiguous.

Anchors:

| name | id |
|---|---|
| `E_HEART_BEAT` | 0 |
| `E_ACCEPT_CONNECTION_NOT` | 1 |
| `E_RESERVE_DESTROY` | 6 |
| `E_SYSTEM_EVENT_ID_END` | 20 |
| `EGS_GOOD_JOB_1_REQ` | 21 |
| `EGS_CONNECT_REQ` / `_ACK` | 26 / 27 |
| `EGS_VERIFY_ACCOUNT_REQ` / `_ACK` | 30 / 31 |
| `EGS_CLIENT_EVENT_ID_END` | 1324 |
| `EVENT_EVENTID_SENTINEL` | 3385 |

The table in `src/proto/event_ids.inc` is **generated** by
`tools/gen_event_ids.py` and then **cross-verified by a second, independent
method**: a throwaway program compiles the original headers with a real C++
preprocessor and dumps `(index, name, value)` for all 3386 rows. The regex
generator and the compiler agree on every row. See
`docs/VERIFICATION.md`.

*Verified by:* `tests/test_event_ids.cpp` — 36 checks.

---

## 8. Deliberate deviations

Everything here is invisible to the client: the peer either cannot observe it,
or only ever echoes the value back.

| # | legacy | now | why |
|---|---|---|---|
| 1 | IV = 8 copies of `'A' + rand() % 40`, unseeded CRT `rand()` | 8 CSPRNG bytes | 40 possible IVs from a fully predictable sequence; with a fixed key, equal plaintext blocks gave equal ciphertext across sessions. The IV travels in clear and the peer just reads it. |
| 2 | SA keys from a default-seeded function-local `boost::mt19937`, bytes drawn from `[1,255]` | 8 CSPRNG bytes, full `[0,255]` | every server produced the identical key stream from boot. The peer stores whatever it is handed. |
| 3 | ICV compared with `operator!=` | constant-time compare | byte-at-a-time timing oracle on a 10-byte MAC. |
| 4 | frame length `< 2` accepted → infinite loop + wild read | rejected | see §1. |
| 5 | oversize frame processed when it happened to fit | rejected on the length prefix | see §1. |
| 6 | container/blob/decompress sizes trusted from the wire | capped, and checked against bytes remaining | one packet could allocate 4 GiB. |
| 7 | serializer underflow returns `false`, usually ignored | throws `ProtocolError`, session dropped | silent truncation became a parse that "succeeded" with garbage. |
| 8 | ciphertext length not checked against the block size | rejected | fed straight into `ProcessData` otherwise. |
| 9 | global `KSADatabase` map + `CRITICAL_SECTION` per packet, entries never erased | SA lives in the session | pure lock contention, plus an unbounded leak. |
| 10 | any event id from a client is dispatched | `is_client_originable()` rejects ids > 1324 at the edge | a crafted client could inject server-to-server events (party, guild, DB replies). The legacy guard existed only under `SERV_KEVENT_FROM`. |
| 11 | `KEGIANT_BILLING_CONSUME_ACK` and `KELOG_ITEM_TRADE_ERROR_NOT` have `SERIALIZE_DEFINE_GET` bodies that call `PUT` | the reader reads the fields the writer wrote | see below. |

Deviation 10 is the only one a *malicious* client can notice. Nothing an honest
client does is affected.

### Deviation 11 in detail

`PUT(x)` expands to `ks.Put(obj.x)` and `GET(x)` to `ks.Get(obj.x)`. In a
`SERIALIZE_DEFINE_GET` body `obj` is a non-const reference, so writing `PUT`
where `GET` was meant still compiles. At run time the "reader" writes into the
serializer and consumes nothing from the stream, and the destination object
keeps its default-constructed value.

Two bodies in `Common/OnlyGlobal/Packet/ServerPacket_Global.cpp` do this:

| packet | damage |
|---|---|
| `KEGIANT_BILLING_CONSUME_ACK` | all six fields; the packet was effectively write-only |
| `KELOG_ITEM_TRADE_ERROR_NOT` | the trailing `m_wsrtRegDate` only; its bytes were left unconsumed |

Both are server-internal (Giant billing, log DB) and never reach a game
client, and the rewrite is a single process with no legacy peer on the far
end, so the `PUT` body is taken as the authoritative wire format and a correct
reader is generated from it.

These two are an allowlist in `tools/gen_packets.py`
(`KNOWN_GET_BODY_DEFECTS`), not a general rule: any *other* packet whose PUT
and GET disagree is a hard error that stops generation. The allowlist is also
checked for staleness, so an entry that stops being needed fails the build
rather than rotting.

### Not changed, though it is tempting

* **DES and MD5 stay.** Both are broken by modern standards. Both are in the
  client. Changing either means changing the client, which is out of scope.
* **`bool` still reads as `val == 1`.** It looks like a bug. Senders depend on
  it.
* **The `wchar_t` byte-order exception stays.** Obviously.
* **Padding is still the predictable `01 02 03 ...` series.** RFC 2406 specifies
  it, and the client validates it.

## 9. Packet layouts

The 1953 serialisable structs of the TW/HK build are not written by hand. They
are generated from the original headers by `tools/gen_packets.py` into
`src/proto/packets/*.inc`, and exposed as `knc.proto.packets`.

Two things decide a layout, and they live in different files:

* **order** - the `SERIALIZE_DEFINE_PUT` body in `Common/*Packet.cpp`. This is
  the wire order, and it is not always the member declaration order.
* **types** - the struct body in `Common/*Packet.h`.

Both are shot through with regional `#ifdef`s (742 macros are live for TW/HK),
so the generator runs the real C preprocessor rather than trying to evaluate
them itself.

| | TW/HK |
|---|---|
| serialisable types | 1953 |
| serialised fields | 7537 |
| from `Common/*Packet.cpp` | 1743 |
| from `Common/OnlyGlobal/Packet/*_Global.cpp` | 210 |
| legacy types defined across *all* regions | 2410 |
| PUT/GET field-order disagreements | 2, both listed above |
| `PUTARRAY`/`GETARRAY` uses | 0 - the macros are dead |

`OnlyGlobal/Packet/*_Global.cpp` are separate translation units that nothing
`#include`s. Missing them costs 210 types and, worse, gives the wrong
definition of `KBillProductInfo`: the copy in `CommonPacket.cpp` sits behind
`#ifndef SERV_GLOBAL_BILLING` and is dead for this region.

### Type mapping

| legacy | generated | note |
|---|---|---|
| `char`, `BYTE`, `UCHAR`, `u_char`, `byte`, `UINT8` | `char` / `std::uint8_t` | 1 raw byte |
| `bool` | `bool` | writes 1/0, reads `== 1` |
| `short`, `USHORT`, `u_short` | `std::int16_t` / `std::uint16_t` | 2 bytes BE |
| `int`, `UINT`, `DWORD`, `u_int`, `UINT32`, `unsigned long` | `std::int32_t` / `std::uint32_t` | 4 bytes BE |
| `__int64`, `INT64`, `UidType`, `__time64_t` | `std::int64_t` | 8 bytes BE |
| `float`, `double` | `float`, `double` | IEEE-754 BE |
| `std::string` | `std::string` | DWORD byte count + raw |
| `std::wstring` | `std::u16string` | DWORD **byte** count + UTF-16**LE** units |
| `std::vector/list/deque/set/map` | same | DWORD count + elements |
| `T m_a[N]` | `std::array<T, N>` | **no count** - element by element |

`unsigned long` deserves its own line. The protocol is Win32/LLP64, where
`long` is 4 bytes. A literal port on an LP64 host would widen every `DWORD` to
8 bytes and silently double the field. `tests/test_packets.cpp` pins this.

Fixed C arrays are the one shape that is *not* length-prefixed: the legacy
`PUT` body names each index explicitly, so the elements go out bare.
`Codec<std::array<...>>` is deliberately a `static_assert` failure so that
hand-written code cannot accidentally frame one as a container.

## 10. Which packet belongs to which event

An event id and a packet struct are bound by a naming convention —
`DECL_PACKET(id)` expands to `struct Kid` — and conventions have exceptions.
`src/proto/event_packets.inc` records the binding for all 3386 ids, in four
buckets:

| bucket | ids | meaning |
|---|---|---|
| `kStruct` | 1958 | a generated struct carries the payload |
| `kScalar` | 41 | the payload is a **bare value**, not a struct |
| `kNoStruct` | 1268 | nothing named `Kid` is serialisable in any region |
| `kOtherRegion` | 119 | serialisable, but not in this region's build |

The decisive question for every bucket is *can this build serialise it*, never
*is the name declared*. Those differ: `KDBE_CCU_AGENT_ACK` is declared
unconditionally in `ServerPacket.h` while its `PUT`/`GET` bodies sit behind
`SERV_KOG_STATISTICS`, and `KELG_CHAT_OBSERVATION_NOT` is declared under
`SERV_GLOBAL_AUTH` but needs `SERV_INFOSERVER_ADD_WORK` as well for its
bodies. Both are declared under TW/HK and neither can go on the wire.

### kScalar — the easy one to miss

41 ids have no `DECL_PACKET` anywhere, yet carry a payload:

```cpp
typedef __int64       KEGS_ACCEPT_FRIEND_REQ;
typedef bool          KEGS_CALL_MY_LOVER_AGREE_RESULT_NOT;
typedef float         KEGS_REMAINING_PLAY_TIME_NOT;
typedef std::wstring  /* ... */;
```

The entire body is one value, written exactly as that type would be written
inside a struct — eight big-endian bytes for the int64, one byte for the bool,
no count and no framing. `PacketOf<Ev::EGS_ACCEPT_FRIEND_REQ>` is
`std::int64_t`, and `decode_packet` handles it through the same `Codec<T>`
path as a struct.

### kNoStruct does not mean "no payload"

`E_ACCEPT_CONNECTION_NOT` is in this bucket, and it carries the SPI and the
entire security association — serialised by hand in the handshake because it
has to be written before any SA exists (§3). Treat `kNoStruct` as "no
generated struct"; whether a payload exists is a separate question the table
does not answer.

### The 119 in `kOtherRegion` are an edge concern

Event numbering is region invariant, so an id belonging to a feature this
build does not have still occupies its slot. **38 of them sit in the
client-originable band**: a client can name them and nothing here can parse
them. `is_client_originable()` lets those through because it only checks the
band; `is_acceptable_from_client()` refuses them. Inbound client traffic
should use the latter.
