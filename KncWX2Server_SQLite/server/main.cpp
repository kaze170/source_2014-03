// server/main.cpp - kncserver: the single process that replaces the five 2014 executables.
//
//   kncserver [--config config/knc.ini] [--port login=9001,channel=9101] [--version <tag>]
//             [--db data/knc.db] [--print-config] [--check-config] [--seconds N] [--log-level debug]
//
// The defaults match config/knc.ini: login on 9000, channel on 9100, game on 9200, one channel in
// one server set, database data/knc.db.
#include <csignal>
#include <cstdio>
#include <cstdlib>
#include <cstring>
#include <string>

#include "kcore/Log.h"
#include "kcore/Types.h"
#include "kserver/Config.h"
#include "kserver/Server.h"

namespace {

knc::server::Server* g_server = nullptr;

void onSignal(int) {
    if (g_server) {
        KLOG(knc::LogLevel::kInfo, "server") << "signal received, shutting down";
        g_server->reactor().stop();
    }
}

void usage() {
    std::printf(
        "kncserver - Elsword X2 single process server (SQLite)\n"
        "\n"
        "  --config <path>        ini file to load (default config/knc.ini)\n"
        "  --db <path>            database file, overrides [Database] Path\n"
        "  --port <role=port>     override a listener port; role is login|channel|game|fep; repeatable\n"
        "  --version <tag>        override the announced version string\n"
        "  --log-level <level>    error|warn|info|debug|off\n"
        "  --seconds <n>          stop after n seconds (0 = run until a signal)\n"
        "  --print-config         print the effective configuration and exit\n"
        "  --check-config         validate the configuration and exit\n"
        "  --help                 this text\n");
}

bool parsePortOverrides(const std::string& value, knc::server::Config& config, std::string* error) {
    // "login=9001,channel=9101"
    std::size_t begin = 0;
    while (begin <= value.size()) {
        const std::size_t comma = value.find(',', begin);
        const std::string item = value.substr(begin, comma == std::string::npos ? std::string::npos : comma - begin);
        const std::size_t equals = item.find('=');
        if (equals == std::string::npos) {
            if (error) *error = "bad --port '" + item + "' (expected role=port)";
            return false;
        }
        const std::string role = item.substr(0, equals);
        const long port = std::strtol(item.c_str() + equals + 1, nullptr, 10);
        if (port <= 0 || port > 65535) {
            if (error) *error = "bad port in --port '" + item + "'";
            return false;
        }
        if (role == "login") {
            config.login.port = static_cast<unsigned short>(port);
        } else if (role == "channel") {
            config.channel.port = static_cast<unsigned short>(port);
        } else if (role == "game") {
            config.game.port = static_cast<unsigned short>(port);
        } else if (role == "fep") {
            config.fep.port = static_cast<unsigned short>(port);
            config.fep.enabled = true;
        } else {
            if (error) *error = "unknown port role '" + role + "'";
            return false;
        }
        if (comma == std::string::npos) break;
        begin = comma + 1;
    }
    return true;
}

}  // namespace

