using KncWX2Server.Core.Database;
using KncWX2Server.Core.Framework;
using KncWX2Server.Core.Scripting;
using MoonSharp.Interpreter;

namespace KncWX2Server.GameServer;

/// <summary>
/// Faithful C# port of GameServer/UserAbuserManager.{h,cpp} (SERV_USER_ABUSER_MANAGER):
/// tracks account/character abuse flags (Item, Enchant, ED, Traffic, PacketMonitoring),
/// trade restrictions, rapid item/ED gain anomalies, and automatic audit logging.
/// </summary>
public sealed class KUserAbuserManager
{
    public enum AbuserEnum
    {
        AeEdAbuser = 0,
        AeItemAbuser = 1,
        AeTrafficAbuser = 2,
        AeEnchantAbuser = 3,
        AeMonitoringAbuser = 4,
    }

    public enum HackingUserCheckResult : byte
    {
        HucrNone = 0,
        HucrWait = 1,
        HucrBlock = 2,
    }

    public enum AccountAbuserType
    {
        AtMonitoringAbuser = 0,
        AtMax = 1,
    }

    public enum CharAbuserType
    {
        CatItemAbuser = 0,
        CatEnchantAbuser = 1,
        CatEdAbuser = 2,
        CatTrafficAbuser = 3,
        CatMax = 4,
    }

    public const int DefaultRapidItemThresholdPerSession = 200;
    public const int DefaultSingleEdGainThreshold = 50_000_000;

    public static string GlobalAbuserLogPath { get; private set; } = string.Empty;
    public static bool GlobalAbuserEventLogEnable { get; private set; }
    public static bool GlobalAbuserCheckLevelUp { get; private set; }
    public static int GlobalGetItemCountMin { get; private set; } = 100;
    public static int GlobalGetItemCountMax { get; private set; } = DefaultRapidItemThresholdPerSession;
    public static HashSet<string> GlobalJoinAccountCheckIps { get; } = new(StringComparer.OrdinalIgnoreCase);
    public static List<string> GlobalAbuserNotifyPhoneNums { get; } = new();
    public static Dictionary<int, (int checkCount, int blockNotifyType)> GlobalHackingUserCheckInfos { get; } = new();
    public static int GlobalDefaultNotifyType { get; private set; }

    private readonly bool[] _accountAbuser = new bool[(int)AccountAbuserType.AtMax];
    private readonly bool[] _charAbuser = new bool[(int)CharAbuserType.CatMax];
    private readonly Dictionary<ushort, int> _trafficEventCounts = new();
    private readonly Dictionary<byte, (byte count, DateTime resetUtc)> _hackingCheckCounts = new();
    private readonly object _sync = new();

    public bool IsTradeBlockUser { get; set; }
    public bool IsTradeBlockUnit { get; set; }
    public int GetItemCount { get; private set; }
    public int GetItemCountMin { get; set; } = GlobalGetItemCountMin;
    public int GetItemCountMax { get; set; } = GlobalGetItemCountMax;

    public bool IsMonitoringAbuser => _accountAbuser[(int)AccountAbuserType.AtMonitoringAbuser];
    public bool IsItemAbuser => _charAbuser[(int)CharAbuserType.CatItemAbuser];
    public bool IsEnchantAbuser => _charAbuser[(int)CharAbuserType.CatEnchantAbuser];
    public bool IsEdAbuser => _charAbuser[(int)CharAbuserType.CatEdAbuser];
    public bool IsTrafficAbuser => _charAbuser[(int)CharAbuserType.CatTrafficAbuser];

    public void InitAccountAbuser(bool monitoringAbuser, bool tradeBlockUser)
    {
        lock (_sync)
        {
            _accountAbuser[(int)AccountAbuserType.AtMonitoringAbuser] = monitoringAbuser;
            IsTradeBlockUser = tradeBlockUser;
        }
    }

    public void InitCharAbuser(bool itemAbuser, bool enchantAbuser, bool edAbuser, bool trafficAbuser, bool tradeBlockUnit)
    {
        lock (_sync)
        {
            _charAbuser[(int)CharAbuserType.CatItemAbuser] = itemAbuser;
            _charAbuser[(int)CharAbuserType.CatEnchantAbuser] = enchantAbuser;
            _charAbuser[(int)CharAbuserType.CatEdAbuser] = edAbuser;
            _charAbuser[(int)CharAbuserType.CatTrafficAbuser] = trafficAbuser;
            IsTradeBlockUnit = tradeBlockUnit;
        }
    }

    public void CheckMonitoringAbuser(SqliteDatabaseManager db, long accountUid, long unitUid, int ed, int spirit)
    {
        if (!IsMonitoringAbuser) return;
        db.RecordHackOrPortOrKomLog(accountUid, unitUid, $"MonitoringAbuser_ED={ed}_Spirit={spirit}", 1);
    }

