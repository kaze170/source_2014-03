using KncWX2Server.ChannelServer;
using KncWX2Server.Core.Database;
using KncWX2Server.Core.Events;
using KncWX2Server.Core.Framework;
using KncWX2Server.Core.Logging;
using KncWX2Server.Core.Packets;
using KncWX2Server.Core.Region;

namespace KncWX2Server.ChannelServer.DB;

/// <summary>
/// Port of ChannelServer/ChannelLogDBThread.{h,cpp} backed by SqliteDatabaseManager.
/// </summary>
public sealed class KChannelLogDBThread : KDBThread
{
    private readonly Func<KChannelServer> _getServer;

    public override uint PerformerTypeId => PerformerId.PI_CHANNEL_LOG_DB;

    public KChannelLogDBThread(SqliteDatabaseManager db, InProcessRouter router, Func<KChannelServer> getServer)
        : base(db, router)
    {
        _getServer = getServer;
    }

    protected override void ProcessEvent(KEvent ev)
    {
        var server = _getServer();
        var region = server.Region;

        switch (ev.EventId)
        {
            case EventId.DBE_SERVER_ON_REQ:
            {
                var netAddr = ev.GetPayload<KNetAddress>(region);
                var info = Db.GetServerInfo(region.Code, netAddr.Ip, serverClass: 4, serverGroupId: server.ServerGroupId)
                    ?? new KServerInfo
                    {
                        Uid = KncUid.MakeUid(region, server.ServerGroupId, 1000 + server.ServerGroupId),
                        Name = $"{region.Code}_ChannelServer_{server.ServerGroupId}",
                        ServerGroupId = server.ServerGroupId,
                        ServerClass = (int)ServerClass.Channel,
                        Ip = netAddr.Ip,
                        PublicIp = netAddr.Ip,
                        PrivateIp = netAddr.Ip,
                        MasterPort = netAddr.Port,
                        NcUdpPort = netAddr.Port,
                        MaxUser = 3000,
                        On = true,
                    };

                var ack = new KDBE_CHANNEL_SERVER_ON_ACK
                {
                    Ok = NetError.NET_OK,
                    ServerInfo = info,
                    ChannelList = Db.GetChannelList(region.Code, server.ServerGroupId),
                    ServerGroupList = Db.GetServerGroupList(region.Code),
                };
                SendToServer(EventId.DBE_SERVER_ON_ACK, ack, ev.Trace);
                break;
            }

            case EventId.DBE_SERVER_OFF_NOT:
            {
                Db.UpdateChannelConcurrentUser(region.Code, server.Uid, 0);
                break;
            }

            case EventId.DBE_CHANNEL_LIST_REQ:
            {
                var req = ev.GetPayload<KDBE_CHANNEL_LIST_REQ>(region);
                var ack = new KDBE_CHANNEL_LIST_ACK
                {
                    ChannelList = Db.GetChannelList(region.Code, req.ServerGroupId),
                };
                SendToServer(EventId.DBE_CHANNEL_LIST_ACK, ack, ev.Trace);
                break;
            }

            case EventId.DBE_GET_CONCURRENT_USER_INFO_REQ:
            {
                var channels = Db.GetChannelList(region.Code, server.ServerGroupId);
                var ccuList = channels.Values.Select(c => new KCCUInfo
                {
                    CcuTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    CcuCount = c.CurrentUser,
                }).ToList();
                var ack = new KDBE_GET_CONCURRENT_USER_INFO_ACK
                {
                    Ok = NetError.NET_OK,
                    CcuInfo = ccuList,
                };
                SendToUser(ev.Trace0, EventId.DBE_GET_CONCURRENT_USER_INFO_ACK, ack, ev.Trace);
                break;
            }

            case EventId.DBE_CHECK_HACKING_TOOL_LIST_REQ:
            {
                var tools = Db.GetHackingToolList();
                SendToServer(EventId.DBE_CHECK_HACKING_TOOL_LIST_ACK, new KDBE_CHECK_HACKING_TOOL_LIST_ACK
                {
                    HackingToolList = tools,
                }, ev.Trace);
                break;
            }

            case EventId.DBE_GET_GAME_SERVER_LIST_REQ:
            {
                var servers = Db.GetServerList(region.Code, serverClass: 3);
                var toolServers = servers.Select(s => new KToolServerList
                {
                    ServerUid = unchecked((int)s.Uid),
                    ServerSetId = s.ServerGroupId,
                    ServerType = s.ServerClass,
                    ChannelId = 0,
                    ChannelName = s.Name,
                    Ip = s.PublicIp,
                    Port = s.MasterPort,
                }).ToList();
                SendToUser(ev.Trace0, EventId.DBE_GET_GAME_SERVER_LIST_ACK, new KDBE_GET_GAME_SERVER_LIST_ACK
                {
                    ServerCount = toolServers.Count,
                    ServerList = toolServers,
                }, ev.Trace);
                break;
            }

            case EventId.DBE_GET_TOTAL_CCU_FOR_WAIT_QUEUE_REQ:
            {
                int totalCcu = Db.GetTotalConcurrentUsers(region.Code, server.ServerGroupId);
                SendToServer(EventId.DBE_GET_TOTAL_CCU_FOR_WAIT_QUEUE_ACK, new KDBE_GET_TOTAL_CCU_FOR_WAIT_QUEUE_ACK
                {
                    TotalCcu = totalCcu,
                }, ev.Trace);
                break;
            }

            case EventId.DBE_TICK_PERFORMANCE_LOG_NOT:
            {
                var not = ev.GetPayload<KDBE_TICK_PERFORMANCE_LOG_NOT>(region);
                Db.InsertPerformanceLog(not.ServerUid, (int)not.MaxTickCount, 0);
                break;
            }

            case EventId.DBE_LOG_STATISTICS_INFO_NOT:
            {
                var not = ev.GetPayload<KDBE_LOG_STATISTICS_INFO_NOT>(region);
                Db.InsertErrorStatistics(not.ServerUid, not.LogStats);
                break;
            }

            case EventId.DBE_SERVERGROUP_LIST_REQ:
            {
                var groups = Db.GetServerGroupList(region.Code);
                SendToServer(EventId.DBE_SERVERGROUP_LIST_ACK, new KDBE_SERVERGROUP_LIST_ACK
                {
                    ServerGroupList = groups,
                }, ev.Trace);
                break;
            }

            case EventId.DBE_INSERT_LOGIN_RESULT_INFO_REQ:
            {
                var not = ev.GetPayload<KDBE_INSERT_LOGIN_RESULT_INFO_REQ>(region);
                Db.InsertLoginStats(region.Code, server.Uid, not.ResultCount);
                break;
            }

            case EventId.DBE_PCBANG_IP_AND_MAC_INFO_NOT:
            {
                var not = ev.GetPayload<KECH_PCBANG_IP_AND_MAC_INFO_NOT>(region);
                Db.InsertPCBangAccessLog(ev.Trace0, not.PublicIp, not.MacInfo);
                break;
            }

            default:
                KLog.Cwarn($"[KChannelLogDBThread] Unhandled DB event: {ev.EventId} ({ev.EventIdValue})");
                break;
        }
    }
}

/// <summary>
/// Port of ChannelServer/ChannelSMSDBThread.{h,cpp} backed by SqliteDatabaseManager.
/// </summary>
public sealed class KChannelSMSDBThread : KDBThread
{
    private readonly Func<RegionProfile> _getRegion;

    public override uint PerformerTypeId => PerformerId.PI_CHANNEL_SMS_DB;

    public KChannelSMSDBThread(SqliteDatabaseManager db, InProcessRouter router, Func<RegionProfile> getRegion)
        : base(db, router)
    {
        _getRegion = getRegion;
    }

    protected override void ProcessEvent(KEvent ev)
    {
        if (ev.EventId == EventId.DBE_SEND_PHONE_MSG_NOT)
        {
            var not = ev.GetPayload<KDBE_SEND_PHONE_MSG_NOT>(_getRegion());
            foreach (string phone in not.PhoneNumbers)
            {
                Db.InsertSmsMessage(phone, not.SmsMessage);
            }
        }
    }
}
