#!/usr/bin/env bash
# Differential check of the WHOLE ingress/egress path: a complete login
# conversation, byte for byte, against an independent implementation.
#
# The other five scripts each check one layer. This one checks that the layers
# compose - the part no unit test can see, because every ordering mistake
# (seal before switching keys, verify before decrypting, decompress after
# reading the event, count the prefix inside the length) leaves each
# individual layer perfectly correct.
#
# The reference side is ~250 lines of Python written from the legacy C++
# only: KSerializer, KSerBuffer, KEvent, KSecureBuffer and the KSkSession
# receive loop. It shares no code with knc.proto - only pyDes and hashlib.
#
# Both directions are checked:
#
#   forward   Python records a conversation; the C++ Session replays it and
#             reports what it saw. Catches anything the C++ cannot READ.
#   backward  the C++ Session emits the same conversation with the same keys
#             and IVs; Python requires the bytes to be identical. Catches
#             anything the C++ WRITES differently.
#
# A real client capture would be better still - this cannot catch a mistake
# both implementations make from the same misreading of the source. It does
# catch every mistake only one of them makes.
#
#   pip install pyDes
set -euo pipefail
ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
WORK="${WORK_DIR:-${ROOT}/.build/crosscheck}"
PY="${PYTHON:-python3}"
mkdir -p "$WORK/handshake"
W="$WORK/handshake"

# Fixed material, shared by both sides. Nothing here is secret; the point is
# that both implementations must produce identical bytes from identical input.
SESSION_SPI=4660                     # 0x1234
AUTH_KEY=0001020304050607
CRYPTO_KEY=08090a0b0c0d0e0f
IV_HELLO=b0b1b2b3b4b5b6b7
IV_REQ=a0a1a2a3a4a5a6a7
IV_ACK=c0c1c2c3c4c5c6c7
IV_HB=d0d1d2d3d4d5d6d7
IV_SCALAR=e0e1e2e3e4e5e6e7
IV_ZIP=f0f1f2f3f4f5f6f7

# ---------------------------------------------------------------------------
# The reference implementation.
# ---------------------------------------------------------------------------
# The reference implementation lives in knc_ref.py next to this script, so
# that check_live.sh can drive the same client over a real socket.
cp "$(dirname "${BASH_SOURCE[0]}")/knc_ref.py" "$W/ref.py"

# ---------------------------------------------------------------------------
# Stage 1: Python records the conversation.
# ---------------------------------------------------------------------------
"$PY" - "$W" <<PYEOF
import sys, os, json
sys.path.insert(0, "$W")
from ref import *

W = sys.argv[1]
spi = $SESSION_SPI
ak = bytes.fromhex("$AUTH_KEY")
ck = bytes.fromhex("$CRYPTO_KEY")
boot_ak = bytes.fromhex("C0D3BDC3B7CEB8B8")   # KncSecurity default keys,
boot_ck = bytes.fromhex("C7D8C4BFB5E9C0FD")   # CP949 for two Korean words

# --- server -> client: the key exchange, sealed under SPI 0 ---------------
w = Writer(); w.u16(spi); sa_put(w, ak, ck)
hello_payload = w.take()
hello = frame(seal(boot_ak, boot_ck, 0, 1, bytes.fromhex("$IV_HELLO"),
                   event_put(1, hello_payload)))

# --- client -> server ------------------------------------------------------
# EGS_CONNECT_REQ(26): one wstring.
req = frame(seal(ak, ck, spi, 1, bytes.fromhex("$IV_REQ"),
                 event_put(26, Writer().wstr("1.2.3").take())))
# E_HEART_BEAT(0): an empty event, 30 bytes before sealing.
hb = frame(seal(ak, ck, spi, 2, bytes.fromhex("$IV_HB"), event_put(0, b'')))
# EGS_ACCEPT_FRIEND_REQ(599): a SCALAR payload - typedef __int64, no struct.
scalar = frame(seal(ak, ck, spi, 3, bytes.fromhex("$IV_SCALAR"),
                    event_put(599, Writer().i64(-81985529216486896).take())))
