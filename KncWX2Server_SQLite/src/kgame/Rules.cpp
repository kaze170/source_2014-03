#include "kgame/Rules.h"

#include <algorithm>
#include <cstdio>

namespace knc::game {
namespace {

bool isAsciiLetterOrDigit(char16_t c) {
    return (c >= u'0' && c <= u'9') || (c >= u'A' && c <= u'Z') || (c >= u'a' && c <= u'z');
}

}  // namespace

NicknameVerdict validateNickname(const WideString& nickName) {
    if (nickName.size() < NicknameRules::kMinLength) return NicknameVerdict::kTooShort;
    if (nickName.size() > NicknameRules::kMaxLength) return NicknameVerdict::kTooLong;

    bool allDigits = true;
    for (char16_t c : nickName) {
        const bool asciiDigit = c >= u'0' && c <= u'9';
        // Reject control characters, whitespace and the characters the client itself cannot type
        // into the name box. Hangul and the Latin/Greek/Cyrillic ranges are allowed, UHC for the
        // legacy Korean text included (it arrives already converted to UTF-16 by kcore::Text).
        if (c < 0x20 || c == 0x7F) return NicknameVerdict::kBadCharacter;
        if (c == u' ' || c == u'\t') return NicknameVerdict::kBadCharacter;
        if (!asciiDigit) allDigits = false;
    }
    if (allDigits) return NicknameVerdict::kAllDigits;
    return NicknameVerdict::kOk;
}

NicknameVerdict validateLoginId(const std::string& loginId) {
    if (loginId.size() < NicknameRules::kMinLoginIdLength) return NicknameVerdict::kTooShort;
    if (loginId.size() > NicknameRules::kMaxLoginIdLength) return NicknameVerdict::kTooLong;
    for (char c : loginId) {
        if (!isAsciiLetterOrDigit(static_cast<char16_t>(static_cast<unsigned char>(c)))) {
            return NicknameVerdict::kBadCharacter;
        }
    }
    return NicknameVerdict::kOk;
}

NicknameVerdict validatePassword(const std::string& password) {
    if (password.size() < NicknameRules::kMinPasswordLength) return NicknameVerdict::kTooShort;
    if (password.size() > NicknameRules::kMaxPasswordLength) return NicknameVerdict::kTooLong;
    for (char c : password) {
        if (static_cast<unsigned char>(c) < 0x21 || static_cast<unsigned char>(c) > 0x7E) {
            return NicknameVerdict::kBadCharacter;
        }
    }
    return NicknameVerdict::kOk;
}

int EnchantRules::successChance(int currentLevel) {
    if (currentLevel < 0) return 0;
    // The 2014 ladder: 100% up to +4, then a decline to 20% at +20.
    static const int kLadder[21] = {100, 100, 100, 100, 100, 90, 80, 70, 60, 50, 45,
                                    40, 35, 30, 28, 26, 24, 22, 21, 20, 20};
    if (currentLevel >= kMaxEnchantLevel) return 0;
    return kLadder[currentLevel];
}

bool EnchantRules::destroysOnFailure(int currentLevel) {
    return currentLevel >= kDestroyFromLevel && currentLevel < kMaxEnchantLevel;
}

EnchantOutcome rollEnchant(int currentLevel, std::mt19937& rng) {
    if (currentLevel < 0 || currentLevel >= EnchantRules::kMaxEnchantLevel) return EnchantOutcome::kInvalid;
    const int chance = EnchantRules::successChance(currentLevel);
    std::uniform_int_distribution<int> roll(1, 100);
    if (roll(rng) <= chance) return EnchantOutcome::kSuccess;
    if (EnchantRules::destroysOnFailure(currentLevel)) return EnchantOutcome::kDestroyed;
    return EnchantOutcome::kFailed;
}

int pickSocketItem(int groupId, const std::vector<SocketGroupEntry>& entries, std::mt19937& rng) {
    int total = 0;
    for (const auto& entry : entries) {
        if (entry.groupId == groupId && entry.weight > 0) total += entry.weight;
    }
    if (total <= 0) return 0;

    std::uniform_int_distribution<int> roll(1, total);
    int target = roll(rng);
    for (const auto& entry : entries) {
        if (entry.groupId != groupId || entry.weight <= 0) continue;
        target -= entry.weight;
        if (target <= 0) return entry.socketItemCode;
    }
    return 0;
}

const DropEntry* rollDrop(const std::vector<DropEntry>& table, std::mt19937& rng, int bonusPercent) {
    if (table.empty()) return nullptr;

    std::int64_t total = 0;
    for (const auto& entry : table) total += std::max(0, entry.weight);
    if (total <= 0) return nullptr;

    // The channel/field drop bonus widens the table rather than re-rolling: it multiplies the total
    // weight the roll is taken against, so a 50% bonus makes the same entry 1.5x as likely and
    // everything else proportionally less so.
    const int bonus = std::clamp(bonusPercent, 0, 100);
    std::uniform_int_distribution<std::int64_t> roll(1, total * (100 + bonus) / 100);
    std::int64_t target = roll(rng);
    for (const auto& entry : table) {
        if (entry.weight <= 0) continue;
        target -= entry.weight;
        if (target <= 0) return &entry;
    }
    return &table.back();
}

std::int64_t ExpRules::applyBonus(std::int64_t baseExp, bool premium, bool vip, double extraRate) {
    if (baseExp <= 0) return 0;
    double rate = 1.0 + extraRate;
    if (premium) rate += kPremiumExpRate;
    if (vip) rate += kVipExpRate;
    if (rate < 0.0) rate = 0.0;
    return static_cast<std::int64_t>(static_cast<double>(baseExp) * rate);
}

std::string makeGuestLoginId(std::uint32_t serial) {
    char buffer[32];
    std::snprintf(buffer, sizeof(buffer), "Guest%08u", serial);
    return std::string(buffer);
}

}  // namespace knc::game
