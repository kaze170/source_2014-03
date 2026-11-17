# Differential cross-checks

The test suite in `tests/` pins behaviour against published vectors and literal
byte strings. These scripts do something different and complementary: they
check this implementation against **other people's implementations**, on large
numbers of random inputs.

The point is to break correlated errors. A test I wrote can share a
misunderstanding with the code I wrote. A disagreement with `pyDes` or Python's
`hashlib` cannot.

All four are reproducible from a clean checkout. They are not part of
`run_tests.sh` because they need Python packages and the original legacy tree.

| script | checks | against |
|---|---|---|
| `check_event_ids.sh` | all 3386 event ids | a real C++ preprocessor expanding the original headers |
| `check_md5.sh` | MD5 + HMAC-MD5 (full and 10-byte) | Python `hashlib` / `hmac` |
| `check_des.sh` | DES-CBC, both directions | `pyDes` (pure-Python, independent) |
| `check_envelope.sh` | the complete `KSecureBuffer` frame | a Python re-implementation written from the legacy C++ sources |

## Requirements

```sh
python3 -m venv .venv
.venv/bin/pip install pyDes
```

`check_event_ids.sh` additionally needs the original `KncWX2Server/Common`
tree, which lives beside this project in the same repository.

## Running

```sh
cd KncWX2ServerNG
tools/crosscheck/check_md5.sh
tools/crosscheck/check_des.sh
tools/crosscheck/check_envelope.sh
tools/crosscheck/check_event_ids.sh      # needs the legacy tree
```

## Last recorded results

```
event ids : 3386 rows, 0 mismatches, index == value throughout
MD5/HMAC  : 1200 comparisons, 0 mismatches
DES-CBC   :  600 comparisons, 0 mismatches
envelope  :  200 frames,      0 mismatches
```

## Why `check_envelope.sh` matters most

It is the only check that validates the *composition* rather than the pieces.
DES could be right, HMAC could be right, and the envelope could still be wrong
— ICV computed over the wrong span, sequence number in the wrong byte order,
padding counted from the wrong base. The Python reference is written straight
from `KSecureBuffer::Create_notRWM` and `GeneratePadding`, independently of the
C++ module, so agreement on 200 random frames covers the layout as a whole.
