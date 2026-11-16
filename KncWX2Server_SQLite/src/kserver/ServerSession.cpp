#include "kserver/ServerSession.h"

#include "kcore/Log.h"
#include "kcore/Serialize.h"
#include "kproto/Packet.h"
#include "kserver/Server.h"

namespace knc::server {

ServerSession::ServerSession(std::shared_ptr<net::Session> session, PortRole role, Server& server)
    : session_(std::move(session)), role_(role), server_(server) {
    state_.clientIp = session_->peerIp();
}

bool ServerSession::send(EventId id, const SerBuffer& payload) {
    Event event;
    // Login/channel packets travel under the client proxy performer with no target uid, exactly
    // like KSession::SendPacket in the 2014 code.
    event.setData(kPiGsProxy, nullptr, id);
    event.payload = payload;
    if (!session_->send(event)) {
        KLOG(LogLevel::kWarn, "session") << session_->endpoint() << " send of event " << id << " failed";
        return false;
    }
    return true;
}

void ServerSession::disconnect(const std::string& reason) { session_->close(reason); }

int ServerSession::connectedUsers() const { return server_.connectedUsers(); }

void ServerSession::onEvent(Event& event) {
    bool keep = true;
    // The channel port repeats the login handshake: the 2014 client sent EGS_CONNECT_REQ and
    // EGS_VERIFY_ACCOUNT_REQ again after being redirected (one connection per process, so both
    // services have to answer those two ids).
    const bool isLoginPacket = event.eventId == EGS_CONNECT_REQ ||
                               event.eventId == EGS_VERIFY_ACCOUNT_REQ ||
                               event.eventId == EGS_NEW_USER_JOIN_REQ;
    if (role_ == PortRole::kLogin || isLoginPacket) {
        keep = server_.login().handle(state_, *this, event);
    } else {
        keep = server_.channel().handle(state_, *this, event);
    }
    if (!keep) {
        session_->close("protocol ended the session");
    }
    // Let the account uid travel back with the event trace so later room code can find the actor.
    if (state_.account.uid != 0) session_->setUid(state_.account.uid);
}

void ServerSession::onClosed(const char* reason) {
    KLOG(LogLevel::kDebug, "session") << session_->endpoint() << " closed: " << (reason ? reason : "");
}

}  // namespace knc::server
