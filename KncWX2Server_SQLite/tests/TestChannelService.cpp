// tests/TestChannelService.cpp - packet level tests of the character flow, no sockets.
//
// Everything the 2014 ChannelServer did after the redirect: server set list, channel list, the
// character list, create/delete/select and the inventory read. Driving ChannelService directly
// makes it cheap to cover each reply code, which is what a client actually branches on.
#include <cstdio>
#include <memory>
#include <string>
#include <vector>

#include "TestFramework.h"
#include "kcore/Log.h"
#include "kcore/Serialize.h"
#include "kdb/Schema.h"
#include "kproto/Event.h"
#include "kproto/LoginPackets.h"
#include "kproto/UnitPackets.h"
#include "kserver/ChannelService.h"
#include "kserver/Config.h"

using namespace knc;
using namespace knc::server;

namespace {

class RecordingResponder : public LoginResponder {
public:
    explicit RecordingResponder(std::string ip = "10.0.0.8") : ip_(std::move(ip)) {}

    bool send(EventId id, const SerBuffer& payload) override {
        Sent sent;
        sent.id = id;
        sent.payload = payload;
        sent_.push_back(std::move(sent));
        return true;
    }
    void disconnect(const std::string& reason) override { disconnectReason = reason; }
    const std::string& peerIp() const override { return ip_; }
    int connectedUsers() const override { return 1; }

    struct Sent {
        EventId id = 0;
        SerBuffer payload;
    };

    template <class T>
    bool at(std::size_t index, T& out) const {
        if (index >= sent_.size()) return false;
        SerBuffer copy = sent_[index].payload;
        return decodePayload(copy, out);
    }

    template <class T>
    bool last(T& out) const { return at(sent_.empty() ? 0 : sent_.size() - 1, out); }

    EventId lastId() const { return sent_.empty() ? EventId(0) : sent_.back().id; }
    std::size_t count() const { return sent_.size(); }
    void clear() { sent_.clear(); }

    std::string disconnectReason;

private:
    std::string ip_;
    std::vector<Sent> sent_;
};

struct Fixture {
    Fixture() {
        Logger::instance().setLevel(LogLevel::kError);
        db::Database::Config config;
        config.memory = true;
        std::string error;
        if (!database.open(config, &error)) std::fprintf(stderr, "open: %s\n", error.c_str());
        if (!db::applySchema(database).ok) std::fprintf(stderr, "schema failed\n");
        if (!db::seedDefaultAccounts(database, &error)) std::fprintf(stderr, "seed: %s\n", error.c_str());
        ownedRepos = db::makeRepositories(database);
        serverConfig = Config::defaults();

        // The login handshake already happened: the session is authenticated as `test`.
        session.authenticated = true;
        session.state = LoginState::kAuthenticated;
        if (repos().accounts.authenticate("test", "test", "10.0.0.8", &session.account) !=
            db::AuthOutcome::kOk) {
            std::fprintf(stderr, "fixture: could not authenticate the seeded account\n");
        }
        service = std::make_unique<ChannelService>(*ownedRepos, serverConfig);
    }

    db::Repositories& repos() { return *ownedRepos; }

    Event makeEvent(EventId id) const {
        Event event;
        event.setData(kPiGsProxy, nullptr, id);
        return event;
    }

    template <class T>
    Event eventWith(EventId id, const T& payload) const {
        Event event = makeEvent(id);
        encodePayload(payload, event.payload);
        return event;
    }

    // Creates a character the way a client does and returns its uid (0 on failure).
    UidType createUnit(RecordingResponder& responder, const std::string& nick, int unitClass = 1) {
        CreateUnitReq request;
        request.nickName = toWide(nick);
        request.unitClass = unitClass;
        Event event = eventWith(EGS_CREATE_UNIT_REQ, request);
        service->handle(session, responder, event);
        CreateUnitAck ack;
        if (!responder.last(ack) || ack.ok != kNetOk) return 0;
        return ack.unitInfo.unitUid;
    }

