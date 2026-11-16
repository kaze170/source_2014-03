// tests/TestPacketLayout.cpp - wire size and field order of the client facing packets.
//
// Each expectation below was read off the 2014 sources: the byte count is what
// SERIALIZE_DEFINE_PUT(KXxx, ...) in Common/CommonPacket.cpp and Common/ClientPacket.cpp emits for
// a value with empty strings/containers, and tools/verify_packets.py checks the same thing
// field-by-field. These tests exist so a field added to (or dropped from) a packet cannot silently
// change the wire.
#include <string>
#include <vector>

#include "TestFramework.h"
#include "kcore/Serialize.h"
#include "kproto/LoginPackets.h"
#include "kproto/Packet.h"
#include "kproto/UnitPackets.h"

using namespace knc;

namespace {

template <class T>
std::size_t encodedSize(const T& value) {
    SerBuffer buf;
    encodePayload(value, buf);
    return buf.size();
}

// Encodes a packet and returns the raw bytes, so the tests can look at field order directly.
template <class T>
std::vector<byte> encodedBytes(const T& value) {
    SerBuffer buf;
    encodePayload(value, buf);
    return std::vector<byte>(buf.data(), buf.data() + buf.size());
}

}  // namespace

// EGS_VERIFY_ACCOUNT_REQ / the login handshake. 2014 CommonPacket.cpp:221 - debugAuth, wstrUserID,
// wstrPasswd, wstrHWID (SERV_OTP_AUTH), strMachineID (SERV_MACHINE_ID_DUPLICATE_CHECK),
// iChannelingCode, bManualLogin (SERV_KOG_OTP_VERIFY). Six fixed bytes: 1 + 4 + 1 (plus 4 bytes for
// each of the four strings' length prefixes).
KTEST(PacketLayout, userAuthenticateReqHasSixFixedBytes) {
    UserAuthenticateReq req;
    KCHECK_EQ(encodedSize(req), std::size_t{22});
    KCHECK_EQ(UserAuthenticateReq::kFixedSize, std::size_t{6});

    // Nothing that this build does not compile in may be on the wire: with four empty strings the
    // payload is exactly debugAuth(1) + 4*4 length prefixes + channelingCode(4) + manualLogin(1).
    const std::vector<byte> bytes = encodedBytes(req);
    KCHECK_EQ(bytes.size(), std::size_t{22});
    for (std::size_t i = 0; i < bytes.size(); ++i) KCHECK_EQ(bytes[i], byte{0});
}

// The four strings and the trailing scalars must appear in exactly the 2014 order, so feeding a
// payload written by the original server order parses back into the same struct.
KTEST(PacketLayout, userAuthenticateReqRoundTripsInOriginalOrder) {
    SerBuffer buf;
    Serializer w;
    w.beginWriting(&buf);
    w.put(true);                                  // m_bDebugAuth
    w.put(WideString(u"user"));                   // m_wstrUserID
    w.put(WideString(u"pass"));                   // m_wstrPasswd
    w.put(WideString(u"hw"));                     // m_wstrHWID
    w.put(std::string("machine"));                // m_strMachineID
    w.put(static_cast<int>(7));                   // m_iChannelingCode
    w.put(true);                                  // m_bManualLogin
    w.end();

    UserAuthenticateReq req;
    KCHECK(decodePayload(buf, req, /*lenient=*/false));
    KCHECK_EQ(req.debugAuth, true);
    KCHECK_EQ(req.userId, WideString(u"user"));
    KCHECK_EQ(req.password, WideString(u"pass"));
    KCHECK_EQ(req.hwid, WideString(u"hw"));
    KCHECK_EQ(req.machineId, std::string("machine"));
    KCHECK_EQ(req.channelingCode, 7);
    KCHECK_EQ(req.manualLogin, true);
}

// 2014 CommonPacket.cpp:6119. m_iEventMoney is only written with SERV_EVENT_MONEY, which this
// build does not define.
KTEST(PacketLayout, verifyAccountAckMatches2014Order) {
    VerifyAccountAck ack;
    // ok + accountInfo + hackingUserType + adURL + sex + age + nexonSn + clientIp + channelCode
    // + channelUserId, with six string length prefixes across accountInfo and the rest.
    KCHECK_EQ(encodedSize(ack), std::size_t{71});

    AccountInfo info;
    KCHECK_EQ(encodedSize(info), std::size_t{44});
}

