using KncWX2Server.Core.Region;

namespace KncWX2Server.GameServer;

/// <summary>
/// Faithful C# port of GameServer/InventoryLocker.h (SERV_INVENTORY_LOCK):
/// prevents concurrent inventory mutations during trade, crafting, synthesis, and enchantment,
/// supporting infinite locks and auto-expiring timeout locks.
/// </summary>
public sealed class KInventoryLocker
{
    public enum LockType
    {
        Infinite = 0,
        TimeOut = 1,
    }

    private bool _locked;
    private DateTime _lockedUtc;
    private double _lockSeconds;
    private LockType _lockType = LockType.Infinite;
    private readonly object _sync = new();

    public void Clear()
    {
        lock (_sync)
        {
            _locked = false;
            _lockedUtc = DateTime.MinValue;
            _lockSeconds = 0.0;
            _lockType = LockType.Infinite;
        }
    }

    public bool IsLocked(DateTime? nowUtcOverride = null)
    {
        lock (_sync)
        {
            if (!_locked) return false;
            if (_lockType == LockType.TimeOut)
            {
                double elapsed = ((nowUtcOverride ?? DateTime.UtcNow) - _lockedUtc).TotalSeconds;
                if (elapsed > _lockSeconds)
                {
                    _locked = false;
                    return false;
                }
            }
            return true;
        }
    }

    public bool Lock(LockType lockType, double lockSeconds = 99_999.0, DateTime? nowUtcOverride = null)
    {
        lock (_sync)
        {
            if (IsLocked(nowUtcOverride))
                return false;

            _locked = true;
            _lockType = lockType;
            _lockSeconds = Math.Max(0.0, lockSeconds);
            _lockedUtc = nowUtcOverride ?? DateTime.UtcNow;
            return true;
        }
    }

    public bool TryLock(out int errCode, double lockSeconds = 30.0, DateTime? nowUtcOverride = null)
    {
        if (Lock(LockType.TimeOut, lockSeconds, nowUtcOverride))
        {
            errCode = NetError.NET_OK;
            return true;
        }
        errCode = NetError.ERR_INVENTORY_LOCK_00;
        return false;
    }

    public void Unlock() => Clear();
}
