namespace KncWX2Server.Core.Region;

/// <summary>
/// Logical NetError identifiers mapped at runtime to the exact region-specific NetErrorEnum value
/// from NetError_def_{TW,CN,JP}.h. Note: indices 0..1401 are identical across TW/HK/CN/JP,
/// whereas 1402+ diverge (e.g. ERR_ACCOUNT_BLOCK_01..04 exist in TW/HK/CN at 1402..1405).
/// </summary>
public enum NetErrorKey
{
    NET_OK,
    ERR_UNKNOWN,
    ERR_CLIENT_STATE,
    ERR_CONNECT_00,
    ERR_CONNECT_04,
    ERR_CONNECT_05,
    ERR_VERIFY_00,
    ERR_VERIFY_01,
    ERR_VERIFY_02,
    ERR_VERIFY_03,
    ERR_VERIFY_04,
    ERR_VERIFY_05,
    ERR_VERIFY_06,
    ERR_VERIFY_08,
    ERR_VERIFY_10,
    ERR_VERIFY_11,
    ERR_VERIFY_12,
    ERR_VERIFY_13,
    ERR_VERIFY_14,
    ERR_VERIFY_15,
    ERR_VERIFY_16,
    ERR_ODBC_00,
    ERR_SEARCH_UNIT_03,
    ERR_BUY_CASH_ITEM_25,
    ERR_ODBC_01,
    ERR_VERIFY_17,
    ERR_VERIFY_18,
    ERR_VERIFY_19,
    ERR_VERIFY_20,
    ERR_VERIFY_21,
    ERR_GASH_03,
    ERR_GASH_12,
    ERR_GASH_13,
    ERR_GASH_16,
    ERR_GASH_17,
    ERR_GAMEFORGE_00,
    ERR_GAMEFORGE_01,
    ERR_GAMEFORGE_02,
    ERR_GIANT_VERIFY_01,
    ERR_GIANT_VERIFY_02,
    ERR_GIANT_VERIFY_03,
    ERR_K3RCOMBO_AUTH_01,
    ERR_VERIFY_22,
    ERR_K3RCOMBO_AUTH_02,
    ERR_SERVERSN_01,
    ERR_SERVERSN_02,
    ERR_SERVERSN_03,
    ERR_SERVERSN_04,
    ERR_SERVERSN_05,
    ERR_SERVERSN_07,
    ERR_VERIFY_23,
    ERR_GIANT_VERIFY_04,
    ERR_GIANT_VERIFY_05,
    ERR_GIANT_VERIFY_06,
    ERR_GIANT_VERIFY_07,
    ERR_VERIFY_24,
    ERR_LEVELUP_01,
    ERR_ACCOUNT_BLOCK_03,
    ERR_ACCOUNT_BLOCK_02,
    ERR_ACCOUNT_BLOCK_04,
    ERR_ACCOUNT_BLOCK_01,
    EmptyId,
    DuplicateId,
}

public static class NetErrorRegistry
{
    private static readonly Dictionary<NetErrorKey, int> CommonMap = new()
    {
        [NetErrorKey.NET_OK] = 0,
        [NetErrorKey.ERR_UNKNOWN] = 1,
        [NetErrorKey.ERR_CLIENT_STATE] = 2,
        [NetErrorKey.ERR_CONNECT_00] = 3,
        [NetErrorKey.ERR_CONNECT_04] = 4,
        [NetErrorKey.ERR_CONNECT_05] = 5,
        [NetErrorKey.ERR_VERIFY_00] = 6,
        [NetErrorKey.ERR_VERIFY_01] = 7,
        [NetErrorKey.ERR_VERIFY_02] = 8,
        [NetErrorKey.ERR_VERIFY_03] = 9,
        [NetErrorKey.ERR_VERIFY_04] = 10,
        [NetErrorKey.ERR_VERIFY_05] = 11,
        [NetErrorKey.ERR_VERIFY_06] = 12,
        [NetErrorKey.ERR_VERIFY_08] = 13,
        [NetErrorKey.ERR_VERIFY_10] = 14,
        [NetErrorKey.ERR_VERIFY_11] = 15,
        [NetErrorKey.ERR_VERIFY_12] = 16,
        [NetErrorKey.ERR_VERIFY_13] = 17,
        [NetErrorKey.ERR_VERIFY_14] = 18,
        [NetErrorKey.ERR_VERIFY_15] = 19,
        [NetErrorKey.ERR_VERIFY_16] = 20,
        [NetErrorKey.ERR_ODBC_00] = 175,
        [NetErrorKey.ERR_SEARCH_UNIT_03] = 178,
        [NetErrorKey.ERR_BUY_CASH_ITEM_25] = 437,
        [NetErrorKey.ERR_ODBC_01] = 555,
        [NetErrorKey.ERR_VERIFY_17] = 567,
        [NetErrorKey.ERR_VERIFY_18] = 575,
        [NetErrorKey.ERR_VERIFY_19] = 576,
        [NetErrorKey.ERR_VERIFY_20] = 577,
        [NetErrorKey.ERR_VERIFY_21] = 578,
        [NetErrorKey.ERR_GASH_03] = 600,
        [NetErrorKey.ERR_GASH_12] = 660,
        [NetErrorKey.ERR_GASH_13] = 661,
        [NetErrorKey.ERR_GASH_16] = 705,
        [NetErrorKey.ERR_GASH_17] = 706,
        [NetErrorKey.ERR_GAMEFORGE_00] = 765,
        [NetErrorKey.ERR_GAMEFORGE_01] = 766,
        [NetErrorKey.ERR_GAMEFORGE_02] = 767,
        [NetErrorKey.ERR_GIANT_VERIFY_01] = 770,
        [NetErrorKey.ERR_GIANT_VERIFY_02] = 771,
        [NetErrorKey.ERR_GIANT_VERIFY_03] = 772,
        [NetErrorKey.ERR_K3RCOMBO_AUTH_01] = 851,
        [NetErrorKey.ERR_VERIFY_22] = 854,
        [NetErrorKey.ERR_K3RCOMBO_AUTH_02] = 930,
        [NetErrorKey.ERR_SERVERSN_01] = 943,
        [NetErrorKey.ERR_SERVERSN_02] = 944,
        [NetErrorKey.ERR_SERVERSN_03] = 945,
        [NetErrorKey.ERR_SERVERSN_04] = 946,
        [NetErrorKey.ERR_SERVERSN_05] = 947,
        [NetErrorKey.ERR_SERVERSN_07] = 949,
        [NetErrorKey.ERR_VERIFY_23] = 950,
        [NetErrorKey.ERR_GIANT_VERIFY_04] = 954,
        [NetErrorKey.ERR_GIANT_VERIFY_05] = 955,
        [NetErrorKey.ERR_GIANT_VERIFY_06] = 956,
        [NetErrorKey.EmptyId] = 20,
        [NetErrorKey.DuplicateId] = 27,
        [NetErrorKey.ERR_GIANT_VERIFY_07] = 957,
        [NetErrorKey.ERR_VERIFY_24] = 1064,
        [NetErrorKey.ERR_LEVELUP_01] = 1286,
    };

