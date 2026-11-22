using KncWX2Server.Core.Region;

namespace KncWX2Server.CenterServer;

/// <summary>
/// Faithful C# port of CenterServer/TrainingCenterRoom.{h,cpp}:
/// single-player training center room lifecycle (Init -> Wait -> Play -> Close),
/// remaining-playtime toggle, clear/fail/drop completion statistics, and auto-close on empty.
/// </summary>
public sealed class KTrainingCenterRoom
{
    public enum TcRoomState
    {
        Init = 0,
        Wait = 1,
        Play = 2,
        Close = 3,
    }

    public enum TcGameResult : sbyte
    {
        Success = 0,
        Fail = 1,
        Drop = 2,
    }

    private readonly HashSet<long> _members = [];
    private DateTime _playStartedUtc;

    public long RoomUid { get; init; }
    public int TcId { get; private set; }
    public int DungeonId { get; private set; }
    public float PlayTimeLimit { get; private set; }
    public bool RemainingPlayTimeEnabled { get; private set; }
    public TcRoomState State { get; private set; } = TcRoomState.Init;

    public int MemberCount => _members.Count;
    public bool IsEmpty => _members.Count == 0;

    public int OpenRoom(long unitUid, int tcId, int dungeonId, float playTimeLimit, DateTime? nowUtc = null)
    {
        if (State != TcRoomState.Init)
            return NetError.ERR_ROOM_09;

        TcId = tcId;
        DungeonId = dungeonId;
        PlayTimeLimit = playTimeLimit;
        RemainingPlayTimeEnabled = false; // TrainingCenter enables timer only after client narration finishes
        _members.Clear();
        _members.Add(unitUid);
        _playStartedUtc = nowUtc ?? DateTime.UtcNow;
        State = TcRoomState.Play;
        return NetError.NET_OK;
    }

    public int SetRemainingTimeEnabled(bool enabled)
    {
        if (State != TcRoomState.Play)
            return NetError.ERR_ROOM_09;
        RemainingPlayTimeEnabled = enabled;
        return NetError.NET_OK;
    }

    public (int ok, TcGameResult result, int playTimeSeconds) EndGame(long unitUid, bool isSuccess, DateTime? nowUtc = null)
    {
        if (State != TcRoomState.Play)
            return (NetError.ERR_ROOM_09, TcGameResult.Fail, 0);

        int ok = _members.Remove(unitUid) ? NetError.NET_OK : NetError.ERR_SLOT_04;
        var now = nowUtc ?? DateTime.UtcNow;
        int playSec = (int)Math.Max(0, Math.Round((now - _playStartedUtc).TotalSeconds));
        if (_members.Count == 0)
            State = TcRoomState.Close;
        return (ok, isSuccess ? TcGameResult.Success : TcGameResult.Fail, playSec);
    }

    public (int ok, TcGameResult result, int playTimeSeconds) LeaveRoom(long unitUid, DateTime? nowUtc = null)
    {
        if (State == TcRoomState.Init || State == TcRoomState.Close)
            return (NetError.ERR_ROOM_09, TcGameResult.Drop, 0);
        if (!_members.Remove(unitUid))
            return (NetError.ERR_SLOT_04, TcGameResult.Drop, 0);

        var now = nowUtc ?? DateTime.UtcNow;
        int playSec = (int)Math.Max(0, Math.Round((now - _playStartedUtc).TotalSeconds));
        if (_members.Count == 0)
            State = TcRoomState.Close;
        return (NetError.NET_OK, TcGameResult.Drop, playSec);
    }
}
