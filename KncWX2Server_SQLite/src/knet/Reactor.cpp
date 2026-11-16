#include "knet/Reactor.h"

#include <algorithm>
#include <cstring>

#if defined(_WIN32)
#include <winsock2.h>
#include <ws2tcpip.h>
#else
#include <poll.h>
#endif

#include "kcore/Log.h"

namespace knc::net {

Reactor::Reactor(std::string name) : name_(std::move(name)) {}
Reactor::~Reactor() = default;

void Reactor::add(std::shared_ptr<Handler> handler) {
    if (!handler) return;
    pendingAdd_.push_back(std::move(handler));
}

void Reactor::remove(const std::shared_ptr<Handler>& handler) {
    if (!handler) return;
    handlers_.erase(std::remove(handlers_.begin(), handlers_.end(), handler), handlers_.end());
}

void Reactor::removeHandle(SocketHandle handle) {
    handlers_.erase(std::remove_if(handlers_.begin(), handlers_.end(),
                                   [handle, this](const std::shared_ptr<Handler>& h) {
                                       if (h && h->handle() == handle) {
                                           h->onClose(*this);
                                           return true;
                                       }
                                       return false;
                                   }),
                    handlers_.end());
}

void Reactor::closeLater(std::shared_ptr<Handler> handler) {
    if (!handler) return;
    pendingClose_.push_back(std::move(handler));
}

void Reactor::post(std::function<void()> fn) {
    std::lock_guard<std::mutex> guard(postMutex_);
    posted_.push_back(std::move(fn));
}

void Reactor::runPending() {
    if (!pendingAdd_.empty()) {
        for (auto& h : pendingAdd_) handlers_.push_back(std::move(h));
        pendingAdd_.clear();
    }
    if (!pendingClose_.empty()) {
        for (auto& h : pendingClose_) {
            const auto it = std::find(handlers_.begin(), handlers_.end(), h);
            if (it != handlers_.end()) {
                (*it)->onClose(*this);
                handlers_.erase(it);
            }
        }
        pendingClose_.clear();
    }
    std::vector<std::function<void()>> jobs;
    {
        std::lock_guard<std::mutex> guard(postMutex_);
        jobs.swap(posted_);
    }
    for (auto& job : jobs) job();
}

int Reactor::nextTimeoutMs() const {
    if (timers_.empty()) return 200;
    const auto now = std::chrono::steady_clock::now();
    int best = 1000;
    for (const auto& [id, entry] : timers_) {
        const auto deltaMs =
            std::chrono::duration_cast<std::chrono::milliseconds>(entry.due - now).count();
        best = std::min(best, static_cast<int>(std::max<std::int64_t>(0, deltaMs)));
    }
    return std::max(0, std::min(best, 200));
}

void Reactor::runTimers() {
    if (timers_.empty()) return;
    const auto now = std::chrono::steady_clock::now();
    std::vector<TimerEntry> due;
    for (auto it = timers_.begin(); it != timers_.end();) {
        if (it->second.due <= now) {
            due.push_back(it->second);
            if (it->second.intervalMs > 0) {
                it->second.due = now + std::chrono::milliseconds(it->second.intervalMs);
                ++it;
            } else {
                it = timers_.erase(it);
            }
        } else {
            ++it;
        }
    }
    for (auto& entry : due) entry.callback();
}

void Reactor::run() {
    running_ = true;
    while (running_) runOnce(nextTimeoutMs());
}

void Reactor::runOnce(int timeoutMs) {
    runPending();
    runTimers();
    ++iterations_;

    if (handlers_.empty()) {
        if (timeoutMs > 0) {
#if defined(_WIN32)
            ::Sleep(static_cast<DWORD>(timeoutMs));
#else
            ::poll(nullptr, 0, timeoutMs);
#endif
        }
        return;
    }

    std::vector<pollfd> fds;
    fds.reserve(handlers_.size());
    for (auto& h : handlers_) {
        pollfd p{};
        p.fd = static_cast<decltype(p.fd)>(h->handle());
        p.events = POLLIN;
        if (h->wantsWrite()) p.events |= POLLOUT;
        fds.push_back(p);
    }

#if defined(_WIN32)
    const int ready = ::WSAPoll(fds.data(), static_cast<ULONG>(fds.size()), timeoutMs);
#else
    const int ready = ::poll(fds.data(), static_cast<nfds_t>(fds.size()), timeoutMs);
#endif
    if (ready <= 0) return;

    // Copy the handler list: callbacks may add/remove handlers.
    const std::vector<std::shared_ptr<Handler>> snapshot = handlers_;
    for (std::size_t i = 0; i < fds.size() && i < snapshot.size(); ++i) {
        const auto& handler = snapshot[i];
        if (!handler || handler->handle() != static_cast<SocketHandle>(fds[i].fd)) continue;
        if (fds[i].revents == 0) continue;
        if (fds[i].revents & (POLLERR | POLLHUP | POLLNVAL)) {
            closeLater(handler);
            continue;
        }
        if (fds[i].revents & POLLOUT) handler->onWritable(*this);
        if (fds[i].revents & POLLIN) handler->onReadable(*this);
    }
    runPending();
}

Reactor::TimerId Reactor::scheduleOnce(int delayMs, std::function<void()> callback) {
    const TimerId id = nextTimerId_++;
    TimerEntry entry;
    entry.id = id;
    entry.due = std::chrono::steady_clock::now() + std::chrono::milliseconds(delayMs);
    entry.intervalMs = 0;
    entry.callback = std::move(callback);
    timers_.emplace(id, std::move(entry));
    return id;
}

Reactor::TimerId Reactor::scheduleRepeating(int intervalMs, std::function<void()> callback) {
    const TimerId id = nextTimerId_++;
    TimerEntry entry;
    entry.id = id;
    entry.due = std::chrono::steady_clock::now() + std::chrono::milliseconds(intervalMs);
    entry.intervalMs = intervalMs > 0 ? intervalMs : 1;
    entry.callback = std::move(callback);
    timers_.emplace(id, std::move(entry));
    return id;
}

void Reactor::cancelTimer(TimerId id) { timers_.erase(id); }

}  // namespace knc::net
