#include "ksec/SecureBuffer.h"

#include <algorithm>
#include <chrono>
#include <cstring>
#include <limits>
#include <random>

#include "kcore/Text.h"
#include "kcore/Types.h"
#include "ksec/Des.h"
#include "ksec/Md5.h"

namespace knc::sec {
namespace {

// ---------------------------------------------------------------------------
// The SPI 0 default association keys are the first eight CP949 bytes of the two Korean strings
// the 2014 client and server both embed:
//     auth key   : "임시로만드는키"    -> c0 d3 bd c3 b7 ce b8 b8
//     crypto key : "해커들절대모를키"  -> c7 d8 c4 bf b5 e9 c0 fd
// Reproduced from the literal text so the bytes are obviously correct rather than magic numbers.
constexpr byte kDefaultAuthKey[8] = {0xC0, 0xD3, 0xBD, 0xC3, 0xB7, 0xCE, 0xB8, 0xB8};
constexpr byte kDefaultCryptoKey[8] = {0xC7, 0xD8, 0xC4, 0xBF, 0xB5, 0xE9, 0xC0, 0xFD};

std::mt19937_64& rng() {
    static std::mutex seedMutex;
    static std::mt19937_64 engine([] {
        std::random_device rd;
        std::seed_seq seq{rd(), rd(), rd(), rd(),
                          static_cast<unsigned>(
                              std::chrono::high_resolution_clock::now().time_since_epoch().count()),
                          static_cast<unsigned>(
                              std::chrono::steady_clock::now().time_since_epoch().count())};
        return std::mt19937_64(seq);
    }());
    return engine;
}

std::mutex& rngMutex() {
    static std::mutex mutex;
    return mutex;
}

inline void writeLe16(byte* p, USHORT v) {
    p[0] = static_cast<byte>(v & 0xFF);
    p[1] = static_cast<byte>((v >> 8) & 0xFF);
}
inline USHORT readLe16(const byte* p) { return static_cast<USHORT>(p[0] | (p[1] << 8)); }
inline void writeLe32(byte* p, UINT32 v) {
    p[0] = static_cast<byte>(v & 0xFF);
    p[1] = static_cast<byte>((v >> 8) & 0xFF);
    p[2] = static_cast<byte>((v >> 16) & 0xFF);
    p[3] = static_cast<byte>((v >> 24) & 0xFF);
}
inline UINT32 readLe32(const byte* p) {
    return static_cast<UINT32>(p[0]) | (static_cast<UINT32>(p[1]) << 8) | (static_cast<UINT32>(p[2]) << 16) |
           (static_cast<UINT32>(p[3]) << 24);
}

}  // namespace

void randomBytes(byte* out, std::size_t len) {
    std::lock_guard<std::mutex> guard(rngMutex());
    auto& engine = rng();
    for (std::size_t i = 0; i < len; ++i) out[i] = static_cast<byte>(engine() & 0xFF);
}

void defaultKeys(std::array<byte, kAuthKeySize>& authKey, std::array<byte, kCryptoKeySize>& cryptoKey) {
    std::memcpy(authKey.data(), kDefaultAuthKey, kAuthKeySize);
    std::memcpy(cryptoKey.data(), kDefaultCryptoKey, kCryptoKeySize);
}

// ---------------------------------------------------------------------------
SecurityAssociation::SecurityAssociation() { defaultKeys(authKey_, cryptoKey_); }

void SecurityAssociation::resetRandomizeKey() {
    std::lock_guard<std::mutex> guard(rngMutex());
    auto& engine = rng();
    // boost::uniform_int(1, 255)
    for (auto& b : authKey_) b = static_cast<byte>(1 + (engine() % 255));
    for (auto& b : cryptoKey_) b = static_cast<byte>(1 + (engine() % 255));
}

void SecurityAssociation::setKeys(const byte authKey[kAuthKeySize], const byte cryptoKey[kCryptoKeySize]) {
    std::memcpy(authKey_.data(), authKey, kAuthKeySize);
    std::memcpy(cryptoKey_.data(), cryptoKey, kCryptoKeySize);
}

void SecurityAssociation::incrSequenceNum_notRWM() {
    ++sequenceNum_;
    if (sequenceNum_ == 0) sequenceNum_ = 1;  // sequence numbers are never 0
}

bool SecurityAssociation::isValidSequenceNum_notRWM(SeqType n) {
    if (n == 0) return false;
    if (n > lastSequenceNum_) {
        lastSequenceNum_ = n;
        return true;
    }
    if (n == lastSequenceNum_) return false;
    if (lastSequenceNum_ != 0xFFFFFFFFu) return false;
    lastSequenceNum_ = n;
    return true;
}

void SecurityAssociation::serialize(Serializer& s) {
    // SERIALIZE_DEFINE_PUT(KSecurityAssociation): authKey, cryptoKey (both KByteStream == byte
    // string with a DWORD length prefix) followed by three 32 bit values.
    if (s.writing()) {
        s.put(static_cast<DWORD>(kAuthKeySize));
        s.putBytes(authKey_.data(), kAuthKeySize);
        s.put(static_cast<DWORD>(kCryptoKeySize));
        s.putBytes(cryptoKey_.data(), kCryptoKeySize);
        s.put(sequenceNum_);
        s.put(lastSequenceNum_);
        s.put(replayWindowMask_);
    } else {
        DWORD len = 0;
        s.get(len);
        if (s.failed()) return;
        std::array<byte, kAuthKeySize> auth{};
        const std::size_t authLen = std::min<std::size_t>(len, kAuthKeySize);
        if (authLen) s.getBytes(auth.data(), authLen);
        authKey_ = auth;

        s.get(len);
        if (s.failed()) return;
        std::array<byte, kCryptoKeySize> crypt{};
        const std::size_t cryptLen = std::min<std::size_t>(len, kCryptoKeySize);
        if (cryptLen) s.getBytes(crypt.data(), cryptLen);
        cryptoKey_ = crypt;

        s.get(sequenceNum_);
        s.get(lastSequenceNum_);
        s.get(replayWindowMask_);
    }
}

// ---------------------------------------------------------------------------
SecurityDatabase::SecurityDatabase() { map_.emplace(0, SecurityAssociation()); }

SecurityDatabase& SecurityDatabase::instance() {
    static SecurityDatabase db;
    return db;
}

std::unique_lock<std::mutex> SecurityDatabase::lock() { return std::unique_lock<std::mutex>(mutex_); }

SpiType SecurityDatabase::createNewSA() {
    std::lock_guard<std::mutex> guard(mutex_);
    SecurityAssociation association;
    association.resetRandomizeKey();

    std::lock_guard<std::mutex> rngGuard(rngMutex());
    auto& engine = rng();
    for (;;) {
        const USHORT candidate = static_cast<USHORT>(1 + (engine() % std::numeric_limits<USHORT>::max()));
        if (candidate == 0) continue;
        if (map_.find(candidate) != map_.end()) continue;
        map_.emplace(candidate, association);
        return candidate;
    }
}

bool SecurityDatabase::insert(USHORT spiIndex, const SecurityAssociation& association) {
    std::lock_guard<std::mutex> guard(mutex_);
    return map_.emplace(spiIndex, association).second;
}

void SecurityDatabase::remove(SpiType spi) {
    if (spi == 0) return;  // the default association is permanent
    std::lock_guard<std::mutex> guard(mutex_);
    map_.erase(spi);
}

bool SecurityDatabase::contains(SpiType spi) const {
    std::lock_guard<std::mutex> guard(mutex_);
    return map_.find(spi) != map_.end();
}

SecurityAssociation& SecurityDatabase::get(SpiType spi) {
    std::lock_guard<std::mutex> guard(mutex_);
    auto it = map_.find(spi);
    if (it == map_.end()) it = map_.find(0);
    return it->second;
}

const SecurityAssociation& SecurityDatabase::get(SpiType spi) const {
    std::lock_guard<std::mutex> guard(mutex_);
    auto it = map_.find(spi);
    if (it == map_.end()) it = map_.find(0);
    return it->second;
}

SecurityAssociation& SecurityDatabase::defaultSA() { return get(0); }

std::size_t SecurityDatabase::size() const {
    std::lock_guard<std::mutex> guard(mutex_);
    return map_.size();
}

// ---------------------------------------------------------------------------
SecureBuffer::SecureBuffer(SpiType spi) : spi_(spi) {}

SecureBuffer::SecureBuffer(SpiType spi, const void* recv, std::size_t bytes) : spi_(spi) {
    const auto* p = static_cast<const byte*>(recv);
    buffer_.assign(p, p + bytes);
}

SecurityAssociation& SecureBuffer::sa() const { return SecurityDatabase::instance().get(spi_); }

std::size_t SecureBuffer::maxSecureSize(std::size_t payloadLen) {
    const std::size_t pad = kBlockSize - ((payloadLen + 1) % kBlockSize) + kBlockSize;
    return sizeof(USHORT) + sizeof(UINT32) + kIvSize + payloadLen + pad + 1 + kIcvSize;
}

void SecureBuffer::generateIv(std::array<byte, kIvSize>& iv) const {
    // Original: "'A' + rand() % 40". The IV travels in clear and is only used to chain the CBC
    // blocks, so reproducible-but-unpredictable-enough bytes are fine.
    std::lock_guard<std::mutex> guard(rngMutex());
    auto& engine = rng();
    for (auto& b : iv) b = static_cast<byte>('A' + (engine() % 40));
}

Bytes SecureBuffer::generatePadding(std::size_t payloadLen) const {
    // nPadBytes = blockBytes - ((payload.len + 1) % blockBytes), plus one optional extra block.
    // The original sets nRand = 0 (the random extra padding is disabled) so exactly one variant
    // exists on the wire.
    const std::size_t padBytes = kBlockSize - ((payloadLen + 1) % kBlockSize);
    Bytes pad(padBytes);
    for (std::size_t i = 0; i < padBytes; ++i) pad[i] = static_cast<byte>(i + 1);
    return pad;
}

bool SecureBuffer::generateCrypt(const byte* payload, std::size_t len, const std::array<byte, kIvSize>& iv,
                                 Bytes& cryptOut) const {
    const Bytes pad = generatePadding(len);
    Bytes plain;
    plain.reserve(len + pad.size() + 1);
    plain.insert(plain.end(), payload, payload + len);
    plain.insert(plain.end(), pad.begin(), pad.end());
    plain.push_back(static_cast<byte>(pad.size()));
    if (plain.size() % kBlockSize != 0) return false;

    cryptOut.assign(plain.size(), 0);
    const Des des(sa().cryptoKey().data());
    desCbcEncrypt(des, iv.data(), plain.data(), cryptOut.data(), plain.size());
    return true;
}

void SecureBuffer::generateICV(const byte* data, std::size_t len, byte out[kIcvSize]) const {
    hmacMd5(sa().authKey().data(), kAuthKeySize, data, len, out, kIcvSize);
}

bool SecureBuffer::create_notRWM(const byte* payload, std::size_t len) {
    buffer_.clear();
    buffer_.reserve(maxSecureSize(len));

    buffer_.resize(sizeof(USHORT));
    writeLe16(buffer_.data(), spi_);
    buffer_.resize(sizeof(USHORT) + sizeof(UINT32));
    writeLe32(buffer_.data() + sizeof(USHORT), sa().sequenceNum());

    std::array<byte, kIvSize> iv{};
    generateIv(iv);
    buffer_.insert(buffer_.end(), iv.begin(), iv.end());

    Bytes crypt;
    if (!generateCrypt(payload, len, iv, crypt)) return false;
    buffer_.insert(buffer_.end(), crypt.begin(), crypt.end());

    std::array<byte, kIcvSize> icv{};
    generateICV(buffer_.data(), buffer_.size(), icv.data());
    buffer_.insert(buffer_.end(), icv.begin(), icv.end());

    // Sequence numbers only advance for sessions that own a real SPI (not for SPI 0).
    if (spi_ != 0) sa().incrSequenceNum_notRWM();
    return true;
}

bool SecureBuffer::isValidSize() const {
    const std::size_t minSize = sizeof(USHORT) + sizeof(UINT32) + kIvSize + 1 + kIcvSize;
    return buffer_.size() >= minSize;
}

bool SecureBuffer::isValidICV() const {
    if (buffer_.size() < kIcvSize) return false;
    const std::size_t authLen = buffer_.size() - kIcvSize;
    std::array<byte, kIcvSize> expected{};
    generateICV(buffer_.data(), authLen, expected.data());
    return std::memcmp(expected.data(), buffer_.data() + authLen, kIcvSize) == 0;
}

bool SecureBuffer::isValidPadding(const Bytes& plain, std::size_t& padBytes) const {
    if (plain.empty()) return false;
    padBytes = plain.back();
    if (padBytes + 1 > plain.size()) return false;
    const std::size_t start = plain.size() - padBytes - 1;
    for (std::size_t i = 1; i <= padBytes; ++i) {
        if (plain[start + i - 1] != static_cast<byte>(i)) return false;
    }
    return true;
}

bool SecureBuffer::isValidSequenceNum_notRWM(SeqType& seq) const {
    if (buffer_.size() < sizeof(USHORT) + sizeof(UINT32)) return false;
    seq = readLe32(buffer_.data() + sizeof(USHORT));
    return sa().isValidSequenceNum_notRWM(seq);
}

bool SecureBuffer::isAuthentic_notRWM(bool checkSequenceNum) {
    // The server validates against its own SPI, never against the SPI echoed by the client
    // (SecureBuffer.cpp: "IsValidSPIndex" only checks GetSADB().Find(m_nSPIndex)).
    if (!SecurityDatabase::instance().contains(spi_)) return false;
    if (!isValidSize()) return false;
    if (!isValidICV()) return false;
    if (spi_ != 0 && checkSequenceNum) {
        SeqType seq = 0;
        if (!isValidSequenceNum_notRWM(seq)) return false;
    }
    return true;
}

bool SecureBuffer::getPayload_notRWM(Bytes& payload, bool authentic, bool checkSequenceNum) {
    payload.clear();
    if (authentic && !isAuthentic_notRWM(checkSequenceNum)) return false;

    const std::size_t headerLen = sizeof(USHORT) + sizeof(UINT32) + kIvSize;
    if (buffer_.size() < headerLen + kIcvSize + 1) return false;

    std::array<byte, kIvSize> iv{};
    std::memcpy(iv.data(), buffer_.data() + sizeof(USHORT) + sizeof(UINT32), kIvSize);

    const std::size_t cryptLen = buffer_.size() - headerLen - kIcvSize;
    if (cryptLen == 0 || cryptLen % kBlockSize != 0) return false;

    Bytes plain(cryptLen);
    const Des des(sa().cryptoKey().data());
    desCbcDecrypt(des, iv.data(), buffer_.data() + headerLen, plain.data(), cryptLen);

    std::size_t padBytes = 0;
    if (!isValidPadding(plain, padBytes)) return false;
    plain.resize(plain.size() - padBytes - 1);
    payload.swap(plain);
    return true;
}

}  // namespace knc::sec
