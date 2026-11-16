// tests/TestNet.cpp - end to end tests over real sockets: handshake, framing, event dispatch.
#if defined(_WIN32)
#include <winsock2.h>
#include <ws2tcpip.h>
#else
#include <arpa/inet.h>
#include <netinet/in.h>
#include <sys/socket.h>
#include <unistd.h>
#endif

#include <algorithm>
#include <atomic>
#include <chrono>
#include <thread>
#include <vector>

#include "TestFramework.h"
#include "kcore/Serialize.h"
#include "knet/Listener.h"
#include "knet/Reactor.h"
#include "kproto/Event.h"
#include "kproto/Packet.h"
#include "ksec/SecureBuffer.h"

using namespace knc;
using namespace knc::net;

namespace {

struct Recorder : EventSink {
    std::mutex mutex;
    std::vector<EventId> ids;
    std::vector<std::string> payloads;
    std::atomic<int> closed{0};

    void onEvent(Session& session, Event& event) override {
        (void)session;
        std::lock_guard<std::mutex> guard(mutex);
        ids.push_back(event.eventId);
        payloads.emplace_back(reinterpret_cast<const char*>(event.payload.data()), event.payload.size());
    }
    void onSessionClosed(Session& session, const char* reason) override {
        (void)session;
        (void)reason;
        closed.fetch_add(1);
    }
    bool waitClosed(int timeoutMs = 2000) {
        const auto deadline = std::chrono::steady_clock::now() + std::chrono::milliseconds(timeoutMs);
        while (std::chrono::steady_clock::now() < deadline) {
            if (closed.load() > 0) return true;
            std::this_thread::sleep_for(std::chrono::milliseconds(5));
        }
        return false;
    }

    bool waitFor(std::size_t count, int timeoutMs = 2000) {
        const auto deadline = std::chrono::steady_clock::now() + std::chrono::milliseconds(timeoutMs);
        while (std::chrono::steady_clock::now() < deadline) {
            {
                std::lock_guard<std::mutex> guard(mutex);
                if (ids.size() >= count) return true;
            }
            std::this_thread::sleep_for(std::chrono::milliseconds(5));
        }
        return false;
    }
};

// Builds a framed packet: [u16_le length][sealed payload]. Every test client needs this, and doing
// it by hand trips -Warray-bounds because the compiler cannot see through the vector growth.
std::vector<byte> makeFrame(const sec::SecureBuffer& secure) {
    const std::size_t total = 2 + secure.size();
    std::vector<byte> frame(total);
    frame[0] = static_cast<byte>(total & 0xff);
    frame[1] = static_cast<byte>((total >> 8) & 0xff);
    std::copy_n(secure.dataPtr(), secure.size(), frame.begin() + 2);
    return frame;
}

// Minimal client: shares the SecurityAssociation database with the server (same process), which
// is exactly how the original server-side integration tests worked.
class TestClient {
public:
    bool connectTo(unsigned short port) {
        sock_ = ::socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
        if (sock_ == kInvalidSocket) return false;
        sockaddr_in addr{};
        addr.sin_family = AF_INET;
        addr.sin_port = htons(port);
        addr.sin_addr.s_addr = htonl(INADDR_LOOPBACK);
        if (::connect(sock_, reinterpret_cast<sockaddr*>(&addr), sizeof(addr)) != 0) return false;
        return true;
    }

    // Reads the E_ACCEPT_CONNECTION_NOT packet and adopts the SPI + association.
    bool handshake(int timeoutMs = 2000) {
        const auto deadline = std::chrono::steady_clock::now() + std::chrono::milliseconds(timeoutMs);
        while (std::chrono::steady_clock::now() < deadline) {
            if (readFrame(50)) return true;
        }
        return false;
    }

