#!/usr/bin/env python3
"""Generate kproto/FeatureFlags.gen.h from the original ServerDefine.h tree.

ServerDefine.h is a switchboard of feature macros that select protocol variants inside the shared
packet headers (ClientPacket.h, CommonPacket.h, ServerPacket.h). Getting the set wrong silently
changes packet layouts - a missed `#undef SERV_...` alone can shift every field after it - so this
runs the file the way the preprocessor would:

  * `#define X` sets X, `#undef X` clears it again (62 of those, mostly in the OnlyGlobal files),
  * `//#define X` and code inside `/* ... */` are the *disabled* variants,
  * `#ifdef` / `#ifndef` / `#if defined(..)` / `#elif` / `#else` / `#endif` are evaluated against the
    set as it stands at that point, in file order,
  * `#include "..."` is followed, so the OnlyGlobal/ServerDefine/*.h files are part of the answer,
  * finally the packet headers next to ServerDefine.h are scanned, because they guard *fields* with
    macros the define tree never mentions (`#ifdef SERV_GOLD_TICKET` inside KItemInfo is the case
    that matters). Those are recorded as false and listed separately, so "off" and "unknown" never
    look the same.

The tree as shipped selects no region: the `#define SERV_COUNTRY_*` block sits inside
`#ifdef SERV_INT_INTERNAL`, which nothing defines, and no build configuration passes one on the
command line. `SERV_GLOBAL_COMMON` *is* defined, so ServerDefine_Global.h always participates.

Anything the evaluator cannot decide is reported and the run fails, rather than guessing.

Usage: python3 tools/gen_featureflags.py <KncWX2Server/Common/ServerDefine.h> <out header>
"""
import os
import re
import sys

DEFINE_RE = re.compile(r'^[ \t]*#[ \t]*define[ \t]+([A-Za-z_][A-Za-z0-9_]*)')
UNDEF_RE = re.compile(r'^[ \t]*#[ \t]*undef[ \t]+([A-Za-z_][A-Za-z0-9_]*)')
DISABLED_RE = re.compile(r'^[ \t]*//[ \t]*#[ \t]*define[ \t]+([A-Za-z_][A-Za-z0-9_]*)')
INCLUDE_RE = re.compile(r'^[ \t]*#[ \t]*include[ \t]+"([^"]+)"')
COND_RE = re.compile(r'^[ \t]*#[ \t]*(ifdef|ifndef|if|elif|else|endif)\b(.*)$')


class Preprocessor:
    def __init__(self):
        self.state = {}        # macro -> True/False as it currently stands
        self.order = []        # first-appearance order of every macro we saw
        self.referenced = {}   # macros tested by a packet header but never defined by the tree
        self.unknown = []      # conditionals that could not be evaluated

    # -- bookkeeping ---------------------------------------------------------
    def _touch(self, name):
        if name not in self.order:
            self.order.append(name)

    def define(self, name):
        self._touch(name)
        self.state[name] = True

    def undef(self, name):
        self._touch(name)
        self.state[name] = False

    def disabled_only(self, name):
        self._touch(name)
        self.state.setdefault(name, False)

    def reference(self, name):
        """Records a flag that a packet header *tests* but ServerDefine.h never mentions.

        `#ifdef SERV_GOLD_TICKET` guards a field of KItemInfo, but the macro is not defined (or even
        named) anywhere in the 2014 define tree, so a pass over ServerDefine.h alone never sees it.
        Recording it as `false` makes every tested-but-undefined flag explicit in the generated
        header, which is what the static_asserts in kproto depend on.
        """
        self.referenced.setdefault(name, False)

    def enabled(self, name):
        return self.state.get(name, False)

    # -- expressions --------------------------------------------------------
    def evaluate(self, expr):
        expr = expr.strip()
        if not expr:
            return None
        for op in ('||', '&&'):
            if op in expr:
                left, _, right = expr.partition(op)
                a, b = self.evaluate(left), self.evaluate(right)
                if a is None or b is None:
                    return None
                return (a or b) if op == '||' else (a and b)
        if expr.startswith('!'):
            inner = self.evaluate(expr[1:])
            return None if inner is None else not inner
        m = re.match(r'defined\s*\(?\s*([A-Za-z_]\w*)\s*\)?$', expr)
        if m:
            return self.enabled(m.group(1))
        if expr in ('0', '1'):
            return expr == '1'
        if re.fullmatch(r'[A-Za-z_]\w*', expr):
            return self.enabled(expr)
        return None

    # -- driver -------------------------------------------------------------
    def process(self, path, depth=0):
        if depth > 8:
            return
        raw = open(path, encoding='cp949', errors='replace').read()
        lines = raw.replace('\r\n', '\n').replace('\r', '\n').split('\n')
        stack = []  # [active_now, any_branch_taken]
        in_block = False

        for line in lines:
            stripped = line.strip()

            if in_block:
                if '*/' in stripped:
                    in_block = False
                    stripped = stripped.split('*/', 1)[1].strip()
                else:
                    continue
            if stripped.startswith('/*'):
                if '*/' not in stripped:
                    in_block = True
                    continue
                stripped = stripped.split('*/', 1)[1].strip()

            m = DISABLED_RE.match(line)
            if m:
                self.disabled_only(m.group(1))
                continue

            active = all(entry[0] for entry in stack) if stack else True

            cond = COND_RE.match(line)
            if cond:
                kind, rest = cond.group(1), cond.group(2).strip()
                # `#endif SERV_X` / `#else // note` carry a trailing comment worth dropping
                rest = rest.split('//')[0].strip()
                if kind == 'ifdef':
                    name = rest.split()[0] if rest else ''
                    if name:
                        self.reference(name)
                    value = self.enabled(name) if name else False
                    stack.append([value, value])
                elif kind == 'ifndef':
                    name = rest.split()[0] if rest else ''
                    if name:
                        self.reference(name)
                    value = not self.enabled(name) if name else True
                    stack.append([value, value])
                elif kind == 'if':
                    value = self.evaluate(rest)
                    if value is None:
                        self.unknown.append('%s:%d  #if %s' %
                                            (os.path.basename(path), _line_no(lines, line), rest))
                        value = False
                    stack.append([value, value])
                elif kind == 'elif':
                    if not stack:
                        continue
                    entry = stack[-1]
                    if entry[1]:
                        entry[0] = False
                    else:
                        value = self.evaluate(rest)
                        if value is None:
                            self.unknown.append('%s  #elif %s' % (os.path.basename(path), rest))
                            value = False
                        entry[0] = value
                        entry[1] = value or entry[1]
                elif kind == 'else':
                    if stack:
                        entry = stack[-1]
                        entry[0] = not entry[1]
                        entry[1] = True
                else:  # endif
                    if stack:
                        stack.pop()
                continue

            if not active:
                continue

            inc = INCLUDE_RE.match(line)
            if inc:
                target = os.path.join(os.path.dirname(path), inc.group(1).replace('\\', '/'))
                if os.path.exists(target):
                    self.process(target, depth + 1)
                continue

            m = UNDEF_RE.match(line)
            if m:
                self.undef(m.group(1))
                continue

            m = DEFINE_RE.match(line)
            if m:
                self.define(m.group(1))


