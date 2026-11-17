// ---------------------------------------------------------------------------
// knc.proto.serbuffer - byte-exact replacement for KNCSDK's KSerBuffer.
//
// Source of truth: KNCSDK/Include/Serializer/{SerBuffer.h,SerBuffer.cpp}.
//
// A KSerBuffer is the opaque per-event payload carried inside a KEvent. Its
// serialized form is:
//
//     DWORD  length            (big-endian, from KSerializer::Put(DWORD))
//     if length > 0:
//         bool   compressed    (1 byte)
//         byte   data[length]  (PutRaw - no further length prefix)
//
// When `compressed` is set, `data` is NOT a bare zlib stream. KSerBuffer::
// Compress() lays it out as:
//
//     DWORD  originalSize      (NATIVE LITTLE-ENDIAN - written with a raw
//                               memcpy via Write(&dwOriginSize, 4), so it is
//                               *not* byte-swapped like the outer length)
//     byte   zlibStream[...]   (zlib format, i.e. compress2() with a 2-byte
//                               0x78 header and an Adler-32 trailer, level 1)
//
// That mixed endianness inside a single structure is a genuine property of the
// 2014 format, not a transcription slip. It is pinned by tests.
// ---------------------------------------------------------------------------
#ifdef KNC_CLASSIC
#  pragma once
#else
module;
#endif

#include "../knc_prelude.hpp"
#include <zlib.h>

#ifdef KNC_CLASSIC
#  include "../core/types.cppm"
#  include "serializer.cppm"
#else
export module knc.proto.serbuffer;
import knc.core.types;
import knc.proto.serializer;
#endif

KNC_EXPORT namespace knc::proto {

/// Compression level used by KSerBuffer::Compress(): `compress2(..., 1)`.
inline constexpr int kSerBufferCompressionLevel = 1;

/// Refuse to inflate anything claiming to expand beyond this. The legacy
/// UnCompress() trusted the embedded originalSize and resized a vector to it,
/// which is a one-packet remote OOM. 64 MiB is ~3 orders of magnitude above
/// the largest real payload (bulk inventory/ranking lists).
inline constexpr std::uint32_t kMaxDecompressedBytes = 64u * 1024u * 1024u;

/// The opaque payload blob carried by an Event.
class SerBuffer {
public:
    SerBuffer() = default;
    explicit SerBuffer(Bytes data) : data_(std::move(data)) {}

    [[nodiscard]] const Bytes& data() const noexcept { return data_; }
    [[nodiscard]] Bytes& data() noexcept { return data_; }
    [[nodiscard]] bool compressed() const noexcept { return compressed_; }
    [[nodiscard]] bool empty() const noexcept { return data_.empty(); }
    [[nodiscard]] std::size_t size() const noexcept { return data_.size(); }

    void clear() noexcept {
        data_.clear();
        compressed_ = false;
    }

    /// Replace the contents with freshly serialized plaintext.
    void assign(Bytes plain) {
        data_ = std::move(plain);
        compressed_ = false;
    }

    /// A Reader over the *uncompressed* payload. Call decompress() first if
    /// the buffer arrived compressed.
    [[nodiscard]] Reader reader() const {
        if (compressed_) {
            throw ProtocolError{"SerBuffer::reader() on a still-compressed buffer"};
        }
        return Reader{view(data_)};
    }

    // --- compression -------------------------------------------------------

    /// KSerBuffer::Compress(). No-op if already compressed.
    void compress() {
        if (compressed_) {
            return;
        }
        const std::uint32_t original_size = static_cast<std::uint32_t>(data_.size());

        uLongf bound = ::compressBound(static_cast<uLong>(data_.size()));
        Bytes staging(bound);
        const int rc = ::compress2(
            staging.data(), &bound,
            data_.empty() ? reinterpret_cast<const Bytef*>("") : data_.data(),
            static_cast<uLong>(data_.size()), kSerBufferCompressionLevel);
        if (rc != Z_OK) {
            throw ProtocolError{"zlib compress2 failed"};
        }
        staging.resize(bound);

        Bytes out;
        out.reserve(4 + staging.size());
        // NATIVE little-endian, matching Write(&dwOriginSize, sizeof(DWORD)).
        out.push_back(static_cast<std::uint8_t>(original_size));
        out.push_back(static_cast<std::uint8_t>(original_size >> 8));
        out.push_back(static_cast<std::uint8_t>(original_size >> 16));
        out.push_back(static_cast<std::uint8_t>(original_size >> 24));
        out.insert(out.end(), staging.begin(), staging.end());

        data_ = std::move(out);
        compressed_ = true;
    }

    /// KSerBuffer::UnCompress(). No-op if not compressed.
    void decompress() {
        if (!compressed_) {
            return;
        }
        if (data_.size() < 4) {
            throw ProtocolError{"compressed SerBuffer shorter than its size header"};
        }
        const std::uint32_t original_size =
            static_cast<std::uint32_t>(data_[0]) |
            (static_cast<std::uint32_t>(data_[1]) << 8) |
            (static_cast<std::uint32_t>(data_[2]) << 16) |
            (static_cast<std::uint32_t>(data_[3]) << 24);

        if (original_size > kMaxDecompressedBytes) {
            throw ProtocolError{"compressed SerBuffer declares an absurd size"};
        }

        Bytes out(original_size);
        uLongf produced = original_size;
        const int rc = ::uncompress(
            out.empty() ? reinterpret_cast<Bytef*>(&produced) : out.data(), &produced,
            data_.data() + 4, static_cast<uLong>(data_.size() - 4));
        if (rc != Z_OK || produced != original_size) {
            throw ProtocolError{"zlib uncompress failed or produced a short result"};
        }

        data_ = std::move(out);
        compressed_ = false;
    }

    // --- wire format -------------------------------------------------------

    void write_to(Writer& w) const {
        w.put_dword(static_cast<std::uint32_t>(data_.size()));
        if (!data_.empty()) {
            w.put_bool(compressed_);
            w.put_raw(view(data_));
        }
    }

    static SerBuffer read_from(Reader& r) {
        SerBuffer buf;
        const std::uint32_t length = r.get_dword();
        if (length == 0) {
            return buf;
        }
        // Legacy guard: `if (len > serializer.GetReadLenth()) return false;`
        if (length > r.remaining()) {
            throw ProtocolError{"SerBuffer length exceeds bytes remaining"};
        }
        buf.compressed_ = r.get_bool();
        buf.data_ = r.get_raw(length);
        return buf;
    }

private:
    Bytes data_;
    bool compressed_ = false;
};

}  // namespace knc::proto