    bool readFrame(int timeoutMs) {
        const auto deadline = std::chrono::steady_clock::now() + std::chrono::milliseconds(timeoutMs);
        while (std::chrono::steady_clock::now() < deadline) {
            if (inBuf_.size() >= 2) {
                std::uint16_t len = 0;
                std::memcpy(&len, inBuf_.data(), 2);
                if (inBuf_.size() >= len) {
                    std::vector<byte> frame(inBuf_.begin() + 2, inBuf_.begin() + len);
                    inBuf_.erase(inBuf_.begin(), inBuf_.begin() + len);
                    lastFrame_ = std::move(frame);
                    return true;
                }
            }
            byte chunk[4096];
            const int n = ::recv(sock_, reinterpret_cast<char*>(chunk), sizeof(chunk), 0);
            if (n <= 0) return false;
            inBuf_.insert(inBuf_.end(), chunk, chunk + n);
        }
        return false;
    }

    // Decrypt the last frame with the given SPI and decode the KEvent inside.
    bool decodeLast(SpiType spi, Event& out, bool checkSeq = false) {
        sec::SecureBuffer buffer(spi, lastFrame_.data(), lastFrame_.size());
        if (!buffer.isAuthentic_notRWM(checkSeq)) return false;
        Bytes payload;
        if (!buffer.getPayload_notRWM(payload, false, checkSeq)) return false;
        SerBuffer buf;
        buf.assign(payload.data(), payload.size());
        Serializer s;
        s.beginReading(&buf);
        s.get(out);
        s.end();
        out.payload.uncompress();
        return s.ok();
    }

    // Sends an event sealed with `spi`.
    bool sendEvent(SpiType spi, Event ev) {
        SerBuffer wire;
        Serializer s;
        s.beginWriting(&wire);
        s.put(ev);
        s.end();
        sec::SecureBuffer secure(spi);
        if (!secure.create_notRWM(wire.data(), wire.size())) return false;
        const std::vector<byte> frame = makeFrame(secure);
        std::size_t off = 0;
        while (off < frame.size()) {
            const auto n = ::send(sock_, reinterpret_cast<const char*>(frame.data() + off),
                                  static_cast<int>(frame.size() - off), 0);
            if (n <= 0) return false;
            off += static_cast<std::size_t>(n);
        }
        return true;
    }

    // Raw byte send, bypassing framing/sealing (used to probe malformed input handling).
    bool sendRaw(const void* data, std::size_t len) {
        std::size_t off = 0;
        while (off < len) {
            const auto n = ::send(sock_, static_cast<const char*>(data) + off, static_cast<int>(len - off), 0);
            if (n <= 0) return false;
            off += static_cast<std::size_t>(n);
        }
        return true;
    }

    SocketHandle rawHandle() const { return sock_; }

    void close() {
        if (sock_ != kInvalidSocket) {
            ::knc::net::closeSocket(sock_);
            sock_ = kInvalidSocket;
        }
    }
    ~TestClient() { close(); }

    SpiType spi = 0;
    std::vector<byte> lastFrame_;

private:
    SocketHandle sock_ = kInvalidSocket;
    std::vector<byte> inBuf_;
};

// Tiny harness that runs a reactor on a background thread.
class ServerHarness {
public:
    ServerHarness(unsigned short port) : reactor_("test"), sink_() {
        ListenerConfig cfg;
        cfg.name = "test";
        cfg.port = port;
        cfg.clientTier = true;
        listener_ = std::make_shared<Listener>(cfg, reactor_, sink_);
    }
    ~ServerHarness() { stop(); }

