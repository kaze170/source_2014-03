#include "kcore/IniFile.h"

#include <algorithm>
#include <cctype>
#include <cstdlib>
#include <fstream>
#include <sstream>

namespace knc {
namespace {

std::string trim(const std::string& value) {
    std::size_t begin = 0;
    std::size_t end = value.size();
    while (begin < end && std::isspace(static_cast<unsigned char>(value[begin]))) ++begin;
    while (end > begin && std::isspace(static_cast<unsigned char>(value[end - 1]))) --end;
    return value.substr(begin, end - begin);
}

// Strips a UTF-8 BOM so a file saved by a modern editor parses the same as an ANSI one.
void stripBom(std::string& text) {
    if (text.size() >= 3 && static_cast<unsigned char>(text[0]) == 0xEF &&
        static_cast<unsigned char>(text[1]) == 0xBB && static_cast<unsigned char>(text[2]) == 0xBF) {
        text.erase(0, 3);
    }
}

}  // namespace

std::string IniFile::lower(std::string value) {
    std::transform(value.begin(), value.end(), value.begin(), [](unsigned char c) {
        return static_cast<char>(std::tolower(c));
    });
    return value;
}

bool IniFile::parse(const std::string& text, const std::string& sourceName, std::string* error) {
    sourceName_ = sourceName;
    sections_.clear();
    keyOrder_.clear();
    sectionOrder_.clear();

    std::string content = text;
    stripBom(content);

    std::istringstream stream(content);
    std::string line;
    std::string current;
    int lineNumber = 0;
    bool sawSection = false;

    while (std::getline(stream, line)) {
        ++lineNumber;
        if (!line.empty() && line.back() == '\r') line.pop_back();
        const std::string trimmed = trim(line);
        if (trimmed.empty()) continue;
        if (trimmed[0] == ';' || trimmed[0] == '#') continue;

        if (trimmed.front() == '[') {
            const std::size_t close = trimmed.find(']');
            if (close == std::string::npos) {
                if (error) *error = sourceName + ":" + std::to_string(lineNumber) + ": unterminated section";
                return false;
            }
            current = lower(trim(trimmed.substr(1, close - 1)));
            if (sections_.find(current) == sections_.end()) {
                sections_[current] = {};
                keyOrder_[current] = {};
                sectionOrder_.push_back(trim(trimmed.substr(1, close - 1)));
            }
            sawSection = true;
            continue;
        }

        const std::size_t equals = trimmed.find('=');
        if (equals == std::string::npos) {
            if (error) *error = sourceName + ":" + std::to_string(lineNumber) + ": expected key = value";
            return false;
        }
        const std::string key = lower(trim(trimmed.substr(0, equals)));
        const std::string value = trim(trimmed.substr(equals + 1));
        if (key.empty()) {
            if (error) *error = sourceName + ":" + std::to_string(lineNumber) + ": empty key";
            return false;
        }
        // The original files carried stray keys before the first section header; like
        // GetPrivateProfileString they belong to no section and are ignored.
        if (!sawSection) continue;

        auto& section = sections_[current];
        if (section.find(key) == section.end()) keyOrder_[current].push_back(key);
        section[key] = value;
    }
    return true;
}

bool IniFile::load(const std::string& path, std::string* error) {
    std::ifstream file(path, std::ios::binary);
    if (!file) {
        if (error) *error = "cannot open " + path;
        return false;
    }
    std::ostringstream buffer;
    buffer << file.rdbuf();
    return parse(buffer.str(), path, error);
}

bool IniFile::hasSection(const std::string& section) const {
    return sections_.find(lower(section)) != sections_.end();
}

bool IniFile::hasKey(const std::string& section, const std::string& key) const {
    const auto it = sections_.find(lower(section));
    if (it == sections_.end()) return false;
    return it->second.find(lower(key)) != it->second.end();
}

std::string IniFile::getString(const std::string& section, const std::string& key,
                               const std::string& def) const {
    const auto it = sections_.find(lower(section));
    if (it == sections_.end()) return def;
    const auto entry = it->second.find(lower(key));
    return entry == it->second.end() ? def : entry->second;
}

WideString IniFile::getWideString(const std::string& section, const std::string& key,
                                  const WideString& def) const {
    if (!hasKey(section, key)) return def;
    return cp949ToWide(getString(section, key));
}

long long IniFile::getInt(const std::string& section, const std::string& key, long long def) const {
    const std::string value = getString(section, key);
    if (value.empty()) return def;
    char* end = nullptr;
    const long long parsed = std::strtoll(value.c_str(), &end, 0);  // base 0 honours 0x.. like atoi did
    if (end == value.c_str()) return def;
    return parsed;
}

bool IniFile::getBool(const std::string& section, const std::string& key, bool def) const {
    const std::string value = lower(getString(section, key));
    if (value.empty()) return def;
    if (value == "1" || value == "true" || value == "yes" || value == "on") return true;
    if (value == "0" || value == "false" || value == "no" || value == "off") return false;
    return def;
}

double IniFile::getDouble(const std::string& section, const std::string& key, double def) const {
    const std::string value = getString(section, key);
    if (value.empty()) return def;
    char* end = nullptr;
    const double parsed = std::strtod(value.c_str(), &end);
    if (end == value.c_str()) return def;
    return parsed;
}

std::vector<std::pair<std::string, std::string>> IniFile::entries(const std::string& section) const {
    std::vector<std::pair<std::string, std::string>> out;
    const std::string name = lower(section);
    const auto it = sections_.find(name);
    if (it == sections_.end()) return out;
    const auto order = keyOrder_.find(name);
    if (order == keyOrder_.end()) return out;
    for (const auto& key : order->second) {
        const auto entry = it->second.find(key);
        if (entry != it->second.end()) out.emplace_back(entry->first, entry->second);
    }
    return out;
}

}  // namespace knc
