# How the rewrite is verified

The target toolchain is VC++26 on Windows, which is not available here. That
does not make the work unverifiable — it makes the *verification strategy* part
of the design.

## The dual-build contract

Every `.cppm` in `src/` is simultaneously:

* a real C++26 module interface unit, for the MSVC ship build; and
* a `#pragma once` header, when `KNC_CLASSIC` is defined.

The preamble that makes this work is documented in `src/knc_prelude.hpp` and is
identical in every module:

```cpp
#ifdef KNC_CLASSIC
#  pragma once
#else
module;
#endif

#include "../knc_prelude.hpp"

#ifdef KNC_CLASSIC
#  include "dependency.cppm"
#else
export module knc.something;
import knc.dependency;
#endif
```

`KNC_EXPORT` expands to `export` in module mode and to nothing in header mode.

The point: **the code under test is the code that ships**, byte for byte. Only
the module plumbing differs — no shims, no reimplementation, no "test version"
of an algorithm. A conformance run on Linux/g++ therefore says something real
about the MSVC build.

## Two layers of verification

### Layer 1 — the test suite (`tests/`)

Pins behaviour against published vectors and literal byte strings.

| area | method | result |
|---|---|---|
| DES | 6 FIPS 46-3 / NIST known-answer vectors, encrypt **and** decrypt | pass |
| DES-CBC | chaining checked against manual block-by-block XOR+ECB | pass |
| MD5 | all 7 RFC 1321 test vectors | pass |
| MD5 streaming | one-shot vs. 9 different chunk sizes | pass |
| HMAC-MD5 | 4 RFC 2202 vectors | pass |
| HMAC truncation | the 10-byte ICV is a prefix of the full tag | pass |
| `KSerializer` | byte strings written out literally, not derived from the code | pass |
| ESP envelope | round-trip at every payload length 0..24; every single-bit flip of a sealed frame must be rejected | pass |
| framing | byte-at-a-time delivery, multi-frame reads, every malformed length | pass |
| event ids | structural invariants over all 3386 rows | pass |
| packet layouts | hand-written byte vectors for the login handshake; container, fixed-array and nested-struct shapes; the two repaired `GET` bodies | pass |
| event→packet binding | table internally consistent with `event_ids.inc`; bucket counts; scalar payloads; the client-edge filter only ever narrows | pass |
| session | a two-party login conversation driven byte by byte; handshake ordering; what the edge refuses; liveness | pass |

Totals: **7 suites, 2575 checks, 0 failures.**

### Layer 2 — differential cross-checks (`tools/crosscheck/`)

Layer 1 has a blind spot: a test I wrote can share a misunderstanding with the
code I wrote. So each primitive is additionally checked against **someone
else's implementation**, on hundreds of random inputs.

| script | checks | against | result |
|---|---|---|---|
| `check_md5.sh` | MD5, HMAC-MD5, 10-byte truncation | Python `hashlib` / `hmac` | 1200 comparisons, 0 mismatches |
| `check_des.sh` | DES-CBC, encrypt and decrypt | `pyDes` (independent pure-Python DES) | 600 comparisons, 0 mismatches |
| `check_envelope.sh` | the **complete** `KSecureBuffer` frame | a Python reference written from the legacy C++ sources | 200 frames, 0 mismatches |
| `check_event_ids.sh` | all 3386 event ids | a real C++ preprocessor on the original headers | 3386 rows, 0 mismatches |
| `check_packets.sh` | all 1953 packet layouts — every field, type and position | a real C++ **compiler** running the original serialization macros | 1953 types / 7537 fields, 0 mismatches |
| `check_event_packets.sh` | all 3386 event→packet bindings | `static_assert` against the original headers, plus the **symbol tables** of the legacy code compiled for 10 regions | 3386 bindings, 0 mismatches |
| `check_handshake.sh` | a whole login conversation, both directions | an independent Python implementation of the entire stack | 26 checks, 0 mismatches |