    private static readonly Dictionary<int, string> Messages = new()
    {
        [0] = "Success (NET_OK)",
        [1] = "Unknown error (ERR_UNKNOWN)",
        [2] = "Invalid client state (ERR_CLIENT_STATE)",
        [3] = "Client version mismatch (ERR_CONNECT_00)",
        [4] = "Server capacity reached (ERR_CONNECT_04)",
        [5] = "Server is under maintenance (ERR_CONNECT_05)",
        [8] = "Server IP verification failed (ERR_VERIFY_02)",
        [10] = "Account does not exist (ERR_VERIFY_04)",
        [11] = "Account has been deleted (ERR_VERIFY_05)",
        [12] = "Incorrect password (ERR_VERIFY_06)",
        [13] = "Server enable flag update failed (ERR_VERIFY_08)",
        [15] = "Blocked account (ERR_VERIFY_11)",
        [175] = "SQL invalid character check failed (ERR_ODBC_00)",
        [555] = "Database query failed (ERR_ODBC_01)",
        [567] = "Security / MachineID / ServerSN verification failed (ERR_VERIFY_17)",
        [575] = "Account suspended/blocked (ERR_VERIFY_18)",
        [576] = "Authentication rejected (ERR_VERIFY_19)",
        [577] = "Publisher authentication failed (ERR_VERIFY_20)",
        [578] = "Authentication system error (ERR_VERIFY_21)",
        [770] = "Giant auth: password error (ERR_GIANT_VERIFY_01)",
        [771] = "Giant auth: account not exist (ERR_GIANT_VERIFY_02)",
        [772] = "Giant auth: account blocked (ERR_GIANT_VERIFY_03)",
        [943] = "ServerSN mismatch reissue (ERR_SERVERSN_01)",
        [944] = "ServerSN verification error (ERR_SERVERSN_02)",
        [945] = "ServerSN newly issued (ERR_SERVERSN_03)",
        [946] = "ServerSN anomalous state (ERR_SERVERSN_04)",
        [947] = "ServerSN updated (ERR_SERVERSN_05)",
        [949] = "ServerSN blocked (ERR_SERVERSN_07)",
        [954] = "Giant matrix card error (ERR_GIANT_VERIFY_04)",
        [955] = "Giant OTP error (ERR_GIANT_VERIFY_05)",
        [956] = "Giant account not activated (ERR_GIANT_VERIFY_06)",
        [957] = "Giant duplicate login / system error (ERR_GIANT_VERIFY_07)",
        [1402] = "Account blocked (ERR_ACCOUNT_BLOCK_03)",
        [1403] = "Account blocked (ERR_ACCOUNT_BLOCK_02)",
        [1404] = "Account blocked (ERR_ACCOUNT_BLOCK_04)",
        [1405] = "Account blocked (ERR_ACCOUNT_BLOCK_01)",
    };

    public static int Resolve(RegionCode region, NetErrorKey key)
    {
        if (CommonMap.TryGetValue(key, out int value))
            return value;

        // TW, HK, and CN define ERR_ACCOUNT_BLOCK_01..04 at 1402..1405; JP falls back to ERR_VERIFY_11 (15).
        return key switch
        {
            NetErrorKey.ERR_ACCOUNT_BLOCK_03 => region == RegionCode.JP ? 15 : 1402,
            NetErrorKey.ERR_ACCOUNT_BLOCK_02 => region == RegionCode.JP ? 15 : 1403,
            NetErrorKey.ERR_ACCOUNT_BLOCK_04 => region == RegionCode.JP ? 15 : 1404,
            NetErrorKey.ERR_ACCOUNT_BLOCK_01 => region == RegionCode.JP ? 15 : 1405,
            _ => 1,
        };
    }

    public static int Resolve(NetErrorKey key) => Resolve(RegionProfile.Current.Region, key);

    public static string GetMessage(RegionCode region, int errorId)
    {
        if (errorId >= 1402 && errorId <= 1405 && region == RegionCode.JP)
            return $"JP NetError {errorId}";
        return Messages.TryGetValue(errorId, out var msg) ? msg : $"NetError({errorId})";
    }
}

