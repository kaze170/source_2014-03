using System.Collections.Concurrent;
using KncWX2Server.Core.Scripting;
using MoonSharp.Interpreter;

namespace KncWX2Server.GameServer;

/// <summary>
/// Faithful C# port of Common/GameEvent/GameEventManager.{h,cpp} &amp; GameServer/UserEventManager.{h,cpp}:
/// server-wide timed event multipliers (EXP/ED/Drop), code-gated events, and per-user
/// connect-time &amp; cumulative-time reward tracking.
/// </summary>
public sealed class KGameEventManager
{
    public sealed class TimedGameEvent
    {
        public int EventUid { get; init; }
        public string EventName { get; init; } = string.Empty;
        public DateTime StartUtc { get; init; }
        public DateTime EndUtc { get; init; }
        public float ExpRate { get; init; } = 1.0f;
        public float EdRate { get; init; } = 1.0f;
        public float DropRate { get; init; } = 1.0f;
        public int ConnectMinutesRequired { get; init; }
        public int RewardItemId { get; init; }
        public int RewardQuantity { get; init; } = 1;
        public bool IsCumulative { get; init; }

        public bool IsActive(DateTime nowUtc) => nowUtc >= StartUtc && nowUtc <= EndUtc;
    }

    public sealed class UserEventTracker
    {
        public Dictionary<int, double> ConnectElapsedSeconds { get; } = new();
        public Dictionary<int, double> CumulativeElapsedSeconds { get; } = new();
        public HashSet<int> CompletedEventUids { get; } = [];
    }

    private readonly ConcurrentDictionary<int, TimedGameEvent> _events = new();
    private readonly ConcurrentDictionary<int, bool> _codeEvents = new();
    private readonly ConcurrentDictionary<long, UserEventTracker> _userTrackers = new();
    private readonly ConcurrentDictionary<int, List<KncWX2Server.Core.Packets.KPacketGetItemOnOff>> _timeControlItems = new();
    private readonly List<string> _advertisementUrls = new();

    public bool IsEnchantEventActive { get; set; }
    public bool IsCheckPowerEventEnabled { get; set; } = true;
    public bool IsOldYearEvent2013Enabled { get; set; } = true;
    public bool IsNewYearEvent2014Enabled { get; set; } = true;
    public bool IsChinaSpiritEventEnabled { get; set; } = true;
    public int JumpingEventTargetLevel { get; set; } = 60;
    public bool JumpingEventEnabled { get; set; } = true;
    public int ValentineDungeonPlayTimeEasy { get; private set; } = 300;
    public int ValentineDungeonPlayTimeNormal { get; private set; } = 240;
    public int ValentineDungeonPlayTimeHard { get; private set; } = 180;
    public int ValentineRewardItemId { get; private set; } = 141000972;

    public sealed record EventMonsterInfo(int EventId, int MonsterDropCount, bool IsAdventInBossStage, List<(int NpcId, int AdventRate)> AdventNpcs);
    public sealed record EventRewardInfo(int RewardId, float ExpRate, float VpRate, float EdRate, float EventTimeMinutes, int MinLevel, int MaxLevel, int DungeonId, int PartyNum, bool GiveOnce, bool AccountEvent);
    public sealed record EventBonusRate(float ExpRate, float VpRate, float EdRate, float DropRate);
    public sealed record EventPeriodRule(int EventUid, int TargetId, DateTime StartUtc, DateTime EndUtc, float Rate = 1.0f, int Count = 0, bool Flag = false);

    public Dictionary<int, EventMonsterInfo> EventMonsterInfos { get; } = new();
    public Dictionary<int, EventRewardInfo> EventRewards { get; } = new();
    public Dictionary<int, int> NotifyMsgServerInfos { get; } = new();

    private readonly ConcurrentDictionary<int, EventPeriodRule> _secretDungeonEvents = new();
    private readonly ConcurrentDictionary<int, EventPeriodRule> _spiritEvents = new();
    private readonly ConcurrentDictionary<int, EventPeriodRule> _tournamentEvents = new();
    private readonly ConcurrentDictionary<int, EventPeriodRule> _eventDungeons = new();
    private readonly ConcurrentDictionary<int, EventPeriodRule> _maxLevelEvents = new();
    private readonly ConcurrentDictionary<int, EventPeriodRule> _questItemDropEvents = new();
    private readonly ConcurrentDictionary<int, EventPeriodRule> _defenseDungeonOpenEvents = new();
    private readonly ConcurrentDictionary<int, EventPeriodRule> _henirRewardEvents = new();
    private readonly ConcurrentDictionary<int, EventPeriodRule> _monsterEvents = new();
    private readonly ConcurrentDictionary<int, bool> _eventQuestsEnabled = new();
    public int EventReleaseTick { get; set; }
    public int EventDbScriptReleaseTick { get; set; }
    public int AppliedEventTick { get; set; }

