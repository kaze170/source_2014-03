using System.Net.Sockets;
using System.Text;
using KncWX2Server.ChannelServer;
using KncWX2Server.Core.Database;
using KncWX2Server.Core.Events;
using KncWX2Server.Core.Framework;
using KncWX2Server.Core.Logging;
using KncWX2Server.Core.Packets;
using KncWX2Server.Core.Region;

namespace KncWX2Server.ChannelServer.Auth;

/// <summary>
/// Port of Common/OnlyGlobal/AuthAndBilling/TW/GASHAuthManager + ChannelUserAuth_TW.cpp.
/// Supports local SQLite authentication by default and optional live GASH+ TCP gateway.
/// </summary>
public sealed class GashAuthManager : IRegionalAuthProvider
{
    private readonly SqliteDatabaseManager _db;
    private readonly InProcessRouter _router;

    public string ServiceCode { get; private set; } = "300148";
    public string ServiceRegion { get; private set; } = "AF";
    public string ServerIp { get; private set; } = "127.0.0.1";
    public int AuthenticationPort { get; private set; } = 5411;
    public int AuthorizationPort { get; private set; } = 5412;
    public int AccountingPort { get; private set; } = 5413;
    public int NumThreads { get; private set; } = 1;
    public bool UseExternalGashGateway { get; set; }

    public RegionCode[] SupportedRegions { get; } = [RegionCode.TW, RegionCode.HK];

    public GashAuthManager(SqliteDatabaseManager db, InProcessRouter router)
    {
        _db = db;
        _router = router;
    }

    // Lua bindings matching config_ch_TW_SERVICE.lua
    public void InitGASHAuthValue(string serviceCode, string serviceRegion)
    {
        ServiceCode = serviceCode;
        ServiceRegion = serviceRegion;
    }

    public void InitGASHAuthAddress(string ip, int authPort, int authzPort, int acctPort)
    {
        ServerIp = ip;
        AuthenticationPort = authPort;
        AuthorizationPort = authzPort;
        AccountingPort = acctPort;
    }

    public void SetNumThread(int num) => NumThreads = Math.Max(1, num);

    public string BuildGashAuthenticationPacket(string serviceAccountId, string servicePassword, string ip)
        => $"{ServiceCode}'{ServiceRegion}'{serviceAccountId}'{servicePassword}'1'{ip}'{serviceAccountId}'\r\n";

    public void RequestPublisherLogin(KChannelUser user, KECH_VERIFY_ACCOUNT_REQ req)
    {
        var authReq = new KEPUBLISHER_AUTHENTICATION_REQ
        {
            ActorUid = user.Uid,
            Ip = user.IpStr,
            ServiceAccountId = req.Id,
            ServicePassword = req.Password,
            ServerUseKogOtp = true,
        };

        _ = Task.Run(async () =>
        {
            KEPUBLISHER_AUTHENTICATION_ACK ack;
            if (UseExternalGashGateway)
            {
                ack = await AuthenticateExternalAsync(authReq);
            }
            else
            {
                var (netErr, _, _, _, _) = _db.VerifyOrRegisterAccount(user.Region.Code, authReq.ServiceAccountId, authReq.ServicePassword, autoCreateOnMissing: false);
                ack = new KEPUBLISHER_AUTHENTICATION_ACK
                {
                    Ok = netErr == NetError.NET_OK ? NetError.NET_OK : NetError.ERR_GAMEFORGE_00,
                    Id = authReq.ServiceAccountId,
                    ChannelingCode = req.ChannelingCode,
                };
            }

            _router.Send(PerformerId.PI_CHANNEL_PUBLISHER_AUTH, 0, PerformerId.PI_CHANNEL_USER, user.Uid, EventId.EPUBLISHER_AUTHENTICATION_ACK, ack);
        });
    }

    private async Task<KEPUBLISHER_AUTHENTICATION_ACK> AuthenticateExternalAsync(KEPUBLISHER_AUTHENTICATION_REQ req)
    {
        try
        {
            using var tcp = new TcpClient();
            await tcp.ConnectAsync(ServerIp, AuthenticationPort).WaitAsync(TimeSpan.FromSeconds(5));
            using var stream = tcp.GetStream();
            byte[] payload = Encoding.ASCII.GetBytes(BuildGashAuthenticationPacket(req.ServiceAccountId, req.ServicePassword, req.Ip));
            await stream.WriteAsync(payload);

            byte[] buf = new byte[1024];
            int read = await stream.ReadAsync(buf).WaitAsync(TimeSpan.FromSeconds(5));
            string resp = Encoding.ASCII.GetString(buf, 0, read);
            string[] parts = resp.Split(';');
            bool success = parts.Length > 0 && parts[0] == "1";

            return new KEPUBLISHER_AUTHENTICATION_ACK
            {
                Ok = success ? NetError.NET_OK : NetError.ERR_GAMEFORGE_00,
                Id = req.ServiceAccountId,
            };
        }
        catch (Exception ex)
        {
            KLog.Cerr($"GASH gateway connection failed: {ex.Message}");
            return new KEPUBLISHER_AUTHENTICATION_ACK
            {
                Ok = NetError.ERR_VERIFY_21,
                Id = req.ServiceAccountId,
            };
        }
    }

    public bool ShouldKeepAliveOnFailure(int netError)
        => netError == NetError.ERR_GAMEFORGE_00
        || netError == NetError.ERR_GAMEFORGE_01
        || netError == NetError.ERR_GAMEFORGE_02;

    public void RequestPublisherSecurityAuth(KChannelUser user, KECH_SECURITY_AUTH_REQ req)
    {
        // TW/HK does not require a secondary security auth step.
    }

    public void HandlePublisherSecurityInfoNot(KChannelUser user, KEPUBLISHER_SECURITY_INFO_NOT info)
    {
    }
}
