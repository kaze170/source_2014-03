// ksec/SecureBuffer.h - KSecureBuffer / KSecurityAssociation reimplementation.
//
// Wire layout of one KSecureBuffer (everything after the uint16 length prefix):
//   [SpiType  uint16 LE]        SPI, 0 before E_ACCEPT_CONNECTION_NOT, session SPI afterwards
//   [SeqType  uint32 LE]        sequence number, starts at 1
//   [IV       8 bytes]
//   [cipher   N * 8 bytes]      DES-CBC(payload || pad || padLen)
//   [ICV      10 bytes]         HMAC-MD5(authKey, spi..cipher) truncated to 10 bytes
//
// All the "little endian" in this header refers to how the original code did memcpy of native
// integers into a byte string on Win32/x86 which is little endian, consequently the SPI and the
// sequence number travel little endian even though KSerializer writes big endian.
#pragma once

#include <array>
#include <cstddef>
#include <cstdint>
#include <mutex>
#include <unordered_map>

#include "kcore/Serialize.h"
#include "kcore/Types.h"

namespace knc::sec {

constexpr std::size_t kIvSize = 8;
constexpr std::size_t kIcvSize = 10;
constexpr std::size_t kBlockSize = 8;
constexpr std::size_t kAuthKeySize = 8;
constexpr std::size_t kCryptoKeySize = 8;

// KSecurityAssociation ----------------------------------------------------
class SecurityAssociation {
public:
    SecurityAssociation();

    // Fills both keys with random bytes in [1, 255] (ResetRandomizeKey).
    void resetRandomizeKey();
    void setKeys(const byte authKey[kAuthKeySize], const byte cryptoKey[kCryptoKeySize]);

    const std::array<byte, kAuthKeySize>& authKey() const { return authKey_; }
    const std::array<byte, kCryptoKeySize>& cryptoKey() const { return cryptoKey_; }

    SeqType sequenceNum() const { return sequenceNum_; }
    void setSequenceNum(SeqType n) { sequenceNum_ = n; }

    // notRWM (no replay window mask) variants used by TCP sessions. Validation mutates the
    // recorded last sequence number, exactly like the original implementation.
    void incrSequenceNum_notRWM();
    bool isValidSequenceNum_notRWM(SeqType n);
    SeqType lastSequenceNum() const { return lastSequenceNum_; }

    void serialize(Serializer& s);

private:
    std::array<byte, kAuthKeySize> authKey_{};
    std::array<byte, kCryptoKeySize> cryptoKey_{};
    SeqType sequenceNum_ = 1;
    SeqType lastSequenceNum_ = 0;
    UINT32 replayWindowMask_ = 0;
};

// KSADatabase -------------------------------------------------------------
// Maps SPI -> SecurityAssociation. SPI 0 always holds the built-in default association whose keys
// are the CP949 byte strings from the 2014 client ("임시로만드는키" / "해커들절대모를키").
class SecurityDatabase {
public:
    static SecurityDatabase& instance();

    // Creates a fresh randomized association and returns its SPI (>= 1). The pointer stays valid
    // until remove() is called for that SPI.
    SpiType createNewSA();
    bool insert(USHORT spi, const SecurityAssociation& sa);
    void remove(SpiType spi);
    bool contains(SpiType spi) const;

    // Returns the association for spi; falls back to the SPI 0 default when unknown (the original
    // behaviour of KSADatabase::GetSA).
    SecurityAssociation& get(SpiType spi);
    const SecurityAssociation& get(SpiType spi) const;

    SecurityAssociation& defaultSA();

    std::size_t size() const;

    // Locks the whole database. Sessions are served by many threads so a single mutex keeps the
    // multi threaded access pattern of the original CRITICAL_SECTION.
    std::unique_lock<std::mutex> lock();

private:
    SecurityDatabase();

    mutable std::mutex mutex_;
    std::unordered_map<USHORT, SecurityAssociation> map_;
    SpiType nextSpi_ = 1;
};

// KSecureBuffer -----------------------------------------------------------
class SecureBuffer {
public:
    explicit SecureBuffer(SpiType spi);
    SecureBuffer(SpiType spi, const void* recv, std::size_t bytes);
    SecureBuffer(const SecureBuffer&) = default;
    SecureBuffer& operator=(const SecureBuffer&) = default;

    // Encrypts `payload` into the internal buffer using the notRWM code path (the one used by all
    // TCP sessions). Returns false on a crypto failure.
    bool create_notRWM(const byte* payload, std::size_t len);

    // Authenticates and decrypts a received buffer. When checkSequenceNum is false the sequence
    // number is not validated (used by servers configured with NetLayer:SetCheckSequenceNum(false)).
    bool isAuthentic_notRWM(bool checkSequenceNum);
    // Decrypts after a successful isAuthentic_notRWM() (pass authentic=true to re-authenticate).
    bool getPayload_notRWM(Bytes& payload, bool authentic = true, bool checkSequenceNum = true);

    const byte* dataPtr() const { return buffer_.data(); }
    std::size_t size() const { return buffer_.size(); }
    SpiType spi() const { return spi_; }

    // Worst case wire size for a payload of payloadLen bytes (used to reserve buffers).
    static std::size_t maxSecureSize(std::size_t payloadLen);

private:
    SecurityAssociation& sa() const;

    void generateIv(std::array<byte, kIvSize>& iv) const;
    bool generateCrypt(const byte* payload, std::size_t len, const std::array<byte, kIvSize>& iv,
                       Bytes& cryptOut) const;
    Bytes generatePadding(std::size_t payloadLen) const;
    void generateICV(const byte* data, std::size_t len, byte out[kIcvSize]) const;
    bool isValidSize() const;
    bool isValidICV() const;
    bool isValidPadding(const Bytes& plain, std::size_t& padBytes) const;
    bool isValidSequenceNum_notRWM(SeqType& seq) const;

    SpiType spi_ = 0;
    Bytes buffer_;
};

// Utility used by the unit tests and the tools: build the SPI 0 default keys.
void defaultKeys(std::array<byte, kAuthKeySize>& authKey, std::array<byte, kCryptoKeySize>& cryptoKey);

// Fills `out` with cryptographically strong random bytes (std::random_device based).
void randomBytes(byte* out, std::size_t len);

}  // namespace knc::sec
