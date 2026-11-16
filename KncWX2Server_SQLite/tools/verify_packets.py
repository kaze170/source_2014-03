#!/usr/bin/env python3
"""Cross checks the rewritten packet layouts against the 2014 originals.

This is the guard rail for the whole `kproto` layer. For every type we reimplement it compares the
ordered *wire field sequence* of the 2014 definition against our `serialize()` body:

  * the order and the flags come from the original `SERIALIZE_DEFINE_PUT` (that is what the 2014
    server actually wrote) resolved with our generated FeatureFlags.gen.h,
  * each field is reduced to a canonical type token (i8/u8/i16/u16/i32/u32/i64/f32/wstr/str and
    vec<...>/map<...>/<user struct>), so a field that is dropped, added or reordered shows up as a
    sequence mismatch even when the C++ member names differ,
  * user-defined members are expanded recursively, so a struct nested three levels deep is still
    compared field by field.

This is worth the effort because a single wrong feature flag - one missed `#undef` in the flag set -
silently shifts every following field, and the client then misreads the rest of the packet. That is
exactly what this script caught during the port.

Usage:  python3 tools/verify_packets.py [--verbose]
Exit code 0 means every checked type matches; 1 means at least one mismatch.
"""
import importlib.util
import os
import re
import sys

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
OUR_LOGIN = os.path.join(ROOT, 'include', 'kproto', 'LoginPackets.h')
OUR_UNIT = os.path.join(ROOT, 'include', 'kproto', 'UnitPackets.h')
OUR_PACKET = os.path.join(ROOT, 'include', 'kproto', 'Packet.h')
DUMPER = os.path.join(ROOT, 'tools', 'dump_original_layouts.py')


def load_dumper():
    spec = importlib.util.spec_from_file_location('dump_original_layouts', DUMPER)
    module = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(module)
    return module


ORIGINAL = load_dumper()

# ------------------------------------------------------------------ canonical type tokens
SCALAR_TOKENS = {
    'bool': 'u8', 'char': 'i8', 'byte': 'u8', 'UCHAR': 'u8', 'u_char': 'u8',
    'unsigned char': 'u8', 'UINT8': 'u8',
    'short': 'i16', 'USHORT': 'u16', 'u_short': 'u16', 'unsigned short': 'u16',
    'int': 'i32', 'UINT': 'u32', 'DWORD': 'u32', 'u_int': 'u32', 'unsigned int': 'u32',
    'INT64': 'i64', 'UINT64': 'u64', '__int64': 'i64', 'UidType': 'i64', 'long long': 'i64',
    'int64_t': 'i64', 'uint64_t': 'u64', 'int32_t': 'i32', 'uint32_t': 'u32',
    'int16_t': 'i16', 'uint16_t': 'u16', 'int8_t': 'i8', 'uint8_t': 'u8',
    'float': 'f32', 'double': 'f64',
    'UINT8': 'u8', 'INT8': 'i8', 'UINT16': 'u16', 'INT16': 'i16',
    'UINT32': 'u32', 'INT32': 'i32', 'UINT64': 'u64', 'INT64_': 'i64',
    'unsigned __int64': 'u64', 'signed char': 'i8', 'wchar_t': 'u16',
}
STRING_TOKENS = {'std::wstring': 'wstr', 'std::string': 'str', 'WideString': 'wstr', 'String': 'str'}


def canonical(type_name, is_ours):
    t = ' '.join(type_name.split())
    # strip cv and storage noise; std::int64_t etc. are the modern spellings of the same widths
    t = re.sub(r'\b(const|static)\b', '', t).strip()
    t = re.sub(r'^std::int(8|16|32|64)_t$', r'int\1_t', t)
    t = re.sub(r'^std::uint(8|16|32|64)_t$', r'uint\1_t', t)
    if t in SCALAR_TOKENS:
        return SCALAR_TOKENS[t]
    if t in STRING_TOKENS:
        return STRING_TOKENS[t]
    m = re.match(r'std::vector\s*<\s*(.+?)\s*>$', t) or re.match(r'vector\s*<\s*(.+?)\s*>$', t)
    if m:
        return 'vec<%s>' % canonical(m.group(1), is_ours)
    m = re.match(r'std::map\s*<\s*(.+?)\s*,\s*(.+?)\s*>$', t) or \
        re.match(r'map\s*<\s*(.+?)\s*,\s*(.+?)\s*>$', t)
    if m:
        return 'map<%s,%s>' % (canonical(m.group(1), is_ours), canonical(m.group(2), is_ours))
    m = re.match(r'std::set\s*<\s*(.+?)\s*>$', t) or re.match(r'set\s*<\s*(.+?)\s*>$', t)
    if m:
        return 'set<%s>' % canonical(m.group(1), is_ours)
    # K-prefixed original names vs our unprefixed ones -> a normalised struct name
    if t.startswith('K') and len(t) > 1 and t[1].isupper():
        t = t[1:]
    return 'struct:%s' % t


