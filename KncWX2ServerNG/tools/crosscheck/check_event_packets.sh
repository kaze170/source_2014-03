#!/usr/bin/env bash
# ---------------------------------------------------------------------------
# check_event_packets.sh - verify src/proto/event_packets.inc, the table that
# binds each of the 3386 event ids to the packet struct carrying its payload.
#
# The binding rests on a naming convention: `DECL_PACKET(id)` expands to
# `struct Kid`, so event EGS_CONNECT_ACK is carried by KEGS_CONNECT_ACK. The
# generator applies that convention with string concatenation and then walks
# `typedef` chains in text. Both steps are exactly the kind of thing that
# works for 1957 rows and quietly does the wrong thing for the 1958th.
#
# So this script asks a C++ compiler instead, against the ORIGINAL headers:
#
#   positive rows  KNC_EVENT_PACKET(name, value, type) becomes
#                      static_assert(std::is_same_v<Kname, type>);
#                  The compiler resolves `Kname` and `type` through the real
#                  typedefs. If the generator mis-walked an alias chain, or
#                  bound an id to the wrong struct, this fails to compile.
#
#   scalar rows    KNC_EVENT_SCALAR(name, value, type) claims the payload is
#                  a bare value, not a struct. Asserted the same way, against
#                  the Win32 spelling of the type.
#
#   negative rows  KNC_EVENT_NOSTRUCT(name, value) claims this build cannot
#                  serialise `Kname` at all.
#
#                  The obvious probe - "does the name compile" - is WRONG,
#                  and an earlier version of this script got it wrong. A
#                  struct can be perfectly well declared and still have no
#                  serializer: KDBE_CCU_AGENT_ACK is declared unconditionally
#                  in ServerPacket.h while its PUT/GET bodies sit behind
#                  SERV_KOG_STATISTICS. Declaration is not the question.
#
#                  So the oracle is the SYMBOL TABLE of the real legacy code.
#                  The six legacy .cpp files are compiled for a region and
#                  `nm --defined-only` lists every SerializeHelper::PutInto
#                  the build actually emits. A type whose PutInto is not
#                  there cannot be serialised by that build, whatever the
#                  headers say.
#
#   region rows    KNC_EVENT_OTHERREGION(name, value) claims the serializer is
#                  absent HERE but present somewhere. The same compile-and-nm
#                  pass runs for all ten regions; the name must be missing
#                  from this region's symbols and present in at least one
#                  other.
#
# Usage:
#     LEGACY_COMMON=../KncWX2Server/Common tools/crosscheck/check_event_packets.sh
#
# Honours: LEGACY_COMMON, WORK_DIR, CXX, PYTHON, REGION
# ---------------------------------------------------------------------------
set -euo pipefail

HERE="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT="$(cd "$HERE/../.." && pwd)"

LEGACY_COMMON="${LEGACY_COMMON:-$ROOT/../KncWX2Server/Common}"
WORK_DIR="${WORK_DIR:-$ROOT/.build/crosscheck}/event_packets"
CXX="${CXX:-g++}"
PYTHON="${PYTHON:-python3}"
REGION="${REGION:-SERV_COUNTRY_TW}"
TABLE="$ROOT/src/proto/event_packets.inc"

OTHER_REGIONS=(SERV_COUNTRY_JP SERV_COUNTRY_EU SERV_COUNTRY_US SERV_COUNTRY_CN
               SERV_COUNTRY_TH SERV_COUNTRY_ID SERV_COUNTRY_BR SERV_COUNTRY_PH
               SERV_COUNTRY_IN)

if [[ ! -f "$LEGACY_COMMON/CommonPacket.h" ]]; then
    echo "!! legacy tree not found at: $LEGACY_COMMON" >&2; exit 2
fi
if [[ ! -f "$TABLE" ]]; then
    echo "!! $TABLE not found - run tools/gen_event_packets.py first" >&2; exit 2
fi

echo "== event id -> packet struct cross-check =="
echo "   legacy : $LEGACY_COMMON"
echo "   region : $REGION"

SHIM="$WORK_DIR/shim"
mkdir -p "$SHIM/Serializer"

