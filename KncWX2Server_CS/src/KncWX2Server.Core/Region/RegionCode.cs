namespace KncWX2Server.Core.Region;

/// <summary>
/// Supported game service regions for runtime multi-region switching in a single binary.
/// </summary>
public enum RegionCode
{
    TW = 0,
    HK = 1,
    CN = 2,
    JP = 3,
}

public enum AuthFlag
{
    Internal = 0,       // AF_INTERNAL
    NexonKorea = 1,     // AF_NEXON_KOREA
    GlobalService = 2,  // AF_GLOBAL_SERVICE
}

public enum NationFlag
{
    None = 0,   // NF_NONE
    TW = 1,     // NF_TW
    HK = 2,     // NF_HK
    CN = 3,     // NF_CN
    JP = 4,     // NF_JP
    US = 5,     // NF_US
    DE = 6,     // NF_DE
    FR = 7,     // NF_FR
    IT = 8,     // NF_IT
    PL = 9,     // NF_PL
    ES = 10,    // NF_ES
    TR = 11,    // NF_TR
    EN = 12,    // NF_EN
    KR = 13,    // NF_KR
    VN = 14,    // NF_VN
    TH = 15,    // NF_TH
    ID = 16,    // NF_ID
    BR = 17,    // NF_BR
    PH = 18,    // NF_PH
    IN = 19,    // NF_IN
}

public enum BillingFlag
{
    None = 0,               // BF_NONE
    Internal = 1,           // BF_INTERNAL
    NexonKoreaTest = 2,     // BF_NEXON_KOREA_TEST
    NexonKorea = 3,         // BF_NEXON_KOREA
    GlobalService = 4,      // BF_GLOBAL_SERVICE
}

public enum VersionFlag
{
    None = 0,               // VF_NONE
    Internal = 1,           // VF_INTERNAL
    OpenTest = 2,           // VF_OPEN_TEST
    Service = 3,            // VF_SERVICE
}

[Flags]
public enum ClientFlag : uint
{
    None = 0x00000000,      // CF_NONE
    Npgg = 0x00000001,      // CF_NPGG
    CheckIp = 0x00000002,   // CF_CHECK_IP
    HShield = 0x00000004,   // CF_HSHIELD
    XTrap = 0x00000008,     // CF_XTRAP
}

public enum PublisherBillingConnectType
{
    None = 0,       // PBCT_NONE
    Normal = 1,     // PBCT_NORMAL
    Db = 2,         // PBCT_DB
    Unusual = 3,    // PBCT_UNUSUAL
}

public enum ChannelingCode
{
    Kog = 0,
    Nexon = 1,
    GameForge = 2,
    Gamania = 3,
    NhNJapan = 4,
    Giant = 5,
}
