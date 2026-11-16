// kcore/Serialize.h - byte exact reimplementation of KncSDK's KSerializer.
//
// Wire rules reproduced from KNCSDK/Include/Serializer/Serializer.cpp:
//   * no type tags (m_tagsEnabled == false for every packet in the original server)
//   * every fixed width number is big endian
//   * bool is one byte, 1 or 0
//   * std::string   -> [DWORD_BE byteLength][bytes]
//   * std::wstring  -> [DWORD_BE byteLength][UTF-16LE code units]  (byteLength == units * 2)
//   * containers    -> [DWORD_BE count][elements]
//   * pair          -> first, second
//   * KSerBuffer    -> [DWORD_BE len][ (len>0) ? bool compressed + bytes ]
#pragma once

#include <concepts>
#include <cstring>
#include <deque>
#include <list>
#include <map>
#include <set>
#include <type_traits>
#include <utility>
#include <vector>

#include "kcore/ByteOrder.h"
#include "kcore/SerBuffer.h"
#include "kcore/Types.h"

namespace knc {

class Serializer;

// User defined wire types provide `void serialize(Serializer&)` (const for writing) or a free
// `serialize(Serializer&, T&)` found through ADL.
template <class T>
concept MemberSerializable = requires(T& v, Serializer& s) { v.serialize(s); };

class Serializer {
public:
    enum class Mode { kIdle, kWrite, kRead };
    enum class Failure { kOk, kUnderflow, kBadTag, kBadLength };

    Serializer() = default;
    explicit Serializer(SerBuffer& buffer) { beginWriting(&buffer); }

    // ---- buffer management (BeginWriting / BeginReading) ------------------
    void beginWriting(SerBuffer* buffer, bool tagging = false) {
        buffer_ = buffer;
        tagging_ = tagging;
        mode_ = Mode::kWrite;
        failure_ = Failure::kOk;
    }
    void beginReading(SerBuffer* buffer, bool tagging = false) {
        buffer_ = buffer;
        tagging_ = tagging;
        mode_ = Mode::kRead;
        failure_ = Failure::kOk;
    }
    void end() { mode_ = Mode::kIdle; }

    Mode mode() const { return mode_; }
    bool failed() const { return failure_ != Failure::kOk; }
    bool ok() const { return !failed(); }
    Failure failure() const { return failure_; }
    bool writing() const { return mode_ == Mode::kWrite; }
    bool reading() const { return mode_ == Mode::kRead; }
    std::size_t readRemaining() const { return buffer_ ? buffer_->readRemaining() : 0; }
    bool hasMore() const { return readRemaining() > 0; }
    SerBuffer* buffer() { return buffer_; }

    // Lenient reading: truncated/absent trailing fields read as zero and do not fail.
    void setLenient(bool lenient) { lenient_ = lenient; }
    bool lenient() const { return lenient_; }

    void setTagsEnabled(bool on) { tagging_ = on; }
    bool tagsEnabled() const { return tagging_; }

    // ---- public API ------------------------------------------------------
    template <class T>
    void put(const T& v) {
        writeOne(v);
    }
    template <class T, class... Rest>
    void put(const T& v, const Rest&... rest) {
        writeOne(v);
        put(rest...);
    }
    // Direction-agnostic field access: writes when the serializer is in write mode, reads
    // otherwise. This is what wire structs use so a single serialize() body serves both ways.
    template <class T>
    void io(T& v) {
        if (mode_ == Mode::kWrite) {
            writeOne(v);
        } else {
            readOne(v);
        }
    }
    template <class T>
    void get(T& v) {
        readOne(v);
    }
    template <class T, class... Rest>
    void get(T& v, Rest&... rest) {
        readOne(v);
        get(rest...);
    }

    void putBytes(const void* src, std::size_t len) {
        if (mode_ == Mode::kWrite) buffer_->append(src, len);
    }
    bool getBytes(void* dst, std::size_t len) {
        if (!buffer_ || !buffer_->read(dst, len)) {
            fail(Failure::kUnderflow);
            if (lenient_) std::memset(dst, 0, len);
            return false;
        }
        return true;
    }

private:
    void fail(Failure f) {
        if (failure_ == Failure::kOk) failure_ = f;
    }

    template <class T>
    void writeNumber(T v) {
        if (mode_ != Mode::kWrite) {
            fail(Failure::kBadTag);
            return;
        }
        if constexpr (sizeof(T) == 1) {
            buffer_->append(&v, 1);
        } else {
            T be;
            if constexpr (sizeof(T) == 2) {
                be = static_cast<T>(hton16(static_cast<USHORT>(v)));
            } else if constexpr (sizeof(T) == 4) {
                be = static_cast<T>(hton32(static_cast<UINT>(v)));
            } else {
                be = static_cast<T>(hton64(static_cast<UINT64>(v)));
            }
            buffer_->append(&be, sizeof(T));
        }
    }

