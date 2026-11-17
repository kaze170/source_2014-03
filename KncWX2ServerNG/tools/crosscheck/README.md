# Differential cross-checks

The test suite in `tests/` pins behaviour against published vectors and literal
byte strings. These scripts do something different and complementary: they
check this implementation against **other people's implementations**, on large
numbers of random inputs.

The point is to break correlated errors. A test I wrote can share a
misunderstanding with the code I wrote. A disagreement with `pyDes` or Python's
`hashlib` cannot.

All six are reproducible from a clean checkout. They are not part of
`run_tests.sh` because they need Python packages and the original legacy tree.

| script | checks | against |
|---|---|---|
| `check_event_ids.sh` | all 3386 event ids | a real C++ preprocessor expanding the original headers |
| `check_md5.sh` | MD5 + HMAC-MD5 (full and 10-byte) | Python `hashlib` / `hmac` |
| `check_des.sh` | DES-CBC, both directions | `pyDes` (pure-Python, independent) |
| `check_envelope.sh` | the complete `KSecureBuffer` frame | a Python re-implementation written from the legacy C++ sources |
| `check_packets.sh` | all 1953 packet layouts, every field and position | a real C++ **compiler** running the original serialization macros |
| `check_event_packets.sh` | all 3386 event→packet bindings | `static_assert` on the original headers + the **symbol tables** of the legacy code built for 10 regions |

## Requirements

```sh
python3 -m venv .venv
.venv/bin/pip install pyDes
```

`check_event_ids.sh`, `check_packets.sh` and `check_event_packets.sh`
additionally need the original
`KncWX2Server/Common` tree, which lives beside this project in the same
repository. Neither needs any Python package.

## Running

```sh
cd KncWX2ServerNG
tools/crosscheck/check_md5.sh
tools/crosscheck/check_des.sh
tools/crosscheck/check_envelope.sh
tools/crosscheck/check_event_ids.sh      # needs the legacy tree
tools/crosscheck/check_packets.sh        # needs the legacy tree, ~30s
tools/crosscheck/check_event_packets.sh  # needs the legacy tree, ~90s
```

## Last recorded results

```
event ids : 3386 rows, 0 mismatches, index == value throughout
MD5/HMAC  : 1200 comparisons, 0 mismatches
DES-CBC   :  600 comparisons, 0 mismatches
envelope  :  200 frames,      0 mismatches
packets   : 1953 types / 7537 field signatures, 0 mismatches
bindings  : 3386 event->packet bindings, 0 mismatches
```

## Why `check_envelope.sh` and `check_packets.sh` matter most

`check_envelope.sh` is the only check that validates the *composition* of the
crypto rather than the pieces.
DES could be right, HMAC could be right, and the envelope could still be wrong
— ICV computed over the wrong span, sequence number in the wrong byte order,
padding counted from the wrong base. The Python reference is written straight
from `KSecureBuffer::Create_notRWM` and `GeneratePadding`, independently of the
C++ module, so agreement on 200 random frames covers the layout as a whole.

`check_packets.sh` is the same idea applied to the thing with the most surface
area. `tools/gen_packets.py` builds the packet table by preprocessing the
legacy sources and parsing the result with regular expressions; if those
regexes are subtly wrong, the table is subtly wrong and every test written
against it still passes. So this script compiles the **original, unmodified**
`.cpp` files with the **original** `PUT`/`GET` macros and swaps in a
`KSerializer` that records the static type of each field instead of writing
bytes. Field order then comes from the compiler walking the real
`return PUT(a) && PUT(b) && ...;` chains, and field types come from its own
overload resolution. The two paths share no code and no failure modes.

It found three real problems that the regex path had silently accepted: the
`OnlyGlobal/Packet/*_Global.cpp` translation units (210 missing types, plus
the wrong definition of `KBillProductInfo`), a member swallowed by an inline
method with no trailing semicolon, and two `SERIALIZE_DEFINE_GET` bodies that
call `PUT`.

`check_event_packets.sh` found a fourth: 41 event ids whose payload is a bare
`typedef __int64 KEGS_ACCEPT_FRIEND_REQ;` rather than a struct. They have no
`DECL_PACKET` anywhere and had been filed under "no payload".

Both scripts are worth reading for how their *first* versions were wrong —
each docstring records it. Testing "does the name compile" instead of "can
this build serialise it", and running `nm` on a probe TU where every symbol
is undefined by construction, both produced green checks that measured
nothing.