    int lastOk(RecordingResponder& responder) {
        if (responder.lastId() == EGS_CREATE_UNIT_ACK) {
            CreateUnitAck ack;
            if (responder.last(ack)) return ack.ok;
        }
        return -1;
    }

    db::Database database;
    std::unique_ptr<db::Repositories> ownedRepos;
    Config serverConfig;
    std::unique_ptr<ChannelService> service;
    AccountSession session;
};

}  // namespace

// --------------------------------------------------------------------------- server set / channel
KTEST(Channel, serverSetListDescribesTheConfiguredSets) {
    Fixture f;
    RecordingResponder responder;
    Event event = f.makeEvent(EGS_GET_SERVER_SET_DATA_REQ);
    KCHECK(f.service->handle(f.session, responder, event));

    KCHECK_EQ(responder.lastId(), EGS_GET_SERVER_SET_DATA_ACK);
    GetServerSetDataAck ack;
    KCHECK(responder.last(ack));
    KCHECK_EQ(ack.ok, static_cast<int>(kNetOk));
    KCHECK_EQ(ack.serverSetList.size(), f.serverConfig.serverSets.size());
    KCHECK_EQ(ack.serverSetList.front().serverSetId, f.serverConfig.serverSets.front().id);
    KCHECK_EQ(ack.serverSetList.front().serverSetName, f.serverConfig.serverSets.front().name);
    // An empty server reports the lowest bucket, which the client renders as "smooth".
    KCHECK_EQ(static_cast<int>(ack.serverSetList.front().userCountLevel),
              static_cast<int>(ServerSetData::kUclFree));
}

KTEST(Channel, selectingAKnownServerSetReturnsItsAddress) {
    Fixture f;
    RecordingResponder responder;
    SelectServerSetReq request;
    request.serverSetId = f.serverConfig.serverSets.front().id;
    Event event = f.eventWith(EGS_SELECT_SERVER_SET_REQ, request);
    KCHECK(f.service->handle(f.session, responder, event));

    SelectServerSetAck ack;
    KCHECK(responder.last(ack));
    KCHECK_EQ(ack.ok, static_cast<int>(kNetOk));
    KCHECK_EQ(ack.ip, toWide(f.serverConfig.serverSets.front().ip));
    KCHECK_EQ(ack.port, f.serverConfig.serverSets.front().port);
    KCHECK_EQ(f.session.serverSetId, f.serverConfig.serverSets.front().id);
}

// GSUserSession.cpp:2166 never looks at m_ServerSetID - the comment there says the deployment has
// exactly one server set. Sending it a set id that does not exist is therefore not an error: the
// original still answers with its own address, so a client cannot be stranded by a stale id.
KTEST(Channel, selectServerSetIgnoresTheRequestedIdLikeThe2014Handler) {
    Fixture f;
    RecordingResponder responder;
    SelectServerSetReq request;
    request.serverSetId = 4242;
    Event event = f.eventWith(EGS_SELECT_SERVER_SET_REQ, request);
    f.service->handle(f.session, responder, event);

    SelectServerSetAck ack;
    KCHECK(responder.last(ack));
    KCHECK_EQ(ack.ok, static_cast<int>(kNetOk));
    KCHECK_EQ(ack.ip, toWide(f.serverConfig.serverSets.front().ip));
    KCHECK_EQ(ack.port, f.serverConfig.serverSets.front().port);
    KCHECK_EQ(f.session.serverSetId, f.serverConfig.serverSets.front().id);
}

// The block branch of the same handler: any punished level gets ERR_VERIFY_11 (15, because
// SERV_ACCOUNT_BLOCK_MESSAGE_RENEWAL is not defined), the block reason, and then a disconnect.
KTEST(Channel, aBlockedAccountIsToldWhyAndThenDisconnected) {
    Fixture f;
    RecordingResponder responder;
    f.session.account.authLevel = -3;  // SEnum::UAL_BLOCKED_3
    f.session.account.blockInfo.endTime = WideString(u"2014-04-01 00:00");
    f.session.account.blockInfo.blockReason = WideString(u"cheating");
    Event event = f.makeEvent(EGS_SELECT_SERVER_SET_REQ);
    const bool keepOpen = f.service->handle(f.session, responder, event);

    SelectServerSetAck ack;
    KCHECK(responder.last(ack));
    KCHECK_EQ(ack.ok, static_cast<int>(kErrVerifyBlocked));  // ERR_VERIFY_11 == 15
    KCHECK(ack.accountBlockInfo.endTime == WideString(u"2014-04-01 00:00"));
    KCHECK(ack.accountBlockInfo.blockReason == WideString(u"cheating"));
    KCHECK(responder.disconnectReason == "account blocked");
    KCHECK(!keepOpen);
}

