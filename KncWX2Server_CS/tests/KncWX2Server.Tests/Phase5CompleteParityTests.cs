using KncWX2Server.CenterServer;
using KncWX2Server.Core.Database;
using KncWX2Server.Core.Events;
using KncWX2Server.Core.Framework;
using KncWX2Server.Core.Packets;
using KncWX2Server.Core.Region;
using KncWX2Server.Core.Serialization;
using KncWX2Server.GameServer;
using KncWX2Server.GlobalServer;
using KncWX2Server.LoginServer;
using Xunit;

namespace KncWX2Server.Tests;

public sealed class Phase5CompleteParityTests
{
    [Fact]
    public void GuildSkillTree_And_GuildBoardManager_MatchCppRules()
    {
        var tree = new KGuildSkillTree { GuildSPoint = 10, GuildCsPoint = 2, MaxGuildCsPoint = 5 };
        tree.SetSkillLevelAndCsp(9001, 7, 2); // clamped to master level 5
        Assert.Equal(5, tree.GetSkillLevel(9001));
        Assert.True(tree.IsMasterSkillLevel(9001));
        Assert.True(tree.IsAllPrecedingSkillMasterLevel(9002));
        Assert.True(tree.IsTierOpened(9002));

        var (spUsed, cspUsed) = tree.CalcUsedSPointAndCsPoint();
        Assert.Equal(3, spUsed);
        Assert.Equal(2, cspUsed);

        var (_, retrieved, modified) = tree.CalcAndExpireCashSkillPoint();
        Assert.Equal(2, retrieved);
        Assert.Single(modified);
        Assert.Equal(3, tree.GetSkillLevel(9001));

        var board = new KGuildBoardManager();
        for (int i = 1; i <= 8; i++)
        {
            board.NewGuildAd(new KGuildBoardManager.GuildAdEntry
            {
                GuildUid = i,
                GuildName = $"Guild{i}",
                GuildLevel = (byte)i,
                CurMemberCount = (ushort)(i * 3),
            });
        }
        var (page0, totalPages) = board.GetGuildAdList(KGuildBoardManager.AdSortType.GuildLevel, 0);
        Assert.Equal(2, totalPages);
        Assert.Equal(KGuildBoardManager.PagePerCount, page0.Count);
        Assert.Equal(8, page0[0].GuildLevel);

        Assert.True(board.ApplyJoinGuild(new KGuildBoardManager.ApplyJoinEntry { GuildUid = 8, UnitUid = 99, NickName = "Hero" }, out _));
        Assert.Single(board.GetApplyJoinGuildList(8));
    }

    [Fact]
    public void DefenceDungeon_HackingCheck_And_FieldBoss_MatchCppRules()
    {
        var def = new KDefenceDungeonManager { IsOpenDefenceDungeon = true };
        var partyLevels = new Dictionary<long, int> { [1] = 40, [2] = 42 };
        Assert.False(def.DecisionEnterDefenceDungeon(30000, 40, partyLevels, 1.0f, true)); // excluded dungeon
        Assert.True(def.DecisionEnterDefenceDungeon(31000, 40, partyLevels, 1.0f, is100PercentEventOn: true));
        Assert.Equal(KDefenceDungeonManager.DdpPhase1, def.DefenceDungeonPhase);
        Assert.True(def.IncreaseDefenceDungeonPhase(90001));
        Assert.Equal(KDefenceDungeonManager.DdpPhase2, def.DefenceDungeonPhase);

        var hack = new KCnHackingCheckManager();
        hack.SetHenirStageMinimumClearTime(5, 10);
        Assert.True(hack.CheckDungeonClearTimeAutoHackUser(0, 101, 31000, 0, 1, 50, true, 1, 4));
        Assert.False(hack.CheckDungeonClearTimeAutoHackUser(2, 101, 31000, 0, 1, 50, true, 1, 4)); // GM bypass
        Assert.True(hack.CheckHenirClearTimeHackUser(0, 101, 39000, 5, 3));

        var boss = new KFieldBossManager();
        boss.UpdateTotalDangerousValue(20001, 200);
        boss.UpdateTotalDangerousValue(20001, 100);
        Assert.Equal(byte.MaxValue, boss.GetTotalDangerousValue(20001));
        boss.OpenPortal(20001, 20099, 1700000000);
        Assert.True(boss.IsPortalOpen);
        boss.ClosePortal();
        Assert.False(boss.IsPortalOpen);
        Assert.True(boss.CheckTerm(1700000060));
    }

    [Fact]
    public void Socket_Resolve_Trade_Pets_Relationship_And_GameEvent_WorkEndToEnd()
    {
        string dbPath = Path.Combine(Path.GetTempPath(), $"knc_phase5_{Guid.NewGuid():N}.db");
        try
        {
            var region = RegionProfile.FromCode(RegionCode.TW);
            var db = new SqliteDatabaseManager(dbPath);
            db.InitializeAndSeed();

            var a1 = db.VerifyOrRegisterAccount(region.Code, "p5_a1", "pw", autoCreateOnMissing: true);
            var a2 = db.VerifyOrRegisterAccount(region.Code, "p5_a2", "pw", autoCreateOnMissing: true);
            var (ok1, u1, _) = db.CreateUnit(region, a1.UserUid, "SellerUnit", 1);
            var (ok2, u2, _) = db.CreateUnit(region, a2.UserUid, "BuyerUnit", 2);
            Assert.True(ok1 && ok2 && u1 != null && u2 != null);

            db.UpdateUnitProgress(u1!.UnitUid, 0, 1000, 0);
            db.UpdateUnitProgress(u2!.UnitUid, 0, 5000, 0);

            var sword = db.GrantUnitItem(u1.UnitUid, 110001, 1);

            // Socket & Attrib
            var sockMgr = new KSocketItemManager();
            int opt = sockMgr.RollSocketOption(KSocketItemManager.SocketDataType.SdtWeaponNormal, fixedUnitRoll: 0.1);
            Assert.True(opt >= 10000);
            var socked = db.UpdateItemSockets(u1.UnitUid, sword.ItemUid, [opt]);
            Assert.Single(socked.ItemInfo.ItemSockets);

            sbyte attr = sockMgr.RollOrResolveAttribute(KSocketItemManager.AtiUnknown, 0, fixedUnitRoll: 0.05);
            Assert.Equal((sbyte)KSocketItemManager.EnchantElement.Blaze, attr);
            var attribed = db.UpdateItemAttrib(u1.UnitUid, sword.ItemUid, 0, attr);
            Assert.Equal(attr, attribed.ItemInfo.AttribEnchantInfo.AttribEnchant0);

            // Atomic Player Trade: Seller trades sword for Buyer's 1500 ED
            var tradeMgr = new KTradeManager();
            var session = tradeMgr.CreateInvite(u1.UnitUid, u1.NickName, u2.UnitUid, u2.NickName);
            Assert.NotNull(session);
            Assert.True(tradeMgr.AcceptInvite(u2.UnitUid, true));
            Assert.True(tradeMgr.UpdateItem(u1.UnitUid, sword.ItemUid, 1, false));
            Assert.True(tradeMgr.UpdateEd(u2.UnitUid, 1500));
            Assert.False(tradeMgr.Confirm(u1.UnitUid).readyToExecute);
            Assert.True(tradeMgr.Confirm(u2.UnitUid).readyToExecute);

            var tradeRes = db.ExecutePlayerTrade(
                session!.Sender.UnitUid, session.Sender.OfferEd, session.Sender.OfferItems,
                session.Receiver.UnitUid, session.Receiver.OfferEd, session.Receiver.OfferItems);
            tradeMgr.CompleteSession(session, tradeRes.ok);
            Assert.True(tradeRes.ok);
            Assert.Equal(2500, tradeRes.unitAEd);
            Assert.Equal(3500, tradeRes.unitBEd);
            Assert.Single(tradeRes.unitBItems);

            // Resolve received item
            var resolveMgr = new KResolveItemManager();
            var (outputs, jackpot) = resolveMgr.ResolveItem(30, isWeapon: true, deterministicRoll: 0.0);
            Assert.True(jackpot);
            var (resolvedOk, slots) = db.ResolveInventoryItem(u2.UnitUid, tradeRes.unitBItems[0].ItemUid, outputs);
            Assert.True(resolvedOk);
            Assert.True(slots.Count >= 3);

            // Pets & Riding Pets
            var petRow = db.CreatePet(u1.UnitUid, 50001, "Phoru");
            var petMgr = new KUserPetManager();
            petMgr.Init(db.GetPets(u1.UnitUid));
            Assert.True(petMgr.SummonOrUnsummon(petRow.PetUid).ok);
            Assert.True(petMgr.FeedSummonedPet(100, 50).ok);
            Assert.True(petMgr.EvolvePet(petRow.PetUid).ok);

            var rpetRow = db.CreateRidingPet(u1.UnitUid, 60001);
            var rpetMgr = new KUserRidingPetManager();
            Assert.Equal(KUserRidingPetManager.UserRidingPetManagerResult.RidingPetSucceed,
                rpetMgr.Init(db.GetRidingPets(u1.UnitUid)));
            Assert.Equal(KUserRidingPetManager.UserRidingPetManagerResult.RidingPetSucceed,
                rpetMgr.SummonRidingPet(rpetRow.RidingPetUid).res);
            Assert.Equal(KUserRidingPetManager.UserRidingPetManagerResult.RidingPetSucceed,
                rpetMgr.UnSummonRidingPet(rpetRow.RidingPetUid, 82.5f));

            // Relationship sibling restriction (Elsword <-> Elesis) & same-gender guard (Elsword <-> Raven)
            Assert.Equal(NetError.ERR_RELATIONSHIP_47,
                KUserRelationshipManager.CheckChar(100, 120)); // Lord Knight <-> Grand Master
            Assert.Equal(NetError.ERR_RELATIONSHIP_05,
                KUserRelationshipManager.CheckChar(100, 106)); // Lord Knight <-> Blade Master
            Assert.Equal(NetError.NET_OK,
                KUserRelationshipManager.CheckChar(100, 104)); // Lord Knight <-> Elemental Master

            // GameEventManager
            var gem = new KGameEventManager();
            gem.RegisterEvent(new KGameEventManager.TimedGameEvent
            {
                EventUid = 10,
                EventName = "2x Weekend",
                StartUtc = DateTime.UtcNow.AddHours(-1),
                EndUtc = DateTime.UtcNow.AddHours(1),
                ExpRate = 2.0f,
                ConnectMinutesRequired = 1,
                RewardItemId = 99828,
            });
            Assert.Equal(2.0f, gem.GetActiveRates().expRate);
            Assert.Single(gem.TickUserEvents(u1.UnitUid, 65.0));

            // Packet round-trip verification
            var ks = new KSerializer(region);
            var buf = new KSerBuffer();
            var pkt = new KEGS_TRADE_COMPLETE_NOT { Ed = 2500, InventorySlotInfo = tradeRes.unitBItems };
            ks.BeginWriting(buf);
            Assert.True(pkt.Put(ks, region));
            ks.EndWriting();
            var pkt2 = new KEGS_TRADE_COMPLETE_NOT();
            ks.BeginReading(buf);
            Assert.True(pkt2.Get(ks, region));
            ks.EndReading();
            Assert.Equal(2500, pkt2.Ed);
            Assert.Single(pkt2.InventorySlotInfo);
        }
        finally
        {
            if (File.Exists(dbPath)) File.Delete(dbPath);
        }
    }

