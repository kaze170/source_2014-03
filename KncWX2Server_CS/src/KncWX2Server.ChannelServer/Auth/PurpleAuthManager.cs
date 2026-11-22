using KncWX2Server.ChannelServer;
using KncWX2Server.Core.Database;
using KncWX2Server.Core.Events;
using KncWX2Server.Core.Framework;
using KncWX2Server.Core.Packets;
using KncWX2Server.Core.Region;

namespace KncWX2Server.ChannelServer.Auth;

/// <summary>
/// Port of Common/OnlyGlobal/AuthAndBilling/JP/PURPLEAuthManager + ChannelUserAuth_JP.cpp.
/// </summary>
public sealed class PurpleAuthManager : IRegionalAuthProvider
{
    private readonly SqliteDatabaseManager _db;
    private readonly InProcessRouter _router;

    public int NumThreads { get; private set; } = 10;
    public RegionCode[] SupportedRegions { get; } = [RegionCode.JP];

    public PurpleAuthManager(SqliteDatabaseManager db, InProcessRouter router)
    {
        _db = db;
        _router = router;
    }

    // Lua binding matching config_ch_NHN_JAPAN_SERVICE.lua
    public void SetNumThread(int num) => NumThreads = Math.Max(1, num);

    public void RequestPublisherLogin(KChannelUser user, KECH_VERIFY_ACCOUNT_REQ req)
    {
        _ = Task.Run(() =>
        {
            var (netErr, _, _, _, _) = _db.VerifyOrRegisterAccount(user.Region.Code, req.Id, req.Password, autoCreateOnMissing: false);
            var ack = new KEPUBLISHER_AUTHENTICATION_ACK
            {
                Ok = netErr == NetError.NET_OK ? NetError.NET_OK : NetError.ERR_GAMEFORGE_00,
                Id = req.Id,
                ChannelingCode = req.ChannelingCode,
            };

            _router.Send(PerformerId.PI_CHANNEL_PUBLISHER_AUTH, 0, PerformerId.PI_CHANNEL_USER, user.Uid, EventId.EPUBLISHER_AUTHENTICATION_ACK, ack);
        });
    }

    public bool ShouldKeepAliveOnFailure(int netError)
        => netError == NetError.ERR_GAMEFORGE_00
        || netError == NetError.ERR_GAMEFORGE_01
        || netError == NetError.ERR_GAMEFORGE_02;

    public void RequestPublisherSecurityAuth(KChannelUser user, KECH_SECURITY_AUTH_REQ req)
    {
    }

    public void HandlePublisherSecurityInfoNot(KChannelUser user, KEPUBLISHER_SECURITY_INFO_NOT info)
    {
    }
}
