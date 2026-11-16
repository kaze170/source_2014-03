// kcore/SerBuffer.h - byte buffer with the original KSerBuffer compression layout.
#pragma once

#include "kcore/Types.h"

namespace knc {

// Reproduces KncSDK/Include/Serializer/SerBuffer.{h,cpp}.
//
// Wire representation (KSerBuffer is serialised inline inside KEvent):
//     [DWORD_BE length]
//     if length > 0:
//         [bool compressed]
//         [length raw bytes]
//
// When compressed == true the raw bytes are
//     [DWORD_LE uncompressedSize][zlib deflate stream, level 1]
// Note the interior size header is host order (little endian), exactly like the original
// implementation which wrote `dwOriginSize` with memcpy.
class SerBuffer {
public:
    SerBuffer() = default;

    // Raw access -----------------------------------------------------------
    byte* data() { return buf_.data(); }
    const byte* data() const { return buf_.data(); }
    std::size_t size() const { return buf_.size(); }
    bool empty() const { return buf_.empty(); }
    std::size_t readOffset() const { return readOffset_; }
    std::size_t readRemaining() const { return buf_.size() - readOffset_; }

    void clear();
    void resetRead() { readOffset_ = 0; }
    void assign(const byte* src, std::size_t len);
    void append(const void* src, std::size_t len);
    void appendByte(byte b) { buf_.push_back(b); }

    bool read(void* dst, std::size_t len);
    const byte* peek() const { return buf_.data() + readOffset_; }

    bool compressed() const { return compressed_; }
    void setCompressed(bool on) { compressed_ = on; }

    // Compression ----------------------------------------------------------
    bool compress();
    bool uncompress();

    const std::vector<byte>& raw() const { return buf_; }
    std::vector<byte>& raw() { return buf_; }

private:
    std::vector<byte> buf_;
    std::size_t readOffset_ = 0;
    bool compressed_ = false;
};

}  // namespace knc
