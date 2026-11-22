using KncWX2Server.Core.Region;
using KncWX2Server.Core.Serialization;

namespace KncWX2Server.Core.Packets;

// Note: Server/DB packets are routed in-process in this single-process architecture,
// so they do not cross external server sockets; CLR properties suffice with clean defaults.

public sealed class KDBE_CHANNEL_SERVER_ON_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KServerInfo ServerInfo { get; set; } = new();
    public Dictionary<int, KNetAddress> LoginList { get; set; } = new();
    public Dictionary<int, KChannelInfo> ChannelList { get; set; } = new();
    public Dictionary<int, KServerGroupInfo> ServerGroupList { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(ServerInfo)) return false;
        if (!ks.PutMap(LoginList, static (s, v) => s.Put(v), static (s, v) => s.Put(v))) return false;
        if (!ks.PutMap(ChannelList, static (s, v) => s.Put(v), static (s, v) => s.Put(v))) return false;
        if (!ks.PutMap(ServerGroupList, static (s, v) => s.Put(v), static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        Ok = v0;
        if (!ks.Get(ServerInfo)) return false;
        if (!ks.GetMap(LoginList, static s => (s.Get(out int v), v), static s => { var v = new KNetAddress(); return (s.Get(v), v); })) return false;
        if (!ks.GetMap(ChannelList, static s => (s.Get(out int v), v), static s => { var v = new KChannelInfo(); return (s.Get(v), v); })) return false;
        if (!ks.GetMap(ServerGroupList, static s => (s.Get(out int v), v), static s => { var v = new KServerGroupInfo(); return (s.Get(v), v); })) return false;
        return true;
    }
}

public sealed class KDBE_INSERT_LOGIN_RESULT_INFO_REQ : IKSerializable
{
    public int ServerGroupId { get; set; }
    public long ServerUid { get; set; }
    public List<int> ResultCount { get; set; } = [];
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ServerGroupId)) return false;
        if (!ks.Put(ServerUid)) return false;
        if (!ks.PutList(ResultCount, static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        ServerGroupId = v0;
        if (!ks.Get(out long v1)) return false;
        ServerUid = v1;
        if (!ks.GetList(ResultCount, static s => (s.Get(out int v), v))) return false;
        return true;
    }
}

public sealed class KEPUBLISHER_SECURITY_INFO_NOT : IKSerializable
{
    public const int SIT_NONE = 0;
    public const int SIT_SECOND_SECURITY = 1;
    public const int SIT_MATRIX_CARD = 2;

    public int SecurityInfoType { get; set; }
    public uint MatrixKey { get; set; }
    public string Coordinate { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(SecurityInfoType)) return false;
        if (!ks.Put(MatrixKey)) return false;
        if (!ks.PutWideString(Coordinate)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        SecurityInfoType = v0;
        if (!ks.Get(out uint v1)) return false;
        MatrixKey = v1;
        if (!ks.GetWideString(out string v2)) return false;
        Coordinate = v2;
        return true;
    }
}

public sealed class KDBE_CHANNEL_LIST_REQ : IKSerializable
{
    public int ServerGroupId { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ServerGroupId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        ServerGroupId = v0;
        return true;
    }
}

public sealed class KDBE_CHANNEL_LIST_ACK : IKSerializable
{
    public Dictionary<int, KChannelInfo> ChannelList { get; set; } = new();
    public Dictionary<int, KChannelBonusInfo> ChannelBonusList { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutMap(ChannelList, static (s, v) => s.Put(v), static (s, v) => s.Put(v))) return false;
        if (!ks.PutMap(ChannelBonusList, static (s, v) => s.Put(v), static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetMap(ChannelList, static s => (s.Get(out int v), v), static s => { var v = new KChannelInfo(); return (s.Get(v), v); })) return false;
        if (!ks.GetMap(ChannelBonusList, static s => (s.Get(out int v), v), static s => { var v = new KChannelBonusInfo(); return (s.Get(v), v); })) return false;
        return true;
    }
}

public sealed class KDBE_SERVERGROUP_LIST_ACK : IKSerializable
{
    public Dictionary<int, KServerGroupInfo> ServerGroupList { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutMap(ServerGroupList, static (s, v) => s.Put(v), static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetMap(ServerGroupList, static s => (s.Get(out int v), v), static s => { var v = new KServerGroupInfo(); return (s.Get(v), v); })) return false;
        return true;
    }
}

public sealed class KEPUBLISHER_AUTHENTICATION_REQ : IKSerializable
{
    public long ActorUid { get; set; } = -1;
    public string Ip { get; set; } = string.Empty;
    public string ServiceAccountId { get; set; } = string.Empty;
    public string ServicePassword { get; set; } = string.Empty;
    public int ChannelingCode { get; set; }
    public bool ServerUseKogOtp { get; set; }
    public string MacAddress { get; set; } = string.Empty;
    public string PublisherOtp { get; set; } = string.Empty;
    public string Domain { get; set; } = string.Empty;
    public uint RequestId { get; set; }
    public ushort Zone { get; set; }
    public ushort Game { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ActorUid)) return false;
        if (!ks.PutWideString(Ip)) return false;
        if (!ks.PutWideString(ServiceAccountId)) return false;
        if (!ks.PutWideString(ServicePassword)) return false;
        if (!ks.Put(ChannelingCode)) return false;
        if (!ks.Put(ServerUseKogOtp)) return false;
        if (!ks.PutWideString(MacAddress)) return false;
        if (!ks.PutWideString(PublisherOtp)) return false;
        if (!ks.PutWideString(Domain)) return false;
        if (!ks.Put(RequestId)) return false;
        if (!ks.Put(Zone)) return false;
        if (!ks.Put(Game)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        ActorUid = v0;
        if (!ks.GetWideString(out string v1)) return false;
        Ip = v1;
        if (!ks.GetWideString(out string v2)) return false;
        ServiceAccountId = v2;
        if (!ks.GetWideString(out string v3)) return false;
        ServicePassword = v3;
        if (!ks.Get(out int v4)) return false;
        ChannelingCode = v4;
        if (!ks.Get(out bool v5)) return false;
        ServerUseKogOtp = v5;
        if (!ks.GetWideString(out string v6)) return false;
        MacAddress = v6;
        if (!ks.GetWideString(out string v7)) return false;
        PublisherOtp = v7;
        if (!ks.GetWideString(out string v8)) return false;
        Domain = v8;
        if (!ks.Get(out uint v9)) return false;
        RequestId = v9;
        if (!ks.Get(out ushort v10)) return false;
        Zone = v10;
        if (!ks.Get(out ushort v11)) return false;
        Game = v11;
        return true;
    }
}

public sealed class KEPUBLISHER_AUTHENTICATION_ACK : IKSerializable
{
    public int Ok { get; set; } = -1;
    public int Result { get; set; }
    public string Id { get; set; } = string.Empty;
    public uint PublisherUid { get; set; }
    public string Ip { get; set; } = string.Empty;
    public int ChannelingCode { get; set; }
    public string PurchaseTok { get; set; } = string.Empty;
    public uint AccountType { get; set; }
    public uint AccountState { get; set; }
    public string AgreementUrl { get; set; } = string.Empty;
    public int SecurityInfoType { get; set; }
    public uint MatrixKey { get; set; }
    public string Coordinate { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(Result)) return false;
        if (!ks.PutWideString(Id)) return false;
        if (!ks.Put(PublisherUid)) return false;
        if (!ks.PutWideString(Ip)) return false;
        if (!ks.Put(ChannelingCode)) return false;
        if (!ks.PutWideString(PurchaseTok)) return false;
        if (!ks.Put(AccountType)) return false;
        if (!ks.Put(AccountState)) return false;
        if (!ks.PutWideString(AgreementUrl)) return false;
        if (!ks.Put(SecurityInfoType)) return false;
        if (!ks.Put(MatrixKey)) return false;
        if (!ks.PutWideString(Coordinate)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        Ok = v0;
        if (!ks.Get(out int v1)) return false;
        Result = v1;
        if (!ks.GetWideString(out string v2)) return false;
        Id = v2;
        if (!ks.Get(out uint v3)) return false;
        PublisherUid = v3;
        if (!ks.GetWideString(out string v4)) return false;
        Ip = v4;
        if (!ks.Get(out int v5)) return false;
        ChannelingCode = v5;
        if (!ks.GetWideString(out string v6)) return false;
        PurchaseTok = v6;
        if (!ks.Get(out uint v7)) return false;
        AccountType = v7;
        if (!ks.Get(out uint v8)) return false;
        AccountState = v8;
        if (!ks.GetWideString(out string v9)) return false;
        AgreementUrl = v9;
        if (!ks.Get(out int v10)) return false;
        SecurityInfoType = v10;
        if (!ks.Get(out uint v11)) return false;
        MatrixKey = v11;
        if (!ks.GetWideString(out string v12)) return false;
        Coordinate = v12;
        return true;
    }
}

public sealed class KEPUBLISHER_SECURITY_AUTH_REQ : IKSerializable
{
    public const int SatNone = 0;
    public const int SatChkSecondSecurity = 1;
    public const int SatMatrixCard = 2;

    public long ActorUid { get; set; }
    public int SecurityAuthType { get; set; }
    public long UserUid { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Matrix { get; set; } = string.Empty;
    public uint MatrixKey { get; set; }
    public string Ip { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ActorUid)) return false;
        if (!ks.Put(SecurityAuthType)) return false;
        if (!ks.Put(UserUid)) return false;
        if (!ks.PutWideString(UserId)) return false;
        if (!ks.PutWideString(Matrix)) return false;
        if (!ks.Put(MatrixKey)) return false;
        if (!ks.PutWideString(Ip)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        ActorUid = v0;
        if (!ks.Get(out int v1)) return false;
        SecurityAuthType = v1;
        if (!ks.Get(out long v2)) return false;
        UserUid = v2;
        if (!ks.GetWideString(out string v3)) return false;
        UserId = v3;
        if (!ks.GetWideString(out string v4)) return false;
        Matrix = v4;
        if (!ks.Get(out uint v5)) return false;
        MatrixKey = v5;
        if (!ks.GetWideString(out string v6)) return false;
        Ip = v6;
        return true;
    }
}

public sealed class KEPUBLISHER_SECURITY_AUTH_ACK : IKSerializable
{
    public int Ok { get; set; } = -1;
    public string SecondSecurityUrl { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.PutWideString(SecondSecurityUrl)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        Ok = v0;
        if (!ks.GetWideString(out string v1)) return false;
        SecondSecurityUrl = v1;
        return true;
    }
}

public sealed class KELG_VERIFY_ACCOUNT_REQ : IKSerializable
{
    public long ChannelUserUid { get; set; }
    public bool Login { get; set; }
    public string Id { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int ChannelingCode { get; set; }
    public string Ip { get; set; } = string.Empty;
    public string MacAddress { get; set; } = string.Empty;
    public string PublisherOtp { get; set; } = string.Empty;
    public bool ServerUseKogOtp { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ChannelUserUid)) return false;
        if (!ks.Put(Login)) return false;
        if (!ks.PutWideString(Id)) return false;
        if (!ks.PutWideString(Password)) return false;
        if (!ks.Put(ChannelingCode)) return false;
        if (!ks.PutWideString(Ip)) return false;
        if (!ks.PutWideString(MacAddress)) return false;
        if (!ks.PutWideString(PublisherOtp)) return false;
        if (!ks.Put(ServerUseKogOtp)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        ChannelUserUid = v0;
        if (!ks.Get(out bool v1)) return false;
        Login = v1;
        if (!ks.GetWideString(out string v2)) return false;
        Id = v2;
        if (!ks.GetWideString(out string v3)) return false;
        Password = v3;
        if (!ks.Get(out int v4)) return false;
        ChannelingCode = v4;
        if (!ks.GetWideString(out string v5)) return false;
        Ip = v5;
        if (!ks.GetWideString(out string v6)) return false;
        MacAddress = v6;
        if (!ks.GetWideString(out string v7)) return false;
        PublisherOtp = v7;
        if (!ks.Get(out bool v8)) return false;
        ServerUseKogOtp = v8;
        return true;
    }
}

public sealed class KDBE_CH_USER_GENERAL_LOGIN_REQ : IKSerializable
{
    public string ServiceAccountId { get; set; } = string.Empty;
    public string Ip { get; set; } = string.Empty;
    public uint PublisherUid { get; set; }
    public int ChannelingCode { get; set; }
    public string PurchaseTok { get; set; } = string.Empty;
    public bool ServerUseKogOtp { get; set; }
    public string Password { get; set; } = string.Empty;
    public uint AccountType { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutWideString(ServiceAccountId)) return false;
        if (!ks.PutWideString(Ip)) return false;
        if (!ks.Put(PublisherUid)) return false;
        if (!ks.Put(ChannelingCode)) return false;
        if (!ks.PutWideString(PurchaseTok)) return false;
        if (!ks.Put(ServerUseKogOtp)) return false;
        if (!ks.PutWideString(Password)) return false;
        if (!ks.Put(AccountType)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string v0)) return false;
        ServiceAccountId = v0;
        if (!ks.GetWideString(out string v1)) return false;
        Ip = v1;
        if (!ks.Get(out uint v2)) return false;
        PublisherUid = v2;
        if (!ks.Get(out int v3)) return false;
        ChannelingCode = v3;
        if (!ks.GetWideString(out string v4)) return false;
        PurchaseTok = v4;
        if (!ks.Get(out bool v5)) return false;
        ServerUseKogOtp = v5;
        if (!ks.GetWideString(out string v6)) return false;
        Password = v6;
        if (!ks.Get(out uint v7)) return false;
        AccountType = v7;
        return true;
    }
}

public sealed class KDBE_CH_USER_GENERAL_LOGIN_ACK : IKSerializable
{
    public int Ok { get; set; }
    public string UserId { get; set; } = string.Empty;
    public long UserUid { get; set; }
    public string Otp { get; set; } = string.Empty;
    public uint PublisherUid { get; set; }
    public int ChannelingCode { get; set; }
    public string PurchaseTok { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.PutWideString(UserId)) return false;
        if (!ks.Put(UserUid)) return false;
        if (!ks.PutWideString(Otp)) return false;
        if (!ks.Put(PublisherUid)) return false;
        if (!ks.Put(ChannelingCode)) return false;
        if (!ks.PutWideString(PurchaseTok)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        Ok = v0;
        if (!ks.GetWideString(out string v1)) return false;
        UserId = v1;
        if (!ks.Get(out long v2)) return false;
        UserUid = v2;
        if (!ks.GetWideString(out string v3)) return false;
        Otp = v3;
        if (!ks.Get(out uint v4)) return false;
        PublisherUid = v4;
        if (!ks.Get(out int v5)) return false;
        ChannelingCode = v5;
        if (!ks.GetWideString(out string v6)) return false;
        PurchaseTok = v6;
        return true;
    }
}

public sealed class KDBE_CH_USER_KOGOTP_LOGIN_REQ : IKSerializable
{
    public string ServiceAccountId { get; set; } = string.Empty;
    public uint PublisherUid { get; set; }
    public string Otp { get; set; } = string.Empty;
    public string Ip { get; set; } = string.Empty;
    public string PurchaseTok { get; set; } = string.Empty;
    public int ChannelingCode { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutWideString(ServiceAccountId)) return false;
        if (!ks.Put(PublisherUid)) return false;
        if (!ks.PutWideString(Otp)) return false;
        if (!ks.PutWideString(Ip)) return false;
        if (!ks.PutWideString(PurchaseTok)) return false;
        if (!ks.Put(ChannelingCode)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string v0)) return false;
        ServiceAccountId = v0;
        if (!ks.Get(out uint v1)) return false;
        PublisherUid = v1;
        if (!ks.GetWideString(out string v2)) return false;
        Otp = v2;
        if (!ks.GetWideString(out string v3)) return false;
        Ip = v3;
        if (!ks.GetWideString(out string v4)) return false;
        PurchaseTok = v4;
        if (!ks.Get(out int v5)) return false;
        ChannelingCode = v5;
        return true;
    }
}

public sealed class KDBE_CH_USER_KOGOTP_LOGIN_ACK : IKSerializable
{
    public int Ok { get; set; }
    public string UserId { get; set; } = string.Empty;
    public long UserUid { get; set; }
    public string Otp { get; set; } = string.Empty;
    public uint PublisherUid { get; set; }
    public string PurchaseTok { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.PutWideString(UserId)) return false;
        if (!ks.Put(UserUid)) return false;
        if (!ks.PutWideString(Otp)) return false;
        if (!ks.Put(PublisherUid)) return false;
        if (!ks.PutWideString(PurchaseTok)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        Ok = v0;
        if (!ks.GetWideString(out string v1)) return false;
        UserId = v1;
        if (!ks.Get(out long v2)) return false;
        UserUid = v2;
        if (!ks.GetWideString(out string v3)) return false;
        Otp = v3;
        if (!ks.Get(out uint v4)) return false;
        PublisherUid = v4;
        if (!ks.GetWideString(out string v5)) return false;
        PurchaseTok = v5;
        return true;
    }
}

public sealed class KDBE_INTERNAL_LOGIN_REQ : IKSerializable
{
    public bool DebugAuth { get; set; } = true;
    public string UserId { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Ip { get; set; } = string.Empty;
    public int ChannelingCode { get; set; }
    public bool AutoCreateIfMissing { get; set; } = true;
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(DebugAuth)) return false;
        if (!ks.PutWideString(UserId)) return false;
        if (!ks.PutWideString(Password)) return false;
        if (!ks.PutWideString(Ip)) return false;
        if (!ks.Put(ChannelingCode)) return false;
        if (!ks.Put(AutoCreateIfMissing)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool v0)) return false;
        DebugAuth = v0;
        if (!ks.GetWideString(out string v1)) return false;
        UserId = v1;
        if (!ks.GetWideString(out string v2)) return false;
        Password = v2;
        if (!ks.GetWideString(out string v3)) return false;
        Ip = v3;
        if (!ks.Get(out int v4)) return false;
        ChannelingCode = v4;
        if (!ks.Get(out bool v5)) return false;
        AutoCreateIfMissing = v5;
        return true;
    }
}

public sealed class KDBE_INTERNAL_LOGIN_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long UserUid { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Otp { get; set; } = string.Empty;
    public int AuthLevel { get; set; }
    public bool InternalUser { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(UserUid)) return false;
        if (!ks.PutWideString(UserId)) return false;
        if (!ks.PutWideString(Otp)) return false;
        if (!ks.Put(AuthLevel)) return false;
        if (!ks.Put(InternalUser)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        Ok = v0;
        if (!ks.Get(out long v1)) return false;
        UserUid = v1;
        if (!ks.GetWideString(out string v2)) return false;
        UserId = v2;
        if (!ks.GetWideString(out string v3)) return false;
        Otp = v3;
        if (!ks.Get(out int v4)) return false;
        AuthLevel = v4;
        if (!ks.Get(out bool v5)) return false;
        InternalUser = v5;
        return true;
    }
}

public sealed class KDBE_GET_CONCURRENT_USER_LOGIN_REQ : IKSerializable
{
    public string Id { get; set; } = string.Empty;
    public string Pw { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutWideString(Id)) return false;
        if (!ks.PutWideString(Pw)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string v0)) return false;
        Id = v0;
        if (!ks.GetWideString(out string v1)) return false;
        Pw = v1;
        return true;
    }
}

public sealed class KDBE_GET_CONCURRENT_USER_LOGIN_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int AuthLevel { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(AuthLevel)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        Ok = v0;
        if (!ks.Get(out int v1)) return false;
        AuthLevel = v1;
        return true;
    }
}

public sealed class KDBE_GET_CONCURRENT_USER_INFO_REQ : IKSerializable
{
    public sbyte CcuType { get; set; }
    public long ServerUid { get; set; }
    public string Date { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(CcuType)) return false;
        if (!ks.Put(ServerUid)) return false;
        if (!ks.PutWideString(Date)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte v0)) return false;
        CcuType = v0;
        if (!ks.Get(out long v1)) return false;
        ServerUid = v1;
        if (!ks.GetWideString(out string v2)) return false;
        Date = v2;
        return true;
    }
}

public sealed class KDBE_GET_CONCURRENT_USER_INFO_ACK : IKSerializable
{
    public int Ok { get; set; }
    public sbyte CcuType { get; set; }
    public long ServerUid { get; set; }
    public string Date { get; set; } = string.Empty;
    public List<KCCUInfo> CcuInfo { get; set; } = [];
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(CcuType)) return false;
        if (!ks.Put(ServerUid)) return false;
        if (!ks.PutWideString(Date)) return false;
        if (!ks.PutList(CcuInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        Ok = v0;
        if (!ks.Get(out sbyte v1)) return false;
        CcuType = v1;
        if (!ks.Get(out long v2)) return false;
        ServerUid = v2;
        if (!ks.GetWideString(out string v3)) return false;
        Date = v3;
        if (!ks.GetList(CcuInfo)) return false;
        return true;
    }
}

public sealed class KDBE_CHECK_HACKING_TOOL_LIST_REQ : IKSerializable
{
    public int ReleaseTick { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ReleaseTick)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        ReleaseTick = v0;
        return true;
    }
}

public sealed class KDBE_CHECK_HACKING_TOOL_LIST_ACK : IKSerializable
{
    public int ReleaseTick { get; set; }
    public List<KHackingToolInfo> HackingToolList { get; set; } = [];
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ReleaseTick)) return false;
        if (!ks.PutList(HackingToolList)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        ReleaseTick = v0;
        if (!ks.GetList(HackingToolList)) return false;
        return true;
    }
}

public sealed class KDBE_GET_GAME_SERVER_LIST_ACK : IKSerializable
{
    public int ServerCount { get; set; }
    public List<KToolServerList> ServerList { get; set; } = [];
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ServerCount)) return false;
        if (!ks.PutList(ServerList)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        ServerCount = v0;
        if (!ks.GetList(ServerList)) return false;
        return true;
    }
}

public sealed class KDBE_UPDATE_CHANNEL_RANDOMKEY_REQ : IKSerializable
{
    public string UserId { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutWideString(UserId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string v0)) return false;
        UserId = v0;
        return true;
    }
}

public sealed class KDBE_UPDATE_CHANNEL_RANDOMKEY_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int RandomKey { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(RandomKey)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        Ok = v0;
        if (!ks.Get(out int v1)) return false;
        RandomKey = v1;
        return true;
    }
}

public sealed class KDBE_CHECK_ACCOUNT_BLOCK_LIST_REQ : IKSerializable
{
    public Dictionary<int, int> ReleaseTick { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutMap(ReleaseTick, static (s, v) => s.Put(v), static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetMap(ReleaseTick, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false;
        return true;
    }
}

public sealed class KDBE_CHECK_ACCOUNT_BLOCK_LIST_ACK : IKSerializable
{
    public Dictionary<int, int> ReleaseTick { get; set; } = new();
    public Dictionary<int, HashSet<long>> AccountBlockList { get; set; } = new();
    public List<string> MachineIdBlockList { get; set; } = [];
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutMap(ReleaseTick, static (s, v) => s.Put(v), static (s, v) => s.Put(v))) return false;
        if (!ks.PutMap(AccountBlockList, static (s, v) => s.Put(v), static (s1, hs) => s1.PutList(hs, static (s2, v) => s2.Put(v)))) return false;
        if (!ks.PutList(MachineIdBlockList, static (s, v) => s.PutWideString(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetMap(ReleaseTick, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false;
        if (!ks.GetMap(AccountBlockList, static s => (s.Get(out int v), v), static s1 => { var hs = new HashSet<long>(); return (s1.GetHashSet(hs, static s2 => (s2.Get(out long v), v)), hs); })) return false;
        if (!ks.GetList(MachineIdBlockList, static s => (s.GetWideString(out string v), v))) return false;
        return true;
    }
}

public sealed class KDBE_CHECK_SERVER_SN_REQ : IKSerializable
{
    public string Id { get; set; } = string.Empty;
    public long ServerSN { get; set; }
    public string MachineId { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutWideString(Id)) return false;
        if (!ks.Put(ServerSN)) return false;
        if (!ks.PutWideString(MachineId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string v0)) return false;
        Id = v0;
        if (!ks.Get(out long v1)) return false;
        ServerSN = v1;
        if (!ks.GetWideString(out string v2)) return false;
        MachineId = v2;
        return true;
    }
}

public sealed class KDBE_CHECK_SERVER_SN_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long ServerSN { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(ServerSN)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        Ok = v0;
        if (!ks.Get(out long v1)) return false;
        ServerSN = v1;
        return true;
    }
}

public sealed class KDBE_CREATE_SERVER_SN_REQ : IKSerializable
{
    public string MachineId { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutWideString(MachineId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string v0)) return false;
        MachineId = v0;
        return true;
    }
}

public sealed class KDBE_CREATE_SERVER_SN_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long ServerSN { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(ServerSN)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        Ok = v0;
        if (!ks.Get(out long v1)) return false;
        ServerSN = v1;
        return true;
    }
}

