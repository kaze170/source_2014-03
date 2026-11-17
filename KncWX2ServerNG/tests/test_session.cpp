// ---------------------------------------------------------------------------
// Tests for the composed ingress path: raw bytes -> frame -> envelope ->
// KEvent -> packet.
//
// Every layer below has its own suite and its own cross-check against the
// legacy source. Those prove each layer is faithful in isolation. They say
// nothing about the ORDER the layers are used in, which is where a rewrite
// actually goes wrong: seal before switching keys, verify before decrypting,
// decompress after reading the event, count the length prefix inside the
// length. Each of those is invisible to a unit test of the layer involved.
//
// So this suite runs two Sessions against each other and moves bytes between
// them by hand - including one byte at a time, which is what a TCP socket
// really does - and asserts on what comes out the far end.
//
// Several tests deliberately build a BROKEN sender to prove the check would
// catch the mistake. A test that cannot fail is not a test.
// ---------------------------------------------------------------------------
#include "test_util.hpp"

#include <string>
#include <vector>

#include "../src/proto/session.cppm"

using namespace knc;
using namespace knc::proto;
using knctest::from_hex;
using knctest::to_hex;

namespace {

// --- helpers ---------------------------------------------------------------

/// Deterministic keys, so a whole conversation can be pinned byte for byte.
SecurityAssociation fixed_sa() {
    return SecurityAssociation{from_hex("0001020304050607"),
                               from_hex("08090a0b0c0d0e0f")};
}

const Bytes kFixedIv = from_hex("a0a1a2a3a4a5a6a7");

/// The declared length of a frame, read straight off the wire.
std::uint16_t declared_len(const Bytes& frame) {
    return static_cast<std::uint16_t>(frame[0] |
                                      (static_cast<std::uint16_t>(frame[1]) << 8));
}

ByteView body_of(const Bytes& frame) {
    return ByteView{frame.data() + 2, frame.size() - 2};
}

/// Hand a complete buffer over and take the single event it should produce.
Inbound deliver(Session& to, const Bytes& bytes, std::uint64_t now = 0) {
    to.push(view(bytes));
    return to.next(now);
}

/// A connected pair, past the handshake.
struct Pair {
    Session server{Role::kServer};
    Session client{Role::kClient};

