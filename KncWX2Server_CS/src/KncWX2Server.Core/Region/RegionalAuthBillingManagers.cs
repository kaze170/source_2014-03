using KncWX2Server.Core.Logging;
using KncWX2Server.Core.Scripting;

namespace KncWX2Server.Core.Region;

public abstract class KRegionalThreadManagerBase
{
    public int ThreadCount { get; protected set; }
    public virtual string ManagerName => GetType().Name;

    public virtual void Init(int nThreadNum) => ThreadCount = Math.Max(1, nThreadNum);
    public void SetNumThread(int nThreadNum) => Init(nThreadNum);
    public virtual void dump() => KLog.Cout($"[{ManagerName}] Threads={ThreadCount}");
}

public abstract class KSoapAuthBillingManagerBase : KRegionalThreadManagerBase
{
    public string WsdlAddress { get; protected set; } = string.Empty;
    public Dictionary<int, string> WebMethods { get; } = new();

    public void AddWebServiceAddress(string szWSDL) => WsdlAddress = szWSDL ?? string.Empty;
    public void AddWebMethod(int iSoapAction, string szMethod) => WebMethods[iSoapAction] = szMethod ?? string.Empty;

    public override void dump()
        => KLog.Cout($"[{ManagerName}] Threads={ThreadCount}, WSDL={WsdlAddress}, Methods={WebMethods.Count}");
}

public class KGiantBaseManager : KRegionalThreadManagerBase
{
    public string ServerIp { get; protected set; } = string.Empty;
    public ushort Port { get; protected set; }
    public int HeartBitInterval { get; protected set; } = 30_000;
    public int ConnectionCheckInterval { get; protected set; } = 10_000;

    public void SetAddress(string szServerIP, ushort usPort)
    {
        ServerIp = szServerIP ?? string.Empty;
        Port = usPort;
    }

    public void SetHeartBitInterval(int time) => HeartBitInterval = time;
    public void SetConnectionCheckInterval(int time) => ConnectionCheckInterval = time;

    public override void dump()
        => KLog.Cout($"[{ManagerName}] Address={ServerIp}:{Port}, Threads={ThreadCount}, HB={HeartBitInterval}, ConnCheck={ConnectionCheckInterval}");
}

public sealed class KGiantAuthConfigManager : KGiantBaseManager { }
public sealed class KGiantBillingConfigManager : KGiantBaseManager { }
public sealed class KGiantCouponConfigManager : KGiantBaseManager { }
public sealed class KGiantInfoConfigManager : KGiantBaseManager { }
public sealed class KGiantRoleRegConfigManager : KGiantBaseManager { }

public sealed class KPayletterBillingConfigManager : KRegionalThreadManagerBase
{
    public string BillingIp { get; private set; } = string.Empty;
    public ushort Port { get; private set; }
    public int Domain { get; private set; }

    public void InitPayletterBillingInfo(string szNexonAuthIP, ushort usPort, int iDomain)
    {
        BillingIp = szNexonAuthIP ?? string.Empty;
        Port = usPort;
        Domain = iDomain;
    }

    public override void dump()
        => KLog.Cout($"[{ManagerName}] IP={BillingIp}:{Port}, Domain={Domain}, Threads={ThreadCount}");
}

public sealed class KLevelUpAuthConfigManager : KSoapAuthBillingManagerBase { }
public sealed class KGameForgeAuthConfigManager : KSoapAuthBillingManagerBase { }
public sealed class KGameForgeBillingConfigManager : KSoapAuthBillingManagerBase { }
public sealed class KK3RComboAuthConfigManager : KSoapAuthBillingManagerBase { }

public sealed class KCJBillingConfigManager : KRegionalThreadManagerBase
{
    public string BillingIp { get; private set; } = string.Empty;
    public ushort Port { get; private set; }

    public void InitCJBillingInfo(string szNexonAuthIP, ushort usPort)
    {
        BillingIp = szNexonAuthIP ?? string.Empty;
        Port = usPort;
    }
}

public sealed class KFunizenAuthConfigManager : KRegionalThreadManagerBase { }
public sealed class KNaunBillingConfigManager : KRegionalThreadManagerBase { }
public sealed class KLoginNetCafeConfigManager : KRegionalThreadManagerBase { }
public sealed class KPurpleAuthConfigManager : KRegionalThreadManagerBase { }
public sealed class KPurpleBillingConfigManager : KRegionalThreadManagerBase { }

public sealed class KGarenaBillingServerConfig : KRegionalThreadManagerBase
{
    public int WorkerThreads { get; private set; }
    public void WorkerThreadCount(int iWorkerThreadCount) => WorkerThreads = Math.Max(1, iWorkerThreadCount);
}

public sealed class KAsiaSoftAuthConfigManager : KRegionalThreadManagerBase
{
    public string ServiceCode { get; private set; } = string.Empty;
    public string ServiceRegion { get; private set; } = string.Empty;
    public (string Ip, ushort Port) AuthAddress { get; private set; }
    public (string Ip, ushort Port) AKeyAddress { get; private set; }
    public (string Ip, ushort Port) IpBonusAddress { get; private set; }

