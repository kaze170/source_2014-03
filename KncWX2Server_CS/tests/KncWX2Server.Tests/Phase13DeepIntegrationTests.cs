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
using Xunit;

namespace KncWX2Server.Tests;

public sealed class Phase13DeepIntegrationTests : IDisposable
{
    private readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"knc_phase13_{Guid.NewGuid():N}.db");

    [Fact]
    public async Task GameServer_EndToEndHandlers_RouteThroughWiredManagers()
    {
        var router = new InProcessRouter();
        var center = new KCenterServer(RegionCode.TW, router);
        var global = new KGlobalServer(RegionCode.TW, router);
        var game = new KGameServer(RegionCode.TW, _dbPath, router, center, global);

        // Configure wired GameServer managers before running handlers
        for (int i = 1; i <= 7; i++)
        {
            game.TutorialManager.AddUnit(new KTutorialUnitInfo
            {
                UnitUid = 5000 + i,
                Level = (byte)(5 + (i % 4)),
                NickName = $"Pupil{i}",
                IsOnline = true,
            });
        }

        game.ManufactureItemManager.AddManufactureResultGroupWithRate(901, 777001, 100.0);
        game.ManufactureItemManager.RegisterManufacture(new KManufactureItemManager.ManufactureData
        {
            ManufactureId = 42,
            ResultGroupId = 901,
            Cost = 1234,
        });

        game.RandomItemManager.AddRandomItemGroup(8801, 777002, 100f, period: 0, quantity: 2);
        game.RandomItemManager.RegisterRandomItem(new KRandomItemManager.RandomItemData
        {
            ItemId = 165001,
            Condition = KRandomItemManager.UseCondition.OneUnit,
            ResurrectionCount = 2,
            RestoreSpirit = 15,
            UnitData = [new KRandomItemManager.RandomUnitData(1, 8801)],
        });

        game.SynthesisManager.AddAvatarSocketOptionForEquipPosition(KSynthesisManager.EquipWeaponHand, 310);
        game.SynthesisManager.AddSynthesisGroup(10, 777003, 100f); // UnitType=1, EquipWeaponHand=2 -> group 10

        game.EdInventoryExpand.SetInventoryGradeEd(1, 45_000);
        game.EdInventoryExpand.SetBankGradeEd(1, 85_000);

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

            game.Db.VerifyOrRegisterAccount("TW", "p13user", "p13user", autoCreateOnMissing: true);
            game.Db.SetAccountAuthLevel(RegionProfile.Taiwan, "p13user", 3);

            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_VERIFY_ACCOUNT_REQ, new KUserAuthenticateReq { UserId = "p13user", Password = "p13user" });
            var verifyAck = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KEGS_VERIFY_ACCOUNT_ACK>(RegionProfile.Taiwan);
            Assert.Equal(NetError.NET_OK, verifyAck.Ok);
            Assert.Equal(EventId.ENX_USER_LOGIN_NOT, (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).EventId);
            Assert.Equal(EventId.EGS_SECOND_SECURITY_INFO_NOT, (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).EventId);

            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_CREATE_UNIT_REQ, new KEGS_CREATE_UNIT_REQ { NickName = "P13Hero", UnitClass = 1 });
            var createUnitAck = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KEGS_CREATE_UNIT_ACK>(RegionProfile.Taiwan);
            long unitUid = createUnitAck.UnitInfo.UnitUid;
            game.Db.SetUnitLevel(unitUid, 25);
            game.Db.UpdateUnitProgress(unitUid, 0, 500_000, 0);
            var cubeSlot = game.Db.GrantUnitItem(unitUid, 165001, 1);
            var syn1 = game.Db.GrantUnitItem(unitUid, 91001, 1);
            var syn2 = game.Db.GrantUnitItem(unitUid, 91002, 1);
            var syn3 = game.Db.GrantUnitItem(unitUid, 91003, 1);
            var syn4 = game.Db.GrantUnitItem(unitUid, 91004, 1);
            var synKey = game.Db.GrantUnitItem(unitUid, 91005, 1);

            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_SELECT_UNIT_REQ, new KEGS_SELECT_UNIT_REQ { UnitUid = unitUid });
            Assert.Equal(EventId.EGS_SELECT_UNIT_1_NOT, (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).EventId);
            Assert.Equal(EventId.EGS_SELECT_UNIT_2_NOT, (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).EventId);
            Assert.Equal(EventId.EGS_SELECT_UNIT_3_NOT, (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).EventId);
            Assert.Equal(EventId.EGS_SELECT_UNIT_4_NOT, (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).EventId);
            Assert.Equal(EventId.EGS_SELECT_UNIT_5_NOT, (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).EventId);
            Assert.Equal(EventId.EGS_SELECT_UNIT_ACK, (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).EventId);
            Assert.Equal(EventId.EGS_GET_MY_INVENTORY_ACK, (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).EventId);

            // Enter Village Field
            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_STATE_CHANGE_FIELD_REQ, new KEGS_STATE_CHANGE_FIELD_REQ { MapId = 20001 });
            Assert.Equal(EventId.EGS_STATE_CHANGE_FIELD_ACK, (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).EventId);
            Assert.Equal(EventId.EGS_CHANGE_USER_LIST_IN_SECTOR_NOT, (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).EventId);

            var gsUser = game.GetUserByUnitUid(unitUid);
            Assert.NotNull(gsUser);
            gsUser!.SpiritManager.Init(50, 100);
            int initialEd = gsUser.ActiveUnit!.Ed;

            // 1. Tutorial student list uses TutorialManager (StudentPageNum = 5)
            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_TUTORIAL_STUDENT_LIST_REQ, new KEGS_TUTORIAL_STUDENT_LIST_REQ { ViewPage = 1 });
            var tutAck = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KEGS_TUTORIAL_STUDENT_LIST_ACK>(RegionProfile.Taiwan);
            Assert.Equal(NetError.NET_OK, tutAck.Ok);
            Assert.Equal(2, tutAck.TotalPage);
            Assert.Equal(5, tutAck.TutorialStudentList.Count);

            // 2. WishList synchronizes DB & KUserWishList (21-slot max enforcement)
            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_GET_WISH_LIST_REQ, new KPacketOK());
            Assert.Equal(EventId.EGS_GET_WISH_LIST_ACK, (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).EventId);

            var batch21 = Enumerable.Range(50001, 21).ToList();
            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_MODIFY_WISH_LIST_REQ, new KEGS_MODIFY_WISH_LIST_REQ { WishListItemId = batch21, IsDelete = false });
            var wishOk = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KEGS_MODIFY_WISH_LIST_ACK>(RegionProfile.Taiwan);
            Assert.Equal(NetError.NET_OK, wishOk.Ok);

            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_MODIFY_WISH_LIST_REQ, new KEGS_MODIFY_WISH_LIST_REQ { WishListItemId = [50099], IsDelete = false });
            var wishFull = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KEGS_MODIFY_WISH_LIST_ACK>(RegionProfile.Taiwan);
            Assert.Equal(NetError.ERR_WISH_LIST_02, wishFull.Ok);

            // 3. Item Manufacture uses ManufactureItemManager recipe cost & lottery
            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_ITEM_MANUFACTURE_REQ, new KEGS_ITEM_MANUFACTURE_REQ { ManufactureId = 42, Quantity = 2 });
            var mfgAck = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KEGS_ITEM_MANUFACTURE_ACK>(RegionProfile.Taiwan);
            Assert.Equal(NetError.NET_OK, mfgAck.Ok);
            Assert.Equal(initialEd - (1234 * 2), mfgAck.Ed);
            Assert.True(mfgAck.InsertedItem.ContainsKey(777001));

            // 4. Open Random Item uses RandomItemManager (rewards + resurrection + spirit restore)
            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_OPEN_RANDOM_ITEM_REQ, new KEGS_OPEN_RANDOM_ITEM_REQ { ItemUid = cubeSlot.ItemUid });
            var rndAck = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KEGS_OPEN_RANDOM_ITEM_ACK>(RegionProfile.Taiwan);
            Assert.Equal(NetError.NET_OK, rndAck.Ok);
            Assert.Equal(2, rndAck.RessurectionCount);
            Assert.Equal(15, rndAck.RestoreSpirit);
            Assert.Equal(2, rndAck.InsertedItem[777002]);
            Assert.Equal(65, gsUser.SpiritManager.Spirit);

            // 5. Synthesis socket group query & synthesis roll use SynthesisManager
            await SendNoParamAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_SYNTHESIS_SOCKET_GROUPID_REQ);
            var synGrpAck = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KEGS_SYNTHESIS_SOCKET_GROUPID_ACK>(RegionProfile.Taiwan);
            Assert.Equal(NetError.NET_OK, synGrpAck.Ok);
            Assert.Equal(310, synGrpAck.AvatarSocketOptionGroupId[KSynthesisManager.EquipWeaponHand]);

            var synReq = new KEGS_SYNTHESIS_ITEM_REQ { KeyData = synKey.ItemUid, SocketNo = 7 };
            synReq.SynthesisData[0] = syn1.ItemUid;
            synReq.SynthesisData[1] = syn2.ItemUid;
            synReq.SynthesisData[2] = syn3.ItemUid;
            synReq.SynthesisData[3] = syn4.ItemUid;
            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_SYNTHESIS_ITEM_REQ, synReq);
            var synAck = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KEGS_SYNTHESIS_ITEM_ACK>(RegionProfile.Taiwan);
            Assert.Equal(NetError.NET_OK, synAck.Ok);
            Assert.True(synAck.InsertedItem.ContainsKey(777003));

            // 6. ED inventory & bank expansion queries consult EdInventoryExpand
            await SendNoParamAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_GET_NEXT_INVENTORY_ED_REQ);
            var nextInvAck = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KEGS_GET_NEXT_BANK_ED_ACK>(RegionProfile.Taiwan);
            Assert.Equal(45_000, nextInvAck.Ed);

            await SendNoParamAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_GET_NEXT_BANK_ED_REQ);
            var nextBankAck = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KEGS_GET_NEXT_BANK_ED_ACK>(RegionProfile.Taiwan);
            Assert.Equal(85_000, nextBankAck.Ed);

            // 7. GM Cheat enforces CompanyIpTable when RestrictGmCheatByIp is active
            game.CompanyIpTable.RestrictGmCheatByIp = true;
            // Remove 127.0.0.1 behavior simulation: connect from 127.0.0.1 passes default allowlist
            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_ADMIN_CHEAT_GET_ALL_SKILL_REQ, new KEGS_ADMIN_CHEAT_GET_ALL_SKILL_REQ { ActiveSkillPageNumber = 1 });
            var cheatAck = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KEGS_ADMIN_CHEAT_GET_ALL_SKILL_ACK>(RegionProfile.Taiwan);
            Assert.Equal(NetError.NET_OK, cheatAck.Ok);

            // 8. Verify LogManager & ItemQuantityVariation recorded real activity
            string diag = gsUser.LogManager.FormatDiagnosticSnapshot(KUserLogManager.UserLogType.HeartBeat, verifyAck.AccountInfo.UserUid, unitUid);
            Assert.Contains("Send=[", diag);
            Assert.Contains("Proc=[", diag);
            var itemInc = gsUser.ItemQuantityVariation.GetIncreaseSnapshot();
            var itemDec = gsUser.ItemQuantityVariation.GetDecreaseSnapshot();
            Assert.True(itemInc[777001] >= 2);
            Assert.Equal(2, itemInc[777002]);
            Assert.Equal(1, itemDec[165001]);
        }
        finally
        {
            game.ShutDown();
            global.ShutDown();
            center.ShutDown();
        }
    }

    [Fact]
    public void CrossServerRouting_CenterAndGlobalServer_ProcessEvent_WiresRealManagers()
    {
        var router = new InProcessRouter();
        var center = new KCenterServer(RegionCode.TW, router);
        var global = new KGlobalServer(RegionCode.TW, router);

        Assert.True(center.Start(overridePort: 0));
        Assert.True(global.Start(overridePort: 0));

        try
        {
            // CenterServer party creation, training center room opening, and dungeon security reporting
            router.Send(PerformerId.PI_GS_SERVER, 1, PerformerId.PI_CN_SERVER, center.Uid,
                EventId.EPM_CREATE_PARTY_BY_INVITE_REQ, new KEPM_CREATE_PARTY_BY_INVITE_REQ
                {
                    CreatePartyInfo = new KCreatePartyInfo { DungeonId = 30000, PartyType = 0 },
                    HostInfo = new KPartyUserInfo { UnitUid = 701, NickName = "Leader" },
                });
            router.Send(PerformerId.PI_GS_SERVER, 1, PerformerId.PI_CN_SERVER, center.Uid,
                EventId.ERM_OPEN_TC_ROOM_REQ, new KERM_OPEN_TC_ROOM_REQ
                {
                    UnitUid = 701,
                    TcId = 2,
                    DungeonId = 30010,
                    PlayTimeLimit = 180f,
                });
            router.Send(PerformerId.PI_GS_SERVER, 1, PerformerId.PI_CN_SERVER, center.Uid,
                EventId.ERM_SERVER_CHECK_HACK_USER_NOT, new KERM_DUNGEON_SECURITY_REPORT_NOT
                {
                    RoomUid = 88,
                    DungeonId = 30000,
                    UnitUid = 701,
                    MaxSingleDamage = 12345,
                });

            center.Tick();
            Assert.Single(center.PartyManager.GetPartiesSnapshot());
            Assert.Single(center.TrainingCenterRooms);
            var tc = center.TrainingCenterRooms.Values.First();
            Assert.Equal(KTrainingCenterRoom.TcRoomState.Play, tc.State);
            Assert.Equal(30010, tc.DungeonId);

            // GlobalServer CCU & SerialNumber tracking
            router.Send(PerformerId.PI_GS_SERVER, 1, PerformerId.PI_GB_SERVER, global.Uid,
                EventId.EGB_UPDATE_CCU_INFO_NOT, new KEGB_UPDATE_CCU_INFO_NOT
                {
                    ServerUid = 1001,
                    ServerGroupId = 1,
                    ConcurrentUser = 345,
                    CurPartyCount = 22,
                });
            router.Send(PerformerId.PI_GS_SERVER, 1, PerformerId.PI_GB_SERVER, global.Uid,
                EventId.EGB_REGISTER_UNIT_NOT, new KEGB_REGISTER_UNIT_NOT
                {
                    ServerSn = 99887766,
                    UserUid = 500,
                    GameServerUid = 1001,
                });
            global.Tick();

            Assert.Equal(345, global.ServerCcuManager.GetTotalConcurrentUser());
            Assert.Equal(1, global.ServerSnManager.ActiveSerialCount);

            router.Send(PerformerId.PI_GS_SERVER, 1, PerformerId.PI_GB_SERVER, global.Uid,
                EventId.EGB_UNIT_DISCONNECT_NOT, new KEGB_UNIT_DISCONNECT_NOT
                {
                    ServerSn = 99887766,
                    UserUid = 500,
                });
            global.Tick();
            Assert.Equal(0, global.ServerSnManager.ActiveSerialCount);
        }
        finally
        {
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

    private static async Task SendNoParamAsync(NetworkStream stream, KSADatabase saDb, ushort spi, RegionProfile region, uint destPi, EventId id)
    {
        var ev = new KEvent();
        ev.DestPerformer.PerformerId = destPi;
        ev.EventId = id;
        var ks = new KSerializer(region);
        ks.BeginWriting(ev.Buffer);
        ks.Put((sbyte)0);
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
