using KncWX2Server.Core.Network;

namespace KncWX2Server.Core.Framework;

/// <summary>
/// Port of Common/NetLayer.{h,cpp}.
/// </summary>
public abstract class KNetLayer
{
    public ushort Port { get; set; } = 9100;
    public bool CheckSequenceNum { get; set; } = true;
    public uint PacketAuthFailLimit { get; set; } = 100;
    public KIocpServer TcpServer { get; } = new();

    public void SetCheckSequenceNum(bool value) => CheckSequenceNum = value;
    public void SetPacketAuthFailLimit(uint limit) => PacketAuthFailLimit = limit;

    public virtual bool Init() => true;
    public virtual void RegToLua(Scripting.KScriptManager lua) { }

    public virtual void ShutDown()
    {
        TcpServer.StopAsync().GetAwaiter().GetResult();
    }
}
