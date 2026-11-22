using System.Buffers.Binary;
using System.Net.Sockets;
using KncWX2Server.CenterServer;
using KncWX2Server.Core.Events;
using KncWX2Server.Core.Framework;
using KncWX2Server.Core.Packets;
using KncWX2Server.Core.Region;
using KncWX2Server.Core.Security;
using KncWX2Server.Core.Serialization;
using KncWX2Server.GameServer;
using KncWX2Server.GlobalServer;
using KncWX2Server.LoginServer;
using Xunit;

namespace KncWX2Server.Tests;

public sealed class Phase14DeepParityTests : IDisposable
{
    private readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"knc_phase14_{Guid.NewGuid():N}.db");

    [Fact]
    public void InventoryLocker_UserUnitManager_BuffManager_And_AgencyShopList_MatchCpp()
    {
        var locker = new KInventoryLocker();
        var t0 = DateTime.UtcNow;
        Assert.False(locker.IsLocked(t0));
        Assert.True(locker.Lock(KInventoryLocker.LockType.TimeOut, 10.0, t0));
        Assert.False(locker.Lock(KInventoryLocker.LockType.Infinite, 10.0, t0.AddSeconds(2)));
        Assert.True(locker.IsLocked(t0.AddSeconds(5)));
        Assert.False(locker.IsLocked(t0.AddSeconds(11))); // auto-unlocked after timeout

        var unitMgr = new KUserUnitManager();
        var initStatus = unitMgr.Init(null, 12000);
        Assert.Equal(12000, initStatus.MaxHp);
        Assert.Equal(300, initStatus.MaxMp);
        Assert.True(unitMgr.SetGamePlayStatusAndCheckChangedHpMp(
            new KUserUnitManager.KGamePlayStatus { MaxHp = 12000, CurHp = 6000, MaxMp = 300, CurMp = 150 },
            out var hpMp));
        Assert.Equal(50f, hpMp.HpPercent, 1);
        Assert.Equal(50f, hpMp.MpPercent, 1);

        unitMgr.ReserveReturnToFieldInfo(new KUserUnitManager.KReturnToFieldInfo(20001, 3, 10f, 20f, 30f));
        Assert.True(unitMgr.TryPopReservedReturnToFieldInfo(out var retPos));
        Assert.Equal(20001, retPos!.MapId);
        Assert.False(unitMgr.TryPopReservedReturnToFieldInfo(out _));

        var buffMgr = new KBuffManager();
        var templet = buffMgr.GetBuffTemplet((int)KBuffManager.BuffTempletId.WarmEnergyOfEl);
        Assert.NotNull(templet);
        var userBuffs = new KUserServerBuffManager();
        Assert.True(userBuffs.ActivateFromTemplet(templet!, expRate: 0.2f, nowUtc: t0));
        Assert.True(userBuffs.IsBuffActivated((int)KBuffManager.BuffTempletId.WarmEnergyOfEl));
        Assert.Single(userBuffs.CheckBuffPeriod(t0.AddSeconds(templet!.BuffPeriodSecond + 1)));

        var agency = new KPersonalShopAgencyManager();
        var shop = agency.CreateAgencyShop(9001, "Seller", "Agency", [
            new KPersonalShopAgencyManager.AgencyShopItem { ItemUid = 1, ItemId = 130001, Quantity = 2, PricePerUnit = 1000 },
            new KPersonalShopAgencyManager.AgencyShopItem { ItemUid = 2, ItemId = 130002, Quantity = 1, PricePerUnit = 2000 },
        ], TimeSpan.FromHours(12));
        Assert.NotNull(shop);
        Assert.True(agency.BuyFromAgencyShop(shop!.ShopUid, 2, 1, 10000).ok);
        Assert.Equal(2, agency.GetSellPersonalShopItemInfoList(9001, KPersonalShopAgencyManager.ItemListType.AllItems).Count);
        Assert.Single(agency.GetSellPersonalShopItemInfoList(9001, KPersonalShopAgencyManager.ItemListType.OnlyOnSaleItems));
        Assert.Single(agency.GetSellPersonalShopItemInfoList(9001, KPersonalShopAgencyManager.ItemListType.OnlySoldOutItems));

        var henil = new KHenirRankingManager();
        Assert.True(henil.CheckNewRecord(new KHenirRankingInfo
        {
            UnitUid = 777,
            NickName = "Ranker",
            UnitClass = 1,
            Level = 60,
            StageCount = 42,
            PlayTime = 380,
        }));

        var exMgr = new KItemExchangeManager();
        exMgr.AddExchangeEntry(101, 50001, 2, KItemExchangeManager.ItemExchangeType.All, 60001, 100f, 3);
        exMgr.AddExchangeEntry(101, 50001, 2, KItemExchangeManager.ItemExchangeType.All, 60002, 100f, 1, period: 7);
        exMgr.AddExchangeEntry(102, 50002, 1, KItemExchangeManager.ItemExchangeType.Select, 60010, 100f, 1);
        exMgr.AddExchangeEntry(103, 50003, 1, KItemExchangeManager.ItemExchangeType.Random, 60020, 100f, 2);

        var srcEquip = new KItemInfo { ItemId = 50002, EnchantLevel = 9, ItemSockets = [10010, 10020] };
        Assert.True(exMgr.TryGetExchangeResult(101, 50001, 2, null, 0, out var allRes, out var isRnd1, out _));
        Assert.False(isRnd1);
        Assert.Equal(2, allRes.Count);
        Assert.Equal(3, allRes[60001].Quantity);
        Assert.Equal((short)7, allRes[60002].Period);

        Assert.True(exMgr.TryGetExchangeResult(102, 50002, 1, srcEquip, 60010, out var selRes, out _, out _));
        Assert.Equal((sbyte)9, selRes[60010].EnchantLevel);
        Assert.Equal(2, selRes[60010].ItemSockets.Count);
        Assert.False(exMgr.TryGetExchangeResult(102, 50002, 1, srcEquip, 99999, out _, out _, out _));

        Assert.True(exMgr.TryGetExchangeResult(103, 50003, 1, null, 0, out var rndRes, out var isRnd2, out _));
        Assert.True(isRnd2);
        Assert.Equal(2, rndRes[60020].Quantity);
    }

    [Fact]
    public async Task GameServer_AttribEnchant_InventoryLock_And_PartyListPage12_WorkEndToEnd()
    {
        var router = new InProcessRouter();
        var center = new KCenterServer(RegionCode.TW, router);
        var global = new KGlobalServer(RegionCode.TW, router);
        var game = new KGameServer(RegionCode.TW, _dbPath, router, center, global);

        game.ItemExchangeManager.AddExchangeEntry(101, 130099, 2, KItemExchangeManager.ItemExchangeType.All, 777101, 100f, 3);
        game.ItemExchangeManager.AddExchangeEntry(101, 130099, 2, KItemExchangeManager.ItemExchangeType.All, 777102, 100f, 1);

        // Seed 14 public parties (13 normal, 1 secret dungeon) to verify PLE_PARTY_PAGE = 12 and PltSecretDungeon filter
        for (int i = 1; i <= 14; i++)
        {
            center.PartyManager.CreateParty(
                new KCreatePartyInfo
                {
                    DungeonId = i == 14 ? 34001 : 30000,
                    DungeonMode = (sbyte)(i == 14 ? 2 : 0),
                },
                new KPartyUserInfo { UnitUid = 8000 + i, NickName = $"Host{i}", Level = 50 });
        }

        Assert.True(center.Start(overridePort: 0));
        Assert.True(global.Start(overridePort: 0));
        Assert.True(game.Start(overridePort: 0, overrideTrPort: 0, overrideUdpEchoPort: 0, overrideUdpRelayPort: 0));

        try
        {
            using var gsClient = new TcpClient();
            await gsClient.ConnectAsync("127.0.0.1", game.ListenPort);
            using var gsStream = gsClient.GetStream();
            var gsSa = new KSADatabase();
            ushort gsSpi = await HandshakeAsync(gsStream, gsSa, RegionProfile.Taiwan);

            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_CONNECT_REQ, new KEGS_CONNECT_REQ { Version = "TW_2014_03" });
            Assert.Equal(EventId.EGS_CONNECT_ACK, (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).EventId);

            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_VERIFY_ACCOUNT_REQ, new KUserAuthenticateReq { UserId = "p14user", Password = "p14user" });
            Assert.Equal(EventId.EGS_VERIFY_ACCOUNT_ACK, (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).EventId);
            Assert.Equal(EventId.ENX_USER_LOGIN_NOT, (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).EventId);
            Assert.Equal(EventId.EGS_SECOND_SECURITY_INFO_NOT, (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).EventId);

            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_CREATE_UNIT_REQ, new KEGS_CREATE_UNIT_REQ { NickName = "P14Hero", UnitClass = 1 });
            long unitUid = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan))
                .GetPayload<KEGS_CREATE_UNIT_ACK>(RegionProfile.Taiwan).UnitInfo.UnitUid;
            var weaponSlot = game.Db.GrantUnitItem(unitUid, 110001, 1);

            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_SELECT_UNIT_REQ, new KEGS_SELECT_UNIT_REQ { UnitUid = unitUid });
            Assert.Equal(EventId.EGS_SELECT_UNIT_1_NOT, (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).EventId);
            Assert.Equal(EventId.EGS_SELECT_UNIT_2_NOT, (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).EventId);
            Assert.Equal(EventId.EGS_SELECT_UNIT_3_NOT, (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).EventId);
            Assert.Equal(EventId.EGS_SELECT_UNIT_4_NOT, (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).EventId);
            Assert.Equal(EventId.EGS_SELECT_UNIT_5_NOT, (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).EventId);
            Assert.Equal(EventId.EGS_SELECT_UNIT_ACK, (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).EventId);
            Assert.Equal(EventId.EGS_GET_MY_INVENTORY_ACK, (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).EventId);

            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_STATE_CHANGE_FIELD_REQ, new KEGS_STATE_CHANGE_FIELD_REQ { MapId = 20001 });
            Assert.Equal(EventId.EGS_STATE_CHANGE_FIELD_ACK, (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).EventId);
            Assert.Equal(EventId.EGS_CHANGE_USER_LIST_IN_SECTOR_NOT, (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).EventId);

            var gsUser = game.GetUserByUnitUid(unitUid);
            Assert.NotNull(gsUser);
            Assert.Equal(10000, gsUser!.UnitManager.GamePlayStatus.MaxHp);

            // 1. Missing El Shards fails with ERR_ATTRIB_ENCHANT_01; granting AtiUnknown allows AttribEnchant (id = 7 -> 1..6)
            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_ATTRIB_ENCHANT_ITEM_REQ, new KEGS_ATTRIB_ENCHANT_ITEM_REQ
                {
                    ItemUid = weaponSlot.ItemUid,
                    AttribEnchantSlotNo = 0,
                    AttribEnchantId = 7, // ET_RANDOM
                });
            var attribFail = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan))
                .GetPayload<KEGS_ATTRIB_ENCHANT_ITEM_ACK>(RegionProfile.Taiwan);
            Assert.Equal(NetError.ERR_ATTRIB_ENCHANT_01, attribFail.Ok);

            game.Db.GrantUnitItem(unitUid, KSocketItemManager.AtiUnknown, 10);
            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_ATTRIB_ENCHANT_ITEM_REQ, new KEGS_ATTRIB_ENCHANT_ITEM_REQ
                {
                    ItemUid = weaponSlot.ItemUid,
                    AttribEnchantSlotNo = 0,
                    AttribEnchantId = 7, // ET_RANDOM
                });
            var attribAck = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan))
                .GetPayload<KEGS_ATTRIB_ENCHANT_ITEM_ACK>(RegionProfile.Taiwan);
            Assert.Equal(NetError.NET_OK, attribAck.Ok);
            Assert.InRange(attribAck.AttribEnchantId, (sbyte)1, (sbyte)6);

            // 2. PartyList uses PLE_PARTY_PAGE = 12 and PltSecretDungeon filtering
            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_PARTY_LIST_REQ, new KEGS_PARTY_LIST_REQ
                {
                    ListType = KEGS_PARTY_LIST_REQ.PltGateTotal,
                    ViewPage = 1,
                });
            var partyPage1 = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan))
                .GetPayload<KEGS_PARTY_LIST_ACK>(RegionProfile.Taiwan);
            Assert.Equal(NetError.NET_OK, partyPage1.Ok);
            Assert.Equal(2u, partyPage1.TotalPage);
            Assert.Equal(12, partyPage1.PartyInfo.Count);

            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_PARTY_LIST_REQ, new KEGS_PARTY_LIST_REQ
                {
                    ListType = KEGS_PARTY_LIST_REQ.PltSecretDungeon,
                    ViewPage = 1,
                });
            var secretParties = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan))
                .GetPayload<KEGS_PARTY_LIST_ACK>(RegionProfile.Taiwan);
            Assert.Single(secretParties.PartyInfo);
            Assert.Equal(34001, secretParties.PartyInfo[0].PartyInfo.DungeonId);

            // 3. InventoryLocker blocks concurrent EGS_BUY_UI_SETTING_REQ
            Assert.True(gsUser.InventoryLocker.Lock(KInventoryLocker.LockType.Infinite));
            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_BUY_UI_SETTING_REQ, new KEGS_BUY_UI_SETTING_REQ { TimeControlItemType = 1 });
            var lockedAck = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan))
                .GetPayload<KEGS_BUY_UI_SETTING_ACK>(RegionProfile.Taiwan);
            Assert.Equal(NetError.ERR_INVENTORY_LOCK_00, lockedAck.Ok);
            gsUser.InventoryLocker.Unlock();

            // 4. ItemExchangeManager multi-item IET_ALL exchange + UnitManager.EnterCashShop sync
            var ticketSlot = game.Db.GrantUnitItem(unitUid, 130099, 2);
            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_ITEM_EXCHANGE_REQ, new KEGS_ITEM_EXCHANGE_REQ
                {
                    HouseId = 101,
                    SourceItemUid = ticketSlot.ItemUid,
                    SourceItemId = 130099,
                    SourceQuantity = 2,
                });
            var exAck = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan))
                .GetPayload<KEGS_ITEM_EXCHANGE_ACK>(RegionProfile.Taiwan);
            Assert.Equal(NetError.NET_OK, exAck.Ok);
            Assert.Equal(3, exAck.ResultItem[777101]);
            Assert.Equal(1, exAck.ResultItem[777102]);

            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_VISIT_CASH_SHOP_NOT, new KEGS_VISIT_CASH_SHOP_NOT { EnterCashShop = true, UnitUid = unitUid });
            await Task.Delay(50);
            Assert.True(gsUser.UnitManager.EnterCashShop);

            // 5. Socket expand persists via ItemSockets into SQLite
            var matSlot = game.Db.GrantUnitItem(unitUid, 130080, 1);
            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_SOCKET_EXPAND_ITEM_REQ, new KEGS_SOCKET_EXPAND_ITEM_REQ
                {
                    TargetItemUid = weaponSlot.ItemUid,
                    MaterialItemUid = matSlot.ItemUid,
                });
            var sockExpAck = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan))
                .GetPayload<KEGS_SOCKET_EXPAND_ITEM_ACK>(RegionProfile.Taiwan);
            Assert.Equal(NetError.NET_OK, sockExpAck.Ok);
            Assert.NotEmpty(sockExpAck.UpdatedInventorySlot[0].ItemInfo.ItemSockets);
        }
        finally
        {
            game.ShutDown();
            global.ShutDown();
            center.ShutDown();
        }
    }

    private static async Task<ushort> HandshakeAsync(NetworkStream stream, KSADatabase saDb, RegionProfile region)
    {
        var acceptEv = await ReadEventAsync(stream, saDb, region);
        Assert.Equal(EventId.E_ACCEPT_CONNECTION_NOT, acceptEv.EventId);
        var ks = new KSerializer(region);
        ks.BeginReading(acceptEv.Buffer);
        Assert.True(ks.Get(out ushort newSpi));
        var newSa = new KSecurityAssociation();
        Assert.True(newSa.Get(ks, region));
        ks.EndReading();
        saDb.Insert(newSpi, newSa);
        return newSpi;
    }

    private static async Task SendPacketAsync(NetworkStream stream, KSADatabase saDb, ushort spi, RegionProfile region, uint destPi, EventId id, IKSerializable packet)
    {
        var ev = new KEvent();
        ev.DestPerformer.PerformerId = destPi;
        ev.EventId = id;
        var ks = new KSerializer(region);
        ks.BeginWriting(ev.Buffer);
        packet.Put(ks, region);
        ks.EndWriting();
        var wire = new KSerBuffer();
        ks.BeginWriting(wire);
        ev.Put(ks, region);
        ks.EndWriting();
        var sec = new KSecureBuffer(saDb, spi);
        Assert.True(sec.Create(wire.AsSpan()));
        ushort totalLen = (ushort)(sec.WireBytes.Length + 2);
        byte[] frame = new byte[totalLen];
        BinaryPrimitives.WriteUInt16LittleEndian(frame.AsSpan(0, 2), totalLen);
        sec.WireBytes.CopyTo(frame.AsSpan(2));
        await stream.WriteAsync(frame);
    }

    private static async Task<KEvent> ReadEventAsync(NetworkStream stream, KSADatabase saDb, RegionProfile region)
    {
        byte[] lenBuf = new byte[2];
        int o = 0;
        while (o < 2)
        {
            int n = await stream.ReadAsync(lenBuf.AsMemory(o, 2 - o));
            if (n == 0) throw new EndOfStreamException();
            o += n;
        }
        ushort totalLen = BinaryPrimitives.ReadUInt16LittleEndian(lenBuf);
        byte[] payload = new byte[totalLen - 2];
        o = 0;
        while (o < payload.Length)
        {
            int n = await stream.ReadAsync(payload.AsMemory(o, payload.Length - o));
            if (n == 0) throw new EndOfStreamException();
            o += n;
        }
        var sec = new KSecureBuffer(saDb, payload);
        Assert.True(sec.TryGetPayload(out byte[] plain));
        var ev = new KEvent();
        var ks = new KSerializer(region);
        ks.BeginReading(new KSerBuffer(plain));
        Assert.True(ev.Get(ks, region));
        ks.EndReading();
        ev.Buffer.UnCompress();
        return ev;
    }

    public void Dispose()
    {
        try { if (File.Exists(_dbPath)) File.Delete(_dbPath); } catch { }
    }
}
