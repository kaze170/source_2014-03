// kproto/LoginPackets.h - the login/channel handshake packets, field for field.
//
// Every struct here mirrors a DECL_DATA / DECL_PACKET in KncWX2Server/Common/{CommonPacket.h,
// ClientPacket.h} and its SERIALIZE_DEFINE_PUT order. The feature-flag #ifdef blocks are resolved
// with the single fixed flag set in include/kproto/FeatureFlags.gen.h (568 enabled / 133 disabled),
// which is the same set the rest of the rewrite is built with.
#pragma once

#include <cstdint>
#include <map>
#include <string>
#include <vector>

#include "kcore/Serialize.h"
#include "kcore/Types.h"
#include "kproto/Packet.h"

namespace knc {

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
    int channelRandomKey = 0; // SERV_DLL_LIST_CHECK_BEFOR_LOADING

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
        s.io(channelRandomKey);
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
    int channelRandomKey = 0;        // SERV_DLL_LIST_CHECK_BEFOR_LOADING
    std::vector<byte> charServerSn;  // SERV_SERIAL_NUMBER_AVAILABILITY_CHECK_IN_GAMESERVER, SERVER_SN bytes
    WideString clientTime;           // SERV_CHECK_MACHINE_LOCALTIME
    int channelingCode = 0;
    bool manualLogin = false;        // SERV_KOG_OTP_VERIFY
    WideString socketId;             // SERV_COUNTRY_TH
    bool steamClient = false;        // SERV_STEAM
    bool aeriaClient = false;        // SERV_CHANNELING_AERIA

    UserAuthenticateReq() : charServerSn(kServerSn, 0) {}

    static constexpr std::size_t kServerSn = 8;

    void serialize(Serializer& s) {
        s.io(debugAuth);
        s.io(userId);
        s.io(password);
        s.io(hwid);
        s.io(machineId);
        s.io(channelRandomKey);
        s.io(charServerSn);
        s.io(clientTime);
        s.io(channelingCode);
        s.io(manualLogin);
        s.io(socketId);
        s.io(steamClient);
        s.io(aeriaClient);
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
    bool udpKickOff = false;  // UDP_CAN_NOT_SEND_USER_KICK
    void serialize(Serializer& s) {
        s.io(ok);
        s.io(udpPort);
        s.io(channelId);
        s.io(udpKickOff);
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
    int eventMoney = 0;       // SERV_EVENT_MONEY

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
        s.io(eventMoney);
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
struct ChannelBonusInfo {
    int expBonus = 0;
    int dropBonus = 0;
    void serialize(Serializer& s) {
        s.io(expBonus);
        s.io(dropBonus);
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
    ChannelBonusInfo bonusInfo;  // SERV_INTEGRATE_THE_CHANNEL_INFO

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
        s.io(bonusInfo);
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
    WideString nickName;  // SERV_CHAR_LOG
    UCHAR level = 0;
    char unitClass = 0;
    std::int64_t reDeleteAbleDate = 0;  // SERV_UNIT_WAIT_DELETE
    std::int64_t deleteAbleDate = 0;

    void serialize(Serializer& s) {
        s.io(ok);
        s.io(unitUid);
        s.io(knmSerialNum);
        s.io(nickName);
        s.io(level);
        s.io(unitClass);
        s.io(reDeleteAbleDate);
        s.io(deleteAbleDate);
    }
};

}  // namespace knc
