using KncWX2Server.Core.Framework;
using KncWX2Server.Core.Logging;
using KncWX2Server.Core.Packets;
using KncWX2Server.Core.Scripting;
using MoonSharp.Interpreter;

namespace KncWX2Server.GameServer;

/// <summary>
/// Central MoonSharp Lua ServerResource loader for KGameServer and its sibling Center/Global managers.
/// Loads regional ServerResource/{Region}/*.lua files with fallback to ServerResource/*.lua.
/// </summary>
public static class KServerResourceLoader
{
    public static void BindGlobalProxies(KGameServer game, KScriptManager script)
    {
        game.GameSysValTable.BindToLua(script, "GameSysVal");
        game.EdInventoryExpand.BindToLua(script, "g_pEDInventoryExpand");
        game.SynthesisManager.BindToLua(script, "g_pSynthesisManager");
        game.ResolveItemManager.BindToLua(script, "g_pResolveItemManager");
        game.EnchantItemManager.BindToLua(script, "EnchantItemManager");
        game.SocketItemManager.BindSocketToLua(script, "g_pCX2SocketItem");
        game.SocketItemManager.BindAttribToLua(script, "g_pCX2EnchantItem");
        game.RandomItemManager.BindToLua(script, "g_pRandomItemManager");
        game.ManufactureItemManager.BindToLua(script, "g_pManufactureItemManager");
        game.StatTable.BindToLua(script, "StatTable");
        game.GameEventManager.BindToLua(script, "GameEvent");
        game.ClassChangeTable.BindToLua(script, "ClassChangeTable");
        game.FieldManager.BindToLua(script, "FieldManager");
        game.PetManager.BindToLua(script, "g_pPetManager");
        game.TrainingCenterTable.BindToLua(script, "KTrainingCenterTable");
        game.ExpTable.BindToLua(script, "g_pkExpTable");
        game.SpiritTable.BindToLua(script, "g_pkSpiritTable");
        game.RewardTable.BindToLua(script, "g_pKRewardTable");
        game.ScriptRefreshManager.BindToLua(script, "ScriptRefresh");

        BindMiscProxies(script, game);
    }

    public static int LoadOrder(KGameServer game, int orderType, string? resourceBaseDir = null)
    {
        var files = orderType switch
        {
            KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_EXP_TABLE => new[] { "ExpTable.lua" },
            KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_REWARD_TABLE => new[] { "RewardTable.lua" },
            KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_GAME_SYS_VAL => new[] { "GameSysValTable.lua" },
            KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_ED_INVENTORY_EXPAND => new[] { "EDInventoryExpand.lua" },
            KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_SYNTHESIS_TABLE => new[] { "SynthesisTable.lua" },
            KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_RESOLVE_ITEM_MANAGER => new[] { "ResolveTable.lua" },
            KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_ENCHANT_ITEM_MANAGER => new[] { "EnchantTable.lua" },
            KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_ATTRIB_ENCHANT_ITEM => new[] { "AttribEnchantTable.lua", "AttribAttachTable.lua" },
            KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_SOCKET_ITEM => new[] { "ItemRandomSocket.lua", "SocketItemTable.lua" },
            KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_RANDOM_ITEM_MANAGER => new[] { "RandomItemData.lua", "RandomItemMapping.lua", "RandomItemTable.lua" },
            KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_MANUFACTURE_ITEM_MANAGER => new[] { "ManufactureResultTable.lua" },
            KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_CLASS_CHANGE_TABLE => new[] { "ClassChangeTable.lua", "ClassChangeItem.lua", "ClassChangeMemo.lua", "ClassChangeSkill.lua", "ClassChangeQuest.lua" },
            KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_COMPANY_IP_TABLE => new[] { "CompanyIPTable.lua" },
            KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_PET_MANAGER => new[] { "PetData.lua" },
            KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_RIDING_PET_MANAGER => new[] { "RidingPetData.lua" },
            KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_CONTENT_MANAGER => new[] { "ContentData.lua" },
            KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_RECOMMEND_USER_TABLE => new[] { "RecommendUserTable.lua" },
            KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_ABUSER_LOG_MANAGER => new[] { "AbuserList.lua" },
            KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_DAILY_GIFT_BOX_MANAGER => new[] { "DailyGiftBoxPackage.lua" },
            KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_BINGO_EVENT_MANAGER => new[] { "BingoEventInfo.lua" },
            KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_GAME_EVENT_SCRIPT_MANAGER => new[] { "EventData.lua" },
            KESR_SCRIPT_REFRESH_ORDER_NOT.OT_GS_MAP_DATA => new[] { "FieldData.lua", "MapData.lua" },
            _ => Array.Empty<string>(),
        };

        if (files.Length == 0)
            return LoadAll(game, resourceBaseDir, includeHeavyTables: false);

        var script = new KScriptManager();
        BindGlobalProxies(game, script);
        int loaded = 0;
        foreach (string file in files)
        {
            if (script.DoServerResourceFile(file, game.Region, resourceBaseDir))
                loaded++;
        }
        return loaded;
    }

