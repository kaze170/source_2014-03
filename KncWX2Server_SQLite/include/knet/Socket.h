// knet/Socket.h - thin portable socket layer (Winsock2 on Windows, BSD sockets elsewhere).
#pragma once

#include <cstdint>
#include <string>

#include "kcore/Types.h"

namespace knc::net {

#if defined(_WIN32)
using SocketHandle = std::uintptr_t;  // SOCKET
inline constexpr SocketHandle kInvalidSocket = static_cast<SocketHandle>(~0ull);
#else
using SocketHandle = int;
inline constexpr SocketHandle kInvalidSocket = -1;
#endif

// Must be called once per process (WSAStartup on Windows, no-op elsewhere).
bool startupSockets();
void cleanupSockets();
std::string lastSocketError();

// Sets a socket non blocking, disables Nagle and enables SO_REUSEADDR.
bool configureSocket(SocketHandle sock, bool reuseAddr);

// Creates a listening TCP socket bound to 0.0.0.0:port. Returns kInvalidSocket on failure.
SocketHandle listenTcp(unsigned short port, int backlog = 128);

// Creates a UDP socket bound to 0.0.0.0:port.
SocketHandle bindUdp(unsigned short port);

void closeSocket(SocketHandle sock);

// SO_RCVTIMEO is a timeval on POSIX and a millisecond DWORD on Winsock, so deadlines on a blocking
// socket go through this helper instead of a raw setsockopt.
bool setRecvTimeout(SocketHandle sock, int milliseconds);
bool setSendTimeout(SocketHandle sock, int milliseconds);

// Accepts one pending connection. Returns kInvalidSocket when nothing is pending.
SocketHandle acceptSocket(SocketHandle listener, std::string* peerIp, unsigned short* peerPort);

// Reads up to len bytes. Returns >0 bytes read, 0 when the peer closed, -1 when no data is pending
// and -2 on a hard error.
int recvSome(SocketHandle sock, void* buffer, std::size_t len);
// Sends len bytes, returns the number of bytes accepted (may be short) or -1.
int sendSome(SocketHandle sock, const void* buffer, std::size_t len);

// Returns 1 when the socket has an error pending, 0 otherwise.
int socketError(SocketHandle sock);

int recvFrom(SocketHandle sock, void* buffer, std::size_t len, std::string* fromIp, unsigned short* fromPort);
int sendTo(SocketHandle sock, const void* buffer, std::size_t len, const std::string& ip, unsigned short port);

// "ip:port" formatting helper.
std::string endpointString(const std::string& ip, unsigned short port);

// Local addresses of the host (used to fill the channel/game server list, the original code used
// NetCommon::GetLocalIPW()).
std::string localIpv4Address();

}  // namespace knc::net
