using KncWX2Server.Core.Events;
using KncWX2Server.Core.Framework;

namespace KncWX2Server.GameServer;

/// <summary>
/// Faithful C# port of GameServer/UserHackingManager.{h,cpp}:
/// schedules 10..20s randomized anti-hack tool sweeps, monitors distinct letter/trade senders,
/// tracks deserialization failures for disconnection, and checks 10s server-hack challenge timeouts.
/// </summary>
public sealed class KUserHackingManager
{
    public const int HeMinTimeSeconds = 10;
    public const int HeMaxTimeSeconds = 20;
    public const sbyte HctNone = 0;

    private readonly Random _rng = new();
    private readonly HashSet<long> _recvLetterUnits = [];
    private readonly HashSet<long> _recvTradeUnits = [];
    private readonly Dictionary<ushort, int> _deserializeFailCounts = new();

    private bool _checkedDeserializeFail;
    private int _totalDeserializeFailCount;
    private sbyte _lastSendType = HctNone;
    private DateTime _hackingUserTimerUtc = DateTime.UtcNow;

    public double HackingToolCheckTimeSeconds { get; private set; }
    public bool EdMonitoringMode { get; set; }

    public KUserHackingManager()
    {
        SetRandomCheckTime();
    }

    public void SetRandomCheckTime()
    {
        HackingToolCheckTimeSeconds = _rng.Next(HeMinTimeSeconds, HeMaxTimeSeconds);
    }

    public void ClearLetterMonitoringList() => _recvLetterUnits.Clear();
    public void ClearTradeMonitoringList() => _recvTradeUnits.Clear();

    public bool AddLetterMonitoringList(long fromUnitUid, KGameSysValTable sysVal)
    {
        _recvLetterUnits.Add(fromUnitUid);
        return _recvLetterUnits.Count >= sysVal.LimitsCountOfLetter;
    }

    public bool AddTradeMonitoringList(long fromUnitUid, KGameSysValTable sysVal)
    {
        _recvTradeUnits.Add(fromUnitUid);
        return _recvTradeUnits.Count >= sysVal.LimitsCountOfTrade;
    }

    public void IncreaseDeserializeFailCount(ushort eventId)
    {
        // C++ UserHackingManager.cpp L122: specifically monitors EGS_DUNGEON_PLAY_INFO_TO_SERVER_NOT
        if (eventId != (ushort)EventId.EGS_DUNGEON_PLAY_INFO_TO_SERVER_NOT)
            return;

        _totalDeserializeFailCount++;
        _deserializeFailCounts[eventId] = _deserializeFailCounts.GetValueOrDefault(eventId) + 1;
    }

    public bool HackingCheckDeserializeFailCount(KGameSysValTable sysVal)
    {
        if (_checkedDeserializeFail) return false;
        bool exceeded = _totalDeserializeFailCount > sysVal.DeserializeFailCheckCount;
        if (exceeded)
            _checkedDeserializeFail = true;
        _totalDeserializeFailCount = 0;
        _deserializeFailCounts.Clear();
        return exceeded;
    }

    public void SetHackingUserSendType(sbyte type, DateTime? nowUtc = null)
    {
        _lastSendType = type;
        _hackingUserTimerUtc = nowUtc ?? DateTime.UtcNow;
    }

    public bool CheckHackingUserReceivedPacket(out sbyte lastSendType, DateTime? nowUtc = null)
    {
        var now = nowUtc ?? DateTime.UtcNow;
        if (_lastSendType != HctNone)
        {
            if ((now - _hackingUserTimerUtc).TotalSeconds < 10.0)
            {
                lastSendType = HctNone;
                return true;
            }
            lastSendType = _lastSendType;
            _lastSendType = HctNone;
            _hackingUserTimerUtc = now;
            return false;
        }
        lastSendType = HctNone;
        return true;
    }

    public bool SetHackingUserReceiveType(sbyte type, out sbyte lastSendType, DateTime? nowUtc = null)
    {
        var now = nowUtc ?? DateTime.UtcNow;
        if (_lastSendType == type)
        {
            lastSendType = HctNone;
            _lastSendType = HctNone;
            _hackingUserTimerUtc = now;
            return true;
        }
        lastSendType = _lastSendType;
        _lastSendType = HctNone;
        _hackingUserTimerUtc = now;
        return false;
    }
}