PACKET_FILES = ('CommonPacket.h', 'CommonPacket.cpp', 'ClientPacket.h', 'ClientPacket.cpp',
                'ServerPacket.h')
TESTED_RE = re.compile(r'^[ \t]*#[ \t]*(?:ifdef|ifndef|if|elif)\b[ \t]*(.*)$')
NAME_RE = re.compile(r'defined\s*\(?\s*([A-Za-z_]\w*)|\b([A-Za-z_]\w*)\b')


def scan_packet_flags(pp, directory):
    """Records flags the packet headers test but the define tree never defines."""
    for name in PACKET_FILES:
        path = os.path.join(directory, name)
        if not os.path.exists(path):
            continue
        text = open(path, encoding='cp949', errors='replace').read()
        for line in text.split('\n'):
            m = TESTED_RE.match(line)
            if not m:
                continue
            rest = m.group(1).split('//')[0]
            for a, b in NAME_RE.findall(rest):
                symbol = a or b
                if symbol and symbol not in pp.state:
                    pp.referenced.setdefault(symbol, False)


def _line_no(lines, target):
    for index, line in enumerate(lines, 1):
        if line is target:
            return index
    return 0


def main():
    if len(sys.argv) != 3:
        print(__doc__)
        return 1
    src, out = sys.argv[1], sys.argv[2]
    pp = Preprocessor()
    pp.process(src)
    scan_packet_flags(pp, os.path.dirname(os.path.abspath(src)))

    enabled = [name for name in pp.order if pp.state.get(name)]
    disabled = [name for name in pp.order if not pp.state.get(name)]
    # A macro the tree never defines but a header tests is resolved `false`; listing it keeps the
    # generated header and the static_asserts in kproto in agreement.
    never_defined = sorted(name for name in pp.referenced
                           if name not in pp.state and name not in disabled)
    regions = [n for n in enabled if n.startswith('SERV_COUNTRY_')]

    lines = [
        '// kproto/FeatureFlags.gen.h - GENERATED by tools/gen_featureflags.py, do not edit.',
        '//',
        '// The feature macros that select packet variants in the shared 2014 headers, resolved with a',
        '// real define/undef pass over ServerDefine.h *and* the OnlyGlobal/ServerDefine_* files it',
        '// includes. Every layout in kproto assumes exactly this set; UnitPackets.h static_asserts the',
        '// ones a packet depends on so a future change cannot shift a field silently.',
        '//',
        '// Region: %s (the SERV_COUNTRY_* block in ServerDefine.h is inside #ifdef SERV_INT_INTERNAL,' % (
            ', '.join(regions) if regions else 'none - the shared/global configuration'),
        '// which is not defined, and no build configuration passes one on the command line).',
        '#pragma once',
        '',
        'namespace knc::features {',
        '',
        '// ---- enabled in the 2014 build (%d) ----' % len(enabled),
    ]
    for name in enabled:
        lines.append('inline constexpr bool k%s = true;' % name)
    lines += ['', '// ---- undefined or commented out in the 2014 build (%d) ----' % len(disabled)]
    for name in disabled:
        lines.append('inline constexpr bool k%s = false;' % name)
    if never_defined:
        lines += ['', '// ---- tested by a packet header but never defined by the define tree (%d) ----'
                  % len(never_defined)]
        lines += ['// A header says `#ifdef X` and X is nowhere in ServerDefine.h, so it is off - and',
                  '// the field it guards is not on the wire. These are listed so the static_asserts in',
                  '// kproto can name them instead of guessing.']
        for name in never_defined:
            lines.append('inline constexpr bool k%s = false;' % name)
    lines += ['', '}  // namespace knc::features', '']
    open(out, 'w').write('\n'.join(lines))

    print('feature flags: %d enabled, %d disabled (%d of them tested but never defined), '
          'region: %s -> %s'
          % (len(enabled), len(disabled), len(never_defined), ', '.join(regions) or 'none', out))
    if pp.unknown:
        print('%d conditional(s) could not be evaluated:' % len(pp.unknown))
        for entry in pp.unknown[:20]:
            print('   ', entry)
        return 1
    return 0


if __name__ == '__main__':
    sys.exit(main())
