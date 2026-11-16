#include "kcore/Log.h"

#include <ctime>

namespace knc {

Logger& Logger::instance() {
    static Logger logger;
    return logger;
}

bool Logger::openFile(const std::string& path) {
    std::lock_guard<std::mutex> lock(mutex_);
    if (file_) {
        std::fclose(file_);
        file_ = nullptr;
    }
    file_ = std::fopen(path.c_str(), "ab");
    return file_ != nullptr;
}

void Logger::closeFile() {
    std::lock_guard<std::mutex> lock(mutex_);
    if (file_) {
        std::fflush(file_);
        std::fclose(file_);
        file_ = nullptr;
    }
}

void Logger::write(LogLevel lvl, std::string_view tag, std::string_view message) {
    if (static_cast<int>(lvl) > static_cast<int>(level_)) return;

    using clock = std::chrono::system_clock;
    const auto now = clock::now();
    const std::time_t secs = clock::to_time_t(now);
    const auto ms = std::chrono::duration_cast<std::chrono::milliseconds>(now.time_since_epoch()) % 1000;

    std::tm tm{};
#if defined(_WIN32)
    localtime_s(&tm, &secs);
#else
    localtime_r(&secs, &tm);
#endif

    char stamp[40];
    std::snprintf(stamp, sizeof(stamp), "%02d:%02d:%02d.%03d", tm.tm_hour, tm.tm_min, tm.tm_sec,
                  static_cast<int>(ms.count()));

    std::lock_guard<std::mutex> lock(mutex_);
    std::FILE* out = (lvl <= LogLevel::kError) ? stderr : stdout;
    std::fprintf(out, "[%s][%s][%.*s] %.*s\n", stamp, detail::levelTag(lvl), static_cast<int>(tag.size()),
                 tag.data(), static_cast<int>(message.size()), message.data());
    std::fflush(out);
    if (file_) {
        std::fprintf(file_, "[%s][%s][%.*s] %.*s\n", stamp, detail::levelTag(lvl), static_cast<int>(tag.size()),
                     tag.data(), static_cast<int>(message.size()), message.data());
    }
}

}  // namespace knc
