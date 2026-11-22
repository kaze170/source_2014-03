using KncWX2Server.Core.Region;
using KncWX2Server.Core.Serialization;

namespace KncWX2Server.Core.Packets;

public sealed class KNetAddress : IKSerializable
{
    public string Ip { get; set; } = string.Empty;
    public ushort Port { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
        => ks.PutWideString(Ip) && ks.Put(Port);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out var ip)) return false;
        if (!ks.Get(out ushort port)) return false;
        Ip = ip;
        Port = port;
        return true;
    }
}

public sealed class KServerInfo : IKSerializable
{
    public long Uid { get; set; }
    public string Name { get; set; } = string.Empty;
    public int ServerGroupId { get; set; }
    public int ServerClass { get; set; }
    public string Ip { get; set; } = string.Empty;
    public string PublicIp { get; set; } = string.Empty;
    public string PrivateIp { get; set; } = string.Empty;
    public ushort MasterPort { get; set; }
    public ushort NcUdpPort { get; set; }
    public int MaxUser { get; set; }
    public int CurrentUser { get; set; }
    public bool On { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        bool ok = ks.Put(Uid)
            && ks.PutWideString(Name)
            && ks.Put(ServerGroupId)
            && ks.Put(ServerClass);
        if (!ok) return false;

        if (region.PrivateIpServerNetworking)
        {
            if (!ks.PutWideString(PublicIp) || !ks.PutWideString(PrivateIp)) return false;
        }
        else
        {
            if (!ks.PutWideString(Ip)) return false;
        }

        return ks.Put(MasterPort)
            && ks.Put(NcUdpPort)
            && ks.Put(MaxUser)
            && ks.Put(CurrentUser)
            && ks.Put(On);
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long uid)) return false;
        if (!ks.GetWideString(out var name)) return false;
        if (!ks.Get(out int groupId)) return false;
        if (!ks.Get(out int srvClass)) return false;
        Uid = uid;
        Name = name;
        ServerGroupId = groupId;
        ServerClass = srvClass;

        if (region.PrivateIpServerNetworking)
        {
            if (!ks.GetWideString(out var pub) || !ks.GetWideString(out var priv)) return false;
            PublicIp = pub;
            PrivateIp = priv;
            Ip = pub;
        }
        else
        {
            if (!ks.GetWideString(out var ip)) return false;
            Ip = ip;
            PublicIp = ip;
        }

        if (!ks.Get(out ushort mp)) return false;
        if (!ks.Get(out ushort up)) return false;
        if (!ks.Get(out int maxU)) return false;
        if (!ks.Get(out int curU)) return false;
        if (!ks.Get(out bool on)) return false;
        MasterPort = mp;
        NcUdpPort = up;
        MaxUser = maxU;
        CurrentUser = curU;
        On = on;
        return true;
    }
}

public sealed class KChannelBonusInfo : IKSerializable
{
    public bool Enable { get; set; }
    public int BeginLv { get; set; }
    public int EndLv { get; set; }
    public int PerExp { get; set; }
    public int PerEd { get; set; }

    public KChannelBonusInfo Clone() => new()
    {
        Enable = Enable,
        BeginLv = BeginLv,
        EndLv = EndLv,
        PerExp = PerExp,
        PerEd = PerEd,
    };

    public bool Put(KSerializer ks, RegionProfile region)
        => ks.Put(Enable) && ks.Put(BeginLv) && ks.Put(EndLv) && ks.Put(PerExp) && ks.Put(PerEd);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out bool en)) return false;
        if (!ks.Get(out int b)) return false;
        if (!ks.Get(out int e)) return false;
        if (!ks.Get(out int exp)) return false;
        if (!ks.Get(out int ed)) return false;
        Enable = en;
        BeginLv = b;
        EndLv = e;
        PerExp = exp;
        PerEd = ed;
        return true;
    }
}

