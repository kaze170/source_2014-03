using System.Collections.Concurrent;
using System.Diagnostics;
using KncWX2Server.Core.Database;
using KncWX2Server.Core.Events;
using KncWX2Server.Core.Framework;
using KncWX2Server.Core.Logging;
using KncWX2Server.Core.Network;
using KncWX2Server.Core.Packets;
using KncWX2Server.Core.Region;
using KncWX2Server.Core.Scripting;

namespace KncWX2Server.LoginServer;

public sealed class KLoginSimLayer : KSimLayer { }

public sealed class KLoginNetLayer : KNetLayer
{
    private readonly KLoginServer _server;

    public KLoginNetLayer(KLoginServer server)
    {
        _server = server;
        Port = server.Region.DefaultLoginPort;
    }

    public override bool Init()
        => TcpServer.Start(Port, _server.AcceptSessionAsync, () => _server.Region);
}

/// <summary>
/// Managed port of KLoginServer (LoginServer/*.{h,cpp}).
/// Integrates AccountBlockManager, KLoginGuildManager, KLoginRankingManager, KLoginUserList,
/// duplicate-login enforcement, CCU logging, and in-process proxied login events.
/// </summary>
public sealed class KLoginServer : KBaseServer
{
    private readonly KLoginSimLayer _simLayer = new();
    private readonly KLoginNetLayer _netLayer;
    private readonly ConcurrentDictionary<long, KLoginUser> _authedUsers = new();
    private readonly ConcurrentDictionary<sbyte, int> _disconnectReports = new();
    private CancellationTokenSource? _tickCts;
    private Task? _tickTask;
    private DateTime _lastCcuLogUtc = DateTime.UtcNow;

    public KAccountBlockManager AccountBlockManager { get; } = new();
    public KLoginGuildManager GuildManager { get; } = new();
    public KGuildBoardManager GuildBoardManager { get; } = new();
    public KWeddingHallManager WeddingHallManager { get; } = new();
    public KRandomItemEventManager RandomItemEventManager { get; } = new();
    public KPersonalShopAgencyManager PersonalShopAgencyManager { get; } = new();
    private readonly ConcurrentDictionary<long, string> _pcBangActiveIps = new();
    public ConcurrentDictionary<EventId, long> LoginLogCounters { get; } = new();
    public KHenirRankingManager HenirRankingManager { get; } = new();
    public KLoginRankingManager RankingManager { get; } = new();
    public KLoginLocalRankingManager LocalRankingManager { get; } = new();
    public KLoginBuffManager BuffManager { get; } = new();
    public KServerCCUManager ServerCcuManager { get; } = new();
    public KLoginUserList UserList { get; } = new();
    public KGameSysValTable GameSysValTable { get; } = new();
    public KScriptRefreshManager ScriptRefreshManager { get; }

    public SqliteDatabaseManager Db => DbLayer.Sqlite;
    public override uint PerformerTypeId => PerformerId.PI_LOGIN_SERVER;
    public override KSimLayer SimLayer => _simLayer;
    public override KNetLayer NetLayer => _netLayer;
    public ushort ListenPort => _netLayer.Port;
    public bool IsRunning { get; private set; }
    public int ConnectedUserCount => _simLayer.ActorManager.Count;
    public int AuthenticatedUserCount => _authedUsers.Count;

    public KLoginServer(
        RegionCode initialRegion = RegionCode.TW,
        string dbPath = "KncWX2Server.db",
        InProcessRouter? sharedRouter = null)
        : base(sharedRouter)
    {
        Region = RegionProfile.FromCode(initialRegion);
        _simLayer.NationFlag = Region.DefaultNationFlag;
        _netLayer = new KLoginNetLayer(this);
        DbLayer.Initialize(dbPath);
        ScriptRefreshManager = new KScriptRefreshManager(Router, PerformerId.PI_LOGIN_SERVER, Uid);
        ScriptRefreshManager.SetDefaultReloadHandler(_ => LoadServerResources());
    }

    public bool Start(ushort? overridePort = null, int serverGroupId = 0)
    {
        if (IsRunning) return true;
        ServerGroupId = serverGroupId;
        Uid = KncUid.MakeUid(Region, serverGroupId, 100);
        Name = "KLoginServer";

        _netLayer.Port = overridePort ?? Region.DefaultLoginPort;
        Router.RegisterPerformer(PerformerId.PI_LOGIN_SERVER, this);
        Router.RegisterRoute(PerformerId.PI_LOGIN_USER, ev =>
        {
            foreach (long uid in ev.DestPerformer.Uids)
                _simLayer.ActorManager.QueueingTo(uid, ev.Clone());
            return true;
        });

        AccountBlockManager.RefreshNow(Db, Region);
        RankingManager.RefreshNow(Db, Region);
        HenirRankingManager.InitFromDb(Db.GetHenirRankings().Select(r => new KHenirRankingManager.HenirRecord
        {
            UnitUid = r.unitUid,
            NickName = r.nickName,
            UnitClass = r.unitClass,
            Level = r.level,
            StageCount = r.stageCount,
            ClearTimeSeconds = r.clearTimeSec,
            RecordedUtc = r.recordedUtc,
        }));

        RegToLua();
        if (!_netLayer.Init()) return false;
        _netLayer.Port = _netLayer.TcpServer.Port;

        IsRunning = true;
        ServerReady = true;
        ScriptRefreshManager.Start();
        _tickCts = new CancellationTokenSource();
        _tickTask = Task.Run(() => TickLoopAsync(_tickCts.Token));
        KLog.Cout($"[{Name}] Started on :{ListenPort} (Region={Region.Code}, UID={Uid})");
        return true;
    }

    public override void RegToLua()
    {
        base.RegToLua();
        ScriptManager.RegisterGlobal("GameSysVal", GameSysValTable);
        ScriptManager.RegisterGlobal("GuildBoardManager", GuildBoardManager);
        ScriptManager.RegisterGlobal("RankingManager", RankingManager);
        ScriptRefreshManager.BindToLua(ScriptManager, "ScriptRefresh");
    }

    public int LoadServerResources(string? resourceBaseDir = null)
    {
        var script = new KScriptManager();
        GameSysValTable.BindToLua(script, "GameSysVal");
        RankingManager.BindToLua(script, "RankingManager");
        ScriptRefreshManager.BindToLua(script, "ScriptRefresh");
        int loaded = 0;
        foreach (string f in new[] { "GameSysValTable.lua", "RankingData.lua" })
        {
            if (script.DoServerResourceFile(f, Region, resourceBaseDir))
                loaded++;
        }
        return loaded;
    }

    public Task AcceptSessionAsync(KTcpSession session)
    {
        var user = new KLoginUser(session, this);
        session.OnEventReceived += ev => user.QueueingEvent(ev);
        session.OnDisconnected += _ => user.OnDestroy();
        if (!_simLayer.ActorManager.Add(user))
        {
            session.ReserveDestroy();
            return Task.CompletedTask;
        }
        return session.StartAsync();
    }

    internal void RegisterAuthenticatedUser(KLoginUser user)
    {
        if (user.AccountUid <= 0) return;

        var (wasDup, prev) = UserList.RegisterOnline(user.AccountUid, user.UserId, user.Uid);
        if (wasDup && prev != null && _authedUsers.TryGetValue(user.AccountUid, out var oldActor) && oldActor.Uid != user.Uid)
        {
            oldActor.SendPacket(EventId.E_RESERVE_DESTROY, new KPacketOK { Ok = NetError.NET_OK });
            oldActor.OnDestroy();
        }
        _authedUsers[user.AccountUid] = user;
    }

    internal void OnUserDisconnected(KLoginUser user)
    {
        if (user.AccountUid > 0)
        {
            UserList.UnregisterOnline(user.AccountUid, user.Uid);
            if (_authedUsers.TryGetValue(user.AccountUid, out var cur) && cur.Uid == user.Uid)
                _authedUsers.TryRemove(user.AccountUid, out _);
        }
    }

    public void SwitchRegion(RegionCode region)
    {
        Region = RegionProfile.FromCode(region);
        _simLayer.NationFlag = Region.DefaultNationFlag;
        AccountBlockManager.RefreshNow(Db, Region);
        RankingManager.RefreshNow(Db, Region);
    }

    private async Task TickLoopAsync(CancellationToken ct)
    {
        var sw = new Stopwatch();
        while (!ct.IsCancellationRequested)
        {
            sw.Restart();
            try { Tick(); } catch (Exception ex) { KLog.Cerr($"KLoginServer Tick error: {ex.Message}"); }
            sw.Stop();
            try { await Task.Delay(Math.Max(5, 50 - (int)sw.ElapsedMilliseconds), ct); }
            catch (OperationCanceledException) { break; }
        }
    }

    public override void Tick()
    {
        base.Tick();
        _simLayer.Tick();
        AccountBlockManager.Tick(Db, Region);
        RankingManager.Tick(Db, Region);
        LocalRankingManager.TickWeeklyReset();
        BuffManager.CheckEndBuff();

        if (DateTime.UtcNow - _lastCcuLogUtc >= TimeSpan.FromSeconds(60))
        {
            _lastCcuLogUtc = DateTime.UtcNow;
            GuildBoardManager.PruneExpiredAds(_lastCcuLogUtc);
            GuildManager.CheckExpiredCashSkillPoints(_lastCcuLogUtc);
            UserList.UpdateChannelCcu(1, _authedUsers.Count);
            ServerCcuManager.UpdateCcuInfo(new KServerCCUManager.ServerCcuInfo
            {
                ServerUid = Uid,
                ConcurrentUser = _authedUsers.Count,
            });
            ServerCcuManager.TryBuildDbUpdate(_lastCcuLogUtc);
            Db.InsertConcurrentUserLog(Region.Code, Uid, ServerGroupId, 1, _authedUsers.Count);
        }
    }

    protected override void ProcessEvent(KEvent ev)
    {
        switch (ev.EventId)
        {
            case EventId.ELG_VERIFY_ACCOUNT_REQ:
            case EventId.ELG_SECURITY_AUTH_REQ:
            {
                var req = ev.GetPayload<KECH_VERIFY_ACCOUNT_REQ>(Region);
                int netErr;
                long userUid = 0;

                if (AccountBlockManager.IsBlocked(req.Id, KAccountBlockManager.BlockType.Connect, out _))
                {
                    netErr = NetError.ERR_VERIFY_11;
                }
                else
                {
                    (netErr, userUid, _, _, _) = Db.VerifyOrRegisterAccount(Region.Code, req.Id, req.Password);
                }

                var ack = new KECH_VERIFY_ACCOUNT_ACK
                {
                    Ok = netErr,
                    UserUid = userUid,
                    UserId = req.Id,
                    Passport = userUid > 0 ? Convert.ToHexString(Guid.NewGuid().ToByteArray()) : string.Empty,
                    ServerGroupId = ServerGroupId,
                };

                var replyId = ev.EventId == EventId.ELG_VERIFY_ACCOUNT_REQ
                    ? EventId.ELG_VERIFY_ACCOUNT_ACK
                    : EventId.ELG_SECURITY_AUTH_ACK;
                long targetUid = ev.Trace0 > 0 ? ev.Trace0 : ev.FirstSenderUid;
                Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_CHANNEL_USER, targetUid, replyId, ack);
                break;
            }

            case EventId.ELG_USER_DISCONNECT_REQ:
            {
                var req = ev.GetPayload<KELG_USER_DISCONNECT_REQ>(Region);
                if (req.UserUid > 0 && _authedUsers.TryRemove(req.UserUid, out var oldActor))
                {
                    UserList.UnregisterOnline(req.UserUid, oldActor.Uid);
                    oldActor.OnDestroy();
                }
                long senderGs = ev.FirstSenderUid;
                if (senderGs > 0)
                {
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_SERVER, senderGs,
                        EventId.ELG_USER_DISCONNECT_ACK, new KELG_USER_DISCONNECT_ACK { Ok = NetError.NET_OK, UserUid = req.UserUid });
                }
                break;
            }

