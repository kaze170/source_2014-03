// kproto/LoginPackets.h - the login/channel handshake packets, field for field.
//
// Every struct here mirrors a DECL_DATA / DECL_PACKET in KncWX2Server/Common/{CommonPacket.h,
// ClientPacket.h} and its SERIALIZE_DEFINE_PUT order. The feature-flag #ifdef blocks are resolved
// with the single fixed flag set in include/kproto/FeatureFlags.gen.h (691 enabled, 185 disabled and
// 61 tested-but-never-defined, region NONE), which is the same set the rest of the rewrite is built
// with.
#pragma once

#include <cstdint>
#include <map>
#include <string>
#include <vector>

#include "kcore/Serialize.h"
#include "kcore/Types.h"
#include "kproto/FeatureFlags.gen.h"
#include "kproto/Packet.h"

namespace knc {

// The same guard as in UnitPackets.h: this file spells out the flag-resolved layout, so the few
// flags that decide which *login* fields exist are asserted rather than trusted.
static_assert(features::kSERV_OTP_AUTH, "KUserAuthenticateReq carries m_wstrHWID");
static_assert(features::kSERV_MACHINE_ID_DUPLICATE_CHECK, "KUserAuthenticateReq carries m_strMachineID");
static_assert(!features::kSERV_DLL_LIST_CHECK_BEFOR_LOADING,
              "KUserAuthenticateReq does not carry m_kChannelRandomKey");
static_assert(!features::kSERV_SERIAL_NUMBER_AVAILABILITY_CHECK_IN_GAMESERVER,
              "KUserAuthenticateReq does not carry m_charServerSN");
static_assert(features::kSERV_KOG_OTP_VERIFY, "KUserAuthenticateReq carries m_bManualLogin");
static_assert(!features::kSERV_EVENT_MONEY, "EGS_VERIFY_ACCOUNT_ACK does not carry m_iEventMoney");
static_assert(!features::kSERV_HACKING_USER_CHECK_COUNT,
              "KAccountBlockInfo carries only the end time and the reason");
static_assert(!features::kSERV_INTEGRATE_THE_CHANNEL_INFO, "KChannelInfo carries no m_kBonusInfo");
static_assert(features::kSERV_CHAR_LOG, "EGS_DELETE_UNIT_ACK carries the nick name, level and class");
static_assert(features::kSERV_UNIT_DELETE_EXCEPTION_MODIFY, "EGS_DELETE_UNIT_REQ carries m_iUserUID");
static_assert(features::kSERV_CLIENT_PUBLIC_IP, "EGS_VERIFY_ACCOUNT_ACK carries m_wstrClientIP");
static_assert(features::kSERV_SHARING_BANK_QUEST_CASH, "the unit list and inventory acks carry sharingBank");
static_assert(!features::kSERV_STEAM, "KUserAuthenticateReq carries no m_bSteamClient");
static_assert(!features::kSERV_CHANNELING_AERIA, "KUserAuthenticateReq carries no m_bAeriaClient");

// --------------------------------------------------------------------------- KAccountOption
// DECL_DATA( KAccountOption ) { bool m_bPlayGuide; }
struct AccountOption {
    bool playGuide = false;
    void serialize(Serializer& s) { s.io(playGuide); }
};

// --------------------------------------------------------------------------- KAccountInfo
// DECL_DATA( KAccountInfo ): UID, id, name, auth level, internal flag, options, block info,
// recommend flag, guest flag, OTP, and (with the flags this build uses) reg date, last login,
// channel random key.
struct AccountInfo {
    UidType userUid = 0;
    WideString id;
    WideString name;
    int authLevel = 0;
    bool internalUser = false;
    AccountOption option;
    AccountBlockInfo blockInfo;
    bool isRecommend = false;
    bool isGuestUser = false;
    WideString otp;
    WideString regDate;       // SERV_CASH_ITEM_LIST
    WideString lastLogin;     // SERV_SECOND_SECURITY
    // SERV_DLL_LIST_CHECK_BEFOR_LOADING is #undef'd in the OnlyGlobal defines, so
    // m_iChannelRandomKey is NOT on the wire.

