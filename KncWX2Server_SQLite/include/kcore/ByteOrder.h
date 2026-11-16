// kcore/ByteOrder.h - host <-> network (big endian) conversion helpers.
#pragma once

#include "kcore/Types.h"

namespace knc {

// The X2 wire protocol is always big endian (KncSDK/Include/Serializer/ByteOrder.h:
// #define BYTEORDER_NET BYTEORDER_BIG). The original host is little endian, so every numeric field
// is byte swapped exactly once on the way out and once on the way in.
inline USHORT swap16(USHORT x) { return static_cast<USHORT>((x >> 8) | (x << 8)); }
inline UINT swap32(UINT x) {
    return ((x & 0x000000FFu) << 24) | ((x & 0x0000FF00u) << 8) | ((x & 0x00FF0000u) >> 8) |
           ((x & 0xFF000000u) >> 24);
}
inline UINT64 swap64(UINT64 x) {
    return ((x & 0x00000000000000FFull) << 56) | ((x & 0x000000000000FF00ull) << 40) |
           ((x & 0x0000000000FF0000ull) << 24) | ((x & 0x00000000FF000000ull) << 8) |
           ((x & 0x000000FF00000000ull) >> 8) | ((x & 0x0000FF0000000000ull) >> 24) |
           ((x & 0x00FF000000000000ull) >> 40) | ((x & 0xFF00000000000000ull) >> 56);
}
inline float swapF32(float x) {
    union {
        float f;
        UINT u;
    } v;
    v.f = x;
    v.u = swap32(v.u);
    return v.f;
}
inline double swapF64(double x) {
    union {
        double d;
        UINT64 u;
    } v;
    v.d = x;
    v.u = swap64(v.u);
    return v.d;
}

// Host -> network. On a big endian host these become no-ops; the target platforms are little
// endian but the check keeps the code honest.
#if defined(__BYTE_ORDER__) && (__BYTE_ORDER__ == __ORDER_BIG_ENDIAN__)
inline USHORT hton16(USHORT x) { return x; }
inline UINT hton32(UINT x) { return x; }
inline UINT64 hton64(UINT64 x) { return x; }
#else
inline USHORT hton16(USHORT x) { return swap16(x); }
inline UINT hton32(UINT x) { return swap32(x); }
inline UINT64 hton64(UINT64 x) { return swap64(x); }
#endif

inline USHORT ntoh16(USHORT x) { return hton16(x); }
inline UINT ntoh32(UINT x) { return hton32(x); }
inline UINT64 ntoh64(UINT64 x) { return hton64(x); }
inline float htonF32(float x) { return swapF32(x); }
inline float ntohF32(float x) { return swapF32(x); }
inline double htonF64(double x) { return swapF64(x); }
inline double ntohF64(double x) { return swapF64(x); }

}  // namespace knc
