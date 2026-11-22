using System.Collections.Concurrent;

namespace KncWX2Server.GlobalServer;

/// <summary>
/// Faithful C# port of GlobalServer/BlockListManager.{h,cpp} (SERV_BLOCK_LIST):
/// tracks Trade, Account, IP, and Hacking DLL module blocks across the global shard.
/// </summary>
public sealed class KGlobalBlockListManager
{
    private readonly ConcurrentDictionary<long, string> _tradeBlocks = new();
    private readonly ConcurrentDictionary<long, string> _accountBlocks = new();
    private readonly ConcurrentDictionary<string, string> _ipBlocks = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, string> _dllBlocks = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, int> _observedModules = new(StringComparer.OrdinalIgnoreCase);

    public int ReleaseTick { get; set; }

    public void SetTradeBlock(long userUid, bool blocked, string reason = "")
    {
        if (blocked) _tradeBlocks[userUid] = reason;
        else _tradeBlocks.TryRemove(userUid, out _);
    }

    public void SetAccountBlock(long userUid, bool blocked, string reason = "")
    {
        if (blocked) _accountBlocks[userUid] = reason;
        else _accountBlocks.TryRemove(userUid, out _);
    }

    public void SetIpBlock(string ip, bool blocked, string reason = "")
    {
        if (blocked) _ipBlocks[ip] = reason;
        else _ipBlocks.TryRemove(ip, out _);
    }

    public void AddHackingModule(string moduleName, string reason = "HackModule")
    {
        if (!string.IsNullOrWhiteSpace(moduleName))
            _dllBlocks[moduleName.Trim()] = reason;
    }

    public void UpdateModuleList(IEnumerable<string> modules)
    {
        foreach (string mod in modules)
        {
            if (!string.IsNullOrWhiteSpace(mod))
                _observedModules.AddOrUpdate(mod.Trim(), 1, static (_, c) => c + 1);
        }
    }

    public bool IsTradeBlockUser(long userUid) => _tradeBlocks.ContainsKey(userUid);
    public bool IsAccountBlockUser(long userUid) => _accountBlocks.ContainsKey(userUid);
    public bool IsIpBlockUser(string ip) => _ipBlocks.ContainsKey(ip);

    public List<string> CheckHackingModuleList(IEnumerable<string> clientModules)
    {
        var matched = new List<string>();
        foreach (string mod in clientModules)
        {
            if (_dllBlocks.ContainsKey(mod.Trim()))
                matched.Add(mod);
        }
        return matched;
    }
}