public sealed class KDBE_GET_TOTAL_CCU_FOR_WAIT_QUEUE_ACK : IKSerializable
{
    public int TotalCcu { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(TotalCcu)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        TotalCcu = v0;
        return true;
    }
}

public sealed class KDBE_TICK_PERFORMANCE_LOG_NOT : IKSerializable
{
    public long ServerUid { get; set; }
    public int ServerType { get; set; }
    public uint MaxTickCount { get; set; }
    public string MaxTickRegDate { get; set; } = string.Empty;
    public string RegDate { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ServerUid)) return false;
        if (!ks.Put(ServerType)) return false;
        if (!ks.Put(MaxTickCount)) return false;
        if (!ks.PutWideString(MaxTickRegDate)) return false;
        if (!ks.PutWideString(RegDate)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        ServerUid = v0;
        if (!ks.Get(out int v1)) return false;
        ServerType = v1;
        if (!ks.Get(out uint v2)) return false;
        MaxTickCount = v2;
        if (!ks.GetWideString(out string v3)) return false;
        MaxTickRegDate = v3;
        if (!ks.GetWideString(out string v4)) return false;
        RegDate = v4;
        return true;
    }
}

public sealed class KDBE_LOG_STATISTICS_INFO_NOT : IKSerializable
{
    public long ServerUid { get; set; }
    public List<KLogStatInfo> LogStats { get; set; } = [];
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ServerUid)) return false;
        if (!ks.PutList(LogStats)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        ServerUid = v0;
        if (!ks.GetList(LogStats)) return false;
        return true;
    }
}

public sealed class KDBE_SEND_PHONE_MSG_NOT : IKSerializable
{
    public List<string> PhoneNumbers { get; set; } = [];
    public string SmsMessage { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutList(PhoneNumbers, static (s, v) => s.PutWideString(v))) return false;
        if (!ks.PutWideString(SmsMessage)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetList(PhoneNumbers, static s => (s.GetWideString(out string v), v))) return false;
        if (!ks.GetWideString(out string v1)) return false;
        SmsMessage = v1;
        return true;
    }
}

// --- Cross-Server In-Process Routing Packets (Login / Center / Global) ---

public sealed class KELG_VERIFY_ACCOUNT_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long UserUid { get; set; }
    public string Id { get; set; } = string.Empty;
    public int AuthLevel { get; set; }
    public bool IsGuestUser { get; set; }
    public long ChannelUserUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(UserUid)) return false;
        if (!ks.PutWideString(Id)) return false;
        if (!ks.Put(AuthLevel)) return false;
        if (!ks.Put(IsGuestUser)) return false;
        if (!ks.Put(ChannelUserUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        Ok = v0;
        if (!ks.Get(out long v1)) return false;
        UserUid = v1;
        if (!ks.GetWideString(out string v2)) return false;
        Id = v2;
        if (!ks.Get(out int v3)) return false;
        AuthLevel = v3;
        if (!ks.Get(out bool v4)) return false;
        IsGuestUser = v4;
        if (!ks.Get(out long v5)) return false;
        ChannelUserUid = v5;
        return true;
    }
}

public sealed class KELG_USER_DISCONNECT_REQ : IKSerializable
{
    public long UserUid { get; set; }
    public string UserId { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
        => ks.Put(UserUid) && ks.PutWideString(UserId);

    public bool Get(KSerializer ks, RegionProfile region)
        => ks.Get(out long uid) && ks.GetWideString(out string id) && ((UserUid = uid) == uid) && ((UserId = id) == id);
}

public sealed class KELG_USER_DISCONNECT_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long UserUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(UserUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        Ok = v0;
        if (!ks.Get(out long v1)) return false;
        UserUid = v1;
        return true;
    }
}

public sealed class KELG_REGISTER_UNIT_NOT : IKSerializable
{
    public long UserUid { get; set; }
    public long UnitUid { get; set; }
    public string NickName { get; set; } = string.Empty;
    public int GuildUid { get; set; }
    public long GameServerUid { get; set; }
    public int ChannelId { get; set; } = 1;
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UserUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutWideString(NickName)) return false;
        if (!ks.Put(GuildUid)) return false;
        if (!ks.Put(GameServerUid)) return false;
        if (!ks.Put(ChannelId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        UserUid = v0;
        if (!ks.Get(out long v1)) return false;
        UnitUid = v1;
        if (!ks.GetWideString(out string v2)) return false;
        NickName = v2;
        if (!ks.Get(out int v3)) return false;
        GuildUid = v3;
        if (!ks.Get(out long v4)) return false;
        GameServerUid = v4;
        if (!ks.Get(out int v5)) return false;
        ChannelId = v5;
        return true;
    }
}

public sealed class KELG_ADMIN_KICK_USER_REQ : IKSerializable
{
    public long TargetUserUid { get; set; }
    public long TargetUnitUid { get; set; }
    public string Reason { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(TargetUserUid)) return false;
        if (!ks.Put(TargetUnitUid)) return false;
        if (!ks.PutWideString(Reason)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        TargetUserUid = v0;
        if (!ks.Get(out long v1)) return false;
        TargetUnitUid = v1;
        if (!ks.GetWideString(out string v2)) return false;
        Reason = v2;
        return true;
    }
}

public sealed class KELG_ADMIN_KICK_USER_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long TargetUserUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(TargetUserUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        Ok = v0;
        if (!ks.Get(out long v1)) return false;
        TargetUserUid = v1;
        return true;
    }
}

public sealed class KELG_CREATE_GUILD_REQ : IKSerializable
{
    public long GameServerUid { get; set; }
    public long MasterUnitUid { get; set; }
    public string GuildName { get; set; } = string.Empty;
    public string GuildMessage { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(GameServerUid)) return false;
        if (!ks.Put(MasterUnitUid)) return false;
        if (!ks.PutWideString(GuildName)) return false;
        if (!ks.PutWideString(GuildMessage)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        GameServerUid = v0;
        if (!ks.Get(out long v1)) return false;
        MasterUnitUid = v1;
        if (!ks.GetWideString(out string v2)) return false;
        GuildName = v2;
        if (!ks.GetWideString(out string v3)) return false;
        GuildMessage = v3;
        return true;
    }
}

public sealed class KELG_CREATE_GUILD_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long MasterUnitUid { get; set; }
    public KGuildInfo GuildInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
        => ks.Put(Ok) && ks.Put(MasterUnitUid) && ks.Put(GuildInfo);

    public bool Get(KSerializer ks, RegionProfile region)
        => ks.Get(out int ok) && ks.Get(out long muid) && ks.Get(GuildInfo) && ((Ok = ok) == ok) && ((MasterUnitUid = muid) == muid);
}

public sealed class KELG_GET_GUILD_INFO_REQ : IKSerializable
{
    public long GameServerUid { get; set; }
    public long RequesterUnitUid { get; set; }
    public int GuildUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(GameServerUid)) return false;
        if (!ks.Put(RequesterUnitUid)) return false;
        if (!ks.Put(GuildUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        GameServerUid = v0;
        if (!ks.Get(out long v1)) return false;
        RequesterUnitUid = v1;
        if (!ks.Get(out int v2)) return false;
        GuildUid = v2;
        return true;
    }
}

public sealed class KELG_GET_GUILD_INFO_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long RequesterUnitUid { get; set; }
    public KGuildInfo GuildInfo { get; set; } = new();
    public KGuildSkillInfo GuildSkillInfo { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(RequesterUnitUid)) return false;
        if (!ks.Put(GuildInfo)) return false;
        if (!ks.Put(GuildSkillInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        Ok = v0;
        if (!ks.Get(out long v1)) return false;
        RequesterUnitUid = v1;
        if (!ks.Get(GuildInfo)) return false;
        if (!ks.Get(GuildSkillInfo)) return false;
        return true;
    }
}

public sealed class KELG_UPDATE_GUILD_EXP_NOT : IKSerializable
{
    public int GuildUid { get; set; }
    public long UnitUid { get; set; }
    public int DeltaExp { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(GuildUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(DeltaExp)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        GuildUid = v0;
        if (!ks.Get(out long v1)) return false;
        UnitUid = v1;
        if (!ks.Get(out int v2)) return false;
        DeltaExp = v2;
        return true;
    }
}

public sealed class KELG_LOCAL_RANKING_CHANGED_RANKER_INFO_NOT : IKSerializable
{
    public int SystemUid { get; set; } = 1;
    public KLocalRankingUserInfo UserInfo { get; set; } = new();
    public KLocalRankingUnitInfo UnitInfo { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(SystemUid)) return false;
        if (!ks.Put(UserInfo)) return false;
        if (!ks.Put(UnitInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        SystemUid = v0;
        if (!ks.Get(UserInfo)) return false;
        if (!ks.Get(UnitInfo)) return false;
        return true;
    }
}

public sealed class KELG_NEW_RECORD_HENIR_RANKING_NOT : IKSerializable
{
    public KHenirRankingInfo RankingInfo { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(RankingInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(RankingInfo)) return false;
        return true;
    }
}

public sealed class KEGB_REQUEST_MATCH_MAKING_REQ : IKSerializable
{
    public bool RegFrontOfList { get; set; }
    public sbyte PvpPlayerCount { get; set; }
    public sbyte PvpGameType { get; set; }
    public List<KMatchUserInfo> MatchUserInfo { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(RegFrontOfList)) return false;
        if (!ks.Put(PvpPlayerCount)) return false;
        if (!ks.Put(PvpGameType)) return false;
        if (!ks.PutList(MatchUserInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool v0)) return false;
        RegFrontOfList = v0;
        if (!ks.Get(out sbyte v1)) return false;
        PvpPlayerCount = v1;
        if (!ks.Get(out sbyte v2)) return false;
        PvpGameType = v2;
        if (!ks.GetList(MatchUserInfo)) return false;
        return true;
    }
}

public sealed class KEGB_CANCEL_MATCH_MAKING_REQ : IKSerializable
{
    public long MatchUid { get; set; }
    public long PartyUid { get; set; }
    public int Reason { get; set; }
    public bool SendBySystem { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(MatchUid)) return false;
        if (!ks.Put(PartyUid)) return false;
        if (!ks.Put(Reason)) return false;
        if (!ks.Put(SendBySystem)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        MatchUid = v0;
        if (!ks.Get(out long v1)) return false;
        PartyUid = v1;
        if (!ks.Get(out int v2)) return false;
        Reason = v2;
        if (!ks.Get(out bool v3)) return false;
        SendBySystem = v3;
        return true;
    }
}

public sealed class KEGB_MATCH_MAKING_SUCCESS_REPLY_NOT : IKSerializable
{
    public long MatchUid { get; set; }
    public long UnitUid { get; set; }
    public bool Accept { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(MatchUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(Accept)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        MatchUid = v0;
        if (!ks.Get(out long v1)) return false;
        UnitUid = v1;
        if (!ks.Get(out bool v2)) return false;
        Accept = v2;
        return true;
    }
}

public sealed class KEGB_REQUEST_AUTO_PARTY_MAKING_REQ : IKSerializable
{
    public bool RemakingAutoParty { get; set; }
    public KAutoPartyConditionInfo AutoPartyCondition { get; set; } = new();
    public List<KAutoPartyUserInfo> AutoPartyUser { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(RemakingAutoParty)) return false;
        if (!ks.Put(AutoPartyCondition)) return false;
        if (!ks.PutList(AutoPartyUser)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool v0)) return false;
        RemakingAutoParty = v0;
        if (!ks.Get(AutoPartyCondition)) return false;
        if (!ks.GetList(AutoPartyUser)) return false;
        return true;
    }
}

public sealed class KEGB_CANCEL_AUTO_PARTY_MAKING_REQ : IKSerializable
{
    public long AutoPartyUid { get; set; }
    public long AutoPartyWaitNumber { get; set; }
    public int Reason { get; set; }
    public bool SendBySystem { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(AutoPartyUid)) return false;
        if (!ks.Put(AutoPartyWaitNumber)) return false;
        if (!ks.Put(Reason)) return false;
        if (!ks.Put(SendBySystem)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        AutoPartyUid = v0;
        if (!ks.Get(out long v1)) return false;
        AutoPartyWaitNumber = v1;
        if (!ks.Get(out int v2)) return false;
        Reason = v2;
        if (!ks.Get(out bool v3)) return false;
        SendBySystem = v3;
        return true;
    }
}

public sealed class KEGB_AUTO_PARTY_MAKING_SUCCESS_REPLY_NOT : IKSerializable
{
    public long AutoPartyUid { get; set; }
    public long UnitUid { get; set; }
    public bool Accept { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(AutoPartyUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(Accept)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        AutoPartyUid = v0;
        if (!ks.Get(out long v1)) return false;
        UnitUid = v1;
        if (!ks.Get(out bool v2)) return false;
        Accept = v2;
        return true;
    }
}

public sealed class KEGB_INCREASE_THE_PROTECTED_CRYSTAL_COUNT_NOT : IKSerializable
{
    public int DeltaCrystalCount { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(DeltaCrystalCount)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        DeltaCrystalCount = v0;
        return true;
    }
}

public sealed class KEGB_GET_MISSION_INFO_REQ : IKSerializable
{
    public long RequesterServerUid { get; set; }
    public int FlagType { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(RequesterServerUid)) return false;
        if (!ks.Put(FlagType)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        RequesterServerUid = v0;
        if (!ks.Get(out int v1)) return false;
        FlagType = v1;
        return true;
    }
}

public sealed class KEGB_GET_MISSION_INFO_ACK : IKSerializable
{
    public bool IsActive { get; set; }
    public int StartedCrystalCount { get; set; }
    public int ProtectedCrystalCount { get; set; }
    public int WorldBuffId { get; set; }
    public int IndividualBuffLevel { get; set; }
    public int RemainingSeconds { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(IsActive)) return false;
        if (!ks.Put(StartedCrystalCount)) return false;
        if (!ks.Put(ProtectedCrystalCount)) return false;
        if (!ks.Put(WorldBuffId)) return false;
        if (!ks.Put(IndividualBuffLevel)) return false;
        if (!ks.Put(RemainingSeconds)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool v0)) return false;
        IsActive = v0;
        if (!ks.Get(out int v1)) return false;
        StartedCrystalCount = v1;
        if (!ks.Get(out int v2)) return false;
        ProtectedCrystalCount = v2;
        if (!ks.Get(out int v3)) return false;
        WorldBuffId = v3;
        if (!ks.Get(out int v4)) return false;
        IndividualBuffLevel = v4;
        if (!ks.Get(out int v5)) return false;
        RemainingSeconds = v5;
        return true;
    }
}

public sealed class KEGB_UPDATE_GLOBAL_MISSION_INFO_NOT : IKSerializable
{
    public int MissionId { get; set; }
    public long DeltaCount { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(MissionId)) return false;
        if (!ks.Put(DeltaCount)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        MissionId = v0;
        if (!ks.Get(out long v1)) return false;
        DeltaCount = v1;
        return true;
    }
}

public sealed class KERM_DUNGEON_SECURITY_REPORT_NOT : IKSerializable
{
    public long RoomUid { get; set; }
    public int DungeonId { get; set; }
    public int PlaySeconds { get; set; }
    public long UnitUid { get; set; }
    public int MaxSingleDamage { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(RoomUid)) return false;
        if (!ks.Put(DungeonId)) return false;
        if (!ks.Put(PlaySeconds)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(MaxSingleDamage)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        RoomUid = v0;
        if (!ks.Get(out int v1)) return false;
        DungeonId = v1;
        if (!ks.Get(out int v2)) return false;
        PlaySeconds = v2;
        if (!ks.Get(out long v3)) return false;
        UnitUid = v3;
        if (!ks.Get(out int v4)) return false;
        MaxSingleDamage = v4;
        return true;
    }
}

public sealed class KEGB_VERIFY_SERVER_CONNECT_REQ : IKSerializable
{
    public long ServerUid { get; set; }
    public int ServerGroupId { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ServerUid)) return false;
        if (!ks.Put(ServerGroupId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        ServerUid = v0;
        if (!ks.Get(out int v1)) return false;
        ServerGroupId = v1;
        return true;
    }
}

public sealed class KEGB_UPDATE_CCU_INFO_NOT : IKSerializable
{
    public long ServerUid { get; set; }
    public int ServerGroupId { get; set; }
    public int ConcurrentUser { get; set; }
    public int CurPartyCount { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ServerUid)) return false;
        if (!ks.Put(ServerGroupId)) return false;
        if (!ks.Put(ConcurrentUser)) return false;
        if (!ks.Put(CurPartyCount)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        ServerUid = v0;
        if (!ks.Get(out int v1)) return false;
        ServerGroupId = v1;
        if (!ks.Get(out int v2)) return false;
        ConcurrentUser = v2;
        if (!ks.Get(out int v3)) return false;
        CurPartyCount = v3;
        return true;
    }
}

public sealed class KEGB_REGISTER_UNIT_NOT : IKSerializable
{
    public long ServerSn { get; set; }
    public long UserUid { get; set; }
    public long UnitUid { get; set; }
    public string NickName { get; set; } = string.Empty;
    public long GameServerUid { get; set; }
    public long ServerGroupId { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ServerSn)) return false;
        if (!ks.Put(UserUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutWideString(NickName)) return false;
        if (!ks.Put(GameServerUid)) return false;
        if (!ks.Put(ServerGroupId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        ServerSn = v0;
        if (!ks.Get(out long v1)) return false;
        UserUid = v1;
        if (!ks.Get(out long v2)) return false;
        UnitUid = v2;
        if (!ks.GetWideString(out string v3)) return false;
        NickName = v3;
        if (!ks.Get(out long v4)) return false;
        GameServerUid = v4;
        if (!ks.Get(out long v5)) return false;
        ServerGroupId = v5;
        return true;
    }
}

public sealed class KEGB_UNIT_DISCONNECT_NOT : IKSerializable
{
    public long ServerSn { get; set; }
    public long UserUid { get; set; }
    public long UnitUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ServerSn)) return false;
        if (!ks.Put(UserUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        ServerSn = v0;
        if (!ks.Get(out long v1)) return false;
        UserUid = v1;
        if (!ks.Get(out long v2)) return false;
        UnitUid = v2;
        return true;
    }
}

public sealed class KEPM_CREATE_PARTY_BY_INVITE_REQ : IKSerializable
{
    public long GameServerUid { get; set; }
    public KCreatePartyInfo CreatePartyInfo { get; set; } = new();
    public KPartyUserInfo HostInfo { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(GameServerUid)) return false;
        if (!ks.Put(CreatePartyInfo)) return false;
        if (!ks.Put(HostInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        GameServerUid = v0;
        if (!ks.Get(CreatePartyInfo)) return false;
        if (!ks.Get(HostInfo)) return false;
        return true;
    }
}

public sealed class KEPM_LEAVE_PARTY_REQ : IKSerializable
{
    public long PartyUid { get; set; }
    public long UnitUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PartyUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        PartyUid = v0;
        if (!ks.Get(out long v1)) return false;
        UnitUid = v1;
        return true;
    }
}

public sealed class KERM_OPEN_TC_ROOM_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public long GameServerUid { get; set; }
    public int TcId { get; set; }
    public int DungeonId { get; set; }
    public float PlayTimeLimit { get; set; } = 300f;
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(GameServerUid)) return false;
        if (!ks.Put(TcId)) return false;
        if (!ks.Put(DungeonId)) return false;
        if (!ks.Put(PlayTimeLimit)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        UnitUid = v0;
        if (!ks.Get(out long v1)) return false;
        GameServerUid = v1;
        if (!ks.Get(out int v2)) return false;
        TcId = v2;
        if (!ks.Get(out int v3)) return false;
        DungeonId = v3;
        if (!ks.Get(out float v4)) return false;
        PlayTimeLimit = v4;
        return true;
    }
}

public sealed class KEGS_ADD_COMMON_FLAG_NOT : IKSerializable
{
    public uint Flag { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Flag)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint v0)) return false;
        Flag = v0;
        return true;
    }
}

public sealed class KEGS_DEL_COMMON_FLAG_NOT : IKSerializable
{
    public uint Flag { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Flag)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint v0)) return false;
        Flag = v0;
        return true;
    }
}

public sealed class KECN_ADD_COMMON_FLAG_NOT : IKSerializable
{
    public uint Flag { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Flag)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint v0)) return false;
        Flag = v0;
        return true;
    }
}

public sealed class KECN_DEL_COMMON_FLAG_NOT : IKSerializable
{
    public uint Flag { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Flag)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint v0)) return false;
        Flag = v0;
        return true;
    }
}

public sealed class KE_DISCONNECT_SERVER_REPORT_NOT : IKSerializable
{
    public sbyte Type { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Type)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte v0)) return false;
        Type = v0;
        return true;
    }
}

public sealed class KEGB_BATTLE_FIELD_BOSS_INFO_NOT : IKSerializable
{
    public long RemainPortalTime { get; set; }
    public long RemainCoolTime { get; set; }
    public int PortalAppearanceMap { get; set; }
    public int PortalDestination { get; set; }
    public bool PortalOpen { get; set; }
    public bool CoolTime { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(RemainPortalTime)) return false;
        if (!ks.Put(RemainCoolTime)) return false;
        if (!ks.Put(PortalAppearanceMap)) return false;
        if (!ks.Put(PortalDestination)) return false;
        if (!ks.Put(PortalOpen)) return false;
        if (!ks.Put(CoolTime)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        RemainPortalTime = v0;
        if (!ks.Get(out long v1)) return false;
        RemainCoolTime = v1;
        if (!ks.Get(out int v2)) return false;
        PortalAppearanceMap = v2;
        if (!ks.Get(out int v3)) return false;
        PortalDestination = v3;
        if (!ks.Get(out bool v4)) return false;
        PortalOpen = v4;
        if (!ks.Get(out bool v5)) return false;
        CoolTime = v5;
        return true;
    }
}

public sealed class KEGB_GET_GLOBAL_MISSION_INFO_REQ : IKSerializable
{
    public long RequesterServerUid { get; set; }
    public int MissionId { get; set; } = 1;
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(RequesterServerUid)) return false;
        if (!ks.Put(MissionId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        RequesterServerUid = v0;
        if (!ks.Get(out int v1)) return false;
        MissionId = v1;
        return true;
    }
}

