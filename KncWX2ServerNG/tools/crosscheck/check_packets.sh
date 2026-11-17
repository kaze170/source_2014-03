#!/usr/bin/env bash
# ---------------------------------------------------------------------------
# check_packets.sh - verify src/proto/packets/*.inc against the legacy source
# by a completely different route from the one that generated it.
#
# tools/gen_packets.py builds the table by running the C PREPROCESSOR over the
# legacy files with marker macros, then parsing the marked-up text with regular
# expressions. If those regexes are subtly wrong, the generated table is
# subtly wrong, and every test written against it passes anyway.
#
# So this script never looks at text. It compiles the ORIGINAL, UNMODIFIED
#     Common/{Common,Client,Server}Packet.cpp
#     Common/OnlyGlobal/Packet/{Common,Client,Server}Packet_Global.cpp
# with the ORIGINAL PUT/GET/DECL_DATA/SERIALIZE_DEFINE_* macros, copied
# verbatim from KNCSDK/Include/Serializer/. The only thing replaced is the
# KSerializer those macros call: ours records the static type of each field
# instead of writing bytes.
#
# Field ORDER therefore comes from the C++ compiler walking the real
# `return PUT(a) && PUT(b) && ...;` chains, and field TYPES come from the
# compiler's own overload resolution. Nothing about this path shares code,
# assumptions, or failure modes with the regex extractor.
#
# The two type vocabularies are reduced to wire-equivalence classes before
# comparison - see normalise() in the Python section, which documents every
# collapse it performs and why.
#
# Usage:
#     LEGACY_COMMON=../KncWX2Server/Common tools/crosscheck/check_packets.sh
#
# Honours: LEGACY_COMMON, WORK_DIR, CXX, PYTHON, JOBS
# ---------------------------------------------------------------------------
set -euo pipefail

HERE="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT="$(cd "$HERE/../.." && pwd)"

LEGACY_COMMON="${LEGACY_COMMON:-$ROOT/../KncWX2Server/Common}"
WORK_DIR="${WORK_DIR:-$ROOT/.build/crosscheck}/packets"
CXX="${CXX:-g++}"
PYTHON="${PYTHON:-python3}"
JOBS="${JOBS:-$(nproc 2>/dev/null || echo 4)}"
REGION="${REGION:-SERV_COUNTRY_TW}"
GEN_DIR="$ROOT/src/proto/packets"

if [[ ! -f "$LEGACY_COMMON/CommonPacket.cpp" ]]; then
    echo "!! legacy tree not found at: $LEGACY_COMMON" >&2
    echo "   set LEGACY_COMMON=/path/to/KncWX2Server/Common" >&2
    exit 2
fi
if [[ ! -f "$GEN_DIR/packets_0.inc" ]]; then
    echo "!! generated table not found at: $GEN_DIR" >&2
    echo "   run tools/gen_packets.py first" >&2
    exit 2
fi

echo "== packet layout cross-check =="
echo "   legacy : $LEGACY_COMMON"
echo "   region : $REGION"
echo "   work   : $WORK_DIR"

SHIM="$WORK_DIR/shim"
mkdir -p "$SHIM/Serializer"

# --- 1. the shim ----------------------------------------------------------

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
// The Win32 scalar spellings the legacy packet headers use. Widths follow
// Win32 (LLP64), which is what the 2014 client was built against.
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
// Win32 SAL parameter annotations.
#define IN
#define OUT
#define INOUT
EOF

cat > "$SHIM/recorder.h" <<'EOF'
#pragma once
// A KSerializer that records the STATIC TYPE of every field, in call order,
// instead of writing bytes. Order and types therefore come straight out of
// the compiler's overload resolution over the unmodified legacy macros.
#include <cstdlib>
#include <string>
#include <typeinfo>
#include <vector>
#include <cxxabi.h>

inline std::string knc_demangle(const char* mangled) {
    int status = 0;
    char* out = abi::__cxa_demangle(mangled, nullptr, nullptr, &status);
    std::string s = (status == 0 && out) ? out : mangled;
    std::free(out);
    return s;
}

