// ---------------------------------------------------------------------------
// Conformance tests for knc.proto.serializer.
//
// These pin the *byte layout* of the 2014 KSerializer format. If a change
// here is ever "needed", the wire protocol has been broken and every shipped
// client stops working - so the expected byte strings below are written out
// literally rather than derived from the implementation.
// ---------------------------------------------------------------------------
#include "test_util.hpp"

#include <deque>
#include <list>
#include <map>
#include <set>

#include "../src/proto/serializer.cppm"

using namespace knc;
using namespace knc::proto;
using knctest::from_ascii;
using knctest::from_hex;

namespace {

Bytes written(void (*fn)(Writer&)) {
    Writer w;
    fn(w);
    return std::move(w).take();
}

// ---------------------------------------------------------------------------

void test_integer_endianness(knctest::Suite& s) {
    s.run("short / ushort are big-endian", [] {
        Writer w;
        w.put_ushort(0x1234);
        w.put_short(static_cast<std::int16_t>(-2));  // 0xFFFE
        KNC_CHECK_EQ_BYTES(std::move(w).take(), from_hex("1234 fffe"),
                           "ushort/short BE layout");
    });

    s.run("int / DWORD are big-endian", [] {
        Writer w;
        w.put_dword(0x01020304u);
        w.put_int(-2);
        KNC_CHECK_EQ_BYTES(std::move(w).take(), from_hex("01020304 fffffffe"),
                           "dword/int BE layout");
    });

    s.run("INT64 / UINT64 are big-endian", [] {
        Writer w;
        w.put_uint64(0x0102030405060708ull);
        w.put_int64(-1);
        KNC_CHECK_EQ_BYTES(std::move(w).take(),
                           from_hex("0102030405060708 ffffffffffffffff"),
                           "int64 BE layout");
    });

    s.run("wchar_t stays native LITTLE-endian", [] {
        // The single most dangerous asymmetry in the format. Put(wchar_t)
        // omits the HTON_ conversion that Put(USHORT) applies.
        Writer w;
        w.put_wchar(u'A');       // 0x0041 -> 41 00
        w.put_wchar(u'\u4E2D');  // 0x4E2D -> 2d 4e
        KNC_CHECK_EQ_BYTES(std::move(w).take(), from_hex("4100 2d4e"),
                           "wchar_t LE layout");
    });

    s.run("wchar_t and ushort disagree for the same value", [] {
        Writer a;
        a.put_wchar(static_cast<char16_t>(0x1234));
        Writer b;
        b.put_ushort(0x1234);
        const Bytes wide = std::move(a).take();
        const Bytes narrow = std::move(b).take();
        KNC_CHECK(wide != narrow, "wchar_t must NOT match ushort byte order");
        KNC_CHECK_EQ_BYTES(wide, from_hex("3412"), "wchar_t little-endian");
        KNC_CHECK_EQ_BYTES(narrow, from_hex("1234"), "ushort big-endian");
    });
}

void test_floats(knctest::Suite& s) {
    s.run("float / double are byte-reversed IEEE-754", [] {
        // 1.0f  = 0x3F800000 -> BE bytes 3f 80 00 00
        // -2.5  = 0xC004000000000000
        Writer w;
        w.put_float(1.0f);
        w.put_double(-2.5);
        KNC_CHECK_EQ_BYTES(std::move(w).take(),
                           from_hex("3f800000 c004000000000000"),
                           "float/double layout");
    });

    s.run("float / double round-trip", [] {
        Writer w;
        w.put_float(3.14159265f);
        w.put_double(2.718281828459045);
        const Bytes buf = std::move(w).take();
        Reader r{view(buf)};
        KNC_CHECK(r.get_float() == 3.14159265f, "float round-trip");
        KNC_CHECK(r.get_double() == 2.718281828459045, "double round-trip");
        KNC_CHECK(r.exhausted(), "float/double consumed every byte");
    });
}

void test_bool_and_bytes(knctest::Suite& s) {
    s.run("bool writes 1/0", [] {
        Writer w;
        w.put_bool(true);
        w.put_bool(false);
        KNC_CHECK_EQ_BYTES(std::move(w).take(), from_hex("0100"), "bool layout");
    });

    s.run("bool reads as (byte == 1), not (byte != 0)", [] {
        // Legacy: `Get(bool& x) { ... x = (val == 1); }`. A byte of 2 is
        // therefore FALSE. Several legacy senders do emit non-0/1 bytes, so
        // this quirk is load-bearing.
        const Bytes buf = from_hex("00 01 02 ff");
        Reader r{view(buf)};
        KNC_CHECK_EQ(r.get_bool(), false, "0x00 -> false");
        KNC_CHECK_EQ(r.get_bool(), true, "0x01 -> true");
        KNC_CHECK_EQ(r.get_bool(), false, "0x02 -> false (NOT true)");
        KNC_CHECK_EQ(r.get_bool(), false, "0xff -> false (NOT true)");
    });

    s.run("char / uchar are raw single bytes", [] {
        Writer w;
        w.put_char('Z');
        w.put_uchar(0xFE);
        KNC_CHECK_EQ_BYTES(std::move(w).take(), from_hex("5a fe"), "char layout");
    });
}

void test_strings(knctest::Suite& s) {
    s.run("std::string = BE DWORD byte count + bytes", [] {
        Writer w;
        w.put_string("abc");
        KNC_CHECK_EQ_BYTES(std::move(w).take(), from_hex("00000003 616263"),
                           "string layout");
    });

    s.run("empty string is just a zero length", [] {
        Writer w;
        w.put_string("");
        KNC_CHECK_EQ_BYTES(std::move(w).take(), from_hex("00000000"),
                           "empty string layout");
    });

    s.run("std::wstring length is in BYTES, units are UTF-16LE", [] {
        Writer w;
        w.put_wstring(u"AB");
        KNC_CHECK_EQ_BYTES(std::move(w).take(), from_hex("00000004 4100 4200"),
                           "wstring layout");
    });

    s.run("wstring carries non-ASCII intact", [] {
        // U+5929 U+4E0B ("天下") -> 29 59 0b 4e, length 4.
        Writer w;
        w.put_wstring(u"\u5929\u4E0B");
        const Bytes buf = std::move(w).take();
        KNC_CHECK_EQ_BYTES(buf, from_hex("00000004 2959 0b4e"), "CJK wstring layout");
        Reader r{view(buf)};
        KNC_CHECK(r.get_wstring() == std::u16string(u"\u5929\u4E0B"),
                  "CJK wstring round-trip");
    });

    s.run("odd wstring byte length is rejected", [] {
        const Bytes buf = from_hex("00000003 414243");
        Reader r{view(buf)};
        KNC_CHECK_THROWS(r.get_wstring(), "odd wstring length must throw");
    });

    s.run("string length beyond the buffer is rejected", [] {
        const Bytes buf = from_hex("000000ff 4142");
        Reader r{view(buf)};
        KNC_CHECK_THROWS(r.get_string(), "truncated string must throw");
    });

    s.run("absurd string length is rejected before allocating", [] {
        const Bytes buf = from_hex("ffffffff");
        Reader r{view(buf)};
        KNC_CHECK_THROWS(r.get_string(), "4 GiB string must throw");
    });
}

void test_raw(knctest::Suite& s) {
    s.run("put_raw has no length prefix", [] {
        Writer w;
        w.put_raw(view(from_hex("deadbeef")));
        KNC_CHECK_EQ_BYTES(std::move(w).take(), from_hex("deadbeef"), "raw layout");
    });

    s.run("get_raw honours the requested count and no more", [] {
        const Bytes buf = from_hex("00112233");
        Reader r{view(buf)};
        KNC_CHECK_EQ_BYTES(r.get_raw(2), from_hex("0011"), "raw partial read");
        KNC_CHECK_EQ(r.remaining(), std::size_t{2}, "raw read advanced the cursor");
        KNC_CHECK_THROWS(r.get_raw(3), "over-long raw read must throw");
    });
}

void test_containers(knctest::Suite& s) {
    s.run("vector = BE DWORD count + elements", [] {
        const std::vector<std::uint32_t> values{1, 2, 3};
        Writer w;
        w.put_container(values,
                        [](Writer& out, std::uint32_t v) { out.put_dword(v); });
        KNC_CHECK_EQ_BYTES(std::move(w).take(),
                           from_hex("00000003 00000001 00000002 00000003"),
                           "vector layout");
    });

    s.run("set serializes in ascending order", [] {
        const std::set<std::int64_t> values{5, -1, 3};
        Writer w;
        w.put_container(values,
                        [](Writer& out, std::int64_t v) { out.put_int64(v); });
        // std::set order: -1, 3, 5
        KNC_CHECK_EQ_BYTES(
            std::move(w).take(),
            from_hex("00000003 ffffffffffffffff 0000000000000003 0000000000000005"),
            "set layout and ordering");
    });

    s.run("map = BE DWORD count + (key, value) pairs with no inner count", [] {
        const std::map<std::uint16_t, std::uint32_t> values{{1, 10}, {2, 20}};
        Writer w;
        w.put_container(values, [](Writer& out, const auto& kv) {
            out.put_ushort(kv.first);
            out.put_dword(kv.second);
        });
        KNC_CHECK_EQ_BYTES(std::move(w).take(),
                           from_hex("00000002 0001 0000000a 0002 00000014"),
                           "map layout");
    });

    s.run("empty container is a bare zero count", [] {
        const std::list<int> values;
        Writer w;
        w.put_container(values, [](Writer& out, int v) { out.put_int(v); });
        KNC_CHECK_EQ_BYTES(std::move(w).take(), from_hex("00000000"),
                           "empty container layout");
    });

    s.run("container round-trips through a deque", [] {
        const std::deque<std::uint16_t> values{7, 8, 9};
        Writer w;
        w.put_container(values,
                        [](Writer& out, std::uint16_t v) { out.put_ushort(v); });
        const Bytes buf = std::move(w).take();
        std::deque<std::uint16_t> back;
        Reader r{view(buf)};
        r.get_range([&back](Reader& in) { back.push_back(in.get_ushort()); });
        KNC_CHECK(back == values, "deque round-trip");
        KNC_CHECK(r.exhausted(), "deque consumed every byte");
    });

    s.run("hostile element count is rejected without looping", [] {
        // 0xFFFFFFFF elements followed by 4 bytes. The legacy reader would
        // loop 4 billion times; we must refuse immediately.
        const Bytes buf = from_hex("ffffffff 00000000");
        Reader r{view(buf)};
        KNC_CHECK_THROWS(r.get_range([](Reader& in) { (void)in.get_dword(); }),
                         "4-billion element count must throw");
    });

    s.run("plausible-but-impossible element count is rejected", [] {
        // 1000 elements but only 4 bytes left: impossible even at 1 byte each.
        const Bytes buf = from_hex("000003e8 00000000");
        Reader r{view(buf)};
        KNC_CHECK_THROWS(r.get_range([](Reader& in) { (void)in.get_dword(); }),
                         "count exceeding remaining bytes must throw");
    });

    s.run("put_array matches container layout", [] {
        const std::uint8_t items[3] = {1, 2, 3};
        Writer w;
        w.put_array(items, 3, [](Writer& out, std::uint8_t v) { out.put_uchar(v); });
        KNC_CHECK_EQ_BYTES(std::move(w).take(), from_hex("00000003 010203"),
                           "put_array layout");
    });
}

void test_cursor(knctest::Suite& s) {
    s.run("underflow throws instead of silently returning false", [] {
        const Bytes buf = from_hex("0102");
        Reader r{view(buf)};
        KNC_CHECK_THROWS(r.get_dword(), "reading 4 bytes from 2 must throw");
    });

    s.run("remaining() tracks the cursor", [] {
        const Bytes buf = from_hex("0102030405");
        Reader r{view(buf)};
        KNC_CHECK_EQ(r.remaining(), std::size_t{5}, "initial remaining");
        (void)r.get_uchar();
        KNC_CHECK_EQ(r.remaining(), std::size_t{4}, "remaining after 1 byte");
        KNC_CHECK_EQ(r.offset(), std::size_t{1}, "offset after 1 byte");
        (void)r.get_dword();
        KNC_CHECK(r.exhausted(), "exhausted after 5 bytes");
    });

    s.run("expect_exhausted flags trailing junk", [] {
        const Bytes buf = from_hex("01 02");
        Reader r{view(buf)};
        (void)r.get_uchar();
        KNC_CHECK_THROWS(r.expect_exhausted("test"), "trailing byte must throw");
    });
}

void test_mixed_record(knctest::Suite& s) {
    s.run("a realistic mixed record is byte-exact", [] {
        // Shape of a typical login packet body: int, wstring, bool, string.
        Writer w;
        w.put_int(1);
        w.put_wstring(u"Hi");
        w.put_bool(true);
        w.put_string("x");
        const Bytes buf = std::move(w).take();
        KNC_CHECK_EQ_BYTES(buf,
                           from_hex("00000001"        // int 1, BE
                                    "00000004 48006900"  // wstring "Hi", LE units
                                    "01"              // bool true
                                    "00000001 78"),   // string "x"
                           "mixed record layout");

        Reader r{view(buf)};
        KNC_CHECK_EQ(r.get_int(), 1, "mixed: int");
        KNC_CHECK(r.get_wstring() == std::u16string(u"Hi"), "mixed: wstring");
        KNC_CHECK_EQ(r.get_bool(), true, "mixed: bool");
        KNC_CHECK(r.get_string() == "x", "mixed: string");
        KNC_CHECK(r.exhausted(), "mixed: consumed every byte");
    });

    s.run("writer can be seeded and taken repeatedly", [] {
        const Bytes buf = written([](Writer& w) {
            w.put_uchar(0xAA);
            w.put_ushort(0xBBCC);
        });
        KNC_CHECK_EQ_BYTES(buf, from_hex("aa bbcc"), "seeded writer layout");
    });
}

}  // namespace

int main() {
    knctest::Suite suite{"KSerializer wire format"};
    test_integer_endianness(suite);
    test_floats(suite);
    test_bool_and_bytes(suite);
    test_strings(suite);
    test_raw(suite);
    test_containers(suite);
    test_cursor(suite);
    test_mixed_record(suite);
    return suite.summary();
}
