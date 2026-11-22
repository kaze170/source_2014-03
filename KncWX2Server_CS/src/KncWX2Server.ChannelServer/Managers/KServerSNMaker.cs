using System.Buffers.Binary;

namespace KncWX2Server.ChannelServer.Managers;

/// <summary>
/// Port of ChannelServer/ServerSNMaker.{h,cpp}:
/// Generates and extracts 12-byte encrypted ServerSN tokens (8-byte BE int64 + 4-byte raw CRC32 + fixed 12-byte permutation).
/// </summary>
public static class KServerSNMaker
{
    private static readonly uint[] Crc32Table = BuildCrcTable();
    private static readonly int[] EncryptTable = [5, 3, 2, 11, 4, 8, 9, 10, 6, 1, 0, 7];

    private static uint[] BuildCrcTable()
    {
        var table = new uint[256];
        for (int i = 0; i < 256; i++)
        {
            uint crc = (uint)i << 24;
            for (int j = 0; j < 8; j++)
            {
                crc = (crc & 0x8000_0000u) != 0 ? (crc << 1) ^ 0x04C11DB7u : (crc << 1);
            }
            table[i] = crc;
        }
        return table;
    }

    private static uint CalcCrc32(ReadOnlySpan<byte> data)
    {
        uint crc = 0;
        foreach (byte b in data)
        {
            uint idx = ((crc >> 24) ^ b) & 0xFF;
            crc = (crc << 8) ^ Crc32Table[idx];
        }
        return crc;
    }

    public static bool MakeServerSN(long serverSn, out byte[] outBytes)
    {
        outBytes = new byte[12];
        if (serverSn <= 0) return false;

        Span<byte> raw = stackalloc byte[12];
        BinaryPrimitives.WriteInt64BigEndian(raw[..8], serverSn);
        uint crc = CalcCrc32(raw[..8]);
        // C++ memcpy(&cTemp[8], &crc32, 4) uses native little-endian layout on x86
        BinaryPrimitives.WriteUInt32LittleEndian(raw[8..], crc);

        for (int i = 0; i < 12; i++)
            outBytes[EncryptTable[i]] = raw[i];

        return true;
    }

    public static bool IsEmptyServerSN(ReadOnlySpan<byte> encrypted)
    {
        for (int i = 0; i < encrypted.Length; i++)
        {
            if (encrypted[i] != 0)
                return false;
        }
        return true;
    }

    public static bool CheckServerSN(ReadOnlySpan<byte> encrypted, out long serverSn)
    {
        serverSn = 0;
        if (encrypted.Length != 12) return false;

        Span<byte> raw = stackalloc byte[12];
        for (int i = 0; i < 12; i++)
            raw[i] = encrypted[EncryptTable[i]];

        uint expectedCrc = CalcCrc32(raw[..8]);
        uint actualCrc = BinaryPrimitives.ReadUInt32LittleEndian(raw[8..]);
        if (expectedCrc != actualCrc)
            return false;

        serverSn = BinaryPrimitives.ReadInt64BigEndian(raw[..8]);
        return true;
    }
}