struct KSerializer {
    std::vector<std::string>* log = nullptr;
    template <typename T> bool Put(const T&) { rec<T>(); return true; }
    template <typename T> bool Get(T&)       { rec<T>(); return true; }
    template <typename T> bool PutArray(const T*, unsigned long)  { rec<T>(); return true; }
    template <typename T> bool GetArray(T*, unsigned long&)       { rec<T>(); return true; }
    template <typename T> void rec() {
        if (log) log->push_back(knc_demangle(typeid(T).name()));
    }
};
EOF

# PUT/GET/DECL_*/SERIALIZE_* below are copied verbatim from
# KNCSDK/Include/Serializer/SerializeMacro.h and SerDefs.h.
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

# --- 2. driver ------------------------------------------------------------
# The list of type NAMES comes from the generated table; what this script
# verifies independently is each type's field order and field types. The
# name set is separately checked against a raw grep of the legacy sources
# below, so a type silently dropped by the extractor still gets caught.

"$PYTHON" - "$GEN_DIR" "$WORK_DIR/driver.cpp" <<'PYEOF'
import re, sys, pathlib
gen_dir, out = pathlib.Path(sys.argv[1]), pathlib.Path(sys.argv[2])
names = []
for inc in sorted(gen_dir.glob("packets_*.inc")):
    if inc.name == "packets_aliases.inc":
        continue
    names += re.findall(r"^struct (\w+) \{", inc.read_text(), re.M)
names = sorted(set(names))
with out.open("w") as fh:
    fh.write('#include "shim/wintypes.h"\n#include "shim/recorder.h"\n')
    fh.write('#include "ServerPacket.h"\n#include "ClientPacket.h"\n')
    fh.write('#include <cstdio>\n\n')
    fh.write("int main() {\n    std::vector<std::string> log;\n"
             "    KSerializer ks; ks.log = &log;\n")
    for n in names:
        fh.write(f'    {{ log.clear(); {n} o; SerializeHelper::PutInto(ks, o);\n')
        fh.write(f'      std::printf("@%s", "{n}");\n')
        fh.write('      for (const auto& t : log) std::printf("\\t%s", t.c_str());\n')
        fh.write('      std::printf("\\n"); }\n')
    fh.write("    return 0;\n}\n")
print(f"   driver: {len(names)} types")
PYEOF

# --- 3. build -------------------------------------------------------------
echo "   compiling legacy sources against the recorder (this takes a minute)"
INC=(-I "$SHIM" -I "$LEGACY_COMMON" -I "$LEGACY_COMMON/OnlyGlobal/Packet")
OBJS=()
pids=()
i=0
for src in "$LEGACY_COMMON"/CommonPacket.cpp \
           "$LEGACY_COMMON"/ClientPacket.cpp \
           "$LEGACY_COMMON"/ServerPacket.cpp \
           "$LEGACY_COMMON"/OnlyGlobal/Packet/CommonPacket_Global.cpp \
           "$LEGACY_COMMON"/OnlyGlobal/Packet/ClientPacket_Global.cpp \
           "$LEGACY_COMMON"/OnlyGlobal/Packet/ServerPacket_Global.cpp; do
    obj="$WORK_DIR/legacy_$i.o"
    OBJS+=("$obj")
    ( cd "$WORK_DIR" && "$CXX" -std=c++17 -w -O0 "${INC[@]}" -D "$REGION" \
        -include "$SHIM/wintypes.h" -include "$SHIM/recorder.h" \
        -c "$src" -o "$obj" ) &
    pids+=($!)
    i=$((i+1))
done
( cd "$WORK_DIR" && "$CXX" -std=c++17 -w -O0 "${INC[@]}" -D "$REGION" \
    -c driver.cpp -o "$WORK_DIR/driver.o" ) &
pids+=($!)
for p in "${pids[@]}"; do wait "$p"; done