public static class NetError
{
    public const int NET_OK = 0;
    public const int ERR_UNKNOWN = 1;
    public const int ERR_CLIENT_STATE = 2;
    public const int ERR_CONNECT_00 = 3;
    public const int ERR_CONNECT_04 = 4;
    public const int ERR_CONNECT_05 = 5;
    public const int ERR_VERIFY_00 = 6;
    public const int ERR_VERIFY_01 = 7;
    public const int ERR_VERIFY_02 = 8;
    public const int ERR_VERIFY_03 = 9;
    public const int ERR_VERIFY_04 = 10;
    public const int ERR_VERIFY_05 = 11;
    public const int ERR_VERIFY_06 = 12;
    public const int ERR_VERIFY_08 = 13;
    public const int ERR_VERIFY_10 = 14;
    public const int ERR_VERIFY_11 = 15;
    public const int ERR_VERIFY_12 = 16;
    public const int ERR_VERIFY_14 = 18;
    public const int ERR_ROOM_27 = 77;
    public const int ERR_GUILD_00 = 609;
    public const int ERR_GUILD_01 = 610;
    public const int ERR_GUILD_04 = 613;
    public const int ERR_ODBC_00 = 175;
    public const int ERR_ODBC_01 = 555;
    public const int ERR_VERIFY_17 = 567;
    public const int ERR_VERIFY_18 = 575;
    public const int ERR_VERIFY_19 = 576;
    public const int ERR_VERIFY_20 = 577;
    public const int ERR_VERIFY_21 = 578;
    public const int ERR_GAMEFORGE_00 = 765;
    public const int ERR_GAMEFORGE_01 = 766;
    public const int ERR_GAMEFORGE_02 = 767;
    public const int ERR_GIANT_VERIFY_01 = 770;
    public const int ERR_GIANT_VERIFY_02 = 771;
    public const int ERR_GIANT_VERIFY_03 = 772;
    public const int ERR_GIANT_VERIFY_07 = 957;
    public const int ERR_SERVERSN_01 = 943;
    public const int ERR_SERVERSN_02 = 944;
    public const int ERR_SERVERSN_03 = 945;
    public const int ERR_SERVERSN_05 = 947;
    public const int ERR_SERVERSN_07 = 949;
    public const int ERR_TRADE_00 = 257;
    public const int ERR_BLACKLIST_00 = 242;
    public const int ERR_BLACKLIST_01 = 243;
    public const int ERR_BLACKLIST_02 = 244;
    public const int ERR_POST_LETTER_00 = 472;
    public const int ERR_POST_LETTER_01 = 473;
    public const int ERR_POST_LETTER_04 = 476;
    public const int ERR_POST_LETTER_06 = 478;
    public const int ERR_POST_LETTER_14 = 485;
    public const int ERR_POST_LETTER_19 = 559;
    public const int ERR_POST_BLACK_LIST_01 = 490;
    public const int ERR_POST_BLACK_LIST_03 = 492;
    public const int ERR_PET_00 = 747;
    public const int ERR_MATCH_MAKING_00 = 903;
    public const int ERR_MATCH_MAKING_08 = 911;
    public const int ERR_MATCH_MAKING_14 = 919;
    public const int ERR_NEW_ACCOUNT_06 = 24;
    public const int ERR_PARTY_00 = 207;
    public const int ERR_KNM_00 = 250;
    public const int ERR_KNM_01 = 251;
    public const int ERR_KNM_02 = 252;
    public const int ERR_KNM_03 = 253;
    public const int ERR_MESSENGER_00 = 534;
    public const int ERR_MESSENGER_01 = 535;
    public const int ERR_MESSENGER_02 = 536;
    public const int ERR_MESSENGER_03 = 537;
    public const int ERR_MESSENGER_04 = 538;
    public const int ERR_MESSENGER_05 = 539;
    public const int ERR_MESSENGER_06 = 540;
    public const int ERR_MESSENGER_07 = 541;
    public const int ERR_MESSENGER_08 = 542;
    public const int ERR_MESSENGER_09 = 543;
    public const int ERR_MESSENGER_10 = 549;
    public const int ERR_MESSENGER_11 = 550;
    public const int ERR_MESSENGER_12 = 551;
    public const int ERR_DIF_SERVER_GROUP_04 = 742;
    public const int ERR_ROOM_00 = 52;
    public const int ERR_ROOM_01 = 53;
    public const int ERR_ROOM_34 = 84;
    public const int ERR_ROOM_44 = 90;
    public const int ERR_ROOM_45 = 91;
    public const int ERR_PARTY_01 = 208;
    public const int ERR_PARTY_03 = 209;
    public const int ERR_POST_BLACK_LIST_00 = 489;
    public const int ERR_POST_BLACK_LIST_02 = 491;
    public const int ERR_RELATIONSHIP_00 = 1190;
    public const int ERR_RELATIONSHIP_03 = 1189;
    public const int ERR_RELATIONSHIP_05 = 1193;
    public const int ERR_RELATIONSHIP_08 = 1191;
    public const int ERR_RELATIONSHIP_11 = 1201;
    public const int ERR_RELATIONSHIP_12 = 1202;
    public const int ERR_RELATIONSHIP_13 = 1203;
    public const int ERR_RELATIONSHIP_14 = 1205;
    public const int ERR_RELATIONSHIP_15 = 1208;
    public const int ERR_RELATIONSHIP_16 = 1206;
    public const int ERR_RELATIONSHIP_17 = 1204;
    public const int ERR_RELATIONSHIP_18 = 1207;
    public const int ERR_RELATIONSHIP_19 = 1210;
    public const int ERR_RELATIONSHIP_20 = 1211;
    public const int ERR_RELATIONSHIP_21 = 1212;
    public const int ERR_RELATIONSHIP_22 = 1227;
    public const int ERR_RELATIONSHIP_23 = 1228;
    public const int ERR_RELATIONSHIP_24 = 1229;
    public const int ERR_RELATIONSHIP_27 = 1232;
    public const int ERR_RELATIONSHIP_28 = 1233;
    public const int ERR_RELATIONSHIP_29 = 1235;
    public const int ERR_RELATIONSHIP_30 = 1236;
    public const int ERR_RELATIONSHIP_31 = 1238;
    public const int ERR_RELATIONSHIP_32 = 1240;
    public const int ERR_RELATIONSHIP_33 = 1239;
    public const int ERR_RELATIONSHIP_34 = 1242;
    public const int ERR_RELATIONSHIP_36 = 1243;
    public const int ERR_RELATIONSHIP_39 = 1246;
    public const int ERR_RELATIONSHIP_40 = 1247;
    public const int ERR_RELATIONSHIP_41 = 1248;
    public const int ERR_RELATIONSHIP_42 = 1249;
    public const int ERR_RELATIONSHIP_43 = 1256;
    public const int ERR_RELATIONSHIP_44 = 1257;
    public const int ERR_RELATIONSHIP_45 = 1262;
    public const int ERR_RELATIONSHIP_46 = 1263;
    public const int ERR_RELATIONSHIP_47 = 1287;
    public const int ERR_REQUEST_OF_LOVE_01 = 1252;
    public const int ERR_REQUEST_OF_LOVE_02 = 1253;
    public const int ERR_REQUEST_OF_LOVE_03 = 1254;
    public const int ERR_REQUEST_OF_LOVE_04 = 1255;
    public const int ERR_SEARCH_UNIT_03 = 178;

