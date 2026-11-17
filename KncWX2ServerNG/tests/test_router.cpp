// ---------------------------------------------------------------------------
// Tests for the handler table and for Connection - the socket-free half of
// the server.
//
// Connection is deliberately separable from the poll() loop: you hand it
// bytes that arrived and take bytes to send. That makes the interesting
// behaviour - what closes a connection, what merely counts against it -
// testable without a network, and it is what check_live.sh then exercises
// for real.
// ---------------------------------------------------------------------------
#include "test_util.hpp"

#include <string>
#include <vector>

#include "../src/net/server.cppm"

using namespace knc;
using namespace knc::proto;
using namespace knc::net;
using knc::app::Routed;
using knc::app::Router;
using knctest::from_hex;

namespace {

/// A client that talks to a Connection directly, with no socket in between.
struct Peer {
    Session session{Role::kClient};

    explicit Peer(Connection& server) {
        session.begin(0);
        pump(server);
        // One frame is waiting: E_ACCEPT_CONNECTION_NOT. Pushing bytes is
        // not the same as processing them - the session only adopts the key
        // when next() runs.
        const Inbound hello = session.next(1);
        KNC_CHECK(hello.status == Ingress::kHandled,
                  "peer setup: the key exchange was consumed");
    }

    /// Move everything the server has queued into the client.
    void pump(Connection& server) {
        while (server.has_pending_output()) {
            const ByteView out = server.pending_output();
            const Bytes copy{out.begin(), out.end()};
            server.consume_output(copy.size());
            session.push(view(copy));
        }
    }

