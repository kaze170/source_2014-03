#!/usr/bin/env python3
"""
gen_packets.py - Generate the KncWX2ServerNG packet structs from the ORIGINAL
KncWX2Server headers, so the new server's field order and types are identical
to what the 2014 client was compiled against.

Why generate instead of hand-port?
----------------------------------
For TW/HK there are 1953 serialisable structs carrying 7537 fields, and a
layout is decided in two places that must agree:

    Common/*Packet.h    the struct members          -> field TYPES
    Common/*Packet.cpp  SERIALIZE_DEFINE_PUT bodies -> field ORDER

Declaration order and wire order are not always the same, so neither file can
be read alone. Both are riddled with regional `#ifdef`s - 742 macros are live
for TW/HK - so "which fields exist" is not a question you can answer by
reading either. Hand-porting 7537 fields would be thousands of chances to
transpose two lines, and every transposition is a silent wire-format break
that compiles.

How it works
------------
1. The real C preprocessor resolves every `#ifdef` for the chosen region. We
   feed the original headers through `cpp` with a stub `SerializeMacro.h` whose
   DECL_PACKET / PUT / GET macros expand to unambiguous text markers instead of
   C++. Nothing is hand-evaluated.
2. The marked-up output is parsed into (struct -> members) and
   (packet -> ordered field list).
3. The two are joined: order comes from the .cpp, types come from the .h.
4. C++ is emitted, in dependency order, against knc.proto.packet_io.

Note which files get preprocessed. `OnlyGlobal/Packet/*_Global.cpp` are
separate translation units that nothing #includes; miss them and you lose 210
types and pick up the wrong KBillProductInfo, because the copy in
CommonPacket.cpp sits behind `#ifndef SERV_GLOBAL_BILLING`.

Self-checks that must hold, or the tool refuses to emit
-------------------------------------------------------
  * every SERIALIZE_DEFINE_PUT has a matching SERIALIZE_DEFINE_GET
  * PUT field order == GET field order, outside the KNOWN_GET_BODY_DEFECTS
    allowlist (which is itself checked for staleness)
  * every field named in a PUT body resolves to a declared member
  * every member type resolves to a known codec or a generated struct
  * every fixed array is serialised as exactly indices 0..N-1
  * no two translation units define conflicting bodies for one type
  * the struct dependency graph is acyclic

The output is verified a second time, by a path that shares nothing with this
one, in tools/crosscheck/check_packets.sh - read that before trusting this.

Usage:
    python3 tools/gen_packets.py \
        --legacy ../KncWX2Server/Common \
        --region SERV_COUNTRY_TW \
        --out src/proto/packets
"""

from __future__ import annotations

import argparse
import collections
import os
import re
import subprocess
import sys
import tempfile

# ---------------------------------------------------------------------------
# Phase 1: preprocess
# ---------------------------------------------------------------------------

# Stub macros. Each expands to a marker that cannot occur in real source, so
# the parser never has to guess where a construct starts or ends.
STUB_SERIALIZE_MACRO = r"""
#pragma once
#define DECL_DATA(name)                     @@S@@ name
#define DECL_PACKET(id)                     DECL_DATA( K##id )
#define COPYCON_ASSIGNOP( type, t )         @@CTOR@@
#define CON_COPYCON_ASSIGNOP( type, t )     @@CTOR@@
#define PUT( x )                            @@P@@ x @@/P@@
#define GET( x )                            @@G@@ x @@/G@@
#define PUTARRAY( x, size )                 @@PA@@ x @@,@@ size @@/PA@@
#define GETARRAY( x, size )                 @@GA@@ x @@,@@ size @@/GA@@
#define SERIALIZE_DEFINE_PUT(typ, obj, ks)  @@PUTDEF@@ typ @@/PUTDEF@@
#define SERIALIZE_DEFINE_GET(typ, obj, ks)  @@GETDEF@@ typ @@/GETDEF@@
#define SERIALIZE_DEFINE_TAG(typ, tag)
#define SERIALIZE_DECLARE_PUTGET(typ)
#define SERIALIZE_DECLARE_FRIEND(typ)
"""

# The three main headers. Each of them #includes its OnlyGlobal counterpart
# when the region enables SERV_GLOBAL_COMMON, so parsing these three picks up
# every struct declaration.
HEADERS = ["CommonPacket.h", "ClientPacket.h", "ServerPacket.h"]

