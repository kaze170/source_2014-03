#!/usr/bin/env python3
"""Extracts the authoritative wire layout of the 2014 packet types we reimplement.

The wire order is the one in `SERIALIZE_DEFINE_PUT` (that is what the 2014 server actually wrote);
the field types come from the `DECL_DATA`/`DECL_PACKET` declaration. Feature-flag `#ifdef` blocks are
resolved with the same flag set the rewrite is built with (`FeatureFlags.gen.h`), so this prints the
layout the 2014 build really produced, not the union of all variants.

Usage:  python3 tools/dump_original_layouts.py [Type ...]
        python3 tools/dump_original_layouts.py --sizes       # canonical byte sizes
"""
import glob
import os
import re
import sys

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
ORIGINAL = os.path.join(ROOT, '..', 'KncWX2Server', 'Common')
FLAGS = os.path.join(ROOT, 'include', 'kproto', 'FeatureFlags.gen.h')

# Field widths in bytes, fixed part only; strings/containers are variable.
FIXED = {
    'bool': 1, 'char': 1, 'byte': 1, 'UCHAR': 1, 'u_char': 1, 'unsigned char': 1,
    'short': 2, 'USHORT': 2, 'u_short': 2, 'unsigned short': 2,
    'int': 4, 'UINT': 4, 'DWORD': 4, 'u_int': 4, 'unsigned int': 4,
    'INT64': 8, 'UINT64': 8, '__int64': 8, 'UidType': 8,
    'float': 4, 'double': 8,
}


def load_flags():
    flags = {}
    for line in open(FLAGS, encoding='utf-8'):
        m = re.match(r'inline constexpr bool k(\w+) = (true|false);', line.strip())
        if m:
            flags[m.group(1)] = m.group(2) == 'true'
    return flags


def strip_comments(text):
    text = re.sub(r'/\*.*?\*/', '', text, flags=re.S)
    return re.sub(r'//[^\n]*', '', text)


def condition_value(expr, flags):
    """Evaluates a #if / #elif expression against the flag table.

    The 2014 headers only ever use `defined(X)`, `defined X`, `X`, `!`, `&&`, `||` and parentheses,
    so the expression is rewritten into Python and evaluated. An unknown macro is `false`, exactly
    like the preprocessor treats it.
    """
    expr = expr.strip()
    if not expr:
        return True
    expr = re.sub(r'defined\s*\(\s*(\w+)\s*\)',
                  lambda m: ' 1 ' if flags.get(m.group(1), False) else ' 0 ', expr)
    expr = re.sub(r'defined\s+(\w+)',
                  lambda m: ' 1 ' if flags.get(m.group(1), False) else ' 0 ', expr)
    expr = re.sub(r'\b([A-Za-z_]\w*)\b',
                  lambda m: ' 1 ' if flags.get(m.group(1), False) else ' 0 ', expr)
    expr = expr.replace('&&', ' and ').replace('||', ' or ').replace('!', ' not ')
    try:
        return bool(eval(expr, {'__builtins__': {}}, {}))
    except Exception:                                        # pragma: no cover - defensive
        return False


def resolve(text, flags):
    """Evaluates the #ifdef/#ifndef/#if/#elif/#else/#endif ladder in a header fragment."""
    out = []
    stack = []          # [current_branch_active, any_branch_taken]
    for line in text.split('\n'):
        state = all(e[0] for e in stack)
        m = re.match(r'#\s*(ifdef|ifndef|if|elif|else|endif)\b\s*(.*)', line.strip())
        if not m:
            if state:
                out.append(line)
            continue
        kind, rest = m.group(1), m.group(2).strip()
        if kind in ('ifdef', 'ifndef', 'if'):
            if kind == 'ifdef':
                v = flags.get(rest.split()[0] if rest else '', False)
            elif kind == 'ifndef':
                v = not flags.get(rest.split()[0] if rest else '', False)
            else:
                v = condition_value(rest, flags)
            stack.append([v, v])
        elif kind == 'elif':
            entry = stack[-1]
            if entry[1]:
                entry[0] = False
            else:
                entry[0] = condition_value(rest, flags)
                entry[1] = entry[0]
        elif kind == 'else':
            entry = stack[-1]
            entry[0] = not entry[1]
            entry[1] = True
        else:
            stack.pop()
    return '\n'.join(out)