    public bool CheckItemAbuser(SqliteDatabaseManager db, long accountUid, long unitUid, int authLevel, int itemId, int quantity)
    {
        if (quantity <= 0) return false;
        if (quantity >= DefaultRapidItemThresholdPerSession)
        {
            RegItemAbuser(db, accountUid, unitUid, authLevel);
            db.RecordHackOrPortOrKomLog(accountUid, unitUid, $"ItemAbuser_ItemID={itemId}_Qty={quantity}", quantity);
            return true;
        }
        return false;
    }

    public static bool IsAutoHackLevelUpUser(byte currentLevel, int beforeExp, int afterExp, int expectedMaxExpPerAction = 10_000_000)
    {
        if (!GlobalAbuserCheckLevelUp) return false;
        int gained = afterExp - beforeExp;
        return currentLevel < 70 && gained >= expectedMaxExpPerAction;
    }

    public void InitCheckCount(byte hackType, byte blockCheckCount, string lastResetDate)
    {
        var parsedDate = DateTime.TryParse(lastResetDate, out var dt) ? dt.ToUniversalTime() : DateTime.UtcNow;
        lock (_sync)
            _hackingCheckCounts[hackType] = (blockCheckCount, parsedDate);
    }

    public Dictionary<byte, (byte blockCheckCount, DateTime lastResetUtc)> GetDbUpdateInfo()
    {
        lock (_sync) return new Dictionary<byte, (byte, DateTime)>(_hackingCheckCounts);
    }

    public static bool CheckAbuserEvent(ushort eventId) => eventId > 0;

    public void Clear(bool onlyCharAbuser = false)
    {
        lock (_sync)
        {
            if (!onlyCharAbuser)
            {
                Array.Clear(_accountAbuser);
                IsTradeBlockUser = false;
                IsTradeBlockUnit = false;
                _hackingCheckCounts.Clear();
            }
            Array.Clear(_charAbuser);
            _trafficEventCounts.Clear();
            GetItemCount = 0;
        }
    }

    public void RegEdAbuser(SqliteDatabaseManager db, long accountUid, long unitUid, int authLevel)
    {
        if (authLevel > 2) return; // GM exempt
        lock (_sync)
        {
            if (_charAbuser[(int)CharAbuserType.CatEdAbuser]) return;
            _charAbuser[(int)CharAbuserType.CatEdAbuser] = true;
        }
        db.RecordHackOrPortOrKomLog(accountUid, unitUid, "AutoAbuser_ED", 1);
    }

    public void RegItemAbuser(SqliteDatabaseManager db, long accountUid, long unitUid, int authLevel)
    {
        if (authLevel > 2) return;
        lock (_sync)
        {
            if (_charAbuser[(int)CharAbuserType.CatItemAbuser]) return;
            _charAbuser[(int)CharAbuserType.CatItemAbuser] = true;
        }
        db.RecordHackOrPortOrKomLog(accountUid, unitUid, "AutoAbuser_Item", 1);
    }

    public void RegEnchantAbuser(SqliteDatabaseManager db, long accountUid, long unitUid, int authLevel)
    {
        if (authLevel > 2) return;
        lock (_sync)
        {
            if (_charAbuser[(int)CharAbuserType.CatEnchantAbuser]) return;
            _charAbuser[(int)CharAbuserType.CatEnchantAbuser] = true;
        }
        db.RecordHackOrPortOrKomLog(accountUid, unitUid, "AutoAbuser_Enchant", 1);
    }

    public void RegPacketMonitoring(SqliteDatabaseManager db, long accountUid, long unitUid, int authLevel)
    {
        if (authLevel > 2) return;
        lock (_sync)
        {
            if (_accountAbuser[(int)AccountAbuserType.AtMonitoringAbuser]) return;
            _accountAbuser[(int)AccountAbuserType.AtMonitoringAbuser] = true;
        }
        db.RecordHackOrPortOrKomLog(accountUid, unitUid, "AutoAbuser_Monitoring", 1);
    }

    public bool CheckEdDeltaAndFlag(
        SqliteDatabaseManager db,
        KGameSysValTable sysVal,
        long accountUid,
        long unitUid,
        int authLevel,
        int beforeEd,
        int afterEd)
    {
        int delta = afterEd - beforeEd;
        if (delta >= DefaultSingleEdGainThreshold)
        {
            RegEdAbuser(db, accountUid, unitUid, authLevel);
            if (sysVal.IsHackUserBlockTrade) IsTradeBlockUnit = true;
            return true;
        }
        return false;
    }

    public bool IncreaseGetItemCountAndCheck(
        SqliteDatabaseManager db,
        long accountUid,
        long unitUid,
        int authLevel,
        int deltaCount)
    {
        if (deltaCount <= 0) return false;
        bool exceeded;
        lock (_sync)
        {
            GetItemCount += deltaCount;
            exceeded = GetItemCount >= GetItemCountMax;
        }
        if (exceeded)
        {
            RegItemAbuser(db, accountUid, unitUid, authLevel);
            RegPacketMonitoring(db, accountUid, unitUid, authLevel);
        }
        return exceeded;
    }