# The .cpp files are separate translation units and must each be preprocessed.
# OnlyGlobal/Packet/*_Global.cpp are *not* #included by anything - they are
# compiled on their own, and for TW/HK they carry 210 further PUT/GET pairs
# (including a REPLACEMENT KBillProductInfo; the CommonPacket.cpp one is
# behind #ifndef SERV_GLOBAL_BILLING and is dead for this region).
CPP_UNITS = ["CommonPacket.cpp", "ClientPacket.cpp", "ServerPacket.cpp",
             "OnlyGlobal/Packet/CommonPacket_Global.cpp",
             "OnlyGlobal/Packet/ClientPacket_Global.cpp",
             "OnlyGlobal/Packet/ServerPacket_Global.cpp"]


def preprocess(legacy_dir: str, region: str, work: str, cpp: str) -> dict[str, str]:
    """Run the real preprocessor over every packet header and source."""
    stub = os.path.join(work, "stub")
    os.makedirs(os.path.join(stub, "Serializer"), exist_ok=True)
    with open(os.path.join(stub, "Serializer", "SerializeMacro.h"), "w") as fh:
        fh.write(STUB_SERIALIZE_MACRO)
    # Headers the .cpp files pull in that do not exist off-Windows and that
    # contribute no packet definitions.
    for name in ("winsock2.h", "windows.h", "KncUidType.h"):
        open(os.path.join(stub, name), "w").close()

    out: dict[str, str] = {}
    for rel in HEADERS + CPP_UNITS:
        src = os.path.join(legacy_dir, rel)
        dst = os.path.join(work, rel.replace("/", "_").replace(".", "_") + ".txt")
        cmd = [cpp, "-undef", "-nostdinc", "-P", "-Wno-endif-labels",
               "-I", stub, "-I", legacy_dir,
               "-I", os.path.join(legacy_dir, "OnlyGlobal", "Packet"),
               "-D", region, src, "-o", dst]
        res = subprocess.run(cmd, capture_output=True, text=True)
        if res.returncode != 0:
            sys.exit(f"preprocessing {src} failed:\n{res.stderr}")
        if "error:" in res.stderr:
            sys.exit(f"preprocessing {src} reported errors:\n{res.stderr}")
        out[rel] = dst
    return out


# ---------------------------------------------------------------------------
# Phase 2: parse
# ---------------------------------------------------------------------------

def _balanced(text: str, open_at: int) -> tuple[str, int]:
    """Return the text inside the braces starting at `open_at`, and the index
    of the closing brace."""
    depth = 0
    i = open_at
    while i < len(text):
        if text[i] == "{":
            depth += 1
        elif text[i] == "}":
            depth -= 1
            if depth == 0:
                return text[open_at + 1:i], i
        i += 1
    raise ValueError("unbalanced braces")


MEMBER_RE = re.compile(
    r"^([A-Za-z_][\w:<>,\s\*&]*?)\s+([A-Za-z_]\w*)\s*(\[\s*([^\]]+)\s*\])?$")

# `static const int EQUIPPED_SKILL_SLOT_COUNT = 4;` - struct-scoped constants
# used as fixed-array bounds. They survive preprocessing (they are C++, not
# macros), so the array extent has to be resolved from them.
CONST_RE = re.compile(
    r"^static\s+const\s+(?:int|unsigned|size_t|std::size_t|short|char|long)\s+"
    r"(\w+)\s*=\s*(\w+)$")


ENUM_RE = re.compile(r"\benum\b\s*(?:\w+\s*)?\{")


def parse_enum_constants(body: str) -> dict[str, int]:
    """Enumerators declared inside a struct body.

    Needed because fixed-array extents are often an enum's trailing
    "total" member, e.g. `unsigned long m_ulCash[GCT_TOTAL_NUM];` where
    GCT_TOTAL_NUM is the implicit 4 at the end of an enum. These are C++
    constants, not macros, so the preprocessor leaves them untouched.
    """
    out: dict[str, int] = {}
    for m in ENUM_RE.finditer(body):
        inner, _ = _balanced(body, m.end() - 1)
        nxt = 0
        for item in inner.split(","):
            item = item.strip()
            if not item:
                continue
            if "=" in item:
                name, _, val = item.partition("=")
                name, val = name.strip(), val.strip()
                try:
                    nxt = int(val, 0)
                except ValueError:
                    if val in out:
                        nxt = out[val]
                    else:
                        continue        # unresolvable; skip this enumerator
            else:
                name = item
            if re.fullmatch(r"\w+", name):
                out[name] = nxt
                nxt += 1
    return out