    public void BindToLua(KScriptManager script, string globalName = "GameEvent")
    {
        Table? proxy = null;
        proxy = script.CreateLuaProxy(globalName, t =>
        {
            t["AddReward"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                int key = KScriptManager.ArgInt(a, o);
                var tbl = KScriptManager.ArgTable(a, o + 1);
                if (tbl != null && key > 0)
                {
                    int rewardId = (int)(tbl.Get("iRewardID").CastToNumber() ?? key);
                    float expRate = (float)(tbl.Get("fEXPRate").CastToNumber() ?? 0);
                    float vpRate = (float)(tbl.Get("fVPRate").CastToNumber() ?? 0);
                    float edRate = (float)(tbl.Get("fEDRate").CastToNumber() ?? 0);
                    float eventTime = (float)(tbl.Get("fEventTime").CastToNumber() ?? 0);
                    int minLv = (int)(tbl.Get("iMinLevel").CastToNumber() ?? 0);
                    int maxLv = (int)(tbl.Get("iMaxLevel").CastToNumber() ?? 0);
                    int dgnId = (int)(tbl.Get("iDungeonID").CastToNumber() ?? 0);
                    int partyNum = (int)(tbl.Get("iPartyNum").CastToNumber() ?? 0);
                    bool giveOnce = tbl.Get("bGiveOnce").Type == DataType.Boolean && tbl.Get("bGiveOnce").Boolean;
                    bool accEvt = tbl.Get("bAccountEvent").Type == DataType.Boolean && tbl.Get("bAccountEvent").Boolean;
                    EventRewards[key] = new EventRewardInfo(rewardId, expRate, vpRate, edRate, eventTime, minLv, maxLv, dgnId, partyNum, giveOnce, accEvt);
                }
                return DynValue.Nil;
            });
            t["AddNotifyMSGServerInfo"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                int serverGroupId = KScriptManager.ArgInt(a, o);
                int serverUid = KScriptManager.ArgInt(a, o + 1);
                NotifyMsgServerInfos[serverGroupId] = serverUid;
                return DynValue.Nil;
            });
            t["AddValenTineEventData"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                ValentineDungeonPlayTimeEasy = KScriptManager.ArgInt(a, o, ValentineDungeonPlayTimeEasy);
                ValentineDungeonPlayTimeNormal = KScriptManager.ArgInt(a, o + 1, ValentineDungeonPlayTimeNormal);
                ValentineDungeonPlayTimeHard = KScriptManager.ArgInt(a, o + 2, ValentineDungeonPlayTimeHard);
                ValentineRewardItemId = KScriptManager.ArgInt(a, o + 4, ValentineRewardItemId);
                return DynValue.Nil;
            });
            t["AddEventMonsterInfo"] = DynValue.NewCallback((_, a) =>
            {
                var tbl = KScriptManager.ArgTable(a, KScriptManager.ArgStart(a, proxy!));
                if (tbl != null)
                {
                    int eventId = (int)(tbl.Get("m_iEventID").CastToNumber() ?? 0);
                    int dropCount = (int)(tbl.Get("m_iMonsterDropCount").CastToNumber() ?? 1);
                    var bossVal = tbl.Get("m_bIsAdventInBossStage");
                    bool inBoss = bossVal.Type == DataType.Boolean && bossVal.Boolean;
                    var npcs = new List<(int, int)>();
                    var npcTbl = tbl.Get("ADVENT_NPC_TABLE").Table;
                    if (npcTbl != null)
                    {
                        foreach (var pair in npcTbl.Pairs)
                        {
                            var row = pair.Value.Table;
                            if (row == null) continue;
                            int npcId = (int)(row.Get("NPC_ID").CastToNumber() ?? 0);
                            int rate = (int)(row.Get("ADVENT_RATE").CastToNumber() ?? 0);
                            if (npcId > 0) npcs.Add((npcId, rate));
                        }
                    }
                    if (eventId > 0)
                        EventMonsterInfos[eventId] = new EventMonsterInfo(eventId, dropCount, inBoss, npcs);
                }
                return DynValue.Nil;
            });
        });
    }

    public void AddTimeControlItem(int type, KncWX2Server.Core.Packets.KPacketGetItemOnOff entry)
    {
        var list = _timeControlItems.GetOrAdd(type, _ => new());
        lock (list) list.Add(entry);
    }

    public Dictionary<int, List<KncWX2Server.Core.Packets.KPacketGetItemOnOff>> GetTimeControlItemsSnapshot()
    {
        var snap = new Dictionary<int, List<KncWX2Server.Core.Packets.KPacketGetItemOnOff>>();
        foreach (var kv in _timeControlItems)
            lock (kv.Value) snap[kv.Key] = kv.Value.ToList();
        return snap;
    }

    public void AddAdvertisementUrl(string url)
    {
        if (!string.IsNullOrWhiteSpace(url))
            lock (_advertisementUrls) _advertisementUrls.Add(url);
    }

    public List<string> GetAdvertisementUrls()
    {
        lock (_advertisementUrls) return _advertisementUrls.ToList();
    }

    public void RegisterEvent(TimedGameEvent ev) => _events[ev.EventUid] = ev;

    public void RemoveEvent(int eventUid) => _events.TryRemove(eventUid, out _);

    public void SetCodeEvent(int codeId, bool enabled) => _codeEvents[codeId] = enabled;

    public bool IsEnableCode(int codeId) => _codeEvents.TryGetValue(codeId, out bool enabled) && enabled;

    public (float expRate, float edRate, float dropRate) GetActiveRates(DateTime? nowUtc = null)
    {
        var now = nowUtc ?? DateTime.UtcNow;
        float exp = 1.0f, ed = 1.0f, drop = 1.0f;
        foreach (var ev in _events.Values)
        {
            if (!ev.IsActive(now)) continue;
            exp = Math.Max(exp, ev.ExpRate);
            ed = Math.Max(ed, ev.EdRate);
            drop = Math.Max(drop, ev.DropRate);
        }
        return (exp, ed, drop);
    }

    public float GetActiveExpMultiplier() => GetActiveRates().expRate;

    public float GetActiveDropRateMultiplier() => GetActiveRates().dropRate;

    public List<TimedGameEvent> TickUserEvents(long unitUid, double deltaSeconds, DateTime? nowUtc = null)
    {
        var now = nowUtc ?? DateTime.UtcNow;
        var tracker = _userTrackers.GetOrAdd(unitUid, _ => new UserEventTracker());
        var newlyCompleted = new List<TimedGameEvent>();

        lock (tracker)
        {
            foreach (var ev in _events.Values)
            {
                if (!ev.IsActive(now) || ev.ConnectMinutesRequired <= 0) continue;
                if (tracker.CompletedEventUids.Contains(ev.EventUid)) continue;

                var map = ev.IsCumulative ? tracker.CumulativeElapsedSeconds : tracker.ConnectElapsedSeconds;
                map.TryGetValue(ev.EventUid, out double cur);
                cur += Math.Max(0, deltaSeconds);
                map[ev.EventUid] = cur;

                if (cur >= ev.ConnectMinutesRequired * 60.0)
                {
                    tracker.CompletedEventUids.Add(ev.EventUid);
                    newlyCompleted.Add(ev);
                }
            }
        }
        return newlyCompleted;
    }

    public void ResetConnectTimersOnDisconnect(long unitUid)
    {
        if (_userTrackers.TryGetValue(unitUid, out var tracker))
        {
            lock (tracker) tracker.ConnectElapsedSeconds.Clear();
        }
    }

    public EventBonusRate GetRewardEvent(int dungeonId, int unitLevel, int partyNum, bool isPcBang, DateTime? nowUtc = null)
    {
        var (exp, ed, drop) = GetActiveRates(nowUtc);
        float vp = 1.0f;
        foreach (var r in EventRewards.Values)
        {
            if (r.DungeonId > 0 && r.DungeonId != dungeonId) continue;
            if (r.MinLevel > 0 && unitLevel < r.MinLevel) continue;
            if (r.MaxLevel > 0 && unitLevel > r.MaxLevel) continue;
            if (r.PartyNum > 0 && partyNum < r.PartyNum) continue;
            exp = Math.Max(exp, Math.Max(1.0f, r.ExpRate));
            vp = Math.Max(vp, Math.Max(1.0f, r.VpRate));
            ed = Math.Max(ed, Math.Max(1.0f, r.EdRate));
        }
        if (isPcBang) exp *= 1.2f;
        return new EventBonusRate(exp, vp, ed, drop);
    }

    public bool CheckItemDropProbEvent(out float dropRate, DateTime? nowUtc = null)
    {
        dropRate = GetActiveRates(nowUtc).dropRate;
        return dropRate > 1.0f;
    }

    public bool CheckSpiritEvent(DateTime? nowUtc = null)
    {
        var now = nowUtc ?? DateTime.UtcNow;
        return _spiritEvents.Values.Any(e => now >= e.StartUtc && now <= e.EndUtc);
    }

    public bool CheckTournamentEvent(DateTime? nowUtc = null)
    {
        var now = nowUtc ?? DateTime.UtcNow;
        return _tournamentEvents.Values.Any(e => now >= e.StartUtc && now <= e.EndUtc);
    }

    public bool CheckSecretDungeonEvent(int dungeonId, DateTime? nowUtc = null)
    {
        var now = nowUtc ?? DateTime.UtcNow;
        return _secretDungeonEvents.Values.Any(e => now >= e.StartUtc && now <= e.EndUtc && (e.TargetId == 0 || e.TargetId == dungeonId));
    }

    public List<int> CheckEventMonster(DateTime? nowUtc = null)
    {
        var now = nowUtc ?? DateTime.UtcNow;
        return _monsterEvents.Values.Where(e => now >= e.StartUtc && now <= e.EndUtc).Select(e => e.TargetId > 0 ? e.TargetId : e.EventUid).ToList();
    }

    public int GetMaxLevel(int baseMaxLevel = 70, DateTime? nowUtc = null)
    {
        var now = nowUtc ?? DateTime.UtcNow;
        int maxLv = baseMaxLevel;
        foreach (var e in _maxLevelEvents.Values)
        {
            if (now >= e.StartUtc && now <= e.EndUtc && e.Count > maxLv)
                maxLv = e.Count;
        }
        return maxLv;
    }

    public bool IsEnableDungeon(int dungeonId, bool isEventDungeon, DateTime? nowUtc = null)
    {
        if (!isEventDungeon) return true;
        var now = nowUtc ?? DateTime.UtcNow;
        return _eventDungeons.Values.Any(e => now >= e.StartUtc && now <= e.EndUtc && (e.TargetId == 0 || e.TargetId == dungeonId));
    }

    public bool CheckQuestItemDropProbEvent(out float rate, DateTime? nowUtc = null)
    {
        var now = nowUtc ?? DateTime.UtcNow;
        rate = 1.0f;
        bool active = false;
        foreach (var e in _questItemDropEvents.Values)
        {
            if (now >= e.StartUtc && now <= e.EndUtc)
            {
                rate = Math.Max(rate, e.Rate);
                active = true;
            }
        }
        return active;
    }

    public bool GetDefenseDungeonOpenRate(out float rate, DateTime? nowUtc = null)
    {
        var now = nowUtc ?? DateTime.UtcNow;
        rate = 1.0f;
        bool active = false;
        foreach (var e in _defenseDungeonOpenEvents.Values)
        {
            if (now >= e.StartUtc && now <= e.EndUtc)
            {
                rate = Math.Max(rate, e.Rate);
                active = true;
            }
        }
        return active;
    }

    public bool GetHenirRewardCountEvent(out bool unlimited, out int eventMax, DateTime? nowUtc = null)
    {
        var now = nowUtc ?? DateTime.UtcNow;
        unlimited = false;
        eventMax = 0;
        bool active = false;
        foreach (var e in _henirRewardEvents.Values)
        {
            if (now >= e.StartUtc && now <= e.EndUtc)
            {
                unlimited |= e.Flag;
                eventMax = Math.Max(eventMax, e.Count);
                active = true;
            }
        }
        return active;
    }

    public bool IsEnableEventQuest(int questId) => _eventQuestsEnabled.GetValueOrDefault(questId, true);
    public void SetEventQuestEnabled(int questId, bool enabled) => _eventQuestsEnabled[questId] = enabled;

    public void AddSecretDungeonEvent(EventPeriodRule rule) => _secretDungeonEvents[rule.EventUid] = rule;
    public void AddSpiritEvent(EventPeriodRule rule) => _spiritEvents[rule.EventUid] = rule;
    public void AddTournamentEvent(EventPeriodRule rule) => _tournamentEvents[rule.EventUid] = rule;
    public void AddDungeonEvent(EventPeriodRule rule) => _eventDungeons[rule.EventUid] = rule;
    public void AddMaxLevelEvent(EventPeriodRule rule) => _maxLevelEvents[rule.EventUid] = rule;
    public void AddQuestItemDropEvent(EventPeriodRule rule) => _questItemDropEvents[rule.EventUid] = rule;
    public void AddDefenseDungeonOpenRateEvent(EventPeriodRule rule) => _defenseDungeonOpenEvents[rule.EventUid] = rule;
    public void AddHenirRewardCountEvent(EventPeriodRule rule) => _henirRewardEvents[rule.EventUid] = rule;
    public void AddMonsterEvent(EventPeriodRule rule) => _monsterEvents[rule.EventUid] = rule;
}