    public static int LoadAll(
        KGameServer game,
        string? resourceBaseDir = null,
        bool includeHeavyTables = true)
    {
        var script = new KScriptManager();
        BindGlobalProxies(game, script);

        var files = new List<string>
        {
            "GameSysValTable.lua",
            "ExpTable.lua",
            "SpiritTable.lua",
            "RewardTable.lua",
            "EDInventoryExpand.lua",
            "SynthesisTable.lua",
            "ResolveTable.lua",
            "EnchantTable.lua",
            "AttribEnchantTable.lua",
            "AttribAttachTable.lua",
            "MiracleCubeTable.lua",
            "ItemRandomSocket.lua",
            "SocketItemTable.lua",
            "RandomItemData.lua",
            "RandomItemMapping.lua",
            "ClassChangeTable.lua",
            "ClassChangeItem.lua",
            "ClassChangeMemo.lua",
            "ClassChangeSkill.lua",
            "ClassChangeQuest.lua",
            "CompanyIPTable.lua",
            "PetData.lua",
            "RidingPetData.lua",
            "ContentData.lua",
            "CorruptionMonitorConfig.lua",
            "RecommendUserTable.lua",
            "AbuserList.lua",
            "ValenTineData.lua",
            "EventMonsterData.lua",
            "ExpandPeriodItemData.lua",
            "DeleteItemData.lua",
            "DailyGiftBoxPackage.lua",
            "SubTitleOpenMission.lua",
            "BingoEventInfo.lua",
            "CashItemData.lua",
            "CashItemPrice.lua",
            "BillingConfig.lua",
            "EventData.lua",
            "FieldData.lua",
            "MapData.lua",
            "RoomListData.lua",
            "KeepItem.lua",
            "KFLOAT_FUNC.lua",
            "ResultData.lua",
            "TrainingCenterTemplet.lua",
            "LogConfig.lua",
            "ServerMornitoringConfig.lua",
        };

        if (includeHeavyTables)
        {
            files.Add("StatTable.lua");
            files.Add("RandomItemTable.lua");
            files.Add("ManufactureResultTable.lua");
        }

        int loaded = 0;
        foreach (string file in files)
        {
            if (script.DoServerResourceFile(file, game.Region, resourceBaseDir))
                loaded++;
        }

        loaded += game.Center.LoadServerResources(resourceBaseDir, includeHeavyDropTables: includeHeavyTables);
        loaded += game.Global.LoadServerResources(resourceBaseDir);

        string? floatFuncPath = KScriptManager.ResolveServerResourceFile("KFLOAT_FUNC.lua", game.Region, resourceBaseDir);
        if (floatFuncPath != null && File.Exists(floatFuncPath))
        {
            string raw = File.ReadAllText(floatFuncPath);
            if (raw.Length > 0 && raw[0] == '\uFEFF') raw = raw[1..];
            game.ClientScripts[0] = raw;
        }

        KLog.Cout($"[KServerResourceLoader] Loaded {loaded} Lua resource scripts for region {game.Region.Code}.");
        return loaded;
    }

