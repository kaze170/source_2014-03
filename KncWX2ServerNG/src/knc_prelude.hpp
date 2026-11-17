// ---------------------------------------------------------------------------
// knc_prelude.hpp
//
// Included from the *global module fragment* of every .cppm in this project.
//
// Dual-build contract
// -------------------
// The shipping build is C++26 + named modules (MSVC / VC++26, `.cppm` primary
// module interface units).  The same sources also compile in "classic" mode
// (-DKNC_CLASSIC) where each .cppm behaves like a `#pragma once` header, which
// is what the portable conformance harness in tests/ uses.  One source of
// truth, two build modes - so the thing we verify is literally the thing we
// ship.
//
// Each .cppm therefore begins with:
//
//     #ifdef KNC_CLASSIC
//     #  pragma once
//     #else
//     module;
//     #endif
//     #include "knc_prelude.hpp"
//     ... any extra std headers ...
//     #ifdef KNC_CLASSIC
//     #  include "dependency.cppm"
//     #else
//     export module knc.something;
//     import knc.dependency;
//     #endif
//
// and marks exported entities with KNC_EXPORT.
// ---------------------------------------------------------------------------
#pragma once

#include <algorithm>
#include <array>
#include <bit>
#include <concepts>
#include <cstddef>
#include <cstdint>
#include <cstring>
#include <limits>
#include <optional>
#include <span>
#include <stdexcept>
#include <string>
#include <string_view>
#include <type_traits>
#include <utility>
#include <vector>

#ifdef KNC_CLASSIC
#  define KNC_EXPORT
#else
#  define KNC_EXPORT export
#endif

// The legacy protocol is defined in terms of a little-endian host (every
// KncWX2Server build was x86 Win32).  We reproduce the byte layout explicitly
// rather than relying on the host, but a big-endian host would still need the
// raw-struct paths reviewed, so make the assumption loud.
static_assert(std::endian::native == std::endian::little,
              "KncWX2ServerNG currently assumes a little-endian host; see "
              "docs/PROTOCOL.md section 'Host byte order'.");
static_assert(sizeof(char16_t) == 2, "wire wchar_t is UTF-16LE");