int main(int argc, char** argv) {
    std::string configPath = "config/knc.ini";
    bool printConfig = false;
    bool checkConfig = false;
    std::string error;

    // Command line overrides are collected first and applied *after* the ini file is loaded, so
    // "--port login=19000" always wins over whatever config/knc.ini says.
    struct Overrides {
        bool hasDb = false;
        std::string db;
        bool hasVersion = false;
        std::string version;
        bool hasLogLevel = false;
        knc::LogLevel logLevel = knc::LogLevel::kInfo;
        bool hasSeconds = false;
        int seconds = 0;
        std::vector<std::string> ports;
    } overrides;

    // Start from the built-in defaults so a missing ini file is not fatal.
    knc::server::Config config = knc::server::Config::defaults();

    for (int i = 1; i < argc; ++i) {
        const std::string arg = argv[i];
        auto next = [&](const char* name) -> const char* {
            if (i + 1 >= argc) {
                std::fprintf(stderr, "%s requires a value\n", name);
                std::exit(2);
            }
            return argv[++i];
        };
        if (arg == "--help" || arg == "-h") {
            usage();
            return 0;
        } else if (arg == "--config") {
            configPath = next("--config");
        } else if (arg == "--db") {
            overrides.hasDb = true;
            overrides.db = next("--db");
        } else if (arg == "--version") {
            overrides.hasVersion = true;
            overrides.version = next("--version");
        } else if (arg == "--log-level") {
            const std::string value = next("--log-level");
            overrides.hasLogLevel = true;
            if (value == "error") {
                overrides.logLevel = knc::LogLevel::kError;
            } else if (value == "warn") {
                overrides.logLevel = knc::LogLevel::kWarn;
            } else if (value == "debug") {
                overrides.logLevel = knc::LogLevel::kDebug;
            } else if (value == "off") {
                overrides.logLevel = knc::LogLevel::kOff;
            } else {
                overrides.logLevel = knc::LogLevel::kInfo;
            }
        } else if (arg == "--seconds") {
            overrides.hasSeconds = true;
            overrides.seconds = std::atoi(next("--seconds"));
        } else if (arg == "--port") {
            overrides.ports.emplace_back(next("--port"));
        } else if (arg == "--print-config") {
            printConfig = true;
        } else if (arg == "--check-config") {
            checkConfig = true;
        } else {
            std::fprintf(stderr, "unknown argument '%s' (try --help)\n", arg.c_str());
            return 2;
        }
    }

    // An explicit --config that does not exist is an error; the built-in default is not.
    knc::IniFile ini;
    const bool hasFile = ini.load(configPath, &error);
    if (hasFile) {
        if (!config.applyIni(ini, configPath, &error)) {
            std::fprintf(stderr, "config error: %s\n", error.c_str());
            return 1;
        }
    } else if (configPath != "config/knc.ini") {
        std::fprintf(stderr, "config error: %s\n", error.c_str());
        return 1;
    } else {
        std::fprintf(stderr, "warning: %s not found, using built-in defaults\n", configPath.c_str());
    }

    // ---- command line overrides win over the file --------------------------------
    if (overrides.hasDb) config.database.path = overrides.db;
    if (overrides.hasVersion) {
        config.internalVersion = knc::toWide(overrides.version);
        config.openTestVersion = config.internalVersion;
        config.serviceVersion = config.internalVersion;
    }
    if (overrides.hasLogLevel) config.logLevel = overrides.logLevel;
    if (overrides.hasSeconds) config.shutdownAfterSeconds = overrides.seconds;
    for (const auto& ports : overrides.ports) {
        if (!parsePortOverrides(ports, config, &error)) {
            std::fprintf(stderr, "%s\n", error.c_str());
            return 2;
        }
    }
    // A --port override also has to move the channel/server-set entries the client receives,
    // otherwise the client would still be redirected to the old port.
    if (!overrides.ports.empty() && !config.channels.empty()) {
        for (auto& channel : config.channels) channel.masterPort = config.channel.port;
        for (auto& set : config.serverSets) set.port = config.channel.port;
    }

    if (!config.validate(&error)) {
        std::fprintf(stderr, "config error: %s\n", error.c_str());
        return 1;
    }

    if (printConfig) {
        std::printf("%s", config.describe().c_str());
        return 0;
    }
    if (checkConfig) {
        std::printf("%s: configuration is valid (%zu channel(s), %zu server set(s))\n",
                    configPath.c_str(), config.channels.size(), config.serverSets.size());
        return 0;
    }

    knc::Logger::instance().setLevel(config.logLevel);
    knc::server::Server server(config);
    g_server = &server;
    std::signal(SIGINT, onSignal);
    std::signal(SIGTERM, onSignal);
#if defined(SIGPIPE)
    std::signal(SIGPIPE, SIG_IGN);  // a dead peer must surface as a send error, not a signal
#endif

    if (!server.start(&error)) {
        std::fprintf(stderr, "startup failed: %s\n", error.c_str());
        return 1;
    }
    KLOG(knc::LogLevel::kInfo, "server") << "listening (login " << config.login.port << ", channel "
                                         << config.channel.port << ", game " << config.game.port << ")";
    server.run();
    server.stop();
    g_server = nullptr;
    KLOG(knc::LogLevel::kInfo, "server") << "stopped";
    return 0;
}