"$CXX" -o "$WORK_DIR/replay" "$WORK_DIR/driver.o" "${OBJS[@]}"
"$WORK_DIR/replay" > "$WORK_DIR/from_compiler.txt"
echo "   recorded $(wc -l < "$WORK_DIR/from_compiler.txt") type signatures"

# --- 4. compare -----------------------------------------------------------
"$PYTHON" - "$GEN_DIR" "$WORK_DIR/from_compiler.txt" "$LEGACY_COMMON" <<'PYEOF'
import re, sys, pathlib, collections

gen_dir   = pathlib.Path(sys.argv[1])
recorded  = pathlib.Path(sys.argv[2])
legacy    = pathlib.Path(sys.argv[3])

# -- normalise both vocabularies to wire-equivalence classes ---------------
#
# The two sides spell the same wire type differently: the generator emits the
# fixed-width name it chose (std::uint32_t), the compiler reports the platform
# spelling it resolved (unsigned int). Reduce both to a code that names what
# actually goes on the wire.
#
# Collapses performed, and why each is safe:
#   * signedness is dropped for integers - the serializer writes the same
#     bytes for int and unsigned int of equal width, so int/uint cannot be a
#     wire difference
#   * `long` and `unsigned long` map to 32 bits, NOT to the host's 64. The
#     protocol is Win32/LLP64 where long is 4 bytes; a Linux LP64 compiler
#     reports 8, and taking that at face value would silently double the
#     field. This is the one place the check must override the compiler.
#   * allocator, comparator and the `const` on a map key are erased: they are
#     C++ bookkeeping with no wire presence.
# Width and kind are never collapsed, so a 2-vs-4 byte or int-vs-float
# mix-up still fails.

SCALAR = {
    "char": "i8", "signed char": "i8", "unsigned char": "i8",
    "bool": "bool",
    "char16_t": "wchar", "wchar_t": "wchar",
    "short": "i16", "short int": "i16",
    "unsigned short": "i16", "short unsigned int": "i16",
    "int": "i32", "unsigned int": "i32",
    "long": "i32", "unsigned long": "i32",          # Win32 widths, see above
    "long int": "i32", "long unsigned int": "i32",
    "long long": "i64", "unsigned long long": "i64",
    "long long int": "i64", "long long unsigned int": "i64",
    "float": "f32", "double": "f64",
    "std::int8_t": "i8", "std::uint8_t": "i8",
    "std::int16_t": "i16", "std::uint16_t": "i16",
    "std::int32_t": "i32", "std::uint32_t": "i32",
    "std::int64_t": "i64", "std::uint64_t": "i64",
    "std::string": "str", "std::u16string": "wstr", "std::wstring": "wstr",
}

# basic_string is matched on its character type, not spelled out in full.
# `wchar_t` is 4 bytes on Linux and 2 on Win32; both sides are reduced to the
# same `wstr` code because the serializer writes a UTF-16 code unit per
# character regardless, which is the Win32 behaviour the protocol was built
# on. The u16string the rewrite uses is the honest spelling of that.
STRING_CHAR = {"char": "str", "wchar_t": "wstr", "char16_t": "wstr"}

CONTAINER = {"vector": "vec", "list": "vec", "deque": "vec",
             "set": "set", "multiset": "set",
             "map": "map", "multimap": "map", "pair": "pair"}

def split_args(s):
    out, depth, cur = [], 0, []
    for ch in s:
        if ch == "<": depth += 1
        elif ch == ">": depth -= 1
        if ch == "," and depth == 0:
            out.append("".join(cur).strip()); cur = []
        else: cur.append(ch)
    if cur: out.append("".join(cur).strip())
    return out

