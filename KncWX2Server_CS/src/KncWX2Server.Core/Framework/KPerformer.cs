using System.Collections.Concurrent;
using KncWX2Server.Core.Events;
using KncWX2Server.Core.Logging;

namespace KncWX2Server.Core.Framework;

/// <summary>
/// Port of Common/Performer.{h,cpp} — thread-safe event queue + Tick processing loop.
/// </summary>
public abstract class KPerformer
{
    private readonly ConcurrentQueue<KEvent> _eventQueue = new();

    public long Uid { get; set; }
    public string Name { get; set; } = string.Empty;
    public abstract uint PerformerTypeId { get; }
    public int MaxQueueSize { get; private set; }

    public void QueueingEvent(KEvent ev)
    {
        _eventQueue.Enqueue(ev);
        int cnt = _eventQueue.Count;
        if (cnt > MaxQueueSize)
            MaxQueueSize = cnt;
    }

    public void QueueEvent<T>(EventId eventId, T payload)
    {
        var ev = new KEvent();
        ev.SetInProcess(PerformerTypeId, Uid > 0 ? [Uid] : null, eventId, payload);
        QueueingEvent(ev);
    }

    public virtual void Tick()
    {
        while (_eventQueue.TryDequeue(out var ev))
        {
            try
            {
                ProcessEvent(ev);
            }
            catch (Exception ex)
            {
                KLog.Cerr($"Unhandled exception processing {ev.EventId} ({ev.EventIdValue}) on {GetType().Name}: {ex}");
            }
        }
    }

    protected abstract void ProcessEvent(KEvent ev);
}
