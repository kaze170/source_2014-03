using KncWX2Server.ChannelServer.Managers;
using KncWX2Server.Core.Events;
using KncWX2Server.Core.Framework;
using KncWX2Server.Core.Logging;
using KncWX2Server.Core.Network;
using KncWX2Server.Core.Packets;
using KncWX2Server.Core.Region;
using KncWX2Server.Core.Serialization;

namespace KncWX2Server.ChannelServer;

/// <summary>
/// Port of ChannelServer/ChannelUser.{h,cpp} and OnlyGlobal/ChannelUserAuth_{TW,CN,JP}.cpp.
/// </summary>
public sealed class KChannelUser : KActor
{
    private readonly KChannelServer _server;
    private readonly KRepeatEventFilter _repeatFilter = new();
    private readonly KUserSecurityManager _securityManager = new();
    public KChannelUserHackingManager HackingManager { get; } = new();

    public string UserId { get; private set; } = string.Empty;
    public long AccountUid { get; private set; }
    public uint MatrixKey { get; set; }
    public bool PassedEnterChannel { get; private set; }
    public int AuthLevel { get; private set; }

    public override uint PerformerTypeId => PerformerId.PI_CHANNEL_USER;

    public KChannelUser(KTcpSession session, KChannelServer server)
        : base(session, new KCHFSM(), server.Region)
    {
        _server = server;

        _repeatFilter.RegRepeatEvent(EventId.ECH_VERIFY_ACCOUNT_REQ, EventId.ECH_VERIFY_ACCOUNT_ACK);
        _repeatFilter.RegRepeatEvent(EventId.ECH_GET_CHANNEL_LIST_REQ, EventId.ECH_GET_CHANNEL_LIST_ACK);
        _repeatFilter.RegRepeatEvent(EventId.E_TOOL_GET_CCU_INFO_REQ, EventId.E_TOOL_GET_CCU_INFO_ACK);
    }

    public override void OnDestroy()
    {
        if (IsDestroyed) return;
        base.OnDestroy();
        _server.WaitQueueManager.UnregWaitUser(Uid);
        _server.WaitQueueManager.UnregPassedUser(Uid);
    }

    public new bool SendPacket(EventId eventId, IKSerializable packet)
    {
        _repeatFilter.CheckAckEvent((ushort)eventId);
        return base.SendPacket(eventId, packet);
    }

    public void NotifyWaitQueueEnterChannel()
    {
        if (Fsm.CurrentState == KCHFSM.S_WAIT_QUEUE)
            Fsm.TryStateTransition(KCHFSM.I_TO_AUTHENTICATED, out _);

        PassedEnterChannel = true;
        SendChannelList();
    }

    public void SendChannelList()
    {
        var not = new KECH_GET_CHANNEL_LIST_NOT
        {
            ServerGroupId = _server.ServerGroupId,
            ChannelList = _server.GetChannelListSnapshot(),
            ChannelBonusList = _server.GetChannelBonusListSnapshot(),
        };
        SendPacket(EventId.ECH_GET_CHANNEL_LIST_NOT, not);
    }

