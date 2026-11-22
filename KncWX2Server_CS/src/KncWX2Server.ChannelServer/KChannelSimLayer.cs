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

    public new CheckIpMode CheckIpMode { get; private set; } = CheckIpMode.CIM_SERVICE_MODE;
    public int HanInitFlag { get; private set; } = 1;

    public new void SetCheckIPMode(int mode)
        => CheckIpMode = mode == (int)CheckIpMode.CIM_CHECK_MODE ? CheckIpMode.CIM_CHECK_MODE : CheckIpMode.CIM_SERVICE_MODE;

    public void AddIP(string prefix)
    {
        if (!string.IsNullOrWhiteSpace(prefix))
            _ipConfigs[(int)CheckIpMode.CIM_SERVICE_MODE].Prefixes.Add(prefix);
    }

    public new void SetCheckIPType(int type)
        => _ipConfigs[(int)CheckIpMode.CIM_SERVICE_MODE].Type = (global::KncWX2Server.ChannelServer.CheckIpType)type;

    public void AddCheckModeIP(string prefix)
    {
        if (!string.IsNullOrWhiteSpace(prefix))
            _ipConfigs[(int)CheckIpMode.CIM_CHECK_MODE].Prefixes.Add(prefix);
    }

    public new void SetCheckModeCheckIPType(int type)
        => _ipConfigs[(int)CheckIpMode.CIM_CHECK_MODE].Type = (global::KncWX2Server.ChannelServer.CheckIpType)type;

    public void SetHanInitFlag(int flag) => HanInitFlag = flag;
    public int GetNationFlag() => (int)NationFlag;
    public int GetAuthFlag() => (int)AuthFlag;
    public CheckIpMode GetCheckIPMode() => CheckIpMode;

    public bool CheckIP(string ip)
    {
        var cfg = _ipConfigs[(int)CheckIpMode];
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
