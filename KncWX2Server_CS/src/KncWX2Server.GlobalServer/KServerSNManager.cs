using KncWX2Server.Core.Region;

namespace KncWX2Server.GlobalServer;

/// <summary>
/// Faithful C# port of GlobalServer/ServerSNManager.{h,cpp} (SERV_SERIAL_NUMBER_AVAILABILITY_CHECK_FILTER):
/// tracks active session SerialNumbers across GameServers, detecting duplicate SN logins (ERR_SN_OVERLAP_01)
/// and clean deregistration (ERR_SN_OVERLAP_02).
/// </summary>
public sealed class KServerSNManager
{
    private readonly Dictionary<long, Dictionary<long, long>> _snToUsers = new();
    private readonly object _sync = new();

    public int ActiveSerialCount
    {
        get { lock (_sync) return _snToUsers.Count; }
    }

    public (int result, Dictionary<long, long> overlapUsers) RegServerSN(long serverSn, long userUid, long gsUid)
    {
        lock (_sync)
        {
            if (_snToUsers.TryGetValue(serverSn, out var existing))
            {
                existing[userUid] = gsUid;
                return (NetError.ERR_SN_OVERLAP_01, new Dictionary<long, long>(existing));
            }

            _snToUsers[serverSn] = new Dictionary<long, long> { [userUid] = gsUid };
            return (NetError.NET_OK, new Dictionary<long, long>());
        }
    }

    public int UnRegServerSN(long serverSn, long userUid)
    {
        lock (_sync)
        {
            if (!_snToUsers.TryGetValue(serverSn, out var existing))
                return NetError.ERR_SN_OVERLAP_02;

            if (existing.Count > 1)
            {
                if (!existing.Remove(userUid))
                    return NetError.ERR_SN_OVERLAP_02;
                return NetError.NET_OK;
            }

            if (!existing.ContainsKey(userUid))
                return NetError.ERR_SN_OVERLAP_02;

            _snToUsers.Remove(serverSn);
            return NetError.NET_OK;
        }
    }
}