    protected override void ProcessEvent(KEvent ev)
    {
        if (ev.FromType == EventFromType.Client && !_repeatFilter.CheckReqEvent(ev.EventIdValue))
        {
            KLog.Cwarn($"[KChannelUser] Duplicate repeat request blocked: {ev.EventId} from UID={Uid}");
            return;
        }

        switch (ev.EventId)
        {
            case EventId.ECH_VERIFY_ACCOUNT_REQ:
                OnVerifyAccountReq(ev.GetPayload<KECH_VERIFY_ACCOUNT_REQ>(Region));
                break;

            case EventId.EPUBLISHER_AUTHENTICATION_ACK:
                OnPublisherAuthenticationAck(ev.GetPayload<KEPUBLISHER_AUTHENTICATION_ACK>(Region));
                break;

            case EventId.DBE_CH_USER_GENERAL_LOGIN_ACK:
                OnGeneralLoginAck(ev.GetPayload<KDBE_CH_USER_GENERAL_LOGIN_ACK>(Region));
                break;

            case EventId.DBE_CH_USER_KOGOTP_LOGIN_ACK:
                OnKogOtpLoginAck(ev.GetPayload<KDBE_CH_USER_KOGOTP_LOGIN_ACK>(Region));
                break;

            case EventId.ECH_SECURITY_AUTH_REQ:
                OnSecurityAuthReq(ev.GetPayload<KECH_SECURITY_AUTH_REQ>(Region));
                break;

            case EventId.EPUBLISHER_SECURITY_INFO_NOT:
                OnPublisherSecurityInfoNot(ev.GetPayload<KEPUBLISHER_SECURITY_INFO_NOT>(Region));
                break;

            case EventId.EPUBLISHER_SECURITY_AUTH_ACK:
                OnPublisherSecurityAuthAck(ev.GetPayload<KEPUBLISHER_SECURITY_AUTH_ACK>(Region));
                break;

            case EventId.ECH_GET_CHANNEL_LIST_REQ:
                OnGetChannelListReq();
                break;

            case EventId.ECH_GET_SERVERGROUP_LIST_REQ:
                OnGetServerGroupListReq();
                break;

            case EventId.ECH_GET_SERVER_TIME_REQ:
                OnGetServerTimeReq();
                break;

            case EventId.ECH_DISCONNECT_REQ:
                OnDisconnectReq();
                break;

            case EventId.ECH_PCBANG_IP_AND_MAC_INFO_NOT:
                OnPcBangIpAndMacNot(ev.GetPayload<KECH_PCBANG_IP_AND_MAC_INFO_NOT>(Region));
                break;

            case EventId.E_TOOL_CHECK_LOGIN_REQ:
                OnToolCheckLoginReq(ev.GetPayload<KE_TOOL_CHECK_LOGIN_REQ>(Region));
                break;

            case EventId.DBE_GET_CONCURRENT_USER_LOGIN_ACK:
                OnGetConcurrentUserLoginAck(ev.GetPayload<KDBE_GET_CONCURRENT_USER_LOGIN_ACK>(Region));
                break;

            case EventId.E_TOOL_GET_CCU_INFO_REQ:
                OnToolGetCcuInfoReq();
                break;

            case EventId.DBE_GET_CONCURRENT_USER_INFO_ACK:
                OnGetConcurrentUserInfoAck(ev.GetPayload<KDBE_GET_CONCURRENT_USER_INFO_ACK>(Region));
                break;

            case EventId.E_TOOL_SERVER_LIST_REQ:
                OnToolServerListReq();
                break;

            case EventId.DBE_GET_GAME_SERVER_LIST_ACK:
                OnGetGameServerListAck(ev.GetPayload<KDBE_GET_GAME_SERVER_LIST_ACK>(Region));
                break;

            case EventId.DBE_CHECK_SERVER_SN_ACK:
                OnCheckServerSnAck(ev.GetPayload<KDBE_CHECK_SERVER_SN_ACK>(Region));
                break;

            case EventId.DBE_CREATE_SERVER_SN_ACK:
                OnCreateServerSnAck(ev.GetPayload<KDBE_CREATE_SERVER_SN_ACK>(Region));
                break;

            case EventId.ECH_HACKING_TOOL_LIST_ACK:
                OnHackingToolListAck(ev.GetPayload<KECH_HACKING_TOOL_LIST_ACK>(Region));
                break;

            case EventId.DBE_UPDATE_CHANNEL_RANDOMKEY_ACK:
                OnUpdateChannelRandomKeyAck(ev.GetPayload<KDBE_UPDATE_CHANNEL_RANDOMKEY_ACK>(Region));
                break;

            case EventId.ECH_VERIFY_ACCOUNT_ACK:
                OnVerifyAccountAck(ev.GetPayload<KECH_VERIFY_ACCOUNT_ACK>(Region));
                break;

            case EventId.E_RESERVE_DESTROY:
                ReserveDestroy();
                break;

            default:
                KLog.Cwarn($"[KChannelUser] Unhandled event: {ev.EventId} ({ev.EventIdValue}) UID={Uid}");
                break;
        }
    }

