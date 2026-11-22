using System.Buffers.Binary;
using System.Net;
using System.Net.Sockets;
using KncWX2Server.Core.Events;
using KncWX2Server.Core.Framework;
using KncWX2Server.Core.Network;
using KncWX2Server.Core.Packets;
using KncWX2Server.Core.Region;
using KncWX2Server.Core.Serialization;
using KncWX2Server.GameServer;
using Xunit;

namespace KncWX2Server.Tests;

public sealed class RelayAndProtocolTests
{
    [Fact]
    public async Task UdpRelayServer_ConnectSetUidListAndTransparentXptRelay_ForwardsPayload()
    {
        await using var relay = new KUdpRelayServer();
        ushort port = relay.Start(0);
        var ep = new IPEndPoint(IPAddress.Loopback, port);

        using var peerA = new UdpClient(0);
        using var peerB = new UdpClient(0);

        // 1. Both peers register via SP_CONNECT_RELAY_REQ (with dwTimestamp)
        await peerA.SendAsync(BuildConnectRelay(1001, 1, 10), ep);
        await peerB.SendAsync(BuildConnectRelay(1002, 2, 20), ep);

        var ackA = await peerA.ReceiveAsync();
        var ackB = await peerB.ReceiveAsync();
        Assert.Equal((byte)KUdpRelayServer.SystemPacket.SP_CONNECT_RELAY_ACK, ackA.Buffer[0]);
        Assert.Equal(1001, BinaryPrimitives.ReadInt64LittleEndian(ackA.Buffer.AsSpan(1, 8)));
        Assert.Equal((byte)KUdpRelayServer.SystemPacket.SP_CONNECT_RELAY_ACK, ackB.Buffer[0]);
        Assert.Equal(1002, BinaryPrimitives.ReadInt64LittleEndian(ackB.Buffer.AsSpan(1, 8)));

        // 2. Peer A configures preconfig UID list [1002] + trailing XPT payload
        byte[] xptPayload1 = [0xAA, 0xBB, 0xCC, 0xDD, 0x01, 0x02];
        await peerA.SendAsync(BuildSetUidList(1001, 11, 110, [1002], xptPayload1), ep);

        var listAckA = await peerA.ReceiveAsync();
        Assert.Equal((byte)KUdpRelayServer.SystemPacket.SP_RELAY_SET_UID_LIST_ACK, listAckA.Buffer[0]);

        var recvB1 = await peerB.ReceiveAsync();
        Assert.Equal(xptPayload1, recvB1.Buffer);

        // 3. Peer A sends SP_RELAY_PRECONFIG with opaque XPT frame
        byte[] xptPayload2 = [0x10, 0x20, 0x30, 0x40];
        var preconfig = new byte[1 + 8 + xptPayload2.Length];
        preconfig[0] = (byte)KUdpRelayServer.SystemPacket.SP_RELAY_PRECONFIG;
        BinaryPrimitives.WriteInt64LittleEndian(preconfig.AsSpan(1, 8), 1001);
        xptPayload2.CopyTo(preconfig.AsSpan(9));
        await peerA.SendAsync(preconfig, ep);

        var recvB2 = await peerB.ReceiveAsync();
        Assert.Equal(xptPayload2, recvB2.Buffer);
        Assert.Equal(2, relay.RelayedPackets);
    }

    [Fact]
    public async Task UdpPortCheck_RespondsOnEchoAndRelayWithObservedEndpoint()
    {
        await using var echo = new KUdpEchoServer(new InProcessRouter());
        await using var relay = new KUdpRelayServer();
        ushort echoPort = echo.Start(0);
        ushort relayPort = relay.Start(0);
        using var sender = new UdpClient(new IPEndPoint(IPAddress.Loopback, 0));
        int localPort = ((IPEndPoint)sender.Client.LocalEndPoint!).Port;
        // XPT_PORT_CHECK_REQ: ID + packed UID + internal IPv4 + internal port.
        byte[] request = new byte[15];
        request[0] = 17;
        BinaryPrimitives.WriteInt64LittleEndian(request.AsSpan(1, 8), 987654);
        request[9] = 127; request[12] = 1;
        BinaryPrimitives.WriteUInt16LittleEndian(request.AsSpan(13, 2), (ushort)localPort);
        foreach (ushort port in new[] { echoPort, relayPort })
        {
            await sender.SendAsync(request, new IPEndPoint(IPAddress.Loopback, port));
            var ack = await sender.ReceiveAsync().WaitAsync(TimeSpan.FromSeconds(5));
            Assert.Equal(port, ack.RemoteEndPoint.Port);
            Assert.Equal(9, ack.Buffer.Length);
            Assert.Equal((byte)18, ack.Buffer[0]);
            Assert.Equal(new byte[] { 127, 0, 0, 1 }, ack.Buffer[1..5]);
            Assert.Equal(localPort, BinaryPrimitives.ReadInt32LittleEndian(ack.Buffer.AsSpan(5, 4)));
        }
    }

