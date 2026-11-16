#!/usr/bin/env python3
"""Check knc::server::NetError against the 2014 Common/NetError_def.h.

The rewrite hands the client a bare integer, so the *number* matters as much as the packet
layout: "password wrong" is 12 because ERR_VERIFY_06 happens to sit at index 12 of a plain C
enum. Nothing in the code expresses that arithmetic, so it is re-derived here from the original
header and compared with every value in include/kserver/LoginService.h.

Two things are checked:

  * every `kErrX = N,  // ERR_Y` line in the rewrite agrees with the value ERR_Y has in the
    original enumeration (the header is a flat enum: NET_OK = 0 and each entry advances by one,
    with explicit `= n` assignments where the 2014 authors skipped a name);
  * the comment names a real 2014 error, so a typo cannot silently point at some other number.

Usage:
  verify_neterrors.py [-v] [--self-test]
"""

from __future__ import annotations

import argparse
import re
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parent.parent
ORIGINALS = [
    ROOT.parent / "KncWX2Server" / "Common" / "NetError_def.h",
]
REGIONAL_DIR = ROOT.parent / "KncWX2Server" / "Common" / "Neterror"
REWRITE = ROOT / "include" / "kserver" / "LoginService.h"

# `kErrVerifyBlocked = 15,            // ERR_VERIFY_11`
REWRITE_RE = re.compile(
    r"^\s*(k\w+)\s*=\s*(-?\d+)\s*,\s*//\s*([A-Z][A-Z0-9_]*)\s*$"
)


def decode_header(path: Path) -> str:
    raw = path.read_bytes()
    for encoding in ("utf-8-sig", "cp949"):
        try:
            return raw.decode(encoding)
        except UnicodeDecodeError:
            continue
    return raw.decode("utf-8", errors="replace")


def original_table(path: Path) -> dict[str, int]:
    """Re-evaluate the NetErrorEnum enumeration from an original header.

    Raises ValueError when the file is not an enumeration (the regional directory also holds
    `szErrorStrXX[]` string tables, one per language, which share the file name pattern).
    """
    text = decode_header(path)
    if "enum NetErrorEnum" not in text:
        raise ValueError("not an enumeration")
    body = text[text.index("enum NetErrorEnum") :]
    body = body[body.index("{") + 1 :]
    body = body[: body.index("}")]

    table: dict[str, int] = {}
    value = 0
    for line in body.splitlines():
        line = line.split("//")[0].strip()
        if not line:
            continue
        match = re.match(r"([A-Z][A-Z0-9_]*)\s*(?:=\s*(-?\d+))?\s*,?", line)
        if not match:
            continue
        name, explicit = match.group(1), match.group(2)
        if explicit is not None:
            value = int(explicit)
        table[name] = value
        value += 1
    return table


def rewrite_table(path: Path = REWRITE) -> list[tuple[str, int, str]]:
    """Every `kErrX = N, // ERR_Y` line in the rewrite, in file order."""
    rows: list[tuple[str, int, str]] = []
    for line in path.read_text(encoding="utf-8").splitlines():
        match = REWRITE_RE.match(line)
        if match:
            rows.append((match.group(1), int(match.group(2)), match.group(3)))
    return rows


def variant_tables() -> list[tuple[str, dict[str, int]]]:
    """Every regional enumeration that actually parses, keyed by file name."""
    tables: list[tuple[str, dict[str, int]]] = []
    if not REGIONAL_DIR.is_dir():
        return tables
    for path in sorted(REGIONAL_DIR.glob("NetError_def*.h")):
        try:
            tables.append((path.name, original_table(path)))
        except (ValueError, IndexError):
            continue  # an error *string* table, not an enumeration
    return tables


def check_variants(path: Path = REWRITE) -> list[str]:
    """The codes the rewrite uses must mean the same thing in every regional build.

    The rewrite models one deployment, but SERV_SEPARATE_NET_ERROR renumbers the enum per region -
    so a code that drifted would silently mean "blocked" here and something else in another build.
    All the variants agree on the codes in use today; this keeps that true.
    """
    base = original_table(ORIGINALS[0])
    regions = variant_tables()
    problems: list[str] = []
    if not regions:
        return problems
    for name, value, comment in rewrite_table(path):
        if comment not in base:
            continue
        for variant_name, variant in regions:
            found = variant.get(comment)
            if found is not None and found != base[comment]:
                problems.append(
                    f"{comment} is {base[comment]} in Common/NetError_def.h but {found} in "
                    f"{variant_name} ({name} would be wrong for that build)"
                )
    return problems


def check(path: Path = REWRITE, verbose: bool = False) -> list[str]:
    original = original_table(ORIGINALS[0])
    rows = rewrite_table(path)
    problems: list[str] = []

    if not original:
        return [f"could not read any name out of {ORIGINALS[0]}"]
    if not rows:
        return [f"could not read any kErr value out of {path}"]

    for name, value, comment in rows:
        if comment not in original:
            problems.append(f"{name}: {comment} is not an error in the 2014 header")
            continue
        expected = original[comment]
        if value != expected:
            problems.append(f"{name}: {value} but {comment} is {expected} in the 2014 header")
        elif verbose:
            print(f"  {name:34s} {value:5d}  == {comment}")

    return problems


def self_test() -> int:
    """Prove the comparator can fail, by doctoring a copy of the rewrite."""
    import tempfile

    good = rewrite_table()
    if not good:
        print("self test: no rows to work with")
        return 1

    name, value, comment = good[0]
    mutated = REWRITE.read_text(encoding="utf-8").replace(
        f"{name} = {value},", f"{name} = {value + 1},", 1
    )
    with tempfile.NamedTemporaryFile("w", suffix=".h", delete=False, encoding="utf-8") as handle:
        handle.write(mutated)
        path = Path(handle.name)
    try:
        caught = check(path)
    finally:
        path.unlink()

    if not caught:
        print("self test: a shifted error code was NOT caught")
        return 1

    # and a name that does not exist in the original must also be rejected
    bogus = REWRITE.read_text(encoding="utf-8").replace(
        f"// {comment}", "// ERR_NOT_A_REAL_ERROR", 1
    )
    with tempfile.NamedTemporaryFile("w", suffix=".h", delete=False, encoding="utf-8") as handle:
        handle.write(bogus)
        path = Path(handle.name)
    try:
        caught_name = check(path)
    finally:
        path.unlink()

    if not caught_name:
        print("self test: an unknown error name was NOT caught")
        return 1

    print(
        f"self test: caught a shifted value ({caught[0]}) and an unknown name "
        f"({caught_name[0]})"
    )
    return 0


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("-v", "--verbose", action="store_true")
    parser.add_argument("--self-test", action="store_true")
    args = parser.parse_args()

    if args.self_test:
        return self_test()

    problems = check(verbose=args.verbose)
    problems += check_variants()
    if problems:
        for problem in problems:
            print(f"FAIL {problem}", file=sys.stderr)
        return 1

    rows = rewrite_table()
    names = len(original_table(ORIGINALS[0]))
    variants = variant_tables()
    print(
        f"{len(rows)} error code(s) match the 2014 NetErrorEnum "
        f"({names} names in the enumeration, and the same values in all "
        f"{len(variants)} regional enumerations)"
    )
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