# A compressed payload: 400 'A's through KSerBuffer::Compress.
big = Writer().wstr("A" * 400).take()
zipped = frame(seal(ak, ck, spi, 4, bytes.fromhex("$IV_ZIP"),
                    event_put(26, compress(big), compressed=True)))

# --- server -> client ------------------------------------------------------
# EGS_CONNECT_ACK(27): three ints, 12 bytes exactly.
ack_payload = Writer().u32(1).u32(0x3A98).u32(7).take()
ack = frame(seal(ak, ck, spi, 1, bytes.fromhex("$IV_ACK"), event_put(27, ack_payload)))

open(os.path.join(W, "to_client.bin"), "wb").write(hello + ack)
open(os.path.join(W, "to_server.bin"), "wb").write(req + hb + scalar + zipped)
json.dump({
    "hello": hello.hex(), "req": req.hex(), "hb": hb.hex(),
    "scalar": scalar.hex(), "zipped": zipped.hex(), "ack": ack.hex(),
    "event_sizes": {"empty": len(event_put(0, b''))},
}, open(os.path.join(W, "recording.json"), "w"), indent=1)
print("   recorded %d client->server bytes, %d server->client bytes"
      % (len(req + hb + scalar + zipped), len(hello + ack)))
PYEOF

# ---------------------------------------------------------------------------
# Stage 2: the C++ Session replays it, and emits its own copy.
# ---------------------------------------------------------------------------
cat > "$W/replay.cpp" <<'EOF'
#define KNC_CLASSIC 1
#include <cstdio>
#include <cstdlib>
#include <string>
#include <vector>
#include "proto/session.cppm"
using namespace knc;
using namespace knc::proto;

static Bytes slurp(const char* path) {
    std::FILE* f = std::fopen(path, "rb");
    if (!f) { std::fprintf(stderr, "cannot open %s\n", path); std::exit(2); }
    Bytes out; std::uint8_t buf[4096]; std::size_t n;
    while ((n = std::fread(buf, 1, sizeof buf, f)) > 0) out.insert(out.end(), buf, buf + n);
    std::fclose(f);
    return out;
}
static std::string hex(ByteView b) {
    static const char* d = "0123456789abcdef";
    std::string o;
    for (auto c : b) { o += d[c >> 4]; o += d[c & 15]; }
    return o;
}
static Bytes unhex(const std::string& s) {
    Bytes o;
    for (std::size_t i = 0; i + 1 < s.size(); i += 2)
        o.push_back((std::uint8_t)std::stoul(s.substr(i, 2), nullptr, 16));
    return o;
}

