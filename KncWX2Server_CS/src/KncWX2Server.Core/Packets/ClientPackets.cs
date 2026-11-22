using KncWX2Server.Core.Region;
using KncWX2Server.Core.Serialization;

namespace KncWX2Server.Core.Packets;

public class KPacketOK : IKSerializable
{
    public int Ok { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(Ok);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok)) return false;
        Ok = ok;
        return true;
    }
}

public sealed class KECH_VERIFY_ACCOUNT_REQ : IKSerializable
{
    public const int ServerSnLength = 12;

    public bool Login { get; set; }
    public string Id { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int ChannelingCode { get; set; }
    public string UserToken { get; set; } = string.Empty;
    public bool UseVerify { get; set; }
    public string MachineId { get; set; } = string.Empty;
    public byte[] ServerSN { get; set; } = new byte[ServerSnLength];
    public bool IdVerified { get; set; }
    public string MacAddress { get; set; } = string.Empty;
    public string PublisherOtp { get; set; } = string.Empty;
    public string Domain { get; set; } = string.Empty;
    public bool SteamClient { get; set; }
    public bool AeriaClient { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Login)) return false;
        if (!ks.PutWideString(Id)) return false;
        if (!ks.PutWideString(Password)) return false;
        if (region.GlobalAuth && !ks.Put(ChannelingCode)) return false;
        if (region.UsePasswordToken && !ks.PutNarrowString(UserToken)) return false;
        if (region.DllListCheckBeforeLoading && !ks.Put(UseVerify)) return false;
        if (region.MachineIdCheckBeforeLoading && !ks.PutNarrowString(MachineId)) return false;
        if (region.SerialNumberAvailabilityCheck)
        {
            for (int i = 0; i < ServerSnLength; i++)
            {
                byte b = i < ServerSN.Length ? ServerSN[i] : (byte)0;
                if (!ks.Put(b)) return false;
            }
        }
        if (region.KogOtpVerify && !ks.Put(IdVerified)) return false;
        if (region.LoginMacAddress && !ks.PutWideString(MacAddress)) return false;
        if (region.PublisherOtp && !ks.PutWideString(PublisherOtp)) return false;
        if (region.UseDomainAccount && !ks.PutWideString(Domain)) return false;
        if (region.Steam && !ks.Put(SteamClient)) return false;
        if (region.ChannelingAeria && !ks.Put(AeriaClient)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool login)) return false;
        if (!ks.GetWideString(out var id)) return false;
        if (!ks.GetWideString(out var pw)) return false;
        Login = login;
        Id = id;
        Password = pw;

        if (region.GlobalAuth)
        {
            if (!ks.Get(out int cc)) return false;
            ChannelingCode = cc;
        }
        if (region.UsePasswordToken)
        {
            if (!ks.GetNarrowString(out var tok)) return false;
            UserToken = tok;
        }
        if (region.DllListCheckBeforeLoading)
        {
            if (!ks.Get(out bool uv)) return false;
            UseVerify = uv;
        }
        if (region.MachineIdCheckBeforeLoading)
        {
            if (!ks.GetNarrowString(out var mid)) return false;
            MachineId = mid;
        }
        if (region.SerialNumberAvailabilityCheck)
        {
            ServerSN = new byte[ServerSnLength];
            for (int i = 0; i < ServerSnLength; i++)
            {
                if (!ks.Get(out byte b)) return false;
                ServerSN[i] = b;
            }
        }
        if (region.KogOtpVerify)
        {
            if (!ks.Get(out bool idv)) return false;
            IdVerified = idv;
        }
        if (region.LoginMacAddress)
        {
            if (!ks.GetWideString(out var mac)) return false;
            MacAddress = mac;
        }
        if (region.PublisherOtp)
        {
            if (!ks.GetWideString(out var otp)) return false;
            PublisherOtp = otp;
        }
        if (region.UseDomainAccount)
        {
            if (!ks.GetWideString(out var dom)) return false;
            Domain = dom;
        }
        if (region.Steam)
        {
            if (!ks.Get(out bool st)) return false;
            SteamClient = st;
        }
        if (region.ChannelingAeria)
        {
            if (!ks.Get(out bool ae)) return false;
            AeriaClient = ae;
        }
        return true;
    }
}