def parse_members(body: str):
    """Extract `type name;` / `type name[N];` declarations from a struct body.

    Returns (members, constants) where members maps name -> (legacy_type,
    array_extent_or_None).

    Brace-delimited blocks (constructors, operators, inline methods, nested
    enums) are replaced by a `;`, NOT deleted. Deleting them merges a method
    signature - which has no trailing semicolon of its own - with the member
    declaration that follows it, and that member is then silently lost
    because the merged text contains a `(`. That bug cost one real field
    (KEGS_END_GAME_DUNGEON_RESULT_DATA_NOT::m_mapHaveExpInDungeon, which sits
    directly after an inline GetOngoingQuestByUnitUID method).
    """
    depth = 0
    flat = []
    for ch in body:
        if ch == "{":
            depth += 1
            if depth == 1:
                flat.append(";")     # terminate whatever preceded the block
        elif ch == "}":
            depth -= 1
            if depth == 0:
                flat.append(";")     # and start a fresh statement after it
        elif depth == 0:
            flat.append(ch)

    members = collections.OrderedDict()
    constants: dict[str, int] = parse_enum_constants(body)
    for stmt in "".join(flat).split(";"):
        s = re.sub(r"\s+", " ", stmt.strip())
        if not s or s.startswith("@@CTOR@@"):
            continue
        c = CONST_RE.match(s)
        if c:
            try:
                constants[c.group(1)] = int(c.group(2), 0)
            except ValueError:
                pass
            continue
        if s.startswith("static") or s.startswith("enum") or s.startswith("typedef"):
            continue
        if "(" in s:            # a declaration with parens is a function
            continue
        m = MEMBER_RE.match(s)
        if m:
            extent = m.group(4)
            members[m.group(2)] = (m.group(1).strip(),
                                   extent.strip() if extent else None)
    return members, constants


def parse_structs(path: str) -> "collections.OrderedDict[str, str]":
    text = open(path, encoding="utf-8", errors="replace").read()
    out: "collections.OrderedDict[str, str]" = collections.OrderedDict()
    for m in re.finditer(r"@@S@@\s+(\w+)\s*\{", text):
        body, _ = _balanced(text, m.end() - 1)
        out[m.group(1)] = body
    return out


TYPEDEF_RE = re.compile(r"typedef\s+([\w:<>,\s]+?)\s+(\w+)\s*;")


def parse_typedefs(path: str) -> dict[str, str]:
    text = open(path, encoding="utf-8", errors="replace").read()
    out = {}
    for m in TYPEDEF_RE.finditer(text):
        out.setdefault(m.group(2), re.sub(r"\s+", " ", m.group(1).strip()))
    return out


def parse_serialize_bodies(path: str, kind: str) -> dict[str, list[str]]:
    """kind is 'PUT' or 'GET'. Returns packet -> ordered field expressions."""
    text = open(path, encoding="utf-8", errors="replace").read()
    tag = "P" if kind == "PUT" else "G"
    field_re = re.compile(r"@@%s@@(.*?)@@/%s@@" % (tag, tag), re.S)
    array_re = re.compile(r"@@%sA@@(.*?)@@,@@(.*?)@@/%sA@@" % (tag, tag), re.S)
    out: dict[str, list[str]] = {}
    for m in re.finditer(r"@@%sDEF@@\s*(\S+)\s*@@/%sDEF@@\s*\{" % (kind, kind), text):
        name = m.group(1)
        body, _ = _balanced(text, m.end() - 1)
        if array_re.search(body):
            sys.exit(f"{name}: PUTARRAY/GETARRAY is not supported by this "
                     f"generator and was believed unused; re-check {path}")
        out[name] = [f.strip() for f in field_re.findall(body)]
    return out


# ---------------------------------------------------------------------------
# Known legacy defects
# ---------------------------------------------------------------------------

