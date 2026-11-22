using KncWX2Server.Core.Packets;

namespace KncWX2Server.GameServer;

/// <summary>
/// Faithful C# port of Common/X2Data/XSLRidingPetManager.{h,cpp} and GameServer/UserRidingPet.{h,cpp}:
/// stores riding pet templates, create-item mappings, offline stamina regeneration
/// ((Now - LastUnSummonDate) / (RecoveryTimePerOne * StaminaRecoveryRate)), and expiration checks.
/// </summary>
public sealed class KRidingPetManager
{
    public sealed class RidingPetTemplet
    {
        public ushort RidingPetId { get; init; }
        public ushort MaxStamina { get; init; } = 100;
        public float StaminaRecoveryRate { get; init; } = 1.0f;
    }

    private readonly Dictionary<ushort, RidingPetTemplet> _templets = new();
    private readonly Dictionary<int, (ushort ridingPetId, short period)> _createItems = new();

    public float EnableStamina { get; set; } = 10.0f;
    public float RecoveryTimePerOne { get; set; } = 3.0f; // seconds per 1 stamina point

    public void AddRidingPetTemplet(ushort ridingPetId, ushort maxStamina, float staminaRecoveryRate)
    {
        _templets[ridingPetId] = new RidingPetTemplet
        {
            RidingPetId = ridingPetId,
            MaxStamina = maxStamina,
            StaminaRecoveryRate = staminaRecoveryRate,
        };
    }

    public void AddRidingPetCreateItemInfo(int itemId, ushort ridingPetId, short period)
        => _createItems[itemId] = (ridingPetId, period);

    public RidingPetTemplet? GetRidingPetTemplet(ushort ridingPetId)
        => _templets.GetValueOrDefault(ridingPetId);

    public bool IsExistRidingPetId(ushort ridingPetId)
        => _templets.ContainsKey(ridingPetId);

    public (ushort ridingPetId, short period) GetRidingPetId(int itemId)
        => _createItems.TryGetValue(itemId, out var pair) ? pair : ((ushort)0, (short)0);

    public KRidingPetInfo ApplyOfflineRecovery(KRidingPetInfo info, bool created, int? nowUnixOverride = null)
    {
        int nowUnix = nowUnixOverride ?? (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var templet = GetRidingPetTemplet(info.RidingPetId);
        if (templet == null)
        {
            info.Stamina = 0f;
        }
        else
        {
            float denom = RecoveryTimePerOne * templet.StaminaRecoveryRate;
            if (denom > 0f && nowUnix > info.LastUnSummonDate)
                info.Stamina += (nowUnix - info.LastUnSummonDate) / denom;

            if (templet.MaxStamina < info.Stamina || info.Stamina < 0f)
                info.Stamina = templet.MaxStamina;
        }

        if (created)
            info.LastUnSummonDate = nowUnix;

        return info;
    }

    public bool EnableCheck(KRidingPetInfo info) => EnableStamina <= info.Stamina;

    public bool IsExpirationRidingPet(KRidingPetInfo info, DateTime? nowUtcOverride = null)
    {
        if (string.IsNullOrWhiteSpace(info.DestroyDate))
            return false;
        if (!DateTime.TryParse(info.DestroyDate, out var destroyAt))
            return false;
        return destroyAt <= (nowUtcOverride ?? DateTime.UtcNow);
    }

    public bool CheckRidingPetStamina(ushort ridingPetId, float stamina)
    {
        var templet = GetRidingPetTemplet(ridingPetId);
        return templet != null && stamina >= 0f && stamina <= templet.MaxStamina;
    }
}
