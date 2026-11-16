#include "ksec/Md5.h"

#include <cstring>

namespace knc::sec {
namespace {

constexpr std::uint32_t kS[64] = {7, 12, 17, 22, 7, 12, 17, 22, 7, 12, 17, 22, 7, 12, 17, 22,
                                  5, 9,  14, 20, 5, 9,  14, 20, 5, 9,  14, 20, 5, 9,  14, 20,
                                  4, 11, 16, 23, 4, 11, 16, 23, 4, 11, 16, 23, 4, 11, 16, 23,
                                  6, 10, 15, 21, 6, 10, 15, 21, 6, 10, 15, 21, 6, 10, 15, 21};

constexpr std::uint32_t kK[64] = {
    0xd76aa478, 0xe8c7b756, 0x242070db, 0xc1bdceee, 0xf57c0faf, 0x4787c62a, 0xa8304613, 0xfd469501,
    0x698098d8, 0x8b44f7af, 0xffff5bb1, 0x895cd7be, 0x6b901122, 0xfd987193, 0xa679438e, 0x49b40821,
    0xf61e2562, 0xc040b340, 0x265e5a51, 0xe9b6c7aa, 0xd62f105d, 0x02441453, 0xd8a1e681, 0xe7d3fbc8,
    0x21e1cde6, 0xc33707d6, 0xf4d50d87, 0x455a14ed, 0xa9e3e905, 0xfcefa3f8, 0x676f02d9, 0x8d2a4c8a,
    0xfffa3942, 0x8771f681, 0x6d9d6122, 0xfde5380c, 0xa4beea44, 0x4bdecfa9, 0xf6bb4b60, 0xbebfbc70,
    0x289b7ec6, 0xeaa127fa, 0xd4ef3085, 0x04881d05, 0xd9d4d039, 0xe6db99e5, 0x1fa27cf8, 0xc4ac5665,
    0xf4292244, 0x432aff97, 0xab9423a7, 0xfc93a039, 0x655b59c3, 0x8f0ccc92, 0xffeff47d, 0x85845dd1,
    0x6fa87e4f, 0xfe2ce6e0, 0xa3014314, 0x4e0811a1, 0xf7537e82, 0xbd3af235, 0x2ad7d2bb, 0xeb86d391};

inline std::uint32_t rotl(std::uint32_t x, std::uint32_t c) { return (x << c) | (x >> (32 - c)); }

}  // namespace

void Md5::reset() {
    state_[0] = 0x67452301;
    state_[1] = 0xefcdab89;
    state_[2] = 0x98badcfe;
    state_[3] = 0x10325476;
    bitCount_ = 0;
    bufferLen_ = 0;
    std::memset(buffer_, 0, sizeof(buffer_));
}

void Md5::transform(const std::uint8_t block[64]) {
    std::uint32_t m[16];
    for (int i = 0; i < 16; ++i) {
        m[i] = static_cast<std::uint32_t>(block[i * 4]) | (static_cast<std::uint32_t>(block[i * 4 + 1]) << 8) |
               (static_cast<std::uint32_t>(block[i * 4 + 2]) << 16) |
               (static_cast<std::uint32_t>(block[i * 4 + 3]) << 24);
    }

    std::uint32_t a = state_[0], b = state_[1], c = state_[2], d = state_[3];
    for (std::uint32_t i = 0; i < 64; ++i) {
        std::uint32_t f;
        std::uint32_t g;
        if (i < 16) {
            f = (b & c) | (~b & d);
            g = i;
        } else if (i < 32) {
            f = (d & b) | (~d & c);
            g = (5 * i + 1) % 16;
        } else if (i < 48) {
            f = b ^ c ^ d;
            g = (3 * i + 5) % 16;
        } else {
            f = c ^ (b | ~d);
            g = (7 * i) % 16;
        }
        const std::uint32_t tmp = d;
        d = c;
        c = b;
        b = b + rotl(a + f + kK[i] + m[g], kS[i]);
        a = tmp;
    }

    state_[0] += a;
    state_[1] += b;
    state_[2] += c;
    state_[3] += d;
}

void Md5::update(const void* data, std::size_t len) {
    const auto* p = static_cast<const std::uint8_t*>(data);
    bitCount_ += static_cast<std::uint64_t>(len) * 8;
    while (len > 0) {
        const std::size_t space = kBlockSize - bufferLen_;
        const std::size_t take = (len < space) ? len : space;
        std::memcpy(buffer_ + bufferLen_, p, take);
        bufferLen_ += take;
        p += take;
        len -= take;
        if (bufferLen_ == kBlockSize) {
            transform(buffer_);
            bufferLen_ = 0;
        }
    }
}

void Md5::final(std::uint8_t out[kDigestSize]) {
    const std::uint64_t bits = bitCount_;
    const std::uint8_t pad = 0x80;
    update(&pad, 1);
    const std::uint8_t zero = 0;
    while (bufferLen_ != 56) update(&zero, 1);

    std::uint8_t lenBytes[8];
    for (int i = 0; i < 8; ++i) lenBytes[i] = static_cast<std::uint8_t>((bits >> (8 * i)) & 0xFF);
    std::memcpy(buffer_ + bufferLen_, lenBytes, 8);
    transform(buffer_);
    bufferLen_ = 0;

    for (int i = 0; i < 4; ++i) {
        out[i * 4] = static_cast<std::uint8_t>(state_[i] & 0xFF);
        out[i * 4 + 1] = static_cast<std::uint8_t>((state_[i] >> 8) & 0xFF);
        out[i * 4 + 2] = static_cast<std::uint8_t>((state_[i] >> 16) & 0xFF);
        out[i * 4 + 3] = static_cast<std::uint8_t>((state_[i] >> 24) & 0xFF);
    }
}

std::array<std::uint8_t, Md5::kDigestSize> Md5::digest() {
    std::array<std::uint8_t, kDigestSize> out{};
    final(out.data());
    return out;
}

void hmacMd5(const void* key, std::size_t keyLen, const void* data, std::size_t dataLen, std::uint8_t* out,
             std::size_t outLen) {
    std::uint8_t k[64];
    std::memset(k, 0, sizeof(k));
    if (keyLen > 64) {
        Md5 kh;
        kh.update(key, keyLen);
        kh.final(k);
    } else if (keyLen > 0) {
        std::memcpy(k, key, keyLen);
    }

    std::uint8_t ipad[64];
    std::uint8_t opad[64];
    for (int i = 0; i < 64; ++i) {
        ipad[i] = static_cast<std::uint8_t>(k[i] ^ 0x36);
        opad[i] = static_cast<std::uint8_t>(k[i] ^ 0x5C);
    }

    std::uint8_t inner[Md5::kDigestSize];
    {
        Md5 h;
        h.update(ipad, 64);
        if (dataLen != 0) h.update(data, dataLen);
        h.final(inner);
    }

    std::uint8_t full[Md5::kDigestSize];
    {
        Md5 h;
        h.update(opad, 64);
        h.update(inner, sizeof(inner));
        h.final(full);
    }

    const std::size_t n = (outLen < Md5::kDigestSize) ? outLen : Md5::kDigestSize;
    std::memcpy(out, full, n);
}

}  // namespace knc::sec
