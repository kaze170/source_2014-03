// KncWX2Server_SQLite - single-process rewrite of the 2014 Elsword X2 server.
// kcore/Types.h - fundamental types shared by every subsystem.
#pragma once

#include <cstddef>
#include <cstdint>
#include <string>
#include <string_view>
#include <vector>

namespace knc {

// The original code base was built against Win32 typedefs (KncSDK/Include/Serializer/BaseTypes.h).
// They are reproduced verbatim so that the wire structures keep identical layouts.
using UCHAR = std::uint8_t;
using USHORT = std::uint16_t;
using UINT = std::uint32_t;
using DWORD = std::uint32_t;
using UINT32 = std::uint32_t;
using INT64 = std::int64_t;
using UINT64 = std::uint64_t;
using byte = std::uint8_t;

// UidType is the signed 64 bit performer identity used by KEvent traces (see KncSend.h PI_*).
using UidType = INT64;
using SpiType = std::uint16_t;
using SeqType = std::uint32_t;

// Byte-stream alias used by the security layer (KByteStream in the original).
using Bytes = std::vector<byte>;

using String = std::string;
// The original serialised std::wstring as UTF-16LE with a 2 byte code unit. On Linux wchar_t is
// 4 bytes wide, therefore every wire string is handled through this alias instead of std::wstring.
using WideString = std::u16string;

constexpr UidType kInvalidUid = static_cast<UidType>(-1);

// Converts UTF-8 <-> UTF-16 (no locale dependency, surrogate pair aware).
WideString toWide(std::string_view utf8);
std::string toUtf8(std::u16string_view utf16);

// Best-effort CP949 (Unified Hangul Code) -> Unicode conversion for the legacy Korean text that is
// stored inside the original server resources and database rows.
std::u16string cp949ToWide(std::string_view cp949);
std::string wideToCp949(std::u16string_view utf16);

}  // namespace knc