    private void OnVerifyAccountAck(KECH_VERIFY_ACCOUNT_ACK ack)
    {
        if (StateId != KCHFSM.S_INIT) return;
        SendPacket(EventId.ECH_VERIFY_ACCOUNT_ACK, ack);
        if (ack.Ok == NetError.NET_OK)
        {
            AccountUid = ack.UserUid;
            Fsm.TryStateTransition(KCHFSM.I_TO_AUTHENTICATED, out _);
        }
        else
        {
            ReserveDestroy();
        }
    }

    private void OnVerifyAccountReq(KECH_VERIFY_ACCOUNT_REQ req)
    {
        if (StateId != KCHFSM.S_INIT)
        {
            KLog.Cwarn($"ECH_VERIFY_ACCOUNT_REQ in invalid state {StateIdString} UID={Uid}");
            return;
        }

        if (!_server.ChannelSimLayer.CheckIP(IpStr))
        {
            KLog.Cout($"[IP Blocked] Rejected connection from {IpStr} UserID={req.Id}");
            SendPacket(EventId.ECH_VERIFY_ACCOUNT_ACK, new KECH_VERIFY_ACCOUNT_ACK { Ok = NetError.ERR_CONNECT_05 });
            ReserveDestroy();
            return;
        }

        if (ContainsInvalidSqlChars(req.Id) || ContainsInvalidSqlChars(req.Password))
        {
            SendPacket(EventId.ECH_VERIFY_ACCOUNT_ACK, new KECH_VERIFY_ACCOUNT_ACK { Ok = NetError.ERR_ODBC_00 });
            return;
        }

        UserId = req.Id;
        var sim = _server.ChannelSimLayer;

        if (Region.MachineIdCheckBeforeLoading && req.UseVerify)
        {
            if (_server.GameSysVal.MachineIdDuplicateCheck &&
                !string.IsNullOrEmpty(req.MachineId) &&
                _server.MachineBlockManager.IsBlockedMachine(req.MachineId, _server.Db))
            {
                SendPacket(EventId.ECH_VERIFY_ACCOUNT_ACK, new KECH_VERIFY_ACCOUNT_ACK { Ok = NetError.ERR_VERIFY_17 });
                ReserveDestroy();
                return;
            }
        }

        if (Region.SerialNumberAvailabilityCheck &&
            !KServerSNMaker.IsEmptyServerSN(req.ServerSN) &&
            !KServerSNMaker.CheckServerSN(req.ServerSN, out _))
        {
            KLog.Cwarn($"[KChannelUser] Invalid ServerSN from UserID={req.Id}");
        }

        if (sim.AuthFlag == AuthFlag.Internal)
        {
            if (sim.UseKogOtp)
            {
                if (!req.IdVerified)
                {
                    var dbReq = new KDBE_CH_USER_GENERAL_LOGIN_REQ
                    {
                        ServiceAccountId = req.Id,
                        Ip = IpStr,
                        ChannelingCode = req.ChannelingCode,
                        ServerUseKogOtp = true,
                        Password = req.Password,
                    };
                    SendToAccountDB(EventId.DBE_CH_USER_GENERAL_LOGIN_REQ, dbReq);
                }
                else
                {
                    RequestKogOtpLogin(req);
                }
            }
            else
            {
                Fsm.TryStateTransition(KCHFSM.I_TO_AUTHENTICATED, out _);
                SendPacket(EventId.ECH_VERIFY_ACCOUNT_ACK, new KECH_VERIFY_ACCOUNT_ACK
                {
                    Ok = NetError.NET_OK,
                    UserId = req.Id,
                    CurrentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                });
            }
            return;
        }

        if (sim.UseKogOtp && req.IdVerified)
        {
            RequestKogOtpLogin(req);
        }
        else
        {
            _server.GetAuthProvider(Region.Region).RequestPublisherLogin(this, req);
        }
    }