def blank_comments(text):
    """Replaces comments with spaces, keeping offsets so brace matching stays valid.

    The 2014 headers are full of `//{{ ... }}` timestamp markers, and a `{` or `}` inside a comment
    would otherwise unbalance the brace walk that finds the end of a struct.
    """
    out = list(text)
    i = 0
    n = len(text)
    while i < n:
        if text.startswith('//', i):
            while i < n and text[i] != '\n':
                out[i] = ' '
                i += 1
        elif text.startswith('/*', i):
            while i < n and not text.startswith('*/', i):
                if text[i] != '\n':
                    out[i] = ' '
                i += 1
            for _ in range(2):
                if i < n:
                    out[i] = ' '
                    i += 1
        elif text[i] in '"\'':
            quote = text[i]
            i += 1
            while i < n and text[i] != quote:
                if text[i] == '\\':
                    i += 1
                i += 1
            i += 1
        else:
            i += 1
    return ''.join(out)


def load_headers():
    text = ''
    for path in sorted(glob.glob(os.path.join(ORIGINAL, '*.h'))):
        text += '\n' + blank_comments(open(path, encoding='cp949', errors='replace').read())
    return text


def load_serializers():
    text = ''
    for path in sorted(glob.glob(os.path.join(ORIGINAL, '*.cpp'))):
        text += '\n' + open(path, encoding='cp949', errors='replace').read()
    return text


HEADERS = load_headers()
SERIALIZERS = load_serializers()
FLAG_TABLE = load_flags()


def body_of(name):
    # `DECL_DATA(x)` expands to `struct x; ... struct x` so the macro name is immediately followed
    # by a newline, not by `{` - both spellings have to be accepted.
    pattern = r'(?:DECL_PACKET|DECL_DATA)\(\s*' + name + r'\s*\)[ \t]*\r?\n\s*\{'
    m = re.search(pattern, HEADERS)
    if not m:
        return None
    brace = HEADERS.index('{', m.start())
    start = brace
    depth = 0
    for i in range(start, len(HEADERS)):
        if HEADERS[i] == '{':
            depth += 1
        elif HEADERS[i] == '}':
            depth -= 1
            if depth == 0:
                return HEADERS[start:i + 1]
    return None


FIELD_RE = re.compile(
    r'^\s*(?P<type>(?:(?:unsigned|signed|const|static)\s+)*(?:std::)?[A-Za-z_][A-Za-z0-9_:]*'
    r'(?:\s*<[^;]*?>)?)\s+(?P<name>[A-Za-z_]\w*)\s*(?:\[(?P<dim>[^\]]+)\])?\s*;')


def declaration_fields(name):
    """Ordered [(field, type, dim)] from the declaration, flags resolved."""
    body = body_of(name)
    if body is None:
        return None
    body = resolve(strip_comments(body), FLAG_TABLE)
    fields = []
    for line in body.split('\n'):
        m = FIELD_RE.match(line)
        if not m:
            continue
        fields.append((m.group('name'), ' '.join(m.group('type').split()), m.group('dim')))
    return fields


def put_fields(name):
    """Ordered PUT(...) field names, flags resolved; None when the type has no PUT."""
    m = re.search(r'SERIALIZE_DEFINE_PUT\(\s*' + name + r'\s*,[^)]*\)\s*\{(.*?)\n\}', SERIALIZERS, re.S)
    if not m:
        return None
    body = resolve(strip_comments(m.group(1)), FLAG_TABLE)
    return [f.strip() for f in re.findall(r'PUT\(([^)]*)\)', body)]


def is_user_class(type_name):
    return body_of(type_name) is not None


def scalar_size(type_name):
    t = type_name.strip()
    if t in FIXED:
        return FIXED[t]
    return 0  # string / container / user class: variable, handled by the caller