    // Gameplay Parity Batch (PvP, Dungeon, BattleField, Party, Field, RoomCommon, Square) - TW NetError_def_TW.h ordinals
    public const int ERR_ROOM_LIST_00 = 48;
    public const int ERR_ROOM_LIST_01 = 49;
    public const int ERR_ROOM_LIST_02 = 50;
    public const int ERR_ROOM_07 = 58;
    public const int ERR_ROOM_16 = 67;
    public const int ERR_ROOM_17 = 68;
    public const int ERR_ROOM_18 = 69;
    public const int ERR_ROOM_19 = 70;
    public const int ERR_ROOM_23 = 74;
    public const int ERR_ROOM_28 = 78;
    public const int ERR_ROOM_30 = 80;
    public const int ERR_ROOM_43 = 89;
    public const int ERR_ROOM_47 = 93;
    public const int ERR_ITEM_04 = 97;
    public const int ERR_SLOT_02 = 108;
    public const int NOT_LEAVE_PARTY_REASON_01 = 135;
    public const int ERR_SQUARE_01 = 180;
    public const int ERR_GUEST_USER_02 = 454;
    public const int NOT_LEAVE_FIELD_REASON_01 = 513;
    public const int ERR_FIELD_04 = 517;
    public const int ERR_FIELD_05 = 518;
    public const int ERR_FIELD_06 = 519;
    public const int ERR_GM_CHAR_DENY_00 = 528;
    public const int ERR_UPDATE_FRIEND_00 = 529;
    public const int ERR_ROOM_37 = 556;
    public const int ERR_DUNGEON_ITEM_00 = 684;
    public const int ERR_DUNGEON_ITEM_01 = 685;
    public const int ERR_DUNGEON_ITEM_02 = 686;
    public const int ERR_BLOCK_TRADE_00 = 796;
    public const int ERR_BLOCK_TRADE_06 = 814;
    public const int ERR_CHANGE_PARTY_MIN_LEVEL_CHECK_FAILE = 887;
    public const int NOT_LEAVE_MATCH_REASON_00 = 896;
    public const int NOT_LEAVE_MATCH_REASON_01 = 897;
    public const int NOT_LEAVE_MATCH_REASON_02 = 898;
    public const int NOT_LEAVE_MATCH_REASON_03 = 899;
    public const int NOT_LEAVE_MATCH_WAIT_REASON_02 = 902;
    public const int NOT_LEAVE_AUTO_PARTY_REASON_00 = 1039;
    public const int NOT_LEAVE_AUTO_PARTY_REASON_01 = 1040;
    public const int NOT_LEAVE_AUTO_PARTY_REASON_02 = 1041;
    public const int NOT_LEAVE_AUTO_PARTY_REASON_03 = 1042;
    public const int NOT_LEAVE_AUTO_PARTY_WAIT_REASON_02 = 1045;
    public const int ERR_TRADE_BLOCK_UNIT_02 = 1282;
    public const int ERR_ROOM_53 = 1345;
    public const int ERR_FIELD_BOSS_02 = 1365;
    public const int ERR_FIELD_BOSS_04 = 1367;
    public const int ERR_FIELD_BOSS_03 = 1368;

