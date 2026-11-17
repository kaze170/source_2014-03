#!/usr/bin/env python3
"""
gen_event_ids.py - Generate the KncWX2ServerNG event-ID table from the ORIGINAL
KncWX2Server headers, so that the new server's numbering is bit-identical to the
2014 client's.

Why generate instead of hand-copy?
---------------------------------
The original numbering comes from three X-macro headers expanded into three
consecutive C enums in Common/Event.h:

    enum ENUM_SYSTEM_EVENT_ID { #include "EventID_System.h" };
    enum ENUM_CLIENT_EVENT_ID { E_CLIENT_EVENT_ID_BEGIN = E_SYSTEM_EVENT_ID_END,
                                #include "EventID_Client.h" };
    enum ENUM_SERVER_EVENT_ID { E_SERVER_EVENT_ID_BEGIN = EGS_CLIENT_EVENT_ID_END,
                                #include "EventID_Server.h" };

and the human-readable table KEvent::ms_szEventID[] is the *concatenation* of the
same three headers.  A single inserted or dropped line shifts every later ID and
silently breaks the client.  Generating removes that whole class of error.

Verified invariants (asserted below, and re-checked by tests/test_event_ids.cpp):
  * E_HEART_BEAT            == 0
  * E_ACCEPT_CONNECTION_NOT == 1
  * E_SYSTEM_EVENT_ID_END   == 20
  * EGS_GOOD_JOB_1_REQ      == 21
  * EGS_CLIENT_EVENT_ID_END == 1324
  * EVENT_EVENTID_SENTINEL  == 3385
  * ms_szEventID[] length   == 3386 and index == enum value for every entry

Usage:
    python3 tools/gen_event_ids.py --legacy <path-to>/KncWX2Server/Common \
                                   --out    src/proto/event_ids.inc
"""

from __future__ import annotations

import argparse
import os
import re
import sys

# _ENUM( NAME )  or  _ENUM( NAME = 0 )
_ENUM_RE = re.compile(r"_ENUM\s*\(\s*([A-Za-z_]\w*)\s*(?:=\s*([^)]+?))?\s*\)")
_INC_RE = re.compile(r'^\s*#\s*include\s+"([^"]+)"')


def _strip_comments(text: str) -> str:
    """Remove /* */ and // comments while preserving line structure."""
    text = re.sub(r"/\*.*?\*/", "", text, flags=re.S)
    out = []
    for line in text.split("\n"):
        idx = line.find("//")
        if idx >= 0:
            line = line[:idx]
        out.append(line)
    return "\n".join(out)


def _resolve_case_insensitive(path: str) -> str:
    """The legacy tree was authored on Windows; some #includes differ in case."""
    if os.path.exists(path):
        return path
    directory, base = os.path.dirname(path), os.path.basename(path)
    if os.path.isdir(directory):
        for candidate in os.listdir(directory):
            if candidate.lower() == base.lower():
                return os.path.join(directory, candidate)
    raise FileNotFoundError(path)


def _collect(path: str, acc: list[tuple[str, str | None]]) -> None:
    """Append (name, explicit_value_or_None) for every _ENUM in `path`, recursing
    into #include directives exactly the way the C preprocessor would."""
    path = _resolve_case_insensitive(path)
    # The 2014 sources are Korean CP949; decode leniently - we only need ASCII
    # identifiers, comments may contain anything.
    with open(path, "rb") as handle:
        raw = handle.read().decode("cp949", errors="replace")

    for line in _strip_comments(raw).split("\n"):
        stripped = line.strip()
        if stripped.startswith("#"):
            # Never harvest _ENUM from a preprocessor line: `#define _ENUM(VAL) VAL,`
            # inside the `#ifdef __NEVER_DEFINED_MACRO__` guard would otherwise
            # be mistaken for a real entry.
            match = _INC_RE.match(line)
            if match:
                _collect(
                    os.path.normpath(
                        os.path.join(os.path.dirname(path), match.group(1))
                    ),
                    acc,
                )
            continue
        for match in _ENUM_RE.finditer(line):
            value = match.group(2).strip() if match.group(2) else None
            acc.append((match.group(1), value))