    public void InitAsiaSoftAuthValue(string szServiceCode, string szServiceRegion)
    {
        ServiceCode = szServiceCode ?? string.Empty;
        ServiceRegion = szServiceRegion ?? string.Empty;
    }

    public void InitAsiaSoftAuthAddress(string szGconsoleIP, ushort usPort) => AuthAddress = (szGconsoleIP ?? string.Empty, usPort);
    public void InitAsiaSoftAKeyAddress(string szGconsoleIP, ushort usPort) => AKeyAddress = (szGconsoleIP ?? string.Empty, usPort);
    public void InitAsiaSoftIPBonusAddress(string szGconsoleIP, ushort usPort) => IpBonusAddress = (szGconsoleIP ?? string.Empty, usPort);
}

public sealed class KGashAuthConfigManager : KRegionalThreadManagerBase
{
    public string ServiceCode { get; private set; } = string.Empty;
    public string ServiceRegion { get; private set; } = string.Empty;
    public string GconsoleIp { get; private set; } = string.Empty;
    public ushort AuthenticationPort { get; private set; }
    public ushort AuthorizationPort { get; private set; }
    public ushort AccountingPort { get; private set; }

    public void InitGASHAuthValue(string szServiceCode, string szServiceRegion)
    {
        ServiceCode = szServiceCode ?? string.Empty;
        ServiceRegion = szServiceRegion ?? string.Empty;
    }

    public void InitGASHAuthAddress(string szGconsoleIP, ushort usAuthenticationPort, ushort usAuthorizationPort, ushort usAccountingPort)
    {
        GconsoleIp = szGconsoleIP ?? string.Empty;
        AuthenticationPort = usAuthenticationPort;
        AuthorizationPort = usAuthorizationPort;
        AccountingPort = usAccountingPort;
    }
}

public sealed class KGashBillingTcpConfigManager : KRegionalThreadManagerBase
{
    public string ServiceCode { get; private set; } = string.Empty;
    public string ServiceRegion { get; private set; } = string.Empty;
    public string GconsoleIp { get; private set; } = string.Empty;
    public ushort AuthorizationPort { get; private set; }
    public ushort AccountingPort { get; private set; }

    public void InitGASHBillingTCPValue(string szServiceCode, string szServiceRegion)
    {
        ServiceCode = szServiceCode ?? string.Empty;
        ServiceRegion = szServiceRegion ?? string.Empty;
    }

    public void InitGASHBillingTCPAddress(string szGconsoleIP, ushort usAuthorizationPort, ushort usAccountingPort)
    {
        GconsoleIp = szGconsoleIP ?? string.Empty;
        AuthorizationPort = usAuthorizationPort;
        AccountingPort = usAccountingPort;
    }
}

public sealed class KNexonAuthConfigManager : KRegionalThreadManagerBase
{
    public string AuthIp { get; private set; } = string.Empty;
    public ushort Port { get; private set; }
    public int Domain { get; private set; }
    public string DomainName { get; private set; } = string.Empty;
    public int SynchronizeType { get; private set; }
    public int SynchronizeCount { get; private set; }
    public bool Disconnected { get; private set; }

    public void InitNexonAuthInfo(string szNexonAuthIP, ushort usPort, int iDomain, string szDomainName, int iSynchronizeType = 0, int iSynchronizeCount = 0)
    {
        AuthIp = szNexonAuthIP ?? string.Empty;
        Port = usPort;
        Domain = iDomain;
        DomainName = szDomainName ?? string.Empty;
        SynchronizeType = iSynchronizeType;
        SynchronizeCount = iSynchronizeCount;
    }

    public void Disconnect() => Disconnected = true;
}

public sealed class KNexonBillingAuthConfigManager : KRegionalThreadManagerBase
{
    public string AuthIp { get; private set; } = string.Empty;
    public ushort Port { get; private set; }
    public int Domain { get; private set; }
    public string DomainName { get; private set; } = string.Empty;

    public void InitNexonBillingAuthInfo(string szNexonAuthIP, ushort usPort, int iDomain, string szDomainName)
    {
        AuthIp = szNexonAuthIP ?? string.Empty;
        Port = usPort;
        Domain = iDomain;
        DomainName = szDomainName ?? string.Empty;
    }
}

public sealed class KNexonBillingConfigManager : KRegionalThreadManagerBase
{
    public Dictionary<int, (string Wsdl, string Method)> Services { get; } = new();

    public void AddWebServiceAddress(int iSoapAction, string szWSDL, string szMethod)
        => Services[iSoapAction] = (szWSDL ?? string.Empty, szMethod ?? string.Empty);
}

public sealed class KNexonBillingTcpConfigManager : KRegionalThreadManagerBase
{
    public string BillingIp { get; private set; } = string.Empty;
    public ushort Port { get; private set; }
    public int Domain { get; private set; }

    public void InitNexonBillingTCPInfo(string szNexonAuthIP, ushort usPort, int iDomain)
    {
        BillingIp = szNexonAuthIP ?? string.Empty;
        Port = usPort;
        Domain = iDomain;
    }
}