# ------------------------------------------------------------------ our side
OURS = ''.join(open(p, encoding='utf-8').read() for p in (OUR_PACKET, OUR_LOGIN, OUR_UNIT))

MEMBER_RE = re.compile(
    r'^\s*(?P<type>(?:(?:unsigned|signed|const|static|struct)\s+)*[A-Za-z_][A-Za-z0-9_:]*'
    r'(?:\s*<[^;{}]*?>)?)\s+'
    r'(?P<name>[A-Za-z_]\w*)\s*(?:\[(?P<dim>[^\]]+)\])?\s*(?:=[^;]*)?;')


def our_struct_body(name):
    m = re.search(r'struct\s+' + name + r'\s*(?:final\s*)?\{', OURS)
    if not m:
        return None
    start = m.end() - 1
    depth = 0
    for i in range(start, len(OURS)):
        if OURS[i] == '{':
            depth += 1
        elif OURS[i] == '}':
            depth -= 1
            if depth == 0:
                return OURS[start:i + 1]
    return None


def our_members(name):
    """Ordered {member: (canonical_type, count)} for a struct we define."""
    body = our_struct_body(name)
    if body is None:
        return None
    members = {}
    # Walk only the struct's own scope: drop the outer braces, then track nesting so members declared
    # inside serialize()/Init() bodies are not mistaken for wire fields.
    inner = body.strip()
    if inner.startswith('{'):
        inner = inner[1:]
    if inner.endswith('}'):
        inner = inner[:-1]
    depth = 0
    for line in inner.split('\n'):
        if depth == 0:
            m = MEMBER_RE.match(line)
            if m and m.group('name') != 'return':
                count = 1
                if m.group('dim'):
                    dim = m.group('dim').strip()
                    try:
                        count = int(dim)
                    except ValueError:
                        constant = re.search(
                            r'constexpr\s+\w+\s+' + re.escape(dim) + r'\s*=\s*(\d+)\s*;', body)
                        if constant:
                            count = int(constant.group(1))
                        else:
                            num = re.search(r'(\d+)', dim)
                            count = int(num.group(1)) if num else 1
                members[m.group('name')] = (canonical(m.group('type'), True), count)
                continue  # an initialiser's braces belong to the member, not to a new scope
        depth += line.count('{') - line.count('}')
    return members


def expand_token(token, count, depth, expander):
    """Expands one (token, count) pair, recursing into struct members when possible."""
    out = []
    for _ in range(count):
        if depth < 8 and token.startswith('struct:'):
            inner = token.split(':', 1)[1]
            nested = expander(inner, depth + 1)
            if nested:
                out.extend(nested)
                continue
        elif depth < 8 and (token.startswith('vec<struct:') or token.startswith('map<')):
            element = token[4:-1] if token.startswith('vec<') else token
            marker = 'vec<struct:'
            if element.startswith(marker):
                inner = element[len(marker):-1]
                nested = expander(inner, depth + 1)
                if nested:
                    out.append('vec<%s>' % ','.join(nested))
                    continue
        out.append(token)
    return out


def our_wire_sequence(name, depth=0):
    """Expands our serialize() body into the ordered list of canonical wire tokens."""
    body = our_struct_body(name)
    if body is None:
        return None
    members = our_members(name) or {}
    ser = re.search(r'void serialize\(Serializer& s\)\s*\{(.*?)\}', body, re.S)
    if not ser:
        if not members:
            return None
        sequence = []
        for token, count in members.values():
            sequence.extend(expand_token(token, count, depth, our_wire_sequence))
        return sequence
    sequence = []
    for field in re.findall(r's\.io\(([^;]*)\);', ser.group(1)):
        field = field.strip()
        base = re.sub(r'\[.*?\]$', '', field)
        if base not in members:
            sequence.append('?%s' % field)
            continue
        token, count = members[base]
        if '[' in field:
            # `s.io(x[0])` writes one element; `s.io(x)` writes the whole (unframed) array.
            count = 1
        sequence.extend(expand_token(token, count, depth, our_wire_sequence))
    return sequence