    // Dungeon + Session + Security + Inventory Parity Batch (49 handlers) - TW NetError_def_TW.h ordinals
    public const int ERR_SELECT_UNIT_06 = 40;
    public const int ERR_ITEM_08 = 101;
    public const int ERR_ITEM_09 = 102;
    public const int ERR_ITEM_10 = 103;
    public const int ERR_RESURRECT_00 = 236;
    public const int ERR_RESURRECT_03 = 239;
    public const int ERR_TRADE_09 = 265;
    public const int ERR_RANDOM_ITEM_00 = 283;
    public const int ERR_RANDOM_ITEM_01 = 284;
    public const int ERR_RANDOM_ITEM_02 = 285;
    public const int ERR_RECOMMEND_USER_01 = 403;
    public const int ERR_BUY_CASH_ITEM_00 = 412;
    public const int ERR_BUY_CASH_ITEM_16 = 428;
    public const int ERR_BUY_CASH_ITEM_18 = 430;
    public const int ERR_HACK_USER_00 = 456;
    public const int ERR_HACK_USER_02 = 458;
    public const int ERR_FIELD_00 = 515;
    public const int ERR_CHANNEL_CHANGE_01 = 520;
    public const int ERR_CHANNEL_CHANGE_06 = 523;
    public const int ERR_CHANNEL_CHANGE_07 = 524;
    public const int ERR_CHANNEL_CHANGE_09 = 526;
    public const int ERR_CHANNEL_CHANGE_10 = 527;
    public const int ERR_ITEM_14 = 652;
    public const int ERR_INVENTORY_LOCK_00 = 1030;
    public const int ERR_RECOMMEND_USER_05 = 1037;
    public const int ERR_SYNTHESIS_01 = 1076;
    public const int ERR_SYNTHESIS_02 = 1077;
    public const int ERR_SELECT_UNIT_07 = 1081;
    public const int ERR_RELATIONSHIP_02 = 1192;
    public const int ERR_RELATIONSHIP_01 = 1196;
    public const int ERR_SYNTHESIS_03 = 1260;
    public const int ERR_SYNTHESIS_04 = 1261;
    public const int ERR_EVALUATE_ITEM_05 = 1264;
    public const int ERR_EVALUATE_ITEM_03 = 1265;
    public const int ERR_EVALUATE_ITEM_00 = 1266;
    public const int ERR_EVALUATE_ITEM_02 = 1267;
    public const int ERR_EVALUATE_ITEM_04 = 1268;
    public const int ERR_EVALUATE_ITEM_06 = 1269;
    public const int ERR_EVALUATE_ITEM_01 = 1270;
    public const int ERR_EVALUATE_ITEM_07 = 1273;
    public const int ERR_EVALUATE_ITEM_08 = 1274;
    public const int ERR_DANO_EVENT_03 = 1277;
    public const int ERR_EXTRACT_00 = 1300;
    public const int ERR_EXTRACT_02 = 1301;
    public const int ERR_EXTRACT_01 = 1304;
    public const int ERR_FINALITY_SKILL_03 = 1305;
    public const int ERR_FINALITY_SKILL_00 = 1306;
    public const int ERR_FINALITY_SKILL_02 = 1307;
    public const int ERR_FINALITY_SKILL_01 = 1308;
    public const int ERR_USE_ITEM_IN_QUICKSLOT_00 = 1311;
    public const int ERR_RELATIONSHIP_EVENT_02 = 1328;

    // CashShop + CashShop_Global + AdminCheat Parity Batch (79 handlers) - TW NetError_def_TW.h ordinals
    public const int ERR_CREATE_UNIT_04 = 29;
    public const int ERR_SELECT_UNIT_01 = 36;
    public const int ERR_ADMIN_COMMAND_09 = 150;
    public const int ERR_SKILL_01 = 183;
    public const int ERR_SKILL_12 = 192;
    public const int ERR_SKILL_20 = 200;
    public const int ERR_SKILL_23 = 203;
    public const int ERR_NX_WEB_SERVICE_01 = 310;
    public const int ERR_NX_SHOP_01 = 318;
    public const int ERR_NX_SHOP_02 = 319;
    public const int ERR_NX_SHOP_03 = 320;
    public const int ERR_NX_SHOP_07 = 324;
    public const int ERR_NX_SHOP_08 = 325;
    public const int ERR_NX_SHOP_09 = 326;
    public const int ERR_SOCKET_ITEM_09 = 401;
    public const int ERR_BUY_CASH_ITEM_01 = 413;
    public const int ERR_BUY_CASH_ITEM_08 = 420;
    public const int ERR_BUY_CASH_ITEM_10 = 422;
    public const int ERR_BUY_CASH_ITEM_11 = 423;
    public const int ERR_BUY_CASH_ITEM_17 = 429;
    public const int ERR_BUY_CASH_ITEM_19 = 431;
    public const int ERR_BUY_CASH_ITEM_20 = 432;
    public const int ERR_BUY_CASH_ITEM_23 = 435;
    public const int ERR_BUY_CASH_ITEM_26 = 438;
    public const int ERR_BUY_CASH_ITEM_27 = 439;
    public const int ERR_BUY_CASH_ITEM_30 = 441;
    public const int ERR_BUY_CASH_ITEM_32 = 443;
    public const int ERR_RESET_SKILL_01 = 445;
    public const int ERR_WISH_LIST_00 = 450;
    public const int ERR_GUEST_USER_00 = 452;
    public const int ERR_TITLE_03 = 497;
    public const int ERR_TITLE_04 = 498;
    public const int ERR_TITLE_05 = 499;
    public const int ERR_BUY_CASH_ITEM_33 = 563;
    public const int ERR_BUY_CASH_ITEM_34 = 564;
    public const int ERR_BUY_CASH_ITEM_35 = 565;
    public const int ERR_BUY_CASH_ITEM_36 = 571;
    public const int ERR_BUY_CASH_ITEM_37 = 572;
    public const int ERR_BUY_CASH_ITEM_38 = 573;
    public const int ERR_BUY_CASH_ITEM_39 = 574;
    public const int ERR_GASH_10 = 607;
    public const int ERR_BUY_CASH_ITEM_40 = 608;
    public const int ERR_GUILD_19 = 629;
    public const int ERR_BUY_CASH_ITEM_41 = 659;
    public const int ERR_GUILD_SKILL_00 = 663;
    public const int ERR_SKILL_NOTE_06 = 736;
    public const int ERR_SKILL_NOTE_07 = 737;
    public const int ERR_BUY_CASH_ITEM_45 = 778;
    public const int ERR_BUY_CASH_ITEM_52 = 785;
    public const int ERR_BUY_CASH_ITEM_53 = 786;
    public const int ERR_BUY_CASH_ITEM_55 = 790;
    public const int ERR_GIANT_BILLING_03 = 804;
    public const int ERR_EXCHANGE_CASH = 915;
    public const int ERR_CONTENT_00 = 935;
    public const int ERR_SHARING_BANK_01 = 942;
    public const int ERR_BUY_CASH_ITEM_56 = 952;
    public const int ERR_BUY_CASH_ITEM_57 = 953;
    public const int ERR_CHECK_MACHINE_ID_00 = 927;
    public const int ERR_BUY_CASH_ITEM_58 = 959;
    public const int ERR_BUY_CASH_ITEM_60 = 964;
    public const int ERR_BUY_CASH_ITEM_62 = 968;
    public const int ERR_BUY_CASH_ITEM_66 = 978;
    public const int ERR_BUY_CASH_ITEM_67 = 1001;
    public const int ERR_PET_21 = 1002;
    public const int ERR_BATTLEFIELD_07 = 1012;
    public const int ERR_HERO_PVP_USER_LIST_01 = 1122;
    public const int ERR_BUY_CASH_ITEM_68 = 1142;
    public const int ERR_BINGO_01 = 1149;
    public const int ERR_BINGO_02 = 1150;
    public const int ERR_BINGO_03 = 1151;
    public const int ERR_GLOBAL_BILLING_01 = 1234;
    public const int ERR_NX_COUPON_06 = 1321;
    public const int ERR_ADMIN_COMMAND_10 = 1364;
    public const int ERR_FIELD_BOSS_01 = 1366;
    public const int ERR_SKILL_PAGE_01 = 1387;
    public const int ERR_SKILL_PAGE_02 = 1390;
    public const int ERR_SKILL_PAGE_08 = 1393;
    public const int ERR_SKILL_PAGE_09 = 1394;
    public const int ERR_CLASS_CHANGE_EVENT_01 = 1395;

