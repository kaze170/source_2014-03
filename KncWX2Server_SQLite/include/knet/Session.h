// knet/Session.h - one X2 TCP connection: framing, KSecureBuffer, KEvent dispatch, out queue.
//
// Framing (Session.cpp:OnRecvCompleted):
//   [uint16 LE totalPacketLen][KSecureBuffer of totalPacketLen - 2 bytes]
// The length includes its own two bytes. Nothing is encrypted before E_ACCEPT_CONNECTION_NOT
// except that packet itself, which is sealed with the SPI 0 default association.
#pragma once

#include <deque>
#include <functional>
#include <memory>
#include <string>
#include <vector>

#include "kcore/SerBuffer.h"
#include "kcore/Types.h"
#include "knet/Reactor.h"
#include "knet/Socket.h"
#include "kproto/Event.h"

namespace knc::net {

class Session;

// Receives fully decoded events.
class EventSink {
public:
    virtual ~EventSink() = default;
    virtual void onEvent(Session& session, Event& event) = 0;
    virtual void onSessionClosed(Session& session, const char* reason) = 0;
};

class Session : public Handler, public std::enable_shared_from_this<Session> {
public:
    enum class Tier {
        kClient,    // a real X2 client on the channel/game/FEP port
        kInternal,  // an internal peer (never happens in the single process build)
    };

    Session(SocketHandle sock, std::string peerIp, unsigned short peerPort, EventSink& sink, Tier tier,
            std::string name);
    ~Session() override;

    // Handler ---------------------------------------------------------------
    SocketHandle handle() const override { return sock_; }
    void onReadable(Reactor& reactor) override;
    void onWritable(Reactor& reactor) override;
    void onClose(Reactor& reactor) override;
    bool wantsWrite() const override { return !outQueue_.empty(); }
    const char* tag() const override { return "session"; }

    // Protocol --------------------------------------------------------------
    // Performs the accept handshake: allocates an SPI, sends E_ACCEPT_CONNECTION_NOT sealed with
    // SPI 0 and switches the session to the new SPI (Session::OnAcceptConnection).
    void onAcceptConnection();

    // Serialises `event`, seals it and queues the frame.
    bool send(const Event& event);
    template <class T>
    bool sendEvent(UINT32 toPerformer, UidType toUid, EventId id, const T& data, bool compress = false) {
        Event ev;
        ev.setData(toPerformer, nullptr, id, data);
        if (toUid != kInvalidUid) ev.destPerformer.addUid(toUid);
        if (compress) ev.compressPayload();
        return send(ev);
    }
    bool sendEvent(UINT32 toPerformer, UidType toUid, EventId id);

    void close(const std::string& reason = "closed");

    UidType uid() const { return uid_; }
    void setUid(UidType uid) { uid_ = uid; }
    const std::string& name() const { return name_; }
    const std::string& peerIp() const { return peerIp_; }
    unsigned short peerPort() const { return peerPort_; }
    std::string endpoint() const { return endpointString(peerIp_, peerPort_); }
    SpiType spi() const { return spi_; }
    Tier tier() const { return tier_; }
    bool closed() const { return closed_; }
    UINT64 bytesIn() const { return bytesIn_; }
    UINT64 bytesOut() const { return bytesOut_; }
    UINT64 framesIn() const { return framesIn_; }
    UINT64 framesOut() const { return framesOut_; }
    std::size_t pendingOut() const { return outQueue_.size(); }

    bool checkSequenceNum() const { return checkSequenceNum_; }
    void setCheckSequenceNum(bool on) { checkSequenceNum_ = on; }
    void setAuthFailLimit(unsigned int limit) { authFailLimit_ = limit; }
    unsigned int authFailCount() const { return authFailCount_; }

private:
    void feed(const byte* data, std::size_t len);
    void processFrames();
    bool processOneFrame();
    void flush();
    void failAuth(const char* why);

    SocketHandle sock_ = kInvalidSocket;
    std::string peerIp_;
    unsigned short peerPort_ = 0;
    EventSink& sink_;
    Tier tier_;
    std::string name_;

    SpiType spi_ = 0;
    UidType uid_ = kInvalidUid;
    bool accepted_ = false;
    bool closed_ = false;
    bool checkSequenceNum_ = false;
    unsigned int authFailLimit_ = 100;
    unsigned int authFailCount_ = 0;

    std::vector<byte> inBuf_;
    std::size_t inOffset_ = 0;
    std::deque<std::vector<byte>> outQueue_;
    std::size_t outOffset_ = 0;

    UINT64 bytesIn_ = 0;
    UINT64 bytesOut_ = 0;
    UINT64 framesIn_ = 0;
    UINT64 framesOut_ = 0;
};

}  // namespace knc::net
