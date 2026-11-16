// tests/TestWire.cpp - byte level conformance tests for the X2 wire protocol.
#include <cstring>
#include <string>
#include <vector>

#include "TestFramework.h"
#include "kcore/ByteOrder.h"
#include "kcore/Serialize.h"
#include "kcore/Text.h"
#include "kproto/Event.h"
#include "kproto/EventId.h"
#include "kproto/FeatureFlags.gen.h"
#include "ksec/Des.h"
#include "ksec/Md5.h"
#include "ksec/SecureBuffer.h"

using namespace knc;

KTEST(ByteOrder, bigEndianNumericEncoding) {
    SerBuffer buf;
    Serializer s;
    s.beginWriting(&buf);
    s.put(static_cast<USHORT>(0x1234));
    s.put(static_cast<DWORD>(0x89ABCDEF));
    s.put(static_cast<int>(-2));
    s.put(static_cast<INT64>(0x0102030405060708LL));
    s.put(true);
    s.put(static_cast<byte>(0x7F));
    s.end();

    const byte expected[] = {0x12, 0x34, 0x89, 0xAB, 0xCD, 0xEF, 0xFF, 0xFF, 0xFF, 0xFE,
                             0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x01, 0x7F};
    KCHECK_EQ(buf.size(), sizeof(expected));
    KCHECK_BYTES(buf.data(), expected, sizeof(expected));

    // and read back
    Serializer r;
    r.beginReading(&buf);
    USHORT a = 0;
    DWORD b = 0;
    int c = 0;
    INT64 d = 0;
    bool e = false;
    byte f = 0;
    r.get(a, b, c, d, e, f);
    r.end();
    KCHECK(r.ok());
    KCHECK_EQ(a, 0x1234);
    KCHECK_EQ(b, 0x89ABCDEFu);
    KCHECK_EQ(c, -2);
    KCHECK_EQ(d, 0x0102030405060708LL);
    KCHECK_EQ(e, true);
    KCHECK_EQ(f, 0x7F);
}

KTEST(Serializer, stringAndWideStringLayout) {
    SerBuffer buf;
    Serializer s;
    s.beginWriting(&buf);
    s.put(std::string("AB"));
    s.put(WideString(u"하"))  ;   // U+D558 -> D5 58 little endian on the wire
    s.end();

    // UTF-16LE: U+D558 is stored low byte first (58 D5); KSerializer writes the raw wchar_t
    // memory, and the client is little endian.
    const byte expected[] = {0x00, 0x00, 0x00, 0x02, 'A', 'B',                    // "AB"
                             0x00, 0x00, 0x00, 0x02, 0x58, 0xD5};                  // wide "하"
    KCHECK_EQ(buf.size(), sizeof(expected));
    KCHECK_BYTES(buf.data(), expected, sizeof(expected));

    Serializer r;
    r.beginReading(&buf);
    std::string a;
    WideString b;
    r.get(a, b);
    r.end();
    KCHECK(r.ok());
    KCHECK_EQ(a, std::string("AB"));
    KCHECK_EQ(b.size(), std::size_t(1));
    KCHECK_EQ(static_cast<int>(b[0]), 0xD558);
}

KTEST(Serializer, containersAndPairs) {
    SerBuffer buf;
    Serializer s;
    s.beginWriting(&buf);
    std::vector<int> v{1, 2, 3};
    std::set<UidType> uids{-1, 7};
    std::map<std::string, int> m{{"x", 1}};
    s.put(v, uids, m);
    s.end();

    // vector: count(4) + 3 ints(12)
    // set:    count(4) + 2 x INT64(16)
    // map:    count(4) + key("x": len(4) + 1 byte) + value(4)
    KCHECK_EQ(buf.size(), std::size_t(4 + 12 + 4 + 16 + 4 + 4 + 1 + 4));
    const byte head[] = {0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00, 0x01};
    KCHECK_BYTES(buf.data(), head, sizeof(head));

    Serializer r;
    r.beginReading(&buf);
    std::vector<int> v2;
    std::set<UidType> u2;
    std::map<std::string, int> m2;
    r.get(v2, u2, m2);
    r.end();
    KCHECK(r.ok());
    KCHECK_EQ(v2.size(), std::size_t(3));
    KCHECK_EQ(v2[2], 3);
    KCHECK_EQ(u2.count(-1), std::size_t(1));
    KCHECK_EQ(u2.count(7), std::size_t(1));
    KCHECK_EQ(m2["x"], 1);
}

