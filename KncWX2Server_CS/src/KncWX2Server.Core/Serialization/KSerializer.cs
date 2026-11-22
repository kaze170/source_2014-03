using System.Buffers.Binary;
using System.Text;
using KncWX2Server.Core.Region;

namespace KncWX2Server.Core.Serialization;

public interface IKSerializable
{
    bool Put(KSerializer ks, RegionProfile region);
    bool Get(KSerializer ks, RegionProfile region);
}

/// <summary>
/// Wire-exact equivalent of KSerializer (KNCSDK/Include/Serializer/Serializer.{h,cpp}).
/// Numeric types are Big-Endian; wstring is Big-Endian byte length + UTF-16LE bytes;
/// bool is 1 byte; tags are disabled (m_tagsEnabled = false).
/// </summary>
public sealed class KSerializer
{
    private KSerBuffer? _buffer;
    public RegionProfile Region { get; }

    public KSerializer(RegionProfile? region = null)
    {
        Region = region ?? RegionProfile.Current;
    }

    public bool BeginWriting(KSerBuffer buffer, bool clear = true)
    {
        _buffer = buffer ?? throw new ArgumentNullException(nameof(buffer));
        if (clear)
            _buffer.Clear();
        return true;
    }

    public bool BeginReading(KSerBuffer buffer, bool resetOffset = true)
    {
        _buffer = buffer ?? throw new ArgumentNullException(nameof(buffer));
        if (resetOffset)
            _buffer.ReadOffset = 0;
        return true;
    }

    public void EndWriting() => _buffer = null;
    public void EndReading() => _buffer = null;

    private KSerBuffer Buf => _buffer ?? throw new InvalidOperationException("Serializer buffer is not active.");

    // ---- PUT ----

    public bool Put(bool val)
    {
        Span<byte> b = stackalloc byte[1];
        b[0] = val ? (byte)1 : (byte)0;
        Buf.Write(b);
        return true;
    }

    public bool Put(byte val)
    {
        Span<byte> b = stackalloc byte[1];
        b[0] = val;
        Buf.Write(b);
        return true;
    }

    public bool Put(sbyte val) => Put(unchecked((byte)val));

    public bool Put(short val)
    {
        Span<byte> b = stackalloc byte[2];
        BinaryPrimitives.WriteInt16BigEndian(b, val);
        Buf.Write(b);
        return true;
    }

    public bool Put(ushort val)
    {
        Span<byte> b = stackalloc byte[2];
        BinaryPrimitives.WriteUInt16BigEndian(b, val);
        Buf.Write(b);
        return true;
    }

    public bool Put(int val)
    {
        Span<byte> b = stackalloc byte[4];
        BinaryPrimitives.WriteInt32BigEndian(b, val);
        Buf.Write(b);
        return true;
    }

    public bool Put(uint val)
    {
        Span<byte> b = stackalloc byte[4];
        BinaryPrimitives.WriteUInt32BigEndian(b, val);
        Buf.Write(b);
        return true;
    }

    public bool Put(long val)
    {
        Span<byte> b = stackalloc byte[8];
        BinaryPrimitives.WriteInt64BigEndian(b, val);
        Buf.Write(b);
        return true;
    }

    public bool Put(ulong val)
    {
        Span<byte> b = stackalloc byte[8];
        BinaryPrimitives.WriteUInt64BigEndian(b, val);
        Buf.Write(b);
        return true;
    }

    public bool Put(float val)
    {
        Span<byte> b = stackalloc byte[4];
        BinaryPrimitives.WriteSingleBigEndian(b, val);
        Buf.Write(b);
        return true;
    }

    public bool Put(double val)
    {
        Span<byte> b = stackalloc byte[8];
        BinaryPrimitives.WriteDoubleBigEndian(b, val);
        Buf.Write(b);
        return true;
    }