def build_table(common_dir: str) -> list[tuple[str, int]]:
    """Return [(NAME, value)] in ms_szEventID[] order, i.e. index == value."""
    ordered: list[tuple[str, int]] = []
    named: dict[str, int] = {}

    def run(header: str, start: int) -> int:
        nonlocal ordered, named
        acc: list[tuple[str, str | None]] = []
        _collect(os.path.join(common_dir, header), acc)
        value = start
        for name, explicit in acc:
            if explicit is not None:
                value = int(explicit, 0)
            named[name] = value
            ordered.append((name, value))
            value += 1
        return value

    run("EventID_System.h", 0)
    # E_CLIENT_EVENT_ID_BEGIN = E_SYSTEM_EVENT_ID_END, so the first client
    # enumerator is E_SYSTEM_EVENT_ID_END + 1.  E_CLIENT_EVENT_ID_BEGIN itself is
    # declared in Event.h, NOT in the header, so it contributes no ms_szEventID[]
    # slot - which is exactly why index still equals value.
    run("EventID_Client.h", named["E_SYSTEM_EVENT_ID_END"] + 1)
    run("EventID_Server.h", named["EGS_CLIENT_EVENT_ID_END"] + 1)
    return ordered


_EXPECTED = {
    "E_HEART_BEAT": 0,
    "E_ACCEPT_CONNECTION_NOT": 1,
    "E_CONNECTION_LOST_NOT": 2,
    "E_UDP_PORT_NOT": 3,
    "E_RESERVE_DESTROY": 6,
    "E_SYSTEM_EVENT_ID_END": 20,
    "EGS_GOOD_JOB_1_REQ": 21,
    "EGS_CONNECT_REQ": 26,
    "EGS_CONNECT_ACK": 27,
    "EGS_NEW_USER_JOIN_REQ": 28,
    "EGS_VERIFY_ACCOUNT_REQ": 30,
    "EGS_VERIFY_ACCOUNT_ACK": 31,
    "EGS_CLIENT_EVENT_ID_END": 1324,
    "EVENT_EVENTID_SENTINEL": 3385,
}


def verify(table: list[tuple[str, int]]) -> None:
    index_ok = all(i == value for i, (_, value) in enumerate(table))
    if not index_ok:
        for i, (name, value) in enumerate(table):
            if i != value:
                raise SystemExit(
                    f"FATAL: ms_szEventID[] index {i} != enum value {value} ({name})"
                )
    lookup = dict(table)
    for name, expected in _EXPECTED.items():
        actual = lookup.get(name)
        if actual != expected:
            raise SystemExit(
                f"FATAL: {name} == {actual}, expected {expected}. "
                "The legacy headers changed - review before regenerating."
            )
    if len(table) != 3386:
        raise SystemExit(f"FATAL: expected 3386 ids, got {len(table)}")


_HEADER = """\
// ---------------------------------------------------------------------------
// event_ids.inc - GENERATED FILE, DO NOT EDIT BY HAND.
//
// Produced by tools/gen_event_ids.py from the original KncWX2Server headers
//   Common/EventID_System.h  (+ OnlyGlobal/EventID_System_Global.h)
//   Common/EventID_Client.h  (+ OnlyGlobal/EventID_Client_Global.h)
//   Common/EventID_Server.h  (+ OnlyGlobal/EventID_Server_Global.h)
//
// The X-macro below is expanded in several places; the Nth entry has event id N
// which is exactly what the 2014 client expects on the wire.
//
// Entries: {count}   (0 .. {last} inclusive, EVENT_EVENTID_SENTINEL == {last})
//
// None of the three legacy headers contains conditional compilation around an
// _ENUM entry, so this numbering is identical for every regional build
// (KR / TW-HK / JP / EU / US / CN / ...). Confirmed by inspection and by
// tests/test_event_ids.cpp.
// ---------------------------------------------------------------------------
// clang-format off

#ifndef KNC_EVENT_ID
#  error "define KNC_EVENT_ID(name, value) before including event_ids.inc"
#endif

"""


def main(argv: list[str]) -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument(
        "--legacy",
        required=True,
        help="path to the original KncWX2Server/Common directory",
    )
    parser.add_argument("--out", required=True, help="output .inc path")
    args = parser.parse_args(argv)

    table = build_table(args.legacy)
    verify(table)

    lines = [_HEADER.format(count=len(table), last=len(table) - 1)]
    for name, value in table:
        lines.append(f"KNC_EVENT_ID({name}, {value})\n")
    lines.append("\n#undef KNC_EVENT_ID\n// clang-format on\n")

    os.makedirs(os.path.dirname(os.path.abspath(args.out)), exist_ok=True)
    with open(args.out, "w", encoding="utf-8", newline="\n") as handle:
        handle.writelines(lines)

    print(f"wrote {args.out}: {len(table)} event ids (0..{len(table) - 1})")
    return 0


if __name__ == "__main__":
    raise SystemExit(main(sys.argv[1:]))
