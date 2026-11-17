// ---------------------------------------------------------------------------
// knc.proto.packet_io - the Codec<T> bridge between the generated packet
// structs and the hand-written Writer/Reader primitives.
//
// The generated code in src/proto/packets/*.inc contains nothing but
//
//     Codec<std::remove_cvref_t<decltype(field)>>::put(w, field);
//     field = Codec<std::remove_cvref_t<decltype(field)>>::get(r);
//
// so every decision about how a C++ type maps onto the 2014 wire format lives
// here, in one place that can be read end to end, rather than being smeared
// across 7537 generated lines.
//
// The mapping is pinned by docs/PROTOCOL.md section 6 and by the tests in
// tests/test_serializer.cpp. If you change anything in this file you are
// changing the wire format.
// ---------------------------------------------------------------------------

#ifdef KNC_CLASSIC
#  pragma once
#else
module;
#endif

#include "../knc_prelude.hpp"

#include <array>
#include <deque>
#include <list>
#include <map>
#include <set>
#include <string>
#include <type_traits>
#include <utility>
#include <vector>

#ifdef KNC_CLASSIC
#  include "../core/types.cppm"
#  include "serializer.cppm"
#else
export module knc.proto.packet_io;
export import knc.core.types;
export import knc.proto.serializer;
#endif

