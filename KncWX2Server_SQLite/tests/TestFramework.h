// tests/TestFramework.h - tiny, dependency free test runner (no GoogleTest available offline).
#pragma once

#include <cstdio>
#include <cstring>
#include <type_traits>
#include <cstdlib>
#include <functional>
#include <string>
#include <vector>

namespace knctest {

struct TestCase {
    std::string suite;
    std::string name;
    std::function<void()> fn;
};

std::vector<TestCase>& registry();
void reportFailure(const char* file, int line, const std::string& message);

struct Registrar {
    Registrar(const char* suite, const char* name, std::function<void()> fn) {
        registry().push_back(TestCase{suite, name, std::move(fn)});
    }
};

int runAll(int argc, char** argv);

}  // namespace knctest

#define KTEST(suite, name)                                                        \
    static void ktest_##suite##_##name();                                         \
    static ::knctest::Registrar ktest_reg_##suite##_##name(#suite, #name,         \
                                                           &ktest_##suite##_##name); \
    static void ktest_##suite##_##name()

#define KCHECK(cond)                                                              \
    do {                                                                          \
        if (!(cond)) ::knctest::reportFailure(__FILE__, __LINE__, "KCHECK failed: " #cond); \
    } while (0)

#define KCHECK_EQ(a, b)                                                           \
    do {                                                                          \
        const auto& lhs_ = (a);                                                   \
        const auto& rhs_ = (b);                                                   \
        if (!(lhs_ == rhs_))                                                      \
            ::knctest::reportFailure(__FILE__, __LINE__,                          \
                                     std::string("KCHECK_EQ failed: " #a " == " #b) + \
                                         " (" + ::knctest::toString(lhs_) + " vs " + \
                                         ::knctest::toString(rhs_) + ")");        \
    } while (0)

#define KCHECK_BYTES(actual, expected, len)                                       \
    do {                                                                          \
        if (std::memcmp((actual), (expected), (len)) != 0)                        \
            ::knctest::reportFailure(__FILE__, __LINE__,                          \
                                     "KCHECK_BYTES failed: " #actual " != " #expected); \
    } while (0)

namespace knctest {
template <class T>
std::string toString(const T& v);

inline std::string toString(const std::string& v) { return v; }
inline std::string toString(const char* v) { return v ? v : "(null)"; }
inline std::string toString(bool v) { return v ? "true" : "false"; }
template <class T>
std::string toString(const T& v) {
    if constexpr (std::is_arithmetic_v<T>) {
        return std::to_string(v);
    } else {
        return "<value>";
    }
}

}  // namespace knctest
