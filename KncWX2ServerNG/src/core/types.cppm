// ---------------------------------------------------------------------------
// knc.core.types - fundamental vocabulary shared by every layer.
//
// Mirrors the legacy typedefs so the mapping back to the 2014 sources stays
// obvious:
//     KncWX2Server/Common/KncUidType.h   -> UidType  (__int64)
//     KNCSDK/Include/KncSecurity/...     -> SpiType  (unsigned short)
//                                           SeqType  (unsigned int)
//     KncSecurity::KByteStream           -> Bytes    (std::vector<std::uint8_t>)
// ---------------------------------------------------------------------------
#ifdef KNC_CLASSIC
#  pragma once
#else
module;
#endif

#include "../knc_prelude.hpp"

#ifndef KNC_CLASSIC
export module knc.core.types;
#endif

KNC_EXPORT namespace knc {

// --- legacy scalar aliases -------------------------------------------------

/// `typedef __int64 UidType;` - KncWX2Server/Common/KncUidType.h
using UidType = std::int64_t;

/// Security Parameters Index. `typedef unsigned short SpiType;`
using SpiType = std::uint16_t;

/// ESP-style sequence number. `typedef unsigned int SeqType;`
using SeqType = std::uint32_t;

/// Event id as it appears on the wire (`unsigned short KEvent::m_usEventID`).
using EventId = std::uint16_t;

/// Performer id (`DWORD KPerformerInfo::m_dwPerformerID`).
using PerformerId = std::uint32_t;

using Bytes = std::vector<std::uint8_t>;
using ByteView = std::span<const std::uint8_t>;

/// An empty trace is {-1, -1}; see KEvent::KEvent().
inline constexpr UidType kNoUid = -1;

// --- error type ------------------------------------------------------------

/// Thrown for malformed input that a well-behaved peer could never produce.
/// Network code catches this per-packet and drops the connection rather than
/// letting a malformed frame unwind further.
class ProtocolError : public std::runtime_error {
public:
    using std::runtime_error::runtime_error;
};

// --- small helpers ---------------------------------------------------------

[[nodiscard]] inline ByteView view(const Bytes& b) noexcept {
    return ByteView{b.data(), b.size()};
}

inline void append(Bytes& dst, ByteView src) {
    dst.insert(dst.end(), src.begin(), src.end());
}

/// Constant-time comparison; used for the packet ICV so that a malicious peer
/// cannot learn the expected MAC byte-by-byte from timing.
[[nodiscard]] inline bool constant_time_equal(ByteView a, ByteView b) noexcept {
    if (a.size() != b.size()) {
        return false;
    }
    unsigned diff = 0;
    for (std::size_t i = 0; i < a.size(); ++i) {
        diff |= static_cast<unsigned>(a[i] ^ b[i]);
    }
    return diff == 0;
}

}  // namespace knc