    [Fact]
    public async Task UdpEchoServer_ReceivesKUdpToServer_EmitsObservedPortEvent()
    {
        var router = new InProcessRouter();
        KEvent? captured = null;
        var tcs = new TaskCompletionSource<KEvent>();
        router.RegisterRoute(PerformerId.PI_GS_PROXY, ev =>
        {
            captured = ev;
            tcs.TrySetResult(ev);
            return true;
        });

        await using var echo = new KUdpEchoServer(router, PerformerId.PI_GS_SERVER);
        ushort port = echo.Start(0);

        using var sender = new UdpClient(0);
        ushort localPort = (ushort)((IPEndPoint)sender.Client.LocalEndPoint!).Port;
        byte[] packet = new byte[10];
        BinaryPrimitives.WriteInt64LittleEndian(packet.AsSpan(0, 8), 987654);
        BinaryPrimitives.WriteUInt16LittleEndian(packet.AsSpan(8, 2), localPort);

        await sender.SendAsync(packet, new IPEndPoint(IPAddress.Loopback, port));
        var ev = await tcs.Task.WaitAsync(TimeSpan.FromSeconds(5));
        Assert.NotNull(captured);
        Assert.Equal(EventId.E_UDP_PORT_NOT, ev.EventId);
        Assert.Contains(987654, ev.DestPerformer.Uids);

        var ks = new KSerializer(RegionProfile.Taiwan);
        ks.BeginReading(ev.Buffer);
        Assert.True(ks.Get(out ushort observedPort));
        ks.EndReading();
        Assert.Equal(localPort, observedPort);
    }

    [Fact]
    public async Task TRServer_RegUidAndTransparentRelay_ForwardsKEventBetweenPeers()
    {
        await using var tr = new KTRServer(RegionProfile.Taiwan);
        ushort port = tr.Start(0);

        using var c1 = new TcpClient();
        using var c2 = new TcpClient();
        await c1.ConnectAsync("127.0.0.1", port);
        await c2.ConnectAsync("127.0.0.1", port);

        await SendTrPacketAsync(c1.GetStream(), EventId.ETR_REG_UID, new KETR_REG_UID { UnitUid = 5001 }, 0);
        await SendTrPacketAsync(c2.GetStream(), EventId.ETR_REG_UID, new KETR_REG_UID { UnitUid = 5002 }, 0);
        await Task.Delay(100);

        await SendTrPacketAsync(c1.GetStream(), EventId.ETR_CHAT_NOT, new KEGS_CHAT_NOT
        {
            SenderUnitUid = 5001,
            SenderNickName = "Elsword",
            Message = "Hello TR!",
        }, 5002);

        var recv = await ReadTrEventAsync(c2.GetStream());
        Assert.Equal(EventId.EGS_CHAT_NOT, recv.EventId);
        var msg = recv.GetPayload<KEGS_CHAT_NOT>(RegionProfile.Taiwan);
        Assert.Equal("Hello TR!", msg.Message);
        Assert.Equal(5001, msg.SenderUnitUid);

        await SendTrPacketAsync(c1.GetStream(), EventId.ETR_PET_ACTION_NOT, new KEGS_PET_ACTION_NOT
        {
            UnitUid = 5001,
            ActionType = 3,
        }, 5002);
        var petRecv = await ReadTrEventAsync(c2.GetStream());
        Assert.Equal(EventId.EGS_PET_ACTION_NOT, petRecv.EventId);
        var petMsg = petRecv.GetPayload<KEGS_PET_ACTION_NOT>(RegionProfile.Taiwan);
        Assert.Equal(5001, petMsg.UnitUid);
        Assert.Equal((sbyte)3, petMsg.ActionType);
    }

