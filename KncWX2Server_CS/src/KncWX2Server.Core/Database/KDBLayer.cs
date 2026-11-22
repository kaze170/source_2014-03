using KncWX2Server.Core.Events;
using KncWX2Server.Core.Framework;
using KncWX2Server.Core.Logging;

namespace KncWX2Server.Core.Database;

public enum DbConnectionType
{
    DC_ACCOUNT = 0,
    DC_GAME = 1,
    DC_LOG = 2,
    DC_SMS = 3,
    DC_NX_SHOP_GATEWAY = 4,
    DC_NX_WEB = 5,
    DC_NETMARBLE_BILLING = 6,
    DC_MANAGER = 7,
    DC_KOG_BILLING = 8,
}

/// <summary>
/// Port of Common/DBLayer.{h,cpp} managing regional SQLite database and registered DB thread pools.
/// </summary>
public sealed class KDBLayer
{
    private readonly List<KDBThread> _threads = [];
    private readonly Dictionary<DbConnectionType, (string Dsn, int ThreadCount)> _configs = new();

    public SqliteDatabaseManager Sqlite { get; private set; }

    public KDBLayer(string dbPath = "KncWX2Server.db")
    {
        Sqlite = new SqliteDatabaseManager(dbPath);
    }

    public void Initialize(string? dbPath = null)
    {
        if (!string.IsNullOrWhiteSpace(dbPath))
            Sqlite = new SqliteDatabaseManager(dbPath);
        Sqlite.InitializeAndSeed();
    }

    public void AddDB(int dbType, string dsn, int threadCount, bool isConnString)
    {
        var type = (DbConnectionType)dbType;
        _configs[type] = (dsn, Math.Max(1, threadCount));
        KLog.Clog($"DBLayer:AddDB({type}, dsn={dsn}, threads={threadCount}, isConnStr={isConnString}) -> backed by SQLite");
    }

    public void SetManagerDB(int serverClass, string dsn, bool isConnString)
    {
        _configs[DbConnectionType.DC_MANAGER] = (dsn, 1);
    }

    public void RegisterThread(uint performerId, KDBThread thread, InProcessRouter router)
    {
        _threads.Add(thread);
        router.RegisterRoute(performerId, ev =>
        {
            thread.QueueingEvent(ev);
            return true;
        });
        thread.Start();
    }

    public void RegToLua(Scripting.KScriptManager lua)
    {
        lua.RegisterDBLayer(this);
    }

    public void ShutDown()
    {
        foreach (var t in _threads)
            t.Stop();
        _threads.Clear();
    }
}
