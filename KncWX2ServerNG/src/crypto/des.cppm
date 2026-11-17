// ---------------------------------------------------------------------------
// knc.crypto.des - single DES (FIPS 46-3) in CBC mode.
//
// Replaces the Crypto++ dependency of the original
//     CryptoPP::CBC_Mode<CryptoPP::DES>::Encryption / ::Decryption
// used by KSecurityAssociation::Encryption / ::Decryption
// (KNCSDK/Include/KncSecurity/SecurityAssociation.cpp).
//
// Legacy behaviour reproduced exactly:
//   * 8-byte key, 8-byte IV, 8-byte block (KSecurityAssociation::GetBlockSize())
//   * NO cipher-level padding.  The caller (KSecureBuffer) has already padded
//     the plaintext to a multiple of 8, RFC 2406 style.  Crypto++'s
//     ProcessData() is a raw multiple-of-blocksize transform, so we must not
//     add or strip anything here.
//   * Key parity bits are ignored, exactly like Crypto++ (the low bit of each
//     key byte is discarded by PC-1).
//
// DES is cryptographically broken.  It is implemented here solely because the
// 2014 client speaks it and the brief is wire compatibility.  See
// docs/PROTOCOL.md section "Security posture".
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
export module knc.crypto.des;
import knc.core.types;
#endif

