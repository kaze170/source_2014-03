// ---------------------------------------------------------------------------
// knc.proto.event_ids - the complete 2014 event-id table.
//
// The values come from src/proto/event_ids.inc, which is GENERATED from the
// original Common/EventID_{System,Client,Server}.h by tools/gen_event_ids.py.
// Never hand-edit either file: regenerate.
//
// Layout, reproducing Common/Event.h:
//
//   0    .. 20    system events   (E_HEART_BEAT .. E_SYSTEM_EVENT_ID_END)
//   21   .. 1324  client events   (EGS_* .. EGS_CLIENT_EVENT_ID_END)
//   1325 .. 3385  server events   (.. EVENT_EVENTID_SENTINEL)
//
// EVENT_EVENTID_SENTINEL (3385) is both the "unknown id" marker and the array
// bound, exactly as in KEvent::GetIDStr().
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
export module knc.proto.event_ids;
import knc.core.types;
#endif

KNC_EXPORT namespace knc::proto {

/// Every event id the 2014 protocol knows about.
enum class Ev : EventId {
#define KNC_EVENT_ID(name, value) name = (value),
#include "event_ids.inc"
};

[[nodiscard]] constexpr EventId raw(Ev e) noexcept {
    return static_cast<EventId>(e);
}

/// Number of entries, i.e. EVENT_EVENTID_SENTINEL + 1.
inline constexpr std::size_t kEventIdCount =
    static_cast<std::size_t>(Ev::EVENT_EVENTID_SENTINEL) + 1;

/// First / last id of each band (see the header comment).
inline constexpr EventId kSystemEventEnd =
    static_cast<EventId>(Ev::E_SYSTEM_EVENT_ID_END);
inline constexpr EventId kClientEventBegin = kSystemEventEnd + 1;
inline constexpr EventId kClientEventEnd =
    static_cast<EventId>(Ev::EGS_CLIENT_EVENT_ID_END);
inline constexpr EventId kServerEventBegin = kClientEventEnd + 1;
inline constexpr EventId kEventSentinel =
    static_cast<EventId>(Ev::EVENT_EVENTID_SENTINEL);

static_assert(static_cast<EventId>(Ev::E_HEART_BEAT) == 0);
static_assert(static_cast<EventId>(Ev::E_ACCEPT_CONNECTION_NOT) == 1);
static_assert(kSystemEventEnd == 20);
static_assert(static_cast<EventId>(Ev::EGS_GOOD_JOB_1_REQ) == 21);
static_assert(kClientEventEnd == 1324);
static_assert(kEventSentinel == 3385);
static_assert(kEventIdCount == 3386);

namespace detail {
inline constexpr const char* kEventNames[] = {
#define KNC_EVENT_ID(name, value) #name,
#include "event_ids.inc"
};
static_assert(sizeof(kEventNames) / sizeof(kEventNames[0]) == kEventIdCount,
              "event name table and id table disagree");
}  // namespace detail

/// KEvent::GetIDStr(). Out-of-range ids report as the sentinel, matching the
/// legacy clamp `if (id >= EVENT_EVENTID_SENTINEL) return ms_szEventID[...]`.
[[nodiscard]] inline const char* event_name(EventId id) noexcept {
    if (id >= kEventSentinel) {
        return detail::kEventNames[kEventSentinel];
    }
    return detail::kEventNames[id];
}

[[nodiscard]] inline const char* event_name(Ev e) noexcept {
    return event_name(raw(e));
}

/// True for ids a *client* is ever allowed to originate. The legacy server
/// trusted whatever id arrived and dispatched it, which let a crafted client
/// inject server-to-server events (party/guild/DB replies). We reject those at
/// the edge; see KEvent::IsValidEventID(), which the original only enabled
/// under SERV_KEVENT_FROM.
[[nodiscard]] constexpr bool is_client_originable(EventId id) noexcept {
    return id <= kClientEventEnd;
}

}  // namespace knc::proto
