using System.Security.Cryptography;
using System.Text;
using KncWX2Server.Core.Framework;
using KncWX2Server.Core.Logging;
using KncWX2Server.Core.Packets;
using KncWX2Server.Core.Region;
using Microsoft.Data.Sqlite;

namespace KncWX2Server.Core.Database;

/// <summary>
/// Self-implemented SQLite persistence layer replacing MSSQL/ODBC stored procedures for
/// Account, Log/Statistics, and SMS databases across TW, HK, CN, and JP regions.
/// </summary>
public sealed class SqliteDatabaseManager
{
    public string DatabasePath { get; }
    public KExpTable ExpTable { get; } = new();
    private readonly string _connectionString;
    private readonly object _writeLock = new();

    public SqliteDatabaseManager(string databasePath = "KncWX2Server.db")
    {
        DatabasePath = Path.GetFullPath(databasePath);
        Directory.CreateDirectory(Path.GetDirectoryName(DatabasePath)!);
        _connectionString = new SqliteConnectionStringBuilder
        {
            DataSource = DatabasePath,
            Mode = SqliteOpenMode.ReadWriteCreate,
            Cache = SqliteCacheMode.Shared,
        }.ToString();
    }

    public SqliteConnection OpenConnection()
    {
        var conn = new SqliteConnection(_connectionString);
        conn.Open();
        using var pragma = conn.CreateCommand();
        pragma.CommandText = "PRAGMA journal_mode=WAL; PRAGMA synchronous=NORMAL;";
        pragma.ExecuteNonQuery();
        return conn;
    }

    public void InitializeAndSeed()
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                CREATE TABLE IF NOT EXISTS ServerGroups (
                    RegionCode TEXT NOT NULL,
                    ServerGroupID INTEGER NOT NULL,
                    ServerGroupName TEXT NOT NULL,
                    FirstServerIP TEXT NOT NULL,
                    FirstServerPort INTEGER NOT NULL,
                    OrderIndex INTEGER NOT NULL DEFAULT 0,
                    IsEnable INTEGER NOT NULL DEFAULT 1,
                    PRIMARY KEY (RegionCode, ServerGroupID)
                );

                CREATE TABLE IF NOT EXISTS ServerList (
                    RegionCode TEXT NOT NULL,
                    ServerUID INTEGER NOT NULL,
                    ServerName TEXT NOT NULL,
                    ServerGroupID INTEGER NOT NULL,
                    ServerClass INTEGER NOT NULL,
                    PublicIP TEXT NOT NULL,
                    PrivateIP TEXT NOT NULL,
                    MasterPort INTEGER NOT NULL,
                    ClientPort INTEGER NOT NULL,
                    MaxUser INTEGER NOT NULL DEFAULT 3000,
                    IsEnabled INTEGER NOT NULL DEFAULT 1,
                    PRIMARY KEY (RegionCode, ServerUID)
                );

                CREATE TABLE IF NOT EXISTS ChannelList (
                    RegionCode TEXT NOT NULL,
                    ChannelID INTEGER NOT NULL,
                    ServerGroupID INTEGER NOT NULL,
                    ServerUID INTEGER NOT NULL,
                    ChannelName TEXT NOT NULL,
                    MaxUser INTEGER NOT NULL DEFAULT 1000,
                    CurrentUser INTEGER NOT NULL DEFAULT 0,
                    IP TEXT NOT NULL,
                    Port INTEGER NOT NULL,
                    PRIMARY KEY (RegionCode, ChannelID)
                );

                CREATE TABLE IF NOT EXISTS Accounts (
                    UserUID INTEGER PRIMARY KEY AUTOINCREMENT,
                    RegionCode TEXT NOT NULL,
                    UserID TEXT NOT NULL,
                    PasswordHash TEXT NOT NULL,
                    AuthLevel INTEGER NOT NULL DEFAULT 0,
                    IsBlocked INTEGER NOT NULL DEFAULT 0,
                    BlockReason TEXT NOT NULL DEFAULT '',
                    IsGuestUser INTEGER NOT NULL DEFAULT 0,
                    RandomKey TEXT NOT NULL DEFAULT '',
                    LastServerGroupID INTEGER NOT NULL DEFAULT 0,
                    CreatedAt TEXT NOT NULL DEFAULT (datetime('now')),
                    UNIQUE (RegionCode, UserID)
                );

                CREATE TABLE IF NOT EXISTS MachineBlocks (
                    MachineID TEXT PRIMARY KEY,
                    Reason TEXT NOT NULL DEFAULT '',
                    CreatedAt TEXT NOT NULL DEFAULT (datetime('now'))
                );

                CREATE TABLE IF NOT EXISTS ServerSNRegistry (
                    ServerSN INTEGER PRIMARY KEY AUTOINCREMENT,
                    LastUserUID INTEGER NOT NULL DEFAULT 0,
                    CreatedAt TEXT NOT NULL DEFAULT (datetime('now'))
                );

                CREATE TABLE IF NOT EXISTS LetterTradeMonitorLogs (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    RecvUnitUID INTEGER NOT NULL,
                    MonitorType INTEGER NOT NULL DEFAULT 0,
                    RegDate TEXT NOT NULL DEFAULT (datetime('now'))
                );

                CREATE TABLE IF NOT EXISTS HackingTools (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ToolName TEXT NOT NULL,
                    Flag INTEGER NOT NULL DEFAULT 1
                );

                CREATE TABLE IF NOT EXISTS ConcurrentUserLogs (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    RegionCode TEXT NOT NULL,
                    ServerUID INTEGER NOT NULL,
                    ServerGroupID INTEGER NOT NULL,
                    ChannelID INTEGER NOT NULL,
                    ConcurrentUser INTEGER NOT NULL,
                    RecordedAt TEXT NOT NULL DEFAULT (datetime('now'))
                );

                CREATE TABLE IF NOT EXISTS LoginResultLogs (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    RegionCode TEXT NOT NULL,
                    ServerUID INTEGER NOT NULL,
                    ResultType INTEGER NOT NULL,
                    Count INTEGER NOT NULL,
                    RecordedAt TEXT NOT NULL DEFAULT (datetime('now'))
                );

                CREATE TABLE IF NOT EXISTS PerformanceLogs (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ServerUID INTEGER NOT NULL,
                    MaxTick INTEGER NOT NULL,
                    MaxQueueSize INTEGER NOT NULL,
                    RecordedAt TEXT NOT NULL DEFAULT (datetime('now'))
                );

                CREATE TABLE IF NOT EXISTS ErrorStatisticsLogs (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ServerUID INTEGER NOT NULL,
                    FunctionName TEXT NOT NULL,
                    FileName TEXT NOT NULL,
                    LineNum TEXT NOT NULL,
                    Count INTEGER NOT NULL,
                    RecordedAt TEXT NOT NULL DEFAULT (datetime('now'))
                );

                CREATE TABLE IF NOT EXISTS SmsMessages (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    PhoneNumber TEXT NOT NULL,
                    Message TEXT NOT NULL,
                    SentAt TEXT NOT NULL DEFAULT (datetime('now'))
                );

                CREATE TABLE IF NOT EXISTS PCBangAccessLogs (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    UserUID INTEGER NOT NULL,
                    PublicIP TEXT NOT NULL,
                    MacAddress TEXT NOT NULL,
                    RecordedAt TEXT NOT NULL DEFAULT (datetime('now'))
                );

                CREATE TABLE IF NOT EXISTS Units (
                    UnitUID INTEGER PRIMARY KEY AUTOINCREMENT,
                    UserUID INTEGER NOT NULL,
                    RegionCode TEXT NOT NULL,
                    NickName TEXT NOT NULL,
                    UnitClass INTEGER NOT NULL DEFAULT 1,
                    Level INTEGER NOT NULL DEFAULT 1,
                    Exp INTEGER NOT NULL DEFAULT 0,
                    ED INTEGER NOT NULL DEFAULT 100000,
                    APoint INTEGER NOT NULL DEFAULT 0,
                    MapID INTEGER NOT NULL DEFAULT 20001,
                    Rating INTEGER NOT NULL DEFAULT 1000,
                    Win INTEGER NOT NULL DEFAULT 0,
                    Lose INTEGER NOT NULL DEFAULT 0,
                    Spirit INTEGER NOT NULL DEFAULT 100,
                    SpiritMax INTEGER NOT NULL DEFAULT 100,
                    GuildUID INTEGER NOT NULL DEFAULT 0,
                    IsDeleted INTEGER NOT NULL DEFAULT 0,
                    CreatedAt TEXT NOT NULL DEFAULT (datetime('now')),
                    UNIQUE (RegionCode, NickName)
                );

                CREATE TABLE IF NOT EXISTS DungeonClears (
                    UnitUID INTEGER NOT NULL,
                    DungeonID INTEGER NOT NULL,
                    MaxScore INTEGER NOT NULL DEFAULT 0,
                    MaxTotalRank INTEGER NOT NULL DEFAULT 0,
                    ClearTime TEXT NOT NULL DEFAULT '',
                    PRIMARY KEY (UnitUID, DungeonID)
                );

                CREATE TABLE IF NOT EXISTS InventoryItems (
                    ItemUID INTEGER PRIMARY KEY AUTOINCREMENT,
                    UnitUID INTEGER NOT NULL,
                    SlotCategory INTEGER NOT NULL DEFAULT 1,
                    SlotID INTEGER NOT NULL DEFAULT 0,
                    ItemID INTEGER NOT NULL,
                    UsageType INTEGER NOT NULL DEFAULT 1,
                    Quantity INTEGER NOT NULL DEFAULT 1,
                    Endurance INTEGER NOT NULL DEFAULT 100,
                    EnchantLevel INTEGER NOT NULL DEFAULT 0
                );

                CREATE TABLE IF NOT EXISTS MailLetters (
                    PostNo INTEGER PRIMARY KEY AUTOINCREMENT,
                    RegionCode TEXT NOT NULL,
                    FromUnitUID INTEGER NOT NULL,
                    FromNickName TEXT NOT NULL,
                    ToUnitUID INTEGER NOT NULL,
                    Title TEXT NOT NULL,
                    Message TEXT NOT NULL,
                    ItemID INTEGER NOT NULL DEFAULT 0,
                    Quantity INTEGER NOT NULL DEFAULT 0,
                    ED INTEGER NOT NULL DEFAULT 0,
                    IsRead INTEGER NOT NULL DEFAULT 0,
                    IsClaimed INTEGER NOT NULL DEFAULT 0,
                    SentAt TEXT NOT NULL DEFAULT (datetime('now'))
                );

                CREATE TABLE IF NOT EXISTS Guilds (
                    GuildUID INTEGER PRIMARY KEY AUTOINCREMENT,
                    RegionCode TEXT NOT NULL,
                    GuildName TEXT NOT NULL,
                    GuildMessage TEXT NOT NULL DEFAULT '',
                    GuildLevel INTEGER NOT NULL DEFAULT 1,
                    GuildExp INTEGER NOT NULL DEFAULT 0,
                    MasterUnitUID INTEGER NOT NULL,
                    MaxMembers INTEGER NOT NULL DEFAULT 30,
                    CreatedAt TEXT NOT NULL DEFAULT (datetime('now')),
                    UNIQUE (RegionCode, GuildName)
                );

                CREATE TABLE IF NOT EXISTS GuildMembers (
                    GuildUID INTEGER NOT NULL,
                    UnitUID INTEGER NOT NULL,
                    MemberGrade INTEGER NOT NULL DEFAULT 1,
                    PRIMARY KEY (GuildUID, UnitUID)
                );

                CREATE TABLE IF NOT EXISTS Quests (
                    UnitUID INTEGER NOT NULL,
                    QuestID INTEGER NOT NULL,
                    Status INTEGER NOT NULL DEFAULT 1,
                    Progress INTEGER NOT NULL DEFAULT 0,
                    UpdatedAt TEXT NOT NULL DEFAULT (datetime('now')),
                    PRIMARY KEY (UnitUID, QuestID)
                );

                CREATE TABLE IF NOT EXISTS SecondSecurity (
                    UserUID INTEGER PRIMARY KEY,
                    UseSecondPW INTEGER NOT NULL DEFAULT 0,
                    SecondPWHash TEXT NOT NULL DEFAULT '',
                    FailedCount INTEGER NOT NULL DEFAULT 0,
                    UpdatedAt TEXT NOT NULL DEFAULT (datetime('now'))
                );

                CREATE TABLE IF NOT EXISTS AccountBlockEntries (
                    RegionCode TEXT NOT NULL,
                    UserID TEXT NOT NULL,
                    BlockType INTEGER NOT NULL,
                    IsBlocked INTEGER NOT NULL DEFAULT 1,
                    Reason TEXT NOT NULL DEFAULT '',
                    UpdatedAt TEXT NOT NULL DEFAULT (datetime('now')),
                    PRIMARY KEY (RegionCode, UserID, BlockType)
                );

                CREATE TABLE IF NOT EXISTS PersonalShopListings (
                    ListingUID INTEGER PRIMARY KEY AUTOINCREMENT,
                    RegionCode TEXT NOT NULL,
                    SellerUnitUID INTEGER NOT NULL,
                    SellerNickName TEXT NOT NULL,
                    ItemID INTEGER NOT NULL,
                    Quantity INTEGER NOT NULL DEFAULT 1,
                    PriceED INTEGER NOT NULL,
                    IsSold INTEGER NOT NULL DEFAULT 0,
                    CreatedAt TEXT NOT NULL DEFAULT (datetime('now'))
                );

                CREATE TABLE IF NOT EXISTS GuildSkills (
                    GuildUID INTEGER NOT NULL,
                    SkillID INTEGER NOT NULL,
                    SkillLevel INTEGER NOT NULL DEFAULT 0,
                    PRIMARY KEY (GuildUID, SkillID)
                );

                CREATE TABLE IF NOT EXISTS GuildAds (
                    GuildUID INTEGER PRIMARY KEY,
                    RegionCode TEXT NOT NULL,
                    PeriodDays INTEGER NOT NULL DEFAULT 7,
                    AdMessage TEXT NOT NULL DEFAULT '',
                    UpdatedAt TEXT NOT NULL DEFAULT (datetime('now'))
                );

                CREATE TABLE IF NOT EXISTS GuildJoinApplies (
                    ApplyUID INTEGER PRIMARY KEY AUTOINCREMENT,
                    GuildUID INTEGER NOT NULL,
                    UnitUID INTEGER NOT NULL,
                    NickName TEXT NOT NULL,
                    UnitClass INTEGER NOT NULL DEFAULT 1,
                    Level INTEGER NOT NULL DEFAULT 1,
                    Message TEXT NOT NULL DEFAULT '',
                    CreatedAt TEXT NOT NULL DEFAULT (datetime('now')),
                    UNIQUE (GuildUID, UnitUID)
                );

                CREATE TABLE IF NOT EXISTS UnitSkillSlots (
                    UnitUID INTEGER NOT NULL,
                    SlotID INTEGER NOT NULL,
                    SkillID INTEGER NOT NULL DEFAULT 0,
                    PRIMARY KEY (UnitUID, SlotID)
                );

                CREATE TABLE IF NOT EXISTS UnitTitles (
                    UnitUID INTEGER NOT NULL,
                    TitleID INTEGER NOT NULL,
                    UnlockedAt TEXT NOT NULL DEFAULT (datetime('now')),
                    PRIMARY KEY (UnitUID, TitleID)
                );

                CREATE TABLE IF NOT EXISTS UnitPets (
                    PetUID INTEGER PRIMARY KEY AUTOINCREMENT,
                    UnitUID INTEGER NOT NULL,
                    PetID INTEGER NOT NULL,
                    PetName TEXT NOT NULL,
                    EvolutionStep INTEGER NOT NULL DEFAULT 0,
                    Intimacy INTEGER NOT NULL DEFAULT 0,
                    Satiety INTEGER NOT NULL DEFAULT 4800
                );

                CREATE TABLE IF NOT EXISTS UnitRidingPets (
                    RidingPetUID INTEGER PRIMARY KEY AUTOINCREMENT,
                    UnitUID INTEGER NOT NULL,
                    RidingPetID INTEGER NOT NULL,
                    Stamina REAL NOT NULL DEFAULT 100.0
                );

                CREATE TABLE IF NOT EXISTS Friendships (
                    UnitUID INTEGER NOT NULL,
                    FriendUnitUID INTEGER NOT NULL,
                    GroupID INTEGER NOT NULL DEFAULT 0,
                    IsBlocked INTEGER NOT NULL DEFAULT 0,
                    PRIMARY KEY (UnitUID, FriendUnitUID)
                );

                CREATE TABLE IF NOT EXISTS BlackListUsers (
                    UnitUID INTEGER NOT NULL,
                    TargetUnitUID INTEGER NOT NULL,
                    TargetNickName TEXT NOT NULL,
                    PRIMARY KEY (UnitUID, TargetUnitUID)
                );

                CREATE TABLE IF NOT EXISTS PostBlackList (
                    UnitUID INTEGER NOT NULL,
                    NickName TEXT NOT NULL,
                    PRIMARY KEY (UnitUID, NickName)
                );

                CREATE TABLE IF NOT EXISTS FriendGroups (
                    UnitUID INTEGER NOT NULL,
                    GroupID INTEGER NOT NULL,
                    GroupName TEXT NOT NULL,
                    PRIMARY KEY (UnitUID, GroupID)
                );

                CREATE TABLE IF NOT EXISTS LoveLetters (
                    LetterID INTEGER PRIMARY KEY AUTOINCREMENT,
                    RegionCode TEXT NOT NULL,
                    UnitUID INTEGER NOT NULL,
                    Message TEXT NOT NULL,
                    CreatedAt TEXT NOT NULL DEFAULT (datetime('now'))
                );

                CREATE TABLE IF NOT EXISTS Weddings (
                    WeddingUID INTEGER PRIMARY KEY AUTOINCREMENT,
                    RegionCode TEXT NOT NULL,
                    GroomUID INTEGER NOT NULL,
                    BrideUID INTEGER NOT NULL,
                    WeddingHallType INTEGER NOT NULL,
                    OfficiantNPC INTEGER NOT NULL,
                    WeddingDate TEXT NOT NULL,
                    WeddingMsg TEXT NOT NULL,
                    IsCompleted INTEGER NOT NULL DEFAULT 0
                );

                CREATE TABLE IF NOT EXISTS UnitRelationships (
                    UnitUID INTEGER PRIMARY KEY,
                    RelationshipType INTEGER NOT NULL DEFAULT 0,
                    LoverUnitUID INTEGER NOT NULL DEFAULT 0,
                    LoverNickName TEXT NOT NULL DEFAULT '',
                    LoveWord TEXT NOT NULL DEFAULT '',
                    AnniversaryDay TEXT NOT NULL DEFAULT '',
                    LastRewardDate TEXT NOT NULL DEFAULT '',
                    RewardTitleType INTEGER NOT NULL DEFAULT 0
                );

                CREATE TABLE IF NOT EXISTS FieldWorkingsBlockLogs (
                    LogId INTEGER PRIMARY KEY AUTOINCREMENT,
                    UnitUID INTEGER NOT NULL,
                    X2State INTEGER NOT NULL,
                    PhysicalMemory INTEGER NOT NULL,
                    VirtualMemory INTEGER NOT NULL,
                    ProcessTerm INTEGER NOT NULL,
                    FieldUserNum INTEGER NOT NULL,
                    FieldNpcNum INTEGER NOT NULL,
                    CreatedUtc TEXT NOT NULL
                );

                CREATE TABLE IF NOT EXISTS GamePlayNetworkLogs (
                    LogId INTEGER PRIMARY KEY AUTOINCREMENT,
                    UnitUID INTEGER NOT NULL,
                    OpponentUnitUID INTEGER NOT NULL,
                    HolePunchingTryCount INTEGER NOT NULL,
                    P2PPlayTime REAL NOT NULL,
                    RelayPlayTime REAL NOT NULL,
                    CreatedUtc TEXT NOT NULL
                );

                CREATE TABLE IF NOT EXISTS UserSystemInfos (
                    AccountUID INTEGER PRIMARY KEY,
                    SystemKey INTEGER NOT NULL,
                    Cpu TEXT NOT NULL DEFAULT '',
                    Memory TEXT NOT NULL DEFAULT '',
                    Gpu TEXT NOT NULL DEFAULT '',
                    Os TEXT NOT NULL DEFAULT '',
                    UpdatedUtc TEXT NOT NULL
                );

                CREATE TABLE IF NOT EXISTS GamePerformanceLogs (
                    LogId INTEGER PRIMARY KEY AUTOINCREMENT,
                    LogScope TEXT NOT NULL,
                    CharUID INTEGER NOT NULL,
                    SystemKey INTEGER NOT NULL,
                    MapID INTEGER NOT NULL,
                    AvgFps REAL NOT NULL,
                    PartyUserNum INTEGER NOT NULL,
                    CreatedUtc TEXT NOT NULL
                );

                CREATE TABLE IF NOT EXISTS UserHackingProcessLogs (
                    LogId INTEGER PRIMARY KEY AUTOINCREMENT,
                    AccountUID INTEGER NOT NULL,
                    UnitUID INTEGER NOT NULL,
                    HackingType INTEGER NOT NULL,
                    Reason TEXT NOT NULL DEFAULT '',
                    CreatedUtc TEXT NOT NULL
                );

                CREATE TABLE IF NOT EXISTS UnitRecruiters (
                    UnitUID INTEGER PRIMARY KEY,
                    RecruiterUnitUID INTEGER NOT NULL,
                    RecruiterNickName TEXT NOT NULL,
                    HasTicket INTEGER NOT NULL DEFAULT 0,
                    CreatedUtc TEXT NOT NULL
                );

                CREATE TABLE IF NOT EXISTS BillingOrders (
                    TransNo INTEGER PRIMARY KEY AUTOINCREMENT,
                    AccountUID INTEGER NOT NULL,
                    ProductNo INTEGER NOT NULL,
                    ProductID INTEGER NOT NULL,
                    ProductName TEXT NOT NULL,
                    Quantity INTEGER NOT NULL DEFAULT 1,
                    Price INTEGER NOT NULL DEFAULT 0,
                    OrderType INTEGER NOT NULL DEFAULT 1,
                    SenderNickName TEXT NOT NULL DEFAULT '',
                    GiftMessage TEXT NOT NULL DEFAULT '',
                    FromUnitUID INTEGER NOT NULL DEFAULT 0,
                    ProductKind INTEGER NOT NULL DEFAULT 0,
                    IsPickedUp INTEGER NOT NULL DEFAULT 0,
                    IsRefunded INTEGER NOT NULL DEFAULT 0,
                    CreatedUtc TEXT NOT NULL
                );

                CREATE TABLE IF NOT EXISTS CashWishLists (
                    AccountUID INTEGER NOT NULL,
                    ItemID INTEGER NOT NULL,
                    AddedUtc TEXT NOT NULL,
                    PRIMARY KEY (AccountUID, ItemID)
                );

                CREATE TABLE IF NOT EXISTS CashCoupons (
                    CouponSerial TEXT PRIMARY KEY,
                    CouponType INTEGER NOT NULL DEFAULT 0,
                    ProductNo INTEGER NOT NULL DEFAULT 0,
                    ItemID INTEGER NOT NULL DEFAULT 0,
                    Quantity INTEGER NOT NULL DEFAULT 1,
                    RedeemedByAccountUID INTEGER NOT NULL DEFAULT 0,
                    RedeemedUtc TEXT NOT NULL DEFAULT ''
                );

                CREATE TABLE IF NOT EXISTS CheatLogs (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    UnitUID INTEGER NOT NULL,
                    AccountUID INTEGER NOT NULL,
                    CheatCommand TEXT NOT NULL,
                    CheatContents TEXT NOT NULL,
                    CreatedUtc TEXT NOT NULL
                );

                CREATE TABLE IF NOT EXISTS DailyGiftBoxPurchases (
                    AccountUID INTEGER NOT NULL,
                    UnitUID INTEGER NOT NULL,
                    ItemID INTEGER NOT NULL,
                    State INTEGER NOT NULL DEFAULT 0,
                    RegDate TEXT NOT NULL DEFAULT '',
                    PRIMARY KEY (AccountUID, UnitUID, ItemID)
                );

                CREATE TABLE IF NOT EXISTS UserSettings (
                    AccountUID INTEGER PRIMARY KEY,
                    PlayGuideOn INTEGER NOT NULL DEFAULT 1,
                    ChatOptionsJson TEXT NOT NULL DEFAULT '{}',
                    KeyboardMappingJson TEXT NOT NULL DEFAULT '{}',
                    GamePadMappingJson TEXT NOT NULL DEFAULT '{}',
                    UpdatedUtc TEXT NOT NULL DEFAULT ''
                );

                CREATE TABLE IF NOT EXISTS UserLocalRankingProfiles (
                    AccountUID INTEGER PRIMARY KEY,
                    UnitUID INTEGER NOT NULL DEFAULT 0,
                    Category INTEGER NOT NULL DEFAULT -1,
                    Birth INTEGER NOT NULL DEFAULT 0,
                    Profile TEXT NOT NULL DEFAULT '',
                    FilterByte INTEGER NOT NULL DEFAULT 0,
                    GenderByte INTEGER NOT NULL DEFAULT 0,
                    Point0 INTEGER NOT NULL DEFAULT 0,
                    Point1 INTEGER NOT NULL DEFAULT 0,
                    UpdatedUtc TEXT NOT NULL DEFAULT ''
                );

                CREATE TABLE IF NOT EXISTS UserEventProgress (
                    UnitUID INTEGER PRIMARY KEY,
                    AttendanceLastDate TEXT NOT NULL DEFAULT '',
                    OldYearRewardedLevel INTEGER NOT NULL DEFAULT 0,
                    NewYearStepID INTEGER NOT NULL DEFAULT 0,
                    Anniv4LastDate TEXT NOT NULL DEFAULT '',
                    Anniv4Flags TEXT NOT NULL DEFAULT '000000000000',
                    CheckPowerTime INTEGER NOT NULL DEFAULT 0,
                    CheckPowerCount INTEGER NOT NULL DEFAULT 0,
                    CheckPowerScore INTEGER NOT NULL DEFAULT 0,
                    NoteViewCount INTEGER NOT NULL DEFAULT 0,
                    CoboClickTime TEXT NOT NULL DEFAULT '',
                    ChungChoices TEXT NOT NULL DEFAULT '',
                    MomotiReply INTEGER NOT NULL DEFAULT -1,
                    ChinaSpirit TEXT NOT NULL DEFAULT '0,0,0,0,0,0',
                    JumpingDate TEXT NOT NULL DEFAULT ''
                );

                CREATE TABLE IF NOT EXISTS UserRecommendations (
                    UnitUID INTEGER PRIMARY KEY,
                    RecommendedUnitUID INTEGER NOT NULL,
                    RecommendedNickName TEXT NOT NULL DEFAULT '',
                    CreatedUtc TEXT NOT NULL DEFAULT ''
                );

                CREATE TABLE IF NOT EXISTS HackAndPortLogs (
                    LogId INTEGER PRIMARY KEY AUTOINCREMENT,
                    AccountUID INTEGER NOT NULL DEFAULT 0,
                    UnitUID INTEGER NOT NULL DEFAULT 0,
                    LogKind TEXT NOT NULL,
                    CodeValue INTEGER NOT NULL DEFAULT 0,
                    SubValue INTEGER NOT NULL DEFAULT 0,
                    TextValue TEXT NOT NULL DEFAULT '',
                    CreatedUtc TEXT NOT NULL
                );

                CREATE TABLE IF NOT EXISTS HenirRankings (
                    RankingType INTEGER NOT NULL DEFAULT 0,
                    UnitUID INTEGER NOT NULL,
                    NickName TEXT NOT NULL,
                    UnitClass INTEGER NOT NULL DEFAULT 0,
                    Level INTEGER NOT NULL DEFAULT 1,
                    StageCount INTEGER NOT NULL DEFAULT 0,
                    ClearTimeSeconds INTEGER NOT NULL DEFAULT 0,
                    RecordedUtc TEXT NOT NULL,
                    PRIMARY KEY (RankingType, UnitUID)
                );

                CREATE TABLE IF NOT EXISTS TCClears (
                    UnitUID INTEGER NOT NULL,
                    TcID INTEGER NOT NULL,
                    ClearTime TEXT NOT NULL,
                    PRIMARY KEY (UnitUID, TcID)
                );

                CREATE TABLE IF NOT EXISTS DungeonPlays (
                    UnitUID INTEGER NOT NULL,
                    DungeonID INTEGER NOT NULL,
                    PlayTimes INTEGER NOT NULL DEFAULT 0,
                    ClearTimes INTEGER NOT NULL DEFAULT 0,
                    PRIMARY KEY (UnitUID, DungeonID)
                );

                CREATE TABLE IF NOT EXISTS ChatLogs (
                    LogID INTEGER PRIMARY KEY AUTOINCREMENT,
                    AccountUID INTEGER NOT NULL,
                    UnitUID INTEGER NOT NULL,
                    ServerGroupID INTEGER NOT NULL DEFAULT 0,
                    ChatText TEXT NOT NULL DEFAULT '',
                    ChatType INTEGER NOT NULL DEFAULT 0,
                    RecordedUtc TEXT NOT NULL
                );

                CREATE TABLE IF NOT EXISTS JackpotEventCoupons (
                    CouponNumber TEXT PRIMARY KEY,
                    BeginDate TEXT NOT NULL,
                    EndDate TEXT NOT NULL,
                    UserUID INTEGER NOT NULL DEFAULT 0,
                    UnitUID INTEGER NOT NULL DEFAULT 0,
                    State INTEGER NOT NULL DEFAULT 0
                );

