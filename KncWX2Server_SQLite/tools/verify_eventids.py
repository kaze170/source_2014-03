#!/usr/bin/env python3
"""Cross checks every event id the rewrite uses against the 2014 headers.

The wire has no field names: a client learns what a packet *is* from the event id alone, so a
single off-by-one makes a perfectly encoded payload meaningless. EventId.gen.h is generated, but
the ids are also *spelled by name* in the service code, the docs and the tests - and those
occurrences are hand written. This script is what keeps them honest:

  * it rebuilds the id table straight from EventID_System.h / EventID_Client.h / EventID_Server.h
    using the same collect() the generator uses, so the header and the truth cannot drift,
  * it re-derives every id from EventId.gen.h by evaluating the `X = Y + 1` chain, so a subtly
    broken generated file (a reordered block, a dropped line) is caught rather than trusted,
  * it checks the ids the rewrite actually depends on - the request/ack pairs the services answer
    and the handful of system events the transport raises - and reports any that disagree.

Usage:  python3 tools/verify_eventids.py [-v]
Exit code 0 means every checked id matches; 1 means at least one is wrong.
"""
import importlib.util
import os
import re
import sys

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
GENERATOR = os.path.join(ROOT, 'tools', 'gen_eventids.py')
HEADER = os.path.join(ROOT, 'include', 'kproto', 'EventId.gen.h')
ORIGINAL = os.path.abspath(os.path.join(ROOT, '..', 'KncWX2Server', 'Common'))

GROUPS = ['EventID_System.h', 'EventID_Client.h', 'EventID_Server.h']

# The ids the rewrite touches: every REQ the services dispatch on plus the ACK/NOT they send back,
# and the system events the session layer raises on its own.
CHECKED = [
    # transport
    'E_HEART_BEAT', 'E_ACCEPT_CONNECTION_NOT', 'E_CONNECTION_LOST_NOT',
    # login port
    'EGS_CONNECT_REQ', 'EGS_CONNECT_ACK',
    'EGS_VERIFY_ACCOUNT_REQ', 'EGS_VERIFY_ACCOUNT_ACK',
    'EGS_NEW_USER_JOIN_REQ', 'EGS_NEW_USER_JOIN_ACK',
    # channel port
    'EGS_GET_SERVER_SET_DATA_REQ', 'EGS_GET_SERVER_SET_DATA_ACK',
    'EGS_SELECT_SERVER_SET_REQ', 'EGS_SELECT_SERVER_SET_ACK',
    'EGS_GET_CHANNEL_LIST_REQ', 'EGS_GET_CHANNEL_LIST_ACK',
    'ECH_GET_CHANNEL_LIST_REQ',
    'EGS_MY_UNIT_AND_INVENTORY_INFO_LIST_REQ', 'EGS_MY_UNIT_AND_INVENTORY_INFO_LIST_ACK',
    'EGS_CREATE_UNIT_REQ', 'EGS_CREATE_UNIT_ACK',
    'EGS_DELETE_UNIT_REQ', 'EGS_DELETE_UNIT_ACK',
    'EGS_SELECT_UNIT_REQ', 'EGS_SELECT_UNIT_ACK',
    'EGS_GET_MY_INVENTORY_REQ', 'EGS_GET_MY_INVENTORY_ACK',
    'EGS_SELECT_UNIT_INVENTORY_INFO_NOT',
    'EGS_CHECK_CHANNEL_CHANGE_REQ', 'EGS_CHECK_CHANNEL_CHANGE_ACK',
]