    template <class T>
    void readNumber(T& v) {
        if (mode_ != Mode::kRead || !buffer_) {
            fail(Failure::kBadTag);
            v = 0;
            return;
        }
        T be{};
        if (!buffer_->read(&be, sizeof(T))) {
            fail(Failure::kUnderflow);
            v = 0;
            return;
        }
        if constexpr (sizeof(T) == 1) {
            v = be;
        } else if constexpr (sizeof(T) == 2) {
            v = static_cast<T>(ntoh16(static_cast<USHORT>(be)));
        } else if constexpr (sizeof(T) == 4) {
            v = static_cast<T>(ntoh32(static_cast<UINT>(be)));
        } else {
            v = static_cast<T>(ntoh64(static_cast<UINT64>(be)));
        }
    }

    void writeCount(std::size_t n) { writeNumber<DWORD>(static_cast<DWORD>(n)); }

    std::uint32_t readCount() {
        DWORD n = 0;
        readNumber<DWORD>(n);
        if (failed()) return 0;
        // Guard against absurd counts: every element is at least one byte.
        if (n > readRemaining() + 1) {
            fail(Failure::kBadLength);
            return 0;
        }
        return n;
    }

    // ---- write overload set ---------------------------------------------
    void w(char v) { writeNumber<unsigned char>(static_cast<unsigned char>(v)); }
    void w(signed char v) { writeNumber<signed char>(v); }
    void w(unsigned char v) { writeNumber<unsigned char>(v); }
    void w(bool v) {
        const unsigned char b = v ? 1 : 0;
        if (mode_ == Mode::kWrite) buffer_->append(&b, 1);
    }
    void w(short v) { writeNumber<short>(v); }
    void w(unsigned short v) { writeNumber<unsigned short>(v); }
    void w(int v) { writeNumber<int>(v); }
    void w(unsigned int v) { writeNumber<unsigned int>(v); }
    void w(long v) { writeNumber<std::int64_t>(static_cast<std::int64_t>(v)); }
    void w(unsigned long v) { writeNumber<std::uint64_t>(static_cast<std::uint64_t>(v)); }
    void w(long long v) { writeNumber<std::int64_t>(static_cast<std::int64_t>(v)); }
    void w(unsigned long long v) { writeNumber<std::uint64_t>(static_cast<std::uint64_t>(v)); }
    void w(float v) {
        if (mode_ != Mode::kWrite) return;
        const float be = htonF32(v);
        buffer_->append(&be, sizeof(be));
    }
    void w(double v) {
        if (mode_ != Mode::kWrite) return;
        const double be = htonF64(v);
        buffer_->append(&be, sizeof(be));
    }
    // wchar_t on the client is a 16 bit code unit that KSerializer copies verbatim with
    // WriteBytes(), i.e. little endian UTF-16LE on x86 regardless of the network byte order used
    // for the numeric primitives. This is a deliberate exception to the big endian rule.
    void w(char16_t v) {
        if (mode_ != Mode::kWrite) return;
        const std::uint16_t u = static_cast<std::uint16_t>(v);
        const byte le[2] = {static_cast<byte>(u & 0xFF), static_cast<byte>((u >> 8) & 0xFF)};
        buffer_->append(le, 2);
    }
    void w(const String& v) { w(std::string_view(v)); }
    void w(const char* v) { w(std::string_view(v ? v : "")); }
    void w(std::string_view v) {
        if (mode_ != Mode::kWrite) return;
        writeCount(v.size());
        if (!v.empty()) buffer_->append(v.data(), v.size());
    }
    void w(const WideString& v) {
        if (mode_ != Mode::kWrite) return;
        writeCount(v.size() * 2);
        for (char16_t c : v) w(c);
    }
    void w(const char16_t* v) { w(WideString(v ? v : u"")); }
    void w(const SerBuffer& v);

    template <class T>
    void w(const std::vector<T>& v) {
        writeCount(v.size());
        for (const auto& e : v) writeOne(e);
    }
    template <class T>
    void w(const std::set<T>& v) {
        writeCount(v.size());
        for (const auto& e : v) writeOne(e);
    }
    template <class T>
    void w(const std::list<T>& v) {
        writeCount(v.size());
        for (const auto& e : v) writeOne(e);
    }
    template <class T>
    void w(const std::deque<T>& v) {
        writeCount(v.size());
        for (const auto& e : v) writeOne(e);
    }
    template <class K, class V>
    void w(const std::map<K, V>& v) {
        writeCount(v.size());
        for (const auto& kv : v) {
            writeOne(kv.first);
            writeOne(kv.second);
        }
    }
    template <class A, class B>
    void w(const std::pair<A, B>& v) {
        writeOne(v.first);
        writeOne(v.second);
    }