// -1 is UAL_BLOCKED_1, which the 2014 enum documents as a warning that still plays.
KTEST(Channel, aWarningLevelIsNotABlock) {
    Fixture f;
    RecordingResponder responder;
    f.session.account.authLevel = -1;  // SEnum::UAL_BLOCKED_1
    Event event = f.makeEvent(EGS_SELECT_SERVER_SET_REQ);
    const bool keepOpen = f.service->handle(f.session, responder, event);

    SelectServerSetAck ack;
    KCHECK(responder.last(ack));
    KCHECK_EQ(ack.ok, static_cast<int>(kNetOk));
    KCHECK(responder.disconnectReason.empty());
    KCHECK(keepOpen);
}

// GSUserSession.cpp:2086 hard codes the name, so an unnamed configuration must still look like the
// original rather than shipping an empty string to the client.
KTEST(Channel, anUnnamedServerSetFallsBackToThe2014Literal) {
    Fixture f;
    RecordingResponder responder;
    // The service holds its own copy of the configuration, so it has to be built after the edit.
    f.serverConfig.serverSets.front().name.clear();
    f.service = std::make_unique<ChannelService>(f.repos(), f.serverConfig);
    Event event = f.makeEvent(EGS_GET_SERVER_SET_DATA_REQ);
    f.service->handle(f.session, responder, event);

    GetServerSetDataAck ack;
    KCHECK(responder.last(ack));
    KCHECK_EQ(ack.serverSetList.size(), std::size_t(1));
    KCHECK(ack.serverSetList.front().serverSetName == WideString(u"\uC194\uB808\uC2A4"));
}

// GSUserSession.cpp:2086 compares against 1500 and 3000, not against a percentage of the capacity.
KTEST(Channel, userCountLevelUsesThe2014AbsoluteThresholds) {
    KCHECK_EQ(static_cast<int>(userCountLevel(0, 3000)), static_cast<int>(ServerSetData::kUclFree));
    KCHECK_EQ(static_cast<int>(userCountLevel(1499, 3000)),
              static_cast<int>(ServerSetData::kUclFree));
    KCHECK_EQ(static_cast<int>(userCountLevel(1500, 3000)),
              static_cast<int>(ServerSetData::kUclNormal));
    KCHECK_EQ(static_cast<int>(userCountLevel(2999, 3000)),
              static_cast<int>(ServerSetData::kUclNormal));
    KCHECK_EQ(static_cast<int>(userCountLevel(3000, 4000)), static_cast<int>(ServerSetData::kUclBusy));
    KCHECK_EQ(static_cast<int>(userCountLevel(4000, 4000)), static_cast<int>(ServerSetData::kUclFull));
    // The capacity only separates BUSY from FULL; a tiny capacity does not make an idle server busy.
    KCHECK_EQ(static_cast<int>(userCountLevel(0, 10)), static_cast<int>(ServerSetData::kUclFree));
    KCHECK_EQ(static_cast<int>(userCountLevel(10, 10)), static_cast<int>(ServerSetData::kUclFree));
    // The original substitutes 1 for a nonsensical maximum rather than dividing by it.
    KCHECK_EQ(static_cast<int>(userCountLevel(0, 0)), static_cast<int>(ServerSetData::kUclFree));
}