public sealed class KChannelInfo : IKSerializable
{
    public int ServerUid { get; set; }
    public int ChannelId { get; set; }
    public string ChannelName { get; set; } = string.Empty;
    public int ServerGroupId { get; set; }
    public string Ip { get; set; } = string.Empty;
    public ushort MasterPort { get; set; }
    public ushort NcUdpPort { get; set; }
    public int MaxUser { get; set; }
    public int CurrentUser { get; set; }
    public int CurPartyCount { get; set; }
    public int PlayGamePartyCount { get; set; }
    public KChannelBonusInfo BonusInfo { get; set; } = new();

    public KChannelInfo Clone() => new()
    {
        ServerUid = ServerUid,
        ChannelId = ChannelId,
        ChannelName = ChannelName,
        ServerGroupId = ServerGroupId,
        Ip = Ip,
        MasterPort = MasterPort,
        NcUdpPort = NcUdpPort,
        MaxUser = MaxUser,
        CurrentUser = CurrentUser,
        CurPartyCount = CurPartyCount,
        PlayGamePartyCount = PlayGamePartyCount,
        BonusInfo = BonusInfo.Clone(),
    };

    public bool Put(KSerializer ks, RegionProfile region)
    {
        bool ok = ks.Put(ServerUid)
            && ks.Put(ChannelId)
            && ks.PutWideString(ChannelName)
            && ks.Put(ServerGroupId)
            && ks.PutWideString(Ip)
            && ks.Put(MasterPort)
            && ks.Put(NcUdpPort)
            && ks.Put(MaxUser)
            && ks.Put(CurrentUser)
            && ks.Put(CurPartyCount)
            && ks.Put(PlayGamePartyCount);
        if (!ok) return false;
        if (region.IntegrateChannelInfo && !BonusInfo.Put(ks, region)) return false;
        return true;
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int suid)) return false;
        if (!ks.Get(out int cid)) return false;
        if (!ks.GetWideString(out var cname)) return false;
        if (!ks.Get(out int sgid)) return false;
        if (!ks.GetWideString(out var ip)) return false;
        if (!ks.Get(out ushort mp)) return false;
        if (!ks.Get(out ushort up)) return false;
        if (!ks.Get(out int maxU)) return false;
        if (!ks.Get(out int curU)) return false;
        if (!ks.Get(out int curP)) return false;
        if (!ks.Get(out int playP)) return false;

        ServerUid = suid;
        ChannelId = cid;
        ChannelName = cname;
        ServerGroupId = sgid;
        Ip = ip;
        MasterPort = mp;
        NcUdpPort = up;
        MaxUser = maxU;
        CurrentUser = curU;
        CurPartyCount = curP;
        PlayGamePartyCount = playP;

        if (region.IntegrateChannelInfo && !BonusInfo.Get(ks, region)) return false;
        return true;
    }
}

public sealed class KServerGroupInfo : IKSerializable
{
    public int Order { get; set; }
    public int ServerGroupUid { get; set; }
    public string ServerGroupName { get; set; } = string.Empty;
    public string ServerIp { get; set; } = string.Empty;
    public ushort MasterPort { get; set; }
    public ushort NcUdpPort { get; set; }
    public bool Enable { get; set; }

    public KServerGroupInfo Clone() => new()
    {
        Order = Order,
        ServerGroupUid = ServerGroupUid,
        ServerGroupName = ServerGroupName,
        ServerIp = ServerIp,
        MasterPort = MasterPort,
        NcUdpPort = NcUdpPort,
        Enable = Enable,
    };

    public bool Put(KSerializer ks, RegionProfile region)
        => ks.Put(Order)
        && ks.Put(ServerGroupUid)
        && ks.PutWideString(ServerGroupName)
        && ks.PutWideString(ServerIp)
        && ks.Put(MasterPort)
        && ks.Put(NcUdpPort)
        && ks.Put(Enable);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int ord)) return false;
        if (!ks.Get(out int uid)) return false;
        if (!ks.GetWideString(out var name)) return false;
        if (!ks.GetWideString(out var ip)) return false;
        if (!ks.Get(out ushort mp)) return false;
        if (!ks.Get(out ushort up)) return false;
        if (!ks.Get(out bool en)) return false;

        Order = ord;
        ServerGroupUid = uid;
        ServerGroupName = name;
        ServerIp = ip;
        MasterPort = mp;
        NcUdpPort = up;
        Enable = en;
        return true;
    }
}

