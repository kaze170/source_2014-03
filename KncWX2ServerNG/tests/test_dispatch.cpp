// ---------------------------------------------------------------------------
// Tests for the event id -> packet struct binding.
//
// tools/crosscheck/check_event_packets.sh proves the table matches the legacy
// source: it static_asserts every bound id against the original headers and
// reads the symbol tables of the compiled legacy code for all ten regions.
// That is about FAITHFULNESS.
//
// These tests cover what that cannot see: that the table is internally
// consistent, that the C++ surface built on it behaves, and that the edge
// filter actually refuses what it claims to refuse.
// ---------------------------------------------------------------------------
#include "test_util.hpp"

#include <map>
#include <set>
#include <string>
#include <type_traits>
#include <vector>

#include "../src/proto/dispatch.cppm"

using namespace knc;
using namespace knc::proto;

namespace {

// Rebuild the table independently of the module's own accessors, straight
// from the .inc, so a bug in packet_binding() cannot hide itself.
struct Row {
    const char* name;
    unsigned value;
    PacketBinding binding;
    const char* type_name;
};

const Row kRows[] = {
#define KNC_EVENT_PACKET(name, value, type) {#name, (value), PacketBinding::kStruct, #type},
#define KNC_EVENT_SCALAR(name, value, type) {#name, (value), PacketBinding::kScalar, #type},
#define KNC_EVENT_NOSTRUCT(name, value)     {#name, (value), PacketBinding::kNoStruct, nullptr},
#define KNC_EVENT_OTHERREGION(name, value)  {#name, (value), PacketBinding::kOtherRegion, nullptr},
#include "../src/proto/event_packets.inc"
};

constexpr std::size_t kRowCount = sizeof(kRows) / sizeof(kRows[0]);

void test_table_shape(knctest::Suite& s) {
    s.run("the binding table covers every event id exactly once", [] {
        KNC_CHECK_EQ(kRowCount, kEventIdCount, "row count");
        for (std::size_t i = 0; i < kRowCount; ++i) {
            if (kRows[i].value != i) {
                KNC_CHECK(false, std::string{"row "} + std::to_string(i) +
                                     " has value " + std::to_string(kRows[i].value));
                return;
            }
        }
        KNC_CHECK(true, "row index == event id throughout");
    });

    s.run("binding row names match the event id table", [] {
        // Two generators, two files, one naming. If they ever drift, a
        // dispatcher would bind the right struct to the wrong id.
        for (std::size_t i = 0; i < kRowCount; ++i) {
            if (std::string{kRows[i].name} != event_name(static_cast<EventId>(i))) {
                KNC_CHECK(false, std::string{"row "} + std::to_string(i) + ": " +
                                     kRows[i].name + " vs " +
                                     event_name(static_cast<EventId>(i)));
                return;
            }
        }
        KNC_CHECK(true, "all 3386 names agree with event_ids.inc");
    });

    s.run("packet_binding() agrees with the raw table", [] {
        for (std::size_t i = 0; i < kRowCount; ++i) {
            if (packet_binding(static_cast<EventId>(i)) != kRows[i].binding) {
                KNC_CHECK(false, std::string{"row "} + std::to_string(i));
                return;
            }
        }
        KNC_CHECK(true, "accessor matches the table for all rows");
    });

    s.run("the four buckets add up", [] {
        std::size_t st = 0, sc = 0, no = 0, other = 0;
        for (const Row& r : kRows) {
            switch (r.binding) {
                case PacketBinding::kStruct:      ++st; break;
                case PacketBinding::kScalar:      ++sc; break;
                case PacketBinding::kNoStruct:    ++no; break;
                case PacketBinding::kOtherRegion: ++other; break;
            }
        }
        KNC_CHECK_EQ(st, std::size_t{1958}, "kStruct");
        KNC_CHECK_EQ(sc, std::size_t{41}, "kScalar");
        KNC_CHECK_EQ(no, std::size_t{1268}, "kNoStruct");
        KNC_CHECK_EQ(other, std::size_t{119}, "kOtherRegion");
        KNC_CHECK_EQ(st + sc + no + other, kEventIdCount, "total");
    });

    s.run("a type name is present exactly when one is decodable", [] {
        for (const Row& r : kRows) {
            const bool named = r.type_name != nullptr;
            const bool decodable = r.binding == PacketBinding::kStruct ||
                                   r.binding == PacketBinding::kScalar;
            if (named != decodable) {
                KNC_CHECK(false, std::string{r.name} + " names " +
                                     (named ? "a type" : "nothing") +
                                     " but is " + to_string(r.binding));
                return;
            }
        }
        KNC_CHECK(true, "type_name is set iff the payload is decodable");
    });

    s.run("out-of-range ids report kOtherRegion, not kStruct", [] {
        // Unknown must be handled like unparseable. The sentinel and
        // anything past it are not decodable.
        KNC_CHECK(packet_binding(static_cast<EventId>(kEventIdCount)) ==
                      PacketBinding::kOtherRegion,
                  "one past the end");
        KNC_CHECK(packet_binding(static_cast<EventId>(0xFFFFu)) ==
                      PacketBinding::kOtherRegion,
                  "far out of range");
        KNC_CHECK(packet_type_name(static_cast<EventId>(0xFFFFu)) == nullptr,
                  "no type name out of range");
    });
}

void test_anchors(knctest::Suite& s) {
    s.run("the login handshake binds to the structs it should", [] {
        KNC_CHECK(packet_binding(Ev::EGS_CONNECT_REQ) == PacketBinding::kStruct,
                  "EGS_CONNECT_REQ");
        KNC_CHECK(packet_binding(Ev::EGS_CONNECT_ACK) == PacketBinding::kStruct,
                  "EGS_CONNECT_ACK");
        KNC_CHECK_EQ(std::string{packet_type_name(27)},
                     std::string{"KEGS_CONNECT_ACK"}, "event 27 type name");
        static_assert(std::is_same_v<PacketOf<Ev::EGS_CONNECT_ACK>,
                                     packets::KEGS_CONNECT_ACK>);
        static_assert(std::is_same_v<PacketOf<Ev::EGS_CONNECT_REQ>,
                                     packets::KEGS_CONNECT_REQ>);
        KNC_CHECK(true, "compile-time binding resolves");
    });

    s.run("EGS_VERIFY_ACCOUNT_REQ resolves through its typedef", [] {
        // The legacy header says `typedef KUserAuthenticateReq
        // KEGS_VERIFY_ACCOUNT_REQ;`. A generator that only matched
        // `struct K<name>` would have missed this entirely.
        static_assert(std::is_same_v<PacketOf<Ev::EGS_VERIFY_ACCOUNT_REQ>,
                                     packets::KUserAuthenticateReq>);
        KNC_CHECK_EQ(std::string{packet_type_name(30)},
                     std::string{"KUserAuthenticateReq"},
                     "event 30 binds to the aliased struct");
    });

    s.run("E_ACCEPT_CONNECTION_NOT has no struct but is not empty", [] {
        // The handshake notification carries the SPI and the whole security
        // association, written by hand because it has to go out before any
        // SA exists. kNoStruct means "no generated struct", never "no
        // payload" - this is the id that proves the distinction matters.
        KNC_CHECK(packet_binding(Ev::E_ACCEPT_CONNECTION_NOT) ==
                      PacketBinding::kNoStruct,
                  "E_ACCEPT_CONNECTION_NOT is kNoStruct");
        KNC_CHECK(packet_type_name(1) == nullptr, "and names no type");
    });

    s.run("E_HEART_BEAT has no payload type", [] {
        KNC_CHECK(packet_binding(Ev::E_HEART_BEAT) == PacketBinding::kNoStruct,
                  "E_HEART_BEAT");
    });
}

void test_scalar_payloads(knctest::Suite& s) {
    s.run("a scalar-payload event decodes as a bare value", [] {
        // typedef __int64 KEGS_ACCEPT_FRIEND_REQ - the entire body is one
        // big-endian int64, with no struct and no length prefix.
        static_assert(std::is_same_v<PacketOf<Ev::EGS_ACCEPT_FRIEND_REQ>,
                                     std::int64_t>);
        KNC_CHECK(packet_binding(Ev::EGS_ACCEPT_FRIEND_REQ) ==
                      PacketBinding::kScalar,
                  "binding is kScalar");

        const auto bytes = encode_packet<std::int64_t>(0x0102030405060708LL);
        KNC_CHECK_EQ(bytes.size(), std::size_t{8}, "8 bytes, no framing");
        KNC_CHECK_EQ_BYTES(bytes, knctest::from_hex("0102030405060708"),
                           "big-endian, bare");
        KNC_CHECK_EQ(decode_packet<Ev::EGS_ACCEPT_FRIEND_REQ>(view(bytes)),
                     0x0102030405060708LL, "round trip");
    });

    s.run("a bool-payload event is a single byte", [] {
        static_assert(std::is_same_v<
                      PacketOf<Ev::EGS_CALL_MY_LOVER_AGREE_RESULT_NOT>, bool>);
        const auto bytes = encode_packet<bool>(true);
        KNC_CHECK_EQ_BYTES(bytes, knctest::from_hex("01"), "true is 01");
    });

    s.run("every scalar binding names a type the codec can handle", [] {
        // Structural: a kScalar row whose type had no codec would have
        // stopped generation, but assert the count we expect to survive.
        std::size_t n = 0;
        for (const Row& r : kRows) {
            if (r.binding == PacketBinding::kScalar) {
                ++n;
                KNC_CHECK(r.type_name != nullptr && r.type_name[0] != '\0',
                          "scalar row names a type");
            }
        }
        KNC_CHECK_EQ(n, std::size_t{41}, "scalar row count");
    });
}

void test_edge_filter(knctest::Suite& s) {
    s.run("the client band is still the client band", [] {
        KNC_CHECK(is_acceptable_from_client(kClientEventBegin),
                  "first client id");
        KNC_CHECK(!is_acceptable_from_client(kServerEventBegin),
                  "first server id is refused");
        KNC_CHECK(!is_acceptable_from_client(kEventSentinel),
                  "the sentinel is refused");
    });

    s.run("client-band ids from another region's build are refused", [] {
        // 38 ids sit in the client-originable band but belong to features
        // this build was not compiled with. is_client_originable() lets them
        // through; is_acceptable_from_client() must not.
        std::size_t band_excluded = 0, refused = 0;
        for (const Row& r : kRows) {
            if (r.binding != PacketBinding::kOtherRegion) continue;
            if (!is_client_originable(static_cast<EventId>(r.value))) continue;
            ++band_excluded;
            if (!is_acceptable_from_client(static_cast<EventId>(r.value))) {
                ++refused;
            }
        }
        KNC_CHECK_EQ(band_excluded, std::size_t{38},
                     "client-band ids belonging to another region");
        KNC_CHECK_EQ(refused, band_excluded, "all of them are refused");
    });

    s.run("the filter only ever narrows", [] {
        // It must never accept something is_client_originable() rejects.
        for (std::size_t i = 0; i < kRowCount; ++i) {
            const auto id = static_cast<EventId>(i);
            if (is_acceptable_from_client(id) && !is_client_originable(id)) {
                KNC_CHECK(false, std::string{"id "} + std::to_string(i) +
                                     " widened the filter");
                return;
            }
        }
        KNC_CHECK(true, "acceptable implies originable for all 3386 ids");
    });

    s.run("out-of-range ids are refused", [] {
        KNC_CHECK(!is_acceptable_from_client(static_cast<EventId>(0xFFFFu)),
                  "far out of range");
    });
}

void test_decode(knctest::Suite& s) {
    s.run("decode_packet round-trips a real packet", [] {
        packets::KEGS_CONNECT_ACK ack;
        ack.m_iOK = 1;
        ack.m_UDPPort = 15000;
        ack.m_iChannelID = 3;
        const auto bytes = encode_packet(ack);
        const auto back = decode_packet<Ev::EGS_CONNECT_ACK>(view(bytes));
        KNC_CHECK_EQ(back.m_iOK, 1, "m_iOK");
        KNC_CHECK_EQ(back.m_UDPPort, 15000, "m_UDPPort");
        KNC_CHECK_EQ(back.m_iChannelID, 3, "m_iChannelID");
    });

    s.run("trailing bytes are rejected, not ignored", [] {
        // The legacy reader stopped once it had enough and left the rest.
        // A sender that disagrees with us about the layout would then go
        // unnoticed, which is the exact failure this project exists to stop.
        packets::KEGS_CONNECT_ACK ack;
        auto bytes = encode_packet(ack);
        bytes.push_back(0xFF);
        KNC_CHECK_THROWS(decode_packet<Ev::EGS_CONNECT_ACK>(view(bytes)),
                         "one extra byte is an error");
    });

    s.run("a truncated payload is rejected", [] {
        packets::KEGS_CONNECT_ACK ack;
        auto bytes = encode_packet(ack);
        bytes.pop_back();
        KNC_CHECK_THROWS(decode_packet<Ev::EGS_CONNECT_ACK>(view(bytes)),
                         "one byte short is an error");
    });
}

}  // namespace

int main() {
    knctest::Suite suite{"Event id to packet binding"};
    test_table_shape(suite);
    test_anchors(suite);
    test_scalar_payloads(suite);
    test_edge_filter(suite);
    test_decode(suite);
    return suite.summary();
}