KTEST(Channel, channelListCarriesEveryConfiguredChannel) {
    Fixture f;
    RecordingResponder responder;
    Event event = f.makeEvent(EGS_GET_CHANNEL_LIST_REQ);
    f.service->handle(f.session, responder, event);

    KCHECK_EQ(responder.lastId(), EGS_GET_CHANNEL_LIST_ACK);
    GetChannelListAck ack;
    KCHECK(responder.last(ack));
    KCHECK_EQ(ack.channelInfo.size(), f.serverConfig.channels.size());
    const ChannelConfig& first = f.serverConfig.channels.front();
    KCHECK_EQ(ack.channelInfo.front().channelId, first.channelId);
    KCHECK(ack.channelInfo.front().channelName == first.displayName);
    KCHECK_EQ(ack.channelInfo.front().masterPort, first.masterPort);
    KCHECK_EQ(ack.channelInfo.front().ncUdpPort, first.ncUdpPort);
    KCHECK_EQ(ack.channelInfo.front().maxUser, first.maxUser);
    KCHECK_EQ(ack.serverGroupId, f.serverConfig.serverSets.front().id);
    // Every channel also appears in the bonus map, which the client uses for exp/drop events.
    KCHECK_EQ(ack.channelBonusInfo.size(), f.serverConfig.channels.size());
}

// --------------------------------------------------------------------------- character list
KTEST(Channel, emptyAccountGetsAnEmptyListAndTheFullSlotCount) {
    Fixture f;
    RecordingResponder responder;
    Event event = f.makeEvent(EGS_MY_UNIT_AND_INVENTORY_INFO_LIST_REQ);
    f.service->handle(f.session, responder, event);

    KCHECK_EQ(responder.lastId(), EGS_MY_UNIT_AND_INVENTORY_INFO_LIST_ACK);
    MyUnitAndInventoryInfoListAck ack;
    KCHECK(responder.last(ack));
    KCHECK_EQ(ack.ok, static_cast<int>(kNetOk));
    KCHECK_EQ(ack.unitInfo.size(), std::size_t(0));
    KCHECK_EQ(ack.unitSlot, ChannelLimits::kMaxUnitsPerAccount);
    KCHECK(f.session.state == LoginState::kUnitList);
}

// --------------------------------------------------------------------------- create
KTEST(Channel, creatingAUnitPutsItInTheListWithAFreshInventory) {
    Fixture f;
    RecordingResponder responder;
    const UidType uid = f.createUnit(responder, "Rena", /*unitClass=*/2);
    KCHECK(uid > 0);

    CreateUnitAck ack;
    KCHECK(responder.last(ack));
    KCHECK_EQ(ack.unitInfo.nickName, WideString(u"Rena"));
    KCHECK_EQ(static_cast<int>(ack.unitInfo.unitClass), 2);
    KCHECK_EQ(ack.unitInfo.level, static_cast<std::uint8_t>(1));
    KCHECK_EQ(ack.unitInfo.ownerUserUid, f.session.account.uid);
    KCHECK_EQ(ack.skillSlot.size(), std::size_t(4));
    KCHECK_EQ(f.service->unitsCreated(), 1);

    // A new character owns the bags the 2014 client expects plus the slots the expansion grants.
    KCHECK_EQ(f.repos().inventory.getInventorySize(uid, 1),
              std::int64_t(ChannelLimits::kDefaultEquipmentSlots));
    KCHECK(f.repos().inventory.getInventorySize(uid, 2) > 0);

    // And it shows up in the list.
    responder.clear();
    Event list = f.makeEvent(EGS_MY_UNIT_AND_INVENTORY_INFO_LIST_REQ);
    f.service->handle(f.session, responder, list);
    MyUnitAndInventoryInfoListAck listAck;
    KCHECK(responder.last(listAck));
    KCHECK_EQ(listAck.unitInfo.size(), std::size_t(1));
    KCHECK_EQ(listAck.unitInfo.front().unitUid, uid);
}

KTEST(Channel, aTakenNicknameIsRefusedWithErrCreateUnit01) {
    Fixture f;
    RecordingResponder responder;
    KCHECK(f.createUnit(responder, "Aisha") > 0);

    CreateUnitReq request;
    request.nickName = toWide("Aisha");
    request.unitClass = 1;
    Event event = f.eventWith(EGS_CREATE_UNIT_REQ, request);
    f.service->handle(f.session, responder, event);

    KCHECK_EQ(f.lastOk(responder), static_cast<int>(kErrCreateUnitNameTaken));  // 26
    KCHECK_EQ(f.repos().units.countForAccount(f.session.account.uid), std::int64_t(1));
}

