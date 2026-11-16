// knet/Reactor.h - portable readiness loop (poll(2) / WSAPoll) with timers.
//
// One Reactor runs per process on the network thread. Sockets are non blocking; every readable
// socket is drained until EWOULDBLOCK so a slow peer cannot starve the loop.
#pragma once

#include <chrono>
#include <functional>
#include <map>
#include <memory>
#include <mutex>
#include <string>
#include <vector>

#include "kcore/Types.h"
#include "knet/Socket.h"

namespace knc::net {

class Reactor;

// A registered I/O object. Implementations return false from the callbacks to be closed.
class Handler {
public:
    virtual ~Handler() = default;
    virtual SocketHandle handle() const = 0;
    virtual void onReadable(Reactor& reactor) = 0;
    virtual void onWritable(Reactor& reactor) { (void)reactor; }
    virtual void onClose(Reactor& reactor) { (void)reactor; }
    virtual bool wantsWrite() const { return false; }
    virtual const char* tag() const { return "handler"; }
};

class Reactor {
public:
    using TimerId = UINT64;

    explicit Reactor(std::string name);
    ~Reactor();

    Reactor(const Reactor&) = delete;
    Reactor& operator=(const Reactor&) = delete;

    // Ownership is taken unless shared ownership is provided.
    void add(std::shared_ptr<Handler> handler);
    void remove(const std::shared_ptr<Handler>& handler);
    void removeHandle(SocketHandle handle);
    // Requests the handler be closed after the current poll round.
    void closeLater(std::shared_ptr<Handler> handler);

    // Runs until stop() is called. `maxIterations` is only used by the tests.
    void run();
    void runOnce(int timeoutMs);
    void stop() { running_ = false; }
    bool running() const { return running_; }

    TimerId scheduleOnce(int delayMs, std::function<void()> callback);
    TimerId scheduleRepeating(int intervalMs, std::function<void()> callback);
    void cancelTimer(TimerId id);

    // Runs `fn` on the loop thread at the next iteration (thread safe). Used by DB workers.
    void post(std::function<void()> fn);

    const std::string& name() const { return name_; }
    std::size_t handlerCount() const { return handlers_.size(); }

    // Diagnostics.
    UINT64 iterations() const { return iterations_; }

private:
    struct TimerEntry {
        TimerId id = 0;
        std::chrono::steady_clock::time_point due;
        int intervalMs = 0;  // 0 == one shot
        std::function<void()> callback;
    };

    void runPending();
    void runTimers();
    int nextTimeoutMs() const;

    std::string name_;
    bool running_ = false;
    UINT64 iterations_ = 0;
    std::vector<std::shared_ptr<Handler>> handlers_;
    std::vector<std::shared_ptr<Handler>> pendingAdd_;
    std::vector<std::shared_ptr<Handler>> pendingClose_;
    std::map<TimerId, TimerEntry> timers_;
    TimerId nextTimerId_ = 1;

    std::mutex postMutex_;
    std::vector<std::function<void()>> posted_;
};

}  // namespace knc::net
