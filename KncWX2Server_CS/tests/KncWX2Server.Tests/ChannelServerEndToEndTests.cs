using System.Buffers.Binary;
using System.Net.Sockets;
using KncWX2Server.ChannelServer;
using KncWX2Server.Core.Events;
using KncWX2Server.Core.Packets;
using KncWX2Server.Core.Region;
using KncWX2Server.Core.Security;
using KncWX2Server.Core.Serialization;
using Xunit;

namespace KncWX2Server.Tests;

public sealed class ChannelServerEndToEndTests : IDisposable
{
    private readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"knc_e2e_{Guid.NewGuid():N}.db");

    [Fact]
    public async Task TcpClient_HandshakeLoginAndChannelList_SucceedsAcrossRegions()
    {
        ushort port = (ushort)Random.Shared.Next(29100, 39100);
        var server = new KChannelServer(RegionCode.TW, _dbPath);
        server.ChannelSimLayer.AuthFlag = AuthFlag.GlobalService;
        Assert.True(server.Start(overridePort: port, serverGroupId: 0));

        try
        {
            using var client = new TcpClient();
            await client.ConnectAsync("127.0.0.1", port);
            using var stream = client.GetStream();

            var saDb = new KSADatabase();

            // 1. Read E_ACCEPT_CONNECTION_NOT encrypted with SPI=0
            var acceptEv = await ReadEventAsync(stream, saDb, RegionProfile.Taiwan);
            Assert.Equal(EventId.E_ACCEPT_CONNECTION_NOT, acceptEv.EventId);

            var ksAccept = new KSerializer(RegionProfile.Taiwan);
            ksAccept.BeginReading(acceptEv.Buffer);
            Assert.True(ksAccept.Get(out ushort newSpi));
            var newSa = new KSecurityAssociation();
            Assert.True(newSa.Get(ksAccept, RegionProfile.Taiwan));
            ksAccept.EndReading();
            saDb.Insert(newSpi, newSa);

            // 2. Send ECH_VERIFY_ACCOUNT_REQ for test01/test01
            await SendPacketAsync(stream, saDb, newSpi, RegionProfile.Taiwan, EventId.ECH_VERIFY_ACCOUNT_REQ,
                new KECH_VERIFY_ACCOUNT_REQ
                {
                    Id = "test01",
                    Password = "test01",
                    Login = true,
                });

            var verifyAckEv = await ReadEventAsync(stream, saDb, RegionProfile.Taiwan);
            Assert.Equal(EventId.ECH_VERIFY_ACCOUNT_ACK, verifyAckEv.EventId);
            var verifyAck = verifyAckEv.GetPayload<KECH_VERIFY_ACCOUNT_ACK>(RegionProfile.Taiwan);
            Assert.Equal(NetError.NET_OK, verifyAck.Ok);
            Assert.Equal("test01", verifyAck.UserId);
            Assert.NotEmpty(verifyAck.Passport);

            // 3. Request channel list
            await SendNoParamAsync(stream, saDb, newSpi, RegionProfile.Taiwan, EventId.ECH_GET_CHANNEL_LIST_REQ);
            var chAckEv = await ReadEventAsync(stream, saDb, RegionProfile.Taiwan);
            Assert.Equal(EventId.ECH_GET_CHANNEL_LIST_ACK, chAckEv.EventId);

            var chNotEv = await ReadEventAsync(stream, saDb, RegionProfile.Taiwan);
            Assert.Equal(EventId.ECH_GET_CHANNEL_LIST_NOT, chNotEv.EventId);
            var chNot = chNotEv.GetPayload<KECH_GET_CHANNEL_LIST_NOT>(RegionProfile.Taiwan);
            Assert.NotEmpty(chNot.ChannelList);

            // 4. Switch server region live to CN and JP
            server.SwitchRegion(RegionCode.CN);
            Assert.Equal(RegionCode.CN, server.Region.Region);
            server.SwitchRegion(RegionCode.JP);
            Assert.Equal(RegionCode.JP, server.Region.Region);
        }
        finally
        {
            server.ShutDown();
        }
    }

    private static async Task SendPacketAsync(NetworkStream stream, KSADatabase saDb, ushort spi, RegionProfile region, EventId id, IKSerializable packet)
    {
        var ev = new KEvent();
        ev.DestPerformer.PerformerId = PerformerId.PI_CHANNEL_USER;
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

    private static async Task SendNoParamAsync(NetworkStream stream, KSADatabase saDb, ushort spi, RegionProfile region, EventId id)
    {
        var ev = new KEvent();
        ev.DestPerformer.PerformerId = PerformerId.PI_CHANNEL_USER;
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
        await ReadExactAsync(stream, lenBuf);
        ushort totalLen = BinaryPrimitives.ReadUInt16LittleEndian(lenBuf);
        byte[] secData = new byte[totalLen - 2];
        await ReadExactAsync(stream, secData);

        var sec = new KSecureBuffer(saDb, secData);
        Assert.True(sec.TryGetPayload(out byte[] payload, authentic: true, checkSequenceNum: true));

        var ev = new KEvent();
        var ks = new KSerializer(region);
        ks.BeginReading(new KSerBuffer(payload));
        Assert.True(ev.Get(ks, region));
        ks.EndReading();
        if (ev.Buffer.IsCompressed)
            ev.Buffer.UnCompress();
        return ev;
    }

    private static async Task ReadExactAsync(NetworkStream stream, byte[] buf)
    {
        int offset = 0;
        while (offset < buf.Length)
        {
            int read = await stream.ReadAsync(buf.AsMemory(offset)).WaitAsync(TimeSpan.FromSeconds(5));
            if (read == 0) throw new EndOfStreamException();
            offset += read;
        }
    }

    public void Dispose()
    {
        try { if (File.Exists(_dbPath)) File.Delete(_dbPath); } catch { }
    }
}