# Two SERIALIZE_DEFINE_GET bodies in OnlyGlobal/Packet/ServerPacket_Global.cpp
# call PUT() where they meant GET(). Because PUT(x) expands to `ks.Put(obj.x)`
# and the GET body's `obj` is non-const, this compiles silently - and at run
# time the reader WRITES to the serializer instead of reading from it, so the
# affected fields are never pulled off the wire and the destination object
# keeps its default-constructed value.
#
# Both are server-internal packets (Giant billing, log-DB), never sent to a
# game client, and the rewrite is a single process with no legacy peer on the
# other end. We therefore take the PUT body as the authoritative wire format
# and generate a read_from that actually reads every field the writer wrote.
# This is a deliberate repair, recorded in docs/PROTOCOL.md.
#
# Anything NOT listed here that shows the same PUT/GET divergence is a hard
# error: a new region, or a mistake in this tool, must not slip through.
KNOWN_GET_BODY_DEFECTS = {
    "KEGIANT_BILLING_CONSUME_ACK":
        "GET body PUTs all 6 fields; the packet was effectively write-only.",
    "KELOG_ITEM_TRADE_ERROR_NOT":
        "GET body PUTs m_wsrtRegDate instead of reading it.",
}


# ---------------------------------------------------------------------------
# Phase 3: type mapping
# ---------------------------------------------------------------------------

# Legacy scalar -> (C++ type, Codec suffix). The Codec suffix is only used for
# documentation in the generated comment; dispatch is by C++ type.
SCALARS = {
    "char":           "char",
    "byte":           "std::uint8_t",
    "BYTE":           "std::uint8_t",
    "UCHAR":          "std::uint8_t",
    "u_char":         "std::uint8_t",
    "unsigned char":  "std::uint8_t",
    "UINT8":          "std::uint8_t",
    "bool":           "bool",
    "short":          "std::int16_t",
    "USHORT":         "std::uint16_t",
    "u_short":        "std::uint16_t",
    "unsigned short": "std::uint16_t",
    "int":            "std::int32_t",
    "INT32":          "std::int32_t",
    "UINT":           "std::uint32_t",
    "u_int":          "std::uint32_t",
    "unsigned int":   "std::uint32_t",
    "UINT32":         "std::uint32_t",
    "DWORD":          "std::uint32_t",
    # Win32 LP64-free: `long` is 32 bits in every build this protocol shipped
    # in. Mapping it to uint32_t keeps the wire width right on LP64 hosts.
    "unsigned long":  "std::uint32_t",
    "long":           "std::int32_t",
    "__int64":        "std::int64_t",
    "INT64":          "std::int64_t",
    "UidType":        "std::int64_t",
    "__time64_t":     "std::int64_t",
    "UINT64":         "std::uint64_t",
    "float":          "float",
    "double":         "double",
    "std::string":    "std::string",
    "std::wstring":   "std::u16string",
}

CONTAINER_RE = re.compile(r"^std::(vector|list|deque|set|multiset|map|multimap|pair)\s*<(.*)>$")


def split_template_args(s: str) -> list[str]:
    args, depth, cur = [], 0, []
    for ch in s:
        if ch == "<":
            depth += 1
        elif ch == ">":
            depth -= 1
        if ch == "," and depth == 0:
            args.append("".join(cur).strip())
            cur = []
        else:
            cur.append(ch)
    if cur:
        args.append("".join(cur).strip())
    return args


class TypeMapper:
    def __init__(self, struct_names: set[str], typedefs: dict[str, str]):
        self.structs = struct_names
        self.typedefs = typedefs
        self.unresolved: collections.Counter = collections.Counter()

    def resolve_alias(self, t: str, _depth: int = 0) -> str:
        """Follow typedefs that are not themselves generated structs."""
        if _depth > 16:
            return t
        if t in self.typedefs and t not in self.structs and t not in SCALARS:
            return self.resolve_alias(self.typedefs[t], _depth + 1)
        return t

    def map(self, t: str) -> str | None:
        t = re.sub(r"\s+", " ", t.strip())
        t = self.resolve_alias(t)
        if t in SCALARS:
            return SCALARS[t]
        if t in self.structs:
            return t
        m = CONTAINER_RE.match(t)
        if m:
            kind, inner = m.group(1), m.group(2)
            args = [self.map(a) for a in split_template_args(inner)]
            if any(a is None for a in args):
                return None
            return f"std::{kind}<{', '.join(args)}>"
        self.unresolved[t] += 1
        return None