    // Batch 2 (TW NetError_def_TW.h exact ordinals + synthetic fallbacks for non-TW flags)
    public const int ERR_CREATE_UNIT_01 = 26;
    public const int ERR_CREATE_UNIT_03 = 28;
    public const int ERR_CREATE_UNIT_08 = 33;
    public const int ERR_CREATE_UNIT_11 = 1289;
    public const int ERR_ITEM_13 = 106;
    public const int ERR_QUEST_01 = 153;
    public const int ERR_QUEST_09 = 161;
    public const int ERR_SEARCH_UNIT_01 = 177;
    public const int ERR_PARTY_12 = 212;
    public const int ERR_PARTY_14 = 214;
    public const int ERR_PARTY_17 = 217;
    public const int ERR_PARTY_25 = 224;
    public const int ERR_DUNGEON_REQUIRE_ITEM_00 = 279;
    public const int ERR_STATISTICS_00 = 327;
    public const int ERR_RESTORE_NICK_NAME_05 = 333;
    public const int ERR_RESTORE_NICK_NAME_06 = 334;
    public const int ERR_TUTORIAL_01 = 406;
    public const int ERR_TUTORIAL_02 = 407;
    public const int ERR_USE_ITEM_IN_INVENTORY_05 = 653;
    public const int ERR_CUMULATIVE_TIME_EVENT_00 = 681;
    public const int ERR_CUMULATIVE_TIME_EVENT_01 = 682;
    public const int ERR_CUMULATIVE_TIME_EVENT_02 = 683;
    public const int ERR_SKILL_NOTE_00 = 730;
    public const int ERR_SKILL_NOTE_01 = 731;
    public const int ERR_SKILL_NOTE_02 = 732;
    public const int ERR_SKILL_NOTE_04 = 734;
    public const int ERR_WEB_POINT_EVENT_00 = 715;
    public const int ERR_DIF_SERVER_GROUP_03 = 741;
    public const int ERR_PET_05 = 752;
    public const int ERR_MIN_LEVEL_JOIN_PARTY_FAILE = 874;
    public const int ERR_MATCH_MAKING_18 = 924;
    public const int ERR_MATCH_MAKING_19 = 925;
    public const int ERR_USE_ITEM_IN_INVENTORY_08 = 1109;
    public const int ERR_RECOMMEND_USER_09 = 1174;
    public const int ERR_RECOMMEND_USER_08 = 1175;
    public const int ERR_LOCALRANKING_08 = 1177;
    public const int ERR_DELETE_UNIT_02 = 980;
    public const int ERR_DELETE_UNIT_00 = 986;
    public const int ERR_SEARCH_UNIT_04 = 179;
    public const int ERR_BATTLEFIELD_16 = 1140;
    public const int ERR_BATTLEFIELD_17 = 1141;
    public const int ERR_DELETE_UNIT_06 = 1258;
    public const int ERR_DANO_EVENT_02 = 1276;
    public const int ERR_JUMPING_04 = 1290;
    public const int ERR_JUMPING_01 = 1291;
    public const int ERR_JUMPING_07 = 1292;
    public const int ERR_JUMPING_06 = 1293;
    public const int ERR_JUMPING_03 = 1294;
    public const int ERR_JUMPING_08 = 1295;
    public const int ERR_JUMPING_02 = 1296;
    public const int ERR_JUMPING_05 = 1297;
    public const int ERR_EXPAND_INVENTORY_ED_01 = 1355;
    public const int ERR_SKILL_PAGE_03 = 1388;

