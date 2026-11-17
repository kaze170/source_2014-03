#!/usr/bin/env bash
# Cross-verify src/proto/event_ids.inc (produced by a Python regex parser)
# against a REAL C++ preprocessor expanding the original legacy headers.
set -euo pipefail
ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
LEGACY="${LEGACY_COMMON:-${ROOT}/../KncWX2Server/Common}"
WORK="${WORK_DIR:-${ROOT}/.build/crosscheck}"
PY="${PYTHON:-python3}"

if [[ ! -f "${LEGACY}/EventID_System.h" ]]; then
  echo "legacy headers not found at ${LEGACY}; set LEGACY_COMMON" >&2
  exit 2
fi

H="$WORK/hdr"
rm -rf "$H"; mkdir -p "$H/OnlyGlobal"
cp "${LEGACY}/EventID_System.h" "${LEGACY}/EventID_Client.h" "${LEGACY}/EventID_Server.h" "$H/"
# The legacy headers #include "OnlyGlobal/EventID_*_global.h" with a lowercase
# 'g' while the files on disk are '..._Global.h' - a Windows case-insensitivity
# accident. Copy under the expected names; never rename anything in the
# legacy tree.
cp "${LEGACY}/OnlyGlobal/EventID_System_Global.h" "$H/OnlyGlobal/EventID_System_global.h"
cp "${LEGACY}/OnlyGlobal/EventID_Client_Global.h" "$H/OnlyGlobal/EventID_Client_global.h"
cp "${LEGACY}/OnlyGlobal/EventID_Server_Global.h" "$H/OnlyGlobal/EventID_Server_global.h"

# Row 0 is written `_ENUM( E_HEART_BEAT = 0 )`; the initialiser makes the
# (unsigned)(id) value-table trick ill-formed. Drop it in the throwaway copy -
# row 0 of the first enum is 0 either way. Print the diff so the transform is
# auditable.
sed -i 's/_ENUM(\s*E_HEART_BEAT\s*=\s*0\s*)/_ENUM( E_HEART_BEAT )/' "$H/EventID_System.h"
echo "-- the only transform applied to the legacy headers:"
diff "${LEGACY}/EventID_System.h" "$H/EventID_System.h" || true
echo

cat > "$WORK/dump.cpp" <<'EOF'
#include <cstdio>
#undef  _ENUM
#define _ENUM( id ) id,
enum ENUM_SYSTEM_EVENT_ID {
#   include "EventID_System.h"
};
enum ENUM_CLIENT_EVENT_ID {
    E_CLIENT_EVENT_ID_BEGIN = E_SYSTEM_EVENT_ID_END,
#   include "EventID_Client.h"
};
enum ENUM_SERVER_EVENT_ID {
    E_SERVER_EVENT_ID_BEGIN = EGS_CLIENT_EVENT_ID_END,
#   include "EventID_Server.h"
};
#undef  _ENUM
#define _ENUM( id ) #id,
static const char* kNames[] = {
#   include "EventID_System.h"
#   include "EventID_Client.h"
#   include "EventID_Server.h"
};
#undef  _ENUM
#define _ENUM( id ) (unsigned)(id),
static const unsigned kValues[] = {
#   include "EventID_System.h"
#   include "EventID_Client.h"
#   include "EventID_Server.h"
};
int main() {
    const unsigned n = (unsigned)(sizeof(kNames) / sizeof(kNames[0]));
    const unsigned m = (unsigned)(sizeof(kValues) / sizeof(kValues[0]));
    if (n != m) { std::fprintf(stderr, "size mismatch %u vs %u\n", n, m); return 2; }
    for (unsigned i = 0; i < n; ++i) std::printf("%u\t%s\t%u\n", i, kNames[i], kValues[i]);
}
EOF

"${CXX:-g++}" -std=c++17 -w -I"$H" "$WORK/dump.cpp" -o "$WORK/dump"
"$WORK/dump" > "$WORK/events.tsv"

"$PY" - "$WORK/events.tsv" "${ROOT}/src/proto/event_ids.inc" <<'EOF'
import re, sys
truth = []
for line in open(sys.argv[1]):
    i, name, val = line.rstrip('\n').split('\t')
    truth.append((int(i), name, int(val)))
gen = [(n, int(v)) for n, v in
       re.findall(r'KNC_EVENT_ID\(\s*([A-Za-z_]\w*)\s*,\s*(\d+)\s*\)', open(sys.argv[2]).read())]

print(f"compiler-derived rows : {len(truth)}")
print(f"generated .inc rows   : {len(gen)}")
if len(truth) != len(gen):
    print("!! LENGTH MISMATCH"); sys.exit(1)
bad = [(i, name, val, gn, gv)
       for (i, name, val), (gn, gv) in zip(truth, gen)
       if name != gn or val != gv or i != gv]
print(f"mismatched rows       : {len(bad)}")
for b in bad[:20]: print("  ", b)
sys.exit(1 if bad else 0)
EOF
echo "PASS: regex generator and C++ compiler agree on every row; index == value throughout"
