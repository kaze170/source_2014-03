using System.Collections.Concurrent;
using KncWX2Server.Core.Framework;
using KncWX2Server.Core.Packets;
using KncWX2Server.Core.Region;

namespace KncWX2Server.CenterServer;

public enum RoomState : sbyte
{
    S_INIT = 0,
    S_WAIT = 1,
    S_LOADING = 2,
    S_PLAY = 3,
    S_RESULT = 4,
    S_CLOSE = 5,
    S_WAIT_FOR_DEFENCE = 6,
    S_RETURN_TO_FIELD = 7,
}

public sealed class KCnRoom
{
    public enum RoomType : sbyte
    {
        RtPvp = 0,
        RtDungeon = 1,
        RtTrainingCenter = 2,
        RtBattleField = 3,
        RtWeddingHall = 4,
        RtSquare = 5,
    }

    public sealed class SlotCombatStats
    {
        public long UnitUid { get; set; }
        public int TeamNum { get; set; }
        public int Level { get; set; } = 1;
        public int Rating { get; set; } = 1000;
        public int PvpPlayCount { get; set; }
        public bool WasWinBeforeMatch { get; set; }
        public float CurrentKFactor { get; set; } = 300.0f;
        public int Ed { get; set; }
        public int Exp { get; set; }
        public int KillNum { get; set; }
        public int MDKillNum { get; set; }
        public int DeathNum { get; set; }
        public int ComboScore { get; set; }
        public int TechnicalScore { get; set; }
        public int Damaged { get; set; }
        public int KillNpcNum { get; set; }
        public bool IsGameBang { get; set; }
        public int PcBangType { get; set; } = -1;
        public bool HasPet { get; set; }
        public int BaseHp { get; set; }
        public Dictionary<int, float> BonusRate { get; set; } = new();
    }

    private readonly object _lock = new();
    private readonly Dictionary<long, SlotCombatStats> _combatStats = new();
    private readonly HashSet<long> _stageLoadedUsers = new();
    private readonly HashSet<long> _subStageLoadedUsers = new();
    private readonly HashSet<long> _deadUnits = new();
    private int _subStageActiveNpcCount;
    private int _subStageKillActiveNpcCount;
    private DateTime _subStageFirstKillUtc;
    private readonly Dictionary<int, (int itemId, int quantity, int ed, sbyte enchantLevel)> _droppedItems = new();
    private int _nextDropItemUid = 1;
    private readonly Dictionary<long, int> _pvpNpcRebirthPos = new();
    private readonly Dictionary<long, float> _pvpNpcHpRatios = new();
    private readonly Dictionary<long, int> _unitRebirthPos = new();
    private readonly Dictionary<long, bool> _rematchVotes = new();
    private readonly HashSet<long> _resultSuccessUsers = new();
    private readonly Dictionary<long, KDungeonPlayResultInfo> _dungeonPlayResults = new();
    private readonly Dictionary<long, bool> _secretStageVotes = new();
    private readonly Dictionary<long, bool> _defenceDungeonVotes = new();
    private readonly HashSet<long> _stopContinueTimeUsers = new();
    private readonly Dictionary<int, List<KBuffSkillInfo>> _buffSkills = new();
    private readonly Dictionary<long, (bool forceHost, uint pingScore)> _pingScores = new();
    private readonly Dictionary<int, (int itemId, int posIndex, bool isClaimed)> _pvpDropItems = new();
    private readonly Dictionary<(long unitUid, int stageId, int subStageId), (int combo, int tech)> _badAttitudeSubStageScores = new();
    private readonly Dictionary<long, long> _bossFieldGivenDamage = new();
    private readonly Dictionary<long, long> _bossFieldAttackDamage = new();
    private readonly Dictionary<long, KSquareUserInfo> _squareUsers = new();
    private int _nextPvpDropUid = 1;
    private DateTime _playStartedUtc = DateTime.UtcNow;
    private DateTime _lastHostCheckUtc = DateTime.UtcNow;

    public bool PvpBaseDefence { get; private set; }
    public bool PvpBossCombat { get; private set; }
    public byte BossFieldStateFlags { get; set; } // 1=BossField, 2=IntrudeRestriction
    public bool TcRemainingTimeEnabled { get; private set; }
    public sbyte SquareType { get; init; }
    public KGameSysValTable? GameSysValTable { get; set; }
    public int CurrentStageId { get; set; }
    public sbyte CurrentSubStageId { get; set; }
    public KRoomMonsterManager MonsterManager { get; } = new();
    public KDungeonMonsterManager DungeonMonsterManager { get; }
    public KPvpMonsterManager PvpMonsterManager { get; }
    public KBadAttitudeManager BadAttitudeManager { get; } = new();
    public KDefenceDungeonManager DefenceDungeonManager { get; } = new();
    public KValentineManager ValentineManager { get; } = new();
    public KBattleFieldMonsterManager BattleFieldMonsterManager { get; } = new();
    public KBattleFieldGameManager BattleFieldGameManager { get; } = new();
    public KRoomInfo Info { get; }
    public List<KRoomSlotInfo> Slots { get; } = new();
    public List<KRoomSlotInfo> ObserverSlots { get; } = new();
    public Dictionary<long, int> LoadingProgress { get; } = new();

    public KCnRoom(long roomUid, uint roomListId, KRoomInfo template, string udpRelayIp, ushort udpRelayPort)
    {
        DungeonMonsterManager = new KDungeonMonsterManager(MonsterManager);
        PvpMonsterManager = new KPvpMonsterManager(MonsterManager);
        Info = new KRoomInfo
        {
            RoomType = template.RoomType,
            RoomUid = roomUid,
            RoomListId = roomListId,
            RoomName = string.IsNullOrWhiteSpace(template.RoomName) ? $"Room #{roomListId}" : template.RoomName,
            RoomState = (sbyte)RoomState.S_WAIT,
            Public = template.Public,
            TeamBalance = template.TeamBalance,
            Password = template.Password,
            MaxSlot = (sbyte)Math.Clamp((int)template.MaxSlot, 1, 8),
            JoinSlot = 0,
            UdpRelayIp = udpRelayIp,
            UdpRelayPort = udpRelayPort,
            StartedByAutoParty = template.StartedByAutoParty,
            PvpGameType = template.PvpGameType,
            IsItemMode = template.IsItemMode,
            PvpChannelClass = template.PvpChannelClass,
            WinKillNum = template.WinKillNum <= 0 ? (sbyte)3 : template.WinKillNum,
            CanIntrude = template.CanIntrude,
            PlayTime = template.PlayTime <= 0 ? 300f : template.PlayTime,
            WorldId = template.WorldId,
            ShowPvpMapWorldId = template.ShowPvpMapWorldId,
            DifficultyLevel = template.DifficultyLevel,
            DungeonId = template.DungeonId,
            GetItemType = template.GetItemType,
            DungeonMode = template.DungeonMode,
            PartyUid = template.PartyUid,
            BattleFieldId = template.BattleFieldId,
        };

        for (sbyte i = 0; i < Info.MaxSlot; i++)
        {
            Slots.Add(new KRoomSlotInfo
            {
                Index = i,
                SlotState = 1, // SS_EMPTY
                TeamNum = i % 2,
            });
        }
    }

