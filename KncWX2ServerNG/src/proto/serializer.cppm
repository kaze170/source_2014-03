// ---------------------------------------------------------------------------
// knc.proto.serializer - byte-exact replacement for KNCSDK's KSerializer.
//
// Source of truth: KNCSDK/Include/Serializer/{Serializer.cpp,ByteOrder.cpp,
// SerSTL.h,SerVector.h,SerSet.h,SerMap.h,SerPair.h}.
//
// THE RULES (tagging disabled - see below)
// ----------------------------------------
//   char            1 byte, raw
//   wchar_t         2 bytes, NATIVE little-endian   <-- NOT byte-swapped
//   unsigned char   1 byte, raw
//   short/ushort    2 bytes, BIG-endian
//   int/DWORD/UINT  4 bytes, BIG-endian
//   INT64/UINT64    8 bytes, BIG-endian
//   float           4 bytes, IEEE-754 with the bytes reversed
//   double          8 bytes, IEEE-754 with the bytes reversed
//   bool            1 byte, 1 or 0; on read, `x = (byte == 1)`
//   std::string     DWORD byteCount (BE) + that many raw bytes
//   std::wstring    DWORD byteCount (BE, = chars*2) + UTF-16LE code units
//   raw             bytes only, NO length prefix (PutRaw/GetRaw)
//   vector/list/    DWORD count (BE) + each element
//   deque/set/
//   multiset
//   map/multimap    DWORD count (BE) + each pair (first then second)
//   pair            first then second
//
// Note the deliberate asymmetry: integers are big-endian but wchar_t is not,
// because KSerializer::Put(wchar_t) calls WriteNumber() without an HTON_
// conversion while Put(USHORT) does. Getting this backwards silently corrupts
// every character name and chat line, so it is pinned by tests.
//
// Tagging
// -------
// KSerializer takes a `tagging` flag that prefixes each value with a one-byte
// SerializeTag. Every call site on the wire path uses the default
// (`BeginWriting(&buf)` / `BeginReading(&buf)`), i.e. tagging OFF. We therefore
// implement the untagged format only, and say so loudly rather than carrying a
// dead code path.
//
// Error handling
// --------------
// Legacy Get() returns false on underflow and callers mostly ignore it. We
// throw ProtocolError instead; the session layer turns that into "drop this
// peer", which is the behaviour you actually want facing the internet.
// ---------------------------------------------------------------------------
#ifdef KNC_CLASSIC
#  pragma once
#else
module;
#endif

#include "../knc_prelude.hpp"

#ifdef KNC_CLASSIC
#  include "../core/types.cppm"
#else
export module knc.proto.serializer;
import knc.core.types;
#endif

