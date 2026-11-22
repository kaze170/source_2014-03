namespace KncWX2Server.GameServer;

/// <summary>
/// Faithful C# port of GameServer/UserPcBangManager.{h,cpp}:
/// tracks PC Bang authentication status, tier type, bonus EXP/ED/Drop rates, and pending notifications.
/// </summary>
public sealed class KUserPcBangManager
{
    public bool CanTry { get; set; } = true;
    public bool IsPcBang { get; set; }
    public int PcBangType { get; set; } = -1;
    public byte AuthResult { get; private set; }
    public byte AuthorizeType { get; private set; }
    public byte ChargeType { get; private set; }
    public bool IsReservedPcBangMessage { get; private set; }
    public uint MessageArgument { get; private set; }

    public void InitPcBangAuth(byte authResult, byte authorizeType, byte chargeType)
    {
        AuthResult = authResult;
        AuthorizeType = authorizeType;
        ChargeType = chargeType;
        IsPcBang = authResult == 1;
    }

    public void SetPcBangMessage(uint argument)
    {
        MessageArgument = argument;
        IsReservedPcBangMessage = true;
    }

    public uint ConsumePcBangMessageArgument()
    {
        IsReservedPcBangMessage = false;
        return MessageArgument;
    }

    public (float expBonus, float edBonus, float dropBonus, bool unlimitedSpirit) GetPcBangReward()
    {
        if (!IsPcBang) return (0f, 0f, 0f, false);
        return PcBangType >= 2
            ? (0.50f, 0.30f, 0.30f, true)
            : (0.30f, 0.20f, 0.20f, false);
    }
}
