using System.Collections.Concurrent;
using KncWX2Server.Core.Events;
using KncWX2Server.Core.Logging;

namespace KncWX2Server.Core.Framework;

/// <summary>
/// Port of Common/ActorManager.{h,cpp} managing all active KActor instances, UID lookup, and per-tick event pumping.
/// </summary>
public sealed class KActorManager
{
    private readonly ConcurrentDictionary<long, KActor> _actors = new();
    private readonly List<Func<KActor, bool>> _tickFilters = [];

    public int Count => _actors.Count;

    public bool Add(KActor actor)
    {
        if (!_actors.TryAdd(actor.Uid, actor))
            return false;

        actor.OnConnected();
        return true;
    }

    public bool Remove(long uid)
        => _actors.TryRemove(uid, out _);

    public KActor? GetByUid(long uid)
        => _actors.TryGetValue(uid, out var actor) ? actor : null;

    public long? GetFirstActorKey()
    {
        foreach (var key in _actors.Keys)
            return key;
        return null;
    }

    public IReadOnlyCollection<KActor> GetSnapshot() => _actors.Values.ToArray();
    public IReadOnlyCollection<KActor> GetAllActors() => GetSnapshot();

    public bool QueueingTo(long uid, KEvent ev)
    {
        if (_actors.TryGetValue(uid, out var actor))
        {
            actor.QueueingEvent(ev);
            return true;
        }
        return false;
    }

    public void QueueingToAll(KEvent ev)
    {
        foreach (var actor in _actors.Values)
            actor.QueueingEvent(ev.Clone());
    }

    public void ForEach(Action<KActor> action)
    {
        foreach (var actor in _actors.Values)
            action(actor);
    }

    public void Tick()
    {
        foreach (var kv in _actors)
        {
            var actor = kv.Value;
            try
            {
                actor.Tick();
                if (actor.IsDestroyed)
                {
                    _actors.TryRemove(kv.Key, out _);
                }
            }
            catch (Exception ex)
            {
                KLog.Cerr($"ActorManager Tick exception for UID={kv.Key}: {ex}");
            }
        }
    }

    public void DestroyAll()
    {
        foreach (var kv in _actors)
        {
            kv.Value.ReserveDestroy();
        }
    }
}
