#!/usr/bin/env bash
# Differential check over a real socket: start the server, log in with an
# independent client, and see whether the conversation works.
#
# check_handshake.sh compares bytes offline. This one runs the actual
# process: TCP, poll(), partial reads, the accept path, the router. The
# client is knc_ref.py - the same independent implementation of the wire
# protocol, written from the legacy C++ - driving a genuine socket.
#
# It is the first check that could catch a bug in the server rather than in
# the protocol library: a handshake never sent, a reply never flushed, a
# connection dropped on something it should tolerate.
#
#   pip install pyDes
set -euo pipefail
ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
WORK="${WORK_DIR:-${ROOT}/.build/crosscheck}"
PY="${PYTHON:-python3}"
HERE="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
mkdir -p "$WORK/live"
W="$WORK/live"

echo ">> building the server"
"${CXX:-g++}" -std=c++20 -O1 -w -I"${ROOT}/src" -I"${ROOT}/third_party/zlib" \
  -DHAVE_UNISTD_H "${ROOT}/src/main.cpp" "${ROOT}"/third_party/zlib/*.c \
  -o "$W/kncng_server"

# Port 0 asks the kernel for a free one, which is the only way to avoid
# racing another test or a leftover process.
"$W/kncng_server" --port 0 --version 1.2.3 --channel-id 7 > "$W/server.log" 2>&1 &
SERVER_PID=$!
trap 'kill $SERVER_PID 2>/dev/null || true; wait $SERVER_PID 2>/dev/null || true' EXIT

PORT=""
for _ in $(seq 1 100); do
  if [ -s "$W/server.log" ]; then
    PORT="$(sed -n 's/.*listening on 0\.0\.0\.0:\([0-9]*\).*/\1/p' "$W/server.log" | head -1)"
    [ -n "$PORT" ] && break
  fi
  sleep 0.05
done
if [ -z "$PORT" ]; then
  echo "!! the server never reported a port"; cat "$W/server.log"; exit 1
fi
echo "   server up on port $PORT (pid $SERVER_PID)"

"$PY" - "$HERE" "$PORT" <<'PYEOF'
import sys, socket, struct, time
sys.path.insert(0, sys.argv[1])
from knc_ref import *

PORT = int(sys.argv[2])
BOOT_AK = bytes.fromhex("C0D3BDC3B7CEB8B8")
BOOT_CK = bytes.fromhex("C7D8C4BFB5E9C0FD")

bad = []
def check(ok, what, detail=''):
    print(('   ok   ' if ok else '   FAIL ') + what + (' - ' + detail if detail and not ok else ''))
    if not ok: bad.append(what)

class Client:
    """A client built only out of knc_ref primitives."""
    def __init__(self, timeout=5.0):
        self.s = socket.create_connection(('127.0.0.1', PORT), timeout=timeout)
        self.s.settimeout(timeout)
        self.buf = b''
        self.ak, self.ck = BOOT_AK, BOOT_CK
        self.spi, self.seq = 0, 1

    def recv_frame(self):
        while True:
            frames, self.buf = unframe(self.buf)
            if frames:
                # unframe consumed everything it could; keep the extras.
                first, rest = frames[0], frames[1:]
                for f in reversed(rest):
                    self.buf = frame(f) + self.buf
                return first
            chunk = self.s.recv(65536)
            if not chunk:
                raise EOFError('server closed')
            self.buf += chunk

    def handshake(self):
        spi, seq, payload = open_frame(self.ak, self.ck, self.recv_frame())
        ev = event_get(payload)
        assert ev['id'] == 1, 'expected E_ACCEPT_CONNECTION_NOT, got %d' % ev['id']
        r = Reader(ev['payload'])
        self.spi = r.u16()
        sa = sa_get(r)
        self.ak, self.ck = sa['auth_key'], sa['crypto_key']
        return spi, self.spi, sa

    def send_event(self, event_id, payload=b'', iv=b'\x11' * 8):
        wire = frame(seal(self.ak, self.ck, self.spi, self.seq, iv,
                          event_put(event_id, payload)))
        self.seq += 1
        self.s.sendall(wire)

    def send_raw(self, data):
        self.s.sendall(data)

    def expect_event(self):
        spi, seq, payload = open_frame(self.ak, self.ck, self.recv_frame())
        return event_get(payload)

    def expect_closed(self, within=2.0):
        self.s.settimeout(within)
        try:
            return self.s.recv(4096) == b''
        except socket.timeout:
            return False
        except (ConnectionResetError, BrokenPipeError):
            return True

    def close(self): self.s.close()

