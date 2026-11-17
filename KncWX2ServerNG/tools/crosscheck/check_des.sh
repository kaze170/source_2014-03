#!/usr/bin/env bash
# Differential check: knc.crypto.des vs pyDes (independent pure-Python DES).
#   pip install pyDes
set -euo pipefail
ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
WORK="${WORK_DIR:-${ROOT}/.build/crosscheck}"
PY="${PYTHON:-python3}"
mkdir -p "$WORK"

cat > "$WORK/desgen.cpp" <<'EOF'
#define KNC_CLASSIC 1
#include <cstdio>
#include <cstdint>
#include "crypto/des.cppm"
using namespace knc;
static std::uint64_t st = 0xfeedfacecafebeefULL;
static std::uint8_t rnd() { st ^= st << 13; st ^= st >> 7; st ^= st << 17; return (std::uint8_t)(st >> 24); }
static void hexv(const Bytes& b) { for (auto x : b) std::printf("%02x", x); }
int main() {
    for (int i = 0; i < 300; ++i) {
        Bytes key(8), iv(8), pt((std::size_t)(8 * (1 + i % 12)));
        for (auto& b : key) b = rnd();
        for (auto& b : iv)  b = rnd();
        for (auto& b : pt)  b = rnd();
        const Bytes ct = crypto::des_cbc_encrypt(view(key), view(iv), view(pt));
        if (crypto::des_cbc_decrypt(view(key), view(iv), view(ct)) != pt) {
            std::fprintf(stderr, "self round-trip failed at %d\n", i); return 1;
        }
        hexv(key); std::printf("\t"); hexv(iv); std::printf("\t");
        hexv(pt);  std::printf("\t"); hexv(ct); std::printf("\n");
    }
}
EOF

"${CXX:-g++}" -std=c++20 -O1 -w -I"${ROOT}/src" "$WORK/desgen.cpp" -o "$WORK/desgen"
"$WORK/desgen" > "$WORK/des.tsv"

"$PY" - "$WORK/des.tsv" <<'EOF'
import sys, pyDes
n = 0; bad = []
for line in open(sys.argv[1]):
    k, iv, pt, ct = [bytes.fromhex(x) for x in line.rstrip('\n').split('\t')]
    d = pyDes.des(k, pyDes.CBC, iv, pad=None, padmode=pyDes.PAD_NORMAL)
    n += 2
    if d.encrypt(pt) != ct: bad.append(('encrypt', k.hex(), pt.hex()))
    if d.decrypt(ct) != pt: bad.append(('decrypt', k.hex(), ct.hex()))
print(f"DES-CBC comparisons : {n}")
print(f"mismatches          : {len(bad)}")
for b in bad[:5]: print("  ", b)
sys.exit(1 if bad else 0)
EOF
echo "PASS: knc.crypto.des agrees with pyDes (both directions)"
