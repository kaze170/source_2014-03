#include "knet/Listener.h"

#include <algorithm>

#include "kcore/Log.h"

namespace knc::net {

Listener::Listener(const ListenerConfig& config, Reactor& reactor, EventSink& sink)
    : config_(config), reactor_(reactor), sink_(sink) {}

Listener::~Listener() { stop(); }

bool Listener::start() {
    listenSock_ = listenTcp(config_.port, 512);
    if (listenSock_ == kInvalidSocket) {
        KLOG(LogLevel::kError, "listen") << "cannot bind tcp port " << config_.port << ": " << lastSocketError();
        return false;
    }
    reactor_.add(shared_from_this());
    KLOG(LogLevel::kInfo, "listen") << config_.name << " listening on tcp/" << config_.port;
    return true;
}

void Listener::stop() {
    if (listenSock_ != kInvalidSocket) {
        closeSocket(listenSock_);
        listenSock_ = kInvalidSocket;
    }
}

void Listener::onReadable(Reactor& reactor) {
    for (;;) {
        std::string ip;
        unsigned short port = 0;
        const SocketHandle sock = acceptSocket(listenSock_, &ip, &port);
        if (sock == kInvalidSocket) {
            if (socketError(listenSock_) != 0) {
                KLOG(LogLevel::kError, "listen") << config_.name << " accept failed";
            }
            return;
        }
        ++accepted_;

        std::shared_ptr<Session> session;
        if (factory_) {
            session = factory_(sock, ip, port, config_);
        } else {
            session = std::make_shared<Session>(sock, ip, port, sink_,
                                                config_.clientTier ? Session::Tier::kClient : Session::Tier::kInternal,
                                                config_.name + "/" + endpointString(ip, port));
        }
        if (!session) {
            closeSocket(sock);
            continue;
        }
        session->setCheckSequenceNum(config_.checkSequenceNum);
        session->setAuthFailLimit(config_.authFailLimit);
        sessions_.push_back(session);
        reactor.add(session);
        if (config_.clientTier) {
            // E_ACCEPT_CONNECTION_NOT is sent immediately, exactly like KSession::OnAcceptConnection.
            session->onAcceptConnection();
        }
        KLOG(LogLevel::kDebug, "listen") << config_.name << " accepted " << endpointString(ip, port);
    }
}

void Listener::onClose(Reactor& reactor) {
    (void)reactor;
    stop();
}

void Listener::pruneSessions() {
    sessions_.erase(std::remove_if(sessions_.begin(), sessions_.end(),
                                   [](const std::shared_ptr<Session>& s) { return !s || s->closed(); }),
                    sessions_.end());
}

}  // namespace knc::net