    void serialize(Serializer& s) {
        s.io(userUid);
        s.io(id);
        s.io(name);
        s.io(authLevel);
        s.io(internalUser);
        s.io(option);
        s.io(blockInfo);
        s.io(isRecommend);
        s.io(isGuestUser);
        s.io(otp);
        s.io(regDate);
        s.io(lastLogin);
    }
};

// --------------------------------------------------------------------------- KUserAuthenticateReq
// DECL_DATA( KUserAuthenticateReq ), also typedef'd as KEGS_VERIFY_ACCOUNT_REQ.
struct UserAuthenticateReq {
    bool debugAuth = false;
    WideString userId;
    WideString password;
    WideString hwid;                 // SERV_OTP_AUTH
    std::string machineId;           // SERV_MACHINE_ID_DUPLICATE_CHECK
    int channelingCode = 0;
    bool manualLogin = false;        // SERV_KOG_OTP_VERIFY
    // Absent from this build's flag set, so absent from the wire too:
    //   m_kChannelRandomKey (SERV_DLL_LIST_CHECK_BEFOR_LOADING disabled)
    //   m_charServerSN[12]  (SERV_SERIAL_NUMBER_AVAILABILITY_CHECK_IN_GAMESERVER disabled)
    //   m_wstrClientTime    (SERV_CHECK_MACHINE_LOCALTIME commented out)
    //   m_wstrSocketID      (SERV_COUNTRY_TH disabled, region block inert)
    //   m_bSteamClient / m_bAeriaClient (not defined by ServerDefine.h at all)
    static constexpr std::size_t kFixedSize = 1 + 4 + 1;  // debugAuth + channelingCode + manualLogin

    void serialize(Serializer& s) {
        s.io(debugAuth);
        s.io(userId);
        s.io(password);
        s.io(hwid);
        s.io(machineId);
        s.io(channelingCode);
        s.io(manualLogin);
    }
};

// --------------------------------------------------------------------------- KEGS_CONNECT_REQ / ACK
struct ConnectReq {
    WideString version;
    void serialize(Serializer& s) { s.io(version); }
};

struct ConnectAck {
    int ok = 0;
    int udpPort = 0;
    int channelId = 0;
    // UDP_CAN_NOT_SEND_USER_KICK is not defined, so m_bUDPKickOff is not on the wire.
    void serialize(Serializer& s) {
        s.io(ok);
        s.io(udpPort);
        s.io(channelId);
    }
};

// --------------------------------------------------------------------------- KEGS_VERIFY_ACCOUNT_ACK
struct VerifyAccountAck {
    int ok = 0;
    AccountInfo accountInfo;
    char hackingUserType = 0;
    WideString adUrl;
    bool sex = false;
    UINT32 age = 0;
    UINT32 nexonSn = 0;
    WideString clientIp;      // SERV_CLIENT_PUBLIC_IP
    std::uint8_t channelCode = 0;
    WideString channelUserId;
    // SERV_EVENT_MONEY is disabled, so m_iEventMoney is absent from the wire.

    void serialize(Serializer& s) {
        s.io(ok);
        s.io(accountInfo);
        s.io(hackingUserType);
        s.io(adUrl);
        s.io(sex);
        s.io(age);
        s.io(nexonSn);
        s.io(clientIp);
        s.io(channelCode);
        s.io(channelUserId);
    }
};

// --------------------------------------------------------------------------- KEGS_NEW_USER_JOIN_REQ / ACK
struct NewUserJoinReq {
    WideString id;
    WideString password;
    WideString name;
    byte guestUser = 0;
    void serialize(Serializer& s) {
        s.io(id);
        s.io(password);
        s.io(name);
        s.io(guestUser);
    }
};

struct NewUserJoinAck {
    int ok = 0;
    UidType userUid = 0;
    void serialize(Serializer& s) {
        s.io(ok);
        s.io(userUid);
    }
};

// --------------------------------------------------------------------------- server set list
// DECL_DATA( KServerSetData ) { int serverSetID; std::wstring serverSetName; char userCountLevel; }
struct ServerSetData {
    enum UserCountLevel : char {
        kUclInvalid = 0,
        kUclFree = 1,    // < 30 %
        kUclNormal = 2,  // < 70 %
        kUclBusy = 3,    // < 100 %
        kUclFull = 4,
    };

    int serverSetId = 0;
    WideString serverSetName;
    char userCountLevel = kUclFree;