# ------------------------------------------------------------------ original side
def original_wire_sequence(name, depth=0):
    """Expands the 2014 declaration (resolved) into the ordered canonical wire tokens."""
    fields = ORIGINAL.declaration_fields(name)
    if fields is None and not name.startswith('K'):
        # canonical() drops the K prefix; the 2014 headers use it everywhere.
        fields = ORIGINAL.declaration_fields('K' + name)
        if fields is not None:
            name = 'K' + name
    if fields is None:
        return None
    order = ORIGINAL.put_fields(name)
    # A `return X;` line inside the declaration region parses as a field named X with type
    # `return`; it is not a member, and must not shadow the real declaration of that name.
    members = [f for f in fields if f[1].strip() != 'return']
    by_name = {f[0]: f for f in members}
    entries = []
    if order is not None:
        for entry in order:
            base = re.sub(r'\[\d+\]$', '', entry)
            if base not in by_name:
                alt = 'm_' + base
                if alt in by_name:
                    base = alt
            if base not in by_name:
                entries.append((entry, '?', 1))
                continue
            fname, ftype, fdim = by_name[base]
            if '[' in entry:
                # `PUT( m_aEquippedSkill[0] )` writes exactly one element; the number in the
                # brackets is a subscript, not a repeat count.
                fdim = '1'
            entries.append((fname, ftype, fdim))
    else:
        entries = members

    sequence = []
    for fname, ftype, fdim in entries:
        count = 1
        if fdim is not None:
            try:
                count = int(fdim)
            except ValueError:
                num = re.search(r'(\d+)', fdim)
                count = int(num.group(1)) if num else 1
        if ftype.strip() == 'return':
            continue
        token = canonical(ftype, False)
        sequence.extend(expand_token(token, count, depth, original_wire_sequence))
    return sequence


# ------------------------------------------------------------------ the table
# (type in the 2014 headers, our struct)
CHECKS = [
    ('KUserAuthenticateReq', 'UserAuthenticateReq'),
    ('KAccountInfo', 'AccountInfo'),
    ('KAccountOption', 'AccountOption'),
    ('KAccountBlockInfo', 'AccountBlockInfo'),
    ('EGS_CONNECT_REQ', 'ConnectReq'),
    ('EGS_CONNECT_ACK', 'ConnectAck'),
    ('EGS_VERIFY_ACCOUNT_ACK', 'VerifyAccountAck'),
    ('KServerSetData', 'ServerSetData'),
    ('EGS_GET_SERVER_SET_DATA_ACK', 'GetServerSetDataAck'),
    ('EGS_SELECT_SERVER_SET_REQ', 'SelectServerSetReq'),
    ('EGS_SELECT_SERVER_SET_ACK', 'SelectServerSetAck'),
    ('KChannelBonusInfo', 'ChannelBonusInfo'),
    ('KChannelInfo', 'ChannelInfo'),
    ('EGS_GET_CHANNEL_LIST_ACK', 'GetChannelListAck'),
    ('EGS_NEW_USER_JOIN_REQ', 'NewUserJoinReq'),
    ('EGS_NEW_USER_JOIN_ACK', 'NewUserJoinAck'),
    ('EGS_CREATE_UNIT_REQ', 'CreateUnitReq'),
    ('EGS_CREATE_UNIT_ACK', 'CreateUnitAck'),
    ('EGS_DELETE_UNIT_REQ', 'DeleteUnitReq'),
    ('EGS_DELETE_UNIT_ACK', 'DeleteUnitAck'),
    ('EGS_SELECT_UNIT_REQ', 'SelectUnitReq'),
    ('EGS_SELECT_UNIT_ACK', 'SelectUnitAck'),
    ('EGS_GET_MY_INVENTORY_ACK', 'GetMyInventoryAck'),
    ('EGS_SELECT_UNIT_INVENTORY_INFO_NOT', 'SelectUnitInventoryInfoNot'),
    ('EGS_MY_UNIT_AND_INVENTORY_INFO_LIST_ACK', 'MyUnitAndInventoryInfoListAck'),
    ('KUnitInfo', 'UnitInfo'),
    ('KStat', 'Stat'),
    ('KLastPositionInfo', 'LastPositionInfo'),
    ('KDungeonClearInfo', 'DungeonClearInfo'),
    ('KTCClearInfo', 'TCClearInfo'),
    ('KDungeonPlayInfo', 'DungeonPlayInfo'),
    ('KBuffIdentity', 'BuffIdentity'),
    ('KBuffBehaviorFactor', 'BuffBehaviorFactor'),
    ('KBuffFinalizerFactor', 'BuffFinalizerFactor'),
    ('KBuffFactor', 'BuffFactor'),
    ('KBuffInfo', 'BuffInfo'),
    ('KItemAttributeEnchantInfo', 'ItemAttributeEnchantInfo'),
    ('KItemInfo', 'ItemInfo'),
    ('KInventoryItemInfo', 'InventoryItemInfo'),
    ('KSkillData', 'SkillData'),
    ('KUnitSkillData', 'UnitSkillData'),
    ('KUserGuildInfo', 'UserGuildInfo'),
]

