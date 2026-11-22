using KncWX2Server.Core.Logging;

namespace KncWX2Server.GameServer;

/// <summary>
/// Faithful C# port of GameServer/GSContentManager.{h,cpp} (SERV_CONTENT_MANAGER_INT):
/// manages live content toggles (e.g. CashShop open/close) and release-tick versioning.
/// </summary>
public sealed class KGSContentManager
{
    public bool IsEnableCashShop { get; set; } = true;
    public int ReleaseTick { get; set; } = -1;

    public void Init(bool enableCashShop = true, int releaseTick = -1)
    {
        IsEnableCashShop = enableCashShop;
        ReleaseTick = releaseTick;
    }

    public void SetEnableCashShop(bool enable)
    {
        if (IsEnableCashShop != enable)
            KLog.Cout($"[ContentManager] CashShop enabled: {enable}");
        IsEnableCashShop = enable;
    }

    public void SetEnableCashShopLua(bool enable)
    {
        IsEnableCashShop = enable;
        KLog.Cout($"[ContentManager] CashShop Lua setting: {enable}");
    }

    public string Dump() =>
        $"----------[ Game Server Content Manager ]----------\nIsEnableCashShop={IsEnableCashShop}, ReleaseTick={ReleaseTick}";
}
