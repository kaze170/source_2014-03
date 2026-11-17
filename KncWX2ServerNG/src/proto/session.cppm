// ---------------------------------------------------------------------------
// knc.proto.session - the whole ingress path, bytes in, events out.
//
// Every layer below this one has been verified in isolation: framing, the
// ESP-like envelope, the serializer, SerBuffer, KEvent, the event ids, the
// packet structs, the id->packet binding. None of that proves they compose.
// The order of operations across layers is its own contract, and it is the
// part most likely to be wrong in a rewrite:
//
//   * the length prefix counts ITSELF, so the body is `declared - 2`;
//   * the ICV is verified BEFORE the ciphertext is decrypted;
//   * the sequence check is skipped while still on the bootstrap SA;
//   * E_ACCEPT_CONNECTION_NOT is sealed under the OLD SPI and only then does
//     the sender switch to the new one;
//   * the event's SerBuffer is decompressed AFTER the KEvent is read, not
//     during;
//   * heartbeats are consumed at this layer and never reach a handler.
//
// Get any of those backwards and every individual unit test still passes.
//
// This mirrors KSkSession::OnRecvCompleted (Socket/Session.cpp:13) feeding
// KSession::OnRecvCompleted (:601), plus KSession::SendPacket (:391) and
// KSession::OnAcceptConnection (:575).
// ---------------------------------------------------------------------------
#ifdef KNC_CLASSIC
#  pragma once
#else
module;
#endif

#include "../knc_prelude.hpp"

#include <cstdint>
#include <utility>

#ifdef KNC_CLASSIC
#  include "../core/types.cppm"
#  include "dispatch.cppm"
#  include "event.cppm"
#  include "framing.cppm"
#  include "security.cppm"
#else
export module knc.proto.session;
export import knc.core.types;
export import knc.proto.dispatch;
export import knc.proto.event;
export import knc.proto.framing;
export import knc.proto.security;
#endif

