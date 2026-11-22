using System.Collections.Concurrent;
using KncWX2Server.Core.Scripting;
using MoonSharp.Interpreter;

namespace KncWX2Server.GlobalServer;

/// <summary>
/// Faithful C# port of GlobalServer/FieldBossManager.{h,cpp} (SERV_BATTLE_FIELD_BOSS):
/// tracks per-field dangerousness saturation (0..255), portal open/cooldown bitmask state,
/// and 60-second check terms.
/// </summary>
public sealed class KFieldBossManager
{
    [Flags]
    public enum FieldBossState : byte
    {
        None = 0,
        FbsPortalOpen = 1 << 0,
        FbsCoolTime = 1 << 1,
    }

    private readonly ConcurrentDictionary<int, byte> _totalDangerousValue = new();
    private long _lastCheckTermUnix;
    private FieldBossState _state = FieldBossState.None;

    public long PortalAppearanceTimeUnix { get; private set; }
    public int PortalAppearanceMap { get; private set; }
    public int PortalDestination { get; private set; }

    public bool IsPortalOpen => (_state & FieldBossState.FbsPortalOpen) == FieldBossState.FbsPortalOpen;
    public bool IsCoolTime => (_state & FieldBossState.FbsCoolTime) == FieldBossState.FbsCoolTime;

    public int ConfiguredTotalDangerousValue { get; private set; } = 28;
    public int FieldBossCoolTimeMinutes { get; private set; } = 60;
    public int PortalHoldingTimeMinutes { get; private set; } = 15;
    public int FieldHoldingTimeMinutes { get; private set; } = 12;
    public int EndGameFieldHoldingTimeSeconds { get; private set; } = 255;

    public void UpdateTotalDangerousValue(int portalAppearanceMap, byte deltaValue)
    {
        _totalDangerousValue.AddOrUpdate(
            portalAppearanceMap,
            deltaValue,
            (_, current) => (byte)Math.Min(byte.MaxValue, current + deltaValue));
    }

    public void SetTotalDangerousValue(int portalAppearanceMap, byte value, bool isSet)
    {
        if (isSet)
            _totalDangerousValue[portalAppearanceMap] = value;
        else
            UpdateTotalDangerousValue(portalAppearanceMap, value);
    }

    public void ClearTotalDangerousValue() => _totalDangerousValue.Clear();

    public byte GetTotalDangerousValue(int portalAppearanceMap)
        => _totalDangerousValue.TryGetValue(portalAppearanceMap, out byte v) ? v : (byte)0;

    public byte GetTotalDangerousValue()
    {
        int sum = 0;
        foreach (byte v in _totalDangerousValue.Values)
            sum = Math.Min(byte.MaxValue, sum + v);
        return (byte)sum;
    }

    public void OpenPortal(int portalAppearanceMap, int portalDestination, long portalAppearanceTimeUnix)
    {
        PortalAppearanceMap = portalAppearanceMap;
        PortalDestination = portalDestination;
        PortalAppearanceTimeUnix = portalAppearanceTimeUnix;
        _state |= FieldBossState.FbsPortalOpen;
    }

    public void ClosePortal() => _state &= ~FieldBossState.FbsPortalOpen;

    public void SetCoolTimeState(bool value)
    {
        if (value) _state |= FieldBossState.FbsCoolTime;
        else _state &= ~FieldBossState.FbsCoolTime;
    }

    public bool CheckTerm(long currentUnixSeconds)
    {
        if (currentUnixSeconds - _lastCheckTermUnix >= 60)
        {
            _lastCheckTermUnix = currentUnixSeconds;
            return true;
        }
        return false;
    }

    public int BossFieldOpenProcess(int battleFieldId, long currentUnixSeconds, int destinationFieldId = 20001)
    {
        if (IsCoolTime)
            return KncWX2Server.Core.Region.NetError.ERR_FIELD_BOSS_02;

        OpenPortal(battleFieldId, destinationFieldId, currentUnixSeconds);
        SetCoolTimeState(true);
        return KncWX2Server.Core.Region.NetError.NET_OK;
    }

    public int TotalDangerousValueUpdateProcess(int battleFieldId, byte value, long currentUnixSeconds, int destinationFieldId = 20001)
    {
        SetTotalDangerousValue(battleFieldId, value, isSet: true);
        if (!IsPortalOpen && !IsCoolTime && value >= ConfiguredTotalDangerousValue)
            return BossFieldOpenProcess(battleFieldId, currentUnixSeconds, destinationFieldId);
        return KncWX2Server.Core.Region.NetError.NET_OK;
    }

    public (long remainPortalSec, long remainCoolSec) GetRemainingTimes(long currentUnixSeconds)
    {
        long remainPortal = IsPortalOpen
            ? Math.Max(0, (PortalAppearanceTimeUnix + PortalHoldingTimeMinutes * 60L) - currentUnixSeconds)
            : 0L;
        long remainCool = IsCoolTime
            ? Math.Max(0, (PortalAppearanceTimeUnix + FieldBossCoolTimeMinutes * 60L) - currentUnixSeconds)
            : 0L;
        return (remainPortal, remainCool);
    }

    /// <summary>
    /// Port of GlobalServer.cpp CheckFieldBossSystem(tCurrentTime):
    /// closes expired portal and clears cooldown/dangerousness once FieldBossCoolTimeMinutes elapses.
    /// Returns true if state changed and should be broadcast to GameServers.
    /// </summary>
    public bool Tick(long currentUnixSeconds)
    {
        if (!CheckTerm(currentUnixSeconds))
            return false;

        bool wasPortal = IsPortalOpen;
        bool wasCool = IsCoolTime;

        if (IsPortalOpen && currentUnixSeconds >= PortalAppearanceTimeUnix + PortalHoldingTimeMinutes * 60L)
            ClosePortal();

        if (IsCoolTime && currentUnixSeconds >= PortalAppearanceTimeUnix + FieldBossCoolTimeMinutes * 60L)
        {
            SetCoolTimeState(false);
            ClearTotalDangerousValue();
        }

        return wasPortal != IsPortalOpen || wasCool != IsCoolTime;
    }

    public void BindToLua(KScriptManager script, string globalName = "FieldBossData")
    {
        Table? proxy = null;
        proxy = script.CreateLuaProxy(globalName, t =>
        {
            t["SetData"] = DynValue.NewCallback((_, a) =>
            {
                var tbl = KScriptManager.ArgTable(a, KScriptManager.ArgStart(a, proxy!));
                if (tbl != null)
                {
                    ConfiguredTotalDangerousValue = (int)(tbl.Get("TotalDangerousValue").CastToNumber() ?? ConfiguredTotalDangerousValue);
                    FieldBossCoolTimeMinutes = (int)(tbl.Get("FieldBossCoolTime_M").CastToNumber() ?? FieldBossCoolTimeMinutes);
                    PortalHoldingTimeMinutes = (int)(tbl.Get("PortalHoldingTime_M").CastToNumber() ?? PortalHoldingTimeMinutes);
                    FieldHoldingTimeMinutes = (int)(tbl.Get("FieldHoldingTime_M").CastToNumber() ?? FieldHoldingTimeMinutes);
                    EndGameFieldHoldingTimeSeconds = (int)(tbl.Get("EndGameFieldHoldingTime_S").CastToNumber() ?? EndGameFieldHoldingTimeSeconds);
                }
                return DynValue.Nil;
            });
        });
    }
}
