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
    public void RoomInfo_SelectedMap_UsesCppShortKeyAndIntValue()
    {
        var packet = new KRoomInfo { AllPlayersSelectedMap = new() { [0x1234] = 0x01020304 }, DefenceDungeonOpen = true };
        var buffer = new KSerBuffer();
        var writer = new KSerializer(RegionProfile.Taiwan);
        writer.BeginWriting(buffer);
        Assert.True(packet.Put(writer, RegionProfile.Taiwan));
        writer.EndWriting();
        Assert.Equal(Convert.FromHexString("0000000112340102030401"), buffer.ToArray()[^11..]);
    }

    [Theory]
    [InlineData(30030, true)]
    [InlineData(30035, true)]
    [InlineData(30040, true)]
    [InlineData(39600, true)]
    [InlineData(39608, true)]
    [InlineData(39609, false)]
    [InlineData(30000, false)]
    [InlineData(30010, false)]
    [InlineData(39000, false)]
    public void TutorialRoom_SinglePlayIdsMatchCpp(int id, bool expected)
    {
        Assert.Equal(expected, KncWX2Server.GameServer.TutorialRoomRules.IsSinglePlayDungeon(id));
    }

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
    public void ChannelListNotification_MatchesCppFourByteServerUidWireLayout()
    {
        // C++ KChannelInfo uses int for m_iServerUID. Two entries catch per-entry shifts.
        byte[] expected = Convert.FromHexString(
            "000000000000000200000001000003E800000001000000024100000000000000000024542457000003E8000000000000000000000000" +
            "00000002000003E900000002000000024200000000000000000024542457000003E800000000000000000000000000000000");
        var region = RegionProfile.Taiwan;
        var packet = new KECH_GET_CHANNEL_LIST_NOT
        {
            ServerGroupId = 0,
            ChannelList = new()
            {
                [1] = new KChannelInfo { ServerUid = 1000, ChannelId = 1, ChannelName = "A", MasterPort = 9300, NcUdpPort = 9303, MaxUser = 1000 },
                [2] = new KChannelInfo { ServerUid = 1001, ChannelId = 2, ChannelName = "B", MasterPort = 9300, NcUdpPort = 9303, MaxUser = 1000 },
            },
        };
        var output = new KSerBuffer();
        var writer = new KSerializer(region);
        writer.BeginWriting(output);
        Assert.True(packet.Put(writer, region));
        writer.EndWriting();
        Assert.Equal(expected, output.ToArray());

        var input = new KSerBuffer(expected);
        var reader = new KSerializer(region);
        reader.BeginReading(input);
        var decoded = new KECH_GET_CHANNEL_LIST_NOT();
        Assert.True(decoded.Get(reader, region));
        Assert.Equal(expected.Length, input.ReadOffset);
        Assert.Equal(1000, decoded.ChannelList[1].ServerUid);
        Assert.Equal("B", decoded.ChannelList[2].ChannelName);
        Assert.Equal(1001, decoded.ChannelList[2].ServerUid);
    }

    [Fact]
    public void ToolServerList_UsesFourByteServerUid()
    {
        var region = RegionProfile.Taiwan;
        var buffer = new KSerBuffer();
        var writer = new KSerializer(region);
        writer.BeginWriting(buffer);
        Assert.True(new KToolServerList { ServerUid = 1000, ServerSetId = 7 }.Put(writer, region));
        writer.EndWriting();
        Assert.Equal(Convert.FromHexString("000003E8000000070000000000000000000000000000000000000000"), buffer.ToArray());
        var reader = new KSerializer(region);
        reader.BeginReading(new KSerBuffer(buffer.ToArray()));
        var result = new KToolServerList();
        Assert.True(result.Get(reader, region));
        Assert.Equal(1000, result.ServerUid);
        Assert.Equal(7, result.ServerSetId);
    }

    [Fact]
    public void CharacterListUnitInfo_UsesCppRegionalTailOrder()
    {
        var unit = new KUnitInfo
        {
            LastLoginTime = "Z", LastDate = "D", Deleted = true, DelAbleDate = 1, RestoreAbleDate = 2,
            WarpVipEndDate = 3, EventQuestClearCount = 4, ExchangeCount = 5, AccumulationSpirit = 99,
            OldYearMissionRewardedLevel = 6, NewYearMissionStepId = 7,
            CheckPowerCount = 8, CheckPowerTime = 9, CheckPowerShowPopUp = true, CheckPowerScore = 10,
            AccountPvpLoseCount = 11,
        };
        var cases = new[]
        {
            (RegionProfile.Taiwan, "000000025A00000000000000000300000004000000050600000007080000000000000009010A"),
            (RegionProfile.HongKong, "000000025A00000000000000000300000004000000050600000007080000000000000009010A"),
            (RegionProfile.China, "000000025A000000000244000100000000000000010000000000000002000000000000000300000004000000050600000007"),
            (RegionProfile.Japan, "000000025A00000000000000000300000004000000050600000007080000000000000009010A0000000B"),
        };
        foreach (var (region, tailHex) in cases)
        {
            Assert.False(region.AccumulationSpiritSystem); // guarded-out burning event in these builds
            var buffer = new KSerBuffer();
            var writer = new KSerializer(region);
            writer.BeginWriting(buffer);
            Assert.True(unit.Put(writer, region));
            var expectedTail = Convert.FromHexString(tailHex);
            Assert.Equal(expectedTail, buffer.ToArray()[^expectedTail.Length..]);
            var reader = new KSerializer(region);
            var input = new KSerBuffer(buffer.ToArray());
            reader.BeginReading(input);
            var parsed = new KUnitInfo();
            Assert.True(parsed.Get(reader, region));
            Assert.Equal(input.Length, input.ReadOffset);
            Assert.Equal("Z", parsed.LastLoginTime);
            Assert.Equal(3, parsed.WarpVipEndDate);
            Assert.Equal(0, parsed.AccumulationSpirit);
            if (region.UnitWaitDelete)
            {
                Assert.Equal("D", parsed.LastDate);
                Assert.True(parsed.Deleted);
                Assert.Equal(1, parsed.DelAbleDate);
                Assert.Equal(2, parsed.RestoreAbleDate);
            }
            if (region.Region is RegionCode.TW or RegionCode.HK or RegionCode.JP)
            {
                Assert.Equal((byte)8, parsed.CheckPowerCount);
                Assert.Equal(9, parsed.CheckPowerTime);
                Assert.True(parsed.CheckPowerShowPopUp);
                Assert.Equal((byte)10, parsed.CheckPowerScore);
            }
            if (region.Region == RegionCode.JP) Assert.Equal(11, parsed.AccountPvpLoseCount);
            reader.BeginReading(new KSerBuffer(buffer.AsSpan()[..^1]));
            Assert.False(new KUnitInfo().Get(reader, region));
        }
    }

    [Fact]
    public void SelectServerSetAck_MatchesCppWireLayout()
    {
        var region = RegionProfile.Taiwan;
        var output = new KSerBuffer();
        var writer = new KSerializer(region);
        writer.BeginWriting(output);
        Assert.True(new KEGS_SELECT_SERVER_SET_ACK { Ok = 0, Ip = "A", Port = 9300 }.Put(writer, region));
        Assert.Equal(Convert.FromHexString("0000000000000002410024540000000000000000"), output.ToArray());
    }

    [Fact]
    public void GameConnectAck_UsesCppFourByteUdpPort()
    {
        var region = RegionProfile.Taiwan;
        var buffer = new KSerBuffer();
        var writer = new KSerializer(region);
        writer.BeginWriting(buffer);
        Assert.True(new KEGS_CONNECT_ACK { Ok = 0, UdpPort = 9302, ChannelId = 1 }.Put(writer, region));
        byte[] expected = Convert.FromHexString("000000000000245600000001");
        Assert.Equal(expected, buffer.ToArray());
        var reader = new KSerializer(region);
        reader.BeginReading(new KSerBuffer(expected));
        var result = new KEGS_CONNECT_ACK();
        Assert.True(result.Get(reader, region));
        Assert.Equal(9302, result.UdpPort);
        Assert.Equal(1, result.ChannelId);
    }

    [Fact]
    public void EnxUserLoginNot_MatchesCppWireLayoutForSupportedRegions()
    {
        var expected = Convert.FromHexString("01000000023F8000004000000040400000014080000000000007010001");
        var packet = new KENX_USER_LOGIN_NOT
        {
            IsGameBang = true,
            PcBangReward = new KPcBangReward
            {
                PcBangType = 2, ExpRate = 1, EdRate = 2, VsPointRate = 3,
                SpiritFree = true, PetExpRate = 4, AdditionalHenirReward = 7,
                SecretDungeonFree = true, Avatar = false, Title = true,
            },
        };
        foreach (var region in new[] { RegionProfile.Taiwan, RegionProfile.HongKong, RegionProfile.China, RegionProfile.Japan })
        {
            Assert.True(region.PcBangType);
            var output = new KSerBuffer();
            var writer = new KSerializer(region);
            writer.BeginWriting(output);
            Assert.True(packet.Put(writer, region));
            Assert.Equal(expected, output.ToArray());
            var input = new KSerBuffer(expected);
            var reader = new KSerializer(region);
            reader.BeginReading(input);
            var parsed = new KENX_USER_LOGIN_NOT();
            Assert.True(parsed.Get(reader, region));
            Assert.Equal(expected.Length, input.ReadOffset);
            Assert.True(parsed.IsGameBang);
            Assert.Equal(2, parsed.PcBangReward.PcBangType);
            Assert.Equal(4f, parsed.PcBangReward.PetExpRate);
            Assert.Equal(7, parsed.PcBangReward.AdditionalHenirReward);
            Assert.True(parsed.PcBangReward.SecretDungeonFree);
            Assert.False(parsed.PcBangReward.Avatar);
            Assert.True(parsed.PcBangReward.Title);
            reader.BeginReading(new KSerBuffer(expected.AsSpan(0, expected.Length - 1)));
            Assert.False(new KENX_USER_LOGIN_NOT().Get(reader, region));
        }
    }

    [Fact]
    public void EnxUserLoginNot_DefaultNonPcBangRewardMatchesCpp()
    {
        var region = RegionProfile.Taiwan;
        var buffer = new KSerBuffer();
        var writer = new KSerializer(region);
        writer.BeginWriting(buffer);
        Assert.True(new KENX_USER_LOGIN_NOT().Put(writer, region));
        Assert.Equal(Convert.FromHexString("00FFFFFFFF000000000000000000000000000000000000000000000000"), buffer.ToArray());
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
