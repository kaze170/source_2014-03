namespace KncWX2Server.GameServer;

/// <summary>
/// Faithful C# port of GameServer/UserJumpingCharacterManager.{h,cpp} (SERV_JUMPING_CHARACTER):
/// manages jumping-character event registration per account/unit within configured event date windows.
/// </summary>
public sealed class KUserJumpingCharacterManager
{
    public enum JumpingCharacterResult
    {
        JcrSucceed = 0,
        JcrFailed = 1,
        JcrNotInit = 2,
        JcrAlreadyInit = 3,
    }

    private readonly Dictionary<long, DateTime> _jumpingMap = new();
    public bool IsInit { get; private set; }

    public JumpingCharacterResult Init(IReadOnlyDictionary<long, string> jumpingInfo)
    {
        if (IsInit)
            return JumpingCharacterResult.JcrAlreadyInit;

        _jumpingMap.Clear();
        foreach (var kv in jumpingInfo)
        {
            if (!DateTime.TryParse(kv.Value, out var dt))
                return JumpingCharacterResult.JcrFailed;
            _jumpingMap[kv.Key] = dt;
        }

        IsInit = true;
        return JumpingCharacterResult.JcrSucceed;
    }

    public JumpingCharacterResult Init(IReadOnlyDictionary<long, DateTime> jumpedInfo)
    {
        if (IsInit)
            return JumpingCharacterResult.JcrAlreadyInit;

        _jumpingMap.Clear();
        foreach (var kv in jumpedInfo)
            _jumpingMap[kv.Key] = kv.Value;

        IsInit = true;
        return JumpingCharacterResult.JcrSucceed;
    }

    public JumpingCharacterResult Check(long unitUid, DateTime eventBeginUtc, DateTime eventEndUtc, bool accountEvent)
    {
        if (!IsInit)
            return JumpingCharacterResult.JcrNotInit;

        foreach (var kv in _jumpingMap)
        {
            if (kv.Value < eventBeginUtc) continue;
            if (eventEndUtc < kv.Value) continue;
            if (!accountEvent && kv.Key != unitUid) continue;
            return JumpingCharacterResult.JcrFailed;
        }
        return JumpingCharacterResult.JcrSucceed;
    }

    public JumpingCharacterResult SetJumpingCharacter(long unitUid, DateTime? regDateUtc = null)
    {
        if (!IsInit) return JumpingCharacterResult.JcrNotInit;
        return _jumpingMap.TryAdd(unitUid, regDateUtc ?? DateTime.UtcNow)
            ? JumpingCharacterResult.JcrSucceed
            : JumpingCharacterResult.JcrFailed;
    }

    public JumpingCharacterResult ResetJumpingCharacter(long unitUid)
    {
        if (!IsInit) return JumpingCharacterResult.JcrNotInit;
        _jumpingMap.Remove(unitUid);
        return JumpingCharacterResult.JcrSucceed;
    }

    public JumpingCharacterResult IsJumpingCharacter(long unitUid)
    {
        if (!IsInit) return JumpingCharacterResult.JcrNotInit;
        return _jumpingMap.ContainsKey(unitUid)
            ? JumpingCharacterResult.JcrSucceed
            : JumpingCharacterResult.JcrFailed;
    }
}
