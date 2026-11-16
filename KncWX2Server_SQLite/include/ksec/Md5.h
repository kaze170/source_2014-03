// ksec/Md5.h - RFC 1321 MD5 and the HMAC-MD5 construction used for the packet ICV.
#pragma once

#include <array>
#include <cstddef>
#include <cstdint>
#include <string>

namespace knc::sec {

class Md5 {
public:
    static constexpr std::size_t kDigestSize = 16;
    static constexpr std::size_t kBlockSize = 64;

    Md5() { reset(); }
    void reset();
    void update(const void* data, std::size_t len);
    void final(std::uint8_t out[kDigestSize]);
    std::array<std::uint8_t, kDigestSize> digest();

private:
    void transform(const std::uint8_t block[kBlockSize]);

    std::uint32_t state_[4]{};
    std::uint64_t bitCount_ = 0;
    std::uint8_t buffer_[kBlockSize]{};
    std::size_t bufferLen_ = 0;
};

// HMAC-MD5 (RFC 2104) truncated to `outLen` bytes; the X2 protocol uses 10 bytes.
void hmacMd5(const void* key, std::size_t keyLen, const void* data, std::size_t dataLen, std::uint8_t* out,
             std::size_t outLen);

}  // namespace knc::sec
