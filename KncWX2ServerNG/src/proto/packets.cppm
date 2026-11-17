// ---------------------------------------------------------------------------
// knc.proto.packets - the 1953 serialisable packet and data structs of the
// TW/HK build, generated from the original 2014 headers.
//
// This file is hand-written; the eight packets_N.inc files it includes are
// not. See tools/gen_packets.py for how they are produced and for the
// self-checks that have to pass before they can be.
//
// The shards exist purely so that no single translation unit has to swallow
// 2 MB of struct definitions at once; the split point carries no meaning and
// moves whenever the struct count changes. Always include this module, never
// an individual .inc.
//
// Names are the legacy ones, verbatim: KEGS_CONNECT_ACK, m_iOK, m_wstrVersion.
// That is deliberate. These structs are a transcription of a wire format we
// do not control, and keeping the names identical is what makes it possible
// to diff any struct here against KncWX2Server/Common/*Packet.h by eye.
// ---------------------------------------------------------------------------

#ifdef KNC_CLASSIC
#  pragma once
#else
module;
#endif

#include "../knc_prelude.hpp"

#include <array>
#include <cstdint>
#include <deque>
#include <list>
#include <map>
#include <set>
#include <string>
#include <type_traits>
#include <utility>
#include <vector>

#ifdef KNC_CLASSIC
#  include "../core/types.cppm"
#  include "serializer.cppm"
#  include "packet_io.cppm"
#else
export module knc.proto.packets;
export import knc.core.types;
export import knc.proto.serializer;
export import knc.proto.packet_io;
#endif

KNC_EXPORT namespace knc::proto::packets {

using knc::proto::Codec;
using knc::proto::Reader;
using knc::proto::Writer;

#include "packets/packets_0.inc"
#include "packets/packets_1.inc"
#include "packets/packets_2.inc"
#include "packets/packets_3.inc"
#include "packets/packets_4.inc"
#include "packets/packets_5.inc"
#include "packets/packets_6.inc"
#include "packets/packets_7.inc"
#include "packets/packets_aliases.inc"

}  // namespace knc::proto::packets