KNC_EXPORT namespace knc::proto {

/// Which end of the connection this is.
///
/// The legacy code called these "un-proxy" and "proxy" and selected between
/// them with the UNPROXY_ONLY / PROXY_ONLY macros, which expand to an early
/// `return` in the wrong role. The distinction that matters on the wire: the
/// server MINTS the security association and the client ADOPTS it.
enum class Role : std::uint8_t { kServer, kClient };

/// Outcome of trying to pull one event off the stream.
enum class Ingress : std::uint8_t {
    /// Not a whole frame yet. Push more bytes.
    kNeedMoreData,
    /// `Inbound::event` is filled in.
    kEvent,
    /// The session consumed it itself: a heartbeat, or the key exchange.
    kHandled,
    /// Authentication failed. The frame is discarded and the stream survives,
    /// which is deliberate - see `auth_failures()`.
    kAuthFailed,
    /// Authentic, but the contents could not be parsed as a KEvent.
    kMalformed,
    /// Well-formed, but this peer is not allowed to send this event id.
    kRefused,
    /// The byte stream itself is unusable. Close the connection.
    kFatal,
};

[[nodiscard]] inline const char* to_string(Ingress s) noexcept {
    switch (s) {
        case Ingress::kNeedMoreData: return "need more data";
        case Ingress::kEvent: return "event";
        case Ingress::kHandled: return "handled by the session";
        case Ingress::kAuthFailed: return "authentication failed";
        case Ingress::kMalformed: return "malformed event";
        case Ingress::kRefused: return "event id refused from this peer";
        case Ingress::kFatal: return "stream is unusable";
    }
    return "unknown";
}

struct Inbound {
    Ingress status = Ingress::kNeedMoreData;
    /// Valid only when `status == kEvent`.
    Event event{};
    /// Set when the failure came from the framing or envelope layer.
    FrameError frame_error = FrameError::kOk;
    OpenError open_error = OpenError::kOk;
    /// Human-readable reason for kMalformed / kRefused / kFatal.
    const char* detail = "";

    [[nodiscard]] explicit operator bool() const noexcept {
        return status == Ingress::kEvent;
    }
};

/// KSession::PACKET_AUTH_FAIL_DEFAULT (Socket/Session.h:38).
inline constexpr std::uint32_t kDefaultAuthFailLimit = 100;

/// KSession::SECURE_KEY_WAITING_TIME (Socket/Session.h:37). How long a client
/// waits for the server to hand it a key before giving up.
inline constexpr std::uint64_t kSecureKeyWaitMs = 5000;

/// A client sends a heartbeat when it has been quiet for longer than this.
inline constexpr std::uint64_t kHeartbeatIntervalMs = 15000;

/// A server treats a silent connection as dead after this long.
inline constexpr std::uint64_t kHeartbeatTimeoutMs = 60000;

/// One connection's protocol state: framing buffer, security association,
/// handshake progress, liveness.
///
/// Produces byte buffers rather than writing to a socket, so the whole stack
/// is testable without one - and so a recorded conversation can be replayed
/// through it offline.
class Session {
public:
    explicit Session(Role role) : role_(role) {}

    [[nodiscard]] Role role() const noexcept { return role_; }
    [[nodiscard]] SpiType spi() const noexcept { return channel_.spi(); }
    [[nodiscard]] SecureChannel& channel() noexcept { return channel_; }
    [[nodiscard]] const SecureChannel& channel() const noexcept { return channel_; }

    /// True once the two ends share a session SA. Mirrors m_bAuthKeyRecved
    /// on the client; on the server it is set when the key is handed out.
    [[nodiscard]] bool keyed() const noexcept { return keyed_; }

    void set_check_sequence(bool on) noexcept { channel_.set_check_sequence(on); }

    // --- handshake ---------------------------------------------------------

    /// Server side of KSession::OnAcceptConnection.
    ///
    /// Mints a session SA, seals it into E_ACCEPT_CONNECTION_NOT **under the
    /// bootstrap SPI**, and only then adopts it. That ordering is the whole
    /// handshake: seal first, switch second. Reverse the two lines and the
    /// client - still on the bootstrap key - cannot read the frame that
    /// carries the key it needs.
    ///
    /// Returns the framed bytes to put on the socket.
    [[nodiscard]] Bytes accept() { return accept_with(create_sa(), ByteView{}); }

    /// Deterministic accept(): the caller supplies the SA, and optionally the
    /// IV, so that a handshake can be replayed byte for byte. Same ordering
    /// guarantee - seal under the old SPI, then switch.
    [[nodiscard]] Bytes accept_with(FreshSa fresh, ByteView iv) {
        if (role_ != Role::kServer) {
            throw ProtocolError{"only a server session hands out keys"};
        }
        if (keyed_) {
            throw ProtocolError{"session has already handed out a key"};
        }

        // KSerializer: Put(SpiType) then Put(KSecurityAssociation). SpiType is
        // a USHORT, so it goes out BIG-endian here - while the same number in
        // the frame header is LITTLE-endian. Not a typo; see PROTOCOL.md.
        Writer w;
        w.put_ushort(fresh.spi);
        fresh.sa.write_to(w);

        Event ev{Ev::E_ACCEPT_CONNECTION_NOT};
        ev.payload().assign(std::move(w).take());

        Bytes out = iv.empty() ? send(ev) : send_with_iv(ev, iv);  // old SPI
        channel_.adopt(fresh.spi, std::move(fresh.sa));            // then switch
        keyed_ = true;
        return out;
    }

    // --- egress ------------------------------------------------------------

    /// KSession::SendPacket: serialize the event, seal it, prepend the length.
    [[nodiscard]] Bytes send(const Event& ev) {
        return frame(view(channel_.seal(view(ev.serialize()))));
    }

    /// Deterministic variant: caller supplies the IV. For tests and for
    /// replaying a recorded conversation byte for byte.
    [[nodiscard]] Bytes send_with_iv(const Event& ev, ByteView iv) {
        return frame(view(channel_.seal_with_iv(view(ev.serialize()), iv)));
    }

    /// Build and send an event whose payload is the packet type bound to `E`.
    ///
    /// The binding comes from knc.proto.dispatch, so a mismatched packet type
    /// is a compile error rather than a wire-format surprise.
    template <Ev E, typename T = PacketOf<E>>
    [[nodiscard]] Bytes send_packet(const T& packet) {
        static_assert(has_payload_type(raw(E)),
                      "this event id has no packet type in this build");
        Event ev{E};
        ev.payload().assign(encode_packet(packet));
        return send(ev);
    }

    // --- outbound queue ----------------------------------------------------
    //
    // KSession::SendPacket wrote straight to the socket, which is why the
    // protocol and the transport were one class. Here a reply is queued and
    // whoever owns the socket drains it, so a handler can answer without
    // knowing whether there is a socket at all.

    /// Queue already-framed bytes.
    void post_bytes(const Bytes& bytes) { append(out_, view(bytes)); }

    /// Seal, frame and queue an event.
    void post(const Event& ev) { post_bytes(send(ev)); }

    /// Seal, frame and queue the packet bound to `E`.
    template <Ev E, typename T = PacketOf<E>>
    void post_packet(const T& packet) {
        post_bytes(send_packet<E>(packet));
    }

    [[nodiscard]] bool has_pending_output() const noexcept { return !out_.empty(); }
    [[nodiscard]] ByteView pending_output() const noexcept { return view(out_); }
    void consume_output(std::size_t n) {
        out_.erase(out_.begin(), out_.begin() + static_cast<std::ptrdiff_t>(n));
    }

    /// An empty E_HEART_BEAT: 30 bytes of event before sealing, 58 on the wire.
    [[nodiscard]] Bytes heartbeat() {
        last_heartbeat_sent_ms_ = now_ms_;
        return send(Event{Ev::E_HEART_BEAT});
    }

    // --- ingress -----------------------------------------------------------

    void push(ByteView data) { reader_.push(data); }

    /// Pull the next event, if the stream holds a complete one.
    ///
    /// `now_ms` is supplied by the caller rather than read from a clock, so
    /// that liveness is testable and so the 49.7-day wrap of the legacy
    /// GetTickCount() cannot recur. Zero means "no opinion, keep the clock
    /// where it is".
    [[nodiscard]] Inbound next(std::uint64_t now_ms = 0) {
        if (now_ms != 0) {
            set_now(now_ms);
        }
        Inbound in;

        const FrameReader::Next frame_in = reader_.next();
        if (frame_in.failed()) {
            // A bad length is unrecoverable: the stream is desynchronised and
            // there is no resynchronisation marker to hunt for. The legacy
            // loop instead computed `usPacketLen - 2` in unsigned arithmetic,
            // so a declared length of 0 produced a body length of SIZE_MAX-1
            // and then advanced the cursor by zero. One two-byte write would
            // spin a server thread forever.
            in.status = Ingress::kFatal;
            in.frame_error = frame_in.error;
            in.detail = to_string(frame_in.error);
            return in;
        }
        if (!frame_in.has_frame) {
            return in;  // kNeedMoreData
        }

        const OpenResult opened = channel_.open(frame_in.body);
        if (!opened) {
            // Deliberately NOT fatal. The legacy server tolerated up to
            // PACKET_AUTH_FAIL_DEFAULT failures before dropping the peer,
            // because a single corrupted frame is not evidence of an attack -
            // and because disconnecting on the first one hands anyone who can
            // inject a packet a free denial of service.
            ++auth_failures_;
            in.status = Ingress::kAuthFailed;
            in.open_error = opened.error;
            in.detail = to_string(opened.error);
            return in;
        }

        // Any authentic frame counts as proof of life, including heartbeats
        // and including events we go on to refuse.
        last_recv_ms_ = now_ms_;

        Event ev;
        try {
            ev = Event::deserialize(view(opened.payload));
            // KSession::OnRecvCompleted decompresses AFTER reading the event,
            // not inside SerBuffer::read_from - the compression flag belongs
            // to the payload, and the event header around it is never
            // compressed.
            ev.payload().decompress();
        } catch (const ProtocolError& e) {
            in.status = Ingress::kMalformed;
            in.detail = e.what();
            return in;
        }

        ev.set_origin(role_ == Role::kServer ? Origin::kClient : Origin::kServer);

        // --- events the session answers itself -----------------------------

        if (ev.id() == raw(Ev::E_HEART_BEAT)) {
            // "heart bit filtering - no queueing". Never reaches a handler.
            in.status = Ingress::kHandled;
            in.detail = "heartbeat";
            return in;
        }

        if (ev.id() == raw(Ev::E_ACCEPT_CONNECTION_NOT)) {
            if (role_ != Role::kClient) {
                // A client telling the server what key to use would be an
                // excellent way to pick your own session key.
                in.status = Ingress::kRefused;
                in.detail = "only a server may hand out a security association";
                return in;
            }
            try {
                adopt_from(ev);
            } catch (const ProtocolError& e) {
                in.status = Ingress::kMalformed;
                in.detail = e.what();
                return in;
            }
            in.status = Ingress::kHandled;
            in.detail = "adopted the session key";
            return in;
        }

        // --- the client edge -----------------------------------------------

        if (role_ == Role::kServer && !is_acceptable_from_client(ev.id())) {
            // Two separate refusals wearing one name. Either the id is in the
            // server-only band (>= 1325) and a client has no business naming
            // it, or it is in the client band but belongs to a region this
            // build was not compiled for - 38 ids are in that second group,
            // and nothing here could parse their payload anyway.
            in.status = Ingress::kRefused;
            in.detail = is_client_originable(ev.id())
                            ? "event id belongs to another region's build"
                            : "event id is not client-originable";
            return in;
        }

        in.status = Ingress::kEvent;
        in.event = std::move(ev);
        return in;
    }

    /// Decode an inbound event's payload as the packet type bound to `E`.
    /// Throws unless the event really is `E`.
    template <Ev E, typename T = PacketOf<E>>
    [[nodiscard]] static T packet_of(const Event& ev) {
        static_assert(has_payload_type(raw(E)),
                      "this event id has no packet type in this build");
        if (ev.id() != raw(E)) {
            throw ProtocolError{"event id does not match the requested packet"};
        }
        return decode_packet<T>(view(ev.payload().data()));
    }

    // --- liveness ----------------------------------------------------------

    /// Start the clocks.
    ///
    /// Every deadline here is `now - some_past_event`, and all of those start
    /// at zero. A session created when the clock already reads large - which
    /// is what any steady clock gives you, milliseconds since boot - is
    /// therefore born 60 seconds silent and reaped on its first tick. The
    /// unit tests never saw it because they count from zero; the first live
    /// connection died instantly.
    void begin(std::uint64_t now_ms) noexcept {
        started_ = true;
        started_at_ = now_ms;
        now_ms_ = now_ms;
        last_recv_ms_ = now_ms;
        last_heartbeat_sent_ms_ = now_ms;
    }

    /// The first call also starts the clocks, so a caller that only ever
    /// passes a real timestamp cannot trip over the above.
    void set_now(std::uint64_t now_ms) noexcept {
        if (!started_) {
            begin(now_ms);
            return;
        }
        now_ms_ = now_ms;
    }

    [[nodiscard]] bool started() const noexcept { return started_; }
    [[nodiscard]] std::uint64_t started_at() const noexcept { return started_at_; }
    [[nodiscard]] std::uint64_t now() const noexcept { return now_ms_; }
    [[nodiscard]] std::uint64_t last_recv() const noexcept { return last_recv_ms_; }

    /// Client side: time to send a heartbeat? Suppressed until keyed, exactly
    /// as the legacy `if( !m_bAuthKeyRecved ) return;`.
    [[nodiscard]] bool heartbeat_due(std::uint64_t now_ms) const noexcept {
        if (role_ != Role::kClient || !keyed_) {
            return false;
        }
        return now_ms - last_heartbeat_sent_ms_ > kHeartbeatIntervalMs;
    }

    /// Server side: has the peer gone quiet long enough to be declared dead?
    /// (eSIColDR_Heart_Beat.)
    [[nodiscard]] bool peer_is_zombie(std::uint64_t now_ms) const noexcept {
        return role_ == Role::kServer && now_ms - last_recv_ms_ > kHeartbeatTimeoutMs;
    }

    /// Client side: the server never sent a key within SECURE_KEY_WAITING_TIME.
    /// Measured from begin(), not from zero - see the note there.
    [[nodiscard]] bool key_wait_expired(std::uint64_t now_ms) const noexcept {
        return role_ == Role::kClient && !keyed_ &&
               now_ms - started_at_ > kSecureKeyWaitMs;
    }

    // --- abuse counters ----------------------------------------------------

    [[nodiscard]] std::uint32_t auth_failures() const noexcept {
        return auth_failures_;
    }
    void set_auth_fail_limit(std::uint32_t limit) noexcept {
        auth_fail_limit_ = limit;
    }
    /// KSkSession: `> limit`, not `>=`. Kept exact.
    [[nodiscard]] bool traffic_attack() const noexcept {
        return auth_failures_ > auth_fail_limit_;
    }

    [[nodiscard]] std::size_t buffered() const noexcept { return reader_.buffered(); }

private:
    /// Client side of E_ACCEPT_CONNECTION_NOT: read SPI + SA and switch.
    void adopt_from(const Event& ev) {
        Reader r = ev.payload().reader();
        const SpiType spi = r.get_ushort();
        SecurityAssociation sa = SecurityAssociation::read_from(r);
        if (r.remaining() != 0) {
            throw ProtocolError{"trailing bytes in the key-exchange payload"};
        }
        if (spi == kBootstrapSpi) {
            // The server is required to mint a non-zero SPI. Accepting 0 here
            // would silently keep us on the bootstrap key, which every client
            // in the world already has.
            throw ProtocolError{"server handed out the bootstrap SPI"};
        }
        channel_.adopt(spi, std::move(sa));
        keyed_ = true;
    }

    Role role_;
    FrameReader reader_;
    Bytes out_;
    SecureChannel channel_;
    bool keyed_ = false;
    std::uint32_t auth_failures_ = 0;
    std::uint32_t auth_fail_limit_ = kDefaultAuthFailLimit;
    bool started_ = false;
    std::uint64_t started_at_ = 0;
    std::uint64_t now_ms_ = 0;
    std::uint64_t last_recv_ms_ = 0;
    std::uint64_t last_heartbeat_sent_ms_ = 0;
};

}  // namespace knc::proto