int main(int argc, char** argv) {
    const std::string dir = argv[1];
    const SpiType spi = (SpiType)std::stoul(argv[2]);
    const SecurityAssociation sa{unhex(argv[3]), unhex(argv[4])};

    // --- forward: replay what the reference recorded ----------------------
    {
        Session client{Role::kClient};
        const Bytes stream = slurp((dir + "/to_client.bin").c_str());
        client.push(view(stream));
        for (;;) {
            const Inbound in = client.next(1);
            if (in.status == Ingress::kNeedMoreData) break;
            std::printf("C\t%s\t%u\t%s\t%s\n", to_string(in.status),
                        (unsigned)(in ? in.event.id() : 0),
                        in ? hex(view(in.event.payload().data())).c_str() : "",
                        in.detail);
            if (in.status == Ingress::kFatal) break;
        }
        std::printf("CSPI\t%u\t%s\t%s\n", (unsigned)client.spi(),
                    hex(view(client.channel().sa().auth_key())).c_str(),
                    hex(view(client.channel().sa().crypto_key())).c_str());
    }
    {
        Session server{Role::kServer};
        server.channel().adopt(spi, sa);
        const Bytes stream = slurp((dir + "/to_server.bin").c_str());
        server.push(view(stream));
        for (;;) {
            const Inbound in = server.next(1);
            if (in.status == Ingress::kNeedMoreData) break;
            std::printf("S\t%s\t%u\t%s\t%s\n", to_string(in.status),
                        (unsigned)(in ? in.event.id() : 0),
                        in ? hex(view(in.event.payload().data())).c_str() : "",
                        in.detail);
            if (in.status == Ingress::kFatal) break;
        }
    }

    // --- backward: emit the same conversation -----------------------------
    // Same keys, same IVs, same order. Any byte that differs is a bug in one
    // of the two implementations.
    {
        Session server{Role::kServer};
        const Bytes hello = server.accept_with(FreshSa{spi, sa}, view(unhex(argv[5])));
        std::printf("EMIT\thello\t%s\n", hex(view(hello)).c_str());

        Event ack_ev{Ev::EGS_CONNECT_ACK};
        packets::KEGS_CONNECT_ACK ack{};
        ack.m_iOK = 1; ack.m_UDPPort = 0x3A98; ack.m_iChannelID = 7;
        ack_ev.payload().assign(encode_packet(ack));
        std::printf("EMIT\tack\t%s\n",
                    hex(view(server.send_with_iv(ack_ev, view(unhex(argv[6]))))).c_str());
    }
    {
        Session client{Role::kClient};
        client.channel().adopt(spi, sa);

        Event req{Ev::EGS_CONNECT_REQ};
        req.payload().assign(encode_packet(packets::KEGS_CONNECT_REQ{u"1.2.3"}));
        std::printf("EMIT\treq\t%s\n",
                    hex(view(client.send_with_iv(req, view(unhex(argv[7]))))).c_str());

        Event hb{Ev::E_HEART_BEAT};
        std::printf("EMIT\thb\t%s\n",
                    hex(view(client.send_with_iv(hb, view(unhex(argv[8]))))).c_str());

        Event sc{Ev::EGS_ACCEPT_FRIEND_REQ};
        sc.payload().assign(encode_packet<std::int64_t>(-81985529216486896LL));
        std::printf("EMIT\tscalar\t%s\n",
                    hex(view(client.send_with_iv(sc, view(unhex(argv[9]))))).c_str());

        Event zip{Ev::EGS_CONNECT_REQ};
        packets::KEGS_CONNECT_REQ big{};
        big.m_wstrVersion = std::u16string(400, u'A');
        zip.payload().assign(encode_packet(big));
        zip.payload().compress();
        std::printf("EMIT\tzipped\t%s\n",
                    hex(view(client.send_with_iv(zip, view(unhex(argv[10]))))).c_str());
    }
    return 0;
}
EOF

