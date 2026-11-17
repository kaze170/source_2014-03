// ---------------------------------------------------------------------------
// knc.proto.dispatch - what to do with an event id once you have one.
//
// `knc.proto.event_ids` says event 27 is named EGS_CONNECT_ACK.
// `knc.proto.packets` says there is a struct KEGS_CONNECT_ACK.
// Nothing so far says those are the same thing. This module does, from a
// generated table (src/proto/event_packets.inc), and it is explicit about the
// ids where they are NOT the same thing - which is most of them.
//
// Of the 3386 ids in this build:
//
//   1958  have a packet struct here
//     41  carry a BARE VALUE, not a struct - `typedef __int64 KEGS_...`
//   1268  are not serialisable in any region
//    119  are serialisable, but not in this region's build
//
// That last bucket is the interesting one. Event numbering is region
// invariant, so an id belonging to a feature this build does not have still
// occupies its slot. Thirty-eight of them sit in the client-originable band,
// which means a client can name them and nothing here can parse them.
// is_acceptable_from_client() refuses those.
// ---------------------------------------------------------------------------
#ifdef KNC_CLASSIC
#  pragma once
#else
module;
#endif

#include "../knc_prelude.hpp"

#include <array>
#include <cstdint>

#ifdef KNC_CLASSIC
#  include "../core/types.cppm"
#  include "event_ids.cppm"
#  include "packets.cppm"
#  include "serializer.cppm"
#else
export module knc.proto.dispatch;
export import knc.core.types;
export import knc.proto.event_ids;
export import knc.proto.packets;
export import knc.proto.serializer;
#endif