    public (int ok, KRoomSlotInfo? joinedSlot) TryAddUser(KRoomUserInfo user, bool asObserver, string password)
    {
        lock (_lock)
        {
            if (!Info.Public && Info.Password != password && Slots.Any(s => s.SlotState == 2))
                return (NetError.ERR_ROOM_04, null);

            if (asObserver)
            {
                var obs = new KRoomSlotInfo
                {
                    Index = (sbyte)ObserverSlots.Count,
                    SlotState = 2,
                    IsHost = false,
                    RoomUserInfo = user,
                };
                user.IsObserver = true;
                ObserverSlots.Add(obs);
                return (NetError.NET_OK, obs);
            }

            var empty = Slots.FirstOrDefault(s => s.SlotState == 1);
            if (empty == null) return (NetError.ERR_ROOM_03, null);

            bool hasHost = Slots.Any(s => s.SlotState == 2 && s.IsHost);
            empty.SlotState = 2; // SS_JOIN
            empty.IsHost = !hasHost;
            empty.IsReady = empty.IsHost;
            empty.RoomUserInfo = user;
            Info.JoinSlot = (sbyte)Slots.Count(s => s.SlotState == 2);
            LoadingProgress[user.UnitUid] = 0;
            _combatStats[user.UnitUid] = new SlotCombatStats
            {
                UnitUid = user.UnitUid,
                TeamNum = empty.TeamNum,
                Level = Math.Max(1, (int)user.Level),
                Rating = user.Rating > 0 ? user.Rating : 1000,
                PvpPlayCount = user.OfficialMatchCnt,
                IsGameBang = user.IsGameBang,
                PcBangType = user.PcBangType,
                HasPet = user.Pet.Count > 0,
                BaseHp = user.GameStat.BaseHp,
                BonusRate = new Dictionary<int, float>(user.BonusRate),
            };
            return (NetError.NET_OK, empty);
        }
    }

    public (bool removed, bool roomEmpty, long newHostUid) RemoveUser(long unitUid)
    {
        lock (_lock)
        {
            int obsIdx = ObserverSlots.FindIndex(s => s.RoomUserInfo.UnitUid == unitUid);
            if (obsIdx >= 0)
            {
                ObserverSlots.RemoveAt(obsIdx);
                return (true, Info.JoinSlot == 0 && ObserverSlots.Count == 0, 0);
            }

            var slot = Slots.FirstOrDefault(s => s.SlotState == 2 && s.RoomUserInfo.UnitUid == unitUid);
            if (slot == null) return (false, false, 0);

            bool wasHost = slot.IsHost;
            slot.SlotState = 1;
            slot.IsHost = false;
            slot.IsReady = false;
            slot.RoomUserInfo = new KRoomUserInfo();
            LoadingProgress.Remove(unitUid);
            _combatStats.Remove(unitUid);
            BadAttitudeManager.RemoveUnit(unitUid, out _);

            Info.JoinSlot = (sbyte)Slots.Count(s => s.SlotState == 2);
            if (Info.JoinSlot == 0)
            {
                Info.RoomState = (sbyte)RoomState.S_CLOSE;
                return (true, true, 0);
            }

            long newHostUid = 0;
            if (wasHost)
            {
                var next = Slots.FirstOrDefault(s => s.SlotState == 2);
                if (next != null)
                {
                    next.IsHost = true;
                    next.IsReady = true;
                    newHostUid = next.RoomUserInfo.UnitUid;
                }
            }
            return (true, false, newHostUid);
        }
    }

    public bool SetReady(long unitUid, bool ready)
    {
        lock (_lock)
        {
            var slot = Slots.FirstOrDefault(s => s.SlotState == 2 && s.RoomUserInfo.UnitUid == unitUid);
            if (slot == null) return false;
            slot.IsReady = slot.IsHost || ready;
            return true;
        }
    }

    public (bool ok, sbyte slotIndex) SetTeam(long unitUid, int teamNum)
    {
        lock (_lock)
        {
            var slot = Slots.FirstOrDefault(s => s.SlotState == 2 && s.RoomUserInfo.UnitUid == unitUid);
            if (slot == null) return (false, -1);
            slot.TeamNum = teamNum;
            return (true, slot.Index);
        }
    }

    public bool StartLoading(short worldId, int dungeonId)
    {
        lock (_lock)
        {
            if (Info.JoinSlot <= 0) return false;
            if (worldId > 0) Info.WorldId = worldId;
            if (dungeonId > 0) Info.DungeonId = dungeonId;
            Info.RoomState = (sbyte)RoomState.S_LOADING;
            foreach (var s in Slots.Where(x => x.SlotState == 2))
                LoadingProgress[s.RoomUserInfo.UnitUid] = 0;
            return true;
        }
    }

    public bool UpdateLoading(long unitUid, int progress, out bool allLoaded)
    {
        lock (_lock)
        {
            allLoaded = false;
            if (!LoadingProgress.ContainsKey(unitUid)) return false;
            LoadingProgress[unitUid] = Math.Clamp(progress, 0, 100);
            allLoaded = LoadingProgress.Values.All(p => p >= 100);
            if (allLoaded)
            {
                StartGame();
            }
            return true;
        }
    }

    public void StartGame()
    {
        lock (_lock)
        {
            Info.RoomState = (sbyte)RoomState.S_PLAY;
            _playStartedUtc = DateTime.UtcNow;
            CurrentStageId = 0;
            CurrentSubStageId = 0;
            _stageLoadedUsers.Clear();
            _subStageLoadedUsers.Clear();
            _deadUnits.Clear();
            _subStageActiveNpcCount = 0;
            _subStageKillActiveNpcCount = 0;
            _subStageFirstKillUtc = DateTime.MinValue;
            _droppedItems.Clear();
            MonsterManager.StartGame();
            DungeonMonsterManager.StartGame();
            if (Info.RoomType == 3 && Info.BattleFieldId > 0) // RT_BATTLE_FIELD
            {
                BattleFieldMonsterManager.StartGame(Info.BattleFieldId, Math.Max(1, (int)Info.JoinSlot), 10, false);
                BattleFieldGameManager.StartGame();
            }
            BadAttitudeManager.Init(Slots.Where(s => s.SlotState == 2).Select(s => s.RoomUserInfo.UnitUid));
        }
    }

    public bool IsSecretStage { get; private set; }

    public void BeginStageLoad(int stageId)
    {
        lock (_lock)
        {
            CurrentStageId = stageId;
            CurrentSubStageId = 0;
            IsSecretStage = false;
            _stageLoadedUsers.Clear();
            _subStageLoadedUsers.Clear();
            _subStageActiveNpcCount = 0;
            _subStageKillActiveNpcCount = 0;
            _subStageFirstKillUtc = DateTime.MinValue;
        }
    }

    public bool MarkStageLoaded(long unitUid, out bool allLoaded, out sbyte slotIndex, bool isSecretStage = false)
    {
        lock (_lock)
        {
            var slot = Slots.FirstOrDefault(s => s.SlotState == 2 && s.RoomUserInfo.UnitUid == unitUid);
            slotIndex = slot?.Index ?? -1;
            if (slot == null) { allLoaded = false; return false; }
            if (isSecretStage) IsSecretStage = true;
            _stageLoadedUsers.Add(unitUid);
            int active = Slots.Count(s => s.SlotState == 2);
            allLoaded = active > 0 && _stageLoadedUsers.Count >= active;
            return true;
        }
    }