# ---------------------------------------------------------------------------
# Phase 4: emit
# ---------------------------------------------------------------------------

BANNER = """\
// ---------------------------------------------------------------------------
// {name} - GENERATED FILE, DO NOT EDIT BY HAND.
//
// Produced by tools/gen_packets.py from the original KncWX2Server headers:
//     Common/{{Common,Client,Server}}Packet.{{h,cpp}}
// Region: {region}
//
// Field ORDER comes from the SERIALIZE_DEFINE_PUT bodies in the .cpp files -
// that is the wire order, and it is not always the member declaration order.
// Field TYPES come from the struct bodies in the .h files.
//
// Legacy names are preserved exactly (KEGS_CONNECT_ACK, m_iOK, ...) so that
// any struct here can be diffed against the 2014 source by name.
//
// Regenerate with:
//     python3 tools/gen_packets.py --legacy ../KncWX2Server/Common
//                                  --region {region}
//                                  --out src/proto/packets
// ---------------------------------------------------------------------------
"""


def topo_order(deps: dict[str, set[str]]) -> list[str]:
    """Kahn's algorithm; raises on a cycle."""
    indeg = {n: 0 for n in deps}
    for n, ds in deps.items():
        for d in ds:
            if d in indeg:
                indeg[n] += 0  # ensure key
    # count incoming edges (dependency -> dependent)
    incoming = {n: 0 for n in deps}
    for n, ds in deps.items():
        incoming[n] = len([d for d in ds if d in deps])
    ready = sorted([n for n, c in incoming.items() if c == 0])
    order: list[str] = []
    dependents: dict[str, list[str]] = collections.defaultdict(list)
    for n, ds in deps.items():
        for d in ds:
            if d in deps:
                dependents[d].append(n)
    while ready:
        n = ready.pop(0)
        order.append(n)
        for m in sorted(dependents[n]):
            incoming[m] -= 1
            if incoming[m] == 0:
                ready.append(m)
        ready.sort()
    if len(order) != len(deps):
        cyclic = sorted(set(deps) - set(order))
        raise ValueError(f"cycle in struct dependencies: {cyclic[:10]}")
    return order


def referenced_structs(cpp_type: str, struct_names: set[str]) -> set[str]:
    return {tok for tok in re.findall(r"\w+", cpp_type) if tok in struct_names}


def emit(records, aliases, order, region, out_dir, shards):
    os.makedirs(out_dir, exist_ok=True)
    per_shard = (len(order) + shards - 1) // shards
    written = []

    for idx in range(shards):
        chunk = order[idx * per_shard:(idx + 1) * per_shard]
        if not chunk:
            continue
        name = f"packets_{idx}.inc"
        with open(os.path.join(out_dir, name), "w", encoding="utf-8") as fh:
            fh.write(BANNER.format(name=name, region=region))
            fh.write("// clang-format off\n\n")
            for sname in chunk:
                fh.write(render_struct(sname, records[sname]["fields"]))
        written.append(name)

    with open(os.path.join(out_dir, "packets_aliases.inc"), "w",
              encoding="utf-8") as fh:
        fh.write(BANNER.format(name="packets_aliases.inc", region=region))
        fh.write("// Legacy `typedef KFoo KBar;` spellings. The client knows\n"
                 "// some packets only by the alias.\n\n")
        for alias, target in sorted(aliases.items()):
            fh.write(f"using {alias} = {target};\n")
    written.append("packets_aliases.inc")
    return written


