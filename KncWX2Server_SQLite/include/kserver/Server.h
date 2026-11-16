// kserver/Server.h - the single process that replaces the five 2014 executables.
//
// 2014 deployment                          this build
//   LoginServer.exe    (login port)   ->   one Listener on [Login] Port
//   ChannelServer.exe  (channel port) ->   one Listener on [Channel] Port
//   GameServer.exe     (game port)    ->   one Listener on [Game] Port (stub until kgame/room lands)
//   CenterServer.exe / GlobalServer.exe -> in-process services (no listeners by default)
//
// All of them share one net::Reactor (one thread), one SQLite database and one logger, which is
// what let the five ODBC DSNs and the DBE_* cross-process event glue disappear.
#pragma once

#include <map>
#include <memory>
#include <string>
#include <vector>

#include "kcore/Types.h"
#include "kdb/Database.h"
#include "kdb/Repositories.h"
#include "knet/Listener.h"
#include "knet/Reactor.h"
#include "knet/Session.h"
#include "kserver/ChannelService.h"
#include "kserver/Config.h"
#include "kserver/LoginService.h"
#include "kserver/ServerSession.h"

namespace knc::server {

class Server : public net::EventSink {
public:
    explicit Server(Config config);
    ~Server() override;

    // Opens the database (running the schema migration and the demo seeds when configured) and
    // binds every enabled listener. Returns false on the first hard error.
    bool start(std::string* error = nullptr);
    void stop();

    // Runs the reactor until stop() (or the configured ShutdownAfterSeconds). `maxIterations` is
    // only used by the smoke tests.
    void run(int maxIterations = 0);
    net::Reactor& reactor() { return reactor_; }

    // net::EventSink
    void onEvent(net::Session& session, Event& event) override;
    void onSessionClosed(net::Session& session, const char* reason) override;

    // Session bookkeeping (called by ServerSession).
    int connectedUsers() const { return static_cast<int>(sessions_.size()); }
    ServerSession* findSession(net::Session& session);

    db::Database& database() { return db_; }
    db::Repositories& repositories() { return repos_; }
    LoginService& login() { return login_; }
    ChannelService& channel() { return channel_; }
    const Config& config() const { return config_; }

    // Per-port counters, for the status line and the tests.
    std::size_t acceptedOn(PortRole role) const;
    std::size_t activeOn(PortRole role) const;

private:
    void installListener(const ListenerConfig& cfg, PortRole role, const char* name);
    std::shared_ptr<net::Session> makeSession(net::SocketHandle sock, std::string ip, unsigned short port,
                                              const net::ListenerConfig& cfg);

    Config config_;
    net::Reactor reactor_{"knc"};
    db::Database db_;
    std::unique_ptr<db::Repositories> reposOwner_;
    db::Repositories& repos_;
    LoginService login_;
    ChannelService channel_;

    std::vector<std::shared_ptr<net::Listener>> listeners_;
    std::vector<std::pair<net::Listener*, PortRole>> listenerRoles_;
    std::map<net::Session*, std::shared_ptr<ServerSession>> sessions_;
    bool started_ = false;
};

}  // namespace knc::server