    [Theory]
    [InlineData(RegionCode.TW)]
    [InlineData(RegionCode.HK)]
    [InlineData(RegionCode.CN)]
    [InlineData(RegionCode.JP)]
    public void GameplayPackets_RoundTripAcrossAllRegions(RegionCode code)
    {
        var region = RegionProfile.FromCode(code);
        var unit = new KUnitInfo
        {
            OwnerUserUid = 10,
            UnitUid = 2001,
            NickName = "AishaTest",
            UnitClass = 2,
            Level = 45,
            Ed = 99999,
            CheckPowerScore = 7,
            LastDate = "2026-11-20",
            AccountPvpLoseCount = 3,
        };
        unit.EquippedItem[1] = new KInventoryItemInfo
        {
            ItemUid = 888,
            SlotCategory = 1,
            SlotId = 0,
            ItemInfo = new KItemInfo { ItemId = 10200, Quantity = 1, GoldTicketKeyUid = 55 },
        };

        var buf = new KSerBuffer();
        var ks = new KSerializer(region);
        ks.BeginWriting(buf);
        Assert.True(unit.Put(ks, region));
        ks.EndWriting();

        var parsed = new KUnitInfo();
        ks.BeginReading(buf);
        Assert.True(parsed.Get(ks, region));
        ks.EndReading();

        Assert.Equal("AishaTest", parsed.NickName);
        Assert.Equal((byte)45, parsed.Level);
        Assert.Equal(99999, parsed.Ed);
        Assert.Single(parsed.EquippedItem);
    }

    private static byte[] BuildConnectRelay(long uid, uint stamp, uint ts)
    {
        byte[] buf = new byte[17];
        buf[0] = (byte)KUdpRelayServer.SystemPacket.SP_CONNECT_RELAY_REQ;
        BinaryPrimitives.WriteInt64LittleEndian(buf.AsSpan(1, 8), uid);
        BinaryPrimitives.WriteUInt32LittleEndian(buf.AsSpan(9, 4), stamp);
        BinaryPrimitives.WriteUInt32LittleEndian(buf.AsSpan(13, 4), ts);
        return buf;
    }

    private static byte[] BuildSetUidList(long uid, uint stamp, uint ts, long[] targets, byte[] payload)
    {
        byte[] buf = new byte[1 + 17 + targets.Length * 8 + payload.Length];
        buf[0] = (byte)KUdpRelayServer.SystemPacket.SP_RELAY_SET_UID_LIST_REQ;
        BinaryPrimitives.WriteInt64LittleEndian(buf.AsSpan(1, 8), uid);
        BinaryPrimitives.WriteUInt32LittleEndian(buf.AsSpan(9, 4), stamp);
        BinaryPrimitives.WriteUInt32LittleEndian(buf.AsSpan(13, 4), ts);
        buf[17] = (byte)targets.Length;
        for (int i = 0; i < targets.Length; i++)
            BinaryPrimitives.WriteInt64LittleEndian(buf.AsSpan(18 + i * 8, 8), targets[i]);
        payload.CopyTo(buf.AsSpan(18 + targets.Length * 8));
        return buf;
    }

    private static async Task SendTrPacketAsync(NetworkStream stream, EventId id, IKSerializable payload, long targetUid)
    {
        var ev = new KEvent { EventId = id };
        ev.DestPerformer.PerformerId = PerformerId.PI_TR_USER;
        if (targetUid > 0) ev.DestPerformer.AddUID(targetUid);
        var ks = new KSerializer(RegionProfile.Taiwan);
        ks.BeginWriting(ev.Buffer);
        payload.Put(ks, RegionProfile.Taiwan);
        ks.EndWriting();

        var wire = new KSerBuffer();
        ks.BeginWriting(wire);
        ev.Put(ks, RegionProfile.Taiwan);
        ks.EndWriting();

        ushort len = (ushort)(wire.Length + 2);
        byte[] frame = new byte[len];
        BinaryPrimitives.WriteUInt16LittleEndian(frame.AsSpan(0, 2), len);
        wire.AsSpan().CopyTo(frame.AsSpan(2));
        await stream.WriteAsync(frame);
    }

    private static async Task<KEvent> ReadTrEventAsync(NetworkStream stream)
    {
        byte[] lenBuf = new byte[2];
        int o = 0;
        while (o < 2) o += await stream.ReadAsync(lenBuf.AsMemory(o, 2 - o));
        ushort len = BinaryPrimitives.ReadUInt16LittleEndian(lenBuf);
        byte[] body = new byte[len - 2];
        o = 0;
        while (o < body.Length) o += await stream.ReadAsync(body.AsMemory(o, body.Length - o));

        var ev = new KEvent();
        var ks = new KSerializer(RegionProfile.Taiwan);
        ks.BeginReading(new KSerBuffer(body));
        Assert.True(ev.Get(ks, RegionProfile.Taiwan));
        ks.EndReading();
        return ev;
    }
}
