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
    std::lock_guard<std::mutex> guard(handlerMutex_);
    pendingAdd_.push_back(std::move(handler));
}

void Reactor::remove(const std::shared_ptr<Handler>& handler) {
    if (!handler) return;
    std::lock_guard<std::mutex> guard(handlerMutex_);
    handlers_.erase(std::remove(handlers_.begin(), handlers_.end(), handler), handlers_.end());
    pendingAdd_.erase(std::remove(pendingAdd_.begin(), pendingAdd_.end(), handler), pendingAdd_.end());
}

void Reactor::removeHandle(SocketHandle handle) {
    std::lock_guard<std::mutex> guard(handlerMutex_);
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
    std::lock_guard<std::mutex> guard(handlerMutex_);
    pendingClose_.push_back(std::move(handler));
}

void Reactor::post(std::function<void()> fn) {
    std::lock_guard<std::mutex> guard(postMutex_);
    posted_.push_back(std::move(fn));
}

void Reactor::runPending() {
    // Take a snapshot under the lock, then do the callbacks without it: onClose() reaches back into
    // the listener, which may call add()/remove() and would deadlock on a non recursive lock.
    std::vector<std::shared_ptr<Handler>> added;
    std::vector<std::shared_ptr<Handler>> closing;
    {
        std::lock_guard<std::mutex> guard(handlerMutex_);
        added.swap(pendingAdd_);
        closing.swap(pendingClose_);
    }
    if (!added.empty()) {
        std::lock_guard<std::mutex> guard(handlerMutex_);
        for (auto& h : added) handlers_.push_back(std::move(h));
    }
    if (!closing.empty()) {
        std::lock_guard<std::mutex> guard(handlerMutex_);
        handlers_.erase(std::remove_if(handlers_.begin(), handlers_.end(),
                                       [&closing](const std::shared_ptr<Handler>& h) {
                                           return std::find(closing.begin(), closing.end(), h) !=
                                                  closing.end();
                                       }),
                        handlers_.end());
    }
    for (auto& h : closing) h->onClose(*this);
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

    // Snapshot the handlers: callbacks add and remove them from other threads.
    std::vector<std::shared_ptr<Handler>> snapshot;
    {
        std::lock_guard<std::mutex> guard(handlerMutex_);
        snapshot = handlers_;
    }

    if (snapshot.empty()) {
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
    fds.reserve(snapshot.size());
    for (auto& h : snapshot) {
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
