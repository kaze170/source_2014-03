using System.Collections.Concurrent;
using KncWX2Server.Core.Database;
using KncWX2Server.Core.Framework;
using KncWX2Server.Core.Logging;
using KncWX2Server.Core.Region;
using MoonSharp.Interpreter;

namespace KncWX2Server.Core.Scripting;

/// <summary>
/// MoonSharp-powered Lua scripting engine replacing lua_tinker / KLuaManager across all KncWX2Server configs and ServerResource tables.
/// </summary>
public sealed class KScriptManager
{
    public Script Lua { get; }
    private readonly ConcurrentDictionary<string, int> _dynamicIdCache = new(StringComparer.Ordinal);

    public KScriptManager()
    {
        UserData.RegistrationPolicy = MoonSharp.Interpreter.Interop.InteropRegistrationPolicy.Automatic;
        Lua = new Script(CoreModules.Preset_Complete);
        RegisterBuiltInConstants();
    }

    private void RegisterBuiltInConstants()
    {
        Lua.Globals["True"] = true;
        Lua.Globals["False"] = false;
        Lua.Globals["TRUE"] = true;
        Lua.Globals["FALSE"] = false;

        // AuthFlag
        Lua.Globals["AF_INTERNAL"] = (int)AuthFlag.Internal;
        Lua.Globals["AF_NEXON_KOREA"] = (int)AuthFlag.NexonKorea;
        Lua.Globals["AF_GLOBAL_SERVICE"] = (int)AuthFlag.GlobalService;

        // BillingFlag
        Lua.Globals["BF_NONE"] = (int)BillingFlag.None;
        Lua.Globals["BF_INTERNAL"] = (int)BillingFlag.Internal;
        Lua.Globals["BF_NEXON_KOREA_TEST"] = (int)BillingFlag.NexonKoreaTest;
        Lua.Globals["BF_NEXON_KOREA"] = (int)BillingFlag.NexonKorea;
        Lua.Globals["BF_GLOBAL_SERVICE"] = (int)BillingFlag.GlobalService;

        // VersionFlag
        Lua.Globals["VF_NONE"] = (int)VersionFlag.None;
        Lua.Globals["VF_INTERNAL"] = (int)VersionFlag.Internal;
        Lua.Globals["VF_OPEN_TEST"] = (int)VersionFlag.OpenTest;
        Lua.Globals["VF_SERVICE"] = (int)VersionFlag.Service;

        // VERSION_ENUM
        Lua.Globals["VE_INTERNAL"] = 0;
        Lua.Globals["VE_OPEN_TEST"] = 1;
        Lua.Globals["VE_SERVICE"] = 2;

        // ClientFlag
        Lua.Globals["CF_NONE"] = (int)ClientFlag.None;
        Lua.Globals["CF_NPGG"] = (int)ClientFlag.Npgg;
        Lua.Globals["CF_CHECK_IP"] = (int)ClientFlag.CheckIp;
        Lua.Globals["CF_HSHIELD"] = (int)ClientFlag.HShield;
        Lua.Globals["CF_XTRAP"] = (int)ClientFlag.XTrap;

        // NationFlag
        Lua.Globals["NF_NONE"] = (int)NationFlag.None;
        Lua.Globals["NF_TW"] = (int)NationFlag.TW;
        Lua.Globals["NF_HK"] = (int)NationFlag.HK;
        Lua.Globals["NF_CN"] = (int)NationFlag.CN;
        Lua.Globals["NF_JP"] = (int)NationFlag.JP;
        Lua.Globals["NF_US"] = (int)NationFlag.US;
        Lua.Globals["NF_DE"] = (int)NationFlag.DE;
        Lua.Globals["NF_FR"] = (int)NationFlag.FR;
        Lua.Globals["NF_IT"] = (int)NationFlag.IT;
        Lua.Globals["NF_PL"] = (int)NationFlag.PL;
        Lua.Globals["NF_ES"] = (int)NationFlag.ES;
        Lua.Globals["NF_TR"] = (int)NationFlag.TR;
        Lua.Globals["NF_EN"] = (int)NationFlag.EN;
        Lua.Globals["NF_KR"] = (int)NationFlag.KR;
        Lua.Globals["NF_VN"] = (int)NationFlag.VN;
        Lua.Globals["NF_TH"] = (int)NationFlag.TH;
        Lua.Globals["NF_ID"] = (int)NationFlag.ID;
        Lua.Globals["NF_BR"] = (int)NationFlag.BR;
        Lua.Globals["NF_PH"] = (int)NationFlag.PH;
        Lua.Globals["NF_IN"] = (int)NationFlag.IN;

        // PublisherBillingConnectType
        Lua.Globals["PBCT_NONE"] = (int)PublisherBillingConnectType.None;
        Lua.Globals["PBCT_NORMAL"] = (int)PublisherBillingConnectType.Normal;
        Lua.Globals["PBCT_DB"] = (int)PublisherBillingConnectType.Db;
        Lua.Globals["PBCT_UNUSUAL"] = (int)PublisherBillingConnectType.Unusual;

        // CheckIPMode & CheckIPType
        Lua.Globals["CIM_SERVICE_MODE"] = 0;
        Lua.Globals["CIM_CHECK_MODE"] = 1;
        Lua.Globals["CIT_ALL_IP_ALLOW"] = 0;
        Lua.Globals["CIT_CHECK_IP_ALLOW"] = 1;
        Lua.Globals["CIT_CHECK_IP_BLOCK"] = 2;

        // DB types & ServerClass
        Lua.Globals["DC_ACCOUNT"] = (int)DbConnectionType.DC_ACCOUNT;
        Lua.Globals["DC_GAME"] = (int)DbConnectionType.DC_GAME;
        Lua.Globals["DC_LOG"] = (int)DbConnectionType.DC_LOG;
        Lua.Globals["DC_SMS"] = (int)DbConnectionType.DC_SMS;
        Lua.Globals["DC_NX_SHOP_GATEWAY"] = (int)DbConnectionType.DC_NX_SHOP_GATEWAY;
        Lua.Globals["DC_NX_WEB"] = (int)DbConnectionType.DC_NX_WEB;
        Lua.Globals["DC_NETMARBLE_BILLING"] = (int)DbConnectionType.DC_NETMARBLE_BILLING;
        Lua.Globals["DC_KOG_BILLING"] = (int)DbConnectionType.DC_KOG_BILLING;
        Lua.Globals["ESC_CENTER"] = (int)ServerClass.Center;
        Lua.Globals["ESC_LOGIN"] = (int)ServerClass.Login;
        Lua.Globals["ESC_GAME"] = (int)ServerClass.Game;
        Lua.Globals["ESC_CHANNEL"] = (int)ServerClass.Channel;
        Lua.Globals["ESC_SIMULATION"] = (int)ServerClass.Simulation;
        Lua.Globals["ESC_GLOBAL"] = (int)ServerClass.Global;

        // Statistics IDs (StatisticsID_def.h)
        Lua.Globals["SI_PVP_MAP"] = KStatisticsManager.SI_PVP_MAP;
        Lua.Globals["SI_FPS"] = KStatisticsManager.SI_FPS;
        Lua.Globals["SI_RES_STONE"] = KStatisticsManager.SI_RES_STONE;
        Lua.Globals["SI_ED"] = KStatisticsManager.SI_ED;
        Lua.Globals["SI_QUEST"] = KStatisticsManager.SI_QUEST;
        Lua.Globals["SI_DISCONNECT_REASON"] = KStatisticsManager.SI_DISCONNECT_REASON;
        Lua.Globals["SI_DISCONNECT_USERFSM"] = KStatisticsManager.SI_DISCONNECT_USERFSM;
        Lua.Globals["SI_LOC_ITEM"] = KStatisticsManager.SI_LOC_ITEM;
        Lua.Globals["SI_LOC_PVP"] = KStatisticsManager.SI_LOC_PVP;
        Lua.Globals["SI_LOC_DUNGEON"] = KStatisticsManager.SI_LOC_DUNGEON;
        Lua.Globals["SI_LOC_SPIRIT"] = KStatisticsManager.SI_LOC_SPIRIT;
        Lua.Globals["SI_LOC_ENCHANT"] = KStatisticsManager.SI_LOC_ENCHANT;
        Lua.Globals["SI_LOC_PVP_VICTORY_PER"] = KStatisticsManager.SI_LOC_PVP_VICTORY_PER;
        Lua.Globals["SI_LOC_PVP_ROOM"] = KStatisticsManager.SI_LOC_PVP_ROOM;
        Lua.Globals["SI_LOC_DUNGEON_ROOM"] = KStatisticsManager.SI_LOC_DUNGEON_ROOM;
        Lua.Globals["SI_LOC_HENIR_RANKING"] = KStatisticsManager.SI_LOC_HENIR_RANKING;
        Lua.Globals["SI_LOC_TITLE"] = KStatisticsManager.SI_LOC_TITLE;
        Lua.Globals["SI_LOC_PET_SUMMON"] = KStatisticsManager.SI_LOC_PET_SUMMON;
        Lua.Globals["SI_LOC_SERV_DISCONNECT"] = KStatisticsManager.SI_LOC_SERV_DISCONNECT;
        Lua.Globals["SI_LOC_BILLING_PACKET"] = KStatisticsManager.SI_LOC_BILLING_PACKET;
        Lua.Globals["SI_LOC_ABUSER_MORNITORING"] = KStatisticsManager.SI_LOC_ABUSER_MORNITORING;
        Lua.Globals["SI_CHARACTER"] = KStatisticsManager.SI_CHARACTER;
        Lua.Globals["SI_ITEM_DB"] = KStatisticsManager.SI_ITEM_DB;
        Lua.Globals["SI_SENTINEL"] = KStatisticsManager.SI_SENTINEL;
        Lua.Globals["SI_LOC_UNDEFINED_QUEST_TEMPLET"] = KStatisticsManager.SI_LOC_UNDEFINED_QUEST_TEMPLET;

        // Global BaseServer functions
        Lua.Globals["GetLocalIP"] = DynValue.NewCallback((_, _) =>
        {
            string ip = "127.0.0.1";
            try
            {
                var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
                var addr = host.AddressList.FirstOrDefault(a => a.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
                if (addr != null) ip = addr.ToString();
            }
            catch { }
            return DynValue.NewString(ip);
        });
        Lua.Globals["log"] = DynValue.NewCallback((_, a) =>
        {
            KLog.Cout($"[Lua log] level={ArgInt(a, 0)}");
            return DynValue.Nil;
        });
        Lua.Globals["changelog"] = DynValue.NewCallback((_, _) => DynValue.Nil);

        // NHN Japan HanAuth init flags
        Lua.Globals["HIF_ALPHA"] = 0;
        Lua.Globals["HIF_REAL"] = 1;

        // Regional Auth / Billing Lua constants
        Lua.Globals["AF_NHN_JAPAN"] = (int)AuthFlag.GlobalService;
        Lua.Globals["AF_GAME_FORGE_EU"] = (int)AuthFlag.GlobalService;
        Lua.Globals["BF_GAME_FORGE_EU"] = (int)BillingFlag.GlobalService;

        Lua.Globals["LU_SALT"] = 0;
        Lua.Globals["LU_AUTH"] = 1;
        Lua.Globals["LU_NUM"] = 2;

        Lua.Globals["GFO_AUTH"] = 0;
        Lua.Globals["GFO_NUM"] = 1;
        Lua.Globals["GFBilling_GetCash"] = 0;
        Lua.Globals["GFBilling_ReduceCash"] = 1;
        Lua.Globals["GFBilling_NUM"] = 2;

        Lua.Globals["K3R_AUTH"] = 0;
        Lua.Globals["K3R_AUTH_SECOND_SECURITY"] = 1;
        Lua.Globals["K3R_NUM"] = 2;

        Lua.Globals["NXSOAP_AUTH"] = 0;
        Lua.Globals["NXSOAP_CHECK_ID"] = 1;
        Lua.Globals["NXSOAP_NUM"] = 2;
        Lua.Globals["NBO_PURCHASE_COUPON"] = 0;
        Lua.Globals["NBO_NUM"] = 1;

        Lua.Globals["BALANCE_CHECK"] = 10;
        Lua.Globals["PURCHASEITEM"] = 20;
        Lua.Globals["GIFTITEM"] = 21;
        Lua.Globals["REGCOUPON"] = 30;
        Lua.Globals["HEALTH_CHECK"] = 40;

        // WaitQueueMode
        Lua.Globals["WQ_NONE"] = 0;
        Lua.Globals["WQ_CCU_LIMIT"] = 1;
        Lua.Globals["WQ_ENTRY_BAN"] = 2;

        // EDInventoryExpand constants
        Lua.Globals["NONE"] = 0;
        Lua.Globals["NORMAL"] = 8;
        Lua.Globals["SILVER"] = 16;
        Lua.Globals["GOLD"] = 24;
        Lua.Globals["EMERALD"] = 32;
        Lua.Globals["DIAMOND"] = 40;
        Lua.Globals["PLATINUM"] = 48;

        // SERVER_GROUP_ID table (TW/HK/JP/CN)
        Lua.Globals["SERVER_GROUP_ID"] = MakeEnumTable(new Dictionary<string, int>
        {
            ["SGI_INVALID"] = -1,
            ["SGI_SOLES"] = 0,
            ["SGI_GAIA"] = 1,
            ["SGI_ZR01"] = 1,
            ["SGI_ZR02"] = 2,
            ["SGI_ZR03"] = 3,
            ["SGI_WH01"] = 4,
            ["SGI_WH02"] = 5,
            ["SGI_ZR04"] = 6,
            ["SGI_TJ01"] = 101,
            ["SGI_TJ02"] = 102,
        });

        Lua.Globals["UNIT_TYPE"] = MakeEnumTable(new Dictionary<string, int>
        {
            ["UT_NONE"] = 0,
            ["UT_ELSWORD"] = 1,
            ["UT_ARME"] = 2,
            ["UT_LIRE"] = 3,
            ["UT_RAVEN"] = 4,
            ["UT_EVE"] = 5,
            ["UT_CHUNG"] = 6,
            ["UT_ARA"] = 7,
            ["UT_ELESIS"] = 8,
            ["UT_ADD"] = 9,
            ["UT_END"] = 10,
        });

        Lua.Globals["UNIT_CLASS"] = MakeEnumTable(new Dictionary<string, int>
        {
            ["UC_NONE"] = 0,
            ["UC_ELSWORD_SWORDMAN"] = 1,
            ["UC_ARME_VIOLET_MAGE"] = 2,
            ["UC_LIRE_ELVEN_RANGER"] = 3,
            ["UC_RAVEN_FIGHTER"] = 4,
            ["UC_EVE_NASOD"] = 5,
            ["UC_CHUNG_IRON_CANNON"] = 6,
            ["UC_ARA_MARTIAL_ARTIST"] = 7,
            ["UC_ELESIS_KNIGHT"] = 8,
            ["UC_ADD_NASOD_RULER"] = 9,
            ["UC_ELSWORD_KNIGHT"] = 10,
            ["UC_ELSWORD_MAGIC_KNIGHT"] = 11,
            ["UC_LIRE_COMBAT_RANGER"] = 12,
            ["UC_LIRE_SNIPING_RANGER"] = 13,
            ["UC_ARME_HIGH_MAGICIAN"] = 14,
            ["UC_ARME_DARK_MAGICIAN"] = 15,
            ["UC_RAVEN_SOUL_TAKER"] = 16,
            ["UC_RAVEN_OVER_TAKER"] = 17,
            ["UC_EVE_EXOTIC_GEAR"] = 18,
            ["UC_EVE_ARCHITECTURE"] = 19,
            ["UC_CHUNG_FURY_GUARDIAN"] = 20,
            ["UC_CHUNG_SHOOTING_GUARDIAN"] = 21,
            ["UC_ELSWORD_SHEATH_KNIGHT"] = 22,
            ["UC_ARME_BATTLE_MAGICIAN"] = 23,
            ["UC_LIRE_TRAPPING_RANGER"] = 24,
            ["UC_RAVEN_WEAPON_TAKER"] = 25,
            ["UC_EVE_ELECTRA"] = 26,
            ["UC_CHUNG_SHELLING_GUARDIAN"] = 27,
            ["UC_ARA_LITTLE_HSIEN"] = 28,
            ["UC_ELESIS_SABER_KNIGHT"] = 29,
            ["UC_ELESIS_PYRO_KNIGHT"] = 30,
            ["UC_ARA_LITTLE_DEVIL"] = 31,
            ["UC_ADD_PSYCHIC_TRACER"] = 32,
            ["UC_ELSWORD_LORD_KNIGHT"] = 100,
            ["UC_ELSWORD_RUNE_SLAYER"] = 101,
            ["UC_LIRE_WIND_SNEAKER"] = 102,
            ["UC_LIRE_GRAND_ARCHER"] = 103,
            ["UC_ARME_ELEMENTAL_MASTER"] = 104,
            ["UC_ARME_VOID_PRINCESS"] = 105,
            ["UC_RAVEN_BLADE_MASTER"] = 106,
            ["UC_RAVEN_RECKLESS_FIST"] = 107,
            ["UC_EVE_CODE_NEMESIS"] = 108,
            ["UC_EVE_CODE_EMPRESS"] = 109,
            ["UC_CHUNG_IRON_PALADIN"] = 110,
            ["UC_CHUNG_DEADLY_CHASER"] = 111,
            ["UC_ELSWORD_INFINITY_SWORD"] = 112,
            ["UC_ARME_DIMENSION_WITCH"] = 113,
            ["UC_LIRE_NIGHT_WATCHER"] = 114,
            ["UC_RAVEN_VETERAN_COMMANDER"] = 115,
            ["UC_EVE_BATTLE_SERAPH"] = 116,
            ["UC_CHUNG_TACTICAL_TROOPER"] = 117,
            ["UC_ARA_SAKRA_DEVANAM"] = 118,
            ["UC_ARA_YAMA_RAJA"] = 119,
            ["UC_ELESIS_GRAND_MASTER"] = 120,
            ["UC_ELESIS_BLAZING_HEART"] = 121,
            ["UC_ADD_LUNATIC_PSYKER"] = 122,
        }, fallbackNamespace: "UNIT_CLASS");

        Lua.Globals["ITEM_TYPE"] = MakeEnumTable(new Dictionary<string, int>
        {
            ["IT_NONE"] = 0,
            ["IT_WEAPON"] = 1,
            ["IT_DEFENCE"] = 2,
            ["IT_ACCESSORY"] = 3,
            ["IT_SKILL"] = 4,
            ["IT_QICK_SLOT"] = 5,
            ["IT_MATERIAL"] = 6,
            ["IT_SPECIAL"] = 7,
            ["IT_QUEST"] = 8,
            ["IT_OUTLAY"] = 9,
            ["IT_ETC"] = 10,
            ["IT_SKILL_MEMO"] = 11,
            ["IT_PET"] = 12,
            ["IT_RIDING"] = 13,
        });

        Lua.Globals["ITEM_GRADE"] = MakeEnumTable(new Dictionary<string, int>
        {
            ["IG_NONE"] = 0,
            ["IG_UNIQUE"] = 1,
            ["IG_ELITE"] = 2,
            ["IG_RARE"] = 3,
            ["IG_NORMAL"] = 4,
            ["IG_LOW"] = 5,
        });

        Lua.Globals["EQIP_POSITION"] = MakeEnumTable(new Dictionary<string, int>
        {
            ["EP_NONE"] = 0,
            ["EP_QUICK_SLOT"] = 1,
            ["EP_WEAPON_HAND"] = 2,
            ["EP_WEAPON_TEMP"] = 3,
            ["EP_DEFENCE_HAIR"] = 6,
            ["EP_DEFENCE_FACE"] = 7,
            ["EP_DEFENCE_BODY"] = 8,
            ["EP_DEFENCE_LEG"] = 9,
            ["EP_DEFENCE_HAND"] = 10,
            ["EP_DEFENCE_FOOT"] = 11,
            ["EP_DEFENCE_TEMP"] = 12,
        }, fallbackNamespace: "EQIP_POSITION");

        Lua.Globals["USE_CONDITION"] = MakeEnumTable(new Dictionary<string, int>
        {
            ["UC_ANYONE"] = 0,
            ["UC_ONE_UNIT"] = 1,
            ["UC_ONE_CLASS"] = 2,
        });

        Lua.Globals["ENCHANT_TYPE"] = MakeEnumTable(new Dictionary<string, int>
        {
            ["ET_NONE"] = 0,
            ["ET_BLAZE"] = 1,
            ["ET_WATER"] = 2,
            ["ET_NATURE"] = 3,
            ["ET_WIND"] = 4,
            ["ET_LIGHT"] = 5,
            ["ET_DARK"] = 6,
            ["ET_RANDOM"] = 7,
        });

        Lua.Globals["RESOLVE_RESULT_TYPE"] = MakeEnumTable(new Dictionary<string, int>
        {
            ["RRT_EL_FLOUR"] = 0,
            ["RRT_WEAPON_BROKEN_PIECE"] = 1,
            ["RRT_ARMOR_BROKEN_PIECE"] = 2,
        });

        var sdt = new Dictionary<string, int>();
        string[] sdtOrdered =
        [
            "SDT_NONE", "SDT_WEAPON_NORMAL", "SDT_WEAPON_RARE", "SDT_DEFENCE_NORMAL", "SDT_DEFENCE_RARE",
            "SDT_SET_ITEM", "SDT_ACCESSORY", "SDT_TITLE", "SDT_WEAPON_HARMONY", "SDT_DEFENCE_HARMONY",
            "SDT_WEAPON_MYSTERY", "SDT_DEFENCE_MYSTERY", "SDT_WEAPON_SHINE", "SDT_DEFENCE_SHINE",
            "SDT_WEAPON_BATTLE", "SDT_DEFENCE_BATTLE", "SDT_WEAPON_INCREASE", "SDT_DEFENCE_INCREASE",
            "SDT_WEAPON_HIT", "SDT_DEFENCE_HIT", "SDT_WEAPON_ATK_PHYSIC", "SDT_DEFENCE_ATK_PHYSIC",
            "SDT_WEAPON_ATK_MAGIC", "SDT_DEFENCE_ATK_MAGIC", "SDT_WEAPON_DEF_PHYSIC", "SDT_DEFENCE_DEF_PHYSIC",
            "SDT_WEAPON_DEF_MAGIC", "SDT_DEFENCE_DEF_MAGIC", "SDT_WEAPON_HYPER_TIME", "SDT_DEFENCE_HYPER_TIME",
            "SDT_WEAPON_INCREASE_HP", "SDT_DEFENCE_INCREASE_HP", "SDT_WEAPON_HYPER_CHARGE", "SDT_DEFENCE_HYPER_CHARGE",
            "SDT_WEAPON_SPEED", "SDT_DEFENCE_SPEED", "SDT_WEAPON_CRITICAL", "SDT_DEFENCE_CRITICAL",
            "SDT_WEAPON_MOVE_SPEED", "SDT_DEFENCE_MOVE_SPEED", "SDT_WEAPON_STRIKE", "SDT_DEFENCE_STRIKE",
            "SDT_WEAPON_AVOID", "SDT_DEFENCE_AVOID", "SDT_WEAPON_JUMP_SPEED", "SDT_DEFENCE_JUMP_SPEED",
            "SDT_WEAPON_ADD_DAMAGE", "SDT_DEFENCE_ADD_DAMAGE", "SDT_WEAPON_ADD_DAMAGE_REDUCE", "SDT_DEFENCE_ADD_DAMAGE_REDUCE",
            "SDT_WEAPON_DESTROY", "SDT_DEFENCE_DESTROY", "SDT_WEAPON_EXPLOSION", "SDT_DEFENCE_EXPLOSION",
            "SDT_WEAPON_BURN", "SDT_DEFENCE_BURN", "SDT_WEAPON_QUICK", "SDT_DEFENCE_QUICK",
            "SDT_WEAPON_FURY", "SDT_DEFENCE_FURY", "SDT_WEAPON_SAGE", "SDT_DEFENCE_SAGE",
            "SDT_WEAPON_HYPER_TIME_PVP", "SDT_DEFENCE_HYPER_TIME_PVP",
            "SDT_WEAPON_INCREASE_HP_PVP", "SDT_DEFENCE_INCREASE_HP_PVP",
            "SDT_WEAPON_HYPER_CHARGE_PVP", "SDT_DEFENCE_HYPER_CHARGE_PVP",
            "SDT_WEAPON_SPEED_PVP", "SDT_DEFENCE_SPEED_PVP",
            "SDT_WEAPON_CRITICAL_PVP", "SDT_DEFENCE_CRITICAL_PVP",
            "SDT_WEAPON_MOVE_SPEED_PVP", "SDT_DEFENCE_MOVE_SPEED_PVP",
            "SDT_WEAPON_STRIKE_PVP", "SDT_DEFENCE_STRIKE_PVP",
            "SDT_WEAPON_AVOID_PVP", "SDT_DEFENCE_AVOID_PVP",
            "SDT_WEAPON_JUMP_SPEED_PVP", "SDT_DEFENCE_JUMP_SPEED_PVP",
            "SDT_WEAPON_ADD_DAMAGE_PVP", "SDT_DEFENCE_ADD_DAMAGE_PVP",
            "SDT_WEAPON_ADD_DAMAGE_REDUCE_PVP", "SDT_DEFENCE_ADD_DAMAGE_REDUCE_PVP",
            "SDT_WEAPON_DESTROY_PVP", "SDT_DEFENCE_DESTROY_PVP",
            "SDT_WEAPON_EXPLOSION_PVP", "SDT_DEFENCE_EXPLOSION_PVP",
            "SDT_WEAPON_BURN_PVP", "SDT_DEFENCE_BURN_PVP",
            "SDT_WEAPON_QUICK_PVP", "SDT_DEFENCE_QUICK_PVP",
            "SDT_WEAPON_FURY_PVP", "SDT_DEFENCE_FURY_PVP",
            "SDT_PVP_TAG_POINT",
            "SDT_WEAPON_SPEED_2P5_FIXED", "SDT_DEFFENCE_SPEED_2P5_FIXED",
            "SDT_WEAPON_SPEED_5P_FIXED", "SDT_DEFFENCE_SPEED_5P_FIXED",
            "SDT_WEAPON_CRITICAL_2P5_FIXED", "SDT_DEFFENCE_CRITICAL_2P5_FIXED",
            "SDT_WEAPON_CRITICAL_5P_FIXED", "SDT_DEFFENCE_CRITICAL_5P_FIXED",
            "SDT_WEAPON_AVOID_2P5_FIXED", "SDT_DEFFECNE_AVOID_2P5_FIXED",
            "SDT_WEAPON_AVOID_5P_FIXED", "SDT_DEFFENCE_AVOID_5P_FIXED",
            "SDT_WEAPON_ADD_DAMAGE_2P5_FIXED", "SDT_DEFFENCE_ADD_DAMAGE_2P5_FIXED",
            "SDT_WEAPON_ADD_DAMAGE_5P_FIXED", "SDT_DEFFENCE_ADD_DAMAGE_5P_FIXED",
            "SDT_WEAPON_DAMAGE_REDUCE_2P5_FIXED", "SDT_DEFFECNE_DAMAGE_REDUCE_2P5_FIXED",
            "SDT_WEAPON_DAMAGE_REDUCE_5P_FIXED", "SDT_DEFFENCE_DAMAGE_REDUCE_5P_FIXED",
            "SDT_WEAPON_HYPER_CHARGE_5P_FIXED", "SDT_DEFFENCE_HYPER_CHARGE_5P_FIXED",
            "SDT_WEAPON_HYPER_CHARGE_10P_FIXED", "SDT_DEFFENCE_HYPER_CHARGE_10P_FIXED",
            "SDT_SKILL_LV_GROUP",
        ];
        for (int i = 0; i < sdtOrdered.Length; i++) sdt[sdtOrdered[i]] = i;
        sdt["SDT_QUEST_CLEAR_COUNT"] = 120;
        sdt["SDT_ITEM_EXCHANGE_COUNT"] = 121;
        Lua.Globals["SOCKET_DATA_TYPE"] = MakeEnumTable(sdt, fallbackNamespace: "SOCKET_DATA_TYPE");

        Lua.Globals["DUNGEON_TYPE"] = MakeEnumTable(new Dictionary<string, int>
        {
            ["DT_INVALID"] = 0,
            ["DT_NORMAL"] = 1,
            ["DT_HENIR"] = 2,
            ["DT_SECRET"] = 3,
            ["DT_DEFENCE_DUNGEON"] = 4,
        });

        Lua.Globals["DUNGEON_MODE"] = MakeEnumTable(new Dictionary<string, int>
        {
            ["DM_INVALID"] = 0,
            ["DM_HENIR_PRACTICE"] = 1,
            ["DM_HENIR_CHALLENGE"] = 2,
            ["DM_SECRET_NORMAL"] = 3,
            ["DM_SECRET_HELL"] = 4,
        });

        Lua.Globals["RIDING_PET_UNIT_ID"] = MakeEnumTable(new Dictionary<string, int>
        {
            ["RPUI_NONE"] = 0,
            ["RPUI_NASOD_MOBI"] = 1,
            ["RPUI_KOUCIKA"] = 2,
            ["RPUI_ANCIENT_PPORU"] = 3,
            ["RPUI_SCORPION_TYPE_R"] = 4,
            ["RPUI_ICE_HAMELING"] = 5,
            ["RPUI_DARK_HAMELING"] = 6,
            ["RPUI_RED_HAMELING"] = 7,
            ["RPUI_NASOD_MOBI_INT"] = 8,
            ["RPUI_KOUCIKA_INT"] = 9,
            ["RPUI_ANCIENT_PPORU_INT"] = 10,
            ["RPUI_OBERON_CUSTOM_650"] = 11,
            ["RPUI_ANCIENT_PPORU_NAVER"] = 12,
            ["RPUI_OBERON_CUSTOM_650_BLACK_SKULL"] = 13,
            ["RPUI_OBERON_CUSTOM_650_ANGEL_HEART"] = 14,
        }, fallbackNamespace: "RIDING_PET_UNIT_ID");

        Lua.Globals["PVP_FAILRATE"] = MakeEnumTable(new Dictionary<string, int>
        {
            ["PF_WINTEAM"] = 0,
            ["PF_LOSETEAM"] = 1,
            ["PF_INC_NUM"] = 2,
            ["PF_DEC_NUM"] = 3,
        });

        Lua.Globals["PET_UNIT_ID"] = MakeEnumTable(new Dictionary<string, int>
        {
            ["PUI_NONE"] = 0,
            ["PUI_TEST"] = 1,
            ["PUI_PETTE_PPORU"] = 2,
            ["PUI_PETTE_ANCIENT_PPORU"] = 3,
            ["PUI_TREE_KNIGHT"] = 4,
            ["PUI_PETTE_PPORU_EVENT"] = 5,
            ["PUI_PETTE_ANCIENT_PPORU_F"] = 6,
            ["PUI_TREE_KNIGHT_F"] = 7,
            ["PUI_HOLLO_WITCH"] = 8,
            ["PUI_HOLLO_WITCH_F"] = 9,
            ["PUI_XMAS2010_PPORU"] = 10,
            ["PUI_MYSTIC_NORM"] = 11,
            ["PUI_MYSTIC_BLK"] = 12,
            ["PUI_MYSTIC_WHT"] = 13,
            ["PUI_MYSTIC_RED"] = 14,
            ["PUI_HATCHLING_NORM"] = 15,
            ["PUI_HATCHLING_DARK"] = 16,
            ["PUI_HATCHLING_FIRE"] = 17,
            ["PUI_HATCHLING_LIGHT"] = 18,
            ["PUI_PETIT_ANCIENT_HALLOW_DARK"] = 19,
            ["PUI_PETIT_ANCIENT_HALLOW_FLAME"] = 20,
            ["PUI_PETIT_ANCIENT_HALLOW_SPOOK"] = 21,
            ["PUI_PETIT_ANCIENT_HALLOW_SPRITE"] = 22,
            ["PUI_PETIT_PPORU_BOOMHILL"] = 23,
            ["PUI_TREE_KNIGHT_XMAS2011"] = 24,
            ["PUI_TREE_KNIGHT_SANTA2011"] = 25,
            ["PUI_PETIT_LIGHTBRINGER_UNIEL"] = 26,
            ["PUI_PETIT_DARKBRINGER_UNIRING"] = 27,
            ["PUI_PET_SILVERFOX_MIHO"] = 28,
            ["PUI_PET_FIREFOX_SHIHO"] = 29,
            ["PUI_PET_DARKFOX_NAHO"] = 30,
            ["PUI_PET_BLOOD_EATER"] = 31,
            ["PUI_PET_MANA_EATER"] = 32,
            ["PUI_PET_MILLENNIAFOX_EUN"] = 33,
            ["PUI_PETIT_PPORU_HATS_ON"] = 34,
            ["PUI_STINKO_HATS_ON"] = 35,
            ["PUI_EBONY_HATS_ON"] = 36,
            ["PUI_PPORU_HOLLOWEEN_EVENT"] = 37,
            ["PUI_GRIM_REAPER_DEATH"] = 38,
            ["PUI_GRIM_REAPER_REBIRTH"] = 39,
            ["PUI_RAINCOAT_CAT_BLACK_KR"] = 40,
            ["PUI_RAINCOAT_CAT_YELLOW_KR"] = 41,
            ["PUI_PPORU_NAVER_EVENT"] = 42,
            ["PUI_PANDA_STICK_RED"] = 96,
            ["PUI_PANDA_STICK_BLUE"] = 97,
            ["PUI_PANDA_STICK_RED_F"] = 98,
            ["PUI_PANDA_STICK_BLUE_F"] = 99,
            ["PUI_PETTE_ANCIENT_PPORU_SHADOW"] = 100,
            ["PUI_GAMANIA_PETTE_PPORU"] = 101,
            ["PUI_PETTE_ANCIENT_PPORU_SHADOW_F"] = 102,
            ["PUI_TREE_KNIGHT_SPRING"] = 110,
            ["PUI_TREE_KNIGHT_FALL"] = 111,
            ["PUI_TREE_KNIGHT_WINTER"] = 112,
            ["PUI_TREE_KNIGHT_SPRING_F"] = 113,
            ["PUI_TREE_KNIGHT_FALL_F"] = 114,
            ["PUI_TREE_KNIGHT_WINTER_F"] = 115,
            ["PUI_HATCHLING_FIRE_F"] = 116,
            ["PUI_HATCHLING_LIGHT_F"] = 117,
            ["PUI_HATCHLING_STAR_F"] = 118,
            ["PUI_HATCHLING_NORM_F"] = 119,
            ["PUI_HATCHLING_DARK_F"] = 120,
            ["PUI_CHINAJOY_PPORU_EVENT"] = 121,
            ["PUI_HATCHLING_BLUE"] = 122,
            ["PUI_DEATH_SCYTHE"] = 123,
            ["PUI_DEATH_SCYTHE_F"] = 124,
            ["PUI_PANDA_ROYAL_RED"] = 125,
            ["PUI_PANDA_ROYAL_WHITE"] = 126,
            ["PUI_MYSTIC_NORM_F"] = 127,
            ["PUI_MYSTIC_BLK_F"] = 128,
            ["PUI_MYSTIC_WHT_F"] = 129,
            ["PUI_MYSTIC_RED_F"] = 130,
            ["PUI_MYSTIC_BLUE"] = 131,
            ["PUI_MYSTIC_BLUE_F"] = 132,
            ["PUI_PETIT_LIGHTBRINGER_UNIEL_F"] = 133,
            ["PUI_SHERIFF_NORM"] = 134,
            ["PUI_SHERIFF_WHITE"] = 135,
            ["PUI_SHERIFF_NORM_F"] = 136,
            ["PUI_SHERIFF_WHITE_F"] = 137,
            ["PUI_PETIT_DARKBRINGER_UNIRING_F"] = 138,
            ["PUI_PET_BLOOD_EATER_EVENT"] = 139,
            ["PUI_HATCHLING_BLUE_F"] = 140,
            ["PUI_PETTE_BERTHE"] = 141,
            ["PUI_PETTE_BERTHE_F"] = 142,
            ["PUI_PET_SILVERFOX_MIHO_F"] = 30000,
            ["PUI_PET_FIREFOX_SHIHO_F"] = 30001,
            ["PUI_PET_DARKFOX_NAHO_F"] = 30002,
            ["PUI_PET_MANA_EATER_EVENT_INT"] = 30003,
            ["PUI_PET_MERCENARY_PPORU_EVENT_INT"] = 30004,
            ["PUI_RAINCOAT_CAT_BLACK"] = 30005,
            ["PUI_RAINCOAT_CAT_YELLOW"] = 30006,
            ["PUI_RAINCOAT_CAT_BLACK_F"] = 30007,
            ["PUI_RAINCOAT_CAT_YELLOW_F"] = 30008,
            ["PUI_EVENT_EAT_FOOD_PPORU_F"] = 30009,
            ["PUI_HOLLO_WITCH_EVENT_F"] = 30010,
            ["PUI_PETTE_PPORU_EVENT_BRAZIL"] = 30011,
            ["PUI_HALLOWEEN_PUMPKIN_TANYA"] = 30012,
            ["PUI_HALLOWEEN_PUMPKIN_LAEL"] = 30013,
            ["PUI_HALLOWEEN_PUMPKIN_LOJETA"] = 30014,
            ["PUI_HALLOWEEN_PUMPKIN_NARENEA"] = 30015,
            ["PUI_HALLOWEEN_PUMPKIN_ELDENA"] = 30016,
            ["PUI_HALLOWEEN_PUMPKIN_TANYA_F"] = 30017,
            ["PUI_HALLOWEEN_PUMPKIN_LAEL_F"] = 30018,
            ["PUI_HALLOWEEN_PUMPKIN_LOJETA_F"] = 30019,
            ["PUI_HALLOWEEN_PUMPKIN_NARENEA_F"] = 30020,
            ["PUI_HALLOWEEN_PUMPKIN_ELDENA_F"] = 30021,
            ["PUI_SONOKONG"] = 30022,
            ["PUI_SONOKONG_F"] = 30023,
            ["PUI_FIREWORK_PPORU"] = 30024,
            ["PUI_PET_SNOWBUMP"] = 30025,
            ["PUI_PET_HUNTER_PENGUIN"] = 30026,
            ["PUI_TREE_KNIGHT_EVENT_F"] = 30027,
            ["PUI_HATCHLING_NORM_EVENT_F"] = 30028,
            ["PUI_MYSTIC_EVENT_NORM_F"] = 30029,
        }, fallbackNamespace: "PET_UNIT_ID");

        Lua.Globals["MATCH_TYPE"] = MakeEnumTable(new Dictionary<string, int>
        {
            ["MT_NONE"] = 0,
            ["MT_TEAM_1_ON_1"] = 1,
            ["MT_TEAM_2_ON_2"] = 2,
            ["MT_TEAM_3_ON_3"] = 3,
            ["MT_TEAM_DEATH_1_ON_1"] = 4,
            ["MT_TEAM_DEATH_2_ON_2"] = 5,
            ["MT_TEAM_DEATH_3_ON_3"] = 6,
        }, fallbackNamespace: "MATCH_TYPE");

        Lua.Globals["RANK_TYPE"] = MakeEnumTable(KScriptBuiltInEnums.RankType(), fallbackNamespace: "RANK_TYPE");
        Lua.Globals["HACKING_USER_CHECK_TYPE"] = MakeEnumTable(KScriptBuiltInEnums.HackingUserCheckType(), fallbackNamespace: "HUCT");
        Lua.Globals["HACKING_USER_BLOCK_NOTIFY_TYPE"] = MakeEnumTable(KScriptBuiltInEnums.HackingUserBlockNotifyType(), fallbackNamespace: "HUCBNT");
        Lua.Globals["PVP_NPC_TYPE"] = MakeEnumTable(new Dictionary<string, int>
        {
            ["PNT_NONE"] = 0,
            ["PNT_BEGINNER_NPC"] = 1,
            ["PNT_HERO_NPC"] = 2,
        }, fallbackNamespace: "PNT");
        Lua.Globals["PVP_RANK"] = MakeEnumTable(KScriptBuiltInEnums.PvpRank(), fallbackNamespace: "PVPRANK");
        Lua.Globals["DIFFICULTY_LEVEL"] = MakeEnumTable(new Dictionary<string, int>
        {
            ["DL_NULL"] = -1,
            ["DL_NORMAL"] = 0,
            ["DL_HARD"] = 1,
            ["DL_EXPERT"] = 2,
        }, fallbackNamespace: "DL");
        Lua.Globals["TITLE_MISSION_CLEAR_TYPE"] = MakeEnumTable(KScriptBuiltInEnums.TitleMissionClearType(), fallbackNamespace: "TMCT");
        Lua.Globals["DUNGEON_KINDS"] = MakeEnumTable(new Dictionary<string, int>
        {
            ["ALL_DUNGEON"] = 0,
            ["EXP_DUNGEON"] = 1,
            ["LEVEL_DUNGEON"] = 2,
        });
        Lua.Globals["SMS_TYPE"] = MakeEnumTable(KScriptBuiltInEnums.SmsType(), fallbackNamespace: "SMS");
        Lua.Globals["NPC_ABILITY_ID"] = MakeEnumTable(KScriptBuiltInEnums.NpcAbilityId(), fallbackNamespace: "NAI");
        Lua.Globals["NPC_UNIT_ID"] = MakeEnumTable(KScriptBuiltInEnums.NpcUnitId(), fallbackNamespace: "NUI");
        Lua.Globals["DUNGEON_ID"] = MakeEnumTable(KScriptBuiltInEnums.DungeonId(), fallbackNamespace: "DI");
        Lua.Globals["WORLD_ID"] = MakeEnumTable(new Dictionary<string, int>(), fallbackNamespace: "WI");
        Lua.Globals["VILLAGE_MAP_ID"] = MakeEnumTable(KScriptBuiltInEnums.VillageMapId(), fallbackNamespace: "VMI");
        Lua.Globals["LOCAL_MAP_ID"] = MakeEnumTable(KScriptBuiltInEnums.LocalMapId(), fallbackNamespace: "LMI");
        Lua.Globals["PVP_CHANNEL_CLASS"] = MakeEnumTable(KScriptBuiltInEnums.PvpChannelClass(), fallbackNamespace: "PCC");
        Lua.Globals["PVP_EMBLEM"] = MakeEnumTable(KScriptBuiltInEnums.PvpEmblem(), fallbackNamespace: "PE");
        Lua.Globals["RANKING_TYPE"] = MakeEnumTable(KScriptBuiltInEnums.RankingType(), fallbackNamespace: "RT");
        Lua.Globals["CRITERION_USER_TYPE"] = MakeEnumTable(KScriptBuiltInEnums.CriterionUserType(), fallbackNamespace: "CUT");
        Lua.Globals["ARCADE_CHANNEL_ID"] = MakeEnumTable(new Dictionary<string, int>(), fallbackNamespace: "ACI");
        Lua.Globals["CHANNELING_ENUM"] = MakeEnumTable(KScriptBuiltInEnums.ChannelingEnum(), fallbackNamespace: "CE");
        Lua.Globals["BUBBLE_FIGHTER_ENUM"] = MakeEnumTable(new Dictionary<string, int>
        {
            ["BFE_ELSWORD"] = 1,
            ["BFE_BUBBLE_FIGHTER"] = 2,
        }, fallbackNamespace: "BF");
        Lua.Globals["WEEK_OF_MONTH"] = MakeEnumTable(new Dictionary<string, int>
        {
            ["WOM_NONE"] = 0,
            ["WOM_SUNDAY"] = 1,
            ["WOM_MONDAY"] = 2,
            ["WOM_TUESDAY"] = 3,
            ["WOM_WEDNESDAY"] = 4,
            ["WOM_WEDNESDDAY"] = 4,
            ["WOM_THURSDAY"] = 5,
            ["WOM_FRIDAY"] = 6,
            ["WOM_SATURDAY"] = 7,
            ["WOM_SATERDAY"] = 7,
        });

        Lua.Globals["PNLT_ELSWORD_DEV_TEAM"] = 1;
        Lua.Globals["PNLT_ELSWORD_QA_TEAM"] = 2;
        Lua.Globals["PNLT_ELSWORD_MOR_TEAM"] = 3;
        Lua.Globals["PNLT_NEXON"] = 4;
        Lua.Globals["PNLT_ELSWORD_DEV_TEAM_SERVER"] = 5;
        Lua.Globals["FS_MINUS_ED"] = 1;
        Lua.Globals["FS_SEND_A_LETTER_TO_SAME_NICKNAME"] = 2;
        Lua.Globals["FS_INCORRECT_USER_UNIT"] = 3;
        Lua.Globals["FS_CHECK_AUTH_LEVEL"] = 4;
        Lua.Globals["FS_SURVEILLANCE_THE_LETTER_AND_TRADE"] = 5;
        Lua.Globals["FS_PERSONAL_SHOP_ITEM"] = 6;
        Lua.Globals["FS_AUTO_HACK_CHECK_GET_ITEM"] = 7;
        Lua.Globals["FS_DUNGEON_CLEAR_TIME_HACK_USER_CHECK"] = 8;
        Lua.Globals["FS_DB_HACKING_ED_UPDATE_TRAP"] = 9;
        Lua.Globals["FS_CHECK_DROP_CCU"] = 10;
        Lua.Globals["FS_SERVER_STATE"] = 11;
        Lua.Globals["FS_MORNITORING_ABUSER_NOTIFY"] = 12;

        // log(level)
        Lua.Globals["log"] = (Action<int>)(lvl => KLog.SetLevel(lvl));

        // IncludeLua(path)
        Lua.Globals["IncludeLua"] = (Action<string>)(path => DoFile(path));
    }

    private Table MakeEnumTable(Dictionary<string, int> entries, string? fallbackNamespace = null)
    {
        var tbl = new Table(Lua);
        foreach (var kv in entries)
            tbl[kv.Key] = kv.Value;

        if (!string.IsNullOrEmpty(fallbackNamespace))
        {
            var meta = new Table(Lua);
            meta["__index"] = DynValue.NewCallback((_, args) =>
            {
                string key = args.Count > 1 && args[1].Type == DataType.String ? args[1].String : string.Empty;
                if (string.IsNullOrEmpty(key)) return DynValue.NewNumber(0);
                int id = _dynamicIdCache.GetOrAdd($"{fallbackNamespace}:{key}", static k =>
                {
                    unchecked
                    {
                        uint h = 2166136261u;
                        foreach (char c in k) { h ^= c; h *= 16777619u; }
                        return (int)((h & 0x00FFFFFF) + 10000);
                    }
                });
                tbl[key] = id;
                return DynValue.NewNumber(id);
            });
            tbl.MetaTable = meta;
        }
        return tbl;
    }

    /// <summary>
    /// Creates a Lua object table with explicitly bound methods plus a safe fallback metatable
    /// so optional regional Lua methods become graceful no-ops instead of nil-call exceptions.
    /// </summary>
    public Table CreateLuaProxy(string globalName, Action<Table> registerMethods)
    {
        var tbl = new Table(Lua);
        registerMethods(tbl);

        var noop = DynValue.NewCallback((_, _) => DynValue.Nil);
        var meta = new Table(Lua);
        meta["__index"] = DynValue.NewCallback((_, args) =>
        {
            string key = args.Count > 1 && args[1].Type == DataType.String ? args[1].String : string.Empty;
            if (!string.IsNullOrEmpty(key))
                tbl[key] = noop;
            return noop;
        });
        tbl.MetaTable = meta;
        Lua.Globals[globalName] = tbl;
        return tbl;
    }

    public static int ArgInt(CallbackArguments args, int offset, int fallback = 0)
        => offset < args.Count && args[offset].Type == DataType.Number ? (int)args[offset].Number : fallback;

    public static double ArgDouble(CallbackArguments args, int offset, double fallback = 0.0)
        => offset < args.Count && args[offset].Type == DataType.Number ? args[offset].Number : fallback;

    public static float ArgFloat(CallbackArguments args, int offset, float fallback = 0f)
        => offset < args.Count && args[offset].Type == DataType.Number ? (float)args[offset].Number : fallback;

    public static bool ArgBool(CallbackArguments args, int offset, bool fallback = false)
        => offset < args.Count && args[offset].Type == DataType.Boolean ? args[offset].Boolean : fallback;

    public static string ArgString(CallbackArguments args, int offset, string fallback = "")
        => offset < args.Count && args[offset].Type == DataType.String ? args[offset].String : fallback;

    public static Table? ArgTable(CallbackArguments args, int offset)
        => offset < args.Count && args[offset].Type == DataType.Table ? args[offset].Table : null;

    /// <summary>Returns 1 when Lua called obj:Method(...) (args[0] is self table) or 0 when called obj.Method(...).</summary>
    public static int ArgStart(CallbackArguments args, Table selfTable)
        => args.Count > 0 && args[0].Type == DataType.Table && ReferenceEquals(args[0].Table, selfTable) ? 1 : 0;

    public void RegisterGlobal(string name, object instance)
    {
        UserData.RegisterType(instance.GetType());
        Lua.Globals[name] = instance;
    }

    public void RegisterBaseServer(KBaseServer server) => RegisterGlobal("Server", server);
    public void RegisterSimLayer(KSimLayer simLayer) => RegisterGlobal("SimLayer", simLayer);
    public void RegisterNetLayer(KNetLayer netLayer) => RegisterGlobal("NetLayer", netLayer);
    public void RegisterDBLayer(KDBLayer dbLayer) => RegisterGlobal("DBLayer", dbLayer);

    public static string? ResolveServerResourceFile(string fileName, RegionProfile region, string? explicitBaseDir = null)
    {
        var roots = new List<string>();
        if (!string.IsNullOrWhiteSpace(explicitBaseDir))
            roots.Add(explicitBaseDir);

        string cwd = Directory.GetCurrentDirectory();
        for (string? dir = cwd; dir != null; dir = Directory.GetParent(dir)?.FullName)
        {
            string candidate = Path.Combine(dir, "KncWX2Server", "ServerResource");
            if (Directory.Exists(candidate)) { roots.Add(candidate); break; }
            string direct = Path.Combine(dir, "ServerResource");
            if (Directory.Exists(direct)) { roots.Add(direct); break; }
        }

        string appBase = AppContext.BaseDirectory;
        for (string? dir = appBase; dir != null; dir = Directory.GetParent(dir)?.FullName)
        {
            string candidate = Path.Combine(dir, "KncWX2Server", "ServerResource");
            if (Directory.Exists(candidate)) { roots.Add(candidate); break; }
            string direct = Path.Combine(dir, "ServerResource");
            if (Directory.Exists(direct)) { roots.Add(direct); break; }
        }

        foreach (string root in roots.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            string regional = Path.Combine(root, region.Code, fileName);
            if (File.Exists(regional)) return regional;
            string common = Path.Combine(root, fileName);
            if (File.Exists(common)) return common;
        }
        return null;
    }

    public bool DoServerResourceFile(string fileName, RegionProfile region, string? explicitBaseDir = null)
    {
        string? resolved = ResolveServerResourceFile(fileName, region, explicitBaseDir);
        return resolved != null && DoFile(resolved);
    }

    public bool DoFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            KLog.Cwarn($"Lua script file not found: {filePath}");
            return false;
        }

        try
        {
            string code = File.ReadAllText(filePath);
            if (code.Length > 0 && code[0] == '\uFEFF')
                code = code[1..];
            Lua.DoString(code, codeFriendlyName: Path.GetFileName(filePath));
            return true;
        }
        catch (ScriptRuntimeException ex)
        {
            KLog.Cerr($"MoonSharp runtime error in {filePath}: {ex.DecoratedMessage}");
            return false;
        }
        catch (SyntaxErrorException ex)
        {
            KLog.Cerr($"MoonSharp syntax error in {filePath}: {ex.DecoratedMessage}");
            return false;
        }
    }

    public bool DoString(string luaCode, string name = "inline")
    {
        try
        {
            Lua.DoString(luaCode, codeFriendlyName: name);
            return true;
        }
        catch (InterpreterException ex)
        {
            KLog.Cerr($"MoonSharp error in {name}: {ex.DecoratedMessage}");
            return false;
        }
    }
}