KTEST(Channel, nicknamesAreValidatedBeforeTheDatabaseIsTouched) {
    Fixture f;
    RecordingResponder responder;

    // One character is below the 2 character minimum.
    CreateUnitReq shortName;
    shortName.nickName = toWide("A");
    shortName.unitClass = 1;
    Event shortEvent = f.eventWith(EGS_CREATE_UNIT_REQ, shortName);
    f.service->handle(f.session, responder, shortEvent);
    const int shortOk = f.lastOk(responder);

    responder.clear();
    CreateUnitReq tooLong;
    tooLong.nickName = toWide("ThisNameIsWayTooLongFor2014");
    tooLong.unitClass = 1;
    Event longEvent = f.eventWith(EGS_CREATE_UNIT_REQ, tooLong);
    f.service->handle(f.session, responder, longEvent);
    const int longOk = f.lastOk(responder);

    KCHECK(shortOk != static_cast<int>(kNetOk));
    KCHECK(longOk != static_cast<int>(kNetOk));
    KCHECK_EQ(f.repos().units.countForAccount(f.session.account.uid), std::int64_t(0));
}

KTEST(Channel, theDailyCreationLimitIsEnforced) {
    Fixture f;
    RecordingResponder responder;
    for (int i = 0; i < ChannelLimits::kMaxUnitsCreatedPerDay; ++i) {
        KCHECK(f.createUnit(responder, "Unit" + std::to_string(i)) > 0);
    }

    CreateUnitReq request;
    request.nickName = toWide("OneMore");
    request.unitClass = 1;
    Event event = f.eventWith(EGS_CREATE_UNIT_REQ, request);
    f.service->handle(f.session, responder, event);

    KCHECK_EQ(f.lastOk(responder), static_cast<int>(kErrCreateUnitDailyLimit));  // 34
    KCHECK_EQ(f.repos().units.countForAccount(f.session.account.uid),
              std::int64_t(ChannelLimits::kMaxUnitsCreatedPerDay));
}

// --------------------------------------------------------------------------- select / delete
KTEST(Channel, selectingAUnitChangesTheSessionState) {
    Fixture f;
    RecordingResponder responder;
    const UidType uid = f.createUnit(responder, "Eve");
    KCHECK(uid > 0);

    SelectUnitReq request;
    request.unitUid = uid;
    Event event = f.eventWith(EGS_SELECT_UNIT_REQ, request);
    f.service->handle(f.session, responder, event);

    SelectUnitAck ack;
    KCHECK(responder.last(ack));
    KCHECK_EQ(ack.ok, static_cast<int>(kNetOk));
    KCHECK_EQ(f.session.unitUid, uid);
    KCHECK(f.session.state == LoginState::kInChannel);
}

KTEST(Channel, selectingSomebodyElsesUnitLooksLikeAMissingUnit) {
    Fixture f;
    RecordingResponder responder;
    // `gm` owns this one, `test` is the session.
    db::AccountRecord gm;
    KCHECK(f.repos().accounts.authenticate("gm", "gm", "10.0.0.8", &gm) == db::AuthOutcome::kOk);
    const db::CreateUnitResult created = f.repos().units.create(gm.uid, 0, "GmUnit", 1, 1);
    KCHECK(created.ok);

    SelectUnitReq request;
    request.unitUid = created.unitUid;
    Event event = f.eventWith(EGS_SELECT_UNIT_REQ, request);
    f.service->handle(f.session, responder, event);

    SelectUnitAck ack;
    KCHECK(responder.last(ack));
    KCHECK_EQ(ack.ok, static_cast<int>(kErrSelectUnitNotFound));  // 35
    KCHECK_EQ(f.session.unitUid, UidType(0));
}