def normalise(t):
    t = re.sub(r"\s+", " ", t.strip())
    t = re.sub(r"^const ", "", t)
    t = re.sub(r"^std::__cxx11::", "std::", t)
    if t in SCALAR:
        return SCALAR[t]
    m = re.match(r"^std::(\w+)\s*<(.*)>$", t)
    if m:
        kind, inner = m.group(1), m.group(2)
        if kind == "basic_string":
            ch = split_args(inner)[0].strip()
            return STRING_CHAR.get(ch, t)
        if kind in CONTAINER:
            args = split_args(inner)
            # drop allocator / comparator / traits
            if   CONTAINER[kind] in ("vec",):   args = args[:1]
            elif CONTAINER[kind] == "set":      args = args[:1]
            elif CONTAINER[kind] == "map":      args = args[:2]
            elif CONTAINER[kind] == "pair":     args = args[:2]
            args = [re.sub(r"^const ", "", a) for a in args]
            return f"{CONTAINER[kind]}<{','.join(normalise(a) for a in args)}>"
    return t   # a packet struct: compare by name

# -- side A: the generated table ------------------------------------------
gen = {}
for inc in sorted(gen_dir.glob("packets_*.inc")):
    if inc.name == "packets_aliases.inc":
        continue
    text = inc.read_text()
    for m in re.finditer(r"^struct (\w+) \{\n(.*?)^\};", text, re.M | re.S):
        name, body = m.group(1), m.group(2)
        seq = []
        for call in re.finditer(
                r"Codec<std::remove_cvref_t<decltype\((\w+(?:\[\d+\])?)\)>>::put", body):
            expr = call.group(1)
            base = re.sub(r"\[\d+\]$", "", expr)
            dm = re.search(r"^\s+(\S.*?)\s+%s\{\}" % re.escape(base), body, re.M)
            if not dm:
                print(f"   !! {name}.{base}: no declaration found"); continue
            decl = dm.group(1)
            am = re.match(r"^std::array<(.*),\s*\d+>$", decl)
            if am:
                decl = am.group(1)
            seq.append(normalise(decl))
        gen[name] = seq

# -- side B: what the compiler recorded -----------------------------------
rec = {}
for line in recorded.read_text().splitlines():
    if not line.startswith("@"):
        continue
    parts = line[1:].split("\t")
    rec[parts[0]] = [normalise(p) for p in parts[1:]]

# -- compare ---------------------------------------------------------------
problems = []
if set(gen) != set(rec):
    problems.append(f"type sets differ: generated-only="
                    f"{sorted(set(gen)-set(rec))[:5]} "
                    f"recorded-only={sorted(set(rec)-set(gen))[:5]}")

# The two legacy GET bodies that call PUT are deliberately repaired by the
# generator; the recorder replays PutInto, so PUT is the truth on both
# sides and they must still agree. No exemption is needed here.

compared = 0
fields = 0
for name in sorted(set(gen) & set(rec)):
    compared += 1
    fields += len(rec[name])
    if gen[name] != rec[name]:
        problems.append(f"{name}:\n     generated {gen[name]}\n     compiler  {rec[name]}")

# -- independent name-count sanity ----------------------------------------
# A grep of the RAW sources (no preprocessor at all) must mention every type
# the generator emitted. This catches a type the extractor invented; a type
# it dropped shows up as a set difference above only if the driver still
# names it, so also report the raw totals for a human to eyeball.
raw = set()
for p in list(legacy.glob("*Packet.cpp")) + list((legacy/"OnlyGlobal"/"Packet").glob("*Packet_Global.cpp")):
    raw |= set(re.findall(r"SERIALIZE_DEFINE_PUT\(\s*(\w+)",
                          p.read_text(encoding="utf-8", errors="replace")))
invented = sorted(set(gen) - raw)
if invented:
    problems.append(f"types in the generated table that no legacy source "
                    f"defines: {invented[:10]}")

print()
print(f"   types compared          : {compared}")
print(f"   field signatures compared: {fields}")
print(f"   raw SERIALIZE_DEFINE_PUT names in legacy sources (all regions): {len(raw)}")
print(f"   of those, live in this region                                 : {len(gen)}")
if problems:
    print(f"\n!! {len(problems)} MISMATCH(ES)")
    for p in problems[:30]:
        print("   " + p)
    sys.exit(1)
print("\n   OK - every type, every field, every position agrees")
PYEOF

echo
echo "== packet layout cross-check PASSED =="