    [Fact]
    public void Exp_ClassChange_Filter_Punishment_Buffs_PcBang_BlockList_Valentine_And_Wedding_Work()
    {
        var exp = new KExpTable();
        int lv2Exp = exp.GetRequireTotalExpByLevel(2);
        Assert.True(lv2Exp > 0);
        Assert.Equal((byte)2, exp.CheckLevelUp(1, lv2Exp));

        var cc = new KClassChangeTable();
        Assert.True(cc.CompareUnitClass(10, 11));
        Assert.False(cc.CompareUnitClass(10, 20));

        var sf = new KStringFilterManager();
        Assert.False(sf.CheckIsValidString(KStringFilterManager.FilterType.FtNickName, "GM_Tester"));
        Assert.True(sf.CheckIsValidString(KStringFilterManager.FilterType.FtNickName, "ElswordKnight"));
        Assert.Equal("hi ****!", sf.FilteringChatString("hi fuck!"));

        var pun = new KPunishmentManager();
        pun.SetPunishment(new KPunishmentManager.PunishmentEntry
        {
            Type = (int)KPunishmentManager.PunishmentType.PtChat,
            StartUtc = DateTime.UtcNow.AddMinutes(-5),
            EndUtc = DateTime.UtcNow.AddMinutes(5),
        });
        Assert.True(pun.IsAppliedPunishment((int)KPunishmentManager.PunishmentType.PtChat));

        var wed = new KWeddingHallManager();
        Assert.True(wed.AddWeddingInfo(new KWeddingHallManager.WeddingHallEntry { WeddingUid = 1, BrideUnitUid = 10, GroomUnitUid = 20 }));
        Assert.True(wed.UpdateWeddingInfo(1, 555));
        Assert.True(wed.TryGetWeddingInfo(1, out var winfo) && winfo!.RoomUid == 555);

        var randEv = new KRandomItemEventManager();
        randEv.UpdateEventInfo([
            new KRandomItemEventManager.RandomItemEventEntry
            {
                EventId = 1, RewardItemId = 99999, OpenItemId = 0,
                DropStartUtc = DateTime.UtcNow.AddHours(-1), DropEndUtc = DateTime.UtcNow.AddHours(1),
            }
        ], []);
        Assert.Equal(99999, randEv.CheckTimeOpenRandomItemEvent(100, 200, 12345));
        Assert.Equal(0, randEv.CheckTimeOpenRandomItemEvent(100, 200, 12345)); // already claimed

        var val = new KValentineManager();
        var start = DateTime.UtcNow;
        val.SetStartTimer(KValentineManager.TimerSlot.AfterDungeonStart, true, start);
        Assert.Equal(KValentineManager.TimeCheckResult.EndGame, val.CheckTimer(KValentineManager.TimerSlot.AfterDungeonStart, start.AddSeconds(100)));

        var gbl = new KGlobalBlockListManager();
        gbl.SetTradeBlock(50, true, "RMT");
        gbl.AddHackingModule("cheat.dll");
        Assert.True(gbl.IsTradeBlockUser(50));
        Assert.Single(gbl.CheckHackingModuleList(["kernel32.dll", "cheat.dll"]));

        var wm = new KWorldMissionManager();
        wm.CompleteMission(true, 15);
        Assert.Equal(4, wm.GetDefenseRewardBuffLevel());
        Assert.True(wm.IsWorldBuffActive());

        var bl = new KBlackList();
        Assert.True(bl.AddNewBlackListUser(99, "Troll"));
        Assert.True(bl.VerifyBlackListUser("troll"));
        Assert.True(bl.DeleteBlackListUser("TROLL", out long rem) && rem == 99);

        var buffs = new KUserServerBuffManager();
        buffs.ActivateBuff(new KUserServerBuffManager.ServerBuffInfo { BuffId = 1, ExpRate = 0.5f, DeactivateOnDeath = true });
        Assert.Equal(0.5f, buffs.GetBuffBonusRates().expBonus);
        Assert.Single(buffs.OnUserDied());
        Assert.False(buffs.IsBuffActivated(1));

        var pcb = new KUserPcBangManager { PcBangType = 2 };
        pcb.InitPcBangAuth(1, 1, 1);
        Assert.True(pcb.GetPcBangReward().unlimitedSpirit);

        var cash = new KUserCashInventory();
        cash.SetSocketForCash(new Dictionary<int, int> { [70001] = 30050 });
        Assert.True(cash.TryGetSocketOptionForCash(70001, out int sockOpt) && sockOpt == 30050);
    }

