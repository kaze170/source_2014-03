// ---------------------------------------------------------------------------
// knc.app.router - event id to handler.
//
// The legacy servers dispatched with a switch. GameServer's ran to several
// thousand cases across a dozen files, and the compiler checked nothing about
// it: an id handled in two places, an id handled nowhere, an id whose payload
// was read as the wrong struct - all of those compiled.
//
// A table fixes the first two by construction (a duplicate registration
// throws, and the unhandled set can be listed). The third is fixed by taking
// the packet type from knc.proto.dispatch instead of from the handler
// author:
//
//     router.on<Ev::EGS_CONNECT_REQ>([](Session& s, const auto& req) { ... });
//
// `req` is whatever src/proto/event_packets.inc says event 26 carries, so a
// handler cannot disagree with the wire format. Naming an id that has no
// packet in this build is a compile error.
// ---------------------------------------------------------------------------
#ifdef KNC_CLASSIC
#  pragma once
#else
module;
#endif

#include "../knc_prelude.hpp"

#include <cstdint>
#include <functional>
#include <string>
#include <unordered_map>
#include <utility>
#include <vector>

#ifdef KNC_CLASSIC
#  include "../core/types.cppm"
#  include "../proto/dispatch.cppm"
#  include "../proto/event.cppm"
#  include "../proto/session.cppm"
#else
export module knc.app.router;
export import knc.core.types;
export import knc.proto.dispatch;
export import knc.proto.event;
export import knc.proto.session;
#endif

KNC_EXPORT namespace knc::app {

using knc::EventId;
using proto::Event;
using proto::Ev;
using proto::Session;

/// What happened to one event.
enum class Routed : std::uint8_t {
    kHandled,
    /// No handler is registered. Not an error at this layer - most of the
    /// 3386 ids will never have one - but worth counting.
    kUnhandled,
    /// A handler exists, but the payload did not decode as the bound packet.
    /// The peer and this build disagree about the layout.
    kDecodeFailed,
    /// The handler threw.
    kHandlerFailed,
};

[[nodiscard]] inline const char* to_string(Routed r) noexcept {
    switch (r) {
        case Routed::kHandled: return "handled";
        case Routed::kUnhandled: return "no handler";
        case Routed::kDecodeFailed: return "payload did not decode";
        case Routed::kHandlerFailed: return "handler threw";
    }
    return "unknown";
}

struct RouteResult {
    Routed status = Routed::kUnhandled;
    /// Reason for kDecodeFailed / kHandlerFailed.
    std::string detail;

    [[nodiscard]] explicit operator bool() const noexcept {
        return status == Routed::kHandled;
    }
};

class Router {
public:
    /// A handler receives the session it arrived on, so it can reply.
    using Handler = std::function<void(Session&, const Event&)>;

    /// Register a handler that takes the decoded packet.
    ///
    /// The packet type comes from the generated binding table, not from the
    /// caller - `PacketOf<E>` - so a handler physically cannot read the
    /// payload as the wrong struct. Ids with no packet in this build
    /// (kNoStruct, kOtherRegion) fail the static_assert; use on_raw() if the
    /// payload really is hand-rolled, as it is for the key exchange.
    template <Ev E, typename Fn>
    void on(Fn&& fn) {
        static_assert(proto::has_payload_type(proto::raw(E)),
                      "this event id has no packet type in this build - use "
                      "on_raw() if the payload is hand-serialised");
        using Packet = proto::PacketOf<E>;
        add(proto::raw(E),
            [fn = std::forward<Fn>(fn)](Session& s, const Event& ev) {
                fn(s, proto::decode_packet<Packet>(view(ev.payload().data())));
            });
    }

    /// Register a handler that takes the raw event.
    void on_raw(EventId id, Handler h) { add(id, std::move(h)); }
    void on_raw(Ev e, Handler h) { add(proto::raw(e), std::move(h)); }

    [[nodiscard]] bool has(EventId id) const {
        return handlers_.find(id) != handlers_.end();
    }
    [[nodiscard]] std::size_t size() const noexcept { return handlers_.size(); }

    /// Every registered id, ascending. For a boot-time report of what this
    /// build actually answers.
    [[nodiscard]] std::vector<EventId> registered() const {
        std::vector<EventId> out;
        out.reserve(handlers_.size());
        for (const auto& [id, _] : handlers_) {
            out.push_back(id);
        }
        std::sort(out.begin(), out.end());
        return out;
    }

    /// Run the handler for `ev`, if there is one.
    ///
    /// Nothing here throws. A handler that throws takes down one event, not
    /// the connection and certainly not the process - the legacy servers ran
    /// handlers inside a __try/__except for exactly this reason, having
    /// learned it the expensive way.
    [[nodiscard]] RouteResult route(Session& session, const Event& ev) const {
        const auto it = handlers_.find(ev.id());
        if (it == handlers_.end()) {
            return RouteResult{Routed::kUnhandled, {}};
        }
        try {
            it->second(session, ev);
        } catch (const ProtocolError& e) {
            // decode_packet throws ProtocolError, so this is overwhelmingly
            // a layout disagreement rather than a handler bug.
            return RouteResult{Routed::kDecodeFailed, e.what()};
        } catch (const std::exception& e) {
            return RouteResult{Routed::kHandlerFailed, e.what()};
        } catch (...) {
            return RouteResult{Routed::kHandlerFailed, "non-std exception"};
        }
        return RouteResult{Routed::kHandled, {}};
    }

private:
    void add(EventId id, Handler h) {
        if (proto::packet_binding(id) == proto::PacketBinding::kOtherRegion) {
            // Nothing in this build can parse the payload, so a handler for
            // it is dead code at best and a misreading of the table at worst.
            throw ProtocolError{"event " + std::to_string(id) +
                                " belongs to another region's build"};
        }
        if (!handlers_.emplace(id, std::move(h)).second) {
            // The legacy switch made this impossible by construction. A map
            // does not, so say so loudly at startup rather than silently
            // keeping whichever registration ran first.
            throw ProtocolError{"event " + std::to_string(id) +
                                " already has a handler"};
        }
    }

    std::unordered_map<EventId, Handler> handlers_;
};

}  // namespace knc::app
