using KncWX2Server.Core.Region;

namespace KncWX2Server.GameServer;

/// <summary>
/// Faithful C# port of GameServer/UserRelationshipManager.{h,cpp} (SERV_RELATIONSHIP_SYSTEM)
/// &amp; XSLUnit::GetUnitClassToUnitType / IsMan_UnitType:
/// relationship state (Solo/Couple/Married), sibling guard (Elsword &lt;-&gt; Elesis = ERR_RELATIONSHIP_47),
/// same-gender guard (ERR_RELATIONSHIP_05), 2-day couple duration check, breakup cost, and anniversary reward checks.
/// </summary>
public sealed class KUserRelationshipManager
{
    public enum RelationshipType : sbyte
    {
        RtSolo = 0,
        RtCouple = 1,
        RtMarried = 2,
    }

    public const int BreakUpCostCouple = 100_000;
    public const int BreakUpCostMarried = 200_000;
    public const int BreakUpCostMax = 100_000_000;

    public const int CiRequestOfLove = 135404;
    public const int SiWeddingRingItem = 269711;
    public const int SiWeddingHallAItem = 269712;
    public const int SiWeddingHallBItem = 269713;
    public const int SiWeddingInviteTicketItem = 269714;
    public const int SiWeddingReservationItem = 269715;
    public const int SiWeddingInvitationItem = 269716;
    public const int SiLoveWordChangeItem = 269718;

    public const sbyte WhtCashA = 0;
    public const sbyte WhtCashB = 1;
    public const sbyte WhtEnd = 2;

    public const sbyte WotNpcA = 0;
    public const sbyte WotNpcEnd = 3;

    public const sbyte WsNone = 0;
    public const sbyte WsGroom = 1;
    public const sbyte WsBride = 2;
    public const sbyte WsGuest = 3;

    public const sbyte UnitTypeNone = 0;
    public const sbyte UnitTypeElsword = 1;
    public const sbyte UnitTypeArme = 2;
    public const sbyte UnitTypeLire = 3;
    public const sbyte UnitTypeRaven = 4;
    public const sbyte UnitTypeEve = 5;
    public const sbyte UnitTypeChung = 6;
    public const sbyte UnitTypeAra = 7;
    public const sbyte UnitTypeElesis = 8;
    public const sbyte UnitTypeAdd = 9;

    public RelationshipType CurrentType { get; private set; } = RelationshipType.RtSolo;
    public long LoverUnitUid { get; set; }
    public string LoverNickName { get; set; } = string.Empty;
    public string LoveWord { get; set; } = string.Empty;
    public DateTime? AnniversaryDayUtc { get; set; }
    public DateTime? LastRewardDateUtc { get; set; }
    public sbyte RewardTitleType { get; set; }

    public bool IsSolo => CurrentType == RelationshipType.RtSolo;
    public bool IsCouple => CurrentType == RelationshipType.RtCouple;
    public bool IsMarried => CurrentType == RelationshipType.RtMarried;

    public bool SetRelationshipType(RelationshipType type)
    {
        CurrentType = type;
        return true;
    }

    public static sbyte GetUnitClassToUnitType(int unitClass) => unitClass switch
    {
        1 or 10 or 11 or 22 or 100 or 101 or 112 => UnitTypeElsword,
        2 or 14 or 15 or 23 or 104 or 105 or 113 => UnitTypeArme,
        3 or 12 or 13 or 24 or 102 or 103 or 114 => UnitTypeLire,
        4 or 16 or 17 or 25 or 106 or 107 or 115 => UnitTypeRaven,
        5 or 18 or 19 or 26 or 108 or 109 or 116 => UnitTypeEve,
        6 or 20 or 21 or 27 or 110 or 111 or 117 => UnitTypeChung,
        7 or 28 or 31 or 118 or 119 => UnitTypeAra,
        8 or 29 or 30 or 120 or 121 => UnitTypeElesis,
        9 or 32 or 122 => UnitTypeAdd,
        _ => UnitTypeNone,
    };