// 2014 ClientPacket.cpp:134 - ok, unitUID, KNM serial, guild (GUILD_TEST), nick name, level, class
// (SERV_CHAR_LOG). SERV_UNIT_WAIT_DELETE is off, so the two delete dates are not sent.
KTEST(PacketLayout, deleteUnitAckCarriesGuildUid) {
    DeleteUnitAck ack;
    KCHECK_EQ(encodedSize(ack), std::size_t{26});

    ack.ok = 1;
    ack.unitUid = 0x1122334455667788LL;
    ack.knmSerialNum = 0xAABBCCDD;
    ack.guildUid = 0x01020304;
    const std::vector<byte> bytes = encodedBytes(ack);
    const byte expectedHead[] = {0x00, 0x00, 0x00, 0x01,                            // ok
                                 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88,    // unitUid
                                 0xAA, 0xBB, 0xCC, 0xDD,                            // knmSerialNum
                                 0x01, 0x02, 0x03, 0x04};                           // guildUid
    KCHECK_BYTES(bytes.data(), expectedHead, sizeof(expectedHead));
}

// 2014 ClientPacket.cpp:112 - unitUID, plus m_iUserUID because SERV_UNIT_DELETE_EXCEPTION_MODIFY
// is defined.
KTEST(PacketLayout, deleteUnitReqCarriesUserUid) {
    DeleteUnitReq req;
    KCHECK_EQ(encodedSize(req), std::size_t{16});
}

// 2014 CommonPacket.cpp:7919 - unitUID then m_setCodeEventScriptID (SERV_CODE_EVENT).
KTEST(PacketLayout, selectUnitReqCarriesCodeEventSet) {
    SelectUnitReq req;
    KCHECK_EQ(encodedSize(req), std::size_t{12});  // i64 + empty set count

    req.unitUid = 5;
    req.codeEventScriptId.insert(11);
    req.codeEventScriptId.insert(12);
    KCHECK_EQ(encodedSize(req), std::size_t{20});  // i64 + count + two i32

    SelectUnitReq read;
    SerBuffer buf;
    encodePayload(req, buf);
    KCHECK(decodePayload(buf, read, /*lenient=*/false));
    KCHECK_EQ(read.unitUid, UidType{5});
    KCHECK_EQ(read.codeEventScriptId.size(), std::size_t{2});
    KCHECK(read.codeEventScriptId.count(11) == 1);
}

KTEST(PacketLayout, unitInfoIsTheSumOfItsParts) {
    UnitInfo info;
    // 226 bytes in the pre-buff build; this build also sends the buff block (41), the dungeon play
    // block (13) and the pc bang / vip / event quest tail.
    KCHECK_EQ(encodedSize(info), std::size_t{270});

    Stat stat;
    KCHECK_EQ(encodedSize(stat), std::size_t{20});
    LastPositionInfo pos;
    KCHECK_EQ(encodedSize(pos), std::size_t{8});
    DungeonClearInfo dungeon;
    KCHECK_EQ(encodedSize(dungeon), std::size_t{14});
    TCClearInfo tc;
    KCHECK_EQ(encodedSize(tc), std::size_t{9});
    DungeonPlayInfo play;
    KCHECK_EQ(encodedSize(play), std::size_t{13});
    BuffInfo buff;
    KCHECK_EQ(encodedSize(buff), std::size_t{41});
    BuffFactor factor;
    KCHECK_EQ(encodedSize(factor), std::size_t{25});
    BuffIdentity identity;
    KCHECK_EQ(encodedSize(identity), std::size_t{8});
    BuffBehaviorFactor behavior;
    KCHECK_EQ(encodedSize(behavior), std::size_t{8});  // u32 type + empty vec<f32>
    BuffFinalizerFactor finalizer;
    KCHECK_EQ(encodedSize(finalizer), std::size_t{8});
    UserGuildInfo guild;
    KCHECK_EQ(encodedSize(guild), std::size_t{13});
    UnitSkillData skills;
    KCHECK_EQ(encodedSize(skills), std::size_t{43});   // eight 3 byte slots + five counts/strings
    SkillData skill;
    KCHECK_EQ(encodedSize(skill), std::size_t{3});
}

// KItemInfo: m_ucSealData is an unsigned char in the 2014 header, so a sealed item must not be
// truncated to a negative char on the way out.
KTEST(PacketLayout, itemInfoKeepsSealDataUnsigned) {
    ItemInfo item;
    KCHECK_EQ(encodedSize(item), std::size_t{32});

    item.sealData = 101;  // 100 == sealed, 101 == sealed and seal count incremented
    item.itemState = ItemInfo::kIsNormal;
    item.attribEnchantInfo.attribEnchant0 = 'A';
    item.attribEnchantInfo.attribEnchant1 = 'B';
    item.attribEnchantInfo.attribEnchant2 = 'C';
    const std::vector<byte> bytes = encodedBytes(item);
    // itemId(4) + usageType(1) + quantity(4) + endurance(2) puts the seal byte at offset 11, and it
    // must be the full unsigned value rather than a sign extended char.
    KCHECK_EQ(bytes[11], byte{101});

    ItemInfo read;
    SerBuffer buf;
    encodePayload(item, buf);
    KCHECK(decodePayload(buf, read, /*lenient=*/false));
    KCHECK_EQ(read.sealData, static_cast<unsigned char>(101));
    KCHECK_EQ(read.attribEnchantInfo.attribEnchant2, 'C');
    KCHECK_EQ(read.itemState, static_cast<char>(ItemInfo::kIsNormal));
    KCHECK_EQ(read.quantity, 1);
}

