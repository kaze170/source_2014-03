#include "kcore/Serialize.h"

namespace knc {

void Serializer::w(const SerBuffer& v) {
    if (mode_ != Mode::kWrite) return;
    const DWORD len = static_cast<DWORD>(v.size());
    writeCount(len);
    if (len == 0) return;
    const bool compressed = v.compressed();
    w(compressed);
    buffer_->append(v.data(), len);
}

void Serializer::r(SerBuffer& v) {
    v.clear();
    DWORD len = 0;
    readNumber<DWORD>(len);
    if (failed()) return;
    if (len == 0) return;
    if (len > readRemaining()) {
        fail(Failure::kBadLength);
        return;
    }
    bool compressed = false;
    r(compressed);
    if (failed()) return;
    v.raw().resize(len);
    if (!buffer_->read(v.raw().data(), len)) {
        fail(Failure::kUnderflow);
        v.clear();
        return;
    }
    v.setCompressed(compressed);
    v.resetRead();
}

void Serializer::r(String& v) {
    v.clear();
    DWORD len = 0;
    readNumber<DWORD>(len);
    if (failed()) return;
    if (len == 0) return;
    if (len > readRemaining()) {
        fail(Failure::kBadLength);
        return;
    }
    v.resize(len);
    if (!getBytes(v.data(), len)) v.clear();
}

void Serializer::r(WideString& v) {
    v.clear();
    DWORD len = 0;
    readNumber<DWORD>(len);
    if (failed()) return;
    if (len == 0) return;
    if ((len % 2) != 0 || len > readRemaining()) {
        fail(Failure::kBadLength);
        return;
    }
    const std::size_t units = len / 2;
    v.resize(units);
    for (std::size_t i = 0; i < units; ++i) {
        char16_t c{};
        r(c);
        v[i] = c;
    }
}

}  // namespace knc
