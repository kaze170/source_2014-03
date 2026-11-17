// ---------------------------------------------------------------------------
// classic_tu.cpp - the single translation unit of the KNC_CLASSIC build.
//
// In module mode every .cppm is its own translation unit and this file is not
// compiled. In header mode (-DKNC_CLASSIC) the .cppm files are header-only, so
// a static library still needs one real TU; this is it. Pulling every module
// in here also means the classic build type-checks the whole core, not just
// whatever the tests happen to touch.
// ---------------------------------------------------------------------------
#ifndef KNC_CLASSIC
#  error "classic_tu.cpp is only part of the KNC_CLASSIC build"
#endif

#include "core/types.cppm"
#include "crypto/des.cppm"
#include "crypto/md5.cppm"
#include "proto/event.cppm"
#include "proto/event_ids.cppm"
#include "proto/framing.cppm"
#include "proto/packet_io.cppm"
#include "proto/packets.cppm"
#include "proto/security.cppm"
#include "proto/serbuffer.cppm"
#include "proto/serializer.cppm"

namespace knc {

/// Linked into the classic build so the TU is never empty.
const char* core_build_mode() noexcept { return "classic (headers)"; }

}  // namespace knc
