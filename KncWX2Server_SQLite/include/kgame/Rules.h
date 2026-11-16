// kgame/Rules.h - the small, self contained game rule functions the 2014 server spread across
// KItemManager, KEnchantItemManager, KExpTable and KChannelManager.
//
// Everything here is deterministic and side effect free so it can be unit tested without a socket
// or a database. Random decisions take an explicit seed/engine.
#pragma once

#include <cstdint>
#include <random>
#include <string>
#include <vector>

#include "kcore/Types.h"

namespace knc::game {

// ---------------------------------------------------------------- nickname / validation rules
// The 2014 server limited nicknames to 2..12 characters of the client's UHC range and rejected
// strings containing digits-only or filter-list hits. The filter list itself was resource data; the
// length/character class checks are reproduced here.
struct NicknameRules {
    static constexpr std::size_t kMinLength = 2;
    static constexpr std::size_t kMaxLength = 12;
    static constexpr std::size_t kMinLoginIdLength = 4;
    static constexpr std::size_t kMaxLoginIdLength = 16;
    static constexpr std::size_t kMinPasswordLength = 4;
    static constexpr std::size_t kMaxPasswordLength = 16;
};

enum class NicknameVerdict { kOk = 0, kTooShort, kTooLong, kBadCharacter, kAllDigits };

// Validates a nickname already converted to UTF-16 (the wire form).
NicknameVerdict validateNickname(const WideString& nickName);
NicknameVerdict validateLoginId(const std::string& loginId);
NicknameVerdict validatePassword(const std::string& password);

// ---------------------------------------------------------------- enchant rules
// EnchantTable.lua capped enhancement at +20 for the "new" enchant stones (SetSupportMaterialLimit
// 10 materials, +11 during events). The success/failure ladder below is the 2014 curve: success
// rate falls with the current level, destruction only happens above +6.
struct EnchantRules {
    static constexpr int kMaxEnchantLevel = 20;
    static constexpr int kDestroyFromLevel = 7;

    // Percent chance to go from `currentLevel` to `currentLevel + 1`.
    static int successChance(int currentLevel);
    // True when a failed attempt destroys the item instead of only dropping a level.
    static bool destroysOnFailure(int currentLevel);
};

enum class EnchantOutcome { kSuccess, kFailed, kDestroyed, kInvalid };
EnchantOutcome rollEnchant(int currentLevel, std::mt19937& rng);

// ---------------------------------------------------------------- socket rules
// ItemRandomSocket.lua mapped an item code to a socket group; a socket group entry is
// (group, socket item code, weight). Weighted pick reproduces g_pCX2SocketItem.
struct SocketGroupEntry {
    int groupId = 0;
    int socketItemCode = 0;
    int weight = 1;
};

int pickSocketItem(int groupId, const std::vector<SocketGroupEntry>& entries, std::mt19937& rng);

// ---------------------------------------------------------------- drop rules
struct DropEntry {
    int itemCode = 0;
    int weight = 1;
    int minQuantity = 1;
    int maxQuantity = 1;
};

// One weighted roll over a drop table (DropTable.lua). Returns the chosen entry or nullptr for an
// empty table. `bonusPercent` is the field/channel drop bonus from KChannelBonusInfo.
const DropEntry* rollDrop(const std::vector<DropEntry>& table, std::mt19937& rng, int bonusPercent = 0);

// ---------------------------------------------------------------- exp / level rules
// GameSysValTable.lua: GameSysVal:AddLimitsMaxLevel( 65 ) and AddPremiumEXP( 0.3 ).
struct ExpRules {
    static constexpr int kMaxLevel = 65;
    static constexpr double kPremiumExpRate = 0.30;   // premium medal
    static constexpr double kVipExpRate = 0.05;       // VIP ticket
    static constexpr int kGuestUserLimitLevel = 19;   // SetGuestUserLimitLevel( 19 )

    // Applies the multiplicative bonuses above to a base exp award, rounding down like the server.
    static std::int64_t applyBonus(std::int64_t baseExp, bool premium, bool vip, double extraRate = 0.0);
};

// ---------------------------------------------------------------- channel level rules
// The 2014 channel list had a level window per channel type; a character below the minimum could
// not enter a PvP channel (ERR_ROOM_LIST_03, "3 레벨부터 대전").
struct ChannelRules {
    static constexpr int kPvpMinLevel = 3;
    static constexpr int kPvpMaxLevel = ExpRules::kMaxLevel;
    static bool canEnterPvp(int level) { return level >= kPvpMinLevel && level <= kPvpMaxLevel; }
};

// ---------------------------------------------------------------- mail / trade limits
struct MailRules {
    static constexpr std::size_t kMaxSubjectLength = 32;
    static constexpr std::size_t kMaxBodyLength = 512;
    static constexpr std::size_t kMaxEdPerMail = 1000000000;  // 1e9 ED, the 2014 cap
    static constexpr std::size_t kMaxItemsPerMail = 5;
};

// ---------------------------------------------------------------- name formatting helpers
// "Name" + a deterministic suffix, used for guest accounts (the original generated "Guest####").
std::string makeGuestLoginId(std::uint32_t serial);

}  // namespace knc::game
