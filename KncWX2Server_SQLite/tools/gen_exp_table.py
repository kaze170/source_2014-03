#!/usr/bin/env python3
"""Embeds the 2014 level curve into src/kgame/ExpTableData.gen.cpp.

The authoritative source is ScriptData/ExpTable.xls (the spreadsheet the original DBManager
generated ExpTable.lua from). It is a BIFF8 workbook inside an OLE2 container, so this script
carries a small reader instead of depending on xlrd/openpyxl.

Usage:  python3 tools/gen_exp_table.py [path/to/ExpTable.xls]
"""
from __future__ import annotations

import struct
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parent.parent
DEFAULT_XLS = ROOT.parent / "ScriptData" / "ExpTable.xls"
OUT_CPP = ROOT / "src" / "kgame" / "ExpTableData.gen.cpp"

FREE = 0xFFFFFFFF
ENDOFCHAIN = 0xFFFFFFFE


# --------------------------------------------------------------------------- OLE2 container
class Ole2:
    def __init__(self, blob: bytes) -> None:
        if blob[:8] != b"\xd0\xcf\x11\xe0\xa1\xb1\x1a\xe1":
            raise ValueError("not an OLE2 compound document")
        self.blob = blob
        (sector_shift,) = struct.unpack_from("<H", blob, 30)
        self.sector_size = 1 << sector_shift
        (num_fat, dir_start) = struct.unpack_from("<II", blob, 44)
        (self.mini_cutoff,) = struct.unpack_from("<I", blob, 56)
        mini_fat_start, num_mini_fat, difat_start, num_difat = struct.unpack_from("<IIII", blob, 60)

        difat = list(struct.unpack_from("<109I", blob, 76))
        sector = difat_start
        for _ in range(num_difat):
            values = self._sector(sector)
            entries = struct.unpack("<%dI" % (self.sector_size // 4), values)
            difat.extend(entries[:-1])
            sector = entries[-1]

        fat: list[int] = []
        for fat_sector in difat[:num_fat]:
            fat.extend(struct.unpack("<%dI" % (self.sector_size // 4), self._sector(fat_sector)))
        self.fat = fat

        directory = self._read_chain(dir_start)
        self.entries = []
        for offset in range(0, len(directory), 128):
            entry = directory[offset : offset + 128]
            name = entry[:64].decode("utf-16-le").split("\x00")[0]
            kind = entry[66]
            start, size = struct.unpack_from("<II", entry, 116)
            self.entries.append((name, kind, start, size))

        self.ministream = self._read_chain(self.entries[0][2])
        self.mini_fat: list[int] = []
        if num_mini_fat:
            raw = self._read_chain(mini_fat_start)
            self.mini_fat = list(struct.unpack("<%dI" % (len(raw) // 4), raw))

    def _sector(self, index: int) -> bytes:
        offset = (index + 1) * self.sector_size
        return self.blob[offset : offset + self.sector_size]

    def _chain(self, start: int) -> list[int]:
        chain = []
        sector = start
        while sector < ENDOFCHAIN:
            chain.append(sector)
            sector = self.fat[sector]
        return chain

    def _read_chain(self, start: int) -> bytes:
        return b"".join(self._sector(s) for s in self._chain(start))

    def _read_mini(self, start: int, size: int) -> bytes:
        data = []
        sector = start
        while sector < ENDOFCHAIN:
            data.append(self.ministream[sector * 64 : (sector + 1) * 64])
            sector = self.mini_fat[sector]
        return b"".join(data)[:size]

    def stream(self, name: str) -> bytes:
        for entry_name, kind, start, size in self.entries:
            if kind == 2 and entry_name == name:
                if size < self.mini_cutoff:
                    return self._read_mini(start, size)
                return self._read_chain(start)[:size]
        raise KeyError(name)


# --------------------------------------------------------------------------- BIFF8 records
def _rk_value(rk: int) -> float:
    if rk & 0x02:
        value = float(rk >> 2)
    else:
        value = struct.unpack("<d", struct.pack("<Q", (rk & 0xFFFFFFFC) << 32))[0]
    return value / 100.0 if rk & 0x01 else value


def read_first_sheet(workbook: bytes) -> dict[tuple[int, int], float]:
    boundsheets = []
    position = 0
    while position + 4 <= len(workbook):
        record, length = struct.unpack_from("<HH", workbook, position)
        body = workbook[position + 4 : position + 4 + length]
        position += 4 + length
        if record == 0x0085:  # BOUNDSHEET
            offset = struct.unpack_from("<I", body, 0)[0]
            cch = body[6]
            flags = body[7]
            name = (
                body[8 : 8 + cch * 2].decode("utf-16-le")
                if flags & 0x01
                else body[8 : 8 + cch].decode("cp1252", "replace")
            )
            boundsheets.append((name, offset))
    if not boundsheets:
        raise ValueError("workbook has no sheets")

    start = boundsheets[0][1]
    cells: dict[tuple[int, int], float] = {}
    position = start
    while position + 4 <= len(workbook):
        record, length = struct.unpack_from("<HH", workbook, position)
        body = workbook[position + 4 : position + 4 + length]
        position += 4 + length
        if record == 0x000A:  # EOF
            break
        if record == 0x0203:  # NUMBER
            row, col = struct.unpack_from("<HH", body, 0)
            cells[(row, col)] = struct.unpack_from("<d", body, 6)[0]
        elif record == 0x027E:  # RK
            row, col = struct.unpack_from("<HH", body, 0)
            cells[(row, col)] = _rk_value(struct.unpack_from("<I", body, 6)[0])
        elif record == 0x00BD:  # MULRK
            row, first_col = struct.unpack_from("<HH", body, 0)
            count = (length - 6) // 6
            for index in range(count):
                rk = struct.unpack_from("<I", body, 4 + index * 6 + 2)[0]
                cells[(row, first_col + index)] = _rk_value(rk)
    return cells


# --------------------------------------------------------------------------- main
def build_rows(xls_path: Path) -> list[tuple[int, int, int]]:
    book = Ole2(xls_path.read_bytes())
    cells = read_first_sheet(book.stream("Workbook"))
    if not cells:
        raise ValueError("no cells found")
    last_row = max(row for row, _ in cells)

    raw: list[tuple[int, int]] = []
    for row in range(1, last_row + 1):
        level = cells.get((row, 0))
        need = cells.get((row, 1), 0.0)
        if level is None:
            continue
        level = int(level)
        need = int(round(need))
        if need < 0:
            raise ValueError(f"negative exp requirement at level {level}")
        raw.append((level, need))
    if not raw:
        raise ValueError("empty exp table")

    # Column 1 is the *cumulative* exp at which the unit reaches that level (level 2 -> 600,
    # level 3 -> 1300, level 80 -> 91056800). KExpTable::CheckLevelUp compared the total exp against
    # exactly these values, so expToReach is kept as published and expToNext is derived.
    rows: list[tuple[int, int, int]] = []
    for index, (level, reach) in enumerate(raw):
        nxt = raw[index + 1][1] if index + 1 < len(raw) else reach
        rows.append((level, reach, max(0, nxt - reach)))
    return rows


def main(argv: list[str]) -> int:
    xls_path = Path(argv[1]) if len(argv) > 1 else DEFAULT_XLS
    rows = build_rows(xls_path)
    OUT_CPP.parent.mkdir(parents=True, exist_ok=True)

    lines = [
        "// src/kgame/ExpTableData.gen.cpp - generated by tools/gen_exp_table.py; do not edit.",
        "//",
        f"// Source: {xls_path.name} (BIFF8 sheet 1: level / cumulative exp to reach the level).",
        "//",
        '// The original server loaded this through KExpTable::AddExpTable_LUA from ExpTable.lua, which',
        "// DBManager wrote out of the same spreadsheet. The array is compiled in so the server starts",
        "// without a resource directory.",
        '#include "kgame/ExpTable.h"',
        "",
        "namespace knc::game {",
        "namespace {",
        "",
        f"constexpr ExpEntry kExpTable[{len(rows)}] = {{",
    ]
    for level, reach, to_next in rows:
        lines.append(f"    {{{level}, {reach}LL, {to_next}LL}},")
    lines += [
        "};",
        "",
        "}  // namespace",
        "",
        "const ExpEntry* expTableData() { return kExpTable; }",
        f"std::size_t expTableSize() {{ return {len(rows)}; }}",
        "",
        "}  // namespace knc::game",
        "",
    ]
    OUT_CPP.write_text("\n".join(lines), encoding="utf-8")
    print(f"{OUT_CPP.relative_to(ROOT)}: {len(rows)} levels, "
          f"max {rows[-1][0]} (reached at {rows[-1][1]} exp)")
    return 0


if __name__ == "__main__":
    raise SystemExit(main(sys.argv))