KNC_EXPORT namespace knc::proto {

/// How an event id relates to the packet structs in this build.
enum class PacketBinding : std::uint8_t {
    /// A generated struct carries this event's payload. `PacketOf` names it.
    kStruct = 0,
    /// The payload is a bare serialisable value: `typedef __int64
    /// KEGS_ACCEPT_FRIEND_REQ;` means the whole body is one int64.
    ///
    /// Easy to miss, because there is no `DECL_PACKET` anywhere for these
    /// and the id looks exactly like any other packet. `PacketOf` names the
    /// value type, and decode_packet works on it unchanged.
    kScalar = 3,
    /// Nothing named `Kid` that this build - or any region's build - can
    /// serialise.
    ///
    /// Usually a bare notification. NOT a promise that the payload is empty:
    /// `E_ACCEPT_CONNECTION_NOT` is in this bucket and carries the SPI and
    /// the whole security association, serialised by hand in the handshake
    /// because it has to be written before any SA exists.
    kNoStruct = 1,
    /// A serializer exists, but not in this region's build.
    ///
    /// Note this is about the SERIALIZER, not the declaration. A struct can
    /// be declared here and still be undecodable: KDBE_CCU_AGENT_ACK is
    /// declared unconditionally while its PUT/GET bodies sit behind
    /// SERV_KOG_STATISTICS.
    ///
    /// The id is reserved and must stay reserved - renumbering is not an
    /// option - but nothing here can decode it. Reject rather than guess.
    kOtherRegion = 2,
};

[[nodiscard]] constexpr const char* to_string(PacketBinding b) noexcept {
    switch (b) {
        case PacketBinding::kStruct:      return "struct";
        case PacketBinding::kScalar:      return "scalar";
        case PacketBinding::kNoStruct:    return "no-struct";
        case PacketBinding::kOtherRegion: return "other-region";
    }
    return "unknown";
}

namespace detail {

struct BindingRow {
    PacketBinding binding;
    /// The struct's C++ name, or nullptr. For logging and for the
    /// cross-check; the compile-time binding is `PacketOf`.
    const char* type_name;
};

inline constexpr BindingRow kBindings[] = {
#define KNC_EVENT_PACKET(name, value, type) {PacketBinding::kStruct, #type},
#define KNC_EVENT_SCALAR(name, value, type) {PacketBinding::kScalar, #type},
#define KNC_EVENT_NOSTRUCT(name, value)     {PacketBinding::kNoStruct, nullptr},
#define KNC_EVENT_OTHERREGION(name, value)  {PacketBinding::kOtherRegion, nullptr},
#include "event_packets.inc"
};

inline constexpr std::size_t kBindingCount =
    sizeof(kBindings) / sizeof(kBindings[0]);

static_assert(kBindingCount == kEventIdCount,
              "event_packets.inc and event_ids.inc disagree about how many "
              "event ids exist - regenerate both from the same legacy tree");

}  // namespace detail

/// Out-of-range ids report kOtherRegion: unknown is handled like unparseable,
/// which is the safe direction at the edge.
[[nodiscard]] constexpr PacketBinding packet_binding(EventId id) noexcept {
    return id < detail::kBindingCount ? detail::kBindings[id].binding
                                      : PacketBinding::kOtherRegion;
}

[[nodiscard]] constexpr PacketBinding packet_binding(Ev e) noexcept {
    return packet_binding(raw(e));
}

/// The packet struct's C++ name, or nullptr when there is none.
[[nodiscard]] constexpr const char* packet_type_name(EventId id) noexcept {
    return id < detail::kBindingCount ? detail::kBindings[id].type_name
                                      : nullptr;
}

/// Whether this build can decode the payload of `id` at all - struct or
/// bare value. This, not kStruct alone, is the question a dispatcher asks.
[[nodiscard]] constexpr bool has_payload_type(EventId id) noexcept {
    const PacketBinding b = packet_binding(id);
    return b == PacketBinding::kStruct || b == PacketBinding::kScalar;
}

/// Edge filter for inbound client traffic.
///
/// `is_client_originable` only asks whether the id is in the client band.
/// This additionally refuses the 38 band-resident ids whose packet belongs to
/// a region this build was not compiled for. A legitimate TW/HK client never
/// sends one; accepting it would mean dispatching an event whose payload
/// nothing here understands.
[[nodiscard]] constexpr bool is_acceptable_from_client(EventId id) noexcept {
    return is_client_originable(id) &&
           packet_binding(id) != PacketBinding::kOtherRegion;
}

// ---------------------------------------------------------------------------
// Compile-time binding
//
// `PacketOf<Ev::EGS_CONNECT_REQ>` is `packets::KEGS_CONNECT_REQ`. Only ids in
// the kStruct bucket have a specialisation, so naming one that has no packet
// is a compile error rather than a runtime surprise.
//
// These are type aliases, not code: ~1958 of them cost compile time but emit
// nothing.
// ---------------------------------------------------------------------------

template <Ev e>
struct PacketTraits;

#define KNC_EVENT_PACKET(name, value, type)               \
    template <>                                           \
    struct PacketTraits<Ev::name> {                       \
        using type_t = packets::type;                     \
        static constexpr EventId id = (value);            \
        static constexpr const char* type_name = #type;   \
    };
// Scalar payloads resolve to a plain value type in namespace std / the
// built-ins, so they are NOT qualified with packets::.
#define KNC_EVENT_SCALAR(name, value, type)               \
    template <>                                           \
    struct PacketTraits<Ev::name> {                       \
        using type_t = type;                              \
        static constexpr EventId id = (value);            \
        static constexpr const char* type_name = #type;   \
    };
#include "event_packets.inc"

template <Ev e>
using PacketOf = typename PacketTraits<e>::type_t;

// ---------------------------------------------------------------------------
// Decoding
// ---------------------------------------------------------------------------

/// Decode a payload into `T`, insisting the payload is exactly consumed.
///
/// Goes through `Codec<T>` rather than `T::read_from` so that the 41 events
/// whose payload is a bare int64, bool or wstring decode by the same call as
/// the 1958 that use a struct.
///
/// The legacy reader stopped when it had enough and ignored whatever was
/// left, so a packet with extra bytes on the end parsed "successfully". Here
/// leftover bytes mean the sender and this table disagree about the layout,
/// which is precisely the failure this project exists to avoid - so it
/// throws.
template <typename T>
[[nodiscard]] T decode_packet(ByteView payload) {
    Reader r{payload};
    T value = Codec<T>::get(r);
    if (r.remaining() != 0) {
        throw ProtocolError{"packet payload has trailing bytes"};
    }
    return value;
}

/// Decode the payload of a statically known event.
template <Ev e>
[[nodiscard]] PacketOf<e> decode_packet(ByteView payload) {
    return decode_packet<PacketOf<e>>(payload);
}

/// Encode a packet into a fresh buffer.
template <typename T>
[[nodiscard]] Bytes encode_packet(const T& value) {
    Writer w;
    Codec<T>::put(w, value);
    return std::move(w).take();
}

}  // namespace knc::proto