    private static void BindMiscProxies(KScriptManager script, KGameServer game)
    {
        Table? ipProxy = null;
        ipProxy = script.CreateLuaProxy("g_pKCompanyIPTable", t =>
        {
            t["AddCompanyIP"] = DynValue.NewCallback((_, a) =>
            {
                string ip = KScriptManager.ArgString(a, KScriptManager.ArgStart(a, ipProxy!));
                game.CompanyIpTable.AddCompanyIp(ip);
                return DynValue.Nil;
            });
        });

        Table? rpProxy = null;
        rpProxy = script.CreateLuaProxy("g_pRidingPetManager", t =>
        {
            t["AddRidingPetCreateItemInfo"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, rpProxy!);
                game.RidingPetManager.AddRidingPetCreateItemInfo(
                    KScriptManager.ArgInt(a, o),
                    (ushort)Math.Max(0, KScriptManager.ArgInt(a, o + 1)),
                    (short)KScriptManager.ArgInt(a, o + 2));
                return DynValue.Nil;
            });
        });

        Table? contentProxy = null;
        contentProxy = script.CreateLuaProxy("GSContentManager", t =>
        {
            t["SetEnableCashShop"] = DynValue.NewCallback((_, a) =>
            {
                game.ContentManager.SetEnableCashShopLua(KScriptManager.ArgBool(a, KScriptManager.ArgStart(a, contentProxy!), true));
                return DynValue.Nil;
            });
        });

        Table? cmProxy = null;
        cmProxy = script.CreateLuaProxy("g_pKCorruptionMonitor", t =>
        {
            t["SetMornitoringOn"] = DynValue.NewCallback((_, a) =>
            {
                game.CorruptionMonitor.MonitoringOn = KScriptManager.ArgBool(a, KScriptManager.ArgStart(a, cmProxy!));
                return DynValue.Nil;
            });
        });

        game.RecommendUserTable.BindToLua(script, "g_pKRecommendUserTable");
        KUserAbuserManager.BindToLua(script, "AbuserLogManager");
        script.CreateLuaProxy("g_pCX2SetItemManager", _ => { });

        Table? itemProxy = null;
        itemProxy = script.CreateLuaProxy("g_pItemManager", t =>
        {
            t["AddItemExpandPeriodData"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, itemProxy!);
                int id = KScriptManager.ArgInt(a, o);
                int days = KScriptManager.ArgInt(a, o + 1);
                if (id > 0) game.ExpandPeriodItems[id] = days;
                return DynValue.Nil;
            });
            t["AddDeleteItemData"] = DynValue.NewCallback((_, a) =>
            {
                int id = KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, itemProxy!));
                if (id > 0) game.DeleteItemIds.Add(id);
                return DynValue.Nil;
            });
            t["AddKeepShowItem"] = DynValue.NewCallback((_, a) =>
            {
                var tbl = KScriptManager.ArgTable(a, KScriptManager.ArgStart(a, itemProxy!));
                if (tbl != null)
                {
                    int type = (int)(tbl.Get("m_Type").CastToNumber() ?? 0);
                    int showItem = (int)(tbl.Get("m_ShowItem").CastToNumber() ?? 0);
                    int proOne = (int)(tbl.Get("m_ProDuctNumber_One").CastToNumber() ?? 0);
                    int proTwo = (int)(tbl.Get("m_ProDuctNumber_Two").CastToNumber() ?? proOne);
                    var keys = new List<int>();
                    var keyTbl = tbl.Get("KEEP_SHOW_ITEM_KEY").Table;
                    if (keyTbl != null)
                    {
                        foreach (var pair in keyTbl.Pairs)
                        {
                            int k = (int)(pair.Value.CastToNumber() ?? 0);
                            if (k > 0) keys.Add(k);
                        }
                    }
                    if (showItem > 0)
                    {
                        if (type == 2)
                        {
                            game.KeepDiscountItems[showItem] = new KncWX2Server.Core.Packets.kDisCountItemInfo
                            {
                                DisCountProNo = proOne,
                                BasicProNo = proTwo,
                                DisCountKeepItem = keys,
                            };
                        }
                        else
                        {
                            game.KeepShowItems[showItem] = keys;
                        }
                    }
                }
                return DynValue.Nil;
            });
            t["AddItemConvertGroup"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, itemProxy!);
                int grp = KScriptManager.ArgInt(a, o);
                int itemId = KScriptManager.ArgInt(a, o + 1);
                if (grp > 0 && itemId > 0)
                {
                    if (!game.ItemConvertGroups.TryGetValue(grp, out var list))
                        game.ItemConvertGroups[grp] = list = [];
                    list.Add(itemId);
                }
                return DynValue.Nil;
            });
            t["AddItemConvertInfo"] = DynValue.NewCallback((_, a) =>
            {
                var tbl = KScriptManager.ArgTable(a, KScriptManager.ArgStart(a, itemProxy!));
                if (tbl != null)
                {
                    var targets = new List<int>();
                    int grp = (int)(tbl.Get("m_TargetItemGroup").CastToNumber() ?? 0);
                    if (grp > 0 && game.ItemConvertGroups.TryGetValue(grp, out var grpList))
                        targets.AddRange(grpList);
                    var targetTbl = tbl.Get("m_TargetItemID").Table;
                    if (targetTbl != null)
                    {
                        foreach (var pair in targetTbl.Pairs)
                        {
                            int tid = (int)(pair.Value.CastToNumber() ?? 0);
                            if (tid > 0) targets.Add(tid);
                        }
                    }
                    int convType = (int)(tbl.Get("m_ConvertType").CastToNumber() ?? 0);
                    int comm = (int)(tbl.Get("m_Commission").CastToNumber() ?? 0);
                    var results = new List<int>();
                    var resTbl = tbl.Get("m_ResultItemID").Table;
                    if (resTbl != null)
                    {
                        foreach (var pair in resTbl.Pairs)
                        {
                            int rid = (int)(pair.Value.CastToNumber() ?? 0);
                            if (rid > 0) results.Add(rid);
                        }
                    }
                    foreach (int tid in targets)
                        game.ItemConvertTable[tid] = (convType, comm, results);
                }
                return DynValue.Nil;
            });
        });

        Table? giftProxy = null;
        giftProxy = script.CreateLuaProxy("g_pKDailyGiftBoxManager", t =>
        {
            t["AddPackageInfo"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, giftProxy!);
                int itemId = KScriptManager.ArgInt(a, o);
                int scriptId = KScriptManager.ArgInt(a, o + 1);
                game.DailyGiftBoxManager.AddPackageInfo(itemId, scriptId);
                return DynValue.Nil;
            });
        });

        Table? titleProxy = null;
        titleProxy = script.CreateLuaProxy("TitleManager", t =>
        {
            t["AddSubTitleMissionInfo_LUA"] = DynValue.NewCallback((_, a) =>
            {
                var tbl = KScriptManager.ArgTable(a, KScriptManager.ArgStart(a, titleProxy!));
                if (tbl != null)
                {
                    int id = (int)(tbl.Get("m_iID").CastToNumber() ?? 0);
                    int clearType = (int)(tbl.Get("m_eClearType").CastToNumber() ?? 0);
                    var cond = tbl.Get("m_ClearCondition").Table;
                    int dgnId = (int)(cond?.Get("m_eDungeonID").CastToNumber() ?? 0);
                    int diff = (int)(cond?.Get("m_cDifficulty").CastToNumber() ?? -1);
                    int count = (int)(cond?.Get("m_iDungeonClearCount").CastToNumber() ?? 1);
                    if (id > 0) game.SubTitleOpenMissions[id] = (clearType, dgnId, diff, count);
                }
                return DynValue.Nil;
            });
        });

        Table? bingoProxy = null;
        bingoProxy = script.CreateLuaProxy("KGSBingoEventInfo", t =>
        {
            t["AddBanSpotInfo"] = DynValue.NewCallback((_, a) =>
            {
                int spot = KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, bingoProxy!), -1);
                if (spot >= 0) game.BingoBanSpots.Add(spot);
                return DynValue.Nil;
            });
            t["AddPresentInfo"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, bingoProxy!);
                int itemId = KScriptManager.ArgInt(a, o);
                uint prodNo = (uint)Math.Max(0, KScriptManager.ArgInt(a, o + 1));
                bool isBan = KScriptManager.ArgBool(a, o + 2);
                if (itemId > 0) game.BingoPresentInfos[itemId] = (prodNo, isBan);
                return DynValue.Nil;
            });
            t["AddPackageItemPrice"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, bingoProxy!);
                uint prodNo = (uint)Math.Max(0, KScriptManager.ArgInt(a, o));
                uint price = (uint)Math.Max(0, KScriptManager.ArgInt(a, o + 1));
                if (prodNo > 0) game.BingoPackageItemPrices[prodNo] = price;
                return DynValue.Nil;
            });
            t["AddPackageItemPriceForGift"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, bingoProxy!);
                uint prodNo = (uint)Math.Max(0, KScriptManager.ArgInt(a, o));
                uint price = (uint)Math.Max(0, KScriptManager.ArgInt(a, o + 1));
                if (prodNo > 0) game.BingoPackageGiftPrices[prodNo] = price;
                return DynValue.Nil;
            });
        });

        Table? cashProxy = null;
        cashProxy = script.CreateLuaProxy("g_pCashItemManager", t =>
        {
            t["AddCashItemServerGroupData"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, cashProxy!);
                int sgi = KScriptManager.ArgInt(a, o);
                int itemId = KScriptManager.ArgInt(a, o + 1);
                if (itemId > 0)
                {
                    if (!game.CashItemServerGroupBlocked.TryGetValue(sgi, out var set))
                        game.CashItemServerGroupBlocked[sgi] = set = [];
                    set.Add(itemId);
                }
                return DynValue.Nil;
            });
            t["AddCashItemProductNoData"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, cashProxy!);
                int sgi = KScriptManager.ArgInt(a, o);
                uint prodNo = (uint)Math.Max(0, KScriptManager.ArgInt(a, o + 1));
                if (prodNo > 0)
                {
                    if (!game.CashItemProductNoServerGroupBlocked.TryGetValue(sgi, out var set))
                        game.CashItemProductNoServerGroupBlocked[sgi] = set = [];
                    set.Add(prodNo);
                }
                return DynValue.Nil;
            });
            t["AddCashItemPrice"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, cashProxy!);
                int itemId = KScriptManager.ArgInt(a, o);
                uint price = (uint)Math.Max(0, KScriptManager.ArgInt(a, o + 1));
                if (itemId > 0) game.CashItemPrices[itemId] = price;
                return DynValue.Nil;
            });
            t["AddCashItemGroup"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, cashProxy!);
                int groupId = KScriptManager.ArgInt(a, o);
                int itemId = KScriptManager.ArgInt(a, o + 1);
                if (itemId > 0 && !game.CashItemGroupMap.ContainsKey(itemId))
                    game.CashItemGroupMap[itemId] = groupId;
                return DynValue.Nil;
            });
            t["AddCashItemResolveAdd"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, cashProxy!);
                int groupId = KScriptManager.ArgInt(a, o);
                int itemId = KScriptManager.ArgInt(a, o + 1);
                int qty = Math.Max(1, KScriptManager.ArgInt(a, o + 2, 1));
                double prob = KScriptManager.ArgDouble(a, o + 3);
                if (itemId > 0)
                {
                    if (!game.CashItemResolveGroups.TryGetValue(groupId, out var list))
                        game.CashItemResolveGroups[groupId] = list = [];
                    list.Add((itemId, qty, prob));
                }
                return DynValue.Nil;
            });
        });

        Table? billingProxy = null;
        billingProxy = script.CreateLuaProxy("g_pBillingManager", t =>
        {
            t["AddProductCategoryModifyForItemID"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, billingProxy!);
                int itemId = KScriptManager.ArgInt(a, o);
                int serviceCat = KScriptManager.ArgInt(a, o + 1);
                if (itemId > 0 && serviceCat > 0)
                    game.BillingProductCategoryOverrides[itemId] = serviceCat;
                return DynValue.Nil;
            });
        });

        Table? mapProxy = null;
        mapProxy = script.CreateLuaProxy("MapData", t =>
        {
            t["AddMapData"] = DynValue.NewCallback((_, a) =>
            {
                var tbl = KScriptManager.ArgTable(a, KScriptManager.ArgStart(a, mapProxy!));
                if (tbl != null)
                {
                    int id = (int)(tbl.Get("ID").CastToNumber() ?? 0);
                    int before = (int)(tbl.Get("BEFORE").CastToNumber() ?? 0);
                    int after = (int)(tbl.Get("AFTER").CastToNumber() ?? 0);
                    var defVal = tbl.Get("DEFAULT");
                    bool isDef = defVal.Type == DataType.Boolean && defVal.Boolean;
                    var reqLv = tbl.Get("REQUIRE_LEVEL").Table;
                    int minLv = (int)(reqLv?.Get("MIN").CastToNumber() ?? 1);
                    int maxLv = (int)(reqLv?.Get("MAX").CastToNumber() ?? 100);
                    var reqDungeons = new List<int>();
                    var reqDgnTbl = tbl.Get("REQUIRE_DUNGEON").Table;
                    if (reqDgnTbl != null)
                    {
                        foreach (var pair in reqDgnTbl.Pairs)
                        {
                            int dgnId = (int)(pair.Value.CastToNumber() ?? 0);
                            if (dgnId > 0) reqDungeons.Add(dgnId);
                        }
                    }
                    if (id > 0) game.MapDatas[id] = new KGameServer.MapDataEntry(id, before, after, isDef, minLv, maxLv, reqDungeons);
                }
                return DynValue.Nil;
            });
            t["AddLocalMapInfo"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, mapProxy!);
                int localMapId = KScriptManager.ArgInt(a, o);
                int mode = KScriptManager.ArgInt(a, o + 1);
                int dgnId = KScriptManager.ArgInt(a, o + 2);
                game.LocalMapInfos.Add((localMapId, mode, dgnId));
                return DynValue.Nil;
            });
        });

        Table? roomListProxy = null;
        roomListProxy = script.CreateLuaProxy("g_pRoomListManager", t =>
        {
            t["SetStartPVPID"] = DynValue.NewCallback((_, a) =>
            {
                game.RoomListStartPvpId = KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, roomListProxy!), game.RoomListStartPvpId);
                return DynValue.Nil;
            });
            t["SetTournamentPVPID"] = DynValue.NewCallback((_, a) =>
            {
                game.RoomListTournamentPvpId = KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, roomListProxy!), game.RoomListTournamentPvpId);
                return DynValue.Nil;
            });
            t["SetPvpOffcialRoomListID"] = DynValue.NewCallback((_, a) =>
            {
                game.RoomListOfficialPvpId = KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, roomListProxy!), game.RoomListOfficialPvpId);
                return DynValue.Nil;
            });
            t["AddRoomListTemplet"] = DynValue.NewCallback((_, a) =>
            {
                var tbl = KScriptManager.ArgTable(a, KScriptManager.ArgStart(a, roomListProxy!));
                if (tbl != null)
                {
                    int id = (int)(tbl.Get("ROOM_LIST_ID").CastToNumber() ?? 0);
                    int type = (int)(tbl.Get("ROOM_LIST_TYPE").CastToNumber() ?? 0);
                    int pvpClass = (int)(tbl.Get("PVP_CLASS").CastToNumber() ?? 0);
                    int minLv = (int)(tbl.Get("MIN_LEVEL").CastToNumber() ?? 0);
                    int maxLv = (int)(tbl.Get("MAX_LEVEL").CastToNumber() ?? 0);
                    int minVp = (int)(tbl.Get("MIN_VP").CastToNumber() ?? 0);
                    int maxVp = (int)(tbl.Get("MAX_VP").CastToNumber() ?? 0);
                    if (id > 0) game.RoomListTemplates[id] = new KGameServer.RoomListTemplate(id, type, pvpClass, minLv, maxLv, minVp, maxVp);
                }
                return DynValue.Nil;
            });
        });

        Table? logMgrProxy = null;
        logMgrProxy = script.CreateLuaProxy("LogManager", t =>
        {
            t["SetNewLogByFileSize"] = DynValue.NewCallback((_, a) =>
            {
                KLog.NewLogByFileSize = KScriptManager.ArgBool(a, KScriptManager.ArgStart(a, logMgrProxy!));
                return DynValue.Nil;
            });
            t["SetNewLogDaily"] = DynValue.NewCallback((_, a) =>
            {
                KLog.NewLogDaily = KScriptManager.ArgBool(a, KScriptManager.ArgStart(a, logMgrProxy!));
                return DynValue.Nil;
            });
            t["SetErrorLogStat"] = DynValue.NewCallback((_, a) =>
            {
                KLog.ErrorLogStatEnabled = KScriptManager.ArgBool(a, KScriptManager.ArgStart(a, logMgrProxy!), true);
                return DynValue.Nil;
            });
            t["SetCheckTimeGap"] = DynValue.NewCallback((_, a) =>
            {
                KLog.CheckTimeGapSeconds = Math.Max(1, KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, logMgrProxy!), KLog.CheckTimeGapSeconds));
                return DynValue.Nil;
            });
            t["SetErrorLogStatTimeGap"] = DynValue.NewCallback((_, a) =>
            {
                KLog.ErrorLogStatTimeGapSeconds = Math.Max(1, KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, logMgrProxy!), KLog.ErrorLogStatTimeGapSeconds));
                return DynValue.Nil;
            });
            t["SetLogFileMaxSize"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, logMgrProxy!);
                long mb = Math.Max(0, KScriptManager.ArgInt(a, o, 10));
                long kb = Math.Max(0, KScriptManager.ArgInt(a, o + 1, 0));
                long bytes = (mb * 1024L + kb) * 1024L;
                KLog.LogFileMaxSizeBytes = Math.Max(1024L, bytes);
                return DynValue.Nil;
            });
        });

        Table? monMgrProxy = null;
        monMgrProxy = script.CreateLuaProxy("MornitoringManager", t =>
        {
            t["SetSendSMS"] = DynValue.NewCallback((_, a) =>
            {
                game.MornitoringConfig.SendSms = KScriptManager.ArgBool(a, KScriptManager.ArgStart(a, monMgrProxy!));
                return DynValue.Nil;
            });
            t["SetSendMail"] = DynValue.NewCallback((_, a) =>
            {
                game.MornitoringConfig.SendMail = KScriptManager.ArgBool(a, KScriptManager.ArgStart(a, monMgrProxy!));
                return DynValue.Nil;
            });
            t["AddDumpServerUID"] = DynValue.NewCallback((_, a) =>
            {
                int uid = KScriptManager.ArgInt(a, KScriptManager.ArgStart(a, monMgrProxy!));
                if (uid > 0) game.MornitoringConfig.DumpServerUids.Add(uid);
                return DynValue.Nil;
            });
            t["AddPhoneNum"] = DynValue.NewCallback((_, a) =>
            {
                string num = KScriptManager.ArgString(a, KScriptManager.ArgStart(a, monMgrProxy!));
                if (!string.IsNullOrWhiteSpace(num)) game.MornitoringConfig.PhoneNumbers.Add(num);
                return DynValue.Nil;
            });
            t["AddServerNetworkMailAddr"] = DynValue.NewCallback((_, a) =>
            {
                string addr = KScriptManager.ArgString(a, KScriptManager.ArgStart(a, monMgrProxy!));
                if (!string.IsNullOrWhiteSpace(addr)) game.MornitoringConfig.ServerNetworkMailAddrs.Add(addr);
                return DynValue.Nil;
            });
            t["AddServerDumpMailAddr"] = DynValue.NewCallback((_, a) =>
            {
                string addr = KScriptManager.ArgString(a, KScriptManager.ArgStart(a, monMgrProxy!));
                if (!string.IsNullOrWhiteSpace(addr)) game.MornitoringConfig.ServerDumpMailAddrs.Add(addr);
                return DynValue.Nil;
            });
            t["CheckModuleList_X2Module"] = DynValue.NewCallback((_, a) =>
            {
                game.MornitoringConfig.CheckX2Module = KScriptManager.ArgBool(a, KScriptManager.ArgStart(a, monMgrProxy!));
                return DynValue.Nil;
            });
            t["AddCheckModuleList"] = DynValue.NewCallback((_, a) =>
            {
                string mod = KScriptManager.ArgString(a, KScriptManager.ArgStart(a, monMgrProxy!));
                if (!string.IsNullOrWhiteSpace(mod)) game.MornitoringConfig.CheckModuleList.Add(mod);
                return DynValue.Nil;
            });
        });
    }
}