public sealed class KEGB_GET_GLOBAL_MISSION_INFO_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int MissionId { get; set; }
    public string Title { get; set; } = string.Empty;
    public long CurrentCount { get; set; }
    public long TargetCount { get; set; }
    public bool IsActive { get; set; }
    public bool? Succeeded { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(MissionId)) return false;
        if (!ks.PutWideString(Title)) return false;
        if (!ks.Put(CurrentCount)) return false;
        if (!ks.Put(TargetCount)) return false;
        if (!ks.Put(IsActive)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        Ok = v0;
        if (!ks.Get(out int v1)) return false;
        MissionId = v1;
        if (!ks.GetWideString(out string v2)) return false;
        Title = v2;
        if (!ks.Get(out long v3)) return false;
        CurrentCount = v3;
        if (!ks.Get(out long v4)) return false;
        TargetCount = v4;
        if (!ks.Get(out bool v5)) return false;
        IsActive = v5;
        return true;
    }
}

public sealed class KECN_MEGAPHONE_MESSAGE_NOT : IKSerializable
{
    public long SenderUnitUid { get; set; }
    public string SenderNickName { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public int ItemId { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(SenderUnitUid)) return false;
        if (!ks.PutWideString(SenderNickName)) return false;
        if (!ks.PutWideString(Message)) return false;
        if (!ks.Put(ItemId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        SenderUnitUid = v0;
        if (!ks.GetWideString(out string v1)) return false;
        SenderNickName = v1;
        if (!ks.GetWideString(out string v2)) return false;
        Message = v2;
        if (!ks.Get(out int v3)) return false;
        ItemId = v3;
        return true;
    }
}

public sealed class KECN_NOTIFY_MSG_NOT : IKSerializable
{
    public sbyte NotifyType { get; set; }
    public string Message { get; set; } = string.Empty;
    public int Count { get; set; } = 1;
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(NotifyType)) return false;
        if (!ks.PutWideString(Message)) return false;
        if (!ks.Put(Count)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte v0)) return false;
        NotifyType = v0;
        if (!ks.GetWideString(out string v1)) return false;
        Message = v1;
        if (!ks.Get(out int v2)) return false;
        Count = v2;
        return true;
    }
}

public sealed class KAutoPartyBonusInfo : IKSerializable
{
    public sbyte DungeonMode { get; set; }
    public int DungeonIdWithDif { get; set; }
    public bool ActivatedBonus { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(DungeonMode)) return false;
        if (!ks.Put(DungeonIdWithDif)) return false;
        if (!ks.Put(ActivatedBonus)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte v0)) return false;
        DungeonMode = v0;
        if (!ks.Get(out int v1)) return false;
        DungeonIdWithDif = v1;
        if (!ks.Get(out bool v2)) return false;
        ActivatedBonus = v2;
        return true;
    }
}

public sealed class KEGB_GET_AUTO_PARTY_BONUS_INFO_ACK : IKSerializable
{
    public List<KAutoPartyBonusInfo> AutoPartyBonusInfo { get; set; } = [];
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutList(AutoPartyBonusInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetList(AutoPartyBonusInfo)) return false;
        return true;
    }
}

public sealed class KELG_UNIT_DISCONNECT_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public HashSet<long> FriendUids { get; set; } = [];
    public int GuildUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutList(FriendUids, static (s, v) => s.Put(v))) return false;
        if (!ks.Put(GuildUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        UnitUid = v0;
        if (!ks.GetHashSet(FriendUids, static s => (s.Get(out long v), v))) return false;
        if (!ks.Get(out int v2)) return false;
        GuildUid = v2;
        return true;
    }
}

public sealed class KELG_ACCEPT_FRIEND_NOT : IKSerializable
{
    public long DestUnitUid { get; set; }
    public long UnitUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(DestUnitUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        DestUnitUid = v0;
        if (!ks.Get(out long v1)) return false;
        UnitUid = v1;
        return true;
    }
}

public sealed class KELG_BLOCK_FRIEND_NOT : IKSerializable
{
    public long DestUnitUid { get; set; }
    public long UnitUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(DestUnitUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        DestUnitUid = v0;
        if (!ks.Get(out long v1)) return false;
        UnitUid = v1;
        return true;
    }
}

public sealed class KELG_CALL_MY_LOVER_FIND_USER_VILLAGE_REQ : IKSerializable
{
    public int MapId { get; set; }
    public KVector3 Pos { get; set; } = new();
    public int LastTouchLineIndex { get; set; }
    public long CalledUserUid { get; set; }
    public long CalledGsUid { get; set; }
    public int ChannelId { get; set; }
    public long LoverUnitUid { get; set; }
    public sbyte CallType { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(MapId)) return false;
        if (!ks.Put(Pos)) return false;
        if (!ks.Put(LastTouchLineIndex)) return false;
        if (!ks.Put(CalledUserUid)) return false;
        if (!ks.Put(CalledGsUid)) return false;
        if (!ks.Put(ChannelId)) return false;
        if (!ks.Put(LoverUnitUid)) return false;
        if (!ks.Put(CallType)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        MapId = v0;
        if (!ks.Get(Pos)) return false;
        if (!ks.Get(out int v2)) return false;
        LastTouchLineIndex = v2;
        if (!ks.Get(out long v3)) return false;
        CalledUserUid = v3;
        if (!ks.Get(out long v4)) return false;
        CalledGsUid = v4;
        if (!ks.Get(out int v5)) return false;
        ChannelId = v5;
        if (!ks.Get(out long v6)) return false;
        LoverUnitUid = v6;
        if (!ks.Get(out sbyte v7)) return false;
        CallType = v7;
        return true;
    }
}

public sealed class KELG_CALL_MY_LOVER_FIND_USER_VILLAGE_ACK : IKSerializable
{
    public int Ok { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        Ok = v0;
        return true;
    }
}

public sealed class KELG_CALL_MY_LOVER_AGREE_RESULT_VILLAGE_NOT : IKSerializable
{
    public bool Agree { get; set; }
    public int MapId { get; set; }
    public KVector3 Pos { get; set; } = new();
    public int LastTouchLineIndex { get; set; }
    public long CalledUserUid { get; set; }
    public long CalledGsUid { get; set; }
    public long MyUserUid { get; set; }
    public long MyGsUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Agree)) return false;
        if (!ks.Put(MapId)) return false;
        if (!ks.Put(Pos)) return false;
        if (!ks.Put(LastTouchLineIndex)) return false;
        if (!ks.Put(CalledUserUid)) return false;
        if (!ks.Put(CalledGsUid)) return false;
        if (!ks.Put(MyUserUid)) return false;
        if (!ks.Put(MyGsUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool v0)) return false;
        Agree = v0;
        if (!ks.Get(out int v1)) return false;
        MapId = v1;
        if (!ks.Get(Pos)) return false;
        if (!ks.Get(out int v3)) return false;
        LastTouchLineIndex = v3;
        if (!ks.Get(out long v4)) return false;
        CalledUserUid = v4;
        if (!ks.Get(out long v5)) return false;
        CalledGsUid = v5;
        if (!ks.Get(out long v6)) return false;
        MyUserUid = v6;
        if (!ks.Get(out long v7)) return false;
        MyGsUid = v7;
        return true;
    }
}

public sealed class KELG_CALL_MY_LOVER_STATE_CHANGE_FIELD_NOT : IKSerializable
{
    public bool UseWarpItem { get; set; }
    public int MapId { get; set; }
    public KVector3 Pos { get; set; } = new();
    public int LastTouchLineIndex { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UseWarpItem)) return false;
        if (!ks.Put(MapId)) return false;
        if (!ks.Put(Pos)) return false;
        if (!ks.Put(LastTouchLineIndex)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool v0)) return false;
        UseWarpItem = v0;
        if (!ks.Get(out int v1)) return false;
        MapId = v1;
        if (!ks.Get(Pos)) return false;
        if (!ks.Get(out int v3)) return false;
        LastTouchLineIndex = v3;
        return true;
    }
}

public sealed class KELG_CALL_MY_LOVER_FIND_USER_ROOM_REQ : IKSerializable
{
    public int Ok { get; set; }
    public KBattleFieldJoinInfo BtInfo { get; set; } = new();
    public KVector3 Pos { get; set; } = new();
    public int LastTouchLineIndex { get; set; }
    public long LoverUnitUid { get; set; }
    public sbyte CallType { get; set; }
    public int ChannelId { get; set; }
    public long CalledUserUid { get; set; }
    public long CalledGsUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(BtInfo)) return false;
        if (!ks.Put(Pos)) return false;
        if (!ks.Put(LastTouchLineIndex)) return false;
        if (!ks.Put(LoverUnitUid)) return false;
        if (!ks.Put(CallType)) return false;
        if (!ks.Put(ChannelId)) return false;
        if (!ks.Put(CalledUserUid)) return false;
        if (!ks.Put(CalledGsUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        Ok = v0;
        if (!ks.Get(BtInfo)) return false;
        if (!ks.Get(Pos)) return false;
        if (!ks.Get(out int v3)) return false;
        LastTouchLineIndex = v3;
        if (!ks.Get(out long v4)) return false;
        LoverUnitUid = v4;
        if (!ks.Get(out sbyte v5)) return false;
        CallType = v5;
        if (!ks.Get(out int v6)) return false;
        ChannelId = v6;
        if (!ks.Get(out long v7)) return false;
        CalledUserUid = v7;
        if (!ks.Get(out long v8)) return false;
        CalledGsUid = v8;
        return true;
    }
}

public sealed class KELG_CALL_MY_LOVER_FIND_USER_ROOM_ACK : IKSerializable
{
    public int Ok { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        Ok = v0;
        return true;
    }
}

public sealed class KELG_CALL_MY_LOVER_AGREE_RESULT_ROOM_NOT : IKSerializable
{
    public bool Agree { get; set; }
    public long CalledUserUid { get; set; }
    public long CalledGsUid { get; set; }
    public long MyUserUid { get; set; }
    public long MyGsUid { get; set; }
    public int BattleFieldId { get; set; }
    public long RoomUid { get; set; }
    public KVector3 Pos { get; set; } = new();
    public int LastTouchLineIndex { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Agree)) return false;
        if (!ks.Put(CalledUserUid)) return false;
        if (!ks.Put(CalledGsUid)) return false;
        if (!ks.Put(MyUserUid)) return false;
        if (!ks.Put(MyGsUid)) return false;
        if (!ks.Put(BattleFieldId)) return false;
        if (!ks.Put(RoomUid)) return false;
        if (!ks.Put(Pos)) return false;
        if (!ks.Put(LastTouchLineIndex)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool v0)) return false;
        Agree = v0;
        if (!ks.Get(out long v1)) return false;
        CalledUserUid = v1;
        if (!ks.Get(out long v2)) return false;
        CalledGsUid = v2;
        if (!ks.Get(out long v3)) return false;
        MyUserUid = v3;
        if (!ks.Get(out long v4)) return false;
        MyGsUid = v4;
        if (!ks.Get(out int v5)) return false;
        BattleFieldId = v5;
        if (!ks.Get(out long v6)) return false;
        RoomUid = v6;
        if (!ks.Get(Pos)) return false;
        if (!ks.Get(out int v8)) return false;
        LastTouchLineIndex = v8;
        return true;
    }
}

public sealed class KELG_CALL_MY_LOVER_JOIN_BATTLE_FIELD_NOT : IKSerializable
{
    public long CalledUserUid { get; set; }
    public long CalledUnitUid { get; set; }
    public long CalledGsUid { get; set; }
    public int BattleFieldId { get; set; }
    public long RoomUid { get; set; }
    public KVector3 Pos { get; set; } = new();
    public int LastTouchLineIndex { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(CalledUserUid)) return false;
        if (!ks.Put(CalledUnitUid)) return false;
        if (!ks.Put(CalledGsUid)) return false;
        if (!ks.Put(BattleFieldId)) return false;
        if (!ks.Put(RoomUid)) return false;
        if (!ks.Put(Pos)) return false;
        if (!ks.Put(LastTouchLineIndex)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        CalledUserUid = v0;
        if (!ks.Get(out long v1)) return false;
        CalledUnitUid = v1;
        if (!ks.Get(out long v2)) return false;
        CalledGsUid = v2;
        if (!ks.Get(out int v3)) return false;
        BattleFieldId = v3;
        if (!ks.Get(out long v4)) return false;
        RoomUid = v4;
        if (!ks.Get(Pos)) return false;
        if (!ks.Get(out int v6)) return false;
        LastTouchLineIndex = v6;
        return true;
    }
}

public sealed class KELG_CALL_MY_LOVER_JOIN_BATTLE_FIELD_FAILD_NOT : IKSerializable
{
    public int Ok { get; set; }
    public long CalledUserUid { get; set; }
    public long CalledGsUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(CalledUserUid)) return false;
        if (!ks.Put(CalledGsUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        Ok = v0;
        if (!ks.Get(out long v1)) return false;
        CalledUserUid = v1;
        if (!ks.Get(out long v2)) return false;
        CalledGsUid = v2;
        return true;
    }
}

// ---- Batch 1: GlobalServer EGB_* packets ----

public sealed class KEGB_REGISTER_SERVERSN_REQ : IKSerializable
{
    public long UserUid { get; set; }
    public long ServerSn { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UserUid)) return false;
        if (!ks.Put(ServerSn)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        UserUid = v0;
        if (!ks.Get(out long v1)) return false;
        ServerSn = v1;
        return true;
    }
}

public sealed class KEGB_REGISTER_SERVERSN_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long UserUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(UserUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        Ok = v0;
        if (!ks.Get(out long v1)) return false;
        UserUid = v1;
        return true;
    }
}

public sealed class KEGB_UNREGISTER_SERVERSN_NOT : IKSerializable
{
    public long ServerSn { get; set; }
    public long UserUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ServerSn)) return false;
        if (!ks.Put(UserUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        ServerSn = v0;
        if (!ks.Get(out long v1)) return false;
        UserUid = v1;
        return true;
    }
}

public sealed class KEGB_UPDATE_TOTAL_DANGEROUS_VALUE_NOT : IKSerializable
{
    public int BattleFieldId { get; set; }
    public byte IncreasedValue { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(BattleFieldId)) return false;
        if (!ks.Put(IncreasedValue)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        BattleFieldId = v0;
        if (!ks.Get(out byte v1)) return false;
        IncreasedValue = v1;
        return true;
    }
}

public sealed class KEGB_REQUEST_AUTO_PARTY_WAIT_USER_COUNT_NOT : IKSerializable
{
    public int AutoPartyWaitNumber { get; set; }
    public int DungeonId { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(AutoPartyWaitNumber)) return false;
        if (!ks.Put(DungeonId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        AutoPartyWaitNumber = v0;
        if (!ks.Get(out int v1)) return false;
        DungeonId = v1;
        return true;
    }
}

public sealed class KEGB_MATCH_MAKING_ADMIN_COMMAND_NOT : IKSerializable
{
    public const sbyte CtDump = 0;
    public const sbyte CtClear = 1;
    public sbyte CommandType { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(CommandType)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte v0)) return false;
        CommandType = v0;
        return true;
    }
}

public sealed class KEGB_AUTO_PARTY_MAKING_ADMIN_COMMAND_NOT : IKSerializable
{
    public const sbyte CtDump = 0;
    public const sbyte CtClear = 1;
    public sbyte CommandType { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(CommandType)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte v0)) return false;
        CommandType = v0;
        return true;
    }
}

public sealed class KEGB_ADMIN_HERO_PVP_USER_TOURNAMENT_CONNECTION_NOT : IKSerializable
{
    public int ServerGroupId { get; set; } = -1;
    public string NickName { get; set; } = string.Empty;
    public bool Connect { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ServerGroupId)) return false;
        if (!ks.PutWideString(NickName)) return false;
        if (!ks.Put(Connect)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        ServerGroupId = v0;
        if (!ks.GetWideString(out string v1)) return false;
        NickName = v1;
        if (!ks.Get(out bool v2)) return false;
        Connect = v2;
        return true;
    }
}

public sealed class KEGB_EXCHANGE_LIMIT_REQ : IKSerializable
{
    public long GsUid { get; set; }
    public long UnitUid { get; set; }
    public int AfterWorkStorageKey { get; set; }
    public int DestItemId { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(GsUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(AfterWorkStorageKey)) return false;
        if (!ks.Put(DestItemId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        GsUid = v0;
        if (!ks.Get(out long v1)) return false;
        UnitUid = v1;
        if (!ks.Get(out int v2)) return false;
        AfterWorkStorageKey = v2;
        if (!ks.Get(out int v3)) return false;
        DestItemId = v3;
        return true;
    }
}

public sealed class KEGB_EXCHANGE_LIMIT_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int ExchangeLimitUid { get; set; }
    public int LimitUid { get; set; }
    public int RollBackUid { get; set; }
    public int AfterWorkStorageKey { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(ExchangeLimitUid)) return false;
        if (!ks.Put(LimitUid)) return false;
        if (!ks.Put(RollBackUid)) return false;
        if (!ks.Put(AfterWorkStorageKey)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        Ok = v0;
        if (!ks.Get(out int v1)) return false;
        ExchangeLimitUid = v1;
        if (!ks.Get(out int v2)) return false;
        LimitUid = v2;
        if (!ks.Get(out int v3)) return false;
        RollBackUid = v3;
        if (!ks.Get(out int v4)) return false;
        AfterWorkStorageKey = v4;
        return true;
    }
}

public sealed class KEGB_EXCHANGE_LIMIT_INFO_ROLLBACK_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public int ExchangeLimitUid { get; set; }
    public int LimitUid { get; set; }
    public int RollBackUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(ExchangeLimitUid)) return false;
        if (!ks.Put(LimitUid)) return false;
        if (!ks.Put(RollBackUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        UnitUid = v0;
        if (!ks.Get(out int v1)) return false;
        ExchangeLimitUid = v1;
        if (!ks.Get(out int v2)) return false;
        LimitUid = v2;
        if (!ks.Get(out int v3)) return false;
        RollBackUid = v3;
        return true;
    }
}

