namespace KncWX2Server.GameServer;

/// <summary>
/// Faithful C# port of GameServer/UserLogManager.{h,cpp}:
/// maintains bounded FIFO queues (QUEUE_SIZE = 5) of recent outbound packets and processed events
/// to snapshot diagnostic trails on heartbeat timeouts or abnormal disconnects.
/// </summary>
public sealed class KUserLogManager
{
    public const int QueueSize = 5;

    public enum UserLogType
    {
        HeartBeat = 0,
        AbnormalDisconnect = 1,
    }

    public readonly record struct EventLogEntry(ushort EventId, long UnixTime);

    private readonly Queue<EventLogEntry> _sendQueue = new();
    private readonly Queue<EventLogEntry> _processQueue = new();
    private readonly object _sync = new();

    public void SaveSendPacket(ushort eventId, long? unixTimeOverride = null)
    {
        long ts = unixTimeOverride ?? DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        lock (_sync)
        {
            if (_sendQueue.Count >= QueueSize)
                _sendQueue.Dequeue();
            _sendQueue.Enqueue(new EventLogEntry(eventId, ts));
        }
    }

    public void SaveProcessEvent(ushort eventId, long? unixTimeOverride = null)
    {
        long ts = unixTimeOverride ?? DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        lock (_sync)
        {
            if (_processQueue.Count >= QueueSize)
                _processQueue.Dequeue();
            _processQueue.Enqueue(new EventLogEntry(eventId, ts));
        }
    }

    public void Clear()
    {
        lock (_sync)
        {
            _sendQueue.Clear();
            _processQueue.Clear();
        }
    }

    public string FormatDiagnosticSnapshot(UserLogType logType, long userUid, long unitUid)
    {
        lock (_sync)
        {
            var sent = string.Join(",", _sendQueue.Select(e => $"0x{e.EventId:X4}@{e.UnixTime}"));
            var proc = string.Join(",", _processQueue.Select(e => $"0x{e.EventId:X4}@{e.UnixTime}"));
            return $"[{logType}] UserUID={userUid}, UnitUID={unitUid}, Send=[{sent}], Proc=[{proc}]";
        }
    }
}
