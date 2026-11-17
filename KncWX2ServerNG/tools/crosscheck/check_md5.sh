#!/usr/bin/env bash
# Differential check: knc.crypto.md5 vs Python hashlib/hmac.
set -euo pipefail
ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
WORK="${WORK_DIR:-${ROOT}/.build/crosscheck}"
PY="${PYTHON:-python3}"
mkdir -p "$WORK"

cat > "$WORK/md5gen.cpp" <<'EOF'
#define KNC_CLASSIC 1
#include <cstdio>
#include <cstdint>
#include "crypto/md5.cppm"
using namespace knc;
static std::uint64_t st = 0x123456789abcdefULL;
static std::uint8_t rnd() { st ^= st << 13; st ^= st >> 7; st ^= st << 17; return (std::uint8_t)(st >> 24); }
static void hexv(const Bytes& b) { for (auto x : b) std::printf("%02x", x); }
template <std::size_t N> static void hexa(const std::array<std::uint8_t, N>& b) { for (auto x : b) std::printf("%02x", x); }
int main() {
    for (int i = 0; i < 400; ++i) {
        Bytes msg((std::size_t)(i * 7 % 300)), key((std::size_t)(1 + i * 11 % 90));
        for (auto& b : msg) b = rnd();
        for (auto& b : key) b = rnd();
        std::printf("MD5\t");  hexv(msg); std::printf("\t"); hexa(crypto::Md5::hash(view(msg))); std::printf("\n");
        std::printf("HMAC\t"); hexv(key); std::printf("\t"); hexv(msg); std::printf("\t");
        hexv(crypto::hmac_md5(view(key), view(msg), 16)); std::printf("\t");
        hexv(crypto::hmac_md5(view(key), view(msg), 10)); std::printf("\n");
    }
}
EOF

"${CXX:-g++}" -std=c++20 -O1 -w -I"${ROOT}/src" "$WORK/md5gen.cpp" -o "$WORK/md5gen"
"$WORK/md5gen" > "$WORK/md5.tsv"

"$PY" - "$WORK/md5.tsv" <<'EOF'
import hashlib, hmac, sys
n = 0; bad = []
for line in open(sys.argv[1]):
    f = line.rstrip('\n').split('\t')
    if f[0] == 'MD5':
        want = hashlib.md5(bytes.fromhex(f[1])).hexdigest(); n += 1
        if f[2] != want: bad.append(('MD5', f[2], want))
    else:
        want = hmac.new(bytes.fromhex(f[1]), bytes.fromhex(f[2]), hashlib.md5).hexdigest(); n += 2
        if f[3] != want:      bad.append(('HMAC16', f[3], want))
        if f[4] != want[:20]: bad.append(('HMAC10', f[4], want[:20]))
print(f"MD5 / HMAC-MD5 comparisons : {n}")
print(f"mismatches                 : {len(bad)}")
for b in bad[:5]: print("  ", b)
sys.exit(1 if bad else 0)
EOF
echo "PASS: knc.crypto.md5 agrees with Python hashlib/hmac"
