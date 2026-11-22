namespace KncWX2Server.ChannelServer.Managers;

/// <summary>
/// Faithful C# port of ChannelServer/UserHackingManager.{h,cpp} (SERV_HACKING_TOOL_LIST):
/// picks randomized 10..20s check intervals for anti-hack tool checks at channel login.
/// </summary>
public sealed class KChannelUserHackingManager
{
    public const int HeMinTimeSeconds = 10;
    public const int HeMaxTimeSeconds = 20;

    private readonly Random _rng = new();
    public double HackingToolCheckTimeSeconds { get; private set; }

    public KChannelUserHackingManager()
    {
        SetRandomCheckTime();
    }

    public void SetRandomCheckTime()
    {
        HackingToolCheckTimeSeconds = _rng.Next(HeMinTimeSeconds, HeMaxTimeSeconds);
    }
}