    void serialize(Serializer& s) {
        s.io(serverSetId);
        s.io(serverSetName);
        s.io(userCountLevel);
    }
};

struct GetServerSetDataAck {
    int ok = 0;
    std::vector<ServerSetData> serverSetList;
    void serialize(Serializer& s) {
        s.io(ok);
        s.io(serverSetList);
    }
};

struct SelectServerSetReq {
    int serverSetId = 0;
    void serialize(Serializer& s) { s.io(serverSetId); }
};

struct SelectServerSetAck {
    int ok = 0;
    WideString ip;
    unsigned short port = 0;
    AccountBlockInfo accountBlockInfo;
    void serialize(Serializer& s) {
        s.io(ok);
        s.io(ip);
        s.io(port);
        s.io(accountBlockInfo);
    }
};

// --------------------------------------------------------------------------- channel list
// DECL_DATA( KChannelBonusInfo ): the level range that gets the bonus, not just the percentages.
struct ChannelBonusInfo {
    bool enable = false;
    int beginLevel = 0;
    int endLevel = 0;
    int perExp = 0;
    int perEd = 0;

    void serialize(Serializer& s) {
        s.io(enable);
        s.io(beginLevel);
        s.io(endLevel);
        s.io(perExp);
        s.io(perEd);
    }
};

// DECL_DATA( KChannelInfo )
struct ChannelInfo {
    int serverUid = 0;
    int channelId = 0;
    WideString channelName;
    int serverGroupId = 0;
    WideString ip;
    unsigned short masterPort = 0;
    unsigned short ncUdpPort = 0;
    int maxUser = 0;
    int currentUser = 0;
    int curPartyCount = 0;
    int playGamePartyCount = 0;
    // SERV_INTEGRATE_THE_CHANNEL_INFO is not defined, so m_kBonusInfo is not on the wire.
    // (The bonus table still travels in EGS_GET_CHANNEL_LIST_ACK's map, keyed by channel id.)

    void serialize(Serializer& s) {
        s.io(serverUid);
        s.io(channelId);
        s.io(channelName);
        s.io(serverGroupId);
        s.io(ip);
        s.io(masterPort);
        s.io(ncUdpPort);
        s.io(maxUser);
        s.io(currentUser);
        s.io(curPartyCount);
        s.io(playGamePartyCount);
    }
};

struct ChannelSimpleInfo {
    int channelId = 0;
    int maxUser = 0;
    int currentUser = 0;
    void serialize(Serializer& s) {
        s.io(channelId);
        s.io(maxUser);
        s.io(currentUser);
    }
};

// EGS_GET_CHANNEL_LIST_ACK with SERV_CHANNEL_LIST_RENEWAL.
struct GetChannelListAck {
    int serverGroupId = 0;
    std::vector<ChannelInfo> channelInfo;
    // KSerializer writes std::map as [u32 count][key, value ...] - the same layout as a vector of
    // pairs - so the map shaped bonus list stays wire compatible.
    std::map<int, ChannelBonusInfo> channelBonusInfo;
    void serialize(Serializer& s) {
        s.io(serverGroupId);
        s.io(channelInfo);
        s.io(channelBonusInfo);
    }
};

// EGS_CHECK_CHANNEL_CHANGE_REQ
struct CheckChannelChangeReq {
    int channelId = 0;
    void serialize(Serializer& s) { s.io(channelId); }
};

// --------------------------------------------------------------------------- units
struct CreateUnitReq {
    WideString nickName;
    int unitClass = 0;
    void serialize(Serializer& s) {
        s.io(nickName);
        s.io(unitClass);
    }
};

struct DeleteUnitReq {
    UidType unitUid = 0;
    UidType userUid = 0;  // SERV_UNIT_DELETE_EXCEPTION_MODIFY
    void serialize(Serializer& s) {
        s.io(unitUid);
        s.io(userUid);
    }
};

struct DeleteUnitAck {
    int ok = 0;
    UidType unitUid = 0;
    UINT32 knmSerialNum = 0;
    int guildUid = 0;     // GUILD_TEST
    WideString nickName;  // SERV_CHAR_LOG
    UCHAR level = 0;      // SERV_CHAR_LOG
    char unitClass = 0;   // SERV_CHAR_LOG
    // SERV_UNIT_WAIT_DELETE is disabled, so m_tReDelAbleDate / m_tDelAbleDate are absent.

    void serialize(Serializer& s) {
        s.io(ok);
        s.io(unitUid);
        s.io(knmSerialNum);
        s.io(guildUid);
        s.io(nickName);
        s.io(level);
        s.io(unitClass);
    }
};

}  // namespace knc