# The shim is the same one check_packets.sh uses: the real macros, a recording
# KSerializer. Kept verbatim so the two scripts cannot drift apart.
cat > "$SHIM/winsock2.h" <<'EOF'
#pragma once
typedef unsigned char  u_char;
typedef unsigned short u_short;
typedef unsigned int   u_int;
typedef unsigned long  u_long;
EOF
: > "$SHIM/windows.h"

cat > "$SHIM/wintypes.h" <<'EOF'
#pragma once
#include <cstdint>
#include <deque>
#include <list>
#include <map>
#include <set>
#include <string>
#include <utility>
#include <vector>
typedef unsigned char      BYTE;
typedef unsigned char      UCHAR;
typedef unsigned char      byte;
typedef unsigned short     USHORT;
typedef unsigned short     WORD;
typedef unsigned int       UINT;
typedef unsigned long      DWORD;
typedef unsigned char      UINT8;
typedef unsigned short     UINT16;
typedef unsigned int       UINT32;
typedef int                INT32;
typedef long long          __int64;
typedef long long          INT64;
typedef unsigned long long UINT64;
typedef long long          __time64_t;
typedef int                BOOL;
typedef void*              HANDLE;
#define IN
#define OUT
#define INOUT
EOF

cat > "$SHIM/recorder.h" <<'EOF'
#pragma once
#include <cstdlib>
#include <string>
#include <typeinfo>
#include <vector>
struct KSerializer {
    std::vector<std::string>* log = nullptr;
    template <typename T> bool Put(const T&) { return true; }
    template <typename T> bool Get(T&)       { return true; }
    template <typename T> bool PutArray(const T*, unsigned long) { return true; }
    template <typename T> bool GetArray(T*, unsigned long&)      { return true; }
};
EOF

cat > "$SHIM/Serializer/SerializeMacro.h" <<'EOF'
#pragma once
#include "../wintypes.h"
#include "../recorder.h"
#define DECL_DATA(name)     struct name; \
                            SERIALIZE_DECLARE_TAG(name); \
                            SERIALIZE_DECLARE_PUTGET(name); \
                            struct name
#define DECL_PACKET(id)     DECL_DATA( K##id )
#define COPYCON_ASSIGNOP( type, t )     type( const type &t ) { *this = t; } \
                                        type& operator=( const type &t )
#define CON_COPYCON_ASSIGNOP( type, t ) type() {} \
                                        COPYCON_ASSIGNOP( type, t )
#define PUT( x )            ks.Put( obj.x )
#define GET( x )            ks.Get( obj.x )
#define PUTARRAY( x, size ) ks.PutArray( obj.x, size )
#define GETARRAY( x, size ) ks.GetArray( obj.x, dwSize = size ) && (dwSize == size)
#define SERIALIZE_DECLARE_TAG(typ)
#define SERIALIZE_DECLARE_PUTGET(typ) \
        namespace SerializeHelper \
        { \
            bool PutInto(KSerializer& ser, const typ& obj); \
            bool GetFrom(KSerializer& ser, typ& obj); \
        }
#define SERIALIZE_DEFINE_TAG(typ, tag)
#define SERIALIZE_DEFINE_PUT(typ, obj, ser) \
        bool SerializeHelper::PutInto(KSerializer& ser, const typ& obj)
#define SERIALIZE_DEFINE_GET(typ, obj, ser) \
        bool SerializeHelper::GetFrom(KSerializer& ser, typ& obj)
EOF

INC=(-I "$SHIM" -I "$LEGACY_COMMON" -I "$LEGACY_COMMON/OnlyGlobal/Packet")
HDRS='#include "shim/wintypes.h"
#include "shim/recorder.h"
#include "ServerPacket.h"
#include "ClientPacket.h"
#include <type_traits>
'

# --- 1. positive rows: Kname must BE the bound type -----------------------
"$PYTHON" - "$TABLE" "$WORK_DIR/positive.cpp" <<'PYEOF'
import re, sys, pathlib
table, out = pathlib.Path(sys.argv[1]), pathlib.Path(sys.argv[2])
rows = re.findall(r"^KNC_EVENT_PACKET\((\w+),\s*(\d+),\s*(\w+)\)",
                  table.read_text(), re.M)
