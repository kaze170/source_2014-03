using System.IO.Compression;

namespace KncWX2Server.Core.Serialization;

/// <summary>
/// Binary buffer matching KNCSDK/Include/Serializer/SerBuffer.{h,cpp}.
/// Supports ZLib level-1 compression prefixed by 4-byte Little-Endian original size.
/// </summary>
public sealed class KSerBuffer
{
    private readonly MemoryStream _stream;
    private int _readOffset;

    public bool IsCompressed { get; private set; }

    public KSerBuffer()
    {
        _stream = new MemoryStream();
    }

    public KSerBuffer(ReadOnlySpan<byte> data, bool isCompressed = false)
    {
        _stream = new MemoryStream(data.Length);
        _stream.Write(data);
        IsCompressed = isCompressed;
    }

    public int Length => (int)_stream.Length;

    public int ReadOffset
    {
        get => _readOffset;
        set => _readOffset = value;
    }

    public void Clear()
    {
        _stream.SetLength(0);
        _readOffset = 0;
        IsCompressed = false;
    }

    public void Write(ReadOnlySpan<byte> data)
    {
        _stream.Write(data);
    }

    public bool Read(Span<byte> dest)
    {
        if (_readOffset + dest.Length > _stream.Length)
            return false;

        var buf = _stream.GetBuffer();
        buf.AsSpan(_readOffset, dest.Length).CopyTo(dest);
        _readOffset += dest.Length;
        return true;
    }

    public byte[] ToArray() => _stream.ToArray();

    public ReadOnlySpan<byte> AsSpan() => _stream.GetBuffer().AsSpan(0, (int)_stream.Length);

    public KSerBuffer Clone() => new(AsSpan(), IsCompressed) { _readOffset = _readOffset };

    /// <summary>
    /// Compresses buffer using ZLib (Fastest == level 1), prefixed with 4-byte Little-Endian origin size.
    /// Matches KSerBuffer::Compress() in SerBuffer.cpp.
    /// </summary>
    public bool Compress()
    {
        if (IsCompressed || Length == 0)
            return false;

        uint originSize = (uint)Length;
        ReadOnlySpan<byte> raw = AsSpan();

        using var outMs = new MemoryStream();
        Span<byte> lenBytes = stackalloc byte[4];
        System.Buffers.Binary.BinaryPrimitives.WriteUInt32LittleEndian(lenBytes, originSize);
        outMs.Write(lenBytes);

        using (var zlib = new ZLibStream(outMs, CompressionLevel.Fastest, leaveOpen: true))
        {
            zlib.Write(raw);
        }

        if (outMs.Length >= originSize)
        {
            // Keep uncompressed if zlib does not shrink payload, matching typical C++ behavior
            return false;
        }

        byte[] compressed = outMs.ToArray();
        _stream.SetLength(0);
        _stream.Write(compressed);
        _readOffset = 0;
        IsCompressed = true;
        return true;
    }

    /// <summary>
    /// Uncompresses buffer from 4-byte Little-Endian origin size + ZLib stream.
    /// Matches KSerBuffer::UnCompress() in SerBuffer.cpp.
    /// </summary>
    public bool UnCompress()
    {
        if (!IsCompressed)
            return false;

        if (Length < 4)
            return false;

        ReadOnlySpan<byte> src = AsSpan();
        uint originSize = System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(src[..4]);
        if (originSize > 64 * 1024 * 1024)
            return false;

        using var inMs = new MemoryStream(src[4..].ToArray());
        using var zlib = new ZLibStream(inMs, CompressionMode.Decompress);
        byte[] output = new byte[originSize];
        int totalRead = 0;
        while (totalRead < output.Length)
        {
            int read = zlib.Read(output, totalRead, output.Length - totalRead);
            if (read == 0)
                break;
            totalRead += read;
        }

        if (totalRead != originSize)
            return false;

        _stream.SetLength(0);
        _stream.Write(output);
        _readOffset = 0;
        IsCompressed = false;
        return true;
    }

    internal void SetRaw(ReadOnlySpan<byte> data, bool compressed)
    {
        _stream.SetLength(0);
        _stream.Write(data);
        _readOffset = 0;
        IsCompressed = compressed;
    }
}