# Every request the services answer must have its acknowledgement exactly one id later - the 2014
# headers are written that way, and a client that assumed it would break otherwise.
REQ_ACK_PAIRS = [
    ('EGS_CONNECT_REQ', 'EGS_CONNECT_ACK'),
    ('EGS_VERIFY_ACCOUNT_REQ', 'EGS_VERIFY_ACCOUNT_ACK'),
    ('EGS_NEW_USER_JOIN_REQ', 'EGS_NEW_USER_JOIN_ACK'),
    ('EGS_GET_SERVER_SET_DATA_REQ', 'EGS_GET_SERVER_SET_DATA_ACK'),
    ('EGS_SELECT_SERVER_SET_REQ', 'EGS_SELECT_SERVER_SET_ACK'),
    ('EGS_GET_CHANNEL_LIST_REQ', 'EGS_GET_CHANNEL_LIST_ACK'),
    ('EGS_MY_UNIT_AND_INVENTORY_INFO_LIST_REQ', 'EGS_MY_UNIT_AND_INVENTORY_INFO_LIST_ACK'),
    ('EGS_CREATE_UNIT_REQ', 'EGS_CREATE_UNIT_ACK'),
    ('EGS_DELETE_UNIT_REQ', 'EGS_DELETE_UNIT_ACK'),
    ('EGS_SELECT_UNIT_REQ', 'EGS_SELECT_UNIT_ACK'),
    ('EGS_GET_MY_INVENTORY_REQ', 'EGS_GET_MY_INVENTORY_ACK'),
    ('EGS_CHECK_CHANNEL_CHANGE_REQ', 'EGS_CHECK_CHANNEL_CHANGE_ACK'),
]


def load_generator():
    spec = importlib.util.spec_from_file_location('gen_eventids', GENERATOR)
    module = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(module)
    return module


def truth_table():
    """The ids exactly as the 2014 headers enumerate them."""
    gen = load_generator()
    table = {}
    index = 0
    for header in GROUPS:
        for name, value in gen.collect(os.path.join(ORIGINAL, header), ORIGINAL):
            table[name] = int(value) if value is not None else index
            index = int(table[name]) + 1 if value is not None else index + 1
    return table


ABSOLUTE_RE = re.compile(r'inline constexpr EventId (\w+) = (\d+);')
RELATIVE_RE = re.compile(r'inline constexpr EventId (\w+) = (\w+) \+ 1;')


def generated_table():
    """Re-evaluates EventId.gen.h the way the compiler would, following the `X + 1` chain."""
    text = open(HEADER, encoding='utf-8').read()
    values = {}
    order = []
    for line in text.split('\n'):
        line = line.strip()
        m = ABSOLUTE_RE.match(line)
        if m:
            values[m.group(1)] = int(m.group(2))
            order.append(m.group(1))
            continue
        m = RELATIVE_RE.match(line)
        if m:
            base = m.group(2)
            if base not in values:
                raise SystemExit('%s: %s is defined before %s' % (HEADER, m.group(1), base))
            values[m.group(1)] = values[base] + 1
            order.append(m.group(1))
    return values, order


def main():
    verbose = '-v' in sys.argv or '--verbose' in sys.argv
    truth = truth_table()
    header, order = generated_table()

    problems = []

    # 1. The generated header must agree with the headers it claims to be generated from, in order.
    if len(order) != len(truth):
        problems.append('generated header has %d ids, the 2014 headers have %d' % (len(order), len(truth)))
    for name in truth:
        if name not in header:
            problems.append('%s is in the 2014 headers but missing from EventId.gen.h' % name)
        elif header[name] != truth[name]:
            problems.append('%s is %d in the 2014 headers but %d in EventId.gen.h'
                            % (name, truth[name], header[name]))
    for name in header:
        if name not in truth:
            problems.append('%s is in EventId.gen.h but not in the 2014 headers' % name)

    # 2. The ids the rewrite spells by name must be the right ones.
    for name in CHECKED:
        if name not in truth:
            problems.append('%s: checked by this script but absent from the 2014 headers' % name)
        elif name not in header:
            problems.append('%s: absent from EventId.gen.h' % name)
        elif verbose:
            print('  %-42s %d' % (name, truth[name]))

    # 3. Every REQ/ACK pair the services rely on must be adjacent.
    for req, ack in REQ_ACK_PAIRS:
        if req in truth and ack in truth and truth[ack] != truth[req] + 1:
            problems.append('%s is followed by id %d, but %s is %d'
                            % (req, truth[req] + 1, ack, truth[ack]))

    if problems:
        for problem in problems[:40]:
            print(problem)
        if len(problems) > 40:
            print('... and %d more' % (len(problems) - 40))
        print('\n%d event id problem(s)' % len(problems))
        return 1
    print('%d event id(s) verified against the 2014 headers (%d total), %d checked by name, '
          '%d REQ/ACK pairs adjacent'
          % (len(header), len(truth), len(CHECKED), len(REQ_ACK_PAIRS)))
    return 0


if __name__ == '__main__':
    sys.exit(main())