    [Fact]
    public void Statistics_AgencyShop_HenirRanking_Fever_AttribNpc_ED_TempInv_Dungeon_Quest_And_Jumping_Work()
    {
        var stats = new KStatisticsManager();
        stats.AddItemStat(KStatisticsManager.StatInventory.SiItemBuy, 110001, 2);
        stats.IncrementCounter("DungeonClears");
        Assert.Equal(2, stats.GetItemStat(KStatisticsManager.StatInventory.SiItemBuy, 110001));
        Assert.Single(stats.FlushAndResetCounters());

        var agency = new KPersonalShopAgencyManager();
        var shop = agency.CreateAgencyShop(10, "Merchant", "Best Gears",
            [new KPersonalShopAgencyManager.AgencyShopItem { ItemUid = 1, ItemId = 55001, Quantity = 5, PricePerUnit = 1000 }],
            TimeSpan.FromHours(12));
        Assert.NotNull(shop);
        var (bought, paid, net, _) = agency.BuyFromAgencyShop(shop!.ShopUid, 1, 2, 5000);
        Assert.True(bought);
        Assert.Equal(2000, paid);
        Assert.Equal(1800, net); // 10% commission
        var (closed, earned, rem) = agency.PickUpAndCloseShop(10);
        Assert.True(closed);
        Assert.Equal(1800, earned);
        Assert.Equal(3, rem[0].Quantity);

        var rank = new KHenirRankingManager();
        Assert.True(rank.CheckNewRecord(new KHenirRankingManager.HenirRecord { UnitUid = 1, NickName = "A", StageCount = 20, ClearTimeSeconds = 400 }));
        Assert.True(rank.CheckNewRecord(new KHenirRankingManager.HenirRecord { UnitUid = 2, NickName = "B", StageCount = 20, ClearTimeSeconds = 350 }));
        Assert.Equal(2, rank.GetRankings()[0].UnitUid);

        var fever = new KCnPartyFeverManager();
        int inc = fever.CalcIncreaseFeverPoint(4, isBattleField: false, subStageNpcCount: 3, hasComeBackUserInParty: true);
        Assert.Equal(60, inc);
        var (fres, disp) = fever.UpdateFeverPoint(KCnPartyFeverManager.ActivateFeverPoint, 4);
        Assert.Equal(KCnPartyFeverManager.UpdateFeverPointResult.UfpActiveFever, fres);
        Assert.Equal(KCnPartyFeverManager.Fever100PercentPoint, disp);

        var attribNpc = new KAttribNpcTable();
        Assert.NotEqual(KAttribNpcTable.AttribType.AtNone, attribNpc.RollNpcAttrib(0, 2, fixedUnitRoll: 0.01));

        var edMgr = new KUserEDManager();
        edMgr.Init(1000);
        Assert.False(edMgr.TryAddEd(-2000, KUserEDManager.EdReason.ErOutEdBuyItemNpcShop, out _));
        Assert.True(edMgr.TryAddEd(KUserEDManager.MaxEd, KUserEDManager.EdReason.ErInEdQuestReward, out int capped));
        Assert.Equal(KUserEDManager.MaxEd, capped);

        var tempInv = new KTempInventory();
        var tItem = tempInv.AddTempInventoryItem(88001, 0);
        Assert.False(tempInv.IsEmpty);
        Assert.True(tempInv.PrepareTempItemToInventory(tItem.TempItemUid, out var moved) && moved!.ItemId == 88001);
        Assert.True(tempInv.IsEmpty);

        var dungMgr = new KUserDungeonManager();
        dungMgr.Init(false, 1, 2, 0, 0, 0, isPcBang: true);
        Assert.True(dungMgr.TryConsumeHenirReward(KUserDungeonManager.ModeHenirNormal));
        Assert.True(dungMgr.TryConsumeHenirReward(KUserDungeonManager.ModeHenirChallenge));
        Assert.False(dungMgr.TryConsumeHenirReward(KUserDungeonManager.ModeHenirChallenge));

        var questMgr = new KUserQuestManager();
        questMgr.RegisterTemplate(new KUserQuestManager.QuestTemplate
        {
            QuestId = 500, TargetNpcId = 9001, TargetKillCount = 2, RewardEd = 3000, RewardExp = 1200,
        });
        Assert.True(questMgr.AcceptQuest(1, 500).ok);
        Assert.False(questMgr.CompleteQuest(500).ok); // kill count not met yet
        Assert.Single(questMgr.OnNpcUnitDie(30000, 9001));
        Assert.Single(questMgr.OnNpcUnitDie(30000, 9001));
        var (qDone, qEd, qExp) = questMgr.CompleteQuest(500);
        Assert.True(qDone);
        Assert.Equal(3000, qEd);
        Assert.Equal(1200, qExp);

        var jump = new KUserJumpingCharacterManager();
        Assert.Equal(KUserJumpingCharacterManager.JumpingCharacterResult.JcrSucceed, jump.Init(new Dictionary<long, DateTime>()));
        Assert.Equal(KUserJumpingCharacterManager.JumpingCharacterResult.JcrSucceed,
            jump.Check(1, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1), accountEvent: true));
        jump.SetJumpingCharacter(1);
        Assert.Equal(KUserJumpingCharacterManager.JumpingCharacterResult.JcrFailed,
            jump.Check(2, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1), accountEvent: true));

        // TW-verified Skill/Quest/Shop/Pet/AutoParty packet round-trips
        var region = RegionProfile.FromCode(RegionCode.TW);
        var ks = new KSerializer(region);

        void RoundTrip<T>(T src, Action<T> assertFn) where T : IKSerializable, new()
        {
            var buf = new KSerBuffer();
            ks.BeginWriting(buf);
            Assert.True(src.Put(ks, region));
            ks.EndWriting();
            var dst = new T();
            ks.BeginReading(buf);
            Assert.True(dst.Get(ks, region));
            ks.EndReading();
            assertFn(dst);
        }

        RoundTrip(new KEGS_GET_SKILL_REQ
        {
            NowLearnSkill = [1001],
            ActiveSkillPageNumber = 1,
            SkillList = new Dictionary<int, KGetSkillInfo>
            {
                [1001] = new KGetSkillInfo { SkillId = 1001, BeforeSkillLevel = 0, SkillLevel = 3 },
            },
        }, d =>
        {
            Assert.Single(d.NowLearnSkill);
            Assert.Equal(3, d.SkillList[1001].SkillLevel);
        });

        RoundTrip(new KEGS_REG_PERSONAL_SHOP_ITEM_REQ
        {
            PersonalShopName = "TW Market",
            RegItems = [new KRegPersonalShopItemInfo { ItemUid = 77, Quantity = 2, PricePerOne = 5000 }],
        }, d => Assert.Equal("TW Market", d.PersonalShopName));

        RoundTrip(new KEGS_AUTO_PARTY_MAKING_SUCCESS_NOT
        {
            AutoPartyUid = 800001, MemberCount = 4, DungeonId = 30100, DifficultyLevel = 2, DungeonMode = 0,
        }, d => Assert.Equal(800001, d.AutoPartyUid));

        RoundTrip(new KQuestAck { Ok = 0, QuestId = 500 }, d => Assert.Equal(500, d.QuestId));

        RoundTrip(new KEGS_BUY_CASH_ITEM_REQ
        {
            IsWishList = false,
            PurchaseReqInfo = [new KNXBTPurchaseReqInfo { ProductNo = 70001, OrderQuantity = 2 }],
        }, d => Assert.Equal(70001u, d.PurchaseReqInfo[0].ProductNo));

        RoundTrip(new KEGS_GET_GUILD_AD_LIST_ACK
        {
            SortType = 4,
            TotalPage = 1,
            ViewPage = 0,
            GuildAdList = [new KGuildAdInfo { GuildUid = 1, GuildName = "ElSearch", GuildLevel = 5 }],
        }, d => Assert.Equal("ElSearch", d.GuildAdList[0].GuildName));

        RoundTrip(new KEGS_COUPLE_PROPOSE_AGREE_NOT
        {
            Answer = 0, RequestUnitUid = 1, RequestUnitName = "Els", AcceptUnitUid = 2, AcceptUnitName = "Aisha",
        }, d => Assert.Equal("Els", d.RequestUnitName));

        var bfm = new KBattleFieldMonsterManager();
        bfm.StartGame(20001, 2, 10, isBossField: true);
        Assert.True(bfm.IsAliveBossMonster(200011));
        bfm.RecordNpcDie(200011, 4, 5001);
        Assert.False(bfm.IsAliveBossMonster(200011));

        // KUserLetterBox parity (6-per-page, unread top-5, ED=127000, claim & delete)
        var letterBox = new KUserLetterBox();
        var (nUser, nSys, nRel, nTitles) = letterBox.Init("Els",
        [
            new KPostItemInfo
            {
                PostNo = 10, ScriptType = (sbyte)KUserLetterBox.LetterType.LtPostOffice,
                ScriptIndex = KUserLetterBox.EdiBronzeEd, Quantity = 2500,
                FromNickName = "Aisha", Title = "Allowance", RegDate = "2026-11-20 01:00:00", Read = false,
            },
            new KPostItemInfo
            {
                PostNo = 11, ScriptType = (sbyte)KUserLetterBox.LetterType.LtWeddingInvitation,
                ScriptIndex = 5001, Quantity = 1,
                FromNickName = "Rena", Title = "Invite", RegDate = "2026-11-20 02:00:00", Read = false,
            },
        ]);
        Assert.Equal(1, nUser);
        Assert.Equal(0, nSys);
        Assert.Equal(1, nRel);
        Assert.Equal(2, nTitles.Count);
        Assert.True(letterBox.GetLetterList((sbyte)KUserLetterBox.LetterListType.LltNormal, 1, out var userTitles));
        Assert.Single(userTitles);
        Assert.Equal("Allowance", userTitles[0].Title);
        letterBox.SetReceivedFlag(10);
        Assert.True(letterBox.TryGetLetter(10, out var post10) && post10!.ScriptIndex == 0);
        letterBox.DeleteLetter(10);
        Assert.False(letterBox.IsExist(10));

        // KGbAutoPartyManager & KGbMatchMakingManager accept/decline FSM pipeline
        var apMgr = new KGbAutoPartyManager();
        apMgr.Enqueue(new AutoPartyEntry(101, "U1", 30100, 1, DateTime.UtcNow));
        apMgr.Enqueue(new AutoPartyEntry(102, "U2", 30100, 1, DateTime.UtcNow));
        Assert.True(apMgr.TryDequeueGroup(out var apGroup) && apGroup != null);
        Assert.True(apMgr.SubmitReply(101, true, out var apReady1, out _));
        Assert.Null(apReady1);
        Assert.True(apMgr.SubmitReply(102, true, out var apReady2, out _));
        Assert.NotNull(apReady2);

        var mmMgr = new KGbMatchMakingManager();
        mmMgr.Enqueue(new MatchRequestEntry(201, "P1", 1000, 1, DateTime.UtcNow));
        mmMgr.Enqueue(new MatchRequestEntry(202, "P2", 1050, 1, DateTime.UtcNow));
        Assert.True(mmMgr.TryDequeueMatch(out var mmMatch) && mmMatch != null);
        Assert.True(mmMgr.SubmitReply(201, false, out var mmReady, out var mmCancelled));
        Assert.Null(mmReady);
        Assert.NotNull(mmCancelled);

        // New Mail / Friend / BlackList / Community / DeleteItem packet round-trips
        RoundTrip(new KEGS_GET_ITEM_FROM_LETTER_ACK
        {
            Ok = 0, PostNo = 10, LetterType = 1, Ed = 5000,
            ResultItem = new Dictionary<int, int> { [130001] = 3 },
        }, d => Assert.Equal(3, d.ResultItem[130001]));

        RoundTrip(new KEGS_NEW_BLACKLIST_USER_ACK
        {
            Ok = 0, ChatBlackListUnit = new KChatBlackListUnit { UnitUid = 777, NickName = "Spammer" },
        }, d => Assert.Equal("Spammer", d.ChatBlackListUnit.NickName));

        RoundTrip(new KEGS_COMMUNITY_USER_LIST_ACK
        {
            Ok = 0, TotalPage = 1, ViewPage = 1,
            UserList = [new KCommunityUserInfo { UnitUid = 1, NickName = "Els", UnitClass = 100, Level = 70, Ed = 9999 }],
        }, d => Assert.Equal(9999, d.UserList[0].Ed));

        RoundTrip(new KEGS_GAME_START_PVP_MATCH_NOT
        {
            RoomInfo = new KRoomInfo { RoomUid = 9001, RoomName = "Arena", WorldId = 10001 },
            PvpNpcInfo = new Dictionary<long, KPvpNpcInfo> { [1] = new KPvpNpcInfo { NpcId = 500 } },
        }, d => Assert.Equal(500, d.PvpNpcInfo[1].NpcId));

        RoundTrip(new KEGS_DELETE_ITEM_REQ { ItemUid = 55, Quantity = 2 }, d => Assert.Equal(55, d.ItemUid));

        // Dungeon & BattleField room + packet parity
        var cnRoomMgr = new KCnRoomManager();
        var bfRoom = cnRoomMgr.GetOrCreateBattleFieldRoom(40001);
        Assert.Equal(3, bfRoom.Info.RoomType);
        bfRoom.BeginStageLoad(1);
        int dropUid = bfRoom.RegisterDropItem(130001, 2);
        Assert.True(bfRoom.TryClaimDropItem(dropUid, out var claimedDrop) && claimedDrop.itemId == 130001);

        RoundTrip(new KEGS_DUNGEON_STAGE_LOAD_NOT
        {
            StageId = 1,
            NumMember = 2,
            NpcData = new Dictionary<int, KNPCList>
            {
                [0] = new KNPCList { NpcList = [new KNPCUnitReq { Uid = 1, NpcId = 9001, Level = 5 }] },
            },
            AttribNpcInfo = new Dictionary<int, KAttribEnchantNpcInfo>
            {
                [1] = new KAttribEnchantNpcInfo { AttribEnchant1 = 1 },
            },
            ItemDropEventProbRate = 1.5f,
        }, d => Assert.Equal(9001, d.NpcData[0].NpcList[0].NpcId));

        RoundTrip(new KEGS_NPC_UNIT_DIE_NOT
        {
            DieNpcUid = 1,
            NpcId = 9001,
            ExpList = new Dictionary<long, KEXPData> { [1001] = new KEXPData { Exp = 120, PartyExp = 25 } },
        }, d => Assert.Equal(120, d.ExpList[1001].Exp));

        RoundTrip(new KEGS_GET_ITEM_NOT
        {
            GetUnitUid = 1001,
            ItemId = KUserLetterBox.EdiBronzeEd,
            TotalEd = 300,
            GetEd = new Dictionary<long, KDungeonRewardED> { [1001] = new KDungeonRewardED { Ed = 150 } },
        }, d => Assert.Equal(150, d.GetEd[1001].Ed));

        RoundTrip(new KEGS_JOIN_BATTLE_FIELD_ACK
        {
            Ok = 0,
            BattleFieldJoinInfo = new KBattleFieldJoinInfo { BattleFieldId = 40001, StartPosIndex = 1 },
            RoomInfo = bfRoom.Info,
            UdpRelayPort = 9303,
        }, d => Assert.Equal(40001, d.BattleFieldJoinInfo.BattleFieldId));

        // Session / Field / Room / Party / PvP Room parity
        Assert.True(KFieldOption.VerifyLimitOption(35));
        var partyMgr = new KCnPartyManager();
        var p = partyMgr.CreateParty(new KCreatePartyInfo { DungeonId = 30000 }, new KPartyUserInfo { UnitUid = 1001, NickName = "Host" });
        Assert.True(p.TryJoin(new KPartyUserInfo { UnitUid = 1002, NickName = "Mate" }));
        Assert.True(p.ChangeMaxSlot(1001, 3));
        Assert.True(p.ChangeHost(1001, 1002, out long bUid, out long aUid) && bUid == 1001 && aUid == 1002);
        Assert.True(p.BanUser(1002, 1001));

        RoundTrip(new KEGS_GET_CHANNEL_LIST_ACK
        {
            ServerGroupId = 1,
            ChannelInfo = [new KChannelInfo { ChannelId = 1, ChannelName = "TW-CH1" }],
            ChannelBonusInfo = new Dictionary<int, KChannelBonusInfo> { [1] = new KChannelBonusInfo { Enable = true, PerExp = 20 } },
        }, d => Assert.Equal("TW-CH1", d.ChannelInfo[0].ChannelName));

        RoundTrip(new KEGS_CREATE_TC_ROOM_ACK
        {
            Ok = 0,
            TcInfo = new KTCInfo { TcId = 5, DungeonId = 30005, RoomUid = 1001, RoomType = 4, PlayTime = 180f },
        }, d => Assert.Equal(5, d.TcInfo.TcId));

        RoundTrip(new KEGS_CHANGE_PVP_ROOM_PUBLIC_REQ { Public = false, Password = "777" }, d => Assert.Equal("777", d.Password));
        RoundTrip(new KEGS_CHANGE_PVP_ROOM_NAME_ACK { RoomName = "1v1 Pro", Ok = 0 }, d => Assert.Equal("1v1 Pro", d.RoomName));
        RoundTrip(new KEGS_QUICK_START_DUNGEON_GAME_REQ { DungeonId = 30000, DifficultyLevel = 2 }, d => Assert.Equal(30000, d.DungeonId));

        // Pet / Riding Pet / 2nd Security / Inventory & Bank parity
        RoundTrip(new KPetInfo
        {
            PetUid = 88, PetId = 10, PetName = "Phoru", AutoFeed = true, FreeAutoLooting = true, DestroyDate = "2099-01-01",
        }, d => Assert.Equal("Phoru", d.PetName));
        RoundTrip(new KRidingPetInfo
        {
            RidingPetUid = 99, RidingPetId = 5, Stamina = 85.5f, DestroyDate = "2099-01-01", LastUnSummonDate = 12345,
        }, d => Assert.Equal((ushort)5, d.RidingPetId));
        RoundTrip(new KEGS_GET_PET_LIST_ACK
        {
            Ok = 0, TotalPage = 1, ViewPage = 0, PetList = [new KPetInfo { PetUid = 88, PetId = 10, PetName = "Phoru" }],
        }, d => Assert.Equal(1, d.PetList.Count));
        RoundTrip(new KEGS_OPEN_RANDOM_ITEM_ACK
        {
            Ok = 0, InsertedItem = new Dictionary<int, int> { [130001] = 2 }, Ed = 5000,
        }, d => Assert.Equal(2, d.InsertedItem[130001]));
        RoundTrip(new KEGS_ITEM_EXCHANGE_ACK
        {
            Ok = 0, ResultItem = new Dictionary<int, int> { [130010] = 1 }, ExchangeCount = 3,
        }, d => Assert.Equal(3, d.ExchangeCount));

        // Personal Shop / Agency / Trade Board & Guild Management parity
        RoundTrip(new KEGS_JOIN_MY_PSHOP_AGENCY_ACK
        {
            Ok = 0, JoinType = 1, PersonalShopName = "Agency", NickName = "Seller", UsedItemId = 9001,
            SellItemInfo = [new KSellPersonalShopItemInfo { PricePerOne = 5000, InventoryItemUid = 77 }],
        }, d => Assert.Equal(77, d.SellItemInfo[0].InventoryItemUid));

        RoundTrip(new KEGS_SEARCH_TRADE_BOARD_ACK
        {
            Ok = 0,
            ItemInfo = [new KTradeBoardItemInfo { PersonalShopUid = 10, HostUnitUid = 20, SellerNickName = "Alice" }],
        }, d => Assert.Equal("Alice", d.ItemInfo[0].SellerNickName));

        RoundTrip(new KEGS_GET_GUILD_USER_LIST_ACK
        {
            Ok = 0, TotalPage = 1, ViewPage = 1, LoginedMemberCount = 1,
            MemberPositionGradeList = [new KGuildMemberInfo { UnitUid = 20, NickName = "Master", MemberShipGrade = 1 }],
            MemberMessageList = [new KGuildMemberMessageInfo { UnitUid = 20, NickName = "Master", Message = "Hi" }],
        }, d => Assert.Equal("Master", d.MemberPositionGradeList[0].NickName));

        RoundTrip(new KEGS_GET_GUILD_SKILL_IN_BOARD_ACK
        {
            Ok = 0,
            GuildSkillInfo = new KGuildSkillInfo
            {
                GuildSkill = [new KGuildSkillData { SkillId = 5001, SkillLevel = 3 }],
                GuildSPoint = 7,
            },
        }, d => Assert.Equal(3, d.GuildSkillInfo.GuildSkill[0].SkillLevel));

        // 40-handler batch: Letter/PostBlackList/LoveLetter, Messenger/FriendGroups/KNM,
        // Relationship/Wedding/CallMyLover, Match/PartyType/PvP NPC, AutoPartyBonus, AntiAddiction
        Assert.True(KStringFilterManager.IsInvalidMarkIn("bad nick"));
        Assert.True(KStringFilterManager.IsInvalidMarkInForLetter("it's"));
        Assert.False(KStringFilterManager.IsInvalidMarkInForLetter("Hello love!"));

        var relMgr = new KUserRelationshipManager();
        relMgr.SetRelationshipType(KUserRelationshipManager.RelationshipType.RtCouple);
        relMgr.AnniversaryDayUtc = DateTime.UtcNow.AddDays(-2);
        Assert.True(relMgr.GetBreakUpCost() >= 200_000);
        Assert.True(KUserRelationshipManager.IsValidWeddingHall(KUserRelationshipManager.WhtCashA));
        Assert.True(KUserRelationshipManager.IsValidWeddingOfficiantNpc(KUserRelationshipManager.WotNpcA));
        Assert.True(KUserRelationshipManager.IsWeddingItem(KUserRelationshipManager.SiWeddingReservationItem, out sbyte ws) && ws == KUserRelationshipManager.WsGroom);

        RoundTrip(new KEGS_GET_POST_BLACK_LIST_ACK
        {
            BlackList = ["SpammerA", "SpammerB"],
        }, d => Assert.Equal(2, d.BlackList.Count));

        RoundTrip(new KEGS_SEND_LOVE_LETTER_EVENT_ACK
        {
            Ok = 0, Ed = 90000,
        }, d => Assert.Equal(90000, d.Ed));

        RoundTrip(new KEGS_MAKE_FRIEND_GROUP_ACK
        {
            Ok = 0, GroupId = 2, GroupName = "Squad",
        }, d => Assert.Equal("Squad", d.GroupName));

        RoundTrip(new KEGS_KNM_TOGETHER_ACK
        {
            Ok = 0,
            UnitInfo = new KNMUnitInfo { UnitUid = 5001, State = 6, RoomUid = 1234 },
        }, d => Assert.Equal(1234L, d.UnitInfo.RoomUid));

        RoundTrip(new KEGS_WEDDING_PROPOSE_REQ
        {
            WeddingHallType = KUserRelationshipManager.WhtCashB,
            OfficiantNpc = KUserRelationshipManager.WotNpcA,
            WeddingDate = 1735689600,
            WeddingMsg = "Forever!",
            InviteUnitList = [1001, 1002],
        }, d => Assert.Equal("Forever!", d.WeddingMsg));

        RoundTrip(new KEGS_JOIN_WEDDING_HALL_ACK
        {
            Ok = 0,
            WeddingInfo = new KWeddingHallInfo { WeddingUid = 11, Groom = 100, Bride = 200, WeddingMsg = "Yay" },
            RoomInfo = new KRoomInfo { RoomType = 8, RoomUid = 999, RoomName = "WeddingHall_11" },
            CnIp = "127.0.0.1",
            UdpRelayIp = "127.0.0.1",
            UdpRelayPort = 9303,
        }, d => Assert.Equal(8, d.RoomInfo.RoomType));

        RoundTrip(new KEGS_CALL_MY_LOVER_JOIN_BATTLE_FIELD_NOT
        {
            Ok = 0,
            BattleFieldJoinInfo = new KBattleFieldJoinInfo { BattleFieldId = 40001, StartPosIndex = 1 },
            RoomInfo = new KRoomInfo { RoomType = 3, RoomUid = 777 },
            CalledUserUid = 10,
            CalledGsUid = 1,
        }, d => Assert.Equal(40001, d.BattleFieldJoinInfo.BattleFieldId));

        RoundTrip(new KEGS_RELATIONSHIP_INFO_ACK
        {
            Ok = 0, RequestUnitUid = 100, AcceptUnitUid = 200, MyLevel = 70, Level = 68, IsConnect = true,
        }, d => Assert.True(d.IsConnect));

        Assert.True(p.ChangePartyType(1002, new KPartyInfo { PartyType = 1, DungeonId = 30010, MaxSlot = 4 }));
        Assert.True(p.ChangePvpMatchInfo(1002, 2));

        var autoPartyMgr = new KGbAutoPartyManager();
        autoPartyMgr.SetAutoPartyBonus(0, 30000, true);
        var bonusOut = new Dictionary<int, KAutoPartyDungeonBonusInfo>();
        KUserDungeonManager.GetAutoPartyBonusDungeonList(
            [new KDungeonGameSimpleInfo { DungeonId = 30000, DungeonMode = 0 }],
            autoPartyMgr.IsAutoPartyBonus,
            bonusOut);
        Assert.True(bonusOut[30000].ActivatedBonus);

        RoundTrip(new KEGS_UPDATE_AUTO_PARTY_BONUS_INFO_NOT
        {
            DungeonBonusList = bonusOut,
        }, d => Assert.True(d.DungeonBonusList[30000].ActivatedBonus));

        RoundTrip(new KEGS_ANTI_ADDICTION_NOT
        {
            Type = KEGS_ANTI_ADDICTION_NOT.NtCenter, PlayTime = 180,
        }, d => Assert.Equal(180, d.PlayTime));
    }

    [Fact]
    public void Gameplay_47Handler_Batch_RoomManager_Square_Packets_And_DB_Work()
    {
        var ks = new KSerializer(RegionProfile.FromCode(RegionCode.TW));
        var reg = RegionProfile.FromCode(RegionCode.TW);
        void RoundTrip<T>(T pkt, Action<T> assert) where T : IKSerializable, new()
        {
            var buf = new KSerBuffer();
            Assert.True(ks.BeginWriting(buf));
            Assert.True(pkt.Put(ks, reg));
            ks.EndWriting();
            Assert.True(ks.BeginReading(buf));
            var dec = new T();
            Assert.True(dec.Get(ks, reg));
            ks.EndReading();
            assert(dec);
        }

        // Common DTO & packet round-trips
        RoundTrip(new KEGS_OPEN_PVP_ROOM_LIST_ACK
        {
            Ok = NetError.NET_OK,
            ConnectChannelId = 2,
            Channel = [new KPVPChannelInfo { ChannelId = 2, PvpChannelClass = KPVPChannelInfo.PccFree, MinRating = 0, MaxRating = 9999 }],
        }, d => Assert.Equal(KPVPChannelInfo.PccFree, d.Channel[0].PvpChannelClass));

        var pvpDrop = new KEGS_CREATE_PVP_ITEM_NOT
        {
            PositionIndex = 3,
            DropItemData = new KDropItemData { ItemId = 130001, DropItemUid = 1, Speed = 1.0f, EnchantLevel = 2 },
        };
        pvpDrop.DropItemData.GetPossibleUnitUid.Add(1001);
        RoundTrip(pvpDrop, d =>
        {
            Assert.Equal(3, d.PositionIndex);
            Assert.Contains(1001L, d.DropItemData.GetPossibleUnitUid);
            Assert.Equal(2, d.DropItemData.EnchantLevel);
        });

        var myResultReq = new KEGS_MY_USER_UNIT_INFO_TO_SERVER_REQ
        {
            MyPlayResult = new KDungeonPlayResultInfo
            {
                Hp = 95.5f, TotalScore = 50000, ComboScore = 15000, TechScore = 12000,
                DashCount = 14, Frame = 60, SkillSlotType = true,
            },
        };
        RoundTrip(myResultReq, d => Assert.Equal(50000, d.MyPlayResult.TotalScore));

        var resultReq = new KEGS_RESULT_SUCCESS_REQ();
        resultReq.GamePlayNetworkInfo.GamePlayNetworkInfo.Add(new KGamePlayNetworkInfo
        {
            OpponentUnitUid = 2002, HolePunchingTryCount = 1, P2PPlayTime = 120000f, RelayPlayTime = 0f,
        });
        RoundTrip(resultReq, d => Assert.Equal(2002L, d.GamePlayNetworkInfo.GamePlayNetworkInfo[0].OpponentUnitUid));

        RoundTrip(new KEGS_SQUARE_UNIT_SYNC_DATA_NOT
        {
            UnitUid = 1001, StateId = 2, StateChangeNum = 5, Pos = new KVector3 { X = 10f, Y = 20f, Z = 0f },
            LastTouchLineIndex = 1, IsRight = true, Ed = 75000,
        }, d => Assert.Equal(75000, d.Ed));

        var buffReq = new KEGS_BUFF_SKILL_INFO_CONVEY_REQ
        {
            BuffEnum = 7, CastingUnitUid = 1001, BuffTime = 10f, IsBuffCasting = true, BuffLevel = 2,
        };
        buffReq.CaughtUidList.Add(1001);
        RoundTrip(buffReq, d => Assert.Equal(7, d.BuffEnum));

        var bfAck = new KEGS_BATTLE_FIELD_NPC_DAMAGED_MAP_ACK { UnitUid = 1001, NpcUid = 55 };
        bfAck.DamagedMap[1001] = 3250.5f;
        RoundTrip(bfAck, d => Assert.Equal(3250.5f, d.DamagedMap[1001]));

        // KCnRoom & Square gameplay parity
        var rm = new KCnRoomManager();
        var host = new KRoomUserInfo { UnitUid = 1001, NickName = "Host", Level = 70 };
        var guest = new KRoomUserInfo { UnitUid = 1002, NickName = "Guest", Level = 68 };
        var pvpRoom = rm.CreateRoom(new KRoomInfo
        {
            RoomType = 0, Public = true, MaxSlot = 4, PvpGameType = 1, PvpChannelClass = KPVPChannelInfo.PccFree,
        }, host);
        Assert.Equal(NetError.NET_OK, rm.JoinRoom(pvpRoom.Info.RoomUid, guest, string.Empty, false).ok);
        Assert.Equal(pvpRoom.Info.RoomUid, rm.GetWaitingRoomUid(0, 1, false));

        var (slotOk, tbIdx) = pvpRoom.ChangeSlotOpen(1001, 2, 0);
        Assert.Equal(NetError.NET_OK, slotOk);
        Assert.Equal(NetError.NET_OK, pvpRoom.ChangeWinningNumKill(1001, 5));
        Assert.Equal(NetError.NET_OK, pvpRoom.ChangePvpGameOption(1001, true, false));
        Assert.Equal(NetError.NET_OK, pvpRoom.TogglePvpItemMode(out bool itemMode));
        Assert.True(itemMode);

        pvpRoom.StartGame();
        Assert.Equal(NetError.NET_OK, pvpRoom.CreatePvpItem(2, 130001, out var createdDrop));
        Assert.NotNull(createdDrop);
        Assert.Equal(NetError.NET_OK, pvpRoom.ChangePvpItemPos(createdDrop!.DropItemUid, 4));
        Assert.Equal(NetError.NET_OK, pvpRoom.SetUnitRebirthPos(1001, 1));
        Assert.Equal(NetError.NET_OK, pvpRoom.SetTcRemainingTime(true));
        var (buffOk, buffNot) = pvpRoom.ProcessBuffSkill(buffReq);
        Assert.True(buffOk);
        Assert.True(buffNot.CaughtUidList.ContainsKey(7));

        // Square list / join / leave parity
        var (sqList, totalSqPages, _) = rm.GetSquareInfoPage(0, 1);
        Assert.NotEmpty(sqList);
        Assert.True(totalSqPages >= 1);
        var (sqJoinOk, sqInfo, sqUsers, sqRoom) = rm.JoinSquare(sqList[0].SquareUid, new KSquareUserInfo { UnitUid = 1001, NickName = "Host", Ed = 5000 });
        Assert.Equal(NetError.NET_OK, sqJoinOk);
        Assert.NotNull(sqInfo);
        Assert.Single(sqUsers);
        Assert.True(rm.LeaveSquare(sqList[0].SquareUid, 1001).removed);

        // SQLite logging for FieldWorkingsBlockLogs & GamePlayNetworkLogs
        string dbPath = Path.Combine(Path.GetTempPath(), $"knc_47batch_{Guid.NewGuid():N}.db");
        try
        {
            var db = new SqliteDatabaseManager(dbPath);
            db.InitializeAndSeed();
            db.RecordFieldWorkingsBlockLog(1001, 6, 1024, 2048, 0, 4, 3);
            db.RecordGamePlayNetworkInfo(1001, resultReq.GamePlayNetworkInfo.GamePlayNetworkInfo);
        }
        finally
        {
            if (File.Exists(dbPath)) File.Delete(dbPath);
        }

        Assert.Equal(48, NetError.ERR_ROOM_LIST_00);
        Assert.Equal(1345, NetError.ERR_ROOM_53);
        Assert.Equal(1368, NetError.ERR_FIELD_BOSS_03);
    }

    [Fact]
    public void SessionSecurityInventoryDungeon_49Handler_Batch_Packets_And_DB_Work()
    {
        var region = RegionProfile.FromCode(RegionCode.TW);
        static void RoundTrip<T>(T pkt, Action<T> assert) where T : IKSerializable, new()
        {
            var tw = RegionProfile.FromCode(RegionCode.TW);
            var buf = new KSerBuffer();
            var ksOut = new KSerializer(tw);
            ksOut.BeginWriting(buf);
            Assert.True(pkt.Put(ksOut, tw));
            ksOut.EndWriting();
            var ksIn = new KSerializer(tw);
            ksIn.BeginReading(new KSerBuffer(buf.ToArray()));
            var dec = new T();
            Assert.True(dec.Get(ksIn, tw));
            ksIn.EndReading();
            assert(dec);
        }

        RoundTrip(new KEGS_CONNECT_CHANNEL_CHANGE_REQ
        {
            ConnectReq = new KEGS_CONNECT_REQ { Version = "TW_2014" },
            VerifyAccountReq = new KUserAuthenticateReq { UserId = "test01", ChannelingCode = 1, ManualLogin = true },
            SelectUnitReq = new KEGS_SELECT_UNIT_REQ { UnitUid = 777, CodeEventScriptIds = [10, 20] },
        }, d =>
        {
            Assert.Equal("TW_2014", d.ConnectReq.Version);
            Assert.Equal("test01", d.VerifyAccountReq.UserId);
            Assert.Equal(777L, d.SelectUnitReq.UnitUid);
        });

        RoundTrip(new KEGS_CONNECT_CHANNEL_CHANGE_ACK
        {
            Ok = NetError.NET_OK,
            ConnectAck = new KEGS_CONNECT_ACK { Ok = NetError.NET_OK, UdpPort = 9302, ChannelId = 1 },
            ChangeUserInfo = new KChannelChangeInfo { EquippedTitleId = 42, IsPcBang = true, PcBangType = 2, UnitSlot = 1 },
        }, d =>
        {
            Assert.Equal(42, d.ChangeUserInfo.EquippedTitleId);
            Assert.True(d.ChangeUserInfo.IsPcBang);
        });

        RoundTrip(new KEGS_KOG_GAME_PERFORMANCE_DUNGEON_LOG
        {
            CharUid = 1001, MapId = 30010, AvgFps = 59.8f, Difficulty = 2, Escape = false, Stage = 3, Block = 1,
            UnknownLagInfo = new KDungeonLagInfo { LagType = 1, LagTime = 12.5f },
        }, d =>
        {
            Assert.Equal(30010, d.MapId);
            Assert.Equal(12.5f, d.UnknownLagInfo.LagTime);
        });

        RoundTrip(new KEGS_GET_SHARE_BANK_REQ
        {
            NickName = "AltChar", NewUnitTradeBlockDay = 3, NewUnitTradeBlockUnitClass = 1,
        }, d => Assert.Equal("AltChar", d.NickName));

        RoundTrip(new KEGS_SYNTHESIS_ITEM_REQ
        {
            KeyData = 9001, SocketNo = 2,
        }, d => Assert.Equal(9001L, d.KeyData));

        RoundTrip(new KEGS_REGISTER_RECRUITER_NOT
        {
            RecruiterUnitUid = 500,
            RecruitUnitInfo = new KRecommendUserInfo { UnitUid = 501, NickName = "Newbie", Level = 15, IsOnline = true },
        }, d => Assert.Equal("Newbie", d.RecruitUnitInfo.NickName));

        RoundTrip(new KEGS_USE_FINALITY_SKILL_ACK
        {
            Ok = NetError.NET_OK, NoConsume = true,
        }, d => Assert.True(d.NoConsume));

        var rm = new KCnRoomManager();
        var dungeonRoom = rm.CreateRoom(new KRoomInfo { RoomType = 1, DungeonId = 30010, MaxSlot = 4 },
            new KRoomUserInfo { UnitUid = 1001, NickName = "Host", Level = 70 });
        dungeonRoom.StartGame();
        Assert.True(dungeonRoom.CheckDungeonKillAllNpc(1001));
        Assert.False(dungeonRoom.CheckDungeonKillAllNpc(9999));

        string dbPath = Path.Combine(Path.GetTempPath(), $"knc_49batch_{Guid.NewGuid():N}.db");
        try
        {
            var db = new SqliteDatabaseManager(dbPath);
            db.InitializeAndSeed();
            db.RecordUserSystemInfo(1, 123456, "i7", "16GB", "GTX", "Win11");
            db.RecordGamePerformanceLog("Dungeon", 1001, 123456, 30010, 60.0f, 4);
            db.RecordHackingProcessLog(1, 1001, 1, "cheat.exe");
            db.UpdateHackingUserType(1, 2);

            var (_, acctUid, _, _, _) = db.VerifyOrRegisterAccount("TW", "share_u", "pw");
            var (_, u1, _) = db.CreateUnit(region, acctUid, "BankCharA", 1);
            var (_, u2, _) = db.CreateUnit(region, acctUid, "BankCharB", 2);
            Assert.NotNull(u1);
            Assert.NotNull(u2);

            var granted = db.GrantUnitItem(u1!.UnitUid, 130001, 2, 8);
            Assert.NotNull(granted);
            var share = db.GetShareBankItems(region, acctUid, "BankCharA");
            Assert.Equal(NetError.NET_OK, share.ok);
            Assert.Single(share.items);

            var evalRes = db.SetInventoryItemEvalState(u1.UnitUid, granted!.ItemUid, 0, 1);
            Assert.Equal(NetError.NET_OK, evalRes.ok);
            var restRes = db.SetInventoryItemEvalState(u1.UnitUid, granted.ItemUid, 1, 0);
            Assert.Equal(NetError.NET_OK, restRes.ok);

            db.SetUnitRecruitTicketUsed(u2!.UnitUid);
            var recRes = db.RegisterUnitRecruiter(region, u2.UnitUid, "BankCharA");
            Assert.Equal(NetError.NET_OK, recRes.ok);
            Assert.Equal(u1.UnitUid, recRes.recruiterUid);
        }
        finally
        {
            if (File.Exists(dbPath)) File.Delete(dbPath);
        }

        Assert.Equal(456, NetError.ERR_HACK_USER_00);
        Assert.Equal(1030, NetError.ERR_INVENTORY_LOCK_00);
        Assert.Equal(1306, NetError.ERR_FINALITY_SKILL_00);
    }

    [Fact]
    public void CashShop_And_AdminCheat_80Handler_Batch_Packets_And_DB_Work()
    {
        var region = RegionProfile.FromCode(RegionCode.TW);

        RoundTrip(new KEGS_CHECK_BALANCE_ACK
        {
            Ok = NetError.NET_OK,
            Balance = 5000,
            OnlyType = true,
            CashType = KGlobalCashInfo.GctKogElswordCash,
            GlobalCashInfo = new KGlobalCashInfo { Cash = [1000, 5000, 200, 300] },
        }, d => Assert.Equal(5000u, d.GlobalCashInfo.Cash[1]));

        RoundTrip(new KEGS_BILL_PRODUCT_INFO_ACK
        {
            Final = true,
            ProductInfo = [new KBillProductInfo { ProductNo = 1001, ProductId = 130001, ProductName = "Stone", SalePrice = 100 }],
            KeepShowItem = new Dictionary<int, List<int>> { [1] = [1001] },
            DisCountInfoMap = new Dictionary<int, kDisCountItemInfo> { [1001] = new() { DisCountProNo = 2001, BasicProNo = 1001, DisCountKeepItem = [130001] } },
        }, d => Assert.Equal("Stone", d.ProductInfo[0].ProductName));

        RoundTrip(new KEGS_BILL_INVENTORY_INQUIRY_ACK
        {
            Ok = NetError.NET_OK,
            OrderInfo = [new KBillOrderInfo { TransNo = 77, OrderId = "TW-77", BillProductInfo = new KBillProductInfo { ProductNo = 1002, ProductId = 130048 }, Type = KBillOrderInfo.BcitBuyItem }],
            PackageInfo = [new KBillPackageInfo { PackageNo = 77, VecProductNo = [1001, 1002], ProductNo = 1002, ProductId = 130048, ProductPieces = 1 }],
            SubPackageTrans = [78],
            SubPackageInfo = [1003],
        }, d => Assert.Equal("TW-77", d.OrderInfo[0].OrderId));

        RoundTrip(new KEGS_GET_PURCHASED_PACKAGE_CASH_ITEM_REQ
        {
            OrderNo = 77,
            PackageProductNo = 1002,
            PackageProductId = "130048",
            ProductKind = 1,
            PickUpPackageInfo = [new KNXBTPickUpPackageInfo { ProductNo = 1002, ProductId = "130048", ProductPieces = 1, SubProductNo = 130048 }],
            SocketForCash = new Dictionary<int, int> { [1] = 100 },
            SocketGroupIdForCash = new Dictionary<int, int> { [1] = 2 },
            ProductId = 130048,
        }, d => Assert.Equal(130048, d.PickUpPackageInfo[0].SubProductNo));

        RoundTrip(new KEGS_BINGO_EVENT_INFO_ACK
        {
            Ok = NetError.NET_OK,
            LotteryChance = 3,
            MixChance = 1,
            ResetChance = 1,
            BingoBoard = [1, 2, 3, 4],
            PresentInfo = [(130001, false), (130002, true)],
        }, d => Assert.True(d.PresentInfo[1].Acquired));

        RoundTrip(new KEGS_EXCHANGE_CASH_ACK
        {
            Ok = NetError.NET_OK,
            Point = 1500,
            AddBonusCashInfo = new KGlobalCashInfo { Cash = [0, 1500, 0, 0] },
        }, d => Assert.Equal(1500, d.Point));

        RoundTrip(new KEGS_ADMIN_CHANGE_UNIT_CLASS_ACK
        {
            Ok = NetError.NET_OK,
            UnitClass = 10,
            SPoint = 40,
            CsPoint = 10,
            DefaultSkill1 = 1001,
            DefaultSkill6 = 1006,
        }, d => Assert.Equal(1006, d.DefaultSkill6));

        RoundTrip(new KEGS_ADMIN_CHEAT_GET_ALL_SKILL_ACK
        {
            Ok = NetError.NET_OK,
            ActiveSkillPageNumber = 1,
            UnSealedSkill = [1001, 1002],
            SkillInfo = new Dictionary<int, KAdminCheatSkill> { [1001] = new() { SkillLevel = 5, SkillCsPoint = 1 } },
        }, d => Assert.Equal(5, d.SkillInfo[1001].SkillLevel));

        RoundTrip(new KEGS_ADMIN_CHANGE_VSPOINT_ACK
        {
            Ok = NetError.NET_OK,
            Rank = 4,
            ChangedRPoint = 2500,
            ChangedAPoint = 9000,
        }, d => Assert.Equal(9000, d.ChangedAPoint));

        RoundTrip(new KEGS_ADMIN_HERO_PVP_USER_LIST_ACK
        {
            UserList = [new KHeroPvpUserInfo { ServerGroupId = 0, NickName = "HeroTW", On = true, Connect = true }],
        }, d => Assert.Equal("HeroTW", d.UserList[0].NickName));

        RoundTrip(new KEGS_NOTIFY_MSG_REQ
        {
            NotifyType = KEGS_NOTIFY_MSG_REQ.NtGmNotifyAllServers,
            Count = 1,
            Msg = "TW Maintenance Notice",
            RandomItem = 130048,
        }, d => Assert.Equal(130048, d.RandomItem));

        RoundTrip(new KEGS_ADMIN_SET_PC_BANG_ACK
        {
            Ok = NetError.NET_OK,
            On = true,
            PcBangType = 1,
            PcBangReward = new KPcBangReward { PcBangType = 1, ExpRate = 1.3f, EdRate = 1.2f, SpiritFree = true },
        }, d => Assert.True(d.PcBangReward.SpiritFree));

        var cashInv = new KUserCashInventory();
        var lottery = cashInv.BingoLottery();
        Assert.Equal(NetError.NET_OK, lottery.ok);
        Assert.Equal(1, cashInv.BingoLotteryRecord);
        var mix = cashInv.BingoMix();
        Assert.Equal(NetError.NET_OK, mix.ok);
        var rst = cashInv.BingoReset(false);
        Assert.Equal(NetError.NET_OK, rst.ok);

        var skillTree = new KUserSkillTree();
        skillTree.SetAvailableSkillPoints(1, 55);
        Assert.True(skillTree.LearnOrSetSkill(1001, 5));
        Assert.Equal(5, skillTree.GetSkillLevel(1001));

        var fbm = new KFieldBossManager();
        fbm.SetTotalDangerousValue(20001, 90, isSet: true);
        Assert.Equal(90, fbm.GetTotalDangerousValue(20001));

        string dbPath = Path.Combine(Path.GetTempPath(), $"knc_cash_admin_test_{Guid.NewGuid():N}.db");
        try
        {
            var db = new SqliteDatabaseManager(dbPath);
            db.InitializeAndSeed();
            var (_, acctUid, _, _, _) = db.VerifyOrRegisterAccount("TW", "cash_u", "pw");
            var (_, unit, _) = db.CreateUnit(region, acctUid, "CashHero", 1);
            Assert.NotNull(unit);

            long transNo = db.AddBillingOrder(acctUid, 1002, 130048, "Ice Burner", 2, 300);
            Assert.True(transNo > 0);
            var orders = db.GetBillingOrders(acctUid);
            Assert.Single(orders);
            var picked = db.MarkBillingOrderPickedUp(acctUid, transNo);
            Assert.True(picked.found);
            Assert.Equal(130048, picked.productId);

            long trans2 = db.AddBillingOrder(acctUid, 1001, 130001, "Stone", 1, 100);
            var refund = db.RefundBillingOrder(acctUid, (uint)trans2);
            Assert.True(refund.ok);
            Assert.Equal(100, refund.refundPrice);

            db.ModifyWishList(acctUid, [1001, 1002], isDelete: false);
            Assert.Equal(2, db.GetWishList(acctUid).Count);
            db.ModifyWishList(acctUid, [1001], isDelete: true);
            Assert.Single(db.GetWishList(acctUid));

            var coup = db.RedeemCoupon(acctUid, "TW-ELSWORD-2014", 0);
            Assert.Equal(NetError.NET_OK, coup.ok);
            var coupDup = db.RedeemCoupon(acctUid, "TW-ELSWORD-2014", 0);
            Assert.Equal(NetError.ERR_GASH_10, coupDup.ok);

            db.SetUnitClass(unit!.UnitUid, 10);
            db.SetUnitSkillPoint(unit.UnitUid, 77);
            db.SetUnitPvpStats(unit.UnitUid, 2100, 3000, 9999);
            db.WriteCheatLog(unit.UnitUid, acctUid, "/vspoint", "2100");
            Assert.Equal(acctUid, db.GetAccountUidByUnitUid(unit.UnitUid));
        }
        finally
        {
            if (File.Exists(dbPath)) File.Delete(dbPath);
        }

        Assert.Equal(413, NetError.ERR_BUY_CASH_ITEM_01);
        Assert.Equal(1149, NetError.ERR_BINGO_01);
        Assert.Equal(1364, NetError.ERR_ADMIN_COMMAND_10);
    }

    [Fact]
    public void GameCommon_Party_Pet_Inventory_BattleField_90Handler_Batch_Tests()
    {
        var region = RegionProfile.FromCode(RegionCode.TW);
        var ks = new KSerializer(region);

        void RoundTrip<T>(T src, Action<T> assertFn) where T : IKSerializable, new()
        {
            var buf = new KSerBuffer();
            ks.BeginWriting(buf);
            Assert.True(src.Put(ks, region));
            ks.EndWriting();
            var dst = new T();
            ks.BeginReading(buf);
            Assert.True(dst.Get(ks, region));
            ks.EndReading();
            assertFn(dst);
        }

        // Batch 2 packet serialization parity (Party legacy, SkillPage/Note, LocalRanking, PlayStatus, WatchUnit, Buffs)
        RoundTrip(new KEGS_CREATE_PARTY_REQ
        {
            PartyInfo = new KPartyInfo { PartyName = "HenirRun", MaxSlot = 4, DungeonId = 30100 },
        }, d => Assert.Equal("HenirRun", d.PartyInfo.PartyName));

        RoundTrip(new KEGS_PARTY_QUICK_JOIN_ACK
        {
            Ok = NetError.NET_OK,
            PartyUid = 777001,
        }, d => Assert.Equal(777001, d.PartyUid));

        RoundTrip(new KEGS_EXPAND_SKILL_PAGE_ACK
        {
            Ok = NetError.NET_OK,
            Ed = 500000,
            TheNumberOfSkillPagesAvailable = 2,
            SPointAvailable = 80,
            CsPointAvailable = 10,
            SkillListLearned = [1001, 1002],
            PayWithEd = true,
        }, d =>
        {
            Assert.Equal(2, d.TheNumberOfSkillPagesAvailable);
            Assert.Equal(2, d.SkillListLearned.Count);
        });

        RoundTrip(new KEGS_LOCAL_RANKING_USER_INFO_WRITE_REQ
        {
            Info = new KLocalRankingUserInfo
            {
                UserUid = 10,
                Category = 3,
                Birth = 19950101,
                Gender = 1,
                Profile = "Main Elsword",
            },
            Password = "pw",
        }, d => Assert.Equal("Main Elsword", d.Info.Profile));

        RoundTrip(new KEGS_UPDATE_PLAY_STATUS_NOT
        {
            GamePlayStatus = new KGamePlayStatus { CurHp = 9500, MaxHp = 10000, CurMp = 250, MaxMp = 300 },
        }, d => Assert.Equal(9500, d.GamePlayStatus.CurHp));

        RoundTrip(new KEGS_WATCH_UNIT_ACK
        {
            Ok = NetError.NET_OK,
            CUnitInfo = new KConnectionUnitInfo { UnitUid = 55, CharName = "KnightTW", Level = 70 },
        }, d => Assert.Equal("KnightTW", d.CUnitInfo.CharName));

        RoundTrip(new KEGS_USE_INTIMACY_UP_ITEM_ACK
        {
            Ok = NetError.NET_OK,
            UpPercent = 15.5f,
        }, d => Assert.Equal(15.5f, d.UpPercent));

        // Skill pages & skill note pages
        var skillTree = new KUserSkillTree();
        skillTree.SetAvailableSkillPoints(1, 40);
        Assert.True(skillTree.CanExpandSkillPage());
        var (expanded, pageCnt, pageData) = skillTree.ExpandSkillPage(70, 10);
        Assert.True(expanded);
        Assert.Equal(2, pageCnt);
        Assert.True(pageData.SkillPoint > 0);
        Assert.True(skillTree.SelectSkillPage(2, 70).ok);

        skillTree.SetSkillNoteState(hasNote: true, maxPages: 3, memoIds: new HashSet<int>());
        Assert.True(skillTree.IsExistSkillNotePage(2));
        skillTree.RegisterSkillNoteMemo(2, 910001);
        Assert.True(skillTree.IsExistSkillNoteMemoId(910001));

        // Pet intimacy item + Legacy party ops
        var petMgr = new KUserPetManager();
        petMgr.Init([new KPetInfo { PetUid = 9001, PetId = 1, PetName = "Phoru", Intimacy = 1000, Satiety = 2000 }]);
        petMgr.SummonPet(9001);
        var (intOk, _, upPct, updatedPet) = petMgr.UseIntimacyUpItem();
        Assert.True(intOk);
        Assert.True(upPct > 0f);
        Assert.True(updatedPet!.Intimacy > 1000);

        var partyMgr = new KCnPartyManager();
        var host = new KPartyUserInfo { UnitUid = 1, NickName = "Leader", Level = 70 };
        var party = partyMgr.CreateParty(new KPartyInfo { PartyName = "Legacy", MaxSlot = 4, Public = true, DungeonId = 30100 }, host);
        Assert.True(partyMgr.ChangePartyName(party.Info.PartyUid, 1, "UpdatedName"));
        Assert.True(partyMgr.ChangePartyPublic(party.Info.PartyUid, 1, false));
        Assert.Equal("UpdatedName", party.Info.PartyName);
        Assert.False(party.Info.Public);

        // 51-table SQLite Batch 2 methods
        string dbPath = Path.Combine(Path.GetTempPath(), $"knc_batch2_test_{Guid.NewGuid():N}.db");
        try
        {
            var db = new SqliteDatabaseManager(dbPath);
            db.InitializeAndSeed();
            var (_, acc1, _, _, _) = db.VerifyOrRegisterAccount("TW", "b2_u1", "pw");
            var (_, acc2, _, _, _) = db.VerifyOrRegisterAccount("TW", "b2_u2", "pw");
            var (cu1, unit1, _) = db.CreateUnitInServerGroup(region, acc1, "HeroB2", 1, 1);
            var (cu2, unit2, _) = db.CreateUnitInServerGroup(region, acc2, "PalB2", 2, 1);
            Assert.True(cu1 && cu2);

            var todayCounts = db.GetCreateUnitCountTodayByServerGroup(region, acc1);
            Assert.Equal(1, todayCounts[1]);
            Assert.Single(db.GetUnitsForAccountByServerGroup(region, acc1, 1));

            Assert.Equal(NetError.NET_OK, db.ChangeUnitNickName(region, acc1, unit1!.UnitUid, "HeroB2New", checkOnly: false));
            Assert.Equal("HeroB2New", db.GetUnitByUid(region, unit1.UnitUid)!.NickName);

            db.SetPlayGuideOption(acc1, true);
            db.SetChatOption(acc1, 2, true);
            db.SetKeyboardMapping(acc1, new Dictionary<short, short> { [1] = 65 }, new Dictionary<short, short>());

            db.SaveLocalRankingUserInfo(acc1, unit1.UnitUid, new KLocalRankingUserInfo { Category = 7, Birth = 19980101, Profile = "Ranker" }, 70, 50000);
            var (lr, _) = db.GetLocalRankingUserInfo(acc1);
            Assert.Equal("Ranker", lr.Profile);
            Assert.Single(db.QueryLocalRankings(region, 7, 10));

            Assert.True(db.CheckAttendanceToday(unit1.UnitUid));
            Assert.False(db.CheckAttendanceToday(unit1.UnitUid));
            db.SaveUnitSkillPageState(unit1.UnitUid, 2, 2, [1001]);
            db.SaveUserEventState(unit1.UnitUid, 40, 3, 123456, 1, 100, 1, [10, 20, 0, 0, 0, 0], "2026-01-01");
            Assert.Equal(NetError.NET_OK, db.RecommendUser(region, unit2!.UnitUid, "HeroB2New").ok);
            Assert.Single(db.GetRecommendedByUsers(region, unit1.UnitUid));
            db.RecordHackOrPortOrKomLog(acc1, unit1.UnitUid, "KomCheck", 0, 0, "data001.kom");

            var (restOk, restoreDate) = db.RestoreUnit(region, acc1, unit1.UnitUid);
            Assert.Equal(NetError.NET_OK, restOk);
            Assert.True(restoreDate > 0);
            Assert.Equal(NetError.NET_OK, db.FinalDeleteUnit(region, acc1, unit1.UnitUid).ok);
        }
        finally
        {
            if (File.Exists(dbPath)) File.Delete(dbPath);
        }

        Assert.Equal(986, NetError.ERR_DELETE_UNIT_00);
        Assert.Equal(1291, NetError.ERR_JUMPING_01);
        Assert.Equal(1388, NetError.ERR_SKILL_PAGE_03);
    }
}