            case EventId.ELG_ADMIN_KICK_USER_REQ:
            {
                var req = ev.GetPayload<KELG_ADMIN_KICK_USER_REQ>(Region);
                bool kicked = false;
                if (req.TargetUserUid > 0 && _authedUsers.TryRemove(req.TargetUserUid, out var user))
                {
                    user.SendPacket(EventId.E_KICK_USER_NOT, new KPacketOK { Ok = NetError.NET_OK });
                    user.OnDestroy();
                    kicked = true;
                }
                long senderGs = ev.FirstSenderUid;
                if (senderGs > 0)
                {
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_SERVER, senderGs,
                        EventId.ELG_ADMIN_KICK_USER_ACK,
                        new KELG_ADMIN_KICK_USER_ACK { Ok = kicked ? NetError.NET_OK : NetError.ERR_ADMIN_COMMAND_04, TargetUserUid = req.TargetUserUid });
                }
                break;
            }

            case EventId.ELG_CREATE_GUILD_REQ:
            {
                var req = ev.GetPayload<KELG_CREATE_GUILD_REQ>(Region);
                var (ok, guild, err) = GuildManager.CreateGuild(Db, Region, req.MasterUnitUid, req.GuildName, req.GuildMessage);
                if (req.GameServerUid > 0)
                {
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_SERVER, req.GameServerUid,
                        EventId.ELG_CREATE_GUILD_ACK,
                        new KELG_CREATE_GUILD_ACK
                        {
                            Ok = ok ? NetError.NET_OK : err,
                            MasterUnitUid = req.MasterUnitUid,
                            GuildInfo = guild ?? new KGuildInfo(),
                        });
                }
                break;
            }

            case EventId.ELG_GET_GUILD_INFO_REQ:
            {
                var req = ev.GetPayload<KELG_GET_GUILD_INFO_REQ>(Region);
                var guild = GuildManager.GetGuildInfo(Db, Region, req.GuildUid);
                var entry = GuildManager.GetGuildEntry(req.GuildUid);
                if (req.GameServerUid > 0)
                {
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_SERVER, req.GameServerUid,
                        EventId.ELG_GET_GUILD_INFO_ACK,
                        new KELG_GET_GUILD_INFO_ACK
                        {
                            Ok = guild != null ? NetError.NET_OK : NetError.ERR_GUILD_01,
                            RequesterUnitUid = req.RequesterUnitUid,
                            GuildInfo = guild ?? new KGuildInfo(),
                            GuildSkillInfo = entry != null ? new KGuildSkillInfo { GuildSPoint = entry.GuildSkillPoints } : new KGuildSkillInfo(),
                        });
                }
                break;
            }

            case EventId.ELG_UPDATE_GUILD_EXP_NOT:
            {
                var req = ev.GetPayload<KELG_UPDATE_GUILD_EXP_NOT>(Region);
                GuildManager.AddGuildExp(Db, Region, req.GuildUid, req.DeltaExp);
                break;
            }

            case EventId.ELG_LOCAL_RANKING_CHANGED_RANKER_INFO_NOT:
            {
                var req = ev.GetPayload<KELG_LOCAL_RANKING_CHANGED_RANKER_INFO_NOT>(Region);
                LocalRankingManager.SomethingChanged(req.SystemUid, req.UserInfo, req.UnitInfo);
                break;
            }

            case EventId.ELG_NEW_RECORD_HENIR_RANKING_NOT:
            {
                var req = ev.GetPayload<KELG_NEW_RECORD_HENIR_RANKING_NOT>(Region);
                if (HenirRankingManager.CheckNewRecord(req.RankingInfo))
                {
                    Db.UpsertHenirRanking(
                        0,
                        req.RankingInfo.UnitUid,
                        req.RankingInfo.NickName,
                        (byte)Math.Max((sbyte)0, req.RankingInfo.UnitClass),
                        req.RankingInfo.Level,
                        req.RankingInfo.StageCount,
                        (int)req.RankingInfo.PlayTime,
                        DateTime.UtcNow);
                }
                break;
            }

            case EventId.ELG_ADMIN_DELETE_HENIR_RANKING_RANK_NOT:
            {
                var req = ev.GetPayload<KEGS_ADMIN_DELETE_HENIR_RANKING_RANK_NOT>(Region);
                var existing = HenirRankingManager.GetRankings();
                long uidToDel = req.Rank > 0 && req.Rank <= existing.Count ? existing[req.Rank - 1].UnitUid : 0;
                if (HenirRankingManager.DeleteRecordByRank(req.Rank) && uidToDel > 0)
                    Db.DeleteHenirRankingByUnitUid(uidToDel);
                break;
            }

            case EventId.ELG_ADMIN_DELETE_HENIR_RANKING_NICKNAME_NOT:
            {
                var req = ev.GetPayload<KEGS_ADMIN_DELETE_HENIR_RANKING_NICKNAME_NOT>(Region);
                if (HenirRankingManager.DeleteRecordByNickName(req.NickName))
                    Db.DeleteHenirRankingByNickName(req.NickName);
                break;
            }

            case EventId.ELG_ADMIN_DELETE_HENIR_RANKING_UNITUID_NOT:
            {
                var req = ev.GetPayload<KEGS_ADMIN_DELETE_HENIR_RANKING_UNITUID_NOT>(Region);
                if (HenirRankingManager.DeleteRecord(req.UnitUid))
                    Db.DeleteHenirRankingByUnitUid(req.UnitUid);
                break;
            }

            case EventId.ELG_REGISTER_UNIT_NOT:
            {
                var req = ev.GetPayload<KELG_REGISTER_UNIT_NOT>(Region);
                long gsUserUid = ev.FirstSenderUid > 0 ? ev.FirstSenderUid : req.UserUid;
                UserList.RegisterOnlineUnit(req.UnitUid, req.UserUid, req.GameServerUid, gsUserUid, req.ChannelId, req.NickName);
                break;
            }

            case EventId.ELG_UNIT_DISCONNECT_NOT:
            {
                var req = ev.GetPayload<KELG_UNIT_DISCONNECT_NOT>(Region);
                UserList.UnregisterOnlineUnit(req.UnitUid);
                break;
            }

            case EventId.ELG_ACCEPT_FRIEND_NOT:
            {
                var req = ev.GetPayload<KELG_ACCEPT_FRIEND_NOT>(Region);
                if (UserList.TryGetGSUserInfoByUnitUid(req.DestUnitUid, out var target))
                {
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, target.GsUserUid,
                        EventId.ELG_ACCEPT_FRIEND_NOT, req);
                }
                break;
            }

            case EventId.ELG_BLOCK_FRIEND_NOT:
            {
                var req = ev.GetPayload<KELG_BLOCK_FRIEND_NOT>(Region);
                if (UserList.TryGetGSUserInfoByUnitUid(req.DestUnitUid, out var target))
                {
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, target.GsUserUid,
                        EventId.ELG_BLOCK_FRIEND_NOT, req);
                }
                break;
            }

            case EventId.ELG_CALL_MY_LOVER_FIND_USER_VILLAGE_REQ:
            {
                var req = ev.GetPayload<KELG_CALL_MY_LOVER_FIND_USER_VILLAGE_REQ>(Region);
                if (!UserList.TryGetGSUserInfoByUnitUid(req.LoverUnitUid, out var target))
                {
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, req.CalledUserUid,
                        EventId.ELG_CALL_MY_LOVER_FIND_USER_VILLAGE_ACK,
                        new KELG_CALL_MY_LOVER_FIND_USER_VILLAGE_ACK { Ok = NetError.ERR_RELATIONSHIP_08 });
                    break;
                }
                if (target.ChannelId != req.ChannelId)
                {
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, req.CalledUserUid,
                        EventId.ELG_CALL_MY_LOVER_FIND_USER_VILLAGE_ACK,
                        new KELG_CALL_MY_LOVER_FIND_USER_VILLAGE_ACK { Ok = NetError.ERR_RELATIONSHIP_30 });
                    break;
                }
                Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, target.GsUserUid,
                    EventId.ELG_CALL_MY_LOVER_VILLAGE_NOT, req);
                Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, req.CalledUserUid,
                    EventId.ELG_CALL_MY_LOVER_FIND_USER_VILLAGE_ACK,
                    new KELG_CALL_MY_LOVER_FIND_USER_VILLAGE_ACK { Ok = NetError.NET_OK });
                break;
            }

            case EventId.ELG_CALL_MY_LOVER_AGREE_RESULT_VILLAGE_NOT:
            {
                var req = ev.GetPayload<KELG_CALL_MY_LOVER_AGREE_RESULT_VILLAGE_NOT>(Region);
                Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, req.CalledUserUid,
                    EventId.ELG_CALL_MY_LOVER_AGREE_RESULT_VILLAGE_NOT, req);
                if (req.Agree)
                {
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, req.MyUserUid,
                        EventId.ELG_CALL_MY_LOVER_STATE_CHANGE_FIELD_NOT,
                        new KELG_CALL_MY_LOVER_STATE_CHANGE_FIELD_NOT
                        {
                            UseWarpItem = false,
                            MapId = req.MapId,
                            Pos = req.Pos,
                            LastTouchLineIndex = req.LastTouchLineIndex,
                        });
                }
                break;
            }

            case EventId.ELG_CALL_MY_LOVER_FIND_USER_ROOM_REQ:
            {
                var req = ev.GetPayload<KELG_CALL_MY_LOVER_FIND_USER_ROOM_REQ>(Region);
                if (!UserList.TryGetGSUserInfoByUnitUid(req.LoverUnitUid, out var target))
                {
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, req.CalledUserUid,
                        EventId.ELG_CALL_MY_LOVER_FIND_USER_ROOM_ACK,
                        new KELG_CALL_MY_LOVER_FIND_USER_ROOM_ACK { Ok = NetError.ERR_RELATIONSHIP_08 });
                    break;
                }
                Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, target.GsUserUid,
                    EventId.ELG_CALL_MY_LOVER_ROOM_NOT, req);
                Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, req.CalledUserUid,
                    EventId.ELG_CALL_MY_LOVER_FIND_USER_ROOM_ACK,
                    new KELG_CALL_MY_LOVER_FIND_USER_ROOM_ACK { Ok = NetError.NET_OK });
                break;
            }

            case EventId.ELG_CALL_MY_LOVER_AGREE_RESULT_ROOM_NOT:
            {
                var req = ev.GetPayload<KELG_CALL_MY_LOVER_AGREE_RESULT_ROOM_NOT>(Region);
                if (!UserList.TryGetGSUserInfoByUserUid(req.CalledUserUid, out var target))
                {
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, req.CalledUserUid,
                        EventId.ELG_CALL_MY_LOVER_AGREE_RESULT_ROOM_NOT, req);
                    break;
                }
                Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, req.CalledUserUid,
                    EventId.ELG_CALL_MY_LOVER_AGREE_RESULT_ROOM_NOT, req);
                if (req.Agree)
                {
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, req.MyUserUid,
                        EventId.ELG_CALL_MY_LOVER_JOIN_BATTLE_FIELD_NOT,
                        new KELG_CALL_MY_LOVER_JOIN_BATTLE_FIELD_NOT
                        {
                            CalledUserUid = req.CalledUserUid,
                            CalledUnitUid = target.UnitUid,
                            CalledGsUid = req.CalledGsUid,
                            BattleFieldId = req.BattleFieldId,
                            RoomUid = req.RoomUid,
                            Pos = req.Pos,
                            LastTouchLineIndex = req.LastTouchLineIndex,
                        });
                }
                break;
            }

            case EventId.ELG_CALL_MY_LOVER_JOIN_BATTLE_FIELD_FAILD_NOT:
            {
                var req = ev.GetPayload<KELG_CALL_MY_LOVER_JOIN_BATTLE_FIELD_FAILD_NOT>(Region);
                Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, req.CalledUserUid,
                    EventId.ELG_CALL_MY_LOVER_JOIN_BATTLE_FIELD_FAILD_NOT, req);
                break;
            }

            case EventId.ELG_SEARCH_UNIT_REQ:
            {
                var req = ev.GetPayload<KELG_SEARCH_UNIT_REQ>(Region);
                KLoginUserList.OnlineUnitEntry? target = null;
                if (req.UnitUid > 0)
                    UserList.TryGetGSUserInfoByUnitUid(req.UnitUid, out target!);
                else if (!string.IsNullOrWhiteSpace(req.NickName))
                    UserList.TryGetGSUserInfoByNickName(req.NickName, out target!);

                var dbUnit = target != null
                    ? Db.GetUnitByUid(Region, target.UnitUid)
                    : (req.UnitUid > 0 ? Db.GetUnitByUid(Region, req.UnitUid) : Db.GetUnitByNickName(Region, req.NickName));

                var ack = new KELG_SEARCH_UNIT_ACK
                {
                    Ok = dbUnit != null ? NetError.NET_OK : NetError.ERR_SEARCH_UNIT_03,
                    EventIdValue = req.EventIdValue,
                    CUnitInfo = dbUnit != null
                        ? new KConnectionUnitInfo
                        {
                            UnitUid = dbUnit.UnitUid,
                            CharName = dbUnit.NickName,
                            UnitClass = (sbyte)dbUnit.UnitClass,
                            Level = dbUnit.Level,
                            ChannelId = target?.ChannelId ?? 0,
                        }
                        : new KConnectionUnitInfo(),
                };
                if (ev.FirstSenderUid > 0)
                {
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ELG_SEARCH_UNIT_ACK, ack);
                }
                break;
            }

            case EventId.ELG_CHAT_REQ:
            {
                var req = ev.GetPayload<KELG_CHAT_REQ>(Region);
                bool found = !string.IsNullOrWhiteSpace(req.ReceiverNickName) &&
                             UserList.TryGetGSUserInfoByNickName(req.ReceiverNickName, out var target);
                if (found)
                {
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, target!.GsUserUid,
                        EventId.ELG_CHAT_NOT, req);
                }
                if (ev.FirstSenderUid > 0)
                {
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ELG_CHAT_ACK, new KELG_CHAT_ACK
                        {
                            Ok = found ? NetError.NET_OK : NetError.ERR_SEARCH_UNIT_03,
                            ChatPacketType = req.ChatPacketType,
                        });
                }
                break;
            }

            case EventId.ELG_GET_RANKING_FOR_GS_RANKING_MANAGER_REQ:
            {
                if (ev.FirstSenderUid > 0)
                {
                    var ack = new KELG_GET_RANKING_FOR_GS_RANKING_MANAGER_ACK();
                    var ranks = HenirRankingManager.GetRankings();
                    ack.HenirRanking[0] = ranks;
                    ack.LastRank[0] = (uint)ranks.Count;
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_SERVER, ev.FirstSenderUid,
                        EventId.ELG_GET_RANKING_FOR_GS_RANKING_MANAGER_ACK, ack);
                }
                break;
            }

            case EventId.ELG_UPDATE_USER_UNIT_BUFF_INFO_REQ:
            {
                var req = ev.GetPayload<KELG_UPDATE_USER_UNIT_BUFF_INFO_REQ>(Region);
                if (req.ActivateBuff.Count > 0)
                {
                    BuffManager.AddUnitBuffInfo(req.UnitUid, req.ActivateBuff.Select(b => new KLoginBuffManager.CachedBuffInfo
                    {
                        BuffTempletId = b.FactorInfo.BuffIdentity.BuffTempletId,
                        FactorId = b.FactorInfo.FactorId,
                        BuffStartTime = b.BuffStartTime,
                        BuffEndTime = b.BuffEndTime,
                    }));
                }
                if (req.DeactivateBuff.Count > 0)
                    BuffManager.DeleteUnitBuffInfo(req.UnitUid, req.DeactivateBuff);

                if (req.SendAck && ev.FirstSenderUid > 0)
                {
                    var activeList = new List<KBuffInfo>();
                    if (BuffManager.GetUnitAllBuffInfo(req.UnitUid, out var cached) && cached != null)
                    {
                        foreach (var b in cached.Buffs.Values)
                        {
                            activeList.Add(new KBuffInfo
                            {
                                FactorInfo = new KBuffFactor
                                {
                                    BuffIdentity = new KBuffIdentity { BuffTempletId = b.BuffTempletId },
                                    FactorId = b.FactorId,
                                },
                                BuffStartTime = b.BuffStartTime,
                                BuffEndTime = b.BuffEndTime,
                            });
                        }
                    }
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ELG_UPDATE_USER_UNIT_BUFF_INFO_ACK, new KELG_UPDATE_USER_UNIT_BUFF_INFO_ACK
                        {
                            Ok = NetError.NET_OK,
                            UnitUid = req.UnitUid,
                            ActiveBuffs = activeList,
                        });
                }
                break;
            }

            case EventId.ELG_UPDATE_CCU_INFO_NOT:
            {
                var req = ev.GetPayload<KELG_UPDATE_CCU_INFO_NOT>(Region);
                ServerCcuManager.UpdateCcuInfo(new KServerCCUManager.ServerCcuInfo
                {
                    ServerUid = req.ServerUid,
                    ConcurrentUser = req.ConcurrentUser,
                });
                break;
            }

            case EventId.ELG_INVITE_PARTY_FIND_RECEIVER_REQ:
            {
                var req = ev.GetPayload<KELG_INVITE_PARTY_FIND_RECEIVER_REQ>(Region);
                bool found = req.ReceiverUnitUid > 0
                    ? UserList.TryGetGSUserInfoByUnitUid(req.ReceiverUnitUid, out var target)
                    : UserList.TryGetGSUserInfoByNickName(req.ReceiverNickName, out target);

                if (ev.FirstSenderUid > 0)
                {
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ELG_INVITE_PARTY_FIND_RECEIVER_ACK, new KELG_INVITE_PARTY_FIND_RECEIVER_ACK
                        {
                            Ok = found ? NetError.NET_OK : NetError.ERR_SEARCH_UNIT_03,
                            HostUnitUid = req.HostUnitUid,
                            ReceiverUnitUid = target?.UnitUid ?? req.ReceiverUnitUid,
                            ReceiverNickName = target?.NickName ?? req.ReceiverNickName,
                        });
                }
                break;
            }

            case EventId.ELG_INVITE_PVP_ROOM_REQ:
            {
                var req = ev.GetPayload<KELG_INVITE_PVP_ROOM_REQ>(Region);
                bool found = UserList.TryGetGSUserInfoByUnitUid(req.ToUnitUid, out var target);
                if (found)
                {
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, target!.GsUserUid,
                        EventId.ELG_INVITE_PVP_ROOM_REQ, req);
                }
                if (ev.FirstSenderUid > 0)
                {
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ELG_INVITE_PVP_ROOM_ACK, new KELG_INVITE_PVP_ROOM_ACK
                        {
                            Ok = found ? NetError.NET_OK : NetError.ERR_SEARCH_UNIT_03,
                            ToUnitUid = req.ToUnitUid,
                        });
                }
                break;
            }

            case EventId.ELG_FIND_USER_SEND_LETTER_NOT:
            {
                var req = ev.GetPayload<KELG_FIND_USER_SEND_LETTER_NOT>(Region);
                bool found = req.TargetUnitUid > 0
                    ? UserList.TryGetGSUserInfoByUnitUid(req.TargetUnitUid, out var target)
                    : UserList.TryGetGSUserInfoByNickName(req.TargetNickName, out target);
                if (found)
                {
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, target!.GsUserUid,
                        EventId.ELG_FIND_USER_SEND_LETTER_NOT, req);
                }
                break;
            }

            case EventId.ELG_REQUEST_FRIEND_NOT:
            case EventId.ELG_DELETE_FRIEND_NOT:
            case EventId.ELG_DENY_FRIEND_NOT:
            case EventId.ELG_UNBLOCK_FRIEND_NOT:
            {
                var req = ev.GetPayload<KELG_FRIEND_UNIT_NOT>(Region);
                if (UserList.TryGetGSUserInfoByUnitUid(req.DestUnitUid, out var target))
                {
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, target.GsUserUid,
                        ev.EventId, req);
                }
                break;
            }

            case EventId.ELG_FRIEND_POSITION_UPDATE_NOT:
            {
                var req = ev.GetPayload<KELG_FRIEND_POSITION_UPDATE_NOT>(Region);
                foreach (long friendUid in req.FriendUnitUids)
                {
                    if (UserList.TryGetGSUserInfoByUnitUid(friendUid, out var target))
                    {
                        Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, target.GsUserUid,
                            EventId.ELG_FRIEND_POSITION_UPDATE_NOT, req);
                    }
                }
                break;
            }

            case EventId.ELG_ADD_WEDDING_HALL_INFO_NOT:
            {
                var req = ev.GetPayload<KELG_ADD_WEDDING_HALL_INFO_NOT>(Region);
                WeddingHallManager.AddWeddingInfo(new KWeddingHallManager.WeddingHallEntry
                {
                    WeddingUid = req.WeddingUid,
                    RoomUid = req.RoomUid,
                    BrideUnitUid = req.BrideUnitUid,
                    GroomUnitUid = req.GroomUnitUid,
                    BrideNickName = req.BrideNickName,
                    GroomNickName = req.GroomNickName,
                    WeddingHallType = req.WeddingHallType,
                    OfficiantNpc = req.OfficiantNpc,
                });
                break;
            }

            case EventId.ELG_CHECK_WEDDING_HALL_INFO_REQ:
            {
                var req = ev.GetPayload<KELG_CHECK_WEDDING_HALL_INFO_REQ>(Region);
                bool ok = WeddingHallManager.TryGetWeddingInfo(req.WeddingUid, out var entry) && entry != null;
                if (ev.FirstSenderUid > 0)
                {
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ELG_CHECK_WEDDING_HALL_INFO_ACK, new KELG_CHECK_WEDDING_HALL_INFO_ACK
                        {
                            Ok = ok ? NetError.NET_OK : NetError.ERR_RELATIONSHIP_08,
                            WeddingUid = req.WeddingUid,
                            RoomUid = entry?.RoomUid ?? 0,
                        });
                }
                break;
            }

            case EventId.ELG_BREAK_UP_NOT:
            case EventId.ELG_DIVORCE_NOT:
            case EventId.ELG_CHANGE_LOVE_WORD_NOT:
            {
                var req = ev.GetPayload<KELG_RELATIONSHIP_EVENT_NOT>(Region);
                if (UserList.TryGetGSUserInfoByUnitUid(req.PartnerUnitUid, out var target))
                {
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, target.GsUserUid,
                        ev.EventId, req);
                }
                break;
            }

            case EventId.ELG_GET_GUILD_AD_LIST_REQ:
            {
                var req = ev.GetPayload<KELG_GET_GUILD_AD_LIST_REQ>(Region);
                var (items, totalPages) = GuildBoardManager.GetGuildAdList(
                    KGuildBoardManager.AdSortType.RegDate, Math.Max(0, req.CurrentPage - 1));
                if (ev.FirstSenderUid > 0)
                {
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ELG_GET_GUILD_AD_LIST_ACK, new KELG_GET_GUILD_AD_LIST_ACK
                        {
                            Ok = NetError.NET_OK,
                            TotalPage = totalPages,
                            CurrentPage = req.CurrentPage,
                            AdList = items.Select(a => new KGuildAdInfo
                            {
                                GuildUid = a.GuildUid,
                                GuildName = a.GuildName,
                                GuildLevel = a.GuildLevel,
                                GuildExp = a.GuildExp,
                                MasterNickName = a.MasterNickName,
                                CurNumMember = a.CurMemberCount,
                                AdMessage = a.AdMessage,
                                AdRegDate = a.RegDateUtc.ToString("yyyy-MM-dd HH:mm:ss"),
                                AdEndDate = a.EndDateUtc.ToString("yyyy-MM-dd HH:mm:ss"),
                            }).ToList(),
                        });
                }
                break;
            }

            case EventId.ELG_REGISTRATION_GUILD_AD_REQ:
            {
                var req = ev.GetPayload<KELG_REGISTRATION_GUILD_AD_REQ>(Region);
                var guild = GuildManager.GetGuildInfo(Db, Region, req.GuildUid);
                if (guild != null)
                {
                    GuildBoardManager.NewGuildAd(new KGuildBoardManager.GuildAdEntry
                    {
                        GuildUid = req.GuildUid,
                        GuildName = guild.GuildName,
                        GuildLevel = guild.GuildLevel,
                        GuildExp = guild.GuildExp,
                        AdMessage = req.AdMessage,
                        EndDateUtc = DateTime.UtcNow.AddDays(Math.Clamp((int)req.AdPeriod, 1, 30)),
                    });
                }
                if (ev.FirstSenderUid > 0)
                {
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ELG_REGISTRATION_GUILD_AD_ACK, new KELG_REGISTRATION_GUILD_AD_ACK
                        {
                            Ok = guild != null ? NetError.NET_OK : NetError.ERR_GUILD_01,
                            GuildUid = req.GuildUid,
                        });
                }
                break;
            }

            // ---- Guild Cross-GS Handlers ----
            case EventId.ELG_GET_GUILD_USER_LIST_REQ:
            {
                var req = ev.GetPayload<KELG_GET_GUILD_USER_LIST_REQ>(Region);
                var members = Db.GetGuildMembers(req.GuildUid);
                var list = members.Select(m =>
                {
                    bool online = UserList.TryGetGSUserInfoByUnitUid(m.UnitUid, out var u);
                    return new KGuildMemberInfo
                    {
                        GsUid = online ? u.GsUid : 0,
                        UnitUid = m.UnitUid,
                        NickName = m.NickName,
                        UnitClass = m.UnitClass,
                        Level = m.Level,
                        MemberShipGrade = m.MemberGrade,
                        ChannelId = online ? u.ChannelId : 0,
                        Position = online ? (sbyte)1 : (sbyte)-1,
                    };
                }).ToList();
                if (ev.FirstSenderUid > 0)
                {
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ELG_GET_GUILD_USER_LIST_ACK, new KEGS_GET_GUILD_USER_LIST_ACK
                        {
                            Ok = NetError.NET_OK,
                            TotalPage = 1,
                            ViewPage = req.ViewPage,
                            GuildUserListType = req.GuildUserListType,
                            LoginedMemberCount = list.Count(m => m.Position > 0),
                            MemberPositionGradeList = list,
                        });
                }
                break;
            }

            case EventId.ELG_INVITE_GUILD_REQ:
            {
                var req = ev.GetPayload<KELG_INVITE_GUILD_REQ>(Region);
                var guild = GuildManager.GetGuildInfo(Db, Region, req.GuildUid);
                bool found = req.ReceiverUnitUid > 0
                    ? UserList.TryGetGSUserInfoByUnitUid(req.ReceiverUnitUid, out var target)
                    : UserList.TryGetGSUserInfoByNickName(req.ReceiverNickName, out target);
                int ok = guild == null ? NetError.ERR_GUILD_01 : (found ? NetError.NET_OK : NetError.ERR_SEARCH_UNIT_03);
                if (ev.FirstSenderUid > 0)
                {
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ELG_INVITE_GUILD_ACK, new KEGS_INVITE_GUILD_ACK
                        {
                            Ok = ok,
                            ReceiverNickName = target?.NickName ?? req.ReceiverNickName,
                        });
                }
                if (ok == NetError.NET_OK && target != null && guild != null)
                {
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, target.GsUserUid,
                        EventId.ELG_INVITE_GUILD_FIND_USER_REQ, new KEGS_INVITE_GUILD_NOT
                        {
                            GuildInfo = guild,
                            SenderNickName = req.SenderNickName,
                        });
                }
                break;
            }

            case EventId.ELG_INVITE_GUILD_REPLY_REQ:
            {
                var req = ev.GetPayload<KELG_INVITE_GUILD_REPLY_REQ>(Region);
                if (req.Reason == 0)
                {
                    var (ok, guild, err) = GuildManager.JoinGuild(Db, Region, req.GuildUid, req.ReceiverUnitUid);
                    if (ev.FirstSenderUid > 0)
                        Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                            EventId.ELG_INVITE_GUILD_REPLY_ACK, new KEGS_JOIN_GUILD_ACK { Ok = err, GuildInfo = guild ?? new KGuildInfo() });
                }
                else if (UserList.TryGetGSUserInfoByNickName(req.SenderNickName, out var inviter))
                {
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, inviter.GsUserUid,
                        EventId.ELG_INVITE_GUILD_REPLY_DENY_NOT, new KEGS_INVITE_GUILD_REPLY_DENY_NOT
                        {
                            NickName = req.SenderNickName,
                            Reason = req.Reason,
                        });
                }
                break;
            }

            case EventId.ELG_JOIN_GUILD_REQ:
            {
                var req = ev.GetPayload<KELG_GUILD_MEMBER_OP_NOT>(Region);
                var (ok, guild, err) = GuildManager.JoinGuild(Db, Region, req.GuildUid, req.TargetUnitUid);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ELG_JOIN_GUILD_ACK, new KEGS_JOIN_GUILD_ACK { Ok = err, GuildInfo = guild ?? new KGuildInfo() });
                break;
            }

            case EventId.ELG_CHECK_CHANGE_GUILD_MEMBER_GRADE_REQ:
            case EventId.ELG_UPDATE_CHANGE_GUILD_MEMBER_GRADE_NOT:
            {
                var req = ev.GetPayload<KELG_GUILD_MEMBER_OP_NOT>(Region);
                var (ok, targetNick) = GuildManager.ChangeMemberGrade(Db, req.GuildUid, req.TargetUnitUid, req.NewGrade);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ELG_CHECK_CHANGE_GUILD_MEMBER_GRADE_ACK, new KEGS_CHANGE_GUILD_MEMBER_GRADE_ACK
                        {
                            Ok = ok ? NetError.NET_OK : NetError.ERR_GUILD_01,
                            TargetNickName = targetNick,
                            MembershipGrade = req.NewGrade,
                        });
                break;
            }

            case EventId.ELG_UPDATE_CHANGE_GUILD_MESSAGE_NOT:
            {
                var req = ev.GetPayload<KELG_GUILD_MEMBER_OP_NOT>(Region);
                Db.UpdateGuildMessage(req.GuildUid, req.Message);
                var info = GuildManager.GetGuildInfo(Db, Region, req.GuildUid);
                if (info != null) info.GuildMessage = req.Message;
                break;
            }

            case EventId.ELG_UPDATE_KICK_GUILD_MEMBER_NOT:
            {
                var req = ev.GetPayload<KELG_GUILD_MEMBER_OP_NOT>(Region);
                if (req.ActorUnitUid == req.TargetUnitUid || req.ActorUnitUid <= 0)
                    GuildManager.QuitGuild(Db, req.GuildUid, req.TargetUnitUid);
                else
                    GuildManager.KickMember(Db, req.GuildUid, req.ActorUnitUid, req.TargetUnitUid);
                break;
            }

            case EventId.ELG_UPDATE_DISBAND_GUILD_NOT:
            {
                var req = ev.GetPayload<KELG_GUILD_MEMBER_OP_NOT>(Region);
                GuildManager.DisbandGuild(Db, req.GuildUid, req.ActorUnitUid);
                GuildBoardManager.DeleteGuildAd(req.GuildUid);
                break;
            }

            case EventId.ELG_CHECK_EXPAND_GUILD_MAX_MEMBER_REQ:
            {
                var req = ev.GetPayload<KELG_CHECK_EXPAND_GUILD_MAX_MEMBER_REQ>(Region);
                var info = GuildManager.GetGuildInfo(Db, Region, req.GuildUid);
                int ok = info == null
                    ? NetError.ERR_GUILD_06
                    : (info.MaxNumMember >= 200 ? NetError.ERR_GUILD_39 : NetError.NET_OK);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ELG_CHECK_EXPAND_GUILD_MAX_MEMBER_ACK, new KELG_CHECK_EXPAND_GUILD_MAX_MEMBER_ACK
                        {
                            Ok = ok,
                            ItemUid = req.ItemUid,
                        });
                break;
            }

            case EventId.ELG_CHANGE_GUILD_NAME_CHECK_REQ:
            {
                var req = ev.GetPayload<KELG_CHANGE_GUILD_NAME_REQ>(Region);
                bool avail = Db.IsGuildNameAvailable(Region, req.NewGuildName);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ELG_CHANGE_GUILD_NAME_CHECK_ACK,
                        new KPacketOK { Ok = avail ? NetError.NET_OK : NetError.ERR_GUILD_05 });
                break;
            }

            case EventId.ELG_CHANGE_GUILD_NAME_REQ:
            {
                var req = ev.GetPayload<KELG_CHANGE_GUILD_NAME_REQ>(Region);
                var info = GuildManager.GetGuildInfo(Db, Region, req.GuildUid);
                bool ok = info != null && Db.IsGuildNameAvailable(Region, req.NewGuildName);
                if (ok && info != null)
                {
                    info.GuildName = req.NewGuildName;
                    GuildBoardManager.UpdateGuildName(req.GuildUid, req.NewGuildName);
                }
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ELG_CHANGE_GUILD_NAME_ACK, new KEGS_CHANGE_GUILD_NAME_ACK
                        {
                            Ok = ok ? NetError.NET_OK : NetError.ERR_GUILD_05,
                            GuildInfo = info ?? new KGuildInfo(),
                        });
                break;
            }

            case EventId.ELG_GET_GUILD_SKILL_REQ:
            {
                var req = ev.GetPayload<KELG_GET_GUILD_SKILL_REQ>(Region);
                bool ok = Db.LearnGuildSkill(req.GuildUid, req.GuildSkillId);
                var entry = GuildManager.GetGuildEntry(req.GuildUid);
                if (ok && entry != null)
                {
                    int curLv = entry.SkillTree.GetSkillLevel(req.GuildSkillId);
                    entry.SkillTree.SetSkillLevelAndCsp(req.GuildSkillId, curLv + 1, 0);
                }
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ELG_GET_GUILD_SKILL_ACK, new KPacketOK { Ok = ok ? NetError.NET_OK : NetError.ERR_GUILD_SKILL_00 });
                break;
            }

            case EventId.ELG_RESET_GUILD_SKILL_REQ:
            {
                var req = ev.GetPayload<KELG_GET_GUILD_SKILL_REQ>(Region);
                Db.ResetGuildSkill(req.GuildUid, req.GuildSkillId);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ELG_RESET_GUILD_SKILL_ACK, new KEGS_RESET_GUILD_SKILL_ACK { Ok = NetError.NET_OK });
                break;
            }

            case EventId.ELG_INIT_GUILD_SKILL_REQ:
            {
                var req = ev.GetPayload<KELG_GET_GUILD_SKILL_REQ>(Region);
                Db.InitGuildSkills(req.GuildUid);
                GuildManager.GetGuildEntry(req.GuildUid)?.SkillTree.ResetGuildSkillTree();
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ELG_INIT_GUILD_SKILL_ACK, new KEGS_INIT_GUILD_SKILL_ACK { Ok = NetError.NET_OK });
                break;
            }

            case EventId.ELG_ADMIN_GET_GUILD_SKILL_POINT_REQ:
            {
                var req = ev.GetPayload<KELG_ADMIN_GET_GUILD_SKILL_POINT_REQ>(Region);
                var (ok, sp) = Db.AddGuildSkillPoints(req.GuildUid, req.GuildSPoint);
                var entry = GuildManager.GetGuildEntry(req.GuildUid);
                if (entry != null) entry.SkillTree.GuildSPoint = sp;
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ELG_ADMIN_GET_GUILD_SKILL_POINT_ACK, new KEGS_ADMIN_GET_GUILD_SKILL_POINT_ACK { Ok = ok, CurGuildSPoint = sp });
                break;
            }

            case EventId.ELG_APPLY_JOIN_GUILD_REQ:
            {
                var req = ev.GetPayload<KELG_APPLY_JOIN_GUILD_REQ>(Region);
                int ok = Db.ApplyJoinGuild(Region, req.GuildUid, new KUnitInfo
                {
                    UnitUid = req.UnitUid,
                    NickName = req.NickName,
                    UnitClass = (sbyte)req.UnitClass,
                    Level = req.Level,
                }, req.Message, req.OldApplyDelete);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ELG_APPLY_JOIN_GUILD_ACK, new KPacketOK { Ok = ok });
                break;
            }

            case EventId.ELG_GET_APPLY_JOIN_GUILD_LIST_REQ:
            {
                var req = ev.GetPayload<KELG_GET_APPLY_JOIN_GUILD_LIST_REQ>(Region);
                var applies = Db.GetGuildApplies(req.GuildUid);
                var guild = GuildManager.GetGuildInfo(Db, Region, req.GuildUid);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ELG_GET_APPLY_JOIN_GUILD_LIST_ACK, new KEGS_GET_APPLY_JOIN_GUILD_LIST_ACK
                        {
                            Ok = NetError.NET_OK,
                            GuildName = guild?.GuildName ?? string.Empty,
                            TotalPage = 1,
                            ViewPage = (uint)Math.Max(1, req.ViewPage),
                            ApplyJoinGuildList = applies,
                        });
                break;
            }

            case EventId.ELG_ACCEPT_JOIN_GUILD_REQ:
            {
                var req = ev.GetPayload<KELG_ACCEPT_JOIN_GUILD_REQ>(Region);
                var (ok, _, err) = GuildManager.JoinGuild(Db, Region, req.GuildUid, req.TargetUnitUid);
                if (ok) Db.DeleteGuildApply(req.GuildUid, req.TargetUnitUid);
                var target = Db.GetUnitByUid(Region, req.TargetUnitUid);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ELG_ACCEPT_JOIN_GUILD_ACK, new KEGS_ACCEPT_JOIN_GUILD_ACK
                        {
                            Ok = err,
                            ReceiverNickName = target?.NickName ?? string.Empty,
                        });
                break;
            }

            case EventId.ELG_DELETE_APPLY_JOIN_GUILD_REQ:
            {
                var req = ev.GetPayload<KELG_ACCEPT_JOIN_GUILD_REQ>(Region);
                Db.DeleteGuildApply(req.GuildUid, req.TargetUnitUid);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ELG_DELETE_APPLY_JOIN_GUILD_ACK, new KPacketOK { Ok = NetError.NET_OK });
                break;
            }

            case EventId.ELG_ADMIN_DELETE_GUILD_ADD_NOT:
            {
                var req = ev.GetPayload<KEGS_ADMIN_DELETE_GUILD_ADD_NOT>(Region);
                GuildBoardManager.DeleteGuildAd(req.GuildUid);
                break;
            }

            // ---- PersonalShop Agency Handlers ----
            case EventId.ERM_CHECK_MY_PSHOP_AGENCY_INFO_REQ:
            case EventId.ERM_CHECK_OPEN_PSHOP_AGENCY_REQ:
            {
                var pkt = ev.GetPayload<KPacketLong>(Region);
                var shop = PersonalShopAgencyManager.GetShopByHost(pkt.Value);
                EventId ackId = ev.EventId == EventId.ERM_CHECK_MY_PSHOP_AGENCY_INFO_REQ
                    ? EventId.ERM_CHECK_MY_PSHOP_AGENCY_INFO_ACK
                    : EventId.ERM_CHECK_OPEN_PSHOP_AGENCY_ACK;
                if (ev.FirstSenderUid > 0)
                {
                    int ok = ev.EventId == EventId.ERM_CHECK_MY_PSHOP_AGENCY_INFO_REQ
                        ? (shop != null ? NetError.NET_OK : NetError.ERR_PERSONAL_SHOP_23)
                        : (shop == null ? NetError.NET_OK : NetError.ERR_PERSONAL_SHOP_24);
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        ackId, new KERM_OPEN_PSHOP_AGENCY_ACK { Ok = ok, ShopUid = shop?.ShopUid ?? 0 });
                }
                break;
            }

            case EventId.ERM_OPEN_PSHOP_AGENCY_REQ:
            {
                var req = ev.GetPayload<KERM_OPEN_PSHOP_AGENCY_REQ>(Region);
                var shop = PersonalShopAgencyManager.CreateAgencyShop(
                    req.HostUnitUid, req.HostNickName, req.ShopName,
                    req.Items.Select(i => new KPersonalShopAgencyManager.AgencyShopItem
                    {
                        ItemUid = i.ItemUid,
                        ItemId = i.ItemId,
                        Quantity = i.Quantity,
                        PricePerUnit = i.PricePerUnit,
                    }),
                    TimeSpan.FromMinutes(Math.Max(1, req.DurationMinutes)));
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ERM_OPEN_PSHOP_AGENCY_ACK, new KERM_OPEN_PSHOP_AGENCY_ACK
                        {
                            Ok = shop != null ? NetError.NET_OK : NetError.ERR_PERSONAL_SHOP_24,
                            ShopUid = shop?.ShopUid ?? 0,
                        });
                break;
            }

            case EventId.ERM_CHECK_FOR_REG_PSHOP_AGENCY_ITEM_REQ:
            case EventId.ERM_REG_PSHOP_AGENCY_ITEM_REQ:
            {
                var req = ev.GetPayload<KERM_REG_PSHOP_AGENCY_ITEM_REQ>(Region);
                var shop = PersonalShopAgencyManager.GetShopByHost(req.HostUnitUid);
                bool canReg = shop != null && req.Items.Count <= KPersonalShopAgencyManager.SellItemLimitNum;
                if (canReg && ev.EventId == EventId.ERM_REG_PSHOP_AGENCY_ITEM_REQ && shop != null)
                {
                    lock (shop.SyncRoot)
                    {
                        foreach (var it in req.Items)
                        {
                            if (shop.Items.Count < KPersonalShopAgencyManager.SellItemLimitNum)
                                shop.Items.Add(new KPersonalShopAgencyManager.AgencyShopItem
                                {
                                    ItemUid = it.ItemUid,
                                    ItemId = it.ItemId,
                                    Quantity = it.Quantity,
                                    PricePerUnit = it.PricePerUnit,
                                });
                        }
                    }
                }
                EventId ackId = ev.EventId == EventId.ERM_CHECK_FOR_REG_PSHOP_AGENCY_ITEM_REQ
                    ? EventId.ERM_CHECK_FOR_REG_PSHOP_AGENCY_ITEM_ACK
                    : EventId.ERM_REG_PSHOP_AGENCY_ITEM_ACK;
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        ackId, new KPacketOK { Ok = canReg ? NetError.NET_OK : NetError.ERR_PERSONAL_SHOP_07 });
                break;
            }

            case EventId.ERM_JOIN_PERSONAL_SHOP_REQ:
            case EventId.ERM_JOIN_MY_PSHOP_AGENCY_REQ:
            {
                var pkt = ev.GetPayload<KPacketLong>(Region);
                var shop = PersonalShopAgencyManager.GetShopByHost(pkt.Value);
                if (shop != null && ev.FirstSenderUid > 0)
                {
                    lock (shop.SyncRoot)
                        shop.ActiveBuyerUnitUids.Add(ev.FirstSenderUid);
                }
                EventId ackId = ev.EventId == EventId.ERM_JOIN_MY_PSHOP_AGENCY_REQ
                    ? EventId.ERM_JOIN_MY_PSHOP_AGENCY_ACK
                    : EventId.ERM_JOIN_PERSONAL_SHOP_ACK;
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        ackId, new KERM_OPEN_PSHOP_AGENCY_ACK
                        {
                            Ok = shop != null ? NetError.NET_OK : NetError.ERR_PERSONAL_SHOP_09,
                            ShopUid = shop?.ShopUid ?? 0,
                        });
                break;
            }

            case EventId.ERM_BUY_PERSONAL_SHOP_ITEM_REQ:
            {
                var req = ev.GetPayload<KERM_BUY_PERSONAL_SHOP_ITEM_REQ>(Region);
                var (ok, totalPaid, sellerNet, itemId) = PersonalShopAgencyManager.BuyFromAgencyShop(
                    req.ShopUid, req.ItemUid, req.Quantity, int.MaxValue);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ERM_BUY_PERSONAL_SHOP_ITEM_ACK, new KERM_BUY_PERSONAL_SHOP_ITEM_ACK
                        {
                            Ok = ok ? NetError.NET_OK : NetError.ERR_PERSONAL_SHOP_16,
                            TotalCost = totalPaid,
                            NetSellerProceeds = sellerNet,
                            CommissionFee = Math.Max(0, totalPaid - sellerNet),
                        });
                if (ok)
                {
                    var shop = PersonalShopAgencyManager.GetShopByUid(req.ShopUid);
                    if (shop != null && UserList.TryGetGSUserInfoByUnitUid(shop.HostUnitUid, out var hostUser))
                    {
                        bool allSoldOut;
                        lock (shop.SyncRoot)
                            allSoldOut = shop.Items.All(i => i.Quantity <= 0);
                        Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, hostUser.GsUserUid,
                            EventId.ELG_PSHOP_AGENCY_MESSAGE_NOT, new KEGS_PSHOP_AGENCY_MESSAGE_NOT
                            {
                                MessageType = (sbyte)(allSoldOut
                                    ? KEGS_PSHOP_AGENCY_MESSAGE_NOT.PAMT_SOLD_OUT
                                    : KEGS_PSHOP_AGENCY_MESSAGE_NOT.PAMT_SELL_ITEM),
                                HostUnitUid = shop.HostUnitUid,
                                SellItemId = itemId,
                                SellQuantity = req.Quantity,
                                SellED = sellerNet,
                            });
                    }
                }
                break;
            }

            case EventId.ERM_STOP_SALE_PSHOP_AGENCY_REQ:
            case EventId.ERM_PICK_UP_FROM_PSHOP_AGENCY_REQ:
            case EventId.ERM_PREPARE_PICK_UP_FROM_PSHOP_AGENCY_REQ:
            {
                var pkt = ev.GetPayload<KPacketLong>(Region);
                var (ok, pickedEd, rem) = PersonalShopAgencyManager.PickUpAndCloseShop(pkt.Value);
                EventId ackId = ev.EventId switch
                {
                    EventId.ERM_STOP_SALE_PSHOP_AGENCY_REQ => EventId.ERM_STOP_SALE_PSHOP_AGENCY_ACK,
                    EventId.ERM_PREPARE_PICK_UP_FROM_PSHOP_AGENCY_REQ => EventId.ERM_PREPARE_PICK_UP_FROM_PSHOP_AGENCY_ACK,
                    _ => EventId.ERM_PICK_UP_FROM_PSHOP_AGENCY_ACK,
                };
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        ackId, new KERM_PICK_UP_FROM_PSHOP_AGENCY_ACK
                        {
                            Ok = ok ? NetError.NET_OK : NetError.ERR_PERSONAL_SHOP_23,
                            PickedEd = pickedEd,
                            RemainingItemCount = rem.Count,
                        });
                break;
            }

            // ---- Ranking / LocalRanking / Server / Admin ----
            case EventId.ELG_ADMIN_NEW_RECORD_NOT:
            {
                var info = ev.GetPayload<KHenirRankingInfo>(Region);
                HenirRankingManager.CheckNewRecord(info);
                break;
            }

            case EventId.ELG_ADMIN_DELETE_RECORD_NOT:
            {
                var pkt = ev.GetPayload<KPacketLong>(Region);
                HenirRankingManager.DeleteRecord(pkt.Value);
                break;
            }

            case EventId.EGS_LOCAL_RANKING_INQUIRY_REQ:
            {
                var req = ev.GetPayload<KEGS_LOCAL_RANKING_INQUIRY_REQ>(Region);
                var (list, totalPage, viewPage, myRank) = LocalRankingManager.GetRankerList(
                    req.MainTabIndex, req.SubTabIndex, req.Filter, req.ViewPage, ev.FirstSenderUid);
                if (ev.FirstSenderUid > 0)
                {
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.EGS_LOCAL_RANKING_INQUIRY_ACK, new KEGS_LOCAL_RANKING_INQUIRY_ACK
                        {
                            Ok = NetError.NET_OK,
                            TotalPage = totalPage,
                            ViewPage = viewPage,
                            MainTabIndex = req.MainTabIndex,
                            SubTabIndex = req.SubTabIndex,
                            Filter = req.Filter,
                            RankerList = list,
                            MyRankInfo = myRank,
                        });
                }
                break;
            }

            case EventId.ELG_ADMIN_SHOW_ED_CHEAT_REQ:
            {
                var req = ev.GetPayload<KEGS_ADMIN_SHOW_ED_CHEAT_REQ>(Region);
                var unit = Db.GetUnitByNickName(Region, req.NickName);
                if (ev.FirstSenderUid > 0)
                {
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ELG_ADMIN_SHOW_ED_CHEAT_ACK, new KEGS_ADMIN_SHOW_ED_CHEAT_ACK
                        {
                            Ok = unit != null ? NetError.NET_OK : NetError.ERR_SEARCH_UNIT_03,
                            NickName = req.NickName,
                            Ed = unit?.Ed ?? 0,
                        });
                }
                break;
            }

            case EventId.ELG_INVITE_PARTY_GET_RECEIVER_USER_INFO_REQ:
            {
                var req = ev.GetPayload<KELG_INVITE_PARTY_FIND_RECEIVER_REQ>(Region);
                bool found = req.ReceiverUnitUid > 0
                    ? UserList.TryGetGSUserInfoByUnitUid(req.ReceiverUnitUid, out var target)
                    : UserList.TryGetGSUserInfoByNickName(req.ReceiverNickName, out target);
                if (ev.FirstSenderUid > 0)
                {
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ELG_INVITE_PARTY_GET_RECEIVER_USER_INFO_ACK, new KELG_INVITE_PARTY_FIND_RECEIVER_ACK
                        {
                            Ok = found ? NetError.NET_OK : NetError.ERR_PARTY_07,
                            HostUnitUid = req.HostUnitUid,
                            ReceiverUnitUid = target?.UnitUid ?? req.ReceiverUnitUid,
                            ReceiverNickName = target?.NickName ?? req.ReceiverNickName,
                        });
                }
                break;
            }

            case EventId.ELG_VERIFY_SERVER_CONNECT_REQ:
            {
                long targetServerUid = ev.LastSenderUid > 0 ? ev.LastSenderUid : ev.FirstSenderUid;
                if (targetServerUid > 0)
                {
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_CH_SERVER, targetServerUid,
                        EventId.ELG_VERIFY_SERVER_CONNECT_ACK, new KECN_VERIFY_SERVER_CONNECT_ACK
                        {
                            Ok = NetError.NET_OK,
                            CenterUid = Uid,
                        });
                }
                break;
            }

            case EventId.ELG_DISCONNECT_SERVER_REPORT_NOT:
            case EventId.E_DISCONNECT_SERVER_REPORT_NOT:
            {
                var req = ev.GetPayload<KE_DISCONNECT_SERVER_REPORT_NOT>(Region);
                _disconnectReports.AddOrUpdate(req.Type, 1, static (_, c) => c + 1);
                break;
            }

            case EventId.ESR_ORDER_TO_REFRESH_MANAGER_REQ:
            case EventId.ESR_ORDER_TO_REFRESH_MANAGER_ACK:
            {
                var order = ev.PayloadBytes.Length > 0
                    ? ev.GetPayload<KESR_SCRIPT_REFRESH_ORDER_NOT>(Region)
                    : new KESR_SCRIPT_REFRESH_ORDER_NOT();
                ScriptRefreshManager.ExecuteRefreshOrder(order.OrderType);
                break;
            }

            // ---- Additional TW ELG / DLG / ERM / EGIANT LoginServer Handlers ----
            case EventId.ELG_REGISTER_USER_REQ:
            case EventId.ELG_CHANNEL_CHANGE_REGISTER_USER_REQ:
            {
                var req = ev.GetPayload<KELG_REGISTER_USER_REQ>(Region);
                long gsUserUid = ev.FirstSenderUid > 0 ? ev.FirstSenderUid : req.UnitUid;
                var (dup, _) = UserList.RegisterOnline(req.UserUid, req.UserId, Uid, req.GameServerUid, req.ChannelId);
                if (req.UnitUid > 0)
                    UserList.RegisterOnlineUnit(req.UnitUid, req.UserUid, req.GameServerUid, gsUserUid, req.ChannelId, req.NickName);
                EventId ackId = ev.EventId == EventId.ELG_CHANNEL_CHANGE_REGISTER_USER_REQ
                    ? EventId.ELG_CHANNEL_CHANGE_REGISTER_USER_ACK
                    : EventId.ELG_REGISTER_USER_ACK;
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        ackId, new KPacketOK { Ok = dup ? NetError.ERR_VERIFY_10 : NetError.NET_OK });
                break;
            }

            case EventId.ELG_CHECK_MACHINE_ID_REQ:
            {
                var req = ev.GetPayload<KEGS_CHECK_MACHINE_ID_REQ>(Region);
                bool blocked = Db.IsMachineBlocked(req.MachineId);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ELG_CHECK_MACHINE_ID_ACK, new KEGS_CHECK_MACHINE_ID_ACK
                        {
                            Ok = blocked ? NetError.ERR_CHECK_MACHINE_ID_00 : NetError.NET_OK,
                            MachineId = req.MachineId,
                        });
                break;
            }

            case EventId.ELG_CHECK_TIME_RANDOM_ITEM_EVENT_REQ:
            {
                var req = ev.GetPayload<KELG_CHECK_TIME_RANDOM_ITEM_EVENT_REQ>(Region);
                int rewardId = RandomItemEventManager.CheckTimeOpenRandomItemEvent(req.UserUid, req.UnitUid, req.OpenItemId);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ELG_CHECK_TIME_RANDOM_ITEM_EVENT_ACK, new KELG_CHECK_TIME_RANDOM_ITEM_EVENT_ACK
                        {
                            ItemUid = req.OpenItemId,
                            EventItemId = rewardId,
                        });
                break;
            }

            case EventId.ELG_CREATE_GUILD_RESULT_REQ:
            {
                var req = ev.GetPayload<KELG_CREATE_GUILD_RESULT_REQ>(Region);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ELG_CREATE_GUILD_RESULT_ACK, new KPacketOK { Ok = req.Ok });
                break;
            }

            case EventId.ELG_CHANGE_GUILD_NAME_TIME_CHECK_REQ:
            {
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ELG_CHANGE_GUILD_NAME_TIME_CHECK_ACK, new KELG_CHANGE_GUILD_NAME_TIME_CHECK_ACK { Ok = NetError.NET_OK });
                break;
            }

            case EventId.ELG_CHANGE_GUILD_NAME_RESULT_REQ:
            {
                var req = ev.GetPayload<KELG_CHANGE_GUILD_NAME_REQ>(Region);
                var info = GuildManager.GetGuildInfo(Db, Region, req.GuildUid);
                if (info != null)
                {
                    info.GuildName = req.NewGuildName;
                    GuildBoardManager.UpdateGuildName(req.GuildUid, req.NewGuildName);
                }
                break;
            }

            case EventId.ELG_GET_GUILD_SKILL_IN_BOARD_REQ:
            {
                var req = ev.GetPayload<KELG_GET_GUILD_SKILL_REQ>(Region);
                var skills = Db.GetGuildSkills(req.GuildUid);
                var entry = GuildManager.GetGuildEntry(req.GuildUid);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ELG_GET_GUILD_SKILL_IN_BOARD_ACK, new KEGS_GET_GUILD_SKILL_IN_BOARD_ACK
                        {
                            Ok = NetError.NET_OK,
                            GuildSkillInfo = new KGuildSkillInfo
                            {
                                GuildSkill = skills,
                                GuildSPoint = entry?.SkillTree.GuildSPoint ?? 0,
                                GuildCsPoint = entry?.SkillTree.GuildCsPoint ?? 0,
                            },
                        });
                break;
            }

            case EventId.ELG_MODIFY_REG_GUILD_AD_REQ:
            {
                var req = ev.GetPayload<KELG_MODIFY_REG_GUILD_AD_REQ>(Region);
                var guild = GuildManager.GetGuildInfo(Db, Region, req.GuildUid);
                if (guild != null)
                    Db.UpsertGuildAd(Region, req.GuildUid, req.PeriodDays, req.AdMessage);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ELG_MODIFY_REG_GUILD_AD_ACK, new KELG_MODIFY_REG_GUILD_AD_ACK
                        {
                            Ok = guild != null ? NetError.NET_OK : NetError.ERR_GUILD_01,
                            Period = (short)req.PeriodDays,
                            AdMessage = req.AdMessage,
                        });
                break;
            }

            case EventId.DLG_UPDATE_EXPAND_GUILD_MAX_MEMBER_NOT:
            {
                var ack = ev.GetPayload<KDBE_EXPAND_GUILD_MAX_MEMBER_ACK>(Region);
                var info = GuildManager.GetGuildInfo(Db, Region, ack.GuildUid);
                if (info != null && ack.MaxNumMember > info.MaxNumMember)
                {
                    info.MaxNumMember = ack.MaxNumMember;
                    Db.SetGuildMaxMembers(Region, ack.GuildUid, ack.MaxNumMember);
                }
                break;
            }

            case EventId.ELG_INIT_GUILD_SKILL_DEL_ITEM_ACK:
                if (ev.FirstSenderUid > 0 && ev.InProcessPayload != null)
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ELG_INIT_GUILD_SKILL_ACK, ev.InProcessPayload);
                break;

            case EventId.ELG_RESET_GUILD_SKILL_DEL_ITEM_ACK:
            {
                if (ev.TryReadPayload<KDBE_RESET_GUILD_SKILL_ACK>(Region, out var dback))
                {
                    GuildManager.ResetGuildSkill(
                        dback.GuildUid, dback.GuildSkillId, dback.GuildSkillLevel,
                        dback.GuildSkillCSPoint, dback.GuildSPoint, dback.GuildCSPoint);
                }
                if (ev.FirstSenderUid > 0 && ev.InProcessPayload != null)
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ELG_RESET_GUILD_SKILL_ACK, ev.InProcessPayload);
                break;
            }

            case EventId.ELG_INSERT_GUILD_CASH_SKILL_POINT_NOT:
            {
                var not = ev.GetPayload<KELG_INSERT_GUILD_CASH_SKILL_POINT_NOT>(Region);
                GuildManager.InsertGuildCashSkillPoint(not.GuildUid, not.GuildCSPoint, not.Period);
                break;
            }

            case EventId.ELG_INVITE_GUILD_FIND_USER_ACK:
            {
                if (ev.TryReadPayload<KELG_INVITE_GUILD_FIND_USER_ACK>(Region, out var fack))
                {
                    if (fack.SenderUnitUid > 0)
                    {
                        if (fack.EventId == (ushort)EventId.DBE_DELETE_APLLY_FOR_ACCEPT_JOIN_GUILD_ACK)
                        {
                            Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, fack.SenderUnitUid,
                                EventId.ELG_ACCEPT_JOIN_GUILD_ACK, new KEGS_ACCEPT_JOIN_GUILD_ACK
                                {
                                    Ok = fack.Ok,
                                    ReceiverNickName = fack.ReceiverNickName,
                                });
                        }
                        else
                        {
                            Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, fack.SenderUnitUid,
                                EventId.ELG_INVITE_GUILD_ACK, new KEGS_INVITE_GUILD_ACK
                                {
                                    Ok = fack.Ok,
                                    ReceiverNickName = fack.ReceiverNickName,
                                });
                        }
                    }
                    if (fack.Ok == NetError.NET_OK && fack.ReceiverUnitUid > 0)
                    {
                        var gInfo = GuildManager.GetGuildInfo(Db, Region, fack.GuildUid);
                        if (gInfo != null)
                        {
                            string senderNick = string.Empty;
                            if (fack.SenderUnitUid > 0)
                            {
                                senderNick = GuildManager.GetGuildEntry(fack.GuildUid)?.MemberInfos
                                    .GetValueOrDefault(fack.SenderUnitUid)?.NickName ?? string.Empty;
                                if (string.IsNullOrEmpty(senderNick) &&
                                    UserList.TryGetGSUserInfoByUnitUid(fack.SenderUnitUid, out var senderInfo))
                                {
                                    senderNick = senderInfo.NickName;
                                }
                            }
                            Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, fack.ReceiverUnitUid,
                                EventId.ELG_INVITE_GUILD_FIND_USER_REQ, new KEGS_INVITE_GUILD_NOT
                                {
                                    GuildInfo = gInfo,
                                    SenderNickName = senderNick,
                                });
                        }
                    }
                }
                else if (ev.FirstSenderUid > 0 && ev.InProcessPayload != null)
                {
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ELG_INVITE_GUILD_ACK, ev.InProcessPayload);
                }
                break;
            }

            case EventId.ELG_UPDATE_CHANGE_GUILD_MEMBER_MESSAGE_NOT:
            {
                var msgNot = ev.GetPayload<KDBE_CHANGE_GUILD_MEMBER_MESSAGE_ACK>(Region);
                GuildManager.UpdateMemberMessage(msgNot.GuildUid, msgNot.TargetUnitUid, msgNot.Message);
                break;
            }

            case EventId.ELG_UPDATE_GUILD_MEMBER_INFO_NOT:
            {
                var memNot = ev.GetPayload<KELG_UPDATE_GUILD_MEMBER_INFO_NOT>(Region);
                GuildManager.UpdateMemberInfo(memNot.GuildUid, memNot.MemberInfo);
                break;
            }

            case EventId.ELG_INVITE_PARTY_GET_RECEIVER_USER_INFO_ACK:
                if (ev.FirstSenderUid > 0 && ev.InProcessPayload != null)
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ELG_INVITE_PARTY_FIND_RECEIVER_ACK, ev.InProcessPayload);
                break;

            case EventId.ELG_GET_UNIT_INFO_ACK:
            {
                var ack = ev.GetPayload<KELG_GET_UNIT_INFO_ACK>(Region);
                if (ack.DemandUserUid > 0)
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ack.DemandUserUid,
                        EventId.ELG_SEARCH_UNIT_ACK, new KELG_SEARCH_UNIT_ACK
                        {
                            Ok = ack.Ok,
                            EventIdValue = ack.EventId,
                            CUnitInfo = ack.CUnitInfo,
                        });
                break;
            }

            case EventId.ELG_ADMIN_SHOW_ED_CHEAT_ACK:
                if (ev.FirstSenderUid > 0 && ev.InProcessPayload != null)
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ELG_ADMIN_SHOW_ED_CHEAT_ACK, ev.InProcessPayload);
                break;

            case EventId.ELG_ADMIN_NEW_RECORD_TEST_NOT:
            {
                var info = ev.GetPayload<KHenirRankingInfo>(Region);
                HenirRankingManager.CheckNewRecord(info);
                break;
            }

            case EventId.ELG_CLEAR_RANKING_NOT:
                HenirRankingManager.ClearIfDue(DateTime.MaxValue);
                RankingManager.RefreshNow(Db, Region);
                break;

            case EventId.ELG_FIND_DELETE_CHAR_IN_RANKING_NOT:
            {
                var pkt = ev.GetPayload<KPacketLong>(Region);
                HenirRankingManager.DeleteRecord(pkt.Value);
                break;
            }

            case EventId.ELG_GET_WEB_RANKING_REFRESH_NOT:
            case EventId.ELG_RANKING_TITLE_REWARD_NOT:
                RankingManager.RefreshNow(Db, Region);
                break;

            case EventId.ELG_LOCAL_RANKING_GUILD_CHANGE_INFO_NOT:
            {
                var req = ev.GetPayload<KELG_LOCAL_RANKING_GUILD_CHANGE_INFO_NOT>(Region);
                GuildBoardManager.UpdateGuildName(req.GuildUid, req.GuildName);
                break;
            }

            case EventId.ELG_LOCAL_RANKING_SYSTEM_CHECK_NOT:
            case EventId.ELG_LOCAL_RANKING_SYSTEM_INFO_CHECK_NOT:
                LocalRankingManager.TickWeeklyReset();
                break;

            case EventId.ELG_LOCAL_RANKING_WATCH_UNIT_REQ:
            {
                var req = ev.GetPayload<KELG_LOCAL_RANKING_WATCH_UNIT_REQ>(Region);
                bool online = UserList.TryGetGSUserInfoByUnitUid(req.TargetUnitUid, out _);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ELG_LOCAL_RANKING_WATCH_UNIT_ACK, new KELG_LOCAL_RANKING_WATCH_UNIT_ACK
                        {
                            Ok = online ? NetError.NET_OK : NetError.ERR_SEARCH_UNIT_03,
                        });
                break;
            }

            case EventId.ELG_LOCAL_RANKING_WATCH_UNIT_ACK:
                if (ev.FirstSenderUid > 0 && ev.InProcessPayload != null)
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ELG_LOCAL_RANKING_WATCH_UNIT_ACK, ev.InProcessPayload);
                break;

            case EventId.ELG_COUPLE_PROPOSE_CHECK_CONNECT_USER_REQ:
            case EventId.ELG_EVENT_PROPOSE_CHECK_CONNECT_USER_REQ:
            {
                var req = ev.GetPayload<KELG_COUPLE_PROPOSE_CHECK_CONNECT_USER_REQ>(Region);
                bool found = UserList.TryGetGSUserInfoByNickName(req.TargetNickName, out var target);
                EventId ackId = ev.EventId == EventId.ELG_COUPLE_PROPOSE_CHECK_CONNECT_USER_REQ
                    ? EventId.ELG_COUPLE_PROPOSE_CHECK_CONNECT_USER_ACK
                    : EventId.ELG_EVENT_PROPOSE_CHECK_CONNECT_USER_ACK;
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        ackId, new KEGS_COUPLE_PROPOSE_ACK
                        {
                            Ok = found ? NetError.NET_OK : NetError.ERR_SEARCH_UNIT_03,
                            NickName = target?.NickName ?? req.TargetNickName,
                        });
                if (found && target != null)
                {
                    EventId notId = ev.EventId == EventId.ELG_COUPLE_PROPOSE_CHECK_CONNECT_USER_REQ
                        ? EventId.ELG_COUPLE_PROPOSE_NOT
                        : EventId.ELG_EVENT_PROPOSE_NOT;
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, target.GsUserUid,
                        notId, new KEGS_COUPLE_PROPOSE_REQ { UnitName = req.RequestNickName });
                }
                break;
            }

            case EventId.ELG_COUPLE_PROPOSE_AGREE_CHECK_NOT:
            case EventId.ELG_COUPLE_PROPOSE_RESULT_FAIL_NOT:
            case EventId.ELG_COUPLE_PROPOSE_RESULT_SUCCESS_NOT:
            case EventId.ELG_EVENT_PROPOSE_AGREE_CHECK_NOT:
            case EventId.ELG_EVENT_PROPOSE_RESULT_SUCCESS_NOT:
            {
                var req = ev.GetPayload<KELG_COUPLE_PROPOSE_RESULT_NOT>(Region);
                if (req.RequestUnitUid > 0 && UserList.TryGetGSUserInfoByUnitUid(req.RequestUnitUid, out var requester))
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, requester.GsUserUid,
                        ev.EventId, req);
                break;
            }

            case EventId.ELG_RELATIONSHIP_INFO_REQ:
            {
                var req = ev.GetPayload<KEGS_RELATIONSHIP_INFO_REQ>(Region);
                bool online = UserList.TryGetGSUserInfoByUnitUid(req.AcceptUnitUid, out var partner);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ELG_RELATIONSHIP_INFO_ACK, new KEGS_RELATIONSHIP_INFO_ACK
                        {
                            Ok = NetError.NET_OK,
                            AcceptUnitUid = req.AcceptUnitUid,
                            IsConnect = online,
                            ChannelName = online ? $"CH-{partner!.ChannelId}" : string.Empty,
                        });
                break;
            }

            case EventId.ELG_RELATIONSHIP_INFO_GAME_SERVER_ACK:
            {
                if (ev.TryReadPayload<KEGS_RELATIONSHIP_INFO_ACK>(Region, out var relAck) && relAck.RequestUnitUid > 0)
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, relAck.RequestUnitUid,
                        EventId.ELG_RELATIONSHIP_INFO_ACK, relAck);
                break;
            }

            case EventId.ELG_WEDDING_ITEM_FIND_INFO_REQ:
            {
                var req = ev.GetPayload<KELG_WEDDING_ITEM_FIND_INFO_REQ>(Region);
                bool found = WeddingHallManager.TryGetWeddingInfo(req.WeddingUid, out _);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ELG_WEDDING_ITEM_FIND_INFO_ACK, new KPacketOK { Ok = found ? NetError.NET_OK : NetError.ERR_SEARCH_UNIT_03 });
                break;
            }

            case EventId.ELG_FIND_FRIEND_UNIT_INFO_REQ:
            {
                var req = ev.GetPayload<KELG_FIND_FRIEND_UNIT_INFO_REQ>(Region);
                var infos = new List<KConnectionUnitInfo>();
                foreach (long fuid in req.FriendUnitUids)
                {
                    bool on = UserList.TryGetGSUserInfoByUnitUid(fuid, out var u);
                    var unit = Db.GetUnitByUid(Region, fuid);
                    infos.Add(new KConnectionUnitInfo
                    {
                        UnitUid = fuid,
                        CharName = on ? u!.NickName : (unit?.NickName ?? string.Empty),
                        UnitClass = unit?.UnitClass ?? 1,
                        Level = unit?.Level ?? 1,
                        ChannelId = on ? u!.ChannelId : 0,
                        State = on ? (sbyte)1 : (sbyte)0,
                    });
                }
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ELG_FIND_FRIEND_UNIT_INFO_ACK, new KELG_FIND_FRIEND_UNIT_INFO_ACK
                        {
                            Ok = NetError.NET_OK,
                            FriendUnitInfos = infos,
                        });
                break;
            }

            case EventId.ELG_SEARCH_BLACK_LIST_UNIT_REQ:
            {
                var req = ev.GetPayload<KELG_SEARCH_BLACK_LIST_UNIT_REQ>(Region);
                var unit = Db.GetUnitByNickName(Region, req.TargetNickName);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ELG_SEARCH_BLACK_LIST_UNIT_ACK, new KELG_SEARCH_BLACK_LIST_UNIT_ACK
                        {
                            Ok = unit != null ? NetError.NET_OK : NetError.ERR_SEARCH_UNIT_03,
                            UnitUid = unit?.UnitUid ?? 0,
                        });
                break;
            }

            case EventId.ELG_UPDATE_DENY_FRIEND_SHIP_NOT:
            {
                var req = ev.GetPayload<KELG_UPDATE_DENY_FRIEND_SHIP_NOT>(Region);
                Db.SetFriendshipBlock(req.UnitUid, req.FriendUnitUid, req.Deny);
                break;
            }

            case EventId.ELG_GET_RECOMMEND_USER_LIST_REQ:
            {
                var pkt = ev.GetPayload<KPacketLong>(Region);
                var list = Db.GetRecommendedByUsers(Region, pkt.Value);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ELG_GET_RECOMMEND_USER_LIST_ACK, new KEGS_GET_RECOMMEND_USER_LIST_ACK
                        {
                            Ok = NetError.NET_OK,
                            TotalPage = 1,
                            ViewPage = 1,
                            RecommendUserList = list,
                        });
                break;
            }

            case EventId.ELG_GET_RECRUIT_RECRUITER_LIST_REQ:
            {
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ELG_GET_RECRUIT_RECRUITER_LIST_ACK, new KEGS_GET_RECRUIT_RECRUITER_LIST_ACK { Ok = NetError.NET_OK });
                break;
            }

            case EventId.ELG_RECOMMEND_USER_INFO_NOT:
            case EventId.ELG_RECOMMEND_USER_RESULT_NOT:
            {
                var req = ev.GetPayload<KELG_RECOMMEND_USER_INFO_NOT>(Region);
                if (req.TargetUnitUid > 0 && UserList.TryGetGSUserInfoByUnitUid(req.TargetUnitUid, out var target))
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, target.GsUserUid,
                        ev.EventId, req);
                break;
            }

            case EventId.ELG_RECRUIT_RECRUITER_INFO_NOT:
                if (ev.FirstSenderUid > 0 && ev.InProcessPayload != null)
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ELG_RECRUIT_RECRUITER_INFO_NOT, ev.InProcessPayload);
                break;

            case EventId.ELG_REGISTER_RECRUITER_NOT:
            {
                var req = ev.GetPayload<KELG_REGISTER_RECRUITER_NOT>(Region);
                Db.RegisterUnitRecruiter(Region, req.UnitUid, req.RecruiterNickName);
                break;
            }

            case EventId.ELG_KNM_INVITE_REQ:
            case EventId.ELG_KNM_REQUEST_NEW_FRIEND_INFO_REQ:
            {
                var req = ev.GetPayload<KEGS_KNM_REQUEST_NEW_FRIEND_INFO_REQ>(Region);
                bool found = req.UnitUid > 0
                    ? UserList.TryGetGSUserInfoByUnitUid(req.UnitUid, out var target)
                    : UserList.TryGetGSUserInfoByNickName(req.UnitNickName, out target);
                EventId ackId = ev.EventId == EventId.ELG_KNM_INVITE_REQ
                    ? EventId.ELG_KNM_INVITE_ACK
                    : EventId.ELG_KNM_REQUEST_NEW_FRIEND_INFO_ACK;
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        ackId, new KPacketOK { Ok = found ? NetError.NET_OK : NetError.ERR_SEARCH_UNIT_03 });
                break;
            }

            case EventId.ELG_KNM_TOGETHER_REQ:
            {
                var req = ev.GetPayload<KELG_KNM_TOGETHER_REQ>(Region);
                bool found = UserList.TryGetGSUserInfoByUnitUid(req.TargetUnitUid, out _);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ELG_KNM_TOGETHER_ACK, new KEGS_KNM_TOGETHER_ACK
                        {
                            Ok = found ? NetError.NET_OK : NetError.ERR_SEARCH_UNIT_03,
                        });
                break;
            }

            case EventId.ELG_KNM_TOGETHER_BY_UNIT_INFO_ACK:
                if (ev.FirstSenderUid > 0 && ev.InProcessPayload != null)
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ELG_KNM_TOGETHER_ACK, ev.InProcessPayload);
                break;

            case EventId.ELG_REQUEST_TUTORIAL_NOT:
            case EventId.ELG_REQUEST_TUTORIAL_REPLY_NOT:
            {
                var req = ev.GetPayload<KELG_TUTORIAL_NOT>(Region);
                long dest = ev.EventId == EventId.ELG_REQUEST_TUTORIAL_NOT ? req.StudentUnitUid : req.TeacherUnitUid;
                if (dest > 0 && UserList.TryGetGSUserInfoByUnitUid(dest, out var target))
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, target.GsUserUid, ev.EventId, req);
                break;
            }

            case EventId.ELG_TUTORIAL_UPDATE_UNIT_INFO_NOT:
            case EventId.ELG_DEL_TUTORIAL_NOT:
                if (ev.FirstSenderUid > 0 && ev.InProcessPayload != null)
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        ev.EventId, ev.InProcessPayload);
                break;

            case EventId.ELG_INSERT_TITLE_NOT:
            {
                var req = ev.GetPayload<KELG_INSERT_TITLE_NOT>(Region);
                if (req.UnitUid > 0 && req.TitleId > 0) Db.GrantUnitTitle(req.UnitUid, req.TitleId);
                break;
            }

            case EventId.ELG_CHAT_OBSERVATION_NOT:
            {
                var req = ev.GetPayload<KELG_CHAT_OBSERVATION_NOT>(Region);
                KLog.Clog($"[ChatObs] {req.NickName}({req.UnitUid}): {req.Message}");
                break;
            }

            case EventId.ELG_MODULE_INFO_UPDATE_NOT:
            {
                var req = ev.GetPayload<KELG_CHECK_MODULE_INFO_UPDATE_NOT>(Region);
                if (ev.LastSenderUid > 0)
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_SERVER, ev.LastSenderUid,
                        EventId.ELG_CHECK_MODULE_INFO_UPDATE_NOT, req);
                break;
            }

            case EventId.ELG_REG_REJECTED_USER_NOT:
            case EventId.ELG_REG_TRADE_BLOCK_USER_NOT:
            {
                var req = ev.GetPayload<KELG_REG_USER_BLOCK_NOT>(Region);
                var btype = ev.EventId == EventId.ELG_REG_TRADE_BLOCK_USER_NOT
                    ? KAccountBlockManager.BlockType.TradeBlock
                    : KAccountBlockManager.BlockType.AccountBlock;
                AccountBlockManager.SetBlock(Db, Region, req.UserId, btype, req.Block, req.Reason);
                break;
            }

            case EventId.ELG_USE_COUPON_REQ:
            {
                var req = ev.GetPayload<KELG_USE_COUPON_REQ>(Region);
                var (ok, _, _, _) = Db.RedeemCoupon(req.AccountUid, req.CouponSerial, req.CouponType);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.EBILL_USE_COUPON_ACK, new KEBILL_USE_COUPON_ACK { Ok = ok });
                break;
            }

            case EventId.ELG_PRESENT_CASH_ITEM_REQ:
            {
                var req = ev.GetPayload<KEGS_PRESENT_CASH_ITEM_REQ>(Region);
                bool found = UserList.TryGetGSUserInfoByNickName(req.ReceiverNickName, out _) ||
                             Db.GetUnitByNickName(Region, req.ReceiverNickName) != null;
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ELG_PRESENT_CASH_ITEM_ACK, new KELG_PRESENT_CASH_ITEM_ACK
                        {
                            Ok = found ? NetError.NET_OK : NetError.ERR_SEARCH_UNIT_03,
                            ReceiverNickName = req.ReceiverNickName,
                            Message = req.Message,
                        });
                break;
            }

            case EventId.ELG_PRESENT_MESSAGE_TO_RECEIVER_NOT:
            {
                var not = ev.GetPayload<KEGS_PRESENT_MESSAGE_TO_RECEIVER_NOT>(Region);
                if (!string.IsNullOrEmpty(not.ReceiverUserId))
                {
                    var auth = _authedUsers.Values.FirstOrDefault(u =>
                        string.Equals(u.UserId, not.ReceiverUserId, StringComparison.OrdinalIgnoreCase));
                    if (auth != null && UserList.TryGetGSUserInfoByUserUid(auth.AccountUid, out var target))
                        Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, target.GsUserUid,
                            EventId.ELG_PRESENT_MESSAGE_TO_RECEIVER_NOT, not);
                }
                break;
            }

            case EventId.ERM_CHECK_INVALID_USER_NOT:
            {
                if (ev.FirstSenderUid > 0)
                    PersonalShopAgencyManager.RemoveBuyerFromAllShops(ev.FirstSenderUid);
                break;
            }

            case EventId.ERM_CHECK_ZOMBIE_USER_ACK:
            {
                var ack = ev.GetPayload<KERM_CHECK_ZOMBIE_USER_ACK>(Region);
                if (ack.ZombieCheckType is KERM_CHECK_ZOMBIE_USER_ACK.ZCT_CONNECT_ZOMBIE
                    or KERM_CHECK_ZOMBIE_USER_ACK.ZCT_DISCONNECT_ZOMBIE
                    or KERM_CHECK_ZOMBIE_USER_ACK.ZCT_JOIN_DIF_ROOM)
                {
                    if (ev.FirstSenderUid > 0)
                        PersonalShopAgencyManager.RemoveBuyerFromAllShops(ev.FirstSenderUid);
                }
                break;
            }

            case EventId.ERM_LEAVE_PERSONAL_SHOP_REQ:
            case EventId.ERM_LEAVE_PSHOP_AGENCY_REQ:
            {
                if (ev.FirstSenderUid > 0)
                    PersonalShopAgencyManager.RemoveBuyerFromAllShops(ev.FirstSenderUid);
                EventId ackId = ev.EventId == EventId.ERM_LEAVE_PSHOP_AGENCY_REQ
                    ? EventId.ERM_LEAVE_PSHOP_AGENCY_ACK
                    : EventId.ERM_LEAVE_PERSONAL_SHOP_ACK;
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        ackId, new KPacketOK { Ok = NetError.NET_OK });
                break;
            }

            case EventId.ERM_LOAD_PSHOP_BOARD_INFO_NOT:
            {
                if (ev.FirstSenderUid > 0)
                    _ = PersonalShopAgencyManager.GetSellPersonalShopItemInfoList(ev.FirstSenderUid);
                LoginLogCounters.AddOrUpdate(ev.EventId, 1, static (_, c) => c + 1);
                break;
            }

            case EventId.ERM_OPEN_PSHOP_AGENCY_BY_SERVER_NOT:
            {
                var req = ev.GetPayload<KERM_OPEN_PSHOP_AGENCY_BY_SERVER_NOT>(Region);
                DateTime expUtc = DateTime.TryParse(req.AgencyExpirationDate, out var parsedExp)
                    ? parsedExp
                    : DateTime.UtcNow.AddDays(3);
                TimeSpan dur = expUtc > DateTime.UtcNow ? expUtc - DateTime.UtcNow : TimeSpan.FromDays(3);
                PersonalShopAgencyManager.CreateAgencyShop(
                    req.UnitUid,
                    req.NickName,
                    req.PersonalShopName,
                    req.SellItemInfo.Select(s => new KPersonalShopAgencyManager.AgencyShopItem
                    {
                        ItemUid = s.SellPShopItemInfo.InventoryItemInfo.ItemUid,
                        ItemId = s.SellPShopItemInfo.InventoryItemInfo.ItemInfo.ItemId,
                        Quantity = s.SellPShopItemInfo.InventoryItemInfo.ItemInfo.Quantity,
                        PricePerUnit = s.SellPShopItemInfo.PricePerOne,
                        SoldQuantity = s.TotalSoldItemQuantity,
                    }),
                    dur);
                break;
            }

            case EventId.ERM_PSHOP_AGENCY_MESSAGE_NOT:
            {
                var msg = ev.GetPayload<KEGS_PSHOP_AGENCY_MESSAGE_NOT>(Region);
                if (msg.HostUnitUid > 0 && UserList.TryGetGSUserInfoByUnitUid(msg.HostUnitUid, out var hostUser))
                {
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, hostUser.GsUserUid,
                        EventId.ELG_PSHOP_AGENCY_MESSAGE_NOT, msg);
                }
                break;
            }

            case EventId.ERM_UPDATE_PERIOD_PSHOP_AGENCY_NOT:
            {
                var req = ev.GetPayload<KERM_UPDATE_PERIOD_PSHOP_AGENCY_NOT>(Region);
                if (req.HostUnitUid > 0 && req.AddMinutes > 0)
                    PersonalShopAgencyManager.ExtendExpiration(req.HostUnitUid, TimeSpan.FromMinutes(req.AddMinutes));
                break;
            }

            case EventId.EGIANT_INFO_GET_CCU_REQ:
            {
                int ccu = Math.Max(UserList.OnlineCount, ServerCcuManager.GetTotalConcurrentUser());
                if (ev.LastSenderUid > 0)
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_SERVER, ev.LastSenderUid,
                        EventId.EGIANT_INFO_GET_TOTAL_CCU_ACK, new KEGIANT_INFO_GET_TOTAL_CCU_ACK
                        {
                            TimeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                            CCU = (uint)Math.Max(0, ccu),
                        });
                break;
            }

            case EventId.EGIANT_INFO_USERONLINE_REQ:
            {
                var req = ev.GetPayload<KEGIANT_INFO_USERONLINE_REQ>(Region);
                bool online = UserList.TryGetGSUserInfoByUserUid(req.UserUid, out _);
                if (ev.LastSenderUid > 0)
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_SERVER, ev.LastSenderUid,
                        EventId.EGIANT_INFO_USERONLINE_ACK, new KEGIANT_INFO_USERONLINE_ACK
                        {
                            UserOnline = (byte)(online ? 1 : 0),
                        });
                break;
            }

            // ---- Remaining LoginServer TW / Billing / Agency / Diagnostics Handlers ----
            case EventId.ELG_LOCAL_RANKING_RESET_NOT:
            {
                LocalRankingManager.TickWeeklyReset(DateTime.UtcNow.AddDays(8));
                break;
            }

            case EventId.ELG_DUMP_LOGIN_USER_NOT:
            case EventId.DMP_DUMP_RANKING_MANAGER_NOT:
            {
                KLog.Clog($"[{Name}] Dump requested ({ev.EventId}): Online={UserList.OnlineCount}, HenirCount={HenirRankingManager.GetTop(100).Count}");
                break;
            }

            case EventId.ELG_PCBANG_LOGOUT_NOT:
            {
                var not = ev.GetPayload<KELG_PCBANG_LOGOUT_NOT>(Region);
                if (not.UserUid > 0) _pcBangActiveIps.TryRemove(not.UserUid, out _);
                break;
            }

            case EventId.ELG_EXCHANGE_CASH_REQ:
            {
                var req = ev.GetPayload<KELG_EXCHANGE_CASH_REQ>(Region);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.EBILL_EXCHANGE_CASH_ACK, new KEBILL_EXCHANGE_CASH_ACK
                        {
                            Ok = NetError.NET_OK,
                            UnitUid = req.PurchaserInfo.UnitUid,
                            Point = req.Point,
                        });
                break;
            }

            case EventId.EBILL_EXCHANGE_CASH_RESERVE_REQ:
            {
                var req = ev.GetPayload<KEBILL_EXCHANGE_CASH_RESERVE_REQ>(Region);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.EBILL_EXCHANGE_CASH_RESERVE_ACK, new KEBILL_EXCHANGE_CASH_RESERVE_ACK
                        {
                            Ok = NetError.NET_OK,
                            Transaction = Guid.NewGuid().ToString("N")[..16],
                            PacketReq = req,
                        });
                break;
            }

            case EventId.EBILL_EXCHANGE_CASH_RESERVE_ACK:
            {
                var ack = ev.GetPayload<KEBILL_EXCHANGE_CASH_RESERVE_ACK>(Region);
                if (ack.PacketReq.PurchaserInfo.UserUid > 0)
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ack.PacketReq.PurchaserInfo.UserUid,
                        EventId.EBILL_EXCHANGE_CASH_ACK, new KEBILL_EXCHANGE_CASH_ACK
                        {
                            Ok = ack.Ok,
                            UnitUid = ack.PacketReq.PurchaserInfo.UnitUid,
                            Point = ack.PacketReq.Point,
                        });
                break;
            }

            case EventId.EBILL_EXCHANGE_CASH_REQ:
            {
                var req = ev.GetPayload<KEBILL_EXCHANGE_CASH_REQ>(Region);
                if (req.UserUid > 0)
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, req.UserUid,
                        EventId.EBILL_EXCHANGE_CASH_ACK, new KEBILL_EXCHANGE_CASH_ACK
                        {
                            Ok = req.Ret == 0 ? NetError.NET_OK : NetError.ERR_UNKNOWN,
                            UnitUid = req.UserUid,
                            Point = req.Point,
                        });
                break;
            }

            case EventId.EBILL_USE_COUPON_RESERVE_REQ:
            {
                var req = ev.GetPayload<KEBILL_USE_COUPON_REQ>(Region);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.EBILL_USE_COUPON_RESERVE_ACK, new KEBILL_USE_COUPON_RESERVE_ACK
                        {
                            Ok = NetError.NET_OK,
                            PacketReq = req,
                        });
                break;
            }

            case EventId.EBILL_USE_COUPON_RESERVE_ACK:
            {
                var ack = ev.GetPayload<KEBILL_USE_COUPON_RESERVE_ACK>(Region);
                var (ok, _, itemId, qty) = Db.RedeemCoupon(ack.PacketReq.PurchaserInfo.UserUid, ack.PacketReq.SerialCode, ack.PacketReq.CouponType);
                if (ack.PacketReq.PurchaserInfo.UserUid > 0)
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ack.PacketReq.PurchaserInfo.UserUid,
                        EventId.EBILL_USE_COUPON_RESULT_ACK, new KEBILL_USE_COUPON_RESULT_ACK
                        {
                            Ok = ack.Ok != NetError.NET_OK ? ack.Ok : ok,
                            ChargeItem = itemId,
                            ChargeItemCnt = qty,
                        });
                break;
            }

            case EventId.EBILL_USE_COUPON_RESULT_REQ:
            {
                var req = ev.GetPayload<KEBILL_USE_COUPON_RESULT_REQ>(Region);
                if (req.UserUid > 0)
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, req.UserUid,
                        EventId.EBILL_USE_COUPON_RESULT_ACK, new KEBILL_USE_COUPON_RESULT_ACK
                        {
                            Ok = req.Ret == 0 ? NetError.NET_OK : NetError.ERR_UNKNOWN,
                            ChargeItem = req.ChargeItem,
                            ChargeItemCnt = req.ChargeItemCnt,
                            ChargeCash = req.ChargeCash,
                            ChargeBonus = req.ChargeBonus,
                        });
                break;
            }

            case EventId.EBILL_LOGIN_CHECK_BALANCE_REQ:
            {
                var req = ev.GetPayload<KEBILL_LOGIN_CHECK_BALANCE_REQ>(Region);
                if (req.SessionUid > 0)
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, req.SessionUid,
                        EventId.EPUBLISHER_BILLING_BALANCE_ACK, new KEPUBLISHER_BILLING_BALANCE_ACK { Ok = NetError.NET_OK });
                break;
            }

            case EventId.ELOG_INSERT_PERSONAL_SHOP_LOG_NOT:
                LoginLogCounters.AddOrUpdate(ev.EventId, 1, static (_, c) => c + 1);
                break;

            case EventId.ERM_PREPARE_PICK_UP_FROM_PSHOP_AGENCY_ACK:
            {
                var (ok, pickedEd, rem) = PersonalShopAgencyManager.PickUpAndCloseShop(ev.FirstSenderUid);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ERM_PICK_UP_FROM_PSHOP_AGENCY_ACK, new KERM_PICK_UP_FROM_PSHOP_AGENCY_ACK
                        {
                            Ok = ok ? NetError.NET_OK : NetError.ERR_PERSONAL_SHOP_23,
                            PickedEd = pickedEd,
                            RemainingItemCount = rem.Count,
                        });
                break;
            }

            case EventId.EPUBLISHER_BILLING_BALANCE_REQ:
            {
                var req = ev.GetPayload<KEPUBLISHER_BILLING_BALANCE_REQ>(Region);
                if (req.ActorUid > 0)
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, req.ActorUid,
                        EventId.EPUBLISHER_BILLING_BALANCE_ACK, new KEPUBLISHER_BILLING_BALANCE_ACK { Ok = NetError.NET_OK });
                break;
            }

            case EventId.EPUBLISHER_REG_USER_ACK:
            {
                var ack = ev.GetPayload<KEPUBLISHER_REG_USER_ACK>(Region);
                bool wasWaiting = UserList.UnRegAuthWaitUser(ack.ServiceAccountId, out long targetGsUserUid);
                long destUserUid = targetGsUserUid;
                if (destUserUid <= 0 && UserList.TryGetAccountByUserId(ack.ServiceAccountId, out var acct))
                {
                    if (UserList.TryGetGSUserInfoByUserUid(acct.UserUid, out var unitInfo))
                        destUserUid = unitInfo.GsUserUid;
                    else
                        destUserUid = acct.LoginActorUid;
                }
                if (destUserUid <= 0) destUserUid = ev.FirstSenderUid;

                if (destUserUid > 0)
                {
                    var resp = new KELG_REGISTER_USER_GLOBAL_PUBLISHER_ACK
                    {
                        Ok = wasWaiting || ack.Ok == NetError.NET_OK ? ack.Ok : NetError.NET_OK,
                        ServiceAccountId = ack.ServiceAccountId,
                        IsPcBang = ack.IsPcBang,
                        PCBangType = ack.PcBangType,
                        CheckState = ack.CheckState,
                    };
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, destUserUid,
                        EventId.ELG_REGISTER_USER_GLOBAL_PUBLISHER_ACK, resp);
                }
                break;
            }

            case EventId.EPUBLISHER_UNREG_USER_ACK:
                // Intentionally empty in C++ LoginServer.cpp
                break;

            // ---- Region-gated CN / JP / PH Auth & Garena / Nexon Billing Handlers ----
            case EventId.ELG_NEXON_USER_AUTHENTICATE_REQ:
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ELG_NEXON_USER_AUTHENTICATE_ACK, new KELG_NEXON_USER_AUTHENTICATE_ACK { Ok = NetError.NET_OK });
                break;

            case EventId.ELG_CHANNEL_CHANGE_NEXON_USER_AUTHENTICATE_REQ:
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ELG_CHANNEL_CHANGE_NEXON_USER_AUTHENTICATE_ACK, new KELG_CHANNEL_CHANGE_NEXON_USER_AUTHENTICATE_ACK
                        {
                            ConnectAck = new KEGS_CONNECT_ACK { Ok = NetError.NET_OK },
                            UserAuthAck = new KELG_NEXON_USER_AUTHENTICATE_ACK { Ok = NetError.NET_OK },
                        });
                break;

            case EventId.ELG_NEXON_USER_LOGIN_TIME_OUT_NOT:
                if (ev.FirstSenderUid > 0)
                    _authedUsers.TryRemove(ev.FirstSenderUid, out _);
                break;

            case EventId.ELG_CASH_DIRECT_CHARGE_CN_REQ:
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ELG_CASH_DIRECT_CHARGE_CN_ACK, new KEGS_CASH_DIRECT_CHARGE_CN_ACK { Ok = NetError.NET_OK });
                break;

            case EventId.EBILL_GET_TRANS_FOR_CHECK_BALANCE_REQ:
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.EBILL_GET_TRANS_FOR_CHECK_BALANCE_ACK, new KEBILL_GET_TRANS_FOR_CHECK_BALANCE_ACK { Ok = NetError.NET_OK });
                break;

            case EventId.EBILL_GET_TRANS_FOR_EXCHANGE_REQ:
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.EBILL_GET_TRANS_FOR_EXCHANGE_ACK, new KPacketOK { Ok = NetError.NET_OK });
                break;

            case EventId.ELG_VERIFY_GIANT_MATRIX_CARD_REQ:
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.ELG_VERIFY_GIANT_MATRIX_CARD_ACK, new KELG_VERIFY_GIANT_MATRIX_CARD_ACK { Ok = NetError.NET_OK });
                break;

            case EventId.EBILL_GET_TRANS_FOR_CHECK_BALANCE_ACK:
            {
                var ack = ev.GetPayload<KEBILL_GET_TRANS_FOR_CHECK_BALANCE_ACK>(Region);
                long target = ack.CheckGiantBalanceReq.ActorUid > 0 ? ack.CheckGiantBalanceReq.ActorUid : ev.FirstSenderUid;
                if (target > 0)
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, target,
                        EventId.EPUBLISHER_BILLING_BALANCE_ACK, new KEPUBLISHER_BILLING_BALANCE_ACK { Ok = ack.Ok });
                break;
            }

            case EventId.EBILL_GN_CHANGE_GAME_CURRENCY_FAIL_NOT:
                LoginLogCounters.AddOrUpdate(ev.EventId, 1, static (_, c) => c + 1);
                break;

            case EventId.EBILL_GN_CHANGE_GAME_CURRENCY_REQ:
                LoginLogCounters.AddOrUpdate(ev.EventId, 1, static (_, c) => c + 1);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.EBILL_GN_CHANGE_GAME_CURRENCY_ACK, new KPacketOK { Ok = NetError.NET_OK });
                break;

            case EventId.EBILL_GN_TRANSACTION_CHECK_REQ:
                LoginLogCounters.AddOrUpdate(ev.EventId, 1, static (_, c) => c + 1);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.EBILL_GN_TRANSACTION_CHECK_ACK, new KPacketOK { Ok = NetError.NET_OK });
                break;

            case EventId.EBILL_LOGIN_ACCOUNT_USER_OFFLINE_GN_ACCOUNT_CHECK_REQ:
                LoginLogCounters.AddOrUpdate(ev.EventId, 1, static (_, c) => c + 1);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.EBILL_LOGIN_ACCOUNT_USER_OFFLINE_GN_ACCOUNT_CHECK_ACK, new KPacketOK { Ok = NetError.NET_OK });
                break;

            case EventId.EBILL_LOGIN_ACCOUNT_USER_OFFLINE_GN_EXCHANGE_REQ:
                LoginLogCounters.AddOrUpdate(ev.EventId, 1, static (_, c) => c + 1);
                if (ev.FirstSenderUid > 0)
                    Router.Send(PerformerId.PI_LOGIN_SERVER, Uid, PerformerId.PI_GS_USER, ev.FirstSenderUid,
                        EventId.EBILL_LOGIN_ACCOUNT_USER_OFFLINE_GN_EXCHANGE_ACK, new KPacketOK { Ok = NetError.NET_OK });
                break;

            case EventId.ELG_GN_CHANGE_GAME_CURRENCY_NOT:
                LoginLogCounters.AddOrUpdate(ev.EventId, 1, static (_, c) => c + 1);
                break;
        }
    }

    public override bool DestroyAllSession()
    {
        _simLayer.ActorManager.DestroyAll();
        return true;
    }

    public override void ShutDown()
    {
        if (!IsRunning) return;
        IsRunning = false;
        try { _tickCts?.Cancel(); } catch { }
        try { _tickTask?.Wait(1500); } catch { }
        try { ScriptRefreshManager.StopAsync().GetAwaiter().GetResult(); } catch { }
        _authedUsers.Clear();
        base.ShutDown();
        ServerReady = false;
    }
}