    private void RequestKogOtpLogin(KECH_VERIFY_ACCOUNT_REQ req)
    {
        var dbReq = new KDBE_CH_USER_KOGOTP_LOGIN_REQ
        {
            ServiceAccountId = req.Id,
            Otp = req.Password,
            Ip = IpStr,
            ChannelingCode = req.ChannelingCode,
        };
        SendToAccountDB(EventId.DBE_CH_USER_KOGOTP_LOGIN_REQ, dbReq);
    }

    private void OnPublisherAuthenticationAck(KEPUBLISHER_AUTHENTICATION_ACK ack)
    {
        if (StateId != KCHFSM.S_INIT) return;

        ProcessLoginResultCount(ack.Ok);

        if (ack.Ok == NetError.NET_OK)
        {
            var dbReq = new KDBE_CH_USER_GENERAL_LOGIN_REQ
            {
                ServiceAccountId = ack.Id,
                PublisherUid = ack.PublisherUid,
                Ip = IpStr,
                ChannelingCode = ack.ChannelingCode,
                AccountType = ack.AccountType,
                PurchaseTok = ack.PurchaseTok,
                ServerUseKogOtp = _server.ChannelSimLayer.UseKogOtp,
            };
            SendToAccountDB(EventId.DBE_CH_USER_GENERAL_LOGIN_REQ, dbReq);
        }
        else
        {
            SendPacket(EventId.ECH_VERIFY_ACCOUNT_ACK, new KECH_VERIFY_ACCOUNT_ACK
            {
                Ok = ack.Ok,
                AgreementUrl = ack.AgreementUrl,
            });

            if (!_server.GetAuthProvider(Region.Region).ShouldKeepAliveOnFailure(ack.Ok))
                ReserveDestroy();
        }
    }

    private void OnGeneralLoginAck(KDBE_CH_USER_GENERAL_LOGIN_ACK ack)
    {
        if (StateId != KCHFSM.S_INIT) return;

        if (_server.ChannelSimLayer.AuthFlag == AuthFlag.Internal)
            ProcessLoginResultCount(ack.Ok);

        AccountUid = ack.UserUid;
        var clientAck = new KECH_VERIFY_ACCOUNT_ACK
        {
            Ok = ack.Ok,
            UserId = ack.UserId,
            UserUid = ack.UserUid,
            Passport = ack.Otp,
            ChannelingCode = ack.ChannelingCode,
            PurchaseTok = ack.PurchaseTok,
            CurrentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
        };
        SendPacket(EventId.ECH_VERIFY_ACCOUNT_ACK, clientAck);

        if (ack.Ok == NetError.NET_OK)
            Fsm.TryStateTransition(KCHFSM.I_TO_AUTHENTICATED, out _);
        else
            ReserveDestroy();
    }

    private void OnKogOtpLoginAck(KDBE_CH_USER_KOGOTP_LOGIN_ACK ack)
    {
        if (StateId != KCHFSM.S_INIT) return;

        AccountUid = ack.UserUid;
        SendPacket(EventId.ECH_VERIFY_ACCOUNT_ACK, new KECH_VERIFY_ACCOUNT_ACK
        {
            Ok = ack.Ok,
            UserId = ack.UserId,
            UserUid = ack.UserUid,
            Passport = ack.Otp,
            CurrentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
        });

        if (ack.Ok == NetError.NET_OK)
            Fsm.TryStateTransition(KCHFSM.I_TO_AUTHENTICATED, out _);
        else
            ReserveDestroy();
    }