def render_struct(sname: str, fields: list[dict]) -> str:
    """One struct + its write_to/read_from, in legacy wire order."""
    out = [f"struct {sname} {{\n"]

    # declarations: each array member is declared once, at the position of
    # its first element
    declared: set[str] = set()
    decls = []
    for f in fields:
        if f["name"] in declared:
            continue
        declared.add(f["name"])
        decls.append((f["cpp"], f["name"], f["legacy"]))
    width = max((len(c) for c, _, _ in decls), default=0) + 1
    for cpp_t, fname, legacy_t in decls:
        comment = "" if cpp_t == legacy_t else f"  // {legacy_t}"
        out.append(f"    {cpp_t.ljust(width)}{fname}{{}};{comment}\n")
    if not decls:
        out.append("    // no serialised fields\n")
    out.append("\n")

    def access(f, prefix=""):
        base = prefix + f["name"]
        return base if f["index"] is None else f"{base}[{f['index']}]"

    out.append("    void write_to(Writer& w) const {\n")
    if not fields:
        out.append("        (void)w;\n")
    for f in fields:
        a = access(f)
        out.append(f"        Codec<std::remove_cvref_t<decltype({a})>>::put(w, {a});\n")
    out.append("    }\n\n")

    out.append(f"    static {sname} read_from(Reader& r) {{\n")
    out.append(f"        {sname} v;\n")
    if not fields:
        out.append("        (void)r;\n")
    for f in fields:
        a = access(f, "v.")
        out.append(f"        {a} = Codec<std::remove_cvref_t<decltype({a})>>::get(r);\n")
    out.append("        return v;\n    }\n")
    out.append("};\n\n")
    return "".join(out)


# ---------------------------------------------------------------------------

