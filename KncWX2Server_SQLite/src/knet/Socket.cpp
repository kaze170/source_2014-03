#include "knet/Socket.h"

#include <algorithm>
#include <csignal>
#include <cstring>
#include <mutex>
#include <vector>

#if defined(_WIN32)
#include <winsock2.h>
#include <ws2tcpip.h>
#pragma comment(lib, "ws2_32.lib")
#else
#include <arpa/inet.h>
#include <cerrno>
#include <fcntl.h>
#include <ifaddrs.h>
#include <netinet/in.h>
#include <netinet/tcp.h>
#include <sys/socket.h>
#include <sys/types.h>
#include <unistd.h>
#endif

namespace knc::net {
namespace {

sockaddr_in makeAddr(const std::string& ip, unsigned short port) {
    sockaddr_in addr{};
    addr.sin_family = AF_INET;
    addr.sin_port = htons(port);
    if (ip.empty()) {
        addr.sin_addr.s_addr = htonl(INADDR_ANY);
    } else if (::inet_pton(AF_INET, ip.c_str(), &addr.sin_addr) != 1) {
        addr.sin_addr.s_addr = htonl(INADDR_ANY);
    }
    return addr;
}

}  // namespace

bool startupSockets() {
#if defined(_WIN32)
    static std::once_flag once;
    static int result = -1;
    std::call_once(once, [] {
        WSADATA data{};
        result = ::WSAStartup(MAKEWORD(2, 2), &data);
    });
    return result == 0;
#else
    static std::once_flag once;
    std::call_once(once, [] { std::signal(SIGPIPE, SIG_IGN); });
    return true;
#endif
}

void cleanupSockets() {
#if defined(_WIN32)
    ::WSACleanup();
#endif
}

std::string lastSocketError() {
#if defined(_WIN32)
    return "wsa error " + std::to_string(::WSAGetLastError());
#else
    return std::strerror(errno);
#endif
}

namespace {
bool applyTimeout(SocketHandle sock, int option, int milliseconds) {
#if defined(_WIN32)
    DWORD value = static_cast<DWORD>(milliseconds);
    return ::setsockopt(static_cast<SOCKET>(sock), SOL_SOCKET, option,
                        reinterpret_cast<const char*>(&value), sizeof(value)) == 0;
#else
    timeval value{};
    value.tv_sec = milliseconds / 1000;
    value.tv_usec = (milliseconds % 1000) * 1000;
    return ::setsockopt(static_cast<int>(sock), SOL_SOCKET, option, &value, sizeof(value)) == 0;
#endif
}
}  // namespace

bool setRecvTimeout(SocketHandle sock, int milliseconds) {
    return applyTimeout(sock, SO_RCVTIMEO, milliseconds);
}

bool setSendTimeout(SocketHandle sock, int milliseconds) {
    return applyTimeout(sock, SO_SNDTIMEO, milliseconds);
}

bool configureSocket(SocketHandle sock, bool reuseAddr) {
    if (sock == kInvalidSocket) return false;
    const int one = 1;
#if defined(_WIN32)
    u_long nonBlocking = 1;
    if (::ioctlsocket(static_cast<SOCKET>(sock), FIONBIO, &nonBlocking) != 0) return false;
    if (reuseAddr)
        ::setsockopt(static_cast<SOCKET>(sock), SOL_SOCKET, SO_REUSEADDR,
                     reinterpret_cast<const char*>(&one), sizeof(one));
    ::setsockopt(static_cast<SOCKET>(sock), IPPROTO_TCP, TCP_NODELAY,
                 reinterpret_cast<const char*>(&one), sizeof(one));
#else
    const int flags = ::fcntl(static_cast<int>(sock), F_GETFL, 0);
    if (flags < 0 || ::fcntl(static_cast<int>(sock), F_SETFL, flags | O_NONBLOCK) < 0) return false;
    if (reuseAddr) ::setsockopt(static_cast<int>(sock), SOL_SOCKET, SO_REUSEADDR, &one, sizeof(one));
    ::setsockopt(static_cast<int>(sock), IPPROTO_TCP, TCP_NODELAY, &one, sizeof(one));
#endif
    return true;
}

SocketHandle listenTcp(unsigned short port, int backlog) {
    const SocketHandle sock = ::socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
    if (sock == kInvalidSocket) return kInvalidSocket;
    if (!configureSocket(sock, true)) {
        closeSocket(sock);
        return kInvalidSocket;
    }
    const sockaddr_in addr = makeAddr({}, port);
    if (::bind(sock, reinterpret_cast<const sockaddr*>(&addr), sizeof(addr)) != 0) {
        closeSocket(sock);
        return kInvalidSocket;
    }
    if (::listen(sock, backlog) != 0) {
        closeSocket(sock);
        return kInvalidSocket;
    }
    return sock;
}

SocketHandle bindUdp(unsigned short port) {
    const SocketHandle sock = ::socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP);
    if (sock == kInvalidSocket) return kInvalidSocket;
    if (!configureSocket(sock, true)) {
        closeSocket(sock);
        return kInvalidSocket;
    }
    const sockaddr_in addr = makeAddr({}, port);
    if (::bind(sock, reinterpret_cast<const sockaddr*>(&addr), sizeof(addr)) != 0) {
        closeSocket(sock);
        return kInvalidSocket;
    }
    return sock;
}

