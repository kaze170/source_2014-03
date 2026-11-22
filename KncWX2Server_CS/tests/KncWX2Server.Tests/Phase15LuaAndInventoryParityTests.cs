using System.Buffers.Binary;
using System.Net.Sockets;
using KncWX2Server.CenterServer;
using KncWX2Server.ChannelServer;
using KncWX2Server.ChannelServer.Managers;
using KncWX2Server.Core.Events;
using KncWX2Server.Core.Framework;
using KncWX2Server.Core.Packets;
using KncWX2Server.Core.Region;
using KncWX2Server.Core.Security;
using KncWX2Server.Core.Serialization;
using KncWX2Server.GameServer;
using KncWX2Server.GlobalServer;
using KncWX2Server.LoginServer;
using Xunit;

namespace KncWX2Server.Tests;

public sealed class Phase15LuaAndInventoryParityTests : IDisposable
{
    private readonly string _dbPath;

    public Phase15LuaAndInventoryParityTests()
    {
        _dbPath = Path.Combine(Path.GetTempPath(), $"knc_p15_{Guid.NewGuid():N}.db");
    }

    public void Dispose()
    {
        try { if (File.Exists(_dbPath)) File.Delete(_dbPath); } catch { }
    }

    [Fact]
    public void MoonSharp_LoadsOriginalTwServerResourceLuaTables()
    {
        var router = new InProcessRouter();
        var center = new KCenterServer(RegionCode.TW, router);
        var global = new KGlobalServer(RegionCode.TW, router);
        var login = new KLoginServer(RegionCode.TW, _dbPath, router);
        var game = new KGameServer(RegionCode.TW, _dbPath, router, center, global);

        int loginLoaded = login.LoadServerResources();
        int gameLoaded = game.LoadServerResources(includeHeavyTables: false);
        var channel = new KChannelServer(RegionCode.TW, _dbPath, router);
        int channelLoaded = channel.LoadServerResources();

        Assert.True(loginLoaded >= 1);
        Assert.True(gameLoaded >= 18);
        Assert.True(channelLoaded >= 1);

        // Verify EDInventoryExpand.lua values
        Assert.Equal(5_000_000, game.EdInventoryExpand.GetNextInventoryUpgradeEd(1));
        Assert.Equal(75_000_000, game.EdInventoryExpand.GetNextInventoryUpgradeEd(4));
        var bankUpgrade1 = game.EdInventoryExpand.GetBankUpgradeForCurrentSlots(8);
        Assert.True(bankUpgrade1.canUpgrade);
        Assert.Equal(16, bankUpgrade1.nextSlotCount);
        Assert.Equal(4_000_000, bankUpgrade1.costEd);

        // Verify SynthesisTable.lua values with corrected EQIP_POSITION (Weapon=2 -> group 2000, Body=8 -> 1000)
        Assert.Equal(2000, game.SynthesisManager.GetAvatarSocketOptionGroupId(KSynthesisManager.EquipWeaponHand));
        Assert.Equal(1000, game.SynthesisManager.GetAvatarSocketOptionGroupId(KSynthesisManager.EquipDefenceBody));
        Assert.Equal(10, KSynthesisManager.ComputeGroupId(1, KSynthesisManager.EquipWeaponHand));
        Assert.Equal(11, KSynthesisManager.ComputeGroupId(1, KSynthesisManager.EquipDefenceBody));
        Assert.Equal(15, KSynthesisManager.ComputeGroupId(1, KSynthesisManager.EquipDefenceHair));

        // Verify EnchantTable.lua (stones + attach scrolls), AttribEnchantTable.lua, GameSysValTable.lua
        Assert.Equal(109950, game.EnchantItemManager.WeaponEnchantStoneId);
        Assert.Equal(152000121, game.EnchantItemManager.RareWeaponEnchantStoneId);
        Assert.True(game.EnchantItemManager.TryGetAttachItemInfo(130152, out var scrollLv10));
        Assert.Equal(10, scrollLv10.EnchantLevel);
        Assert.Equal(70, game.GameSysValTable.LimitsMaxLevel);
        int shard = game.SocketItemManager.RollIdentifyResult(KSocketItemManager.AtiUnknown);
        Assert.InRange(shard, 130048, 130053);

        // Verify RecommendUserTable.lua, AbuserList.lua, AttribNpcTable.lua, PvpMatchData.lua, AutoPartyData.lua, WaitQueueData.lua
        Assert.Equal(10515, game.RecommendUserTable.RecommendEventReward);
        Assert.Equal(10498, game.RecommendUserTable.GetRecommendUserEventReward(30));
        Assert.Equal(5000, KUserAbuserManager.GlobalGetItemCountMin);
        Assert.Equal(10000, KUserAbuserManager.GlobalGetItemCountMax);
        Assert.Contains("112.175.245.156", KUserAbuserManager.GlobalJoinAccountCheckIps);
        Assert.True(center.AttribNpcTable.LoadedNpcRuleCount > 0);
        Assert.Equal(5, center.AttribNpcTable.AttribSubStageCounts[(1, 2)]);
        Assert.Equal(200_000, global.MatchMakingManager.MaxRankingPoint);
        Assert.True(global.MatchMakingManager.PvpMapsForMatch.Count >= 5);
        Assert.True(global.AutoPartyManager.AutoPartyDungeonRanges.Count > 0);
        Assert.Equal(WaitQueueMode.WQ_NONE, channel.WaitQueueManager.Mode);
        Assert.Equal(100_000, channel.WaitQueueManager.GetTotalCCULimit(0));
        Assert.Equal(70, channel.GameSysVal.LimitsMaxLevel);

        // Verify AttribAttachTable.lua, MiracleCubeTable.lua, ItemRandomSocket.lua, ValenTineData.lua
        Assert.True(game.SocketItemManager.TryGetAttribAttachInfo(133371, out var blazeWindScroll));
        Assert.Equal(KSocketItemManager.EnchantElement.Blaze, blazeWindScroll.Slot0);
        Assert.Equal(KSocketItemManager.EnchantElement.Wind, blazeWindScroll.Slot1);
        Assert.True(game.SocketItemManager.TryGetMiracleCubeInfo(270848, out var cubeLv1));
        Assert.Equal(1, cubeLv1.MinLevel);
        Assert.Equal(20, cubeLv1.MaxLevel);
        Assert.True(game.SocketItemManager.RollRandomSocketOption(1003) > 0);
        Assert.Equal(141000972, game.GameEventManager.ValentineRewardItemId);

        // Verify CenterServer Lua scripts (HenirResultTable/HenirData/NpcExpTable/CnHackingCheckData/PartyServerData/BattleFieldServerData/SecretDungeonData)
        Assert.True(center.HenirResultTable.IsResurrectionStage(9));
        Assert.True(center.HenirResultTable.IsClearTempInventoryStage(10));
        Assert.True(center.HenirResultTable.IsClearNotifyStage(31));
        Assert.Equal(9, center.DropTable.GetNpcExp(1));
        Assert.False(center.HackingCheckManager.IsDungeonResultCheckEnable);
        Assert.Equal(4800, KCnPartyFeverManager.ConfiguredActivateFeverPoint);
        Assert.Equal(4800, KBattleFieldGameManager.DefaultDangerousValueMax);
        Assert.Equal(2, center.SecretDungeonExtraStageNpcLevel);
        Assert.True(center.SecretDungeonExtraStageConditions.Count > 0);

        // Verify newly wired TW Lua scripts across Game/Center/Global/Channel/Login servers
        Assert.True(game.ClassChangeTable.AreInSameClassGroup(10, 11));
        Assert.True(game.ExpandPeriodItems.Count > 0);
        Assert.True(game.DeleteItemIds.Count > 0);
        Assert.True(game.DailyGiftBoxPackages.Count > 0);
        Assert.True(game.SubTitleOpenMissions.Count > 0);
        Assert.True(game.GameEventManager.EventMonsterInfos.Count > 0);
        Assert.True(channel.SmsPhoneNumberManager.GetPhoneNumberList(1).Count > 0);
        Assert.True(global.WorldMissionManager.OpenTimes.Count > 0);
        Assert.Equal(28, global.FieldBossManager.ConfiguredTotalDangerousValue);
        Assert.True(global.MatchMakingManager.PvpNpcNames.Count > 0);
        Assert.True(global.MatchMakingManager.PvpNpcInfos.Count > 0);
        Assert.True(center.DefenceDungeonManager.NotBeginDefenceDungeonIds.Count > 0);
        Assert.True(center.HenirResultTable.GetHenirChallangeRewards(0, 0).Count > 0);
        Assert.True(center.DropTable.PaymentDungeons.Count > 0);
        Assert.True(KBattleFieldGameManager.MiddleBossDropInfos.Count > 0);
        Assert.True(login.RankingManager.HenirRankingConfigs.Count > 0);

        var fever = new KCnPartyFeverManager();
        Assert.Equal((short)4000, fever.GetMaxFeverPointByNumMember(4500, 2));
        var (ufp, _) = fever.UpdateFeverPoint(4800, 2);
        Assert.Equal(KCnPartyFeverManager.UpdateFeverPointResult.UfpActiveFever, ufp);
    }