def describe(type_name, indent=0, seen=None):
    """Prints the resolved wire layout and returns its (fixed, variable) size model."""
    seen = seen or set()
    pad = '  ' * indent
    fields = declaration_fields(type_name)
    if fields is None:
        print(pad + '!! %s: declaration not found' % type_name)
        return (0, True)
    by_name = {f[0]: f for f in fields}
    order = put_fields(type_name)
    if order is not None:
        ordered = []
        for entry in order:
            base = re.sub(r'\[.*?\]$', '', entry)
            if base not in by_name:
                # m_charServerSN[0] -> m_charServerSN (an array element is written per index)
                base = re.sub(r'\[\d+\]$', '', base)
            if base not in by_name and not base.startswith('m_') and ('m_' + base) in by_name:
                # KServerSetData declares its members without the m_ prefix.
                base = 'm_' + base
            if base not in by_name:
                print(pad + '!! %s: PUT(%s) has no matching declaration field' % (type_name, entry))
                continue
            fname, ftype, fdim = by_name[base]
            if '[' in entry:
                fdim = re.search(r'\[(.*?)\]', entry).group(1)
            ordered.append((fname, ftype, fdim))
    else:
        ordered = fields

    total = 0
    variable = False
    for fname, ftype, fdim in ordered:
        count = 1
        if fdim is not None:
            try:
                count = int(fdim)
            except ValueError:
                count = int(re.search(r'(\d+)', fdim).group(1)) if re.search(r'(\d+)', fdim) else 1
        if ftype.startswith('std::wstring') or ftype.startswith('std::string') or \
           ftype.startswith('std::vector') or ftype.startswith('std::map') or \
           ftype.startswith('std::set') or ftype.startswith('std::list'):
            variable = True
            print('%s%-34s %-28s [variable%s]' % (pad, fname, ftype, '' if fdim is None else ' x%d' % count))
            continue
        if is_user_class(ftype):
            print('%s%-34s %s' % (pad, fname, ftype))
            sub_fixed, sub_var = describe(ftype, indent + 1, seen)
            total += sub_fixed * count
            variable = variable or sub_var
            continue
        size = scalar_size(ftype)
        total += size * count
        print('%s%-34s %-20s %3d B%s' % (pad, fname, ftype, size * count,
                                         '' if fdim is None else ' x%d' % count))
    return (total, variable)


WAYS = [
    'KUserAuthenticateReq', 'KAccountInfo', 'KAccountOption', 'KAccountBlockInfo',
    'KEGS_CONNECT_REQ', 'KEGS_CONNECT_ACK', 'KEGS_VERIFY_ACCOUNT_ACK',
    'KServerSetData', 'KEGS_GET_SERVER_SET_DATA_ACK', 'KEGS_SELECT_SERVER_SET_REQ',
    'KEGS_SELECT_SERVER_SET_ACK', 'KChannelBonusInfo', 'KChannelInfo', 'KChannelSimpleInfo',
    'KEGS_GET_CHANNEL_LIST_ACK', 'KEGS_NEW_USER_JOIN_REQ', 'KEGS_NEW_USER_JOIN_ACK',
    'KEGS_CREATE_UNIT_REQ', 'KEGS_CREATE_UNIT_ACK', 'KEGS_DELETE_UNIT_REQ', 'KEGS_DELETE_UNIT_ACK',
    'KEGS_SELECT_UNIT_REQ', 'KEGS_SELECT_UNIT_ACK', 'KEGS_GET_MY_INVENTORY_ACK',
    'KEGS_SELECT_UNIT_INVENTORY_INFO_NOT', 'KEGS_MY_UNIT_AND_INVENTORY_INFO_LIST_ACK',
    'KUnitInfo', 'KSkillData', 'KUnitSkillData', 'KStat', 'KLastPositionInfo',
    'KBuffInfo', 'KBuffFactor', 'KBuffIdentity', 'KBuffBehaviorFactor', 'KBuffFinalizerFactor',
    'KDungeonClearInfo', 'KTCClearInfo', 'KInventoryItemInfo', 'KItemInfo',
    'KItemAttributeEnchantInfo', 'KUserGuildInfo',
]


def main():
    args = [a for a in sys.argv[1:]]
    sizes_only = '--sizes' in args
    args = [a for a in args if not a.startswith('--')]
    wanted = args or WAYS
    for type_name in wanted:
        fields = declaration_fields(type_name)
        if fields is None:
            print('===== %s: NOT FOUND' % type_name)
            continue
        if sizes_only:
            fixed, variable = describe(type_name, 0)
            note = ' + variable parts' if variable else ''
            print('%-44s %3d B fixed%s' % (type_name, fixed, note))
        else:
            print('===== %s' % type_name)
            describe(type_name, 1)
    return 0


if __name__ == '__main__':
    sys.exit(main())
