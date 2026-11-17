// ---------------------------------------------------------------------------
// test_util.hpp - a deliberately tiny assertion harness.
//
// The conformance tests compile the project's .cppm sources in KNC_CLASSIC
// mode (see src/knc_prelude.hpp), so they are plain, dependency-free C++ and
// can run anywhere a compiler exists - including CI boxes without MSVC.
// ---------------------------------------------------------------------------
#pragma once

#ifndef KNC_CLASSIC
#  define KNC_CLASSIC 1
#endif

#include <cstdint>
#include <cstdio>
#include <exception>
#include <iostream>
#include <sstream>
#include <string>
#include <string_view>
#include <vector>

namespace knctest {

inline int g_failures = 0;
inline int g_checks = 0;

[[nodiscard]] inline std::vector<std::uint8_t> from_hex(std::string_view hex) {
    auto nibble = [](char c) -> int {
        if (c >= '0' && c <= '9') return c - '0';
        if (c >= 'a' && c <= 'f') return c - 'a' + 10;
        if (c >= 'A' && c <= 'F') return c - 'A' + 10;
        return -1;
    };
    std::vector<std::uint8_t> out;
    int high = -1;
    for (char c : hex) {
        const int v = nibble(c);
        if (v < 0) continue;  // allow spaces / separators
        if (high < 0) {
            high = v;
        } else {
            out.push_back(static_cast<std::uint8_t>((high << 4) | v));
            high = -1;
        }
    }
    return out;
}

[[nodiscard]] inline std::string to_hex(const std::vector<std::uint8_t>& bytes) {
    static const char* kDigits = "0123456789abcdef";
    std::string out;
    out.reserve(bytes.size() * 2);
    for (std::uint8_t b : bytes) {
        out.push_back(kDigits[b >> 4]);
        out.push_back(kDigits[b & 0x0F]);
    }
    return out;
}

[[nodiscard]] inline std::vector<std::uint8_t> from_ascii(std::string_view text) {
    return std::vector<std::uint8_t>{text.begin(), text.end()};
}

inline void report(bool ok, const std::string& what, const std::string& detail = {}) {
    ++g_checks;
    if (ok) {
        return;
    }
    ++g_failures;
    std::cout << "    FAIL: " << what << '\n';
    if (!detail.empty()) {
        std::cout << "          " << detail << '\n';
    }
}

inline void check_eq_bytes(const std::vector<std::uint8_t>& got,
                           const std::vector<std::uint8_t>& want,
                           const std::string& what) {
    const bool ok = got == want;
    report(ok, what,
           ok ? std::string{}
              : ("got  " + to_hex(got) + "\n          want " + to_hex(want)));
}

/// A named group of checks.
class Suite {
public:
    explicit Suite(std::string name) : name_(std::move(name)) {
        std::cout << "== " << name_ << " ==\n";
    }

    template <typename Fn>
    void run(std::string_view label, Fn&& fn) {
        const int before = g_failures;
        std::cout << "  - " << label << '\n';
        try {
            fn();
        } catch (const std::exception& ex) {
            report(false, std::string{label} + " threw", ex.what());
        } catch (...) {
            report(false, std::string{label} + " threw a non-std exception");
        }
        if (g_failures != before) {
            std::cout << "    ^^ " << (g_failures - before) << " failure(s)\n";
        }
    }

    [[nodiscard]] int summary() const {
        std::cout << "-- " << name_ << ": " << g_checks << " checks, " << g_failures
                  << " failure(s)\n";
        return g_failures == 0 ? 0 : 1;
    }

private:
    std::string name_;
};

}  // namespace knctest

#define KNC_CHECK(cond, what) ::knctest::report((cond), (what))

#define KNC_CHECK_EQ(got, want, what)                                        \
    do {                                                                     \
        const auto& knc_got_ = (got);                                        \
        const auto& knc_want_ = (want);                                      \
        std::ostringstream knc_oss_;                                         \
        knc_oss_ << "got " << knc_got_ << ", want " << knc_want_;            \
        ::knctest::report(knc_got_ == knc_want_, (what), knc_oss_.str());    \
    } while (false)

#define KNC_CHECK_EQ_BYTES(got, want, what) \
    ::knctest::check_eq_bytes((got), (want), (what))

#define KNC_CHECK_THROWS(expr, what)                             \
    do {                                                         \
        bool knc_threw_ = false;                                 \
        try {                                                    \
            (void)(expr);                                        \
        } catch (...) {                                          \
            knc_threw_ = true;                                   \
        }                                                        \
        ::knctest::report(knc_threw_, (what), "expected a throw"); \
    } while (false)