    [Fact]
    public async Task InventoryParity_RandomItemKeys_Identify_ExpandSteps_ManufactureMaterials_Enchant_Socket_Synthesis()
    {
        var router = new InProcessRouter();
        var center = new KCenterServer(RegionCode.TW, router);
        var global = new KGlobalServer(RegionCode.TW, router);
        var game = new KGameServer(RegionCode.TW, _dbPath, router, center, global);

        game.EdInventoryExpand.SetInventoryGradeEd(1, 50_000);
        game.EdInventoryExpand.SetInventoryGradeEd(2, 120_000);
        game.EdInventoryExpand.SetBankGradeEd(16, 40_000);
        game.EdInventoryExpand.SetBankGradeEd(24, 90_000);

        game.RandomItemManager.AddRandomItemGroup(9501, 778001, 100f);
        game.RandomItemManager.AddSealRandomItem(165900);
        game.RandomItemManager.RegisterRandomItem(new KRandomItemManager.RandomItemData
        {
            ItemId = 165900,
            KeyItemId = 69423,
            RequiredKeyCount = 1,
            RequiredEd = 10_000,
            UnitData = [new KRandomItemManager.RandomUnitData(0, 9501)],
        });

        game.ManufactureItemManager.AddManufactureResultGroupWithRate(9510, 778002, 100.0, isSealed: true);
        game.ManufactureItemManager.RegisterManufacture(new KManufactureItemManager.ManufactureData
        {
            ManufactureId = 95,
            ResultGroupId = 9510,
            Cost = 5_000,
            Materials = [new KManufactureItemManager.MaterialData(99825, 3)],
        });

        game.SynthesisManager.AddSynthesisGroup(10, 778003, 100f);

        Assert.True(center.Start(overridePort: 0));
        Assert.True(global.Start(overridePort: 0));
        Assert.True(game.Start(overridePort: 0, overrideTrPort: 0, overrideUdpEchoPort: 0, overrideUdpRelayPort: 0));

        try
        {
            using var gsClient = new TcpClient();
            await gsClient.ConnectAsync("127.0.0.1", game.ListenPort);
            using var gsStream = gsClient.GetStream();
            var gsSa = new KSADatabase();
            ushort gsSpi = await HandshakeAsync(gsStream, gsSa, RegionProfile.Taiwan);

            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_CONNECT_REQ, new KEGS_CONNECT_REQ { Version = "TW_2014_03" });
            Assert.Equal(EventId.EGS_CONNECT_ACK, (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).EventId);

            game.Db.VerifyOrRegisterAccount("TW", "p15user", "p15user", autoCreateOnMissing: true);
            game.Db.SetAccountAuthLevel(RegionProfile.Taiwan, "p15user", 3);
            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_VERIFY_ACCOUNT_REQ, new KUserAuthenticateReq { UserId = "p15user", Password = "p15user" });
            Assert.Equal(EventId.EGS_VERIFY_ACCOUNT_ACK, (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).EventId);
            Assert.Equal(EventId.ENX_USER_LOGIN_NOT, (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).EventId);
            Assert.Equal(EventId.EGS_SECOND_SECURITY_INFO_NOT, (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).EventId);

            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_CREATE_UNIT_REQ, new KEGS_CREATE_UNIT_REQ { NickName = "P15Hero", UnitClass = 1 });
            long unitUid = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan))
                .GetPayload<KEGS_CREATE_UNIT_ACK>(RegionProfile.Taiwan).UnitInfo.UnitUid;
            game.Db.UpdateUnitProgress(unitUid, 0, 500_000, 0);

            var lockedCube = game.Db.GrantUnitItem(unitUid, 165900, 1);
            var unknownEl = game.Db.GrantUnitItem(unitUid, KSocketItemManager.AtiUnknown, 1);
            var swordSlot = game.Db.GrantUnitItem(unitUid, 10010, 1);
            var stoneSlot = game.Db.GrantUnitItem(unitUid, 109945, 2);
            var syn1 = game.Db.GrantUnitItem(unitUid, 67002660, 1);
            var syn2 = game.Db.GrantUnitItem(unitUid, 67002670, 1);
            var syn3 = game.Db.GrantUnitItem(unitUid, 90001150, 1);
            var syn4 = game.Db.GrantUnitItem(unitUid, 90001160, 1);
            var synKey = game.Db.GrantUnitItem(unitUid, 160530, 1);

            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_SELECT_UNIT_REQ, new KEGS_SELECT_UNIT_REQ { UnitUid = unitUid });
            Assert.Equal(EventId.EGS_SELECT_UNIT_1_NOT, (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).EventId);
            Assert.Equal(EventId.EGS_SELECT_UNIT_2_NOT, (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).EventId);
            Assert.Equal(EventId.EGS_SELECT_UNIT_3_NOT, (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).EventId);
            Assert.Equal(EventId.EGS_SELECT_UNIT_4_NOT, (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).EventId);
            Assert.Equal(EventId.EGS_SELECT_UNIT_5_NOT, (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).EventId);
            Assert.Equal(EventId.EGS_SELECT_UNIT_ACK, (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).EventId);
            Assert.Equal(EventId.EGS_GET_MY_INVENTORY_ACK, (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).EventId);

            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_STATE_CHANGE_FIELD_REQ, new KEGS_STATE_CHANGE_FIELD_REQ { MapId = 20001 });
            Assert.Equal(EventId.EGS_STATE_CHANGE_FIELD_ACK, (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).EventId);
            Assert.Equal(EventId.EGS_CHANGE_USER_LIST_IN_SECTOR_NOT, (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).EventId);

            // 1. Random item requires key (69423) and ED
            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_OPEN_RANDOM_ITEM_REQ, new KEGS_OPEN_RANDOM_ITEM_REQ { ItemUid = lockedCube.ItemUid });
            var rndFail = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KEGS_OPEN_RANDOM_ITEM_ACK>(RegionProfile.Taiwan);
            Assert.Equal(NetError.ERR_RANDOM_ITEM_04, rndFail.Ok);

            game.Db.GrantUnitItem(unitUid, 69423, 1);
            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_OPEN_RANDOM_ITEM_REQ, new KEGS_OPEN_RANDOM_ITEM_REQ { ItemUid = lockedCube.ItemUid });
            var rndOk = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KEGS_OPEN_RANDOM_ITEM_ACK>(RegionProfile.Taiwan);
            Assert.Equal(NetError.NET_OK, rndOk.Ok);
            Assert.Equal(1, rndOk.InsertedItem[778001]);

            // 2. Identify consumes ATI_IDENTIFY_STONE (130054) and grants elemental shard (130048..130053)
            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_IDENTIFY_ITEM_REQ, new KEGS_IDENTIFY_ITEM_REQ { ItemUid = unknownEl.ItemUid, Quantity = 1 });
            var idFail = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KEGS_IDENTIFY_ITEM_ACK>(RegionProfile.Taiwan);
            Assert.Equal(NetError.ERR_ITEM_08, idFail.Ok);

            game.Db.GrantUnitItem(unitUid, KSocketItemManager.AtiIdentifyStone, 1);
            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_IDENTIFY_ITEM_REQ, new KEGS_IDENTIFY_ITEM_REQ { ItemUid = unknownEl.ItemUid, Quantity = 1 });
            var idOk = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KEGS_IDENTIFY_ITEM_ACK>(RegionProfile.Taiwan);
            Assert.Equal(NetError.NET_OK, idOk.Ok);
            Assert.InRange(idOk.InsertedItem.Keys.Single(), 130048, 130053);

            // 3. Stepwise ED Inventory & Bank expansion advances grade/slot count in SQLite
            await SendNoParamAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_EXPAND_INVENTORY_ED_REQ);
            var inv1 = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KEGS_EXPAND_BANK_SLOT_ED_ACK>(RegionProfile.Taiwan);
            Assert.Equal(NetError.NET_OK, inv1.Ok);

            await SendNoParamAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_GET_NEXT_INVENTORY_ED_REQ);
            var nextInv2 = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KEGS_GET_NEXT_BANK_ED_ACK>(RegionProfile.Taiwan);
            Assert.Equal(120_000, nextInv2.Ed);

            await SendNoParamAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_EXPAND_BANK_SLOT_ED_REQ);
            var bank1 = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KEGS_EXPAND_BANK_SLOT_ED_ACK>(RegionProfile.Taiwan);
            Assert.Equal(NetError.NET_OK, bank1.Ok);
            Assert.Equal(16, bank1.ExpandedCategorySlot[9]);

            await SendNoParamAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_GET_NEXT_BANK_ED_REQ);
            var nextBank2 = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KEGS_GET_NEXT_BANK_ED_ACK>(RegionProfile.Taiwan);
            Assert.Equal(90_000, nextBank2.Ed);

            // 4. Manufacture checks & consumes recipe materials
            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_ITEM_MANUFACTURE_REQ, new KEGS_ITEM_MANUFACTURE_REQ { ManufactureId = 95, Quantity = 1 });
            var mfgFail = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KEGS_ITEM_MANUFACTURE_ACK>(RegionProfile.Taiwan);
            Assert.Equal(NetError.ERR_MANUFACTURE_03, mfgFail.Ok);

            game.Db.GrantUnitItem(unitUid, 99825, 3);
            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_ITEM_MANUFACTURE_REQ, new KEGS_ITEM_MANUFACTURE_REQ { ManufactureId = 95, Quantity = 1 });
            var mfgOk = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KEGS_ITEM_MANUFACTURE_ACK>(RegionProfile.Taiwan);
            Assert.Equal(NetError.NET_OK, mfgOk.Ok);
            Assert.Equal(1, mfgOk.InsertedItem[778002]);

            // 5. Socket item consumes magic stones & writes socket IDs
            var sockReq = new KEGS_SOCKET_ITEM_REQ { ItemUid = swordSlot.ItemUid };
            sockReq.SocketInfo[0] = stoneSlot.ItemUid;
            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_SOCKET_ITEM_REQ, sockReq);
            var sockOk = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KEGS_SOCKET_ITEM_ACK>(RegionProfile.Taiwan);
            Assert.Equal(NetError.NET_OK, sockOk.Ok);
            Assert.True(sockOk.InventorySlotInfo.Last().ItemInfo.ItemSockets[0] > 0);

            // 6. Synthesis persists SocketNo on synthesized item
            var synReq = new KEGS_SYNTHESIS_ITEM_REQ { KeyData = synKey.ItemUid, SocketNo = 777 };
            synReq.SynthesisData[KSynthesisManager.EquipWeaponHand] = syn1.ItemUid;
            synReq.SynthesisData[KSynthesisManager.EquipDefenceBody] = syn2.ItemUid;
            synReq.SynthesisData[KSynthesisManager.EquipDefenceLeg] = syn3.ItemUid;
            synReq.SynthesisData[KSynthesisManager.EquipDefenceHand] = syn4.ItemUid;
            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_SYNTHESIS_ITEM_REQ, synReq);
            var synAck = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KEGS_SYNTHESIS_ITEM_ACK>(RegionProfile.Taiwan);
            Assert.Equal(NetError.NET_OK, synAck.Ok);
            Assert.Equal(777, synAck.InventorySlotInfo.Last().ItemInfo.ItemSockets[0]);

            // 7. Seal / Unseal + Enchant Attach Scroll + Attrib Attach + PShop Agency Pickup
            game.LoadServerResources(includeHeavyTables: false);
            var sealScroll = game.Db.GrantUnitItem(unitUid, 129900, 1);
            var enchantScroll = game.Db.GrantUnitItem(unitUid, 130152, 1); // +10 scroll from EnchantTable.lua
            var redShard = game.Db.GrantUnitItem(unitUid, KSocketItemManager.AtiRed, 1);

            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_SEAL_ITEM_REQ, new KEGS_SEAL_ITEM_REQ { DestItemUid = swordSlot.ItemUid, SealItemUid = sealScroll.ItemUid });
            var sealAck = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KEGS_SEAL_ITEM_ACK>(RegionProfile.Taiwan);
            Assert.Equal(NetError.NET_OK, sealAck.Ok);
            Assert.True(game.Db.GetItemFullDetails(unitUid, swordSlot.ItemUid).isSealed);

            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_UNSEAL_ITEM_REQ, new KEGS_UNSEAL_ITEM_REQ { DestItemUid = swordSlot.ItemUid });
            var unsealAck = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KEGS_SEAL_ITEM_ACK>(RegionProfile.Taiwan);
            Assert.Equal(NetError.NET_OK, unsealAck.Ok);
            Assert.False(game.Db.GetItemFullDetails(unitUid, swordSlot.ItemUid).isSealed);

            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_ENCHANT_ATTACH_ITEM_REQ, new KEGS_ENCHANT_ATTACH_ITEM_REQ { DestItemUid = swordSlot.ItemUid, AttachItemUid = enchantScroll.ItemUid });
            var encAttachAck = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KEGS_ENCHANT_ATTACH_ITEM_ACK>(RegionProfile.Taiwan);
            Assert.Equal(NetError.NET_OK, encAttachAck.Ok);
            Assert.Equal(10, encAttachAck.InventorySlotInfo[0].ItemInfo.EnchantLevel);

            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_ATTRIB_ATTACH_ITEM_REQ, new KEGS_ATTRIB_ATTACH_ITEM_REQ { DestItemUid = swordSlot.ItemUid, AttachItemUid = redShard.ItemUid });
            var attrAttachAck = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KEGS_ATTRIB_ATTACH_ITEM_ACK>(RegionProfile.Taiwan);
            Assert.Equal(NetError.NET_OK, attrAttachAck.Ok);
            Assert.Equal((sbyte)KSocketItemManager.EnchantElement.Blaze, attrAttachAck.InventorySlotInfo[0].ItemInfo.AttribEnchantInfo.AttribEnchant0);

            var dualScroll = game.Db.GrantUnitItem(unitUid, 133371, 1); // Blaze + Wind scroll from AttribAttachTable.lua
            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_ATTRIB_ATTACH_ITEM_REQ, new KEGS_ATTRIB_ATTACH_ITEM_REQ { DestItemUid = swordSlot.ItemUid, AttachItemUid = dualScroll.ItemUid });
            var dualAttrAck = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KEGS_ATTRIB_ATTACH_ITEM_ACK>(RegionProfile.Taiwan);
            Assert.Equal(NetError.NET_OK, dualAttrAck.Ok);
            Assert.Equal((sbyte)KSocketItemManager.EnchantElement.Blaze, dualAttrAck.InventorySlotInfo[0].ItemInfo.AttribEnchantInfo.AttribEnchant0);
            Assert.Equal((sbyte)KSocketItemManager.EnchantElement.Wind, dualAttrAck.InventorySlotInfo[0].ItemInfo.AttribEnchantInfo.AttribEnchant1);

            long listedUid = game.Db.CreatePersonalShopListing(RegionProfile.Taiwan, unitUid, "P15Hero", 10025, 2, 1000);
            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_PICK_UP_FROM_PSHOP_AGENCY_REQ, new KEGS_PICK_UP_FROM_PSHOP_AGENCY_REQ { PickUpItemList = [listedUid] });
            var pickupAck = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KEGS_PICK_UP_FROM_PSHOP_AGENCY_ACK>(RegionProfile.Taiwan);
            Assert.Equal(NetError.NET_OK, pickupAck.Ok);
            Assert.Single(pickupAck.InventorySlotInfo);
            Assert.Equal(10025, pickupAck.InventorySlotInfo[0].ItemInfo.ItemId);

            // 8. Retaining user reward + Henir ranking deletion + Valentine cheat parity
            var gsUser = game.GetUserByUnitUid(unitUid);
            Assert.NotNull(gsUser);
            gsUser!.IsOutUserRetaining = true;
            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_OUT_USER_RETAINING_REQ, new KEGS_OUT_USER_RETAINING_REQ());
            var retainInfo = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KEGS_OUT_USER_RETAINING_ACK>(RegionProfile.Taiwan);
            Assert.True(retainInfo.DepartureUser);

            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_RETAINING_SELECT_REWARD_REQ, new KEGS_RETAINING_SELECT_REWARD_REQ { SlotId = 0 });
            var retainSel = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KEGS_RETAINING_SELECT_REWARD_ACK>(RegionProfile.Taiwan);
            Assert.Equal(NetError.NET_OK, retainSel.Ok);
            Assert.Equal(130048, retainSel.RewardItem.ItemId);

            var heMgr = new KHenirRankingManager();
            heMgr.CheckNewRecord(new KHenirRankingManager.HenirRecord { UnitUid = unitUid, NickName = "P15Hero", StageCount = 50, ClearTimeSeconds = 180 });
            Assert.True(heMgr.DeleteRecordByNickName("P15Hero"));
            Assert.Empty(heMgr.GetRankings());

            // 9. ItemConvert + ItemEvaluate + RestoreItemEvaluate + KickGuildMember parity
            game.ItemConvertTable[778001] = (1, 500, [778002]);
            var convSrc = game.Db.GrantUnitItem(unitUid, 778001, 1);
            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_ITEM_CONVERT_REQ, new KEGS_ITEM_CONVERT_REQ { ItemUid = convSrc.ItemUid, Quantity = 1 });
            var convAck = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KEGS_ITEM_CONVERT_ACK>(RegionProfile.Taiwan);
            Assert.Equal(NetError.NET_OK, convAck.Ok);
            Assert.Equal(778002, convAck.InventorySlotInfo[1].ItemInfo.ItemId);

            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_ITEM_EVALUATE_REQ, new KEGS_ITEM_EVALUATE_REQ { ItemUid = swordSlot.ItemUid });
            var evalAck = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KEGS_ITEM_EVALUATE_ACK>(RegionProfile.Taiwan);
            Assert.Equal(NetError.NET_OK, evalAck.Ok);

            var miracleCube = game.Db.GrantUnitItem(unitUid, 270848, 1);
            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_RESTORE_ITEM_EVALUATE_REQ, new KEGS_RESTORE_ITEM_EVALUATE_REQ { SupportItemUid = miracleCube.ItemUid, TargetItemUid = swordSlot.ItemUid });
            var restoreEvalAck = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KEGS_RESTORE_ITEM_EVALUATE_ACK>(RegionProfile.Taiwan);
            Assert.Equal(NetError.NET_OK, restoreEvalAck.Ok);

            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_KICK_GUILD_MEMBER_REQ, new KEGS_KICK_GUILD_MEMBER_REQ { UnitUid = unitUid + 1 });
            var kickFail = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KEGS_KICK_GUILD_MEMBER_ACK>(RegionProfile.Taiwan);
            Assert.Equal(NetError.ERR_GUILD_19, kickFail.Ok);

            // 10. Cobo cheat NextDay reset & Party opinion-check cancel
            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_EVENT_COBO_ITEM_GIVE_CHEAT_NOT, new KEGS_EVENT_COBO_ITEM_GIVE_CHEAT_NOT { CoboEventItemGet = true, NextDay = true });
            var coboNot = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KEGS_EVENT_COBO_DUNGEON_FIELD_ACK>(RegionProfile.Taiwan);
            Assert.Equal(NetError.NET_OK, coboNot.Ok);
            Assert.True(coboNot.StartUi);
            Assert.Equal(-1, coboNot.RemaindTime);

            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_CREATE_PARTY_REQ, new KEGS_CREATE_PARTY_REQ
                {
                    PartyInfo = new KPartyInfo { PartyType = KPartyInfo.PtDungeonParty, PartyName = "OpinionCheckParty" },
                });
            var createPartyAck = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KEGS_CREATE_PARTY_ACK>(RegionProfile.Taiwan);
            Assert.Equal(NetError.NET_OK, createPartyAck.Ok);

            await SendPacketAsync(gsStream, gsSa, gsSpi, RegionProfile.Taiwan, PerformerId.PI_GS_USER,
                EventId.EGS_PARTY_GAME_START_OPINION_CHECK_REPLY_NOT, new KEGS_PARTY_GAME_START_OPINION_CHECK_REPLY_NOT { Accept = false, Reason = 0 });
            var partyFailNot = (await ReadEventAsync(gsStream, gsSa, RegionProfile.Taiwan)).GetPayload<KEGS_PARTY_GAME_START_FAILED_NOT>(RegionProfile.Taiwan);
            Assert.Equal(NetError.NOT_PARTY_GAME_START_FAILED_00, partyFailNot.Reason);
            Assert.Equal(unitUid, partyFailNot.FailUnitUid);
        }
        finally
        {
            game.ShutDown();
            global.ShutDown();
            center.ShutDown();
        }
    }

    [Fact]
    public void RemainingTwLuaTablesAndStrictWirePacketsLoadAndRoundTrip()
    {
        var dbPath = Path.Combine(Path.GetTempPath(), $"p15_lua_{Guid.NewGuid():N}.db");
        var router = new InProcessRouter();
        var center = new KCenterServer(RegionCode.TW, router);
        var global = new KGlobalServer(RegionCode.TW, router);
        var game = new KGameServer(RegionCode.TW, dbPath, router, center, global);
        var channel = new KChannelServer(RegionCode.TW, dbPath, router);

        try
        {
            Assert.True(game.LoadServerResources(includeHeavyTables: false) >= 50);
            Assert.NotEmpty(game.BingoBanSpots);
            Assert.NotEmpty(game.BingoPresentInfos);
            Assert.NotEmpty(game.CashItemPrices);
            Assert.NotEmpty(game.CashItemServerGroupBlocked);
            Assert.NotEmpty(game.GameEventManager.EventRewards);
            Assert.NotEmpty(game.GameEventManager.NotifyMsgServerInfos);
            Assert.NotEmpty(game.FieldManager.FieldTemplates);
            Assert.NotEmpty(game.MapDatas);
            Assert.NotEmpty(game.LocalMapInfos);
            Assert.NotEmpty(game.RoomListTemplates);
            Assert.NotEmpty(game.KeepShowItems);
            Assert.NotEmpty(game.GetFilteredBillingProducts(game.ServerGroupId));

            Assert.Equal(5, channel.LoadServerResources());
            Assert.NotEmpty(channel.ChannelTemplates);
            Assert.NotEmpty(channel.GetChannelListSnapshot());
            Assert.True(channel.UdpRelayCheckerOn);
            Assert.Equal(26424, channel.UdpRelayCheckerPort);

            Assert.False(RegionProfile.Taiwan.KeyMappingInt);
            Assert.False(RegionProfile.Taiwan.DllListCheckBeforeLoading);
            Assert.False(RegionProfile.Taiwan.SerialNumberCheckInGameServer);
            Assert.False(RegionProfile.Taiwan.LocalRankingSystem);
            Assert.True(RegionProfile.China.KeyMappingInt);
            Assert.True(RegionProfile.Japan.KeyMappingInt);

            static T Rt<T>(T pkt, RegionProfile region) where T : IKSerializable, new()
            {
                var buf = new KSerBuffer();
                var ks = new KSerializer(region);
                ks.BeginWriting(buf);
                Assert.True(pkt.Put(ks, region));
                ks.EndWriting();
                var outPkt = new T();
                ks.BeginReading(buf);
                Assert.True(outPkt.Get(ks, region));
                ks.EndReading();
                return outPkt;
            }

            var authTw = Rt(new KUserAuthenticateReq
            {
                DebugAuth = true,
                UserId = "tw01",
                Password = "pw",
                ChannelRandomKey = 999,
                ChannelingCode = 1,
                ManualLogin = true,
            }, RegionProfile.Taiwan);
            Assert.Equal("tw01", authTw.UserId);
            Assert.Equal(0, authTw.ChannelRandomKey);

            var connTw = Rt(new KConnectionUnitInfo
            {
                UnitUid = 777,
                CharName = "TWChar",
                Level = 60,
                GuildName = "TWGuild",
            }, RegionProfile.Taiwan);
            Assert.Equal(777, connTw.UnitUid);
            Assert.Equal("TWGuild", connTw.GuildName);

            var cashInv = new KUserCashInventory();
            cashInv.EnsureBingoInitialized(game.BingoBanSpots, game.BingoPresentInfos);
            Assert.Equal(KUserCashInventory.BingoBoardSlotNum, cashInv.BingoBoard.Length);
            Assert.Equal(KUserCashInventory.BingoPresentNum, cashInv.BingoPresents.Count);
            Assert.True(cashInv.BingoBoard[0] >= KUserCashInventory.BingoNumMax);
            Assert.True(cashInv.BingoBoard[4] >= KUserCashInventory.BingoNumMax);
            Assert.True(cashInv.BingoBoard[20] >= KUserCashInventory.BingoNumMax);
            Assert.True(cashInv.BingoBoard[24] >= KUserCashInventory.BingoNumMax);
            Assert.Equal(NetError.NET_OK, cashInv.BingoLottery().ok);
            Assert.Equal(NetError.NET_OK, cashInv.BingoMix().ok);
            Assert.Equal(NetError.NET_OK, cashInv.BingoReset(isInit: false, game.BingoBanSpots, game.BingoPresentInfos).ok);

            var kbInput = new KKeyboardMappingInfo
            {
                KeyboardMappingInfo = new Dictionary<short, short> { [1] = 65 },
                GamePadMappingInfo = new Dictionary<short, short> { [2] = 99 },
            };
            var kbTw = Rt(kbInput, RegionProfile.Taiwan);
            Assert.Single(kbTw.KeyboardMappingInfo);
            Assert.Empty(kbTw.GamePadMappingInfo);
            var kbCn = Rt(kbInput, RegionProfile.China);
            Assert.Single(kbCn.GamePadMappingInfo);

            var pFail = Rt(new KEGS_PARTY_GAME_START_FAILED_NOT { Reason = NetError.NOT_PARTY_GAME_START_FAILED_00, FailUnitUid = 123 }, RegionProfile.Taiwan);
            Assert.Equal(NetError.NOT_PARTY_GAME_START_FAILED_00, pFail.Reason);
            Assert.Equal(123, pFail.FailUnitUid);

            var selInv = Rt(new KEGS_SELECT_UNIT_INVENTORY_INFO_NOT { SlotCategory = 1 }, RegionProfile.Taiwan);
            Assert.Equal(1, selInv.SlotCategory);

            var updUnit = Rt(new KEGS_UPDATE_UNIT_INFO_NOT { GameEnd = true, UnitInfo = new KUnitInfo { UnitUid = 99 } }, RegionProfile.Taiwan);
            Assert.True(updUnit.GameEnd);
            Assert.Equal(99, updUnit.UnitInfo.UnitUid);
        }
        finally
        {
            if (File.Exists(dbPath)) File.Delete(dbPath);
        }
    }

    private static async Task<ushort> HandshakeAsync(NetworkStream stream, KSADatabase saDb, RegionProfile region)
    {
        var acceptEv = await ReadEventAsync(stream, saDb, region);
        Assert.Equal(EventId.E_ACCEPT_CONNECTION_NOT, acceptEv.EventId);
        var ks = new KSerializer(region);
        ks.BeginReading(acceptEv.Buffer);
        Assert.True(ks.Get(out ushort newSpi));
        var newSa = new KSecurityAssociation();
        Assert.True(newSa.Get(ks, region));
        ks.EndReading();
        saDb.Insert(newSpi, newSa);
        return newSpi;
    }

    private static async Task SendPacketAsync(NetworkStream stream, KSADatabase saDb, ushort spi, RegionProfile region, uint destPi, EventId id, IKSerializable packet)
    {
        var ev = new KEvent();
        ev.DestPerformer.PerformerId = destPi;
        ev.EventId = id;
        var ks = new KSerializer(region);
        ks.BeginWriting(ev.Buffer);
        packet.Put(ks, region);
        ks.EndWriting();
        var wire = new KSerBuffer();
        ks.BeginWriting(wire);
        ev.Put(ks, region);
        ks.EndWriting();
        var sec = new KSecureBuffer(saDb, spi);
        Assert.True(sec.Create(wire.AsSpan()));
        ushort totalLen = (ushort)(sec.WireBytes.Length + 2);
        byte[] frame = new byte[totalLen];
        BinaryPrimitives.WriteUInt16LittleEndian(frame.AsSpan(0, 2), totalLen);
        sec.WireBytes.CopyTo(frame.AsSpan(2));
        await stream.WriteAsync(frame);
    }

    private static async Task SendNoParamAsync(NetworkStream stream, KSADatabase saDb, ushort spi, RegionProfile region, uint destPi, EventId id)
    {
        var ev = new KEvent();
        ev.DestPerformer.PerformerId = destPi;
        ev.EventId = id;
        var ks = new KSerializer(region);
        ks.BeginWriting(ev.Buffer);
        ks.Put((sbyte)0);
        ks.EndWriting();
        var wire = new KSerBuffer();
        ks.BeginWriting(wire);
        ev.Put(ks, region);
        ks.EndWriting();
        var sec = new KSecureBuffer(saDb, spi);
        Assert.True(sec.Create(wire.AsSpan()));
        ushort totalLen = (ushort)(sec.WireBytes.Length + 2);
        byte[] frame = new byte[totalLen];
        BinaryPrimitives.WriteUInt16LittleEndian(frame.AsSpan(0, 2), totalLen);
        sec.WireBytes.CopyTo(frame.AsSpan(2));
        await stream.WriteAsync(frame);
    }

    private static async Task<KEvent> ReadEventAsync(NetworkStream stream, KSADatabase saDb, RegionProfile region)
    {
        byte[] lenBuf = new byte[2];
        int o = 0;
        while (o < 2)
        {
            int n = await stream.ReadAsync(lenBuf.AsMemory(o, 2 - o));
            if (n == 0) throw new EndOfStreamException();
            o += n;
        }
        ushort totalLen = BinaryPrimitives.ReadUInt16LittleEndian(lenBuf);
        byte[] payload = new byte[totalLen - 2];
        o = 0;
        while (o < payload.Length)
        {
            int n = await stream.ReadAsync(payload.AsMemory(o, payload.Length - o));
            if (n == 0) throw new EndOfStreamException();
            o += n;
        }
        var sec = new KSecureBuffer(saDb, payload);
        Assert.True(sec.TryGetPayload(out byte[] plain));
        var ev = new KEvent();
        var ks = new KSerializer(region);
        ks.BeginReading(new KSerBuffer(plain));
        Assert.True(ev.Get(ks, region));
        ks.EndReading();
        ev.Buffer.UnCompress();
        return ev;
    }
}
