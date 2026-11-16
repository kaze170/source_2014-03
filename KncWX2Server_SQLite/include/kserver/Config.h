// kserver/Config.h - the single process configuration.
//
// The 2014 deployment ran five executables (LoginServer, ChannelServer, GameServer, CenterServer,
// GlobalServer), each with its own <name>.ini containing only a version block and a [String]
// message table; everything else - ports, addresses, user limits, ODBC DSNs - was compiled in or
// read from the CenterServer database. The single process build keeps that shape: one file,
// config/knc.ini, with a section per former server plus a [Server] block for the shared settings.
//
// The version keys are the ones KBaseServer::LoadINIFile read with GetPrivateProfileStringW:
//     [Version]
//     Internal Version = <tag>
//     OpenTest Version = <tag>
//     Service Version  = <tag>
// A client sends its KEGS_CONNECT_REQ m_wstrVersion and KGSUser::OnConnectReq compared it against
// GetMainVersion(); a mismatch answered NetError::ERR_CONNECT_00 and dropped the connection. This
// config keeps that contract: version() is what the server compares against.
#pragma once

#include <string>
#include <vector>

#include "kcore/IniFile.h"
#include "kcore/Log.h"
#include "kcore/Types.h"

namespace knc::server {

// Which of the three version strings this process announces (KBaseServer::m_eUseVersion).
enum class VersionKind { kInternal = 0, kOpenTest, kService };

struct ListenerConfig {
    bool enabled = true;
    std::string address = "0.0.0.0";
    unsigned short port = 0;
    int maxUser = 0;
    bool checkSequenceNum = false;
    unsigned int authFailLimit = 100;
};

struct DatabaseConfig {
    std::string path = "data/knc.db";
    bool wal = true;
    int busyTimeoutMs = 5000;
    int cacheSizeKb = 65536;
};

struct ChannelConfig {
    int channelId = 0;
    std::int64_t serverUid = 0;
    int serverSetId = 0;
    std::string name;             // narrow fallback ("Ch.1")
    WideString displayName;       // CP949 decoded from the ini, what KChannelInfo carries
    std::string ip = "127.0.0.1";
    unsigned short masterPort = 0;
    unsigned short ncUdpPort = 0;
    int maxUser = 0;
    int bonusExpPercent = 0;
    int bonusDropPercent = 0;
};

struct ServerSetConfig {
    int id = 0;
    WideString name;
    std::int64_t serverUid = 0;
    std::string ip = "127.0.0.1";
    unsigned short port = 0;
    int maxUser = 0;
};

struct Config {
    // [Version]
    WideString internalVersion;
    WideString openTestVersion;
    WideString serviceVersion;
    VersionKind versionKind = VersionKind::kInternal;

    // [Server]
    std::string name = "KncWX2Server";
    std::string dataDirectory = "data";
    std::string logFile;                  // empty: stdout only
    LogLevel logLevel = LogLevel::kInfo;
    bool seedTestAccounts = true;         // inserts test/test, gm/gm when Accounts is empty
    bool migrateOnStart = true;
    DatabaseConfig database;
    int shutdownAfterSeconds = 0;         // 0 = run until a signal; used by the smoke tests

    // [Login] / [Channel] / [Game] / [Fep] / [Center] / [Global]
    ListenerConfig login;
    ListenerConfig channel;
    ListenerConfig game;
    ListenerConfig fep;
    ListenerConfig center;
    ListenerConfig global;

    // [Channels] - "Channel0 = 1 | Ch.1 | 127.0.0.1 | 9100 | 9151 | 300 | 0 | 0"
    std::vector<ChannelConfig> channels;
    // [ServerSet] - "Set0 = 1 | Test Server | 127.0.0.1 | 9000 | 500"
    std::vector<ServerSetConfig> serverSets;

    // The version string the server compares KEGS_CONNECT_REQ against.
    const WideString& version() const;
    // Port of the channel a client should be sent to after selecting a server set.
    unsigned short channelPortForSet(int serverSetId) const;
    const ChannelConfig* findChannel(int channelId) const;

    // Helpers used by main() and by the tests.
    static Config defaults();
    bool load(const std::string& path, std::string* error = nullptr);
    bool loadFromText(const std::string& text, const std::string& sourceName, std::string* error = nullptr);
    // Applies an already parsed ini on top of the current values.
    bool applyIni(const IniFile& ini, const std::string& source, std::string* error = nullptr);
    // Sanity checks: ports in range, at least one channel, versions non empty, db path set.
    bool validate(std::string* error = nullptr) const;
    // Human readable dump for `kncserver --print-config`.
    std::string describe() const;
};

// Parses the "a | b | c" channel/server-set line format.
bool parseChannelLine(const std::string& value, int index, ChannelConfig* out);
bool parseServerSetLine(const std::string& value, ServerSetConfig* out);

}  // namespace knc::server