public sealed class KEGB_TIME_ENCHANT_EVENT_NOT : IKSerializable
{
    public long GsUid { get; set; }
    public long UnitUid { get; set; }
    public int AfterWorkStorageKey { get; set; }
    public ushort EventId { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(GsUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(AfterWorkStorageKey)) return false;
        if (!ks.Put(EventId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        GsUid = v0;
        if (!ks.Get(out long v1)) return false;
        UnitUid = v1;
        if (!ks.Get(out int v2)) return false;
        AfterWorkStorageKey = v2;
        if (!ks.Get(out ushort v3)) return false;
        EventId = v3;
        return true;
    }
}

public sealed class KTimeEnchantEventResult : IKSerializable
{
    public int AfterWorkStorageKey { get; set; }
    public sbyte TimeEnchantEventLevel { get; set; }
    public bool NotifyGetItem { get; set; } = true;
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(AfterWorkStorageKey)) return false;
        if (!ks.Put(TimeEnchantEventLevel)) return false;
        if (!ks.Put(NotifyGetItem)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        AfterWorkStorageKey = v0;
        if (!ks.Get(out sbyte v1)) return false;
        TimeEnchantEventLevel = v1;
        if (!ks.Get(out bool v2)) return false;
        NotifyGetItem = v2;
        return true;
    }
}

public sealed class KEGB_OPEN_PVP_ROOM_FOR_MATCH_REQ : IKSerializable
{
    public long MatchUid { get; set; }
    public sbyte MatchType { get; set; }
    public sbyte PvpNpcType { get; set; }
    public KRoomInfo RoomInfo { get; set; } = new();
    public List<KRoomUserInfo> RedTeam { get; set; } = new();
    public List<KRoomUserInfo> BlueTeam { get; set; } = new();
    public Dictionary<long, int> MatchWaitTime { get; set; } = new();
    public Dictionary<long, KPvpNpcInfo> PvpNpcInfo { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(MatchUid)) return false;
        if (!ks.Put(MatchType)) return false;
        if (!ks.Put(PvpNpcType)) return false;
        if (!ks.Put(RoomInfo)) return false;
        if (!ks.PutList(RedTeam)) return false;
        if (!ks.PutList(BlueTeam)) return false;
        if (!ks.PutMap(MatchWaitTime, static (s, v) => s.Put(v), static (s, v) => s.Put(v))) return false;
        if (!ks.PutMap(PvpNpcInfo, static (s, v) => s.Put(v), static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        MatchUid = v0;
        if (!ks.Get(out sbyte v1)) return false;
        MatchType = v1;
        if (!ks.Get(out sbyte v2)) return false;
        PvpNpcType = v2;
        if (!ks.Get(RoomInfo)) return false;
        if (!ks.GetList(RedTeam)) return false;
        if (!ks.GetList(BlueTeam)) return false;
        if (!ks.GetMap(MatchWaitTime, static s => (s.Get(out long v), v), static s => (s.Get(out int v), v))) return false;
        if (!ks.GetMap(PvpNpcInfo, static s => (s.Get(out long v), v), static s => { var v = new KPvpNpcInfo(); return (s.Get(v), v); })) return false;
        return true;
    }
}

public sealed class KEGB_GET_ROOM_USER_INFO_FOR_MATCH_ACK : IKSerializable
{
    public long MatchUid { get; set; }
    public KRoomUserInfo RoomUserInfo { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(MatchUid)) return false;
        if (!ks.Put(RoomUserInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        MatchUid = v0;
        if (!ks.Get(RoomUserInfo)) return false;
        return true;
    }
}

public sealed class KEGB_GET_REGROUP_PARTY_INFO_ACK : IKSerializable
{
    public long AutoPartyUid { get; set; }
    public KPartyUserInfo PartyUserInfo { get; set; } = new();
    public KRoomUserInfo RoomUserInfo { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(AutoPartyUid)) return false;
        if (!ks.Put(PartyUserInfo)) return false;
        if (!ks.Put(RoomUserInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        AutoPartyUid = v0;
        if (!ks.Get(PartyUserInfo)) return false;
        if (!ks.Get(RoomUserInfo)) return false;
        return true;
    }
}

// ---- Batch 2: CenterServer ECN_* & ERM_* packets ----

public sealed class KECN_VERIFY_SERVER_CONNECT_REQ : IKSerializable
{
    public long ServerUid { get; set; }
    public int ServerGroupId { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ServerUid)) return false;
        if (!ks.Put(ServerGroupId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        ServerUid = v0;
        if (!ks.Get(out int v1)) return false;
        ServerGroupId = v1;
        return true;
    }
}

public sealed class KECN_VERIFY_SERVER_CONNECT_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long CenterUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(CenterUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        Ok = v0;
        if (!ks.Get(out long v1)) return false;
        CenterUid = v1;
        return true;
    }
}

public sealed class KECN_CHECK_CHANNEL_CHANGE_REQ : IKSerializable
{
    public long UserUid { get; set; }
    public long UnitUid { get; set; }
    public int TargetChannelId { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UserUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(TargetChannelId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        UserUid = v0;
        if (!ks.Get(out long v1)) return false;
        UnitUid = v1;
        if (!ks.Get(out int v2)) return false;
        TargetChannelId = v2;
        return true;
    }
}

public sealed class KECN_CHECK_CHANNEL_CHANGE_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long UserUid { get; set; }
    public int TargetChannelId { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(UserUid)) return false;
        if (!ks.Put(TargetChannelId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        Ok = v0;
        if (!ks.Get(out long v1)) return false;
        UserUid = v1;
        if (!ks.Get(out int v2)) return false;
        TargetChannelId = v2;
        return true;
    }
}

public sealed class KERM_OPEN_ROOM_REQ : IKSerializable
{
    public long RoomUid { get; set; }
    public long PartyUid { get; set; }
    public KRoomInfo RoomInfo { get; set; } = new();
    public KRoomUserInfo HostInfo { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(RoomUid)) return false;
        if (!ks.Put(PartyUid)) return false;
        if (!ks.Put(RoomInfo)) return false;
        if (!ks.Put(HostInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        RoomUid = v0;
        if (!ks.Get(out long v1)) return false;
        PartyUid = v1;
        if (!ks.Get(RoomInfo)) return false;
        if (!ks.Get(HostInfo)) return false;
        return true;
    }
}

public sealed class KERM_OPEN_ROOM_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KRoomInfo RoomInfo { get; set; } = new();
    public List<KRoomSlotInfo> Slots { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(RoomInfo)) return false;
        if (!ks.PutList(Slots)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        Ok = v0;
        if (!ks.Get(RoomInfo)) return false;
        if (!ks.GetList(Slots)) return false;
        return true;
    }
}

public sealed class KERM_JOIN_ROOM_REQ : IKSerializable
{
    public long RoomUid { get; set; }
    public int MapId { get; set; }
    public KRoomUserInfo UserInfo { get; set; } = new();
    public string Password { get; set; } = string.Empty;
    public bool IsObserver { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(RoomUid)) return false;
        if (!ks.Put(MapId)) return false;
        if (!ks.Put(UserInfo)) return false;
        if (!ks.PutWideString(Password)) return false;
        if (!ks.Put(IsObserver)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        RoomUid = v0;
        if (!ks.Get(out int v1)) return false;
        MapId = v1;
        if (!ks.Get(UserInfo)) return false;
        if (!ks.GetWideString(out string v3)) return false;
        Password = v3;
        if (!ks.Get(out bool v4)) return false;
        IsObserver = v4;
        return true;
    }
}

public sealed class KERM_JOIN_ROOM_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KRoomInfo RoomInfo { get; set; } = new();
    public List<KRoomSlotInfo> Slots { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(RoomInfo)) return false;
        if (!ks.PutList(Slots)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        Ok = v0;
        if (!ks.Get(RoomInfo)) return false;
        if (!ks.GetList(Slots)) return false;
        return true;
    }
}

public sealed class KERM_LEAVE_ROOM_REQ : IKSerializable
{
    public long RoomUid { get; set; }
    public long UnitUid { get; set; }
    public int Reason { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(RoomUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(Reason)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        RoomUid = v0;
        if (!ks.Get(out long v1)) return false;
        UnitUid = v1;
        if (!ks.Get(out int v2)) return false;
        Reason = v2;
        return true;
    }
}

public sealed class KERM_GAME_START_REQ : IKSerializable
{
    public long RoomUid { get; set; }
    public long HostUnitUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(RoomUid)) return false;
        if (!ks.Put(HostUnitUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        RoomUid = v0;
        if (!ks.Get(out long v1)) return false;
        HostUnitUid = v1;
        return true;
    }
}

public sealed class KERM_OPEN_TRADE_REQ : IKSerializable
{
    public long OfferUnitUid { get; set; }
    public long TargetUnitUid { get; set; }
    public string OfferNickName { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(OfferUnitUid)) return false;
        if (!ks.Put(TargetUnitUid)) return false;
        if (!ks.PutWideString(OfferNickName)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        OfferUnitUid = v0;
        if (!ks.Get(out long v1)) return false;
        TargetUnitUid = v1;
        if (!ks.GetWideString(out string v2)) return false;
        OfferNickName = v2;
        return true;
    }
}

public sealed class KERM_JOIN_TRADE_REQ : IKSerializable
{
    public long TradeUid { get; set; }
    public long UnitUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(TradeUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        TradeUid = v0;
        if (!ks.Get(out long v1)) return false;
        UnitUid = v1;
        return true;
    }
}

public sealed class KERM_UPDATE_TRADE_ED_REQ : IKSerializable
{
    public long TradeUid { get; set; }
    public long UnitUid { get; set; }
    public long EdAmount { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(TradeUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(EdAmount)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        TradeUid = v0;
        if (!ks.Get(out long v1)) return false;
        UnitUid = v1;
        if (!ks.Get(out long v2)) return false;
        EdAmount = v2;
        return true;
    }
}

public sealed class KERM_UPDATE_TRADE_ITEM_REQ : IKSerializable
{
    public long TradeUid { get; set; }
    public long UnitUid { get; set; }
    public KInventoryItemInfo Item { get; set; } = new();
    public bool Add { get; set; } = true;
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(TradeUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(Item)) return false;
        if (!ks.Put(Add)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        TradeUid = v0;
        if (!ks.Get(out long v1)) return false;
        UnitUid = v1;
        if (!ks.Get(Item)) return false;
        if (!ks.Get(out bool v3)) return false;
        Add = v3;
        return true;
    }
}

public sealed class KERM_OPEN_PERSONAL_SHOP_REQ : IKSerializable
{
    public long OwnerUnitUid { get; set; }
    public string OwnerNickName { get; set; } = string.Empty;
    public string ShopName { get; set; } = string.Empty;
    public sbyte ShopType { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(OwnerUnitUid)) return false;
        if (!ks.PutWideString(OwnerNickName)) return false;
        if (!ks.PutWideString(ShopName)) return false;
        if (!ks.Put(ShopType)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        OwnerUnitUid = v0;
        if (!ks.GetWideString(out string v1)) return false;
        OwnerNickName = v1;
        if (!ks.GetWideString(out string v2)) return false;
        ShopName = v2;
        if (!ks.Get(out sbyte v3)) return false;
        ShopType = v3;
        return true;
    }
}

public sealed class KERM_JOIN_PERSONAL_SHOP_REQ : IKSerializable
{
    public long ShopUid { get; set; }
    public long UnitUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ShopUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        ShopUid = v0;
        if (!ks.Get(out long v1)) return false;
        UnitUid = v1;
        return true;
    }
}

public sealed class KERM_LEAVE_PERSONAL_SHOP_REQ : IKSerializable
{
    public long ShopUid { get; set; }
    public long UnitUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ShopUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        ShopUid = v0;
        if (!ks.Get(out long v1)) return false;
        UnitUid = v1;
        return true;
    }
}

public sealed class KERM_OPEN_WEDDING_HALL_REQ : IKSerializable
{
    public long RoomUid { get; set; }
    public int WeddingUid { get; set; }
    public long BrideUnitUid { get; set; }
    public long GroomUnitUid { get; set; }
    public sbyte HallType { get; set; }
    public sbyte OfficiantNpc { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(RoomUid)) return false;
        if (!ks.Put(WeddingUid)) return false;
        if (!ks.Put(BrideUnitUid)) return false;
        if (!ks.Put(GroomUnitUid)) return false;
        if (!ks.Put(HallType)) return false;
        if (!ks.Put(OfficiantNpc)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        RoomUid = v0;
        if (!ks.Get(out int v1)) return false;
        WeddingUid = v1;
        if (!ks.Get(out long v2)) return false;
        BrideUnitUid = v2;
        if (!ks.Get(out long v3)) return false;
        GroomUnitUid = v3;
        if (!ks.Get(out sbyte v4)) return false;
        HallType = v4;
        if (!ks.Get(out sbyte v5)) return false;
        OfficiantNpc = v5;
        return true;
    }
}

public sealed class KERM_JOIN_WEDDING_HALL_REQ : IKSerializable
{
    public long RoomUid { get; set; }
    public long UnitUid { get; set; }
    public string NickName { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(RoomUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutWideString(NickName)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        RoomUid = v0;
        if (!ks.Get(out long v1)) return false;
        UnitUid = v1;
        if (!ks.GetWideString(out string v2)) return false;
        NickName = v2;
        return true;
    }
}

public sealed class KERM_OPEN_SQUARE_NOT : IKSerializable
{
    public sbyte SquareType { get; set; }
    public string RoomName { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(SquareType)) return false;
        if (!ks.PutWideString(RoomName)) return false;
        if (!ks.Put(IsDefault)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte v0)) return false;
        SquareType = v0;
        if (!ks.GetWideString(out string v1)) return false;
        RoomName = v1;
        if (!ks.Get(out bool v2)) return false;
        IsDefault = v2;
        return true;
    }
}

public sealed class KERM_JOIN_SQUARE_REQ : IKSerializable
{
    public long SquareUid { get; set; }
    public long UnitUid { get; set; }
    public string NickName { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(SquareUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutWideString(NickName)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        SquareUid = v0;
        if (!ks.Get(out long v1)) return false;
        UnitUid = v1;
        if (!ks.GetWideString(out string v2)) return false;
        NickName = v2;
        return true;
    }
}

// ---- Batch 3: LoginServer ELG_* packets ----

public sealed class KELG_SEARCH_UNIT_REQ : IKSerializable
{
    public ushort EventIdValue { get; set; }
    public string NickName { get; set; } = string.Empty;
    public long UnitUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(EventIdValue)) return false;
        if (!ks.PutWideString(NickName)) return false;
        if (!ks.Put(UnitUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out ushort v0)) return false;
        EventIdValue = v0;
        if (!ks.GetWideString(out string v1)) return false;
        NickName = v1;
        if (!ks.Get(out long v2)) return false;
        UnitUid = v2;
        return true;
    }
}

public sealed class KELG_SEARCH_UNIT_ACK : IKSerializable
{
    public int Ok { get; set; }
    public ushort EventIdValue { get; set; }
    public KConnectionUnitInfo CUnitInfo { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(EventIdValue)) return false;
        if (!ks.Put(CUnitInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        Ok = v0;
        if (!ks.Get(out ushort v1)) return false;
        EventIdValue = v1;
        if (!ks.Get(CUnitInfo)) return false;
        return true;
    }
}

public sealed class KELG_CHAT_REQ : IKSerializable
{
    public sbyte ChatPacketType { get; set; }
    public int ChatData { get; set; }
    public long SenderUnitUid { get; set; }
    public string SenderNickName { get; set; } = string.Empty;
    public string ReceiverNickName { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ChatPacketType)) return false;
        if (!ks.Put(ChatData)) return false;
        if (!ks.Put(SenderUnitUid)) return false;
        if (!ks.PutWideString(SenderNickName)) return false;
        if (!ks.PutWideString(ReceiverNickName)) return false;
        if (!ks.PutWideString(Message)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte v0)) return false;
        ChatPacketType = v0;
        if (!ks.Get(out int v1)) return false;
        ChatData = v1;
        if (!ks.Get(out long v2)) return false;
        SenderUnitUid = v2;
        if (!ks.GetWideString(out string v3)) return false;
        SenderNickName = v3;
        if (!ks.GetWideString(out string v4)) return false;
        ReceiverNickName = v4;
        if (!ks.GetWideString(out string v5)) return false;
        Message = v5;
        return true;
    }
}

public sealed class KELG_CHAT_ACK : IKSerializable
{
    public int Ok { get; set; }
    public sbyte ChatPacketType { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(ChatPacketType)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        Ok = v0;
        if (!ks.Get(out sbyte v1)) return false;
        ChatPacketType = v1;
        return true;
    }
}

public sealed class KELG_GET_RANKING_FOR_GS_RANKING_MANAGER_ACK : IKSerializable
{
    public Dictionary<int, List<KHenirRankingInfo>> HenirRanking { get; set; } = new();
    public Dictionary<int, uint> LastRank { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutMap(HenirRanking, static (s, v) => s.Put(v), static (s, l) => s.PutList(l))) return false;
        if (!ks.PutMap(LastRank, static (s, v) => s.Put(v), static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetMap(HenirRanking, static s => (s.Get(out int v), v), static s => { var l = new List<KHenirRankingInfo>(); return (s.GetList(l), l); })) return false;
        if (!ks.GetMap(LastRank, static s => (s.Get(out int v), v), static s => (s.Get(out uint v), v))) return false;
        return true;
    }
}

public sealed class KELG_UPDATE_USER_UNIT_BUFF_INFO_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public List<KBuffInfo> ActivateBuff { get; set; } = new();
    public List<int> DeactivateBuff { get; set; } = new();
    public bool SendAck { get; set; } = true;
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutList(ActivateBuff)) return false;
        if (!ks.PutList(DeactivateBuff, static (s, v) => s.Put(v))) return false;
        if (!ks.Put(SendAck)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        UnitUid = v0;
        if (!ks.GetList(ActivateBuff)) return false;
        if (!ks.GetList(DeactivateBuff, static s => (s.Get(out int v), v))) return false;
        if (!ks.Get(out bool v3)) return false;
        SendAck = v3;
        return true;
    }
}

public sealed class KELG_UPDATE_USER_UNIT_BUFF_INFO_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long UnitUid { get; set; }
    public List<KBuffInfo> ActiveBuffs { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutList(ActiveBuffs)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        Ok = v0;
        if (!ks.Get(out long v1)) return false;
        UnitUid = v1;
        if (!ks.GetList(ActiveBuffs)) return false;
        return true;
    }
}

public sealed class KELG_UPDATE_CCU_INFO_NOT : IKSerializable
{
    public long ServerUid { get; set; }
    public int ServerGroupId { get; set; }
    public int ConcurrentUser { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ServerUid)) return false;
        if (!ks.Put(ServerGroupId)) return false;
        if (!ks.Put(ConcurrentUser)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        ServerUid = v0;
        if (!ks.Get(out int v1)) return false;
        ServerGroupId = v1;
        if (!ks.Get(out int v2)) return false;
        ConcurrentUser = v2;
        return true;
    }
}

public sealed class KELG_INVITE_PARTY_FIND_RECEIVER_REQ : IKSerializable
{
    public long PartyUid { get; set; }
    public long HostUnitUid { get; set; }
    public string HostNickName { get; set; } = string.Empty;
    public long ReceiverUnitUid { get; set; }
    public string ReceiverNickName { get; set; } = string.Empty;
    public KCreatePartyInfo CreatePartyInfo { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PartyUid)) return false;
        if (!ks.Put(HostUnitUid)) return false;
        if (!ks.PutWideString(HostNickName)) return false;
        if (!ks.Put(ReceiverUnitUid)) return false;
        if (!ks.PutWideString(ReceiverNickName)) return false;
        if (!ks.Put(CreatePartyInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        PartyUid = v0;
        if (!ks.Get(out long v1)) return false;
        HostUnitUid = v1;
        if (!ks.GetWideString(out string v2)) return false;
        HostNickName = v2;
        if (!ks.Get(out long v3)) return false;
        ReceiverUnitUid = v3;
        if (!ks.GetWideString(out string v4)) return false;
        ReceiverNickName = v4;
        if (!ks.Get(CreatePartyInfo)) return false;
        return true;
    }
}

public sealed class KELG_INVITE_PARTY_FIND_RECEIVER_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long HostUnitUid { get; set; }
    public long ReceiverUnitUid { get; set; }
    public string ReceiverNickName { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(HostUnitUid)) return false;
        if (!ks.Put(ReceiverUnitUid)) return false;
        if (!ks.PutWideString(ReceiverNickName)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        Ok = v0;
        if (!ks.Get(out long v1)) return false;
        HostUnitUid = v1;
        if (!ks.Get(out long v2)) return false;
        ReceiverUnitUid = v2;
        if (!ks.GetWideString(out string v3)) return false;
        ReceiverNickName = v3;
        return true;
    }
}

public sealed class KELG_INVITE_PVP_ROOM_REQ : IKSerializable
{
    public string FromNickName { get; set; } = string.Empty;
    public long ToUnitUid { get; set; }
    public KRoomInfo RoomInfo { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutWideString(FromNickName)) return false;
        if (!ks.Put(ToUnitUid)) return false;
        if (!ks.Put(RoomInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string v0)) return false;
        FromNickName = v0;
        if (!ks.Get(out long v1)) return false;
        ToUnitUid = v1;
        if (!ks.Get(RoomInfo)) return false;
        return true;
    }
}

public sealed class KELG_INVITE_PVP_ROOM_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long ToUnitUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(ToUnitUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        Ok = v0;
        if (!ks.Get(out long v1)) return false;
        ToUnitUid = v1;
        return true;
    }
}

public sealed class KELG_FIND_USER_SEND_LETTER_NOT : IKSerializable
{
    public long TargetUnitUid { get; set; }
    public string TargetNickName { get; set; } = string.Empty;
    public KPostItemInfo Letter { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(TargetUnitUid)) return false;
        if (!ks.PutWideString(TargetNickName)) return false;
        if (!ks.Put(Letter)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        TargetUnitUid = v0;
        if (!ks.GetWideString(out string v1)) return false;
        TargetNickName = v1;
        if (!ks.Get(Letter)) return false;
        return true;
    }
}

public sealed class KELG_FRIEND_UNIT_NOT : IKSerializable
{
    public long SrcUnitUid { get; set; }
    public string SrcNickName { get; set; } = string.Empty;
    public long DestUnitUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(SrcUnitUid)) return false;
        if (!ks.PutWideString(SrcNickName)) return false;
        if (!ks.Put(DestUnitUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        SrcUnitUid = v0;
        if (!ks.GetWideString(out string v1)) return false;
        SrcNickName = v1;
        if (!ks.Get(out long v2)) return false;
        DestUnitUid = v2;
        return true;
    }
}

public sealed class KELG_FRIEND_POSITION_UPDATE_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public sbyte Position { get; set; }
    public int VillageMapId { get; set; }
    public List<long> FriendUnitUids { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(Position)) return false;
        if (!ks.Put(VillageMapId)) return false;
        if (!ks.PutList(FriendUnitUids, static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        UnitUid = v0;
        if (!ks.Get(out sbyte v1)) return false;
        Position = v1;
        if (!ks.Get(out int v2)) return false;
        VillageMapId = v2;
        if (!ks.GetList(FriendUnitUids, static s => (s.Get(out long v), v))) return false;
        return true;
    }
}

public sealed class KELG_ADD_WEDDING_HALL_INFO_NOT : IKSerializable
{
    public int WeddingUid { get; set; }
    public long RoomUid { get; set; }
    public long BrideUnitUid { get; set; }
    public long GroomUnitUid { get; set; }
    public string BrideNickName { get; set; } = string.Empty;
    public string GroomNickName { get; set; } = string.Empty;
    public sbyte WeddingHallType { get; set; }
    public sbyte OfficiantNpc { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(WeddingUid)) return false;
        if (!ks.Put(RoomUid)) return false;
        if (!ks.Put(BrideUnitUid)) return false;
        if (!ks.Put(GroomUnitUid)) return false;
        if (!ks.PutWideString(BrideNickName)) return false;
        if (!ks.PutWideString(GroomNickName)) return false;
        if (!ks.Put(WeddingHallType)) return false;
        if (!ks.Put(OfficiantNpc)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        WeddingUid = v0;
        if (!ks.Get(out long v1)) return false;
        RoomUid = v1;
        if (!ks.Get(out long v2)) return false;
        BrideUnitUid = v2;
        if (!ks.Get(out long v3)) return false;
        GroomUnitUid = v3;
        if (!ks.GetWideString(out string v4)) return false;
        BrideNickName = v4;
        if (!ks.GetWideString(out string v5)) return false;
        GroomNickName = v5;
        if (!ks.Get(out sbyte v6)) return false;
        WeddingHallType = v6;
        if (!ks.Get(out sbyte v7)) return false;
        OfficiantNpc = v7;
        return true;
    }
}

public sealed class KELG_CHECK_WEDDING_HALL_INFO_REQ : IKSerializable
{
    public int WeddingUid { get; set; }
    public long UnitUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(WeddingUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        WeddingUid = v0;
        if (!ks.Get(out long v1)) return false;
        UnitUid = v1;
        return true;
    }
}

public sealed class KELG_CHECK_WEDDING_HALL_INFO_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int WeddingUid { get; set; }
    public long RoomUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(WeddingUid)) return false;
        if (!ks.Put(RoomUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        Ok = v0;
        if (!ks.Get(out int v1)) return false;
        WeddingUid = v1;
        if (!ks.Get(out long v2)) return false;
        RoomUid = v2;
        return true;
    }
}

public sealed class KELG_RELATIONSHIP_EVENT_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public long PartnerUnitUid { get; set; }
    public string LoveWord { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(PartnerUnitUid)) return false;
        if (!ks.PutWideString(LoveWord)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        UnitUid = v0;
        if (!ks.Get(out long v1)) return false;
        PartnerUnitUid = v1;
        if (!ks.GetWideString(out string v2)) return false;
        LoveWord = v2;
        return true;
    }
}

public sealed class KELG_GET_GUILD_AD_LIST_REQ : IKSerializable
{
    public int CurrentPage { get; set; } = 1;
    public long RequesterUnitUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(CurrentPage)) return false;
        if (!ks.Put(RequesterUnitUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        CurrentPage = v0;
        if (!ks.Get(out long v1)) return false;
        RequesterUnitUid = v1;
        return true;
    }
}

public sealed class KELG_GET_GUILD_AD_LIST_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int TotalPage { get; set; }
    public int CurrentPage { get; set; }
    public List<KGuildAdInfo> AdList { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(TotalPage)) return false;
        if (!ks.Put(CurrentPage)) return false;
        if (!ks.PutList(AdList)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        Ok = v0;
        if (!ks.Get(out int v1)) return false;
        TotalPage = v1;
        if (!ks.Get(out int v2)) return false;
        CurrentPage = v2;
        if (!ks.GetList(AdList)) return false;
        return true;
    }
}

public sealed class KELG_REGISTRATION_GUILD_AD_REQ : IKSerializable
{
    public int GuildUid { get; set; }
    public long UnitUid { get; set; }
    public string AdMessage { get; set; } = string.Empty;
    public short AdPeriod { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(GuildUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutWideString(AdMessage)) return false;
        if (!ks.Put(AdPeriod)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        GuildUid = v0;
        if (!ks.Get(out long v1)) return false;
        UnitUid = v1;
        if (!ks.GetWideString(out string v2)) return false;
        AdMessage = v2;
        if (!ks.Get(out short v3)) return false;
        AdPeriod = v3;
        return true;
    }
}

public sealed class KELG_REGISTRATION_GUILD_AD_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int GuildUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(GuildUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        Ok = v0;
        if (!ks.Get(out int v1)) return false;
        GuildUid = v1;
        return true;
    }
}