KTEST(SerBuffer, compressionRoundTrip) {
    SerBuffer buf;
    std::string payload(400, 'Z');
    buf.append(payload.data(), payload.size());
    const std::size_t original = buf.size();
    KCHECK(buf.compress());
    KCHECK(buf.compressed());
    KCHECK(buf.size() < original);
    // Layout: [u32 LE origin size][zlib stream]
    const byte* p = buf.data();
    const std::uint32_t le = static_cast<std::uint32_t>(p[0]) | (static_cast<std::uint32_t>(p[1]) << 8) |
                             (static_cast<std::uint32_t>(p[2]) << 16) |
                             (static_cast<std::uint32_t>(p[3]) << 24);
    KCHECK_EQ(le, original);
    KCHECK(buf.uncompress());
    KCHECK_EQ(buf.size(), original);
    KCHECK(std::memcmp(buf.data(), payload.data(), original) == 0);
}

KTEST(SerBuffer, serialisedInsideEvent) {
    Event ev;
    ev.setData(kPiGsUser, nullptr, EGS_CONNECT_ACK);
    SerBuffer inner;
    std::string raw = "hello";
    inner.append(raw.data(), raw.size());

    ev.payload = inner;
    SerBuffer wire;
    Serializer s;
    s.beginWriting(&wire);
    s.put(ev);
    s.end();

    // envelope: performer(4) + uidCount(4) + trace0(8) + trace1(8) + eventId(2) + bufLen(4)
    //           + compressed flag(1) + 5 bytes
    KCHECK_EQ(wire.size(), std::size_t(4 + 4 + 8 + 8 + 2 + 4 + 1 + 5));
    // envelope offsets: performer 0..3, uidCount 4..7, trace 8..23, eventId 24..25
    KCHECK_EQ(wire.data()[24], static_cast<byte>((EGS_CONNECT_ACK >> 8) & 0xFF));
    KCHECK_EQ(wire.data()[25], static_cast<byte>(EGS_CONNECT_ACK & 0xFF));

    Event decoded;
    Serializer r;
    r.beginReading(&wire);
    r.get(decoded);
    r.end();
    KCHECK(r.ok());
    KCHECK_EQ(decoded.eventId, EGS_CONNECT_ACK);
    KCHECK_EQ(decoded.payload.size(), std::size_t(5));
    KCHECK(std::memcmp(decoded.payload.data(), "hello", 5) == 0);
}

KTEST(EventId, knownValuesMatchClientEnumeration) {
    // System ids
    KCHECK_EQ(E_HEART_BEAT, 0);
    KCHECK_EQ(E_ACCEPT_CONNECTION_NOT, 1);
    KCHECK_EQ(E_CONNECTION_LOST_NOT, 2);
    KCHECK_EQ(E_UDP_PORT_NOT, 3);
    // The client id block starts right after the system block.
    KCHECK(EGS_GOOD_JOB_1_REQ > E_CONNECTION_LOST_NOT);
    KCHECK(EGS_CONNECT_REQ == EGS_GOOD_JOB_5_REQ + 1);
    KCHECK_EQ(std::string(eventIdName(EGS_CONNECT_ACK)), std::string("EGS_CONNECT_ACK"));
    KCHECK_EQ(std::string(eventIdName(0)), std::string("E_HEART_BEAT"));
    KCHECK_EQ(std::string(eventIdName(0xFFF0)), std::string("E_UNKNOWN"));
}

