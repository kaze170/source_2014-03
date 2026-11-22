using System.Buffers.Binary;

namespace KncWX2Server.ChannelServer.Managers;

/// <summary>
/// Port of ChannelServer/UserSecurityManager.{h,cpp}:
/// Validates 16-byte MachineID checksum (WORD[7] == sum_{i=0..5} WORD[i]*(i+3)).
/// </summary>
public sealed class KUserSecurityManager
{
    public const int MachineIdSize = 16;
    private byte[] _machineId = new byte[MachineIdSize];

    public string MachineIdString { get; private set; } = string.Empty;

    public static bool CheckValidMachineID(ReadOnlySpan<byte> machineId)
    {
        if (machineId.Length != MachineIdSize)
            return false;

        ushort sum = 0;
        for (int i = 0; i < 6; i++)
        {
            ushort w = BinaryPrimitives.ReadUInt16LittleEndian(machineId.Slice(i * 2, 2));
            sum = unchecked((ushort)(sum + w * (i + 3)));
        }

        ushort checksumWord = BinaryPrimitives.ReadUInt16LittleEndian(machineId.Slice(14, 2));
        return sum == checksumWord;
    }

    public bool SetMachineID(byte[] machineId)
    {
        if (!CheckValidMachineID(machineId))
            return false;

        _machineId = (byte[])machineId.Clone();
        MachineIdString = Convert.ToHexString(_machineId);
        return true;
    }
}
