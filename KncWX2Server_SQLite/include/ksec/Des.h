// ksec/Des.h - self contained DES (FIPS 46-3) used by the X2 wire protocol.
//
// The original server used CryptoPP::CBC_Mode<CryptoPP::DES> with an 8 byte key.
// Reimplemented here so that both MSVC 2026 and GCC builds stay dependency free.
#pragma once

#include <array>
#include <cstddef>
#include <cstdint>

namespace knc::sec {

class Des {
public:
    static constexpr std::size_t kBlockSize = 8;
    static constexpr std::size_t kKeySize = 8;

    Des() = default;
    explicit Des(const std::uint8_t key[kKeySize]) { setKey(key); }

    void setKey(const std::uint8_t key[kKeySize]);
    // Encrypts/decrypts one 8 byte block in place (host order, big endian bit numbering per FIPS).
    void encryptBlock(std::uint8_t block[kBlockSize]) const;
    void decryptBlock(std::uint8_t block[kBlockSize]) const;

private:
    // 16 round keys, 48 bits each, kept in the low bits of a 64 bit word.
    std::array<std::uint64_t, 16> subKeys_{};
};

// DES-CBC without padding. in/out may alias. len must be a multiple of 8.
void desCbcEncrypt(const Des& des, const std::uint8_t iv[8], const std::uint8_t* in, std::uint8_t* out,
                   std::size_t len);
void desCbcDecrypt(const Des& des, const std::uint8_t iv[8], const std::uint8_t* in, std::uint8_t* out,
                   std::size_t len);

}  // namespace knc::sec