    private void OnSecurityAuthReq(KECH_SECURITY_AUTH_REQ req)
    {
        if (StateId != KCHFSM.S_INIT) return;

        if (_server.ChannelSimLayer.AuthFlag == AuthFlag.Internal)
        {
            Fsm.TryStateTransition(KCHFSM.I_TO_AUTHENTICATED, out _);
            SendPacket(EventId.ECH_SECURITY_AUTH_ACK, new KECH_SECURITY_AUTH_ACK { Ok = NetError.NET_OK });
            return;
        }

        _server.GetAuthProvider(Region.Region).RequestPublisherSecurityAuth(this, req);
    }

    private void OnPublisherSecurityInfoNot(KEPUBLISHER_SECURITY_INFO_NOT not)
    {
        if (StateId != KCHFSM.S_INIT) return;
        _server.GetAuthProvider(Region.Region).HandlePublisherSecurityInfoNot(this, not);
    }

    private void OnPublisherSecurityAuthAck(KEPUBLISHER_SECURITY_AUTH_ACK ack)
    {
        SendPacket(EventId.ECH_SECURITY_AUTH_ACK, new KECH_SECURITY_AUTH_ACK { Ok = ack.Ok });
    }

    private void OnGetChannelListReq()
    {
        if (StateId != KCHFSM.S_LOGINED && StateId != KCHFSM.S_WAIT_QUEUE)
        {
            KLog.Cerr($"ECH_GET_CHANNEL_LIST_REQ in invalid state {StateIdString} UID={Uid}");
            return;
        }

        SendPacket(EventId.ECH_GET_CHANNEL_LIST_ACK, new KPacketOK { Ok = NetError.NET_OK });

        var wq = _server.WaitQueueManager;
        switch (wq.Mode)
        {
            case WaitQueueMode.WQ_NONE:
                if (!PassedEnterChannel)
                {
                    PassedEnterChannel = true;
                    wq.RegPassedUser(Uid);
                }
                break;

            case WaitQueueMode.WQ_CCU_LIMIT:
                if (!PassedEnterChannel)
                {
                    PassedEnterChannel = wq.IsCanEnterChannel(_server.ServerGroupId);
                    if (!PassedEnterChannel)
                    {
                        wq.RegWaitUser(Uid);
                        wq.GetWaitUserCount(Uid, out int count);
                        SendPacket(EventId.ECH_WAIT_QUEUE_MESSAGE_NOT, new KECH_WAIT_QUEUE_MESSAGE_NOT { WaitUserCount = count });
                        Fsm.TryStateTransition(KCHFSM.I_TO_WAIT_QUEUE, out _);
                        return;
                    }
                    wq.RegPassedUser(Uid);
                }
                break;

            case WaitQueueMode.WQ_ENTRY_BAN:
                wq.RegWaitUser(Uid);
                wq.GetWaitUserCount(Uid, out int waitPos);
                SendPacket(EventId.ECH_WAIT_QUEUE_MESSAGE_NOT, new KECH_WAIT_QUEUE_MESSAGE_NOT { WaitUserCount = waitPos });
                Fsm.TryStateTransition(KCHFSM.I_TO_WAIT_QUEUE, out _);
                return;
        }

        SendChannelList();
    }

    private void OnGetServerGroupListReq()
    {
        if (StateId != KCHFSM.S_LOGINED) return;

        SendPacket(EventId.ECH_GET_SERVERGROUP_LIST_ACK, new KPacketOK { Ok = NetError.NET_OK });
        SendPacket(EventId.ECH_GET_SERVERGROUP_LIST_NOT, new KECH_GET_SERVERGROUP_LIST_NOT
        {
            ServerGroupList = _server.GetServerGroupListSnapshot(),
        });
    }

    private void OnGetServerTimeReq()
    {
        SendPacket(EventId.ECH_GET_SERVER_TIME_ACK, new KECH_GET_SERVER_TIME_ACK
        {
            CurrentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
        });
    }