echo ">> compiling the replay driver"
"${CXX:-g++}" -std=c++20 -O1 -w -I"${ROOT}/src" -I"${ROOT}/third_party/zlib" \
  "$W/replay.cpp" "${ROOT}"/third_party/zlib/*.c -DHAVE_UNISTD_H -o "$W/replay"
"$W/replay" "$W" "$SESSION_SPI" "$AUTH_KEY" "$CRYPTO_KEY" \
  "$IV_HELLO" "$IV_ACK" "$IV_REQ" "$IV_HB" "$IV_SCALAR" "$IV_ZIP" > "$W/transcript.tsv"

# ---------------------------------------------------------------------------
# Stage 3: reconcile.
# ---------------------------------------------------------------------------
"$PY" - "$W" <<PYEOF
import sys, json, os
sys.path.insert(0, "$W")
from ref import *

W = sys.argv[1]
rec = json.load(open(os.path.join(W, "recording.json")))
rows = [l.rstrip('\n').split('\t') for l in open(os.path.join(W, "transcript.tsv"))]
ak = bytes.fromhex("$AUTH_KEY"); ck = bytes.fromhex("$CRYPTO_KEY")

bad = []
def check(ok, what, detail=''):
    if not ok: bad.append(what + ((' - ' + detail) if detail else ''))

# --- forward ---------------------------------------------------------------
cl = [r for r in rows if r[0] == 'C']
sv = [r for r in rows if r[0] == 'S']

check(len(cl) == 2, 'client saw two frames', str(len(cl)))
check(cl[0][1] == 'handled by the session' and 'key' in cl[0][4],
      'the client adopted the key from the recorded handshake', str(cl[0]))
check(cl[1][1] == 'event' and cl[1][2] == '27',
      'the client decoded EGS_CONNECT_ACK', str(cl[1]))
want_ack = Writer().u32(1).u32(0x3A98).u32(7).take().hex()
check(cl[1][3] == want_ack, 'EGS_CONNECT_ACK payload matches', cl[1][3])

spi_row = [r for r in rows if r[0] == 'CSPI'][0]
check(int(spi_row[1]) == $SESSION_SPI, 'the client adopted the recorded SPI', spi_row[1])
check(spi_row[2] == "$AUTH_KEY" and spi_row[3] == "$CRYPTO_KEY",
      'the client adopted the recorded keys', str(spi_row))

check(len(sv) == 4, 'server saw four frames', str(len(sv)))
check(sv[0][1] == 'event' and sv[0][2] == '26', 'server decoded EGS_CONNECT_REQ')
check(sv[0][3] == Writer().wstr("1.2.3").take().hex(), 'version payload matches', sv[0][3])
check(sv[1][1] == 'handled by the session' and sv[1][4] == 'heartbeat',
      'server filtered the heartbeat', str(sv[1]))
check(sv[2][1] == 'event' and sv[2][2] == '599', 'server decoded the scalar event')
check(sv[2][3] == Writer().i64(-81985529216486896).take().hex(),
      'scalar payload matches', sv[2][3])
check(sv[3][1] == 'event' and sv[3][2] == '26', 'server decoded the compressed event')
check(sv[3][3] == Writer().wstr("A" * 400).take().hex(),
      'the compressed payload was inflated correctly')

# --- backward --------------------------------------------------------------
emitted = {r[1]: r[2] for r in rows if r[0] == 'EMIT'}
for name in ('hello', 'req', 'hb', 'scalar', 'ack'):
    check(emitted[name] == rec[name], 'emitted %s is byte-identical' % name,
          '\n     cpp %s\n     ref %s' % (emitted[name], rec[name]))

# The compressed frame is compared after inflation rather than byte for byte:
# two zlib builds may make different (equally valid) choices, and requiring
# identical compressed bytes would be testing zlib, not the protocol.
spi, seq, payload = open_frame(ak, ck, bytes.fromhex(emitted['zipped'])[2:])
ev = event_get(payload)
check(ev['id'] == 26 and ev['payload'] == Writer().wstr("A" * 400).take(),
      'the emitted compressed frame inflates to the right payload')
check(seq == 4, 'the emitted compressed frame carries sequence 4', str(seq))

# --- structural facts worth pinning ---------------------------------------
check(rec['event_sizes']['empty'] == 30, 'an empty KEvent is 30 bytes')
hello_raw = bytes.fromhex(rec['hello'])
check(int.from_bytes(hello_raw[0:2], 'little') == len(hello_raw),
      'the length prefix counts itself')
check(int.from_bytes(hello_raw[2:4], 'little') == 0,
      'the key exchange is sealed under SPI 0')
# ...and the SPI inside the body is the SAME number, big-endian.
_, _, hello_payload = open_frame(bytes.fromhex("C0D3BDC3B7CEB8B8"),
                                 bytes.fromhex("C7D8C4BFB5E9C0FD"), hello_raw[2:])
inner = Reader(event_get(hello_payload)['payload'])
check(inner.u16() == $SESSION_SPI,
      'the SPI is big-endian in the body and little-endian in the header')
check(sa_get(inner)['auth_key'] == ak, 'the SA travels intact')

print()
if bad:
    print('   %d MISMATCH(ES):' % len(bad))
    for b in bad: print('     - ' + b)
    sys.exit(1)
print('   OK - %d checks, the recorded conversation and the C++ session agree'
      % (len(cl) + len(sv) + 20))
PYEOF

echo
echo "== handshake replay cross-check PASSED =="