    public void BeginSubStageOpen(sbyte subStageId)
    {
        lock (_lock)
        {
            CurrentSubStageId = subStageId;
            _subStageLoadedUsers.Clear();
            _subStageActiveNpcCount = 0;
            _subStageKillActiveNpcCount = 0;
            _subStageFirstKillUtc = DateTime.MinValue;
        }
    }

    public bool MarkSubStageLoaded(long unitUid, out bool allLoaded)
    {
        lock (_lock)
        {
            _subStageLoadedUsers.Add(unitUid);
            int active = Slots.Count(s => s.SlotState == 2);
            allLoaded = active > 0 && _subStageLoadedUsers.Count >= active;
            return true;
        }
    }

    public int RegisterDropItem(int itemId, int quantity, int ed = 0, sbyte enchantLevel = 0)
    {
        lock (_lock)
        {
            int uid = _nextDropItemUid++;
            _droppedItems[uid] = (itemId, quantity, ed, enchantLevel);
            return uid;
        }
    }

    public bool TryClaimDropItem(int dropItemUid, out (int itemId, int quantity, int ed, sbyte enchantLevel) drop)
    {
        lock (_lock)
            return _droppedItems.Remove(dropItemUid, out drop);
    }

    public void RecordPvpKill(long killerUnitUid, long victimUnitUid, bool isMidKill = false)
    {
        lock (_lock)
        {
            if (_combatStats.TryGetValue(killerUnitUid, out var k))
            {
                if (isMidKill) k.MDKillNum++;
                else k.KillNum++;
            }
            if (_combatStats.TryGetValue(victimUnitUid, out var v))
                v.DeathNum++;
        }
    }

    public void RecordDungeonStats(long unitUid, int addCombo, int addTech, int addDamaged, int addNpcKills)
    {
        lock (_lock)
        {
            if (_combatStats.TryGetValue(unitUid, out var s))
            {
                s.ComboScore += Math.Max(0, addCombo);
                s.TechnicalScore += Math.Max(0, addTech);
                s.Damaged += Math.Max(0, addDamaged);
                s.KillNpcNum += Math.Max(0, addNpcKills);
            }
        }
    }

    public List<SlotCombatStats> SnapshotActiveCombatStats()
    {
        lock (_lock)
        {
            foreach (var s in Slots.Where(x => x.SlotState == 2))
            {
                if (_combatStats.TryGetValue(s.RoomUserInfo.UnitUid, out var st))
                    st.TeamNum = s.TeamNum;
            }
            return _combatStats.Values.ToList();
        }
    }

    public bool TryGetDungeonPlayResult(long unitUid, out KDungeonPlayResultInfo? info)
    {
        lock (_lock)
            return _dungeonPlayResults.TryGetValue(unitUid, out info);
    }

    public Dictionary<long, KDungeonPlayResultInfo> SnapshotDungeonPlayResults()
    {
        lock (_lock)
            return new Dictionary<long, KDungeonPlayResultInfo>(_dungeonPlayResults);
    }

    public KEGS_END_GAME_PVP_RESULT_DATA_NOT FinishPvpGame(bool abnormalEnd = false)
    {
        int sec = Math.Max(1, (int)(DateTime.UtcNow - _playStartedUtc).TotalSeconds);
        EndGame();
        return KResultProcess.BuildPvpResult(this, sec, abnormalEnd, GameSysValTable);
    }

    public KEGS_END_GAME_DUNGEON_RESULT_DATA_NOT FinishDungeonGame(bool isWin = true)
    {
        int sec = Math.Max(1, (int)(DateTime.UtcNow - _playStartedUtc).TotalSeconds);
        EndGame();
        return KResultProcess.BuildDungeonResult(this, sec, isWin, GameSysValTable);
    }

    public void EndGame()
    {
        lock (_lock)
        {
            Info.RoomState = (sbyte)RoomState.S_RESULT;
            MonsterManager.EndGame();
            DungeonMonsterManager.EndGame();
            PvpMonsterManager.EndGame();
            foreach (var s in Slots.Where(x => x.SlotState == 2))
                s.IsReady = s.IsHost;
        }
    }

    public List<long> GetMemberUnitUids()
    {
        lock (_lock)
        {
            return Slots.Where(s => s.SlotState == 2)
                .Select(s => s.RoomUserInfo.UnitUid)
                .Concat(ObserverSlots.Select(o => o.RoomUserInfo.UnitUid))
                .ToList();
        }
    }

    public List<KRoomSlotInfo> SnapshotSlots()
    {
        lock (_lock)
            return Slots.ToList();
    }

    public bool IsHost(long unitUid)
    {
        lock (_lock)
            return Slots.Any(s => s.SlotState == 2 && s.IsHost && s.RoomUserInfo.UnitUid == unitUid);
    }

    public bool BanUser(long requesterUnitUid, long targetUnitUid)
    {
        lock (_lock)
        {
            if (!IsHost(requesterUnitUid) || requesterUnitUid == targetUnitUid) return false;
            var (removed, _, _) = RemoveUser(targetUnitUid);
            return removed;
        }
    }

    public bool ChangeMap(long requesterUnitUid, short worldId)
    {
        lock (_lock)
        {
            if (!IsHost(requesterUnitUid)) return false;
            Info.WorldId = worldId;
            return true;
        }
    }

    public bool ChangePublic(long requesterUnitUid, bool isPublic, string password)
    {
        lock (_lock)
        {
            if (!IsHost(requesterUnitUid)) return false;
            Info.Public = isPublic;
            Info.Password = isPublic ? string.Empty : password;
            return true;
        }
    }

    public bool ChangeRoomName(long requesterUnitUid, string roomName)
    {
        lock (_lock)
        {
            if (!IsHost(requesterUnitUid)) return false;
            Info.RoomName = roomName;
            return true;
        }
    }

    public bool ChangePlayTime(long requesterUnitUid, float playTime)
    {
        lock (_lock)
        {
            if (!IsHost(requesterUnitUid)) return false;
            Info.PlayTime = playTime;
            return true;
        }
    }

    public bool SetPitIn(long unitUid, bool pitIn)
    {
        lock (_lock)
        {
            var slot = Slots.FirstOrDefault(s => s.SlotState == 2 && s.RoomUserInfo.UnitUid == unitUid);
            if (slot == null) return false;
            slot.IsPitIn = pitIn;
            return true;
        }
    }

    public List<int> GetNotExistNpcUids(IEnumerable<int> npcUids)
    {
        var missing = new List<int>();
        foreach (int uid in npcUids)
        {
            if (!MonsterManager.IsAliveMonster(uid))
                missing.Add(uid);
        }
        return missing;
    }

    public bool RecordForcedExitVote(long voterUnitUid, long targetUnitUid, bool wantForceExit)
    {
        lock (_lock)
        {
            if (voterUnitUid == targetUnitUid) return false;
            var target = Slots.FirstOrDefault(s => s.SlotState == 2 && s.RoomUserInfo.UnitUid == targetUnitUid);
            if (target == null) return false;
            if (wantForceExit)
                BadAttitudeManager.VoteForcedExit(voterUnitUid, targetUnitUid);
            return true;
        }
    }

