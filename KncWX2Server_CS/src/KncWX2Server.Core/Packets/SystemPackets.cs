using KncWX2Server.Core.Region;
using KncWX2Server.Core.Serialization;

namespace KncWX2Server.Core.Packets;

public enum ToolCcuType : sbyte
{
    Today = 0,
    Yesterday = 1,
    Date = 2,
    NowTime = 3,
}

public sealed class KE_TOOL_GET_CCU_INFO_REQ : IKSerializable
{
    public sbyte CcuType { get; set; }
    public long ServerUid { get; set; }
    public string Date { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
        => ks.Put(CcuType) && ks.Put(ServerUid) && ks.PutWideString(Date);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out sbyte t)) return false;
        if (!ks.Get(out long uid)) return false;
        if (!ks.GetWideString(out var d)) return false;
        CcuType = t;
        ServerUid = uid;
        Date = d;
        return true;
    }
}

public sealed class KE_TOOL_GET_CCU_INFO_ACK : IKSerializable
{
    public int Ok { get; set; }
    public sbyte CcuType { get; set; }
    public long ServerUid { get; set; }
    public string Date { get; set; } = string.Empty;
    public List<KCCUInfo> CcuInfo { get; set; } = [];

    public bool Put(KSerializer ks, RegionProfile region)
        => ks.Put(Ok)
        && ks.Put(CcuType)
        && ks.Put(ServerUid)
        && ks.PutWideString(Date)
        && ks.PutList(CcuInfo);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ok)) return false;
        if (!ks.Get(out sbyte t)) return false;
        if (!ks.Get(out long uid)) return false;
        if (!ks.GetWideString(out var d)) return false;
        if (!ks.GetList(CcuInfo)) return false;
        Ok = ok;
        CcuType = t;
        ServerUid = uid;
        Date = d;
        return true;
    }
}

public sealed class KE_TOOL_CHECK_LOGIN_REQ : IKSerializable
{
    public string Id { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public bool Put(KSerializer ks, RegionProfile region)
        => ks.PutWideString(Id) && ks.PutWideString(Password);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out var id)) return false;
        if (!ks.GetWideString(out var pw)) return false;
        Id = id;
        Password = pw;
        return true;
    }
}

public sealed class KE_TOOL_CHECK_LOGIN_ACK : IKSerializable
{
    public int AuthLevel { get; set; }

    public bool Put(KSerializer ks, RegionProfile region) => ks.Put(AuthLevel);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int al)) return false;
        AuthLevel = al;
        return true;
    }
}

public sealed class KE_TOOL_SERVER_LIST_ACK : IKSerializable
{
    public int ServerCount { get; set; }
    public List<KToolServerList> ServerList { get; set; } = [];

    public bool Put(KSerializer ks, RegionProfile region)
        => ks.Put(ServerCount) && ks.PutList(ServerList);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int cnt)) return false;
        if (!ks.GetList(ServerList)) return false;
        ServerCount = cnt;
        return true;
    }
}
