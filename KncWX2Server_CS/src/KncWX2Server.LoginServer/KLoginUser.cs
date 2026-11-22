using KncWX2Server.Core.Events;
using KncWX2Server.Core.Framework;
using KncWX2Server.Core.Logging;
using KncWX2Server.Core.Network;
using KncWX2Server.Core.Packets;
using KncWX2Server.Core.Region;
using KncWX2Server.Core.Serialization;

namespace KncWX2Server.LoginServer;

public sealed class KLoginUser : KActor
{
    private readonly KLoginServer _server;
    private readonly KRepeatEventFilter _repeatFilter = new();

    public string UserId { get; private set; } = string.Empty;
    public long AccountUid { get; private set; }
    public int AuthLevel { get; private set; }
    public int SelectedServerGroupId { get; private set; }
    public bool SecondSecurityVerified { get; private set; }

    public override uint PerformerTypeId => PerformerId.PI_LOGIN_USER;

    public KLoginUser(KTcpSession session, KLoginServer server)
        : base(session, new KLoginFSM(), server.Region)
    {
        _server = server;
        _repeatFilter.RegRepeatEvent(EventId.EGS_NEW_USER_JOIN_REQ, EventId.EGS_NEW_USER_JOIN_ACK);
        _repeatFilter.RegRepeatEvent(EventId.EGS_VERIFY_ACCOUNT_REQ, EventId.EGS_VERIFY_ACCOUNT_ACK);
        _repeatFilter.RegRepeatEvent(EventId.EGS_AUTH_SECOND_SECURITY_REQ, EventId.EGS_AUTH_SECOND_SECURITY_ACK);
    }

    public new bool SendPacket(EventId eventId, IKSerializable packet)
    {
        _repeatFilter.CheckAckEvent((ushort)eventId);
        return base.SendPacket(eventId, packet);
    }

    public override void OnDestroy()
    {
        if (IsDestroyed) return;
        base.OnDestroy();
        Fsm.TryStateTransition(KLoginFSM.I_TO_EXIT, out _);
        _server.OnUserDisconnected(this);
    }

