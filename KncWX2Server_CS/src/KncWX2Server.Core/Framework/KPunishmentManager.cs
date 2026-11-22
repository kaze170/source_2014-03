namespace KncWX2Server.Core.Framework;

/// <summary>
/// Faithful C# port of Common/PunishmentManager.{h,cpp} (SERV_NEW_PUNISHMENT):
/// tracks timed user punishments (chat mute, trade ban, dungeon ban) with expiration check.
/// </summary>
public sealed class KPunishmentManager
{
    public enum PunishmentType
    {
        PtChat = 1,
        PtTrade = 2,
        PtDungeon = 3,
        PtPvp = 4,
    }

    public sealed class PunishmentEntry
    {
        public int Type { get; init; }
        public DateTime StartUtc { get; init; }
        public DateTime EndUtc { get; init; }
        public string Reason { get; init; } = string.Empty;
    }

    private readonly Dictionary<int, PunishmentEntry> _punishments = new();
    private readonly object _sync = new();

    public void SetPunishment(PunishmentEntry entry)
    {
        lock (_sync)
            _punishments[entry.Type] = entry;
    }

    public void ClearPunishment(int type)
    {
        lock (_sync)
            _punishments.Remove(type);
    }

    public bool IsAppliedPunishment(int type, DateTime? nowUtc = null)
    {
        var now = nowUtc ?? DateTime.UtcNow;
        lock (_sync)
        {
            if (!_punishments.TryGetValue(type, out var p))
                return false;
            if (now >= p.EndUtc)
            {
                _punishments.Remove(type);
                return false;
            }
            return now >= p.StartUtc;
        }
    }
}