# Fields that exist in the 2014 declaration but are deliberately not part of the wire (their PUT is
# absent), documented here so the report can call them out if they ever show up in our stream.
NOT_ON_THE_WIRE = {
    'KBuffFactor': ['m_uidMesGameUnit', 'm_bIsMesGameUnitNpc'],
}


# ------------------------------------------------------------------ flag completeness
# A flag that decides whether a field exists must be in the generated flag table: if it is missing,
# the resolver treats it as "off", and an off-by-one flag silently shifts every field after it. The
# 42 types below are exactly the ones this server reimplements, so a flag they test but the define
# tree never defines is a real risk - `#ifdef SERV_GOLD_TICKET` inside KItemInfo is that case.
DEFINED_RE = re.compile(r'defined\s*\(?\s*([A-Za-z_]\w*)|\b([A-Za-z_]\w*)\b')

# Macros that only change how a value is computed, never which fields exist.
VALUE_ONLY_FLAGS = {'SERV_CHAR_LOG'}


def _raw_put_body(name):
    m = re.search(r'SERIALIZE_DEFINE_PUT\(\s*' + name + r'\s*,[^)]*\)\s*\{(.*?)\n\}',
                  ORIGINAL.SERIALIZERS, re.S)
    return m.group(1) if m else ''


def _tested_flags(text):
    found = set()
    for line in text.split('\n'):
        directive = re.match(r'\s*#\s*(ifdef|ifndef|if|elif)\b\s*(.*)', line)
        if not directive:
            continue
        kind, rest = directive.group(1), directive.group(2).split('//')[0]
        names = [a or b for a, b in DEFINED_RE.findall(rest)]
        found.update(n for n in names if n)
    return found


def flags_used_by(original_type, seen=None):
    """Every flag tested in a type's declaration / PUT body and in the types it nests."""
    seen = seen or set()
    name = original_type
    if ORIGINAL.body_of(name) is None and not name.startswith('K'):
        name = 'K' + name
    if name in seen or ORIGINAL.body_of(name) is None:
        return set()
    seen.add(name)
    used = _tested_flags(ORIGINAL.body_of(name)) | _tested_flags(_raw_put_body(name))
    for _, ftype, _ in ORIGINAL.declaration_fields(name) or []:
        token = canonical(ftype, False)
        if token.startswith('struct:'):
            used |= flags_used_by(token.split(':', 1)[1], seen)
        elif 'struct:' in token:
            inner = token.split('struct:', 1)[1].rstrip('>')
            used |= flags_used_by(inner, seen)
    return used


def check_flags():
    problems = []
    resolved = set()
    for original_type, _ in CHECKS:
        resolved |= flags_used_by(original_type)
    for flag in sorted(resolved):
        if flag not in ORIGINAL.FLAG_TABLE and flag not in VALUE_ONLY_FLAGS:
            problems.append('%s: tested by a packet this server implements, but '
                            'FeatureFlags.gen.h does not define it' % flag)
    return problems


def compare(original_type, our_struct, verbose):
    expected = original_wire_sequence(original_type)
    if expected is None:
        return ['%s: declaration not found in the 2014 headers' % original_type]
    actual = our_wire_sequence(our_struct)
    if actual is None or not actual:
        return ['%s: no serialize() found for struct %s' % (original_type, our_struct)]

    problems = []
    if expected != actual:
        # A compact diff: the first index where the streams diverge plus the tails.
        index = 0
        while index < min(len(expected), len(actual)) and expected[index] == actual[index]:
            index += 1
        problems.append(
            '%s vs %s: diverges at field %d\n      original: %s\n      ours    : %s'
            % (original_type, our_struct, index,
               expected[max(0, index - 2):index + 6],
               actual[max(0, index - 2):index + 6]))
    if verbose:
        print('  %-34s %d fields' % (original_type, len(expected)))
    return problems


def main():
    verbose = '--verbose' in sys.argv
    if not ORIGINAL.FLAG_TABLE:
        raise SystemExit('no feature flags loaded - run tools/gen_featureflags.py first')
    problems = 0
    for problem in check_flags():
        print(problem)
        problems += 1
    for original_type, our_struct in CHECKS:
        found = compare(original_type, our_struct, verbose)
        for problem in found:
            print(problem)
        problems += len(found)
    if problems:
        print('\n%d layout problem(s) across %d type(s)' % (problems, len(CHECKS)))
        return 1
    print('%d packet layout(s) match the 2014 serializers' % len(CHECKS))
    return 0


if __name__ == '__main__':
    sys.exit(main())
