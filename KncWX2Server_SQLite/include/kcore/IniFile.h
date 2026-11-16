// kcore/IniFile.h - reader for the .ini files the 2014 servers were configured with.
//
// KBaseServer::LoadINIFile called GetPrivateProfileStringW against <cwd>\<name>.ini, so every
// server had exactly one flat file with [Section] headers, "key = value" lines and ";"/"#" comments.
// This reader reproduces the behaviour that matters: case insensitive section/key lookup
// (GetPrivateProfileString is case insensitive), an int/bool/string accessor set, and CP949
// decoding of the Korean values stored in the original files.
#pragma once

#include <map>
#include <string>
#include <vector>

#include "kcore/Types.h"

namespace knc {

class IniFile {
public:
    // Parses `text`. `sourceName` is only used for diagnostics. Values are kept as raw bytes; use the
    // wide accessors to decode them as CP949 (the encoding of the 2014 .ini files).
    bool parse(const std::string& text, const std::string& sourceName, std::string* error = nullptr);
    bool load(const std::string& path, std::string* error = nullptr);

    bool hasSection(const std::string& section) const;
    bool hasKey(const std::string& section, const std::string& key) const;

    // Byte value, as stored in the file.
    std::string getString(const std::string& section, const std::string& key,
                          const std::string& def = {}) const;
    // Value decoded from CP949 into UTF-16, matching the original's wchar_t config strings.
    WideString getWideString(const std::string& section, const std::string& key,
                             const WideString& def = WideString()) const;

    long long getInt(const std::string& section, const std::string& key, long long def = 0) const;
    bool getBool(const std::string& section, const std::string& key, bool def = false) const;
    double getDouble(const std::string& section, const std::string& key, double def = 0.0) const;

    // All "key = value" pairs of a section, in file order.
    std::vector<std::pair<std::string, std::string>> entries(const std::string& section) const;

    const std::vector<std::string>& sections() const { return sectionOrder_; }
    const std::string& sourceName() const { return sourceName_; }

private:
    static std::string lower(std::string value);

    std::string sourceName_;
    std::map<std::string, std::map<std::string, std::string>> sections_;  // lower cased keys
    std::map<std::string, std::vector<std::string>> keyOrder_;
    std::vector<std::string> sectionOrder_;
};

}  // namespace knc