// FeatureFlags.gen.h decides which fields every packet has. These assertions are the tripwire for
// a regenerated flag table: if a flag that guards a wire field ever flips, this fails instead of the
// bytes quietly moving. tools/verify_packets.py checks the same thing by construction.
KTEST(PacketFlags, wireDecidingFlagsAreTheExpectedValues) {
    using namespace knc::features;
    KCHECK(kSERV_OTP_AUTH);
    KCHECK(kSERV_MACHINE_ID_DUPLICATE_CHECK);
    KCHECK(kSERV_KOG_OTP_VERIFY);
    KCHECK(kSERV_CLIENT_PUBLIC_IP);
    KCHECK(kSERV_CHAR_LOG);
    KCHECK(kSERV_UNIT_DELETE_EXCEPTION_MODIFY);
    KCHECK(kGUILD_TEST);
    KCHECK(kSERV_CODE_EVENT);
    KCHECK(kSERV_SHARING_BANK_QUEST_CASH);
    KCHECK(kSERV_NEW_ITEM_SYSTEM_2013_05);
    KCHECK(kSERV_BATTLE_FIELD_BOSS);
    KCHECK(kSERV_TITLE_DATA_SIZE);
    KCHECK(kSERV_LIMITED_DUNGEON_PLAY_TIMES);
    KCHECK(kSERV_ADD_WARP_BUTTON);
    KCHECK(kSERV_GROW_UP_SOCKET);
    KCHECK(kSERV_PC_BANG_TYPE);
    KCHECK(kSERV_REFORM_THE_GATE_OF_DARKNESS);
    KCHECK(kSERV_ITEM_OPTION_DATA_SIZE);
    KCHECK(kSERV_EXPAND_SLOT_ID_DATA_SIZE);
    KCHECK(kSERV_SKILL_PAGE_SYSTEM);
    KCHECK(kGUILD_SKILL_TEST);
    KCHECK(kSERV_SKILL_NOTE);

    // ... and the ones this build leaves out. Every one of them removes bytes from a packet.
    KCHECK(!kSERV_UNIT_WAIT_DELETE);
    KCHECK(!kSERV_DLL_LIST_CHECK_BEFOR_LOADING);
    KCHECK(!kSERV_SERIAL_NUMBER_AVAILABILITY_CHECK_IN_GAMESERVER);
    KCHECK(!kSERV_EVENT_MONEY);
    KCHECK(!kSERV_HACKING_USER_CHECK_COUNT);
    KCHECK(!kSERV_INTEGRATE_THE_CHANNEL_INFO);
    KCHECK(!kSERV_GLOBAL_EVENT_TABLE);
    KCHECK(!kSERV_GOLD_TICKET);
    KCHECK(!kSERV_STEAM);
    KCHECK(!kSERV_CHANNELING_AERIA);

    // Region: the tree selects none, so the shared/global configuration is what is compiled.
    KCHECK(!kSERV_COUNTRY_CN);
    KCHECK(!kSERV_COUNTRY_TH);
    KCHECK(!kSERV_COUNTRY_US);
}

KTEST(Performer, identifiersMatchOriginalHeader) {
    KCHECK_EQ(kPiGsProxy, 0x00000101u);
    KCHECK_EQ(kPiGsUser, 0x00000202u);
    KCHECK_EQ(kPiCnUser, 0x00000302u);
    KCHECK_EQ(kPiLoginUser, 0x00000502u);
    KCHECK_EQ(kPiChannelUser, 0x00000402u);
    KCHECK_EQ(kPiGlobalUser, 0x00000702u);
    KCHECK_EQ(kPiAccountDb, 0x00000006u);
    KCHECK_EQ(performerClass(kPiGsUser), static_cast<UINT32>(kPcUser));
    KCHECK_EQ(serverClass(kPiGsUser), static_cast<UINT32>(kScGame));
    KCHECK_EQ(kPiNull, 0xFFFFFFFFu);
}