    public bool UpdatePvpNpcRebirthPos(long pvpNpcUnitUid, int startPosIndex)
    {
        lock (_lock)
        {
            if (pvpNpcUnitUid <= 0) return false;
            _pvpNpcRebirthPos[pvpNpcUnitUid] = startPosIndex;
            return true;
        }
    }

    public bool UpdatePvpNpcInfo(IReadOnlyDictionary<long, float> pvpNpcInfo)
    {
        lock (_lock)
        {
            foreach (var kv in pvpNpcInfo)
                _pvpNpcHpRatios[kv.Key] = kv.Value;
            return true;
        }
    }

    public bool IsObserver(long unitUid)
    {
        lock (_lock) return ObserverSlots.Any(s => s.RoomUserInfo.UnitUid == unitUid);
    }

    public (int ok, sbyte teamBalanceSlotIndex) ChangeSlotOpen(long requesterUnitUid, sbyte slotIndex, sbyte slotState)
    {
        lock (_lock)
        {
            if (Info.RoomState != (sbyte)RoomState.S_WAIT) return (NetError.ERR_CLIENT_STATE, -1);
            if (!IsObserver(requesterUnitUid) && !IsHost(requesterUnitUid)) return (NetError.ERR_ROOM_17, -1);
            if (Info.PvpChannelClass == KPVPChannelInfo.PccTournament) return (NetError.ERR_SLOT_02, -1);
            if (slotIndex < 0 || slotIndex >= Slots.Count) return (NetError.ERR_ROOM_17, -1);

            var slot = Slots[slotIndex];
            sbyte tbIdx = -1;
            if (slotState == 1) // SS_EMPTY -> OpenSlot
            {
                if (slot.SlotState != 0) return (NetError.ERR_ROOM_17, -1);
                slot.SlotState = 1;
                if (Info.RoomType == 0 && (Info.PvpGameType == 1 || Info.PvpGameType == 2)) // PGT_TEAM / PGT_TEAM_DEATH
                {
                    int partnerIdx = slotIndex % 2 == 0 ? slotIndex + 1 : slotIndex - 1;
                    if (partnerIdx >= 0 && partnerIdx < Slots.Count && Slots[partnerIdx].SlotState == 0)
                    {
                        Slots[partnerIdx].SlotState = 1;
                        tbIdx = (sbyte)partnerIdx;
                    }
                }
            }
            else if (slotState == 0) // SS_CLOSE -> CloseSlot
            {
                if (slot.SlotState != 1) return (NetError.ERR_ROOM_17, -1);
                slot.SlotState = 0;
            }
            else
            {
                return (NetError.ERR_ROOM_17, -1);
            }
            return (NetError.NET_OK, tbIdx);
        }
    }

    public int ChangeWinningNumKill(long requesterUnitUid, int winKillNum)
    {
        lock (_lock)
        {
            if (Info.RoomState != (sbyte)RoomState.S_WAIT) return NetError.ERR_CLIENT_STATE;
            if (!IsObserver(requesterUnitUid) && !IsHost(requesterUnitUid)) return NetError.ERR_ROOM_17;
            if (winKillNum <= 0 || winKillNum > 100) return NetError.ERR_ROOM_47;
            Info.WinKillNum = (sbyte)winKillNum;
            return NetError.NET_OK;
        }
    }

    public int ChangePvpGameOption(long requesterUnitUid, bool baseDefence, bool bossCombat)
    {
        lock (_lock)
        {
            if (Info.RoomState != (sbyte)RoomState.S_WAIT) return NetError.ERR_CLIENT_STATE;
            if (!IsObserver(requesterUnitUid) && !IsHost(requesterUnitUid)) return NetError.ERR_ROOM_17;
            PvpBaseDefence = baseDefence;
            PvpBossCombat = bossCombat;
            return NetError.NET_OK;
        }
    }

    public int TogglePvpItemMode(out bool newItemMode)
    {
        lock (_lock)
        {
            newItemMode = Info.IsItemMode;
            if (Info.RoomState != (sbyte)RoomState.S_WAIT) return NetError.ERR_CLIENT_STATE;
            Info.IsItemMode = !Info.IsItemMode;
            newItemMode = Info.IsItemMode;
            return NetError.NET_OK;
        }
    }

    public int CreatePvpItem(int positionIndex, int itemId, out KDropItemData? dropData)
    {
        lock (_lock)
        {
            dropData = null;
            if (Info.RoomState != (sbyte)RoomState.S_PLAY) return NetError.ERR_CLIENT_STATE;
            if (!Info.IsItemMode) return NetError.ERR_DUNGEON_ITEM_00;
            if (_pvpDropItems.Values.Any(d => !d.isClaimed)) return NetError.ERR_DUNGEON_ITEM_01;
            if (itemId <= 0) return NetError.ERR_DUNGEON_ITEM_02;

            int dropUid = _nextPvpDropUid++;
            _pvpDropItems[dropUid] = (itemId, positionIndex, false);
            dropData = new KDropItemData
            {
                ItemId = itemId,
                DropItemUid = dropUid,
                Left = false,
                Speed = 1.0f,
                EnchantLevel = 0,
            };
            return NetError.NET_OK;
        }
    }

    public int ChangePvpItemPos(long itemUid, int pos)
    {
        lock (_lock)
        {
            if (Info.RoomState != (sbyte)RoomState.S_PLAY) return NetError.ERR_CLIENT_STATE;
            if (!_pvpDropItems.TryGetValue((int)itemUid, out var existing)) return NetError.ERR_ITEM_04;
            _pvpDropItems[(int)itemUid] = (existing.itemId, pos, existing.isClaimed);
            return NetError.NET_OK;
        }
    }

    public (int ok, KRoomSlotInfo? intruderSlot) CheckGameIntrude(long unitUid, bool isObserver)
    {
        lock (_lock)
        {
            if (Info.RoomState != (sbyte)RoomState.S_LOADING && Info.RoomState != (sbyte)RoomState.S_PLAY)
                return (NetError.ERR_CLIENT_STATE, null);

            if (!isObserver)
            {
                if (!Info.CanIntrude) return (NetError.ERR_ROOM_18, null);
                if (Info.PvpChannelClass == KPVPChannelInfo.PccTournament) return (NetError.ERR_ROOM_37, null);
                if (Info.PvpChannelClass == KPVPChannelInfo.PccOfficial) return (NetError.ERR_ROOM_53, null);
            }

            var slot = (isObserver ? ObserverSlots : Slots).FirstOrDefault(s => s.RoomUserInfo.UnitUid == unitUid);
            if (slot == null) return (NetError.ERR_ROOM_18, null);
            return (NetError.NET_OK, slot);
        }
    }

    public (int ok, KRoomSlotInfo? intruderSlot, bool isObserver) IntrudeStart(long unitUid)
    {
        lock (_lock)
        {
            bool isObserver = ObserverSlots.Any(s => s.RoomUserInfo.UnitUid == unitUid);
            if (Info.RoomState != (sbyte)RoomState.S_LOADING && Info.RoomState != (sbyte)RoomState.S_PLAY)
                return (NetError.ERR_CLIENT_STATE, null, isObserver);
            if (!isObserver && Info.PvpChannelClass == KPVPChannelInfo.PccOfficial)
                return (NetError.ERR_ROOM_53, null, isObserver);

            var slot = (isObserver ? ObserverSlots : Slots).FirstOrDefault(s => s.RoomUserInfo.UnitUid == unitUid);
            if (slot == null) return (NetError.ERR_ROOM_18, null, isObserver);
            slot.IsReady = true;
            return (NetError.NET_OK, slot, isObserver);
        }
    }

