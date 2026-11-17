// ---------------------------------------------------------------------------
// knc.proto.security - the per-connection packet security layer.
//
// Replaces KNCSDK/Include/KncSecurity/{SecurityAssociation,SecureBuffer,
// SADatabase}.{h,cpp}. The scheme is a cut-down IPsec ESP:
//
//   +--------+-----------+--------+---------------------------+---------+
//   | SPI(2) | SeqNum(4) | IV(8)  | DES-CBC( payload | pad |  | ICV(10) |
//   |  LE    |    LE     |        |          padLen )         |         |
//   +--------+-----------+--------+---------------------------+---------+
//   \__________________ ICV input (HMAC-MD5-80) ______________/
//
// Byte-order gotcha: the header SPI and SeqNum are written with
// KByteStream::Append(), a raw memcpy of the native value, so they are
// LITTLE-endian - unlike everything the KSerializer writes inside the
// payload, which is big-endian. (The *same* SPI value is also sent inside the
// handshake payload, where it IS big-endian. Both appear in the tests.)
//
// Padding (RFC 2406 style, from KSecureBuffer::GeneratePadding):
//   padLen = blockSize - ((payloadLen + 1) % blockSize)   -> always 1..8
//   pad    = 0x01 0x02 ... padLen
//   then one trailing byte holding padLen itself
// GetMaxExtraPadBlocks() is 1, but the random source is hard-coded off
// (`unsigned char nRand = 0;`), so nRandBlocks is always 0 and no extra
// blocks are ever added. We reproduce the effective behaviour. The reader
// validates the pad *pattern* rather than assuming a length, so it would
// accept extra blocks if a peer ever sent them.
//
// Handshake
// ---------
// On accept the server mints a fresh SA plus a random SPI and sends
// E_ACCEPT_CONNECTION_NOT carrying {SPI, SA} *protected by SPI 0* - the
// well-known bootstrap SA compiled into both client and server. Only after
// that packet is on the wire does the server switch to the new SPI
// (KSession::OnAcceptConnection: `SendPacket(kEvent); m_nSPIndex = nSpiNew;`).
//
// Important asymmetry, straight from the legacy comment in
// KSecureBuffer::IsValidSPIndex():
//     "060302. florist. 서버와 클라이언트에서 등록된 SPIndex가 달라 가능하도록 수정."
//     (the SPI the two sides record differ, so make it work anyway)
// The SPI *field on the wire is ignored on receive*: `IsValidSPIndex` looks up
// `m_nSPIndex`, the locally stored index, not the one parsed out of the
// packet. Each side decrypts with its own session-local SA. Validating the
// received SPI would break every shipped client.
// ---------------------------------------------------------------------------
#ifdef KNC_CLASSIC
#  pragma once
#else
module;
#endif

#include "../knc_prelude.hpp"
#include <random>

#ifdef KNC_CLASSIC
#  include "../core/types.cppm"
#  include "../crypto/des.cppm"
#  include "../crypto/md5.cppm"
#  include "serializer.cppm"
#else
export module knc.proto.security;
import knc.core.types;
import knc.crypto.des;
import knc.crypto.md5;
import knc.proto.serializer;
#endif

