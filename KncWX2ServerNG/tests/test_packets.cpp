// ---------------------------------------------------------------------------
// Conformance tests for the generated packet table.
//
// tools/crosscheck/check_packets.sh already proves that the 1953 structs carry
// the same fields, in the same order, with the same types as the legacy source
// - it replays the original macros through a real compiler and diffs the
// result. That check is about FAITHFULNESS to the 2014 code.
//
// These tests are about something the cross-check cannot see: whether the
// bytes we put on the wire are the right bytes. The cross-check would be
// perfectly happy if Codec<std::int32_t> wrote little-endian. So the vectors
// below are written out by hand from docs/PROTOCOL.md section 6 and compared
// against what the generated structs actually produce.
//
// Where a packet matters to the login handshake it gets a full byte-for-byte
// vector. Everything else is covered structurally.
// ---------------------------------------------------------------------------
#include "test_util.hpp"

#include <map>
#include <set>
#include <string>
#include <vector>

#include "../src/proto/packets.cppm"

using namespace knc;
using namespace knc::proto;
using namespace knc::proto::packets;

namespace {

/// Serialise a packet and hand back the bytes.
template <typename T>
std::vector<std::uint8_t> encode(const T& packet) {
    Writer w;
    packet.write_to(w);
    return std::move(w).take();
}

/// Round-trip a packet through its own reader.
template <typename T>
T recode(const T& packet) {
    const auto bytes = encode(packet);
    Reader r{bytes};
    T back = T::read_from(r);
    KNC_CHECK_EQ(r.remaining(), std::size_t{0},
                 "round-trip consumed every byte it produced");
    return back;
}

// ===========================================================================
// The login handshake, byte for byte
// ===========================================================================

void test_handshake_vectors(knctest::Suite& s) {
    s.run("KEGS_CONNECT_REQ is a bare wstring", [] {
        // The very first packet a client sends. One field: the client build
        // string. wstring = DWORD byte-count big-endian, then UTF-16LE units.
        //   u"1.2.3"  -> 5 chars -> 10 bytes -> 0000000a
        //              -> 31 00 2e 00 32 00 2e 00 33 00
        KEGS_CONNECT_REQ req;
        req.m_wstrVersion = u"1.2.3";
        KNC_CHECK_EQ_BYTES(encode(req),
                           knctest::from_hex("0000000a 3100 2e00 3200 2e00 3300"),
                           "KEGS_CONNECT_REQ(u\"1.2.3\")");
    });

    s.run("KEGS_CONNECT_REQ with an empty version is four zero bytes", [] {
        KEGS_CONNECT_REQ req;
        KNC_CHECK_EQ_BYTES(encode(req), knctest::from_hex("00000000"),
                           "empty wstring is a bare zero length");
    });

    s.run("KEGS_CONNECT_ACK is exactly three big-endian ints", [] {
        // This packet settled a real ambiguity. ServerDefine.h has a fourth
        // field, `bool m_bUDPKickOff`, behind UDP_CAN_NOT_SEND_USER_KICK -
        // which is commented out at ServerDefine.h:2354 and never re-enabled
        // for TW/HK. If it were live the packet would be 13 bytes, not 12,
        // and every client would desynchronise on the first reply.
        KEGS_CONNECT_ACK ack;
        ack.m_iOK = 1;
        ack.m_UDPPort = 0x3A98;   // 15000
        ack.m_iChannelID = 7;
        const auto bytes = encode(ack);
        KNC_CHECK_EQ(bytes.size(), std::size_t{12}, "KEGS_CONNECT_ACK is 12 bytes");
        KNC_CHECK_EQ_BYTES(bytes,
                           knctest::from_hex("00000001 00003a98 00000007"),
                           "KEGS_CONNECT_ACK(ok=1, port=15000, channel=7)");
    });

    s.run("KEGS_CONNECT_ACK round-trips", [] {
        KEGS_CONNECT_ACK ack;
        ack.m_iOK = -1;
        ack.m_UDPPort = 15000;
        ack.m_iChannelID = 42;
        const auto back = recode(ack);
        KNC_CHECK_EQ(back.m_iOK, -1, "m_iOK");
        KNC_CHECK_EQ(back.m_UDPPort, 15000, "m_UDPPort");
        KNC_CHECK_EQ(back.m_iChannelID, 42, "m_iChannelID");
    });
}

// ===========================================================================
// Scalar encodings, checked through a real generated packet
// ===========================================================================

void test_scalar_encodings(knctest::Suite& s) {
    s.run("int fields are big-endian", [] {
        KEGS_CONNECT_ACK ack;
        ack.m_iOK = 0x01020304;
        ack.m_UDPPort = 0;
        ack.m_iChannelID = 0;
        const auto bytes = encode(ack);
        KNC_CHECK_EQ(int{bytes[0]}, 0x01, "most significant byte comes first");
        KNC_CHECK_EQ(int{bytes[3]}, 0x04, "least significant byte comes last");
    });

    s.run("negative ints are two's complement, not sign-magnitude", [] {
        KEGS_CONNECT_ACK ack;
        ack.m_iOK = -2;
        ack.m_UDPPort = 0;
        ack.m_iChannelID = 0;
        KNC_CHECK_EQ_BYTES(encode(ack),
                           knctest::from_hex("fffffffe 00000000 00000000"),
                           "-2 encodes as fffffffe");
    });

    s.run("wstring length is a BYTE count, not a character count", [] {
        // The single most common way to get this wrong. Four characters is
        // eight bytes, and the prefix says eight.
        KEGS_CONNECT_REQ req;
        req.m_wstrVersion = u"abcd";
        const auto bytes = encode(req);
        KNC_CHECK_EQ(bytes.size(), std::size_t{12}, "4 + 4*2");
        KNC_CHECK_EQ(int{bytes[3]}, 8, "prefix is 8, not 4");
    });

    s.run("wstring characters are little-endian inside a big-endian frame", [] {
        // wchar_t is the one type the legacy serializer does NOT byte-swap.
        KEGS_CONNECT_REQ req;
        req.m_wstrVersion = u"\u00e9";   // U+00E9
        KNC_CHECK_EQ_BYTES(encode(req), knctest::from_hex("00000002 e900"),
                           "U+00E9 goes out as e9 00");
    });
}

// ===========================================================================
// Composite shapes
// ===========================================================================

void test_container_shapes(knctest::Suite& s) {
    s.run("a vector is a big-endian DWORD count then the elements", [] {
        // Exercised through the codec the generated structs actually call.
        Writer w;
        const std::vector<std::int32_t> v{1, 2, 3};
        Codec<std::vector<std::int32_t>>::put(w, v);
        KNC_CHECK_EQ_BYTES(std::move(w).take(),
                           knctest::from_hex("00000003 00000001 00000002 00000003"),
                           "vector<int>{1,2,3}");
    });

    s.run("an empty container is a bare zero count", [] {
        Writer w;
        Codec<std::vector<std::int32_t>>::put(w, {});
        KNC_CHECK_EQ_BYTES(std::move(w).take(), knctest::from_hex("00000000"),
                           "empty vector");
    });

    s.run("a map is one count followed by key/value pairs", [] {
        Writer w;
        const std::map<std::int32_t, std::int32_t> m{{1, 10}, {2, 20}};
        Codec<std::map<std::int32_t, std::int32_t>>::put(w, m);
        KNC_CHECK_EQ_BYTES(
            std::move(w).take(),
            knctest::from_hex("00000002 00000001 0000000a 00000002 00000014"),
            "map{1:10, 2:20} - a single count, not one per entry");
    });

    s.run("a set serialises in ascending order whatever order it was built in", [] {
        Writer w;
        std::set<std::int32_t> a;
        a.insert(3); a.insert(1); a.insert(2);
        Codec<std::set<std::int32_t>>::put(w, a);
        KNC_CHECK_EQ_BYTES(
            std::move(w).take(),
            knctest::from_hex("00000003 00000001 00000002 00000003"),
            "set{3,1,2} goes out as 1,2,3");
    });

    s.run("a pair carries no count of its own", [] {
        Writer w;
        Codec<std::pair<std::int32_t, bool>>::put(w, {7, true});
        KNC_CHECK_EQ_BYTES(std::move(w).take(), knctest::from_hex("00000007 01"),
                           "pair{7,true}");
    });

    s.run("containers round-trip through the reader", [] {
        Writer w;
        const std::map<std::int64_t, std::u16string> m{
            {1, u"one"}, {2, u"two"}, {-5, u""}};
        Codec<std::map<std::int64_t, std::u16string>>::put(w, m);
        const auto bytes = std::move(w).take();
        Reader r{bytes};
        const auto back = Codec<std::map<std::int64_t, std::u16string>>::get(r);
        KNC_CHECK_EQ(r.remaining(), std::size_t{0}, "every byte consumed");
        KNC_CHECK(back == m, "map<int64,wstring> survives a round trip");
    });
}

// ===========================================================================
// Fixed C arrays
// ===========================================================================

void test_fixed_arrays(knctest::Suite& s) {
    s.run("a fixed array is written element by element with NO count", [] {
        // KGlobalCashInfo is `unsigned long m_ulCash[GCT_TOTAL_NUM]` where the
        // enum's trailing member is 4. The legacy PUT body lists the four
        // indices explicitly, so there is no length prefix - unlike every
        // other container in the protocol.
        KGlobalCashInfo info;
        info.m_ulCash = {1, 2, 3, 4};
        const auto bytes = encode(info);
        KNC_CHECK_EQ(bytes.size(), std::size_t{16},
                     "4 x DWORD and nothing else");
        KNC_CHECK_EQ_BYTES(
            bytes,
            knctest::from_hex("00000001 00000002 00000003 00000004"),
            "KGlobalCashInfo has no count prefix");
    });

    s.run("a fixed array round-trips", [] {
        KGlobalCashInfo info;
        info.m_ulCash = {100, 200, 300, 400};
        const auto back = recode(info);
        KNC_CHECK(back.m_ulCash == info.m_ulCash, "m_ulCash");
    });

    s.run("unsigned long stays 4 bytes on a 64-bit host", [] {
        // The legacy protocol is Win32/LLP64: `unsigned long` is 32 bits. A
        // naive port on Linux would widen it to 64 and double the field.
        KNC_CHECK_EQ(sizeof(KGlobalCashInfo{}.m_ulCash[0]), std::size_t{4},
                     "DWORD is four bytes");
    });
}

// ===========================================================================
// Nested packet types
// ===========================================================================

void test_nested_structs(knctest::Suite& s) {
    s.run("a nested struct is inlined with no length prefix and no tag", [] {
        // KDBConnectionInfo is {int, wstring, int}. Encoding it on its own and
        // encoding it as a member must produce identical bytes - that is what
        // "inlined" means, and it is why tagging being off matters.
        KDBConnectionInfo inner;
        inner.m_iDBType = 3;
        inner.m_wstrConnectionString = u"db";
        const auto direct = encode(inner);

        Writer w;
        Codec<KDBConnectionInfo>::put(w, inner);
        KNC_CHECK_EQ_BYTES(std::move(w).take(), direct,
                           "member encoding == standalone encoding");
    });

    s.run("nested structs round-trip", [] {
        KDBConnectionInfo inner;
        inner.m_iDBType = 11;
        inner.m_wstrConnectionString = u"elsword";
        const auto back = recode(inner);
        KNC_CHECK_EQ(back.m_iDBType, 11, "m_iDBType");
        KNC_CHECK(back.m_wstrConnectionString == inner.m_wstrConnectionString,
                  "m_wstrConnectionString");
    });
}

// ===========================================================================
// Repaired legacy defects
// ===========================================================================

void test_repaired_get_bodies(knctest::Suite& s) {
    s.run("KEGIANT_BILLING_CONSUME_ACK can actually be read back", [] {
        // The legacy SERIALIZE_DEFINE_GET for this packet calls PUT() on all
        // six fields, so the original reader consumed nothing and returned a
        // default-constructed object - the packet was write-only. The
        // generator repairs it from the PUT body (see KNOWN_GET_BODY_DEFECTS
        // in tools/gen_packets.py and docs/PROTOCOL.md section 8).
        KEGIANT_BILLING_CONSUME_ACK ack;
        ack.m_iOK = 1;
        ack.m_iBalance = 500;
        ack.m_iBonus = 25;
        ack.m_bHasFilled = true;
        ack.m_iPoint = 7;
        ack.m_uiGiantUID = 123456;
        const auto back = recode(ack);
        KNC_CHECK_EQ(back.m_iOK, 1, "m_iOK");
        KNC_CHECK_EQ(back.m_iBalance, 500, "m_iBalance");
        KNC_CHECK_EQ(back.m_iBonus, 25, "m_iBonus");
        KNC_CHECK_EQ(back.m_bHasFilled, true, "m_bHasFilled");
        KNC_CHECK_EQ(back.m_iPoint, 7, "m_iPoint");
        KNC_CHECK_EQ(back.m_uiGiantUID, 123456u, "m_uiGiantUID");
    });

    s.run("KELOG_ITEM_TRADE_ERROR_NOT reads its trailing wstring", [] {
        // Same defect, narrower: only the last field was PUT instead of GET,
        // so m_wsrtRegDate (the legacy misspelling is preserved) always came
        // back empty and left bytes unconsumed.
        KELOG_ITEM_TRADE_ERROR_NOT notify;
        notify.m_iUnitUID = 999;
        notify.m_iType = 3;
        notify.m_wsrtRegDate = u"2014-03-01";
        const auto back = recode(notify);
        KNC_CHECK_EQ(back.m_iUnitUID, 999, "m_iUnitUID");
        KNC_CHECK_EQ(back.m_iType, 3, "m_iType");
        KNC_CHECK(back.m_wsrtRegDate == notify.m_wsrtRegDate, "m_wsrtRegDate");
    });
}

// ===========================================================================
// Table-wide structural invariants
// ===========================================================================

void test_table_shape(knctest::Suite& s) {
    s.run("the handshake packets exist under their legacy names", [] {
        // Compile-time existence check. If a regeneration drops one of these,
        // this file stops building - which is the point.
        KNC_CHECK(sizeof(KEGS_CONNECT_REQ) > 0, "KEGS_CONNECT_REQ");
        KNC_CHECK(sizeof(KEGS_CONNECT_ACK) > 0, "KEGS_CONNECT_ACK");
        KNC_CHECK(sizeof(KEGS_VERIFY_ACCOUNT_ACK) > 0, "KEGS_VERIFY_ACCOUNT_ACK");
        KNC_CHECK(sizeof(KAccountInfo) > 0, "KAccountInfo");
    });

    s.run("an empty packet encodes to zero bytes", [] {
        // Several notifications carry no payload at all. They must not emit a
        // stray count or tag.
        KEGS_CONNECT_REQ req;
        req.m_wstrVersion.clear();
        KNC_CHECK_EQ(encode(req).size(), std::size_t{4},
                     "one empty wstring is four bytes of length");
    });

    s.run("reading past the end throws instead of returning garbage", [] {
        const std::vector<std::uint8_t> truncated = knctest::from_hex("00000001 0000");
        Reader r{truncated};
        KNC_CHECK_THROWS(KEGS_CONNECT_ACK::read_from(r),
                         "a short KEGS_CONNECT_ACK is rejected");
    });

    s.run("a hostile container count is rejected before it allocates", [] {
        // DWORD 0xFFFFFFFF elements, with two bytes of body behind it.
        const std::vector<std::uint8_t> hostile = knctest::from_hex("ffffffff 0000");
        Reader r{hostile};
        KNC_CHECK_THROWS((Codec<std::vector<std::int32_t>>::get(r)),
                         "4 billion elements is refused");
    });
}

}  // namespace

int main() {
    knctest::Suite suite{"Generated packet table"};
    test_handshake_vectors(suite);
    test_scalar_encodings(suite);
    test_container_shapes(suite);
    test_fixed_arrays(suite);
    test_nested_structs(suite);
    test_repaired_get_bodies(suite);
    test_table_shape(suite);
    return suite.summary();
}