`check_envelope.sh` and `check_packets.sh` are the two that earn their keep.
DES can be right and HMAC can be right while the composition is still wrong. DES can be right and
HMAC can be right while the composition is still wrong — ICV computed over the
wrong span, sequence number in the wrong byte order, padding counted from the
wrong base. Its Python reference is transcribed directly from
`KSecureBuffer::Create_notRWM` and `GeneratePadding`, with no reference to the
new C++ module, so byte-identical output across 200 random frames (payload
lengths 0..96, random keys, IVs, SPIs and sequence numbers) covers the layout
as a whole.

```
test_crypto      41 checks
test_serializer  51 checks
test_proto     2220 checks
test_event_ids   36 checks
test_packets     48 checks
test_dispatch    80 checks
test_session     99 checks
```

Run them with:

```sh
cd KncWX2ServerNG
tests/run_tests.sh
```

or through CMake:

```sh
cmake -S . -B build -DKNCNG_CLASSIC_MODE=ON
cmake --build build -j
ctest --test-dir build --output-on-failure
```

Run them with:

```sh
python3 -m venv .venv && .venv/bin/pip install pyDes
PYTHON=.venv/bin/python tools/crosscheck/check_md5.sh
PYTHON=.venv/bin/python tools/crosscheck/check_des.sh
PYTHON=.venv/bin/python tools/crosscheck/check_envelope.sh
PYTHON=.venv/bin/python tools/crosscheck/check_event_ids.sh
LEGACY_COMMON=../KncWX2Server/Common tools/crosscheck/check_packets.sh
LEGACY_COMMON=../KncWX2Server/Common tools/crosscheck/check_event_packets.sh
tools/crosscheck/check_handshake.sh        # no legacy tree needed
```

## Cross-verification of the event-id table

This table is the single widest piece of client-visible contract: 3386 ids that
must each keep the exact value the 2014 client was compiled against. One
inserted line shifts everything after it, and the failure mode is a server that
connects and then misinterprets every packet.

It is therefore extracted **twice, by unrelated mechanisms**:

1. `tools/gen_event_ids.py` parses the original `EventID_*.h` headers with
   Python regexes, resolving the `_ENUM` X-macro and the nested
   `OnlyGlobal/` includes by hand. Output: `src/proto/event_ids.inc`.
2. A throwaway program reproduces the exact three-enum layout from
   `Common/Event.h` and the concatenated name table from `Common/Event.cpp`,
   and lets a **real C++ preprocessor and compiler** expand them:

   ```cpp
   #define _ENUM( id ) id,
   enum ENUM_SYSTEM_EVENT_ID { #include "EventID_System.h" };
   enum ENUM_CLIENT_EVENT_ID { E_CLIENT_EVENT_ID_BEGIN = E_SYSTEM_EVENT_ID_END,
                               #include "EventID_Client.h" };
   enum ENUM_SERVER_EVENT_ID { E_SERVER_EVENT_ID_BEGIN = EGS_CLIENT_EVENT_ID_END,
                               #include "EventID_Server.h" };
   ```

   then prints `(index, name, value)` for every row.

The two outputs are compared row by row. They agree on **all 3386 rows**, and
`index == value` holds throughout.

The compiler route needs two mechanical fixups, both audited:

* `Common/EventID_*.h` include `"OnlyGlobal/EventID_*_global.h"` with a
  lowercase `g`, while the files on disk are `..._Global.h`. That is a
  Windows-only case-insensitivity accident. It is resolved with copies under
  the expected names — **never** by renaming anything in the legacy tree.
* Row 0 is written `_ENUM( E_HEART_BEAT = 0 )`, whose initialiser makes the
  `(unsigned)(id)` value-table trick ill-formed. The `= 0` is dropped in the
  throwaway copy. Row 0 of the first enum is 0 with or without it, and the diff
  of that transform is printed as part of the procedure.

`tests/test_event_ids.cpp` then re-asserts the structural invariants
(index == value, unique names, band boundaries, 17 anchor values, the five
`EGS_GOOD_JOB_n_REQ` placeholders at 21..25) at build time.