    public (int ok, bool returnedToWait) LeaveGame(long unitUid)
    {
        lock (_lock)
        {
            bool isObs = ObserverSlots.Any(s => s.RoomUserInfo.UnitUid == unitUid);
            if (!isObs &&
                Info.RoomState != (sbyte)RoomState.S_LOADING &&
                Info.RoomState != (sbyte)RoomState.S_PLAY &&
                Info.RoomState != (sbyte)RoomState.S_RESULT)
            {
                return (NetError.ERR_CLIENT_STATE, false);
            }

            var slot = (isObs ? ObserverSlots : Slots).FirstOrDefault(s => s.RoomUserInfo.UnitUid == unitUid);
            if (slot == null) return (NetError.ERR_ROOM_19, false);

            slot.IsReady = slot.IsHost;
            bool returnedToWait = false;
            if (Slots.Count(s => s.SlotState == 2 && s.IsReady) <= 1 && Info.RoomState == (sbyte)RoomState.S_PLAY)
            {
                Info.RoomState = (sbyte)RoomState.S_WAIT;
                returnedToWait = true;
            }
            return (NetError.NET_OK, returnedToWait);
        }
    }

    public (int ok, bool allAccepted) VoteRematch(long unitUid, bool acceptRematch)
    {
        lock (_lock)
        {
            if (Info.RoomState != (sbyte)RoomState.S_RESULT) return (NetError.ERR_CLIENT_STATE, false);
            var slot = Slots.FirstOrDefault(s => s.SlotState == 2 && s.RoomUserInfo.UnitUid == unitUid);
            if (slot == null) return (NetError.ERR_ROOM_17, false);
            _rematchVotes[unitUid] = acceptRematch;
            var activeUids = Slots.Where(s => s.SlotState == 2).Select(s => s.RoomUserInfo.UnitUid).ToList();
            bool allAccepted = activeUids.Count > 0 && activeUids.All(u => _rematchVotes.TryGetValue(u, out bool v) && v);
            if (allAccepted)
            {
                Info.RoomState = (sbyte)RoomState.S_WAIT;
                _rematchVotes.Clear();
                foreach (var s in Slots.Where(x => x.SlotState == 2)) s.IsReady = true;
            }
            return (NetError.NET_OK, allAccepted);
        }
    }

    public int StateChangeResult(long requesterUnitUid)
    {
        lock (_lock)
        {
            if (Info.RoomState != (sbyte)RoomState.S_RESULT) return NetError.ERR_CLIENT_STATE;
            if (!IsHost(requesterUnitUid)) return NetError.ERR_ROOM_17;
            return NetError.NET_OK;
        }
    }

    public int SetUnitRebirthPos(long unitUid, int startPosIndex)
    {
        lock (_lock)
        {
            if (Info.RoomState != (sbyte)RoomState.S_PLAY) return NetError.ERR_CLIENT_STATE;
            if (!Slots.Any(s => s.SlotState == 2 && s.RoomUserInfo.UnitUid == unitUid)) return NetError.ERR_ROOM_17;
            _unitRebirthPos[unitUid] = startPosIndex;
            return NetError.NET_OK;
        }
    }

    public (int ok, bool allSubmitted) SubmitDungeonPlayResult(long unitUid, KDungeonPlayResultInfo resultInfo)
    {
        lock (_lock)
        {
            if (Info.RoomState != (sbyte)RoomState.S_RESULT) return (NetError.ERR_CLIENT_STATE, false);
            if (!Slots.Any(s => s.SlotState == 2 && s.RoomUserInfo.UnitUid == unitUid)) return (NetError.ERR_ROOM_01, false);
            _dungeonPlayResults[unitUid] = resultInfo;
            var activeUids = Slots.Where(s => s.SlotState == 2).Select(s => s.RoomUserInfo.UnitUid).ToList();
            bool all = activeUids.Count > 0 && activeUids.All(u => _dungeonPlayResults.ContainsKey(u));
            return (NetError.NET_OK, all);
        }
    }

    public (int ok, bool allSuccess) ConfirmResultSuccess(long unitUid)
    {
        lock (_lock)
        {
            if (Info.RoomState != (sbyte)RoomState.S_RESULT) return (NetError.ERR_CLIENT_STATE, false);
            if (!Slots.Any(s => s.SlotState == 2 && s.RoomUserInfo.UnitUid == unitUid)) return (NetError.ERR_ROOM_17, false);
            _resultSuccessUsers.Add(unitUid);
            var activeUids = Slots.Where(s => s.SlotState == 2).Select(s => s.RoomUserInfo.UnitUid).ToList();
            bool all = activeUids.Count > 0 && activeUids.All(u => _resultSuccessUsers.Contains(u));
            if (all)
            {
                EndGame();
                _resultSuccessUsers.Clear();
                _dungeonPlayResults.Clear();
            }
            return (NetError.NET_OK, all);
        }
    }

    public int SetTcRemainingTime(bool enable)
    {
        lock (_lock)
        {
            if (Info.RoomState != (sbyte)RoomState.S_PLAY) return NetError.ERR_CLIENT_STATE;
            TcRemainingTimeEnabled = enable;
            return NetError.NET_OK;
        }
    }

    public (bool ok, KEGS_BUFF_SKILL_INFO_CONVEY_NOT not) ProcessBuffSkill(KEGS_BUFF_SKILL_INFO_CONVEY_REQ req)
    {
        lock (_lock)
        {
            var not = new KEGS_BUFF_SKILL_INFO_CONVEY_NOT();
            if (Info.RoomState != (sbyte)RoomState.S_PLAY) return (false, not);

            var list = new List<KBuffSkillInfo>();
            foreach (long caughtUid in req.CaughtUidList)
            {
                list.Add(new KBuffSkillInfo
                {
                    CaughtUnitUid = caughtUid,
                    BuffEnum = req.BuffEnum,
                    CastingUnitUid = req.CastingUnitUid,
                    IsBuffCasting = req.IsBuffCasting,
                    BuffLevel = req.BuffLevel,
                });
            }
            _buffSkills[req.BuffEnum] = list;
            foreach (var kv in _buffSkills)
                not.CaughtUidList[kv.Key] = new List<KBuffSkillInfo>(kv.Value);
            return (true, not);
        }
    }

    public void UpdatePingScore(long unitUid, bool forceHost, uint pingScore)
    {
        lock (_lock)
        {
            _pingScores[unitUid] = (forceHost, pingScore);
        }
    }

