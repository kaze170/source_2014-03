using KncWX2Server.Core.Region;

namespace KncWX2Server.Core.Framework;

/// <summary>
/// Port of Common/SimLayer.{h,cpp} managing AuthFlag, ActorManager, script paths, and common flags.
/// </summary>
public abstract class KSimLayer
{
    public KActorManager ActorManager { get; } = new();

    public AuthFlag AuthFlag { get; set; } = AuthFlag.GlobalService;
    public BillingFlag BillingFlag { get; set; } = BillingFlag.GlobalService;
    public VersionFlag VersionFlag { get; set; } = VersionFlag.Service;
    public NationFlag NationFlag { get; set; } = NationFlag.TW;
    public PublisherBillingConnectType PublisherBillingConnectType { get; set; } = PublisherBillingConnectType.Normal;
    public bool UseKogOtp { get; set; } = true;
    public bool CheckCouponByPublisher { get; set; }
    public bool EnableAuthWait { get; set; } = true;
    public bool PublisherCheckGameServerLogin { get; set; } = true;
    public List<string> ScriptPaths { get; } = [];
    private readonly HashSet<uint> _commonFlags = new();
    private readonly object _commonFlagSync = new();

    public void AddCommonFlag(uint flag)
    {
        lock (_commonFlagSync) _commonFlags.Add(flag);
    }

    public void DeleteCommonFlag(uint flag)
    {
        lock (_commonFlagSync) _commonFlags.Remove(flag);
    }

    public bool CheckCommonFlag(uint flag)
    {
        lock (_commonFlagSync) return _commonFlags.Contains(flag);
    }

    public int MaxTickCount { get; protected set; }
    public DateTime MaxTickRegDate { get; protected set; } = DateTime.Now;
    public int CheckIpMode { get; set; }
    public int CheckIpType { get; set; }
    public int CheckModeCheckIpType { get; set; }
    public int HanInitFlag { get; set; } = 1;
    public int ServerGroupNum { get; set; } = 1;
    public List<string> AllowedIps { get; } = [];
    public List<string> CheckModeIps { get; } = [];

    public void SetAuthFlag(int flag) => AuthFlag = (AuthFlag)flag;
    public int GetAuthFlag() => (int)AuthFlag;
    public void SetBillingFlag(int flag) => BillingFlag = (BillingFlag)flag;
    public int GetBillingFlag() => (int)BillingFlag;
    public void SetVersionFlag(int flag) => VersionFlag = (VersionFlag)flag;
    public void SetNationFlag(int flag) => NationFlag = (NationFlag)flag;
    public int GetNationFlag() => (int)NationFlag;
    public uint GetDwNationFlag() => (uint)NationFlag;
    public void SetPublisherBillingConnectType(int flag) => PublisherBillingConnectType = (PublisherBillingConnectType)flag;
    public void SetUseKogOTP(bool use) => UseKogOtp = use;
    public void SetCheckCouponByPublisher(bool check) => CheckCouponByPublisher = check;
    public void SetEnableAuthWait(bool enable) => EnableAuthWait = enable;
    public void SetPublisherCheckGameServerLogin(bool enable) => PublisherCheckGameServerLogin = enable;
    public void SetCheckIPMode(int mode) => CheckIpMode = mode;
    public void SetCheckIPType(int type) => CheckIpType = type;
    public void SetCheckModeCheckIPType(int type) => CheckModeCheckIpType = type;
    public void SetHanInitFlag(int flag) => HanInitFlag = flag;
    public void SetServerGroupNum(int num) => ServerGroupNum = num;
    public void AddIP(string ip)
    {
        if (!string.IsNullOrWhiteSpace(ip) && !AllowedIps.Contains(ip, StringComparer.OrdinalIgnoreCase))
            AllowedIps.Add(ip);
    }
    public void AddCheckModeIP(string ip)
    {
        if (!string.IsNullOrWhiteSpace(ip) && !CheckModeIps.Contains(ip, StringComparer.OrdinalIgnoreCase))
            CheckModeIps.Add(ip);
    }
    public void AddPath(string path)
    {
        if (!string.IsNullOrWhiteSpace(path) && !ScriptPaths.Contains(path, StringComparer.OrdinalIgnoreCase))
            ScriptPaths.Add(path);
    }

    public virtual bool Init() => true;
    public virtual void RegToLua(Scripting.KScriptManager lua) { }
    public virtual void Tick() => ActorManager.Tick();
    public virtual void ShutDown() => ActorManager.DestroyAll();
}
