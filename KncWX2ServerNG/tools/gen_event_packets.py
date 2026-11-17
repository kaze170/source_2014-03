#!/usr/bin/env python3
"""
gen_event_packets.py - bind each of the 3386 event ids to the packet struct
that carries its payload, and emit src/proto/event_packets.inc.

Why this is a separate question
-------------------------------
`event_ids.inc` says event 27 is called EGS_CONNECT_ACK. `packets/*.inc` says
there is a struct named KEGS_CONNECT_ACK. Nothing says those two are the same
thing - that is a naming convention (`DECL_PACKET(id)` expands to
`struct K##id`), and conventions have exceptions. There are 3386 ids, 1953
structs for this region, and the relationship is not one to one in either
direction.

Every id falls into exactly one of four buckets:

  kStruct       a packet struct exists in THIS region's build. 1954 ids.
                `DECL_PACKET(id)` produced `struct Kid`, possibly reached
                through a `typedef` alias.

  kScalar       `typedef __int64 KEGS_ACCEPT_FRIEND_REQ;` - the payload is a
                bare serialisable value, not a struct. 41 ids. Easy to miss,
                because the id looks exactly like a packet and there is no
                `DECL_PACKET` anywhere for it; the whole body is one int64,
                bool, wstring or vector.

  kNoStruct     nothing named `Kid` can be serialised here, in any region.
                Most are bare notifications. A few - E_ACCEPT_CONNECTION_NOT
                is the important one - do carry a payload, but it is written
                by hand at the call site rather than through a generated
                struct. The name is "no struct", not "no payload"; do not
                assume an empty body.

  kOtherRegion  `DECL_PACKET` exists, but not in this region's build. 119 ids.
                The id number is still reserved, because event numbering is
                region invariant, but nothing in this build can parse it.
                38 of them sit in the client-originable band, which makes
                them an attack surface: see is_acceptable_from_client().

A struct can be declared and still be unserialisable: KDBE_CCU_AGENT_ACK is
declared unconditionally in ServerPacket.h but its PUT/GET bodies sit behind
SERV_KOG_STATISTICS, and KELG_CHAT_OBSERVATION_NOT is declared under
SERV_GLOBAL_AUTH while its bodies need SERV_INFOSERVER_ADD_WORK as well. The
question that decides a bucket is always "can this build serialise it", never
"is the name declared".

The third bucket is the reason this tool preprocesses all ten regions instead
of just one. Without that comparison a region-excluded id is indistinguishable
from an id that never had a packet, and the difference matters - one is a
feature this build does not have, the other is a protocol-level absence.

Usage:
    python3 tools/gen_event_packets.py --legacy ../KncWX2Server/Common
                                       --region SERV_COUNTRY_TW
                                       --out src/proto/event_packets.inc
"""

from __future__ import annotations

import argparse
import collections
import importlib.util
import os
import re
import sys
import tempfile

HERE = os.path.dirname(os.path.abspath(__file__))
ROOT = os.path.dirname(HERE)

# Every region whose ServerDefine_*.h exists. Preprocessing all of them is
# what separates "this build has no packet for id N" from "no build does".
ALL_REGIONS = [
    "SERV_COUNTRY_TW", "SERV_COUNTRY_JP", "SERV_COUNTRY_EU", "SERV_COUNTRY_US",
    "SERV_COUNTRY_CN", "SERV_COUNTRY_TH", "SERV_COUNTRY_ID", "SERV_COUNTRY_BR",
    "SERV_COUNTRY_PH", "SERV_COUNTRY_IN",
]


def load_gen_packets():
    """Reuse the preprocessing and parsing from gen_packets.py rather than
    reimplementing it - the two must agree about what a packet is."""
    spec = importlib.util.spec_from_file_location(
        "gen_packets", os.path.join(HERE, "gen_packets.py"))
    mod = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(mod)
    return mod


def types_for_region(gp, legacy: str, region: str, work: str) -> set[str]:
    """Names with a SERIALIZE_DEFINE_PUT body under `region`."""
    os.makedirs(work, exist_ok=True)
    files = gp.preprocess(legacy, region, work, "cpp")
    names: set[str] = set()
    for cpp_unit in gp.CPP_UNITS:
        names |= set(gp.parse_serialize_bodies(files[cpp_unit], "PUT"))
    return names


