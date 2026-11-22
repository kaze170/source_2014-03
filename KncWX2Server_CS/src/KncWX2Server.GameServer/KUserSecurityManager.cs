namespace KncWX2Server.GameServer;

/// <summary>
/// Faithful C# port of GameServer/UserSecurityManager.{h,cpp} (SERV_MACHINE_ID_BLOCK,
/// SERV_SERIAL_NUMBER_MORNITORING, SERV_IMPOSSIBLE_SELL_ITEM):
/// validates Base64-encoded 16-byte MachineID checksums (weighted uint16 MAC+Serial hash),
/// tracks ServerSN monitoring intervals (30s), and schedules randomized delayed-kick timers.
/// </summary>
public sealed class KUserSecurityManager
{
    public enum ServerSnMonitoring
    {
        SsmNone = 0,
        SsmMonitoring = 1,
        SsmBlock = 2,
    }

    public enum KickType : sbyte
    {
        KtNone = 0,
        KtAfterRandomTime = 1,
    }

    private readonly Random _rng = new();
    private DateTime _snMonitoringTimerUtc = DateTime.UtcNow;
    private DateTime _kickTimerUtc = DateTime.UtcNow;
    private int _kickRandomSeconds;
    private KickType _kickState = KickType.KtNone;

    public string MachineId { get; set; } = string.Empty;
    public int ChannelRandomKey { get; set; }
    public long ServerSn { get; set; }
    public ServerSnMonitoring MonitoringFlag { get; set; } = ServerSnMonitoring.SsmNone;
    public int KickReason { get; set; }

    public static ushort GetHashFromMachineId(byte[] decodedMachineId)
    {
        if (decodedMachineId.Length < 16) return 0;
        return BitConverter.ToUInt16(decodedMachineId, 14);
    }

    public static ushort MakeHashFromMachineId(byte[] decodedMachineId)
    {
        if (decodedMachineId.Length < 16) return 0;
        ushort hash = 0;
        unchecked
        {
            for (int i = 0; i < 6; i++)
            {
                ushort word = BitConverter.ToUInt16(decodedMachineId, i * 2);
                hash += (ushort)(word * (i + 3));
            }
        }
        return hash;
    }

    public static string EncodeMachineIdWithChecksum(byte[] raw14PlusBytes)
    {
        var buf = new byte[16];
        Array.Copy(raw14PlusBytes, 0, buf, 0, Math.Min(14, raw14PlusBytes.Length));
        ushort hash = MakeHashFromMachineId(buf);
        BitConverter.GetBytes(hash).CopyTo(buf, 14);
        return Convert.ToBase64String(buf);
    }

    public bool CheckValidMachineId(string base64MachineId)
    {
        if (string.IsNullOrEmpty(base64MachineId) || base64MachineId.Length < 16)
            return false;
        try
        {
            byte[] decoded = Convert.FromBase64String(base64MachineId);
            if (decoded.Length < 16) return false;
            return GetHashFromMachineId(decoded) == MakeHashFromMachineId(decoded);
        }
        catch (FormatException)
        {
            return false;
        }
    }

    public bool CheckServerSnMonitoring(DateTime? nowUtc = null)
    {
        if (MonitoringFlag != ServerSnMonitoring.SsmMonitoring)
            return false;
        var now = nowUtc ?? DateTime.UtcNow;
        if ((now - _snMonitoringTimerUtc).TotalSeconds < 30.0)
            return false;
        _snMonitoringTimerUtc = now;
        return true;
    }

    public void SetKickRandomTime(int minTimeSec, int rangeSec, DateTime? nowUtc = null)
    {
        int clampedRange = Math.Max(1, rangeSec);
        _kickRandomSeconds = _rng.Next(clampedRange) + minTimeSec;
        _kickTimerUtc = nowUtc ?? DateTime.UtcNow;
    }

    public void SetKickState(KickType state) => _kickState = state;

    public bool CheckIsKickUser(DateTime? nowUtc = null)
    {
        if (_kickState != KickType.KtAfterRandomTime)
            return false;
        var now = nowUtc ?? DateTime.UtcNow;
        if ((now - _kickTimerUtc).TotalSeconds < _kickRandomSeconds)
            return false;

        _kickTimerUtc = now;
        _kickState = KickType.KtNone;
        return true;
    }
}
