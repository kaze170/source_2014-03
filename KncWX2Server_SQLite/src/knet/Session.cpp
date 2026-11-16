#include "knet/Session.h"

#include <algorithm>
#include <cstring>

#include "kcore/Log.h"
#include "kcore/Serialize.h"
#include "ksec/SecureBuffer.h"

namespace knc::net {
namespace {
constexpr std::size_t kLengthFieldSize = sizeof(std::uint16_t);
constexpr std::size_t kMaxFrameSize = 64 * 1024;  // uint16 length field caps this anyway
constexpr std::size_t kReadChunk = 16 * 1024;
}  // namespace

Session::Session(SocketHandle sock, std::string peerIp, unsigned short peerPort, EventSink& sink, Tier tier,
                 std::string name)
    : sock_(sock), peerIp_(std::move(peerIp)), peerPort_(peerPort), sink_(sink), tier_(tier), name_(std::move(name)) {
    inBuf_.reserve(kReadChunk * 2);
}

Session::~Session() {
    if (sock_ != kInvalidSocket) {
        closeSocket(sock_);
        sock_ = kInvalidSocket;
    }
    if (spi_ != 0) sec::SecurityDatabase::instance().remove(spi_);
}

void Session::onAcceptConnection() {
    auto& db = sec::SecurityDatabase::instance();
    const SpiType newSpi = db.createNewSA();

    // The association must be serialised while it is still reachable by its SPI; send() seals with
    // the current (SPI 0) association, so build the payload first and send afterwards.
    const sec::SecurityAssociation& association = db.get(newSpi);

    SerBuffer payload;
    Serializer ser;
    ser.beginWriting(&payload);
    ser.put(newSpi);
    ser.put(const_cast<sec::SecurityAssociation&>(association));
    ser.end();

    Event ev;
    ev.setData(kPiGsProxy, nullptr, E_ACCEPT_CONNECTION_NOT);
    ev.payload = payload;
    if (!send(ev)) {
        close("handshake send failed");
        return;
    }
    spi_ = newSpi;
    accepted_ = true;
    KLOG(LogLevel::kDebug, "session") << name_ << " handshake sent spi=" << newSpi;
}

bool Session::send(const Event& event) {
    if (closed_) return false;

    SerBuffer serialised;
    Serializer ser;
    ser.beginWriting(&serialised);
    ser.put(const_cast<Event&>(event));
    ser.end();

    sec::SecureBuffer secure(spi_);
    if (!secure.create_notRWM(serialised.data(), serialised.size())) {
        KLOG(LogLevel::kError, "session") << name_ << " encrypt failed";
        return false;
    }

    const std::size_t total = kLengthFieldSize + secure.size();
    if (total > kMaxFrameSize) {
        KLOG(LogLevel::kError, "session") << name_ << " frame too large: " << total;
        return false;
    }

    std::vector<byte> frame(total);
    frame[0] = static_cast<byte>(total & 0xFF);
    frame[1] = static_cast<byte>((total >> 8) & 0xFF);
    std::memcpy(frame.data() + kLengthFieldSize, secure.dataPtr(), secure.size());

    outQueue_.push_back(std::move(frame));
    ++framesOut_;
    flush();
    return true;
}

bool Session::sendEvent(UINT32 toPerformer, UidType toUid, EventId id) {
    Event ev;
    ev.setData(toPerformer, nullptr, id);
    if (toUid != kInvalidUid) ev.destPerformer.addUid(toUid);
    return send(ev);
}

void Session::flush() {
    while (!outQueue_.empty()) {
        auto& frame = outQueue_.front();
        const int sent = sendSome(sock_, frame.data() + outOffset_, frame.size() - outOffset_);
        if (sent > 0) {
            outOffset_ += static_cast<std::size_t>(sent);
            bytesOut_ += static_cast<UINT64>(sent);
            if (outOffset_ >= frame.size()) {
                outQueue_.pop_front();
                outOffset_ = 0;
            }
            continue;
        }
        if (sent == -1) return;  // would block, wait for POLLOUT
        close("send error");
        return;
    }
}

void Session::onReadable(Reactor& reactor) {
    (void)reactor;
    if (closed_) return;
    std::vector<byte> chunk(kReadChunk);
    for (;;) {
        const int n = recvSome(sock_, chunk.data(), chunk.size());
        if (n > 0) {
            bytesIn_ += static_cast<UINT64>(n);
            feed(chunk.data(), static_cast<std::size_t>(n));
            if (closed_) return;
            if (static_cast<std::size_t>(n) < chunk.size()) continue;
            continue;
        }
        if (n == 0) {
            close("peer closed");
            return;
        }
        if (n == -1) return;  // drained
        close("recv error");
        return;
    }
}

void Session::onWritable(Reactor& reactor) {
    (void)reactor;
    if (closed_) return;
    flush();
}

void Session::onClose(Reactor& reactor) {
    (void)reactor;
    if (closed_) return;
    closed_ = true;
    if (sock_ != kInvalidSocket) {
        closeSocket(sock_);
        sock_ = kInvalidSocket;
    }
    if (spi_ != 0) {
        sec::SecurityDatabase::instance().remove(spi_);
        spi_ = 0;
    }
}

void Session::close(const std::string& reason) {
    if (closed_) return;
    KLOG(LogLevel::kDebug, "session") << name_ << " closing: " << reason;
    closed_ = true;
    if (sock_ != kInvalidSocket) {
        closeSocket(sock_);
        sock_ = kInvalidSocket;
    }
    if (spi_ != 0) {
        sec::SecurityDatabase::instance().remove(spi_);
        spi_ = 0;
    }
    sink_.onSessionClosed(*this, reason.c_str());
}

void Session::feed(const byte* data, std::size_t len) {
    // Compact occasionally so the buffer does not grow without bound.
    if (inOffset_ > 0 && inOffset_ >= inBuf_.size() / 2) {
        inBuf_.erase(inBuf_.begin(), inBuf_.begin() + static_cast<std::ptrdiff_t>(inOffset_));
        inOffset_ = 0;
    }
    inBuf_.insert(inBuf_.end(), data, data + len);
    processFrames();
}

void Session::processFrames() {
    while (!closed_ && processOneFrame()) {
    }
}

bool Session::processOneFrame() {
    const std::size_t available = inBuf_.size() - inOffset_;
    if (available < kLengthFieldSize) return false;

    std::uint16_t frameLen = 0;
    std::memcpy(&frameLen, inBuf_.data() + inOffset_, kLengthFieldSize);
    // The length field itself is host order little endian in the original code (memcpy of a USHORT).
    if (frameLen < kLengthFieldSize || frameLen > kMaxFrameSize) {
        close("bad frame length");
        return false;
    }
    if (available < frameLen) return false;

    const byte* secureData = inBuf_.data() + inOffset_ + kLengthFieldSize;
    const std::size_t secureLen = frameLen - kLengthFieldSize;

    sec::SecureBuffer secure(spi_, secureData, secureLen);
    if (!secure.isAuthentic_notRWM(checkSequenceNum_)) {
        failAuth("packet authentication failed");
        return false;
    }

    Bytes payload;
    if (!secure.getPayload_notRWM(payload, false, checkSequenceNum_)) {
        failAuth("packet decryption failed");
        return false;
    }
    inOffset_ += frameLen;
    ++framesIn_;

    SerBuffer eventBuffer;
    eventBuffer.assign(payload.data(), payload.size());
    Event event;
    {
        Serializer ser;
        ser.beginReading(&eventBuffer);
        ser.get(event);
        ser.end();
        if (!ser.ok()) {
            failAuth("event deserialise failed");
            return false;
        }
    }
    // The payload is compressed independently of the envelope.
    event.payload.uncompress();
    sink_.onEvent(*this, event);
    return true;
}

void Session::failAuth(const char* why) {
    ++authFailCount_;
    KLOG(LogLevel::kWarn, "session") << name_ << " " << why << " (count=" << authFailCount_ << ')';
    if (authFailCount_ > authFailLimit_) {
        close("packet authentication failure limit reached");
        return;
    }
    close(why);
}

}  // namespace knc::net
