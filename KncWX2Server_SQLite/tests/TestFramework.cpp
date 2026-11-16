#include "TestFramework.h"

#include <cstring>
#include <iostream>

namespace knctest {
namespace {
int g_failures = 0;
std::string g_current;
}  // namespace

std::vector<TestCase>& registry() {
    static std::vector<TestCase> cases;
    return cases;
}

void reportFailure(const char* file, int line, const std::string& message) {
    ++g_failures;
    std::fprintf(stderr, "  FAIL %s:%d %s\n", file, line, message.c_str());
}

int runAll(int argc, char** argv) {
    std::string filter;
    for (int i = 1; i < argc; ++i) {
        if (std::strcmp(argv[i], "--filter") == 0 && i + 1 < argc) filter = argv[++i];
    }

    int executed = 0;
    int failedSuites = 0;
    for (auto& test : registry()) {
        const std::string full = test.suite + "." + test.name;
        if (!filter.empty() && full.find(filter) == std::string::npos) continue;
        ++executed;
        const int before = g_failures;
        std::printf("[ RUN ] %s\n", full.c_str());
        try {
            test.fn();
        } catch (const std::exception& ex) {
            reportFailure(__FILE__, __LINE__, std::string("threw: ") + ex.what());
        }
        if (g_failures != before) {
            ++failedSuites;
            std::printf("[FAIL ] %s\n", full.c_str());
        } else {
            std::printf("[  OK ] %s\n", full.c_str());
        }
    }

    std::printf("\n%d test(s) executed, %d failed, %d assertion failure(s)\n", executed, failedSuites, g_failures);
    return g_failures == 0 ? 0 : 1;
}

}  // namespace knctest

int main(int argc, char** argv) { return knctest::runAll(argc, argv); }
