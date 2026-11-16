#include "kserver/Config.h"

#include <algorithm>
#include <cstdlib>
#include <sstream>

namespace knc::server {
namespace {

std::vector<std::string> split(const std::string& value, char separator) {
    std::vector<std::string> parts;
    std::string current;
    for (char c : value) {
        if (c == separator) {
            parts.push_back(current);
            current.clear();
        } else {
            current.push_back(c);
        }
    }
    parts.push_back(current);
    for (auto& part : parts) {
        std::size_t begin = 0;
        while (begin < part.size() && (part[begin] == ' ' || part[begin] == '\t')) ++begin;
        std::size_t end = part.size();
        while (end > begin && (part[end - 1] == ' ' || part[end - 1] == '\t')) --end;
        part = part.substr(begin, end - begin);
    }
    return parts;
}

bool toInt(const std::string& text, long long* out) {
    if (text.empty()) return false;
    char* end = nullptr;
    const long long value = std::strtoll(text.c_str(), &end, 0);
    if (end == text.c_str() || *end != '\0') return false;
    *out = value;
    return true;
}

unsigned short toPort(const std::string& text) {
    long long value = 0;
    if (!toInt(text, &value) || value < 0 || value > 65535) return 0;
    return static_cast<unsigned short>(value);
}

int toIntOr(const std::string& text, int def) {
    long long value = 0;
    return toInt(text, &value) ? static_cast<int>(value) : def;
}

void readListener(const IniFile& ini, const char* section, ListenerConfig* out, unsigned short defaultPort) {
    if (!ini.hasSection(section)) {
        out->port = defaultPort;
        out->maxUser = 300;
        return;
    }
    out->enabled = ini.getBool(section, "Enable", true);
    out->address = ini.getString(section, "Address", "0.0.0.0");
    out->port = static_cast<unsigned short>(ini.getInt(section, "Port", defaultPort));
    out->maxUser = static_cast<int>(ini.getInt(section, "MaxUser", 300));
    out->checkSequenceNum = ini.getBool(section, "CheckSequenceNum", false);
    const long long authFail = ini.getInt(section, "AuthFailLimit", 100);
    out->authFailLimit = authFail > 0 ? static_cast<unsigned int>(authFail) : 100u;
}

std::string formatVersion(const WideString& value) { return value.empty() ? "<unset>" : toUtf8(value); }

}  // namespace

const WideString& Config::version() const {
    switch (versionKind) {
        case VersionKind::kOpenTest: return openTestVersion;
        case VersionKind::kService: return serviceVersion;
        case VersionKind::kInternal:
        default: return internalVersion;
    }
}

unsigned short Config::channelPortForSet(int serverSetId) const {
    for (const auto& set : serverSets) {
        if (set.id == serverSetId) return set.port;
    }
    // Fall back to the first channel's master port so a client that asks for an unknown set still
    // lands somewhere reachable instead of a dead port.
    return channels.empty() ? 0 : channels.front().masterPort;
}

const ChannelConfig* Config::findChannel(int channelId) const {
    for (const auto& channel : channels) {
        if (channel.channelId == channelId) return &channel;
    }
    return nullptr;
}

Config Config::defaults() {
    Config config;
    config.internalVersion = u"2014031300";
    config.openTestVersion = u"2014031300";
    config.serviceVersion = u"2014031300";
    config.versionKind = VersionKind::kInternal;
    config.database.path = "data/knc.db";

    config.login.address = "0.0.0.0";
    config.login.port = 9000;
    config.login.maxUser = 500;

    config.channel.address = "0.0.0.0";
    config.channel.port = 9100;
    config.channel.maxUser = 300;

    config.game.address = "0.0.0.0";
    config.game.port = 9200;
    config.game.maxUser = 300;

    config.fep.enabled = false;
    config.fep.port = 9300;
    config.fep.maxUser = 100;

    config.center.enabled = false;
    config.center.port = 9400;
    config.center.maxUser = 8;

    config.global.enabled = false;
    config.global.port = 9500;
    config.global.maxUser = 8;

    ChannelConfig channel;
    channel.channelId = 1;
    channel.serverSetId = 1;
    channel.serverUid = 1;
    channel.name = "Ch.1";
    channel.displayName = u"Ch.1";
    channel.masterPort = 9100;
    channel.ncUdpPort = 9151;
    channel.maxUser = 300;
    config.channels.push_back(channel);

    ServerSetConfig set;
    set.id = 1;
    set.name = u"Test Server";
    set.serverUid = 1;
    set.port = 9100;
    set.maxUser = 300;
    config.serverSets.push_back(set);
    return config;
}

bool parseChannelLine(const std::string& value, int index, ChannelConfig* out) {
    if (!out) return false;
    const auto parts = split(value, '|');
    if (parts.size() < 2) return false;

    ChannelConfig channel;
    // Accept both orderings the configs use: "name | port | udp | maxuser" and the fully explicit
    // "id | name | ip | port | udp | maxuser | bonusExp | bonusDrop".
    std::size_t field = 0;
    long long numeric = 0;
    if (toInt(parts[0], &numeric) && parts.size() >= 5) {
        channel.channelId = static_cast<int>(numeric);
        ++field;
    } else {
        channel.channelId = index + 1;
    }
    channel.name = parts[field];
    channel.displayName = cp949ToWide(parts[field]);
    ++field;
    if (field < parts.size() && parts[field].find('.') != std::string::npos &&
        !toInt(parts[field], &numeric)) {
        channel.ip = parts[field];
        ++field;
    }
    if (field < parts.size()) channel.masterPort = toPort(parts[field++]);
    if (field < parts.size()) channel.ncUdpPort = toPort(parts[field++]);
    if (field < parts.size()) channel.maxUser = toIntOr(parts[field++], 0);
    if (field < parts.size()) channel.bonusExpPercent = toIntOr(parts[field++], 0);
    if (field < parts.size()) channel.bonusDropPercent = toIntOr(parts[field++], 0);
    if (field < parts.size()) channel.serverSetId = toIntOr(parts[field++], channel.serverSetId);
    if (field < parts.size()) channel.serverUid = toIntOr(parts[field++], channel.serverUid);

    if (channel.masterPort == 0) return false;
    *out = channel;
    return true;
}

bool parseServerSetLine(const std::string& value, ServerSetConfig* out) {
    if (!out) return false;
    const auto parts = split(value, '|');
    if (parts.size() < 2) return false;

    ServerSetConfig set;
    std::size_t field = 0;
    long long numeric = 0;
    if (toInt(parts[0], &numeric)) {
        set.id = static_cast<int>(numeric);
        ++field;
    }
    set.name = cp949ToWide(parts[field]);
    ++field;
    if (field < parts.size() && !toInt(parts[field], &numeric)) {
        set.ip = parts[field];
        ++field;
    }
    if (field < parts.size()) set.port = toPort(parts[field++]);
    if (field < parts.size()) set.maxUser = toIntOr(parts[field++], 0);
    if (field < parts.size()) set.serverUid = toIntOr(parts[field++], set.id);
    if (set.id == 0) return false;
    *out = set;
    return true;
}

bool Config::load(const std::string& path, std::string* error) {
    IniFile ini;
    if (!ini.load(path, error)) return false;
    return applyIni(ini, path, error);
}

bool Config::loadFromText(const std::string& text, const std::string& sourceName, std::string* error) {
    IniFile ini;
    if (!ini.parse(text, sourceName, error)) return false;
    return applyIni(ini, sourceName, error);
}

bool Config::applyIni(const IniFile& ini, const std::string& source,
                      std::string* error) {
    Config& config = *this;

    config.internalVersion = ini.getWideString("Version", "Internal Version", config.internalVersion);
    config.openTestVersion = ini.getWideString("Version", "OpenTest Version", config.openTestVersion);
    config.serviceVersion = ini.getWideString("Version", "Service Version", config.serviceVersion);

    const std::string kind = ini.getString("Version", "UseVersion", "internal");
    if (kind == "opentest" || kind == "open") {
        config.versionKind = VersionKind::kOpenTest;
    } else if (kind == "service") {
        config.versionKind = VersionKind::kService;
    } else if (kind == "internal" || kind.empty()) {
        config.versionKind = VersionKind::kInternal;
    } else {
        if (error) *error = source + ": unknown UseVersion '" + kind + "'";
        return false;
    }

    config.name = ini.getString("Server", "Name", config.name);
    config.dataDirectory = ini.getString("Server", "DataDirectory", config.dataDirectory);
    config.logFile = ini.getString("Server", "LogFile", config.logFile);
    config.seedTestAccounts = ini.getBool("Server", "SeedTestAccounts", config.seedTestAccounts);
    config.migrateOnStart = ini.getBool("Server", "MigrateOnStart", config.migrateOnStart);
    config.shutdownAfterSeconds = static_cast<int>(ini.getInt("Server", "ShutdownAfterSeconds", 0));

    const std::string level = ini.getString("Server", "LogLevel", "info");
    if (level == "error") {
        config.logLevel = LogLevel::kError;
    } else if (level == "warn" || level == "warning") {
        config.logLevel = LogLevel::kWarn;
    } else if (level == "debug") {
        config.logLevel = LogLevel::kDebug;
    } else if (level == "info") {
        config.logLevel = LogLevel::kInfo;
    } else if (level == "off") {
        config.logLevel = LogLevel::kOff;
    } else {
        if (error) *error = source + ": unknown LogLevel '" + level + "'";
        return false;
    }

    config.database.path = ini.getString("Database", "Path", config.database.path);
    config.database.wal = ini.getBool("Database", "WAL", config.database.wal);
    config.database.busyTimeoutMs = static_cast<int>(ini.getInt("Database", "BusyTimeoutMs", 5000));
    config.database.cacheSizeKb = static_cast<int>(ini.getInt("Database", "CacheSizeKb", 65536));

    readListener(ini, "Login", &config.login, 9000);
    readListener(ini, "Channel", &config.channel, 9100);
    readListener(ini, "Game", &config.game, 9200);
    readListener(ini, "Fep", &config.fep, 9300);
    readListener(ini, "Center", &config.center, 9400);
    readListener(ini, "Global", &config.global, 9500);

    if (ini.hasSection("Channels")) {
        config.channels.clear();
        const auto entries = ini.entries("Channels");
        for (std::size_t i = 0; i < entries.size(); ++i) {
            ChannelConfig channel;
            if (!parseChannelLine(entries[i].second, static_cast<int>(i), &channel)) {
                if (error) *error = source + ": bad [Channels] line '" + entries[i].first + "'";
                return false;
            }
            config.channels.push_back(channel);
        }
    }

    if (ini.hasSection("ServerSet")) {
        config.serverSets.clear();
        for (const auto& entry : ini.entries("ServerSet")) {
            ServerSetConfig set;
            if (!parseServerSetLine(entry.second, &set)) {
                if (error) *error = source + ": bad [ServerSet] line '" + entry.first + "'";
                return false;
            }
            config.serverSets.push_back(set);
        }
    }

    // A [Channels] block without [ServerSet] (or the reverse) is normal in the original configs; make
    // the missing side consistent so the login handshake always has an address to hand out.
    if (config.serverSets.empty() && !config.channels.empty()) {
        for (const auto& channel : config.channels) {
            ServerSetConfig set;
            set.id = channel.serverSetId == 0 ? 1 : channel.serverSetId;
            set.name = channel.displayName.empty() ? u"Server" : channel.displayName;
            set.ip = channel.ip;
            set.port = channel.masterPort;
            set.maxUser = channel.maxUser;
            config.serverSets.push_back(set);
        }
    }
    if (config.channels.empty() && !config.serverSets.empty()) {
        for (const auto& set : config.serverSets) {
            ChannelConfig channel;
            channel.channelId = set.id;
            channel.serverSetId = set.id;
            channel.serverUid = set.serverUid;
            channel.name = "Ch.1";
            channel.displayName = u"Ch.1";
            channel.ip = set.ip;
            channel.masterPort = set.port;
            channel.ncUdpPort = static_cast<unsigned short>(set.port + 51);
            channel.maxUser = set.maxUser;
            config.channels.push_back(channel);
        }
    }
    return true;
}

bool Config::validate(std::string* error) const {
    auto fail = [error](const std::string& message) {
        if (error) *error = message;
        return false;
    };

    if (internalVersion.empty() && openTestVersion.empty() && serviceVersion.empty()) {
        return fail("[Version] has no version string; the client handshake has nothing to compare against");
    }
    if (version().empty()) return fail("the selected version kind has no version string");
    if (database.path.empty()) return fail("[Database] Path is empty");

    const struct {
        const char* name;
        const ListenerConfig* listener;
    } listeners[] = {{"Login", &login},     {"Channel", &channel}, {"Game", &game},
                     {"Fep", &fep},         {"Center", &center},   {"Global", &global}};
    for (const auto& entry : listeners) {
        if (!entry.listener->enabled) continue;
        if (entry.listener->port == 0) return fail(std::string("[") + entry.name + "] Port is 0");
        if (entry.listener->maxUser < 0) return fail(std::string("[") + entry.name + "] MaxUser is negative");
    }

    if (channels.empty()) return fail("no channels configured");
    for (const auto& channel : channels) {
        if (channel.masterPort == 0) return fail("channel " + std::to_string(channel.channelId) + " has no port");
        if (channel.maxUser <= 0) return fail("channel " + std::to_string(channel.channelId) + " has no MaxUser");
    }
    if (serverSets.empty()) return fail("no server sets configured");
    for (const auto& set : serverSets) {
        if (set.port == 0) return fail("server set " + std::to_string(set.id) + " has no port");
    }
    return true;
}

std::string Config::describe() const {
    std::ostringstream out;
    out << "server      : " << name << "\n";
    out << "version     : " << formatVersion(version()) << " (" <<
        (versionKind == VersionKind::kInternal ? "internal"
         : versionKind == VersionKind::kOpenTest ? "opentest" : "service") << ")\n";
    out << "database    : " << database.path << (database.wal ? " (WAL)" : "") << "\n";
    out << "data dir    : " << dataDirectory << "\n";
    out << "log         : " << (logFile.empty() ? "<stdout>" : logFile) << "\n";

    auto dump = [&out](const char* label, const ListenerConfig& listener) {
        out << label << " : " << (listener.enabled ? "on  " : "off ") << listener.address << ":"
            << listener.port << " max " << listener.maxUser << "\n";
    };
    dump("login   ", login);
    dump("channel ", channel);
    dump("game    ", game);
    dump("fep     ", fep);
    dump("center  ", center);
    dump("global  ", global);

    for (const auto& channel : channels) {
        out << "channel " << channel.channelId << ": " << toUtf8(channel.displayName) << " " << channel.ip
            << ":" << channel.masterPort << " udp " << channel.ncUdpPort << " max " << channel.maxUser
            << " bonus exp " << channel.bonusExpPercent << "% drop " << channel.bonusDropPercent << "%\n";
    }
    for (const auto& set : serverSets) {
        out << "set " << set.id << ": " << toUtf8(set.name) << " " << set.ip << ":" << set.port << " max "
            << set.maxUser << "\n";
    }
    return out.str();
}

}  // namespace knc::server
