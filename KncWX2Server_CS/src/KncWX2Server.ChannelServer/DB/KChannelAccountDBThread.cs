using System.Security.Cryptography;
using KncWX2Server.Core.Database;
using KncWX2Server.Core.Events;
using KncWX2Server.Core.Framework;
using KncWX2Server.Core.Logging;
using KncWX2Server.Core.Packets;
using KncWX2Server.Core.Region;

namespace KncWX2Server.ChannelServer.DB;

/// <summary>
/// Port of ChannelServer/ChannelAccountDBThread.{h,cpp} backed by SqliteDatabaseManager.
/// </summary>
public sealed class KChannelAccountDBThread : KDBThread
{
    private readonly Func<RegionProfile> _getRegion;

    public override uint PerformerTypeId => PerformerId.PI_CHANNEL_ACCOUNT_DB;

    public KChannelAccountDBThread(SqliteDatabaseManager db, InProcessRouter router, Func<RegionProfile> getRegion)
        : base(db, router)
    {
        _getRegion = getRegion;
    }

    protected override void ProcessEvent(KEvent ev)
    {
        var region = _getRegion();
        switch (ev.EventId)
        {
            case EventId.DBE_CH_USER_GENERAL_LOGIN_REQ:
                HandleGeneralLogin(ev, region);
                break;

            case EventId.DBE_CH_USER_KOGOTP_LOGIN_REQ:
                HandleKogOtpLogin(ev, region);
                break;

            case EventId.DBE_GET_CONCURRENT_USER_LOGIN_REQ:
                HandleGetConcurrentUserLogin(ev, region);
                break;

            case EventId.DBE_UPDATE_CHANNEL_RANDOMKEY_REQ:
                HandleUpdateRandomKey(ev, region);
                break;

            case EventId.DBE_CHECK_ACCOUNT_BLOCK_LIST_REQ:
                HandleCheckAccountBlockList(ev);
                break;

            case EventId.DBE_CHECK_SERVER_SN_REQ:
                HandleCheckServerSn(ev);
                break;

            case EventId.DBE_CREATE_SERVER_SN_REQ:
                HandleCreateServerSn(ev);
                break;

            case EventId.DBE_IP_CHECK_LIST_NOT:
                HandleIpCheckListNot(ev);
                break;

            default:
                KLog.Cwarn($"[KChannelAccountDBThread] Unhandled DB event: {ev.EventId} ({ev.EventIdValue})");
                break;
        }
    }

    private void HandleGeneralLogin(KEvent ev, RegionProfile region)
    {
        var req = ev.GetPayload<KDBE_CH_USER_GENERAL_LOGIN_REQ>(region);
        long actorUid = ev.Trace0;

        // If AuthFlag == Internal, verify password if supplied; if already publisher-verified, auto-provision account row in SQLite.
        var (netErr, userUid, _, _, _) = Db.VerifyOrRegisterAccount(
            region.Code,
            req.ServiceAccountId,
            req.Password,
            autoCreateOnMissing: !string.IsNullOrEmpty(req.ServiceAccountId) && string.IsNullOrEmpty(req.Password));

        string otp = netErr == NetError.NET_OK
            ? Convert.ToHexString(RandomNumberGenerator.GetBytes(16))
            : string.Empty;

        var ack = new KDBE_CH_USER_GENERAL_LOGIN_ACK
        {
            Ok = netErr,
            UserId = req.ServiceAccountId,
            UserUid = userUid,
            Otp = otp,
            ChannelingCode = req.ChannelingCode,
            PurchaseTok = req.PurchaseTok,
        };

        SendToUser(actorUid, EventId.DBE_CH_USER_GENERAL_LOGIN_ACK, ack, ev.Trace);
    }

    private void HandleKogOtpLogin(KEvent ev, RegionProfile region)
    {
        var req = ev.GetPayload<KDBE_CH_USER_KOGOTP_LOGIN_REQ>(region);
        long actorUid = ev.Trace0;

        var (netErr, userUid, _, _, _) = Db.VerifyOrRegisterAccount(
            region.Code,
            req.ServiceAccountId,
            string.Empty,
            autoCreateOnMissing: false);

        var ack = new KDBE_CH_USER_KOGOTP_LOGIN_ACK
        {
            Ok = netErr,
            UserId = req.ServiceAccountId,
            UserUid = userUid,
            Otp = req.Otp,
        };

        SendToUser(actorUid, EventId.DBE_CH_USER_KOGOTP_LOGIN_ACK, ack, ev.Trace);
    }

    private void HandleGetConcurrentUserLogin(KEvent ev, RegionProfile region)
    {
        var req = ev.GetPayload<KDBE_GET_CONCURRENT_USER_LOGIN_REQ>(region);
        var (netErr, _, authLevel, _, _) = Db.VerifyOrRegisterAccount(region.Code, req.Id, req.Pw, autoCreateOnMissing: false);

        var ack = new KDBE_GET_CONCURRENT_USER_LOGIN_ACK
        {
            Ok = netErr,
            AuthLevel = authLevel,
        };
        SendToUser(ev.Trace0, EventId.DBE_GET_CONCURRENT_USER_LOGIN_ACK, ack, ev.Trace);
    }

    private void HandleUpdateRandomKey(KEvent ev, RegionProfile region)
    {
        var req = ev.GetPayload<KDBE_UPDATE_CHANNEL_RANDOMKEY_REQ>(region);
        int randomKey = Random.Shared.Next(100000, 999999);
        Db.UpdateRandomKey(region.Code, req.UserId, randomKey.ToString());

        SendToUser(ev.Trace0, EventId.DBE_UPDATE_CHANNEL_RANDOMKEY_ACK, new KDBE_UPDATE_CHANNEL_RANDOMKEY_ACK
        {
            Ok = NetError.NET_OK,
            RandomKey = randomKey,
        }, ev.Trace);
    }

    private void HandleCheckAccountBlockList(KEvent ev)
    {
        SendToServer(EventId.DBE_CHECK_ACCOUNT_BLOCK_LIST_ACK, new KDBE_CHECK_ACCOUNT_BLOCK_LIST_ACK(), ev.Trace);
    }

    private void HandleCheckServerSn(KEvent ev)
    {
        var req = ev.GetPayload<KDBE_CHECK_SERVER_SN_REQ>(_getRegion());
        bool exists = Db.CheckServerSNExists(req.ServerSN);
        long sn = exists ? req.ServerSN : Db.CreateServerSN(0);

        SendToUser(ev.Trace0, EventId.DBE_CHECK_SERVER_SN_ACK, new KDBE_CHECK_SERVER_SN_ACK
        {
            Ok = exists ? NetError.NET_OK : NetError.ERR_SERVERSN_01,
            ServerSN = sn,
        }, ev.Trace);
    }

    private void HandleCreateServerSn(KEvent ev)
    {
        long newSn = Db.CreateServerSN(0);
        SendToUser(ev.Trace0, EventId.DBE_CREATE_SERVER_SN_ACK, new KDBE_CREATE_SERVER_SN_ACK
        {
            Ok = NetError.NET_OK,
            ServerSN = newSn,
        }, ev.Trace);
    }

    private void HandleIpCheckListNot(KEvent ev)
    {
        KLog.Cout($"[KChannelAccountDBThread] IP check recorded for trace={ev.Trace0}");
    }
}