    /// <summary>
    /// Faithful port of KRoom::CheckPingScore (Room.cpp:1016) &amp; KRoomUserManager::ChooseBetterHost:
    /// evaluates ping scores on active non-dungeon rooms and elects a faster host when within MaxPingScore.
    /// </summary>
    public (bool changed, long oldHostUid, long newHostUid) CheckPingScore(DateTime? nowUtc = null)
    {
        var now = nowUtc ?? DateTime.UtcNow;
        lock (_lock)
        {
            if (Info.RoomState != (sbyte)RoomState.S_PLAY) return (false, 0, 0);
            if (Info.RoomType == 1) return (false, 0, 0); // SERV_DO_NOT_CHANGE_HOST_AT_DUNGEON

            float interval = GameSysValTable?.CheckChangeHostTimeSec ?? 10.0f;
            if ((now - _lastHostCheckUtc).TotalSeconds < Math.Max(1.0, interval))
                return (false, 0, 0);
            _lastHostCheckUtc = now;

            var active = Slots.Where(s => s.SlotState == 2 && s.RoomUserInfo.UnitUid > 0).ToList();
            if (active.Count <= 1) return (false, 0, 0);

            var oldHost = active.FirstOrDefault(s => s.IsHost);
            if (oldHost == null) return (false, 0, 0);

            long oldUid = oldHost.RoomUserInfo.UnitUid;
            uint oldPing = _pingScores.TryGetValue(oldUid, out var ohp) ? ohp.pingScore : 0u;
            uint maxPing = (uint)Math.Max(1, GameSysValTable?.MaxPingScore ?? 300);

            KRoomSlotInfo? bestCandidate = null;
            uint bestPing = uint.MaxValue;
            bool bestForced = false;
            foreach (var s in active)
            {
                long uid = s.RoomUserInfo.UnitUid;
                if (uid == oldUid) continue;
                var (forced, ping) = _pingScores.TryGetValue(uid, out var p) ? p : (false, 100u);
                if (forced && !bestForced)
                {
                    bestCandidate = s;
                    bestPing = ping;
                    bestForced = true;
                }
                else if (forced == bestForced && ping < bestPing)
                {
                    bestCandidate = s;
                    bestPing = ping;
                }
            }

            if (bestCandidate == null) return (false, 0, 0);
            if (!bestForced && bestPing >= maxPing && oldPing < maxPing)
                return (false, 0, 0);
            if (!bestForced && bestPing >= oldPing)
                return (false, 0, 0);

            oldHost.IsHost = false;
            bestCandidate.IsHost = true;
            return (true, oldUid, bestCandidate.RoomUserInfo.UnitUid);
        }
    }

    public (bool allVoted, bool enterSecretStage) VoteSecretStage(long unitUid, bool wantEnter)
    {
        lock (_lock)
        {
            if (Info.RoomState != (sbyte)RoomState.S_PLAY) return (false, false);
            _secretStageVotes[unitUid] = wantEnter;
            var activeUids = Slots.Where(s => s.SlotState == 2).Select(s => s.RoomUserInfo.UnitUid).ToList();
            bool all = activeUids.Count > 0 && activeUids.All(u => _secretStageVotes.ContainsKey(u));
            bool enter = all && activeUids.All(u => _secretStageVotes[u]);
            return (all, enter);
        }
    }

    public int SetStopContinueTime(long unitUid, bool isStop)
    {
        lock (_lock)
        {
            if (Info.RoomState != (sbyte)RoomState.S_PLAY) return NetError.ERR_CLIENT_STATE;
            if (isStop) _stopContinueTimeUsers.Add(unitUid);
            else _stopContinueTimeUsers.Remove(unitUid);
            return NetError.NET_OK;
        }
    }

    public (List<long> warningUsers, List<long> exitUsers) RecordBadAttitudeCheckInfo(
        long unitUid, int stageId, int subStageId, int comboScore, int techScore)
    {
        lock (_lock)
        {
            var warn = new List<long>();
            var exit = new List<long>();
            if (Info.RoomState != (sbyte)RoomState.S_PLAY) return (warn, exit);
            var activeUids = Slots.Where(s => s.SlotState == 2).Select(s => s.RoomUserInfo.UnitUid).ToList();
            if (activeUids.Count <= 1 || !Info.StartedByAutoParty) return (warn, exit);

            _badAttitudeSubStageScores[(unitUid, stageId, subStageId)] = (comboScore, techScore);
            if (activeUids.All(u => _badAttitudeSubStageScores.ContainsKey((u, stageId, subStageId))))
            {
                foreach (long u in activeUids)
                {
                    var (c, t) = _badAttitudeSubStageScores[(u, stageId, subStageId)];
                    if (c == 0 && t == 0)
                    {
                        BadAttitudeManager.IncreaseBadAttitudePoint(u);
                        warn.Add(u);
                    }
                }
            }
            return (warn, exit);
        }
    }

    public (bool allPrepared, List<long> leaveUnitUids) VoteEnterDefenceDungeon(long unitUid, bool agree)
    {
        lock (_lock)
        {
            _defenceDungeonVotes[unitUid] = agree;
            var activeUids = Slots.Where(s => s.SlotState == 2).Select(s => s.RoomUserInfo.UnitUid).ToList();
            bool all = activeUids.Count > 0 && activeUids.All(u => _defenceDungeonVotes.ContainsKey(u));
            var leaves = all ? activeUids.Where(u => !_defenceDungeonVotes[u]).ToList() : new List<long>();
            if (all) Info.RoomState = (sbyte)RoomState.S_PLAY;
            return (all, leaves);
        }
    }

    public int RestrictBossFieldIntrude(long requesterUnitUid)
    {
        lock (_lock)
        {
            if ((BossFieldStateFlags & 1) == 0) return NetError.ERR_FIELD_BOSS_03;
            if (!IsHost(requesterUnitUid)) return NetError.ERR_FIELD_BOSS_04;
            BossFieldStateFlags |= 2;
            Info.CanIntrude = false;
            return NetError.NET_OK;
        }
    }

    public void RecordBossFieldLog(IReadOnlyDictionary<long, long> givenDamage, IReadOnlyDictionary<long, long> attackDamage)
    {
        lock (_lock)
        {
            foreach (var kv in givenDamage) _bossFieldGivenDamage[kv.Key] = kv.Value;
            foreach (var kv in attackDamage) _bossFieldAttackDamage[kv.Key] = kv.Value;
        }
    }

    public void SetUserDieState(long unitUid, bool isDead)
    {
        lock (_lock)
        {
            if (isDead) _deadUnits.Add(unitUid);
            else _deadUnits.Remove(unitUid);
        }
    }

    public List<long> GetDeadUnitUids()
    {
        lock (_lock) return _deadUnits.ToList();
    }

    public void RecordSubStageActiveNpc(int count = 1)
    {
        if (count <= 0) return;
        lock (_lock) _subStageActiveNpcCount += count;
    }

    /// <summary>
    /// Exact C++ KDungeonRoom SubStageNpcDieCheck (DungeonRoom.cpp:6129):
    /// flags HCT_CHECK_NPC_DIE_TIME if a solo player clears >= 5 active NPCs in <= 1 second.
    /// </summary>
    public bool CheckSubStageNpcDieHack(bool isActiveNpc, DateTime? nowUtc = null)
    {
        if (!isActiveNpc) return false;
        lock (_lock)
        {
            if (GameSysValTable == null || !GameSysValTable.SubStageNpcDieCheck)
                return false;

            int activeMembers = Slots.Count(s => s.SlotState == 2);
            if (activeMembers != 1 || _subStageActiveNpcCount < 5)
                return false;

            _subStageKillActiveNpcCount++;
            var now = nowUtc ?? DateTime.UtcNow;
            if (_subStageKillActiveNpcCount == 1)
            {
                _subStageFirstKillUtc = now;
                return false;
            }

            if (_subStageActiveNpcCount <= _subStageKillActiveNpcCount &&
                (now - _subStageFirstKillUtc).TotalSeconds <= 1.0)
            {
                return true;
            }
            return false;
        }
    }

