// kcore/Log.h - small, thread safe, level filtered logger that mirrors the original dbg::clog
// streams used all over KncWX2Server.
#pragma once

#include <array>
#include <chrono>
#include <cstdio>
#include <mutex>
#include <sstream>
#include <string>
#include <string_view>

namespace knc {

enum class LogLevel : int {
    kOff = -1,
    kError = 0,   // cerr
    kWarn = 1,    // cwarn
    kInfo = 2,    // clog
    kDebug = 3,   // clog2
};

class Logger {
public:
    static Logger& instance();

    void setLevel(LogLevel lvl) { level_ = lvl; }
    LogLevel level() const { return level_; }
    // Optional: also append to a file next to the database.
    bool openFile(const std::string& path);
    void closeFile();

    void write(LogLevel lvl, std::string_view tag, std::string_view message);

private:
    Logger() = default;
    std::mutex mutex_;
    LogLevel level_ = LogLevel::kInfo;
    std::FILE* file_ = nullptr;
};

namespace detail {
inline const char* levelTag(LogLevel lvl) {
    switch (lvl) {
        case LogLevel::kError: return "ERR ";
        case LogLevel::kWarn: return "WARN";
        case LogLevel::kInfo: return "INFO";
        default: return "DBG ";
    }
}
}  // namespace detail

// Usage: KLOG(LogLevel::kInfo, "net") << "connected " << ip;
class LogLine {
public:
    LogLine(LogLevel lvl, std::string_view tag) : lvl_(lvl), tag_(tag) {}
    LogLine(const LogLine&) = delete;
    LogLine& operator=(const LogLine&) = delete;
    ~LogLine() {
        Logger::instance().write(lvl_, tag_, stream_.str());
    }
    template <typename T>
    LogLine& operator<<(const T& value) {
        stream_ << value;
        return *this;
    }

private:
    LogLevel lvl_;
    std::string_view tag_;
    std::ostringstream stream_;
};

}  // namespace knc

namespace knc::detail {
// Null sink used when a log statement is filtered out; keeps `KLOG(...) << x` well formed in any
// context (including `if (x) KLOG(...)` where the dangling-else problem would otherwise appear).
struct NullLogLine {
    template <typename T>
    NullLogLine& operator<<(const T&) {
        return *this;
    }
};
inline bool logEnabled(LogLevel lvl) {
    return static_cast<int>(lvl) <= static_cast<int>(Logger::instance().level());
}
}  // namespace knc::detail

#define KLOG(lvl, tag) \
    for (bool klog_once_ = ::knc::detail::logEnabled(lvl); klog_once_; klog_once_ = false) \
    ::knc::LogLine(lvl, tag)