    protected override void ProcessEvent(KEvent ev)
    {
        if (ev.FromType == EventFromType.Client && !_repeatFilter.CheckReqEvent(ev.EventIdValue))
            return;

        switch (ev.EventId)
        {
            case EventId.E_HEART_BEAT:
                break;

            case EventId.EGS_CONNECT_REQ:
            {
                if (Fsm.CurrentState != KLoginFSM.S_INIT) break;
                var req = ev.GetPayload<KEGS_CONNECT_REQ>(Region);
                KLog.Clog($"KLoginUser[{Uid}] connect version={req.Version}");
                SendPacket(EventId.EGS_CONNECT_ACK, new KEGS_CONNECT_ACK
                {
                    Ok = NetError.NET_OK,
                    UdpPort = Region.DefaultUdpEchoPort,
                    ChannelId = 1,
                });
                break;
            }

            case EventId.EGS_NEW_USER_JOIN_REQ:
            {
                var req = ev.GetPayload<KEGS_NEW_USER_JOIN_REQ>(Region);
                var (ok, uid, _) = _server.Db.CreateAccount(Region, req.Id, req.Password);
                SendPacket(EventId.EGS_NEW_USER_JOIN_ACK, new KEGS_NEW_USER_JOIN_ACK
                {
                    Ok = ok ? NetError.NET_OK : Region.ResolveNetError(NetErrorKey.DuplicateId),
                    UserUid = uid,
                });
                break;
            }

            case EventId.EGS_VERIFY_ACCOUNT_REQ:
            {
                var req = ev.GetPayload<KUserAuthenticateReq>(Region);
                if (_server.AccountBlockManager.IsBlocked(req.UserId, KAccountBlockManager.BlockType.Connect, out _))
                {
                    SendPacket(EventId.EGS_VERIFY_ACCOUNT_ACK, new KEGS_VERIFY_ACCOUNT_ACK
                    {
                        Ok = NetError.ERR_VERIFY_11,
                        ClientIp = IpStr,
                    });
                    break;
                }

                var (netErr, userUid, authLevel, isGuest, _) = _server.Db.VerifyOrRegisterAccount(
                    Region.Code, req.UserId, req.Password);

                if (netErr == NetError.NET_OK && userUid > 0)
                {
                    AccountUid = userUid;
                    UserId = req.UserId;
                    AuthLevel = authLevel;

                    bool useSecondPw = false;
                    if (!Region.DisableSecondSecurity)
                    {
                        var sec = _server.Db.GetSecondSecurityStatus(userUid);
                        useSecondPw = sec.useSecondPw;
                    }

                    Fsm.TryStateTransition(useSecondPw ? KLoginFSM.I_TO_CHECK_SECOND_PW : KLoginFSM.I_TO_LOGINED, out _);
                    SecondSecurityVerified = !useSecondPw;
                    _server.RegisterAuthenticatedUser(this);

                    SendPacket(EventId.EGS_VERIFY_ACCOUNT_ACK, new KEGS_VERIFY_ACCOUNT_ACK
                    {
                        Ok = NetError.NET_OK,
                        ClientIp = IpStr,
                        ChannelCode = req.ChannelingCode > 0 ? (byte)req.ChannelingCode : (byte)Region.DefaultChannelingCode,
                        ChannelUserId = req.UserId,
                        AccountInfo = new KAccountInfo
                        {
                            UserUid = userUid,
                            Id = req.UserId,
                            Name = req.UserId,
                            AuthLevel = authLevel,
                            IsGuestUser = isGuest,
                            LastLogin = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                        },
                    });

                    if (!Region.DisableSecondSecurity)
                    {
                        SendPacket(EventId.EGS_SECOND_SECURITY_INFO_NOT, new KEGS_SECOND_SECURITY_INFO_NOT
                        {
                            Ok = NetError.NET_OK,
                            UseSecondPw = useSecondPw,
                        });
                    }
                }
                else
                {
                    SendPacket(EventId.EGS_VERIFY_ACCOUNT_ACK, new KEGS_VERIFY_ACCOUNT_ACK
                    {
                        Ok = netErr,
                        ClientIp = IpStr,
                    });
                }
                break;
            }

            case EventId.EGS_CREATE_SECOND_SECURITY_REQ:
            {
                if (AccountUid <= 0) break;
                var req = ev.GetPayload<KEGS_CREATE_SECOND_SECURITY_REQ>(Region);
                _server.Db.CreateSecondSecurityPin(AccountUid, req.SecondPw);
                SecondSecurityVerified = true;
                SendPacket(EventId.EGS_CREATE_SECOND_SECURITY_ACK, new KPacketOK { Ok = NetError.NET_OK });
                break;
            }

            case EventId.EGS_AUTH_SECOND_SECURITY_REQ:
            {
                if (AccountUid <= 0) break;
                var req = ev.GetPayload<KEGS_AUTH_SECOND_SECURITY_REQ>(Region);
                var (ok, failedCount) = _server.Db.AuthSecondSecurityPin(AccountUid, req.SecondPw);
                if (ok)
                {
                    SecondSecurityVerified = true;
                    Fsm.TryStateTransition(KLoginFSM.I_TO_LOGINED, out _);
                }
                SendPacket(EventId.EGS_AUTH_SECOND_SECURITY_ACK, new KEGS_AUTH_SECOND_SECURITY_ACK
                {
                    Ok = ok ? NetError.NET_OK : NetError.ERR_VERIFY_14,
                    FailedCount = failedCount,
                });
                break;
            }

            case EventId.EGS_GET_SERVER_SET_DATA_REQ:
            {
                var groups = _server.Db.GetServerGroupList(Region.Code);
                var servers = _server.Db.GetServerList(Region.Code);
                var ack = new KEGS_GET_SERVER_SET_DATA_ACK { Ok = NetError.NET_OK };
                foreach (var g in groups.OrderBy(k => k.Key))
                {
                    int maxCcu = 0, ccu = 0;
                    foreach (var s in servers.Where(sv => sv.ServerGroupId == g.Key && sv.ServerClass == 1))
                    {
                        maxCcu += s.MaxUser;
                        ccu += s.CurrentUser;
                    }
                    if (maxCcu <= 0) maxCcu = 1;
                    sbyte level = ccu < 1500 ? KServerSetData.UclFree
                        : ccu < 3000 ? KServerSetData.UclNormal
                        : ccu < maxCcu ? KServerSetData.UclBusy
                        : KServerSetData.UclFull;
                    ack.ServerSetList.Add(new KServerSetData
                    {
                        ServerSetId = g.Key,
                        ServerSetName = g.Value.ServerGroupName,
                        UserCountLevel = level,
                    });
                }
                SendPacket(EventId.EGS_GET_SERVER_SET_DATA_ACK, ack);
                break;
            }

            case EventId.ECH_GET_CHANNEL_LIST_REQ:
            {
                var channels = _server.Db.GetChannelList(Region.Code);
                var ack = new KECH_GET_CHANNEL_LIST_NOT();
                foreach (var kv in channels)
                {
                    var ch = kv.Value;
                    ack.ChannelList[ch.ChannelId] = new KChannelInfo
                    {
                        ServerUid = ch.ServerUid,
                        ChannelId = ch.ChannelId,
                        ServerGroupId = ch.ServerGroupId,
                        ChannelName = ch.ChannelName,
                        MaxUser = ch.MaxUser,
                        CurrentUser = ch.CurrentUser,
                        Ip = ch.Ip,
                        MasterPort = Region.DefaultGamePort,
                    };
                }
                SendPacket(EventId.ECH_GET_CHANNEL_LIST_NOT, ack);
                break;
            }

            case EventId.EGS_SELECT_SERVER_SET_REQ:
            {
                var req = ev.GetPayload<KEGS_SELECT_SERVER_SET_REQ>(Region);
                if (!string.IsNullOrEmpty(UserId) && _server.Db.IsAccountBlocked(Region.Code, UserId, out string blockReason))
                {
                    SendPacket(EventId.EGS_SELECT_SERVER_SET_ACK, new KEGS_SELECT_SERVER_SET_ACK
                    {
                        Ok = NetError.ERR_VERIFY_11,
                        AccountBlockInfo = new KAccountBlockInfo
                        {
                            BlockReason = blockReason,
                            EndTime = string.Empty,
                        },
                    });
                    ReserveDestroy();
                    break;
                }

                SelectedServerGroupId = req.ServerSetId;
                Fsm.TryStateTransition(KLoginFSM.I_TO_SERVER_SELECT, out _);

                var channels = _server.Db.GetChannelList(Region.Code).Values
                    .Where(c => c.ServerGroupId == req.ServerSetId)
                    .OrderBy(c => c.CurrentUser)
                    .ToList();
                var chosen = channels.FirstOrDefault();
                string ip = chosen != null && !string.IsNullOrWhiteSpace(chosen.Ip) ? chosen.Ip : "127.0.0.1";
                ushort port = chosen != null && chosen.MasterPort > 0 ? chosen.MasterPort : Region.DefaultGamePort;

                SendPacket(EventId.EGS_SELECT_SERVER_SET_ACK, new KEGS_SELECT_SERVER_SET_ACK
                {
                    Ok = NetError.NET_OK,
                    Ip = ip,
                    Port = port,
                });
                break;
            }

            case EventId.EGS_STATE_CHANGE_SERVER_SELECT_REQ:
            {
                Fsm.TryStateTransition(KLoginFSM.I_TO_SERVER_SELECT, out _);
                SendPacket(EventId.EGS_STATE_CHANGE_SERVER_SELECT_ACK, new KEGS_STATE_CHANGE_SERVER_SELECT_ACK
                {
                    Ok = NetError.NET_OK,
                });
                break;
            }

            case EventId.EGS_CHARACTER_LIST_REQ:
            {
                if (AccountUid <= 0) break;
                var units = _server.Db.GetUnitsForAccountByServerGroup(Region, AccountUid, SelectedServerGroupId);
                var ack = new KEGS_CHARACTER_LIST_ACK
                {
                    Ok = NetError.NET_OK,
                    UserId = UserId,
                };
                ack.ServerGroupUnitSlot[SelectedServerGroupId] = 8;
                ack.ServerGroupUnitInfo[SelectedServerGroupId] = units;
                SendPacket(EventId.EGS_CHARACTER_LIST_ACK, ack);
                break;
            }

            case EventId.EGS_CREATE_UNIT_REQ:
            {
                if (AccountUid <= 0) break;
                var req = ev.GetPayload<KEGS_CREATE_UNIT_REQ>(Region);
                if (req.UnitClass <= 0 || req.UnitClass >= 100)
                {
                    SendPacket(EventId.EGS_CREATE_UNIT_ACK, new KEGS_CREATE_UNIT_ACK { Ok = NetError.ERR_CREATE_UNIT_03 });
                    break;
                }
                string nick = req.NickName?.Trim() ?? string.Empty;
                if (nick.Length < 2 || nick.Length > 12 || nick.Any(char.IsWhiteSpace))
                {
                    SendPacket(EventId.EGS_CREATE_UNIT_ACK, new KEGS_CREATE_UNIT_ACK { Ok = NetError.ERR_CREATE_UNIT_04 });
                    break;
                }
                var (ok, unit, err) = _server.Db.CreateUnitInServerGroup(
                    Region, AccountUid, nick, req.UnitClass, SelectedServerGroupId);
                SendPacket(EventId.EGS_CREATE_UNIT_ACK, new KEGS_CREATE_UNIT_ACK
                {
                    Ok = ok ? NetError.NET_OK : err,
                    UnitInfo = unit ?? new KUnitInfo(),
                });
                break;
            }

            case EventId.EGS_DELETE_UNIT_REQ:
            {
                if (AccountUid <= 0) break;
                var req = ev.GetPayload<KEGS_DELETE_UNIT_REQ>(Region);
                var existing = _server.Db.GetUnitByUid(Region, req.UnitUid);
                if (existing == null || existing.OwnerUserUid != AccountUid)
                {
                    SendPacket(EventId.EGS_DELETE_UNIT_ACK, new KEGS_DELETE_UNIT_ACK
                    {
                        Ok = NetError.ERR_DELETE_UNIT_00,
                        UnitUid = req.UnitUid,
                    });
                    break;
                }
                if (existing.UserGuildInfo.GuildUid > 0)
                {
                    SendPacket(EventId.EGS_DELETE_UNIT_ACK, new KEGS_DELETE_UNIT_ACK
                    {
                        Ok = NetError.ERR_DELETE_UNIT_02,
                        UnitUid = req.UnitUid,
                    });
                    break;
                }
                bool ok = _server.Db.DeleteUnit(Region, AccountUid, req.UnitUid);
                SendPacket(EventId.EGS_DELETE_UNIT_ACK, new KEGS_DELETE_UNIT_ACK
                {
                    Ok = ok ? NetError.NET_OK : NetError.ERR_DELETE_UNIT_00,
                    UnitUid = req.UnitUid,
                    GuildUid = existing.UserGuildInfo.GuildUid,
                    NickName = existing.NickName,
                    Level = existing.Level,
                    UnitClass = existing.UnitClass,
                });
                break;
            }

            case EventId.EGS_CHECK_NICK_NAME_REQ:
            {
                var req = ev.GetPayload<KEGS_CHECK_NICK_NAME_REQ>(Region);
                string nick = req.NickName?.Trim() ?? string.Empty;
                if (nick.Length < 2 || nick.Length > 12 || nick.Any(char.IsWhiteSpace))
                {
                    SendPacket(EventId.EGS_CHECK_NICK_NAME_ACK, new KEGS_CHECK_NICK_NAME_ACK
                    {
                        Ok = NetError.ERR_CREATE_UNIT_04,
                        NickName = req.NickName,
                    });
                    break;
                }
                bool taken = _server.Db.GetUnitByNickName(Region, nick) != null;
                SendPacket(EventId.EGS_CHECK_NICK_NAME_ACK, new KEGS_CHECK_NICK_NAME_ACK
                {
                    Ok = taken ? NetError.ERR_CREATE_UNIT_02 : NetError.NET_OK,
                    NickName = nick,
                });
                break;
            }

            case EventId.EGS_CHECK_SECOND_SECURITY_PW_CHRACTER_LIST_REQ:
            {
                if (AccountUid <= 0) break;
                var req = ev.GetPayload<KPacketString>(Region);
                var (ok, _) = _server.Db.AuthSecondSecurityPin(AccountUid, req.Value);
                if (ok) SecondSecurityVerified = true;
                SendPacket(EventId.EGS_CHECK_SECOND_SECURITY_PW_CHRACTER_LIST_ACK, new KPacketOK
                {
                    Ok = ok ? NetError.NET_OK : NetError.ERR_SECOND_SECURITY_AUTH_FAILED,
                });
                break;
            }

            case EventId.EGS_CLIENT_QUIT_REQ:
            {
                var req = ev.GetPayload<KEGS_CLIENT_QUIT_REQ>(Region);
                KLog.Clog($"KLoginUser[{Uid}] client quit reason={req.Reason} errs={req.ErrorMsgInfoList.Count}");
                SendPacket(EventId.EGS_CLIENT_QUIT_ACK, new KPacketOK { Ok = NetError.NET_OK });
                ReserveDestroy();
                break;
            }

            case EventId.E_CHECK_DDOS_GUARD_REQ:
            {
                var evAck = new KEvent();
                evAck.DestPerformer.PerformerId = PerformerId.PI_GS_PROXY;
                evAck.DestPerformer.AddUID(Uid);
                evAck.EventId = EventId.E_CHECK_DDOS_GUARD_ACK;
                var ks = new KSerializer(Region);
                ks.BeginWriting(evAck.Buffer);
                ks.Put((sbyte)0);
                ks.EndWriting();
                _ = Session.SendEventAsync(evAck);
                break;
            }

            case EventId.E_RESERVE_DESTROY:
                ReserveDestroy();
                break;

            default:
                KLog.Clog($"KLoginUser[{Uid}] unhandled event {ev.EventId} ({ev.EventIdValue})");
                break;
        }
    }
}