KNC_EXPORT namespace knc::proto {

inline constexpr std::size_t kAuthKeySize = 8;    // KSecurityAssociation::AUTH_KEY_SIZE
inline constexpr std::size_t kCryptoKeySize = 8;  // ::CRYPTO_KEY_SIZE
inline constexpr std::size_t kIvSize = 8;         // GetIVSize()
inline constexpr std::size_t kIcvSize = 10;       // GetICVSize()
inline constexpr std::size_t kBlockSize = 8;      // GetBlockSize()

/// SPI 0 is the bootstrap index; it is never handed out as a session SPI.
inline constexpr SpiType kBootstrapSpi = 0;

/// Smallest possible secure buffer: SPI + Seq + IV + padLen + ICV == 25.
/// Mirrors KSecureBuffer::IsValidSize() exactly.
inline constexpr std::size_t kMinSecureSize =
    sizeof(SpiType) + sizeof(SeqType) + kIvSize + 1 + kIcvSize;
static_assert(kMinSecureSize == 25);

// ---------------------------------------------------------------------------
// The bootstrap SA (SPI 0)
// ---------------------------------------------------------------------------
// KSecurityAssociation's default constructor does:
//     m_bsAuthKey.Assign(   "<CP949 literal>", AUTH_KEY_SIZE   );
//     m_bsCryptoKey.Assign( "<CP949 literal>", CRYPTO_KEY_SIZE );
// taking the first 8 bytes of a CP949-encoded Korean string literal. Those
// exact bytes are baked into the shipped client, so they are reproduced here
// as hex rather than as a source-encoding-dependent literal - a UTF-8 build of
// the original line would silently produce different keys and lock every
// client out.
//
//   auth   : "임시로만드는키"  CP949 = C0D3 BDC3 B7CE B8B8 B5E5 B4C2 C5B0
//                              -> first 8: C0 D3 BD C3 B7 CE B8 B8
//   crypto : "해커들절대모를키" CP949 = C7D8 C4BF B5E9 C0FD B4EB B8F0 B8A6 C5B0
//                              -> first 8: C7 D8 C4 BF B5 E9 C0 FD
//
// Verified by a byte-level dump of
// KNCSDK/Include/KncSecurity/SecurityAssociation.cpp lines 38-39.
inline constexpr std::array<std::uint8_t, kAuthKeySize> kDefaultAuthKey = {
    0xC0, 0xD3, 0xBD, 0xC3, 0xB7, 0xCE, 0xB8, 0xB8};
inline constexpr std::array<std::uint8_t, kCryptoKeySize> kDefaultCryptoKey = {
    0xC7, 0xD8, 0xC4, 0xBF, 0xB5, 0xE9, 0xC0, 0xFD};

// ---------------------------------------------------------------------------
// SecurityAssociation
// ---------------------------------------------------------------------------

class SecurityAssociation {
public:
    /// Default = the bootstrap SA, matching KSecurityAssociation().
    SecurityAssociation()
        : auth_key_(kDefaultAuthKey.begin(), kDefaultAuthKey.end()),
          crypto_key_(kDefaultCryptoKey.begin(), kDefaultCryptoKey.end()) {}

    SecurityAssociation(Bytes auth_key, Bytes crypto_key)
        : auth_key_(std::move(auth_key)), crypto_key_(std::move(crypto_key)) {
        validate_keys();
    }

    /// KSecurityAssociation::ResetRandomizeKey().
    ///
    /// The legacy generator drew from `boost::uniform_int<DWORD>(1, 255)` on a
    /// *default-seeded, function-local static* boost::mt19937 - so every
    /// server process produced the identical key stream from boot, and no key
    /// byte could ever be zero. Both properties are fatal, and neither is
    /// observable by the peer (it just echoes back whatever key it is handed).
    /// We draw the full 0..255 range from the system CSPRNG instead.
    /// See docs/PROTOCOL.md, "Deliberate deviations".
    static SecurityAssociation random() {
        SecurityAssociation sa;
        sa.auth_key_ = random_bytes(kAuthKeySize);
        sa.crypto_key_ = random_bytes(kCryptoKeySize);
        return sa;
    }

    [[nodiscard]] const Bytes& auth_key() const noexcept { return auth_key_; }
    [[nodiscard]] const Bytes& crypto_key() const noexcept { return crypto_key_; }
    [[nodiscard]] SeqType sequence_num() const noexcept { return sequence_num_; }
    [[nodiscard]] SeqType last_sequence_num() const noexcept {
        return last_sequence_num_;
    }
    [[nodiscard]] std::uint32_t replay_window_mask() const noexcept {
        return replay_window_mask_;
    }

    void set_sequence_num(SeqType v) noexcept { sequence_num_ = v; }
    void set_last_sequence_num(SeqType v) noexcept { last_sequence_num_ = v; }

    /// IncrSequenceNum_notRWM(): wraps to 1, never 0.
    void increment_sequence_num() noexcept {
        ++sequence_num_;
        if (sequence_num_ == 0) {
            sequence_num_ = 1;
        }
    }

    /// IsValidSequenceNum_notRWM(): the TCP variant, no replay-window mask.
    /// Strictly increasing, except that it tolerates exactly one wrap from the
    /// maximum. Mutating: it records the accepted number.
    [[nodiscard]] bool accept_sequence_num(SeqType n) noexcept {
        if (n == 0) {
            return false;
        }
        if (n > last_sequence_num_) {
            last_sequence_num_ = n;
            return true;
        }
        if (n == last_sequence_num_) {
            return false;
        }
        if (last_sequence_num_ != std::numeric_limits<SeqType>::max()) {
            return false;
        }
        last_sequence_num_ = n;
        return true;
    }

    [[nodiscard]] Bytes encrypt(ByteView plain, ByteView iv) const {
        return crypto::des_cbc_encrypt(view(crypto_key_), iv, plain);
    }
    [[nodiscard]] Bytes decrypt(ByteView cipher, ByteView iv) const {
        return crypto::des_cbc_decrypt(view(crypto_key_), iv, cipher);
    }
    [[nodiscard]] Bytes icv(ByteView authenticated) const {
        return crypto::hmac_md5(view(auth_key_), authenticated, kIcvSize);
    }

    // --- wire format (sent to the client in E_ACCEPT_CONNECTION_NOT) -------
    // SERIALIZE_DEFINE_PUT(KSecurityAssociation): authKey, cryptoKey,
    // sequenceNum, lastSequenceNum, replayWindowMask.
    // A KByteStream serializes as a big-endian DWORD length then raw bytes.

    void write_to(Writer& w) const {
        w.put_dword(static_cast<std::uint32_t>(auth_key_.size()));
        w.put_raw(view(auth_key_));
        w.put_dword(static_cast<std::uint32_t>(crypto_key_.size()));
        w.put_raw(view(crypto_key_));
        w.put_uint(sequence_num_);
        w.put_uint(last_sequence_num_);
        w.put_uint(replay_window_mask_);
    }

    static SecurityAssociation read_from(Reader& r) {
        SecurityAssociation sa;
        const std::uint32_t auth_len = r.get_dword();
        if (auth_len != kAuthKeySize) {
            throw ProtocolError{"SA auth key must be 8 bytes"};
        }
        sa.auth_key_ = r.get_raw(auth_len);
        const std::uint32_t crypto_len = r.get_dword();
        if (crypto_len != kCryptoKeySize) {
            throw ProtocolError{"SA crypto key must be 8 bytes"};
        }
        sa.crypto_key_ = r.get_raw(crypto_len);
        sa.sequence_num_ = r.get_uint();
        sa.last_sequence_num_ = r.get_uint();
        sa.replay_window_mask_ = r.get_uint();
        return sa;
    }

    [[nodiscard]] bool operator==(const SecurityAssociation& o) const noexcept {
        return auth_key_ == o.auth_key_ && crypto_key_ == o.crypto_key_ &&
               sequence_num_ == o.sequence_num_ &&
               last_sequence_num_ == o.last_sequence_num_ &&
               replay_window_mask_ == o.replay_window_mask_;
    }

    /// Cryptographically strong random bytes.
    static Bytes random_bytes(std::size_t count) {
        static thread_local std::random_device rd;
        std::uniform_int_distribution<int> dist(0, 255);
        Bytes out(count);
        for (auto& b : out) {
            b = static_cast<std::uint8_t>(dist(rd));
        }
        return out;
    }

private:
    void validate_keys() const {
        if (auth_key_.size() != kAuthKeySize) {
            throw ProtocolError{"SA auth key must be 8 bytes"};
        }
        if (crypto_key_.size() != kCryptoKeySize) {
            throw ProtocolError{"SA crypto key must be 8 bytes"};
        }
    }

    Bytes auth_key_;
    Bytes crypto_key_;
    SeqType sequence_num_ = 1;       // legacy ctor: m_nSequenceNum(1)
    SeqType last_sequence_num_ = 0;  // legacy ctor: m_nLastSequenceNum(0)
    std::uint32_t replay_window_mask_ = 0;
};

// ---------------------------------------------------------------------------
// Framing primitives
// ---------------------------------------------------------------------------

/// Why a frame was rejected. Mirrors the checks in IsAuthentic_notRWM() and
/// GetPayload_notRWM().
enum class OpenError {
    kOk,
    kTooShort,
    kBadIcv,
    kReplayedSequence,
    kBadCipherLength,
    kBadPadding,
};

[[nodiscard]] inline const char* to_string(OpenError e) noexcept {
    switch (e) {
        case OpenError::kOk: return "ok";
        case OpenError::kTooShort: return "frame shorter than the 25-byte minimum";
        case OpenError::kBadIcv: return "ICV mismatch";
        case OpenError::kReplayedSequence: return "replayed or stale sequence number";
        case OpenError::kBadCipherLength: return "ciphertext is not a multiple of 8";
        case OpenError::kBadPadding: return "invalid RFC 2406 padding";
    }
    return "unknown";
}

struct OpenResult {
    OpenError error = OpenError::kOk;
    Bytes payload;

    [[nodiscard]] explicit operator bool() const noexcept {
        return error == OpenError::kOk;
    }
};

/// KSecureBuffer::Create_notRWM() with an explicit IV - the deterministic form
/// used by the tests. `spi` is the value written into the header; the caller's
/// own SPI decides whether the sequence counter advances.
[[nodiscard]] inline Bytes seal_with_iv(SecurityAssociation& sa, SpiType spi,
                                        ByteView payload, ByteView iv) {
    if (iv.size() != kIvSize) {
        throw ProtocolError{"seal(): IV must be 8 bytes"};
    }

    Bytes out;
    out.reserve(sizeof(SpiType) + sizeof(SeqType) + kIvSize + payload.size() +
                kBlockSize + kIcvSize);

    // SPI and sequence number: raw native (little-endian) words.
    out.push_back(static_cast<std::uint8_t>(spi));
    out.push_back(static_cast<std::uint8_t>(spi >> 8));
    const SeqType seq = sa.sequence_num();
    out.push_back(static_cast<std::uint8_t>(seq));
    out.push_back(static_cast<std::uint8_t>(seq >> 8));
    out.push_back(static_cast<std::uint8_t>(seq >> 16));
    out.push_back(static_cast<std::uint8_t>(seq >> 24));

    append(out, iv);

    // payload | pad | padLen, then DES-CBC.
    Bytes plain;
    const std::size_t pad_len = kBlockSize - ((payload.size() + 1) % kBlockSize);
    plain.reserve(payload.size() + pad_len + 1);
    append(plain, payload);
    for (std::size_t i = 1; i <= pad_len; ++i) {
        plain.push_back(static_cast<std::uint8_t>(i));
    }
    plain.push_back(static_cast<std::uint8_t>(pad_len));

    const Bytes cipher = sa.encrypt(view(plain), iv);
    append(out, view(cipher));

    // ICV over everything written so far.
    append(out, view(sa.icv(view(out))));

    // "SPIndex가 0이 아닌 경우에만 sequence number를 증가!"
    if (spi != kBootstrapSpi) {
        sa.increment_sequence_num();
    }
    return out;
}

/// Generate the IV internally.
///
/// The legacy GenerateIV() filled all 8 bytes with the *same* character,
/// `'A' + rand() % 40`, from an unseeded CRT rand(). That is 40 distinct IVs
/// drawn from a fully predictable sequence - with a fixed key, identical
/// plaintext blocks then produce identical ciphertext across sessions. The IV
/// travels in clear and the peer simply reads it, so replacing it with CSPRNG
/// bytes is invisible on the wire and strictly better.
[[nodiscard]] inline Bytes seal(SecurityAssociation& sa, SpiType spi,
                                ByteView payload) {
    const Bytes iv = SecurityAssociation::random_bytes(kIvSize);
    return seal_with_iv(sa, spi, payload, view(iv));
}

/// KSecureBuffer::IsAuthentic_notRWM() + GetPayload_notRWM().
///
/// `check_sequence` mirrors the legacy `bCheckSequenceNum` argument. The
/// legacy code additionally skips the check whenever the *local* SPI is 0
/// (`if( !IsDefaultSPIndex() && bCheckSequenceNum )`), because the bootstrap
/// SA is shared by every connection and its counter is meaningless. Callers
/// should pass `check_sequence = false` while still on SPI 0; SecureChannel
/// below does that for you.
[[nodiscard]] inline OpenResult open(SecurityAssociation& sa, ByteView frame,
                                     bool check_sequence) {
    OpenResult result;

    if (frame.size() < kMinSecureSize) {
        result.error = OpenError::kTooShort;
        return result;
    }

    // --- ICV first: never touch the ciphertext before it is authenticated --
    const std::size_t icv_offset = frame.size() - kIcvSize;
    const ByteView received_icv = frame.subspan(icv_offset, kIcvSize);
    const Bytes expected_icv = sa.icv(frame.subspan(0, icv_offset));
    if (!constant_time_equal(received_icv, view(expected_icv))) {
        result.error = OpenError::kBadIcv;
        return result;
    }

    // --- sequence number (little-endian, immediately after the SPI) --------
    if (check_sequence) {
        const SeqType seq = static_cast<SeqType>(frame[2]) |
                            (static_cast<SeqType>(frame[3]) << 8) |
                            (static_cast<SeqType>(frame[4]) << 16) |
                            (static_cast<SeqType>(frame[5]) << 24);
        if (!sa.accept_sequence_num(seq)) {
            result.error = OpenError::kReplayedSequence;
            return result;
        }
    }

    // --- decrypt -----------------------------------------------------------
    constexpr std::size_t kHeader = sizeof(SpiType) + sizeof(SeqType) + kIvSize;
    const ByteView iv = frame.subspan(sizeof(SpiType) + sizeof(SeqType), kIvSize);
    const ByteView cipher = frame.subspan(kHeader, icv_offset - kHeader);

    // The legacy code fed a non-multiple-of-8 length straight into
    // CryptoPP::CBC_Mode::ProcessData. We refuse it instead.
    if (cipher.empty() || (cipher.size() % kBlockSize) != 0) {
        result.error = OpenError::kBadCipherLength;
        return result;
    }

    Bytes plain = sa.decrypt(cipher, iv);

    // --- strip and verify padding (IsValidPadding) -------------------------
    const std::size_t pad_len = plain.back();
    if (pad_len + 1 > plain.size()) {
        result.error = OpenError::kBadPadding;
        return result;
    }
    const std::size_t payload_len = plain.size() - pad_len - 1;
    for (std::size_t i = 0; i < pad_len; ++i) {
        if (plain[payload_len + i] != static_cast<std::uint8_t>(i + 1)) {
            result.error = OpenError::kBadPadding;
            return result;
        }
    }

    plain.resize(payload_len);
    result.payload = std::move(plain);
    return result;
}

/// Read the SPI a frame claims. Informational only - see the header comment on
/// why it must never gate acceptance.
[[nodiscard]] inline SpiType peek_spi(ByteView frame) {
    if (frame.size() < sizeof(SpiType)) {
        throw ProtocolError{"frame too short to contain an SPI"};
    }
    return static_cast<SpiType>(static_cast<std::uint16_t>(frame[0]) |
                                (static_cast<std::uint16_t>(frame[1]) << 8));
}

/// Read the sequence number a frame claims. Informational only.
[[nodiscard]] inline SeqType peek_sequence(ByteView frame) {
    if (frame.size() < sizeof(SpiType) + sizeof(SeqType)) {
        throw ProtocolError{"frame too short to contain a sequence number"};
    }
    return static_cast<SeqType>(frame[2]) | (static_cast<SeqType>(frame[3]) << 8) |
           (static_cast<SeqType>(frame[4]) << 16) |
           (static_cast<SeqType>(frame[5]) << 24);
}

// ---------------------------------------------------------------------------
// SecureChannel - one connection's view of the security layer
// ---------------------------------------------------------------------------

/// Replaces the (KSession::m_nSPIndex + global KSADatabase) pair.
///
/// The legacy SADB was a process-wide `std::map<SpiType, KSecurityAssociation>`
/// behind a CRITICAL_SECTION, consulted on every single packet, even though a
/// session only ever uses its own SA. That global lock was pure contention and
/// a lifetime hazard (entries were never erased on disconnect, so the map grew
/// for the life of the process). Here the SA simply lives in the session.
class SecureChannel {
public:
    /// A freshly accepted connection starts on the bootstrap SA.
    SecureChannel() = default;

    [[nodiscard]] SpiType spi() const noexcept { return spi_; }
    [[nodiscard]] bool on_bootstrap_sa() const noexcept {
        return spi_ == kBootstrapSpi;
    }
    [[nodiscard]] SecurityAssociation& sa() noexcept { return sa_; }
    [[nodiscard]] const SecurityAssociation& sa() const noexcept { return sa_; }

    /// Whether inbound sequence numbers are enforced. KSession leaves this
    /// off by default (`m_bCheckSequenceNum(false)`); it is also forced off
    /// while still on the bootstrap SA.
    void set_check_sequence(bool on) noexcept { check_sequence_ = on; }
    [[nodiscard]] bool check_sequence() const noexcept { return check_sequence_; }

    /// Adopt the session SA. Call this AFTER the E_ACCEPT_CONNECTION_NOT frame
    /// has been sealed, matching KSession::OnAcceptConnection's ordering.
    void adopt(SpiType spi, SecurityAssociation sa) {
        spi_ = spi;
        sa_ = std::move(sa);
    }

    [[nodiscard]] Bytes seal(ByteView payload) {
        return proto::seal(sa_, spi_, payload);
    }

    [[nodiscard]] Bytes seal_with_iv(ByteView payload, ByteView iv) {
        return proto::seal_with_iv(sa_, spi_, payload, iv);
    }

    [[nodiscard]] OpenResult open(ByteView frame) {
        return proto::open(sa_, frame, check_sequence_ && !on_bootstrap_sa());
    }

private:
    SecurityAssociation sa_{};  // bootstrap SA
    SpiType spi_ = kBootstrapSpi;
    bool check_sequence_ = false;
};

/// Mint a session SPI + SA pair. Replaces KSADatabase::CreateNewSA().
struct FreshSa {
    SpiType spi;
    SecurityAssociation sa;
};

[[nodiscard]] inline FreshSa create_sa() {
    // KSADatabase::Insert drew from uniform_int<DWORD>(1, 65535) and retried
    // on collision; with per-session storage there is nothing to collide with,
    // but SPI 0 stays reserved for the bootstrap SA.
    SpiType spi = 0;
    while (spi == kBootstrapSpi) {
        const Bytes r = SecurityAssociation::random_bytes(2);
        spi = static_cast<SpiType>(static_cast<std::uint16_t>(r[0]) |
                                   (static_cast<std::uint16_t>(r[1]) << 8));
    }
    return FreshSa{spi, SecurityAssociation::random()};
}

}  // namespace knc::proto
