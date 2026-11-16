#include "kserver/Server.h"

#include <algorithm>

#include "kcore/IniFile.h"
#include "kcore/Log.h"
#include "kdb/Schema.h"

namespace knc::server {

Server::Server(Config config)
    : config_(std::move(config)),
      db_(),
      reposOwner_(db::makeRepositories(db_)),
      repos_(*reposOwner_),
      login_(repos_, config_),
      channel_(repos_, config_) {}

Server::~Server() { stop(); }

void Server::installListener(const ListenerConfig& cfg, PortRole role, const char* name) {
    if (!cfg.enabled || cfg.port == 0) return;

    net::ListenerConfig listenerConfig;
    listenerConfig.name = name;
    listenerConfig.port = cfg.port;
    listenerConfig.clientTier = true;  // real X2 clients: send E_ACCEPT_CONNECTION_NOT
    listenerConfig.checkSequenceNum = cfg.checkSequenceNum;
    listenerConfig.authFailLimit = cfg.authFailLimit;

    auto listener = std::make_shared<net::Listener>(listenerConfig, reactor_, *this);
    listener->setSessionFactory(
        [this](net::SocketHandle sock, std::string ip, unsigned short port,
               const net::ListenerConfig& accepted) { return makeSession(sock, ip, port, accepted); });
    listenerRoles_.emplace_back(listener.get(), role);
    listeners_.push_back(listener);
}

std::shared_ptr<net::Session> Server::makeSession(net::SocketHandle sock, std::string ip,
                                                  unsigned short port,
                                                  const net::ListenerConfig& cfg) {
    auto session = std::make_shared<net::Session>(sock, std::move(ip), port, *this,
                                                  net::Session::Tier::kClient,
                                                  cfg.name + "/" + net::endpointString(ip, port));
    // The listener the session came from decides which service handles its events; the name is
    // unique per listener so it doubles as the lookup key.
    PortRole role = PortRole::kChannel;
    for (const auto& [listener, listenerRole] : listenerRoles_) {
        if (listener->name() == cfg.name) {
            role = listenerRole;
            break;
        }
    }
    auto bound = std::make_shared<ServerSession>(session, role, *this);
    sessions_[session.get()] = bound;
    return session;
}

bool Server::start(std::string* error) {
    if (started_) return true;

    // Winsock needs this before the first socket call; on POSIX it is a no-op.
    if (!net::startupSockets()) {
        if (error) *error = "socket subsystem failed to start";
        return false;
    }

    // ---- logger -----------------------------------------------------------
    Logger::instance().setLevel(config_.logLevel);
    if (!config_.logFile.empty()) {
        if (!Logger::instance().openFile(config_.logFile)) {
            KLOG(LogLevel::kWarn, "server") << "cannot open log file " << config_.logFile;
        }
    }

    if (config_.shutdownAfterSeconds > 0) {
        reactor_.scheduleOnce(config_.shutdownAfterSeconds * 1000, [this] {
            KLOG(LogLevel::kInfo, "server") << "shutdown timer expired, stopping";
            reactor_.stop();
        });
    }

    // ---- database ---------------------------------------------------------
    db::Database::Config dbConfig;
    dbConfig.path = config_.database.path;
    dbConfig.busyTimeoutMs = config_.database.busyTimeoutMs;
    dbConfig.cacheSizeKb = config_.database.cacheSizeKb;
    dbConfig.synchronousNormal = true;
    if (!db_.open(dbConfig, error)) return false;

    if (config_.migrateOnStart) {
        const db::MigrationResult migration = db::applySchema(db_);
        if (!migration.ok) {
            if (error) *error = "schema migration failed: " + migration.error;
            return false;
        }
        if (!migration.appliedScripts.empty()) {
            KLOG(LogLevel::kInfo, "server") << "schema at version " << migration.toVersion << " ("
                                            << migration.appliedScripts.size() << " scripts)";
        }
    }
    if (config_.seedTestAccounts) {
        std::string seedError;
        if (!db::seedDefaultAccounts(db_, &seedError)) {
            KLOG(LogLevel::kWarn, "server") << "seeding demo accounts failed: " << seedError;
        }
    }

    std::string topologyError;
    if (!db::seedDefaultTopology(db_, config_.name, config_.channels.empty() ? "127.0.0.1"
                                                                            : config_.channels.front().ip,
                                 config_.channel.port, config_.game.port,
                                 config_.channels.empty() ? 0 : config_.channels.front().ncUdpPort,
                                 config_.channel.maxUser, &topologyError)) {
        KLOG(LogLevel::kWarn, "server") << "seeding topology failed: " << topologyError;
    }

    // ---- services ---------------------------------------------------------
    login_.setOptions([this] {
        LoginService::Options options;
        options.expectedVersion = config_.version();
        options.maxUser = config_.login.maxUser;
        options.authFailLimit = config_.login.authFailLimit;
        return options;
    }());
    channel_.setDeleteReuseDelayMs(login_.options().unitDeleteReuseDelayMs);

    // ---- listeners --------------------------------------------------------
    installListener(config_.login, PortRole::kLogin, "login");
    installListener(config_.channel, PortRole::kChannel, "channel");
    installListener(config_.game, PortRole::kGame, "game");

    if (listeners_.empty()) {
        if (error) *error = "no listener enabled";
        return false;
    }
    for (const auto& listener : listeners_) {
        if (!listener->start()) {
            if (error) *error = "cannot bind " + listener->name() + " port " +
                                std::to_string(listener->port());
            return false;
        }
    }

    started_ = true;
    KLOG(LogLevel::kInfo, "server") << config_.name << " started, version "
                                    << toUtf8(config_.version()) << ", db " << config_.database.path;
    return true;
}

void Server::run(int maxIterations) {
    if (maxIterations > 0) {
        for (int i = 0; i < maxIterations && reactor_.running(); ++i) reactor_.runOnce(20);
        return;
    }
    reactor_.run();
}

void Server::stop() {
    for (auto& listener : listeners_) listener->stop();
    listeners_.clear();
    sessions_.clear();
    listenerRoles_.clear();
    if (db_.isOpen()) db_.close();
    Logger::instance().closeFile();
    started_ = false;
}

ServerSession* Server::findSession(net::Session& session) {
    const auto it = sessions_.find(&session);
    return it == sessions_.end() ? nullptr : it->second.get();
}

void Server::onEvent(net::Session& session, Event& event) {
    if (ServerSession* bound = findSession(session)) {
        bound->onEvent(event);
        return;
    }
    KLOG(LogLevel::kWarn, "server") << "event from an unbound session " << session.endpoint() << ": "
                                    << event.describe();
}

void Server::onSessionClosed(net::Session& session, const char* reason) {
    const auto it = sessions_.find(&session);
    if (it == sessions_.end()) return;
    it->second->onClosed(reason);
    sessions_.erase(it);
    for (auto& listener : listeners_) listener->pruneSessions();
}

std::size_t Server::acceptedOn(PortRole role) const {
    std::size_t total = 0;
    for (const auto& [listener, listenerRole] : listenerRoles_) {
        if (listenerRole == role) total += listener->accepted();
    }
    return total;
}

std::size_t Server::activeOn(PortRole role) const {
    std::size_t total = 0;
    for (const auto& [session, bound] : sessions_) {
        (void)session;
        if (bound->role() == role) ++total;
    }
    return total;
}

}  // namespace knc::server