    // ---- Square Room Helpers ----

    public KSquareInfo GetSquareInfoSnapshot()
    {
        lock (_lock)
        {
            return new KSquareInfo
            {
                SquareType = SquareType,
                SquareUid = Info.RoomUid,
                RoomName = Info.RoomName,
                MaxSlot = Info.MaxSlot,
                JoinSlot = (sbyte)_squareUsers.Count,
                RelayIp = Info.UdpRelayIp,
                Port = Info.UdpRelayPort,
            };
        }
    }

    public (int ok, List<KSquareUserInfo> users) TryJoinSquare(KSquareUserInfo user)
    {
        lock (_lock)
        {
            if (_squareUsers.Count >= Info.MaxSlot && !_squareUsers.ContainsKey(user.UnitUid))
                return (NetError.ERR_ROOM_18, new List<KSquareUserInfo>());
            _squareUsers[user.UnitUid] = user;
            Info.JoinSlot = (sbyte)_squareUsers.Count;
            return (NetError.NET_OK, _squareUsers.Values.ToList());
        }
    }

    public bool LeaveSquare(long unitUid)
    {
        lock (_lock)
        {
            bool removed = _squareUsers.Remove(unitUid);
            Info.JoinSlot = (sbyte)_squareUsers.Count;
            return removed;
        }
    }

    public List<long> GetSquareUserUids()
    {
        lock (_lock) return _squareUsers.Keys.ToList();
    }

    public bool CheckDungeonKillAllNpc(long unitUid)
    {
        lock (_lock)
        {
            if (Info.RoomType is not (1 or 2) || Info.RoomState != 4) return false;
            var host = Slots.FirstOrDefault(s => s.SlotState == 2 && s.IsHost);
            if (host != null && host.RoomUserInfo.UnitUid != unitUid) return false;
            return true;
        }
    }
}

public sealed class KCnRoomManager
{
    private readonly ConcurrentDictionary<long, KCnRoom> _rooms = new();
    private long _nextRoomUid = 100_000;
    private int _nextRoomListId = 1;
    private readonly List<KPVPChannelInfo> _pvpChannels =
    [
        new KPVPChannelInfo { ChannelId = 1, PvpChannelClass = KPVPChannelInfo.PccRecruit, MinRating = 0, MaxRating = 1200 },
        new KPVPChannelInfo { ChannelId = 2, PvpChannelClass = KPVPChannelInfo.PccFree, MinRating = 0, MaxRating = 9999 },
        new KPVPChannelInfo { ChannelId = 3, PvpChannelClass = KPVPChannelInfo.PccOfficial, MinRating = 0, MaxRating = 9999 },
    ];

    public IReadOnlyList<KPVPChannelInfo> PvpChannels => _pvpChannels;
    public string DefaultUdpRelayIp { get; set; } = "127.0.0.1";
    public ushort DefaultUdpRelayPort { get; set; } = 9303;
    public KGameSysValTable? GameSysValTable { get; set; }
    public KDefenceDungeonManager? DefenceDungeonManagerTemplate { get; set; }

    public int RoomCount => _rooms.Count;

    public int NumPvPRoom { get; private set; } = 1500;
    public int NumDungeonRoom { get; private set; } = 1500;
    public int NumSquare { get; private set; } = 10;
    public int NumTrade { get; private set; } = 300;
    public int NumPersonalShop { get; private set; } = 150;
    public int NumTrainingCenterRoom { get; private set; } = 1500;
    public int NumBattleField { get; private set; } = 1500;
    public int NumArcadeRoom { get; private set; } = 300;
    public int NumParty { get; private set; } = 3000;
    public int NumWeddingHall { get; private set; } = 100;
    public double ZUTickTime { get; private set; } = 60.0;
    public double BattleFieldZUTickTime { get; private set; } = 20.0;

    public void SetNumPvPRoom(int n) => NumPvPRoom = Math.Max(0, n);
    public void SetNumDungeonRoom(int n) => NumDungeonRoom = Math.Max(0, n);
    public void SetNumSquare(int n) => NumSquare = Math.Max(0, n);
    public void SetNumTrade(int n) => NumTrade = Math.Max(0, n);
    public void SetNumPersonalShop(int n) => NumPersonalShop = Math.Max(0, n);
    public void SetNumTrainingCenterRoom(int n) => NumTrainingCenterRoom = Math.Max(0, n);
    public void SetNumBattleField(int n) => NumBattleField = Math.Max(0, n);
    public void SetNumArcadeRoom(int n) => NumArcadeRoom = Math.Max(0, n);
    public void SetNumParty(int n) => NumParty = Math.Max(0, n);
    public void SetNumWeddingHall(int n) => NumWeddingHall = Math.Max(0, n);
    public void SetZUTickTime(double sec) => ZUTickTime = Math.Max(1.0, sec);
    public void SetBattleFieldZUTickTime(double sec) => BattleFieldZUTickTime = Math.Max(1.0, sec);

    public KCnRoom CreateRoom(KRoomInfo template)
    {
        long ruid = Interlocked.Increment(ref _nextRoomUid);
        uint rlid = (uint)Interlocked.Increment(ref _nextRoomListId);
        var room = new KCnRoom(ruid, rlid, template, DefaultUdpRelayIp, DefaultUdpRelayPort)
        {
            GameSysValTable = GameSysValTable,
        };
        if (DefenceDungeonManagerTemplate != null)
            room.DefenceDungeonManager.CopyConfigFrom(DefenceDungeonManagerTemplate);
        _rooms[ruid] = room;
        return room;
    }

    public KCnRoom CreateRoom(KRoomInfo template, KRoomUserInfo hostUser, bool asObserver = false)
    {
        long ruid = Interlocked.Increment(ref _nextRoomUid);
        uint rlid = (uint)Interlocked.Increment(ref _nextRoomListId);
        var room = new KCnRoom(ruid, rlid, template, DefaultUdpRelayIp, DefaultUdpRelayPort)
        {
            GameSysValTable = GameSysValTable,
        };
        if (DefenceDungeonManagerTemplate != null)
            room.DefenceDungeonManager.CopyConfigFrom(DefenceDungeonManagerTemplate);
        room.TryAddUser(hostUser, asObserver, template.Password);
        _rooms[ruid] = room;
        return room;
    }

    public (int ok, KCnRoom? room) JoinRoom(long roomUid, KRoomUserInfo user, string password, bool asObserver)
    {
        if (!_rooms.TryGetValue(roomUid, out var room))
            return (NetError.ERR_ROOM_16, null);
        var (ok, _) = room.TryAddUser(user, asObserver, password);
        return (ok, ok == NetError.NET_OK ? room : null);
    }

    public KCnRoom? GetRoom(long roomUid) => _rooms.TryGetValue(roomUid, out var r) ? r : null;

