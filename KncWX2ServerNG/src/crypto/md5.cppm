// ---------------------------------------------------------------------------
// knc.crypto.md5 - RFC 1321 MD5 and RFC 2104 HMAC-MD5.
//
// Replaces CryptoPP::HMAC<CryptoPP::MD5> used by
// KSecurityAssociation::GenerateICV (KNCSDK/Include/KncSecurity/
// SecurityAssociation.cpp), which calls mac.TruncatedFinal(&icv[0], 10) - i.e.
// HMAC-MD5-80, the first 10 bytes of the 16-byte tag.
//
// MD5 is cryptographically broken.  It is implemented here solely because the
// 2014 client computes packet ICVs with it; see docs/PROTOCOL.md.
// ---------------------------------------------------------------------------
#ifdef KNC_CLASSIC
#  pragma once
#else
module;
#endif

#include "../knc_prelude.hpp"

#ifdef KNC_CLASSIC
#  include "../core/types.cppm"
#else
export module knc.crypto.md5;
import knc.core.types;
#endif

namespace knc::crypto::detail {

inline constexpr std::uint32_t kMd5K[64] = {
    0xd76aa478, 0xe8c7b756, 0x242070db, 0xc1bdceee, 0xf57c0faf, 0x4787c62a,
    0xa8304613, 0xfd469501, 0x698098d8, 0x8b44f7af, 0xffff5bb1, 0x895cd7be,
    0x6b901122, 0xfd987193, 0xa679438e, 0x49b40821, 0xf61e2562, 0xc040b340,
    0x265e5a51, 0xe9b6c7aa, 0xd62f105d, 0x02441453, 0xd8a1e681, 0xe7d3fbc8,
    0x21e1cde6, 0xc33707d6, 0xf4d50d87, 0x455a14ed, 0xa9e3e905, 0xfcefa3f8,
    0x676f02d9, 0x8d2a4c8a, 0xfffa3942, 0x8771f681, 0x6d9d6122, 0xfde5380c,
    0xa4beea44, 0x4bdecfa9, 0xf6bb4b60, 0xbebfbc70, 0x289b7ec6, 0xeaa127fa,
    0xd4ef3085, 0x04881d05, 0xd9d4d039, 0xe6db99e5, 0x1fa27cf8, 0xc4ac5665,
    0xf4292244, 0x432aff97, 0xab9423a7, 0xfc93a039, 0x655b59c3, 0x8f0ccc92,
    0xffeff47d, 0x85845dd1, 0x6fa87e4f, 0xfe2ce6e0, 0xa3014314, 0x4e0811a1,
    0xf7537e82, 0xbd3af235, 0x2ad7d2bb, 0xeb86d391};

inline constexpr std::uint8_t kMd5S[64] = {
    7, 12, 17, 22, 7, 12, 17, 22, 7, 12, 17, 22, 7, 12, 17, 22,
    5,  9, 14, 20, 5,  9, 14, 20, 5,  9, 14, 20, 5,  9, 14, 20,
    4, 11, 16, 23, 4, 11, 16, 23, 4, 11, 16, 23, 4, 11, 16, 23,
    6, 10, 15, 21, 6, 10, 15, 21, 6, 10, 15, 21, 6, 10, 15, 21};

}  // namespace knc::crypto::detail