public sealed class KNexonSoapConfigManager : KSoapAuthBillingManagerBase
{
    public string SoapServerAddressForUser { get; private set; } = string.Empty;
    public string SoapServerAddressForCoupon { get; private set; } = string.Empty;

    public void Init(int nThreadNum, string? szSoapServerAddressForUser, string? szSoapServerAddressForCoupon = null)
    {
        base.Init(nThreadNum);
        SoapServerAddressForUser = szSoapServerAddressForUser ?? string.Empty;
        SoapServerAddressForCoupon = szSoapServerAddressForCoupon ?? string.Empty;
    }
}

/// <summary>
/// Holds and binds all 21 OnlyGlobal/AuthAndBilling/ regional managers + KR Nexon managers
/// to Lua with exact C++ RegToLua() global names and enum constants.
/// </summary>
public sealed class RegionalAuthBillingRegistry
{
    public KPayletterBillingConfigManager PayletterBillingManager { get; } = new();
    public KLevelUpAuthConfigManager LevelUpAuthManager { get; } = new();
    public KGiantAuthConfigManager GiantAuthManager { get; } = new();
    public KGiantBillingConfigManager GiantBillingManager { get; } = new();
    public KGiantCouponConfigManager GiantCouponManager { get; } = new();
    public KGiantInfoConfigManager GiantInfoManager { get; } = new();
    public KGiantRoleRegConfigManager GiantRoleRegManager { get; } = new();
    public KGameForgeAuthConfigManager GFAuthManager { get; } = new();
    public KGameForgeBillingConfigManager GFBillingManager { get; } = new();
    public KCJBillingConfigManager CJBillingManager { get; } = new();
    public KFunizenAuthConfigManager FunizenAuthManager { get; } = new();
    public KNaunBillingConfigManager NaunBillingManager { get; } = new();
    public KLoginNetCafeConfigManager LoginNetCafeManager { get; } = new();
    public KPurpleAuthConfigManager PurpleAuthManager { get; } = new();
    public KPurpleBillingConfigManager PurpleBillingManager { get; } = new();
    public KGarenaBillingServerConfig GarenaBillingServer { get; } = new();
    public KAsiaSoftAuthConfigManager AsiaSoftAuthManager { get; } = new();
    public KGashAuthConfigManager GASHAuthManager { get; } = new();
    public KGashBillingTcpConfigManager GASHBillingTCPManager { get; } = new();
    public KK3RComboAuthConfigManager K3RComboAuthManager { get; } = new();
    public KNexonAuthConfigManager NXAuthManager { get; } = new();
    public KNexonBillingAuthConfigManager NXBillingAuthManager { get; } = new();
    public KNexonBillingConfigManager NXBillingManager { get; } = new();
    public KNexonBillingTcpConfigManager NXBillingTCPManager { get; } = new();
    public KNexonSoapConfigManager NexonSOAP { get; } = new();

    public void RegToLua(KScriptManager lua)
    {
        lua.RegisterGlobal("PayletterBillingManager", PayletterBillingManager);
        lua.RegisterGlobal("LevelUpAuthManager", LevelUpAuthManager);
        lua.RegisterGlobal("GiantAuthManager", GiantAuthManager);
        lua.RegisterGlobal("GiantBillingManager", GiantBillingManager);
        lua.RegisterGlobal("GiantCouponManager", GiantCouponManager);
        lua.RegisterGlobal("GiantInfoManager", GiantInfoManager);
        lua.RegisterGlobal("GiantRoleRegManager", GiantRoleRegManager);
        lua.RegisterGlobal("GFAuthManager", GFAuthManager);
        lua.RegisterGlobal("GFBillingManager", GFBillingManager);
        lua.RegisterGlobal("CJBillingManager", CJBillingManager);
        lua.RegisterGlobal("FunizenAuthManager", FunizenAuthManager);
        lua.RegisterGlobal("NaunBillingManager", NaunBillingManager);
        lua.RegisterGlobal("LoginNetCafeManager", LoginNetCafeManager);
        lua.RegisterGlobal("PurpleAuthManager", PurpleAuthManager);
        lua.RegisterGlobal("PurpleBillingManager", PurpleBillingManager);
        lua.RegisterGlobal("KGarenaBillingServer", GarenaBillingServer);
        lua.RegisterGlobal("AsiaSoftAuthManager", AsiaSoftAuthManager);
        lua.RegisterGlobal("GASHAuthManager", GASHAuthManager);
        lua.RegisterGlobal("GASHBillingTCPManager", GASHBillingTCPManager);
        lua.RegisterGlobal("K3RComboAuthManager", K3RComboAuthManager);
        lua.RegisterGlobal("NXAuthManager", NXAuthManager);
        lua.RegisterGlobal("NXBillingAuthManager", NXBillingAuthManager);
        lua.RegisterGlobal("NXBillingManager", NXBillingManager);
        lua.RegisterGlobal("NXBillingTCPManager", NXBillingTCPManager);
        lua.RegisterGlobal("NexonSOAP", NexonSOAP);
    }
}
