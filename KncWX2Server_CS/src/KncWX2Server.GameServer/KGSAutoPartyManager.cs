namespace KncWX2Server.GameServer;

/// <summary>
/// Faithful C# port of GameServer/GSAutoPartyManager.{h,cpp} (SERV_BATTLE_FIELD_SYSTEM):
/// tracks auto-party bonus dungeon status keyed by (DungeonMode, DungeonIdWithDif) and increments UpdateTick.
/// </summary>
public sealed class KGSAutoPartyManager
{
    private readonly Dictionary<(sbyte DungeonMode, int DungeonIdWithDif), bool> _bonusMap = new();
    private readonly object _sync = new();

    public long UpdateTick { get; private set; }

    public void UpdateAutoPartyBonusInfo(sbyte dungeonMode, int dungeonIdWithDif, bool activatedBonus)
    {
        lock (_sync)
        {
            _bonusMap[(dungeonMode, dungeonIdWithDif)] = activatedBonus;
            UpdateTick++;
        }
    }

    public bool IsAutoPartyBonus(sbyte dungeonMode, int dungeonIdWithDif)
    {
        lock (_sync)
        {
            return _bonusMap.TryGetValue((dungeonMode, dungeonIdWithDif), out bool active) && active;
        }
    }

    public List<KncWX2Server.Core.Packets.KAutoPartyBonusInfo> SnapshotAll()
    {
        lock (_sync)
        {
            return _bonusMap
                .Select(kv => new KncWX2Server.Core.Packets.KAutoPartyBonusInfo
                {
                    DungeonMode = kv.Key.DungeonMode,
                    DungeonIdWithDif = kv.Key.DungeonIdWithDif,
                    ActivatedBonus = kv.Value,
                })
                .ToList();
        }
    }

    public string Dump()
    {
        lock (_sync)
        {
            return $"-------------- GSAutoPartyManager --------------\nCount={_bonusMap.Count}, UpdateTick={UpdateTick}";
        }
    }
}
