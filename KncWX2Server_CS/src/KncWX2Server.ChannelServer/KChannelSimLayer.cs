using KncWX2Server.Core.Framework;

namespace KncWX2Server.ChannelServer;

public enum CheckIpType
{
    CIT_ALL_IP_ALLOW = 0,
    CIT_CHECK_IP_ALLOW = 1,
    CIT_CHECK_IP_BLOCK = 2,
}

public enum CheckIpMode
{
    CIM_SERVICE_MODE = 0,
    CIM_CHECK_MODE = 1,
}

/// <summary>
/// Port of ChannelServer/ChannelSimLayer.{h,cpp} supporting service/maintenance IP check modes and Lua setup.
/// </summary>
public sealed class KChannelSimLayer : KSimLayer
{
    private sealed class IpCheckConfig
    {
        public List<string> Prefixes { get; } = [];
        public global::KncWX2Server.ChannelServer.CheckIpType Type { get; set; } = global::KncWX2Server.ChannelServer.CheckIpType.CIT_ALL_IP_ALLOW;
    }

    private readonly IpCheckConfig[] _ipConfigs = [new IpCheckConfig(), new IpCheckConfig()];

    public override void SetCheckIPMode(int mode)
    {
        int resolved = mode == (int)global::KncWX2Server.ChannelServer.CheckIpMode.CIM_CHECK_MODE
            ? (int)global::KncWX2Server.ChannelServer.CheckIpMode.CIM_CHECK_MODE
            : (int)global::KncWX2Server.ChannelServer.CheckIpMode.CIM_SERVICE_MODE;
        CheckIpMode = resolved;
    }

    public override void AddIP(string prefix)
    {
        base.AddIP(prefix);
        if (!string.IsNullOrWhiteSpace(prefix))
            _ipConfigs[(int)global::KncWX2Server.ChannelServer.CheckIpMode.CIM_SERVICE_MODE].Prefixes.Add(prefix);
    }

    public override void SetCheckIPType(int type)
    {
        base.SetCheckIPType(type);
        _ipConfigs[(int)global::KncWX2Server.ChannelServer.CheckIpMode.CIM_SERVICE_MODE].Type = (global::KncWX2Server.ChannelServer.CheckIpType)type;
    }

    public override void AddCheckModeIP(string prefix)
    {
        base.AddCheckModeIP(prefix);
        if (!string.IsNullOrWhiteSpace(prefix))
            _ipConfigs[(int)global::KncWX2Server.ChannelServer.CheckIpMode.CIM_CHECK_MODE].Prefixes.Add(prefix);
    }

    public override void SetCheckModeCheckIPType(int type)
    {
        base.SetCheckModeCheckIPType(type);
        _ipConfigs[(int)global::KncWX2Server.ChannelServer.CheckIpMode.CIM_CHECK_MODE].Type = (global::KncWX2Server.ChannelServer.CheckIpType)type;
    }

    public override void SetHanInitFlag(int flag) => HanInitFlag = flag;
    public global::KncWX2Server.ChannelServer.CheckIpMode GetCheckIPMode()
        => (global::KncWX2Server.ChannelServer.CheckIpMode)CheckIpMode;

    public bool CheckIP(string ip)
    {
        int modeIdx = CheckIpMode == (int)global::KncWX2Server.ChannelServer.CheckIpMode.CIM_CHECK_MODE ? 1 : 0;
        var cfg = _ipConfigs[modeIdx];
        switch (cfg.Type)
        {
            case global::KncWX2Server.ChannelServer.CheckIpType.CIT_ALL_IP_ALLOW:
                return true;

            case global::KncWX2Server.ChannelServer.CheckIpType.CIT_CHECK_IP_ALLOW:
                if (ip == "127.0.0.1" || ip == "::1")
                    return true;
                return cfg.Prefixes.Any(p => ip.StartsWith(p, StringComparison.OrdinalIgnoreCase));

            case global::KncWX2Server.ChannelServer.CheckIpType.CIT_CHECK_IP_BLOCK:
                return !cfg.Prefixes.Any(p => ip.StartsWith(p, StringComparison.OrdinalIgnoreCase));

            default:
                return true;
        }
    }

    public override void RegToLua(Core.Scripting.KScriptManager lua)
    {
        lua.RegisterSimLayer(this);
    }
}