KNC_EXPORT namespace knc::proto {

/// Maximum element count accepted for a length-prefixed container.
///
/// The legacy reader trusted the DWORD count and looped, so a hostile 4 GiB
/// count was an instant OOM. We cap it: no legitimate packet in the 2014
/// protocol carries anywhere near this many elements (the largest is
/// KPerformerInfo::MAX_UID_NUM == 2000).
inline constexpr std::uint32_t kMaxContainerElements = 1u << 20;

/// Maximum byte length accepted for a length-prefixed blob.
inline constexpr std::uint32_t kMaxBlobBytes = 32u * 1024u * 1024u;

// ===========================================================================
// Writer
// ===========================================================================

class Writer {
public:
    Writer() = default;
    explicit Writer(Bytes initial) : buf_(std::move(initial)) {}

    // --- scalars -----------------------------------------------------------

    void put_char(char v) { buf_.push_back(static_cast<std::uint8_t>(v)); }
    void put_uchar(std::uint8_t v) { buf_.push_back(v); }
    void put_bool(bool v) { buf_.push_back(v ? std::uint8_t{1} : std::uint8_t{0}); }

    /// wchar_t: 2 bytes, native little-endian, NOT byte-swapped.
    void put_wchar(char16_t v) {
        buf_.push_back(static_cast<std::uint8_t>(v & 0xFF));
        buf_.push_back(static_cast<std::uint8_t>((v >> 8) & 0xFF));
    }

    void put_short(std::int16_t v) { put_ushort(static_cast<std::uint16_t>(v)); }
    void put_ushort(std::uint16_t v) {
        buf_.push_back(static_cast<std::uint8_t>(v >> 8));
        buf_.push_back(static_cast<std::uint8_t>(v));
    }

    void put_int(std::int32_t v) { put_dword(static_cast<std::uint32_t>(v)); }
    void put_uint(std::uint32_t v) { put_dword(v); }
    void put_dword(std::uint32_t v) {
        buf_.push_back(static_cast<std::uint8_t>(v >> 24));
        buf_.push_back(static_cast<std::uint8_t>(v >> 16));
        buf_.push_back(static_cast<std::uint8_t>(v >> 8));
        buf_.push_back(static_cast<std::uint8_t>(v));
    }

    void put_int64(std::int64_t v) { put_uint64(static_cast<std::uint64_t>(v)); }
    void put_uint64(std::uint64_t v) {
        for (int shift = 56; shift >= 0; shift -= 8) {
            buf_.push_back(static_cast<std::uint8_t>(v >> shift));
        }
    }

    void put_float(float v) { put_dword(std::bit_cast<std::uint32_t>(v)); }
    void put_double(double v) { put_uint64(std::bit_cast<std::uint64_t>(v)); }

    // --- strings -----------------------------------------------------------

    /// std::string: DWORD byte count, then the bytes.
    void put_string(std::string_view v) {
        put_dword(static_cast<std::uint32_t>(v.size()));
        buf_.insert(buf_.end(), v.begin(), v.end());
    }

    /// std::wstring: DWORD *byte* count (chars * 2), then UTF-16LE units.
    void put_wstring(std::u16string_view v) {
        put_dword(static_cast<std::uint32_t>(v.size() * 2));
        for (char16_t c : v) {
            put_wchar(c);
        }
    }

    // --- raw ---------------------------------------------------------------

    /// PutRaw: no length prefix. The reader must already know the size.
    void put_raw(ByteView v) { buf_.insert(buf_.end(), v.begin(), v.end()); }

    // --- containers --------------------------------------------------------

    /// DWORD count followed by each element, written by `emit`.
    template <typename Range, typename Emit>
    void put_range(const Range& range, std::size_t count, Emit&& emit) {
        put_dword(static_cast<std::uint32_t>(count));
        std::size_t written = 0;
        for (const auto& element : range) {
            emit(*this, element);
            ++written;
        }
        if (written != count) {
            throw ProtocolError{"put_range: element count did not match"};
        }
    }

    template <typename Range, typename Emit>
    void put_container(const Range& range, Emit&& emit) {
        put_range(range, static_cast<std::size_t>(std::size(range)),
                  std::forward<Emit>(emit));
    }

    /// PutArray: identical layout to a container (DWORD count + elements).
    template <typename T, typename Emit>
    void put_array(const T* items, std::uint32_t count, Emit&& emit) {
        put_dword(count);
        for (std::uint32_t i = 0; i < count; ++i) {
            emit(*this, items[i]);
        }
    }

    // --- access ------------------------------------------------------------

    [[nodiscard]] const Bytes& bytes() const& noexcept { return buf_; }
    [[nodiscard]] Bytes take() && noexcept { return std::move(buf_); }
    [[nodiscard]] std::size_t size() const noexcept { return buf_.size(); }
    void clear() noexcept { buf_.clear(); }

private:
    Bytes buf_;
};

// ===========================================================================
// Reader
// ===========================================================================

class Reader {
public:
    explicit Reader(ByteView data) : data_(data) {}

    // --- scalars -----------------------------------------------------------

    [[nodiscard]] char get_char() { return static_cast<char>(take(1)[0]); }
    [[nodiscard]] std::uint8_t get_uchar() { return take(1)[0]; }

    /// Legacy: `x = (val == 1)`. Any byte other than 1 reads as false - we keep
    /// that exactly, because some legacy senders write values other than 0/1.
    [[nodiscard]] bool get_bool() { return take(1)[0] == 1; }

    [[nodiscard]] char16_t get_wchar() {
        const auto p = take(2);
        return static_cast<char16_t>(static_cast<std::uint16_t>(p[0]) |
                                     (static_cast<std::uint16_t>(p[1]) << 8));
    }

    [[nodiscard]] std::int16_t get_short() {
        return static_cast<std::int16_t>(get_ushort());
    }
    [[nodiscard]] std::uint16_t get_ushort() {
        const auto p = take(2);
        return static_cast<std::uint16_t>((static_cast<std::uint16_t>(p[0]) << 8) |
                                          p[1]);
    }

    [[nodiscard]] std::int32_t get_int() {
        return static_cast<std::int32_t>(get_dword());
    }
    [[nodiscard]] std::uint32_t get_uint() { return get_dword(); }
    [[nodiscard]] std::uint32_t get_dword() {
        const auto p = take(4);
        return (static_cast<std::uint32_t>(p[0]) << 24) |
               (static_cast<std::uint32_t>(p[1]) << 16) |
               (static_cast<std::uint32_t>(p[2]) << 8) |
               static_cast<std::uint32_t>(p[3]);
    }

    [[nodiscard]] std::int64_t get_int64() {
        return static_cast<std::int64_t>(get_uint64());
    }
    [[nodiscard]] std::uint64_t get_uint64() {
        const auto p = take(8);
        std::uint64_t v = 0;
        for (std::size_t i = 0; i < 8; ++i) {
            v = (v << 8) | p[i];
        }
        return v;
    }

    [[nodiscard]] float get_float() { return std::bit_cast<float>(get_dword()); }
    [[nodiscard]] double get_double() { return std::bit_cast<double>(get_uint64()); }

    // --- strings -----------------------------------------------------------

    [[nodiscard]] std::string get_string() {
        const std::uint32_t size = get_dword();
        if (size > kMaxBlobBytes) {
            throw ProtocolError{"string length exceeds the sanity cap"};
        }
        // Legacy guard: `if (GetReadLength() < size) return false;`
        const auto p = take(size);
        return std::string{reinterpret_cast<const char*>(p.data()), p.size()};
    }

    [[nodiscard]] std::u16string get_wstring() {
        const std::uint32_t size = get_dword();  // in BYTES
        if ((size % 2) != 0) {
            throw ProtocolError{"wstring byte length is odd"};
        }
        if (size > kMaxBlobBytes) {
            throw ProtocolError{"wstring length exceeds the sanity cap"};
        }
        const auto p = take(size);
        std::u16string out;
        out.reserve(size / 2);
        for (std::size_t i = 0; i < size; i += 2) {
            out.push_back(static_cast<char16_t>(
                static_cast<std::uint16_t>(p[i]) |
                (static_cast<std::uint16_t>(p[i + 1]) << 8)));
        }
        return out;
    }

    // --- raw ---------------------------------------------------------------

    [[nodiscard]] Bytes get_raw(std::size_t count) {
        const auto p = take(count);
        return Bytes{p.begin(), p.end()};
    }

    void get_raw_into(std::span<std::uint8_t> out) {
        const auto p = take(out.size());
        std::memcpy(out.data(), p.data(), p.size());
    }

    // --- containers --------------------------------------------------------

    /// Reads a DWORD count, validates it, then calls `read` that many times.
    template <typename Consume>
    void get_range(Consume&& read) {
        const std::uint32_t count = get_dword();
        if (count > kMaxContainerElements) {
            throw ProtocolError{"container element count exceeds the sanity cap"};
        }
        // Cheap structural guard: every element costs at least one byte, so a
        // count larger than the bytes left can never be honest.
        if (count > remaining()) {
            throw ProtocolError{"container element count exceeds bytes remaining"};
        }
        for (std::uint32_t i = 0; i < count; ++i) {
            read(*this);
        }
    }

    // --- cursor ------------------------------------------------------------

    /// Mirrors KSerBuffer::GetReadLength().
    [[nodiscard]] std::size_t remaining() const noexcept {
        return data_.size() - offset_;
    }
    [[nodiscard]] std::size_t offset() const noexcept { return offset_; }
    [[nodiscard]] bool exhausted() const noexcept { return remaining() == 0; }

    /// Legacy readers routinely stop before the end (newer client, older
    /// server). Call this only where trailing data really is an error.
    void expect_exhausted(const char* what) const {
        if (!exhausted()) {
            throw ProtocolError{std::string{what} + ": " +
                                std::to_string(remaining()) +
                                " unread trailing byte(s)"};
        }
    }

private:
    [[nodiscard]] ByteView take(std::size_t count) {
        if (count > remaining()) {
            throw ProtocolError{"serializer read past end of buffer"};
        }
        const ByteView out = data_.subspan(offset_, count);
        offset_ += count;
        return out;
    }

    ByteView data_;
    std::size_t offset_ = 0;
};

}  // namespace knc::proto