public sealed class KECH_VERIFY_ACCOUNT_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long UserUid { get; set; }
    public string Passport { get; set; } = string.Empty;
    public int ChannelingCode { get; set; }
    public string CurrentTime { get; set; } = string.Empty;
    public bool UseVerify { get; set; }
    public int ChannelRandomKey { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string PurchaseTok { get; set; } = string.Empty;
    public string AgreementUrl { get; set; } = string.Empty;
    public string SocketId { get; set; } = string.Empty;
    public int ServerGroupId { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(UserUid)) return false;
        if (!ks.PutWideString(Passport)) return false;
        if (region.GlobalAuth && !ks.Put(ChannelingCode)) return false;
        if (!region.ServerTimeGet && region.MassFileMappingFunction && !ks.PutWideString(CurrentTime)) return false;
        if (region.DllListCheckBeforeLoading)
        {
            if (!ks.Put(UseVerify) || !ks.Put(ChannelRandomKey)) return false;
        }
        if (region.KogOtpVerify && !ks.PutWideString(UserId)) return false;
        if (region.PurchaseToken && !ks.PutWideString(PurchaseTok)) return false;
        if (region.LoginTou && !ks.PutNarrowString(AgreementUrl)) return false;
        if (region.CountryTH && !ks.PutWideString(SocketId)) return false;
        if (region.SouthAmerica && !ks.Put(ServerGroupId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok)) return false;
        if (!ks.Get(out long uid)) return false;
        if (!ks.GetWideString(out var pp)) return false;
        Ok = ok;
        UserUid = uid;
        Passport = pp;

        if (region.GlobalAuth)
        {
            if (!ks.Get(out int cc)) return false;
            ChannelingCode = cc;
        }
        if (!region.ServerTimeGet && region.MassFileMappingFunction)
        {
            if (!ks.GetWideString(out var ct)) return false;
            CurrentTime = ct;
        }
        if (region.DllListCheckBeforeLoading)
        {
            if (!ks.Get(out bool uv) || !ks.Get(out int rk)) return false;
            UseVerify = uv;
            ChannelRandomKey = rk;
        }
        if (region.KogOtpVerify)
        {
            if (!ks.GetWideString(out var userId)) return false;
            UserId = userId;
        }
        if (region.PurchaseToken)
        {
            if (!ks.GetWideString(out var pt)) return false;
            PurchaseTok = pt;
        }
        if (region.LoginTou)
        {
            if (!ks.GetNarrowString(out var url)) return false;
            AgreementUrl = url;
        }
        if (region.CountryTH)
        {
            if (!ks.GetWideString(out var sid)) return false;
            SocketId = sid;
        }
        if (region.SouthAmerica)
        {
            if (!ks.Get(out int sg)) return false;
            ServerGroupId = sg;
        }
        return true;
    }
}

public sealed class KECH_GET_CHANNEL_LIST_NOT : IKSerializable
{
    public int ServerGroupId { get; set; }
    public Dictionary<int, KChannelInfo> ChannelList { get; set; } = new();
    public Dictionary<int, KChannelBonusInfo> ChannelBonusList { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ServerGroupId)) return false;
        if (!ks.PutMap(ChannelList, static (s, k) => s.Put(k), static (s, v) => v.Put(s, s.Region))) return false;
        if (region.ChannelListRenewal &&
            !ks.PutMap(ChannelBonusList, static (s, k) => s.Put(k), static (s, v) => v.Put(s, s.Region)))
            return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int sg)) return false;
        ServerGroupId = sg;
        if (!ks.GetMap(ChannelList,
            static s => (s.Get(out int k), k),
            static s => { var c = new KChannelInfo(); return (c.Get(s, s.Region), c); }))
            return false;
        if (region.ChannelListRenewal &&
            !ks.GetMap(ChannelBonusList,
                static s => (s.Get(out int k), k),
                static s => { var b = new KChannelBonusInfo(); return (b.Get(s, s.Region), b); }))
            return false;
        return true;
    }
}

