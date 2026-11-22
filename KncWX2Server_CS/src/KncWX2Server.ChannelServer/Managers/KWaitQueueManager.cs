using KncWX2Server.Core.Events;
using KncWX2Server.Core.Framework;
using KncWX2Server.Core.Logging;
using KncWX2Server.Core.Packets;
using KncWX2Server.Core.Scripting;
using MoonSharp.Interpreter;

namespace KncWX2Server.ChannelServer.Managers;

public enum WaitQueueMode
{
    WQ_NONE = 0,
    WQ_CCU_LIMIT = 1,
    WQ_ENTRY_BAN = 2,
}

/// <summary>
/// Port of ChannelServer/WaitQueueManager.{h,cpp}.
/// </summary>
public sealed class KWaitQueueManager
{
    private readonly object _lock = new();
    private readonly List<long> _waitQueue = [];
    private readonly HashSet<long> _waitSet = [];
    private readonly HashSet<long> _passedUsers = [];
    private readonly Dictionary<int, int> _serverGroupCcuLimits = new();

    private DateTime _lastDbQuery = DateTime.MinValue;
    private DateTime _lastPassTick = DateTime.UtcNow;

    public WaitQueueMode Mode { get; private set; } = WaitQueueMode.WQ_NONE;
    public double SendTimeGapSeconds { get; private set; } = 5.0;
    public int TotalCcu { get; private set; }

    public void SetMode(int mode)
    {
        lock (_lock)
            Mode = (WaitQueueMode)mode;
        KLog.Cout($"WaitQueueManager:Mode = {Mode}");
    }

    public void SetSendTimeGap(double seconds)
    {
        lock (_lock)
            SendTimeGapSeconds = Math.Max(1.0, seconds);
    }

    public void SetTotalCCULimit(int serverGroupId, int limit)
    {
        lock (_lock)
            _serverGroupCcuLimits[serverGroupId] = limit;
    }

    public int GetTotalCCULimit(int serverGroupId)
    {
        lock (_lock)
            return _serverGroupCcuLimits.TryGetValue(serverGroupId, out int limit) ? limit : 100_000;
    }

    public void UpdateTotalCCU(int totalCcu)
    {
        lock (_lock)
            TotalCcu = totalCcu;
    }

    public bool IsCanEnterChannel(int serverGroupId)
    {
        lock (_lock)
        {
            if (Mode == WaitQueueMode.WQ_NONE)
                return true;
            if (Mode == WaitQueueMode.WQ_ENTRY_BAN)
                return false;

            int limit = _serverGroupCcuLimits.TryGetValue(serverGroupId, out int v) ? v : 100_000;
            return (TotalCcu + _passedUsers.Count) < limit && _waitQueue.Count == 0;
        }
    }

    public bool RegPassedUser(long userUid)
    {
        lock (_lock)
            return _passedUsers.Add(userUid);
    }

    public bool UnregPassedUser(long userUid)
    {
        lock (_lock)
            return _passedUsers.Remove(userUid);
    }

    public bool RegWaitUser(long userUid)
    {
        lock (_lock)
        {
            if (!_waitSet.Add(userUid)) return false;
            _waitQueue.Add(userUid);
            return true;
        }
    }

    public bool UnregWaitUser(long userUid)
    {
        lock (_lock)
        {
            if (!_waitSet.Remove(userUid)) return false;
            _waitQueue.Remove(userUid);
            return true;
        }
    }

    public bool GetWaitUserCount(long userUid, out int count)
    {
        lock (_lock)
        {
            int idx = _waitQueue.IndexOf(userUid);
            if (idx < 0)
            {
                count = 0;
                return false;
            }
            count = idx + 1;
            return true;
        }
    }

    public void Tick(int serverGroupId, InProcessRouter router, KActorManager actorManager)
    {
        var now = DateTime.UtcNow;
        if ((now - _lastDbQuery).TotalSeconds >= SendTimeGapSeconds)
        {
            _lastDbQuery = now;
            router.SendNoParam(PerformerId.PI_CHANNEL_SERVER, 0, PerformerId.PI_CHANNEL_LOG_DB, 0, EventId.DBE_GET_TOTAL_CCU_FOR_WAIT_QUEUE_REQ);
        }

        if ((now - _lastPassTick).TotalSeconds < 1.0)
            return;
        _lastPassTick = now;

        List<long> toNotify = [];
        List<(long Uid, int Pos)> waitUpdates = [];

        lock (_lock)
        {
            if (Mode == WaitQueueMode.WQ_NONE)
            {
                toNotify.AddRange(_waitQueue);
                _waitQueue.Clear();
                _waitSet.Clear();
                foreach (long uid in toNotify)
                    _passedUsers.Add(uid);
            }
            else if (Mode == WaitQueueMode.WQ_CCU_LIMIT)
            {
                int limit = _serverGroupCcuLimits.TryGetValue(serverGroupId, out int v) ? v : 100_000;
                while (_waitQueue.Count > 0 && (TotalCcu + _passedUsers.Count) < limit)
                {
                    long uid = _waitQueue[0];
                    _waitQueue.RemoveAt(0);
                    _waitSet.Remove(uid);
                    _passedUsers.Add(uid);
                    toNotify.Add(uid);
                }
            }

            for (int i = 0; i < _waitQueue.Count; i++)
                waitUpdates.Add((_waitQueue[i], i + 1));
        }

        foreach (long uid in toNotify)
        {
            if (actorManager.GetByUid(uid) is KChannelUser user)
                user.NotifyWaitQueueEnterChannel();
        }

        foreach (var (uid, pos) in waitUpdates)
        {
            if (actorManager.GetByUid(uid) is KChannelUser user)
            {
                user.SendPacket(EventId.ECH_WAIT_QUEUE_MESSAGE_NOT, new KECH_WAIT_QUEUE_MESSAGE_NOT
                {
                    WaitUserCount = pos,
                });
            }
        }
    }

    public void BindToLua(KScriptManager script, string globalName = "WaitQueueManager")
    {
        Table? proxy = null;
        proxy = script.CreateLuaProxy(globalName, t =>
        {
            t["SetMode"] = DynValue.NewCallback((_, a) =>
            {
                SetMode(KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, proxy!)));
                return DynValue.Nil;
            });
            t["SetSendTimeGap"] = DynValue.NewCallback((_, a) =>
            {
                SetSendTimeGap(KScriptManager.ArgDouble(a, KScriptManager.ArgStart(a, proxy!), 5.0));
                return DynValue.Nil;
            });
            t["SetTotalCCULimit"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                SetTotalCCULimit(KScriptManager.ArgInt(a, o), KScriptManager.ArgInt(a, o + 1, 100_000));
                return DynValue.Nil;
            });
        });
    }
}

/// <summary>
/// Port of ChannelServer/DDOSGuardSimulator.{h,cpp}.
/// </summary>
public sealed class KDDOSGuardSimulator
{
    public bool IsEnabled { get; set; }
    public int CheckIntervalSeconds { get; set; } = 30;
}