    bool start() {
        if (!listener_->start()) return false;
        thread_ = std::thread([this] { reactor_.run(); });
        return true;
    }
    void stop() {
        reactor_.stop();
        if (thread_.joinable()) thread_.join();
    }
    Reactor& reactor() { return reactor_; }
    Recorder& sink() { return sink_; }
    Listener& listener() { return *listener_; }

private:
    Reactor reactor_;
    Recorder sink_;
    std::shared_ptr<Listener> listener_;
    std::thread thread_;
};

unsigned short pickPort() {
    // Bind to port 0 and read back the assigned port.
    const SocketHandle s = ::socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
    sockaddr_in addr{};
    addr.sin_family = AF_INET;
    addr.sin_port = 0;
    addr.sin_addr.s_addr = htonl(INADDR_LOOPBACK);
    ::bind(s, reinterpret_cast<sockaddr*>(&addr), sizeof(addr));
#if defined(_WIN32)
    int len = sizeof(addr);
    ::getsockname(s, reinterpret_cast<sockaddr*>(&addr), &len);
    const unsigned short port = ntohs(addr.sin_port);
    closesocket(s);
#else
    socklen_t len = sizeof(addr);
    ::getsockname(s, reinterpret_cast<sockaddr*>(&addr), &len);
    const unsigned short port = ntohs(addr.sin_port);
    ::close(s);
#endif
    return port;
}

}  // namespace

KTEST(Net, handshakeDeliversSpiAndAssociation) {
    KCHECK(net::startupSockets());
    const unsigned short port = pickPort();
    ServerHarness server(port);
    KCHECK(server.start());

    TestClient client;
    KCHECK(client.connectTo(port));
    KCHECK(client.handshake());

    // The first packet is sealed with SPI 0 and carries the new SPI + association.
    Event ev;
    KCHECK(client.decodeLast(0, ev, false));
    KCHECK_EQ(ev.eventId, E_ACCEPT_CONNECTION_NOT);

    SpiType newSpi = 0;
    sec::SecurityAssociation sa;
    {
        SerBuffer copy = ev.payload;
        Serializer s;
        s.beginReading(&copy);
        s.get(newSpi, sa);
        s.end();
        KCHECK(s.ok());
    }
    KCHECK(newSpi >= 1);
    KCHECK(sec::SecurityDatabase::instance().contains(newSpi));
    // The association carries the randomized keys the client will use from now on.
    KCHECK(std::memcmp(sa.authKey().data(), sec::SecurityDatabase::instance().get(newSpi).authKey().data(), 8) == 0);

    // Now the client can send a sealed event with the new SPI and the server must dispatch it.
    Event ping;
    ping.setData(kPiGsProxy, nullptr, E_HEART_BEAT);
    KCHECK(client.sendEvent(newSpi, ping));
    KCHECK(server.sink().waitFor(1));
    {
        std::lock_guard<std::mutex> guard(server.sink().mutex);
        KCHECK_EQ(server.sink().ids[0], E_HEART_BEAT);
    }
    server.stop();
}

KTEST(Net, serverSendsEventSealedWithSessionSpi) {
    KCHECK(net::startupSockets());
    const unsigned short port = pickPort();
    ServerHarness server(port);
    KCHECK(server.start());

    TestClient client;
    KCHECK(client.connectTo(port));
    KCHECK(client.handshake());
    Event accept;
    KCHECK(client.decodeLast(0, accept, false));
    SpiType spi = 0;
    {
        SerBuffer copy = accept.payload;
        Serializer s;
        s.beginReading(&copy);
        sec::SecurityAssociation sa;
        s.get(spi, sa);
        s.end();
    }
    KCHECK(spi >= 1);

    // Ask the harness listener for the live session and push a packet to the client.
    server.listener().pruneSessions();
    KCHECK_EQ(server.listener().sessions().size(), std::size_t(1));
    auto session = server.listener().sessions()[0];

    PacketOk ok;
    ok.ok = 1;
    KCHECK(session->sendEvent(kPiGsProxy, session->uid(), EGS_CONNECT_ACK, ok));

    Event received;
    KCHECK(client.readFrame(2000));
    KCHECK(client.decodeLast(spi, received, false));
    KCHECK_EQ(received.eventId, EGS_CONNECT_ACK);
    PacketOk decoded;
    KCHECK(received.getPayload(decoded));
    KCHECK_EQ(decoded.ok, 1);
    server.stop();
}