                CREATE TABLE IF NOT EXISTS GameLogs (
                    LogUID INTEGER PRIMARY KEY AUTOINCREMENT,
                    AccountUID INTEGER NOT NULL DEFAULT 0,
                    UnitUID INTEGER NOT NULL DEFAULT 0,
                    LogKind TEXT NOT NULL DEFAULT '',
                    CodeValue INTEGER NOT NULL DEFAULT 0,
                    SubValue INTEGER NOT NULL DEFAULT 0,
                    Message TEXT NOT NULL DEFAULT '',
                    CreatedUtc TEXT NOT NULL
                );
                """;
            cmd.ExecuteNonQuery();

            EnsureColumn(conn, "Units", "RPoint", "INTEGER NOT NULL DEFAULT 0");
            EnsureColumn(conn, "Units", "MaxRating", "INTEGER NOT NULL DEFAULT 1000");
            EnsureColumn(conn, "Units", "KFactor", "REAL NOT NULL DEFAULT 300.0");
            EnsureColumn(conn, "Units", "TitleID", "INTEGER NOT NULL DEFAULT 0");
            EnsureColumn(conn, "Units", "SPoint", "INTEGER NOT NULL DEFAULT 0");
            EnsureColumn(conn, "Units", "CSPointEndDate", "TEXT NOT NULL DEFAULT ''");
            EnsureColumn(conn, "Units", "ServerGroupID", "INTEGER NOT NULL DEFAULT 0");
            EnsureColumn(conn, "Units", "RestoreAbleDate", "INTEGER NOT NULL DEFAULT 0");
            EnsureColumn(conn, "Units", "SkillPageCount", "INTEGER NOT NULL DEFAULT 1");
            EnsureColumn(conn, "Units", "ActiveSkillPage", "INTEGER NOT NULL DEFAULT 1");
            EnsureColumn(conn, "Units", "SkillNotePages", "TEXT NOT NULL DEFAULT ''");
            EnsureColumn(conn, "Units", "WarpVipEndDate", "TEXT NOT NULL DEFAULT ''");
            EnsureColumn(conn, "Units", "InventoryExpandGrade", "INTEGER NOT NULL DEFAULT 0");
            EnsureColumn(conn, "Units", "BankSlotCount", "INTEGER NOT NULL DEFAULT 8");
            EnsureColumn(conn, "InventoryItems", "IsBroken", "INTEGER NOT NULL DEFAULT 0");
            EnsureColumn(conn, "InventoryItems", "IsSealed", "INTEGER NOT NULL DEFAULT 0");
            EnsureColumn(conn, "InventoryItems", "SocketIds", "TEXT NOT NULL DEFAULT ''");
            EnsureColumn(conn, "InventoryItems", "RandomSocketIds", "TEXT NOT NULL DEFAULT ''");
            EnsureColumn(conn, "InventoryItems", "AttribEnchant", "TEXT NOT NULL DEFAULT '0,0,0'");
            EnsureColumn(conn, "InventoryItems", "EvalState", "INTEGER NOT NULL DEFAULT 0");
            EnsureColumn(conn, "InventoryItems", "Period", "INTEGER NOT NULL DEFAULT 0");
            EnsureColumn(conn, "InventoryItems", "ExpirationDate", "TEXT NOT NULL DEFAULT ''");
            EnsureColumn(conn, "Accounts", "HackingUserType", "INTEGER NOT NULL DEFAULT 0");
            EnsureColumn(conn, "Accounts", "ChannelingCode", "INTEGER NOT NULL DEFAULT 0");
            EnsureColumn(conn, "Accounts", "CashBalance", "INTEGER NOT NULL DEFAULT 10000");
            EnsureColumn(conn, "Accounts", "LastLoginUtc", "TEXT NOT NULL DEFAULT ''");
            EnsureColumn(conn, "Accounts", "ComeBackRewardStep", "INTEGER NOT NULL DEFAULT 0");
            EnsureColumn(conn, "Accounts", "ComeBackBuffEndUtc", "TEXT NOT NULL DEFAULT ''");
            EnsureColumn(conn, "Guilds", "GuildSPoint", "INTEGER NOT NULL DEFAULT 0");
            EnsureColumn(conn, "MailLetters", "ScriptType", "INTEGER NOT NULL DEFAULT 1");

            SeedDefaults(conn);
        }

        KLog.Cout($"SQLite database initialized at {DatabasePath}");
    }

    private static void EnsureColumn(SqliteConnection conn, string table, string column, string definition)
    {
        using var info = conn.CreateCommand();
        info.CommandText = $"PRAGMA table_info({table});";
        using var r = info.ExecuteReader();
        while (r.Read())
        {
            if (string.Equals(r.GetString(1), column, StringComparison.OrdinalIgnoreCase))
                return;
        }
        r.Close();
        using var alter = conn.CreateCommand();
        alter.CommandText = $"ALTER TABLE {table} ADD COLUMN {column} {definition};";
        alter.ExecuteNonQuery();
    }

    private static void SeedDefaults(SqliteConnection conn)
    {
        // Seed ServerGroups, ChannelServers, GameServers, Channels, and default test accounts per region if absent.
        var regions = new[]
        {
            (Code: "TW", Groups: new[] { (0, "冒險者 - 索雷斯 (Soles)"), (1, "自由者 - 蓋亞 (Gaia)") }),
            (Code: "HK", Groups: new[] { (0, "香港 - 索雷斯 (Soles)"), (1, "香港 - 蓋亞 (Gaia)") }),
            (Code: "CN", Groups: new[] { (1, "华东一区 (ZR01)"), (2, "华东二区 (ZR02)"), (101, "华北一区 (TJ01)") }),
            (Code: "JP", Groups: new[] { (0, "Soles (ソレス)"), (1, "Gaia (ガイア)") }),
        };

        foreach (var (code, groups) in regions)
        {
            var profile = RegionProfile.FromCode(Enum.Parse<RegionCode>(code));
            int order = 0;
            foreach (var (gid, gname) in groups)
            {
                using var gcmd = conn.CreateCommand();
                gcmd.CommandText = """
                    INSERT OR IGNORE INTO ServerGroups (RegionCode, ServerGroupID, ServerGroupName, FirstServerIP, FirstServerPort, OrderIndex, IsEnable)
                    VALUES ($r, $gid, $gname, '127.0.0.1', $chPort, $ord, 1);
                    """;
                gcmd.Parameters.AddWithValue("$r", code);
                gcmd.Parameters.AddWithValue("$gid", gid);
                gcmd.Parameters.AddWithValue("$gname", gname);
                gcmd.Parameters.AddWithValue("$chPort", profile.DefaultChannelPort);
                gcmd.Parameters.AddWithValue("$ord", order++);
                gcmd.ExecuteNonQuery();

                long chServerUid = KncUid.MakeUid(profile, gid, 1000 + gid);
                using var scmd = conn.CreateCommand();
                scmd.CommandText = """
                    INSERT OR IGNORE INTO ServerList (RegionCode, ServerUID, ServerName, ServerGroupID, ServerClass, PublicIP, PrivateIP, MasterPort, ClientPort, MaxUser, IsEnabled)
                    VALUES ($r, $uid, $name, $gid, 4, '127.0.0.1', '127.0.0.1', $chMasterPort, $chPort, 3000, 1);
                    """;
                scmd.Parameters.AddWithValue("$r", code);
                scmd.Parameters.AddWithValue("$uid", chServerUid);
                scmd.Parameters.AddWithValue("$name", $"{code}_ChannelServer_{gid}");
                scmd.Parameters.AddWithValue("$gid", gid);
                scmd.Parameters.AddWithValue("$chMasterPort", profile.DefaultChannelPort + 1);
                scmd.Parameters.AddWithValue("$chPort", profile.DefaultChannelPort);
                scmd.ExecuteNonQuery();

                for (int ch = 1; ch <= 3; ch++)
                {
                    int channelId = gid * 100 + ch;
                    using var ccmd = conn.CreateCommand();
                    ccmd.CommandText = """
                        INSERT OR IGNORE INTO ChannelList (RegionCode, ChannelID, ServerGroupID, ServerUID, ChannelName, MaxUser, CurrentUser, IP, Port)
                        VALUES ($r, $cid, $gid, $uid, $cname, 1000, 0, '127.0.0.1', $port);
                        """;
                    ccmd.Parameters.AddWithValue("$r", code);
                    ccmd.Parameters.AddWithValue("$cid", channelId);
                    ccmd.Parameters.AddWithValue("$gid", gid);
                    ccmd.Parameters.AddWithValue("$uid", chServerUid);
                    ccmd.Parameters.AddWithValue("$cname", $"{gname} CH-{ch}");
                    ccmd.Parameters.AddWithValue("$port", profile.DefaultGamePort);
                    ccmd.ExecuteNonQuery();
                }

                // Seed a virtual GameServer entry for E_TOOL_SERVER_LIST_ACK / DBE_GET_GAME_SERVER_LIST_ACK
                long gsUid = KncUid.MakeUid(profile, gid, 2000 + gid);
                using var gsCmd = conn.CreateCommand();
                gsCmd.CommandText = """
                    INSERT OR IGNORE INTO ServerList (RegionCode, ServerUID, ServerName, ServerGroupID, ServerClass, PublicIP, PrivateIP, MasterPort, ClientPort, MaxUser, IsEnabled)
                    VALUES ($r, $uid, $name, $gid, 3, '127.0.0.1', '127.0.0.1', $trPort, $gsPort, 3000, 1);
                    """;
                gsCmd.Parameters.AddWithValue("$r", code);
                gsCmd.Parameters.AddWithValue("$uid", gsUid);
                gsCmd.Parameters.AddWithValue("$name", $"{code}_GameServer_{gid}");
                gsCmd.Parameters.AddWithValue("$gid", gid);
                gsCmd.Parameters.AddWithValue("$trPort", profile.DefaultTrPort);
                gsCmd.Parameters.AddWithValue("$gsPort", profile.DefaultGamePort);
                gsCmd.ExecuteNonQuery();
            }

            // Migrate prior local dev placeholder ports (9100/9101 -> official 9400/12400, ChannelList 9301..9303 -> DefaultGamePort)
            using var migCmd = conn.CreateCommand();
            migCmd.CommandText = """
                UPDATE ServerGroups SET FirstServerPort = $chPort
                WHERE RegionCode = $r AND FirstServerIP = '127.0.0.1' AND FirstServerPort = 9100;

                UPDATE ServerList SET MasterPort = $chMasterPort, ClientPort = $chPort
                WHERE RegionCode = $r AND ServerClass = 4 AND PublicIP = '127.0.0.1' AND ClientPort = 9100;

                UPDATE ChannelList SET Port = $gsPort
                WHERE RegionCode = $r AND IP = '127.0.0.1' AND Port IN (9301, 9302, 9303);

