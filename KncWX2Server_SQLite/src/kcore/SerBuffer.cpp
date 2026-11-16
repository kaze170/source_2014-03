#include "kcore/SerBuffer.h"

#include <cstring>

#include "kcore/Serialize.h"
#include "zlib.h"

namespace knc {

void SerBuffer::clear() {
    buf_.clear();
    readOffset_ = 0;
    compressed_ = false;
}

void SerBuffer::assign(const byte* src, std::size_t len) {
    buf_.assign(src, src + len);
    readOffset_ = 0;
}

void SerBuffer::append(const void* src, std::size_t len) {
    if (len == 0) return;
    const auto* p = static_cast<const byte*>(src);
    buf_.insert(buf_.end(), p, p + len);
}

bool SerBuffer::read(void* dst, std::size_t len) {
    if (len > buf_.size() - readOffset_) return false;
    if (len != 0) std::memcpy(dst, buf_.data() + readOffset_, len);
    readOffset_ += len;
    return true;
}

bool SerBuffer::compress() {
    if (compressed_) return true;
    if (buf_.empty()) return true;

    const std::size_t originSize = buf_.size();
    uLongf bound = compressBound(static_cast<uLong>(originSize));
    std::vector<byte> out(bound);
    const int rc = compress2(out.data(), &bound, buf_.data(), static_cast<uLong>(originSize), 1);
    if (rc != Z_OK) return false;
    out.resize(bound);

    std::vector<byte> rebuilt;
    rebuilt.resize(sizeof(std::uint32_t));
    // host order (little endian) origin size, matching the original memcpy of a DWORD.
    const std::uint32_t le = static_cast<std::uint32_t>(originSize);
    rebuilt[0] = static_cast<byte>(le & 0xFF);
    rebuilt[1] = static_cast<byte>((le >> 8) & 0xFF);
    rebuilt[2] = static_cast<byte>((le >> 16) & 0xFF);
    rebuilt[3] = static_cast<byte>((le >> 24) & 0xFF);
    rebuilt.insert(rebuilt.end(), out.begin(), out.end());

    buf_.swap(rebuilt);
    readOffset_ = 0;
    compressed_ = true;
    return true;
}

bool SerBuffer::uncompress() {
    if (!compressed_) return true;
    if (buf_.size() < sizeof(std::uint32_t)) return false;

    std::uint32_t originSize = 0;
    originSize |= static_cast<std::uint32_t>(buf_[0]);
    originSize |= static_cast<std::uint32_t>(buf_[1]) << 8;
    originSize |= static_cast<std::uint32_t>(buf_[2]) << 16;
    originSize |= static_cast<std::uint32_t>(buf_[3]) << 24;

    std::vector<byte> out(originSize);
    uLongf dstLen = originSize;
    const int rc = ::uncompress(out.data(), &dstLen, buf_.data() + sizeof(std::uint32_t),
                                static_cast<uLong>(buf_.size() - sizeof(std::uint32_t)));
    if (rc != Z_OK || dstLen != originSize) return false;

    buf_.swap(out);
    readOffset_ = 0;
    compressed_ = false;
    return true;
}

}  // namespace knc