    /// Read the next event the server sent, if any.
    [[nodiscard]] Inbound next() { return session.next(1); }
};

void deliver(Connection& c, const Bytes& bytes, std::uint64_t now = 1) {
    c.on_bytes(view(bytes), now);
}

// --- router ----------------------------------------------------------------

void test_typed_handler_gets_the_bound_packet() {
    Router r;
    std::u16string seen;
    r.on<Ev::EGS_CONNECT_REQ>([&](Session&, const packets::KEGS_CONNECT_REQ& p) {
        seen = p.m_wstrVersion;
    });
    KNC_CHECK_EQ(r.size(), std::size_t{1}, "one handler registered");
    KNC_CHECK(r.has(raw(Ev::EGS_CONNECT_REQ)), "...for event 26");

    Session s{Role::kServer};
    Event ev{Ev::EGS_CONNECT_REQ};
    ev.payload().assign(encode_packet(packets::KEGS_CONNECT_REQ{u"7.7.7"}));
    const auto result = r.route(s, ev);
    KNC_CHECK(result.status == Routed::kHandled, "the event is handled");
    KNC_CHECK(seen == u"7.7.7", "the handler received the decoded packet");
}

void test_scalar_handler() {
    // The 41 scalar ids go through the same registration as a struct.
    Router r;
    std::int64_t got = 0;
    r.on<Ev::EGS_ACCEPT_FRIEND_REQ>([&](Session&, std::int64_t uid) { got = uid; });

    Session s{Role::kServer};
    Event ev{Ev::EGS_ACCEPT_FRIEND_REQ};
    ev.payload().assign(encode_packet<std::int64_t>(-1));
    KNC_CHECK(r.route(s, ev).status == Routed::kHandled, "a scalar event routes");
    KNC_CHECK_EQ(got, std::int64_t{-1}, "the handler received the bare value");
}

void test_unhandled_is_not_an_error() {
    Router r;
    Session s{Role::kServer};
    const auto result = r.route(s, Event{Ev::EGS_CONNECT_REQ});
    KNC_CHECK(result.status == Routed::kUnhandled, "no handler is reported, not thrown");
    KNC_CHECK(!result, "and the result is falsy");
}

void test_duplicate_registration_is_refused() {
    // The legacy switch made this impossible by construction; a map does not.
    Router r;
    r.on<Ev::EGS_CONNECT_REQ>([](Session&, const packets::KEGS_CONNECT_REQ&) {});
    KNC_CHECK_THROWS(
        r.on<Ev::EGS_CONNECT_REQ>([](Session&, const packets::KEGS_CONNECT_REQ&) {}),
        "registering the same id twice throws");
    KNC_CHECK_EQ(r.size(), std::size_t{1}, "the first registration survives");
}

void test_other_region_registration_is_refused() {
    EventId other = 0;
    for (EventId id = 0; id <= 1324; ++id) {
        if (packet_binding(id) == PacketBinding::kOtherRegion) {
            other = id;
            break;
        }
    }
    KNC_CHECK(other != 0, "there is an other-region id to try");

    Router r;
    KNC_CHECK_THROWS(r.on_raw(other, [](Session&, const Event&) {}),
                     "a handler for another region's event is refused");
}

void test_decode_failure_is_reported_not_thrown() {
    Router r;
    bool ran = false;
    r.on<Ev::EGS_CONNECT_REQ>(
        [&](Session&, const packets::KEGS_CONNECT_REQ&) { ran = true; });

    Session s{Role::kServer};
    Event ev{Ev::EGS_CONNECT_REQ};
    ev.payload().assign(from_hex("ffffffff"));  // a 4GB string length
    const auto result = r.route(s, ev);
    KNC_CHECK(result.status == Routed::kDecodeFailed, "a bad payload is reported");
    KNC_CHECK(!result.detail.empty(), "...with a reason");
    KNC_CHECK(!ran, "and the handler never ran");
}

void test_throwing_handler_is_contained() {
    Router r;
    r.on_raw(Ev::EGS_GOOD_JOB_1_REQ,
             [](Session&, const Event&) { throw std::runtime_error("boom"); });
    Session s{Role::kServer};
    const auto result = r.route(s, Event{Ev::EGS_GOOD_JOB_1_REQ});
    KNC_CHECK(result.status == Routed::kHandlerFailed, "a throwing handler is caught");
    KNC_CHECK_EQ(result.detail, std::string{"boom"}, "...and reported");
}

void test_handler_can_reply() {
    Router r;
    r.on<Ev::EGS_CONNECT_REQ>([](Session& s, const packets::KEGS_CONNECT_REQ&) {
        packets::KEGS_CONNECT_ACK ack{};
        ack.m_iOK = 0;
        ack.m_iChannelID = 42;
        s.post_packet<Ev::EGS_CONNECT_ACK>(ack);
    });

    Connection conn{1, r};
    conn.start(0);
    Peer peer{conn};

    deliver(conn, peer.session.send_packet<Ev::EGS_CONNECT_REQ>(
                      packets::KEGS_CONNECT_REQ{u"1.0"}));
    peer.pump(conn);
    const Inbound in = peer.next();
    KNC_CHECK(in.status == Ingress::kEvent, "the reply arrives");
    KNC_CHECK_EQ(static_cast<unsigned>(in.event.id()),
                 static_cast<unsigned>(raw(Ev::EGS_CONNECT_ACK)), "...as the ack");
    KNC_CHECK_EQ(Session::packet_of<Ev::EGS_CONNECT_ACK>(in.event).m_iChannelID, 42,
                 "...with the handler's value");
}

// --- connection ------------------------------------------------------------

void test_connection_hands_out_a_key_on_start() {
    Router r;
    Connection conn{7, r};
    KNC_CHECK(!conn.has_pending_output(), "nothing queued before start");
    conn.start(0);
    KNC_CHECK(conn.has_pending_output(), "start queues the key exchange");
    KNC_CHECK(conn.session().keyed(), "and the session is keyed");

    Peer peer{conn};
    KNC_CHECK(peer.session.keyed(), "the peer adopted the key");
    KNC_CHECK_EQ(static_cast<unsigned>(peer.session.spi()),
                 static_cast<unsigned>(conn.session().spi()), "both agree on the SPI");
}

void test_connection_counts_without_closing() {
    Router r;
    Connection conn{1, r};
    conn.start(0);
    Peer peer{conn};

    // Unhandled: a client-band id with no handler.
    deliver(conn, peer.session.send(Event{Ev::EGS_GOOD_JOB_1_REQ}));
    KNC_CHECK_EQ(conn.stats().events_unhandled, std::uint64_t{1}, "unhandled counted");

    // Refused: a server-band id.
    deliver(conn, peer.session.send(Event{EventId{1325}}));
    KNC_CHECK_EQ(conn.stats().events_refused, std::uint64_t{1}, "refused counted");

    // Authentication failure.
    Bytes bad = peer.session.send(Event{Ev::EGS_GOOD_JOB_1_REQ});
    bad[bad.size() - 1] ^= 0xFF;
    deliver(conn, bad);
    KNC_CHECK_EQ(conn.stats().auth_failures, std::uint64_t{1}, "auth failure counted");

    KNC_CHECK(!conn.wants_close(), "none of those close the connection");
    KNC_CHECK_EQ(conn.stats().frames_in, std::uint64_t{3}, "three frames seen");
}

void test_connection_closes_on_framing_error() {
    Router r;
    Connection conn{1, r};
    conn.start(0);
    deliver(conn, from_hex("0000"));
    KNC_CHECK(conn.wants_close(), "a zero-length frame closes the connection");
    KNC_CHECK(conn.close_reason() == Closed::kFramingError, "...as a framing error");
}

void test_connection_closes_on_traffic_attack() {
    Router r;
    Connection conn{1, r};
    conn.start(0);
    Peer peer{conn};
    conn.session().set_auth_fail_limit(2);

    for (int i = 0; i < 3; ++i) {
        Bytes bad = peer.session.send(Event{Ev::EGS_GOOD_JOB_1_REQ});
        bad[bad.size() - 1] ^= 0x5A;
        deliver(conn, bad);
    }
    KNC_CHECK(conn.wants_close(), "past the limit the connection is dropped");
    KNC_CHECK(conn.close_reason() == Closed::kTrafficAttack, "...as a traffic attack");
}

void test_connection_zombie() {
    Router r;
    Connection conn{1, r};
    const std::uint64_t boot = 1'700'000'000'000ULL;
    conn.start(boot);
    KNC_CHECK(conn.tick(boot + 1000), "a fresh connection survives a tick");
    KNC_CHECK(conn.tick(boot + 60000), "...and 60s exactly");
    KNC_CHECK(!conn.tick(boot + 60001), "but not 60s+1");
    KNC_CHECK(conn.close_reason() == Closed::kZombie, "...reported as a zombie");
}

void test_connection_reassembles_a_split_frame() {
    Router r;
    int hits = 0;
    r.on<Ev::EGS_CONNECT_REQ>(
        [&](Session&, const packets::KEGS_CONNECT_REQ&) { ++hits; });

    Connection conn{1, r};
    conn.start(0);
    Peer peer{conn};

    const Bytes f = peer.session.send_packet<Ev::EGS_CONNECT_REQ>(
        packets::KEGS_CONNECT_REQ{u"split"});
    for (std::size_t i = 0; i < f.size(); ++i) {
        conn.on_bytes(ByteView{f.data() + i, 1}, 1);
    }
    KNC_CHECK_EQ(hits, 1, "one event from a byte-at-a-time delivery");
    KNC_CHECK_EQ(conn.stats().events_routed, std::uint64_t{1}, "...counted once");
}

void test_connection_handles_coalesced_frames() {
    Router r;
    int hits = 0;
    r.on<Ev::EGS_CONNECT_REQ>(
        [&](Session&, const packets::KEGS_CONNECT_REQ&) { ++hits; });

    Connection conn{1, r};
    conn.start(0);
    Peer peer{conn};

    Bytes blob;
    for (int i = 0; i < 4; ++i) {
        const Bytes f = peer.session.send_packet<Ev::EGS_CONNECT_REQ>(
            packets::KEGS_CONNECT_REQ{u"x"});
        blob.insert(blob.end(), f.begin(), f.end());
    }
    deliver(conn, blob);
    KNC_CHECK_EQ(hits, 4, "four events from one delivery");
}

void test_connection_reports_every_event() {
    Router r;
    r.on<Ev::EGS_CONNECT_REQ>([](Session&, const packets::KEGS_CONNECT_REQ&) {});

    Connection conn{1, r};
    conn.start(0);
    Peer peer{conn};

    std::vector<std::pair<unsigned, Routed>> seen;
    conn.on_event = [&](const Connection&, const Event& ev, Routed status,
                        const std::string&) {
        seen.emplace_back(static_cast<unsigned>(ev.id()), status);
    };

    deliver(conn, peer.session.send_packet<Ev::EGS_CONNECT_REQ>(
                      packets::KEGS_CONNECT_REQ{u"1"}));
    deliver(conn, peer.session.send(Event{Ev::EGS_GOOD_JOB_1_REQ}));
    deliver(conn, peer.session.send(Event{Ev::E_HEART_BEAT}));

    KNC_CHECK_EQ(seen.size(), std::size_t{2},
                 "the heartbeat is absorbed and never reported as an event");
    KNC_CHECK(seen[0].second == Routed::kHandled, "the first was handled");
    KNC_CHECK(seen[1].second == Routed::kUnhandled, "the second had no handler");
}

}  // namespace

