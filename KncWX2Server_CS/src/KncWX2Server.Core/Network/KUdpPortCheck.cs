using System.Buffers.Binary;
using System.Net;
using System.Net.Sockets;

namespace KncWX2Server.Core.Network;

/// <summary>CKTDNUDP/X2UDPPacket packed XPT_PORT_CHECK request and reply.</summary>
internal static class KUdpPortCheck
{
    // CKTDNUDP::SP_END = 16, XPT_BASIC = 16, followed by request and ACK.
    private const byte RequestId = 17;
    private const byte AckId = 18;
    private const int RequestSize = 1 + 8 + 4 + 2;

    public static bool TryCreateReply(ReadOnlySpan<byte> packet, IPEndPoint source, out byte[] reply)
    {
        reply = [];
        if (packet.Length != RequestSize || packet[0] != RequestId || source.Address.AddressFamily != AddressFamily.InterNetwork)
            return false;

        // X2UDPPacket.h uses #pragma pack(1). Address is the raw network-order
        // IPv4 bytes stored in Windows in_addr.s_addr; port is a native LE int.
        reply = new byte[1 + 4 + 4];
        reply[0] = AckId;
        source.Address.GetAddressBytes().CopyTo(reply, 1);
        BinaryPrimitives.WriteInt32LittleEndian(reply.AsSpan(5), source.Port);
        return true;
    }
}
