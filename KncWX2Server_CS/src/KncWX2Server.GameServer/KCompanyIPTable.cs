namespace KncWX2Server.GameServer;

/// <summary>
/// Faithful C# port of GameServer/CompanyIPTable.{h,cpp}:
/// validates GM cheat command origin IPs against configured company IP allowlist.
/// </summary>
public sealed class KCompanyIPTable
{
    private readonly HashSet<string> _allowedIps = new(StringComparer.OrdinalIgnoreCase)
    {
        "127.0.0.1",
    };

    public bool RestrictGmCheatByIp { get; set; }

    public bool AddCompanyIp(string ip)
    {
        if (string.IsNullOrWhiteSpace(ip) || ip.Length > 15)
            return false;
        return _allowedIps.Add(ip.Trim());
    }

    public bool CheckIp(string? ip)
    {
        if (!RestrictGmCheatByIp) return true;
        if (string.IsNullOrWhiteSpace(ip)) return false;
        return _allowedIps.Contains(ip.Trim());
    }
}