    public int RecordTrafficEvent(ushort eventId, int warnThreshold = 500)
    {
        lock (_sync)
        {
            int next = _trafficEventCounts.GetValueOrDefault(eventId) + 1;
            _trafficEventCounts[eventId] = next;
            if (next >= warnThreshold)
                _charAbuser[(int)CharAbuserType.CatTrafficAbuser] = true;
            return next;
        }
    }

    public byte IncreaseHackingCheckCount(byte hackType, out bool shouldBlockAccount, DateTime? nowUtc = null)
    {
        var now = nowUtc ?? DateTime.UtcNow;
        lock (_sync)
        {
            if (!_hackingCheckCounts.TryGetValue(hackType, out var cur) || cur.resetUtc.Date != now.Date)
                cur = (0, now);

            byte next = (byte)Math.Min(255, cur.count + 1);
            _hackingCheckCounts[hackType] = (next, now);
            shouldBlockAccount = next >= 3;
            return next;
        }
    }

    public Dictionary<ushort, int> GetTrafficEventCountsSnapshot()
    {
        lock (_sync) return new Dictionary<ushort, int>(_trafficEventCounts);
    }

    public static void BindToLua(KScriptManager script, string globalName = "AbuserLogManager")
    {
        Table? proxy = null;
        proxy = script.CreateLuaProxy(globalName, t =>
        {
            t["SetAbuserLogPath"] = DynValue.NewCallback((_, a) =>
            {
                GlobalAbuserLogPath = KScriptManager.ArgString(a, KScriptManager.ArgStart(a, proxy!), GlobalAbuserLogPath);
                return DynValue.Nil;
            });
            t["SetAbuserEventLogEnable"] = DynValue.NewCallback((_, a) =>
            {
                GlobalAbuserEventLogEnable = KScriptManager.ArgBool(a, KScriptManager.ArgStart(a, proxy!), GlobalAbuserEventLogEnable);
                return DynValue.Nil;
            });
            t["SetAbuserCheckLevelUp"] = DynValue.NewCallback((_, a) =>
            {
                GlobalAbuserCheckLevelUp = KScriptManager.ArgBool(a, KScriptManager.ArgStart(a, proxy!), GlobalAbuserCheckLevelUp);
                return DynValue.Nil;
            });
            t["SetGetItemCountMinMax"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                GlobalGetItemCountMin = KScriptManager.ArgInt(a, o, GlobalGetItemCountMin);
                GlobalGetItemCountMax = KScriptManager.ArgInt(a, o + 1, GlobalGetItemCountMax);
                return DynValue.Nil;
            });
            t["AddJoinAccountCheckIP"] = DynValue.NewCallback((_, a) =>
            {
                string ip = KScriptManager.ArgString(a, KScriptManager.ArgStart(a, proxy!));
                if (!string.IsNullOrWhiteSpace(ip)) GlobalJoinAccountCheckIps.Add(ip);
                return DynValue.Nil;
            });
            t["AddAbuserNotifyPhoneNum"] = DynValue.NewCallback((_, a) =>
            {
                string num = KScriptManager.ArgString(a, KScriptManager.ArgStart(a, proxy!));
                if (!string.IsNullOrWhiteSpace(num) && !GlobalAbuserNotifyPhoneNums.Contains(num))
                    GlobalAbuserNotifyPhoneNums.Add(num);
                return DynValue.Nil;
            });
            t["SetHackingUserCheckDefaultNotify"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                if (KScriptManager.ArgTable(a, o) == null)
                    GlobalDefaultNotifyType = KScriptManager.ArgInt(a, o, GlobalDefaultNotifyType);
                return DynValue.Nil;
            });
            t["SetHackingUserCheckInfo"] = DynValue.NewCallback((_, a) =>
            {
                int o = KScriptManager.ArgStart(a, proxy!);
                var tbl = KScriptManager.ArgTable(a, o);
                if (tbl != null)
                {
                    int type = (int)(tbl.Get("byteHackingType").CastToNumber() ?? 0);
                    int cnt = (int)(tbl.Get("byteAccountBlock").CastToNumber() ?? 0);
                    int notify = (int)(tbl.Get("byteAccountBlockNotifyType").CastToNumber() ?? GlobalDefaultNotifyType);
                    if (type > 0)
                        GlobalHackingUserCheckInfos[type] = (cnt, notify);
                }
                else
                {
                    int type = KScriptManager.ArgInt(a, o);
                    int cnt = KScriptManager.ArgInt(a, o + 1);
                    int notify = KScriptManager.ArgInt(a, o + 2, GlobalDefaultNotifyType);
                    GlobalHackingUserCheckInfos[type] = (cnt, notify);
                }
                return DynValue.Nil;
            });
        });
    }
}