public sealed class KHackingToolInfo : IKSerializable
{
    public string ProcessName { get; set; } = string.Empty;
    public string WindowClassName { get; set; } = string.Empty;
    public sbyte Flag { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
        => ks.PutWideString(ProcessName) && ks.PutWideString(WindowClassName) && ks.Put(Flag);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out var pn)) return false;
        if (!ks.GetWideString(out var wc)) return false;
        if (!ks.Get(out sbyte fl)) return false;
        ProcessName = pn;
        WindowClassName = wc;
        Flag = fl;
        return true;
    }
}

public sealed class KCCUInfo : IKSerializable
{
    public long CcuTime { get; set; }
    public int CcuCount { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
        => ks.Put(CcuTime) && ks.Put(CcuCount);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out long t)) return false;
        if (!ks.Get(out int c)) return false;
        CcuTime = t;
        CcuCount = c;
        return true;
    }
}

public sealed class KToolServerList : IKSerializable
{
    public int ServerUid { get; set; }
    public int ServerSetId { get; set; }
    public int ServerType { get; set; }
    public int ChannelId { get; set; }
    public string ChannelName { get; set; } = string.Empty;
    public string Ip { get; set; } = string.Empty;
    public int Port { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
        => ks.Put(ServerUid)
        && ks.Put(ServerSetId)
        && ks.Put(ServerType)
        && ks.Put(ChannelId)
        && ks.PutWideString(ChannelName)
        && ks.PutWideString(Ip)
        && ks.Put(Port);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int suid)) return false;
        if (!ks.Get(out int ssid)) return false;
        if (!ks.Get(out int st)) return false;
        if (!ks.Get(out int cid)) return false;
        if (!ks.GetWideString(out var cname)) return false;
        if (!ks.GetWideString(out var ip)) return false;
        if (!ks.Get(out int port)) return false;

        ServerUid = suid;
        ServerSetId = ssid;
        ServerType = st;
        ChannelId = cid;
        ChannelName = cname;
        Ip = ip;
        Port = port;
        return true;
    }
}

public sealed class KLogStatInfo : IKSerializable
{
    public string FunctionName { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string LineNum { get; set; } = string.Empty;
    public int Count { get; set; }

    public bool Put(KSerializer ks, RegionProfile region)
        => ks.PutWideString(FunctionName)
        && ks.PutWideString(FileName)
        && ks.PutWideString(LineNum)
        && ks.Put(Count);

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.GetWideString(out var fn)) return false;
        if (!ks.GetWideString(out var file)) return false;
        if (!ks.GetWideString(out var line)) return false;
        if (!ks.Get(out int cnt)) return false;
        FunctionName = fn;
        FileName = file;
        LineNum = line;
        Count = cnt;
        return true;
    }
}

public enum LoginResultType
{
    Success = 0,
    FailUnknown = 1,
    FailReject = 2,
    FailSystemError = 3,
    FailAccountNotExist = 4,
    FailPasswordError = 5,
    FailOtpPasswordError = 6,
    FailAccountBlock = 7,
    FailMatrixNumberError = 8,
    FailAccountNotActivate = 9,
    Sentinel = 10,
}

public sealed class KLoginResultInfo : IKSerializable
{
    public int ServerGroupId { get; set; }
    public long ServerUid { get; set; }
    public List<int> ResultCount { get; set; } = [];

    public bool Put(KSerializer ks, RegionProfile region)
        => ks.Put(ServerGroupId)
        && ks.Put(ServerUid)
        && ks.PutList(ResultCount, static (s, v) => s.Put(v));

    public bool Get(KSerializer ks, RegionProfile region)
    {
        if (!ks.Get(out int sgid)) return false;
        if (!ks.Get(out long suid)) return false;
        if (!ks.GetList(ResultCount, static s => (s.Get(out int v), v))) return false;
        ServerGroupId = sgid;
        ServerUid = suid;
        return true;
    }
}