KTEST(Channel, deletingAUnitFreesTheSlotButReservesTheName) {
    Fixture f;
    RecordingResponder responder;
    const UidType uid = f.createUnit(responder, "Chung");
    KCHECK(uid > 0);

    DeleteUnitReq request;
    request.unitUid = uid;
    Event event = f.eventWith(EGS_DELETE_UNIT_REQ, request);
    f.service->handle(f.session, responder, event);

    DeleteUnitAck ack;
    KCHECK(responder.last(ack));
    KCHECK_EQ(ack.ok, static_cast<int>(kNetOk));
    KCHECK_EQ(ack.unitUid, uid);
    KCHECK_EQ(ack.nickName, WideString(u"Chung"));
    KCHECK_EQ(f.service->unitsDeleted(), 1);

    // The name is reserved (SERV_UNIT_WAIT_DELETE), so a new character cannot take it straight away.
    KCHECK(!f.repos().units.isNickNameAvailable("Chung"));
    const int recreated = [&] {
        CreateUnitReq create;
        create.nickName = toWide("Chung");
        create.unitClass = 1;
        Event createEvent = f.eventWith(EGS_CREATE_UNIT_REQ, create);
        f.service->handle(f.session, responder, createEvent);
        return f.lastOk(responder);
    }();
    KCHECK(recreated != static_cast<int>(kNetOk));

    // But the slot is free again: another name goes into slot 0.
    KCHECK(f.createUnit(responder, "Ara") > 0);
    std::vector<db::UnitSummary> units;
    KCHECK(f.repos().units.listSummaries(f.session.account.uid, &units));
    KCHECK_EQ(units.size(), std::size_t(1));
    KCHECK_EQ(units.front().slot, 0);
}

KTEST(Channel, deletingAMissingUnitIsRefused) {
    Fixture f;
    RecordingResponder responder;
    DeleteUnitReq request;
    request.unitUid = 987654;
    Event event = f.eventWith(EGS_DELETE_UNIT_REQ, request);
    f.service->handle(f.session, responder, event);

    DeleteUnitAck ack;
    KCHECK(responder.last(ack));
    KCHECK_EQ(ack.ok, static_cast<int>(kErrSelectUnitNotFound));
    KCHECK_EQ(f.service->unitsDeleted(), 0);
}

// --------------------------------------------------------------------------- inventory
KTEST(Channel, inventoryNeedsASelectedUnit) {
    Fixture f;
    RecordingResponder responder;
    Event event = f.makeEvent(EGS_GET_MY_INVENTORY_REQ);
    f.service->handle(f.session, responder, event);

    GetMyInventoryAck ack;
    KCHECK(responder.last(ack));
    KCHECK_EQ(ack.ok, static_cast<int>(kErrClientState));
    KCHECK_EQ(responder.count(), std::size_t(1));  // no bag notices
}

// GSUserGameCommon.cpp:1158 sends every notice first and the ack last, so an empty inventory is a
// single frame: the ack. An empty bag produces no notice at all (KInventory::
// GetSelectUnitInventoryInfo skips it), which is why nothing else arrives here.
KTEST(Channel, anEmptyInventoryIsJustTheAckAndItComesLast) {
    Fixture f;
    RecordingResponder responder;
    const UidType uid = f.createUnit(responder, "Elsword");
    KCHECK(uid > 0);

    SelectUnitReq select;
    select.unitUid = uid;
    Event selectEvent = f.eventWith(EGS_SELECT_UNIT_REQ, select);
    f.service->handle(f.session, responder, selectEvent);

    responder.clear();
    Event event = f.makeEvent(EGS_GET_MY_INVENTORY_REQ);
    f.service->handle(f.session, responder, event);

    KCHECK_EQ(responder.count(), std::size_t(1));
    KCHECK_EQ(responder.lastId(), EGS_GET_MY_INVENTORY_ACK);
    GetMyInventoryAck ack;
    KCHECK(responder.last(ack));
    KCHECK_EQ(ack.ok, static_cast<int>(kNetOk));
    KCHECK(!ack.isRecommend);
}