int main() {
    knctest::Suite suite{"router and connection"};

    suite.run("typed handler gets the bound packet", test_typed_handler_gets_the_bound_packet);
    suite.run("scalar handler", test_scalar_handler);
    suite.run("unhandled is not an error", test_unhandled_is_not_an_error);
    suite.run("duplicate registration is refused", test_duplicate_registration_is_refused);
    suite.run("other-region registration is refused", test_other_region_registration_is_refused);
    suite.run("decode failure is reported not thrown", test_decode_failure_is_reported_not_thrown);
    suite.run("throwing handler is contained", test_throwing_handler_is_contained);
    suite.run("handler can reply", test_handler_can_reply);
    suite.run("connection hands out a key on start", test_connection_hands_out_a_key_on_start);
    suite.run("connection counts without closing", test_connection_counts_without_closing);
    suite.run("connection closes on framing error", test_connection_closes_on_framing_error);
    suite.run("connection closes on traffic attack", test_connection_closes_on_traffic_attack);
    suite.run("connection zombie", test_connection_zombie);
    suite.run("connection reassembles a split frame", test_connection_reassembles_a_split_frame);
    suite.run("connection handles coalesced frames", test_connection_handles_coalesced_frames);
    suite.run("connection reports every event", test_connection_reports_every_event);

    return suite.summary();
}
