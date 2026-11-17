// ---------------------------------------------------------------------------
// KncWX2ServerNG - the server process.
//
// One process where there were five (Center, Game, Login, Global, Channel).
// At this point it speaks the protocol and answers the connect handshake;
// the game logic, the actors and the database are not here yet.
//
//   kncng_server --port 9800 --version 1.2.3
//
// Then connect with tools/crosscheck/knc_ref.py, which is an independent
// implementation of the client side of the wire protocol.
// ---------------------------------------------------------------------------
#define KNC_CLASSIC 1

#include <chrono>
#include <csignal>
#include <cstdio>
#include <cstdlib>
#include <cstring>
#include <iostream>
#include <string>

#include "net/server.cppm"

using namespace knc;
using namespace knc::proto;
using namespace knc::net;

namespace {

// NetError_def.h. Only the handful the connect path needs; the full 4060-row
// table (and its ten regional variants) has not been generated yet.
constexpr std::int32_t kNetOk = 0;           // NET_OK
constexpr std::int32_t kErrConnectVersion = 3;  // ERR_CONNECT_00, bad version
constexpr std::int32_t kErrConnectFull = 4;     // ERR_CONNECT_04, server full

struct Options {
    std::uint16_t port = 9800;
    std::string address = "0.0.0.0";
    std::u16string version = u"1.2.3";
    int max_users = 1000;
    int channel_id = 1;
    int udp_port = 0x3A98;
    bool quiet = false;
};

std::u16string to_u16(const char* s) {
    std::u16string out;
    for (const char* p = s; *p; ++p) {
        out.push_back(static_cast<char16_t>(static_cast<unsigned char>(*p)));
    }
    return out;
}

std::string to_utf8ish(const std::u16string& s) {
    std::string out;
    for (const char16_t c : s) {
        out.push_back(c < 0x80 ? static_cast<char>(c) : '?');
    }
    return out;
}

std::uint64_t now_ms() {
    using namespace std::chrono;
    return static_cast<std::uint64_t>(
        duration_cast<milliseconds>(steady_clock::now().time_since_epoch()).count());
}

volatile std::sig_atomic_t g_stop = 0;
void on_signal(int) { g_stop = 1; }

void usage() {
    std::puts(
        "kncng_server [options]\n"
        "  --port N          listen port (default 9800, 0 = pick one)\n"
        "  --address A       bind address (default 0.0.0.0)\n"
        "  --version V       accepted client version (default 1.2.3)\n"
        "  --max-users N     connection limit (default 1000)\n"
        "  --channel-id N    reported in EGS_CONNECT_ACK (default 1)\n"
        "  --quiet           only log listen/open/close\n");
}

}  // namespace

int main(int argc, char** argv) {
    Options opt;
    for (int i = 1; i < argc; ++i) {
        const std::string a = argv[i];
        const char* next = (i + 1 < argc) ? argv[i + 1] : nullptr;
        if (a == "--help" || a == "-h") {
            usage();
            return 0;
        } else if (a == "--port" && next) {
            opt.port = static_cast<std::uint16_t>(std::atoi(argv[++i]));
        } else if (a == "--address" && next) {
            opt.address = argv[++i];
        } else if (a == "--version" && next) {
            opt.version = to_u16(argv[++i]);
        } else if (a == "--max-users" && next) {
            opt.max_users = std::atoi(argv[++i]);
        } else if (a == "--channel-id" && next) {
            opt.channel_id = std::atoi(argv[++i]);
        } else if (a == "--quiet") {
            opt.quiet = true;
        } else {
            std::fprintf(stderr, "unknown option: %s\n", a.c_str());
            usage();
            return 2;
        }
    }

    app::Router router;
    int live_connections = 0;

    // KGSUser::OnConnectReq (GameServer/GSUserFunction.cpp:4303).
    //
    // The packet type is not written here: it comes from the generated
    // binding for event 26, so this handler cannot read the payload as
    // something the wire does not carry.
    router.on<Ev::EGS_CONNECT_REQ>(
        [&](Session& session, const packets::KEGS_CONNECT_REQ& req) {
            packets::KEGS_CONNECT_ACK ack{};
            if (req.m_wstrVersion != opt.version) {
                ack.m_iOK = kErrConnectVersion;
            } else if (live_connections > opt.max_users) {
                ack.m_iOK = kErrConnectFull;
            } else {
                ack.m_iOK = kNetOk;
                ack.m_UDPPort = opt.udp_port;
                ack.m_iChannelID = opt.channel_id;
            }
            // The legacy code disconnected on a version mismatch. Here the
            // reply goes out and the peer is left to close: the client shows
            // the user "patch and reconnect", and it cannot do that if the
            // socket dies first.
            session.post_packet<Ev::EGS_CONNECT_ACK>(ack);
        });

    ServerCallbacks cb;
    cb.on_log = [](const std::string& line) {
        std::cout << "[server] " << line << std::endl;
    };
    cb.on_open = [&](const Connection& c) {
        ++live_connections;
        std::cout << "[open ] #" << c.id() << " " << c.peer() << " (spi "
                  << c.session().spi() << ")" << std::endl;
    };
    cb.on_close = [&](const Connection& c, Closed why) {
        --live_connections;
        const auto& s = c.stats();
        std::cout << "[close] #" << c.id() << " " << to_string(why) << " - frames "
                  << s.frames_in << ", routed " << s.events_routed << ", unhandled "
                  << s.events_unhandled << ", refused " << s.events_refused
                  << ", auth failures " << s.auth_failures << std::endl;
    };
    if (!opt.quiet) {
        cb.on_event = [](const Connection& c, const Event& ev, Routed r,
                         const std::string& detail) {
            std::cout << "[event] #" << c.id() << " " << ev.id() << " " << ev.name()
                      << " -> " << app::to_string(r)
                      << (detail.empty() ? "" : (": " + detail)) << std::endl;
        };
    }

    std::signal(SIGINT, on_signal);
    std::signal(SIGTERM, on_signal);

    try {
        TcpServer server{router, std::move(cb)};
        server.listen(opt.port, opt.address.c_str());
        std::cout << "[server] accepting version " << to_utf8ish(opt.version)
                  << ", channel " << opt.channel_id << ", " << router.size()
                  << " handler(s) registered" << std::endl;
        std::cout.flush();

        while (!g_stop) {
            server.poll_once(50, now_ms());
        }
        std::cout << "[server] shutting down" << std::endl;
        server.stop();
    } catch (const std::exception& e) {
        std::fprintf(stderr, "fatal: %s\n", e.what());
        return 1;
    }
    return 0;
}
