// ---------------------------------------------------------------------------
// knc.proto.event - KEvent / KPerformerInfo, the envelope every packet rides in.
//
// Source of truth: KncWX2Server/Common/{Event.h,Event.cpp}.
//
// Wire layout (KSerializer, untagged):
//
//   KPerformerInfo:
//       DWORD              performerId          (big-endian)
//       DWORD              uidCount             (big-endian)
//       INT64[uidCount]    uids, ascending      (big-endian, std::set order)
//   INT64   trace[0]                            (big-endian)
//   INT64   trace[1]                            (big-endian)
//   USHORT  eventId                             (big-endian)
//   KSerBuffer payload                          (see knc.proto.serbuffer)
//
// An empty trace is {-1, -1}. trace[0] is the first sender, trace[1] the last;
// GetLastSenderUID() returns trace[1] unless it is -1, in which case trace[0].
//
// KEvent::m_usFromType (SERV_KEVENT_FROM) is deliberately NOT on the wire - it
// is a local annotation the legacy server sets on receive. We keep the same
// idea as `origin` below.
// ---------------------------------------------------------------------------
#ifdef KNC_CLASSIC
#  pragma once
#else
module;
#endif

#include "../knc_prelude.hpp"
#include <set>

#ifdef KNC_CLASSIC
#  include "../core/types.cppm"
#  include "event_ids.cppm"
#  include "serbuffer.cppm"
#  include "serializer.cppm"
#else
export module knc.proto.event;
import knc.core.types;
import knc.proto.event_ids;
import knc.proto.serbuffer;
import knc.proto.serializer;
#endif

KNC_EXPORT namespace knc::proto {

/// KPerformerInfo::MAX_UID_NUM
inline constexpr std::size_t kMaxPerformerUids = 2000;

/// Destination descriptor: a performer class plus a set of target uids.
struct PerformerInfo {
    PerformerId performer_id = 0;
    std::set<UidType> uids;

    bool add_uid(UidType uid) {
        if (uids.size() >= kMaxPerformerUids) {
            return false;
        }
        uids.insert(uid);
        return true;
    }

    [[nodiscard]] bool has_uid(UidType uid) const {
        return uids.find(uid) != uids.end();
    }

    [[nodiscard]] UidType first_uid() const {
        return uids.empty() ? kNoUid : *uids.begin();
    }

    void write_to(Writer& w) const {
        w.put_dword(performer_id);
        w.put_container(uids, [](Writer& out, UidType uid) { out.put_int64(uid); });
    }

    static PerformerInfo read_from(Reader& r) {
        PerformerInfo info;
        info.performer_id = r.get_dword();
        r.get_range([&info](Reader& in) { info.uids.insert(in.get_int64()); });
        if (info.uids.size() > kMaxPerformerUids) {
            throw ProtocolError{"PerformerInfo exceeds MAX_UID_NUM"};
        }
        return info;
    }
};

/// Where an event came from. Local only - never serialized.
/// Mirrors KEvent::EVENT_FROM_TYPE under SERV_KEVENT_FROM.
enum class Origin : std::uint8_t { kNone = 0, kServer, kClient };

class Event {
public:
    Event() = default;

    explicit Event(Ev id) : event_id_(raw(id)) {}
    explicit Event(EventId id) : event_id_(id) {}

    [[nodiscard]] EventId id() const noexcept { return event_id_; }
    void set_id(EventId id) noexcept { event_id_ = id; }
    void set_id(Ev id) noexcept { event_id_ = raw(id); }
    [[nodiscard]] const char* name() const noexcept { return event_name(event_id_); }

    [[nodiscard]] PerformerInfo& dest() noexcept { return dest_; }
    [[nodiscard]] const PerformerInfo& dest() const noexcept { return dest_; }

    [[nodiscard]] SerBuffer& payload() noexcept { return payload_; }
    [[nodiscard]] const SerBuffer& payload() const noexcept { return payload_; }

    [[nodiscard]] Origin origin() const noexcept { return origin_; }
    void set_origin(Origin o) noexcept { origin_ = o; }

    // --- trace -------------------------------------------------------------

    [[nodiscard]] UidType trace(std::size_t i) const { return trace_.at(i); }
    void set_trace(UidType first, UidType last) noexcept {
        trace_[0] = first;
        trace_[1] = last;
    }

    [[nodiscard]] UidType first_sender() const noexcept { return trace_[0]; }
    [[nodiscard]] UidType last_sender() const noexcept {
        return trace_[1] == kNoUid ? trace_[0] : trace_[1];
    }
    [[nodiscard]] bool trace_empty() const noexcept { return trace_[0] == kNoUid; }

    /// KEvent::PushTrace - shift the sender chain by one.
    void push_trace(UidType uid) noexcept {
        if (trace_[0] == kNoUid) {
            trace_[0] = uid;
        } else {
            trace_[1] = uid;
        }
    }
    void pop_trace() noexcept {
        if (trace_[1] != kNoUid) {
            trace_[1] = kNoUid;
        } else {
            trace_[0] = kNoUid;
        }
    }

    // --- payload helpers ---------------------------------------------------

    /// Serialize `packet` into the payload. `Packet` must expose
    /// `void write_to(Writer&) const`.
    template <typename Packet>
    void set_packet(const Packet& packet) {
        Writer w;
        packet.write_to(w);
        payload_.assign(std::move(w).take());
    }

    /// Deserialize the payload into a `Packet`, which must expose
    /// `static Packet read_from(Reader&)`.
    template <typename Packet>
    [[nodiscard]] Packet get_packet() const {
        Reader r = payload_.reader();
        return Packet::read_from(r);
    }

    // --- wire format -------------------------------------------------------

    void write_to(Writer& w) const {
        dest_.write_to(w);
        w.put_int64(trace_[0]);
        w.put_int64(trace_[1]);
        w.put_ushort(event_id_);
        payload_.write_to(w);
    }

    [[nodiscard]] Bytes serialize() const {
        Writer w;
        write_to(w);
        return std::move(w).take();
    }

    static Event read_from(Reader& r) {
        Event e;
        e.dest_ = PerformerInfo::read_from(r);
        e.trace_[0] = r.get_int64();
        e.trace_[1] = r.get_int64();
        e.event_id_ = r.get_ushort();
        e.payload_ = SerBuffer::read_from(r);
        return e;
    }

    [[nodiscard]] static Event deserialize(ByteView bytes) {
        Reader r{bytes};
        return read_from(r);
    }

private:
    PerformerInfo dest_;
    std::array<UidType, 2> trace_{kNoUid, kNoUid};
    EventId event_id_ = 0;
    SerBuffer payload_;
    Origin origin_ = Origin::kNone;
};

}  // namespace knc::proto
