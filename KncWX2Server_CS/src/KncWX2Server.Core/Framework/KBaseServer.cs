using KncWX2Server.Core.Database;
using KncWX2Server.Core.Events;
using KncWX2Server.Core.Logging;
using KncWX2Server.Core.Packets;
using KncWX2Server.Core.Region;
using KncWX2Server.Core.Scripting;

namespace KncWX2Server.Core.Framework;

public enum ServerClass
{
    Center = 1,
    Login = 2,
    Game = 3,
    Channel = 4,
    Simulation = 5,
    Global = 6,
}

/// <summary>
/// Port of Common/BaseServer.{h,cpp} providing shared server lifecycle, Lua config bootstrap,
/// in-process routing, DB dispatching, and shutdown handling.
/// </summary>
public abstract class KBaseServer : KPerformer, IDisposable
{
    public RegionProfile Region { get; set; } = RegionProfile.Current;
    public int ServerGroupId { get; set; }
    public ServerClass ServerClass { get; set; } = ServerClass.Channel;
    public int DefaultMaxUser { get; set; } = 3000;
    public string PublicIp { get; set; } = "127.0.0.1";
    public string PrivateIp { get; set; } = "127.0.0.1";
    public uint ShutDownWaitMs { get; set; } = 60_000;
    public string MailingList { get; set; } = string.Empty;
    public bool ScriptParsingErrReport { get; set; }
    public int UseVersion { get; set; } = 2; // VE_SERVICE
    public bool UseSecondVersion { get; set; }
    public Dictionary<int, string> MainVersions { get; } = new();
    public int PassportLevel { get; set; } = 2;
    public string HackShieldHsbFilePath { get; set; } = string.Empty;
    public List<TimeSpan> ShutDownMessageTimes { get; } = [];
    public List<string> FailScriptFiles { get; } = [];
    public List<string> PortCheckMonitoringIps { get; } = [];
    public bool ServerReady { get; protected set; }

    public InProcessRouter Router { get; }
    public KScriptManager ScriptManager { get; } = new();
    public KDBLayer DbLayer { get; } = new();
    public KStatisticsManager Statistics { get; } = new();
    public RegionalAuthBillingRegistry RegionalAuthBilling { get; } = new();
    public KActionDecider ActionDecider { get; } = new();

    private readonly bool _ownsRouter;

    protected KBaseServer(InProcessRouter? sharedRouter = null)
    {
        _ownsRouter = sharedRouter == null;
        Router = sharedRouter ?? new InProcessRouter();
    }

    protected readonly object ServerListLock = new();
    protected Dictionary<long, KServerInfo> ServerList { get; } = new();

    public override uint PerformerTypeId => PerformerId.PI_CHANNEL_SERVER;

    public abstract KSimLayer SimLayer { get; }
    public abstract KNetLayer NetLayer { get; }
    public abstract bool DestroyAllSession();

    public virtual bool Init()
    {
        RegionProfile.SetCurrent(Region);
        Router.RegisterPerformer(PerformerTypeId, this);
        Router.RegisterChannelActorManager(SimLayer.ActorManager);
        return true;
    }

    public virtual void RegToLua()
    {
        ScriptManager.RegisterBaseServer(this);
        ScriptManager.RegisterGlobal("StatisticsManager", Statistics);
        ScriptManager.RegisterGlobal("ActionDecider", ActionDecider);
        SimLayer.RegToLua(ScriptManager);
        NetLayer.RegToLua(ScriptManager);
        DbLayer.RegToLua(ScriptManager);
        RegionalAuthBilling.RegToLua(ScriptManager);
    }

    public virtual void OnServerReadyComplete()
    {
        ServerReady = true;
        KLog.Cout($"[{GetType().Name}] Server ready complete! Region={Region.Code}, ServerGroupID={ServerGroupId}, UID={Uid}");
    }

    public virtual void ShutDown()
    {
        KLog.Cout($"[{GetType().Name}] ShutDown started...");
        NetLayer.ShutDown();
        DestroyAllSession();
        SimLayer.ShutDown();
        DbLayer.ShutDown();
        if (_ownsRouter)
            Router.StopTcpBridge();
    }

    public virtual void Dispose()
    {
        ShutDown();
        GC.SuppressFinalize(this);
    }

    public void SetServerInfo(KServerInfo info)
    {
        Uid = info.Uid;
        Name = info.Name;
        ServerGroupId = info.ServerGroupId;
        ServerClass = (ServerClass)info.ServerClass;
        PublicIp = string.IsNullOrEmpty(info.PublicIp) ? info.Ip : info.PublicIp;
        PrivateIp = string.IsNullOrEmpty(info.PrivateIp) ? PublicIp : info.PrivateIp;
        DefaultMaxUser = info.MaxUser;
    }

    public void SendToAccountDB<T>(EventId eventId, T payload)
        => Router.Send(PerformerTypeId, Uid, PerformerId.PI_CHANNEL_ACCOUNT_DB, 0, eventId, payload);

    public void SendToAccountDB(EventId eventId)
        => Router.SendNoParam(PerformerTypeId, Uid, PerformerId.PI_CHANNEL_ACCOUNT_DB, 0, eventId);

    public void SendToLogDB<T>(EventId eventId, T payload)
        => Router.Send(PerformerTypeId, Uid, PerformerId.PI_CHANNEL_LOG_DB, 0, eventId, payload);

    public void SendToLogDB(EventId eventId)
        => Router.SendNoParam(PerformerTypeId, Uid, PerformerId.PI_CHANNEL_LOG_DB, 0, eventId);

    public void SendToSmsDB<T>(EventId eventId, T payload)
        => Router.Send(PerformerTypeId, Uid, PerformerId.PI_CHANNEL_SMS_DB, 0, eventId, payload);

    public long GetFirstActorKey() => SimLayer.ActorManager.GetFirstActorKey() ?? 0;

    // Lua-exposed helpers
    public void SetMailingList(string list) => MailingList = list;
    public void SetShutDownWait(uint ms) => ShutDownWaitMs = ms;
    public void SetScriptParsingErrReport(bool val) => ScriptParsingErrReport = val;
    public void AddScriptParsingErrReportMail(string mail) { }
    public void AddPortCheckMoritoringServer(string ip) => PortCheckMonitoringIps.Add(ip);
    public bool IsMonitoringServerForPortCheck(string ip) => PortCheckMonitoringIps.Contains(ip);
    public void SetConnectionMornitoringEnable(bool val) { }
    public void SetMonitoringLetterAndTradeSMS(bool val) { }
    public int GetServerGroupID() => ServerGroupId;
    public void SetUseVersion(int versionEnum) => UseVersion = versionEnum;
    public void SetSecondVersion(bool useSecond) => UseSecondVersion = useSecond;
    public void SetMainVersion(int index, string version) => MainVersions[index] = version ?? string.Empty;
    public void SetPassportLevel(int level) => PassportLevel = level;
    public void SetHackShieldHSBFilePath(string path) => HackShieldHsbFilePath = path ?? string.Empty;
    public void AddMSGBeforShutDown(int hour, int minute, int second)
    {
        if (hour is < 0 or > 23 || minute is < 0 or > 59 || second is < 0 or > 60)
        {
            KLog.Cerr($"Invalid shutdown message time {hour}:{minute}:{second}");
            return;
        }
        ShutDownMessageTimes.Add(new TimeSpan(hour, minute, Math.Min(second, 59)));
    }
}