namespace knc::crypto::detail {

// --- FIPS 46-3 tables (1-based bit positions, MSB-first) -------------------

inline constexpr std::uint8_t kPC1[56] = {
    57, 49, 41, 33, 25, 17,  9,  1, 58, 50, 42, 34, 26, 18,
    10,  2, 59, 51, 43, 35, 27, 19, 11,  3, 60, 52, 44, 36,
    63, 55, 47, 39, 31, 23, 15,  7, 62, 54, 46, 38, 30, 22,
    14,  6, 61, 53, 45, 37, 29, 21, 13,  5, 28, 20, 12,  4};

inline constexpr std::uint8_t kPC2[48] = {
    14, 17, 11, 24,  1,  5,  3, 28, 15,  6, 21, 10,
    23, 19, 12,  4, 26,  8, 16,  7, 27, 20, 13,  2,
    41, 52, 31, 37, 47, 55, 30, 40, 51, 45, 33, 48,
    44, 49, 39, 56, 34, 53, 46, 42, 50, 36, 29, 32};

inline constexpr std::uint8_t kShifts[16] = {1, 1, 2, 2, 2, 2, 2, 2,
                                             1, 2, 2, 2, 2, 2, 2, 1};

inline constexpr std::uint8_t kIP[64] = {
    58, 50, 42, 34, 26, 18, 10, 2, 60, 52, 44, 36, 28, 20, 12, 4,
    62, 54, 46, 38, 30, 22, 14, 6, 64, 56, 48, 40, 32, 24, 16, 8,
    57, 49, 41, 33, 25, 17,  9, 1, 59, 51, 43, 35, 27, 19, 11, 3,
    61, 53, 45, 37, 29, 21, 13, 5, 63, 55, 47, 39, 31, 23, 15, 7};

inline constexpr std::uint8_t kFP[64] = {
    40, 8, 48, 16, 56, 24, 64, 32, 39, 7, 47, 15, 55, 23, 63, 31,
    38, 6, 46, 14, 54, 22, 62, 30, 37, 5, 45, 13, 53, 21, 61, 29,
    36, 4, 44, 12, 52, 20, 60, 28, 35, 3, 43, 11, 51, 19, 59, 27,
    34, 2, 42, 10, 50, 18, 58, 26, 33, 1, 41,  9, 49, 17, 57, 25};

inline constexpr std::uint8_t kE[48] = {
    32,  1,  2,  3,  4,  5,  4,  5,  6,  7,  8,  9,
     8,  9, 10, 11, 12, 13, 12, 13, 14, 15, 16, 17,
    16, 17, 18, 19, 20, 21, 20, 21, 22, 23, 24, 25,
    24, 25, 26, 27, 28, 29, 28, 29, 30, 31, 32,  1};

inline constexpr std::uint8_t kP[32] = {
    16,  7, 20, 21, 29, 12, 28, 17,  1, 15, 23, 26,  5, 18, 31, 10,
     2,  8, 24, 14, 32, 27,  3,  9, 19, 13, 30,  6, 22, 11,  4, 25};

inline constexpr std::uint8_t kS[8][64] = {
    {14, 4, 13, 1, 2, 15, 11, 8, 3, 10, 6, 12, 5, 9, 0, 7,
     0, 15, 7, 4, 14, 2, 13, 1, 10, 6, 12, 11, 9, 5, 3, 8,
     4, 1, 14, 8, 13, 6, 2, 11, 15, 12, 9, 7, 3, 10, 5, 0,
     15, 12, 8, 2, 4, 9, 1, 7, 5, 11, 3, 14, 10, 0, 6, 13},
    {15, 1, 8, 14, 6, 11, 3, 4, 9, 7, 2, 13, 12, 0, 5, 10,
     3, 13, 4, 7, 15, 2, 8, 14, 12, 0, 1, 10, 6, 9, 11, 5,
     0, 14, 7, 11, 10, 4, 13, 1, 5, 8, 12, 6, 9, 3, 2, 15,
     13, 8, 10, 1, 3, 15, 4, 2, 11, 6, 7, 12, 0, 5, 14, 9},
    {10, 0, 9, 14, 6, 3, 15, 5, 1, 13, 12, 7, 11, 4, 2, 8,
     13, 7, 0, 9, 3, 4, 6, 10, 2, 8, 5, 14, 12, 11, 15, 1,
     13, 6, 4, 9, 8, 15, 3, 0, 11, 1, 2, 12, 5, 10, 14, 7,
     1, 10, 13, 0, 6, 9, 8, 7, 4, 15, 14, 3, 11, 5, 2, 12},
    {7, 13, 14, 3, 0, 6, 9, 10, 1, 2, 8, 5, 11, 12, 4, 15,
     13, 8, 11, 5, 6, 15, 0, 3, 4, 7, 2, 12, 1, 10, 14, 9,
     10, 6, 9, 0, 12, 11, 7, 13, 15, 1, 3, 14, 5, 2, 8, 4,
     3, 15, 0, 6, 10, 1, 13, 8, 9, 4, 5, 11, 12, 7, 2, 14},
    {2, 12, 4, 1, 7, 10, 11, 6, 8, 5, 3, 15, 13, 0, 14, 9,
     14, 11, 2, 12, 4, 7, 13, 1, 5, 0, 15, 10, 3, 9, 8, 6,
     4, 2, 1, 11, 10, 13, 7, 8, 15, 9, 12, 5, 6, 3, 0, 14,
     11, 8, 12, 7, 1, 14, 2, 13, 6, 15, 0, 9, 10, 4, 5, 3},
    {12, 1, 10, 15, 9, 2, 6, 8, 0, 13, 3, 4, 14, 7, 5, 11,
     10, 15, 4, 2, 7, 12, 9, 5, 6, 1, 13, 14, 0, 11, 3, 8,
     9, 14, 15, 5, 2, 8, 12, 3, 7, 0, 4, 10, 1, 13, 11, 6,
     4, 3, 2, 12, 9, 5, 15, 10, 11, 14, 1, 7, 6, 0, 8, 13},
    {4, 11, 2, 14, 15, 0, 8, 13, 3, 12, 9, 7, 5, 10, 6, 1,
     13, 0, 11, 7, 4, 9, 1, 10, 14, 3, 5, 12, 2, 15, 8, 6,
     1, 4, 11, 13, 12, 3, 7, 14, 10, 15, 6, 8, 0, 5, 9, 2,
     6, 11, 13, 8, 1, 4, 10, 7, 9, 5, 0, 15, 14, 2, 3, 12},
    {13, 2, 8, 4, 6, 15, 11, 1, 10, 9, 3, 14, 5, 0, 12, 7,
     1, 15, 13, 8, 10, 3, 7, 4, 12, 5, 6, 11, 0, 14, 9, 2,
     7, 11, 4, 1, 9, 12, 14, 2, 0, 6, 10, 13, 15, 3, 5, 8,
     2, 1, 14, 7, 4, 10, 8, 13, 15, 12, 9, 0, 3, 5, 6, 11}};

/// Read bit `pos` (1-based, MSB-first) out of a big-endian bit string.
[[nodiscard]] constexpr std::uint64_t bit_at(std::uint64_t value, unsigned width,
                                             unsigned pos) noexcept {
    return (value >> (width - pos)) & 1ULL;
}

/// Apply a 1-based MSB-first permutation table.
template <std::size_t N>
[[nodiscard]] constexpr std::uint64_t permute(std::uint64_t value, unsigned in_width,
                                              const std::uint8_t (&table)[N]) noexcept {
    std::uint64_t out = 0;
    for (std::size_t i = 0; i < N; ++i) {
        out = (out << 1) | bit_at(value, in_width, table[i]);
    }
    return out;
}

}  // namespace knc::crypto::detail