public sealed class KEGB_EXCHANGE_LIMIT_INFO_NOT : IKSerializable
{
    public int ExchangeLimitUid { get; set; }
    public Dictionary<int, int> ExchangeLimitInfo { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ExchangeLimitUid)) return false;
        if (!ks.PutMap(ExchangeLimitInfo, static (s, v) => s.Put(v), static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        ExchangeLimitUid = v0;
        if (!ks.GetMap(ExchangeLimitInfo, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false;
        return true;
    }
}

public sealed class KEGB_TIME_ENCHANT_EVENT_INFO_NOT : IKSerializable
{
    public int TimeEnchantEventUid { get; set; }
    public int NumOfTimeEnchantEvent { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(TimeEnchantEventUid)) return false;
        if (!ks.Put(NumOfTimeEnchantEvent)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        TimeEnchantEventUid = v0;
        if (!ks.Get(out int v1)) return false;
        NumOfTimeEnchantEvent = v1;
        return true;
    }
}

public sealed class KEGB_TODAY_RANDOM_QUEST_ACK : IKSerializable
{
    public Dictionary<int, int> TodayRandomQuest { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutMap(TodayRandomQuest, static (s, v) => s.Put(v), static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetMap(TodayRandomQuest, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false;
        return true;
    }
}

public sealed class KEGB_GLOBAL_MISSION_UPDATE_NOT : IKSerializable
{
    public string GlobalMissionDay { get; set; } = string.Empty;
    public int NowCount { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutWideString(GlobalMissionDay)) return false;
        if (!ks.Put(NowCount)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string v0)) return false;
        GlobalMissionDay = v0;
        if (!ks.Get(out int v1)) return false;
        NowCount = v1;
        return true;
    }
}

public sealed class KELG_CHECK_MODULE_INFO_UPDATE_NOT : IKSerializable
{
    public int Ok { get; set; }
    public Dictionary<string, int> ModuleName { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.PutMap(ModuleName, static (s, v) => s.PutWideString(v), static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        Ok = v0;
        if (!ks.GetMap(ModuleName, static s => (s.GetWideString(out string v), v), static s => (s.Get(out int v), v))) return false;
        return true;
    }
}

public sealed class KESR_SCRIPT_REFRESH_ORDER_NOT : IKSerializable
{
    public const int OT_NONE = 0;

    // CenterServer
    public const int OT_CN_ITEM_MANAGER = 1;
    public const int OT_CN_QUEST_MANAGER = 2;
    public const int OT_CN_DUNGEON_MANAGER = 3;
    public const int OT_CN_DROP_TABLE = 4;
    public const int OT_CN_GAME_EVENT_SCRIPT_MANAGER = 5;
    public const int OT_CN_GAME_SYS_VAL = 6;
    public const int OT_CN_ATTRIB_NPC_TABLE = 7;
    public const int OT_CN_HACKING_CHECK_MANAGER = 8;
    public const int OT_CN_ABUSER_LOG_MANAGER = 9;
    public const int OT_CN_RESULT_PROCESS = 10;
    public const int OT_CN_PVP_MATCH_MANAGER = 11;
    public const int OT_CN_PVP_MATCH_RESULT_TABLE = 12;
    public const int OT_CN_HENIR_RESULT_TABLE = 13;
    public const int OT_CN_BATTLE_FIELD_MANAGER = 14;
    public const int OT_CN_PARTY_MANAGER = 15;
    public const int OT_CN_SMS_MANAGER = 16;
    public const int OT_CN_AUTO_PARTY_SCRIPT_MANAGER = 17;
    public const int OT_CN_BAD_ATTITUDE_TABLE = 18;
    public const int OT_CN_LOG_MANAGER = 19;
    public const int OT_CN_DEFENCE_DUNGEON_MANAGER = 20;
    public const int OT_CN_BUFF_MANAGER = 21;
    public const int OT_CN_FIELD_BOSS_DATA = 22;
    public const int OT_CN_SENTINEL = 23;

    // LoginServer
    public const int OT_LG_GUILD_MANAGER = 24;
    public const int OT_LG_MORNITORING_MANAGER = 25;
    public const int OT_LG_WORLD_MISSION_MANAGER = 26;
    public const int OT_LG_GAME_SYS_VAL = 27;
    public const int OT_LG_TITLE_MANAGER = 28;
    public const int OT_LG_SMS_MANAGER = 29;
    public const int OT_LG_LOG_MANAGER = 30;
    public const int OT_LG_SKILL_TREE = 31;
    public const int OT_LG_SENTINEL = 32;

    // GameServer
    public const int OT_GS_ITEM_MANAGER = 33;
    public const int OT_GS_RANDOM_ITEM_MANAGER = 34;
    public const int OT_GS_MANUFACTURE_ITEM_MANAGER = 35;
    public const int OT_GS_REWARD_TABLE = 36;
    public const int OT_GS_QUEST_MANAGER = 37;
    public const int OT_GS_TITLE_MANAGER = 38;
    public const int OT_GS_DUNGEON_MANAGER = 39;
    public const int OT_GS_MAP_DATA = 40;
    public const int OT_GS_GUILD_MANAGER = 41;
    public const int OT_GS_GAME_SYS_VAL = 42;
    public const int OT_GS_RESOLVE_ITEM_MANAGER = 43;
    public const int OT_GS_ENCHANT_ITEM_MANAGER = 44;
    public const int OT_GS_SOCKET_ITEM = 45;
    public const int OT_GS_ATTRIB_ENCHANT_ITEM = 46;
    public const int OT_GS_RECOMMEND_USER_TABLE = 47;
    public const int OT_GS_EXP_TABLE = 48;
    public const int OT_GS_PET_MANAGER = 49;
    public const int OT_GS_MORNITORING_MANAGER = 50;
    public const int OT_GS_ABUSER_LOG_MANAGER = 51;
    public const int OT_GS_GAME_EVENT_SCRIPT_MANAGER = 52;
    public const int OT_GS_PVP_MATCH_MANAGER = 53;
    public const int OT_GS_CONTENT_MANAGER = 54;
    public const int OT_GS_COMPANY_IP_TABLE = 55;
    public const int OT_GS_CLASS_CHANGE_TABLE = 56;
    public const int OT_GS_STRING_FILTER = 57;
    public const int OT_GS_BATTLE_FIELD_MANAGER = 58;
    public const int OT_GS_PARTY_MANAGER = 59;
    public const int OT_GS_DAILY_GIFT_BOX_MANAGER = 60;
    public const int OT_GS_SMS_MANAGER = 61;
    public const int OT_GS_BINGO_EVENT_MANAGER = 62;
    public const int OT_GS_SYNTHESIS_TABLE = 63;
    public const int OT_GS_LOG_MANAGER = 64;
    public const int OT_GS_DEFENCE_DUNGEON_MANAGER = 65;
    public const int OT_GS_BUFF_MANAGER = 66;
    public const int OT_GS_RIDING_PET_MANAGER = 67;
    public const int OT_GS_SKILL_TREE = 68;
    public const int OT_GS_FIELD_BOSS_DATA = 69;
    public const int OT_GS_ED_INVENTORY_EXPAND = 70;
    public const int OT_GS_SENTINEL = 71;

    // GlobalServer
    public const int OT_GB_MORNITORING_MANAGER = 72;
    public const int OT_GB_WORLD_MISSION_MANAGER = 73;
    public const int OT_GB_PVP_MATCH_MANAGER = 74;
    public const int OT_GB_QUEST_MANAGER = 75;
    public const int OT_GB_SMS_MANAGER = 76;
    public const int OT_GB_AUTO_PARTY_SCRIPT_MANAGER = 77;
    public const int OT_GB_LOG_MANAGER = 78;
    public const int OT_GB_BATTLE_FIELD_MANAGER = 79;
    public const int OT_GB_FIELD_BOSS_DATA = 80;
    public const int OT_GB_SENTINEL = 81;

    // ChannelServer
    public const int OT_CH_SMS_MANAGER = 82;
    public const int OT_CH_LOG_MANAGER = 83;
    public const int OT_CH_SENTINEL = 84;

    // PascalCase aliases preserved for existing callers
    public const int OtNone = OT_NONE;
    public const int OtGbPvpMatchManager = OT_GB_PVP_MATCH_MANAGER;
    public const int OtGbAutoPartyScriptManager = OT_GB_AUTO_PARTY_SCRIPT_MANAGER;
    public const int OtGbWorldMissionManager = OT_GB_WORLD_MISSION_MANAGER;
    public const int OtGbQuestManager = OT_GB_QUEST_MANAGER;

    public int OrderType { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(OrderType)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        OrderType = v0;
        return true;
    }
}

// ---- CenterServer EPM_* / ERM_* / ECN_* packets ----

public sealed class KEPM_CHAT_REQ : IKSerializable
{
    public long PartyUid { get; set; }
    public sbyte ChatPacketType { get; set; }
    public sbyte RoomChatType { get; set; }
    public long SenderUnitUid { get; set; }
    public string SenderNickName { get; set; } = string.Empty;
    public long ToUnitUid { get; set; }
    public string ReceiverNickName { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PartyUid)) return false;
        if (!ks.Put(ChatPacketType)) return false;
        if (!ks.Put(RoomChatType)) return false;
        if (!ks.Put(SenderUnitUid)) return false;
        if (!ks.PutWideString(SenderNickName)) return false;
        if (!ks.Put(ToUnitUid)) return false;
        if (!ks.PutWideString(ReceiverNickName)) return false;
        if (!ks.PutWideString(Message)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        PartyUid = v0;
        if (!ks.Get(out sbyte v1)) return false;
        ChatPacketType = v1;
        if (!ks.Get(out sbyte v2)) return false;
        RoomChatType = v2;
        if (!ks.Get(out long v3)) return false;
        SenderUnitUid = v3;
        if (!ks.GetWideString(out string v4)) return false;
        SenderNickName = v4;
        if (!ks.Get(out long v5)) return false;
        ToUnitUid = v5;
        if (!ks.GetWideString(out string v6)) return false;
        ReceiverNickName = v6;
        if (!ks.GetWideString(out string v7)) return false;
        Message = v7;
        return true;
    }
}

public sealed class KEPM_INVITE_PARTY_REQ : IKSerializable
{
    public long PartyUid { get; set; }
    public KPartyUserInfo PartyUserInfo { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PartyUid)) return false;
        if (!ks.Put(PartyUserInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        PartyUid = v0;
        if (!ks.Get(PartyUserInfo)) return false;
        return true;
    }
}

public sealed class KEPM_INVITE_PARTY_REPLY_NOT : IKSerializable
{
    public long PartyUid { get; set; }
    public sbyte ReplyReason { get; set; }
    public KPartyUserInfo PartyUserInfo { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PartyUid)) return false;
        if (!ks.Put(ReplyReason)) return false;
        if (!ks.Put(PartyUserInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        PartyUid = v0;
        if (!ks.Get(out sbyte v1)) return false;
        ReplyReason = v1;
        if (!ks.Get(PartyUserInfo)) return false;
        return true;
    }
}

public sealed class KEPM_PARTY_BAN_USER_REQ : IKSerializable
{
    public long PartyUid { get; set; }
    public long UnitUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PartyUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        PartyUid = v0;
        if (!ks.Get(out long v1)) return false;
        UnitUid = v1;
        return true;
    }
}

public sealed class KEPM_PARTY_CHANGE_CHECK_MIN_LEVEL_REQ : IKSerializable
{
    public long PartyUid { get; set; }
    public bool Check { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PartyUid)) return false;
        if (!ks.Put(Check)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        PartyUid = v0;
        if (!ks.Get(out bool v1)) return false;
        Check = v1;
        return true;
    }
}

public sealed class KEPM_PARTY_CHANGE_DUNGEON_REQ : IKSerializable
{
    public long PartyUid { get; set; }
    public int DungeonId { get; set; }
    public sbyte DifficultyLevel { get; set; }
    public sbyte DungeonMode { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PartyUid)) return false;
        if (!ks.Put(DungeonId)) return false;
        if (!ks.Put(DifficultyLevel)) return false;
        if (!ks.Put(DungeonMode)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        PartyUid = v0;
        if (!ks.Get(out int v1)) return false;
        DungeonId = v1;
        if (!ks.Get(out sbyte v2)) return false;
        DifficultyLevel = v2;
        if (!ks.Get(out sbyte v3)) return false;
        DungeonMode = v3;
        return true;
    }
}

public sealed class KEPM_PARTY_CHANGE_GET_ITEM_TYPE_REQ : IKSerializable
{
    public long PartyUid { get; set; }
    public sbyte GetItemType { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PartyUid)) return false;
        if (!ks.Put(GetItemType)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        PartyUid = v0;
        if (!ks.Get(out sbyte v1)) return false;
        GetItemType = v1;
        return true;
    }
}

public sealed class KEPM_PARTY_CHANGE_HOST_REQ : IKSerializable
{
    public long PartyUid { get; set; }
    public long NewHostUnitUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PartyUid)) return false;
        if (!ks.Put(NewHostUnitUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        PartyUid = v0;
        if (!ks.Get(out long v1)) return false;
        NewHostUnitUid = v1;
        return true;
    }
}

public sealed class KEPM_PARTY_CHANGE_NUM_OF_PER_REQ : IKSerializable
{
    public long PartyUid { get; set; }
    public sbyte NumOfPer { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PartyUid)) return false;
        if (!ks.Put(NumOfPer)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        PartyUid = v0;
        if (!ks.Get(out sbyte v1)) return false;
        NumOfPer = v1;
        return true;
    }
}

public sealed class KEPM_PARTY_CHANGE_READY_REQ : IKSerializable
{
    public long PartyUid { get; set; }
    public bool Ready { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PartyUid)) return false;
        if (!ks.Put(Ready)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        PartyUid = v0;
        if (!ks.Get(out bool v1)) return false;
        Ready = v1;
        return true;
    }
}

public sealed class KEPM_PARTY_GAME_START_REQ : IKSerializable
{
    public long PartyUid { get; set; }
    public short WorldId { get; set; }
    public ushort EventIdValue { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PartyUid)) return false;
        if (!ks.Put(WorldId)) return false;
        if (!ks.Put(EventIdValue)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        PartyUid = v0;
        if (!ks.Get(out short v1)) return false;
        WorldId = v1;
        if (!ks.Get(out ushort v2)) return false;
        EventIdValue = v2;
        return true;
    }
}

public sealed class KEPM_END_GAME_NOT : IKSerializable
{
    public long PartyUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PartyUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        PartyUid = v0;
        return true;
    }
}

public sealed class KEPM_GET_PARTY_INFO_FOR_AUTO_PARTY_MAKING_REQ : IKSerializable
{
    public bool RemakingAutoParty { get; set; }
    public long PartyUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(RemakingAutoParty)) return false;
        if (!ks.Put(PartyUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool v0)) return false;
        RemakingAutoParty = v0;
        if (!ks.Get(out long v1)) return false;
        PartyUid = v1;
        return true;
    }
}

public sealed class KEPM_GET_PARTY_INFO_FOR_AUTO_PARTY_MAKING_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KPartyInfo PartyInfo { get; set; } = new();
    public List<KPartyUserInfo> Members { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(PartyInfo)) return false;
        if (!ks.PutList(Members)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        Ok = v0;
        if (!ks.Get(PartyInfo)) return false;
        if (!ks.GetList(Members)) return false;
        return true;
    }
}

public sealed class KEPM_GET_PARTY_INFO_FOR_JOIN_BATTLE_FIELD_REQ : IKSerializable
{
    public KBattleFieldJoinInfo BattleFieldJoinInfo { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(BattleFieldJoinInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(BattleFieldJoinInfo)) return false;
        return true;
    }
}

public sealed class KEPM_GET_PARTY_INFO_FOR_JOIN_BATTLE_FIELD_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KBattleFieldJoinInfo BattleFieldJoinInfo { get; set; } = new();
    public List<long> PartyMemberUids { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(BattleFieldJoinInfo)) return false;
        if (!ks.PutList(PartyMemberUids, static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        Ok = v0;
        if (!ks.Get(BattleFieldJoinInfo)) return false;
        if (!ks.GetList(PartyMemberUids, static s => (s.Get(out long v), v))) return false;
        return true;
    }
}

public sealed class KEPM_GET_PARTY_INFO_FOR_MATCH_MAKING_REQ : IKSerializable
{
    public bool RegFrontOfList { get; set; }
    public sbyte PvpPlayerCount { get; set; }
    public sbyte PvpGameType { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(RegFrontOfList)) return false;
        if (!ks.Put(PvpPlayerCount)) return false;
        if (!ks.Put(PvpGameType)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool v0)) return false;
        RegFrontOfList = v0;
        if (!ks.Get(out sbyte v1)) return false;
        PvpPlayerCount = v1;
        if (!ks.Get(out sbyte v2)) return false;
        PvpGameType = v2;
        return true;
    }
}

public sealed class KEPM_GET_PARTY_INFO_FOR_MATCH_MAKING_ACK : IKSerializable
{
    public int Ok { get; set; }
    public bool RegFrontOfList { get; set; }
    public sbyte PvpPlayerCount { get; set; }
    public sbyte PvpGameType { get; set; }
    public List<KPartyUserInfo> Members { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(RegFrontOfList)) return false;
        if (!ks.Put(PvpPlayerCount)) return false;
        if (!ks.Put(PvpGameType)) return false;
        if (!ks.PutList(Members)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        Ok = v0;
        if (!ks.Get(out bool v1)) return false;
        RegFrontOfList = v1;
        if (!ks.Get(out sbyte v2)) return false;
        PvpPlayerCount = v2;
        if (!ks.Get(out sbyte v3)) return false;
        PvpGameType = v3;
        if (!ks.GetList(Members)) return false;
        return true;
    }
}

public sealed class KEPM_REGROUP_PARTY_FOR_AUTO_PARTY_DUNGEON_GAME_REQ : IKSerializable
{
    public long AutoPartyUid { get; set; }
    public long AutoPartyHostUnitUid { get; set; }
    public KCreatePartyInfo CreatePartyInfo { get; set; } = new();
    public List<KPartyUserInfo> Members { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(AutoPartyUid)) return false;
        if (!ks.Put(AutoPartyHostUnitUid)) return false;
        if (!ks.Put(CreatePartyInfo)) return false;
        if (!ks.PutList(Members)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        AutoPartyUid = v0;
        if (!ks.Get(out long v1)) return false;
        AutoPartyHostUnitUid = v1;
        if (!ks.Get(CreatePartyInfo)) return false;
        if (!ks.GetList(Members)) return false;
        return true;
    }
}

public sealed class KEPM_REGROUP_PARTY_FOR_AUTO_PARTY_DUNGEON_GAME_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long PartyUid { get; set; }
    public long AutoPartyUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(PartyUid)) return false;
        if (!ks.Put(AutoPartyUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        Ok = v0;
        if (!ks.Get(out long v1)) return false;
        PartyUid = v1;
        if (!ks.Get(out long v2)) return false;
        AutoPartyUid = v2;
        return true;
    }
}

public sealed class KEPM_UPDATE_PARTY_USER_INFO_NOT : IKSerializable
{
    public long PartyUid { get; set; }
    public KPartyUserInfo UserInfo { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PartyUid)) return false;
        if (!ks.Put(UserInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        PartyUid = v0;
        if (!ks.Get(UserInfo)) return false;
        return true;
    }
}

public sealed class KEPM_UPDATE_GAME_PLAY_STATUS_NOT : IKSerializable
{
    public long PartyUid { get; set; }
    public long UnitUid { get; set; }
    public bool Playing { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PartyUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(Playing)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        PartyUid = v0;
        if (!ks.Get(out long v1)) return false;
        UnitUid = v1;
        if (!ks.Get(out bool v2)) return false;
        Playing = v2;
        return true;
    }
}

public sealed class KERM_INCREASE_PARTY_FEVER_NOT : IKSerializable
{
    public long PartyUid { get; set; }
    public short SubStageNpcCount { get; set; }
    public sbyte RoomType { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PartyUid)) return false;
        if (!ks.Put(SubStageNpcCount)) return false;
        if (!ks.Put(RoomType)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        PartyUid = v0;
        if (!ks.Get(out short v1)) return false;
        SubStageNpcCount = v1;
        if (!ks.Get(out sbyte v2)) return false;
        RoomType = v2;
        return true;
    }
}

public sealed class KERM_MAX_PARTY_FEVER_NOT : IKSerializable
{
    public long PartyUid { get; set; }
    public int DeltaFever { get; set; } = 10000;
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PartyUid)) return false;
        if (!ks.Put(DeltaFever)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        PartyUid = v0;
        if (!ks.Get(out int v1)) return false;
        DeltaFever = v1;
        return true;
    }
}

public sealed class KERM_GET_ITEM_REQ : IKSerializable
{
    public long RoomUid { get; set; }
    public long UnitUid { get; set; }
    public int DropItemUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(RoomUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(DropItemUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        RoomUid = v0;
        if (!ks.Get(out long v1)) return false;
        UnitUid = v1;
        if (!ks.Get(out int v2)) return false;
        DropItemUid = v2;
        return true;
    }
}

public sealed class KERM_DUNGEON_PLAY_INFO_TO_SERVER_NOT : IKSerializable
{
    public long RoomUid { get; set; }
    public long UnitUid { get; set; }
    public int ComboScore { get; set; }
    public int TechScore { get; set; }
    public int Damaged { get; set; }
    public int NpcKills { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(RoomUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(ComboScore)) return false;
        if (!ks.Put(TechScore)) return false;
        if (!ks.Put(Damaged)) return false;
        if (!ks.Put(NpcKills)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        RoomUid = v0;
        if (!ks.Get(out long v1)) return false;
        UnitUid = v1;
        if (!ks.Get(out int v2)) return false;
        ComboScore = v2;
        if (!ks.Get(out int v3)) return false;
        TechScore = v3;
        if (!ks.Get(out int v4)) return false;
        Damaged = v4;
        if (!ks.Get(out int v5)) return false;
        NpcKills = v5;
        return true;
    }
}

public sealed class KERM_ADD_ON_STAT_REQ : IKSerializable
{
    public long RoomUid { get; set; }
    public long KillerUnitUid { get; set; }
    public long VictimUnitUid { get; set; }
    public bool IsMidKill { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(RoomUid)) return false;
        if (!ks.Put(KillerUnitUid)) return false;
        if (!ks.Put(VictimUnitUid)) return false;
        if (!ks.Put(IsMidKill)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        RoomUid = v0;
        if (!ks.Get(out long v1)) return false;
        KillerUnitUid = v1;
        if (!ks.Get(out long v2)) return false;
        VictimUnitUid = v2;
        if (!ks.Get(out bool v3)) return false;
        IsMidKill = v3;
        return true;
    }
}

public sealed class KERM_BUY_PERSONAL_SHOP_ITEM_REQ : IKSerializable
{
    public long ShopUid { get; set; }
    public long BuyerUnitUid { get; set; }
    public long ItemUid { get; set; }
    public int Quantity { get; set; }
    public bool ViaTradeBoard { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ShopUid)) return false;
        if (!ks.Put(BuyerUnitUid)) return false;
        if (!ks.Put(ItemUid)) return false;
        if (!ks.Put(Quantity)) return false;
        if (!ks.Put(ViaTradeBoard)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        ShopUid = v0;
        if (!ks.Get(out long v1)) return false;
        BuyerUnitUid = v1;
        if (!ks.Get(out long v2)) return false;
        ItemUid = v2;
        if (!ks.Get(out int v3)) return false;
        Quantity = v3;
        if (!ks.Get(out bool v4)) return false;
        ViaTradeBoard = v4;
        return true;
    }
}

public sealed class KERM_BUY_PERSONAL_SHOP_ITEM_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int TotalCost { get; set; }
    public int NetSellerProceeds { get; set; }
    public int CommissionFee { get; set; }
    public bool ShopSoldOut { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(TotalCost)) return false;
        if (!ks.Put(NetSellerProceeds)) return false;
        if (!ks.Put(CommissionFee)) return false;
        if (!ks.Put(ShopSoldOut)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        Ok = v0;
        if (!ks.Get(out int v1)) return false;
        TotalCost = v1;
        if (!ks.Get(out int v2)) return false;
        NetSellerProceeds = v2;
        if (!ks.Get(out int v3)) return false;
        CommissionFee = v3;
        if (!ks.Get(out bool v4)) return false;
        ShopSoldOut = v4;
        return true;
    }
}

// ---- LoginServer Guild / PShopAgency / LocalRanking / Misc packets ----

public sealed class KELG_GUILD_MEMBER_OP_NOT : IKSerializable
{
    public int GuildUid { get; set; }
    public long ActorUnitUid { get; set; }
    public long TargetUnitUid { get; set; }
    public byte NewGrade { get; set; }
    public string Message { get; set; } = string.Empty;
    public string GuildName { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(GuildUid)) return false;
        if (!ks.Put(ActorUnitUid)) return false;
        if (!ks.Put(TargetUnitUid)) return false;
        if (!ks.Put(NewGrade)) return false;
        if (!ks.PutWideString(Message)) return false;
        if (!ks.PutWideString(GuildName)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        GuildUid = v0;
        if (!ks.Get(out long v1)) return false;
        ActorUnitUid = v1;
        if (!ks.Get(out long v2)) return false;
        TargetUnitUid = v2;
        if (!ks.Get(out byte v3)) return false;
        NewGrade = v3;
        if (!ks.GetWideString(out string v4)) return false;
        Message = v4;
        if (!ks.GetWideString(out string v5)) return false;
        GuildName = v5;
        return true;
    }
}

