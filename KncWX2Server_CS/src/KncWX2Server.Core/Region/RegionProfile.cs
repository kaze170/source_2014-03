using System.Collections.Concurrent;

namespace KncWX2Server.Core.Region;

/// <summary>
/// Encapsulates compile-time C++ region/feature macros (#ifdef SERV_COUNTRY_*, ServerDefine_*.h)
/// as runtime configuration so TW, HK, CN, and JP operate inside one binary.
/// </summary>
public sealed class RegionProfile
{
    public RegionCode Region { get; }
    public string Code { get; }
    public string DisplayName { get; }

    // KncUid bitmask layout (CN uses EXTEND_SERVER_GROUP_MASK: 36-bit PureUID, 10-bit ServerGroupID)
    public bool ExtendServerGroupMask { get; init; }

    // Wire packet field toggles from ServerDefine_{Global,TWHK,CN,JP}.h & ClientPacket.cpp
    public bool GlobalAuth { get; init; } = true;                 // SERV_GLOBAL_AUTH
    public bool KogOtpVerify { get; init; } = true;               // SERV_KOG_OTP_VERIFY
    public bool PurchaseToken { get; init; } = true;              // SERV_PURCHASE_TOKEN
    public bool MassFileMappingFunction { get; init; } = true;    // SERV_MASSFILE_MAPPING_FUNCTION
    public bool ServerTimeGet { get; init; }                      // SERV_SERVER_TIME_GET
    public bool UsePasswordToken { get; init; }                   // SERV_USE_PASSWORD_TOKEN
    public bool DllListCheckBeforeLoading { get; init; }          // SERV_DLL_LIST_CHECK_BEFOR_LOADING
    public bool MachineIdCheckBeforeLoading { get; init; } = true;// SERV_MACHINE_ID_CHECK_BEFOR_LOADING
    public bool MachineIdBlock { get; init; } = true;             // SERV_MACHINE_ID_BLOCK
    public bool MachineIdBlockUpdate { get; init; } = true;       // SERV_MACHINE_ID_BLOCK_UPDATE
    public bool SerialNumberAvailabilityCheck { get; init; }      // SERV_SERIAL_NUMBER_AVAILABILITY_CHECK
    public bool SerialNumberCheckInGameServer { get; init; }      // SERV_SERIAL_NUMBER_AVAILABILITY_CHECK_IN_GAMESERVER
    public bool LocalRankingSystem { get; init; }                 // SERV_LOCAL_RANKING_SYSTEM
    public bool LoginMacAddress { get; init; }                    // SERV_LOGIN_MAC_ADDRESS (CN)
    public bool PublisherOtp { get; init; }                       // SERV_PUBLISHER_OTP (CN)
    public bool PublisherMatrixKey { get; init; }                 // SERV_PUBLISHER_MATRIXKEY (CN)
    public bool FromChannelToLoginProxy { get; init; }            // SERV_FROM_CHANNEL_TO_LOGIN_PROXY (CN)
    public bool UseDomainAccount { get; init; }                   // SERV_USE_DOMAIN_ACCOUNT
    public bool Steam { get; init; }                              // SERV_STEAM
    public bool ChannelingAeria { get; init; }                    // SERV_CHANNELING_AERIA
    public bool LoginTou { get; init; }                           // SERV_LOGIN_TOU
    public bool CountryTH { get; init; }                          // SERV_COUNTRY_TH
    public bool SouthAmerica { get; init; }                       // SERV_SOUTH_AMERICA
    public bool ChannelListRenewal { get; init; } = true;         // SERV_CHANNEL_LIST_RENEWAL
    public bool IntegrateChannelInfo { get; init; }               // SERV_INTEGRATE_THE_CHANNEL_INFO
    public bool ServerGroupUiAdvanced { get; init; } = true;      // SERVER_GROUP_UI_ADVANCED
    public bool WaitQueueSystem { get; init; } = true;            // SERV_WAIT_QUEUE_SYSTEM
    public bool HackingToolList { get; init; } = true;            // SERV_HACKING_TOOL_LIST
    public bool CcuMonitoringTool { get; init; } = true;          // SERV_CCU_MONITORING_TOOL
    public bool PrivateIpServerNetworking { get; init; } = true;  // SERV_PRIVATE_IP_SERVER_NETWORKING
    public bool LoginResultInfo { get; init; } = true;            // SERV_LOGIN_RESULT_INFO
    public bool AntiAddictionSystem { get; init; }                // SERV_ANTI_ADDICTION_SYSTEM (CN)
    public bool DisableSecondSecurity { get; init; }              // SERV_DISABLE_SECOND_SECURITY (HK)
    public bool AccountBlockMessageRenewal { get; init; }         // SERV_ACCOUNT_BLOCK_MESSAGE_RENEWAL (TW/CN)
    public bool ServerCheckHackUserReqAck { get; init; } = true;  // SERV_SERVER_CHECK_HACK_USER_REQ_ACK
    public bool ReconnectForDdosSolution { get; init; }           // SERV_RECONNECT_LOGIC_FOR_DDOS_SOLUTION
    public bool PcBangType { get; init; } = true;                 // SERV_PC_BANG_TYPE (global builds)
    public bool IdNetmarblePcBang { get; init; }                  // SERV_ID_NETMARBLE_PCBANG

