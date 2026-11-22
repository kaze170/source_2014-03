using System.Collections.Concurrent;
using System.Net;
using KncWX2Server.Core.Events;
using KncWX2Server.Core.Logging;
using KncWX2Server.Core.Network;
using KncWX2Server.Core.Region;

namespace KncWX2Server.Core.Framework;

/// <summary>
/// Single-process in-memory event router replacing Common/KncSend.{h,cpp} and inter-server TCP proxy layers,
/// with optional KTcpEventBridge support for multi-process inter-server event forwarding.
/// </summary>
public sealed class InProcessRouter
{
    private readonly ConcurrentDictionary<uint, KPerformer> _performers = new();
    private readonly ConcurrentDictionary<uint, Func<KEvent, bool>> _customRoutes = new();
    private KActorManager? _channelActorManager;
    public KTcpEventBridge? TcpBridge { get; private set; }

    public int EnableRemoteBridge(RegionProfile? region = null, int listenPort = 0, IPAddress? bindAddress = null)
    {
        TcpBridge ??= new KTcpEventBridge(this, region);
        return TcpBridge.StartListener(listenPort, bindAddress);
    }

    public void RegisterRemoteRoute(uint performerId, string host, int port, RegionProfile? region = null)
    {
        TcpBridge ??= new KTcpEventBridge(this, region);
        TcpBridge.RegisterRemotePerformer(performerId, host, port);
    }

    public void RegisterPerformer(uint performerId, KPerformer performer)
        => _performers[performerId] = performer;

    public void RegisterRoute(uint performerId, Func<KEvent, bool> handler)
        => _customRoutes[performerId] = handler;

    public void RegisterRoute(uint performerId, Action<KEvent> handler)
        => _customRoutes[performerId] = ev => { handler(ev); return true; };

    public void RegisterChannelActorManager(KActorManager manager)
        => _channelActorManager = manager;

    public bool HasRoute(uint performerId)
        => _performers.ContainsKey(performerId) || _customRoutes.ContainsKey(performerId);

    public void StopTcpBridge()
    {
        TcpBridge?.Dispose();
        TcpBridge = null;
    }

    public void UnregisterAll()
    {
        _performers.Clear();
        _customRoutes.Clear();
        _channelActorManager = null;
        StopTcpBridge();
    }

    public bool Send(uint fromPerformerId, long fromUid, KEvent ev)
    {
        if (ev.DestPerformer.PerformerId == PerformerId.PI_CHANNEL_USER)
        {
            if (_channelActorManager == null) return false;
            var uids = ev.DestPerformer.Uids;
            if (uids.Count == 0)
            {
                _channelActorManager.QueueingToAll(ev);
                return true;
            }
            bool any = false;
            foreach (long uid in uids)
            {
                if (_channelActorManager.QueueingTo(uid, ev.Clone()))
                    any = true;
            }
            return any;
        }

        if (_customRoutes.TryGetValue(ev.DestPerformer.PerformerId, out var handler))
            return handler(ev);

        if (_performers.TryGetValue(ev.DestPerformer.PerformerId, out var performer))
        {
            performer.QueueingEvent(ev);
            return true;
        }

        KLog.Cwarn($"No in-process route registered for PerformerID=0x{ev.DestPerformer.PerformerId:X8} Event={ev.EventId}.");
        return false;
    }

    public bool Send<T>(uint fromPerformerId, long fromUid, uint toPerformerId, long toUid, EventId eventId, T payload, long[]? trace = null)
    {
        var ev = new KEvent();
        ev.SetInProcess(toPerformerId, trace, eventId, payload);
        ev.DestPerformer.AddUID(toUid);
        return Send(fromPerformerId, fromUid, ev);
    }

    public bool SendNoParam(uint fromPerformerId, long fromUid, uint toPerformerId, long toUid, EventId eventId, long[]? trace = null)
    {
        var ev = new KEvent();
        ev.SetNoParam(toPerformerId, trace, eventId);
        ev.DestPerformer.AddUID(toUid);
        return Send(fromPerformerId, fromUid, ev);
    }
}