KTEST(Channel, storedItemsAreReportedInTheRightBag) {
    Fixture f;
    RecordingResponder responder;
    const UidType uid = f.createUnit(responder, "Raven");
    KCHECK(uid > 0);

    std::int64_t itemUid = 0;
    KCHECK(f.repos().inventory.addItem(uid, /*itemCode=*/100001, /*category=*/1,
                                       /*slotIndex=*/3, /*quantity=*/1, &itemUid));

    SelectUnitReq select;
    select.unitUid = uid;
    Event selectEvent = f.eventWith(EGS_SELECT_UNIT_REQ, select);
    f.service->handle(f.session, responder, selectEvent);

    responder.clear();
    Event event = f.makeEvent(EGS_GET_MY_INVENTORY_REQ);
    f.service->handle(f.session, responder, event);

    // One non-empty bag, then the ack: two frames, notice first.
    KCHECK_EQ(responder.count(), std::size_t(2));
    SelectUnitInventoryInfoNot equipment;
    KCHECK(responder.at(0, equipment));
    KCHECK_EQ(responder.lastId(), EGS_GET_MY_INVENTORY_ACK);
    KCHECK_EQ(equipment.slotCategory, kStEquip);
    KCHECK_EQ(equipment.items.size(), std::size_t(1));
    const InventoryItemInfo& wire = equipment.items.begin()->second;
    KCHECK_EQ(wire.itemUid, itemUid);
    KCHECK_EQ(wire.itemInfo.itemId, 100001);
    KCHECK_EQ(static_cast<int>(wire.slotId), 3);
    KCHECK_EQ(wire.itemInfo.quantity, 1);
}

// KInventory::GetSelectUnitInventoryInfo walks ST_EQUIP..ST_END but skips ST_BANK (8) and
// ST_E_EQUIP (9): bank and worn equipment are not part of this packet.
KTEST(Channel, theBankAndWornEquipmentBagsAreNotSent) {
    Fixture f;
    RecordingResponder responder;
    const UidType uid = f.createUnit(responder, "Eve");
    KCHECK(uid > 0);
    for (const int category : {kStBank, kStEquipped}) {
        std::int64_t itemUid = 0;
        KCHECK(f.repos().inventory.addItem(uid, 100001, category, 0, 1, &itemUid));
    }

    SelectUnitReq select;
    select.unitUid = uid;
    Event selectEvent = f.eventWith(EGS_SELECT_UNIT_REQ, select);
    f.service->handle(f.session, responder, selectEvent);

    responder.clear();
    Event event = f.makeEvent(EGS_GET_MY_INVENTORY_REQ);
    f.service->handle(f.session, responder, event);

    // Nothing but the ack: both bags were skipped, not sent empty.
    KCHECK_EQ(responder.count(), std::size_t(1));
    KCHECK_EQ(responder.lastId(), EGS_GET_MY_INVENTORY_ACK);
}

// Only non-empty bags produce a notice, and they arrive in category order.
KTEST(Channel, noticesFollowCategoryOrderAndSkipEmptyBags) {
    Fixture f;
    RecordingResponder responder;
    const UidType uid = f.createUnit(responder, "Chung");
    KCHECK(uid > 0);
    // Deliberately out of order, with gaps.
    for (const int category : {kStQuest, kStEquip, kStMaterial}) {
        std::int64_t itemUid = 0;
        KCHECK(f.repos().inventory.addItem(uid, 100001, category, 0, 1, &itemUid));
    }

    SelectUnitReq select;
    select.unitUid = uid;
    Event selectEvent = f.eventWith(EGS_SELECT_UNIT_REQ, select);
    f.service->handle(f.session, responder, selectEvent);

    responder.clear();
    Event event = f.makeEvent(EGS_GET_MY_INVENTORY_REQ);
    f.service->handle(f.session, responder, event);

    KCHECK_EQ(responder.count(), std::size_t(4));  // three bags + ack
    const int expected[] = {kStEquip, kStMaterial, kStQuest};
    for (std::size_t i = 0; i < 3; ++i) {
        SelectUnitInventoryInfoNot notice;
        KCHECK(responder.at(i, notice));
        KCHECK_EQ(notice.slotCategory, expected[i]);
        KCHECK_EQ(notice.items.size(), std::size_t(1));
    }
    KCHECK_EQ(responder.lastId(), EGS_GET_MY_INVENTORY_ACK);
}