    // ---- read overload set ----------------------------------------------
    void r(char& v) {
        unsigned char b = 0;
        readNumber<unsigned char>(b);
        v = static_cast<char>(b);
    }
    void r(signed char& v) { readNumber<signed char>(v); }
    void r(unsigned char& v) { readNumber<unsigned char>(v); }
    void r(bool& v) {
        unsigned char b = 0;
        if (!getBytes(&b, 1)) {
            v = false;
            return;
        }
        v = (b == 1);
    }
    void r(short& v) { readNumber<short>(v); }
    void r(unsigned short& v) { readNumber<unsigned short>(v); }
    void r(int& v) { readNumber<int>(v); }
    void r(unsigned int& v) { readNumber<unsigned int>(v); }
    void r(long& v) {
        std::int64_t t = 0;
        readNumber<std::int64_t>(t);
        v = static_cast<long>(t);
    }
    void r(unsigned long& v) {
        std::uint64_t t = 0;
        readNumber<std::uint64_t>(t);
        v = static_cast<unsigned long>(t);
    }
    void r(long long& v) {
        std::int64_t t = 0;
        readNumber<std::int64_t>(t);
        v = static_cast<long long>(t);
    }
    void r(unsigned long long& v) {
        std::uint64_t t = 0;
        readNumber<std::uint64_t>(t);
        v = static_cast<unsigned long long>(t);
    }
    void r(float& v) {
        float be = 0.f;
        if (!getBytes(&be, sizeof(be))) {
            v = 0.f;
            return;
        }
        v = ntohF32(be);
    }
    void r(double& v) {
        double be = 0.0;
        if (!getBytes(&be, sizeof(be))) {
            v = 0.0;
            return;
        }
        v = ntohF64(be);
    }
    void r(char16_t& v) {
        byte le[2] = {0, 0};
        if (!getBytes(le, 2)) {
            v = 0;
            return;
        }
        v = static_cast<char16_t>(static_cast<std::uint16_t>(le[0] | (le[1] << 8)));
    }
    void r(String& v);
    void r(WideString& v);
    void r(SerBuffer& v);

    template <class T>
    void r(std::vector<T>& v) {
        v.clear();
        const std::uint32_t n = readCount();
        v.reserve(n);
        for (std::uint32_t i = 0; i < n; ++i) {
            T e{};
            readOne(e);
            if (failed()) return;
            v.push_back(std::move(e));
        }
    }
    template <class T>
    void r(std::set<T>& v) {
        v.clear();
        const std::uint32_t n = readCount();
        for (std::uint32_t i = 0; i < n; ++i) {
            T e{};
            readOne(e);
            if (failed()) return;
            v.insert(std::move(e));
        }
    }
    template <class T>
    void r(std::list<T>& v) {
        v.clear();
        const std::uint32_t n = readCount();
        for (std::uint32_t i = 0; i < n; ++i) {
            T e{};
            readOne(e);
            if (failed()) return;
            v.push_back(std::move(e));
        }
    }
    template <class T>
    void r(std::deque<T>& v) {
        v.clear();
        const std::uint32_t n = readCount();
        for (std::uint32_t i = 0; i < n; ++i) {
            T e{};
            readOne(e);
            if (failed()) return;
            v.push_back(std::move(e));
        }
    }
    template <class K, class V>
    void r(std::map<K, V>& v) {
        v.clear();
        const std::uint32_t n = readCount();
        for (std::uint32_t i = 0; i < n; ++i) {
            K k{};
            V val{};
            readOne(k);
            readOne(val);
            if (failed()) return;
            v.emplace(std::move(k), std::move(val));
        }
    }
    template <class A, class B>
    void r(std::pair<A, B>& v) {
        readOne(v.first);
        readOne(v.second);
    }

    // ---- dispatch --------------------------------------------------------
    template <class T>
    void writeOne(const T& v) {
        if constexpr (std::is_enum_v<T>) {
            writeNumber<std::underlying_type_t<T>>(static_cast<std::underlying_type_t<T>>(v));
        } else if constexpr (MemberSerializable<T>) {
            const_cast<T&>(v).serialize(*this);
        } else if constexpr (requires { serialize(*this, const_cast<T&>(v)); }) {
            serialize(*this, const_cast<T&>(v));
        } else {
            w(v);
        }
    }

    template <class T>
    void readOne(T& v) {
        if constexpr (std::is_enum_v<T>) {
            std::underlying_type_t<T> raw{};
            readNumber<std::underlying_type_t<T>>(raw);
            v = static_cast<T>(raw);
        } else if constexpr (MemberSerializable<T>) {
            v.serialize(*this);
        } else if constexpr (requires { serialize(*this, v); }) {
            serialize(*this, v);
        } else {
            r(v);
        }
    }

    SerBuffer* buffer_ = nullptr;
    Mode mode_ = Mode::kIdle;
    bool tagging_ = false;
    bool lenient_ = false;
    Failure failure_ = Failure::kOk;
};

// Convenience helpers.
template <class T>
SerBuffer toSerialized(const T& value) {
    SerBuffer buffer;
    Serializer ser;
    ser.beginWriting(&buffer);
    ser.put(value);
    ser.end();
    return buffer;
}

template <class T>
bool fromSerialized(const SerBuffer& buffer, T& value) {
    SerBuffer copy = buffer;
    Serializer ser;
    ser.beginReading(&copy);
    ser.get(value);
    ser.end();
    return ser.ok();
}

}  // namespace knc