def main() -> int:
    ap = argparse.ArgumentParser(description=__doc__,
                                 formatter_class=argparse.RawDescriptionHelpFormatter)
    ap.add_argument("--legacy", required=True, help="path to KncWX2Server/Common")
    ap.add_argument("--region", default="SERV_COUNTRY_TW")
    ap.add_argument("--out", required=True, help="output directory")
    ap.add_argument("--cpp", default="cpp", help="C preprocessor to invoke")
    ap.add_argument("--shards", type=int, default=8,
                    help="split the generated structs across N .inc files")
    ap.add_argument("--work", default=None, help="keep intermediates here")
    args = ap.parse_args()

    work = args.work or tempfile.mkdtemp(prefix="genpackets-")
    os.makedirs(work, exist_ok=True)

    print(f"-- preprocessing with region {args.region}")
    files = preprocess(args.legacy, args.region, work, args.cpp)

    # --- parse -------------------------------------------------------------
    structs: "collections.OrderedDict[str, str]" = collections.OrderedDict()
    typedefs: dict[str, str] = {}
    put: dict[str, list[str]] = {}
    get: dict[str, list[str]] = {}
    dupes: list[str] = []
    for h in HEADERS:
        for k, v in parse_structs(files[h]).items():
            structs.setdefault(k, v)
        for k, v in parse_typedefs(files[h]).items():
            typedefs.setdefault(k, v)
    for c in CPP_UNITS:
        for kind, table in (("PUT", put), ("GET", get)):
            for k, v in parse_serialize_bodies(files[c], kind).items():
                if k in table and table[k] != v:
                    dupes.append(f"{k}: conflicting {kind} bodies "
                                 f"(second one in {c})")
                table.setdefault(k, v)

    print(f"-- parsed {len(structs)} structs, {len(typedefs)} typedefs, "
          f"{len(put)} PUT bodies, {len(get)} GET bodies")

    # --- self-checks -------------------------------------------------------
    problems: list[str] = list(dupes)
    if set(put) != set(get):
        problems.append(f"PUT/GET set mismatch: "
                        f"PUT-only={sorted(set(put)-set(get))[:5]} "
                        f"GET-only={sorted(set(get)-set(put))[:5]}")
    repaired: list[str] = []
    for name in sorted(set(put) & set(get)):
        if put[name] == get[name]:
            continue
        if name in KNOWN_GET_BODY_DEFECTS:
            # the GET body is known-broken; PUT is the wire truth
            if get[name] != put[name][:len(get[name])]:
                problems.append(
                    f"{name}: known-defective GET body no longer looks the "
                    f"way it did when it was triaged\n"
                    f"    PUT {put[name]}\n    GET {get[name]}")
            else:
                repaired.append(name)
            continue
        problems.append(f"{name}: PUT order != GET order\n"
                        f"    PUT {put[name]}\n    GET {get[name]}")

    stale = sorted(set(KNOWN_GET_BODY_DEFECTS) - set(repaired))
    if stale:
        problems.append(f"KNOWN_GET_BODY_DEFECTS lists packets that no longer "
                        f"diverge (or no longer exist): {stale}")

    parsed = {n: parse_members(b) for n, b in structs.items()}
    members = {n: v[0] for n, v in parsed.items()}
    constants = {n: v[1] for n, v in parsed.items()}
    mapper = TypeMapper(set(structs), typedefs)

    records: dict[str, dict] = {}
    for name in sorted(put):
        mem = members.get(name)
        if mem is None:
            problems.append(f"{name}: has PUT/GET but no struct body")
            continue
        consts = constants.get(name, {})
        fields: list[dict] = []
        seen_arrays: dict[str, list[int]] = collections.OrderedDict()
        for expr in put[name]:
            idx_m = re.match(r"^(\w+)\s*\[\s*(\d+)\s*\]$", expr)
            base = idx_m.group(1) if idx_m else expr
            if base not in mem:
                problems.append(f"{name}: PUT({expr}) has no matching member")
                continue
            legacy_t, extent = mem[base]
            cpp_elem = mapper.map(legacy_t)
            if cpp_elem is None:
                problems.append(f"{name}.{base}: cannot map type {legacy_t!r}")
                continue

            if extent is None:
                if idx_m:
                    problems.append(f"{name}.{base}: PUT is indexed but the "
                                    f"member is not an array")
                    continue
                fields.append({"name": base, "cpp": cpp_elem,
                               "legacy": legacy_t, "index": None})
                continue

            # fixed C array: the legacy code serialises it element by element
            if not idx_m:
                problems.append(f"{name}.{base}: array member serialised "
                                f"without an index")
                continue
            n = consts.get(extent)
            if n is None:
                try:
                    n = int(extent, 0)
                except ValueError:
                    problems.append(f"{name}.{base}: cannot resolve array "
                                    f"extent {extent!r}")
                    continue
            arr_cpp = f"std::array<{cpp_elem}, {n}>"
            seen_arrays.setdefault(base, []).append(int(idx_m.group(2)))
            fields.append({"name": base, "cpp": arr_cpp,
                           "legacy": f"{legacy_t}[{extent}]",
                           "index": int(idx_m.group(2)), "extent": n})

        # An array whose PUT list is not exactly 0..N-1 would mean the wire
        # carries a subset or a permutation. Flag it rather than guess.
        for base, idxs in seen_arrays.items():
            n = next(f["extent"] for f in fields if f["name"] == base)
            if idxs != list(range(n)):
                problems.append(f"{name}.{base}: array of {n} is serialised "
                                f"as indices {idxs}, not 0..{n-1}")

        records[name] = {"fields": fields}

    # structs referenced as field types must also be emitted
    needed = set(records)
    frontier = list(needed)
    while frontier:
        n = frontier.pop()
        for f in records.get(n, {}).get("fields", []):
            for ref in referenced_structs(f["cpp"], set(structs)):
                if ref not in needed:
                    if ref not in records:
                        # a plain data struct with no PUT/GET of its own
                        problems.append(
                            f"{n} references {ref}, which has no "
                            f"SERIALIZE_DEFINE_PUT - cannot serialise it")
                    needed.add(ref)
                    frontier.append(ref)

    if mapper.unresolved:
        for t, c in mapper.unresolved.most_common():
            problems.append(f"unresolved type {t!r} ({c} use(s))")

    if problems:
        print(f"\n!! {len(problems)} problem(s):", file=sys.stderr)
        for p in problems[:40]:
            print("   " + p, file=sys.stderr)
        if len(problems) > 40:
            print(f"   ... and {len(problems)-40} more", file=sys.stderr)
        return 1

    # --- dependency order --------------------------------------------------
    deps = {n: set() for n in records}
    for n, rec in records.items():
        for f in rec["fields"]:
            deps[n] |= referenced_structs(f["cpp"], set(records)) - {n}
    order = topo_order(deps)

    # --- aliases -----------------------------------------------------------
    aliases = {a: t for a, t in typedefs.items()
               if t in records and a not in records and a.startswith("K")}

    written = emit(records, aliases, order, args.region, args.out, args.shards)

    total_fields = sum(len(r["fields"]) for r in records.values())
    print(f"-- emitted {len(records)} structs, {total_fields} fields, "
          f"{len(aliases)} aliases")
    for name in repaired:
        print(f"   ! repaired legacy GET body: {name}"
              f" - {KNOWN_GET_BODY_DEFECTS[name]}")
    for w in written:
        print(f"     {w}")
    print("-- all self-checks passed")
    return 0


if __name__ == "__main__":
    sys.exit(main())
