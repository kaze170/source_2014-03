#!/usr/bin/env bash
# Differential check: the whole KSecureBuffer envelope produced by
# knc.proto.security vs a Python re-implementation written directly from the
# legacy C++ sources (KSecureBuffer::Create_notRWM + GeneratePadding).
#   pip install pyDes
set -euo pipefail
ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
WORK="${WORK_DIR:-${ROOT}/.build/crosscheck}"
PY="${PYTHON:-python3}"
mkdir -p "$WORK"

cat > "$WORK/sealgen.cpp" <<'EOF'
#define KNC_CLASSIC 1
#include <cstdio>
#include <cstdint>
#include "proto/security.cppm"
using namespace knc;
using namespace knc::proto;
static std::uint64_t st = 0x0badc0de12345678ULL;
static std::uint8_t rnd() { st ^= st << 13; st ^= st >> 7; st ^= st << 17; return (std::uint8_t)(st >> 24); }
static void hexv(const Bytes& b) { for (auto x : b) std::printf("%02x", x); }
int main() {
    for (int i = 0; i < 200; ++i) {
        Bytes ak(8), ck(8), iv(8), payload((std::size_t)(i % 97));
        for (auto& b : ak) b = rnd();
        for (auto& b : ck) b = rnd();
        for (auto& b : iv) b = rnd();
        for (auto& b : payload) b = rnd();
        SecurityAssociation sa{ak, ck};
        const std::uint32_t seq = 1u + (std::uint32_t)i * 7919u;
        sa.set_sequence_num(seq);
        const SpiType spi = (SpiType)(i * 311 % 65536);
        const Bytes frame = seal_with_iv(sa, spi, view(payload), view(iv));
        std::printf("%u\t%u\t", (unsigned)spi, (unsigned)seq);
        hexv(ak); std::printf("\t"); hexv(ck); std::printf("\t"); hexv(iv); std::printf("\t");
        hexv(payload); std::printf("\t"); hexv(frame); std::printf("\n");
    }
}
EOF

"${CXX:-g++}" -std=c++20 -O1 -w -I"${ROOT}/src" -I"${ROOT}/third_party/zlib" \
  "$WORK/sealgen.cpp" -o "$WORK/sealgen"
"$WORK/sealgen" > "$WORK/seal.tsv"

"$PY" - "$WORK/seal.tsv" <<'EOF'
# Written from the legacy sources, NOT from the new C++ module:
#   Append(m_nSPIndex)                 -> 2 bytes native LE
#   Append(GetSA().GetSequenceNum())   -> 4 bytes native LE
#   append(iv)                         -> 8 bytes
#   Encryption(payload+pad+padLen, iv) -> DES-CBC
#   GenerateICV(buf so far)            -> HMAC-MD5 truncated to 10
# GeneratePadding: nPadBytes = 8 - ((len + 1) % 8); pad[i] = i + 1
import sys, struct, hmac, hashlib, pyDes
n = 0; bad = []
for line in open(sys.argv[1]):
    f = line.rstrip('\n').split('\t')
    spi, seq = int(f[0]), int(f[1])
    ak, ck, iv, payload, got = [bytes.fromhex(x) for x in f[2:7]]
    pad_len = 8 - ((len(payload) + 1) % 8)
    plain = payload + bytes(range(1, pad_len + 1)) + bytes([pad_len])
    assert len(plain) % 8 == 0
    crypt = pyDes.des(ck, pyDes.CBC, iv, pad=None, padmode=pyDes.PAD_NORMAL).encrypt(plain)
    head = struct.pack('<HI', spi, seq) + iv + crypt
    want = head + hmac.new(ak, head, hashlib.md5).digest()[:10]
    n += 1
    if want != got:
        bad.append((spi, seq, len(payload), got.hex()[:64], want.hex()[:64]))
print(f"envelope comparisons : {n}")
print(f"mismatches           : {len(bad)}")
for b in bad[:5]: print("  ", b)
sys.exit(1 if bad else 0)
EOF
echo "PASS: the KSecureBuffer envelope is byte-identical to the independent reference"