public sealed class KECH_GET_SERVERGROUP_LIST_NOT : IKSerializable
{
    public Dictionary<int, KServerGroupInfo> ServerGroupList { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
        => ks.PutMap(ServerGroupList, static (s, k) => s.Put(k), static (s, v) => v.Put(s, s.Region));

    public bool Get(KSerializer ks, RegionProfile region)
        => ks.GetMap(ServerGroupList,
            static s => (s.Get(out int k), k),
            static s => { var g = new KServerGroupInfo(); return (g.Get(s, s.Region), g); });
}

public sealed class KECH_HACKING_TOOL_LIST_REQ : IKSerializable
{
    public int RandomKey { get; set; }
    public List<KHackingToolInfo> HackingToolList { get; set; } = [];

    public bool Put(KSerializer ks, RegionProfile region)
        => ks.Put(RandomKey) && ks.PutList(HackingToolList);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int rk)) return false;
        if (!ks.GetList(HackingToolList)) return false;
        RandomKey = rk;
        return true;
    }
}

public sealed class KECH_HACKING_TOOL_LIST_ACK : IKSerializable
{
    public int Ok { get; set; }
    public string MachineId { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
        => ks.Put(Ok) && ks.PutNarrowString(MachineId);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok)) return false;
        if (!ks.GetNarrowString(out var mid)) return false;
        Ok = ok;
        MachineId = mid;
        return true;
    }
}

public sealed class KECH_NEW_SERVER_SN_NOT : IKSerializable
{
    public byte[] ServerSN { get; set; } = new byte[12];

    public bool Put(KSerializer ks, RegionProfile region)
    {
        for (int i = 0; i < 12; i++)
            if (!ks.Put(i < ServerSN.Length ? ServerSN[i] : (byte)0)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        ServerSN = new byte[12];
        for (int i = 0; i < 12; i++)
            if (!ks.Get(out ServerSN[i])) return false;
        return true;
    }
}

public sealed class KECH_WAIT_QUEUE_MESSAGE_NOT : IKSerializable
{
    public int WaitUserCount { get; set; }
    public bool Passed { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
        => ks.Put(WaitUserCount) && ks.Put(Passed);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int cnt)) return false;
        if (!ks.Get(out bool p)) return false;
        WaitUserCount = cnt;
        Passed = p;
        return true;
    }
}

public sealed class KECH_SECURITY_AUTH_REQ : IKSerializable
{
    public int SecurityAuthType { get; set; }
    public long UserUid { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Matrix { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(SecurityAuthType) || !ks.Put(UserUid) || !ks.PutWideString(UserId)) return false;
        if (region.PublisherMatrixKey && !ks.PutWideString(Matrix)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int st) || !ks.Get(out long uid) || !ks.GetWideString(out var id)) return false;
        SecurityAuthType = st;
        UserUid = uid;
        UserId = id;
        if (region.PublisherMatrixKey)
        {
            if (!ks.GetWideString(out var m)) return false;
            Matrix = m;
        }
        return true;
    }
}

public sealed class KECH_SECURITY_AUTH_ACK : IKSerializable
{
    public int Ok { get; set; }
    public string SecondSecurityUrl { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
        => ks.Put(Ok) && ks.PutNarrowString(SecondSecurityUrl);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.GetNarrowString(out var url)) return false;
        Ok = ok;
        SecondSecurityUrl = url;
        return true;
    }
}

public sealed class KECH_SECURITY_INFO_NOT : IKSerializable
{
    public enum SecurityInfoType
    {
        None = 0,
        SecondSecurity = 1,
        MatrixCard = 2,
    }

    public int InfoType { get; set; }
    public uint MatrixKey { get; set; }
    public string Coordinate { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
        => ks.Put(InfoType) && ks.Put(MatrixKey) && ks.PutWideString(Coordinate);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int t) || !ks.Get(out uint mk) || !ks.GetWideString(out var c)) return false;
        InfoType = t;
        MatrixKey = mk;
        Coordinate = c;
        return true;
    }
}

