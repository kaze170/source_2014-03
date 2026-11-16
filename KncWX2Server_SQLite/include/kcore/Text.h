// kcore/Text.h - character set helpers (UTF-8, UTF-16LE, CP949/UHC).
#pragma once

#include "kcore/Types.h"

namespace knc {

// Number of (lead, trail) pairs covered by kUhcToUnicode: 70 lead bytes (0x81..0xC6) * 178 trail
// bytes (0x41..0x5A, 0x61..0x7A, 0x81..0xFE).
constexpr std::size_t kUhcLeadFirst = 0x81;
constexpr std::size_t kUhcLeadLast = 0xC6;
constexpr std::size_t kUhcLeadCount = kUhcLeadLast - kUhcLeadFirst + 1;  // 70
constexpr std::size_t kUhcTrailTableSize = 178;
constexpr std::size_t kUhcEntryCount = kUhcLeadCount * kUhcTrailTableSize;  // 12460

extern const std::uint16_t kUhcToUnicode[kUhcEntryCount];
// Reverse table lookup helper: index into the trail table for a given trail byte, or -1.
int uhcTrailIndex(byte trail);
// Builds the reverse map (Unicode BMP -> 2 byte UHC) on first use.
byte uhcFromUnicode(char32_t ch, byte& leadOut, byte& trailOut);

}  // namespace knc