    private void OnDisconnectReq()
    {
        SendID(EventId.ECH_DISCONNECT_ACK);
        ReserveDestroy();
    }

    private void OnPcBangIpAndMacNot(KECH_PCBANG_IP_AND_MAC_INFO_NOT not)
    {
        _server.Router.Send(PerformerTypeId, Uid, PerformerId.PI_CHANNEL_LOG_DB, 0,
            EventId.DBE_PCBANG_IP_AND_MAC_INFO_NOT, not, [AccountUid > 0 ? AccountUid : Uid, -1]);
    }

    private void OnToolCheckLoginReq(KE_TOOL_CHECK_LOGIN_REQ req)
    {
        if (_server.CcuToolManager.CheckLogin(req.Id, req.Password))
        {
            AuthLevel = 5;
            Fsm.TryStateTransition(KCHFSM.I_TO_AUTHENTICATED, out _);
            SendPacket(EventId.E_TOOL_CHECK_LOGIN_ACK, new KE_TOOL_CHECK_LOGIN_ACK
            {
                AuthLevel = 5,
            });
            return;
        }

        SendToAccountDB(EventId.DBE_GET_CONCURRENT_USER_LOGIN_REQ, new KDBE_GET_CONCURRENT_USER_LOGIN_REQ
        {
            Id = req.Id,
            Pw = req.Password,
        });
    }

    private void OnGetConcurrentUserLoginAck(KDBE_GET_CONCURRENT_USER_LOGIN_ACK ack)
    {
        AuthLevel = ack.AuthLevel;
        if (ack.Ok == NetError.NET_OK)
            Fsm.TryStateTransition(KCHFSM.I_TO_AUTHENTICATED, out _);

        SendPacket(EventId.E_TOOL_CHECK_LOGIN_ACK, new KE_TOOL_CHECK_LOGIN_ACK
        {
            AuthLevel = ack.AuthLevel,
        });
    }

    private void OnToolGetCcuInfoReq()
    {
        _server.Router.SendNoParam(PerformerTypeId, Uid, PerformerId.PI_CHANNEL_LOG_DB, 0,
            EventId.DBE_GET_CONCURRENT_USER_INFO_REQ, [Uid, -1]);
    }

    private void OnGetConcurrentUserInfoAck(KDBE_GET_CONCURRENT_USER_INFO_ACK ack)
    {
        SendPacket(EventId.E_TOOL_GET_CCU_INFO_ACK, new KE_TOOL_GET_CCU_INFO_ACK
        {
            Ok = ack.Ok,
            CcuType = ack.CcuType,
            ServerUid = ack.ServerUid,
            Date = ack.Date,
            CcuInfo = ack.CcuInfo,
        });
    }

    private void OnToolServerListReq()
    {
        _server.Router.SendNoParam(PerformerTypeId, Uid, PerformerId.PI_CHANNEL_LOG_DB, 0,
            EventId.DBE_GET_GAME_SERVER_LIST_REQ, [Uid, -1]);
    }

    private void OnGetGameServerListAck(KDBE_GET_GAME_SERVER_LIST_ACK ack)
    {
        SendPacket(EventId.E_TOOL_SERVER_LIST_ACK, new KE_TOOL_SERVER_LIST_ACK
        {
            ServerCount = ack.ServerCount,
            ServerList = ack.ServerList,
        });
    }

