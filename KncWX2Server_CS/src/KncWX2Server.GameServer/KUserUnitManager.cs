namespace KncWX2Server.GameServer;

/// <summary>
/// Faithful C# port of GameServer/UserUnitManager.{h,cpp} (SERV_BATTLE_FIELD_SYSTEM &amp; SERV_VISIT_CASH_SHOP):
/// maintains active character HP/MP gameplay status, detects party HP/MP percentage changes,
/// manages return-to-field position reservations across dungeon/cash-shop transitions.
/// </summary>
public sealed class KUserUnitManager
{
    public sealed class KGamePlayStatus
    {
        public int MaxHp { get; set; }
        public int CurHp { get; set; }
        public int MaxMp { get; set; } = 300;
        public int CurMp { get; set; } = 300;
    }

    public sealed class KPartyMemberStatus
    {
        public float HpPercent { get; set; } = 100f;
        public float MpPercent { get; set; } = 100f;
    }

    public sealed record KReturnToFieldInfo(int MapId, int StartPosId, float PosX, float PosY, float PosZ);

    private readonly Stack<KReturnToFieldInfo> _reservedReturnToField = new();
    private readonly object _sync = new();

    public KGamePlayStatus GamePlayStatus { get; } = new();
    public KPartyMemberStatus PartyMemberStatus { get; } = new();
    public bool EnterCashShop { get; set; }

    public void Clear()
    {
        lock (_sync)
        {
            GamePlayStatus.MaxHp = 0;
            GamePlayStatus.CurHp = 0;
            GamePlayStatus.MaxMp = 300;
            GamePlayStatus.CurMp = 300;
            PartyMemberStatus.HpPercent = 100f;
            PartyMemberStatus.MpPercent = 100f;
            _reservedReturnToField.Clear();
            EnterCashShop = false;
        }
    }

    public KGamePlayStatus Init(KGamePlayStatus? dbStatus, int baseHp)
    {
        int safeBaseHp = Math.Max(1, baseHp);
        lock (_sync)
        {
            if (dbStatus == null || dbStatus.MaxHp <= 0)
            {
                GamePlayStatus.MaxMp = 300;
                GamePlayStatus.CurMp = 300;
                GamePlayStatus.MaxHp = safeBaseHp;
                GamePlayStatus.CurHp = safeBaseHp;
            }
            else
            {
                GamePlayStatus.MaxMp = dbStatus.MaxMp > 0 ? dbStatus.MaxMp : 300;
                GamePlayStatus.CurMp = Math.Clamp(dbStatus.CurMp, 0, GamePlayStatus.MaxMp);
                GamePlayStatus.MaxHp = safeBaseHp;
                GamePlayStatus.CurHp = Math.Clamp(dbStatus.CurHp, 0, safeBaseHp);
            }

            RecalculatePartyPercentagesLocked();
            return new KGamePlayStatus
            {
                MaxHp = GamePlayStatus.MaxHp,
                CurHp = GamePlayStatus.CurHp,
                MaxMp = GamePlayStatus.MaxMp,
                CurMp = GamePlayStatus.CurMp,
            };
        }
    }

    public bool SetGamePlayStatusAndCheckChangedHpMp(KGamePlayStatus nextStatus, out KPartyMemberStatus updatedHpMp)
    {
        lock (_sync)
        {
            bool changed = GamePlayStatus.CurHp != nextStatus.CurHp ||
                           GamePlayStatus.MaxHp != nextStatus.MaxHp ||
                           GamePlayStatus.CurMp != nextStatus.CurMp ||
                           GamePlayStatus.MaxMp != nextStatus.MaxMp;

            GamePlayStatus.MaxHp = Math.Max(1, nextStatus.MaxHp);
            GamePlayStatus.CurHp = Math.Clamp(nextStatus.CurHp, 0, GamePlayStatus.MaxHp);
            GamePlayStatus.MaxMp = Math.Max(1, nextStatus.MaxMp);
            GamePlayStatus.CurMp = Math.Clamp(nextStatus.CurMp, 0, GamePlayStatus.MaxMp);

            RecalculatePartyPercentagesLocked();
            updatedHpMp = new KPartyMemberStatus
            {
                HpPercent = PartyMemberStatus.HpPercent,
                MpPercent = PartyMemberStatus.MpPercent,
            };
            return changed;
        }
    }

    public void ReserveReturnToFieldInfo(KReturnToFieldInfo info)
    {
        lock (_sync) _reservedReturnToField.Push(info);
    }

    public bool TryPopReservedReturnToFieldInfo(out KReturnToFieldInfo? info)
    {
        lock (_sync)
        {
            if (_reservedReturnToField.Count > 0)
            {
                info = _reservedReturnToField.Pop();
                return true;
            }
            info = null;
            return false;
        }
    }

    private void RecalculatePartyPercentagesLocked()
    {
        PartyMemberStatus.HpPercent = GamePlayStatus.MaxHp > 0
            ? (float)GamePlayStatus.CurHp / GamePlayStatus.MaxHp * 100f
            : 0f;
        PartyMemberStatus.MpPercent = GamePlayStatus.MaxMp > 0
            ? (float)GamePlayStatus.CurMp / GamePlayStatus.MaxMp * 100f
            : 0f;
    }
}
