// kproto/Packet.h - strongly typed packets on top of Serializer.
//
// Each protocol message is a plain struct with a serialize() function, so the same definition is
// used for writing and reading. Fields are declared in wire order.
#pragma once

#include <cstdint>
#include <string>

#include "kcore/Serialize.h"
#include "kcore/Types.h"

namespace knc {

// Reads a payload into a packet; on a short buffer the missing trailing fields stay zeroed and
// the call reports success only when the packet was fully present (setLenient(false)) or at least
// partially present (setLenient(true)).
template <class T>
bool decodePayload(const SerBuffer& buffer, T& out, bool lenient = false) {
    SerBuffer copy = buffer;
    Serializer ser;
    ser.beginReading(&copy);
    ser.setLenient(lenient);
    ser.get(out);
    ser.end();
    return lenient ? true : ser.ok();
}

template <class T>
void encodePayload(const T& value, SerBuffer& out) {
    out.clear();
    Serializer ser;
    ser.beginWriting(&out);
    ser.put(value);
    ser.end();
}

// ---------------------------------------------------------------------------
// Wire primitives shared by many packets.

// KPacketOK: an int "is success" field used by dozens of ACK packets.
struct PacketOk {
    int ok = 0;
    void serialize(Serializer& s) { s.io(ok); }
};

// KNetAddress (CommonPacket.h): [wstring ip][u16 port]
struct NetAddress {
    WideString ip;
    unsigned short port = 0;
    void serialize(Serializer& s) {
        s.io(ip);
        s.io(port);
    }
};

// KAccountBlockInfo: block end time, reason, plus the 2013 extra fields.
struct AccountBlockInfo {
    WideString endTime;
    WideString blockReason;
    byte blockType = 0;
    WideString blockReason2;
    WideString blockEndDate;
    void serialize(Serializer& s) {
        s.io(endTime);
        s.io(blockReason);
        s.io(blockType);
        s.io(blockReason2);
        s.io(blockEndDate);
    }
};

}  // namespace knc