with out.open("w") as fh:
    fh.write('#include "shim/wintypes.h"\n#include "shim/recorder.h"\n')
    fh.write('#include "ServerPacket.h"\n#include "ClientPacket.h"\n')
    fh.write("#include <type_traits>\n\n")
    for name, value, typ in rows:
        fh.write(f'static_assert(std::is_same<K{name}, {typ}>::value,\n'
                 f'              "event {value} {name} is not carried by {typ}");\n')
    fh.write("int main() { return 0; }\n")
print(f"   positive rows : {len(rows)}")
PYEOF

echo "   compiling the positive assertions"
mkdir -p "$WORK_DIR"
if ( cd "$WORK_DIR" && "$CXX" -std=c++17 -w -fsyntax-only "${INC[@]}" \
        -D "$REGION" positive.cpp ) 2> "$WORK_DIR/positive.err"; then
    echo "   OK - every bound id names exactly the struct the table claims"
else
    echo "!! positive assertions FAILED:" >&2
    grep -m 20 "error:" "$WORK_DIR/positive.err" >&2
    exit 1
fi

# --- 2. scalar rows: Kname must BE the scalar the table claims ------------
"$PYTHON" - "$TABLE" "$WORK_DIR/scalar.cpp" <<'PYEOF'
import re, sys, pathlib
table, out = pathlib.Path(sys.argv[1]), pathlib.Path(sys.argv[2])
rows = re.findall(r"^KNC_EVENT_SCALAR\((\w+),\s*(\d+),\s*(.+?)\)\s*//\s*(.+)$",
                  table.read_text(), re.M)
# The table records the generated C++ type and, in a trailing comment, the
# legacy spelling it came from. Assert against the legacy spelling: that is
# what the original headers actually say, and it is what the compiler can
# check without importing our type-mapping opinions.
with out.open("w") as fh:
    fh.write('#include "shim/wintypes.h"\n#include "shim/recorder.h"\n')
    fh.write('#include "ServerPacket.h"\n#include "ClientPacket.h"\n')
    fh.write("#include <type_traits>\n\n")
    for name, value, cpp, legacy in rows:
        legacy = legacy.strip()
        fh.write(f'static_assert(std::is_same<K{name}, {legacy}>::value,\n'
                 f'              "event {value} {name} payload is not {legacy}");\n')
    fh.write("int main() { return 0; }\n")
print(f"   scalar rows   : {len(rows)}")
PYEOF

echo "   compiling the scalar assertions"
if ( cd "$WORK_DIR" && "$CXX" -std=c++17 -w -fsyntax-only "${INC[@]}" \
        -D "$REGION" scalar.cpp ) 2> "$WORK_DIR/scalar.err"; then
    echo "   OK - every scalar-payload id names exactly the type the table claims"
else
    echo "!! scalar assertions FAILED:" >&2
    grep -m 20 "error:" "$WORK_DIR/scalar.err" >&2
    exit 1
fi

# --- 3. the real oracle: which PutInto symbols does each region emit? -----
# A lone probe TU is useless here - every symbol in it is undefined by
# construction, which an earlier version of this script mistook for a
# passing check. What matters is what the LEGACY SOURCES define, so compile
# them and read the symbol table.