// "인벤정보는 한번만 준다!" - a second request for the same unit answers with the ack only, and
// selecting the unit again (KInventory::Clear) makes the bags available once more.
KTEST(Channel, inventoryBagsAreHandedOutOnlyOncePerUnitLoad) {
    Fixture f;
    RecordingResponder responder;
    const UidType uid = f.createUnit(responder, "Ara");
    KCHECK(uid > 0);
    std::int64_t itemUid = 0;
    KCHECK(f.repos().inventory.addItem(uid, 100001, kStEquip, 0, 1, &itemUid));

    SelectUnitReq select;
    select.unitUid = uid;
    Event selectEvent = f.eventWith(EGS_SELECT_UNIT_REQ, select);
    f.service->handle(f.session, responder, selectEvent);

    responder.clear();
    Event event = f.makeEvent(EGS_GET_MY_INVENTORY_REQ);
    f.service->handle(f.session, responder, event);
    KCHECK_EQ(responder.count(), std::size_t(2));  // bag + ack

    responder.clear();
    f.service->handle(f.session, responder, event);
    KCHECK_EQ(responder.count(), std::size_t(1));  // ack only
    KCHECK_EQ(responder.lastId(), EGS_GET_MY_INVENTORY_ACK);

    // Re-selecting the unit reloads the inventory, so the bag is sent again.
    f.service->handle(f.session, responder, selectEvent);
    responder.clear();
    f.service->handle(f.session, responder, event);
    KCHECK_EQ(responder.count(), std::size_t(2));
}

// KAccountInfo.m_bIsRecommend travels to the client and KEGS_GET_MY_INVENTORY_ACK carries it again;
// both read the account option, so a recommendable account looks the same on both screens.
KTEST(Channel, theRecommendAndShareBankFlagsComeFromTheAccount) {
    Fixture f;
    RecordingResponder responder;
    const UidType uid = f.createUnit(responder, "Rose");
    KCHECK(uid > 0);
    SelectUnitReq select;
    select.unitUid = uid;
    Event selectEvent = f.eventWith(EGS_SELECT_UNIT_REQ, select);
    f.service->handle(f.session, responder, selectEvent);

    responder.clear();
    Event event = f.makeEvent(EGS_GET_MY_INVENTORY_REQ);
    f.service->handle(f.session, responder, event);
    GetMyInventoryAck ack;
    KCHECK(responder.last(ack));
    KCHECK(!ack.isRecommend);  // a fresh account has nothing stored
    KCHECK(!ack.sharingBank);

    KCHECK(f.repos().accounts.setOption(f.session.account.uid, db::kOptIsRecommend, 1));
    KCHECK(f.repos().accounts.setOption(f.session.account.uid, db::kOptShareBank, 1));

    responder.clear();
    f.service->handle(f.session, responder, event);
    KCHECK(responder.last(ack));
    KCHECK(ack.isRecommend);
    KCHECK(ack.sharingBank);
}

// --------------------------------------------------------------------------- access control
KTEST(Channel, unauthenticatedRequestsGetNoReply) {
    Fixture f;
    f.session.authenticated = false;
    f.session.account = db::AccountRecord{};
    RecordingResponder responder;

    for (const EventId id : {EGS_GET_SERVER_SET_DATA_REQ, EGS_GET_CHANNEL_LIST_REQ,
                             EGS_MY_UNIT_AND_INVENTORY_INFO_LIST_REQ, EGS_GET_MY_INVENTORY_REQ}) {
        Event event = f.makeEvent(id);
        KCHECK(f.service->handle(f.session, responder, event));  // true: keep the connection
    }
    KCHECK_EQ(responder.count(), std::size_t(0));
}

KTEST(Channel, heartBeatIsIgnored) {
    Fixture f;
    RecordingResponder responder;
    Event beat = f.makeEvent(E_HEART_BEAT);
    KCHECK(f.service->handle(f.session, responder, beat));
    KCHECK_EQ(responder.count(), std::size_t(0));
}
