"""KncWX2 client stack, written from the legacy C++ sources.

Sources, for every rule below:
  KNCSDK/Include/Serializer/Serializer.h   - Put/Get for each scalar type
  Common/SerBuffer.cpp                     - KSerBuffer::Serialize
  Common/Event.cpp                         - KEvent::Serialize
  KNCSDK/Include/KncSecurity/SecureBuffer.* - Create_notRWM, GeneratePadding
  Common/Socket/Session.cpp:13             - the receive loop and its framing
"""
import hmac, hashlib, struct, zlib
import pyDes

# --- KSerializer -----------------------------------------------------------
# Tagless and big-endian for everything except where noted.

class Writer:
    def __init__(self): self.b = bytearray()
    def raw(self, data): self.b += data; return self
    def u8(self, v): self.b.append(v & 0xFF); return self
    def boolean(self, v): return self.u8(1 if v else 0)
    def u16(self, v): self.b += struct.pack('>H', v & 0xFFFF); return self
    def u32(self, v): self.b += struct.pack('>I', v & 0xFFFFFFFF); return self
    def i64(self, v): self.b += struct.pack('>q', v); return self
    def f32(self, v): self.b += struct.pack('>f', v); return self
    def wstr(self, s):
        # DWORD *byte* count, then UTF-16 in NATIVE little-endian order -
        # wchar_t is Put() as a raw 2-byte word, never byte-swapped.
        enc = s.encode('utf-16-le')
        return self.u32(len(enc)).raw(enc)
    def bytestream(self, data):
        # KByteStream: big-endian DWORD length then the bytes.
        return self.u32(len(data)).raw(data)
    def take(self): return bytes(self.b)

class Reader:
    def __init__(self, data): self.d = data; self.i = 0
    def take(self, n):
        if self.i + n > len(self.d): raise ValueError('short read')
        out = self.d[self.i:self.i + n]; self.i += n; return out
    def u8(self): return self.take(1)[0]
    def boolean(self): return self.take(1)[0] == 1
    def u16(self): return struct.unpack('>H', self.take(2))[0]
    def u32(self): return struct.unpack('>I', self.take(4))[0]
    def i32(self): return struct.unpack('>i', self.take(4))[0]
    def i64(self): return struct.unpack('>q', self.take(8))[0]
    def wstr(self): return self.take(self.u32()).decode('utf-16-le')
    def bytestream(self): return self.take(self.u32())
    def rest(self): return len(self.d) - self.i

# --- KSerBuffer ------------------------------------------------------------
# Serialize(): an empty buffer is a bare big-endian 0. Otherwise a big-endian
# DWORD length, a bool compression flag, then the raw bytes. A compressed blob
# starts with the original size as a NATIVE little-endian DWORD.

def serbuffer_put(w, data, compressed=False):
    if not data and not compressed:
        return w.u32(0)
    return w.u32(len(data)).boolean(compressed).raw(data)

def serbuffer_get(r):
    n = r.u32()
    if n == 0: return b'', False
    comp = r.boolean()
    return r.take(n), comp

def uncompress(blob):
    original = struct.unpack('<I', blob[:4])[0]
    out = zlib.decompress(blob[4:])
    assert len(out) == original, 'original size disagrees'
    return out

def compress(plain):
    return struct.pack('<I', len(plain)) + zlib.compress(plain, 1)

# --- KEvent ----------------------------------------------------------------
# KPerformerInfo: DWORD id, DWORD uid count, then the uids.
# KEvent: performer, trace[0], trace[1], USHORT event id, SerBuffer.
# m_usFromType is a local annotation and is never serialized.

NO_UID = -1

def event_put(event_id, payload, performer=0, uids=(), trace=(NO_UID, NO_UID),
              compressed=False):
    w = Writer()
    w.u32(performer).u32(len(uids))
    for u in uids: w.i64(u)
    w.i64(trace[0]).i64(trace[1])
    w.u16(event_id)
    serbuffer_put(w, payload, compressed)
    return w.take()