public sealed class KELG_INVITE_GUILD_REQ : IKSerializable
{
    public int GuildUid { get; set; }
    public long SenderUnitUid { get; set; }
    public string SenderNickName { get; set; } = string.Empty;
    public long ReceiverUnitUid { get; set; }
    public string ReceiverNickName { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(GuildUid)) return false;
        if (!ks.Put(SenderUnitUid)) return false;
        if (!ks.PutWideString(SenderNickName)) return false;
        if (!ks.Put(ReceiverUnitUid)) return false;
        if (!ks.PutWideString(ReceiverNickName)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        GuildUid = v0;
        if (!ks.Get(out long v1)) return false;
        SenderUnitUid = v1;
        if (!ks.GetWideString(out string v2)) return false;
        SenderNickName = v2;
        if (!ks.Get(out long v3)) return false;
        ReceiverUnitUid = v3;
        if (!ks.GetWideString(out string v4)) return false;
        ReceiverNickName = v4;
        return true;
    }
}

public sealed class KELG_INVITE_GUILD_REPLY_REQ : IKSerializable
{
    public int GuildUid { get; set; }
    public long ReceiverUnitUid { get; set; }
    public string SenderNickName { get; set; } = string.Empty;
    public sbyte Reason { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(GuildUid)) return false;
        if (!ks.Put(ReceiverUnitUid)) return false;
        if (!ks.PutWideString(SenderNickName)) return false;
        if (!ks.Put(Reason)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        GuildUid = v0;
        if (!ks.Get(out long v1)) return false;
        ReceiverUnitUid = v1;
        if (!ks.GetWideString(out string v2)) return false;
        SenderNickName = v2;
        if (!ks.Get(out sbyte v3)) return false;
        Reason = v3;
        return true;
    }
}

public sealed class KELG_GET_GUILD_USER_LIST_REQ : IKSerializable
{
    public int GuildUid { get; set; }
    public int ViewPage { get; set; } = 1;
    public sbyte GuildUserListType { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(GuildUid)) return false;
        if (!ks.Put(ViewPage)) return false;
        if (!ks.Put(GuildUserListType)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        GuildUid = v0;
        if (!ks.Get(out int v1)) return false;
        ViewPage = v1;
        if (!ks.Get(out sbyte v2)) return false;
        GuildUserListType = v2;
        return true;
    }
}

public sealed class KELG_GET_GUILD_SKILL_REQ : IKSerializable
{
    public int GuildUid { get; set; }
    public int GuildSkillId { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(GuildUid)) return false;
        if (!ks.Put(GuildSkillId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        GuildUid = v0;
        if (!ks.Get(out int v1)) return false;
        GuildSkillId = v1;
        return true;
    }
}

public sealed class KELG_ADMIN_GET_GUILD_SKILL_POINT_REQ : IKSerializable
{
    public int GuildUid { get; set; }
    public int GuildSPoint { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(GuildUid)) return false;
        if (!ks.Put(GuildSPoint)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        GuildUid = v0;
        if (!ks.Get(out int v1)) return false;
        GuildSPoint = v1;
        return true;
    }
}

public sealed class KELG_APPLY_JOIN_GUILD_REQ : IKSerializable
{
    public int GuildUid { get; set; }
    public long UnitUid { get; set; }
    public string NickName { get; set; } = string.Empty;
    public byte UnitClass { get; set; }
    public byte Level { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool OldApplyDelete { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(GuildUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutWideString(NickName)) return false;
        if (!ks.Put(UnitClass)) return false;
        if (!ks.Put(Level)) return false;
        if (!ks.PutWideString(Message)) return false;
        if (!ks.Put(OldApplyDelete)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        GuildUid = v0;
        if (!ks.Get(out long v1)) return false;
        UnitUid = v1;
        if (!ks.GetWideString(out string v2)) return false;
        NickName = v2;
        if (!ks.Get(out byte v3)) return false;
        UnitClass = v3;
        if (!ks.Get(out byte v4)) return false;
        Level = v4;
        if (!ks.GetWideString(out string v5)) return false;
        Message = v5;
        if (!ks.Get(out bool v6)) return false;
        OldApplyDelete = v6;
        return true;
    }
}

public sealed class KELG_GET_APPLY_JOIN_GUILD_LIST_REQ : IKSerializable
{
    public int GuildUid { get; set; }
    public int ViewPage { get; set; } = 1;
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(GuildUid)) return false;
        if (!ks.Put(ViewPage)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        GuildUid = v0;
        if (!ks.Get(out int v1)) return false;
        ViewPage = v1;
        return true;
    }
}

public sealed class KELG_ACCEPT_JOIN_GUILD_REQ : IKSerializable
{
    public int GuildUid { get; set; }
    public long TargetUnitUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(GuildUid)) return false;
        if (!ks.Put(TargetUnitUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        GuildUid = v0;
        if (!ks.Get(out long v1)) return false;
        TargetUnitUid = v1;
        return true;
    }
}

public sealed class KELG_CHECK_EXPAND_GUILD_MAX_MEMBER_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public int GuildUid { get; set; }
    public long ItemUid { get; set; }
    public int ExpandNum { get; set; } = 10;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(GuildUid)) return false;
        if (!ks.Put(ItemUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        UnitUid = v0;
        if (!ks.Get(out int v1)) return false;
        GuildUid = v1;
        if (!ks.Get(out long v2)) return false;
        ItemUid = v2;
        return true;
    }
}

public sealed class KELG_CHANGE_GUILD_NAME_REQ : IKSerializable
{
    public int GuildUid { get; set; }
    public string NewGuildName { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(GuildUid)) return false;
        if (!ks.PutWideString(NewGuildName)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        GuildUid = v0;
        if (!ks.GetWideString(out string v1)) return false;
        NewGuildName = v1;
        return true;
    }
}

public sealed class KERM_OPEN_PSHOP_AGENCY_REQ : IKSerializable
{
    public long HostUnitUid { get; set; }
    public string HostNickName { get; set; } = string.Empty;
    public string ShopName { get; set; } = string.Empty;
    public int DurationMinutes { get; set; } = 1440;
    public List<KAgencyShopItemDto> Items { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(HostUnitUid)) return false;
        if (!ks.PutWideString(HostNickName)) return false;
        if (!ks.PutWideString(ShopName)) return false;
        if (!ks.Put(DurationMinutes)) return false;
        if (!ks.PutList(Items)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        HostUnitUid = v0;
        if (!ks.GetWideString(out string v1)) return false;
        HostNickName = v1;
        if (!ks.GetWideString(out string v2)) return false;
        ShopName = v2;
        if (!ks.Get(out int v3)) return false;
        DurationMinutes = v3;
        if (!ks.GetList(Items)) return false;
        return true;
    }
}

public sealed class KAgencyShopItemDto : IKSerializable
{
    public long ItemUid { get; set; }
    public int ItemId { get; set; }
    public int Quantity { get; set; }
    public int PricePerUnit { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ItemUid)) return false;
        if (!ks.Put(ItemId)) return false;
        if (!ks.Put(Quantity)) return false;
        if (!ks.Put(PricePerUnit)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        ItemUid = v0;
        if (!ks.Get(out int v1)) return false;
        ItemId = v1;
        if (!ks.Get(out int v2)) return false;
        Quantity = v2;
        if (!ks.Get(out int v3)) return false;
        PricePerUnit = v3;
        return true;
    }
}

public sealed class KERM_OPEN_PSHOP_AGENCY_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long ShopUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(ShopUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        Ok = v0;
        if (!ks.Get(out long v1)) return false;
        ShopUid = v1;
        return true;
    }
}

public sealed class KERM_REG_PSHOP_AGENCY_ITEM_REQ : IKSerializable
{
    public long HostUnitUid { get; set; }
    public List<KAgencyShopItemDto> Items { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(HostUnitUid)) return false;
        if (!ks.PutList(Items)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        HostUnitUid = v0;
        if (!ks.GetList(Items)) return false;
        return true;
    }
}

public sealed class KERM_PICK_UP_FROM_PSHOP_AGENCY_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long PickedEd { get; set; }
    public int RemainingItemCount { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(PickedEd)) return false;
        if (!ks.Put(RemainingItemCount)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        Ok = v0;
        if (!ks.Get(out long v1)) return false;
        PickedEd = v1;
        if (!ks.Get(out int v2)) return false;
        RemainingItemCount = v2;
        return true;
    }
}

public sealed class KELG_LOCAL_RANKING_WATCH_UNIT_REQ : IKSerializable
{
    public long TargetUnitUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(TargetUnitUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        TargetUnitUid = v0;
        return true;
    }
}

public sealed class KELG_LOCAL_RANKING_WATCH_UNIT_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KLocalRankingUserInfo UserInfo { get; set; } = new();
    public KLocalRankingUnitInfo UnitInfo { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(UserInfo)) return false;
        if (!ks.Put(UnitInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        Ok = v0;
        if (!ks.Get(UserInfo)) return false;
        if (!ks.Get(UnitInfo)) return false;
        return true;
    }
}

public sealed class KECN_PARTY_BUFF_UPDATE_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public List<KBuffInfo> BuffInfos { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
        => ks.Put(UnitUid) && ks.PutList(BuffInfos);

    public bool Get(KSerializer ks, RegionProfile region)
        => ks.Get(out long uid) && ks.GetList(BuffInfos) && ((UnitUid = uid) == uid);
}

// ---- Additional CenterServer ECN / EPM / ERM DTOs ----

public sealed class KServerList : IKSerializable
{
    public List<KServerInfo> ServerList { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutList(ServerList)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetList(ServerList)) return false;
        return true;
    }
}

public sealed class KEPM_CHECK_DUNGEON_GAME_PLAY_CONDITION_ACK : IKSerializable
{
    public int Ok { get; set; }
    public string FailUserNickName { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.PutWideString(FailUserNickName)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        Ok = v0;
        if (!ks.GetWideString(out string v1)) return false;
        FailUserNickName = v1;
        return true;
    }
}

public sealed class KEPM_CHECK_FOR_AUTO_PARTY_MAKING_ACK : IKSerializable
{
    public int Ok { get; set; }
    public string FailUserNickName { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.PutWideString(FailUserNickName)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        Ok = v0;
        if (!ks.GetWideString(out string v1)) return false;
        FailUserNickName = v1;
        return true;
    }
}

public sealed class KEPM_CHECK_FOR_PARTY_GAME_START_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long PartyUid { get; set; }
    public ushort EventId { get; set; }
    public short WorldId { get; set; }
    public List<KPartyUserInfo> UserInfos { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(PartyUid)) return false;
        if (!ks.Put(EventId)) return false;
        if (!ks.Put(WorldId)) return false;
        if (!ks.PutList(UserInfos)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        Ok = v0;
        if (!ks.Get(out long v1)) return false;
        PartyUid = v1;
        if (!ks.Get(out ushort v2)) return false;
        EventId = v2;
        if (!ks.Get(out short v3)) return false;
        WorldId = v3;
        if (!ks.GetList(UserInfos)) return false;
        return true;
    }
}

public sealed class KEPM_CHECK_PARTY_CHANGE_HOST_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long PartyUid { get; set; }
    public long OldHostUnitUid { get; set; }
    public long NewHostUnitUid { get; set; }
    public bool ForRegroupParty { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(PartyUid)) return false;
        if (!ks.Put(OldHostUnitUid)) return false;
        if (!ks.Put(NewHostUnitUid)) return false;
        if (!ks.Put(ForRegroupParty)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        Ok = v0;
        if (!ks.Get(out long v1)) return false;
        PartyUid = v1;
        if (!ks.Get(out long v2)) return false;
        OldHostUnitUid = v2;
        if (!ks.Get(out long v3)) return false;
        NewHostUnitUid = v3;
        if (!ks.Get(out bool v4)) return false;
        ForRegroupParty = v4;
        return true;
    }
}

public sealed class KEPM_REQUEST_REGROUP_PARTY_NOT : IKSerializable
{
    public KCreatePartyInfo CreatePartyInfo { get; set; } = new();
    public long OldHostUnitUid { get; set; }
    public short PartyFever { get; set; }
    public List<KPartyUserInfo> Members { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(CreatePartyInfo)) return false;
        if (!ks.Put(OldHostUnitUid)) return false;
        if (!ks.Put(PartyFever)) return false;
        if (!ks.PutList(Members)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(CreatePartyInfo)) return false;
        if (!ks.Get(out long v1)) return false;
        OldHostUnitUid = v1;
        if (!ks.Get(out short v2)) return false;
        PartyFever = v2;
        if (!ks.GetList(Members)) return false;
        return true;
    }
}

public sealed class KEPM_REQUEST_UPDATE_PARTY_MEMBER_LIST_NOT : IKSerializable
{
    public long PartyUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PartyUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        PartyUid = v0;
        return true;
    }
}

public sealed class KEPM_UPDATE_PARTY_USER_JOIN_ROOM_INFO_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public long RoomUid { get; set; }
    public sbyte State { get; set; }
    public int StateCode { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(RoomUid)) return false;
        if (!ks.Put(State)) return false;
        if (!ks.Put(StateCode)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        UnitUid = v0;
        if (!ks.Get(out long v1)) return false;
        RoomUid = v1;
        if (!ks.Get(out sbyte v2)) return false;
        State = v2;
        if (!ks.Get(out int v3)) return false;
        StateCode = v3;
        return true;
    }
}

public sealed class KEPM_UPDATE_PVP_INFO_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public sbyte Rank { get; set; }
    public int OfficialMatchCnt { get; set; }
    public int Rating { get; set; } = 1000;
    public int RPoint { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(Rank)) return false;
        if (!ks.Put(OfficialMatchCnt)) return false;
        if (!ks.Put(Rating)) return false;
        if (!ks.Put(RPoint)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        UnitUid = v0;
        if (!ks.Get(out sbyte v1)) return false;
        Rank = v1;
        if (!ks.Get(out int v2)) return false;
        OfficialMatchCnt = v2;
        if (!ks.Get(out int v3)) return false;
        Rating = v3;
        if (!ks.Get(out int v4)) return false;
        RPoint = v4;
        return true;
    }
}

public sealed class KERM_BATTLE_FIELD_JOIN_USER_LOAD_COMPLETE_NOT : IKSerializable
{
    public long LoadedUnitUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(LoadedUnitUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        LoadedUnitUid = v0;
        return true;
    }
}

public sealed class KERM_BOSS_FIELD_LOG_NOT : IKSerializable
{
    public Dictionary<long, long> GivenDamage { get; set; } = new();
    public Dictionary<long, long> AttackDamage { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutMap(GivenDamage, static (s, v) => s.Put(v), static (s, v) => s.Put(v))) return false;
        if (!ks.PutMap(AttackDamage, static (s, v) => s.Put(v), static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetMap(GivenDamage, static s => (s.Get(out long v), v), static s => (s.Get(out long v), v))) return false;
        if (!ks.GetMap(AttackDamage, static s => (s.Get(out long v), v), static s => (s.Get(out long v), v))) return false;
        return true;
    }
}

public sealed class KERM_BREAK_PERSONAL_SHOP_REQ : IKSerializable
{
    public int Reason { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Reason)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        Reason = v0;
        return true;
    }
}

public sealed class KERM_CALL_MY_LOVER_CHECK_ROOM_REQ : IKSerializable
{
    public KVector3 Pos { get; set; } = new();
    public byte LastTouchLineIndex { get; set; }
    public long LoverUnitUid { get; set; }
    public sbyte CallType { get; set; }
    public int ChannelId { get; set; }
    public long CalledUserUid { get; set; }
    public long CalledGsUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Pos)) return false;
        if (!ks.Put(LastTouchLineIndex)) return false;
        if (!ks.Put(LoverUnitUid)) return false;
        if (!ks.Put(CallType)) return false;
        if (!ks.Put(ChannelId)) return false;
        if (!ks.Put(CalledUserUid)) return false;
        if (!ks.Put(CalledGsUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(Pos)) return false;
        if (!ks.Get(out byte v1)) return false;
        LastTouchLineIndex = v1;
        if (!ks.Get(out long v2)) return false;
        LoverUnitUid = v2;
        if (!ks.Get(out sbyte v3)) return false;
        CallType = v3;
        if (!ks.Get(out int v4)) return false;
        ChannelId = v4;
        if (!ks.Get(out long v5)) return false;
        CalledUserUid = v5;
        if (!ks.Get(out long v6)) return false;
        CalledGsUid = v6;
        return true;
    }
}

public sealed class KERM_CALL_MY_LOVER_JOIN_BATTLE_FIELD_REQ : IKSerializable
{
    public long CalledUserUid { get; set; }
    public long CalledUnitUid { get; set; }
    public long CalledGsUid { get; set; }
    public long RoomUid { get; set; }
    public KVector3 Pos { get; set; } = new();
    public KRoomUserInfo RoomUserInfo { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(CalledUserUid)) return false;
        if (!ks.Put(CalledUnitUid)) return false;
        if (!ks.Put(CalledGsUid)) return false;
        if (!ks.Put(RoomUid)) return false;
        if (!ks.Put(Pos)) return false;
        if (!ks.Put(RoomUserInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        CalledUserUid = v0;
        if (!ks.Get(out long v1)) return false;
        CalledUnitUid = v1;
        if (!ks.Get(out long v2)) return false;
        CalledGsUid = v2;
        if (!ks.Get(out long v3)) return false;
        RoomUid = v3;
        if (!ks.Get(Pos)) return false;
        if (!ks.Get(RoomUserInfo)) return false;
        return true;
    }
}

public sealed class KERM_CHANGED_PET_SATIETY_INTIMACY_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public long PetUid { get; set; }
    public short CurrentSatiety { get; set; }
    public int CurrentIntimacy { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(PetUid)) return false;
        if (!ks.Put(CurrentSatiety)) return false;
        if (!ks.Put(CurrentIntimacy)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        UnitUid = v0;
        if (!ks.Get(out long v1)) return false;
        PetUid = v1;
        if (!ks.Get(out short v2)) return false;
        CurrentSatiety = v2;
        if (!ks.Get(out int v3)) return false;
        CurrentIntimacy = v3;
        return true;
    }
}

public sealed class KERM_CHANGE_EQUIPPED_ITEM_IN_ROOM_NOT : IKSerializable
{
    public sbyte ChangeEquippedPacketType { get; set; }
    public long UnitUid { get; set; }
    public List<KInventoryItemInfo> InventorySlotInfo { get; set; } = new();
    public int EquippedTitleId { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ChangeEquippedPacketType)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutList(InventorySlotInfo)) return false;
        if (!ks.Put(EquippedTitleId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte v0)) return false;
        ChangeEquippedPacketType = v0;
        if (!ks.Get(out long v1)) return false;
        UnitUid = v1;
        if (!ks.GetList(InventorySlotInfo)) return false;
        if (!ks.Get(out int v3)) return false;
        EquippedTitleId = v3;
        return true;
    }
}

public sealed class KERM_CHANGE_EQUIPPED_ITEM_IN_SQUARE_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public List<KInventoryItemInfo> InventorySlotInfo { get; set; } = new();
    public int EquippedTitleId { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutList(InventorySlotInfo)) return false;
        if (!ks.Put(EquippedTitleId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        UnitUid = v0;
        if (!ks.GetList(InventorySlotInfo)) return false;
        if (!ks.Get(out int v2)) return false;
        EquippedTitleId = v2;
        return true;
    }
}

public sealed class KERM_CHAR_LEVEL_UP_NOT : IKSerializable
{
    public KStat BaseStat { get; set; } = new();
    public KRoomUserInfo RoomUserInfo { get; set; } = new();
    public int NoteViewCount { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(BaseStat)) return false;
        if (!ks.Put(RoomUserInfo)) return false;
        if (!ks.Put(NoteViewCount)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(BaseStat)) return false;
        if (!ks.Get(RoomUserInfo)) return false;
        if (!ks.Get(out int v2)) return false;
        NoteViewCount = v2;
        return true;
    }
}

public sealed class KERM_CONFIRM_BUY_PERSONAL_SHOP_ITEM_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long BuyUnitUid { get; set; }
    public long BuyItemUid { get; set; }
    public int BuyQuantity { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(BuyUnitUid)) return false;
        if (!ks.Put(BuyItemUid)) return false;
        if (!ks.Put(BuyQuantity)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        Ok = v0;
        if (!ks.Get(out long v1)) return false;
        BuyUnitUid = v1;
        if (!ks.Get(out long v2)) return false;
        BuyItemUid = v2;
        if (!ks.Get(out int v3)) return false;
        BuyQuantity = v3;
        return true;
    }
}

public sealed class KERM_CONFIRM_TRADE_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KItemQuantityUpdate ItemQuantityUpdateTemp { get; set; } = new();
    public List<KItemInfo> ItemInfoTemp { get; set; } = new();
    public int EdTemp { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
        => ks.Put(Ok)
        && ks.Put(ItemQuantityUpdateTemp)
        && ks.PutList(ItemInfoTemp)
        && ks.Put(EdTemp);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.Get(ItemQuantityUpdateTemp) || !ks.GetList(ItemInfoTemp) || !ks.Get(out int ed)) return false;
        Ok = ok; EdTemp = ed;
        return true;
    }
}

public sealed class KERM_COUPLE_MAKING_SUCCESS_NOT : IKSerializable
{
    public long RequestUnitUid { get; set; }
    public string RequestUnitName { get; set; } = string.Empty;
    public long AcceptUnitUid { get; set; }
    public string AcceptUnitName { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(RequestUnitUid)) return false;
        if (!ks.PutWideString(RequestUnitName)) return false;
        if (!ks.Put(AcceptUnitUid)) return false;
        if (!ks.PutWideString(AcceptUnitName)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        RequestUnitUid = v0;
        if (!ks.GetWideString(out string v1)) return false;
        RequestUnitName = v1;
        if (!ks.Get(out long v2)) return false;
        AcceptUnitUid = v2;
        if (!ks.GetWideString(out string v3)) return false;
        AcceptUnitName = v3;
        return true;
    }
}

public sealed class KERM_DB_UPDATE_TRADE_RESULT_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public int ChangedEd { get; set; }
    public List<KInventoryItemInfo> UpdatedItems { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
        => ks.Put(UnitUid) && ks.Put(ChangedEd) && ks.PutList(UpdatedItems);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid) || !ks.Get(out int ed) || !ks.GetList(UpdatedItems))
            return false;
        UnitUid = uid;
        ChangedEd = ed;
        return true;
    }
}

public sealed class KERM_DUMP_BATTLE_FIELD_NOT : IKSerializable
{
    public string NickName { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutWideString(NickName)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string v0)) return false;
        NickName = v0;
        return true;
    }
}

public sealed class KERM_DUMP_ROOM_MONSTER_NOT : IKSerializable
{
    public string NickName { get; set; } = string.Empty;
    public int NpcUid { get; set; } = -1;
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutWideString(NickName)) return false;
        if (!ks.Put(NpcUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string v0)) return false;
        NickName = v0;
        if (!ks.Get(out int v1)) return false;
        NpcUid = v1;
        return true;
    }
}

public sealed class KERM_DUNGEON_SUB_STAGE_OPEN_REQ : IKSerializable
{
    public sbyte SubStageId { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(SubStageId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte v0)) return false;
        SubStageId = v0;
        return true;
    }
}

public sealed class KERM_GET_ITEM_COMPLETE_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public Dictionary<int, int> GetItems { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutMap(GetItems, static (s, v) => s.Put(v), static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        UnitUid = v0;
        if (!ks.GetMap(GetItems, static s => (s.Get(out int v), v), static s => (s.Get(out int v), v))) return false;
        return true;
    }
}

public sealed class KELG_GET_UNIT_INFO_ACK : IKSerializable
{
    public ushort EventId { get; set; }
    public long DemandUserUid { get; set; }
    public long DemandGsUid { get; set; }
    public int Ok { get; set; }
    public KConnectionUnitInfo CUnitInfo { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(EventId)) return false;
        if (!ks.Put(DemandUserUid)) return false;
        if (!ks.Put(DemandGsUid)) return false;
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(CUnitInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out ushort v0)) return false;
        EventId = v0;
        if (!ks.Get(out long v1)) return false;
        DemandUserUid = v1;
        if (!ks.Get(out long v2)) return false;
        DemandGsUid = v2;
        if (!ks.Get(out int v3)) return false;
        Ok = v3;
        if (!ks.Get(CUnitInfo)) return false;
        return true;
    }
}