    // Additional TW NetError_def_TW.h ordinals for unit/room/guild/dungeon checks
    public const int ERR_CREATE_UNIT_02 = 27;
    public const int ERR_ROOM_26 = 76;
    public const int ERR_GUILD_05 = 614;
    public const int ERR_GUILD_06 = 615;
    public const int ERR_GUILD_37 = 647;
    public const int ERR_GUILD_38 = 648;
    public const int ERR_GUILD_39 = 649;
    public const int ERR_GUILD_44 = 656;
    public const int ERR_SLOT_04 = 110;
    public const int ERR_TC_01 = 280;
    public const int ERR_TC_03 = 281;
    public const int ERR_TC_04 = 282;
    public const int ERR_ITEM_16 = 788;
    public const int ERR_ITEM_17 = 789;
    public const int ERR_SLOT_05 = 111;
    public const int ERR_DUNGEON_01 = 1342;
    public const int ERR_WISH_LIST_02 = 451;
    public const int ERR_TUTORIAL_07 = 544;
    public const int ERR_TUTORIAL_08 = 545;
    public const int ERR_TUTORIAL_09 = 546;
    public const int ERR_TUTORIAL_10 = 547;
    public const int ERR_ROOM_09 = 60;
    public const int NOT_LEAVE_ROOM_REASON_07 = 119;
    public const int NOT_LEAVE_ROOM_REASON_08 = 120;
    public const int NOT_LEAVE_ROOM_REASON_09 = 121;
    public const int ERR_TRADE_01 = 258;
    public const int ERR_TRADE_02 = 259;
    public const int ERR_TRADE_03 = 260;
    public const int ERR_TRADE_15 = 270;
    public const int ERR_TRADE_16 = 271;
    public const int ERR_TRADE_17 = 272;
    public const int ERR_TRADE_18 = 273;
    public const int ERR_DIF_SERVER_GROUP_00 = 738;
    public const int ERR_SN_OVERLAP_01 = 1028;
    public const int ERR_SN_OVERLAP_02 = 1029;
    public const int ERR_EXCHANGE_LIMIT_02 = 1279;
    public const int ERR_EXCHANGE_LIMIT_01 = 1280;
    public const int ERR_EXCHANGE_LIMIT_03 = 1281;
    public const int ERR_PERSONAL_SHOP_00 = 336;
    public const int ERR_PERSONAL_SHOP_05 = 340;
    public const int ERR_PERSONAL_SHOP_07 = 342;
    public const int ERR_PERSONAL_SHOP_08 = 343;
    public const int ERR_PERSONAL_SHOP_09 = 344;
    public const int ERR_PERSONAL_SHOP_10 = 345;
    public const int ERR_PERSONAL_SHOP_12 = 347;
    public const int ERR_PERSONAL_SHOP_14 = 348;
    public const int ERR_PERSONAL_SHOP_15 = 349;
    public const int ERR_PERSONAL_SHOP_16 = 350;
    public const int ERR_PERSONAL_SHOP_17 = 351;
    public const int ERR_PERSONAL_SHOP_20 = 352;
    public const int ERR_PERSONAL_SHOP_22 = 354;
    public const int ERR_PERSONAL_SHOP_23 = 355;
    public const int ERR_PERSONAL_SHOP_24 = 356;
    public const int ERR_PERSONAL_SHOP_25 = 357;
    public const int ERR_PERSONAL_SHOP_26 = 721;
    public const int ERR_PERSONAL_SHOP_27 = 722;
    public const int ERR_PERSONAL_SHOP_28 = 720;
    public const int ERR_PERSONAL_SHOP_32 = 727;
    public const int ERR_PERSONAL_SHOP_34 = 824;

    // Non-TW feature-gated synthetic error codes
    public const int ERR_4TH_ANNIV_EVENT_01 = 60101;
    public const int ERR_4TH_ANNIV_EVENT_02 = 60102;
    public const int ERR_STRING_FILTER_00 = 60103;
    public const int ERR_STRING_FILTER_01 = 60104;
    public const int ERR_STRING_FILTER_02 = 60105;
    public const int ERR_STRING_FILTER_03 = 60106;

    // Additional TW item/enchant/socket/manufacture/attrib ordinals (NetError_def_TW.h)
    public const int ERR_ITEM_00 = 94;
    public const int ERR_ITEM_01 = 95;
    public const int ERR_RANDOM_ITEM_04 = 286;
    public const int ERR_RANDOM_ITEM_05 = 287;
    public const int ERR_MANUFACTURE_00 = 300;
    public const int ERR_MANUFACTURE_02 = 301;
    public const int ERR_MANUFACTURE_03 = 302;
    public const int ERR_MANUFACTURE_05 = 303;
    public const int ERR_ENCHANT_ITEM_00 = 366;
    public const int ERR_ENCHANT_ITEM_01 = 367;
    public const int ERR_ENCHANT_ITEM_02 = 368;
    public const int ERR_ENCHANT_ITEM_03 = 369;
    public const int ERR_ENCHANT_ITEM_04 = 370;
    public const int ERR_ENCHANT_ITEM_05 = 371;
    public const int ERR_ENCHANT_ITEM_06 = 372;
    public const int ERR_SOCKET_ITEM_00 = 392;
    public const int ERR_SOCKET_ITEM_01 = 393;
    public const int ERR_SOCKET_ITEM_05 = 397;
    public const int ERR_ATTRIB_ENCHANT_00 = 459;
    public const int ERR_ATTRIB_ENCHANT_01 = 460;
    public const int ERR_ATTRIB_ENCHANT_02 = 461;
    public const int ERR_ATTRIB_ENCHANT_05 = 464;
    public const int ERR_ATTRIB_ENCHANT_06 = 465;
    public const int ERR_RESTORE_ITEM_06 = 386;
    public const int ERR_ITEM_EXCHANGE_00 = 508;
    public const int ERR_ITEM_EXCHANGE_01 = 509;
    public const int ERR_BANK_01 = 569;
    public const int ERR_BANK_02 = 570;
    public const int ERR_SEAL_ITEM_00 = 579;
    public const int ERR_SEAL_ITEM_01 = 580;
    public const int ERR_SEAL_ITEM_02 = 581;
    public const int ERR_PC_BANG_00 = 690;
    public const int ERR_ITEM_15 = 746;
    public const int ERR_PERSONAL_SHOP_51 = 871;
    public const int ERR_EXPAND_INVENTORY_ED_02 = 1356;
    public const int ERR_EXPAND_INVENTORY_ED_03 = 1357;
    public const int ERR_FIELD_BOSS_13 = 1373;
    public const int ERR_FIELD_BOSS_10 = 1376;
    public const int ERR_FIELD_BOSS_09 = 1379;
    public const int ERR_FIELD_BOSS_12 = 1380;
    public const int ERR_FIELD_BOSS_11 = 1381;
    public const int ERR_FIELD_BOSS_08 = 1384;