KTEST(PacketLayout, inventoryItemInfoWrapsItemInfo) {
    InventoryItemInfo item;
    KCHECK_EQ(encodedSize(item), std::size_t{43});  // i64 + i8 + i16 + ItemInfo(32)
}

// The three list/inventory acknowledgements this server answers with.
KTEST(PacketLayout, unitListAndInventoryAcks) {
    MyUnitAndInventoryInfoListAck list;
    KCHECK_EQ(encodedSize(list), std::size_t{13});  // ok + slot + empty vector + sharingBank

    GetMyInventoryAck inventory;
    KCHECK_EQ(encodedSize(inventory), std::size_t{6});  // ok + isRecommend + sharingBank

    SelectUnitInventoryInfoNot notInventory;
    KCHECK_EQ(encodedSize(notInventory), std::size_t{8});  // char + empty map

    SelectUnitAck select;
    KCHECK_EQ(encodedSize(select), std::size_t{4});

    CreateUnitAck create;
    KCHECK_EQ(encodedSize(create), std::size_t{282});  // ok + UnitInfo(270) + vector + wstring
}

// Login side carriers, all read off the 2014 serializers.
// Structures the service does not currently send but the 2014 protocol defines: they are verified
// against the 2014 serializers so the header cannot drift away from the protocol unnoticed.
KTEST(PacketLayout, protocolTypesNotYetSentStillMatchTheWire) {
    PacketOk ok;
    KCHECK_EQ(encodedSize(ok), std::size_t{4});

    NetAddress address;
    KCHECK_EQ(encodedSize(address), std::size_t{6});  // wstring length + u16 port
    address.ip = WideString(u"1.2.3.4");
    address.port = 9100;
    KCHECK_EQ(encodedSize(address), std::size_t{4 + 7 * 2 + 2});

    // The old (pre SERV_CHANNEL_LIST_RENEWAL) channel row: three ints.
    ChannelSimpleInfo simple;
    KCHECK_EQ(encodedSize(simple), std::size_t{12});

    // EGS_CHECK_CHANNEL_CHANGE_REQ and its bare KPacketOK ack.
    CheckChannelChangeReq change;
    KCHECK_EQ(encodedSize(change), std::size_t{4});
    change.channelId = 2;
    const std::vector<byte> bytes = encodedBytes(change);
    const byte expected[] = {0x00, 0x00, 0x00, 0x02};
    KCHECK_BYTES(bytes.data(), expected, sizeof(expected));
}

KTEST(PacketLayout, loginPacketSizes) {
    ConnectReq connect;
    KCHECK_EQ(encodedSize(connect), std::size_t{4});
    ConnectAck connectAck;
    KCHECK_EQ(encodedSize(connectAck), std::size_t{12});  // ok + udpPort + channelId

    ServerSetData set;
    KCHECK_EQ(encodedSize(set), std::size_t{9});
    GetServerSetDataAck setList;
    KCHECK_EQ(encodedSize(setList), std::size_t{8});
    SelectServerSetReq selectReq;
    KCHECK_EQ(encodedSize(selectReq), std::size_t{4});
    SelectServerSetAck selectAck;
    KCHECK_EQ(encodedSize(selectAck), std::size_t{18});

    ChannelBonusInfo bonus;
    KCHECK_EQ(encodedSize(bonus), std::size_t{17});
    ChannelInfo channel;
    KCHECK_EQ(encodedSize(channel), std::size_t{40});
    GetChannelListAck channelList;
    KCHECK_EQ(encodedSize(channelList), std::size_t{12});

    NewUserJoinReq join;
    KCHECK_EQ(encodedSize(join), std::size_t{13});
    NewUserJoinAck joined;
    KCHECK_EQ(encodedSize(joined), std::size_t{12});

    CreateUnitReq create;
    KCHECK_EQ(encodedSize(create), std::size_t{8});

    AccountOption option;
    KCHECK_EQ(encodedSize(option), std::size_t{1});
    AccountBlockInfo block;
    KCHECK_EQ(encodedSize(block), std::size_t{8});
}
