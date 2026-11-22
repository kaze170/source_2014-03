using KncWX2Server.ChannelServer.Managers;
using KncWX2Server.Core.Events;
using KncWX2Server.Core.Framework;
using KncWX2Server.Core.Region;
using Xunit;

namespace KncWX2Server.Tests;

public sealed class RegionAndManagerTests
{
    [Fact]
    public void KncUid_PacksAndExtractsServerGroupId_PerRegion()
    {
        long twUid = KncUid.MakeUid(RegionProfile.Taiwan, serverGroupId: 1, pureUid: 123456);
        Assert.Equal(1L, KncUid.ExtractServerGroupID(twUid, RegionProfile.Taiwan));
        Assert.Equal(123456L, KncUid.ExtractPureUID(twUid, RegionProfile.Taiwan));

        long cnUid = KncUid.MakeUid(RegionProfile.China, serverGroupId: 102, pureUid: 987654);
        Assert.Equal(102L, KncUid.ExtractServerGroupID(cnUid, RegionProfile.China));
        Assert.Equal(987654L, KncUid.ExtractPureUID(cnUid, RegionProfile.China));
    }

    [Fact]
    public void NetError_ResolvesRegionalDivergences()
    {
        Assert.Equal(1402, NetErrorRegistry.Resolve(RegionCode.TW, NetErrorKey.ERR_ACCOUNT_BLOCK_03));
        Assert.Equal(1402, NetErrorRegistry.Resolve(RegionCode.CN, NetErrorKey.ERR_ACCOUNT_BLOCK_03));
        Assert.Equal(15, NetErrorRegistry.Resolve(RegionCode.JP, NetErrorKey.ERR_ACCOUNT_BLOCK_03));
    }

    [Fact]
    public void KServerSNMaker_RoundtripAndTamperCheck()
    {
        Assert.True(KServerSNMaker.MakeServerSN(9876543210L, out byte[] token));
        Assert.Equal(12, token.Length);
        Assert.True(KServerSNMaker.CheckServerSN(token, out long extracted));
        Assert.Equal(9876543210L, extracted);

        token[3] ^= 0xFF;
        Assert.False(KServerSNMaker.CheckServerSN(token, out _));
    }

    [Fact]
    public void KUserSecurityManager_ValidatesMachineIdChecksum()
    {
        byte[] mid = new byte[16];
        // WORD[0..5] = 1, 2, 3, 4, 5, 6 -> sum(w_i * (i + 3)) = 1*3 + 2*4 + 3*5 + 4*6 + 5*7 + 6*8 = 133
        for (ushort i = 0; i < 6; i++)
        {
            BitConverter.TryWriteBytes(mid.AsSpan(i * 2, 2), (ushort)(i + 1));
        }
        BitConverter.TryWriteBytes(mid.AsSpan(14, 2), (ushort)133);

        Assert.True(KUserSecurityManager.CheckValidMachineID(mid));
        mid[14] ^= 1;
        Assert.False(KUserSecurityManager.CheckValidMachineID(mid));
    }

    [Fact]
    public void KCHFSM_And_RepeatEventFilter_WorkAsExpected()
    {
        var fsm = new KCHFSM();
        Assert.Equal(KCHFSM.S_INIT, fsm.CurrentState);
        Assert.True(fsm.TryStateTransition(KCHFSM.I_TO_AUTHENTICATED, out int s1));
        Assert.Equal(KCHFSM.S_LOGINED, s1);
        Assert.True(fsm.TryStateTransition(KCHFSM.I_TO_WAIT_QUEUE, out int s2));
        Assert.Equal(KCHFSM.S_WAIT_QUEUE, s2);
        Assert.True(fsm.TryStateTransition(KCHFSM.I_TO_AUTHENTICATED, out int s3));
        Assert.Equal(KCHFSM.S_LOGINED, s3);

        var filter = new KRepeatEventFilter();
        filter.RegRepeatEvent(EventId.ECH_VERIFY_ACCOUNT_REQ, EventId.ECH_VERIFY_ACCOUNT_ACK);
        Assert.True(filter.CheckReqEvent((ushort)EventId.ECH_VERIFY_ACCOUNT_REQ));
        Assert.False(filter.CheckReqEvent((ushort)EventId.ECH_VERIFY_ACCOUNT_REQ));
        filter.CheckAckEvent((ushort)EventId.ECH_VERIFY_ACCOUNT_ACK);
        Assert.True(filter.CheckReqEvent((ushort)EventId.ECH_VERIFY_ACCOUNT_REQ));
    }
}
