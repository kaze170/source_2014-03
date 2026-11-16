// kproto/Event.h - KPerformerInfo / KEvent, the envelope every X2 message travels in.
//
// Wire form (KSerializer, no tags, big endian):
//     [DWORD   m_dwPerformerID]
//     [DWORD   uidCount][INT64 uid ...]        (std::set<UidType>, ascending)
//     [INT64   m_anTrace[0]]
//     [INT64   m_anTrace[1]]
//     [USHORT  m_usEventID]
//     [KSerBuffer m_kbuff]                      [DWORD len][bool compressed][bytes]
#pragma once

#include <set>
#include <vector>

#include "kcore/SerBuffer.h"
#include "kcore/Serialize.h"
#include "kcore/Types.h"
#include "kproto/EventId.h"

namespace knc {

struct PerformerInfo {
    static constexpr std::size_t kMaxUidNum = 2000;

    UINT32 performerId = 0;
    std::set<UidType> uids;

    bool addUid(UidType uid);
    bool findUid(UidType uid) const;
    UidType firstUid() const;
    std::size_t uidCount() const { return uids.size(); }
    void clear() {
        performerId = 0;
        uids.clear();
    }

    void serialize(Serializer& s);
};

class Event {
public:
    Event() = default;

    PerformerInfo destPerformer;
    UidType trace[2] = {kInvalidUid, kInvalidUid};
    EventId eventId = 0;
    SerBuffer payload;

    void setData(UINT32 performerId, const UidType anTrace[2], EventId id) {
        destPerformer.performerId = performerId;
        eventId = id;
        if (anTrace != nullptr) {
            trace[0] = anTrace[0];
            trace[1] = anTrace[1];
        } else {
            trace[0] = trace[1] = kInvalidUid;
        }
    }

    template <class T>
    void setData(UINT32 performerId, const UidType anTrace[2], EventId id, const T& data) {
        setData(performerId, anTrace, id);
        payload.clear();
        Serializer ser;
        ser.beginWriting(&payload);
        ser.put(data);
        ser.end();
    }

    template <class T>
    bool getPayload(T& out) const {
        SerBuffer copy = payload;
        Serializer ser;
        ser.beginReading(&copy);
        ser.get(out);
        ser.end();
        return ser.ok();
    }

    UidType firstSenderUid() const { return trace[0]; }
    UidType lastSenderUid() const { return trace[1] == kInvalidUid ? trace[0] : trace[1]; }
    bool isEmptyTrace() const { return trace[0] == kInvalidUid; }

    // Pushes the current first sender into slot 1 and stores uid in slot 0.
    void pushTrace(UidType uid);
    void popTrace();

    // Serialised envelope plus its inner payload (compression applied to m_kbuff only).
    void compressPayload() { payload.compress(); }
    bool uncompressPayload() { return payload.uncompress(); }

    void serialize(Serializer& s);

    // Debug helper: "EGS_CONNECT_REQ(1207) from 5 to PI_GS_USER".
    std::string describe() const;
};

}  // namespace knc