KNC_EXPORT namespace knc::proto {

// ---------------------------------------------------------------------------
// Primary template: anything with write_to/read_from is a generated packet.
// ---------------------------------------------------------------------------

/// Every specialisation provides
///     static void put(Writer&, const T&)
///     static T    get(Reader&)
template <typename T, typename Enable = void>
struct Codec;

template <typename T>
concept SerializableStruct = requires(const T& c, T& m, Writer& w, Reader& r) {
    { c.write_to(w) };
    { T::read_from(r) } -> std::same_as<T>;
};

/// Nested DECL_DATA / DECL_PACKET types. The legacy serializer writes them
/// inline with no length prefix and no tag, so this is a plain recursion.
template <SerializableStruct T>
struct Codec<T, void> {
    static void put(Writer& w, const T& v) { v.write_to(w); }
    static T get(Reader& r) { return T::read_from(r); }
};

// ---------------------------------------------------------------------------
// Scalars
//
// Note the asymmetries that are faithful to the original and are NOT bugs
// here: `bool` reads as `byte == 1` (any other value is false), and `wchar_t`
// is the one integral type written in native little-endian rather than big.
// ---------------------------------------------------------------------------

#define KNC_SCALAR_CODEC(TYPE, PUTFN, GETFN)                     \
    template <>                                                  \
    struct Codec<TYPE, void> {                                   \
        static void put(Writer& w, TYPE v) { w.PUTFN(v); }       \
        static TYPE get(Reader& r) { return r.GETFN(); }         \
    }

KNC_SCALAR_CODEC(char,          put_char,    get_char);
KNC_SCALAR_CODEC(std::uint8_t,  put_uchar,   get_uchar);
KNC_SCALAR_CODEC(bool,          put_bool,    get_bool);
KNC_SCALAR_CODEC(char16_t,      put_wchar,   get_wchar);
KNC_SCALAR_CODEC(std::int16_t,  put_short,   get_short);
KNC_SCALAR_CODEC(std::uint16_t, put_ushort,  get_ushort);
KNC_SCALAR_CODEC(std::int32_t,  put_int,     get_int);
KNC_SCALAR_CODEC(std::uint32_t, put_dword,   get_dword);
KNC_SCALAR_CODEC(std::int64_t,  put_int64,   get_int64);
KNC_SCALAR_CODEC(std::uint64_t, put_uint64,  get_uint64);
KNC_SCALAR_CODEC(float,         put_float,   get_float);
KNC_SCALAR_CODEC(double,        put_double,  get_double);

#undef KNC_SCALAR_CODEC

// ---------------------------------------------------------------------------
// Strings
//
// Both are DWORD *byte* counts, big-endian, followed by raw bytes. For
// u16string that means 2 x the character count, and the characters themselves
// go out little-endian (see put_wchar).
// ---------------------------------------------------------------------------

template <>
struct Codec<std::string, void> {
    static void put(Writer& w, const std::string& v) { w.put_string(v); }
    static std::string get(Reader& r) { return r.get_string(); }
};

template <>
struct Codec<std::u16string, void> {
    static void put(Writer& w, const std::u16string& v) { w.put_wstring(v); }
    static std::u16string get(Reader& r) { return r.get_wstring(); }
};

// ---------------------------------------------------------------------------
// pair - first then second, with NO count in front of it.
// ---------------------------------------------------------------------------

template <typename A, typename B>
struct Codec<std::pair<A, B>, void> {
    static void put(Writer& w, const std::pair<A, B>& v) {
        Codec<A>::put(w, v.first);
        Codec<B>::put(w, v.second);
    }
    static std::pair<A, B> get(Reader& r) {
        // Evaluation order inside a braced initialiser is left to right and
        // guaranteed, unlike a function call's arguments. Do not "simplify"
        // this into `return {Codec<A>::get(r), Codec<B>::get(r)};` without
        // that guarantee in mind - the two reads are order dependent.
        A first = Codec<A>::get(r);
        B second = Codec<B>::get(r);
        return std::pair<A, B>{std::move(first), std::move(second)};
    }
};

// ---------------------------------------------------------------------------
// Sequence containers - DWORD count, big-endian, then the elements.
// ---------------------------------------------------------------------------

template <typename Seq, typename Elem>
struct SequenceCodec {
    static void put(Writer& w, const Seq& v) {
        w.put_container(v, [](Writer& ww, const Elem& e) {
            Codec<Elem>::put(ww, e);
        });
    }
    static Seq get(Reader& r) {
        Seq out;
        r.get_range([&out](Reader& rr) { out.push_back(Codec<Elem>::get(rr)); });
        return out;
    }
};

template <typename T, typename A>
struct Codec<std::vector<T, A>, void> : SequenceCodec<std::vector<T, A>, T> {};

template <typename T, typename A>
struct Codec<std::list<T, A>, void> : SequenceCodec<std::list<T, A>, T> {};

template <typename T, typename A>
struct Codec<std::deque<T, A>, void> : SequenceCodec<std::deque<T, A>, T> {};

// ---------------------------------------------------------------------------
// Associative containers.
//
// A legacy std::set serialises in ascending order simply because that is the
// order iteration yields; rebuilding one by insertion restores the same order
// regardless of the order the elements arrive in. A map is a sequence of
// pairs with a single count in front, not a count per key.
// ---------------------------------------------------------------------------

template <typename Assoc, typename Elem>
struct AssociativeCodec {
    static void put(Writer& w, const Assoc& v) {
        w.put_container(v, [](Writer& ww, const Elem& e) {
            Codec<Elem>::put(ww, e);
        });
    }
    static Assoc get(Reader& r) {
        Assoc out;
        r.get_range([&out](Reader& rr) { out.insert(Codec<Elem>::get(rr)); });
        return out;
    }
};

template <typename K, typename C, typename A>
struct Codec<std::set<K, C, A>, void>
    : AssociativeCodec<std::set<K, C, A>, K> {};

template <typename K, typename C, typename A>
struct Codec<std::multiset<K, C, A>, void>
    : AssociativeCodec<std::multiset<K, C, A>, K> {};

template <typename K, typename V, typename C, typename A>
struct Codec<std::map<K, V, C, A>, void>
    : AssociativeCodec<std::map<K, V, C, A>, std::pair<K, V>> {};

template <typename K, typename V, typename C, typename A>
struct Codec<std::multimap<K, V, C, A>, void>
    : AssociativeCodec<std::multimap<K, V, C, A>, std::pair<K, V>> {};

// Map elements are std::pair<const K, V>; the Codec above is written in terms
// of std::pair<K, V>, so teach the pair codec to accept the const-key form.
template <typename A, typename B>
struct Codec<std::pair<const A, B>, void> {
    static void put(Writer& w, const std::pair<const A, B>& v) {
        Codec<A>::put(w, v.first);
        Codec<B>::put(w, v.second);
    }
    static std::pair<const A, B> get(Reader& r) {
        A first = Codec<A>::get(r);
        B second = Codec<B>::get(r);
        return std::pair<const A, B>{std::move(first), std::move(second)};
    }
};

// ---------------------------------------------------------------------------
// std::array is NOT a container on the wire.
//
// It stands in for a fixed C array (`KSkillData m_a[4]`), which the legacy
// code serialises one explicit element at a time with no count. The generated
// code therefore never asks for Codec<std::array<...>> - it emits a separate
// call per index. This specialisation exists only to make that a compile
// error rather than a silent framing change if someone hand-writes one.
// ---------------------------------------------------------------------------

template <typename T, std::size_t N>
struct Codec<std::array<T, N>, void> {
    static_assert(sizeof(T) == 0,
                  "fixed arrays are serialised element by element, with no "
                  "count prefix - serialise each index explicitly");
};

}  // namespace knc::proto