KTEST(Security, defaultKeysAreTheCp949Bytes) {
    std::array<byte, sec::kAuthKeySize> auth{};
    std::array<byte, sec::kCryptoKeySize> crypt{};
    sec::defaultKeys(auth, crypt);
    const byte expectedAuth[8] = {0xC0, 0xD3, 0xBD, 0xC3, 0xB7, 0xCE, 0xB8, 0xB8};
    const byte expectedCrypt[8] = {0xC7, 0xD8, 0xC4, 0xBF, 0xB5, 0xE9, 0xC0, 0xFD};
    KCHECK_BYTES(auth.data(), expectedAuth, 8);
    KCHECK_BYTES(crypt.data(), expectedCrypt, 8);
    // Sanity: the CP949 encoder agrees.
    KCHECK_EQ(cp949ToWide("\xC0\xD3\xBD\xC3\xB7\xCE\xB8\xB8").size(), std::size_t(4));
}

KTEST(Security, secureBufferRoundTripSpi0) {
    const std::string plain = "the quick brown fox";
    sec::SecureBuffer sender(0);
    KCHECK(sender.create_notRWM(reinterpret_cast<const byte*>(plain.data()), plain.size()));

    // Wire layout checks: SPI(2) + seq(4) + IV(8) + cipher(N*8) + ICV(10)
    const std::size_t cipherLen = sender.size() - 2 - 4 - 8 - 10;
    KCHECK_EQ((sender.size() - 24) % 8, std::size_t(0));
    KCHECK(cipherLen >= plain.size());
    KCHECK_EQ(sender.dataPtr()[0], static_cast<byte>(0));
    KCHECK_EQ(sender.dataPtr()[1], static_cast<byte>(0));
    // Sequence number starts at 1 and SPI 0 does not advance it.
    KCHECK_EQ(sender.dataPtr()[2], static_cast<byte>(1));
    KCHECK_EQ(sender.dataPtr()[3], static_cast<byte>(0));

    sec::SecureBuffer receiver(0, sender.dataPtr(), sender.size());
    KCHECK(receiver.isAuthentic_notRWM(true));
    Bytes out;
    KCHECK(receiver.getPayload_notRWM(out, false));
    KCHECK_EQ(out.size(), plain.size());
    KCHECK(std::memcmp(out.data(), plain.data(), plain.size()) == 0);
}

KTEST(Security, tamperedPacketIsRejected) {
    const std::string plain = "tamper me";
    sec::SecureBuffer sender(0);
    KCHECK(sender.create_notRWM(reinterpret_cast<const byte*>(plain.data()), plain.size()));

    std::vector<byte> copy(sender.dataPtr(), sender.dataPtr() + sender.size());
    copy[12] ^= 0x01;  // flip a cipher bit
    sec::SecureBuffer receiver(0, copy.data(), copy.size());
    KCHECK(!receiver.isAuthentic_notRWM(true));

    std::vector<byte> copy2(sender.dataPtr(), sender.dataPtr() + sender.size());
    copy2[copy2.size() - 1] ^= 0x80;  // flip an ICV bit
    sec::SecureBuffer receiver2(0, copy2.data(), copy2.size());
    KCHECK(!receiver2.isAuthentic_notRWM(true));
}

KTEST(Security, sequenceNumberAdvancesOnlyForRealSpi) {
    auto& db = sec::SecurityDatabase::instance();
    const SpiType spi = db.createNewSA();
    KCHECK(spi >= 1);

    sec::SecureBuffer first(spi);
    KCHECK(first.create_notRWM(nullptr, 0));
    const SeqType seq1 = db.get(spi).sequenceNum();
    sec::SecureBuffer second(spi);
    KCHECK(second.create_notRWM(nullptr, 0));
    const SeqType seq2 = db.get(spi).sequenceNum();
    KCHECK_EQ(seq2, seq1 + 1);

    // Round trip on the receiver with sequence checking enabled.
    sec::SecureBuffer rx(spi, second.dataPtr(), second.size());
    KCHECK(rx.isAuthentic_notRWM(true));
    db.remove(spi);
    KCHECK(!db.contains(spi));
}