    public KCnRoom GetOrCreateBattleFieldRoom(int battleFieldId, int worldId = 0)
    {
        var existing = _rooms.Values.FirstOrDefault(r =>
            r.Info.RoomType == 3 &&
            r.Info.BattleFieldId == battleFieldId &&
            r.Info.JoinSlot < r.Info.MaxSlot);
        if (existing != null) return existing;

        var room = CreateRoom(new KRoomInfo
        {
            RoomType = 3, // RT_BATTLE_FIELD
            RoomName = $"BattleField {battleFieldId}",
            Public = true,
            MaxSlot = 8,
            BattleFieldId = battleFieldId,
            WorldId = (short)(worldId > 0 ? worldId : battleFieldId),
        });
        room.StartGame();
        return room;
    }

    public (bool removed, long newHostUid) LeaveRoom(long roomUid, long unitUid)
    {
        if (!_rooms.TryGetValue(roomUid, out var room))
            return (false, 0);
        if (room.Info.RoomType == 5)
            return (room.LeaveSquare(unitUid), 0);
        var (removed, empty, newHostUid) = room.RemoveUser(unitUid);
        if (empty)
            _rooms.TryRemove(roomUid, out _);
        return (removed, newHostUid);
    }

    public List<KRoomInfo> GetRoomList(int viewPage = 1, int pageSize = 10)
    {
        var all = _rooms.Values.Select(r => r.Info).OrderBy(r => r.RoomListId).ToList();
        int skip = Math.Max(0, viewPage - 1) * pageSize;
        return all.Skip(skip).Take(pageSize).ToList();
    }

    public long GetWaitingRoomUid(uint roomListId, int option, bool isItemMode)
    {
        var match = _rooms.Values.FirstOrDefault(r =>
            r.Info.RoomType == 0 && // RT_PVP
            r.Info.RoomState == (sbyte)RoomState.S_WAIT &&
            r.Info.Public &&
            r.Info.JoinSlot < r.Info.MaxSlot &&
            (roomListId == 0 || r.Info.RoomListId == roomListId || r.Info.PvpChannelClass != KPVPChannelInfo.PccOfficial) &&
            (option <= 0 || r.Info.PvpGameType == option) &&
            (!isItemMode || r.Info.IsItemMode));
        return match?.Info.RoomUid ?? 0;
    }

    public void EnsureDefaultSquares()
    {
        if (_rooms.Values.Any(r => r.Info.RoomType == 5)) return; // RT_SQUARE = 5
        for (sbyte sqType = 0; sqType <= 1; sqType++)
        {
            long ruid = Interlocked.Increment(ref _nextRoomUid);
            uint rlid = (uint)Interlocked.Increment(ref _nextRoomListId);
            var square = new KCnRoom(ruid, rlid, new KRoomInfo
            {
                RoomType = 5, // RT_SQUARE
                RoomName = sqType == 0 ? "Trade Square #1" : "Event Square #1",
                Public = true,
                MaxSlot = 32,
            }, DefaultUdpRelayIp, DefaultUdpRelayPort)
            {
                SquareType = sqType,
            };
            _rooms[ruid] = square;
        }
    }

    public (List<KSquareInfo> items, uint totalPage, uint viewPage) GetSquareInfoPage(sbyte squareType, uint viewPage, int pageSize = 8)
    {
        EnsureDefaultSquares();
        var all = _rooms.Values
            .Where(r => r.Info.RoomType == 5 && (squareType < 0 || r.SquareType == squareType))
            .OrderBy(r => r.Info.RoomUid)
            .Select(r => r.GetSquareInfoSnapshot())
            .ToList();
        uint totalPage = (uint)Math.Max(1, (all.Count + pageSize - 1) / pageSize);
        uint clampedPage = Math.Clamp(viewPage == 0 ? 1u : viewPage, 1u, totalPage);
        var slice = all.Skip((int)((clampedPage - 1) * pageSize)).Take(pageSize).ToList();
        return (slice, totalPage, clampedPage);
    }

    public (int ok, KSquareInfo? info, List<KSquareUserInfo> users, KCnRoom? room) JoinSquare(long squareUid, KSquareUserInfo user)
    {
        EnsureDefaultSquares();
        if (!_rooms.TryGetValue(squareUid, out var room) || room.Info.RoomType != 5)
            return (NetError.ERR_ROOM_16, null, new List<KSquareUserInfo>(), null);
        var (ok, users) = room.TryJoinSquare(user);
        return (ok, ok == NetError.NET_OK ? room.GetSquareInfoSnapshot() : null, users, ok == NetError.NET_OK ? room : null);
    }

    public (bool removed, KCnRoom? room) LeaveSquare(long squareUid, long unitUid)
    {
        if (!_rooms.TryGetValue(squareUid, out var room) || room.Info.RoomType != 5)
            return (false, null);
        bool removed = room.LeaveSquare(unitUid);
        return (removed, room);
    }

    public bool SendBattleFieldSyncComplete(long roomUid, long unitUid)
    {
        if (!_rooms.TryGetValue(roomUid, out var room)) return false;
        room.MarkStageLoaded(unitUid, out _, out _);
        return true;
    }

    public IReadOnlyCollection<KCnRoom> GetRoomsSnapshot() => _rooms.Values.ToArray();

    public KCnRoom? GetRoomByUnitUid(long unitUid)
        => unitUid <= 0 ? null : _rooms.Values.FirstOrDefault(r => r.GetMemberUnitUids().Contains(unitUid));

    public KCnRoom? ResolveRoom(long explicitRoomUid, long unitUid)
        => GetRoom(explicitRoomUid) ?? GetRoomByUnitUid(unitUid);

    public bool UpdateRoomUserInfo(KCnRoom room, KRoomUserInfo userInfo)
    {
        var slot = room.Slots.FirstOrDefault(s => s.RoomUserInfo.UnitUid == userInfo.UnitUid)
                   ?? room.ObserverSlots.FirstOrDefault(s => s.RoomUserInfo.UnitUid == userInfo.UnitUid);
        if (slot == null) return false;
        slot.RoomUserInfo = userInfo;
        return true;
    }

    public bool SetUserTitle(KCnRoom room, long unitUid, int titleId)
    {
        var slot = room.Slots.FirstOrDefault(s => s.RoomUserInfo.UnitUid == unitUid);
        if (slot == null) return false;
        slot.RoomUserInfo.TitleId = titleId;
        return true;
    }

    public bool SetUserBuffs(KCnRoom room, long unitUid, List<KBuffInfo> buffs)
    {
        var slot = room.Slots.FirstOrDefault(s => s.RoomUserInfo.UnitUid == unitUid);
        if (slot == null) return false;
        slot.RoomUserInfo.BuffInfo = buffs;
        return true;
    }

    public bool SetUserCashShop(KCnRoom room, long unitUid, bool enter)
    {
        var slot = room.Slots.FirstOrDefault(s => s.RoomUserInfo.UnitUid == unitUid);
        if (slot == null) return false;
        slot.RoomUserInfo.EnterCashShop = enter;
        return true;
    }

    public List<(long roomUid, List<long> memberUids, long oldHostUid, long newHostUid)> TickRooms(DateTime? nowUtc = null)
    {
        var now = nowUtc ?? DateTime.UtcNow;
        var hostChanges = new List<(long, List<long>, long, long)>();
        foreach (var room in _rooms.Values)
        {
            var (changed, oldHostUid, newHostUid) = room.CheckPingScore(now);
            if (changed)
                hostChanges.Add((room.Info.RoomUid, room.GetMemberUnitUids(), oldHostUid, newHostUid));
        }
        return hostChanges;
    }
}