## Cross-verification of the packet table

Same problem as the event ids, one order of magnitude larger: 1953 structs,
7537 fields, and a field in the wrong place is a server that connects, logs
in, and then misreads everything.

`tools/gen_packets.py` builds the table by running the **C preprocessor** over
the legacy sources with marker macros and parsing the marked-up text with
regular expressions. That resolves the regional `#ifdef`s correctly by
construction, but the regexes are mine, and a subtly wrong regex produces a
subtly wrong table that every test written against it will happily confirm.

`tools/crosscheck/check_packets.sh` therefore never looks at text. It compiles
the original, unmodified

```
Common/{Common,Client,Server}Packet.cpp
Common/OnlyGlobal/Packet/{Common,Client,Server}Packet_Global.cpp
```

with the original `PUT` / `GET` / `DECL_DATA` / `SERIALIZE_DEFINE_*` macros,
copied verbatim out of `KNCSDK/Include/Serializer/`. The only substitution is
the `KSerializer` those macros call:

```cpp
struct KSerializer {
    std::vector<std::string>* log;
    template <typename T> bool Put(const T&) { rec<T>(); return true; }
    template <typename T> bool Get(T&)       { rec<T>(); return true; }
    template <typename T> void rec() { log->push_back(demangle(typeid(T).name())); }
};
```

Field **order** then comes from a real C++ compiler walking the real
`return PUT(a) && PUT(b) && ...;` chains, and field **types** come from that
compiler's own overload resolution. No regex, no text parsing, no
hand-maintained type table — nothing shared with the generator except the list
of type names, which is separately checked against a raw grep of the sources.

Result: **1953 types, 7537 field signatures, 0 mismatches.**

### Reconciling the two type vocabularies

The generator emits the fixed-width name it chose; the compiler reports the
platform spelling it resolved. Both sides are reduced to wire-equivalence
classes first. Every collapse is deliberate:

| collapse | why it is safe |
|---|---|
| signedness dropped for integers | the serializer emits identical bytes for `int` and `unsigned int` of equal width, so it cannot be a wire difference |
| `long` / `unsigned long` → **32** bits | the protocol is Win32/LLP64 where `long` is 4 bytes; an LP64 host reports 8, and believing it would silently double the field. This is the one place the check overrides the compiler. |
| `wchar_t` and `char16_t` strings → one code | `wchar_t` is 2 bytes on Win32 and 4 on Linux; the serializer writes one UTF-16 code unit per character either way |
| allocator, comparator, and the `const` on a map key erased | C++ bookkeeping with no wire presence |

Width and kind are never collapsed, so a 2-vs-4 byte or an int-vs-float
mix-up still fails the diff.

### What the generator refuses to do

Generation aborts rather than guessing. All of these are hard errors:

* a `SERIALIZE_DEFINE_PUT` with no matching `GET`, or vice versa
* `PUT` order ≠ `GET` order, outside the two-entry allowlist
* a field in a `PUT` body with no matching struct member
* a member type that maps to no known codec
* a fixed array serialised as anything other than indices `0..N-1`
* a cycle in the struct dependency graph
* an allowlist entry that is no longer needed

Each of the first four fired at least once during development and caught a
real problem — including the missing `OnlyGlobal/Packet/*_Global.cpp`
translation units (210 types, and the wrong `KBillProductInfo`) and the two
`GET` bodies that call `PUT`.

## Cross-verification of the event -> packet binding

Binding event 27 to `KEGS_CONNECT_ACK` is string concatenation plus a walk
over `typedef` chains. It works for 1957 rows and then quietly does something
else for the 1958th — `EGS_VERIFY_ACCOUNT_REQ` is a typedef of
`KUserAuthenticateReq`, and 41 ids are typedefs of bare scalars with no
struct anywhere.

`tools/crosscheck/check_event_packets.sh` checks each bucket with a different
oracle, none of which is the generator:

**Bound ids** become `static_assert`s compiled against the original headers:

```cpp
static_assert(std::is_same<KEGS_VERIFY_ACCOUNT_REQ, KUserAuthenticateReq>::value,
              "event 30 EGS_VERIFY_ACCOUNT_REQ is not carried by KUserAuthenticateReq");
```

The compiler resolves both names through the real typedefs. A mis-walked
alias chain, or an id bound to the wrong struct, fails to compile. Same for
the 41 scalar rows, asserted against the legacy spelling (`__int64`, `bool`,
`std::wstring`) rather than against our mapped type, so the check does not
import our own opinions.

**Unbound ids** are checked against the **symbol table of the compiled legacy
code**. The six legacy `.cpp` files are built for each of the ten regions and
`nm --defined-only` lists every `SerializeHelper::PutInto` the build actually
emits. `kNoStruct` must be absent from all ten; `kOtherRegion` must be absent
from this one and present in at least one other.

Result: **3386 bindings, 0 mismatches.** The ten per-region type counts the
symbol tables report (1953, 1965, 1957, 1965, 1975, 1939, 1937, 1945, 1943,
1915) match the preprocessor-derived counts exactly — two unrelated
mechanisms arriving at the same numbers.

### Two probes that were wrong first

Worth recording, because both looked right:

* **"Does the name compile?"** is not the same question as "can this build
  serialise it". `KDBE_CCU_AGENT_ACK` is declared unconditionally and has no
  serializer under TW/HK. The declaration probe reported 42 false
  disagreements — 41 of which turned out to be the scalar-payload bucket
  nobody had noticed, so the wrong probe still earned its keep.
* **Reading `nm` on a lone probe TU** finds every symbol undefined by
  construction, so the check passed 1268/1268 while testing nothing. The
  definitions live in the legacy `.cpp` files; the oracle has to compile
  those.

A third, smaller lesson: the first negative probe produced a **2.4 GB**
diagnostic log from 1268 undeclared names. Probes that work by provoking
compiler errors do not scale.

## Cross-verification of the composed path

Each layer above is checked on its own. Nothing so far checks the ORDER they
run in, and that order is where a rewrite actually breaks: seal before
switching keys, verify before decrypting, decompress after reading the event,
count the length prefix inside the length. Get any of those backwards and
every single-layer test still passes.

`tools/crosscheck/check_handshake.sh` runs a complete login conversation past
an independent implementation — ~250 lines of Python written from the legacy
C++ (`KSerializer`, `KSerBuffer`, `KEvent`, `KSecureBuffer`, the
`KSkSession` receive loop), sharing nothing with `knc.proto` but pyDes and
hashlib. Both directions:

* **forward** — Python records the conversation; the C++ `Session` replays it
  and reports what it saw. Catches anything the C++ cannot *read*.
* **backward** — the C++ emits the same conversation with the same keys and
  IVs; Python requires byte equality. Catches anything the C++ *writes*
  differently.

The conversation covers the key exchange under SPI 0, a struct packet, a
heartbeat, a bare-scalar payload (event 599, `typedef __int64`), and a
zlib-compressed payload. The compressed frame is compared after inflation
rather than byte for byte: two zlib builds may make different, equally valid
choices, and demanding identical compressed bytes would be testing zlib
rather than the protocol.

### It is falsifiable

A check nobody has seen fail is a check nobody has tested. Five mutations of
the reference implementation, each detected:

| mutation | caught by |
|---|---|
| sequence number big-endian | emitted `hello` differs |
| length prefix excludes itself | framing fails on both sides |
| SPI big-endian in the header | emitted `req` differs |
| ICV truncated to 9 bytes | authentication fails |
| skip `UnCompress()` | inflated payload differs |

### What it found

That an empty `KEvent` is **30 bytes**, not 26. The byte vector pinned in
`test_proto.cpp` had been correct since it was written; its label and
PROTOCOL.md both said 26, which is the size of the event *header* without the
4-byte empty `SerBuffer`. Two independent implementations agreeing on the
bytes is what made the stray number visible.

It cannot catch a mistake both implementations make from the same misreading
of the source. Only a capture from a real client can, and that remains the
largest gap.

