// ---------------------------------------------------------------------------
// knc.proto.framing - the TCP length-prefix layer.
//
// Source of truth: KncWX2Server/Common/Socket/{Session.cpp,Overlapped.h}.
//
//   +----------------+---------------------------------+
//   | USHORT len LE  | KSecureBuffer frame (len - 2 B) |
//   +----------------+---------------------------------+
//
// `len` is the TOTAL frame size *including its own two bytes*, written with
// KByteStream::Assign(value, 2) - raw native little-endian, NOT the
// big-endian the KSerializer uses inside the payload.
//
// The legacy receive loop (KSkSession::OnRecvCompleted) is:
//
//     while( m_dwLeft >= sizeof(usPacketLen) ) {
//         memcpy( &usPacketLen, buffer, 2 );
//         if( m_dwLeft >= usPacketLen ) {
//             ... process bytes [2, usPacketLen) ...
//             m_dwLeft -= usPacketLen;
//             memmove( buffer, buffer + usPacketLen, m_dwLeft );
//         }
//         else if( usPacketLen > MAX_PACKET_SIZE ) { disconnect; }
//         else break;
//     }
//
// Two defects in that loop are fixed here, both remotely triggerable:
//
//   1. usPacketLen < 2 is accepted. `usPacketLen - sizeof(usPacketLen)` is
//      size_t arithmetic, so len==0 yields SIZE_MAX-1 and is handed straight
//      to KByteStream::assign(ptr, nBytes) - an immediate wild read. Worse,
//      `m_dwLeft -= 0` leaves the loop state unchanged, so the server spins
//      forever on the same two bytes. A single 00 00 from any peer is enough.
//   2. The oversize check only runs in the `else` branch. A frame whose
//      declared length exceeds MAX_PACKET_SIZE but which happens to fit in
//      the buffer is processed instead of rejected, and a frame larger than
//      the fixed 32 KiB buffer can never satisfy `m_dwLeft >= usPacketLen`,
//      so the connection stalls until the oversize branch finally fires.
//
// This reader validates the declared length before using it, and never
// advances by zero.
// ---------------------------------------------------------------------------
#ifdef KNC_CLASSIC
#  pragma once
#else
module;
#endif

#include "../knc_prelude.hpp"
#include <deque>

#ifdef KNC_CLASSIC
#  include "../core/types.cppm"
#else
export module knc.proto.framing;
import knc.core.types;
#endif

KNC_EXPORT namespace knc::proto {

/// MAX_PACKET_SIZE from Common/Socket/Overlapped.h (1024 * 32).
inline constexpr std::size_t kMaxPacketSize = 32768;

/// Size of the length prefix itself.
inline constexpr std::size_t kLengthPrefixSize = sizeof(std::uint16_t);

/// Smallest legal frame: the prefix plus a minimum-size secure buffer (25).
/// Kept as a plain number here so that framing does not depend on security.
inline constexpr std::size_t kMinFrameSize = kLengthPrefixSize + 25;

enum class FrameError {
    kOk,
    kLengthTooSmall,   ///< declared length < kMinFrameSize (includes 0 and 1)
    kLengthTooLarge,   ///< declared length > kMaxPacketSize
    kPayloadTooLarge,  ///< outbound body would not fit under kMaxPacketSize
};

[[nodiscard]] inline const char* to_string(FrameError e) noexcept {
    switch (e) {
        case FrameError::kOk: return "ok";
        case FrameError::kLengthTooSmall: return "declared frame length is too small";
        case FrameError::kLengthTooLarge: return "declared frame length exceeds MAX_PACKET_SIZE";
        case FrameError::kPayloadTooLarge: return "payload exceeds MAX_PACKET_SIZE";
    }
    return "unknown";
}

/// Prepend the little-endian total length to an already-sealed secure buffer.
/// Mirrors the tail of KSession::SendPacket.
[[nodiscard]] inline Bytes frame(ByteView secure_buffer) {
    const std::size_t total = secure_buffer.size() + kLengthPrefixSize;
    if (total > kMaxPacketSize) {
        // The legacy code only logged this and sent the packet anyway, which
        // guarantees the peer's 32 KiB receive buffer can never assemble it.
        throw ProtocolError{"outbound frame exceeds MAX_PACKET_SIZE"};
    }
    Bytes out;
    out.reserve(total);
    out.push_back(static_cast<std::uint8_t>(total & 0xFF));
    out.push_back(static_cast<std::uint8_t>((total >> 8) & 0xFF));
    append(out, secure_buffer);
    return out;
}

/// Accumulates bytes from a socket and yields complete frame bodies.
///
/// Replaces the fixed 32 KiB array plus memmove-on-every-packet in
/// KOVERLAPPED. A deque-backed buffer with an offset cursor gives amortised
/// O(1) consumption instead of O(n) per packet.
class FrameReader {
public:
    /// Feed freshly received bytes.
    void push(ByteView data) { append(buf_, data); }

    struct Next {
        bool has_frame = false;
        FrameError error = FrameError::kOk;
        /// The secure-buffer body: the frame WITHOUT its 2-byte length prefix.
        ByteView body;

        [[nodiscard]] bool failed() const noexcept {
            return error != FrameError::kOk;
        }
    };

    /// Pop the next complete frame, if one has arrived.
    ///
    /// The returned `body` points into this reader's internal storage and is
    /// invalidated by the next push()/next() call - consume it before either.
    [[nodiscard]] Next next() {
        Next result;

        compact();
        const std::size_t available = buf_.size() - head_;
        if (available < kLengthPrefixSize) {
            return result;
        }

        const std::size_t declared = static_cast<std::size_t>(buf_[head_]) |
                                     (static_cast<std::size_t>(buf_[head_ + 1]) << 8);

        // Validate BEFORE trusting the value for arithmetic or buffering.
        if (declared > kMaxPacketSize) {
            result.error = FrameError::kLengthTooLarge;
            return result;
        }
        if (declared < kMinFrameSize) {
            result.error = FrameError::kLengthTooSmall;
            return result;
        }
        if (available < declared) {
            return result;  // keep waiting
        }

        result.has_frame = true;
        result.body = ByteView{buf_.data() + head_ + kLengthPrefixSize,
                               declared - kLengthPrefixSize};
        head_ += declared;  // declared >= kMinFrameSize, so always progresses
        return result;
    }

    [[nodiscard]] std::size_t buffered() const noexcept { return buf_.size() - head_; }

    void clear() noexcept {
        buf_.clear();
        head_ = 0;
    }

private:
    /// Drop consumed bytes once they dominate the buffer. Amortised O(1).
    void compact() {
        if (head_ == 0) {
            return;
        }
        if (head_ == buf_.size()) {
            buf_.clear();
            head_ = 0;
            return;
        }
        if (head_ >= buf_.size() / 2) {
            buf_.erase(buf_.begin(), buf_.begin() + static_cast<std::ptrdiff_t>(head_));
            head_ = 0;
        }
    }

    Bytes buf_;
    std::size_t head_ = 0;
};

}  // namespace knc::proto
