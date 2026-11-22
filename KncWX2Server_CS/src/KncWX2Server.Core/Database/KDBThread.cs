using System.Collections.Concurrent;
using KncWX2Server.Core.Events;
using KncWX2Server.Core.Framework;
using KncWX2Server.Core.Logging;

namespace KncWX2Server.Core.Database;

/// <summary>
/// Port of Common/DBLayer/DBThread.{h,cpp}: asynchronous worker queue draining KEvent DB requests.
/// </summary>
public abstract class KDBThread : KPerformer
{
    private readonly BlockingCollection<KEvent> _queue = new();
    private CancellationTokenSource? _cts;
    private Task? _workerTask;

    protected SqliteDatabaseManager Db { get; }
    protected InProcessRouter Router { get; }

    protected KDBThread(SqliteDatabaseManager db, InProcessRouter router)
    {
        Db = db;
        Router = router;
    }

    public new void QueueingEvent(KEvent ev)
    {
        if (!_queue.IsAddingCompleted)
            _queue.Add(ev);
    }

    public void Start(CancellationToken externalToken = default)
    {
        _cts = CancellationTokenSource.CreateLinkedTokenSource(externalToken);
        _workerTask = Task.Run(() => RunLoop(_cts.Token));
    }

    public void Stop()
    {
        _queue.CompleteAdding();
        try { _cts?.Cancel(); } catch { }
        try { _workerTask?.Wait(3000); } catch { }
    }

    private void RunLoop(CancellationToken ct)
    {
        try
        {
            foreach (var ev in _queue.GetConsumingEnumerable(ct))
            {
                try
                {
                    ProcessEvent(ev);
                }
                catch (Exception ex)
                {
                    KLog.Cerr($"[{GetType().Name}] DB Event {ev.EventId} error: {ex}");
                }
            }
        }
        catch (OperationCanceledException)
        {
        }
    }

    protected void SendToServer<T>(EventId eventId, T payload, long[]? trace = null)
        => Router.Send(PerformerTypeId, 0, PerformerId.PI_CHANNEL_SERVER, 0, eventId, payload, trace);

    protected void SendToUser<T>(long userUid, EventId eventId, T payload, long[]? trace = null)
        => Router.Send(PerformerTypeId, 0, PerformerId.PI_CHANNEL_USER, userUid, eventId, payload, trace);
}