    public bool PutNarrowString(string? val)
    {
        string s = val ?? string.Empty;
        byte[] bytes = Encoding.UTF8.GetBytes(s);
        Put((uint)bytes.Length);
        if (bytes.Length > 0)
            Buf.Write(bytes);
        return true;
    }

    public bool PutAsciiString(string? val) => PutNarrowString(val);

    public bool PutWideString(string? val)
    {
        string s = val ?? string.Empty;
        // C++ std::wstring on Windows writes DWORD byteCount (Big-Endian) + raw UTF-16LE bytes (NOT per-char swapped).
        byte[] bytes = Encoding.Unicode.GetBytes(s);
        Put((uint)bytes.Length);
        if (bytes.Length > 0)
            Buf.Write(bytes);
        return true;
    }

    public bool Put(KSerBuffer val)
    {
        uint len = (uint)val.Length;
        Put(len);
        if (len > 0)
        {
            Put(val.IsCompressed);
            Buf.Write(val.AsSpan());
        }
        return true;
    }

    public bool Put<T>(T obj) where T : IKSerializable => obj.Put(this, Region);

    public bool PutRawBytes(ReadOnlySpan<byte> bytes)
    {
        Buf.Write(bytes);
        return true;
    }

    public bool PutList<T>(IReadOnlyCollection<T> list, Func<KSerializer, T, bool> putElem)
    {
        if (!Put((uint)list.Count)) return false;
        foreach (var item in list)
        {
            if (!putElem(this, item)) return false;
        }
        return true;
    }

    public bool PutList<T>(IReadOnlyCollection<T> list) where T : IKSerializable
        => PutList(list, static (ks, item) => item.Put(ks, ks.Region));

    public bool PutMap<TKey, TVal>(
        IReadOnlyDictionary<TKey, TVal> map,
        Func<KSerializer, TKey, bool> putKey,
        Func<KSerializer, TVal, bool> putVal) where TKey : notnull
    {
        if (!Put((uint)map.Count)) return false;
        foreach (var kv in map)
        {
            if (!putKey(this, kv.Key)) return false;
            if (!putVal(this, kv.Value)) return false;
        }
        return true;
    }

    // ---- GET ----

    public bool Get(out bool val)
    {
        Span<byte> b = stackalloc byte[1];
        if (!Buf.Read(b)) { val = false; return false; }
        val = b[0] != 0;
        return true;
    }

    public bool Get(out byte val)
    {
        Span<byte> b = stackalloc byte[1];
        if (!Buf.Read(b)) { val = 0; return false; }
        val = b[0];
        return true;
    }

    public bool Get(out sbyte val)
    {
        bool ok = Get(out byte u);
        val = unchecked((sbyte)u);
        return ok;
    }

    public bool Get(out short val)
    {
        Span<byte> b = stackalloc byte[2];
        if (!Buf.Read(b)) { val = 0; return false; }
        val = BinaryPrimitives.ReadInt16BigEndian(b);
        return true;
    }

    public bool Get(out ushort val)
    {
        Span<byte> b = stackalloc byte[2];
        if (!Buf.Read(b)) { val = 0; return false; }
        val = BinaryPrimitives.ReadUInt16BigEndian(b);
        return true;
    }

    public bool Get(out int val)
    {
        Span<byte> b = stackalloc byte[4];
        if (!Buf.Read(b)) { val = 0; return false; }
        val = BinaryPrimitives.ReadInt32BigEndian(b);
        return true;
    }

    public bool Get(out uint val)
    {
        Span<byte> b = stackalloc byte[4];
        if (!Buf.Read(b)) { val = 0; return false; }
        val = BinaryPrimitives.ReadUInt32BigEndian(b);
        return true;
    }

    public bool Get(out long val)
    {
        Span<byte> b = stackalloc byte[8];
        if (!Buf.Read(b)) { val = 0; return false; }
        val = BinaryPrimitives.ReadInt64BigEndian(b);
        return true;
    }