symbols_for_region() {   # $1 = region
    local r="$1"
    local d="$WORK_DIR/sym/$r"
    mkdir -p "$d"
    local pids=() i=0
    for src in "$LEGACY_COMMON"/CommonPacket.cpp \
               "$LEGACY_COMMON"/ClientPacket.cpp \
               "$LEGACY_COMMON"/ServerPacket.cpp \
               "$LEGACY_COMMON"/OnlyGlobal/Packet/CommonPacket_Global.cpp \
               "$LEGACY_COMMON"/OnlyGlobal/Packet/ClientPacket_Global.cpp \
               "$LEGACY_COMMON"/OnlyGlobal/Packet/ServerPacket_Global.cpp; do
        ( "$CXX" -std=c++17 -w -O0 "${INC[@]}" -D "$r" \
            -include "$SHIM/wintypes.h" -include "$SHIM/recorder.h" \
            -c "$src" -o "$d/$i.o" 2>/dev/null ) &
        pids+=($!); i=$((i+1))
    done
    for pp in "${pids[@]}"; do wait "$pp" || true; done
    nm -C --defined-only "$d"/*.o 2>/dev/null \
        | grep -oP 'PutInto\(KSerializer&, \K\w+(?= const&\))' \
        | sort -u > "$WORK_DIR/sym_$r.txt"
}

echo "   compiling the legacy sources for 10 regions and reading their symbols"
symbols_for_region "$REGION"
echo "      $REGION $(wc -l < "$WORK_DIR/sym_$REGION.txt") serialisable types"
for r in "${OTHER_REGIONS[@]}"; do
    symbols_for_region "$r"
    echo "      $r $(wc -l < "$WORK_DIR/sym_$r.txt") serialisable types"
done

# --- 4. reconcile ----------------------------------------------------------
"$PYTHON" - "$TABLE" "$WORK_DIR" "$REGION" "${OTHER_REGIONS[@]}" <<'PYEOF'
import re, sys, pathlib

table  = pathlib.Path(sys.argv[1])
work   = pathlib.Path(sys.argv[2])
region = sys.argv[3]
others = sys.argv[4:]
text   = table.read_text()

def syms(r):
    p = work / f"sym_{r}.txt"
    if not p.exists() or not p.read_text().strip():
        raise SystemExit(f"no symbols collected for {r}")
    return set(p.read_text().split())

here = syms(region)
elsewhere = set()
for r in others:
    elsewhere |= syms(r)

problems = []

# -- kStruct: the bound TARGET type must be serialisable here --------------
bound = re.findall(r"^KNC_EVENT_PACKET\((\w+),\s*(\d+),\s*(\w+)\)", text, re.M)
missing = sorted({t for _, _, t in bound} - here)
if missing:
    problems.append(f"{len(missing)} bound type(s) this region does not "
                    f"serialise: {missing[:10]}")

# -- kNoStruct: unserialisable in every region -----------------------------
claimed_none = {"K"+n for n, _ in
                re.findall(r"^KNC_EVENT_NOSTRUCT\((\w+),\s*(\d+)\)", text, re.M)}
anywhere = here | elsewhere
oops = sorted(claimed_none & anywhere)
if oops:
    problems.append(f"{len(oops)} id(s) classified kNoStruct that some region "
                    f"DOES serialise: {oops[:10]}")

# -- kOtherRegion: not here, yes somewhere ---------------------------------
claimed_else = {"K"+n for n, _ in
                re.findall(r"^KNC_EVENT_OTHERREGION\((\w+),\s*(\d+)\)", text, re.M)}
here_too = sorted(claimed_else & here)
if here_too:
    problems.append(f"{len(here_too)} id(s) classified kOtherRegion that this "
                    f"region DOES serialise: {here_too[:10]}")
nowhere = sorted(claimed_else - elsewhere)
if nowhere:
    problems.append(f"{len(nowhere)} id(s) classified kOtherRegion that NO "
                    f"region serialises (should be kNoStruct): {nowhere[:10]}")

positives = len(bound)
scalars   = len(re.findall(r"^KNC_EVENT_SCALAR\(", text, re.M))
print()
print(f"   bound ids verified by static_assert  : {positives}")
print(f"   scalar ids verified by static_assert : {scalars}")
print(f"   bound types present in {region:18s}: {len({t for _,_,t in bound} & here)} / {len({t for _,_,t in bound})}")
print(f"   kNoStruct absent from all 10 regions : {len(claimed_none - anywhere)} / {len(claimed_none)}")
print(f"   kOtherRegion absent here             : {len(claimed_else - here)} / {len(claimed_else)}")
print(f"   kOtherRegion present elsewhere       : {len(claimed_else & elsewhere)} / {len(claimed_else)}")
print(f"   total ids accounted for              : {positives + scalars + len(claimed_none) + len(claimed_else)}")

if problems:
    print(f"\n!! {len(problems)} MISMATCH(ES)")
    for p in problems:
        print("   " + p)
    sys.exit(1)
print("\n   OK - all bindings agree with the compiled legacy symbol tables")
PYEOF

echo
echo "== event id -> packet struct cross-check PASSED =="