KTEST(Net, badCiphertextClosesSession) {
    KCHECK(net::startupSockets());
    const unsigned short port = pickPort();
    ServerHarness server(port);
    KCHECK(server.start());

    TestClient client;
    KCHECK(client.connectTo(port));
    KCHECK(client.handshake());
    Event accept;
    KCHECK(client.decodeLast(0, accept, false));
    SpiType spi = 0;
    {
        SerBuffer copy = accept.payload;
        Serializer s;
        s.beginReading(&copy);
        sec::SecurityAssociation sa;
        s.get(spi, sa);
        s.end();
    }
    KCHECK(spi >= 1);

    Event ev;
    ev.setData(kPiGsProxy, nullptr, E_HEART_BEAT);
    SerBuffer wire;
    Serializer s;
    s.beginWriting(&wire);
    s.put(ev);
    s.end();
    sec::SecureBuffer secure(spi);
    KCHECK(secure.create_notRWM(wire.data(), wire.size()));

    std::vector<byte> frame = makeFrame(secure);
    frame[2 + 12] ^= 0xFF;  // flip a ciphertext bit -> ICV mismatch on the server

    KCHECK(client.sendRaw(frame.data(), frame.size()));
    KCHECK(server.sink().waitClosed(1500));  // session close is reported through the sink
    KCHECK_EQ(server.sink().closed.load(), 1);
    {
        std::lock_guard<std::mutex> guard(server.sink().mutex);
        KCHECK_EQ(server.sink().ids.size(), std::size_t(0));  // nothing was dispatched
    }
    server.stop();
}

KTEST(Net, partialFrameWaitsForTheRest) {
    KCHECK(net::startupSockets());
    const unsigned short port = pickPort();
    ServerHarness server(port);
    KCHECK(server.start());

    TestClient client;
    KCHECK(client.connectTo(port));
    KCHECK(client.handshake());
    Event accept;
    KCHECK(client.decodeLast(0, accept, false));
    SpiType spi = 0;
    {
        SerBuffer copy = accept.payload;
        Serializer s;
        s.beginReading(&copy);
        sec::SecurityAssociation sa;
        s.get(spi, sa);
        s.end();
    }

    Event ev;
    ev.setData(kPiGsProxy, nullptr, E_HEART_BEAT);
    SerBuffer wire;
    Serializer s;
    s.beginWriting(&wire);
    s.put(ev);
    s.end();
    sec::SecureBuffer secure(spi);
    KCHECK(secure.create_notRWM(wire.data(), wire.size()));

    std::vector<byte> frame(2 + secure.size());
    const std::uint16_t total = static_cast<std::uint16_t>(frame.size());
    std::memcpy(frame.data(), &total, 2);
    std::memcpy(frame.data() + 2, secure.dataPtr(), secure.size());

    // Send the first 6 bytes only and give the server time to notice it is incomplete.
    KCHECK(client.sendRaw(frame.data(), 6));
    std::this_thread::sleep_for(std::chrono::milliseconds(120));
    {
        std::lock_guard<std::mutex> guard(server.sink().mutex);
        KCHECK_EQ(server.sink().ids.size(), std::size_t(0));
    }
    // Then the remainder: the frame must be dispatched exactly once.
    KCHECK(client.sendRaw(frame.data() + 6, frame.size() - 6));
    KCHECK(server.sink().waitFor(1));
    {
        std::lock_guard<std::mutex> guard(server.sink().mutex);
        KCHECK_EQ(server.sink().ids.size(), std::size_t(1));
        KCHECK_EQ(server.sink().ids[0], E_HEART_BEAT);
    }
    server.stop();
}

KTEST(Net, oversizedLengthFieldClosesConnection) {
    KCHECK(net::startupSockets());
    const unsigned short port = pickPort();
    ServerHarness server(port);
    KCHECK(server.start());

    TestClient client;
    KCHECK(client.connectTo(port));
    KCHECK(client.handshake());

    const std::uint16_t bogus = 1;  // smaller than the length field itself
    byte header[2];
    std::memcpy(header, &bogus, 2);
    KCHECK(client.sendRaw(header, 2));
    KCHECK(server.sink().waitClosed(1500));
    KCHECK_EQ(server.sink().closed.load(), 1);
    server.stop();
}