def aliases_for_region(gp, legacy: str, region: str, work: str) -> dict[str, str]:
    """`typedef KFoo KBar;` where KFoo is a real packet - the client knows
    some packets only by the alias."""
    files = gp.preprocess(legacy, region, work, "cpp")
    typedefs: dict[str, str] = {}
    for header in gp.HEADERS:
        for k, v in gp.parse_typedefs(files[header]).items():
            typedefs.setdefault(k, v)
    return typedefs


BANNER = """\
// ---------------------------------------------------------------------------
// event_packets.inc - GENERATED FILE, DO NOT EDIT BY HAND.
//
// Binds every event id to the packet struct carrying its payload.
// Produced by tools/gen_event_packets.py for region {region}.
//
// Three X-macros, one per binding kind. Define the ones you care about and
// leave the rest; each is #undef'd at the end.
//
//   KNC_EVENT_PACKET(name, value, struct)  a packet struct exists here
//   KNC_EVENT_SCALAR(name, value, type)    payload is a bare value
//   KNC_EVENT_NOSTRUCT(name, value)        not serialisable in any region
//   KNC_EVENT_OTHERREGION(name, value)     serialisable, but not in this build
//
// KNC_EVENT_NOSTRUCT means no generated struct, NOT "no payload".
// E_ACCEPT_CONNECTION_NOT carries the SPI and the security association and
// is serialised by hand at the call site.
//
// Regenerate with:
//     python3 tools/gen_event_packets.py --legacy ../KncWX2Server/Common
//                                        --region {region}
//                                        --out src/proto/event_packets.inc
// ---------------------------------------------------------------------------
#ifndef KNC_EVENT_PACKET
#  define KNC_EVENT_PACKET(name, value, type)
#  define KNC_EVENT_PACKET_WAS_EMPTY
#endif
#ifndef KNC_EVENT_SCALAR
#  define KNC_EVENT_SCALAR(name, value, type)
#  define KNC_EVENT_SCALAR_WAS_EMPTY
#endif
#ifndef KNC_EVENT_NOSTRUCT
#  define KNC_EVENT_NOSTRUCT(name, value)
#  define KNC_EVENT_NOSTRUCT_WAS_EMPTY
#endif
#ifndef KNC_EVENT_OTHERREGION
#  define KNC_EVENT_OTHERREGION(name, value)
#  define KNC_EVENT_OTHERREGION_WAS_EMPTY
#endif

"""

FOOTER = """
#ifdef KNC_EVENT_PACKET_WAS_EMPTY
#  undef KNC_EVENT_PACKET_WAS_EMPTY
#endif
#ifdef KNC_EVENT_SCALAR_WAS_EMPTY
#  undef KNC_EVENT_SCALAR_WAS_EMPTY
#endif
#ifdef KNC_EVENT_NOSTRUCT_WAS_EMPTY
#  undef KNC_EVENT_NOSTRUCT_WAS_EMPTY
#endif
#ifdef KNC_EVENT_OTHERREGION_WAS_EMPTY
#  undef KNC_EVENT_OTHERREGION_WAS_EMPTY
#endif
#undef KNC_EVENT_PACKET
#undef KNC_EVENT_SCALAR
#undef KNC_EVENT_NOSTRUCT
#undef KNC_EVENT_OTHERREGION
"""


