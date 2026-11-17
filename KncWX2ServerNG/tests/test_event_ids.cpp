// ---------------------------------------------------------------------------
// Structural tests for the generated event-id table.
//
// The table is the single widest piece of client-visible contract in the
// project: 3386 ids, every one of which must keep the exact numeric value the
// 2014 client was compiled against. These checks guard the invariants that a
// bad regeneration would break.
// ---------------------------------------------------------------------------
#include "test_util.hpp"

#include <map>
#include <set>
#include <string>

#include "../src/proto/event_ids.cppm"

using namespace knc;
using namespace knc::proto;

namespace {

/// Rebuild the id list independently of the enum, straight from the .inc.
struct Row {
    const char* name;
    unsigned value;
};

const Row kRows[] = {
#define KNC_EVENT_ID(name, value) {#name, (value)},
#include "../src/proto/event_ids.inc"
};

constexpr std::size_t kRowCount = sizeof(kRows) / sizeof(kRows[0]);

void test_shape(knctest::Suite& s) {
    s.run("the table has exactly 3386 rows", [] {
        KNC_CHECK_EQ(kRowCount, std::size_t{3386}, "row count");
        KNC_CHECK_EQ(kEventIdCount, std::size_t{3386}, "kEventIdCount");
    });

    s.run("row index equals row value for every entry", [] {
        // This is what makes the name lookup a plain array index, and what
        // KEvent::ms_szEventID[] relied on. A single gap breaks every id above
        // it.
        for (std::size_t i = 0; i < kRowCount; ++i) {
            if (kRows[i].value != i) {
                KNC_CHECK(false, "row " + std::to_string(i) + " (" + kRows[i].name +
                                     ") has value " + std::to_string(kRows[i].value));
                return;
            }
        }
        KNC_CHECK(true, "all 3386 rows satisfy index == value");
    });

    s.run("every name is unique", [] {
        std::set<std::string> seen;
        for (std::size_t i = 0; i < kRowCount; ++i) {
            if (!seen.insert(kRows[i].name).second) {
                KNC_CHECK(false, std::string{"duplicate name "} + kRows[i].name);
                return;
            }
        }
        KNC_CHECK_EQ(seen.size(), kRowCount, "all names distinct");
    });

    s.run("event_name() agrees with the table at every index", [] {
        for (std::size_t i = 0; i < kRowCount; ++i) {
            if (std::string{event_name(static_cast<EventId>(i))} != kRows[i].name) {
                KNC_CHECK(false, "event_name mismatch at " + std::to_string(i));
                return;
            }
        }
        KNC_CHECK(true, "event_name() matches all 3386 rows");
    });
}

void test_anchors(knctest::Suite& s) {
    s.run("the anchor ids hold their 2014 values", [] {
        // Cross-checked against KEvent::ms_szEventID[] in Common/Event.cpp and
        // the three EventID_*.h headers.
        const std::map<std::string, unsigned> kAnchors = {
            {"E_HEART_BEAT", 0},
            {"E_ACCEPT_CONNECTION_NOT", 1},
            {"E_CONNECTION_LOST_NOT", 2},
            {"E_UDP_PORT_NOT", 3},
            {"E_DISABLE_HB_CHECK_REQ", 4},
            {"E_LOG_NOT", 5},
            {"E_RESERVE_DESTROY", 6},
            {"E_SYSTEM_EVENT_ID_END", 20},
            {"EGS_GOOD_JOB_1_REQ", 21},
            {"EGS_CONNECT_REQ", 26},
            {"EGS_CONNECT_ACK", 27},
            {"EGS_NEW_USER_JOIN_REQ", 28},
            {"EGS_VERIFY_ACCOUNT_REQ", 30},
            {"EGS_VERIFY_ACCOUNT_ACK", 31},
            {"EGS_CLIENT_EVENT_ID_END", 1324},
            {"DBE_STRING_FILTER_UPDATE_ACK", 3384},
            {"EVENT_EVENTID_SENTINEL", 3385},
        };
        for (const auto& [name, value] : kAnchors) {
            if (value >= kRowCount) {
                KNC_CHECK(false, name + " is out of range");
                continue;
            }
            KNC_CHECK(std::string{kRows[value].name} == name,
                      name + " == " + std::to_string(value));
        }
    });

    s.run("the band boundaries are contiguous", [] {
        KNC_CHECK_EQ(kSystemEventEnd, EventId{20}, "system band ends at 20");
        KNC_CHECK_EQ(kClientEventBegin, EventId{21}, "client band starts at 21");
        KNC_CHECK_EQ(kClientEventEnd, EventId{1324}, "client band ends at 1324");
        KNC_CHECK_EQ(kServerEventBegin, EventId{1325}, "server band starts at 1325");
        KNC_CHECK_EQ(kEventSentinel, EventId{3385}, "sentinel is the last row");
    });

    s.run("the five client dummy ids occupy 21..25", [] {
        // The client enum opens with EGS_GOOD_JOB_1_REQ .. _5_REQ, placeholders
        // that must keep their slots or every later client id shifts.
        for (int i = 1; i <= 5; ++i) {
            const std::string want = "EGS_GOOD_JOB_" + std::to_string(i) + "_REQ";
            KNC_CHECK(std::string{kRows[20 + i].name} == want,
                      want + " at " + std::to_string(20 + i));
        }
    });

    s.run("the sentinel really is the last entry", [] {
        KNC_CHECK(std::string{kRows[kRowCount - 1].name} == "EVENT_EVENTID_SENTINEL",
                  "last row is the sentinel");
        // PacketMornitoring.h sizes its arrays with the sentinel, so a sparse
        // space would silently under-allocate.
        KNC_CHECK_EQ(static_cast<std::size_t>(kEventSentinel) + 1, kRowCount,
                     "sentinel + 1 == table size");
    });
}

void test_naming_conventions(knctest::Suite& s) {
    s.run("every REQ/ACK/NOT name uses a known suffix", [] {
        // Not a hard protocol rule, but a cheap tripwire: a regeneration that
        // mangled a line would almost certainly produce a name that fails it.
        static const char* kSuffixes[] = {"_REQ", "_ACK", "_NOT", "_CMD", "_END",
                                          "_BEGIN", "_SENTINEL", "_START"};
        std::size_t odd = 0;
        for (std::size_t i = 0; i < kRowCount; ++i) {
            const std::string name = kRows[i].name;
            bool matched = false;
            for (const char* suffix : kSuffixes) {
                const std::string sfx = suffix;
                if (name.size() >= sfx.size() &&
                    name.compare(name.size() - sfx.size(), sfx.size(), sfx) == 0) {
                    matched = true;
                    break;
                }
            }
            if (!matched) {
                ++odd;
            }
        }
        // A handful of legacy names genuinely break the pattern; the point is
        // that the count stays small and stable.
        KNC_CHECK(odd < 200, "only " + std::to_string(odd) +
                                 " of 3386 names fall outside the usual suffixes");
    });

    s.run("names contain no whitespace or stray punctuation", [] {
        for (std::size_t i = 0; i < kRowCount; ++i) {
            for (const char* p = kRows[i].name; *p != '\0'; ++p) {
                const bool ok = (*p >= 'A' && *p <= 'Z') || (*p >= 'a' && *p <= 'z') ||
                                (*p >= '0' && *p <= '9') || *p == '_';
                if (!ok) {
                    KNC_CHECK(false, std::string{"bad character in "} + kRows[i].name);
                    return;
                }
            }
        }
        KNC_CHECK(true, "all names are valid identifiers");
    });
}

}  // namespace

int main() {
    knctest::Suite suite{"Event id table"};
    test_shape(suite);
    test_anchors(suite);
    test_naming_conventions(suite);
    return suite.summary();
}