                UPDATE ServerList SET MasterPort = $trPort, ClientPort = $gsPort
                WHERE RegionCode = $r AND ServerClass = 3 AND PublicIP = '127.0.0.1' AND ClientPort = 9300 AND MasterPort = 9301;
                """;
            migCmd.Parameters.AddWithValue("$r", code);
            migCmd.Parameters.AddWithValue("$chPort", profile.DefaultChannelPort);
            migCmd.Parameters.AddWithValue("$chMasterPort", profile.DefaultChannelPort + 1);
            migCmd.Parameters.AddWithValue("$gsPort", profile.DefaultGamePort);
            migCmd.Parameters.AddWithValue("$trPort", profile.DefaultTrPort);
            migCmd.ExecuteNonQuery();

            // Default dev test accounts per region
            EnsureAccountInternal(conn, code, "test01", "test01", authLevel: 2);
            EnsureAccountInternal(conn, code, "gm01", "gm01", authLevel: 5);
        }

        // Seed default server SN row if empty
        using var snCheck = conn.CreateCommand();
        snCheck.CommandText = "SELECT COUNT(1) FROM ServerSNRegistry;";
        if (Convert.ToInt64(snCheck.ExecuteScalar()) == 0)
        {
            using var snIns = conn.CreateCommand();
            snIns.CommandText = "INSERT INTO ServerSNRegistry (ServerSN, LastUserUID) VALUES (100001, 0);";
            snIns.ExecuteNonQuery();
        }
    }

    public static string HashPassword(string password)
    {
        byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(bytes);
    }

    private static void EnsureAccountInternal(SqliteConnection conn, string regionCode, string userId, string password, int authLevel = 0)
    {
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            INSERT OR IGNORE INTO Accounts (RegionCode, UserID, PasswordHash, AuthLevel)
            VALUES ($r, $id, $pw, $lvl);
            """;
        cmd.Parameters.AddWithValue("$r", regionCode);
        cmd.Parameters.AddWithValue("$id", userId);
        cmd.Parameters.AddWithValue("$pw", HashPassword(password));
        cmd.Parameters.AddWithValue("$lvl", authLevel);
        cmd.ExecuteNonQuery();
    }

    public bool CreateAccount(string regionCode, string userId, string password, int authLevel = 0)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                INSERT OR IGNORE INTO Accounts (RegionCode, UserID, PasswordHash, AuthLevel)
                VALUES ($r, $id, $pw, $lvl);
                """;
            cmd.Parameters.AddWithValue("$r", regionCode);
            cmd.Parameters.AddWithValue("$id", userId);
            cmd.Parameters.AddWithValue("$pw", HashPassword(password));
            cmd.Parameters.AddWithValue("$lvl", authLevel);
            return cmd.ExecuteNonQuery() > 0;
        }
    }

    public (int NetErr, long UserUid, int AuthLevel, bool IsGuest, string BlockReason) VerifyOrRegisterAccount(
        string regionCode,
        string userId,
        string password,
        bool autoCreateOnMissing = false)
    {
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT UserUID, PasswordHash, AuthLevel, IsBlocked, BlockReason, IsGuestUser
            FROM Accounts
            WHERE RegionCode = $r AND UserID = $id
            LIMIT 1;
            """;
        cmd.Parameters.AddWithValue("$r", regionCode);
        cmd.Parameters.AddWithValue("$id", userId);

        using var reader = cmd.ExecuteReader();
        if (!reader.Read())
        {
            reader.Close();
            if (!autoCreateOnMissing)
                return (NetError.ERR_VERIFY_02, 0, 0, false, string.Empty);

            CreateAccount(regionCode, userId, password, 0);
            return VerifyOrRegisterAccount(regionCode, userId, password, autoCreateOnMissing: false);
        }

        long uid = reader.GetInt64(0);
        string storedHash = reader.GetString(1);
        int authLevel = reader.GetInt32(2);
        bool isBlocked = reader.GetInt32(3) != 0;
        string blockReason = reader.GetString(4);
        bool isGuest = reader.GetInt32(5) != 0;

        if (isBlocked)
            return (NetError.ERR_VERIFY_04, uid, authLevel, isGuest, blockReason);

        if (!string.IsNullOrEmpty(password) && !string.Equals(storedHash, HashPassword(password), StringComparison.OrdinalIgnoreCase))
            return (NetError.ERR_VERIFY_03, 0, 0, false, string.Empty);

        return (NetError.NET_OK, uid, authLevel, isGuest, string.Empty);
    }

    public KServerInfo? GetServerInfo(string regionCode, string ip, int serverClass = 4, int serverGroupId = -1)
    {
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = serverGroupId >= 0
            ? "SELECT ServerUID, ServerName, ServerGroupID, ServerClass, PublicIP, PrivateIP, MasterPort, ClientPort, MaxUser, IsEnabled FROM ServerList WHERE RegionCode=$r AND ServerClass=$sc AND ServerGroupID=$gid LIMIT 1;"
            : "SELECT ServerUID, ServerName, ServerGroupID, ServerClass, PublicIP, PrivateIP, MasterPort, ClientPort, MaxUser, IsEnabled FROM ServerList WHERE RegionCode=$r AND ServerClass=$sc ORDER BY ServerGroupID ASC LIMIT 1;";
        cmd.Parameters.AddWithValue("$r", regionCode);
        cmd.Parameters.AddWithValue("$sc", serverClass);
        if (serverGroupId >= 0)
            cmd.Parameters.AddWithValue("$gid", serverGroupId);

        using var r = cmd.ExecuteReader();
        if (!r.Read()) return null;

        return new KServerInfo
        {
            Uid = r.GetInt64(0),
            Name = r.GetString(1),
            ServerGroupId = r.GetInt32(2),
            ServerClass = r.GetInt32(3),
            Ip = r.GetString(4),
            PublicIp = r.GetString(4),
            PrivateIp = r.GetString(5),
            MasterPort = (ushort)r.GetInt32(6),
            NcUdpPort = (ushort)r.GetInt32(7),
            MaxUser = r.GetInt32(8),
            On = r.GetInt32(9) != 0,
        };
    }

    public Dictionary<int, KChannelInfo> GetChannelList(string regionCode, int? serverGroupId = null)
    {
        var result = new Dictionary<int, KChannelInfo>();
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = serverGroupId.HasValue
            ? "SELECT ChannelID, ServerGroupID, ServerUID, ChannelName, MaxUser, CurrentUser, IP, Port FROM ChannelList WHERE RegionCode=$r AND ServerGroupID=$gid ORDER BY ChannelID ASC;"
            : "SELECT ChannelID, ServerGroupID, ServerUID, ChannelName, MaxUser, CurrentUser, IP, Port FROM ChannelList WHERE RegionCode=$r ORDER BY ChannelID ASC;";
        cmd.Parameters.AddWithValue("$r", regionCode);
        if (serverGroupId.HasValue)
            cmd.Parameters.AddWithValue("$gid", serverGroupId.Value);

        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            int cid = r.GetInt32(0);
            result[cid] = new KChannelInfo
            {
                ChannelId = cid,
                ServerGroupId = r.GetInt32(1),
                ServerUid = r.GetInt64(2),
                ChannelName = r.GetString(3),
                MaxUser = r.GetInt32(4),
                CurrentUser = r.GetInt32(5),
                Ip = r.GetString(6),
                MasterPort = (ushort)r.GetInt32(7),
            };
        }
        return result;
    }

    public Dictionary<int, KServerGroupInfo> GetServerGroupList(string regionCode)
    {
        var result = new Dictionary<int, KServerGroupInfo>();
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT ServerGroupID, ServerGroupName, FirstServerIP, FirstServerPort, OrderIndex, IsEnable
            FROM ServerGroups WHERE RegionCode=$r ORDER BY OrderIndex ASC;
            """;
        cmd.Parameters.AddWithValue("$r", regionCode);

        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            int gid = r.GetInt32(0);
            result[gid] = new KServerGroupInfo
            {
                Order = r.GetInt32(4),
                ServerGroupUid = gid,
                ServerGroupName = r.GetString(1),
                ServerIp = r.GetString(2),
                MasterPort = (ushort)r.GetInt32(3),
                Enable = r.GetInt32(5) != 0,
            };
        }
        return result;
    }

    public List<KServerInfo> GetServerList(string regionCode, int? serverClass = null)
    {
        var list = new List<KServerInfo>();
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = serverClass.HasValue
            ? "SELECT ServerUID, ServerName, ServerGroupID, ServerClass, PublicIP, PrivateIP, MasterPort, ClientPort, MaxUser, IsEnabled FROM ServerList WHERE RegionCode=$r AND ServerClass=$sc;"
            : "SELECT ServerUID, ServerName, ServerGroupID, ServerClass, PublicIP, PrivateIP, MasterPort, ClientPort, MaxUser, IsEnabled FROM ServerList WHERE RegionCode=$r;";
        cmd.Parameters.AddWithValue("$r", regionCode);
        if (serverClass.HasValue)
            cmd.Parameters.AddWithValue("$sc", serverClass.Value);

        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            list.Add(new KServerInfo
            {
                Uid = r.GetInt64(0),
                Name = r.GetString(1),
                ServerGroupId = r.GetInt32(2),
                ServerClass = r.GetInt32(3),
                Ip = r.GetString(4),
                PublicIp = r.GetString(4),
                PrivateIp = r.GetString(5),
                MasterPort = (ushort)r.GetInt32(6),
                NcUdpPort = (ushort)r.GetInt32(7),
                MaxUser = r.GetInt32(8),
                On = r.GetInt32(9) != 0,
            });
        }
        return list;
    }

    public bool IsAccountBlocked(string regionCode, string userId, out string reason)
    {
        reason = string.Empty;
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT IsBlocked, BlockReason FROM Accounts WHERE RegionCode=$r AND UserID=$id LIMIT 1;";
        cmd.Parameters.AddWithValue("$r", regionCode);
        cmd.Parameters.AddWithValue("$id", userId);
        using var r = cmd.ExecuteReader();
        if (!r.Read()) return false;
        if (r.GetInt32(0) == 0) return false;
        reason = r.GetString(1);
        return true;
    }

    public bool IsMachineBlocked(string machineId)
    {
        if (string.IsNullOrEmpty(machineId)) return false;
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT COUNT(1) FROM MachineBlocks WHERE MachineID=$m;";
        cmd.Parameters.AddWithValue("$m", machineId);
        return Convert.ToInt64(cmd.ExecuteScalar()) > 0;
    }

    public long CreateServerSN(long userUid)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO ServerSNRegistry (LastUserUID) VALUES ($uid); SELECT last_insert_rowid();";
            cmd.Parameters.AddWithValue("$uid", userUid);
            return Convert.ToInt64(cmd.ExecuteScalar());
        }
    }

    public bool CheckServerSNExists(long serverSn)
    {
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT COUNT(1) FROM ServerSNRegistry WHERE ServerSN=$sn;";
        cmd.Parameters.AddWithValue("$sn", serverSn);
        return Convert.ToInt64(cmd.ExecuteScalar()) > 0;
    }

    public void UpdateRandomKey(string regionCode, string userId, string randomKey)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE Accounts SET RandomKey=$k WHERE RegionCode=$r AND UserID=$id;";
            cmd.Parameters.AddWithValue("$k", randomKey);
            cmd.Parameters.AddWithValue("$r", regionCode);
            cmd.Parameters.AddWithValue("$id", userId);
            cmd.ExecuteNonQuery();
        }
    }

    public int GetTotalConcurrentUsers(string regionCode, int serverGroupId)
    {
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT COALESCE(SUM(CurrentUser), 0) FROM ChannelList WHERE RegionCode=$r AND ServerGroupID=$gid;";
        cmd.Parameters.AddWithValue("$r", regionCode);
        cmd.Parameters.AddWithValue("$gid", serverGroupId);
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    public void UpdateChannelConcurrentUser(string regionCode, long serverUid, int concurrentUser)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE ChannelList SET CurrentUser=$ccu WHERE RegionCode=$r AND ServerUID=$uid;";
            cmd.Parameters.AddWithValue("$ccu", concurrentUser);
            cmd.Parameters.AddWithValue("$r", regionCode);
            cmd.Parameters.AddWithValue("$uid", serverUid);
            cmd.ExecuteNonQuery();
        }
    }

    public void InsertConcurrentUserLog(string regionCode, long serverUid, int serverGroupId, int channelId, int ccu)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO ConcurrentUserLogs (RegionCode, ServerUID, ServerGroupID, ChannelID, ConcurrentUser) VALUES ($r, $uid, $gid, $cid, $ccu);";
            cmd.Parameters.AddWithValue("$r", regionCode);
            cmd.Parameters.AddWithValue("$uid", serverUid);
            cmd.Parameters.AddWithValue("$gid", serverGroupId);
            cmd.Parameters.AddWithValue("$cid", channelId);
            cmd.Parameters.AddWithValue("$ccu", ccu);
            cmd.ExecuteNonQuery();
        }
    }

    public void InsertLoginStats(string regionCode, long serverUid, IReadOnlyList<int> counts)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var tx = conn.BeginTransaction();
            for (int i = 0; i < counts.Count; i++)
            {
                if (counts[i] <= 0) continue;
                using var cmd = conn.CreateCommand();
                cmd.Transaction = tx;
                cmd.CommandText = "INSERT INTO LoginResultLogs (RegionCode, ServerUID, ResultType, Count) VALUES ($r, $uid, $rt, $cnt);";
                cmd.Parameters.AddWithValue("$r", regionCode);
                cmd.Parameters.AddWithValue("$uid", serverUid);
                cmd.Parameters.AddWithValue("$rt", i);
                cmd.Parameters.AddWithValue("$cnt", counts[i]);
                cmd.ExecuteNonQuery();
            }
            tx.Commit();
        }
    }

    public void InsertPerformanceLog(long serverUid, int maxTick, int maxQueueSize)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO PerformanceLogs (ServerUID, MaxTick, MaxQueueSize) VALUES ($uid, $mt, $mq);";
            cmd.Parameters.AddWithValue("$uid", serverUid);
            cmd.Parameters.AddWithValue("$mt", maxTick);
            cmd.Parameters.AddWithValue("$mq", maxQueueSize);
            cmd.ExecuteNonQuery();
        }
    }

    public void InsertErrorStatistics(long serverUid, IReadOnlyList<KLogStatInfo> stats)
    {
        if (stats.Count == 0) return;
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var tx = conn.BeginTransaction();
            foreach (var s in stats)
            {
                using var cmd = conn.CreateCommand();
                cmd.Transaction = tx;
                cmd.CommandText = "INSERT INTO ErrorStatisticsLogs (ServerUID, FunctionName, FileName, LineNum, Count) VALUES ($uid, $fn, $file, $ln, $cnt);";
                cmd.Parameters.AddWithValue("$uid", serverUid);
                cmd.Parameters.AddWithValue("$fn", s.FunctionName);
                cmd.Parameters.AddWithValue("$file", s.FileName);
                cmd.Parameters.AddWithValue("$ln", s.LineNum);
                cmd.Parameters.AddWithValue("$cnt", s.Count);
                cmd.ExecuteNonQuery();
            }
            tx.Commit();
        }
    }

    public void InsertSmsMessage(string phoneNumber, string message)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO SmsMessages (PhoneNumber, Message) VALUES ($ph, $msg);";
            cmd.Parameters.AddWithValue("$ph", phoneNumber);
            cmd.Parameters.AddWithValue("$msg", message);
            cmd.ExecuteNonQuery();
        }
    }

    public void InsertPCBangAccessLog(long userUid, string publicIp, string macAddress)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO PCBangAccessLogs (UserUID, PublicIP, MacAddress) VALUES ($uid, $ip, $mac);";
            cmd.Parameters.AddWithValue("$uid", userUid);
            cmd.Parameters.AddWithValue("$ip", publicIp);
            cmd.Parameters.AddWithValue("$mac", macAddress);
            cmd.ExecuteNonQuery();
        }
    }

    public List<KHackingToolInfo> GetHackingToolList()
    {
        var list = new List<KHackingToolInfo>();
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT ToolName, Flag FROM HackingTools;";
        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            list.Add(new KHackingToolInfo
            {
                ProcessName = r.GetString(0),
                Flag = (sbyte)r.GetInt32(1),
            });
        }
        return list;
    }

    // ---------------- Accounts (New Join) ----------------

    public (bool ok, long userUid, string error) CreateAccount(RegionProfile region, string userId, string password)
    {
        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(password))
            return (false, 0, "Empty credentials");

        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var check = conn.CreateCommand();
            check.CommandText = "SELECT UserUID FROM Accounts WHERE RegionCode=$r AND UserID=$id LIMIT 1;";
            check.Parameters.AddWithValue("$r", region.Code);
            check.Parameters.AddWithValue("$id", userId);
            if (check.ExecuteScalar() != null)
                return (false, 0, "Account already exists");

            using var ins = conn.CreateCommand();
            ins.CommandText = """
                INSERT INTO Accounts (RegionCode, UserID, PasswordHash, AuthLevel, IsBlocked, IsGuestUser)
                VALUES ($r, $id, $hash, 2, 0, 0);
                SELECT last_insert_rowid();
                """;
            ins.Parameters.AddWithValue("$r", region.Code);
            ins.Parameters.AddWithValue("$id", userId);
            ins.Parameters.AddWithValue("$hash", HashPassword(password));
            long uid = Convert.ToInt64(ins.ExecuteScalar());
            return (true, uid, string.Empty);
        }
    }

    // ---------------- Units ----------------

    public List<KUnitInfo> GetUnitsForAccount(RegionProfile region, long userUid)
    {
        var list = new List<KUnitInfo>();
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT UnitUID, NickName, UnitClass, Level, Exp, ED, APoint, MapID, Rating, Win, Lose, Spirit, SpiritMax, GuildUID
            FROM Units
            WHERE UserUID=$uid AND RegionCode=$r AND IsDeleted=0
            ORDER BY UnitUID ASC;
            """;
        cmd.Parameters.AddWithValue("$uid", userUid);
        cmd.Parameters.AddWithValue("$r", region.Code);
        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            int guildUid = r.GetInt32(13);
            var unit = new KUnitInfo
            {
                OwnerUserUid = userUid,
                UnitUid = r.GetInt64(0),
                NickName = r.GetString(1),
                UnitClass = (sbyte)r.GetInt32(2),
                Level = (byte)Math.Clamp(r.GetInt32(3), 1, 255),
                Exp = r.GetInt32(4),
                Ed = r.GetInt32(5),
                APoint = r.GetInt32(6),
                LastPos = new KLastPositionInfo { MapId = r.GetInt32(7) },
                Rating = r.GetInt32(8),
                MaxRating = r.GetInt32(8),
                Win = r.GetInt32(9),
                Lose = r.GetInt32(10),
                Spirit = r.GetInt32(11),
                SpiritMax = r.GetInt32(12),
                NextBaseLevelExp = Math.Max(1000, r.GetInt32(3) * 1000),
                UserGuildInfo = new KUserGuildInfo { GuildUid = guildUid },
            };
            list.Add(unit);
        }
        return list;
    }

    public (bool ok, KUnitInfo? unit, int netError) CreateUnit(RegionProfile region, long userUid, string nickName, int unitClass)
    {
        if (string.IsNullOrWhiteSpace(nickName))
            return (false, null, region.ResolveNetError(NetErrorKey.EmptyId));

        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var check = conn.CreateCommand();
            check.CommandText = "SELECT UnitUID FROM Units WHERE RegionCode=$r AND NickName=$n AND IsDeleted=0 LIMIT 1;";
            check.Parameters.AddWithValue("$r", region.Code);
            check.Parameters.AddWithValue("$n", nickName);
            if (check.ExecuteScalar() != null)
                return (false, null, region.ResolveNetError(NetErrorKey.DuplicateId));

            using var ins = conn.CreateCommand();
            ins.CommandText = """
                INSERT INTO Units (UserUID, RegionCode, NickName, UnitClass, Level, Exp, ED, MapID)
                VALUES ($uid, $r, $n, $uc, 1, 0, 100000, 20001);
                SELECT last_insert_rowid();
                """;
            ins.Parameters.AddWithValue("$uid", userUid);
            ins.Parameters.AddWithValue("$r", region.Code);
            ins.Parameters.AddWithValue("$n", nickName);
            ins.Parameters.AddWithValue("$uc", unitClass);
            long unitUid = Convert.ToInt64(ins.ExecuteScalar());

            // Grant starter weapon & potion
            InsertItemInternal(conn, unitUid, slotCategory: 1, slotId: 0, itemId: 10000 + Math.Max(1, unitClass) * 100, qty: 1);
            InsertItemInternal(conn, unitUid, slotCategory: 2, slotId: 0, itemId: 130001, qty: 10);

            return (true, new KUnitInfo
            {
                OwnerUserUid = userUid,
                UnitUid = unitUid,
                NickName = nickName,
                UnitClass = (sbyte)unitClass,
                Level = 1,
                Ed = 100000,
                LastPos = new KLastPositionInfo { MapId = 20001 },
            }, NetError.NET_OK);
        }
    }

    public bool DeleteUnit(RegionProfile region, long userUid, long unitUid)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE Units SET IsDeleted=1 WHERE UnitUID=$u AND UserUID=$a AND RegionCode=$r;";
            cmd.Parameters.AddWithValue("$u", unitUid);
            cmd.Parameters.AddWithValue("$a", userUid);
            cmd.Parameters.AddWithValue("$r", region.Code);
            return cmd.ExecuteNonQuery() > 0;
        }
    }

    public void UpdateUnitProgress(long unitUid, int addExp, int addEd, int mapId, bool win = false, bool lose = false)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            byte curLevel = 1;
            int curExp = 0;
            using (var sel = conn.CreateCommand())
            {
                sel.CommandText = "SELECT Level, Exp FROM Units WHERE UnitUID=$u LIMIT 1;";
                sel.Parameters.AddWithValue("$u", unitUid);
                using var r = sel.ExecuteReader();
                if (r.Read())
                {
                    curLevel = (byte)Math.Clamp(r.GetInt32(0), 1, 99);
                    curExp = Math.Max(0, r.GetInt32(1));
                }
            }

            int nextExp = Math.Max(0, curExp + addExp);
            byte nextLevel = Math.Max(curLevel, ExpTable.CheckLevelUp(curLevel, nextExp));

            using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                UPDATE Units
                SET Exp = $exp,
                    Level = $lvl,
                    ED = MAX(0, ED + $ed),
                    MapID = CASE WHEN $map > 0 THEN $map ELSE MapID END,
                    Win = Win + $win,
                    Lose = Lose + $lose
                WHERE UnitUID = $u;
                """;
            cmd.Parameters.AddWithValue("$exp", nextExp);
            cmd.Parameters.AddWithValue("$lvl", (int)nextLevel);
            cmd.Parameters.AddWithValue("$ed", addEd);
            cmd.Parameters.AddWithValue("$map", mapId);
            cmd.Parameters.AddWithValue("$win", win ? 1 : 0);
            cmd.Parameters.AddWithValue("$lose", lose ? 1 : 0);
            cmd.Parameters.AddWithValue("$u", unitUid);
            cmd.ExecuteNonQuery();
        }
    }

    public KUnitInfo? GetUnitByUid(RegionProfile region, long unitUid)
    {
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT UserUID, NickName, UnitClass, Level, Exp, ED, APoint, MapID, Rating, Win, Lose, Spirit, SpiritMax, GuildUID, TitleID, MaxRating
            FROM Units WHERE UnitUID=$u AND IsDeleted=0 LIMIT 1;
            """;
        cmd.Parameters.AddWithValue("$u", unitUid);
        using var r = cmd.ExecuteReader();
        if (!r.Read()) return null;
        var unit = new KUnitInfo
        {
            OwnerUserUid = r.GetInt64(0),
            UnitUid = unitUid,
            NickName = r.GetString(1),
            UnitClass = (sbyte)r.GetInt32(2),
            Level = (byte)Math.Clamp(r.GetInt32(3), 1, 255),
            Exp = r.GetInt32(4),
            Ed = r.GetInt32(5),
            APoint = r.GetInt32(6),
            LastPos = new KLastPositionInfo { MapId = r.GetInt32(7) },
            Rating = r.GetInt32(8),
            MaxRating = r.GetInt32(15),
            Win = r.GetInt32(9),
            Lose = r.GetInt32(10),
            Spirit = r.GetInt32(11),
            SpiritMax = r.GetInt32(12),
            NextBaseLevelExp = Math.Max(1000, r.GetInt32(3) * 1000),
            UserGuildInfo = new KUserGuildInfo { GuildUid = r.GetInt32(13) },
            TitleId = r.GetInt32(14),
        };
        r.Close();
        using var dcCmd = conn.CreateCommand();
        dcCmd.CommandText = "SELECT DungeonID, MaxScore, MaxTotalRank, ClearTime FROM DungeonClears WHERE UnitUID=$u;";
        dcCmd.Parameters.AddWithValue("$u", unitUid);
        using var dcr = dcCmd.ExecuteReader();
        while (dcr.Read())
        {
            int did = dcr.GetInt32(0);
            unit.DungeonClear[did] = new KDungeonClearInfo
            {
                DungeonId = did,
                MaxScore = dcr.GetInt32(1),
                MaxTotalRank = (sbyte)dcr.GetInt32(2),
                ClearTime = dcr.GetString(3),
            };
        }
        dcr.Close();
        using var tcCmd = conn.CreateCommand();
        tcCmd.CommandText = "SELECT TcID, ClearTime FROM TCClears WHERE UnitUID=$u;";
        tcCmd.Parameters.AddWithValue("$u", unitUid);
        using var tcr = tcCmd.ExecuteReader();
        while (tcr.Read())
        {
            int tcId = tcr.GetInt32(0);
            unit.TcClear[tcId] = new KTCClearInfo
            {
                TcId = tcId,
                ClearTime = tcr.GetString(1),
                New = false,
            };
        }
        tcr.Close();
        using var dpCmd = conn.CreateCommand();
        dpCmd.CommandText = "SELECT DungeonID, PlayTimes, ClearTimes FROM DungeonPlays WHERE UnitUID=$u;";
        dpCmd.Parameters.AddWithValue("$u", unitUid);
        using var dpr = dpCmd.ExecuteReader();
        while (dpr.Read())
        {
            int did = dpr.GetInt32(0);
            unit.DungeonPlay[did] = new KDungeonPlayInfo
            {
                DungeonId = did,
                PlayTimes = dpr.GetInt32(1),
                ClearTimes = dpr.GetInt32(2),
                New = false,
            };
        }
        return unit;
    }

    public void RecordTcClear(long unitUid, int tcId)
    {
        if (unitUid <= 0 || tcId <= 0) return;
        string nowStr = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                INSERT INTO TCClears (UnitUID, TcID, ClearTime)
                VALUES ($u, $tc, $t)
                ON CONFLICT(UnitUID, TcID) DO UPDATE SET ClearTime = excluded.ClearTime;
                """;
            cmd.Parameters.AddWithValue("$u", unitUid);
            cmd.Parameters.AddWithValue("$tc", tcId);
            cmd.Parameters.AddWithValue("$t", nowStr);
            cmd.ExecuteNonQuery();
        }
    }

    public void RecordDungeonPlay(long unitUid, int dungeonId, bool cleared)
    {
        if (unitUid <= 0 || dungeonId <= 0) return;
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                INSERT INTO DungeonPlays (UnitUID, DungeonID, PlayTimes, ClearTimes)
                VALUES ($u, $d, $p, $c)
                ON CONFLICT(UnitUID, DungeonID) DO UPDATE SET
                    PlayTimes = PlayTimes + $p,
                    ClearTimes = ClearTimes + $c;
                """;
            cmd.Parameters.AddWithValue("$u", unitUid);
            cmd.Parameters.AddWithValue("$d", dungeonId);
            cmd.Parameters.AddWithValue("$p", cleared ? 0 : 1);
            cmd.Parameters.AddWithValue("$c", cleared ? 1 : 0);
            cmd.ExecuteNonQuery();
        }
    }

    public void RecordDungeonClear(long unitUid, int dungeonId, int maxScore, sbyte maxTotalRank)
    {
        if (unitUid <= 0 || dungeonId <= 0) return;
        string nowStr = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                INSERT INTO DungeonClears (UnitUID, DungeonID, MaxScore, MaxTotalRank, ClearTime)
                VALUES ($u, $d, $s, $r, $t)
                ON CONFLICT(UnitUID, DungeonID) DO UPDATE SET
                    MaxScore = MAX(MaxScore, excluded.MaxScore),
                    MaxTotalRank = MAX(MaxTotalRank, excluded.MaxTotalRank),
                    ClearTime = excluded.ClearTime;
                """;
            cmd.Parameters.AddWithValue("$u", unitUid);
            cmd.Parameters.AddWithValue("$d", dungeonId);
            cmd.Parameters.AddWithValue("$s", maxScore);
            cmd.Parameters.AddWithValue("$r", (int)maxTotalRank);
            cmd.Parameters.AddWithValue("$t", nowStr);
            cmd.ExecuteNonQuery();
        }
    }

    public long? FindUnitUidByNickName(RegionProfile region, string nickName)
    {
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT UnitUID FROM Units WHERE RegionCode=$r AND NickName=$n AND IsDeleted=0 LIMIT 1;";
        cmd.Parameters.AddWithValue("$r", region.Code);
        cmd.Parameters.AddWithValue("$n", nickName);
        var val = cmd.ExecuteScalar();
        return val == null || val == DBNull.Value ? null : Convert.ToInt64(val);
    }

    public long? GetAccountUidByUnitUid(long unitUid)
    {
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT UserUID FROM Units WHERE UnitUID=$u AND IsDeleted=0 LIMIT 1;";
        cmd.Parameters.AddWithValue("$u", unitUid);
        var val = cmd.ExecuteScalar();
        return val == null || val == DBNull.Value ? null : Convert.ToInt64(val);
    }

    public KUnitInfo? GetUnitByNickName(RegionProfile region, string nickName)
    {
        long? uid = FindUnitUidByNickName(region, nickName);
        return uid.HasValue ? GetUnitByUid(region, uid.Value) : null;
    }

    // ---------------- Inventory & Shop ----------------

    private static long InsertItemInternal(SqliteConnection conn, long unitUid, sbyte slotCategory, short slotId, int itemId, int qty)
    {
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            INSERT INTO InventoryItems (UnitUID, SlotCategory, SlotID, ItemID, UsageType, Quantity, Endurance, EnchantLevel)
            VALUES ($u, $cat, $slot, $item, 1, $qty, 100, 0);
            SELECT last_insert_rowid();
            """;
        cmd.Parameters.AddWithValue("$u", unitUid);
        cmd.Parameters.AddWithValue("$cat", (int)slotCategory);
        cmd.Parameters.AddWithValue("$slot", (int)slotId);
        cmd.Parameters.AddWithValue("$item", itemId);
        cmd.Parameters.AddWithValue("$qty", qty);
        return Convert.ToInt64(cmd.ExecuteScalar());
    }

    public List<KInventoryItemInfo> GetInventory(long unitUid)
    {
        var list = new List<KInventoryItemInfo>();
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT ItemUID, SlotCategory, SlotID, ItemID, UsageType, Quantity, Endurance, EnchantLevel, IsBroken, IsSealed, SocketIds, AttribEnchant, RandomSocketIds, Period, ExpirationDate
            FROM InventoryItems WHERE UnitUID=$u ORDER BY SlotCategory, SlotID;
            """;
        cmd.Parameters.AddWithValue("$u", unitUid);
        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            bool isBroken = !r.IsDBNull(8) && r.GetInt32(8) != 0;
            bool isSealed = !r.IsDBNull(9) && r.GetInt32(9) != 0;
            string sockRaw = r.IsDBNull(10) ? "" : r.GetString(10);
            var sockets = sockRaw.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => int.TryParse(s, out int v) ? v : 0)
                .ToList();
            string attrRaw = r.IsDBNull(11) ? "0,0,0" : r.GetString(11);
            var attrs = attrRaw.Split(',').Select(s => sbyte.TryParse(s, out sbyte v) ? v : (sbyte)0).ToList();
            while (attrs.Count < 3) attrs.Add(0);
            string randSockRaw = r.IsDBNull(12) ? "" : r.GetString(12);
            var randomSockets = randSockRaw.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => int.TryParse(s, out int v) ? v : 0)
                .ToList();
            short period = r.IsDBNull(13) ? (short)0 : (short)r.GetInt32(13);
            string expirationDate = r.IsDBNull(14) ? string.Empty : r.GetString(14);

            list.Add(new KInventoryItemInfo
            {
                ItemUid = r.GetInt64(0),
                SlotCategory = (sbyte)r.GetInt32(1),
                SlotId = (short)r.GetInt32(2),
                ItemInfo = new KItemInfo
                {
                    ItemId = r.GetInt32(3),
                    UsageType = (sbyte)r.GetInt32(4),
                    Quantity = r.GetInt32(5),
                    Endurance = (short)r.GetInt32(6),
                    EnchantLevel = (sbyte)r.GetInt32(7),
                    SealData = (byte)(isSealed ? 1 : 0),
                    ItemState = (sbyte)(isBroken ? 2 : 1),
                    ItemSockets = sockets,
                    RandomSockets = randomSockets,
                    Period = period,
                    ExpirationDate = expirationDate,
                    AttribEnchantInfo = new KItemAttributeEnchantInfo
                    {
                        AttribEnchant0 = attrs[0],
                        AttribEnchant1 = attrs[1],
                        AttribEnchant2 = attrs[2],
                    },
                },
            });
        }
        return list;
    }

    public List<KInventoryItemInfo> MoveInventorySlot(long unitUid, sbyte fromCat, int fromSlot, sbyte toCat, int toSlot)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                UPDATE InventoryItems
                SET SlotCategory=$tc, SlotID=$ts
                WHERE UnitUID=$u AND SlotCategory=$fc AND SlotID=$fs;
                """;
            cmd.Parameters.AddWithValue("$u", unitUid);
            cmd.Parameters.AddWithValue("$fc", (int)fromCat);
            cmd.Parameters.AddWithValue("$fs", fromSlot);
            cmd.Parameters.AddWithValue("$tc", (int)toCat);
            cmd.Parameters.AddWithValue("$ts", toSlot);
            cmd.ExecuteNonQuery();
            return GetInventory(unitUid);
        }
    }

    public (bool ok, int newEd, List<KInventoryItemInfo> added) BuyEdItems(long unitUid, Dictionary<int, int> items, int unitPriceEd = 500)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var edCmd = conn.CreateCommand();
            edCmd.CommandText = "SELECT ED FROM Units WHERE UnitUID=$u;";
            edCmd.Parameters.AddWithValue("$u", unitUid);
            var edObj = edCmd.ExecuteScalar();
            if (edObj == null) return (false, 0, new());
            int curEd = Convert.ToInt32(edObj);

            long totalCost = items.Sum(kv => (long)Math.Max(1, kv.Value) * unitPriceEd);
            if (curEd < totalCost) return (false, curEd, new());

            int newEd = (int)(curEd - totalCost);
            using var upd = conn.CreateCommand();
            upd.CommandText = "UPDATE Units SET ED=$ed WHERE UnitUID=$u;";
            upd.Parameters.AddWithValue("$ed", newEd);
            upd.Parameters.AddWithValue("$u", unitUid);
            upd.ExecuteNonQuery();

            using var maxSlotCmd = conn.CreateCommand();
            maxSlotCmd.CommandText = "SELECT COALESCE(MAX(SlotID), -1) FROM InventoryItems WHERE UnitUID=$u AND SlotCategory=1;";
            maxSlotCmd.Parameters.AddWithValue("$u", unitUid);
            short nextSlot = (short)(Convert.ToInt32(maxSlotCmd.ExecuteScalar()) + 1);

            var added = new List<KInventoryItemInfo>();
            foreach (var kv in items)
            {
                int qty = Math.Max(1, kv.Value);
                long iuid = InsertItemInternal(conn, unitUid, 1, nextSlot, kv.Key, qty);
                added.Add(new KInventoryItemInfo
                {
                    ItemUid = iuid,
                    SlotCategory = 1,
                    SlotId = nextSlot,
                    ItemInfo = new KItemInfo { ItemId = kv.Key, Quantity = qty },
                });
                nextSlot++;
            }
            return (true, newEd, added);
        }
    }

    public (bool ok, int newEd, KInventoryItemInfo sold) SellEdItem(long unitUid, long itemUid, int quantity, int sellPriceEd = 250)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var sel = conn.CreateCommand();
            sel.CommandText = "SELECT ItemID, Quantity, SlotCategory, SlotID FROM InventoryItems WHERE ItemUID=$i AND UnitUID=$u;";
            sel.Parameters.AddWithValue("$i", itemUid);
            sel.Parameters.AddWithValue("$u", unitUid);
            using var r = sel.ExecuteReader();
            if (!r.Read()) return (false, 0, new());
            int itemId = r.GetInt32(0);
            int curQty = r.GetInt32(1);
            sbyte cat = (sbyte)r.GetInt32(2);
            short slot = (short)r.GetInt32(3);
            r.Close();

            int sellQty = Math.Clamp(quantity, 1, curQty);
            int remQty = curQty - sellQty;
            if (remQty <= 0)
            {
                using var del = conn.CreateCommand();
                del.CommandText = "DELETE FROM InventoryItems WHERE ItemUID=$i;";
                del.Parameters.AddWithValue("$i", itemUid);
                del.ExecuteNonQuery();
            }
            else
            {
                using var upd = conn.CreateCommand();
                upd.CommandText = "UPDATE InventoryItems SET Quantity=$q WHERE ItemUID=$i;";
                upd.Parameters.AddWithValue("$q", remQty);
                upd.Parameters.AddWithValue("$i", itemUid);
                upd.ExecuteNonQuery();
            }

            int gain = sellQty * sellPriceEd;
            using var edUpd = conn.CreateCommand();
            edUpd.CommandText = "UPDATE Units SET ED = ED + $g WHERE UnitUID=$u; SELECT ED FROM Units WHERE UnitUID=$u;";
            edUpd.Parameters.AddWithValue("$g", gain);
            edUpd.Parameters.AddWithValue("$u", unitUid);
            int newEd = Convert.ToInt32(edUpd.ExecuteScalar());

            return (true, newEd, new KInventoryItemInfo
            {
                ItemUid = itemUid,
                SlotCategory = cat,
                SlotId = slot,
                ItemInfo = new KItemInfo { ItemId = remQty > 0 ? itemId : 0, Quantity = remQty },
            });
        }
    }

    // ---------------- Mail ----------------

    public long SendLetter(RegionProfile region, long fromUnitUid, string fromNick, long toUnitUid, string title, string message, int itemId, int qty, int ed, sbyte scriptType = 1)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                INSERT INTO MailLetters (RegionCode, FromUnitUID, FromNickName, ToUnitUID, Title, Message, ItemID, Quantity, ED, ScriptType)
                VALUES ($r, $fu, $fn, $tu, $t, $m, $item, $qty, $ed, $st);
                SELECT last_insert_rowid();
                """;
            cmd.Parameters.AddWithValue("$r", region.Code);
            cmd.Parameters.AddWithValue("$fu", fromUnitUid);
            cmd.Parameters.AddWithValue("$fn", fromNick);
            cmd.Parameters.AddWithValue("$tu", toUnitUid);
            cmd.Parameters.AddWithValue("$t", title);
            cmd.Parameters.AddWithValue("$m", message);
            cmd.Parameters.AddWithValue("$item", itemId);
            cmd.Parameters.AddWithValue("$qty", qty);
            cmd.Parameters.AddWithValue("$ed", ed);
            cmd.Parameters.AddWithValue("$st", scriptType);
            return Convert.ToInt64(cmd.ExecuteScalar());
        }
    }

    public List<KPostItemInfo> GetLettersForUnit(long toUnitUid)
    {
        var list = new List<KPostItemInfo>();
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT PostNo, FromUnitUID, FromNickName, Title, Message, ItemID, Quantity, IsRead, SentAt, ScriptType, ED
            FROM MailLetters WHERE ToUnitUID=$u ORDER BY PostNo ASC LIMIT 200;
            """;
        cmd.Parameters.AddWithValue("$u", toUnitUid);
        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            int itemId = r.GetInt32(5);
            int qty = r.GetInt32(6);
            int ed = r.GetInt32(10);
            if (itemId <= 0 && ed > 0)
            {
                itemId = 127000; // CXSLItem::EDI_BRONZE_ED
                qty = ed;
            }
            list.Add(new KPostItemInfo
            {
                PostNo = r.GetInt64(0),
                FromUnitUid = r.GetInt64(1),
                ToUnitUid = toUnitUid,
                FromNickName = r.GetString(2),
                Title = r.GetString(3),
                Message = r.GetString(4),
                ScriptIndex = itemId,
                Quantity = qty,
                Read = r.GetInt32(7) != 0,
                RegDate = r.GetString(8),
                ScriptType = (sbyte)r.GetInt32(9),
            });
        }
        return list;
    }

    public KPostItemInfo? ReadLetter(long toUnitUid, long postNo)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var upd = conn.CreateCommand();
            upd.CommandText = "UPDATE MailLetters SET IsRead=1 WHERE PostNo=$p AND ToUnitUID=$u;";
            upd.Parameters.AddWithValue("$p", postNo);
            upd.Parameters.AddWithValue("$u", toUnitUid);
            upd.ExecuteNonQuery();

            using var sel = conn.CreateCommand();
            sel.CommandText = """
                SELECT PostNo, FromUnitUID, FromNickName, Title, Message, ItemID, Quantity, IsRead, SentAt, ScriptType, ED
                FROM MailLetters WHERE PostNo=$p AND ToUnitUID=$u LIMIT 1;
                """;
            sel.Parameters.AddWithValue("$p", postNo);
            sel.Parameters.AddWithValue("$u", toUnitUid);
            using var r = sel.ExecuteReader();
            if (!r.Read()) return null;
            int itemId = r.GetInt32(5);
            int qty = r.GetInt32(6);
            int ed = r.GetInt32(10);
            if (itemId <= 0 && ed > 0)
            {
                itemId = 127000;
                qty = ed;
            }
            return new KPostItemInfo
            {
                PostNo = r.GetInt64(0),
                FromUnitUid = r.GetInt64(1),
                ToUnitUid = toUnitUid,
                FromNickName = r.GetString(2),
                Title = r.GetString(3),
                Message = r.GetString(4),
                ScriptIndex = itemId,
                Quantity = qty,
                Read = r.GetInt32(7) != 0,
                RegDate = r.GetString(8),
                ScriptType = (sbyte)r.GetInt32(9),
            };
        }
    }

    public bool ClaimLetterAttachment(long toUnitUid, long postNo)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                UPDATE MailLetters
                SET IsClaimed=1, ItemID=0, Quantity=0, ED=0
                WHERE PostNo=$p AND ToUnitUID=$u AND IsClaimed=0;
                """;
            cmd.Parameters.AddWithValue("$p", postNo);
            cmd.Parameters.AddWithValue("$u", toUnitUid);
            return cmd.ExecuteNonQuery() > 0;
        }
    }

    public int DeleteLetters(long toUnitUid, IEnumerable<long> postNos)
    {
        int removed = 0;
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var tx = conn.BeginTransaction();
            foreach (long pno in postNos)
            {
                using var cmd = conn.CreateCommand();
                cmd.Transaction = tx;
                cmd.CommandText = "DELETE FROM MailLetters WHERE PostNo=$p AND ToUnitUID=$u;";
                cmd.Parameters.AddWithValue("$p", pno);
                cmd.Parameters.AddWithValue("$u", toUnitUid);
                removed += cmd.ExecuteNonQuery();
            }
            tx.Commit();
        }
        return removed;
    }

    public bool HasTodayEventLetter(long toUnitUid, sbyte scriptType = KPostItemInfo.LtEvent)
    {
        if (toUnitUid <= 0)
            return false;

        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT 1 FROM MailLetters
            WHERE ToUnitUID=$u AND ScriptType=$st AND date(SentAt)=date('now')
            LIMIT 1;
            """;
        cmd.Parameters.AddWithValue("$u", toUnitUid);
        cmd.Parameters.AddWithValue("$st", scriptType);
        return cmd.ExecuteScalar() != null;
    }

    public (bool ok, KInventoryItemInfo updatedSlot) DeleteInventoryItem(long unitUid, long itemUid, int quantity)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var sel = conn.CreateCommand();
            sel.CommandText = "SELECT ItemID, Quantity, SlotCategory, SlotID FROM InventoryItems WHERE ItemUID=$i AND UnitUID=$u LIMIT 1;";
            sel.Parameters.AddWithValue("$i", itemUid);
            sel.Parameters.AddWithValue("$u", unitUid);
            using var r = sel.ExecuteReader();
            if (!r.Read()) return (false, new KInventoryItemInfo());
            int itemId = r.GetInt32(0);
            int curQty = r.GetInt32(1);
            sbyte slotCat = (sbyte)r.GetInt32(2);
            short slotId = (short)r.GetInt32(3);
            r.Close();

            int rem = curQty - Math.Max(1, quantity);
            if (rem <= 0)
            {
                using var del = conn.CreateCommand();
                del.CommandText = "DELETE FROM InventoryItems WHERE ItemUID=$i AND UnitUID=$u;";
                del.Parameters.AddWithValue("$i", itemUid);
                del.Parameters.AddWithValue("$u", unitUid);
                del.ExecuteNonQuery();
                return (true, new KInventoryItemInfo { ItemUid = itemUid, SlotCategory = slotCat, SlotId = slotId });
            }

            using var upd = conn.CreateCommand();
            upd.CommandText = "UPDATE InventoryItems SET Quantity=$q WHERE ItemUID=$i AND UnitUID=$u;";
            upd.Parameters.AddWithValue("$q", rem);
            upd.Parameters.AddWithValue("$i", itemUid);
            upd.Parameters.AddWithValue("$u", unitUid);
            upd.ExecuteNonQuery();
            return (true, new KInventoryItemInfo
            {
                ItemUid = itemUid,
                SlotCategory = slotCat,
                SlotId = slotId,
                ItemInfo = new KItemInfo { ItemId = itemId, Quantity = rem },
            });
        }
    }

    // ---------------- Guilds ----------------

    public (bool ok, KGuildInfo? guild, int netError) CreateGuild(RegionProfile region, long masterUnitUid, string guildName, string guildMessage)
    {
        if (string.IsNullOrWhiteSpace(guildName))
            return (false, null, region.ResolveNetError(NetErrorKey.EmptyId));

        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var chk = conn.CreateCommand();
            chk.CommandText = "SELECT GuildUID FROM Guilds WHERE RegionCode=$r AND GuildName=$gn LIMIT 1;";
            chk.Parameters.AddWithValue("$r", region.Code);
            chk.Parameters.AddWithValue("$gn", guildName);
            if (chk.ExecuteScalar() != null)
                return (false, null, region.ResolveNetError(NetErrorKey.DuplicateId));

            using var ins = conn.CreateCommand();
            ins.CommandText = """
                INSERT INTO Guilds (RegionCode, GuildName, GuildMessage, MasterUnitUID)
                VALUES ($r, $gn, $gm, $mu);
                SELECT last_insert_rowid();
                """;
            ins.Parameters.AddWithValue("$r", region.Code);
            ins.Parameters.AddWithValue("$gn", guildName);
            ins.Parameters.AddWithValue("$gm", guildMessage);
            ins.Parameters.AddWithValue("$mu", masterUnitUid);
            int guildUid = Convert.ToInt32(ins.ExecuteScalar());

            using var mem = conn.CreateCommand();
            mem.CommandText = "INSERT OR REPLACE INTO GuildMembers (GuildUID, UnitUID, MemberGrade) VALUES ($g, $u, 1);";
            mem.Parameters.AddWithValue("$g", guildUid);
            mem.Parameters.AddWithValue("$u", masterUnitUid);
            mem.ExecuteNonQuery();

            using var uupd = conn.CreateCommand();
            uupd.CommandText = "UPDATE Units SET GuildUID=$g WHERE UnitUID=$u;";
            uupd.Parameters.AddWithValue("$g", guildUid);
            uupd.Parameters.AddWithValue("$u", masterUnitUid);
            uupd.ExecuteNonQuery();

            return (true, new KGuildInfo
            {
                GuildUid = guildUid,
                GuildName = guildName,
                GuildMessage = guildMessage,
                GuildLevel = 1,
                MaxNumMember = 30,
                FoundingDay = DateTime.UtcNow.ToString("yyyy-MM-dd"),
            }, NetError.NET_OK);
        }
    }

    // ---------------- Quests ----------------

    public void AcceptQuest(long unitUid, int questId)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                INSERT OR REPLACE INTO Quests (UnitUID, QuestID, Status, Progress, UpdatedAt)
                VALUES ($u, $q, 1, 0, datetime('now'));
                """;
            cmd.Parameters.AddWithValue("$u", unitUid);
            cmd.Parameters.AddWithValue("$q", questId);
            cmd.ExecuteNonQuery();
        }
    }

    public bool CompleteQuest(long unitUid, int questId)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE Quests SET Status=2, Progress=100, UpdatedAt=datetime('now') WHERE UnitUID=$u AND QuestID=$q;";
            cmd.Parameters.AddWithValue("$u", unitUid);
            cmd.Parameters.AddWithValue("$q", questId);
            return cmd.ExecuteNonQuery() > 0;
        }
    }

    public void GiveUpQuest(long unitUid, int questId)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM Quests WHERE UnitUID=$u AND QuestID=$q AND Status=1;";
            cmd.Parameters.AddWithValue("$u", unitUid);
            cmd.Parameters.AddWithValue("$q", questId);
            cmd.ExecuteNonQuery();
        }
    }

    public (List<KQuestInstance> active, List<(int questId, DateTime completedUtc)> completed) GetUnitQuests(long unitUid)
    {
        var active = new List<KQuestInstance>();
        var completed = new List<(int questId, DateTime completedUtc)>();
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT QuestID, Status, UpdatedAt FROM Quests WHERE UnitUID=$u;";
        cmd.Parameters.AddWithValue("$u", unitUid);
        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            int questId = r.GetInt32(0);
            int status = r.GetInt32(1);
            DateTime updatedUtc = r.IsDBNull(2) || !DateTime.TryParse(r.GetString(2), out var dt)
                ? DateTime.UtcNow
                : DateTime.SpecifyKind(dt, DateTimeKind.Utc);
            if (status == 2)
                completed.Add((questId, updatedUtc));
            else
                active.Add(new KQuestInstance { Id = questId, OwnerUnitUid = unitUid });
        }
        return (active, completed);
    }

    // ---------------- Second Security (2nd PIN) ----------------

    public (bool useSecondPw, int failedCount) GetSecondSecurityStatus(long userUid)
    {
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT UseSecondPW, FailedCount FROM SecondSecurity WHERE UserUID=$u LIMIT 1;";
        cmd.Parameters.AddWithValue("$u", userUid);
        using var r = cmd.ExecuteReader();
        if (!r.Read()) return (false, 0);
        return (r.GetInt32(0) != 0, r.GetInt32(1));
    }

    public void CreateSecondSecurityPin(long userUid, string pin)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                INSERT INTO SecondSecurity (UserUID, UseSecondPW, SecondPWHash, FailedCount, UpdatedAt)
                VALUES ($u, 1, $h, 0, datetime('now'))
                ON CONFLICT(UserUID) DO UPDATE SET UseSecondPW=1, SecondPWHash=$h, FailedCount=0, UpdatedAt=datetime('now');
                """;
            cmd.Parameters.AddWithValue("$u", userUid);
            cmd.Parameters.AddWithValue("$h", HashPassword(pin));
            cmd.ExecuteNonQuery();
        }
    }

    public (bool ok, int failedCount) AuthSecondSecurityPin(long userUid, string pin)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var sel = conn.CreateCommand();
            sel.CommandText = "SELECT UseSecondPW, SecondPWHash, FailedCount FROM SecondSecurity WHERE UserUID=$u LIMIT 1;";
            sel.Parameters.AddWithValue("$u", userUid);
            using var r = sel.ExecuteReader();
            if (!r.Read()) return (true, 0);
            bool enabled = r.GetInt32(0) != 0;
            string expected = r.GetString(1);
            int failed = r.GetInt32(2);
            r.Close();

            if (!enabled) return (true, 0);
            if (string.Equals(expected, HashPassword(pin), StringComparison.Ordinal))
            {
                using var reset = conn.CreateCommand();
                reset.CommandText = "UPDATE SecondSecurity SET FailedCount=0, UpdatedAt=datetime('now') WHERE UserUID=$u;";
                reset.Parameters.AddWithValue("$u", userUid);
                reset.ExecuteNonQuery();
                return (true, 0);
            }

            failed++;
            using var upd = conn.CreateCommand();
            upd.CommandText = "UPDATE SecondSecurity SET FailedCount=$f, UpdatedAt=datetime('now') WHERE UserUID=$u;";
            upd.Parameters.AddWithValue("$f", failed);
            upd.Parameters.AddWithValue("$u", userUid);
            upd.ExecuteNonQuery();
            return (false, failed);
        }
    }

    public void DeleteSecondSecurityPin(long userUid)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE SecondSecurity SET UseSecondPW=0, SecondPWHash='', FailedCount=0, UpdatedAt=datetime('now') WHERE UserUID=$u;";
            cmd.Parameters.AddWithValue("$u", userUid);
            cmd.ExecuteNonQuery();
        }
    }

    // ---------------- Account Block Polling ----------------

    public List<(string UserId, int BlockType, string Reason)> GetAccountBlockList(RegionProfile region)
    {
        var list = new List<(string, int, string)>();
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT UserID, BlockType, Reason FROM AccountBlockEntries WHERE RegionCode=$r AND IsBlocked=1;";
        cmd.Parameters.AddWithValue("$r", region.Code);
        using var r = cmd.ExecuteReader();
        while (r.Read())
            list.Add((r.GetString(0), r.GetInt32(1), r.GetString(2)));
        return list;
    }

    public void SetAccountBlock(RegionProfile region, string userId, int blockType, bool blocked, string reason = "")
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                INSERT INTO AccountBlockEntries (RegionCode, UserID, BlockType, IsBlocked, Reason, UpdatedAt)
                VALUES ($r, $u, $bt, $b, $rs, datetime('now'))
                ON CONFLICT(RegionCode, UserID, BlockType) DO UPDATE SET IsBlocked=$b, Reason=$rs, UpdatedAt=datetime('now');
                """;
            cmd.Parameters.AddWithValue("$r", region.Code);
            cmd.Parameters.AddWithValue("$u", userId);
            cmd.Parameters.AddWithValue("$bt", blockType);
            cmd.Parameters.AddWithValue("$b", blocked ? 1 : 0);
            cmd.Parameters.AddWithValue("$rs", reason);
            cmd.ExecuteNonQuery();
        }
    }

    // ---------------- Extended Guilds & Personal Shop ----------------

    public KGuildInfo? GetGuildInfo(RegionProfile region, int guildUid)
    {
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT GuildUID, GuildName, GuildMessage, GuildLevel, GuildExp, MaxMembers, CreatedAt FROM Guilds WHERE RegionCode=$r AND GuildUID=$g LIMIT 1;";
        cmd.Parameters.AddWithValue("$r", region.Code);
        cmd.Parameters.AddWithValue("$g", guildUid);
        using var r = cmd.ExecuteReader();
        if (!r.Read()) return null;
        return new KGuildInfo
        {
            GuildUid = r.GetInt32(0),
            GuildName = r.GetString(1),
            GuildMessage = r.GetString(2),
            GuildLevel = (byte)Math.Clamp(r.GetInt32(3), 1, 255),
            GuildExp = r.GetInt32(4),
            MaxNumMember = (ushort)Math.Clamp(r.GetInt32(5), 1, 500),
            FoundingDay = r.GetString(6),
        };
    }

    public (bool ok, ushort newMaxMembers) ExpandGuildMaxMembers(RegionProfile region, int guildUid, int delta, ushort cap = 200)
    {
        lock (_writeLock)
        {
            var guild = GetGuildInfo(region, guildUid);
            if (guild == null) return (false, 0);
            if (guild.MaxNumMember >= cap) return (false, guild.MaxNumMember);
            ushort next = (ushort)Math.Min(cap, guild.MaxNumMember + Math.Max(1, delta));
            using var conn = OpenConnection();
            using var upd = conn.CreateCommand();
            upd.CommandText = "UPDATE Guilds SET MaxMembers=$m WHERE RegionCode=$r AND GuildUID=$g;";
            upd.Parameters.AddWithValue("$m", (int)next);
            upd.Parameters.AddWithValue("$r", region.Code);
            upd.Parameters.AddWithValue("$g", guildUid);
            upd.ExecuteNonQuery();
            return (true, next);
        }
    }

    public void SetGuildMaxMembers(RegionProfile region, int guildUid, ushort maxMembers)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var upd = conn.CreateCommand();
            upd.CommandText = "UPDATE Guilds SET MaxMembers=$m WHERE RegionCode=$r AND GuildUID=$g;";
            upd.Parameters.AddWithValue("$m", (int)maxMembers);
            upd.Parameters.AddWithValue("$r", region.Code);
            upd.Parameters.AddWithValue("$g", guildUid);
            upd.ExecuteNonQuery();
        }
    }

    public (bool ok, KGuildInfo? guild, int netError) JoinGuild(RegionProfile region, int guildUid, long unitUid)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            var guild = GetGuildInfo(region, guildUid);
            if (guild == null)
                return (false, null, NetError.ERR_GUILD_01);

            using var mem = conn.CreateCommand();
            mem.CommandText = "INSERT OR REPLACE INTO GuildMembers (GuildUID, UnitUID, MemberGrade) VALUES ($g, $u, 5);";
            mem.Parameters.AddWithValue("$g", guildUid);
            mem.Parameters.AddWithValue("$u", unitUid);
            mem.ExecuteNonQuery();

            using var uupd = conn.CreateCommand();
            uupd.CommandText = "UPDATE Units SET GuildUID=$g WHERE UnitUID=$u;";
            uupd.Parameters.AddWithValue("$g", guildUid);
            uupd.Parameters.AddWithValue("$u", unitUid);
            uupd.ExecuteNonQuery();
            return (true, guild, NetError.NET_OK);
        }
    }

    public bool QuitGuild(long unitUid)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var del = conn.CreateCommand();
            del.CommandText = "DELETE FROM GuildMembers WHERE UnitUID=$u;";
            del.Parameters.AddWithValue("$u", unitUid);
            int affected = del.ExecuteNonQuery();

            using var upd = conn.CreateCommand();
            upd.CommandText = "UPDATE Units SET GuildUID=0 WHERE UnitUID=$u;";
            upd.Parameters.AddWithValue("$u", unitUid);
            upd.ExecuteNonQuery();
            return affected > 0;
        }
    }

    public (bool ok, string targetNick) ChangeGuildMemberGrade(int guildUid, long targetUnitUid, byte grade)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var upd = conn.CreateCommand();
            upd.CommandText = "UPDATE GuildMembers SET MemberGrade=$gr WHERE GuildUID=$g AND UnitUID=$u;";
            upd.Parameters.AddWithValue("$gr", (int)grade);
            upd.Parameters.AddWithValue("$g", guildUid);
            upd.Parameters.AddWithValue("$u", targetUnitUid);
            if (upd.ExecuteNonQuery() == 0) return (false, string.Empty);

            using var nick = conn.CreateCommand();
            nick.CommandText = "SELECT NickName FROM Units WHERE UnitUID=$u LIMIT 1;";
            nick.Parameters.AddWithValue("$u", targetUnitUid);
            var n = nick.ExecuteScalar() as string ?? string.Empty;
            return (true, n);
        }
    }

    // ---------------- PvP Results, Spirit, GM Cheats & Rankings ----------------

    public void UpdateUnitPvpResult(long unitUid, int deltaRating, int deltaRPoint, int deltaAPoint, float deltaKFactor, int addExp, bool win)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                UPDATE Units
                SET Rating = MAX(0, Rating + $dr),
                    MaxRating = MAX(MaxRating, MAX(0, Rating + $dr)),
                    RPoint = MAX(0, RPoint + $drp),
                    APoint = MAX(0, APoint + $dap),
                    KFactor = MAX(12.0, KFactor + $dk),
                    Exp = Exp + $exp,
                    Level = MIN(99, MAX(1, 1 + (Exp + $exp) / 1000)),
                    Win = Win + $w,
                    Lose = Lose + $l
                WHERE UnitUID = $u;
                """;
            cmd.Parameters.AddWithValue("$dr", deltaRating);
            cmd.Parameters.AddWithValue("$drp", deltaRPoint);
            cmd.Parameters.AddWithValue("$dap", deltaAPoint);
            cmd.Parameters.AddWithValue("$dk", (double)deltaKFactor);
            cmd.Parameters.AddWithValue("$exp", Math.Max(0, addExp));
            cmd.Parameters.AddWithValue("$w", win ? 1 : 0);
            cmd.Parameters.AddWithValue("$l", win ? 0 : 1);
            cmd.Parameters.AddWithValue("$u", unitUid);
            cmd.ExecuteNonQuery();
        }
    }

    public void UpdateUnitSpirit(long unitUid, int spirit)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE Units SET Spirit = MIN(SpiritMax, MAX(0, $s)) WHERE UnitUID=$u;";
            cmd.Parameters.AddWithValue("$s", spirit);
            cmd.Parameters.AddWithValue("$u", unitUid);
            cmd.ExecuteNonQuery();
        }
    }

    public KInventoryItemInfo GrantUnitItem(long unitUid, int itemId, int quantity, sbyte slotCategory = 1)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var maxSlotCmd = conn.CreateCommand();
            maxSlotCmd.CommandText = "SELECT COALESCE(MAX(SlotID), -1) FROM InventoryItems WHERE UnitUID=$u AND SlotCategory=$cat;";
            maxSlotCmd.Parameters.AddWithValue("$u", unitUid);
            maxSlotCmd.Parameters.AddWithValue("$cat", slotCategory);
            short nextSlot = (short)(Convert.ToInt32(maxSlotCmd.ExecuteScalar()) + 1);
            int qty = Math.Max(1, quantity);
            long iuid = InsertItemInternal(conn, unitUid, slotCategory, nextSlot, itemId, qty);
            return new KInventoryItemInfo
            {
                ItemUid = iuid,
                SlotCategory = slotCategory,
                SlotId = nextSlot,
                ItemInfo = new KItemInfo { ItemId = itemId, Quantity = qty },
            };
        }
    }

    public void SetUnitLevel(long unitUid, byte level)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            int clamped = Math.Clamp((int)level, 1, 99);
            cmd.CommandText = "UPDATE Units SET Level=$lvl, Exp=($lvl - 1) * 1000 WHERE UnitUID=$u;";
            cmd.Parameters.AddWithValue("$lvl", clamped);
            cmd.Parameters.AddWithValue("$u", unitUid);
            cmd.ExecuteNonQuery();
        }
    }

    public void SetUnitEd(long unitUid, int ed)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE Units SET ED=$ed WHERE UnitUID=$u;";
            cmd.Parameters.AddWithValue("$ed", Math.Max(0, ed));
            cmd.Parameters.AddWithValue("$u", unitUid);
            cmd.ExecuteNonQuery();
        }
    }

    public List<(long UnitUid, string NickName, int Rating, int RPoint, int Win, int Lose)> GetTopPvpRankings(RegionProfile region, int topN = 50)
    {
        var list = new List<(long, string, int, int, int, int)>();
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT UnitUID, NickName, Rating, RPoint, Win, Lose
            FROM Units WHERE RegionCode=$r AND IsDeleted=0
            ORDER BY Rating DESC, RPoint DESC, Win DESC
            LIMIT $lim;
            """;
        cmd.Parameters.AddWithValue("$r", region.Code);
        cmd.Parameters.AddWithValue("$lim", Math.Clamp(topN, 1, 200));
        using var r = cmd.ExecuteReader();
        while (r.Read())
            list.Add((r.GetInt64(0), r.GetString(1), r.GetInt32(2), r.GetInt32(3), r.GetInt32(4), r.GetInt32(5)));
        return list;
    }

    public List<(long UnitUid, string NickName, int Level, int Exp)> GetTopLevelRankings(RegionProfile region, int topN = 50)
    {
        var list = new List<(long, string, int, int)>();
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT UnitUID, NickName, Level, Exp
            FROM Units WHERE RegionCode=$r AND IsDeleted=0
            ORDER BY Level DESC, Exp DESC
            LIMIT $lim;
            """;
        cmd.Parameters.AddWithValue("$r", region.Code);
        cmd.Parameters.AddWithValue("$lim", Math.Clamp(topN, 1, 200));
        using var r = cmd.ExecuteReader();
        while (r.Read())
            list.Add((r.GetInt64(0), r.GetString(1), r.GetInt32(2), r.GetInt32(3)));
        return list;
    }

    // ---------------- Enchant / Restore / Skills / Titles / Personal Shop ----------------

    public (bool found, int itemId, byte enchantLevel, bool isBroken, sbyte slotCat, short slotId) GetInventoryItemMeta(long unitUid, long itemUid)
    {
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT ItemID, EnchantLevel, IsBroken, SlotCategory, SlotID FROM InventoryItems WHERE ItemUID=$i AND UnitUID=$u LIMIT 1;";
        cmd.Parameters.AddWithValue("$i", itemUid);
        cmd.Parameters.AddWithValue("$u", unitUid);
        using var r = cmd.ExecuteReader();
        if (!r.Read()) return (false, 0, 0, false, 0, 0);
        return (true, r.GetInt32(0), (byte)Math.Clamp(r.GetInt32(1), 0, 20), r.GetInt32(2) != 0, (sbyte)r.GetInt32(3), (short)r.GetInt32(4));
    }

    public KInventoryItemInfo UpdateItemEnchant(long unitUid, long itemUid, byte newEnchantLevel, bool isBroken)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE InventoryItems SET EnchantLevel=$e, IsBroken=$b WHERE ItemUID=$i AND UnitUID=$u;";
            cmd.Parameters.AddWithValue("$e", (int)newEnchantLevel);
            cmd.Parameters.AddWithValue("$b", isBroken ? 1 : 0);
            cmd.Parameters.AddWithValue("$i", itemUid);
            cmd.Parameters.AddWithValue("$u", unitUid);
            cmd.ExecuteNonQuery();
        }
        var meta = GetInventoryItemMeta(unitUid, itemUid);
        return new KInventoryItemInfo
        {
            ItemUid = itemUid,
            SlotCategory = meta.slotCat,
            SlotId = meta.slotId,
            ItemInfo = new KItemInfo
            {
                ItemId = meta.itemId,
                Quantity = 1,
                EnchantLevel = (sbyte)newEnchantLevel,
            },
        };
    }

    public void SetUnitSkillSlot(long unitUid, int slotId, int skillId)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                INSERT INTO UnitSkillSlots (UnitUID, SlotID, SkillID)
                VALUES ($u, $s, $sk)
                ON CONFLICT(UnitUID, SlotID) DO UPDATE SET SkillID=$sk;
                """;
            cmd.Parameters.AddWithValue("$u", unitUid);
            cmd.Parameters.AddWithValue("$s", slotId);
            cmd.Parameters.AddWithValue("$sk", skillId);
            cmd.ExecuteNonQuery();
        }
    }

    public Dictionary<int, int> GetUnitSkillSlots(long unitUid)
    {
        var map = new Dictionary<int, int>();
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT SlotID, SkillID FROM UnitSkillSlots WHERE UnitUID=$u ORDER BY SlotID;";
        cmd.Parameters.AddWithValue("$u", unitUid);
        using var r = cmd.ExecuteReader();
        while (r.Read())
            map[r.GetInt32(0)] = r.GetInt32(1);
        return map;
    }

    public void GrantUnitTitle(long unitUid, int titleId)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT OR IGNORE INTO UnitTitles (UnitUID, TitleID) VALUES ($u, $t);";
            cmd.Parameters.AddWithValue("$u", unitUid);
            cmd.Parameters.AddWithValue("$t", titleId);
            cmd.ExecuteNonQuery();
        }
    }

    public bool EquipUnitTitle(long unitUid, int titleId)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            if (titleId > 0)
            {
                using var chk = conn.CreateCommand();
                chk.CommandText = "SELECT 1 FROM UnitTitles WHERE UnitUID=$u AND TitleID=$t LIMIT 1;";
                chk.Parameters.AddWithValue("$u", unitUid);
                chk.Parameters.AddWithValue("$t", titleId);
                if (chk.ExecuteScalar() == null) return false;
            }
            using var upd = conn.CreateCommand();
            upd.CommandText = "UPDATE Units SET TitleID=$t WHERE UnitUID=$u;";
            upd.Parameters.AddWithValue("$t", titleId);
            upd.Parameters.AddWithValue("$u", unitUid);
            upd.ExecuteNonQuery();
            return true;
        }
    }

    public long CreatePersonalShopListing(RegionProfile region, long sellerUnitUid, string sellerNick, int itemId, int quantity, int priceEd)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                INSERT INTO PersonalShopListings (RegionCode, SellerUnitUID, SellerNickName, ItemID, Quantity, PriceED, IsSold)
                VALUES ($r, $su, $sn, $iid, $q, $p, 0);
                SELECT last_insert_rowid();
                """;
            cmd.Parameters.AddWithValue("$r", region.Code);
            cmd.Parameters.AddWithValue("$su", sellerUnitUid);
            cmd.Parameters.AddWithValue("$sn", sellerNick);
            cmd.Parameters.AddWithValue("$iid", itemId);
            cmd.Parameters.AddWithValue("$q", Math.Max(1, quantity));
            cmd.Parameters.AddWithValue("$p", Math.Max(1, priceEd));
            return Convert.ToInt64(cmd.ExecuteScalar());
        }
    }

    public (bool ok, int buyerNewEd, KInventoryItemInfo? granted) BuyPersonalShopListing(
        RegionProfile region, long buyerUnitUid, long listingUid, int quantity)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var sel = conn.CreateCommand();
            sel.CommandText = """
                SELECT SellerUnitUID, ItemID, Quantity, PriceED, IsSold
                FROM PersonalShopListings WHERE ListingUID=$l AND RegionCode=$r LIMIT 1;
                """;
            sel.Parameters.AddWithValue("$l", listingUid);
            sel.Parameters.AddWithValue("$r", region.Code);
            using var r = sel.ExecuteReader();
            if (!r.Read()) return (false, 0, null);
            long sellerUid = r.GetInt64(0);
            int itemId = r.GetInt32(1);
            int availQty = r.GetInt32(2);
            int unitPrice = r.GetInt32(3);
            bool isSold = r.GetInt32(4) != 0;
            r.Close();

            int buyQty = Math.Max(1, quantity);
            if (isSold || buyQty > availQty) return (false, 0, null);
            int totalCost = unitPrice * buyQty;

            using var edChk = conn.CreateCommand();
            edChk.CommandText = "SELECT ED FROM Units WHERE UnitUID=$u LIMIT 1;";
            edChk.Parameters.AddWithValue("$u", buyerUnitUid);
            int buyerEd = Convert.ToInt32(edChk.ExecuteScalar() ?? 0);
            if (buyerEd < totalCost) return (false, buyerEd, null);

            // Deduct buyer ED, credit seller ED (minus 5% agency tax), update listing, grant item
            using var bUpd = conn.CreateCommand();
            bUpd.CommandText = "UPDATE Units SET ED = ED - $c WHERE UnitUID=$u;";
            bUpd.Parameters.AddWithValue("$c", totalCost);
            bUpd.Parameters.AddWithValue("$u", buyerUnitUid);
            bUpd.ExecuteNonQuery();

            int sellerNet = (int)(totalCost * 0.95);
            using var sUpd = conn.CreateCommand();
            sUpd.CommandText = "UPDATE Units SET ED = ED + $g WHERE UnitUID=$u;";
            sUpd.Parameters.AddWithValue("$g", sellerNet);
            sUpd.Parameters.AddWithValue("$u", sellerUid);
            sUpd.ExecuteNonQuery();

            int remQty = availQty - buyQty;
            using var lUpd = conn.CreateCommand();
            lUpd.CommandText = "UPDATE PersonalShopListings SET Quantity=$q, IsSold=$s WHERE ListingUID=$l;";
            lUpd.Parameters.AddWithValue("$q", remQty);
            lUpd.Parameters.AddWithValue("$s", remQty <= 0 ? 1 : 0);
            lUpd.Parameters.AddWithValue("$l", listingUid);
            lUpd.ExecuteNonQuery();

            using var maxSlotCmd = conn.CreateCommand();
            maxSlotCmd.CommandText = "SELECT COALESCE(MAX(SlotID), -1) FROM InventoryItems WHERE UnitUID=$u AND SlotCategory=1;";
            maxSlotCmd.Parameters.AddWithValue("$u", buyerUnitUid);
            short nextSlot = (short)(Convert.ToInt32(maxSlotCmd.ExecuteScalar()) + 1);
            long newIuid = InsertItemInternal(conn, buyerUnitUid, 1, nextSlot, itemId, buyQty);

            return (true, buyerEd - totalCost, new KInventoryItemInfo
            {
                ItemUid = newIuid,
                SlotCategory = 1,
                SlotId = nextSlot,
                ItemInfo = new KItemInfo { ItemId = itemId, Quantity = buyQty },
            });
        }
    }

    // ---------------- Socket / Attrib / Resolve / Trade / Pets ----------------

    public KInventoryItemInfo UpdateItemSockets(long unitUid, long itemUid, IReadOnlyList<int> socketOptions)
    {
        string encoded = string.Join(",", socketOptions);
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE InventoryItems SET SocketIds=$s WHERE ItemUID=$i AND UnitUID=$u;";
            cmd.Parameters.AddWithValue("$s", encoded);
            cmd.Parameters.AddWithValue("$i", itemUid);
            cmd.Parameters.AddWithValue("$u", unitUid);
            cmd.ExecuteNonQuery();
        }
        var meta = GetInventoryItemMeta(unitUid, itemUid);
        return new KInventoryItemInfo
        {
            ItemUid = itemUid,
            SlotCategory = meta.slotCat,
            SlotId = meta.slotId,
            ItemInfo = new KItemInfo
            {
                ItemId = meta.itemId,
                Quantity = 1,
                EnchantLevel = (sbyte)meta.enchantLevel,
                ItemSockets = socketOptions.ToList(),
            },
        };
    }

    public KInventoryItemInfo UpdateItemAttrib(long unitUid, long itemUid, sbyte slotNo, sbyte attribId)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var sel = conn.CreateCommand();
            sel.CommandText = "SELECT AttribEnchant FROM InventoryItems WHERE ItemUID=$i AND UnitUID=$u LIMIT 1;";
            sel.Parameters.AddWithValue("$i", itemUid);
            sel.Parameters.AddWithValue("$u", unitUid);
            string cur = sel.ExecuteScalar() as string ?? "0,0,0";
            var parts = cur.Split(',').Select(s => sbyte.TryParse(s, out var v) ? v : (sbyte)0).ToList();
            while (parts.Count < 3) parts.Add(0);
            int idx = Math.Clamp((int)slotNo, 0, 2);
            parts[idx] = attribId;

            using var upd = conn.CreateCommand();
            upd.CommandText = "UPDATE InventoryItems SET AttribEnchant=$a WHERE ItemUID=$i AND UnitUID=$u;";
            upd.Parameters.AddWithValue("$a", string.Join(",", parts));
            upd.Parameters.AddWithValue("$i", itemUid);
            upd.Parameters.AddWithValue("$u", unitUid);
            upd.ExecuteNonQuery();
        }

        var meta = GetInventoryItemMeta(unitUid, itemUid);
        return new KInventoryItemInfo
        {
            ItemUid = itemUid,
            SlotCategory = meta.slotCat,
            SlotId = meta.slotId,
            ItemInfo = new KItemInfo
            {
                ItemId = meta.itemId,
                Quantity = 1,
                EnchantLevel = (sbyte)meta.enchantLevel,
                AttribEnchantInfo = new KItemAttributeEnchantInfo { AttribEnchant0 = attribId },
            },
        };
    }

    public (bool ok, List<KInventoryItemInfo> updatedSlots) ResolveInventoryItem(
        long unitUid, long targetItemUid, IReadOnlyDictionary<int, int> outputItems)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var del = conn.CreateCommand();
            del.CommandText = "DELETE FROM InventoryItems WHERE ItemUID=$i AND UnitUID=$u;";
            del.Parameters.AddWithValue("$i", targetItemUid);
            del.Parameters.AddWithValue("$u", unitUid);
            if (del.ExecuteNonQuery() == 0)
                return (false, []);

            using var maxSlotCmd = conn.CreateCommand();
            maxSlotCmd.CommandText = "SELECT COALESCE(MAX(SlotID), -1) FROM InventoryItems WHERE UnitUID=$u AND SlotCategory=1;";
            maxSlotCmd.Parameters.AddWithValue("$u", unitUid);
            short nextSlot = (short)(Convert.ToInt32(maxSlotCmd.ExecuteScalar()) + 1);

            var updated = new List<KInventoryItemInfo>
            {
                new() { ItemUid = targetItemUid, ItemInfo = new KItemInfo { ItemId = 0, Quantity = 0 } }
            };

            foreach (var kv in outputItems)
            {
                int qty = Math.Max(1, kv.Value);
                long newUid = InsertItemInternal(conn, unitUid, 1, nextSlot, kv.Key, qty);
                updated.Add(new KInventoryItemInfo
                {
                    ItemUid = newUid,
                    SlotCategory = 1,
                    SlotId = nextSlot,
                    ItemInfo = new KItemInfo { ItemId = kv.Key, Quantity = qty },
                });
                nextSlot++;
            }
            return (true, updated);
        }
    }

    public (bool ok, int unitAEd, int unitBEd, List<KInventoryItemInfo> unitAItems, List<KInventoryItemInfo> unitBItems) ExecutePlayerTrade(
        long unitAUid, int offerEdA, IReadOnlyDictionary<long, int> offerItemsA,
        long unitBUid, int offerEdB, IReadOnlyDictionary<long, int> offerItemsB)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var tx = conn.BeginTransaction();
            try
            {
                int edA = GetUnitEdInternal(conn, unitAUid);
                int edB = GetUnitEdInternal(conn, unitBUid);
                if (offerEdA < 0 || offerEdB < 0 || edA < offerEdA || edB < offerEdB)
                    return (false, edA, edB, [], []);

                var newA = new List<KInventoryItemInfo>();
                var newB = new List<KInventoryItemInfo>();

                if (!TransferTradeItemsInternal(conn, unitAUid, unitBUid, offerItemsA, newB) ||
                    !TransferTradeItemsInternal(conn, unitBUid, unitAUid, offerItemsB, newA))
                {
                    tx.Rollback();
                    return (false, edA, edB, [], []);
                }

                int nextEdA = edA - offerEdA + offerEdB;
                int nextEdB = edB - offerEdB + offerEdA;
                SetUnitEdInternal(conn, unitAUid, nextEdA);
                SetUnitEdInternal(conn, unitBUid, nextEdB);

                tx.Commit();
                return (true, nextEdA, nextEdB, newA, newB);
            }
            catch
            {
                tx.Rollback();
                return (false, 0, 0, [], []);
            }
        }
    }

    private static int GetUnitEdInternal(SqliteConnection conn, long unitUid)
    {
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT ED FROM Units WHERE UnitUID=$u LIMIT 1;";
        cmd.Parameters.AddWithValue("$u", unitUid);
        return Convert.ToInt32(cmd.ExecuteScalar() ?? 0);
    }

    private static void SetUnitEdInternal(SqliteConnection conn, long unitUid, int ed)
    {
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "UPDATE Units SET ED=$e WHERE UnitUID=$u;";
        cmd.Parameters.AddWithValue("$e", Math.Max(0, ed));
        cmd.Parameters.AddWithValue("$u", unitUid);
        cmd.ExecuteNonQuery();
    }

    private static bool TransferTradeItemsInternal(
        SqliteConnection conn, long fromUnitUid, long toUnitUid,
        IReadOnlyDictionary<long, int> items, List<KInventoryItemInfo> destAdded)
    {
        using var maxSlotCmd = conn.CreateCommand();
        maxSlotCmd.CommandText = "SELECT COALESCE(MAX(SlotID), -1) FROM InventoryItems WHERE UnitUID=$u AND SlotCategory=1;";
        maxSlotCmd.Parameters.AddWithValue("$u", toUnitUid);
        short nextSlot = (short)(Convert.ToInt32(maxSlotCmd.ExecuteScalar()) + 1);

        foreach (var kv in items)
        {
            using var sel = conn.CreateCommand();
            sel.CommandText = "SELECT ItemID, Quantity FROM InventoryItems WHERE ItemUID=$i AND UnitUID=$u LIMIT 1;";
            sel.Parameters.AddWithValue("$i", kv.Key);
            sel.Parameters.AddWithValue("$u", fromUnitUid);
            using var r = sel.ExecuteReader();
            if (!r.Read()) return false;
            int itemId = r.GetInt32(0);
            int curQty = r.GetInt32(1);
            r.Close();

            int moveQty = Math.Clamp(kv.Value, 1, curQty);
            if (moveQty == curQty)
            {
                using var del = conn.CreateCommand();
                del.CommandText = "DELETE FROM InventoryItems WHERE ItemUID=$i;";
                del.Parameters.AddWithValue("$i", kv.Key);
                del.ExecuteNonQuery();
            }
            else
            {
                using var upd = conn.CreateCommand();
                upd.CommandText = "UPDATE InventoryItems SET Quantity=$q WHERE ItemUID=$i;";
                upd.Parameters.AddWithValue("$q", curQty - moveQty);
                upd.Parameters.AddWithValue("$i", kv.Key);
                upd.ExecuteNonQuery();
            }

            long newUid = InsertItemInternal(conn, toUnitUid, 1, nextSlot, itemId, moveQty);
            destAdded.Add(new KInventoryItemInfo
            {
                ItemUid = newUid,
                SlotCategory = 1,
                SlotId = nextSlot,
                ItemInfo = new KItemInfo { ItemId = itemId, Quantity = moveQty },
            });
            nextSlot++;
        }
        return true;
    }

    public KPetInfo CreatePet(long unitUid, int petId, string petName)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                INSERT INTO UnitPets (UnitUID, PetID, PetName, EvolutionStep, Intimacy, Satiety)
                VALUES ($u, $pid, $name, 0, 0, 4800);
                SELECT last_insert_rowid();
                """;
            cmd.Parameters.AddWithValue("$u", unitUid);
            cmd.Parameters.AddWithValue("$pid", petId);
            cmd.Parameters.AddWithValue("$name", petName);
            long petUid = Convert.ToInt64(cmd.ExecuteScalar());
            return new KPetInfo { PetUid = petUid, PetId = petId, PetName = petName };
        }
    }

    public List<KPetInfo> GetPets(long unitUid)
    {
        var list = new List<KPetInfo>();
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT PetUID, PetID, PetName, EvolutionStep, Intimacy, Satiety FROM UnitPets WHERE UnitUID=$u;";
        cmd.Parameters.AddWithValue("$u", unitUid);
        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            list.Add(new KPetInfo
            {
                PetUid = r.GetInt64(0),
                PetId = r.GetInt32(1),
                PetName = r.GetString(2),
                EvolutionStep = (sbyte)r.GetInt32(3),
                Intimacy = r.GetInt32(4),
                Satiety = (short)r.GetInt32(5),
            });
        }
        return list;
    }

    public void UpdatePetStats(long petUid, int intimacy, short satiety, sbyte evolutionStep)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE UnitPets SET Intimacy=$i, Satiety=$s, EvolutionStep=$e WHERE PetUID=$p;";
            cmd.Parameters.AddWithValue("$i", intimacy);
            cmd.Parameters.AddWithValue("$s", (int)satiety);
            cmd.Parameters.AddWithValue("$e", (int)evolutionStep);
            cmd.Parameters.AddWithValue("$p", petUid);
            cmd.ExecuteNonQuery();
        }
    }

    public KRidingPetInfo CreateRidingPet(long unitUid, int ridingPetId)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                INSERT INTO UnitRidingPets (UnitUID, RidingPetID, Stamina)
                VALUES ($u, $id, 100.0);
                SELECT last_insert_rowid();
                """;
            cmd.Parameters.AddWithValue("$u", unitUid);
            cmd.Parameters.AddWithValue("$id", ridingPetId);
            long uid = Convert.ToInt64(cmd.ExecuteScalar());
            return new KRidingPetInfo { RidingPetUid = uid, RidingPetId = (ushort)ridingPetId, Stamina = 100f };
        }
    }

    public List<KRidingPetInfo> GetRidingPets(long unitUid)
    {
        var list = new List<KRidingPetInfo>();
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT RidingPetUID, RidingPetID, Stamina FROM UnitRidingPets WHERE UnitUID=$u;";
        cmd.Parameters.AddWithValue("$u", unitUid);
        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            list.Add(new KRidingPetInfo
            {
                RidingPetUid = r.GetInt64(0),
                RidingPetId = (ushort)r.GetInt32(1),
                Stamina = r.GetFloat(2),
            });
        }
        return list;
    }

    // ---------------- Friends & BlackList ----------------

    public void AddFriendship(long unitA, long unitB)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                INSERT OR IGNORE INTO Friendships (UnitUID, FriendUnitUID) VALUES ($a, $b);
                INSERT OR IGNORE INTO Friendships (UnitUID, FriendUnitUID) VALUES ($b, $a);
                """;
            cmd.Parameters.AddWithValue("$a", unitA);
            cmd.Parameters.AddWithValue("$b", unitB);
            cmd.ExecuteNonQuery();
        }
    }

    public void RemoveFriendship(long unitA, long unitB)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                DELETE FROM Friendships WHERE (UnitUID=$a AND FriendUnitUID=$b) OR (UnitUID=$b AND FriendUnitUID=$a);
                """;
            cmd.Parameters.AddWithValue("$a", unitA);
            cmd.Parameters.AddWithValue("$b", unitB);
            cmd.ExecuteNonQuery();
        }
    }

    public void SetFriendshipBlock(long unitUid, long friendUid, bool blocked)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE Friendships SET IsBlocked=$blk WHERE UnitUID=$u AND FriendUnitUID=$f;";
            cmd.Parameters.AddWithValue("$blk", blocked ? 1 : 0);
            cmd.Parameters.AddWithValue("$u", unitUid);
            cmd.Parameters.AddWithValue("$f", friendUid);
            cmd.ExecuteNonQuery();
        }
    }

    public List<(long friendUid, sbyte groupId, bool isBlocked)> GetFriendList(long unitUid)
    {
        var list = new List<(long, sbyte, bool)>();
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT FriendUnitUID, GroupID, IsBlocked FROM Friendships WHERE UnitUID=$u;";
        cmd.Parameters.AddWithValue("$u", unitUid);
        using var r = cmd.ExecuteReader();
        while (r.Read())
            list.Add((r.GetInt64(0), (sbyte)r.GetInt32(1), r.GetInt32(2) != 0));
        return list;
    }

    public void AddBlackListUser(long unitUid, long targetUnitUid, string targetNickName)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT OR REPLACE INTO BlackListUsers (UnitUID, TargetUnitUID, TargetNickName) VALUES ($u, $t, $n);";
            cmd.Parameters.AddWithValue("$u", unitUid);
            cmd.Parameters.AddWithValue("$t", targetUnitUid);
            cmd.Parameters.AddWithValue("$n", targetNickName);
            cmd.ExecuteNonQuery();
        }
    }

    public void RemoveBlackListUser(long unitUid, long targetUnitUid)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM BlackListUsers WHERE UnitUID=$u AND TargetUnitUID=$t;";
            cmd.Parameters.AddWithValue("$u", unitUid);
            cmd.Parameters.AddWithValue("$t", targetUnitUid);
            cmd.ExecuteNonQuery();
        }
    }

    public List<(long targetUid, string nickName)> GetBlackListUsers(long unitUid)
    {
        var list = new List<(long, string)>();
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT TargetUnitUID, TargetNickName FROM BlackListUsers WHERE UnitUID=$u;";
        cmd.Parameters.AddWithValue("$u", unitUid);
        using var r = cmd.ExecuteReader();
        while (r.Read())
            list.Add((r.GetInt64(0), r.GetString(1)));
        return list;
    }

    // ---------------- Pet / RidingPet / Bank & Item Helpers ----------------

    public bool RenamePet(long unitUid, long petUid, string petName)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE UnitPets SET PetName=$n WHERE PetUID=$p AND UnitUID=$u;";
            cmd.Parameters.AddWithValue("$n", petName);
            cmd.Parameters.AddWithValue("$p", petUid);
            cmd.Parameters.AddWithValue("$u", unitUid);
            return cmd.ExecuteNonQuery() > 0;
        }
    }

    public bool ReleaseRidingPet(long unitUid, long ridingPetUid)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM UnitRidingPets WHERE RidingPetUID=$r AND UnitUID=$u;";
            cmd.Parameters.AddWithValue("$r", ridingPetUid);
            cmd.Parameters.AddWithValue("$u", unitUid);
            return cmd.ExecuteNonQuery() > 0;
        }
    }

    public void UpdateRidingPetStamina(long unitUid, long ridingPetUid, float stamina)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE UnitRidingPets SET Stamina=$s WHERE RidingPetUID=$r AND UnitUID=$u;";
            cmd.Parameters.AddWithValue("$s", stamina);
            cmd.Parameters.AddWithValue("$r", ridingPetUid);
            cmd.Parameters.AddWithValue("$u", unitUid);
            cmd.ExecuteNonQuery();
        }
    }

    public (bool ok, KInventoryItemInfo slot) GetUnitItemByUid(long unitUid, long itemUid)
    {
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT SlotCategory, SlotID, ItemID, Quantity, EnchantLevel FROM InventoryItems WHERE ItemUID=$i AND UnitUID=$u LIMIT 1;";
        cmd.Parameters.AddWithValue("$i", itemUid);
        cmd.Parameters.AddWithValue("$u", unitUid);
        using var r = cmd.ExecuteReader();
        if (!r.Read()) return (false, new KInventoryItemInfo());
        return (true, new KInventoryItemInfo
        {
            ItemUid = itemUid,
            SlotCategory = (sbyte)r.GetInt32(0),
            SlotId = (short)r.GetInt32(1),
            ItemInfo = new KItemInfo
            {
                ItemId = r.GetInt32(2),
                Quantity = r.GetInt32(3),
                EnchantLevel = (sbyte)r.GetInt32(4),
            },
        });
    }

    public List<KInventoryItemInfo> GetUnitCategoryItems(long unitUid, sbyte slotCategory)
    {
        var list = new List<KInventoryItemInfo>();
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT ItemUID, SlotID, ItemID, Quantity, EnchantLevel FROM InventoryItems WHERE UnitUID=$u AND SlotCategory=$c ORDER BY SlotID ASC;";
        cmd.Parameters.AddWithValue("$u", unitUid);
        cmd.Parameters.AddWithValue("$c", (int)slotCategory);
        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            list.Add(new KInventoryItemInfo
            {
                ItemUid = r.GetInt64(0),
                SlotCategory = slotCategory,
                SlotId = (short)r.GetInt32(1),
                ItemInfo = new KItemInfo
                {
                    ItemId = r.GetInt32(2),
                    Quantity = r.GetInt32(3),
                    EnchantLevel = (sbyte)r.GetInt32(4),
                },
            });
        }
        return list;
    }

    public bool UpdateInventoryItemEnchant(long unitUid, long itemUid, sbyte newLevel)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE InventoryItems SET EnchantLevel=$lv WHERE ItemUID=$i AND UnitUID=$u;";
            cmd.Parameters.AddWithValue("$lv", (int)newLevel);
            cmd.Parameters.AddWithValue("$i", itemUid);
            cmd.Parameters.AddWithValue("$u", unitUid);
            return cmd.ExecuteNonQuery() > 0;
        }
    }

    // ---------------- Trade Board / Guild Management / Guild Skills ----------------

    public List<(long ListingUid, long SellerUnitUid, string SellerNickName, int ItemId, int Quantity, int PriceEd)> SearchPersonalShopListings(
        RegionProfile region, string finder = "")
    {
        var list = new List<(long, long, string, int, int, int)>();
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT ListingUID, SellerUnitUID, SellerNickName, ItemID, Quantity, PriceED
            FROM PersonalShopListings
            WHERE RegionCode=$r AND IsSold=0 AND Quantity > 0
              AND ($f='' OR SellerNickName LIKE $like)
            ORDER BY ListingUID DESC LIMIT 50;
            """;
        cmd.Parameters.AddWithValue("$r", region.Code);
        cmd.Parameters.AddWithValue("$f", finder ?? string.Empty);
        cmd.Parameters.AddWithValue("$like", $"%{finder}%");
        using var r = cmd.ExecuteReader();
        while (r.Read())
            list.Add((r.GetInt64(0), r.GetInt64(1), r.GetString(2), r.GetInt32(3), r.GetInt32(4), r.GetInt32(5)));
        return list;
    }

    public List<(long UnitUid, string NickName, sbyte UnitClass, byte Level, byte MemberGrade)> GetGuildMembers(int guildUid)
    {
        var list = new List<(long, string, sbyte, byte, byte)>();
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT gm.UnitUID, u.NickName, u.UnitClass, u.Level, gm.MemberGrade
            FROM GuildMembers gm
            JOIN Units u ON u.UnitUID = gm.UnitUID
            WHERE gm.GuildUID=$g
            ORDER BY gm.MemberGrade ASC, u.Level DESC;
            """;
        cmd.Parameters.AddWithValue("$g", guildUid);
        using var r = cmd.ExecuteReader();
        while (r.Read())
            list.Add((r.GetInt64(0), r.GetString(1), (sbyte)r.GetInt32(2), (byte)r.GetInt32(3), (byte)r.GetInt32(4)));
        return list;
    }

    public bool UpdateGuildMessage(int guildUid, string message)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE Guilds SET GuildMessage=$m WHERE GuildUID=$g;";
            cmd.Parameters.AddWithValue("$m", message);
            cmd.Parameters.AddWithValue("$g", guildUid);
            return cmd.ExecuteNonQuery() > 0;
        }
    }

    public bool IsGuildNameAvailable(RegionProfile region, string guildName)
    {
        if (string.IsNullOrWhiteSpace(guildName)) return false;
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT COUNT(*) FROM Guilds WHERE RegionCode=$r AND GuildName=$gn;";
        cmd.Parameters.AddWithValue("$r", region.Code);
        cmd.Parameters.AddWithValue("$gn", guildName);
        return Convert.ToInt64(cmd.ExecuteScalar()) == 0;
    }

    public bool DisbandGuild(RegionProfile region, int guildUid, long masterUnitUid)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var chk = conn.CreateCommand();
            chk.CommandText = "SELECT MasterUnitUID FROM Guilds WHERE RegionCode=$r AND GuildUID=$g LIMIT 1;";
            chk.Parameters.AddWithValue("$r", region.Code);
            chk.Parameters.AddWithValue("$g", guildUid);
            var masterObj = chk.ExecuteScalar();
            if (masterObj == null || Convert.ToInt64(masterObj) != masterUnitUid) return false;

            using var uupd = conn.CreateCommand();
            uupd.CommandText = "UPDATE Units SET GuildUID=0 WHERE GuildUID=$g;";
            uupd.Parameters.AddWithValue("$g", guildUid);
            uupd.ExecuteNonQuery();

            using var mdel = conn.CreateCommand();
            mdel.CommandText = "DELETE FROM GuildMembers WHERE GuildUID=$g;";
            mdel.Parameters.AddWithValue("$g", guildUid);
            mdel.ExecuteNonQuery();

            using var gdel = conn.CreateCommand();
            gdel.CommandText = "DELETE FROM Guilds WHERE GuildUID=$g;";
            gdel.Parameters.AddWithValue("$g", guildUid);
            return gdel.ExecuteNonQuery() > 0;
        }
    }

    public List<KGuildSkillData> GetGuildSkills(int guildUid)
    {
        var list = new List<KGuildSkillData>();
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT SkillID, SkillLevel FROM GuildSkills WHERE GuildUID=$g;";
        cmd.Parameters.AddWithValue("$g", guildUid);
        using var r = cmd.ExecuteReader();
        while (r.Read())
            list.Add(new KGuildSkillData { SkillId = (short)r.GetInt32(0), SkillLevel = (byte)r.GetInt32(1) });
        return list;
    }

    public bool LearnGuildSkill(int guildUid, int skillId)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                INSERT INTO GuildSkills (GuildUID, SkillID, SkillLevel) VALUES ($g, $s, 1)
                ON CONFLICT(GuildUID, SkillID) DO UPDATE SET SkillLevel = MIN(5, SkillLevel + 1);
                """;
            cmd.Parameters.AddWithValue("$g", guildUid);
            cmd.Parameters.AddWithValue("$s", skillId);
            return cmd.ExecuteNonQuery() > 0;
        }
    }

    public void ResetGuildSkill(int guildUid, int skillId)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM GuildSkills WHERE GuildUID=$g AND SkillID=$s;";
            cmd.Parameters.AddWithValue("$g", guildUid);
            cmd.Parameters.AddWithValue("$s", skillId);
            cmd.ExecuteNonQuery();
        }
    }

    public void InitGuildSkills(int guildUid)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM GuildSkills WHERE GuildUID=$g;";
            cmd.Parameters.AddWithValue("$g", guildUid);
            cmd.ExecuteNonQuery();
        }
    }

    public void UpsertGuildAd(RegionProfile region, int guildUid, short periodDays, string adMessage)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                INSERT INTO GuildAds (GuildUID, RegionCode, PeriodDays, AdMessage, UpdatedAt)
                VALUES ($g, $r, $p, $m, datetime('now'))
                ON CONFLICT(GuildUID) DO UPDATE SET PeriodDays=$p, AdMessage=$m, UpdatedAt=datetime('now');
                """;
            cmd.Parameters.AddWithValue("$g", guildUid);
            cmd.Parameters.AddWithValue("$r", region.Code);
            cmd.Parameters.AddWithValue("$p", (int)periodDays);
            cmd.Parameters.AddWithValue("$m", adMessage);
            cmd.ExecuteNonQuery();
        }
    }

    public List<KApplyJoinGuildInfo> GetGuildApplies(int guildUid)
    {
        var list = new List<KApplyJoinGuildInfo>();
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT ApplyUID, UnitUID, NickName, UnitClass, Level, Message, CreatedAt FROM GuildJoinApplies WHERE GuildUID=$g ORDER BY ApplyUID ASC;";
        cmd.Parameters.AddWithValue("$g", guildUid);
        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            list.Add(new KApplyJoinGuildInfo
            {
                ApplyJoinGuildUid = r.GetInt64(0),
                UnitUid = r.GetInt64(1),
                NickName = r.GetString(2),
                UnitClass = (sbyte)r.GetInt32(3),
                Level = (byte)r.GetInt32(4),
                Message = r.GetString(5),
                RegDate = r.GetString(6),
            });
        }
        return list;
    }

    public bool DeleteGuildApply(int guildUid, long unitUid)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM GuildJoinApplies WHERE GuildUID=$g AND UnitUID=$u;";
            cmd.Parameters.AddWithValue("$g", guildUid);
            cmd.Parameters.AddWithValue("$u", unitUid);
            return cmd.ExecuteNonQuery() > 0;
        }
    }

    public int ApplyJoinGuild(RegionProfile region, int guildUid, KUnitInfo unit, string message, bool oldApplyDelete)
    {
        if (guildUid <= 0 || unit.UnitUid <= 0)
            return NetError.ERR_GUILD_01;

        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var chk = conn.CreateCommand();
            chk.CommandText = "SELECT 1 FROM Guilds WHERE GuildUID=$g AND RegionCode=$r LIMIT 1;";
            chk.Parameters.AddWithValue("$g", guildUid);
            chk.Parameters.AddWithValue("$r", region.Code);
            if (chk.ExecuteScalar() == null)
                return NetError.ERR_GUILD_01;

            if (oldApplyDelete)
            {
                using var delOld = conn.CreateCommand();
                delOld.CommandText = "DELETE FROM GuildJoinApplies WHERE UnitUID=$u;";
                delOld.Parameters.AddWithValue("$u", unit.UnitUid);
                delOld.ExecuteNonQuery();
            }

            using var ins = conn.CreateCommand();
            ins.CommandText = """
                INSERT INTO GuildJoinApplies (GuildUID, UnitUID, NickName, UnitClass, Level, Message)
                VALUES ($g, $u, $n, $c, $l, $m)
                ON CONFLICT(GuildUID, UnitUID) DO UPDATE SET NickName=$n, UnitClass=$c, Level=$l, Message=$m;
                """;
            ins.Parameters.AddWithValue("$g", guildUid);
            ins.Parameters.AddWithValue("$u", unit.UnitUid);
            ins.Parameters.AddWithValue("$n", unit.NickName);
            ins.Parameters.AddWithValue("$c", (int)unit.UnitClass);
            ins.Parameters.AddWithValue("$l", (int)unit.Level);
            ins.Parameters.AddWithValue("$m", message ?? string.Empty);
            ins.ExecuteNonQuery();
            return NetError.NET_OK;
        }
    }

    // ---------------- Inventory Multi-Item Consume & Equipped Lookup ----------------

    public bool HasItemsInInventory(long unitUid, IReadOnlyDictionary<int, int> requiredItems)
    {
        using var conn = OpenConnection();
        foreach (var kv in requiredItems)
        {
            if (kv.Value <= 0) continue;
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COALESCE(SUM(Quantity), 0) FROM InventoryItems WHERE UnitUID=$u AND ItemID=$id;";
            cmd.Parameters.AddWithValue("$u", unitUid);
            cmd.Parameters.AddWithValue("$id", kv.Key);
            long total = Convert.ToInt64(cmd.ExecuteScalar() ?? 0);
            if (total < kv.Value) return false;
        }
        return true;
    }

    public (bool ok, List<KInventoryItemInfo> updatedSlots) ConsumeItemsByItemId(
        long unitUid, IReadOnlyDictionary<int, int> requiredItems)
    {
        var updated = new List<KInventoryItemInfo>();
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var tx = conn.BeginTransaction();
            foreach (var kv in requiredItems)
            {
                int remainingToTake = kv.Value;
                if (remainingToTake <= 0) continue;

                var rows = new List<(long itemUid, int qty, sbyte slotCat, short slotId)>();
                using (var sel = conn.CreateCommand())
                {
                    sel.Transaction = tx;
                    sel.CommandText = "SELECT ItemUID, Quantity, SlotCategory, SlotID FROM InventoryItems WHERE UnitUID=$u AND ItemID=$id ORDER BY ItemUID ASC;";
                    sel.Parameters.AddWithValue("$u", unitUid);
                    sel.Parameters.AddWithValue("$id", kv.Key);
                    using var r = sel.ExecuteReader();
                    while (r.Read())
                        rows.Add((r.GetInt64(0), r.GetInt32(1), (sbyte)r.GetInt32(2), (short)r.GetInt32(3)));
                }

                long available = rows.Sum(x => (long)x.qty);
                if (available < remainingToTake)
                {
                    tx.Rollback();
                    return (false, new List<KInventoryItemInfo>());
                }

                foreach (var row in rows)
                {
                    if (remainingToTake <= 0) break;
                    int take = Math.Min(row.qty, remainingToTake);
                    int left = row.qty - take;
                    remainingToTake -= take;

                    if (left <= 0)
                    {
                        using var del = conn.CreateCommand();
                        del.Transaction = tx;
                        del.CommandText = "DELETE FROM InventoryItems WHERE ItemUID=$i;";
                        del.Parameters.AddWithValue("$i", row.itemUid);
                        del.ExecuteNonQuery();
                        updated.Add(new KInventoryItemInfo { ItemUid = row.itemUid, SlotCategory = row.slotCat, SlotId = row.slotId });
                    }
                    else
                    {
                        using var upd = conn.CreateCommand();
                        upd.Transaction = tx;
                        upd.CommandText = "UPDATE InventoryItems SET Quantity=$q WHERE ItemUID=$i;";
                        upd.Parameters.AddWithValue("$q", left);
                        upd.Parameters.AddWithValue("$i", row.itemUid);
                        upd.ExecuteNonQuery();
                        updated.Add(new KInventoryItemInfo
                        {
                            ItemUid = row.itemUid,
                            SlotCategory = row.slotCat,
                            SlotId = row.slotId,
                            ItemInfo = new KItemInfo { ItemId = kv.Key, Quantity = left },
                        });
                    }
                }
            }
            tx.Commit();
            return (true, updated);
        }
    }

    public Dictionary<int, KInventoryItemInfo> GetUnitEquippedItemsMap(long unitUid)
    {
        var map = new Dictionary<int, KInventoryItemInfo>();
        foreach (var item in GetInventory(unitUid))
        {
            if (item.SlotCategory == 0) // equipped category
                map[item.SlotId] = item;
        }
        return map;
    }

    public List<KDecreaseEnduranceInfo> DecreaseEquippedItemEndurance(
        long unitUid, int enduranceDamage, int expDropNpcNum = 0, int partyNumMember = 1)
    {
        var result = new List<KDecreaseEnduranceInfo>();
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var tx = conn.BeginTransaction();
            var equipped = new List<(long ItemUid, int Endurance)>();
            using (var sel = conn.CreateCommand())
            {
                sel.Transaction = tx;
                sel.CommandText = "SELECT ItemUID, Endurance FROM InventoryItems WHERE UnitUID=$u AND SlotCategory IN (0, 2) AND Endurance > 0;";
                sel.Parameters.AddWithValue("$u", unitUid);
                using var r = sel.ExecuteReader();
                while (r.Read())
                    equipped.Add((r.GetInt64(0), r.GetInt32(1)));
            }

            foreach (var (itemUid, curEndurance) in equipped)
            {
                int dmg = enduranceDamage > 0 ? enduranceDamage : Random.Shared.Next(1, 4);
                if (expDropNpcNum > 0) dmg /= expDropNpcNum;
                if (partyNumMember > 0) dmg /= partyNumMember;
                if (dmg <= 0) dmg = 1;

                int nextEndurance = Math.Max(0, curEndurance - dmg);
                using (var upd = conn.CreateCommand())
                {
                    upd.Transaction = tx;
                    upd.CommandText = "UPDATE InventoryItems SET Endurance=$e WHERE ItemUID=$i AND UnitUID=$u;";
                    upd.Parameters.AddWithValue("$e", nextEndurance);
                    upd.Parameters.AddWithValue("$i", itemUid);
                    upd.Parameters.AddWithValue("$u", unitUid);
                    upd.ExecuteNonQuery();
                }

                result.Add(new KDecreaseEnduranceInfo
                {
                    ItemUid = itemUid,
                    Endurance = nextEndurance,
                });
            }
            tx.Commit();
        }
        return result;
    }

    public int RepairItems(long unitUid, IReadOnlyList<long> itemUids)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var tx = conn.BeginTransaction();
            int updated = 0;
            if (itemUids.Count == 0)
            {
                using var cmd = conn.CreateCommand();
                cmd.Transaction = tx;
                cmd.CommandText = "UPDATE InventoryItems SET Endurance=100 WHERE UnitUID=$u AND SlotCategory IN (0, 2) AND Endurance < 100;";
                cmd.Parameters.AddWithValue("$u", unitUid);
                updated = cmd.ExecuteNonQuery();
            }
            else
            {
                foreach (long itemUid in itemUids)
                {
                    using var cmd = conn.CreateCommand();
                    cmd.Transaction = tx;
                    cmd.CommandText = "UPDATE InventoryItems SET Endurance=100 WHERE ItemUID=$i AND UnitUID=$u;";
                    cmd.Parameters.AddWithValue("$i", itemUid);
                    cmd.Parameters.AddWithValue("$u", unitUid);
                    updated += cmd.ExecuteNonQuery();
                }
            }
            tx.Commit();
            return updated;
        }
    }

    // ---------------- Post BlackList & Love Letter ----------------

    public List<string> GetPostBlackList(long unitUid)
    {
        var list = new List<string>();
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT NickName FROM PostBlackList WHERE UnitUID=$u ORDER BY NickName ASC;";
        cmd.Parameters.AddWithValue("$u", unitUid);
        using var r = cmd.ExecuteReader();
        while (r.Read()) list.Add(r.GetString(0));
        return list;
    }

    public bool AddPostBlackList(long unitUid, string nickName)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT OR IGNORE INTO PostBlackList (UnitUID, NickName) VALUES ($u, $n);";
            cmd.Parameters.AddWithValue("$u", unitUid);
            cmd.Parameters.AddWithValue("$n", nickName);
            return cmd.ExecuteNonQuery() > 0;
        }
    }

    public bool RemovePostBlackList(long unitUid, string nickName)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM PostBlackList WHERE UnitUID=$u AND NickName=$n COLLATE NOCASE;";
            cmd.Parameters.AddWithValue("$u", unitUid);
            cmd.Parameters.AddWithValue("$n", nickName);
            return cmd.ExecuteNonQuery() > 0;
        }
    }

    public void SaveLoveLetterEvent(RegionProfile region, long unitUid, string message)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO LoveLetters (RegionCode, UnitUID, Message) VALUES ($r, $u, $m);";
            cmd.Parameters.AddWithValue("$r", region.Code);
            cmd.Parameters.AddWithValue("$u", unitUid);
            cmd.Parameters.AddWithValue("$m", message);
            cmd.ExecuteNonQuery();
        }
    }

    // ---------------- Friend Groups & Messenger SN ----------------

    public Dictionary<sbyte, string> GetFriendGroups(long unitUid)
    {
        var map = new Dictionary<sbyte, string>();
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT GroupID, GroupName FROM FriendGroups WHERE UnitUID=$u ORDER BY GroupID ASC;";
        cmd.Parameters.AddWithValue("$u", unitUid);
        using var r = cmd.ExecuteReader();
        while (r.Read()) map[(sbyte)r.GetInt32(0)] = r.GetString(1);
        return map;
    }

    public void MakeFriendGroup(long unitUid, sbyte groupId, string groupName)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT OR REPLACE INTO FriendGroups (UnitUID, GroupID, GroupName) VALUES ($u, $g, $n);";
            cmd.Parameters.AddWithValue("$u", unitUid);
            cmd.Parameters.AddWithValue("$g", (int)groupId);
            cmd.Parameters.AddWithValue("$n", groupName);
            cmd.ExecuteNonQuery();
        }
    }

    public void RenameFriendGroup(long unitUid, sbyte groupId, string groupName)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE FriendGroups SET GroupName=$n WHERE UnitUID=$u AND GroupID=$g;";
            cmd.Parameters.AddWithValue("$u", unitUid);
            cmd.Parameters.AddWithValue("$g", (int)groupId);
            cmd.Parameters.AddWithValue("$n", groupName);
            cmd.ExecuteNonQuery();
        }
    }

    public void DeleteFriendGroup(long unitUid, sbyte groupId)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                DELETE FROM FriendGroups WHERE UnitUID=$u AND GroupID=$g;
                UPDATE Friendships SET GroupID=0 WHERE UnitUID=$u AND GroupID=$g;
                """;
            cmd.Parameters.AddWithValue("$u", unitUid);
            cmd.Parameters.AddWithValue("$g", (int)groupId);
            cmd.ExecuteNonQuery();
        }
    }

    public void MoveFriendGroup(long unitUid, long friendUid, sbyte targetGroupId)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE Friendships SET GroupID=$g WHERE UnitUID=$u AND FriendUnitUID=$f;";
            cmd.Parameters.AddWithValue("$g", (int)targetGroupId);
            cmd.Parameters.AddWithValue("$u", unitUid);
            cmd.Parameters.AddWithValue("$f", friendUid);
            cmd.ExecuteNonQuery();
        }
    }

    public uint GetMessengerSerialNum(long unitUid)
        => (uint)(0x10000000u + (uint)(unitUid & 0x0FFFFFFFu));

    // ---------------- Relationships & Weddings ----------------

    public int CreateWedding(
        RegionProfile region, long groomUid, long brideUid,
        sbyte weddingHallType, sbyte officiantNpc, string weddingDateStr, string weddingMsg)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                INSERT INTO Weddings (RegionCode, GroomUID, BrideUID, WeddingHallType, OfficiantNPC, WeddingDate, WeddingMsg)
                VALUES ($r, $g, $b, $wht, $npc, $wd, $wm);
                SELECT last_insert_rowid();
                """;
            cmd.Parameters.AddWithValue("$r", region.Code);
            cmd.Parameters.AddWithValue("$g", groomUid);
            cmd.Parameters.AddWithValue("$b", brideUid);
            cmd.Parameters.AddWithValue("$wht", (int)weddingHallType);
            cmd.Parameters.AddWithValue("$npc", (int)officiantNpc);
            cmd.Parameters.AddWithValue("$wd", weddingDateStr);
            cmd.Parameters.AddWithValue("$wm", weddingMsg);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }
    }

    public KWeddingHallInfo? GetWeddingInfo(int weddingUid)
    {
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT WeddingUID, WeddingHallType, OfficiantNPC, GroomUID, BrideUID, WeddingDate, WeddingMsg, IsCompleted
            FROM Weddings WHERE WeddingUID=$w LIMIT 1;
            """;
        cmd.Parameters.AddWithValue("$w", weddingUid);
        using var r = cmd.ExecuteReader();
        if (!r.Read()) return null;
        string dateStr = r.GetString(5);
        long unix = DateTime.TryParse(dateStr, out var dt) ? new DateTimeOffset(DateTime.SpecifyKind(dt, DateTimeKind.Utc)).ToUnixTimeSeconds() : 0;
        return new KWeddingHallInfo
        {
            WeddingUid = r.GetInt32(0),
            WeddingHallType = (sbyte)r.GetInt32(1),
            OfficiantNpc = (sbyte)r.GetInt32(2),
            Groom = r.GetInt64(3),
            Bride = r.GetInt64(4),
            WeddingDateStr = dateStr,
            WeddingDate = unix,
            WeddingMsg = r.GetString(6),
            Success = r.GetInt32(7) != 0,
        };
    }

    public void CompleteWedding(int weddingUid)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE Weddings SET IsCompleted=1 WHERE WeddingUID=$w;";
            cmd.Parameters.AddWithValue("$w", weddingUid);
            cmd.ExecuteNonQuery();
        }
    }

    public void SaveRelationship(
        long unitUid, sbyte relationshipType, long loverUid, string loverNick,
        string loveWord, DateTime? anniversaryUtc, DateTime? lastRewardUtc, sbyte rewardTitleType)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                INSERT INTO UnitRelationships (UnitUID, RelationshipType, LoverUnitUID, LoverNickName, LoveWord, AnniversaryDay, LastRewardDate, RewardTitleType)
                VALUES ($u, $rt, $lu, $ln, $lw, $ad, $lr, $rtt)
                ON CONFLICT(UnitUID) DO UPDATE SET
                    RelationshipType=$rt, LoverUnitUID=$lu, LoverNickName=$ln, LoveWord=$lw,
                    AnniversaryDay=$ad, LastRewardDate=$lr, RewardTitleType=$rtt;
                """;
            cmd.Parameters.AddWithValue("$u", unitUid);
            cmd.Parameters.AddWithValue("$rt", (int)relationshipType);
            cmd.Parameters.AddWithValue("$lu", loverUid);
            cmd.Parameters.AddWithValue("$ln", loverNick);
            cmd.Parameters.AddWithValue("$lw", loveWord);
            cmd.Parameters.AddWithValue("$ad", anniversaryUtc?.ToString("O") ?? string.Empty);
            cmd.Parameters.AddWithValue("$lr", lastRewardUtc?.ToString("O") ?? string.Empty);
            cmd.Parameters.AddWithValue("$rtt", (int)rewardTitleType);
            cmd.ExecuteNonQuery();
        }
    }

    public (sbyte relType, long loverUid, string loverNick, string loveWord, DateTime? anniversaryUtc, DateTime? lastRewardUtc, sbyte rewardTitleType)? GetRelationship(long unitUid)
    {
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT RelationshipType, LoverUnitUID, LoverNickName, LoveWord, AnniversaryDay, LastRewardDate, RewardTitleType FROM UnitRelationships WHERE UnitUID=$u LIMIT 1;";
        cmd.Parameters.AddWithValue("$u", unitUid);
        using var r = cmd.ExecuteReader();
        if (!r.Read()) return null;
        DateTime? ann = DateTime.TryParse(r.GetString(4), out var aDt) ? aDt.ToUniversalTime() : null;
        DateTime? rew = DateTime.TryParse(r.GetString(5), out var rDt) ? rDt.ToUniversalTime() : null;
        return ((sbyte)r.GetInt32(0), r.GetInt64(1), r.GetString(2), r.GetString(3), ann, rew, (sbyte)r.GetInt32(6));
    }

    public void UpdateLoveWord(long unitUid, long loverUid, string loveWord)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE UnitRelationships SET LoveWord=$lw WHERE UnitUID=$u OR UnitUID=$l;";
            cmd.Parameters.AddWithValue("$lw", loveWord);
            cmd.Parameters.AddWithValue("$u", unitUid);
            cmd.Parameters.AddWithValue("$l", loverUid);
            cmd.ExecuteNonQuery();
        }
    }

    public void RecordFieldWorkingsBlockLog(
        long unitUid, sbyte x2State, int physicalMem, int virtualMem, sbyte processTerm, sbyte fieldUserNum, sbyte fieldNpcNum)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                INSERT INTO FieldWorkingsBlockLogs
                (UnitUID, X2State, PhysicalMemory, VirtualMemory, ProcessTerm, FieldUserNum, FieldNpcNum, CreatedUtc)
                VALUES ($u, $xs, $pm, $vm, $pt, $fu, $fn, $now);
                """;
            cmd.Parameters.AddWithValue("$u", unitUid);
            cmd.Parameters.AddWithValue("$xs", (int)x2State);
            cmd.Parameters.AddWithValue("$pm", physicalMem);
            cmd.Parameters.AddWithValue("$vm", virtualMem);
            cmd.Parameters.AddWithValue("$pt", (int)processTerm);
            cmd.Parameters.AddWithValue("$fu", (int)fieldUserNum);
            cmd.Parameters.AddWithValue("$fn", (int)fieldNpcNum);
            cmd.Parameters.AddWithValue("$now", DateTime.UtcNow.ToString("O"));
            cmd.ExecuteNonQuery();
        }
    }

    public void RecordGamePlayNetworkInfo(long unitUid, IReadOnlyList<KncWX2Server.Core.Packets.KGamePlayNetworkInfo> items)
    {
        if (items.Count == 0) return;
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var tx = conn.BeginTransaction();
            foreach (var item in items)
            {
                using var cmd = conn.CreateCommand();
                cmd.Transaction = tx;
                cmd.CommandText = """
                    INSERT INTO GamePlayNetworkLogs
                    (UnitUID, OpponentUnitUID, HolePunchingTryCount, P2PPlayTime, RelayPlayTime, CreatedUtc)
                    VALUES ($u, $ou, $hp, $p2p, $rel, $now);
                    """;
                cmd.Parameters.AddWithValue("$u", unitUid);
                cmd.Parameters.AddWithValue("$ou", item.OpponentUnitUid);
                cmd.Parameters.AddWithValue("$hp", item.HolePunchingTryCount);
                cmd.Parameters.AddWithValue("$p2p", item.P2PPlayTime);
                cmd.Parameters.AddWithValue("$rel", item.RelayPlayTime);
                cmd.Parameters.AddWithValue("$now", DateTime.UtcNow.ToString("O"));
                cmd.ExecuteNonQuery();
            }
            tx.Commit();
        }
    }

    public void RecordUserSystemInfo(long accountUid, long systemKey, string cpu, string memory, string gpu, string os)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                INSERT INTO UserSystemInfos (AccountUID, SystemKey, Cpu, Memory, Gpu, Os, UpdatedUtc)
                VALUES ($a, $sk, $c, $m, $g, $o, $now)
                ON CONFLICT(AccountUID) DO UPDATE SET
                    SystemKey = excluded.SystemKey,
                    Cpu = excluded.Cpu,
                    Memory = excluded.Memory,
                    Gpu = excluded.Gpu,
                    Os = excluded.Os,
                    UpdatedUtc = excluded.UpdatedUtc;
                """;
            cmd.Parameters.AddWithValue("$a", accountUid);
            cmd.Parameters.AddWithValue("$sk", systemKey);
            cmd.Parameters.AddWithValue("$c", cpu);
            cmd.Parameters.AddWithValue("$m", memory);
            cmd.Parameters.AddWithValue("$g", gpu);
            cmd.Parameters.AddWithValue("$o", os);
            cmd.Parameters.AddWithValue("$now", DateTime.UtcNow.ToString("O"));
            cmd.ExecuteNonQuery();
        }
    }

    public void RecordGamePerformanceLog(string scope, long charUid, long systemKey, int mapId, float avgFps, int partyUserNum)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                INSERT INTO GamePerformanceLogs (LogScope, CharUID, SystemKey, MapID, AvgFps, PartyUserNum, CreatedUtc)
                VALUES ($sc, $u, $sk, $m, $fps, $pun, $now);
                """;
            cmd.Parameters.AddWithValue("$sc", scope);
            cmd.Parameters.AddWithValue("$u", charUid);
            cmd.Parameters.AddWithValue("$sk", systemKey);
            cmd.Parameters.AddWithValue("$m", mapId);
            cmd.Parameters.AddWithValue("$fps", avgFps);
            cmd.Parameters.AddWithValue("$pun", partyUserNum);
            cmd.Parameters.AddWithValue("$now", DateTime.UtcNow.ToString("O"));
            cmd.ExecuteNonQuery();
        }
    }

    public void RecordHackingProcessLog(long accountUid, long unitUid, byte hackType, string reason)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                INSERT INTO UserHackingProcessLogs (AccountUID, UnitUID, HackingType, Reason, CreatedUtc)
                VALUES ($a, $u, $t, $r, $now);
                """;
            cmd.Parameters.AddWithValue("$a", accountUid);
            cmd.Parameters.AddWithValue("$u", unitUid);
            cmd.Parameters.AddWithValue("$t", (int)hackType);
            cmd.Parameters.AddWithValue("$r", reason);
            cmd.Parameters.AddWithValue("$now", DateTime.UtcNow.ToString("O"));
            cmd.ExecuteNonQuery();
        }
    }

    public void UpdateHackingUserType(long accountUid, sbyte hackingUserType)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE Accounts SET HackingUserType=$t WHERE UserUID=$a;";
            cmd.Parameters.AddWithValue("$t", (int)hackingUserType);
            cmd.Parameters.AddWithValue("$a", accountUid);
            cmd.ExecuteNonQuery();
        }
    }

    public (int ok, long unitUid, string nickName, int bankSize, Dictionary<long, KInventoryItemInfo> items) GetShareBankItems(
        RegionProfile region, long accountUid, string nickName)
    {
        var items = new Dictionary<long, KInventoryItemInfo>();
        using var conn = OpenConnection();
        using var ucmd = conn.CreateCommand();
        ucmd.CommandText = "SELECT UnitUID, NickName FROM Units WHERE RegionCode=$r AND UserUID=$a AND NickName=$n AND IsDeleted=0 LIMIT 1;";
        ucmd.Parameters.AddWithValue("$r", region.Code);
        ucmd.Parameters.AddWithValue("$a", accountUid);
        ucmd.Parameters.AddWithValue("$n", nickName);
        using var ur = ucmd.ExecuteReader();
        if (!ur.Read()) return (NetError.ERR_SEARCH_UNIT_03, 0, nickName, 0, items);
        long targetUnitUid = ur.GetInt64(0);
        string foundNick = ur.GetString(1);
        ur.Close();

        using var icmd = conn.CreateCommand();
        icmd.CommandText = """
            SELECT ItemUID, SlotCategory, SlotID, ItemID, UsageType, Quantity, Endurance, EnchantLevel
            FROM InventoryItems WHERE UnitUID=$u AND SlotCategory=8 ORDER BY SlotID;
            """;
        icmd.Parameters.AddWithValue("$u", targetUnitUid);
        using var ir = icmd.ExecuteReader();
        while (ir.Read())
        {
            var info = new KInventoryItemInfo
            {
                ItemUid = ir.GetInt64(0),
                SlotCategory = (sbyte)ir.GetInt32(1),
                SlotId = (short)ir.GetInt32(2),
                ItemInfo = new KItemInfo
                {
                    ItemId = ir.GetInt32(3),
                    UsageType = (sbyte)ir.GetInt32(4),
                    Quantity = ir.GetInt32(5),
                    Endurance = (short)ir.GetInt32(6),
                    EnchantLevel = (sbyte)ir.GetInt32(7),
                },
            };
            items[info.ItemUid] = info;
        }
        return (NetError.NET_OK, targetUnitUid, foundNick, 24, items);
    }

    public (int ok, int evalState, KInventoryItemInfo? updated) SetInventoryItemEvalState(long unitUid, long itemUid, int expectedState, int newState)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var sel = conn.CreateCommand();
            sel.CommandText = "SELECT ItemID, UsageType, Quantity, Endurance, EnchantLevel, SlotCategory, SlotID, EvalState FROM InventoryItems WHERE ItemUID=$i AND UnitUID=$u LIMIT 1;";
            sel.Parameters.AddWithValue("$i", itemUid);
            sel.Parameters.AddWithValue("$u", unitUid);
            using var r = sel.ExecuteReader();
            if (!r.Read()) return (NetError.ERR_ITEM_04, 0, null);
            int itemId = r.GetInt32(0);
            sbyte usage = (sbyte)r.GetInt32(1);
            int qty = r.GetInt32(2);
            short end = (short)r.GetInt32(3);
            sbyte ench = (sbyte)r.GetInt32(4);
            sbyte cat = (sbyte)r.GetInt32(5);
            short slotId = (short)r.GetInt32(6);
            int curState = r.GetInt32(7);
            r.Close();

            if (curState != expectedState)
                return (expectedState == 0 ? NetError.ERR_EVALUATE_ITEM_05 : NetError.ERR_EVALUATE_ITEM_07, curState, null);

            using var upd = conn.CreateCommand();
            upd.CommandText = "UPDATE InventoryItems SET EvalState=$ns WHERE ItemUID=$i AND UnitUID=$u;";
            upd.Parameters.AddWithValue("$ns", newState);
            upd.Parameters.AddWithValue("$i", itemUid);
            upd.Parameters.AddWithValue("$u", unitUid);
            upd.ExecuteNonQuery();

            return (NetError.NET_OK, newState, new KInventoryItemInfo
            {
                ItemUid = itemUid,
                SlotCategory = cat,
                SlotId = slotId,
                ItemInfo = new KItemInfo
                {
                    ItemId = itemId,
                    UsageType = usage,
                    Quantity = qty,
                    Endurance = end,
                    EnchantLevel = ench,
                },
            });
        }
    }

    public (int ok, long recruiterUid) RegisterUnitRecruiter(RegionProfile region, long unitUid, string recruiterNickName)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var chk = conn.CreateCommand();
            chk.CommandText = "SELECT RecruiterUnitUID FROM UnitRecruiters WHERE UnitUID=$u LIMIT 1;";
            chk.Parameters.AddWithValue("$u", unitUid);
            if (chk.ExecuteScalar() != null) return (NetError.ERR_RECOMMEND_USER_01, 0);

            using var find = conn.CreateCommand();
            find.CommandText = "SELECT UnitUID FROM Units WHERE RegionCode=$r AND NickName=$n AND IsDeleted=0 LIMIT 1;";
            find.Parameters.AddWithValue("$r", region.Code);
            find.Parameters.AddWithValue("$n", recruiterNickName);
            var obj = find.ExecuteScalar();
            if (obj == null) return (NetError.ERR_SEARCH_UNIT_03, 0);
            long recruiterUid = Convert.ToInt64(obj);
            if (recruiterUid == unitUid) return (NetError.ERR_RECOMMEND_USER_01, 0);

            using var ins = conn.CreateCommand();
            ins.CommandText = """
                INSERT INTO UnitRecruiters (UnitUID, RecruiterUnitUID, RecruiterNickName, HasTicket, CreatedUtc)
                VALUES ($u, $ru, $rn, 0, $now);
                """;
            ins.Parameters.AddWithValue("$u", unitUid);
            ins.Parameters.AddWithValue("$ru", recruiterUid);
            ins.Parameters.AddWithValue("$rn", recruiterNickName);
            ins.Parameters.AddWithValue("$now", DateTime.UtcNow.ToString("O"));
            ins.ExecuteNonQuery();
            return (NetError.NET_OK, recruiterUid);
        }
    }

    public void SetUnitRecruitTicketUsed(long unitUid)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                INSERT INTO UnitRecruiters (UnitUID, RecruiterUnitUID, RecruiterNickName, HasTicket, CreatedUtc)
                VALUES ($u, 0, '', 1, $now)
                ON CONFLICT(UnitUID) DO UPDATE SET HasTicket = 1;
                """;
            cmd.Parameters.AddWithValue("$u", unitUid);
            cmd.Parameters.AddWithValue("$now", DateTime.UtcNow.ToString("O"));
            cmd.ExecuteNonQuery();
        }
    }

    // -------------------------------------------------------------------------
    // CashShop / Global Billing / AdminCheat DB Helpers
    // -------------------------------------------------------------------------

    public long AddBillingOrder(
        long accountUid,
        int productNo,
        int productId,
        string productName,
        int quantity,
        int price,
        byte orderType = KBillOrderInfo.BcitBuyItem,
        string senderNickName = "",
        string giftMessage = "",
        long fromUnitUid = 0,
        byte productKind = 0)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                INSERT INTO BillingOrders
                    (AccountUID, ProductNo, ProductID, ProductName, Quantity, Price, OrderType, SenderNickName, GiftMessage, FromUnitUID, ProductKind, IsPickedUp, IsRefunded, CreatedUtc)
                VALUES
                    ($acc, $pno, $pid, $pname, $qty, $price, $ot, $sn, $msg, $from, $pk, 0, 0, $now);
                SELECT last_insert_rowid();
                """;
            cmd.Parameters.AddWithValue("$acc", accountUid);
            cmd.Parameters.AddWithValue("$pno", productNo);
            cmd.Parameters.AddWithValue("$pid", productId);
            cmd.Parameters.AddWithValue("$pname", productName);
            cmd.Parameters.AddWithValue("$qty", Math.Max(1, quantity));
            cmd.Parameters.AddWithValue("$price", Math.Max(0, price));
            cmd.Parameters.AddWithValue("$ot", orderType);
            cmd.Parameters.AddWithValue("$sn", senderNickName);
            cmd.Parameters.AddWithValue("$msg", giftMessage);
            cmd.Parameters.AddWithValue("$from", fromUnitUid);
            cmd.Parameters.AddWithValue("$pk", productKind);
            cmd.Parameters.AddWithValue("$now", DateTime.UtcNow.ToString("O"));
            return Convert.ToInt64(cmd.ExecuteScalar());
        }
    }

    public List<KBillOrderInfo> GetBillingOrders(long accountUid, int itemPerPage = 10, int currentPage = 1)
    {
        var result = new List<KBillOrderInfo>();
        int limit = Math.Clamp(itemPerPage, 1, 50);
        int offset = Math.Max(0, currentPage - 1) * limit;
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT TransNo, ProductNo, ProductID, ProductName, Quantity, Price, OrderType, SenderNickName, GiftMessage, FromUnitUID, ProductKind
            FROM BillingOrders
            WHERE AccountUID = $acc AND IsPickedUp = 0 AND IsRefunded = 0
            ORDER BY TransNo DESC
            LIMIT $lim OFFSET $off;
            """;
        cmd.Parameters.AddWithValue("$acc", accountUid);
        cmd.Parameters.AddWithValue("$lim", limit);
        cmd.Parameters.AddWithValue("$off", offset);
        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            long transNo = r.GetInt64(0);
            int pno = r.GetInt32(1);
            int pid = r.GetInt32(2);
            string pname = r.GetString(3);
            int qty = r.GetInt32(4);
            int price = r.GetInt32(5);
            byte ot = (byte)r.GetInt32(6);
            string snick = r.GetString(7);
            string msg = r.GetString(8);
            long fromUid = r.GetInt64(9);
            byte pk = (byte)r.GetInt32(10);

            result.Add(new KBillOrderInfo
            {
                TransNo = transNo,
                OrderId = $"TW-{transNo}",
                BillProductInfo = new KBillProductInfo
                {
                    ProductNo = pno,
                    ProductId = pid,
                    ProductName = pname,
                    Quantity = (sbyte)Math.Clamp(qty, 1, 127),
                    SalePrice = price,
                    RealPrice = price,
                    Show = true,
                    EnableGift = true,
                },
                Type = ot,
                SenderNickName = snick,
                SenderPresentMessage = msg,
                FromUnitUid = fromUid,
                ProductKind = pk,
            });
        }
        return result;
    }

    public (bool found, int productId, int quantity, byte productKind) MarkBillingOrderPickedUp(long accountUid, long transNo)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var sel = conn.CreateCommand();
            sel.CommandText = """
                SELECT ProductID, Quantity, ProductKind
                FROM BillingOrders
                WHERE TransNo = $tno AND AccountUID = $acc AND IsPickedUp = 0 AND IsRefunded = 0;
                """;
            sel.Parameters.AddWithValue("$tno", transNo);
            sel.Parameters.AddWithValue("$acc", accountUid);
            using var r = sel.ExecuteReader();
            if (!r.Read())
                return (false, 0, 0, 0);
            int pid = r.GetInt32(0);
            int qty = r.GetInt32(1);
            byte pk = (byte)r.GetInt32(2);
            r.Close();

            using var upd = conn.CreateCommand();
            upd.CommandText = "UPDATE BillingOrders SET IsPickedUp = 1 WHERE TransNo = $tno;";
            upd.Parameters.AddWithValue("$tno", transNo);
            upd.ExecuteNonQuery();
            return (true, pid, qty, pk);
        }
    }

    public (bool ok, int refundPrice) RefundBillingOrder(long accountUid, uint orderNo)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var sel = conn.CreateCommand();
            sel.CommandText = """
                SELECT Price FROM BillingOrders
                WHERE TransNo = $tno AND AccountUID = $acc AND IsPickedUp = 0 AND IsRefunded = 0;
                """;
            sel.Parameters.AddWithValue("$tno", (long)orderNo);
            sel.Parameters.AddWithValue("$acc", accountUid);
            var obj = sel.ExecuteScalar();
            if (obj == null)
                return (false, 0);
            int price = Convert.ToInt32(obj);

            using var upd = conn.CreateCommand();
            upd.CommandText = "UPDATE BillingOrders SET IsRefunded = 1 WHERE TransNo = $tno;";
            upd.Parameters.AddWithValue("$tno", (long)orderNo);
            upd.ExecuteNonQuery();
            return (true, price);
        }
    }

    public bool MarkBillingOrderChecked(long accountUid, uint orderNo, uint productNo)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                SELECT COUNT(1) FROM BillingOrders
                WHERE TransNo = $tno AND AccountUID = $acc AND ($pno = 0 OR ProductNo = $pno);
                """;
            cmd.Parameters.AddWithValue("$tno", (long)orderNo);
            cmd.Parameters.AddWithValue("$acc", accountUid);
            cmd.Parameters.AddWithValue("$pno", (int)productNo);
            return Convert.ToInt64(cmd.ExecuteScalar()) > 0;
        }
    }

    public Dictionary<int, int> GetWishList(long accountUid)
    {
        var map = new Dictionary<int, int>();
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT ItemID FROM CashWishLists WHERE AccountUID = $acc ORDER BY AddedUtc;";
        cmd.Parameters.AddWithValue("$acc", accountUid);
        using var r = cmd.ExecuteReader();
        int idx = 0;
        while (r.Read())
            map[r.GetInt32(0)] = idx++;
        return map;
    }

    public void ModifyWishList(long accountUid, IReadOnlyList<int> itemIds, bool isDelete)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            foreach (int itemId in itemIds)
            {
                if (itemId <= 0) continue;
                using var cmd = conn.CreateCommand();
                if (isDelete)
                {
                    cmd.CommandText = "DELETE FROM CashWishLists WHERE AccountUID = $acc AND ItemID = $iid;";
                }
                else
                {
                    cmd.CommandText = """
                        INSERT OR IGNORE INTO CashWishLists (AccountUID, ItemID, AddedUtc)
                        VALUES ($acc, $iid, $now);
                        """;
                    cmd.Parameters.AddWithValue("$now", DateTime.UtcNow.ToString("O"));
                }
                cmd.Parameters.AddWithValue("$acc", accountUid);
                cmd.Parameters.AddWithValue("$iid", itemId);
                cmd.ExecuteNonQuery();
            }
        }
    }

    public (int ok, int productNo, int itemId, int quantity) RedeemCoupon(long accountUid, string couponSerial, int couponType)
    {
        if (string.IsNullOrWhiteSpace(couponSerial) || couponSerial.Length < 4)
            return (NetError.ERR_GLOBAL_BILLING_01, 0, 0, 0);

        string normalized = couponSerial.Trim().ToUpperInvariant();
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var sel = conn.CreateCommand();
            sel.CommandText = "SELECT ProductNo, ItemID, Quantity, RedeemedByAccountUID FROM CashCoupons WHERE CouponSerial = $s;";
            sel.Parameters.AddWithValue("$s", normalized);
            using var r = sel.ExecuteReader();
            if (r.Read())
            {
                long redeemedBy = r.GetInt64(3);
                if (redeemedBy > 0)
                    return (NetError.ERR_GASH_10, 0, 0, 0);
                int pno = r.GetInt32(0);
                int iid = r.GetInt32(1);
                int qty = r.GetInt32(2);
                r.Close();

                using var upd = conn.CreateCommand();
                upd.CommandText = "UPDATE CashCoupons SET RedeemedByAccountUID = $acc, RedeemedUtc = $now WHERE CouponSerial = $s;";
                upd.Parameters.AddWithValue("$acc", accountUid);
                upd.Parameters.AddWithValue("$now", DateTime.UtcNow.ToString("O"));
                upd.Parameters.AddWithValue("$s", normalized);
                upd.ExecuteNonQuery();
                return (NetError.NET_OK, pno, iid, qty);
            }
            r.Close();

            // Allow well-formed dynamic test coupons (e.g. TW-COUPON-xxxx) once per serial
            if (normalized.StartsWith("TW-") || normalized.StartsWith("EL-"))
            {
                const int defaultCouponItemId = 130048; // Cube / coupon gift
                using var ins = conn.CreateCommand();
                ins.CommandText = """
                    INSERT INTO CashCoupons (CouponSerial, CouponType, ProductNo, ItemID, Quantity, RedeemedByAccountUID, RedeemedUtc)
                    VALUES ($s, $t, 130048, $iid, 1, $acc, $now);
                    """;
                ins.Parameters.AddWithValue("$s", normalized);
                ins.Parameters.AddWithValue("$t", couponType);
                ins.Parameters.AddWithValue("$iid", defaultCouponItemId);
                ins.Parameters.AddWithValue("$acc", accountUid);
                ins.Parameters.AddWithValue("$now", DateTime.UtcNow.ToString("O"));
                ins.ExecuteNonQuery();
                return (NetError.NET_OK, defaultCouponItemId, defaultCouponItemId, 1);
            }

            return (NetError.ERR_GLOBAL_BILLING_01, 0, 0, 0);
        }
    }

    public int GrantAccountCoupon(long accountUid, string couponSerial, int couponType, int productNo, int itemId, int quantity)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var ins = conn.CreateCommand();
            ins.CommandText = """
                INSERT OR REPLACE INTO CashCoupons (CouponSerial, CouponType, ProductNo, ItemID, Quantity, RedeemedByAccountUID, RedeemedUtc)
                VALUES ($s, $t, $p, $iid, $q, $acc, '');
                """;
            ins.Parameters.AddWithValue("$s", couponSerial.Trim().ToUpperInvariant());
            ins.Parameters.AddWithValue("$t", couponType);
            ins.Parameters.AddWithValue("$p", productNo);
            ins.Parameters.AddWithValue("$iid", itemId);
            ins.Parameters.AddWithValue("$q", Math.Max(1, quantity));
            ins.Parameters.AddWithValue("$acc", accountUid);
            ins.ExecuteNonQuery();

            using var rowIdCmd = conn.CreateCommand();
            rowIdCmd.CommandText = "SELECT rowid FROM CashCoupons WHERE CouponSerial=$s LIMIT 1;";
            rowIdCmd.Parameters.AddWithValue("$s", couponSerial.Trim().ToUpperInvariant());
            return Convert.ToInt32(rowIdCmd.ExecuteScalar() ?? 0);
        }
    }

    public List<KClientCouponBox> GetAccountCoupons(long accountUid, bool used, short couponBoxType, ushort pageNum)
    {
        var list = new List<KClientCouponBox>();
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT rowid, CouponSerial, CouponType, ProductNo, ItemID, Quantity, RedeemedUtc
            FROM CashCoupons
            WHERE RedeemedByAccountUID = $acc
              AND ($used = 1 AND RedeemedUtc <> '' OR $used = 0 AND RedeemedUtc = '')
              AND ($cbt <= 0 OR CouponType = $cbt)
            ORDER BY rowid ASC
            LIMIT 20 OFFSET $off;
            """;
        cmd.Parameters.AddWithValue("$acc", accountUid);
        cmd.Parameters.AddWithValue("$used", used ? 1 : 0);
        cmd.Parameters.AddWithValue("$cbt", (int)couponBoxType);
        cmd.Parameters.AddWithValue("$off", Math.Max(0, pageNum - 1) * 20);
        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            int cardNo = r.GetInt32(0);
            int cType = r.GetInt32(2);
            int prodNo = r.GetInt32(3);
            int itemId = r.GetInt32(4);
            string redeemedUtc = r.GetString(6);
            list.Add(new KClientCouponBox
            {
                ContractDetailName = r.GetString(1),
                ContractDetailNo = prodNo,
                CouponBoxType = cType.ToString(),
                CouponBoxTypeText = cType == 1 ? "Discount" : "Item",
                CouponCardNo = cardNo,
                CouponCardStatus = string.IsNullOrEmpty(redeemedUtc) ? 1 : 2,
                CouponCardStatusText = string.IsNullOrEmpty(redeemedUtc) ? "Unused" : "Used",
                DiscountValue = cType == 1 ? $"{prodNo}%" : string.Empty,
                UsedDateTime = redeemedUtc,
                TargetItem = itemId > 0 ? [new KCouponBoxTargetItem { ItemCode = itemId.ToString(), ItemQuantity = 1 }] : [],
            });
        }
        return list;
    }

    public int CountAccountCoupons(long accountUid, bool used, short couponBoxType)
    {
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT COUNT(*)
            FROM CashCoupons
            WHERE RedeemedByAccountUID = $acc
              AND ($used = 1 AND RedeemedUtc <> '' OR $used = 0 AND RedeemedUtc = '')
              AND ($cbt <= 0 OR CouponType = $cbt);
            """;
        cmd.Parameters.AddWithValue("$acc", accountUid);
        cmd.Parameters.AddWithValue("$used", used ? 1 : 0);
        cmd.Parameters.AddWithValue("$cbt", (int)couponBoxType);
        return Convert.ToInt32(cmd.ExecuteScalar() ?? 0);
    }

    public (int ok, int itemId, int quantity) UseAccountItemCoupon(long accountUid, int couponCardNo)
    {
        if (couponCardNo <= 0)
            return (NetError.ERR_NX_COUPON_06, 0, 0);

        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var sel = conn.CreateCommand();
            sel.CommandText = "SELECT ItemID, Quantity, RedeemedByAccountUID, RedeemedUtc FROM CashCoupons WHERE rowid=$id LIMIT 1;";
            sel.Parameters.AddWithValue("$id", couponCardNo);
            using var r = sel.ExecuteReader();
            if (!r.Read())
                return (NetError.ERR_NX_COUPON_06, 0, 0);

            int itemId = r.GetInt32(0);
            int qty = r.GetInt32(1);
            long owner = r.GetInt64(2);
            string redeemedUtc = r.GetString(3);
            r.Close();

            if ((owner != 0 && owner != accountUid) || !string.IsNullOrEmpty(redeemedUtc))
                return (NetError.ERR_NX_COUPON_06, 0, 0);

            using var upd = conn.CreateCommand();
            upd.CommandText = "UPDATE CashCoupons SET RedeemedByAccountUID=$acc, RedeemedUtc=$now WHERE rowid=$id;";
            upd.Parameters.AddWithValue("$acc", accountUid);
            upd.Parameters.AddWithValue("$now", DateTime.UtcNow.ToString("O"));
            upd.Parameters.AddWithValue("$id", couponCardNo);
            upd.ExecuteNonQuery();
            return (NetError.NET_OK, itemId, Math.Max(1, qty));
        }
    }

    public List<KDiscountCoupon> InquireDiscountCoupons(long accountUid, IEnumerable<uint> productNos)
    {
        var filter = productNos.Select(p => (int)p).ToHashSet();
        var list = new List<KDiscountCoupon>();
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT rowid, CouponSerial, ProductNo, ItemID
            FROM CashCoupons
            WHERE RedeemedByAccountUID = $acc AND CouponType = 1 AND RedeemedUtc = ''
            ORDER BY rowid ASC;
            """;
        cmd.Parameters.AddWithValue("$acc", accountUid);
        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            int targetProd = r.GetInt32(3);
            if (filter.Count > 0 && targetProd > 0 && !filter.Contains(targetProd))
                continue;
            list.Add(new KDiscountCoupon
            {
                CouponCardNo = r.GetInt32(0),
                ContractDetailName = r.GetString(1),
                DiscountValue = $"{r.GetInt32(2)}%",
                ExpireDateTime = "2099-12-31 23:59:59",
            });
        }
        return list;
    }

    public void SetUnitClass(long unitUid, sbyte unitClass)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE Units SET UnitClass = $uc WHERE UnitUID = $u;";
            cmd.Parameters.AddWithValue("$uc", unitClass);
            cmd.Parameters.AddWithValue("$u", unitUid);
            cmd.ExecuteNonQuery();
        }
    }

    public void SetUnitSkillPoint(long unitUid, int sPoint)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE Units SET SPoint = $sp WHERE UnitUID = $u;";
            cmd.Parameters.AddWithValue("$sp", Math.Max(0, sPoint));
            cmd.Parameters.AddWithValue("$u", unitUid);
            cmd.ExecuteNonQuery();
        }
    }

    public void SetUnitCashSkillPointEndDate(long unitUid, string endDate)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE Units SET CSPointEndDate = $dt WHERE UnitUID = $u;";
            cmd.Parameters.AddWithValue("$dt", endDate);
            cmd.Parameters.AddWithValue("$u", unitUid);
            cmd.ExecuteNonQuery();
        }
    }

    public void SetUnitPvpStats(long unitUid, int rating, int rPoint, int aPoint)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                UPDATE Units
                SET Rating = $r,
                    MaxRating = CASE WHEN $r > MaxRating THEN $r ELSE MaxRating END,
                    RPoint = $rp,
                    APoint = $ap
                WHERE UnitUID = $u;
                """;
            cmd.Parameters.AddWithValue("$r", Math.Max(0, rating));
            cmd.Parameters.AddWithValue("$rp", Math.Max(0, rPoint));
            cmd.Parameters.AddWithValue("$ap", Math.Max(0, aPoint));
            cmd.Parameters.AddWithValue("$u", unitUid);
            cmd.ExecuteNonQuery();
        }
    }

    public void SetAccountChannelingCode(long accountUid, int channelingCode)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE Accounts SET ChannelingCode = $cc WHERE UserUID = $acc;";
            cmd.Parameters.AddWithValue("$cc", channelingCode);
            cmd.Parameters.AddWithValue("$acc", accountUid);
            cmd.ExecuteNonQuery();
        }
    }

    public List<string> GetAuthLevelUsers(RegionProfile region, sbyte authLevel)
    {
        var list = new List<string>();
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT UserID FROM Accounts WHERE RegionCode = $r AND AuthLevel = $lv ORDER BY UserID;";
        cmd.Parameters.AddWithValue("$r", region.Code);
        cmd.Parameters.AddWithValue("$lv", authLevel);
        using var r = cmd.ExecuteReader();
        while (r.Read())
            list.Add(r.GetString(0));
        return list;
    }

    public (int ok, int curGuildSPoint) AddGuildSkillPoints(int guildUid, int delta)
    {
        if (guildUid <= 0) return (NetError.ERR_GUILD_19, 0);
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var sel = conn.CreateCommand();
            sel.CommandText = "SELECT GuildSPoint FROM Guilds WHERE GuildUID = $gid;";
            sel.Parameters.AddWithValue("$gid", guildUid);
            var obj = sel.ExecuteScalar();
            if (obj == null) return (NetError.ERR_GUILD_19, 0);
            int next = Math.Max(0, Convert.ToInt32(obj) + delta);

            using var upd = conn.CreateCommand();
            upd.CommandText = "UPDATE Guilds SET GuildSPoint = $sp WHERE GuildUID = $gid;";
            upd.Parameters.AddWithValue("$sp", next);
            upd.Parameters.AddWithValue("$gid", guildUid);
            upd.ExecuteNonQuery();
            return (NetError.NET_OK, next);
        }
    }

    public void DeleteGuildByAdmin(int guildUid)
    {
        if (guildUid <= 0) return;
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var c1 = conn.CreateCommand();
            c1.CommandText = "UPDATE Units SET GuildUID = 0 WHERE GuildUID = $gid;";
            c1.Parameters.AddWithValue("$gid", guildUid);
            c1.ExecuteNonQuery();

            using var c2 = conn.CreateCommand();
            c2.CommandText = "DELETE FROM GuildMembers WHERE GuildUID = $gid;";
            c2.Parameters.AddWithValue("$gid", guildUid);
            c2.ExecuteNonQuery();

            using var c3 = conn.CreateCommand();
            c3.CommandText = "DELETE FROM Guilds WHERE GuildUID = $gid;";
            c3.Parameters.AddWithValue("$gid", guildUid);
            c3.ExecuteNonQuery();
        }
    }

    public void SetAccountAuthLevel(RegionProfile region, string userId, sbyte authLevel)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE Accounts SET AuthLevel = $lv WHERE RegionCode = $r AND UserID = $uid;";
            cmd.Parameters.AddWithValue("$lv", authLevel);
            cmd.Parameters.AddWithValue("$r", region.Code);
            cmd.Parameters.AddWithValue("$uid", userId);
            cmd.ExecuteNonQuery();
        }
    }

    public void WriteCheatLog(long unitUid, long accountUid, string command, string contents)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                INSERT INTO CheatLogs (UnitUID, AccountUID, CheatCommand, CheatContents, CreatedUtc)
                VALUES ($u, $acc, $cmd, $cnt, $now);
                """;
            cmd.Parameters.AddWithValue("$u", unitUid);
            cmd.Parameters.AddWithValue("$acc", accountUid);
            cmd.Parameters.AddWithValue("$cmd", command);
            cmd.Parameters.AddWithValue("$cnt", contents);
            cmd.Parameters.AddWithValue("$now", DateTime.UtcNow.ToString("O"));
            cmd.ExecuteNonQuery();
        }
    }

    // ---------------- Batch 2: Unit Lifecycle, Settings, LocalRanking, Events, Recommendations, Logs ----------------

    public Dictionary<int, int> GetCreateUnitCountTodayByServerGroup(RegionProfile region, long accountUid)
    {
        var result = new Dictionary<int, int>();
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        string todayPrefix = DateTime.UtcNow.ToString("yyyy-MM-dd");
        cmd.CommandText = """
            SELECT ServerGroupID, COUNT(*)
            FROM Units
            WHERE RegionCode = $r AND UserUID = $acc AND CreatedAt LIKE $today || '%'
            GROUP BY ServerGroupID;
            """;
        cmd.Parameters.AddWithValue("$r", region.Code);
        cmd.Parameters.AddWithValue("$acc", accountUid);
        cmd.Parameters.AddWithValue("$today", todayPrefix);
        using var r = cmd.ExecuteReader();
        while (r.Read())
            result[r.GetInt32(0)] = r.GetInt32(1);
        return result;
    }

    public List<KUnitInfo> GetUnitsForAccountByServerGroup(RegionProfile region, long accountUid, int serverGroupId)
    {
        var all = GetUnitsForAccount(region, accountUid);
        if (serverGroupId < 0) return all;
        var filtered = new List<KUnitInfo>();
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT UnitUID FROM Units WHERE RegionCode = $r AND UserUID = $acc AND ServerGroupID = $sg AND IsDeleted = 0;";
        cmd.Parameters.AddWithValue("$r", region.Code);
        cmd.Parameters.AddWithValue("$acc", accountUid);
        cmd.Parameters.AddWithValue("$sg", serverGroupId);
        var allowed = new HashSet<long>();
        using var r = cmd.ExecuteReader();
        while (r.Read()) allowed.Add(r.GetInt64(0));
        foreach (var u in all)
            if (allowed.Contains(u.UnitUid)) filtered.Add(u);
        return filtered;
    }

    public (bool ok, KUnitInfo? unit, int netError) CreateUnitInServerGroup(RegionProfile region, long userUid, string nickName, int unitClass, int serverGroupId)
    {
        var res = CreateUnit(region, userUid, nickName, unitClass);
        if (!res.ok || res.unit == null) return res;
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var upd = conn.CreateCommand();
            upd.CommandText = "UPDATE Units SET ServerGroupID = $sg WHERE UnitUID = $u;";
            upd.Parameters.AddWithValue("$sg", serverGroupId);
            upd.Parameters.AddWithValue("$u", res.unit.UnitUid);
            upd.ExecuteNonQuery();
        }
        return res;
    }

    public (int ok, long restoreAbleDate) RestoreUnit(RegionProfile region, long accountUid, long unitUid)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var sel = conn.CreateCommand();
            sel.CommandText = "SELECT IsDeleted FROM Units WHERE RegionCode = $r AND UserUID = $acc AND UnitUID = $u LIMIT 1;";
            sel.Parameters.AddWithValue("$r", region.Code);
            sel.Parameters.AddWithValue("$acc", accountUid);
            sel.Parameters.AddWithValue("$u", unitUid);
            var obj = sel.ExecuteScalar();
            if (obj == null) return (NetError.ERR_DELETE_UNIT_00, 0);

            long restoreAbleDate = DateTimeOffset.UtcNow.AddDays(30).ToUnixTimeSeconds();
            using var upd = conn.CreateCommand();
            upd.CommandText = "UPDATE Units SET IsDeleted = 0, RestoreAbleDate = $rd WHERE UnitUID = $u AND UserUID = $acc;";
            upd.Parameters.AddWithValue("$rd", restoreAbleDate);
            upd.Parameters.AddWithValue("$u", unitUid);
            upd.Parameters.AddWithValue("$acc", accountUid);
            upd.ExecuteNonQuery();
            return (NetError.NET_OK, restoreAbleDate);
        }
    }

    public (int ok, int guildUid, string nickName, byte level, sbyte unitClass, long reDelAbleDate) FinalDeleteUnit(
        RegionProfile region, long accountUid, long unitUid)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var sel = conn.CreateCommand();
            sel.CommandText = "SELECT GuildUID, NickName, Level, UnitClass FROM Units WHERE RegionCode = $r AND UserUID = $acc AND UnitUID = $u LIMIT 1;";
            sel.Parameters.AddWithValue("$r", region.Code);
            sel.Parameters.AddWithValue("$acc", accountUid);
            sel.Parameters.AddWithValue("$u", unitUid);
            using var r = sel.ExecuteReader();
            if (!r.Read()) return (NetError.ERR_DELETE_UNIT_00, 0, string.Empty, 0, 0, 0);
            int guildUid = r.GetInt32(0);
            string nick = r.GetString(1);
            byte lvl = (byte)Math.Clamp(r.GetInt32(2), 1, 99);
            sbyte ucls = (sbyte)r.GetInt32(3);
            r.Close();

            using var del = conn.CreateCommand();
            del.CommandText = "UPDATE Units SET IsDeleted = 2 WHERE UnitUID = $u AND UserUID = $acc;";
            del.Parameters.AddWithValue("$u", unitUid);
            del.Parameters.AddWithValue("$acc", accountUid);
            del.ExecuteNonQuery();

            long reDel = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            return (NetError.NET_OK, guildUid, nick, lvl, ucls, reDel);
        }
    }

    public int ChangeUnitNickName(RegionProfile region, long accountUid, long unitUid, string newNickName, bool checkOnly)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var dup = conn.CreateCommand();
            dup.CommandText = "SELECT COUNT(*) FROM Units WHERE RegionCode = $r AND LOWER(NickName) = LOWER($n);";
            dup.Parameters.AddWithValue("$r", region.Code);
            dup.Parameters.AddWithValue("$n", newNickName);
            if (Convert.ToInt32(dup.ExecuteScalar()) > 0)
                return NetError.ERR_CREATE_UNIT_01;

            if (checkOnly) return NetError.NET_OK;

            using var upd = conn.CreateCommand();
            upd.CommandText = "UPDATE Units SET NickName = $n WHERE RegionCode = $r AND UserUID = $acc AND UnitUID = $u;";
            upd.Parameters.AddWithValue("$n", newNickName);
            upd.Parameters.AddWithValue("$r", region.Code);
            upd.Parameters.AddWithValue("$acc", accountUid);
            upd.Parameters.AddWithValue("$u", unitUid);
            return upd.ExecuteNonQuery() > 0 ? NetError.NET_OK : NetError.ERR_DELETE_UNIT_00;
        }
    }

    public void SetPlayGuideOption(long accountUid, bool on)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                INSERT INTO UserSettings (AccountUID, PlayGuideOn, UpdatedUtc)
                VALUES ($acc, $on, $now)
                ON CONFLICT(AccountUID) DO UPDATE SET PlayGuideOn = $on, UpdatedUtc = $now;
                """;
            cmd.Parameters.AddWithValue("$acc", accountUid);
            cmd.Parameters.AddWithValue("$on", on ? 1 : 0);
            cmd.Parameters.AddWithValue("$now", DateTime.UtcNow.ToString("O"));
            cmd.ExecuteNonQuery();
        }
    }

    public void SetChatOption(long accountUid, byte index, bool value)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            Dictionary<string, bool> opts = new();
            using (var sel = conn.CreateCommand())
            {
                sel.CommandText = "SELECT ChatOptionsJson FROM UserSettings WHERE AccountUID = $acc LIMIT 1;";
                sel.Parameters.AddWithValue("$acc", accountUid);
                if (sel.ExecuteScalar() is string json && !string.IsNullOrWhiteSpace(json))
                {
                    try { opts = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, bool>>(json) ?? new(); }
                    catch { opts = new(); }
                }
            }
            opts[index.ToString()] = value;
            string serialized = System.Text.Json.JsonSerializer.Serialize(opts);

            using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                INSERT INTO UserSettings (AccountUID, ChatOptionsJson, UpdatedUtc)
                VALUES ($acc, $json, $now)
                ON CONFLICT(AccountUID) DO UPDATE SET ChatOptionsJson = $json, UpdatedUtc = $now;
                """;
            cmd.Parameters.AddWithValue("$acc", accountUid);
            cmd.Parameters.AddWithValue("$json", serialized);
            cmd.Parameters.AddWithValue("$now", DateTime.UtcNow.ToString("O"));
            cmd.ExecuteNonQuery();
        }
    }

    public void SetKeyboardMapping(long accountUid, IReadOnlyDictionary<short, short> keyboard, IReadOnlyDictionary<short, short> gamePad)
    {
        lock (_writeLock)
        {
            string kbJson = System.Text.Json.JsonSerializer.Serialize(keyboard);
            string gpJson = System.Text.Json.JsonSerializer.Serialize(gamePad);
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                INSERT INTO UserSettings (AccountUID, KeyboardMappingJson, GamePadMappingJson, UpdatedUtc)
                VALUES ($acc, $kb, $gp, $now)
                ON CONFLICT(AccountUID) DO UPDATE SET KeyboardMappingJson = $kb, GamePadMappingJson = $gp, UpdatedUtc = $now;
                """;
            cmd.Parameters.AddWithValue("$acc", accountUid);
            cmd.Parameters.AddWithValue("$kb", kbJson);
            cmd.Parameters.AddWithValue("$gp", gpJson);
            cmd.Parameters.AddWithValue("$now", DateTime.UtcNow.ToString("O"));
            cmd.ExecuteNonQuery();
        }
    }

    public (KLocalRankingUserInfo info, string updated) GetLocalRankingUserInfo(long accountUid)
    {
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT Category, Birth, Profile, FilterByte, GenderByte, UpdatedUtc
            FROM UserLocalRankingProfiles
            WHERE AccountUID = $acc LIMIT 1;
            """;
        cmd.Parameters.AddWithValue("$acc", accountUid);
        using var r = cmd.ExecuteReader();
        if (!r.Read())
            return (new KLocalRankingUserInfo { UserUid = accountUid }, string.Empty);

        return (new KLocalRankingUserInfo
        {
            UserUid = accountUid,
            Category = r.GetInt32(0),
            Birth = r.GetInt64(1),
            Profile = r.GetString(2),
            Filter = (byte)r.GetInt32(3),
            Gender = (byte)r.GetInt32(4),
        }, r.GetString(5));
    }

    public void SaveLocalRankingUserInfo(long accountUid, long unitUid, KLocalRankingUserInfo info, int point0 = 0, int point1 = 0)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                INSERT INTO UserLocalRankingProfiles (AccountUID, UnitUID, Category, Birth, Profile, FilterByte, GenderByte, Point0, Point1, UpdatedUtc)
                VALUES ($acc, $u, $cat, $birth, $prof, $flt, $gen, $p0, $p1, $now)
                ON CONFLICT(AccountUID) DO UPDATE SET
                    UnitUID = $u, Category = $cat, Birth = $birth, Profile = $prof,
                    FilterByte = $flt, GenderByte = $gen, Point0 = $p0, Point1 = $p1, UpdatedUtc = $now;
                """;
            cmd.Parameters.AddWithValue("$acc", accountUid);
            cmd.Parameters.AddWithValue("$u", unitUid);
            cmd.Parameters.AddWithValue("$cat", info.Category);
            cmd.Parameters.AddWithValue("$birth", info.Birth);
            cmd.Parameters.AddWithValue("$prof", info.Profile);
            cmd.Parameters.AddWithValue("$flt", (int)info.Filter);
            cmd.Parameters.AddWithValue("$gen", (int)info.Gender);
            cmd.Parameters.AddWithValue("$p0", point0);
            cmd.Parameters.AddWithValue("$p1", point1);
            cmd.Parameters.AddWithValue("$now", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.ExecuteNonQuery();
        }
    }

    public List<KLocalRankingRankerInfo> QueryLocalRankings(RegionProfile region, int categoryFilter = -1, int maxCount = 50)
    {
        var list = new List<KLocalRankingRankerInfo>();
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT p.AccountUID, p.UnitUID, p.Category, p.Birth, p.Profile, p.FilterByte, p.GenderByte, p.Point0, p.Point1,
                   u.NickName, u.Level, u.UnitClass
            FROM UserLocalRankingProfiles p
            JOIN Units u ON u.UnitUID = p.UnitUID
            WHERE u.RegionCode = $r AND u.IsDeleted = 0 AND ($cat < 0 OR p.Category = $cat)
            ORDER BY p.Point0 DESC, u.Level DESC, p.UnitUID ASC
            LIMIT $lim;
            """;
        cmd.Parameters.AddWithValue("$r", region.Code);
        cmd.Parameters.AddWithValue("$cat", categoryFilter);
        cmd.Parameters.AddWithValue("$lim", Math.Clamp(maxCount, 1, 200));
        using var r = cmd.ExecuteReader();
        int rank = 1;
        while (r.Read())
        {
            list.Add(new KLocalRankingRankerInfo
            {
                Rank = rank++,
                UserInfo = new KLocalRankingUserInfo
                {
                    UserUid = r.GetInt64(0),
                    Category = r.GetInt32(2),
                    Birth = r.GetInt64(3),
                    Profile = r.GetString(4),
                    Filter = (byte)r.GetInt32(5),
                    Gender = (byte)r.GetInt32(6),
                },
                UnitInfo = new KLocalRankingUnitInfo
                {
                    UnitUid = r.GetInt64(1),
                    NickName = r.GetString(9),
                    Level = (byte)Math.Clamp(r.GetInt32(10), 1, 99),
                    UnitClass = (sbyte)r.GetInt32(11),
                    Point = { [0] = r.GetInt32(7), [1] = r.GetInt32(8) },
                },
            });
        }
        return list;
    }

    public bool CheckAttendanceToday(long unitUid)
    {
        string today = DateTime.UtcNow.ToString("yyyy-MM-dd");
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var sel = conn.CreateCommand();
            sel.CommandText = "SELECT AttendanceLastDate FROM UserEventProgress WHERE UnitUID = $u LIMIT 1;";
            sel.Parameters.AddWithValue("$u", unitUid);
            if (sel.ExecuteScalar() is string last && last == today)
                return false;

            using var up = conn.CreateCommand();
            up.CommandText = """
                INSERT INTO UserEventProgress (UnitUID, AttendanceLastDate)
                VALUES ($u, $d)
                ON CONFLICT(UnitUID) DO UPDATE SET AttendanceLastDate = $d;
                """;
            up.Parameters.AddWithValue("$u", unitUid);
            up.Parameters.AddWithValue("$d", today);
            up.ExecuteNonQuery();
            return true;
        }
    }

    public void SaveUnitSkillPageState(long unitUid, int pageCount, int activePage, IReadOnlyDictionary<int, int> notePages)
    {
        lock (_writeLock)
        {
            string notesJson = System.Text.Json.JsonSerializer.Serialize(notePages);
            using var conn = OpenConnection();
            using var upd = conn.CreateCommand();
            upd.CommandText = "UPDATE Units SET SkillPageCount = $pc, ActiveSkillPage = $ap, SkillNotePages = $np WHERE UnitUID = $u;";
            upd.Parameters.AddWithValue("$pc", pageCount);
            upd.Parameters.AddWithValue("$ap", activePage);
            upd.Parameters.AddWithValue("$np", notesJson);
            upd.Parameters.AddWithValue("$u", unitUid);
            upd.ExecuteNonQuery();
        }
    }

    public (int pageCount, int activePage, Dictionary<int, int> notePages) GetUnitSkillPageState(long unitUid)
    {
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT SkillPageCount, ActiveSkillPage, SkillNotePages FROM Units WHERE UnitUID=$u LIMIT 1;";
        cmd.Parameters.AddWithValue("$u", unitUid);
        using var r = cmd.ExecuteReader();
        if (!r.Read()) return (1, 1, new Dictionary<int, int>());
        int pageCount = Math.Clamp(r.GetInt32(0), 1, 3);
        int activePage = Math.Clamp(r.GetInt32(1), 1, pageCount);
        string rawJson = r.IsDBNull(2) ? "" : r.GetString(2);
        Dictionary<int, int> notes;
        try
        {
            notes = string.IsNullOrWhiteSpace(rawJson)
                ? new Dictionary<int, int>()
                : (System.Text.Json.JsonSerializer.Deserialize<Dictionary<int, int>>(rawJson) ?? new Dictionary<int, int>());
        }
        catch
        {
            notes = new Dictionary<int, int>();
        }
        return (pageCount, activePage, notes);
    }

    // ---------------- Account Cash Balance & Henir Rankings ----------------

    public uint GetAccountCashBalance(long accountUid)
    {
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT CashBalance FROM Accounts WHERE UserUID=$u LIMIT 1;";
        cmd.Parameters.AddWithValue("$u", accountUid);
        var obj = cmd.ExecuteScalar();
        return obj == null || obj == DBNull.Value ? 10_000u : (uint)Math.Max(0L, Convert.ToInt64(obj));
    }

    public (bool ok, uint remaining) TrySpendAccountCash(long accountUid, uint cost)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var sel = conn.CreateCommand();
            sel.CommandText = "SELECT CashBalance FROM Accounts WHERE UserUID=$u LIMIT 1;";
            sel.Parameters.AddWithValue("$u", accountUid);
            var obj = sel.ExecuteScalar();
            uint cur = obj == null || obj == DBNull.Value ? 10_000u : (uint)Math.Max(0L, Convert.ToInt64(obj));
            if (cur < cost) return (false, cur);
            uint rem = cur - cost;
            using var upd = conn.CreateCommand();
            upd.CommandText = "UPDATE Accounts SET CashBalance=$c WHERE UserUID=$u;";
            upd.Parameters.AddWithValue("$c", (long)rem);
            upd.Parameters.AddWithValue("$u", accountUid);
            upd.ExecuteNonQuery();
            return (true, rem);
        }
    }

    public uint AddAccountCash(long accountUid, uint amount)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var sel = conn.CreateCommand();
            sel.CommandText = "SELECT CashBalance FROM Accounts WHERE UserUID=$u LIMIT 1;";
            sel.Parameters.AddWithValue("$u", accountUid);
            var obj = sel.ExecuteScalar();
            uint cur = obj == null || obj == DBNull.Value ? 10_000u : (uint)Math.Max(0L, Convert.ToInt64(obj));
            uint updated = cur + amount;
            using var upd = conn.CreateCommand();
            upd.CommandText = "UPDATE Accounts SET CashBalance=$c WHERE UserUID=$u;";
            upd.Parameters.AddWithValue("$c", (long)updated);
            upd.Parameters.AddWithValue("$u", accountUid);
            upd.ExecuteNonQuery();
            return updated;
        }
    }

    public void UpsertHenirRanking(int rankingType, long unitUid, string nickName, byte unitClass, byte level, int stageCount, int clearTimeSeconds, DateTime recordedUtc)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                INSERT INTO HenirRankings (RankingType, UnitUID, NickName, UnitClass, Level, StageCount, ClearTimeSeconds, RecordedUtc)
                VALUES ($rt, $u, $n, $uc, $lv, $sc, $ct, $dt)
                ON CONFLICT(RankingType, UnitUID) DO UPDATE SET
                    NickName = excluded.NickName,
                    UnitClass = excluded.UnitClass,
                    Level = excluded.Level,
                    StageCount = excluded.StageCount,
                    ClearTimeSeconds = excluded.ClearTimeSeconds,
                    RecordedUtc = excluded.RecordedUtc;
                """;
            cmd.Parameters.AddWithValue("$rt", rankingType);
            cmd.Parameters.AddWithValue("$u", unitUid);
            cmd.Parameters.AddWithValue("$n", nickName);
            cmd.Parameters.AddWithValue("$uc", (int)unitClass);
            cmd.Parameters.AddWithValue("$lv", (int)level);
            cmd.Parameters.AddWithValue("$sc", stageCount);
            cmd.Parameters.AddWithValue("$ct", clearTimeSeconds);
            cmd.Parameters.AddWithValue("$dt", recordedUtc.ToString("O"));
            cmd.ExecuteNonQuery();
        }
    }

    public List<(long unitUid, string nickName, byte unitClass, byte level, int stageCount, int clearTimeSec, DateTime recordedUtc)> GetHenirRankings(int rankingType = 0)
    {
        var list = new List<(long, string, byte, byte, int, int, DateTime)>();
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT UnitUID, NickName, UnitClass, Level, StageCount, ClearTimeSeconds, RecordedUtc
            FROM HenirRankings WHERE RankingType=$rt
            ORDER BY StageCount DESC, ClearTimeSeconds ASC LIMIT 100;
            """;
        cmd.Parameters.AddWithValue("$rt", rankingType);
        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            DateTime dt = DateTime.TryParse(r.GetString(6), out var parsed)
                ? DateTime.SpecifyKind(parsed, DateTimeKind.Utc)
                : DateTime.UtcNow;
            list.Add((r.GetInt64(0), r.GetString(1), (byte)r.GetInt32(2), (byte)r.GetInt32(3), r.GetInt32(4), r.GetInt32(5), dt));
        }
        return list;
    }

    public void DeleteHenirRankingByUnitUid(long unitUid, int rankingType = 0)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM HenirRankings WHERE RankingType=$rt AND UnitUID=$u;";
            cmd.Parameters.AddWithValue("$rt", rankingType);
            cmd.Parameters.AddWithValue("$u", unitUid);
            cmd.ExecuteNonQuery();
        }
    }

    public void DeleteHenirRankingByNickName(string nickName, int rankingType = 0)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM HenirRankings WHERE RankingType=$rt AND LOWER(NickName)=LOWER($n);";
            cmd.Parameters.AddWithValue("$rt", rankingType);
            cmd.Parameters.AddWithValue("$n", nickName);
            cmd.ExecuteNonQuery();
        }
    }

    public void SaveUserEventState(
        long unitUid,
        int oldYearRewardedLevel,
        int newYearStepId,
        long checkPowerTime,
        byte checkPowerCount,
        byte checkPowerScore,
        int noteViewCount,
        int[] chinaSpirit,
        string jumpingDate = "")
    {
        lock (_writeLock)
        {
            string spiritCsv = string.Join(",", chinaSpirit.Take(6));
            using var conn = OpenConnection();
            using var up = conn.CreateCommand();
            up.CommandText = """
                INSERT INTO UserEventProgress (
                    UnitUID, OldYearRewardedLevel, NewYearStepID, CheckPowerTime, CheckPowerCount,
                    CheckPowerScore, NoteViewCount, ChinaSpirit, JumpingDate
                ) VALUES ($u, $oy, $ny, $cpt, $cpc, $cps, $nvc, $sp, $jd)
                ON CONFLICT(UnitUID) DO UPDATE SET
                    OldYearRewardedLevel = $oy, NewYearStepID = $ny, CheckPowerTime = $cpt,
                    CheckPowerCount = $cpc, CheckPowerScore = $cps, NoteViewCount = $nvc,
                    ChinaSpirit = $sp, JumpingDate = CASE WHEN $jd <> '' THEN $jd ELSE JumpingDate END;
                """;
            up.Parameters.AddWithValue("$u", unitUid);
            up.Parameters.AddWithValue("$oy", oldYearRewardedLevel);
            up.Parameters.AddWithValue("$ny", newYearStepId);
            up.Parameters.AddWithValue("$cpt", checkPowerTime);
            up.Parameters.AddWithValue("$cpc", (int)checkPowerCount);
            up.Parameters.AddWithValue("$cps", (int)checkPowerScore);
            up.Parameters.AddWithValue("$nvc", noteViewCount);
            up.Parameters.AddWithValue("$sp", spiritCsv);
            up.Parameters.AddWithValue("$jd", jumpingDate);
            up.ExecuteNonQuery();
        }
    }

    public (int oldYearRewardedLevel, int newYearStepId, long checkPowerTime, byte checkPowerCount, byte checkPowerScore, int noteViewCount, int[] chinaSpirit, string jumpingDate) GetUserEventState(long unitUid)
    {
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT OldYearRewardedLevel, NewYearStepID, CheckPowerTime, CheckPowerCount,
                   CheckPowerScore, NoteViewCount, ChinaSpirit, JumpingDate
            FROM UserEventProgress WHERE UnitUID = $u LIMIT 1;
            """;
        cmd.Parameters.AddWithValue("$u", unitUid);
        using var r = cmd.ExecuteReader();
        if (!r.Read())
            return (0, 0, 0L, 0, 0, 1, new int[6], string.Empty);

        var spirit = new int[6];
        string spRaw = r.IsDBNull(6) ? "" : r.GetString(6);
        var parts = spRaw.Split(',', StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < Math.Min(6, parts.Length); i++)
            if (int.TryParse(parts[i], out int val)) spirit[i] = val;

        return (
            r.GetInt32(0),
            r.GetInt32(1),
            r.GetInt64(2),
            (byte)Math.Clamp(r.GetInt32(3), 0, 255),
            (byte)Math.Clamp(r.GetInt32(4), 0, 255),
            r.GetInt32(5),
            spirit,
            r.IsDBNull(7) ? string.Empty : r.GetString(7));
    }

    public (int ok, long targetUnitUid) RecommendUser(RegionProfile region, long unitUid, string targetNickName)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var chk = conn.CreateCommand();
            chk.CommandText = "SELECT COUNT(*) FROM UserRecommendations WHERE UnitUID = $u;";
            chk.Parameters.AddWithValue("$u", unitUid);
            if (Convert.ToInt32(chk.ExecuteScalar()) > 0)
                return (NetError.ERR_RECOMMEND_USER_08, 0);

            long? targetUid = FindUnitUidByNickName(region, targetNickName);
            if (targetUid == null || targetUid.Value == unitUid)
                return (NetError.ERR_RECOMMEND_USER_01, 0);

            using var ins = conn.CreateCommand();
            ins.CommandText = """
                INSERT INTO UserRecommendations (UnitUID, RecommendedUnitUID, RecommendedNickName, CreatedUtc)
                VALUES ($u, $tu, $tn, $now);
                """;
            ins.Parameters.AddWithValue("$u", unitUid);
            ins.Parameters.AddWithValue("$tu", targetUid.Value);
            ins.Parameters.AddWithValue("$tn", targetNickName);
            ins.Parameters.AddWithValue("$now", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
            ins.ExecuteNonQuery();
            return (NetError.NET_OK, targetUid.Value);
        }
    }

    public List<KRecommendUserInfo> GetRecommendedByUsers(RegionProfile region, long targetUnitUid)
    {
        var list = new List<KRecommendUserInfo>();
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT r.UnitUID, u.NickName, u.Level, r.CreatedUtc
            FROM UserRecommendations r
            JOIN Units u ON u.UnitUID = r.UnitUID
            WHERE r.RecommendedUnitUID = $tu AND u.RegionCode = $r AND u.IsDeleted = 0
            ORDER BY r.CreatedUtc DESC;
            """;
        cmd.Parameters.AddWithValue("$tu", targetUnitUid);
        cmd.Parameters.AddWithValue("$r", region.Code);
        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            list.Add(new KRecommendUserInfo
            {
                UnitUid = r.GetInt64(0),
                NickName = r.GetString(1),
                Level = (byte)Math.Clamp(r.GetInt32(2), 1, 99),
                RecommendDate = r.GetString(3),
            });
        }
        return list;
    }

    public void RecordHackOrPortOrKomLog(long accountUid, long unitUid, string logKind, int codeValue, int subValue = 0, string textValue = "")
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                INSERT INTO HackAndPortLogs (AccountUID, UnitUID, LogKind, CodeValue, SubValue, TextValue, CreatedUtc)
                VALUES ($acc, $u, $k, $cv, $sv, $tv, $now);
                """;
            cmd.Parameters.AddWithValue("$acc", accountUid);
            cmd.Parameters.AddWithValue("$u", unitUid);
            cmd.Parameters.AddWithValue("$k", logKind);
            cmd.Parameters.AddWithValue("$cv", codeValue);
            cmd.Parameters.AddWithValue("$sv", subValue);
            cmd.Parameters.AddWithValue("$tv", textValue);
            cmd.Parameters.AddWithValue("$now", DateTime.UtcNow.ToString("O"));
            cmd.ExecuteNonQuery();
        }
    }

    public void InsertGameLog(long accountUid, long unitUid, string logKind, int codeValue = 0, int subValue = 0, string message = "")
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                INSERT INTO GameLogs (AccountUID, UnitUID, LogKind, CodeValue, SubValue, Message, CreatedUtc)
                VALUES ($acc, $u, $k, $cv, $sv, $msg, $now);
                """;
            cmd.Parameters.AddWithValue("$acc", accountUid);
            cmd.Parameters.AddWithValue("$u", unitUid);
            cmd.Parameters.AddWithValue("$k", logKind);
            cmd.Parameters.AddWithValue("$cv", codeValue);
            cmd.Parameters.AddWithValue("$sv", subValue);
            cmd.Parameters.AddWithValue("$msg", message);
            cmd.Parameters.AddWithValue("$now", DateTime.UtcNow.ToString("O"));
            cmd.ExecuteNonQuery();
        }
    }

    public (int inventoryGrade, int bankSlotCount) GetUnitExpandProgress(long unitUid)
    {
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT InventoryExpandGrade, BankSlotCount FROM Units WHERE UnitUID=$u LIMIT 1;";
        cmd.Parameters.AddWithValue("$u", unitUid);
        using var r = cmd.ExecuteReader();
        if (!r.Read()) return (0, 8);
        return (Math.Max(0, r.GetInt32(0)), Math.Max(8, r.GetInt32(1)));
    }

    public void SetUnitInventoryExpandGrade(long unitUid, int grade)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE Units SET InventoryExpandGrade=$g WHERE UnitUID=$u;";
            cmd.Parameters.AddWithValue("$g", Math.Max(0, grade));
            cmd.Parameters.AddWithValue("$u", unitUid);
            cmd.ExecuteNonQuery();
        }
    }

    public void SetUnitBankSlotCount(long unitUid, int slotCount)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE Units SET BankSlotCount=$s WHERE UnitUID=$u;";
            cmd.Parameters.AddWithValue("$s", Math.Max(8, slotCount));
            cmd.Parameters.AddWithValue("$u", unitUid);
            cmd.ExecuteNonQuery();
        }
    }

    public int GetTotalItemCountByItemId(long unitUid, int itemId, sbyte slotCategory = -1)
    {
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = slotCategory >= 0
            ? "SELECT COALESCE(SUM(Quantity), 0) FROM InventoryItems WHERE UnitUID=$u AND ItemID=$id AND SlotCategory=$cat;"
            : "SELECT COALESCE(SUM(Quantity), 0) FROM InventoryItems WHERE UnitUID=$u AND ItemID=$id;";
        cmd.Parameters.AddWithValue("$u", unitUid);
        cmd.Parameters.AddWithValue("$id", itemId);
        if (slotCategory >= 0) cmd.Parameters.AddWithValue("$cat", slotCategory);
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    public (bool ok, List<KInventoryItemInfo> updatedSlots) TryConsumeItemsByItemId(long unitUid, int itemId, int count, sbyte slotCategory = -1)
    {
        if (count <= 0) return (true, []);
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var sel = conn.CreateCommand();
            sel.CommandText = slotCategory >= 0
                ? "SELECT ItemUID, SlotCategory, SlotID, Quantity FROM InventoryItems WHERE UnitUID=$u AND ItemID=$id AND SlotCategory=$cat ORDER BY SlotID ASC;"
                : "SELECT ItemUID, SlotCategory, SlotID, Quantity FROM InventoryItems WHERE UnitUID=$u AND ItemID=$id ORDER BY SlotCategory ASC, SlotID ASC;";
            sel.Parameters.AddWithValue("$u", unitUid);
            sel.Parameters.AddWithValue("$id", itemId);
            if (slotCategory >= 0) sel.Parameters.AddWithValue("$cat", slotCategory);

            var rows = new List<(long uid, sbyte cat, short slot, int qty)>();
            int total = 0;
            using (var r = sel.ExecuteReader())
            {
                while (r.Read())
                {
                    int q = r.GetInt32(3);
                    total += q;
                    rows.Add((r.GetInt64(0), (sbyte)r.GetInt32(1), (short)r.GetInt32(2), q));
                }
            }
            if (total < count) return (false, []);

            int need = count;
            var updated = new List<KInventoryItemInfo>();
            foreach (var row in rows)
            {
                if (need <= 0) break;
                int take = Math.Min(need, row.qty);
                int rem = row.qty - take;
                need -= take;
                if (rem <= 0)
                {
                    using var del = conn.CreateCommand();
                    del.CommandText = "DELETE FROM InventoryItems WHERE ItemUID=$i AND UnitUID=$u;";
                    del.Parameters.AddWithValue("$i", row.uid);
                    del.Parameters.AddWithValue("$u", unitUid);
                    del.ExecuteNonQuery();
                    updated.Add(new KInventoryItemInfo { ItemUid = row.uid, SlotCategory = row.cat, SlotId = row.slot });
                }
                else
                {
                    using var upd = conn.CreateCommand();
                    upd.CommandText = "UPDATE InventoryItems SET Quantity=$q WHERE ItemUID=$i AND UnitUID=$u;";
                    upd.Parameters.AddWithValue("$q", rem);
                    upd.Parameters.AddWithValue("$i", row.uid);
                    upd.Parameters.AddWithValue("$u", unitUid);
                    upd.ExecuteNonQuery();
                    updated.Add(new KInventoryItemInfo
                    {
                        ItemUid = row.uid,
                        SlotCategory = row.cat,
                        SlotId = row.slot,
                        ItemInfo = new KItemInfo { ItemId = itemId, Quantity = rem },
                    });
                }
            }
            return (true, updated);
        }
    }

    public (bool ok, int consumedItemId, List<KInventoryItemInfo> updatedSlots) TryConsumeAnyItemId(
        long unitUid, IReadOnlyList<int> candidateItemIds, int count = 1, sbyte slotCategory = -1)
    {
        foreach (int cid in candidateItemIds.Distinct())
        {
            if (cid <= 0) continue;
            if (GetTotalItemCountByItemId(unitUid, cid, slotCategory) >= count)
            {
                var (ok, upd) = TryConsumeItemsByItemId(unitUid, cid, count, slotCategory);
                if (ok) return (true, cid, upd);
            }
        }
        return (false, 0, []);
    }

    public (bool found, KInventoryItemInfo slot, bool isBroken, bool isSealed) GetItemFullDetails(long unitUid, long itemUid)
    {
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT ItemID, SlotCategory, SlotID, UsageType, Quantity, Endurance, EnchantLevel, IsBroken, IsSealed, SocketIds, AttribEnchant, RandomSocketIds, Period, ExpirationDate
            FROM InventoryItems WHERE ItemUID=$i AND UnitUID=$u LIMIT 1;
            """;
        cmd.Parameters.AddWithValue("$i", itemUid);
        cmd.Parameters.AddWithValue("$u", unitUid);
        using var r = cmd.ExecuteReader();
        if (!r.Read()) return (false, new KInventoryItemInfo(), false, false);

        string sockRaw = r.IsDBNull(9) ? "" : r.GetString(9);
        var sockets = sockRaw.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(s => int.TryParse(s, out int v) ? v : 0)
            .ToList();
        string attrRaw = r.IsDBNull(10) ? "0,0,0" : r.GetString(10);
        var attrs = attrRaw.Split(',').Select(s => sbyte.TryParse(s, out sbyte v) ? v : (sbyte)0).ToList();
        while (attrs.Count < 3) attrs.Add(0);
        string randSockRaw = r.IsDBNull(11) ? "" : r.GetString(11);
        var randomSockets = randSockRaw.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(s => int.TryParse(s, out int v) ? v : 0)
            .ToList();
        short period = r.IsDBNull(12) ? (short)0 : (short)r.GetInt32(12);
        string expirationDate = r.IsDBNull(13) ? string.Empty : r.GetString(13);

        var slot = new KInventoryItemInfo
        {
            ItemUid = itemUid,
            SlotCategory = (sbyte)r.GetInt32(1),
            SlotId = (short)r.GetInt32(2),
            ItemInfo = new KItemInfo
            {
                ItemId = r.GetInt32(0),
                UsageType = (sbyte)r.GetInt32(3),
                Quantity = r.GetInt32(4),
                Endurance = (short)r.GetInt32(5),
                EnchantLevel = (sbyte)Math.Clamp(r.GetInt32(6), 0, 20),
                ItemSockets = sockets,
                RandomSockets = randomSockets,
                Period = period,
                ExpirationDate = expirationDate,
                AttribEnchantInfo = new KItemAttributeEnchantInfo
                {
                    AttribEnchant0 = attrs[0],
                    AttribEnchant1 = attrs[1],
                    AttribEnchant2 = attrs[2],
                },
            },
        };
        return (true, slot, r.GetInt32(7) != 0, r.GetInt32(8) != 0);
    }

    public KInventoryItemInfo UpdateItemPeriod(long unitUid, long itemUid, short periodDays, string expirationDate)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE InventoryItems SET Period=$p, ExpirationDate=$e WHERE ItemUID=$i AND UnitUID=$u;";
            cmd.Parameters.AddWithValue("$p", (int)periodDays);
            cmd.Parameters.AddWithValue("$e", expirationDate ?? string.Empty);
            cmd.Parameters.AddWithValue("$i", itemUid);
            cmd.Parameters.AddWithValue("$u", unitUid);
            cmd.ExecuteNonQuery();
        }
        return GetItemFullDetails(unitUid, itemUid).slot;
    }

    public KInventoryItemInfo UpdateItemRandomSockets(long unitUid, long itemUid, IReadOnlyList<int> randomSockets)
    {
        string encoded = string.Join(",", randomSockets);
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE InventoryItems SET RandomSocketIds=$s WHERE ItemUID=$i AND UnitUID=$u;";
            cmd.Parameters.AddWithValue("$s", encoded);
            cmd.Parameters.AddWithValue("$i", itemUid);
            cmd.Parameters.AddWithValue("$u", unitUid);
            cmd.ExecuteNonQuery();
        }
        return GetItemFullDetails(unitUid, itemUid).slot;
    }

    public int DeleteUnitCategoryItems(long unitUid, sbyte slotCategory)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM InventoryItems WHERE UnitUID=$u AND SlotCategory=$cat;";
            cmd.Parameters.AddWithValue("$u", unitUid);
            cmd.Parameters.AddWithValue("$cat", (int)slotCategory);
            return cmd.ExecuteNonQuery();
        }
    }

    public KInventoryItemInfo SetItemSealed(long unitUid, long itemUid, bool isSealed)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE InventoryItems SET IsSealed=$s WHERE ItemUID=$i AND UnitUID=$u;";
            cmd.Parameters.AddWithValue("$s", isSealed ? 1 : 0);
            cmd.Parameters.AddWithValue("$i", itemUid);
            cmd.Parameters.AddWithValue("$u", unitUid);
            cmd.ExecuteNonQuery();
        }
        return GetItemFullDetails(unitUid, itemUid).slot;
    }

    public DateTime GetUnitCreatedAtUtc(long unitUid)
    {
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT CreatedAt FROM Units WHERE UnitUID=$u LIMIT 1;";
        cmd.Parameters.AddWithValue("$u", unitUid);
        object? raw = cmd.ExecuteScalar();
        if (raw is string s && DateTime.TryParse(s, System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.AssumeUniversal | System.Globalization.DateTimeStyles.AdjustToUniversal, out var dt))
        {
            return dt;
        }
        return DateTime.UtcNow;
    }

    public (List<KInventoryItemInfo> returnedItems, bool remainSellItem) PickUpPersonalShopAgencyListings(
        RegionProfile region, long sellerUnitUid, IReadOnlyList<long> listingUids)
    {
        var pickedUp = new List<(long listingUid, int itemId, int quantity)>();
        bool remain = false;

        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var sel = conn.CreateCommand();
            sel.CommandText = """
                SELECT ListingUID, ItemID, Quantity
                FROM PersonalShopListings
                WHERE RegionCode=$r AND SellerUnitUID=$su AND IsSold=0 AND Quantity > 0;
                """;
            sel.Parameters.AddWithValue("$r", region.Code);
            sel.Parameters.AddWithValue("$su", sellerUnitUid);

            var requested = listingUids.Count > 0 ? new HashSet<long>(listingUids) : null;
            using (var r = sel.ExecuteReader())
            {
                while (r.Read())
                {
                    long luid = r.GetInt64(0);
                    int iid = r.GetInt32(1);
                    int qty = r.GetInt32(2);
                    if (requested == null || requested.Contains(luid))
                        pickedUp.Add((luid, iid, qty));
                    else
                        remain = true;
                }
            }

            foreach (var (luid, _, _) in pickedUp)
            {
                using var del = conn.CreateCommand();
                del.CommandText = "DELETE FROM PersonalShopListings WHERE ListingUID=$l AND SellerUnitUID=$su;";
                del.Parameters.AddWithValue("$l", luid);
                del.Parameters.AddWithValue("$su", sellerUnitUid);
                del.ExecuteNonQuery();
            }
        }

        var grantedSlots = new List<KInventoryItemInfo>(pickedUp.Count);
        foreach (var (_, iid, qty) in pickedUp)
            grantedSlots.Add(GrantUnitItem(sellerUnitUid, iid, qty));

        return (grantedSlots, remain);
    }

    public (DateTime? LastLoginUtc, int ComeBackRewardStep, string ComeBackBuffEndUtc) GetAccountComeBackInfo(long userUid)
    {
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT LastLoginUtc, ComeBackRewardStep, ComeBackBuffEndUtc FROM Accounts WHERE UserUID=$u LIMIT 1;";
        cmd.Parameters.AddWithValue("$u", userUid);
        using var r = cmd.ExecuteReader();
        if (!r.Read()) return (null, 0, string.Empty);

        string lastStr = r.IsDBNull(0) ? string.Empty : r.GetString(0);
        int step = r.IsDBNull(1) ? 0 : r.GetInt32(1);
        string endStr = r.IsDBNull(2) ? string.Empty : r.GetString(2);
        DateTime? lastUtc = DateTime.TryParse(lastStr, null, System.Globalization.DateTimeStyles.RoundtripKind, out var dt)
            ? dt.ToUniversalTime()
            : null;
        return (lastUtc, step, endStr);
    }

    public void UpdateAccountLoginTimestamp(long userUid, DateTime? nowUtc = null)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE Accounts SET LastLoginUtc=$ts WHERE UserUID=$u;";
            cmd.Parameters.AddWithValue("$ts", (nowUtc ?? DateTime.UtcNow).ToString("O"));
            cmd.Parameters.AddWithValue("$u", userUid);
            cmd.ExecuteNonQuery();
        }
    }

    public void SaveAccountComeBackReward(long userUid, int rewardStep, string buffEndUtcStr)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE Accounts SET ComeBackRewardStep=$s, ComeBackBuffEndUtc=$e WHERE UserUID=$u;";
            cmd.Parameters.AddWithValue("$s", rewardStep);
            cmd.Parameters.AddWithValue("$e", buffEndUtcStr);
            cmd.Parameters.AddWithValue("$u", userUid);
            cmd.ExecuteNonQuery();
        }
    }

    public void ClearAccountComeBackState(long userUid)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE Accounts SET ComeBackRewardStep=0, ComeBackBuffEndUtc='' WHERE UserUID=$u;";
            cmd.Parameters.AddWithValue("$u", userUid);
            cmd.ExecuteNonQuery();
        }
    }

    public void LogLetterAndTradeMonitoring(long recvUnitUid, sbyte monitorType, string regDate)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO LetterTradeMonitorLogs (RecvUnitUID, MonitorType, RegDate) VALUES ($u, $t, $d);";
            cmd.Parameters.AddWithValue("$u", recvUnitUid);
            cmd.Parameters.AddWithValue("$t", (int)monitorType);
            cmd.Parameters.AddWithValue("$d", regDate);
            cmd.ExecuteNonQuery();
        }
    }

    public int GetBillingOrderProductId(long accountUid, uint orderNo)
    {
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT ProductID FROM BillingOrders WHERE TransNo = $tno AND AccountUID = $acc;";
        cmd.Parameters.AddWithValue("$tno", (long)orderNo);
        cmd.Parameters.AddWithValue("$acc", accountUid);
        var obj = cmd.ExecuteScalar();
        return obj != null && obj != DBNull.Value ? Convert.ToInt32(obj) : 0;
    }

    public List<(long UnitUid, int ItemId, int State, string RegDate)> GetDailyGiftBoxList(long accountUid)
    {
        var result = new List<(long UnitUid, int ItemId, int State, string RegDate)>();
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT UnitUID, ItemID, State, RegDate FROM DailyGiftBoxPurchases WHERE AccountUID = $acc;";
        cmd.Parameters.AddWithValue("$acc", accountUid);
        using var r = cmd.ExecuteReader();
        while (r.Read())
            result.Add((r.GetInt64(0), r.GetInt32(1), r.GetInt32(2), r.GetString(3)));
        return result;
    }

    public void UpsertDailyGiftBoxPurchase(long accountUid, long unitUid, int itemId, int state, string regDate)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                INSERT INTO DailyGiftBoxPurchases (AccountUID, UnitUID, ItemID, State, RegDate)
                VALUES ($acc, $unit, $item, $state, $dt)
                ON CONFLICT(AccountUID, UnitUID, ItemID) DO UPDATE SET
                    State = excluded.State,
                    RegDate = excluded.RegDate;
                """;
            cmd.Parameters.AddWithValue("$acc", accountUid);
            cmd.Parameters.AddWithValue("$unit", unitUid);
            cmd.Parameters.AddWithValue("$item", itemId);
            cmd.Parameters.AddWithValue("$state", state);
            cmd.Parameters.AddWithValue("$dt", regDate);
            cmd.ExecuteNonQuery();
        }
    }

    public void RecordChatLog(long accountUid, long unitUid, int serverGroupId, string chatText, int chatType)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                INSERT INTO ChatLogs (AccountUID, UnitUID, ServerGroupID, ChatText, ChatType, RecordedUtc)
                VALUES ($acc, $unit, $sg, $txt, $type, $utc);
                """;
            cmd.Parameters.AddWithValue("$acc", accountUid);
            cmd.Parameters.AddWithValue("$unit", unitUid);
            cmd.Parameters.AddWithValue("$sg", serverGroupId);
            cmd.Parameters.AddWithValue("$txt", chatText ?? string.Empty);
            cmd.Parameters.AddWithValue("$type", chatType);
            cmd.Parameters.AddWithValue("$utc", DateTime.UtcNow.ToString("O"));
            cmd.ExecuteNonQuery();
        }
    }

    public List<(long AccountUid, long UnitUid, int ServerGroupId, string ChatText, int ChatType, string RecordedUtc)> GetChatLogs(long unitUid, int limit = 100)
    {
        var result = new List<(long, long, int, string, int, string)>();
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT AccountUID, UnitUID, ServerGroupID, ChatText, ChatType, RecordedUtc FROM ChatLogs WHERE UnitUID = $unit ORDER BY LogID DESC LIMIT $limit;";
        cmd.Parameters.AddWithValue("$unit", unitUid);
        cmd.Parameters.AddWithValue("$limit", Math.Max(1, limit));
        using var r = cmd.ExecuteReader();
        while (r.Read())
            result.Add((r.GetInt64(0), r.GetInt64(1), r.GetInt32(2), r.GetString(3), r.GetInt32(4), r.GetString(5)));
        return result;
    }

    public void UpsertJackpotEventCoupon(
        string couponNumber,
        string beginDate,
        string endDate,
        long userUid = 0,
        long unitUid = 0,
        int state = 0)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                INSERT INTO JackpotEventCoupons (CouponNumber, BeginDate, EndDate, UserUID, UnitUID, State)
                VALUES ($cpn, $begin, $end, $user, $unit, $state)
                ON CONFLICT(CouponNumber) DO UPDATE SET
                    BeginDate = excluded.BeginDate,
                    EndDate = excluded.EndDate,
                    UserUID = excluded.UserUID,
                    UnitUID = excluded.UnitUID,
                    State = excluded.State;
                """;
            cmd.Parameters.AddWithValue("$cpn", couponNumber ?? string.Empty);
            cmd.Parameters.AddWithValue("$begin", beginDate ?? string.Empty);
            cmd.Parameters.AddWithValue("$end", endDate ?? string.Empty);
            cmd.Parameters.AddWithValue("$user", userUid);
            cmd.Parameters.AddWithValue("$unit", unitUid);
            cmd.Parameters.AddWithValue("$state", state);
            cmd.ExecuteNonQuery();
        }
    }

    public List<(string CouponNumber, string BeginDate, string EndDate, long UserUid, long UnitUid, int State)> GetJackpotEventCoupons()
    {
        var result = new List<(string, string, string, long, long, int)>();
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT CouponNumber, BeginDate, EndDate, UserUID, UnitUID, State FROM JackpotEventCoupons ORDER BY BeginDate, CouponNumber;";
        using var r = cmd.ExecuteReader();
        while (r.Read())
            result.Add((r.GetString(0), r.GetString(1), r.GetString(2), r.GetInt64(3), r.GetInt64(4), r.GetInt32(5)));
        return result;
    }

    public int UpdateJackpotEventCouponWinner(string couponNumber, long userUid, long unitUid, int state)
    {
        lock (_writeLock)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                UPDATE JackpotEventCoupons
                SET UserUID = $user, UnitUID = $unit, State = $state
                WHERE CouponNumber = $cpn;
                """;
            cmd.Parameters.AddWithValue("$user", userUid);
            cmd.Parameters.AddWithValue("$unit", unitUid);
            cmd.Parameters.AddWithValue("$state", state);
            cmd.Parameters.AddWithValue("$cpn", couponNumber ?? string.Empty);
            return cmd.ExecuteNonQuery() > 0 ? NetError.NET_OK : NetError.ERR_ODBC_01;
        }
    }
}