def main() -> int:
    ap = argparse.ArgumentParser(
        description=__doc__,
        formatter_class=argparse.RawDescriptionHelpFormatter)
    ap.add_argument("--legacy", required=True)
    ap.add_argument("--region", default="SERV_COUNTRY_TW")
    ap.add_argument("--event-ids", default=os.path.join(ROOT, "src/proto/event_ids.inc"))
    ap.add_argument("--out", required=True)
    ap.add_argument("--work", default=None)
    args = ap.parse_args()

    gp = load_gen_packets()
    work_root = args.work or tempfile.mkdtemp(prefix="geneventpkt-")

    # --- the event id table -----------------------------------------------
    ids = [(m.group(1), int(m.group(2))) for m in re.finditer(
        r"KNC_EVENT_ID\((\w+),\s*(\d+)\)", open(args.event_ids).read())]
    if not ids:
        sys.exit(f"no rows found in {args.event_ids}")
    print(f"-- {len(ids)} event ids")

    # --- packets, per region ----------------------------------------------
    here: set[str] = set()
    anywhere: set[str] = set()
    for region in ALL_REGIONS:
        names = types_for_region(gp, args.legacy, region,
                                 os.path.join(work_root, region))
        anywhere |= names
        if region == args.region:
            here = names
        print(f"   {region:22s} {len(names):5d}")
    if args.region not in ALL_REGIONS:
        here = types_for_region(gp, args.legacy, args.region,
                                os.path.join(work_root, args.region))
        anywhere |= here
        print(f"   {args.region:22s} {len(here):5d}")
    if not here:
        sys.exit(f"region {args.region} produced no packets")

    alias = aliases_for_region(gp, args.legacy, args.region,
                               os.path.join(work_root, args.region))

    # --- classify ----------------------------------------------------------
    mapper = gp.TypeMapper(here, alias)

    rows = []
    counts: collections.Counter = collections.Counter()
    problems: list[str] = []
    for name, value in ids:
        k = "K" + name
        target = None
        if k in here:
            target = k
        elif k in alias:
            # Follow the typedef chain. It may land on a packet struct (an
            # alias row) or on something that is not a packet at all (a
            # scalar payload).
            seen, cur = set(), alias[k]
            while cur in alias and cur not in here and cur not in seen:
                seen.add(cur)
                cur = alias[cur]
            if cur in here:
                target = cur
            else:
                mapped = mapper.map(cur)
                if mapped is None:
                    problems.append(
                        f"{name}: K{name} is a typedef to {cur!r}, which is "
                        f"neither a packet nor a type with a known codec")
                    continue
                rows.append(("SCALAR", name, value, mapped, cur))
                counts["kScalar"] += 1
                continue
        if target is not None:
            rows.append(("PACKET", name, value, target, None))
            counts["kStruct"] += 1
        elif k in anywhere:
            rows.append(("OTHERREGION", name, value, None, None))
            counts["kOtherRegion"] += 1
        else:
            rows.append(("NOSTRUCT", name, value, None, None))
            counts["kNoStruct"] += 1

    # --- self-checks -------------------------------------------------------
    if [v for _, v in ids] != list(range(len(ids))):
        problems.append("event id table is not a dense 0..N-1 range; the "
                        "binding table indexes by value and relies on it")
    if len(rows) != len(ids):
        problems.append("row count does not match the event id count")

    # Every struct this build has that is NAMED like an event id must be
    # bound. If one is not, the naming convention has an exception nobody
    # noticed, and a dispatcher would silently never reach that packet.
    bound = {r[3] for r in rows if r[0] == "PACKET"}
    idnames = {"K" + n for n, _ in ids}
    unbound = sorted((here & idnames) - bound)
    if unbound:
        problems.append(f"structs named after an event id but not bound to "
                        f"it: {unbound[:10]}")

    if problems:
        print(f"\n!! {len(problems)} problem(s):", file=sys.stderr)
        for p in problems:
            print("   " + p, file=sys.stderr)
        return 1

    # --- emit --------------------------------------------------------------
    os.makedirs(os.path.dirname(os.path.abspath(args.out)) or ".", exist_ok=True)
    with open(args.out, "w", encoding="utf-8") as fh:
        fh.write(BANNER.format(region=args.region))
        for kind, name, value, target, legacy in rows:
            if kind == "PACKET":
                fh.write(f"KNC_EVENT_PACKET({name}, {value}, {target})\n")
            elif kind == "SCALAR":
                fh.write(f"KNC_EVENT_SCALAR({name}, {value}, {target})"
                         f"  // {legacy}\n")
            elif kind == "OTHERREGION":
                fh.write(f"KNC_EVENT_OTHERREGION({name}, {value})\n")
            else:
                fh.write(f"KNC_EVENT_NOSTRUCT({name}, {value})\n")
        fh.write(FOOTER)

    client_band_excluded = sum(
        1 for r in rows if r[0] == "OTHERREGION" and r[2] <= 1324)
    print(f"\n-- wrote {args.out}")
    for k in ("kStruct", "kScalar", "kNoStruct", "kOtherRegion"):
        print(f"     {counts[k]:5d}  {k}")
    print(f"   of the kOtherRegion ids, {client_band_excluded} are in the "
          f"client-originable band and must be refused at the edge")
    print("-- all self-checks passed")
    return 0


if __name__ == "__main__":
    sys.exit(main())
