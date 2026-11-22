using System.Diagnostics;
using KncWX2Server.Core.Packets;

namespace KncWX2Server.GameServer;

/// <summary>
/// Faithful C# port of Common/EventDelaySystemManager.{h,cpp}:
/// throttles random cube openings when SERV_OPEN_RANDOM_CUBE_DELAY_SYSTEM is active in GameSysVal.
/// </summary>
public sealed class KEventDelaySystemManager
{
    public sealed record PendingOpenRandomCubeEvent(long UnitUid, KEGS_OPEN_RANDOM_ITEM_REQ Request);

    private readonly LinkedList<PendingOpenRandomCubeEvent> _queue = new();
    private readonly HashSet<long> _registeredUnits = new();
    private readonly Stopwatch _timer = Stopwatch.StartNew();
    private readonly object _sync = new();

    public int PendingCount
    {
        get { lock (_sync) return _queue.Count; }
    }

    public bool IsRegRandomCubeEvent(long unitUid)
    {
        lock (_sync)
            return _registeredUnits.Contains(unitUid);
    }

    public void RegOpenRandomCubeEvent(long unitUid, KEGS_OPEN_RANDOM_ITEM_REQ request)
    {
        lock (_sync)
        {
            if (_registeredUnits.Contains(unitUid))
                UnRegOpenRandomCubeEventLocked(unitUid);

            _registeredUnits.Add(unitUid);
            _queue.AddLast(new PendingOpenRandomCubeEvent(unitUid, new KEGS_OPEN_RANDOM_ITEM_REQ
            {
                ItemUid = request.ItemUid,
                DelayedOk = request.DelayedOk,
                NotifyGetItem = request.NotifyGetItem,
            }));
        }
    }

    public void UnRegOpenRandomCubeEvent(long unitUid)
    {
        lock (_sync)
            UnRegOpenRandomCubeEventLocked(unitUid);
    }

    private void UnRegOpenRandomCubeEventLocked(long unitUid)
    {
        if (!_registeredUnits.Remove(unitUid))
            return;

        for (var node = _queue.First; node != null; node = node.Next)
        {
            if (node.Value.UnitUid == unitUid)
            {
                _queue.Remove(node);
                break;
            }
        }
    }

    public int Tick(bool enabled, double delayTimeSeconds, int delayCount, Action<long, KEGS_OPEN_RANDOM_ITEM_REQ> dispatch)
    {
        if (!enabled)
            return 0;

        List<PendingOpenRandomCubeEvent>? batch = null;
        lock (_sync)
        {
            if (_timer.Elapsed.TotalSeconds < delayTimeSeconds)
                return 0;

            _timer.Restart();
            int count = Math.Max(0, delayCount);
            for (int i = 0; i < count && _queue.First != null; i++)
            {
                var item = _queue.First.Value;
                _queue.RemoveFirst();
                _registeredUnits.Remove(item.UnitUid);
                item.Request.DelayedOk = true;
                batch ??= [];
                batch.Add(item);
            }
        }

        if (batch == null)
            return 0;

        foreach (var item in batch)
            dispatch(item.UnitUid, item.Request);

        return batch.Count;
    }

    public void Clear()
    {
        lock (_sync)
        {
            _queue.Clear();
            _registeredUnits.Clear();
        }
    }
}
