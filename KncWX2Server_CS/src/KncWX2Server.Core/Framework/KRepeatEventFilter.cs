using KncWX2Server.Core.Events;

namespace KncWX2Server.Core.Framework;

/// <summary>
/// Port of Common/RepeatEventFilter/RepeatEventFilter.{h,cpp}.
/// Prevents duplicate concurrent REQ packets from being processed before their ACK completes.
/// </summary>
public sealed class KRepeatEventFilter
{
    private sealed class FlagBox { public bool InFlight; }

    private readonly Dictionary<ushort, FlagBox> _reqMap = new();
    private readonly Dictionary<ushort, FlagBox> _ackMap = new();

    public int RegCount { get; private set; }
    public int ReqEventCount => _reqMap.Count;
    public int AckEventCount => _ackMap.Count;

    public bool RegRepeatEvent(EventId reqEventId, EventId ackEventId)
        => RegRepeatEvent((ushort)reqEventId, (ushort)ackEventId);

    public bool RegRepeatEvent(ushort reqEventId, ushort ackEventId)
    {
        RegCount++;
        var box = new FlagBox();
        _reqMap[reqEventId] = box;
        _ackMap[ackEventId] = box;
        return true;
    }

    public bool CheckReqEvent(ushort eventId)
    {
        if (_reqMap.TryGetValue(eventId, out var box))
        {
            if (!box.InFlight)
            {
                box.InFlight = true;
                return true;
            }
            return false;
        }
        return true;
    }

    public void CheckAckEvent(ushort eventId)
    {
        if (_ackMap.TryGetValue(eventId, out var box))
        {
            box.InFlight = false;
        }
    }
}
