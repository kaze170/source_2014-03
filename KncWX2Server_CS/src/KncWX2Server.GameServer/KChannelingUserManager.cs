namespace KncWX2Server.GameServer;

/// <summary>
/// Faithful C# port of GameServer/ChannelingUserManager.{h,cpp} (SERV_CHANNELING_USER_MANAGER):
/// tracks concurrent connected user counts broken down by regional publisher channeling codes.
/// </summary>
public sealed class KChannelingUserManager
{
    public const int MaxChannelingCode = 32;

    private readonly Dictionary<int, int> _channelCounts = new();
    private readonly object _sync = new();

    public KChannelingUserManager()
    {
        for (int code = 0; code < MaxChannelingCode; code++)
            _channelCounts[code] = 0;
    }

    public void UpdateChannelingUserList(bool login, int channelingCode, bool gameServerLoggedIn = false)
    {
        if (channelingCode <= 0 || channelingCode >= MaxChannelingCode)
            return;

        lock (_sync)
        {
            int cur = _channelCounts.GetValueOrDefault(channelingCode, 0);
            if (login)
                _channelCounts[channelingCode] = cur + 1;
            else if (gameServerLoggedIn)
                _channelCounts[channelingCode] = Math.Max(0, cur - 1);
        }
    }

    public Dictionary<int, int> GetChannelingUserList()
    {
        lock (_sync) return new Dictionary<int, int>(_channelCounts);
    }
}
