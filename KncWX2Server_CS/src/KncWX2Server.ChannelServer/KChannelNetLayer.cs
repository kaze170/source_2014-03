using KncWX2Server.Core.Framework;

namespace KncWX2Server.ChannelServer;

/// <summary>
/// Port of ChannelServer/ChannelNetLayer.{h,cpp}.
/// </summary>
public sealed class KChannelNetLayer : KNetLayer
{
    private readonly KChannelServer _server;

    public KChannelNetLayer(KChannelServer server)
    {
        _server = server;
        Port = server.Region.DefaultChannelPort;
    }

    public override bool Init()
    {
        return TcpServer.Start(Port, _server.AcceptSessionAsync, () => _server.Region);
    }

    public override void RegToLua(Core.Scripting.KScriptManager lua)
    {
        lua.RegisterNetLayer(this);
    }
}
