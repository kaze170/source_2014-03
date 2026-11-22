using System.Buffers.Binary;
using System.Net;
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

public sealed class FullServerSuiteEndToEndTests : IDisposable
{
    private readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"knc_suite_{Guid.NewGuid():N}.db");

    [Fact]
    public async Task FullSuite_LoginGameUnitFieldRoomShopMailGuildQuestMatchmaking_Succeeds()
    {
        var router = new InProcessRouter();
        var center = new KCenterServer(RegionCode.TW, router);
        var global = new KGlobalServer(RegionCode.TW, router);
        var login = new KLoginServer(RegionCode.TW, _dbPath, router);
        var game = new KGameServer(RegionCode.TW, _dbPath, router, center, global);

        Assert.True(center.Start(overridePort: 0));
        Assert.True(global.Start(overridePort: 0));
        Assert.True(login.Start(overridePort: 0));
        Assert.True(game.Start(overridePort: 0, overrideTrPort: 0, overrideUdpEchoPort: 0, overrideUdpRelayPort: 0));

        try
        {
            // 1. LoginServer TCP flow
            using (var lgClient = new TcpClient())
            {
                await lgClient.ConnectAsync("127.0.0.1", login.ListenPort);
                using var lgStream = lgClient.GetStream();
                var lgSa = new KSADatabase();
                ushort lgSpi = await HandshakeAsync(lgStream, lgSa, RegionProfile.Taiwan);

                await SendPacketAsync(lgStream, lgSa, lgSpi, RegionProfile.Taiwan, PerformerId.PI_LOGIN_USER,
                    EventId.EGS_VERIFY_ACCOUNT_REQ,
                    new KECH_VERIFY_ACCOUNT_REQ { Id = "test01", Password = "test01", Login = true });
                var verifyAckEv = await ReadEventAsync(lgStream, lgSa, RegionProfile.Taiwan);
                Assert.Equal(EventId.EGS_VERIFY_ACCOUNT_ACK, verifyAckEv.EventId);
                var verifyAck = verifyAckEv.GetPayload<KEGS_VERIFY_ACCOUNT_ACK>(RegionProfile.Taiwan);
                Assert.Equal(NetError.NET_OK, verifyAck.Ok);
            }

            // 2. GameServer full gameplay flow
            using var gsClient = new TcpClient();
            await gsClient.ConnectAsync("127.0.0.1", game.ListenPort);
            using var gsStream = gsClient.GetStream();
            var gsSa = new KSADatabase();
            ushort gsSpi = await HandshakeAsync(gsStream, gsSa, RegionProfile.Taiwan);

            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_CONNECT_REQ, new KEGS_CONNECT_REQ { Version = "TW_2014_03" });
            var connAck = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KEGS_CONNECT_ACK>(RegionProfile.Taiwan);
            Assert.Equal(NetError.NET_OK, connAck.Ok);
            Assert.Equal(game.UdpEchoPort, connAck.UdpPort);

            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_VERIFY_ACCOUNT_REQ, new KUserAuthenticateReq { UserId = "test01", Password = "test01" });
            var gsVerify = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KEGS_VERIFY_ACCOUNT_ACK>(RegionProfile.Taiwan);
            Assert.Equal(NetError.NET_OK, gsVerify.Ok);
            var loginNot = await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan);
            Assert.Equal(EventId.ENX_USER_LOGIN_NOT, loginNot.EventId);
            Assert.Equal(Convert.FromHexString("00FFFFFFFF000000000000000000000000000000000000000000000000"), loginNot.Buffer.ToArray());
            Assert.True(loginNot.TryReadPayload<KENX_USER_LOGIN_NOT>(RegionProfile.Taiwan, out var loginInfo));
            Assert.False(loginInfo.IsGameBang);
            Assert.Equal(-1, loginInfo.PcBangReward.PcBangType);
            Assert.Equal(EventId.EGS_SECOND_SECURITY_INFO_NOT, (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).EventId);

            // Follow the real client's packed UDP port check on the ACK's advertised port.
            using (var udp = new UdpClient(new IPEndPoint(System.Net.IPAddress.Loopback, 0)))
            {
                byte[] probe = new byte[15];
                probe[0] = 17;
                BinaryPrimitives.WriteInt64LittleEndian(probe.AsSpan(1, 8), gsVerify.AccountInfo.UserUid);
                probe[9] = 127; probe[12] = 1;
                int clientPort = ((System.Net.IPEndPoint)udp.Client.LocalEndPoint!).Port;
                BinaryPrimitives.WriteUInt16LittleEndian(probe.AsSpan(13, 2), (ushort)clientPort);
                await udp.SendAsync(probe, new System.Net.IPEndPoint(System.Net.IPAddress.Loopback, connAck.UdpPort));
                var reply = await udp.ReceiveAsync().WaitAsync(TimeSpan.FromSeconds(5));
                Assert.Equal((byte)18, reply.Buffer[0]);
                Assert.Equal(clientPort, BinaryPrimitives.ReadInt32LittleEndian(reply.Buffer.AsSpan(5, 4)));
            }

            // After ENX_USER_LOGIN_NOT the real client checks UDP and requests this state.
            await SendNoParamAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_STATE_CHANGE_SERVER_SELECT_REQ);
            var stateAck = await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan);
            Assert.Equal(EventId.EGS_STATE_CHANGE_SERVER_SELECT_ACK, stateAck.EventId);
            Assert.Equal(NetError.NET_OK, stateAck.GetPayload<KEGS_STATE_CHANGE_SERVER_SELECT_ACK>(RegionProfile.Taiwan).Ok);

            // The retail client sends an uninitialised server-set integer; C++ ignores it.
            foreach (int ignoredServerSet in new[] { 0, 0x001AF5A0, -1 })
            {
                await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                    EventId.EGS_SELECT_SERVER_SET_REQ, new KEGS_SELECT_SERVER_SET_REQ { ServerSetId = ignoredServerSet });
                var selectEvent = await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan).WaitAsync(TimeSpan.FromSeconds(5));
                Assert.Equal(EventId.EGS_SELECT_SERVER_SET_ACK, selectEvent.EventId);
                Assert.True(selectEvent.TryReadPayload<KEGS_SELECT_SERVER_SET_ACK>(RegionProfile.Taiwan, out var selectAck));
                Assert.Equal(NetError.NET_OK, selectAck.Ok);
                Assert.Equal(game.PrivateIp, selectAck.Ip);
                Assert.Equal(game.ListenPort, selectAck.Port);
            }
            await SendNoParamAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_CHARACTER_LIST_REQ);
            var characterListEvent = await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan).WaitAsync(TimeSpan.FromSeconds(5));
            Assert.Equal(EventId.EGS_CHARACTER_LIST_ACK, characterListEvent.EventId);
            Assert.True(characterListEvent.TryReadPayload<KEGS_CHARACTER_LIST_ACK>(RegionProfile.Taiwan, out var characterList));
            Assert.Equal(NetError.NET_OK, characterList.Ok);
            Assert.Equal("test01", characterList.UserId);

            // Create Unit
            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_CREATE_UNIT_REQ, new KEGS_CREATE_UNIT_REQ { NickName = "ElswordHero", UnitClass = 1 });
            var createUnitAck = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KEGS_CREATE_UNIT_ACK>(RegionProfile.Taiwan);
            Assert.Equal(NetError.NET_OK, createUnitAck.Ok);
            long unitUid = createUnitAck.UnitInfo.UnitUid;
            Assert.True(unitUid > 0);

            // List Units & Select Unit
            await SendNoParamAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_MY_UNIT_AND_INVENTORY_INFO_LIST_REQ);
            var listAck = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KEGS_MY_UNIT_AND_INVENTORY_INFO_LIST_ACK>(RegionProfile.Taiwan);
            Assert.Contains(listAck.Units, u => u.UnitUid == unitUid);

            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_SELECT_UNIT_REQ, new KEGS_SELECT_UNIT_REQ { UnitUid = unitUid });
            var selNot1 = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KEGS_SELECT_UNIT_1_NOT>(RegionProfile.Taiwan);
            Assert.Equal(NetError.NET_OK, selNot1.Ok);
            Assert.Equal(unitUid, selNot1.UnitInfo.UnitUid);
            var selNot2 = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KEGS_SELECT_UNIT_2_NOT>(RegionProfile.Taiwan);
            Assert.Equal(NetError.NET_OK, selNot2.Ok);
            var selNot3 = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KEGS_SELECT_UNIT_3_NOT>(RegionProfile.Taiwan);
            Assert.Equal(NetError.NET_OK, selNot3.Ok);
            var selNot4 = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KEGS_SELECT_UNIT_4_NOT>(RegionProfile.Taiwan);
            Assert.Equal(NetError.NET_OK, selNot4.Ok);
            var selNot5 = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KEGS_SELECT_UNIT_5_NOT>(RegionProfile.Taiwan);
            Assert.Equal(NetError.NET_OK, selNot5.Ok);
            var selAck = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KEGS_SELECT_UNIT_ACK>(RegionProfile.Taiwan);
            Assert.Equal(NetError.NET_OK, selAck.Ok);
            var invAck = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KEGS_GET_MY_INVENTORY_ACK>(RegionProfile.Taiwan);
            Assert.Equal(NetError.NET_OK, invAck.Ok);

            await SendNoParamAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_GET_MY_INVENTORY_REQ);
            var invItemsNot = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KEGS_SELECT_UNIT_INVENTORY_INFO_NOT>(RegionProfile.Taiwan);
            Assert.NotEmpty(invItemsNot.Items);
            var invReqAck = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KEGS_GET_MY_INVENTORY_ACK>(RegionProfile.Taiwan);
            Assert.Equal(NetError.NET_OK, invReqAck.Ok);

            // The original server rejects ordinary dungeons before checking room type.
            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_CREATE_TUTORIAL_ROOM_REQ, new KEGS_CREATE_TUTORIAL_ROOM_REQ
                {
                    RoomInfo = new KRoomInfo { RoomType = 1, DungeonId = 30000 },
                });
            var invalidTutorialAck = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KEGS_CREATE_TUTORIAL_ROOM_ACK>(RegionProfile.Taiwan);
            Assert.Equal(NetError.ERR_ROOM_30, invalidTutorialAck.Ok);

            // Enter Village Field & Chat
            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_STATE_CHANGE_FIELD_REQ, new KEGS_STATE_CHANGE_FIELD_REQ { MapId = 20001 });
            var fieldAck = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KEGS_STATE_CHANGE_FIELD_ACK>(RegionProfile.Taiwan);
            Assert.Equal(NetError.NET_OK, fieldAck.Ok);
            var sectorNot = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KEGS_CHANGE_USER_LIST_IN_SECTOR_NOT>(RegionProfile.Taiwan);
            Assert.Contains(sectorNot.NewUsers, u => u.UnitUid == unitUid);

            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_CHAT_REQ, new KEGS_CHAT_REQ { Message = "Hello Ruben Village!" });
            Assert.Equal(EventId.EGS_CHAT_ACK, (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).EventId);
            var chatNot = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KEGS_CHAT_NOT>(RegionProfile.Taiwan);
            Assert.Equal("Hello Ruben Village!", chatNot.Message);

            // Create Room -> Start -> Load 100% -> End Game
            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_CREATE_ROOM_REQ, new KEGS_CREATE_ROOM_REQ
                {
                    RoomInfo = new KRoomInfo { RoomName = "Ruben Forest", RoomType = 2, MaxSlot = 4, DungeonId = 30000 },
                });
            var createRoomAck = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KEGS_CREATE_ROOM_ACK>(RegionProfile.Taiwan);
            Assert.Equal(NetError.NET_OK, createRoomAck.Ok);
            Assert.Equal(game.UdpRelayPort, createRoomAck.RoomInfo.UdpRelayPort);

            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_STATE_CHANGE_GAME_START_REQ, new KEGS_STATE_CHANGE_GAME_START_REQ { WorldId = 1, DungeonId = 30000 });
            Assert.Equal(EventId.EGS_STATE_CHANGE_GAME_START_ACK, (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).EventId);
            Assert.Equal(EventId.EGS_STATE_CHANGE_GAME_START_NOT, (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).EventId);

            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_GAME_LOADING_REQ, new KEGS_GAME_LOADING_REQ { LoadingProgress = 100, UnitLevel = 1 });
            Assert.Equal(EventId.EGS_GAME_LOADING_ACK, (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).EventId);
            Assert.Equal(EventId.EGS_GAME_LOADING_NOT, (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).EventId);
            Assert.Equal(EventId.EGS_PLAY_START_NOT, (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).EventId);

            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_END_GAME_REQ, new KEGS_END_GAME_REQ { WinTeam = 0 });
            Assert.Equal(EventId.EGS_END_GAME_ACK, (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).EventId);
            Assert.Equal(EventId.EGS_END_GAME_NOT, (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).EventId);

            // Shop, Mail, Guild, Quest
            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_BUY_ED_ITEM_REQ, new KEGS_BUY_ED_ITEM_REQ { Items = new Dictionary<int, int> { [130002] = 5 } });
            var buyAck = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KEGS_BUY_ED_ITEM_ACK>(RegionProfile.Taiwan);
            Assert.Equal(NetError.NET_OK, buyAck.Ok);
            Assert.Single(buyAck.InventorySlotInfo);

            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_SEND_LETTER_REQ, new KEGS_SEND_LETTER_REQ { ToNickName = "ElswordHero", Title = "Gift", Message = "Hi!" });
            var sendMailAck = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KEGS_SEND_LETTER_ACK>(RegionProfile.Taiwan);
            Assert.Equal(NetError.NET_OK, sendMailAck.Ok);

            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_GET_POST_LETTER_LIST_REQ, new KEGS_GET_POST_LETTER_LIST_REQ { PostPageNo = 1 });
            var mailListAck = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KEGS_GET_POST_LETTER_LIST_ACK>(RegionProfile.Taiwan);
            Assert.Single(mailListAck.PostItems);

            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_CREATE_GUILD_REQ, new KEGS_CREATE_GUILD_REQ { GuildName = "ElSearchParty", GuildMessage = "For Elrios!" });
            var guildAck = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KEGS_CREATE_GUILD_ACK>(RegionProfile.Taiwan);
            Assert.Equal(NetError.NET_OK, guildAck.Ok);
            Assert.Equal("ElSearchParty", guildAck.GuildInfo.GuildName);

            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_NEW_QUEST_REQ, new KEGS_NEW_QUEST_REQ { QuestId = 1001, TalkNpcId = 100 });
            Assert.Equal(EventId.EGS_NEW_QUEST_ACK, (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).EventId);
            Assert.Equal(EventId.EGS_NEW_QUEST_NOT, (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).EventId);

            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_QUEST_COMPLETE_REQ, new KEGS_QUEST_COMPLETE_REQ { QuestId = 1001, TalkNpcId = 100 });
            var questAck = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KEGS_QUEST_COMPLETE_ACK>(RegionProfile.Taiwan);
            Assert.Equal(NetError.NET_OK, questAck.Ok);
            Assert.Equal(1001, questAck.CompleteQuestInfo.QuestId);
            Assert.Equal(2000, questAck.UpdateUnitInfo.UnitInfo.Ed);

            // GlobalServer MatchMaking & AutoParty
            global.MatchMakingManager.Enqueue(new MatchRequestEntry(1, "P1", 1000, 1, DateTime.UtcNow));
            global.MatchMakingManager.Enqueue(new MatchRequestEntry(2, "P2", 1020, 1, DateTime.UtcNow));
            Assert.True(global.MatchMakingManager.TryDequeueMatch(out var match));
            Assert.NotNull(match);
            Assert.Single(match.TeamA);
            Assert.Single(match.TeamB);

            global.AutoPartyManager.Enqueue(new AutoPartyEntry(1, "P1", 30000, 0, DateTime.UtcNow));
            global.AutoPartyManager.Enqueue(new AutoPartyEntry(2, "P2", 30000, 0, DateTime.UtcNow));
            Assert.True(global.AutoPartyManager.TryDequeueGroup(out var apGroup));
            Assert.NotNull(apGroup);
            Assert.Equal(2, apGroup.Members.Count);
        }
        finally
        {
            game.ShutDown();
            login.ShutDown();
            global.ShutDown();
            center.ShutDown();
        }
    }

    [Fact]
    public async Task GameLoginNotification_RejectsFailedLoginAndPreservesChannelChangePcBangReward()
    {
        var router = new InProcessRouter();
        var center = new KCenterServer(RegionCode.TW, router);
        var global = new KGlobalServer(RegionCode.TW, router);
        var game = new KGameServer(RegionCode.TW, _dbPath, router, center, global);
        Assert.True(center.Start(overridePort: 0));
        Assert.True(global.Start(overridePort: 0));
        Assert.True(game.Start(overridePort: 0, overrideTrPort: 0, overrideUdpEchoPort: 0, overrideUdpRelayPort: 0));
        try
        {
            var account = game.Db.VerifyOrRegisterAccount("TW", "test01", "test01");
            var (created, unit, _) = game.Db.CreateUnit(RegionProfile.Taiwan, account.UserUid, "ChangeTest", 1);
            Assert.True(created);
            Assert.NotNull(unit);
            game.GameSysValTable.RegisterPcBangProfile(new KGameSysValTable.PcBangRewardProfile
            {
                PcBangType = 7, ExpRate = 1, EdRate = 2, VpRate = 3, PetExpRate = 4,
                IsSpiritFree = true, IsSecretDungeonFree = true, AdditionalHenirRewardCount = 5,
                IsUsingPcBangAvatar = false, IsUsingPcBangTitle = true,
            });
            game.RegisterChannelChangeReservation(unit!.UnitUid, new KChannelChangeInfo { IsPcBang = true, PcBangType = 7 });
            using var client = new TcpClient();
            await client.ConnectAsync("127.0.0.1", game.ListenPort);
            using var stream = client.GetStream();
            var sa = new KSADatabase();
            ushort spi = await HandshakeAsync(stream, sa, RegionProfile.Taiwan);
            await SendPacketAsync(stream, sa, spi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_VERIFY_ACCOUNT_REQ, new KUserAuthenticateReq { UserId = "test01", Password = "incorrect" });
            var failure = await ReadEventAsync(stream, sa, RegionProfile.Taiwan).WaitAsync(TimeSpan.FromSeconds(5));
            Assert.Equal(EventId.EGS_VERIFY_ACCOUNT_ACK, failure.EventId);
            Assert.Equal(NetError.ERR_VERIFY_03, failure.GetPayload<KEGS_VERIFY_ACCOUNT_ACK>(RegionProfile.Taiwan).Ok);
            // A subsequent connect ACK must not be preceded by an unsolicited login notification.
            await SendPacketAsync(stream, sa, spi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_CONNECT_REQ, new KEGS_CONNECT_REQ { Version = "TW_2014_03" });
            Assert.Equal(EventId.EGS_CONNECT_ACK, (await ReadEventAsync(stream, sa, RegionProfile.Taiwan).WaitAsync(TimeSpan.FromSeconds(5))).EventId);
            await SendPacketAsync(stream, sa, spi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_CONNECT_CHANNEL_CHANGE_REQ, new KEGS_CONNECT_CHANNEL_CHANGE_REQ
                {
                    ConnectReq = new KEGS_CONNECT_REQ { Version = "TW_2014_03" },
                    VerifyAccountReq = new KUserAuthenticateReq { UserId = "test01", Password = "test01" },
                    SelectUnitReq = new KEGS_SELECT_UNIT_REQ { UnitUid = unit.UnitUid },
                });
            var changeAck = await ReadEventAsync(stream, sa, RegionProfile.Taiwan).WaitAsync(TimeSpan.FromSeconds(5));
            Assert.Equal(EventId.EGS_CONNECT_CHANNEL_CHANGE_ACK, changeAck.EventId);
            Assert.Equal(NetError.NET_OK, changeAck.GetPayload<KEGS_CONNECT_CHANNEL_CHANGE_ACK>(RegionProfile.Taiwan).Ok);
            var not = await ReadEventAsync(stream, sa, RegionProfile.Taiwan).WaitAsync(TimeSpan.FromSeconds(5));
            Assert.Equal(EventId.ENX_USER_LOGIN_NOT, not.EventId);
            Assert.True(not.TryReadPayload<KENX_USER_LOGIN_NOT>(RegionProfile.Taiwan, out var info));
            Assert.True(info.IsGameBang);
            Assert.Equal(7, info.PcBangReward.PcBangType);
            Assert.Equal(1f, info.PcBangReward.ExpRate);
            Assert.Equal(2f, info.PcBangReward.EdRate);
            Assert.Equal(3f, info.PcBangReward.VsPointRate);
            Assert.True(info.PcBangReward.SpiritFree);
            Assert.Equal(4f, info.PcBangReward.PetExpRate);
            Assert.Equal(5, info.PcBangReward.AdditionalHenirReward);
            Assert.True(info.PcBangReward.SecretDungeonFree);
            Assert.False(info.PcBangReward.Avatar);
            Assert.True(info.PcBangReward.Title);
        }
        finally
        {
            game.ShutDown();
            global.ShutDown();
            center.ShutDown();
        }
    }

    [Fact]
    public async Task ServerSelect_BlockedAuthLevelSendsErrorBeforeDisconnect()
    {
        var router = new InProcessRouter();
        var center = new KCenterServer(RegionCode.TW, router);
        var global = new KGlobalServer(RegionCode.TW, router);
        var game = new KGameServer(RegionCode.TW, _dbPath, router, center, global);
        Assert.True(center.Start(overridePort: 0));
        Assert.True(global.Start(overridePort: 0));
        Assert.True(game.Start(overridePort: 0, overrideTrPort: 0, overrideUdpEchoPort: 0, overrideUdpRelayPort: 0));
        try
        {
            game.Db.SetAccountAuthLevel(RegionProfile.Taiwan, "test01", -2);
            using var client = new TcpClient();
            await client.ConnectAsync("127.0.0.1", game.ListenPort);
            using var stream = client.GetStream();
            var sa = new KSADatabase();
            ushort spi = await HandshakeAsync(stream, sa, RegionProfile.Taiwan);
            await SendPacketAsync(stream, sa, spi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_VERIFY_ACCOUNT_REQ, new KUserAuthenticateReq { UserId = "test01", Password = "test01" });
            var verify = await ReadEventAsync(stream, sa, RegionProfile.Taiwan).WaitAsync(TimeSpan.FromSeconds(5));
            Assert.Equal(EventId.EGS_VERIFY_ACCOUNT_ACK, verify.EventId);
            Assert.Equal(NetError.NET_OK, verify.GetPayload<KEGS_VERIFY_ACCOUNT_ACK>(RegionProfile.Taiwan).Ok);
            Assert.Equal(EventId.ENX_USER_LOGIN_NOT, (await ReadEventAsync(stream, sa, RegionProfile.Taiwan).WaitAsync(TimeSpan.FromSeconds(5))).EventId);
            Assert.Equal(EventId.EGS_SECOND_SECURITY_INFO_NOT, (await ReadEventAsync(stream, sa, RegionProfile.Taiwan).WaitAsync(TimeSpan.FromSeconds(5))).EventId);
            await SendPacketAsync(stream, sa, spi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_SELECT_SERVER_SET_REQ, new KEGS_SELECT_SERVER_SET_REQ { ServerSetId = 0x001AF5A0 });
            var response = await ReadEventAsync(stream, sa, RegionProfile.Taiwan).WaitAsync(TimeSpan.FromSeconds(5));
            Assert.Equal(EventId.EGS_SELECT_SERVER_SET_ACK, response.EventId);
            Assert.True(response.TryReadPayload<KEGS_SELECT_SERVER_SET_ACK>(RegionProfile.Taiwan, out var ack));
            Assert.Equal(NetError.ERR_VERIFY_11, ack.Ok);
            byte[] oneByte = new byte[1];
            Assert.Equal(0, await stream.ReadAsync(oneByte.AsMemory()).AsTask().WaitAsync(TimeSpan.FromSeconds(5)));
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