    public static bool IsManUnitType(sbyte unitType) =>
        unitType is UnitTypeElsword or UnitTypeRaven or UnitTypeChung or UnitTypeAdd;

    /// <summary>
    /// Exact C++ KUserRelationshipManager::CheckChar(cUnitClassA, cUnitClassB):
    /// converts UnitClass to UnitType, forbids Elsword &lt;-&gt; Elesis (ERR_RELATIONSHIP_47 = 1217),
    /// and forbids same-gender couples (ERR_RELATIONSHIP_05 = 1125).
    /// </summary>
    public static int CheckChar(int unitClassA, int unitClassB)
    {
        sbyte typeA = GetUnitClassToUnitType(unitClassA);
        sbyte typeB = GetUnitClassToUnitType(unitClassB);

        if ((typeA == UnitTypeElsword && typeB == UnitTypeElesis) ||
            (typeA == UnitTypeElesis && typeB == UnitTypeElsword))
            return NetError.ERR_RELATIONSHIP_47;

        if (typeA != UnitTypeNone && typeB != UnitTypeNone &&
            IsManUnitType(typeA) == IsManUnitType(typeB))
            return NetError.ERR_RELATIONSHIP_05;

        return NetError.NET_OK;
    }

    public bool CheckCoupleDate(DateTime? nowUtc = null)
        => AnniversaryDayUtc.HasValue && (nowUtc ?? DateTime.UtcNow) >= AnniversaryDayUtc.Value.AddDays(2);

    public static bool IsValidWeddingOfficiantNpc(sbyte officiantNpc)
        => officiantNpc >= WotNpcA && officiantNpc < WotNpcEnd;

    public static bool IsValidWeddingHall(sbyte weddingHallType)
        => weddingHallType >= WhtCashA && weddingHallType < WhtEnd;

    public static int GetWeddingHallItem(sbyte weddingHallType) => weddingHallType switch
    {
        WhtCashA => SiWeddingHallAItem,
        WhtCashB => SiWeddingHallBItem,
        _ => 0,
    };

    public static bool IsWeddingItem(int itemId, out sbyte weddingStatus)
    {
        switch (itemId)
        {
            case SiWeddingInvitationItem:
                weddingStatus = WsGuest;
                return true;
            case SiWeddingReservationItem:
                weddingStatus = WsGroom;
                return true;
            default:
                weddingStatus = WsNone;
                return false;
        }
    }

    public int GetBreakUpCost(DateTime? nowUtc = null)
    {
        int days = 1;
        if (AnniversaryDayUtc.HasValue)
        {
            var span = (nowUtc ?? DateTime.UtcNow) - AnniversaryDayUtc.Value;
            days = Math.Max(1, (int)(span.TotalHours / 24.0) + 1);
        }

        return CurrentType switch
        {
            RelationshipType.RtCouple => (int)Math.Min((long)days * BreakUpCostCouple, BreakUpCostMax),
            RelationshipType.RtMarried => (int)Math.Min((long)days * BreakUpCostMarried, BreakUpCostMax),
            _ => 0,
        };
    }

    public void SetBreakUp()
    {
        CurrentType = RelationshipType.RtSolo;
        LoverUnitUid = 0;
        LoverNickName = string.Empty;
        LoveWord = string.Empty;
        AnniversaryDayUtc = null;
        RewardTitleType = 0;
    }

    public bool IsRewardDay(DateTime? nowUtc = null)
    {
        if (IsSolo || !AnniversaryDayUtc.HasValue) return false;
        var today = (nowUtc ?? DateTime.UtcNow).Date;
        if (LastRewardDateUtc.HasValue && LastRewardDateUtc.Value.Date >= today)
            return false;
        return today.Day == AnniversaryDayUtc.Value.Day;
    }
}
