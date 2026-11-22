using KncWX2Server.Core.Region;
using KncWX2Server.Core.Serialization;

namespace KncWX2Server.Core.Events;

public enum EventFromType : ushort
{
    None = 0,
    Client = 1,
    Server = 2,
}

/// <summary>
/// Wire-exact port of KEvent (Common/Event.{h,cpp}).
/// Wire layout: KPerformerInfo, Int64 Trace[0], Int64 Trace[1], ushort EventID, KSerBuffer Buffer.
/// Also optionally carries an in-process typed payload object to avoid unnecessary binary re-serialization
/// for single-process server-to-server / DB dispatch while preserving full wire serialization for clients.
/// </summary>
public sealed class KEvent : IKSerializable
{
    public KPerformerInfo DestPerformer { get; set; } = new();
    public long Trace0 { get; set; } = -1;
    public long Trace1 { get; set; } = -1;
    public ushort EventIdValue { get; set; }
    public EventId EventId
    {
        get => (EventId)EventIdValue;
        set => EventIdValue = (ushort)value;
    }

    public KSerBuffer Buffer { get; set; } = new();
    public EventFromType FromType { get; set; } = EventFromType.None;

    /// <summary>
    /// Optional in-process CLR object payload for zero-overhead single-process routing.
    /// </summary>
    public object? InProcessPayload { get; set; }

    public long FirstSenderUid => Trace1 != -1 ? Trace1 : Trace0;

    public long LastSenderUid => Trace1 != -1 ? Trace1 : Trace0;

    public long[] Trace => [Trace0, Trace1];

    public bool IsEmptyTrace => Trace0 == -1 && Trace1 == -1;

    public void PushTrace(long uid)
    {
        if (Trace0 == -1)
            Trace0 = uid;
        else if (Trace1 == -1)
            Trace1 = uid;
        else
        {
            Trace0 = Trace1;
            Trace1 = uid;
        }
    }

    public void PopTrace()
    {
        if (Trace1 != -1)
            Trace1 = -1;
        else if (Trace0 != -1)
            Trace0 = -1;
    }

    public KEvent Clone() => new()
    {
        DestPerformer = DestPerformer.Clone(),
        Trace0 = Trace0,
        Trace1 = Trace1,
        EventIdValue = EventIdValue,
        Buffer = Buffer.Clone(),
        FromType = FromType,
        InProcessPayload = InProcessPayload,
    };

    public void SetData<T>(uint performerId, long[]? trace, EventId eventId, T data, RegionProfile? region = null)
        where T : IKSerializable
    {
        DestPerformer = new KPerformerInfo { PerformerId = performerId };
        Trace0 = trace is { Length: > 0 } ? trace[0] : -1;
        Trace1 = trace is { Length: > 1 } ? trace[1] : -1;
        EventId = eventId;
        InProcessPayload = data;

        var ks = new KSerializer(region ?? RegionProfile.Current);
        ks.BeginWriting(Buffer);
        data.Put(ks, ks.Region);
        ks.EndWriting();
    }

    public void SetInProcess<T>(uint performerId, long[]? trace, EventId eventId, T data)
    {
        DestPerformer = new KPerformerInfo { PerformerId = performerId };
        Trace0 = trace is { Length: > 0 } ? trace[0] : -1;
        Trace1 = trace is { Length: > 1 } ? trace[1] : -1;
        EventId = eventId;
        InProcessPayload = data;
    }

    public void SetNoParam(uint performerId, long[]? trace, EventId eventId, RegionProfile? region = null)
    {
        DestPerformer = new KPerformerInfo { PerformerId = performerId };
        Trace0 = trace is { Length: > 0 } ? trace[0] : -1;
        Trace1 = trace is { Length: > 1 } ? trace[1] : -1;
        EventId = eventId;

        var ks = new KSerializer(region ?? RegionProfile.Current);
        ks.BeginWriting(Buffer);
        ks.Put((sbyte)0); // matches C++ char()
        ks.EndWriting();
    }

    public bool TryReadPayload<T>(RegionProfile? region, out T packet) where T : IKSerializable, new()
        => TryReadPayload(out packet, region);

    public bool TryReadPayload<T>(out T packet, RegionProfile? region = null) where T : IKSerializable, new()
    {
        if (InProcessPayload is T typed)
        {
            packet = typed;
            return true;
        }

        packet = new T();
        var reg = region ?? RegionProfile.Current;
        var readBuf = Buffer;
        if (readBuf.Length == 0 && InProcessPayload is IKSerializable src)
        {
            readBuf = new KSerBuffer();
            var wks = new KSerializer(reg);
            wks.BeginWriting(readBuf);
            src.Put(wks, reg);
            wks.EndWriting();
        }
        var ks = new KSerializer(reg);
        ks.BeginReading(readBuf);
        bool ok = packet.Get(ks, ks.Region);
        ks.EndReading();
        return ok;
    }

    public T GetPayload<T>(RegionProfile? region = null) where T : new()
    {
        if (InProcessPayload is T typed)
            return typed;

        var instance = new T();
        if (instance is IKSerializable serializable)
        {
            var reg = region ?? RegionProfile.Current;
            var readBuf = Buffer;
            if (readBuf.Length == 0 && InProcessPayload is IKSerializable src)
            {
                readBuf = new KSerBuffer();
                var wks = new KSerializer(reg);
                wks.BeginWriting(readBuf);
                src.Put(wks, reg);
                wks.EndWriting();
            }
            var ks = new KSerializer(reg);
            ks.BeginReading(readBuf);
            serializable.Get(ks, ks.Region);
            ks.EndReading();
        }
        return instance;
    }

    public bool Put(KSerializer ks, RegionProfile region)
    {
        return DestPerformer.Put(ks, region)
            && ks.Put(Trace0)
            && ks.Put(Trace1)
            && ks.Put(EventIdValue)
            && ks.Put(Buffer);
    }

    public bool Get(KSerializer ks, RegionProfile region)
    {
        return DestPerformer.Get(ks, region)
            && ks.Get(out long t0) && SetTrace0(t0)
            && ks.Get(out long t1) && SetTrace1(t1)
            && ks.Get(out ushort eid) && SetEventId(eid)
            && ks.Get(Buffer);
    }

    private bool SetTrace0(long v) { Trace0 = v; return true; }
    private bool SetTrace1(long v) { Trace1 = v; return true; }
    private bool SetEventId(ushort v) { EventIdValue = v; return true; }
}