public sealed class KECH_GET_SERVER_TIME_ACK : IKSerializable
{
    public string CurrentTime { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region) => ks.PutWideString(CurrentTime);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out var t)) return false;
        CurrentTime = t;
        return true;
    }
}

public sealed class KECH_PCBANG_IP_AND_MAC_INFO_NOT : IKSerializable
{
    public string PublicIp { get; set; } = string.Empty;
    public string MacInfo { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
        => ks.PutWideString(PublicIp) && ks.PutWideString(MacInfo);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out var ip) || !ks.GetWideString(out var mac)) return false;
        PublicIp = ip;
        MacInfo = mac;
        return true;
    }
}

public sealed class KEGS_HACKING_TOOL_LIST_NOT : IKSerializable
{
    public int RandomKey { get; set; }
    public List<KHackingToolInfo> HackingToolList { get; set; } = [];

    public bool Put(KSerializer ks, RegionProfile region)
        => ks.Put(RandomKey) && ks.PutList(HackingToolList);

    public bool Get(KSerializer ks, RegionProfile region)
        => ks.Get(out int rk) && (RandomKey = rk) >= int.MinValue && ks.GetList(HackingToolList);
}

public sealed class KEGS_SERVER_CHECK_HACK_USER_REQ : IKSerializable
{
    public const sbyte HctNone = 0;
    public const sbyte HctDungeonNpcUnitDie = 1;
    public const sbyte HctDungeonGetItem = 2;
    public const sbyte HctDungeonClearResult = 6;
    public const sbyte HctDungeonClearTime = 8;
    public const sbyte HctDeserializeFail = 9;
    public const sbyte HctMonitoringLetterAndTrade = 10;
    public const sbyte HctCheckMachineId = 13;
    public const sbyte HctCheckNpcDieTime = 14;
    public const sbyte HctCheckGetItemCount = 15;
    public const sbyte HctEdMonitoringSendMail = 16;
    public const sbyte HctEdMonitoringAccountBlock = 17;
    public const sbyte HctEdMonitoringMachineIdBlock = 18;
    public const sbyte HctChangeServerSnSendMail = 21;
    public const sbyte HctVerifyDungeonNpcUid = 26;
    public const sbyte HctCheckSequenceCount = 27;
    public const sbyte HctCheckBlockMachineId = 32;
    public const sbyte HctKickByGm = 33;
    public const sbyte HctImpossibleSellItem = 39;

    public sbyte HackingCheckType { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(HackingCheckType);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte t)) return false;
        HackingCheckType = t;
        return true;
    }
}

public class KEGS_LOCAL_RANKING_RANK_UP_NOT : IKSerializable
{
    public int MyRank { get; set; }
    public sbyte MainTabIndex { get; set; } = -1;
    public sbyte SubTabIndex { get; set; } = -1;
    public KLocalRankingUnitInfo MyUnitInfo { get; set; } = new();
    public KLocalRankingUnitInfo NextUnitInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(MyRank) && ks.Put(MainTabIndex) && ks.Put(SubTabIndex) &&
        ks.Put(MyUnitInfo) && ks.Put(NextUnitInfo);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int r) || !ks.Get(out sbyte mt) || !ks.Get(out sbyte st) ||
            !ks.Get(MyUnitInfo) || !ks.Get(NextUnitInfo))
            return false;
        MyRank = r;
        MainTabIndex = mt;
        SubTabIndex = st;
        return true;
    }
}

public sealed class KEGS_RECRUIT_RECRUITER_INFO_NOT : IKSerializable
{
    public long SenderUnitUid { get; set; }
    public sbyte Type { get; set; }
    public int Data { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(SenderUnitUid) && ks.Put(Type) && ks.Put(Data);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.Get(out sbyte t) || !ks.Get(out int d))
            return false;
        SenderUnitUid = uid;
        Type = t;
        Data = d;
        return true;
    }
}