void closeSocket(SocketHandle sock) {
    if (sock == kInvalidSocket) return;
#if defined(_WIN32)
    ::closesocket(static_cast<SOCKET>(sock));
#else
    ::close(static_cast<int>(sock));
#endif
}

SocketHandle acceptSocket(SocketHandle listener, std::string* peerIp, unsigned short* peerPort) {
    sockaddr_in addr{};
#if defined(_WIN32)
    int len = sizeof(addr);
#else
    socklen_t len = sizeof(addr);
#endif
    const SocketHandle sock = ::accept(listener, reinterpret_cast<sockaddr*>(&addr), &len);
    if (sock == kInvalidSocket) return kInvalidSocket;
    configureSocket(sock, false);
    if (peerIp) {
        char buf[64]{};
        ::inet_ntop(AF_INET, &addr.sin_addr, buf, sizeof(buf));
        *peerIp = buf;
    }
    if (peerPort) *peerPort = ntohs(addr.sin_port);
    return sock;
}

int recvSome(SocketHandle sock, void* buffer, std::size_t len) {
    const auto n = ::recv(sock, static_cast<char*>(buffer), static_cast<int>(len), 0);
    if (n > 0) return static_cast<int>(n);
    if (n == 0) return 0;
#if defined(_WIN32)
    const int err = ::WSAGetLastError();
    if (err == WSAEWOULDBLOCK) return -1;
    return -2;
#else
    if (errno == EAGAIN || errno == EWOULDBLOCK || errno == EINTR) return -1;
    return -2;
#endif
}

int sendSome(SocketHandle sock, const void* buffer, std::size_t len) {
    const auto n = ::send(sock, static_cast<const char*>(buffer), static_cast<int>(len), 0);
    if (n >= 0) return static_cast<int>(n);
#if defined(_WIN32)
    const int err = ::WSAGetLastError();
    if (err == WSAEWOULDBLOCK) return -1;
    return -2;
#else
    if (errno == EAGAIN || errno == EWOULDBLOCK || errno == EINTR) return -1;
    return -2;
#endif
}

int socketError(SocketHandle sock) {
    int err = 0;
#if defined(_WIN32)
    int len = sizeof(err);
    if (::getsockopt(static_cast<SOCKET>(sock), SOL_SOCKET, SO_ERROR, reinterpret_cast<char*>(&err), &len) != 0)
        return 1;
#else
    socklen_t len = sizeof(err);
    if (::getsockopt(static_cast<int>(sock), SOL_SOCKET, SO_ERROR, &err, &len) != 0) return 1;
#endif
    return err != 0 ? 1 : 0;
}

int recvFrom(SocketHandle sock, void* buffer, std::size_t len, std::string* fromIp, unsigned short* fromPort) {
    sockaddr_in addr{};
#if defined(_WIN32)
    int addrLen = sizeof(addr);
#else
    socklen_t addrLen = sizeof(addr);
#endif
    const int n = static_cast<int>(::recvfrom(sock, static_cast<char*>(buffer), static_cast<int>(len), 0,
                                             reinterpret_cast<sockaddr*>(&addr), &addrLen));
    if (n <= 0) return n;
    if (fromIp) {
        char buf[64]{};
        ::inet_ntop(AF_INET, &addr.sin_addr, buf, sizeof(buf));
        *fromIp = buf;
    }
    if (fromPort) *fromPort = ntohs(addr.sin_port);
    return n;
}

int sendTo(SocketHandle sock, const void* buffer, std::size_t len, const std::string& ip, unsigned short port) {
    const sockaddr_in addr = makeAddr(ip, port);
    return static_cast<int>(::sendto(sock, static_cast<const char*>(buffer), static_cast<int>(len), 0,
                                     reinterpret_cast<const sockaddr*>(&addr), sizeof(addr)));
}

std::string endpointString(const std::string& ip, unsigned short port) {
    return ip + ":" + std::to_string(port);
}

std::string localIpv4Address() {
#if defined(_WIN32)
    char host[256]{};
    if (::gethostname(host, sizeof(host)) != 0) return "127.0.0.1";
    addrinfo hints{};
    hints.ai_family = AF_INET;
    addrinfo* result = nullptr;
    if (::getaddrinfo(host, nullptr, &hints, &result) != 0 || result == nullptr) return "127.0.0.1";
    char buf[64]{};
    auto* addr = reinterpret_cast<sockaddr_in*>(result->ai_addr);
    ::inet_ntop(AF_INET, &addr->sin_addr, buf, sizeof(buf));
    ::freeaddrinfo(result);
    std::string ip = buf;
    return ip.empty() ? "127.0.0.1" : ip;
#else
    ifaddrs* list = nullptr;
    if (::getifaddrs(&list) != 0) return "127.0.0.1";
    for (ifaddrs* it = list; it != nullptr; it = it->ifa_next) {
        if (!it->ifa_addr || it->ifa_addr->sa_family != AF_INET) continue;
        char buf[64]{};
        auto* addr = reinterpret_cast<sockaddr_in*>(it->ifa_addr);
        ::inet_ntop(AF_INET, &addr->sin_addr, buf, sizeof(buf));
        const std::string ip = buf;
        if (ip.rfind("127.", 0) == 0) continue;
        ::freeifaddrs(list);
        return ip;
    }
    ::freeifaddrs(list);
    return "127.0.0.1";
#endif
}

}  // namespace knc::net
