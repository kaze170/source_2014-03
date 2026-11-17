// ---------------------------------------------------------------------------
// knc.net.server - the part that actually owns a socket.
//
// Split in two on purpose:
//
//   Connection  a Session plus an outbound byte queue. No socket calls at
//               all: you hand it bytes that arrived and take bytes to send.
//               Testable without a network, and shared by every transport.
//   TcpServer   a poll() loop that moves bytes between sockets and
//               Connections.
//
// The ship target is MSVC and the original used IOCP, so TcpServer is the
// piece that gets rewritten per platform. Connection - where the protocol
// decisions live - is not.
//
// Replaces: KNetLayer, KSkSession, KOVERLAPPED and the IOCP worker pool
// (Common/NetLayer.cpp, Common/Socket/*). The legacy design gave every
// session a 32 KiB fixed receive array and memmove'd the remainder after
// every packet; FrameReader already fixed that.
// ---------------------------------------------------------------------------
#ifdef KNC_CLASSIC
#  pragma once
#else
module;
#endif

#include "../knc_prelude.hpp"

#include <cstdint>
#include <cstring>
#include <deque>
#include <functional>
#include <map>
#include <memory>
#include <string>
#include <utility>
#include <vector>

#ifndef _WIN32
#  include <arpa/inet.h>
#  include <errno.h>
#  include <fcntl.h>
#  include <netinet/in.h>
#  include <netinet/tcp.h>
#  include <poll.h>
#  include <sys/socket.h>
#  include <unistd.h>
#endif

#ifdef KNC_CLASSIC
#  include "../app/router.cppm"
#  include "../core/types.cppm"
#  include "../proto/session.cppm"
#else
export module knc.net.server;
export import knc.app.router;
export import knc.core.types;
export import knc.proto.session;
#endif

