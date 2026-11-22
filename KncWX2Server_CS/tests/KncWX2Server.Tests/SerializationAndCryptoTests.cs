using KncWX2Server.Core.Events;
using KncWX2Server.Core.Packets;
using KncWX2Server.Core.Region;
using KncWX2Server.Core.Security;
using KncWX2Server.Core.Serialization;
using Xunit;

namespace KncWX2Server.Tests;

public sealed class SerializationAndCryptoTests
{
    [Fact]
    public void KSerializer_EncodesBigEndianNumericsAndUtf16LeStrings()
    {
        var region = RegionProfile.Taiwan;
        var buf = new KSerBuffer();
        var ks = new KSerializer(region);

        ks.BeginWriting(buf);
        ks.Put(0x01020304);
        ks.PutNarrowString("abc");
        ks.PutWideString("台灣");
        ks.Put(true);
        ks.EndWriting();

        byte[] raw = buf.ToArray();
        // First 4 bytes = Big-Endian 0x01020304
        Assert.Equal(new byte[] { 0x01, 0x02, 0x03, 0x04 }, raw[..4]);

        var ksRead = new KSerializer(region);
        ksRead.BeginReading(new KSerBuffer(raw));
        Assert.True(ksRead.Get(out int num));
        Assert.True(ksRead.GetNarrowString(out string s));
        Assert.True(ksRead.GetWideString(out string ws));
        Assert.True(ksRead.Get(out bool b));
        ksRead.EndReading();

        Assert.Equal(0x01020304, num);
        Assert.Equal("abc", s);
        Assert.Equal("台灣", ws);
        Assert.True(b);
    }

    [Fact]
    public void KSerBuffer_CompressAndUncompress_Roundtrip()
    {
        var original = Enumerable.Range(0, 500).Select(i => (byte)(i & 0xFF)).ToArray();
        var buf = new KSerBuffer(original);
        Assert.True(buf.Compress());
        Assert.True(buf.IsCompressed);
        Assert.True(buf.Length < original.Length);

        Assert.True(buf.UnCompress());
        Assert.False(buf.IsCompressed);
        Assert.Equal(original, buf.ToArray());
    }

    [Fact]
    public void KSecureBuffer_EncryptDecrypt_BothSpi0AndSessionSpi()
    {
        var saDb = new KSADatabase();
        byte[] payload = "Hello Elsword Wire Protocol!"u8.ToArray();

        // SPI = 0 default keys
        var enc0 = new KSecureBuffer(saDb, 0);
        Assert.True(enc0.Create(payload));

        var dec0 = new KSecureBuffer(saDb, enc0.WireBytes.ToArray());
        Assert.True(dec0.TryGetPayload(out byte[] out0, authentic: true, checkSequenceNum: true));
        Assert.Equal(payload, out0);

        // Negotiated random SPI
        ushort newSpi = saDb.CreateNewSA(out var sa);
        Assert.NotEqual((ushort)0, newSpi);

        var enc1 = new KSecureBuffer(saDb, newSpi);
        Assert.True(enc1.Create(payload));

        var dec1 = new KSecureBuffer(saDb, enc1.WireBytes.ToArray());
        Assert.True(dec1.TryGetPayload(out byte[] out1, authentic: true, checkSequenceNum: true));
        Assert.Equal(payload, out1);

        // Replay of the exact same SPI != 0 packet must fail sequence check
        var replay = new KSecureBuffer(saDb, enc1.WireBytes.ToArray());
        Assert.False(replay.TryGetPayload(out _, authentic: true, checkSequenceNum: true));
    }

    [Fact]
    public void KSecureBuffer_ClientLocalSpiDiffersFromServerSpi_DecryptsViaExpectedSpi()
    {
        // Simulates X2ServerProtocol KSession::OnRecvCompleted(E_ACCEPT_CONNECTION_NOT) + SADatabase::Insert(OUT SpiType&),
        // where the client stores the server's SA under a different client-local SPI and stamps that SPI in packet headers.
        var serverSaDb = new KSADatabase();
        ushort serverSpi = serverSaDb.CreateNewSA(out var serverSa);

        var clientSaDb = new KSADatabase();
        ushort clientSpi = (ushort)(serverSpi == 12345 ? 54321 : 12345);
        Assert.True(clientSaDb.Insert(clientSpi, serverSa.Clone()));

        byte[] payload = "ECH_VERIFY_ACCOUNT_REQ"u8.ToArray();
        var clientPacket = new KSecureBuffer(clientSaDb, clientSpi);
        Assert.True(clientPacket.Create(payload));

        // Server receive-side uses serverSpi (like C++ KSecureBuffer(spSession->m_nSPIndex, ...))
        var serverRecv = new KSecureBuffer(serverSaDb, serverSpi, clientPacket.WireBytes.ToArray());
        Assert.True(serverRecv.TryGetPayload(out byte[] decrypted, out bool seqMismatch, out string reason, authentic: true, checkSequenceNum: true));
        Assert.False(seqMismatch);
        Assert.Equal(string.Empty, reason);
        Assert.Equal(payload, decrypted);
    }

    [Theory]
    [InlineData(RegionCode.TW)]
    [InlineData(RegionCode.HK)]
    [InlineData(RegionCode.CN)]
    [InlineData(RegionCode.JP)]
    public void RegionalPackets_RoundtripAcrossAllProfiles(RegionCode code)
    {
        var region = RegionProfile.FromCode(code);
        var req = new KECH_VERIFY_ACCOUNT_REQ
        {
            Id = "player01",
            Password = "secret",
            ChannelingCode = 1,
            IdVerified = true,
            Login = true,
            MacAddress = "00-11-22-33-44-55",
            PublisherOtp = "9999",
        };

        var ev = new KEvent();
        ev.SetData(PerformerId.PI_CHANNEL_USER, [100L, -1L], EventId.ECH_VERIFY_ACCOUNT_REQ, req, region);

        var ks = new KSerializer(region);
        var wire = new KSerBuffer();
        ks.BeginWriting(wire);
        ev.Put(ks, region);
        ks.EndWriting();

        var evOut = new KEvent();
        var ksIn = new KSerializer(region);
        ksIn.BeginReading(new KSerBuffer(wire.ToArray()));
        Assert.True(evOut.Get(ksIn, region));
        ksIn.EndReading();

        var parsed = evOut.GetPayload<KECH_VERIFY_ACCOUNT_REQ>(region);
        Assert.Equal("player01", parsed.Id);
        Assert.Equal("secret", parsed.Password);
        Assert.Equal(1, parsed.ChannelingCode);
        Assert.True(parsed.IdVerified);

        if (code == RegionCode.CN)
        {
            Assert.Equal("00-11-22-33-44-55", parsed.MacAddress);
            Assert.Equal("9999", parsed.PublisherOtp);
        }
    }
}
