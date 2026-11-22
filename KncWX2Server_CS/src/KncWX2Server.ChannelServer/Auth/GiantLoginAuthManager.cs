using KncWX2Server.Core.Database;
using KncWX2Server.Core.Events;
using KncWX2Server.Core.Framework;
using KncWX2Server.Core.Packets;
using KncWX2Server.Core.Region;

namespace KncWX2Server.ChannelServer.Auth;

/// <summary>
/// Port of ChannelUserAuth_CN.cpp + in-process Giant Login proxy (ELG_VERIFY_ACCOUNT_REQ / ELG_SECURITY_AUTH_REQ).
/// </summary>
public sealed class GiantLoginAuthManager : IRegionalAuthProvider
{
    private readonly SqliteDatabaseManager _db;
    private readonly InProcessRouter _router;

    public bool RequireMatrixCardForAccounts { get; set; }
    public RegionCode[] SupportedRegions { get; } = [RegionCode.CN];

    public GiantLoginAuthManager(SqliteDatabaseManager db, InProcessRouter router)
    {
        _db = db;
        _router = router;
    }

    public void RequestPublisherLogin(KChannelUser user, KECH_VERIFY_ACCOUNT_REQ req)
    {
        _ = Task.Run(() =>
        {
            var (netErr, _, _, _, _) = _db.VerifyOrRegisterAccount(user.Region.Code, req.Id, req.Password, autoCreateOnMissing: false);

            if (netErr == NetError.NET_OK && RequireMatrixCardForAccounts)
            {
                uint matrixKey = (uint)Random.Shared.Next(1000, 9999);
                var secNot = new KEPUBLISHER_SECURITY_INFO_NOT
                {
                    SecurityInfoType = KEPUBLISHER_SECURITY_INFO_NOT.SIT_MATRIX_CARD,
                    MatrixKey = matrixKey,
                    Coordinate = "A1,B3,C5",
                };
                _router.Send(PerformerId.PI_LOGIN_SERVER, 0, PerformerId.PI_CHANNEL_USER, user.Uid, EventId.EPUBLISHER_SECURITY_INFO_NOT, secNot);
                return;
            }

            var ack = new KEPUBLISHER_AUTHENTICATION_ACK
            {
                Ok = netErr == NetError.NET_OK ? NetError.NET_OK : NetError.ERR_GIANT_VERIFY_01,
                Id = req.Id,
                ChannelingCode = req.ChannelingCode,
            };

            _router.Send(PerformerId.PI_LOGIN_SERVER, 0, PerformerId.PI_CHANNEL_USER, user.Uid, EventId.EPUBLISHER_AUTHENTICATION_ACK, ack);
        });
    }

    public bool ShouldKeepAliveOnFailure(int netError)
        => netError == NetError.ERR_GIANT_VERIFY_01;

    public void RequestPublisherSecurityAuth(KChannelUser user, KECH_SECURITY_AUTH_REQ req)
    {
        _ = Task.Run(() =>
        {
            bool valid = !string.IsNullOrWhiteSpace(req.Matrix);
            var ack = new KEPUBLISHER_AUTHENTICATION_ACK
            {
                Ok = valid ? NetError.NET_OK : NetError.ERR_GIANT_VERIFY_01,
                Id = user.UserId,
            };
            _router.Send(PerformerId.PI_LOGIN_SERVER, 0, PerformerId.PI_CHANNEL_USER, user.Uid, EventId.EPUBLISHER_AUTHENTICATION_ACK, ack);
        });
    }

    public void HandlePublisherSecurityInfoNot(KChannelUser user, KEPUBLISHER_SECURITY_INFO_NOT info)
    {
        if (info.SecurityInfoType == KEPUBLISHER_SECURITY_INFO_NOT.SIT_MATRIX_CARD)
        {
            user.MatrixKey = info.MatrixKey;
            user.SendPacket(EventId.ECH_SECURITY_INFO_NOT, new KECH_SECURITY_INFO_NOT
            {
                InfoType = info.SecurityInfoType,
                MatrixKey = info.MatrixKey,
                Coordinate = info.Coordinate,
            });
        }
    }
}
