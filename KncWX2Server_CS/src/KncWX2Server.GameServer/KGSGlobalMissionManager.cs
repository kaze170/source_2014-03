using System.Diagnostics;
using KncWX2Server.Core.Packets;

namespace KncWX2Server.GameServer;

/// <summary>
/// Faithful C# port of GameServer/GSGlobalMissionManager.{h,cpp}:
/// tracks live global mission progress, tick versions, and update broadcasts.
/// </summary>
public sealed class KGSGlobalMissionManager
{
    private readonly Dictionary<string, KGlobalMissionInfo> _missions = new(StringComparer.Ordinal);
    private readonly Stopwatch _timer = new();

    public int TickCount { get; private set; }

    public void UpdateGlobalMissionInfo(KEGB_GET_GLOBAL_MISSION_INFO_ACK ack, bool isBegin = false)
    {
        string key = ack.MissionId > 0 ? ack.MissionId.ToString() : (string.IsNullOrEmpty(ack.Title) ? "default" : ack.Title);
        _missions[key] = new KGlobalMissionInfo
        {
            Active = ack.IsActive,
            MissionType = ack.MissionId,
            ClearCount = (int)Math.Clamp(ack.TargetCount, 0, int.MaxValue),
            NowCount = (int)Math.Clamp(ack.CurrentCount, 0, int.MaxValue),
        };
        if (isBegin || (ack.IsActive && !_timer.IsRunning))
            _timer.Restart();
        else if (!ack.IsActive)
            _timer.Stop();

        TickCount++;
    }

    public void UpdateGlobalMissionInfo(IDictionary<string, KGlobalMissionInfo> missions, bool isBegin = false)
    {
        _missions.Clear();
        foreach (var kv in missions)
            _missions[kv.Key] = kv.Value;
        if (isBegin)
            _timer.Restart();
        TickCount++;
    }

    public KEGS_GLOBAL_MISSION_UPDATE_NOT GetGlobalMissionInfo()
    {
        var result = new KEGS_GLOBAL_MISSION_UPDATE_NOT();
        foreach (var kv in _missions)
            result.GlobalMissionInfo[kv.Key] = kv.Value;
        return result;
    }

    public IReadOnlyDictionary<string, KGlobalMissionInfo> Missions => _missions;

    public int GetGlobalMissionStartRemainTime() => 0;
}