KNC_EXPORT namespace knc::crypto {

/// Streaming MD5.  Little-endian word order, per RFC 1321.
class Md5 {
public:
    static constexpr std::size_t kDigestSize = 16;
    static constexpr std::size_t kBlockSize = 64;

    void update(ByteView data) {
        length_ += static_cast<std::uint64_t>(data.size());
        std::size_t offset = 0;
        if (buffered_ != 0) {
            const std::size_t take =
                std::min(kBlockSize - buffered_, data.size());
            std::memcpy(buffer_.data() + buffered_, data.data(), take);
            buffered_ += take;
            offset = take;
            if (buffered_ == kBlockSize) {
                transform(buffer_.data());
                buffered_ = 0;
            }
        }
        for (; offset + kBlockSize <= data.size(); offset += kBlockSize) {
            transform(data.data() + offset);
        }
        if (offset < data.size()) {
            buffered_ = data.size() - offset;
            std::memcpy(buffer_.data(), data.data() + offset, buffered_);
        }
    }

    [[nodiscard]] std::array<std::uint8_t, kDigestSize> finish() {
        const std::uint64_t bit_length = length_ * 8;

        std::array<std::uint8_t, 1> pad_one{0x80};
        update(ByteView{pad_one.data(), pad_one.size()});

        static constexpr std::array<std::uint8_t, kBlockSize> zeros{};
        while (buffered_ != 56) {
            const std::size_t need = (buffered_ < 56) ? (56 - buffered_)
                                                      : (kBlockSize - buffered_ + 56);
            update(ByteView{zeros.data(), std::min(need, zeros.size())});
        }

        std::array<std::uint8_t, 8> len_le{};
        for (std::size_t i = 0; i < 8; ++i) {
            len_le[i] = static_cast<std::uint8_t>(bit_length >> (8 * i));
        }
        update(ByteView{len_le.data(), len_le.size()});

        std::array<std::uint8_t, kDigestSize> digest{};
        for (std::size_t i = 0; i < 4; ++i) {
            for (std::size_t b = 0; b < 4; ++b) {
                digest[i * 4 + b] = static_cast<std::uint8_t>(state_[i] >> (8 * b));
            }
        }
        return digest;
    }

    [[nodiscard]] static std::array<std::uint8_t, kDigestSize> hash(ByteView data) {
        Md5 md5;
        md5.update(data);
        return md5.finish();
    }

private:
    void transform(const std::uint8_t* block) noexcept {
        std::array<std::uint32_t, 16> m{};
        for (std::size_t i = 0; i < 16; ++i) {
            m[i] = static_cast<std::uint32_t>(block[i * 4]) |
                   (static_cast<std::uint32_t>(block[i * 4 + 1]) << 8) |
                   (static_cast<std::uint32_t>(block[i * 4 + 2]) << 16) |
                   (static_cast<std::uint32_t>(block[i * 4 + 3]) << 24);
        }

        std::uint32_t a = state_[0], b = state_[1], c = state_[2], d = state_[3];
        for (std::size_t i = 0; i < 64; ++i) {
            std::uint32_t f = 0;
            std::size_t g = 0;
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
            const std::uint32_t sum = a + f + detail::kMd5K[i] + m[g];
            b = b + std::rotl(sum, detail::kMd5S[i]);
            a = tmp;
        }
        state_[0] += a;
        state_[1] += b;
        state_[2] += c;
        state_[3] += d;
    }

    std::array<std::uint32_t, 4> state_{0x67452301u, 0xefcdab89u, 0x98badcfeu,
                                        0x10325476u};
    std::array<std::uint8_t, kBlockSize> buffer_{};
    std::size_t buffered_ = 0;
    std::uint64_t length_ = 0;
};

/// RFC 2104 HMAC-MD5, truncated to `out_len` bytes (the ICV uses 10).
[[nodiscard]] inline Bytes hmac_md5(ByteView key, ByteView message,
                                    std::size_t out_len = Md5::kDigestSize) {
    if (out_len > Md5::kDigestSize) {
        throw ProtocolError{"HMAC-MD5 output longer than the 16-byte tag"};
    }

    std::array<std::uint8_t, Md5::kBlockSize> k0{};
    if (key.size() > Md5::kBlockSize) {
        const auto hashed = Md5::hash(key);
        std::memcpy(k0.data(), hashed.data(), hashed.size());
    } else if (!key.empty()) {
        std::memcpy(k0.data(), key.data(), key.size());
    }

    std::array<std::uint8_t, Md5::kBlockSize> ipad{};
    std::array<std::uint8_t, Md5::kBlockSize> opad{};
    for (std::size_t i = 0; i < Md5::kBlockSize; ++i) {
        ipad[i] = static_cast<std::uint8_t>(k0[i] ^ 0x36);
        opad[i] = static_cast<std::uint8_t>(k0[i] ^ 0x5C);
    }

    Md5 inner;
    inner.update(ByteView{ipad.data(), ipad.size()});
    inner.update(message);
    const auto inner_digest = inner.finish();

    Md5 outer;
    outer.update(ByteView{opad.data(), opad.size()});
    outer.update(ByteView{inner_digest.data(), inner_digest.size()});
    const auto tag = outer.finish();

    return Bytes{tag.begin(), tag.begin() + static_cast<std::ptrdiff_t>(out_len)};
}

}  // namespace knc::crypto
