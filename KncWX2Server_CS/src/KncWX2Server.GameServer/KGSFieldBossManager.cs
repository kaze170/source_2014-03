namespace KncWX2Server.GameServer;

/// <summary>
/// Faithful C# port of GameServer/GSFieldBossManager.{h,cpp}:
/// tracks GlobalServer field-boss portal open/cooldown state and falls back to
/// local timer expiration checks if GlobalServer disconnects.
/// </summary>
public sealed class KGSFieldBossManager
{
    [Flags]
    public enum StateFlags : byte
    {
        None = 0,
        PortalOpen = 1 << 0,
        CoolTime = 1 << 1,
        Connected = 1 << 2,
    }

    private readonly object _sync = new();
    private long _reservedTimeForInitRequest;
    private long _checkTerm;
    private long _portalAppearanceTime;
    private long _remainPortalTime;
    private long _remainCoolTime;
    private int _portalAppearanceMap;
    private int _portalDestination;
    private StateFlags _state;

    public long ReservedTimeForInitRequest
    {
        get { lock (_sync) return _reservedTimeForInitRequest; }
        set { lock (_sync) _reservedTimeForInitRequest = value; }
    }

    public long PortalAppearanceTime { get { lock (_sync) return _portalAppearanceTime; } }
    public int PortalAppearanceMap { get { lock (_sync) return _portalAppearanceMap; } }
    public int PortalDestination { get { lock (_sync) return _portalDestination; } }
    public long RemainPortalTime { get { lock (_sync) return _remainPortalTime; } }
    public long RemainCoolTime { get { lock (_sync) return _remainCoolTime; } }

    public bool IsPortalOpen { get { lock (_sync) return (_state & StateFlags.PortalOpen) == StateFlags.PortalOpen; } }
    public bool IsCoolTime { get { lock (_sync) return (_state & StateFlags.CoolTime) == StateFlags.CoolTime; } }
    public bool IsConnected { get { lock (_sync) return (_state & StateFlags.Connected) == StateFlags.Connected; } }

    public bool CheckTerm(long currentUnixSeconds)
    {
        lock (_sync)
        {
            if (currentUnixSeconds - _checkTerm <= 60)
                return false;
            _checkTerm = currentUnixSeconds;
            return true;
        }
    }

    public void SetConnected(bool connected)
    {
        lock (_sync)
        {
            if (connected) _state |= StateFlags.Connected;
            else _state &= ~StateFlags.Connected;
        }
    }

    public void SetInfo(
        long portalAppearanceTime,
        long remainPortalTime,
        long remainCoolTime,
        int portalAppearanceMap,
        int portalDestination,
        bool portalOpen,
        bool coolTime)
    {
        lock (_sync)
        {
            _portalAppearanceTime = portalAppearanceTime;
            _remainPortalTime = remainPortalTime;
            _remainCoolTime = remainCoolTime;
            _portalAppearanceMap = portalAppearanceMap;
            _portalDestination = portalDestination;
            if (portalOpen) _state |= StateFlags.PortalOpen;
            else _state &= ~StateFlags.PortalOpen;
            if (coolTime) _state |= StateFlags.CoolTime;
            else _state &= ~StateFlags.CoolTime;
        }
    }

    public void ClosePortal()
    {
        lock (_sync)
            _state &= ~StateFlags.PortalOpen;
    }

    public void SetCoolTimeState(bool value)
    {
        lock (_sync)
        {
            if (value) _state |= StateFlags.CoolTime;
            else _state &= ~StateFlags.CoolTime;
        }
    }
}
