// kserver/ServerSession.h - binds one knc::net::Session to the login or channel service.
//
// knc::net::Session owns framing and the KSecureBuffer; this adapter owns the protocol state
// (AccountSession), routes events to LoginService or ChannelService depending on which listener
// accepted the connection, and implements LoginResponder on top of Session::sendEvent.
#pragma once

#include <memory>
#include <string>

#include "kcore/Types.h"
#include "knet/Session.h"
#include "kproto/Event.h"
#include "kserver/AccountSession.h"
#include "kserver/LoginService.h"

namespace knc::server {

class Server;

// Which service a port belongs to. The single process build keeps the 2014 port split so a client
// still connects to the login port first and is then redirected to the channel port.
enum class PortRole { kLogin, kChannel, kGame };

class ServerSession : public LoginResponder {
public:
    ServerSession(std::shared_ptr<net::Session> session, PortRole role, Server& server);

    // net::EventSink entry points, forwarded by Server.
    void onEvent(Event& event);
    void onClosed(const char* reason);

    // LoginResponder
    bool send(EventId id, const SerBuffer& payload) override;
    void disconnect(const std::string& reason) override;
    const std::string& peerIp() const override { return session_->peerIp(); }
    int connectedUsers() const override;

    AccountSession& state() { return state_; }
    PortRole role() const { return role_; }
    const std::shared_ptr<net::Session>& session() const { return session_; }
    void setUid(UidType uid) { session_->setUid(uid); }

private:
    std::shared_ptr<net::Session> session_;
    PortRole role_;
    Server& server_;
    AccountSession state_;
};

}  // namespace knc::server