    Pair() {
        const Bytes hello = server.accept();
        client.push(view(hello));
        (void)client.next(1);
    }
};

// --- tests -----------------------------------------------------------------

void test_handshake() {
    Session server{Role::kServer};
    Session client{Role::kClient};

    KNC_CHECK(server.spi() == kBootstrapSpi, "server starts on the bootstrap SPI");
    KNC_CHECK(!server.keyed(), "server starts unkeyed");

    const Bytes hello = server.accept();

    // The frame that carries the key must itself be readable with the key the
    // client already has - i.e. sealed under SPI 0, before the switch.
    KNC_CHECK_EQ(static_cast<unsigned>(peek_spi(body_of(hello))), 0u,
                 "E_ACCEPT_CONNECTION_NOT is sealed under the bootstrap SPI");
    KNC_CHECK(server.spi() != kBootstrapSpi, "server switched after sealing");
    KNC_CHECK(server.keyed(), "server is keyed once the SA is handed out");

    const Inbound got = deliver(client, hello, 1);
    KNC_CHECK(got.status == Ingress::kHandled,
              "the client consumes the key exchange itself");
    KNC_CHECK(!got, "a handled frame yields no event");
    KNC_CHECK(client.keyed(), "client is keyed");
    KNC_CHECK_EQ(static_cast<unsigned>(client.spi()),
                 static_cast<unsigned>(server.spi()), "both ends agree on the SPI");
    KNC_CHECK(client.channel().sa() == server.channel().sa(),
              "both ends agree on the security association");
}

void test_handshake_ordering_is_load_bearing() {
    // Prove the ordering in accept() matters, by doing it the other way round.
    // A server that adopts the new SA and only then seals produces a frame
    // the client cannot authenticate - and the client is left stranded,
    // because the key it needs is inside the frame it cannot read.
    SecureChannel wrong;
    FreshSa fresh = create_sa();

    Writer w;
    w.put_ushort(fresh.spi);
    fresh.sa.write_to(w);
    Event ev{Ev::E_ACCEPT_CONNECTION_NOT};
    ev.payload().assign(std::move(w).take());

    wrong.adopt(fresh.spi, fresh.sa);               // switch first (wrong)
    const Bytes sealed = wrong.seal(view(ev.serialize()));

    Session client{Role::kClient};
    const Inbound got = deliver(client, frame(view(sealed)), 1);
    KNC_CHECK(got.status == Ingress::kAuthFailed,
              "sealing after the switch leaves the client unable to read the key");
    KNC_CHECK(got.open_error == OpenError::kBadIcv, "...and it fails on the ICV");
    KNC_CHECK(!client.keyed(), "client stays unkeyed");
}

void test_login_exchange() {
    Pair net;

    // Client -> server.
    const Bytes req =
        net.client.send_packet<Ev::EGS_CONNECT_REQ>(packets::KEGS_CONNECT_REQ{u"1.2.3"});
    const Inbound in = deliver(net.server, req, 2);
    KNC_CHECK(in.status == Ingress::kEvent, "server accepts EGS_CONNECT_REQ");
    KNC_CHECK_EQ(static_cast<unsigned>(in.event.id()),
                 static_cast<unsigned>(raw(Ev::EGS_CONNECT_REQ)), "id survives");
    KNC_CHECK(in.event.origin() == Origin::kClient, "origin is tagged client");
    const auto req_pkt = Session::packet_of<Ev::EGS_CONNECT_REQ>(in.event);
    KNC_CHECK(req_pkt.m_wstrVersion == u"1.2.3", "version string round-trips");

    // Server -> client.
    packets::KEGS_CONNECT_ACK ack{};
    ack.m_iOK = 1;
    ack.m_UDPPort = 0x3A98;
    ack.m_iChannelID = 7;
    const Bytes rsp = net.server.send_packet<Ev::EGS_CONNECT_ACK>(ack);
    const Inbound back = deliver(net.client, rsp, 3);
    KNC_CHECK(back.status == Ingress::kEvent, "client accepts EGS_CONNECT_ACK");
    KNC_CHECK(back.event.origin() == Origin::kServer, "origin is tagged server");
    const auto ack_pkt = Session::packet_of<Ev::EGS_CONNECT_ACK>(back.event);
    KNC_CHECK_EQ(ack_pkt.m_UDPPort, 0x3A98, "UDP port round-trips");
    KNC_CHECK_EQ(ack_pkt.m_iChannelID, 7, "channel id round-trips");

    KNC_CHECK_THROWS(Session::packet_of<Ev::EGS_CONNECT_REQ>(back.event),
                     "decoding an event as the wrong packet throws");
}

void test_length_prefix_counts_itself() {
    Pair net;
    const Bytes f = net.client.send(Event{Ev::EGS_GOOD_JOB_1_REQ});
    KNC_CHECK_EQ(static_cast<std::size_t>(declared_len(f)), f.size(),
                 "the declared length includes its own two bytes");

    // 30-byte empty event (26 of header + 4 for the empty SerBuffer), padded
    // to a multiple of 8 (32), + SPI/Seq/IV/ICV (2+4+8+10) + the 2-byte prefix.
    KNC_CHECK_EQ(f.size(), std::size_t{58},
                 "an empty event frames to exactly 58 bytes");
}

void test_tcp_is_a_byte_stream() {
    // The same conversation, delivered one byte at a time. Nothing may be
    // produced until the last byte of a frame arrives, and then exactly one
    // event must appear.
    Session server{Role::kServer};
    Session client{Role::kClient};
    client.push(view(server.accept()));
    (void)client.next(1);

    const Bytes req =
        client.send_packet<Ev::EGS_CONNECT_REQ>(packets::KEGS_CONNECT_REQ{u"drip"});

    int premature = 0;
    for (std::size_t i = 0; i + 1 < req.size(); ++i) {
        server.push(ByteView{req.data() + i, 1});
        if (server.next(2).status != Ingress::kNeedMoreData) {
            ++premature;
        }
    }
    KNC_CHECK_EQ(premature, 0, "no event before the final byte");

    server.push(ByteView{req.data() + req.size() - 1, 1});
    const Inbound in = server.next(2);
    KNC_CHECK(in.status == Ingress::kEvent, "the last byte completes the frame");
    KNC_CHECK(Session::packet_of<Ev::EGS_CONNECT_REQ>(in.event).m_wstrVersion == u"drip",
              "a byte-at-a-time frame decodes identically");
    KNC_CHECK_EQ(server.buffered(), std::size_t{0}, "nothing left buffered");
}

void test_coalesced_frames() {
    // The opposite case: three frames arrive in one read.
    Pair net;
    Bytes blob;
    for (int i = 0; i < 3; ++i) {
        packets::KEGS_CONNECT_REQ p{};
        p.m_wstrVersion = u"v";
        p.m_wstrVersion[0] = static_cast<char16_t>(u'0' + i);
        const Bytes f = net.client.send_packet<Ev::EGS_CONNECT_REQ>(p);
        blob.insert(blob.end(), f.begin(), f.end());
    }
    net.server.push(view(blob));

    std::u16string seen;
    for (int i = 0; i < 3; ++i) {
        const Inbound in = net.server.next(4);
        KNC_CHECK(in.status == Ingress::kEvent, "coalesced frame yields an event");
        seen += Session::packet_of<Ev::EGS_CONNECT_REQ>(in.event).m_wstrVersion;
    }
    KNC_CHECK(seen == u"012", "all three arrive in order");
    KNC_CHECK(net.server.next(4).status == Ingress::kNeedMoreData,
              "and the stream is then empty");
}

void test_zero_length_frame_cannot_hang() {
    // The legacy loop read a USHORT length, then computed the body size as
    // `usPacketLen - sizeof(USHORT)` in unsigned arithmetic and advanced the
    // cursor by usPacketLen. A declared length of 0 therefore produced a body
    // length of SIZE_MAX-1 and advanced by nothing: a two-byte write spins
    // the receive thread forever.
    Session server{Role::kServer};
    server.push(from_hex("0000"));
    const Inbound in = server.next();
    KNC_CHECK(in.status == Ingress::kFatal, "a zero length is fatal, not a loop");
    KNC_CHECK(in.frame_error == FrameError::kLengthTooSmall, "...reported as too small");

    // Still fatal on the next call - no progress, but no spin either.
    KNC_CHECK(server.next().status == Ingress::kFatal, "it stays fatal");

    Session s2{Role::kServer};
    s2.push(from_hex("ffff"));
    KNC_CHECK(s2.next().frame_error == FrameError::kLengthTooLarge,
              "0xffff exceeds MAX_PACKET_SIZE");
}

void test_auth_failure_is_survivable() {
    Pair net;
    Bytes bad = net.client.send(Event{Ev::EGS_GOOD_JOB_1_REQ});
    bad[bad.size() - 1] ^= 0x01;  // corrupt the ICV

    const Inbound in = deliver(net.server, bad, 5);
    KNC_CHECK(in.status == Ingress::kAuthFailed, "a corrupted frame is rejected");
    KNC_CHECK(in.open_error == OpenError::kBadIcv, "...on the ICV");
    KNC_CHECK_EQ(net.server.auth_failures(), 1u, "the failure is counted");
    KNC_CHECK(!net.server.traffic_attack(), "one failure is not an attack");

    // The stream must still work: dropping a peer on a single bad frame hands
    // anyone who can inject one packet a free disconnect.
    const Bytes good =
        net.client.send_packet<Ev::EGS_CONNECT_REQ>(packets::KEGS_CONNECT_REQ{u"ok"});
    const Inbound after = deliver(net.server, good, 6);
    KNC_CHECK(after.status == Ingress::kEvent, "the next good frame still parses");
}

void test_traffic_attack_threshold() {
    Pair net;
    net.server.set_auth_fail_limit(3);
    for (int i = 0; i < 3; ++i) {
        Bytes bad = net.client.send(Event{Ev::EGS_GOOD_JOB_1_REQ});
        bad[bad.size() - 1] ^= 0x80;
        (void)deliver(net.server, bad, 7);
    }
    KNC_CHECK_EQ(net.server.auth_failures(), 3u, "three failures counted");
    KNC_CHECK(!net.server.traffic_attack(), "the limit is exclusive: 3 > 3 is false");

    Bytes bad = net.client.send(Event{Ev::EGS_GOOD_JOB_1_REQ});
    bad[bad.size() - 1] ^= 0x80;
    (void)deliver(net.server, bad, 7);
    KNC_CHECK(net.server.traffic_attack(), "the fourth crosses it");
}

void test_truncated_body_is_not_authentic() {
    // A frame whose declared length is honest but whose contents were cut
    // short must fail authentication, not parse into something.
    Pair net;
    Bytes f = net.client.send(Event{Ev::EGS_GOOD_JOB_1_REQ});
    f.resize(f.size() - 8);
    f[0] = static_cast<std::uint8_t>(f.size() & 0xFF);
    f[1] = static_cast<std::uint8_t>((f.size() >> 8) & 0xFF);

    const Inbound in = deliver(net.server, f, 8);
    KNC_CHECK(in.status == Ingress::kAuthFailed, "a truncated frame is rejected");
}

void test_heartbeat_is_filtered() {
    Pair net;
    const Bytes hb = net.client.heartbeat();
    const Inbound in = deliver(net.server, hb, 20000);
    KNC_CHECK(in.status == Ingress::kHandled, "a heartbeat never reaches a handler");
    KNC_CHECK(!in, "and yields no event");
    KNC_CHECK_EQ(net.server.last_recv(), std::uint64_t{20000},
                 "but it does count as proof of life");
}

void test_liveness() {
    // The legacy code kept ONE tick field and updated it from different
    // places depending on the role: on send for the client, on receive for
    // the server. Two fields here, same behaviour.
    Pair net;  // handshake at t=1

    // Server: a peer that says nothing for 60s is a zombie.
    KNC_CHECK(!net.server.peer_is_zombie(60000), "60s exactly is not yet a zombie");
    KNC_CHECK(net.server.peer_is_zombie(60001), "past 60s it is");

    // Client: send a heartbeat once 15s have passed since the last one.
    KNC_CHECK(!net.client.heartbeat_due(15000), "15s exactly is not yet due");
    KNC_CHECK(net.client.heartbeat_due(15001), "past 15s it is");

    net.client.set_now(50000);
    const Bytes hb = net.client.heartbeat();  // sent at t=50000
    KNC_CHECK(!net.client.heartbeat_due(65000), "sending one resets the interval");
    KNC_CHECK(net.client.heartbeat_due(65001), "...for exactly 15s");

    (void)deliver(net.server, hb, 50000);
    KNC_CHECK(!net.server.peer_is_zombie(60001),
              "receiving it resets the server's clock too");
    KNC_CHECK(net.server.peer_is_zombie(110001), "...but only for another 60s");

    KNC_CHECK(!net.server.heartbeat_due(999999), "servers do not send heartbeats");

    Session fresh{Role::kClient};
    KNC_CHECK(!fresh.heartbeat_due(999999),
              "an unkeyed client stays silent, whatever the clock says");
    KNC_CHECK(!fresh.key_wait_expired(5000), "5000ms exactly is still waiting");
    KNC_CHECK(fresh.key_wait_expired(5001), "past SECURE_KEY_WAITING_TIME it gives up");
    KNC_CHECK(!net.client.key_wait_expired(999999), "a keyed client never gives up");
}

void test_client_edge_filter() {
    Pair net;

    // A server-band id (>= 1325) sent by a client.
    const Bytes forged = net.client.send(Event{EventId{1325}});
    const Inbound in = deliver(net.server, forged, 9);
    KNC_CHECK(in.status == Ingress::kRefused, "a server-band id from a client is refused");
    KNC_CHECK(std::string{in.detail}.find("not client-originable") != std::string::npos,
              "...for the right reason");

    // One of the 38 client-band ids whose packet belongs to another region.
    EventId other_region = 0;
    for (EventId id = 0; id <= 1324; ++id) {
        if (packet_binding(id) == PacketBinding::kOtherRegion) {
            other_region = id;
            break;
        }
    }
    KNC_CHECK(other_region != 0, "there is a client-band other-region id to test");
    const Bytes region = net.client.send(Event{other_region});
    const Inbound rin = deliver(net.server, region, 10);
    KNC_CHECK(rin.status == Ingress::kRefused, "an other-region id is refused");
    KNC_CHECK(std::string{rin.detail}.find("another region") != std::string::npos,
              "...and says so");

    // The same ids are fine in the other direction: a server may send them.
    const Bytes from_server = net.server.send(Event{EventId{1325}});
    const Inbound sin = deliver(net.client, from_server, 11);
    KNC_CHECK(sin.status == Ingress::kEvent,
              "the filter applies to inbound client traffic only");
}

void test_only_a_server_hands_out_keys() {
    Pair net;

    // A client that could hand the server an SA would be choosing its own
    // session key - and the server would adopt it.
    FreshSa fresh = create_sa();
    Writer w;
    w.put_ushort(fresh.spi);
    fresh.sa.write_to(w);
    Event ev{Ev::E_ACCEPT_CONNECTION_NOT};
    ev.payload().assign(std::move(w).take());

    const SpiType before = net.server.spi();
    const Inbound in = deliver(net.server, net.client.send(ev), 12);
    KNC_CHECK(in.status == Ingress::kRefused, "a client may not hand out a key");
    KNC_CHECK_EQ(static_cast<unsigned>(net.server.spi()),
                 static_cast<unsigned>(before), "the server's SA is unchanged");

    Session twice{Role::kServer};
    (void)twice.accept();
    KNC_CHECK_THROWS(twice.accept(), "a server hands out exactly one key");

    Session not_a_server{Role::kClient};
    KNC_CHECK_THROWS(not_a_server.accept(), "a client cannot accept()");
}

void test_bootstrap_spi_is_refused() {
    // A server offering SPI 0 would leave the client on the bootstrap key,
    // which is a constant compiled into every client in the world.
    SecureChannel ch;
    Writer w;
    w.put_ushort(kBootstrapSpi);
    SecurityAssociation::random().write_to(w);
    Event ev{Ev::E_ACCEPT_CONNECTION_NOT};
    ev.payload().assign(std::move(w).take());

    Session client{Role::kClient};
    const Inbound in = deliver(client, frame(view(ch.seal(view(ev.serialize())))), 13);
    KNC_CHECK(in.status == Ingress::kMalformed, "SPI 0 in a key exchange is rejected");
    KNC_CHECK(!client.keyed(), "and the client stays unkeyed");
}

void test_malformed_event_survives() {
    // Authentic at the envelope layer, garbage inside.
    Pair net;
    SecureChannel& ch = net.client.channel();
    const Bytes junk = from_hex("ffffffff00");
    const Bytes f = frame(view(ch.seal(view(junk))));

    const Inbound in = deliver(net.server, f, 14);
    KNC_CHECK(in.status == Ingress::kMalformed, "an unparseable event is reported");
    KNC_CHECK(std::string{in.detail}.length() > 0, "...with a reason");

    const Bytes good =
        net.client.send_packet<Ev::EGS_CONNECT_REQ>(packets::KEGS_CONNECT_REQ{u"after"});
    KNC_CHECK(deliver(net.server, good, 15).status == Ingress::kEvent,
              "the stream survives a malformed event");
}

void test_compressed_payload() {
    // KSession::OnRecvCompleted decompresses AFTER reading the KEvent. If the
    // ingress path forgets, the payload stays zlib-compressed and every
    // decode downstream fails - or worse, succeeds on garbage.
    Pair net;
    packets::KEGS_CONNECT_REQ big{};
    big.m_wstrVersion = std::u16string(400, u'A');

    Event ev{Ev::EGS_CONNECT_REQ};
    ev.payload().assign(encode_packet(big));
    const std::size_t plain = ev.payload().size();
    ev.payload().compress();
    KNC_CHECK(ev.payload().compressed(), "the payload is compressed");
    KNC_CHECK(ev.payload().size() < plain, "...and smaller");

    const Inbound in = deliver(net.server, net.client.send(ev), 16);
    KNC_CHECK(in.status == Ingress::kEvent, "a compressed event arrives");
    KNC_CHECK(!in.event.payload().compressed(), "the session decompressed it");
    KNC_CHECK(Session::packet_of<Ev::EGS_CONNECT_REQ>(in.event).m_wstrVersion == big.m_wstrVersion,
              "and the packet is intact");
}

void test_sequence_enforcement() {
    Pair net;

    // Off by default - KSession::m_bCheckSequenceNum(false).
    KNC_CHECK(!net.server.channel().check_sequence(), "sequence checking is off by default");
    const Bytes f = net.client.send(Event{Ev::EGS_GOOD_JOB_1_REQ});
    net.server.push(view(f));
    KNC_CHECK(net.server.next(17).status == Ingress::kEvent, "first delivery accepted");
    net.server.push(view(f));
    KNC_CHECK(net.server.next(17).status == Ingress::kEvent,
              "a replay is accepted when checking is off");

    // On: the same frame must not be accepted twice.
    Pair strict;
    strict.server.set_check_sequence(true);
    const Bytes g = strict.client.send(Event{Ev::EGS_GOOD_JOB_1_REQ});
    strict.server.push(view(g));
    KNC_CHECK(strict.server.next(18).status == Ingress::kEvent, "first delivery accepted");
    strict.server.push(view(g));
    const Inbound replay = strict.server.next(18);
    KNC_CHECK(replay.status == Ingress::kAuthFailed, "a replay is rejected when checking is on");
    KNC_CHECK(replay.open_error == OpenError::kReplayedSequence, "...as a stale sequence");
}

void test_sequence_is_not_checked_on_the_bootstrap_sa() {
    // The bootstrap SA is shared by every connection in the world, so its
    // counter is meaningless; the legacy code skips the check while the local
    // SPI is 0 (`if( !IsDefaultSPIndex() && bCheckSequenceNum )`).
    Session server{Role::kServer};
    server.set_check_sequence(true);
    KNC_CHECK(server.spi() == kBootstrapSpi, "still on the bootstrap SA");

    SecureChannel peer;  // also bootstrap
    const Bytes f = frame(view(peer.seal(view(Event{Ev::EGS_GOOD_JOB_1_REQ}.serialize()))));
    server.push(view(f));
    KNC_CHECK(server.next(19).status == Ingress::kEvent, "accepted on the bootstrap SA");
    server.push(view(f));
    KNC_CHECK(server.next(19).status == Ingress::kEvent,
              "and the sequence check stays off until the SA is adopted");
}

void test_recorded_conversation_is_byte_exact() {
    // A full exchange with fixed keys and a fixed IV, pinned byte for byte.
    // This is the regression net: any change anywhere in the stack - framing,
    // padding, key schedule, field order, string encoding - moves these bytes.
    Session client{Role::kClient};
    client.channel().adopt(0x1234, fixed_sa());

    Event ev{Ev::EGS_CONNECT_REQ};
    ev.payload().assign(encode_packet(packets::KEGS_CONNECT_REQ{u"1.2.3"}));
    const Bytes wire = client.send_with_iv(ev, view(kFixedIv));

    // Header: length LE, SPI LE, sequence LE, then the IV in clear.
    //
    // The sequence number is LITTLE-endian here, unlike every DWORD the
    // serializer writes. The envelope header predates the serializer and
    // memcpy's native words; the body above it is big-endian throughout. The
    // first draft of this test asserted big-endian and the pin caught it.
    KNC_CHECK_EQ(static_cast<std::size_t>(declared_len(wire)), wire.size(),
                 "pinned frame: length counts itself");
    KNC_CHECK_EQ(to_hex(Bytes{wire.begin() + 2, wire.begin() + 4}), std::string{"3412"},
                 "pinned frame: SPI is little-endian in the header");
    KNC_CHECK_EQ(to_hex(Bytes{wire.begin() + 4, wire.begin() + 8}),
                 std::string{"01000000"},
                 "pinned frame: sequence starts at 1, little-endian");
    KNC_CHECK_EQ(to_hex(Bytes{wire.begin() + 8, wire.begin() + 16}),
                 to_hex(kFixedIv), "pinned frame: the IV travels in clear");

    // Full-frame pin.
    KNC_CHECK_EQ(wire.size(), std::size_t{74}, "pinned frame: 74 bytes");
    KNC_CHECK_EQ(to_hex(wire),
                 std::string{"4a00341201000000a0a1a2a3a4a5a6a7"
                             "dec905c8764a11a9b73fea13865502db"
                             "1b7c635b6b3b45d776a56200d90170d9"
                             "0558aa630345b1cdf4aed904994166a7"
                             "36a6e019dd66b2cf1378"},
                 "pinned frame: full bytes");

    // And it must still decode on the far side.
    Session server{Role::kServer};
    server.channel().adopt(0x1234, fixed_sa());
    const Inbound in = deliver(server, wire, 21);
    KNC_CHECK(in.status == Ingress::kEvent, "the pinned frame decodes");
    KNC_CHECK(Session::packet_of<Ev::EGS_CONNECT_REQ>(in.event).m_wstrVersion == u"1.2.3",
              "...to the right packet");
}

}  // namespace

