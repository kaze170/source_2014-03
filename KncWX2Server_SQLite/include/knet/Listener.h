// knet/Listener.h - TCP acceptor that hands accepted sockets to a session factory.
#pragma once

#include <functional>
#include <memory>
#include <string>
#include <vector>

#include "knet/Reactor.h"
#include "knet/Session.h"
#include "knet/Socket.h"

namespace knc::net {

struct ListenerConfig {
    std::string name;           // "channel", "game", "fep"
    unsigned short port = 0;
    bool clientTier = true;      // true: a real client port (performs the accept handshake)
    bool checkSequenceNum = false;
    unsigned int authFailLimit = 100;
};

class Listener : public Handler, public std::enable_shared_from_this<Listener> {
public:
    using SessionFactory = std::function<std::shared_ptr<Session>(SocketHandle, std::string, unsigned short,
                                                                  const ListenerConfig&)>;

    Listener(const ListenerConfig& config, Reactor& reactor, EventSink& sink);
    ~Listener() override;

    bool start();
    void stop();

    // Handler
    SocketHandle handle() const override { return listenSock_; }
    void onReadable(Reactor& reactor) override;
    void onClose(Reactor& reactor) override;
    const char* tag() const override { return "listener"; }

    const ListenerConfig& config() const { return config_; }
    unsigned short port() const { return config_.port; }
    std::size_t accepted() const { return accepted_; }
    const std::string& name() const { return config_.name; }

    // Sessions created by this listener, pruned when they close.
    const std::vector<std::shared_ptr<Session>>& sessions() const { return sessions_; }
    void pruneSessions();

    void setSessionFactory(SessionFactory factory) { factory_ = std::move(factory); }

private:
    ListenerConfig config_;
    Reactor& reactor_;
    EventSink& sink_;
    SocketHandle listenSock_ = kInvalidSocket;
    std::size_t accepted_ = 0;
    std::vector<std::shared_ptr<Session>> sessions_;
    SessionFactory factory_;
};

}  // namespace knc::net