KNC_EXPORT namespace knc::crypto {

/// Expanded DES key schedule: 16 x 48-bit round subkeys.
class DesKey {
public:
    static constexpr std::size_t kKeySize = 8;
    static constexpr std::size_t kBlockSize = 8;

    /// `key` must be exactly 8 bytes.  Parity bits are ignored (PC-1 drops them),
    /// matching Crypto++ and therefore the 2014 client.
    explicit DesKey(ByteView key) {
        if (key.size() != kKeySize) {
            throw ProtocolError{"DES key must be exactly 8 bytes"};
        }
        std::uint64_t k = 0;
        for (std::size_t i = 0; i < kKeySize; ++i) {
            k = (k << 8) | key[i];
        }

        const std::uint64_t permuted = detail::permute(k, 64, detail::kPC1);  // 56 bits
        std::uint32_t c = static_cast<std::uint32_t>((permuted >> 28) & 0x0FFFFFFFU);
        std::uint32_t d = static_cast<std::uint32_t>(permuted & 0x0FFFFFFFU);

        for (std::size_t round = 0; round < 16; ++round) {
            const unsigned s = detail::kShifts[round];
            c = ((c << s) | (c >> (28 - s))) & 0x0FFFFFFFU;
            d = ((d << s) | (d >> (28 - s))) & 0x0FFFFFFFU;
            const std::uint64_t cd = (static_cast<std::uint64_t>(c) << 28) | d;
            subkeys_[round] = detail::permute(cd, 56, detail::kPC2);  // 48 bits
        }
    }

    [[nodiscard]] std::uint64_t encrypt_block(std::uint64_t block) const noexcept {
        return run(block, /*forward=*/true);
    }
    [[nodiscard]] std::uint64_t decrypt_block(std::uint64_t block) const noexcept {
        return run(block, /*forward=*/false);
    }

private:
    [[nodiscard]] static std::uint32_t feistel(std::uint32_t r,
                                               std::uint64_t subkey) noexcept {
        const std::uint64_t expanded = detail::permute(r, 32, detail::kE) ^ subkey;
        std::uint32_t sout = 0;
        for (unsigned box = 0; box < 8; ++box) {
            const unsigned six =
                static_cast<unsigned>((expanded >> (42 - 6 * box)) & 0x3FU);
            // Row = outer bits (b1 b6), column = inner four bits (b2..b5).
            const unsigned row = ((six & 0x20U) >> 4) | (six & 0x01U);
            const unsigned col = (six >> 1) & 0x0FU;
            sout = (sout << 4) | detail::kS[box][row * 16 + col];
        }
        return static_cast<std::uint32_t>(detail::permute(sout, 32, detail::kP));
    }

    [[nodiscard]] std::uint64_t run(std::uint64_t block, bool forward) const noexcept {
        const std::uint64_t ip = detail::permute(block, 64, detail::kIP);
        std::uint32_t l = static_cast<std::uint32_t>(ip >> 32);
        std::uint32_t r = static_cast<std::uint32_t>(ip & 0xFFFFFFFFU);

        for (unsigned i = 0; i < 16; ++i) {
            const std::uint64_t sk = subkeys_[forward ? i : 15 - i];
            const std::uint32_t next = l ^ feistel(r, sk);
            l = r;
            r = next;
        }
        // Final swap: the pre-output is R16 L16.
        const std::uint64_t preoutput =
            (static_cast<std::uint64_t>(r) << 32) | static_cast<std::uint64_t>(l);
        return detail::permute(preoutput, 64, detail::kFP);
    }

    std::array<std::uint64_t, 16> subkeys_{};
};

[[nodiscard]] inline std::uint64_t load_be64(const std::uint8_t* p) noexcept {
    std::uint64_t v = 0;
    for (int i = 0; i < 8; ++i) {
        v = (v << 8) | p[static_cast<std::size_t>(i)];
    }
    return v;
}

inline void store_be64(std::uint8_t* p, std::uint64_t v) noexcept {
    for (int i = 7; i >= 0; --i) {
        p[static_cast<std::size_t>(7 - i)] = static_cast<std::uint8_t>(v >> (8 * i));
    }
}

/// DES-CBC encryption with no padding.  `plain.size()` must be a multiple of 8.
/// Equivalent to CryptoPP::CBC_Mode<DES>::Encryption::ProcessData().
[[nodiscard]] inline Bytes des_cbc_encrypt(ByteView key, ByteView iv, ByteView plain) {
    if (iv.size() != DesKey::kBlockSize) {
        throw ProtocolError{"DES-CBC IV must be exactly 8 bytes"};
    }
    if (plain.size() % DesKey::kBlockSize != 0) {
        throw ProtocolError{"DES-CBC input must be a multiple of the 8-byte block"};
    }
    const DesKey schedule{key};
    std::uint64_t chain = load_be64(iv.data());

    Bytes out(plain.size());
    for (std::size_t off = 0; off < plain.size(); off += DesKey::kBlockSize) {
        const std::uint64_t block = load_be64(plain.data() + off) ^ chain;
        chain = schedule.encrypt_block(block);
        store_be64(out.data() + off, chain);
    }
    return out;
}

/// DES-CBC decryption with no unpadding.  Mirrors ::Decryption::ProcessData().
[[nodiscard]] inline Bytes des_cbc_decrypt(ByteView key, ByteView iv, ByteView cipher) {
    if (iv.size() != DesKey::kBlockSize) {
        throw ProtocolError{"DES-CBC IV must be exactly 8 bytes"};
    }
    if (cipher.size() % DesKey::kBlockSize != 0) {
        throw ProtocolError{"DES-CBC input must be a multiple of the 8-byte block"};
    }
    const DesKey schedule{key};
    std::uint64_t chain = load_be64(iv.data());

    Bytes out(cipher.size());
    for (std::size_t off = 0; off < cipher.size(); off += DesKey::kBlockSize) {
        const std::uint64_t ct = load_be64(cipher.data() + off);
        store_be64(out.data() + off, schedule.decrypt_block(ct) ^ chain);
        chain = ct;
    }
    return out;
}

}  // namespace knc::crypto