    // Additional TW Unit/Room/Party/Skill/Pet/RidingPet/SecondSecurity/Admin ordinals (NetError_def_TW.h)
    public const int ERR_CREATE_UNIT_00 = 25;
    public const int ERR_SELECT_UNIT_00 = 35;
    public const int ERR_ROOM_02 = 54;
    public const int ERR_ROOM_03 = 55;
    public const int ERR_ROOM_04 = 56;
    public const int ERR_ROOM_06 = 57;
    public const int ERR_ROOM_08 = 59;
    public const int ERR_ROOM_10 = 61;
    public const int ERR_ROOM_11 = 62;
    public const int ERR_ROOM_12 = 63;
    public const int ERR_ROOM_13 = 64;
    public const int ERR_ROOM_14 = 65;
    public const int ERR_ROOM_15 = 66;
    public const int ERR_ADMIN_COMMAND_00 = 142;
    public const int ERR_ADMIN_COMMAND_01 = 143;
    public const int ERR_ADMIN_COMMAND_03 = 144;
    public const int ERR_ADMIN_COMMAND_04 = 145;
    public const int ERR_ADMIN_COMMAND_07 = 148;
    public const int ERR_ADMIN_COMMAND_08 = 149;
    public const int ERR_ACCOUNT_BLOCK_00 = 744;
    public const int ERR_BUY_ED_ITEM_05 = 151;
    public const int ERR_SELL_ED_ITEM_05 = 152;
    public const int ERR_QUEST_02 = 154;
    public const int ERR_SKILL_00 = 182;
    public const int ERR_SKILL_03 = 184;
    public const int ERR_PARTY_07 = 210;
    public const int ERR_PARTY_09 = 211;
    public const int ERR_PARTY_13 = 213;
    public const int ERR_PERSONAL_SHOP_01 = 337;
    public const int ERR_PERSONAL_SHOP_03 = 338;
    public const int ERR_PERSONAL_SHOP_04 = 339;
    public const int ERR_TITLE_00 = 494;
    public const int ERR_TITLE_01 = 495;
    public const int ERR_USE_ITEM_IN_INVENTORY_00 = 500;
    public const int ERR_USE_ITEM_IN_INVENTORY_01 = 501;
    public const int ERR_USE_ITEM_IN_INVENTORY_02 = 502;
    public const int ERR_MESSENGER_13 = 552;
    public const int ERR_MESSENGER_14 = 553;
    public const int ERR_PET_01 = 748;
    public const int ERR_PET_02 = 749;
    public const int ERR_PET_03 = 750;
    public const int ERR_PET_04 = 751;
    public const int ERR_PET_07 = 754;
    public const int ERR_PET_08 = 755;
    public const int ERR_PET_09 = 756;
    public const int ERR_SECOND_SECURITY_AUTH_FAILED = 856;
    public const int ERR_SECOND_SECURITY_DIFFER_PW = 859;
    public const int ERR_SECOND_SECURITY_NO_USE = 893;
    public const int NOT_PARTY_GAME_START_FAILED_00 = 1022;
    public const int ERR_RIDING_PET_03 = 1214;
    public const int ERR_RIDING_PET_02 = 1215;
    public const int ERR_RIDING_PET_01 = 1217;
    public const int ERR_RIDING_PET_07 = 1219;
    public const int ERR_RIDING_PET_06 = 1220;
    public const int ERR_RIDING_PET_09 = 1221;
    public const int ERR_RIDING_PET_08 = 1222;
    public const int ERR_MATCH_MAKING_15 = 920;
    public const int ERR_CHECK_DLL_01 = 972;
    public const int ERR_ROOM_24 = 75;
    public const int ERR_FIELD_01 = 516;
    public const int NOT_LEAVE_ROOM_REASON_16 = 127;
    public const int ERR_TRADE_13 = 268;
    public const int NOT_REQUEST_TRADE_REPLY_02 = 276;
    public const int ERR_RESOLVE_ITEM_00 = 358;
    public const int ERR_RESOLVE_ITEM_01 = 359;
    public const int ERR_RESOLVE_ITEM_02 = 360;
    public const int ERR_RESOLVE_ITEM_03 = 361;
    public const int ERR_RESOLVE_ITEM_04 = 362;
    public const int ERR_RESOLVE_ITEM_06 = 364;
    public const int ERR_RESOLVE_ITEM_08 = 365;
    public const int ERR_POST_LETTER_03 = 475;
    public const int ERR_POST_LETTER_11 = 482;
    public const int ERR_BANK_00 = 568;
    public const int ERR_TRADE_BLOCK_UNIT_00 = 938;
    public const int ERR_TRADE_BLOCK_UNIT_01 = 939;
    public const int ERR_BLOCK_TRADE_07 = 1310;
    public const int NOT_LEAVE_ROOM_REASON_00 = 114;
    public const int NOT_LEAVE_ROOM_REASON_01 = 115;
    public const int NOT_LEAVE_ROOM_REASON_02 = 116;
    public const int NOT_LEAVE_ROOM_REASON_06 = 118;
    public const int NOT_LEAVE_ROOM_REASON_14 = 125;
    public const int NOT_LEAVE_ROOM_REASON_22 = 800;
    public const int NOT_LEAVE_ROOM_REASON_32 = 1094;
    public const int NOT_LEAVE_PARTY_REASON_02 = 136;
    public const int NOT_REQUEST_TRADE_REPLY_04 = 278;
    public const int ERR_UDP_01 = 1187;

    public static int GetRegionalCode(RegionCode region, NetErrorKey key) => NetErrorRegistry.Resolve(region, key);
}
