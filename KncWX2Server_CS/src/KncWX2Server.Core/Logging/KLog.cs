using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using KncWX2Server.Core.Packets;

namespace KncWX2Server.Core.Logging;

public enum LogLevel
{
    None = -1,
    ErrorAndInfo = 0, // cerr, cout
    Warn = 1,         // cerr, cwarn, cout
    All = 2,          // cerr, cwarn, clog, cout
}

public enum LogCategory
{
    Cout,
    Cerr,
    Cwarn,
    Clog,
}

public readonly record struct LogEntry(DateTime Timestamp, LogCategory Category, string Message, string Caller, string File, int Line);

/// <summary>
/// Thread-safe logging facility matching START_LOG(cerr/cwarn/clog/cout, ...) and SERV_LOG_SYSTEM_NEW statistics.
/// </summary>
public static class KLog
{
    private static volatile int _level = (int)LogLevel.All;
    private static readonly ConcurrentDictionary<(string Func, string File, int Line), int> _errorStats = new();

    public static event Action<LogEntry>? OnLog;

    public static int Level
    {
        get => _level;
        set => _level = value;
    }

    public static int ErrorLogStatTimeGapSeconds { get; set; } = 600;
    public static long LogFileMaxSizeBytes { get; set; } = 100 * 1024 * 1024;
    public static bool NewLogByFileSize { get; set; }
    public static bool NewLogDaily { get; set; }
    public static bool ErrorLogStatEnabled { get; set; } = true;
    public static int CheckTimeGapSeconds { get; set; } = 300;

    public static void SetLevel(int level) => _level = level;

    public static void Write(
        LogCategory category,
        string message,
        [CallerMemberName] string caller = "",
        [CallerFilePath] string file = "",
        [CallerLineNumber] int line = 0)
    {
        if (category == LogCategory.Cerr)
        {
            string fileName = Path.GetFileName(file);
            _errorStats.AddOrUpdate((caller, fileName, line), 1, static (_, c) => c + 1);
        }

        int lvl = _level;
        bool shouldEmit = category switch
        {
            LogCategory.Cout or LogCategory.Cerr => lvl >= 0,
            LogCategory.Cwarn => lvl >= 1,
            LogCategory.Clog => lvl >= 2,
            _ => false,
        };

        if (!shouldEmit)
            return;

        var entry = new LogEntry(DateTime.Now, category, message, caller, Path.GetFileName(file), line);
        OnLog?.Invoke(entry);
    }

    public static void Cout(string message, [CallerMemberName] string caller = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        => Write(LogCategory.Cout, message, caller, file, line);

    public static void Cerr(string message, [CallerMemberName] string caller = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        => Write(LogCategory.Cerr, message, caller, file, line);

    public static void Cwarn(string message, [CallerMemberName] string caller = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        => Write(LogCategory.Cwarn, message, caller, file, line);

    public static void Clog(string message, [CallerMemberName] string caller = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        => Write(LogCategory.Clog, message, caller, file, line);

    public static List<KLogStatInfo> DrainErrorStatistics()
    {
        var list = new List<KLogStatInfo>();
        foreach (var kv in _errorStats)
        {
            if (_errorStats.TryRemove(kv.Key, out int count) && count > 0)
            {
                list.Add(new KLogStatInfo
                {
                    FunctionName = kv.Key.Func,
                    FileName = kv.Key.File,
                    LineNum = kv.Key.Line.ToString(),
                    Count = count,
                });
            }
        }
        return list;
    }
}