KTEST(Security, handshakePayloadCarriesSpiAndAssociation) {
    auto& db = sec::SecurityDatabase::instance();
    const SpiType spi = db.createNewSA();

    SerBuffer payload;
    Serializer s;
    s.beginWriting(&payload);
    s.put(spi);
    s.put(db.get(spi));
    s.end();

    // SpiType (2) + keyLen(4) + 8 + keyLen(4) + 8 + seq(4) + last(4) + mask(4) == 38
    KCHECK_EQ(payload.size(), std::size_t(2 + 4 + 8 + 4 + 8 + 4 + 4 + 4));

    Serializer r;
    r.beginReading(&payload);
    SpiType readSpi = 0;
    sec::SecurityAssociation sa;
    r.get(readSpi, sa);
    r.end();
    KCHECK(r.ok());
    KCHECK_EQ(readSpi, spi);
    KCHECK(std::memcmp(sa.authKey().data(), db.get(spi).authKey().data(), 8) == 0);
    KCHECK(std::memcmp(sa.cryptoKey().data(), db.get(spi).cryptoKey().data(), 8) == 0);
    db.remove(spi);
}

KTEST(Text, utf8Utf16Conversion) {
    const std::string utf8 = "Elsword 한글";
    const WideString wide = toWide(utf8);
    KCHECK_EQ(wide.size(), std::size_t(10));
    KCHECK_EQ(toUtf8(wide), utf8);
    // Surrogate pair round trip
    const std::string emoji = "\xF0\x9F\x98\x80";  // U+1F600
    const WideString w2 = toWide(emoji);
    KCHECK_EQ(w2.size(), std::size_t(2));
    KCHECK_EQ(toUtf8(w2), emoji);
}

KTEST(Text, cp949Conversion) {
    // "솔레스" in CP949
    const std::string solares = "\xBC\xDB\xB7\xB9\xBD\xBA";
    const WideString wide = cp949ToWide(solares);
    KCHECK_EQ(wide.size(), std::size_t(3));
    KCHECK_EQ(static_cast<int>(wide[0]), 0xC1A1);  // 송
    KCHECK_EQ(static_cast<int>(wide[1]), 0xB808);  // 레
    KCHECK_EQ(wideToCp949(wide), solares);
}

KTEST(Crypto, desKnownAnswer) {
    const byte key[8] = {0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF};
    const byte iv[8] = {0, 0, 0, 0, 0, 0, 0, 0};
    const char* plain = "Now is t";
    byte out[8]{};
    const sec::Des des(key);
    sec::desCbcEncrypt(des, iv, reinterpret_cast<const byte*>(plain), out, 8);
    const byte expected[8] = {0x3F, 0xA4, 0x0E, 0x8A, 0x98, 0x4D, 0x48, 0x15};
    KCHECK_BYTES(out, expected, 8);

    byte back[8]{};
    sec::desCbcDecrypt(des, iv, out, back, 8);
    KCHECK_BYTES(back, plain, 8);
}

KTEST(Crypto, md5KnownAnswers) {
    sec::Md5 md5;
    md5.update("abc", 3);
    const auto digest = md5.digest();
    const char* hex = "900150983cd24fb0d6963f7d28e17f72";
    char actual[33]{};
    for (int i = 0; i < 16; ++i) std::snprintf(actual + i * 2, 3, "%02x", digest[i]);
    KCHECK_EQ(std::string(actual), std::string(hex));

    sec::Md5 empty;
    const auto d2 = empty.digest();
    char actual2[33]{};
    for (int i = 0; i < 16; ++i) std::snprintf(actual2 + i * 2, 3, "%02x", d2[i]);
    KCHECK_EQ(std::string(actual2), std::string("d41d8cd98f00b204e9800998ecf8427e"));
}

KTEST(Crypto, hmacMd5Truncated) {
    // RFC 2202 test case 1 truncated to 10 bytes.
    const byte key[16] = {0x0b, 0x0b, 0x0b, 0x0b, 0x0b, 0x0b, 0x0b, 0x0b,
                          0x0b, 0x0b, 0x0b, 0x0b, 0x0b, 0x0b, 0x0b, 0x0b};
    const char* data = "Hi There";
    byte out[10]{};
    sec::hmacMd5(key, sizeof(key), data, 8, out, sizeof(out));
    const byte expected[10] = {0x92, 0x94, 0x72, 0x7a, 0x36, 0x38, 0xbb, 0x1c, 0x13, 0xf4};
    KCHECK_BYTES(out, expected, 10);
}