    // GameServer / Gameplay wire toggles (verified via g++ -E across TW/HK/CN/JP)
    public bool GoldTicket { get; init; } = true;                 // SERV_GOLD_TICKET (TW/HK)
    public bool GuaranteeUniquenessOfNameCn { get; init; }        // SERV_GUARANTEE_UNIQUENESS_OF_NAME_CN (CN)
    public bool UnitWaitDelete { get; init; }                     // SERV_UNIT_WAIT_DELETE (CN)
    public bool SharingBankQuestCash { get; init; } = true;       // SERV_SHARING_BANK_QUEST_CASH
    public bool Anniv4ThEvent { get; init; }                      // SERV_4TH_ANNIV_EVENT (JP)
    public bool GlobalEventTable { get; init; }                   // SERV_GLOBAL_EVENT_TABLE (JP)
    public bool RelationshipSystem { get; init; } = true;         // SERV_RELATIONSHIP_SYSTEM
    public bool RidingPetSystem { get; init; } = true;            // SERV_RIDING_PET_SYSTM
    public bool NewItemSystem2013 { get; init; } = true;          // SERV_NEW_ITEM_SYSTEM_2013_05
    public bool KeyMappingInt { get; init; }                      // SERV_KEY_MAPPING_INT (CN/EU/JP/US)

    // Default server ports for this region (Channel=9400 per X2Main.cpp, Game=9300 per config_rb.lua, TR=9301 per TRServer.cpp)
    public ushort DefaultChannelPort { get; init; } = 9400;
    public ushort DefaultLoginPort { get; init; } = 9200;
    public ushort DefaultGamePort { get; init; } = 9300;
    public ushort DefaultTrPort { get; init; } = 9301;
    public ushort DefaultUdpEchoPort { get; init; } = 9302;
    public ushort DefaultUdpRelayPort { get; init; } = 9303;
    public ushort DefaultCenterPort { get; init; } = 9100;
    public ushort DefaultGlobalPort { get; init; } = 9500;

    // Default regional policy metadata
    public NationFlag DefaultNationFlag { get; init; }
    public AuthFlag DefaultAuthFlag { get; init; } = AuthFlag.GlobalService;
    public PublisherBillingConnectType DefaultBillingType { get; init; }
    public ChannelingCode DefaultChannelingCode { get; init; }

    private RegionProfile(RegionCode region, string code, string displayName)
    {
        Region = region;
        Code = code;
        DisplayName = displayName;
    }

    public int ResolveNetError(NetErrorKey key) => NetErrorRegistry.Resolve(Region, key);
    public string GetNetErrorString(int errorId) => NetErrorRegistry.GetMessage(Region, errorId);

    public static RegionProfile Taiwan { get; } = new(RegionCode.TW, "TW", "Taiwan (Gamania GASH)")
    {
        DefaultNationFlag = NationFlag.TW,
        DefaultBillingType = PublisherBillingConnectType.Unusual,
        DefaultChannelingCode = ChannelingCode.Gamania,
        AccountBlockMessageRenewal = false,
    };

    public static RegionProfile HongKong { get; } = new(RegionCode.HK, "HK", "Hong Kong (Gamania GASH)")
    {
        DefaultNationFlag = NationFlag.HK,
        DefaultBillingType = PublisherBillingConnectType.Unusual,
        DefaultChannelingCode = ChannelingCode.Gamania,
        DisableSecondSecurity = true,
        AccountBlockMessageRenewal = false,
    };

    public static RegionProfile China { get; } = new(RegionCode.CN, "CN", "China (Giant Network)")
    {
        DefaultNationFlag = NationFlag.CN,
        DefaultBillingType = PublisherBillingConnectType.Unusual,
        DefaultChannelingCode = ChannelingCode.Giant,
        ExtendServerGroupMask = true,
        LoginMacAddress = true,
        PublisherOtp = true,
        PublisherMatrixKey = true,
        FromChannelToLoginProxy = true,
        AntiAddictionSystem = true,
        AccountBlockMessageRenewal = true,
        GoldTicket = false,
        GuaranteeUniquenessOfNameCn = true,
        UnitWaitDelete = true,
        SharingBankQuestCash = true,
        KeyMappingInt = true,
    };

    public static RegionProfile Japan { get; } = new(RegionCode.JP, "JP", "Japan (NHN Hangame / PURPLE)")
    {
        DefaultNationFlag = NationFlag.JP,
        DefaultBillingType = PublisherBillingConnectType.Normal,
        DefaultChannelingCode = ChannelingCode.NhNJapan,
        AccountBlockMessageRenewal = false,
        GoldTicket = false,
        Anniv4ThEvent = true,
        GlobalEventTable = true,
        KeyMappingInt = true,
        DefaultChannelPort = 12400,
        DefaultLoginPort = 12200,
        DefaultGamePort = 12300,
        DefaultTrPort = 12301,
        DefaultUdpEchoPort = 12302,
        DefaultUdpRelayPort = 12303,
        DefaultCenterPort = 12100,
        DefaultGlobalPort = 12500,
    };

    public static RegionProfile FromCode(RegionCode region) => region switch
    {
        RegionCode.TW => Taiwan,
        RegionCode.HK => HongKong,
        RegionCode.CN => China,
        RegionCode.JP => Japan,
        _ => throw new ArgumentOutOfRangeException(nameof(region), region, null),
    };

    public static RegionProfile FromString(string code) => code.Trim().ToUpperInvariant() switch
    {
        "TW" or "TWHK" => Taiwan,
        "HK" => HongKong,
        "CN" => China,
        "JP" => Japan,
        _ => throw new ArgumentException($"Unsupported region code: '{code}'. Expected TW, HK, CN, or JP.", nameof(code)),
    };

    /// <summary>
    /// Global active region context (can also be overridden per-server instance).
    /// </summary>
    public static RegionProfile Current { get; private set; } = Taiwan;

    public static void SetCurrent(RegionCode region) => Current = FromCode(region);
    public static void SetCurrent(RegionProfile profile) => Current = profile ?? throw new ArgumentNullException(nameof(profile));
}