public sealed class KERM_KILL_ZOMBIE_USER_IN_BATTLE_FIELD_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public string NickName { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutWideString(NickName)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        UnitUid = v0;
        if (!ks.GetWideString(out string v1)) return false;
        NickName = v1;
        return true;
    }
}

public sealed class KERM_LEAVE_BATTLE_FIELD_FOR_JOIN_WEDDING_HALL_REQ : IKSerializable
{
    public KWeddingHallInfo WeddingInfo { get; set; } = new();
    public sbyte UnitWeddingStatus { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(WeddingInfo)) return false;
        if (!ks.Put(UnitWeddingStatus)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(WeddingInfo)) return false;
        if (!ks.Get(out sbyte v1)) return false;
        UnitWeddingStatus = v1;
        return true;
    }
}

public sealed class KERM_MY_USER_UNIT_INFO_TO_SERVER_REQ : IKSerializable
{
    public KRoomUserInfo RoomUserInfo { get; set; } = new();
    public KDungeonPlayResultInfo DungeonUnitInfo { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(RoomUserInfo)) return false;
        if (!ks.Put(DungeonUnitInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(RoomUserInfo)) return false;
        if (!ks.Get(DungeonUnitInfo)) return false;
        return true;
    }
}

public sealed class KERM_COMPLETE_RETURN_TO_FIELD_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public int FieldMapId { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(FieldMapId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        UnitUid = v0;
        if (!ks.Get(out int v1)) return false;
        FieldMapId = v1;
        return true;
    }
}

public sealed class KERM_PET_ACTION_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public sbyte ActionType { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(ActionType)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        UnitUid = v0;
        if (!ks.Get(out sbyte v1)) return false;
        ActionType = v1;
        return true;
    }
}

public sealed class KERM_PET_EVOLUTION_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public KPetInfo EvolutionPetInfo { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(EvolutionPetInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        UnitUid = v0;
        if (!ks.Get(EvolutionPetInfo)) return false;
        return true;
    }
}

public sealed class KERM_PREPARE_FOR_DEFENCE_DUNGEON_ACK : IKSerializable
{
    public KRoomUserInfo RoomUserInfo { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(RoomUserInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(RoomUserInfo)) return false;
        return true;
    }
}

public sealed class KERM_REG_PERSONAL_SHOP_ITEM_REQ : IKSerializable
{
    public long ShopUid { get; set; }
    public string PersonalShopName { get; set; } = string.Empty;
    public List<KAgencyShopItemDto> SellItems { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ShopUid)) return false;
        if (!ks.PutWideString(PersonalShopName)) return false;
        if (!ks.PutList(SellItems)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        ShopUid = v0;
        if (!ks.GetWideString(out string v1)) return false;
        PersonalShopName = v1;
        if (!ks.GetList(SellItems)) return false;
        return true;
    }
}

public sealed class KERM_REQUEST_TRADE_NOT : IKSerializable
{
    public long SenderUid { get; set; }
    public string SenderNickName { get; set; } = string.Empty;
    public long ReceiverUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(SenderUid)) return false;
        if (!ks.PutWideString(SenderNickName)) return false;
        if (!ks.Put(ReceiverUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        SenderUid = v0;
        if (!ks.GetWideString(out string v1)) return false;
        SenderNickName = v1;
        if (!ks.Get(out long v2)) return false;
        ReceiverUid = v2;
        return true;
    }
}

public sealed class KERM_REQUEST_TRADE_REPLY_NOT : IKSerializable
{
    public long ReceiverUid { get; set; }
    public int Ok { get; set; }
    public long MemberUid { get; set; }
    public long MemberGsUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(ReceiverUid)) return false;
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(MemberUid)) return false;
        if (!ks.Put(MemberGsUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        ReceiverUid = v0;
        if (!ks.Get(out int v1)) return false;
        Ok = v1;
        if (!ks.Get(out long v2)) return false;
        MemberUid = v2;
        if (!ks.Get(out long v3)) return false;
        MemberGsUid = v3;
        return true;
    }
}

public sealed class KERM_RESTORE_SPIRIT_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public int Spirit { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(Spirit)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        UnitUid = v0;
        if (!ks.Get(out int v1)) return false;
        Spirit = v1;
        return true;
    }
}

public sealed class KERM_SET_QUEST_ITEM_INFO_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public Dictionary<int, KSubQuestInfo> DropQuestItemByIngQuest { get; set; } = new();
    public HashSet<int> QuestInfo { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutMap(DropQuestItemByIngQuest, static (s, v) => s.Put(v), static (s, v) => s.Put(v))) return false;
        if (!ks.PutList(QuestInfo, static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        UnitUid = v0;
        if (!ks.GetMap(DropQuestItemByIngQuest, static s => (s.Get(out int v), v), static s => { var v = new KSubQuestInfo(); return (s.Get(v), v); })) return false;
        if (!ks.GetHashSet(QuestInfo, static s => (s.Get(out int v), v))) return false;
        return true;
    }
}

public sealed class KERM_SQUARE_UNIT_REFRESH_ED_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public int Ed { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(Ed)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        UnitUid = v0;
        if (!ks.Get(out int v1)) return false;
        Ed = v1;
        return true;
    }
}

public sealed class KERM_SUMMON_PET_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public List<KPetInfo> PetInfo { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutList(PetInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        UnitUid = v0;
        if (!ks.GetList(PetInfo)) return false;
        return true;
    }
}

public sealed class KERM_TUTORIAL_DUNGEON_ROOM_UPDATE_NOT : IKSerializable
{
    public List<long> StudentUnitUids { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutList(StudentUnitUids, static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetList(StudentUnitUids, static s => (s.Get(out long v), v))) return false;
        return true;
    }
}

public sealed class KERM_UPDATE_BUFF_INFO_IN_ROOM_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public List<KBuffInfo> ActivateBuffList { get; set; } = new();
    public KStat GameStat { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutList(ActivateBuffList)) return false;
        if (!ks.Put(GameStat)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        UnitUid = v0;
        if (!ks.GetList(ActivateBuffList)) return false;
        if (!ks.Get(GameStat)) return false;
        return true;
    }
}

public sealed class KERM_UPDATE_PARTY_INFO_TO_BATTLE_FIELD_NOT : IKSerializable
{
    public long PartyUid { get; set; }
    public List<long> PartyUnitUidList { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PartyUid)) return false;
        if (!ks.PutList(PartyUnitUidList, static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        PartyUid = v0;
        if (!ks.GetList(PartyUnitUidList, static s => (s.Get(out long v), v))) return false;
        return true;
    }
}

public sealed class KERM_UPDATE_PVP_PLAY_UNIT_INFO_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public int KillNum { get; set; }
    public int MDKillNum { get; set; }
    public int DeathNum { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(KillNum)) return false;
        if (!ks.Put(MDKillNum)) return false;
        if (!ks.Put(DeathNum)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        UnitUid = v0;
        if (!ks.Get(out int v1)) return false;
        KillNum = v1;
        if (!ks.Get(out int v2)) return false;
        MDKillNum = v2;
        if (!ks.Get(out int v3)) return false;
        DeathNum = v3;
        return true;
    }
}

public sealed class KSquarePersonalShopInfo : IKSerializable
{
    public const sbyte UtNone = 0;
    public const sbyte UtAll = 1;
    public const sbyte UtStateName = 2;
    public const sbyte UtDataClear = 3;

    public sbyte UpdateType { get; set; }
    public sbyte PersonalShopState { get; set; }
    public sbyte PersonalShopType { get; set; }
    public long PersonalShopUid { get; set; }
    public string PersonalShopName { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UpdateType)) return false;
        if (!ks.Put(PersonalShopState)) return false;
        if (!ks.Put(PersonalShopType)) return false;
        if (!ks.Put(PersonalShopUid)) return false;
        if (!ks.PutWideString(PersonalShopName)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte v0)) return false; UpdateType = v0;
        if (!ks.Get(out sbyte v1)) return false; PersonalShopState = v1;
        if (!ks.Get(out sbyte v2)) return false; PersonalShopType = v2;
        if (!ks.Get(out long v3)) return false; PersonalShopUid = v3;
        if (!ks.GetWideString(out string v4)) return false; PersonalShopName = v4;
        return true;
    }
}

public sealed class KERM_UPDATE_TUTORIAL_INFO_NOT : IKSerializable
{
    public List<long> StudentUnitUids { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutList(StudentUnitUids, static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetList(StudentUnitUids, static s => (s.Get(out long v), v))) return false;
        return true;
    }
}

public sealed class KERM_ZERO_ENDURANCE_ITEM_IN_ROOM_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public List<KInventoryItemInfo> InventorySlotInfo { get; set; } = new();
    public KRoomUserInfo RoomUserInfo { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutList(InventorySlotInfo)) return false;
        if (!ks.Put(RoomUserInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        UnitUid = v0;
        if (!ks.GetList(InventorySlotInfo)) return false;
        if (!ks.Get(RoomUserInfo)) return false;
        return true;
    }
}

// ---- Additional LoginServer ELG / DLG / ERM / EGIANT DTOs ----

public sealed class KELG_REGISTER_USER_REQ : IKSerializable
{
    public long UserUid { get; set; }
    public string UserId { get; set; } = string.Empty;
    public long GameServerUid { get; set; }
    public int ChannelId { get; set; } = 1;
    public long UnitUid { get; set; }
    public string NickName { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
        => ks.Put(UserUid) && ks.PutWideString(UserId) && ks.Put(GameServerUid) &&
           ks.Put(ChannelId) && ks.Put(UnitUid) && ks.PutWideString(NickName);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long userUid) || !ks.GetWideString(out string uid) || !ks.Get(out long gsUid) ||
            !ks.Get(out int chId) || !ks.Get(out long unitUid) || !ks.GetWideString(out string nick))
            return false;
        UserUid = userUid;
        UserId = uid;
        GameServerUid = gsUid;
        ChannelId = chId;
        UnitUid = unitUid;
        NickName = nick;
        return true;
    }
}

public sealed class KELG_CHECK_TIME_RANDOM_ITEM_EVENT_REQ : IKSerializable
{
    public long UserUid { get; set; }
    public long UnitUid { get; set; }
    public int OpenItemId { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UserUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(OpenItemId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        UserUid = v0;
        if (!ks.Get(out long v1)) return false;
        UnitUid = v1;
        if (!ks.Get(out int v2)) return false;
        OpenItemId = v2;
        return true;
    }
}

public sealed class KELG_CREATE_GUILD_RESULT_REQ : IKSerializable
{
    public int Ok { get; set; }
    public long MasterUnitUid { get; set; }
    public KGuildInfo GuildInfo { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(MasterUnitUid)) return false;
        if (!ks.Put(GuildInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        Ok = v0;
        if (!ks.Get(out long v1)) return false;
        MasterUnitUid = v1;
        if (!ks.Get(GuildInfo)) return false;
        return true;
    }
}

public sealed class KELG_MODIFY_REG_GUILD_AD_REQ : IKSerializable
{
    public int GuildUid { get; set; }
    public short PeriodDays { get; set; } = 7;
    public string AdMessage { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(GuildUid)) return false;
        if (!ks.Put(PeriodDays)) return false;
        if (!ks.PutWideString(AdMessage)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        GuildUid = v0;
        if (!ks.Get(out short v1)) return false;
        PeriodDays = v1;
        if (!ks.GetWideString(out string v2)) return false;
        AdMessage = v2;
        return true;
    }
}

public sealed class KELG_COUPLE_PROPOSE_CHECK_CONNECT_USER_REQ : IKSerializable
{
    public long RequestUnitUid { get; set; }
    public string RequestNickName { get; set; } = string.Empty;
    public string TargetNickName { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(RequestUnitUid)) return false;
        if (!ks.PutWideString(RequestNickName)) return false;
        if (!ks.PutWideString(TargetNickName)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        RequestUnitUid = v0;
        if (!ks.GetWideString(out string v1)) return false;
        RequestNickName = v1;
        if (!ks.GetWideString(out string v2)) return false;
        TargetNickName = v2;
        return true;
    }
}

public sealed class KELG_COUPLE_PROPOSE_RESULT_NOT : IKSerializable
{
    public long RequestUnitUid { get; set; }
    public long AcceptUnitUid { get; set; }
    public string AcceptNickName { get; set; } = string.Empty;
    public int Ok { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(RequestUnitUid)) return false;
        if (!ks.Put(AcceptUnitUid)) return false;
        if (!ks.PutWideString(AcceptNickName)) return false;
        if (!ks.Put(Ok)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        RequestUnitUid = v0;
        if (!ks.Get(out long v1)) return false;
        AcceptUnitUid = v1;
        if (!ks.GetWideString(out string v2)) return false;
        AcceptNickName = v2;
        if (!ks.Get(out int v3)) return false;
        Ok = v3;
        return true;
    }
}

public sealed class KELG_FIND_FRIEND_UNIT_INFO_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public List<long> FriendUnitUids { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutList(FriendUnitUids, static (s, v) => s.Put(v))) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        UnitUid = v0;
        if (!ks.GetList(FriendUnitUids, static s => (s.Get(out long v), v))) return false;
        return true;
    }
}

public sealed class KELG_FIND_FRIEND_UNIT_INFO_ACK : IKSerializable
{
    public int Ok { get; set; }
    public List<KConnectionUnitInfo> FriendUnitInfos { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.PutList(FriendUnitInfos)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        Ok = v0;
        if (!ks.GetList(FriendUnitInfos)) return false;
        return true;
    }
}

public sealed class KELG_SEARCH_BLACK_LIST_UNIT_REQ : IKSerializable
{
    public long UnitUid { get; set; }
    public string TargetNickName { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutWideString(TargetNickName)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        UnitUid = v0;
        if (!ks.GetWideString(out string v1)) return false;
        TargetNickName = v1;
        return true;
    }
}

public sealed class KELG_UPDATE_DENY_FRIEND_SHIP_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public long FriendUnitUid { get; set; }
    public bool Deny { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(FriendUnitUid)) return false;
        if (!ks.Put(Deny)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        UnitUid = v0;
        if (!ks.Get(out long v1)) return false;
        FriendUnitUid = v1;
        if (!ks.Get(out bool v2)) return false;
        Deny = v2;
        return true;
    }
}

public sealed class KELG_RECOMMEND_USER_INFO_NOT : IKSerializable
{
    public long TargetUnitUid { get; set; }
    public long RecommenderUnitUid { get; set; }
    public string RecommenderNickName { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(TargetUnitUid)) return false;
        if (!ks.Put(RecommenderUnitUid)) return false;
        if (!ks.PutWideString(RecommenderNickName)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        TargetUnitUid = v0;
        if (!ks.Get(out long v1)) return false;
        RecommenderUnitUid = v1;
        if (!ks.GetWideString(out string v2)) return false;
        RecommenderNickName = v2;
        return true;
    }
}

public sealed class KELG_REGISTER_RECRUITER_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public string RecruiterNickName { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutWideString(RecruiterNickName)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        UnitUid = v0;
        if (!ks.GetWideString(out string v1)) return false;
        RecruiterNickName = v1;
        return true;
    }
}

public sealed class KELG_KNM_TOGETHER_REQ : IKSerializable
{
    public long SenderUnitUid { get; set; }
    public long TargetUnitUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(SenderUnitUid)) return false;
        if (!ks.Put(TargetUnitUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        SenderUnitUid = v0;
        if (!ks.Get(out long v1)) return false;
        TargetUnitUid = v1;
        return true;
    }
}

public sealed class KELG_TUTORIAL_NOT : IKSerializable
{
    public long TeacherUnitUid { get; set; }
    public long StudentUnitUid { get; set; }
    public bool Accepted { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(TeacherUnitUid)) return false;
        if (!ks.Put(StudentUnitUid)) return false;
        if (!ks.Put(Accepted)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        TeacherUnitUid = v0;
        if (!ks.Get(out long v1)) return false;
        StudentUnitUid = v1;
        if (!ks.Get(out bool v2)) return false;
        Accepted = v2;
        return true;
    }
}

public sealed class KELG_INSERT_TITLE_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public int TitleId { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(TitleId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        UnitUid = v0;
        if (!ks.Get(out int v1)) return false;
        TitleId = v1;
        return true;
    }
}

public sealed class KELG_CHAT_OBSERVATION_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public string NickName { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutWideString(NickName)) return false;
        if (!ks.PutWideString(Message)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        UnitUid = v0;
        if (!ks.GetWideString(out string v1)) return false;
        NickName = v1;
        if (!ks.GetWideString(out string v2)) return false;
        Message = v2;
        return true;
    }
}

public sealed class KELG_REG_USER_BLOCK_NOT : IKSerializable
{
    public string UserId { get; set; } = string.Empty;
    public bool Block { get; set; } = true;
    public string Reason { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.PutWideString(UserId)) return false;
        if (!ks.Put(Block)) return false;
        if (!ks.PutWideString(Reason)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out string v0)) return false;
        UserId = v0;
        if (!ks.Get(out bool v1)) return false;
        Block = v1;
        if (!ks.GetWideString(out string v2)) return false;
        Reason = v2;
        return true;
    }
}

public sealed class KELG_LOCAL_RANKING_GUILD_CHANGE_INFO_NOT : IKSerializable
{
    public int GuildUid { get; set; }
    public string GuildName { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(GuildUid)) return false;
        if (!ks.PutWideString(GuildName)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        GuildUid = v0;
        if (!ks.GetWideString(out string v1)) return false;
        GuildName = v1;
        return true;
    }
}

public sealed class KELG_WEDDING_ITEM_FIND_INFO_REQ : IKSerializable
{
    public int WeddingUid { get; set; }
    public long UnitUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(WeddingUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        WeddingUid = v0;
        if (!ks.Get(out long v1)) return false;
        UnitUid = v1;
        return true;
    }
}

public sealed class KELG_USE_COUPON_REQ : IKSerializable
{
    public long AccountUid { get; set; }
    public string CouponSerial { get; set; } = string.Empty;
    public int CouponType { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(AccountUid)) return false;
        if (!ks.PutWideString(CouponSerial)) return false;
        if (!ks.Put(CouponType)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        AccountUid = v0;
        if (!ks.GetWideString(out string v1)) return false;
        CouponSerial = v1;
        if (!ks.Get(out int v2)) return false;
        CouponType = v2;
        return true;
    }
}

public sealed class KEGIANT_INFO_USERONLINE_REQ : IKSerializable
{
    public long UserUid { get; set; }
    public string AccountName { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UserUid)) return false;
        if (!ks.PutWideString(AccountName)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        UserUid = v0;
        if (!ks.GetWideString(out string v1)) return false;
        AccountName = v1;
        return true;
    }
}

public sealed class KERM_UPDATE_PERIOD_PSHOP_AGENCY_NOT : IKSerializable
{
    public long HostUnitUid { get; set; }
    public int AddMinutes { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(HostUnitUid)) return false;
        if (!ks.Put(AddMinutes)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        HostUnitUid = v0;
        if (!ks.Get(out int v1)) return false;
        AddMinutes = v1;
        return true;
    }
}

public sealed class KERM_PSHOP_AGENCY_MESSAGE_NOT : IKSerializable
{
    public long HostUnitUid { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(HostUnitUid)) return false;
        if (!ks.PutWideString(Message)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        HostUnitUid = v0;
        if (!ks.GetWideString(out string v1)) return false;
        Message = v1;
        return true;
    }
}

public sealed class KEGS_CHANGE_TRADE_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public bool Trade { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(Trade)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        UnitUid = v0;
        if (!ks.Get(out bool v1)) return false;
        Trade = v1;
        return true;
    }
}

public sealed class KEPM_LEAVE_PARTY_BY_AUTO_PARTY_NOT : IKSerializable
{
    public long PartyUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PartyUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        PartyUid = v0;
        return true;
    }
}

public sealed class KERM_PREPARE_PICK_UP_FROM_PSHOP_AGENCY_ACK : IKSerializable
{
    public sbyte ShopType { get; set; }
    public List<long> PickUpItemList { get; set; } = new();

    public bool Put(KSerializer ks, RegionProfile region)
        => ks.Put(ShopType) && ks.PutList(PickUpItemList, static (s, v) => s.Put(v));

    public bool Get(KSerializer ks, RegionProfile region)
        => ks.Get(out sbyte st) && ks.GetList(PickUpItemList, static s => (s.Get(out long v), v)) && ((ShopType = st) == st);
}

public sealed class KELG_PCBANG_LOGOUT_NOT : IKSerializable
{
    public long UserUid { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Ip { get; set; } = string.Empty;
    public string SockId { get; set; } = string.Empty;
    public long GsUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UserUid)) return false;
        if (!ks.PutWideString(UserId)) return false;
        if (!ks.PutWideString(Ip)) return false;
        if (!ks.PutWideString(SockId)) return false;
        if (!ks.Put(GsUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        UserUid = v0;
        if (!ks.GetWideString(out string v1)) return false;
        UserId = v1;
        if (!ks.GetWideString(out string v2)) return false;
        Ip = v2;
        if (!ks.GetWideString(out string v3)) return false;
        SockId = v3;
        if (!ks.Get(out long v4)) return false;
        GsUid = v4;
        return true;
    }
}

public sealed class KPurchaserInfo : IKSerializable
{
    public int ServerGroupId { get; set; }
    public long UserUid { get; set; }
    public string UserName { get; set; } = string.Empty;
    public long UnitUid { get; set; }
    public string UnitNickName { get; set; } = string.Empty;
    public byte Level { get; set; }
    public string Ip { get; set; } = string.Empty;
    public uint PublisherUid { get; set; }
    public int ChannelingCode { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
        => ks.Put(ServerGroupId)
        && ks.Put(UserUid)
        && ks.PutWideString(UserName)
        && ks.Put(UnitUid)
        && ks.PutWideString(UnitNickName)
        && ks.Put(Level)
        && ks.PutWideString(Ip)
        && ks.Put(PublisherUid)
        && ks.Put(ChannelingCode);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int sgId) || !ks.Get(out long userUid) || !ks.GetWideString(out string userName) ||
            !ks.Get(out long unitUid) || !ks.GetWideString(out string unitNick) || !ks.Get(out byte level) ||
            !ks.GetWideString(out string ip) || !ks.Get(out uint pubUid) || !ks.Get(out int chCode))
            return false;
        ServerGroupId = sgId; UserUid = userUid; UserName = userName;
        UnitUid = unitUid; UnitNickName = unitNick; Level = level;
        Ip = ip; PublisherUid = pubUid; ChannelingCode = chCode;
        return true;
    }
}

public sealed class KELG_EXCHANGE_CASH_REQ : IKSerializable
{
    public KPurchaserInfo PurchaserInfo { get; set; } = new();
    public int Point { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PurchaserInfo)) return false;
        if (!ks.Put(Point)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(PurchaserInfo)) return false;
        if (!ks.Get(out int v1)) return false;
        Point = v1;
        return true;
    }
}

public sealed class KEBILL_EXCHANGE_CASH_REQ : IKSerializable
{
    public long GsUid { get; set; }
    public long UserUid { get; set; }
    public string Transaction { get; set; } = string.Empty;
    public int Ret { get; set; }
    public int Point { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(GsUid)) return false;
        if (!ks.Put(UserUid)) return false;
        if (!ks.PutWideString(Transaction)) return false;
        if (!ks.Put(Ret)) return false;
        if (!ks.Put(Point)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        GsUid = v0;
        if (!ks.Get(out long v1)) return false;
        UserUid = v1;
        if (!ks.GetWideString(out string v2)) return false;
        Transaction = v2;
        if (!ks.Get(out int v3)) return false;
        Ret = v3;
        if (!ks.Get(out int v4)) return false;
        Point = v4;
        return true;
    }
}

