#include "kcore/Types.h"
#include "kcore/Text.h"

#include <array>
#include <mutex>
#include <unordered_map>

namespace knc {
namespace {

constexpr char32_t kReplacement = 0xFFFD;

void appendUtf8(std::string& out, char32_t cp) {
    if (cp < 0x80) {
        out.push_back(static_cast<char>(cp));
    } else if (cp < 0x800) {
        out.push_back(static_cast<char>(0xC0 | (cp >> 6)));
        out.push_back(static_cast<char>(0x80 | (cp & 0x3F)));
    } else if (cp < 0x10000) {
        out.push_back(static_cast<char>(0xE0 | (cp >> 12)));
        out.push_back(static_cast<char>(0x80 | ((cp >> 6) & 0x3F)));
        out.push_back(static_cast<char>(0x80 | (cp & 0x3F)));
    } else {
        out.push_back(static_cast<char>(0xF0 | (cp >> 18)));
        out.push_back(static_cast<char>(0x80 | ((cp >> 12) & 0x3F)));
        out.push_back(static_cast<char>(0x80 | ((cp >> 6) & 0x3F)));
        out.push_back(static_cast<char>(0x80 | (cp & 0x3F)));
    }
}

// Decodes one UTF-8 code point, advancing i. Invalid input yields U+FFFD and advances one byte.
char32_t decodeUtf8(std::string_view s, std::size_t& i) {
    const byte c0 = static_cast<byte>(s[i]);
    if (c0 < 0x80) {
        ++i;
        return c0;
    }
    std::size_t extra = 0;
    char32_t cp = 0;
    if ((c0 & 0xE0) == 0xC0) {
        extra = 1;
        cp = c0 & 0x1F;
    } else if ((c0 & 0xF0) == 0xE0) {
        extra = 2;
        cp = c0 & 0x0F;
    } else if ((c0 & 0xF8) == 0xF0) {
        extra = 3;
        cp = c0 & 0x07;
    } else {
        ++i;
        return kReplacement;
    }
    if (i + extra >= s.size()) {
        i = s.size();
        return kReplacement;
    }
    for (std::size_t k = 1; k <= extra; ++k) {
        const byte cc = static_cast<byte>(s[i + k]);
        if ((cc & 0xC0) != 0x80) {
            ++i;
            return kReplacement;
        }
        cp = (cp << 6) | (cc & 0x3F);
    }
    i += extra + 1;
    return cp;
}

}  // namespace

int uhcTrailIndex(byte trail) {
    if (trail >= 0x41 && trail <= 0x5A) return trail - 0x41;              // 0..25
    if (trail >= 0x61 && trail <= 0x7A) return 26 + (trail - 0x61);       // 26..51
    if (trail >= 0x81 && trail <= 0xFE) return 52 + (trail - 0x81);       // 52..177
    return -1;
}

WideString toWide(std::string_view utf8) {
    WideString out;
    out.reserve(utf8.size());
    std::size_t i = 0;
    while (i < utf8.size()) {
        char32_t cp = decodeUtf8(utf8, i);
        if (cp >= 0x10000) {
            cp -= 0x10000;
            out.push_back(static_cast<char16_t>(0xD800 + (cp >> 10)));
            out.push_back(static_cast<char16_t>(0xDC00 + (cp & 0x3FF)));
        } else {
            out.push_back(static_cast<char16_t>(cp));
        }
    }
    return out;
}

std::string toUtf8(std::u16string_view utf16) {
    std::string out;
    out.reserve(utf16.size());
    for (std::size_t i = 0; i < utf16.size(); ++i) {
        char32_t cp = utf16[i];
        if (cp >= 0xD800 && cp <= 0xDBFF && i + 1 < utf16.size()) {
            const char32_t lo = utf16[i + 1];
            if (lo >= 0xDC00 && lo <= 0xDFFF) {
                cp = 0x10000 + ((cp - 0xD800) << 10) + (lo - 0xDC00);
                ++i;
            }
        }
        appendUtf8(out, cp);
    }
    return out;
}

std::u16string cp949ToWide(std::string_view cp949) {
    std::u16string out;
    out.reserve(cp949.size());
    std::size_t i = 0;
    while (i < cp949.size()) {
        const byte b = static_cast<byte>(cp949[i]);
        if (b < 0x80) {
            out.push_back(static_cast<char16_t>(b));
            ++i;
            continue;
        }
        if (i + 1 >= cp949.size()) {
            out.push_back(static_cast<char16_t>(kReplacement));
            break;
        }
        const byte trail = static_cast<byte>(cp949[i + 1]);
        const int ti = uhcTrailIndex(trail);
        if (ti < 0 || b < kUhcLeadFirst || b > kUhcLeadLast) {
            out.push_back(static_cast<char16_t>(kReplacement));
            ++i;
            continue;
        }
        const std::size_t idx = static_cast<std::size_t>(b - kUhcLeadFirst) * kUhcTrailTableSize + ti;
        const std::uint16_t uni = kUhcToUnicode[idx];
        out.push_back(uni != 0 ? static_cast<char16_t>(uni) : static_cast<char16_t>(kReplacement));
        i += 2;
    }
    return out;
}

std::string wideToCp949(std::u16string_view utf16) {
    std::string out;
    out.reserve(utf16.size());
    for (char16_t unit : utf16) {
        if (unit < 0x80) {
            out.push_back(static_cast<char>(unit));
            continue;
        }
        byte lead = 0;
        byte trail = 0;
        if (uhcFromUnicode(unit, lead, trail)) {
            out.push_back(static_cast<char>(lead));
            out.push_back(static_cast<char>(trail));
        } else {
            out.push_back('?');
        }
    }
    return out;
}

byte uhcFromUnicode(char32_t ch, byte& leadOut, byte& trailOut) {
    static std::once_flag once;
    static std::unordered_map<char32_t, std::uint16_t> reverse;
    std::call_once(once, [] {
        reverse.reserve(kUhcEntryCount);
        for (std::size_t leadIdx = 0; leadIdx < kUhcLeadCount; ++leadIdx) {
            for (std::size_t trailIdx = 0; trailIdx < kUhcTrailTableSize; ++trailIdx) {
                const std::uint16_t uni = kUhcToUnicode[leadIdx * kUhcTrailTableSize + trailIdx];
                if (uni == 0) continue;
                reverse.emplace(static_cast<char32_t>(uni),
                                static_cast<std::uint16_t>((kUhcLeadFirst + leadIdx) << 8 | trailIdx));
            }
        }
    });
    const auto it = reverse.find(ch);
    if (it == reverse.end()) return 0;
    const std::size_t leadIdx = (it->second >> 8) - kUhcLeadFirst;
    const std::size_t trailIdx = it->second & 0xFF;
    leadOut = static_cast<byte>(kUhcLeadFirst + leadIdx);
    // replicate the encoding order used by uhcTrailIndex()
    if (trailIdx < 26) {
        trailOut = static_cast<byte>(0x41 + trailIdx);
    } else if (trailIdx < 52) {
        trailOut = static_cast<byte>(0x61 + (trailIdx - 26));
    } else {
        trailOut = static_cast<byte>(0x81 + (trailIdx - 52));
    }
    return 1;
}

}  // namespace knc