    private void OnCheckServerSnAck(KDBE_CHECK_SERVER_SN_ACK ack)
    {
        if (ack.Ok == NetError.ERR_SERVERSN_01 && KServerSNMaker.MakeServerSN(ack.ServerSN, out byte[] snBytes))
        {
            SendPacket(EventId.ECH_NEW_SERVER_SN_NOT, new KECH_NEW_SERVER_SN_NOT { ServerSN = snBytes });
        }
        else if (ack.Ok == NetError.ERR_SERVERSN_02)
        {
            SendToAccountDB(EventId.DBE_CREATE_SERVER_SN_REQ, new KDBE_CREATE_SERVER_SN_REQ
            {
                MachineId = _securityManager.MachineIdString,
            });
            return;
        }
        else if (ack.Ok != NetError.NET_OK)
        {
            SendPacket(EventId.ECH_VERIFY_ACCOUNT_ACK, new KECH_VERIFY_ACCOUNT_ACK { Ok = NetError.ERR_VERIFY_17 });
            ReserveDestroy();
            return;
        }

        _server.HackingProtectionManager.GetHackingToolList(out int rk, out var tools);
        SendPacket(EventId.ECH_HACKING_TOOL_LIST_REQ, new KECH_HACKING_TOOL_LIST_REQ
        {
            RandomKey = rk,
            HackingToolList = tools,
        });
    }

    private void OnCreateServerSnAck(KDBE_CREATE_SERVER_SN_ACK ack)
    {
        if ((ack.Ok == NetError.NET_OK || ack.Ok == NetError.ERR_SERVERSN_05) &&
            KServerSNMaker.MakeServerSN(ack.ServerSN, out byte[] snBytes))
        {
            SendPacket(EventId.ECH_NEW_SERVER_SN_NOT, new KECH_NEW_SERVER_SN_NOT { ServerSN = snBytes });
        }
        _server.HackingProtectionManager.GetHackingToolList(out int rk, out var tools);
        SendPacket(EventId.ECH_HACKING_TOOL_LIST_REQ, new KECH_HACKING_TOOL_LIST_REQ
        {
            RandomKey = rk,
            HackingToolList = tools,
        });
    }

    private void OnHackingToolListAck(KECH_HACKING_TOOL_LIST_ACK ack)
    {
        if (ack.Ok != NetError.NET_OK) return;

        if (string.IsNullOrEmpty(UserId))
        {
            SendPacket(EventId.ECH_VERIFY_ACCOUNT_ACK, new KECH_VERIFY_ACCOUNT_ACK
            {
                Ok = ack.Ok,
                ChannelRandomKey = -58566,
                UseVerify = true,
            });
            return;
        }

        SendToAccountDB(EventId.DBE_UPDATE_CHANNEL_RANDOMKEY_REQ, new KDBE_UPDATE_CHANNEL_RANDOMKEY_REQ
        {
            UserId = UserId,
        });
    }

    private void OnUpdateChannelRandomKeyAck(KDBE_UPDATE_CHANNEL_RANDOMKEY_ACK ack)
    {
        SendPacket(EventId.ECH_VERIFY_ACCOUNT_ACK, new KECH_VERIFY_ACCOUNT_ACK
        {
            Ok = ack.Ok,
            ChannelRandomKey = ack.RandomKey,
            UseVerify = true,
        });
    }

    private void SendToAccountDB<T>(EventId eventId, T payload)
        => _server.Router.Send(PerformerTypeId, Uid, PerformerId.PI_CHANNEL_ACCOUNT_DB, 0, eventId, payload, [Uid, -1]);

    private void ProcessLoginResultCount(int netErr)
    {
        var type = netErr switch
        {
            NetError.NET_OK => LoginResultType.Success,
            NetError.ERR_VERIFY_21 or NetError.ERR_GIANT_VERIFY_07 => LoginResultType.FailSystemError,
            NetError.ERR_VERIFY_02 => LoginResultType.FailAccountNotExist,
            NetError.ERR_VERIFY_03 => LoginResultType.FailPasswordError,
            NetError.ERR_VERIFY_04 => LoginResultType.FailAccountBlock,
            _ => LoginResultType.FailUnknown,
        };
        _server.Statistics.AddLoginResult(type);
    }

    private static bool ContainsInvalidSqlChars(string s)
    {
        foreach (char c in s)
        {
            if (c is '\'' or '"' or ';' or '\\' or '\0')
                return true;
        }
        return false;
    }
}
