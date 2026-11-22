namespace KncWX2Server.GameServer;

/// <summary>
/// Faithful C# port of GameServer/UserEDManager.{h,cpp} (SERV_ED_AND_ITEM_MORNITORING_SYSTEM):
/// overflow-safe MAX_ED (2,000,000,000) cap enforcement, reason-tagged ED mutations, and audit trail.
/// </summary>
public sealed class KUserEDManager
{
    public const int MaxEd = 2_000_000_000;

    public enum EdReason
    {
        ErNoReason = 0,
        ErSetEdSelectUnit,
        ErSetEdSyncForSendLetter,
        ErSetEdPersonalShopSellItem,
        ErSetEdPersonalShopBuyItem,
        ErSetEdPShopAgencySellItem,
        ErSetEdPShopAgencyBuyItem,
        ErSetEdInTrade,
        ErSetEdOutTrade,
        ErInEdDungeonDrop,
        ErInEdSellItemNpcShop,
        ErInEdDungeonClearBonus,
        ErInEdGetEdFromLetter,
        ErInEdTrainingReward,
        ErInEdQuestReward,
        ErOutEdBuyItemNpcShop,
        ErOutEdItemRepair,
        ErOutEdItemManufacture,
        ErOutEdItemEnchant,
        ErOutEdItemSocket,
        ErOutEdItemAttribute,
        ErOutEdSendLetterCommission,
        ErOutEdPersonalShopOpenCommission,
        ErOutEdUseCubeOpen,
        ErOutEdBreakUpRelationship,
        ErOutEdExpandSkillPage,
        ErOutEdWarp,
    }

    public readonly record struct EdAuditEntry(EdReason Reason, int Delta, int AfterEd, DateTime AtUtc);

    private readonly List<EdAuditEntry> _ledger = [];
    public int CurrentEd { get; private set; }
    public IReadOnlyList<EdAuditEntry> Ledger => _ledger;

    public void Init(int initialEd)
    {
        CurrentEd = Math.Clamp(initialEd, 0, MaxEd);
        _ledger.Clear();
        _ledger.Add(new EdAuditEntry(EdReason.ErSetEdSelectUnit, CurrentEd, CurrentEd, DateTime.UtcNow));
    }

    public void SetEd(int newEd, EdReason reason)
    {
        int clamped = Math.Clamp(newEd, 0, MaxEd);
        int delta = clamped - CurrentEd;
        CurrentEd = clamped;
        _ledger.Add(new EdAuditEntry(reason, delta, CurrentEd, DateTime.UtcNow));
    }

    public bool TryAddEd(int delta, EdReason reason, out int afterEd)
    {
        long next = (long)CurrentEd + delta;
        if (next < 0)
        {
            afterEd = CurrentEd;
            return false;
        }
        CurrentEd = (int)Math.Min(MaxEd, next);
        afterEd = CurrentEd;
        _ledger.Add(new EdAuditEntry(reason, delta, CurrentEd, DateTime.UtcNow));
        return true;
    }

    public bool TryConsumeEd(int cost, EdReason reason, out int afterEd)
    {
        if (cost < 0)
        {
            afterEd = CurrentEd;
            return false;
        }
        return TryAddEd(-cost, reason, out afterEd);
    }
}
