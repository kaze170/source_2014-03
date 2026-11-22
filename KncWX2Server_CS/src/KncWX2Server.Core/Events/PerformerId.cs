namespace KncWX2Server.Core.Events;

/// <summary>
/// Performer IDs and bitmasks matching Common/KncSend.h.
/// </summary>
public static class PerformerId
{
    public const uint PC_PROXY = 0x00000001;
    public const uint PC_USER = 0x00000002;
    public const uint PC_CHARACTER = 0x00000003;
    public const uint PC_SERVER = 0x00000004;
    public const uint PC_ROOM = 0x00000005;
    public const uint PC_ACCOUNT_DB = 0x00000006;
    public const uint PC_GAME_DB = 0x00000007;
    public const uint PC_LOG_DB = 0x00000008;
    public const uint PC_SMS_DB = 0x00000009;
    public const uint PC_PARTY = 0x0000000C;
    public const uint PC_MATCH_MAKING = 0x0000000F;
    public const uint PC_AUTO_PARTY = 0x00000011;
    public const uint PC_KOG_BILLING_MANAGER = 0x00000051;
    public const uint PC_PUBLISHER_AUTH = 0x00000053;
    public const uint PC_GIANT_AUTH = 0x00000083;
    public const uint PC_MASK_BIT = 0x000000FF;

    public const uint SC_CLIENT = 0x00000100;
    public const uint SC_GAME = 0x00000200;
    public const uint SC_CENTER = 0x00000300;
    public const uint SC_CHANNEL = 0x00000400;
    public const uint SC_LOGIN = 0x00000500;
    public const uint SC_SIMULATION = 0x00000600;
    public const uint SC_GLOBAL = 0x00000700;
    public const uint SC_MASK_BIT = 0x00000F00;

    public const uint ST_SAME_SERV_GROUP = 0x00001000;
    public const uint ST_MASK_BIT = 0x0000F000;

    public const uint PI_NULL = unchecked((uint)-1);

    // Client
    public const uint PI_GS_PROXY = SC_CLIENT | PC_PROXY;

    // Channel Server
    public const uint PI_CHANNEL_USER = SC_CHANNEL | PC_USER;
    public const uint PI_CHANNEL_SERVER = SC_CHANNEL | PC_SERVER;
    public const uint PI_CH_SERVER = PI_CHANNEL_SERVER;
    public const uint PI_CHANNEL_LOG_DB = SC_CHANNEL | PC_LOG_DB;
    public const uint PI_CHANNEL_ACCOUNT_DB = SC_CHANNEL | PC_ACCOUNT_DB;
    public const uint PI_CHANNEL_LOGIN_PROXY = SC_CHANNEL | PC_PROXY;
    public const uint PI_CHANNEL_SMS_DB = SC_CHANNEL | PC_SMS_DB;
    public const uint PI_CHANNEL_PUBLISHER_AUTH = SC_CHANNEL | PC_PUBLISHER_AUTH;

    // Login Server (in-process target)
    public const uint PI_LOGIN_USER = SC_LOGIN | PC_USER;
    public const uint PI_LOGIN_SERVER = SC_LOGIN | PC_SERVER;
    public const uint PI_LOGIN_ACCOUNT_DB = SC_LOGIN | PC_ACCOUNT_DB;
    public const uint PI_LOGIN_LOG_DB = SC_LOGIN | PC_LOG_DB;
    public const uint PI_LOGIN_GIANT_AUTH = SC_LOGIN | PC_GIANT_AUTH;
    public const uint PI_LOGIN_PUBLISHER_AUTH = SC_LOGIN | PC_PUBLISHER_AUTH;

    // Game Server
    public const uint PI_GS_USER = SC_GAME | PC_USER;
    public const uint PI_GS_CHARACTER = SC_GAME | PC_CHARACTER;
    public const uint PI_GS_SERVER = SC_GAME | PC_SERVER;
    public const uint PI_GS_ROOM = SC_GAME | PC_ROOM;
    public const uint PI_GS_FIELD = SC_GAME | PC_ROOM;
    public const uint PI_GS_PARTY = SC_GAME | PC_PARTY;
    public const uint PI_GS_KOG_BILLING_MANAGER = SC_GAME | PC_KOG_BILLING_MANAGER;
    public const uint PI_GS_ACCOUNT_DB = SC_GAME | PC_ACCOUNT_DB;
    public const uint PI_GS_GAME_DB = SC_GAME | PC_GAME_DB;
    public const uint PI_GS_LOG_DB = SC_GAME | PC_LOG_DB;
    public const uint PI_TR_SERVER = SC_SIMULATION | PC_SERVER;
    public const uint PI_TR_USER = SC_SIMULATION | PC_USER;

    // Center Server
    public const uint PI_CN_PROXY = SC_GAME | PC_PROXY;
    public const uint PI_CN_USER = SC_CENTER | PC_USER;
    public const uint PI_CN_SERVER = SC_CENTER | PC_SERVER;
    public const uint PI_CENTER_SERVER = PI_CN_SERVER;
    public const uint PI_CN_ROOM = SC_CENTER | PC_ROOM;
    public const uint PI_CN_PARTY = SC_CENTER | PC_PARTY;
    public const uint PI_CN_ACCOUNT_DB = SC_CENTER | PC_ACCOUNT_DB;
    public const uint PI_CN_GAME_DB = SC_CENTER | PC_GAME_DB;
    public const uint PI_CN_LOG_DB = SC_CENTER | PC_LOG_DB;

    // Global Server
    public const uint PI_GB_PROXY = SC_CENTER | PC_PROXY;
    public const uint PI_GB_USER = SC_GLOBAL | PC_USER;
    public const uint PI_GLOBAL_USER = PI_GB_USER;
    public const uint PI_GB_SERVER = SC_GLOBAL | PC_SERVER;
    public const uint PI_GLOBAL_SERVER = PI_GB_SERVER;
    public const uint PI_GLOBAL_MATCH_MAKING = SC_GLOBAL | PC_MATCH_MAKING;
    public const uint PI_GLOBAL_AUTO_PARTY = SC_GLOBAL | PC_AUTO_PARTY;
    public const uint PI_GB_ACCOUNT_DB = SC_GLOBAL | PC_ACCOUNT_DB;
    public const uint PI_GB_LOG_DB = SC_GLOBAL | PC_LOG_DB;

    // Common DB aliases
    public const uint PI_ACCOUNT_DB = PC_ACCOUNT_DB;
    public const uint PI_GAME_DB = PC_GAME_DB;
    public const uint PI_LOG_DB = PC_LOG_DB;

    public static uint GetPerformerClass(uint pi) => pi & PC_MASK_BIT;
    public static uint GetServerClass(uint pi) => pi & SC_MASK_BIT;
    public static uint GetSendType(uint pi) => pi & ST_MASK_BIT;

    /// <summary>
    /// Compares server hierarchy level matching KncSend.cpp CompareServerLevel().
    /// Returns &lt;0 when dest is downstream (client), 0 when same server, &gt;0 when upstream (login/center/global).
    /// </summary>
    public static int CompareServerLevel(uint destPi, uint currentPi)
    {
        uint destSc = GetServerClass(destPi);
        uint curSc = GetServerClass(currentPi);
        if (destSc == curSc) return 0;

        static int Rank(uint sc) => sc switch
        {
            SC_CLIENT => 0,
            SC_CHANNEL => 1,
            SC_GAME => 2,
            SC_LOGIN => 3,
            SC_CENTER => 4,
            SC_GLOBAL => 5,
            _ => 1,
        };
        return Rank(destSc) - Rank(curSc);
    }
}