int main() {
    knctest::Suite suite{"session"};

    suite.run("handshake", test_handshake);
    suite.run("handshake ordering is load-bearing", test_handshake_ordering_is_load_bearing);
    suite.run("login exchange", test_login_exchange);
    suite.run("length prefix counts itself", test_length_prefix_counts_itself);
    suite.run("tcp is a byte stream", test_tcp_is_a_byte_stream);
    suite.run("coalesced frames", test_coalesced_frames);
    suite.run("zero length frame cannot hang", test_zero_length_frame_cannot_hang);
    suite.run("auth failure is survivable", test_auth_failure_is_survivable);
    suite.run("traffic attack threshold", test_traffic_attack_threshold);
    suite.run("truncated body is not authentic", test_truncated_body_is_not_authentic);
    suite.run("heartbeat is filtered", test_heartbeat_is_filtered);
    suite.run("liveness", test_liveness);
    suite.run("client edge filter", test_client_edge_filter);
    suite.run("only a server hands out keys", test_only_a_server_hands_out_keys);
    suite.run("bootstrap spi is refused", test_bootstrap_spi_is_refused);
    suite.run("malformed event survives", test_malformed_event_survives);
    suite.run("compressed payload", test_compressed_payload);
    suite.run("sequence enforcement", test_sequence_enforcement);
    suite.run("sequence is not checked on the bootstrap sa",
              test_sequence_is_not_checked_on_the_bootstrap_sa);
    suite.run("recorded conversation is byte exact", test_recorded_conversation_is_byte_exact);

    return suite.summary();
}
