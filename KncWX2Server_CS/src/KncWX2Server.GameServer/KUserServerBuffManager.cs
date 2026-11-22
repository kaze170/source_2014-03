using KncWX2Server.Core.Framework;
using KncWX2Server.Core.Packets;

namespace KncWX2Server.GameServer;

/// <summary>
/// Faithful C# port of GameServer/UserServerBuffManager.{h,cpp} (SERV_SERVER_BUFF_SYSTEM):
/// manages timed server buffs, bonus EXP/ED/Drop rates, room-leave deactivation, and death expiry.
/// </summary>
public sealed class KUserServerBuffManager
{
    public sealed class ServerBuffInfo
    {
        public int BuffId { get; init; }
        public float ExpRate { get; init; }
        public float EdRate { get; init; }
        public float DropRate { get; init; }
        public bool DeactivateOnLeaveField { get; init; }
        public bool DeactivateOnEndDungeon { get; init; }
        public bool DeactivateOnDeath { get; init; }
        public DateTime StartUtc { get; init; } = DateTime.UtcNow;
        public DateTime? ExpireUtc { get; init; }
    }

    private readonly Dictionary<int, ServerBuffInfo> _buffs = new();
    private readonly object _sync = new();

    public void Clear()
    {
        lock (_sync) _buffs.Clear();
    }

    public bool ActivateBuff(ServerBuffInfo buff)
    {
        lock (_sync)
        {
            _buffs[buff.BuffId] = buff;
            return true;
        }
    }

    public bool ActivateFromTemplet(
        KBuffManager.BuffTemplet templet,
        float expRate = 0f,
        float edRate = 0f,
        float dropRate = 0f,
        DateTime? nowUtc = null)
    {
        DateTime? expire = templet.BuffPeriodSecond > 0
            ? (nowUtc ?? DateTime.UtcNow).AddSeconds(templet.BuffPeriodSecond)
            : null;
        return ActivateBuff(new ServerBuffInfo
        {
            BuffId = templet.BuffId,
            ExpRate = expRate,
            EdRate = edRate,
            DropRate = dropRate,
            DeactivateOnLeaveField = templet.EndBuffAtTheLeaveBattleFieldRoom || templet.EndBuffAtTheLeaveDungeonRoom,
            DeactivateOnEndDungeon = templet.EndBuffAtTheEndDungeonGame,
            DeactivateOnDeath = templet.EndBuffAtTheUserDied,
            StartUtc = nowUtc ?? DateTime.UtcNow,
            ExpireUtc = expire,
        });
    }

    public List<KBuffInfo> ToPacketBuffInfos(DateTime? nowUtc = null)
    {
        var now = nowUtc ?? DateTime.UtcNow;
        var list = new List<KBuffInfo>();
        lock (_sync)
        {
            uint seq = 1;
            foreach (var b in _buffs.Values)
            {
                if (b.ExpireUtc.HasValue && now >= b.ExpireUtc.Value) continue;
                list.Add(new KBuffInfo
                {
                    FactorInfo = new KBuffFactor
                    {
                        BuffIdentity = new KBuffIdentity
                        {
                            BuffTempletId = b.BuffId,
                            UniqueNum = seq++,
                        },
                        FactorId = b.BuffId,
                    },
                    BuffStartTime = new DateTimeOffset(b.StartUtc).ToUnixTimeSeconds(),
                    BuffEndTime = b.ExpireUtc.HasValue ? new DateTimeOffset(b.ExpireUtc.Value).ToUnixTimeSeconds() : 0,
                });
            }
        }
        return list;
    }

    public bool DeactivateBuff(int buffId)
    {
        lock (_sync) return _buffs.Remove(buffId);
    }

    public bool IsBuffActivated(int buffId)
    {
        lock (_sync) return _buffs.ContainsKey(buffId);
    }

    public (float expBonus, float edBonus, float dropBonus) GetBuffBonusRates(DateTime? nowUtc = null)
    {
        var now = nowUtc ?? DateTime.UtcNow;
        float exp = 0f, ed = 0f, drop = 0f;
        lock (_sync)
        {
            foreach (var b in _buffs.Values)
            {
                if (b.ExpireUtc.HasValue && now >= b.ExpireUtc.Value) continue;
                exp += b.ExpRate;
                ed += b.EdRate;
                drop += b.DropRate;
            }
        }
        return (exp, ed, drop);
    }

    public List<int> CheckBuffPeriod(DateTime? nowUtc = null)
    {
        var now = nowUtc ?? DateTime.UtcNow;
        var expired = new List<int>();
        lock (_sync)
        {
            foreach (var kv in _buffs.Where(k => k.Value.ExpireUtc.HasValue && now >= k.Value.ExpireUtc.Value).ToList())
            {
                _buffs.Remove(kv.Key);
                expired.Add(kv.Key);
            }
        }
        return expired;
    }

    public List<int> OnLeaveField()
    {
        lock (_sync)
        {
            var removed = _buffs.Where(k => k.Value.DeactivateOnLeaveField).Select(k => k.Key).ToList();
            foreach (int id in removed) _buffs.Remove(id);
            return removed;
        }
    }

    public List<int> OnEndDungeonGame()
    {
        lock (_sync)
        {
            var removed = _buffs.Where(k => k.Value.DeactivateOnEndDungeon).Select(k => k.Key).ToList();
            foreach (int id in removed) _buffs.Remove(id);
            return removed;
        }
    }

    public List<int> OnUserDied()
    {
        lock (_sync)
        {
            var removed = _buffs.Where(k => k.Value.DeactivateOnDeath).Select(k => k.Key).ToList();
            foreach (int id in removed) _buffs.Remove(id);
            return removed;
        }
    }
}