public sealed class KEBILL_EXCHANGE_CASH_ACK : IKSerializable
{
    public int Ok { get; set; }
    public long UnitUid { get; set; }
    public int Point { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(Point)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        Ok = v0;
        if (!ks.Get(out long v1)) return false;
        UnitUid = v1;
        if (!ks.Get(out int v2)) return false;
        Point = v2;
        return true;
    }
}

public sealed class KEBILL_EXCHANGE_CASH_RESERVE_REQ : IKSerializable
{
    public KPurchaserInfo PurchaserInfo { get; set; } = new();
    public int Point { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PurchaserInfo)) return false;
        if (!ks.Put(Point)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(PurchaserInfo)) return false;
        if (!ks.Get(out int v1)) return false;
        Point = v1;
        return true;
    }
}

public sealed class KEBILL_EXCHANGE_CASH_RESERVE_ACK : IKSerializable
{
    public int Ok { get; set; }
    public string Transaction { get; set; } = string.Empty;
    public KEBILL_EXCHANGE_CASH_RESERVE_REQ PacketReq { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.PutWideString(Transaction)) return false;
        if (!ks.Put(PacketReq)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        Ok = v0;
        if (!ks.GetWideString(out string v1)) return false;
        Transaction = v1;
        if (!ks.Get(PacketReq)) return false;
        return true;
    }
}

public sealed class KEBILL_USE_COUPON_REQ : IKSerializable
{
    public KPurchaserInfo PurchaserInfo { get; set; } = new();
    public string SerialCode { get; set; } = string.Empty;
    public string Transaction { get; set; } = string.Empty;
    public int CouponType { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
        => ks.Put(PurchaserInfo) && ks.PutWideString(SerialCode) && ks.PutWideString(Transaction) && ks.Put(CouponType);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(PurchaserInfo) || !ks.GetWideString(out string sc) ||
            !ks.GetWideString(out string tx) || !ks.Get(out int ct))
            return false;
        SerialCode = sc; Transaction = tx; CouponType = ct;
        return true;
    }
}

public sealed class KEBILL_USE_COUPON_RESERVE_ACK : IKSerializable
{
    public int Ok { get; set; }
    public KEBILL_USE_COUPON_REQ PacketReq { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(PacketReq)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        Ok = v0;
        if (!ks.Get(PacketReq)) return false;
        return true;
    }
}

public sealed class KEBILL_USE_COUPON_RESULT_REQ : IKSerializable
{
    public long GsUid { get; set; }
    public long UserUid { get; set; }
    public string Transaction { get; set; } = string.Empty;
    public int Ret { get; set; }
    public int ChargeItem { get; set; }
    public int ChargeItemCnt { get; set; } = 1;
    public int ChargeCash { get; set; }
    public int ChargeBonus { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(GsUid)) return false;
        if (!ks.Put(UserUid)) return false;
        if (!ks.PutWideString(Transaction)) return false;
        if (!ks.Put(Ret)) return false;
        if (!ks.Put(ChargeItem)) return false;
        if (!ks.Put(ChargeItemCnt)) return false;
        if (!ks.Put(ChargeCash)) return false;
        if (!ks.Put(ChargeBonus)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        GsUid = v0;
        if (!ks.Get(out long v1)) return false;
        UserUid = v1;
        if (!ks.GetWideString(out string v2)) return false;
        Transaction = v2;
        if (!ks.Get(out int v3)) return false;
        Ret = v3;
        if (!ks.Get(out int v4)) return false;
        ChargeItem = v4;
        if (!ks.Get(out int v5)) return false;
        ChargeItemCnt = v5;
        if (!ks.Get(out int v6)) return false;
        ChargeCash = v6;
        if (!ks.Get(out int v7)) return false;
        ChargeBonus = v7;
        return true;
    }
}

public sealed class KEBILL_USE_COUPON_RESULT_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int ChargeItem { get; set; }
    public int ChargeItemCnt { get; set; }
    public int ChargeCash { get; set; }
    public int ChargeBonus { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(ChargeItem)) return false;
        if (!ks.Put(ChargeItemCnt)) return false;
        if (!ks.Put(ChargeCash)) return false;
        if (!ks.Put(ChargeBonus)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        Ok = v0;
        if (!ks.Get(out int v1)) return false;
        ChargeItem = v1;
        if (!ks.Get(out int v2)) return false;
        ChargeItemCnt = v2;
        if (!ks.Get(out int v3)) return false;
        ChargeCash = v3;
        if (!ks.Get(out int v4)) return false;
        ChargeBonus = v4;
        return true;
    }
}

public sealed class KEBILL_LOGIN_CHECK_BALANCE_REQ : IKSerializable
{
    public uint PublisherUid { get; set; }
    public string CharName { get; set; } = string.Empty;
    public long SessionUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PublisherUid)) return false;
        if (!ks.PutWideString(CharName)) return false;
        if (!ks.Put(SessionUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint v0)) return false;
        PublisherUid = v0;
        if (!ks.GetWideString(out string v1)) return false;
        CharName = v1;
        if (!ks.Get(out long v2)) return false;
        SessionUid = v2;
        return true;
    }
}

public sealed class KELOG_INSERT_PERSONAL_SHOP_LOG_NOT : IKSerializable
{
    public bool IsSquare { get; set; }
    public sbyte PShopType { get; set; }
    public long FromUnitUid { get; set; }
    public long ToUnitUid { get; set; }
    public long ItemUid { get; set; }
    public int ItemId { get; set; }
    public int Quantity { get; set; }
    public int Ed { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(IsSquare)) return false;
        if (!ks.Put(PShopType)) return false;
        if (!ks.Put(FromUnitUid)) return false;
        if (!ks.Put(ToUnitUid)) return false;
        if (!ks.Put(ItemUid)) return false;
        if (!ks.Put(ItemId)) return false;
        if (!ks.Put(Quantity)) return false;
        if (!ks.Put(Ed)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool v0)) return false;
        IsSquare = v0;
        if (!ks.Get(out sbyte v1)) return false;
        PShopType = v1;
        if (!ks.Get(out long v2)) return false;
        FromUnitUid = v2;
        if (!ks.Get(out long v3)) return false;
        ToUnitUid = v3;
        if (!ks.Get(out long v4)) return false;
        ItemUid = v4;
        if (!ks.Get(out int v5)) return false;
        ItemId = v5;
        if (!ks.Get(out int v6)) return false;
        Quantity = v6;
        if (!ks.Get(out int v7)) return false;
        Ed = v7;
        return true;
    }
}

public sealed class KEPUBLISHER_BILLING_BALANCE_REQ : IKSerializable
{
    public uint PublisherUid { get; set; }
    public long ActorUid { get; set; }
    public string Account { get; set; } = string.Empty;
    public string Ip { get; set; } = string.Empty;
    public string NickName { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
        => ks.Put(PublisherUid) && ks.Put(ActorUid) && ks.PutWideString(Account) &&
           ks.PutWideString(Ip) && ks.PutWideString(NickName);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out uint pubUid) || !ks.Get(out long actorUid) || !ks.GetWideString(out string acct) ||
            !ks.GetWideString(out string ip) || !ks.GetWideString(out string nick))
            return false;
        PublisherUid = pubUid; ActorUid = actorUid; Account = acct; Ip = ip; NickName = nick;
        return true;
    }
}

public sealed class KEPUBLISHER_BILLING_BALANCE_ACK : IKSerializable
{
    public int Ok { get; set; }
    public int Balance { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(Balance)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        Ok = v0;
        if (!ks.Get(out int v1)) return false;
        Balance = v1;
        return true;
    }
}

public sealed class KEPUBLISHER_REG_USER_ACK : IKSerializable
{
    public int Ok { get; set; } = -1;
    public string ServiceAccountId { get; set; } = string.Empty;
    public bool IsPcBang { get; set; }
    public int PcBangType { get; set; } = -1;
    public int CheckState { get; set; } = -99;
    public string SockId { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
        => ks.Put(Ok) && ks.PutWideString(ServiceAccountId) && ks.Put(IsPcBang) &&
           ks.Put(PcBangType) && ks.Put(CheckState) && ks.PutWideString(SockId);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok) || !ks.GetWideString(out string id) || !ks.Get(out bool pcb) ||
            !ks.Get(out int pcbt) || !ks.Get(out int cs) || !ks.GetWideString(out string sock))
            return false;
        Ok = ok; ServiceAccountId = id; IsPcBang = pcb; PcBangType = pcbt; CheckState = cs; SockId = sock;
        return true;
    }
}

public sealed class KEPUBLISHER_UNREG_USER_ACK : IKSerializable
{
    public int Ok { get; set; } = -1;
    public string ServiceAccountId { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.PutWideString(ServiceAccountId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        Ok = v0;
        if (!ks.GetWideString(out string v1)) return false;
        ServiceAccountId = v1;
        return true;
    }
}

public sealed class KEBILL_CHECK_PRODUCT_INFO_ACK : IKSerializable
{
    public int Ok { get; set; }
    public bool Enable { get; set; }
    public int ReleaseTick { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(Ok)) return false;
        if (!ks.Put(Enable)) return false;
        if (!ks.Put(ReleaseTick)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        Ok = v0;
        if (!ks.Get(out bool v1)) return false;
        Enable = v1;
        if (!ks.Get(out int v2)) return false;
        ReleaseTick = v2;
        return true;
    }
}

public sealed class KUserListView : IKSerializable
{
    public long UnitUid { get; set; }
    public string NickName { get; set; } = string.Empty;
    public sbyte UnitClass { get; set; }
    public byte Level { get; set; }
    public sbyte State { get; set; }
    public long RoomUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutWideString(NickName)) return false;
        if (!ks.Put(UnitClass)) return false;
        if (!ks.Put(Level)) return false;
        if (!ks.Put(State)) return false;
        if (!ks.Put(RoomUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        UnitUid = v0;
        if (!ks.GetWideString(out string v1)) return false;
        NickName = v1;
        if (!ks.Get(out sbyte v2)) return false;
        UnitClass = v2;
        if (!ks.Get(out byte v3)) return false;
        Level = v3;
        if (!ks.Get(out sbyte v4)) return false;
        State = v4;
        if (!ks.Get(out long v5)) return false;
        RoomUid = v5;
        return true;
    }
}

public sealed class KECN_NEW_PVP_USER_INFO_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public KUserListView Info { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(Info)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        UnitUid = v0;
        if (!ks.Get(Info)) return false;
        return true;
    }
}

public sealed class KECN_UPDATE_PVP_USER_INFO_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public sbyte State { get; set; }
    public long RoomUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(State)) return false;
        if (!ks.Put(RoomUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        UnitUid = v0;
        if (!ks.Get(out sbyte v1)) return false;
        State = v1;
        if (!ks.Get(out long v2)) return false;
        RoomUid = v2;
        return true;
    }
}

public sealed class KEGB_THE_GATE_OF_DARKNESS_INDIVIDUAL_BUFF_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public int BuffId { get; set; }
    public int RemainSeconds { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(BuffId)) return false;
        if (!ks.Put(RemainSeconds)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        UnitUid = v0;
        if (!ks.Get(out int v1)) return false;
        BuffId = v1;
        if (!ks.Get(out int v2)) return false;
        RemainSeconds = v2;
        return true;
    }
}

public sealed class KEGB_JACKPOT_EVENT_NOTIFY_NOT : IKSerializable
{
    public int EventIdValue { get; set; }
    public long WinnerUnitUid { get; set; }
    public string WinnerNickName { get; set; } = string.Empty;
    public int RewardItemId { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(EventIdValue)) return false;
        if (!ks.Put(WinnerUnitUid)) return false;
        if (!ks.PutWideString(WinnerNickName)) return false;
        if (!ks.Put(RewardItemId)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        EventIdValue = v0;
        if (!ks.Get(out long v1)) return false;
        WinnerUnitUid = v1;
        if (!ks.GetWideString(out string v2)) return false;
        WinnerNickName = v2;
        if (!ks.Get(out int v3)) return false;
        RewardItemId = v3;
        return true;
    }
}

public sealed class KEGB_JACKPOT_EVENT_POST_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public string NickName { get; set; } = string.Empty;
    public int RewardItemId { get; set; }
    public int Quantity { get; set; } = 1;
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutWideString(NickName)) return false;
        if (!ks.Put(RewardItemId)) return false;
        if (!ks.Put(Quantity)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        UnitUid = v0;
        if (!ks.GetWideString(out string v1)) return false;
        NickName = v1;
        if (!ks.Get(out int v2)) return false;
        RewardItemId = v2;
        if (!ks.Get(out int v3)) return false;
        Quantity = v3;
        return true;
    }
}

public sealed class KELG_GUILD_LEVEL_UP_NOTIFY_NOT : IKSerializable
{
    public int GuildUid { get; set; }
    public string GuildName { get; set; } = string.Empty;
    public int GuildLevel { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(GuildUid)) return false;
        if (!ks.PutWideString(GuildName)) return false;
        if (!ks.Put(GuildLevel)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        GuildUid = v0;
        if (!ks.GetWideString(out string v1)) return false;
        GuildName = v1;
        if (!ks.Get(out int v2)) return false;
        GuildLevel = v2;
        return true;
    }
}

public sealed class KLetterInfo : IKSerializable
{
    public long FromUnitUid { get; set; }
    public long ToUnitUid { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public int ItemId { get; set; }
    public int Quantity { get; set; }
    public int Ed { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(FromUnitUid)) return false;
        if (!ks.Put(ToUnitUid)) return false;
        if (!ks.PutWideString(Title)) return false;
        if (!ks.PutWideString(Message)) return false;
        if (!ks.Put(ItemId)) return false;
        if (!ks.Put(Quantity)) return false;
        if (!ks.Put(Ed)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        FromUnitUid = v0;
        if (!ks.Get(out long v1)) return false;
        ToUnitUid = v1;
        if (!ks.GetWideString(out string v2)) return false;
        Title = v2;
        if (!ks.GetWideString(out string v3)) return false;
        Message = v3;
        if (!ks.Get(out int v4)) return false;
        ItemId = v4;
        if (!ks.Get(out int v5)) return false;
        Quantity = v5;
        if (!ks.Get(out int v6)) return false;
        Ed = v6;
        return true;
    }
}

public sealed class KELG_KICK_RESULT_LETTER_NOT : IKSerializable
{
    public long TargetUnitUid { get; set; }
    public string GuildName { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(TargetUnitUid)) return false;
        if (!ks.PutWideString(GuildName)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        TargetUnitUid = v0;
        if (!ks.GetWideString(out string v1)) return false;
        GuildName = v1;
        return true;
    }
}

public sealed class KELG_RANKING_TITLE_REWARD_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public int TitleId { get; set; }
    public int PeriodSeconds { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(TitleId)) return false;
        if (!ks.Put(PeriodSeconds)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        UnitUid = v0;
        if (!ks.Get(out int v1)) return false;
        TitleId = v1;
        if (!ks.Get(out int v2)) return false;
        PeriodSeconds = v2;
        return true;
    }
}

public sealed class KELG_SEND_LETTER_JOIN_GUILD_NOT : IKSerializable
{
    public long TargetUnitUid { get; set; }
    public string GuildName { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(TargetUnitUid)) return false;
        if (!ks.PutWideString(GuildName)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        TargetUnitUid = v0;
        if (!ks.GetWideString(out string v1)) return false;
        GuildName = v1;
        return true;
    }
}

public sealed class KELG_WEB_POINT_REWARD_INSERT_TO_POST_NOT : IKSerializable
{
    public long UnitUid { get; set; }
    public int ItemId { get; set; }
    public int Quantity { get; set; } = 1;
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.Put(ItemId)) return false;
        if (!ks.Put(Quantity)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        UnitUid = v0;
        if (!ks.Get(out int v1)) return false;
        ItemId = v1;
        if (!ks.Get(out int v2)) return false;
        Quantity = v2;
        return true;
    }
}

public sealed class KELG_WEDDING_HALL_INFO_UPDATE_NOT : IKSerializable
{
    public int WeddingUid { get; set; }
    public KWeddingHallInfo HallInfo { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(WeddingUid)) return false;
        if (!ks.Put(HallInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        WeddingUid = v0;
        if (!ks.Get(HallInfo)) return false;
        return true;
    }
}

public sealed class KPartyMemberInfo : IKSerializable
{
    public long UnitUid { get; set; }
    public string NickName { get; set; } = string.Empty;
    public int Level { get; set; }
    public sbyte UnitClass { get; set; }
    public bool Host { get; set; }
    public bool Ready { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(UnitUid)) return false;
        if (!ks.PutWideString(NickName)) return false;
        if (!ks.Put(Level)) return false;
        if (!ks.Put(UnitClass)) return false;
        if (!ks.Put(Host)) return false;
        if (!ks.Put(Ready)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        UnitUid = v0;
        if (!ks.GetWideString(out string v1)) return false;
        NickName = v1;
        if (!ks.Get(out int v2)) return false;
        Level = v2;
        if (!ks.Get(out sbyte v3)) return false;
        UnitClass = v3;
        if (!ks.Get(out bool v4)) return false;
        Host = v4;
        if (!ks.Get(out bool v5)) return false;
        Ready = v5;
        return true;
    }
}

public sealed class KEPM_ADD_PARTY_LIST_NOT : IKSerializable
{
    public KPartyListInfo PartyInfo { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PartyInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(PartyInfo)) return false;
        return true;
    }
}

public sealed class KEPM_DEL_PARTY_LIST_NOT : IKSerializable
{
    public long PartyUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PartyUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        PartyUid = v0;
        return true;
    }
}

public sealed class KEPM_ADD_PARTY_MEMBER_IN_PARTY_LIST_NOT : IKSerializable
{
    public long PartyUid { get; set; }
    public KPartyMemberInfo MemberInfo { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PartyUid)) return false;
        if (!ks.Put(MemberInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        PartyUid = v0;
        if (!ks.Get(MemberInfo)) return false;
        return true;
    }
}

public sealed class KEPM_DEL_PARTY_MEMBER_IN_PARTY_LIST_NOT : IKSerializable
{
    public long PartyUid { get; set; }
    public long UnitUid { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PartyUid)) return false;
        if (!ks.Put(UnitUid)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        PartyUid = v0;
        if (!ks.Get(out long v1)) return false;
        UnitUid = v1;
        return true;
    }
}

public sealed class KEPM_UPDATE_PARTY_FEVER_IN_PARTY_LIST_NOT : IKSerializable
{
    public long PartyUid { get; set; }
    public short FeverPoint { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PartyUid)) return false;
        if (!ks.Put(FeverPoint)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        PartyUid = v0;
        if (!ks.Get(out short v1)) return false;
        FeverPoint = v1;
        return true;
    }
}

public sealed class KEPM_UPDATE_PARTY_IN_PARTY_LIST_NOT : IKSerializable
{
    public KPartyListInfo PartyInfo { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PartyInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(PartyInfo)) return false;
        return true;
    }
}

public sealed class KEPM_UPDATE_PARTY_MEMBER_IN_PARTY_LIST_NOT : IKSerializable
{
    public long PartyUid { get; set; }
    public KPartyMemberInfo MemberInfo { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PartyUid)) return false;
        if (!ks.Put(MemberInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long v0)) return false;
        PartyUid = v0;
        if (!ks.Get(MemberInfo)) return false;
        return true;
    }
}

public sealed class KERM_BATTLE_FIELD_ROOM_INFO_NOT : IKSerializable
{
    public sbyte InfoType { get; set; }
    public KRoomInfo RoomInfo { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(InfoType)) return false;
        if (!ks.Put(RoomInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte v0)) return false;
        InfoType = v0;
        if (!ks.Get(RoomInfo)) return false;
        return true;
    }
}

public sealed class KERM_PERSONAL_SHOP_INFO_NOT : IKSerializable
{
    public sbyte InfoType { get; set; }
    public long ShopUid { get; set; }
    public long HostUnitUid { get; set; }
    public string ShopName { get; set; } = string.Empty;
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(InfoType)) return false;
        if (!ks.Put(ShopUid)) return false;
        if (!ks.Put(HostUnitUid)) return false;
        if (!ks.PutWideString(ShopName)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte v0)) return false;
        InfoType = v0;
        if (!ks.Get(out long v1)) return false;
        ShopUid = v1;
        if (!ks.Get(out long v2)) return false;
        HostUnitUid = v2;
        if (!ks.GetWideString(out string v3)) return false;
        ShopName = v3;
        return true;
    }
}

public sealed class KERM_ROOM_LIST_INFO_NOT : IKSerializable
{
    public sbyte InfoType { get; set; }
    public KRoomInfo RoomInfo { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(InfoType)) return false;
        if (!ks.Put(RoomInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte v0)) return false;
        InfoType = v0;
        if (!ks.Get(RoomInfo)) return false;
        return true;
    }
}

public sealed class KERM_ROOM_COUNT_INFO_NOT : IKSerializable
{
    public int PvpRoomCount { get; set; }
    public int DungeonRoomCount { get; set; }
    public int BattleFieldRoomCount { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(PvpRoomCount)) return false;
        if (!ks.Put(DungeonRoomCount)) return false;
        if (!ks.Put(BattleFieldRoomCount)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        PvpRoomCount = v0;
        if (!ks.Get(out int v1)) return false;
        DungeonRoomCount = v1;
        if (!ks.Get(out int v2)) return false;
        BattleFieldRoomCount = v2;
        return true;
    }
}

public sealed class KERM_SQUARE_LIST_INFO_NOT : IKSerializable
{
    public sbyte InfoType { get; set; }
    public KSquareInfo SquareInfo { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(InfoType)) return false;
        if (!ks.Put(SquareInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte v0)) return false;
        InfoType = v0;
        if (!ks.Get(SquareInfo)) return false;
        return true;
    }
}

public sealed class KERM_UPDATE_FIELD_INFO_NOT : IKSerializable
{
    public int MapId { get; set; }
    public int FieldCount { get; set; }
    public int UserCount { get; set; }
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(MapId)) return false;
        if (!ks.Put(FieldCount)) return false;
        if (!ks.Put(UserCount)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int v0)) return false;
        MapId = v0;
        if (!ks.Get(out int v1)) return false;
        FieldCount = v1;
        if (!ks.Get(out int v2)) return false;
        UserCount = v2;
        return true;
    }
}

public sealed class KERM_WEDDING_HALL_INFO_NOT : IKSerializable
{
    public sbyte InfoType { get; set; }
    public KWeddingHallInfo HallInfo { get; set; } = new();
    public bool Put(KSerializer ks, RegionProfile region)
    {
        if (!ks.Put(InfoType)) return false;
        if (!ks.Put(HallInfo)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte v0)) return false;
        InfoType = v0;
        if (!ks.Get(HallInfo)) return false;
        return true;
    }
}

public sealed class KELG_LOCAL_RANKING_RANK_UP_NOT : KEGS_LOCAL_RANKING_RANK_UP_NOT { }

public sealed class KELG_RECRUIT_RECRUITER_INFO_NOT : IKSerializable
{
    public long SenderUnitUid { get; set; }
    public long ReceiverUnitUid { get; set; }
    public sbyte Type { get; set; }
    public int Data { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) =>
        ks.Put(SenderUnitUid) && ks.Put(ReceiverUnitUid) && ks.Put(Type) && ks.Put(Data);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long sUid) || !ks.Get(out long rUid) || !ks.Get(out sbyte t) || !ks.Get(out int d))
            return false;
        SenderUnitUid = sUid;
        ReceiverUnitUid = rUid;
        Type = t;
        Data = d;
        return true;
    }
}






