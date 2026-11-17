// ---------------------------------------------------------------------------
// test_crypto.cpp - known-answer tests for the DES and MD5/HMAC primitives.
//
// These are the two primitives the 2014 client's packet security depends on.
// If either is off by a bit, every packet after the handshake is garbage, so
// they are pinned against published vectors rather than against our own output.
// ---------------------------------------------------------------------------
#include "test_util.hpp"

#include "../src/crypto/des.cppm"
#include "../src/crypto/md5.cppm"

using namespace knc;
using namespace knc::crypto;

namespace {

// --- DES ------------------------------------------------------------------
// FIPS / Crypto++ standard single-block vectors.

void test_des_known_blocks() {
    struct Vector {
        const char* name;
        const char* key;
        const char* plain;
        const char* cipher;
    };
    // Sources: FIPS PUB 46-3 appendix, Crypto++ des.txt / NIST SP 800-17.
    static const Vector kVectors[] = {
        {"all-zero key/plaintext", "0000000000000000", "0000000000000000",
         "8CA64DE9C1B123A7"},
        {"all-ones key/plaintext", "FFFFFFFFFFFFFFFF", "FFFFFFFFFFFFFFFF",
         "7359B2163E4EDC58"},
        {"classic 'Now is the '", "0123456789ABCDEF", "4E6F772069732074",
         "3FA40E8A984D4815"},
        {"NBS sample", "0E329232EA6D0D73", "8787878787878787",
         "0000000000000000"},
        {"K=3000..  P=1000..", "3000000000000000", "1000000000000001",
         "958E6E627A05557B"},
        {"weak-ish key 0101..", "0101010101010101", "95F8A5E5DD31D900",
         "8000000000000000"},
    };

    for (const auto& v : kVectors) {
        const Bytes key = knctest::from_hex(v.key);
        const Bytes plain = knctest::from_hex(v.plain);
        const Bytes expect = knctest::from_hex(v.cipher);

        const DesKey schedule{view(key)};
        const std::uint64_t ct = schedule.encrypt_block(load_be64(plain.data()));
        Bytes got(8);
        store_be64(got.data(), ct);
        KNC_CHECK_EQ_BYTES(got, expect, std::string{"DES encrypt: "} + v.name);

        const std::uint64_t back = schedule.decrypt_block(ct);
        Bytes round(8);
        store_be64(round.data(), back);
        KNC_CHECK_EQ_BYTES(round, plain, std::string{"DES decrypt: "} + v.name);
    }
}

void test_des_cbc_roundtrip_and_vector() {
    // NIST SP 800-38A style CBC check built from the single-block primitive:
    // C1 = E(P1 ^ IV), C2 = E(P2 ^ C1).  We verify our CBC against a manual
    // composition of the (already pinned) block cipher, which catches chaining
    // and byte-order mistakes.
    const Bytes key = knctest::from_hex("0123456789ABCDEF");
    const Bytes iv = knctest::from_hex("1234567890ABCDEF");
    const Bytes plain = knctest::from_hex("4E6F77206973207468652074696D6520666F72");
    // pad to a multiple of 8 the way the caller must
    Bytes padded = plain;
    padded.resize(24, 0x00);

    const DesKey schedule{view(key)};
    Bytes manual(padded.size());
    std::uint64_t chain = load_be64(iv.data());
    for (std::size_t off = 0; off < padded.size(); off += 8) {
        chain = schedule.encrypt_block(load_be64(padded.data() + off) ^ chain);
        store_be64(manual.data() + off, chain);
    }

    const Bytes got = des_cbc_encrypt(view(key), view(iv), view(padded));
    KNC_CHECK_EQ_BYTES(got, manual, "DES-CBC matches manual chaining");

    const Bytes back = des_cbc_decrypt(view(key), view(iv), view(got));
    KNC_CHECK_EQ_BYTES(back, padded, "DES-CBC round-trip");
}

void test_des_rejects_bad_sizes() {
    const Bytes key8 = knctest::from_hex("0123456789ABCDEF");
    const Bytes iv8 = knctest::from_hex("0000000000000000");
    const Bytes short_key = knctest::from_hex("0123456789AB");
    const Bytes ragged = knctest::from_hex("00112233445566778899");

    KNC_CHECK_THROWS(DesKey{view(short_key)}, "DES rejects a 6-byte key");
    KNC_CHECK_THROWS(des_cbc_encrypt(view(key8), view(short_key), view(ragged)),
                     "DES-CBC rejects a short IV");
    KNC_CHECK_THROWS(des_cbc_encrypt(view(key8), view(iv8), view(ragged)),
                     "DES-CBC rejects a non-multiple-of-8 input");
}

// --- MD5 / HMAC-MD5 --------------------------------------------------------

void test_md5_rfc1321() {
    struct Vector {
        const char* input;
        const char* digest;
    };
    // RFC 1321 appendix A.5 test suite.
    static const Vector kVectors[] = {
        {"", "d41d8cd98f00b204e9800998ecf8427e"},
        {"a", "0cc175b9c0f1b6a831c399e269772661"},
        {"abc", "900150983cd24fb0d6963f7d28e17f72"},
        {"message digest", "f96b697d7cb7938d525a2f31aaf161d0"},
        {"abcdefghijklmnopqrstuvwxyz", "c3fcd3d76192e4007dfb496cca67e13b"},
        {"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789",
         "d174ab98d277d9f5a5611c2c9f419d9f"},
        {"123456789012345678901234567890123456789012345678901234567890123456"
         "78901234567890",
         "57edf4a22be3c955ac49da2e2107b67a"},
    };
    for (const auto& v : kVectors) {
        const Bytes in = knctest::from_ascii(v.input);
        const auto digest = Md5::hash(view(in));
        const Bytes got{digest.begin(), digest.end()};
        KNC_CHECK_EQ_BYTES(got, knctest::from_hex(v.digest),
                           std::string{"MD5(\""} + v.input + "\")");
    }
}

void test_md5_streaming_matches_oneshot() {
    // Chunk boundaries around the 64-byte block and the 56-byte length split
    // are where buffering bugs live.
    Bytes data(200);
    for (std::size_t i = 0; i < data.size(); ++i) {
        data[i] = static_cast<std::uint8_t>(i * 7 + 3);
    }
    const auto oneshot = Md5::hash(view(data));

    for (std::size_t chunk : {1u, 3u, 55u, 56u, 57u, 63u, 64u, 65u, 127u}) {
        Md5 md5;
        for (std::size_t off = 0; off < data.size(); off += chunk) {
            const std::size_t n = std::min(chunk, data.size() - off);
            md5.update(ByteView{data.data() + off, n});
        }
        const auto got = md5.finish();
        KNC_CHECK(std::equal(got.begin(), got.end(), oneshot.begin()),
                  "MD5 streaming chunk=" + std::to_string(chunk));
    }
}

void test_hmac_md5_rfc2202() {
    struct Vector {
        const char* key_hex;
        const char* data_ascii;
        const char* data_hex;  // used when data_ascii is null
        const char* tag;
    };
    // RFC 2202 section 2 test cases for HMAC-MD5.
    static const Vector kVectors[] = {
        {"0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b", "Hi There", nullptr,
         "9294727a3638bb1c13f48ef8158bfc9d"},
        {"4a656665", "what do ya want for nothing?", nullptr,
         "750c783e6ab0b503eaa86e310a5db738"},
        // RFC 2202 case 3: key = 0xaa x 16, data = 0xdd x 50.
        {"aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", nullptr,
         "dddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd"
         "dddddddddddddddddddddddddddddddddd",
         "56be34521d144c88dbb8c733f0e8b3f6"},
        // RFC 2202 case 4: key = 0x0102..19, data = 0xcd x 50.
        {"0102030405060708090a0b0c0d0e0f10111213141516171819", nullptr,
         "cdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcd"
         "cdcdcdcdcdcdcdcdcdcdcdcdcdcdcdcd",
         "697eaf0aca3a3aea3a75164746ffaa79"},
        // 80-byte key, longer than the 64-byte block -> key gets hashed first.
        {"aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"
         "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"
         "aaaaaaaaaaaaaaaaaaaaaaaa",
         "Test Using Larger Than Block-Size Key - Hash Key First", nullptr,
         "6b1ab7fe4bd7bf8f0b62e6ce61b9d0cd"},
    };

    for (const auto& v : kVectors) {
        const Bytes key = knctest::from_hex(v.key_hex);
        const Bytes data = v.data_ascii ? knctest::from_ascii(v.data_ascii)
                                        : knctest::from_hex(v.data_hex);
        const Bytes got = hmac_md5(view(key), view(data));
        KNC_CHECK_EQ_BYTES(got, knctest::from_hex(v.tag),
                           std::string{"HMAC-MD5 "} +
                               (v.data_ascii ? v.data_ascii : "(hex data)"));
    }
}

void test_hmac_md5_truncation() {
    // The ICV is HMAC-MD5-80: the leading 10 bytes of the full tag.
    const Bytes key = knctest::from_hex("4a656665");
    const Bytes data = knctest::from_ascii("what do ya want for nothing?");
    const Bytes full = hmac_md5(view(key), view(data));
    const Bytes icv = hmac_md5(view(key), view(data), 10);

    KNC_CHECK_EQ(icv.size(), std::size_t{10}, "ICV is 10 bytes");
    KNC_CHECK(std::equal(icv.begin(), icv.end(), full.begin()),
              "ICV is a prefix of the full HMAC tag");
    KNC_CHECK_THROWS(hmac_md5(view(key), view(data), 17),
                     "HMAC rejects truncation longer than the digest");
}

}  // namespace

int main() {
    knctest::Suite suite{"crypto"};
    suite.run("DES known blocks", test_des_known_blocks);
    suite.run("DES-CBC", test_des_cbc_roundtrip_and_vector);
    suite.run("DES size validation", test_des_rejects_bad_sizes);
    suite.run("MD5 RFC 1321", test_md5_rfc1321);
    suite.run("MD5 streaming", test_md5_streaming_matches_oneshot);
    suite.run("HMAC-MD5 RFC 2202", test_hmac_md5_rfc2202);
    suite.run("HMAC-MD5 truncation", test_hmac_md5_truncation);
    return suite.summary();
}