    public bool Get(out ulong val)
    {
        Span<byte> b = stackalloc byte[8];
        if (!Buf.Read(b)) { val = 0; return false; }
        val = BinaryPrimitives.ReadUInt64BigEndian(b);
        return true;
    }

    public bool Get(out float val)
    {
        Span<byte> b = stackalloc byte[4];
        if (!Buf.Read(b)) { val = 0; return false; }
        val = BinaryPrimitives.ReadSingleBigEndian(b);
        return true;
    }

    public bool Get(out double val)
    {
        Span<byte> b = stackalloc byte[8];
        if (!Buf.Read(b)) { val = 0; return false; }
        val = BinaryPrimitives.ReadDoubleBigEndian(b);
        return true;
    }

    public bool GetNarrowString(out string val)
    {
        val = string.Empty;
        if (!Get(out uint byteLen)) return false;
        if (byteLen > 16 * 1024 * 1024) return false;
        if (byteLen == 0) return true;
        byte[] bytes = new byte[byteLen];
        if (!Buf.Read(bytes)) return false;
        val = Encoding.UTF8.GetString(bytes);
        return true;
    }

    public bool GetAsciiString(out string val) => GetNarrowString(out val);

    public bool GetWideString(out string val)
    {
        val = string.Empty;
        if (!Get(out uint byteLen)) return false;
        if (byteLen > 16 * 1024 * 1024 || (byteLen & 1) != 0) return false;
        if (byteLen == 0) return true;
        byte[] bytes = new byte[byteLen];
        if (!Buf.Read(bytes)) return false;
        val = Encoding.Unicode.GetString(bytes);
        return true;
    }

    public bool Get(KSerBuffer val)
    {
        val.Clear();
        if (!Get(out uint byteLen)) return false;
        if (byteLen > 64 * 1024 * 1024) return false;
        if (byteLen == 0) return true;
        if (!Get(out bool compressed)) return false;
        byte[] bytes = new byte[byteLen];
        if (!Buf.Read(bytes)) return false;
        val.SetRaw(bytes, compressed);
        return true;
    }

    public bool Get<T>(T obj) where T : IKSerializable => obj.Get(this, Region);

    public bool GetRawBytes(Span<byte> dest) => Buf.Read(dest);

    public bool GetList<T>(List<T> list, Func<KSerializer, (bool ok, T item)> getElem)
    {
        list.Clear();
        if (!Get(out uint count)) return false;
        if (count > 1_000_000) return false;
        list.Capacity = (int)count;
        for (uint i = 0; i < count; i++)
        {
            var (ok, item) = getElem(this);
            if (!ok) return false;
            list.Add(item);
        }
        return true;
    }

    public bool GetList<T>(List<T> list) where T : IKSerializable, new()
        => GetList(list, static ks =>
        {
            var obj = new T();
            return (obj.Get(ks, ks.Region), obj);
        });

    public bool GetHashSet<T>(HashSet<T> set, Func<KSerializer, (bool ok, T item)> getElem)
    {
        set.Clear();
        if (!Get(out uint count)) return false;
        if (count > 1_000_000) return false;
        for (uint i = 0; i < count; i++)
        {
            var (ok, item) = getElem(this);
            if (!ok) return false;
            set.Add(item);
        }
        return true;
    }

    public bool GetMap<TKey, TVal>(
        Dictionary<TKey, TVal> map,
        Func<KSerializer, (bool ok, TKey key)> getKey,
        Func<KSerializer, (bool ok, TVal val)> getVal) where TKey : notnull
    {
        map.Clear();
        if (!Get(out uint count)) return false;
        if (count > 1_000_000) return false;
        for (uint i = 0; i < count; i++)
        {
            var (okK, k) = getKey(this);
            if (!okK) return false;
            var (okV, v) = getVal(this);
            if (!okV) return false;
            map[k] = v;
        }
        return true;
    }
}