def event_get(data):
    r = Reader(data)
    performer = r.u32()
    uids = [r.i64() for _ in range(r.u32())]
    trace = (r.i64(), r.i64())
    event_id = r.u16()
    payload, comp = serbuffer_get(r)
    if comp: payload = uncompress(payload)
    return dict(id=event_id, payload=payload, performer=performer,
                uids=uids, trace=trace, trailing=r.rest())

# --- KSecureBuffer ---------------------------------------------------------
# [SPI u16 LE][seq u32 LE][IV 8][DES-CBC(payload|pad|padLen)][ICV 10]
# The two header words are memcpy'd native - little-endian - while every DWORD
# above them is big-endian. GeneratePadding: n = 8 - ((len + 1) % 8), the pad
# bytes count 1..n and the final byte repeats n.

ICV_LEN = 10

def seal(auth_key, crypto_key, spi, seq, iv, payload):
    head = struct.pack('<H', spi) + struct.pack('<I', seq) + iv
    pad_len = 8 - ((len(payload) + 1) % 8)
    plain = payload + bytes(range(1, pad_len + 1)) + bytes([pad_len])
    des = pyDes.des(crypto_key, pyDes.CBC, iv, pad=None, padmode=pyDes.PAD_NORMAL)
    body = head + des.encrypt(plain)
    icv = hmac.new(auth_key, body, hashlib.md5).digest()[:ICV_LEN]
    return body + icv

def open_frame(auth_key, crypto_key, frame):
    if len(frame) < 2 + 4 + 8 + 1 + ICV_LEN: raise ValueError('too short')
    body, icv = frame[:-ICV_LEN], frame[-ICV_LEN:]
    want = hmac.new(auth_key, body, hashlib.md5).digest()[:ICV_LEN]
    if not hmac.compare_digest(icv, want): raise ValueError('ICV mismatch')
    spi = struct.unpack('<H', body[0:2])[0]
    seq = struct.unpack('<I', body[2:6])[0]
    iv, cipher = body[6:14], body[14:]
    if len(cipher) % 8: raise ValueError('cipher length')
    des = pyDes.des(crypto_key, pyDes.CBC, iv, pad=None, padmode=pyDes.PAD_NORMAL)
    plain = des.decrypt(cipher)
    pad_len = plain[-1]
    if not 1 <= pad_len <= 8: raise ValueError('pad length')
    if plain[-pad_len - 1:-1] != bytes(range(1, pad_len + 1)):
        raise ValueError('pad contents')
    return spi, seq, plain[:-pad_len - 1]

# --- framing ---------------------------------------------------------------
# The USHORT length prefix is little-endian and COUNTS ITSELF.

def frame(secure):
    total = len(secure) + 2
    assert total <= 32768, 'MAX_PACKET_SIZE'
    return struct.pack('<H', total) + secure

def unframe(stream):
    out, i = [], 0
    while i + 2 <= len(stream):
        total = struct.unpack('<H', stream[i:i + 2])[0]
        if total < 27 or i + total > len(stream): break
        out.append(stream[i + 2:i + total]); i += total
    return out, stream[i:]

# --- the SA as it travels inside E_ACCEPT_CONNECTION_NOT -------------------
# SERIALIZE_DEFINE_PUT(KSecurityAssociation): authKey, cryptoKey, seq,
# lastSeq, replayWindowMask. Each key is a KByteStream.

def sa_put(w, auth_key, crypto_key, seq=1, last_seq=0, mask=0):
    return (w.bytestream(auth_key).bytestream(crypto_key)
             .u32(seq).u32(last_seq).u32(mask))

def sa_get(r):
    return dict(auth_key=r.bytestream(), crypto_key=r.bytestream(),
                seq=r.u32(), last_seq=r.u32(), mask=r.u32())