KNC_EXPORT namespace knc::net {

using app::Router;
using app::Routed;
using proto::Event;
using proto::Ingress;
using proto::Role;
using proto::Session;

/// Why a connection was closed.
enum class Closed : std::uint8_t {
    kPeerClosed,
    kFramingError,   ///< the stream desynchronised; there is no resync marker
    kTrafficAttack,  ///< too many authentication failures
    kZombie,         ///< silent past the heartbeat timeout
    kSocketError,
    kShutdown,
};

[[nodiscard]] inline const char* to_string(Closed c) noexcept {
    switch (c) {
        case Closed::kPeerClosed: return "peer closed";
        case Closed::kFramingError: return "framing error";
        case Closed::kTrafficAttack: return "traffic attack";
        case Closed::kZombie: return "heartbeat timeout";
        case Closed::kSocketError: return "socket error";
        case Closed::kShutdown: return "server shutting down";
    }
    return "unknown";
}

/// Per-connection counters, so a running server can be asked what it is
/// doing without a debugger attached.
struct ConnectionStats {
    std::uint64_t frames_in = 0;
    std::uint64_t events_routed = 0;
    std::uint64_t events_unhandled = 0;
    std::uint64_t events_refused = 0;
    std::uint64_t auth_failures = 0;
    std::uint64_t bytes_in = 0;
    std::uint64_t bytes_out = 0;
};

/// A Session plus an outbound queue. Knows nothing about sockets.
class Connection {
public:
    explicit Connection(std::uint64_t id, const Router& router)
        : id_(id), router_(&router), session_(Role::kServer) {}

    [[nodiscard]] std::uint64_t id() const noexcept { return id_; }
    [[nodiscard]] Session& session() noexcept { return session_; }
    [[nodiscard]] const Session& session() const noexcept { return session_; }
    [[nodiscard]] const ConnectionStats& stats() const noexcept { return stats_; }
    [[nodiscard]] const std::string& peer() const noexcept { return peer_; }
    void set_peer(std::string p) { peer_ = std::move(p); }

    [[nodiscard]] bool wants_close() const noexcept { return close_.has_value(); }
    [[nodiscard]] Closed close_reason() const noexcept {
        return close_.value_or(Closed::kShutdown);
    }
    void request_close(Closed why) noexcept {
        if (!close_) {
            close_ = why;
        }
    }

    /// KSession::OnAcceptConnection. Call once, immediately after accept.
    void start(std::uint64_t now_ms) {
        session_.begin(now_ms);
        session_.post_bytes(session_.accept());
    }

    /// Feed bytes that arrived, and route everything they complete.
    void on_bytes(ByteView data, std::uint64_t now_ms) {
        stats_.bytes_in += data.size();
        session_.push(data);
        drain(now_ms);
    }

    /// Periodic work: liveness only. Returns false if the connection is done.
    [[nodiscard]] bool tick(std::uint64_t now_ms) {
        if (session_.peer_is_zombie(now_ms)) {
            request_close(Closed::kZombie);
        }
        return !wants_close();
    }

    /// Output lives in the Session, so a handler that only has the session
    /// can still reply. These just forward.
    [[nodiscard]] bool has_pending_output() const noexcept {
        return session_.has_pending_output();
    }
    [[nodiscard]] ByteView pending_output() const noexcept {
        return session_.pending_output();
    }
    void consume_output(std::size_t n) {
        session_.consume_output(n);
        stats_.bytes_out += n;
    }

    /// Reported for every event that arrives. Default does nothing.
    std::function<void(const Connection&, const Event&, Routed, const std::string&)>
        on_event;

private:
    void drain(std::uint64_t now_ms) {
        for (;;) {
            const proto::Inbound in = session_.next(now_ms);
            if (in.status == Ingress::kNeedMoreData) {
                return;
            }
            ++stats_.frames_in;

            switch (in.status) {
                case Ingress::kFatal:
                    request_close(Closed::kFramingError);
                    return;

                case Ingress::kAuthFailed:
                    stats_.auth_failures = session_.auth_failures();
                    if (session_.traffic_attack()) {
                        request_close(Closed::kTrafficAttack);
                        return;
                    }
                    break;

                case Ingress::kRefused:
                case Ingress::kMalformed:
                    // Both mean the peer sent something this build will not
                    // process. Neither closes the connection: a real client
                    // that is one patch behind does this, and so does a
                    // client talking to a region it was not built for.
                    ++stats_.events_refused;
                    break;

                case Ingress::kHandled:
                    break;  // heartbeat or key exchange, already dealt with

                case Ingress::kEvent: {
                    const app::RouteResult r = router_->route(session_, in.event);
                    if (r.status == Routed::kHandled) {
                        ++stats_.events_routed;
                    } else {
                        ++stats_.events_unhandled;
                    }
                    if (on_event) {
                        on_event(*this, in.event, r.status, r.detail);
                    }
                    break;
                }

                case Ingress::kNeedMoreData:
                    return;
            }
        }
    }

    std::uint64_t id_;
    const Router* router_;
    Session session_;
    std::string peer_;
    std::optional<Closed> close_;
    ConnectionStats stats_;
};

#ifndef _WIN32

/// Everything the owner might want to watch. All optional.
struct ServerCallbacks {
    std::function<void(const Connection&)> on_open;
    std::function<void(const Connection&, Closed)> on_close;
    std::function<void(const Connection&, const Event&, Routed, const std::string&)>
        on_event;
    std::function<void(const std::string&)> on_log;
};

/// A poll()-driven TCP listener. Single threaded on purpose.
///
/// The original ran an IOCP worker pool plus nine database threads and
/// guarded the shared SA database with a process-wide critical section that
/// every packet had to take. Since a session's state is private to that
/// session, the concurrency bought contention rather than throughput. One
/// thread first; measure before adding more.
class TcpServer {
public:
    explicit TcpServer(Router& router, ServerCallbacks cb = {})
        : router_(&router), cb_(std::move(cb)) {}

    ~TcpServer() { stop(); }

    TcpServer(const TcpServer&) = delete;
    TcpServer& operator=(const TcpServer&) = delete;

    /// Bind and listen. Port 0 asks the kernel to choose; read it back with
    /// port(), which is what the tests do rather than guessing a free one.
    void listen(std::uint16_t port, const char* address = "0.0.0.0",
                int backlog = 128) {
        listen_fd_ = ::socket(AF_INET, SOCK_STREAM, 0);
        if (listen_fd_ < 0) {
            throw ProtocolError{std::string{"socket(): "} + std::strerror(errno)};
        }
        int one = 1;
        ::setsockopt(listen_fd_, SOL_SOCKET, SO_REUSEADDR, &one, sizeof one);
        set_nonblocking(listen_fd_);

        sockaddr_in addr{};
        addr.sin_family = AF_INET;
        addr.sin_port = htons(port);
        if (::inet_pton(AF_INET, address, &addr.sin_addr) != 1) {
            close_fd(listen_fd_);
            throw ProtocolError{std::string{"bad listen address: "} + address};
        }
        if (::bind(listen_fd_, reinterpret_cast<sockaddr*>(&addr), sizeof addr) < 0) {
            const std::string why = std::strerror(errno);
            close_fd(listen_fd_);
            throw ProtocolError{"bind(): " + why};
        }
        if (::listen(listen_fd_, backlog) < 0) {
            const std::string why = std::strerror(errno);
            close_fd(listen_fd_);
            throw ProtocolError{"listen(): " + why};
        }

        sockaddr_in bound{};
        socklen_t len = sizeof bound;
        if (::getsockname(listen_fd_, reinterpret_cast<sockaddr*>(&bound), &len) == 0) {
            port_ = ntohs(bound.sin_port);
        }
        log("listening on " + std::string{address} + ":" + std::to_string(port_));
    }

    [[nodiscard]] std::uint16_t port() const noexcept { return port_; }
    [[nodiscard]] std::size_t connection_count() const noexcept {
        return conns_.size();
    }
    [[nodiscard]] std::uint64_t total_accepted() const noexcept { return accepted_; }

    /// One pass: wait up to `timeout_ms` for activity, then service it.
    /// `now_ms` is passed in so the caller owns the clock.
    void poll_once(int timeout_ms, std::uint64_t now_ms) {
        std::vector<pollfd> fds;
        fds.reserve(conns_.size() + 1);
        fds.push_back(pollfd{listen_fd_, POLLIN, 0});
        for (const auto& [fd, conn] : conns_) {
            short events = POLLIN;
            if (conn->has_pending_output()) {
                events |= POLLOUT;
            }
            fds.push_back(pollfd{fd, events, 0});
        }

        const int n = ::poll(fds.data(), static_cast<nfds_t>(fds.size()), timeout_ms);
        if (n < 0) {
            if (errno == EINTR) {
                return;
            }
            throw ProtocolError{std::string{"poll(): "} + std::strerror(errno)};
        }

        if (fds[0].revents & POLLIN) {
            accept_all(now_ms);
        }
        for (std::size_t i = 1; i < fds.size(); ++i) {
            service(fds[i].fd, fds[i].revents, now_ms);
        }

        // Liveness, then reap. Done after servicing so that a connection
        // which spoke this pass is never declared dead in the same pass.
        for (auto& [fd, conn] : conns_) {
            (void)conn->tick(now_ms);
        }
        reap();
    }

    /// Run until stop() is called from a handler or another thread.
    void run(std::uint64_t (*clock_ms)(), int timeout_ms = 50) {
        running_ = true;
        while (running_) {
            poll_once(timeout_ms, clock_ms());
        }
    }

    void stop() {
        running_ = false;
        for (auto& [fd, conn] : conns_) {
            conn->request_close(Closed::kShutdown);
            if (cb_.on_close) {
                cb_.on_close(*conn, Closed::kShutdown);
            }
            close_fd(fd);
        }
        conns_.clear();
        if (listen_fd_ >= 0) {
            close_fd(listen_fd_);
            listen_fd_ = -1;
        }
    }

    /// Visit every live connection - for a broadcast, or a status dump.
    template <typename Fn>
    void for_each(Fn&& fn) {
        for (auto& [fd, conn] : conns_) {
            fn(*conn);
        }
    }

private:
    static void set_nonblocking(int fd) {
        const int flags = ::fcntl(fd, F_GETFL, 0);
        ::fcntl(fd, F_SETFL, (flags < 0 ? 0 : flags) | O_NONBLOCK);
    }
    static void close_fd(int fd) {
        if (fd >= 0) {
            ::close(fd);
        }
    }
    void log(const std::string& line) const {
        if (cb_.on_log) {
            cb_.on_log(line);
        }
    }

    void accept_all(std::uint64_t now_ms) {
        for (;;) {
            sockaddr_in peer{};
            socklen_t len = sizeof peer;
            const int fd =
                ::accept(listen_fd_, reinterpret_cast<sockaddr*>(&peer), &len);
            if (fd < 0) {
                if (errno == EAGAIN || errno == EWOULDBLOCK) {
                    return;
                }
                if (errno == EINTR || errno == ECONNABORTED) {
                    continue;
                }
                log(std::string{"accept(): "} + std::strerror(errno));
                return;
            }
            set_nonblocking(fd);
            int one = 1;
            // The handshake is a single small frame and the client blocks
            // waiting for it; Nagle would add 40ms to every login.
            ::setsockopt(fd, IPPROTO_TCP, TCP_NODELAY, &one, sizeof one);

            auto conn = std::make_unique<Connection>(++accepted_, *router_);
            char ip[INET_ADDRSTRLEN] = {};
            ::inet_ntop(AF_INET, &peer.sin_addr, ip, sizeof ip);
            conn->set_peer(std::string{ip} + ":" + std::to_string(ntohs(peer.sin_port)));
            conn->on_event = cb_.on_event;

            try {
                conn->start(now_ms);  // mint the SA and queue the key exchange
            } catch (const std::exception& e) {
                log(std::string{"handshake failed: "} + e.what());
                close_fd(fd);
                continue;
            }

            if (cb_.on_open) {
                cb_.on_open(*conn);
            }
            conns_.emplace(fd, std::move(conn));
            flush(fd);  // send the key exchange now, do not wait for a poll
        }
    }

    void service(int fd, short revents, std::uint64_t now_ms) {
        const auto it = conns_.find(fd);
        if (it == conns_.end() || revents == 0) {
            return;
        }
        Connection& conn = *it->second;

        if (revents & (POLLERR | POLLNVAL)) {
            conn.request_close(Closed::kSocketError);
            return;
        }
        if (revents & POLLIN) {
            std::uint8_t buf[16384];
            for (;;) {
                const ssize_t n = ::recv(fd, buf, sizeof buf, 0);
                if (n > 0) {
                    try {
                        conn.on_bytes(ByteView{buf, static_cast<std::size_t>(n)}, now_ms);
                    } catch (const std::exception& e) {
                        // Connection::drain already swallows per-event
                        // failures; reaching here means the session itself
                        // is unusable.
                        log("connection " + std::to_string(conn.id()) + ": " + e.what());
                        conn.request_close(Closed::kFramingError);
                        return;
                    }
                    if (conn.wants_close()) {
                        return;
                    }
                    continue;
                }
                if (n == 0) {
                    conn.request_close(Closed::kPeerClosed);
                    return;
                }
                if (errno == EAGAIN || errno == EWOULDBLOCK) {
                    break;
                }
                if (errno == EINTR) {
                    continue;
                }
                conn.request_close(Closed::kSocketError);
                return;
            }
        }
        if (revents & POLLHUP) {
            // Drain first: a half-closed peer may still have sent a complete
            // final frame, and POLLIN above has already taken it.
            conn.request_close(Closed::kPeerClosed);
            return;
        }
        flush(fd);
    }

    void flush(int fd) {
        const auto it = conns_.find(fd);
        if (it == conns_.end()) {
            return;
        }
        Connection& conn = *it->second;
        while (conn.has_pending_output()) {
            const ByteView out = conn.pending_output();
            const ssize_t n = ::send(fd, out.data(), out.size(), MSG_NOSIGNAL);
            if (n > 0) {
                conn.consume_output(static_cast<std::size_t>(n));
                continue;
            }
            if (n < 0 && (errno == EAGAIN || errno == EWOULDBLOCK)) {
                return;  // POLLOUT will bring us back
            }
            if (n < 0 && errno == EINTR) {
                continue;
            }
            conn.request_close(Closed::kSocketError);
            return;
        }
    }

    void reap() {
        for (auto it = conns_.begin(); it != conns_.end();) {
            if (!it->second->wants_close()) {
                ++it;
                continue;
            }
            // One last push: a refusal or an error reply is worth delivering
            // before the socket goes away.
            flush(it->first);
            if (cb_.on_close) {
                cb_.on_close(*it->second, it->second->close_reason());
            }
            close_fd(it->first);
            it = conns_.erase(it);
        }
    }

    Router* router_;
    ServerCallbacks cb_;
    int listen_fd_ = -1;
    std::uint16_t port_ = 0;
    bool running_ = false;
    std::uint64_t accepted_ = 0;
    std::map<int, std::unique_ptr<Connection>> conns_;
};

#endif  // !_WIN32

}  // namespace knc::net
