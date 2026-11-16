// kproto/EventId.h - event id constants and the performer (routing) identifiers.
#pragma once

#include <array>
#include <string>

#include "kcore/Types.h"
#include "kproto/EventId.gen.h"

namespace knc {

// ---------------------------------------------------------------------------
// Performer classes, reproduced from KncWX2Server/Common/KncSend.h. A performer id (PI_*) is what
// KEvent::m_kDestPerformer.m_dwPerformerID carries; it selects the server subsystem and the kind of
// actor the event is addressed to.
enum PerformerClass : std::uint32_t {
    kPcProxy = 0x00000001,
    kPcUser = 0x00000002,
    kPcCharacter = 0x00000003,
    kPcServer = 0x00000004,
    kPcRoom = 0x00000005,
    kPcAccountDb = 0x00000006,
    kPcGameDb = 0x00000007,
    kPcLogDb = 0x00000008,
    kPcSmsDb = 0x00000009,
    kPcNxWebDb = 0x0000000A,
    kPcNxAuth = 0x0000000B,
    kPcParty = 0x0000000C,
    kPcNxBilling = 0x0000000D,
    kPcNxBillingTcp = 0x0000000E,
    kPcMatchMaking = 0x0000000F,
    kPcAutoParty = 0x00000011,
    kPcEventDb = 0x00000091,
    kPcScriptDb = 0x00000092,

    kPcMaskBit = 0x000000FF,

    kScClient = 0x00000100,
    kScGame = 0x00000200,
    kScCenter = 0x00000300,
    kScChannel = 0x00000400,
    kScLogin = 0x00000500,
    kScSimulation = 0x00000600,
    kScGlobal = 0x00000700,
    kScMaskBit = 0x00000F00,

    kStSameServGroup = 0x00001000,
    kStMaskBit = 0x0000F000,
};

// Full performer identifiers (ePerformerID).
enum PerformerId : std::uint32_t {
    kPiGsProxy = kScClient | kPcProxy,

    kPiGsUser = kScGame | kPcUser,
    kPiGsCharacter = kScGame | kPcCharacter,
    kPiGsServer = kScGame | kPcServer,
    kPiGsServerSame = kScGame | kPcServer | kStSameServGroup,
    kPiGsGameDb = kScGame | kPcGameDb,
    kPiGsAccountDb = kScGame | kPcAccountDb,
    kPiGsLogDb = kScGame | kPcLogDb,
    kPiGsSmsDb = kScGame | kPcSmsDb,
    kPiGsField = kScGame | kPcRoom,
    kPiGsParty = kScGame | kPcParty,
    kPiGsChatLogDb = kScGame | 0x15,  // PC_CHAT_LOG_DB

    kPiCnUser = kScCenter | kPcUser,
    kPiCnServer = kScCenter | kPcServer,
    kPiCnRoom = kScCenter | kPcRoom,
    kPiCnParty = kScCenter | kPcParty,
    kPiCnAccountDb = kScCenter | kPcAccountDb,
    kPiCnGameDb = kScCenter | kPcGameDb,
    kPiCnLogDb = kScCenter | kPcLogDb,
    kPiCnSmsDb = kScCenter | kPcSmsDb,
    kPiCnEventDb = kScCenter | kPcEventDb,

    kPiLoginUser = kScLogin | kPcUser,
    kPiLoginServer = kScLogin | kPcServer,
    kPiLoginRoom = kScLogin | kPcRoom,
    kPiLoginAccountDb = kScLogin | kPcAccountDb,
    kPiLoginGameDb = kScLogin | kPcGameDb,
    kPiLoginLogDb = kScLogin | kPcLogDb,
    kPiLoginSmsDb = kScLogin | kPcSmsDb,
    kPiLoginNxAuth = kScLogin | kPcNxAuth,
    kPiLoginNxWebDb = kScLogin | kPcNxWebDb,
    kPiLoginEventDb = kScLogin | kPcEventDb,

    kPiChannelUser = kScChannel | kPcUser,
    kPiChannelServer = kScChannel | kPcServer,
    kPiChannelLogDb = kScChannel | kPcLogDb,
    kPiChannelAccountDb = kScChannel | kPcAccountDb,
    kPiChannelLoginProxy = kScChannel | kPcProxy,
    kPiChannelSmsDb = kScChannel | kPcSmsDb,
    kPiChannelGameDb = kScChannel | kPcGameDb,
    kPiChannelEventDb = kScChannel | kPcEventDb,

    kPiGlobalUser = kScGlobal | kPcUser,
    kPiGlobalServer = kScGlobal | kPcServer,
    kPiGlobalMatchMaking = kScGlobal | kPcMatchMaking,
    kPiGlobalAutoParty = kScGlobal | kPcAutoParty,
    kPiGlobalGameDb = kScGlobal | kPcGameDb,
    kPiGlobalLogDb = kScGlobal | kPcLogDb,
    kPiGlobalSmsDb = kScGlobal | kPcSmsDb,
    kPiGlobalAccountDb = kScGlobal | kPcAccountDb,
    kPiGlobalEventDb = kScGlobal | kPcEventDb,

    kPiAccountDb = kPcAccountDb,
    kPiLogDb = kPcLogDb,
    kPiEventDb = kPcEventDb,
    kPiScriptDb = kPcScriptDb,

    kPiNull = 0xFFFFFFFFu,
};

constexpr std::uint32_t kNxGmToolTcpClass = 0x14;
constexpr std::uint32_t kChatLogClass = 0x15;

// Helpers mirroring GetPerformerClass / GetServerClass / CompareServerLevel.
constexpr std::uint32_t performerClass(std::uint32_t pi) { return pi & kPcMaskBit; }
constexpr std::uint32_t serverClass(std::uint32_t pi) { return pi & kScMaskBit; }
constexpr bool isSameServerGroup(std::uint32_t pi) { return (pi & kStSameServGroup) != 0; }

}  // namespace knc