# --- 1. the handshake over a real socket ----------------------------------
c = Client()
header_spi, session_spi, sa = c.handshake()
check(header_spi == 0, 'the key exchange arrives sealed under SPI 0', str(header_spi))
check(session_spi != 0, 'the server minted a non-zero session SPI', str(session_spi))
check(len(sa['auth_key']) == 8 and len(sa['crypto_key']) == 8,
      'the SA carries two 8-byte keys')
check(sa['auth_key'] != BOOT_AK, 'the session key is not the bootstrap key')
check(sa['seq'] == 1 and sa['last_seq'] == 0, 'the SA starts at sequence 1')

# --- 2. a good login ------------------------------------------------------
c.send_event(26, Writer().wstr("1.2.3").take())
ack = c.expect_event()
check(ack['id'] == 27, 'EGS_CONNECT_REQ is answered with EGS_CONNECT_ACK', str(ack['id']))
r = Reader(ack['payload'])
ok, udp, chan = r.i32(), r.i32(), r.i32()
check(ok == 0, 'the matching version is accepted (NET_OK)', str(ok))
check(chan == 7, 'the ack carries the configured channel id', str(chan))
check(r.rest() == 0, 'EGS_CONNECT_ACK is exactly 12 bytes',
      'leftover %d' % r.rest())

# --- 3. a wrong version ---------------------------------------------------
c2 = Client(); c2.handshake()
c2.send_event(26, Writer().wstr("9.9.9").take())
nack = c2.expect_event()
r = Reader(nack['payload'])
check(r.i32() == 3, 'a mismatched version is refused with ERR_CONNECT_00')
c2.close()

# --- 4. a heartbeat is absorbed silently ----------------------------------
c.send_event(0)
c.send_event(26, Writer().wstr("1.2.3").take())
again = c.expect_event()
check(again['id'] == 27,
      'a heartbeat is consumed and the next event still answered')

# --- 5. things the server must refuse WITHOUT dropping the connection -----
c.send_event(1325, b'')                      # server-band id from a client
c.send_event(599, Writer().i64(7).take())    # client-band, no handler
c.send_event(26, Writer().wstr("1.2.3").take())
survived = c.expect_event()
check(survived['id'] == 27,
      'a refused event and an unhandled one both leave the connection up')

# --- 6. a corrupted frame is tolerated ------------------------------------
wire = bytearray(frame(seal(c.ak, c.ck, c.spi, c.seq, b'\x22' * 8,
                            event_put(26, Writer().wstr("1.2.3").take()))))
c.seq += 1
wire[-1] ^= 0xFF                              # break the ICV
c.send_raw(bytes(wire))
c.send_event(26, Writer().wstr("1.2.3").take())
after = c.expect_event()
check(after['id'] == 27, 'one bad ICV does not kill the connection')

# --- 7. a frame split across two writes -----------------------------------
whole = frame(seal(c.ak, c.ck, c.spi, c.seq, b'\x33' * 8,
                   event_put(26, Writer().wstr("1.2.3").take())))
c.seq += 1
c.send_raw(whole[:9]); time.sleep(0.15); c.send_raw(whole[9:])
split = c.expect_event()
check(split['id'] == 27, 'a frame split across two TCP writes is reassembled')

# --- 8. a bad length IS fatal ---------------------------------------------
# The legacy loop would have spun forever on this one.
c3 = Client(); c3.handshake()
c3.send_raw(b'\x00\x00')
check(c3.expect_closed(), 'a zero-length frame closes the connection')
c3.close()

# --- 9. several clients at once -------------------------------------------
many = []
for i in range(5):
    m = Client(); m.handshake(); many.append(m)
spis = {m.spi for m in many}
check(len(spis) == 5, 'five concurrent clients get five distinct SPIs', str(spis))
for i, m in enumerate(many):
    m.send_event(26, Writer().wstr("1.2.3").take())
oks = 0
for m in many:
    e = m.expect_event()
    if e['id'] == 27 and Reader(e['payload']).i32() == 0: oks += 1
check(oks == 5, 'all five log in', '%d of 5' % oks)
for m in many: m.close()

c.close()
print()
if bad:
    print('   %d FAILURE(S)' % len(bad))
    sys.exit(1)
print('   OK - the server speaks the protocol to an independent client')
PYEOF

echo
echo ">> server log"
sed 's/^/   | /' "$W/server.log" | tail -12
echo
echo "== live server cross-check PASSED =="