## Known-answer vectors, not self-consistency

A round-trip test proves only that a module agrees with itself; it cannot catch
a byte order that is uniformly wrong. So the primitives are pinned to published
vectors, and the serializer's expected byte strings are written out as hex
literals rather than produced by calling the writer:

```cpp
Writer w;
w.put_wchar(u'A');       // 0x0041
w.put_wchar(u'\u4E2D');  // 0x4E2D
KNC_CHECK_EQ_BYTES(std::move(w).take(), from_hex("4100 2d4e"), "wchar_t LE layout");
```

and the asymmetry itself is asserted:

```cpp
KNC_CHECK(wide != narrow, "wchar_t must NOT match ushort byte order");
```

## Negative testing

Correct handling of correct input is the easy half. The envelope suite also
requires that:

* every single-bit flip, at every byte position of a sealed frame, is rejected;
* every truncation of a sealed frame below 25 bytes reports `kTooShort`;
* every truncation of a serialized event throws;
* a wrong key fails at the **ICV**, never at the padding (otherwise the padding
  parser is a decryption oracle);
* a 4-billion-element container count, a 4 GiB string, a 4 GiB decompressed
  size, and a zero-length TCP frame are all refused.

## State of the module build

The ship configuration (`cmake -DKNCNG_CLASSIC_MODE=OFF`) needs MSVC v145+,
GCC 14+, or Clang 17+. For the record, on the GCC 12.2 available here:

| module | `-fmodules-ts` result |
|---|---|
| `knc.core.types` | compiles, `.gcm` emitted |
| `knc.crypto.des` | compiles, `.gcm` emitted |
| `knc.crypto.md5` | compiles, `.gcm` emitted |
| `knc.proto.serializer` | `.gcm` emitted, then **ICE** in `make_decl_rtl, at varasm.cc:1446` |
| `knc.proto.event_ids` | compiles, `.gcm` emitted |
| `knc.proto.serbuffer` | compiles, `.gcm` emitted |
| `knc.proto.framing` | compiles, `.gcm` emitted |
| `knc.proto.security` | **ICE** in `write_location, at cp/module.cc:15635` |
| `knc.proto.event` | **ICE** in `write_location, at cp/module.cc:15635` |

Both are internal compiler errors in GCC 12's experimental modules
implementation — crashes in the compiler's own serialisation of module state,
not diagnostics about this code. Note that the interface for
`knc.proto.serializer` is successfully written out, and the modules that import
it (`serbuffer`, `framing`) build cleanly against it.

This is exactly the situation the dual-build contract exists for: the module
*structure* is expressed in the real source, and the module *semantics* are
verified through the header mode on a compiler that works. Nothing is stubbed
out to make a test pass.

## What is not yet verified

Honest list:

* **No live client has been connected.** This is now the single largest gap.
  The envelope is byte-exact against the sources and against published crypto
  vectors, and the packet layouts are confirmed by two independent
  extractions, but an end-to-end capture replay is the only thing that closes
  the loop. A recorded handshake from the original client, replayed through
  `FrameReader` + `SecureChannel`, remains the strongest possible test and is
  the recommended next step.
* The packet table is generated for **TW/HK only**. Other regions are a
  `--region` flag away, and `check_event_packets.sh` already compiles all ten
  to classify the bindings, but no other region's table has been generated.
* The binding table says which struct carries an event, not **who may send
  it**, nor which handler should run. `is_acceptable_from_client()` is a band
  and region filter, not an authorisation model.
* `Session` composes the protocol layers; it is not a server. There is no
  socket, no IOCP replacement, no event queue, no handler dispatch, and no
  concurrency. Those are the next thing to build on top of it.
* The two repaired `GET` bodies (PROTOCOL.md deviation 11) are repaired on the
  assumption that both packets stay server-internal. If either is ever sent to
  a client, the repair becomes a wire break.
* Packet layouts are verified for *structure*, not *semantics*. Nothing here
  says what `m_iOK == 1` means, or which fields a given handler must populate.
